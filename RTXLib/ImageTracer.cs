namespace RTXLib;

public class ImageTracer
{
    public HdrImage Image;
    public ICamera Camera;
    
    public ImageTracer(HdrImage image, ICamera camera)
    {
        Image = image;
        Camera = camera;
    }

    public Ray FireRay(int col, int row, float uPixel = 0.5f, float vPixel = 0.5f)
    {
        var u = (col + uPixel) / Image.Width;
        var v = 1 - (row + vPixel) / Image.Height;
        return Camera.FireRay(u, v);
    }

    public void FireAllRays(Func<Ray, Color> function, int numSubDivisions = 0, PCG? pcg = null)
    {
        using var progress = new ProgressBar();
        pcg ??= new PCG();
        for (int row = 0; row < Image.Height; ++row)
        {
            progress.Report((double) row / Image.Height);
            for (int col = 0; col < Image.Width; col += 2)
            {
                var color = CalculateColor(col, row, numSubDivisions, pcg, function);
                Image.SetPixel(col, row, color);

                if (col <= 1) continue;
                var prevColor = Image.GetPixel(col - 2, row);
                var inBetweenColor = (prevColor + color) / 2;
                if (((prevColor - color) / 255).IsBiggerThan(0.1))
                    inBetweenColor = CalculateColor(col - 1, row, numSubDivisions, pcg, function);
                Image.SetPixel(col - 1, row, inBetweenColor);
            }
        }
    }

    private Color CalculateColor(int col, int row, int numSubDivisions, PCG pcg, Func<Ray, Color> function)
    {
        var color = new Color();
        if (numSubDivisions == 0)
        {
            var ray = FireRay(col, row);
            color = function(ray);
        }
        else
        {
            var numSubRays = (int) Math.Pow(numSubDivisions + 1, 2);
            for (var i = 0; i < numSubRays; ++i)
            {
                // (uPixel, vPixel) specifies a uniformly distributed random point inside the sub square
                var (u, v) = (pcg.RandomFloat(), pcg.RandomFloat());
                var ray = FireRay(col, row, u, v);
                color += function(ray);
            }
            color /= numSubRays;
        }

        return color;
    }

    private void SaveSnapShotImage()
    {
        // to improve
        Image.WritePfm("temp.pfm");
        /*var tempImage = Image.ShallowCopy();
        tempImage.NormalizeImage(1);
        tempImage.ClampImage();
        tempImage.SaveAsPng("temp.png", 1);*/
    }
}