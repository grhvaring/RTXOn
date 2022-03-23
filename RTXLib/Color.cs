namespace RTXLib;

/// <summary>
/// Color as combination of a <i>red</i> (R), <i>green</i> (G), and <i>blue</i> (B) value with range 0-255.
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
    
    // *** Math operations *** //
    
    // (left) product with a scalar
    public static Color operator *(float scalar, Color c)
    {
        return new Color(scalar * c.R, scalar * c.G, scalar *c.B);
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
    public static Color operator*(Color c1, Color c2)
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
        // WHAT TO DO WHEN a - b < 0 ????
        return new Color(-a.R, -a.G, -a.B);
    }
    
    // Sum of two colors
    public static Color operator +(Color c1, Color c2)
    {
        return new Color(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B);
    }

    // Difference of two colors
    public static Color operator -(Color c1, Color c2)
    {
        return c1 + (-c2);
    }
    
    // *** Other methods *** //
    
    public bool IsClose(Color otherColor, double epsilon = 1e-5)
    {
        return (Math.Abs(R - otherColor.R) < epsilon) && (Math.Abs(G - otherColor.G) < epsilon) && (Math.Abs(B - otherColor.B) < epsilon);
    }
    
    public override string ToString()
    {
        return $"<r: {R}, g: {G}, b: {B}>";
    }

}