/*
 * Copyright (c) 1998, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// file and determines that the major and minor version numbers
	/// in the file are not supported.
	/// 
	/// @since   1.2
	/// </summary>
	public class UnsupportedClassVersionError : ClassFormatError
	{
		private new const long SerialVersionUID = -7123279212883497373L;

		/// <summary>
		/// Constructs a <code>UnsupportedClassVersionError</code>
		/// with no detail message.
		/// </summary>
		public UnsupportedClassVersionError() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>UnsupportedClassVersionError</code> with
		/// the specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public UnsupportedClassVersionError(String s) : base(s)
		{
		}
	}

}