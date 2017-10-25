using System;

/*
 * Copyright (c) 2004, 2008, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.instrument
{

	/// <summary>
	/// Thrown by an implementation of
	/// <seealso cref="java.lang.instrument.Instrumentation#redefineClasses Instrumentation.redefineClasses"/>
	/// when one of the specified classes cannot be modified.
	/// </summary>
	/// <seealso cref=     java.lang.instrument.Instrumentation#redefineClasses
	/// @since   1.5 </seealso>
	public class UnmodifiableClassException : Exception
	{
		private new const long SerialVersionUID = 1716652643585309178L;

		/// <summary>
		/// Constructs an <code>UnmodifiableClassException</code> with no
		/// detail message.
		/// </summary>
		public UnmodifiableClassException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>UnmodifiableClassException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public UnmodifiableClassException(String s) : base(s)
		{
		}
	}

}