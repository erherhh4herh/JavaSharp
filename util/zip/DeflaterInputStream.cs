using System;

/*
 * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Implements an input stream filter for compressing data in the "deflate"
	/// compression format.
	/// 
	/// @since       1.6
	/// @author      David R Tribble (david@tribble.com)
	/// </summary>
	/// <seealso cref= DeflaterOutputStream </seealso>
	/// <seealso cref= InflaterOutputStream </seealso>
	/// <seealso cref= InflaterInputStream </seealso>

	public class DeflaterInputStream : FilterInputStream
	{
		/// <summary>
		/// Compressor for this stream. </summary>
		protected internal readonly Deflater Def;

		/// <summary>
		/// Input buffer for reading compressed data. </summary>
		protected internal readonly sbyte[] Buf;

		/// <summary>
		/// Temporary read buffer. </summary>
		private sbyte[] Rbuf = new sbyte[1];

		/// <summary>
		/// Default compressor is used. </summary>
		private bool UsesDefaultDeflater = false;

		/// <summary>
		/// End of the underlying input stream has been reached. </summary>
		private bool ReachEOF = false;

		/// <summary>
		/// Check to make sure that this stream has not been closed.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureOpen() throws java.io.IOException
		private void EnsureOpen()
		{
			if (@in == null)
			{
				throw new IOException("Stream closed");
			}
		}

		/// <summary>
		/// Creates a new input stream with a default compressor and buffer
		/// size.
		/// </summary>
		/// <param name="in"> input stream to read the uncompressed data to </param>
		/// <exception cref="NullPointerException"> if {@code in} is null </exception>
		public DeflaterInputStream(InputStream @in) : this(@in, new Deflater())
		{
			UsesDefaultDeflater = true;
		}

		/// <summary>
		/// Creates a new input stream with the specified compressor and a
		/// default buffer size.
		/// </summary>
		/// <param name="in"> input stream to read the uncompressed data to </param>
		/// <param name="defl"> compressor ("deflater") for this stream </param>
		/// <exception cref="NullPointerException"> if {@code in} or {@code defl} is null </exception>
		public DeflaterInputStream(InputStream @in, Deflater defl) : this(@in, defl, 512)
		{
		}

		/// <summary>
		/// Creates a new input stream with the specified compressor and buffer
		/// size.
		/// </summary>
		/// <param name="in"> input stream to read the uncompressed data to </param>
		/// <param name="defl"> compressor ("deflater") for this stream </param>
		/// <param name="bufLen"> compression buffer size </param>
		/// <exception cref="IllegalArgumentException"> if {@code bufLen <= 0} </exception>
		/// <exception cref="NullPointerException"> if {@code in} or {@code defl} is null </exception>
		public DeflaterInputStream(InputStream @in, Deflater defl, int bufLen) : base(@in)
		{

			// Sanity checks
			if (@in == null)
			{
				throw new NullPointerException("Null input");
			}
			if (defl == null)
			{
				throw new NullPointerException("Null deflater");
			}
			if (bufLen < 1)
			{
				throw new IllegalArgumentException("Buffer size < 1");
			}

			// Initialize
			Def = defl;
			Buf = new sbyte[bufLen];
		}

		/// <summary>
		/// Closes this input stream and its underlying input stream, discarding
		/// any pending uncompressed data.
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public override void Close()
		{
			if (@in != null)
			{
				try
				{
					// Clean up
					if (UsesDefaultDeflater)
					{
						Def.End();
					}

					@in.Close();
				}
				finally
				{
					@in = null;
				}
			}
		}

		/// <summary>
		/// Reads a single byte of compressed data from the input stream.
		/// This method will block until some input can be read and compressed.
		/// </summary>
		/// <returns> a single byte of compressed data, or -1 if the end of the
		/// uncompressed input stream is reached </returns>
		/// <exception cref="IOException"> if an I/O error occurs or if this stream is
		/// already closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws java.io.IOException
		public override int Read()
		{
			// Read a single byte of compressed data
			int len = Read(Rbuf, 0, 1);
			if (len <= 0)
			{
				return -1;
			}
			return (Rbuf[0] & 0xFF);
		}

		/// <summary>
		/// Reads compressed data into a byte array.
		/// This method will block until some input can be read and compressed.
		/// </summary>
		/// <param name="b"> buffer into which the data is read </param>
		/// <param name="off"> starting offset of the data within {@code b} </param>
		/// <param name="len"> maximum number of compressed bytes to read into {@code b} </param>
		/// <returns> the actual number of bytes read, or -1 if the end of the
		/// uncompressed input stream is reached </returns>
		/// <exception cref="IndexOutOfBoundsException">  if {@code len > b.length - off} </exception>
		/// <exception cref="IOException"> if an I/O error occurs or if this input stream is
		/// already closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] b, int off, int len) throws java.io.IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			// Sanity checks
			EnsureOpen();
			if (b == null)
			{
				throw new NullPointerException("Null buffer for read");
			}
			else if (off < 0 || len < 0 || len > b.Length - off)
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return 0;
			}

			// Read and compress (deflate) input data bytes
			int cnt = 0;
			while (len > 0 && !Def.Finished())
			{
				int n;

				// Read data from the input stream
				if (Def.NeedsInput())
				{
					n = @in.Read(Buf, 0, Buf.Length);
					if (n < 0)
					{
						// End of the input stream reached
						Def.Finish();
					}
					else if (n > 0)
					{
						Def.SetInput(Buf, 0, n);
					}
				}

				// Compress the input data, filling the read buffer
				n = Def.Deflate(b, off, len);
				cnt += n;
				off += n;
				len -= n;
			}
			if (cnt == 0 && Def.Finished())
			{
				ReachEOF = true;
				cnt = -1;
			}

			return cnt;
		}

		/// <summary>
		/// Skips over and discards data from the input stream.
		/// This method may block until the specified number of bytes are read and
		/// skipped. <em>Note:</em> While {@code n} is given as a {@code long},
		/// the maximum number of bytes which can be skipped is
		/// {@code Integer.MAX_VALUE}.
		/// </summary>
		/// <param name="n"> number of bytes to be skipped </param>
		/// <returns> the actual number of bytes skipped </returns>
		/// <exception cref="IOException"> if an I/O error occurs or if this stream is
		/// already closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws java.io.IOException
		public override long Skip(long n)
		{
			if (n < 0)
			{
				throw new IllegalArgumentException("negative skip length");
			}
			EnsureOpen();

			// Skip bytes by repeatedly decompressing small blocks
			if (Rbuf.Length < 512)
			{
				Rbuf = new sbyte[512];
			}

			int total = (int)System.Math.Min(n, Integer.MaxValue);
			long cnt = 0;
			while (total > 0)
			{
				// Read a small block of uncompressed bytes
				int len = Read(Rbuf, 0, (total <= Rbuf.Length ? total : Rbuf.Length));

				if (len < 0)
				{
					break;
				}
				cnt += len;
				total -= len;
			}
			return cnt;
		}

		/// <summary>
		/// Returns 0 after EOF has been reached, otherwise always return 1.
		/// <para>
		/// Programs should not count on this method to return the actual number
		/// of bytes that could be read without blocking
		/// </para>
		/// </summary>
		/// <returns> zero after the end of the underlying input stream has been
		/// reached, otherwise always returns 1 </returns>
		/// <exception cref="IOException"> if an I/O error occurs or if this stream is
		/// already closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws java.io.IOException
		public override int Available()
		{
			EnsureOpen();
			if (ReachEOF)
			{
				return 0;
			}
			return 1;
		}

		/// <summary>
		/// Always returns {@code false} because this input stream does not support
		/// the <seealso cref="#mark mark()"/> and <seealso cref="#reset reset()"/> methods.
		/// </summary>
		/// <returns> false, always </returns>
		public override bool MarkSupported()
		{
			return false;
		}

		/// <summary>
		/// <i>This operation is not supported</i>.
		/// </summary>
		/// <param name="limit"> maximum bytes that can be read before invalidating the position marker </param>
		public override void Mark(int limit)
		{
			// Operation not supported
		}

		/// <summary>
		/// <i>This operation is not supported</i>.
		/// </summary>
		/// <exception cref="IOException"> always thrown </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws java.io.IOException
		public override void Reset()
		{
			throw new IOException("mark/reset not supported");
		}
	}

}