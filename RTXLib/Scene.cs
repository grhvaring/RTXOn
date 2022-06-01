using System;
namespace RTXLib;

/// <summary>Struct <c>Scene</c> models a scene read from a scene file.</summary>
public struct Scene
{
	public World World;
	public ICamera? Camera;
	public Dictionary<string, Material> Materials;
	public Dictionary<string, float> FloatVariables;
	public SortedSet<string> OverriddenVariables;

	public Scene()
	{
		World = new World();
		Camera = null;
		Materials = new Dictionary<string, Material>();
		FloatVariables = new Dictionary<string, float>();
		OverriddenVariables = new SortedSet<string>();

	}
}
