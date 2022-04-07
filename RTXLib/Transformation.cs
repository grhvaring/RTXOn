using System.Numerics;

namespace RTXLib;

public struct Transformation
{
    public Matrix4x4 M;
    public Matrix4x4 InvM;

    private static readonly Matrix4x4 I = Matrix4x4.Identity;
    private const float DefaultAngle = 0;

    public Transformation()
    {
        M = I;
        InvM = I;
    }
    
    public Transformation(Matrix4x4 m, Matrix4x4 invM)
    {
        M = m;
        InvM = invM;
    }

    public Transformation
    (
        float m11, float m12, float m13, float m14,
        float m21, float m22, float m23, float m24,
        float m31, float m32, float m33, float m34, 
        float m41, float m42, float m43, float m44,
        float mInv11, float mInv12, float mInv13, float mInv14, 
        float mInv21, float mInv22, float mInv23, float mInv24,
        float mInv31, float mInv32, float mInv33, float mInv34, 
        float mInv41, float mInv42, float mInv43, float mInv44
    )
    {
        M = new Matrix4x4
            (
                m11, m12, m13, m14, 
                m21, m22, m23, m24, 
                m31, m32, m33, m34, 
                m41, m42, m43, m44
            );
        InvM = new Matrix4x4
            (
                mInv11, mInv12, mInv13, mInv14,
                mInv21, mInv22, mInv23, mInv24, 
                mInv31, mInv32, mInv33, mInv34,
                mInv41, mInv42, mInv43, mInv44
            );
    }

    public Transformation(float[][] m, float[][] mInv)
    {
        M = new Matrix4x4(m[0][0], m[0][1], m[0][2], m[0][3],
                          m[1][0], m[1][1], m[1][2], m[1][3], 
                          m[2][0], m[2][1], m[2][2], m[2][3], 
                          m[3][0], m[3][1], m[3][2], m[3][3]);
        InvM = new Matrix4x4(mInv[0][0], mInv[0][1], mInv[0][2], mInv[0][3], 
                             mInv[1][0], mInv[1][1], mInv[1][2], mInv[1][3], 
                             mInv[2][0], mInv[2][1], mInv[2][2], mInv[2][3], 
                             mInv[3][0], mInv[3][1], mInv[3][2], mInv[3][3]);
    }

    public Transformation(float[,] m, float[,] mInv)
    {
        M = new Matrix4x4(m[0,0], m[0,1], m[0,2], m[0,3],
                          m[1,0], m[1,1], m[1,2], m[1,3], 
                          m[2,0], m[2,1], m[2,2], m[2,3], 
                          m[3,0], m[3,1], m[3,2], m[3,3]);
        InvM = new Matrix4x4(mInv[0,0], mInv[0,1], mInv[0,2], mInv[0,3], 
                             mInv[1,0], mInv[1,1], mInv[1,2], mInv[1,3], 
                             mInv[2,0], mInv[2,1], mInv[2,2], mInv[2,3], 
                             mInv[3,0], mInv[3,1], mInv[3,2], mInv[3,3]);
    }

    public static Transformation Translation(float x=0, float y=0, float z=0)
    {
        var T = Matrix4x4.CreateTranslation(x ,y, z);
        var invT = Matrix4x4.CreateTranslation(-x, -y, -z);
        return new Transformation(T, invT);
    }

    public static Transformation Translation(Vec v)
    {
        return Translation(v.X, v.Y, v.Z);
    }

    public static Transformation RotationX(float thetaRad = DefaultAngle)
    {
        var R = Matrix4x4.CreateRotationX(thetaRad);
        var invR = Matrix4x4.CreateRotationX(-thetaRad);
        return new Transformation(R, invR);
    }
    
    public static Transformation RotationY(float thetaRad = DefaultAngle)
    {
        var R = Matrix4x4.CreateRotationY(thetaRad);
        var invR = Matrix4x4.CreateRotationY(-thetaRad);
        return new Transformation(R, invR);
    }
    
    public static Transformation RotationZ(float thetaRad = DefaultAngle)
    {
        var R = Matrix4x4.CreateRotationZ(thetaRad);
        var invR = Matrix4x4.CreateRotationZ(-thetaRad);
        return new Transformation(R, invR);

    }

    public static Transformation Scaling(float factor = 1)
    {
        var S = Matrix4x4.CreateScale(factor);
        var invS = Matrix4x4.CreateScale(1 / factor);
        return new Transformation(S, invS);
    }

    public Transformation Inverse()
    {
        return new Transformation(InvM, M);
    }

    public Transformation Transpose()
    {
        return new Transformation(Matrix4x4.Transpose(M), Matrix4x4.Transpose(InvM));
    }

    public static Transformation operator *(Transformation A, Transformation B)
    {
        return new Transformation(A.M * B.M, B.InvM * A.InvM);
    }
    
    public static Point operator *(Transformation t, Point p)
    {
        float newX = t.M.M11 * p.X + t.M.M12 * p.Y + t.M.M13 * p.Z + t.M.M14;
        float newY = t.M.M21 * p.X + t.M.M22 * p.Y + t.M.M23 * p.Z + t.M.M24;
        float newZ = t.M.M31 * p.X + t.M.M32 * p.Y + t.M.M33 * p.Z + t.M.M34;
        float w = t.M.M41 * p.X + t.M.M42 * p.Y + t.M.M43 * p.Z + t.M.M44;

        Point newPoint = new Point(newX, newY, newZ);
        
        if (w != 0 && Math.Abs(w - 1) > 1e-5) newPoint /= w;
        
        return newPoint;
    }
     
    public static Vec operator *(Transformation t, Vec v)
    {
        float newX = t.M.M11 * v.X + t.M.M12 * v.Y + t.M.M13 * v.Z;
        float newY = t.M.M21 * v.X + t.M.M22 * v.Y + t.M.M23 * v.Z;
        float newZ = t.M.M31 * v.X + t.M.M32 * v.Y + t.M.M33 * v.Z;
        return new Vec(newX, newY, newZ);
    }
    
    public static Normal operator *(Transformation t, Normal n)
    {
        Transformation tInv = t.Inverse();
        float newX = t.M.M11 * n.X + t.M.M21 * n.Y + t.M.M31 * n.Z;
        float newY = t.M.M12 * n.X + t.M.M22 * n.Y + t.M.M32 * n.Z;
        float newZ = t.M.M13 * n.X + t.M.M23 * n.Y + t.M.M33 * n.Z;
        return new Normal(newX, newY, newZ);
    }
    
    public bool IsConsistent(double e = 1e-5)
    {
        Transformation supposedIdentity = new(M * InvM, InvM * M);
        Transformation identity = new(I, I);
        return supposedIdentity.IsClose(identity, e);
    }

    public bool IsClose(Transformation otherT, double e = 1e-5)
    {
        return AreMatricesClose(M, otherT.M, e) && AreMatricesClose(InvM, otherT.InvM, e);
    }

    public static bool AreMatricesClose(Matrix4x4 M1, Matrix4x4 M2, double e = 1e-5)
    {
        Matrix4x4 diff = M1 - M2;
        return Math.Abs(diff.M11) < e && Math.Abs(diff.M12) < e && Math.Abs(diff.M13) < e && Math.Abs(diff.M14) < e && 
               Math.Abs(diff.M21) < e && Math.Abs(diff.M22) < e && Math.Abs(diff.M23) < e && Math.Abs(diff.M24) < e && 
               Math.Abs(diff.M31) < e && Math.Abs(diff.M32) < e && Math.Abs(diff.M33) < e && Math.Abs(diff.M34) < e && 
               Math.Abs(diff.M41) < e && Math.Abs(diff.M42) < e && Math.Abs(diff.M43) < e && Math.Abs(diff.M44) < e;
    }
}