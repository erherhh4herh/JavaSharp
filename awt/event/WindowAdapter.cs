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

namespace java.awt.@event
{

	/// <summary>
	/// An abstract adapter class for receiving window events.
	/// The methods in this class are empty. This class exists as
	/// convenience for creating listener objects.
	/// <P>
	/// Extend this class to create a <code>WindowEvent</code> listener
	/// and override the methods for the events of interest. (If you implement the
	/// <code>WindowListener</code> interface, you have to define all of
	/// the methods in it. This abstract class defines null methods for them
	/// all, so you can only have to define methods for events you care about.)
	/// <P>
	/// Create a listener object using the extended class and then register it with
	/// a Window using the window's <code>addWindowListener</code>
	/// method. When the window's status changes by virtue of being opened,
	/// closed, activated or deactivated, iconified or deiconified,
	/// the relevant method in the listener
	/// object is invoked, and the <code>WindowEvent</code> is passed to it.
	/// </summary>
	/// <seealso cref= WindowEvent </seealso>
	/// <seealso cref= WindowListener </seealso>
	/// <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/windowlistener.html">Tutorial: Writing a Window Listener</a>
	/// 
	/// @author Carl Quinn
	/// @author Amy Fowler
	/// @author David Mendenhall
	/// @since 1.1 </seealso>
	public abstract class WindowAdapter : WindowListener, WindowStateListener, WindowFocusListener
	{
		/// <summary>
		/// Invoked when a window has been opened.
		/// </summary>
		public virtual void WindowOpened(WindowEvent e)
		{
		}

		/// <summary>
		/// Invoked when a window is in the process of being closed.
		/// The close operation can be overridden at this point.
		/// </summary>
		public virtual void WindowClosing(WindowEvent e)
		{
		}

		/// <summary>
		/// Invoked when a window has been closed.
		/// </summary>
		public virtual void WindowClosed(WindowEvent e)
		{
		}

		/// <summary>
		/// Invoked when a window is iconified.
		/// </summary>
		public virtual void WindowIconified(WindowEvent e)
		{
		}

		/// <summary>
		/// Invoked when a window is de-iconified.
		/// </summary>
		public virtual void WindowDeiconified(WindowEvent e)
		{
		}

		/// <summary>
		/// Invoked when a window is activated.
		/// </summary>
		public virtual void WindowActivated(WindowEvent e)
		{
		}

		/// <summary>
		/// Invoked when a window is de-activated.
		/// </summary>
		public virtual void WindowDeactivated(WindowEvent e)
		{
		}

		/// <summary>
		/// Invoked when a window state is changed.
		/// @since 1.4
		/// </summary>
		public virtual void WindowStateChanged(WindowEvent e)
		{
		}

		/// <summary>
		/// Invoked when the Window is set to be the focused Window, which means
		/// that the Window, or one of its subcomponents, will receive keyboard
		/// events.
		/// 
		/// @since 1.4
		/// </summary>
		public virtual void WindowGainedFocus(WindowEvent e)
		{
		}

		/// <summary>
		/// Invoked when the Window is no longer the focused Window, which means
		/// that keyboard events will no longer be delivered to the Window or any of
		/// its subcomponents.
		/// 
		/// @since 1.4
		/// </summary>
		public virtual void WindowLostFocus(WindowEvent e)
		{
		}
	}

}