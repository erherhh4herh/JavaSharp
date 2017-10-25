using System;

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
	/// {@code RuntimeException} is the superclass of those
	/// exceptions that can be thrown during the normal operation of the
	/// Java Virtual Machine.
	/// 
	/// <para>{@code RuntimeException} and its subclasses are <em>unchecked
	/// exceptions</em>.  Unchecked exceptions do <em>not</em> need to be
	/// declared in a method or constructor's {@code throws} clause if they
	/// can be thrown by the execution of the method or constructor and
	/// propagate outside the method or constructor boundary.
	/// 
	/// @author  Frank Yellin
	/// @jls 11.2 Compile-Time Checking of Exceptions
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public class RuntimeException : Exception
	{
		internal new const long SerialVersionUID = -7034897190745766939L;

		/// <summary>
		/// Constructs a new runtime exception with {@code null} as its
		/// detail message.  The cause is not initialized, and may subsequently be
		/// initialized by a call to <seealso cref="#initCause"/>.
		/// </summary>
		public RuntimeException() : base()
		{
		}

		/// <summary>
		/// Constructs a new runtime exception with the specified detail message.
		/// The cause is not initialized, and may subsequently be initialized by a
		/// call to <seealso cref="#initCause"/>.
		/// </summary>
		/// <param name="message">   the detail message. The detail message is saved for
		///          later retrieval by the <seealso cref="#getMessage()"/> method. </param>
		public RuntimeException(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a new runtime exception with the specified detail message and
		/// cause.  <para>Note that the detail message associated with
		/// {@code cause} is <i>not</i> automatically incorporated in
		/// this runtime exception's detail message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///         by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.4 </param>
		public RuntimeException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Constructs a new runtime exception with the specified cause and a
		/// detail message of <tt>(cause==null ? null : cause.toString())</tt>
		/// (which typically contains the class and detail message of
		/// <tt>cause</tt>).  This constructor is useful for runtime exceptions
		/// that are little more than wrappers for other throwables.
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.4 </param>
		public RuntimeException(Throwable cause) : base(cause)
		{
		}

		/// <summary>
		/// Constructs a new runtime exception with the specified detail
		/// message, cause, suppression enabled or disabled, and writable
		/// stack trace enabled or disabled.
		/// </summary>
		/// <param name="message"> the detail message. </param>
		/// <param name="cause"> the cause.  (A {@code null} value is permitted,
		/// and indicates that the cause is nonexistent or unknown.) </param>
		/// <param name="enableSuppression"> whether or not suppression is enabled
		///                          or disabled </param>
		/// <param name="writableStackTrace"> whether or not the stack trace should
		///                           be writable
		/// 
		/// @since 1.7 </param>
		protected internal RuntimeException(String message, Throwable cause, bool enableSuppression, bool writableStackTrace) : base(message, cause, enableSuppression, writableStackTrace)
		{
		}
	}

}