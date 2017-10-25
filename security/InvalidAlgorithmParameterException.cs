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
	/// This is the exception for invalid or inappropriate algorithm parameters.
	/// 
	/// @author Jan Luehe
	/// 
	/// </summary>
	/// <seealso cref= AlgorithmParameters </seealso>
	/// <seealso cref= java.security.spec.AlgorithmParameterSpec
	/// 
	/// @since 1.2 </seealso>

	public class InvalidAlgorithmParameterException : GeneralSecurityException
	{

		private new const long SerialVersionUID = 2864672297499471472L;

		/// <summary>
		/// Constructs an InvalidAlgorithmParameterException with no detail
		/// message.
		/// A detail message is a String that describes this particular
		/// exception.
		/// </summary>
		public InvalidAlgorithmParameterException() : base()
		{
		}

		/// <summary>
		/// Constructs an InvalidAlgorithmParameterException with the specified
		/// detail message.
		/// A detail message is a String that describes this
		/// particular exception.
		/// </summary>
		/// <param name="msg"> the detail message. </param>
		public InvalidAlgorithmParameterException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Creates a {@code InvalidAlgorithmParameterException} with the
		/// specified detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public InvalidAlgorithmParameterException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a {@code InvalidAlgorithmParameterException} with the
		/// specified cause and a detail message of
		/// {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of
		/// {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public InvalidAlgorithmParameterException(Throwable cause) : base(cause)
		{
		}
	}

}