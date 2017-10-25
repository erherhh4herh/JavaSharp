/*
 * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Signals that a malformed string in
	/// <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
	/// format has been read in a data
	/// input stream or by any class that implements the data input
	/// interface.
	/// See the
	/// <a href="DataInput.html#modified-utf-8"><code>DataInput</code></a>
	/// class description for the format in
	/// which modified UTF-8 strings are read and written.
	/// 
	/// @author  Frank Yellin </summary>
	/// <seealso cref=     java.io.DataInput </seealso>
	/// <seealso cref=     java.io.DataInputStream#readUTF(java.io.DataInput) </seealso>
	/// <seealso cref=     java.io.IOException
	/// @since   JDK1.0 </seealso>
	public class UTFDataFormatException : IOException
	{
		private new const long SerialVersionUID = 420743449228280612L;

		/// <summary>
		/// Constructs a <code>UTFDataFormatException</code> with
		/// <code>null</code> as its error detail message.
		/// </summary>
		public UTFDataFormatException() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>UTFDataFormatException</code> with the
		/// specified detail message. The string <code>s</code> can be
		/// retrieved later by the
		/// <code><seealso cref="java.lang.Throwable#getMessage"/></code>
		/// method of class <code>java.lang.Throwable</code>.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public UTFDataFormatException(String s) : base(s)
		{
		}
	}

}