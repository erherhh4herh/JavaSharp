using System;
using System.Collections;
using System.Threading;

/*
 * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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




	using sun.awt;
	using SunDropTargetEvent = sun.awt.dnd.SunDropTargetEvent;
	using PlatformLogger = sun.util.logging.PlatformLogger;


	using SharedSecrets = sun.misc.SharedSecrets;
	using JavaSecurityAccess = sun.misc.JavaSecurityAccess;

	/// <summary>
	/// <code>EventQueue</code> is a platform-independent class
	/// that queues events, both from the underlying peer classes
	/// and from trusted application classes.
	/// <para>
	/// It encapsulates asynchronous event dispatch machinery which
	/// extracts events from the queue and dispatches them by calling
	/// <seealso cref="#dispatchEvent(AWTEvent) dispatchEvent(AWTEvent)"/> method
	/// on this <code>EventQueue</code> with the event to be dispatched
	/// as an argument.  The particular behavior of this machinery is
	/// implementation-dependent.  The only requirements are that events
	/// which were actually enqueued to this queue (note that events
	/// being posted to the <code>EventQueue</code> can be coalesced)
	/// are dispatched:
	/// <dl>
	///   <dt> Sequentially.
	///   <dd> That is, it is not permitted that several events from
	///        this queue are dispatched simultaneously.
	///   <dt> In the same order as they are enqueued.
	///   <dd> That is, if <code>AWTEvent</code>&nbsp;A is enqueued
	///        to the <code>EventQueue</code> before
	///        <code>AWTEvent</code>&nbsp;B then event B will not be
	///        dispatched before event A.
	/// </dl>
	/// </para>
	/// <para>
	/// Some browsers partition applets in different code bases into
	/// separate contexts, and establish walls between these contexts.
	/// In such a scenario, there will be one <code>EventQueue</code>
	/// per context. Other browsers place all applets into the same
	/// context, implying that there will be only a single, global
	/// <code>EventQueue</code> for all applets. This behavior is
	/// implementation-dependent.  Consult your browser's documentation
	/// for more information.
	/// </para>
	/// <para>
	/// For information on the threading issues of the event dispatch
	/// machinery, see <a href="doc-files/AWTThreadIssues.html#Autoshutdown"
	/// >AWT Threading Issues</a>.
	/// 
	/// @author Thomas Ball
	/// @author Fred Ecks
	/// @author David Mendenhall
	/// 
	/// @since       1.1
	/// </para>
	/// </summary>
	public class EventQueue
	{
		private static readonly AtomicInteger ThreadInitNumber = new AtomicInteger(0);

		private const int LOW_PRIORITY = 0;
		private const int NORM_PRIORITY = 1;
		private const int HIGH_PRIORITY = 2;
		private const int ULTIMATE_PRIORITY = 3;

		private static readonly int NUM_PRIORITIES = ULTIMATE_PRIORITY + 1;

		/*
		 * We maintain one Queue for each priority that the EventQueue supports.
		 * That is, the EventQueue object is actually implemented as
		 * NUM_PRIORITIES queues and all Events on a particular internal Queue
		 * have identical priority. Events are pulled off the EventQueue starting
		 * with the Queue of highest priority. We progress in decreasing order
		 * across all Queues.
		 */
		private Queue[] Queues = new Queue[NUM_PRIORITIES];

		/*
		 * The next EventQueue on the stack, or null if this EventQueue is
		 * on the top of the stack.  If nextQueue is non-null, requests to post
		 * an event are forwarded to nextQueue.
		 */
		private EventQueue NextQueue;

		/*
		 * The previous EventQueue on the stack, or null if this is the
		 * "base" EventQueue.
		 */
		private EventQueue PreviousQueue;

		/*
		 * A single lock to synchronize the push()/pop() and related operations with
		 * all the EventQueues from the AppContext. Synchronization on any particular
		 * event queue(s) is not enough: we should lock the whole stack.
		 */
		private readonly Lock PushPopLock;
		private readonly Condition PushPopCond;

		/*
		 * Dummy runnable to wake up EDT from getNextEvent() after
		 push/pop is performed
		 */
		private static readonly Runnable dummyRunnable = new RunnableAnonymousInnerClassHelper();

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			public RunnableAnonymousInnerClassHelper()
			{
			}

			public virtual void Run()
			{
			}
		}

		private EventDispatchThread DispatchThread_Renamed;

		private readonly ThreadGroup ThreadGroup = Thread.CurrentThread.ThreadGroup;
		private readonly ClassLoader ClassLoader = Thread.CurrentThread.ContextClassLoader;

		/*
		 * The time stamp of the last dispatched InputEvent or ActionEvent.
		 */
		private long MostRecentEventTime_Renamed = DateTimeHelperClass.CurrentUnixTimeMillis();

		/*
		 * The time stamp of the last KeyEvent .
		 */
		private long MostRecentKeyEventTime_Renamed = DateTimeHelperClass.CurrentUnixTimeMillis();

		/// <summary>
		/// The modifiers field of the current event, if the current event is an
		/// InputEvent or ActionEvent.
		/// </summary>
		private WeakReference<AWTEvent> CurrentEvent_Renamed;

		/*
		 * Non-zero if a thread is waiting in getNextEvent(int) for an event of
		 * a particular ID to be posted to the queue.
		 */
		private volatile int WaitForID;

		/*
		 * AppContext corresponding to the queue.
		 */
		private readonly AppContext AppContext;

		private readonly String Name = "AWT-EventQueue-" + ThreadInitNumber.AndIncrement;

		private FwDispatcher FwDispatcher_Renamed;

		private static volatile PlatformLogger EventLog_Renamed;

		private static PlatformLogger EventLog
		{
			get
			{
				if (EventLog_Renamed == null)
				{
					EventLog_Renamed = PlatformLogger.getLogger("java.awt.event.EventQueue");
				}
				return EventLog_Renamed;
			}
		}

		static EventQueue()
		{
			AWTAccessor.EventQueueAccessor = new EventQueueAccessorAnonymousInnerClassHelper();
		}

		private class EventQueueAccessorAnonymousInnerClassHelper : AWTAccessor.EventQueueAccessor
		{
			public EventQueueAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual Thread GetDispatchThread(EventQueue eventQueue)
			{
				return eventQueue.DispatchThread;
			}
			public virtual bool IsDispatchThreadImpl(EventQueue eventQueue)
			{
				return eventQueue.DispatchThreadImpl;
			}
			public virtual void RemoveSourceEvents(EventQueue eventQueue, Object source, bool removeAllEvents)
			{
				eventQueue.RemoveSourceEvents(source, removeAllEvents);
			}
			public virtual bool NoEvents(EventQueue eventQueue)
			{
				return eventQueue.NoEvents();
			}
			public virtual void Wakeup(EventQueue eventQueue, bool isShutdown)
			{
				eventQueue.Wakeup(isShutdown);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void invokeAndWait(Object source, Runnable r) throws InterruptedException, InvocationTargetException
			public virtual void InvokeAndWait(Object source, Runnable r)
			{
				EventQueue.InvokeAndWait(source, r);
			}
			public virtual void SetFwDispatcher(EventQueue eventQueue, FwDispatcher dispatcher)
			{
				eventQueue.FwDispatcher = dispatcher;
			}

			public override long GetMostRecentEventTime(EventQueue eventQueue)
			{
				return eventQueue.MostRecentEventTimeImpl;
			}
		}

		public EventQueue()
		{
			for (int i = 0; i < NUM_PRIORITIES; i++)
			{
				Queues[i] = new Queue();
			}
			/*
			 * NOTE: if you ever have to start the associated event dispatch
			 * thread at this point, be aware of the following problem:
			 * If this EventQueue instance is created in
			 * SunToolkit.createNewAppContext() the started dispatch thread
			 * may call AppContext.getAppContext() before createNewAppContext()
			 * completes thus causing mess in thread group to appcontext mapping.
			 */

			AppContext = AppContext.AppContext;
			PushPopLock = (Lock)AppContext.get(AppContext.EVENT_QUEUE_LOCK_KEY);
			PushPopCond = (Condition)AppContext.get(AppContext.EVENT_QUEUE_COND_KEY);
		}

		/// <summary>
		/// Posts a 1.1-style event to the <code>EventQueue</code>.
		/// If there is an existing event on the queue with the same ID
		/// and event source, the source <code>Component</code>'s
		/// <code>coalesceEvents</code> method will be called.
		/// </summary>
		/// <param name="theEvent"> an instance of <code>java.awt.AWTEvent</code>,
		///          or a subclass of it </param>
		/// <exception cref="NullPointerException"> if <code>theEvent</code> is <code>null</code> </exception>
		public virtual void PostEvent(AWTEvent theEvent)
		{
			SunToolkit.flushPendingEvents(AppContext);
			PostEventPrivate(theEvent);
		}

		/// <summary>
		/// Posts a 1.1-style event to the <code>EventQueue</code>.
		/// If there is an existing event on the queue with the same ID
		/// and event source, the source <code>Component</code>'s
		/// <code>coalesceEvents</code> method will be called.
		/// </summary>
		/// <param name="theEvent"> an instance of <code>java.awt.AWTEvent</code>,
		///          or a subclass of it </param>
		private void PostEventPrivate(AWTEvent theEvent)
		{
			theEvent.IsPosted = true;
			PushPopLock.@lock();
			try
			{
				if (NextQueue != null)
				{
					// Forward the event to the top of EventQueue stack
					NextQueue.PostEventPrivate(theEvent);
					return;
				}
				if (DispatchThread_Renamed == null)
				{
					if (theEvent.Source == AWTAutoShutdown.Instance)
					{
						return;
					}
					else
					{
						InitDispatchThread();
					}
				}
				PostEvent(theEvent, GetPriority(theEvent));
			}
			finally
			{
				PushPopLock.Unlock();
			}
		}

		private static int GetPriority(AWTEvent theEvent)
		{
			if (theEvent is PeerEvent)
			{
				PeerEvent peerEvent = (PeerEvent)theEvent;
				if ((peerEvent.Flags & PeerEvent.ULTIMATE_PRIORITY_EVENT) != 0)
				{
					return ULTIMATE_PRIORITY;
				}
				if ((peerEvent.Flags & PeerEvent.PRIORITY_EVENT) != 0)
				{
					return HIGH_PRIORITY;
				}
				if ((peerEvent.Flags & PeerEvent.LOW_PRIORITY_EVENT) != 0)
				{
					return LOW_PRIORITY;
				}
			}
			int id = theEvent.ID;
			if ((id >= PaintEvent.PAINT_FIRST) && (id <= PaintEvent.PAINT_LAST))
			{
				return LOW_PRIORITY;
			}
			return NORM_PRIORITY;
		}

		/// <summary>
		/// Posts the event to the internal Queue of specified priority,
		/// coalescing as appropriate.
		/// </summary>
		/// <param name="theEvent"> an instance of <code>java.awt.AWTEvent</code>,
		///          or a subclass of it </param>
		/// <param name="priority">  the desired priority of the event </param>
		private void PostEvent(AWTEvent theEvent, int priority)
		{
			if (CoalesceEvent(theEvent, priority))
			{
				return;
			}

			EventQueueItem newItem = new EventQueueItem(theEvent);

			CacheEQItem(newItem);

			bool notifyID = (theEvent.ID == this.WaitForID);

			if (Queues[priority].Head == null)
			{
				bool shouldNotify = NoEvents();
				Queues[priority].Head = Queues[priority].Tail = newItem;

				if (shouldNotify)
				{
					if (theEvent.Source != AWTAutoShutdown.Instance)
					{
						AWTAutoShutdown.Instance.notifyThreadBusy(DispatchThread_Renamed);
					}
					PushPopCond.SignalAll();
				}
				else if (notifyID)
				{
					PushPopCond.SignalAll();
				}
			}
			else
			{
				// The event was not coalesced or has non-Component source.
				// Insert it at the end of the appropriate Queue.
				Queues[priority].Tail.next = newItem;
				Queues[priority].Tail = newItem;
				if (notifyID)
				{
					PushPopCond.SignalAll();
				}
			}
		}

		private bool CoalescePaintEvent(PaintEvent e)
		{
			ComponentPeer sourcePeer = ((Component)e.Source).Peer_Renamed;
			if (sourcePeer != null)
			{
				sourcePeer.CoalescePaintEvent(e);
			}
			EventQueueItem[] cache = ((Component)e.Source).EventCache;
			if (cache == null)
			{
				return false;
			}
			int index = EventToCacheIndex(e);

			if (index != -1 && cache[index] != null)
			{
				PaintEvent merged = MergePaintEvents(e, (PaintEvent)cache[index].@event);
				if (merged != null)
				{
					cache[index].@event = merged;
					return true;
				}
			}
			return false;
		}

		private PaintEvent MergePaintEvents(PaintEvent a, PaintEvent b)
		{
			Rectangle aRect = a.UpdateRect;
			Rectangle bRect = b.UpdateRect;
			if (bRect.Contains(aRect))
			{
				return b;
			}
			if (aRect.Contains(bRect))
			{
				return a;
			}
			return null;
		}

		private bool CoalesceMouseEvent(MouseEvent e)
		{
			EventQueueItem[] cache = ((Component)e.Source).EventCache;
			if (cache == null)
			{
				return false;
			}
			int index = EventToCacheIndex(e);
			if (index != -1 && cache[index] != null)
			{
				cache[index].@event = e;
				return true;
			}
			return false;
		}

		private bool CoalescePeerEvent(PeerEvent e)
		{
			EventQueueItem[] cache = ((Component)e.Source).EventCache;
			if (cache == null)
			{
				return false;
			}
			int index = EventToCacheIndex(e);
			if (index != -1 && cache[index] != null)
			{
				e = e.coalesceEvents((PeerEvent)cache[index].@event);
				if (e != null)
				{
					cache[index].@event = e;
					return true;
				}
				else
				{
					cache[index] = null;
				}
			}
			return false;
		}

		/*
		 * Should avoid of calling this method by any means
		 * as it's working time is dependant on EQ length.
		 * In the wors case this method alone can slow down the entire application
		 * 10 times by stalling the Event processing.
		 * Only here by backward compatibility reasons.
		 */
		private bool CoalesceOtherEvent(AWTEvent e, int priority)
		{
			int id = e.ID;
			Component source = (Component)e.Source;
			for (EventQueueItem entry = Queues[priority].Head; entry != null; entry = entry.next)
			{
				// Give Component.coalesceEvents a chance
				if (entry.@event.Source == source && entry.@event.ID == id)
				{
					AWTEvent coalescedEvent = source.CoalesceEvents(entry.@event, e);
					if (coalescedEvent != null)
					{
						entry.@event = coalescedEvent;
						return true;
					}
				}
			}
			return false;
		}

		private bool CoalesceEvent(AWTEvent e, int priority)
		{
			if (!(e.Source is Component))
			{
				return false;
			}
			if (e is PeerEvent)
			{
				return CoalescePeerEvent((PeerEvent)e);
			}
			// The worst case
			if (((Component)e.Source).CoalescingEnabled && CoalesceOtherEvent(e, priority))
			{
				return true;
			}
			if (e is PaintEvent)
			{
				return CoalescePaintEvent((PaintEvent)e);
			}
			if (e is MouseEvent)
			{
				return CoalesceMouseEvent((MouseEvent)e);
			}
			return false;
		}

		private void CacheEQItem(EventQueueItem entry)
		{
			int index = EventToCacheIndex(entry.@event);
			if (index != -1 && entry.@event.Source is Component)
			{
				Component source = (Component)entry.@event.Source;
				if (source.EventCache == null)
				{
					source.EventCache = new EventQueueItem[CACHE_LENGTH];
				}
				source.EventCache[index] = entry;
			}
		}

		private void UncacheEQItem(EventQueueItem entry)
		{
			int index = EventToCacheIndex(entry.@event);
			if (index != -1 && entry.@event.Source is Component)
			{
				Component source = (Component)entry.@event.Source;
				if (source.EventCache == null)
				{
					return;
				}
				source.EventCache[index] = null;
			}
		}

		private const int PAINT = 0;
		private const int UPDATE = 1;
		private const int MOVE = 2;
		private const int DRAG = 3;
		private const int PEER = 4;
		private const int CACHE_LENGTH = 5;

		private static int EventToCacheIndex(AWTEvent e)
		{
			switch (e.ID)
			{
			case PaintEvent.PAINT:
				return PAINT;
			case PaintEvent.UPDATE:
				return UPDATE;
			case MouseEvent.MOUSE_MOVED:
				return MOVE;
			case MouseEvent.MOUSE_DRAGGED:
				// Return -1 for SunDropTargetEvent since they are usually synchronous
				// and we don't want to skip them by coalescing with MouseEvent or other drag events
				return e is SunDropTargetEvent ? - 1 : DRAG;
			default:
				return e is PeerEvent ? PEER : -1;
			}
		}

		/// <summary>
		/// Returns whether an event is pending on any of the separate
		/// Queues. </summary>
		/// <returns> whether an event is pending on any of the separate Queues </returns>
		private bool NoEvents()
		{
			for (int i = 0; i < NUM_PRIORITIES; i++)
			{
				if (Queues[i].Head != null)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Removes an event from the <code>EventQueue</code> and
		/// returns it.  This method will block until an event has
		/// been posted by another thread. </summary>
		/// <returns> the next <code>AWTEvent</code> </returns>
		/// <exception cref="InterruptedException">
		///            if any thread has interrupted this thread </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AWTEvent getNextEvent() throws InterruptedException
		public virtual AWTEvent NextEvent
		{
			get
			{
				do
				{
					/*
					 * SunToolkit.flushPendingEvents must be called outside
					 * of the synchronized block to avoid deadlock when
					 * event queues are nested with push()/pop().
					 */
					SunToolkit.flushPendingEvents(AppContext);
					PushPopLock.@lock();
					try
					{
						AWTEvent @event = NextEventPrivate;
						if (@event != null)
						{
							return @event;
						}
						AWTAutoShutdown.Instance.notifyThreadFree(DispatchThread_Renamed);
						PushPopCond.@await();
					}
					finally
					{
						PushPopLock.Unlock();
					}
				} while (true);
			}
		}

		/*
		 * Must be called under the lock. Doesn't call flushPendingEvents()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: AWTEvent getNextEventPrivate() throws InterruptedException
		internal virtual AWTEvent NextEventPrivate
		{
			get
			{
				for (int i = NUM_PRIORITIES - 1; i >= 0; i--)
				{
					if (Queues[i].Head != null)
					{
						EventQueueItem entry = Queues[i].Head;
						Queues[i].Head = entry.next;
						if (entry.next == null)
						{
							Queues[i].Tail = null;
						}
						UncacheEQItem(entry);
						return entry.@event;
					}
				}
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: AWTEvent getNextEvent(int id) throws InterruptedException
		internal virtual AWTEvent GetNextEvent(int id)
		{
			do
			{
				/*
				 * SunToolkit.flushPendingEvents must be called outside
				 * of the synchronized block to avoid deadlock when
				 * event queues are nested with push()/pop().
				 */
				SunToolkit.flushPendingEvents(AppContext);
				PushPopLock.@lock();
				try
				{
					for (int i = 0; i < NUM_PRIORITIES; i++)
					{
						for (EventQueueItem entry = Queues[i].Head, prev = null; entry != null; prev = entry, entry = entry.next)
						{
							if (entry.@event.ID == id)
							{
								if (prev == null)
								{
									Queues[i].Head = entry.next;
								}
								else
								{
									prev.next = entry.next;
								}
								if (Queues[i].Tail == entry)
								{
									Queues[i].Tail = prev;
								}
								UncacheEQItem(entry);
								return entry.@event;
							}
						}
					}
					WaitForID = id;
					PushPopCond.@await();
					WaitForID = 0;
				}
				finally
				{
					PushPopLock.Unlock();
				}
			} while (true);
		}

		/// <summary>
		/// Returns the first event on the <code>EventQueue</code>
		/// without removing it. </summary>
		/// <returns> the first event </returns>
		public virtual AWTEvent PeekEvent()
		{
			PushPopLock.@lock();
			try
			{
				for (int i = NUM_PRIORITIES - 1; i >= 0; i--)
				{
					if (Queues[i].Head != null)
					{
						return Queues[i].Head.@event;
					}
				}
			}
			finally
			{
				PushPopLock.Unlock();
			}

			return null;
		}

		/// <summary>
		/// Returns the first event with the specified id, if any. </summary>
		/// <param name="id"> the id of the type of event desired </param>
		/// <returns> the first event of the specified id or <code>null</code>
		///    if there is no such event </returns>
		public virtual AWTEvent PeekEvent(int id)
		{
			PushPopLock.@lock();
			try
			{
				for (int i = NUM_PRIORITIES - 1; i >= 0; i--)
				{
					EventQueueItem q = Queues[i].Head;
					for (; q != null; q = q.next)
					{
						if (q.@event.ID == id)
						{
							return q.@event;
						}
					}
				}
			}
			finally
			{
				PushPopLock.Unlock();
			}

			return null;
		}

		private static readonly JavaSecurityAccess JavaSecurityAccess = SharedSecrets.JavaSecurityAccess;

		/// <summary>
		/// Dispatches an event. The manner in which the event is
		/// dispatched depends upon the type of the event and the
		/// type of the event's source object:
		/// 
		/// <table border=1 summary="Event types, source types, and dispatch methods">
		/// <tr>
		///     <th>Event Type</th>
		///     <th>Source Type</th>
		///     <th>Dispatched To</th>
		/// </tr>
		/// <tr>
		///     <td>ActiveEvent</td>
		///     <td>Any</td>
		///     <td>event.dispatch()</td>
		/// </tr>
		/// <tr>
		///     <td>Other</td>
		///     <td>Component</td>
		///     <td>source.dispatchEvent(AWTEvent)</td>
		/// </tr>
		/// <tr>
		///     <td>Other</td>
		///     <td>MenuComponent</td>
		///     <td>source.dispatchEvent(AWTEvent)</td>
		/// </tr>
		/// <tr>
		///     <td>Other</td>
		///     <td>Other</td>
		///     <td>No action (ignored)</td>
		/// </tr>
		/// </table>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="event"> an instance of <code>java.awt.AWTEvent</code>,
		///          or a subclass of it </param>
		/// <exception cref="NullPointerException"> if <code>event</code> is <code>null</code>
		/// @since           1.2 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void dispatchEvent(final AWTEvent event)
		protected internal virtual void DispatchEvent(AWTEvent @event)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object src = event.getSource();
			Object src = @event.Source;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.security.PrivilegedAction<Void> action = new java.security.PrivilegedAction<Void>()
			PrivilegedAction<Void> action = new PrivilegedActionAnonymousInnerClassHelper(this, @event, src);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.security.AccessControlContext stack = java.security.AccessController.getContext();
			AccessControlContext stack = AccessController.Context;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.security.AccessControlContext srcAcc = getAccessControlContextFrom(src);
			AccessControlContext srcAcc = GetAccessControlContextFrom(src);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.security.AccessControlContext eventAcc = event.getAccessControlContext();
			AccessControlContext eventAcc = @event.AccessControlContext;
			if (srcAcc == null)
			{
				JavaSecurityAccess.doIntersectionPrivilege(action, stack, eventAcc);
			}
			else
			{
				JavaSecurityAccess.doIntersectionPrivilege(new PrivilegedActionAnonymousInnerClassHelper2(this, action, eventAcc), stack, srcAcc);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Void>
		{
			private readonly EventQueue OuterInstance;

			private java.awt.AWTEvent @event;
			private object Src;

			public PrivilegedActionAnonymousInnerClassHelper(EventQueue outerInstance, java.awt.AWTEvent @event, object src)
			{
				this.OuterInstance = outerInstance;
				this.@event = @event;
				this.Src = src;
			}

			public virtual Void Run()
			{
				// In case fwDispatcher is installed and we're already on the
				// dispatch thread (e.g. performing DefaultKeyboardFocusManager.sendMessage),
				// dispatch the event straight away.
				if (OuterInstance.FwDispatcher_Renamed == null || outerInstance.DispatchThreadImpl)
				{
					outerInstance.DispatchEventImpl(@event, Src);
				}
				else
				{
					OuterInstance.FwDispatcher_Renamed.scheduleDispatch(new RunnableAnonymousInnerClassHelper2(this));
				}
				return null;
			}

			private class RunnableAnonymousInnerClassHelper2 : Runnable
			{
				private readonly PrivilegedActionAnonymousInnerClassHelper OuterInstance;

				public RunnableAnonymousInnerClassHelper2(PrivilegedActionAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public virtual void Run()
				{
					outerInstance.outerInstance.DispatchEventImpl(OuterInstance.@event, OuterInstance.Src);
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Void>
		{
			private readonly EventQueue OuterInstance;

			private PrivilegedAction<Void> Action;
			private AccessControlContext EventAcc;

			public PrivilegedActionAnonymousInnerClassHelper2(EventQueue outerInstance, PrivilegedAction<Void> action, AccessControlContext eventAcc)
			{
				this.OuterInstance = outerInstance;
				this.Action = action;
				this.EventAcc = eventAcc;
			}

			public virtual Void Run()
			{
				JavaSecurityAccess.doIntersectionPrivilege(Action, EventAcc);
				return null;
			}
		}

		private static AccessControlContext GetAccessControlContextFrom(Object src)
		{
			return src is Component ? ((Component)src).AccessControlContext : src is MenuComponent ? ((MenuComponent)src).AccessControlContext : src is TrayIcon ? ((TrayIcon)src).AccessControlContext : null;
		}

		/// <summary>
		/// Called from dispatchEvent() under a correct AccessControlContext
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void dispatchEventImpl(final AWTEvent event, final Object src)
		private void DispatchEventImpl(AWTEvent @event, Object src)
		{
			@event.IsPosted = true;
			if (@event is ActiveEvent)
			{
				// This could become the sole method of dispatching in time.
				CurrentEventAndMostRecentTimeImpl = @event;
				((ActiveEvent)@event).Dispatch();
			}
			else if (src is Component)
			{
				((Component)src).DispatchEvent(@event);
				@event.Dispatched();
			}
			else if (src is MenuComponent)
			{
				((MenuComponent)src).DispatchEvent(@event);
			}
			else if (src is TrayIcon)
			{
				((TrayIcon)src).DispatchEvent(@event);
			}
			else if (src is AWTAutoShutdown)
			{
				if (NoEvents())
				{
					DispatchThread_Renamed.StopDispatching();
				}
			}
			else
			{
				if (EventLog.isLoggable(PlatformLogger.Level.FINE))
				{
					EventLog.fine("Unable to dispatch event: " + @event);
				}
			}
		}

		/// <summary>
		/// Returns the timestamp of the most recent event that had a timestamp, and
		/// that was dispatched from the <code>EventQueue</code> associated with the
		/// calling thread. If an event with a timestamp is currently being
		/// dispatched, its timestamp will be returned. If no events have yet
		/// been dispatched, the EventQueue's initialization time will be
		/// returned instead.In the current version of
		/// the JDK, only <code>InputEvent</code>s,
		/// <code>ActionEvent</code>s, and <code>InvocationEvent</code>s have
		/// timestamps; however, future versions of the JDK may add timestamps to
		/// additional event types. Note that this method should only be invoked
		/// from an application's <seealso cref="#isDispatchThread event dispatching thread"/>.
		/// If this method is
		/// invoked from another thread, the current system time (as reported by
		/// <code>System.currentTimeMillis()</code>) will be returned instead.
		/// </summary>
		/// <returns> the timestamp of the last <code>InputEvent</code>,
		///         <code>ActionEvent</code>, or <code>InvocationEvent</code> to be
		///         dispatched, or <code>System.currentTimeMillis()</code> if this
		///         method is invoked on a thread other than an event dispatching
		///         thread </returns>
		/// <seealso cref= java.awt.event.InputEvent#getWhen </seealso>
		/// <seealso cref= java.awt.event.ActionEvent#getWhen </seealso>
		/// <seealso cref= java.awt.event.InvocationEvent#getWhen </seealso>
		/// <seealso cref= #isDispatchThread
		/// 
		/// @since 1.4 </seealso>
		public static long MostRecentEventTime
		{
			get
			{
				return Toolkit.EventQueue.MostRecentEventTimeImpl;
			}
		}
		private long MostRecentEventTimeImpl
		{
			get
			{
				PushPopLock.@lock();
				try
				{
					return (Thread.CurrentThread == DispatchThread_Renamed) ? MostRecentEventTime_Renamed : DateTimeHelperClass.CurrentUnixTimeMillis();
				}
				finally
				{
					PushPopLock.Unlock();
				}
			}
		}

		/// <returns> most recent event time on all threads. </returns>
		internal virtual long MostRecentEventTimeEx
		{
			get
			{
				PushPopLock.@lock();
				try
				{
					return MostRecentEventTime_Renamed;
				}
				finally
				{
					PushPopLock.Unlock();
				}
			}
		}

		/// <summary>
		/// Returns the the event currently being dispatched by the
		/// <code>EventQueue</code> associated with the calling thread. This is
		/// useful if a method needs access to the event, but was not designed to
		/// receive a reference to it as an argument. Note that this method should
		/// only be invoked from an application's event dispatching thread. If this
		/// method is invoked from another thread, null will be returned.
		/// </summary>
		/// <returns> the event currently being dispatched, or null if this method is
		///         invoked on a thread other than an event dispatching thread
		/// @since 1.4 </returns>
		public static AWTEvent CurrentEvent
		{
			get
			{
				return Toolkit.EventQueue.CurrentEventImpl;
			}
		}
		private AWTEvent CurrentEventImpl
		{
			get
			{
				PushPopLock.@lock();
				try
				{
						return (Thread.CurrentThread == DispatchThread_Renamed) ? CurrentEvent_Renamed.get() : null;
				}
				finally
				{
					PushPopLock.Unlock();
				}
			}
		}

		/// <summary>
		/// Replaces the existing <code>EventQueue</code> with the specified one.
		/// Any pending events are transferred to the new <code>EventQueue</code>
		/// for processing by it.
		/// </summary>
		/// <param name="newEventQueue"> an <code>EventQueue</code>
		///          (or subclass thereof) instance to be use </param>
		/// <seealso cref=      java.awt.EventQueue#pop </seealso>
		/// <exception cref="NullPointerException"> if <code>newEventQueue</code> is <code>null</code>
		/// @since           1.2 </exception>
		public virtual void Push(EventQueue newEventQueue)
		{
			if (EventLog.isLoggable(PlatformLogger.Level.FINE))
			{
				EventLog.fine("EventQueue.push(" + newEventQueue + ")");
			}

			PushPopLock.@lock();
			try
			{
				EventQueue topQueue = this;
				while (topQueue.NextQueue != null)
				{
					topQueue = topQueue.NextQueue;
				}
				if (topQueue.FwDispatcher_Renamed != null)
				{
					throw new RuntimeException("push() to queue with fwDispatcher");
				}
				if ((topQueue.DispatchThread_Renamed != null) && (topQueue.DispatchThread_Renamed.EventQueue == this))
				{
					newEventQueue.DispatchThread_Renamed = topQueue.DispatchThread_Renamed;
					topQueue.DispatchThread_Renamed.EventQueue = newEventQueue;
				}

				// Transfer all events forward to new EventQueue.
				while (topQueue.PeekEvent() != null)
				{
					try
					{
						// Use getNextEventPrivate() as it doesn't call flushPendingEvents()
						newEventQueue.PostEventPrivate(topQueue.NextEventPrivate);
					}
					catch (InterruptedException ie)
					{
						if (EventLog.isLoggable(PlatformLogger.Level.FINE))
						{
							EventLog.fine("Interrupted push", ie);
						}
					}
				}

				// Wake up EDT waiting in getNextEvent(), so it can
				// pick up a new EventQueue. Post the waking event before
				// topQueue.nextQueue is assigned, otherwise the event would
				// go newEventQueue
				topQueue.PostEventPrivate(new InvocationEvent(topQueue, dummyRunnable));

				newEventQueue.PreviousQueue = topQueue;
				topQueue.NextQueue = newEventQueue;

				if (AppContext.get(AppContext.EVENT_QUEUE_KEY) == topQueue)
				{
					AppContext.put(AppContext.EVENT_QUEUE_KEY, newEventQueue);
				}

				PushPopCond.SignalAll();
			}
			finally
			{
				PushPopLock.Unlock();
			}
		}

		/// <summary>
		/// Stops dispatching events using this <code>EventQueue</code>.
		/// Any pending events are transferred to the previous
		/// <code>EventQueue</code> for processing.
		/// <para>
		/// Warning: To avoid deadlock, do not declare this method
		/// synchronized in a subclass.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="EmptyStackException"> if no previous push was made
		///  on this <code>EventQueue</code> </exception>
		/// <seealso cref=      java.awt.EventQueue#push
		/// @since           1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void pop() throws java.util.EmptyStackException
		protected internal virtual void Pop()
		{
			if (EventLog.isLoggable(PlatformLogger.Level.FINE))
			{
				EventLog.fine("EventQueue.pop(" + this + ")");
			}

			PushPopLock.@lock();
			try
			{
				EventQueue topQueue = this;
				while (topQueue.NextQueue != null)
				{
					topQueue = topQueue.NextQueue;
				}
				EventQueue prevQueue = topQueue.PreviousQueue;
				if (prevQueue == null)
				{
					throw new EmptyStackException();
				}

				topQueue.PreviousQueue = null;
				prevQueue.NextQueue = null;

				// Transfer all events back to previous EventQueue.
				while (topQueue.PeekEvent() != null)
				{
					try
					{
						prevQueue.PostEventPrivate(topQueue.NextEventPrivate);
					}
					catch (InterruptedException ie)
					{
						if (EventLog.isLoggable(PlatformLogger.Level.FINE))
						{
							EventLog.fine("Interrupted pop", ie);
						}
					}
				}

				if ((topQueue.DispatchThread_Renamed != null) && (topQueue.DispatchThread_Renamed.EventQueue == this))
				{
					prevQueue.DispatchThread_Renamed = topQueue.DispatchThread_Renamed;
					topQueue.DispatchThread_Renamed.EventQueue = prevQueue;
				}

				if (AppContext.get(AppContext.EVENT_QUEUE_KEY) == this)
				{
					AppContext.put(AppContext.EVENT_QUEUE_KEY, prevQueue);
				}

				// Wake up EDT waiting in getNextEvent(), so it can
				// pick up a new EventQueue
				topQueue.PostEventPrivate(new InvocationEvent(topQueue, dummyRunnable));

				PushPopCond.SignalAll();
			}
			finally
			{
				PushPopLock.Unlock();
			}
		}

		/// <summary>
		/// Creates a new {@code secondary loop} associated with this
		/// event queue. Use the <seealso cref="SecondaryLoop#enter"/> and
		/// <seealso cref="SecondaryLoop#exit"/> methods to start and stop the
		/// event loop and dispatch the events from this queue.
		/// </summary>
		/// <returns> secondaryLoop A new secondary loop object, which can
		///                       be used to launch a new nested event
		///                       loop and dispatch events from this queue
		/// </returns>
		/// <seealso cref= SecondaryLoop#enter </seealso>
		/// <seealso cref= SecondaryLoop#exit
		/// 
		/// @since 1.7 </seealso>
		public virtual SecondaryLoop CreateSecondaryLoop()
		{
			return CreateSecondaryLoop(null, null, 0);
		}

		internal virtual SecondaryLoop CreateSecondaryLoop(Conditional cond, EventFilter filter, long interval)
		{
			PushPopLock.@lock();
			try
			{
				if (NextQueue != null)
				{
					// Forward the request to the top of EventQueue stack
					return NextQueue.CreateSecondaryLoop(cond, filter, interval);
				}
				if (FwDispatcher_Renamed != null)
				{
					return FwDispatcher_Renamed.createSecondaryLoop();
				}
				if (DispatchThread_Renamed == null)
				{
					InitDispatchThread();
				}
				return new WaitDispatchSupport(DispatchThread_Renamed, cond, filter, interval);
			}
			finally
			{
				PushPopLock.Unlock();
			}
		}

		/// <summary>
		/// Returns true if the calling thread is
		/// <seealso cref="Toolkit#getSystemEventQueue the current AWT EventQueue"/>'s
		/// dispatch thread. Use this method to ensure that a particular
		/// task is being executed (or not being) there.
		/// <para>
		/// Note: use the <seealso cref="#invokeLater"/> or <seealso cref="#invokeAndWait"/>
		/// methods to execute a task in
		/// <seealso cref="Toolkit#getSystemEventQueue the current AWT EventQueue"/>'s
		/// dispatch thread.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if running in
		/// <seealso cref="Toolkit#getSystemEventQueue the current AWT EventQueue"/>'s
		/// dispatch thread </returns>
		/// <seealso cref=             #invokeLater </seealso>
		/// <seealso cref=             #invokeAndWait </seealso>
		/// <seealso cref=             Toolkit#getSystemEventQueue
		/// @since           1.2 </seealso>
		public static bool DispatchThread
		{
			get
			{
				EventQueue eq = Toolkit.EventQueue;
				return eq.DispatchThreadImpl;
			}
		}

		internal bool DispatchThreadImpl
		{
			get
			{
				EventQueue eq = this;
				PushPopLock.@lock();
				try
				{
					EventQueue next = eq.NextQueue;
					while (next != null)
					{
						eq = next;
						next = eq.NextQueue;
					}
					if (eq.FwDispatcher_Renamed != null)
					{
						return eq.FwDispatcher_Renamed.DispatchThread;
					}
					return (Thread.CurrentThread == eq.DispatchThread_Renamed);
				}
				finally
				{
					PushPopLock.Unlock();
				}
			}
		}

		internal void InitDispatchThread()
		{
			PushPopLock.@lock();
			try
			{
				if (DispatchThread_Renamed == null && !ThreadGroup.Destroyed && !AppContext.Disposed)
				{
					DispatchThread_Renamed = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper3(this)
				   );
					DispatchThread_Renamed.Start();
				}
			}
			finally
			{
				PushPopLock.Unlock();
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper3 : PrivilegedAction<EventDispatchThread>
		{
			private readonly EventQueue OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper3(EventQueue outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual EventDispatchThread Run()
			{
				EventDispatchThread t = new EventDispatchThread(OuterInstance.ThreadGroup, OuterInstance.Name, OuterInstance);
				t.ContextClassLoader = OuterInstance.ClassLoader;
				t.Priority = Thread.NORM_PRIORITY + 1;
				t.Daemon = false;
				AWTAutoShutdown.Instance.notifyThreadBusy(t);
				return t;
			}
		}

		internal void DetachDispatchThread(EventDispatchThread edt)
		{
			/*
			 * Minimize discard possibility for non-posted events
			 */
			SunToolkit.flushPendingEvents(AppContext);
			/*
			 * This synchronized block is to secure that the event dispatch
			 * thread won't die in the middle of posting a new event to the
			 * associated event queue. It is important because we notify
			 * that the event dispatch thread is busy after posting a new event
			 * to its queue, so the EventQueue.dispatchThread reference must
			 * be valid at that point.
			 */
			PushPopLock.@lock();
			try
			{
				if (edt == DispatchThread_Renamed)
				{
					DispatchThread_Renamed = null;
				}
				AWTAutoShutdown.Instance.notifyThreadFree(edt);
				/*
				 * Event was posted after EDT events pumping had stopped, so start
				 * another EDT to handle this event
				 */
				if (PeekEvent() != null)
				{
					InitDispatchThread();
				}
			}
			finally
			{
				PushPopLock.Unlock();
			}
		}

		/*
		 * Gets the <code>EventDispatchThread</code> for this
		 * <code>EventQueue</code>.
		 * @return the event dispatch thread associated with this event queue
		 *         or <code>null</code> if this event queue doesn't have a
		 *         working thread associated with it
		 * @see    java.awt.EventQueue#initDispatchThread
		 * @see    java.awt.EventQueue#detachDispatchThread
		 */
		internal EventDispatchThread DispatchThread
		{
			get
			{
				PushPopLock.@lock();
				try
				{
					return DispatchThread_Renamed;
				}
				finally
				{
					PushPopLock.Unlock();
				}
			}
		}

		/*
		 * Removes any pending events for the specified source object.
		 * If removeAllEvents parameter is <code>true</code> then all
		 * events for the specified source object are removed, if it
		 * is <code>false</code> then <code>SequencedEvent</code>, <code>SentEvent</code>,
		 * <code>FocusEvent</code>, <code>WindowEvent</code>, <code>KeyEvent</code>,
		 * and <code>InputMethodEvent</code> are kept in the queue, but all other
		 * events are removed.
		 *
		 * This method is normally called by the source's
		 * <code>removeNotify</code> method.
		 */
		internal void RemoveSourceEvents(Object source, bool removeAllEvents)
		{
			SunToolkit.flushPendingEvents(AppContext);
			PushPopLock.@lock();
			try
			{
				for (int i = 0; i < NUM_PRIORITIES; i++)
				{
					EventQueueItem entry = Queues[i].Head;
					EventQueueItem prev = null;
					while (entry != null)
					{
						if ((entry.@event.Source == source) && (removeAllEvents || !(entry.@event is SequencedEvent || entry.@event is SentEvent || entry.@event is FocusEvent || entry.@event is WindowEvent || entry.@event is KeyEvent || entry.@event is InputMethodEvent)))
						{
							if (entry.@event is SequencedEvent)
							{
								((SequencedEvent)entry.@event).Dispose();
							}
							if (entry.@event is SentEvent)
							{
								((SentEvent)entry.@event).Dispose();
							}
							if (entry.@event is InvocationEvent)
							{
								AWTAccessor.InvocationEventAccessor.dispose((InvocationEvent)entry.@event);
							}
							if (prev == null)
							{
								Queues[i].Head = entry.next;
							}
							else
							{
								prev.next = entry.next;
							}
							UncacheEQItem(entry);
						}
						else
						{
							prev = entry;
						}
						entry = entry.next;
					}
					Queues[i].Tail = prev;
				}
			}
			finally
			{
				PushPopLock.Unlock();
			}
		}

		internal virtual long MostRecentKeyEventTime
		{
			get
			{
				lock (this)
				{
					PushPopLock.@lock();
					try
					{
						return MostRecentKeyEventTime_Renamed;
					}
					finally
					{
						PushPopLock.Unlock();
					}
				}
			}
		}

		internal static AWTEvent CurrentEventAndMostRecentTime
		{
			set
			{
				Toolkit.EventQueue.CurrentEventAndMostRecentTimeImpl = value;
			}
		}
		private AWTEvent CurrentEventAndMostRecentTimeImpl
		{
			set
			{
				PushPopLock.@lock();
				try
				{
					if (Thread.CurrentThread != DispatchThread_Renamed)
					{
						return;
					}
    
					CurrentEvent_Renamed = new WeakReference<>(value);
    
					// This series of 'instanceof' checks should be replaced with a
					// polymorphic type (for example, an interface which declares a
					// getWhen() method). However, this would require us to make such
					// a type public, or to place it in sun.awt. Both of these approaches
					// have been frowned upon. So for now, we hack.
					//
					// In tiger, we will probably give timestamps to all events, so this
					// will no longer be an issue.
					long mostRecentEventTime2 = Long.MinValue;
					if (value is InputEvent)
					{
						InputEvent ie = (InputEvent)value;
						mostRecentEventTime2 = ie.When;
						if (value is KeyEvent)
						{
							MostRecentKeyEventTime_Renamed = ie.When;
						}
					}
					else if (value is InputMethodEvent)
					{
						InputMethodEvent ime = (InputMethodEvent)value;
						mostRecentEventTime2 = ime.When;
					}
					else if (value is ActionEvent)
					{
						ActionEvent ae = (ActionEvent)value;
						mostRecentEventTime2 = ae.When;
					}
					else if (value is InvocationEvent)
					{
						InvocationEvent ie = (InvocationEvent)value;
						mostRecentEventTime2 = ie.When;
					}
					MostRecentEventTime_Renamed = System.Math.Max(MostRecentEventTime_Renamed, mostRecentEventTime2);
				}
				finally
				{
					PushPopLock.Unlock();
				}
			}
		}

		/// <summary>
		/// Causes <code>runnable</code> to have its <code>run</code>
		/// method called in the <seealso cref="#isDispatchThread dispatch thread"/> of
		/// <seealso cref="Toolkit#getSystemEventQueue the system EventQueue"/>.
		/// This will happen after all pending events are processed.
		/// </summary>
		/// <param name="runnable">  the <code>Runnable</code> whose <code>run</code>
		///                  method should be executed
		///                  asynchronously in the
		///                  <seealso cref="#isDispatchThread event dispatch thread"/>
		///                  of <seealso cref="Toolkit#getSystemEventQueue the system EventQueue"/> </param>
		/// <seealso cref=             #invokeAndWait </seealso>
		/// <seealso cref=             Toolkit#getSystemEventQueue </seealso>
		/// <seealso cref=             #isDispatchThread
		/// @since           1.2 </seealso>
		public static void InvokeLater(Runnable runnable)
		{
			Toolkit.EventQueue.PostEvent(new InvocationEvent(Toolkit.DefaultToolkit, runnable));
		}

		/// <summary>
		/// Causes <code>runnable</code> to have its <code>run</code>
		/// method called in the <seealso cref="#isDispatchThread dispatch thread"/> of
		/// <seealso cref="Toolkit#getSystemEventQueue the system EventQueue"/>.
		/// This will happen after all pending events are processed.
		/// The call blocks until this has happened.  This method
		/// will throw an Error if called from the
		/// <seealso cref="#isDispatchThread event dispatcher thread"/>.
		/// </summary>
		/// <param name="runnable">  the <code>Runnable</code> whose <code>run</code>
		///                  method should be executed
		///                  synchronously in the
		///                  <seealso cref="#isDispatchThread event dispatch thread"/>
		///                  of <seealso cref="Toolkit#getSystemEventQueue the system EventQueue"/> </param>
		/// <exception cref="InterruptedException">  if any thread has
		///                  interrupted this thread </exception>
		/// <exception cref="InvocationTargetException">  if an throwable is thrown
		///                  when running <code>runnable</code> </exception>
		/// <seealso cref=             #invokeLater </seealso>
		/// <seealso cref=             Toolkit#getSystemEventQueue </seealso>
		/// <seealso cref=             #isDispatchThread
		/// @since           1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void invokeAndWait(Runnable runnable) throws InterruptedException, InvocationTargetException
		public static void InvokeAndWait(Runnable runnable)
		{
			InvokeAndWait(Toolkit.DefaultToolkit, runnable);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void invokeAndWait(Object source, Runnable runnable) throws InterruptedException, InvocationTargetException
		internal static void InvokeAndWait(Object source, Runnable runnable)
		{
			if (EventQueue.DispatchThread)
			{
				throw new Error("Cannot call invokeAndWait from the event dispatcher thread");
			}

//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class AWTInvocationLock
	//		{
	//		}
			Object @lock = new AWTInvocationLock();

			InvocationEvent @event = new InvocationEvent(source, runnable, @lock, true);

			lock (@lock)
			{
				Toolkit.EventQueue.PostEvent(@event);
				while (!@event.Dispatched)
				{
					@lock.Wait();
				}
			}

			Throwable eventThrowable = @event.Throwable;
			if (eventThrowable != null)
			{
				throw new InvocationTargetException(eventThrowable);
			}
		}

		/*
		 * Called from PostEventQueue.postEvent to notify that a new event
		 * appeared. First it proceeds to the EventQueue on the top of the
		 * stack, then notifies the associated dispatch thread if it exists
		 * or starts a new one otherwise.
		 */
		private void Wakeup(bool isShutdown)
		{
			PushPopLock.@lock();
			try
			{
				if (NextQueue != null)
				{
					// Forward call to the top of EventQueue stack.
					NextQueue.Wakeup(isShutdown);
				}
				else if (DispatchThread_Renamed != null)
				{
					PushPopCond.SignalAll();
				}
				else if (!isShutdown)
				{
					InitDispatchThread();
				}
			}
			finally
			{
				PushPopLock.Unlock();
			}
		}

		// The method is used by AWTAccessor for javafx/AWT single threaded mode.
		private FwDispatcher FwDispatcher
		{
			set
			{
				if (NextQueue != null)
				{
					NextQueue.FwDispatcher = value;
				}
				else
				{
					FwDispatcher_Renamed = value;
				}
			}
		}
	}

	/// <summary>
	/// The Queue object holds pointers to the beginning and end of one internal
	/// queue. An EventQueue object is composed of multiple internal Queues, one
	/// for each priority supported by the EventQueue. All Events on a particular
	/// internal Queue have identical priority.
	/// </summary>
	internal class Queue
	{
		internal EventQueueItem Head;
		internal EventQueueItem Tail;
	}

}