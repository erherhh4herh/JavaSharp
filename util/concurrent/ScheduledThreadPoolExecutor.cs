using System;
using System.Collections;
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
	/// A <seealso cref="ThreadPoolExecutor"/> that can additionally schedule
	/// commands to run after a given delay, or to execute
	/// periodically. This class is preferable to <seealso cref="java.util.Timer"/>
	/// when multiple worker threads are needed, or when the additional
	/// flexibility or capabilities of <seealso cref="ThreadPoolExecutor"/> (which
	/// this class extends) are required.
	/// 
	/// <para>Delayed tasks execute no sooner than they are enabled, but
	/// without any real-time guarantees about when, after they are
	/// enabled, they will commence. Tasks scheduled for exactly the same
	/// execution time are enabled in first-in-first-out (FIFO) order of
	/// submission.
	/// 
	/// </para>
	/// <para>When a submitted task is cancelled before it is run, execution
	/// is suppressed. By default, such a cancelled task is not
	/// automatically removed from the work queue until its delay
	/// elapses. While this enables further inspection and monitoring, it
	/// may also cause unbounded retention of cancelled tasks. To avoid
	/// this, set <seealso cref="#setRemoveOnCancelPolicy"/> to {@code true}, which
	/// causes tasks to be immediately removed from the work queue at
	/// time of cancellation.
	/// 
	/// </para>
	/// <para>Successive executions of a task scheduled via
	/// {@code scheduleAtFixedRate} or
	/// {@code scheduleWithFixedDelay} do not overlap. While different
	/// executions may be performed by different threads, the effects of
	/// prior executions <a
	/// href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// those of subsequent ones.
	/// 
	/// </para>
	/// <para>While this class inherits from <seealso cref="ThreadPoolExecutor"/>, a few
	/// of the inherited tuning methods are not useful for it. In
	/// particular, because it acts as a fixed-sized pool using
	/// {@code corePoolSize} threads and an unbounded queue, adjustments
	/// to {@code maximumPoolSize} have no useful effect. Additionally, it
	/// is almost never a good idea to set {@code corePoolSize} to zero or
	/// use {@code allowCoreThreadTimeOut} because this may leave the pool
	/// without threads to handle tasks once they become eligible to run.
	/// 
	/// </para>
	/// <para><b>Extension notes:</b> This class overrides the
	/// <seealso cref="ThreadPoolExecutor#execute(Runnable) execute"/> and
	/// <seealso cref="AbstractExecutorService#submit(Runnable) submit"/>
	/// methods to generate internal <seealso cref="ScheduledFuture"/> objects to
	/// control per-task delays and scheduling.  To preserve
	/// functionality, any further overrides of these methods in
	/// subclasses must invoke superclass versions, which effectively
	/// disables additional task customization.  However, this class
	/// provides alternative protected extension method
	/// {@code decorateTask} (one version each for {@code Runnable} and
	/// {@code Callable}) that can be used to customize the concrete task
	/// types used to execute commands entered via {@code execute},
	/// {@code submit}, {@code schedule}, {@code scheduleAtFixedRate},
	/// and {@code scheduleWithFixedDelay}.  By default, a
	/// {@code ScheduledThreadPoolExecutor} uses a task type extending
	/// <seealso cref="FutureTask"/>. However, this may be modified or replaced using
	/// subclasses of the form:
	/// 
	///  <pre> {@code
	/// public class CustomScheduledExecutor extends ScheduledThreadPoolExecutor {
	/// 
	///   static class CustomTask<V> implements RunnableScheduledFuture<V> { ... }
	/// 
	///   protected <V> RunnableScheduledFuture<V> decorateTask(
	///                Runnable r, RunnableScheduledFuture<V> task) {
	///       return new CustomTask<V>(r, task);
	///   }
	/// 
	///   protected <V> RunnableScheduledFuture<V> decorateTask(
	///                Callable<V> c, RunnableScheduledFuture<V> task) {
	///       return new CustomTask<V>(c, task);
	///   }
	///   // ... add constructors, etc.
	/// }}</pre>
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public class ScheduledThreadPoolExecutor : ThreadPoolExecutor, ScheduledExecutorService
	{

		/*
		 * This class specializes ThreadPoolExecutor implementation by
		 *
		 * 1. Using a custom task type, ScheduledFutureTask for
		 *    tasks, even those that don't require scheduling (i.e.,
		 *    those submitted using ExecutorService execute, not
		 *    ScheduledExecutorService methods) which are treated as
		 *    delayed tasks with a delay of zero.
		 *
		 * 2. Using a custom queue (DelayedWorkQueue), a variant of
		 *    unbounded DelayQueue. The lack of capacity constraint and
		 *    the fact that corePoolSize and maximumPoolSize are
		 *    effectively identical simplifies some execution mechanics
		 *    (see delayedExecute) compared to ThreadPoolExecutor.
		 *
		 * 3. Supporting optional run-after-shutdown parameters, which
		 *    leads to overrides of shutdown methods to remove and cancel
		 *    tasks that should NOT be run after shutdown, as well as
		 *    different recheck logic when task (re)submission overlaps
		 *    with a shutdown.
		 *
		 * 4. Task decoration methods to allow interception and
		 *    instrumentation, which are needed because subclasses cannot
		 *    otherwise override submit methods to get this effect. These
		 *    don't have any impact on pool control logic though.
		 */

		/// <summary>
		/// False if should cancel/suppress periodic tasks on shutdown.
		/// </summary>
		private volatile bool ContinueExistingPeriodicTasksAfterShutdown;

		/// <summary>
		/// False if should cancel non-periodic tasks on shutdown.
		/// </summary>
		private volatile bool ExecuteExistingDelayedTasksAfterShutdown = true;

		/// <summary>
		/// True if ScheduledFutureTask.cancel should remove from queue
		/// </summary>
		private volatile bool RemoveOnCancel = false;

		/// <summary>
		/// Sequence number to break scheduling ties, and in turn to
		/// guarantee FIFO order among tied entries.
		/// </summary>
		private static readonly AtomicLong Sequencer = new AtomicLong();

		/// <summary>
		/// Returns current nanosecond time.
		/// </summary>
		internal long Now()
		{
			return System.nanoTime();
		}

		private class ScheduledFutureTask<V> : FutureTask<V>, RunnableScheduledFuture<V>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				OuterTask = this;
			}

			private readonly ScheduledThreadPoolExecutor OuterInstance;


			/// <summary>
			/// Sequence number to break ties FIFO </summary>
			internal readonly long SequenceNumber;

			/// <summary>
			/// The time the task is enabled to execute in nanoTime units </summary>
			internal long Time;

			/// <summary>
			/// Period in nanoseconds for repeating tasks.  A positive
			/// value indicates fixed-rate execution.  A negative value
			/// indicates fixed-delay execution.  A value of 0 indicates a
			/// non-repeating task.
			/// </summary>
			internal readonly long Period;

			/// <summary>
			/// The actual task to be re-enqueued by reExecutePeriodic </summary>
			internal RunnableScheduledFuture<V> OuterTask;

			/// <summary>
			/// Index into delay queue, to support faster cancellation.
			/// </summary>
			internal int HeapIndex;

			/// <summary>
			/// Creates a one-shot action with given nanoTime-based trigger time.
			/// </summary>
			internal ScheduledFutureTask(ScheduledThreadPoolExecutor outerInstance, Runnable r, V result, long ns) : base(r, result)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
				this.Time = ns;
				this.Period = 0;
				this.SequenceNumber = Sequencer.AndIncrement;
			}

			/// <summary>
			/// Creates a periodic action with given nano time and period.
			/// </summary>
			internal ScheduledFutureTask(ScheduledThreadPoolExecutor outerInstance, Runnable r, V result, long ns, long period) : base(r, result)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
				this.Time = ns;
				this.Period = period;
				this.SequenceNumber = Sequencer.AndIncrement;
			}

			/// <summary>
			/// Creates a one-shot action with given nanoTime-based trigger time.
			/// </summary>
			internal ScheduledFutureTask(ScheduledThreadPoolExecutor outerInstance, Callable<V> callable, long ns) : base(callable)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
				this.Time = ns;
				this.Period = 0;
				this.SequenceNumber = Sequencer.AndIncrement;
			}

			public virtual long GetDelay(TimeUnit unit)
			{
				return unit.Convert(Time - outerInstance.Now(), NANOSECONDS);
			}

			public virtual int CompareTo(Delayed other)
			{
				if (other == this) // compare zero if same object
				{
					return 0;
				}
				if (other is ScheduledFutureTask)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ScheduledFutureTask<?> x = (ScheduledFutureTask<?>)other;
					ScheduledFutureTask<?> x = (ScheduledFutureTask<?>)other;
					long diff = Time - x.Time;
					if (diff < 0)
					{
						return -1;
					}
					else if (diff > 0)
					{
						return 1;
					}
					else if (SequenceNumber < x.SequenceNumber)
					{
						return -1;
					}
					else
					{
						return 1;
					}
				}
				long diff = GetDelay(NANOSECONDS) - other.GetDelay(NANOSECONDS);
				return (diff < 0) ? - 1 : (diff > 0) ? 1 : 0;
			}

			/// <summary>
			/// Returns {@code true} if this is a periodic (not a one-shot) action.
			/// </summary>
			/// <returns> {@code true} if periodic </returns>
			public virtual bool Periodic
			{
				get
				{
					return Period != 0;
				}
			}

			/// <summary>
			/// Sets the next time to run for a periodic task.
			/// </summary>
			internal virtual void SetNextRunTime()
			{
				long p = Period;
				if (p > 0)
				{
					Time += p;
				}
				else
				{
					Time = outerInstance.TriggerTime(-p);
				}
			}

			public virtual bool Cancel(bool mayInterruptIfRunning)
			{
				bool cancelled = base.Cancel(mayInterruptIfRunning);
				if (cancelled && outerInstance.RemoveOnCancel && HeapIndex >= 0)
				{
					outerInstance.Remove(this);
				}
				return cancelled;
			}

			/// <summary>
			/// Overrides FutureTask version so as to reset/requeue if periodic.
			/// </summary>
			public virtual void Run()
			{
				bool periodic = Periodic;
				if (!outerInstance.CanRunInCurrentRunState(periodic))
				{
					Cancel(false);
				}
				else if (!periodic)
				{
					ScheduledFutureTask.this.Run();
				}
				else if (ScheduledFutureTask.this.RunAndReset())
				{
					SetNextRunTime();
					outerInstance.ReExecutePeriodic(OuterTask);
				}
			}
		}

		/// <summary>
		/// Returns true if can run a task given current run state
		/// and run-after-shutdown parameters.
		/// </summary>
		/// <param name="periodic"> true if this task periodic, false if delayed </param>
		internal virtual bool CanRunInCurrentRunState(bool periodic)
		{
			return IsRunningOrShutdown(periodic ? ContinueExistingPeriodicTasksAfterShutdown : ExecuteExistingDelayedTasksAfterShutdown);
		}

		/// <summary>
		/// Main execution method for delayed or periodic tasks.  If pool
		/// is shut down, rejects the task. Otherwise adds task to queue
		/// and starts a thread, if necessary, to run it.  (We cannot
		/// prestart the thread to run the task because the task (probably)
		/// shouldn't be run yet.)  If the pool is shut down while the task
		/// is being added, cancel and remove it if required by state and
		/// run-after-shutdown parameters.
		/// </summary>
		/// <param name="task"> the task </param>
		private void delayedExecute<T1>(RunnableScheduledFuture<T1> task)
		{
			if (Shutdown)
			{
				Reject(task);
			}
			else
			{
				base.Queue.Add(task);
				if (Shutdown && !CanRunInCurrentRunState(task.Periodic) && Remove(task))
				{
					task.Cancel(false);
				}
				else
				{
					EnsurePrestart();
				}
			}
		}

		/// <summary>
		/// Requeues a periodic task unless current run state precludes it.
		/// Same idea as delayedExecute except drops task rather than rejecting.
		/// </summary>
		/// <param name="task"> the task </param>
		internal virtual void reExecutePeriodic<T1>(RunnableScheduledFuture<T1> task)
		{
			if (CanRunInCurrentRunState(true))
			{
				base.Queue.Add(task);
				if (!CanRunInCurrentRunState(true) && Remove(task))
				{
					task.Cancel(false);
				}
				else
				{
					EnsurePrestart();
				}
			}
		}

		/// <summary>
		/// Cancels and clears the queue of all tasks that should not be run
		/// due to shutdown policy.  Invoked within super.shutdown.
		/// </summary>
		internal override void OnShutdown()
		{
			BlockingQueue<Runnable> q = base.Queue;
			bool keepDelayed = ExecuteExistingDelayedTasksAfterShutdownPolicy;
			bool keepPeriodic = ContinueExistingPeriodicTasksAfterShutdownPolicy;
			if (!keepDelayed && !keepPeriodic)
			{
				foreach (Object e in q.ToArray())
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (e instanceof RunnableScheduledFuture<?>)
					if (e is RunnableScheduledFuture<?>)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ((RunnableScheduledFuture<?>) e).cancel(false);
						((RunnableScheduledFuture<?>) e).Cancel(false);
					}
				}
				q.Clear();
			}
			else
			{
				// Traverse snapshot to avoid iterator exceptions
				foreach (Object e in q.ToArray())
				{
					if (e is RunnableScheduledFuture)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> t = (RunnableScheduledFuture<?>)e;
						RunnableScheduledFuture<?> t = (RunnableScheduledFuture<?>)e;
						if ((t.Periodic ?!keepPeriodic :!keepDelayed) || t.Cancelled) // also remove if already cancelled
						{
							if (q.Remove(t))
							{
								t.Cancel(false);
							}
						}
					}
				}
			}
			TryTerminate();
		}

		/// <summary>
		/// Modifies or replaces the task used to execute a runnable.
		/// This method can be used to override the concrete
		/// class used for managing internal tasks.
		/// The default implementation simply returns the given task.
		/// </summary>
		/// <param name="runnable"> the submitted Runnable </param>
		/// <param name="task"> the task created to execute the runnable </param>
		/// @param <V> the type of the task's result </param>
		/// <returns> a task that can execute the runnable
		/// @since 1.6 </returns>
		protected internal virtual RunnableScheduledFuture<V> decorateTask<V>(Runnable runnable, RunnableScheduledFuture<V> task)
		{
			return task;
		}

		/// <summary>
		/// Modifies or replaces the task used to execute a callable.
		/// This method can be used to override the concrete
		/// class used for managing internal tasks.
		/// The default implementation simply returns the given task.
		/// </summary>
		/// <param name="callable"> the submitted Callable </param>
		/// <param name="task"> the task created to execute the callable </param>
		/// @param <V> the type of the task's result </param>
		/// <returns> a task that can execute the callable
		/// @since 1.6 </returns>
		protected internal virtual RunnableScheduledFuture<V> decorateTask<V>(Callable<V> callable, RunnableScheduledFuture<V> task)
		{
			return task;
		}

		/// <summary>
		/// Creates a new {@code ScheduledThreadPoolExecutor} with the
		/// given core pool size.
		/// </summary>
		/// <param name="corePoolSize"> the number of threads to keep in the pool, even
		///        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		/// <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		public ScheduledThreadPoolExecutor(int corePoolSize) : base(corePoolSize, Integer.MaxValue, 0, NANOSECONDS, new DelayedWorkQueue())
		{
		}

		/// <summary>
		/// Creates a new {@code ScheduledThreadPoolExecutor} with the
		/// given initial parameters.
		/// </summary>
		/// <param name="corePoolSize"> the number of threads to keep in the pool, even
		///        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		/// <param name="threadFactory"> the factory to use when the executor
		///        creates a new thread </param>
		/// <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		/// <exception cref="NullPointerException"> if {@code threadFactory} is null </exception>
		public ScheduledThreadPoolExecutor(int corePoolSize, ThreadFactory threadFactory) : base(corePoolSize, Integer.MaxValue, 0, NANOSECONDS, new DelayedWorkQueue(), threadFactory)
		{
		}

		/// <summary>
		/// Creates a new ScheduledThreadPoolExecutor with the given
		/// initial parameters.
		/// </summary>
		/// <param name="corePoolSize"> the number of threads to keep in the pool, even
		///        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		/// <param name="handler"> the handler to use when execution is blocked
		///        because the thread bounds and queue capacities are reached </param>
		/// <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		/// <exception cref="NullPointerException"> if {@code handler} is null </exception>
		public ScheduledThreadPoolExecutor(int corePoolSize, RejectedExecutionHandler handler) : base(corePoolSize, Integer.MaxValue, 0, NANOSECONDS, new DelayedWorkQueue(), handler)
		{
		}

		/// <summary>
		/// Creates a new ScheduledThreadPoolExecutor with the given
		/// initial parameters.
		/// </summary>
		/// <param name="corePoolSize"> the number of threads to keep in the pool, even
		///        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		/// <param name="threadFactory"> the factory to use when the executor
		///        creates a new thread </param>
		/// <param name="handler"> the handler to use when execution is blocked
		///        because the thread bounds and queue capacities are reached </param>
		/// <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		/// <exception cref="NullPointerException"> if {@code threadFactory} or
		///         {@code handler} is null </exception>
		public ScheduledThreadPoolExecutor(int corePoolSize, ThreadFactory threadFactory, RejectedExecutionHandler handler) : base(corePoolSize, Integer.MaxValue, 0, NANOSECONDS, new DelayedWorkQueue(), threadFactory, handler)
		{
		}

		/// <summary>
		/// Returns the trigger time of a delayed action.
		/// </summary>
		private long TriggerTime(long delay, TimeUnit unit)
		{
			return TriggerTime(unit.ToNanos((delay < 0) ? 0 : delay));
		}

		/// <summary>
		/// Returns the trigger time of a delayed action.
		/// </summary>
		internal virtual long TriggerTime(long delay)
		{
			return Now() + ((delay < (Long.MaxValue >> 1)) ? delay : OverflowFree(delay));
		}

		/// <summary>
		/// Constrains the values of all delays in the queue to be within
		/// Long.MAX_VALUE of each other, to avoid overflow in compareTo.
		/// This may occur if a task is eligible to be dequeued, but has
		/// not yet been, while some other task is added with a delay of
		/// Long.MAX_VALUE.
		/// </summary>
		private long OverflowFree(long delay)
		{
			Delayed head = (Delayed) base.Queue.Peek();
			if (head != null)
			{
				long headDelay = head.GetDelay(NANOSECONDS);
				if (headDelay < 0 && (delay - headDelay < 0))
				{
					delay = Long.MaxValue + headDelay;
				}
			}
			return delay;
		}

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ScheduledFuture<?> schedule(Runnable command, long delay, TimeUnit unit)
		public virtual ScheduledFuture<?> Schedule(Runnable command, long delay, TimeUnit unit)
		{
			if (command == null || unit == null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> t = decorateTask(command, new ScheduledFutureTask<Void>(command, null, triggerTime(delay, unit)));
			RunnableScheduledFuture<?> t = DecorateTask(command, new ScheduledFutureTask<Void>(command, null, TriggerTime(delay, unit)));
			DelayedExecute(t);
			return t;
		}

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
		public virtual ScheduledFuture<V> schedule<V>(Callable<V> callable, long delay, TimeUnit unit)
		{
			if (callable == null || unit == null)
			{
				throw new NullPointerException();
			}
			RunnableScheduledFuture<V> t = DecorateTask(callable, new ScheduledFutureTask<V>(callable, TriggerTime(delay, unit)));
			DelayedExecute(t);
			return t;
		}

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">   {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ScheduledFuture<?> scheduleAtFixedRate(Runnable command, long initialDelay, long period, TimeUnit unit)
		public virtual ScheduledFuture<?> ScheduleAtFixedRate(Runnable command, long initialDelay, long period, TimeUnit unit)
		{
			if (command == null || unit == null)
			{
				throw new NullPointerException();
			}
			if (period <= 0)
			{
				throw new IllegalArgumentException();
			}
			ScheduledFutureTask<Void> sft = new ScheduledFutureTask<Void>(command, null, TriggerTime(initialDelay, unit), unit.ToNanos(period));
			RunnableScheduledFuture<Void> t = DecorateTask(command, sft);
			sft.OuterTask = t;
			DelayedExecute(t);
			return t;
		}

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">   {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ScheduledFuture<?> scheduleWithFixedDelay(Runnable command, long initialDelay, long delay, TimeUnit unit)
		public virtual ScheduledFuture<?> ScheduleWithFixedDelay(Runnable command, long initialDelay, long delay, TimeUnit unit)
		{
			if (command == null || unit == null)
			{
				throw new NullPointerException();
			}
			if (delay <= 0)
			{
				throw new IllegalArgumentException();
			}
			ScheduledFutureTask<Void> sft = new ScheduledFutureTask<Void>(command, null, TriggerTime(initialDelay, unit), unit.ToNanos(-delay));
			RunnableScheduledFuture<Void> t = DecorateTask(command, sft);
			sft.OuterTask = t;
			DelayedExecute(t);
			return t;
		}

		/// <summary>
		/// Executes {@code command} with zero required delay.
		/// This has effect equivalent to
		/// <seealso cref="#schedule(Runnable,long,TimeUnit) schedule(command, 0, anyUnit)"/>.
		/// Note that inspections of the queue and of the list returned by
		/// {@code shutdownNow} will access the zero-delayed
		/// <seealso cref="ScheduledFuture"/>, not the {@code command} itself.
		/// 
		/// <para>A consequence of the use of {@code ScheduledFuture} objects is
		/// that <seealso cref="ThreadPoolExecutor#afterExecute afterExecute"/> is always
		/// called with a null second {@code Throwable} argument, even if the
		/// {@code command} terminated abruptly.  Instead, the {@code Throwable}
		/// thrown by such a task can be obtained via <seealso cref="Future#get"/>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="RejectedExecutionException"> at discretion of
		///         {@code RejectedExecutionHandler}, if the task
		///         cannot be accepted for execution because the
		///         executor has been shut down </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public override void Execute(Runnable command)
		{
			Schedule(command, 0, NANOSECONDS);
		}

		// Override AbstractExecutorService methods

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public Future<?> submit(Runnable task)
		public override Future<?> Submit(Runnable task)
		{
			return Schedule(task, 0, NANOSECONDS);
		}

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
		public override Future<T> submit<T>(Runnable task, T result)
		{
			return Schedule(Executors.Callable(task, result), 0, NANOSECONDS);
		}

		/// <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">       {@inheritDoc} </exception>
		public override Future<T> submit<T>(Callable<T> task)
		{
			return Schedule(task, 0, NANOSECONDS);
		}

		/// <summary>
		/// Sets the policy on whether to continue executing existing
		/// periodic tasks even when this executor has been {@code shutdown}.
		/// In this case, these tasks will only terminate upon
		/// {@code shutdownNow} or after setting the policy to
		/// {@code false} when already shutdown.
		/// This value is by default {@code false}.
		/// </summary>
		/// <param name="value"> if {@code true}, continue after shutdown, else don't </param>
		/// <seealso cref= #getContinueExistingPeriodicTasksAfterShutdownPolicy </seealso>
		public virtual bool ContinueExistingPeriodicTasksAfterShutdownPolicy
		{
			set
			{
				ContinueExistingPeriodicTasksAfterShutdown = value;
				if (!value && Shutdown)
				{
					OnShutdown();
				}
			}
			get
			{
				return ContinueExistingPeriodicTasksAfterShutdown;
			}
		}


		/// <summary>
		/// Sets the policy on whether to execute existing delayed
		/// tasks even when this executor has been {@code shutdown}.
		/// In this case, these tasks will only terminate upon
		/// {@code shutdownNow}, or after setting the policy to
		/// {@code false} when already shutdown.
		/// This value is by default {@code true}.
		/// </summary>
		/// <param name="value"> if {@code true}, execute after shutdown, else don't </param>
		/// <seealso cref= #getExecuteExistingDelayedTasksAfterShutdownPolicy </seealso>
		public virtual bool ExecuteExistingDelayedTasksAfterShutdownPolicy
		{
			set
			{
				ExecuteExistingDelayedTasksAfterShutdown = value;
				if (!value && Shutdown)
				{
					OnShutdown();
				}
			}
			get
			{
				return ExecuteExistingDelayedTasksAfterShutdown;
			}
		}


		/// <summary>
		/// Sets the policy on whether cancelled tasks should be immediately
		/// removed from the work queue at time of cancellation.  This value is
		/// by default {@code false}.
		/// </summary>
		/// <param name="value"> if {@code true}, remove on cancellation, else don't </param>
		/// <seealso cref= #getRemoveOnCancelPolicy
		/// @since 1.7 </seealso>
		public virtual bool RemoveOnCancelPolicy
		{
			set
			{
				RemoveOnCancel = value;
			}
			get
			{
				return RemoveOnCancel;
			}
		}


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
		/// <para>If the {@code ExecuteExistingDelayedTasksAfterShutdownPolicy}
		/// has been set {@code false}, existing delayed tasks whose delays
		/// have not yet elapsed are cancelled.  And unless the {@code
		/// ContinueExistingPeriodicTasksAfterShutdownPolicy} has been set
		/// {@code true}, future executions of existing periodic tasks will
		/// be cancelled.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException"> {@inheritDoc} </exception>
		public override void Shutdown()
		{
			base.Shutdown();
		}

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
		/// processing actively executing tasks.  This implementation
		/// cancels tasks via <seealso cref="Thread#interrupt"/>, so any task that
		/// fails to respond to interrupts may never terminate.
		/// 
		/// </para>
		/// </summary>
		/// <returns> list of tasks that never commenced execution.
		///         Each element of this list is a <seealso cref="ScheduledFuture"/>,
		///         including those tasks submitted using {@code execute},
		///         which are for scheduling purposes used as the basis of a
		///         zero-delay {@code ScheduledFuture}. </returns>
		/// <exception cref="SecurityException"> {@inheritDoc} </exception>
		public override List<Runnable> ShutdownNow()
		{
			return base.ShutdownNow();
		}

		/// <summary>
		/// Returns the task queue used by this executor.  Each element of
		/// this queue is a <seealso cref="ScheduledFuture"/>, including those
		/// tasks submitted using {@code execute} which are for scheduling
		/// purposes used as the basis of a zero-delay
		/// {@code ScheduledFuture}.  Iteration over this queue is
		/// <em>not</em> guaranteed to traverse tasks in the order in
		/// which they will execute.
		/// </summary>
		/// <returns> the task queue </returns>
		public override BlockingQueue<Runnable> Queue
		{
			get
			{
				return base.Queue;
			}
		}

		/// <summary>
		/// Specialized delay queue. To mesh with TPE declarations, this
		/// class must be declared as a BlockingQueue<Runnable> even though
		/// it can only hold RunnableScheduledFutures.
		/// </summary>
		internal class DelayedWorkQueue : AbstractQueue<Runnable>, BlockingQueue<Runnable>
		{
			internal bool InstanceFieldsInitialized = false;

			public DelayedWorkQueue()
			{
				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
			}

			internal virtual void InitializeInstanceFields()
			{
				Available = @lock.NewCondition();
			}


			/*
			 * A DelayedWorkQueue is based on a heap-based data structure
			 * like those in DelayQueue and PriorityQueue, except that
			 * every ScheduledFutureTask also records its index into the
			 * heap array. This eliminates the need to find a task upon
			 * cancellation, greatly speeding up removal (down from O(n)
			 * to O(log n)), and reducing garbage retention that would
			 * otherwise occur by waiting for the element to rise to top
			 * before clearing. But because the queue may also hold
			 * RunnableScheduledFutures that are not ScheduledFutureTasks,
			 * we are not guaranteed to have such indices available, in
			 * which case we fall back to linear search. (We expect that
			 * most tasks will not be decorated, and that the faster cases
			 * will be much more common.)
			 *
			 * All heap operations must record index changes -- mainly
			 * within siftUp and siftDown. Upon removal, a task's
			 * heapIndex is set to -1. Note that ScheduledFutureTasks can
			 * appear at most once in the queue (this need not be true for
			 * other kinds of tasks or work queues), so are uniquely
			 * identified by heapIndex.
			 */

			internal const int INITIAL_CAPACITY = 16;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private RunnableScheduledFuture<?>[] queue = new RunnableScheduledFuture<?>[INITIAL_CAPACITY];
			internal RunnableScheduledFuture<?>[] Queue = new RunnableScheduledFuture<?>[INITIAL_CAPACITY];
			internal readonly ReentrantLock @lock = new ReentrantLock();
			internal int Size_Renamed = 0;

			/// <summary>
			/// Thread designated to wait for the task at the head of the
			/// queue.  This variant of the Leader-Follower pattern
			/// (http://www.cs.wustl.edu/~schmidt/POSA/POSA2/) serves to
			/// minimize unnecessary timed waiting.  When a thread becomes
			/// the leader, it waits only for the next delay to elapse, but
			/// other threads await indefinitely.  The leader thread must
			/// signal some other thread before returning from take() or
			/// poll(...), unless some other thread becomes leader in the
			/// interim.  Whenever the head of the queue is replaced with a
			/// task with an earlier expiration time, the leader field is
			/// invalidated by being reset to null, and some waiting
			/// thread, but not necessarily the current leader, is
			/// signalled.  So waiting threads must be prepared to acquire
			/// and lose leadership while waiting.
			/// </summary>
			internal Thread Leader = null;

			/// <summary>
			/// Condition signalled when a newer task becomes available at the
			/// head of the queue or a new thread may need to become leader.
			/// </summary>
			internal Condition Available;

			/// <summary>
			/// Sets f's heapIndex if it is a ScheduledFutureTask.
			/// </summary>
			internal virtual void setIndex<T1>(RunnableScheduledFuture<T1> f, int idx)
			{
				if (f is ScheduledFutureTask)
				{
					((ScheduledFutureTask)f).heapIndex = idx;
				}
			}

			/// <summary>
			/// Sifts element added at bottom up to its heap-ordered spot.
			/// Call only when holding lock.
			/// </summary>
			internal virtual void siftUp<T1>(int k, RunnableScheduledFuture<T1> key)
			{
				while (k > 0)
				{
					int parent = (int)((uint)(k - 1) >> 1);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> e = queue[parent];
					RunnableScheduledFuture<?> e = Queue[parent];
					if (key.CompareTo(e) >= 0)
					{
						break;
					}
					Queue[k] = e;
					SetIndex(e, k);
					k = parent;
				}
				Queue[k] = key;
				SetIndex(key, k);
			}

			/// <summary>
			/// Sifts element added at top down to its heap-ordered spot.
			/// Call only when holding lock.
			/// </summary>
			internal virtual void siftDown<T1>(int k, RunnableScheduledFuture<T1> key)
			{
				int half = (int)((uint)Size_Renamed >> 1);
				while (k < half)
				{
					int child = (k << 1) + 1;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> c = queue[child];
					RunnableScheduledFuture<?> c = Queue[child];
					int right = child + 1;
					if (right < Size_Renamed && c.CompareTo(Queue[right]) > 0)
					{
						c = Queue[child = right];
					}
					if (key.CompareTo(c) <= 0)
					{
						break;
					}
					Queue[k] = c;
					SetIndex(c, k);
					k = child;
				}
				Queue[k] = key;
				SetIndex(key, k);
			}

			/// <summary>
			/// Resizes the heap array.  Call only when holding lock.
			/// </summary>
			internal virtual void Grow()
			{
				int oldCapacity = Queue.Length;
				int newCapacity = oldCapacity + (oldCapacity >> 1); // grow 50%
				if (newCapacity < 0) // overflow
				{
					newCapacity = Integer.MaxValue;
				}
				Queue = Arrays.CopyOf(Queue, newCapacity);
			}

			/// <summary>
			/// Finds index of given object, or -1 if absent.
			/// </summary>
			internal virtual int IndexOf(Object x)
			{
				if (x != null)
				{
					if (x is ScheduledFutureTask)
					{
						int i = ((ScheduledFutureTask) x).heapIndex;
						// Sanity check; x could conceivably be a
						// ScheduledFutureTask from some other pool.
						if (i >= 0 && i < Size_Renamed && Queue[i] == x)
						{
							return i;
						}
					}
					else
					{
						for (int i = 0; i < Size_Renamed; i++)
						{
							if (x.Equals(Queue[i]))
							{
								return i;
							}
						}
					}
				}
				return -1;
			}

			public virtual bool Contains(Object x)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					return IndexOf(x) != -1;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual bool Remove(Object x)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					int i = IndexOf(x);
					if (i < 0)
					{
						return false;
					}

					SetIndex(Queue[i], -1);
					int s = --Size_Renamed;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> replacement = queue[s];
					RunnableScheduledFuture<?> replacement = Queue[s];
					Queue[s] = null;
					if (s != i)
					{
						SiftDown(i, replacement);
						if (Queue[i] == replacement)
						{
							SiftUp(i, replacement);
						}
					}
					return true;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual int Size()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					return Size_Renamed;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual bool Empty
			{
				get
				{
					return Size() == 0;
				}
			}

			public virtual int RemainingCapacity()
			{
				return Integer.MaxValue;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public RunnableScheduledFuture<?> peek()
			public virtual RunnableScheduledFuture<?> Peek()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					return Queue[0];
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual bool Offer(Runnable x)
			{
				if (x == null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> e = (RunnableScheduledFuture<?>)x;
				RunnableScheduledFuture<?> e = (RunnableScheduledFuture<?>)x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					int i = Size_Renamed;
					if (i >= Queue.Length)
					{
						Grow();
					}
					Size_Renamed = i + 1;
					if (i == 0)
					{
						Queue[0] = e;
						SetIndex(e, 0);
					}
					else
					{
						SiftUp(i, e);
					}
					if (Queue[0] == e)
					{
						Leader = null;
						Available.Signal();
					}
				}
				finally
				{
					@lock.Unlock();
				}
				return true;
			}

			public virtual void Put(Runnable e)
			{
				Offer(e);
			}

			public virtual bool Add(Runnable e)
			{
				return Offer(e);
			}

			public virtual bool Offer(Runnable e, long timeout, TimeUnit unit)
			{
				return Offer(e);
			}

			/// <summary>
			/// Performs common bookkeeping for poll and take: Replaces
			/// first element with last and sifts it down.  Call only when
			/// holding lock. </summary>
			/// <param name="f"> the task to remove and return </param>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private RunnableScheduledFuture<?> finishPoll(RunnableScheduledFuture<?> f)
			internal virtual RunnableScheduledFuture<?> FinishPoll(RunnableScheduledFuture<T1> f)
			{
				int s = --Size_Renamed;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> x = queue[s];
				RunnableScheduledFuture<?> x = Queue[s];
				Queue[s] = null;
				if (s != 0)
				{
					SiftDown(0, x);
				}
				SetIndex(f, -1);
				return f;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public RunnableScheduledFuture<?> poll()
			public virtual RunnableScheduledFuture<?> Poll()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> first = queue[0];
					RunnableScheduledFuture<?> first = Queue[0];
					if (first == null || first.getDelay(NANOSECONDS) > 0)
					{
						return null;
					}
					else
					{
						return FinishPoll(first);
					}
				}
				finally
				{
					@lock.Unlock();
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RunnableScheduledFuture<?> take() throws InterruptedException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public virtual RunnableScheduledFuture<?> Take()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.LockInterruptibly();
				try
				{
					for (;;)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> first = queue[0];
						RunnableScheduledFuture<?> first = Queue[0];
						if (first == null)
						{
							Available.@await();
						}
						else
						{
							long delay = first.getDelay(NANOSECONDS);
							if (delay <= 0)
							{
								return FinishPoll(first);
							}
							first = null; // don't retain ref while waiting
							if (Leader != null)
							{
								Available.@await();
							}
							else
							{
								Thread thisThread = Thread.CurrentThread;
								Leader = thisThread;
								try
								{
									Available.AwaitNanos(delay);
								}
								finally
								{
									if (Leader == thisThread)
									{
										Leader = null;
									}
								}
							}
						}
					}
				}
				finally
				{
					if (Leader == null && Queue[0] != null)
					{
						Available.Signal();
					}
					@lock.Unlock();
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RunnableScheduledFuture<?> poll(long timeout, TimeUnit unit) throws InterruptedException
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public virtual RunnableScheduledFuture<?> Poll(long timeout, TimeUnit unit)
			{
				long nanos = unit.ToNanos(timeout);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.LockInterruptibly();
				try
				{
					for (;;)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> first = queue[0];
						RunnableScheduledFuture<?> first = Queue[0];
						if (first == null)
						{
							if (nanos <= 0)
							{
								return null;
							}
							else
							{
								nanos = Available.AwaitNanos(nanos);
							}
						}
						else
						{
							long delay = first.getDelay(NANOSECONDS);
							if (delay <= 0)
							{
								return FinishPoll(first);
							}
							if (nanos <= 0)
							{
								return null;
							}
							first = null; // don't retain ref while waiting
							if (nanos < delay || Leader != null)
							{
								nanos = Available.AwaitNanos(nanos);
							}
							else
							{
								Thread thisThread = Thread.CurrentThread;
								Leader = thisThread;
								try
								{
									long timeLeft = Available.AwaitNanos(delay);
									nanos -= delay - timeLeft;
								}
								finally
								{
									if (Leader == thisThread)
									{
										Leader = null;
									}
								}
							}
						}
					}
				}
				finally
				{
					if (Leader == null && Queue[0] != null)
					{
						Available.Signal();
					}
					@lock.Unlock();
				}
			}

			public virtual void Clear()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					for (int i = 0; i < Size_Renamed; i++)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> t = queue[i];
						RunnableScheduledFuture<?> t = Queue[i];
						if (t != null)
						{
							Queue[i] = null;
							SetIndex(t, -1);
						}
					}
					Size_Renamed = 0;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			/// <summary>
			/// Returns first element only if it is expired.
			/// Used only by drainTo.  Call only when holding lock.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private RunnableScheduledFuture<?> peekExpired()
			internal virtual RunnableScheduledFuture<?> PeekExpired()
			{
				// assert lock.isHeldByCurrentThread();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> first = queue[0];
				RunnableScheduledFuture<?> first = Queue[0];
				return (first == null || first.getDelay(NANOSECONDS) > 0) ? null : first;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int drainTo(Collection<? base Runnable> c)
			public virtual int drainTo<T1>(Collection<T1> c)
			{
				if (c == null)
				{
					throw new NullPointerException();
				}
				if (c == this)
				{
					throw new IllegalArgumentException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> first;
					RunnableScheduledFuture<?> first;
					int n = 0;
					while ((first = PeekExpired()) != null)
					{
						c.Add(first); // In this order, in case add() throws.
						FinishPoll(first);
						++n;
					}
					return n;
				}
				finally
				{
					@lock.Unlock();
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int drainTo(Collection<? base Runnable> c, int maxElements)
			public virtual int drainTo<T1>(Collection<T1> c, int maxElements)
			{
				if (c == null)
				{
					throw new NullPointerException();
				}
				if (c == this)
				{
					throw new IllegalArgumentException();
				}
				if (maxElements <= 0)
				{
					return 0;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RunnableScheduledFuture<?> first;
					RunnableScheduledFuture<?> first;
					int n = 0;
					while (n < maxElements && (first = PeekExpired()) != null)
					{
						c.Add(first); // In this order, in case add() throws.
						FinishPoll(first);
						++n;
					}
					return n;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual Object[] ToArray()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					return Arrays.CopyOf(Queue, Size_Renamed, typeof(Object[]));
				}
				finally
				{
					@lock.Unlock();
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
			public virtual T[] toArray<T>(T[] a)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					if (a.Length < Size_Renamed)
					{
						return (T[]) Arrays.CopyOf(Queue, Size_Renamed, a.GetType());
					}
					System.Array.Copy(Queue, 0, a, 0, Size_Renamed);
					if (a.Length > Size_Renamed)
					{
						a[Size_Renamed] = null;
					}
					return a;
				}
				finally
				{
					@lock.Unlock();
				}
			}

			public virtual Iterator<Runnable> Iterator()
			{
				return new Itr(this, Arrays.CopyOf(Queue, Size_Renamed));
			}

			/// <summary>
			/// Snapshot iterator that works off copy of underlying q array.
			/// </summary>
			private class Itr : Iterator<Runnable>
			{
				private readonly ScheduledThreadPoolExecutor.DelayedWorkQueue OuterInstance;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final RunnableScheduledFuture<?>[] array;
				internal readonly RunnableScheduledFuture<?>[] Array;
				internal int Cursor = 0; // index of next element to return
				internal int LastRet = -1; // index of last element, or -1 if no such

				internal Itr<T1>(ScheduledThreadPoolExecutor.DelayedWorkQueue outerInstance, RunnableScheduledFuture<T1>[] array)
				{
					this.OuterInstance = outerInstance;
					this.Array = array;
				}

				public virtual bool HasNext()
				{
					return Cursor < Array.Length;
				}

				public virtual Runnable Next()
				{
					if (Cursor >= Array.Length)
					{
						throw new NoSuchElementException();
					}
					LastRet = Cursor;
					return Array[Cursor++];
				}

				public virtual void Remove()
				{
					if (LastRet < 0)
					{
						throw new IllegalStateException();
					}
					OuterInstance.Remove(Array[LastRet]);
					LastRet = -1;
				}
			}
		}
	}

}