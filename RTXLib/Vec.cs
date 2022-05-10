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

	public static Vec Ex => new Vec(1, 0, 0);
	public static Vec Ey => new Vec(0, 1, 0);
	public static Vec Ez => new Vec(0, 0, 1);

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

	// Creating a new 3d vector equal to a C# vector
	public Vec(Vector3 vector)
	{
		vec = new Vector3(vector.X, vector.Y, vector.Z);
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

	// Negation operator ... an alternative form of unary - operator

	public Vec Negation()
	{
		return new Vec(-X, -Y, -Z);
	}

	// Product of vector * scalar
	public static Vec operator *(Vec vector, float scalar)
    {
		return new Vec(scalar * vector.X, scalar * vector.Y, scalar * vector.Z);
	}

	// Product of scalar * vector
	public static Vec operator *(float scalar, Vec vector)
	{
		return new Vec(scalar * vector.X, scalar * vector.Y, scalar * vector.Z);
	}

	// Division of vector by a scalar
	public static Vec operator /(Vec vector, float scalar)
	{
		if (scalar == 0) throw new DivideByZeroException();
		return new Vec(vector.X /scalar, vector.Y / scalar, vector.Z / scalar);
	}

	// Scalar products
	// Here are definied the combination of scalar product between a vector and a normal (all possible combination)
	// The scalar product are definied also using the * operator (except the normal * normal case that it is definied in the Normal struct)

	public static float ScalarProduct(Vec vector1, Vec vector2)
    {
		return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
    }

	public static float ScalarProduct(Vec vector1, Normal normal)
	{
		Vec vector2 = normal.ToVec();
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

	// This function normalize a specified vector without creating a new one
	
	public void Normalize()
    {
		float norm = Norm();
		X = X / norm;
		Y = Y / norm;
		Z = Z / norm;
	}

	// This function creates a new normalized vector

	public Vec CreateNomalizedVec()
    {
		float norm = Norm();
		return new Vec(X / norm, Y / norm, Z / norm);
	}


	// Trasformation in a normal struct
	public Normal ConversionToNormal()
    {
		return new Normal(X, Y, Z);
	}

	// *** Other functions *** //

	// IsClose checks if a vector can be considered equal to another vector
	public bool IsClose(Vec otherVector, double epsilon = 1e-5)
	{
		return (Math.Abs(X - otherVector.X) < epsilon) && (Math.Abs(Y - otherVector.Y) < epsilon) && (Math.Abs(Z - otherVector.Z) < epsilon);
	}

	// ToString shows the coordinates of a vector in string format
	public override string ToString()
	{
		return $"(x= {X}, y= {Y}, z= {Z})";
	}
}
