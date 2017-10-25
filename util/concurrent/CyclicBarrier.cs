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
	/// A synchronization aid that allows a set of threads to all wait for
	/// each other to reach a common barrier point.  CyclicBarriers are
	/// useful in programs involving a fixed sized party of threads that
	/// must occasionally wait for each other. The barrier is called
	/// <em>cyclic</em> because it can be re-used after the waiting threads
	/// are released.
	/// 
	/// <para>A {@code CyclicBarrier} supports an optional <seealso cref="Runnable"/> command
	/// that is run once per barrier point, after the last thread in the party
	/// arrives, but before any threads are released.
	/// This <em>barrier action</em> is useful
	/// for updating shared-state before any of the parties continue.
	/// 
	/// </para>
	/// <para><b>Sample usage:</b> Here is an example of using a barrier in a
	/// parallel decomposition design:
	/// 
	///  <pre> {@code
	/// class Solver {
	///   final int N;
	///   final float[][] data;
	///   final CyclicBarrier barrier;
	/// 
	///   class Worker implements Runnable {
	///     int myRow;
	///     Worker(int row) { myRow = row; }
	///     public void run() {
	///       while (!done()) {
	///         processRow(myRow);
	/// 
	///         try {
	///           barrier.await();
	///         } catch (InterruptedException ex) {
	///           return;
	///         } catch (BrokenBarrierException ex) {
	///           return;
	///         }
	///       }
	///     }
	///   }
	/// 
	///   public Solver(float[][] matrix) {
	///     data = matrix;
	///     N = matrix.length;
	///     Runnable barrierAction =
	///       new Runnable() { public void run() { mergeRows(...); }};
	///     barrier = new CyclicBarrier(N, barrierAction);
	/// 
	///     List<Thread> threads = new ArrayList<Thread>(N);
	///     for (int i = 0; i < N; i++) {
	///       Thread thread = new Thread(new Worker(i));
	///       threads.add(thread);
	///       thread.start();
	///     }
	/// 
	///     // wait until done
	///     for (Thread thread : threads)
	///       thread.join();
	///   }
	/// }}</pre>
	/// 
	/// Here, each worker thread processes a row of the matrix then waits at the
	/// barrier until all rows have been processed. When all rows are processed
	/// the supplied <seealso cref="Runnable"/> barrier action is executed and merges the
	/// rows. If the merger
	/// determines that a solution has been found then {@code done()} will return
	/// {@code true} and each worker will terminate.
	/// 
	/// </para>
	/// <para>If the barrier action does not rely on the parties being suspended when
	/// it is executed, then any of the threads in the party could execute that
	/// action when it is released. To facilitate this, each invocation of
	/// <seealso cref="#await"/> returns the arrival index of that thread at the barrier.
	/// You can then choose which thread should execute the barrier action, for
	/// example:
	///  <pre> {@code
	/// if (barrier.await() == 0) {
	///   // log the completion of this iteration
	/// }}</pre>
	/// 
	/// </para>
	/// <para>The {@code CyclicBarrier} uses an all-or-none breakage model
	/// for failed synchronization attempts: If a thread leaves a barrier
	/// point prematurely because of interruption, failure, or timeout, all
	/// other threads waiting at that barrier point will also leave
	/// abnormally via <seealso cref="BrokenBarrierException"/> (or
	/// <seealso cref="InterruptedException"/> if they too were interrupted at about
	/// the same time).
	/// 
	/// </para>
	/// <para>Memory consistency effects: Actions in a thread prior to calling
	/// {@code await()}
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// actions that are part of the barrier action, which in turn
	/// <i>happen-before</i> actions following a successful return from the
	/// corresponding {@code await()} in other threads.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	/// <seealso cref= CountDownLatch
	/// 
	/// @author Doug Lea </seealso>
	public class CyclicBarrier
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			Trip = @lock.NewCondition();
		}

		/// <summary>
		/// Each use of the barrier is represented as a generation instance.
		/// The generation changes whenever the barrier is tripped, or
		/// is reset. There can be many generations associated with threads
		/// using the barrier - due to the non-deterministic way the lock
		/// may be allocated to waiting threads - but only one of these
		/// can be active at a time (the one to which {@code count} applies)
		/// and all the rest are either broken or tripped.
		/// There need not be an active generation if there has been a break
		/// but no subsequent reset.
		/// </summary>
		private class Generation
		{
			internal bool Broken = false;
		}

		/// <summary>
		/// The lock for guarding barrier entry </summary>
		private readonly ReentrantLock @lock = new ReentrantLock();
		/// <summary>
		/// Condition to wait on until tripped </summary>
		private Condition Trip;
		/// <summary>
		/// The number of parties </summary>
		private readonly int Parties_Renamed;
		/* The command to run when tripped */
		private readonly Runnable BarrierCommand;
		/// <summary>
		/// The current generation </summary>
		private Generation Generation = new Generation();

		/// <summary>
		/// Number of parties still waiting. Counts down from parties to 0
		/// on each generation.  It is reset to parties on each new
		/// generation or when broken.
		/// </summary>
		private int Count;

		/// <summary>
		/// Updates state on barrier trip and wakes up everyone.
		/// Called only while holding lock.
		/// </summary>
		private void NextGeneration()
		{
			// signal completion of last generation
			Trip.SignalAll();
			// set up next generation
			Count = Parties_Renamed;
			Generation = new Generation();
		}

		/// <summary>
		/// Sets current barrier generation as broken and wakes up everyone.
		/// Called only while holding lock.
		/// </summary>
		private void BreakBarrier()
		{
			Generation.Broken = true;
			Count = Parties_Renamed;
			Trip.SignalAll();
		}

		/// <summary>
		/// Main barrier code, covering the various policies.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int dowait(boolean timed, long nanos) throws InterruptedException, BrokenBarrierException, TimeoutException
		private int Dowait(bool timed, long nanos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Generation g = generation;
				Generation g = Generation;

				if (g.Broken)
				{
					throw new BrokenBarrierException();
				}

				if (Thread.Interrupted())
				{
					BreakBarrier();
					throw new InterruptedException();
				}

				int index = --Count;
				if (index == 0) // tripped
				{
					bool ranAction = false;
					try
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Runnable command = barrierCommand;
						Runnable command = BarrierCommand;
						if (command != null)
						{
							command.Run();
						}
						ranAction = true;
						NextGeneration();
						return 0;
					}
					finally
					{
						if (!ranAction)
						{
							BreakBarrier();
						}
					}
				}

				// loop until tripped, broken, interrupted, or timed out
				for (;;)
				{
					try
					{
						if (!timed)
						{
							Trip.@await();
						}
						else if (nanos > 0L)
						{
							nanos = Trip.AwaitNanos(nanos);
						}
					}
					catch (InterruptedException ie)
					{
						if (g == Generation && !g.Broken)
						{
							BreakBarrier();
							throw ie;
						}
						else
						{
							// We're about to finish waiting even if we had not
							// been interrupted, so this interrupt is deemed to
							// "belong" to subsequent execution.
							Thread.CurrentThread.Interrupt();
						}
					}

					if (g.Broken)
					{
						throw new BrokenBarrierException();
					}

					if (g != Generation)
					{
						return index;
					}

					if (timed && nanos <= 0L)
					{
						BreakBarrier();
						throw new TimeoutException();
					}
				}
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Creates a new {@code CyclicBarrier} that will trip when the
		/// given number of parties (threads) are waiting upon it, and which
		/// will execute the given barrier action when the barrier is tripped,
		/// performed by the last thread entering the barrier.
		/// </summary>
		/// <param name="parties"> the number of threads that must invoke <seealso cref="#await"/>
		///        before the barrier is tripped </param>
		/// <param name="barrierAction"> the command to execute when the barrier is
		///        tripped, or {@code null} if there is no action </param>
		/// <exception cref="IllegalArgumentException"> if {@code parties} is less than 1 </exception>
		public CyclicBarrier(int parties, Runnable barrierAction)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			if (parties <= 0)
			{
				throw new IllegalArgumentException();
			}
			this.Parties_Renamed = parties;
			this.Count = parties;
			this.BarrierCommand = barrierAction;
		}

		/// <summary>
		/// Creates a new {@code CyclicBarrier} that will trip when the
		/// given number of parties (threads) are waiting upon it, and
		/// does not perform a predefined action when the barrier is tripped.
		/// </summary>
		/// <param name="parties"> the number of threads that must invoke <seealso cref="#await"/>
		///        before the barrier is tripped </param>
		/// <exception cref="IllegalArgumentException"> if {@code parties} is less than 1 </exception>
		public CyclicBarrier(int parties) : this(parties, null)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Returns the number of parties required to trip this barrier.
		/// </summary>
		/// <returns> the number of parties required to trip this barrier </returns>
		public virtual int Parties
		{
			get
			{
				return Parties_Renamed;
			}
		}

		/// <summary>
		/// Waits until all <seealso cref="#getParties parties"/> have invoked
		/// {@code await} on this barrier.
		/// 
		/// <para>If the current thread is not the last to arrive then it is
		/// disabled for thread scheduling purposes and lies dormant until
		/// one of the following things happens:
		/// <ul>
		/// <li>The last thread arrives; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// one of the other waiting threads; or
		/// <li>Some other thread times out while waiting for barrier; or
		/// <li>Some other thread invokes <seealso cref="#reset"/> on this barrier.
		/// </ul>
		/// 
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared.
		/// 
		/// </para>
		/// <para>If the barrier is <seealso cref="#reset"/> while any thread is waiting,
		/// or if the barrier <seealso cref="#isBroken is broken"/> when
		/// {@code await} is invoked, or while any thread is waiting, then
		/// <seealso cref="BrokenBarrierException"/> is thrown.
		/// 
		/// </para>
		/// <para>If any thread is <seealso cref="Thread#interrupt interrupted"/> while waiting,
		/// then all other waiting threads will throw
		/// <seealso cref="BrokenBarrierException"/> and the barrier is placed in the broken
		/// state.
		/// 
		/// </para>
		/// <para>If the current thread is the last thread to arrive, and a
		/// non-null barrier action was supplied in the constructor, then the
		/// current thread runs the action before allowing the other threads to
		/// continue.
		/// If an exception occurs during the barrier action then that exception
		/// will be propagated in the current thread and the barrier is placed in
		/// the broken state.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the arrival index of the current thread, where index
		///         {@code getParties() - 1} indicates the first
		///         to arrive and zero indicates the last to arrive </returns>
		/// <exception cref="InterruptedException"> if the current thread was interrupted
		///         while waiting </exception>
		/// <exception cref="BrokenBarrierException"> if <em>another</em> thread was
		///         interrupted or timed out while the current thread was
		///         waiting, or the barrier was reset, or the barrier was
		///         broken when {@code await} was called, or the barrier
		///         action (if present) failed due to an exception </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int await() throws InterruptedException, BrokenBarrierException
		public virtual int @await()
		{
			try
			{
				return Dowait(false, 0L);
			}
			catch (TimeoutException toe)
			{
				throw new Error(toe); // cannot happen
			}
		}

		/// <summary>
		/// Waits until all <seealso cref="#getParties parties"/> have invoked
		/// {@code await} on this barrier, or the specified waiting time elapses.
		/// 
		/// <para>If the current thread is not the last to arrive then it is
		/// disabled for thread scheduling purposes and lies dormant until
		/// one of the following things happens:
		/// <ul>
		/// <li>The last thread arrives; or
		/// <li>The specified timeout elapses; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// one of the other waiting threads; or
		/// <li>Some other thread times out while waiting for barrier; or
		/// <li>Some other thread invokes <seealso cref="#reset"/> on this barrier.
		/// </ul>
		/// 
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared.
		/// 
		/// </para>
		/// <para>If the specified waiting time elapses then <seealso cref="TimeoutException"/>
		/// is thrown. If the time is less than or equal to zero, the
		/// method will not wait at all.
		/// 
		/// </para>
		/// <para>If the barrier is <seealso cref="#reset"/> while any thread is waiting,
		/// or if the barrier <seealso cref="#isBroken is broken"/> when
		/// {@code await} is invoked, or while any thread is waiting, then
		/// <seealso cref="BrokenBarrierException"/> is thrown.
		/// 
		/// </para>
		/// <para>If any thread is <seealso cref="Thread#interrupt interrupted"/> while
		/// waiting, then all other waiting threads will throw {@link
		/// BrokenBarrierException} and the barrier is placed in the broken
		/// state.
		/// 
		/// </para>
		/// <para>If the current thread is the last thread to arrive, and a
		/// non-null barrier action was supplied in the constructor, then the
		/// current thread runs the action before allowing the other threads to
		/// continue.
		/// If an exception occurs during the barrier action then that exception
		/// will be propagated in the current thread and the barrier is placed in
		/// the broken state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeout"> the time to wait for the barrier </param>
		/// <param name="unit"> the time unit of the timeout parameter </param>
		/// <returns> the arrival index of the current thread, where index
		///         {@code getParties() - 1} indicates the first
		///         to arrive and zero indicates the last to arrive </returns>
		/// <exception cref="InterruptedException"> if the current thread was interrupted
		///         while waiting </exception>
		/// <exception cref="TimeoutException"> if the specified timeout elapses.
		///         In this case the barrier will be broken. </exception>
		/// <exception cref="BrokenBarrierException"> if <em>another</em> thread was
		///         interrupted or timed out while the current thread was
		///         waiting, or the barrier was reset, or the barrier was broken
		///         when {@code await} was called, or the barrier action (if
		///         present) failed due to an exception </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int await(long timeout, TimeUnit unit) throws InterruptedException, BrokenBarrierException, TimeoutException
		public virtual int @await(long timeout, TimeUnit unit)
		{
			return Dowait(true, unit.ToNanos(timeout));
		}

		/// <summary>
		/// Queries if this barrier is in a broken state.
		/// </summary>
		/// <returns> {@code true} if one or more parties broke out of this
		///         barrier due to interruption or timeout since
		///         construction or the last reset, or a barrier action
		///         failed due to an exception; {@code false} otherwise. </returns>
		public virtual bool Broken
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					return Generation.Broken;
				}
				finally
				{
					@lock.Unlock();
				}
			}
		}

		/// <summary>
		/// Resets the barrier to its initial state.  If any parties are
		/// currently waiting at the barrier, they will return with a
		/// <seealso cref="BrokenBarrierException"/>. Note that resets <em>after</em>
		/// a breakage has occurred for other reasons can be complicated to
		/// carry out; threads need to re-synchronize in some other way,
		/// and choose one to perform the reset.  It may be preferable to
		/// instead create a new barrier for subsequent use.
		/// </summary>
		public virtual void Reset()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
			ReentrantLock @lock = this.@lock;
			@lock.@lock();
			try
			{
				BreakBarrier(); // break the current generation
				NextGeneration(); // start a new generation
			}
			finally
			{
				@lock.Unlock();
			}
		}

		/// <summary>
		/// Returns the number of parties currently waiting at the barrier.
		/// This method is primarily useful for debugging and assertions.
		/// </summary>
		/// <returns> the number of parties currently blocked in <seealso cref="#await"/> </returns>
		public virtual int NumberWaiting
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.concurrent.locks.ReentrantLock lock = this.lock;
				ReentrantLock @lock = this.@lock;
				@lock.@lock();
				try
				{
					return Parties_Renamed - Count;
				}
				finally
				{
					@lock.Unlock();
				}
			}
		}
	}

}