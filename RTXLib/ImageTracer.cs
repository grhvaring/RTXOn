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

    public void FireAllRays(Func<Ray, Color> function)
    {
        using var progress = new ProgressBar();
        for (int row = 0; row < Image.Height; ++row)
        {
            SaveSnapShotImage();
            progress.Report((double) row / Image.Height);
            for (int col = 0; col < Image.Width; ++col)
            {
                var ray = FireRay(col, row);
                var color = function(ray);
                Image.SetPixel(col, row, color);
            }
        }
    }

    private void SaveSnapShotImage()
    {
        var tempImage = Image.ShallowCopy();
        tempImage.WritePfm("temp.pfm");
        tempImage.NormalizeImage(1);
        tempImage.ClampImage();
        tempImage.SaveAsPng("temp.png", 1);
    }
}