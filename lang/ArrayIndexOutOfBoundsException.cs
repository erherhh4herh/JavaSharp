/*
 * Copyright (c) 1994, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown to indicate that an array has been accessed with an
	/// illegal index. The index is either negative or greater than or
	/// equal to the size of the array.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class ArrayIndexOutOfBoundsException : IndexOutOfBoundsException
	{
		private new const long SerialVersionUID = -5116101128118950844L;

		/// <summary>
		/// Constructs an <code>ArrayIndexOutOfBoundsException</code> with no
		/// detail message.
		/// </summary>
		public ArrayIndexOutOfBoundsException() : base()
		{
		}

		/// <summary>
		/// Constructs a new <code>ArrayIndexOutOfBoundsException</code>
		/// class with an argument indicating the illegal index.
		/// </summary>
		/// <param name="index">   the illegal index. </param>
		public ArrayIndexOutOfBoundsException(int index) : base("Array index out of range: " + index)
		{
		}

		/// <summary>
		/// Constructs an <code>ArrayIndexOutOfBoundsException</code> class
		/// with the specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public ArrayIndexOutOfBoundsException(String s) : base(s)
		{
		}
	}

}