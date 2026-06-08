namespace DustInTheWind.Bcr.Toolkit;

public class DocumentLoadException : Exception
{
	public DocumentLoadException()
		: base("Failed to load statement CSV file.")
	{
	}
	
	public DocumentLoadException(Exception innerException)
		: base("Failed to load statement CSV file.", innerException)
	{
	}


	public DocumentLoadException(string message)
		: base(message)
	{
	}

	public DocumentLoadException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}