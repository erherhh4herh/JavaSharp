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
	/// Certificate Expired Exception. This is thrown whenever the current
	/// {@code Date} or the specified {@code Date} is after the
	/// {@code notAfter} date/time specified in the validity period
	/// of the certificate.
	/// 
	/// @author Hemma Prafullchandra
	/// </summary>
	public class CertificateExpiredException : CertificateException
	{

		private new const long SerialVersionUID = 9071001339691533771L;

		/// <summary>
		/// Constructs a CertificateExpiredException with no detail message. A
		/// detail message is a String that describes this particular
		/// exception.
		/// </summary>
		public CertificateExpiredException() : base()
		{
		}

		/// <summary>
		/// Constructs a CertificateExpiredException with the specified detail
		/// message. A detail message is a String that describes this
		/// particular exception.
		/// </summary>
		/// <param name="message"> the detail message. </param>
		public CertificateExpiredException(String message) : base(message)
		{
		}
	}

}