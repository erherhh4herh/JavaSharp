/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{

	/// <summary>
	/// This is the general key management exception for all operations
	/// dealing with key management. Examples of subclasses of
	/// KeyManagementException that developers might create for
	/// giving more detailed information could include:
	/// 
	/// <ul>
	/// <li>KeyIDConflictException
	/// <li>KeyAuthorizationFailureException
	/// <li>ExpiredKeyException
	/// </ul>
	/// 
	/// @author Benjamin Renaud
	/// </summary>
	/// <seealso cref= Key </seealso>
	/// <seealso cref= KeyException </seealso>

	public class KeyManagementException : KeyException
	{

		private new const long SerialVersionUID = 947674216157062695L;

		/// <summary>
		/// Constructs a KeyManagementException with no detail message. A
		/// detail message is a String that describes this particular
		/// exception.
		/// </summary>
		public KeyManagementException() : base()
		{
		}

		 /// <summary>
		 /// Constructs a KeyManagementException with the specified detail
		 /// message. A detail message is a String that describes this
		 /// particular exception.
		 /// </summary>
		 /// <param name="msg"> the detail message. </param>
	   public KeyManagementException(String msg) : base(msg)
	   {
	   }

		/// <summary>
		/// Creates a {@code KeyManagementException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public KeyManagementException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a {@code KeyManagementException} with the specified cause
		/// and a detail message of {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of
		/// {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public KeyManagementException(Throwable cause) : base(cause)
		{
		}
	}

}