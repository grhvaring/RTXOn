using RTXLib;
using System.Globalization;
using Xunit;

/// <summary>Class <c>InputStream</c> models a stream used to parse the file describing the scene to be rendered.</summary>
public class InputStream
{
	public const char EOF = (char)0xFFFF; // TextReader default return value when no character is read
	private const string whitespaces = " \t\n\r";
	private const string symbols = "()<>[],*";
	public TextReader Stream;
	public SourceLocation Location;
	public char SavedChar;
	public SourceLocation SavedLocation;
	public int Tabulations;
	public Token? SavedToken;

	public InputStream(TextReader stream, string filename = "", int tabulations = 8)
	{
		Stream = stream;
		Location = new SourceLocation(filename, 1, 1);
		SavedChar = '\0';
		SavedLocation = Location;
		Tabulations = tabulations;
		SavedToken = null;
	}

	/// <summary>Method <c>UpdatePosition</c> updates the <c>Location</c> after reading a character from the stream.</summary>
	public void UpdatePosition(char ch)
	{
		// end of file
		if (ch == EOF) return;
		if (ch == '\n')
		{
			// newline
			Location.LineNumber += 1;
			Location.ColumnNumber = 1;
		}
		else if (ch == '\t') Location.ColumnNumber += Tabulations; // tab
		else Location.ColumnNumber += 1; // any other character
	}

	public Token ReadToken()
	{
		SkipWhitespacesAndComments();
		var ch = ReadChar();

		if (SavedToken != null)
		{
			var result = SavedToken;
			SavedToken = null;
			return result;
		}
		
		// end of file
		if (ch == EOF) return new StopToken(Location);

		var tokenLocation = Location.ShallowCopy();
		
		if (symbols.Contains(ch)) return new SymbolToken(tokenLocation, ch);
		if (char.IsNumber(ch)) return ParseFloatToken(ch, tokenLocation);
		if (ch == '"') return ParseStringToken(tokenLocation);
		if (char.IsLetter(ch)) return ParseKeywordOrIdentifierToken(ch, tokenLocation);
		
		// if none of the previous checks returned a valid token
		throw new GrammarError(tokenLocation, $"Invalid character {ch}");
	}

	public void UnreadToken(Token token)
	{
		Assert.True(SavedToken == null);
		SavedToken = token;
	}

	/// <summary>Method <c>ReadChar</c> reads a new character from the stream and saves his location.</summary>
	public char ReadChar()
	{
		char ch;

		if (SavedChar != '\0')
		{
			ch = SavedChar;
			SavedChar = '\0';
		}
		else ch = (char)Stream.Read();

		SavedLocation = Location.ShallowCopy();
		UpdatePosition(ch);
		return ch;
	}

	public void UnreadChar(char ch)
    {
	    Assert.True(SavedChar == '\0');
		SavedChar = ch;
		Location = SavedLocation.ShallowCopy();
    }

	/// <summary>Method <c>ParseStringTokens</c> parse the character that form a literal string. If the string is not close an error is raised.</summary>
	public LiteralStringToken ParseStringToken(SourceLocation tokenLocation)
    {
		var token = "";

		while(true)
		{
			var ch = ReadChar();

			// stop the parsing at the character " (double quote)
			if (ch == '"') break;

			// raise an error if an empty character has been read without reaching the end
			if (ch == EOF) continue;
			// NOTE: to be implemented with GrammarError
			token += ch;
        }

		return new LiteralStringToken(tokenLocation, token);
    }

	public LiteralNumberToken ParseFloatToken(char firstCharacter, SourceLocation tokenLocation)
    {
		char[] SCIENTIFIC_NOTATION_BASE = { 'e', 'E' };

		var token = firstCharacter.ToString();

		while(true)
        {
			var ch = ReadChar();

			// If the character read is not a 0-9 digit, a decimal separetor or a base of scientific notation stop the parsing
			if (!(Char.IsDigit(ch) || ch == '.' || SCIENTIFIC_NOTATION_BASE.Contains(ch)))
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
			value = float.Parse(token, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent);
		}
		catch
		{
			throw new GrammarError(tokenLocation, $"The string {token} could not be converted into a number."); 
		}

		return new LiteralNumberToken(tokenLocation, value);
    }

	// To be changed to a UNION for C#
	public Token ParseKeywordOrIdentifierToken(char firstCharacter, SourceLocation tokenLocation)
	{
		string token = firstCharacter.ToString();

		while (true)
		{
			var ch = ReadChar();

			// If the character read is not a 0-9 digit, a letter or a _ stop the parsing
			if (!(char.IsLetterOrDigit(ch) || ch == '_'))
			{
				UnreadChar(ch);
				break;
			}
			token += ch;
		}

		try
		{
			return new KeywordToken(tokenLocation, MyLib.Keywords[token]);
		}
		catch (KeyNotFoundException)
		{
			return new IdentifierToken(tokenLocation, token);
		}
	}

	public void SkipWhitespacesAndComments()
	{
		const string whitespaces = " \t\n\r";
		var ch = ReadChar();
		while (whitespaces.Contains(ch) || ch == '#')
		{
			// read the comment til the end
			if (ch == '#')
				while (!"\r\n".Contains(ReadChar()))
					continue;
			ch = ReadChar();
			// stop reading if at the end of file
			if (ch == EOF) return;
		}
		UnreadChar(ch);
	}
}
