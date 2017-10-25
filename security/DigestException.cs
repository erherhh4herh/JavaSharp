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
	/// This is the generic Message Digest exception.
	/// 
	/// @author Benjamin Renaud
	/// </summary>
	public class DigestException : GeneralSecurityException
	{

		private new const long SerialVersionUID = 5821450303093652515L;

		/// <summary>
		/// Constructs a DigestException with no detail message.  (A
		/// detail message is a String that describes this particular
		/// exception.)
		/// </summary>
		public DigestException() : base()
		{
		}

		/// <summary>
		/// Constructs a DigestException with the specified detail
		/// message.  (A detail message is a String that describes this
		/// particular exception.)
		/// </summary>
		/// <param name="msg"> the detail message. </param>
	   public DigestException(String msg) : base(msg)
	   {
	   }

		/// <summary>
		/// Creates a {@code DigestException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message"> the detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method). </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public DigestException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a {@code DigestException} with the specified cause
		/// and a detail message of {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of
		/// {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// @since 1.5 </param>
		public DigestException(Throwable cause) : base(cause)
		{
		}
	}

}