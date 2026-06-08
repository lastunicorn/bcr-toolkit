using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.Bcr.Toolkit.Demo;

internal static class Program
{
	public static async Task Main(string[] args)
	{
		const string fileName = "statement.csv";

		try
		{
			StatementDocument document = await StatementDocument.LoadFromFileAsync(fileName);

			DisplayStatementInfo(document);
			Console.WriteLine();

			DisplayTransactions(document);
		}
		catch (DocumentLoadException ex)
		{
			await Console.Error.WriteLineAsync($"Failed to read '{fileName}': {ex.Message}");
			Environment.ExitCode = 1;
		}
		catch (Exception ex)
		{
			await Console.Error.WriteLineAsync($"Unexpected error: {ex.Message}");
			Environment.ExitCode = 1;
		}
	}

	private static void DisplayStatementInfo(StatementDocument document)
	{
		DataGrid dataGrid = new()
		{
			Title = "Statement Document",
			BorderTemplate = BorderTemplate.PlusMinusBorderTemplate
		};

		dataGrid.Columns.Add("Name");
		dataGrid.Columns.Add("Value");

		dataGrid.Rows.Add("Issuing Date", document.IssuingDate.ToString("yyyy-MM-dd"));
		dataGrid.Rows.Add("Issuing Time", document.IssuingTime.ToString("HH:mm:ss"));
		dataGrid.Rows.Add("Starting Date", document.StartingDate.ToString("yyyy-MM-dd"));
		dataGrid.Rows.Add("End Date", document.EndDate.ToString("yyyy-MM-dd"));
		dataGrid.Rows.Add("Currency", document.Currency.ToString());
		dataGrid.Rows.Add("Bnr Exchange Rate", document.BnrExchangeRate.ToString());
		dataGrid.Rows.Add("Bank Account", document.BankAccount);
		dataGrid.Rows.Add("Product Type", document.ProductType);
		dataGrid.Rows.Add("Account Owner", document.AccountOwner);
		dataGrid.Rows.Add("End Balance", document.EndBalance);

		dataGrid.Display();
	}

	private static void DisplayTransactions(StatementDocument document)
	{
		DataGrid dataGrid = new()
		{
			Title = "Transactions",
			BorderTemplate = BorderTemplate.PlusMinusBorderTemplate,
			Footer = $"Count: {document.Count}"
		};

		dataGrid.Columns.Add("Opening\nBalance", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Completion\nDate");
		dataGrid.Columns.Add("Completion\nHour");
		dataGrid.Columns.Add("Details");
		dataGrid.Columns.Add("Operation\nReference");
		dataGrid.Columns.Add("Debit\nAmount", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Credit\nAmount", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Debit\nTotal", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Credit\nTotal", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Final\nBalance", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Blocked\nAmounts", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Available\nBalance", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Credit\nAvailable\nLimit", HorizontalAlignment.Right);

		foreach (BankTransaction bankTransaction in document)
		{
			dataGrid.Rows.Add(
				bankTransaction.OpeningBalance.ToString(),
				bankTransaction.CompletionDate.ToString("yyyy-MM-dd"),
				bankTransaction.CompletionHour.ToString("HH:mm:ss"),
				bankTransaction.Details.Truncate(20),
				bankTransaction.OperationReference.Truncate(20),
				bankTransaction.DebitAmount.ToString(),
				bankTransaction.CreditAmount.ToString(),
				bankTransaction.DebitTotal.ToString(),
				bankTransaction.CreditTotal.ToString(),
				bankTransaction.FinalBalance.ToString(),
				bankTransaction.BlockedAmounts.ToString(),
				bankTransaction.AvailableBalance.ToString(),
				bankTransaction.CreditAvailableLimit.ToString());
		}

		dataGrid.Display();
	}
}