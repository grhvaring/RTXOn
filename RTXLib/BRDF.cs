namespace RTXLib;

///<summary>
/// Abstract class that models a generic BRDF (Bidirectional Reflectance Distribution Function).
///</summary>
public abstract class BRDF
{
    public Pigment Pigment { get; }
    
    /// <summary>
    /// Initializes a <c>BRDF</c> object with a given <c>pigment</c> describing the surface behaviour
    /// </summary>
    public BRDF(Pigment? pigment = null)
    {
        Pigment = pigment ?? new UniformPigment();
    }

    public virtual Color Eval(Normal normal, Vec directionIn, Vec directionOut, Vec2D coordinates)
    {
        return Color.BLACK;
    }

    /// <summary>
    /// Abstract method to be implemented in child classes for the generation of a scattered <c>Ray</c>.
    /// </summary>
    public abstract Ray ScatterRay(PCG pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth);
}

///<summary>
///Models an ideal diffusive BRDF (Lambertian BRDF).
///</summary>
public class DiffuseBRDF : BRDF
{
    /// <summary>
    /// Initializes a <c>DiffuseBRDF</c> object with a given <c>pigment</c>
    /// </summary>
    public DiffuseBRDF(Pigment? pigment = null) : base(pigment) {}
    
    /// <summary>
    /// Evaluates the BRDF at a specific point of the surface
    /// </summary>
    /// <param name="normal">Normal to the surface</param>
    /// <param name="directionIn">Incoming direction</param>
    /// <param name="directionOut">Outgoing direction</param>
    /// <param name="coordinates">2D coordinates of the point of the surface</param>
    /// <returns></returns>
    public override Color Eval(Normal normal, Vec directionIn, Vec directionOut, Vec2D coordinates)
    {
        return Pigment.GetColor(coordinates) / (float)Math.PI;
    }

    /// <summary>
    /// Scatter a new <c>Ray</c> with uniform distribution in the solid angle
    /// </summary>
    /// <param name="normal">Normal to the surface</param>
    /// <param name="directionIn">Incoming direction</param>
    /// <param name="directionOut">Outgoing direction</param>
    /// <param name="coordinates">2D coordinates of the point of the surface</param>
    /// <returns></returns>
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
    /// <summary>
    /// Initializes a new <c>SpecularBRDF</c> object, which models a reflective surface, with base color <c>pigment</c>
    /// </summary>
    /// <param name="pigment">Pigment emitted by the surface that adds to the reflections</param>
    public SpecularBRDF(Pigment? pigment = null) : base(pigment) {}
    
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

    /// <summary>
    /// Scatter a new <c>Ray</c> object according to the Law of Reflection
    /// </summary>
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
