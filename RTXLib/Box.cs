using Xunit;

namespace RTXLib;

/// <summary>
/// A 3D Box defined by its <i>minimum</i> and <i>maximum</i> corners, default: (0,0,0) and (1,1,1).
/// Can be changed by the <c>Transformation</c> passed to the constructor.
/// </summary>
public class Box : Shape
{
    /// <summary>
    /// Creates a 3D Box with <i>minimum</i> vertex in (0,0,0) and <i>maximum</i> in (1, 1, 1), transformed
    /// according to <c>transformation</c>
    /// </summary>
    public Box(Material? material = null, Transformation? transformation = null) : base(material, transformation)
    {}

    /// <summary>
    /// Checks if a given <c>Ray</c> hits the box
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check</param>
    /// <returns><c>HitRecord</c> containing the intersection information. If there is no intersection returns <c>null</c></returns>
    public override HitRecord? RayIntersection(Ray ray)
    {
        var invRay = ray.Transform(Transformation.Inverse());

        var t = FirstIntersectionTime(invRay);
        if (t is null) return null;
        
        var time = t.Value;
        var hitPoint = invRay.At(time);
        var hitRecord = new HitRecord(Transformation * hitPoint, Transformation * NormalAt(hitPoint, invRay.Dir),
            PointToUV(hitPoint), time, ray, this);
        // Console.WriteLine(hitPoint);
        return hitRecord;
    }
    
    /// <summary>
    /// Calculates the time (t-parameter in <c>Ray</c>) of the first valid intersection between a given <c>Ray</c> and the box
    /// </summary>
    /// <param name="invRay"><c>Ray</c> transformed with the inverse of the Box's transformation</param>
    /// <returns>The time of the first intersection if there is a valid one, <c>null</c> otherwise</returns>
    private static float? FirstIntersectionTime(Ray invRay)
    {
        if (!QuickIntersection(ref invRay)) return null;

        if (OnTheBox(invRay.At(invRay.TMin))) return invRay.TMin;
        if (OnTheBox(invRay.At(invRay.TMax))) return invRay.TMax;
        return null;
    }
    
    
    public static bool QuickIntersection(ref Ray invRay)
    {
        // limits for an acceptable intersection
        (float? t0, float t1) = (invRay.TMin, invRay.TMax);
        
        // faces orthogonal to ex
        (t0, t1) = IntersectionTimes(invRay.Origin.X, 1 / invRay.Dir.X, t0.Value, t1);
        if (t0 is null) return false;
        
        // faces orthogonal to ey
        (t0, t1) = IntersectionTimes(invRay.Origin.Y, 1 / invRay.Dir.Y, t0.Value, t1);
        if (t0 is null) return false;
        
        // faces orthogonal to ez
        (t0, t1) = IntersectionTimes(invRay.Origin.Z, 1 / invRay.Dir.Z, t0.Value, t1);
        if (t0 is null) return false;
        
        // change limits in invRay (passed by reference) in order to optimize subsequent comparisons
        invRay.UpdateLimits(t0.Value, t1);
        return true;
    }

    /// <summary>
    /// Calculates the two intersection times between the projection of the ray and one of the axis-aligned faces of the box
    /// </summary>
    /// <param name="origin">Coordinate of the origin of the ray along the dimension considered</param>
    /// <param name="invDir">Reciprocal of component of the direction of the ray along the dimension considered</param>
    /// <returns><c>tuple(entryTime, exitTime)</c>, with <c>entryTime = null</c> if no intersection occurred</returns>
    private static (float?, float) IntersectionTimes(float origin, float invDir, float tmin, float tmax)
    {
        // NOTE: invDir = 1 / direction.ith_component -> can be infinity, but the algorithm works just as fine
        var entryTime = -origin * invDir;
        var exitTime = (1 - origin) * invDir;
        // correct time ordering if flipped
        if (entryTime > exitTime) Swap(ref entryTime, ref exitTime);

        entryTime = Math.Max(tmin, entryTime);
        exitTime = Math.Min(tmax, exitTime);
        
        //  if entryTime > exitTime no intersection occurred -> in that case return null for the first value
        return entryTime < exitTime ? (entryTime, exitTime) : (null, exitTime);
    }

    private static void Swap(ref float a, ref float b)
    {
        (a, b) = (b, a);
    }
    
    /// <summary>
    /// Checks if a 3D point is on the surface of the Box in the reference frame where <i>min</i> = (0,0,0) and
    /// <i>max</i> = (1,1,1)
    /// </summary>
    /// <param name="p">3D world point to check</param>
    /// <returns><c>true</c> if the point is on the surface of the box, <c>false</c> otherwise</returns>
    public static bool OnTheBox(Point p)
    {
        // check it's not outside
        if (!(p.X, p.Y, p.Z).AreBoundedBy(0, 1)) return false;
        
        // check it's not on the interior
        return p.HasZeros() || p.HasOnes();
    }

    /// <summary>
    /// Converts a 3D Point into 2D coordinates describing the point on the parametrized surface.
    /// </summary>
    /// <param name="point">3D World Point (assumed on the surface of the Box)</param>
    /// <returns>2D Coordinates (u, v) in [0,1] x [0,1]</returns>
    private static Vec2D PointToUV(Point point)
    {
        // this works, but does not differentiate between faces
        var (x, y) = (0f, 0f);
        if (point.X.IsZeroOrOne()) (x, y) = (point.Y, point.Z);
        if (point.Y.IsZeroOrOne()) (x, y) = (point.X, point.Z);
        if (point.Z.IsZeroOrOne()) (x, y) = (point.X, point.Y);
        var u = x - (float)Math.Floor(x);
        var v = y - (float)Math.Floor(y);
        return new Vec2D(u, v);
    }

    /// <summary>
    /// Returns the normal to the surface of the box at a point, given a direction of incidence.
    /// In the reference frame where the box is a cube with one vertex in the origin and another in the point (1,1,1)
    /// </summary>
    /// <param name="p">3D point on the surface</param>
    /// <param name="dir">Direction of incidence</param>
    /// <returns></returns>
    private static Normal NormalAt(Point p, Vec dir)
    {
        if (p.X.IsZeroOrOne())
        {
            return dir.X > 0 ? new Normal(-1, 0, 0) : new Normal(1, 0, 0);
        }
        if (p.Y.IsZeroOrOne())
        {
            return dir.Y > 0 ? new Normal(0, -1, 0) : new Normal(0, 1, 0);
        }
        if (p.Z.IsZeroOrOne())
        {
            return dir.Z > 0 ? new Normal(0, 0, -1) : new Normal(0, 0, 1);
        }
        Assert.True(true, "This line should be unreachable.");
        return new Normal(); // Useless return just to make the compiler happy
    }
}