using RTXLib;

/// <summary>Class <c>SourceLocation</c> describes the position of a token in a source file.
/// The position of the token is defined by the name of the file , the number of the row (starting from 1) and the number of the column (starting from 1)
/// </summary>
public class SourceLocation
{
	/// <value>Property <c>FileName</c> represents the name of the source file or an empty string if the file name is missing for some reason.</value>
	public string FileName;

	/// <value>Property <c>LineNumber</c> represents the number of the row where the token is located in the source file. The row numbering starts from 1.</value>
	public int LineNumber;

	/// <value>Property <c>LineNumber</c> represents the number of the column where the token is located in the source file. The column numbering starts from 1.</value>
	public int ColumnNumber;

	/// <summary>Default constructor <c>SourceLocation</c> creates a <c>SourceLoacation</c> with an empty string as file name, 0 lines and 0 columns.</summary>
	public SourceLocation()
	{
		FileName = "";
		LineNumber = 0;
		ColumnNumber = 0;
	}

	/// <summary>Constructor <c>SourceLocation</c> creates a <c>SourceLoacation</c> with specified file name, number of lines and number of columns.</summary>
	public SourceLocation(string fileName, int lineNumber,  int columnNumber)
	{
		FileName = fileName;
		LineNumber = lineNumber;
		ColumnNumber = columnNumber;
	}

	/// <summary>Method <c>ShallowCopy</c> performs a shallow copy of a given <c>SourceLocation</c>.</summary>
	public SourceLocation ShallowCopy()
	{
		return (SourceLocation)this.MemberwiseClone();
	}
}
