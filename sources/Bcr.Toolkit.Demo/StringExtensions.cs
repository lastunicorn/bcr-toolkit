namespace DustInTheWind.Bcr.Toolkit.Demo;

internal static class StringExtensions
{
	public static string Truncate(this string value, int maxLength)
	{
		if (string.IsNullOrEmpty(value))
			return value;

		if (maxLength <= 0)
			return string.Empty;

		if (maxLength <= 3)
			return new string('.', maxLength);

		return value.Length <= maxLength
			? value
			: value.Substring(0, maxLength - 3) + "...";
	}
}