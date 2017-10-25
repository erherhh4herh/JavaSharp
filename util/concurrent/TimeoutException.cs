using System;

/*
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

/*
 *
 *
 *
 *
 *
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{

	/// <summary>
	/// Exception thrown when a blocking operation times out.  Blocking
	/// operations for which a timeout is specified need a means to
	/// indicate that the timeout has occurred. For many such operations it
	/// is possible to return a value that indicates timeout; when that is
	/// not possible or desirable then {@code TimeoutException} should be
	/// declared and thrown.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </summary>
	public class TimeoutException : Exception
	{
		private new const long SerialVersionUID = 1900926677490660714L;

		/// <summary>
		/// Constructs a {@code TimeoutException} with no specified detail
		/// message.
		/// </summary>
		public TimeoutException()
		{
		}

		/// <summary>
		/// Constructs a {@code TimeoutException} with the specified detail
		/// message.
		/// </summary>
		/// <param name="message"> the detail message </param>
		public TimeoutException(String message) : base(message)
		{
		}
	}

}