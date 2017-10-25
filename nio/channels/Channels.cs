using System;
using System.Threading;

/*
 * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels
{

	using ChannelInputStream = sun.nio.ch.ChannelInputStream;
	using StreamDecoder = sun.nio.cs.StreamDecoder;
	using StreamEncoder = sun.nio.cs.StreamEncoder;


	/// <summary>
	/// Utility methods for channels and streams.
	/// 
	/// <para> This class defines static methods that support the interoperation of the
	/// stream classes of the <tt><seealso cref="java.io"/></tt> package with the channel
	/// classes of this package.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author Mike McCloskey
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public sealed class Channels
	{

		private Channels() // No instantiation
		{
		}

		private static void CheckNotNull(Object o, String name)
		{
			if (o == null)
			{
				throw new NullPointerException("\"" + name + "\" is null!");
			}
		}

		/// <summary>
		/// Write all remaining bytes in buffer to the given channel.
		/// If the channel is selectable then it must be configured blocking.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void writeFullyImpl(WritableByteChannel ch, java.nio.ByteBuffer bb) throws java.io.IOException
		private static void WriteFullyImpl(WritableByteChannel ch, ByteBuffer bb)
		{
			while (bb.Remaining() > 0)
			{
				int n = ch.Write(bb);
				if (n <= 0)
				{
					throw new RuntimeException("no bytes written");
				}
			}
		}

		/// <summary>
		/// Write all remaining bytes in buffer to the given channel.
		/// </summary>
		/// <exception cref="IllegalBlockingModeException">
		///          If the channel is selectable and configured non-blocking. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void writeFully(WritableByteChannel ch, java.nio.ByteBuffer bb) throws java.io.IOException
		private static void WriteFully(WritableByteChannel ch, ByteBuffer bb)
		{
			if (ch is SelectableChannel)
			{
				SelectableChannel sc = (SelectableChannel)ch;
				lock (sc.BlockingLock())
				{
					if (!sc.Blocking)
					{
						throw new IllegalBlockingModeException();
					}
					WriteFullyImpl(ch, bb);
				}
			}
			else
			{
				WriteFullyImpl(ch, bb);
			}
		}

		// -- Byte streams from channels --

		/// <summary>
		/// Constructs a stream that reads bytes from the given channel.
		/// 
		/// <para> The <tt>read</tt> methods of the resulting stream will throw an
		/// <seealso cref="IllegalBlockingModeException"/> if invoked while the underlying
		/// channel is in non-blocking mode.  The stream will not be buffered, and
		/// it will not support the <seealso cref="InputStream#mark mark"/> or {@link
		/// InputStream#reset reset} methods.  The stream will be safe for access by
		/// multiple concurrent threads.  Closing the stream will in turn cause the
		/// channel to be closed.  </para>
		/// </summary>
		/// <param name="ch">
		///         The channel from which bytes will be read
		/// </param>
		/// <returns>  A new input stream </returns>
		public static InputStream NewInputStream(ReadableByteChannel ch)
		{
			CheckNotNull(ch, "ch");
			return new ChannelInputStream(ch);
		}

		/// <summary>
		/// Constructs a stream that writes bytes to the given channel.
		/// 
		/// <para> The <tt>write</tt> methods of the resulting stream will throw an
		/// <seealso cref="IllegalBlockingModeException"/> if invoked while the underlying
		/// channel is in non-blocking mode.  The stream will not be buffered.  The
		/// stream will be safe for access by multiple concurrent threads.  Closing
		/// the stream will in turn cause the channel to be closed.  </para>
		/// </summary>
		/// <param name="ch">
		///         The channel to which bytes will be written
		/// </param>
		/// <returns>  A new output stream </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static java.io.OutputStream newOutputStream(final WritableByteChannel ch)
		public static OutputStream NewOutputStream(WritableByteChannel ch)
		{
			CheckNotNull(ch, "ch");

			return new OutputStreamAnonymousInnerClassHelper(ch);
		}

		private class OutputStreamAnonymousInnerClassHelper : OutputStream
		{
			private java.nio.channels.WritableByteChannel Ch;

			public OutputStreamAnonymousInnerClassHelper(java.nio.channels.WritableByteChannel ch)
			{
				this.Ch = ch;
			}


			private ByteBuffer bb = null;
			private sbyte[] bs = null; // Invoker's previous array
			private sbyte[] b1 = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void write(int b) throws java.io.IOException
			public override void Write(int b)
			{
				lock (this)
				{
				   if (b1 == null)
				   {
						b1 = new sbyte[1];
				   }
					b1[0] = (sbyte)b;
					this.write(b1);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void write(byte[] bs, int off, int len) throws java.io.IOException
			public override void Write(sbyte[] bs, int off, int len)
			{
				lock (this)
				{
					if ((off < 0) || (off > bs.Length) || (len < 0) || ((off + len) > bs.Length) || ((off + len) < 0))
					{
						throw new IndexOutOfBoundsException();
					}
					else if (len == 0)
					{
						return;
					}
					ByteBuffer bb = ((this.bs == bs) ? this.bb : ByteBuffer.Wrap(bs));
					bb.Limit(System.Math.Min(off + len, bb.Capacity()));
					bb.Position(off);
					this.bb = bb;
					this.bs = bs;
					Channels.WriteFully(Ch, bb);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
			public override void Close()
			{
				Ch.Close();
			}

		}

		/// <summary>
		/// Constructs a stream that reads bytes from the given channel.
		/// 
		/// <para> The stream will not be buffered, and it will not support the {@link
		/// InputStream#mark mark} or <seealso cref="InputStream#reset reset"/> methods.  The
		/// stream will be safe for access by multiple concurrent threads.  Closing
		/// the stream will in turn cause the channel to be closed.  </para>
		/// </summary>
		/// <param name="ch">
		///         The channel from which bytes will be read
		/// </param>
		/// <returns>  A new input stream
		/// 
		/// @since 1.7 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static java.io.InputStream newInputStream(final AsynchronousByteChannel ch)
		public static InputStream NewInputStream(AsynchronousByteChannel ch)
		{
			CheckNotNull(ch, "ch");
			return new InputStreamAnonymousInnerClassHelper(ch);
		}

		private class InputStreamAnonymousInnerClassHelper : InputStream
		{
			private java.nio.channels.AsynchronousByteChannel Ch;

			public InputStreamAnonymousInnerClassHelper(java.nio.channels.AsynchronousByteChannel ch)
			{
				this.Ch = ch;
			}


			private ByteBuffer bb = null;
			private sbyte[] bs = null; // Invoker's previous array
			private sbyte[] b1 = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized int read() throws java.io.IOException
			public override int Read()
			{
				lock (this)
				{
					if (b1 == null)
					{
						b1 = new sbyte[1];
					}
					int n = this.read(b1);
					if (n == 1)
					{
						return b1[0] & 0xff;
					}
					return -1;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized int read(byte[] bs, int off, int len) throws java.io.IOException
			public override int Read(sbyte[] bs, int off, int len)
			{
				lock (this)
				{
					if ((off < 0) || (off > bs.Length) || (len < 0) || ((off + len) > bs.Length) || ((off + len) < 0))
					{
						throw new IndexOutOfBoundsException();
					}
					else if (len == 0)
					{
						return 0;
					}
        
					ByteBuffer bb = ((this.bs == bs) ? this.bb : ByteBuffer.Wrap(bs));
					bb.Position(off);
					bb.Limit(System.Math.Min(off + len, bb.Capacity()));
					this.bb = bb;
					this.bs = bs;
        
					bool interrupted = false;
					try
					{
						for (;;)
						{
							try
							{
								return Ch.Read(bb).Get();
							}
							catch (ExecutionException ee)
							{
								throw new IOException(ee.InnerException);
							}
							catch (InterruptedException)
							{
								interrupted = true;
							}
						}
					}
					finally
					{
						if (interrupted)
						{
							Thread.CurrentThread.Interrupt();
						}
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void close() throws java.io.IOException
			public override void Close()
			{
				Ch.Close();
			}
		}

		/// <summary>
		/// Constructs a stream that writes bytes to the given channel.
		/// 
		/// <para> The stream will not be buffered. The stream will be safe for access
		/// by multiple concurrent threads.  Closing the stream will in turn cause
		/// the channel to be closed.  </para>
		/// </summary>
		/// <param name="ch">
		///         The channel to which bytes will be written
		/// </param>
		/// <returns>  A new output stream
		/// 
		/// @since 1.7 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static java.io.OutputStream newOutputStream(final AsynchronousByteChannel ch)
		public static OutputStream NewOutputStream(AsynchronousByteChannel ch)
		{
			CheckNotNull(ch, "ch");
			return new OutputStreamAnonymousInnerClassHelper2(ch);
		}

		private class OutputStreamAnonymousInnerClassHelper2 : OutputStream
		{
			private java.nio.channels.AsynchronousByteChannel Ch;

			public OutputStreamAnonymousInnerClassHelper2(java.nio.channels.AsynchronousByteChannel ch)
			{
				this.Ch = ch;
			}


			private ByteBuffer bb = null;
			private sbyte[] bs = null; // Invoker's previous array
			private sbyte[] b1 = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void write(int b) throws java.io.IOException
			public override void Write(int b)
			{
				lock (this)
				{
				   if (b1 == null)
				   {
						b1 = new sbyte[1];
				   }
					b1[0] = (sbyte)b;
					this.write(b1);
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void write(byte[] bs, int off, int len) throws java.io.IOException
			public override void Write(sbyte[] bs, int off, int len)
			{
				lock (this)
				{
					if ((off < 0) || (off > bs.Length) || (len < 0) || ((off + len) > bs.Length) || ((off + len) < 0))
					{
						throw new IndexOutOfBoundsException();
					}
					else if (len == 0)
					{
						return;
					}
					ByteBuffer bb = ((this.bs == bs) ? this.bb : ByteBuffer.Wrap(bs));
					bb.Limit(System.Math.Min(off + len, bb.Capacity()));
					bb.Position(off);
					this.bb = bb;
					this.bs = bs;
        
					bool interrupted = false;
					try
					{
						while (bb.Remaining() > 0)
						{
							try
							{
								Ch.Write(bb).Get();
							}
							catch (ExecutionException ee)
							{
								throw new IOException(ee.InnerException);
							}
							catch (InterruptedException)
							{
								interrupted = true;
							}
						}
					}
					finally
					{
						if (interrupted)
						{
							Thread.CurrentThread.Interrupt();
						}
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void close() throws java.io.IOException
			public override void Close()
			{
				Ch.Close();
			}
		}


		// -- Channels from streams --

		/// <summary>
		/// Constructs a channel that reads bytes from the given stream.
		/// 
		/// <para> The resulting channel will not be buffered; it will simply redirect
		/// its I/O operations to the given stream.  Closing the channel will in
		/// turn cause the stream to be closed.  </para>
		/// </summary>
		/// <param name="in">
		///         The stream from which bytes are to be read
		/// </param>
		/// <returns>  A new readable byte channel </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static ReadableByteChannel newChannel(final java.io.InputStream in)
		public static ReadableByteChannel NewChannel(InputStream @in)
		{
			CheckNotNull(@in, "in");

			if (@in is FileInputStream && typeof(FileInputStream).Equals(@in.GetType()))
			{
				return ((FileInputStream)@in).Channel;
			}

			return new ReadableByteChannelImpl(@in);
		}

		private class ReadableByteChannelImpl : AbstractInterruptibleChannel, ReadableByteChannel // Not really interruptible
		{
			internal InputStream @in;
			internal const int TRANSFER_SIZE = 8192;
			internal sbyte[] Buf = new sbyte[0];
			internal bool Open = true;
			internal Object ReadLock = new Object();

			internal ReadableByteChannelImpl(InputStream @in)
			{
				this.@in = @in;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(java.nio.ByteBuffer dst) throws java.io.IOException
			public virtual int Read(ByteBuffer dst)
			{
				int len = dst.Remaining();
				int totalRead = 0;
				int bytesRead = 0;
				lock (ReadLock)
				{
					while (totalRead < len)
					{
						int bytesToRead = System.Math.Min((len - totalRead), TRANSFER_SIZE);
						if (Buf.Length < bytesToRead)
						{
							Buf = new sbyte[bytesToRead];
						}
						if ((totalRead > 0) && !(@in.Available() > 0))
						{
							break; // block at most once
						}
						try
						{
							Begin();
							bytesRead = @in.Read(Buf, 0, bytesToRead);
						}
						finally
						{
							End(bytesRead > 0);
						}
						if (bytesRead < 0)
						{
							break;
						}
						else
						{
							totalRead += bytesRead;
						}
						dst.Put(Buf, 0, bytesRead);
					}
					if ((bytesRead < 0) && (totalRead == 0))
					{
						return -1;
					}

					return totalRead;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void implCloseChannel() throws java.io.IOException
			protected internal override void ImplCloseChannel()
			{
				@in.Close();
				Open = false;
			}
		}


		/// <summary>
		/// Constructs a channel that writes bytes to the given stream.
		/// 
		/// <para> The resulting channel will not be buffered; it will simply redirect
		/// its I/O operations to the given stream.  Closing the channel will in
		/// turn cause the stream to be closed.  </para>
		/// </summary>
		/// <param name="out">
		///         The stream to which bytes are to be written
		/// </param>
		/// <returns>  A new writable byte channel </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static WritableByteChannel newChannel(final java.io.OutputStream out)
		public static WritableByteChannel NewChannel(OutputStream @out)
		{
			CheckNotNull(@out, "out");

			if (@out is FileOutputStream && typeof(FileOutputStream).Equals(@out.GetType()))
			{
					return ((FileOutputStream)@out).Channel;
			}

			return new WritableByteChannelImpl(@out);
		}

		private class WritableByteChannelImpl : AbstractInterruptibleChannel, WritableByteChannel // Not really interruptible
		{
			internal OutputStream @out;
			internal const int TRANSFER_SIZE = 8192;
			internal sbyte[] Buf = new sbyte[0];
			internal bool Open = true;
			internal Object WriteLock = new Object();

			internal WritableByteChannelImpl(OutputStream @out)
			{
				this.@out = @out;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int write(java.nio.ByteBuffer src) throws java.io.IOException
			public virtual int Write(ByteBuffer src)
			{
				int len = src.Remaining();
				int totalWritten = 0;
				lock (WriteLock)
				{
					while (totalWritten < len)
					{
						int bytesToWrite = System.Math.Min((len - totalWritten), TRANSFER_SIZE);
						if (Buf.Length < bytesToWrite)
						{
							Buf = new sbyte[bytesToWrite];
						}
						src.Get(Buf, 0, bytesToWrite);
						try
						{
							Begin();
							@out.Write(Buf, 0, bytesToWrite);
						}
						finally
						{
							End(bytesToWrite > 0);
						}
						totalWritten += bytesToWrite;
					}
					return totalWritten;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void implCloseChannel() throws java.io.IOException
			protected internal override void ImplCloseChannel()
			{
				@out.Close();
				Open = false;
			}
		}


		// -- Character streams from channels --

		/// <summary>
		/// Constructs a reader that decodes bytes from the given channel using the
		/// given decoder.
		/// 
		/// <para> The resulting stream will contain an internal input buffer of at
		/// least <tt>minBufferCap</tt> bytes.  The stream's <tt>read</tt> methods
		/// will, as needed, fill the buffer by reading bytes from the underlying
		/// channel; if the channel is in non-blocking mode when bytes are to be
		/// read then an <seealso cref="IllegalBlockingModeException"/> will be thrown.  The
		/// resulting stream will not otherwise be buffered, and it will not support
		/// the <seealso cref="Reader#mark mark"/> or <seealso cref="Reader#reset reset"/> methods.
		/// Closing the stream will in turn cause the channel to be closed.  </para>
		/// </summary>
		/// <param name="ch">
		///         The channel from which bytes will be read
		/// </param>
		/// <param name="dec">
		///         The charset decoder to be used
		/// </param>
		/// <param name="minBufferCap">
		///         The minimum capacity of the internal byte buffer,
		///         or <tt>-1</tt> if an implementation-dependent
		///         default capacity is to be used
		/// </param>
		/// <returns>  A new reader </returns>
		public static Reader NewReader(ReadableByteChannel ch, CharsetDecoder dec, int minBufferCap)
		{
			CheckNotNull(ch, "ch");
			return StreamDecoder.forDecoder(ch, dec.Reset(), minBufferCap);
		}

		/// <summary>
		/// Constructs a reader that decodes bytes from the given channel according
		/// to the named charset.
		/// 
		/// <para> An invocation of this method of the form
		/// 
		/// <blockquote><pre>
		/// Channels.newReader(ch, csname)</pre></blockquote>
		/// 
		/// behaves in exactly the same way as the expression
		/// 
		/// <blockquote><pre>
		/// Channels.newReader(ch,
		///                    Charset.forName(csName)
		///                        .newDecoder(),
		///                    -1);</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">
		///         The channel from which bytes will be read
		/// </param>
		/// <param name="csName">
		///         The name of the charset to be used
		/// </param>
		/// <returns>  A new reader
		/// </returns>
		/// <exception cref="UnsupportedCharsetException">
		///          If no support for the named charset is available
		///          in this instance of the Java virtual machine </exception>
		public static Reader NewReader(ReadableByteChannel ch, String csName)
		{
			CheckNotNull(csName, "csName");
			return NewReader(ch, Charset.ForName(csName).NewDecoder(), -1);
		}

		/// <summary>
		/// Constructs a writer that encodes characters using the given encoder and
		/// writes the resulting bytes to the given channel.
		/// 
		/// <para> The resulting stream will contain an internal output buffer of at
		/// least <tt>minBufferCap</tt> bytes.  The stream's <tt>write</tt> methods
		/// will, as needed, flush the buffer by writing bytes to the underlying
		/// channel; if the channel is in non-blocking mode when bytes are to be
		/// written then an <seealso cref="IllegalBlockingModeException"/> will be thrown.
		/// The resulting stream will not otherwise be buffered.  Closing the stream
		/// will in turn cause the channel to be closed.  </para>
		/// </summary>
		/// <param name="ch">
		///         The channel to which bytes will be written
		/// </param>
		/// <param name="enc">
		///         The charset encoder to be used
		/// </param>
		/// <param name="minBufferCap">
		///         The minimum capacity of the internal byte buffer,
		///         or <tt>-1</tt> if an implementation-dependent
		///         default capacity is to be used
		/// </param>
		/// <returns>  A new writer </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static java.io.Writer newWriter(final WritableByteChannel ch, final java.nio.charset.CharsetEncoder enc, final int minBufferCap)
		public static Writer NewWriter(WritableByteChannel ch, CharsetEncoder enc, int minBufferCap)
		{
			CheckNotNull(ch, "ch");
			return StreamEncoder.forEncoder(ch, enc.Reset(), minBufferCap);
		}

		/// <summary>
		/// Constructs a writer that encodes characters according to the named
		/// charset and writes the resulting bytes to the given channel.
		/// 
		/// <para> An invocation of this method of the form
		/// 
		/// <blockquote><pre>
		/// Channels.newWriter(ch, csname)</pre></blockquote>
		/// 
		/// behaves in exactly the same way as the expression
		/// 
		/// <blockquote><pre>
		/// Channels.newWriter(ch,
		///                    Charset.forName(csName)
		///                        .newEncoder(),
		///                    -1);</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">
		///         The channel to which bytes will be written
		/// </param>
		/// <param name="csName">
		///         The name of the charset to be used
		/// </param>
		/// <returns>  A new writer
		/// </returns>
		/// <exception cref="UnsupportedCharsetException">
		///          If no support for the named charset is available
		///          in this instance of the Java virtual machine </exception>
		public static Writer NewWriter(WritableByteChannel ch, String csName)
		{
			CheckNotNull(csName, "csName");
			return NewWriter(ch, Charset.ForName(csName).NewEncoder(), -1);
		}
	}

}