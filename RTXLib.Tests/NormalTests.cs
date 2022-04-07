using System;
using System.Numerics;
using Xunit;

namespace RTXLib.Tests
{ 
	public class NormalTests
	{
		static bool AreClose(float a, float b, float delta = (float)1e-5)
		{
			return ((float)Math.Abs(b - a) <= delta);
		}

		[Fact]
		public void TestUnaryMinus()
		{
			Normal a = new Normal(1.0f, 2.0f, -3.0f);
			Normal b = new Normal(-1.0f, -2.0f, 3.0f);

			Assert.True((-a).IsClose(b));
			
		}

		[Fact]
		public void TestNegation()
		{
			Normal a = new Normal(1.0f, 2.0f, -3.0f);
			Normal b = new Normal(-1.0f, -2.0f, 3.0f);

			Assert.True((a.Negation()).IsClose(b));

		}

		[Fact]
		public void TestProductScalarVector()
		{
			Normal a = new Normal(1.0f, 2.0f, 3.0f);
			float b = 2.0f;
			float c = -3.0f;
			float d = 0.0f;

			Assert.True((a * b).IsClose(new Normal(2.0f, 4.0f, 6.0f)));
			Assert.True((b * a).IsClose(new Normal(2.0f, 4.0f, 6.0f)));
			Assert.True((a * c).IsClose(new Normal(-3.0f, -6.0f, -9.0f)));
			Assert.True((c * a).IsClose(new Normal(-3.0f, -6.0f, -9.0f)));
			Assert.True((a * d).IsClose(new Normal(0.0f, 0.0f, 0.0f)));
			Assert.True((d * a).IsClose(new Normal(0.0f, 0.0f, 0.0f)));

			Assert.False((a * b).IsClose(new Normal(1.0f, 2.0f, 3.0f)));
			Assert.False((b * a).IsClose(new Normal(1.0f, 2.0f, 3.0f)));
		}

		[Fact]
		public void TestDivision()
		{
			Normal a = new Normal(20.0f, 10.0f, -10.0f);
			float b = 2.0f;
			float c = -5.0f;

			Assert.True((a / b).IsClose(new Normal(10.0f, 5.0f, -5.0f)));
			Assert.True((a / c).IsClose(new Normal(-4.0f, -2.0f, 2.0f)));

			Assert.False((a / b).IsClose(new Normal(1.0f, 2.0f, 3.0f)));
		}

		[Fact]
		public void TestNorm()
		{
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Assert.True(AreClose((float)Math.Pow(a.Norm(), 2), 14.0f));

		}

		[Fact]
		public void TestSquaredNorm()
		{
			Normal a = new Normal(1.0f, 2.0f, 3.0f);
			Assert.True(AreClose(a.SquaredNorm(), 14.0f));
		}

		[Fact]
		public void TestNormalize()
		{
			Normal a = new Normal(1.0f, 2.0f, 3.0f);
			Normal b = new Normal(1.0f, 2.0f, 3.0f);

			a.Normalize();
			b = b / b.Norm();

			Assert.True(a.IsClose(b));
		}

		[Fact]
		public void TestCreateNomalizedNormal()
		{
			Normal a = new Normal(1.0f, 2.0f, 3.0f);
			Normal b = a.CreateNomalizedNormal();

			Assert.True(b.IsClose(a / a.Norm()));
		}
	}
}