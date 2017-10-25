/*
 * Copyright (c) 1994, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown when the Java Virtual Machine cannot allocate an object
	/// because it is out of memory, and no more memory could be made
	/// available by the garbage collector.
	/// 
	/// {@code OutOfMemoryError} objects may be constructed by the virtual
	/// machine as if {@link Throwable#Throwable(String, Throwable,
	/// boolean, boolean) suppression were disabled and/or the stack trace was not
	/// writable}.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class OutOfMemoryError : VirtualMachineError
	{
		private new const long SerialVersionUID = 8228564086184010517L;

		/// <summary>
		/// Constructs an {@code OutOfMemoryError} with no detail message.
		/// </summary>
		public OutOfMemoryError() : base()
		{
		}

		/// <summary>
		/// Constructs an {@code OutOfMemoryError} with the specified
		/// detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public OutOfMemoryError(String s) : base(s)
		{
		}
	}

}