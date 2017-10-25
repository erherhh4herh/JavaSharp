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
	/// Thrown when an application tries to call an abstract method.
	/// Normally, this error is caught by the compiler; this error can
	/// only occur at run time if the definition of some class has
	/// incompatibly changed since the currently executing method was last
	/// compiled.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class AbstractMethodError : IncompatibleClassChangeError
	{
		private new const long SerialVersionUID = -1654391082989018462L;

		/// <summary>
		/// Constructs an <code>AbstractMethodError</code> with no detail  message.
		/// </summary>
		public AbstractMethodError() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>AbstractMethodError</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public AbstractMethodError(String s) : base(s)
		{
		}
	}

}