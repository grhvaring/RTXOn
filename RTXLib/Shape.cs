namespace RTXLib;

/// <summary>
/// Abstract class from which all specific shapes are derived
/// </summary>
public abstract class Shape
{
    public Transformation Transformation;
    public abstract HitRecord? RayIntersection(Ray ray);
    public Material Material;

    /// <summary>
    /// Initializes a <c>Shape</c> object with a given <c>transformation</c> and the default <c>Material</c>
    /// </summary>
    protected Shape(Transformation? transformation = null)
    {
        Transformation = transformation ?? Transformation.Identity;
        Material = new Material();
    }

    /// <summary>
    /// Initializes a <c>Shape</c> object with a given <c>transformation</c> and <c>material</c>
    /// </summary>
    protected Shape(Material? material = null, Transformation? transformation = null)
    {
        Transformation = transformation ?? new Transformation();
        Material = material ?? new Material(); 
    }
}
