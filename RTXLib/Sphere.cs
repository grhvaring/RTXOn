namespace RTXLib;

public class Sphere : Shape
{
    /// <summary>
    /// Creates a <c>Sphere</c> object, with a specific <c>material</c>, center in <c>(x, y, z)</c> and radius <c>r</c>
    /// </summary>
    public Sphere(Material material, float x, float y, float z, float r = 1) : base(material)
    {
        Transformation = Transformation.Translation(x, y, z) * Transformation.Scaling(r);
    }

    public Sphere(Transformation? transformation = null) : base(transformation) {}
    public Sphere(Material material, Transformation transformation) : base(material, transformation) {}

    public override HitRecord? RayIntersection(Ray ray)
    {
        var invRay = ray.Transform(Transformation.Inverse());
        var firstHitTime= CalculateFirstIntersectionTime(ref invRay);
        
        if (!firstHitTime.HasValue) return null; // if there is no intersection return null
        
        var time = firstHitTime.Value;
        var hitPoint = invRay.At(time);
        var hitRecord = new HitRecord(Transformation * hitPoint, Transformation * NormalAt(hitPoint, invRay.Dir),
                PointToUV(hitPoint), time, ray, this);
        return hitRecord;
    }

    private static float? CalculateFirstIntersectionTime(ref Ray invRay)
    {
        var O = invRay.Origin.ToVec();
        var d = invRay.Dir;
        
        var a = d.SquaredNorm();
        var b2 = O * d;
        var c = O.SquaredNorm() - 1;
        var delta4 = b2 * b2 - a * c;
        
        if (delta4 < 0) return null; // null = no hit
        
        var sqrtDelta4 = (float) Math.Sqrt(delta4);
        var invA = 1 / a;
        var t1 = (-b2 - sqrtDelta4) * invA;
        var t2 = (-b2 + sqrtDelta4) * invA;
        
        if (t1.IsBoundedBy(invRay.TMin, invRay.TMax))
        {
            invRay.UpdateLimits(invRay.TMin, t1);
            return t1;
        }

        if (!t2.IsBoundedBy(invRay.TMin, invRay.TMax)) return null; // null -> no hit
        invRay.UpdateLimits(invRay.TMin, t2);
        return t2;

    }

    private static Normal NormalAt(Point point, Vec rayDir)
    {
        var result = new Normal(point.X, point.Y, point.Z);
        return point.ToVec() * rayDir < 0 ? result : -result;
    }

    private static Vec2D PointToUV(Point point)
    {
        const float pi = (float) Math.PI;
        var u = (float)(Math.Atan2(point.Y, point.X) / (2 * pi));
        u = u >= 0 ? u : u + 1;
        return new Vec2D(u, (float)Math.Acos(point.Z) / pi);
    }

}