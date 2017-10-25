using System;
using System.Diagnostics;
using System.Threading;

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

	using ManagementFactoryHelper = sun.management.ManagementFactoryHelper;
	using ThreadInfoCompositeData = sun.management.ThreadInfoCompositeData;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static Thread.State.*;

	/// <summary>
	/// Thread information. <tt>ThreadInfo</tt> contains the information
	/// about a thread including:
	/// <h3>General thread information</h3>
	/// <ul>
	///   <li>Thread ID.</li>
	///   <li>Name of the thread.</li>
	/// </ul>
	/// 
	/// <h3>Execution information</h3>
	/// <ul>
	///   <li>Thread state.</li>
	///   <li>The object upon which the thread is blocked due to:
	///       <ul>
	///       <li>waiting to enter a synchronization block/method, or</li>
	///       <li>waiting to be notified in a <seealso cref="Object#wait Object.wait"/> method,
	///           or</li>
	///       <li>parking due to a {@link java.util.concurrent.locks.LockSupport#park
	///           LockSupport.park} call.</li>
	///       </ul>
	///   </li>
	///   <li>The ID of the thread that owns the object
	///       that the thread is blocked.</li>
	///   <li>Stack trace of the thread.</li>
	///   <li>List of object monitors locked by the thread.</li>
	///   <li>List of <a href="LockInfo.html#OwnableSynchronizer">
	///       ownable synchronizers</a> locked by the thread.</li>
	/// </ul>
	/// 
	/// <h4><a name="SyncStats">Synchronization Statistics</a></h4>
	/// <ul>
	///   <li>The number of times that the thread has blocked for
	///       synchronization or waited for notification.</li>
	///   <li>The accumulated elapsed time that the thread has blocked
	///       for synchronization or waited for notification
	///       since {@link ThreadMXBean#setThreadContentionMonitoringEnabled
	///       thread contention monitoring}
	///       was enabled. Some Java virtual machine implementation
	///       may not support this.  The
	///       <seealso cref="ThreadMXBean#isThreadContentionMonitoringSupported()"/>
	///       method can be used to determine if a Java virtual machine
	///       supports this.</li>
	/// </ul>
	/// 
	/// <para>This thread information class is designed for use in monitoring of
	/// the system, not for synchronization control.
	/// 
	/// <h4>MXBean Mapping</h4>
	/// <tt>ThreadInfo</tt> is mapped to a <seealso cref="CompositeData CompositeData"/>
	/// with attributes as specified in
	/// the <seealso cref="#from from"/> method.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ThreadMXBean#getThreadInfo </seealso>
	/// <seealso cref= ThreadMXBean#dumpAllThreads
	/// 
	/// @author  Mandy Chung
	/// @since   1.5 </seealso>

	public class ThreadInfo
	{
		private String ThreadName_Renamed;
		private long ThreadId_Renamed;
		private long BlockedTime_Renamed;
		private long BlockedCount_Renamed;
		private long WaitedTime_Renamed;
		private long WaitedCount_Renamed;
		private LockInfo @lock;
		private String LockName_Renamed;
		private long LockOwnerId_Renamed;
		private String LockOwnerName_Renamed;
		private bool InNative_Renamed;
		private bool Suspended_Renamed;
		private Thread.State ThreadState_Renamed;
		private StackTraceElement[] StackTrace_Renamed;
		private MonitorInfo[] LockedMonitors_Renamed;
		private LockInfo[] LockedSynchronizers_Renamed;

		private static MonitorInfo[] EMPTY_MONITORS = new MonitorInfo[0];
		private static LockInfo[] EMPTY_SYNCS = new LockInfo[0];

		/// <summary>
		/// Constructor of ThreadInfo created by the JVM
		/// </summary>
		/// <param name="t">             Thread </param>
		/// <param name="state">         Thread state </param>
		/// <param name="lockObj">       Object on which the thread is blocked </param>
		/// <param name="lockOwner">     the thread holding the lock </param>
		/// <param name="blockedCount">  Number of times blocked to enter a lock </param>
		/// <param name="blockedTime">   Approx time blocked to enter a lock </param>
		/// <param name="waitedCount">   Number of times waited on a lock </param>
		/// <param name="waitedTime">    Approx time waited on a lock </param>
		/// <param name="stackTrace">    Thread stack trace </param>
		private ThreadInfo(Thread t, int state, Object lockObj, Thread lockOwner, long blockedCount, long blockedTime, long waitedCount, long waitedTime, StackTraceElement[] stackTrace)
		{
			Initialize(t, state, lockObj, lockOwner, blockedCount, blockedTime, waitedCount, waitedTime, stackTrace, EMPTY_MONITORS, EMPTY_SYNCS);
		}

		/// <summary>
		/// Constructor of ThreadInfo created by the JVM
		/// for <seealso cref="ThreadMXBean#getThreadInfo(long[],boolean,boolean)"/>
		/// and <seealso cref="ThreadMXBean#dumpAllThreads"/>
		/// </summary>
		/// <param name="t">             Thread </param>
		/// <param name="state">         Thread state </param>
		/// <param name="lockObj">       Object on which the thread is blocked </param>
		/// <param name="lockOwner">     the thread holding the lock </param>
		/// <param name="blockedCount">  Number of times blocked to enter a lock </param>
		/// <param name="blockedTime">   Approx time blocked to enter a lock </param>
		/// <param name="waitedCount">   Number of times waited on a lock </param>
		/// <param name="waitedTime">    Approx time waited on a lock </param>
		/// <param name="stackTrace">    Thread stack trace </param>
		/// <param name="monitors">      List of locked monitors </param>
		/// <param name="stackDepths">   List of stack depths </param>
		/// <param name="synchronizers"> List of locked synchronizers </param>
		private ThreadInfo(Thread t, int state, Object lockObj, Thread lockOwner, long blockedCount, long blockedTime, long waitedCount, long waitedTime, StackTraceElement[] stackTrace, Object[] monitors, int[] stackDepths, Object[] synchronizers)
		{
			int numMonitors = (monitors == null ? 0 : monitors.Length);
			MonitorInfo[] lockedMonitors;
			if (numMonitors == 0)
			{
				lockedMonitors = EMPTY_MONITORS;
			}
			else
			{
				lockedMonitors = new MonitorInfo[numMonitors];
				for (int i = 0; i < numMonitors; i++)
				{
					Object @lock = monitors[i];
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					String className = @lock.GetType().FullName;
					int identityHashCode = System.identityHashCode(@lock);
					int depth = stackDepths[i];
					StackTraceElement ste = (depth >= 0 ? stackTrace[depth] : null);
					lockedMonitors[i] = new MonitorInfo(className, identityHashCode, depth, ste);
				}
			}

			int numSyncs = (synchronizers == null ? 0 : synchronizers.Length);
			LockInfo[] lockedSynchronizers;
			if (numSyncs == 0)
			{
				lockedSynchronizers = EMPTY_SYNCS;
			}
			else
			{
				lockedSynchronizers = new LockInfo[numSyncs];
				for (int i = 0; i < numSyncs; i++)
				{
					Object @lock = synchronizers[i];
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					String className = @lock.GetType().FullName;
					int identityHashCode = System.identityHashCode(@lock);
					lockedSynchronizers[i] = new LockInfo(className, identityHashCode);
				}
			}

			Initialize(t, state, lockObj, lockOwner, blockedCount, blockedTime, waitedCount, waitedTime, stackTrace, lockedMonitors, lockedSynchronizers);
		}

		/// <summary>
		/// Initialize ThreadInfo object
		/// </summary>
		/// <param name="t">             Thread </param>
		/// <param name="state">         Thread state </param>
		/// <param name="lockObj">       Object on which the thread is blocked </param>
		/// <param name="lockOwner">     the thread holding the lock </param>
		/// <param name="blockedCount">  Number of times blocked to enter a lock </param>
		/// <param name="blockedTime">   Approx time blocked to enter a lock </param>
		/// <param name="waitedCount">   Number of times waited on a lock </param>
		/// <param name="waitedTime">    Approx time waited on a lock </param>
		/// <param name="stackTrace">    Thread stack trace </param>
		/// <param name="lockedMonitors"> List of locked monitors </param>
		/// <param name="lockedSynchronizers"> List of locked synchronizers </param>
		private void Initialize(Thread t, int state, Object lockObj, Thread lockOwner, long blockedCount, long blockedTime, long waitedCount, long waitedTime, StackTraceElement[] stackTrace, MonitorInfo[] lockedMonitors, LockInfo[] lockedSynchronizers)
		{
			this.ThreadId_Renamed = t.Id;
			this.ThreadName_Renamed = t.Name;
			this.ThreadState_Renamed = ManagementFactoryHelper.toThreadState(state);
			this.Suspended_Renamed = ManagementFactoryHelper.isThreadSuspended(state);
			this.InNative_Renamed = ManagementFactoryHelper.isThreadRunningNative(state);
			this.BlockedCount_Renamed = blockedCount;
			this.BlockedTime_Renamed = blockedTime;
			this.WaitedCount_Renamed = waitedCount;
			this.WaitedTime_Renamed = waitedTime;

			if (lockObj == null)
			{
				this.@lock = null;
				this.LockName_Renamed = null;
			}
			else
			{
				this.@lock = new LockInfo(lockObj);
				this.LockName_Renamed = @lock.ClassName + '@' + @lock.IdentityHashCode.ToString("x");
			}
			if (lockOwner == null)
			{
				this.LockOwnerId_Renamed = -1;
				this.LockOwnerName_Renamed = null;
			}
			else
			{
				this.LockOwnerId_Renamed = lockOwner.Id;
				this.LockOwnerName_Renamed = lockOwner.Name;
			}
			if (stackTrace == null)
			{
				this.StackTrace_Renamed = NO_STACK_TRACE;
			}
			else
			{
				this.StackTrace_Renamed = stackTrace;
			}
			this.LockedMonitors_Renamed = lockedMonitors;
			this.LockedSynchronizers_Renamed = lockedSynchronizers;
		}

		/*
		 * Constructs a <tt>ThreadInfo</tt> object from a
		 * {@link CompositeData CompositeData}.
		 */
		private ThreadInfo(CompositeData cd)
		{
			ThreadInfoCompositeData ticd = ThreadInfoCompositeData.getInstance(cd);

			ThreadId_Renamed = ticd.threadId();
			ThreadName_Renamed = ticd.threadName();
			BlockedTime_Renamed = ticd.blockedTime();
			BlockedCount_Renamed = ticd.blockedCount();
			WaitedTime_Renamed = ticd.waitedTime();
			WaitedCount_Renamed = ticd.waitedCount();
			LockName_Renamed = ticd.lockName();
			LockOwnerId_Renamed = ticd.lockOwnerId();
			LockOwnerName_Renamed = ticd.lockOwnerName();
			ThreadState_Renamed = ticd.threadState();
			Suspended_Renamed = ticd.suspended();
			InNative_Renamed = ticd.inNative();
			StackTrace_Renamed = ticd.stackTrace();

			// 6.0 attributes
			if (ticd.CurrentVersion)
			{
				@lock = ticd.lockInfo();
				LockedMonitors_Renamed = ticd.lockedMonitors();
				LockedSynchronizers_Renamed = ticd.lockedSynchronizers();
			}
			else
			{
				// lockInfo is a new attribute added in 1.6 ThreadInfo
				// If cd is a 5.0 version, construct the LockInfo object
				//  from the lockName value.
				if (LockName_Renamed != null)
				{
					String[] result = LockName_Renamed.Split("@");
					if (result.Length == 2)
					{
						int identityHashCode = Convert.ToInt32(result[1], 16);
						@lock = new LockInfo(result[0], identityHashCode);
					}
					else
					{
						Debug.Assert(result.Length == 2);
						@lock = null;
					}
				}
				else
				{
					@lock = null;
				}
				LockedMonitors_Renamed = EMPTY_MONITORS;
				LockedSynchronizers_Renamed = EMPTY_SYNCS;
			}
		}

		/// <summary>
		/// Returns the ID of the thread associated with this <tt>ThreadInfo</tt>.
		/// </summary>
		/// <returns> the ID of the associated thread. </returns>
		public virtual long ThreadId
		{
			get
			{
				return ThreadId_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the thread associated with this <tt>ThreadInfo</tt>.
		/// </summary>
		/// <returns> the name of the associated thread. </returns>
		public virtual String ThreadName
		{
			get
			{
				return ThreadName_Renamed;
			}
		}

		/// <summary>
		/// Returns the state of the thread associated with this <tt>ThreadInfo</tt>.
		/// </summary>
		/// <returns> <tt>Thread.State</tt> of the associated thread. </returns>
		public virtual Thread.State ThreadState
		{
			get
			{
				 return ThreadState_Renamed;
			}
		}

		/// <summary>
		/// Returns the approximate accumulated elapsed time (in milliseconds)
		/// that the thread associated with this <tt>ThreadInfo</tt>
		/// has blocked to enter or reenter a monitor
		/// since thread contention monitoring is enabled.
		/// I.e. the total accumulated time the thread has been in the
		/// <seealso cref="java.lang.Thread.State#BLOCKED BLOCKED"/> state since thread
		/// contention monitoring was last enabled.
		/// This method returns <tt>-1</tt> if thread contention monitoring
		/// is disabled.
		/// 
		/// <para>The Java virtual machine may measure the time with a high
		/// resolution timer.  This statistic is reset when
		/// the thread contention monitoring is reenabled.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the approximate accumulated elapsed time in milliseconds
		/// that a thread entered the <tt>BLOCKED</tt> state;
		/// <tt>-1</tt> if thread contention monitoring is disabled.
		/// </returns>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the Java
		/// virtual machine does not support this operation.
		/// </exception>
		/// <seealso cref= ThreadMXBean#isThreadContentionMonitoringSupported </seealso>
		/// <seealso cref= ThreadMXBean#setThreadContentionMonitoringEnabled </seealso>
		public virtual long BlockedTime
		{
			get
			{
				return BlockedTime_Renamed;
			}
		}

		/// <summary>
		/// Returns the total number of times that
		/// the thread associated with this <tt>ThreadInfo</tt>
		/// blocked to enter or reenter a monitor.
		/// I.e. the number of times a thread has been in the
		/// <seealso cref="java.lang.Thread.State#BLOCKED BLOCKED"/> state.
		/// </summary>
		/// <returns> the total number of times that the thread
		/// entered the <tt>BLOCKED</tt> state. </returns>
		public virtual long BlockedCount
		{
			get
			{
				return BlockedCount_Renamed;
			}
		}

		/// <summary>
		/// Returns the approximate accumulated elapsed time (in milliseconds)
		/// that the thread associated with this <tt>ThreadInfo</tt>
		/// has waited for notification
		/// since thread contention monitoring is enabled.
		/// I.e. the total accumulated time the thread has been in the
		/// <seealso cref="java.lang.Thread.State#WAITING WAITING"/>
		/// or <seealso cref="java.lang.Thread.State#TIMED_WAITING TIMED_WAITING"/> state
		/// since thread contention monitoring is enabled.
		/// This method returns <tt>-1</tt> if thread contention monitoring
		/// is disabled.
		/// 
		/// <para>The Java virtual machine may measure the time with a high
		/// resolution timer.  This statistic is reset when
		/// the thread contention monitoring is reenabled.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the approximate accumulated elapsed time in milliseconds
		/// that a thread has been in the <tt>WAITING</tt> or
		/// <tt>TIMED_WAITING</tt> state;
		/// <tt>-1</tt> if thread contention monitoring is disabled.
		/// </returns>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the Java
		/// virtual machine does not support this operation.
		/// </exception>
		/// <seealso cref= ThreadMXBean#isThreadContentionMonitoringSupported </seealso>
		/// <seealso cref= ThreadMXBean#setThreadContentionMonitoringEnabled </seealso>
		public virtual long WaitedTime
		{
			get
			{
				return WaitedTime_Renamed;
			}
		}

		/// <summary>
		/// Returns the total number of times that
		/// the thread associated with this <tt>ThreadInfo</tt>
		/// waited for notification.
		/// I.e. the number of times that a thread has been
		/// in the <seealso cref="java.lang.Thread.State#WAITING WAITING"/>
		/// or <seealso cref="java.lang.Thread.State#TIMED_WAITING TIMED_WAITING"/> state.
		/// </summary>
		/// <returns> the total number of times that the thread
		/// was in the <tt>WAITING</tt> or <tt>TIMED_WAITING</tt> state. </returns>
		public virtual long WaitedCount
		{
			get
			{
				return WaitedCount_Renamed;
			}
		}

		/// <summary>
		/// Returns the <tt>LockInfo</tt> of an object for which
		/// the thread associated with this <tt>ThreadInfo</tt>
		/// is blocked waiting.
		/// A thread can be blocked waiting for one of the following:
		/// <ul>
		/// <li>an object monitor to be acquired for entering or reentering
		///     a synchronization block/method.
		///     <br>The thread is in the <seealso cref="java.lang.Thread.State#BLOCKED BLOCKED"/>
		///     state waiting to enter the <tt>synchronized</tt> statement
		///     or method.
		///     <para></li>
		/// <li>an object monitor to be notified by another thread.
		///     <br>The thread is in the <seealso cref="java.lang.Thread.State#WAITING WAITING"/>
		///     or <seealso cref="java.lang.Thread.State#TIMED_WAITING TIMED_WAITING"/> state
		///     due to a call to the <seealso cref="Object#wait Object.wait"/> method.
		/// </para>
		///     <para></li>
		/// <li>a synchronization object responsible for the thread parking.
		///     <br>The thread is in the <seealso cref="java.lang.Thread.State#WAITING WAITING"/>
		///     or <seealso cref="java.lang.Thread.State#TIMED_WAITING TIMED_WAITING"/> state
		///     due to a call to the
		///     {@link java.util.concurrent.locks.LockSupport#park(Object)
		///     LockSupport.park} method.  The synchronization object
		///     is the object returned from
		///     {@link java.util.concurrent.locks.LockSupport#getBlocker
		///     LockSupport.getBlocker} method. Typically it is an
		///     <a href="LockInfo.html#OwnableSynchronizer"> ownable synchronizer</a>
		///     or a <seealso cref="java.util.concurrent.locks.Condition Condition"/>.</li>
		/// </ul>
		/// 
		/// </para>
		/// <para>This method returns <tt>null</tt> if the thread is not in any of
		/// the above conditions.
		/// 
		/// </para>
		/// </summary>
		/// <returns> <tt>LockInfo</tt> of an object for which the thread
		///         is blocked waiting if any; <tt>null</tt> otherwise.
		/// @since 1.6 </returns>
		public virtual LockInfo LockInfo
		{
			get
			{
				return @lock;
			}
		}

		/// <summary>
		/// Returns the <seealso cref="LockInfo#toString string representation"/>
		/// of an object for which the thread associated with this
		/// <tt>ThreadInfo</tt> is blocked waiting.
		/// This method is equivalent to calling:
		/// <blockquote>
		/// <pre>
		/// getLockInfo().toString()
		/// </pre></blockquote>
		/// 
		/// <para>This method will return <tt>null</tt> if this thread is not blocked
		/// waiting for any object or if the object is not owned by any thread.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the string representation of the object on which
		/// the thread is blocked if any;
		/// <tt>null</tt> otherwise.
		/// </returns>
		/// <seealso cref= #getLockInfo </seealso>
		public virtual String LockName
		{
			get
			{
				return LockName_Renamed;
			}
		}

		/// <summary>
		/// Returns the ID of the thread which owns the object
		/// for which the thread associated with this <tt>ThreadInfo</tt>
		/// is blocked waiting.
		/// This method will return <tt>-1</tt> if this thread is not blocked
		/// waiting for any object or if the object is not owned by any thread.
		/// </summary>
		/// <returns> the thread ID of the owner thread of the object
		/// this thread is blocked on;
		/// <tt>-1</tt> if this thread is not blocked
		/// or if the object is not owned by any thread.
		/// </returns>
		/// <seealso cref= #getLockInfo </seealso>
		public virtual long LockOwnerId
		{
			get
			{
				return LockOwnerId_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the thread which owns the object
		/// for which the thread associated with this <tt>ThreadInfo</tt>
		/// is blocked waiting.
		/// This method will return <tt>null</tt> if this thread is not blocked
		/// waiting for any object or if the object is not owned by any thread.
		/// </summary>
		/// <returns> the name of the thread that owns the object
		/// this thread is blocked on;
		/// <tt>null</tt> if this thread is not blocked
		/// or if the object is not owned by any thread.
		/// </returns>
		/// <seealso cref= #getLockInfo </seealso>
		public virtual String LockOwnerName
		{
			get
			{
				return LockOwnerName_Renamed;
			}
		}

		/// <summary>
		/// Returns the stack trace of the thread
		/// associated with this <tt>ThreadInfo</tt>.
		/// If no stack trace was requested for this thread info, this method
		/// will return a zero-length array.
		/// If the returned array is of non-zero length then the first element of
		/// the array represents the top of the stack, which is the most recent
		/// method invocation in the sequence.  The last element of the array
		/// represents the bottom of the stack, which is the least recent method
		/// invocation in the sequence.
		/// 
		/// <para>Some Java virtual machines may, under some circumstances, omit one
		/// or more stack frames from the stack trace.  In the extreme case,
		/// a virtual machine that has no stack trace information concerning
		/// the thread associated with this <tt>ThreadInfo</tt>
		/// is permitted to return a zero-length array from this method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of <tt>StackTraceElement</tt> objects of the thread. </returns>
		public virtual StackTraceElement[] StackTrace
		{
			get
			{
				return StackTrace_Renamed;
			}
		}

		/// <summary>
		/// Tests if the thread associated with this <tt>ThreadInfo</tt>
		/// is suspended.  This method returns <tt>true</tt> if
		/// <seealso cref="Thread#suspend"/> has been called.
		/// </summary>
		/// <returns> <tt>true</tt> if the thread is suspended;
		///         <tt>false</tt> otherwise. </returns>
		public virtual bool Suspended
		{
			get
			{
				 return Suspended_Renamed;
			}
		}

		/// <summary>
		/// Tests if the thread associated with this <tt>ThreadInfo</tt>
		/// is executing native code via the Java Native Interface (JNI).
		/// The JNI native code does not include
		/// the virtual machine support code or the compiled native
		/// code generated by the virtual machine.
		/// </summary>
		/// <returns> <tt>true</tt> if the thread is executing native code;
		///         <tt>false</tt> otherwise. </returns>
		public virtual bool InNative
		{
			get
			{
				 return InNative_Renamed;
			}
		}

		/// <summary>
		/// Returns a string representation of this thread info.
		/// The format of this string depends on the implementation.
		/// The returned string will typically include
		/// the <seealso cref="#getThreadName thread name"/>,
		/// the <seealso cref="#getThreadId thread ID"/>,
		/// its <seealso cref="#getThreadState state"/>,
		/// and a <seealso cref="#getStackTrace stack trace"/> if any.
		/// </summary>
		/// <returns> a string representation of this thread info. </returns>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder("\"" + ThreadName + "\"" + " Id=" + ThreadId + " " + ThreadState);
			if (LockName != null)
			{
				sb.Append(" on " + LockName);
			}
			if (LockOwnerName != null)
			{
				sb.Append(" owned by \"" + LockOwnerName + "\" Id=" + LockOwnerId);
			}
			if (Suspended)
			{
				sb.Append(" (suspended)");
			}
			if (InNative)
			{
				sb.Append(" (in native)");
			}
			sb.Append('\n');
			int i = 0;
			for (; i < StackTrace_Renamed.Length && i < MAX_FRAMES; i++)
			{
				StackTraceElement ste = StackTrace_Renamed[i];
				sb.Append("\tat " + ste.ToString());
				sb.Append('\n');
				if (i == 0 && LockInfo != null)
				{
					Thread.State ts = ThreadState;
					switch (ts.InnerEnumValue())
					{
						case Thread.State.InnerEnum.BLOCKED:
							sb.Append("\t-  blocked on " + LockInfo);
							sb.Append('\n');
							break;
						case Thread.State.InnerEnum.WAITING:
							sb.Append("\t-  waiting on " + LockInfo);
							sb.Append('\n');
							break;
						case Thread.State.InnerEnum.TIMED_WAITING:
							sb.Append("\t-  waiting on " + LockInfo);
							sb.Append('\n');
							break;
						default:
					break;
					}
				}

				foreach (MonitorInfo mi in LockedMonitors_Renamed)
				{
					if (mi.LockedStackDepth == i)
					{
						sb.Append("\t-  locked " + mi);
						sb.Append('\n');
					}
				}
			}
		   if (i < StackTrace_Renamed.Length)
		   {
			   sb.Append("\t...");
			   sb.Append('\n');
		   }

		   LockInfo[] locks = LockedSynchronizers;
		   if (locks.Length > 0)
		   {
			   sb.Append("\n\tNumber of locked synchronizers = " + locks.Length);
			   sb.Append('\n');
			   foreach (LockInfo li in locks)
			   {
				   sb.Append("\t- " + li);
				   sb.Append('\n');
			   }
		   }
		   sb.Append('\n');
		   return sb.ToString();
		}
		private const int MAX_FRAMES = 8;

		/// <summary>
		/// Returns a <tt>ThreadInfo</tt> object represented by the
		/// given <tt>CompositeData</tt>.
		/// The given <tt>CompositeData</tt> must contain the following attributes
		/// unless otherwise specified below:
		/// <blockquote>
		/// <table border summary="The attributes and their types the given CompositeData contains">
		/// <tr>
		///   <th align=left>Attribute Name</th>
		///   <th align=left>Type</th>
		/// </tr>
		/// <tr>
		///   <td>threadId</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// <tr>
		///   <td>threadName</td>
		///   <td><tt>java.lang.String</tt></td>
		/// </tr>
		/// <tr>
		///   <td>threadState</td>
		///   <td><tt>java.lang.String</tt></td>
		/// </tr>
		/// <tr>
		///   <td>suspended</td>
		///   <td><tt>java.lang.Boolean</tt></td>
		/// </tr>
		/// <tr>
		///   <td>inNative</td>
		///   <td><tt>java.lang.Boolean</tt></td>
		/// </tr>
		/// <tr>
		///   <td>blockedCount</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// <tr>
		///   <td>blockedTime</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// <tr>
		///   <td>waitedCount</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// <tr>
		///   <td>waitedTime</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// <tr>
		///   <td>lockInfo</td>
		///   <td><tt>javax.management.openmbean.CompositeData</tt>
		///       - the mapped type for <seealso cref="LockInfo"/> as specified in the
		///         <seealso cref="LockInfo#from"/> method.
		///       <para>
		///       If <tt>cd</tt> does not contain this attribute,
		///       the <tt>LockInfo</tt> object will be constructed from
		///       the value of the <tt>lockName</tt> attribute. </td>
		/// </tr>
		/// <tr>
		///   <td>lockName</td>
		///   <td><tt>java.lang.String</tt></td>
		/// </tr>
		/// <tr>
		///   <td>lockOwnerId</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// <tr>
		///   <td>lockOwnerName</td>
		///   <td><tt>java.lang.String</tt></td>
		/// </tr>
		/// <tr>
		///   <td><a name="StackTrace">stackTrace</a></td>
		///   <td><tt>javax.management.openmbean.CompositeData[]</tt>
		/// </para>
		///       <para>
		///       Each element is a <tt>CompositeData</tt> representing
		///       StackTraceElement containing the following attributes:
		///       <blockquote>
		///       <table cellspacing=1 cellpadding=0 summary="The attributes and their types the given CompositeData contains">
		///       <tr>
		///         <th align=left>Attribute Name</th>
		///         <th align=left>Type</th>
		///       </tr>
		///       <tr>
		///         <td>className</td>
		///         <td><tt>java.lang.String</tt></td>
		///       </tr>
		///       <tr>
		///         <td>methodName</td>
		///         <td><tt>java.lang.String</tt></td>
		///       </tr>
		///       <tr>
		///         <td>fileName</td>
		///         <td><tt>java.lang.String</tt></td>
		///       </tr>
		///       <tr>
		///         <td>lineNumber</td>
		///         <td><tt>java.lang.Integer</tt></td>
		///       </tr>
		///       <tr>
		///         <td>nativeMethod</td>
		///         <td><tt>java.lang.Boolean</tt></td>
		///       </tr>
		///       </table>
		///       </blockquote>
		///   </td>
		/// </tr>
		/// <tr>
		///   <td>lockedMonitors</td>
		///   <td><tt>javax.management.openmbean.CompositeData[]</tt>
		///       whose element type is the mapped type for
		///       <seealso cref="MonitorInfo"/> as specified in the
		///       <seealso cref="MonitorInfo#from Monitor.from"/> method.
		/// </para>
		///       <para>
		///       If <tt>cd</tt> does not contain this attribute,
		///       this attribute will be set to an empty array. </td>
		/// </tr>
		/// <tr>
		///   <td>lockedSynchronizers</td>
		///   <td><tt>javax.management.openmbean.CompositeData[]</tt>
		///       whose element type is the mapped type for
		///       <seealso cref="LockInfo"/> as specified in the <seealso cref="LockInfo#from"/> method.
		/// </para>
		///       <para>
		///       If <tt>cd</tt> does not contain this attribute,
		///       this attribute will be set to an empty array. </td>
		/// </tr>
		/// </table>
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="cd"> <tt>CompositeData</tt> representing a <tt>ThreadInfo</tt>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <tt>cd</tt> does not
		///   represent a <tt>ThreadInfo</tt> with the attributes described
		///   above.
		/// </exception>
		/// <returns> a <tt>ThreadInfo</tt> object represented
		///         by <tt>cd</tt> if <tt>cd</tt> is not <tt>null</tt>;
		///         <tt>null</tt> otherwise. </returns>
		public static ThreadInfo From(CompositeData cd)
		{
			if (cd == null)
			{
				return null;
			}

			if (cd is ThreadInfoCompositeData)
			{
				return ((ThreadInfoCompositeData) cd).ThreadInfo;
			}
			else
			{
				return new ThreadInfo(cd);
			}
		}

		/// <summary>
		/// Returns an array of <seealso cref="MonitorInfo"/> objects, each of which
		/// represents an object monitor currently locked by the thread
		/// associated with this <tt>ThreadInfo</tt>.
		/// If no locked monitor was requested for this thread info or
		/// no monitor is locked by the thread, this method
		/// will return a zero-length array.
		/// </summary>
		/// <returns> an array of <tt>MonitorInfo</tt> objects representing
		///         the object monitors locked by the thread.
		/// 
		/// @since 1.6 </returns>
		public virtual MonitorInfo[] LockedMonitors
		{
			get
			{
				return LockedMonitors_Renamed;
			}
		}

		/// <summary>
		/// Returns an array of <seealso cref="LockInfo"/> objects, each of which
		/// represents an <a href="LockInfo.html#OwnableSynchronizer">ownable
		/// synchronizer</a> currently locked by the thread associated with
		/// this <tt>ThreadInfo</tt>.  If no locked synchronizer was
		/// requested for this thread info or no synchronizer is locked by
		/// the thread, this method will return a zero-length array.
		/// </summary>
		/// <returns> an array of <tt>LockInfo</tt> objects representing
		///         the ownable synchronizers locked by the thread.
		/// 
		/// @since 1.6 </returns>
		public virtual LockInfo[] LockedSynchronizers
		{
			get
			{
				return LockedSynchronizers_Renamed;
			}
		}

		private static readonly StackTraceElement[] NO_STACK_TRACE = new StackTraceElement[0];
	}

}