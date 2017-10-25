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

namespace java.util.concurrent.locks
{
	using Unsafe = sun.misc.Unsafe;

	/// <summary>
	/// Basic thread blocking primitives for creating locks and other
	/// synchronization classes.
	/// 
	/// <para>This class associates, with each thread that uses it, a permit
	/// (in the sense of the {@link java.util.concurrent.Semaphore
	/// Semaphore} class). A call to {@code park} will return immediately
	/// if the permit is available, consuming it in the process; otherwise
	/// it <em>may</em> block.  A call to {@code unpark} makes the permit
	/// available, if it was not already available. (Unlike with Semaphores
	/// though, permits do not accumulate. There is at most one.)
	/// 
	/// </para>
	/// <para>Methods {@code park} and {@code unpark} provide efficient
	/// means of blocking and unblocking threads that do not encounter the
	/// problems that cause the deprecated methods {@code Thread.suspend}
	/// and {@code Thread.resume} to be unusable for such purposes: Races
	/// between one thread invoking {@code park} and another thread trying
	/// to {@code unpark} it will preserve liveness, due to the
	/// permit. Additionally, {@code park} will return if the caller's
	/// thread was interrupted, and timeout versions are supported. The
	/// {@code park} method may also return at any other time, for "no
	/// reason", so in general must be invoked within a loop that rechecks
	/// conditions upon return. In this sense {@code park} serves as an
	/// optimization of a "busy wait" that does not waste as much time
	/// spinning, but must be paired with an {@code unpark} to be
	/// effective.
	/// 
	/// </para>
	/// <para>The three forms of {@code park} each also support a
	/// {@code blocker} object parameter. This object is recorded while
	/// the thread is blocked to permit monitoring and diagnostic tools to
	/// identify the reasons that threads are blocked. (Such tools may
	/// access blockers using method <seealso cref="#getBlocker(Thread)"/>.)
	/// The use of these forms rather than the original forms without this
	/// parameter is strongly encouraged. The normal argument to supply as
	/// a {@code blocker} within a lock implementation is {@code this}.
	/// 
	/// </para>
	/// <para>These methods are designed to be used as tools for creating
	/// higher-level synchronization utilities, and are not in themselves
	/// useful for most concurrency control applications.  The {@code park}
	/// method is designed for use only in constructions of the form:
	/// 
	///  <pre> {@code
	/// while (!canProceed()) { ... LockSupport.park(this); }}</pre>
	/// 
	/// where neither {@code canProceed} nor any other actions prior to the
	/// call to {@code park} entail locking or blocking.  Because only one
	/// permit is associated with each thread, any intermediary uses of
	/// {@code park} could interfere with its intended effects.
	/// 
	/// </para>
	/// <para><b>Sample Usage.</b> Here is a sketch of a first-in-first-out
	/// non-reentrant lock class:
	///  <pre> {@code
	/// class FIFOMutex {
	///   private final AtomicBoolean locked = new AtomicBoolean(false);
	///   private final Queue<Thread> waiters
	///     = new ConcurrentLinkedQueue<Thread>();
	/// 
	///   public void lock() {
	///     boolean wasInterrupted = false;
	///     Thread current = Thread.currentThread();
	///     waiters.add(current);
	/// 
	///     // Block while not first in queue or cannot acquire lock
	///     while (waiters.peek() != current ||
	///            !locked.compareAndSet(false, true)) {
	///       LockSupport.park(this);
	///       if (Thread.interrupted()) // ignore interrupts while waiting
	///         wasInterrupted = true;
	///     }
	/// 
	///     waiters.remove();
	///     if (wasInterrupted)          // reassert interrupt status on exit
	///       current.interrupt();
	///   }
	/// 
	///   public void unlock() {
	///     locked.set(false);
	///     LockSupport.unpark(waiters.peek());
	///   }
	/// }}</pre>
	/// </para>
	/// </summary>
	public class LockSupport
	{
		private LockSupport() // Cannot be instantiated.
		{
		}

		private static void SetBlocker(Thread t, Object arg)
		{
			// Even though volatile, hotspot doesn't need a write barrier here.
			UNSAFE.putObject(t, ParkBlockerOffset, arg);
		}

		/// <summary>
		/// Makes available the permit for the given thread, if it
		/// was not already available.  If the thread was blocked on
		/// {@code park} then it will unblock.  Otherwise, its next call
		/// to {@code park} is guaranteed not to block. This operation
		/// is not guaranteed to have any effect at all if the given
		/// thread has not been started.
		/// </summary>
		/// <param name="thread"> the thread to unpark, or {@code null}, in which case
		///        this operation has no effect </param>
		public static void Unpark(Thread thread)
		{
			if (thread != null)
			{
				UNSAFE.unpark(thread);
			}
		}

		/// <summary>
		/// Disables the current thread for thread scheduling purposes unless the
		/// permit is available.
		/// 
		/// <para>If the permit is available then it is consumed and the call returns
		/// immediately; otherwise
		/// the current thread becomes disabled for thread scheduling
		/// purposes and lies dormant until one of three things happens:
		/// 
		/// <ul>
		/// <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		/// current thread as the target; or
		/// 
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// 
		/// <li>The call spuriously (that is, for no reason) returns.
		/// </ul>
		/// 
		/// </para>
		/// <para>This method does <em>not</em> report which of these caused the
		/// method to return. Callers should re-check the conditions which caused
		/// the thread to park in the first place. Callers may also determine,
		/// for example, the interrupt status of the thread upon return.
		/// 
		/// </para>
		/// </summary>
		/// <param name="blocker"> the synchronization object responsible for this
		///        thread parking
		/// @since 1.6 </param>
		public static void Park(Object blocker)
		{
			Thread t = Thread.CurrentThread;
			SetBlocker(t, blocker);
			UNSAFE.park(false, 0L);
			SetBlocker(t, null);
		}

		/// <summary>
		/// Disables the current thread for thread scheduling purposes, for up to
		/// the specified waiting time, unless the permit is available.
		/// 
		/// <para>If the permit is available then it is consumed and the call
		/// returns immediately; otherwise the current thread becomes disabled
		/// for thread scheduling purposes and lies dormant until one of four
		/// things happens:
		/// 
		/// <ul>
		/// <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		/// current thread as the target; or
		/// 
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// 
		/// <li>The specified waiting time elapses; or
		/// 
		/// <li>The call spuriously (that is, for no reason) returns.
		/// </ul>
		/// 
		/// </para>
		/// <para>This method does <em>not</em> report which of these caused the
		/// method to return. Callers should re-check the conditions which caused
		/// the thread to park in the first place. Callers may also determine,
		/// for example, the interrupt status of the thread, or the elapsed time
		/// upon return.
		/// 
		/// </para>
		/// </summary>
		/// <param name="blocker"> the synchronization object responsible for this
		///        thread parking </param>
		/// <param name="nanos"> the maximum number of nanoseconds to wait
		/// @since 1.6 </param>
		public static void ParkNanos(Object blocker, long nanos)
		{
			if (nanos > 0)
			{
				Thread t = Thread.CurrentThread;
				SetBlocker(t, blocker);
				UNSAFE.park(false, nanos);
				SetBlocker(t, null);
			}
		}

		/// <summary>
		/// Disables the current thread for thread scheduling purposes, until
		/// the specified deadline, unless the permit is available.
		/// 
		/// <para>If the permit is available then it is consumed and the call
		/// returns immediately; otherwise the current thread becomes disabled
		/// for thread scheduling purposes and lies dormant until one of four
		/// things happens:
		/// 
		/// <ul>
		/// <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		/// current thread as the target; or
		/// 
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/> the
		/// current thread; or
		/// 
		/// <li>The specified deadline passes; or
		/// 
		/// <li>The call spuriously (that is, for no reason) returns.
		/// </ul>
		/// 
		/// </para>
		/// <para>This method does <em>not</em> report which of these caused the
		/// method to return. Callers should re-check the conditions which caused
		/// the thread to park in the first place. Callers may also determine,
		/// for example, the interrupt status of the thread, or the current time
		/// upon return.
		/// 
		/// </para>
		/// </summary>
		/// <param name="blocker"> the synchronization object responsible for this
		///        thread parking </param>
		/// <param name="deadline"> the absolute time, in milliseconds from the Epoch,
		///        to wait until
		/// @since 1.6 </param>
		public static void ParkUntil(Object blocker, long deadline)
		{
			Thread t = Thread.CurrentThread;
			SetBlocker(t, blocker);
			UNSAFE.park(true, deadline);
			SetBlocker(t, null);
		}

		/// <summary>
		/// Returns the blocker object supplied to the most recent
		/// invocation of a park method that has not yet unblocked, or null
		/// if not blocked.  The value returned is just a momentary
		/// snapshot -- the thread may have since unblocked or blocked on a
		/// different blocker object.
		/// </summary>
		/// <param name="t"> the thread </param>
		/// <returns> the blocker </returns>
		/// <exception cref="NullPointerException"> if argument is null
		/// @since 1.6 </exception>
		public static Object GetBlocker(Thread t)
		{
			if (t == null)
			{
				throw new NullPointerException();
			}
			return UNSAFE.getObjectVolatile(t, ParkBlockerOffset);
		}

		/// <summary>
		/// Disables the current thread for thread scheduling purposes unless the
		/// permit is available.
		/// 
		/// <para>If the permit is available then it is consumed and the call
		/// returns immediately; otherwise the current thread becomes disabled
		/// for thread scheduling purposes and lies dormant until one of three
		/// things happens:
		/// 
		/// <ul>
		/// 
		/// <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		/// current thread as the target; or
		/// 
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// 
		/// <li>The call spuriously (that is, for no reason) returns.
		/// </ul>
		/// 
		/// </para>
		/// <para>This method does <em>not</em> report which of these caused the
		/// method to return. Callers should re-check the conditions which caused
		/// the thread to park in the first place. Callers may also determine,
		/// for example, the interrupt status of the thread upon return.
		/// </para>
		/// </summary>
		public static void Park()
		{
			UNSAFE.park(false, 0L);
		}

		/// <summary>
		/// Disables the current thread for thread scheduling purposes, for up to
		/// the specified waiting time, unless the permit is available.
		/// 
		/// <para>If the permit is available then it is consumed and the call
		/// returns immediately; otherwise the current thread becomes disabled
		/// for thread scheduling purposes and lies dormant until one of four
		/// things happens:
		/// 
		/// <ul>
		/// <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		/// current thread as the target; or
		/// 
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// 
		/// <li>The specified waiting time elapses; or
		/// 
		/// <li>The call spuriously (that is, for no reason) returns.
		/// </ul>
		/// 
		/// </para>
		/// <para>This method does <em>not</em> report which of these caused the
		/// method to return. Callers should re-check the conditions which caused
		/// the thread to park in the first place. Callers may also determine,
		/// for example, the interrupt status of the thread, or the elapsed time
		/// upon return.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanos"> the maximum number of nanoseconds to wait </param>
		public static void ParkNanos(long nanos)
		{
			if (nanos > 0)
			{
				UNSAFE.park(false, nanos);
			}
		}

		/// <summary>
		/// Disables the current thread for thread scheduling purposes, until
		/// the specified deadline, unless the permit is available.
		/// 
		/// <para>If the permit is available then it is consumed and the call
		/// returns immediately; otherwise the current thread becomes disabled
		/// for thread scheduling purposes and lies dormant until one of four
		/// things happens:
		/// 
		/// <ul>
		/// <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		/// current thread as the target; or
		/// 
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// 
		/// <li>The specified deadline passes; or
		/// 
		/// <li>The call spuriously (that is, for no reason) returns.
		/// </ul>
		/// 
		/// </para>
		/// <para>This method does <em>not</em> report which of these caused the
		/// method to return. Callers should re-check the conditions which caused
		/// the thread to park in the first place. Callers may also determine,
		/// for example, the interrupt status of the thread, or the current time
		/// upon return.
		/// 
		/// </para>
		/// </summary>
		/// <param name="deadline"> the absolute time, in milliseconds from the Epoch,
		///        to wait until </param>
		public static void ParkUntil(long deadline)
		{
			UNSAFE.park(true, deadline);
		}

		/// <summary>
		/// Returns the pseudo-randomly initialized or updated secondary seed.
		/// Copied from ThreadLocalRandom due to package access restrictions.
		/// </summary>
		internal static int NextSecondarySeed()
		{
			int r;
			Thread t = Thread.CurrentThread;
			if ((r = UNSAFE.getInt(t, SECONDARY)) != 0)
			{
				r ^= r << 13; // xorshift
				r ^= (int)((uint)r >> 17);
				r ^= r << 5;
			}
			else if ((r = java.util.concurrent.ThreadLocalRandom.Current().NextInt()) == 0)
			{
				r = 1; // avoid zero
			}
			UNSAFE.putInt(t, SECONDARY, r);
			return r;
		}

		// Hotspot implementation via intrinsics API
		private static readonly Unsafe UNSAFE;
		private static readonly long ParkBlockerOffset;
		private static readonly long SEED;
		private static readonly long PROBE;
		private static readonly long SECONDARY;
		static LockSupport()
		{
			try
			{
				UNSAFE = Unsafe.Unsafe;
				Class tk = typeof(Thread);
				ParkBlockerOffset = UNSAFE.objectFieldOffset(tk.GetDeclaredField("parkBlocker"));
				SEED = UNSAFE.objectFieldOffset(tk.GetDeclaredField("threadLocalRandomSeed"));
				PROBE = UNSAFE.objectFieldOffset(tk.GetDeclaredField("threadLocalRandomProbe"));
				SECONDARY = UNSAFE.objectFieldOffset(tk.GetDeclaredField("threadLocalRandomSecondarySeed"));
			}
			catch (Exception ex)
			{
				throw new Error(ex);
			}
		}

	}

}