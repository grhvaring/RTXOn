/// <summary>
/// The class World contains the list of shapes that are present in the image to be generated.
/// </summary>

namespace RTXLib;

public class World
{
	// Field of the class (list of shapes)
	public List<Shape> ShapeList;

	// Default constructor (create an empty list of shape)
	public World()
	{
		ShapeList = new List<Shape>();
	}

	// Add a new shape to the image
	public void Add(Shape shape)
    {
		ShapeList.Add(shape);
    }

	// RayIntersection returns the intersection with the closest of the shapes that are present in the world;
	// If there isn't any intersection the function returns a null value.
	public HitRecord? RayIntersection(Ray ray)
    {	
		// Variable for the closest intersection; it is initially set to null.
		HitRecord? closestIntersection = null;

		foreach(var shape in ShapeList)
        {
			HitRecord? intersection = shape.RayIntersection(ray);

			// Check if the ray intersects a shape.
			// If there is not intersection the program pass to next shape without doing other operation.
			if(!intersection.HasValue)	continue;

			// Check if the last intersection found is closer than the intersection that it is now considedered closed and update.
			// The first condition is needed to assigned a not null value to closedIntersection when the first intersection has been found for the first time.
			// The syntax .Value.T it is need because the variables are nullable.
			if ((!closestIntersection.HasValue) || (intersection.Value.T < closestIntersection.Value.T))
			{
				closestIntersection = intersection;
			}
		}

		return closestIntersection;
    }

}

