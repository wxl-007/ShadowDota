/// <summary>  
/// 解压文件 
/// </summary>  
using System;  
using System.Text;
using System.Collections; 
using System.IO; 
using System.Diagnostics; 
using System.Runtime.Serialization.Formatters.Binary;  
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;  
using ICSharpCode.SharpZipLib.Zip.Compression; 
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip; 

namespace DeCompression
{   
	public class UnZipClass  
	{    
		#if DEBUG
		private Stopwatch stopwatch = new Stopwatch();
		#endif

        public bool UnZip(string[] args)   
		{    
			bool unzipResult = true;
			ZipInputStream s = new ZipInputStream(File.OpenRead(args[0]));      
			ZipEntry theEntry; 
			int size = 2048;       
			byte[] data = new byte[size]; 

			#if DEBUG
			stopwatch.Start();
			#endif

			while ((theEntry = s.GetNextEntry()) != null) {
				string fileName = Path.GetFileName(theEntry.Name);         
				if (fileName != String.Empty) {
					//解压文件路径
					string path = Path.Combine(args[1], theEntry.Name);
					try {
						//获取解压文件目录
						string directoryName = Path.GetDirectoryName(path); 
						//生成解压目录    
						if(!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);       

						FileStream streamWriter = File.Create(path);         

						while (true)    
						{      
							size = s.Read(data, 0, data.Length);    
							if (size > 0)      
							{      
								streamWriter.Write(data, 0, size);     
							}       
							else     
							{      
								break;     
							}     
						} 
						streamWriter.Close();

					} catch(Exception ex) {
						ConsoleEx.DebugLog(ex.ToString(), ConsoleEx.RED);
						unzipResult = false;
					} 
				}				
			}

			#if DEBUG
			ConsoleEx.DebugLog("UnZip costs " + stopwatch.ElapsedMilliseconds + " miliseconds to be done!");
			stopwatch.Reset();
			#endif

			s.Close();
			return unzipResult;
		}  
	}
}
