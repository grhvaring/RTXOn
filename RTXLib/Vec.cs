using System;
using System.Numerics;

namespace RTXLib;

public struct Vec
{
	private Vector3 vec;

	// *** Methods get/set *** //
	public float X
    {
		get => vec.X;
		set => vec.X = value;
    }
	public float Y
	{
		get => vec.Y;
		set => vec.Y = value;
	}
	public float Z
	{
		get => vec.Z;
		set => vec.Z = value;
	}

	public static Vec Ex => new (1, 0, 0);
	public static Vec Ey => new (0, 1, 0);
	public static Vec Ez => new (0, 0, 1);

	// *** Constructors *** //

	// Default constructor creates a new 3d vector with all componenents set to 0
	public Vec()
	{
		vec = new Vector3(0);
	}

	// Creating a new 3d vector with all componenents set to assigned values
	public Vec(float x, float y, float z)
    {
		vec = new Vector3(x, y, z);
    }

	// Creating a new 3d vector equal to another 3D vector
	public Vec(Vec vector)
	{
		vec = new Vector3(vector.X, vector.Y, vector.Z);
	}

	// *** Norms and squared norms *** //

	// Norm calculates the norm of a 3d vector
	public float Norm()
    {
		return (float)vec.Length();
    }

	// SquaredNorm calculates the squared norm of a 3d vector
	public float SquaredNorm()
    {
		return (float)vec.LengthSquared();
    }

	// *** Operations *** //

	// Sum of 3d vector
	public static Vec operator+(Vec vector1, Vec vector2)
    {
		return new Vec(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
    }

	// Difference of 3d vector
	public static Vec operator -(Vec vector1, Vec vector2)
	{
		return vector1 + (-vector2);
	}

	// Unary operator minus
	public static Vec operator -(Vec vector)
	{
		return new Vec(-vector.X, -vector.Y, -vector.Z);
	}
	
	// Product of vector * scalar
	public static Vec operator *(Vec vector, float scalar)
    {
		return scalar * vector;
	}

	// Product of scalar * vector
	public static Vec operator *(float scalar, Vec vector)
	{
		return new Vec(scalar * vector.X, scalar * vector.Y, scalar * vector.Z);
	}

	// Division of vector by a scalar
	public static Vec operator /(Vec vector, float scalar)
	{
		if (scalar == 0) throw new DivideByZeroException("You divided by zero!");
		return vector * (1 / scalar);
	}
	
	// "Reciprocal" of vector component-wise
	public static Vec operator /(float a, Vec vector)
	{
		return a * new Vec(1 / vector.X, 1 / vector.Y, 1 / vector.Z);
	}

	// Scalar products
	// Here are defined the combination of scalar product between a vector and a normal (all possible combination)
	// The scalar product are defined also using the * operator (except the normal * normal case that it is definied in the Normal struct)

	public static float ScalarProduct(Vec vector1, Vec vector2)
    {
		return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
    }

	public static float ScalarProduct(Vec vector1, Normal normal)
	{
		var vector2 = normal.ToVec();
		return ScalarProduct(vector1, vector2);
	}

	public static float ScalarProduct(Normal normal, Vec vector2)
	{
		Vec vector1 = normal.ToVec();
		return ScalarProduct(vector1, vector2);
	}

	public static float ScalarProduct(Normal normal1, Normal normal2)
	{
		Vec vector1 = normal1.ToVec();
		Vec vector2 = normal2.ToVec();
		return ScalarProduct(vector1, vector2);
	}

	public static float operator *(Vec vector1, Vec vector2)
    {
		return ScalarProduct(vector1, vector2);

	}

	public static float operator *(Vec vector1, Normal normal)
	{
		return ScalarProduct(vector1, normal);

	}

	public static float operator *(Normal normal, Vec vector2)
    {
		return ScalarProduct(normal, vector2);

	}

	// Cross products
	// All kind of combination of vector and normal are recognized and the result is always a vector

	// vector x vector
	public static Vec CrossProduct(Vec vector1, Vec vector2)
    {
		return new Vec(vector1.Y * vector2.Z - vector1.Z * vector2.Y, vector1.Z * vector2.X - vector1.X * vector2.Z, vector1.X * vector2.Y - vector1.Y * vector2.X);
    }

	// vector x normal
	public static Vec CrossProduct(Vec vector1, Normal normal)
    {
		Vec vector2 = normal.ToVec();
		return CrossProduct(vector1, vector2);
	}

	// normal x vector
	public static Vec CrossProduct(Normal normal, Vec vector2)
	{
		Vec vector1 = normal.ToVec();
		return CrossProduct(vector1, vector2);
	}

	// normal x normal
	public static Vec CrossProduct(Normal normal1, Normal normal2)
	{
		Vec vector1 = normal1.ToVec();
		Vec vector2 = normal2.ToVec();
		return CrossProduct(vector1, vector2);
	}

	// *** Normalization and conversion *** //
	
	public Vec Normalize()
	{
		var norm = Norm();
		return new Vec(X, Y, Z) / norm;
	}
	
	// *** Other functions *** //

	// IsClose checks if a vector can be considered equal to another vector
	public bool IsClose(Vec otherVector, double e = 1e-5)
	{
		return (X - otherVector.X, Y - otherVector.Y, Z - otherVector.Z).AreZero(e);
	}

	public bool IsZero(double e = 1e-5)
	{
		return (X, Y, Z).AreZero(e);
	}
	
	// ToString shows the coordinates of a vector in string format
	public override string ToString()
	{
		return $"(x = {X}, y = {Y}, z = {Z})";
	}
}
