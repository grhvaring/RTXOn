namespace RTXLib;

public readonly struct HitRecord
{
    public Point WorldPoint { get; }
    public Normal Normal { get; }
    public Vec2D SurfacePoint { get; }
    public float T { get; }
    public Ray Ray { get; }
    public Shape Shape { get; }

    public HitRecord(Point worldPoint, Normal normal, Vec2D surfacePoint, float t, Ray ray, Shape shape)
    {
        WorldPoint = worldPoint;
        Normal = normal;
        SurfacePoint = surfacePoint;
        T = t;
        Ray = ray;
        Shape = shape;
    }

    public bool IsClose(HitRecord other, double e = 1e-5)
    {
        return WorldPoint.IsClose(other.WorldPoint, e) && Normal.IsClose(other.Normal, e) &&
               SurfacePoint.IsClose(other.SurfacePoint, e) && MyLib.IsZero(T - other.T, e) && 
               Ray.IsClose(other.Ray, e);
    }
}