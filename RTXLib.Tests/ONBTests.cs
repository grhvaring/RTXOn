using Xunit;
using Xunit.Abstractions;

namespace RTXLib.Tests
{
    public class ONBTests
    {
        private readonly ITestOutputHelper Output;

        public ONBTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void TestCreateONBFromZ()
        {
            var pcg = new PCG();

            // Test multiple ONB using random testing
            for (int i = 0; i < 100; i++)
            {
                Vec normal = new Vec(pcg.RandomFloat(), pcg.RandomFloat(), pcg.RandomFloat());
                normal = normal.Normalize();

                var (e1, e2, e3) = MyLib.CreateONBFromZ(normal);

                // Verify that the z axis is aligned with the normal
                Assert.True(e3.IsClose(normal));

                // Verify that the base is orthogonal
                Assert.True((e1 * e2).IsZero());
                Assert.True((e2 * e3).IsZero());
                Assert.True((e3 * e1).IsZero());

                // Verify that each component is normalized
                Assert.True(e1.SquaredNorm().IsClose(1));
                Assert.True(e2.SquaredNorm().IsClose(1));
                Assert.True(e3.SquaredNorm().IsClose(1));

                // Verify that the base has right handedness
                var v = Vec.CrossProduct(e1, e2);
                Assert.True(v * e3 > 0);
            }
        }
    }
}


