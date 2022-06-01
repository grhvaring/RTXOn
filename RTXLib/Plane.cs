namespace RTXLib;

public class Plane : Shape
{
    public Plane(Material? material = null, Transformation? transformation = null) : base(material, transformation) {}
    
    public override HitRecord? RayIntersection(Ray ray)
    {
        var invRay = ray.Transform(Transformation.Inverse());

        var firstHitTime = CalculateFirstIntersectionTime(invRay);
        if (!firstHitTime.HasValue) return null;

        var time = firstHitTime.Value;
        var hitPoint = invRay.At(time);
        
        return new HitRecord(Transformation * hitPoint, Transformation * NormalAt(invRay.Dir),
                PointToUV(hitPoint), time, ray, this);
    }

    private static float? CalculateFirstIntersectionTime(Ray invRay)
    {
        if (MyLib.IsZero(invRay.Dir.Z)) return null;  // ray parallel to plane
        
        var t = -invRay.Origin.Z / invRay.Dir.Z;
        return t > 0 && t < invRay.TMax ? t : null; // check this one
    }

    private static Normal NormalAt(Vec dir)
    {
        var defaultNormal = new Normal(0, 0, 1);
        return dir.Z > 0 ? -defaultNormal : defaultNormal;
    }

    private static Vec2D PointToUV(Point point)
    {
        var u = point.X - (float)Math.Floor(point.X);
        var v = point.Y - (float)Math.Floor(point.Y);
        return new Vec2D(u, v);
    }

}