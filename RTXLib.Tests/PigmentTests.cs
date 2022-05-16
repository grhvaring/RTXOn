using Xunit;

namespace RTXLib.Tests;

public class PigmentTests
{
	[Fact]
	public void TestUniformPigment()
	{
		var color = new Color(1.0f, 2.0f, 3.0f);
		var pigment = new UniformPigment(color);

		Vec2D vector1 = new Vec2D(0.0f, 0.0f);
		Vec2D vector2 = new Vec2D(1.0f, 0.0f);
		Vec2D vector3 = new Vec2D(0.0f, 1.0f);
		Vec2D vector4 = new Vec2D(1.0f, 1.0f);

		Assert.True(pigment.GetColor(vector1).IsClose(color));
		Assert.True(pigment.GetColor(vector2).IsClose(color));
		Assert.True(pigment.GetColor(vector3).IsClose(color));
		Assert.True(pigment.GetColor(vector4).IsClose(color));
	}

	[Fact]
	public void TestImagePigment()
	{
		var image = new HdrImage(2, 2);

		var color1 = new Color(1.0f, 2.0f, 3.0f);
		var color2 = new Color(2.0f, 3.0f, 1.0f);
		var color3 = new Color(2.0f, 1.0f, 3.0f);
		var color4 = new Color(3.0f, 2.0f, 1.0f);

		Vec2D vector1 = new Vec2D(0.0f, 0.0f);
		Vec2D vector2 = new Vec2D(1.0f, 0.0f);
		Vec2D vector3 = new Vec2D(0.0f, 1.0f);
		Vec2D vector4 = new Vec2D(1.0f, 1.0f);

		image.SetPixel(0, 0, color1);
		image.SetPixel(1, 0, color2);
		image.SetPixel(0, 1, color3);
		image.SetPixel(1, 1, color4);

		var pigment = new ImagePigment(image);

		Assert.True(pigment.GetColor(vector1).IsClose(color1));
		Assert.True(pigment.GetColor(vector2).IsClose(color2));
		Assert.True(pigment.GetColor(vector3).IsClose(color3));
		Assert.True(pigment.GetColor(vector4).IsClose(color4));
	}

	[Fact]
	public void TestCheckeredPigment()
	{
		var color1 = new Color(1.0f, 2.0f, 3.0f);
		var color2 = new Color(10.0f, 20.0f, 30.0f);

		Vec2D vector1 = new Vec2D(0.25f, 0.25f);
		Vec2D vector2 = new Vec2D(0.75f, 0.25f);
		Vec2D vector3 = new Vec2D(0.25f, 0.75f);
		Vec2D vector4 = new Vec2D(0.75f, 0.75f);

		var pigment = new CheckeredPigment(color1, color2, 2);
 
		// NOTE: with number of steps = 2, the pattern should be the following:
		//
		//              (0.5, 0)
		//   (0, 0) +------+------+ (1, 0)
		//          |      |      |
		//          | col1 | col2 |
		//          |      |      |
		// (0, 0.5) +------+------+ (1, 0.5)
		//          |      |      |
		//          | col2 | col1 |
		//          |      |      |
		//  (0, 1)  +------+------+ (1, 1)
		//              (0.5, 1)

		Assert.True(pigment.GetColor(vector1).IsClose(color1));
		Assert.True(pigment.GetColor(vector2).IsClose(color2));
		Assert.True(pigment.GetColor(vector3).IsClose(color2));
		Assert.True(pigment.GetColor(vector4).IsClose(color1));
	}
}
