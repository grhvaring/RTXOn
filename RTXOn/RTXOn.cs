using System.ComponentModel;
using RTXLib;

namespace RTXOn;

class RTXOn
{
    static void Main(string[] args)
    {
        
        var WHITE = new Color(255, 255, 255);
        var BLACK = new Color();

        Console.WriteLine("Hello, World!");
        try
        {
            var parameters = new Parameters(args);
            var image = new HdrImage(parameters.InputPfmFileName);
            image.NormalizeImage(parameters.Factor); 
            image.ClampImage();
            image.SaveAsPng(parameters.OutputPngFileName, parameters.Gamma);
        }
        catch (Exception e)
        {
            Console.WriteLine("Something went wrong, see below for the details.");
            Console.WriteLine(e.Message);
        }

        var world = new World();
        var r = 1.0f / 10.0f;
        float[] limits = {-0.5f, 0.5f};
        foreach (var x in limits)
        {
            foreach (var y in limits)
            {
                foreach (var z in limits)
                {
                    world.Add(new Sphere(x, y, z, r));
                }
            }
        }
        
        world.Add(new Sphere(0, 0.5f, 0, r));
        world.Add(new Sphere(0,0, -0.5f, r));

        var camera = new PerspectiveCamera(1, 1, Transformation.Translation(-1, 0, 0));
        // var camera = new OrthogonalCamera(1, Transformation.Translation(-1, 0, 0));
        var imageSpheres = new HdrImage(1080, 1080);
        var tracer = new ImageTracer(imageSpheres, camera);
        tracer.FireAllRays(ray => world.RayIntersection(ray).HasValue ? WHITE : BLACK);

        var factor = 1.0f;
        var gamma = 1.0f;
        tracer.Image.NormalizeImage(factor); 
        tracer.Image.ClampImage();
        tracer.Image.SaveAsPng("spheres.png", gamma);
    }
}


public readonly struct Parameters
{
    public string InputPfmFileName { get; }
    public string OutputPngFileName { get; }
    public float Factor { get; }
    public float Gamma { get; }

    public Parameters(string[] args)
    {
        if (args.Length != 4)
        {
            throw new InvalidEnumArgumentException("Usage: <program> <pfmFile> <a> <gamma> <outputFile>");
        }

        InputPfmFileName = args[0];

        try
        {
            Factor = float.Parse(args[1]);
        }
        catch
        {
            throw new TypeLoadException($"Invalid factor ('{args[1]}'), it must be a floating-point number.");
        }
        
        try
        {
            Gamma = float.Parse(args[2]);
        }
        catch
        {
            throw new TypeLoadException($"Invalid gamma ('{args[2]}'), it must be a floating-point number.");
        }

        OutputPngFileName = args[3];
    }
}