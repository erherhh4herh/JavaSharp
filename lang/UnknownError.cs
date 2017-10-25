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
	/// Thrown when an unknown but serious exception has occurred in the
	/// Java Virtual Machine.
	/// 
	/// @author unascribed
	/// @since   JDK1.0
	/// </summary>
	public class UnknownError : VirtualMachineError
	{
		private new const long SerialVersionUID = 2524784860676771849L;

		/// <summary>
		/// Constructs an <code>UnknownError</code> with no detail message.
		/// </summary>
		public UnknownError() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>UnknownError</code> with the specified detail
		/// message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public UnknownError(String s) : base(s)
		{
		}
	}

}