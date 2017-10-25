/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security.spec
{

	/// <summary>
	/// This is the exception for invalid key specifications.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= KeySpec
	/// 
	/// @since 1.2 </seealso>

	public class InvalidKeySpecException : GeneralSecurityException
	{

		private new const long SerialVersionUID = 3546139293998810778L;

		/// <summary>
		/// Constructs an InvalidKeySpecException with no detail message. A
		/// detail message is a String that describes this particular
		/// exception.
		/// </summary>
		public InvalidKeySpecException() : base()
		{
		}

		/// <summary>
		/// Constructs an InvalidKeySpecException with the specified detail
		/// message. A detail message is a String that describes this
		/// particular exception.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
		public InvalidKeySpecException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Creates a {@code InvalidKeySpecException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public InvalidKeySpecException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a {@code InvalidKeySpecException} with the specified cause
		/// and a detail message of {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of
		/// {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public InvalidKeySpecException(Throwable cause) : base(cause)
		{
		}
	}

}