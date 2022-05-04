namespace RTXLib;

public class Sphere : IShape
{
    public Transformation Transformation { get; set; }
    
    public Sphere(Transformation? transformation = null)
    {
        Transformation = transformation ?? new Transformation();
    }

    public Sphere(float x, float y, float z, float r = 1)
    {
        Transformation = Transformation.Translation(x, y, z) * Transformation.Scaling(r);
    }

    public HitRecord? RayIntersection(Ray ray)
    {
        var invRay = ray.Transform(Transformation.Inverse());
        var firstHitTime= CalculateFirstIntersectionTime(invRay);
        
        if (!firstHitTime.HasValue) return null; // if there is no intersection return null
        
        var time = firstHitTime.Value;
        var hitPoint = invRay.At(time);
        var hitRecord = new HitRecord(Transformation * hitPoint, Transformation * NormalAt(hitPoint, invRay.Dir),
                PointToUV(hitPoint), time, ray);
        return hitRecord;
    }

    private static float? CalculateFirstIntersectionTime(Ray invRay)
    {
        var O = invRay.Origin.ToVec();
        var d = invRay.Dir;
        
        var a = d.SquaredNorm();
        var b2 = O * d;
        var c = O.SquaredNorm() - 1;
        var delta4 = b2 * b2 - a * c;
        
        if (delta4 < 0) return null; // null = no hit
        
        var sqrtDelta4 = (float) Math.Sqrt(delta4);
        var t1 = (-b2 - sqrtDelta4) / a;
        var t2 = (-b2 + sqrtDelta4) / a;
        
        float? t = null; // // t = null -> no hit

        if (t1 > invRay.TMin && t1 < invRay.TMax)
        {
            t = t1;
        }
        else if (t2 > invRay.TMin && t2 < invRay.TMax)
        {
            t = t2;
        }
        return t;
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