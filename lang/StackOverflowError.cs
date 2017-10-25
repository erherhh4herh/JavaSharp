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
	/// Thrown when a stack overflow occurs because an application
	/// recurses too deeply.
	/// 
	/// @author unascribed
	/// @since   JDK1.0
	/// </summary>
	public class StackOverflowError : VirtualMachineError
	{
		private new const long SerialVersionUID = 8609175038441759607L;

		/// <summary>
		/// Constructs a <code>StackOverflowError</code> with no detail message.
		/// </summary>
		public StackOverflowError() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>StackOverflowError</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public StackOverflowError(String s) : base(s)
		{
		}
	}

}