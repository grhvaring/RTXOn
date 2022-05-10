namespace RTXLib.Tests;
using Xunit;
using Xunit.Abstractions;

public class SphereTests
{
    // Visualization of outputs during a test
    private readonly ITestOutputHelper _testOutputHelper;

    public SphereTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public void TestRayIntersection()
    {
        var ray = new Ray(new Point(0, 0, 2), -Vec.Ez);
        var sphere = new Sphere();
        Assert.True(sphere.RayIntersection(ray).HasValue);
        
        var hitRecord = sphere.RayIntersection(ray)!.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(0, 0, 1)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(0, 0, 1)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0,0));
        Assert.True(MyLib.IsZero(hitRecord.T - 1));
        
        ray = new Ray(new Point(3, 0, 0), -Vec.Ex);
        Assert.True(sphere.RayIntersection(ray).HasValue);

        hitRecord = sphere.RayIntersection(ray)!.Value;
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(1, 0, 0)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(1, 0, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0, 0.5f));
        Assert.True(MyLib.IsZero(hitRecord.T - 2));
        
        ray = new Ray(new Point(0, 0, 0), Vec.Ex);
        Assert.True(sphere.RayIntersection(ray).HasValue);
        
        hitRecord = sphere.RayIntersection(ray)!.Value;
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(1, 0, 0)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(-1, 0, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0,0.5f));
        Assert.True(MyLib.IsZero(hitRecord.T - 1));    }

    [Fact]
    void TestRayIntersectionTranslated()
    {
        var ray = new Ray(new Point(10, 0, 2), -Vec.Ez);
        var sphere = new Sphere(Transformation.Translation(10, 0, 0));
        Assert.True(sphere.RayIntersection(ray).HasValue);
        var hitRecord = sphere.RayIntersection(ray)!.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(10, 0, 1)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(0, 0, 1)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0,0));
        Assert.True(MyLib.IsZero(hitRecord.T - 1));
        
        ray = new Ray(new Point(13, 0, 0), -Vec.Ex);
        Assert.True(sphere.RayIntersection(ray).HasValue);
        hitRecord = sphere.RayIntersection(ray)!.Value;
        
        //_testOutputHelper.WriteLine(hitRecord.Value.WorldPoint.ToString());
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(11, 0, 0)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(1, 0, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0, 0.5f));
        Assert.True(MyLib.IsZero(hitRecord.T - 2));    }

    [Fact]
    public void TestRayIntersectionNoHit()
    {
        var ray = new Ray(new Point(0, 0, 2), Vec.Ez);
        var sphere = new Sphere();
        var hitRecord = sphere.RayIntersection(ray);
        Assert.False(hitRecord.HasValue);
        
        ray = new Ray(new Point(-10, 0, 0), -Vec.Ez);
        hitRecord = sphere.RayIntersection(ray);
        Assert.False(hitRecord.HasValue);
    }
    
}