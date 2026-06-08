namespace DustInTheWind.Bcr.Toolkit.Csv;

internal enum CsvDocumentReadState
{
	HeaderRecord = 0,
	DataRecord,
	FooterRecord,
	Ended
}