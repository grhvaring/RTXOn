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

}