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
	/// This class is the superclass of all classes that filter output
	/// streams. These streams sit on top of an already existing output
	/// stream (the <i>underlying</i> output stream) which it uses as its
	/// basic sink of data, but possibly transforming the data along the
	/// way or providing additional functionality.
	/// <para>
	/// The class <code>FilterOutputStream</code> itself simply overrides
	/// all methods of <code>OutputStream</code> with versions that pass
	/// all requests to the underlying output stream. Subclasses of
	/// <code>FilterOutputStream</code> may further override some of these
	/// methods as well as provide additional methods and fields.
	/// 
	/// @author  Jonathan Payne
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public class FilterOutputStream : OutputStream
	{
		/// <summary>
		/// The underlying output stream to be filtered.
		/// </summary>
		protected internal OutputStream @out;

		/// <summary>
		/// Creates an output stream filter built on top of the specified
		/// underlying output stream.
		/// </summary>
		/// <param name="out">   the underlying output stream to be assigned to
		///                the field <tt>this.out</tt> for later use, or
		///                <code>null</code> if this instance is to be
		///                created without an underlying stream. </param>
		public FilterOutputStream(OutputStream @out)
		{
			this.@out = @out;
		}

		/// <summary>
		/// Writes the specified <code>byte</code> to this output stream.
		/// <para>
		/// The <code>write</code> method of <code>FilterOutputStream</code>
		/// calls the <code>write</code> method of its underlying output stream,
		/// that is, it performs <tt>out.write(b)</tt>.
		/// </para>
		/// <para>
		/// Implements the abstract <tt>write</tt> method of <tt>OutputStream</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">   the <code>byte</code>. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int b) throws IOException
		public override void Write(int b)
		{
			@out.Write(b);
		}

		/// <summary>
		/// Writes <code>b.length</code> bytes to this output stream.
		/// <para>
		/// The <code>write</code> method of <code>FilterOutputStream</code>
		/// calls its <code>write</code> method of three arguments with the
		/// arguments <code>b</code>, <code>0</code>, and
		/// <code>b.length</code>.
		/// </para>
		/// <para>
		/// Note that this method does not call the one-argument
		/// <code>write</code> method of its underlying stream with the single
		/// argument <code>b</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">   the data to be written. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#write(byte[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[]) throws IOException
		public override void Write(sbyte[] b)
		{
			Write(b, 0, b.Length);
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified
		/// <code>byte</code> array starting at offset <code>off</code> to
		/// this output stream.
		/// <para>
		/// The <code>write</code> method of <code>FilterOutputStream</code>
		/// calls the <code>write</code> method of one argument on each
		/// <code>byte</code> to output.
		/// </para>
		/// <para>
		/// Note that this method does not call the <code>write</code> method
		/// of its underlying input stream with the same arguments. Subclasses
		/// of <code>FilterOutputStream</code> should provide a more efficient
		/// implementation of this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">     the data. </param>
		/// <param name="off">   the start offset in the data. </param>
		/// <param name="len">   the number of bytes to write. </param>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#write(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(byte b[] , int off, int len) throws IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			if ((off | len | (b.Length - (len + off)) | (off + len)) < 0)
			{
				throw new IndexOutOfBoundsException();
			}

			for (int i = 0 ; i < len ; i++)
			{
				Write(b[off + i]);
			}
		}

		/// <summary>
		/// Flushes this output stream and forces any buffered output bytes
		/// to be written out to the stream.
		/// <para>
		/// The <code>flush</code> method of <code>FilterOutputStream</code>
		/// calls the <code>flush</code> method of its underlying output stream.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException
		public override void Flush()
		{
			@out.Flush();
		}

		/// <summary>
		/// Closes this output stream and releases any system resources
		/// associated with the stream.
		/// <para>
		/// The <code>close</code> method of <code>FilterOutputStream</code>
		/// calls its <code>flush</code> method, and then calls the
		/// <code>close</code> method of its underlying output stream.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException">  if an I/O error occurs. </exception>
		/// <seealso cref=        java.io.FilterOutputStream#flush() </seealso>
		/// <seealso cref=        java.io.FilterOutputStream#out </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("try") public void close() throws IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public override void Close()
		{
			using (OutputStream ostream = @out)
			{
				Flush();
			}
		}
	}

}