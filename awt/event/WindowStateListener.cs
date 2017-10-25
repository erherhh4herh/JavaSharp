/*
 * Copyright (c) 2001, Oracle and/or its affiliates. All rights reserved.
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
	/// The listener interface for receiving window state events.
	/// <para>
	/// The class that is interested in processing a window state event
	/// either implements this interface (and all the methods it contains)
	/// or extends the abstract <code>WindowAdapter</code> class
	/// (overriding only the methods of interest).
	/// </para>
	/// <para>
	/// The listener object created from that class is then registered with
	/// a window using the <code>Window</code>'s
	/// <code>addWindowStateListener</code> method.  When the window's
	/// state changes by virtue of being iconified, maximized etc., the
	/// <code>windowStateChanged</code> method in the listener object is
	/// invoked, and the <code>WindowEvent</code> is passed to it.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.event.WindowAdapter </seealso>
	/// <seealso cref= java.awt.event.WindowEvent
	/// 
	/// @since 1.4 </seealso>
	public interface WindowStateListener : EventListener
	{
		/// <summary>
		/// Invoked when window state is changed.
		/// </summary>
		void WindowStateChanged(WindowEvent e);
	}

}