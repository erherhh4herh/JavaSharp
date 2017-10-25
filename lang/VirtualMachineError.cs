/*
 * Copyright (c) 1995, 2011, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown to indicate that the Java Virtual Machine is broken or has
	/// run out of resources necessary for it to continue operating.
	/// 
	/// 
	/// @author  Frank Yellin
	/// @since   JDK1.0
	/// </summary>
	public abstract class VirtualMachineError : Error
	{
		private new const long SerialVersionUID = 4161983926571568670L;

		/// <summary>
		/// Constructs a <code>VirtualMachineError</code> with no detail message.
		/// </summary>
		public VirtualMachineError() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>VirtualMachineError</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="message">   the detail message. </param>
		public VirtualMachineError(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a {@code VirtualMachineError} with the specified
		/// detail message and cause.  <para>Note that the detail message
		/// associated with {@code cause} is <i>not</i> automatically
		/// incorporated in this error's detail message.
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
		public VirtualMachineError(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Constructs an a {@code VirtualMachineError} with the specified
		/// cause and a detail message of {@code (cause==null ? null :
		/// cause.toString())} (which typically contains the class and
		/// detail message of {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.8 </param>
		public VirtualMachineError(Throwable cause) : base(cause)
		{
		}
	}

}