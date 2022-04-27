namespace RTXLib;

public class MyLibrary
{
    public static bool AreClose(float a, float b, double e = 1e-5)
    {
        return IsZero(a - b);
    }

    public static bool IsZero(float x, double e = 1e-5)
    {
        return Math.Abs(x) < e;
    }
}