/*
 * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown by a <code>Scanner</code> to indicate that the token
	/// retrieved does not match the pattern for the expected type, or
	/// that the token is out of range for the expected type.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.util.Scanner
	/// @since   1.5 </seealso>
	public class InputMismatchException : NoSuchElementException
	{
		private new const long SerialVersionUID = 8811230760997066428L;

		/// <summary>
		/// Constructs an <code>InputMismatchException</code> with <tt>null</tt>
		/// as its error message string.
		/// </summary>
		public InputMismatchException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>InputMismatchException</code>, saving a reference
		/// to the error message string <tt>s</tt> for later retrieval by the
		/// <tt>getMessage</tt> method.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public InputMismatchException(String s) : base(s)
		{
		}
	}

}