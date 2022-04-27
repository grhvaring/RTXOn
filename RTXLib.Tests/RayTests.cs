namespace RTXLib.Tests;
using Xunit;

public class RayTests
{
    [Fact]
    public void TestIsClose()
    {
        var ray1 = new Ray(new Point(1, 2, 3), new Vec(5, 4, -1));
        var ray2 = new Ray(new Point(1, 2, 3), new Vec(5, 4, -1));
        var ray3 = new Ray(new Point(5, 1, 4), new Vec(3, 9, 4));
        
        Assert.True(ray1.IsClose(ray2));
        Assert.False(ray1.IsClose(ray3));
    }

    [Fact]
    public void TestAt()
    {
        var ray = new Ray(new Point(1, 2, 3), new Vec(4, 2, 1));
        Assert.True(ray.At(0).IsClose(ray.Origin));
        Assert.True(ray.At(1).IsClose(new Point(5, 4, 4)));
        Assert.True(ray.At(2).IsClose(new Point(9, 6, 5)));
    }

    [Fact]
    public void TestTransform()
    {
        var ray = new Ray(new Point(1, 2, 3), new Vec(6, 5, 4));
        var transformation = Transformation.Translation(10, 11, 12) * Transformation.RotationX(90);
        var transformed = ray.Transform(transformation);
        
        Assert.True(transformed.Origin.IsClose(new Point(11, 8, 14)));
        Assert.True(transformed.Dir.IsClose(new Vec(6, -4, 5)));
    }
}