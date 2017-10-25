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

namespace java.security.cert
{

	/// <summary>
	/// Certificate Encoding Exception. This is thrown whenever an error
	/// occurs while attempting to encode a certificate.
	/// 
	/// @author Hemma Prafullchandra
	/// </summary>
	public class CertificateEncodingException : CertificateException
	{

		private new const long SerialVersionUID = 6219492851589449162L;

		/// <summary>
		/// Constructs a CertificateEncodingException with no detail message. A
		/// detail message is a String that describes this particular
		/// exception.
		/// </summary>
		public CertificateEncodingException() : base()
		{
		}

		/// <summary>
		/// Constructs a CertificateEncodingException with the specified detail
		/// message. A detail message is a String that describes this
		/// particular exception.
		/// </summary>
		/// <param name="message"> the detail message. </param>
		public CertificateEncodingException(String message) : base(message)
		{
		}

		/// <summary>
		/// Creates a {@code CertificateEncodingException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public CertificateEncodingException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a {@code CertificateEncodingException}
		/// with the specified cause and a detail message of
		/// {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of
		/// {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public CertificateEncodingException(Throwable cause) : base(cause)
		{
		}
	}

}