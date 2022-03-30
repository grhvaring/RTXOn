using System;
using System.Numerics;

namespace RTXLib;

public struct Vec
{
	private Vector3 vec;

	// Methods for reading and modifing the coordinates
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
