namespace RTXLib;

public abstract class Shape
{
    public Transformation Transformation;
    public abstract HitRecord? RayIntersection(Ray ray);
    public Material Material;
    
    protected Shape(Transformation? transformation = null)
    {
        Transformation = transformation ?? Transformation.Identity;
        Material = new Material();
    }

    protected Shape(Material? material = null, Transformation? transformation = null)
    {
        Transformation = transformation ?? new Transformation();
        Material = material ?? new Material(); 
    }
}
