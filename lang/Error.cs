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
	/// An {@code Error} is a subclass of {@code Throwable}
	/// that indicates serious problems that a reasonable application
	/// should not try to catch. Most such errors are abnormal conditions.
	/// The {@code ThreadDeath} error, though a "normal" condition,
	/// is also a subclass of {@code Error} because most applications
	/// should not try to catch it.
	/// <para>
	/// A method is not required to declare in its {@code throws}
	/// clause any subclasses of {@code Error} that might be thrown
	/// during the execution of the method but not caught, since these
	/// errors are abnormal conditions that should never occur.
	/// 
	/// That is, {@code Error} and its subclasses are regarded as unchecked
	/// exceptions for the purposes of compile-time checking of exceptions.
	/// 
	/// @author  Frank Yellin
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.ThreadDeath
	/// @jls 11.2 Compile-Time Checking of Exceptions
	/// @since   JDK1.0 </seealso>
	public class Error : Throwable
	{
		internal const long SerialVersionUID = 4980196508277280342L;

		/// <summary>
		/// Constructs a new error with {@code null} as its detail message.
		/// The cause is not initialized, and may subsequently be initialized by a
		/// call to <seealso cref="#initCause"/>.
		/// </summary>
		public Error() : base()
		{
		}

		/// <summary>
		/// Constructs a new error with the specified detail message.  The
		/// cause is not initialized, and may subsequently be initialized by
		/// a call to <seealso cref="#initCause"/>.
		/// </summary>
		/// <param name="message">   the detail message. The detail message is saved for
		///          later retrieval by the <seealso cref="#getMessage()"/> method. </param>
		public Error(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a new error with the specified detail message and
		/// cause.  <para>Note that the detail message associated with
		/// {@code cause} is <i>not</i> automatically incorporated in
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
		/// @since  1.4 </param>
		public Error(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Constructs a new error with the specified cause and a detail
		/// message of {@code (cause==null ? null : cause.toString())} (which
		/// typically contains the class and detail message of {@code cause}).
		/// This constructor is useful for errors that are little more than
		/// wrappers for other throwables.
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.4 </param>
		public Error(Throwable cause) : base(cause)
		{
		}

		/// <summary>
		/// Constructs a new error with the specified detail message,
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
		/// 
		/// @since 1.7 </param>
		protected internal Error(String message, Throwable cause, bool enableSuppression, bool writableStackTrace) : base(message, cause, enableSuppression, writableStackTrace)
		{
		}
	}

}