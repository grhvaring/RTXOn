namespace RTXLib.Tests;
using Xunit;
using Xunit.Abstractions;

public class BoxTests
{
    private readonly ITestOutputHelper Output;

    public BoxTests(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public void TestIntervalsIntersect()
    {
        var (a, b) = (0f, 2f);
        var (c, d) = (1f, 3f);
        Assert.True((a, b).Intersects(c, d));
        
        (a, b) = (0, 1);
        (c, d) = (2, 3);
        Assert.False((a, b).Intersects(c, d));
        
        var (min, max) = (float.MinValue, float.MaxValue);
        Assert.True((min, max).Intersects(c, d));
        Assert.True((min, max).Intersects(min, max));
    }

    [Fact]
    public void TestOnTheBox()
    {
        var p = new Point(1, 0.2f, 0.4f);
        Assert.True(Box.OnTheBox(p));
        
        p = new Point(1, 1.2f, 0.4f);
        Assert.False(Box.OnTheBox(p));
        
        p = new Point(-1, 0, 0);
        Assert.False(Box.OnTheBox(p));
    }

    [Fact]
    public void TestRayIntersectionOutside()
    {
        var ray = new Ray(new Point(-1, 0.5f, 0.5f), Vec.Ex);
        var box = new Box();
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);

        var hitRecord = record.Value;

        Assert.True(hitRecord.WorldPoint.IsClose(new Point(0, 0.5f, 0.5f)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(-1, 0, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.5f, 0.5f));
        Assert.True((hitRecord.T - 1).IsZero());
    }
    
    [Fact]
    public void TestRayIntersectionMiss()
    {
        var ray = new Ray(new Point(-1, -1, 0.5f), Vec.Ez);
        var box = new Box();
        var record = box.RayIntersection(ray);
        Assert.False(record.HasValue);
    }
    
    [Fact]
    public void TestRayIntersectionYFace() {

        var ray = new Ray(new Point(0.5f, 2, 0.5f), -Vec.Ey);
        var box = new Box();
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);
        var hitRecord = record.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(0.5f, 1, 0.5f)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(0, 1, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.5f,0.5f));
        Assert.True((hitRecord.T - 1).IsZero());
    }
    
    [Fact]
    public void TestRayIntersectionInside() {

        var ray = new Ray(new Point(0.5f, 0.5f, 0.5f), Vec.Ez);
        var box = new Box();
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);
        var hitRecord = record.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(0.5f, 0.5f, 1)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(0, 0, -1)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.5f,0.5f));
        Assert.True(hitRecord.T.IsClose(0.5f));
    }

    [Fact]
    public void TestRayIntersectionAtAngle()
    {
        var ray = new Ray(new Point(-1, 0, 0), new Vec(1, 0.2f, 0.7f));
        var box = new Box();
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);
        
        var hitRecord = record.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(0, 0.2f, 0.7f)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(-1, 0, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.2f,0.7f));
        Assert.True((hitRecord.T - 1).IsZero());
    }

    [Fact]
    public void TestScaling()
    {
        var T = Transformation.Scaling(2);
        var box = new Box(transformation: T);
        var ray = new Ray(new Point(-1, 1, 1), Vec.Ex);
        
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);
        var hitRecord = record.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(0, 1, 1)));
        var normal = hitRecord.Normal.Normalize();
        Assert.True(normal.IsClose(new Normal(-1, 0, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.5f,0.5f));
        Assert.True((hitRecord.T - 1).IsZero());
    }

    [Fact]
    public void TestScalingHitFromPositiveY()
    {
        var ray = new Ray(new Point(0.2f, -1, 1.5f), Vec.Ey);
        var box = new Box(transformation: Transformation.Scaling(2));
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);
        var hitRecord = record.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(0.2f, 0, 1.5f)));
        var normal = hitRecord.Normal.Normalize();
        Assert.True(normal.IsClose(new Normal(0, -1, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.1f,0.75f));
        Assert.True((hitRecord.T - 1).IsZero());
        
        // not hitting
        ray = new Ray(new Point(0.2f, -1, 1.5f), -Vec.Ey);
        record = box.RayIntersection(ray);
        Assert.False(record.HasValue);
    }

    [Fact]
    public void TestScalingHitFromInside()
    {
        var ray = new Ray(new Point(1, 1, 1), new Vec(0.4f, 0.6f, 1));
        var box = new Box(transformation: Transformation.Scaling(2));
        
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);
        var hitRecord = record.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(1.4f, 1.6f, 2)));
        var normal = hitRecord.Normal.Normalize();
        Assert.True(normal.IsClose(new Normal(0, 0, -1)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.7f,0.8f));
        Assert.True((hitRecord.T - 1).IsZero());
    }
    
    [Fact]
    public void TestTranslation()
    {
        var T = Transformation.Translation(1, 1, 1);
        var box = new Box(transformation: T);
        var ray = new Ray(new Point(), new Vec(1, 1.5f, 1.5f));
        
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);
        var hitRecord = record.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(1, 1.5f, 1.5f)));
        Assert.True(hitRecord.Normal.IsClose(new Normal(-1, 0, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.5f,0.5f));
        Assert.True((hitRecord.T - 1).IsZero());
    }
    
    [Fact]
    public void TestTranslationAndScaling()
    {
        var T = Transformation.Translation(1, 1, 1) * Transformation.Scaling(2);
        var box = new Box(transformation: T);
        var ray = new Ray(new Point(), new Vec(1, 1.5f, 1.5f));
        
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);
        var hitRecord = record.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(1, 1.5f, 1.5f)));
        Assert.True(hitRecord.Normal.Normalize().IsClose(new Normal(-1, 0, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.25f,0.25f));
        Assert.True((hitRecord.T - 1).IsZero());
    }

    [Fact]
    public void TestStupidInvisibleBox()
    {
        var edge = 0.5f;
        var T = Transformation.Translation(0.5f, -edge/2, 1) * Transformation.Scaling(edge);
        var box = new Box(transformation: T);
        
        // setup camera
        var transformation = Transformation.Translation(0, 0, 1);
        var camera = new PerspectiveCamera(2, 1, transformation);
        var ray = camera.FireRay(0.5f, 0.6f);
        
        // check ray equals the one intended
        Assert.True(ray.IsClose(new Ray(new Point(-2, 0, 1), new Vec(2.5f, 0, 0.25f) * 0.8f)));
        // new Ray(new Point(-2, 0, 1), new Vec(2.5f, 0, 0.25f) * 0.8f);
        
        var record = box.RayIntersection(ray);
        Assert.True(record.HasValue);
        var hitRecord = record.Value;
        
        Assert.True(hitRecord.WorldPoint.IsClose(new Point(0.5f, 0, 1.25f)));
        Assert.True(hitRecord.Normal.Normalize().IsClose(new Normal(-1, 0, 0)));
        Assert.True(hitRecord.SurfacePoint.IsClose(0.5f, 0.5f));
        Assert.True((hitRecord.T - 1.25f).IsZero());

        var missingRay = new Ray(new Point(-2, 0, 1), new Vec(2.5f, -edge, 0.25f) * 0.8f);
        record = box.RayIntersection(missingRay);
        Assert.False(record.HasValue);
    }
}