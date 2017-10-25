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
	/// A <seealso cref="CompletionService"/> that uses a supplied <seealso cref="Executor"/>
	/// to execute tasks.  This class arranges that submitted tasks are,
	/// upon completion, placed on a queue accessible using {@code take}.
	/// The class is lightweight enough to be suitable for transient use
	/// when processing groups of tasks.
	/// 
	/// <para>
	/// 
	/// <b>Usage Examples.</b>
	/// 
	/// Suppose you have a set of solvers for a certain problem, each
	/// returning a value of some type {@code Result}, and would like to
	/// run them concurrently, processing the results of each of them that
	/// return a non-null value, in some method {@code use(Result r)}. You
	/// could write this as:
	/// 
	/// <pre> {@code
	/// void solve(Executor e,
	///            Collection<Callable<Result>> solvers)
	///     throws InterruptedException, ExecutionException {
	///     CompletionService<Result> ecs
	///         = new ExecutorCompletionService<Result>(e);
	///     for (Callable<Result> s : solvers)
	///         ecs.submit(s);
	///     int n = solvers.size();
	///     for (int i = 0; i < n; ++i) {
	///         Result r = ecs.take().get();
	///         if (r != null)
	///             use(r);
	///     }
	/// }}</pre>
	/// 
	/// Suppose instead that you would like to use the first non-null result
	/// of the set of tasks, ignoring any that encounter exceptions,
	/// and cancelling all other tasks when the first one is ready:
	/// 
	/// <pre> {@code
	/// void solve(Executor e,
	///            Collection<Callable<Result>> solvers)
	///     throws InterruptedException {
	///     CompletionService<Result> ecs
	///         = new ExecutorCompletionService<Result>(e);
	///     int n = solvers.size();
	///     List<Future<Result>> futures
	///         = new ArrayList<Future<Result>>(n);
	///     Result result = null;
	///     try {
	///         for (Callable<Result> s : solvers)
	///             futures.add(ecs.submit(s));
	///         for (int i = 0; i < n; ++i) {
	///             try {
	///                 Result r = ecs.take().get();
	///                 if (r != null) {
	///                     result = r;
	///                     break;
	///                 }
	///             } catch (ExecutionException ignore) {}
	///         }
	///     }
	///     finally {
	///         for (Future<Result> f : futures)
	///             f.cancel(true);
	///     }
	/// 
	///     if (result != null)
	///         use(result);
	/// }}</pre>
	/// </para>
	/// </summary>
	public class ExecutorCompletionService<V> : CompletionService<V>
	{
		private readonly Executor Executor;
		private readonly AbstractExecutorService Aes;
		private readonly BlockingQueue<Future<V>> CompletionQueue;

		/// <summary>
		/// FutureTask extension to enqueue upon completion
		/// </summary>
		private class QueueingFuture : FutureTask<Void>
		{
			private readonly ExecutorCompletionService<V> OuterInstance;

			internal QueueingFuture(ExecutorCompletionService<V> outerInstance, RunnableFuture<V> task) : base(task, null)
			{
				this.OuterInstance = outerInstance;
				this.Task = task;
			}
			protected internal virtual void Done()
			{
				outerInstance.CompletionQueue.Add(Task);
			}
			internal readonly Future<V> Task;
		}

		private RunnableFuture<V> NewTaskFor(Callable<V> task)
		{
			if (Aes == null)
			{
				return new FutureTask<V>(task);
			}
			else
			{
				return Aes.NewTaskFor(task);
			}
		}

		private RunnableFuture<V> NewTaskFor(Runnable task, V result)
		{
			if (Aes == null)
			{
				return new FutureTask<V>(task, result);
			}
			else
			{
				return Aes.NewTaskFor(task, result);
			}
		}

		/// <summary>
		/// Creates an ExecutorCompletionService using the supplied
		/// executor for base task execution and a
		/// <seealso cref="LinkedBlockingQueue"/> as a completion queue.
		/// </summary>
		/// <param name="executor"> the executor to use </param>
		/// <exception cref="NullPointerException"> if executor is {@code null} </exception>
		public ExecutorCompletionService(Executor executor)
		{
			if (executor == null)
			{
				throw new NullPointerException();
			}
			this.Executor = executor;
			this.Aes = (executor is AbstractExecutorService) ? (AbstractExecutorService) executor : null;
			this.CompletionQueue = new LinkedBlockingQueue<Future<V>>();
		}

		/// <summary>
		/// Creates an ExecutorCompletionService using the supplied
		/// executor for base task execution and the supplied queue as its
		/// completion queue.
		/// </summary>
		/// <param name="executor"> the executor to use </param>
		/// <param name="completionQueue"> the queue to use as the completion queue
		///        normally one dedicated for use by this service. This
		///        queue is treated as unbounded -- failed attempted
		///        {@code Queue.add} operations for completed tasks cause
		///        them not to be retrievable. </param>
		/// <exception cref="NullPointerException"> if executor or completionQueue are {@code null} </exception>
		public ExecutorCompletionService(Executor executor, BlockingQueue<Future<V>> completionQueue)
		{
			if (executor == null || completionQueue == null)
			{
				throw new NullPointerException();
			}
			this.Executor = executor;
			this.Aes = (executor is AbstractExecutorService) ? (AbstractExecutorService) executor : null;
			this.CompletionQueue = completionQueue;
		}

		public virtual Future<V> Submit(Callable<V> task)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			RunnableFuture<V> f = NewTaskFor(task);
			Executor.Execute(new QueueingFuture(this, f));
			return f;
		}

		public virtual Future<V> Submit(Runnable task, V result)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			RunnableFuture<V> f = NewTaskFor(task, result);
			Executor.Execute(new QueueingFuture(this, f));
			return f;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Future<V> take() throws InterruptedException
		public virtual Future<V> Take()
		{
			return CompletionQueue.Take();
		}

		public virtual Future<V> Poll()
		{
			return CompletionQueue.Poll();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Future<V> poll(long timeout, TimeUnit unit) throws InterruptedException
		public virtual Future<V> Poll(long timeout, TimeUnit unit)
		{
			return CompletionQueue.Poll(timeout, unit);
		}

	}

}