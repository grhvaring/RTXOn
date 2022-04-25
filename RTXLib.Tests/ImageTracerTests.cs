namespace RTXLib.Tests;
using Xunit;

public class ImageTracerTests
{
    [Fact]
    public void TestImageTracer()
    {
        var image = new HdrImage(4, 2);
        var camera = new PerspectiveCamera(aspectRatio: 2);
        var tracer = new ImageTracer(image, camera);

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