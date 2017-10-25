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
	/// Thrown if an application tries to access or modify a specified
	/// field of an object, and that object no longer has that field.
	/// <para>
	/// Normally, this error is caught by the compiler; this error can
	/// only occur at run time if the definition of a class has
	/// incompatibly changed.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public class NoSuchFieldError : IncompatibleClassChangeError
	{
		private new const long SerialVersionUID = -3456430195886129035L;

		/// <summary>
		/// Constructs a <code>NoSuchFieldError</code> with no detail message.
		/// </summary>
		public NoSuchFieldError() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>NoSuchFieldError</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public NoSuchFieldError(String s) : base(s)
		{
		}
	}

}