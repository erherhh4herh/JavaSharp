using System.Diagnostics;
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
	/// Provides default implementations of <seealso cref="ExecutorService"/>
	/// execution methods. This class implements the {@code submit},
	/// {@code invokeAny} and {@code invokeAll} methods using a
	/// <seealso cref="RunnableFuture"/> returned by {@code newTaskFor}, which defaults
	/// to the <seealso cref="FutureTask"/> class provided in this package.  For example,
	/// the implementation of {@code submit(Runnable)} creates an
	/// associated {@code RunnableFuture} that is executed and
	/// returned. Subclasses may override the {@code newTaskFor} methods
	/// to return {@code RunnableFuture} implementations other than
	/// {@code FutureTask}.
	/// 
	/// <para><b>Extension example</b>. Here is a sketch of a class
	/// that customizes <seealso cref="ThreadPoolExecutor"/> to use
	/// a {@code CustomTask} class instead of the default {@code FutureTask}:
	///  <pre> {@code
	/// public class CustomThreadPoolExecutor extends ThreadPoolExecutor {
	/// 
	///   static class CustomTask<V> implements RunnableFuture<V> {...}
	/// 
	///   protected <V> RunnableFuture<V> newTaskFor(Callable<V> c) {
	///       return new CustomTask<V>(c);
	///   }
	///   protected <V> RunnableFuture<V> newTaskFor(Runnable r, V v) {
	///       return new CustomTask<V>(r, v);
	///   }
	///   // ... add constructors, etc.
	/// }}</pre>
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public abstract class AbstractExecutorService : ExecutorService
	{
		public abstract void Execute(java.lang.Runnable command);
		public abstract bool AwaitTermination(long timeout, TimeUnit unit);
		public abstract bool Terminated {get;}
		public abstract bool Shutdown {get;}
		public abstract IList<Runnable> ShutdownNow();
		public abstract void Shutdown();

		/// <summary>
		/// Returns a {@code RunnableFuture} for the given runnable and default
		/// value.
		/// </summary>
		/// <param name="runnable"> the runnable task being wrapped </param>
		/// <param name="value"> the default value for the returned future </param>
		/// @param <T> the type of the given value </param>
		/// <returns> a {@code RunnableFuture} which, when run, will run the
		/// underlying runnable and which, as a {@code Future}, will yield
		/// the given value as its result and provide for cancellation of
		/// the underlying task
		/// @since 1.6 </returns>
		protected internal virtual RunnableFuture<T> newTaskFor<T>(Runnable runnable, T value)
		{
			return new FutureTask<T>(runnable, value);
		}

		/// <summary>
		/// Returns a {@code RunnableFuture} for the given callable task.
		/// </summary>
		/// <param name="callable"> the callable task being wrapped </param>
		/// @param <T> the type of the callable's result </param>
		/// <returns> a {@code RunnableFuture} which, when run, will call the
		/// underlying callable and which, as a {@code Future}, will yield
		/// the callable's result as its result and provide for
		/// cancellation of the underlying task
		/// @since 1.6 </returns>
		protected internal virtual RunnableFuture<T> newTaskFor<T>(Callable<T> callable)
		{
			return new FutureTask<T>(callable);
		}

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public Future<?> submit(Runnable task)
		public virtual Future<?> Submit(Runnable task)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			RunnableFuture<Void> ftask = NewTaskFor(task, null);
			Execute(ftask);
			return ftask;
		}

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
		public virtual Future<T> submit<T>(Runnable task, T result)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			RunnableFuture<T> ftask = NewTaskFor(task, result);
			Execute(ftask);
			return ftask;
		}

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
		public virtual Future<T> submit<T>(Callable<T> task)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			RunnableFuture<T> ftask = NewTaskFor(task);
			Execute(ftask);
			return ftask;
		}

		/// <summary>
		/// the main mechanics of invokeAny.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private <T> T doInvokeAny(Collection<? extends Callable<T>> tasks, boolean timed, long nanos) throws InterruptedException, ExecutionException, TimeoutException
		private T doInvokeAny<T, T1>(Collection<T1> tasks, bool timed, long nanos) where T1 : Callable<T>
		{
			if (tasks == null)
			{
				throw new NullPointerException();
			}
			int ntasks = tasks.Size();
			if (ntasks == 0)
			{
				throw new IllegalArgumentException();
			}
			List<Future<T>> futures = new List<Future<T>>(ntasks);
			ExecutorCompletionService<T> ecs = new ExecutorCompletionService<T>(this);

			// For efficiency, especially in executors with limited
			// parallelism, check to see if previously submitted tasks are
			// done before submitting more of them. This interleaving
			// plus the exception mechanics account for messiness of main
			// loop.

			try
			{
				// Record exceptions so that if we fail to obtain any
				// result, we can throw the last exception we got.
				ExecutionException ee = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = timed ? System.nanoTime() + nanos : 0L;
				long deadline = timed ? System.nanoTime() + nanos : 0L;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<? extends Callable<T>> it = tasks.iterator();
				Iterator<?> it = tasks.Iterator();

				// Start one task for sure; the rest incrementally
				futures.Add(ecs.Submit(it.Next()));
				--ntasks;
				int active = 1;

				for (;;)
				{
					Future<T> f = ecs.Poll();
					if (f == null)
					{
						if (ntasks > 0)
						{
							--ntasks;
							futures.Add(ecs.Submit(it.Next()));
							++active;
						}
						else if (active == 0)
						{
							break;
						}
						else if (timed)
						{
							f = ecs.Poll(nanos, TimeUnit.NANOSECONDS);
							if (f == null)
							{
								throw new TimeoutException();
							}
							nanos = deadline - System.nanoTime();
						}
						else
						{
							f = ecs.Take();
						}
					}
					if (f != null)
					{
						--active;
						try
						{
							return f.Get();
						}
						catch (ExecutionException eex)
						{
							ee = eex;
						}
						catch (RuntimeException rex)
						{
							ee = new ExecutionException(rex);
						}
					}
				}

				if (ee == null)
				{
					ee = new ExecutionException();
				}
				throw ee;

			}
			finally
			{
				for (int i = 0, size = futures.Size(); i < size; i++)
				{
					futures.Get(i).Cancel(true);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T invokeAny(Collection<? extends Callable<T>> tasks) throws InterruptedException, ExecutionException
		public virtual T invokeAny<T, T1>(Collection<T1> tasks) where T1 : Callable<T>
		{
			try
			{
				return DoInvokeAny(tasks, false, 0);
			}
			catch (TimeoutException)
			{
				Debug.Assert(false);
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T invokeAny(Collection<? extends Callable<T>> tasks, long timeout, TimeUnit unit) throws InterruptedException, ExecutionException, TimeoutException
		public virtual T invokeAny<T, T1>(Collection<T1> tasks, long timeout, TimeUnit unit) where T1 : Callable<T>
		{
			return DoInvokeAny(tasks, true, unit.ToNanos(timeout));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> List<Future<T>> invokeAll(Collection<? extends Callable<T>> tasks) throws InterruptedException
		public virtual List<Future<T>> invokeAll<T, T1>(Collection<T1> tasks) where T1 : Callable<T>
		{
			if (tasks == null)
			{
				throw new NullPointerException();
			}
			List<Future<T>> futures = new List<Future<T>>(tasks.Size());
			bool done = false;
			try
			{
				foreach (Callable<T> t in tasks)
				{
					RunnableFuture<T> f = NewTaskFor(t);
					futures.Add(f);
					Execute(f);
				}
				for (int i = 0, size = futures.Size(); i < size; i++)
				{
					Future<T> f = futures.Get(i);
					if (!f.Done)
					{
						try
						{
							f.Get();
						}
						catch (CancellationException)
						{
						}
						catch (ExecutionException)
						{
						}
					}
				}
				done = true;
				return futures;
			}
			finally
			{
				if (!done)
				{
					for (int i = 0, size = futures.Size(); i < size; i++)
					{
						futures.Get(i).Cancel(true);
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> List<Future<T>> invokeAll(Collection<? extends Callable<T>> tasks, long timeout, TimeUnit unit) throws InterruptedException
		public virtual List<Future<T>> invokeAll<T, T1>(Collection<T1> tasks, long timeout, TimeUnit unit) where T1 : Callable<T>
		{
			if (tasks == null)
			{
				throw new NullPointerException();
			}
			long nanos = unit.ToNanos(timeout);
			List<Future<T>> futures = new List<Future<T>>(tasks.Size());
			bool done = false;
			try
			{
				foreach (Callable<T> t in tasks)
				{
					futures.Add(NewTaskFor(t));
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = System.nanoTime() + nanos;
				long deadline = System.nanoTime() + nanos;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = futures.size();
				int size = futures.Size();

				// Interleave time checks and calls to execute in case
				// executor doesn't have any/much parallelism.
				for (int i = 0; i < size; i++)
				{
					Execute((Runnable)futures.Get(i));
					nanos = deadline - System.nanoTime();
					if (nanos <= 0L)
					{
						return futures;
					}
				}

				for (int i = 0; i < size; i++)
				{
					Future<T> f = futures.Get(i);
					if (!f.Done)
					{
						if (nanos <= 0L)
						{
							return futures;
						}
						try
						{
							f.Get(nanos, TimeUnit.NANOSECONDS);
						}
						catch (CancellationException)
						{
						}
						catch (ExecutionException)
						{
						}
						catch (TimeoutException)
						{
							return futures;
						}
						nanos = deadline - System.nanoTime();
					}
				}
				done = true;
				return futures;
			}
			finally
			{
				if (!done)
				{
					for (int i = 0, size = futures.Size(); i < size; i++)
					{
						futures.Get(i).Cancel(true);
					}
				}
			}
		}

	}

}