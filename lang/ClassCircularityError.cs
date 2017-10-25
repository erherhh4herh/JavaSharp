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
	/// Thrown when the Java Virtual Machine detects a circularity in the
	/// superclass hierarchy of a class being loaded.
	/// 
	/// @author     unascribed
	/// @since      JDK1.0
	/// </summary>
	public class ClassCircularityError : LinkageError
	{
		private new const long SerialVersionUID = 1054362542914539689L;

		/// <summary>
		/// Constructs a {@code ClassCircularityError} with no detail message.
		/// </summary>
		public ClassCircularityError() : base()
		{
		}

		/// <summary>
		/// Constructs a {@code ClassCircularityError} with the specified detail
		/// message.
		/// </summary>
		/// <param name="s">
		///         The detail message </param>
		public ClassCircularityError(String s) : base(s)
		{
		}
	}

}