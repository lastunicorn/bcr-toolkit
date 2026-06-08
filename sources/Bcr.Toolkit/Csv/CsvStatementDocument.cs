using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MissingFieldException = CsvHelper.MissingFieldException;

namespace DustInTheWind.Bcr.Toolkit.Csv;

internal class CsvStatementDocument
{
	private readonly CsvReader csvReader;
	private bool classMapRegistered;
	private CsvDocumentReadState state;

	public CsvStatementDocument(TextReader textReader)
	{
		if (textReader == null) throw new ArgumentNullException(nameof(textReader));

		CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
		{
			HasHeaderRecord = true,
			IgnoreBlankLines = true,
			TrimOptions = TrimOptions.Trim,
			PrepareHeaderForMatch = args => args.Header.Trim()
		};

		csvReader = new CsvReader(textReader, csvConfiguration);
	}

	public string[] ReadHeaders()
	{
		if (state != CsvDocumentReadState.HeaderRecord)
			throw new InvalidOperationException("Header row was already read.");

		try
		{
			csvReader.Read();
			csvReader.ReadHeader();

			state = CsvDocumentReadState.DataRecord;
			return csvReader.HeaderRecord ?? [];
		}
		catch (HeaderValidationException ex)
		{
			throw new HeaderLoadException(ex);
		}
		catch (ReaderException ex) when (ex is MissingFieldException || ex.InnerException is HeaderValidationException or MissingFieldException)
		{
			throw new HeaderLoadException(ex.InnerException ?? ex);
		}
		catch (ReaderException ex)
		{
			throw new DataLoadException(ex);
		}
		catch (TypeConverterException ex)
		{
			throw new DataLoadException(ex);
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public async IAsyncEnumerable<StatementRecord> ReadTransactions()
	{
		if (state == CsvDocumentReadState.HeaderRecord)
			_ = ReadHeaders();

		if (state == CsvDocumentReadState.Ended)
			yield break;

		if (state != CsvDocumentReadState.DataRecord)
			throw new InvalidOperationException("CSV document is not in a valid state to read transactions.");

		if (!classMapRegistered)
		{
			csvReader.Context.RegisterClassMap(new StatementRecordMap());
			classMapRegistered = true;
		}

		while (true)
		{
			bool hasRecord = await MoveToNextRecord();

			if (hasRecord && state == CsvDocumentReadState.FooterRecord)
			{
				throw new DocumentLoadException("Footer row was already read.");
			}

			if (!hasRecord)
			{
				state = CsvDocumentReadState.Ended;
				yield break;
			}

			bool isFooterRecord = csvReader.Parser.Record is { Length: > 0 } record && string.IsNullOrWhiteSpace(record[0]);
			if (isFooterRecord)
			{
				state = CsvDocumentReadState.FooterRecord;
				yield break;
			}

			StatementRecord statementRecord = GetStatementRecord();
			yield return statementRecord;
		}
	}

	private async Task<bool> MoveToNextRecord()
	{
		try
		{
			return await csvReader.ReadAsync();
		}
		catch (HeaderValidationException ex)
		{
			throw new HeaderLoadException(ex);
		}
		catch (ReaderException ex) when (ex is MissingFieldException || ex.InnerException is HeaderValidationException or MissingFieldException)
		{
			throw new HeaderLoadException(ex.InnerException ?? ex);
		}
		catch (ReaderException ex)
		{
			throw new DataLoadException(ex);
		}
		catch (TypeConverterException ex)
		{
			throw new DataLoadException(ex);
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	private StatementRecord GetStatementRecord()
	{
		try
		{
			return csvReader.GetRecord<StatementRecord>();
		}
		catch (HeaderValidationException ex)
		{
			throw new HeaderLoadException(ex);
		}
		catch (ReaderException ex) when (ex is MissingFieldException || ex.InnerException is HeaderValidationException or MissingFieldException)
		{
			throw new HeaderLoadException(ex.InnerException ?? ex);
		}
		catch (ReaderException ex)
		{
			throw new DataLoadException(ex);
		}
		catch (TypeConverterException ex)
		{
			throw new DataLoadException(ex);
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public CsvFooterRecord ReadFooterRecord()
	{
		if (state != CsvDocumentReadState.FooterRecord)
			throw new InvalidOperationException("Document is not in a valid state to read the footer record.");

		CsvFooterRecord csvFooterRecord = new(csvReader.Parser.Record);

		state = CsvDocumentReadState.Ended;

		return csvFooterRecord;
	}

	public void CheckDocumentHasNoMoreRecordsAfterFooterRecord()
	{
		if (state != CsvDocumentReadState.Ended)
			throw new InvalidOperationException("Document is not in a valid state to check for more records after the footer record.");

		bool isAnotherRecord = csvReader.Read();

		if (isAnotherRecord)
			throw new DocumentLoadException("Document contains more records after the footer record.");
	}
}