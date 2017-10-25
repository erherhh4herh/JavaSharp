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

namespace java.util.concurrent.locks
{

	/// <summary>
	/// An implementation of <seealso cref="ReadWriteLock"/> supporting similar
	/// semantics to <seealso cref="ReentrantLock"/>.
	/// <para>This class has the following properties:
	/// 
	/// <ul>
	/// <li><b>Acquisition order</b>
	/// 
	/// </para>
	/// <para>This class does not impose a reader or writer preference
	/// ordering for lock access.  However, it does support an optional
	/// <em>fairness</em> policy.
	/// 
	/// <dl>
	/// <dt><b><i>Non-fair mode (default)</i></b>
	/// <dd>When constructed as non-fair (the default), the order of entry
	/// to the read and write lock is unspecified, subject to reentrancy
	/// constraints.  A nonfair lock that is continuously contended may
	/// indefinitely postpone one or more reader or writer threads, but
	/// will normally have higher throughput than a fair lock.
	/// 
	/// <dt><b><i>Fair mode</i></b>
	/// <dd>When constructed as fair, threads contend for entry using an
	/// approximately arrival-order policy. When the currently held lock
	/// is released, either the longest-waiting single writer thread will
	/// be assigned the write lock, or if there is a group of reader threads
	/// waiting longer than all waiting writer threads, that group will be
	/// assigned the read lock.
	/// 
	/// </para>
	/// <para>A thread that tries to acquire a fair read lock (non-reentrantly)
	/// will block if either the write lock is held, or there is a waiting
	/// writer thread. The thread will not acquire the read lock until
	/// after the oldest currently waiting writer thread has acquired and
	/// released the write lock. Of course, if a waiting writer abandons
	/// its wait, leaving one or more reader threads as the longest waiters
	/// in the queue with the write lock free, then those readers will be
	/// assigned the read lock.
	/// 
	/// </para>
	/// <para>A thread that tries to acquire a fair write lock (non-reentrantly)
	/// will block unless both the read lock and write lock are free (which
	/// implies there are no waiting threads).  (Note that the non-blocking
	/// <seealso cref="ReadLock#tryLock()"/> and <seealso cref="WriteLock#tryLock()"/> methods
	/// do not honor this fair setting and will immediately acquire the lock
	/// if it is possible, regardless of waiting threads.)
	/// </para>
	/// <para>
	/// </dl>
	/// 
	/// <li><b>Reentrancy</b>
	/// 
	/// </para>
	/// <para>This lock allows both readers and writers to reacquire read or
	/// write locks in the style of a <seealso cref="ReentrantLock"/>. Non-reentrant
	/// readers are not allowed until all write locks held by the writing
	/// thread have been released.
	/// 
	/// </para>
	/// <para>Additionally, a writer can acquire the read lock, but not
	/// vice-versa.  Among other applications, reentrancy can be useful
	/// when write locks are held during calls or callbacks to methods that
	/// perform reads under read locks.  If a reader tries to acquire the
	/// write lock it will never succeed.
	/// 
	/// <li><b>Lock downgrading</b>
	/// </para>
	/// <para>Reentrancy also allows downgrading from the write lock to a read lock,
	/// by acquiring the write lock, then the read lock and then releasing the
	/// write lock. However, upgrading from a read lock to the write lock is
	/// <b>not</b> possible.
	/// 
	/// <li><b>Interruption of lock acquisition</b>
	/// </para>
	/// <para>The read lock and write lock both support interruption during lock
	/// acquisition.
	/// 
	/// <li><b><seealso cref="Condition"/> support</b>
	/// </para>
	/// <para>The write lock provides a <seealso cref="Condition"/> implementation that
	/// behaves in the same way, with respect to the write lock, as the
	/// <seealso cref="Condition"/> implementation provided by
	/// <seealso cref="ReentrantLock#newCondition"/> does for <seealso cref="ReentrantLock"/>.
	/// This <seealso cref="Condition"/> can, of course, only be used with the write lock.
	/// 
	/// </para>
	/// <para>The read lock does not support a <seealso cref="Condition"/> and
	/// {@code readLock().newCondition()} throws
	/// {@code UnsupportedOperationException}.
	/// 
	/// <li><b>Instrumentation</b>
	/// </para>
	/// <para>This class supports methods to determine whether locks
	/// are held or contended. These methods are designed for monitoring
	/// system state, not for synchronization control.
	/// </ul>
	/// 
	/// </para>
	/// <para>Serialization of this class behaves in the same way as built-in
	/// locks: a deserialized lock is in the unlocked state, regardless of
	/// its state when serialized.
	/// 
	/// </para>
	/// <para><b>Sample usages</b>. Here is a code sketch showing how to perform
	/// lock downgrading after updating a cache (exception handling is
	/// particularly tricky when handling multiple locks in a non-nested
	/// fashion):
	/// 
	/// <pre> {@code
	/// class CachedData {
	///   Object data;
	///   volatile boolean cacheValid;
	///   final ReentrantReadWriteLock rwl = new ReentrantReadWriteLock();
	/// 
	///   void processCachedData() {
	///     rwl.readLock().lock();
	///     if (!cacheValid) {
	///       // Must release read lock before acquiring write lock
	///       rwl.readLock().unlock();
	///       rwl.writeLock().lock();
	///       try {
	///         // Recheck state because another thread might have
	///         // acquired write lock and changed state before we did.
	///         if (!cacheValid) {
	///           data = ...
	///           cacheValid = true;
	///         }
	///         // Downgrade by acquiring read lock before releasing write lock
	///         rwl.readLock().lock();
	///       } finally {
	///         rwl.writeLock().unlock(); // Unlock write, still hold read
	///       }
	///     }
	/// 
	///     try {
	///       use(data);
	///     } finally {
	///       rwl.readLock().unlock();
	///     }
	///   }
	/// }}</pre>
	/// 
	/// ReentrantReadWriteLocks can be used to improve concurrency in some
	/// uses of some kinds of Collections. This is typically worthwhile
	/// only when the collections are expected to be large, accessed by
	/// more reader threads than writer threads, and entail operations with
	/// overhead that outweighs synchronization overhead. For example, here
	/// is a class using a TreeMap that is expected to be large and
	/// concurrently accessed.
	/// 
	///  <pre> {@code
	/// class RWDictionary {
	///   private final Map<String, Data> m = new TreeMap<String, Data>();
	///   private final ReentrantReadWriteLock rwl = new ReentrantReadWriteLock();
	///   private final Lock r = rwl.readLock();
	///   private final Lock w = rwl.writeLock();
	/// 
	///   public Data get(String key) {
	///     r.lock();
	///     try { return m.get(key); }
	///     finally { r.unlock(); }
	///   }
	///   public String[] allKeys() {
	///     r.lock();
	///     try { return m.keySet().toArray(); }
	///     finally { r.unlock(); }
	///   }
	///   public Data put(String key, Data value) {
	///     w.lock();
	///     try { return m.put(key, value); }
	///     finally { w.unlock(); }
	///   }
	///   public void clear() {
	///     w.lock();
	///     try { m.clear(); }
	///     finally { w.unlock(); }
	///   }
	/// }}</pre>
	/// 
	/// <h3>Implementation Notes</h3>
	/// 
	/// </para>
	/// <para>This lock supports a maximum of 65535 recursive write locks
	/// and 65535 read locks. Attempts to exceed these limits result in
	/// <seealso cref="Error"/> throws from locking methods.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	[Serializable]
	public class ReentrantReadWriteLock : ReadWriteLock
	{
		private const long SerialVersionUID = -6992448646407690164L;
		/// <summary>
		/// Inner class providing readlock </summary>
		private readonly ReentrantReadWriteLock.ReadLock ReaderLock;
		/// <summary>
		/// Inner class providing writelock </summary>
		private readonly ReentrantReadWriteLock.WriteLock WriterLock;
		/// <summary>
		/// Performs all synchronization mechanics </summary>
		internal readonly Sync Sync;

		/// <summary>
		/// Creates a new {@code ReentrantReadWriteLock} with
		/// default (nonfair) ordering properties.
		/// </summary>
		public ReentrantReadWriteLock() : this(false)
		{
		}

		/// <summary>
		/// Creates a new {@code ReentrantReadWriteLock} with
		/// the given fairness policy.
		/// </summary>
		/// <param name="fair"> {@code true} if this lock should use a fair ordering policy </param>
		public ReentrantReadWriteLock(bool fair)
		{
			Sync = fair ? new FairSync() : new NonfairSync();
			ReaderLock = new ReadLock(this);
			WriterLock = new WriteLock(this);
		}

		public virtual ReentrantReadWriteLock.WriteLock WriteLock()
		{
			return WriterLock;
		}
		public virtual ReentrantReadWriteLock.ReadLock ReadLock()
		{
			return ReaderLock;
		}

		/// <summary>
		/// Synchronization implementation for ReentrantReadWriteLock.
		/// Subclassed into fair and nonfair versions.
		/// </summary>
		internal abstract class Sync : AbstractQueuedSynchronizer
		{
			internal const long SerialVersionUID = 6317671515068378041L;

			/*
			 * Read vs write count extraction constants and functions.
			 * Lock state is logically divided into two unsigned shorts:
			 * The lower one representing the exclusive (writer) lock hold count,
			 * and the upper the shared (reader) hold count.
			 */

			internal const int SHARED_SHIFT = 16;
			internal static readonly int SHARED_UNIT = (1 << SHARED_SHIFT);
			internal static readonly int MAX_COUNT = (1 << SHARED_SHIFT) - 1;
			internal static readonly int EXCLUSIVE_MASK = (1 << SHARED_SHIFT) - 1;

			/// <summary>
			/// Returns the number of shared holds represented in count </summary>
			internal static int SharedCount(int c)
			{
				return (int)((uint)c >> SHARED_SHIFT);
			}
			/// <summary>
			/// Returns the number of exclusive holds represented in count </summary>
			internal static int ExclusiveCount(int c)
			{
				return c & EXCLUSIVE_MASK;
			}

			/// <summary>
			/// A counter for per-thread read hold counts.
			/// Maintained as a ThreadLocal; cached in cachedHoldCounter
			/// </summary>
			internal sealed class HoldCounter
			{
				internal int Count = 0;
				// Use id, not reference, to avoid garbage retention
				internal readonly long Tid = GetThreadId(Thread.CurrentThread);
			}

			/// <summary>
			/// ThreadLocal subclass. Easiest to explicitly define for sake
			/// of deserialization mechanics.
			/// </summary>
			internal sealed class ThreadLocalHoldCounter : ThreadLocal<HoldCounter>
			{
				public HoldCounter InitialValue()
				{
					return new HoldCounter();
				}
			}

			/// <summary>
			/// The number of reentrant read locks held by current thread.
			/// Initialized only in constructor and readObject.
			/// Removed whenever a thread's read hold count drops to 0.
			/// </summary>
			[NonSerialized]
			internal ThreadLocalHoldCounter ReadHolds;

			/// <summary>
			/// The hold count of the last thread to successfully acquire
			/// readLock. This saves ThreadLocal lookup in the common case
			/// where the next thread to release is the last one to
			/// acquire. This is non-volatile since it is just used
			/// as a heuristic, and would be great for threads to cache.
			/// 
			/// <para>Can outlive the Thread for which it is caching the read
			/// hold count, but avoids garbage retention by not retaining a
			/// reference to the Thread.
			/// 
			/// </para>
			/// <para>Accessed via a benign data race; relies on the memory
			/// model's final field and out-of-thin-air guarantees.
			/// </para>
			/// </summary>
			[NonSerialized]
			internal HoldCounter CachedHoldCounter;

			/// <summary>
			/// firstReader is the first thread to have acquired the read lock.
			/// firstReaderHoldCount is firstReader's hold count.
			/// 
			/// <para>More precisely, firstReader is the unique thread that last
			/// changed the shared count from 0 to 1, and has not released the
			/// read lock since then; null if there is no such thread.
			/// 
			/// </para>
			/// <para>Cannot cause garbage retention unless the thread terminated
			/// without relinquishing its read locks, since tryReleaseShared
			/// sets it to null.
			/// 
			/// </para>
			/// <para>Accessed via a benign data race; relies on the memory
			/// model's out-of-thin-air guarantees for references.
			/// 
			/// </para>
			/// <para>This allows tracking of read holds for uncontended read
			/// locks to be very cheap.
			/// </para>
			/// </summary>
			[NonSerialized]
			internal Thread FirstReader = null;
			[NonSerialized]
			internal int FirstReaderHoldCount;

			internal Sync()
			{
				ReadHolds = new ThreadLocalHoldCounter();
				State = State; // ensures visibility of readHolds
			}

			/*
			 * Acquires and releases use the same code for fair and
			 * nonfair locks, but differ in whether/how they allow barging
			 * when queues are non-empty.
			 */

			/// <summary>
			/// Returns true if the current thread, when trying to acquire
			/// the read lock, and otherwise eligible to do so, should block
			/// because of policy for overtaking other waiting threads.
			/// </summary>
			internal abstract bool ReaderShouldBlock();

			/// <summary>
			/// Returns true if the current thread, when trying to acquire
			/// the write lock, and otherwise eligible to do so, should block
			/// because of policy for overtaking other waiting threads.
			/// </summary>
			internal abstract bool WriterShouldBlock();

			/*
			 * Note that tryRelease and tryAcquire can be called by
			 * Conditions. So it is possible that their arguments contain
			 * both read and write holds that are all released during a
			 * condition wait and re-established in tryAcquire.
			 */

			protected internal sealed override bool TryRelease(int releases)
			{
				if (!HeldExclusively)
				{
					throw new IllegalMonitorStateException();
				}
				int nextc = State - releases;
				bool free = ExclusiveCount(nextc) == 0;
				if (free)
				{
					ExclusiveOwnerThread = null;
				}
				State = nextc;
				return free;
			}

			protected internal sealed override bool TryAcquire(int acquires)
			{
				/*
				 * Walkthrough:
				 * 1. If read count nonzero or write count nonzero
				 *    and owner is a different thread, fail.
				 * 2. If count would saturate, fail. (This can only
				 *    happen if count is already nonzero.)
				 * 3. Otherwise, this thread is eligible for lock if
				 *    it is either a reentrant acquire or
				 *    queue policy allows it. If so, update state
				 *    and set owner.
				 */
				Thread current = Thread.CurrentThread;
				int c = State;
				int w = ExclusiveCount(c);
				if (c != 0)
				{
					// (Note: if c != 0 and w == 0 then shared count != 0)
					if (w == 0 || current != ExclusiveOwnerThread)
					{
						return false;
					}
					if (w + ExclusiveCount(acquires) > MAX_COUNT)
					{
						throw new Error("Maximum lock count exceeded");
					}
					// Reentrant acquire
					State = c + acquires;
					return true;
				}
				if (WriterShouldBlock() || !CompareAndSetState(c, c + acquires))
				{
					return false;
				}
				ExclusiveOwnerThread = current;
				return true;
			}

			protected internal sealed override bool TryReleaseShared(int unused)
			{
				Thread current = Thread.CurrentThread;
				if (FirstReader == current)
				{
					// assert firstReaderHoldCount > 0;
					if (FirstReaderHoldCount == 1)
					{
						FirstReader = null;
					}
					else
					{
						FirstReaderHoldCount--;
					}
				}
				else
				{
					HoldCounter rh = CachedHoldCounter;
					if (rh == null || rh.Tid != GetThreadId(current))
					{
						rh = ReadHolds.Get();
					}
					int count = rh.Count;
					if (count <= 1)
					{
						ReadHolds.Remove();
						if (count <= 0)
						{
							throw UnmatchedUnlockException();
						}
					}
					--rh.Count;
				}
				for (;;)
				{
					int c = State;
					int nextc = c - SHARED_UNIT;
					if (CompareAndSetState(c, nextc))
						// Releasing the read lock has no effect on readers,
						// but it may allow waiting writers to proceed if
						// both read and write locks are now free.
					{
						return nextc == 0;
					}
				}
			}

			internal virtual IllegalMonitorStateException UnmatchedUnlockException()
			{
				return new IllegalMonitorStateException("attempt to unlock read lock, not locked by current thread");
			}

			protected internal sealed override int TryAcquireShared(int unused)
			{
				/*
				 * Walkthrough:
				 * 1. If write lock held by another thread, fail.
				 * 2. Otherwise, this thread is eligible for
				 *    lock wrt state, so ask if it should block
				 *    because of queue policy. If not, try
				 *    to grant by CASing state and updating count.
				 *    Note that step does not check for reentrant
				 *    acquires, which is postponed to full version
				 *    to avoid having to check hold count in
				 *    the more typical non-reentrant case.
				 * 3. If step 2 fails either because thread
				 *    apparently not eligible or CAS fails or count
				 *    saturated, chain to version with full retry loop.
				 */
				Thread current = Thread.CurrentThread;
				int c = State;
				if (ExclusiveCount(c) != 0 && ExclusiveOwnerThread != current)
				{
					return -1;
				}
				int r = SharedCount(c);
				if (!ReaderShouldBlock() && r < MAX_COUNT && CompareAndSetState(c, c + SHARED_UNIT))
				{
					if (r == 0)
					{
						FirstReader = current;
						FirstReaderHoldCount = 1;
					}
					else if (FirstReader == current)
					{
						FirstReaderHoldCount++;
					}
					else
					{
						HoldCounter rh = CachedHoldCounter;
						if (rh == null || rh.Tid != GetThreadId(current))
						{
							CachedHoldCounter = rh = ReadHolds.Get();
						}
						else if (rh.Count == 0)
						{
							ReadHolds.Set(rh);
						}
						rh.Count++;
					}
					return 1;
				}
				return FullTryAcquireShared(current);
			}

			/// <summary>
			/// Full version of acquire for reads, that handles CAS misses
			/// and reentrant reads not dealt with in tryAcquireShared.
			/// </summary>
			internal int FullTryAcquireShared(Thread current)
			{
				/*
				 * This code is in part redundant with that in
				 * tryAcquireShared but is simpler overall by not
				 * complicating tryAcquireShared with interactions between
				 * retries and lazily reading hold counts.
				 */
				HoldCounter rh = null;
				for (;;)
				{
					int c = State;
					if (ExclusiveCount(c) != 0)
					{
						if (ExclusiveOwnerThread != current)
						{
							return -1;
						}
						// else we hold the exclusive lock; blocking here
						// would cause deadlock.
					}
					else if (ReaderShouldBlock())
					{
						// Make sure we're not acquiring read lock reentrantly
						if (FirstReader == current)
						{
							// assert firstReaderHoldCount > 0;
						}
						else
						{
							if (rh == null)
							{
								rh = CachedHoldCounter;
								if (rh == null || rh.Tid != GetThreadId(current))
								{
									rh = ReadHolds.Get();
									if (rh.Count == 0)
									{
										ReadHolds.Remove();
									}
								}
							}
							if (rh.Count == 0)
							{
								return -1;
							}
						}
					}
					if (SharedCount(c) == MAX_COUNT)
					{
						throw new Error("Maximum lock count exceeded");
					}
					if (CompareAndSetState(c, c + SHARED_UNIT))
					{
						if (SharedCount(c) == 0)
						{
							FirstReader = current;
							FirstReaderHoldCount = 1;
						}
						else if (FirstReader == current)
						{
							FirstReaderHoldCount++;
						}
						else
						{
							if (rh == null)
							{
								rh = CachedHoldCounter;
							}
							if (rh == null || rh.Tid != GetThreadId(current))
							{
								rh = ReadHolds.Get();
							}
							else if (rh.Count == 0)
							{
								ReadHolds.Set(rh);
							}
							rh.Count++;
							CachedHoldCounter = rh; // cache for release
						}
						return 1;
					}
				}
			}

			/// <summary>
			/// Performs tryLock for write, enabling barging in both modes.
			/// This is identical in effect to tryAcquire except for lack
			/// of calls to writerShouldBlock.
			/// </summary>
			internal bool TryWriteLock()
			{
				Thread current = Thread.CurrentThread;
				int c = State;
				if (c != 0)
				{
					int w = ExclusiveCount(c);
					if (w == 0 || current != ExclusiveOwnerThread)
					{
						return false;
					}
					if (w == MAX_COUNT)
					{
						throw new Error("Maximum lock count exceeded");
					}
				}
				if (!CompareAndSetState(c, c + 1))
				{
					return false;
				}
				ExclusiveOwnerThread = current;
				return true;
			}

			/// <summary>
			/// Performs tryLock for read, enabling barging in both modes.
			/// This is identical in effect to tryAcquireShared except for
			/// lack of calls to readerShouldBlock.
			/// </summary>
			internal bool TryReadLock()
			{
				Thread current = Thread.CurrentThread;
				for (;;)
				{
					int c = State;
					if (ExclusiveCount(c) != 0 && ExclusiveOwnerThread != current)
					{
						return false;
					}
					int r = SharedCount(c);
					if (r == MAX_COUNT)
					{
						throw new Error("Maximum lock count exceeded");
					}
					if (CompareAndSetState(c, c + SHARED_UNIT))
					{
						if (r == 0)
						{
							FirstReader = current;
							FirstReaderHoldCount = 1;
						}
						else if (FirstReader == current)
						{
							FirstReaderHoldCount++;
						}
						else
						{
							HoldCounter rh = CachedHoldCounter;
							if (rh == null || rh.Tid != GetThreadId(current))
							{
								CachedHoldCounter = rh = ReadHolds.Get();
							}
							else if (rh.Count == 0)
							{
								ReadHolds.Set(rh);
							}
							rh.Count++;
						}
						return true;
					}
				}
			}

			protected internal sealed override bool HeldExclusively
			{
				get
				{
					// While we must in general read state before owner,
					// we don't need to do so to check if current thread is owner
					return ExclusiveOwnerThread == Thread.CurrentThread;
				}
			}

			// Methods relayed to outer class

			internal ConditionObject NewCondition()
			{
				return new ConditionObject(this);
			}

			internal Thread Owner
			{
				get
				{
					// Must read state before owner to ensure memory consistency
					return ((ExclusiveCount(State) == 0) ? null : ExclusiveOwnerThread);
				}
			}

			internal int ReadLockCount
			{
				get
				{
					return SharedCount(State);
				}
			}

			internal bool WriteLocked
			{
				get
				{
					return ExclusiveCount(State) != 0;
				}
			}

			internal int WriteHoldCount
			{
				get
				{
					return HeldExclusively ? ExclusiveCount(State) : 0;
				}
			}

			internal int ReadHoldCount
			{
				get
				{
					if (ReadLockCount == 0)
					{
						return 0;
					}
    
					Thread current = Thread.CurrentThread;
					if (FirstReader == current)
					{
						return FirstReaderHoldCount;
					}
    
					HoldCounter rh = CachedHoldCounter;
					if (rh != null && rh.Tid == GetThreadId(current))
					{
						return rh.Count;
					}
    
					int count = ReadHolds.Get().count;
					if (count == 0)
					{
						ReadHolds.Remove();
					}
					return count;
				}
			}

			/// <summary>
			/// Reconstitutes the instance from a stream (that is, deserializes it).
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
			internal virtual void ReadObject(java.io.ObjectInputStream s)
			{
				s.DefaultReadObject();
				ReadHolds = new ThreadLocalHoldCounter();
				State = 0; // reset to unlocked state
			}

			internal int Count
			{
				get
				{
					return State;
				}
			}
		}

		/// <summary>
		/// Nonfair version of Sync
		/// </summary>
		internal sealed class NonfairSync : Sync
		{
			internal const long SerialVersionUID = -8159625535654395037L;
			internal override bool WriterShouldBlock()
			{
				return false; // writers can always barge
			}
			internal override bool ReaderShouldBlock()
			{
				/* As a heuristic to avoid indefinite writer starvation,
				 * block if the thread that momentarily appears to be head
				 * of queue, if one exists, is a waiting writer.  This is
				 * only a probabilistic effect since a new reader will not
				 * block if there is a waiting writer behind other enabled
				 * readers that have not yet drained from the queue.
				 */
				return ApparentlyFirstQueuedIsExclusive();
			}
		}

		/// <summary>
		/// Fair version of Sync
		/// </summary>
		internal sealed class FairSync : Sync
		{
			internal const long SerialVersionUID = -2274990926593161451L;
			internal override bool WriterShouldBlock()
			{
				return HasQueuedPredecessors();
			}
			internal override bool ReaderShouldBlock()
			{
				return HasQueuedPredecessors();
			}
		}

		/// <summary>
		/// The lock returned by method <seealso cref="ReentrantReadWriteLock#readLock"/>.
		/// </summary>
		[Serializable]
		public class ReadLock : Lock
		{
			internal const long SerialVersionUID = -5992448646407690164L;
			internal readonly Sync Sync;

			/// <summary>
			/// Constructor for use by subclasses
			/// </summary>
			/// <param name="lock"> the outer lock object </param>
			/// <exception cref="NullPointerException"> if the lock is null </exception>
			protected internal ReadLock(ReentrantReadWriteLock @lock)
			{
				Sync = @lock.Sync;
			}

			/// <summary>
			/// Acquires the read lock.
			/// 
			/// <para>Acquires the read lock if the write lock is not held by
			/// another thread and returns immediately.
			/// 
			/// </para>
			/// <para>If the write lock is held by another thread then
			/// the current thread becomes disabled for thread scheduling
			/// purposes and lies dormant until the read lock has been acquired.
			/// </para>
			/// </summary>
			public virtual void @lock()
			{
				Sync.AcquireShared(1);
			}

			/// <summary>
			/// Acquires the read lock unless the current thread is
			/// <seealso cref="Thread#interrupt interrupted"/>.
			/// 
			/// <para>Acquires the read lock if the write lock is not held
			/// by another thread and returns immediately.
			/// 
			/// </para>
			/// <para>If the write lock is held by another thread then the
			/// current thread becomes disabled for thread scheduling
			/// purposes and lies dormant until one of two things happens:
			/// 
			/// <ul>
			/// 
			/// <li>The read lock is acquired by the current thread; or
			/// 
			/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
			/// the current thread.
			/// 
			/// </ul>
			/// 
			/// </para>
			/// <para>If the current thread:
			/// 
			/// <ul>
			/// 
			/// <li>has its interrupted status set on entry to this method; or
			/// 
			/// <li>is <seealso cref="Thread#interrupt interrupted"/> while
			/// acquiring the read lock,
			/// 
			/// </ul>
			/// 
			/// then <seealso cref="InterruptedException"/> is thrown and the current
			/// thread's interrupted status is cleared.
			/// 
			/// </para>
			/// <para>In this implementation, as this method is an explicit
			/// interruption point, preference is given to responding to
			/// the interrupt over normal or reentrant acquisition of the
			/// lock.
			/// 
			/// </para>
			/// </summary>
			/// <exception cref="InterruptedException"> if the current thread is interrupted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void lockInterruptibly() throws InterruptedException
			public virtual void LockInterruptibly()
			{
				Sync.AcquireSharedInterruptibly(1);
			}

			/// <summary>
			/// Acquires the read lock only if the write lock is not held by
			/// another thread at the time of invocation.
			/// 
			/// <para>Acquires the read lock if the write lock is not held by
			/// another thread and returns immediately with the value
			/// {@code true}. Even when this lock has been set to use a
			/// fair ordering policy, a call to {@code tryLock()}
			/// <em>will</em> immediately acquire the read lock if it is
			/// available, whether or not other threads are currently
			/// waiting for the read lock.  This &quot;barging&quot; behavior
			/// can be useful in certain circumstances, even though it
			/// breaks fairness. If you want to honor the fairness setting
			/// for this lock, then use {@link #tryLock(long, TimeUnit)
			/// tryLock(0, TimeUnit.SECONDS) } which is almost equivalent
			/// (it also detects interruption).
			/// 
			/// </para>
			/// <para>If the write lock is held by another thread then
			/// this method will return immediately with the value
			/// {@code false}.
			/// 
			/// </para>
			/// </summary>
			/// <returns> {@code true} if the read lock was acquired </returns>
			public virtual bool TryLock()
			{
				return Sync.TryReadLock();
			}

			/// <summary>
			/// Acquires the read lock if the write lock is not held by
			/// another thread within the given waiting time and the
			/// current thread has not been {@link Thread#interrupt
			/// interrupted}.
			/// 
			/// <para>Acquires the read lock if the write lock is not held by
			/// another thread and returns immediately with the value
			/// {@code true}. If this lock has been set to use a fair
			/// ordering policy then an available lock <em>will not</em> be
			/// acquired if any other threads are waiting for the
			/// lock. This is in contrast to the <seealso cref="#tryLock()"/>
			/// method. If you want a timed {@code tryLock} that does
			/// permit barging on a fair lock then combine the timed and
			/// un-timed forms together:
			/// 
			///  <pre> {@code
			/// if (lock.tryLock() ||
			///     lock.tryLock(timeout, unit)) {
			///   ...
			/// }}</pre>
			/// 
			/// </para>
			/// <para>If the write lock is held by another thread then the
			/// current thread becomes disabled for thread scheduling
			/// purposes and lies dormant until one of three things happens:
			/// 
			/// <ul>
			/// 
			/// <li>The read lock is acquired by the current thread; or
			/// 
			/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
			/// the current thread; or
			/// 
			/// <li>The specified waiting time elapses.
			/// 
			/// </ul>
			/// 
			/// </para>
			/// <para>If the read lock is acquired then the value {@code true} is
			/// returned.
			/// 
			/// </para>
			/// <para>If the current thread:
			/// 
			/// <ul>
			/// 
			/// <li>has its interrupted status set on entry to this method; or
			/// 
			/// <li>is <seealso cref="Thread#interrupt interrupted"/> while
			/// acquiring the read lock,
			/// 
			/// </ul> then <seealso cref="InterruptedException"/> is thrown and the
			/// current thread's interrupted status is cleared.
			/// 
			/// </para>
			/// <para>If the specified waiting time elapses then the value
			/// {@code false} is returned.  If the time is less than or
			/// equal to zero, the method will not wait at all.
			/// 
			/// </para>
			/// <para>In this implementation, as this method is an explicit
			/// interruption point, preference is given to responding to
			/// the interrupt over normal or reentrant acquisition of the
			/// lock, and over reporting the elapse of the waiting time.
			/// 
			/// </para>
			/// </summary>
			/// <param name="timeout"> the time to wait for the read lock </param>
			/// <param name="unit"> the time unit of the timeout argument </param>
			/// <returns> {@code true} if the read lock was acquired </returns>
			/// <exception cref="InterruptedException"> if the current thread is interrupted </exception>
			/// <exception cref="NullPointerException"> if the time unit is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean tryLock(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException
			public virtual bool TryLock(long timeout, TimeUnit unit)
			{
				return Sync.TryAcquireSharedNanos(1, unit.ToNanos(timeout));
			}

			/// <summary>
			/// Attempts to release this lock.
			/// 
			/// <para>If the number of readers is now zero then the lock
			/// is made available for write lock attempts.
			/// </para>
			/// </summary>
			public virtual void Unlock()
			{
				Sync.ReleaseShared(1);
			}

			/// <summary>
			/// Throws {@code UnsupportedOperationException} because
			/// {@code ReadLocks} do not support conditions.
			/// </summary>
			/// <exception cref="UnsupportedOperationException"> always </exception>
			public virtual Condition NewCondition()
			{
				throw new UnsupportedOperationException();
			}

			/// <summary>
			/// Returns a string identifying this lock, as well as its lock state.
			/// The state, in brackets, includes the String {@code "Read locks ="}
			/// followed by the number of held read locks.
			/// </summary>
			/// <returns> a string identifying this lock, as well as its lock state </returns>
			public override String ToString()
			{
				int r = Sync.ReadLockCount;
				return base.ToString() + "[Read locks = " + r + "]";
			}
		}

		/// <summary>
		/// The lock returned by method <seealso cref="ReentrantReadWriteLock#writeLock"/>.
		/// </summary>
		[Serializable]
		public class WriteLock : Lock
		{
			internal const long SerialVersionUID = -4992448646407690164L;
			internal readonly Sync Sync;

			/// <summary>
			/// Constructor for use by subclasses
			/// </summary>
			/// <param name="lock"> the outer lock object </param>
			/// <exception cref="NullPointerException"> if the lock is null </exception>
			protected internal WriteLock(ReentrantReadWriteLock @lock)
			{
				Sync = @lock.Sync;
			}

			/// <summary>
			/// Acquires the write lock.
			/// 
			/// <para>Acquires the write lock if neither the read nor write lock
			/// are held by another thread
			/// and returns immediately, setting the write lock hold count to
			/// one.
			/// 
			/// </para>
			/// <para>If the current thread already holds the write lock then the
			/// hold count is incremented by one and the method returns
			/// immediately.
			/// 
			/// </para>
			/// <para>If the lock is held by another thread then the current
			/// thread becomes disabled for thread scheduling purposes and
			/// lies dormant until the write lock has been acquired, at which
			/// time the write lock hold count is set to one.
			/// </para>
			/// </summary>
			public virtual void @lock()
			{
				Sync.Acquire(1);
			}

			/// <summary>
			/// Acquires the write lock unless the current thread is
			/// <seealso cref="Thread#interrupt interrupted"/>.
			/// 
			/// <para>Acquires the write lock if neither the read nor write lock
			/// are held by another thread
			/// and returns immediately, setting the write lock hold count to
			/// one.
			/// 
			/// </para>
			/// <para>If the current thread already holds this lock then the
			/// hold count is incremented by one and the method returns
			/// immediately.
			/// 
			/// </para>
			/// <para>If the lock is held by another thread then the current
			/// thread becomes disabled for thread scheduling purposes and
			/// lies dormant until one of two things happens:
			/// 
			/// <ul>
			/// 
			/// <li>The write lock is acquired by the current thread; or
			/// 
			/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
			/// the current thread.
			/// 
			/// </ul>
			/// 
			/// </para>
			/// <para>If the write lock is acquired by the current thread then the
			/// lock hold count is set to one.
			/// 
			/// </para>
			/// <para>If the current thread:
			/// 
			/// <ul>
			/// 
			/// <li>has its interrupted status set on entry to this method;
			/// or
			/// 
			/// <li>is <seealso cref="Thread#interrupt interrupted"/> while
			/// acquiring the write lock,
			/// 
			/// </ul>
			/// 
			/// then <seealso cref="InterruptedException"/> is thrown and the current
			/// thread's interrupted status is cleared.
			/// 
			/// </para>
			/// <para>In this implementation, as this method is an explicit
			/// interruption point, preference is given to responding to
			/// the interrupt over normal or reentrant acquisition of the
			/// lock.
			/// 
			/// </para>
			/// </summary>
			/// <exception cref="InterruptedException"> if the current thread is interrupted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void lockInterruptibly() throws InterruptedException
			public virtual void LockInterruptibly()
			{
				Sync.AcquireInterruptibly(1);
			}

			/// <summary>
			/// Acquires the write lock only if it is not held by another thread
			/// at the time of invocation.
			/// 
			/// <para>Acquires the write lock if neither the read nor write lock
			/// are held by another thread
			/// and returns immediately with the value {@code true},
			/// setting the write lock hold count to one. Even when this lock has
			/// been set to use a fair ordering policy, a call to
			/// {@code tryLock()} <em>will</em> immediately acquire the
			/// lock if it is available, whether or not other threads are
			/// currently waiting for the write lock.  This &quot;barging&quot;
			/// behavior can be useful in certain circumstances, even
			/// though it breaks fairness. If you want to honor the
			/// fairness setting for this lock, then use {@link
			/// #tryLock(long, TimeUnit) tryLock(0, TimeUnit.SECONDS) }
			/// which is almost equivalent (it also detects interruption).
			/// 
			/// </para>
			/// <para>If the current thread already holds this lock then the
			/// hold count is incremented by one and the method returns
			/// {@code true}.
			/// 
			/// </para>
			/// <para>If the lock is held by another thread then this method
			/// will return immediately with the value {@code false}.
			/// 
			/// </para>
			/// </summary>
			/// <returns> {@code true} if the lock was free and was acquired
			/// by the current thread, or the write lock was already held
			/// by the current thread; and {@code false} otherwise. </returns>
			public virtual bool TryLock()
			{
				return Sync.TryWriteLock();
			}

			/// <summary>
			/// Acquires the write lock if it is not held by another thread
			/// within the given waiting time and the current thread has
			/// not been <seealso cref="Thread#interrupt interrupted"/>.
			/// 
			/// <para>Acquires the write lock if neither the read nor write lock
			/// are held by another thread
			/// and returns immediately with the value {@code true},
			/// setting the write lock hold count to one. If this lock has been
			/// set to use a fair ordering policy then an available lock
			/// <em>will not</em> be acquired if any other threads are
			/// waiting for the write lock. This is in contrast to the {@link
			/// #tryLock()} method. If you want a timed {@code tryLock}
			/// that does permit barging on a fair lock then combine the
			/// timed and un-timed forms together:
			/// 
			///  <pre> {@code
			/// if (lock.tryLock() ||
			///     lock.tryLock(timeout, unit)) {
			///   ...
			/// }}</pre>
			/// 
			/// </para>
			/// <para>If the current thread already holds this lock then the
			/// hold count is incremented by one and the method returns
			/// {@code true}.
			/// 
			/// </para>
			/// <para>If the lock is held by another thread then the current
			/// thread becomes disabled for thread scheduling purposes and
			/// lies dormant until one of three things happens:
			/// 
			/// <ul>
			/// 
			/// <li>The write lock is acquired by the current thread; or
			/// 
			/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
			/// the current thread; or
			/// 
			/// <li>The specified waiting time elapses
			/// 
			/// </ul>
			/// 
			/// </para>
			/// <para>If the write lock is acquired then the value {@code true} is
			/// returned and the write lock hold count is set to one.
			/// 
			/// </para>
			/// <para>If the current thread:
			/// 
			/// <ul>
			/// 
			/// <li>has its interrupted status set on entry to this method;
			/// or
			/// 
			/// <li>is <seealso cref="Thread#interrupt interrupted"/> while
			/// acquiring the write lock,
			/// 
			/// </ul>
			/// 
			/// then <seealso cref="InterruptedException"/> is thrown and the current
			/// thread's interrupted status is cleared.
			/// 
			/// </para>
			/// <para>If the specified waiting time elapses then the value
			/// {@code false} is returned.  If the time is less than or
			/// equal to zero, the method will not wait at all.
			/// 
			/// </para>
			/// <para>In this implementation, as this method is an explicit
			/// interruption point, preference is given to responding to
			/// the interrupt over normal or reentrant acquisition of the
			/// lock, and over reporting the elapse of the waiting time.
			/// 
			/// </para>
			/// </summary>
			/// <param name="timeout"> the time to wait for the write lock </param>
			/// <param name="unit"> the time unit of the timeout argument
			/// </param>
			/// <returns> {@code true} if the lock was free and was acquired
			/// by the current thread, or the write lock was already held by the
			/// current thread; and {@code false} if the waiting time
			/// elapsed before the lock could be acquired.
			/// </returns>
			/// <exception cref="InterruptedException"> if the current thread is interrupted </exception>
			/// <exception cref="NullPointerException"> if the time unit is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean tryLock(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException
			public virtual bool TryLock(long timeout, TimeUnit unit)
			{
				return Sync.TryAcquireNanos(1, unit.ToNanos(timeout));
			}

			/// <summary>
			/// Attempts to release this lock.
			/// 
			/// <para>If the current thread is the holder of this lock then
			/// the hold count is decremented. If the hold count is now
			/// zero then the lock is released.  If the current thread is
			/// not the holder of this lock then {@link
			/// IllegalMonitorStateException} is thrown.
			/// 
			/// </para>
			/// </summary>
			/// <exception cref="IllegalMonitorStateException"> if the current thread does not
			/// hold this lock </exception>
			public virtual void Unlock()
			{
				Sync.Release(1);
			}

			/// <summary>
			/// Returns a <seealso cref="Condition"/> instance for use with this
			/// <seealso cref="Lock"/> instance.
			/// <para>The returned <seealso cref="Condition"/> instance supports the same
			/// usages as do the <seealso cref="Object"/> monitor methods ({@link
			/// Object#wait() wait}, <seealso cref="Object#notify notify"/>, and {@link
			/// Object#notifyAll notifyAll}) when used with the built-in
			/// monitor lock.
			/// 
			/// <ul>
			/// 
			/// <li>If this write lock is not held when any {@link
			/// Condition} method is called then an {@link
			/// IllegalMonitorStateException} is thrown.  (Read locks are
			/// held independently of write locks, so are not checked or
			/// affected. However it is essentially always an error to
			/// invoke a condition waiting method when the current thread
			/// has also acquired read locks, since other threads that
			/// could unblock it will not be able to acquire the write
			/// lock.)
			/// 
			/// <li>When the condition <seealso cref="Condition#await() waiting"/>
			/// methods are called the write lock is released and, before
			/// they return, the write lock is reacquired and the lock hold
			/// count restored to what it was when the method was called.
			/// 
			/// <li>If a thread is <seealso cref="Thread#interrupt interrupted"/> while
			/// waiting then the wait will terminate, an {@link
			/// InterruptedException} will be thrown, and the thread's
			/// interrupted status will be cleared.
			/// 
			/// <li> Waiting threads are signalled in FIFO order.
			/// 
			/// <li>The ordering of lock reacquisition for threads returning
			/// from waiting methods is the same as for threads initially
			/// acquiring the lock, which is in the default case not specified,
			/// but for <em>fair</em> locks favors those threads that have been
			/// waiting the longest.
			/// 
			/// </ul>
			/// 
			/// </para>
			/// </summary>
			/// <returns> the Condition object </returns>
			public virtual Condition NewCondition()
			{
				return Sync.NewCondition();
			}

			/// <summary>
			/// Returns a string identifying this lock, as well as its lock
			/// state.  The state, in brackets includes either the String
			/// {@code "Unlocked"} or the String {@code "Locked by"}
			/// followed by the <seealso cref="Thread#getName name"/> of the owning thread.
			/// </summary>
			/// <returns> a string identifying this lock, as well as its lock state </returns>
			public override String ToString()
			{
				Thread o = Sync.Owner;
				return base.ToString() + ((o == null) ? "[Unlocked]" : "[Locked by thread " + o.Name + "]");
			}

			/// <summary>
			/// Queries if this write lock is held by the current thread.
			/// Identical in effect to {@link
			/// ReentrantReadWriteLock#isWriteLockedByCurrentThread}.
			/// </summary>
			/// <returns> {@code true} if the current thread holds this lock and
			///         {@code false} otherwise
			/// @since 1.6 </returns>
			public virtual bool HeldByCurrentThread
			{
				get
				{
					return Sync.HeldExclusively;
				}
			}

			/// <summary>
			/// Queries the number of holds on this write lock by the current
			/// thread.  A thread has a hold on a lock for each lock action
			/// that is not matched by an unlock action.  Identical in effect
			/// to <seealso cref="ReentrantReadWriteLock#getWriteHoldCount"/>.
			/// </summary>
			/// <returns> the number of holds on this lock by the current thread,
			///         or zero if this lock is not held by the current thread
			/// @since 1.6 </returns>
			public virtual int HoldCount
			{
				get
				{
					return Sync.WriteHoldCount;
				}
			}
		}

		// Instrumentation and status

		/// <summary>
		/// Returns {@code true} if this lock has fairness set true.
		/// </summary>
		/// <returns> {@code true} if this lock has fairness set true </returns>
		public bool Fair
		{
			get
			{
				return Sync is FairSync;
			}
		}

		/// <summary>
		/// Returns the thread that currently owns the write lock, or
		/// {@code null} if not owned. When this method is called by a
		/// thread that is not the owner, the return value reflects a
		/// best-effort approximation of current lock status. For example,
		/// the owner may be momentarily {@code null} even if there are
		/// threads trying to acquire the lock but have not yet done so.
		/// This method is designed to facilitate construction of
		/// subclasses that provide more extensive lock monitoring
		/// facilities.
		/// </summary>
		/// <returns> the owner, or {@code null} if not owned </returns>
		protected internal virtual Thread Owner
		{
			get
			{
				return Sync.Owner;
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
				return Sync.ReadLockCount;
			}
		}

		/// <summary>
		/// Queries if the write lock is held by any thread. This method is
		/// designed for use in monitoring system state, not for
		/// synchronization control.
		/// </summary>
		/// <returns> {@code true} if any thread holds the write lock and
		///         {@code false} otherwise </returns>
		public virtual bool WriteLocked
		{
			get
			{
				return Sync.WriteLocked;
			}
		}

		/// <summary>
		/// Queries if the write lock is held by the current thread.
		/// </summary>
		/// <returns> {@code true} if the current thread holds the write lock and
		///         {@code false} otherwise </returns>
		public virtual bool WriteLockedByCurrentThread
		{
			get
			{
				return Sync.HeldExclusively;
			}
		}

		/// <summary>
		/// Queries the number of reentrant write holds on this lock by the
		/// current thread.  A writer thread has a hold on a lock for
		/// each lock action that is not matched by an unlock action.
		/// </summary>
		/// <returns> the number of holds on the write lock by the current thread,
		///         or zero if the write lock is not held by the current thread </returns>
		public virtual int WriteHoldCount
		{
			get
			{
				return Sync.WriteHoldCount;
			}
		}

		/// <summary>
		/// Queries the number of reentrant read holds on this lock by the
		/// current thread.  A reader thread has a hold on a lock for
		/// each lock action that is not matched by an unlock action.
		/// </summary>
		/// <returns> the number of holds on the read lock by the current thread,
		///         or zero if the read lock is not held by the current thread
		/// @since 1.6 </returns>
		public virtual int ReadHoldCount
		{
			get
			{
				return Sync.ReadHoldCount;
			}
		}

		/// <summary>
		/// Returns a collection containing threads that may be waiting to
		/// acquire the write lock.  Because the actual set of threads may
		/// change dynamically while constructing this result, the returned
		/// collection is only a best-effort estimate.  The elements of the
		/// returned collection are in no particular order.  This method is
		/// designed to facilitate construction of subclasses that provide
		/// more extensive lock monitoring facilities.
		/// </summary>
		/// <returns> the collection of threads </returns>
		protected internal virtual ICollection<Thread> QueuedWriterThreads
		{
			get
			{
				return Sync.ExclusiveQueuedThreads;
			}
		}

		/// <summary>
		/// Returns a collection containing threads that may be waiting to
		/// acquire the read lock.  Because the actual set of threads may
		/// change dynamically while constructing this result, the returned
		/// collection is only a best-effort estimate.  The elements of the
		/// returned collection are in no particular order.  This method is
		/// designed to facilitate construction of subclasses that provide
		/// more extensive lock monitoring facilities.
		/// </summary>
		/// <returns> the collection of threads </returns>
		protected internal virtual ICollection<Thread> QueuedReaderThreads
		{
			get
			{
				return Sync.SharedQueuedThreads;
			}
		}

		/// <summary>
		/// Queries whether any threads are waiting to acquire the read or
		/// write lock. Note that because cancellations may occur at any
		/// time, a {@code true} return does not guarantee that any other
		/// thread will ever acquire a lock.  This method is designed
		/// primarily for use in monitoring of the system state.
		/// </summary>
		/// <returns> {@code true} if there may be other threads waiting to
		///         acquire the lock </returns>
		public bool HasQueuedThreads()
		{
			return Sync.HasQueuedThreads();
		}

		/// <summary>
		/// Queries whether the given thread is waiting to acquire either
		/// the read or write lock. Note that because cancellations may
		/// occur at any time, a {@code true} return does not guarantee
		/// that this thread will ever acquire a lock.  This method is
		/// designed primarily for use in monitoring of the system state.
		/// </summary>
		/// <param name="thread"> the thread </param>
		/// <returns> {@code true} if the given thread is queued waiting for this lock </returns>
		/// <exception cref="NullPointerException"> if the thread is null </exception>
		public bool HasQueuedThread(Thread thread)
		{
			return Sync.IsQueued(thread);
		}

		/// <summary>
		/// Returns an estimate of the number of threads waiting to acquire
		/// either the read or write lock.  The value is only an estimate
		/// because the number of threads may change dynamically while this
		/// method traverses internal data structures.  This method is
		/// designed for use in monitoring of the system state, not for
		/// synchronization control.
		/// </summary>
		/// <returns> the estimated number of threads waiting for this lock </returns>
		public int QueueLength
		{
			get
			{
				return Sync.QueueLength;
			}
		}

		/// <summary>
		/// Returns a collection containing threads that may be waiting to
		/// acquire either the read or write lock.  Because the actual set
		/// of threads may change dynamically while constructing this
		/// result, the returned collection is only a best-effort estimate.
		/// The elements of the returned collection are in no particular
		/// order.  This method is designed to facilitate construction of
		/// subclasses that provide more extensive monitoring facilities.
		/// </summary>
		/// <returns> the collection of threads </returns>
		protected internal virtual ICollection<Thread> QueuedThreads
		{
			get
			{
				return Sync.QueuedThreads;
			}
		}

		/// <summary>
		/// Queries whether any threads are waiting on the given condition
		/// associated with the write lock. Note that because timeouts and
		/// interrupts may occur at any time, a {@code true} return does
		/// not guarantee that a future {@code signal} will awaken any
		/// threads.  This method is designed primarily for use in
		/// monitoring of the system state.
		/// </summary>
		/// <param name="condition"> the condition </param>
		/// <returns> {@code true} if there are any waiting threads </returns>
		/// <exception cref="IllegalMonitorStateException"> if this lock is not held </exception>
		/// <exception cref="IllegalArgumentException"> if the given condition is
		///         not associated with this lock </exception>
		/// <exception cref="NullPointerException"> if the condition is null </exception>
		public virtual bool HasWaiters(Condition condition)
		{
			if (condition == null)
			{
				throw new NullPointerException();
			}
			if (!(condition is AbstractQueuedSynchronizer.ConditionObject))
			{
				throw new IllegalArgumentException("not owner");
			}
			return Sync.HasWaiters((AbstractQueuedSynchronizer.ConditionObject)condition);
		}

		/// <summary>
		/// Returns an estimate of the number of threads waiting on the
		/// given condition associated with the write lock. Note that because
		/// timeouts and interrupts may occur at any time, the estimate
		/// serves only as an upper bound on the actual number of waiters.
		/// This method is designed for use in monitoring of the system
		/// state, not for synchronization control.
		/// </summary>
		/// <param name="condition"> the condition </param>
		/// <returns> the estimated number of waiting threads </returns>
		/// <exception cref="IllegalMonitorStateException"> if this lock is not held </exception>
		/// <exception cref="IllegalArgumentException"> if the given condition is
		///         not associated with this lock </exception>
		/// <exception cref="NullPointerException"> if the condition is null </exception>
		public virtual int GetWaitQueueLength(Condition condition)
		{
			if (condition == null)
			{
				throw new NullPointerException();
			}
			if (!(condition is AbstractQueuedSynchronizer.ConditionObject))
			{
				throw new IllegalArgumentException("not owner");
			}
			return Sync.GetWaitQueueLength((AbstractQueuedSynchronizer.ConditionObject)condition);
		}

		/// <summary>
		/// Returns a collection containing those threads that may be
		/// waiting on the given condition associated with the write lock.
		/// Because the actual set of threads may change dynamically while
		/// constructing this result, the returned collection is only a
		/// best-effort estimate. The elements of the returned collection
		/// are in no particular order.  This method is designed to
		/// facilitate construction of subclasses that provide more
		/// extensive condition monitoring facilities.
		/// </summary>
		/// <param name="condition"> the condition </param>
		/// <returns> the collection of threads </returns>
		/// <exception cref="IllegalMonitorStateException"> if this lock is not held </exception>
		/// <exception cref="IllegalArgumentException"> if the given condition is
		///         not associated with this lock </exception>
		/// <exception cref="NullPointerException"> if the condition is null </exception>
		protected internal virtual ICollection<Thread> GetWaitingThreads(Condition condition)
		{
			if (condition == null)
			{
				throw new NullPointerException();
			}
			if (!(condition is AbstractQueuedSynchronizer.ConditionObject))
			{
				throw new IllegalArgumentException("not owner");
			}
			return Sync.GetWaitingThreads((AbstractQueuedSynchronizer.ConditionObject)condition);
		}

		/// <summary>
		/// Returns a string identifying this lock, as well as its lock state.
		/// The state, in brackets, includes the String {@code "Write locks ="}
		/// followed by the number of reentrantly held write locks, and the
		/// String {@code "Read locks ="} followed by the number of held
		/// read locks.
		/// </summary>
		/// <returns> a string identifying this lock, as well as its lock state </returns>
		public override String ToString()
		{
			int c = Sync.Count;
			int w = Sync.ExclusiveCount(c);
			int r = Sync.SharedCount(c);

			return base.ToString() + "[Write locks = " + w + ", Read locks = " + r + "]";
		}

		/// <summary>
		/// Returns the thread id for the given thread.  We must access
		/// this directly rather than via method Thread.getId() because
		/// getId() is not final, and has been known to be overridden in
		/// ways that do not preserve unique mappings.
		/// </summary>
		internal static long GetThreadId(Thread thread)
		{
			return UNSAFE.getLongVolatile(thread, TID_OFFSET);
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long TID_OFFSET;
		static ReentrantReadWriteLock()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class tk = typeof(Thread);
				TID_OFFSET = UNSAFE.objectFieldOffset(tk.GetDeclaredField("tid"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}

	}

}