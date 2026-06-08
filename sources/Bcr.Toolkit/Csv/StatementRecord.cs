namespace DustInTheWind.Bcr.Toolkit.Csv;

internal class StatementRecord
{
	public DateOnly IssuingDateOfTheStatement { get; init; }

	public TimeOnly IssuingTimeOfTheStatement { get; init; }

	public DateOnly StartingDate { get; init; }

	public DateOnly EndDate { get; init; }

	public Currency Currency { get; init; }

	public decimal? BnrExchangeRate { get; init; }

	public string StatementIssuedForAccount { get; init; }

	public string ProductType { get; init; }

	public string AccountOwner { get; init; }

	public decimal FirstOpeningAccountingBalance { get; init; }

	public DateOnly TransactionCompletionDate { get; init; }

	public TimeOnly TransactionCompletionHour { get; init; }

	public string TransactionDetails { get; init; }

	public string OperationReference { get; init; }

	public decimal DebitAmount { get; init; }

	public decimal CreditAmount { get; init; }

	public decimal TotalDebitAmount { get; init; }

	public decimal TotalCreditAmount { get; init; }

	public decimal FinalAccountingBalance { get; init; }

	public decimal BlockedAmounts { get; init; }

	public decimal AvailableBalance { get; init; }

	public decimal CreditLinesAvailableLimit { get; init; }
}