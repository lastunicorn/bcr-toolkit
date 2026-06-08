namespace DustInTheWind.Bcr.Toolkit;

public class FooterLoadException : DocumentLoadException
{
	public FooterLoadException(string message)
		: base(BuildMessage(message))
	{
	}

	public FooterLoadException(string message, Exception innerException)
		: base(BuildMessage(message), innerException)
	{
	}

	public FooterLoadException(Exception innerException)
		: base(BuildMessage(null), innerException)
	{
	}

	private static string BuildMessage(string message)
	{
		return message == null
			? "The CSV footer is invalid."
			: $"The CSV footer is invalid. {message}";
	}
}