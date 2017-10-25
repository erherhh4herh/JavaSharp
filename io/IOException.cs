using System;

/*
 * Copyright (c) 1994, 2006, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	/// <summary>
	/// Signals that an I/O exception of some sort has occurred. This
	/// class is the general class of exceptions produced by failed or
	/// interrupted I/O operations.
	/// 
	/// @author  unascribed </summary>
	/// <seealso cref=     java.io.InputStream </seealso>
	/// <seealso cref=     java.io.OutputStream
	/// @since   JDK1.0 </seealso>
	public class IOException : Exception
	{
		internal new const long SerialVersionUID = 7818375828146090155L;

		/// <summary>
		/// Constructs an {@code IOException} with {@code null}
		/// as its error detail message.
		/// </summary>
		public IOException() : base()
		{
		}

		/// <summary>
		/// Constructs an {@code IOException} with the specified detail message.
		/// </summary>
		/// <param name="message">
		///        The detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method) </param>
		public IOException(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs an {@code IOException} with the specified detail message
		/// and cause.
		/// 
		/// <para> Note that the detail message associated with {@code cause} is
		/// <i>not</i> automatically incorporated into this exception's detail
		/// message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="message">
		///        The detail message (which is saved for later retrieval
		///        by the <seealso cref="#getMessage()"/> method)
		/// </param>
		/// <param name="cause">
		///        The cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A null value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// 
		/// @since 1.6 </param>
		public IOException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Constructs an {@code IOException} with the specified cause and a
		/// detail message of {@code (cause==null ? null : cause.toString())}
		/// (which typically contains the class and detail message of {@code cause}).
		/// This constructor is useful for IO exceptions that are little more
		/// than wrappers for other throwables.
		/// </summary>
		/// <param name="cause">
		///        The cause (which is saved for later retrieval by the
		///        <seealso cref="#getCause()"/> method).  (A null value is permitted,
		///        and indicates that the cause is nonexistent or unknown.)
		/// 
		/// @since 1.6 </param>
		public IOException(Throwable cause) : base(cause)
		{
		}
	}

}