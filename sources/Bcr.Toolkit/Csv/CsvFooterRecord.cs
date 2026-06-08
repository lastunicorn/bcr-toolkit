using System.Globalization;

namespace DustInTheWind.Bcr.Toolkit.Csv;

internal class CsvFooterRecord
{
	public DateOnly CompletionDate { get; set; }

	public decimal AvailableBalance { get; set; }

	public CsvFooterRecord(string[] record)
	{
		if (DateOnly.TryParse(record[10], new CultureInfo("ro-RO"), out DateOnly completionDate))
		{
			CompletionDate = completionDate;
		}
		else
		{
			throw new FooterLoadException($"Invalid date format for completion date: {record[10]}");
		}

		if (decimal.TryParse(record[20], CultureInfo.InvariantCulture, out decimal availableBalance))
		{
			AvailableBalance = availableBalance;
		}
		else
		{
			throw new FooterLoadException($"Invalid decimal format for available balance: {record[20]}");
		}
	}
}