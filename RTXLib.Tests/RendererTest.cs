using Xunit;
using Xunit.Abstractions;

namespace RTXLib.Tests
{
	public class RendererTests
	{

		private readonly ITestOutputHelper Output;

		public RendererTests(ITestOutputHelper output)
		{
			Output = output;
		}

		// Furnace test for PathTracer
		[Fact]
		public void TestFurnace()
		{
			var pcg = new PCG();

			for(int i = 0; i < 5; i++)
			{
				var world = new World();

				var emittedRadiance = pcg.RandomFloat();
				var reflectance = pcg.RandomFloat();
				var color1 = new Color(1.0f, 1.0f, 1.0f);

				var pigment1 = new UniformPigment(color1 * reflectance);
				var pigment2 = new UniformPigment(color1 * emittedRadiance);
				var diffuseBRDF = new DiffuseBRDF(pigment1);
				var enclosureMaterial = new Material(diffuseBRDF, pigment2);

				var sphere = new Sphere(enclosureMaterial, 0, 0, 0);
				world.Add(sphere);

				var pathTracer = new PathTracer(world, Color.BLACK, pcg, 1, 100, 101);

				var origin = new Point(0.0f, 0.0f, 0.0f);
				var dir = new Vec(1.0f, 0.0f, 0.0f);
				var ray = new Ray(origin, dir);

				var color2 = pathTracer.Run(ray);
				var expected = emittedRadiance / (1.0f - reflectance);

				Output.WriteLine(expected.ToString());
				Output.WriteLine(color2.R.ToString());
				Output.WriteLine(color2.G.ToString());
				Output.WriteLine(color2.B.ToString());
				Output.WriteLine("");

				Assert.True(expected.IsClose(color2.R, 1e-3));
				Assert.True(expected.IsClose(color2.G, 1e-3));
				Assert.True(expected.IsClose(color2.B, 1e-3));
			}
		}
	}
}