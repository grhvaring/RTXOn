using RTXLib;
using System.Globalization;

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

	/// <summary>Method <c>UpdatePosition</c> updates the <c>Location</c> after reading a character from the stream.</summary>
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

	/// <summary>Method <c>ReadChar</c> reads a new character from the stream and save his location.</summary>
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

		// In the example professor request to do an assert. Why ???
		SavedChar = ch;
		Location = SavedLocation.ShallowCopy();

	}

	/// <summary>Method <c>SkipWhitespaceAndComments</c> read (but doesn't save) characters until a non-comment/non-whitespace character is found.</summary>
	public void SkipWhitespaceAndComments()
    {
		char[] WHITESPACE = { ' ', '\t', '\n', '\r' };
		// NOTE: maybe item should be corrected
		char[] ENDOFLINE = { '\0', '\n', '\r' };

		char ch = ReadChar();

		// Check if the character is a whitespace or a comment
		while (Array.Exists(WHITESPACE, whiteSpaceCh => whiteSpaceCh == ch) || (ch == '#'))
		{
			// If ch is a comment keep reading
			if (ch == '#')
				// Check that ch is different from end of line character and continue execution
				while(!Array.Exists(ENDOFLINE, endOfLineCh => endOfLineCh == ch))
					continue;

			ch = ReadChar();

			// If end of file is reached, stop the function
			if (ch == '\0')
				return;
        }

		UnreadChar(ch);
    }

	/// <summary>Method <c>ParseStringTokens</c> parse the character that form a literal string. If the string is not close an error is raised.</summary>
	public LiteralStringToken ParseStringToken(SourceLocation tokenLocation)
    {
		string token = "";

		while(true)
		{
			char ch = ReadChar();

			// If the ending " has been reached stop the parsing of the file
			if (ch == '"')
				break;

			// If an empty character has been read whitout reaching the ending " raise an error
			if (ch == '\0')
				continue;
			// NOTE: to be implemented with GrammarError

			
			token += ch;
        }

		return new LiteralStringToken(tokenLocation, token);
    }

	public LiteralNumberToken ParseFloatToken(string firstCharacter, SourceLocation tokenLocation)
    {
		char[] SCIENTIFIC_NOTATION_BASE = { 'e', 'E' };

		string token = firstCharacter;

		while(true)
        {
			char ch = ReadChar();

			// If the character read is not a 0-9 digit, a decimal separetor or a base of scientific notation stop the parsing
			if (!(Char.IsDigit(ch) || (ch == '.') || Array.Exists(SCIENTIFIC_NOTATION_BASE, scientificNotationBaseCh => scientificNotationBaseCh == ch)))
            {
				UnreadChar(ch);
				break;
            }

			token += ch;
		}

		float value;

		// Convert
		try
		{
			value = Single.Parse(token, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent);
		}
		catch { throw; } //NOTE: to be changed with actual exception

		return new LiteralNumberToken(tokenLocation, value);
    }

	// To be changed to a UNION for C#
	public IdentifierToken ParseKeywordOrIdentifierToken(string firstCharacter, SourceLocation tokenLocation)
    {
		string token = firstCharacter;

		while (true)
		{
			char ch = ReadChar();

			// If the character read is not a 0-9 digit, a letter or a _ stop the parsing
			if (!(Char.IsLetterOrDigit(ch) || (ch == '_')))
			{
				UnreadChar(ch);
				break;
			}

			token += ch;

            try 
			{
				// Check if it is in the keyword
				return new IdentifierToken(tokenLocation, token);
			}
			catch {  return new IdentifierToken(tokenLocation, token);}
		}
	}
}
