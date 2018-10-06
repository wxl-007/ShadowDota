using System;
using System.IO;

public class FileUtility {
	public static void createFolder (string activeDir, string subDir)
	{
		//Create a new subfolder under the current active folder
		string newPath = System.IO.Path.Combine (activeDir, subDir);
		createFolder(newPath);
	}

	public static void createFolder(string path) {
		// Determine whether the directory exists.
		if (!Directory.Exists (path)) {
			// Create the subfolder
			Directory.CreateDirectory (path);
		}
	}

	/// <summary>
	/// Deletes the sub folder.删除path目录下所有的文件，不保留目录
	/// </summary>
	/// <param name="path">Path.</param>
	public static void deleteSubFolder(string path) {
		
		try {
			DirectoryInfo Dir = new DirectoryInfo(path);
			DirectoryInfo[] subDirArray = Dir.GetDirectories();
	
			if(subDirArray != null) {
				foreach (DirectoryInfo subDir in subDirArray) {
					FileInfo[] fileArray = subDir.GetFiles();
					if(fileArray != null) {
						foreach(FileInfo file in fileArray) {
							file.Delete();
						}
					}
					
				}
			}	
		} catch (Exception ex) {
            ConsoleEx.DebugLog(ex.ToString());
		}
		
	}

	/// <summary>
	/// 递归删除子文件夹, 但保留最高目录，不保留子目录
	/// </summary>
	/// <param name="dir">Dir.</param>
	public static void DeleteFolder(string dir) {
		if(string.IsNullOrEmpty(dir)) return;

		try {
			foreach (string d in Directory.GetFileSystemEntries(dir)) {
				if (File.Exists(d)) {
					FileInfo fi = new FileInfo(d);
					if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
						fi.Attributes = FileAttributes.Normal;
					File.Delete(d);
				}
				else {
					DirectoryInfo d1 = new DirectoryInfo(d);
					if (d1.GetFiles().Length != 0) {
						DeleteFolder(d1.FullName);//递归删除子文件夹
					}
					Directory.Delete(d);
				}
			}
		} catch(DirectoryNotFoundException ex) {
			ConsoleEx.DebugLog("ex : " + ex.ToString());
		} catch(IOException ex) {
			ConsoleEx.DebugLog("ex : " + ex.ToString());
		} catch(Exception ex) {
			ConsoleEx.DebugLog("ex : " + ex.ToString());
		}

	}

	//递归拷贝文件夹
	public static void CopyDirectory(string sourcePath, string destinationPath) {
		if(string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath)) {
			return;
		}

		try {
			DirectoryInfo info = new DirectoryInfo(sourcePath);
			Directory.CreateDirectory(destinationPath);
			
			foreach (FileSystemInfo fsi in info.GetFileSystemInfos()) {
				string destName = Path.Combine(destinationPath, fsi.Name);
				if (fsi is System.IO.FileInfo)      
					File.Copy(fsi.FullName, destName);
				else {
					Directory.CreateDirectory(destName);
					CopyDirectory(fsi.FullName, destName);
				}
			}
		} catch(IOException ex) {
			ConsoleEx.DebugLog("ex : " + ex.ToString());
		} catch(Exception ex) {
			ConsoleEx.DebugLog("ex : " + ex.ToString());
		}

	}

		
	//判定图片是否是Unity默认生产的错误图片
	public static bool isErrorImage(UnityEngine.Texture2D tex) {
		//The "?" image that Unity returns for an invalid www.texture has these consistent properties:
		//(we also reject null.)
		return (tex.height == 8 && tex.width == 8);
	}

	public static bool CopyAndPasteFolder(string sPath, string dPath){
		return MoveFile(false, sPath, dPath);
	}

	/// <summary>
	/// Copy文件夹
	/// </summary>
	/// <param name="sPath">源文件夹路径</param>
	/// <param name="dPath">目的文件夹路径</param>
	/// <returns>完成状态：true-完成</returns>
	public static bool CutAndPasteFolder(string sPath, string dPath)
	{
		return MoveFile(true, sPath, dPath);
	}

	private static bool MoveFile(bool cutFile , string sPath, string dPath){
		bool flag = true;
		try {
			// 创建目的文件夹
			if (!Directory.Exists(dPath))
				Directory.CreateDirectory(dPath);
			
			// 拷贝文件
			DirectoryInfo sDir = new DirectoryInfo(sPath);
			FileInfo[] fileArray = sDir.GetFiles();
			if(fileArray != null)
			foreach (FileInfo file in fileArray) {
				file.CopyTo( System.IO.Path.Combine (dPath, file.Name), true);
				if(cutFile) file.Delete();
			}
			
			// 循环子文件夹
			DirectoryInfo[] subDirArray = sDir.GetDirectories();
			
			if(subDirArray != null) {
				foreach (DirectoryInfo subDir in subDirArray)
					CutAndPasteFolder( subDir.FullName, System.IO.Path.Combine(dPath,subDir.Name) );
			}
		}
		catch (Exception ex) {
            ConsoleEx.DebugLog("ex : " + ex.ToString());
			flag = false;
		}
		return flag;
	}

	// Append content to file
	public static void AppendFile(string filepath,string outdata) {

		try{
			using (StreamWriter sw = new StreamWriter( File.Open(filepath , FileMode.Append) )) {
				sw.Write (outdata);
			}	
		}catch(Exception e) {
            ConsoleEx.DebugLog("ex : " + e.ToString());
		}

	}




}


