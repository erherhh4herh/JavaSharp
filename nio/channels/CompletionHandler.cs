/*
 * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.channels
{

	/// <summary>
	/// A handler for consuming the result of an asynchronous I/O operation.
	/// 
	/// <para> The asynchronous channels defined in this package allow a completion
	/// handler to be specified to consume the result of an asynchronous operation.
	/// The <seealso cref="#completed completed"/> method is invoked when the I/O operation
	/// completes successfully. The <seealso cref="#failed failed"/> method is invoked if the
	/// I/O operations fails. The implementations of these methods should complete
	/// in a timely manner so as to avoid keeping the invoking thread from dispatching
	/// to other completion handlers.
	/// 
	/// </para>
	/// </summary>
	/// @param   <V>     The result type of the I/O operation </param>
	/// @param   <A>     The type of the object attached to the I/O operation
	/// 
	/// @since 1.7 </param>

	public interface CompletionHandler<V, A>
	{

		/// <summary>
		/// Invoked when an operation has completed.
		/// </summary>
		/// <param name="result">
		///          The result of the I/O operation. </param>
		/// <param name="attachment">
		///          The object attached to the I/O operation when it was initiated. </param>
		void Completed(V result, A attachment);

		/// <summary>
		/// Invoked when an operation fails.
		/// </summary>
		/// <param name="exc">
		///          The exception to indicate why the I/O operation failed </param>
		/// <param name="attachment">
		///          The object attached to the I/O operation when it was initiated. </param>
		void Failed(Throwable exc, A attachment);
	}

}