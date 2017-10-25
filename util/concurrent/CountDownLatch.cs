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
	/// A synchronization aid that allows one or more threads to wait until
	/// a set of operations being performed in other threads completes.
	/// 
	/// <para>A {@code CountDownLatch} is initialized with a given <em>count</em>.
	/// The <seealso cref="#await await"/> methods block until the current count reaches
	/// zero due to invocations of the <seealso cref="#countDown"/> method, after which
	/// all waiting threads are released and any subsequent invocations of
	/// <seealso cref="#await await"/> return immediately.  This is a one-shot phenomenon
	/// -- the count cannot be reset.  If you need a version that resets the
	/// count, consider using a <seealso cref="CyclicBarrier"/>.
	/// 
	/// </para>
	/// <para>A {@code CountDownLatch} is a versatile synchronization tool
	/// and can be used for a number of purposes.  A
	/// {@code CountDownLatch} initialized with a count of one serves as a
	/// simple on/off latch, or gate: all threads invoking <seealso cref="#await await"/>
	/// wait at the gate until it is opened by a thread invoking {@link
	/// #countDown}.  A {@code CountDownLatch} initialized to <em>N</em>
	/// can be used to make one thread wait until <em>N</em> threads have
	/// completed some action, or some action has been completed N times.
	/// 
	/// </para>
	/// <para>A useful property of a {@code CountDownLatch} is that it
	/// doesn't require that threads calling {@code countDown} wait for
	/// the count to reach zero before proceeding, it simply prevents any
	/// thread from proceeding past an <seealso cref="#await await"/> until all
	/// threads could pass.
	/// 
	/// </para>
	/// <para><b>Sample usage:</b> Here is a pair of classes in which a group
	/// of worker threads use two countdown latches:
	/// <ul>
	/// <li>The first is a start signal that prevents any worker from proceeding
	/// until the driver is ready for them to proceed;
	/// <li>The second is a completion signal that allows the driver to wait
	/// until all workers have completed.
	/// </ul>
	/// 
	///  <pre> {@code
	/// class Driver { // ...
	///   void main() throws InterruptedException {
	///     CountDownLatch startSignal = new CountDownLatch(1);
	///     CountDownLatch doneSignal = new CountDownLatch(N);
	/// 
	///     for (int i = 0; i < N; ++i) // create and start threads
	///       new Thread(new Worker(startSignal, doneSignal)).start();
	/// 
	///     doSomethingElse();            // don't let run yet
	///     startSignal.countDown();      // let all threads proceed
	///     doSomethingElse();
	///     doneSignal.await();           // wait for all to finish
	///   }
	/// }
	/// 
	/// class Worker implements Runnable {
	///   private final CountDownLatch startSignal;
	///   private final CountDownLatch doneSignal;
	///   Worker(CountDownLatch startSignal, CountDownLatch doneSignal) {
	///     this.startSignal = startSignal;
	///     this.doneSignal = doneSignal;
	///   }
	///   public void run() {
	///     try {
	///       startSignal.await();
	///       doWork();
	///       doneSignal.countDown();
	///     } catch (InterruptedException ex) {} // return;
	///   }
	/// 
	///   void doWork() { ... }
	/// }}</pre>
	/// 
	/// </para>
	/// <para>Another typical usage would be to divide a problem into N parts,
	/// describe each part with a Runnable that executes that portion and
	/// counts down on the latch, and queue all the Runnables to an
	/// Executor.  When all sub-parts are complete, the coordinating thread
	/// will be able to pass through await. (When threads must repeatedly
	/// count down in this way, instead use a <seealso cref="CyclicBarrier"/>.)
	/// 
	///  <pre> {@code
	/// class Driver2 { // ...
	///   void main() throws InterruptedException {
	///     CountDownLatch doneSignal = new CountDownLatch(N);
	///     Executor e = ...
	/// 
	///     for (int i = 0; i < N; ++i) // create and start threads
	///       e.execute(new WorkerRunnable(doneSignal, i));
	/// 
	///     doneSignal.await();           // wait for all to finish
	///   }
	/// }
	/// 
	/// class WorkerRunnable implements Runnable {
	///   private final CountDownLatch doneSignal;
	///   private final int i;
	///   WorkerRunnable(CountDownLatch doneSignal, int i) {
	///     this.doneSignal = doneSignal;
	///     this.i = i;
	///   }
	///   public void run() {
	///     try {
	///       doWork(i);
	///       doneSignal.countDown();
	///     } catch (InterruptedException ex) {} // return;
	///   }
	/// 
	///   void doWork() { ... }
	/// }}</pre>
	/// 
	/// </para>
	/// <para>Memory consistency effects: Until the count reaches
	/// zero, actions in a thread prior to calling
	/// {@code countDown()}
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// actions following a successful return from a corresponding
	/// {@code await()} in another thread.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public class CountDownLatch
	{
		/// <summary>
		/// Synchronization control For CountDownLatch.
		/// Uses AQS state to represent count.
		/// </summary>
		private sealed class Sync : AbstractQueuedSynchronizer
		{
			internal const long SerialVersionUID = 4982264981922014374L;

			internal Sync(int count)
			{
				State = count;
			}

			internal int Count
			{
				get
				{
					return State;
				}
			}

			protected internal override int TryAcquireShared(int acquires)
			{
				return (State == 0) ? 1 : -1;
			}

			protected internal override bool TryReleaseShared(int releases)
			{
				// Decrement count; signal when transition to zero
				for (;;)
				{
					int c = State;
					if (c == 0)
					{
						return false;
					}
					int nextc = c - 1;
					if (CompareAndSetState(c, nextc))
					{
						return nextc == 0;
					}
				}
			}
		}

		private readonly Sync Sync;

		/// <summary>
		/// Constructs a {@code CountDownLatch} initialized with the given count.
		/// </summary>
		/// <param name="count"> the number of times <seealso cref="#countDown"/> must be invoked
		///        before threads can pass through <seealso cref="#await"/> </param>
		/// <exception cref="IllegalArgumentException"> if {@code count} is negative </exception>
		public CountDownLatch(int count)
		{
			if (count < 0)
			{
				throw new IllegalArgumentException("count < 0");
			}
			this.Sync = new Sync(count);
		}

		/// <summary>
		/// Causes the current thread to wait until the latch has counted down to
		/// zero, unless the thread is <seealso cref="Thread#interrupt interrupted"/>.
		/// 
		/// <para>If the current count is zero then this method returns immediately.
		/// 
		/// </para>
		/// <para>If the current count is greater than zero then the current
		/// thread becomes disabled for thread scheduling purposes and lies
		/// dormant until one of two things happen:
		/// <ul>
		/// <li>The count reaches zero due to invocations of the
		/// <seealso cref="#countDown"/> method; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread.
		/// </ul>
		/// 
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting,
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		///         while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void await() throws InterruptedException
		public virtual void @await()
		{
			Sync.AcquireSharedInterruptibly(1);
		}

		/// <summary>
		/// Causes the current thread to wait until the latch has counted down to
		/// zero, unless the thread is <seealso cref="Thread#interrupt interrupted"/>,
		/// or the specified waiting time elapses.
		/// 
		/// <para>If the current count is zero then this method returns immediately
		/// with the value {@code true}.
		/// 
		/// </para>
		/// <para>If the current count is greater than zero then the current
		/// thread becomes disabled for thread scheduling purposes and lies
		/// dormant until one of three things happen:
		/// <ul>
		/// <li>The count reaches zero due to invocations of the
		/// <seealso cref="#countDown"/> method; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// <li>The specified waiting time elapses.
		/// </ul>
		/// 
		/// </para>
		/// <para>If the count reaches zero then the method returns with the
		/// value {@code true}.
		/// 
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting,
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared.
		/// 
		/// </para>
		/// <para>If the specified waiting time elapses then the value {@code false}
		/// is returned.  If the time is less than or equal to zero, the method
		/// will not wait at all.
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeout"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the {@code timeout} argument </param>
		/// <returns> {@code true} if the count reached zero and {@code false}
		///         if the waiting time elapsed before the count reached zero </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		///         while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean await(long timeout, TimeUnit unit) throws InterruptedException
		public virtual bool @await(long timeout, TimeUnit unit)
		{
			return Sync.TryAcquireSharedNanos(1, unit.ToNanos(timeout));
		}

		/// <summary>
		/// Decrements the count of the latch, releasing all waiting threads if
		/// the count reaches zero.
		/// 
		/// <para>If the current count is greater than zero then it is decremented.
		/// If the new count is zero then all waiting threads are re-enabled for
		/// thread scheduling purposes.
		/// 
		/// </para>
		/// <para>If the current count equals zero then nothing happens.
		/// </para>
		/// </summary>
		public virtual void CountDown()
		{
			Sync.ReleaseShared(1);
		}

		/// <summary>
		/// Returns the current count.
		/// 
		/// <para>This method is typically used for debugging and testing purposes.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current count </returns>
		public virtual long Count
		{
			get
			{
				return Sync.Count;
			}
		}

		/// <summary>
		/// Returns a string identifying this latch, as well as its state.
		/// The state, in brackets, includes the String {@code "Count ="}
		/// followed by the current count.
		/// </summary>
		/// <returns> a string identifying this latch, as well as its state </returns>
		public override String ToString()
		{
			return base.ToString() + "[Count = " + Sync.Count + "]";
		}
	}

}