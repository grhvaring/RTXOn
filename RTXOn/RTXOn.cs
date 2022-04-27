using System.ComponentModel;
using RTXLib;

namespace RTXOn;

class RTXOn
{
    static void Main(string[] args)
    {
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