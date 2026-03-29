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
	}
}