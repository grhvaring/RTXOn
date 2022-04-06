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
}