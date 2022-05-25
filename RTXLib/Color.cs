namespace RTXLib;

/// <summary>
/// Representation of an RGB color with floating point fields R (<i>red</i>), G (<i>green</i>) and B (<i>blue</i>)
/// ought to take values in 0-255.
/// </summary>

public struct Color
{
    // Fields of the struct
    public float R, G, B;
    
    // Default Constructor (set all fields to 0)
    public Color()
    {
        R = 0;
        G = 0;
        B = 0;
    }

    // Constructor
    public Color(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }
    
    public static Color BLACK => new();
    public static Color WHITE => new(255, 255, 255);
    
    // *** Math operations *** //
    
    // (left) product with a scalar
    public static Color operator *(float scalar, Color c)
    {
        return new Color(scalar * c.R, scalar * c.G, scalar * c.B);
    }
    
    // (right) product with a scalar
    public static Color operator *(Color c, float scalar)
    {
        return scalar * c;
    }

    // division by a scalar
    public static Color operator /(Color c, float scalar)
    {
        if (scalar == 0) throw new DivideByZeroException();
        return new Color( c.R / scalar ,  c.G / scalar, c.B / scalar);
    }
    
    // product of two colors (component-wise)
    public static Color operator *(Color c1, Color c2)
    {
        return new Color(c1.R * c2.R, c1.G * c2.G, c1.B * c2.B);
    }

    // Unary operator plus
    public static Color operator +(Color a)
    {
        return a;
    }

    // Unary operator minus
    public static Color operator -(Color a)
    {
        return new Color(-a.R, -a.G, -a.B);
    }
    
    // Sum of two colors
    /// <summary>
    /// Element-wise sum of two RGB colors.
    /// </summary>
    /// <param name="c1">First color</param>
    /// <param name="c2">Second color</param>
    /// <returns>Sum of c1 and c2</returns>
    public static Color operator +(Color c1, Color c2)
    {
        return new Color(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B);
    }

    // Difference of two colors
    public static Color operator -(Color c1, Color c2)
    {
        // WHAT TO DO WHEN a - b < 0 ????
        return c1 + (-c2);
    }
    
    // *** Other methods *** //
    
    public bool IsClose(float r, float g, float b, double e = 1e-5)
    {
        return MyLib.IsZero(R - r, e) && MyLib.IsZero(G - g, e) && MyLib.IsZero(B - b, e);
    }

    public bool IsClose(float rgb, double e = 1e-5)
    {
        return IsClose(rgb, rgb, rgb, e);
    }

    public bool IsZero(double e = 1e-5)
    {
        return IsClose(0, e);
    }
    
    public bool IsClose(Color otherColor, double e = 1e-5)
    {
        return IsClose(otherColor.R, otherColor.G, otherColor.B, e);
    }
    
    public override string ToString()
    {
        return $"<r: {R}, g: {G}, b: {B}>";
    }

    public float Luminosity()
    {
        var max = Math.Max(R, Math.Max(G, B));
        var min = Math.Min(R, Math.Min(G, B));
        return 0.5f * (max + min);
    }

    private static float Clamp(float x)
    {
        return x / (1 + x);
    }
    
    public void Clamp()
    {
        R = Clamp(R);
        G = Clamp(G);
        B = Clamp(B);
    }

    private static int AdjustPowerLaw(float rgbComponent, float gamma)
    {
        return (int)(255 * Math.Pow(rgbComponent, 1.0f / gamma));
    }

    public void AdjustPowerLaw(float gamma)
    {
        R = AdjustPowerLaw(R, gamma);
        G = AdjustPowerLaw(G, gamma);
        B = AdjustPowerLaw(B, gamma);
    }
}