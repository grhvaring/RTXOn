using Xunit;

namespace RTXLib.Tests;

public class ColorTests
{
    [Fact]
    public void TestAdd()
    {

        Color a = new Color(1.0f, 2.0f, 3.0f);
        Color b = new Color(5.0f, 6.0f, 7.0f);
        // C# convention: *first* the expected value, *then* the test value
        Assert.True((a+b).is_close(new Color(6.0f, 8.0f, 10.0f)));
    }
}