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
	/// Thrown to indicate some unexpected internal error has occurred in
	/// the Java Virtual Machine.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class InternalError : VirtualMachineError
	{
		private new const long SerialVersionUID = -9062593416125562365L;

		/// <summary>
		/// Constructs an <code>InternalError</code> with no detail message.
		/// </summary>
		public InternalError() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>InternalError</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="message">   the detail message. </param>
		public InternalError(String message) : base(message)
		{
		}


		/// <summary>
		/// Constructs an {@code InternalError} with the specified detail
		/// message and cause.  <para>Note that the detail message associated
		/// with {@code cause} is <i>not</i> automatically incorporated in
		/// this error's detail message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///         by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.8 </param>
		public InternalError(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Constructs an {@code InternalError} with the specified cause
		/// and a detail message of {@code (cause==null ? null :
		/// cause.toString())} (which typically contains the class and
		/// detail message of {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.8 </param>
		public InternalError(Throwable cause) : base(cause)
		{
		}

	}

}