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
        
        [Option('r',"aspect-ratio", HelpText = "Aspect ratio of the image.", Default = null)]
        public float? AspectRatio { get; set; }
        
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
        
        [Option("rays", Default = 10)]
        public static int NumberOfRays { get; set; }
        
        [Option("depth", Default = 2)]
        public static int MaxDepth { get; set; }

        [Option("input-file", Default = "examples/demo.txt",
            HelpText = "Input file containing the 3D scene to render.")]
        public string InputSceneFile { get; set; }
        
        [Option("background",  Default = "black",
            HelpText = "Background color, can be black or white.")]
        public static string BackgroundColor { get; set; }

        [Option('s', "subdivisions", Default = 0, HelpText = "Number of subdivisions for the pixels (s = 2 --> 9 rays per pixel.")]
        public int SubDivisions { get; set; }
        
        [Option("seed", HelpText = "Seed for the random number generator.", Default = (ulong)42)]
        public ulong Seed { get; set; }
        
        [Option("sequence", HelpText = "Sequence of the random number generator.", Default = (ulong)54)]
        public ulong Sequence { get; set; }
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
    
    public static List<string> ImagesToAverage(string[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidEnumArgumentException("Expected at least one argument.");
        }

        var files = new List<string>();

        foreach (var a in args)
        {
            if (a.Contains(".pfm")) files.Add(a);
        }

        return files;
    }

    static private (int, int) ReadPfmDimensions(string fileName)
    {
        using var fileStream = File.OpenRead(fileName);
        HdrImage.ReadPfmLine(fileStream); // skip first line
        var dimensions = HdrImage.ReadPfmLine(fileStream);
        return HdrImage.ParseImgSize(dimensions);
    }

    private static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions);
    }

    static readonly Color YELLOW = new(1, 1, 0.2F);
    static readonly Color GREEN = new(0, 1,0 );
    static readonly Color RED = new (1, 0, 0);
    static readonly Color BROWN = new (1.2f, 0.27f,0.2f);
    static readonly Color VIOLET = new (0.9F, 0.2F,1.1F);

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

        ImageTracer? tracer;
      
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
                
                var world = new World();

                // originalDemo(world);

                RedReflectingDemo(world);

                tracer = RenderImage(world, options);
                FinalizeImage(tracer.Image, options);
                break;
            
            case "render":
            {
                using var reader = new StreamReader(options.InputSceneFile);
                var inputStream = new InputStream(reader, options.InputSceneFile);
                try
                {
                    var scene = RTXLib.Parser.ParseScene(inputStream);
                    tracer = RenderImageFromScene(scene, options);
                    FinalizeImage(tracer.Image, options);
                }
                catch (GrammarError e)
                {
                    var loc = e.Location;
                    Console.WriteLine($"In file \"{loc.FileName}\", line {loc.LineNumber}, col {loc.ColumnNumber-1}: {e.Message}");
                }
                break;
            }
            case "average":
                try
                {
                    var images = ImagesToAverage(arguments);
                    if (options.Verbose)
                    {
                        // be verbose
                    }

                    var (width, height) = ReadPfmDimensions(images[0]);
                    var image = new HdrImage(width, height);
                    foreach (var im in images) image += new HdrImage(im);
                    image /= images.Count;
                    FinalizeImage(image, options);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong, see below for the details.");
                    Console.WriteLine(e.Message);
                }
                break;
        }
    }

    private static Renderer SelectRenderer(World world, PCG? pcg = null)
    {
        var background = Options.BackgroundColor == "black" ? Color.BLACK : Color.WHITE;
        return Options.Renderer switch
        {
            "onoff" => new OnOffRenderer(world, background),
            "flat" => new FlatRenderer(world, background),
            "pathtracer" => new PathTracer(world, background, pcg, Options.NumberOfRays, Options.MaxDepth),
            _ => new OnOffRenderer(world, background)
        };
    }

    private static void OriginalDemo(World world)
    {
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
                    world.Add(new Sphere(material, x, y, z, r));
                }
            }
        }

        var firstMaterial = new Material(new CheckeredPigment(VIOLET, YELLOW, 2));
        var checkPigment = new CheckeredPigment(VIOLET, GREEN, 4);
        var checkMaterial = new Material(checkPigment);
        world.Add(new Sphere(checkMaterial, 0, 0, -0.5f * edge, r));
    }

    private static void RedReflectingDemo(World world)
    {
        // reflecting red sphere

        var red = new Color(1.1f, 0.2f, 0.2f);
        var spherePigment = new UniformPigment(red);
        var sphereMaterial = new Material(new SpecularBRDF(spherePigment));
        var sphere = new Sphere(sphereMaterial,0,0,0.5f, 0.5f);
        world.Add(sphere);

        // emitting blue sky dome

        var skyColor = new Color(0.37f, 0.9f, 1);
        var skyMaterial = new Material(new UniformPigment(skyColor));
        var sky = new Sphere(skyMaterial, 0, 0, 0, 100);

        world.Add(sky);

        // optional sun

        var sunMaterial = new Material(new UniformPigment(new Color(1, 1, 1)));
        var sun = new Sphere(sunMaterial, 0, 0, 1.5f, 0.5f);
    
        // world.Add(sun);
                
        // floor
                
        Pigment floorPigment = new CheckeredPigment(GREEN, red);
        //floorPigment = new UniformPigment(new Color(0, 0, 1));
        var floorMaterial = new Material(new DiffuseBRDF(floorPigment));
        var floor = new Plane(floorMaterial);
        world.Add(floor);
    }

    private static ImageTracer RenderImage(World world, Options options, ICamera? camera = null)
    {
        // var transformation = Transformation.RotationZ(options.AngleDegZ) * Transformation.RotationY(15) * Transformation.Translation(-options.Distance);
        camera ??= ChooseCamera(options);
        var image = new HdrImage(options.Width, options.Height);
        var tracer = new ImageTracer(image, camera);
        var renderer = SelectRenderer(world);
        
        tracer.FireAllRays(renderer.Run);

        return tracer;
    }
    
    private static ImageTracer RenderImageFromScene(Scene scene, Options options)
    {
        // var transformation = Transformation.RotationZ(options.AngleDegZ) * Transformation.RotationY(15) * Transformation.Translation(-options.Distance);
        var camera = scene.Camera ?? ChooseCamera(options);
        var image = new HdrImage(options.Width, options.Height);
        var tracer = new ImageTracer(image, camera);
        var pcg = new PCG(options.Seed, options.Sequence);
        var renderer = SelectRenderer(scene.World, pcg);
        Console.WriteLine($"seed: {options.Seed}, sequence: {options.Sequence}");
        tracer.FireAllRays(renderer.Run, options.SubDivisions, pcg);
        return tracer;
    }

    private static ICamera ChooseCamera(Options options, Transformation? transformation = null)
    {
        var aspectRatio = options.AspectRatio ?? options.Width / options.Height;
        if (options.Orthogonal)
        {
            var T = Transformation.RotationZ(options.AngleDegZ) * Transformation.RotationY(5) * Transformation.Translation(-options.Distance);
            return new OrthogonalCamera(aspectRatio, T);
        }

        transformation ??= Transformation.RotationZ(options.AngleDegZ) * Transformation.Translation(0,0,1);
        return new PerspectiveCamera(options.Distance, aspectRatio, transformation.Value);
    }
    
    private static void FinalizeImage(HdrImage image, Options options)
    {
        image.WritePfm(Options.PfmOutput);
        image.NormalizeImage(options.Normalization, Options.Luminosity);
        image.ClampImage();
        image.SaveAsPng(options.PngOutput, options.Gamma);   
    }
}