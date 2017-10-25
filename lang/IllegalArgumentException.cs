/*
 * Copyright (c) 1994, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown to indicate that a method has been passed an illegal or
	/// inappropriate argument.
	/// 
	/// @author  unascribed
	/// @since   JDK1.0
	/// </summary>
	public class IllegalArgumentException : RuntimeException
	{
		/// <summary>
		/// Constructs an <code>IllegalArgumentException</code> with no
		/// detail message.
		/// </summary>
		public IllegalArgumentException() : base()
		{
		}

		/// <summary>
		/// Constructs an <code>IllegalArgumentException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public IllegalArgumentException(String s) : base(s)
		{
		}

		/// <summary>
		/// Constructs a new exception with the specified detail message and
		/// cause.
		/// 
		/// <para>Note that the detail message associated with <code>cause</code> is
		/// <i>not</i> automatically incorporated in this exception's detail
		/// message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///         by the <seealso cref="Throwable#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="Throwable#getCause()"/> method).  (A <tt>null</tt> value
		///         is permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since 1.5 </param>
		public IllegalArgumentException(String message, Throwable cause) : base(message, cause)
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
		///         <seealso cref="Throwable#getCause()"/> method).  (A <tt>null</tt> value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.)
		/// @since  1.5 </param>
		public IllegalArgumentException(Throwable cause) : base(cause)
		{
		}

		private new const long SerialVersionUID = -5365630128856068164L;
	}

}