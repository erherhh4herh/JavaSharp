/*
 * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// Signals that the class doesn't have a field of a specified name.
	/// 
	/// @author  unascribed
	/// @since   JDK1.1
	/// </summary>
	public class NoSuchFieldException : ReflectiveOperationException
	{
		private new const long SerialVersionUID = -6143714805279938260L;

		/// <summary>
		/// Constructor.
		/// </summary>
		public NoSuchFieldException() : base()
		{
		}

		/// <summary>
		/// Constructor with a detail message.
		/// </summary>
		/// <param name="s"> the detail message </param>
		public NoSuchFieldException(String s) : base(s)
		{
		}
	}

}