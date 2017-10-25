using System;
using System.Threading;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	/// <summary>
	/// A piped input stream should be connected
	/// to a piped output stream; the piped  input
	/// stream then provides whatever data bytes
	/// are written to the piped output  stream.
	/// Typically, data is read from a <code>PipedInputStream</code>
	/// object by one thread  and data is written
	/// to the corresponding <code>PipedOutputStream</code>
	/// by some  other thread. Attempting to use
	/// both objects from a single thread is not
	/// recommended, as it may deadlock the thread.
	/// The piped input stream contains a buffer,
	/// decoupling read operations from write operations,
	/// within limits.
	/// A pipe is said to be <a name="BROKEN"> <i>broken</i> </a> if a
	/// thread that was providing data bytes to the connected
	/// piped output stream is no longer alive.
	/// 
	/// @author  James Gosling </summary>
	/// <seealso cref=     java.io.PipedOutputStream
	/// @since   JDK1.0 </seealso>
	public class PipedInputStream : InputStream
	{
		internal bool ClosedByWriter = false;
		internal volatile bool ClosedByReader = false;
		internal bool Connected = false;

			/* REMIND: identification of the read and write sides needs to be
			   more sophisticated.  Either using thread groups (but what about
			   pipes within a thread?) or using finalization (but it may be a
			   long time until the next GC). */
		internal Thread ReadSide;
		internal Thread WriteSide;

		private const int DEFAULT_PIPE_SIZE = 1024;

		/// <summary>
		/// The default size of the pipe's circular input buffer.
		/// @since   JDK1.1
		/// </summary>
		// This used to be a constant before the pipe size was allowed
		// to change. This field will continue to be maintained
		// for backward compatibility.
		protected internal const int PIPE_SIZE = DEFAULT_PIPE_SIZE;

		/// <summary>
		/// The circular buffer into which incoming data is placed.
		/// @since   JDK1.1
		/// </summary>
		protected internal sbyte[] Buffer;

		/// <summary>
		/// The index of the position in the circular buffer at which the
		/// next byte of data will be stored when received from the connected
		/// piped output stream. <code>in&lt;0</code> implies the buffer is empty,
		/// <code>in==out</code> implies the buffer is full
		/// @since   JDK1.1
		/// </summary>
		protected internal int @in = -1;

		/// <summary>
		/// The index of the position in the circular buffer at which the next
		/// byte of data will be read by this piped input stream.
		/// @since   JDK1.1
		/// </summary>
		protected internal int @out = 0;

		/// <summary>
		/// Creates a <code>PipedInputStream</code> so
		/// that it is connected to the piped output
		/// stream <code>src</code>. Data bytes written
		/// to <code>src</code> will then be  available
		/// as input from this stream.
		/// </summary>
		/// <param name="src">   the stream to connect to. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PipedInputStream(PipedOutputStream src) throws IOException
		public PipedInputStream(PipedOutputStream src) : this(src, DEFAULT_PIPE_SIZE)
		{
		}

		/// <summary>
		/// Creates a <code>PipedInputStream</code> so that it is
		/// connected to the piped output stream
		/// <code>src</code> and uses the specified pipe size for
		/// the pipe's buffer.
		/// Data bytes written to <code>src</code> will then
		/// be available as input from this stream.
		/// </summary>
		/// <param name="src">   the stream to connect to. </param>
		/// <param name="pipeSize"> the size of the pipe's buffer. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code pipeSize <= 0}.
		/// @since      1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PipedInputStream(PipedOutputStream src, int pipeSize) throws IOException
		public PipedInputStream(PipedOutputStream src, int pipeSize)
		{
			 InitPipe(pipeSize);
			 Connect(src);
		}

		/// <summary>
		/// Creates a <code>PipedInputStream</code> so
		/// that it is not yet {@link #connect(java.io.PipedOutputStream)
		/// connected}.
		/// It must be {@link java.io.PipedOutputStream#connect(
		/// java.io.PipedInputStream) connected} to a
		/// <code>PipedOutputStream</code> before being used.
		/// </summary>
		public PipedInputStream()
		{
			InitPipe(DEFAULT_PIPE_SIZE);
		}

		/// <summary>
		/// Creates a <code>PipedInputStream</code> so that it is not yet
		/// <seealso cref="#connect(java.io.PipedOutputStream) connected"/> and
		/// uses the specified pipe size for the pipe's buffer.
		/// It must be {@link java.io.PipedOutputStream#connect(
		/// java.io.PipedInputStream)
		/// connected} to a <code>PipedOutputStream</code> before being used.
		/// </summary>
		/// <param name="pipeSize"> the size of the pipe's buffer. </param>
		/// <exception cref="IllegalArgumentException"> if {@code pipeSize <= 0}.
		/// @since      1.6 </exception>
		public PipedInputStream(int pipeSize)
		{
			InitPipe(pipeSize);
		}

		private void InitPipe(int pipeSize)
		{
			 if (pipeSize <= 0)
			 {
				throw new IllegalArgumentException("Pipe Size <= 0");
			 }
			 Buffer = new sbyte[pipeSize];
		}

		/// <summary>
		/// Causes this piped input stream to be connected
		/// to the piped  output stream <code>src</code>.
		/// If this object is already connected to some
		/// other piped output  stream, an <code>IOException</code>
		/// is thrown.
		/// <para>
		/// If <code>src</code> is an
		/// unconnected piped output stream and <code>snk</code>
		/// is an unconnected piped input stream, they
		/// may be connected by either the call:
		/// 
		/// <pre><code>snk.connect(src)</code> </pre>
		/// </para>
		/// <para>
		/// or the call:
		/// 
		/// <pre><code>src.connect(snk)</code> </pre>
		/// </para>
		/// <para>
		/// The two calls have the same effect.
		/// 
		/// </para>
		/// </summary>
		/// <param name="src">   The piped output stream to connect to. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void connect(PipedOutputStream src) throws IOException
		public virtual void Connect(PipedOutputStream src)
		{
			src.Connect(this);
		}

		/// <summary>
		/// Receives a byte of data.  This method will block if no input is
		/// available. </summary>
		/// <param name="b"> the byte being received </param>
		/// <exception cref="IOException"> If the pipe is <a href="#BROKEN"> <code>broken</code></a>,
		///          <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>,
		///          closed, or if an I/O error occurs.
		/// @since     JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected synchronized void receive(int b) throws IOException
		protected internal virtual void Receive(int b)
		{
			lock (this)
			{
				CheckStateForReceive();
				WriteSide = Thread.CurrentThread;
				if (@in == @out)
				{
					AwaitSpace();
				}
				if (@in < 0)
				{
					@in = 0;
					@out = 0;
				}
				Buffer[@in++] = unchecked((sbyte)(b & 0xFF));
				if (@in >= Buffer.Length)
				{
					@in = 0;
				}
			}
		}

		/// <summary>
		/// Receives data into an array of bytes.  This method will
		/// block until some input is available. </summary>
		/// <param name="b"> the buffer into which the data is received </param>
		/// <param name="off"> the start offset of the data </param>
		/// <param name="len"> the maximum number of bytes received </param>
		/// <exception cref="IOException"> If the pipe is <a href="#BROKEN"> broken</a>,
		///           <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>,
		///           closed,or if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: synchronized void receive(byte b[] , int off, int len) throws IOException
		internal virtual void Receive(sbyte[] b, int off, int len)
		{
			lock (this)
			{
				CheckStateForReceive();
				WriteSide = Thread.CurrentThread;
				int bytesToTransfer = len;
				while (bytesToTransfer > 0)
				{
					if (@in == @out)
					{
						AwaitSpace();
					}
					int nextTransferAmount = 0;
					if (@out < @in)
					{
						nextTransferAmount = Buffer.Length - @in;
					}
					else if (@in < @out)
					{
						if (@in == -1)
						{
							@in = @out = 0;
							nextTransferAmount = Buffer.Length - @in;
						}
						else
						{
							nextTransferAmount = @out - @in;
						}
					}
					if (nextTransferAmount > bytesToTransfer)
					{
						nextTransferAmount = bytesToTransfer;
					}
					assert(nextTransferAmount > 0);
					System.Array.Copy(b, off, Buffer, @in, nextTransferAmount);
					bytesToTransfer -= nextTransferAmount;
					off += nextTransferAmount;
					@in += nextTransferAmount;
					if (@in >= Buffer.Length)
					{
						@in = 0;
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkStateForReceive() throws IOException
		private void CheckStateForReceive()
		{
			if (!Connected)
			{
				throw new IOException("Pipe not connected");
			}
			else if (ClosedByWriter || ClosedByReader)
			{
				throw new IOException("Pipe closed");
			}
			else if (ReadSide != null && !ReadSide.Alive)
			{
				throw new IOException("Read end dead");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void awaitSpace() throws IOException
		private void AwaitSpace()
		{
			while (@in == @out)
			{
				CheckStateForReceive();

				/* full: kick any waiting readers */
				Monitor.PulseAll(this);
				try
				{
					Monitor.Wait(this, TimeSpan.FromMilliseconds(1000));
				}
				catch (InterruptedException)
				{
					throw new java.io.InterruptedIOException();
				}
			}
		}

		/// <summary>
		/// Notifies all waiting threads that the last byte of data has been
		/// received.
		/// </summary>
		internal virtual void ReceivedLast()
		{
			lock (this)
			{
				ClosedByWriter = true;
				Monitor.PulseAll(this);
			}
		}

		/// <summary>
		/// Reads the next byte of data from this piped input stream. The
		/// value byte is returned as an <code>int</code> in the range
		/// <code>0</code> to <code>255</code>.
		/// This method blocks until input data is available, the end of the
		/// stream is detected, or an exception is thrown.
		/// </summary>
		/// <returns>     the next byte of data, or <code>-1</code> if the end of the
		///             stream is reached. </returns>
		/// <exception cref="IOException">  if the pipe is
		///           <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>,
		///           <a href="#BROKEN"> <code>broken</code></a>, closed,
		///           or if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int read() throws IOException
		public override int Read()
		{
			lock (this)
			{
				if (!Connected)
				{
					throw new IOException("Pipe not connected");
				}
				else if (ClosedByReader)
				{
					throw new IOException("Pipe closed");
				}
				else if (WriteSide != null && !WriteSide.Alive && !ClosedByWriter && (@in < 0))
				{
					throw new IOException("Write end dead");
				}
        
				ReadSide = Thread.CurrentThread;
				int trials = 2;
				while (@in < 0)
				{
					if (ClosedByWriter)
					{
						/* closed by writer, return EOF */
						return -1;
					}
					if ((WriteSide != null) && (!WriteSide.Alive) && (--trials < 0))
					{
						throw new IOException("Pipe broken");
					}
					/* might be a writer waiting */
					Monitor.PulseAll(this);
					try
					{
						Monitor.Wait(this, TimeSpan.FromMilliseconds(1000));
					}
					catch (InterruptedException)
					{
						throw new java.io.InterruptedIOException();
					}
				}
				int ret = Buffer[@out++] & 0xFF;
				if (@out >= Buffer.Length)
				{
					@out = 0;
				}
				if (@in == @out)
				{
					/* now empty */
					@in = -1;
				}
        
				return ret;
			}
		}

		/// <summary>
		/// Reads up to <code>len</code> bytes of data from this piped input
		/// stream into an array of bytes. Less than <code>len</code> bytes
		/// will be read if the end of the data stream is reached or if
		/// <code>len</code> exceeds the pipe's buffer size.
		/// If <code>len </code> is zero, then no bytes are read and 0 is returned;
		/// otherwise, the method blocks until at least 1 byte of input is
		/// available, end of the stream has been detected, or an exception is
		/// thrown.
		/// </summary>
		/// <param name="b">     the buffer into which the data is read. </param>
		/// <param name="off">   the start offset in the destination array <code>b</code> </param>
		/// <param name="len">   the maximum number of bytes read. </param>
		/// <returns>     the total number of bytes read into the buffer, or
		///             <code>-1</code> if there is no more data because the end of
		///             the stream has been reached. </returns>
		/// <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>b.length - off</code> </exception>
		/// <exception cref="IOException"> if the pipe is <a href="#BROKEN"> <code>broken</code></a>,
		///           <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>,
		///           closed, or if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int read(byte b[] , int off, int len) throws IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			lock (this)
			{
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
        
				/* possibly wait on the first character */
				int c = Read();
				if (c < 0)
				{
					return -1;
				}
				b[off] = (sbyte) c;
				int rlen = 1;
				while ((@in >= 0) && (len > 1))
				{
        
					int available;
        
					if (@in > @out)
					{
						available = System.Math.Min((Buffer.Length - @out), (@in - @out));
					}
					else
					{
						available = Buffer.Length - @out;
					}
        
					// A byte is read beforehand outside the loop
					if (available > (len - 1))
					{
						available = len - 1;
					}
					System.Array.Copy(Buffer, @out, b, off + rlen, available);
					@out += available;
					rlen += available;
					len -= available;
        
					if (@out >= Buffer.Length)
					{
						@out = 0;
					}
					if (@in == @out)
					{
						/* now empty */
						@in = -1;
					}
				}
				return rlen;
			}
		}

		/// <summary>
		/// Returns the number of bytes that can be read from this input
		/// stream without blocking.
		/// </summary>
		/// <returns> the number of bytes that can be read from this input stream
		///         without blocking, or {@code 0} if this input stream has been
		///         closed by invoking its <seealso cref="#close()"/> method, or if the pipe
		///         is <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>, or
		///          <a href="#BROKEN"> <code>broken</code></a>.
		/// </returns>
		/// <exception cref="IOException">  if an I/O error occurs.
		/// @since   JDK1.0.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int available() throws IOException
		public override int Available()
		{
			lock (this)
			{
				if (@in < 0)
				{
					return 0;
				}
				else if (@in == @out)
				{
					return Buffer.Length;
				}
				else if (@in > @out)
				{
					return @in - @out;
				}
				else
				{
					return @in + Buffer.Length - @out;
				}
			}
		}

		/// <summary>
		/// Closes this piped input stream and releases any system resources
		/// associated with the stream.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			ClosedByReader = true;
			lock (this)
			{
				@in = -1;
			}
		}
	}

}