/*
 * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.geom
{

	/// <summary>
	/// The <code>IllegalPathStateException</code> represents an
	/// exception that is thrown if an operation is performed on a path
	/// that is in an illegal state with respect to the particular
	/// operation being performed, such as appending a path segment
	/// to a <seealso cref="GeneralPath"/> without an initial moveto.
	/// 
	/// </summary>

	public class IllegalPathStateException : RuntimeException
	{
		/// <summary>
		/// Constructs an <code>IllegalPathStateException</code> with no
		/// detail message.
		/// 
		/// @since   1.2
		/// </summary>
		public IllegalPathStateException()
		{
		}

		/// <summary>
		/// Constructs an <code>IllegalPathStateException</code> with the
		/// specified detail message. </summary>
		/// <param name="s">   the detail message
		/// @since   1.2 </param>
		public IllegalPathStateException(String s) : base(s)
		{
		}
	}

}