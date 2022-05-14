using RTXLib;


// NOTE: is it really need to implement constructor and eval for the class Pigment ?

///<summary>
///Abstract class <c>BRDF</c> models a generic BRDF (Bidirectional Reflectance Distribution Function).
///</summary>
public abstract class BRDF
{
    public Pigment Pigment { get; set; }
    
    // NOTE: to be changed in order to have pigment = BLACK as default
    public BRDF(UniformPigment pigment)
    {
        Pigment = pigment;
    }

    public virtual Color Eval(Normal normal, Vec directionIn, Vec directionOut, Vec2D coordinates)
    {
        return new Color(0, 0, 0);
    }
}

///<summary>
///Class <c>DiffuseBRDF</c> models an ideal diffuse BRDF (Lambertian BRDF).
///</summary>
public class DiffuseBRDF : BRDF
{
    public float Reflectance { get; set; }

    public DiffuseBRDF(UniformPigment pigment, float reflectance = 1.0f) : base(pigment)
    {      
        Reflectance = reflectance;
    }
    public override Color Eval(Normal normal, Vec directionIn, Vec directionOut, Vec2D coordinates)
    {
        return Pigment.GetColor(coordinates) * (float)(Reflectance/Math.PI);
    }
}
