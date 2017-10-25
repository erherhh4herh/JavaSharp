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
	/// Thrown if an application tries to create an array with negative size.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class NegativeArraySizeException : RuntimeException
	{
		private new const long SerialVersionUID = -8960118058596991861L;

		/// <summary>
		/// Constructs a <code>NegativeArraySizeException</code> with no
		/// detail message.
		/// </summary>
		public NegativeArraySizeException() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>NegativeArraySizeException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public NegativeArraySizeException(String s) : base(s)
		{
		}
	}

}