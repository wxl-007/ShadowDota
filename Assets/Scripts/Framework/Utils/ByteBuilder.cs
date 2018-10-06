using System;
using System.IO;

public static class MemoryStreamExtensions
{
	public static void Append (this MemoryStream stream, byte value)
	{
		stream.Append (value);
	}

	public static void Append (this MemoryStream stream, byte[] values, int count)
	{
		stream.Write (values, 0, count);
	}

	public static void Append (this MemoryStream stream, byte[] values, int offset, int count)
	{
		stream.Write (values, offset, count);
	}
		
	public static void Clear (this MemoryStream stream)
	{
		stream.SetLength (0);
		stream.Position = 0;
	}
}

