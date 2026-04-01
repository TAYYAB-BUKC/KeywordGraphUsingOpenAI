using KeywordGraphUsingOpenAI;

var words = new[] { "cat", "mouse", "lion", "tiger", "helicopter", "train", "blue", "carrot", "space" };

await Helper.GenerateEmbeddings(words);

Console.WriteLine("Embeddings Generated.");