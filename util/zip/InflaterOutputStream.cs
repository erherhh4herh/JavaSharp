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
	/// Implements an output stream filter for uncompressing data stored in the
	/// "deflate" compression format.
	/// 
	/// @since       1.6
	/// @author      David R Tribble (david@tribble.com)
	/// </summary>
	/// <seealso cref= InflaterInputStream </seealso>
	/// <seealso cref= DeflaterInputStream </seealso>
	/// <seealso cref= DeflaterOutputStream </seealso>

	public class InflaterOutputStream : FilterOutputStream
	{
		/// <summary>
		/// Decompressor for this stream. </summary>
		protected internal readonly Inflater Inf;

		/// <summary>
		/// Output buffer for writing uncompressed data. </summary>
		protected internal readonly sbyte[] Buf;

		/// <summary>
		/// Temporary write buffer. </summary>
		private readonly sbyte[] Wbuf = new sbyte[1];

		/// <summary>
		/// Default decompressor is used. </summary>
		private bool UsesDefaultInflater = false;

		/// <summary>
		/// true iff <seealso cref="#close()"/> has been called. </summary>
		private bool Closed = false;

		/// <summary>
		/// Checks to make sure that this stream has not been closed.
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
		/// Creates a new output stream with a default decompressor and buffer
		/// size.
		/// </summary>
		/// <param name="out"> output stream to write the uncompressed data to </param>
		/// <exception cref="NullPointerException"> if {@code out} is null </exception>
		public InflaterOutputStream(OutputStream @out) : this(@out, new Inflater())
		{
			UsesDefaultInflater = true;
		}

		/// <summary>
		/// Creates a new output stream with the specified decompressor and a
		/// default buffer size.
		/// </summary>
		/// <param name="out"> output stream to write the uncompressed data to </param>
		/// <param name="infl"> decompressor ("inflater") for this stream </param>
		/// <exception cref="NullPointerException"> if {@code out} or {@code infl} is null </exception>
		public InflaterOutputStream(OutputStream @out, Inflater infl) : this(@out, infl, 512)
		{
		}

		/// <summary>
		/// Creates a new output stream with the specified decompressor and
		/// buffer size.
		/// </summary>
		/// <param name="out"> output stream to write the uncompressed data to </param>
		/// <param name="infl"> decompressor ("inflater") for this stream </param>
		/// <param name="bufLen"> decompression buffer size </param>
		/// <exception cref="IllegalArgumentException"> if {@code bufLen <= 0} </exception>
		/// <exception cref="NullPointerException"> if {@code out} or {@code infl} is null </exception>
		public InflaterOutputStream(OutputStream @out, Inflater infl, int bufLen) : base(@out)
		{

			// Sanity checks
			if (@out == null)
			{
				throw new NullPointerException("Null output");
			}
			if (infl == null)
			{
				throw new NullPointerException("Null inflater");
			}
			if (bufLen <= 0)
			{
				throw new IllegalArgumentException("Buffer size < 1");
			}

			// Initialize
			Inf = infl;
			Buf = new sbyte[bufLen];
		}

		/// <summary>
		/// Writes any remaining uncompressed data to the output stream and closes
		/// the underlying output stream.
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public override void Close()
		{
			if (!Closed)
			{
				// Complete the uncompressed output
				try
				{
					Finish();
				}
				finally
				{
					@out.Close();
					Closed = true;
				}
			}
		}

		/// <summary>
		/// Flushes this output stream, forcing any pending buffered output bytes to be
		/// written.
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs or this stream is already
		/// closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws java.io.IOException
		public override void Flush()
		{
			EnsureOpen();

			// Finish decompressing and writing pending output data
			if (!Inf.Finished())
			{
				try
				{
					while (!Inf.Finished() && !Inf.NeedsInput())
					{
						int n;

						// Decompress pending output data
						n = Inf.Inflate(Buf, 0, Buf.Length);
						if (n < 1)
						{
							break;
						}

						// Write the uncompressed output data block
						@out.Write(Buf, 0, n);
					}
					base.Flush();
				}
				catch (DataFormatException ex)
				{
					// Improperly formatted compressed (ZIP) data
					String msg = ex.Message;
					if (msg == null)
					{
						msg = "Invalid ZLIB data format";
					}
					throw new ZipException(msg);
				}
			}
		}

		/// <summary>
		/// Finishes writing uncompressed data to the output stream without closing
		/// the underlying stream.  Use this method when applying multiple filters in
		/// succession to the same output stream.
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs or this stream is already
		/// closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finish() throws java.io.IOException
		public virtual void Finish()
		{
			EnsureOpen();

			// Finish decompressing and writing pending output data
			Flush();
			if (UsesDefaultInflater)
			{
				Inf.End();
			}
		}

		/// <summary>
		/// Writes a byte to the uncompressed output stream.
		/// </summary>
		/// <param name="b"> a single byte of compressed data to decompress and write to
		/// the output stream </param>
		/// <exception cref="IOException"> if an I/O error occurs or this stream is already
		/// closed </exception>
		/// <exception cref="ZipException"> if a compression (ZIP) format error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws java.io.IOException
		public override void Write(int b)
		{
			// Write a single byte of data
			Wbuf[0] = (sbyte) b;
			Write(Wbuf, 0, 1);
		}

		/// <summary>
		/// Writes an array of bytes to the uncompressed output stream.
		/// </summary>
		/// <param name="b"> buffer containing compressed data to decompress and write to
		/// the output stream </param>
		/// <param name="off"> starting offset of the compressed data within {@code b} </param>
		/// <param name="len"> number of bytes to decompress from {@code b} </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code off < 0}, or if
		/// {@code len < 0}, or if {@code len > b.length - off} </exception>
		/// <exception cref="IOException"> if an I/O error occurs or this stream is already
		/// closed </exception>
		/// <exception cref="NullPointerException"> if {@code b} is null </exception>
		/// <exception cref="ZipException"> if a compression (ZIP) format error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte[] b, int off, int len) throws java.io.IOException
		public override void Write(sbyte[] b, int off, int len)
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
				return;
			}

			// Write uncompressed data to the output stream
			try
			{
				for (;;)
				{
					int n;

					// Fill the decompressor buffer with output data
					if (Inf.NeedsInput())
					{
						int part;

						if (len < 1)
						{
							break;
						}

						part = (len < 512 ? len : 512);
						Inf.SetInput(b, off, part);
						off += part;
						len -= part;
					}

					// Decompress and write blocks of output data
					do
					{
						n = Inf.Inflate(Buf, 0, Buf.Length);
						if (n > 0)
						{
							@out.Write(Buf, 0, n);
						}
					} while (n > 0);

					// Check the decompressor
					if (Inf.Finished())
					{
						break;
					}
					if (Inf.NeedsDictionary())
					{
						throw new ZipException("ZLIB dictionary missing");
					}
				}
			}
			catch (DataFormatException ex)
			{
				// Improperly formatted compressed (ZIP) data
				String msg = ex.Message;
				if (msg == null)
				{
					msg = "Invalid ZLIB data format";
				}
				throw new ZipException(msg);
			}
		}
	}

}