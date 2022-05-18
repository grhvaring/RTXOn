namespace RTXLib;

public struct Material
{
    public BRDF BRDF;
    public Pigment EmittedRadiance;

    public Material()
    {
        BRDF = new DiffuseBRDF();
        EmittedRadiance = new UniformPigment();
    }
    
    public Material(Pigment emittedRadiance)
    {
        BRDF = new DiffuseBRDF();
        EmittedRadiance = emittedRadiance;
    }
    
    public Material(BRDF brdf, Pigment emittedRadiance)
    {
        BRDF = brdf;
        EmittedRadiance = emittedRadiance;
    }
}