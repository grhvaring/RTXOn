using RTXLib;
using System.Runtime.InteropServices;
///<summary>
///Abstract class <c>Renderer</c> models a generic solver for the render equation (renderer).
///</summary>
public abstract class Renderer
{
    public World World { get; set; }
    public Color BackgroundColor { get; set; }

    // To be modified to use black as a background color by defaul
    public Renderer(World world,  Color backgroundColor)
    {
        World = world;
        BackgroundColor = backgroundColor;
    }
    public abstract Color Call(Ray ray);

}

///<summary>
///Class <c>OnOffRenderer</c> models a simple 2-color rederer: the background is set to a color while all other object on the scene are set to another same color.
///</summary>
public class OnOffRenderer : Renderer 
{ 
    public Color Color { get; set; }

    public OnOffRenderer(World world,  Color backgroundColor, Color color) : base(world, backgroundColor)
    {
        Color = color;
    }

    public override Color Call(Ray ray)
    {
        if ((World.RayIntersection(ray)).HasValue)
            return Color;
        else
            return BackgroundColor;
    }
}

///<summary>
///Class <c>Renderer</c> models a renderer that estimates the the solution of the rendering equation by neglecting any contribution of the light.
///It just uses the pigment of each surface to determine how to compute the final radiance.
///</summary>
public class FlatRenderer : Renderer
{
    public Color Color { get; set; }

    public FlatRenderer(World world, Color backgroundColor) : base(world, backgroundColor) { }
    
    // To be modified and continued !!!
    public override Color Call(Ray ray)
    {
        var hit = World.RayIntersection(ray);

        // Why is needed to use this strange construction to obtain a value ?
        var material = (hit.Value.Shape).Material;
       
        if (hit.HasValue == false)
            return BackgroundColor;
        else
            // To be changed
            return BackgroundColor;

    }
}