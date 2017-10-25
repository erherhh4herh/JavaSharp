using System;
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
	/// A thread managed by a <seealso cref="ForkJoinPool"/>, which executes
	/// <seealso cref="ForkJoinTask"/>s.
	/// This class is subclassable solely for the sake of adding
	/// functionality -- there are no overridable methods dealing with
	/// scheduling or execution.  However, you can override initialization
	/// and termination methods surrounding the main task processing loop.
	/// If you do create such a subclass, you will also need to supply a
	/// custom <seealso cref="ForkJoinPool.ForkJoinWorkerThreadFactory"/> to
	/// <seealso cref="ForkJoinPool#ForkJoinPool use it"/> in a {@code ForkJoinPool}.
	/// 
	/// @since 1.7
	/// @author Doug Lea
	/// </summary>
	public class ForkJoinWorkerThread : Thread
	{
		/*
		 * ForkJoinWorkerThreads are managed by ForkJoinPools and perform
		 * ForkJoinTasks. For explanation, see the internal documentation
		 * of class ForkJoinPool.
		 *
		 * This class just maintains links to its pool and WorkQueue.  The
		 * pool field is set immediately upon construction, but the
		 * workQueue field is not set until a call to registerWorker
		 * completes. This leads to a visibility race, that is tolerated
		 * by requiring that the workQueue field is only accessed by the
		 * owning thread.
		 *
		 * Support for (non-public) subclass InnocuousForkJoinWorkerThread
		 * requires that we break quite a lot of encapsulation (via Unsafe)
		 * both here and in the subclass to access and set Thread fields.
		 */

		internal readonly ForkJoinPool Pool_Renamed; // the pool this thread works in
		internal readonly ForkJoinPool.WorkQueue WorkQueue; // work-stealing mechanics

		/// <summary>
		/// Creates a ForkJoinWorkerThread operating in the given pool.
		/// </summary>
		/// <param name="pool"> the pool this thread works in </param>
		/// <exception cref="NullPointerException"> if pool is null </exception>
		protected internal ForkJoinWorkerThread(ForkJoinPool pool) : base("aForkJoinWorkerThread")
		{
			// Use a placeholder until a useful name can be set in registerWorker
			this.Pool_Renamed = pool;
			this.WorkQueue = pool.RegisterWorker(this);
		}

		/// <summary>
		/// Version for InnocuousForkJoinWorkerThread
		/// </summary>
		internal ForkJoinWorkerThread(ForkJoinPool pool, ThreadGroup threadGroup, AccessControlContext acc) : base(threadGroup, null, "aForkJoinWorkerThread")
		{
			U.putOrderedObject(this, INHERITEDACCESSCONTROLCONTEXT, acc);
			EraseThreadLocals(); // clear before registering
			this.Pool_Renamed = pool;
			this.WorkQueue = pool.RegisterWorker(this);
		}

		/// <summary>
		/// Returns the pool hosting this thread.
		/// </summary>
		/// <returns> the pool </returns>
		public virtual ForkJoinPool Pool
		{
			get
			{
				return Pool_Renamed;
			}
		}

		/// <summary>
		/// Returns the unique index number of this thread in its pool.
		/// The returned value ranges from zero to the maximum number of
		/// threads (minus one) that may exist in the pool, and does not
		/// change during the lifetime of the thread.  This method may be
		/// useful for applications that track status or collect results
		/// per-worker-thread rather than per-task.
		/// </summary>
		/// <returns> the index number </returns>
		public virtual int PoolIndex
		{
			get
			{
				return WorkQueue.PoolIndex;
			}
		}

		/// <summary>
		/// Initializes internal state after construction but before
		/// processing any tasks. If you override this method, you must
		/// invoke {@code super.onStart()} at the beginning of the method.
		/// Initialization requires care: Most fields must have legal
		/// default values, to ensure that attempted accesses from other
		/// threads work correctly even before this thread starts
		/// processing tasks.
		/// </summary>
		protected internal virtual void OnStart()
		{
		}

		/// <summary>
		/// Performs cleanup associated with termination of this worker
		/// thread.  If you override this method, you must invoke
		/// {@code super.onTermination} at the end of the overridden method.
		/// </summary>
		/// <param name="exception"> the exception causing this thread to abort due
		/// to an unrecoverable error, or {@code null} if completed normally </param>
		protected internal virtual void OnTermination(Throwable exception)
		{
		}

		/// <summary>
		/// This method is required to be public, but should never be
		/// called explicitly. It performs the main run loop to execute
		/// <seealso cref="ForkJoinTask"/>s.
		/// </summary>
		public override void Run()
		{
			if (WorkQueue.Array == null) // only run once
			{
				Throwable exception = null;
				try
				{
					OnStart();
					Pool_Renamed.RunWorker(WorkQueue);
				}
				catch (Throwable ex)
				{
					exception = ex;
				}
				finally
				{
					try
					{
						OnTermination(exception);
					}
					catch (Throwable ex)
					{
						if (exception == null)
						{
							exception = ex;
						}
					}
					finally
					{
						Pool_Renamed.DeregisterWorker(this, exception);
					}
				}
			}
		}

		/// <summary>
		/// Erases ThreadLocals by nulling out Thread maps.
		/// </summary>
		internal void EraseThreadLocals()
		{
			U.putObject(this, THREADLOCALS, null);
			U.putObject(this, INHERITABLETHREADLOCALS, null);
		}

		/// <summary>
		/// Non-public hook method for InnocuousForkJoinWorkerThread
		/// </summary>
		internal virtual void AfterTopLevelExec()
		{
		}

		// Set up to allow setting thread fields in constructor
		private static readonly sun.misc.Unsafe U;
		private static readonly long THREADLOCALS;
		private static readonly long INHERITABLETHREADLOCALS;
		private static readonly long INHERITEDACCESSCONTROLCONTEXT;
		static ForkJoinWorkerThread()
		{
			try
			{
				U = sun.misc.Unsafe.Unsafe;
				Class tk = typeof(Thread);
				THREADLOCALS = U.objectFieldOffset(tk.GetDeclaredField("threadLocals"));
				INHERITABLETHREADLOCALS = U.objectFieldOffset(tk.GetDeclaredField("inheritableThreadLocals"));
				INHERITEDACCESSCONTROLCONTEXT = U.objectFieldOffset(tk.GetDeclaredField("inheritedAccessControlContext"));

			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}

		/// <summary>
		/// A worker thread that has no permissions, is not a member of any
		/// user-defined ThreadGroup, and erases all ThreadLocals after
		/// running each top-level task.
		/// </summary>
		internal sealed class InnocuousForkJoinWorkerThread : ForkJoinWorkerThread
		{
			/// <summary>
			/// The ThreadGroup for all InnocuousForkJoinWorkerThreads </summary>
			internal static readonly ThreadGroup InnocuousThreadGroup = CreateThreadGroup();

			/// <summary>
			/// An AccessControlContext supporting no privileges </summary>
			private static final AccessControlContext INNOCUOUS_ACC = new AccessControlContext(new ProtectionDomain[] { new ProtectionDomain(null, null)
		});

			outerInstance.InnocuousForkJoinWorkerThread(ForkJoinPool Pool_Renamed)
			{
				base(Pool_Renamed, InnocuousThreadGroup, INNOCUOUS_ACC);
			}

			void afterTopLevelExec() // to erase ThreadLocals
			{
				EraseThreadLocals();
			}

			public ClassLoader ContextClassLoader // to always report system loader
			{
				return ClassLoader.SystemClassLoader;
			}

			public void setUncaughtExceptionHandler(UncaughtExceptionHandler x) // paranoically
			{
			}
			public void setContextClassLoader(ClassLoader cl)
			{
				throw new SecurityException("setContextClassLoader");
			}

			/// <summary>
			/// Returns a new group with the system ThreadGroup (the
			/// topmost, parent-less group) as parent.  Uses Unsafe to
			/// traverse Thread.group and ThreadGroup.parent fields.
			/// </summary>
			private static ThreadGroup CreateThreadGroup()
			{
				try
				{
					sun.misc.Unsafe u = sun.misc.Unsafe.Unsafe;
					Class tk = typeof(Thread);
					Class gk = typeof(ThreadGroup);
					long tg = u.objectFieldOffset(tk.GetDeclaredField("group"));
					long gp = u.objectFieldOffset(gk.GetDeclaredField("parent"));
					ThreadGroup group = (ThreadGroup) u.getObject(Thread.CurrentThread, tg);
					while (group != null)
					{
						ThreadGroup parent = (ThreadGroup)u.getObject(group, gp);
						if (parent == null)
						{
							return new ThreadGroup(group, "InnocuousForkJoinWorkerThreadGroup");
						}
						group = parent;
					}
				}
				catch (Exception e)
				{
					throw new Error(e);
				}
				// fall through if null as cannot-happen safeguard
				throw new Error("Cannot create ThreadGroup");
			}
	}

}

}