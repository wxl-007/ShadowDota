using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct FloatIntUnion
{
	[FieldOffset(0)]
	public float f;

	[FieldOffset(0)]
	public int i;

	public int FloatToInt(float value)
	{
		var u = new FloatIntUnion();
		u.f   = value;
		return u.i;
	}

	public float IntToFloat(int value)
	{
		var u = new FloatIntUnion();
		u.i   = value;
		return u.f;
	}
}


public struct FloatFog {
	private const int Crypto = 0x0F0F0FF0;
	private int encryValue;
	private static FloatIntUnion convert = new FloatIntUnion();

	public FloatFog(float value) {
		encryValue = convert.FloatToInt(value) ^ Crypto;
	}

	public static implicit operator float(FloatFog value)
	{
		return convert.IntToFloat(value.encryValue ^ Crypto);
	}

	public static implicit operator FloatFog(float value)
	{
		return new FloatFog(value);
	}	

	public override string ToString() {
		return convert.IntToFloat(encryValue ^ Crypto).ToString();
	}
}


