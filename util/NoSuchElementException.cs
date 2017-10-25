/*
 * Copyright (c) 1994, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{

	/// <summary>
	/// Thrown by various accessor methods to indicate that the element being requested
	/// does not exist.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.util.Enumeration#nextElement() </seealso>
	/// <seealso cref=     java.util.Iterator#next()
	/// @since   JDK1.0 </seealso>
	public class NoSuchElementException : RuntimeException
	{
		private new const long SerialVersionUID = 6769829250639411880L;

		/// <summary>
		/// Constructs a <code>NoSuchElementException</code> with <tt>null</tt>
		/// as its error message string.
		/// </summary>
		public NoSuchElementException() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>NoSuchElementException</code>, saving a reference
		/// to the error message string <tt>s</tt> for later retrieval by the
		/// <tt>getMessage</tt> method.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public NoSuchElementException(String s) : base(s)
		{
		}
	}

}