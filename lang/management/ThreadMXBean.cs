/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.management
{

	/// <summary>
	/// The management interface for the thread system of
	/// the Java virtual machine.
	/// 
	/// <para> A Java virtual machine has a single instance of the implementation
	/// class of this interface.  This instance implementing this interface is
	/// an <a href="ManagementFactory.html#MXBean">MXBean</a>
	/// that can be obtained by calling
	/// the <seealso cref="ManagementFactory#getThreadMXBean"/> method or
	/// from the {@link ManagementFactory#getPlatformMBeanServer
	/// platform <tt>MBeanServer</tt>} method.
	/// 
	/// </para>
	/// <para>The <tt>ObjectName</tt> for uniquely identifying the MXBean for
	/// the thread system within an MBeanServer is:
	/// <blockquote>
	///    {@link ManagementFactory#THREAD_MXBEAN_NAME
	///           <tt>java.lang:type=Threading</tt>}
	/// </blockquote>
	/// 
	/// It can be obtained by calling the
	/// <seealso cref="PlatformManagedObject#getObjectName"/> method.
	/// 
	/// <h3>Thread ID</h3>
	/// Thread ID is a positive long value returned by calling the
	/// <seealso cref="java.lang.Thread#getId"/> method for a thread.
	/// The thread ID is unique during its lifetime.  When a thread
	/// is terminated, this thread ID may be reused.
	/// 
	/// </para>
	/// <para> Some methods in this interface take a thread ID or an array
	/// of thread IDs as the input parameter and return per-thread information.
	/// 
	/// <h3>Thread CPU time</h3>
	/// A Java virtual machine implementation may support measuring
	/// the CPU time for the current thread, for any thread, or for no threads.
	/// 
	/// </para>
	/// <para>
	/// The <seealso cref="#isThreadCpuTimeSupported"/> method can be used to determine
	/// if a Java virtual machine supports measuring of the CPU time for any
	/// thread.  The <seealso cref="#isCurrentThreadCpuTimeSupported"/> method can
	/// be used to determine if a Java virtual machine supports measuring of
	/// the CPU time for the current  thread.
	/// A Java virtual machine implementation that supports CPU time measurement
	/// for any thread will also support that for the current thread.
	/// 
	/// </para>
	/// <para> The CPU time provided by this interface has nanosecond precision
	/// but not necessarily nanosecond accuracy.
	/// 
	/// </para>
	/// <para>
	/// A Java virtual machine may disable CPU time measurement
	/// by default.
	/// The <seealso cref="#isThreadCpuTimeEnabled"/> and <seealso cref="#setThreadCpuTimeEnabled"/>
	/// methods can be used to test if CPU time measurement is enabled
	/// and to enable/disable this support respectively.
	/// Enabling thread CPU measurement could be expensive in some
	/// Java virtual machine implementations.
	/// 
	/// <h3>Thread Contention Monitoring</h3>
	/// Some Java virtual machines may support thread contention monitoring.
	/// When thread contention monitoring is enabled, the accumulated elapsed
	/// time that the thread has blocked for synchronization or waited for
	/// notification will be collected and returned in the
	/// <a href="ThreadInfo.html#SyncStats"><tt>ThreadInfo</tt></a> object.
	/// </para>
	/// <para>
	/// The <seealso cref="#isThreadContentionMonitoringSupported"/> method can be used to
	/// determine if a Java virtual machine supports thread contention monitoring.
	/// The thread contention monitoring is disabled by default.  The
	/// <seealso cref="#setThreadContentionMonitoringEnabled"/> method can be used to enable
	/// thread contention monitoring.
	/// 
	/// <h3>Synchronization Information and Deadlock Detection</h3>
	/// Some Java virtual machines may support monitoring of
	/// <seealso cref="#isObjectMonitorUsageSupported object monitor usage"/> and
	/// <seealso cref="#isSynchronizerUsageSupported ownable synchronizer usage"/>.
	/// The <seealso cref="#getThreadInfo(long[], boolean, boolean)"/> and
	/// <seealso cref="#dumpAllThreads"/> methods can be used to obtain the thread stack trace
	/// and synchronization information including which
	/// <seealso cref="LockInfo <i>lock</i>"/> a thread is blocked to
	/// acquire or waiting on and which locks the thread currently owns.
	/// </para>
	/// <para>
	/// The <tt>ThreadMXBean</tt> interface provides the
	/// <seealso cref="#findMonitorDeadlockedThreads"/> and
	/// <seealso cref="#findDeadlockedThreads"/> methods to find deadlocks in
	/// the running application.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ManagementFactory#getPlatformMXBeans(Class) </seealso>
	/// <seealso cref= <a href="../../../javax/management/package-summary.html">
	///      JMX Specification.</a> </seealso>
	/// <seealso cref= <a href="package-summary.html#examples">
	///      Ways to Access MXBeans</a>
	/// 
	/// @author  Mandy Chung
	/// @since   1.5 </seealso>

	public interface ThreadMXBean : PlatformManagedObject
	{
		/// <summary>
		/// Returns the current number of live threads including both
		/// daemon and non-daemon threads.
		/// </summary>
		/// <returns> the current number of live threads. </returns>
		int ThreadCount {get;}

		/// <summary>
		/// Returns the peak live thread count since the Java virtual machine
		/// started or peak was reset.
		/// </summary>
		/// <returns> the peak live thread count. </returns>
		int PeakThreadCount {get;}

		/// <summary>
		/// Returns the total number of threads created and also started
		/// since the Java virtual machine started.
		/// </summary>
		/// <returns> the total number of threads started. </returns>
		long TotalStartedThreadCount {get;}

		/// <summary>
		/// Returns the current number of live daemon threads.
		/// </summary>
		/// <returns> the current number of live daemon threads. </returns>
		int DaemonThreadCount {get;}

		/// <summary>
		/// Returns all live thread IDs.
		/// Some threads included in the returned array
		/// may have been terminated when this method returns.
		/// </summary>
		/// <returns> an array of <tt>long</tt>, each is a thread ID.
		/// </returns>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("monitor"). </exception>
		long[] AllThreadIds {get;}

		/// <summary>
		/// Returns the thread info for a thread of the specified
		/// <tt>id</tt> with no stack trace.
		/// This method is equivalent to calling:
		/// <blockquote>
		///   <seealso cref="#getThreadInfo(long, int) getThreadInfo(id, 0);"/>
		/// </blockquote>
		/// 
		/// <para>
		/// This method returns a <tt>ThreadInfo</tt> object representing
		/// the thread information for the thread of the specified ID.
		/// The stack trace, locked monitors, and locked synchronizers
		/// in the returned <tt>ThreadInfo</tt> object will
		/// be empty.
		/// 
		/// If a thread of the given ID is not alive or does not exist,
		/// this method will return <tt>null</tt>.  A thread is alive if
		/// it has been started and has not yet died.
		/// 
		/// </para>
		/// <para>
		/// <b>MBeanServer access</b>:<br>
		/// The mapped type of <tt>ThreadInfo</tt> is
		/// <tt>CompositeData</tt> with attributes as specified in the
		/// <seealso cref="ThreadInfo#from ThreadInfo.from"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> the thread ID of the thread. Must be positive.
		/// </param>
		/// <returns> a <seealso cref="ThreadInfo"/> object for the thread of the given ID
		/// with no stack trace, no locked monitor and no synchronizer info;
		/// <tt>null</tt> if the thread of the given ID is not alive or
		/// it does not exist.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if {@code id <= 0}. </exception>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("monitor"). </exception>
		ThreadInfo GetThreadInfo(long id);

		/// <summary>
		/// Returns the thread info for each thread
		/// whose ID is in the input array <tt>ids</tt> with no stack trace.
		/// This method is equivalent to calling:
		/// <blockquote><pre>
		///   <seealso cref="#getThreadInfo(long[], int) getThreadInfo"/>(ids, 0);
		/// </pre></blockquote>
		/// 
		/// <para>
		/// This method returns an array of the <tt>ThreadInfo</tt> objects.
		/// The stack trace, locked monitors, and locked synchronizers
		/// in each <tt>ThreadInfo</tt> object will be empty.
		/// 
		/// If a thread of a given ID is not alive or does not exist,
		/// the corresponding element in the returned array will
		/// contain <tt>null</tt>.  A thread is alive if
		/// it has been started and has not yet died.
		/// 
		/// </para>
		/// <para>
		/// <b>MBeanServer access</b>:<br>
		/// The mapped type of <tt>ThreadInfo</tt> is
		/// <tt>CompositeData</tt> with attributes as specified in the
		/// <seealso cref="ThreadInfo#from ThreadInfo.from"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ids"> an array of thread IDs. </param>
		/// <returns> an array of the <seealso cref="ThreadInfo"/> objects, each containing
		/// information about a thread whose ID is in the corresponding
		/// element of the input array of IDs
		/// with no stack trace, no locked monitor and no synchronizer info.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if any element in the input array
		///         <tt>ids</tt> is {@code <= 0}. </exception>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("monitor"). </exception>
		ThreadInfo[] GetThreadInfo(long[] ids);

		/// <summary>
		/// Returns a thread info for a thread of the specified <tt>id</tt>,
		/// with stack trace of a specified number of stack trace elements.
		/// The <tt>maxDepth</tt> parameter indicates the maximum number of
		/// <seealso cref="StackTraceElement"/> to be retrieved from the stack trace.
		/// If <tt>maxDepth == Integer.MAX_VALUE</tt>, the entire stack trace of
		/// the thread will be dumped.
		/// If <tt>maxDepth == 0</tt>, no stack trace of the thread
		/// will be dumped.
		/// This method does not obtain the locked monitors and locked
		/// synchronizers of the thread.
		/// <para>
		/// When the Java virtual machine has no stack trace information
		/// about a thread or <tt>maxDepth == 0</tt>,
		/// the stack trace in the
		/// <tt>ThreadInfo</tt> object will be an empty array of
		/// <tt>StackTraceElement</tt>.
		/// 
		/// </para>
		/// <para>
		/// If a thread of the given ID is not alive or does not exist,
		/// this method will return <tt>null</tt>.  A thread is alive if
		/// it has been started and has not yet died.
		/// 
		/// </para>
		/// <para>
		/// <b>MBeanServer access</b>:<br>
		/// The mapped type of <tt>ThreadInfo</tt> is
		/// <tt>CompositeData</tt> with attributes as specified in the
		/// <seealso cref="ThreadInfo#from ThreadInfo.from"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> the thread ID of the thread. Must be positive. </param>
		/// <param name="maxDepth"> the maximum number of entries in the stack trace
		/// to be dumped. <tt>Integer.MAX_VALUE</tt> could be used to request
		/// the entire stack to be dumped.
		/// </param>
		/// <returns> a <seealso cref="ThreadInfo"/> of the thread of the given ID
		/// with no locked monitor and synchronizer info.
		/// <tt>null</tt> if the thread of the given ID is not alive or
		/// it does not exist.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if {@code id <= 0}. </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>maxDepth is negative</tt>. </exception>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("monitor").
		///  </exception>
		ThreadInfo GetThreadInfo(long id, int maxDepth);

		/// <summary>
		/// Returns the thread info for each thread
		/// whose ID is in the input array <tt>ids</tt>,
		/// with stack trace of a specified number of stack trace elements.
		/// The <tt>maxDepth</tt> parameter indicates the maximum number of
		/// <seealso cref="StackTraceElement"/> to be retrieved from the stack trace.
		/// If <tt>maxDepth == Integer.MAX_VALUE</tt>, the entire stack trace of
		/// the thread will be dumped.
		/// If <tt>maxDepth == 0</tt>, no stack trace of the thread
		/// will be dumped.
		/// This method does not obtain the locked monitors and locked
		/// synchronizers of the threads.
		/// <para>
		/// When the Java virtual machine has no stack trace information
		/// about a thread or <tt>maxDepth == 0</tt>,
		/// the stack trace in the
		/// <tt>ThreadInfo</tt> object will be an empty array of
		/// <tt>StackTraceElement</tt>.
		/// </para>
		/// <para>
		/// This method returns an array of the <tt>ThreadInfo</tt> objects,
		/// each is the thread information about the thread with the same index
		/// as in the <tt>ids</tt> array.
		/// If a thread of the given ID is not alive or does not exist,
		/// <tt>null</tt> will be set in the corresponding element
		/// in the returned array.  A thread is alive if
		/// it has been started and has not yet died.
		/// 
		/// </para>
		/// <para>
		/// <b>MBeanServer access</b>:<br>
		/// The mapped type of <tt>ThreadInfo</tt> is
		/// <tt>CompositeData</tt> with attributes as specified in the
		/// <seealso cref="ThreadInfo#from ThreadInfo.from"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ids"> an array of thread IDs </param>
		/// <param name="maxDepth"> the maximum number of entries in the stack trace
		/// to be dumped. <tt>Integer.MAX_VALUE</tt> could be used to request
		/// the entire stack to be dumped.
		/// </param>
		/// <returns> an array of the <seealso cref="ThreadInfo"/> objects, each containing
		/// information about a thread whose ID is in the corresponding
		/// element of the input array of IDs with no locked monitor and
		/// synchronizer info.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if <tt>maxDepth is negative</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if any element in the input array
		///      <tt>ids</tt> is {@code <= 0}. </exception>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("monitor").
		///  </exception>
		ThreadInfo[] GetThreadInfo(long[] ids, int maxDepth);

		/// <summary>
		/// Tests if the Java virtual machine supports thread contention monitoring.
		/// 
		/// @return
		///   <tt>true</tt>
		///     if the Java virtual machine supports thread contention monitoring;
		///   <tt>false</tt> otherwise.
		/// </summary>
		bool ThreadContentionMonitoringSupported {get;}

		/// <summary>
		/// Tests if thread contention monitoring is enabled.
		/// </summary>
		/// <returns> <tt>true</tt> if thread contention monitoring is enabled;
		///         <tt>false</tt> otherwise.
		/// </returns>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the Java virtual
		/// machine does not support thread contention monitoring.
		/// </exception>
		/// <seealso cref= #isThreadContentionMonitoringSupported </seealso>
		bool ThreadContentionMonitoringEnabled {get;set;}


		/// <summary>
		/// Returns the total CPU time for the current thread in nanoseconds.
		/// The returned value is of nanoseconds precision but
		/// not necessarily nanoseconds accuracy.
		/// If the implementation distinguishes between user mode time and system
		/// mode time, the returned CPU time is the amount of time that
		/// the current thread has executed in user mode or system mode.
		/// 
		/// <para>
		/// This is a convenient method for local management use and is
		/// equivalent to calling:
		/// <blockquote><pre>
		///   <seealso cref="#getThreadCpuTime getThreadCpuTime"/>(Thread.currentThread().getId());
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the total CPU time for the current thread if CPU time
		/// measurement is enabled; <tt>-1</tt> otherwise.
		/// </returns>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the Java
		/// virtual machine does not support CPU time measurement for
		/// the current thread.
		/// </exception>
		/// <seealso cref= #getCurrentThreadUserTime </seealso>
		/// <seealso cref= #isCurrentThreadCpuTimeSupported </seealso>
		/// <seealso cref= #isThreadCpuTimeEnabled </seealso>
		/// <seealso cref= #setThreadCpuTimeEnabled </seealso>
		long CurrentThreadCpuTime {get;}

		/// <summary>
		/// Returns the CPU time that the current thread has executed
		/// in user mode in nanoseconds.
		/// The returned value is of nanoseconds precision but
		/// not necessarily nanoseconds accuracy.
		/// 
		/// <para>
		/// This is a convenient method for local management use and is
		/// equivalent to calling:
		/// <blockquote><pre>
		///   <seealso cref="#getThreadUserTime getThreadUserTime"/>(Thread.currentThread().getId());
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the user-level CPU time for the current thread if CPU time
		/// measurement is enabled; <tt>-1</tt> otherwise.
		/// </returns>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the Java
		/// virtual machine does not support CPU time measurement for
		/// the current thread.
		/// </exception>
		/// <seealso cref= #getCurrentThreadCpuTime </seealso>
		/// <seealso cref= #isCurrentThreadCpuTimeSupported </seealso>
		/// <seealso cref= #isThreadCpuTimeEnabled </seealso>
		/// <seealso cref= #setThreadCpuTimeEnabled </seealso>
		long CurrentThreadUserTime {get;}

		/// <summary>
		/// Returns the total CPU time for a thread of the specified ID in nanoseconds.
		/// The returned value is of nanoseconds precision but
		/// not necessarily nanoseconds accuracy.
		/// If the implementation distinguishes between user mode time and system
		/// mode time, the returned CPU time is the amount of time that
		/// the thread has executed in user mode or system mode.
		/// 
		/// <para>
		/// If the thread of the specified ID is not alive or does not exist,
		/// this method returns <tt>-1</tt>. If CPU time measurement
		/// is disabled, this method returns <tt>-1</tt>.
		/// A thread is alive if it has been started and has not yet died.
		/// </para>
		/// <para>
		/// If CPU time measurement is enabled after the thread has started,
		/// the Java virtual machine implementation may choose any time up to
		/// and including the time that the capability is enabled as the point
		/// where CPU time measurement starts.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> the thread ID of a thread </param>
		/// <returns> the total CPU time for a thread of the specified ID
		/// if the thread of the specified ID exists, the thread is alive,
		/// and CPU time measurement is enabled;
		/// <tt>-1</tt> otherwise.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if {@code id <= 0}. </exception>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the Java
		/// virtual machine does not support CPU time measurement for
		/// other threads.
		/// </exception>
		/// <seealso cref= #getThreadUserTime </seealso>
		/// <seealso cref= #isThreadCpuTimeSupported </seealso>
		/// <seealso cref= #isThreadCpuTimeEnabled </seealso>
		/// <seealso cref= #setThreadCpuTimeEnabled </seealso>
		long GetThreadCpuTime(long id);

		/// <summary>
		/// Returns the CPU time that a thread of the specified ID
		/// has executed in user mode in nanoseconds.
		/// The returned value is of nanoseconds precision but
		/// not necessarily nanoseconds accuracy.
		/// 
		/// <para>
		/// If the thread of the specified ID is not alive or does not exist,
		/// this method returns <tt>-1</tt>. If CPU time measurement
		/// is disabled, this method returns <tt>-1</tt>.
		/// A thread is alive if it has been started and has not yet died.
		/// </para>
		/// <para>
		/// If CPU time measurement is enabled after the thread has started,
		/// the Java virtual machine implementation may choose any time up to
		/// and including the time that the capability is enabled as the point
		/// where CPU time measurement starts.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> the thread ID of a thread </param>
		/// <returns> the user-level CPU time for a thread of the specified ID
		/// if the thread of the specified ID exists, the thread is alive,
		/// and CPU time measurement is enabled;
		/// <tt>-1</tt> otherwise.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if {@code id <= 0}. </exception>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the Java
		/// virtual machine does not support CPU time measurement for
		/// other threads.
		/// </exception>
		/// <seealso cref= #getThreadCpuTime </seealso>
		/// <seealso cref= #isThreadCpuTimeSupported </seealso>
		/// <seealso cref= #isThreadCpuTimeEnabled </seealso>
		/// <seealso cref= #setThreadCpuTimeEnabled </seealso>
		long GetThreadUserTime(long id);

		/// <summary>
		/// Tests if the Java virtual machine implementation supports CPU time
		/// measurement for any thread.
		/// A Java virtual machine implementation that supports CPU time
		/// measurement for any thread will also support CPU time
		/// measurement for the current thread.
		/// 
		/// @return
		///   <tt>true</tt>
		///     if the Java virtual machine supports CPU time
		///     measurement for any thread;
		///   <tt>false</tt> otherwise.
		/// </summary>
		bool ThreadCpuTimeSupported {get;}

		/// <summary>
		/// Tests if the Java virtual machine supports CPU time
		/// measurement for the current thread.
		/// This method returns <tt>true</tt> if <seealso cref="#isThreadCpuTimeSupported"/>
		/// returns <tt>true</tt>.
		/// 
		/// @return
		///   <tt>true</tt>
		///     if the Java virtual machine supports CPU time
		///     measurement for current thread;
		///   <tt>false</tt> otherwise.
		/// </summary>
		bool CurrentThreadCpuTimeSupported {get;}

		/// <summary>
		/// Tests if thread CPU time measurement is enabled.
		/// </summary>
		/// <returns> <tt>true</tt> if thread CPU time measurement is enabled;
		///         <tt>false</tt> otherwise.
		/// </returns>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the Java virtual
		/// machine does not support CPU time measurement for other threads
		/// nor for the current thread.
		/// </exception>
		/// <seealso cref= #isThreadCpuTimeSupported </seealso>
		/// <seealso cref= #isCurrentThreadCpuTimeSupported </seealso>
		bool ThreadCpuTimeEnabled {get;set;}


		/// <summary>
		/// Finds cycles of threads that are in deadlock waiting to acquire
		/// object monitors. That is, threads that are blocked waiting to enter a
		/// synchronization block or waiting to reenter a synchronization block
		/// after an <seealso cref="Object#wait Object.wait"/> call,
		/// where each thread owns one monitor while
		/// trying to obtain another monitor already held by another thread
		/// in a cycle.
		/// <para>
		/// More formally, a thread is <em>monitor deadlocked</em> if it is
		/// part of a cycle in the relation "is waiting for an object monitor
		/// owned by".  In the simplest case, thread A is blocked waiting
		/// for a monitor owned by thread B, and thread B is blocked waiting
		/// for a monitor owned by thread A.
		/// </para>
		/// <para>
		/// This method is designed for troubleshooting use, but not for
		/// synchronization control.  It might be an expensive operation.
		/// </para>
		/// <para>
		/// This method finds deadlocks involving only object monitors.
		/// To find deadlocks involving both object monitors and
		/// <a href="LockInfo.html#OwnableSynchronizer">ownable synchronizers</a>,
		/// the <seealso cref="#findDeadlockedThreads findDeadlockedThreads"/> method
		/// should be used.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of IDs of the threads that are monitor
		/// deadlocked, if any; <tt>null</tt> otherwise.
		/// </returns>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("monitor").
		/// </exception>
		/// <seealso cref= #findDeadlockedThreads </seealso>
		long[] FindMonitorDeadlockedThreads();

		/// <summary>
		/// Resets the peak thread count to the current number of
		/// live threads.
		/// </summary>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("control").
		/// </exception>
		/// <seealso cref= #getPeakThreadCount </seealso>
		/// <seealso cref= #getThreadCount </seealso>
		void ResetPeakThreadCount();

		/// <summary>
		/// Finds cycles of threads that are in deadlock waiting to acquire
		/// object monitors or
		/// <a href="LockInfo.html#OwnableSynchronizer">ownable synchronizers</a>.
		/// 
		/// Threads are <em>deadlocked</em> in a cycle waiting for a lock of
		/// these two types if each thread owns one lock while
		/// trying to acquire another lock already held
		/// by another thread in the cycle.
		/// <para>
		/// This method is designed for troubleshooting use, but not for
		/// synchronization control.  It might be an expensive operation.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of IDs of the threads that are
		/// deadlocked waiting for object monitors or ownable synchronizers, if any;
		/// <tt>null</tt> otherwise.
		/// </returns>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("monitor"). </exception>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the Java virtual
		/// machine does not support monitoring of ownable synchronizer usage.
		/// </exception>
		/// <seealso cref= #isSynchronizerUsageSupported </seealso>
		/// <seealso cref= #findMonitorDeadlockedThreads
		/// @since 1.6 </seealso>
		long[] FindDeadlockedThreads();

		/// <summary>
		/// Tests if the Java virtual machine supports monitoring of
		/// object monitor usage.
		/// 
		/// @return
		///   <tt>true</tt>
		///     if the Java virtual machine supports monitoring of
		///     object monitor usage;
		///   <tt>false</tt> otherwise.
		/// </summary>
		/// <seealso cref= #dumpAllThreads
		/// @since 1.6 </seealso>
		bool ObjectMonitorUsageSupported {get;}

		/// <summary>
		/// Tests if the Java virtual machine supports monitoring of
		/// <a href="LockInfo.html#OwnableSynchronizer">
		/// ownable synchronizer</a> usage.
		/// 
		/// @return
		///   <tt>true</tt>
		///     if the Java virtual machine supports monitoring of ownable
		///     synchronizer usage;
		///   <tt>false</tt> otherwise.
		/// </summary>
		/// <seealso cref= #dumpAllThreads
		/// @since 1.6 </seealso>
		bool SynchronizerUsageSupported {get;}

		/// <summary>
		/// Returns the thread info for each thread
		/// whose ID is in the input array <tt>ids</tt>, with stack trace
		/// and synchronization information.
		/// 
		/// <para>
		/// This method obtains a snapshot of the thread information
		/// for each thread including:
		/// <ul>
		///    <li>the entire stack trace,</li>
		///    <li>the object monitors currently locked by the thread
		///        if <tt>lockedMonitors</tt> is <tt>true</tt>, and</li>
		///    <li>the <a href="LockInfo.html#OwnableSynchronizer">
		///        ownable synchronizers</a> currently locked by the thread
		///        if <tt>lockedSynchronizers</tt> is <tt>true</tt>.</li>
		/// </ul>
		/// </para>
		/// <para>
		/// This method returns an array of the <tt>ThreadInfo</tt> objects,
		/// each is the thread information about the thread with the same index
		/// as in the <tt>ids</tt> array.
		/// If a thread of the given ID is not alive or does not exist,
		/// <tt>null</tt> will be set in the corresponding element
		/// in the returned array.  A thread is alive if
		/// it has been started and has not yet died.
		/// </para>
		/// <para>
		/// If a thread does not lock any object monitor or <tt>lockedMonitors</tt>
		/// is <tt>false</tt>, the returned <tt>ThreadInfo</tt> object will have an
		/// empty <tt>MonitorInfo</tt> array.  Similarly, if a thread does not
		/// lock any synchronizer or <tt>lockedSynchronizers</tt> is <tt>false</tt>,
		/// the returned <tt>ThreadInfo</tt> object
		/// will have an empty <tt>LockInfo</tt> array.
		/// 
		/// </para>
		/// <para>
		/// When both <tt>lockedMonitors</tt> and <tt>lockedSynchronizers</tt>
		/// parameters are <tt>false</tt>, it is equivalent to calling:
		/// <blockquote><pre>
		///     <seealso cref="#getThreadInfo(long[], int)  getThreadInfo(ids, Integer.MAX_VALUE)"/>
		/// </pre></blockquote>
		/// 
		/// </para>
		/// <para>
		/// This method is designed for troubleshooting use, but not for
		/// synchronization control.  It might be an expensive operation.
		/// 
		/// </para>
		/// <para>
		/// <b>MBeanServer access</b>:<br>
		/// The mapped type of <tt>ThreadInfo</tt> is
		/// <tt>CompositeData</tt> with attributes as specified in the
		/// <seealso cref="ThreadInfo#from ThreadInfo.from"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ids"> an array of thread IDs. </param>
		/// <param name="lockedMonitors"> if <tt>true</tt>, retrieves all locked monitors. </param>
		/// <param name="lockedSynchronizers"> if <tt>true</tt>, retrieves all locked
		///             ownable synchronizers.
		/// </param>
		/// <returns> an array of the <seealso cref="ThreadInfo"/> objects, each containing
		/// information about a thread whose ID is in the corresponding
		/// element of the input array of IDs.
		/// </returns>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("monitor"). </exception>
		/// <exception cref="java.lang.UnsupportedOperationException">
		///         <ul>
		///           <li>if <tt>lockedMonitors</tt> is <tt>true</tt> but
		///               the Java virtual machine does not support monitoring
		///               of {@link #isObjectMonitorUsageSupported
		///               object monitor usage}; or</li>
		///           <li>if <tt>lockedSynchronizers</tt> is <tt>true</tt> but
		///               the Java virtual machine does not support monitoring
		///               of {@link #isSynchronizerUsageSupported
		///               ownable synchronizer usage}.</li>
		///         </ul>
		/// </exception>
		/// <seealso cref= #isObjectMonitorUsageSupported </seealso>
		/// <seealso cref= #isSynchronizerUsageSupported
		/// 
		/// @since 1.6 </seealso>
		ThreadInfo[] GetThreadInfo(long[] ids, bool lockedMonitors, bool lockedSynchronizers);

		/// <summary>
		/// Returns the thread info for all live threads with stack trace
		/// and synchronization information.
		/// Some threads included in the returned array
		/// may have been terminated when this method returns.
		/// 
		/// <para>
		/// This method returns an array of <seealso cref="ThreadInfo"/> objects
		/// as specified in the <seealso cref="#getThreadInfo(long[], boolean, boolean)"/>
		/// method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="lockedMonitors"> if <tt>true</tt>, dump all locked monitors. </param>
		/// <param name="lockedSynchronizers"> if <tt>true</tt>, dump all locked
		///             ownable synchronizers.
		/// </param>
		/// <returns> an array of <seealso cref="ThreadInfo"/> for all live threads.
		/// </returns>
		/// <exception cref="java.lang.SecurityException"> if a security manager
		///         exists and the caller does not have
		///         ManagementPermission("monitor"). </exception>
		/// <exception cref="java.lang.UnsupportedOperationException">
		///         <ul>
		///           <li>if <tt>lockedMonitors</tt> is <tt>true</tt> but
		///               the Java virtual machine does not support monitoring
		///               of {@link #isObjectMonitorUsageSupported
		///               object monitor usage}; or</li>
		///           <li>if <tt>lockedSynchronizers</tt> is <tt>true</tt> but
		///               the Java virtual machine does not support monitoring
		///               of {@link #isSynchronizerUsageSupported
		///               ownable synchronizer usage}.</li>
		///         </ul>
		/// </exception>
		/// <seealso cref= #isObjectMonitorUsageSupported </seealso>
		/// <seealso cref= #isSynchronizerUsageSupported
		/// 
		/// @since 1.6 </seealso>
		ThreadInfo[] DumpAllThreads(bool lockedMonitors, bool lockedSynchronizers);
	}

}