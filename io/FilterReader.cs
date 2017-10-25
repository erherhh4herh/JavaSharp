/*
 * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
	/// Abstract class for reading filtered character streams.
	/// The abstract class <code>FilterReader</code> itself
	/// provides default methods that pass all requests to
	/// the contained stream. Subclasses of <code>FilterReader</code>
	/// should override some of these methods and may also provide
	/// additional methods and fields.
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </summary>

	public abstract class FilterReader : Reader
	{

		/// <summary>
		/// The underlying character-input stream.
		/// </summary>
		protected internal Reader @in;

		/// <summary>
		/// Creates a new filtered reader.
		/// </summary>
		/// <param name="in">  a Reader object providing the underlying stream. </param>
		/// <exception cref="NullPointerException"> if <code>in</code> is <code>null</code> </exception>
		protected internal FilterReader(Reader @in) : base(@in)
		{
			this.@in = @in;
		}

		/// <summary>
		/// Reads a single character.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws IOException
		public override int Read()
		{
			return @in.Read();
		}

		/// <summary>
		/// Reads characters into a portion of an array.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(char cbuf[] , int off, int len) throws IOException
		public override int Read(char[] cbuf, int off, int len)
		{
			return @in.Read(cbuf, off, len);
		}

		/// <summary>
		/// Skips characters.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws IOException
		public override long Skip(long n)
		{
			return @in.Skip(n);
		}

		/// <summary>
		/// Tells whether this stream is ready to be read.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean ready() throws IOException
		public override bool Ready()
		{
			return @in.Ready();
		}

		/// <summary>
		/// Tells whether this stream supports the mark() operation.
		/// </summary>
		public override bool MarkSupported()
		{
			return @in.MarkSupported();
		}

		/// <summary>
		/// Marks the present position in the stream.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void mark(int readAheadLimit) throws IOException
		public override void Mark(int readAheadLimit)
		{
			@in.Mark(readAheadLimit);
		}

		/// <summary>
		/// Resets the stream.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws IOException
		public override void Reset()
		{
			@in.Reset();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			@in.Close();
		}

	}

}