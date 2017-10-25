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
	/// Thrown when the Java Virtual Machine attempts to read a class
	/// file and determines that the file is malformed or otherwise cannot
	/// be interpreted as a class file.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class ClassFormatError : LinkageError
	{
		private new const long SerialVersionUID = -8420114879011949195L;

		/// <summary>
		/// Constructs a <code>ClassFormatError</code> with no detail message.
		/// </summary>
		public ClassFormatError() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>ClassFormatError</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public ClassFormatError(String s) : base(s)
		{
		}
	}

}