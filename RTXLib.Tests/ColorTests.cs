using Xunit;

namespace RTXLib.Tests
{
    public class ColorTests
    {

        [Fact]
        public void TestDefaultConstructor()
        {
            var c = new Color();
            Assert.True(c.IsClose(0));
        }
        
        [Fact]
        public void TestLeftProductWithScalar()
        {
            // Base cases
            var c = new Color();
            var a = 2.0f;
            Assert.True((a * c).IsClose(0));

            c = new Color(1.0f, 2.0f, 3.0f);
            a = 0.0f;
            Assert.True((a * c).IsClose(0));

            // General case
            c = new Color(1.0f, 2.0f, 3.0f);
            a = 1.5f;
            Assert.True((a * c).IsClose(1.5f, 3.0f, 4.5f));
        }

        [Fact]
        public void TestRightProductWithScalar()
        {

            // Base cases
            var c = new Color();
            var a = 2.0f;
            Assert.True((c * a).IsClose(0));

            c = new Color(1.0f, 2.0f, 3.0f);
            a = 0.0f;
            Assert.True((c * a).IsClose(0));

            // General case
            c = new Color(3.0f, 2.0f, 1.0f);
            a = 0.5f;
            Assert.True((c * a).IsClose(1.5f, 1.0f, 0.5f));
        }

        [Fact]
        public void TestDivisionByScalar()
        {
            // Base case
            var c = new Color();
            var a = 2.0f;
            Assert.True((c / a).IsClose(0));

            // General case
            c = new Color(1.0f, 2.0f, 3.0f);
            a = 4.0f;
            Assert.True((c / a).IsClose(0.25f, 0.5f, 0.75f));
        }

        [Fact]
        public void TestProductWithColor()
        {
            // Base cases
            var a = new Color();
            var b = new Color();
            Assert.True((a * b).IsClose(0));

            a = new Color(1.0f, 2.0f, 3.0f);
            b = new Color();
            Assert.True((a * b).IsClose(0));

            a = new Color();
            b = new Color(3.0f, 2.0f, 1.0f);
            Assert.True((a * b).IsClose(0));

            // General case
            a = new Color(1.0f, 2.0f, 1.0f);
            b = new Color(3.0f, 2.0f, 4.0f);
            Assert.True((a * b).IsClose(3.0f, 4.0f, 4.0f));
        }

        [Fact]
        public void TestUnaryPlus()
        {
            // Base case
            var c = new Color();
            Assert.True((+c).IsClose(0));

            // General case
            c = new Color(2.0f, 5.0f, 10.0f);
            Assert.True((+c).IsClose(2.0f, 5.0f, 10.0f));
        }

        [Fact]
        public void TestUnaryMinus()
        {
            // Base case
            var c = new Color();
            Assert.True((-c).IsClose(0));

            // General case
            c = new Color(2.0f, 5.0f, 10.0f);
            Assert.True((-c).IsClose(-2.0f, -5.0f, -10.0f));
        }

        [Fact]
        public void TestSum()
        {
            // Base cases
            var a = new Color();
            var b = new Color();
            Assert.True((a + b).IsClose(0));

            a = new Color();
            b = new Color(1.0f, 2.0f, 3.0f);
            Assert.True((a + b).IsClose(1.0f, 2.0f, 3.0f));

            a = new Color(3.0f, 2.0f, 1.0f);
            b = new Color();
            Assert.True((a + b).IsClose(3.0f, 2.0f, 1.0f));

            // General case
            a = new Color(1.0f, 5.0f, 10.0f);
            b = new Color(3.3f, 2.2f, 1.1f);
            Assert.True((a + b).IsClose(4.3f, 7.2f, 11.1f));
        }

        [Fact]
        public void TestDifference()
        {
            // Base cases
            var a = new Color();
            var b = new Color();
            Assert.True((a - b).IsClose(0));

            a = new Color();
            b = new Color(1.0f, 2.0f, 3.0f);
            Assert.True((a - b).IsClose(-1.0f, -2.0f, -3.0f));

            a = new Color(3.0f, 2.0f, 1.0f);
            b = new Color();
            Assert.True((a - b).IsClose(3.0f, 2.0f, 1.0f));

            // General case
            a = new Color(4.0f, 5.0f, 6.0f);
            b = new Color(3.3f, 2.2f, 1.1f);
            Assert.True((a - b).IsClose(0.7f, 2.8f, 4.9f));
        }
        
        [Fact]
        public void TestLuminosity()
        {
            var color1 = new Color(1.0f, 2.0f, 3.0f);
            var color2 = new Color(5.0f, 8.0f, 11.0f);
            Assert.True(2.0f == color1.Luminosity());
            Assert.True(8.0f == color2.Luminosity());
        } 
    }
}