using System.Collections.Generic;

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
	/// An <seealso cref="Executor"/> that provides methods to manage termination and
	/// methods that can produce a <seealso cref="Future"/> for tracking progress of
	/// one or more asynchronous tasks.
	/// 
	/// <para>An {@code ExecutorService} can be shut down, which will cause
	/// it to reject new tasks.  Two different methods are provided for
	/// shutting down an {@code ExecutorService}. The <seealso cref="#shutdown"/>
	/// method will allow previously submitted tasks to execute before
	/// terminating, while the <seealso cref="#shutdownNow"/> method prevents waiting
	/// tasks from starting and attempts to stop currently executing tasks.
	/// Upon termination, an executor has no tasks actively executing, no
	/// tasks awaiting execution, and no new tasks can be submitted.  An
	/// unused {@code ExecutorService} should be shut down to allow
	/// reclamation of its resources.
	/// 
	/// </para>
	/// <para>Method {@code submit} extends base method {@link
	/// Executor#execute(Runnable)} by creating and returning a <seealso cref="Future"/>
	/// that can be used to cancel execution and/or wait for completion.
	/// Methods {@code invokeAny} and {@code invokeAll} perform the most
	/// commonly useful forms of bulk execution, executing a collection of
	/// tasks and then waiting for at least one, or all, to
	/// complete. (Class <seealso cref="ExecutorCompletionService"/> can be used to
	/// write customized variants of these methods.)
	/// 
	/// </para>
	/// <para>The <seealso cref="Executors"/> class provides factory methods for the
	/// executor services provided in this package.
	/// 
	/// <h3>Usage Examples</h3>
	/// 
	/// Here is a sketch of a network service in which threads in a thread
	/// pool service incoming requests. It uses the preconfigured {@link
	/// Executors#newFixedThreadPool} factory method:
	/// 
	///  <pre> {@code
	/// class NetworkService implements Runnable {
	///   private final ServerSocket serverSocket;
	///   private final ExecutorService pool;
	/// 
	///   public NetworkService(int port, int poolSize)
	///       throws IOException {
	///     serverSocket = new ServerSocket(port);
	///     pool = Executors.newFixedThreadPool(poolSize);
	///   }
	/// 
	///   public void run() { // run the service
	///     try {
	///       for (;;) {
	///         pool.execute(new Handler(serverSocket.accept()));
	///       }
	///     } catch (IOException ex) {
	///       pool.shutdown();
	///     }
	///   }
	/// }
	/// 
	/// class Handler implements Runnable {
	///   private final Socket socket;
	///   Handler(Socket socket) { this.socket = socket; }
	///   public void run() {
	///     // read and service request on socket
	///   }
	/// }}</pre>
	/// 
	/// The following method shuts down an {@code ExecutorService} in two phases,
	/// first by calling {@code shutdown} to reject incoming tasks, and then
	/// calling {@code shutdownNow}, if necessary, to cancel any lingering tasks:
	/// 
	///  <pre> {@code
	/// void shutdownAndAwaitTermination(ExecutorService pool) {
	///   pool.shutdown(); // Disable new tasks from being submitted
	///   try {
	///     // Wait a while for existing tasks to terminate
	///     if (!pool.awaitTermination(60, TimeUnit.SECONDS)) {
	///       pool.shutdownNow(); // Cancel currently executing tasks
	///       // Wait a while for tasks to respond to being cancelled
	///       if (!pool.awaitTermination(60, TimeUnit.SECONDS))
	///           System.err.println("Pool did not terminate");
	///     }
	///   } catch (InterruptedException ie) {
	///     // (Re-)Cancel if current thread also interrupted
	///     pool.shutdownNow();
	///     // Preserve interrupt status
	///     Thread.currentThread().interrupt();
	///   }
	/// }}</pre>
	/// 
	/// </para>
	/// <para>Memory consistency effects: Actions in a thread prior to the
	/// submission of a {@code Runnable} or {@code Callable} task to an
	/// {@code ExecutorService}
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// any actions taken by that task, which in turn <i>happen-before</i> the
	/// result is retrieved via {@code Future.get()}.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public interface ExecutorService : Executor
	{

		/// <summary>
		/// Initiates an orderly shutdown in which previously submitted
		/// tasks are executed, but no new tasks will be accepted.
		/// Invocation has no additional effect if already shut down.
		/// 
		/// <para>This method does not wait for previously submitted tasks to
		/// complete execution.  Use <seealso cref="#awaitTermination awaitTermination"/>
		/// to do that.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException"> if a security manager exists and
		///         shutting down this ExecutorService may manipulate
		///         threads that the caller is not permitted to modify
		///         because it does not hold {@link
		///         java.lang.RuntimePermission}{@code ("modifyThread")},
		///         or the security manager's {@code checkAccess} method
		///         denies access. </exception>
		void Shutdown();

		/// <summary>
		/// Attempts to stop all actively executing tasks, halts the
		/// processing of waiting tasks, and returns a list of the tasks
		/// that were awaiting execution.
		/// 
		/// <para>This method does not wait for actively executing tasks to
		/// terminate.  Use <seealso cref="#awaitTermination awaitTermination"/> to
		/// do that.
		/// 
		/// </para>
		/// <para>There are no guarantees beyond best-effort attempts to stop
		/// processing actively executing tasks.  For example, typical
		/// implementations will cancel via <seealso cref="Thread#interrupt"/>, so any
		/// task that fails to respond to interrupts may never terminate.
		/// 
		/// </para>
		/// </summary>
		/// <returns> list of tasks that never commenced execution </returns>
		/// <exception cref="SecurityException"> if a security manager exists and
		///         shutting down this ExecutorService may manipulate
		///         threads that the caller is not permitted to modify
		///         because it does not hold {@link
		///         java.lang.RuntimePermission}{@code ("modifyThread")},
		///         or the security manager's {@code checkAccess} method
		///         denies access. </exception>
		IList<Runnable> ShutdownNow();

		/// <summary>
		/// Returns {@code true} if this executor has been shut down.
		/// </summary>
		/// <returns> {@code true} if this executor has been shut down </returns>
		bool Shutdown {get;}

		/// <summary>
		/// Returns {@code true} if all tasks have completed following shut down.
		/// Note that {@code isTerminated} is never {@code true} unless
		/// either {@code shutdown} or {@code shutdownNow} was called first.
		/// </summary>
		/// <returns> {@code true} if all tasks have completed following shut down </returns>
		bool Terminated {get;}

		/// <summary>
		/// Blocks until all tasks have completed execution after a shutdown
		/// request, or the timeout occurs, or the current thread is
		/// interrupted, whichever happens first.
		/// </summary>
		/// <param name="timeout"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the timeout argument </param>
		/// <returns> {@code true} if this executor terminated and
		///         {@code false} if the timeout elapsed before termination </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean awaitTermination(long timeout, TimeUnit unit) throws InterruptedException;
		bool AwaitTermination(long timeout, TimeUnit unit);

		/// <summary>
		/// Submits a value-returning task for execution and returns a
		/// Future representing the pending results of the task. The
		/// Future's {@code get} method will return the task's result upon
		/// successful completion.
		/// 
		/// <para>
		/// If you would like to immediately block waiting
		/// for a task, you can use constructions of the form
		/// {@code result = exec.submit(aCallable).get();}
		/// 
		/// </para>
		/// <para>Note: The <seealso cref="Executors"/> class includes a set of methods
		/// that can convert some other common closure-like objects,
		/// for example, <seealso cref="java.security.PrivilegedAction"/> to
		/// <seealso cref="Callable"/> form so they can be submitted.
		/// 
		/// </para>
		/// </summary>
		/// <param name="task"> the task to submit </param>
		/// @param <T> the type of the task's result </param>
		/// <returns> a Future representing pending completion of the task </returns>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		/// <exception cref="NullPointerException"> if the task is null </exception>
		Future<T> submit<T>(Callable<T> task);

		/// <summary>
		/// Submits a Runnable task for execution and returns a Future
		/// representing that task. The Future's {@code get} method will
		/// return the given result upon successful completion.
		/// </summary>
		/// <param name="task"> the task to submit </param>
		/// <param name="result"> the result to return </param>
		/// @param <T> the type of the result </param>
		/// <returns> a Future representing pending completion of the task </returns>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		/// <exception cref="NullPointerException"> if the task is null </exception>
		Future<T> submit<T>(Runnable task, T result);

		/// <summary>
		/// Submits a Runnable task for execution and returns a Future
		/// representing that task. The Future's {@code get} method will
		/// return {@code null} upon <em>successful</em> completion.
		/// </summary>
		/// <param name="task"> the task to submit </param>
		/// <returns> a Future representing pending completion of the task </returns>
		/// <exception cref="RejectedExecutionException"> if the task cannot be
		///         scheduled for execution </exception>
		/// <exception cref="NullPointerException"> if the task is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Future<?> submit(Runnable task);
		Future<?> Submit(Runnable task);

		/// <summary>
		/// Executes the given tasks, returning a list of Futures holding
		/// their status and results when all complete.
		/// <seealso cref="Future#isDone"/> is {@code true} for each
		/// element of the returned list.
		/// Note that a <em>completed</em> task could have
		/// terminated either normally or by throwing an exception.
		/// The results of this method are undefined if the given
		/// collection is modified while this operation is in progress.
		/// </summary>
		/// <param name="tasks"> the collection of tasks </param>
		/// @param <T> the type of the values returned from the tasks </param>
		/// <returns> a list of Futures representing the tasks, in the same
		///         sequential order as produced by the iterator for the
		///         given task list, each of which has completed </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting, in
		///         which case unfinished tasks are cancelled </exception>
		/// <exception cref="NullPointerException"> if tasks or any of its elements are {@code null} </exception>
		/// <exception cref="RejectedExecutionException"> if any task cannot be
		///         scheduled for execution </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T> java.util.List<Future<T>> invokeAll(java.util.Collection<? extends Callable<T>> tasks) throws InterruptedException;
		IList<Future<T>> invokeAll<T, T1>(ICollection<T1> tasks) where T1 : Callable<T>;

		/// <summary>
		/// Executes the given tasks, returning a list of Futures holding
		/// their status and results
		/// when all complete or the timeout expires, whichever happens first.
		/// <seealso cref="Future#isDone"/> is {@code true} for each
		/// element of the returned list.
		/// Upon return, tasks that have not completed are cancelled.
		/// Note that a <em>completed</em> task could have
		/// terminated either normally or by throwing an exception.
		/// The results of this method are undefined if the given
		/// collection is modified while this operation is in progress.
		/// </summary>
		/// <param name="tasks"> the collection of tasks </param>
		/// <param name="timeout"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the timeout argument </param>
		/// @param <T> the type of the values returned from the tasks </param>
		/// <returns> a list of Futures representing the tasks, in the same
		///         sequential order as produced by the iterator for the
		///         given task list. If the operation did not time out,
		///         each task will have completed. If it did time out, some
		///         of these tasks will not have completed. </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting, in
		///         which case unfinished tasks are cancelled </exception>
		/// <exception cref="NullPointerException"> if tasks, any of its elements, or
		///         unit are {@code null} </exception>
		/// <exception cref="RejectedExecutionException"> if any task cannot be scheduled
		///         for execution </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T> java.util.List<Future<T>> invokeAll(java.util.Collection<? extends Callable<T>> tasks, long timeout, TimeUnit unit) throws InterruptedException;
		IList<Future<T>> invokeAll<T, T1>(ICollection<T1> tasks, long timeout, TimeUnit unit) where T1 : Callable<T>;

		/// <summary>
		/// Executes the given tasks, returning the result
		/// of one that has completed successfully (i.e., without throwing
		/// an exception), if any do. Upon normal or exceptional return,
		/// tasks that have not completed are cancelled.
		/// The results of this method are undefined if the given
		/// collection is modified while this operation is in progress.
		/// </summary>
		/// <param name="tasks"> the collection of tasks </param>
		/// @param <T> the type of the values returned from the tasks </param>
		/// <returns> the result returned by one of the tasks </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
		/// <exception cref="NullPointerException"> if tasks or any element task
		///         subject to execution is {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if tasks is empty </exception>
		/// <exception cref="ExecutionException"> if no task successfully completes </exception>
		/// <exception cref="RejectedExecutionException"> if tasks cannot be scheduled
		///         for execution </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T> T invokeAny(java.util.Collection<? extends Callable<T>> tasks) throws InterruptedException, ExecutionException;
		T invokeAny<T, T1>(ICollection<T1> tasks) where T1 : Callable<T>;

		/// <summary>
		/// Executes the given tasks, returning the result
		/// of one that has completed successfully (i.e., without throwing
		/// an exception), if any do before the given timeout elapses.
		/// Upon normal or exceptional return, tasks that have not
		/// completed are cancelled.
		/// The results of this method are undefined if the given
		/// collection is modified while this operation is in progress.
		/// </summary>
		/// <param name="tasks"> the collection of tasks </param>
		/// <param name="timeout"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the timeout argument </param>
		/// @param <T> the type of the values returned from the tasks </param>
		/// <returns> the result returned by one of the tasks </returns>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
		/// <exception cref="NullPointerException"> if tasks, or unit, or any element
		///         task subject to execution is {@code null} </exception>
		/// <exception cref="TimeoutException"> if the given timeout elapses before
		///         any task successfully completes </exception>
		/// <exception cref="ExecutionException"> if no task successfully completes </exception>
		/// <exception cref="RejectedExecutionException"> if tasks cannot be scheduled
		///         for execution </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T> T invokeAny(java.util.Collection<? extends Callable<T>> tasks, long timeout, TimeUnit unit) throws InterruptedException, ExecutionException, TimeoutException;
		T invokeAny<T, T1>(ICollection<T1> tasks, long timeout, TimeUnit unit) where T1 : Callable<T>;
	}

}