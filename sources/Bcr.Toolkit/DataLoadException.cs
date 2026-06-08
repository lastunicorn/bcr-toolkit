namespace DustInTheWind.Bcr.Toolkit;

public class DataLoadException : DocumentLoadException
{
    public DataLoadException(Exception innerException)
        : base("The CSV data is invalid.", innerException)
    {
    }
}

