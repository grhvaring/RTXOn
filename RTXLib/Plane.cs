namespace RTXLib;

public class Plane : Shape
{
    public Plane(Transformation? transformation = null) : base(transformation) {}
    public Plane(Transformation transformation, Material? material = null) : base(transformation, material) {}
    public Plane(Material material) : base(material) {}
    
    public override HitRecord? RayIntersection(Ray ray)
    {
        var invRay = ray.Transform(Transformation.Inverse());
        
        if (invRay.Dir.Z == 0) return null;

        var firstHitTime = -invRay.Origin.Z / invRay.Dir.Z;
        if (firstHitTime < 0 || firstHitTime > invRay.TMax) return null;
        
        var hitPoint = invRay.At(firstHitTime);
        
        return new HitRecord(Transformation * hitPoint, Transformation * NormalAt(invRay.Dir),
                PointToUV(hitPoint), firstHitTime, ray, this);
    }

    private static Normal NormalAt(Vec dir)
    {
        var defaultNormal = new Normal(0, 0, 1);
        return dir.Z > 0 ? defaultNormal : -defaultNormal;
    }

    private static Vec2D PointToUV(Point point)
    {
        var u = point.X - (float)Math.Floor(point.X);
        var v = point.Y - (float)Math.Floor(point.Y);
        return new Vec2D(u, v);
    }

}