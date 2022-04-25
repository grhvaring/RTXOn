using System;
//using System.Numerics;

namespace RTXLib;

public struct Ray
{
	// Fields
	public Point Origin;
	public Vec Dir;
	public readonly float TMin;
	public readonly float TMax;
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

	public bool IsClose(Ray otherRay, double epsilon = 1e-5)
    {
		return (Origin.IsClose(otherRay.Origin, epsilon) && Dir.IsClose(otherRay.Dir, epsilon));
    }

	// At calculates the point reached from a ray for a given t
	/// <summary>
	/// Convert parametrization into 3D position
	/// </summary>
	/// <param name="t">Time</param>
	/// <returns>Point of the ray at time <i>t</i></returns>
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
