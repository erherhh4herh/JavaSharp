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
	/// This class implements an output stream filter for compressing data in
	/// the "deflate" compression format. It is also used as the basis for other
	/// types of compression filters, such as GZIPOutputStream.
	/// </summary>
	/// <seealso cref=         Deflater
	/// @author      David Connelly </seealso>
	public class DeflaterOutputStream : FilterOutputStream
	{
		/// <summary>
		/// Compressor for this stream.
		/// </summary>
		protected internal Deflater Def;

		/// <summary>
		/// Output buffer for writing compressed data.
		/// </summary>
		protected internal sbyte[] Buf;

		/// <summary>
		/// Indicates that the stream has been closed.
		/// </summary>

		private bool Closed = false;

		private readonly bool SyncFlush;

		/// <summary>
		/// Creates a new output stream with the specified compressor,
		/// buffer size and flush mode.
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <param name="def"> the compressor ("deflater") </param>
		/// <param name="size"> the output buffer size </param>
		/// <param name="syncFlush">
		///        if {@code true} the <seealso cref="#flush()"/> method of this
		///        instance flushes the compressor with flush mode
		///        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		///        stream, otherwise only flushes the output stream
		/// </param>
		/// <exception cref="IllegalArgumentException"> if {@code size <= 0}
		/// 
		/// @since 1.7 </exception>
		public DeflaterOutputStream(OutputStream @out, Deflater def, int size, bool syncFlush) : base(@out)
		{
			if (@out == null || def == null)
			{
				throw new NullPointerException();
			}
			else if (size <= 0)
			{
				throw new IllegalArgumentException("buffer size <= 0");
			}
			this.Def = def;
			this.Buf = new sbyte[size];
			this.SyncFlush = syncFlush;
		}


		/// <summary>
		/// Creates a new output stream with the specified compressor and
		/// buffer size.
		/// 
		/// <para>The new output stream instance is created as if by invoking
		/// the 4-argument constructor DeflaterOutputStream(out, def, size, false).
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <param name="def"> the compressor ("deflater") </param>
		/// <param name="size"> the output buffer size </param>
		/// <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
		public DeflaterOutputStream(OutputStream @out, Deflater def, int size) : this(@out, def, size, false)
		{
		}

		/// <summary>
		/// Creates a new output stream with the specified compressor, flush
		/// mode and a default buffer size.
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <param name="def"> the compressor ("deflater") </param>
		/// <param name="syncFlush">
		///        if {@code true} the <seealso cref="#flush()"/> method of this
		///        instance flushes the compressor with flush mode
		///        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		///        stream, otherwise only flushes the output stream
		/// 
		/// @since 1.7 </param>
		public DeflaterOutputStream(OutputStream @out, Deflater def, bool syncFlush) : this(@out, def, 512, syncFlush)
		{
		}


		/// <summary>
		/// Creates a new output stream with the specified compressor and
		/// a default buffer size.
		/// 
		/// <para>The new output stream instance is created as if by invoking
		/// the 3-argument constructor DeflaterOutputStream(out, def, false).
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <param name="def"> the compressor ("deflater") </param>
		public DeflaterOutputStream(OutputStream @out, Deflater def) : this(@out, def, 512, false)
		{
		}

		internal bool UsesDefaultDeflater = false;


		/// <summary>
		/// Creates a new output stream with a default compressor, a default
		/// buffer size and the specified flush mode.
		/// </summary>
		/// <param name="out"> the output stream </param>
		/// <param name="syncFlush">
		///        if {@code true} the <seealso cref="#flush()"/> method of this
		///        instance flushes the compressor with flush mode
		///        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		///        stream, otherwise only flushes the output stream
		/// 
		/// @since 1.7 </param>
		public DeflaterOutputStream(OutputStream @out, bool syncFlush) : this(@out, new Deflater(), 512, syncFlush)
		{
			UsesDefaultDeflater = true;
		}

		/// <summary>
		/// Creates a new output stream with a default compressor and buffer size.
		/// 
		/// <para>The new output stream instance is created as if by invoking
		/// the 2-argument constructor DeflaterOutputStream(out, false).
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> the output stream </param>
		public DeflaterOutputStream(OutputStream @out) : this(@out, false)
		{
			UsesDefaultDeflater = true;
		}

		/// <summary>
		/// Writes a byte to the compressed output stream. This method will
		/// block until the byte can be written. </summary>
		/// <param name="b"> the byte to be written </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws java.io.IOException
		public override void Write(int b)
		{
			sbyte[] buf = new sbyte[1];
			buf[0] = unchecked((sbyte)(b & 0xff));
			Write(buf, 0, 1);
		}

		/// <summary>
		/// Writes an array of bytes to the compressed output stream. This
		/// method will block until all the bytes are written. </summary>
		/// <param name="b"> the data to be written </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the length of the data </param>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte[] b, int off, int len) throws java.io.IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			if (Def.Finished())
			{
				throw new IOException("write beyond end of stream");
			}
			if ((off | len | (off + len) | (b.Length - (off + len))) < 0)
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return;
			}
			if (!Def.Finished())
			{
				Def.SetInput(b, off, len);
				while (!Def.NeedsInput())
				{
					Deflate();
				}
			}
		}

		/// <summary>
		/// Finishes writing compressed data to the output stream without closing
		/// the underlying stream. Use this method when applying multiple filters
		/// in succession to the same output stream. </summary>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finish() throws java.io.IOException
		public virtual void Finish()
		{
			if (!Def.Finished())
			{
				Def.Finish();
				while (!Def.Finished())
				{
					Deflate();
				}
			}
		}

		/// <summary>
		/// Writes remaining compressed data to the output stream and closes the
		/// underlying stream. </summary>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public override void Close()
		{
			if (!Closed)
			{
				Finish();
				if (UsesDefaultDeflater)
				{
					Def.End();
				}
				@out.Close();
				Closed = true;
			}
		}

		/// <summary>
		/// Writes next block of compressed data to the output stream. </summary>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void deflate() throws java.io.IOException
		protected internal virtual void Deflate()
		{
			int len = Def.Deflate(Buf, 0, Buf.Length);
			if (len > 0)
			{
				@out.Write(Buf, 0, len);
			}
		}

		/// <summary>
		/// Flushes the compressed output stream.
		/// 
		/// If {@link #DeflaterOutputStream(OutputStream, Deflater, int, boolean)
		/// syncFlush} is {@code true} when this compressed output stream is
		/// constructed, this method first flushes the underlying {@code compressor}
		/// with the flush mode <seealso cref="Deflater#SYNC_FLUSH"/> to force
		/// all pending data to be flushed out to the output stream and then
		/// flushes the output stream. Otherwise this method only flushes the
		/// output stream without flushing the {@code compressor}.
		/// </summary>
		/// <exception cref="IOException"> if an I/O error has occurred
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws java.io.IOException
		public override void Flush()
		{
			if (SyncFlush && !Def.Finished())
			{
				int len = 0;
				while ((len = Def.Deflate(Buf, 0, Buf.Length, Deflater.SYNC_FLUSH)) > 0)
				{
					@out.Write(Buf, 0, len);
					if (len < Buf.Length)
					{
						break;
					}
				}
			}
			@out.Flush();
		}
	}

}