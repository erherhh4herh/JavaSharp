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
	/// This is the generic Signature exception.
	/// 
	/// @author Benjamin Renaud
	/// </summary>

	public class SignatureException : GeneralSecurityException
	{

		private new const long SerialVersionUID = 7509989324975124438L;

		/// <summary>
		/// Constructs a SignatureException with no detail message. A
		/// detail message is a String that describes this particular
		/// exception.
		/// </summary>
		public SignatureException() : base()
		{
		}

		/// <summary>
		/// Constructs a SignatureException with the specified detail
		/// message.  A detail message is a String that describes this
		/// particular exception.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
		public SignatureException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Creates a {@code SignatureException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public SignatureException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a {@code SignatureException} with the specified cause
		/// and a detail message of {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of
		/// {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public SignatureException(Throwable cause) : base(cause)
		{
		}
	}

}