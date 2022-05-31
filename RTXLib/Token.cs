using RTXLib;

/// <summary>Class <c>Token</c> models a generic lexical token, used when parsing a source file.</summary>
public class Token
{
	public SourceLocation Location;

    public Token(SourceLocation location)
    {
        Location = location;
    }
}

/// <summary>Class <c>StopToken</c> models the token that identifies the end of a file.</summary>
public class StopToken : Token
{ 
	public StopToken(SourceLocation location) : base(location) {}
}

/// <summary>Class <c>SymbolToken</c> describes the token that models an identifier (e.g. a variable name).</summary>
public class SymbolToken : Token
{
    public char Symbol;

    public SymbolToken(SourceLocation location, char symbol) : base(location)
    {
        Symbol = symbol;
    }

    public override string ToString()
    {
        return $"{Symbol}";
    }
}

public enum KeywordEnum
{
    New = 1,
    Material = 2,
    Plane = 3,
    Sphere = 4,
    Diffuse = 5,
    Specular = 6,
    Uniform = 7,
    Checkered = 8,
    Image = 9,
    Identity = 10,
    Translation = 11,
    RotationX = 12,
    RotationY = 13,
    RotationZ = 14,
    Scaling = 15,
    Camera = 16,
    Orthogonal = 17,
    Perspective = 18,
    Float = 19
}



/// <summary>Class <c>KeywordToken</c> describes the token that models a keyword.</summary>
public class KeywordToken : Token
{
    public KeywordEnum Keyword;
    
    public KeywordToken(SourceLocation location, KeywordEnum keyword) : base(location)
    {
        Keyword = keyword;
    }

    public override string ToString()
    {
        return Keyword.ToString();
    }
}

/// <summary>Class <c>LiteralStringToken</c> describes the token that models a literal float number.</summary>
public class LiteralNumberToken : Token
{
    public float Value;

    public LiteralNumberToken(SourceLocation location, float value) : base(location)
    {
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

    public LiteralStringToken(SourceLocation location, string literalString) : base(location)
    {
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

    public IdentifierToken(SourceLocation location, string identifier) : base(location)
    {
        Identifier = identifier;
    }

    public override string ToString()
    {
        return Identifier;
    }
}