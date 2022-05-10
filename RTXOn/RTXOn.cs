using System;
using System.ComponentModel;
using CommandLine;
using RTXLib;

namespace RTXOn;

static class RTXOn
{
    private class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Value(0, Required = true, MetaName = "mode", HelpText = "Mode to execute.")]
        public string Mode { get; set; }

        [Value(1, MetaName = "args", HelpText = "Parameters")]
        public IEnumerable<string> Arguments { get; set; }
        
        [Option('w',"width", HelpText = "Width of the image.", Default = 1080)]
        public int Width { get; set; }
        
        [Option('h',"height", HelpText = "Height of the image.", Default = 1080)]
        public int Height { get; set; }
        
        [Option('o',"orthogonal", HelpText = "Use orthogonal camera instead of perspective camera.")]
        public bool Orthogonal { get; set; }
        
        [Option('r',"aspect-ratio", HelpText = "Aspect ratio of the image.", Default = 1)]
        public float AspectRatio { get; set; }
        
        [Option('a', "normalization", HelpText = "Normalization factor for the image.", Default = 1)]
        public float Normalization { get; set; }
        
        [Option('g', "gamma", HelpText = "Gamma factor (monitor flux correction).", Default = 1)]
        public float Gamma { get; set; }
        
        [Option('d', "distance", HelpText = "Distance of the camera from screen.", Default = 1)]
        public float Distance { get; set; }
        
        [Option('R', "radius", HelpText = "Radius of the spheres.", Default = 1.0f / 10.0f)]
        public float Radius { get; set; }
        
        [Option('e', "edge", HelpText = "Edge lenght of the cube.", Default = 1)]
        public float Edge { get; set; }
        
        [Option( "output", HelpText = "Output PNG filename.", Default = "out.png")]
        public string Output { get; set; }
        
        [Option( "angle-deg", HelpText = "Camera rotation angle about Z axis.", Default = 0)]
        public float AngleDegZ { get; set; }
    }
    public readonly struct IOFiles
    {
        public string InputPfmFileName { get; }
        public string OutputPngFileName { get; }

        public IOFiles(string[] args)
        {
            if (args.Length is not 2)
            {
                throw new InvalidEnumArgumentException("Expected arguments: <input-pfm-file> <output-png-file>");
            }

            InputPfmFileName = args[0];
            OutputPngFileName = args[1];
        }
    }

    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions);
    }

    static void RunOptions(Options options)
    {
        if (options.Verbose)
        {
            Console.WriteLine("Verbose output enabled.");
            Console.WriteLine($"Program mode: {options.Mode}");
            Console.WriteLine("Current arguments:");
            Console.WriteLine($"\tAspect ratio\t-r {options.AspectRatio}");
            Console.WriteLine($"\tNormalization\t-a {options.Normalization}");
            Console.WriteLine($"\tFlux correction\t-g {options.Gamma}");   
        }
        var arguments = options.Arguments.ToArray();

        switch (options.Mode)
        {
            case "pfm2png":
                try
                {
                    var files = new IOFiles(arguments);
                    if (options.Verbose)
                    {
                        Console.WriteLine($"\tInput file\t{files.InputPfmFileName}");
                        Console.WriteLine($"\tOutput file\t{files.OutputPngFileName}");
                    }
                    var image = new HdrImage(files.InputPfmFileName);
                    image.NormalizeImage(options.Normalization);
                    image.ClampImage();
                    image.SaveAsPng(files.OutputPngFileName, options.Gamma);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong, see below for the details.");
                    Console.WriteLine(e.Message);
                }

                break;

            case "demo":
                if (options.Verbose)
                {
                    Console.WriteLine($"\tScreen distance\t-d {options.Distance}");
                    Console.WriteLine($"\tSpheres radii\t-R {options.Radius}");
                    Console.WriteLine($"\tCube edge\t-e {options.Edge}");
                }
                var WHITE = new Color(255, 255, 255);
                var BLACK = new Color();

                var world = new World();
                var r = options.Radius; // spheres radius
                var edge = options.Edge;
                float[] limits = {-0.5f * options.Edge, 0.5f * edge}; // edges of the cube
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
        
                world.Add(new Sphere(0, 0.5f * edge, 0, r));
                world.Add(new Sphere(0,0, -0.5f * edge, r));

                var transformation = Transformation.RotationZ(options.AngleDegZ) * Transformation.RotationY(-15) * Transformation.Translation(-options.Distance);
                ICamera camera = options.Orthogonal ? 
                    new OrthogonalCamera(options.AspectRatio, transformation) : 
                    new PerspectiveCamera(options.Distance, options.AspectRatio, transformation);
                var imageSpheres = new HdrImage(options.Width, options.Height);
                var tracer = new ImageTracer(imageSpheres, camera);
                tracer.FireAllRays(ray => world.RayIntersection(ray).HasValue ? WHITE : BLACK);
                
                tracer.Image.NormalizeImage(options.Normalization); 
                tracer.Image.ClampImage();
                tracer.Image.SaveAsPng(options.Output, options.Gamma);
                break;
        }
    }
}