namespace RTXLib;

public struct Ray
{
	public Point Origin;
	public Vec Dir;
	public float TMin;
	public float TMax;
	public int Depth;

	public Ray()
    {
		Origin = new Point();
		Dir = new Vec();
		TMin = 1e-5f;
		TMax = float.PositiveInfinity;
		Depth = 0;
	}

	public Ray(Point origin, Vec dir, float tMin = 1e-5f, float tMax = float.PositiveInfinity, int depth = 0)
    {
		Origin = origin;
		Dir = dir;
		TMin = tMin;
		TMax = tMax;
		Depth = depth;
	}

	public bool IsClose(Ray otherRay, double epsilon = 1e-5)
    {
		return Origin.IsClose(otherRay.Origin, epsilon) && Dir.IsClose(otherRay.Dir, epsilon);
    }

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

	public void UpdateLimits(float tmin, float tmax)
	{
		TMin = tmin;
		TMax = tmax;
	}
}