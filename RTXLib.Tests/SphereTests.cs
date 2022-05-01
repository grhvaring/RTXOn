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
        var hitRecord = sphere.RayIntersection(ray);
        Assert.True(hitRecord.Value.WorldPoint.IsClose(new Point(0, 0, 1)));
        Assert.True(hitRecord.Value.Normal.IsClose(new Normal(0, 0, 1)));
        // (u,v)
        
        ray = new Ray(new Point(3, 0, 0), -Vec.Ex);
        hitRecord = sphere.RayIntersection(ray);
        Assert.True(hitRecord.Value.WorldPoint.IsClose(new Point(1, 0, 0)));
        Assert.True(hitRecord.Value.Normal.IsClose(new Normal(1, 0, 0)));
        // (u,v)

        ray = new Ray(new Point(0, 0, 0), Vec.Ex);
        hitRecord = sphere.RayIntersection(ray);
        Assert.True(hitRecord.Value.WorldPoint.IsClose(new Point(1, 0, 0)));
        Assert.True(hitRecord.Value.Normal.IsClose(new Normal(-1, 0, 0)));
        // (u,v)
    }

    [Fact]
    void TestRayIntersectionTranslated()
    {
        var ray = new Ray(new Point(10, 0, 2), -Vec.Ez);
        var sphere = new Sphere(Transformation.Translation(10, 0, 0));
        var hitRecord = sphere.RayIntersection(ray);
        Assert.True(hitRecord.Value.WorldPoint.IsClose(new Point(10, 0, 1)));
        Assert.True(hitRecord.Value.Normal.IsClose(new Normal(0, 0, 1)));
        // (u,v)

        ray = new Ray(new Point(13, 0, 0), -Vec.Ex);
        hitRecord = sphere.RayIntersection(ray);
        //_testOutputHelper.WriteLine(hitRecord.Value.WorldPoint.ToString());
        Assert.True(hitRecord.Value.WorldPoint.IsClose(new Point(11, 0, 0)));
        Assert.True(hitRecord.Value.Normal.IsClose(new Normal(1, 0, 0)));
        // (u,v)
    }

    [Fact]
    void TestRayIntersectionNoHit()
    {
        var ray = new Ray(new Point(0, 0, 2), Vec.Ez);
        var sphere = new Sphere();
        var hitRecord = sphere.RayIntersection(ray);
        Assert.False(hitRecord.HasValue);
        // (u,v)
        
        ray = new Ray(new Point(-10, 0, 0), -Vec.Ez);
        hitRecord = sphere.RayIntersection(ray);
        Assert.False(hitRecord.HasValue);
    }
    
}