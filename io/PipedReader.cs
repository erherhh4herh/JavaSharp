using System;
using System.Threading;

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

namespace java.io
{


	/// <summary>
	/// Piped character-input streams.
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </summary>

	public class PipedReader : Reader
	{
		internal bool ClosedByWriter = false;
		internal bool ClosedByReader = false;
		internal bool Connected = false;

		/* REMIND: identification of the read and write sides needs to be
		   more sophisticated.  Either using thread groups (but what about
		   pipes within a thread?) or using finalization (but it may be a
		   long time until the next GC). */
		internal Thread ReadSide;
		internal Thread WriteSide;

	   /// <summary>
	   /// The size of the pipe's circular input buffer.
	   /// </summary>
		private const int DEFAULT_PIPE_SIZE = 1024;

		/// <summary>
		/// The circular buffer into which incoming data is placed.
		/// </summary>
		internal char[] Buffer;

		/// <summary>
		/// The index of the position in the circular buffer at which the
		/// next character of data will be stored when received from the connected
		/// piped writer. <code>in&lt;0</code> implies the buffer is empty,
		/// <code>in==out</code> implies the buffer is full
		/// </summary>
		internal int @in = -1;

		/// <summary>
		/// The index of the position in the circular buffer at which the next
		/// character of data will be read by this piped reader.
		/// </summary>
		internal int @out = 0;

		/// <summary>
		/// Creates a <code>PipedReader</code> so
		/// that it is connected to the piped writer
		/// <code>src</code>. Data written to <code>src</code>
		/// will then be available as input from this stream.
		/// </summary>
		/// <param name="src">   the stream to connect to. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PipedReader(PipedWriter src) throws IOException
		public PipedReader(PipedWriter src) : this(src, DEFAULT_PIPE_SIZE)
		{
		}

		/// <summary>
		/// Creates a <code>PipedReader</code> so that it is connected
		/// to the piped writer <code>src</code> and uses the specified
		/// pipe size for the pipe's buffer. Data written to <code>src</code>
		/// will then be  available as input from this stream.
		/// </summary>
		/// <param name="src">       the stream to connect to. </param>
		/// <param name="pipeSize">  the size of the pipe's buffer. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code pipeSize <= 0}.
		/// @since      1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PipedReader(PipedWriter src, int pipeSize) throws IOException
		public PipedReader(PipedWriter src, int pipeSize)
		{
			InitPipe(pipeSize);
			Connect(src);
		}


		/// <summary>
		/// Creates a <code>PipedReader</code> so
		/// that it is not yet {@link #connect(java.io.PipedWriter)
		/// connected}. It must be {@link java.io.PipedWriter#connect(
		/// java.io.PipedReader) connected} to a <code>PipedWriter</code>
		/// before being used.
		/// </summary>
		public PipedReader()
		{
			InitPipe(DEFAULT_PIPE_SIZE);
		}

		/// <summary>
		/// Creates a <code>PipedReader</code> so that it is not yet
		/// <seealso cref="#connect(java.io.PipedWriter) connected"/> and uses
		/// the specified pipe size for the pipe's buffer.
		/// It must be  {@link java.io.PipedWriter#connect(
		/// java.io.PipedReader) connected} to a <code>PipedWriter</code>
		/// before being used.
		/// </summary>
		/// <param name="pipeSize"> the size of the pipe's buffer. </param>
		/// <exception cref="IllegalArgumentException"> if {@code pipeSize <= 0}.
		/// @since      1.6 </exception>
		public PipedReader(int pipeSize)
		{
			InitPipe(pipeSize);
		}

		private void InitPipe(int pipeSize)
		{
			if (pipeSize <= 0)
			{
				throw new IllegalArgumentException("Pipe size <= 0");
			}
			Buffer = new char[pipeSize];
		}

		/// <summary>
		/// Causes this piped reader to be connected
		/// to the piped  writer <code>src</code>.
		/// If this object is already connected to some
		/// other piped writer, an <code>IOException</code>
		/// is thrown.
		/// <para>
		/// If <code>src</code> is an
		/// unconnected piped writer and <code>snk</code>
		/// is an unconnected piped reader, they
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
		/// <param name="src">   The piped writer to connect to. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void connect(PipedWriter src) throws IOException
		public virtual void Connect(PipedWriter src)
		{
			src.Connect(this);
		}

		/// <summary>
		/// Receives a char of data. This method will block if no input is
		/// available.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: synchronized void receive(int c) throws IOException
		internal virtual void Receive(int c)
		{
			lock (this)
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
        
				WriteSide = Thread.CurrentThread;
				while (@in == @out)
				{
					if ((ReadSide != null) && !ReadSide.Alive)
					{
						throw new IOException("Pipe broken");
					}
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
				if (@in < 0)
				{
					@in = 0;
					@out = 0;
				}
				Buffer[@in++] = (char) c;
				if (@in >= Buffer.Length)
				{
					@in = 0;
				}
			}
		}

		/// <summary>
		/// Receives data into an array of characters.  This method will
		/// block until some input is available.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: synchronized void receive(char c[] , int off, int len) throws IOException
		internal virtual void Receive(char[] c, int off, int len)
		{
			lock (this)
			{
				while (--len >= 0)
				{
					Receive(c[off++]);
				}
			}
		}

		/// <summary>
		/// Notifies all waiting threads that the last character of data has been
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
		/// Reads the next character of data from this piped stream.
		/// If no character is available because the end of the stream
		/// has been reached, the value <code>-1</code> is returned.
		/// This method blocks until input data is available, the end of
		/// the stream is detected, or an exception is thrown.
		/// </summary>
		/// <returns>     the next character of data, or <code>-1</code> if the end of the
		///             stream is reached. </returns>
		/// <exception cref="IOException">  if the pipe is
		///          <a href=PipedInputStream.html#BROKEN> <code>broken</code></a>,
		///          <seealso cref="#connect(java.io.PipedWriter) unconnected"/>, closed,
		///          or an I/O error occurs. </exception>
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
				int ret = Buffer[@out++];
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
		/// Reads up to <code>len</code> characters of data from this piped
		/// stream into an array of characters. Less than <code>len</code> characters
		/// will be read if the end of the data stream is reached or if
		/// <code>len</code> exceeds the pipe's buffer size. This method
		/// blocks until at least one character of input is available.
		/// </summary>
		/// <param name="cbuf">     the buffer into which the data is read. </param>
		/// <param name="off">   the start offset of the data. </param>
		/// <param name="len">   the maximum number of characters read. </param>
		/// <returns>     the total number of characters read into the buffer, or
		///             <code>-1</code> if there is no more data because the end of
		///             the stream has been reached. </returns>
		/// <exception cref="IOException">  if the pipe is
		///                  <a href=PipedInputStream.html#BROKEN> <code>broken</code></a>,
		///                  <seealso cref="#connect(java.io.PipedWriter) unconnected"/>, closed,
		///                  or an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int read(char cbuf[] , int off, int len) throws IOException
		public override int Read(char[] cbuf, int off, int len)
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
        
				if ((off < 0) || (off > cbuf.Length) || (len < 0) || ((off + len) > cbuf.Length) || ((off + len) < 0))
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
				cbuf[off] = (char)c;
				int rlen = 1;
				while ((@in >= 0) && (--len > 0))
				{
					cbuf[off + rlen] = Buffer[@out++];
					rlen++;
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
		/// Tell whether this stream is ready to be read.  A piped character
		/// stream is ready if the circular buffer is not empty.
		/// </summary>
		/// <exception cref="IOException">  if the pipe is
		///                  <a href=PipedInputStream.html#BROKEN> <code>broken</code></a>,
		///                  <seealso cref="#connect(java.io.PipedWriter) unconnected"/>, or closed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized boolean ready() throws IOException
		public override bool Ready()
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
				if (@in < 0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		/// <summary>
		/// Closes this piped stream and releases any system resources
		/// associated with the stream.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			@in = -1;
			ClosedByReader = true;
		}
	}

}