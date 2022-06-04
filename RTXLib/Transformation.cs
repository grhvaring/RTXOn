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
    
    public static Transformation Identity => new();

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
        var T = new Matrix4x4
        (
            1, 0, 0, x,
            0, 1, 0, y,
            0, 0, 1, z,
            0, 0, 0, 1
        );
        var invT = new Matrix4x4
        (
            1, 0, 0, -x,
            0, 1, 0, -y,
            0, 0, 1, -z,
            0, 0, 0, 1
        );
        return new Transformation(T, invT);
    }

    public static Transformation Translation(Vec v)
    {
        return Translation(v.X, v.Y, v.Z);
    }

    private static float ToRadians(float angleDeg)
    {
        return (float) (angleDeg * Math.PI / 180);
    }

    public static Transformation RotationX(float angleDeg = DefaultAngle)
    {
        // The implementation in Matrix4x4 uses a notation with the angle flipped
        // The angle is corrected to be consistent with the Right-Hand Rule
        // and matrix - vector multiplication
      
        var angleRad = ToRadians(angleDeg);
        var s = (float)Math.Sin(angleRad);
        var c = (float)Math.Cos(angleRad);
        var R = new Matrix4x4
        (
            1, 0, 0, 0,
            0, c, -s, 0,
            0, s, c, 0,
            0, 0, 0, 1
        );
        var invR = new Matrix4x4
        (
            1, 0, 0, 0,
            0, c, s, 0,
            0, -s, c, 0,
            0, 0, 0, 1
        );
        return new Transformation(R, invR);
    }
    
    public static Transformation RotationY(float angleDeg = DefaultAngle)
    {
        float angleRad = ToRadians(angleDeg);
        var s = (float)Math.Sin(angleRad);
        var c = (float)Math.Cos(angleRad);
        var R = new Matrix4x4
        (
            c, 0, s, 0,
            0, 1, 0, 0,
            -s, 0, c, 0,
            0, 0, 0, 1
        );
        var invR = new Matrix4x4
        (
            c, 0, -s, 0,
            0, 1, 0, 0,
            s, 0, c, 0,
            0, 0, 0, 1
        );
        return new Transformation(R, invR);
    }
    
    public static Transformation RotationZ(float angleDeg = DefaultAngle)
    {
        var angleRad = ToRadians(angleDeg);
        var s = (float)Math.Sin(angleRad);
        var c = (float)Math.Cos(angleRad);
        var R = new Matrix4x4
        (
            c, -s, 0, 0,
            s, c, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
        var invR = new Matrix4x4
        (
            c, s, 0, 0,
            -s, c, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
      
        return new Transformation(R, invR);
    }

    public static Transformation Scaling(float xFactor, float yFactor, float zFactor)
    {
        var S = new Matrix4x4
        (
            xFactor, 0, 0, 0,
            0, yFactor, 0, 0,
            0, 0, zFactor, 0,
            0, 0, 0,       1
        );
        var invS = new Matrix4x4
        (
            1/xFactor, 0, 0, 0,
            0, 1/yFactor, 0, 0,
            0, 0, 1/zFactor, 0,
            0, 0, 0,         1
        );
        return new Transformation(S, invS);
    }
    
    public static Transformation Scaling(float factor = 1)
    {
        return Scaling(factor, factor, factor);
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
        if (t.M.IsIdentity) return p;
        
        var newX = t.M.M11 * p.X + t.M.M12 * p.Y + t.M.M13 * p.Z + t.M.M14;
        var newY = t.M.M21 * p.X + t.M.M22 * p.Y + t.M.M23 * p.Z + t.M.M24;
        var newZ = t.M.M31 * p.X + t.M.M32 * p.Y + t.M.M33 * p.Z + t.M.M34;
        var w = t.M.M41 * p.X + t.M.M42 * p.Y + t.M.M43 * p.Z + t.M.M44;

        var newPoint = new Point(newX, newY, newZ);
        
        if (w != 0 && Math.Abs(w - 1) > 1e-5) newPoint /= w;
        
        return newPoint;
    }
     
    public static Vec operator *(Transformation t, Vec v)
    {
        if (t.M.IsIdentity) return v;

        var newX = t.M.M11 * v.X + t.M.M12 * v.Y + t.M.M13 * v.Z;
        var newY = t.M.M21 * v.X + t.M.M22 * v.Y + t.M.M23 * v.Z;
        var newZ = t.M.M31 * v.X + t.M.M32 * v.Y + t.M.M33 * v.Z;
        return new Vec(newX, newY, newZ);
    }
    
    public static Normal operator *(Transformation t, Normal n)
    {
        if (t.InvM.IsIdentity) return n;

        var newX = t.InvM.M11 * n.X + t.InvM.M21 * n.Y + t.InvM.M31 * n.Z;
        var newY = t.InvM.M12 * n.X + t.InvM.M22 * n.Y + t.InvM.M32 * n.Z;
        var newZ = t.InvM.M13 * n.X + t.InvM.M23 * n.Y + t.InvM.M33 * n.Z;
        return new Normal(newX, newY, newZ);
    }
    
    public bool IsConsistent(double e = 1e-5)
    {
        var supposedIdentity = this * Inverse();
        return supposedIdentity.IsClose(Identity, e);
    }

    public bool IsClose(Transformation otherT, double e = 1e-5)
    {
        return AreMatricesClose(M, otherT.M, e) && AreMatricesClose(InvM, otherT.InvM, e);
    }

    public static bool AreMatricesClose(Matrix4x4 M1, Matrix4x4 M2, double e = 1e-5)
    {
        Matrix4x4 diff = M1 - M2;
        return MyLib.IsZero(diff.M11, e) && MyLib.IsZero(diff.M12, e) && MyLib.IsZero(diff.M13, e) && MyLib.IsZero(diff.M14, e) && 
               MyLib.IsZero(diff.M21, e) && MyLib.IsZero(diff.M22, e) && MyLib.IsZero(diff.M23, e) && MyLib.IsZero(diff.M24, e) && 
               MyLib.IsZero(diff.M31, e) && MyLib.IsZero(diff.M32, e) && MyLib.IsZero(diff.M33, e) && MyLib.IsZero(diff.M34, e) && 
               MyLib.IsZero(diff.M41, e) && MyLib.IsZero(diff.M42, e) && MyLib.IsZero(diff.M43, e) && MyLib.IsZero(diff.M44, e);
    }
}