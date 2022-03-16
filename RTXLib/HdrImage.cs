// This file implements the class HdrImage, which is use to save an image of a given dimension (width * height)
// The image is implemented as a monodimensional array of the struct Color.
// Each element of the array represents the color of a pixel.

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

	public bool  validate_coordinates(int x, int y)
    {
		if ((x < 0) || (x> Width) || (y < 0) || (y > Height))
        {
			return false;
		}
		else
        {
			return true;
        }
	}

	// pixel_offset converts the coordinates (x,y) of a pixel in a positional index

	public int pixel_offset(int x, int y)
    {
		return y * Width + x;
    }

	// set_pixel sets the color of the pixel in position (x,y)

	public void set_pixel(int x, int y, Color newColor)
    {
		Pixels[pixel_offset(x,y)] = newColor;
    }

	// get_pixel gets the color of the pixel in position (x,y)

	public Color get_pixel(int x, int y)
    {
		return Pixels[pixel_offset(x, y)];
    }

}