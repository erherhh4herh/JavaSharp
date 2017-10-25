/*
 * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown to indicate that the requested operation is not supported.<para>
	/// 
	/// This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author  Josh Bloch
	/// @since   1.2
	/// </para>
	/// </summary>
	public class UnsupportedOperationException : RuntimeException
	{
		/// <summary>
		/// Constructs an UnsupportedOperationException with no detail message.
		/// </summary>
		public UnsupportedOperationException()
		{
		}

		/// <summary>
		/// Constructs an UnsupportedOperationException with the specified
		/// detail message.
		/// </summary>
		/// <param name="message"> the detail message </param>
		public UnsupportedOperationException(String message) : base(message)
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
		public UnsupportedOperationException(String message, Throwable cause) : base(message, cause)
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
		public UnsupportedOperationException(Throwable cause) : base(cause)
		{
		}

		internal new const long SerialVersionUID = -1242599979055084673L;
	}

}