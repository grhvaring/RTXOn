namespace RTXLib;

public static class Parser
{
    /// <summary>
    /// Read a token from `input_file` and check that it matches `symbol`.
    /// </summary>
    public static void ExpectSymbol(InputStream inputFile, char symbol)
    {
        var token = inputFile.ReadToken();
        var message = $"Got {token} when expecting {symbol}";
        var e = new GrammarError(token.Location, message);
        if (token is not SymbolToken) throw e;
        var symToken = (SymbolToken) token;
        if (symToken.Symbol != symbol) throw e;
    }

    /// <summary>
    /// Read a token from `input_file` and check if it is contained in `keywords`.
    /// </summary>
    /// <returns>Keyword as KeywordEnum object</returns>
    public static KeywordEnum ExpectKeywords(InputStream inputFile, KeywordEnum[] keywords)
    {
        var token = inputFile.ReadToken();

        if (token is not KeywordToken)
            throw new GrammarError(token.Location, $"Keyword expected, got a '{token}' instead.");

        var keywordToken = (KeywordToken) token;

        if (!keywords.Contains(keywordToken.Keyword))
            throw new GrammarError(token.Location, $"Expected a keyword in the list {string.Join(", ", keywords)}");

        return keywordToken.Keyword;
    }

    /// <summary>
    /// Read a token from `input_file` and check if it is a string.
    /// </summary>
    /// <returns>String</returns>
    public static string ExpectString(InputStream inputFile)
    {
        var token = inputFile.ReadToken();

        if (token is not LiteralStringToken)
            throw new GrammarError(token.Location, $"Expected a string, got a '{token}' instead.");

        var literalStringToken = (LiteralStringToken) token;

        return literalStringToken.String;
    }

    /// <summary>
    /// Read a token from `input_file` and check if it is an identifier.
    /// </summary>
    /// <returns>Indentifier as a string</returns>
    public static string ExpectIdentifier(InputStream inputFile)
    {
        var token = inputFile.ReadToken();

        if (token is not IdentifierToken)
            throw new GrammarError(token.Location, $"Expected a identifier, got a '{token}' instead.");

        var identifierToken = (IdentifierToken)token;

        return identifierToken.Identifier;
    }

    public static float ExpectNumber(InputStream stream, Scene scene)
    {
        var token = stream.ReadToken();
        if (token is LiteralNumberToken numberToken) return numberToken.Value;
        if (token is IdentifierToken idToken)
        {
            var variableName = idToken.Identifier;
            if (!scene.FloatVariables.ContainsKey(variableName)) 
                throw new GrammarError(idToken.Location, $"Unknown variable {variableName}.");
            return scene.FloatVariables[variableName];
        }

        throw new GrammarError(token.Location, $"Expected a number, got a {token}.");
    }

    public static Color ParseColor(InputStream stream, Scene scene)
    {
        ExpectSymbol(stream, '[');
        var r = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ',');
        var g = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ',');
        var b = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ']');
        return new Color(r, g, b);
    }
    
    public static Vec ParseVector(InputStream stream, Scene scene)
    {
        ExpectSymbol(stream, '[');
        var x = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ',');
        var y = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ',');
        var z = ExpectNumber(stream, scene);
        ExpectSymbol(stream, ']');
        return new Vec(x, y, z);
    }

    public static Pigment ParsePigment(InputStream stream, Scene scene)
    {
        return new UniformPigment();
    }

    public static BRDF ParseBRDF(InputStream stream, Scene scene)
    {
        KeywordEnum [] BRDFKEYWORDS = {KeywordEnum.Diffuse, KeywordEnum.Uniform};

        var brdfKeyword = ExpectKeywords(stream, BRDFKEYWORDS);
        ExpectSymbol(stream, '(');
        var pigment = ParsePigment(stream, scene);
        ExpectSymbol(stream, '(');

        if (brdfKeyword == KeywordEnum.Diffuse)
            return new DiffuseBRDF(pigment);

        if (brdfKeyword == KeywordEnum.Uniform)
            return new SpecularBRDF(pigment);

        // NOTE: to be changed and to verify how to handle assert in C#
        throw new Exception("This line should be unreachable");
        
    }

    public static (string, Material) ParseMaterial(InputStream stream, Scene scene)
    {
        var name = ExpectIdentifier(stream);

        ExpectSymbol(stream, '(');
        var brdf = ParseBRDF(stream, scene);
        ExpectSymbol(stream, ',');
        var emittedRadiance = ParsePigment(stream, scene);
        ExpectSymbol(stream, ')');

        var material = new Material(brdf, emittedRadiance);

        return (name, material);
    }

    public static Transformation ParseTransformation(InputStream stream, Scene scene)
    {
        KeywordEnum[] TRANSFORMATIONS = 
        { KeywordEnum.Identity,
          KeywordEnum.Translation,
          KeywordEnum.RotationX,
          KeywordEnum.RotationY,
        
        
        
        };
    }
}