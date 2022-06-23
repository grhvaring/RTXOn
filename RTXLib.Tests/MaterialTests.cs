using Xunit.Abstractions;

namespace RTXLib.Tests;
using Xunit;

public class MaterialTests
{
    private readonly ITestOutputHelper Output;

    public MaterialTests(ITestOutputHelper output)
    {
        Output = output;
    }

    private static Material material = new();
    
    [Fact]
    public void TestDefaultMaterial()
    {
        var ez = new Normal(0, 0, 1);
        var outDir = Vec.Ez;
        var northPole = new Vec2D(0, 0);
        Assert.True(material.BRDF.Eval(ez, -outDir, outDir, northPole).IsClose(Color.BLACK));
    }
    
    /*
    [Fact]
    public void TestRedMaterial()
    {
        // ADD RANDOM DIRECTION TESTING
        var redMaterial = new Material(new UniformPigment(1, 0, 0));
        var ez = new Normal(0, 0, 1);
        var pcg = new PCG();
        var inDir = new Vec(pcg.RandomFloat(), pcg.RandomFloat(), -pcg.RandomFloat());
        var outDir = new Vec(pcg.RandomFloat(), pcg.RandomFloat(), pcg.RandomFloat());
        var northPole = new Vec2D(0, 0);
        var red = new Color(1, 0, 0);
        Assert.True(material.BRDF.Eval(ez, inDir, outDir, northPole).IsClose(red));
    }*/
}