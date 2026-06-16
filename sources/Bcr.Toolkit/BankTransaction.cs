namespace DustInTheWind.Bcr.Toolkit;

public class BankTransaction
{
	public decimal OpeningBalance { get; set; }

	public DateOnly CompletionDate { get; set; }

	public TimeOnly CompletionHour { get; set; }

	public string Details { get; set; }

	public string OperationReference { get; set; }

	public decimal DebitAmount { get; set; }

	public decimal CreditAmount { get; set; }

	public decimal DebitTotal { get; set; }

	public decimal CreditTotal { get; set; }

	public decimal FinalBalance { get; set; }

	public decimal BlockedAmounts { get; set; }

	public decimal AvailableBalance { get; set; }

	public decimal CreditAvailableLimit { get; set; }
}