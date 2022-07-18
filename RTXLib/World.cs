/// <summary>
/// The class World contains the list of shapes that are present in the image to be generated.
/// </summary>

namespace RTXLib;

public class World
{
	public List<Shape> ShapeList;
	
	/// <summary>
	/// Creates a <c>World</c> with an empty list of shapes
	/// </summary>
	public World()
	{
		ShapeList = new List<Shape>();
	}

	/// <summary>
	/// Adds the specified shape to the internal list
	/// </summary>
	public void Add(Shape shape)
    {
		ShapeList.Add(shape);
    }
	
	/// <summary>
	/// Calculates the intersection between a given <c>ray</c> and all the shapes in the internal list
	/// </summary>
	/// <returns><c>HitRecord</c> with the information about the closest intersection if a valid one exists, <c>null otherwise</c></returns>
	public HitRecord? RayIntersection(Ray ray)
    {	
		HitRecord? closestIntersection = null;

		foreach (var shape in ShapeList)
        {
			HitRecord? intersection = shape.RayIntersection(ray);
			if (!intersection.HasValue)	continue;

			// Check whether closestIntersection is still null before comparing its value
			if (!closestIntersection.HasValue || intersection.Value.T < closestIntersection.Value.T)
			{
				closestIntersection = intersection;
			}
		}

		return closestIntersection;
    }

}

