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
	/// Thrown when an incompatible class change has occurred to some class
	/// definition. The definition of some class, on which the currently
	/// executing method depends, has since changed.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class IncompatibleClassChangeError : LinkageError
	{
		private new const long SerialVersionUID = -4914975503642802119L;

		/// <summary>
		/// Constructs an <code>IncompatibleClassChangeError</code> with no
		/// detail message.
		/// </summary>
		public IncompatibleClassChangeError() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>IncompatibleClassChangeError</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public IncompatibleClassChangeError(String s) : base(s)
		{
		}
	}

}