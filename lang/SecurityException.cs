/*
 * Copyright (c) 1995, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// Thrown by the security manager to indicate a security violation.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.lang.SecurityManager
	/// @since   JDK1.0 </seealso>
	public class SecurityException : RuntimeException
	{

		private new const long SerialVersionUID = 6878364983674394167L;

		/// <summary>
		/// Constructs a <code>SecurityException</code> with no detail  message.
		/// </summary>
		public SecurityException() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>SecurityException</code> with the specified
		/// detail message.
		/// </summary>
		/// <param name="s">   the detail message. </param>
		public SecurityException(String s) : base(s)
		{
		}

		/// <summary>
		/// Creates a <code>SecurityException</code> with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public SecurityException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a <code>SecurityException</code> with the specified cause
		/// and a detail message of <tt>(cause==null ? null : cause.toString())</tt>
		/// (which typically contains the class and detail message of
		/// <tt>cause</tt>).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public SecurityException(Throwable cause) : base(cause)
		{
		}
	}

}