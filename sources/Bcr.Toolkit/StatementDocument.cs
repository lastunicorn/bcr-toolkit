using System.Collections.ObjectModel;
using DustInTheWind.Bcr.Toolkit.Csv;

namespace DustInTheWind.Bcr.Toolkit;

public class StatementDocument : Collection<BankTransaction>
{
	public DateOnly IssuingDate { get; set; }

	public TimeOnly IssuingTime { get; set; }

	public DateOnly StartingDate { get; set; }

	public DateOnly EndDate { get; set; }

	public Currency Currency { get; set; }

	public decimal? BnrExchangeRate { get; set; }

	public string BankAccount { get; set; }

	public string ProductType { get; set; }

	public string AccountOwner { get; set; }

	public decimal EndBalance { get; set; }

	public static async Task<StatementDocument> LoadFromFileAsync(string filePath)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		try
		{
			using StreamReader streamReader = File.OpenText(filePath);
			return await LoadInternalAsync(streamReader);
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public static async Task<StatementDocument> LoadAsync(string csv)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(csv);

		try
		{
			using StringReader stringReader = new(csv);
			return await LoadInternalAsync(stringReader);
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public static async Task<StatementDocument> LoadAsync(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream);

		try
		{
			using StreamReader streamReader = new(stream);
			return await LoadInternalAsync(streamReader);
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public static async Task<StatementDocument> LoadAsync(FileInfo fileInfo)
	{
		ArgumentNullException.ThrowIfNull(fileInfo);

		try
		{
			using StreamReader streamReader = fileInfo.OpenText();
			return await LoadInternalAsync(streamReader);
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public static Task<StatementDocument> LoadAsync(StreamReader streamReader)
	{
		ArgumentNullException.ThrowIfNull(streamReader);

		return LoadInternalAsync(streamReader);
	}

	public static Task<StatementDocument> LoadAsync(TextReader textReader)
	{
		ArgumentNullException.ThrowIfNull(textReader);

		return LoadInternalAsync(textReader);
	}

	private static async Task<StatementDocument> LoadInternalAsync(TextReader textReader)
	{
		try
		{
			StatementDocument document = [];
			CsvStatementDocument csvStatementDocument = new(textReader);

			bool isFirstRecord = true;

			await foreach (StatementRecord statementRecord in csvStatementDocument.ReadTransactions())
			{
				if (isFirstRecord)
				{
					document.IssuingDate = statementRecord.IssuingDateOfTheStatement;
					document.IssuingTime = statementRecord.IssuingTimeOfTheStatement;
					document.StartingDate = statementRecord.StartingDate;
					document.EndDate = statementRecord.EndDate;
					document.Currency = statementRecord.Currency;
					document.BnrExchangeRate = statementRecord.BnrExchangeRate;
					document.BankAccount = statementRecord.StatementIssuedForAccount;
					document.ProductType = statementRecord.ProductType;
					document.AccountOwner = statementRecord.AccountOwner;

					isFirstRecord = false;
				}
				else
				{
					bool isRecordFromSameStatement =
						statementRecord.IssuingDateOfTheStatement == document.IssuingDate &&
						statementRecord.IssuingTimeOfTheStatement == document.IssuingTime &&
						statementRecord.StartingDate == document.StartingDate &&
						statementRecord.EndDate == document.EndDate &&
						statementRecord.Currency == document.Currency &&
						statementRecord.BnrExchangeRate == document.BnrExchangeRate &&
						statementRecord.StatementIssuedForAccount == document.BankAccount &&
						statementRecord.ProductType == document.ProductType &&
						statementRecord.AccountOwner == document.AccountOwner;

					if (!isRecordFromSameStatement)
						throw new DocumentLoadException("The statement document contains records from different statements.");
				}

				document.Add(new BankTransaction
				{
					OpeningBalance = statementRecord.FirstOpeningAccountingBalance,
					CompletionDate = statementRecord.TransactionCompletionDate,
					CompletionHour = statementRecord.TransactionCompletionHour,
					Details = statementRecord.TransactionDetails,
					DebitAmount = statementRecord.DebitAmount,
					CreditAmount = statementRecord.CreditAmount,
					DebitTotal = statementRecord.TotalDebitAmount,
					CreditTotal = statementRecord.TotalCreditAmount,
					FinalBalance = statementRecord.FinalAccountingBalance,
					BlockedAmounts = statementRecord.BlockedAmounts,
					AvailableBalance = statementRecord.AvailableBalance,
					CreditAvailableLimit = statementRecord.CreditLinesAvailableLimit
				});
			}

			CsvFooterRecord csvFooterRecord = csvStatementDocument.ReadFooterRecord();
			if (csvFooterRecord.CompletionDate != document.IssuingDate)
				throw new DocumentLoadException("The statement document contains a footer record with a completion date that does not match the issuing date of the statement.");

			document.EndBalance = csvFooterRecord.AvailableBalance;

			csvStatementDocument.CheckDocumentHasNoMoreRecordsAfterFooterRecord();

			return document;
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}
}