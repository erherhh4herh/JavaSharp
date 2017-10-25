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
	/// Signals that an end of file or end of stream has been reached
	/// unexpectedly during input.
	/// <para>
	/// This exception is mainly used by data input streams to signal end of
	/// stream. Note that many other input operations return a special value on
	/// end of stream rather than throwing an exception.
	/// 
	/// @author  Frank Yellin
	/// </para>
	/// </summary>
	/// <seealso cref=     java.io.DataInputStream </seealso>
	/// <seealso cref=     java.io.IOException
	/// @since   JDK1.0 </seealso>
	public class EOFException : IOException
	{
		private new const long SerialVersionUID = 6433858223774886977L;

		/// <summary>
		/// Constructs an <code>EOFException</code> with <code>null</code>
		/// as its error detail message.
		/// </summary>
		public EOFException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>EOFException</code> with the specified detail
		/// message. The string <code>s</code> may later be retrieved by the
		/// <code><seealso cref="java.lang.Throwable#getMessage"/></code> method of class
		/// <code>java.lang.Throwable</code>.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public EOFException(String s) : base(s)
		{
		}
	}

}