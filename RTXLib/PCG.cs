namespace RTXLib;

///<summary> Class <c>PCG</c> models a generator of random floating point numbers in interval [0, 1] using PCG algorithm.</summary>
public class PCG
{
	public ulong State;
	public ulong Inc;

	// Default constructor
	public PCG(ulong intialState = 42, ulong initialSeq = 54)
	{		
		State = 0;
		Inc = (initialSeq << 1) | 1;
		Random();
		State += intialState;
		Random();
	}

	// The Random methods generates a random 32bit unsigned integer in interval [0, 2^32 - 1]
	public uint Random()
    {
		ulong oldState = State;
		State = (oldState * 6364136223846793005 + Inc);
		uint xorShifted = (uint) (((oldState >> 18) ^ oldState) >> 27);
		uint rot = (uint)(oldState >> 59);
		return (xorShifted >> (int) rot) | (xorShifted << (int)(-rot & 31));
		// NOTE: the (int) cast of right side of operators >> and << is not problematic since the maximum size of rot is 64 - 59 = 5.
    }

	// The RandomFloat methods generates a random float number in interval [0, 1]
	public float RandomFloat()
    {
		return Random() / 0xffffffff;
    }
}