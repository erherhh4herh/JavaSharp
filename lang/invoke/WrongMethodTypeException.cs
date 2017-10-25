/*
 * Copyright (c) 2008, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.invoke
{

	/// <summary>
	/// Thrown to indicate that code has attempted to call a method handle
	/// via the wrong method type.  As with the bytecode representation of
	/// normal Java method calls, method handle calls are strongly typed
	/// to a specific type descriptor associated with a call site.
	/// <para>
	/// This exception may also be thrown when two method handles are
	/// composed, and the system detects that their types cannot be
	/// matched up correctly.  This amounts to an early evaluation
	/// of the type mismatch, at method handle construction time,
	/// instead of when the mismatched method handle is called.
	/// 
	/// @author John Rose, JSR 292 EG
	/// @since 1.7
	/// </para>
	/// </summary>
	public class WrongMethodTypeException : RuntimeException
	{
		private new const long SerialVersionUID = 292L;

		/// <summary>
		/// Constructs a {@code WrongMethodTypeException} with no detail message.
		/// </summary>
		public WrongMethodTypeException() : base()
		{
		}

		/// <summary>
		/// Constructs a {@code WrongMethodTypeException} with the specified
		/// detail message.
		/// </summary>
		/// <param name="s"> the detail message. </param>
		public WrongMethodTypeException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs a {@code WrongMethodTypeException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="s"> the detail message. </param>
		/// <param name="cause"> the cause of the exception, or null. </param>
		//FIXME: make this public in MR1
		/*non-public*/	 internal WrongMethodTypeException(String s, Throwable cause) : base(s, cause)
	 {
	 }

		/// <summary>
		/// Constructs a {@code WrongMethodTypeException} with the specified
		/// cause.
		/// </summary>
		/// <param name="cause"> the cause of the exception, or null. </param>
		//FIXME: make this public in MR1
		/*non-public*/	 internal WrongMethodTypeException(Throwable cause) : base(cause)
	 {
	 }
	}

}