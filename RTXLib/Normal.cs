﻿using System;
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
	public Normal(Normal vector)
	{
		normal = new Vector3(vector.X, vector.Y, vector.Z);
	}
}