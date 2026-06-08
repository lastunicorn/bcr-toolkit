using CsvHelper.Configuration;

namespace DustInTheWind.Bcr.Toolkit.Csv;

internal sealed class StatementRecordMap : ClassMap<StatementRecord>
{
	public StatementRecordMap()
	{
		Map(x => x.IssuingDateOfTheStatement)
			.Name("Issuing date of the statement")
			.TypeConverterOption.Format("dd.MM.yyyy");
		Map(x => x.IssuingTimeOfTheStatement)
			.Name("Issuing time of the statement")
			.TypeConverterOption.Format("HH:mm");

		Map(x => x.StartingDate)
			.Name("Starting date")
			.TypeConverterOption.Format("dd.MM.yyyy");

		Map(x => x.EndDate)
			.Name("End date")
			.TypeConverterOption.Format("dd.MM.yyyy");

		Map(x => x.Currency)
			.Name("Currency")
			.TypeConverter<CurrencyConverter>();

		Map(x => x.BnrExchangeRate)
			.Name("BNR exchange rate");

		Map(x => x.StatementIssuedForAccount)
			.Name("Statement issued for account");

		Map(x => x.ProductType)
			.Name("Product type");

		Map(x => x.AccountOwner)
			.Name("Account owner");

		Map(x => x.FirstOpeningAccountingBalance)
			.Name("First opening accounting balance");

		Map(x => x.TransactionCompletionDate)
			.Name("Transaction completion date")
			.TypeConverterOption.Format("dd.MM.yyyy");

		Map(x => x.TransactionCompletionHour)
			.Name("Transaction completion hour")
			.TypeConverterOption.Format("HH:mm");

		Map(x => x.TransactionDetails)
			.Name("Transaction's details");

		Map(x => x.OperationReference)
			.Name("Operation's reference");

		Map(x => x.DebitAmount)
			.Name("Debit (amount)");

		Map(x => x.CreditAmount)
			.Name("Credit (amount)");

		Map(x => x.TotalDebitAmount)
			.Name("Total debit (amount)");

		Map(x => x.TotalCreditAmount)
			.Name("Total credit (amount)");

		Map(x => x.FinalAccountingBalance)
			.Name("Final accounting balance");

		Map(x => x.BlockedAmounts)
			.Name("Blocked amounts");

		Map(x => x.AvailableBalance)
			.Name("Available balance");

		Map(x => x.CreditLinesAvailableLimit)
			.Name("Credit lines available limit");
	}
}