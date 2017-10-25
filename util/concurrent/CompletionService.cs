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
	/// A service that decouples the production of new asynchronous tasks
	/// from the consumption of the results of completed tasks.  Producers
	/// {@code submit} tasks for execution. Consumers {@code take}
	/// completed tasks and process their results in the order they
	/// complete.  A {@code CompletionService} can for example be used to
	/// manage asynchronous I/O, in which tasks that perform reads are
	/// submitted in one part of a program or system, and then acted upon
	/// in a different part of the program when the reads complete,
	/// possibly in a different order than they were requested.
	/// 
	/// <para>Typically, a {@code CompletionService} relies on a separate
	/// <seealso cref="Executor"/> to actually execute the tasks, in which case the
	/// {@code CompletionService} only manages an internal completion
	/// queue. The <seealso cref="ExecutorCompletionService"/> class provides an
	/// implementation of this approach.
	/// 
	/// </para>
	/// <para>Memory consistency effects: Actions in a thread prior to
	/// submitting a task to a {@code CompletionService}
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// actions taken by that task, which in turn <i>happen-before</i>
	/// actions following a successful return from the corresponding {@code take()}.
	/// </para>
	/// </summary>
	public interface CompletionService<V>
	{
		/// <summary>
		/// Submits a value-returning task for execution and returns a Future
		/// representing the pending results of the task.  Upon completion,
		/// this task may be taken or polled.
		/// </summary>
		/// <param name="task"> the task to submit </param>
		/// <returns> a Future representing pending completion of the task </returns>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		/// <exception cref="NullPointerException"> if the task is null </exception>
		Future<V> Submit(Callable<V> task);

		/// <summary>
		/// Submits a Runnable task for execution and returns a Future
		/// representing that task.  Upon completion, this task may be
		/// taken or polled.
		/// </summary>
		/// <param name="task"> the task to submit </param>
		/// <param name="result"> the result to return upon successful completion </param>
		/// <returns> a Future representing pending completion of the task,
		///         and whose {@code get()} method will return the given
		///         result value upon completion </returns>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		/// <exception cref="NullPointerException"> if the task is null </exception>
		Future<V> Submit(Runnable task, V result);

		/// <summary>
		/// Retrieves and removes the Future representing the next
		/// completed task, waiting if none are yet present.
		/// </summary>
		/// <returns> the Future representing the next completed task </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Future<V> take() throws InterruptedException;
		Future<V> Take();

		/// <summary>
		/// Retrieves and removes the Future representing the next
		/// completed task, or {@code null} if none are present.
		/// </summary>
		/// <returns> the Future representing the next completed task, or
		///         {@code null} if none are present </returns>
		Future<V> Poll();

		/// <summary>
		/// Retrieves and removes the Future representing the next
		/// completed task, waiting if necessary up to the specified wait
		/// time if none are yet present.
		/// </summary>
		/// <param name="timeout"> how long to wait before giving up, in units of
		///        {@code unit} </param>
		/// <param name="unit"> a {@code TimeUnit} determining how to interpret the
		///        {@code timeout} parameter </param>
		/// <returns> the Future representing the next completed task or
		///         {@code null} if the specified waiting time elapses
		///         before one is present </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Future<V> poll(long timeout, TimeUnit unit) throws InterruptedException;
		Future<V> Poll(long timeout, TimeUnit unit);
	}

}