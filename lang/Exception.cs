using System;

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
	/// The class {@code Exception} and its subclasses are a form of
	/// {@code Throwable} that indicates conditions that a reasonable
	/// application might want to catch.
	/// 
	/// <para>The class {@code Exception} and any subclasses that are not also
	/// subclasses of <seealso cref="RuntimeException"/> are <em>checked
	/// exceptions</em>.  Checked exceptions need to be declared in a
	/// method or constructor's {@code throws} clause if they can be thrown
	/// by the execution of the method or constructor and propagate outside
	/// the method or constructor boundary.
	/// 
	/// @author  Frank Yellin
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.Error
	/// @jls 11.2 Compile-Time Checking of Exceptions
	/// @since   JDK1.0 </seealso>
	public class Exception : Throwable
	{
		internal const long SerialVersionUID = -3387516993124229948L;

		/// <summary>
		/// Constructs a new exception with {@code null} as its detail message.
		/// The cause is not initialized, and may subsequently be initialized by a
		/// call to <seealso cref="#initCause"/>.
		/// </summary>
		public Exception() : base()
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified detail message.  The
		/// cause is not initialized, and may subsequently be initialized by
		/// a call to <seealso cref="#initCause"/>.
		/// </summary>
		/// <param name="message">   the detail message. The detail message is saved for
		///          later retrieval by the <seealso cref="#getMessage()"/> method. </param>
		public Exception(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified detail message and
		/// cause.  <para>Note that the detail message associated with
		/// {@code cause} is <i>not</i> automatically incorporated in
		/// this exception's detail message.
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
		public Exception(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified cause and a detail
		/// message of <tt>(cause==null ? null : cause.toString())</tt> (which
		/// typically contains the class and detail message of <tt>cause</tt>).
		/// This constructor is useful for exceptions that are little more than
		/// wrappers for other throwables (for example, {@link
		/// java.security.PrivilegedActionException}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.4 </param>
		public Exception(Throwable cause) : base(cause)
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified detail message,
		/// cause, suppression enabled or disabled, and writable stack
		/// trace enabled or disabled.
		/// </summary>
		/// <param name="message"> the detail message. </param>
		/// <param name="cause"> the cause.  (A {@code null} value is permitted,
		/// and indicates that the cause is nonexistent or unknown.) </param>
		/// <param name="enableSuppression"> whether or not suppression is enabled
		///                          or disabled </param>
		/// <param name="writableStackTrace"> whether or not the stack trace should
		///                           be writable
		/// @since 1.7 </param>
		protected internal Exception(String message, Throwable cause, bool enableSuppression, bool writableStackTrace) : base(message, cause, enableSuppression, writableStackTrace)
		{
		}
	}

}