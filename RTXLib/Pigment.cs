namespace RTXLib;

///<summary>
///Abstract interface <c>Pigment</c> models a pigment, i. e., a function that associates a color to each point of a parametric surface.
///
///Available patterns are:
///<list type="patterns">
/// <item>
/// <description>Uniform pattern (implemented by <c>UniformPigment</c>)</description>
/// </item>
/// <item>
/// <description>Checkered pattern (implemented by <c>CheckeredPigment</c>)</description>
/// </item>
/// <item>
/// <description>Image-definied custom pattern (implemented by <c>ImagePigment</c>)</description>
/// </item>
/// </list>
/// </summary>
public abstract class Pigment
{
    public abstract Color GetColor(Vec2D coordinates);
}

///<summary>
///Class <c>UniformPigment</c> models a surface painted by a uniform color.
///</summary>
public class UniformPigment : Pigment
{
    public Color Color;

    public UniformPigment(Color color = default)
    {
        Color = color;
    }

    public UniformPigment(float r, float g, float b)
    {
        Color = new Color(r, g, b);
    }

    public override Color GetColor(Vec2D coordinates)
    {
        return Color;
    }
}

///<summary>
///Class <c>CheckeredPigment</c> models a surface colored by two specified colors arranged in a checkered pattern.
///The number of the columns and rows is set to 10 by default but a different number can be specified; however the numbers of columns and rows must be the same.
///<c>color1</c> is assigned to squares whose coordinates are both even or both odd; <c>color2</c> is assigned to other squares.
///</summary>
public class CheckeredPigment : Pigment
{
    public Color Color1;
    public Color Color2;
    int NumberOfSteps;

    public CheckeredPigment(Color color1, Color color2, int numberOfSteps = 10)
    {
        Color1 = color1;
        Color2 = color2;
        NumberOfSteps = numberOfSteps;
    }

    public override Color GetColor(Vec2D coordinates)
    {
        // Check to which square is assigned to the given coordinate
        int u = (int)Math.Floor(coordinates.U * NumberOfSteps);
        int v = (int)Math.Floor(coordinates.V * NumberOfSteps);

        if ((u % 2) == (v % 2))
            return Color1;
        else
            return Color2;
    }
}

///<summary>
///Class <c>ImagePigment</c> models a surface colored according a texture given by a PFM image
///</summary>
public class ImagePigment : Pigment
{
    public HdrImage Image;

    public ImagePigment(HdrImage image)
    {
        Image = image;
    }

    public ImagePigment(string fileName)
    {
        Image = new HdrImage(fileName);
    }
    
    public override Color GetColor(Vec2D coordinates)
    {
        // Check which pixel is assigned to the given coordinate
        int column = (int)(coordinates.U * Image.Width);
        int row = (int)(coordinates.V * Image.Height);

        // Correction of the coordinates of the pixel if calculated coordinates are out of bonds
        if (column >= Image.Width)
            column = Image.Width - 1;

        if (row >= Image.Height)
            row = Image.Height - 1;

        return Image.GetPixel(column, row);
    }
}
