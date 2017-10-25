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

namespace java.lang
{

	/// <summary>
	/// Thrown by {@code String} methods to indicate that an index
	/// is either negative or greater than the size of the string.  For
	/// some methods such as the charAt method, this exception also is
	/// thrown when the index is equal to the size of the string.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.lang.String#charAt(int)
	/// @since   JDK1.0 </seealso>
	public class StringIndexOutOfBoundsException : IndexOutOfBoundsException
	{
		private new const long SerialVersionUID = -6762910422159637258L;

		/// <summary>
		/// Constructs a {@code StringIndexOutOfBoundsException} with no
		/// detail message.
		/// 
		/// @since   JDK1.0.
		/// </summary>
		public StringIndexOutOfBoundsException() : base()
		{
		}

		/// <summary>
		/// Constructs a {@code StringIndexOutOfBoundsException} with
		/// the specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public StringIndexOutOfBoundsException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs a new {@code StringIndexOutOfBoundsException}
		/// class with an argument indicating the illegal index.
		/// </summary>
		/// <param name="index">   the illegal index. </param>
		public StringIndexOutOfBoundsException(int index) : base("String index out of range: " + index)
		{
		}
	}

}