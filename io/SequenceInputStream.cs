using System.Collections.Generic;

/*
 * Copyright (c) 1994, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// A <code>SequenceInputStream</code> represents
	/// the logical concatenation of other input
	/// streams. It starts out with an ordered
	/// collection of input streams and reads from
	/// the first one until end of file is reached,
	/// whereupon it reads from the second one,
	/// and so on, until end of file is reached
	/// on the last of the contained input streams.
	/// 
	/// @author  Author van Hoff
	/// @since   JDK1.0
	/// </summary>
	public class SequenceInputStream : InputStream
	{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<? extends java.io.InputStream> e;
		internal IEnumerator<?> e;
		internal InputStream @in;

		/// <summary>
		/// Initializes a newly created <code>SequenceInputStream</code>
		/// by remembering the argument, which must
		/// be an <code>Enumeration</code>  that produces
		/// objects whose run-time type is <code>InputStream</code>.
		/// The input streams that are  produced by
		/// the enumeration will be read, in order,
		/// to provide the bytes to be read  from this
		/// <code>SequenceInputStream</code>. After
		/// each input stream from the enumeration
		/// is exhausted, it is closed by calling its
		/// <code>close</code> method.
		/// </summary>
		/// <param name="e">   an enumeration of input streams. </param>
		/// <seealso cref=     java.util.Enumeration </seealso>
		public SequenceInputStream<T1>(IEnumerator<T1> e) where T1 : java.io.InputStream
		{
			this.e = e;
			try
			{
				NextStream();
			}
			catch (IOException)
			{
				// This should never happen
				throw new Error("panic");
			}
		}

		/// <summary>
		/// Initializes a newly
		/// created <code>SequenceInputStream</code>
		/// by remembering the two arguments, which
		/// will be read in order, first <code>s1</code>
		/// and then <code>s2</code>, to provide the
		/// bytes to be read from this <code>SequenceInputStream</code>.
		/// </summary>
		/// <param name="s1">   the first input stream to read. </param>
		/// <param name="s2">   the second input stream to read. </param>
		public SequenceInputStream(InputStream s1, InputStream s2)
		{
			List<InputStream> v = new List<InputStream>(2);

			v.Add(s1);
			v.Add(s2);
			e = v.elements();
			try
			{
				NextStream();
			}
			catch (IOException)
			{
				// This should never happen
				throw new Error("panic");
			}
		}

		/// <summary>
		///  Continues reading in the next stream if an EOF is reached.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: final void nextStream() throws IOException
		internal void NextStream()
		{
			if (@in != null)
			{
				@in.Close();
			}

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (e.hasMoreElements())
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				@in = (InputStream) e.nextElement();
				if (@in == null)
				{
					throw new NullPointerException();
				}
			}
			else
			{
				@in = null;
			}

		}

		/// <summary>
		/// Returns an estimate of the number of bytes that can be read (or
		/// skipped over) from the current underlying input stream without
		/// blocking by the next invocation of a method for the current
		/// underlying input stream. The next invocation might be
		/// the same thread or another thread.  A single read or skip of this
		/// many bytes will not block, but may read or skip fewer bytes.
		/// <para>
		/// This method simply calls {@code available} of the current underlying
		/// input stream and returns the result.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an estimate of the number of bytes that can be read (or
		///         skipped over) from the current underlying input stream
		///         without blocking or {@code 0} if this input stream
		///         has been closed by invoking its <seealso cref="#close()"/> method </returns>
		/// <exception cref="IOException">  if an I/O error occurs.
		/// 
		/// @since   JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws IOException
		public override int Available()
		{
			if (@in == null)
			{
				return 0; // no way to signal EOF from available()
			}
			return @in.Available();
		}

		/// <summary>
		/// Reads the next byte of data from this input stream. The byte is
		/// returned as an <code>int</code> in the range <code>0</code> to
		/// <code>255</code>. If no byte is available because the end of the
		/// stream has been reached, the value <code>-1</code> is returned.
		/// This method blocks until input data is available, the end of the
		/// stream is detected, or an exception is thrown.
		/// <para>
		/// This method
		/// tries to read one character from the current substream. If it
		/// reaches the end of the stream, it calls the <code>close</code>
		/// method of the current substream and begins reading from the next
		/// substream.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next byte of data, or <code>-1</code> if the end of the
		///             stream is reached. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			while (@in != null)
			{
				int c = @in.Read();
				if (c != -1)
				{
					return c;
				}
				NextStream();
			}
			return -1;
		}

		/// <summary>
		/// Reads up to <code>len</code> bytes of data from this input stream
		/// into an array of bytes.  If <code>len</code> is not zero, the method
		/// blocks until at least 1 byte of input is available; otherwise, no
		/// bytes are read and <code>0</code> is returned.
		/// <para>
		/// The <code>read</code> method of <code>SequenceInputStream</code>
		/// tries to read the data from the current substream. If it fails to
		/// read any characters because the substream has reached the end of
		/// the stream, it calls the <code>close</code> method of the current
		/// substream and begins reading from the next substream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">     the buffer into which the data is read. </param>
		/// <param name="off">   the start offset in array <code>b</code>
		///                   at which the data is written. </param>
		/// <param name="len">   the maximum number of bytes read. </param>
		/// <returns>     int   the number of bytes read. </returns>
		/// <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>b.length - off</code> </exception>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[] , int off, int len) throws IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			if (@in == null)
			{
				return -1;
			}
			else if (b == null)
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
			do
			{
				int n = @in.Read(b, off, len);
				if (n > 0)
				{
					return n;
				}
				NextStream();
			} while (@in != null);
			return -1;
		}

		/// <summary>
		/// Closes this input stream and releases any system resources
		/// associated with the stream.
		/// A closed <code>SequenceInputStream</code>
		/// cannot  perform input operations and cannot
		/// be reopened.
		/// <para>
		/// If this stream was created
		/// from an enumeration, all remaining elements
		/// are requested from the enumeration and closed
		/// before the <code>close</code> method returns.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			do
			{
				NextStream();
			} while (@in != null);
		}
	}

}