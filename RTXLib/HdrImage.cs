// This file implements the class HdrImage, which is use to save an image of a given dimension (width * height)
// The image is implemented as a monodimensional array of the struct Color.
// Each element of the array represents the color of a pixel.
using System.IO;
using System.Text;

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
	public bool  ValidCoordinates(int x, int y)
	{
		return ((x >= 0) && (x <= Width) && (y >= 0) && (y <= Height));
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
		string message = "Invalid endianness specification. Should be a non-zero number.";
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
				Color color = GetPixel(x, y);
				WriteFloat(outputStream, color.R, endianness);
				WriteFloat(outputStream, color.G, endianness);
				WriteFloat(outputStream, color.B, endianness);
			}
		}
	}
	
	public float AverageLuminosity(float delta = 1e-10f)
	{
		float sum = 0;
		foreach (Color pixel in Pixels)
		{
			sum += (float)Math.Log10(delta + pixel.Luminosity());
		}

		return (float)Math.Pow(10, sum / (float)NPixels);
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
	public float ReadFloat(Stream fileStream, double fileEndianness = -1)
	{

		int numBytesToRead = 4;                         // Number of bytes to be read (1 float = 4 bytes)
		int intBuffer;									// Temporay buffer
		byte[] bytesBuffer = new byte[numBytesToRead];  // Buffer for the bytes
		double pcEndianness;                            // PC endianness to be checked

		// Check pc endianness
		if (BitConverter.IsLittleEndian == true)
			pcEndianness = -1;
		else
			pcEndianness = 1;

		// Read the bytes and convert to a float
		try
		{
			for (int i = 0; i < numBytesToRead; i++)
			{
				intBuffer = fileStream.ReadByte();

				// Check if end of file is reached before than 4 bytes are read
				if (intBuffer == -1)
					throw new InvalidDataException();
				else
					bytesBuffer[i] = (byte)intBuffer;
			}
		}

		catch
        {
			throw new InvalidPfmFileFormat("Invalid bytes format. A float should be represented by 4 bytes.");
		}

		// If pc and file endianness are not the same, revers the order of the bytes
		if (fileEndianness != pcEndianness)
			Array.Reverse(bytesBuffer);

		return BitConverter.ToSingle(bytesBuffer, 0);
	}

	// ReadPfmLine reads a line of bytes from a stream before a end of line (\n) character and converts them to a string (using ASCII)
	// The maximun number of bytes that can be read is 31 + \n character
	// If the \n character is not found the function returns an error message
	public string ReadPfmLine(Stream fileStream, double fileEndianness = -1)
	{
		int numMaxBytesToRead = 32;							// Maximum expected number of bytes
		int numSignificativeBytesRead = 0;					// Counter of effectively read bytes (different from \n)
		int intBuffer;										// Temporary buffer for int number
		byte[] bytesBuffer = new byte[numMaxBytesToRead];	// Temporary buffer array for bytes
		string controlCharacter = "\n";						// Control end of line character
		double pcEndianness;								// PC endianness to be checked

		// Check pc endianness
		if (BitConverter.IsLittleEndian == true)
			pcEndianness = -1;
		else
			pcEndianness = 1;

		try
		{
			for (int i = 0; i < numMaxBytesToRead; i++)
			{
				intBuffer = fileStream.ReadByte();

				// Trow exception if the end of the stream is reached withoud finding a \n
				if (intBuffer == -1)
					throw new InvalidDataException();
				else
				{
					bytesBuffer[i] = (byte)intBuffer;

					// If the \n character is read than break the reading
					// NOTE: GetString requires strictly an array and an iterval in order to work
					if (Encoding.ASCII.GetString(bytesBuffer, i, 1) == controlCharacter)
						break;

					numSignificativeBytesRead++;
				}
			}
		}

		catch
		{
			throw new InvalidPfmFileFormat("Invalid data format. The end of line character has not been found.");
		}

		// Check if maximum expected number of bytes has been reached without finding the \n character
		if (numSignificativeBytesRead == numMaxBytesToRead)
        {
			throw new InvalidPfmFileFormat("Invalid data format. The number of bytes in the line is longer than expected");
		}

		// Resize of the baffer to the effective number of bytes read; the byte of the \n character will be cut
		Array.Resize(ref bytesBuffer, numSignificativeBytesRead);

		// If pc and file endianness are not the same, revers the order of the bytes
		if (fileEndianness != pcEndianness)
			Array.Reverse(bytesBuffer);

		// Conversion to string
		return Encoding.ASCII.GetString(bytesBuffer);
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
