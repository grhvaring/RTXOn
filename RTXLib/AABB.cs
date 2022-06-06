namespace RTXLib;

public struct AABB
{
    private float xMin, xMax, yMin, yMax, zMin, zMax;

    public bool QuickRayIntersection(Ray ray)
    {
        var O = ray.Origin;
        var d = ray.Dir;
        var (tx1, tx2) = ((xMin - O.X) / d.X, (xMax - O.X) / d.X);
        var (ty1, ty2) = ((yMin - O.Y) / d.Y, (yMax - O.Y) / d.Y);
        var (tz1, tz2) = ((zMin - O.Z) / d.Z, (zMax - O.Z) / d.Z);
        
        if (Math.Min(tx1, tx2) > Math.Max(ty1, ty2)) return false; 
        if (Math.Min(tx1, tx2) > Math.Max(tz1, tz2)) return false; 
        if (Math.Min(ty1, ty2) > Math.Max(tx1, tx2)) return false;
        if (Math.Min(ty1, ty2) > Math.Max(tz1, tz2)) return false;
        if (Math.Min(tz1, tz2) > Math.Max(tx1, tx2)) return false;
        if (Math.Min(tz1, tz2) > Math.Max(ty1, ty2)) return false;

        return true;

    }
}