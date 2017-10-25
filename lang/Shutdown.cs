using System.Runtime.InteropServices;

/*
 * Copyright (c) 1999, 2005, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// Package-private utility class containing data structures and logic
	/// governing the virtual-machine shutdown sequence.
	/// 
	/// @author   Mark Reinhold
	/// @since    1.3
	/// </summary>

	internal class Shutdown
	{

		/* Shutdown state */
		private const int RUNNING = 0;
		private const int HOOKS = 1;
		private const int FINALIZERS = 2;
		private static int State = RUNNING;

		/* Should we run all finalizers upon exit? */
		private static bool RunFinalizersOnExit_Renamed = false;

		// The system shutdown hooks are registered with a predefined slot.
		// The list of shutdown hooks is as follows:
		// (0) Console restore hook
		// (1) Application hooks
		// (2) DeleteOnExit hook
		private const int MAX_SYSTEM_HOOKS = 10;
		private static readonly Runnable[] Hooks = new Runnable[MAX_SYSTEM_HOOKS];

		// the index of the currently running shutdown hook to the hooks array
		private static int CurrentRunningHook = 0;

		/* The preceding static fields are protected by this lock */
		private class Lock
		{
		}
		private static Object @lock = new Lock();

		/* Lock object for the native halt method */
		private static Object HaltLock = new Lock();

		/* Invoked by Runtime.runFinalizersOnExit */
		internal static bool RunFinalizersOnExit
		{
			set
			{
				lock (@lock)
				{
					RunFinalizersOnExit_Renamed = value;
				}
			}
		}


		/// <summary>
		/// Add a new shutdown hook.  Checks the shutdown state and the hook itself,
		/// but does not do any security checks.
		/// 
		/// The registerShutdownInProgress parameter should be false except
		/// registering the DeleteOnExitHook since the first file may
		/// be added to the delete on exit list by the application shutdown
		/// hooks.
		/// 
		/// @params slot  the slot in the shutdown hook array, whose element
		///               will be invoked in order during shutdown
		/// @params registerShutdownInProgress true to allow the hook
		///               to be registered even if the shutdown is in progress.
		/// @params hook  the hook to be registered
		/// 
		/// @throw IllegalStateException
		///        if registerShutdownInProgress is false and shutdown is in progress; or
		///        if registerShutdownInProgress is true and the shutdown process
		///           already passes the given slot
		/// </summary>
		internal static void Add(int slot, bool registerShutdownInProgress, Runnable hook)
		{
			lock (@lock)
			{
				if (Hooks[slot] != null)
				{
					throw new InternalError("Shutdown hook at slot " + slot + " already registered");
				}

				if (!registerShutdownInProgress)
				{
					if (State > RUNNING)
					{
						throw new IllegalStateException("Shutdown in progress");
					}
				}
				else
				{
					if (State > HOOKS || (State == HOOKS && slot <= CurrentRunningHook))
					{
						throw new IllegalStateException("Shutdown in progress");
					}
				}

				Hooks[slot] = hook;
			}
		}

		/* Run all registered shutdown hooks
		 */
		private static void RunHooks()
		{
			for (int i = 0; i < MAX_SYSTEM_HOOKS; i++)
			{
				try
				{
					Runnable hook;
					lock (@lock)
					{
						// acquire the lock to make sure the hook registered during
						// shutdown is visible here.
						CurrentRunningHook = i;
						hook = Hooks[i];
					}
					if (hook != null)
					{
						hook.Run();
					}
				}
				catch (Throwable t)
				{
					if (t is ThreadDeath)
					{
						ThreadDeath td = (ThreadDeath)t;
						throw td;
					}
				}
			}
		}

		/* The halt method is synchronized on the halt lock
		 * to avoid corruption of the delete-on-shutdown file list.
		 * It invokes the true native halt method.
		 */
		internal static void Halt(int status)
		{
			lock (HaltLock)
			{
				halt0(status);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void halt0(int status);

		/* Wormhole for invoking java.lang.ref.Finalizer.runAllFinalizers */
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void runAllFinalizers();


		/* The actual shutdown sequence is defined here.
		 *
		 * If it weren't for runFinalizersOnExit, this would be simple -- we'd just
		 * run the hooks and then halt.  Instead we need to keep track of whether
		 * we're running hooks or finalizers.  In the latter case a finalizer could
		 * invoke exit(1) to cause immediate termination, while in the former case
		 * any further invocations of exit(n), for any n, simply stall.  Note that
		 * if on-exit finalizers are enabled they're run iff the shutdown is
		 * initiated by an exit(0); they're never run on exit(n) for n != 0 or in
		 * response to SIGINT, SIGTERM, etc.
		 */
		private static void Sequence()
		{
			lock (@lock)
			{
				/* Guard against the possibility of a daemon thread invoking exit
				 * after DestroyJavaVM initiates the shutdown sequence
				 */
				if (State != HOOKS)
				{
					return;
				}
			}
			RunHooks();
			bool rfoe;
			lock (@lock)
			{
				State = FINALIZERS;
				rfoe = RunFinalizersOnExit_Renamed;
			}
			if (rfoe)
			{
				runAllFinalizers();
			}
		}


		/* Invoked by Runtime.exit, which does all the security checks.
		 * Also invoked by handlers for system-provided termination events,
		 * which should pass a nonzero status code.
		 */
		internal static void Exit(int status)
		{
			bool runMoreFinalizers = false;
			lock (@lock)
			{
				if (status != 0)
				{
					RunFinalizersOnExit_Renamed = false;
				}
				switch (State)
				{
				case RUNNING: // Initiate shutdown
					State = HOOKS;
					break;
				case HOOKS: // Stall and halt
					break;
				case FINALIZERS:
					if (status != 0)
					{
						/* Halt immediately on nonzero status */
						Halt(status);
					}
					else
					{
						/* Compatibility with old behavior:
						 * Run more finalizers and then halt
						 */
						runMoreFinalizers = RunFinalizersOnExit_Renamed;
					}
					break;
				}
			}
			if (runMoreFinalizers)
			{
				runAllFinalizers();
				Halt(status);
			}
			lock (typeof(Shutdown))
			{
				/* Synchronize on the class object, causing any other thread
				 * that attempts to initiate shutdown to stall indefinitely
				 */
				Sequence();
				Halt(status);
			}
		}


		/* Invoked by the JNI DestroyJavaVM procedure when the last non-daemon
		 * thread has finished.  Unlike the exit method, this method does not
		 * actually halt the VM.
		 */
		internal static void Shutdown()
		{
			lock (@lock)
			{
				switch (State)
				{
				case RUNNING: // Initiate shutdown
					State = HOOKS;
					break;
				case HOOKS: // Stall and then return
				case FINALIZERS:
					break;
				}
			}
			lock (typeof(Shutdown))
			{
				Sequence();
			}
		}

	}

}