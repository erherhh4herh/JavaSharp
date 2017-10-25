using System;

/*
 * Copyright (c) 2009, Oracle and/or its affiliates. All rights reserved.
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
	/// Common superclass of exceptions thrown by reflective operations in
	/// core reflection.
	/// </summary>
	/// <seealso cref= LinkageError
	/// @since 1.7 </seealso>
	public class ReflectiveOperationException : Exception
	{
		internal new const long SerialVersionUID = 123456789L;

		/// <summary>
		/// Constructs a new exception with {@code null} as its detail
		/// message.  The cause is not initialized, and may subsequently be
		/// initialized by a call to <seealso cref="#initCause"/>.
		/// </summary>
		public ReflectiveOperationException() : base()
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified detail message.
		/// The cause is not initialized, and may subsequently be
		/// initialized by a call to <seealso cref="#initCause"/>.
		/// </summary>
		/// <param name="message">   the detail message. The detail message is saved for
		///          later retrieval by the <seealso cref="#getMessage()"/> method. </param>
		public ReflectiveOperationException(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified detail message
		/// and cause.
		/// 
		/// <para>Note that the detail message associated with
		/// {@code cause} is <em>not</em> automatically incorporated in
		/// this exception's detail message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///         by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.) </param>
		public ReflectiveOperationException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified cause and a detail
		/// message of {@code (cause==null ? null : cause.toString())} (which
		/// typically contains the class and detail message of {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.) </param>
		public ReflectiveOperationException(Throwable cause) : base(cause)
		{
		}
	}

}