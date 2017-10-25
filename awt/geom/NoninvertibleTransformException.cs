using System;

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
	/// The <code>NoninvertibleTransformException</code> class represents
	/// an exception that is thrown if an operation is performed requiring
	/// the inverse of an <seealso cref="AffineTransform"/> object but the
	/// <code>AffineTransform</code> is in a non-invertible state.
	/// </summary>

	public class NoninvertibleTransformException : Exception
	{
		/// <summary>
		/// Constructs an instance of
		/// <code>NoninvertibleTransformException</code>
		/// with the specified detail message. </summary>
		/// <param name="s">     the detail message
		/// @since   1.2 </param>
		public NoninvertibleTransformException(String s) : base(s)
		{
		}
	}

}