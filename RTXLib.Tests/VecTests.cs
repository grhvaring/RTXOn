
using System;
using System.Numerics;
using Xunit;

namespace RTXLib.Tests
{
	public class VecTests
	{
		// Function for check if 2 float numbers are equal
		 static bool AreClose(float a, float b, float delta=(float)1e-5)
        {
			return ((float)Math.Abs(b - a) <= delta);
		}
			

		[Fact]
		public void TestSum()
		{
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Vec b = new Vec(4.0f, 6.0f, 8.0f);

			Assert.True((a + b).IsClose(new Vec(5.0f, 8.0f, 11.0f)));
			Assert.True((b + a).IsClose(new Vec(5.0f, 8.0f, 11.0f)));
			Assert.False((a + b).IsClose(new Vec(1.0f, 2.0f, 4.0f)));
		}

		[Fact]
		public void TestDifference()
		{
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Vec b = new Vec(4.0f, 6.0f, 8.0f);

			Assert.True((b - a).IsClose(new Vec(3.0f, 4.0f, 5.0f)));
			Assert.True((a - b).IsClose(new Vec(-3.0f, -4.0f, -5.0f)));
			Assert.False((a - b).IsClose(new Vec(1.0f, 2.0f, 4.0f)));
		}

		[Fact]
		public void TestUnaryMinus()
		{
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Vec b = new Vec(-1.0f, 5.0f, -2.0f);
			
			Assert.True((-a).IsClose(new Vec(-1.0f, -2.0f, -3.0f)));
			Assert.True((-b).IsClose(new Vec(1.0f, -5.0f, 2.0f)));
			Assert.False((-a).IsClose(new Vec(1.0f, 2.0f, 3.0f)));
		}

		[Fact]
		public void TestNegation()
		{
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Vec b = new Vec(-1.0f, 5.0f, -2.0f);

			Assert.True(a.Negation().IsClose(new Vec(-1.0f, -2.0f, -3.0f)));
			Assert.True(b.Negation().IsClose(new Vec(1.0f, -5.0f, 2.0f)));
			Assert.False(a.Negation().IsClose(new Vec(1.0f, 2.0f, 3.0f)));
		}

		[Fact]
		public void TestProductScalarVector()
		{
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			float b = 2.0f;
			float c = -3.0f;
			float d = 0.0f;

			Assert.True((a * b).IsClose(new Vec(2.0f, 4.0f, 6.0f)));
			Assert.True((b * a).IsClose(new Vec(2.0f, 4.0f, 6.0f)));
			Assert.True((a * c).IsClose(new Vec(-3.0f, -6.0f, -9.0f)));
			Assert.True((c * a).IsClose(new Vec(-3.0f, -6.0f, -9.0f)));
			Assert.True((a * d).IsClose(new Vec(0.0f, 0.0f, 0.0f)));
			Assert.True((d * a).IsClose(new Vec(0.0f, 0.0f, 0.0f)));

			Assert.False((a * b).IsClose(new Vec(1.0f, 2.0f, 3.0f)));
			Assert.False((b * a).IsClose(new Vec(1.0f, 2.0f, 3.0f)));
		}

		[Fact]
		public void TestDivision()
		{
			Vec a = new Vec(20.0f, 10.0f, -10.0f);
			float b = 2.0f;
			float c = -5.0f;

			Assert.True((a / b).IsClose(new Vec(10.0f, 5.0f, -5.0f)));
			Assert.True((a / c).IsClose(new Vec(-4.0f, -2.0f, 2.0f)));

			Assert.False((a / b).IsClose(new Vec(1.0f, 2.0f, 3.0f)));
		}

		[Fact]
		public void TestScalarProduct()
		{
			Vec a = new Vec(1.0f, 2.0f, -1.0f);
			Vec b = new Vec(2.0f, 3.0f, 5.0f);

			Normal c = new Normal(1.0f, 2.0f, -1.0f);
			Normal d = new Normal(2.0f, 3.0f, 5.0f);

			Assert.True(AreClose(Vec.ScalarProduct(a, b),3.0f));
			Assert.True(AreClose(Vec.ScalarProduct(b, a),3.0f));
			Assert.True(AreClose(Vec.ScalarProduct(a, d), 3.0f));
			Assert.True(AreClose(Vec.ScalarProduct(c, b), 3.0f));
			Assert.True(AreClose(Vec.ScalarProduct(c, d), 3.0f));
			Assert.True(AreClose(a*b, 3.0f));
			Assert.True(AreClose(b*a, 3.0f));
			Assert.True(AreClose(a*d, 3.0f));
			Assert.True(AreClose(c*d, 3.0f));

			Assert.False(AreClose(Vec.ScalarProduct(a, b),2.0f));
		}

		[Fact]
		public void TestCrossProduct()
		{
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Vec b = new Vec(4.0f, 6.0f, 8.0f);

			Normal c = new Normal(1.0f, 2.0f, 3.0f);
			Normal d = new Normal(4.0f, 6.0f, 8.0f);

			Assert.True((Vec.CrossProduct(a, b)).IsClose(new Vec(-2.0f, 4.0f, -2.0f)));	// vec1 x vec2
			Assert.True((Vec.CrossProduct(b, a)).IsClose(new Vec(2.0f, -4.0f, 2.0f)));	// vec2 x vec1
			Assert.True((Vec.CrossProduct(c, b)).IsClose(new Vec(-2.0f, 4.0f, -2.0f)));	// normal x vec
			Assert.True((Vec.CrossProduct(a, d)).IsClose(new Vec(-2.0f, 4.0f, -2.0f)));	// vec x normal
			Assert.True((Vec.CrossProduct(c, d)).IsClose(new Vec(-2.0f, 4.0f, -2.0f)));	// normal x normal

			Assert.False((Vec.CrossProduct(a, b)).IsClose(new Vec(2.0f, 4.0f, 2.0f)));
		}

		[Fact]
		public void TestNorm()
        {
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Assert.True(AreClose((float)Math.Pow(a.Norm(),2), 14.0f));

		}

		[Fact]
		public void TestSquaredNorm()
		{
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Assert.True(AreClose(a.SquaredNorm(), 14.0f));
		}

		[Fact]
		public void TestNormalize()
        {
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Vec b = new Vec(1.0f, 2.0f, 3.0f);

			a.Normalize();
			b = b / b.Norm();

			Assert.True(a.IsClose(b));
		}

		[Fact]
		public void TestCreateNomalizedVec()
		{
			Vec a = new Vec(1.0f, 2.0f, 3.0f);
			Vec b = a.CreateNomalizedVec();
			
			Assert.True(b.IsClose(a / a.Norm()));
		}
	}
}