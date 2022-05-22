using System;
using System.ComponentModel;
using CommandLine;
using RTXLib;

namespace RTXOn;

class RTXOn
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

        [Option("orthogonal", HelpText = "Use orthogonal camera instead of perspective camera.")]
        public bool Orthogonal { get; set; }
        
        [Option('r',"aspect-ratio", HelpText = "Aspect ratio of the image.", Default = 1)]
        public float AspectRatio { get; set; }
        
        [Option("normalization", HelpText = "Normalization factor for the image.", Default = 1)]
        public float Normalization { get; set; }
        
        [Option("gamma", HelpText = "Gamma factor (monitor flux correction).", Default = 1)]
        public float Gamma { get; set; }
        
        [Option('d', "distance", HelpText = "Distance of the camera from screen.", Default = 1)]
        public float Distance { get; set; }

        [Option( "png-output", HelpText = "Output PNG filename.", Default = "out.png")]
        public string PngOutput { get; set; }
        
        [Option( "pfm-output", HelpText = "Output PNG filename.", Default = "out.pfm")]
        public static string PfmOutput { get; set; }
        
        [Option( "angle-deg", HelpText = "Camera rotation angle about Z axis.", Default = 0)]
        public float AngleDegZ { get; set; }

        [Option("renderer", HelpText = "Renderer to use for the demo: onoff/flat.", Default = "flat")]
        public static string Renderer { get; set; }

        [Option("luminosity", HelpText = "Override average luminosity of the image.")]
        public static float? Luminosity { get; set; }
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

    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions);
    }

    private static void RunOptions(Options options)
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
                    image.NormalizeImage(options.Normalization, Options.Luminosity);
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
                }
                var YELLOW = new Color(1, 1, 0.2F);
                var GREEN = new Color(0, 1,0 );
                var RED = new Color(1.1f, 0.2F, 0.2F);
                var BROWN = new Color(1.2f, 0.27f,0.2f);
                var VIOLET = new Color(0.9F, 0.2F,1.1F);


                var world = new World();
                var r = 1.0f / 10.0f; // spheres radius
                var edge = 1.0f;
                float[] limits = {-0.5f * edge, 0.5f * edge}; // edges of the cube
                var material = new Material(new UniformPigment(RED));
                foreach (var x in limits)
                {
                    foreach (var y in limits)
                    {
                        foreach (var z in limits)
                        {
                            world.Add(new Sphere(x, y, z, material, r));
                        }
                    }
                }

                var firstMaterial = new Material(new CheckeredPigment(VIOLET, YELLOW, 2));
                // var spinniiii = new HdrImage("memorial.pfm");
                // var spinniMaterial = new Material(new ImagePigment(spinniiii));
                world.Add(new Sphere(0, 0.5f * edge, 0, firstMaterial, r));
                var checkPigment = new CheckeredPigment(VIOLET, GREEN, 4);
                var checkMaterial = new Material(checkPigment);
                world.Add(new Sphere(0,0, -0.5f * edge, checkMaterial, r));

                var transformation = Transformation.RotationZ(options.AngleDegZ) * Transformation.RotationY(15) * Transformation.Translation(-options.Distance);
                ICamera camera = options.Orthogonal ? 
                    new OrthogonalCamera(options.AspectRatio, transformation) : 
                    new PerspectiveCamera(options.Distance, options.AspectRatio, transformation);
                var imageSpheres = new HdrImage(options.Width, options.Height);
                var tracer = new ImageTracer(imageSpheres, camera);
                if (Options.Renderer == "onoff") Console.WriteLine("onoff renderer");
                Renderer renderer = Options.Renderer == "onoff" ? new OnOffRenderer(world) : new FlatRenderer(world);
                tracer.FireAllRays(renderer.Run);
                tracer.Image.WritePfm(Options.PfmOutput);
                tracer.Image.NormalizeImage(options.Normalization, Options.Luminosity); 
                tracer.Image.ClampImage();
                tracer.Image.SaveAsPng(options.PngOutput, options.Gamma);
                break;
        }
    }
}