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
	/// Thrown if an application tries to call a specified method of a
	/// class (either static or instance), and that class no longer has a
	/// definition of that method.
	/// <para>
	/// Normally, this error is caught by the compiler; this error can
	/// only occur at run time if the definition of a class has
	/// incompatibly changed.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public class NoSuchMethodError : IncompatibleClassChangeError
	{
		private new const long SerialVersionUID = -3765521442372831335L;

		/// <summary>
		/// Constructs a <code>NoSuchMethodError</code> with no detail message.
		/// </summary>
		public NoSuchMethodError() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>NoSuchMethodError</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public NoSuchMethodError(String s) : base(s)
		{
		}
	}

}