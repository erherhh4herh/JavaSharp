/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// An exception indicating one of a variety of problems encountered when
	/// building a certification path with a {@code CertPathBuilder}.
	/// <para>
	/// A {@code CertPathBuilderException} provides support for wrapping
	/// exceptions. The <seealso cref="#getCause getCause"/> method returns the throwable,
	/// if any, that caused this exception to be thrown.
	/// </para>
	/// <para>
	/// <b>Concurrent Access</b>
	/// </para>
	/// <para>
	/// Unless otherwise specified, the methods defined in this class are not
	/// thread-safe. Multiple threads that need to access a single
	/// object concurrently should synchronize amongst themselves and
	/// provide the necessary locking. Multiple threads each manipulating
	/// separate objects need not synchronize.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CertPathBuilder
	/// 
	/// @since       1.4
	/// @author      Sean Mullan </seealso>
	public class CertPathBuilderException : GeneralSecurityException
	{

		private new const long SerialVersionUID = 5316471420178794402L;

		/// <summary>
		/// Creates a {@code CertPathBuilderException} with {@code null}
		/// as its detail message.
		/// </summary>
		public CertPathBuilderException() : base()
		{
		}

		/// <summary>
		/// Creates a {@code CertPathBuilderException} with the given
		/// detail message. The detail message is a {@code String} that
		/// describes this particular exception in more detail.
		/// </summary>
		/// <param name="msg"> the detail message </param>
		public CertPathBuilderException(String msg) : base(msg)
		{
		}

		/// <summary>
		/// Creates a {@code CertPathBuilderException} that wraps the specified
		/// throwable. This allows any exception to be converted into a
		/// {@code CertPathBuilderException}, while retaining information
		/// about the wrapped exception, which may be useful for debugging. The
		/// detail message is set to ({@code cause==null ? null : cause.toString()})
		/// (which typically contains the class and detail message of
		/// cause).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		/// <seealso cref="#getCause getCause()"/> method). (A {@code null} value is
		/// permitted, and indicates that the cause is nonexistent or unknown.) </param>
		public CertPathBuilderException(Throwable cause) : base(cause)
		{
		}

		/// <summary>
		/// Creates a {@code CertPathBuilderException} with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="msg"> the detail message </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		/// <seealso cref="#getCause getCause()"/> method). (A {@code null} value is
		/// permitted, and indicates that the cause is nonexistent or unknown.) </param>
		public CertPathBuilderException(String msg, Throwable cause) : base(msg, cause)
		{
		}

	}

}