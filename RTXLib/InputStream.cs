using RTXLib;

/// <summary>Class <c>InputStream</c> models a stream used to parse the file descriving the scene to be rendered.</summary>
public class InputStream
{
	public StreamReader Stream;
	public SourceLocation Location;
	public char SavedChar;
	public SourceLocation SavedLocation;
	public int Tabulations;

	public InputStream(StreamReader stream, string filename = "", int tabulations = 8)
	{
		Stream = stream;
		Location = new SourceLocation(filename, 1, 1);
		SavedChar = '\0';
		SavedLocation = Location;
		Tabulations = tabulations;

	}

	public void UpdatePosition(char ch)
	{
		// If nothing is read, do nothing
		if (ch == '\0')
			return;

		// If the newline character is read, add 1 line and return to column 1
		else if (ch == '\n')
		{
			Location.LineNumber += 1;
			Location.ColumnNumber = 1;
		}

		// If the tabulation character is read, add to the number of column the number specified by Tabulations
		// property
		else if (ch == '\t')
			Location.ColumnNumber += Tabulations;

		// Otherwise, add 1 column
		else
			Location.ColumnNumber = 1;
	}

	public char ReadChar()
	{
		char ch;

		if (SavedChar != '\0')
		{
			ch = SavedChar;
			SavedChar = '\0';
		}
		else
			ch = (char)Stream.Read();

	SavedLocation = Location.ShallowCopy();
	UpdatePosition(ch);
		return ch;
	}

	public void UnreadChar(char ch)
    {
		// In the example prof request to do an assert. Why ???
		SavedChar = ch;
		Location = SavedLocation.ShallowCopy();

	}
}
