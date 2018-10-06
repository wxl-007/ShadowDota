using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;

namespace Framework {
	public class BZip2 {
		/// <summary>
		/// Compress the specified sBuffer.
		/// </summary>
		/// <param name="sBuffer">S buffer.</param>
		public static string Compress(string sBuffer) {

			byte[] compressed = null;
			string compressedText = null;


			try {
				byte[] bytesBuffer = Encoding.UTF8.GetBytes(sBuffer);

				using(MemoryStream compressStream = new MemoryStream()) {
					using (BZip2OutputStream bzipStream = new BZip2OutputStream(compressStream)) {

						bzipStream.Write(bytesBuffer, 0, bytesBuffer.Length);
						bzipStream.Flush();
						bzipStream.Close();

						compressed = compressStream.ToArray();
					}
				}

			} catch(Exception ex) {
				ConsoleEx.DebugLog("BZip2 compress error : " + ex.ToString());
			} finally {
				if(compressed != null)
					compressedText = Convert.ToBase64String(compressed);
			}

			return compressedText;
		}

		/// <summary>
		/// Decompress the specified compbytes.
		/// </summary>
		/// <param name="compbytes">Compbytes.</param>
		public static string Decompress(string compbytes) {
			byte[] cleanData = null;
			string cleanText = null;
			try {

				byte[] compressedData = Convert.FromBase64String(compbytes);

				using (MemoryStream compressedStream = new MemoryStream(compressedData)){
					using (BZip2InputStream decompressionStream = new BZip2InputStream(compressedStream))
					{

						using (MemoryStream decompressedStream = new MemoryStream())
						{
							StreamUtils.Copy(decompressionStream, decompressedStream, new byte[4096]);
							cleanData = decompressedStream.ToArray();
						}

						decompressionStream.Close();
						compressedStream.Close();
					}
				}

			} catch(Exception ex) {
				ConsoleEx.DebugLog("BZip2 decompress error : " + ex.ToString());
			} finally {
				if(cleanData != null)
					cleanText = Encoding.UTF8.GetString(cleanData, 0, cleanData.Length);
			}
			return cleanText;
		}
	}


	public class GZip {

		/// <summary>
		/// Compress the specified sBuffer.
		/// </summary>
		/// <param name="sBuffer">S buffer.</param>
		public static string Compress(string sBuffer) {
			string b64 = null;

			MemoryStream rawDataStream = null;
			GZipOutputStream gzipOut = null;
			try {
				rawDataStream = new MemoryStream();
				gzipOut = new GZipOutputStream(rawDataStream);

				byte[] sIn = UTF8Encoding.UTF8.GetBytes(sBuffer);
				gzipOut.IsStreamOwner = false;

				gzipOut.Write(sIn, 0, sIn.Length);
				gzipOut.Close();

				byte[] compressed = rawDataStream.ToArray();
				// data sent to the php service
				b64 = Convert.ToBase64String(compressed);
			} catch(Exception ex) {
				ConsoleEx.DebugLog("GZip compress error : " + ex.ToString());
			} finally {
				if(gzipOut != null) {
					gzipOut.Dispose();
					gzipOut = null;
				}

				if(rawDataStream != null) {
					rawDataStream.Dispose();
					rawDataStream = null;
				}
			}

			return b64;
		}

		/// <summary>
		/// Decompress the specified compbytes.
		/// </summary>
		/// <param name="compbytes">Compbytes.</param>
		public static string Decompress (string compbytes) {
			string result = null;

			StringBuilder sb = new StringBuilder();
			MemoryStream m_msGZip = null;
			GZipInputStream gZipIn = null;
			try {
				m_msGZip = new MemoryStream(Convert.FromBase64String(compbytes));
				gZipIn = new GZipInputStream(m_msGZip);

				// Use a 4K buffer. Any larger is a waste.  
				byte[] bytesUncompressed = new byte[4096];

				int readed = 0;
				do {
					readed = gZipIn.Read(bytesUncompressed, 0, bytesUncompressed.Length);
					if(readed > 0) {
						result = Encoding.ASCII.GetString(bytesUncompressed, 0, readed);
						sb.Append(result);
					}

				} while(readed > 0);

			} catch(Exception ex) {
				ConsoleEx.DebugLog("GZip decompress error : " + ex.ToString());
			} finally {
				if(m_msGZip != null) {
					m_msGZip.Dispose();
					m_msGZip = null;
				}

				if(gZipIn != null) {
					gZipIn.Dispose();
					gZipIn = null;
				}

				result = sb.ToString();
			}

			return result;
		}

	}


}
