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

	// Creating a new normal with all componenents set to 0
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
	public Normal(Normal normal)
	{
		this.normal = new Vector3(normal.X, normal.Y, normal.Z);
	}

	// *** Norms and squared norms *** //

	// Norm calculates the norm of a 3d vector
	public float Norm()
	{
		return (float)normal.Length();
	}

	// SquaredNorm calculates the squared norm of a 3d vector
	public float SquaredNorm()
	{
		return (float)normal.LengthSquared();
	}

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
		return new Normal(scalar * vector.X, scalar * vector.Y, scalar * vector.Z);
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
	// All other combinantion of scalar products are definied in Vec
	public static float operator *(Normal normal1, Normal normal2)
	{
		return normal1.X * normal2.X + normal1.Y * normal2.Y + normal1.Z * normal2.Z;

	}

	// *** Normalization and conversion *** //

	// Normalization of a 3d vector
	// NOTE: the normalization function has to return a new vector or is only a void function that normalize coordinates ?
	public void Normalize()
	{
		float norm = Norm();
		X = X / norm;
		Y = Y / norm;
		Z = Z / norm;
	}

	public Normal CreateNomalizedNormal()
	{
		float norm = Norm();
		return new Normal(X / norm, Y / norm, Z / norm);
	}

	// IsClose checks if a normal can be considered equal to another normal
	public bool IsClose(Normal otherNormal, double epsilon = 1e-5)
	{
		return (Math.Abs(X - otherNormal.X) < epsilon) && (Math.Abs(Y - otherNormal.Y) < epsilon) && (Math.Abs(Z - otherNormal.Z) < epsilon);
	}

	//
	public Vec ConversionToVec()
	{
		return new Vec(X, Y, Z);
	}

	// ToString shows the coordinates of a vector in string format
	public override string ToString()
	{
		return $"(x= {X}, y= {Y}, z= {Z})";
	}
}
