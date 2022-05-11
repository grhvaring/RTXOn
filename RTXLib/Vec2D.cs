using System.Numerics;

namespace RTXLib;

public struct Vec2D
{

	public float U { get; set; }
	public float V { get; set; }

	public static Vec2D Ex => new (1, 0);
	public static Vec2D Ey => new (0, 1);

	// *** Constructors *** //
	

	public Vec2D(float u = 0, float v = 0)
    {
		U = u;
		V = v;
    }
	
	public Vec2D(Vec2D vector)
	{
		U = vector.U;
		V = vector.V;
	}

	public bool IsClose(float u, float v, double e = 1e-5)
	{
		return MyLib.IsZero(u - U, e) && MyLib.IsZero(v - V, e);
	}
	
	public bool IsClose(Vec2D otherVector, double e = 1e-5)
	{
		return IsClose(otherVector.U, otherVector.V, e);
	}

	public override string ToString()
	{
		return $"(u= {U}, v= {V})";
	}
}
