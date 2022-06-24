// This file implements the class HdrImage, which is use to save an image of a given dimension (width * height)
// The image is implemented as a 1D array of the struct Color.
// Each element of the array represents the color of a pixel.

using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

namespace RTXLib;

/// <summary>
/// Image of size (<c>Height</c> x <c>Width</c>), stored in a 1D array of RGB colors.
/// </summary>

public class HdrImage
{

	public int Width;
	public int Height;
	public int NPixels;
	public Color[] Pixels;
	public const double LittleEndian = -1.0;
	public const double BigEndian = +1.0;
	
	// Constructor that creates an image with specified width and height but with unspecified colors.
	// All pixel are set to color black (0,0,0) that is the default for color class
	public HdrImage(int w, int h)
    {
		Width = w;
		Height = h;
		NPixels = w * h;
		Pixels = new Color[NPixels];

		var defaultColor = new Color();

		for (int i = 0; i < NPixels; i++)
		{
			Pixels[i] = defaultColor;
		}
	}

	// Constructor that creates an image with specified width, height and color of each pixel
	public HdrImage(int w, int h, Color[] pixels)
    {
		Width = w;
		Height = h;
		NPixels = w * h;
		Pixels = new Color[NPixels];

		for (int i = 0; i < NPixels; i++)
        {
			Pixels[i] = pixels[i];
        }

    }

	public HdrImage(Stream stream)
	{
		ReadPfmFile(stream);
	}

	public HdrImage(string inputFile)
	{
		using var fileStream = File.OpenRead(inputFile);
		ReadPfmFile(fileStream);
	}

	static public HdrImage operator +(HdrImage a, HdrImage b) 
	{
		if (a.Width != b.Width || a.Height != b.Height) 
			throw new InvalidPfmFileFormat("Cannot sum images with different sizes.");
		var result = new HdrImage(a.Width, a.Height);
		for (int i = 0; i < a.NPixels; ++i)
		{
			result.Pixels[i] = a.Pixels[i] + b.Pixels[i];
		}

		return result;
	}
	
	static public HdrImage operator /(HdrImage image, float a)
	{
		var result = new HdrImage(image.Width, image.Height);
		for (int i = 0; i < image.NPixels; ++i)
		{
			result.Pixels[i] = image.Pixels[i] / a;
		}

		return result;
	}

	public void ReadPfmFile(Stream stream)
	{
		var magic = ReadPfmLine(stream);
		if (magic != "PF") throw new InvalidPfmFileFormat("Invalid magic in PFM file.");

		var imgSize = ReadPfmLine(stream);
		(Width, Height) = ParseImgSize(imgSize);
		NPixels = Width * Height;
		Pixels = new Color[NPixels];

		var endiannessLine = ReadPfmLine(stream);
		double endianness = ParseEndianness(endiannessLine);

		// Write the image bottom-to-up and left-to-right
		for (var y = Height - 1; y >= 0; --y)
		{
			for (var x = 0; x < Width; ++x)
			{
				var r = ReadFloat(stream, endianness);
				var g = ReadFloat(stream, endianness);
				var b = ReadFloat(stream, endianness);
				var color = new Color(r, g, b);
				SetPixel(x, y, color);
			}
		}
	}

	// *** Other function and methods *** //

	// validate_coordinates checks if the coordinates (x,y) of a pixel are compatible with the dimension of HdrImage
	// If they are not compatible the function returns false, else returns true
	public bool ValidCoordinates(int x, int y)
	{
		return x >= 0 && x <= Width && y >= 0 && y <= Height;
	}

	// pixel_offset converts the coordinates (x,y) of a pixel in a positional index
	public int PixelOffset(int x, int y)
    {
		return y * Width + x;
    }

	// set_pixel sets the color of the pixel in position (x,y)
	public void SetPixel(int x, int y, Color newColor)
    {
		Pixels[PixelOffset(x,y)] = newColor;
    }

	// get_pixel gets the color of the pixel in position (x,y)
	public Color GetPixel(int x, int y)
    {
		return Pixels[PixelOffset(x, y)];
    }
	
	// Read the dimensions of a PFM image from a string

	public static (int, int) ParseImgSize(string line)
	{
		char[] delimiterChars = { ' ', '\t' };
		var dimensions = line.Split(delimiterChars);

		if (dimensions.Length != 2)
		{
			throw new InvalidPfmFileFormat("Invalid image size specification: it should be <width> <height>.");
		}

		try
		{
			var width = int.Parse(dimensions[0]);
			var height = int.Parse(dimensions[1]);

			if (width < 0 || height < 0)
			{
				throw new InvalidDataException();
			}

			return (width, height);
		}
		catch
		{
			throw new InvalidPfmFileFormat("Invalid width or height. Should be two positive integers separated by a whitespace.");
		}
	}

	// Decode endianness of from a string
	public static float ParseEndianness(string line)
	{
		float value;
		var message = "Invalid endianness specification. Should be a non-zero number.";
		
		try
		{
			value = float.Parse(line);
		}
		catch
		{
			throw new InvalidPfmFileFormat(message);
		}

		if (value == 0) throw new InvalidPfmFileFormat(message);
		
		return (value > 0) ? (float)BigEndian : (float) LittleEndian;
	}
	
	public static void WriteFloat(Stream outputStream, float value, double endianness = LittleEndian)
	{
		var sequence = BitConverter.GetBytes(value);
		
		var machineEndianness = BitConverter.IsLittleEndian ? LittleEndian : BigEndian;
		if (endianness != machineEndianness) Array.Reverse(sequence);
		
		outputStream.Write(sequence, 0, sequence.Length);
	}

	public void WritePfm(Stream outputStream, double endianness = LittleEndian)
	{
		var endiannessString = (endianness == LittleEndian) ? "-1.0" : "+1.0";
		
		// Write the PFM file header (ascii -> does not change with endianness)
		var header = Encoding.ASCII.GetBytes($"PF\n{Width} {Height}\n{endiannessString}\n");
		outputStream.Write(header, 0, header.Length); 
		
		// Write the image (bottom-to-up, left-to-right)
		for (var y = Height - 1; y >= 0; --y)
		{
			for (var x = 0; x < Width; ++x)
			{
				Color color = GetPixel(x, y);
				WriteFloat(outputStream, color.R, endianness);
				WriteFloat(outputStream, color.G, endianness);
				WriteFloat(outputStream, color.B, endianness);
			}
		}
	}

	public void WritePfm(string outputFile, double endianness = LittleEndian)
	{
		using Stream fileStream = File.OpenWrite(outputFile);
		WritePfm(fileStream, endianness);
	}
	
	public float AverageLuminosity(float delta = 1e-10f)
	{
		float sum = 0;
		foreach (var pixel in Pixels)
		{
			sum += (float)Math.Log10(delta + pixel.Luminosity());
		}
		
		return (float)Math.Pow(10, sum / NPixels);
	}

	public void NormalizeImage(float factor, float? avgLuminosity = null)
	{
		var luminosity = avgLuminosity ?? AverageLuminosity();
		for (int i = 0; i < NPixels; ++i)
		{
			Pixels[i] *= factor / luminosity;
		}
	}

	public void ClampImage()
	{
		for (int i = 0; i < NPixels; ++i)
		{
			Pixels[i].Clamp();
		}
	}
	
	
	// ReadFloat reads 4 bytes from a stream and converts them to a float.
	// The endianness of the bytes can be specified; if not specified little endianness is assumed
	public static float ReadFloat(Stream fileStream, double fileEndianness = LittleEndian)
	{

		const int numBytesToRead = 4;                   // 1 float = 4 bytes
		byte[] bytesBuffer = new byte[numBytesToRead];
		var pcEndianness = BitConverter.IsLittleEndian ? LittleEndian : BigEndian;               
		
		// Read the bytes and convert to a float
		try
		{
			for (var i = 0; i < numBytesToRead; i++)
			{
				var intBuffer = fileStream.ReadByte();

				// Check if end of file is reached before than 4 bytes are read
				if (intBuffer == -1) throw new InvalidDataException();
				
				bytesBuffer[i] = (byte)intBuffer;
			}
		}

		catch
        {
			throw new InvalidPfmFileFormat("Invalid bytes format. A float should be represented by 4 bytes.");
		}

		// If pc and file endianness are not the same, reverse the order of the bytes
		if (fileEndianness != pcEndianness) Array.Reverse(bytesBuffer);

		return BitConverter.ToSingle(bytesBuffer, 0);
	}

	// ReadPfmLine reads a line of bytes from a stream before a end of line (\n) character and converts them to a string (using ASCII)
	// The maximum number of bytes that can be read is 31 + \n character
	// If the \n character is not found the function returns an error message
	public static string ReadPfmLine(Stream fileStream, double fileEndianness = LittleEndian)
	{
		int numMaxBytesToRead = 32;
		int numSignificantBytesRead = 0;					// Counter of effectively read bytes (different from \n)
		int intBuffer;										// Temporary buffer for int number
		byte[] bytesBuffer = new byte[numMaxBytesToRead];	// Temporary buffer array for bytes
		string controlCharacter = "\n";						// Control end of line character
		var pcEndianness = BitConverter.IsLittleEndian ? LittleEndian : BigEndian;
		
		try
		{
			for (var i = 0; i < numMaxBytesToRead; i++)
			{
				intBuffer = fileStream.ReadByte();

				// Trow exception if the end of the stream is reached without finding a \n
				if (intBuffer == -1)
					throw new InvalidDataException();

				bytesBuffer[i] = (byte) intBuffer;

				// If the \n character is read than break the reading
				// NOTE: GetString requires strictly an array and an interval in order to work
				if (Encoding.ASCII.GetString(bytesBuffer, i, 1) == controlCharacter) break;

				numSignificantBytesRead++;
			}
		}

		catch
		{
			throw new InvalidPfmFileFormat("Invalid data format. The end of line character has not been found.");
		}

		// Check if maximum expected number of bytes has been reached without finding the \n character
		if (numSignificantBytesRead == numMaxBytesToRead)
        {
			throw new InvalidPfmFileFormat("Invalid data format. The number of bytes in the line is longer than expected");
		}

		// Resize of the buffer to the effective number of bytes read; the byte of the \n character will be cut out
		Array.Resize(ref bytesBuffer, numSignificantBytesRead);

		// If pc and file endianness are not the same, reverse the order of the bytes
		if (fileEndianness != pcEndianness) Array.Reverse(bytesBuffer);

		// Conversion to string
		return Encoding.ASCII.GetString(bytesBuffer);
	}

	// SaveAsPNG saves the image as a PNG file with a specified name.
	// For coherence the name of the PNG file should end with ".png" extension
	public void SaveAsPng(string fileName, float gamma)
    {
		var bitmap = new Image<Rgb24>(Configuration.Default, Width, Height);

		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				var currentPixel = GetPixel(i, j);
				currentPixel.AdjustPowerLaw(gamma);
				bitmap[i,j] = new Rgb24( (byte)currentPixel.R, (byte)currentPixel.G, (byte)currentPixel.B);
			}
		}

		using Stream fileStream = File.OpenWrite(fileName);
		bitmap.Save(fileStream, new PngEncoder());
    }

	public HdrImage ShallowCopy()
	{
		return new HdrImage(Width, Height, Pixels);
	}
}

public class InvalidPfmFileFormat : FormatException
{
	public InvalidPfmFileFormat()
	{
	}

	public InvalidPfmFileFormat(string message)
		: base($"{message}")
	{
	}
}
