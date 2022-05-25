using RTXLib;


// NOTE: is it really need to implement constructor and eval for the class Pigment ?

///<summary>
///Abstract class <c>BRDF</c> models a generic BRDF (Bidirectional Reflectance Distribution Function).
///</summary>
public abstract class BRDF
{
    public Pigment Pigment { get; }
    
    // NOTE: to be changed in order to have pigment = BLACK as default

    public BRDF()
    {
        Pigment = new UniformPigment(Color.BLACK);
    }
    
    public BRDF(Pigment pigment)
    {
        Pigment = pigment;
    }

    public virtual Color Eval(Normal normal, Vec directionIn, Vec directionOut, Vec2D coordinates)
    {
        return new Color(0, 0, 0);
    }

    public abstract Ray ScatterRay(PCG pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth);
}

///<summary>
///Class <c>DiffuseBRDF</c> models an ideal diffuse BRDF (Lambertian BRDF).
///</summary>
public class DiffuseBRDF : BRDF
{
    public float Reflectance { get; }

    public DiffuseBRDF(float reflectance = 1.0f)
    {      
        Reflectance = reflectance;
    }
    
    public DiffuseBRDF(Pigment pigment, float reflectance = 1.0f) : base(pigment)
    {      
        Reflectance = reflectance;
    }
    
    public override Color Eval(Normal normal, Vec directionIn, Vec directionOut, Vec2D coordinates)
    {
        return Pigment.GetColor(coordinates) * (float)(Reflectance/Math.PI);
    }

    public override Ray ScatterRay(PCG pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
    {
        var (e1, e2, e3) = MyLib.CreateONBFromZ(normal);
        var cosThetaSquared = pcg.RandomFloat();
        var (cosTheta, sinTheta) = ((float)Math.Sqrt(cosThetaSquared), (float)Math.Sqrt(1.0f - cosThetaSquared));
        var phi = 2.0f * (float)Math.PI * pcg.RandomFloat();
        return new Ray(
            interactionPoint,
            e1 * (float) Math.Cos(phi) * cosTheta + e2 * (float) Math.Sin(phi) * cosTheta + e3 * sinTheta,
            1e-3f,
            float.PositiveInfinity,
            depth);
    }
}

public class SpecularBRDF : BRDF
{
    public float Reflectance { get; set; }

    public SpecularBRDF(float reflectance = 1.0f)
    {      
        Reflectance = reflectance;
    }
    
    public SpecularBRDF(Pigment pigment, float reflectance = 1.0f) : base(pigment)
    {      
        Reflectance = reflectance;
    }
    
    public override Color Eval(Normal normal, Vec directionIn, Vec directionOut, Vec2D coordinates)
    {
        // not necessary to fill because not used, but this is the way:
        /*
        theta_in = acos(normal.to_vec().dot(in_dir))
        theta_out = acos(normal.to_vec().dot(out_dir))

        if abs(theta_in - theta_out) < self.threshold_angle_rad:
            return self.pigment.get_color(uv)
        else:
            return Color(0.0, 0.0, 0.0)
        */
        return new Color();
    }

    public override Ray ScatterRay(PCG pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
    {
        var rayDir = new Vec(incomingDir).Normalize();
        var normalVec = normal.ToVec().Normalize();
        var dotProduct = normal * rayDir;
        return new Ray(
            interactionPoint,
            rayDir - normalVec * 2 * dotProduct,
            1e-5f,
            float.PositiveInfinity,
            depth);
    }
}
