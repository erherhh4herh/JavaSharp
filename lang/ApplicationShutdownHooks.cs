/*
 * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

	/*
	 * Class to track and run user level shutdown hooks registered through
	 * <tt>{@link Runtime#addShutdownHook Runtime.addShutdownHook}</tt>.
	 *
	 * @see java.lang.Runtime#addShutdownHook
	 * @see java.lang.Runtime#removeShutdownHook
	 */

	internal class ApplicationShutdownHooks
	{
		/* The set of registered hooks */
		private static IdentityHashMap<Thread, Thread> Hooks;
		static ApplicationShutdownHooks()
		{
			try
			{
				Shutdown.add(1, false, new RunnableAnonymousInnerClassHelper() // not registered if shutdown in progress -  shutdown hook invocation order
			   );
				Hooks = new IdentityHashMap<>();
			}
			catch (IllegalStateException)
			{
				// application shutdown hooks cannot be added if
				// shutdown is in progress.
				Hooks = null;
			}
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			public RunnableAnonymousInnerClassHelper()
			{
			}

			public virtual void Run()
			{
				RunHooks();
			}
		}


		private ApplicationShutdownHooks()
		{
		}

		/* Add a new shutdown hook.  Checks the shutdown state and the hook itself,
		 * but does not do any security checks.
		 */
		internal static void Add(Thread hook)
		{
			lock (typeof(ApplicationShutdownHooks))
			{
				if (Hooks == null)
				{
					throw new IllegalStateException("Shutdown in progress");
				}
        
				if (hook.Alive)
				{
					throw new IllegalArgumentException("Hook already running");
				}
        
				if (Hooks.ContainsKey(hook))
				{
					throw new IllegalArgumentException("Hook previously registered");
				}
        
				Hooks.Put(hook, hook);
			}
		}

		/* Remove a previously-registered hook.  Like the add method, this method
		 * does not do any security checks.
		 */
		internal static bool Remove(Thread hook)
		{
			lock (typeof(ApplicationShutdownHooks))
			{
				if (Hooks == null)
				{
					throw new IllegalStateException("Shutdown in progress");
				}
        
				if (hook == null)
				{
					throw new NullPointerException();
				}
        
				return Hooks.Remove(hook) != null;
			}
		}

		/* Iterates over all application hooks creating a new thread for each
		 * to run in. Hooks are run concurrently and this method waits for
		 * them to finish.
		 */
		internal static void RunHooks()
		{
			Collection<Thread> threads;
			lock (typeof(ApplicationShutdownHooks))
			{
				threads = Hooks.KeySet();
				Hooks = null;
			}

			foreach (Thread hook in threads)
			{
				hook.Start();
			}
			foreach (Thread hook in threads)
			{
				try
				{
					hook.Join();
				}
				catch (InterruptedException)
				{
				}
			}
		}
	}

}