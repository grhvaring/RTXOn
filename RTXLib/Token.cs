using RTXLib;

/// <summary>Class <c>Token</c> models a generic lexical token, used when parsing a source file.</summary>
public class Token
{
	public SourceLocation Location;
}

/// <summary>Class <c>StopToken</c> models the token that identifies the end of a file.</summary>
public class StopToken : Token
{ 
	public StopToken(SourceLocation location)
    {
        Location = location;
    }
}

/// <summary>Class <c>SymbolToken</c> describes the token that models an identifier (e.g. a variable name).</summary>
public class SymbolToken : Token
{
    public string Symbol;

    public SymbolToken(SourceLocation location, string symbol)
    {
        Location = location;
        Symbol = symbol;
    }

    public override string ToString()
    {
        return Symbol;
    }
}

/// <summary>Enum <c>KeywordEnum</c> enumerates all the tokens that model a keyword.</summary>
public enum KeywordEnum
{
    NEW = 1,
    MATERIAL = 2,
    PLANE = 3,
    SPHERE = 4,
    DIFFUSE = 5,
    SPECULAR = 6,
    UNIFORM = 7,
    CHECKERED = 8,
    IMAGE = 9,
    IDENTITY = 10,
    TRANSLATION = 11,
    ROTATION_X = 12,
    ROTATION_Y = 13,
    ROTATION_Z = 14,
    SCALING = 15,
    CAMERA = 16,
    ORTHOGONAL = 17,
    PERSPECTIVE = 18,
    FLOAT = 19
}

/// <summary>Class <c>KeywordToken</c> describes the token that models a keyword.</summary>
public class KeywordToken : Token
{
    public KeywordEnum Keyword;
    
    public KeywordToken(SourceLocation location, KeywordEnum keyword)
    {
        Location = location;
        Keyword = keyword;
    }

    // NOTE: To undestend how to use dictionaries and enum and how to correct itbe corrected !!!!!!!!!!!
    public override string ToString()
    {
        return "A";
    }
}

/// <summary>Class <c>LiteralStringToken</c> describes the token that models a literal float number.</summary>
public class LiteralNumberToken : Token
{
    public float Value;

    public LiteralNumberToken(SourceLocation location, float value)
    {
        Location = location;
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

}

/// <summary>Class <c>LiteralStringToken</c> describes the token that models a literal string.</summary>
public class LiteralStringToken : Token
{
    public string String;

    public LiteralStringToken(SourceLocation location, string literalString)
    {
        Location = location;
        String = literalString;
    }

    public override string ToString()
    {
        return String;
    }
}

/// <summary>Class <c>IdentifierToken</c> describes the token that models an identifier (e.g. a variable name).</summary>
public class IdentifierToken : Token 
{
    public string Identifier;

    public IdentifierToken(SourceLocation location, string identifier)
    {
        Location = location;
        Identifier = identifier;
    }

    public override string ToString()
    {
        return Identifier;
    }
}