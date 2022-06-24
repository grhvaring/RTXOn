namespace RTXLib;

public interface ICamera
{
    public float AspectRatio { get; set; }
    public Transformation Transformation { get; set; }
    public Ray FireRay(float u, float v);
}

public class OrthogonalCamera : ICamera
{
    public OrthogonalCamera(float aspectRatio = 1)
    {
        AspectRatio = aspectRatio;
        Transformation = new Transformation();
    }
    
    public OrthogonalCamera(float aspectRatio, Transformation transformation)
    {
        AspectRatio = aspectRatio;
        Transformation = transformation;
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
    public PerspectiveCamera(float distance = 1, float aspectRatio = 1, Transformation? transformation = null)
    {
        Distance = distance;
        AspectRatio = aspectRatio;
        Transformation = transformation ?? Transformation.Identity;
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