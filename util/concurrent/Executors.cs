using System.Collections.Generic;
using System.Threading;

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
	using SecurityConstants = sun.security.util.SecurityConstants;

	/// <summary>
	/// Factory and utility methods for <seealso cref="Executor"/>, {@link
	/// ExecutorService}, <seealso cref="ScheduledExecutorService"/>, {@link
	/// ThreadFactory}, and <seealso cref="Callable"/> classes defined in this
	/// package. This class supports the following kinds of methods:
	/// 
	/// <ul>
	///   <li> Methods that create and return an <seealso cref="ExecutorService"/>
	///        set up with commonly useful configuration settings.
	///   <li> Methods that create and return a <seealso cref="ScheduledExecutorService"/>
	///        set up with commonly useful configuration settings.
	///   <li> Methods that create and return a "wrapped" ExecutorService, that
	///        disables reconfiguration by making implementation-specific methods
	///        inaccessible.
	///   <li> Methods that create and return a <seealso cref="ThreadFactory"/>
	///        that sets newly created threads to a known state.
	///   <li> Methods that create and return a <seealso cref="Callable"/>
	///        out of other closure-like forms, so they can be used
	///        in execution methods requiring {@code Callable}.
	/// </ul>
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </summary>
	public class Executors
	{

		/// <summary>
		/// Creates a thread pool that reuses a fixed number of threads
		/// operating off a shared unbounded queue.  At any point, at most
		/// {@code nThreads} threads will be active processing tasks.
		/// If additional tasks are submitted when all threads are active,
		/// they will wait in the queue until a thread is available.
		/// If any thread terminates due to a failure during execution
		/// prior to shutdown, a new one will take its place if needed to
		/// execute subsequent tasks.  The threads in the pool will exist
		/// until it is explicitly <seealso cref="ExecutorService#shutdown shutdown"/>.
		/// </summary>
		/// <param name="nThreads"> the number of threads in the pool </param>
		/// <returns> the newly created thread pool </returns>
		/// <exception cref="IllegalArgumentException"> if {@code nThreads <= 0} </exception>
		public static ExecutorService NewFixedThreadPool(int nThreads)
		{
			return new ThreadPoolExecutor(nThreads, nThreads, 0L, TimeUnit.MILLISECONDS, new LinkedBlockingQueue<Runnable>());
		}

		/// <summary>
		/// Creates a thread pool that maintains enough threads to support
		/// the given parallelism level, and may use multiple queues to
		/// reduce contention. The parallelism level corresponds to the
		/// maximum number of threads actively engaged in, or available to
		/// engage in, task processing. The actual number of threads may
		/// grow and shrink dynamically. A work-stealing pool makes no
		/// guarantees about the order in which submitted tasks are
		/// executed.
		/// </summary>
		/// <param name="parallelism"> the targeted parallelism level </param>
		/// <returns> the newly created thread pool </returns>
		/// <exception cref="IllegalArgumentException"> if {@code parallelism <= 0}
		/// @since 1.8 </exception>
		public static ExecutorService NewWorkStealingPool(int parallelism)
		{
			return new ForkJoinPool(parallelism, ForkJoinPool.DefaultForkJoinWorkerThreadFactory, null, true);
		}

		/// <summary>
		/// Creates a work-stealing thread pool using all
		/// <seealso cref="Runtime#availableProcessors available processors"/>
		/// as its target parallelism level. </summary>
		/// <returns> the newly created thread pool </returns>
		/// <seealso cref= #newWorkStealingPool(int)
		/// @since 1.8 </seealso>
		public static ExecutorService NewWorkStealingPool()
		{
			return new ForkJoinPool(Runtime.Runtime.availableProcessors(), ForkJoinPool.DefaultForkJoinWorkerThreadFactory, null, true);
		}

		/// <summary>
		/// Creates a thread pool that reuses a fixed number of threads
		/// operating off a shared unbounded queue, using the provided
		/// ThreadFactory to create new threads when needed.  At any point,
		/// at most {@code nThreads} threads will be active processing
		/// tasks.  If additional tasks are submitted when all threads are
		/// active, they will wait in the queue until a thread is
		/// available.  If any thread terminates due to a failure during
		/// execution prior to shutdown, a new one will take its place if
		/// needed to execute subsequent tasks.  The threads in the pool will
		/// exist until it is explicitly {@link ExecutorService#shutdown
		/// shutdown}.
		/// </summary>
		/// <param name="nThreads"> the number of threads in the pool </param>
		/// <param name="threadFactory"> the factory to use when creating new threads </param>
		/// <returns> the newly created thread pool </returns>
		/// <exception cref="NullPointerException"> if threadFactory is null </exception>
		/// <exception cref="IllegalArgumentException"> if {@code nThreads <= 0} </exception>
		public static ExecutorService NewFixedThreadPool(int nThreads, ThreadFactory threadFactory)
		{
			return new ThreadPoolExecutor(nThreads, nThreads, 0L, TimeUnit.MILLISECONDS, new LinkedBlockingQueue<Runnable>(), threadFactory);
		}

		/// <summary>
		/// Creates an Executor that uses a single worker thread operating
		/// off an unbounded queue. (Note however that if this single
		/// thread terminates due to a failure during execution prior to
		/// shutdown, a new one will take its place if needed to execute
		/// subsequent tasks.)  Tasks are guaranteed to execute
		/// sequentially, and no more than one task will be active at any
		/// given time. Unlike the otherwise equivalent
		/// {@code newFixedThreadPool(1)} the returned executor is
		/// guaranteed not to be reconfigurable to use additional threads.
		/// </summary>
		/// <returns> the newly created single-threaded Executor </returns>
		public static ExecutorService NewSingleThreadExecutor()
		{
			return new FinalizableDelegatedExecutorService(new ThreadPoolExecutor(1, 1, 0L, TimeUnit.MILLISECONDS, new LinkedBlockingQueue<Runnable>()));
		}

		/// <summary>
		/// Creates an Executor that uses a single worker thread operating
		/// off an unbounded queue, and uses the provided ThreadFactory to
		/// create a new thread when needed. Unlike the otherwise
		/// equivalent {@code newFixedThreadPool(1, threadFactory)} the
		/// returned executor is guaranteed not to be reconfigurable to use
		/// additional threads.
		/// </summary>
		/// <param name="threadFactory"> the factory to use when creating new
		/// threads
		/// </param>
		/// <returns> the newly created single-threaded Executor </returns>
		/// <exception cref="NullPointerException"> if threadFactory is null </exception>
		public static ExecutorService NewSingleThreadExecutor(ThreadFactory threadFactory)
		{
			return new FinalizableDelegatedExecutorService(new ThreadPoolExecutor(1, 1, 0L, TimeUnit.MILLISECONDS, new LinkedBlockingQueue<Runnable>(), threadFactory));
		}

		/// <summary>
		/// Creates a thread pool that creates new threads as needed, but
		/// will reuse previously constructed threads when they are
		/// available.  These pools will typically improve the performance
		/// of programs that execute many short-lived asynchronous tasks.
		/// Calls to {@code execute} will reuse previously constructed
		/// threads if available. If no existing thread is available, a new
		/// thread will be created and added to the pool. Threads that have
		/// not been used for sixty seconds are terminated and removed from
		/// the cache. Thus, a pool that remains idle for long enough will
		/// not consume any resources. Note that pools with similar
		/// properties but different details (for example, timeout parameters)
		/// may be created using <seealso cref="ThreadPoolExecutor"/> constructors.
		/// </summary>
		/// <returns> the newly created thread pool </returns>
		public static ExecutorService NewCachedThreadPool()
		{
			return new ThreadPoolExecutor(0, Integer.MaxValue, 60L, TimeUnit.SECONDS, new SynchronousQueue<Runnable>());
		}

		/// <summary>
		/// Creates a thread pool that creates new threads as needed, but
		/// will reuse previously constructed threads when they are
		/// available, and uses the provided
		/// ThreadFactory to create new threads when needed. </summary>
		/// <param name="threadFactory"> the factory to use when creating new threads </param>
		/// <returns> the newly created thread pool </returns>
		/// <exception cref="NullPointerException"> if threadFactory is null </exception>
		public static ExecutorService NewCachedThreadPool(ThreadFactory threadFactory)
		{
			return new ThreadPoolExecutor(0, Integer.MaxValue, 60L, TimeUnit.SECONDS, new SynchronousQueue<Runnable>(), threadFactory);
		}

		/// <summary>
		/// Creates a single-threaded executor that can schedule commands
		/// to run after a given delay, or to execute periodically.
		/// (Note however that if this single
		/// thread terminates due to a failure during execution prior to
		/// shutdown, a new one will take its place if needed to execute
		/// subsequent tasks.)  Tasks are guaranteed to execute
		/// sequentially, and no more than one task will be active at any
		/// given time. Unlike the otherwise equivalent
		/// {@code newScheduledThreadPool(1)} the returned executor is
		/// guaranteed not to be reconfigurable to use additional threads. </summary>
		/// <returns> the newly created scheduled executor </returns>
		public static ScheduledExecutorService NewSingleThreadScheduledExecutor()
		{
			return new DelegatedScheduledExecutorService(new ScheduledThreadPoolExecutor(1));
		}

		/// <summary>
		/// Creates a single-threaded executor that can schedule commands
		/// to run after a given delay, or to execute periodically.  (Note
		/// however that if this single thread terminates due to a failure
		/// during execution prior to shutdown, a new one will take its
		/// place if needed to execute subsequent tasks.)  Tasks are
		/// guaranteed to execute sequentially, and no more than one task
		/// will be active at any given time. Unlike the otherwise
		/// equivalent {@code newScheduledThreadPool(1, threadFactory)}
		/// the returned executor is guaranteed not to be reconfigurable to
		/// use additional threads. </summary>
		/// <param name="threadFactory"> the factory to use when creating new
		/// threads </param>
		/// <returns> a newly created scheduled executor </returns>
		/// <exception cref="NullPointerException"> if threadFactory is null </exception>
		public static ScheduledExecutorService NewSingleThreadScheduledExecutor(ThreadFactory threadFactory)
		{
			return new DelegatedScheduledExecutorService(new ScheduledThreadPoolExecutor(1, threadFactory));
		}

		/// <summary>
		/// Creates a thread pool that can schedule commands to run after a
		/// given delay, or to execute periodically. </summary>
		/// <param name="corePoolSize"> the number of threads to keep in the pool,
		/// even if they are idle </param>
		/// <returns> a newly created scheduled thread pool </returns>
		/// <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		public static ScheduledExecutorService NewScheduledThreadPool(int corePoolSize)
		{
			return new ScheduledThreadPoolExecutor(corePoolSize);
		}

		/// <summary>
		/// Creates a thread pool that can schedule commands to run after a
		/// given delay, or to execute periodically. </summary>
		/// <param name="corePoolSize"> the number of threads to keep in the pool,
		/// even if they are idle </param>
		/// <param name="threadFactory"> the factory to use when the executor
		/// creates a new thread </param>
		/// <returns> a newly created scheduled thread pool </returns>
		/// <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		/// <exception cref="NullPointerException"> if threadFactory is null </exception>
		public static ScheduledExecutorService NewScheduledThreadPool(int corePoolSize, ThreadFactory threadFactory)
		{
			return new ScheduledThreadPoolExecutor(corePoolSize, threadFactory);
		}

		/// <summary>
		/// Returns an object that delegates all defined {@link
		/// ExecutorService} methods to the given executor, but not any
		/// other methods that might otherwise be accessible using
		/// casts. This provides a way to safely "freeze" configuration and
		/// disallow tuning of a given concrete implementation. </summary>
		/// <param name="executor"> the underlying implementation </param>
		/// <returns> an {@code ExecutorService} instance </returns>
		/// <exception cref="NullPointerException"> if executor null </exception>
		public static ExecutorService UnconfigurableExecutorService(ExecutorService executor)
		{
			if (executor == null)
			{
				throw new NullPointerException();
			}
			return new DelegatedExecutorService(executor);
		}

		/// <summary>
		/// Returns an object that delegates all defined {@link
		/// ScheduledExecutorService} methods to the given executor, but
		/// not any other methods that might otherwise be accessible using
		/// casts. This provides a way to safely "freeze" configuration and
		/// disallow tuning of a given concrete implementation. </summary>
		/// <param name="executor"> the underlying implementation </param>
		/// <returns> a {@code ScheduledExecutorService} instance </returns>
		/// <exception cref="NullPointerException"> if executor null </exception>
		public static ScheduledExecutorService UnconfigurableScheduledExecutorService(ScheduledExecutorService executor)
		{
			if (executor == null)
			{
				throw new NullPointerException();
			}
			return new DelegatedScheduledExecutorService(executor);
		}

		/// <summary>
		/// Returns a default thread factory used to create new threads.
		/// This factory creates all new threads used by an Executor in the
		/// same <seealso cref="ThreadGroup"/>. If there is a {@link
		/// java.lang.SecurityManager}, it uses the group of {@link
		/// System#getSecurityManager}, else the group of the thread
		/// invoking this {@code defaultThreadFactory} method. Each new
		/// thread is created as a non-daemon thread with priority set to
		/// the smaller of {@code Thread.NORM_PRIORITY} and the maximum
		/// priority permitted in the thread group.  New threads have names
		/// accessible via <seealso cref="Thread#getName"/> of
		/// <em>pool-N-thread-M</em>, where <em>N</em> is the sequence
		/// number of this factory, and <em>M</em> is the sequence number
		/// of the thread created by this factory. </summary>
		/// <returns> a thread factory </returns>
		public static ThreadFactory DefaultThreadFactory()
		{
			return new DefaultThreadFactory();
		}

		/// <summary>
		/// Returns a thread factory used to create new threads that
		/// have the same permissions as the current thread.
		/// This factory creates threads with the same settings as {@link
		/// Executors#defaultThreadFactory}, additionally setting the
		/// AccessControlContext and contextClassLoader of new threads to
		/// be the same as the thread invoking this
		/// {@code privilegedThreadFactory} method.  A new
		/// {@code privilegedThreadFactory} can be created within an
		/// <seealso cref="AccessController#doPrivileged AccessController.doPrivileged"/>
		/// action setting the current thread's access control context to
		/// create threads with the selected permission settings holding
		/// within that action.
		/// 
		/// <para>Note that while tasks running within such threads will have
		/// the same access control and class loader settings as the
		/// current thread, they need not have the same {@link
		/// java.lang.ThreadLocal} or {@link
		/// java.lang.InheritableThreadLocal} values. If necessary,
		/// particular values of thread locals can be set or reset before
		/// any task runs in <seealso cref="ThreadPoolExecutor"/> subclasses using
		/// <seealso cref="ThreadPoolExecutor#beforeExecute(Thread, Runnable)"/>.
		/// Also, if it is necessary to initialize worker threads to have
		/// the same InheritableThreadLocal settings as some other
		/// designated thread, you can create a custom ThreadFactory in
		/// which that thread waits for and services requests to create
		/// others that will inherit its values.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a thread factory </returns>
		/// <exception cref="AccessControlException"> if the current access control
		/// context does not have permission to both get and set context
		/// class loader </exception>
		public static ThreadFactory PrivilegedThreadFactory()
		{
			return new PrivilegedThreadFactory();
		}

		/// <summary>
		/// Returns a <seealso cref="Callable"/> object that, when
		/// called, runs the given task and returns the given result.  This
		/// can be useful when applying methods requiring a
		/// {@code Callable} to an otherwise resultless action. </summary>
		/// <param name="task"> the task to run </param>
		/// <param name="result"> the result to return </param>
		/// @param <T> the type of the result </param>
		/// <returns> a callable object </returns>
		/// <exception cref="NullPointerException"> if task null </exception>
		public static Callable<T> callable<T>(Runnable task, T result)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			return new RunnableAdapter<T>(task, result);
		}

		/// <summary>
		/// Returns a <seealso cref="Callable"/> object that, when
		/// called, runs the given task and returns {@code null}. </summary>
		/// <param name="task"> the task to run </param>
		/// <returns> a callable object </returns>
		/// <exception cref="NullPointerException"> if task null </exception>
		public static Callable<Object> Callable(Runnable task)
		{
			if (task == null)
			{
				throw new NullPointerException();
			}
			return new RunnableAdapter<Object>(task, null);
		}

		/// <summary>
		/// Returns a <seealso cref="Callable"/> object that, when
		/// called, runs the given privileged action and returns its result. </summary>
		/// <param name="action"> the privileged action to run </param>
		/// <returns> a callable object </returns>
		/// <exception cref="NullPointerException"> if action null </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Callable<Object> callable(final java.security.PrivilegedAction<?> action)
		public static Callable<Object> Callable(PrivilegedAction<T1> action)
		{
			if (action == null)
			{
				throw new NullPointerException();
			}
			return new CallableAnonymousInnerClassHelper(action);
		}

		private class CallableAnonymousInnerClassHelper : Callable<Object>
		{
			private PrivilegedAction<T1> Action;

			public CallableAnonymousInnerClassHelper(PrivilegedAction<T1> action)
			{
				this.Action = action;
			}

			public virtual Object Call()
			{
				return Action.Run();
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Callable"/> object that, when
		/// called, runs the given privileged exception action and returns
		/// its result. </summary>
		/// <param name="action"> the privileged exception action to run </param>
		/// <returns> a callable object </returns>
		/// <exception cref="NullPointerException"> if action null </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Callable<Object> callable(final java.security.PrivilegedExceptionAction<?> action)
		public static Callable<Object> Callable(PrivilegedExceptionAction<T1> action)
		{
			if (action == null)
			{
				throw new NullPointerException();
			}
			return new CallableAnonymousInnerClassHelper2(action);
		}

		private class CallableAnonymousInnerClassHelper2 : Callable<Object>
		{
			private PrivilegedExceptionAction<T1> Action;

			public CallableAnonymousInnerClassHelper2(PrivilegedExceptionAction<T1> action)
			{
				this.Action = action;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object call() throws Exception
			public virtual Object Call()
			{
				return Action.Run();
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Callable"/> object that will, when called,
		/// execute the given {@code callable} under the current access
		/// control context. This method should normally be invoked within
		/// an <seealso cref="AccessController#doPrivileged AccessController.doPrivileged"/>
		/// action to create callables that will, if possible, execute
		/// under the selected permission settings holding within that
		/// action; or if not possible, throw an associated {@link
		/// AccessControlException}. </summary>
		/// <param name="callable"> the underlying task </param>
		/// @param <T> the type of the callable's result </param>
		/// <returns> a callable object </returns>
		/// <exception cref="NullPointerException"> if callable null </exception>
		public static Callable<T> privilegedCallable<T>(Callable<T> callable)
		{
			if (callable == null)
			{
				throw new NullPointerException();
			}
			return new PrivilegedCallable<T>(callable);
		}

		/// <summary>
		/// Returns a <seealso cref="Callable"/> object that will, when called,
		/// execute the given {@code callable} under the current access
		/// control context, with the current context class loader as the
		/// context class loader. This method should normally be invoked
		/// within an
		/// <seealso cref="AccessController#doPrivileged AccessController.doPrivileged"/>
		/// action to create callables that will, if possible, execute
		/// under the selected permission settings holding within that
		/// action; or if not possible, throw an associated {@link
		/// AccessControlException}.
		/// </summary>
		/// <param name="callable"> the underlying task </param>
		/// @param <T> the type of the callable's result </param>
		/// <returns> a callable object </returns>
		/// <exception cref="NullPointerException"> if callable null </exception>
		/// <exception cref="AccessControlException"> if the current access control
		/// context does not have permission to both set and get context
		/// class loader </exception>
		public static Callable<T> privilegedCallableUsingCurrentClassLoader<T>(Callable<T> callable)
		{
			if (callable == null)
			{
				throw new NullPointerException();
			}
			return new PrivilegedCallableUsingCurrentClassLoader<T>(callable);
		}

		// Non-public classes supporting the public methods

		/// <summary>
		/// A callable that runs given task and returns given result
		/// </summary>
		internal sealed class RunnableAdapter<T> : Callable<T>
		{
			internal readonly Runnable Task;
			internal readonly T Result;
			internal RunnableAdapter(Runnable task, T result)
			{
				this.Task = task;
				this.Result = result;
			}
			public T Call()
			{
				Task.Run();
				return Result;
			}
		}

		/// <summary>
		/// A callable that runs under established access control settings
		/// </summary>
		internal sealed class PrivilegedCallable<T> : Callable<T>
		{
			internal readonly Callable<T> Task;
			internal readonly AccessControlContext Acc;

			internal PrivilegedCallable(Callable<T> task)
			{
				this.Task = task;
				this.Acc = AccessController.Context;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T call() throws Exception
			public T Call()
			{
				try
				{
					return AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this), Acc);
				}
				catch (PrivilegedActionException e)
				{
					throw e.Exception;
				}
			}

			private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<T>
			{
				private readonly PrivilegedCallable<T> OuterInstance;

				public PrivilegedExceptionActionAnonymousInnerClassHelper(PrivilegedCallable<T> outerInstance)
				{
					this.outerInstance = outerInstance;
				}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T run() throws Exception
				public virtual T Run()
				{
					return OuterInstance.Task.Call();
				}
			}
		}

		/// <summary>
		/// A callable that runs under established access control settings and
		/// current ClassLoader
		/// </summary>
		internal sealed class PrivilegedCallableUsingCurrentClassLoader<T> : Callable<T>
		{
			internal readonly Callable<T> Task;
			internal readonly AccessControlContext Acc;
			internal readonly ClassLoader Ccl;

			internal PrivilegedCallableUsingCurrentClassLoader(Callable<T> task)
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					// Calls to getContextClassLoader from this class
					// never trigger a security check, but we check
					// whether our callers have this permission anyways.
					sm.CheckPermission(SecurityConstants.GET_CLASSLOADER_PERMISSION);

					// Whether setContextClassLoader turns out to be necessary
					// or not, we fail fast if permission is not available.
					sm.CheckPermission(new RuntimePermission("setContextClassLoader"));
				}
				this.Task = task;
				this.Acc = AccessController.Context;
				this.Ccl = Thread.CurrentThread.ContextClassLoader;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T call() throws Exception
			public T Call()
			{
				try
				{
					return AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this), Acc);
				}
				catch (PrivilegedActionException e)
				{
					throw e.Exception;
				}
			}

			private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<T>
			{
				private readonly PrivilegedCallableUsingCurrentClassLoader<T> OuterInstance;

				public PrivilegedExceptionActionAnonymousInnerClassHelper(PrivilegedCallableUsingCurrentClassLoader<T> outerInstance)
				{
					this.outerInstance = outerInstance;
				}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T run() throws Exception
				public virtual T Run()
				{
					Thread t = Thread.CurrentThread;
					ClassLoader cl = t.ContextClassLoader;
					if (OuterInstance.Ccl == cl)
					{
						return OuterInstance.Task.Call();
					}
					else
					{
						t.ContextClassLoader = OuterInstance.Ccl;
						try
						{
							return OuterInstance.Task.Call();
						}
						finally
						{
							t.ContextClassLoader = cl;
						}
					}
				}
			}
		}

		/// <summary>
		/// The default thread factory
		/// </summary>
		internal class DefaultThreadFactory : ThreadFactory
		{
			internal static readonly AtomicInteger PoolNumber = new AtomicInteger(1);
			internal readonly ThreadGroup Group;
			internal readonly AtomicInteger ThreadNumber = new AtomicInteger(1);
			internal readonly String NamePrefix;

			internal DefaultThreadFactory()
			{
				SecurityManager s = System.SecurityManager;
				Group = (s != null) ? s.ThreadGroup : Thread.CurrentThread.ThreadGroup;
				NamePrefix = "pool-" + PoolNumber.AndIncrement + "-thread-";
			}

			public virtual Thread NewThread(Runnable r)
			{
				Thread t = new Thread(Group, r, NamePrefix + ThreadNumber.AndIncrement, 0);
				if (t.Daemon)
				{
					t.Daemon = false;
				}
				if (t.Priority != Thread.NORM_PRIORITY)
				{
					t.Priority = Thread.NORM_PRIORITY;
				}
				return t;
			}
		}

		/// <summary>
		/// Thread factory capturing access control context and class loader
		/// </summary>
		internal class PrivilegedThreadFactory : DefaultThreadFactory
		{
			internal readonly AccessControlContext Acc;
			internal readonly ClassLoader Ccl;

			internal PrivilegedThreadFactory() : base()
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					// Calls to getContextClassLoader from this class
					// never trigger a security check, but we check
					// whether our callers have this permission anyways.
					sm.CheckPermission(SecurityConstants.GET_CLASSLOADER_PERMISSION);

					// Fail fast
					sm.CheckPermission(new RuntimePermission("setContextClassLoader"));
				}
				this.Acc = AccessController.Context;
				this.Ccl = Thread.CurrentThread.ContextClassLoader;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Thread newThread(final Runnable r)
			public override Thread NewThread(Runnable r)
			{
				return base.NewThread(new RunnableAnonymousInnerClassHelper(this, r));
			}

			private class RunnableAnonymousInnerClassHelper : Runnable
			{
				private readonly PrivilegedThreadFactory OuterInstance;

				private java.lang.Runnable r;

				public RunnableAnonymousInnerClassHelper(PrivilegedThreadFactory outerInstance, java.lang.Runnable r)
				{
					this.OuterInstance = outerInstance;
					this.r = r;
				}

				public virtual void Run()
				{
					AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this), OuterInstance.Acc);
				}

				private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
				{
					private readonly RunnableAnonymousInnerClassHelper OuterInstance;

					public PrivilegedActionAnonymousInnerClassHelper(RunnableAnonymousInnerClassHelper outerInstance)
					{
						this.outerInstance = outerInstance;
					}

					public virtual Void Run()
					{
						Thread.CurrentThread.ContextClassLoader = OuterInstance.OuterInstance.Ccl;
						OuterInstance.r.run();
						return null;
					}
				}
			}
		}

		/// <summary>
		/// A wrapper class that exposes only the ExecutorService methods
		/// of an ExecutorService implementation.
		/// </summary>
		internal class DelegatedExecutorService : AbstractExecutorService
		{
			internal readonly ExecutorService e;
			internal DelegatedExecutorService(ExecutorService executor)
			{
				e = executor;
			}
			public override void Execute(Runnable command)
			{
				e.Execute(command);
			}
			public override void Shutdown()
			{
				e.Shutdown();
			}
			public override List<Runnable> ShutdownNow()
			{
				return e.ShutdownNow();
			}
			public override bool Shutdown
			{
				get
				{
					return e.Shutdown;
				}
			}
			public override bool Terminated
			{
				get
				{
					return e.Terminated;
				}
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean awaitTermination(long timeout, TimeUnit unit) throws InterruptedException
			public override bool AwaitTermination(long timeout, TimeUnit unit)
			{
				return e.AwaitTermination(timeout, unit);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public Future<?> submit(Runnable task)
			public override Future<?> Submit(Runnable task)
			{
				return e.Submit(task);
			}
			public override Future<T> submit<T>(Callable<T> task)
			{
				return e.Submit(task);
			}
			public override Future<T> submit<T>(Runnable task, T result)
			{
				return e.Submit(task, result);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> List<Future<T>> invokeAll(Collection<? extends Callable<T>> tasks) throws InterruptedException
			public override List<Future<T>> invokeAll<T, T1>(Collection<T1> tasks) where T1 : Callable<T>
			{
				return e.InvokeAll(tasks);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> List<Future<T>> invokeAll(Collection<? extends Callable<T>> tasks, long timeout, TimeUnit unit) throws InterruptedException
			public override List<Future<T>> invokeAll<T, T1>(Collection<T1> tasks, long timeout, TimeUnit unit) where T1 : Callable<T>
			{
				return e.InvokeAll(tasks, timeout, unit);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T invokeAny(Collection<? extends Callable<T>> tasks) throws InterruptedException, ExecutionException
			public override T invokeAny<T, T1>(Collection<T1> tasks) where T1 : Callable<T>
			{
				return e.InvokeAny(tasks);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T invokeAny(Collection<? extends Callable<T>> tasks, long timeout, TimeUnit unit) throws InterruptedException, ExecutionException, TimeoutException
			public override T invokeAny<T, T1>(Collection<T1> tasks, long timeout, TimeUnit unit) where T1 : Callable<T>
			{
				return e.InvokeAny(tasks, timeout, unit);
			}
		}

		internal class FinalizableDelegatedExecutorService : DelegatedExecutorService
		{
			internal FinalizableDelegatedExecutorService(ExecutorService executor) : base(executor)
			{
			}
			~FinalizableDelegatedExecutorService()
			{
				base.Shutdown();
			}
		}

		/// <summary>
		/// A wrapper class that exposes only the ScheduledExecutorService
		/// methods of a ScheduledExecutorService implementation.
		/// </summary>
		internal class DelegatedScheduledExecutorService : DelegatedExecutorService, ScheduledExecutorService
		{
			internal readonly ScheduledExecutorService e;
			internal DelegatedScheduledExecutorService(ScheduledExecutorService executor) : base(executor)
			{
				e = executor;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ScheduledFuture<?> schedule(Runnable command, long delay, TimeUnit unit)
			public virtual ScheduledFuture<?> Schedule(Runnable command, long delay, TimeUnit unit)
			{
				return e.Schedule(command, delay, unit);
			}
			public virtual ScheduledFuture<V> schedule<V>(Callable<V> callable, long delay, TimeUnit unit)
			{
				return e.Schedule(callable, delay, unit);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ScheduledFuture<?> scheduleAtFixedRate(Runnable command, long initialDelay, long period, TimeUnit unit)
			public virtual ScheduledFuture<?> ScheduleAtFixedRate(Runnable command, long initialDelay, long period, TimeUnit unit)
			{
				return e.ScheduleAtFixedRate(command, initialDelay, period, unit);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ScheduledFuture<?> scheduleWithFixedDelay(Runnable command, long initialDelay, long delay, TimeUnit unit)
			public virtual ScheduledFuture<?> ScheduleWithFixedDelay(Runnable command, long initialDelay, long delay, TimeUnit unit)
			{
				return e.ScheduleWithFixedDelay(command, initialDelay, delay, unit);
			}
		}

		/// <summary>
		/// Cannot instantiate. </summary>
		private Executors()
		{
		}
	}

}