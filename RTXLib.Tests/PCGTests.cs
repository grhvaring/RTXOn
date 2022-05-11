namespace RTXLib.Tests;
using Xunit;

public class PCGTests
{
    [Fact]
    public void TestRandom()
    {
        var pcg = new PCG();
        
        Assert.True(pcg.State == 1753877967969059832);
        Assert.True(pcg.Inc == 109);

        var expected = new uint[] {
            2707161783, 2068313097,
            3122475824, 2211639955,
            3215226955, 3421331566
        };
        foreach (var n in expected) Assert.True(n == pcg.Random());
    }
}