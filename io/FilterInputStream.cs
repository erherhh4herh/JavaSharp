/*
 * Copyright (c) 1994, 2010, Oracle and/or its affiliates. All rights reserved.
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
	/// A <code>FilterInputStream</code> contains
	/// some other input stream, which it uses as
	/// its  basic source of data, possibly transforming
	/// the data along the way or providing  additional
	/// functionality. The class <code>FilterInputStream</code>
	/// itself simply overrides all  methods of
	/// <code>InputStream</code> with versions that
	/// pass all requests to the contained  input
	/// stream. Subclasses of <code>FilterInputStream</code>
	/// may further override some of  these methods
	/// and may also provide additional methods
	/// and fields.
	/// 
	/// @author  Jonathan Payne
	/// @since   JDK1.0
	/// </summary>
	public class FilterInputStream : InputStream
	{
		/// <summary>
		/// The input stream to be filtered.
		/// </summary>
		protected internal volatile InputStream @in;

		/// <summary>
		/// Creates a <code>FilterInputStream</code>
		/// by assigning the  argument <code>in</code>
		/// to the field <code>this.in</code> so as
		/// to remember it for later use.
		/// </summary>
		/// <param name="in">   the underlying input stream, or <code>null</code> if
		///          this instance is to be created without an underlying stream. </param>
		protected internal FilterInputStream(InputStream @in)
		{
			this.@in = @in;
		}

		/// <summary>
		/// Reads the next byte of data from this input stream. The value
		/// byte is returned as an <code>int</code> in the range
		/// <code>0</code> to <code>255</code>. If no byte is available
		/// because the end of the stream has been reached, the value
		/// <code>-1</code> is returned. This method blocks until input data
		/// is available, the end of the stream is detected, or an exception
		/// is thrown.
		/// <para>
		/// This method
		/// simply performs <code>in.read()</code> and returns the result.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     the next byte of data, or <code>-1</code> if the end of the
		///             stream is reached. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			return @in.Read();
		}

		/// <summary>
		/// Reads up to <code>byte.length</code> bytes of data from this
		/// input stream into an array of bytes. This method blocks until some
		/// input is available.
		/// <para>
		/// This method simply performs the call
		/// <code>read(b, 0, b.length)</code> and returns
		/// the  result. It is important that it does
		/// <i>not</i> do <code>in.read(b)</code> instead;
		/// certain subclasses of  <code>FilterInputStream</code>
		/// depend on the implementation strategy actually
		/// used.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">   the buffer into which the data is read. </param>
		/// <returns>     the total number of bytes read into the buffer, or
		///             <code>-1</code> if there is no more data because the end of
		///             the stream has been reached. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#read(byte[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[]) throws IOException
		public override int Read(sbyte[] b)
		{
			return Read(b, 0, b.Length);
		}

		/// <summary>
		/// Reads up to <code>len</code> bytes of data from this input stream
		/// into an array of bytes. If <code>len</code> is not zero, the method
		/// blocks until some input is available; otherwise, no
		/// bytes are read and <code>0</code> is returned.
		/// <para>
		/// This method simply performs <code>in.read(b, off, len)</code>
		/// and returns the result.
		/// 
		/// </para>
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
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[] , int off, int len) throws IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			return @in.Read(b, off, len);
		}

		/// <summary>
		/// Skips over and discards <code>n</code> bytes of data from the
		/// input stream. The <code>skip</code> method may, for a variety of
		/// reasons, end up skipping over some smaller number of bytes,
		/// possibly <code>0</code>. The actual number of bytes skipped is
		/// returned.
		/// <para>
		/// This method simply performs <code>in.skip(n)</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n">   the number of bytes to be skipped. </param>
		/// <returns>     the actual number of bytes skipped. </returns>
		/// <exception cref="IOException">  if the stream does not support seek,
		///                          or if some other I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws IOException
		public override long Skip(long n)
		{
			return @in.Skip(n);
		}

		/// <summary>
		/// Returns an estimate of the number of bytes that can be read (or
		/// skipped over) from this input stream without blocking by the next
		/// caller of a method for this input stream. The next caller might be
		/// the same thread or another thread.  A single read or skip of this
		/// many bytes will not block, but may read or skip fewer bytes.
		/// <para>
		/// This method returns the result of <seealso cref="#in in"/>.available().
		/// 
		/// </para>
		/// </summary>
		/// <returns>     an estimate of the number of bytes that can be read (or skipped
		///             over) from this input stream without blocking. </returns>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws IOException
		public override int Available()
		{
			return @in.Available();
		}

		/// <summary>
		/// Closes this input stream and releases any system resources
		/// associated with the stream.
		/// This
		/// method simply performs <code>in.close()</code>.
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			@in.Close();
		}

		/// <summary>
		/// Marks the current position in this input stream. A subsequent
		/// call to the <code>reset</code> method repositions this stream at
		/// the last marked position so that subsequent reads re-read the same bytes.
		/// <para>
		/// The <code>readlimit</code> argument tells this input stream to
		/// allow that many bytes to be read before the mark position gets
		/// invalidated.
		/// </para>
		/// <para>
		/// This method simply performs <code>in.mark(readlimit)</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="readlimit">   the maximum limit of bytes that can be read before
		///                      the mark position becomes invalid. </param>
		/// <seealso cref=     java.io.FilterInputStream#in </seealso>
		/// <seealso cref=     java.io.FilterInputStream#reset() </seealso>
		public override void Mark(int readlimit)
		{
			lock (this)
			{
				@in.Mark(readlimit);
			}
		}

		/// <summary>
		/// Repositions this stream to the position at the time the
		/// <code>mark</code> method was last called on this input stream.
		/// <para>
		/// This method
		/// simply performs <code>in.reset()</code>.
		/// </para>
		/// <para>
		/// Stream marks are intended to be used in
		/// situations where you need to read ahead a little to see what's in
		/// the stream. Often this is most easily done by invoking some
		/// general parser. If the stream is of the type handled by the
		/// parse, it just chugs along happily. If the stream is not of
		/// that type, the parser should toss an exception when it fails.
		/// If this happens within readlimit bytes, it allows the outer
		/// code to reset the stream and try another parser.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if the stream has not been marked or if the
		///               mark has been invalidated. </exception>
		/// <seealso cref=        java.io.FilterInputStream#in </seealso>
		/// <seealso cref=        java.io.FilterInputStream#mark(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void reset() throws IOException
		public override void Reset()
		{
			lock (this)
			{
				@in.Reset();
			}
		}

		/// <summary>
		/// Tests if this input stream supports the <code>mark</code>
		/// and <code>reset</code> methods.
		/// This method
		/// simply performs <code>in.markSupported()</code>.
		/// </summary>
		/// <returns>  <code>true</code> if this stream type supports the
		///          <code>mark</code> and <code>reset</code> method;
		///          <code>false</code> otherwise. </returns>
		/// <seealso cref=     java.io.FilterInputStream#in </seealso>
		/// <seealso cref=     java.io.InputStream#mark(int) </seealso>
		/// <seealso cref=     java.io.InputStream#reset() </seealso>
		public override bool MarkSupported()
		{
			return @in.MarkSupported();
		}
	}

}