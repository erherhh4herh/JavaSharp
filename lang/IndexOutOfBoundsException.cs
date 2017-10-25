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

namespace java.lang
{

	/// <summary>
	/// Thrown to indicate that an index of some sort (such as to an array, to a
	/// string, or to a vector) is out of range.
	/// <para>
	/// Applications can subclass this class to indicate similar exceptions.
	/// 
	/// @author  Frank Yellin
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public class IndexOutOfBoundsException : RuntimeException
	{
		private new const long SerialVersionUID = 234122996006267687L;

		/// <summary>
		/// Constructs an <code>IndexOutOfBoundsException</code> with no
		/// detail message.
		/// </summary>
		public IndexOutOfBoundsException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>IndexOutOfBoundsException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public IndexOutOfBoundsException(String s) : base(s)
		{
		}
	}

}