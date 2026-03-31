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
	}
}