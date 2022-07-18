using Xunit.Abstractions;

namespace RTXLib.Tests;
using Xunit;

public class CameraTests
{
    
    // Visualization of outputs during a test
    private readonly ITestOutputHelper _testOutputHelper;

    public CameraTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    [Fact]
    public void TestOrthogonalCamera()
    {
        var aspectRatio = 2;
        var camera = new OrthogonalCamera(new Transformation(), aspectRatio);
        var ray1 = camera.FireRay(0, 0);
        var ray2 = camera.FireRay(1, 0);
        var ray3 = camera.FireRay(0, 1);
        var ray4 = camera.FireRay(1, 1);

        // Check if the rays are parallel 
        Assert.True(MyLib.IsZero(Vec.CrossProduct(ray1.Dir, ray2.Dir).SquaredNorm()));
        Assert.True(MyLib.IsZero(Vec.CrossProduct(ray1.Dir, ray3.Dir).SquaredNorm()));
        Assert.True(MyLib.IsZero(Vec.CrossProduct(ray1.Dir, ray4.Dir).SquaredNorm()));
        
        // Check if the rays hit the corners in the correct coordinates
        Assert.True(ray1.At(1).IsClose(new Point(0, aspectRatio, -1)));
        Assert.True(ray2.At(1).IsClose(new Point(0, -aspectRatio, -1)));
        Assert.True(ray3.At(1).IsClose(new Point(0, aspectRatio, 1)));
        Assert.True(ray4.At(1).IsClose(new Point(0, -aspectRatio, 1)));
    }

    [Fact]
    public void TestOrthogonalCameraTransform()
    {
        var transformation = Transformation.Translation(-2 * Vec.Ey) * Transformation.RotationZ(90);
        var camera = new OrthogonalCamera(transformation, 1);
        var ray = camera.FireRay(0.5f, 0.5f);
        //_testOutputHelper.WriteLine(ray.At(1).ToString());
        Assert.True(ray.At(1).IsClose(new Point(0, -2, 0)));
    }

    [Fact]
    public void TestPerspectiveCamera()
    {
        var aspectRatio = 2;
        var camera = new PerspectiveCamera(distance: 1, aspectRatio: aspectRatio);

        var ray1 = camera.FireRay(0, 0);
        var ray2 = camera.FireRay(1, 0);
        var ray3 = camera.FireRay(0, 1);
        var ray4 = camera.FireRay(1, 1);

        // Check that the rays have the same origin
        Assert.True(ray1.Origin.IsClose(ray2.Origin));
        Assert.True(ray1.Origin.IsClose(ray3.Origin));
        Assert.True(ray1.Origin.IsClose(ray4.Origin));
        
        // Check that the rays hit the corners in the correct coordinates
        Assert.True(ray1.At(1).IsClose(new Point(0, aspectRatio, -1)));
        Assert.True(ray2.At(1).IsClose(new Point(0, -aspectRatio, -1)));
        Assert.True(ray3.At(1).IsClose(new Point(0, aspectRatio, 1)));
        Assert.True(ray4.At(1).IsClose(new Point(0, -aspectRatio, 1)));
    }
    
    [Fact]
    public void TestPerspectiveCameraTransform()
    {
        var transformation = Transformation.Translation(-2 * Vec.Ey) * Transformation.RotationZ(90);
        var camera = new PerspectiveCamera(transformation, 1, 2);
        var ray = camera.FireRay(0.5f, 0.5f);
        //_testOutputHelper.WriteLine(ray.At(1).ToString());
        Assert.True(ray.At(1).IsClose(new Point(0, -2, 0)));
    }
}