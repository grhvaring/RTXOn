using System;
//using System.Numerics;

namespace RTXLib;

public struct Ray
{
	// Fields
	public Point Origin;
	public Vec Dir;
	public float TMin;
	public float TMax;
	public int Depth;


	// Default constructor
	public Ray()
    {
		Origin = new Point();
		Dir = new Vec();
		TMin = 1e-5f;
		TMax = Single.PositiveInfinity;
		Depth = 0;
	}

	// Constructor with arguments
	public Ray(Point originPoint, Vec directionVector, float minimumDistance = 1e-5f, float maximumDistance = Single.PositiveInfinity, int numReflection = 0)
    {
		Origin = originPoint;
		Dir = directionVector;
		TMin = minimumDistance;
		TMax = maximumDistance;
		Depth = numReflection;
	}

	// IsClose checks if two rays have origin and dir close
	public bool IsClose(Ray otherRay, double epsilon = 1e-5)
    {
		return (Origin.IsClose(otherRay.Origin, epsilon) && Dir.IsClose(otherRay.Dir, epsilon));
    }

	// At calculates the point reached from a ray for a given t
	public Point At(float t)
    {
		return Origin + t * Dir;
    }

	public Ray Transform(Transformation transformation)
	{
		// QUESTION: Why don't we just change the current ray?
		return new Ray(transformation * Origin, transformation * Dir, TMin, TMax, Depth);
	}
	
}
