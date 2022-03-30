using System;
using System.Numerics;

namespace RTXLib;

public struct Vec
{
	private Vector3 vec;

	// Only for reading the variable
	public readonly float X => vec.X;
	public readonly float Y => vec.Y;
	public readonly float Z => vec.Z;

	// Creating a new 3d vector with all componenents set to 0
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
}
