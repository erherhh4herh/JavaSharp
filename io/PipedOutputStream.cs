using System.Threading;

/*
 * Copyright (c) 1995, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// A piped output stream can be connected to a piped input stream
	/// to create a communications pipe. The piped output stream is the
	/// sending end of the pipe. Typically, data is written to a
	/// <code>PipedOutputStream</code> object by one thread and data is
	/// read from the connected <code>PipedInputStream</code> by some
	/// other thread. Attempting to use both objects from a single thread
	/// is not recommended as it may deadlock the thread.
	/// The pipe is said to be <a name=BROKEN> <i>broken</i> </a> if a
	/// thread that was reading data bytes from the connected piped input
	/// stream is no longer alive.
	/// 
	/// @author  James Gosling </summary>
	/// <seealso cref=     java.io.PipedInputStream
	/// @since   JDK1.0 </seealso>
	public class PipedOutputStream : OutputStream
	{

			/* REMIND: identification of the read and write sides needs to be
			   more sophisticated.  Either using thread groups (but what about
			   pipes within a thread?) or using finalization (but it may be a
			   long time until the next GC). */
		private PipedInputStream Sink;

		/// <summary>
		/// Creates a piped output stream connected to the specified piped
		/// input stream. Data bytes written to this stream will then be
		/// available as input from <code>snk</code>.
		/// </summary>
		/// <param name="snk">   The piped input stream to connect to. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PipedOutputStream(PipedInputStream snk) throws IOException
		public PipedOutputStream(PipedInputStream snk)
		{
			Connect(snk);
		}

		/// <summary>
		/// Creates a piped output stream that is not yet connected to a
		/// piped input stream. It must be connected to a piped input stream,
		/// either by the receiver or the sender, before being used.
		/// </summary>
		/// <seealso cref=     java.io.PipedInputStream#connect(java.io.PipedOutputStream) </seealso>
		/// <seealso cref=     java.io.PipedOutputStream#connect(java.io.PipedInputStream) </seealso>
		public PipedOutputStream()
		{
		}

		/// <summary>
		/// Connects this piped output stream to a receiver. If this object
		/// is already connected to some other piped input stream, an
		/// <code>IOException</code> is thrown.
		/// <para>
		/// If <code>snk</code> is an unconnected piped input stream and
		/// <code>src</code> is an unconnected piped output stream, they may
		/// be connected by either the call:
		/// <blockquote><pre>
		/// src.connect(snk)</pre></blockquote>
		/// or the call:
		/// <blockquote><pre>
		/// snk.connect(src)</pre></blockquote>
		/// The two calls have the same effect.
		/// 
		/// </para>
		/// </summary>
		/// <param name="snk">   the piped input stream to connect to. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void connect(PipedInputStream snk) throws IOException
		public virtual void Connect(PipedInputStream snk)
		{
			lock (this)
			{
				if (snk == null)
				{
					throw new NullPointerException();
				}
				else if (Sink != null || snk.Connected)
				{
					throw new IOException("Already connected");
				}
				Sink = snk;
				snk.@in = -1;
				snk.@out = 0;
				snk.Connected = true;
			}
		}

		/// <summary>
		/// Writes the specified <code>byte</code> to the piped output stream.
		/// <para>
		/// Implements the <code>write</code> method of <code>OutputStream</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">   the <code>byte</code> to be written. </param>
		/// <exception cref="IOException"> if the pipe is <a href=#BROKEN> broken</a>,
		///          <seealso cref="#connect(java.io.PipedInputStream) unconnected"/>,
		///          closed, or if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws IOException
		public override void Write(int b)
		{
			if (Sink == null)
			{
				throw new IOException("Pipe not connected");
			}
			Sink.Receive(b);
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified byte array
		/// starting at offset <code>off</code> to this piped output stream.
		/// This method blocks until all the bytes are written to the output
		/// stream.
		/// </summary>
		/// <param name="b">     the data. </param>
		/// <param name="off">   the start offset in the data. </param>
		/// <param name="len">   the number of bytes to write. </param>
		/// <exception cref="IOException"> if the pipe is <a href=#BROKEN> broken</a>,
		///          <seealso cref="#connect(java.io.PipedInputStream) unconnected"/>,
		///          closed, or if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[] , int off, int len) throws IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			if (Sink == null)
			{
				throw new IOException("Pipe not connected");
			}
			else if (b == null)
			{
				throw new NullPointerException();
			}
			else if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) > b.Length) || ((off + len) < 0))
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return;
			}
			Sink.Receive(b, off, len);
		}

		/// <summary>
		/// Flushes this output stream and forces any buffered output bytes
		/// to be written out.
		/// This will notify any readers that bytes are waiting in the pipe.
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void flush() throws IOException
		public override void Flush()
		{
			lock (this)
			{
				if (Sink != null)
				{
					lock (Sink)
					{
						Monitor.PulseAll(Sink);
					}
				}
			}
		}

		/// <summary>
		/// Closes this piped output stream and releases any system resources
		/// associated with this stream. This stream may no longer be used for
		/// writing bytes.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			if (Sink != null)
			{
				Sink.ReceivedLast();
			}
		}
	}

}