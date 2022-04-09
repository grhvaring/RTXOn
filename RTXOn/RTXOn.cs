using System.ComponentModel;
using RTXLib;

namespace RTXOn;

class RTXOn
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Parameters parameters = new Parameters(args);
        HdrImage image = new HdrImage(parameters.InputPfmFileName);
        image.NormalizeImage(parameters.Factor);
        image.ClampImage();
        image.SaveAsPng(parameters.OnputPngFileName, parameters.Gamma);
    }
}


public struct Parameters
{
    private readonly string inputFile, outputFile;
    private readonly float factor, gamma;

    public string InputPfmFileName => inputFile;
    public string OnputPngFileName => outputFile;

    public float Factor => factor;
    public float Gamma => gamma;

    public Parameters(string[] args)
    {
        if (args.Length != 4)
        {
            throw new InvalidEnumArgumentException("Usage: <program> <pfmFile> <a> <gamma> <otuputFile>");
        }

        inputFile = args[0];

        try
        {
            factor = float.Parse(args[1]);
        }
        catch
        {
            throw new TypeLoadException($"Invalid factor ('{args[1]}'), it must be a floating-point number.");
        }
        
        try
        {
            gamma = float.Parse(args[2]);
        }
        catch
        {
            throw new TypeLoadException($"Invalid gamma ('{args[2]}'), it must be a floating-point number.");
        }

        outputFile = args[3];
    }
}