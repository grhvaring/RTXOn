namespace RTXLib;

public static class MyLib
{
    // Extension methods for comparison checks with floats
    /// <summary>
    /// Checks if the float's absolute value is less than a given error (default 1e-5)
    /// </summary>
    public static bool IsZero(this float x, double e = 1e-5)
    {
        return Math.Abs(x) < e;
    }
    
    public static bool IsClose(this float a, float b, double e = 1e-5)
    {
        return (a - b).IsZero(e);
    }

    public static bool AreZero(this (float, float, float) triple, double e = 1e-5)
    {
        return triple.Item1.IsZero(e) && triple.Item2.IsZero(e) && triple.Item3.IsZero(e);
    }

    ///<summary>
    ///Method <c>CreateONBFromZ</c> creates a tuple of 3 vectors that represent an orthonormal basis starting from a reference normal and using the algorithm Duff et al. 2017 algorithm.
    ///</summary>
    /// ///<remarks>The reference normal must already be normalized; the method will not check if this condition is met.</remarks>
    public static (Vec, Vec, Vec) CreateONBFromZ(Vec normal)
    {
        float sign;

        if (normal.Z > 0.0f)
            sign = 1.0f;
        else
            sign = -1.0f;

        float a = -1.0f / (sign + normal.Z);
        float b = normal.X * normal.Y * a;

        Vec e1 = new Vec(1.0f + sign * normal.X * normal.X * a, sign * b, -sign * normal.X);
        Vec e2 = new Vec(b, sign + normal.Y * normal.Y * a, -normal.Y);
        Vec e3 = new Vec(normal.X, normal.Y, normal.Z);

        return (e1, e2, e3);
    }

    ///<summary>
    ///Method <c>CreateONBFromZ</c> creates a tuple of 3 vectors that represent an orthonormal basis starting from a reference normal and using the algorithm Duff et al. 2017 algorithm.
    ///</summary>
    ///<remarks>The reference normal must already be normalized; the method will not check if this condition is met.</remarks>
    public static (Vec, Vec, Vec) CreateONBFromZ(Normal normal)
    {
        float sign;

        if (normal.Z > 0)
            sign = 1.0f;
        else
            sign = -1.0f;

        float a = -1.0f / (sign + normal.Z);
        float b = normal.X * normal.Y * a;

        Vec e1 = new Vec(1.0f + sign * normal.X * normal.X * a, sign * b, -sign * normal.X);
        Vec e2 = new Vec(b, sign + normal.Y * normal.Y * a, -normal.Y);
        Vec e3 = new Vec(normal.X, normal.Y, normal.Z);

        return (e1, e2, e3);
    }

    /// <summary><c>KeywordDicitionary</c> is the dictionary that converts string representing keywords in keyword</summary>
    public static Dictionary<string, KeywordEnum> Keywords = new()
    {
	    {"new", KeywordEnum.New},
	    {"material", KeywordEnum.Material},
	    {"plane", KeywordEnum.Plane},
        {"sphere", KeywordEnum.Sphere},
        {"box", KeywordEnum.Box},
	    {"diffuse", KeywordEnum.Diffuse},
	    {"specular", KeywordEnum.Specular},
	    {"uniform", KeywordEnum.Uniform},
	    {"checkered", KeywordEnum.Checkered},
	    {"image", KeywordEnum.Image},
	    {"identity", KeywordEnum.Identity},
	    {"translation", KeywordEnum.Translation},
	    {"rotation_x", KeywordEnum.RotationX},
	    {"rotation_y", KeywordEnum.RotationY},
	    {"rotation_z", KeywordEnum.RotationZ},
	    {"scaling", KeywordEnum.Scaling},
	    {"camera", KeywordEnum.Camera},
	    {"orthogonal", KeywordEnum.Orthogonal},
	    {"perspective", KeywordEnum.Perspective},
	    {"float", KeywordEnum.Float}
    };
    
    /// <summary>
    /// Extension method that checks whether a given float x is in the interval [a,b]
    /// </summary>
    public static bool IsBoundedBy(this float x, float a, float b, float e = 1e-7f)
    {
        // add error to x and not to the edges because a and b can be float.MaxValue/MinValue and cause overflows
        return a <= x + e && x - e <= b;
    }

    public static bool AreBoundedBy(this (float, float, float) values, float a, float b, float e = 1e-7f)
    {
        return values.Item1.IsBoundedBy(a, b, e) && 
               values.Item2.IsBoundedBy(a, b, e) && 
               values.Item3.IsBoundedBy(a, b, e);
    }
    
    public static bool Intersects(this (float , float) I, float a, float b)
    {
        return a < I.Item2 && I.Item1 < b;
    }

    public static bool IsZeroOrOne(this float x, double e = 1e-5)
    {
        return x.IsZero(e) || x.IsClose(1, e);
    }
}