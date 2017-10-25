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
	/// {@code Lock} implementations provide more extensive locking
	/// operations than can be obtained using {@code synchronized} methods
	/// and statements.  They allow more flexible structuring, may have
	/// quite different properties, and may support multiple associated
	/// <seealso cref="Condition"/> objects.
	/// 
	/// <para>A lock is a tool for controlling access to a shared resource by
	/// multiple threads. Commonly, a lock provides exclusive access to a
	/// shared resource: only one thread at a time can acquire the lock and
	/// all access to the shared resource requires that the lock be
	/// acquired first. However, some locks may allow concurrent access to
	/// a shared resource, such as the read lock of a <seealso cref="ReadWriteLock"/>.
	/// 
	/// </para>
	/// <para>The use of {@code synchronized} methods or statements provides
	/// access to the implicit monitor lock associated with every object, but
	/// forces all lock acquisition and release to occur in a block-structured way:
	/// when multiple locks are acquired they must be released in the opposite
	/// order, and all locks must be released in the same lexical scope in which
	/// they were acquired.
	/// 
	/// </para>
	/// <para>While the scoping mechanism for {@code synchronized} methods
	/// and statements makes it much easier to program with monitor locks,
	/// and helps avoid many common programming errors involving locks,
	/// there are occasions where you need to work with locks in a more
	/// flexible way. For example, some algorithms for traversing
	/// concurrently accessed data structures require the use of
	/// &quot;hand-over-hand&quot; or &quot;chain locking&quot;: you
	/// acquire the lock of node A, then node B, then release A and acquire
	/// C, then release B and acquire D and so on.  Implementations of the
	/// {@code Lock} interface enable the use of such techniques by
	/// allowing a lock to be acquired and released in different scopes,
	/// and allowing multiple locks to be acquired and released in any
	/// order.
	/// 
	/// </para>
	/// <para>With this increased flexibility comes additional
	/// responsibility. The absence of block-structured locking removes the
	/// automatic release of locks that occurs with {@code synchronized}
	/// methods and statements. In most cases, the following idiom
	/// should be used:
	/// 
	///  <pre> {@code
	/// Lock l = ...;
	/// l.lock();
	/// try {
	///   // access the resource protected by this lock
	/// } finally {
	///   l.unlock();
	/// }}</pre>
	/// 
	/// When locking and unlocking occur in different scopes, care must be
	/// taken to ensure that all code that is executed while the lock is
	/// held is protected by try-finally or try-catch to ensure that the
	/// lock is released when necessary.
	/// 
	/// </para>
	/// <para>{@code Lock} implementations provide additional functionality
	/// over the use of {@code synchronized} methods and statements by
	/// providing a non-blocking attempt to acquire a lock ({@link
	/// #tryLock()}), an attempt to acquire the lock that can be
	/// interrupted (<seealso cref="#lockInterruptibly"/>, and an attempt to acquire
	/// the lock that can timeout (<seealso cref="#tryLock(long, TimeUnit)"/>).
	/// 
	/// </para>
	/// <para>A {@code Lock} class can also provide behavior and semantics
	/// that is quite different from that of the implicit monitor lock,
	/// such as guaranteed ordering, non-reentrant usage, or deadlock
	/// detection. If an implementation provides such specialized semantics
	/// then the implementation must document those semantics.
	/// 
	/// </para>
	/// <para>Note that {@code Lock} instances are just normal objects and can
	/// themselves be used as the target in a {@code synchronized} statement.
	/// Acquiring the
	/// monitor lock of a {@code Lock} instance has no specified relationship
	/// with invoking any of the <seealso cref="#lock"/> methods of that instance.
	/// It is recommended that to avoid confusion you never use {@code Lock}
	/// instances in this way, except within their own implementation.
	/// 
	/// </para>
	/// <para>Except where noted, passing a {@code null} value for any
	/// parameter will result in a <seealso cref="NullPointerException"/> being
	/// thrown.
	/// 
	/// <h3>Memory Synchronization</h3>
	/// 
	/// </para>
	/// <para>All {@code Lock} implementations <em>must</em> enforce the same
	/// memory synchronization semantics as provided by the built-in monitor
	/// lock, as described in
	/// <a href="https://docs.oracle.com/javase/specs/jls/se7/html/jls-17.html#jls-17.4">
	/// The Java Language Specification (17.4 Memory Model)</a>:
	/// <ul>
	/// <li>A successful {@code lock} operation has the same memory
	/// synchronization effects as a successful <em>Lock</em> action.
	/// <li>A successful {@code unlock} operation has the same
	/// memory synchronization effects as a successful <em>Unlock</em> action.
	/// </ul>
	/// 
	/// Unsuccessful locking and unlocking operations, and reentrant
	/// locking/unlocking operations, do not require any memory
	/// synchronization effects.
	/// 
	/// <h3>Implementation Considerations</h3>
	/// 
	/// </para>
	/// <para>The three forms of lock acquisition (interruptible,
	/// non-interruptible, and timed) may differ in their performance
	/// characteristics, ordering guarantees, or other implementation
	/// qualities.  Further, the ability to interrupt the <em>ongoing</em>
	/// acquisition of a lock may not be available in a given {@code Lock}
	/// class.  Consequently, an implementation is not required to define
	/// exactly the same guarantees or semantics for all three forms of
	/// lock acquisition, nor is it required to support interruption of an
	/// ongoing lock acquisition.  An implementation is required to clearly
	/// document the semantics and guarantees provided by each of the
	/// locking methods. It must also obey the interruption semantics as
	/// defined in this interface, to the extent that interruption of lock
	/// acquisition is supported: which is either totally, or only on
	/// method entry.
	/// 
	/// </para>
	/// <para>As interruption generally implies cancellation, and checks for
	/// interruption are often infrequent, an implementation can favor responding
	/// to an interrupt over normal method return. This is true even if it can be
	/// shown that the interrupt occurred after another action may have unblocked
	/// the thread. An implementation should document this behavior.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ReentrantLock </seealso>
	/// <seealso cref= Condition </seealso>
	/// <seealso cref= ReadWriteLock
	/// 
	/// @since 1.5
	/// @author Doug Lea </seealso>
	public interface Lock
	{

		/// <summary>
		/// Acquires the lock.
		/// 
		/// <para>If the lock is not available then the current thread becomes
		/// disabled for thread scheduling purposes and lies dormant until the
		/// lock has been acquired.
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>A {@code Lock} implementation may be able to detect erroneous use
		/// of the lock, such as an invocation that would cause deadlock, and
		/// may throw an (unchecked) exception in such circumstances.  The
		/// circumstances and the exception type must be documented by that
		/// {@code Lock} implementation.
		/// </para>
		/// </summary>
		void @lock();

		/// <summary>
		/// Acquires the lock unless the current thread is
		/// <seealso cref="Thread#interrupt interrupted"/>.
		/// 
		/// <para>Acquires the lock if it is available and returns immediately.
		/// 
		/// </para>
		/// <para>If the lock is not available then the current thread becomes
		/// disabled for thread scheduling purposes and lies dormant until
		/// one of two things happens:
		/// 
		/// <ul>
		/// <li>The lock is acquired by the current thread; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/> the
		/// current thread, and interruption of lock acquisition is supported.
		/// </ul>
		/// 
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while acquiring the
		/// lock, and interruption of lock acquisition is supported,
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared.
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>The ability to interrupt a lock acquisition in some
		/// implementations may not be possible, and if possible may be an
		/// expensive operation.  The programmer should be aware that this
		/// may be the case. An implementation should document when this is
		/// the case.
		/// 
		/// </para>
		/// <para>An implementation can favor responding to an interrupt over
		/// normal method return.
		/// 
		/// </para>
		/// <para>A {@code Lock} implementation may be able to detect
		/// erroneous use of the lock, such as an invocation that would
		/// cause deadlock, and may throw an (unchecked) exception in such
		/// circumstances.  The circumstances and the exception type must
		/// be documented by that {@code Lock} implementation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="InterruptedException"> if the current thread is
		///         interrupted while acquiring the lock (and interruption
		///         of lock acquisition is supported) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void lockInterruptibly() throws InterruptedException;
		void LockInterruptibly();

		/// <summary>
		/// Acquires the lock only if it is free at the time of invocation.
		/// 
		/// <para>Acquires the lock if it is available and returns immediately
		/// with the value {@code true}.
		/// If the lock is not available then this method will return
		/// immediately with the value {@code false}.
		/// 
		/// </para>
		/// <para>A typical usage idiom for this method would be:
		///  <pre> {@code
		/// Lock lock = ...;
		/// if (lock.tryLock()) {
		///   try {
		///     // manipulate protected state
		///   } finally {
		///     lock.unlock();
		///   }
		/// } else {
		///   // perform alternative actions
		/// }}</pre>
		/// 
		/// This usage ensures that the lock is unlocked if it was acquired, and
		/// doesn't try to unlock if the lock was not acquired.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if the lock was acquired and
		///         {@code false} otherwise </returns>
		bool TryLock();

		/// <summary>
		/// Acquires the lock if it is free within the given waiting time and the
		/// current thread has not been <seealso cref="Thread#interrupt interrupted"/>.
		/// 
		/// <para>If the lock is available this method returns immediately
		/// with the value {@code true}.
		/// If the lock is not available then
		/// the current thread becomes disabled for thread scheduling
		/// purposes and lies dormant until one of three things happens:
		/// <ul>
		/// <li>The lock is acquired by the current thread; or
		/// <li>Some other thread <seealso cref="Thread#interrupt interrupts"/> the
		/// current thread, and interruption of lock acquisition is supported; or
		/// <li>The specified waiting time elapses
		/// </ul>
		/// 
		/// </para>
		/// <para>If the lock is acquired then the value {@code true} is returned.
		/// 
		/// </para>
		/// <para>If the current thread:
		/// <ul>
		/// <li>has its interrupted status set on entry to this method; or
		/// <li>is <seealso cref="Thread#interrupt interrupted"/> while acquiring
		/// the lock, and interruption of lock acquisition is supported,
		/// </ul>
		/// then <seealso cref="InterruptedException"/> is thrown and the current thread's
		/// interrupted status is cleared.
		/// 
		/// </para>
		/// <para>If the specified waiting time elapses then the value {@code false}
		/// is returned.
		/// If the time is
		/// less than or equal to zero, the method will not wait at all.
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>The ability to interrupt a lock acquisition in some implementations
		/// may not be possible, and if possible may
		/// be an expensive operation.
		/// The programmer should be aware that this may be the case. An
		/// implementation should document when this is the case.
		/// 
		/// </para>
		/// <para>An implementation can favor responding to an interrupt over normal
		/// method return, or reporting a timeout.
		/// 
		/// </para>
		/// <para>A {@code Lock} implementation may be able to detect
		/// erroneous use of the lock, such as an invocation that would cause
		/// deadlock, and may throw an (unchecked) exception in such circumstances.
		/// The circumstances and the exception type must be documented by that
		/// {@code Lock} implementation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="time"> the maximum time to wait for the lock </param>
		/// <param name="unit"> the time unit of the {@code time} argument </param>
		/// <returns> {@code true} if the lock was acquired and {@code false}
		///         if the waiting time elapsed before the lock was acquired
		/// </returns>
		/// <exception cref="InterruptedException"> if the current thread is interrupted
		///         while acquiring the lock (and interruption of lock
		///         acquisition is supported) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean tryLock(long time, java.util.concurrent.TimeUnit unit) throws InterruptedException;
		bool TryLock(long time, TimeUnit unit);

		/// <summary>
		/// Releases the lock.
		/// 
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>A {@code Lock} implementation will usually impose
		/// restrictions on which thread can release a lock (typically only the
		/// holder of the lock can release it) and may throw
		/// an (unchecked) exception if the restriction is violated.
		/// Any restrictions and the exception
		/// type must be documented by that {@code Lock} implementation.
		/// </para>
		/// </summary>
		void Unlock();

		/// <summary>
		/// Returns a new <seealso cref="Condition"/> instance that is bound to this
		/// {@code Lock} instance.
		/// 
		/// <para>Before waiting on the condition the lock must be held by the
		/// current thread.
		/// A call to <seealso cref="Condition#await()"/> will atomically release the lock
		/// before waiting and re-acquire the lock before the wait returns.
		/// 
		/// </para>
		/// <para><b>Implementation Considerations</b>
		/// 
		/// </para>
		/// <para>The exact operation of the <seealso cref="Condition"/> instance depends on
		/// the {@code Lock} implementation and must be documented by that
		/// implementation.
		/// 
		/// </para>
		/// </summary>
		/// <returns> A new <seealso cref="Condition"/> instance for this {@code Lock} instance </returns>
		/// <exception cref="UnsupportedOperationException"> if this {@code Lock}
		///         implementation does not support conditions </exception>
		Condition NewCondition();
	}

}