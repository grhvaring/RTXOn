﻿// This file implements the test for the class HdrImage

using System.IO;          // Streams
using System.Linq;        // Necessary for SequenceEqual()
using Xunit;
using Xunit.Abstractions; // For debugging 

namespace RTXLib.Tests
{
    public class HdrImageTests
    {
        // Visualization of outputs during a test
        private readonly ITestOutputHelper _testOutputHelper;

        public HdrImageTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        // This test checks if width, height and number of pixel of a given image are set correctly
        [Fact]
        public void TestCreationHdrImage()
        {
            HdrImage testImage = new HdrImage(10, 5);

            Assert.True(testImage.Width == 10);        // Check correct width
            Assert.True(testImage.Height == 5);        // Check correct height
            Assert.True(testImage.NPixels == 10 * 5);  // Check correct number of pixel
        }

        // This test checks if the offset linked to a given coordinate is calculated correctly
        [Fact]
        public void TestPixelOffset()
        {
            HdrImage testImage = new HdrImage(10, 5);

            // Check correct offsets
            Assert.True(testImage.pixel_offset(0, 0) == 0);                 // Check first pixel
            Assert.True(testImage.pixel_offset(10, 5) == (5 * 10 + 10));      // Check last pixel
            Assert.True(testImage.pixel_offset(1, 2) == (2 * 10 + 1));        // Check correct offset for pixel (1,2)

            // Check wrong offset
            Assert.False(testImage.pixel_offset(1, 2) == 3);                // Check wrong offset for pixel (1,2)
        }

        // This test checks if the test that validates the correctness of a given coordinate works correctly
        [Fact]
        public void TestValidateCoordinates()
        {
            HdrImage testImage = new HdrImage(10, 5);

            // Check correct coordinates
            Assert.True(testImage.valid_coordinates(0, 0));     // Check correct extremal coordinates
            Assert.True(testImage.valid_coordinates(10, 0));    // Check correct extremal coordinates
            Assert.True(testImage.valid_coordinates(0, 5));     // Check correct extremal coordinates
            Assert.True(testImage.valid_coordinates(10, 5));    // Check correct extremal coordinates
            Assert.True(testImage.valid_coordinates(1, 2));     // Check correct not extremal coordinates

            // Check one coordinate correct and other coordinate wrong
            Assert.False(testImage.valid_coordinates(-1, 2));   // Check wrong coordinates (x negative, y correct)
            Assert.False(testImage.valid_coordinates(11, 2));   // Check wrong coordinates (x too high, y correct)
            Assert.False(testImage.valid_coordinates(2, -1));   // Check wrong coordinates (x correct, y negative)
            Assert.False(testImage.valid_coordinates(2, 6));    // Check wrong coordinates (x correct, y too high)

            // Check both coordinates wrong
            Assert.False(testImage.valid_coordinates(-1, -2));  // Check wrong coordinates (x negative, y negative)
            Assert.False(testImage.valid_coordinates(11, 6));   // Check wrong coordinates (x too high, y too high)
            Assert.False(testImage.valid_coordinates(-1, 6));   // Check wrong coordinates (x negative, y too high)
            Assert.False(testImage.valid_coordinates(11, -2));  // Check wrong coordinates (x too high, y negative)
        }

        // This test checks if the function that set a coordinate to a color and get the color linked to a given coordinate work correctly.
        // The function checks also if the pixels with unassigned colors are correctly set to color (0, 0, 0)
        [Fact]
        public void TestSetGetPixel()
        {
            HdrImage testImage = new HdrImage(10, 5);
            Color referenceColor1 = new Color(1.0f, 2.0f, 3.0f);
            Color referenceColor2 = new Color(2.0f, 4.0f, 6.0f);
            Color defaultColor = new Color(0.0f, 0.0f, 0.0f);

            testImage.set_pixel(2, 3, referenceColor1);
            testImage.set_pixel(3, 4, referenceColor2);

            // Check correct color
            Assert.True((testImage.get_pixel(2, 3)).IsClose(referenceColor1));  // Check if color in position (2,3) is correct
            Assert.True((testImage.get_pixel(3, 4)).IsClose(referenceColor2));  // Check if color in position (3,4) is correct
            Assert.True((testImage.get_pixel(1, 2)).IsClose(defaultColor));      // Check if color in unassigned position (1,2) is the default color

            // Check wrong color
            Assert.False((testImage.get_pixel(3, 4)).IsClose(referenceColor1)); // Check if color in position (3,4) is different from color set in position (2,3)
        }

        [Fact]
        public void TestParseImgSize()
        {
            HdrImage testImage = new HdrImage(2, 1);
            
            // Correct format
            testImage.ParseImgSize("3 2");
            Assert.True(testImage.Width == 3 && testImage.Height == 2);
            
            // Incorrect formats
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseImgSize("-1 2"));
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseImgSize("1 -2"));
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseImgSize("-1 -2"));
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseImgSize("2"));
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseImgSize(""));
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseImgSize("-1"));
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseImgSize("1 2 3"));
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseImgSize("abc"));


        }

        [Fact]
        public void TestParseEndianness()
        {
            HdrImage testImage = new HdrImage(2, 1);

            float endianness = testImage.ParseEndianness("1.0");
            Assert.True(endianness == 1.0f);
            
            endianness = testImage.ParseEndianness("-1.0");
            Assert.True(endianness == -1.0f);
            
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseEndianness("0"));
            Assert.Throws<InvalidPfmFileFormat>(() => testImage.ParseEndianness("abc"));
        }

        [Fact]
        public void TestWritePfmLittleEndian()
        {
            byte[] referenceBytesLe = {
                0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
                0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
                0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
                0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
                0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
                0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
                0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42
            };
            
            HdrImage testImage = new HdrImage(3, 2);
            
            testImage.set_pixel(0,0, new Color(10.0f, 20.0f, 30.0f));
            testImage.set_pixel(1,0, new Color(40.0f, 50.0f, 60.0f));
            testImage.set_pixel(2,0, new Color(70.0f, 80.0f, 90.0f));
            testImage.set_pixel(0,1, new Color(100.0f, 200.0f, 300.0f));
            testImage.set_pixel(1,1, new Color(400.0f, 500.0f, 600.0f));
            testImage.set_pixel(2,1, new Color(700.0f, 800.0f, 900.0f));

            // Long way (useful for debugging)
            using (MemoryStream memoryStream = new MemoryStream(referenceBytesLe.Length))
            {
                // Write the content of testImage to the memStream buffer as a PFM image
                testImage.WritePfm(memoryStream, -1.0);
                
                // Set the position to the beginning of the stream.
                memoryStream.Seek(0, SeekOrigin.Begin);
                
                // Scan the buffer to find inconsistencies with the reference
                for (int i = 0; i < referenceBytesLe.Length; ++i)
                {
                    byte b = (byte)memoryStream.ReadByte();
                    //_testOutputHelper.WriteLine($"{b} =? {referenceBytes[i]}");
                    Assert.True(b == referenceBytesLe[i]);
                }
            }
        }
        
        [Fact]
        public void TestWritePfmBigEndian()
        {
            byte[] referenceBytesBe = {
                0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2b, 0x31, 0x2e, 0x30, 0x0a, 
                0x42, 0xc8, 0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00, 
                0x43, 0xc8, 0x00, 0x00, 0x43, 0xfa, 0x00, 0x00, 0x44, 0x16, 0x00, 0x00, 
                0x44, 0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61, 0x00, 0x00,
                0x41, 0x20, 0x00, 0x00, 0x41, 0xa0, 0x00, 0x00, 0x41, 0xf0, 0x00, 0x00, 
                0x42, 0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70, 0x00, 0x00, 
                0x42, 0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00
            };

            HdrImage testImage = new HdrImage(3, 2);
            
            testImage.set_pixel(0,0, new Color(10.0f, 20.0f, 30.0f));
            testImage.set_pixel(1,0, new Color(40.0f, 50.0f, 60.0f));
            testImage.set_pixel(2,0, new Color(70.0f, 80.0f, 90.0f));
            testImage.set_pixel(0,1, new Color(100.0f, 200.0f, 300.0f));
            testImage.set_pixel(1,1, new Color(400.0f, 500.0f, 600.0f));
            testImage.set_pixel(2,1, new Color(700.0f, 800.0f, 900.0f));

            using (MemoryStream memoryStream = new MemoryStream(referenceBytesBe.Length))
            {
                // Write the content of testImage to the memoryStream buffer as a PFM image
                testImage.WritePfm(memoryStream, +1.0);
                
                // Convert buffer into Array of bytes (discarding non-used elements)
                byte[] myBytesBe = memoryStream.ToArray();
                
                // Test equality with the reference bytes
                Assert.True(myBytesBe.SequenceEqual(referenceBytesBe));
            }
        }
    }
}