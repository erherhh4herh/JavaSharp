using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{

	using Interruptible = sun.nio.ch.Interruptible;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;
	using SecurityConstants = sun.security.util.SecurityConstants;


	/// <summary>
	/// A <i>thread</i> is a thread of execution in a program. The Java
	/// Virtual Machine allows an application to have multiple threads of
	/// execution running concurrently.
	/// <para>
	/// Every thread has a priority. Threads with higher priority are
	/// executed in preference to threads with lower priority. Each thread
	/// may or may not also be marked as a daemon. When code running in
	/// some thread creates a new <code>Thread</code> object, the new
	/// thread has its priority initially set equal to the priority of the
	/// creating thread, and is a daemon thread if and only if the
	/// creating thread is a daemon.
	/// </para>
	/// <para>
	/// When a Java Virtual Machine starts up, there is usually a single
	/// non-daemon thread (which typically calls the method named
	/// <code>main</code> of some designated class). The Java Virtual
	/// Machine continues to execute threads until either of the following
	/// occurs:
	/// <ul>
	/// <li>The <code>exit</code> method of class <code>Runtime</code> has been
	///     called and the security manager has permitted the exit operation
	///     to take place.
	/// <li>All threads that are not daemon threads have died, either by
	///     returning from the call to the <code>run</code> method or by
	///     throwing an exception that propagates beyond the <code>run</code>
	///     method.
	/// </ul>
	/// </para>
	/// <para>
	/// There are two ways to create a new thread of execution. One is to
	/// declare a class to be a subclass of <code>Thread</code>. This
	/// subclass should override the <code>run</code> method of class
	/// <code>Thread</code>. An instance of the subclass can then be
	/// allocated and started. For example, a thread that computes primes
	/// larger than a stated value could be written as follows:
	/// <hr><blockquote><pre>
	///     class PrimeThread extends Thread {
	///         long minPrime;
	///         PrimeThread(long minPrime) {
	///             this.minPrime = minPrime;
	///         }
	/// 
	///         public void run() {
	///             // compute primes larger than minPrime
	///             &nbsp;.&nbsp;.&nbsp;.
	///         }
	///     }
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// The following code would then create a thread and start it running:
	/// <blockquote><pre>
	///     PrimeThread p = new PrimeThread(143);
	///     p.start();
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// The other way to create a thread is to declare a class that
	/// implements the <code>Runnable</code> interface. That class then
	/// implements the <code>run</code> method. An instance of the class can
	/// then be allocated, passed as an argument when creating
	/// <code>Thread</code>, and started. The same example in this other
	/// style looks like the following:
	/// <hr><blockquote><pre>
	///     class PrimeRun implements Runnable {
	///         long minPrime;
	///         PrimeRun(long minPrime) {
	///             this.minPrime = minPrime;
	///         }
	/// 
	///         public void run() {
	///             // compute primes larger than minPrime
	///             &nbsp;.&nbsp;.&nbsp;.
	///         }
	///     }
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// The following code would then create a thread and start it running:
	/// <blockquote><pre>
	///     PrimeRun p = new PrimeRun(143);
	///     new Thread(p).start();
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// Every thread has a name for identification purposes. More than
	/// one thread may have the same name. If a name is not specified when
	/// a thread is created, a new name is generated for it.
	/// </para>
	/// <para>
	/// Unless otherwise noted, passing a {@code null} argument to a constructor
	/// or method in this class will cause a <seealso cref="NullPointerException"/> to be
	/// thrown.
	/// 
	/// @author  unascribed
	/// </para>
	/// </summary>
	/// <seealso cref=     Runnable </seealso>
	/// <seealso cref=     Runtime#exit(int) </seealso>
	/// <seealso cref=     #run() </seealso>
	/// <seealso cref=     #stop()
	/// @since   JDK1.0 </seealso>
	public class Thread : Runnable
	{
		/* Make sure registerNatives is the first thing <clinit> does. */
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void registerNatives();
		static Thread()
		{
			registerNatives();
		}

		private volatile char[] Name_Renamed;
		private int Priority_Renamed;
		private Thread ThreadQ;
		private long Eetop;

		/* Whether or not to single_step this thread. */
		private bool Single_step;

		/* Whether or not the thread is a daemon thread. */
		private bool Daemon_Renamed = false;

		/* JVM state */
		private bool Stillborn = false;

		/* What will be run. */
		private Runnable Target;

		/* The group of this thread */
		private ThreadGroup Group;

		/* The context ClassLoader for this thread */
		private ClassLoader ContextClassLoader_Renamed;

		/* The inherited AccessControlContext of this thread */
		private AccessControlContext InheritedAccessControlContext;

		/* For autonumbering anonymous threads. */
		private static int ThreadInitNumber;
		private static int NextThreadNum()
		{
			lock (typeof(Thread))
			{
				return ThreadInitNumber++;
			}
		}

		/* ThreadLocal values pertaining to this thread. This map is maintained
		 * by the ThreadLocal class. */
		internal ThreadLocal.ThreadLocalMap ThreadLocals = null;

		/*
		 * InheritableThreadLocal values pertaining to this thread. This map is
		 * maintained by the InheritableThreadLocal class.
		 */
		internal ThreadLocal.ThreadLocalMap InheritableThreadLocals = null;

		/*
		 * The requested stack size for this thread, or 0 if the creator did
		 * not specify a stack size.  It is up to the VM to do whatever it
		 * likes with this number; some VMs will ignore it.
		 */
		private long StackSize;

		/*
		 * JVM-private state that persists after native thread termination.
		 */
		private long NativeParkEventPointer;

		/*
		 * Thread ID
		 */
		private long Tid;

		/* For generating thread ID */
		private static long ThreadSeqNumber;

		/* Java thread status for tools,
		 * initialized to indicate thread 'not yet started'
		 */

		private volatile int ThreadStatus = 0;


		private static long NextThreadID()
		{
			lock (typeof(Thread))
			{
				return ++ThreadSeqNumber;
			}
		}

		/// <summary>
		/// The argument supplied to the current call to
		/// java.util.concurrent.locks.LockSupport.park.
		/// Set by (private) java.util.concurrent.locks.LockSupport.setBlocker
		/// Accessed using java.util.concurrent.locks.LockSupport.getBlocker
		/// </summary>
		internal volatile Object ParkBlocker;

		/* The object in which this thread is blocked in an interruptible I/O
		 * operation, if any.  The blocker's interrupt method should be invoked
		 * after setting this thread's interrupt status.
		 */
		private volatile Interruptible Blocker;
		private readonly Object BlockerLock = new Object();

		/* Set the blocker field; invoked via sun.misc.SharedSecrets from java.nio code
		 */
		internal virtual void BlockedOn(Interruptible b)
		{
			lock (BlockerLock)
			{
				Blocker = b;
			}
		}

		/// <summary>
		/// The minimum priority that a thread can have.
		/// </summary>
		public const int MIN_PRIORITY = 1;

	   /// <summary>
	   /// The default priority that is assigned to a thread.
	   /// </summary>
		public const int NORM_PRIORITY = 5;

		/// <summary>
		/// The maximum priority that a thread can have.
		/// </summary>
		public const int MAX_PRIORITY = 10;

		/// <summary>
		/// Returns a reference to the currently executing thread object.
		/// </summary>
		/// <returns>  the currently executing thread. </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern Thread currentThread();

		/// <summary>
		/// A hint to the scheduler that the current thread is willing to yield
		/// its current use of a processor. The scheduler is free to ignore this
		/// hint.
		/// 
		/// <para> Yield is a heuristic attempt to improve relative progression
		/// between threads that would otherwise over-utilise a CPU. Its use
		/// should be combined with detailed profiling and benchmarking to
		/// ensure that it actually has the desired effect.
		/// 
		/// </para>
		/// <para> It is rarely appropriate to use this method. It may be useful
		/// for debugging or testing purposes, where it may help to reproduce
		/// bugs due to race conditions. It may also be useful when designing
		/// concurrency control constructs such as the ones in the
		/// <seealso cref="java.util.concurrent.locks"/> package.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern void @yield();

		/// <summary>
		/// Causes the currently executing thread to sleep (temporarily cease
		/// execution) for the specified number of milliseconds, subject to
		/// the precision and accuracy of system timers and schedulers. The thread
		/// does not lose ownership of any monitors.
		/// </summary>
		/// <param name="millis">
		///         the length of time to sleep in milliseconds
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          if the value of {@code millis} is negative
		/// </exception>
		/// <exception cref="InterruptedException">
		///          if any thread has interrupted the current thread. The
		///          <i>interrupted status</i> of the current thread is
		///          cleared when this exception is thrown. </exception>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern void sleep(long millis);

		/// <summary>
		/// Causes the currently executing thread to sleep (temporarily cease
		/// execution) for the specified number of milliseconds plus the specified
		/// number of nanoseconds, subject to the precision and accuracy of system
		/// timers and schedulers. The thread does not lose ownership of any
		/// monitors.
		/// </summary>
		/// <param name="millis">
		///         the length of time to sleep in milliseconds
		/// </param>
		/// <param name="nanos">
		///         {@code 0-999999} additional nanoseconds to sleep
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          if the value of {@code millis} is negative, or the value of
		///          {@code nanos} is not in the range {@code 0-999999}
		/// </exception>
		/// <exception cref="InterruptedException">
		///          if any thread has interrupted the current thread. The
		///          <i>interrupted status</i> of the current thread is
		///          cleared when this exception is thrown. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void sleep(long millis, int nanos) throws InterruptedException
		public static void Sleep(long millis, int nanos)
		{
			if (millis < 0)
			{
				throw new IllegalArgumentException("timeout value is negative");
			}

			if (nanos < 0 || nanos > 999999)
			{
				throw new IllegalArgumentException("nanosecond timeout value out of range");
			}

			if (nanos >= 500000 || (nanos != 0 && millis == 0))
			{
				millis++;
			}

			sleep(millis);
		}

		/// <summary>
		/// Initializes a Thread with the current AccessControlContext. </summary>
		/// <seealso cref= #init(ThreadGroup,Runnable,String,long,AccessControlContext) </seealso>
		private void Init(ThreadGroup g, Runnable target, String name, long stackSize)
		{
			Init(g, target, name, stackSize, null);
		}

		/// <summary>
		/// Initializes a Thread.
		/// </summary>
		/// <param name="g"> the Thread group </param>
		/// <param name="target"> the object whose run() method gets called </param>
		/// <param name="name"> the name of the new Thread </param>
		/// <param name="stackSize"> the desired stack size for the new thread, or
		///        zero to indicate that this parameter is to be ignored. </param>
		/// <param name="acc"> the AccessControlContext to inherit, or
		///            AccessController.getContext() if null </param>
		private void Init(ThreadGroup g, Runnable target, String name, long stackSize, AccessControlContext acc)
		{
			if (name == null)
			{
				throw new NullPointerException("name cannot be null");
			}

			this.Name_Renamed = name.ToCharArray();

			Thread parent = currentThread();
			SecurityManager security = System.SecurityManager;
			if (g == null)
			{
				/* Determine if it's an applet or not */

				/* If there is a security manager, ask the security manager
				   what to do. */
				if (security != null)
				{
					g = security.ThreadGroup;
				}

				/* If the security doesn't have a strong opinion of the matter
				   use the parent thread group. */
				if (g == null)
				{
					g = parent.ThreadGroup;
				}
			}

			/* checkAccess regardless of whether or not threadgroup is
			   explicitly passed in. */
			g.CheckAccess();

			/*
			 * Do we have the required permissions?
			 */
			if (security != null)
			{
				if (IsCCLOverridden(this.GetType()))
				{
					security.CheckPermission(SUBCLASS_IMPLEMENTATION_PERMISSION);
				}
			}

			g.AddUnstarted();

			this.Group = g;
			this.Daemon_Renamed = parent.Daemon;
			this.Priority_Renamed = parent.Priority;
			if (security == null || IsCCLOverridden(parent.GetType()))
			{
				this.ContextClassLoader_Renamed = parent.ContextClassLoader;
			}
			else
			{
				this.ContextClassLoader_Renamed = parent.ContextClassLoader_Renamed;
			}
			this.InheritedAccessControlContext = acc != null ? acc : AccessController.Context;
			this.Target = target;
			Priority = Priority_Renamed;
			if (parent.InheritableThreadLocals != null)
			{
				this.InheritableThreadLocals = ThreadLocal.CreateInheritedMap(parent.InheritableThreadLocals);
			}
			/* Stash the specified stack size in case the VM cares */
			this.StackSize = stackSize;

			/* Set thread ID */
			Tid = NextThreadID();
		}

		/// <summary>
		/// Throws CloneNotSupportedException as a Thread can not be meaningfully
		/// cloned. Construct a new Thread instead.
		/// </summary>
		/// <exception cref="CloneNotSupportedException">
		///          always </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected Object clone() throws CloneNotSupportedException
		protected internal override Object Clone()
		{
			throw new CloneNotSupportedException();
		}

		/// <summary>
		/// Allocates a new {@code Thread} object. This constructor has the same
		/// effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		/// {@code (null, null, gname)}, where {@code gname} is a newly generated
		/// name. Automatically generated names are of the form
		/// {@code "Thread-"+}<i>n</i>, where <i>n</i> is an integer.
		/// </summary>
		public Thread()
		{
			Init(null, null, "Thread-" + NextThreadNum(), 0);
		}

		/// <summary>
		/// Allocates a new {@code Thread} object. This constructor has the same
		/// effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		/// {@code (null, target, gname)}, where {@code gname} is a newly generated
		/// name. Automatically generated names are of the form
		/// {@code "Thread-"+}<i>n</i>, where <i>n</i> is an integer.
		/// </summary>
		/// <param name="target">
		///         the object whose {@code run} method is invoked when this thread
		///         is started. If {@code null}, this classes {@code run} method does
		///         nothing. </param>
		public Thread(Runnable target)
		{
			Init(null, target, "Thread-" + NextThreadNum(), 0);
		}

		/// <summary>
		/// Creates a new Thread that inherits the given AccessControlContext.
		/// This is not a public constructor.
		/// </summary>
		internal Thread(Runnable target, AccessControlContext acc)
		{
			Init(null, target, "Thread-" + NextThreadNum(), 0, acc);
		}

		/// <summary>
		/// Allocates a new {@code Thread} object. This constructor has the same
		/// effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		/// {@code (group, target, gname)} ,where {@code gname} is a newly generated
		/// name. Automatically generated names are of the form
		/// {@code "Thread-"+}<i>n</i>, where <i>n</i> is an integer.
		/// </summary>
		/// <param name="group">
		///         the thread group. If {@code null} and there is a security
		///         manager, the group is determined by {@linkplain
		///         SecurityManager#getThreadGroup SecurityManager.getThreadGroup()}.
		///         If there is not a security manager or {@code
		///         SecurityManager.getThreadGroup()} returns {@code null}, the group
		///         is set to the current thread's thread group.
		/// </param>
		/// <param name="target">
		///         the object whose {@code run} method is invoked when this thread
		///         is started. If {@code null}, this thread's run method is invoked.
		/// </param>
		/// <exception cref="SecurityException">
		///          if the current thread cannot create a thread in the specified
		///          thread group </exception>
		public Thread(ThreadGroup group, Runnable target)
		{
			Init(group, target, "Thread-" + NextThreadNum(), 0);
		}

		/// <summary>
		/// Allocates a new {@code Thread} object. This constructor has the same
		/// effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		/// {@code (null, null, name)}.
		/// </summary>
		/// <param name="name">
		///          the name of the new thread </param>
		public Thread(String name)
		{
			Init(null, null, name, 0);
		}

		/// <summary>
		/// Allocates a new {@code Thread} object. This constructor has the same
		/// effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		/// {@code (group, null, name)}.
		/// </summary>
		/// <param name="group">
		///         the thread group. If {@code null} and there is a security
		///         manager, the group is determined by {@linkplain
		///         SecurityManager#getThreadGroup SecurityManager.getThreadGroup()}.
		///         If there is not a security manager or {@code
		///         SecurityManager.getThreadGroup()} returns {@code null}, the group
		///         is set to the current thread's thread group.
		/// </param>
		/// <param name="name">
		///         the name of the new thread
		/// </param>
		/// <exception cref="SecurityException">
		///          if the current thread cannot create a thread in the specified
		///          thread group </exception>
		public Thread(ThreadGroup group, String name)
		{
			Init(group, null, name, 0);
		}

		/// <summary>
		/// Allocates a new {@code Thread} object. This constructor has the same
		/// effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		/// {@code (null, target, name)}.
		/// </summary>
		/// <param name="target">
		///         the object whose {@code run} method is invoked when this thread
		///         is started. If {@code null}, this thread's run method is invoked.
		/// </param>
		/// <param name="name">
		///         the name of the new thread </param>
		public Thread(Runnable target, String name)
		{
			Init(null, target, name, 0);
		}

		/// <summary>
		/// Allocates a new {@code Thread} object so that it has {@code target}
		/// as its run object, has the specified {@code name} as its name,
		/// and belongs to the thread group referred to by {@code group}.
		/// 
		/// <para>If there is a security manager, its
		/// <seealso cref="SecurityManager#checkAccess(ThreadGroup) checkAccess"/>
		/// method is invoked with the ThreadGroup as its argument.
		/// 
		/// </para>
		/// <para>In addition, its {@code checkPermission} method is invoked with
		/// the {@code RuntimePermission("enableContextClassLoaderOverride")}
		/// permission when invoked directly or indirectly by the constructor
		/// of a subclass which overrides the {@code getContextClassLoader}
		/// or {@code setContextClassLoader} methods.
		/// 
		/// </para>
		/// <para>The priority of the newly created thread is set equal to the
		/// priority of the thread creating it, that is, the currently running
		/// thread. The method <seealso cref="#setPriority setPriority"/> may be
		/// used to change the priority to a new value.
		/// 
		/// </para>
		/// <para>The newly created thread is initially marked as being a daemon
		/// thread if and only if the thread creating it is currently marked
		/// as a daemon thread. The method <seealso cref="#setDaemon setDaemon"/>
		/// may be used to change whether or not a thread is a daemon.
		/// 
		/// </para>
		/// </summary>
		/// <param name="group">
		///         the thread group. If {@code null} and there is a security
		///         manager, the group is determined by {@linkplain
		///         SecurityManager#getThreadGroup SecurityManager.getThreadGroup()}.
		///         If there is not a security manager or {@code
		///         SecurityManager.getThreadGroup()} returns {@code null}, the group
		///         is set to the current thread's thread group.
		/// </param>
		/// <param name="target">
		///         the object whose {@code run} method is invoked when this thread
		///         is started. If {@code null}, this thread's run method is invoked.
		/// </param>
		/// <param name="name">
		///         the name of the new thread
		/// </param>
		/// <exception cref="SecurityException">
		///          if the current thread cannot create a thread in the specified
		///          thread group or cannot override the context class loader methods. </exception>
		public Thread(ThreadGroup group, Runnable target, String name)
		{
			Init(group, target, name, 0);
		}

		/// <summary>
		/// Allocates a new {@code Thread} object so that it has {@code target}
		/// as its run object, has the specified {@code name} as its name,
		/// and belongs to the thread group referred to by {@code group}, and has
		/// the specified <i>stack size</i>.
		/// 
		/// <para>This constructor is identical to {@link
		/// #Thread(ThreadGroup,Runnable,String)} with the exception of the fact
		/// that it allows the thread stack size to be specified.  The stack size
		/// is the approximate number of bytes of address space that the virtual
		/// machine is to allocate for this thread's stack.  <b>The effect of the
		/// {@code stackSize} parameter, if any, is highly platform dependent.</b>
		/// 
		/// </para>
		/// <para>On some platforms, specifying a higher value for the
		/// {@code stackSize} parameter may allow a thread to achieve greater
		/// recursion depth before throwing a <seealso cref="StackOverflowError"/>.
		/// Similarly, specifying a lower value may allow a greater number of
		/// threads to exist concurrently without throwing an {@link
		/// OutOfMemoryError} (or other internal error).  The details of
		/// the relationship between the value of the <tt>stackSize</tt> parameter
		/// and the maximum recursion depth and concurrency level are
		/// platform-dependent.  <b>On some platforms, the value of the
		/// {@code stackSize} parameter may have no effect whatsoever.</b>
		/// 
		/// </para>
		/// <para>The virtual machine is free to treat the {@code stackSize}
		/// parameter as a suggestion.  If the specified value is unreasonably low
		/// for the platform, the virtual machine may instead use some
		/// platform-specific minimum value; if the specified value is unreasonably
		/// high, the virtual machine may instead use some platform-specific
		/// maximum.  Likewise, the virtual machine is free to round the specified
		/// value up or down as it sees fit (or to ignore it completely).
		/// 
		/// </para>
		/// <para>Specifying a value of zero for the {@code stackSize} parameter will
		/// cause this constructor to behave exactly like the
		/// {@code Thread(ThreadGroup, Runnable, String)} constructor.
		/// 
		/// </para>
		/// <para><i>Due to the platform-dependent nature of the behavior of this
		/// constructor, extreme care should be exercised in its use.
		/// The thread stack size necessary to perform a given computation will
		/// likely vary from one JRE implementation to another.  In light of this
		/// variation, careful tuning of the stack size parameter may be required,
		/// and the tuning may need to be repeated for each JRE implementation on
		/// which an application is to run.</i>
		/// 
		/// </para>
		/// <para>Implementation note: Java platform implementers are encouraged to
		/// document their implementation's behavior with respect to the
		/// {@code stackSize} parameter.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="group">
		///         the thread group. If {@code null} and there is a security
		///         manager, the group is determined by {@linkplain
		///         SecurityManager#getThreadGroup SecurityManager.getThreadGroup()}.
		///         If there is not a security manager or {@code
		///         SecurityManager.getThreadGroup()} returns {@code null}, the group
		///         is set to the current thread's thread group.
		/// </param>
		/// <param name="target">
		///         the object whose {@code run} method is invoked when this thread
		///         is started. If {@code null}, this thread's run method is invoked.
		/// </param>
		/// <param name="name">
		///         the name of the new thread
		/// </param>
		/// <param name="stackSize">
		///         the desired stack size for the new thread, or zero to indicate
		///         that this parameter is to be ignored.
		/// </param>
		/// <exception cref="SecurityException">
		///          if the current thread cannot create a thread in the specified
		///          thread group
		/// 
		/// @since 1.4 </exception>
		public Thread(ThreadGroup group, Runnable target, String name, long stackSize)
		{
			Init(group, target, name, stackSize);
		}

		/// <summary>
		/// Causes this thread to begin execution; the Java Virtual Machine
		/// calls the <code>run</code> method of this thread.
		/// <para>
		/// The result is that two threads are running concurrently: the
		/// current thread (which returns from the call to the
		/// <code>start</code> method) and the other thread (which executes its
		/// <code>run</code> method).
		/// </para>
		/// <para>
		/// It is never legal to start a thread more than once.
		/// In particular, a thread may not be restarted once it has completed
		/// execution.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IllegalThreadStateException">  if the thread was already
		///               started. </exception>
		/// <seealso cref=        #run() </seealso>
		/// <seealso cref=        #stop() </seealso>
		public virtual void Start()
		{
			lock (this)
			{
				/// <summary>
				/// This method is not invoked for the main method thread or "system"
				/// group threads created/set up by the VM. Any new functionality added
				/// to this method in the future may have to also be added to the VM.
				/// 
				/// A zero status value corresponds to state "NEW".
				/// </summary>
				if (ThreadStatus != 0)
				{
					throw new IllegalThreadStateException();
				}
        
				/* Notify the group that this thread is about to be started
				 * so that it can be added to the group's list of threads
				 * and the group's unstarted count can be decremented. */
				Group.Add(this);
        
				bool started = false;
				try
				{
					start0();
					started = true;
				}
				finally
				{
					try
					{
						if (!started)
						{
							Group.ThreadStartFailed(this);
						}
					}
					catch (Throwable)
					{
						/* do nothing. If start0 threw a Throwable then
						  it will be passed up the call stack */
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void start0();

		/// <summary>
		/// If this thread was constructed using a separate
		/// <code>Runnable</code> run object, then that
		/// <code>Runnable</code> object's <code>run</code> method is called;
		/// otherwise, this method does nothing and returns.
		/// <para>
		/// Subclasses of <code>Thread</code> should override this method.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     #start() </seealso>
		/// <seealso cref=     #stop() </seealso>
		/// <seealso cref=     #Thread(ThreadGroup, Runnable, String) </seealso>
		public virtual void Run()
		{
			if (Target != null)
			{
				Target.Run();
			}
		}

		/// <summary>
		/// This method is called by the system to give a Thread
		/// a chance to clean up before it actually exits.
		/// </summary>
		private void Exit()
		{
			if (Group != null)
			{
				Group.ThreadTerminated(this);
				Group = null;
			}
			/* Aggressively null out all reference fields: see bug 4006245 */
			Target = null;
			/* Speed the release of some of these resources */
			ThreadLocals = null;
			InheritableThreadLocals = null;
			InheritedAccessControlContext = null;
			Blocker = null;
			UncaughtExceptionHandler_Renamed = null;
		}

		/// <summary>
		/// Forces the thread to stop executing.
		/// <para>
		/// If there is a security manager installed, its <code>checkAccess</code>
		/// method is called with <code>this</code>
		/// as its argument. This may result in a
		/// <code>SecurityException</code> being raised (in the current thread).
		/// </para>
		/// <para>
		/// If this thread is different from the current thread (that is, the current
		/// thread is trying to stop a thread other than itself), the
		/// security manager's <code>checkPermission</code> method (with a
		/// <code>RuntimePermission("stopThread")</code> argument) is called in
		/// addition.
		/// Again, this may result in throwing a
		/// <code>SecurityException</code> (in the current thread).
		/// </para>
		/// <para>
		/// The thread represented by this thread is forced to stop whatever
		/// it is doing abnormally and to throw a newly created
		/// <code>ThreadDeath</code> object as an exception.
		/// </para>
		/// <para>
		/// It is permitted to stop a thread that has not yet been started.
		/// If the thread is eventually started, it immediately terminates.
		/// </para>
		/// <para>
		/// An application should not normally try to catch
		/// <code>ThreadDeath</code> unless it must do some extraordinary
		/// cleanup operation (note that the throwing of
		/// <code>ThreadDeath</code> causes <code>finally</code> clauses of
		/// <code>try</code> statements to be executed before the thread
		/// officially dies).  If a <code>catch</code> clause catches a
		/// <code>ThreadDeath</code> object, it is important to rethrow the
		/// object so that the thread actually dies.
		/// </para>
		/// <para>
		/// The top-level error handler that reacts to otherwise uncaught
		/// exceptions does not print out a message or otherwise notify the
		/// application if the uncaught exception is an instance of
		/// <code>ThreadDeath</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the current thread cannot
		///               modify this thread. </exception>
		/// <seealso cref=        #interrupt() </seealso>
		/// <seealso cref=        #checkAccess() </seealso>
		/// <seealso cref=        #run() </seealso>
		/// <seealso cref=        #start() </seealso>
		/// <seealso cref=        ThreadDeath </seealso>
		/// <seealso cref=        ThreadGroup#uncaughtException(Thread,Throwable) </seealso>
		/// <seealso cref=        SecurityManager#checkAccess(Thread) </seealso>
		/// <seealso cref=        SecurityManager#checkPermission </seealso>
		/// @deprecated This method is inherently unsafe.  Stopping a thread with
		///       Thread.stop causes it to unlock all of the monitors that it
		///       has locked (as a natural consequence of the unchecked
		///       <code>ThreadDeath</code> exception propagating up the stack).  If
		///       any of the objects previously protected by these monitors were in
		///       an inconsistent state, the damaged objects become visible to
		///       other threads, potentially resulting in arbitrary behavior.  Many
		///       uses of <code>stop</code> should be replaced by code that simply
		///       modifies some variable to indicate that the target thread should
		///       stop running.  The target thread should check this variable
		///       regularly, and return from its run method in an orderly fashion
		///       if the variable indicates that it is to stop running.  If the
		///       target thread waits for long periods (on a condition variable,
		///       for example), the <code>interrupt</code> method should be used to
		///       interrupt the wait.
		///       For more information, see
		///       <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">Why
		///       are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		[Obsolete("This method is inherently unsafe.  Stopping a thread with")]
		public void Stop()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				CheckAccess();
				if (this != Thread.CurrentThread)
				{
					security.CheckPermission(SecurityConstants.STOP_THREAD_PERMISSION);
				}
			}
			// A zero status value corresponds to "NEW", it can't change to
			// not-NEW because we hold the lock.
			if (ThreadStatus != 0)
			{
				Resume(); // Wake up thread if it was suspended; no-op otherwise
			}

			// The VM can handle all thread states
			stop0(new ThreadDeath());
		}

		/// <summary>
		/// Throws {@code UnsupportedOperationException}.
		/// </summary>
		/// <param name="obj"> ignored
		/// </param>
		/// @deprecated This method was originally designed to force a thread to stop
		///        and throw a given {@code Throwable} as an exception. It was
		///        inherently unsafe (see <seealso cref="#stop()"/> for details), and furthermore
		///        could be used to generate exceptions that the target thread was
		///        not prepared to handle.
		///        For more information, see
		///        <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">Why
		///        are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		[Obsolete("This method was originally designed to force a thread to stop")]
		public void Stop(Throwable obj)
		{
			lock (this)
			{
				throw new UnsupportedOperationException();
			}
		}

		/// <summary>
		/// Interrupts this thread.
		/// 
		/// <para> Unless the current thread is interrupting itself, which is
		/// always permitted, the <seealso cref="#checkAccess() checkAccess"/> method
		/// of this thread is invoked, which may cause a {@link
		/// SecurityException} to be thrown.
		/// 
		/// </para>
		/// <para> If this thread is blocked in an invocation of the {@link
		/// Object#wait() wait()}, <seealso cref="Object#wait(long) wait(long)"/>, or {@link
		/// Object#wait(long, int) wait(long, int)} methods of the <seealso cref="Object"/>
		/// class, or of the <seealso cref="#join()"/>, <seealso cref="#join(long)"/>, {@link
		/// #join(long, int)}, <seealso cref="#sleep(long)"/>, or <seealso cref="#sleep(long, int)"/>,
		/// methods of this class, then its interrupt status will be cleared and it
		/// will receive an <seealso cref="InterruptedException"/>.
		/// 
		/// </para>
		/// <para> If this thread is blocked in an I/O operation upon an {@link
		/// java.nio.channels.InterruptibleChannel InterruptibleChannel}
		/// then the channel will be closed, the thread's interrupt
		/// status will be set, and the thread will receive a {@link
		/// java.nio.channels.ClosedByInterruptException}.
		/// 
		/// </para>
		/// <para> If this thread is blocked in a <seealso cref="java.nio.channels.Selector"/>
		/// then the thread's interrupt status will be set and it will return
		/// immediately from the selection operation, possibly with a non-zero
		/// value, just as if the selector's {@link
		/// java.nio.channels.Selector#wakeup wakeup} method were invoked.
		/// 
		/// </para>
		/// <para> If none of the previous conditions hold then this thread's interrupt
		/// status will be set. </para>
		/// 
		/// <para> Interrupting a thread that is not alive need not have any effect.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///          if the current thread cannot modify this thread
		/// 
		/// @revised 6.0
		/// @spec JSR-51 </exception>
		public virtual void Interrupt()
		{
			if (this != Thread.CurrentThread)
			{
				CheckAccess();
			}

			lock (BlockerLock)
			{
				Interruptible b = Blocker;
				if (b != null)
				{
					interrupt0(); // Just to set the interrupt flag
					b.interrupt(this);
					return;
				}
			}
			interrupt0();
		}

		/// <summary>
		/// Tests whether the current thread has been interrupted.  The
		/// <i>interrupted status</i> of the thread is cleared by this method.  In
		/// other words, if this method were to be called twice in succession, the
		/// second call would return false (unless the current thread were
		/// interrupted again, after the first call had cleared its interrupted
		/// status and before the second call had examined it).
		/// 
		/// <para>A thread interruption ignored because a thread was not alive
		/// at the time of the interrupt will be reflected by this method
		/// returning false.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  <code>true</code> if the current thread has been interrupted;
		///          <code>false</code> otherwise. </returns>
		/// <seealso cref= #isInterrupted()
		/// @revised 6.0 </seealso>
		public static bool Interrupted()
		{
			return currentThread().isInterrupted(true);
		}

		/// <summary>
		/// Tests whether this thread has been interrupted.  The <i>interrupted
		/// status</i> of the thread is unaffected by this method.
		/// 
		/// <para>A thread interruption ignored because a thread was not alive
		/// at the time of the interrupt will be reflected by this method
		/// returning false.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  <code>true</code> if this thread has been interrupted;
		///          <code>false</code> otherwise. </returns>
		/// <seealso cref=     #interrupted()
		/// @revised 6.0 </seealso>
		public virtual bool Interrupted
		{
			get
			{
				return isInterrupted(false);
			}
		}

		/// <summary>
		/// Tests if some Thread has been interrupted.  The interrupted state
		/// is reset or not based on the value of ClearInterrupted that is
		/// passed.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern boolean isInterrupted(bool ClearInterrupted);

		/// <summary>
		/// Throws <seealso cref="NoSuchMethodError"/>.
		/// </summary>
		/// @deprecated This method was originally designed to destroy this
		///     thread without any cleanup. Any monitors it held would have
		///     remained locked. However, the method was never implemented.
		///     If if were to be implemented, it would be deadlock-prone in
		///     much the manner of <seealso cref="#suspend"/>. If the target thread held
		///     a lock protecting a critical system resource when it was
		///     destroyed, no thread could ever access this resource again.
		///     If another thread ever attempted to lock this resource, deadlock
		///     would result. Such deadlocks typically manifest themselves as
		///     "frozen" processes. For more information, see
		///     <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">
		///     Why are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		/// <exception cref="NoSuchMethodError"> always </exception>
		[Obsolete("This method was originally designed to destroy this")]
		public virtual void Destroy()
		{
			throw new NoSuchMethodError();
		}

		/// <summary>
		/// Tests if this thread is alive. A thread is alive if it has
		/// been started and has not yet died.
		/// </summary>
		/// <returns>  <code>true</code> if this thread is alive;
		///          <code>false</code> otherwise. </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public final extern boolean isAlive();

		/// <summary>
		/// Suspends this thread.
		/// <para>
		/// First, the <code>checkAccess</code> method of this thread is called
		/// with no arguments. This may result in throwing a
		/// <code>SecurityException </code>(in the current thread).
		/// </para>
		/// <para>
		/// If the thread is alive, it is suspended and makes no further
		/// progress unless and until it is resumed.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the current thread cannot modify
		///               this thread. </exception>
		/// <seealso cref= #checkAccess </seealso>
		/// @deprecated   This method has been deprecated, as it is
		///   inherently deadlock-prone.  If the target thread holds a lock on the
		///   monitor protecting a critical system resource when it is suspended, no
		///   thread can access this resource until the target thread is resumed. If
		///   the thread that would resume the target thread attempts to lock this
		///   monitor prior to calling <code>resume</code>, deadlock results.  Such
		///   deadlocks typically manifest themselves as "frozen" processes.
		///   For more information, see
		///   <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">Why
		///   are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		[Obsolete("  This method has been deprecated, as it is")]
		public void Suspend()
		{
			CheckAccess();
			suspend0();
		}

		/// <summary>
		/// Resumes a suspended thread.
		/// <para>
		/// First, the <code>checkAccess</code> method of this thread is called
		/// with no arguments. This may result in throwing a
		/// <code>SecurityException</code> (in the current thread).
		/// </para>
		/// <para>
		/// If the thread is alive but suspended, it is resumed and is
		/// permitted to make progress in its execution.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the current thread cannot modify this
		///               thread. </exception>
		/// <seealso cref=        #checkAccess </seealso>
		/// <seealso cref=        #suspend() </seealso>
		/// @deprecated This method exists solely for use with <seealso cref="#suspend"/>,
		///     which has been deprecated because it is deadlock-prone.
		///     For more information, see
		///     <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">Why
		///     are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		[Obsolete("This method exists solely for use with <seealso cref="#suspend"/>,")]
		public void Resume()
		{
			CheckAccess();
			resume0();
		}

		/// <summary>
		/// Changes the priority of this thread.
		/// <para>
		/// First the <code>checkAccess</code> method of this thread is called
		/// with no arguments. This may result in throwing a
		/// <code>SecurityException</code>.
		/// </para>
		/// <para>
		/// Otherwise, the priority of this thread is set to the smaller of
		/// the specified <code>newPriority</code> and the maximum permitted
		/// priority of the thread's thread group.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newPriority"> priority to set this thread to </param>
		/// <exception cref="IllegalArgumentException">  If the priority is not in the
		///               range <code>MIN_PRIORITY</code> to
		///               <code>MAX_PRIORITY</code>. </exception>
		/// <exception cref="SecurityException">  if the current thread cannot modify
		///               this thread. </exception>
		/// <seealso cref=        #getPriority </seealso>
		/// <seealso cref=        #checkAccess() </seealso>
		/// <seealso cref=        #getThreadGroup() </seealso>
		/// <seealso cref=        #MAX_PRIORITY </seealso>
		/// <seealso cref=        #MIN_PRIORITY </seealso>
		/// <seealso cref=        ThreadGroup#getMaxPriority() </seealso>
		public int Priority
		{
			set
			{
				ThreadGroup g;
				CheckAccess();
				if (value > MAX_PRIORITY || value < MIN_PRIORITY)
				{
					throw new IllegalArgumentException();
				}
				if ((g = ThreadGroup) != null)
				{
					if (value > g.MaxPriority)
					{
						value = g.MaxPriority;
					}
					Priority0 = Priority_Renamed = value;
				}
			}
			get
			{
				return Priority_Renamed;
			}
		}


		/// <summary>
		/// Changes the name of this thread to be equal to the argument
		/// <code>name</code>.
		/// <para>
		/// First the <code>checkAccess</code> method of this thread is called
		/// with no arguments. This may result in throwing a
		/// <code>SecurityException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">   the new name for this thread. </param>
		/// <exception cref="SecurityException">  if the current thread cannot modify this
		///               thread. </exception>
		/// <seealso cref=        #getName </seealso>
		/// <seealso cref=        #checkAccess() </seealso>
		public String Name
		{
			set
			{
				lock (this)
				{
					CheckAccess();
					this.Name_Renamed = value.ToCharArray();
					if (ThreadStatus != 0)
					{
						NativeName = value;
					}
				}
			}
			get
			{
				return new String(Name_Renamed, true);
			}
		}


		/// <summary>
		/// Returns the thread group to which this thread belongs.
		/// This method returns null if this thread has died
		/// (been stopped).
		/// </summary>
		/// <returns>  this thread's thread group. </returns>
		public ThreadGroup ThreadGroup
		{
			get
			{
				return Group;
			}
		}

		/// <summary>
		/// Returns an estimate of the number of active threads in the current
		/// thread's <seealso cref="java.lang.ThreadGroup thread group"/> and its
		/// subgroups. Recursively iterates over all subgroups in the current
		/// thread's thread group.
		/// 
		/// <para> The value returned is only an estimate because the number of
		/// threads may change dynamically while this method traverses internal
		/// data structures, and might be affected by the presence of certain
		/// system threads. This method is intended primarily for debugging
		/// and monitoring purposes.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  an estimate of the number of active threads in the current
		///          thread's thread group and in any other thread group that
		///          has the current thread's thread group as an ancestor </returns>
		public static int ActiveCount()
		{
			return currentThread().ThreadGroup.activeCount();
		}

		/// <summary>
		/// Copies into the specified array every active thread in the current
		/// thread's thread group and its subgroups. This method simply
		/// invokes the <seealso cref="java.lang.ThreadGroup#enumerate(Thread[])"/>
		/// method of the current thread's thread group.
		/// 
		/// <para> An application might use the <seealso cref="#activeCount activeCount"/>
		/// method to get an estimate of how big the array should be, however
		/// <i>if the array is too short to hold all the threads, the extra threads
		/// are silently ignored.</i>  If it is critical to obtain every active
		/// thread in the current thread's thread group and its subgroups, the
		/// invoker should verify that the returned int value is strictly less
		/// than the length of {@code tarray}.
		/// 
		/// </para>
		/// <para> Due to the inherent race condition in this method, it is recommended
		/// that the method only be used for debugging and monitoring purposes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="tarray">
		///         an array into which to put the list of threads
		/// </param>
		/// <returns>  the number of threads put into the array
		/// </returns>
		/// <exception cref="SecurityException">
		///          if <seealso cref="java.lang.ThreadGroup#checkAccess"/> determines that
		///          the current thread cannot access its thread group </exception>
		public static int Enumerate(Thread[] tarray)
		{
			return currentThread().ThreadGroup.enumerate(tarray);
		}

		/// <summary>
		/// Counts the number of stack frames in this thread. The thread must
		/// be suspended.
		/// </summary>
		/// <returns>     the number of stack frames in this thread. </returns>
		/// <exception cref="IllegalThreadStateException">  if this thread is not
		///             suspended. </exception>
		/// @deprecated The definition of this call depends on <seealso cref="#suspend"/>,
		///             which is deprecated.  Further, the results of this call
		///             were never well-defined. 
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern int countStackFrames();

		/// <summary>
		/// Waits at most {@code millis} milliseconds for this thread to
		/// die. A timeout of {@code 0} means to wait forever.
		/// 
		/// <para> This implementation uses a loop of {@code this.wait} calls
		/// conditioned on {@code this.isAlive}. As a thread terminates the
		/// {@code this.notifyAll} method is invoked. It is recommended that
		/// applications not use {@code wait}, {@code notify}, or
		/// {@code notifyAll} on {@code Thread} instances.
		/// 
		/// </para>
		/// </summary>
		/// <param name="millis">
		///         the time to wait in milliseconds
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          if the value of {@code millis} is negative
		/// </exception>
		/// <exception cref="InterruptedException">
		///          if any thread has interrupted the current thread. The
		///          <i>interrupted status</i> of the current thread is
		///          cleared when this exception is thrown. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final synchronized void join(long millis) throws InterruptedException
		public void Join(long millis)
		{
			lock (this)
			{
				long @base = DateTimeHelperClass.CurrentUnixTimeMillis();
				long now = 0;
        
				if (millis < 0)
				{
					throw new IllegalArgumentException("timeout value is negative");
				}
        
				if (millis == 0)
				{
					while (Alive)
					{
						Monitor.Wait(this, TimeSpan.FromMilliseconds(0));
					}
				}
				else
				{
					while (Alive)
					{
						long delay = millis - now;
						if (delay <= 0)
						{
							break;
						}
						Monitor.Wait(this, TimeSpan.FromMilliseconds(delay));
						now = DateTimeHelperClass.CurrentUnixTimeMillis() - @base;
					}
				}
			}
		}

		/// <summary>
		/// Waits at most {@code millis} milliseconds plus
		/// {@code nanos} nanoseconds for this thread to die.
		/// 
		/// <para> This implementation uses a loop of {@code this.wait} calls
		/// conditioned on {@code this.isAlive}. As a thread terminates the
		/// {@code this.notifyAll} method is invoked. It is recommended that
		/// applications not use {@code wait}, {@code notify}, or
		/// {@code notifyAll} on {@code Thread} instances.
		/// 
		/// </para>
		/// </summary>
		/// <param name="millis">
		///         the time to wait in milliseconds
		/// </param>
		/// <param name="nanos">
		///         {@code 0-999999} additional nanoseconds to wait
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          if the value of {@code millis} is negative, or the value
		///          of {@code nanos} is not in the range {@code 0-999999}
		/// </exception>
		/// <exception cref="InterruptedException">
		///          if any thread has interrupted the current thread. The
		///          <i>interrupted status</i> of the current thread is
		///          cleared when this exception is thrown. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final synchronized void join(long millis, int nanos) throws InterruptedException
		public void Join(long millis, int nanos)
		{
			lock (this)
			{
        
				if (millis < 0)
				{
					throw new IllegalArgumentException("timeout value is negative");
				}
        
				if (nanos < 0 || nanos > 999999)
				{
					throw new IllegalArgumentException("nanosecond timeout value out of range");
				}
        
				if (nanos >= 500000 || (nanos != 0 && millis == 0))
				{
					millis++;
				}
        
				Join(millis);
			}
		}

		/// <summary>
		/// Waits for this thread to die.
		/// 
		/// <para> An invocation of this method behaves in exactly the same
		/// way as the invocation
		/// 
		/// <blockquote>
		/// <seealso cref="#join(long) join"/>{@code (0)}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="InterruptedException">
		///          if any thread has interrupted the current thread. The
		///          <i>interrupted status</i> of the current thread is
		///          cleared when this exception is thrown. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void join() throws InterruptedException
		public void Join()
		{
			Join(0);
		}

		/// <summary>
		/// Prints a stack trace of the current thread to the standard error stream.
		/// This method is used only for debugging.
		/// </summary>
		/// <seealso cref=     Throwable#printStackTrace() </seealso>
		public static void DumpStack()
		{
			(new Exception("Stack trace")).PrintStackTrace();
		}

		/// <summary>
		/// Marks this thread as either a <seealso cref="#isDaemon daemon"/> thread
		/// or a user thread. The Java Virtual Machine exits when the only
		/// threads running are all daemon threads.
		/// 
		/// <para> This method must be invoked before the thread is started.
		/// 
		/// </para>
		/// </summary>
		/// <param name="on">
		///         if {@code true}, marks this thread as a daemon thread
		/// </param>
		/// <exception cref="IllegalThreadStateException">
		///          if this thread is <seealso cref="#isAlive alive"/>
		/// </exception>
		/// <exception cref="SecurityException">
		///          if <seealso cref="#checkAccess"/> determines that the current
		///          thread cannot modify this thread </exception>
		public bool Daemon
		{
			set
			{
				CheckAccess();
				if (Alive)
				{
					throw new IllegalThreadStateException();
				}
				Daemon_Renamed = value;
			}
			get
			{
				return Daemon_Renamed;
			}
		}


		/// <summary>
		/// Determines if the currently running thread has permission to
		/// modify this thread.
		/// <para>
		/// If there is a security manager, its <code>checkAccess</code> method
		/// is called with this thread as its argument. This may result in
		/// throwing a <code>SecurityException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if the current thread is not allowed to
		///               access this thread. </exception>
		/// <seealso cref=        SecurityManager#checkAccess(Thread) </seealso>
		public void CheckAccess()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckAccess(this);
			}
		}

		/// <summary>
		/// Returns a string representation of this thread, including the
		/// thread's name, priority, and thread group.
		/// </summary>
		/// <returns>  a string representation of this thread. </returns>
		public override String ToString()
		{
			ThreadGroup group = ThreadGroup;
			if (group != null)
			{
				return "Thread[" + Name + "," + Priority + "," + group.Name + "]";
			}
			else
			{
				return "Thread[" + Name + "," + Priority + "," + "" + "]";
			}
		}

		/// <summary>
		/// Returns the context ClassLoader for this Thread. The context
		/// ClassLoader is provided by the creator of the thread for use
		/// by code running in this thread when loading classes and resources.
		/// If not <seealso cref="#setContextClassLoader set"/>, the default is the
		/// ClassLoader context of the parent Thread. The context ClassLoader of the
		/// primordial thread is typically set to the class loader used to load the
		/// application.
		/// 
		/// <para>If a security manager is present, and the invoker's class loader is not
		/// {@code null} and is not the same as or an ancestor of the context class
		/// loader, then this method invokes the security manager's {@link
		/// SecurityManager#checkPermission(java.security.Permission) checkPermission}
		/// method with a <seealso cref="RuntimePermission RuntimePermission"/>{@code
		/// ("getClassLoader")} permission to verify that retrieval of the context
		/// class loader is permitted.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the context ClassLoader for this Thread, or {@code null}
		///          indicating the system class loader (or, failing that, the
		///          bootstrap class loader)
		/// </returns>
		/// <exception cref="SecurityException">
		///          if the current thread cannot get the context ClassLoader
		/// 
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public ClassLoader getContextClassLoader()
		public virtual ClassLoader ContextClassLoader
		{
			get
			{
				if (ContextClassLoader_Renamed == null)
				{
					return null;
				}
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					ClassLoader.CheckClassLoaderPermission(ContextClassLoader_Renamed, Reflection.CallerClass);
				}
				return ContextClassLoader_Renamed;
			}
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(new RuntimePermission("setContextClassLoader"));
				}
				ContextClassLoader_Renamed = value;
			}
		}


		/// <summary>
		/// Returns <tt>true</tt> if and only if the current thread holds the
		/// monitor lock on the specified object.
		/// 
		/// <para>This method is designed to allow a program to assert that
		/// the current thread already holds a specified lock:
		/// <pre>
		///     assert Thread.holdsLock(obj);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> the object on which to test lock ownership </param>
		/// <exception cref="NullPointerException"> if obj is <tt>null</tt> </exception>
		/// <returns> <tt>true</tt> if the current thread holds the monitor lock on
		///         the specified object.
		/// @since 1.4 </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public static extern boolean holdsLock(Object obj);

		private static readonly StackTraceElement[] EMPTY_STACK_TRACE = new StackTraceElement[0];

		/// <summary>
		/// Returns an array of stack trace elements representing the stack dump
		/// of this thread.  This method will return a zero-length array if
		/// this thread has not started, has started but has not yet been
		/// scheduled to run by the system, or has terminated.
		/// If the returned array is of non-zero length then the first element of
		/// the array represents the top of the stack, which is the most recent
		/// method invocation in the sequence.  The last element of the array
		/// represents the bottom of the stack, which is the least recent method
		/// invocation in the sequence.
		/// 
		/// <para>If there is a security manager, and this thread is not
		/// the current thread, then the security manager's
		/// <tt>checkPermission</tt> method is called with a
		/// <tt>RuntimePermission("getStackTrace")</tt> permission
		/// to see if it's ok to get the stack trace.
		/// 
		/// </para>
		/// <para>Some virtual machines may, under some circumstances, omit one
		/// or more stack frames from the stack trace.  In the extreme case,
		/// a virtual machine that has no stack trace information concerning
		/// this thread is permitted to return a zero-length array from this
		/// method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of <tt>StackTraceElement</tt>,
		/// each represents one stack frame.
		/// </returns>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        <tt>checkPermission</tt> method doesn't allow
		///        getting the stack trace of thread. </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= RuntimePermission </seealso>
		/// <seealso cref= Throwable#getStackTrace
		/// 
		/// @since 1.5 </seealso>
		public virtual StackTraceElement[] StackTrace
		{
			get
			{
				if (this != Thread.CurrentThread)
				{
					// check for getStackTrace permission
					SecurityManager security = System.SecurityManager;
					if (security != null)
					{
						security.CheckPermission(SecurityConstants.GET_STACK_TRACE_PERMISSION);
					}
					// optimization so we do not call into the vm for threads that
					// have not yet started or have terminated
					if (!Alive)
					{
						return EMPTY_STACK_TRACE;
					}
					StackTraceElement[][] stackTraceArray = dumpThreads(new Thread[] {this});
					StackTraceElement[] stackTrace = stackTraceArray[0];
					// a thread that was alive during the previous isAlive call may have
					// since terminated, therefore not having a stacktrace.
					if (stackTrace == null)
					{
						stackTrace = EMPTY_STACK_TRACE;
					}
					return stackTrace;
				}
				else
				{
					// Don't need JVM help for current thread
					return (new Exception()).StackTrace;
				}
			}
		}

		/// <summary>
		/// Returns a map of stack traces for all live threads.
		/// The map keys are threads and each map value is an array of
		/// <tt>StackTraceElement</tt> that represents the stack dump
		/// of the corresponding <tt>Thread</tt>.
		/// The returned stack traces are in the format specified for
		/// the <seealso cref="#getStackTrace getStackTrace"/> method.
		/// 
		/// <para>The threads may be executing while this method is called.
		/// The stack trace of each thread only represents a snapshot and
		/// each stack trace may be obtained at different time.  A zero-length
		/// array will be returned in the map value if the virtual machine has
		/// no stack trace information about a thread.
		/// 
		/// </para>
		/// <para>If there is a security manager, then the security manager's
		/// <tt>checkPermission</tt> method is called with a
		/// <tt>RuntimePermission("getStackTrace")</tt> permission as well as
		/// <tt>RuntimePermission("modifyThreadGroup")</tt> permission
		/// to see if it is ok to get the stack trace of all threads.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a <tt>Map</tt> from <tt>Thread</tt> to an array of
		/// <tt>StackTraceElement</tt> that represents the stack trace of
		/// the corresponding thread.
		/// </returns>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        <tt>checkPermission</tt> method doesn't allow
		///        getting the stack trace of thread. </exception>
		/// <seealso cref= #getStackTrace </seealso>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= RuntimePermission </seealso>
		/// <seealso cref= Throwable#getStackTrace
		/// 
		/// @since 1.5 </seealso>
		public static IDictionary<Thread, StackTraceElement[]> AllStackTraces
		{
			get
			{
				// check for getStackTrace permission
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckPermission(SecurityConstants.GET_STACK_TRACE_PERMISSION);
					security.CheckPermission(SecurityConstants.MODIFY_THREADGROUP_PERMISSION);
				}
    
				// Get a snapshot of the list of all threads
				Thread[] threads = Threads;
				StackTraceElement[][] traces = dumpThreads(threads);
				IDictionary<Thread, StackTraceElement[]> m = new Dictionary<Thread, StackTraceElement[]>(threads.Length);
				for (int i = 0; i < threads.Length; i++)
				{
					StackTraceElement[] stackTrace = traces[i];
					if (stackTrace != null)
					{
						m[threads[i]] = stackTrace;
					}
					// else terminated so we don't put it in the map
				}
				return m;
			}
		}


		private static readonly RuntimePermission SUBCLASS_IMPLEMENTATION_PERMISSION = new RuntimePermission("enableContextClassLoaderOverride");

		/// <summary>
		/// cache of subclass security audit results </summary>
		/* Replace with ConcurrentReferenceHashMap when/if it appears in a future
		 * release */
		private class Caches
		{
			/// <summary>
			/// cache of subclass security audit results </summary>
			internal static readonly ConcurrentMap<WeakClassKey, Boolean> SubclassAudits = new ConcurrentDictionary<WeakClassKey, Boolean>();

			/// <summary>
			/// queue for WeakReferences to audited subclasses </summary>
			internal static readonly ReferenceQueue<Class> SubclassAuditsQueue = new ReferenceQueue<Class>();
		}

		/// <summary>
		/// Verifies that this (possibly subclass) instance can be constructed
		/// without violating security constraints: the subclass must not override
		/// security-sensitive non-final methods, or else the
		/// "enableContextClassLoaderOverride" RuntimePermission is checked.
		/// </summary>
		private static bool IsCCLOverridden(Class cl)
		{
			if (cl == typeof(Thread))
			{
				return false;
			}

			ProcessQueue(Caches.SubclassAuditsQueue, Caches.SubclassAudits);
			WeakClassKey key = new WeakClassKey(cl, Caches.SubclassAuditsQueue);
			Boolean result = Caches.SubclassAudits[key];
			if (result == null)
			{
				result = Convert.ToBoolean(AuditSubclass(cl));
				Caches.SubclassAudits.PutIfAbsent(key, result);
			}

			return result.BooleanValue();
		}

		/// <summary>
		/// Performs reflective checks on given subclass to verify that it doesn't
		/// override security-sensitive non-final methods.  Returns true if the
		/// subclass overrides any of the methods, false otherwise.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static boolean auditSubclass(final Class subcl)
		private static bool AuditSubclass(Class subcl)
		{
			Boolean result = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(subcl)
		   );
			return result.BooleanValue();
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Boolean>
		{
			private Type Subcl;

			public PrivilegedActionAnonymousInnerClassHelper(Type subcl)
			{
				this.Subcl = subcl;
			}

			public virtual Boolean Run()
			{
				for (Class cl = Subcl; cl != typeof(Thread); cl = cl.BaseType)
				{
					try
					{
						cl.GetDeclaredMethod("getContextClassLoader", new Class[0]);
						return true;
					}
					catch (NoSuchMethodException)
					{
					}
					try
					{
						Class[] @params = new Class[] {typeof(ClassLoader)};
						cl.getDeclaredMethod("setContextClassLoader", @params);
						return true;
					}
					catch (NoSuchMethodException)
					{
					}
				}
				return false;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static StackTraceElement[][] dumpThreads(Thread[] threads);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static Thread[] getThreads();

		/// <summary>
		/// Returns the identifier of this Thread.  The thread ID is a positive
		/// <tt>long</tt> number generated when this thread was created.
		/// The thread ID is unique and remains unchanged during its lifetime.
		/// When a thread is terminated, this thread ID may be reused.
		/// </summary>
		/// <returns> this thread's ID.
		/// @since 1.5 </returns>
		public virtual long Id
		{
			get
			{
				return Tid;
			}
		}

		/// <summary>
		/// A thread state.  A thread can be in one of the following states:
		/// <ul>
		/// <li><seealso cref="#NEW"/><br>
		///     A thread that has not yet started is in this state.
		///     </li>
		/// <li><seealso cref="#RUNNABLE"/><br>
		///     A thread executing in the Java virtual machine is in this state.
		///     </li>
		/// <li><seealso cref="#BLOCKED"/><br>
		///     A thread that is blocked waiting for a monitor lock
		///     is in this state.
		///     </li>
		/// <li><seealso cref="#WAITING"/><br>
		///     A thread that is waiting indefinitely for another thread to
		///     perform a particular action is in this state.
		///     </li>
		/// <li><seealso cref="#TIMED_WAITING"/><br>
		///     A thread that is waiting for another thread to perform an action
		///     for up to a specified waiting time is in this state.
		///     </li>
		/// <li><seealso cref="#TERMINATED"/><br>
		///     A thread that has exited is in this state.
		///     </li>
		/// </ul>
		/// 
		/// <para>
		/// A thread can be in only one state at a given point in time.
		/// These states are virtual machine states which do not reflect
		/// any operating system thread states.
		/// 
		/// @since   1.5
		/// </para>
		/// </summary>
		/// <seealso cref= #getState </seealso>
		public sealed class State
		{
			/// <summary>
			/// Thread state for a thread which has not yet started.
			/// </summary>
			NEW,
			public static readonly State NEW = new State("NEW", InnerEnum.NEW);

			/// <summary>
			/// Thread state for a runnable thread.  A thread in the runnable
			/// state is executing in the Java virtual machine but it may
			/// be waiting for other resources from the operating system
			/// such as processor.
			/// </summary>
			RUNNABLE,
			public static readonly State RUNNABLE = new State("RUNNABLE", InnerEnum.RUNNABLE);

			/// <summary>
			/// Thread state for a thread blocked waiting for a monitor lock.
			/// A thread in the blocked state is waiting for a monitor lock
			/// to enter a synchronized block/method or
			/// reenter a synchronized block/method after calling
			/// <seealso cref="Object#wait() Object.wait"/>.
			/// </summary>
			BLOCKED,
			public static readonly State BLOCKED = new State("BLOCKED", InnerEnum.BLOCKED);

			/// <summary>
			/// Thread state for a waiting thread.
			/// A thread is in the waiting state due to calling one of the
			/// following methods:
			/// <ul>
			///   <li><seealso cref="Object#wait() Object.wait"/> with no timeout</li>
			///   <li><seealso cref="#join() Thread.join"/> with no timeout</li>
			///   <li><seealso cref="LockSupport#park() LockSupport.park"/></li>
			/// </ul>
			/// 
			/// <para>A thread in the waiting state is waiting for another thread to
			/// perform a particular action.
			/// 
			/// For example, a thread that has called <tt>Object.wait()</tt>
			/// on an object is waiting for another thread to call
			/// <tt>Object.notify()</tt> or <tt>Object.notifyAll()</tt> on
			/// that object. A thread that has called <tt>Thread.join()</tt>
			/// is waiting for a specified thread to terminate.
			/// </para>
			/// </summary>
			WAITING,
			public static readonly State WAITING = new State("WAITING", InnerEnum.WAITING);

			/// <summary>
			/// Thread state for a waiting thread with a specified waiting time.
			/// A thread is in the timed waiting state due to calling one of
			/// the following methods with a specified positive waiting time:
			/// <ul>
			///   <li><seealso cref="#sleep Thread.sleep"/></li>
			///   <li><seealso cref="Object#wait(long) Object.wait"/> with timeout</li>
			///   <li><seealso cref="#join(long) Thread.join"/> with timeout</li>
			///   <li><seealso cref="LockSupport#parkNanos LockSupport.parkNanos"/></li>
			///   <li><seealso cref="LockSupport#parkUntil LockSupport.parkUntil"/></li>
			/// </ul>
			/// </summary>
			TIMED_WAITING,
			public static readonly State TIMED_WAITING = new State("TIMED_WAITING", InnerEnum.TIMED_WAITING);

			/// <summary>
			/// Thread state for a terminated thread.
			/// The thread has completed execution.
			/// </summary>
			TERMINATED
			public static readonly State TERMINATED = new State("TERMINATED", InnerEnum.TERMINATED);

			private static readonly IList<State> valueList = new List<State>();

			static State()
			{
				valueList.Add(NEW);
				valueList.Add(RUNNABLE);
				valueList.Add(BLOCKED);
				valueList.Add(WAITING);
				valueList.Add(TIMED_WAITING);
				valueList.Add(TERMINATED);
			}

			public enum InnerEnum
			{
				NEW,
				RUNNABLE,
				BLOCKED,
				WAITING,
				TIMED_WAITING,
				TERMINATED
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			public static IList<State> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static State valueOf(string name)
			{
				foreach (State enumInstance in State.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		/// <summary>
		/// Returns the state of this thread.
		/// This method is designed for use in monitoring of the system state,
		/// not for synchronization control.
		/// </summary>
		/// <returns> this thread's state.
		/// @since 1.5 </returns>
		public virtual State State
		{
			get
			{
				// get current thread state
				return sun.misc.VM.toThreadState(ThreadStatus);
			}
		}

		// Added in JSR-166

		/// <summary>
		/// Interface for handlers invoked when a <tt>Thread</tt> abruptly
		/// terminates due to an uncaught exception.
		/// <para>When a thread is about to terminate due to an uncaught exception
		/// the Java Virtual Machine will query the thread for its
		/// <tt>UncaughtExceptionHandler</tt> using
		/// <seealso cref="#getUncaughtExceptionHandler"/> and will invoke the handler's
		/// <tt>uncaughtException</tt> method, passing the thread and the
		/// exception as arguments.
		/// If a thread has not had its <tt>UncaughtExceptionHandler</tt>
		/// explicitly set, then its <tt>ThreadGroup</tt> object acts as its
		/// <tt>UncaughtExceptionHandler</tt>. If the <tt>ThreadGroup</tt> object
		/// has no
		/// special requirements for dealing with the exception, it can forward
		/// the invocation to the {@link #getDefaultUncaughtExceptionHandler
		/// default uncaught exception handler}.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #setDefaultUncaughtExceptionHandler </seealso>
		/// <seealso cref= #setUncaughtExceptionHandler </seealso>
		/// <seealso cref= ThreadGroup#uncaughtException
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface UncaughtExceptionHandler
		public interface UncaughtExceptionHandler
		{
			/// <summary>
			/// Method invoked when the given thread terminates due to the
			/// given uncaught exception.
			/// <para>Any exception thrown by this method will be ignored by the
			/// Java Virtual Machine.
			/// </para>
			/// </summary>
			/// <param name="t"> the thread </param>
			/// <param name="e"> the exception </param>
			void UncaughtException(Thread t, Throwable e);
		}

		// null unless explicitly set
		private volatile UncaughtExceptionHandler UncaughtExceptionHandler_Renamed;

		// null unless explicitly set
		private static volatile UncaughtExceptionHandler DefaultUncaughtExceptionHandler_Renamed;

		/// <summary>
		/// Set the default handler invoked when a thread abruptly terminates
		/// due to an uncaught exception, and no other handler has been defined
		/// for that thread.
		/// 
		/// <para>Uncaught exception handling is controlled first by the thread, then
		/// by the thread's <seealso cref="ThreadGroup"/> object and finally by the default
		/// uncaught exception handler. If the thread does not have an explicit
		/// uncaught exception handler set, and the thread's thread group
		/// (including parent thread groups)  does not specialize its
		/// <tt>uncaughtException</tt> method, then the default handler's
		/// <tt>uncaughtException</tt> method will be invoked.
		/// </para>
		/// <para>By setting the default uncaught exception handler, an application
		/// can change the way in which uncaught exceptions are handled (such as
		/// logging to a specific device, or file) for those threads that would
		/// already accept whatever &quot;default&quot; behavior the system
		/// provided.
		/// 
		/// </para>
		/// <para>Note that the default uncaught exception handler should not usually
		/// defer to the thread's <tt>ThreadGroup</tt> object, as that could cause
		/// infinite recursion.
		/// 
		/// </para>
		/// </summary>
		/// <param name="eh"> the object to use as the default uncaught exception handler.
		/// If <tt>null</tt> then there is no default handler.
		/// </param>
		/// <exception cref="SecurityException"> if a security manager is present and it
		///         denies <tt><seealso cref="RuntimePermission"/>
		///         (&quot;setDefaultUncaughtExceptionHandler&quot;)</tt>
		/// </exception>
		/// <seealso cref= #setUncaughtExceptionHandler </seealso>
		/// <seealso cref= #getUncaughtExceptionHandler </seealso>
		/// <seealso cref= ThreadGroup#uncaughtException
		/// @since 1.5 </seealso>
		public static UncaughtExceptionHandler DefaultUncaughtExceptionHandler
		{
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.checkPermission(new RuntimePermission("setDefaultUncaughtExceptionHandler")
						   );
				}
    
				 DefaultUncaughtExceptionHandler_Renamed = value;
			}
			get
			{
				return DefaultUncaughtExceptionHandler_Renamed;
			}
		}


		/// <summary>
		/// Returns the handler invoked when this thread abruptly terminates
		/// due to an uncaught exception. If this thread has not had an
		/// uncaught exception handler explicitly set then this thread's
		/// <tt>ThreadGroup</tt> object is returned, unless this thread
		/// has terminated, in which case <tt>null</tt> is returned.
		/// @since 1.5 </summary>
		/// <returns> the uncaught exception handler for this thread </returns>
		public virtual UncaughtExceptionHandler UncaughtExceptionHandler
		{
			get
			{
				return UncaughtExceptionHandler_Renamed != null ? UncaughtExceptionHandler_Renamed : Group;
			}
			set
			{
				CheckAccess();
				UncaughtExceptionHandler_Renamed = value;
			}
		}


		/// <summary>
		/// Dispatch an uncaught exception to the handler. This method is
		/// intended to be called only by the JVM.
		/// </summary>
		private void DispatchUncaughtException(Throwable e)
		{
			UncaughtExceptionHandler.UncaughtException(this, e);
		}

		/// <summary>
		/// Removes from the specified map any keys that have been enqueued
		/// on the specified reference queue.
		/// </summary>
		internal static void processQueue<T1>(ReferenceQueue<Class> queue, ConcurrentMap<T1> map) where T1 : WeakReference<Class>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Reference<? extends Class> ref;
			Reference<?> @ref;
			while ((@ref = queue.poll()) != null)
			{
				map.Remove(@ref);
			}
		}

		/// <summary>
		///  Weak key for Class objects.
		/// 
		/// </summary>
		internal class WeakClassKey : WeakReference<Class>
		{
			/// <summary>
			/// saved value of the referent's identity hash code, to maintain
			/// a consistent hash code after the referent has been cleared
			/// </summary>
			internal readonly int Hash;

			/// <summary>
			/// Create a new WeakClassKey to the given object, registered
			/// with a queue.
			/// </summary>
			internal WeakClassKey(Class cl, ReferenceQueue<Class> refQueue) : base(cl, refQueue)
			{
				Hash = System.identityHashCode(cl);
			}

			/// <summary>
			/// Returns the identity hash code of the original referent.
			/// </summary>
			public override int HashCode()
			{
				return Hash;
			}

			/// <summary>
			/// Returns true if the given object is this identical
			/// WeakClassKey instance, or, if this object's referent has not
			/// been cleared, if the given object is another WeakClassKey
			/// instance with the identical non-null referent as this one.
			/// </summary>
			public override bool Equals(Object obj)
			{
				if (obj == this)
				{
					return true;
				}

				if (obj is WeakClassKey)
				{
					Object referent = get();
					return (referent != null) && (referent == ((WeakClassKey) obj).get());
				}
				else
				{
					return false;
				}
			}
		}


		// The following three initially uninitialized fields are exclusively
		// managed by class java.util.concurrent.ThreadLocalRandom. These
		// fields are used to build the high-performance PRNGs in the
		// concurrent code, and we can not risk accidental false sharing.
		// Hence, the fields are isolated with @Contended.

		/// <summary>
		/// The current seed for a ThreadLocalRandom </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @sun.misc.Contended("tlr") long threadLocalRandomSeed;
		internal long ThreadLocalRandomSeed;

		/// <summary>
		/// Probe hash value; nonzero if threadLocalRandomSeed initialized </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @sun.misc.Contended("tlr") int threadLocalRandomProbe;
		internal int ThreadLocalRandomProbe;

		/// <summary>
		/// Secondary seed isolated from public ThreadLocalRandom sequence </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @sun.misc.Contended("tlr") int threadLocalRandomSecondarySeed;
		internal int ThreadLocalRandomSecondarySeed;

		/* Some private helper methods */
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void setPriority0(int newPriority);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void stop0(Object o);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void suspend0();
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void resume0();
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void interrupt0();
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern void setNativeName(String name);
	}

}