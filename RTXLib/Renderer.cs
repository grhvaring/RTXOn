namespace RTXLib;

public abstract class Renderer
{
    public World World;
    public Color BackgroundColor;

    public Renderer(World world)
    {
        World = world;
        BackgroundColor = Color.BLACK;
    }
    
    public Renderer(World world, Color backgroundColor)
    {
        World = world;
        BackgroundColor = backgroundColor;
    }

    public abstract Color Run(Ray ray);
}

public class OnOffRenderer : Renderer
{
    public Color Color;
    public OnOffRenderer(World world) :  base(world)
    {
        Color = Color.WHITE;
    }

    public OnOffRenderer(World world, Color backgroundColor) : base(world, backgroundColor)
    {
        Color = Color.WHITE;
    }

    public OnOffRenderer(World world, Color backgroundColor, Color color) : base(world, backgroundColor)
    {
        Color = color;
    }

    public override Color Run(Ray ray)
    {
        return World.RayIntersection(ray).HasValue ? Color : BackgroundColor;
    }
}

public class FlatRenderer : Renderer
{
    public FlatRenderer(World world) :  base(world) {}

    public FlatRenderer(World world, Color backgroundColor) : base(world, backgroundColor) {}

    public override Color Run(Ray ray)
    {
        var hitRecord = World.RayIntersection(ray);
        if (!hitRecord.HasValue) return BackgroundColor;
        var surfacePoint = hitRecord.Value.SurfacePoint;
        var material = hitRecord.Value.Shape.Material;
        return material.EmittedRadiance.GetColor(surfacePoint) + material.BRDF.Pigment.GetColor(surfacePoint);
    }
}