using Xunit.Abstractions;

namespace RTXLib.Tests;
using Xunit;

public class ImageTracerTests
{
    // Definition of global variable for ImageTracerTests (C# does not support SetUp / TearDown)
    static HdrImage image = new HdrImage(4, 2);
    static PerspectiveCamera camera = new PerspectiveCamera(aspectRatio: 2);
    static ImageTracer tracer = new ImageTracer(image, camera);
    
    private readonly ITestOutputHelper Output;

    public ImageTracerTests(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public void TestOrientation()
    {
        var topLeftRay = tracer.FireRay(0, 0, 0.0f, 0.0f);
        var bottomRightRay = tracer.FireRay(3, 1, 1.0f, 1.0f);

        var point1 = new Point(0.0f, 2.0f, 1.0f);
        var point2 = new Point(0.0f, -2.0f, -1.0f);

        Assert.True(point1.IsClose(topLeftRay.At(1.0f)));
        Assert.True(point2.IsClose(bottomRightRay.At(1.0f)));
    }

    [Fact]
    public void TestUVSubMapping()
    {
        var ray1 = tracer.FireRay(0, 0, 2.5f, 1.5f);
        var ray2 = tracer.FireRay(2, 1, 0.5f, 0.5f);

        Assert.True(ray1.IsClose(ray2));
    }

    [Fact]
    public void TestImageTracer()
    {
        var ray1 = tracer.FireRay(0, 0, 2.5f, 1.5f);
        var ray2 = tracer.FireRay(2, 1, 0.5f, 0.5f);
        Assert.True(ray1.IsClose(ray2));
        
        tracer.FireAllRays((ray) => new Color(1, 2, 3));
        
        for (var row = 0; row < image.Height; ++row)
        {
            for (var col = 0; col < image.Width; ++col)
            {
                Assert.True(image.GetPixel(col, row).IsClose(new Color(1, 2, 3)));
            }
        }
        
    }
}