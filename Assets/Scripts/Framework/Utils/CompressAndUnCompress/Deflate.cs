using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;


namespace Framework
{

	/// <summary>
	/// Deflate. Official Methods, but with None-AOT, so we can't use this one under IOS/Android version
	/// </summary>
	public sealed class Deflate {

		public static string Decompress(string b64pressed){

			byte[] cleanData = null;
			string cleanText = null;

			byte[] compressedData = Convert.FromBase64String(b64pressed);

			using (MemoryStream decompressedStream = new MemoryStream()) {
				using (MemoryStream compressedStream = new MemoryStream(compressedData)) {
					using (DeflateStream decompressionStream = new DeflateStream(compressedStream, CompressionMode.Decompress)) {
						StreamUtils.Copy(decompressionStream, decompressedStream, new byte[4096]);
						cleanData = decompressedStream.ToArray();
					}
				}
			}

			if(cleanData != null) {
				cleanText = Encoding.UTF8.GetString(cleanData, 0, cleanData.Length);
			}

			return cleanText;
		}
	}



	public sealed class DeflateEx {

		public static string Decompress(string b64pressed) {

			byte[] cleanData = null;
			string cleanText = null;

			byte[] compressedData = Convert.FromBase64String(b64pressed);

			using (MemoryStream decompressedStream = new MemoryStream()) {
				using (MemoryStream compressedStream = new MemoryStream(compressedData)) {
					using (InflaterInputStream decompressionStream = new InflaterInputStream(compressedStream, new Inflater(true)) ) {
						StreamUtils.Copy(decompressionStream, decompressedStream, new byte[4096]);
						cleanData = decompressedStream.ToArray();
					}
				}
			}

			if(cleanData != null) {
				cleanText = Encoding.UTF8.GetString(cleanData, 0, cleanData.Length);
			}

			return cleanText;

		}


		public static string Compress(string sBuffer) {

			byte[] compressed = null;
			string compressedText = null;

			try {
				byte[] bytesBuffer = Encoding.UTF8.GetBytes(sBuffer);

				using(MemoryStream compressStream = new MemoryStream()) {
					using (DeflaterOutputStream defStream = new DeflaterOutputStream(compressStream, new Deflater(Deflater.DEFAULT_COMPRESSION, true))) {

						defStream.Write(bytesBuffer, 0, bytesBuffer.Length);
						defStream.Flush();
						defStream.Close();

						compressed = compressStream.ToArray();
					}
				}

			} catch(Exception ex) {
				ConsoleEx.DebugLog("Deflater compress error : " + ex.ToString());
			} finally {
				if(compressed != null)
					compressedText = Convert.ToBase64String(compressed);
			}

			return compressedText;
		}
	}

}

