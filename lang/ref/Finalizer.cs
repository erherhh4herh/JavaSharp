using System.Collections;
using System.Threading;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.@ref
{

	using JavaLangAccess = sun.misc.JavaLangAccess;
	using SharedSecrets = sun.misc.SharedSecrets;
	using VM = sun.misc.VM;

	internal sealed class Finalizer : FinalReference<Object>
	{
	/* Package-private; must be in
	                                                          same package as the Reference
	                                                          class */

		private static ReferenceQueue<Object> Queue = new ReferenceQueue<Object>();
		private static Finalizer Unfinalized = null;
		private static readonly Object @lock = new Object();

		private Finalizer Next = null, Prev = null;

		private bool HasBeenFinalized()
		{
			return (Next == this);
		}

		private void Add()
		{
			lock (@lock)
			{
				if (Unfinalized != null)
				{
					this.Next = Unfinalized;
					Unfinalized.Prev = this;
				}
				Unfinalized = this;
			}
		}

		private void Remove()
		{
			lock (@lock)
			{
				if (Unfinalized == this)
				{
					if (this.Next != null)
					{
						Unfinalized = this.Next;
					}
					else
					{
						Unfinalized = this.Prev;
					}
				}
				if (this.Next != null)
				{
					this.Next.Prev = this.Prev;
				}
				if (this.Prev != null)
				{
					this.Prev.Next = this.Next;
				}
				this.Next = this; // Indicates that this has been finalized
				this.Prev = this;
			}
		}

		private Finalizer(Object finalizee) : base(finalizee, Queue)
		{
			Add();
		}

		/* Invoked by VM */
		internal static void Register(Object finalizee)
		{
			new Finalizer(finalizee);
		}

		private void RunFinalizer(JavaLangAccess jla)
		{
			lock (this)
			{
				if (HasBeenFinalized())
				{
					return;
				}
				Remove();
			}
			try
			{
				Object finalizee = this.Get();
				if (finalizee != null && !(finalizee is java.lang.Enum))
				{
					jla.invokeFinalize(finalizee);

					/* Clear stack slot containing this variable, to decrease
					   the chances of false retention with a conservative GC */
					finalizee = null;
				}
			}
			catch (Throwable)
			{
			}
			base.Clear();
		}

		/* Create a privileged secondary finalizer thread in the system thread
		   group for the given Runnable, and wait for it to complete.
	
		   This method is used by both runFinalization and runFinalizersOnExit.
		   The former method invokes all pending finalizers, while the latter
		   invokes all uninvoked finalizers if on-exit finalization has been
		   enabled.
	
		   These two methods could have been implemented by offloading their work
		   to the regular finalizer thread and waiting for that thread to finish.
		   The advantage of creating a fresh thread, however, is that it insulates
		   invokers of these methods from a stalled or deadlocked finalizer thread.
		 */
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void forkSecondaryFinalizer(final Runnable proc)
		private static void ForkSecondaryFinalizer(Runnable proc)
		{
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(proc));
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private java.lang.Runnable Proc;

			public PrivilegedActionAnonymousInnerClassHelper(java.lang.Runnable proc)
			{
				this.Proc = proc;
			}

			public virtual Void Run()
			{
			ThreadGroup tg = Thread.CurrentThread.ThreadGroup;
			for (ThreadGroup tgn = tg; tgn != null; tg = tgn, tgn = tg.Parent)
			{
				;
			}
			Thread sft = new Thread(tg, Proc, "Secondary finalizer");
			sft.Start();
			try
			{
				sft.Join();
			}
			catch (InterruptedException)
			{
				/* Ignore */
			}
			return null;
			}
		}

		/* Called by Runtime.runFinalization() */
		internal static void RunFinalization()
		{
			if (!VM.Booted)
			{
				return;
			}

			ForkSecondaryFinalizer(new RunnableAnonymousInnerClassHelper());
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			public RunnableAnonymousInnerClassHelper()
			{
			}

			private volatile bool running;
			public virtual void Run()
			{
				if (running)
				{
					return;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final sun.misc.JavaLangAccess jla = sun.misc.SharedSecrets.getJavaLangAccess();
				JavaLangAccess jla = SharedSecrets.JavaLangAccess;
				running = true;
				for (;;)
				{
					Finalizer f = (Finalizer)Queue.Poll();
					if (f == null)
					{
						break;
					}
					f.RunFinalizer(jla);
				}
			}
		}

		/* Invoked by java.lang.Shutdown */
		internal static void RunAllFinalizers()
		{
			if (!VM.Booted)
			{
				return;
			}

			ForkSecondaryFinalizer(new RunnableAnonymousInnerClassHelper2());
		}

		private class RunnableAnonymousInnerClassHelper2 : Runnable
		{
			public RunnableAnonymousInnerClassHelper2()
			{
			}

			private volatile bool running;
			public virtual void Run()
			{
				if (running)
				{
					return;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final sun.misc.JavaLangAccess jla = sun.misc.SharedSecrets.getJavaLangAccess();
				JavaLangAccess jla = SharedSecrets.JavaLangAccess;
				running = true;
				for (;;)
				{
					Finalizer f;
					lock (@lock)
					{
						f = Unfinalized;
						if (f == null)
						{
							break;
						}
						Unfinalized = f.Next;
					}
					f.RunFinalizer(jla);
				}
			}
		}

		private class FinalizerThread : Thread
		{
			internal volatile bool Running;
			internal FinalizerThread(ThreadGroup g) : base(g, "Finalizer")
			{
			}
			public override void Run()
			{
				if (Running)
				{
					return;
				}

				// Finalizer thread starts before System.initializeSystemClass
				// is called.  Wait until JavaLangAccess is available
				while (!VM.Booted)
				{
					// delay until VM completes initialization
					try
					{
						VM.awaitBooted();
					}
					catch (InterruptedException)
					{
						// ignore and continue
					}
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final sun.misc.JavaLangAccess jla = sun.misc.SharedSecrets.getJavaLangAccess();
				JavaLangAccess jla = SharedSecrets.JavaLangAccess;
				Running = true;
				for (;;)
				{
					try
					{
						Finalizer f = (Finalizer)Queue.Remove();
						f.RunFinalizer(jla);
					}
					catch (InterruptedException)
					{
						// ignore and continue
					}
				}
			}
		}

		static Finalizer()
		{
			ThreadGroup tg = Thread.CurrentThread.ThreadGroup;
			for (ThreadGroup tgn = tg; tgn != null; tg = tgn, tgn = tg.Parent)
			{
				;
			}
			Thread finalizer = new FinalizerThread(tg);
			finalizer.Priority = Thread.MAX_PRIORITY - 2;
			finalizer.Daemon = true;
			finalizer.Start();
		}

	}

}