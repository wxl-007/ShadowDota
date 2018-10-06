using System;
using System.Text;
using System.IO;

public class UniqueGUID
{
	private const string UNIQUE_NAME = "id.bin";
	private const int UNIQUELENGTH = 32;
	private string UNIQUE_PATH = null;
	private string _unique = null;
	
	private UniqueGUID(){
		UNIQUE_PATH = DeviceInfo.PersisitFullPath (UNIQUE_NAME);
	}
	private static UniqueGUID _guid;
	public static UniqueGUID getInstance(){
		if(_guid == null)
			_guid = new UniqueGUID();
		return _guid;
	}
	
	public string getUniqueIdetify(){
		if(_unique != null)
			return _unique;
		else {
			if(isExist())
				_unique = ReadUniqueIdentify();
			else
				_unique = GenerateUniqueIdentify();
			return _unique;
		}
	}

	#if UNITY_EDITOR
	public void Unregister() {
		_unique = null;
	}
	#endif

	private void saveToFile(string unique){
		byte[] info = new UTF8Encoding(true).GetBytes(unique);
		// Create the file and write to it.
		// DANGER: System.IO.File.Create will overwrite the file if it already exists. 
		using (FileStream fs = File.Create(UNIQUE_PATH)) {
			fs.Write (info,0,info.Length);
		}
	}
	
	private bool isExist ()
	{
		return File.Exists (UNIQUE_PATH); 
	}
	
	private string ReadUniqueIdentify(){
		MemoryStream ByteBuilder = new MemoryStream();
		byte[] content = new byte[UNIQUELENGTH];
		using (FileStream fs = File.Open(UNIQUE_PATH, FileMode.Open )) {
			int readed = 0;
			while( (readed = fs.Read (content, 0, UNIQUELENGTH)) > 0 ) {
				ByteBuilder.Append(content, readed);
			}
		}
		UTF8Encoding utf8 = new UTF8Encoding(true);
		string Unique = utf8.GetString(ByteBuilder.ToArray());
		//md5 hash
		Unique = MessageDigest_Algorithm.getMd5Hash(Unique);
		return Unique;
	}
	
	// generate 32bit string
	private string GenerateUniqueIdentify(){
		Guid guid = Guid.NewGuid();
		string unique = guid.ToString();
		saveToFile(unique);
		//md5 hash
		unique = MessageDigest_Algorithm.getMd5Hash(unique);
		return unique;
	}
}

