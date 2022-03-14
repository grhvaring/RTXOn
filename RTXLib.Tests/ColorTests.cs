using Xunit;

namespace RTXLib.Tests
{
    public class ColorTests
    {
        [Fact]
        public void TestLeftProductWithScalar()
        {
            Color c = new Color(10.0f, 55.0f, 33.0f);
            double a = 1.5f;
            Assert.True((a * c).is_close(new Color(15.0f, 82.5f, 49.5f)));
        }
        
        [Fact]
        public void TestRightProductWithScalar()
        {
            Color c = new Color(102.0f, 59.0f, 133.0f);
            double a = 0.5f;
            Assert.True((c * a).is_close(new Color(51.0f, 29.5f, 66.5f)));
        }
        
        [Fact]
        public void TestDivisionByScalar()
        {
            Color c = new Color(152.0f, 259.0f, 3.0f);
            double a = 10.0f;
            Assert.True((c / a).is_close(new Color(15.2f, 25.9f, 0.3f)));
        }

        [Fact]
        public void TestProductWithColor()
        {
            Color a = new Color(2.0f, 5.0f, 10.0f);
            Color b = new Color(3.0f, 7.0f, 12.0f);
            Assert.True((a * b).is_close(new Color(6.0f, 35.0f, 120.0f)));
        }

        [Fact]
        public void TestUnaryPlus()
        {
            Color a = new Color(2.0f, 5.0f, 10.0f);
            Assert.True((+a).is_close(new Color(2.0f, 5.0f, 10.0f)));
        }

        [Fact]
        public void TestUnaryMinus()
        {
            Color a = new Color(2.0f, 5.0f, 10.0f);
            Assert.True((-a).is_close(new Color(-2.0f, -5.0f, -10.0f)));
        }

        [Fact]
        public void TestAdd()
        {
            Color a = new Color(2.0f, 5.0f, 10.0f);
            Color b = new Color(3.2f, 2.1f, 5.6f);
            Assert.True((a + b).is_close(new Color(5.2f, 7.1f, 15.6f)));
        }
    
        [Fact]
        public void TestDifference()
        {
            Color a = new Color(3.9f, 5.5f, 10.1f);
            Color b = new Color(3.2f, 2.7f, 5.2f);
            Assert.True((a - b).is_close(new Color(0.7f, 2.8f, 4.9f)));
        }
    }   
}