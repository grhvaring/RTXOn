using System.Numerics;

namespace RTXLib;

public struct Transformation
{
    public Matrix4x4 transformation;

    public Transformation()
    {
        // RENDILA UNA MATRICE IDENTITA'
        transformation = new Matrix4x4();
    }
}