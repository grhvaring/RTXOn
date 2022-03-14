namespace RTXLib;

public struct Color
{
    // Fields of the struct
    public double R, G, B;
    
    // Default Constructor (set all fields to 0)
    public Color()
    {
        R = 0;
        G = 0;
        B = 0;
    }

    // Constructor
    public Color(double r, double g, double b)
    {
        R = r;
        G = g;
        B = b;
    }
    
    // *** Math operations *** //
    
    // (left) product with a scalar
    public static Color operator *(double a, Color c)
    {
        return new Color(a * c.R, a * c.G, a *c.B);
    }
    
    // (right) product with a scalar
    public static Color operator *(Color c, double a)
    {
        return new Color(a * c.R, a * c.G, a *c.B);
    }

    // division by a scalar
    public static Color operator /(Color c, double a)
    {
        if (a == 0) throw new DivideByZeroException();
        return new Color( c.R / a ,  c.G / a, c.B / a);
    }
    
    // product of two colors (component-wise)
    public static Color operator*(Color a, Color b)
    {
        return new Color(a.R * b.R, a.G * b.G, a.B * b.B);
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
    public static Color operator +(Color a, Color b)
    {
        return new Color(a.R + b.R, a.G + b.G, a.B + b.B);
    }

    // Difference of two colors
    public static Color operator -(Color a, Color b)
    {
        return a + (-b);
    }
    
    // *** Other methods *** //
    
    public bool is_close(Color a, double epsilon = 1e-5)
    {
        return (Math.Abs(R - a.R) < epsilon) && (Math.Abs(G - a.G) < epsilon) && (Math.Abs(B - a.B) < epsilon);
    }
    
    public string to_string()
    {
        return $"<r: {R}, g: {G}, b: {B}>";
    }

}