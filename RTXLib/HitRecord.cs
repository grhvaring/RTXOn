namespace RTXLib;

public readonly struct HitRecord
{
    public Point WorldPoint { get; }
    public Normal Normal { get; }
    public Vec2D SurfacePoint { get; }
    public float T { get; }
    public Ray Ray { get; }

    public HitRecord(Point worldPoint, Normal normal, Vec2D surfacePoint, float t, Ray ray)
    {
        WorldPoint = worldPoint;
        Normal = normal;
        SurfacePoint = surfacePoint;
        T = t;
        Ray = ray;
    }

    public bool IsClose(HitRecord other)
    {
        return WorldPoint.IsClose(other.WorldPoint) && Normal.IsClose(other.Normal) &&
               SurfacePoint.IsClose(other.SurfacePoint) && MyLibrary.IsZero(T - other.T) && Ray.IsClose(other.Ray);
    }
}