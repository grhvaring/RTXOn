// This file implements the automated tests for the class HdrImage

using System.IO;          // Streams
using System.Linq;        // Necessary for SequenceEqual()
using System.Text;
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
            var testImage = new HdrImage(10, 5);

            Assert.True(testImage.Width == 10);        // Check correct width
            Assert.True(testImage.Height == 5);        // Check correct height
            Assert.True(testImage.NPixels == 10 * 5);  // Check correct number of pixel
        }

        // This test checks if the offset linked to a given coordinate is calculated correctly
        [Fact]
        public void TestPixelOffset()
        {
            var testImage = new HdrImage(10, 5);

            // Check correct offsets
            Assert.True(testImage.PixelOffset(0, 0) == 0);                   // Check first pixel
            Assert.True(testImage.PixelOffset(10, 5) == (5 * 10 + 10));      // Check last pixel
            Assert.True(testImage.PixelOffset(1, 2) == (2 * 10 + 1));        // Check correct offset for pixel (1,2)

            // Check wrong offset
            Assert.False(testImage.PixelOffset(1, 2) == 3);                // Check wrong offset for pixel (1,2)
        }

        // This test checks if the test that validates the correctness of a given coordinate works correctly
        [Fact]
        public void TestValidCoordinates()
        {
            var testImage = new HdrImage(10, 5);

            // Check correct coordinates
            Assert.True(testImage.ValidCoordinates(0, 0));     // Check correct extremal coordinates
            Assert.True(testImage.ValidCoordinates(10, 0));    // Check correct extremal coordinates
            Assert.True(testImage.ValidCoordinates(0, 5));     // Check correct extremal coordinates
            Assert.True(testImage.ValidCoordinates(10, 5));    // Check correct extremal coordinates
            Assert.True(testImage.ValidCoordinates(1, 2));     // Check correct not extremal coordinates

            // Check one coordinate correct and other coordinate wrong
            Assert.False(testImage.ValidCoordinates(-1, 2));   // Check wrong coordinates (x negative, y correct)
            Assert.False(testImage.ValidCoordinates(11, 2));   // Check wrong coordinates (x too high, y correct)
            Assert.False(testImage.ValidCoordinates(2, -1));   // Check wrong coordinates (x correct, y negative)
            Assert.False(testImage.ValidCoordinates(2, 6));    // Check wrong coordinates (x correct, y too high)

            // Check both coordinates wrong
            Assert.False(testImage.ValidCoordinates(-1, -2));  // Check wrong coordinates (x negative, y negative)
            Assert.False(testImage.ValidCoordinates(11, 6));   // Check wrong coordinates (x too high, y too high)
            Assert.False(testImage.ValidCoordinates(-1, 6));   // Check wrong coordinates (x negative, y too high)
            Assert.False(testImage.ValidCoordinates(11, -2));  // Check wrong coordinates (x too high, y negative)
        }

        // This test checks if the function that set a coordinate to a color and get the color linked to a given coordinate work correctly.
        // The function checks also if the pixels with unassigned colors are correctly set to color (0, 0, 0)
        [Fact]
        public void TestSetGetPixel()
        {
            var testImage = new HdrImage(10, 5);
            var referenceColor1 = new Color(1.0f, 2.0f, 3.0f);
            var referenceColor2 = new Color(2.0f, 4.0f, 6.0f);
            var defaultColor = new Color(0.0f, 0.0f, 0.0f);

            testImage.SetPixel(2, 3, referenceColor1);
            testImage.SetPixel(3, 4, referenceColor2);

            // Check correct colors
            Assert.True((testImage.GetPixel(2, 3)).IsClose(referenceColor1));
            Assert.True((testImage.GetPixel(3, 4)).IsClose(referenceColor2));
            Assert.True((testImage.GetPixel(1, 2)).IsClose(defaultColor));

            // Check wrong color
            Assert.False((testImage.GetPixel(3, 4)).IsClose(referenceColor1));
        }

        [Fact]
        public void TestParseImgSize()
        {
            // Correct format
            var (width, height) = HdrImage.ParseImgSize("3 2");
            Assert.True(width == 3 && height == 2);
            
            // Incorrect formats
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseImgSize("-1 2"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseImgSize("1 -2"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseImgSize("-1 -2"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseImgSize("2"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseImgSize(""));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseImgSize("-1"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseImgSize("1 2 3"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseImgSize("abc"));
        }

        [Fact]
        public void TestParseEndianness()
        {
            var endianness = HdrImage.ParseEndianness("1.0");
            Assert.True(endianness == 1.0);
            
            endianness = HdrImage.ParseEndianness("-1.0");
            Assert.True(endianness == -1.0);
            
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseEndianness("0"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.ParseEndianness("abc"));
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
            
            var testImage = new HdrImage(3, 2);
            
            testImage.SetPixel(0,0, new Color(10.0f, 20.0f, 30.0f));
            testImage.SetPixel(1,0, new Color(40.0f, 50.0f, 60.0f));
            testImage.SetPixel(2,0, new Color(70.0f, 80.0f, 90.0f));
            testImage.SetPixel(0,1, new Color(100.0f, 200.0f, 300.0f));
            testImage.SetPixel(1,1, new Color(400.0f, 500.0f, 600.0f));
            testImage.SetPixel(2,1, new Color(700.0f, 800.0f, 900.0f));

            // Long way (useful for debugging)
            using (var memoryStream = new MemoryStream(referenceBytesLe.Length))
            {
                // Write the content of testImage to the memStream buffer as a PFM image
                testImage.WritePfm(memoryStream, HdrImage.LittleEndian);
                
                // Set the position to the beginning of the stream.
                memoryStream.Seek(0, SeekOrigin.Begin);
                
                // Scan the buffer to find inconsistencies with the reference
                foreach (var refByte in referenceBytesLe)
                {
                    var readByte = (byte)memoryStream.ReadByte();
                    //_testOutputHelper.WriteLine($"{b} =? {referenceBytes[i]}");
                    Assert.True(readByte == refByte);
                }
            }
        }
        
        [Fact]
        public void TestWritePfmBigEndian()
        {
            byte[] referenceBytesBe =
            {
                0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2b, 0x31, 0x2e, 0x30, 0x0a,
                0x42, 0xc8, 0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00,
                0x43, 0xc8, 0x00, 0x00, 0x43, 0xfa, 0x00, 0x00, 0x44, 0x16, 0x00, 0x00,
                0x44, 0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61, 0x00, 0x00,
                0x41, 0x20, 0x00, 0x00, 0x41, 0xa0, 0x00, 0x00, 0x41, 0xf0, 0x00, 0x00, 
                0x42, 0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70, 0x00, 0x00, 
                0x42, 0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00
            };

            var testImage = new HdrImage(3, 2);
            
            testImage.SetPixel(0,0, new Color(10.0f, 20.0f, 30.0f));
            testImage.SetPixel(1,0, new Color(40.0f, 50.0f, 60.0f));
            testImage.SetPixel(2,0, new Color(70.0f, 80.0f, 90.0f));
            testImage.SetPixel(0,1, new Color(100.0f, 200.0f, 300.0f));
            testImage.SetPixel(1,1, new Color(400.0f, 500.0f, 600.0f));
            testImage.SetPixel(2,1, new Color(700.0f, 800.0f, 900.0f));

            using var memoryStream = new MemoryStream(referenceBytesBe.Length);
            testImage.WritePfm(memoryStream, HdrImage.BigEndian);
            var myBytesBe = memoryStream.ToArray();
                
            Assert.True(myBytesBe.SequenceEqual(referenceBytesBe));
        }


        [Fact]
        public void TestReadPfmFile()
        {
            var image = new HdrImage("../../../reference_be.pfm");

            Assert.True(image.Width == 3);
            Assert.True(image.Height == 2);

            /*
            _testOutputHelper.WriteLine($"(0, 0) = {image.GetPixel(0, 0).ToString()}");
            _testOutputHelper.WriteLine($"(1, 0) = {image.GetPixel(1, 0).ToString()}");
            _testOutputHelper.WriteLine($"(2, 0) = {image.GetPixel(2, 0).ToString()}");
            _testOutputHelper.WriteLine($"(0, 1) = {image.GetPixel(0, 1).ToString()}");
            _testOutputHelper.WriteLine($"(1, 2) = {image.GetPixel(1, 1).ToString()}");
            _testOutputHelper.WriteLine($"(2, 1) = {image.GetPixel(2, 1).ToString()}");
            */
            Assert.True(image.GetPixel(0, 0).IsClose(new Color(10.0f, 20.0f, 30.0f)));
            Assert.True(image.GetPixel(1, 0).IsClose(new Color(40.0f, 50.0f, 60.0f)));
            Assert.True(image.GetPixel(2, 0).IsClose(new Color(70.0f, 80.0f, 90.0f)));
            Assert.True(image.GetPixel(0, 1).IsClose(new Color(100.0f, 200.0f, 300.0f)));
            Assert.True(image.GetPixel(1, 1).IsClose(new Color(400.0f, 500.0f, 600.0f)));
            Assert.True(image.GetPixel(2, 1).IsClose(new Color(700.0f, 800.0f, 900.0f)));
            
            image = new HdrImage("../../../reference_le.pfm");

            Assert.True(image.Width == 3);
            Assert.True(image.Height == 2);

            Assert.True(image.GetPixel(0, 0).IsClose(new Color(10.0f, 20.0f, 30.0f)));
            Assert.True(image.GetPixel(1, 0).IsClose(new Color(40.0f, 50.0f, 60.0f)));
            Assert.True(image.GetPixel(2, 0).IsClose(new Color(70.0f, 80.0f, 90.0f)));
            Assert.True(image.GetPixel(0, 1).IsClose(new Color(100.0f, 200.0f, 300.0f)));
            Assert.True(image.GetPixel(1, 1).IsClose(new Color(400.0f, 500.0f, 600.0f)));
            Assert.True(image.GetPixel(2, 1).IsClose(new Color(700.0f, 800.0f, 900.0f)));
        }

        [Fact]
        public void TestAverageLuminosity()
        {
            var testImage = new HdrImage(2, 1);
            testImage.SetPixel(0, 0, new Color(5.0f, 10.0f, 15.0f)); // Luminosity = 10
            testImage.SetPixel(1, 0, new Color(500.0f, 1000.0f, 1500.0f)); // Luminosity = 1000
            
            // _testOutputHelper.WriteLine($"Average luminosity = {testImage.AverageLuminosity()}");
            Assert.True(100.0f == testImage.AverageLuminosity());
        }

        [Fact]
        public void TestNormalizeImage()
        {
            var testImage = new HdrImage(2, 1);
            testImage.SetPixel(0, 0, new Color(5.0f, 10.0f, 15.0f)); // Luminosity = 10
            testImage.SetPixel(1, 0, new Color(500.0f, 1000.0f, 1500.0f)); // Luminosity = 1000
            
            testImage.NormalizeImage(1000.0f, 100.0f); // result *= a / L (= 10.0)
            Assert.True(testImage.GetPixel(0,0).IsClose(new Color(50.0f, 100.0f, 150.0f)));
            Assert.True(testImage.GetPixel(1,0).IsClose(new Color(5e3f, 1e4f, 1.5e4f)));
        }

        [Fact]
        public void TestClampImage()
        {
            var testImage = new HdrImage(2, 1);
            testImage.SetPixel(0, 0, new Color(5.0f, 10.0f, 15.0f)); 
            testImage.SetPixel(1, 0, new Color(500.0f, 1000.0f, 1500.0f));
            
            testImage.ClampImage();
            
            // test the result is in [0,1]

            foreach (var pixel in testImage.Pixels)
            {
                Assert.True(pixel.R is >= 0.0f and <= 1.0f);
                Assert.True(pixel.G is >= 0.0f and <= 1.0f);
                Assert.True(pixel.B is >= 0.0f and <= 1.0f);
            }
        }

        [Fact]
        public void TestReadFloat()
        {
            // Test number
            var testNumber = 3.0f;

            // Creating a memory stream capable to store of 4 bytes
            using var memoryStream = new MemoryStream(4);
            
            HdrImage.WriteFloat(memoryStream, testNumber, HdrImage.LittleEndian);
            
            memoryStream.Seek(0, SeekOrigin.Begin); // reset stream
            Assert.True(testNumber == HdrImage.ReadFloat(memoryStream));

            memoryStream.Seek(0, SeekOrigin.Begin); // reset stream
            Assert.False(5.0f == HdrImage.ReadFloat(memoryStream)); // wrong number

            memoryStream.Seek(0, SeekOrigin.Begin); // reset stream
            Assert.False(testNumber == HdrImage.ReadFloat(memoryStream, HdrImage.BigEndian)); // wrong endianness
        }

        [Fact]
        public void TestReadPfmLine()
        {
            // Test string and conversion in byte of test string
            const string testString = "Hello\nworld\n";
            var testBytes = Encoding.ASCII.GetBytes(testString, 0, testString.Length);

            // Creating a memory stream for allocation of 
            using var memoryStream = new MemoryStream(testString.Length);
            // Little endianness is assumed
            memoryStream.Write(testBytes);

            // Reset to the beginning of the stream to reuse it
            memoryStream.Seek(0, SeekOrigin.Begin);
            Assert.True("Hello" == HdrImage.ReadPfmLine(memoryStream));
            Assert.True("world" == HdrImage.ReadPfmLine(memoryStream));
        }
    }
}