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


	/// <summary>
	/// A capability-based lock with three modes for controlling read/write
	/// access.  The state of a StampedLock consists of a version and mode.
	/// Lock acquisition methods return a stamp that represents and
	/// controls access with respect to a lock state; "try" versions of
	/// these methods may instead return the special value zero to
	/// represent failure to acquire access. Lock release and conversion
	/// methods require stamps as arguments, and fail if they do not match
	/// the state of the lock. The three modes are:
	/// 
	/// <ul>
	/// 
	///  <li><b>Writing.</b> Method <seealso cref="#writeLock"/> possibly blocks
	///   waiting for exclusive access, returning a stamp that can be used
	///   in method <seealso cref="#unlockWrite"/> to release the lock. Untimed and
	///   timed versions of {@code tryWriteLock} are also provided. When
	///   the lock is held in write mode, no read locks may be obtained,
	///   and all optimistic read validations will fail.  </li>
	/// 
	///  <li><b>Reading.</b> Method <seealso cref="#readLock"/> possibly blocks
	///   waiting for non-exclusive access, returning a stamp that can be
	///   used in method <seealso cref="#unlockRead"/> to release the lock. Untimed
	///   and timed versions of {@code tryReadLock} are also provided. </li>
	/// 
	///  <li><b>Optimistic Reading.</b> Method <seealso cref="#tryOptimisticRead"/>
	///   returns a non-zero stamp only if the lock is not currently held
	///   in write mode. Method <seealso cref="#validate"/> returns true if the lock
	///   has not been acquired in write mode since obtaining a given
	///   stamp.  This mode can be thought of as an extremely weak version
	///   of a read-lock, that can be broken by a writer at any time.  The
	///   use of optimistic mode for short read-only code segments often
	///   reduces contention and improves throughput.  However, its use is
	///   inherently fragile.  Optimistic read sections should only read
	///   fields and hold them in local variables for later use after
	///   validation. Fields read while in optimistic mode may be wildly
	///   inconsistent, so usage applies only when you are familiar enough
	///   with data representations to check consistency and/or repeatedly
	///   invoke method {@code validate()}.  For example, such steps are
	///   typically required when first reading an object or array
	///   reference, and then accessing one of its fields, elements or
	///   methods. </li>
	/// 
	/// </ul>
	/// 
	/// <para>This class also supports methods that conditionally provide
	/// conversions across the three modes. For example, method {@link
	/// #tryConvertToWriteLock} attempts to "upgrade" a mode, returning
	/// a valid write stamp if (1) already in writing mode (2) in reading
	/// mode and there are no other readers or (3) in optimistic mode and
	/// the lock is available. The forms of these methods are designed to
	/// help reduce some of the code bloat that otherwise occurs in
	/// retry-based designs.
	/// 
	/// </para>
	/// <para>StampedLocks are designed for use as internal utilities in the
	/// development of thread-safe components. Their use relies on
	/// knowledge of the internal properties of the data, objects, and
	/// methods they are protecting.  They are not reentrant, so locked
	/// bodies should not call other unknown methods that may try to
	/// re-acquire locks (although you may pass a stamp to other methods
	/// that can use or convert it).  The use of read lock modes relies on
	/// the associated code sections being side-effect-free.  Unvalidated
	/// optimistic read sections cannot call methods that are not known to
	/// tolerate potential inconsistencies.  Stamps use finite
	/// representations, and are not cryptographically secure (i.e., a
	/// valid stamp may be guessable). Stamp values may recycle after (no
	/// sooner than) one year of continuous operation. A stamp held without
	/// use or validation for longer than this period may fail to validate
	/// correctly.  StampedLocks are serializable, but always deserialize
	/// into initial unlocked state, so they are not useful for remote
	/// locking.
	/// 
	/// </para>
	/// <para>The scheduling policy of StampedLock does not consistently
	/// prefer readers over writers or vice versa.  All "try" methods are
	/// best-effort and do not necessarily conform to any scheduling or
	/// fairness policy. A zero return from any "try" method for acquiring
	/// or converting locks does not carry any information about the state
	/// of the lock; a subsequent invocation may succeed.
	/// 
	/// </para>
	/// <para>Because it supports coordinated usage across multiple lock
	/// modes, this class does not directly implement the <seealso cref="Lock"/> or
	/// <seealso cref="ReadWriteLock"/> interfaces. However, a StampedLock may be
	/// viewed <seealso cref="#asReadLock()"/>, <seealso cref="#asWriteLock()"/>, or {@link
	/// #asReadWriteLock()} in applications requiring only the associated
	/// set of functionality.
	/// 
	/// </para>
	/// <para><b>Sample Usage.</b> The following illustrates some usage idioms
	/// in a class that maintains simple two-dimensional points. The sample
	/// code illustrates some try/catch conventions even though they are
	/// not strictly needed here because no exceptions can occur in their
	/// bodies.<br>
	/// 
	///  <pre>{@code
	/// class Point {
	///   private double x, y;
	///   private final StampedLock sl = new StampedLock();
	/// 
	///   void move(double deltaX, double deltaY) { // an exclusively locked method
	///     long stamp = sl.writeLock();
	///     try {
	///       x += deltaX;
	///       y += deltaY;
	///     } finally {
	///       sl.unlockWrite(stamp);
	///     }
	///   }
	/// 
	///   double distanceFromOrigin() { // A read-only method
	///     long stamp = sl.tryOptimisticRead();
	///     double currentX = x, currentY = y;
	///     if (!sl.validate(stamp)) {
	///        stamp = sl.readLock();
	///        try {
	///          currentX = x;
	///          currentY = y;
	///        } finally {
	///           sl.unlockRead(stamp);
	///        }
	///     }
	///     return Math.sqrt(currentX * currentX + currentY * currentY);
	///   }
	/// 
	///   void moveIfAtOrigin(double newX, double newY) { // upgrade
	///     // Could instead start with optimistic, not read mode
	///     long stamp = sl.readLock();
	///     try {
	///       while (x == 0.0 && y == 0.0) {
	///         long ws = sl.tryConvertToWriteLock(stamp);
	///         if (ws != 0L) {
	///           stamp = ws;
	///           x = newX;
	///           y = newY;
	///           break;
	///         }
	///         else {
	///           sl.unlockRead(stamp);
	///           stamp = sl.writeLock();
	///         }
	///       }
	///     } finally {
	///       sl.unlock(stamp);
	///     }
	///   }
	/// }}</pre>
	/// 
	/// @since 1.8
	/// @author Doug Lea
	/// </para>
	/// </summary>
	[Serializable]
	public class StampedLock
	{
		/*
		 * Algorithmic notes:
		 *
		 * The design employs elements of Sequence locks
		 * (as used in linux kernels; see Lameter's
		 * http://www.lameter.com/gelato2005.pdf
		 * and elsewhere; see
		 * Boehm's http://www.hpl.hp.com/techreports/2012/HPL-2012-68.html)
		 * and Ordered RW locks (see Shirako et al
		 * http://dl.acm.org/citation.cfm?id=2312015)
		 *
		 * Conceptually, the primary state of the lock includes a sequence
		 * number that is odd when write-locked and even otherwise.
		 * However, this is offset by a reader count that is non-zero when
		 * read-locked.  The read count is ignored when validating
		 * "optimistic" seqlock-reader-style stamps.  Because we must use
		 * a small finite number of bits (currently 7) for readers, a
		 * supplementary reader overflow word is used when the number of
		 * readers exceeds the count field. We do this by treating the max
		 * reader count value (RBITS) as a spinlock protecting overflow
		 * updates.
		 *
		 * Waiters use a modified form of CLH lock used in
		 * AbstractQueuedSynchronizer (see its internal documentation for
		 * a fuller account), where each node is tagged (field mode) as
		 * either a reader or writer. Sets of waiting readers are grouped
		 * (linked) under a common node (field cowait) so act as a single
		 * node with respect to most CLH mechanics.  By virtue of the
		 * queue structure, wait nodes need not actually carry sequence
		 * numbers; we know each is greater than its predecessor.  This
		 * simplifies the scheduling policy to a mainly-FIFO scheme that
		 * incorporates elements of Phase-Fair locks (see Brandenburg &
		 * Anderson, especially http://www.cs.unc.edu/~bbb/diss/).  In
		 * particular, we use the phase-fair anti-barging rule: If an
		 * incoming reader arrives while read lock is held but there is a
		 * queued writer, this incoming reader is queued.  (This rule is
		 * responsible for some of the complexity of method acquireRead,
		 * but without it, the lock becomes highly unfair.) Method release
		 * does not (and sometimes cannot) itself wake up cowaiters. This
		 * is done by the primary thread, but helped by any other threads
		 * with nothing better to do in methods acquireRead and
		 * acquireWrite.
		 *
		 * These rules apply to threads actually queued. All tryLock forms
		 * opportunistically try to acquire locks regardless of preference
		 * rules, and so may "barge" their way in.  Randomized spinning is
		 * used in the acquire methods to reduce (increasingly expensive)
		 * context switching while also avoiding sustained memory
		 * thrashing among many threads.  We limit spins to the head of
		 * queue. A thread spin-waits up to SPINS times (where each
		 * iteration decreases spin count with 50% probability) before
		 * blocking. If, upon wakening it fails to obtain lock, and is
		 * still (or becomes) the first waiting thread (which indicates
		 * that some other thread barged and obtained lock), it escalates
		 * spins (up to MAX_HEAD_SPINS) to reduce the likelihood of
		 * continually losing to barging threads.
		 *
		 * Nearly all of these mechanics are carried out in methods
		 * acquireWrite and acquireRead, that, as typical of such code,
		 * sprawl out because actions and retries rely on consistent sets
		 * of locally cached reads.
		 *
		 * As noted in Boehm's paper (above), sequence validation (mainly
		 * method validate()) requires stricter ordering rules than apply
		 * to normal volatile reads (of "state").  To force orderings of
		 * reads before a validation and the validation itself in those
		 * cases where this is not already forced, we use
		 * Unsafe.loadFence.
		 *
		 * The memory layout keeps lock state and queue pointers together
		 * (normally on the same cache line). This usually works well for
		 * read-mostly loads. In most other cases, the natural tendency of
		 * adaptive-spin CLH locks to reduce memory contention lessens
		 * motivation to further spread out contended locations, but might
		 * be subject to future improvements.
		 */

		private const long SerialVersionUID = -6001602636862214147L;

		/// <summary>
		/// Number of processors, for spin control </summary>
		private static readonly int NCPU = Runtime.Runtime.availableProcessors();

		/// <summary>
		/// Maximum number of retries before enqueuing on acquisition </summary>
		private static readonly int SPINS = (NCPU > 1) ? 1 << 6 : 0;

		/// <summary>
		/// Maximum number of retries before blocking at head on acquisition </summary>
		private static readonly int HEAD_SPINS = (NCPU > 1) ? 1 << 10 : 0;

		/// <summary>
		/// Maximum number of retries before re-blocking </summary>
		private static readonly int MAX_HEAD_SPINS = (NCPU > 1) ? 1 << 16 : 0;

		/// <summary>
		/// The period for yielding when waiting for overflow spinlock </summary>
		private const int OVERFLOW_YIELD_RATE = 7; // must be power 2 - 1

		/// <summary>
		/// The number of bits to use for reader count before overflowing </summary>
		private const int LG_READERS = 7;

		// Values for lock state and stamp operations
		private const long RUNIT = 1L;
		private static readonly long WBIT = 1L << LG_READERS;
		private static readonly long RBITS = WBIT - 1L;
		private static readonly long RFULL = RBITS - 1L;
		private static readonly long ABITS = RBITS | WBIT;
		private static readonly long SBITS = ~RBITS; // note overlap with ABITS

		// Initial value for lock state; avoid failure value zero
		private static readonly long ORIGIN = WBIT << 1;

		// Special value from cancelled acquire methods so caller can throw IE
		private const long INTERRUPTED = 1L;

		// Values for node status; order matters
		private const int WAITING = -1;
		private const int CANCELLED = 1;

		// Modes for nodes (int not boolean to allow arithmetic)
		private const int RMODE = 0;
		private const int WMODE = 1;

		/// <summary>
		/// Wait nodes </summary>
		internal sealed class WNode
		{
			internal volatile WNode Prev;
			internal volatile WNode Next;
			internal volatile WNode Cowait; // list of linked readers
			internal volatile Thread Thread; // non-null while possibly parked
			internal volatile int Status; // 0, WAITING, or CANCELLED
			internal readonly int Mode; // RMODE or WMODE
			internal WNode(int m, WNode p)
			{
				Mode = m;
				Prev = p;
			}
		}

		/// <summary>
		/// Head of CLH queue </summary>
		[NonSerialized]
		private volatile WNode Whead;
		/// <summary>
		/// Tail (last) of CLH queue </summary>
		[NonSerialized]
		private volatile WNode Wtail;

		// views
		[NonSerialized]
		internal ReadLockView ReadLockView;
		[NonSerialized]
		internal WriteLockView WriteLockView;
		[NonSerialized]
		internal ReadWriteLockView ReadWriteLockView;

		/// <summary>
		/// Lock sequence/state </summary>
		[NonSerialized]
		private volatile long State;
		/// <summary>
		/// extra reader count when state read count saturated </summary>
		[NonSerialized]
		private int ReaderOverflow;

		/// <summary>
		/// Creates a new lock, initially in unlocked state.
		/// </summary>
		public StampedLock()
		{
			State = ORIGIN;
		}

		/// <summary>
		/// Exclusively acquires the lock, blocking if necessary
		/// until available.
		/// </summary>
		/// <returns> a stamp that can be used to unlock or convert mode </returns>
		public virtual long WriteLock()
		{
			long s, next; // bypass acquireWrite in fully unlocked case only
			return ((((s = State) & ABITS) == 0L && U.compareAndSwapLong(this, STATE, s, next = s + WBIT)) ? next : AcquireWrite(false, 0L));
		}

		/// <summary>
		/// Exclusively acquires the lock if it is immediately available.
		/// </summary>
		/// <returns> a stamp that can be used to unlock or convert mode,
		/// or zero if the lock is not available </returns>
		public virtual long TryWriteLock()
		{
			long s, next;
			return ((((s = State) & ABITS) == 0L && U.compareAndSwapLong(this, STATE, s, next = s + WBIT)) ? next : 0L);
		}

		/// <summary>
		/// Exclusively acquires the lock if it is available within the
		/// given time and the current thread has not been interrupted.
		/// Behavior under timeout and interruption matches that specified
		/// for method <seealso cref="Lock#tryLock(long,TimeUnit)"/>.
		/// </summary>
		/// <param name="time"> the maximum time to wait for the lock </param>
		/// <param name="unit"> the time unit of the {@code time} argument </param>
		/// <returns> a stamp that can be used to unlock or convert mode,
		/// or zero if the lock is not available </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		/// before acquiring the lock </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long tryWriteLock(long time, java.util.concurrent.TimeUnit unit) throws InterruptedException
		public virtual long TryWriteLock(long time, TimeUnit unit)
		{
			long nanos = unit.ToNanos(time);
			if (!Thread.Interrupted())
			{
				long next, deadline;
				if ((next = TryWriteLock()) != 0L)
				{
					return next;
				}
				if (nanos <= 0L)
				{
					return 0L;
				}
				if ((deadline = System.nanoTime() + nanos) == 0L)
				{
					deadline = 1L;
				}
				if ((next = AcquireWrite(true, deadline)) != INTERRUPTED)
				{
					return next;
				}
			}
			throw new InterruptedException();
		}

		/// <summary>
		/// Exclusively acquires the lock, blocking if necessary
		/// until available or the current thread is interrupted.
		/// Behavior under interruption matches that specified
		/// for method <seealso cref="Lock#lockInterruptibly()"/>.
		/// </summary>
		/// <returns> a stamp that can be used to unlock or convert mode </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		/// before acquiring the lock </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long writeLockInterruptibly() throws InterruptedException
		public virtual long WriteLockInterruptibly()
		{
			long next;
			if (!Thread.Interrupted() && (next = AcquireWrite(true, 0L)) != INTERRUPTED)
			{
				return next;
			}
			throw new InterruptedException();
		}

		/// <summary>
		/// Non-exclusively acquires the lock, blocking if necessary
		/// until available.
		/// </summary>
		/// <returns> a stamp that can be used to unlock or convert mode </returns>
		public virtual long ReadLock()
		{
			long s = State, next ; // bypass acquireRead on common uncontended case
			return ((Whead == Wtail && (s & ABITS) < RFULL && U.compareAndSwapLong(this, STATE, s, next = s + RUNIT)) ? next : AcquireRead(false, 0L));
		}

		/// <summary>
		/// Non-exclusively acquires the lock if it is immediately available.
		/// </summary>
		/// <returns> a stamp that can be used to unlock or convert mode,
		/// or zero if the lock is not available </returns>
		public virtual long TryReadLock()
		{
			for (;;)
			{
				long s, m, next;
				if ((m = (s = State) & ABITS) == WBIT)
				{
					return 0L;
				}
				else if (m < RFULL)
				{
					if (U.compareAndSwapLong(this, STATE, s, next = s + RUNIT))
					{
						return next;
					}
				}
				else if ((next = TryIncReaderOverflow(s)) != 0L)
				{
					return next;
				}
			}
		}

		/// <summary>
		/// Non-exclusively acquires the lock if it is available within the
		/// given time and the current thread has not been interrupted.
		/// Behavior under timeout and interruption matches that specified
		/// for method <seealso cref="Lock#tryLock(long,TimeUnit)"/>.
		/// </summary>
		/// <param name="time"> the maximum time to wait for the lock </param>
		/// <param name="unit"> the time unit of the {@code time} argument </param>
		/// <returns> a stamp that can be used to unlock or convert mode,
		/// or zero if the lock is not available </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		/// before acquiring the lock </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long tryReadLock(long time, java.util.concurrent.TimeUnit unit) throws InterruptedException
		public virtual long TryReadLock(long time, TimeUnit unit)
		{
			long s, m, next, deadline;
			long nanos = unit.ToNanos(time);
			if (!Thread.Interrupted())
			{
				if ((m = (s = State) & ABITS) != WBIT)
				{
					if (m < RFULL)
					{
						if (U.compareAndSwapLong(this, STATE, s, next = s + RUNIT))
						{
							return next;
						}
					}
					else if ((next = TryIncReaderOverflow(s)) != 0L)
					{
						return next;
					}
				}
				if (nanos <= 0L)
				{
					return 0L;
				}
				if ((deadline = System.nanoTime() + nanos) == 0L)
				{
					deadline = 1L;
				}
				if ((next = AcquireRead(true, deadline)) != INTERRUPTED)
				{
					return next;
				}
			}
			throw new InterruptedException();
		}

		/// <summary>
		/// Non-exclusively acquires the lock, blocking if necessary
		/// until available or the current thread is interrupted.
		/// Behavior under interruption matches that specified
		/// for method <seealso cref="Lock#lockInterruptibly()"/>.
		/// </summary>
		/// <returns> a stamp that can be used to unlock or convert mode </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		/// before acquiring the lock </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long readLockInterruptibly() throws InterruptedException
		public virtual long ReadLockInterruptibly()
		{
			long next;
			if (!Thread.Interrupted() && (next = AcquireRead(true, 0L)) != INTERRUPTED)
			{
				return next;
			}
			throw new InterruptedException();
		}

		/// <summary>
		/// Returns a stamp that can later be validated, or zero
		/// if exclusively locked.
		/// </summary>
		/// <returns> a stamp, or zero if exclusively locked </returns>
		public virtual long TryOptimisticRead()
		{
			long s;
			return (((s = State) & WBIT) == 0L) ? (s & SBITS) : 0L;
		}

		/// <summary>
		/// Returns true if the lock has not been exclusively acquired
		/// since issuance of the given stamp. Always returns false if the
		/// stamp is zero. Always returns true if the stamp represents a
		/// currently held lock. Invoking this method with a value not
		/// obtained from <seealso cref="#tryOptimisticRead"/> or a locking method
		/// for this lock has no defined effect or result.
		/// </summary>
		/// <param name="stamp"> a stamp </param>
		/// <returns> {@code true} if the lock has not been exclusively acquired
		/// since issuance of the given stamp; else false </returns>
		public virtual bool Validate(long stamp)
		{
			U.loadFence();
			return (stamp & SBITS) == (State & SBITS);
		}

		/// <summary>
		/// If the lock state matches the given stamp, releases the
		/// exclusive lock.
		/// </summary>
		/// <param name="stamp"> a stamp returned by a write-lock operation </param>
		/// <exception cref="IllegalMonitorStateException"> if the stamp does
		/// not match the current state of this lock </exception>
		public virtual void UnlockWrite(long stamp)
		{
			WNode h;
			if (State != stamp || (stamp & WBIT) == 0L)
			{
				throw new IllegalMonitorStateException();
			}
			State = (stamp += WBIT) == 0L ? ORIGIN : stamp;
			if ((h = Whead) != null && h.Status != 0)
			{
				Release(h);
			}
		}

		/// <summary>
		/// If the lock state matches the given stamp, releases the
		/// non-exclusive lock.
		/// </summary>
		/// <param name="stamp"> a stamp returned by a read-lock operation </param>
		/// <exception cref="IllegalMonitorStateException"> if the stamp does
		/// not match the current state of this lock </exception>
		public virtual void UnlockRead(long stamp)
		{
			long s, m;
			WNode h;
			for (;;)
			{
				if (((s = State) & SBITS) != (stamp & SBITS) || (stamp & ABITS) == 0L || (m = s & ABITS) == 0L || m == WBIT)
				{
					throw new IllegalMonitorStateException();
				}
				if (m < RFULL)
				{
					if (U.compareAndSwapLong(this, STATE, s, s - RUNIT))
					{
						if (m == RUNIT && (h = Whead) != null && h.Status != 0)
						{
							Release(h);
						}
						break;
					}
				}
				else if (TryDecReaderOverflow(s) != 0L)
				{
					break;
				}
			}
		}

		/// <summary>
		/// If the lock state matches the given stamp, releases the
		/// corresponding mode of the lock.
		/// </summary>
		/// <param name="stamp"> a stamp returned by a lock operation </param>
		/// <exception cref="IllegalMonitorStateException"> if the stamp does
		/// not match the current state of this lock </exception>
		public virtual void Unlock(long stamp)
		{
			long a = stamp & ABITS, m , s ;
			WNode h;
			while (((s = State) & SBITS) == (stamp & SBITS))
			{
				if ((m = s & ABITS) == 0L)
				{
					break;
				}
				else if (m == WBIT)
				{
					if (a != m)
					{
						break;
					}
					State = (s += WBIT) == 0L ? ORIGIN : s;
					if ((h = Whead) != null && h.Status != 0)
					{
						Release(h);
					}
					return;
				}
				else if (a == 0L || a >= WBIT)
				{
					break;
				}
				else if (m < RFULL)
				{
					if (U.compareAndSwapLong(this, STATE, s, s - RUNIT))
					{
						if (m == RUNIT && (h = Whead) != null && h.Status != 0)
						{
							Release(h);
						}
						return;
					}
				}
				else if (TryDecReaderOverflow(s) != 0L)
				{
					return;
				}
			}
			throw new IllegalMonitorStateException();
		}

		/// <summary>
		/// If the lock state matches the given stamp, performs one of
		/// the following actions. If the stamp represents holding a write
		/// lock, returns it.  Or, if a read lock, if the write lock is
		/// available, releases the read lock and returns a write stamp.
		/// Or, if an optimistic read, returns a write stamp only if
		/// immediately available. This method returns zero in all other
		/// cases.
		/// </summary>
		/// <param name="stamp"> a stamp </param>
		/// <returns> a valid write stamp, or zero on failure </returns>
		public virtual long TryConvertToWriteLock(long stamp)
		{
			long a = stamp & ABITS, m , s , next ;
			while (((s = State) & SBITS) == (stamp & SBITS))
			{
				if ((m = s & ABITS) == 0L)
				{
					if (a != 0L)
					{
						break;
					}
					if (U.compareAndSwapLong(this, STATE, s, next = s + WBIT))
					{
						return next;
					}
				}
				else if (m == WBIT)
				{
					if (a != m)
					{
						break;
					}
					return stamp;
				}
				else if (m == RUNIT && a != 0L)
				{
					if (U.compareAndSwapLong(this, STATE, s, next = s - RUNIT + WBIT))
					{
						return next;
					}
				}
				else
				{
					break;
				}
			}
			return 0L;
		}

		/// <summary>
		/// If the lock state matches the given stamp, performs one of
		/// the following actions. If the stamp represents holding a write
		/// lock, releases it and obtains a read lock.  Or, if a read lock,
		/// returns it. Or, if an optimistic read, acquires a read lock and
		/// returns a read stamp only if immediately available. This method
		/// returns zero in all other cases.
		/// </summary>
		/// <param name="stamp"> a stamp </param>
		/// <returns> a valid read stamp, or zero on failure </returns>
		public virtual long TryConvertToReadLock(long stamp)
		{
			long a = stamp & ABITS, m , s , next ;
			WNode h;
			while (((s = State) & SBITS) == (stamp & SBITS))
			{
				if ((m = s & ABITS) == 0L)
				{
					if (a != 0L)
					{
						break;
					}
					else if (m < RFULL)
					{
						if (U.compareAndSwapLong(this, STATE, s, next = s + RUNIT))
						{
							return next;
						}
					}
					else if ((next = TryIncReaderOverflow(s)) != 0L)
					{
						return next;
					}
				}
				else if (m == WBIT)
				{
					if (a != m)
					{
						break;
					}
					State = next = s + (WBIT + RUNIT);
					if ((h = Whead) != null && h.Status != 0)
					{
						Release(h);
					}
					return next;
				}
				else if (a != 0L && a < WBIT)
				{
					return stamp;
				}
				else
				{
					break;
				}
			}
			return 0L;
		}

		/// <summary>
		/// If the lock state matches the given stamp then, if the stamp
		/// represents holding a lock, releases it and returns an
		/// observation stamp.  Or, if an optimistic read, returns it if
		/// validated. This method returns zero in all other cases, and so
		/// may be useful as a form of "tryUnlock".
		/// </summary>
		/// <param name="stamp"> a stamp </param>
		/// <returns> a valid optimistic read stamp, or zero on failure </returns>
		public virtual long TryConvertToOptimisticRead(long stamp)
		{
			long a = stamp & ABITS, m , s , next ;
			WNode h;
			U.loadFence();
			for (;;)
			{
				if (((s = State) & SBITS) != (stamp & SBITS))
				{
					break;
				}
				if ((m = s & ABITS) == 0L)
				{
					if (a != 0L)
					{
						break;
					}
					return s;
				}
				else if (m == WBIT)
				{
					if (a != m)
					{
						break;
					}
					State = next = (s += WBIT) == 0L ? ORIGIN : s;
					if ((h = Whead) != null && h.Status != 0)
					{
						Release(h);
					}
					return next;
				}
				else if (a == 0L || a >= WBIT)
				{
					break;
				}
				else if (m < RFULL)
				{
					if (U.compareAndSwapLong(this, STATE, s, next = s - RUNIT))
					{
						if (m == RUNIT && (h = Whead) != null && h.Status != 0)
						{
							Release(h);
						}
						return next & SBITS;
					}
				}
				else if ((next = TryDecReaderOverflow(s)) != 0L)
				{
					return next & SBITS;
				}
			}
			return 0L;
		}

		/// <summary>
		/// Releases the write lock if it is held, without requiring a
		/// stamp value. This method may be useful for recovery after
		/// errors.
		/// </summary>
		/// <returns> {@code true} if the lock was held, else false </returns>
		public virtual bool TryUnlockWrite()
		{
			long s;
			WNode h;
			if (((s = State) & WBIT) != 0L)
			{
				State = (s += WBIT) == 0L ? ORIGIN : s;
				if ((h = Whead) != null && h.Status != 0)
				{
					Release(h);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Releases one hold of the read lock if it is held, without
		/// requiring a stamp value. This method may be useful for recovery
		/// after errors.
		/// </summary>
		/// <returns> {@code true} if the read lock was held, else false </returns>
		public virtual bool TryUnlockRead()
		{
			long s, m;
			WNode h;
			while ((m = (s = State) & ABITS) != 0L && m < WBIT)
			{
				if (m < RFULL)
				{
					if (U.compareAndSwapLong(this, STATE, s, s - RUNIT))
					{
						if (m == RUNIT && (h = Whead) != null && h.Status != 0)
						{
							Release(h);
						}
						return true;
					}
				}
				else if (TryDecReaderOverflow(s) != 0L)
				{
					return true;
				}
			}
			return false;
		}

		// status monitoring methods

		/// <summary>
		/// Returns combined state-held and overflow read count for given
		/// state s.
		/// </summary>
		private int GetReadLockCount(long s)
		{
			long readers;
			if ((readers = s & RBITS) >= RFULL)
			{
				readers = RFULL + ReaderOverflow;
			}
			return (int) readers;
		}

		/// <summary>
		/// Returns {@code true} if the lock is currently held exclusively.
		/// </summary>
		/// <returns> {@code true} if the lock is currently held exclusively </returns>
		public virtual bool WriteLocked
		{
			get
			{
				return (State & WBIT) != 0L;
			}
		}

		/// <summary>
		/// Returns {@code true} if the lock is currently held non-exclusively.
		/// </summary>
		/// <returns> {@code true} if the lock is currently held non-exclusively </returns>
		public virtual bool ReadLocked
		{
			get
			{
				return (State & RBITS) != 0L;
			}
		}

		/// <summary>
		/// Queries the number of read locks held for this lock. This
		/// method is designed for use in monitoring system state, not for
		/// synchronization control. </summary>
		/// <returns> the number of read locks held </returns>
		public virtual int ReadLockCount
		{
			get
			{
				return GetReadLockCount(State);
			}
		}

		/// <summary>
		/// Returns a string identifying this lock, as well as its lock
		/// state.  The state, in brackets, includes the String {@code
		/// "Unlocked"} or the String {@code "Write-locked"} or the String
		/// {@code "Read-locks:"} followed by the current number of
		/// read-locks held.
		/// </summary>
		/// <returns> a string identifying this lock, as well as its lock state </returns>
		public override String ToString()
		{
			long s = State;
			return base.ToString() + ((s & ABITS) == 0L ? "[Unlocked]" : (s & WBIT) != 0L ? "[Write-locked]" : "[Read-locks:" + GetReadLockCount(s) + "]");
		}

		// views

		/// <summary>
		/// Returns a plain <seealso cref="Lock"/> view of this StampedLock in which
		/// the <seealso cref="Lock#lock"/> method is mapped to <seealso cref="#readLock"/>,
		/// and similarly for other methods. The returned Lock does not
		/// support a <seealso cref="Condition"/>; method {@link
		/// Lock#newCondition()} throws {@code
		/// UnsupportedOperationException}.
		/// </summary>
		/// <returns> the lock </returns>
		public virtual Lock AsReadLock()
		{
			ReadLockView v;
			return ((v = ReadLockView) != null ? v : (ReadLockView = new ReadLockView(this)));
		}

		/// <summary>
		/// Returns a plain <seealso cref="Lock"/> view of this StampedLock in which
		/// the <seealso cref="Lock#lock"/> method is mapped to <seealso cref="#writeLock"/>,
		/// and similarly for other methods. The returned Lock does not
		/// support a <seealso cref="Condition"/>; method {@link
		/// Lock#newCondition()} throws {@code
		/// UnsupportedOperationException}.
		/// </summary>
		/// <returns> the lock </returns>
		public virtual Lock AsWriteLock()
		{
			WriteLockView v;
			return ((v = WriteLockView) != null ? v : (WriteLockView = new WriteLockView(this)));
		}

		/// <summary>
		/// Returns a <seealso cref="ReadWriteLock"/> view of this StampedLock in
		/// which the <seealso cref="ReadWriteLock#readLock()"/> method is mapped to
		/// <seealso cref="#asReadLock()"/>, and <seealso cref="ReadWriteLock#writeLock()"/> to
		/// <seealso cref="#asWriteLock()"/>.
		/// </summary>
		/// <returns> the lock </returns>
		public virtual ReadWriteLock AsReadWriteLock()
		{
			ReadWriteLockView v;
			return ((v = ReadWriteLockView) != null ? v : (ReadWriteLockView = new ReadWriteLockView(this)));
		}

		// view classes

		internal sealed class ReadLockView : Lock
		{
			private readonly StampedLock OuterInstance;

			public ReadLockView(StampedLock outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public void @lock()
			{
				outerInstance.ReadLock();
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void lockInterruptibly() throws InterruptedException
			public void LockInterruptibly()
			{
				outerInstance.ReadLockInterruptibly();
			}
			public bool TryLock()
			{
				return outerInstance.TryReadLock() != 0L;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean tryLock(long time, java.util.concurrent.TimeUnit unit) throws InterruptedException
			public bool TryLock(long time, TimeUnit unit)
			{
				return outerInstance.TryReadLock(time, unit) != 0L;
			}
			public void Unlock()
			{
				outerInstance.UnstampedUnlockRead();
			}
			public Condition NewCondition()
			{
				throw new UnsupportedOperationException();
			}
		}

		internal sealed class WriteLockView : Lock
		{
			private readonly StampedLock OuterInstance;

			public WriteLockView(StampedLock outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public void @lock()
			{
				outerInstance.WriteLock();
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void lockInterruptibly() throws InterruptedException
			public void LockInterruptibly()
			{
				outerInstance.WriteLockInterruptibly();
			}
			public bool TryLock()
			{
				return outerInstance.TryWriteLock() != 0L;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean tryLock(long time, java.util.concurrent.TimeUnit unit) throws InterruptedException
			public bool TryLock(long time, TimeUnit unit)
			{
				return outerInstance.TryWriteLock(time, unit) != 0L;
			}
			public void Unlock()
			{
				outerInstance.UnstampedUnlockWrite();
			}
			public Condition NewCondition()
			{
				throw new UnsupportedOperationException();
			}
		}

		internal sealed class ReadWriteLockView : ReadWriteLock
		{
			private readonly StampedLock OuterInstance;

			public ReadWriteLockView(StampedLock outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public Lock ReadLock()
			{
				return outerInstance.AsReadLock();
			}
			public Lock WriteLock()
			{
				return outerInstance.AsWriteLock();
			}
		}

		// Unlock methods without stamp argument checks for view classes.
		// Needed because view-class lock methods throw away stamps.

		internal void UnstampedUnlockWrite()
		{
			WNode h;
			long s;
			if (((s = State) & WBIT) == 0L)
			{
				throw new IllegalMonitorStateException();
			}
			State = (s += WBIT) == 0L ? ORIGIN : s;
			if ((h = Whead) != null && h.Status != 0)
			{
				Release(h);
			}
		}

		internal void UnstampedUnlockRead()
		{
			for (;;)
			{
				long s, m;
				WNode h;
				if ((m = (s = State) & ABITS) == 0L || m >= WBIT)
				{
					throw new IllegalMonitorStateException();
				}
				else if (m < RFULL)
				{
					if (U.compareAndSwapLong(this, STATE, s, s - RUNIT))
					{
						if (m == RUNIT && (h = Whead) != null && h.Status != 0)
						{
							Release(h);
						}
						break;
					}
				}
				else if (TryDecReaderOverflow(s) != 0L)
				{
					break;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject();
			State = ORIGIN; // reset to unlocked state
		}

		// internals

		/// <summary>
		/// Tries to increment readerOverflow by first setting state
		/// access bits value to RBITS, indicating hold of spinlock,
		/// then updating, then releasing.
		/// </summary>
		/// <param name="s"> a reader overflow stamp: (s & ABITS) >= RFULL </param>
		/// <returns> new stamp on success, else zero </returns>
		private long TryIncReaderOverflow(long s)
		{
			// assert (s & ABITS) >= RFULL;
			if ((s & ABITS) == RFULL)
			{
				if (U.compareAndSwapLong(this, STATE, s, s | RBITS))
				{
					++ReaderOverflow;
					State = s;
					return s;
				}
			}
			else if ((LockSupport.NextSecondarySeed() & OVERFLOW_YIELD_RATE) == 0)
			{
				Thread.@yield();
			}
			return 0L;
		}

		/// <summary>
		/// Tries to decrement readerOverflow.
		/// </summary>
		/// <param name="s"> a reader overflow stamp: (s & ABITS) >= RFULL </param>
		/// <returns> new stamp on success, else zero </returns>
		private long TryDecReaderOverflow(long s)
		{
			// assert (s & ABITS) >= RFULL;
			if ((s & ABITS) == RFULL)
			{
				if (U.compareAndSwapLong(this, STATE, s, s | RBITS))
				{
					int r;
					long next;
					if ((r = ReaderOverflow) > 0)
					{
						ReaderOverflow = r - 1;
						next = s;
					}
					else
					{
						next = s - RUNIT;
					}
					 State = next;
					 return next;
				}
			}
			else if ((LockSupport.NextSecondarySeed() & OVERFLOW_YIELD_RATE) == 0)
			{
				Thread.@yield();
			}
			return 0L;
		}

		/// <summary>
		/// Wakes up the successor of h (normally whead). This is normally
		/// just h.next, but may require traversal from wtail if next
		/// pointers are lagging. This may fail to wake up an acquiring
		/// thread when one or more have been cancelled, but the cancel
		/// methods themselves provide extra safeguards to ensure liveness.
		/// </summary>
		private void Release(WNode h)
		{
			if (h != null)
			{
				WNode q;
				Thread w;
				U.compareAndSwapInt(h, WSTATUS, WAITING, 0);
				if ((q = h.Next) == null || q.Status == CANCELLED)
				{
					for (WNode t = Wtail; t != null && t != h; t = t.Prev)
					{
						if (t.Status <= 0)
						{
							q = t;
						}
					}
				}
				if (q != null && (w = q.Thread) != null)
				{
					U.unpark(w);
				}
			}
		}

		/// <summary>
		/// See above for explanation.
		/// </summary>
		/// <param name="interruptible"> true if should check interrupts and if so
		/// return INTERRUPTED </param>
		/// <param name="deadline"> if nonzero, the System.nanoTime value to timeout
		/// at (and return zero) </param>
		/// <returns> next state, or INTERRUPTED </returns>
		private long AcquireWrite(bool interruptible, long deadline)
		{
			WNode node = null, p ;
			for (int spins = -1;;) // spin while enqueuing
			{
				long m, s, ns;
				if ((m = (s = State) & ABITS) == 0L)
				{
					if (U.compareAndSwapLong(this, STATE, s, ns = s + WBIT))
					{
						return ns;
					}
				}
				else if (spins < 0)
				{
					spins = (m == WBIT && Wtail == Whead) ? SPINS : 0;
				}
				else if (spins > 0)
				{
					if (LockSupport.NextSecondarySeed() >= 0)
					{
						--spins;
					}
				}
				else if ((p = Wtail) == null) // initialize queue
				{
					WNode hd = new WNode(WMODE, null);
					if (U.compareAndSwapObject(this, WHEAD, null, hd))
					{
						Wtail = hd;
					}
				}
				else if (node == null)
				{
					node = new WNode(WMODE, p);
				}
				else if (node.Prev != p)
				{
					node.Prev = p;
				}
				else if (U.compareAndSwapObject(this, WTAIL, p, node))
				{
					p.Next = node;
					break;
				}
			}

			for (int spins = -1;;)
			{
				WNode h, np, pp;
				int ps;
				if ((h = Whead) == p)
				{
					if (spins < 0)
					{
						spins = HEAD_SPINS;
					}
					else if (spins < MAX_HEAD_SPINS)
					{
						spins <<= 1;
					}
					for (int k = spins;;) // spin at head
					{
						long s, ns;
						if (((s = State) & ABITS) == 0L)
						{
							if (U.compareAndSwapLong(this, STATE, s, ns = s + WBIT))
							{
								Whead = node;
								node.Prev = null;
								return ns;
							}
						}
						else if (LockSupport.NextSecondarySeed() >= 0 && --k <= 0)
						{
							break;
						}
					}
				}
				else if (h != null) // help release stale waiters
				{
					WNode c;
					Thread w;
					while ((c = h.Cowait) != null)
					{
						if (U.compareAndSwapObject(h, WCOWAIT, c, c.Cowait) && (w = c.Thread) != null)
						{
							U.unpark(w);
						}
					}
				}
				if (Whead == h)
				{
					if ((np = node.Prev) != p)
					{
						if (np != null)
						{
							(p = np).next = node; // stale
						}
					}
					else if ((ps = p.Status) == 0)
					{
						U.compareAndSwapInt(p, WSTATUS, 0, WAITING);
					}
					else if (ps == CANCELLED)
					{
						if ((pp = p.Prev) != null)
						{
							node.Prev = pp;
							pp.Next = node;
						}
					}
					else
					{
						long time; // 0 argument to park means no timeout
						if (deadline == 0L)
						{
							time = 0L;
						}
						else if ((time = deadline - System.nanoTime()) <= 0L)
						{
							return CancelWaiter(node, node, false);
						}
						Thread wt = Thread.CurrentThread;
						U.putObject(wt, PARKBLOCKER, this);
						node.Thread = wt;
						if (p.Status < 0 && (p != h || (State & ABITS) != 0L) && Whead == h && node.Prev == p)
						{
							U.park(false, time); // emulate LockSupport.park
						}
						node.Thread = null;
						U.putObject(wt, PARKBLOCKER, null);
						if (interruptible && Thread.Interrupted())
						{
							return CancelWaiter(node, node, true);
						}
					}
				}
			}
		}

		/// <summary>
		/// See above for explanation.
		/// </summary>
		/// <param name="interruptible"> true if should check interrupts and if so
		/// return INTERRUPTED </param>
		/// <param name="deadline"> if nonzero, the System.nanoTime value to timeout
		/// at (and return zero) </param>
		/// <returns> next state, or INTERRUPTED </returns>
		private long AcquireRead(bool interruptible, long deadline)
		{
			WNode node = null, p ;
			for (int spins = -1;;)
			{
				WNode h;
				if ((h = Whead) == (p = Wtail))
				{
					for (long m, s, ns;;)
					{
						if ((m = (s = State) & ABITS) < RFULL ? U.compareAndSwapLong(this, STATE, s, ns = s + RUNIT) : (m < WBIT && (ns = TryIncReaderOverflow(s)) != 0L))
						{
							return ns;
						}
						else if (m >= WBIT)
						{
							if (spins > 0)
							{
								if (LockSupport.NextSecondarySeed() >= 0)
								{
									--spins;
								}
							}
							else
							{
								if (spins == 0)
								{
									WNode nh = Whead, np = Wtail;
									if ((nh == h && np == p) || (h = nh) != (p = np))
									{
										break;
									}
								}
								spins = SPINS;
							}
						}
					}
				}
				if (p == null) // initialize queue
				{
					WNode hd = new WNode(WMODE, null);
					if (U.compareAndSwapObject(this, WHEAD, null, hd))
					{
						Wtail = hd;
					}
				}
				else if (node == null)
				{
					node = new WNode(RMODE, p);
				}
				else if (h == p || p.Mode != RMODE)
				{
					if (node.Prev != p)
					{
						node.Prev = p;
					}
					else if (U.compareAndSwapObject(this, WTAIL, p, node))
					{
						p.Next = node;
						break;
					}
				}
				else if (!U.compareAndSwapObject(p, WCOWAIT, node.Cowait = p.Cowait, node))
				{
					node.Cowait = null;
				}
				else
				{
					for (;;)
					{
						WNode pp, c;
						Thread w;
						if ((h = Whead) != null && (c = h.Cowait) != null && U.compareAndSwapObject(h, WCOWAIT, c, c.Cowait) && (w = c.Thread) != null) // help release
						{
							U.unpark(w);
						}
						if (h == (pp = p.Prev) || h == p || pp == null)
						{
							long m, s, ns;
							do
							{
								if ((m = (s = State) & ABITS) < RFULL ? U.compareAndSwapLong(this, STATE, s, ns = s + RUNIT) : (m < WBIT && (ns = TryIncReaderOverflow(s)) != 0L))
								{
									return ns;
								}
							} while (m < WBIT);
						}
						if (Whead == h && p.Prev == pp)
						{
							long time;
							if (pp == null || h == p || p.Status > 0)
							{
								node = null; // throw away
								break;
							}
							if (deadline == 0L)
							{
								time = 0L;
							}
							else if ((time = deadline - System.nanoTime()) <= 0L)
							{
								return CancelWaiter(node, p, false);
							}
							Thread wt = Thread.CurrentThread;
							U.putObject(wt, PARKBLOCKER, this);
							node.Thread = wt;
							if ((h != pp || (State & ABITS) == WBIT) && Whead == h && p.Prev == pp)
							{
								U.park(false, time);
							}
							node.Thread = null;
							U.putObject(wt, PARKBLOCKER, null);
							if (interruptible && Thread.Interrupted())
							{
								return CancelWaiter(node, p, true);
							}
						}
					}
				}
			}

			for (int spins = -1;;)
			{
				WNode h, np, pp;
				int ps;
				if ((h = Whead) == p)
				{
					if (spins < 0)
					{
						spins = HEAD_SPINS;
					}
					else if (spins < MAX_HEAD_SPINS)
					{
						spins <<= 1;
					}
					for (int k = spins;;) // spin at head
					{
						long m, s, ns;
						if ((m = (s = State) & ABITS) < RFULL ? U.compareAndSwapLong(this, STATE, s, ns = s + RUNIT) : (m < WBIT && (ns = TryIncReaderOverflow(s)) != 0L))
						{
							WNode c;
							Thread w;
							Whead = node;
							node.Prev = null;
							while ((c = node.Cowait) != null)
							{
								if (U.compareAndSwapObject(node, WCOWAIT, c, c.Cowait) && (w = c.Thread) != null)
								{
									U.unpark(w);
								}
							}
							return ns;
						}
						else if (m >= WBIT && LockSupport.NextSecondarySeed() >= 0 && --k <= 0)
						{
							break;
						}
					}
				}
				else if (h != null)
				{
					WNode c;
					Thread w;
					while ((c = h.Cowait) != null)
					{
						if (U.compareAndSwapObject(h, WCOWAIT, c, c.Cowait) && (w = c.Thread) != null)
						{
							U.unpark(w);
						}
					}
				}
				if (Whead == h)
				{
					if ((np = node.Prev) != p)
					{
						if (np != null)
						{
							(p = np).next = node; // stale
						}
					}
					else if ((ps = p.Status) == 0)
					{
						U.compareAndSwapInt(p, WSTATUS, 0, WAITING);
					}
					else if (ps == CANCELLED)
					{
						if ((pp = p.Prev) != null)
						{
							node.Prev = pp;
							pp.Next = node;
						}
					}
					else
					{
						long time;
						if (deadline == 0L)
						{
							time = 0L;
						}
						else if ((time = deadline - System.nanoTime()) <= 0L)
						{
							return CancelWaiter(node, node, false);
						}
						Thread wt = Thread.CurrentThread;
						U.putObject(wt, PARKBLOCKER, this);
						node.Thread = wt;
						if (p.Status < 0 && (p != h || (State & ABITS) == WBIT) && Whead == h && node.Prev == p)
						{
							U.park(false, time);
						}
						node.Thread = null;
						U.putObject(wt, PARKBLOCKER, null);
						if (interruptible && Thread.Interrupted())
						{
							return CancelWaiter(node, node, true);
						}
					}
				}
			}
		}

		/// <summary>
		/// If node non-null, forces cancel status and unsplices it from
		/// queue if possible and wakes up any cowaiters (of the node, or
		/// group, as applicable), and in any case helps release current
		/// first waiter if lock is free. (Calling with null arguments
		/// serves as a conditional form of release, which is not currently
		/// needed but may be needed under possible future cancellation
		/// policies). This is a variant of cancellation methods in
		/// AbstractQueuedSynchronizer (see its detailed explanation in AQS
		/// internal documentation).
		/// </summary>
		/// <param name="node"> if nonnull, the waiter </param>
		/// <param name="group"> either node or the group node is cowaiting with </param>
		/// <param name="interrupted"> if already interrupted </param>
		/// <returns> INTERRUPTED if interrupted or Thread.interrupted, else zero </returns>
		private long CancelWaiter(WNode node, WNode group, bool interrupted)
		{
			if (node != null && group != null)
			{
				Thread w;
				node.Status = CANCELLED;
				// unsplice cancelled nodes from group
				for (WNode p = group, q; (q = p.Cowait) != null;)
				{
					if (q.status == CANCELLED)
					{
						U.compareAndSwapObject(p, WCOWAIT, q, q.cowait);
						p = group; // restart
					}
					else
					{
						p = q;
					}
				}
				if (group == node)
				{
					for (WNode r = group.Cowait; r != null; r = r.Cowait)
					{
						if ((w = r.Thread) != null)
						{
							U.unpark(w); // wake up uncancelled co-waiters
						}
					}
					for (WNode pred = node.Prev; pred != null;) // unsplice
					{
						WNode succ, pp; // find valid successor
						while ((succ = node.Next) == null || succ.Status == CANCELLED)
						{
							WNode q = null; // find successor the slow way
							for (WNode t = Wtail; t != null && t != node; t = t.Prev)
							{
								if (t.Status != CANCELLED)
								{
									q = t; // don't link if succ cancelled
								}
							}
							if (succ == q || U.compareAndSwapObject(node, WNEXT, succ, succ = q)) // ensure accurate successor
							{
								if (succ == null && node == Wtail)
								{
									U.compareAndSwapObject(this, WTAIL, node, pred);
								}
								break;
							}
						}
						if (pred.next == node) // unsplice pred link
						{
							U.compareAndSwapObject(pred, WNEXT, node, succ);
						}
						if (succ != null && (w = succ.Thread) != null)
						{
							succ.Thread = null;
							U.unpark(w); // wake up succ to observe new pred
						}
						if (pred.status != CANCELLED || (pp = pred.prev) == null)
						{
							break;
						}
						node.Prev = pp; // repeat if new pred wrong/cancelled
						U.compareAndSwapObject(pp, WNEXT, pred, succ);
						pred = pp;
					}
				}
			}
			WNode h; // Possibly release first waiter
			while ((h = Whead) != null)
			{
				long s; // similar to release() but check eligibility
				WNode q;
				if ((q = h.Next) == null || q.Status == CANCELLED)
				{
					for (WNode t = Wtail; t != null && t != h; t = t.Prev)
					{
						if (t.Status <= 0)
						{
							q = t;
						}
					}
				}
				if (h == Whead)
				{
					if (q != null && h.Status == 0 && ((s = State) & ABITS) != WBIT && (s == 0L || q.Mode == RMODE)) // waiter is eligible
					{
						Release(h);
					}
					break;
				}
			}
			return (interrupted || Thread.Interrupted()) ? INTERRUPTED : 0L;
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe U;
		private static readonly long STATE;
		private static readonly long WHEAD;
		private static readonly long WTAIL;
		private static readonly long WNEXT;
		private static readonly long WSTATUS;
		private static readonly long WCOWAIT;
		private static readonly long PARKBLOCKER;

		static StampedLock()
		{
			try
			{
				U = sun.misc.Unsafe.Unsafe;
				Class k = typeof(StampedLock);
				Class wk = typeof(WNode);
				STATE = U.objectFieldOffset(k.GetDeclaredField("state"));
				WHEAD = U.objectFieldOffset(k.GetDeclaredField("whead"));
				WTAIL = U.objectFieldOffset(k.GetDeclaredField("wtail"));
				WSTATUS = U.objectFieldOffset(wk.GetDeclaredField("status"));
				WNEXT = U.objectFieldOffset(wk.GetDeclaredField("next"));
				WCOWAIT = U.objectFieldOffset(wk.GetDeclaredField("cowait"));
				Class tk = typeof(Thread);
				PARKBLOCKER = U.objectFieldOffset(tk.GetDeclaredField("parkBlocker"));

			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}