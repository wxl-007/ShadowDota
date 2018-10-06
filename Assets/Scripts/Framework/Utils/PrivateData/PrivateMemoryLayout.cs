using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct Int32Fog
{
	//something like union
	[FieldOffset(0)] public Int32 Value;

	[FieldOffset(0)] public byte Byte1;
	[FieldOffset(1)] public byte Byte2;
	[FieldOffset(2)] public byte Byte3;
	[FieldOffset(3)] public byte Byte4;
	[FieldOffset(4)] public byte Byte5;
	[FieldOffset(5)] public byte Byte6;


	public Int32Fog(Int32 value)
	{
		Byte1 = Byte2 = 0;
		Byte3 = Byte4 = 0;
		Byte5 = Byte6 = 0;

		Value = value;

		//move Byte1 to Byte5 & move Byte2 to Byte6
		Byte5 = Byte1;
		Byte6 = Byte2;

		//clear the memeory
		Byte1 = 0;
		Byte2 = 0;
	}

	public Int32 toInt32 () {
		Int32 result = 0;

		//restore value
		Byte1 = Byte5;
		Byte2 = Byte6;

		result = Value;

		//clear the memeory
		Byte1 = 0;
		Byte2 = 0;

		return result;
	}
	
	public override string ToString() {
		return toInt32().ToString();
	}
	
	public static implicit operator Int32(Int32Fog value)
	{
		return value.toInt32();
	}

	public static implicit operator Int32Fog(Int32 value)
	{
		return new Int32Fog(value);
	}	
}


[StructLayout(LayoutKind.Explicit)]
public struct Int16Fog {
	//something like union
	[FieldOffset(0)] public Int16 Value;

	[FieldOffset(0)] public byte Byte1;
	[FieldOffset(1)] public byte Byte2;
	[FieldOffset(2)] public byte Byte3;

	public Int16Fog(Int16 value)
	{
		Byte1 = Byte2 = Byte3 = 0;

		Value = value;
		//move Byte1 to Byte3
		Byte3 = Byte1;

		//clear the memeory
		Byte1 = 0;
	}

	public Int16 toInt16 () {
		Int16 result = 0;

		//restore value
		Byte1 = Byte3;

		result = Value;
		//clear the memeory
		Byte1 = 0;

		return result;
	}

	public override string ToString() {
		return toInt16().ToString();
	}

//	public static implicit operator Int16(Int16Fog value)
//	{
//		return value.toInt16();
//	}

    public static explicit operator Int16(Int16Fog value)
    {
        return value.toInt16();
    }

//	public static implicit operator Int16Fog(Int16 value)
//	{
//		return new Int16Fog(value);
//	}	

    public static explicit operator Int16Fog(Int16 value)
    {
        return new Int16Fog(value);
    }
}