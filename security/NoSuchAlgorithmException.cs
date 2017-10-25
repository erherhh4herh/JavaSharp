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
	/// This exception is thrown when a particular cryptographic algorithm is
	/// requested but is not available in the environment.
	/// 
	/// @author Benjamin Renaud
	/// </summary>

	public class NoSuchAlgorithmException : GeneralSecurityException
	{

		private new const long SerialVersionUID = -7443947487218346562L;

		/// <summary>
		/// Constructs a NoSuchAlgorithmException with no detail
		/// message. A detail message is a String that describes this
		/// particular exception.
		/// </summary>
		public NoSuchAlgorithmException() : base()
		{
		}

		/// <summary>
		/// Constructs a NoSuchAlgorithmException with the specified
		/// detail message. A detail message is a String that describes
		/// this particular exception, which may, for example, specify which
		/// algorithm is not available.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
		public NoSuchAlgorithmException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Creates a {@code NoSuchAlgorithmException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public NoSuchAlgorithmException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a {@code NoSuchAlgorithmException} with the specified cause
		/// and a detail message of {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of
		/// {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public NoSuchAlgorithmException(Throwable cause) : base(cause)
		{
		}
	}

}