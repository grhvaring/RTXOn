using System.IO;
using Xunit.Abstractions;

namespace RTXLib.Tests;
using Xunit;

public class InputStreamTests
{
    // Visualization of outputs during a test
    private readonly ITestOutputHelper Output;

    public InputStreamTests(ITestOutputHelper output)
    {
        Output = output;
    }
    
    [Fact]
    public void TestInputFile()
    {
        using var reader = new StringReader("abc   \nd\nef");
        var stream = new InputStream(reader);
        
        Assert.True(stream.Location.LineNumber == 1);
        Assert.True(stream.Location.ColumnNumber == 1);

        Assert.True(stream.ReadChar() == 'a');
        Assert.True(stream.Location.LineNumber == 1);
        Assert.True(stream.Location.ColumnNumber == 2);
        
        Assert.True(stream.ReadChar() == 'b');
        Assert.True(stream.Location.LineNumber == 1);
        Assert.True(stream.Location.ColumnNumber == 3);
        
        Assert.True(stream.ReadChar() == 'c');
        Assert.True(stream.Location.LineNumber == 1);
        Assert.True(stream.Location.ColumnNumber == 4);
        
        stream.SkipWhitespacesAndComments();

        Assert.True(stream.ReadChar() == 'd');
        Assert.True(stream.Location.LineNumber == 2);
        Assert.True(stream.Location.ColumnNumber == 2);
        
        Assert.True(stream.ReadChar() == '\n');
        Assert.True(stream.Location.LineNumber == 3);
        Assert.True(stream.Location.ColumnNumber == 1);
        
        Assert.True(stream.ReadChar() == 'e');
        Assert.True(stream.Location.LineNumber == 3);
        Assert.True(stream.Location.ColumnNumber == 2);
        
        Assert.True(stream.ReadChar() == 'f');
        Assert.True(stream.Location.LineNumber == 3);
        Assert.True(stream.Location.ColumnNumber == 3);

        Assert.True(stream.ReadChar() == InputStream.EOF);
    }

    private void AssertIsKeyword(Token token, KeywordEnum keyword)
    {
        Assert.True(token is KeywordToken);
        var keywordToken = (KeywordToken) token;
        Assert.True(keywordToken.Keyword == keyword);
    }
    
    private void AssertIsIdentifier(Token token, string identifier)
    {
        Assert.True(token is IdentifierToken);
        var idToken = (IdentifierToken) token;
        Assert.True(idToken.Identifier == identifier);
    }
    
    private void AssertIsSymbol(Token token, char symbol)
    {
        Assert.True(token is SymbolToken);
        var symToken = (SymbolToken) token;
        Assert.True(symToken.Symbol == symbol);
    }
    
    private void AssertIsNumber(Token token, float number)
    {
        Assert.True(token is LiteralNumberToken);
        var numToken = (LiteralNumberToken) token;
        Assert.True(numToken.Value == number);
    }
    
    private void AssertIsString(Token token, string s)
    {
        Assert.True(token is LiteralStringToken);
        var stringToken = (LiteralStringToken) token;
        Assert.True(stringToken.String == s);
    }
    
    [Fact]
    public void TestLexer()
    {
        var s = @"
# This is a comment
# This is another comment
new material sky_material(
diffuse(image(""my file.pfm"")),
<5.0, 500.0, 300.0>
) # Comment at the end of the line";
        
        using var reader = new StringReader(s);
        var stream = new InputStream(reader);
        
        AssertIsKeyword(stream.ReadToken(), KeywordEnum.New);
        AssertIsKeyword(stream.ReadToken(), KeywordEnum.Material);
        AssertIsIdentifier(stream.ReadToken(), "sky_material");
        AssertIsSymbol(stream.ReadToken(), '(');
        AssertIsKeyword(stream.ReadToken(), KeywordEnum.Diffuse);
        AssertIsSymbol(stream.ReadToken(), '(');
        AssertIsKeyword(stream.ReadToken(), KeywordEnum.Image);
        AssertIsSymbol(stream.ReadToken(), '(');
        AssertIsString(stream.ReadToken(), "my file.pfm");
        AssertIsSymbol(stream.ReadToken(), ')');
        AssertIsSymbol(stream.ReadToken(), ')');
        AssertIsSymbol(stream.ReadToken(), ',');
        AssertIsSymbol(stream.ReadToken(), '<');
        AssertIsNumber(stream.ReadToken(), 5.0f);
        AssertIsSymbol(stream.ReadToken(), ',');
        AssertIsNumber(stream.ReadToken(), 500);
        AssertIsSymbol(stream.ReadToken(), ',');
        AssertIsNumber(stream.ReadToken(), 300);
        AssertIsSymbol(stream.ReadToken(), '>');
        AssertIsSymbol(stream.ReadToken(), ')');
    }
}