// StreamUtils.cs
//
// Copyright 2005 John Reilly
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// Linking this library statically or dynamically with other modules is
// making a combined work based on this library.  Thus, the terms and
// conditions of the GNU General Public License cover the whole
// combination.
// 
// As a special exception, the copyright holders of this library give you
// permission to link this library with independent modules to produce an
// executable, regardless of the license terms of these independent
// modules, and to copy and distribute the resulting executable under
// terms of your choice, provided that you also meet, for each linked
// independent module, the terms and conditions of the license of that
// module.  An independent module is a module which is not derived from
// or based on this library.  If you modify this library, you may extend
// this exception to your version of the library, but you are not
// obligated to do so.  If you do not wish to do so, delete this
// exception statement from your version.


using System;
using System.IO;

namespace Framework
{

	public delegate void ProgressHandler(object sender, ProgressEventArgs e);

	/// <summary>
	/// Event arguments during processing of a single file or directory.
	/// </summary>
	public class ProgressEventArgs : EventArgs
	{
		#region Constructors
		/// <summary>
		/// Initialise a new instance of <see cref="ScanEventArgs"/>
		/// </summary>
		/// <param name="name">The file or directory name if known.</param>
		/// <param name="processed">The number of bytes processed so far</param>
		/// <param name="target">The total number of bytes to process, 0 if not known</param>
		public ProgressEventArgs(string name, long processed, long target)
		{
			name_ = name;
			processed_ = processed;
			target_ = target;
		}
		#endregion

		/// <summary>
		/// The name for this event if known.
		/// </summary>
		public string Name
		{
			get { return name_; }
		}

		/// <summary>
		/// Get set a value indicating wether scanning should continue or not.
		/// </summary>
		public bool ContinueRunning
		{
			get { return continueRunning_; }
			set { continueRunning_ = value; }
		}

		/// <summary>
		/// Get a percentage representing how much of the <see cref="Target"></see> has been processed
		/// </summary>
		/// <value>0.0 to 100.0 percent; 0 if target is not known.</value>
		public float PercentComplete
		{
			get
			{
				float result;
				if (target_ <= 0)
				{
					result = 0;
				}
				else
				{
					result = ((float)processed_ / (float)target_) * 100.0f;
				}
				return result;
			}
		}

		/// <summary>
		/// The number of bytes processed so far
		/// </summary>
		public long Processed
		{
			get { return processed_; }
		}

		/// <summary>
		/// The number of bytes to process.
		/// </summary>
		/// <remarks>Target may be 0 or negative if the value isnt known.</remarks>
		public long Target
		{
			get { return target_; }
		}

		#region Instance Fields
		string name_;
		long processed_;
		long target_;
		bool continueRunning_ = true;
		#endregion
	}

	/// <summary>
	/// Provides simple <see cref="Stream"/>" utilities.
	/// </summary>
	public sealed class StreamUtils
	{
		/// <summary>
		/// Read from a <see cref="Stream"/> ensuring all the required data is read.
		/// </summary>
		/// <param name="stream">The stream to read.</param>
		/// <param name="buffer">The buffer to fill.</param>
		/// <seealso cref="ReadFully(Stream,byte[],int,int)"/>
		static public void ReadFully(Stream stream, byte[] buffer)
		{
			ReadFully(stream, buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Read from a <see cref="Stream"/>" ensuring all the required data is read.
		/// </summary>
		/// <param name="stream">The stream to read data from.</param>
		/// <param name="buffer">The buffer to store data in.</param>
		/// <param name="offset">The offset at which to begin storing data.</param>
		/// <param name="count">The number of bytes of data to store.</param>
		/// <exception cref="ArgumentNullException">Required parameter is null</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> and or <paramref name="count"/> are invalid.</exception>
		/// <exception cref="EndOfStreamException">End of stream is encountered before all the data has been read.</exception>
		static public void ReadFully(Stream stream, byte[] buffer, int offset, int count)
		{
			if ( stream == null ) {
				throw new ArgumentNullException("stream");
			}

			if ( buffer == null ) {
				throw new ArgumentNullException("buffer");
			}

			// Offset can equal length when buffer and count are 0.
			if ( (offset < 0) || (offset > buffer.Length) ) {
				throw new ArgumentOutOfRangeException("offset");
			}

			if ( (count < 0) || (offset + count > buffer.Length) ) {
				throw new ArgumentOutOfRangeException("count");
			}

			while ( count > 0 ) {
				int readCount = stream.Read(buffer, offset, count);
				if ( readCount <= 0 ) {
					throw new EndOfStreamException();
				}
				offset += readCount;
				count -= readCount;
			}
		}

		/// <summary>
		/// Copy the contents of one <see cref="Stream"/> to another.
		/// </summary>
		/// <param name="source">The stream to source data from.</param>
		/// <param name="destination">The stream to write data to.</param>
		/// <param name="buffer">The buffer to use during copying.</param>
		static public void Copy(Stream source, Stream destination, byte[] buffer)
		{
			if (source == null) {
				throw new ArgumentNullException("source");
			}

			if (destination == null) {
				throw new ArgumentNullException("destination");
			}

			if (buffer == null) {
				throw new ArgumentNullException("buffer");
			}

			// Ensure a reasonable size of buffer is used without being prohibitive.
			if (buffer.Length < 128) {
				throw new ArgumentException("Buffer is too small", "buffer");
			}

			bool copying = true;

			while (copying) {
				int bytesRead = source.Read(buffer, 0, buffer.Length);
				if (bytesRead > 0) {
					destination.Write(buffer, 0, bytesRead);
				}
				else {
					destination.Flush();
					copying = false;
				}
			}
		}

		/// <summary>
		/// Copy the contents of one <see cref="Stream"/> to another.
		/// </summary>
		/// <param name="source">The stream to source data from.</param>
		/// <param name="destination">The stream to write data to.</param>
		/// <param name="buffer">The buffer to use during copying.</param>
		/// <param name="progressHandler">The <see cref="ProgressHandler">progress handler delegate</see> to use.</param>
		/// <param name="updateInterval">The minimum <see cref="TimeSpan"/> between progress updates.</param>
		/// <param name="sender">The source for this event.</param>
		/// <param name="name">The name to use with the event.</param>
		/// <remarks>This form is specialised for use within #Zip to support events during archive operations.</remarks>
		static public void Copy(Stream source, Stream destination,
			byte[] buffer, ProgressHandler progressHandler, TimeSpan updateInterval, object sender, string name)
		{
			Copy(source, destination, buffer, progressHandler, updateInterval, sender, name, -1);
		}

		/// <summary>
		/// Copy the contents of one <see cref="Stream"/> to another.
		/// </summary>
		/// <param name="source">The stream to source data from.</param>
		/// <param name="destination">The stream to write data to.</param>
		/// <param name="buffer">The buffer to use during copying.</param>
		/// <param name="progressHandler">The <see cref="ProgressHandler">progress handler delegate</see> to use.</param>
		/// <param name="updateInterval">The minimum <see cref="TimeSpan"/> between progress updates.</param>
		/// <param name="sender">The source for this event.</param>
		/// <param name="name">The name to use with the event.</param>
		/// <param name="fixedTarget">A predetermined fixed target value to use with progress updates.
		/// If the value is negative the target is calculated by looking at the stream.</param>
		/// <remarks>This form is specialised for use within #Zip to support events during archive operations.</remarks>
		static public void Copy(Stream source, Stream destination,
			byte[] buffer, 
			ProgressHandler progressHandler, TimeSpan updateInterval, 
			object sender, string name, long fixedTarget)
		{
			if (source == null) {
				throw new ArgumentNullException("source");
			}

			if (destination == null) {
				throw new ArgumentNullException("destination");
			}

			if (buffer == null) {
				throw new ArgumentNullException("buffer");
			}

			// Ensure a reasonable size of buffer is used without being prohibitive.
			if (buffer.Length < 128) {
				throw new ArgumentException("Buffer is too small", "buffer");
			}

			if (progressHandler == null) {
				throw new ArgumentNullException("progressHandler");
			}

			bool copying = true;

			DateTime marker = DateTime.Now;
			long processed = 0;
			long target = 0;

			if (fixedTarget >= 0) {
				target = fixedTarget;
			}
			else if (source.CanSeek) {
				target = source.Length - source.Position;
			}

			// Always fire 0% progress..
			ProgressEventArgs args = new ProgressEventArgs(name, processed, target);
			progressHandler(sender, args);

			bool progressFired = true;

			while (copying) {
				int bytesRead = source.Read(buffer, 0, buffer.Length);
				if (bytesRead > 0) {
					processed += bytesRead;
					progressFired = false;
					destination.Write(buffer, 0, bytesRead);
				}
				else {
					destination.Flush();
					copying = false;
				}

				if (DateTime.Now - marker > updateInterval) {
					progressFired = true;
					marker = DateTime.Now;
					args = new ProgressEventArgs(name, processed, target);
					progressHandler(sender, args);

					copying = args.ContinueRunning;
				}
			}

			if (!progressFired) {
				args = new ProgressEventArgs(name, processed, target);
				progressHandler(sender, args);
			}
		}

		/// <summary>
		/// Initialise an instance of <see cref="StreamUtils"></see>
		/// </summary>
		private StreamUtils()
		{
			// Do nothing.
		}
	}
}