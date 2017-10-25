using System;
using System.Threading;

/*
 * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt
{



	using PeerEvent = sun.awt.PeerEvent;

	using PlatformLogger = sun.util.logging.PlatformLogger;

	/// <summary>
	/// This utility class is used to suspend execution on a thread
	/// while still allowing {@code EventDispatchThread} to dispatch events.
	/// The API methods of the class are thread-safe.
	/// 
	/// @author Anton Tarasov, Artem Ananiev
	/// 
	/// @since 1.7
	/// </summary>
	internal class WaitDispatchSupport : SecondaryLoop
	{

		private static readonly PlatformLogger Log = PlatformLogger.getLogger("java.awt.event.WaitDispatchSupport");

		private EventDispatchThread DispatchThread;
		private EventFilter Filter;

		private volatile Conditional ExtCondition;
		private volatile Conditional Condition;

		private long Interval;
		// Use a shared daemon timer to serve all the WaitDispatchSupports
		private static Timer Timer;
		// When this WDS expires, we cancel the timer task leaving the
		// shared timer up and running
		private TimerTask TimerTask;

		private AtomicBoolean KeepBlockingEDT = new AtomicBoolean(false);
		private AtomicBoolean KeepBlockingCT = new AtomicBoolean(false);

		private static void InitializeTimer()
		{
			lock (typeof(WaitDispatchSupport))
			{
				if (Timer == null)
				{
					Timer = new Timer("AWT-WaitDispatchSupport-Timer", true);
				}
			}
		}

		/// <summary>
		/// Creates a {@code WaitDispatchSupport} instance to
		/// serve the given event dispatch thread.
		/// </summary>
		/// <param name="dispatchThread"> An event dispatch thread that
		///        should not stop dispatching events while waiting
		/// 
		/// @since 1.7 </param>
		public WaitDispatchSupport(EventDispatchThread dispatchThread) : this(dispatchThread, null)
		{
		}

		/// <summary>
		/// Creates a {@code WaitDispatchSupport} instance to
		/// serve the given event dispatch thread.
		/// </summary>
		/// <param name="dispatchThread"> An event dispatch thread that
		///        should not stop dispatching events while waiting </param>
		/// <param name="extCond"> A conditional object used to determine
		///        if the loop should be terminated
		/// 
		/// @since 1.7 </param>
		public WaitDispatchSupport(EventDispatchThread dispatchThread, Conditional extCond)
		{
			if (dispatchThread == null)
			{
				throw new IllegalArgumentException("The dispatchThread can not be null");
			}

			this.DispatchThread = dispatchThread;
			this.ExtCondition = extCond;
			this.Condition = new ConditionalAnonymousInnerClassHelper(this);
		}

		private class ConditionalAnonymousInnerClassHelper : Conditional
		{
			private readonly WaitDispatchSupport OuterInstance;

			public ConditionalAnonymousInnerClassHelper(WaitDispatchSupport outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual bool Evaluate()
			{
				if (Log.isLoggable(PlatformLogger.Level.FINEST))
				{
					Log.finest("evaluate(): blockingEDT=" + OuterInstance.KeepBlockingEDT.Get() + ", blockingCT=" + OuterInstance.KeepBlockingCT.Get());
				}
				bool extEvaluate = (OuterInstance.ExtCondition != null) ? OuterInstance.ExtCondition.Evaluate() : true;
				if (!OuterInstance.KeepBlockingEDT.Get() || !extEvaluate)
				{
					if (OuterInstance.TimerTask != null)
					{
						OuterInstance.TimerTask.Cancel();
						OuterInstance.TimerTask = null;
					}
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Creates a {@code WaitDispatchSupport} instance to
		/// serve the given event dispatch thread.
		/// <para>
		/// The <seealso cref="EventFilter"/> is set on the {@code dispatchThread}
		/// while waiting. The filter is removed on completion of the
		/// waiting process.
		/// </para>
		/// <para>
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="dispatchThread"> An event dispatch thread that
		///        should not stop dispatching events while waiting </param>
		/// <param name="filter"> {@code EventFilter} to be set </param>
		/// <param name="interval"> A time interval to wait for. Note that
		///        when the waiting process takes place on EDT
		///        there is no guarantee to stop it in the given time
		/// 
		/// @since 1.7 </param>
		public WaitDispatchSupport(EventDispatchThread dispatchThread, Conditional extCondition, EventFilter filter, long interval) : this(dispatchThread, extCondition)
		{
			this.Filter = filter;
			if (interval < 0)
			{
				throw new IllegalArgumentException("The interval value must be >= 0");
			}
			this.Interval = interval;
			if (interval != 0)
			{
				InitializeTimer();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual bool Enter()
		{
			if (Log.isLoggable(PlatformLogger.Level.FINE))
			{
				Log.fine("enter(): blockingEDT=" + KeepBlockingEDT.Get() + ", blockingCT=" + KeepBlockingCT.Get());
			}

			if (!KeepBlockingEDT.CompareAndSet(false, true))
			{
				Log.fine("The secondary loop is already running, aborting");
				return false;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Runnable run = new Runnable()
			Runnable run = new RunnableAnonymousInnerClassHelper(this);

			// We have two mechanisms for blocking: if we're on the
			// dispatch thread, start a new event pump; if we're
			// on any other thread, call wait() on the treelock

			Thread currentThread = Thread.CurrentThread;
			if (currentThread == DispatchThread)
			{
				if (Log.isLoggable(PlatformLogger.Level.FINEST))
				{
					Log.finest("On dispatch thread: " + DispatchThread);
				}
				if (Interval != 0)
				{
					if (Log.isLoggable(PlatformLogger.Level.FINEST))
					{
						Log.finest("scheduling the timer for " + Interval + " ms");
					}
					Timer.Schedule(TimerTask = new TimerTaskAnonymousInnerClassHelper(this), Interval);
				}
				// Dispose SequencedEvent we are dispatching on the the current
				// AppContext, to prevent us from hang - see 4531693 for details
				SequencedEvent currentSE = KeyboardFocusManager.CurrentKeyboardFocusManager.CurrentSequencedEvent;
				if (currentSE != null)
				{
					if (Log.isLoggable(PlatformLogger.Level.FINE))
					{
						Log.fine("Dispose current SequencedEvent: " + currentSE);
					}
					currentSE.Dispose();
				}
				// In case the exit() method is called before starting
				// new event pump it will post the waking event to EDT.
				// The event will be handled after the the new event pump
				// starts. Thus, the enter() method will not hang.
				//
				// Event pump should be privileged. See 6300270.
				AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, run));
			}
			else
			{
				if (Log.isLoggable(PlatformLogger.Level.FINEST))
				{
					Log.finest("On non-dispatch thread: " + currentThread);
				}
				lock (TreeLock)
				{
					if (Filter != null)
					{
						DispatchThread.AddEventFilter(Filter);
					}
					try
					{
						EventQueue eq = DispatchThread.EventQueue;
						eq.PostEvent(new PeerEvent(this, run, PeerEvent.PRIORITY_EVENT));
						KeepBlockingCT.Set(true);
						if (Interval > 0)
						{
							long currTime = DateTimeHelperClass.CurrentUnixTimeMillis();
							while (KeepBlockingCT.Get() && ((ExtCondition != null) ? ExtCondition.Evaluate() : true) && (currTime + Interval > DateTimeHelperClass.CurrentUnixTimeMillis()))
							{
								Monitor.Wait(TreeLock, TimeSpan.FromMilliseconds(Interval));
							}
						}
						else
						{
							while (KeepBlockingCT.Get() && ((ExtCondition != null) ? ExtCondition.Evaluate() : true))
							{
								TreeLock.Wait();
							}
						}
						if (Log.isLoggable(PlatformLogger.Level.FINE))
						{
							Log.fine("waitDone " + KeepBlockingEDT.Get() + " " + KeepBlockingCT.Get());
						}
					}
					catch (InterruptedException e)
					{
						if (Log.isLoggable(PlatformLogger.Level.FINE))
						{
							Log.fine("Exception caught while waiting: " + e);
						}
					}
					finally
					{
						if (Filter != null)
						{
							DispatchThread.RemoveEventFilter(Filter);
						}
					}
					// If the waiting process has been stopped because of the
					// time interval passed or an exception occurred, the state
					// should be changed
					KeepBlockingEDT.Set(false);
					KeepBlockingCT.Set(false);
				}
			}

			return true;
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			private readonly WaitDispatchSupport OuterInstance;

			public RunnableAnonymousInnerClassHelper(WaitDispatchSupport outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual void Run()
			{
				Log.fine("Starting a new event pump");
				if (OuterInstance.Filter == null)
				{
					OuterInstance.DispatchThread.PumpEvents(OuterInstance.Condition);
				}
				else
				{
					OuterInstance.DispatchThread.PumpEventsForFilter(OuterInstance.Condition, OuterInstance.Filter);
				}
			}
		}

		private class TimerTaskAnonymousInnerClassHelper : TimerTask
		{
			private readonly WaitDispatchSupport OuterInstance;

			public TimerTaskAnonymousInnerClassHelper(WaitDispatchSupport outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public override void Run()
			{
				if (OuterInstance.KeepBlockingEDT.CompareAndSet(true, false))
				{
					outerInstance.WakeupEDT();
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private readonly WaitDispatchSupport OuterInstance;

			private java.lang.Runnable Run;

			public PrivilegedActionAnonymousInnerClassHelper(WaitDispatchSupport outerInstance, java.lang.Runnable run)
			{
				this.OuterInstance = outerInstance;
				this.Run = run;
			}

			public virtual Void Run()
			{
				Run.Run();
				return null;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual bool Exit()
		{
			if (Log.isLoggable(PlatformLogger.Level.FINE))
			{
				Log.fine("exit(): blockingEDT=" + KeepBlockingEDT.Get() + ", blockingCT=" + KeepBlockingCT.Get());
			}
			if (KeepBlockingEDT.CompareAndSet(true, false))
			{
				WakeupEDT();
				return true;
			}
			return false;
		}

		private static Object TreeLock
		{
			get
			{
				return Component.LOCK;
			}
		}

		private readonly Runnable wakingRunnable = new RunnableAnonymousInnerClassHelper2();

		private class RunnableAnonymousInnerClassHelper2 : Runnable
		{
			public RunnableAnonymousInnerClassHelper2()
			{
			}

			public virtual void Run()
			{
				Log.fine("Wake up EDT");
				lock (TreeLock)
				{
					OuterInstance.KeepBlockingCT.Set(false);
					Monitor.PulseAll(TreeLock);
				}
				Log.fine("Wake up EDT done");
			}
		}

		private void WakeupEDT()
		{
			if (Log.isLoggable(PlatformLogger.Level.FINEST))
			{
				Log.finest("wakeupEDT(): EDT == " + DispatchThread);
			}
			EventQueue eq = DispatchThread.EventQueue;
			eq.PostEvent(new PeerEvent(this, wakingRunnable, PeerEvent.PRIORITY_EVENT));
		}
	}

}