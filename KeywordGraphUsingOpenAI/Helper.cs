using System.Globalization;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.AI;
using OpenAI.Embeddings;

namespace KeywordGraphUsingOpenAI
{
	public class Helper
	{
		public static string GetEnvironmentVariable(string key)
		{
			var value = Environment.GetEnvironmentVariable(key);
			if (string.IsNullOrWhiteSpace(value))
				throw new Exception($"Missing Environment Variable: {key}");
			return value!;
		}

		public static string CsvEscape(string s)
		{
			if (s is null) return "";
			var needQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n');
			if (needQuotes)
				return "\"" + s.Replace("\"", "\"\"") + "\"";
			return s;
		}

		public static void TransformAndSaveCsv(List<(string Word, float[] Vec)> data, string path)
		{
			if (data.Count == 0)
			{
				Console.WriteLine("No vectors to project.");
				return;
			}

			// Build an n x d matrix (double) and mean-center
			int n = data.Count;
			int d = data[0].Vec.Length;

			var X = Matrix<double>.Build.Dense(n, d, (i, j) => data[i].Vec[j]);
			// Mean-center columns
			var means = Vector<double>.Build.Dense(d);
			for (int j = 0; j < d; j++)
			{
				means[j] = X.Column(j).Average();
				for (int i = 0; i < n; i++)
				{
					X[i, j] -= means[j];
				}
			}

			// PCA via SVD of mean-centered X
			// X = U * S * V^T, principal directions = V columns
			var svd = X.Svd(computeVectors: true);
			var V = svd.VT.Transpose(); // d x d

			// Take first two principal components
			var V2 = V.SubMatrix(0, d, 0, 2); // d x 2
			var Y = X * V2; // n x 2

			// Write CSV: id,title,x,y (culture-invariant)
			using var sw = new StreamWriter(path, false, Encoding.UTF8);
			sw.WriteLine("WORD,X,Y");

			for (int i = 0; i < n; i++)
			{
				var x = Y[i, 0];
				var y = Y[i, 1];

				sw.WriteLine($"{CsvEscape(data[i].Word)}, {x.ToString(CultureInfo.InvariantCulture)},{y.ToString(CultureInfo.InvariantCulture)}");
			}
		}

		public static async Task GenerateEmbeddings(string[] words)
		{
			var openAiKey = GetEnvironmentVariable("OPEN_API_KEY");
			var vectors = new List<(string Word, float[] Vector)>();

			var embedder = new EmbeddingClient(
							  model: "text-embedding-3-small",
							  apiKey: openAiKey
						   ).AsIEmbeddingGenerator();

			foreach (var word in words)
			{
				var embeddings = await embedder.GenerateAsync(word,
									new Microsoft.Extensions.AI.EmbeddingGenerationOptions { Dimensions = 512 });

				var vector = embeddings.Vector.ToArray();
				vectors.Add((word, vector));
			}

			TransformAndSaveCsv(vectors, "animals.csv");
		}
	}
}