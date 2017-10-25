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
	/// CRL (Certificate Revocation List) Exception.
	/// 
	/// @author Hemma Prafullchandra
	/// </summary>
	public class CRLException : GeneralSecurityException
	{

		private new const long SerialVersionUID = -6694728944094197147L;

	   /// <summary>
	   /// Constructs a CRLException with no detail message. A
	   /// detail message is a String that describes this particular
	   /// exception.
	   /// </summary>
		public CRLException() : base()
		{
		}

		/// <summary>
		/// Constructs a CRLException with the specified detail
		/// message. A detail message is a String that describes this
		/// particular exception.
		/// </summary>
		/// <param name="message"> the detail message. </param>
		public CRLException(String message) : base(message)
		{
		}

		/// <summary>
		/// Creates a {@code CRLException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public CRLException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a {@code CRLException} with the specified cause
		/// and a detail message of {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of
		/// {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public CRLException(Throwable cause) : base(cause)
		{
		}
	}

}