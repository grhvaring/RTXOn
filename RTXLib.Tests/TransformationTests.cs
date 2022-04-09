using System.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace RTXLib.Tests;

public class TransformationTests
{
    // Visualization of outputs during a test
    private readonly ITestOutputHelper _testOutputHelper;

    public TransformationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    private readonly Matrix4x4 m = new
    (
        1.0f, 2.0f, 3.0f, 4.0f, 
        5.0f, 6.0f, 7.0f, 8.0f,
        9.0f, 9.0f, 8.0f, 7.0f,
        6.0f, 5.0f, 4.0f, 1.0f
    );

    private readonly Matrix4x4 mInv = new(
        -3.75f, 2.75f, -1.0f, 0.0f,
        4.375f, -3.875f, 2.0f, -0.5f,
        0.5f, 0.5f, -1.0f, 1.0f,
        -1.375f, 0.875f, 0.0f, -0.5f
    );

    [Fact]
    public void TestIsConsistent()
    {
        Transformation T = new Transformation(m, mInv);
        Assert.True(T.IsConsistent());
    }
    
    [Fact]
    public void TestIsClose()
    {
        Transformation T = new Transformation(m, mInv);
        Matrix4x4 mCopy = new 
        ( 
            m.M11, m.M12, m.M13, m.M14, 
            m.M21, m.M22, m.M23, m.M24,
            m.M31, m.M32, m.M33, m.M34,
            m.M41, m.M42, m.M43, m.M44
        );
        Matrix4x4 mInvCopy = new 
        ( 
            mInv.M11, mInv.M12, mInv.M13, mInv.M14, 
            mInv.M21, mInv.M22, mInv.M23, mInv.M24,
            mInv.M31, mInv.M32, mInv.M33, mInv.M34,
            mInv.M41, mInv.M42, mInv.M43, mInv.M44
        );

        Transformation TCopy = new Transformation(mCopy, mInvCopy);
        
        Assert.True(TCopy.IsConsistent());
        Assert.True(T.IsClose(TCopy));

        Matrix4x4 mWrongCopy = new
            (
                m.M11, m.M12, m.M13, m.M14, 
                m.M21, m.M22 + 1.0f, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44
            );
        
        Transformation TWrongCopy = new Transformation(mWrongCopy, mInvCopy);

        Assert.False(TWrongCopy.IsConsistent());
        Assert.False(T.IsClose(TWrongCopy));
        
        Matrix4x4 mInvWrongCopy = new Matrix4x4(mInv.M11, mInv.M12, mInv.M13, mInv.M14, 
                                                mInv.M21, mInv.M22 + 1.0f, mInv.M23, mInv.M24, 
                                                mInv.M31, mInv.M32, mInv.M33, mInv.M34, 
                                                mInv.M41, mInv.M42, mInv.M43, mInv.M44);

        Transformation TWrongInvCopy = new Transformation(mCopy, mInvWrongCopy);
        
        Assert.False(TWrongInvCopy.IsConsistent());
        Assert.False(T.IsClose(TWrongInvCopy));
    }

    [Fact]
    void TestMultiplication()
    {
        Transformation T1 = new Transformation(m, mInv);
        
        Transformation T2 = new Transformation(
            new Matrix4x4
            (
                3.0f, 5.0f, 2.0f, 4.0f,
                4.0f, 1.0f, 0.0f, 5.0f,
                6.0f, 3.0f, 2.0f, 0.0f,
                1.0f, 4.0f, 2.0f, 1.0f
            ), 
            new Matrix4x4
            (
                0.4f, -0.2f, 0.2f, -0.6f,
                2.9f, -1.7f, 0.2f, -3.1f,
                -5.55f, 3.15f, -0.4f, 6.45f,
                -0.9f, 0.7f, -0.2f, 1.1f
            )
        );
        
        Assert.True(T2.IsConsistent());

        Transformation T1T2 = new Transformation(
            new Matrix4x4
            (
                33.0f, 32.0f, 16.0f, 18.0f, 
                89.0f, 84.0f, 40.0f, 58.0f, 
                118.0f, 106.0f, 48.0f, 88.0f, 
                63.0f, 51.0f, 22.0f, 50.0f
            ),
            new Matrix4x4
            (
                -1.45f, 1.45f, -1.0f, 0.6f, 
                -13.95f, 11.95f, -6.5f, 2.6f, 
                25.525f, -22.025f, 12.25f, -5.2f, 
                4.825f, -4.325f, 2.5f, -1.1f
            )
        );
        //_testOutputHelper.WriteLine((T1 * T2).InvM.ToString());
        Assert.True((T1 * T2).IsConsistent(1e-3)); // the test fails when the error is smaller than 0.001
        Assert.True((T1 * T2).IsClose(T1T2));
    }

    [Fact]
    public void TestVecPointNormalMultiplication()
    {
        Transformation T = new Transformation
        (
            new Matrix4x4
            (
                1.0f, 2.0f, 3.0f, 4.0f,
                5.0f, 6.0f, 7.0f, 8.0f,
                9.0f, 9.0f, 8.0f, 7.0f,
                0.0f, 0.0f, 0.0f, 1.0f
            ),
            new Matrix4x4
            (
                -3.75f, 2.75f, -1.0f, 0.0f,
                5.75f, -4.75f, 2.0f, 1.0f,
                -2.25f, 2.25f, -1.0f, -2.0f,
                0.0f, 0.0f, 0.0f, 1.0f
            )
        );

        Assert.True(T.IsConsistent());
        Vec expectedVec = new(14.0f, 38.0f, 51.0f);
        Assert.True(expectedVec.IsClose(T * new Vec(1.0f, 2.0f, 3.0f)));
        Point expectedPoint = new(18.0f, 46.0f, 58.0f);
        //_testOutputHelper.WriteLine((T * new Point(1.0f, 2.0f, 3.0f)).ToString());
        Assert.True(expectedPoint.IsClose(T * new Point(1.0f, 2.0f, 3.0f)));
        Normal expectedNormal = new(-8.75f, 7.75f, -3.0f);
        Assert.True(expectedNormal.IsClose(T * new Normal(3.0f, 2.0f, 4.0f)));
    }

    [Fact]
    public void TestInverse()
    {
        Transformation T = new(m, mInv);
        Transformation InvT = T.Inverse();
        Assert.True(InvT.IsConsistent());
        Transformation product = T * InvT;
        Assert.True(product.IsConsistent());
        Assert.True(product.IsClose(new Transformation()));
    }

    [Fact]
    public void TestTranslations()
    {
        Transformation tr1 = Transformation.Translation(1, 2, 3);
        Assert.True(tr1.IsConsistent());

        Transformation tr2 = Transformation.Translation(4, 6, 8);
        Assert.True(tr2.IsConsistent());

        Transformation tr12 = tr1 * tr2;
        Assert.True(tr12.IsConsistent());
        Assert.True(tr12.IsClose(Transformation.Translation(5, 8, 11)));
    }

    [Fact]
    public void TestRotations()
    {
        Assert.True(Transformation.RotationX(69).IsConsistent());
        Assert.True(Transformation.RotationX(4).IsConsistent());
        Assert.True(Transformation.RotationX(20).IsConsistent());

        Vec eX = new(1,0,0);
        Vec eY = new(0,1,0);
        Vec eZ = new(0,0,1);
        
        Assert.True((Transformation.RotationX(90) * eY).IsClose(eZ));
        Assert.True((Transformation.RotationY(90) * eZ).IsClose(eX));
        Assert.True((Transformation.RotationZ(90) * eX).IsClose(eY));
    }

    [Fact]
    public void TestScalings()
    {
        Transformation s1 = Transformation.Scaling(2, 5, 10);
        Assert.True(s1.IsConsistent());

        Transformation s2 = Transformation.Scaling(3, 2, 4);
        Assert.True(s2.IsConsistent());

        Transformation s12 = Transformation.Scaling(6, 10, 40);
        Assert.True(s12.IsClose(s1 * s2));
    }
}