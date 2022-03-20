// This file implements the class HdrImage, which is use to save an image of a given dimension (width * height)
// The image is implemented as a monodimensional array of the struct Color.
// Each element of the array represents the color of a pixel.

using System.Text;

namespace RTXLib;

public class HdrImage
{
	// *** Fields and constructors *** //

	public int Width;			// Width of the image
	public int Height;			// Height of the image
	public int NPixels;			// Number of pixels in the image
	public Color[] Pixels;		// Array of the pixels
	
	// Constructor that creates an image with specified width and height but with unspecified colors.
	// All pixel are set to color black (0,0,0) that is the default for color class
	public HdrImage(int w, int h)
    {
		Width = w;
		Height = h;
		NPixels = w * h;
		Pixels = new Color[NPixels];

		// Creation of a default color (with all 0 entries)
		Color defaultColor = new Color();

		for (int i = 0; i < NPixels; i++)
		{
			Pixels[i] = defaultColor;
		}
	}

	// Constructor that creates an image with specified width, height and color of each pixel
	public HdrImage(int w, int h, Color[] colors)
    {
		Width = w;
		Height = h;
		NPixels = w * h;
		Pixels = new Color[NPixels];

		for (int i = 0; i < NPixels; i++)
        {
			Pixels[i] = colors[i];
        }

    }

	// *** Other function and methods *** //

	// validate_coordinates checks if the coordinates (x,y) of a pixel are compatible with the dimension of HdrImage
	// If they are not compatible the function returns false, else returns true

	// According to the C# "style"
	// this should be called
	// ValidCoordinates
	public bool  valid_coordinates(int x, int y)
	{
		return ((x >= 0) && (x <= Width) && (y >= 0) && (y <= Height));
	}

	// pixel_offset converts the coordinates (x,y) of a pixel in a positional index

	// PixelOffset
	public int pixel_offset(int x, int y)
    {
		return y * Width + x;
    }

	// set_pixel sets the color of the pixel in position (x,y)

	// SetPixel
	public void set_pixel(int x, int y, Color newColor)
    {
		Pixels[pixel_offset(x,y)] = newColor;
    }

	// get_pixel gets the color of the pixel in position (x,y)

	// GetPixel
	public Color get_pixel(int x, int y)
    {
		return Pixels[pixel_offset(x, y)];
    }
	
	// Read the dimention of a PFM image from a string

	public void ParseImgSize(string line)
	{
		char[] delimiterChars = { ' ', '\t' };
		string[] dimentions = line.Split(delimiterChars);

		if (dimentions.Length != 2)
		{
			throw new InvalidPfmFileFormat("Invalid image size specification: it should be <width> <height>.");
		}

		try
		{
			Width = int.Parse(dimentions[0]);
			Height = int.Parse(dimentions[1]);
			if (Width < 0 || Height < 0)
			{
				throw new InvalidDataException();
			}
		}
		catch
		{
			throw new InvalidPfmFileFormat("Invalid width or height. Should be two int separated by a whitespace.");
		}
	} 
	
	// Decode endianness of from a string
	public float ParseEndianness(string line)
	{
		float value;
		string message = "Invalid endianness specification. Should be a single non-zero number.";
		try
		{
			value = float.Parse(line);
		}
		catch
		{
			throw new InvalidPfmFileFormat(message);
		}

		if (value > 0)
		{
			return +1.0f;
		}
		else if (value < 0)
		{
			return -1.0f;
		}
		else
		{
			throw new InvalidPfmFileFormat(message);
		}
	}
	
	private static void WriteFloat(Stream outputStream, float value, double endianness = -1.0)
	{
		var sequence = BitConverter.GetBytes(value);
		
		// If the machine is not little-endianness, reverse the sequence of bytes
		if (endianness == 1.0 && BitConverter.IsLittleEndian)
		{
			Array.Reverse(sequence);
		}
		outputStream.Write(sequence, 0, sequence.Length);
	}

	public void WritePfm(Stream outputStream, double endianness = -1.0)
	{
		string endiannessString = (endianness == -1.0) ? "-1.0" : "+1.0";
		
		// Write the PFM file header (ascii -> does not change with endianness)
		var header = Encoding.ASCII.GetBytes($"PF\n{Width} {Height}\n{endiannessString}\n");
		outputStream.Write(header, 0, header.Length); 
		
		// Write the image (bottom-to-up, left-to-right)
		for (int y = Height - 1; y >= 0; --y)
		{
			for (int x = 0; x < Width; ++x)
			{
				Color color = get_pixel(x, y);
				WriteFloat(outputStream, color.R, endianness);
				WriteFloat(outputStream, color.G, endianness);
				WriteFloat(outputStream, color.B, endianness);
			}
		}
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
