using System;

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
	/// {@code Condition} factors out the {@code Object} monitor
	/// methods (<seealso cref="Object#wait() wait"/>, <seealso cref="Object#notify notify"/>
	/// and <seealso cref="Object#notifyAll notifyAll"/>) into distinct objects to
	/// give the effect of having multiple wait-sets per object, by
	/// combining them with the use of arbitrary <seealso cref="Lock"/> implementations.
	/// Where a {@code Lock} replaces the use of {@code synchronized} methods
	/// and statements, a {@code Condition} replaces the use of the Object
	/// monitor methods.
	/// 
	/// <para>Conditions (also known as <em>condition queues</em> or
	/// <em>condition variables</em>) provide a means for one thread to
	/// suspend execution (to &quot;wait&quot;) until notified by another
	/// thread that some state condition may now be true.  Because access
	/// to this shared state information occurs in different threads, it
	/// must be protected, so a lock of some form is associated with the
	/// condition. The key property that waiting for a condition provides
	/// is that it <em>atomically</em> releases the associated lock and
	/// suspends the current thread, just like {@code Object.wait}.
	/// 
	/// </para>
	/// <para>A {@code Condition} instance is intrinsically bound to a lock.
	/// To obtain a {@code Condition} instance for a particular <seealso cref="Lock"/>
	/// instance use its <seealso cref="Lock#newCondition newCondition()"/> method.
	/// 
	/// </para>
	/// <para>As an example, suppose we have a bounded buffer which supports
	/// {@code put} and {@code take} methods.  If a
	/// {@code take} is attempted on an empty buffer, then the thread will block
	/// until an item becomes available; if a {@code put} is attempted on a
	/// full buffer, then the thread will block until a space becomes available.
	/// We would like to keep waiting {@code put} threads and {@code take}
	/// threads in separate wait-sets so that we can use the optimization of
	/// only notifying a single thread at a time when items or spaces become
	/// available in the buffer. This can be achieved using two
	/// <seealso cref="Condition"/> instances.
	/// <pre>
	/// class BoundedBuffer {
	///   <b>final Lock lock = new ReentrantLock();</b>
	///   final Condition notFull  = <b>lock.newCondition(); </b>
	///   final Condition notEmpty = <b>lock.newCondition(); </b>
	/// 
	///   final Object[] items = new Object[100];
	///   int putptr, takeptr, count;
	/// 
	///   public void put(Object x) throws InterruptedException {
	///     <b>lock.lock();
	///     try {</b>
	///       while (count == items.length)
	///         <b>notFull.await();</b>
	///       items[putptr] = x;
	///       if (++putptr == items.length) putptr = 0;
	///       ++count;
	///       <b>notEmpty.signal();</b>
	///     <b>} finally {
	///       lock.unlock();
	///     }</b>
	///   }
	/// 
	///   public Object take() throws InterruptedException {
	///     <b>lock.lock();
	///     try {</b>
	///       while (count == 0)
	///         <b>notEmpty.await();</b>
	///       Object x = items[takeptr];
	///       if (++takeptr == items.length) takeptr = 0;
	///       --count;
	///       <b>notFull.signal();</b>
	///       return x;
	///     <b>} finally {
	///       lock.unlock();
	///     }</b>
	///   }
	/// }
	/// </pre>
	/// 
	/// (The <seealso cref="java.util.concurrent.ArrayBlockingQueue"/> class provides
	/// this functionality, so there is no reason to implement this
	/// sample usage class.)
	/// 
	/// </para>
	/// <para>A {@code Condition} implementation can provide behavior and semantics
	/// that is
	/// different from that of the {@code Object} monitor methods, such as
	/// guaranteed ordering for notifications, or not requiring a lock to be held
	/// when performing notifications.
	/// If an implementation provides such specialized semantics then the
	/// implementation must document those semantics.
	/// 
	/// </para>
	/// <para>Note that {@code Condition} instances are just normal objects and can
	/// themselves be used as the target in a {@code synchronized} statement,
	/// and can have their own monitor <seealso cref="Object#wait wait"/> and
	/// <seealso cref="Object#notify notification"/> methods invoked.
	/// Acquiring the monitor lock of a {@code Condition} instance, or using its
	/// monitor methods, has no specified relationship with acquiring the
	/// <seealso cref="Lock"/> associated with that {@code Condition} or the use of its
	/// <seealso cref="#await waiting"/> and <seealso cref="#signal signalling"/> methods.
	/// It is recommended that to avoid confusion you never use {@code Condition}
	/// instances in this way, except perhaps within their own implementation.
	/// 
	/// </para>
	/// <para>Except where noted, passing a {@code null} value for any parameter
	/// will result in a <seealso cref="NullPointerException"/> being thrown.
	/// 
	/// <h3>Implementation Considerations</h3>
	/// 
	/// </para>
	/// <para>When waiting upon a {@code Condition}, a &quot;<em>spurious
	/// wakeup</em>&quot; is permitted to occur, in
	/// general, as a concession to the underlying platform semantics.
	/// This has little practical impact on most application programs as a
	/// {@code Condition} should always be waited upon in a loop, testing
	/// the state predicate that is being waited for.  An implementation is
	/// free to remove the possibility of spurious wakeups but it is
	/// recommended that applications programmers always assume that they can
	/// occur and so always wait in a loop.
	/// 
	/// </para>
	/// <para>The three forms of condition waiting
	/// (interruptible, non-interruptible, and timed) may differ in their ease of
	/// implementation on some platforms and in their performance characteristics.
	/// In particular, it may be difficult to provide these features and maintain
	/// specific semantics such as ordering guarantees.
	/// Further, the ability to interrupt the actual suspension of the thread may
	/// not always be feasible to implement on all platforms.
	/// 
	/// </para>
	/// <para>Consequently, an implementation is not required to define exactly the
	/// same guarantees or semantics for all three forms of waiting, nor is it
	/// required to support interruption of the actual suspension of the thread.
	/// 
	/// </para>
	/// <para>An implementation is required to
	/// clearly document the semantics and guarantees provided by each of the
	/// waiting methods, and when an implementation does support interruption of
	/// thread suspension then it must obey the interruption semantics as defined
	/// in this interface.
	/// 
	/// </para>
	/// <para>As interruption generally implies cancellation, and checks for
	/// interruption are often infrequent, an implementation can favor responding
	/// to an interrupt over normal method return. This is true even if it can be
	/// shown that the interrupt occurred after another action that may have
	/// unblocked the thread. An implementation should document this behavior.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public interface Condition
	{

		/// <summary>
		/// Causes the current thread to wait until it is signalled or
		/// <seealso cref="Thread#interrupt interrupted"/>.
		/// 
		/// <para>The lock associated with this {@code Condition} is atomically
		/// released and the current thread becomes disabled for thread scheduling
		/// purposes and lies dormant until <em>one</em> of four things happens:
		/// <ul>
		/// <li>Some other thread invokes the <seealso cref="#signal"/> method for this
		/// {@code Condition} and the current thread happens to be chosen as the
		/// thread to be awakened; or
		/// <li>Some other thread invokes the <seealso cref="#signalAll"/> method for this
		/// {@code Condition}; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/> the
		/// current thread, and interruption of thread suspension is supported; or
		/// <li>A &quot;<em>spurious wakeup</em>&quot; occurs.
		/// </ul>
		/// 
		/// </para>
		/// <para>In all cases, before this method can return the current thread must
		/// re-acquire the lock associated with this condition. When the
		/// thread returns it is <em>guaranteed</em> to hold this lock.
		/// 
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		/// and interruption of thread suspension is supported,
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared. It is not specified, in the first
		/// case, whether or not the test for interruption occurs before the lock
		/// is released.
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>The current thread is assumed to hold the lock associated with this
		/// {@code Condition} when this method is called.
		/// It is up to the implementation to determine if this is
		/// the case and if not, how to respond. Typically, an exception will be
		/// thrown (such as <seealso cref="IllegalMonitorStateException"/>) and the
		/// implementation must document that fact.
		/// 
		/// </para>
		/// <para>An implementation can favor responding to an interrupt over normal
		/// method return in response to a signal. In that case the implementation
		/// must ensure that the signal is redirected to another waiting thread, if
		/// there is one.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		///         (and interruption of thread suspension is supported) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void await() throws InterruptedException;
		void @await();

		/// <summary>
		/// Causes the current thread to wait until it is signalled.
		/// 
		/// <para>The lock associated with this condition is atomically
		/// released and the current thread becomes disabled for thread scheduling
		/// purposes and lies dormant until <em>one</em> of three things happens:
		/// <ul>
		/// <li>Some other thread invokes the <seealso cref="#signal"/> method for this
		/// {@code Condition} and the current thread happens to be chosen as the
		/// thread to be awakened; or
		/// <li>Some other thread invokes the <seealso cref="#signalAll"/> method for this
		/// {@code Condition}; or
		/// <li>A &quot;<em>spurious wakeup</em>&quot; occurs.
		/// </ul>
		/// 
		/// </para>
		/// <para>In all cases, before this method can return the current thread must
		/// re-acquire the lock associated with this condition. When the
		/// thread returns it is <em>guaranteed</em> to hold this lock.
		/// 
		/// </para>
		/// <para>If the current thread's interrupted status is set when it enters
		/// this method, or it is <seealso cref="Thread#interrupt interrupted"/>
		/// while waiting, it will continue to wait until signalled. When it finally
		/// returns from this method its interrupted status will still
		/// be set.
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>The current thread is assumed to hold the lock associated with this
		/// {@code Condition} when this method is called.
		/// It is up to the implementation to determine if this is
		/// the case and if not, how to respond. Typically, an exception will be
		/// thrown (such as <seealso cref="IllegalMonitorStateException"/>) and the
		/// implementation must document that fact.
		/// </para>
		/// </summary>
		void AwaitUninterruptibly();

		/// <summary>
		/// Causes the current thread to wait until it is signalled or interrupted,
		/// or the specified waiting time elapses.
		/// 
		/// <para>The lock associated with this condition is atomically
		/// released and the current thread becomes disabled for thread scheduling
		/// purposes and lies dormant until <em>one</em> of five things happens:
		/// <ul>
		/// <li>Some other thread invokes the <seealso cref="#signal"/> method for this
		/// {@code Condition} and the current thread happens to be chosen as the
		/// thread to be awakened; or
		/// <li>Some other thread invokes the <seealso cref="#signalAll"/> method for this
		/// {@code Condition}; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/> the
		/// current thread, and interruption of thread suspension is supported; or
		/// <li>The specified waiting time elapses; or
		/// <li>A &quot;<em>spurious wakeup</em>&quot; occurs.
		/// </ul>
		/// 
		/// </para>
		/// <para>In all cases, before this method can return the current thread must
		/// re-acquire the lock associated with this condition. When the
		/// thread returns it is <em>guaranteed</em> to hold this lock.
		/// 
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		/// and interruption of thread suspension is supported,
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared. It is not specified, in the first
		/// case, whether or not the test for interruption occurs before the lock
		/// is released.
		/// 
		/// </para>
		/// <para>The method returns an estimate of the number of nanoseconds
		/// remaining to wait given the supplied {@code nanosTimeout}
		/// value upon return, or a value less than or equal to zero if it
		/// timed out. This value can be used to determine whether and how
		/// long to re-wait in cases where the wait returns but an awaited
		/// condition still does not hold. Typical uses of this method take
		/// the following form:
		/// 
		///  <pre> {@code
		/// boolean aMethod(long timeout, TimeUnit unit) {
		///   long nanos = unit.toNanos(timeout);
		///   lock.lock();
		///   try {
		///     while (!conditionBeingWaitedFor()) {
		///       if (nanos <= 0L)
		///         return false;
		///       nanos = theCondition.awaitNanos(nanos);
		///     }
		///     // ...
		///   } finally {
		///     lock.unlock();
		///   }
		/// }}</pre>
		/// 
		/// </para>
		/// <para>Design note: This method requires a nanosecond argument so
		/// as to avoid truncation errors in reporting remaining times.
		/// Such precision loss would make it difficult for programmers to
		/// ensure that total waiting times are not systematically shorter
		/// than specified when re-waits occur.
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>The current thread is assumed to hold the lock associated with this
		/// {@code Condition} when this method is called.
		/// It is up to the implementation to determine if this is
		/// the case and if not, how to respond. Typically, an exception will be
		/// thrown (such as <seealso cref="IllegalMonitorStateException"/>) and the
		/// implementation must document that fact.
		/// 
		/// </para>
		/// <para>An implementation can favor responding to an interrupt over normal
		/// method return in response to a signal, or over indicating the elapse
		/// of the specified waiting time. In either case the implementation
		/// must ensure that the signal is redirected to another waiting thread, if
		/// there is one.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nanosTimeout"> the maximum time to wait, in nanoseconds </param>
		/// <returns> an estimate of the {@code nanosTimeout} value minus
		///         the time spent waiting upon return from this method.
		///         A positive value may be used as the argument to a
		///         subsequent call to this method to finish waiting out
		///         the desired time.  A value less than or equal to zero
		///         indicates that no time remains. </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		///         (and interruption of thread suspension is supported) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long awaitNanos(long nanosTimeout) throws InterruptedException;
		long AwaitNanos(long nanosTimeout);

		/// <summary>
		/// Causes the current thread to wait until it is signalled or interrupted,
		/// or the specified waiting time elapses. This method is behaviorally
		/// equivalent to:
		///  <pre> {@code awaitNanos(unit.toNanos(time)) > 0}</pre>
		/// </summary>
		/// <param name="time"> the maximum time to wait </param>
		/// <param name="unit"> the time unit of the {@code time} argument </param>
		/// <returns> {@code false} if the waiting time detectably elapsed
		///         before return from the method, else {@code true} </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		///         (and interruption of thread suspension is supported) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean await(long time, java.util.concurrent.TimeUnit unit) throws InterruptedException;
		bool @await(long time, TimeUnit unit);

		/// <summary>
		/// Causes the current thread to wait until it is signalled or interrupted,
		/// or the specified deadline elapses.
		/// 
		/// <para>The lock associated with this condition is atomically
		/// released and the current thread becomes disabled for thread scheduling
		/// purposes and lies dormant until <em>one</em> of five things happens:
		/// <ul>
		/// <li>Some other thread invokes the <seealso cref="#signal"/> method for this
		/// {@code Condition} and the current thread happens to be chosen as the
		/// thread to be awakened; or
		/// <li>Some other thread invokes the <seealso cref="#signalAll"/> method for this
		/// {@code Condition}; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/> the
		/// current thread, and interruption of thread suspension is supported; or
		/// <li>The specified deadline elapses; or
		/// <li>A &quot;<em>spurious wakeup</em>&quot; occurs.
		/// </ul>
		/// 
		/// </para>
		/// <para>In all cases, before this method can return the current thread must
		/// re-acquire the lock associated with this condition. When the
		/// thread returns it is <em>guaranteed</em> to hold this lock.
		/// 
		/// 
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		/// and interruption of thread suspension is supported,
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared. It is not specified, in the first
		/// case, whether or not the test for interruption occurs before the lock
		/// is released.
		/// 
		/// 
		/// </para>
		/// <para>The return value indicates whether the deadline has elapsed,
		/// which can be used as follows:
		///  <pre> {@code
		/// boolean aMethod(Date deadline) {
		///   boolean stillWaiting = true;
		///   lock.lock();
		///   try {
		///     while (!conditionBeingWaitedFor()) {
		///       if (!stillWaiting)
		///         return false;
		///       stillWaiting = theCondition.awaitUntil(deadline);
		///     }
		///     // ...
		///   } finally {
		///     lock.unlock();
		///   }
		/// }}</pre>
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>The current thread is assumed to hold the lock associated with this
		/// {@code Condition} when this method is called.
		/// It is up to the implementation to determine if this is
		/// the case and if not, how to respond. Typically, an exception will be
		/// thrown (such as <seealso cref="IllegalMonitorStateException"/>) and the
		/// implementation must document that fact.
		/// 
		/// </para>
		/// <para>An implementation can favor responding to an interrupt over normal
		/// method return in response to a signal, or over indicating the passing
		/// of the specified deadline. In either case the implementation
		/// must ensure that the signal is redirected to another waiting thread, if
		/// there is one.
		/// 
		/// </para>
		/// </summary>
		/// <param name="deadline"> the absolute time to wait until </param>
		/// <returns> {@code false} if the deadline has elapsed upon return, else
		///         {@code true} </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		///         (and interruption of thread suspension is supported) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean awaitUntil(java.util.Date deadline) throws InterruptedException;
		bool AwaitUntil(DateTime deadline);

		/// <summary>
		/// Wakes up one waiting thread.
		/// 
		/// <para>If any threads are waiting on this condition then one
		/// is selected for waking up. That thread must then re-acquire the
		/// lock before returning from {@code await}.
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>An implementation may (and typically does) require that the
		/// current thread hold the lock associated with this {@code
		/// Condition} when this method is called. Implementations must
		/// document this precondition and any actions taken if the lock is
		/// not held. Typically, an exception such as {@link
		/// IllegalMonitorStateException} will be thrown.
		/// </para>
		/// </summary>
		void Signal();

		/// <summary>
		/// Wakes up all waiting threads.
		/// 
		/// <para>If any threads are waiting on this condition then they are
		/// all woken up. Each thread must re-acquire the lock before it can
		/// return from {@code await}.
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>An implementation may (and typically does) require that the
		/// current thread hold the lock associated with this {@code
		/// Condition} when this method is called. Implementations must
		/// document this precondition and any actions taken if the lock is
		/// not held. Typically, an exception such as {@link
		/// IllegalMonitorStateException} will be thrown.
		/// </para>
		/// </summary>
		void SignalAll();
	}

}