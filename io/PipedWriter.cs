using System.Threading;

/*
 * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// Piped character-output streams.
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </summary>

	public class PipedWriter : Writer
	{

		/* REMIND: identification of the read and write sides needs to be
		   more sophisticated.  Either using thread groups (but what about
		   pipes within a thread?) or using finalization (but it may be a
		   long time until the next GC). */
		private PipedReader Sink;

		/* This flag records the open status of this particular writer. It
		 * is independent of the status flags defined in PipedReader. It is
		 * used to do a sanity check on connect.
		 */
		private bool Closed = false;

		/// <summary>
		/// Creates a piped writer connected to the specified piped
		/// reader. Data characters written to this stream will then be
		/// available as input from <code>snk</code>.
		/// </summary>
		/// <param name="snk">   The piped reader to connect to. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PipedWriter(PipedReader snk) throws IOException
		public PipedWriter(PipedReader snk)
		{
			Connect(snk);
		}

		/// <summary>
		/// Creates a piped writer that is not yet connected to a
		/// piped reader. It must be connected to a piped reader,
		/// either by the receiver or the sender, before being used.
		/// </summary>
		/// <seealso cref=     java.io.PipedReader#connect(java.io.PipedWriter) </seealso>
		/// <seealso cref=     java.io.PipedWriter#connect(java.io.PipedReader) </seealso>
		public PipedWriter()
		{
		}

		/// <summary>
		/// Connects this piped writer to a receiver. If this object
		/// is already connected to some other piped reader, an
		/// <code>IOException</code> is thrown.
		/// <para>
		/// If <code>snk</code> is an unconnected piped reader and
		/// <code>src</code> is an unconnected piped writer, they may
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
		/// <param name="snk">   the piped reader to connect to. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void connect(PipedReader snk) throws IOException
		public virtual void Connect(PipedReader snk)
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
				else if (snk.ClosedByReader || Closed)
				{
					throw new IOException("Pipe closed");
				}
        
				Sink = snk;
				snk.@in = -1;
				snk.@out = 0;
				snk.Connected = true;
			}
		}

		/// <summary>
		/// Writes the specified <code>char</code> to the piped output stream.
		/// If a thread was reading data characters from the connected piped input
		/// stream, but the thread is no longer alive, then an
		/// <code>IOException</code> is thrown.
		/// <para>
		/// Implements the <code>write</code> method of <code>Writer</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c">   the <code>char</code> to be written. </param>
		/// <exception cref="IOException">  if the pipe is
		///          <a href=PipedOutputStream.html#BROKEN> <code>broken</code></a>,
		///          <seealso cref="#connect(java.io.PipedReader) unconnected"/>, closed
		///          or an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int c) throws IOException
		public override void Write(int c)
		{
			if (Sink == null)
			{
				throw new IOException("Pipe not connected");
			}
			Sink.Receive(c);
		}

		/// <summary>
		/// Writes <code>len</code> characters from the specified character array
		/// starting at offset <code>off</code> to this piped output stream.
		/// This method blocks until all the characters are written to the output
		/// stream.
		/// If a thread was reading data characters from the connected piped input
		/// stream, but the thread is no longer alive, then an
		/// <code>IOException</code> is thrown.
		/// </summary>
		/// <param name="cbuf">  the data. </param>
		/// <param name="off">   the start offset in the data. </param>
		/// <param name="len">   the number of characters to write. </param>
		/// <exception cref="IOException">  if the pipe is
		///          <a href=PipedOutputStream.html#BROKEN> <code>broken</code></a>,
		///          <seealso cref="#connect(java.io.PipedReader) unconnected"/>, closed
		///          or an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(char cbuf[] , int off, int len) throws IOException
		public override void Write(char[] cbuf, int off, int len)
		{
			if (Sink == null)
			{
				throw new IOException("Pipe not connected");
			}
			else if ((off | len | (off + len) | (cbuf.Length - (off + len))) < 0)
			{
				throw new IndexOutOfBoundsException();
			}
			Sink.Receive(cbuf, off, len);
		}

		/// <summary>
		/// Flushes this output stream and forces any buffered output characters
		/// to be written out.
		/// This will notify any readers that characters are waiting in the pipe.
		/// </summary>
		/// <exception cref="IOException">  if the pipe is closed, or an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void flush() throws IOException
		public override void Flush()
		{
			lock (this)
			{
				if (Sink != null)
				{
					if (Sink.ClosedByReader || Closed)
					{
						throw new IOException("Pipe closed");
					}
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
		/// writing characters.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			Closed = true;
			if (Sink != null)
			{
				Sink.ReceivedLast();
			}
		}
	}

}