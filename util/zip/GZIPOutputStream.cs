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
	/// This class implements a stream filter for writing compressed data in
	/// the GZIP file format.
	/// @author      David Connelly
	/// 
	/// </summary>
	public class GZIPOutputStream : DeflaterOutputStream
	{
		/// <summary>
		/// CRC-32 of uncompressed data.
		/// </summary>
		protected internal CRC32 Crc = new CRC32();

		/*
		 * GZIP header magic number.
		 */
		private const int GZIP_MAGIC = 0x8b1f;

		/*
		 * Trailer size in bytes.
		 *
		 */
		private const int TRAILER_SIZE = 8;

		/// <summary>
		/// Creates a new output stream with the specified buffer size.
		/// 
		/// <para>The new output stream instance is created as if by invoking
		/// the 3-argument constructor GZIPOutputStream(out, size, false).
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <param name="size"> the output buffer size </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GZIPOutputStream(java.io.OutputStream out, int size) throws java.io.IOException
		public GZIPOutputStream(OutputStream @out, int size) : this(@out, size, false)
		{
		}

		/// <summary>
		/// Creates a new output stream with the specified buffer size and
		/// flush mode.
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <param name="size"> the output buffer size </param>
		/// <param name="syncFlush">
		///        if {@code true} invocation of the inherited
		///        <seealso cref="DeflaterOutputStream#flush() flush()"/> method of
		///        this instance flushes the compressor with flush mode
		///        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		///        stream, otherwise only flushes the output stream </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code size <= 0}
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GZIPOutputStream(java.io.OutputStream out, int size, boolean syncFlush) throws java.io.IOException
		public GZIPOutputStream(OutputStream @out, int size, bool syncFlush) : base(@out, new Deflater(Deflater.DEFAULT_COMPRESSION, true), size, syncFlush)
		{
			UsesDefaultDeflater = true;
			WriteHeader();
			Crc.Reset();
		}


		/// <summary>
		/// Creates a new output stream with a default buffer size.
		/// 
		/// <para>The new output stream instance is created as if by invoking
		/// the 2-argument constructor GZIPOutputStream(out, false).
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GZIPOutputStream(java.io.OutputStream out) throws java.io.IOException
		public GZIPOutputStream(OutputStream @out) : this(@out, 512, false)
		{
		}

		/// <summary>
		/// Creates a new output stream with a default buffer size and
		/// the specified flush mode.
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <param name="syncFlush">
		///        if {@code true} invocation of the inherited
		///        <seealso cref="DeflaterOutputStream#flush() flush()"/> method of
		///        this instance flushes the compressor with flush mode
		///        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		///        stream, otherwise only flushes the output stream
		/// </param>
		/// <exception cref="IOException"> If an I/O error has occurred.
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GZIPOutputStream(java.io.OutputStream out, boolean syncFlush) throws java.io.IOException
		public GZIPOutputStream(OutputStream @out, bool syncFlush) : this(@out, 512, syncFlush)
		{
		}

		/// <summary>
		/// Writes array of bytes to the compressed output stream. This method
		/// will block until all the bytes are written. </summary>
		/// <param name="buf"> the data to be written </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the length of the data </param>
		/// <exception cref="IOException"> If an I/O error has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void write(byte[] buf, int off, int len) throws java.io.IOException
		public override void Write(sbyte[] buf, int off, int len)
		{
			lock (this)
			{
				base.Write(buf, off, len);
				Crc.Update(buf, off, len);
			}
		}

		/// <summary>
		/// Finishes writing compressed data to the output stream without closing
		/// the underlying stream. Use this method when applying multiple filters
		/// in succession to the same output stream. </summary>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finish() throws java.io.IOException
		public override void Finish()
		{
			if (!Def.Finished())
			{
				Def.Finish();
				while (!Def.Finished())
				{
					int len = Def.Deflate(Buf, 0, Buf.Length);
					if (Def.Finished() && len <= Buf.Length - TRAILER_SIZE)
					{
						// last deflater buffer. Fit trailer at the end
						WriteTrailer(Buf, len);
						len = len + TRAILER_SIZE;
						@out.Write(Buf, 0, len);
						return;
					}
					if (len > 0)
					{
						@out.Write(Buf, 0, len);
					}
				}
				// if we can't fit the trailer at the end of the last
				// deflater buffer, we write it separately
				sbyte[] trailer = new sbyte[TRAILER_SIZE];
				WriteTrailer(trailer, 0);
				@out.Write(trailer);
			}
		}

		/*
		 * Writes GZIP member header.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeHeader() throws java.io.IOException
		private void WriteHeader()
		{
			@out.Write(new sbyte[] {(sbyte) GZIP_MAGIC, (sbyte)(GZIP_MAGIC >> 8), Deflater.DEFLATED, 0, 0, 0, 0, 0, 0, 0}); // Operating system (OS) -  Extra flags (XFLG) -  Modification time MTIME (int) -  Modification time MTIME (int) -  Modification time MTIME (int) -  Modification time MTIME (int) -  Flags (FLG) -  Compression method (CM) -  Magic number (short) -  Magic number (short)
		}

		/*
		 * Writes GZIP member trailer to a byte array, starting at a given
		 * offset.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeTrailer(byte[] buf, int offset) throws java.io.IOException
		private void WriteTrailer(sbyte[] buf, int offset)
		{
			WriteInt((int)Crc.Value, buf, offset); // CRC-32 of uncompr. data
			WriteInt(Def.TotalIn, buf, offset + 4); // Number of uncompr. bytes
		}

		/*
		 * Writes integer in Intel byte order to a byte array, starting at a
		 * given offset.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeInt(int i, byte[] buf, int offset) throws java.io.IOException
		private void WriteInt(int i, sbyte[] buf, int offset)
		{
			WriteShort(i & 0xffff, buf, offset);
			WriteShort((i >> 16) & 0xffff, buf, offset + 2);
		}

		/*
		 * Writes short integer in Intel byte order to a byte array, starting
		 * at a given offset
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeShort(int s, byte[] buf, int offset) throws java.io.IOException
		private void WriteShort(int s, sbyte[] buf, int offset)
		{
			buf[offset] = unchecked((sbyte)(s & 0xff));
			buf[offset + 1] = unchecked((sbyte)((s >> 8) & 0xff));
		}
	}

}