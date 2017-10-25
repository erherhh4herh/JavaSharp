using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using AppContext = sun.awt.AppContext;
	using SunToolkit = sun.awt.SunToolkit;
	using AWTAccessor = sun.awt.AWTAccessor;
	using CausedFocusEvent = sun.awt.CausedFocusEvent;
	using TimedWindowEvent = sun.awt.TimedWindowEvent;

	/// <summary>
	/// The default KeyboardFocusManager for AWT applications. Focus traversal is
	/// done in response to a Component's focus traversal keys, and using a
	/// Container's FocusTraversalPolicy.
	/// <para>
	/// Please see
	/// <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
	/// How to Use the Focus Subsystem</a>,
	/// a section in <em>The Java Tutorial</em>, and the
	/// <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a>
	/// for more information.
	/// 
	/// @author David Mendenhall
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= FocusTraversalPolicy </seealso>
	/// <seealso cref= Component#setFocusTraversalKeys </seealso>
	/// <seealso cref= Component#getFocusTraversalKeys
	/// @since 1.4 </seealso>
	public class DefaultKeyboardFocusManager : KeyboardFocusManager
	{
		private static readonly PlatformLogger FocusLog = PlatformLogger.getLogger("java.awt.focus.DefaultKeyboardFocusManager");

		// null weak references to not create too many objects
		private static readonly WeakReference<Window> NULL_WINDOW_WR = new WeakReference<Window>(null);
		private static readonly WeakReference<Component> NULL_COMPONENT_WR = new WeakReference<Component>(null);
		private WeakReference<Window> RealOppositeWindowWR = NULL_WINDOW_WR;
		private WeakReference<Component> RealOppositeComponentWR = NULL_COMPONENT_WR;
		private int InSendMessage;
		private LinkedList<KeyEvent> EnqueuedKeyEvents = new LinkedList<KeyEvent>();
		private LinkedList<TypeAheadMarker> TypeAheadMarkers = new LinkedList<TypeAheadMarker>();
		private bool ConsumeNextKeyTyped_Renamed;

		static DefaultKeyboardFocusManager()
		{
			AWTAccessor.DefaultKeyboardFocusManagerAccessor = new DefaultKeyboardFocusManagerAccessorAnonymousInnerClassHelper();
		}

		private class DefaultKeyboardFocusManagerAccessorAnonymousInnerClassHelper : AWTAccessor.DefaultKeyboardFocusManagerAccessor
		{
			public DefaultKeyboardFocusManagerAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual void ConsumeNextKeyTyped(DefaultKeyboardFocusManager dkfm, KeyEvent e)
			{
				dkfm.ConsumeNextKeyTyped(e);
			}
		}

		private class TypeAheadMarker
		{
			internal long After;
			internal Component UntilFocused;

			internal TypeAheadMarker(long after, Component untilFocused)
			{
				this.After = after;
				this.UntilFocused = untilFocused;
			}
			/// <summary>
			/// Returns string representation of the marker
			/// </summary>
			public override String ToString()
			{
				return ">>> Marker after " + After + " on " + UntilFocused;
			}
		}

		private Window GetOwningFrameDialog(Window window)
		{
			while (window != null && !(window is Frame || window is Dialog))
			{
				window = (Window)window.Parent;
			}
			return window;
		}

		/*
		 * This series of restoreFocus methods is used for recovering from a
		 * rejected focus or activation change. Rejections typically occur when
		 * the user attempts to focus a non-focusable Component or Window.
		 */
		private void RestoreFocus(FocusEvent fe, Window newFocusedWindow)
		{
			Component realOppositeComponent = this.RealOppositeComponentWR.get();
			Component vetoedComponent = fe.Component;

			if (newFocusedWindow != null && RestoreFocus(newFocusedWindow, vetoedComponent, false))
			{
			}
			else if (realOppositeComponent != null && DoRestoreFocus(realOppositeComponent, vetoedComponent, false))
			{
			}
			else if (fe.OppositeComponent != null && DoRestoreFocus(fe.OppositeComponent, vetoedComponent, false))
			{
			}
			else
			{
				ClearGlobalFocusOwnerPriv();
			}
		}
		private void RestoreFocus(WindowEvent we)
		{
			Window realOppositeWindow = this.RealOppositeWindowWR.get();
			if (realOppositeWindow != null && RestoreFocus(realOppositeWindow, null, false))
			{
				// do nothing, everything is done in restoreFocus()
			}
			else if (we.OppositeWindow != null && RestoreFocus(we.OppositeWindow, null, false))
			{
				// do nothing, everything is done in restoreFocus()
			}
			else
			{
				ClearGlobalFocusOwnerPriv();
			}
		}
		private bool RestoreFocus(Window aWindow, Component vetoedComponent, bool clearOnFailure)
		{
			Component toFocus = KeyboardFocusManager.GetMostRecentFocusOwner(aWindow);

			if (toFocus != null && toFocus != vetoedComponent && DoRestoreFocus(toFocus, vetoedComponent, false))
			{
				return true;
			}
			else if (clearOnFailure)
			{
				ClearGlobalFocusOwnerPriv();
				return true;
			}
			else
			{
				return false;
			}
		}
		private bool RestoreFocus(Component toFocus, bool clearOnFailure)
		{
			return DoRestoreFocus(toFocus, null, clearOnFailure);
		}
		private bool DoRestoreFocus(Component toFocus, Component vetoedComponent, bool clearOnFailure)
		{
			if (toFocus != vetoedComponent && toFocus.Showing && toFocus.CanBeFocusOwner() && toFocus.RequestFocus(false, CausedFocusEvent.Cause.ROLLBACK))
			{
				return true;
			}
			else
			{
				Component nextFocus = toFocus.NextFocusCandidate;
				if (nextFocus != null && nextFocus != vetoedComponent && nextFocus.RequestFocusInWindow(CausedFocusEvent.Cause.ROLLBACK))
				{
					return true;
				}
				else if (clearOnFailure)
				{
					ClearGlobalFocusOwnerPriv();
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// A special type of SentEvent which updates a counter in the target
		/// KeyboardFocusManager if it is an instance of
		/// DefaultKeyboardFocusManager.
		/// </summary>
		private class DefaultKeyboardFocusManagerSentEvent : SentEvent
		{
			/*
			 * serialVersionUID
			 */
			internal const long SerialVersionUID = -2924743257508701758L;

			public DefaultKeyboardFocusManagerSentEvent(AWTEvent nested, AppContext toNotify) : base(nested, toNotify)
			{
			}
			public sealed override void Dispatch()
			{
				KeyboardFocusManager manager = KeyboardFocusManager.CurrentKeyboardFocusManager;
				DefaultKeyboardFocusManager defaultManager = (manager is DefaultKeyboardFocusManager) ? (DefaultKeyboardFocusManager)manager : null;

				if (defaultManager != null)
				{
					lock (defaultManager)
					{
						defaultManager.InSendMessage++;
					}
				}

				base.Dispatch();

				if (defaultManager != null)
				{
					lock (defaultManager)
					{
						defaultManager.InSendMessage--;
					}
				}
			}
		}

		/// <summary>
		/// Sends a synthetic AWTEvent to a Component. If the Component is in
		/// the current AppContext, then the event is immediately dispatched.
		/// If the Component is in a different AppContext, then the event is
		/// posted to the other AppContext's EventQueue, and this method blocks
		/// until the event is handled or target AppContext is disposed.
		/// Returns true if successfuly dispatched event, false if failed
		/// to dispatch.
		/// </summary>
		internal static bool SendMessage(Component target, AWTEvent e)
		{
			e.IsPosted = true;
			AppContext myAppContext = AppContext.AppContext;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final sun.awt.AppContext targetAppContext = target.appContext;
			AppContext targetAppContext = target.AppContext;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SentEvent se = new DefaultKeyboardFocusManagerSentEvent(e, myAppContext);
			SentEvent se = new DefaultKeyboardFocusManagerSentEvent(e, myAppContext);

			if (myAppContext == targetAppContext)
			{
				se.Dispatch();
			}
			else
			{
				if (targetAppContext.Disposed)
				{
					return false;
				}
				SunToolkit.postEvent(targetAppContext, se);
				if (EventQueue.DispatchThread)
				{
					EventDispatchThread edt = (EventDispatchThread) Thread.CurrentThread;
					edt.PumpEvents(SentEvent.ID, new ConditionalAnonymousInnerClassHelper(targetAppContext, se));
				}
				else
				{
					lock (se)
					{
						while (!se.Dispatched && !targetAppContext.Disposed)
						{
							try
							{
								Monitor.Wait(se, TimeSpan.FromMilliseconds(1000));
							}
							catch (InterruptedException)
							{
								break;
							}
						}
					}
				}
			}
			return se.Dispatched;
		}

		private class ConditionalAnonymousInnerClassHelper : Conditional
		{
			private AppContext TargetAppContext;
			private java.awt.SentEvent Se;

			public ConditionalAnonymousInnerClassHelper(AppContext targetAppContext, java.awt.SentEvent se)
			{
				this.TargetAppContext = targetAppContext;
				this.Se = se;
			}

			public virtual bool Evaluate()
			{
				return !Se.Dispatched && !TargetAppContext.Disposed;
			}
		}

		/*
		 * Checks if the focus window event follows key events waiting in the type-ahead
		 * queue (if any). This may happen when a user types ahead in the window, the client
		 * listeners hang EDT for a while, and the user switches b/w toplevels. In that
		 * case the focus window events may be dispatched before the type-ahead events
		 * get handled. This may lead to wrong focus behavior and in order to avoid it,
		 * the focus window events are reposted to the end of the event queue. See 6981400.
		 */
		private bool RepostIfFollowsKeyEvents(WindowEvent e)
		{
			if (!(e is TimedWindowEvent))
			{
				return false;
			}
			TimedWindowEvent we = (TimedWindowEvent)e;
			long time = we.When;
			lock (this)
			{
				KeyEvent ke = EnqueuedKeyEvents.Count == 0 ? null : EnqueuedKeyEvents.First.Value;
				if (ke != null && time >= ke.When)
				{
					TypeAheadMarker marker = TypeAheadMarkers.Count == 0 ? null : TypeAheadMarkers.First.Value;
					if (marker != null)
					{
						Window toplevel = marker.UntilFocused.ContainingWindow;
						// Check that the component awaiting focus belongs to
						// the current focused window. See 8015454.
						if (toplevel != null && toplevel.Focused)
						{
							SunToolkit.postEvent(AppContext.AppContext, new SequencedEvent(e));
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// This method is called by the AWT event dispatcher requesting that the
		/// current KeyboardFocusManager dispatch the specified event on its behalf.
		/// DefaultKeyboardFocusManagers dispatch all FocusEvents, all WindowEvents
		/// related to focus, and all KeyEvents. These events are dispatched based
		/// on the KeyboardFocusManager's notion of the focus owner and the focused
		/// and active Windows, sometimes overriding the source of the specified
		/// AWTEvent. If this method returns <code>false</code>, then the AWT event
		/// dispatcher will attempt to dispatch the event itself.
		/// </summary>
		/// <param name="e"> the AWTEvent to be dispatched </param>
		/// <returns> <code>true</code> if this method dispatched the event;
		///         <code>false</code> otherwise </returns>
		public override bool DispatchEvent(AWTEvent e)
		{
			if (FocusLog.isLoggable(PlatformLogger.Level.FINE) && (e is WindowEvent || e is FocusEvent))
			{
				FocusLog.fine("" + e);
			}
			switch (e.ID)
			{
				case WindowEvent.WINDOW_GAINED_FOCUS:
				{
					if (RepostIfFollowsKeyEvents((WindowEvent)e))
					{
						break;
					}

					WindowEvent we = (WindowEvent)e;
					Window oldFocusedWindow = GlobalFocusedWindow;
					Window newFocusedWindow = we.Window;
					if (newFocusedWindow == oldFocusedWindow)
					{
						break;
					}

					if (!(newFocusedWindow.FocusableWindow && newFocusedWindow.Visible && newFocusedWindow.Displayable))
					{
						// we can not accept focus on such window, so reject it.
						RestoreFocus(we);
						break;
					}
					// If there exists a current focused window, then notify it
					// that it has lost focus.
					if (oldFocusedWindow != null)
					{
						bool isEventDispatched = SendMessage(oldFocusedWindow, new WindowEvent(oldFocusedWindow, WindowEvent.WINDOW_LOST_FOCUS, newFocusedWindow));
						// Failed to dispatch, clear by ourselfves
						if (!isEventDispatched)
						{
							GlobalFocusOwner = null;
							GlobalFocusedWindow = null;
						}
					}

					// Because the native libraries do not post WINDOW_ACTIVATED
					// events, we need to synthesize one if the active Window
					// changed.
					Window newActiveWindow = GetOwningFrameDialog(newFocusedWindow);
					Window currentActiveWindow = GlobalActiveWindow;
					if (newActiveWindow != currentActiveWindow)
					{
						SendMessage(newActiveWindow, new WindowEvent(newActiveWindow, WindowEvent.WINDOW_ACTIVATED, currentActiveWindow));
						if (newActiveWindow != GlobalActiveWindow)
						{
							// Activation change was rejected. Unlikely, but
							// possible.
							RestoreFocus(we);
							break;
						}
					}

					GlobalFocusedWindow = newFocusedWindow;

					if (newFocusedWindow != GlobalFocusedWindow)
					{
						// Focus change was rejected. Will happen if
						// newFocusedWindow is not a focusable Window.
						RestoreFocus(we);
						break;
					}

					// Restore focus to the Component which last held it. We do
					// this here so that client code can override our choice in
					// a WINDOW_GAINED_FOCUS handler.
					//
					// Make sure that the focus change request doesn't change the
					// focused Window in case we are no longer the focused Window
					// when the request is handled.
					if (InSendMessage == 0)
					{
						// Identify which Component should initially gain focus
						// in the Window.
						//
						// * If we're in SendMessage, then this is a synthetic
						//   WINDOW_GAINED_FOCUS message which was generated by a
						//   the FOCUS_GAINED handler. Allow the Component to
						//   which the FOCUS_GAINED message was targeted to
						//   receive the focus.
						// * Otherwise, look up the correct Component here.
						//   We don't use Window.getMostRecentFocusOwner because
						//   window is focused now and 'null' will be returned


						// Calculating of most recent focus owner and focus
						// request should be synchronized on KeyboardFocusManager.class
						// to prevent from thread race when user will request
						// focus between calculation and our request.
						// But if focus transfer is synchronous, this synchronization
						// may cause deadlock, thus we don't synchronize this block.
						Component toFocus = KeyboardFocusManager.GetMostRecentFocusOwner(newFocusedWindow);
						if ((toFocus == null) && newFocusedWindow.FocusableWindow)
						{
							toFocus = newFocusedWindow.FocusTraversalPolicy.GetInitialComponent(newFocusedWindow);
						}
						Component tempLost = null;
						lock (typeof(KeyboardFocusManager))
						{
							tempLost = newFocusedWindow.setTemporaryLostComponent(null);
						}

						// The component which last has the focus when this window was focused
						// should receive focus first
						if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
						{
							FocusLog.finer("tempLost {0}, toFocus {1}", tempLost, toFocus);
						}
						if (tempLost != null)
						{
							tempLost.RequestFocusInWindow(CausedFocusEvent.Cause.ACTIVATION);
						}

						if (toFocus != null && toFocus != tempLost)
						{
							// If there is a component which requested focus when this window
							// was inactive it expects to receive focus after activation.
							toFocus.RequestFocusInWindow(CausedFocusEvent.Cause.ACTIVATION);
						}
					}

					Window realOppositeWindow = this.RealOppositeWindowWR.get();
					if (realOppositeWindow != we.OppositeWindow)
					{
						we = new WindowEvent(newFocusedWindow, WindowEvent.WINDOW_GAINED_FOCUS, realOppositeWindow);
					}
					return TypeAheadAssertions(newFocusedWindow, we);
				}

				case WindowEvent.WINDOW_ACTIVATED:
				{
					WindowEvent we = (WindowEvent)e;
					Window oldActiveWindow = GlobalActiveWindow;
					Window newActiveWindow = we.Window;
					if (oldActiveWindow == newActiveWindow)
					{
						break;
					}

					// If there exists a current active window, then notify it that
					// it has lost activation.
					if (oldActiveWindow != null)
					{
						bool isEventDispatched = SendMessage(oldActiveWindow, new WindowEvent(oldActiveWindow, WindowEvent.WINDOW_DEACTIVATED, newActiveWindow));
						// Failed to dispatch, clear by ourselfves
						if (!isEventDispatched)
						{
							GlobalActiveWindow = null;
						}
						if (GlobalActiveWindow != null)
						{
							// Activation change was rejected. Unlikely, but
							// possible.
							break;
						}
					}

					GlobalActiveWindow = newActiveWindow;

					if (newActiveWindow != GlobalActiveWindow)
					{
						// Activation change was rejected. Unlikely, but
						// possible.
						break;
					}

					return TypeAheadAssertions(newActiveWindow, we);
				}

				case FocusEvent.FOCUS_GAINED:
				{
					FocusEvent fe = (FocusEvent)e;
					CausedFocusEvent.Cause cause = (fe is CausedFocusEvent) ? ((CausedFocusEvent)fe).Cause : CausedFocusEvent.Cause.UNKNOWN;
					Component oldFocusOwner = GlobalFocusOwner;
					Component newFocusOwner = fe.Component;
					if (oldFocusOwner == newFocusOwner)
					{
						if (FocusLog.isLoggable(PlatformLogger.Level.FINE))
						{
							FocusLog.fine("Skipping {0} because focus owner is the same", e);
						}
						// We can't just drop the event - there could be
						// type-ahead markers associated with it.
						DequeueKeyEvents(-1, newFocusOwner);
						break;
					}

					// If there exists a current focus owner, then notify it that
					// it has lost focus.
					if (oldFocusOwner != null)
					{
						bool isEventDispatched = SendMessage(oldFocusOwner, new CausedFocusEvent(oldFocusOwner, FocusEvent.FOCUS_LOST, fe.Temporary, newFocusOwner, cause));
						// Failed to dispatch, clear by ourselfves
						if (!isEventDispatched)
						{
							GlobalFocusOwner = null;
							if (!fe.Temporary)
							{
								GlobalPermanentFocusOwner = null;
							}
						}
					}

					// Because the native windowing system has a different notion
					// of the current focus and activation states, it is possible
					// that a Component outside of the focused Window receives a
					// FOCUS_GAINED event. We synthesize a WINDOW_GAINED_FOCUS
					// event in that case.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Window newFocusedWindow = sun.awt.SunToolkit.getContainingWindow(newFocusOwner);
					Window newFocusedWindow = SunToolkit.getContainingWindow(newFocusOwner);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Window currentFocusedWindow = getGlobalFocusedWindow();
					Window currentFocusedWindow = GlobalFocusedWindow;
					if (newFocusedWindow != null && newFocusedWindow != currentFocusedWindow)
					{
						SendMessage(newFocusedWindow, new WindowEvent(newFocusedWindow, WindowEvent.WINDOW_GAINED_FOCUS, currentFocusedWindow));
						if (newFocusedWindow != GlobalFocusedWindow)
						{
							// Focus change was rejected. Will happen if
							// newFocusedWindow is not a focusable Window.

							// Need to recover type-ahead, but don't bother
							// restoring focus. That was done by the
							// WINDOW_GAINED_FOCUS handler
							DequeueKeyEvents(-1, newFocusOwner);
							break;
						}
					}

					if (!(newFocusOwner.Focusable && newFocusOwner.Showing && (newFocusOwner.Enabled || cause.Equals(CausedFocusEvent.Cause.UNKNOWN))))
						// Refuse focus on a disabled component if the focus event
						// isn't of UNKNOWN reason (i.e. not a result of a direct request
						// but traversal, activation or system generated).
					{
						// we should not accept focus on such component, so reject it.
						DequeueKeyEvents(-1, newFocusOwner);
						if (KeyboardFocusManager.AutoFocusTransferEnabled)
						{
							// If FOCUS_GAINED is for a disposed component (however
							// it shouldn't happen) its toplevel parent is null. In this
							// case we have to try to restore focus in the current focused
							// window (for the details: 6607170).
							if (newFocusedWindow == null)
							{
								RestoreFocus(fe, currentFocusedWindow);
							}
							else
							{
								RestoreFocus(fe, newFocusedWindow);
							}
							SetMostRecentFocusOwner(newFocusedWindow, null); // see: 8013773
						}
						break;
					}

					GlobalFocusOwner = newFocusOwner;

					if (newFocusOwner != GlobalFocusOwner)
					{
						// Focus change was rejected. Will happen if
						// newFocusOwner is not focus traversable.
						DequeueKeyEvents(-1, newFocusOwner);
						if (KeyboardFocusManager.AutoFocusTransferEnabled)
						{
							RestoreFocus(fe, (Window)newFocusedWindow);
						}
						break;
					}

					if (!fe.Temporary)
					{
						GlobalPermanentFocusOwner = newFocusOwner;

						if (newFocusOwner != GlobalPermanentFocusOwner)
						{
							// Focus change was rejected. Unlikely, but possible.
							DequeueKeyEvents(-1, newFocusOwner);
							if (KeyboardFocusManager.AutoFocusTransferEnabled)
							{
								RestoreFocus(fe, (Window)newFocusedWindow);
							}
							break;
						}
					}

					NativeFocusOwner = GetHeavyweight(newFocusOwner);

					Component realOppositeComponent = this.RealOppositeComponentWR.get();
					if (realOppositeComponent != null && realOppositeComponent != fe.OppositeComponent)
					{
						fe = new CausedFocusEvent(newFocusOwner, FocusEvent.FOCUS_GAINED, fe.Temporary, realOppositeComponent, cause);
						((AWTEvent) fe).IsPosted = true;
					}
					return TypeAheadAssertions(newFocusOwner, fe);
				}

				case FocusEvent.FOCUS_LOST:
				{
					FocusEvent fe = (FocusEvent)e;
					Component currentFocusOwner = GlobalFocusOwner;
					if (currentFocusOwner == null)
					{
						if (FocusLog.isLoggable(PlatformLogger.Level.FINE))
						{
							FocusLog.fine("Skipping {0} because focus owner is null", e);
						}
						break;
					}
					// Ignore cases where a Component loses focus to itself.
					// If we make a mistake because of retargeting, then the
					// FOCUS_GAINED handler will correct it.
					if (currentFocusOwner == fe.OppositeComponent)
					{
						if (FocusLog.isLoggable(PlatformLogger.Level.FINE))
						{
							FocusLog.fine("Skipping {0} because current focus owner is equal to opposite", e);
						}
						break;
					}

					GlobalFocusOwner = null;

					if (GlobalFocusOwner != null)
					{
						// Focus change was rejected. Unlikely, but possible.
						RestoreFocus(currentFocusOwner, true);
						break;
					}

					if (!fe.Temporary)
					{
						GlobalPermanentFocusOwner = null;

						if (GlobalPermanentFocusOwner != null)
						{
							// Focus change was rejected. Unlikely, but possible.
							RestoreFocus(currentFocusOwner, true);
							break;
						}
					}
					else
					{
						Window owningWindow = currentFocusOwner.ContainingWindow;
						if (owningWindow != null)
						{
							owningWindow.TemporaryLostComponent = currentFocusOwner;
						}
					}

					NativeFocusOwner = null;

					fe.Source = currentFocusOwner;

					RealOppositeComponentWR = (fe.OppositeComponent != null) ? new WeakReference<Component>(currentFocusOwner) : NULL_COMPONENT_WR;

					return TypeAheadAssertions(currentFocusOwner, fe);
				}

				case WindowEvent.WINDOW_DEACTIVATED:
				{
					WindowEvent we = (WindowEvent)e;
					Window currentActiveWindow = GlobalActiveWindow;
					if (currentActiveWindow == null)
					{
						break;
					}

					if (currentActiveWindow != e.Source)
					{
						// The event is lost in time.
						// Allow listeners to precess the event but do not
						// change any global states
						break;
					}

					GlobalActiveWindow = null;
					if (GlobalActiveWindow != null)
					{
						// Activation change was rejected. Unlikely, but possible.
						break;
					}

					we.Source = currentActiveWindow;
					return TypeAheadAssertions(currentActiveWindow, we);
				}

				case WindowEvent.WINDOW_LOST_FOCUS:
				{
					if (RepostIfFollowsKeyEvents((WindowEvent)e))
					{
						break;
					}

					WindowEvent we = (WindowEvent)e;
					Window currentFocusedWindow = GlobalFocusedWindow;
					Window losingFocusWindow = we.Window;
					Window activeWindow = GlobalActiveWindow;
					Window oppositeWindow = we.OppositeWindow;
					if (FocusLog.isLoggable(PlatformLogger.Level.FINE))
					{
						FocusLog.fine("Active {0}, Current focused {1}, losing focus {2} opposite {3}", activeWindow, currentFocusedWindow, losingFocusWindow, oppositeWindow);
					}
					if (currentFocusedWindow == null)
					{
						break;
					}

					// Special case -- if the native windowing system posts an
					// event claiming that the active Window has lost focus to the
					// focused Window, then discard the event. This is an artifact
					// of the native windowing system not knowing which Window is
					// really focused.
					if (InSendMessage == 0 && losingFocusWindow == activeWindow && oppositeWindow == currentFocusedWindow)
					{
						break;
					}

					Component currentFocusOwner = GlobalFocusOwner;
					if (currentFocusOwner != null)
					{
						// The focus owner should always receive a FOCUS_LOST event
						// before the Window is defocused.
						Component oppositeComp = null;
						if (oppositeWindow != null)
						{
							oppositeComp = oppositeWindow.TemporaryLostComponent;
							if (oppositeComp == null)
							{
								oppositeComp = oppositeWindow.MostRecentFocusOwner;
							}
						}
						if (oppositeComp == null)
						{
							oppositeComp = oppositeWindow;
						}
						SendMessage(currentFocusOwner, new CausedFocusEvent(currentFocusOwner, FocusEvent.FOCUS_LOST, true, oppositeComp, CausedFocusEvent.Cause.ACTIVATION));
					}

					GlobalFocusedWindow = null;
					if (GlobalFocusedWindow != null)
					{
						// Focus change was rejected. Unlikely, but possible.
						RestoreFocus(currentFocusedWindow, null, true);
						break;
					}

					we.Source = currentFocusedWindow;
					RealOppositeWindowWR = (oppositeWindow != null) ? new WeakReference<Window>(currentFocusedWindow) : NULL_WINDOW_WR;
					TypeAheadAssertions(currentFocusedWindow, we);

					if (oppositeWindow == null)
					{
						// Then we need to deactive the active Window as well.
						// No need to synthesize in other cases, because
						// WINDOW_ACTIVATED will handle it if necessary.
						SendMessage(activeWindow, new WindowEvent(activeWindow, WindowEvent.WINDOW_DEACTIVATED, null));
						if (GlobalActiveWindow != null)
						{
							// Activation change was rejected. Unlikely,
							// but possible.
							RestoreFocus(currentFocusedWindow, null, true);
						}
					}
					break;
				}

				case KeyEvent.KEY_TYPED:
				case KeyEvent.KEY_PRESSED:
				case KeyEvent.KEY_RELEASED:
					return TypeAheadAssertions(null, e);

				default:
					return false;
			}

			return true;
		}

		/// <summary>
		/// Called by <code>dispatchEvent</code> if no other
		/// KeyEventDispatcher in the dispatcher chain dispatched the KeyEvent, or
		/// if no other KeyEventDispatchers are registered. If the event has not
		/// been consumed, its target is enabled, and the focus owner is not null,
		/// this method dispatches the event to its target. This method will also
		/// subsequently dispatch the event to all registered
		/// KeyEventPostProcessors. After all this operations are finished,
		/// the event is passed to peers for processing.
		/// <para>
		/// In all cases, this method returns <code>true</code>, since
		/// DefaultKeyboardFocusManager is designed so that neither
		/// <code>dispatchEvent</code>, nor the AWT event dispatcher, should take
		/// further action on the event in any situation.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the KeyEvent to be dispatched </param>
		/// <returns> <code>true</code> </returns>
		/// <seealso cref= Component#dispatchEvent </seealso>
		public override bool DispatchKeyEvent(KeyEvent e)
		{
			Component focusOwner = (((AWTEvent)e).IsPosted) ? FocusOwner : e.Component;

			if (focusOwner != null && focusOwner.Showing && focusOwner.CanBeFocusOwner())
			{
				if (!e.Consumed)
				{
					Component comp = e.Component;
					if (comp != null && comp.Enabled)
					{
						RedispatchEvent(comp, e);
					}
				}
			}
			bool stopPostProcessing = false;
			IList<KeyEventPostProcessor> processors = KeyEventPostProcessors;
			if (processors != null)
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				for (IEnumerator<KeyEventPostProcessor> iter = processors.GetEnumerator(); !stopPostProcessing && iter.hasNext();)
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					stopPostProcessing = iter.next().postProcessKeyEvent(e);
				}
			}
			if (!stopPostProcessing)
			{
				PostProcessKeyEvent(e);
			}

			// Allow the peer to process KeyEvent
			Component source = e.Component;
			ComponentPeer peer = source.Peer;

			if (peer == null || peer is LightweightPeer)
			{
				// if focus owner is lightweight then its native container
				// processes event
				Container target = source.NativeContainer;
				if (target != null)
				{
					peer = target.Peer;
				}
			}
			if (peer != null)
			{
				peer.HandleEvent(e);
			}

			return true;
		}

		/// <summary>
		/// This method will be called by <code>dispatchKeyEvent</code>. It will
		/// handle any unconsumed KeyEvents that map to an AWT
		/// <code>MenuShortcut</code> by consuming the event and activating the
		/// shortcut.
		/// </summary>
		/// <param name="e"> the KeyEvent to post-process </param>
		/// <returns> <code>true</code> </returns>
		/// <seealso cref= #dispatchKeyEvent </seealso>
		/// <seealso cref= MenuShortcut </seealso>
		public override bool PostProcessKeyEvent(KeyEvent e)
		{
			if (!e.Consumed)
			{
				Component target = e.Component;
				Container p = (Container)(target is Container ? target : target.Parent);
				if (p != null)
				{
					p.PostProcessKeyEvent(e);
				}
			}
			return true;
		}

		private void PumpApprovedKeyEvents()
		{
			KeyEvent ke;
			do
			{
				ke = null;
				lock (this)
				{
					if (EnqueuedKeyEvents.Count != 0)
					{
						ke = EnqueuedKeyEvents.First.Value;
						if (TypeAheadMarkers.Count != 0)
						{
							TypeAheadMarker marker = TypeAheadMarkers.First.Value;
							// Fixed 5064013: may appears that the events have the same time
							// if (ke.getWhen() >= marker.after) {
							// The fix is rolled out.

							if (ke.When > marker.After)
							{
								ke = null;
							}
						}
						if (ke != null)
						{
							if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
							{
								FocusLog.finer("Pumping approved event {0}", ke);
							}
							EnqueuedKeyEvents.RemoveFirst();
						}
					}
				}
				if (ke != null)
				{
					PreDispatchKeyEvent(ke);
				}
			} while (ke != null);
		}

		/// <summary>
		/// Dumps the list of type-ahead queue markers to stderr
		/// </summary>
		internal virtual void DumpMarkers()
		{
			if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
			{
				FocusLog.finest(">>> Markers dump, time: {0}", DateTimeHelperClass.CurrentUnixTimeMillis());
				lock (this)
				{
					if (TypeAheadMarkers.Count != 0)
					{
						IEnumerator<TypeAheadMarker> iter = TypeAheadMarkers.GetEnumerator();
						while (iter.MoveNext())
						{
							TypeAheadMarker marker = iter.Current;
							FocusLog.finest("    {0}", marker);
						}
					}
				}
			}
		}

		private bool TypeAheadAssertions(Component target, AWTEvent e)
		{

			// Clear any pending events here as well as in the FOCUS_GAINED
			// handler. We need this call here in case a marker was removed in
			// response to a call to dequeueKeyEvents.
			PumpApprovedKeyEvents();

			switch (e.ID)
			{
				case KeyEvent.KEY_TYPED:
				case KeyEvent.KEY_PRESSED:
				case KeyEvent.KEY_RELEASED:
				{
					KeyEvent ke = (KeyEvent)e;
					lock (this)
					{
						if (e.IsPosted && TypeAheadMarkers.Count != 0)
						{
							TypeAheadMarker marker = TypeAheadMarkers.First.Value;
							// Fixed 5064013: may appears that the events have the same time
							// if (ke.getWhen() >= marker.after) {
							// The fix is rolled out.

							if (ke.When > marker.After)
							{
								if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
								{
									FocusLog.finer("Storing event {0} because of marker {1}", ke, marker);
								}
								EnqueuedKeyEvents.AddLast(ke);
								return true;
							}
						}
					}

					// KeyEvent was posted before focus change request
					return PreDispatchKeyEvent(ke);
				}

				case FocusEvent.FOCUS_GAINED:
					if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
					{
						FocusLog.finest("Markers before FOCUS_GAINED on {0}", target);
					}
					DumpMarkers();
					// Search the marker list for the first marker tied to
					// the Component which just gained focus. Then remove
					// that marker, any markers which immediately follow
					// and are tied to the same component, and all markers
					// that preceed it. This handles the case where
					// multiple focus requests were made for the same
					// Component in a row and when we lost some of the
					// earlier requests. Since FOCUS_GAINED events will
					// not be generated for these additional requests, we
					// need to clear those markers too.
					lock (this)
					{
						bool found = false;
						if (HasMarker(target))
						{
							for (IEnumerator<TypeAheadMarker> iter = TypeAheadMarkers.GetEnumerator(); iter.MoveNext();)
							{
								if (iter.Current.untilFocused == target)
								{
									found = true;
								}
								else if (found)
								{
									break;
								}
								iter.remove();
							}
						}
						else
						{
							// Exception condition - event without marker
							if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
							{
								FocusLog.finer("Event without marker {0}", e);
							}
						}
					}
					FocusLog.finest("Markers after FOCUS_GAINED");
					DumpMarkers();

					RedispatchEvent(target, e);

					// Now, dispatch any pending KeyEvents which have been
					// released because of the FOCUS_GAINED event so that we don't
					// have to wait for another event to be posted to the queue.
					PumpApprovedKeyEvents();
					return true;

				default:
					RedispatchEvent(target, e);
					return true;
			}
		}

		/// <summary>
		/// Returns true if there are some marker associated with component <code>comp</code>
		/// in a markers' queue
		/// @since 1.5
		/// </summary>
		private bool HasMarker(Component comp)
		{
			for (IEnumerator<TypeAheadMarker> iter = TypeAheadMarkers.GetEnumerator(); iter.MoveNext();)
			{
				if (iter.Current.untilFocused == comp)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Clears markers queue
		/// @since 1.5
		/// </summary>
		internal override void ClearMarkers()
		{
			lock (this)
			{
				TypeAheadMarkers.Clear();
			}
		}

		private bool PreDispatchKeyEvent(KeyEvent ke)
		{
			if (((AWTEvent) ke).IsPosted)
			{
				Component focusOwner = FocusOwner;
				ke.Source = ((focusOwner != null) ? focusOwner : FocusedWindow);
			}
			if (ke.Source == null)
			{
				return true;
			}

			// Explicitly set the key event timestamp here (not in Component.dispatchEventImpl):
			// - A key event is anyway passed to this method which starts its actual dispatching.
			// - If a key event is put to the type ahead queue, its time stamp should not be registered
			//   until its dispatching actually starts (by this method).
			EventQueue.CurrentEventAndMostRecentTime = ke;

			/// <summary>
			/// Fix for 4495473.
			/// This fix allows to correctly dispatch events when native
			/// event proxying mechanism is active.
			/// If it is active we should redispatch key events after
			/// we detected its correct target.
			/// </summary>
			if (KeyboardFocusManager.IsProxyActive(ke))
			{
				Component source = (Component)ke.Source;
				Container target = source.NativeContainer;
				if (target != null)
				{
					ComponentPeer peer = target.Peer;
					if (peer != null)
					{
						peer.HandleEvent(ke);
						/// <summary>
						/// Fix for 4478780 - consume event after it was dispatched by peer.
						/// </summary>
						ke.Consume();
					}
				}
				return true;
			}

			IList<KeyEventDispatcher> dispatchers = KeyEventDispatchers;
			if (dispatchers != null)
			{
				for (IEnumerator<KeyEventDispatcher> iter = dispatchers.GetEnumerator(); iter.MoveNext();)
				{
					 if (iter.Current.dispatchKeyEvent(ke))
					 {
						 return true;
					 }
				}
			}
			return DispatchKeyEvent(ke);
		}

		/*
		 * @param e is a KEY_PRESSED event that can be used
		 *          to track the next KEY_TYPED related.
		 */
		private void ConsumeNextKeyTyped(KeyEvent e)
		{
			ConsumeNextKeyTyped_Renamed = true;
		}

		private void ConsumeTraversalKey(KeyEvent e)
		{
			e.Consume();
			ConsumeNextKeyTyped_Renamed = (e.ID == KeyEvent.KEY_PRESSED) && !e.ActionKey;
		}

		/*
		 * return true if event was consumed
		 */
		private bool ConsumeProcessedKeyEvent(KeyEvent e)
		{
			if ((e.ID == KeyEvent.KEY_TYPED) && ConsumeNextKeyTyped_Renamed)
			{
				e.Consume();
				ConsumeNextKeyTyped_Renamed = false;
				return true;
			}
			return false;
		}

		/// <summary>
		/// This method initiates a focus traversal operation if and only if the
		/// KeyEvent represents a focus traversal key for the specified
		/// focusedComponent. It is expected that focusedComponent is the current
		/// focus owner, although this need not be the case. If it is not,
		/// focus traversal will nevertheless proceed as if focusedComponent
		/// were the focus owner.
		/// </summary>
		/// <param name="focusedComponent"> the Component that is the basis for a focus
		///        traversal operation if the specified event represents a focus
		///        traversal key for the Component </param>
		/// <param name="e"> the event that may represent a focus traversal key </param>
		public override void ProcessKeyEvent(Component focusedComponent, KeyEvent e)
		{
			// consume processed event if needed
			if (ConsumeProcessedKeyEvent(e))
			{
				return;
			}

			// KEY_TYPED events cannot be focus traversal keys
			if (e.ID == KeyEvent.KEY_TYPED)
			{
				return;
			}

			if (focusedComponent.FocusTraversalKeysEnabled && !e.Consumed)
			{
				AWTKeyStroke stroke = AWTKeyStroke.GetAWTKeyStrokeForEvent(e), oppStroke = AWTKeyStroke.GetAWTKeyStroke(stroke.KeyCode, stroke.Modifiers, !stroke.OnKeyRelease);
				Set<AWTKeyStroke> toTest;
				bool contains, containsOpp;

				toTest = focusedComponent.GetFocusTraversalKeys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS);
				contains = toTest.Contains(stroke);
				containsOpp = toTest.Contains(oppStroke);

				if (contains || containsOpp)
				{
					ConsumeTraversalKey(e);
					if (contains)
					{
						FocusNextComponent(focusedComponent);
					}
					return;
				}
				else if (e.ID == KeyEvent.KEY_PRESSED)
				{
					// Fix for 6637607: consumeNextKeyTyped should be reset.
					ConsumeNextKeyTyped_Renamed = false;
				}

				toTest = focusedComponent.GetFocusTraversalKeys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS);
				contains = toTest.Contains(stroke);
				containsOpp = toTest.Contains(oppStroke);

				if (contains || containsOpp)
				{
					ConsumeTraversalKey(e);
					if (contains)
					{
						FocusPreviousComponent(focusedComponent);
					}
					return;
				}

				toTest = focusedComponent.GetFocusTraversalKeys(KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS);
				contains = toTest.Contains(stroke);
				containsOpp = toTest.Contains(oppStroke);

				if (contains || containsOpp)
				{
					ConsumeTraversalKey(e);
					if (contains)
					{
						UpFocusCycle(focusedComponent);
					}
					return;
				}

				if (!((focusedComponent is Container) && ((Container)focusedComponent).FocusCycleRoot))
				{
					return;
				}

				toTest = focusedComponent.GetFocusTraversalKeys(KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS);
				contains = toTest.Contains(stroke);
				containsOpp = toTest.Contains(oppStroke);

				if (contains || containsOpp)
				{
					ConsumeTraversalKey(e);
					if (contains)
					{
						DownFocusCycle((Container)focusedComponent);
					}
				}
			}
		}

		/// <summary>
		/// Delays dispatching of KeyEvents until the specified Component becomes
		/// the focus owner. KeyEvents with timestamps later than the specified
		/// timestamp will be enqueued until the specified Component receives a
		/// FOCUS_GAINED event, or the AWT cancels the delay request by invoking
		/// <code>dequeueKeyEvents</code> or <code>discardKeyEvents</code>.
		/// </summary>
		/// <param name="after"> timestamp of current event, or the current, system time if
		///        the current event has no timestamp, or the AWT cannot determine
		///        which event is currently being handled </param>
		/// <param name="untilFocused"> Component which will receive a FOCUS_GAINED event
		///        before any pending KeyEvents </param>
		/// <seealso cref= #dequeueKeyEvents </seealso>
		/// <seealso cref= #discardKeyEvents </seealso>
		protected internal override void EnqueueKeyEvents(long after, Component untilFocused)
		{
			lock (this)
			{
				if (untilFocused == null)
				{
					return;
				}
        
				if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
				{
					FocusLog.finer("Enqueue at {0} for {1}", after, untilFocused);
				}
        
				int insertionIndex = 0, i = TypeAheadMarkers.Count;
				IEnumerator<TypeAheadMarker> iter = TypeAheadMarkers.listIterator(i);
        
				for (; i > 0; i--)
				{
					TypeAheadMarker marker = iter.previous();
					if (marker.After <= after)
					{
						insertionIndex = i;
						break;
					}
				}
        
//JAVA TO C# CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the 2-parameter Java 'add' method:
				TypeAheadMarkers.Add(insertionIndex, new TypeAheadMarker(after, untilFocused));
			}
		}

		/// <summary>
		/// Releases for normal dispatching to the current focus owner all
		/// KeyEvents which were enqueued because of a call to
		/// <code>enqueueKeyEvents</code> with the same timestamp and Component.
		/// If the given timestamp is less than zero, the outstanding enqueue
		/// request for the given Component with the <b>oldest</b> timestamp (if
		/// any) should be cancelled.
		/// </summary>
		/// <param name="after"> the timestamp specified in the call to
		///        <code>enqueueKeyEvents</code>, or any value &lt; 0 </param>
		/// <param name="untilFocused"> the Component specified in the call to
		///        <code>enqueueKeyEvents</code> </param>
		/// <seealso cref= #enqueueKeyEvents </seealso>
		/// <seealso cref= #discardKeyEvents </seealso>
		protected internal override void DequeueKeyEvents(long after, Component untilFocused)
		{
			lock (this)
			{
				if (untilFocused == null)
				{
					return;
				}
        
				if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
				{
					FocusLog.finer("Dequeue at {0} for {1}", after, untilFocused);
				}
        
				TypeAheadMarker marker;
				IEnumerator<TypeAheadMarker> iter = TypeAheadMarkers.listIterator((after >= 0) ? TypeAheadMarkers.Count : 0);
        
				if (after < 0)
				{
					while (iter.MoveNext())
					{
						marker = iter.Current;
						if (marker.UntilFocused == untilFocused)
						{
							iter.remove();
							return;
						}
					}
				}
				else
				{
					while (iter.hasPrevious())
					{
						marker = iter.previous();
						if (marker.UntilFocused == untilFocused && marker.After == after)
						{
							iter.remove();
							return;
						}
					}
				}
			}
		}

		/// <summary>
		/// Discards all KeyEvents which were enqueued because of one or more calls
		/// to <code>enqueueKeyEvents</code> with the specified Component, or one of
		/// its descendants.
		/// </summary>
		/// <param name="comp"> the Component specified in one or more calls to
		///        <code>enqueueKeyEvents</code>, or a parent of such a Component </param>
		/// <seealso cref= #enqueueKeyEvents </seealso>
		/// <seealso cref= #dequeueKeyEvents </seealso>
		protected internal override void DiscardKeyEvents(Component comp)
		{
			lock (this)
			{
				if (comp == null)
				{
					return;
				}
        
				long start = -1;
        
				for (IEnumerator<TypeAheadMarker> iter = TypeAheadMarkers.GetEnumerator(); iter.MoveNext();)
				{
					TypeAheadMarker marker = iter.Current;
					Component toTest = marker.UntilFocused;
					bool match = (toTest == comp);
					while (!match && toTest != null && !(toTest is Window))
					{
						toTest = toTest.Parent;
						match = (toTest == comp);
					}
					if (match)
					{
						if (start < 0)
						{
							start = marker.After;
						}
						iter.remove();
					}
					else if (start >= 0)
					{
						PurgeStampedEvents(start, marker.After);
						start = -1;
					}
				}
        
				PurgeStampedEvents(start, -1);
			}
		}

		// Notes:
		//   * must be called inside a synchronized block
		//   * if 'start' is < 0, then this function does nothing
		//   * if 'end' is < 0, then all KeyEvents from 'start' to the end of the
		//     queue will be removed
		private void PurgeStampedEvents(long start, long end)
		{
			if (start < 0)
			{
				return;
			}

			for (IEnumerator<KeyEvent> iter = EnqueuedKeyEvents.GetEnumerator(); iter.MoveNext();)
			{
				KeyEvent ke = iter.Current;
				long time = ke.When;

				if (start < time && (end < 0 || time <= end))
				{
					iter.remove();
				}

				if (end >= 0 && time > end)
				{
					break;
				}
			}
		}

		/// <summary>
		/// Focuses the Component before aComponent, typically based on a
		/// FocusTraversalPolicy.
		/// </summary>
		/// <param name="aComponent"> the Component that is the basis for the focus
		///        traversal operation </param>
		/// <seealso cref= FocusTraversalPolicy </seealso>
		/// <seealso cref= Component#transferFocusBackward </seealso>
		public override void FocusPreviousComponent(Component aComponent)
		{
			if (aComponent != null)
			{
				aComponent.TransferFocusBackward();
			}
		}

		/// <summary>
		/// Focuses the Component after aComponent, typically based on a
		/// FocusTraversalPolicy.
		/// </summary>
		/// <param name="aComponent"> the Component that is the basis for the focus
		///        traversal operation </param>
		/// <seealso cref= FocusTraversalPolicy </seealso>
		/// <seealso cref= Component#transferFocus </seealso>
		public override void FocusNextComponent(Component aComponent)
		{
			if (aComponent != null)
			{
				aComponent.TransferFocus();
			}
		}

		/// <summary>
		/// Moves the focus up one focus traversal cycle. Typically, the focus owner
		/// is set to aComponent's focus cycle root, and the current focus cycle
		/// root is set to the new focus owner's focus cycle root. If, however,
		/// aComponent's focus cycle root is a Window, then the focus owner is set
		/// to the focus cycle root's default Component to focus, and the current
		/// focus cycle root is unchanged.
		/// </summary>
		/// <param name="aComponent"> the Component that is the basis for the focus
		///        traversal operation </param>
		/// <seealso cref= Component#transferFocusUpCycle </seealso>
		public override void UpFocusCycle(Component aComponent)
		{
			if (aComponent != null)
			{
				aComponent.TransferFocusUpCycle();
			}
		}

		/// <summary>
		/// Moves the focus down one focus traversal cycle. If aContainer is a focus
		/// cycle root, then the focus owner is set to aContainer's default
		/// Component to focus, and the current focus cycle root is set to
		/// aContainer. If aContainer is not a focus cycle root, then no focus
		/// traversal operation occurs.
		/// </summary>
		/// <param name="aContainer"> the Container that is the basis for the focus
		///        traversal operation </param>
		/// <seealso cref= Container#transferFocusDownCycle </seealso>
		public override void DownFocusCycle(Container aContainer)
		{
			if (aContainer != null && aContainer.FocusCycleRoot)
			{
				aContainer.TransferFocusDownCycle();
			}
		}
	}

}