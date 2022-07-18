using System.Numerics;

namespace RTXLib;

public struct Point
{
    private Vector3 point;
    public float X { 
        get => point.X;
        set => point.X = value;
    }
    public float Y { 
        get => point.Y;
        set => point.Y = value;
    }
    public float Z { 
        get => point.Z;
        set => point.Z = value;
    }

    public Point()
    {
        point = new Vector3(0);
    }
    
    public Point(float x, float y, float z)
    {
        point = new Vector3(x, y, z);
    }
    
    public Point(Vector3 v)
    {
        point = new Vector3(v.X, v.Y, v.Z);
    }
    
    public Point(Point p)
    {
        point = new Vector3(p.X, p.Y, p.Z);
    }
    
    public override string ToString()
    {
        return $"Point(x={X}, y={Y}, z={Z})";
    }
    
    public bool IsClose(Point otherPoint, double e = 1e-5)
    {
        return (X - otherPoint.X, Y - otherPoint.Y, Z - otherPoint.Z).AreZero(e);
    }

    public bool IsZero(double e = 1e-5)
    {
        return (X, Y, Z).AreZero(e);
    }

    /// <summary>
    /// Checks if at least one coordinate of the Point is 0
    /// </summary>
    /// <param name="e">Accepted error</param>
    public bool HasZeros(float e = 1e-5f)
    {
        return X.IsZero(e) || Y.IsZero(e) || Z.IsZero(e);
    }
    
    /// <summary>
    /// Checks if at least one coordinate of the Point is 1
    /// </summary>
    /// <param name="e">Accepted error</param>
    public bool HasOnes(float e = 1e-5f)
    {
        return X.IsClose(1, e) || Y.IsClose(1, e) || Z.IsClose(1, e);
    }
    
    public static Point operator +(Point p, Vec v)
    {
        return new Point(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
    }
    
    public static Point operator +(Vec v, Point p)
    {
        return p + v;
    }

    public static Point operator *(float a, Point p)
    {
        return new Point(a * p.X, a * p.Y, a * p.Z);
    }
    
    public static Point operator *(Point p, float a)
    {
        return a * p;
    }
    
    public static Point operator /(Point p, float a)
    {
        return p * (1/a);
    }
    
    public static Point operator -(Point p)
    {
        return -1 * p;
    }
    
    public static Vec operator -(Point p1, Point p2)
    {
        // NOTE: The difference of two points is a vector
        return new Vec(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
    }
    
    public static Point operator -(Point p, Vec v)
    {
        return new Point(p.X - v.X, p.Y - v.Y, p.Z - v.Z);
    }

    public static Point operator -(Vec v, Point p)
    {
        // assume unary operator '-' is defined for Vec
        return -(p - v); // v - p = -(p - v)
    }
    
    public Vec ToVec()
    {
        return new Vec(X, Y, Z);
    }
}