namespace RTXLib;

public static class MyLib
{
    public static bool AreClose(float a, float b, double e = 1e-5)
    {
        return IsZero(a - b, e);
    }

    public static bool AreZero(float dx, float dy, float dz, double e = 1e-5)
    {
        return IsZero(dx, e) && IsZero(dy, e) && IsZero(dz, e);
    }

    public static bool IsZero(float x, double e = 1e-5)
    {
        return Math.Abs(x) < e;
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
}