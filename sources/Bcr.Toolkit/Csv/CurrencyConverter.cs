using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace DustInTheWind.Bcr.Toolkit.Csv;

internal sealed class CurrencyConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new TypeConverterException(this, memberMapData, text, row.Context, "Currency code cannot be empty.");

        try
        {
            return new Currency(text);
        }
        catch (ArgumentException ex)
        {
            throw new TypeConverterException(this, memberMapData, text, row.Context, ex.Message, ex);
        }
    }

    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
    {
        return value is Currency transactionCurrency
            ? transactionCurrency.Value
            : base.ConvertToString(value, row, memberMapData);
    }
}

