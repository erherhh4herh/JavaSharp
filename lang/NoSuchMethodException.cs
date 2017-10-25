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
	/// Thrown when a particular method cannot be found.
	/// 
	/// @author     unascribed
	/// @since      JDK1.0
	/// </summary>
	public class NoSuchMethodException : ReflectiveOperationException
	{
		private new const long SerialVersionUID = 5034388446362600923L;

		/// <summary>
		/// Constructs a <code>NoSuchMethodException</code> without a detail message.
		/// </summary>
		public NoSuchMethodException() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>NoSuchMethodException</code> with a detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public NoSuchMethodException(String s) : base(s)
		{
		}
	}

}