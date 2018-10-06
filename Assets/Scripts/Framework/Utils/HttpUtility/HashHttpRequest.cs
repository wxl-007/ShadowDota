using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

public class HashHttpRequest {
	private const int ________________ = 16;
	//start from 1
	private const int _________________ = 2;
	private const int __________________ = 5;
	private const int ___________________ = 10;
	private const int ____________________ = 11;
	//start from 1
	private const int _____________________ = 3;
	private const int ______________________ = 7;
	private const int _______________________ = 9;
	private const int ________________________ = 14;

	/// <summary>
	/// _________ string lookup table.
	/// </summary>
	private static readonly string[] _______________ = new string[]
	{
		"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
		"10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
		"20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
		"30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
		"40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
		"50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
		"60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
		"70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
		"80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
		"90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
		"A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
		"B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
		"C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
		"D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
		"E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
		"F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
	};


	public static string ______________ = "0123456789ABCDEF";

	public static string _________________________(string a, string b, string c, string d, string e) {

		string _________ = null;
		char[] _ = MessageDigest_Algorithm.getMd5Hash(a + b + c + d + e).ToCharArray();
		UInt16[] __ = new UInt16[8];

		for(int ____________ = 0; ____________ < 8; ++____________) {
			StringBuilder ___ = new StringBuilder();
			___.Append(_[____________ * 4]).Append(_[____________ * 4 + 1]).Append(_[____________ * 4 + 2]).Append(_[____________ * 4 + 3]);
			UInt16 ___________ = UInt16.Parse(___.ToString(), System.Globalization.NumberStyles.HexNumber);
			__[____________] = ___________;
		}

		char[] ____ = new char[________________];
		for(int ____________ = 0; ____________ < ________________; ++ ____________) {
			int _____________ = NetRandom.Instance.Next(16);

			____[____________] = ______________[_____________];
		}

		string _____ = Convert.ToString(____[_________________ - 1]) + Convert.ToString(____[__________________ - 1]) + Convert.ToString(____[___________________ - 1]) 
		               + Convert.ToString(____[____________________ - 1]); 

		UInt16 ______ = UInt16.Parse(_____, System.Globalization.NumberStyles.HexNumber);

		UInt16 _______ = 0;
		foreach(UInt16 __________ in __) {	
			unchecked{
				_______ = (UInt16) (_______ + ( __________ ^ ______));
			};
		}


		UInt16Converter ________ = _______;

		____[_____________________ - 1] = _______________[________.__________________________].ToCharArray()[0];
		____[______________________ - 1] = _______________[________.__________________________].ToCharArray()[1];
		____[_______________________ - 1] = _______________[________._________________________].ToCharArray()[0];
		____[________________________ - 1] = _______________[________._________________________].ToCharArray()[1];

		_________ = new string(____);
		return _________;
	}


}


[StructLayout(LayoutKind.Explicit)]
struct UInt16Converter
{
	[FieldOffset(0)] public UInt16 ___________________________;
	[FieldOffset(0)] public byte _________________________;
	[FieldOffset(1)] public byte __________________________;

	public UInt16Converter(UInt16 value)
	{
		_________________________ = __________________________ = 0;
		___________________________ = value;
	}

	public static implicit operator UInt16(UInt16Converter value)
	{
		return value.___________________________;
	}

	public static implicit operator UInt16Converter(UInt16 value)
	{
		return new UInt16Converter(value);
	}	
}


