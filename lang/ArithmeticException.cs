/*
 * Copyright (c) 1994, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown when an exceptional arithmetic condition has occurred. For
	/// example, an integer "divide by zero" throws an
	/// instance of this class.
	/// 
	/// {@code ArithmeticException} objects may be constructed by the
	/// virtual machine as if {@link Throwable#Throwable(String,
	/// Throwable, boolean, boolean) suppression were disabled and/or the
	/// stack trace was not writable}.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class ArithmeticException : RuntimeException
	{
		private new const long SerialVersionUID = 2256477558314496007L;

		/// <summary>
		/// Constructs an {@code ArithmeticException} with no detail
		/// message.
		/// </summary>
		public ArithmeticException() : base()
		{
		}

		/// <summary>
		/// Constructs an {@code ArithmeticException} with the specified
		/// detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public ArithmeticException(String s) : base(s)
		{
		}
	}

}