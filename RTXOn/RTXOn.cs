using System.ComponentModel;
using CommandLine;
using RTXLib;

namespace RTXOn;

class RTXOn
{
    private static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args)
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
        
        ImageTracer? tracer;
        
        // arguments is an array of strings, used in two places
        // pfm2png:    arguments = [input.pfm, output.png]
        // average:    arguments = [image1.png, ... , imageN.png]
        var arguments = options.Arguments.ToArray();
        
        try
        {
            switch (options.Mode)
            {
                case "pfm2png":
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
                        var message =
                            $"In file \"{loc.FileName}\", line {loc.LineNumber}, col {loc.ColumnNumber - 1}: {e.Message}";
                        throw new GrammarError(loc, message);
                    }

                    break;
                }
                case "average":

                    var images = ImagesToAverage(arguments);
                    if (options.Verbose)
                    {
                        // BE VERBOSE
                    }

                    var (width, height) = ReadPfmDimensions(images[0]);
                    image = new HdrImage(width, height);
                    foreach (var im in images) image += new HdrImage(im);
                    image /= images.Count;
                    FinalizeImage(image, options);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Something went wrong, see below for more details.");
            Console.WriteLine(e.Message);
        }
    }
    
    /// <summary>
    /// A container for input/output file names
    /// </summary>
    public readonly struct IOFiles
    {
        public string InputPfmFileName { get; }
        public string OutputPngFileName { get; }

        /// <summary>
        /// Initializes the input and output file names (respectively) from an array containing exactly 2 strings.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException">Thrown if the number of elements in args is not 2.</exception>
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
    
    /// <summary>
    /// Returns only the filenames ending with ".pfm".
    /// </summary>
    /// <exception cref="InvalidEnumArgumentException">Thrown when the list of images is empty</exception>
    private static List<string> ImagesToAverage(string[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidEnumArgumentException("Expected at least one argument.");
        }
        
        return args.Where(a => a.Contains(".pfm")).ToList();
    }
    
    private static (int, int) ReadPfmDimensions(string fileName)
    {
        using var fileStream = File.OpenRead(fileName);
        HdrImage.ReadPfmLine(fileStream); // skip the first line
        var dimensions = HdrImage.ReadPfmLine(fileStream);
        return HdrImage.ParseImgSize(dimensions);
    }

    private static Renderer SelectRenderer(World world, PCG? pcg = null)
    {
        return Options.Renderer switch
        {
            "pathtracer" => new PathTracer(world, pcg, Options.NumberOfRays, Options.MaxDepth),
            "flat" => new FlatRenderer(world),
            "onoff" => new OnOffRenderer(world),
            _ => new PathTracer(world, pcg, Options.NumberOfRays, Options.MaxDepth)
        };
    }
    
    private static ImageTracer RenderImage(World world, Options options, ICamera? camera = null)
    {
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
        tracer.FireAllRays(renderer.Run, options.SubDivisions, pcg, options.SaveSnapshots);
        return tracer;
    }

    private static ICamera ChooseCamera(Options options, Transformation? transformation = null)
    {
        var aspectRatio = options.AspectRatio ?? options.Width / options.Height;
        if (options.Orthogonal)
        {
            var T = Transformation.RotationZ(options.AngleDegZ) * Transformation.RotationY(5) * Transformation.Translation(-options.Distance);
            return new OrthogonalCamera(T, aspectRatio);
        }

        transformation ??= Transformation.RotationZ(options.AngleDegZ) * Transformation.Translation(-1,0,1);
        return new PerspectiveCamera(transformation.Value, options.Distance, aspectRatio);
    }
    
    private static void FinalizeImage(HdrImage image, Options options)
    {
        image.WritePfm(Options.PfmOutput);
        image.NormalizeImage(options.Normalization, Options.Luminosity);
        image.ClampImage();
        image.SaveAsPng(options.PngOutput, options.Gamma);
        Console.WriteLine($"Saved image {options.PngOutput}");
    }
    
    private class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Value(0, Required = true, MetaName = "mode", HelpText = "Execution mode. Choices: pfm2png, render, demo, average.")]
        public string Mode { get; set; }

        [Value(1, Required = false, MetaName = "args", HelpText = "Arguments for the conversion from PFM to PNG.")]
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
        
        [Option( "pfm-output", HelpText = "Output PFM filename.", Default = "out.pfm")]
        public static string PfmOutput { get; set; }
        
        [Option( "angle-deg", HelpText = "Camera rotation angle about Z axis.", Default = 0)]
        public float AngleDegZ { get; set; }

        [Option("renderer", HelpText = "Renderer to use for the demo. Options: pathtracer, flat, onoff.", Default = "pathtracer")]
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
        

        [Option('s', "subdivisions", Default = 0, HelpText = @"Antialiasing subdivisions of each pixel.

    s = 0:                     s = 1:                     s = 2:                      ...

            0                          1                       1     2
   ┌─────────────────┐        ┌────────┬────────┐        ┌─────┬─────┬─────┐
   │                 │        │        │        │        │     │     │     │
   │                 │        │        │        │      1 ├─────┼─────┼─────┤
 0 │                 │      1 ├────────┼────────┤        │     │     │     │
   │                 │        │        │        │      2 ├─────┼─────┼─────┤
   │                 │        │        │        │        │     │     │     │
   └─────────────────┘        └────────┴────────┘        └─────┴─────┴─────┘")]
            
        public int SubDivisions { get; set; }
        
        [Option("seed", HelpText = "Seed for the random number generator.", Default = (ulong)42)]
        public ulong Seed { get; set; }
        
        [Option("sequence", HelpText = "Sequence of the random number generator.", Default = (ulong)54)]
        public ulong Sequence { get; set; }
        
        [Option("snapshots", HelpText = "Save temporary snapshot images", Default = false)]
        public bool SaveSnapshots { get; set; }
    }
}