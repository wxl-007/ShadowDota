using System;
using System.Text; 
using System.IO;
using System.Security.Cryptography; 

public class MessageDigest_Algorithm
{
	//私有化构造函数 
	private MessageDigest_Algorithm() 
	{ 
	} 
	// Hash an input string and return the hash as
	// a 32 character hexadecimal string.
	public static string getMd5Hash(string input)
	{
		// Create a new instance of the MD5CryptoServiceProvider object.
		MD5 md5Hasher = MD5.Create();
		
		// Convert the input string to a byte array and compute the hash.
		byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
		
		// Create a new Stringbuilder to collect the bytes
		// and create a string.
		StringBuilder sBuilder = new StringBuilder();
		
		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}
		
		// Return the hexadecimal string.
		return sBuilder.ToString();
	}
	
	// Verify a hash against a string.
	public static bool verifyMd5Hash(string input, string hash)
	{
		// Hash the input.
		string hashOfInput = getMd5Hash(input);
		
		// Create a StringComparer an comare the hashes.
		StringComparer comparer = StringComparer.OrdinalIgnoreCase;
		
		if (0 == comparer.Compare(hashOfInput, hash))
		{
			return true;
		}
		else
		{
			return false;
		}
	}


	public static string getFileMd5Hash(string pathName){
		string strResult = "";
		string strHashData = "";
		byte[] arrbytHashValue;

		FileStream oFileStream = null;
		MD5CryptoServiceProvider oMD5Hasher = new MD5CryptoServiceProvider();
		try {
			oFileStream = new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream); //计算指定Stream 对象的哈希值
			oFileStream.Close();
			//由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
			strHashData = System.BitConverter.ToString(arrbytHashValue);
			//替换-
			strHashData = strHashData.Replace("-", "");
			strResult = strHashData;
		} catch (Exception ex) {
            ConsoleEx.DebugLog(ex.Message);
		}
		return strResult;
	}

}


