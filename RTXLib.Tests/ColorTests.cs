using Xunit;

namespace RTXLib.Tests
{
    public class ColorTests
    {

        [Fact]
        public void TestDefaultConstructor()
        {
            Color c = new Color();
            Assert.True(c.is_close(new Color(0.0f, 0.0f, 0.0f)));
        }
        
        [Fact]
        public void TestLeftProductWithScalar()
        {
            // Base cases
            Color c = new Color();
            float a = 2.0f;
            Assert.True((a * c).is_close(new Color()));

            c = new Color(1.0f, 2.0f, 3.0f);
            a = 0.0f;
            Assert.True((a * c).is_close(new Color()));

            // General case
            c = new Color(1.0f, 2.0f, 3.0f);
            a = 1.5f;
            Assert.True((a * c).is_close(new Color(1.5f, 3.0f, 4.5f)));
        }

        [Fact]
        public void TestRightProductWithScalar()
        {

            // Base cases
            Color c = new Color();
            float a = 2.0f;
            Assert.True((c * a).is_close(new Color()));

            c = new Color(1.0f, 2.0f, 3.0f);
            a = 0.0f;
            Assert.True((c * a).is_close(new Color()));

            // General case
            c = new Color(3.0f, 2.0f, 1.0f);
            a = 0.5f;
            Assert.True((c * a).is_close(new Color(1.5f, 1.0f, 0.5f)));
        }

        [Fact]
        public void TestDivisionByScalar()
        {
            // Base case
            Color c = new Color();
            float a = 2.0f;
            Assert.True((c / a).is_close(new Color()));

            // General case
            c = new Color(1.0f, 2.0f, 3.0f);
            a = 4.0f;
            Assert.True((c / a).is_close(new Color(0.25f, 0.5f, 0.75f)));
        }

        [Fact]
        public void TestProductWithColor()
        {
            // Base cases
            Color a = new Color();
            Color b = new Color();
            Assert.True((a * b).is_close(new Color()));

            a = new Color(1.0f, 2.0f, 3.0f);
            b = new Color();
            Assert.True((a * b).is_close(new Color()));

            a = new Color();
            b = new Color(3.0f, 2.0f, 1.0f);
            Assert.True((a * b).is_close(new Color()));

            // General case
            a = new Color(1.0f, 2.0f, 1.0f);
            b = new Color(3.0f, 2.0f, 4.0f);
            Assert.True((a * b).is_close(new Color(3.0f, 4.0f, 4.0f)));
        }

        [Fact]
        public void TestUnaryPlus()
        {
            // Base case
            Color c = new Color();
            Assert.True((+c).is_close(new Color()));

            // General case
            c = new Color(2.0f, 5.0f, 10.0f);
            Assert.True((+c).is_close(new Color(2.0f, 5.0f, 10.0f)));
        }

        [Fact]
        public void TestUnaryMinus()
        {
            // Base case
            Color c = new Color();
            Assert.True((-c).is_close(new Color()));

            // General case
            c = new Color(2.0f, 5.0f, 10.0f);
            Assert.True((-c).is_close(new Color(-2.0f, -5.0f, -10.0f)));
        }

        [Fact]
        public void TestSum()
        {
            // Base cases
            Color a = new Color();
            Color b = new Color();
            Assert.True((a + b).is_close(new Color()));

            a = new Color();
            b = new Color(1.0f, 2.0f, 3.0f);
            Assert.True((a + b).is_close(new Color(1.0f, 2.0f, 3.0f)));

            a = new Color(3.0f, 2.0f, 1.0f);
            b = new Color();
            Assert.True((a + b).is_close(new Color(3.0f, 2.0f, 1.0f)));

            // General case
            a = new Color(1.0f, 5.0f, 10.0f);
            b = new Color(3.3f, 2.2f, 1.1f);
            Assert.True((a + b).is_close(new Color(4.3f, 7.2f, 11.1f)));
        }

        [Fact]
        public void TestDifference()
        {
            // Base cases
            Color a = new Color();
            Color b = new Color();
            Assert.True((a - b).is_close(new Color()));

            a = new Color();
            b = new Color(1.0f, 2.0f, 3.0f);
            Assert.True((a - b).is_close(new Color(-1.0f, -2.0f, -3.0f)));

            a = new Color(3.0f, 2.0f, 1.0f);
            b = new Color();
            Assert.True((a - b).is_close(new Color(3.0f, 2.0f, 1.0f)));

            // General case
            a = new Color(4.0f, 5.0f, 6.0f);
            b = new Color(3.3f, 2.2f, 1.1f);
            Assert.True((a - b).is_close(new Color(0.7f, 2.8f, 4.9f)));
        }
    }
}