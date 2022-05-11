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
}