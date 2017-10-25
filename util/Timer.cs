using System;
using System.Diagnostics;
using System.Collections;
using System.Threading;

/*
 * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{

	/// <summary>
	/// A facility for threads to schedule tasks for future execution in a
	/// background thread.  Tasks may be scheduled for one-time execution, or for
	/// repeated execution at regular intervals.
	/// 
	/// <para>Corresponding to each <tt>Timer</tt> object is a single background
	/// thread that is used to execute all of the timer's tasks, sequentially.
	/// Timer tasks should complete quickly.  If a timer task takes excessive time
	/// to complete, it "hogs" the timer's task execution thread.  This can, in
	/// turn, delay the execution of subsequent tasks, which may "bunch up" and
	/// execute in rapid succession when (and if) the offending task finally
	/// completes.
	/// 
	/// </para>
	/// <para>After the last live reference to a <tt>Timer</tt> object goes away
	/// <i>and</i> all outstanding tasks have completed execution, the timer's task
	/// execution thread terminates gracefully (and becomes subject to garbage
	/// collection).  However, this can take arbitrarily long to occur.  By
	/// default, the task execution thread does not run as a <i>daemon thread</i>,
	/// so it is capable of keeping an application from terminating.  If a caller
	/// wants to terminate a timer's task execution thread rapidly, the caller
	/// should invoke the timer's <tt>cancel</tt> method.
	/// 
	/// </para>
	/// <para>If the timer's task execution thread terminates unexpectedly, for
	/// example, because its <tt>stop</tt> method is invoked, any further
	/// attempt to schedule a task on the timer will result in an
	/// <tt>IllegalStateException</tt>, as if the timer's <tt>cancel</tt>
	/// method had been invoked.
	/// 
	/// </para>
	/// <para>This class is thread-safe: multiple threads can share a single
	/// <tt>Timer</tt> object without the need for external synchronization.
	/// 
	/// </para>
	/// <para>This class does <i>not</i> offer real-time guarantees: it schedules
	/// tasks using the <tt>Object.wait(long)</tt> method.
	/// 
	/// </para>
	/// <para>Java 5.0 introduced the {@code java.util.concurrent} package and
	/// one of the concurrency utilities therein is the {@link
	/// java.util.concurrent.ScheduledThreadPoolExecutor
	/// ScheduledThreadPoolExecutor} which is a thread pool for repeatedly
	/// executing tasks at a given rate or delay.  It is effectively a more
	/// versatile replacement for the {@code Timer}/{@code TimerTask}
	/// combination, as it allows multiple service threads, accepts various
	/// time units, and doesn't require subclassing {@code TimerTask} (just
	/// implement {@code Runnable}).  Configuring {@code
	/// ScheduledThreadPoolExecutor} with one thread makes it equivalent to
	/// {@code Timer}.
	/// 
	/// </para>
	/// <para>Implementation note: This class scales to large numbers of concurrently
	/// scheduled tasks (thousands should present no problem).  Internally,
	/// it uses a binary heap to represent its task queue, so the cost to schedule
	/// a task is O(log n), where n is the number of concurrently scheduled tasks.
	/// 
	/// </para>
	/// <para>Implementation note: All constructors start a timer thread.
	/// 
	/// @author  Josh Bloch
	/// </para>
	/// </summary>
	/// <seealso cref=     TimerTask </seealso>
	/// <seealso cref=     Object#wait(long)
	/// @since   1.3 </seealso>

	public class Timer
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			Thread = new TimerThread(Queue);
		}

		/// <summary>
		/// The timer task queue.  This data structure is shared with the timer
		/// thread.  The timer produces tasks, via its various schedule calls,
		/// and the timer thread consumes, executing timer tasks as appropriate,
		/// and removing them from the queue when they're obsolete.
		/// </summary>
		private readonly TaskQueue Queue = new TaskQueue();

		/// <summary>
		/// The timer thread.
		/// </summary>
		private TimerThread Thread;

		/// <summary>
		/// This object causes the timer's task execution thread to exit
		/// gracefully when there are no live references to the Timer object and no
		/// tasks in the timer queue.  It is used in preference to a finalizer on
		/// Timer as such a finalizer would be susceptible to a subclass's
		/// finalizer forgetting to call it.
		/// </summary>
		private readonly Object threadReaper = new ObjectAnonymousInnerClassHelper();

		private class ObjectAnonymousInnerClassHelper : Object
		{
			public ObjectAnonymousInnerClassHelper()
			{
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws Throwable
			~ObjectAnonymousInnerClassHelper()
			{
				lock (outerInstance.Queue)
				{
					outerInstance.Thread.newTasksMayBeScheduled = false;
					Monitor.Pulse(outerInstance.Queue); // In case queue is empty.
				}
			}
		}

		/// <summary>
		/// This ID is used to generate thread names.
		/// </summary>
		private static readonly AtomicInteger NextSerialNumber = new AtomicInteger(0);
		private static int SerialNumber()
		{
			return NextSerialNumber.AndIncrement;
		}

		/// <summary>
		/// Creates a new timer.  The associated thread does <i>not</i>
		/// <seealso cref="Thread#setDaemon run as a daemon"/>.
		/// </summary>
		public Timer() : this("Timer-" + SerialNumber())
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Creates a new timer whose associated thread may be specified to
		/// <seealso cref="Thread#setDaemon run as a daemon"/>.
		/// A daemon thread is called for if the timer will be used to
		/// schedule repeating "maintenance activities", which must be
		/// performed as long as the application is running, but should not
		/// prolong the lifetime of the application.
		/// </summary>
		/// <param name="isDaemon"> true if the associated thread should run as a daemon. </param>
		public Timer(bool isDaemon) : this("Timer-" + SerialNumber(), isDaemon)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Creates a new timer whose associated thread has the specified name.
		/// The associated thread does <i>not</i>
		/// <seealso cref="Thread#setDaemon run as a daemon"/>.
		/// </summary>
		/// <param name="name"> the name of the associated thread </param>
		/// <exception cref="NullPointerException"> if {@code name} is null
		/// @since 1.5 </exception>
		public Timer(String name)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			Thread.Name = name;
			Thread.Start();
		}

		/// <summary>
		/// Creates a new timer whose associated thread has the specified name,
		/// and may be specified to
		/// <seealso cref="Thread#setDaemon run as a daemon"/>.
		/// </summary>
		/// <param name="name"> the name of the associated thread </param>
		/// <param name="isDaemon"> true if the associated thread should run as a daemon </param>
		/// <exception cref="NullPointerException"> if {@code name} is null
		/// @since 1.5 </exception>
		public Timer(String name, bool isDaemon)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			Thread.Name = name;
			Thread.Daemon = isDaemon;
			Thread.Start();
		}

		/// <summary>
		/// Schedules the specified task for execution after the specified delay.
		/// </summary>
		/// <param name="task">  task to be scheduled. </param>
		/// <param name="delay"> delay in milliseconds before task is to be executed. </param>
		/// <exception cref="IllegalArgumentException"> if <tt>delay</tt> is negative, or
		///         <tt>delay + System.currentTimeMillis()</tt> is negative. </exception>
		/// <exception cref="IllegalStateException"> if task was already scheduled or
		///         cancelled, timer was cancelled, or timer thread terminated. </exception>
		/// <exception cref="NullPointerException"> if {@code task} is null </exception>
		public virtual void Schedule(TimerTask task, long delay)
		{
			if (delay < 0)
			{
				throw new IllegalArgumentException("Negative delay.");
			}
			Sched(task, DateTimeHelperClass.CurrentUnixTimeMillis() + delay, 0);
		}

		/// <summary>
		/// Schedules the specified task for execution at the specified time.  If
		/// the time is in the past, the task is scheduled for immediate execution.
		/// </summary>
		/// <param name="task"> task to be scheduled. </param>
		/// <param name="time"> time at which task is to be executed. </param>
		/// <exception cref="IllegalArgumentException"> if <tt>time.getTime()</tt> is negative. </exception>
		/// <exception cref="IllegalStateException"> if task was already scheduled or
		///         cancelled, timer was cancelled, or timer thread terminated. </exception>
		/// <exception cref="NullPointerException"> if {@code task} or {@code time} is null </exception>
		public virtual void Schedule(TimerTask task, DateTime time)
		{
			Sched(task, time.Ticks, 0);
		}

		/// <summary>
		/// Schedules the specified task for repeated <i>fixed-delay execution</i>,
		/// beginning after the specified delay.  Subsequent executions take place
		/// at approximately regular intervals separated by the specified period.
		/// 
		/// <para>In fixed-delay execution, each execution is scheduled relative to
		/// the actual execution time of the previous execution.  If an execution
		/// is delayed for any reason (such as garbage collection or other
		/// background activity), subsequent executions will be delayed as well.
		/// In the long run, the frequency of execution will generally be slightly
		/// lower than the reciprocal of the specified period (assuming the system
		/// clock underlying <tt>Object.wait(long)</tt> is accurate).
		/// 
		/// </para>
		/// <para>Fixed-delay execution is appropriate for recurring activities
		/// that require "smoothness."  In other words, it is appropriate for
		/// activities where it is more important to keep the frequency accurate
		/// in the short run than in the long run.  This includes most animation
		/// tasks, such as blinking a cursor at regular intervals.  It also includes
		/// tasks wherein regular activity is performed in response to human
		/// input, such as automatically repeating a character as long as a key
		/// is held down.
		/// 
		/// </para>
		/// </summary>
		/// <param name="task">   task to be scheduled. </param>
		/// <param name="delay">  delay in milliseconds before task is to be executed. </param>
		/// <param name="period"> time in milliseconds between successive task executions. </param>
		/// <exception cref="IllegalArgumentException"> if {@code delay < 0}, or
		///         {@code delay + System.currentTimeMillis() < 0}, or
		///         {@code period <= 0} </exception>
		/// <exception cref="IllegalStateException"> if task was already scheduled or
		///         cancelled, timer was cancelled, or timer thread terminated. </exception>
		/// <exception cref="NullPointerException"> if {@code task} is null </exception>
		public virtual void Schedule(TimerTask task, long delay, long period)
		{
			if (delay < 0)
			{
				throw new IllegalArgumentException("Negative delay.");
			}
			if (period <= 0)
			{
				throw new IllegalArgumentException("Non-positive period.");
			}
			Sched(task, DateTimeHelperClass.CurrentUnixTimeMillis() + delay, -period);
		}

		/// <summary>
		/// Schedules the specified task for repeated <i>fixed-delay execution</i>,
		/// beginning at the specified time. Subsequent executions take place at
		/// approximately regular intervals, separated by the specified period.
		/// 
		/// <para>In fixed-delay execution, each execution is scheduled relative to
		/// the actual execution time of the previous execution.  If an execution
		/// is delayed for any reason (such as garbage collection or other
		/// background activity), subsequent executions will be delayed as well.
		/// In the long run, the frequency of execution will generally be slightly
		/// lower than the reciprocal of the specified period (assuming the system
		/// clock underlying <tt>Object.wait(long)</tt> is accurate).  As a
		/// consequence of the above, if the scheduled first time is in the past,
		/// it is scheduled for immediate execution.
		/// 
		/// </para>
		/// <para>Fixed-delay execution is appropriate for recurring activities
		/// that require "smoothness."  In other words, it is appropriate for
		/// activities where it is more important to keep the frequency accurate
		/// in the short run than in the long run.  This includes most animation
		/// tasks, such as blinking a cursor at regular intervals.  It also includes
		/// tasks wherein regular activity is performed in response to human
		/// input, such as automatically repeating a character as long as a key
		/// is held down.
		/// 
		/// </para>
		/// </summary>
		/// <param name="task">   task to be scheduled. </param>
		/// <param name="firstTime"> First time at which task is to be executed. </param>
		/// <param name="period"> time in milliseconds between successive task executions. </param>
		/// <exception cref="IllegalArgumentException"> if {@code firstTime.getTime() < 0}, or
		///         {@code period <= 0} </exception>
		/// <exception cref="IllegalStateException"> if task was already scheduled or
		///         cancelled, timer was cancelled, or timer thread terminated. </exception>
		/// <exception cref="NullPointerException"> if {@code task} or {@code firstTime} is null </exception>
		public virtual void Schedule(TimerTask task, DateTime firstTime, long period)
		{
			if (period <= 0)
			{
				throw new IllegalArgumentException("Non-positive period.");
			}
			Sched(task, firstTime.Ticks, -period);
		}

		/// <summary>
		/// Schedules the specified task for repeated <i>fixed-rate execution</i>,
		/// beginning after the specified delay.  Subsequent executions take place
		/// at approximately regular intervals, separated by the specified period.
		/// 
		/// <para>In fixed-rate execution, each execution is scheduled relative to the
		/// scheduled execution time of the initial execution.  If an execution is
		/// delayed for any reason (such as garbage collection or other background
		/// activity), two or more executions will occur in rapid succession to
		/// "catch up."  In the long run, the frequency of execution will be
		/// exactly the reciprocal of the specified period (assuming the system
		/// clock underlying <tt>Object.wait(long)</tt> is accurate).
		/// 
		/// </para>
		/// <para>Fixed-rate execution is appropriate for recurring activities that
		/// are sensitive to <i>absolute</i> time, such as ringing a chime every
		/// hour on the hour, or running scheduled maintenance every day at a
		/// particular time.  It is also appropriate for recurring activities
		/// where the total time to perform a fixed number of executions is
		/// important, such as a countdown timer that ticks once every second for
		/// ten seconds.  Finally, fixed-rate execution is appropriate for
		/// scheduling multiple repeating timer tasks that must remain synchronized
		/// with respect to one another.
		/// 
		/// </para>
		/// </summary>
		/// <param name="task">   task to be scheduled. </param>
		/// <param name="delay">  delay in milliseconds before task is to be executed. </param>
		/// <param name="period"> time in milliseconds between successive task executions. </param>
		/// <exception cref="IllegalArgumentException"> if {@code delay < 0}, or
		///         {@code delay + System.currentTimeMillis() < 0}, or
		///         {@code period <= 0} </exception>
		/// <exception cref="IllegalStateException"> if task was already scheduled or
		///         cancelled, timer was cancelled, or timer thread terminated. </exception>
		/// <exception cref="NullPointerException"> if {@code task} is null </exception>
		public virtual void ScheduleAtFixedRate(TimerTask task, long delay, long period)
		{
			if (delay < 0)
			{
				throw new IllegalArgumentException("Negative delay.");
			}
			if (period <= 0)
			{
				throw new IllegalArgumentException("Non-positive period.");
			}
			Sched(task, DateTimeHelperClass.CurrentUnixTimeMillis() + delay, period);
		}

		/// <summary>
		/// Schedules the specified task for repeated <i>fixed-rate execution</i>,
		/// beginning at the specified time. Subsequent executions take place at
		/// approximately regular intervals, separated by the specified period.
		/// 
		/// <para>In fixed-rate execution, each execution is scheduled relative to the
		/// scheduled execution time of the initial execution.  If an execution is
		/// delayed for any reason (such as garbage collection or other background
		/// activity), two or more executions will occur in rapid succession to
		/// "catch up."  In the long run, the frequency of execution will be
		/// exactly the reciprocal of the specified period (assuming the system
		/// clock underlying <tt>Object.wait(long)</tt> is accurate).  As a
		/// consequence of the above, if the scheduled first time is in the past,
		/// then any "missed" executions will be scheduled for immediate "catch up"
		/// execution.
		/// 
		/// </para>
		/// <para>Fixed-rate execution is appropriate for recurring activities that
		/// are sensitive to <i>absolute</i> time, such as ringing a chime every
		/// hour on the hour, or running scheduled maintenance every day at a
		/// particular time.  It is also appropriate for recurring activities
		/// where the total time to perform a fixed number of executions is
		/// important, such as a countdown timer that ticks once every second for
		/// ten seconds.  Finally, fixed-rate execution is appropriate for
		/// scheduling multiple repeating timer tasks that must remain synchronized
		/// with respect to one another.
		/// 
		/// </para>
		/// </summary>
		/// <param name="task">   task to be scheduled. </param>
		/// <param name="firstTime"> First time at which task is to be executed. </param>
		/// <param name="period"> time in milliseconds between successive task executions. </param>
		/// <exception cref="IllegalArgumentException"> if {@code firstTime.getTime() < 0} or
		///         {@code period <= 0} </exception>
		/// <exception cref="IllegalStateException"> if task was already scheduled or
		///         cancelled, timer was cancelled, or timer thread terminated. </exception>
		/// <exception cref="NullPointerException"> if {@code task} or {@code firstTime} is null </exception>
		public virtual void ScheduleAtFixedRate(TimerTask task, DateTime firstTime, long period)
		{
			if (period <= 0)
			{
				throw new IllegalArgumentException("Non-positive period.");
			}
			Sched(task, firstTime.Ticks, period);
		}

		/// <summary>
		/// Schedule the specified timer task for execution at the specified
		/// time with the specified period, in milliseconds.  If period is
		/// positive, the task is scheduled for repeated execution; if period is
		/// zero, the task is scheduled for one-time execution. Time is specified
		/// in Date.getTime() format.  This method checks timer state, task state,
		/// and initial execution time, but not period.
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if <tt>time</tt> is negative. </exception>
		/// <exception cref="IllegalStateException"> if task was already scheduled or
		///         cancelled, timer was cancelled, or timer thread terminated. </exception>
		/// <exception cref="NullPointerException"> if {@code task} is null </exception>
		private void Sched(TimerTask task, long time, long period)
		{
			if (time < 0)
			{
				throw new IllegalArgumentException("Illegal execution time.");
			}

			// Constrain value of period sufficiently to prevent numeric
			// overflow while still being effectively infinitely large.
			if (System.Math.Abs(period) > (Long.MaxValue >> 1))
			{
				period >>= 1;
			}

			lock (Queue)
			{
				if (!Thread.NewTasksMayBeScheduled)
				{
					throw new IllegalStateException("Timer already cancelled.");
				}

				lock (task.@lock)
				{
					if (task.State != TimerTask.VIRGIN)
					{
						throw new IllegalStateException("Task already scheduled or cancelled");
					}
					task.NextExecutionTime = time;
					task.Period = period;
					task.State = TimerTask.SCHEDULED;
				}

				Queue.Add(task);
				if (Queue.Min == task)
				{
					Monitor.Pulse(Queue);
				}
			}
		}

		/// <summary>
		/// Terminates this timer, discarding any currently scheduled tasks.
		/// Does not interfere with a currently executing task (if it exists).
		/// Once a timer has been terminated, its execution thread terminates
		/// gracefully, and no more tasks may be scheduled on it.
		/// 
		/// <para>Note that calling this method from within the run method of a
		/// timer task that was invoked by this timer absolutely guarantees that
		/// the ongoing task execution is the last task execution that will ever
		/// be performed by this timer.
		/// 
		/// </para>
		/// <para>This method may be called repeatedly; the second and subsequent
		/// calls have no effect.
		/// </para>
		/// </summary>
		public virtual void Cancel()
		{
			lock (Queue)
			{
				Thread.NewTasksMayBeScheduled = false;
				Queue.Clear();
				Monitor.Pulse(Queue); // In case queue was already empty.
			}
		}

		/// <summary>
		/// Removes all cancelled tasks from this timer's task queue.  <i>Calling
		/// this method has no effect on the behavior of the timer</i>, but
		/// eliminates the references to the cancelled tasks from the queue.
		/// If there are no external references to these tasks, they become
		/// eligible for garbage collection.
		/// 
		/// <para>Most programs will have no need to call this method.
		/// It is designed for use by the rare application that cancels a large
		/// number of tasks.  Calling this method trades time for space: the
		/// runtime of the method may be proportional to n + c log n, where n
		/// is the number of tasks in the queue and c is the number of cancelled
		/// tasks.
		/// 
		/// </para>
		/// <para>Note that it is permissible to call this method from within a
		/// a task scheduled on this timer.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of tasks removed from the queue.
		/// @since 1.5 </returns>
		 public virtual int Purge()
		 {
			 int result = 0;

			 lock (Queue)
			 {
				 for (int i = Queue.Size(); i > 0; i--)
				 {
					 if (Queue.Get(i).state == TimerTask.CANCELLED)
					 {
						 Queue.QuickRemove(i);
						 result++;
					 }
				 }

				 if (result != 0)
				 {
					 Queue.Heapify();
				 }
			 }

			 return result;
		 }
	}

	/// <summary>
	/// This "helper class" implements the timer's task execution thread, which
	/// waits for tasks on the timer queue, executions them when they fire,
	/// reschedules repeating tasks, and removes cancelled tasks and spent
	/// non-repeating tasks from the queue.
	/// </summary>
	internal class TimerThread : Thread
	{
		/// <summary>
		/// This flag is set to false by the reaper to inform us that there
		/// are no more live references to our Timer object.  Once this flag
		/// is true and there are no more tasks in our queue, there is no
		/// work left for us to do, so we terminate gracefully.  Note that
		/// this field is protected by queue's monitor!
		/// </summary>
		internal bool NewTasksMayBeScheduled = true;

		/// <summary>
		/// Our Timer's queue.  We store this reference in preference to
		/// a reference to the Timer so the reference graph remains acyclic.
		/// Otherwise, the Timer would never be garbage-collected and this
		/// thread would never go away.
		/// </summary>
		private TaskQueue Queue;

		internal TimerThread(TaskQueue queue)
		{
			this.Queue = queue;
		}

		public override void Run()
		{
			try
			{
				MainLoop();
			}
			finally
			{
				// Someone killed this Thread, behave as if Timer cancelled
				lock (Queue)
				{
					NewTasksMayBeScheduled = false;
					Queue.Clear(); // Eliminate obsolete references
				}
			}
		}

		/// <summary>
		/// The main timer loop.  (See class comment.)
		/// </summary>
		private void MainLoop()
		{
			while (true)
			{
				try
				{
					TimerTask task;
					bool taskFired;
					lock (Queue)
					{
						// Wait for queue to become non-empty
						while (Queue.Empty && NewTasksMayBeScheduled)
						{
							Monitor.Wait(Queue);
						}
						if (Queue.Empty)
						{
							break; // Queue is empty and will forever remain; die
						}

						// Queue nonempty; look at first evt and do the right thing
						long currentTime, executionTime;
						task = Queue.Min;
						lock (task.@lock)
						{
							if (task.State == TimerTask.CANCELLED)
							{
								Queue.RemoveMin();
								continue; // No action required, poll queue again
							}
							currentTime = DateTimeHelperClass.CurrentUnixTimeMillis();
							executionTime = task.NextExecutionTime;
							if (taskFired = (executionTime <= currentTime))
							{
								if (task.Period == 0) // Non-repeating, remove
								{
									Queue.RemoveMin();
									task.State = TimerTask.EXECUTED;
								} // Repeating task, reschedule
								else
								{
									Queue.RescheduleMin(task.Period < 0 ? currentTime - task.Period : executionTime + task.Period);
								}
							}
						}
						if (!taskFired) // Task hasn't yet fired; wait
						{
							Monitor.Wait(Queue, TimeSpan.FromMilliseconds(executionTime - currentTime));
						}
					}
					if (taskFired) // Task fired; run it, holding no locks
					{
						task.Run();
					}
				}
				catch (InterruptedException)
				{
				}
			}
		}
	}

	/// <summary>
	/// This class represents a timer task queue: a priority queue of TimerTasks,
	/// ordered on nextExecutionTime.  Each Timer object has one of these, which it
	/// shares with its TimerThread.  Internally this class uses a heap, which
	/// offers log(n) performance for the add, removeMin and rescheduleMin
	/// operations, and constant time performance for the getMin operation.
	/// </summary>
	internal class TaskQueue
	{
		/// <summary>
		/// Priority queue represented as a balanced binary heap: the two children
		/// of queue[n] are queue[2*n] and queue[2*n+1].  The priority queue is
		/// ordered on the nextExecutionTime field: The TimerTask with the lowest
		/// nextExecutionTime is in queue[1] (assuming the queue is nonempty).  For
		/// each node n in the heap, and each descendant of n, d,
		/// n.nextExecutionTime <= d.nextExecutionTime.
		/// </summary>
		private TimerTask[] Queue = new TimerTask[128];

		/// <summary>
		/// The number of tasks in the priority queue.  (The tasks are stored in
		/// queue[1] up to queue[size]).
		/// </summary>
		private int Size_Renamed = 0;

		/// <summary>
		/// Returns the number of tasks currently on the queue.
		/// </summary>
		internal virtual int Size()
		{
			return Size_Renamed;
		}

		/// <summary>
		/// Adds a new task to the priority queue.
		/// </summary>
		internal virtual void Add(TimerTask task)
		{
			// Grow backing store if necessary
			if (Size_Renamed + 1 == Queue.Length)
			{
				Queue = Arrays.CopyOf(Queue, 2 * Queue.Length);
			}

			Queue[++Size_Renamed] = task;
			FixUp(Size_Renamed);
		}

		/// <summary>
		/// Return the "head task" of the priority queue.  (The head task is an
		/// task with the lowest nextExecutionTime.)
		/// </summary>
		internal virtual TimerTask Min
		{
			get
			{
				return Queue[1];
			}
		}

		/// <summary>
		/// Return the ith task in the priority queue, where i ranges from 1 (the
		/// head task, which is returned by getMin) to the number of tasks on the
		/// queue, inclusive.
		/// </summary>
		internal virtual TimerTask Get(int i)
		{
			return Queue[i];
		}

		/// <summary>
		/// Remove the head task from the priority queue.
		/// </summary>
		internal virtual void RemoveMin()
		{
			Queue[1] = Queue[Size_Renamed];
			Queue[Size_Renamed--] = null; // Drop extra reference to prevent memory leak
			FixDown(1);
		}

		/// <summary>
		/// Removes the ith element from queue without regard for maintaining
		/// the heap invariant.  Recall that queue is one-based, so
		/// 1 <= i <= size.
		/// </summary>
		internal virtual void QuickRemove(int i)
		{
			Debug.Assert(i <= Size_Renamed);

			Queue[i] = Queue[Size_Renamed];
			Queue[Size_Renamed--] = null; // Drop extra ref to prevent memory leak
		}

		/// <summary>
		/// Sets the nextExecutionTime associated with the head task to the
		/// specified value, and adjusts priority queue accordingly.
		/// </summary>
		internal virtual void RescheduleMin(long newTime)
		{
			Queue[1].NextExecutionTime = newTime;
			FixDown(1);
		}

		/// <summary>
		/// Returns true if the priority queue contains no elements.
		/// </summary>
		internal virtual bool Empty
		{
			get
			{
				return Size_Renamed == 0;
			}
		}

		/// <summary>
		/// Removes all elements from the priority queue.
		/// </summary>
		internal virtual void Clear()
		{
			// Null out task references to prevent memory leak
			for (int i = 1; i <= Size_Renamed; i++)
			{
				Queue[i] = null;
			}

			Size_Renamed = 0;
		}

		/// <summary>
		/// Establishes the heap invariant (described above) assuming the heap
		/// satisfies the invariant except possibly for the leaf-node indexed by k
		/// (which may have a nextExecutionTime less than its parent's).
		/// 
		/// This method functions by "promoting" queue[k] up the hierarchy
		/// (by swapping it with its parent) repeatedly until queue[k]'s
		/// nextExecutionTime is greater than or equal to that of its parent.
		/// </summary>
		private void FixUp(int k)
		{
			while (k > 1)
			{
				int j = k >> 1;
				if (Queue[j].NextExecutionTime <= Queue[k].NextExecutionTime)
				{
					break;
				}
				TimerTask tmp = Queue[j];
				Queue[j] = Queue[k];
				Queue[k] = tmp;
				k = j;
			}
		}

		/// <summary>
		/// Establishes the heap invariant (described above) in the subtree
		/// rooted at k, which is assumed to satisfy the heap invariant except
		/// possibly for node k itself (which may have a nextExecutionTime greater
		/// than its children's).
		/// 
		/// This method functions by "demoting" queue[k] down the hierarchy
		/// (by swapping it with its smaller child) repeatedly until queue[k]'s
		/// nextExecutionTime is less than or equal to those of its children.
		/// </summary>
		private void FixDown(int k)
		{
			int j;
			while ((j = k << 1) <= Size_Renamed && j > 0)
			{
				if (j < Size_Renamed && Queue[j].NextExecutionTime > Queue[j + 1].NextExecutionTime)
				{
					j++; // j indexes smallest kid
				}
				if (Queue[k].NextExecutionTime <= Queue[j].NextExecutionTime)
				{
					break;
				}
				TimerTask tmp = Queue[j];
				Queue[j] = Queue[k];
				Queue[k] = tmp;
				k = j;
			}
		}

		/// <summary>
		/// Establishes the heap invariant (described above) in the entire tree,
		/// assuming nothing about the order of the elements prior to the call.
		/// </summary>
		internal virtual void Heapify()
		{
			for (int i = Size_Renamed / 2; i >= 1; i--)
			{
				FixDown(i);
			}
		}
	}

}