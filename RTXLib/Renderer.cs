namespace RTXLib;

/// <summary>Abstract class <c>Renderer</c> models a generic solver of the render equation.
/// <para>This class is abstract; a concrete derived class should be used in order to actually solve redering equation.</para>
/// </summary>
public abstract class Renderer
{
    ///<summary>Instance variable <c>World</c> represents the list of shapes that form the image.</summary>
    public World World;

    ///<summary>Instance variable <c>BackgroundColor</c> represents the color of the background of the image. If not specified, the color of the background is set to black.</summary>
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

    /// <summary>Method <c>Run</c> runs the renderer for a specified ray.</summary>
    public abstract Color Run(Ray ray);
}

/// <summary>Class <c>OnOffRenderer</c> models a simple two color "on-off" renderer, useful for dubugging puroposes.</summary>
public class OnOffRenderer : Renderer
{
    /// <summary>Instance variable <c>Color</c> represents the color used to color the shapes in the scene. If not specified it is set to white.</summary>
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

    /// <summary>Method <c>Run</c> runs the renderer for a specified ray.</summary>
    public override Color Run(Ray ray)
    {
        return World.RayIntersection(ray).HasValue ? Color : BackgroundColor;
    }
}

/// <summary>Class <c>FlatRenderer</c> models a renderer that solves the rendering equation by neglecting any contribution of the light and taking in account only the pigment of each surface in order to compute the final radiance.</summary>
public class FlatRenderer : Renderer
{
    public FlatRenderer(World world) :  base(world) {}

    public FlatRenderer(World world, Color backgroundColor) : base(world, backgroundColor) {}

    /// <summary>Method <c>Run</c> runs the renderer for a specified ray.</summary>
    public override Color Run(Ray ray)
    {
        var hitRecord = World.RayIntersection(ray);
        if (!hitRecord.HasValue) return BackgroundColor;
        var surfacePoint = hitRecord.Value.SurfacePoint;
        var material = hitRecord.Value.Shape.Material;
        return material.EmittedRadiance.GetColor(surfacePoint) + material.BRDF.Pigment.GetColor(surfacePoint);
    }
}

/// <summary>Class <c>PathTracer</c> models a simple path-tracing renderer that takes in account multiple reflection of the light, using a recursive Monte Carlo algorithm.
/// The algorithm allows the caller to tune the number of rays thrown at each iteration and the maximum depth of the rays. 
/// The algorithm implements the Russian roulette in order to take a finite time to complete the calculation even it the maxium depth of the rays is set to infinite.
/// </summary>
public class PathTracer : Renderer
{
    ///<summary>Instance variable <c>Pcg</c> represents the generator of random floating point numbers. If not specified it is set to a default PCG</summary>
    public PCG Pcg;
    ///<summary>Instance variable <c>NumberOfRays</c> represents the number of rays thrown at each iteration. If not specified it is set to 10.</summary>
    public int NumberOfRays;
    ///<summary>Instance variable <c>MaxDepth</c> represents the maximum depth of a ray before the contribuiton of the ray is discarded. If not specified it is set to 2.</summary>
    public int MaxDepth;
    ///<summary>Instance variable <c>RussianRouletteLimit</c> represents the depth of a ray that triggers the start of the Russian roulette algorithm in order to end the recursion. If not specified it is set to 3.</summary>
    public int RussianRouletteLimit;

    public PathTracer(World world) : base(world) 
    {
        Pcg = new PCG();
        NumberOfRays = 10;
        MaxDepth = 2;
        RussianRouletteLimit = 3;
    }
    public PathTracer(World world, Color backgroundColor, PCG pcg, int numberOfRays = 10, int maxDepth = 2, int russianRouletteLimit = 3) : base(world, backgroundColor) 
    {
        Pcg = pcg;
        NumberOfRays = numberOfRays;
        MaxDepth = maxDepth;
        RussianRouletteLimit = russianRouletteLimit;
    }
    
    public PathTracer(World world, int numberOfRays = 10, int maxDepth = 2) : base(world) 
    {
        Pcg = new PCG();
        NumberOfRays = numberOfRays;
        MaxDepth = maxDepth;
        RussianRouletteLimit = 3;
    }
    
    public PathTracer(World world, int maxDepth = 2) : base(world) 
    {
        Pcg = new PCG();
        NumberOfRays = 10;
        MaxDepth = maxDepth;
        RussianRouletteLimit = 3;
    }

    /// <summary>Method <c>Run</c> runs the renderer for a specified ray.</summary>
    public override Color Run(Ray ray)
    {
        // If the ray does not hit anything, return black color
        if (ray.Depth > MaxDepth) return new Color(0.0f, 0.0f, 0.0f);

        var hitRecord = World.RayIntersection(ray);

        // If the ray hits the background, return the background color
        if (!hitRecord.HasValue) return BackgroundColor;

        Material hitMaterial = hitRecord.Value.Shape.Material;
        Color hitColor = hitMaterial.BRDF.Pigment.GetColor(hitRecord.Value.SurfacePoint);
        Color emittedRadiance = hitMaterial.EmittedRadiance.GetColor(hitRecord.Value.SurfacePoint);

        float hitColorLum = Math.Max(Math.Max(hitColor.R, hitColor.G), hitColor.B);

        // If the depth of the ray equals a certain limit, start the Russian roulette algorithm
        if (ray.Depth >= RussianRouletteLimit)
        {
            float q = Math.Max(0.05f, 1.0f - hitColorLum);
            if (Pcg.RandomFloat() > q)
            {
                hitColor = hitColor * (float)(1.0 / (1.0 - q));
            }
            else
            {
                return emittedRadiance;
            }
        }

        // Monte Carlo integration section

        Color cumulativeRadiance = new Color(0.0f, 0.0f, 0.0f);

        // Start recursion if it is worth
        if (hitColorLum > 0.0f)
        {
            for (int rayIndex = 0; rayIndex < NumberOfRays; rayIndex++)
            {
                Ray newRay = hitMaterial.BRDF.ScatterRay(Pcg, hitRecord.Value.Ray.Dir, hitRecord.Value.WorldPoint, hitRecord.Value.Normal, ray.Depth + 1);

                // Recursive call of Run
                var newRadiance = Run(newRay);
                cumulativeRadiance += hitColor * newRadiance;
            }
        }

        return emittedRadiance + cumulativeRadiance * (float)(1.0f / NumberOfRays);
    }
}