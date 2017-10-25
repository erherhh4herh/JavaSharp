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
	/// Exception thrown when an error or other exception is encountered
	/// in the course of completing a result or task.
	/// 
	/// @since 1.8
	/// @author Doug Lea
	/// </summary>
	public class CompletionException : RuntimeException
	{
		private new const long SerialVersionUID = 7830266012832686185L;

		/// <summary>
		/// Constructs a {@code CompletionException} with no detail message.
		/// The cause is not initialized, and may subsequently be
		/// initialized by a call to <seealso cref="#initCause(Throwable) initCause"/>.
		/// </summary>
		protected internal CompletionException()
		{
		}

		/// <summary>
		/// Constructs a {@code CompletionException} with the specified detail
		/// message. The cause is not initialized, and may subsequently be
		/// initialized by a call to <seealso cref="#initCause(Throwable) initCause"/>.
		/// </summary>
		/// <param name="message"> the detail message </param>
		protected internal CompletionException(String message) : base(message)
		{
		}

		/// <summary>
		/// Constructs a {@code CompletionException} with the specified detail
		/// message and cause.
		/// </summary>
		/// <param name="message"> the detail message </param>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method) </param>
		public CompletionException(String message, Throwable cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Constructs a {@code CompletionException} with the specified cause.
		/// The detail message is set to {@code (cause == null ? null :
		/// cause.toString())} (which typically contains the class and
		/// detail message of {@code cause}).
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method) </param>
		public CompletionException(Throwable cause) : base(cause)
		{
		}
	}

}