using Xunit;

namespace RTXLib;

/// <summary>
/// A 3D Box defined by its <i>minimum</i> and <i>maximum</i> corners.
/// </summary>
public class Box : Shape
{
    /// <summary>
    /// Default constructor: Cube with <i>minimum</i> vertex in the origin and <i>maximum</i> in (1, 1, 1)
    /// </summary>
    public Box(Material? material = null, Transformation? transformation = null) : base(material, transformation)
    {}
    
    public Box(Material material, float xmin, float ymin, float zmin, float xmax, float ymax, float zmax) : base(material) {
        Transformation = Transformation.Translation(xmin, ymin, zmin) * Transformation.Scaling(xmax, ymax, zmax);
    }

    public Box(Material material, Point pmin, Point pmax) :
        this(material, pmin.X, pmin.Y, pmin.Z, pmax.X, pmax.Y, pmax.Z)
    {
        // nothing else to do here
    }

    public override HitRecord? RayIntersection(Ray ray)
    {
        var invRay = ray.Transform(Transformation.Inverse());

        var t = FirstIntersectionTime(invRay);
        // Console.WriteLine(t);
        
        if (!t.HasValue) return null; // if there is no intersection return null
        
        var time = t.Value;
        var hitPoint = invRay.At(time);
        var hitRecord = new HitRecord(Transformation * hitPoint, Transformation * NormalAt(hitPoint, invRay.Dir),
            PointToUV(hitPoint), time, ray, this);
        // Console.WriteLine(hitPoint);
        return hitRecord;
    }

    private Vec2D PointToUV(Point point)
    {
        // this works, but does not differentiate between faces
        var (x, y) = (0f, 0f);
        if (MyLib.IsZero(point.X) || MyLib.IsZero(point.X - 1)) (x, y) = (point.Y, point.Z);
        if (MyLib.IsZero(point.Y) || MyLib.IsZero(point.Y - 1)) (x, y) = (point.X, point.Z);
        if (MyLib.IsZero(point.Z) || MyLib.IsZero(point.Z - 1)) (x, y) = (point.X, point.Y);
        var u = x - (float)Math.Floor(x);
        var v = y - (float)Math.Floor(y);
        return new Vec2D(u, v);
    }

    public static bool OnTheBox(Point p)
    {
        // check it's not outside
        const float e = 1e-3f;
        if (p.X < -e || p.Y < -e || p.Z < -e) return false;
        if (p.X > 1 || p.Y > 1 || p.Z > 1) return false;
        
        // check it's not on the interior
        if (MyLib.IsZero(p.X) || MyLib.IsZero(p.Y) || MyLib.IsZero(p.Z)) return true;
        return MyLib.IsZero(p.X - 1) || MyLib.IsZero(p.Y - 1) || MyLib.IsZero(p.Z - 1);
    }

    private static float? FirstIntersectionTime(Ray invRay)
    {
        // Set quantities in the reference frame where the box is a cube with vertices in (0, 0, 0) and (1, 1, 1)
        var O = invRay.Origin;
        var d = invRay.Dir;

        // Calculate intersections
        // NOTE: The times are not necessarily sorted from small to big because the ray could hit either side of the box
        // (depending on the transformation)
        var (tx1, tx2) = Intersections(O.X, d.X);
        var (ty1, ty2) = Intersections(O.Y, d.Y);
        var (tz1, tz2) = Intersections(O.Z, d.Z);

        // Correct time ordering
        if (tx1 > tx2) Swap(ref tx1, ref tx2);
        if (ty1 > ty2) Swap(ref ty1, ref ty2);
        if (tz1 > tz2) Swap(ref tz1, ref tz2);

        // Check if the time intervals [tx1, tx2], [ty1, ty2] and [tz1, tz2] intersect
        var t1 = TimeIntersectionWithFace(tx1, tx2, ty1, ty2, invRay.TMin, invRay.TMax);
        if (!t1.HasValue) return null;
        var tmin = t1.Value;
        var t2 = TimeIntersectionWithFace(tz1, tz2, ty1, ty2, invRay.TMin, invRay.TMax);
        if (!t2.HasValue) return null;
        var tmiddle = t2.Value;
        if (tmin > tmiddle) Swap(ref tmin, ref tmiddle);
        var t3 = TimeIntersectionWithFace(tx1, tx2, tz1, tz2, invRay.TMin, invRay.TMax);
        if (!t3.HasValue) return null;
        var tmax = t3.Value;
        if (tmin > tmax) Swap(ref tmin, ref tmax);
        if (tmiddle > tmax) Swap(ref tmiddle, ref tmax);
        
        if (OnTheBox(invRay.At(tmin)))
        {
            return tmin > invRay.TMin && tmin < invRay.TMax ? tmin : null;
        }
        if (OnTheBox(invRay.At(tmiddle)))
        {
            return tmiddle > invRay.TMin && tmiddle < invRay.TMax ? tmiddle : null;
        }
        if (OnTheBox(invRay.At(tmax)))
        {
            return tmax > invRay.TMin && tmax < invRay.TMax ? tmax : null;
        }
        return null;
    }

    private static float? TimeIntersectionWithFace(float a1, float a2, float b1, float b2 , float min, float max)
    {
        // ray parallel to direction a
        if (a1 == float.MinValue)
        {
            if (min < b1 && b1 < max) return b1;
            if (min < b2 && b2 < max) return b2;
            return null;
        }
        // ray parallel to direction b
        if (b1 == float.MinValue) 
        {
            if (min < a1 && a1 < max) return a1;
            if (min < a2 && a2 < max) return a2;
            return null;
        }
        // check intersection
        if (IntervalsIntersect(a1, a2, b1, b2))
        {
            var firstHit = Math.Max(a1, b1);
            if (min < firstHit && firstHit < max) return firstHit;
            var secondHit = Math.Min(a2, b2);
            if (min < secondHit && secondHit < max) return secondHit;
        }
        return null;
    }

    private static (float, float) Intersections(float origin, float direction)
    {
        return !MyLib.IsZero(direction) ? (-origin / direction, (1 - origin) / direction) : (float.MinValue, float.MaxValue);
    }

    private static void Swap(ref float a, ref float b)
    {
        (a, b) = (b, a);
    } 

    public static bool IntervalsIntersect(float a1, float a2, float b1, float b2)
    {
        return b1 < a2 && a1 < b2;
    }

    public static float SmallestPositiveTime(float[] times)
    {
        var t = float.MaxValue;
        foreach (var ti in times)
        {
            if (ti > 0 && ti < t) t = ti;
        }
        return t;
    } 

    private static Normal NormalAt(Point p, Vec dir)
    {
        if (MyLib.IsZero(p.X) || MyLib.AreClose(p.X, 1))
        {
            return dir.X > 0 ? new Normal(-1, 0, 0) : new Normal(1, 0, 0);
        }
        if (MyLib.IsZero(p.Y) || MyLib.AreClose(p.Y, 1))
        {
            return dir.Y > 0 ? new Normal(0, -1, 0) : new Normal(0, 1, 0);
        }
        if (MyLib.IsZero(p.Z) || MyLib.AreClose(p.Z, 1))
        {
            return dir.Z > 0 ? new Normal(0, 0, -1) : new Normal(0, 0, 1);
        }
        Assert.True(true, "This line should be unreachable.");
        return new Normal(); // Useless return just to make the compiler happy
    }
}