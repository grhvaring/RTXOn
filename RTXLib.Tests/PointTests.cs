using System.Numerics;
using Xunit;

namespace RTXLib.Tests;

public class PointTests
{

    [Fact]
    public void TestConstructor()
    {
        var zero = new Point(0,0,0);
        Assert.True(zero.IsClose(new Point()));
        
        var a = new Point(1.0f, 2.0f, 3.0f);
        var b = new Point(4.0f, 5.0f, 6.0f);
        Assert.True(a.IsClose(a));
        Assert.False(a.IsClose(b));

        var c = new Point(new Vector3(1, 2, 3));
        Assert.True(c.IsClose(new Point(1, 2, 3)));

        var d = new Point(c);
        Assert.True(d.IsClose(c));
    }

    [Fact]
    public void TestOperators()
    {
        var p1 = new Point(1, 2, 3);
        var v = new Vec(4, 6, 8);
        var p2 = new Point(4, 6, 8);
        var a = 2.0f;
        
        Assert.True((p1 + v).IsClose(new Point(5, 8, 11)));
        Assert.True((v + p1).IsClose(p1 + v));
        Assert.True((p2 - v).IsClose(new Point()));
        Assert.True((-p1).IsClose(new Point(-1, -2, -3)));
        Assert.True((p1 - v).IsClose(-(v - p1)));
        Assert.True((a * p1).IsClose(new Point(2, 4, 6)));
        Assert.True((p1 * a).IsClose(a * p1));
        Assert.True((p2 - p1).IsClose(new Vec(3, 4, 5)));
        Assert.True((p2 / a).IsClose(new Point(2, 3, 4)));
        Assert.True(p1.ToVec().IsClose(new Vec(1, 2, 3)));
    }
}