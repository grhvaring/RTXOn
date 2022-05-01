namespace RTXLib;

public interface IShape
{
    public Transformation Transformation { get; set; }
    public HitRecord? RayIntersection(Ray ray);
}
