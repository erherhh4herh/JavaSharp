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
	/// The listener interface for receiving mouse motion events on a component.
	/// (For clicks and other mouse events, use the <code>MouseListener</code>.)
	/// <P>
	/// The class that is interested in processing a mouse motion event
	/// either implements this interface (and all the methods it
	/// contains) or extends the abstract <code>MouseMotionAdapter</code> class
	/// (overriding only the methods of interest).
	/// <P>
	/// The listener object created from that class is then registered with a
	/// component using the component's <code>addMouseMotionListener</code>
	/// method. A mouse motion event is generated when the mouse is moved
	/// or dragged. (Many such events will be generated). When a mouse motion event
	/// occurs, the relevant method in the listener object is invoked, and
	/// the <code>MouseEvent</code> is passed to it.
	/// 
	/// @author Amy Fowler
	/// </summary>
	/// <seealso cref= MouseMotionAdapter </seealso>
	/// <seealso cref= MouseEvent </seealso>
	/// <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/mousemotionlistener.html">Tutorial: Writing a Mouse Motion Listener</a>
	/// 
	/// @since 1.1 </seealso>
	public interface MouseMotionListener : EventListener
	{

		/// <summary>
		/// Invoked when a mouse button is pressed on a component and then
		/// dragged.  <code>MOUSE_DRAGGED</code> events will continue to be
		/// delivered to the component where the drag originated until the
		/// mouse button is released (regardless of whether the mouse position
		/// is within the bounds of the component).
		/// <para>
		/// Due to platform-dependent Drag&amp;Drop implementations,
		/// <code>MOUSE_DRAGGED</code> events may not be delivered during a native
		/// Drag&amp;Drop operation.
		/// </para>
		/// </summary>
		void MouseDragged(MouseEvent e);

		/// <summary>
		/// Invoked when the mouse cursor has been moved onto a component
		/// but no buttons have been pushed.
		/// </summary>
		void MouseMoved(MouseEvent e);

	}

}