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

    protected Shape(Transformation transformation, Material? material = null)
    {
        Transformation = transformation;
        Material = material ?? new Material(); 
    }
    
    protected Shape(Material material)
    {
        Transformation = Transformation.Identity;
        Material = material; 
    }
    
}
