using System.Collections;
using System;

public class PseudoRandom
{
    private const int THOUSAND = 1000;

	private Random _random;
	
	//Keep Singleton mode
	private static PseudoRandom _pseudoRandom;
	private PseudoRandom ()
	{
		//The resolution of the System.Environment.TickCount property cannot be less than 500 milliseconds.
		//
		_random = new Random (Environment.TickCount);
	}
	
	public static PseudoRandom getInstance ()
	{
		if (_pseudoRandom == null) {
			_pseudoRandom = new PseudoRandom ();
		}
		return _pseudoRandom;
	}
	
	/// <summary>
	/// store proballity , we suppose that precision can not less than one thousandth 
	//  so,  0.5% = _proballity = 5, 65.5% = _proballity = 655
	/// </summary>
	public bool happen (int limitation)
	{
		bool lucky = true;

        int pseudoValue = _random.Next(THOUSAND);
		if (pseudoValue <= limitation) {
			lucky = true;
		} else 
			lucky = false;
		
		return lucky;
	}

	/// <summary>
	/// 判定是否是满足
	/// </summary>
	/// <param name="withInOne">小于1大于0的浮点数</param>
	public bool happen (float withInOne) {
		if(withInOne <= 0F) 
			return false;
		if(withInOne >= 1F)
			return true;
		int limit = (int) (withInOne * THOUSAND);
		return happen(limit);
	}

	public int next(int limitation){
		return _random.Next(limitation);
	}

	public byte[] generateRandom (int bytecount)
	{
		byte[] buffer = new byte[bytecount];
		_random.NextBytes (buffer);
		return buffer;
	}
}
