using UnityEngine;
using System.Collections;

//基于SharpZipLib
using System; using System.IO;   
using ICSharpCode.SharpZipLib.Checksums; 
using ICSharpCode.SharpZipLib.Zip; 
using ICSharpCode.SharpZipLib.GZip; 

namespace Compression
{   
	public class ZipClass  
	{     
		public void ZipFile(string FileToZip, string ZipedFile ,int CompressionLevel, int BlockSize)   
		{    
			//如果文件没有找到，则报错   
			if (! System.IO.File.Exists(FileToZip))     
			{     
				throw new System.IO.FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborderd");   
			}        
			System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip,System.IO.FileMode.Open , System.IO.FileAccess.Read);  
			System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);   
			ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);  
			ZipEntry ZipEntry = new ZipEntry("ZippedFile");   
			ZipStream.PutNextEntry(ZipEntry); 
			ZipStream.SetLevel(CompressionLevel);   
			byte[] buffer = new byte[BlockSize];    
			System.Int32 size =StreamToZip.Read(buffer,0,buffer.Length);   
			ZipStream.Write(buffer,0,size); 
            try     
			{    
				while (size < StreamToZip.Length)    
				{      
					int sizeRead =StreamToZip.Read(buffer,0,buffer.Length);      
					ZipStream.Write(buffer,0,sizeRead);      size += sizeRead;    
				}   
			}      
			catch(System.Exception ex)  
			{      
				throw ex; 
			}     
			ZipStream.Finish();  
			ZipStream.Close();  
			StreamToZip.Close();  
		}      
		
		
		//DirectoryToZipPath:   要压缩的文件夹路径
		//ZipedFilePath:            生成的zip文件路径
		public void ZipFileMain(string DirectoryToZipPath, string ZipedFilePath)
		{
			ZipFileMain(new string[]{DirectoryToZipPath,DirectoryToZipPath});
		}
		
		//args[0]:  要压缩的文件夹路径
		//args[1]:  生成的zip文件路径
		public void ZipFileMain(string[] args)  
		{     
			string[] filenames = Directory.GetFiles(args[0],"*",SearchOption.AllDirectories);         
	
			Crc32 crc = new Crc32();    
			ZipOutputStream s = new ZipOutputStream(File.Create(args[1]));       
			s.SetLevel(6); 
			// 0 - store only to 9 - means best compression       
			foreach (string file in filenames)     
			{      
				if(file.EndsWith(".DS_Store"))continue;
				if(file.EndsWith(".meta"))continue;
				//打开压缩文件      
				FileStream fs = File.OpenRead(file);          
				byte[] buffer = new byte[fs.Length];    
				fs.Read(buffer, 0, buffer.Length);     
				
				int index = file.LastIndexOf(Path.DirectorySeparatorChar);

				string FileName = file;
				if(file.StartsWith("Assets/"))
					FileName = file.Replace("Assets/","");

				//压缩可以带路径,取决于FileName是否带/以及它的完整度<本处带文件夹>
				ZipEntry entry = new ZipEntry( FileName );   

				entry.DateTime = DateTime.Now;         

                entry.Size = fs.Length;   
				fs.Close();       
				crc.Reset();    
				crc.Update(buffer);       
				entry.Crc  = crc.Value;       
				s.PutNextEntry(entry);       
				s.Write(buffer, 0, buffer.Length);   
			}    
			s.Finish(); 
			s.Close(); 
		} 
	} 
} 
