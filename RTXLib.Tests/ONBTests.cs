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
                Assert.True(MyLib.AreClose(e1 * e2, 0.0f));
                Assert.True(MyLib.AreClose(e2 * e3, 0.0f));
                Assert.True(MyLib.AreClose(e3 * e1, 0.0f));

                // Verify that each component is normalized
                Assert.True(MyLib.AreClose(e1.SquaredNorm(), 1.0f));
                Assert.True(MyLib.AreClose(e2.SquaredNorm(), 1.0f));
                Assert.True(MyLib.AreClose(e3.SquaredNorm(), 1.0f));

                // Verify that the base has right handness
                var v = Vec.CrossProduct(e1, e2);
                Assert.True(v * e3 > 0);
            }
        }
    }
}


