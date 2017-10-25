/*
 * Copyright (c) 1995, 2010, Oracle and/or its affiliates. All rights reserved.
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
	/// Subclasses of {@code LinkageError} indicate that a class has
	/// some dependency on another class; however, the latter class has
	/// incompatibly changed after the compilation of the former class.
	/// 
	/// 
	/// @author  Frank Yellin
	/// @since   JDK1.0
	/// </summary>
	public class LinkageError : Error
	{
		private new const long SerialVersionUID = 3579600108157160122L;

		/// <summary>
		/// Constructs a {@code LinkageError} with no detail message.
		/// </summary>
		public LinkageError() : base()
		{
		}

		/// <summary>
		/// Constructs a {@code LinkageError} with the specified detail
		/// message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public LinkageError(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs a {@code LinkageError} with the specified detail
		/// message and cause.
		/// </summary>
		/// <param name="s">     the detail message. </param>
		/// <param name="cause"> the cause, may be {@code null}
		/// @since 1.7 </param>
		public LinkageError(String s, Throwable cause) : base(s, cause)
		{
		}
	}

}