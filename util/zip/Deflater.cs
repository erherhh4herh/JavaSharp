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
	/// This class provides support for general purpose compression using the
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
	///     String inputString = "blahblahblah";
	///     byte[] input = inputString.getBytes("UTF-8");
	/// 
	///     // Compress the bytes
	///     byte[] output = new byte[100];
	///     Deflater compresser = new Deflater();
	///     compresser.setInput(input);
	///     compresser.finish();
	///     int compressedDataLength = compresser.deflate(output);
	///     compresser.end();
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
	/// <seealso cref=         Inflater
	/// @author      David Connelly </seealso>
	public class Deflater
	{

		private readonly ZStreamRef ZsRef;
		private sbyte[] Buf = new sbyte[0];
		private int Off, Len;
		private int Level_Renamed, Strategy_Renamed;
		private bool SetParams;
		private bool Finish_Renamed, Finished_Renamed;
		private long BytesRead_Renamed;
		private long BytesWritten_Renamed;

		/// <summary>
		/// Compression method for the deflate algorithm (the only one currently
		/// supported).
		/// </summary>
		public const int DEFLATED = 8;

		/// <summary>
		/// Compression level for no compression.
		/// </summary>
		public const int NO_COMPRESSION = 0;

		/// <summary>
		/// Compression level for fastest compression.
		/// </summary>
		public const int BEST_SPEED = 1;

		/// <summary>
		/// Compression level for best compression.
		/// </summary>
		public const int BEST_COMPRESSION = 9;

		/// <summary>
		/// Default compression level.
		/// </summary>
		public const int DEFAULT_COMPRESSION = -1;

		/// <summary>
		/// Compression strategy best used for data consisting mostly of small
		/// values with a somewhat random distribution. Forces more Huffman coding
		/// and less string matching.
		/// </summary>
		public const int FILTERED = 1;

		/// <summary>
		/// Compression strategy for Huffman coding only.
		/// </summary>
		public const int HUFFMAN_ONLY = 2;

		/// <summary>
		/// Default compression strategy.
		/// </summary>
		public const int DEFAULT_STRATEGY = 0;

		/// <summary>
		/// Compression flush mode used to achieve best compression result.
		/// </summary>
		/// <seealso cref= Deflater#deflate(byte[], int, int, int)
		/// @since 1.7 </seealso>
		public const int NO_FLUSH = 0;

		/// <summary>
		/// Compression flush mode used to flush out all pending output; may
		/// degrade compression for some compression algorithms.
		/// </summary>
		/// <seealso cref= Deflater#deflate(byte[], int, int, int)
		/// @since 1.7 </seealso>
		public const int SYNC_FLUSH = 2;

		/// <summary>
		/// Compression flush mode used to flush out all pending output and
		/// reset the deflater. Using this mode too often can seriously degrade
		/// compression.
		/// </summary>
		/// <seealso cref= Deflater#deflate(byte[], int, int, int)
		/// @since 1.7 </seealso>
		public const int FULL_FLUSH = 3;

		static Deflater()
		{
			/* Zip library is loaded from System.initializeSystemClass */
			initIDs();
		}

		/// <summary>
		/// Creates a new compressor using the specified compression level.
		/// If 'nowrap' is true then the ZLIB header and checksum fields will
		/// not be used in order to support the compression format used in
		/// both GZIP and PKZIP. </summary>
		/// <param name="level"> the compression level (0-9) </param>
		/// <param name="nowrap"> if true then use GZIP compatible compression </param>
		public Deflater(int level, bool nowrap)
		{
			this.Level_Renamed = level;
			this.Strategy_Renamed = DEFAULT_STRATEGY;
			this.ZsRef = new ZStreamRef(init(level, DEFAULT_STRATEGY, nowrap));
		}

		/// <summary>
		/// Creates a new compressor using the specified compression level.
		/// Compressed data will be generated in ZLIB format. </summary>
		/// <param name="level"> the compression level (0-9) </param>
		public Deflater(int level) : this(level, false)
		{
		}

		/// <summary>
		/// Creates a new compressor with the default compression level.
		/// Compressed data will be generated in ZLIB format.
		/// </summary>
		public Deflater() : this(DEFAULT_COMPRESSION, false)
		{
		}

		/// <summary>
		/// Sets input data for compression. This should be called whenever
		/// needsInput() returns true indicating that more input data is required. </summary>
		/// <param name="b"> the input data bytes </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the length of the data </param>
		/// <seealso cref= Deflater#needsInput </seealso>
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
		/// Sets input data for compression. This should be called whenever
		/// needsInput() returns true indicating that more input data is required. </summary>
		/// <param name="b"> the input data bytes </param>
		/// <seealso cref= Deflater#needsInput </seealso>
		public virtual sbyte[] Input
		{
			set
			{
				SetInput(value, 0, value.Length);
			}
		}

		/// <summary>
		/// Sets preset dictionary for compression. A preset dictionary is used
		/// when the history buffer can be predetermined. When the data is later
		/// uncompressed with Inflater.inflate(), Inflater.getAdler() can be called
		/// in order to get the Adler-32 value of the dictionary required for
		/// decompression. </summary>
		/// <param name="b"> the dictionary data bytes </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the length of the data </param>
		/// <seealso cref= Inflater#inflate </seealso>
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
			}
		}

		/// <summary>
		/// Sets preset dictionary for compression. A preset dictionary is used
		/// when the history buffer can be predetermined. When the data is later
		/// uncompressed with Inflater.inflate(), Inflater.getAdler() can be called
		/// in order to get the Adler-32 value of the dictionary required for
		/// decompression. </summary>
		/// <param name="b"> the dictionary data bytes </param>
		/// <seealso cref= Inflater#inflate </seealso>
		/// <seealso cref= Inflater#getAdler </seealso>
		public virtual sbyte[] Dictionary
		{
			set
			{
				SetDictionary(value, 0, value.Length);
			}
		}

		/// <summary>
		/// Sets the compression strategy to the specified value.
		/// 
		/// <para> If the compression strategy is changed, the next invocation
		/// of {@code deflate} will compress the input available so far with
		/// the old strategy (and may be flushed); the new strategy will take
		/// effect only after that invocation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="strategy"> the new compression strategy </param>
		/// <exception cref="IllegalArgumentException"> if the compression strategy is
		///                                     invalid </exception>
		public virtual int Strategy
		{
			set
			{
				switch (value)
				{
				  case DEFAULT_STRATEGY:
				  case FILTERED:
				  case HUFFMAN_ONLY:
					break;
				  default:
					throw new IllegalArgumentException();
				}
				lock (ZsRef)
				{
					if (this.Strategy_Renamed != value)
					{
						this.Strategy_Renamed = value;
						SetParams = true;
					}
				}
			}
		}

		/// <summary>
		/// Sets the compression level to the specified value.
		/// 
		/// <para> If the compression level is changed, the next invocation
		/// of {@code deflate} will compress the input available so far
		/// with the old level (and may be flushed); the new level will
		/// take effect only after that invocation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="level"> the new compression level (0-9) </param>
		/// <exception cref="IllegalArgumentException"> if the compression level is invalid </exception>
		public virtual int Level
		{
			set
			{
				if ((value < 0 || value > 9) && value != DEFAULT_COMPRESSION)
				{
					throw new IllegalArgumentException("invalid compression level");
				}
				lock (ZsRef)
				{
					if (this.Level_Renamed != value)
					{
						this.Level_Renamed = value;
						SetParams = true;
					}
				}
			}
		}

		/// <summary>
		/// Returns true if the input data buffer is empty and setInput()
		/// should be called in order to provide more input. </summary>
		/// <returns> true if the input data buffer is empty and setInput()
		/// should be called in order to provide more input </returns>
		public virtual bool NeedsInput()
		{
			return Len <= 0;
		}

		/// <summary>
		/// When called, indicates that compression should end with the current
		/// contents of the input buffer.
		/// </summary>
		public virtual void Finish()
		{
			lock (ZsRef)
			{
				Finish_Renamed = true;
			}
		}

		/// <summary>
		/// Returns true if the end of the compressed data output stream has
		/// been reached. </summary>
		/// <returns> true if the end of the compressed data output stream has
		/// been reached </returns>
		public virtual bool Finished()
		{
			lock (ZsRef)
			{
				return Finished_Renamed;
			}
		}

		/// <summary>
		/// Compresses the input data and fills specified buffer with compressed
		/// data. Returns actual number of bytes of compressed data. A return value
		/// of 0 indicates that <seealso cref="#needsInput() needsInput"/> should be called
		/// in order to determine if more input data is required.
		/// 
		/// <para>This method uses <seealso cref="#NO_FLUSH"/> as its compression flush mode.
		/// An invocation of this method of the form {@code deflater.deflate(b, off, len)}
		/// yields the same result as the invocation of
		/// {@code deflater.deflate(b, off, len, Deflater.NO_FLUSH)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b"> the buffer for the compressed data </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the maximum number of bytes of compressed data </param>
		/// <returns> the actual number of bytes of compressed data written to the
		///         output buffer </returns>
		public virtual int Deflate(sbyte[] b, int off, int len)
		{
			return Deflate(b, off, len, NO_FLUSH);
		}

		/// <summary>
		/// Compresses the input data and fills specified buffer with compressed
		/// data. Returns actual number of bytes of compressed data. A return value
		/// of 0 indicates that <seealso cref="#needsInput() needsInput"/> should be called
		/// in order to determine if more input data is required.
		/// 
		/// <para>This method uses <seealso cref="#NO_FLUSH"/> as its compression flush mode.
		/// An invocation of this method of the form {@code deflater.deflate(b)}
		/// yields the same result as the invocation of
		/// {@code deflater.deflate(b, 0, b.length, Deflater.NO_FLUSH)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b"> the buffer for the compressed data </param>
		/// <returns> the actual number of bytes of compressed data written to the
		///         output buffer </returns>
		public virtual int Deflate(sbyte[] b)
		{
			return Deflate(b, 0, b.Length, NO_FLUSH);
		}

		/// <summary>
		/// Compresses the input data and fills the specified buffer with compressed
		/// data. Returns actual number of bytes of data compressed.
		/// 
		/// <para>Compression flush mode is one of the following three modes:
		/// 
		/// <ul>
		/// <li><seealso cref="#NO_FLUSH"/>: allows the deflater to decide how much data
		/// to accumulate, before producing output, in order to achieve the best
		/// compression (should be used in normal use scenario). A return value
		/// of 0 in this flush mode indicates that <seealso cref="#needsInput()"/> should
		/// be called in order to determine if more input data is required.
		/// 
		/// <li><seealso cref="#SYNC_FLUSH"/>: all pending output in the deflater is flushed,
		/// to the specified output buffer, so that an inflater that works on
		/// compressed data can get all input data available so far (In particular
		/// the <seealso cref="#needsInput()"/> returns {@code true} after this invocation
		/// if enough output space is provided). Flushing with <seealso cref="#SYNC_FLUSH"/>
		/// may degrade compression for some compression algorithms and so it
		/// should be used only when necessary.
		/// 
		/// <li><seealso cref="#FULL_FLUSH"/>: all pending output is flushed out as with
		/// <seealso cref="#SYNC_FLUSH"/>. The compression state is reset so that the inflater
		/// that works on the compressed output data can restart from this point
		/// if previous compressed data has been damaged or if random access is
		/// desired. Using <seealso cref="#FULL_FLUSH"/> too often can seriously degrade
		/// compression.
		/// </ul>
		/// 
		/// </para>
		/// <para>In the case of <seealso cref="#FULL_FLUSH"/> or <seealso cref="#SYNC_FLUSH"/>, if
		/// the return value is {@code len}, the space available in output
		/// buffer {@code b}, this method should be invoked again with the same
		/// {@code flush} parameter and more output space.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b"> the buffer for the compressed data </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the maximum number of bytes of compressed data </param>
		/// <param name="flush"> the compression flush mode </param>
		/// <returns> the actual number of bytes of compressed data written to
		///         the output buffer
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if the flush mode is invalid
		/// @since 1.7 </exception>
		public virtual int Deflate(sbyte[] b, int off, int len, int flush)
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
				if (flush == NO_FLUSH || flush == SYNC_FLUSH || flush == FULL_FLUSH)
				{
					int thisLen = this.Len;
					int n = deflateBytes(ZsRef.Address(), b, off, len, flush);
					BytesWritten_Renamed += n;
					BytesRead_Renamed += (thisLen - this.Len);
					return n;
				}
				throw new IllegalArgumentException();
			}
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
		/// Returns the total number of uncompressed bytes input so far.
		/// 
		/// <para>Since the number of bytes may be greater than
		/// Integer.MAX_VALUE, the <seealso cref="#getBytesRead()"/> method is now
		/// the preferred means of obtaining this information.</para>
		/// </summary>
		/// <returns> the total number of uncompressed bytes input so far </returns>
		public virtual int TotalIn
		{
			get
			{
				return (int) BytesRead;
			}
		}

		/// <summary>
		/// Returns the total number of uncompressed bytes input so far.
		/// </summary>
		/// <returns> the total (non-negative) number of uncompressed bytes input so far
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
		/// Returns the total number of compressed bytes output so far.
		/// 
		/// <para>Since the number of bytes may be greater than
		/// Integer.MAX_VALUE, the <seealso cref="#getBytesWritten()"/> method is now
		/// the preferred means of obtaining this information.</para>
		/// </summary>
		/// <returns> the total number of compressed bytes output so far </returns>
		public virtual int TotalOut
		{
			get
			{
				return (int) BytesWritten;
			}
		}

		/// <summary>
		/// Returns the total number of compressed bytes output so far.
		/// </summary>
		/// <returns> the total (non-negative) number of compressed bytes output so far
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
		/// Resets deflater so that a new set of input data can be processed.
		/// Keeps current compression level and strategy settings.
		/// </summary>
		public virtual void Reset()
		{
			lock (ZsRef)
			{
				EnsureOpen();
				reset(ZsRef.Address());
				Finish_Renamed = false;
				Finished_Renamed = false;
				Off = Len = 0;
				BytesRead_Renamed = BytesWritten_Renamed = 0;
			}
		}

		/// <summary>
		/// Closes the compressor and discards any unprocessed input.
		/// This method should be called when the compressor is no longer
		/// being used, but will also be called automatically by the
		/// finalize() method. Once this method is called, the behavior
		/// of the Deflater object is undefined.
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
		/// Closes the compressor when garbage is collected.
		/// </summary>
		~Deflater()
		{
			End();
		}

		private void EnsureOpen()
		{
			Debug.Assert(Thread.holdsLock(ZsRef));
			if (ZsRef.Address() == 0)
			{
				throw new NullPointerException("Deflater has been closed");
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static long init(int level, int strategy, bool nowrap);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void setDictionary(long addr, sbyte[] b, int off, int len);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern int deflateBytes(long addr, sbyte[] b, int off, int len, int flush);
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