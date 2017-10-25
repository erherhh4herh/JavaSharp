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
	/// Thrown when an application tries to create an instance of a class
	/// using the {@code newInstance} method in class
	/// {@code Class}, but the specified class object cannot be
	/// instantiated.  The instantiation can fail for a variety of
	/// reasons including but not limited to:
	/// 
	/// <ul>
	/// <li> the class object represents an abstract class, an interface,
	///      an array class, a primitive type, or {@code void}
	/// <li> the class has no nullary constructor
	/// </ul>
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.lang.Class#newInstance()
	/// @since   JDK1.0 </seealso>
	public class InstantiationException : ReflectiveOperationException
	{
		private new const long SerialVersionUID = -8441929162975509110L;

		/// <summary>
		/// Constructs an {@code InstantiationException} with no detail message.
		/// </summary>
		public InstantiationException() : base()
		{
		}

		/// <summary>
		/// Constructs an {@code InstantiationException} with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public InstantiationException(String s) : base(s)
		{
		}
	}

}