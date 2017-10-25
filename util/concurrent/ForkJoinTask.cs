using System;
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


	/// <summary>
	/// Abstract base class for tasks that run within a <seealso cref="ForkJoinPool"/>.
	/// A {@code ForkJoinTask} is a thread-like entity that is much
	/// lighter weight than a normal thread.  Huge numbers of tasks and
	/// subtasks may be hosted by a small number of actual threads in a
	/// ForkJoinPool, at the price of some usage limitations.
	/// 
	/// <para>A "main" {@code ForkJoinTask} begins execution when it is
	/// explicitly submitted to a <seealso cref="ForkJoinPool"/>, or, if not already
	/// engaged in a ForkJoin computation, commenced in the {@link
	/// ForkJoinPool#commonPool()} via <seealso cref="#fork"/>, <seealso cref="#invoke"/>, or
	/// related methods.  Once started, it will usually in turn start other
	/// subtasks.  As indicated by the name of this class, many programs
	/// using {@code ForkJoinTask} employ only methods <seealso cref="#fork"/> and
	/// <seealso cref="#join"/>, or derivatives such as {@link
	/// #invokeAll(ForkJoinTask...) invokeAll}.  However, this class also
	/// provides a number of other methods that can come into play in
	/// advanced usages, as well as extension mechanics that allow support
	/// of new forms of fork/join processing.
	/// 
	/// </para>
	/// <para>A {@code ForkJoinTask} is a lightweight form of <seealso cref="Future"/>.
	/// The efficiency of {@code ForkJoinTask}s stems from a set of
	/// restrictions (that are only partially statically enforceable)
	/// reflecting their main use as computational tasks calculating pure
	/// functions or operating on purely isolated objects.  The primary
	/// coordination mechanisms are <seealso cref="#fork"/>, that arranges
	/// asynchronous execution, and <seealso cref="#join"/>, that doesn't proceed
	/// until the task's result has been computed.  Computations should
	/// ideally avoid {@code synchronized} methods or blocks, and should
	/// minimize other blocking synchronization apart from joining other
	/// tasks or using synchronizers such as Phasers that are advertised to
	/// cooperate with fork/join scheduling. Subdividable tasks should also
	/// not perform blocking I/O, and should ideally access variables that
	/// are completely independent of those accessed by other running
	/// tasks. These guidelines are loosely enforced by not permitting
	/// checked exceptions such as {@code IOExceptions} to be
	/// thrown. However, computations may still encounter unchecked
	/// exceptions, that are rethrown to callers attempting to join
	/// them. These exceptions may additionally include {@link
	/// RejectedExecutionException} stemming from internal resource
	/// exhaustion, such as failure to allocate internal task
	/// queues. Rethrown exceptions behave in the same way as regular
	/// exceptions, but, when possible, contain stack traces (as displayed
	/// for example using {@code ex.printStackTrace()}) of both the thread
	/// that initiated the computation as well as the thread actually
	/// encountering the exception; minimally only the latter.
	/// 
	/// </para>
	/// <para>It is possible to define and use ForkJoinTasks that may block,
	/// but doing do requires three further considerations: (1) Completion
	/// of few if any <em>other</em> tasks should be dependent on a task
	/// that blocks on external synchronization or I/O. Event-style async
	/// tasks that are never joined (for example, those subclassing {@link
	/// CountedCompleter}) often fall into this category.  (2) To minimize
	/// resource impact, tasks should be small; ideally performing only the
	/// (possibly) blocking action. (3) Unless the {@link
	/// ForkJoinPool.ManagedBlocker} API is used, or the number of possibly
	/// blocked tasks is known to be less than the pool's {@link
	/// ForkJoinPool#getParallelism} level, the pool cannot guarantee that
	/// enough threads will be available to ensure progress or good
	/// performance.
	/// 
	/// </para>
	/// <para>The primary method for awaiting completion and extracting
	/// results of a task is <seealso cref="#join"/>, but there are several variants:
	/// The <seealso cref="Future#get"/> methods support interruptible and/or timed
	/// waits for completion and report results using {@code Future}
	/// conventions. Method <seealso cref="#invoke"/> is semantically
	/// equivalent to {@code fork(); join()} but always attempts to begin
	/// execution in the current thread. The "<em>quiet</em>" forms of
	/// these methods do not extract results or report exceptions. These
	/// may be useful when a set of tasks are being executed, and you need
	/// to delay processing of results or exceptions until all complete.
	/// Method {@code invokeAll} (available in multiple versions)
	/// performs the most common form of parallel invocation: forking a set
	/// of tasks and joining them all.
	/// 
	/// </para>
	/// <para>In the most typical usages, a fork-join pair act like a call
	/// (fork) and return (join) from a parallel recursive function. As is
	/// the case with other forms of recursive calls, returns (joins)
	/// should be performed innermost-first. For example, {@code a.fork();
	/// b.fork(); b.join(); a.join();} is likely to be substantially more
	/// efficient than joining {@code a} before {@code b}.
	/// 
	/// </para>
	/// <para>The execution status of tasks may be queried at several levels
	/// of detail: <seealso cref="#isDone"/> is true if a task completed in any way
	/// (including the case where a task was cancelled without executing);
	/// <seealso cref="#isCompletedNormally"/> is true if a task completed without
	/// cancellation or encountering an exception; <seealso cref="#isCancelled"/> is
	/// true if the task was cancelled (in which case <seealso cref="#getException"/>
	/// returns a <seealso cref="java.util.concurrent.CancellationException"/>); and
	/// <seealso cref="#isCompletedAbnormally"/> is true if a task was either
	/// cancelled or encountered an exception, in which case {@link
	/// #getException} will return either the encountered exception or
	/// <seealso cref="java.util.concurrent.CancellationException"/>.
	/// 
	/// </para>
	/// <para>The ForkJoinTask class is not usually directly subclassed.
	/// Instead, you subclass one of the abstract classes that support a
	/// particular style of fork/join processing, typically {@link
	/// RecursiveAction} for most computations that do not return results,
	/// <seealso cref="RecursiveTask"/> for those that do, and {@link
	/// CountedCompleter} for those in which completed actions trigger
	/// other actions.  Normally, a concrete ForkJoinTask subclass declares
	/// fields comprising its parameters, established in a constructor, and
	/// then defines a {@code compute} method that somehow uses the control
	/// methods supplied by this base class.
	/// 
	/// </para>
	/// <para>Method <seealso cref="#join"/> and its variants are appropriate for use
	/// only when completion dependencies are acyclic; that is, the
	/// parallel computation can be described as a directed acyclic graph
	/// (DAG). Otherwise, executions may encounter a form of deadlock as
	/// tasks cyclically wait for each other.  However, this framework
	/// supports other methods and techniques (for example the use of
	/// <seealso cref="Phaser"/>, <seealso cref="#helpQuiesce"/>, and <seealso cref="#complete"/>) that
	/// may be of use in constructing custom subclasses for problems that
	/// are not statically structured as DAGs. To support such usages, a
	/// ForkJoinTask may be atomically <em>tagged</em> with a {@code short}
	/// value using <seealso cref="#setForkJoinTaskTag"/> or {@link
	/// #compareAndSetForkJoinTaskTag} and checked using {@link
	/// #getForkJoinTaskTag}. The ForkJoinTask implementation does not use
	/// these {@code protected} methods or tags for any purpose, but they
	/// may be of use in the construction of specialized subclasses.  For
	/// example, parallel graph traversals can use the supplied methods to
	/// avoid revisiting nodes/tasks that have already been processed.
	/// (Method names for tagging are bulky in part to encourage definition
	/// of methods that reflect their usage patterns.)
	/// 
	/// </para>
	/// <para>Most base support methods are {@code final}, to prevent
	/// overriding of implementations that are intrinsically tied to the
	/// underlying lightweight task scheduling framework.  Developers
	/// creating new basic styles of fork/join processing should minimally
	/// implement {@code protected} methods <seealso cref="#exec"/>, {@link
	/// #setRawResult}, and <seealso cref="#getRawResult"/>, while also introducing
	/// an abstract computational method that can be implemented in its
	/// subclasses, possibly relying on other {@code protected} methods
	/// provided by this class.
	/// 
	/// </para>
	/// <para>ForkJoinTasks should perform relatively small amounts of
	/// computation. Large tasks should be split into smaller subtasks,
	/// usually via recursive decomposition. As a very rough rule of thumb,
	/// a task should perform more than 100 and less than 10000 basic
	/// computational steps, and should avoid indefinite looping. If tasks
	/// are too big, then parallelism cannot improve throughput. If too
	/// small, then memory and internal task maintenance overhead may
	/// overwhelm processing.
	/// 
	/// </para>
	/// <para>This class provides {@code adapt} methods for <seealso cref="Runnable"/>
	/// and <seealso cref="Callable"/>, that may be of use when mixing execution of
	/// {@code ForkJoinTasks} with other kinds of tasks. When all tasks are
	/// of this form, consider using a pool constructed in <em>asyncMode</em>.
	/// 
	/// </para>
	/// <para>ForkJoinTasks are {@code Serializable}, which enables them to be
	/// used in extensions such as remote execution frameworks. It is
	/// sensible to serialize tasks only before or after, but not during,
	/// execution. Serialization is not relied on during execution itself.
	/// 
	/// @since 1.7
	/// @author Doug Lea
	/// </para>
	/// </summary>
	[Serializable]
	public abstract class ForkJoinTask<V> : Future<V>
	{

		/*
		 * See the internal documentation of class ForkJoinPool for a
		 * general implementation overview.  ForkJoinTasks are mainly
		 * responsible for maintaining their "status" field amidst relays
		 * to methods in ForkJoinWorkerThread and ForkJoinPool.
		 *
		 * The methods of this class are more-or-less layered into
		 * (1) basic status maintenance
		 * (2) execution and awaiting completion
		 * (3) user-level methods that additionally report results.
		 * This is sometimes hard to see because this file orders exported
		 * methods in a way that flows well in javadocs.
		 */

		/*
		 * The status field holds run control status bits packed into a
		 * single int to minimize footprint and to ensure atomicity (via
		 * CAS).  Status is initially zero, and takes on nonnegative
		 * values until completed, upon which status (anded with
		 * DONE_MASK) holds value NORMAL, CANCELLED, or EXCEPTIONAL. Tasks
		 * undergoing blocking waits by other threads have the SIGNAL bit
		 * set.  Completion of a stolen task with SIGNAL set awakens any
		 * waiters via notifyAll. Even though suboptimal for some
		 * purposes, we use basic builtin wait/notify to take advantage of
		 * "monitor inflation" in JVMs that we would otherwise need to
		 * emulate to avoid adding further per-task bookkeeping overhead.
		 * We want these monitors to be "fat", i.e., not use biasing or
		 * thin-lock techniques, so use some odd coding idioms that tend
		 * to avoid them, mainly by arranging that every synchronized
		 * block performs a wait, notifyAll or both.
		 *
		 * These control bits occupy only (some of) the upper half (16
		 * bits) of status field. The lower bits are used for user-defined
		 * tags.
		 */

		/// <summary>
		/// The run status of this task </summary>
		internal volatile int Status; // accessed directly by pool and workers
		internal const int DONE_MASK = unchecked((int)0xf0000000); // mask out non-completion bits
		internal const int NORMAL = unchecked((int)0xf0000000); // must be negative
		internal const int CANCELLED = unchecked((int)0xc0000000); // must be < NORMAL
		internal const int EXCEPTIONAL = unchecked((int)0x80000000); // must be < CANCELLED
		internal const int SIGNAL = 0x00010000; // must be >= 1 << 16
		internal const int SMASK = 0x0000ffff; // short bits for tags

		/// <summary>
		/// Marks completion and wakes up threads waiting to join this
		/// task.
		/// </summary>
		/// <param name="completion"> one of NORMAL, CANCELLED, EXCEPTIONAL </param>
		/// <returns> completion status on exit </returns>
		private int SetCompletion(int completion)
		{
			for (int s;;)
			{
				if ((s = Status) < 0)
				{
					return s;
				}
				if (U.compareAndSwapInt(this, STATUS, s, s | completion))
				{
					if (((int)((uint)s >> 16)) != 0)
					{
						lock (this)
						{
						Monitor.PulseAll(this);
						}
					}
					return completion;
				}
			}
		}

		/// <summary>
		/// Primary execution method for stolen tasks. Unless done, calls
		/// exec and records status if completed, but doesn't wait for
		/// completion otherwise.
		/// </summary>
		/// <returns> status on exit from this method </returns>
		internal int DoExec()
		{
			int s;
			bool completed;
			if ((s = Status) >= 0)
			{
				try
				{
					completed = Exec();
				}
				catch (Throwable rex)
				{
					return setExceptionalCompletion(rex);
				}
				if (completed)
				{
					s = setCompletion(NORMAL);
				}
			}
			return s;
		}

		/// <summary>
		/// If not done, sets SIGNAL status and performs Object.wait(timeout).
		/// This task may or may not be done on exit. Ignores interrupts.
		/// </summary>
		/// <param name="timeout"> using Object.wait conventions. </param>
		internal void InternalWait(long timeout)
		{
			int s;
			if ((s = Status) >= 0 && U.compareAndSwapInt(this, STATUS, s, s | SIGNAL)) // force completer to issue notify
			{
				lock (this)
				{
					if (Status >= 0)
					{
						try
						{
						Monitor.Wait(this, TimeSpan.FromMilliseconds(timeout));
						}
					catch (InterruptedException)
					{
					}
					}
					else
					{
						Monitor.PulseAll(this);
					}
				}
			}
		}

		/// <summary>
		/// Blocks a non-worker-thread until completion. </summary>
		/// <returns> status upon completion </returns>
		private int ExternalAwaitDone()
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: int s = ((this instanceof CountedCompleter) ? ForkJoinPool.common.externalHelpComplete((CountedCompleter<?>)this, 0) : ForkJoinPool.common.tryExternalUnpush(this) ? doExec() : 0);
			int s = ((this is CountedCompleter) ? ForkJoinPool.Common.ExternalHelpComplete((CountedCompleter<?>)this, 0) : ForkJoinPool.Common.TryExternalUnpush(this) ? DoExec() : 0); // try helping
			if (s >= 0 && (s = Status) >= 0)
			{
				bool interrupted = false;
				do
				{
					if (U.compareAndSwapInt(this, STATUS, s, s | SIGNAL))
					{
						lock (this)
						{
							if (Status >= 0)
							{
								try
								{
									Monitor.Wait(this, TimeSpan.FromMilliseconds(0L));
								}
								catch (InterruptedException)
								{
									interrupted = true;
								}
							}
							else
							{
								Monitor.PulseAll(this);
							}
						}
					}
				} while ((s = Status) >= 0);
				if (interrupted)
				{
					Thread.CurrentThread.Interrupt();
				}
			}
			return s;
		}

		/// <summary>
		/// Blocks a non-worker-thread until completion or interruption.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int externalInterruptibleAwaitDone() throws InterruptedException
		private int ExternalInterruptibleAwaitDone()
		{
			int s;
			if (Thread.Interrupted())
			{
				throw new InterruptedException();
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if ((s = status) >= 0 && (s = ((this instanceof CountedCompleter) ? ForkJoinPool.common.externalHelpComplete((CountedCompleter<?>)this, 0) : ForkJoinPool.common.tryExternalUnpush(this) ? doExec() : 0)) >= 0)
			if ((s = Status) >= 0 && (s = ((this is CountedCompleter) ? ForkJoinPool.Common.ExternalHelpComplete((CountedCompleter<?>)this, 0) : ForkJoinPool.Common.TryExternalUnpush(this) ? DoExec() : 0)) >= 0)
			{
				while ((s = Status) >= 0)
				{
					if (U.compareAndSwapInt(this, STATUS, s, s | SIGNAL))
					{
						lock (this)
						{
							if (Status >= 0)
							{
								Monitor.Wait(this, TimeSpan.FromMilliseconds(0L));
							}
							else
							{
								Monitor.PulseAll(this);
							}
						}
					}
				}
			}
			return s;
		}

		/// <summary>
		/// Implementation for join, get, quietlyJoin. Directly handles
		/// only cases of already-completed, external wait, and
		/// unfork+exec.  Others are relayed to ForkJoinPool.awaitJoin.
		/// </summary>
		/// <returns> status upon completion </returns>
		private int DoJoin()
		{
			int s;
			Thread t;
			ForkJoinWorkerThread wt;
			ForkJoinPool.WorkQueue w;
			return (s = Status) < 0 ? s : ((t = Thread.CurrentThread) is ForkJoinWorkerThread) ? (w = (wt = (ForkJoinWorkerThread)t).workQueue).tryUnpush(this) && (s = DoExec()) < 0 ? s : wt.Pool_Renamed.awaitJoin(w, this, 0L) : ExternalAwaitDone();
		}

		/// <summary>
		/// Implementation for invoke, quietlyInvoke.
		/// </summary>
		/// <returns> status upon completion </returns>
		private int DoInvoke()
		{
			int s;
			Thread t;
			ForkJoinWorkerThread wt;
			return (s = DoExec()) < 0 ? s : ((t = Thread.CurrentThread) is ForkJoinWorkerThread) ? (wt = (ForkJoinWorkerThread)t).pool.awaitJoin(wt.WorkQueue, this, 0L) : ExternalAwaitDone();
		}

		// Exception table support

		/// <summary>
		/// Table of exceptions thrown by tasks, to enable reporting by
		/// callers. Because exceptions are rare, we don't directly keep
		/// them with task objects, but instead use a weak ref table.  Note
		/// that cancellation exceptions don't appear in the table, but are
		/// instead recorded as status values.
		/// 
		/// Note: These statics are initialized below in static block.
		/// </summary>
		private static readonly ExceptionNode[] ExceptionTable;
		private static readonly ReentrantLock ExceptionTableLock;
		private static readonly ReferenceQueue<Object> ExceptionTableRefQueue;

		/// <summary>
		/// Fixed capacity for exceptionTable.
		/// </summary>
		private const int EXCEPTION_MAP_CAPACITY = 32;

		/// <summary>
		/// Key-value nodes for exception table.  The chained hash table
		/// uses identity comparisons, full locking, and weak references
		/// for keys. The table has a fixed capacity because it only
		/// maintains task exceptions long enough for joiners to access
		/// them, so should never become very large for sustained
		/// periods. However, since we do not know when the last joiner
		/// completes, we must use weak references and expunge them. We do
		/// so on each operation (hence full locking). Also, some thread in
		/// any ForkJoinPool will call helpExpungeStaleExceptions when its
		/// pool becomes isQuiescent.
		/// </summary>
		internal sealed class ExceptionNode : WeakReference<ForkJoinTask<JavaToDotNetGenericWildcard>>
		{
			internal readonly Throwable Ex;
			internal ExceptionNode Next;
			internal readonly long Thrower; // use id not ref to avoid weak cycles
			internal readonly int HashCode; // store task hashCode before weak ref disappears
			internal ExceptionNode<T1>(ForkJoinTask<T1> task, Throwable ex, ExceptionNode next) : base(task, ExceptionTableRefQueue)
			{
				this.Ex = ex;
				this.Next = next;
				this.Thrower = Thread.CurrentThread.Id;
				this.HashCode = System.identityHashCode(task);
			}
		}

		/// <summary>
		/// Records exception and sets status.
		/// </summary>
		/// <returns> status on exit </returns>
		internal int RecordExceptionalCompletion(Throwable ex)
		{
			int s;
			if ((s = Status) >= 0)
			{
				int h = System.identityHashCode(this);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = exceptionTableLock;
				ReentrantLock @lock = ExceptionTableLock;
				@lock.@lock();
				try
				{
					ExpungeStaleExceptions();
					ExceptionNode[] t = ExceptionTable;
					int i = h & (t.Length - 1);
					for (ExceptionNode e = t[i]; ; e = e.Next)
					{
						if (e == null)
						{
							t[i] = new ExceptionNode(this, ex, t[i]);
							break;
						}
						if (e.get() == this) // already present
						{
							break;
						}
					}
				}
				finally
				{
					@lock.Unlock();
				}
				s = setCompletion(EXCEPTIONAL);
			}
			return s;
		}

		/// <summary>
		/// Records exception and possibly propagates.
		/// </summary>
		/// <returns> status on exit </returns>
		private int SetExceptionalCompletion(Throwable ex)
		{
			int s = RecordExceptionalCompletion(ex);
			if ((s & DONE_MASK) == EXCEPTIONAL)
			{
				InternalPropagateException(ex);
			}
			return s;
		}

		/// <summary>
		/// Hook for exception propagation support for tasks with completers.
		/// </summary>
		internal virtual void InternalPropagateException(Throwable ex)
		{
		}

		/// <summary>
		/// Cancels, ignoring any exceptions thrown by cancel. Used during
		/// worker and pool shutdown. Cancel is spec'ed not to throw any
		/// exceptions, but if it does anyway, we have no recourse during
		/// shutdown, so guard against this case.
		/// </summary>
		internal static void cancelIgnoringExceptions<T1>(ForkJoinTask<T1> t)
		{
			if (t != null && t.Status >= 0)
			{
				try
				{
					t.Cancel(false);
				}
				catch (Throwable)
				{
				}
			}
		}

		/// <summary>
		/// Removes exception node and clears status.
		/// </summary>
		private void ClearExceptionalCompletion()
		{
			int h = System.identityHashCode(this);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = exceptionTableLock;
			ReentrantLock @lock = ExceptionTableLock;
			@lock.@lock();
			try
			{
				ExceptionNode[] t = ExceptionTable;
				int i = h & (t.Length - 1);
				ExceptionNode e = t[i];
				ExceptionNode pred = null;
				while (e != null)
				{
					ExceptionNode next = e.Next;
					if (e.get() == this)
					{
						if (pred == null)
						{
							t[i] = next;
						}
						else
						{
							pred.Next = next;
						}
						break;
					}
					pred = e;
					e = next;
				}
				ExpungeStaleExceptions();
				Status = 0;
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Returns a rethrowable exception for the given task, if
		/// available. To provide accurate stack traces, if the exception
		/// was not thrown by the current thread, we try to create a new
		/// exception of the same type as the one thrown, but with the
		/// recorded exception as its cause. If there is no such
		/// constructor, we instead try to use a no-arg constructor,
		/// followed by initCause, to the same effect. If none of these
		/// apply, or any fail due to other exceptions, we return the
		/// recorded exception, which is still correct, although it may
		/// contain a misleading stack trace.
		/// </summary>
		/// <returns> the exception, or null if none </returns>
		private Throwable ThrowableException
		{
			get
			{
				if ((Status & DONE_MASK) != EXCEPTIONAL)
				{
					return null;
				}
				int h = System.identityHashCode(this);
				ExceptionNode e;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = exceptionTableLock;
				ReentrantLock @lock = ExceptionTableLock;
				@lock.@lock();
				try
				{
					ExpungeStaleExceptions();
					ExceptionNode[] t = ExceptionTable;
					e = t[h & (t.Length - 1)];
					while (e != null && e.get() != this)
					{
						e = e.Next;
					}
				}
				finally
				{
					@lock.Unlock();
				}
				Throwable ex;
				if (e == null || (ex = e.Ex) == null)
				{
					return null;
				}
				if (e.Thrower != Thread.CurrentThread.Id)
				{
					Class ec = ex.GetType();
					try
					{
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: Constructor<?> noArgCtor = null;
						Constructor<?> noArgCtor = null;
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: Constructor<?>[] cs = ec.getConstructors();
						Constructor<?>[] cs = ec.Constructors; // public ctors only
						for (int i = 0; i < cs.Length; ++i)
						{
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: Constructor<?> c = cs[i];
							Constructor<?> c = cs[i];
							Class[] ps = c.ParameterTypes;
							if (ps.Length == 0)
							{
								noArgCtor = c;
							}
							else if (ps.Length == 1 && ps[0] == typeof(Throwable))
							{
								Throwable wx = (Throwable)c.newInstance(ex);
								return (wx == null) ? ex : wx;
							}
						}
						if (noArgCtor != null)
						{
							Throwable wx = (Throwable)(noArgCtor.newInstance());
							if (wx != null)
							{
								wx.InitCause(ex);
								return wx;
							}
						}
					}
					catch (Exception)
					{
					}
				}
				return ex;
			}
		}

		/// <summary>
		/// Poll stale refs and remove them. Call only while holding lock.
		/// </summary>
		private static void ExpungeStaleExceptions()
		{
			for (Object x; (x = ExceptionTableRefQueue.poll()) != null;)
			{
				if (x is ExceptionNode)
				{
					int hashCode = ((ExceptionNode)x).HashCode;
					ExceptionNode[] t = ExceptionTable;
					int i = hashCode & (t.Length - 1);
					ExceptionNode e = t[i];
					ExceptionNode pred = null;
					while (e != null)
					{
						ExceptionNode next = e.Next;
						if (e == x)
						{
							if (pred == null)
							{
								t[i] = next;
							}
							else
							{
								pred.Next = next;
							}
							break;
						}
						pred = e;
						e = next;
					}
				}
			}
		}

		/// <summary>
		/// If lock is available, poll stale refs and remove them.
		/// Called from ForkJoinPool when pools become quiescent.
		/// </summary>
		internal static void HelpExpungeStaleExceptions()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = exceptionTableLock;
			ReentrantLock @lock = ExceptionTableLock;
			if (@lock.TryLock())
			{
				try
				{
					ExpungeStaleExceptions();
				}
				finally
				{
					@lock.Unlock();
				}
			}
		}

		/// <summary>
		/// A version of "sneaky throw" to relay exceptions
		/// </summary>
		internal static void Rethrow(Throwable ex)
		{
			if (ex != null)
			{
				ForkJoinTask.UncheckedThrow<RuntimeException>(ex);
			}
		}

		/// <summary>
		/// The sneaky part of sneaky throw, relying on generics
		/// limitations to evade compiler complaints about rethrowing
		/// unchecked exceptions
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static <T extends Throwable> void uncheckedThrow(Throwable t) throws T
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		internal static void uncheckedThrow<T>(Throwable t) where T : Throwable
		{
			throw (T)t; // rely on vacuous cast
		}

		/// <summary>
		/// Throws exception, if any, associated with the given status.
		/// </summary>
		private void ReportException(int s)
		{
			if (s == CANCELLED)
			{
				throw new CancellationException();
			}
			if (s == EXCEPTIONAL)
			{
				Rethrow(ThrowableException);
			}
		}

		// public methods

		/// <summary>
		/// Arranges to asynchronously execute this task in the pool the
		/// current task is running in, if applicable, or using the {@link
		/// ForkJoinPool#commonPool()} if not <seealso cref="#inForkJoinPool"/>.  While
		/// it is not necessarily enforced, it is a usage error to fork a
		/// task more than once unless it has completed and been
		/// reinitialized.  Subsequent modifications to the state of this
		/// task or any data it operates on are not necessarily
		/// consistently observable by any thread other than the one
		/// executing it unless preceded by a call to <seealso cref="#join"/> or
		/// related methods, or a call to <seealso cref="#isDone"/> returning {@code
		/// true}.
		/// </summary>
		/// <returns> {@code this}, to simplify usage </returns>
		public ForkJoinTask<V> Fork()
		{
			Thread t;
			if ((t = Thread.CurrentThread) is ForkJoinWorkerThread)
			{
				((ForkJoinWorkerThread)t).WorkQueue.Push(this);
			}
			else
			{
				ForkJoinPool.Common.ExternalPush(this);
			}
			return this;
		}

		/// <summary>
		/// Returns the result of the computation when it {@link #isDone is
		/// done}.  This method differs from <seealso cref="#get()"/> in that
		/// abnormal completion results in {@code RuntimeException} or
		/// {@code Error}, not {@code ExecutionException}, and that
		/// interrupts of the calling thread do <em>not</em> cause the
		/// method to abruptly return by throwing {@code
		/// InterruptedException}.
		/// </summary>
		/// <returns> the computed result </returns>
		public V Join()
		{
			int s;
			if ((s = DoJoin() & DONE_MASK) != NORMAL)
			{
				ReportException(s);
			}
			return RawResult;
		}

		/// <summary>
		/// Commences performing this task, awaits its completion if
		/// necessary, and returns its result, or throws an (unchecked)
		/// {@code RuntimeException} or {@code Error} if the underlying
		/// computation did so.
		/// </summary>
		/// <returns> the computed result </returns>
		public V Invoke()
		{
			int s;
			if ((s = DoInvoke() & DONE_MASK) != NORMAL)
			{
				ReportException(s);
			}
			return RawResult;
		}

		/// <summary>
		/// Forks the given tasks, returning when {@code isDone} holds for
		/// each task or an (unchecked) exception is encountered, in which
		/// case the exception is rethrown. If more than one task
		/// encounters an exception, then this method throws any one of
		/// these exceptions. If any task encounters an exception, the
		/// other may be cancelled. However, the execution status of
		/// individual tasks is not guaranteed upon exceptional return. The
		/// status of each task may be obtained using {@link
		/// #getException()} and related methods to check if they have been
		/// cancelled, completed normally or exceptionally, or left
		/// unprocessed.
		/// </summary>
		/// <param name="t1"> the first task </param>
		/// <param name="t2"> the second task </param>
		/// <exception cref="NullPointerException"> if any task is null </exception>
		public static void invokeAll<T1, T2>(ForkJoinTask<T1> t1, ForkJoinTask<T2> t2)
		{
			int s1, s2;
			t2.Fork();
			if ((s1 = t1.DoInvoke() & DONE_MASK) != NORMAL)
			{
				t1.ReportException(s1);
			}
			if ((s2 = t2.DoJoin() & DONE_MASK) != NORMAL)
			{
				t2.ReportException(s2);
			}
		}

		/// <summary>
		/// Forks the given tasks, returning when {@code isDone} holds for
		/// each task or an (unchecked) exception is encountered, in which
		/// case the exception is rethrown. If more than one task
		/// encounters an exception, then this method throws any one of
		/// these exceptions. If any task encounters an exception, others
		/// may be cancelled. However, the execution status of individual
		/// tasks is not guaranteed upon exceptional return. The status of
		/// each task may be obtained using <seealso cref="#getException()"/> and
		/// related methods to check if they have been cancelled, completed
		/// normally or exceptionally, or left unprocessed.
		/// </summary>
		/// <param name="tasks"> the tasks </param>
		/// <exception cref="NullPointerException"> if any task is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static void invokeAll(ForkJoinTask<?>... tasks)
		public static void InvokeAll(params ForkJoinTask<?>[] tasks)
		{
			Throwable ex = null;
			int last = tasks.Length - 1;
			for (int i = last; i >= 0; --i)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t = tasks[i];
				ForkJoinTask<?> t = tasks[i];
				if (t == null)
				{
					if (ex == null)
					{
						ex = new NullPointerException();
					}
				}
				else if (i != 0)
				{
					t.Fork();
				}
				else if (t.DoInvoke() < NORMAL && ex == null)
				{
					ex = t.Exception;
				}
			}
			for (int i = 1; i <= last; ++i)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t = tasks[i];
				ForkJoinTask<?> t = tasks[i];
				if (t != null)
				{
					if (ex != null)
					{
						t.Cancel(false);
					}
					else if (t.DoJoin() < NORMAL)
					{
						ex = t.Exception;
					}
				}
			}
			if (ex != null)
			{
				Rethrow(ex);
			}
		}

		/// <summary>
		/// Forks all tasks in the specified collection, returning when
		/// {@code isDone} holds for each task or an (unchecked) exception
		/// is encountered, in which case the exception is rethrown. If
		/// more than one task encounters an exception, then this method
		/// throws any one of these exceptions. If any task encounters an
		/// exception, others may be cancelled. However, the execution
		/// status of individual tasks is not guaranteed upon exceptional
		/// return. The status of each task may be obtained using {@link
		/// #getException()} and related methods to check if they have been
		/// cancelled, completed normally or exceptionally, or left
		/// unprocessed.
		/// </summary>
		/// <param name="tasks"> the collection of tasks </param>
		/// @param <T> the type of the values returned from the tasks </param>
		/// <returns> the tasks argument, to simplify usage </returns>
		/// <exception cref="NullPointerException"> if tasks or any element are null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T extends ForkJoinTask<?>> java.util.Collection<T> invokeAll(java.util.Collection<T> tasks)
		public static ICollection<T> invokeAll<T>(ICollection<T> tasks) where T : ForkJoinTask<?>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (!(tasks instanceof java.util.RandomAccess) || !(tasks instanceof java.util.List<?>))
			if (!(tasks is RandomAccess) || !(tasks is IList<?>))
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: invokeAll(tasks.toArray(new ForkJoinTask<?>[tasks.size()]));
				InvokeAll(tasks.toArray(new ForkJoinTask<?>[tasks.Count]));
				return tasks;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<? extends ForkJoinTask<?>> ts = (java.util.List<? extends ForkJoinTask<?>>) tasks;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IList<?> ts = (IList<?>) tasks;
			Throwable ex = null;
			int last = ts.Count - 1;
			for (int i = last; i >= 0; --i)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t = ts.get(i);
				ForkJoinTask<?> t = ts[i];
				if (t == null)
				{
					if (ex == null)
					{
						ex = new NullPointerException();
					}
				}
				else if (i != 0)
				{
					t.Fork();
				}
				else if (t.DoInvoke() < NORMAL && ex == null)
				{
					ex = t.Exception;
				}
			}
			for (int i = 1; i <= last; ++i)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ForkJoinTask<?> t = ts.get(i);
				ForkJoinTask<?> t = ts[i];
				if (t != null)
				{
					if (ex != null)
					{
						t.Cancel(false);
					}
					else if (t.DoJoin() < NORMAL)
					{
						ex = t.Exception;
					}
				}
			}
			if (ex != null)
			{
				Rethrow(ex);
			}
			return tasks;
		}

		/// <summary>
		/// Attempts to cancel execution of this task. This attempt will
		/// fail if the task has already completed or could not be
		/// cancelled for some other reason. If successful, and this task
		/// has not started when {@code cancel} is called, execution of
		/// this task is suppressed. After this method returns
		/// successfully, unless there is an intervening call to {@link
		/// #reinitialize}, subsequent calls to <seealso cref="#isCancelled"/>,
		/// <seealso cref="#isDone"/>, and {@code cancel} will return {@code true}
		/// and calls to <seealso cref="#join"/> and related methods will result in
		/// {@code CancellationException}.
		/// 
		/// <para>This method may be overridden in subclasses, but if so, must
		/// still ensure that these properties hold. In particular, the
		/// {@code cancel} method itself must not throw exceptions.
		/// 
		/// </para>
		/// <para>This method is designed to be invoked by <em>other</em>
		/// tasks. To terminate the current task, you can just return or
		/// throw an unchecked exception from its computation method, or
		/// invoke <seealso cref="#completeExceptionally(Throwable)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="mayInterruptIfRunning"> this value has no effect in the
		/// default implementation because interrupts are not used to
		/// control cancellation.
		/// </param>
		/// <returns> {@code true} if this task is now cancelled </returns>
		public virtual bool Cancel(bool mayInterruptIfRunning)
		{
			return (setCompletion(CANCELLED) & DONE_MASK) == CANCELLED;
		}

		public bool Done
		{
			get
			{
				return Status < 0;
			}
		}

		public bool Cancelled
		{
			get
			{
				return (Status & DONE_MASK) == CANCELLED;
			}
		}

		/// <summary>
		/// Returns {@code true} if this task threw an exception or was cancelled.
		/// </summary>
		/// <returns> {@code true} if this task threw an exception or was cancelled </returns>
		public bool CompletedAbnormally
		{
			get
			{
				return Status < NORMAL;
			}
		}

		/// <summary>
		/// Returns {@code true} if this task completed without throwing an
		/// exception and was not cancelled.
		/// </summary>
		/// <returns> {@code true} if this task completed without throwing an
		/// exception and was not cancelled </returns>
		public bool CompletedNormally
		{
			get
			{
				return (Status & DONE_MASK) == NORMAL;
			}
		}

		/// <summary>
		/// Returns the exception thrown by the base computation, or a
		/// {@code CancellationException} if cancelled, or {@code null} if
		/// none or if the method has not yet completed.
		/// </summary>
		/// <returns> the exception, or {@code null} if none </returns>
		public Throwable Exception
		{
			get
			{
				int s = Status & DONE_MASK;
				return ((s >= NORMAL) ? null : (s == CANCELLED) ? new CancellationException() : ThrowableException);
			}
		}

		/// <summary>
		/// Completes this task abnormally, and if not already aborted or
		/// cancelled, causes it to throw the given exception upon
		/// {@code join} and related operations. This method may be used
		/// to induce exceptions in asynchronous tasks, or to force
		/// completion of tasks that would not otherwise complete.  Its use
		/// in other situations is discouraged.  This method is
		/// overridable, but overridden versions must invoke {@code super}
		/// implementation to maintain guarantees.
		/// </summary>
		/// <param name="ex"> the exception to throw. If this exception is not a
		/// {@code RuntimeException} or {@code Error}, the actual exception
		/// thrown will be a {@code RuntimeException} with cause {@code ex}. </param>
		public virtual void CompleteExceptionally(Throwable ex)
		{
			ExceptionalCompletion = (ex is RuntimeException) || (ex is Error) ? ex : new RuntimeException(ex);
		}

		/// <summary>
		/// Completes this task, and if not already aborted or cancelled,
		/// returning the given value as the result of subsequent
		/// invocations of {@code join} and related operations. This method
		/// may be used to provide results for asynchronous tasks, or to
		/// provide alternative handling for tasks that would not otherwise
		/// complete normally. Its use in other situations is
		/// discouraged. This method is overridable, but overridden
		/// versions must invoke {@code super} implementation to maintain
		/// guarantees.
		/// </summary>
		/// <param name="value"> the result value for this task </param>
		public virtual void Complete(V value)
		{
			try
			{
				RawResult = value;
			}
			catch (Throwable rex)
			{
				ExceptionalCompletion = rex;
				return;
			}
			Completion = NORMAL;
		}

		/// <summary>
		/// Completes this task normally without setting a value. The most
		/// recent value established by <seealso cref="#setRawResult"/> (or {@code
		/// null} by default) will be returned as the result of subsequent
		/// invocations of {@code join} and related operations.
		/// 
		/// @since 1.8
		/// </summary>
		public void QuietlyComplete()
		{
			Completion = NORMAL;
		}

		/// <summary>
		/// Waits if necessary for the computation to complete, and then
		/// retrieves its result.
		/// </summary>
		/// <returns> the computed result </returns>
		/// <exception cref="CancellationException"> if the computation was cancelled </exception>
		/// <exception cref="ExecutionException"> if the computation threw an
		/// exception </exception>
		/// <exception cref="InterruptedException"> if the current thread is not a
		/// member of a ForkJoinPool and was interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final V get() throws InterruptedException, java.util.concurrent.ExecutionException
		public V Get()
		{
			int s = (Thread.CurrentThread is ForkJoinWorkerThread) ? DoJoin() : ExternalInterruptibleAwaitDone();
			Throwable ex;
			if ((s &= DONE_MASK) == CANCELLED)
			{
				throw new CancellationException();
			}
			if (s == EXCEPTIONAL && (ex = ThrowableException) != null)
			{
				throw new ExecutionException(ex);
			}
			return RawResult;
		}

		/// <summary>
		/// Waits if necessary for at most the given time for the computation
		/// to complete, and then retrieves its result, if available.
		/// </summary>
		/// <param name="timeout"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the timeout argument </param>
		/// <returns> the computed result </returns>
		/// <exception cref="CancellationException"> if the computation was cancelled </exception>
		/// <exception cref="ExecutionException"> if the computation threw an
		/// exception </exception>
		/// <exception cref="InterruptedException"> if the current thread is not a
		/// member of a ForkJoinPool and was interrupted while waiting </exception>
		/// <exception cref="TimeoutException"> if the wait timed out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final V get(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException, java.util.concurrent.ExecutionException, java.util.concurrent.TimeoutException
		public V Get(long timeout, TimeUnit unit)
		{
			int s;
			long nanos = unit.ToNanos(timeout);
			if (Thread.Interrupted())
			{
				throw new InterruptedException();
			}
			if ((s = Status) >= 0 && nanos > 0L)
			{
				long d = System.nanoTime() + nanos;
				long deadline = (d == 0L) ? 1L : d; // avoid 0
				Thread t = Thread.CurrentThread;
				if (t is ForkJoinWorkerThread)
				{
					ForkJoinWorkerThread wt = (ForkJoinWorkerThread)t;
					s = wt.Pool_Renamed.awaitJoin(wt.WorkQueue, this, deadline);
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: else if ((s = ((this instanceof CountedCompleter) ? ForkJoinPool.common.externalHelpComplete((CountedCompleter<?>)this, 0) : ForkJoinPool.common.tryExternalUnpush(this) ? doExec() : 0)) >= 0)
				else if ((s = ((this is CountedCompleter) ? ForkJoinPool.Common.ExternalHelpComplete((CountedCompleter<?>)this, 0) : ForkJoinPool.Common.TryExternalUnpush(this) ? DoExec() : 0)) >= 0)
				{
					long ns, ms; // measure in nanosecs, but wait in millisecs
					while ((s = Status) >= 0 && (ns = deadline - System.nanoTime()) > 0L)
					{
						if ((ms = TimeUnit.NANOSECONDS.ToMillis(ns)) > 0L && U.compareAndSwapInt(this, STATUS, s, s | SIGNAL))
						{
							lock (this)
							{
								if (Status >= 0)
								{
									Monitor.Wait(this, TimeSpan.FromMilliseconds(ms)); // OK to throw InterruptedException
								}
								else
								{
									Monitor.PulseAll(this);
								}
							}
						}
					}
				}
			}
			if (s >= 0)
			{
				s = Status;
			}
			if ((s &= DONE_MASK) != NORMAL)
			{
				Throwable ex;
				if (s == CANCELLED)
				{
					throw new CancellationException();
				}
				if (s != EXCEPTIONAL)
				{
					throw new TimeoutException();
				}
				if ((ex = ThrowableException) != null)
				{
					throw new ExecutionException(ex);
				}
			}
			return RawResult;
		}

		/// <summary>
		/// Joins this task, without returning its result or throwing its
		/// exception. This method may be useful when processing
		/// collections of tasks when some have been cancelled or otherwise
		/// known to have aborted.
		/// </summary>
		public void QuietlyJoin()
		{
			DoJoin();
		}

		/// <summary>
		/// Commences performing this task and awaits its completion if
		/// necessary, without returning its result or throwing its
		/// exception.
		/// </summary>
		public void QuietlyInvoke()
		{
			DoInvoke();
		}

		/// <summary>
		/// Possibly executes tasks until the pool hosting the current task
		/// <seealso cref="ForkJoinPool#isQuiescent is quiescent"/>. This method may
		/// be of use in designs in which many tasks are forked, but none
		/// are explicitly joined, instead executing them until all are
		/// processed.
		/// </summary>
		public static void HelpQuiesce()
		{
			Thread t;
			if ((t = Thread.CurrentThread) is ForkJoinWorkerThread)
			{
				ForkJoinWorkerThread wt = (ForkJoinWorkerThread)t;
				wt.Pool_Renamed.helpQuiescePool(wt.WorkQueue);
			}
			else
			{
				ForkJoinPool.QuiesceCommonPool();
			}
		}

		/// <summary>
		/// Resets the internal bookkeeping state of this task, allowing a
		/// subsequent {@code fork}. This method allows repeated reuse of
		/// this task, but only if reuse occurs when this task has either
		/// never been forked, or has been forked, then completed and all
		/// outstanding joins of this task have also completed. Effects
		/// under any other usage conditions are not guaranteed.
		/// This method may be useful when executing
		/// pre-constructed trees of subtasks in loops.
		/// 
		/// <para>Upon completion of this method, {@code isDone()} reports
		/// {@code false}, and {@code getException()} reports {@code
		/// null}. However, the value returned by {@code getRawResult} is
		/// unaffected. To clear this value, you can invoke {@code
		/// setRawResult(null)}.
		/// </para>
		/// </summary>
		public virtual void Reinitialize()
		{
			if ((Status & DONE_MASK) == EXCEPTIONAL)
			{
				ClearExceptionalCompletion();
			}
			else
			{
				Status = 0;
			}
		}

		/// <summary>
		/// Returns the pool hosting the current task execution, or null
		/// if this task is executing outside of any ForkJoinPool.
		/// </summary>
		/// <seealso cref= #inForkJoinPool </seealso>
		/// <returns> the pool, or {@code null} if none </returns>
		public static ForkJoinPool Pool
		{
			get
			{
				Thread t = Thread.CurrentThread;
				return (t is ForkJoinWorkerThread) ? ((ForkJoinWorkerThread) t).Pool_Renamed : null;
			}
		}

		/// <summary>
		/// Returns {@code true} if the current thread is a {@link
		/// ForkJoinWorkerThread} executing as a ForkJoinPool computation.
		/// </summary>
		/// <returns> {@code true} if the current thread is a {@link
		/// ForkJoinWorkerThread} executing as a ForkJoinPool computation,
		/// or {@code false} otherwise </returns>
		public static bool InForkJoinPool()
		{
			return Thread.CurrentThread is ForkJoinWorkerThread;
		}

		/// <summary>
		/// Tries to unschedule this task for execution. This method will
		/// typically (but is not guaranteed to) succeed if this task is
		/// the most recently forked task by the current thread, and has
		/// not commenced executing in another thread.  This method may be
		/// useful when arranging alternative local processing of tasks
		/// that could have been, but were not, stolen.
		/// </summary>
		/// <returns> {@code true} if unforked </returns>
		public virtual bool TryUnfork()
		{
			Thread t;
			return (((t = Thread.CurrentThread) is ForkJoinWorkerThread) ? ((ForkJoinWorkerThread)t).WorkQueue.TryUnpush(this) : ForkJoinPool.Common.TryExternalUnpush(this));
		}

		/// <summary>
		/// Returns an estimate of the number of tasks that have been
		/// forked by the current worker thread but not yet executed. This
		/// value may be useful for heuristic decisions about whether to
		/// fork other tasks.
		/// </summary>
		/// <returns> the number of tasks </returns>
		public static int QueuedTaskCount
		{
			get
			{
				Thread t;
				ForkJoinPool.WorkQueue q;
				if ((t = Thread.CurrentThread) is ForkJoinWorkerThread)
				{
					q = ((ForkJoinWorkerThread)t).WorkQueue;
				}
				else
				{
					q = ForkJoinPool.CommonSubmitterQueue();
				}
				return (q == null) ? 0 : q.QueueSize();
			}
		}

		/// <summary>
		/// Returns an estimate of how many more locally queued tasks are
		/// held by the current worker thread than there are other worker
		/// threads that might steal them, or zero if this thread is not
		/// operating in a ForkJoinPool. This value may be useful for
		/// heuristic decisions about whether to fork other tasks. In many
		/// usages of ForkJoinTasks, at steady state, each worker should
		/// aim to maintain a small constant surplus (for example, 3) of
		/// tasks, and to process computations locally if this threshold is
		/// exceeded.
		/// </summary>
		/// <returns> the surplus number of tasks, which may be negative </returns>
		public static int SurplusQueuedTaskCount
		{
			get
			{
				return ForkJoinPool.SurplusQueuedTaskCount;
			}
		}

		// Extension methods

		/// <summary>
		/// Returns the result that would be returned by <seealso cref="#join"/>, even
		/// if this task completed abnormally, or {@code null} if this task
		/// is not known to have been completed.  This method is designed
		/// to aid debugging, as well as to support extensions. Its use in
		/// any other context is discouraged.
		/// </summary>
		/// <returns> the result, or {@code null} if not completed </returns>
		public abstract V RawResult {get;set;}


		/// <summary>
		/// Immediately performs the base action of this task and returns
		/// true if, upon return from this method, this task is guaranteed
		/// to have completed normally. This method may return false
		/// otherwise, to indicate that this task is not necessarily
		/// complete (or is not known to be complete), for example in
		/// asynchronous actions that require explicit invocations of
		/// completion methods. This method may also throw an (unchecked)
		/// exception to indicate abnormal exit. This method is designed to
		/// support extensions, and should not in general be called
		/// otherwise.
		/// </summary>
		/// <returns> {@code true} if this task is known to have completed normally </returns>
		protected internal abstract bool Exec();

		/// <summary>
		/// Returns, but does not unschedule or execute, a task queued by
		/// the current thread but not yet executed, if one is immediately
		/// available. There is no guarantee that this task will actually
		/// be polled or executed next. Conversely, this method may return
		/// null even if a task exists but cannot be accessed without
		/// contention with other threads.  This method is designed
		/// primarily to support extensions, and is unlikely to be useful
		/// otherwise.
		/// </summary>
		/// <returns> the next task, or {@code null} if none are available </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected static ForkJoinTask<?> peekNextLocalTask()
		protected internal static ForkJoinTask<?> PeekNextLocalTask()
		{
			Thread t;
			ForkJoinPool.WorkQueue q;
			if ((t = Thread.CurrentThread) is ForkJoinWorkerThread)
			{
				q = ((ForkJoinWorkerThread)t).WorkQueue;
			}
			else
			{
				q = ForkJoinPool.CommonSubmitterQueue();
			}
			return (q == null) ? null : q.Peek();
		}

		/// <summary>
		/// Unschedules and returns, without executing, the next task
		/// queued by the current thread but not yet executed, if the
		/// current thread is operating in a ForkJoinPool.  This method is
		/// designed primarily to support extensions, and is unlikely to be
		/// useful otherwise.
		/// </summary>
		/// <returns> the next task, or {@code null} if none are available </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected static ForkJoinTask<?> pollNextLocalTask()
		protected internal static ForkJoinTask<?> PollNextLocalTask()
		{
			Thread t;
			return ((t = Thread.CurrentThread) is ForkJoinWorkerThread) ? ((ForkJoinWorkerThread)t).WorkQueue.NextLocalTask() : null;
		}

		/// <summary>
		/// If the current thread is operating in a ForkJoinPool,
		/// unschedules and returns, without executing, the next task
		/// queued by the current thread but not yet executed, if one is
		/// available, or if not available, a task that was forked by some
		/// other thread, if available. Availability may be transient, so a
		/// {@code null} result does not necessarily imply quiescence of
		/// the pool this task is operating in.  This method is designed
		/// primarily to support extensions, and is unlikely to be useful
		/// otherwise.
		/// </summary>
		/// <returns> a task, or {@code null} if none are available </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected static ForkJoinTask<?> pollTask()
		protected internal static ForkJoinTask<?> PollTask()
		{
			Thread t;
			ForkJoinWorkerThread wt;
			return ((t = Thread.CurrentThread) is ForkJoinWorkerThread) ? (wt = (ForkJoinWorkerThread)t).pool.nextTaskFor(wt.WorkQueue) : null;
		}

		// tag operations

		/// <summary>
		/// Returns the tag for this task.
		/// </summary>
		/// <returns> the tag for this task
		/// @since 1.8 </returns>
		public short ForkJoinTaskTag
		{
			get
			{
				return (short)Status;
			}
		}

		/// <summary>
		/// Atomically sets the tag value for this task.
		/// </summary>
		/// <param name="tag"> the tag value </param>
		/// <returns> the previous value of the tag
		/// @since 1.8 </returns>
		public short SetForkJoinTaskTag(short tag)
		{
			for (int s;;)
			{
				if (U.compareAndSwapInt(this, STATUS, s = Status, (s & ~SMASK) | (tag & SMASK)))
				{
					return (short)s;
				}
			}
		}

		/// <summary>
		/// Atomically conditionally sets the tag value for this task.
		/// Among other applications, tags can be used as visit markers
		/// in tasks operating on graphs, as in methods that check: {@code
		/// if (task.compareAndSetForkJoinTaskTag((short)0, (short)1))}
		/// before processing, otherwise exiting because the node has
		/// already been visited.
		/// </summary>
		/// <param name="e"> the expected tag value </param>
		/// <param name="tag"> the new tag value </param>
		/// <returns> {@code true} if successful; i.e., the current value was
		/// equal to e and is now tag.
		/// @since 1.8 </returns>
		public bool CompareAndSetForkJoinTaskTag(short e, short tag)
		{
			for (int s;;)
			{
				if ((short)(s = Status) != e)
				{
					return false;
				}
				if (U.compareAndSwapInt(this, STATUS, s, (s & ~SMASK) | (tag & SMASK)))
				{
					return true;
				}
			}
		}

		/// <summary>
		/// Adaptor for Runnables. This implements RunnableFuture
		/// to be compliant with AbstractExecutorService constraints
		/// when used in ForkJoinPool.
		/// </summary>
		internal sealed class AdaptedRunnable<T> : ForkJoinTask<T>, RunnableFuture<T>
		{
			internal readonly Runnable Runnable;
			internal T Result;
			internal AdaptedRunnable(Runnable runnable, T result)
			{
				if (runnable == null)
				{
					throw new NullPointerException();
				}
				this.Runnable = runnable;
				this.Result = result; // OK to set this even before completion
			}
			public T RawResult
			{
				get
				{
					return Result;
				}
				set
				{
					Result = value;
				}
			}
			public bool Exec()
			{
				Runnable.Run();
				return true;
			}
			public void Run()
			{
				Invoke();
			}
			internal const long SerialVersionUID = 5232453952276885070L;
		}

		/// <summary>
		/// Adaptor for Runnables without results
		/// </summary>
		internal sealed class AdaptedRunnableAction : ForkJoinTask<Void>, RunnableFuture<Void>
		{
			internal readonly Runnable Runnable;
			internal AdaptedRunnableAction(Runnable runnable)
			{
				if (runnable == null)
				{
					throw new NullPointerException();
				}
				this.Runnable = runnable;
			}
			public Void RawResult
			{
				get
				{
					return null;
				}
				set
				{
				}
			}
			public bool Exec()
			{
				Runnable.Run();
				return true;
			}
			public void Run()
			{
				Invoke();
			}
			internal const long SerialVersionUID = 5232453952276885070L;
		}

		/// <summary>
		/// Adaptor for Runnables in which failure forces worker exception
		/// </summary>
		internal sealed class RunnableExecuteAction : ForkJoinTask<Void>
		{
			internal readonly Runnable Runnable;
			internal RunnableExecuteAction(Runnable runnable)
			{
				if (runnable == null)
				{
					throw new NullPointerException();
				}
				this.Runnable = runnable;
			}
			public Void RawResult
			{
				get
				{
					return null;
				}
				set
				{
				}
			}
			public bool Exec()
			{
				Runnable.Run();
				return true;
			}
			internal void InternalPropagateException(Throwable ex)
			{
				Rethrow(ex); // rethrow outside exec() catches.
			}
			internal const long SerialVersionUID = 5232453952276885070L;
		}

		/// <summary>
		/// Adaptor for Callables
		/// </summary>
		internal sealed class AdaptedCallable<T> : ForkJoinTask<T>, RunnableFuture<T>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final java.util.concurrent.Callable<? extends T> callable;
			internal readonly Callable<?> Callable;
			internal T Result;
			internal AdaptedCallable<T1>(Callable<T1> callable) where T1 : T
			{
				if (callable == null)
				{
					throw new NullPointerException();
				}
				this.Callable = callable;
			}
			public T RawResult
			{
				get
				{
					return Result;
				}
				set
				{
					Result = value;
				}
			}
			public bool Exec()
			{
				try
				{
					Result = Callable.Call();
					return true;
				}
				catch (Error err)
				{
					throw err;
				}
				catch (RuntimeException rex)
				{
					throw rex;
				}
				catch (Exception ex)
				{
					throw new RuntimeException(ex);
				}
			}
			public void Run()
			{
				Invoke();
			}
			internal const long SerialVersionUID = 2838392045355241008L;
		}

		/// <summary>
		/// Returns a new {@code ForkJoinTask} that performs the {@code run}
		/// method of the given {@code Runnable} as its action, and returns
		/// a null result upon <seealso cref="#join"/>.
		/// </summary>
		/// <param name="runnable"> the runnable action </param>
		/// <returns> the task </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static ForkJoinTask<?> adapt(Runnable runnable)
		public static ForkJoinTask<?> Adapt(Runnable runnable)
		{
			return new AdaptedRunnableAction(runnable);
		}

		/// <summary>
		/// Returns a new {@code ForkJoinTask} that performs the {@code run}
		/// method of the given {@code Runnable} as its action, and returns
		/// the given result upon <seealso cref="#join"/>.
		/// </summary>
		/// <param name="runnable"> the runnable action </param>
		/// <param name="result"> the result upon completion </param>
		/// @param <T> the type of the result </param>
		/// <returns> the task </returns>
		public static ForkJoinTask<T> adapt<T>(Runnable runnable, T result)
		{
			return new AdaptedRunnable<T>(runnable, result);
		}

		/// <summary>
		/// Returns a new {@code ForkJoinTask} that performs the {@code call}
		/// method of the given {@code Callable} as its action, and returns
		/// its result upon <seealso cref="#join"/>, translating any checked exceptions
		/// encountered into {@code RuntimeException}.
		/// </summary>
		/// <param name="callable"> the callable action </param>
		/// @param <T> the type of the callable's result </param>
		/// <returns> the task </returns>
		public static ForkJoinTask<T> adapt<T, T1>(Callable<T1> callable) where T1 : T
		{
			return new AdaptedCallable<T>(callable);
		}

		// Serialization support

		private const long SerialVersionUID = -7721805057305804111L;

		/// <summary>
		/// Saves this task to a stream (that is, serializes it).
		/// </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs
		/// @serialData the current run status and the exception thrown
		/// during execution, or {@code null} if none </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			s.DefaultWriteObject();
			s.WriteObject(Exception);
		}

		/// <summary>
		/// Reconstitutes this task from a stream (that is, deserializes it). </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized object
		///         could not be found </exception>
		/// <exception cref="java.io.IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject();
			Object ex = s.ReadObject();
			if (ex != null)
			{
				ExceptionalCompletion = (Throwable)ex;
			}
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe U;
		private static readonly long STATUS;

		static ForkJoinTask()
		{
			ExceptionTableLock = new ReentrantLock();
			ExceptionTableRefQueue = new ReferenceQueue<Object>();
			ExceptionTable = new ExceptionNode[EXCEPTION_MAP_CAPACITY];
			try
			{
				U = sun.misc.Unsafe.Unsafe;
				Class k = typeof(ForkJoinTask);
				STATUS = U.objectFieldOffset(k.GetDeclaredField("status"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}

	}

}