using System.Numerics;
using Xunit;

namespace RTXLib.Tests;

public class PointTests
{

    [Fact]
    public void TestConstructor()
    {
        Point zero = new(0,0,0);
        Assert.True(zero.IsClose(new Point()));
        
        Point a = new(1.0f, 2.0f, 3.0f);
        Point b = new(4.0f, 5.0f, 6.0f);
        Assert.True(a.IsClose(a));
        Assert.False(a.IsClose(b));

        Point c = new(new Vector3(1, 2, 3));
        Assert.True(c.IsClose(new Point(1, 2, 3)));

        Point d = new Point(c);
        Assert.True(d.IsClose(c));
    }

    [Fact]
    public void TestOperators()
    {
        Point p1 = new(1, 2, 3);
        Vec v = new(4, 6, 8);
        Point p2 = new(4, 6, 8);
        float a = 2;
        
        Assert.True((p1 + v).IsClose(new Point(5, 8, 11)));
        Assert.True((v + p1).IsClose(p1 + v));
        Assert.True((p2-v).IsClose(new Point()));
        Assert.True((-p1).IsClose(new Point(-1, -2, -3)));
        Assert.True((p1 - v).IsClose(-(v - p1)));
        Assert.True((a * p1).IsClose(new Point(2, 4, 6)));
        Assert.True((p1 * a).IsClose(a * p1));
        Assert.True((p2 - p1).IsClose(new Vec(3, 4, 5)));
        Assert.True((p2 / a).IsClose(new Point(2, 3, 4)));
        Assert.True(p1.ToVec().IsClose(new Vec(1, 2, 3)));
    }
}