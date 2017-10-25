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
	/// A cancellable asynchronous computation.  This class provides a base
	/// implementation of <seealso cref="Future"/>, with methods to start and cancel
	/// a computation, query to see if the computation is complete, and
	/// retrieve the result of the computation.  The result can only be
	/// retrieved when the computation has completed; the {@code get}
	/// methods will block if the computation has not yet completed.  Once
	/// the computation has completed, the computation cannot be restarted
	/// or cancelled (unless the computation is invoked using
	/// <seealso cref="#runAndReset"/>).
	/// 
	/// <para>A {@code FutureTask} can be used to wrap a <seealso cref="Callable"/> or
	/// <seealso cref="Runnable"/> object.  Because {@code FutureTask} implements
	/// {@code Runnable}, a {@code FutureTask} can be submitted to an
	/// <seealso cref="Executor"/> for execution.
	/// 
	/// </para>
	/// <para>In addition to serving as a standalone class, this class provides
	/// {@code protected} functionality that may be useful when creating
	/// customized task classes.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <V> The result type returned by this FutureTask's {@code get} methods </param>
	public class FutureTask<V> : RunnableFuture<V>
	{
		/*
		 * Revision notes: This differs from previous versions of this
		 * class that relied on AbstractQueuedSynchronizer, mainly to
		 * avoid surprising users about retaining interrupt status during
		 * cancellation races. Sync control in the current design relies
		 * on a "state" field updated via CAS to track completion, along
		 * with a simple Treiber stack to hold waiting threads.
		 *
		 * Style note: As usual, we bypass overhead of using
		 * AtomicXFieldUpdaters and instead directly use Unsafe intrinsics.
		 */

		/// <summary>
		/// The run state of this task, initially NEW.  The run state
		/// transitions to a terminal state only in methods set,
		/// setException, and cancel.  During completion, state may take on
		/// transient values of COMPLETING (while outcome is being set) or
		/// INTERRUPTING (only while interrupting the runner to satisfy a
		/// cancel(true)). Transitions from these intermediate to final
		/// states use cheaper ordered/lazy writes because values are unique
		/// and cannot be further modified.
		/// 
		/// Possible state transitions:
		/// NEW -> COMPLETING -> NORMAL
		/// NEW -> COMPLETING -> EXCEPTIONAL
		/// NEW -> CANCELLED
		/// NEW -> INTERRUPTING -> INTERRUPTED
		/// </summary>
		private volatile int State;
		private const int NEW = 0;
		private const int COMPLETING = 1;
		private const int NORMAL = 2;
		private const int EXCEPTIONAL = 3;
		private const int CANCELLED = 4;
		private const int INTERRUPTING = 5;
		private const int INTERRUPTED = 6;

		/// <summary>
		/// The underlying callable; nulled out after running </summary>
		private Callable<V> Callable;
		/// <summary>
		/// The result to return or exception to throw from get() </summary>
		private Object Outcome; // non-volatile, protected by state reads/writes
		/// <summary>
		/// The thread running the callable; CASed during run() </summary>
		private volatile Thread Runner;
		/// <summary>
		/// Treiber stack of waiting threads </summary>
		private volatile WaitNode Waiters;

		/// <summary>
		/// Returns result or throws exception for completed task.
		/// </summary>
		/// <param name="s"> completed state value </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private V report(int s) throws ExecutionException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		private V Report(int s)
		{
			Object x = Outcome;
			if (s == NORMAL)
			{
				return (V)x;
			}
			if (s >= CANCELLED)
			{
				throw new CancellationException();
			}
			throw new ExecutionException((Throwable)x);
		}

		/// <summary>
		/// Creates a {@code FutureTask} that will, upon running, execute the
		/// given {@code Callable}.
		/// </summary>
		/// <param name="callable"> the callable task </param>
		/// <exception cref="NullPointerException"> if the callable is null </exception>
		public FutureTask(Callable<V> callable)
		{
			if (callable == null)
			{
				throw new NullPointerException();
			}
			this.Callable = callable;
			this.State = NEW; // ensure visibility of callable
		}

		/// <summary>
		/// Creates a {@code FutureTask} that will, upon running, execute the
		/// given {@code Runnable}, and arrange that {@code get} will return the
		/// given result on successful completion.
		/// </summary>
		/// <param name="runnable"> the runnable task </param>
		/// <param name="result"> the result to return on successful completion. If
		/// you don't need a particular result, consider using
		/// constructions of the form:
		/// {@code Future<?> f = new FutureTask<Void>(runnable, null)} </param>
		/// <exception cref="NullPointerException"> if the runnable is null </exception>
		public FutureTask(Runnable runnable, V result)
		{
			this.Callable = Executors.Callable(runnable, result);
			this.State = NEW; // ensure visibility of callable
		}

		public virtual bool Cancelled
		{
			get
			{
				return State >= CANCELLED;
			}
		}

		public virtual bool Done
		{
			get
			{
				return State != NEW;
			}
		}

		public virtual bool Cancel(bool mayInterruptIfRunning)
		{
			if (!(State == NEW && UNSAFE.compareAndSwapInt(this, StateOffset, NEW, mayInterruptIfRunning ? INTERRUPTING : CANCELLED)))
			{
				return false;
			}
			try // in case call to interrupt throws exception
			{
				if (mayInterruptIfRunning)
				{
					try
					{
						Thread t = Runner;
						if (t != null)
						{
							t.Interrupt();
						}
					} // final state
					finally
					{
						UNSAFE.putOrderedInt(this, StateOffset, INTERRUPTED);
					}
				}
			}
			finally
			{
				FinishCompletion();
			}
			return true;
		}

		/// <exception cref="CancellationException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public V get() throws InterruptedException, ExecutionException
		public virtual V Get()
		{
			int s = State;
			if (s <= COMPLETING)
			{
				s = AwaitDone(false, 0L);
			}
			return Report(s);
		}

		/// <exception cref="CancellationException"> {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public V get(long timeout, TimeUnit unit) throws InterruptedException, ExecutionException, TimeoutException
		public virtual V Get(long timeout, TimeUnit unit)
		{
			if (unit == null)
			{
				throw new NullPointerException();
			}
			int s = State;
			if (s <= COMPLETING && (s = AwaitDone(true, unit.ToNanos(timeout))) <= COMPLETING)
			{
				throw new TimeoutException();
			}
			return Report(s);
		}

		/// <summary>
		/// Protected method invoked when this task transitions to state
		/// {@code isDone} (whether normally or via cancellation). The
		/// default implementation does nothing.  Subclasses may override
		/// this method to invoke completion callbacks or perform
		/// bookkeeping. Note that you can query status inside the
		/// implementation of this method to determine whether this task
		/// has been cancelled.
		/// </summary>
		protected internal virtual void Done()
		{
		}

		/// <summary>
		/// Sets the result of this future to the given value unless
		/// this future has already been set or has been cancelled.
		/// 
		/// <para>This method is invoked internally by the <seealso cref="#run"/> method
		/// upon successful completion of the computation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="v"> the value </param>
		protected internal virtual void Set(V v)
		{
			if (UNSAFE.compareAndSwapInt(this, StateOffset, NEW, COMPLETING))
			{
				Outcome = v;
				UNSAFE.putOrderedInt(this, StateOffset, NORMAL); // final state
				FinishCompletion();
			}
		}

		/// <summary>
		/// Causes this future to report an <seealso cref="ExecutionException"/>
		/// with the given throwable as its cause, unless this future has
		/// already been set or has been cancelled.
		/// 
		/// <para>This method is invoked internally by the <seealso cref="#run"/> method
		/// upon failure of the computation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="t"> the cause of failure </param>
		protected internal virtual Throwable Exception
		{
			set
			{
				if (UNSAFE.compareAndSwapInt(this, StateOffset, NEW, COMPLETING))
				{
					Outcome = value;
					UNSAFE.putOrderedInt(this, StateOffset, EXCEPTIONAL); // final state
					FinishCompletion();
				}
			}
		}

		public virtual void Run()
		{
			if (State != NEW || !UNSAFE.compareAndSwapObject(this, RunnerOffset, null, Thread.CurrentThread))
			{
				return;
			}
			try
			{
				Callable<V> c = Callable;
				if (c != null && State == NEW)
				{
					V result;
					bool ran;
					try
					{
						result = c.Call();
						ran = true;
					}
					catch (Throwable ex)
					{
						result = null;
						ran = false;
						Exception = ex;
					}
					if (ran)
					{
						Set(result);
					}
				}
			}
			finally
			{
				// runner must be non-null until state is settled to
				// prevent concurrent calls to run()
				Runner = null;
				// state must be re-read after nulling runner to prevent
				// leaked interrupts
				int s = State;
				if (s >= INTERRUPTING)
				{
					HandlePossibleCancellationInterrupt(s);
				}
			}
		}

		/// <summary>
		/// Executes the computation without setting its result, and then
		/// resets this future to initial state, failing to do so if the
		/// computation encounters an exception or is cancelled.  This is
		/// designed for use with tasks that intrinsically execute more
		/// than once.
		/// </summary>
		/// <returns> {@code true} if successfully run and reset </returns>
		protected internal virtual bool RunAndReset()
		{
			if (State != NEW || !UNSAFE.compareAndSwapObject(this, RunnerOffset, null, Thread.CurrentThread))
			{
				return false;
			}
			bool ran = false;
			int s = State;
			try
			{
				Callable<V> c = Callable;
				if (c != null && s == NEW)
				{
					try
					{
						c.Call(); // don't set result
						ran = true;
					}
					catch (Throwable ex)
					{
						Exception = ex;
					}
				}
			}
			finally
			{
				// runner must be non-null until state is settled to
				// prevent concurrent calls to run()
				Runner = null;
				// state must be re-read after nulling runner to prevent
				// leaked interrupts
				s = State;
				if (s >= INTERRUPTING)
				{
					HandlePossibleCancellationInterrupt(s);
				}
			}
			return ran && s == NEW;
		}

		/// <summary>
		/// Ensures that any interrupt from a possible cancel(true) is only
		/// delivered to a task while in run or runAndReset.
		/// </summary>
		private void HandlePossibleCancellationInterrupt(int s)
		{
			// It is possible for our interrupter to stall before getting a
			// chance to interrupt us.  Let's spin-wait patiently.
			if (s == INTERRUPTING)
			{
				while (State == INTERRUPTING)
				{
					Thread.@yield(); // wait out pending interrupt
				}
			}

			// assert state == INTERRUPTED;

			// We want to clear any interrupt we may have received from
			// cancel(true).  However, it is permissible to use interrupts
			// as an independent mechanism for a task to communicate with
			// its caller, and there is no way to clear only the
			// cancellation interrupt.
			//
			// Thread.interrupted();
		}

		/// <summary>
		/// Simple linked list nodes to record waiting threads in a Treiber
		/// stack.  See other classes such as Phaser and SynchronousQueue
		/// for more detailed explanation.
		/// </summary>
		internal sealed class WaitNode
		{
			internal volatile Thread Thread;
			internal volatile WaitNode Next;
			internal WaitNode()
			{
				Thread = Thread.CurrentThread;
			}
		}

		/// <summary>
		/// Removes and signals all waiting threads, invokes done(), and
		/// nulls out callable.
		/// </summary>
		private void FinishCompletion()
		{
			// assert state > COMPLETING;
			for (WaitNode q; (q = Waiters) != null;)
			{
				if (UNSAFE.compareAndSwapObject(this, WaitersOffset, q, null))
				{
					for (;;)
					{
						Thread t = q.thread;
						if (t != null)
						{
							q.thread = null;
							LockSupport.Unpark(t);
						}
						WaitNode next = q.next;
						if (next == null)
						{
							break;
						}
						q.next = null; // unlink to help gc
						q = next;
					}
					break;
				}
			}

			Done();

			Callable = null; // to reduce footprint
		}

		/// <summary>
		/// Awaits completion or aborts on interrupt or timeout.
		/// </summary>
		/// <param name="timed"> true if use timed waits </param>
		/// <param name="nanos"> time to wait, if timed </param>
		/// <returns> state upon completion </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int awaitDone(boolean timed, long nanos) throws InterruptedException
		private int AwaitDone(bool timed, long nanos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long deadline = timed ? System.nanoTime() + nanos : 0L;
			long deadline = timed ? System.nanoTime() + nanos : 0L;
			WaitNode q = null;
			bool queued = false;
			for (;;)
			{
				if (Thread.Interrupted())
				{
					RemoveWaiter(q);
					throw new InterruptedException();
				}

				int s = State;
				if (s > COMPLETING)
				{
					if (q != null)
					{
						q.Thread = null;
					}
					return s;
				}
				else if (s == COMPLETING) // cannot time out yet
				{
					Thread.@yield();
				}
				else if (q == null)
				{
					q = new WaitNode();
				}
				else if (!queued)
				{
					queued = UNSAFE.compareAndSwapObject(this, WaitersOffset, q.Next = Waiters, q);
				}
				else if (timed)
				{
					nanos = deadline - System.nanoTime();
					if (nanos <= 0L)
					{
						RemoveWaiter(q);
						return State;
					}
					LockSupport.ParkNanos(this, nanos);
				}
				else
				{
					LockSupport.Park(this);
				}
			}
		}

		/// <summary>
		/// Tries to unlink a timed-out or interrupted wait node to avoid
		/// accumulating garbage.  Internal nodes are simply unspliced
		/// without CAS since it is harmless if they are traversed anyway
		/// by releasers.  To avoid effects of unsplicing from already
		/// removed nodes, the list is retraversed in case of an apparent
		/// race.  This is slow when there are a lot of nodes, but we don't
		/// expect lists to be long enough to outweigh higher-overhead
		/// schemes.
		/// </summary>
		private void RemoveWaiter(WaitNode node)
		{
			if (node != null)
			{
				node.Thread = null;
				for (;;) // restart on removeWaiter race
				{
					for (WaitNode pred = null, q = Waiters, s; q != null; q = s)
					{
						s = q.next;
						if (q.thread != null)
						{
							pred = q;
						}
						else if (pred != null)
						{
							pred.next = s;
							if (pred.thread == null) // check for race
							{
								goto retryContinue;
							}
						}
						else if (!UNSAFE.compareAndSwapObject(this, WaitersOffset, q, s))
						{
							goto retryContinue;
						}
					}
					break;
					retryContinue:;
				}
				retryBreak:;
			}
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long StateOffset;
		private static readonly long RunnerOffset;
		private static readonly long WaitersOffset;
		static FutureTask()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class k = typeof(FutureTask);
				StateOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("state"));
				RunnerOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("runner"));
				WaitersOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("waiters"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}

	}

}