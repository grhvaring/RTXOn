using System;
using System.Numerics;

namespace RTXLib;

public struct Normal
{
	private Vector3 normal;

	public float X
	{
		get => normal.X;
		set => normal.X = value;
	}
	public float Y
	{
		get => normal.Y;
		set => normal.Y = value;
	}
	public float Z
	{
		get => normal.Z;
		set => normal.Z = value;
	}

	public Normal()
	{
		normal = new Vector3(0);
	}

	// Creating a new normal with all componenents set to assigned values
	public Normal(float x, float y, float z)
	{
		normal = new Vector3(x, y, z);
	}

	// Creating a new normal equal to a C# vector
	public Normal(Vector3 vector)
	{
		normal = new Vector3(vector.X, vector.Y, vector.Z);
	}

	// Creating a new normal equal to another normal
	public Normal(Normal other)
	{
		normal = new Vector3(other.X, other.Y, other.Z);
	}

	// *** Norms and squared norm *** //

	public float Norm => normal.Length();
	public float SquaredNorm => normal.LengthSquared();

	// *** Operations *** //

	// Unary minus operator
	public static Normal operator -(Normal vector)
	{
		return new Normal(-vector.X, -vector.Y, -vector.Z);
	}

	// Negation operator ... an alternative form of unary - operator
	public Normal Negation()
	{
		return new Normal(-X, -Y, -Z);
	}

	// Product of normal * scalar
	public static Normal operator *(Normal vector, float scalar)
	{
		return scalar * vector;
	}

	// Product of scalar * normal
	public static Normal operator *(float scalar, Normal vector)
	{
		return new Normal(scalar * vector.X, scalar * vector.Y, scalar * vector.Z);
	}

	// Division of a normal by a scalar
	public static Normal operator /(Normal vector, float scalar)
	{
		if (scalar == 0) throw new DivideByZeroException();
		return new Normal(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
	}

	// Scalar product normal * normal
	// All other combinations of scalar products are defined in Vec
	public static float operator *(Normal normal1, Normal normal2)
	{
		return normal1.X * normal2.X + normal1.Y * normal2.Y + normal1.Z * normal2.Z;

	}

	// *** Normalization and conversion *** //
	
	public Normal Normalize()
	{
		var norm = Norm;
		return new Normal(X / norm, Y / norm, Z / norm);
	}

	// IsClose checks if a normal can be considered equal to another normal
	public bool IsClose(Normal otherNormal, double epsilon = 1e-5)
	{
		return IsClose(otherNormal.X, otherNormal.Y, otherNormal.Z);
	}
	
	public bool IsClose(float x, float y, float z, double e = 1e-5)
	{
		return MyLib.AreZero(X - x, Y - y, Z - z, e);
	}

	public bool IsClose(float xyz, double e = 1e-5)
	{
		return IsClose(xyz, xyz, xyz, e);
	}
	
	public Vec ToVec()
	{
		return new Vec(X, Y, Z);
	}

	public override string ToString()
	{
		return $"(x= {X}, y= {Y}, z= {Z})";
	}
}
