using System;
using System.Numerics;
using Xunit;

namespace RTXLib.Tests
{ 
	public class NormalTests
	{
		[Fact]
		public void TestUnaryMinus()
		{
			var a = new Normal(1.0f, 2.0f, -3.0f);
			var b = new Normal(-1.0f, -2.0f, 3.0f);

			Assert.True((-a).IsClose(b));
			
		}

		[Fact]
		public void TestProductScalarVector()
		{
			var a = new Normal(1.0f, 2.0f, 3.0f);
			var b = 2.0f;
			var c = -3.0f;
			var d = 0.0f;

			Assert.True((a * b).IsClose(2.0f, 4.0f, 6.0f));
			Assert.True((b * a).IsClose(2.0f, 4.0f, 6.0f));
			Assert.True((a * c).IsClose(-3.0f, -6.0f, -9.0f));
			Assert.True((c * a).IsClose(-3.0f, -6.0f, -9.0f));
			Assert.True((a * d).IsClose(0.0f, 0.0f, 0.0f));
			Assert.True((d * a).IsClose(0.0f, 0.0f, 0.0f));

			Assert.False((a * b).IsClose(1.0f, 2.0f, 3.0f));
			Assert.False((b * a).IsClose(1.0f, 2.0f, 3.0f));
		}

		[Fact]
		public void TestDivision()
		{
			var a = new Normal(20.0f, 10.0f, -10.0f);
			var b = 2.0f;
			var c = -5.0f;

			Assert.True((a / b).IsClose(10.0f, 5.0f, -5.0f));
			Assert.True((a / c).IsClose(-4.0f, -2.0f, 2.0f));

			Assert.False((a / b).IsClose(1.0f, 2.0f, 3.0f));
		}

		[Fact]
		public void TestNorm()
		{
			var a = new Vec(1.0f, 2.0f, 3.0f);
			Assert.True((a.Norm() * a.Norm() - 14.0f).IsZero());

		}

		[Fact]
		public void TestSquaredNorm()
		{
			var a = new Normal(1.0f, 2.0f, 3.0f);
			Assert.True((a.SquaredNorm - 14.0f).IsZero());
		}

		[Fact]
		public void TestNormalize()
		{
			var a = new Normal(1.0f, 2.0f, 3.0f);
			Assert.True(a.Normalize().IsClose(a / a.Norm));
		}
		
	}
}