using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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


	using PlatformLogger = sun.util.logging.PlatformLogger;

	using SunDragSourceContextPeer = sun.awt.dnd.SunDragSourceContextPeer;
	using EventQueueDelegate = sun.awt.EventQueueDelegate;

	/// <summary>
	/// EventDispatchThread is a package-private AWT class which takes
	/// events off the EventQueue and dispatches them to the appropriate
	/// AWT components.
	/// 
	/// The Thread starts a "permanent" event pump with a call to
	/// pumpEvents(Conditional) in its run() method. Event handlers can choose to
	/// block this event pump at any time, but should start a new pump (<b>not</b>
	/// a new EventDispatchThread) by again calling pumpEvents(Conditional). This
	/// secondary event pump will exit automatically as soon as the Condtional
	/// evaluate()s to false and an additional Event is pumped and dispatched.
	/// 
	/// @author Tom Ball
	/// @author Amy Fowler
	/// @author Fred Ecks
	/// @author David Mendenhall
	/// 
	/// @since 1.1
	/// </summary>
	internal class EventDispatchThread : Thread
	{

		private static readonly PlatformLogger EventLog = PlatformLogger.getLogger("java.awt.event.EventDispatchThread");

		private EventQueue TheQueue;
		private volatile bool DoDispatch = true;

		private const int ANY_EVENT = -1;

		private List<EventFilter> EventFilters = new List<EventFilter>();

		internal EventDispatchThread(ThreadGroup group, String name, EventQueue queue) : base(group, name)
		{
			EventQueue = queue;
		}

		/*
		 * Must be called on EDT only, that's why no synchronization
		 */
		public virtual void StopDispatching()
		{
			DoDispatch = false;
		}

		public override void Run()
		{
			try
			{
				PumpEvents(new ConditionalAnonymousInnerClassHelper(this));
			}
			finally
			{
				EventQueue.DetachDispatchThread(this);
			}
		}

		private class ConditionalAnonymousInnerClassHelper : Conditional
		{
			private readonly EventDispatchThread OuterInstance;

			public ConditionalAnonymousInnerClassHelper(EventDispatchThread outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual bool Evaluate()
			{
				return true;
			}
		}

		internal virtual void PumpEvents(Conditional cond)
		{
			PumpEvents(ANY_EVENT, cond);
		}

		internal virtual void PumpEventsForHierarchy(Conditional cond, Component modalComponent)
		{
			PumpEventsForHierarchy(ANY_EVENT, cond, modalComponent);
		}

		internal virtual void PumpEvents(int id, Conditional cond)
		{
			PumpEventsForHierarchy(id, cond, null);
		}

		internal virtual void PumpEventsForHierarchy(int id, Conditional cond, Component modalComponent)
		{
			PumpEventsForFilter(id, cond, new HierarchyEventFilter(modalComponent));
		}

		internal virtual void PumpEventsForFilter(Conditional cond, EventFilter filter)
		{
			PumpEventsForFilter(ANY_EVENT, cond, filter);
		}

		internal virtual void PumpEventsForFilter(int id, Conditional cond, EventFilter filter)
		{
			AddEventFilter(filter);
			DoDispatch = true;
			while (DoDispatch && !Interrupted && cond.Evaluate())
			{
				PumpOneEventForFilters(id);
			}
			RemoveEventFilter(filter);
		}

		internal virtual void AddEventFilter(EventFilter filter)
		{
			if (EventLog.isLoggable(PlatformLogger.Level.FINEST))
			{
				EventLog.finest("adding the event filter: " + filter);
			}
			lock (EventFilters)
			{
				if (!EventFilters.Contains(filter))
				{
					if (filter is ModalEventFilter)
					{
						ModalEventFilter newFilter = (ModalEventFilter)filter;
						int k = 0;
						for (k = 0; k < EventFilters.Count; k++)
						{
							EventFilter f = EventFilters[k];
							if (f is ModalEventFilter)
							{
								ModalEventFilter cf = (ModalEventFilter)f;
								if (cf.CompareTo(newFilter) > 0)
								{
									break;
								}
							}
						}
						EventFilters.Insert(k, filter);
					}
					else
					{
						EventFilters.Add(filter);
					}
				}
			}
		}

		internal virtual void RemoveEventFilter(EventFilter filter)
		{
			if (EventLog.isLoggable(PlatformLogger.Level.FINEST))
			{
				EventLog.finest("removing the event filter: " + filter);
			}
			lock (EventFilters)
			{
				EventFilters.Remove(filter);
			}
		}

		internal virtual void PumpOneEventForFilters(int id)
		{
			AWTEvent @event = null;
			bool eventOK = false;
			try
			{
				EventQueue eq = null;
				EventQueueDelegate.Delegate @delegate = null;
				do
				{
					// EventQueue may change during the dispatching
					eq = EventQueue;
					@delegate = EventQueueDelegate.Delegate;

					if (@delegate != null && id == ANY_EVENT)
					{
						@event = @delegate.getNextEvent(eq);
					}
					else
					{
						@event = (id == ANY_EVENT) ? eq.NextEvent : eq.GetNextEvent(id);
					}

					eventOK = true;
					lock (EventFilters)
					{
						for (int i = EventFilters.Count - 1; i >= 0; i--)
						{
							EventFilter f = EventFilters[i];
							EventFilter_FilterAction accept = f.AcceptEvent(@event);
							if (accept == EventFilter_FilterAction.REJECT)
							{
								eventOK = false;
								break;
							}
							else if (accept == EventFilter_FilterAction.ACCEPT_IMMEDIATELY)
							{
								break;
							}
						}
					}
					eventOK = eventOK && SunDragSourceContextPeer.checkEvent(@event);
					if (!eventOK)
					{
						@event.Consume();
					}
				} while (eventOK == false);

				if (EventLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					EventLog.finest("Dispatching: " + @event);
				}

				Object handle = null;
				if (@delegate != null)
				{
					handle = @delegate.beforeDispatch(@event);
				}
				eq.DispatchEvent(@event);
				if (@delegate != null)
				{
					@delegate.afterDispatch(@event, handle);
				}
			}
			catch (ThreadDeath death)
			{
				DoDispatch = false;
				throw death;
			}
			catch (InterruptedException)
			{
				DoDispatch = false; // AppContext.dispose() interrupts all
									// Threads in the AppContext
			}
			catch (Throwable e)
			{
				ProcessException(e);
			}
		}

		private void ProcessException(Throwable e)
		{
			if (EventLog.isLoggable(PlatformLogger.Level.FINE))
			{
				EventLog.fine("Processing exception: " + e);
			}
			UncaughtExceptionHandler.UncaughtException(this, e);
		}

		public virtual EventQueue EventQueue
		{
			get
			{
				lock (this)
				{
					return TheQueue;
				}
			}
			set
			{
				lock (this)
				{
					TheQueue = value;
				}
			}
		}

		private class HierarchyEventFilter : EventFilter
		{
			internal Component ModalComponent;
			public HierarchyEventFilter(Component modalComponent)
			{
				this.ModalComponent = modalComponent;
			}
			public virtual EventFilter_FilterAction AcceptEvent(AWTEvent @event)
			{
				if (ModalComponent != null)
				{
					int eventID = @event.ID;
					bool mouseEvent = (eventID >= MouseEvent.MOUSE_FIRST) && (eventID <= MouseEvent.MOUSE_LAST);
					bool actionEvent = (eventID >= ActionEvent.ACTION_FIRST) && (eventID <= ActionEvent.ACTION_LAST);
					bool windowClosingEvent = (eventID == WindowEvent.WINDOW_CLOSING);
					/*
					 * filter out MouseEvent and ActionEvent that's outside
					 * the modalComponent hierarchy.
					 * KeyEvent is handled by using enqueueKeyEvent
					 * in Dialog.show
					 */
					if (Component.IsInstanceOf(ModalComponent, "javax.swing.JInternalFrame"))
					{
						/*
						 * Modal internal frames are handled separately. If event is
						 * for some component from another heavyweight than modalComp,
						 * it is accepted. If heavyweight is the same - we still accept
						 * event and perform further filtering in LightweightDispatcher
						 */
						return windowClosingEvent ? EventFilter_FilterAction.REJECT : EventFilter_FilterAction.ACCEPT;
					}
					if (mouseEvent || actionEvent || windowClosingEvent)
					{
						Object o = @event.Source;
						if (o is sun.awt.ModalExclude)
						{
							// Exclude this object from modality and
							// continue to pump it's events.
							return EventFilter_FilterAction.ACCEPT;
						}
						else if (o is Component)
						{
							Component c = (Component) o;
							// 5.0u3 modal exclusion
							bool modalExcluded = false;
							if (ModalComponent is Container)
							{
								while (c != ModalComponent && c != null)
								{
									if ((c is Window) && (sun.awt.SunToolkit.isModalExcluded((Window)c)))
									{
										// Exclude this window and all its children from
										//  modality and continue to pump it's events.
										modalExcluded = true;
										break;
									}
									c = c.Parent;
								}
							}
							if (!modalExcluded && (c != ModalComponent))
							{
								return EventFilter_FilterAction.REJECT;
							}
						}
					}
				}
				return EventFilter_FilterAction.ACCEPT;
			}
		}
	}

}