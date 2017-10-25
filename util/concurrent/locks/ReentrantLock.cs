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
	/// A reentrant mutual exclusion <seealso cref="Lock"/> with the same basic
	/// behavior and semantics as the implicit monitor lock accessed using
	/// {@code synchronized} methods and statements, but with extended
	/// capabilities.
	/// 
	/// <para>A {@code ReentrantLock} is <em>owned</em> by the thread last
	/// successfully locking, but not yet unlocking it. A thread invoking
	/// {@code lock} will return, successfully acquiring the lock, when
	/// the lock is not owned by another thread. The method will return
	/// immediately if the current thread already owns the lock. This can
	/// be checked using methods <seealso cref="#isHeldByCurrentThread"/>, and {@link
	/// #getHoldCount}.
	/// 
	/// </para>
	/// <para>The constructor for this class accepts an optional
	/// <em>fairness</em> parameter.  When set {@code true}, under
	/// contention, locks favor granting access to the longest-waiting
	/// thread.  Otherwise this lock does not guarantee any particular
	/// access order.  Programs using fair locks accessed by many threads
	/// may display lower overall throughput (i.e., are slower; often much
	/// slower) than those using the default setting, but have smaller
	/// variances in times to obtain locks and guarantee lack of
	/// starvation. Note however, that fairness of locks does not guarantee
	/// fairness of thread scheduling. Thus, one of many threads using a
	/// fair lock may obtain it multiple times in succession while other
	/// active threads are not progressing and not currently holding the
	/// lock.
	/// Also note that the untimed <seealso cref="#tryLock()"/> method does not
	/// honor the fairness setting. It will succeed if the lock
	/// is available even if other threads are waiting.
	/// 
	/// </para>
	/// <para>It is recommended practice to <em>always</em> immediately
	/// follow a call to {@code lock} with a {@code try} block, most
	/// typically in a before/after construction such as:
	/// 
	///  <pre> {@code
	/// class X {
	///   private final ReentrantLock lock = new ReentrantLock();
	///   // ...
	/// 
	///   public void m() {
	///     lock.lock();  // block until condition holds
	///     try {
	///       // ... method body
	///     } finally {
	///       lock.unlock()
	///     }
	///   }
	/// }}</pre>
	/// 
	/// </para>
	/// <para>In addition to implementing the <seealso cref="Lock"/> interface, this
	/// class defines a number of {@code public} and {@code protected}
	/// methods for inspecting the state of the lock.  Some of these
	/// methods are only useful for instrumentation and monitoring.
	/// 
	/// </para>
	/// <para>Serialization of this class behaves in the same way as built-in
	/// locks: a deserialized lock is in the unlocked state, regardless of
	/// its state when serialized.
	/// 
	/// </para>
	/// <para>This lock supports a maximum of 2147483647 recursive locks by
	/// the same thread. Attempts to exceed this limit result in
	/// <seealso cref="Error"/> throws from locking methods.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	[Serializable]
	public class ReentrantLock : Lock
	{
		private const long SerialVersionUID = 7373984872572414699L;
		/// <summary>
		/// Synchronizer providing all implementation mechanics </summary>
		private readonly Sync Sync;

		/// <summary>
		/// Base of synchronization control for this lock. Subclassed
		/// into fair and nonfair versions below. Uses AQS state to
		/// represent the number of holds on the lock.
		/// </summary>
		internal abstract class Sync : AbstractQueuedSynchronizer
		{
			internal const long SerialVersionUID = -5179523762034025860L;

			/// <summary>
			/// Performs <seealso cref="Lock#lock"/>. The main reason for subclassing
			/// is to allow fast path for nonfair version.
			/// </summary>
			internal abstract void @lock();

			/// <summary>
			/// Performs non-fair tryLock.  tryAcquire is implemented in
			/// subclasses, but both need nonfair try for trylock method.
			/// </summary>
			internal bool NonfairTryAcquire(int acquires)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Thread current = Thread.currentThread();
				Thread current = Thread.CurrentThread;
				int c = State;
				if (c == 0)
				{
					if (CompareAndSetState(0, acquires))
					{
						ExclusiveOwnerThread = current;
						return true;
					}
				}
				else if (current == ExclusiveOwnerThread)
				{
					int nextc = c + acquires;
					if (nextc < 0) // overflow
					{
						throw new Error("Maximum lock count exceeded");
					}
					State = nextc;
					return true;
				}
				return false;
			}

			protected internal sealed override bool TryRelease(int releases)
			{
				int c = State - releases;
				if (Thread.CurrentThread != ExclusiveOwnerThread)
				{
					throw new IllegalMonitorStateException();
				}
				bool free = false;
				if (c == 0)
				{
					free = true;
					ExclusiveOwnerThread = null;
				}
				State = c;
				return free;
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

			internal ConditionObject NewCondition()
			{
				return new ConditionObject(this);
			}

			// Methods relayed from outer class

			internal Thread Owner
			{
				get
				{
					return State == 0 ? null : ExclusiveOwnerThread;
				}
			}

			internal int HoldCount
			{
				get
				{
					return HeldExclusively ? State : 0;
				}
			}

			internal bool Locked
			{
				get
				{
					return State != 0;
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
				State = 0; // reset to unlocked state
			}
		}

		/// <summary>
		/// Sync object for non-fair locks
		/// </summary>
		internal sealed class NonfairSync : Sync
		{
			internal const long SerialVersionUID = 7316153563782823691L;

			/// <summary>
			/// Performs lock.  Try immediate barge, backing up to normal
			/// acquire on failure.
			/// </summary>
			internal override void @lock()
			{
				if (CompareAndSetState(0, 1))
				{
					ExclusiveOwnerThread = Thread.CurrentThread;
				}
				else
				{
					Acquire(1);
				}
			}

			protected internal override bool TryAcquire(int acquires)
			{
				return NonfairTryAcquire(acquires);
			}
		}

		/// <summary>
		/// Sync object for fair locks
		/// </summary>
		internal sealed class FairSync : Sync
		{
			internal const long SerialVersionUID = -3000897897090466540L;

			internal override void @lock()
			{
				Acquire(1);
			}

			/// <summary>
			/// Fair version of tryAcquire.  Don't grant access unless
			/// recursive call or no waiters or is first.
			/// </summary>
			protected internal override bool TryAcquire(int acquires)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Thread current = Thread.currentThread();
				Thread current = Thread.CurrentThread;
				int c = State;
				if (c == 0)
				{
					if (!HasQueuedPredecessors() && CompareAndSetState(0, acquires))
					{
						ExclusiveOwnerThread = current;
						return true;
					}
				}
				else if (current == ExclusiveOwnerThread)
				{
					int nextc = c + acquires;
					if (nextc < 0)
					{
						throw new Error("Maximum lock count exceeded");
					}
					State = nextc;
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Creates an instance of {@code ReentrantLock}.
		/// This is equivalent to using {@code ReentrantLock(false)}.
		/// </summary>
		public ReentrantLock()
		{
			Sync = new NonfairSync();
		}

		/// <summary>
		/// Creates an instance of {@code ReentrantLock} with the
		/// given fairness policy.
		/// </summary>
		/// <param name="fair"> {@code true} if this lock should use a fair ordering policy </param>
		public ReentrantLock(bool fair)
		{
			Sync = fair ? new FairSync() : new NonfairSync();
		}

		/// <summary>
		/// Acquires the lock.
		/// 
		/// <para>Acquires the lock if it is not held by another thread and returns
		/// immediately, setting the lock hold count to one.
		/// 
		/// </para>
		/// <para>If the current thread already holds the lock then the hold
		/// count is incremented by one and the method returns immediately.
		/// 
		/// </para>
		/// <para>If the lock is held by another thread then the
		/// current thread becomes disabled for thread scheduling
		/// purposes and lies dormant until the lock has been acquired,
		/// at which time the lock hold count is set to one.
		/// </para>
		/// </summary>
		public virtual void @lock()
		{
			Sync.@lock();
		}

		/// <summary>
		/// Acquires the lock unless the current thread is
		/// <seealso cref="Thread#interrupt interrupted"/>.
		/// 
		/// <para>Acquires the lock if it is not held by another thread and returns
		/// immediately, setting the lock hold count to one.
		/// 
		/// </para>
		/// <para>If the current thread already holds this lock then the hold count
		/// is incremented by one and the method returns immediately.
		/// 
		/// </para>
		/// <para>If the lock is held by another thread then the
		/// current thread becomes disabled for thread scheduling
		/// purposes and lies dormant until one of two things happens:
		/// 
		/// <ul>
		/// 
		/// <li>The lock is acquired by the current thread; or
		/// 
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/> the
		/// current thread.
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>If the lock is acquired by the current thread then the lock hold
		/// count is set to one.
		/// 
		/// </para>
		/// <para>If the current thread:
		/// 
		/// <ul>
		/// 
		/// <li>has its interrupted status set on entry to this method; or
		/// 
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while acquiring
		/// the lock,
		/// 
		/// </ul>
		/// 
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared.
		/// 
		/// </para>
		/// <para>In this implementation, as this method is an explicit
		/// interruption point, preference is given to responding to the
		/// interrupt over normal or reentrant acquisition of the lock.
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
		/// Acquires the lock only if it is not held by another thread at the time
		/// of invocation.
		/// 
		/// <para>Acquires the lock if it is not held by another thread and
		/// returns immediately with the value {@code true}, setting the
		/// lock hold count to one. Even when this lock has been set to use a
		/// fair ordering policy, a call to {@code tryLock()} <em>will</em>
		/// immediately acquire the lock if it is available, whether or not
		/// other threads are currently waiting for the lock.
		/// This &quot;barging&quot; behavior can be useful in certain
		/// circumstances, even though it breaks fairness. If you want to honor
		/// the fairness setting for this lock, then use
		/// <seealso cref="#tryLock(long, TimeUnit) tryLock(0, TimeUnit.SECONDS) "/>
		/// which is almost equivalent (it also detects interruption).
		/// 
		/// </para>
		/// <para>If the current thread already holds this lock then the hold
		/// count is incremented by one and the method returns {@code true}.
		/// 
		/// </para>
		/// <para>If the lock is held by another thread then this method will return
		/// immediately with the value {@code false}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if the lock was free and was acquired by the
		///         current thread, or the lock was already held by the current
		///         thread; and {@code false} otherwise </returns>
		public virtual bool TryLock()
		{
			return Sync.NonfairTryAcquire(1);
		}

		/// <summary>
		/// Acquires the lock if it is not held by another thread within the given
		/// waiting time and the current thread has not been
		/// <seealso cref="Thread#interrupt interrupted"/>.
		/// 
		/// <para>Acquires the lock if it is not held by another thread and returns
		/// immediately with the value {@code true}, setting the lock hold count
		/// to one. If this lock has been set to use a fair ordering policy then
		/// an available lock <em>will not</em> be acquired if any other threads
		/// are waiting for the lock. This is in contrast to the <seealso cref="#tryLock()"/>
		/// method. If you want a timed {@code tryLock} that does permit barging on
		/// a fair lock then combine the timed and un-timed forms together:
		/// 
		///  <pre> {@code
		/// if (lock.tryLock() ||
		///     lock.tryLock(timeout, unit)) {
		///   ...
		/// }}</pre>
		/// 
		/// </para>
		/// <para>If the current thread
		/// already holds this lock then the hold count is incremented by one and
		/// the method returns {@code true}.
		/// 
		/// </para>
		/// <para>If the lock is held by another thread then the
		/// current thread becomes disabled for thread scheduling
		/// purposes and lies dormant until one of three things happens:
		/// 
		/// <ul>
		/// 
		/// <li>The lock is acquired by the current thread; or
		/// 
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		/// the current thread; or
		/// 
		/// <li>The specified waiting time elapses
		/// 
		/// </ul>
		/// 
		/// </para>
		/// <para>If the lock is acquired then the value {@code true} is returned and
		/// the lock hold count is set to one.
		/// 
		/// </para>
		/// <para>If the current thread:
		/// 
		/// <ul>
		/// 
		/// <li>has its interrupted status set on entry to this method; or
		/// 
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while
		/// acquiring the lock,
		/// 
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
		/// <para>In this implementation, as this method is an explicit
		/// interruption point, preference is given to responding to the
		/// interrupt over normal or reentrant acquisition of the lock, and
		/// over reporting the elapse of the waiting time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeout"> the time to wait for the lock </param>
		/// <param name="unit"> the time unit of the timeout argument </param>
		/// <returns> {@code true} if the lock was free and was acquired by the
		///         current thread, or the lock was already held by the current
		///         thread; and {@code false} if the waiting time elapsed before
		///         the lock could be acquired </returns>
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
		/// <para>If the current thread is the holder of this lock then the hold
		/// count is decremented.  If the hold count is now zero then the lock
		/// is released.  If the current thread is not the holder of this
		/// lock then <seealso cref="IllegalMonitorStateException"/> is thrown.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IllegalMonitorStateException"> if the current thread does not
		///         hold this lock </exception>
		public virtual void Unlock()
		{
			Sync.Release(1);
		}

		/// <summary>
		/// Returns a <seealso cref="Condition"/> instance for use with this
		/// <seealso cref="Lock"/> instance.
		/// 
		/// <para>The returned <seealso cref="Condition"/> instance supports the same
		/// usages as do the <seealso cref="Object"/> monitor methods ({@link
		/// Object#wait() wait}, <seealso cref="Object#notify notify"/>, and {@link
		/// Object#notifyAll notifyAll}) when used with the built-in
		/// monitor lock.
		/// 
		/// <ul>
		/// 
		/// <li>If this lock is not held when any of the <seealso cref="Condition"/>
		/// <seealso cref="Condition#await() waiting"/> or {@linkplain
		/// Condition#signal signalling} methods are called, then an {@link
		/// IllegalMonitorStateException} is thrown.
		/// 
		/// <li>When the condition <seealso cref="Condition#await() waiting"/>
		/// methods are called the lock is released and, before they
		/// return, the lock is reacquired and the lock hold count restored
		/// to what it was when the method was called.
		/// 
		/// <li>If a thread is <seealso cref="Thread#interrupt interrupted"/>
		/// while waiting then the wait will terminate, an {@link
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
		/// Queries the number of holds on this lock by the current thread.
		/// 
		/// <para>A thread has a hold on a lock for each lock action that is not
		/// matched by an unlock action.
		/// 
		/// </para>
		/// <para>The hold count information is typically only used for testing and
		/// debugging purposes. For example, if a certain section of code should
		/// not be entered with the lock already held then we can assert that
		/// fact:
		/// 
		///  <pre> {@code
		/// class X {
		///   ReentrantLock lock = new ReentrantLock();
		///   // ...
		///   public void m() {
		///     assert lock.getHoldCount() == 0;
		///     lock.lock();
		///     try {
		///       // ... method body
		///     } finally {
		///       lock.unlock();
		///     }
		///   }
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of holds on this lock by the current thread,
		///         or zero if this lock is not held by the current thread </returns>
		public virtual int HoldCount
		{
			get
			{
				return Sync.HoldCount;
			}
		}

		/// <summary>
		/// Queries if this lock is held by the current thread.
		/// 
		/// <para>Analogous to the <seealso cref="Thread#holdsLock(Object)"/> method for
		/// built-in monitor locks, this method is typically used for
		/// debugging and testing. For example, a method that should only be
		/// called while a lock is held can assert that this is the case:
		/// 
		///  <pre> {@code
		/// class X {
		///   ReentrantLock lock = new ReentrantLock();
		///   // ...
		/// 
		///   public void m() {
		///       assert lock.isHeldByCurrentThread();
		///       // ... method body
		///   }
		/// }}</pre>
		/// 
		/// </para>
		/// <para>It can also be used to ensure that a reentrant lock is used
		/// in a non-reentrant manner, for example:
		/// 
		///  <pre> {@code
		/// class X {
		///   ReentrantLock lock = new ReentrantLock();
		///   // ...
		/// 
		///   public void m() {
		///       assert !lock.isHeldByCurrentThread();
		///       lock.lock();
		///       try {
		///           // ... method body
		///       } finally {
		///           lock.unlock();
		///       }
		///   }
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if current thread holds this lock and
		///         {@code false} otherwise </returns>
		public virtual bool HeldByCurrentThread
		{
			get
			{
				return Sync.HeldExclusively;
			}
		}

		/// <summary>
		/// Queries if this lock is held by any thread. This method is
		/// designed for use in monitoring of the system state,
		/// not for synchronization control.
		/// </summary>
		/// <returns> {@code true} if any thread holds this lock and
		///         {@code false} otherwise </returns>
		public virtual bool Locked
		{
			get
			{
				return Sync.Locked;
			}
		}

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
		/// Returns the thread that currently owns this lock, or
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
		/// Queries whether any threads are waiting to acquire this lock. Note that
		/// because cancellations may occur at any time, a {@code true}
		/// return does not guarantee that any other thread will ever
		/// acquire this lock.  This method is designed primarily for use in
		/// monitoring of the system state.
		/// </summary>
		/// <returns> {@code true} if there may be other threads waiting to
		///         acquire the lock </returns>
		public bool HasQueuedThreads()
		{
			return Sync.HasQueuedThreads();
		}

		/// <summary>
		/// Queries whether the given thread is waiting to acquire this
		/// lock. Note that because cancellations may occur at any time, a
		/// {@code true} return does not guarantee that this thread
		/// will ever acquire this lock.  This method is designed primarily for use
		/// in monitoring of the system state.
		/// </summary>
		/// <param name="thread"> the thread </param>
		/// <returns> {@code true} if the given thread is queued waiting for this lock </returns>
		/// <exception cref="NullPointerException"> if the thread is null </exception>
		public bool HasQueuedThread(Thread thread)
		{
			return Sync.IsQueued(thread);
		}

		/// <summary>
		/// Returns an estimate of the number of threads waiting to
		/// acquire this lock.  The value is only an estimate because the number of
		/// threads may change dynamically while this method traverses
		/// internal data structures.  This method is designed for use in
		/// monitoring of the system state, not for synchronization
		/// control.
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
		/// acquire this lock.  Because the actual set of threads may change
		/// dynamically while constructing this result, the returned
		/// collection is only a best-effort estimate.  The elements of the
		/// returned collection are in no particular order.  This method is
		/// designed to facilitate construction of subclasses that provide
		/// more extensive monitoring facilities.
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
		/// associated with this lock. Note that because timeouts and
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
		/// given condition associated with this lock. Note that because
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
		/// waiting on the given condition associated with this lock.
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
		/// The state, in brackets, includes either the String {@code "Unlocked"}
		/// or the String {@code "Locked by"} followed by the
		/// <seealso cref="Thread#getName name"/> of the owning thread.
		/// </summary>
		/// <returns> a string identifying this lock, as well as its lock state </returns>
		public override String ToString()
		{
			Thread o = Sync.Owner;
			return base.ToString() + ((o == null) ? "[Unlocked]" : "[Locked by thread " + o.Name + "]");
		}
	}

}