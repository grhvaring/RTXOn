// This file implements the test for the class HdrImage

using Xunit;

namespace RTXLib.Tests
{
    public class HdrImageTests
    {
        // This test checks if width, height and number of pixel of a given image are set correctly
        [Fact]
        public void TestCreationHdrImage()
        {
            HdrImage test_image = new HdrImage(10, 5);

            Assert.True(test_image.Width == 10);        // Check correct width
            Assert.True(test_image.Height == 5);        // Check correct height
            Assert.True(test_image.NPixels == 10 * 5);  // Check correct number of pixel
        }

        // This test checks if the offset linked to a given coordinate is calculated correctly
        [Fact]
        public void TestPixelOffset()
        {
            HdrImage test_image = new HdrImage(10, 5);

            // Check correct offsets
            Assert.True(test_image.pixel_offset(0, 0) == 0);                 // Check first pixel
            Assert.True(test_image.pixel_offset(10, 5) == (5 * 10 + 10));      // Check last pixel
            Assert.True(test_image.pixel_offset(1, 2) == (2 * 10 + 1));        // Check correct offset for pixel (1,2)

            // Check wrong offset
            Assert.False(test_image.pixel_offset(1, 2) == 3);                // Check wrong offset for pixel (1,2)
        }

        // This test checks if the test that validates the correctness of a given coordinate works correctly
        [Fact]
        public void TestValidateCoordinates()
        {
            HdrImage test_image = new HdrImage(10, 5);

            // Check correct coordinates
            Assert.True(test_image.valid_coordinates(0, 0));     // Check correct extremal coordinates
            Assert.True(test_image.valid_coordinates(10, 0));    // Check correct extremal coordinates
            Assert.True(test_image.valid_coordinates(0, 5));     // Check correct extremal coordinates
            Assert.True(test_image.valid_coordinates(10, 5));    // Check correct extremal coordinates
            Assert.True(test_image.valid_coordinates(1, 2));     // Check correct not extremal coordinates

            // Check one coordinate correct and other coordinate wrong
            Assert.False(test_image.valid_coordinates(-1, 2));   // Check wrong coordinates (x negative, y correct)
            Assert.False(test_image.valid_coordinates(11, 2));   // Check wrong coordinates (x too high, y correct)
            Assert.False(test_image.valid_coordinates(2, -1));   // Check wrong coordinates (x correct, y negative)
            Assert.False(test_image.valid_coordinates(2, 6));    // Check wrong coordinates (x correct, y too high)

            // Check both coordinates wrong
            Assert.False(test_image.valid_coordinates(-1, -2));  // Check wrong coordinates (x negative, y negative)
            Assert.False(test_image.valid_coordinates(11, 6));   // Check wrong coordinates (x too high, y too high)
            Assert.False(test_image.valid_coordinates(-1, 6));   // Check wrong coordinates (x negative, y too high)
            Assert.False(test_image.valid_coordinates(11, -2));  // Check wrong coordinates (x too high, y negative)
        }

        // This test checks if the function that set a coordinate to a color and get the color linked to a given coordinate work correctly.
        // The function checks also if the pixels with unassigned colors are correctly set to color (0, 0, 0)
        [Fact]
        public void TestSetGetPixel()
        {
            HdrImage test_image = new HdrImage(10, 5);
            Color reference_color_1 = new Color(1.0, 2.0, 3.0);
            Color reference_color_2 = new Color(2.0, 4.0, 6.0);
            Color default_color = new Color(0.0, 0.0, 0.0);

            test_image.set_pixel(2, 3, reference_color_1);
            test_image.set_pixel(3, 4, reference_color_2);

            // Check correct color
            Assert.True((test_image.get_pixel(2, 3)).is_close(reference_color_1));  // Check if color in position (2,3) is correct
            Assert.True((test_image.get_pixel(3, 4)).is_close(reference_color_2));  // Check if color in position (3,4) is correct
            Assert.True((test_image.get_pixel(1, 2)).is_close(default_color));      // Check if color in unassigned position (1,2) is the default color

            // Check wrong color
            Assert.False((test_image.get_pixel(3, 4)).is_close(reference_color_1)); // Check if color in position (3,4) is different from color set in position (2,3)
        }
    }
}