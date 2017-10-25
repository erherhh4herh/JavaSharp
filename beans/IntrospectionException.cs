using System;

/*
 * Copyright (c) 1996, 2009, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{

	/// <summary>
	/// Thrown when an exception happens during Introspection.
	/// <para>
	/// Typical causes include not being able to map a string class name
	/// to a Class object, not being able to resolve a string method name,
	/// or specifying a method name that has the wrong type signature for
	/// its intended use.
	/// </para>
	/// </summary>

	public class IntrospectionException : Exception
	{
		private new const long SerialVersionUID = -3728150539969542619L;

		/// <summary>
		/// Constructs an <code>IntrospectionException</code> with a
		/// detailed message.
		/// </summary>
		/// <param name="mess"> Descriptive message </param>
		public IntrospectionException(String mess) : base(mess)
		{
		}
	}

}