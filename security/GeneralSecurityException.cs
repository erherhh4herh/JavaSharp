using System;

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

namespace java.security
{

	/// <summary>
	/// The {@code GeneralSecurityException} class is a generic
	/// security exception class that provides type safety for all the
	/// security-related exception classes that extend from it.
	/// 
	/// @author Jan Luehe
	/// </summary>

	public class GeneralSecurityException : Exception
	{

		private new const long SerialVersionUID = 894798122053539237L;

		/// <summary>
		/// Constructs a GeneralSecurityException with no detail message.
		/// </summary>
		public GeneralSecurityException() : base()
		{
		}

		/// <summary>
		/// Constructs a GeneralSecurityException with the specified detail
		/// message.
		/// A detail message is a String that describes this particular
		/// exception.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
		public GeneralSecurityException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Creates a {@code GeneralSecurityException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public GeneralSecurityException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a {@code GeneralSecurityException} with the specified cause
		/// and a detail message of {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of
		/// {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public GeneralSecurityException(Throwable cause) : base(cause)
		{
		}
	}

}