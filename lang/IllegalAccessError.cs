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
	/// Thrown if an application attempts to access or modify a field, or
	/// to call a method that it does not have access to.
	/// <para>
	/// Normally, this error is caught by the compiler; this error can
	/// only occur at run time if the definition of a class has
	/// incompatibly changed.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public class IllegalAccessError : IncompatibleClassChangeError
	{
		private new const long SerialVersionUID = -8988904074992417891L;

		/// <summary>
		/// Constructs an <code>IllegalAccessError</code> with no detail message.
		/// </summary>
		public IllegalAccessError() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>IllegalAccessError</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public IllegalAccessError(String s) : base(s)
		{
		}
	}

}