namespace RTXLib;

public interface ICamera
{
    public float AspectRatio { get; set; }
    public Transformation Transformation { get; set; }
    public Ray FireRay(float u, float v);
}

public class OrthogonalCamera : ICamera
{
    public OrthogonalCamera(Transformation? transformation = null, float aspectRatio = 1)
    {
        Transformation = transformation ?? Transformation.Identity;
        AspectRatio = aspectRatio;
    }
    
    public float AspectRatio { get; set; }
    public Transformation Transformation { get; set; }

    public Ray FireRay(float u, float v)
    {
        var origin = new Point(-1, (1 - 2 * u) * AspectRatio, 2 * v - 1);
        var direction = Vec.Ex;
        return new Ray(origin, direction, 1).Transform(Transformation);
    }
}

public class PerspectiveCamera : ICamera
{
    public PerspectiveCamera(Transformation? transformation = null, float distance = 1, float aspectRatio = 1)
    {
        Transformation = transformation ?? Transformation.Identity;
        Distance = distance;
        AspectRatio = aspectRatio;
    }

    public float Distance { get; set; }
    public float AspectRatio { get; set; }
    public Transformation Transformation { get; set; }

    public Ray FireRay(float u, float v)
    {
        var origin = new Point(-Distance, 0, 0);
        var direction = new Vec(Distance, (1 - 2 * u) * AspectRatio, 2 * v - 1);
        // tmin = - Origin.X / direction.X = 1;
        return new Ray(origin, direction, 1).Transform(Transformation);
    }
}