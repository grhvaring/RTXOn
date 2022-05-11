namespace RTXLib;

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
		return (uint)((xorShifted >> (int) rot) | (xorShifted << (int)((-rot) & 31)));
    }
}
