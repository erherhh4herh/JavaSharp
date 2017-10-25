using System.Diagnostics;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
 * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 */

namespace java.util.zip
{

	/// <summary>
	/// This class provides support for general purpose decompression using the
	/// popular ZLIB compression library. The ZLIB compression library was
	/// initially developed as part of the PNG graphics standard and is not
	/// protected by patents. It is fully described in the specifications at
	/// the <a href="package-summary.html#package_description">java.util.zip
	/// package description</a>.
	/// 
	/// <para>The following code fragment demonstrates a trivial compression
	/// and decompression of a string using <tt>Deflater</tt> and
	/// <tt>Inflater</tt>.
	/// 
	/// <blockquote><pre>
	/// try {
	///     // Encode a String into bytes
	///     String inputString = "blahblahblah\u20AC\u20AC";
	///     byte[] input = inputString.getBytes("UTF-8");
	/// 
	///     // Compress the bytes
	///     byte[] output = new byte[100];
	///     Deflater compresser = new Deflater();
	///     compresser.setInput(input);
	///     compresser.finish();
	///     int compressedDataLength = compresser.deflate(output);
	/// 
	///     // Decompress the bytes
	///     Inflater decompresser = new Inflater();
	///     decompresser.setInput(output, 0, compressedDataLength);
	///     byte[] result = new byte[100];
	///     int resultLength = decompresser.inflate(result);
	///     decompresser.end();
	/// 
	///     // Decode the bytes into a String
	///     String outputString = new String(result, 0, resultLength, "UTF-8");
	/// } catch(java.io.UnsupportedEncodingException ex) {
	///     // handle
	/// } catch (java.util.zip.DataFormatException ex) {
	///     // handle
	/// }
	/// </pre></blockquote>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=         Deflater
	/// @author      David Connelly
	///  </seealso>
	public class Inflater
	{

		private readonly ZStreamRef ZsRef;
		private sbyte[] Buf = DefaultBuf;
		private int Off, Len;
		private bool Finished_Renamed;
		private bool NeedDict;
		private long BytesRead_Renamed;
		private long BytesWritten_Renamed;

		private static readonly sbyte[] DefaultBuf = new sbyte[0];

		static Inflater()
		{
			/* Zip library is loaded from System.initializeSystemClass */
			initIDs();
		}

		/// <summary>
		/// Creates a new decompressor. If the parameter 'nowrap' is true then
		/// the ZLIB header and checksum fields will not be used. This provides
		/// compatibility with the compression format used by both GZIP and PKZIP.
		/// <para>
		/// Note: When using the 'nowrap' option it is also necessary to provide
		/// an extra "dummy" byte as input. This is required by the ZLIB native
		/// library in order to support certain optimizations.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nowrap"> if true then support GZIP compatible compression </param>
		public Inflater(bool nowrap)
		{
			ZsRef = new ZStreamRef(init(nowrap));
		}

		/// <summary>
		/// Creates a new decompressor.
		/// </summary>
		public Inflater() : this(false)
		{
		}

		/// <summary>
		/// Sets input data for decompression. Should be called whenever
		/// needsInput() returns true indicating that more input data is
		/// required. </summary>
		/// <param name="b"> the input data bytes </param>
		/// <param name="off"> the start offset of the input data </param>
		/// <param name="len"> the length of the input data </param>
		/// <seealso cref= Inflater#needsInput </seealso>
		public virtual void SetInput(sbyte[] b, int off, int len)
		{
			if (b == null)
			{
				throw new NullPointerException();
			}
			if (off < 0 || len < 0 || off > b.Length - len)
			{
				throw new ArrayIndexOutOfBoundsException();
			}
			lock (ZsRef)
			{
				this.Buf = b;
				this.Off = off;
				this.Len = len;
			}
		}

		/// <summary>
		/// Sets input data for decompression. Should be called whenever
		/// needsInput() returns true indicating that more input data is
		/// required. </summary>
		/// <param name="b"> the input data bytes </param>
		/// <seealso cref= Inflater#needsInput </seealso>
		public virtual sbyte[] Input
		{
			set
			{
				SetInput(value, 0, value.Length);
			}
		}

		/// <summary>
		/// Sets the preset dictionary to the given array of bytes. Should be
		/// called when inflate() returns 0 and needsDictionary() returns true
		/// indicating that a preset dictionary is required. The method getAdler()
		/// can be used to get the Adler-32 value of the dictionary needed. </summary>
		/// <param name="b"> the dictionary data bytes </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the length of the data </param>
		/// <seealso cref= Inflater#needsDictionary </seealso>
		/// <seealso cref= Inflater#getAdler </seealso>
		public virtual void SetDictionary(sbyte[] b, int off, int len)
		{
			if (b == null)
			{
				throw new NullPointerException();
			}
			if (off < 0 || len < 0 || off > b.Length - len)
			{
				throw new ArrayIndexOutOfBoundsException();
			}
			lock (ZsRef)
			{
				EnsureOpen();
				setDictionary(ZsRef.Address(), b, off, len);
				NeedDict = false;
			}
		}

		/// <summary>
		/// Sets the preset dictionary to the given array of bytes. Should be
		/// called when inflate() returns 0 and needsDictionary() returns true
		/// indicating that a preset dictionary is required. The method getAdler()
		/// can be used to get the Adler-32 value of the dictionary needed. </summary>
		/// <param name="b"> the dictionary data bytes </param>
		/// <seealso cref= Inflater#needsDictionary </seealso>
		/// <seealso cref= Inflater#getAdler </seealso>
		public virtual sbyte[] Dictionary
		{
			set
			{
				SetDictionary(value, 0, value.Length);
			}
		}

		/// <summary>
		/// Returns the total number of bytes remaining in the input buffer.
		/// This can be used to find out what bytes still remain in the input
		/// buffer after decompression has finished. </summary>
		/// <returns> the total number of bytes remaining in the input buffer </returns>
		public virtual int Remaining
		{
			get
			{
				lock (ZsRef)
				{
					return Len;
				}
			}
		}

		/// <summary>
		/// Returns true if no data remains in the input buffer. This can
		/// be used to determine if #setInput should be called in order
		/// to provide more input. </summary>
		/// <returns> true if no data remains in the input buffer </returns>
		public virtual bool NeedsInput()
		{
			lock (ZsRef)
			{
				return Len <= 0;
			}
		}

		/// <summary>
		/// Returns true if a preset dictionary is needed for decompression. </summary>
		/// <returns> true if a preset dictionary is needed for decompression </returns>
		/// <seealso cref= Inflater#setDictionary </seealso>
		public virtual bool NeedsDictionary()
		{
			lock (ZsRef)
			{
				return NeedDict;
			}
		}

		/// <summary>
		/// Returns true if the end of the compressed data stream has been
		/// reached. </summary>
		/// <returns> true if the end of the compressed data stream has been
		/// reached </returns>
		public virtual bool Finished()
		{
			lock (ZsRef)
			{
				return Finished_Renamed;
			}
		}

		/// <summary>
		/// Uncompresses bytes into specified buffer. Returns actual number
		/// of bytes uncompressed. A return value of 0 indicates that
		/// needsInput() or needsDictionary() should be called in order to
		/// determine if more input data or a preset dictionary is required.
		/// In the latter case, getAdler() can be used to get the Adler-32
		/// value of the dictionary required. </summary>
		/// <param name="b"> the buffer for the uncompressed data </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the maximum number of uncompressed bytes </param>
		/// <returns> the actual number of uncompressed bytes </returns>
		/// <exception cref="DataFormatException"> if the compressed data format is invalid </exception>
		/// <seealso cref= Inflater#needsInput </seealso>
		/// <seealso cref= Inflater#needsDictionary </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int inflate(byte[] b, int off, int len) throws DataFormatException
		public virtual int Inflate(sbyte[] b, int off, int len)
		{
			if (b == null)
			{
				throw new NullPointerException();
			}
			if (off < 0 || len < 0 || off > b.Length - len)
			{
				throw new ArrayIndexOutOfBoundsException();
			}
			lock (ZsRef)
			{
				EnsureOpen();
				int thisLen = this.Len;
				int n = inflateBytes(ZsRef.Address(), b, off, len);
				BytesWritten_Renamed += n;
				BytesRead_Renamed += (thisLen - this.Len);
				return n;
			}
		}

		/// <summary>
		/// Uncompresses bytes into specified buffer. Returns actual number
		/// of bytes uncompressed. A return value of 0 indicates that
		/// needsInput() or needsDictionary() should be called in order to
		/// determine if more input data or a preset dictionary is required.
		/// In the latter case, getAdler() can be used to get the Adler-32
		/// value of the dictionary required. </summary>
		/// <param name="b"> the buffer for the uncompressed data </param>
		/// <returns> the actual number of uncompressed bytes </returns>
		/// <exception cref="DataFormatException"> if the compressed data format is invalid </exception>
		/// <seealso cref= Inflater#needsInput </seealso>
		/// <seealso cref= Inflater#needsDictionary </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int inflate(byte[] b) throws DataFormatException
		public virtual int Inflate(sbyte[] b)
		{
			return Inflate(b, 0, b.Length);
		}

		/// <summary>
		/// Returns the ADLER-32 value of the uncompressed data. </summary>
		/// <returns> the ADLER-32 value of the uncompressed data </returns>
		public virtual int Adler
		{
			get
			{
				lock (ZsRef)
				{
					EnsureOpen();
					return getAdler(ZsRef.Address());
				}
			}
		}

		/// <summary>
		/// Returns the total number of compressed bytes input so far.
		/// 
		/// <para>Since the number of bytes may be greater than
		/// Integer.MAX_VALUE, the <seealso cref="#getBytesRead()"/> method is now
		/// the preferred means of obtaining this information.</para>
		/// </summary>
		/// <returns> the total number of compressed bytes input so far </returns>
		public virtual int TotalIn
		{
			get
			{
				return (int) BytesRead;
			}
		}

		/// <summary>
		/// Returns the total number of compressed bytes input so far.
		/// </summary>
		/// <returns> the total (non-negative) number of compressed bytes input so far
		/// @since 1.5 </returns>
		public virtual long BytesRead
		{
			get
			{
				lock (ZsRef)
				{
					EnsureOpen();
					return BytesRead_Renamed;
				}
			}
		}

		/// <summary>
		/// Returns the total number of uncompressed bytes output so far.
		/// 
		/// <para>Since the number of bytes may be greater than
		/// Integer.MAX_VALUE, the <seealso cref="#getBytesWritten()"/> method is now
		/// the preferred means of obtaining this information.</para>
		/// </summary>
		/// <returns> the total number of uncompressed bytes output so far </returns>
		public virtual int TotalOut
		{
			get
			{
				return (int) BytesWritten;
			}
		}

		/// <summary>
		/// Returns the total number of uncompressed bytes output so far.
		/// </summary>
		/// <returns> the total (non-negative) number of uncompressed bytes output so far
		/// @since 1.5 </returns>
		public virtual long BytesWritten
		{
			get
			{
				lock (ZsRef)
				{
					EnsureOpen();
					return BytesWritten_Renamed;
				}
			}
		}

		/// <summary>
		/// Resets inflater so that a new set of input data can be processed.
		/// </summary>
		public virtual void Reset()
		{
			lock (ZsRef)
			{
				EnsureOpen();
				reset(ZsRef.Address());
				Buf = DefaultBuf;
				Finished_Renamed = false;
				NeedDict = false;
				Off = Len = 0;
				BytesRead_Renamed = BytesWritten_Renamed = 0;
			}
		}

		/// <summary>
		/// Closes the decompressor and discards any unprocessed input.
		/// This method should be called when the decompressor is no longer
		/// being used, but will also be called automatically by the finalize()
		/// method. Once this method is called, the behavior of the Inflater
		/// object is undefined.
		/// </summary>
		public virtual void End()
		{
			lock (ZsRef)
			{
				long addr = ZsRef.Address();
				ZsRef.Clear();
				if (addr != 0)
				{
					end(addr);
					Buf = null;
				}
			}
		}

		/// <summary>
		/// Closes the decompressor when garbage is collected.
		/// </summary>
		~Inflater()
		{
			End();
		}

		private void EnsureOpen()
		{
			Debug.Assert(Thread.holdsLock(ZsRef));
			if (ZsRef.Address() == 0)
			{
				throw new NullPointerException("Inflater has been closed");
			}
		}

		internal virtual bool Ended()
		{
			lock (ZsRef)
			{
				return ZsRef.Address() == 0;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void initIDs();
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static long init(bool nowrap);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void setDictionary(long addr, sbyte[] b, int off, int len);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern int inflateBytes(long addr, sbyte[] b, int off, int len);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static int getAdler(long addr);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void reset(long addr);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void end(long addr);
	}

}