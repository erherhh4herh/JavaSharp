using System;

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
	/// This class implements a stream filter for uncompressing data in the
	/// "deflate" compression format. It is also used as the basis for other
	/// decompression filters, such as GZIPInputStream.
	/// </summary>
	/// <seealso cref=         Inflater
	/// @author      David Connelly </seealso>
	public class InflaterInputStream : FilterInputStream
	{
		/// <summary>
		/// Decompressor for this stream.
		/// </summary>
		protected internal Inflater Inf;

		/// <summary>
		/// Input buffer for decompression.
		/// </summary>
		protected internal sbyte[] Buf;

		/// <summary>
		/// Length of input buffer.
		/// </summary>
		protected internal int Len;

		private bool Closed = false;
		// this flag is set to true after EOF has reached
		private bool ReachEOF = false;

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
		/// Creates a new input stream with the specified decompressor and
		/// buffer size. </summary>
		/// <param name="in"> the input stream </param>
		/// <param name="inf"> the decompressor ("inflater") </param>
		/// <param name="size"> the input buffer size </param>
		/// <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
		public InflaterInputStream(InputStream @in, Inflater inf, int size) : base(@in)
		{
			if (@in == null || inf == null)
			{
				throw new NullPointerException();
			}
			else if (size <= 0)
			{
				throw new IllegalArgumentException("buffer size <= 0");
			}
			this.Inf = inf;
			Buf = new sbyte[size];
		}

		/// <summary>
		/// Creates a new input stream with the specified decompressor and a
		/// default buffer size. </summary>
		/// <param name="in"> the input stream </param>
		/// <param name="inf"> the decompressor ("inflater") </param>
		public InflaterInputStream(InputStream @in, Inflater inf) : this(@in, inf, 512)
		{
		}

		internal bool UsesDefaultInflater = false;

		/// <summary>
		/// Creates a new input stream with a default decompressor and buffer size. </summary>
		/// <param name="in"> the input stream </param>
		public InflaterInputStream(InputStream @in) : this(@in, new Inflater())
		{
			UsesDefaultInflater = true;
		}

		private sbyte[] SingleByteBuf = new sbyte[1];

		/// <summary>
		/// Reads a byte of uncompressed data. This method will block until
		/// enough input is available for decompression. </summary>
		/// <returns> the byte read, or -1 if end of compressed input is reached </returns>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws java.io.IOException
		public override int Read()
		{
			EnsureOpen();
			return Read(SingleByteBuf, 0, 1) == -1 ? - 1 : Byte.ToUnsignedInt(SingleByteBuf[0]);
		}

		/// <summary>
		/// Reads uncompressed data into an array of bytes. If <code>len</code> is not
		/// zero, the method will block until some input can be decompressed; otherwise,
		/// no bytes are read and <code>0</code> is returned. </summary>
		/// <param name="b"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset in the destination array <code>b</code> </param>
		/// <param name="len"> the maximum number of bytes read </param>
		/// <returns> the actual number of bytes read, or -1 if the end of the
		///         compressed input is reached or a preset dictionary is needed </returns>
		/// <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>b.length - off</code> </exception>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] b, int off, int len) throws java.io.IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			EnsureOpen();
			if (b == null)
			{
				throw new NullPointerException();
			}
			else if (off < 0 || len < 0 || len > b.Length - off)
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return 0;
			}
			try
			{
				int n;
				while ((n = Inf.Inflate(b, off, len)) == 0)
				{
					if (Inf.Finished() || Inf.NeedsDictionary())
					{
						ReachEOF = true;
						return -1;
					}
					if (Inf.NeedsInput())
					{
						Fill();
					}
				}
				return n;
			}
			catch (DataFormatException e)
			{
				String s = e.Message;
				throw new ZipException(s != null ? s : "Invalid ZLIB data format");
			}
		}

		/// <summary>
		/// Returns 0 after EOF has been reached, otherwise always return 1.
		/// <para>
		/// Programs should not count on this method to return the actual number
		/// of bytes that could be read without blocking.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     1 before EOF and 0 after EOF. </returns>
		/// <exception cref="IOException">  if an I/O error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws java.io.IOException
		public override int Available()
		{
			EnsureOpen();
			if (ReachEOF)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}

		private sbyte[] b = new sbyte[512];

		/// <summary>
		/// Skips specified number of bytes of uncompressed data. </summary>
		/// <param name="n"> the number of bytes to skip </param>
		/// <returns> the actual number of bytes skipped. </returns>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="IllegalArgumentException"> if {@code n < 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws java.io.IOException
		public override long Skip(long n)
		{
			if (n < 0)
			{
				throw new IllegalArgumentException("negative skip length");
			}
			EnsureOpen();
			int max = (int)System.Math.Min(n, Integer.MaxValue);
			int total = 0;
			while (total < max)
			{
				int len = max - total;
				if (len > b.Length)
				{
					len = b.Length;
				}
				len = Read(b, 0, len);
				if (len == -1)
				{
					ReachEOF = true;
					break;
				}
				total += len;
			}
			return total;
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
				if (UsesDefaultInflater)
				{
					Inf.End();
				}
				@in.Close();
				Closed = true;
			}
		}

		/// <summary>
		/// Fills input buffer with more data to decompress. </summary>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void fill() throws java.io.IOException
		protected internal virtual void Fill()
		{
			EnsureOpen();
			Len = @in.Read(Buf, 0, Buf.Length);
			if (Len == -1)
			{
				throw new EOFException("Unexpected end of ZLIB input stream");
			}
			Inf.SetInput(Buf, 0, Len);
		}

		/// <summary>
		/// Tests if this input stream supports the <code>mark</code> and
		/// <code>reset</code> methods. The <code>markSupported</code>
		/// method of <code>InflaterInputStream</code> returns
		/// <code>false</code>.
		/// </summary>
		/// <returns>  a <code>boolean</code> indicating if this stream type supports
		///          the <code>mark</code> and <code>reset</code> methods. </returns>
		/// <seealso cref=     java.io.InputStream#mark(int) </seealso>
		/// <seealso cref=     java.io.InputStream#reset() </seealso>
		public override bool MarkSupported()
		{
			return false;
		}

		/// <summary>
		/// Marks the current position in this input stream.
		/// 
		/// <para> The <code>mark</code> method of <code>InflaterInputStream</code>
		/// does nothing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="readlimit">   the maximum limit of bytes that can be read before
		///                      the mark position becomes invalid. </param>
		/// <seealso cref=     java.io.InputStream#reset() </seealso>
		public override void Mark(int readlimit)
		{
			lock (this)
			{
			}
		}

		/// <summary>
		/// Repositions this stream to the position at the time the
		/// <code>mark</code> method was last called on this input stream.
		/// 
		/// <para> The method <code>reset</code> for class
		/// <code>InflaterInputStream</code> does nothing except throw an
		/// <code>IOException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if this method is invoked. </exception>
		/// <seealso cref=     java.io.InputStream#mark(int) </seealso>
		/// <seealso cref=     java.io.IOException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void reset() throws java.io.IOException
		public override void Reset()
		{
			lock (this)
			{
				throw new IOException("mark/reset not supported");
			}
		}
	}

}