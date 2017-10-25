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
	/// Abstract class for writing filtered character streams.
	/// The abstract class <code>FilterWriter</code> itself
	/// provides default methods that pass all requests to the
	/// contained stream. Subclasses of <code>FilterWriter</code>
	/// should override some of these methods and may also
	/// provide additional methods and fields.
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </summary>

	public abstract class FilterWriter : Writer
	{

		/// <summary>
		/// The underlying character-output stream.
		/// </summary>
		protected internal Writer @out;

		/// <summary>
		/// Create a new filtered writer.
		/// </summary>
		/// <param name="out">  a Writer object to provide the underlying stream. </param>
		/// <exception cref="NullPointerException"> if <code>out</code> is <code>null</code> </exception>
		protected internal FilterWriter(Writer @out) : base(@out)
		{
			this.@out = @out;
		}

		/// <summary>
		/// Writes a single character.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int c) throws IOException
		public override void Write(int c)
		{
			@out.Write(c);
		}

		/// <summary>
		/// Writes a portion of an array of characters.
		/// </summary>
		/// <param name="cbuf">  Buffer of characters to be written </param>
		/// <param name="off">   Offset from which to start reading characters </param>
		/// <param name="len">   Number of characters to be written
		/// </param>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(char cbuf[] , int off, int len) throws IOException
		public override void Write(char[] cbuf, int off, int len)
		{
			@out.Write(cbuf, off, len);
		}

		/// <summary>
		/// Writes a portion of a string.
		/// </summary>
		/// <param name="str">  String to be written </param>
		/// <param name="off">  Offset from which to start reading characters </param>
		/// <param name="len">  Number of characters to be written
		/// </param>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(String str, int off, int len) throws IOException
		public override void Write(String str, int off, int len)
		{
			@out.Write(str, off, len);
		}

		/// <summary>
		/// Flushes the stream.
		/// </summary>
		/// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws IOException
		public override void Flush()
		{
			@out.Flush();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
		public override void Close()
		{
			@out.Close();
		}

	}

}