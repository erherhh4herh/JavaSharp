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
	/// This class implements a stream filter for reading compressed data in
	/// the GZIP file format.
	/// </summary>
	/// <seealso cref=         InflaterInputStream
	/// @author      David Connelly
	///  </seealso>
	public class GZIPInputStream : InflaterInputStream
	{
		/// <summary>
		/// CRC-32 for uncompressed data.
		/// </summary>
		protected internal CRC32 Crc = new CRC32();

		/// <summary>
		/// Indicates end of input stream.
		/// </summary>
		protected internal bool Eos;

		private bool Closed = false;

		/// <summary>
		/// Check to make sure that this stream has not been closed
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureOpen() throws java.io.IOException
		private void EnsureOpen()
		{
			if (Closed)
			{
				throw new IOException("Stream closed");
			}
		}

		/// <summary>
		/// Creates a new input stream with the specified buffer size. </summary>
		/// <param name="in"> the input stream </param>
		/// <param name="size"> the input buffer size
		/// </param>
		/// <exception cref="ZipException"> if a GZIP format error has occurred or the
		///                         compression method used is unsupported </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GZIPInputStream(java.io.InputStream in, int size) throws java.io.IOException
		public GZIPInputStream(InputStream @in, int size) : base(@in, new Inflater(true), size)
		{
			UsesDefaultInflater = true;
			ReadHeader(@in);
		}

		/// <summary>
		/// Creates a new input stream with a default buffer size. </summary>
		/// <param name="in"> the input stream
		/// </param>
		/// <exception cref="ZipException"> if a GZIP format error has occurred or the
		///                         compression method used is unsupported </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GZIPInputStream(java.io.InputStream in) throws java.io.IOException
		public GZIPInputStream(InputStream @in) : this(@in, 512)
		{
		}

		/// <summary>
		/// Reads uncompressed data into an array of bytes. If <code>len</code> is not
		/// zero, the method will block until some input can be decompressed; otherwise,
		/// no bytes are read and <code>0</code> is returned. </summary>
		/// <param name="buf"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset in the destination array <code>b</code> </param>
		/// <param name="len"> the maximum number of bytes read </param>
		/// <returns>  the actual number of bytes read, or -1 if the end of the
		///          compressed input stream is reached
		/// </returns>
		/// <exception cref="NullPointerException"> If <code>buf</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>buf.length - off</code> </exception>
		/// <exception cref="ZipException"> if the compressed input data is corrupt. </exception>
		/// <exception cref="IOException"> if an I/O error has occurred.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] buf, int off, int len) throws java.io.IOException
		public override int Read(sbyte[] buf, int off, int len)
		{
			EnsureOpen();
			if (Eos)
			{
				return -1;
			}
			int n = base.Read(buf, off, len);
			if (n == -1)
			{
				if (ReadTrailer())
				{
					Eos = true;
				}
				else
				{
					return this.Read(buf, off, len);
				}
			}
			else
			{
				Crc.Update(buf, off, n);
			}
			return n;
		}

		/// <summary>
		/// Closes this input stream and releases any system resources associated
		/// with the stream. </summary>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public override void Close()
		{
			if (!Closed)
			{
				base.Close();
				Eos = true;
				Closed = true;
			}
		}

		/// <summary>
		/// GZIP header magic number.
		/// </summary>
		public const int GZIP_MAGIC = 0x8b1f;

		/*
		 * File header flags.
		 */
		private const int FTEXT = 1; // Extra text
		private const int FHCRC = 2; // Header CRC
		private const int FEXTRA = 4; // Extra field
		private const int FNAME = 8; // File name
		private const int FCOMMENT = 16; // File comment

		/*
		 * Reads GZIP member header and returns the total byte number
		 * of this member header.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readHeader(java.io.InputStream this_in) throws java.io.IOException
		private int ReadHeader(InputStream this_in)
		{
			CheckedInputStream @in = new CheckedInputStream(this_in, Crc);
			Crc.Reset();
			// Check header magic
			if (ReadUShort(@in) != GZIP_MAGIC)
			{
				throw new ZipException("Not in GZIP format");
			}
			// Check compression method
			if (ReadUByte(@in) != 8)
			{
				throw new ZipException("Unsupported compression method");
			}
			// Read flags
			int flg = ReadUByte(@in);
			// Skip MTIME, XFL, and OS fields
			SkipBytes(@in, 6);
			int n = 2 + 2 + 6;
			// Skip optional extra field
			if ((flg & FEXTRA) == FEXTRA)
			{
				int m = ReadUShort(@in);
				SkipBytes(@in, m);
				n += m + 2;
			}
			// Skip optional file name
			if ((flg & FNAME) == FNAME)
			{
				do
				{
					n++;
				} while (ReadUByte(@in) != 0);
			}
			// Skip optional file comment
			if ((flg & FCOMMENT) == FCOMMENT)
			{
				do
				{
					n++;
				} while (ReadUByte(@in) != 0);
			}
			// Check optional header CRC
			if ((flg & FHCRC) == FHCRC)
			{
				int v = (int)Crc.Value & 0xffff;
				if (ReadUShort(@in) != v)
				{
					throw new ZipException("Corrupt GZIP header");
				}
				n += 2;
			}
			Crc.Reset();
			return n;
		}

		/*
		 * Reads GZIP member trailer and returns true if the eos
		 * reached, false if there are more (concatenated gzip
		 * data set)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean readTrailer() throws java.io.IOException
		private bool ReadTrailer()
		{
			InputStream @in = this.@in;
			int n = Inf.Remaining;
			if (n > 0)
			{
				@in = new SequenceInputStream(new ByteArrayInputStream(Buf, Len - n, n), new FilterInputStreamAnonymousInnerClassHelper(this, @in));
			}
			// Uses left-to-right evaluation order
			if ((ReadUInt(@in) != Crc.Value) || (ReadUInt(@in) != (Inf.BytesWritten & 0xffffffffL)))
			{
				// rfc1952; ISIZE is the input size modulo 2^32
				throw new ZipException("Corrupt GZIP trailer");
			}

			// If there are more bytes available in "in" or
			// the leftover in the "inf" is > 26 bytes:
			// this.trailer(8) + next.header.min(10) + next.trailer(8)
			// try concatenated case
			if (this.@in.Available() > 0 || n > 26)
			{
				int m = 8; // this.trailer
				try
				{
					m += ReadHeader(@in); // next.header
				}
				catch (IOException)
				{
					return true; // ignore any malformed, do nothing
				}
				Inf.Reset();
				if (n > m)
				{
					Inf.SetInput(Buf, Len - n + m, n - m);
				}
				return false;
			}
			return true;
		}

		private class FilterInputStreamAnonymousInnerClassHelper : FilterInputStream
		{
			private readonly GZIPInputStream OuterInstance;

			public FilterInputStreamAnonymousInnerClassHelper(GZIPInputStream outerInstance, InputStream @in) : base(@in)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
			public override void Close()
			{
			}
		}

		/*
		 * Reads unsigned integer in Intel byte order.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private long readUInt(java.io.InputStream in) throws java.io.IOException
		private long ReadUInt(InputStream @in)
		{
			long s = ReadUShort(@in);
			return ((long)ReadUShort(@in) << 16) | s;
		}

		/*
		 * Reads unsigned short in Intel byte order.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readUShort(java.io.InputStream in) throws java.io.IOException
		private int ReadUShort(InputStream @in)
		{
			int b = ReadUByte(@in);
			return (ReadUByte(@in) << 8) | b;
		}

		/*
		 * Reads unsigned byte.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readUByte(java.io.InputStream in) throws java.io.IOException
		private int ReadUByte(InputStream @in)
		{
			int b = @in.Read();
			if (b == -1)
			{
				throw new EOFException();
			}
			if (b < -1 || b > 255)
			{
				// Report on this.in, not argument in; see read{Header, Trailer}.
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new IOException(this.@in.GetType().FullName + ".read() returned value out of range -1..255: " + b);
			}
			return b;
		}

		private sbyte[] Tmpbuf = new sbyte[128];

		/*
		 * Skips bytes of input data blocking until all bytes are skipped.
		 * Does not assume that the input stream is capable of seeking.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void skipBytes(java.io.InputStream in, int n) throws java.io.IOException
		private void SkipBytes(InputStream @in, int n)
		{
			while (n > 0)
			{
				int len = @in.Read(Tmpbuf, 0, n < Tmpbuf.Length ? n : Tmpbuf.Length);
				if (len == -1)
				{
					throw new EOFException();
				}
				n -= len;
			}
		}
	}

}