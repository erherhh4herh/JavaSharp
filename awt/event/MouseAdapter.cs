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
	/// An abstract adapter class for receiving mouse events.
	/// The methods in this class are empty. This class exists as
	/// convenience for creating listener objects.
	/// <P>
	/// Mouse events let you track when a mouse is pressed, released, clicked,
	/// moved, dragged, when it enters a component, when it exits and
	/// when a mouse wheel is moved.
	/// <P>
	/// Extend this class to create a {@code MouseEvent}
	/// (including drag and motion events) or/and {@code MouseWheelEvent}
	/// listener and override the methods for the events of interest. (If you implement the
	/// {@code MouseListener},
	/// {@code MouseMotionListener}
	/// interface, you have to define all of
	/// the methods in it. This abstract class defines null methods for them
	/// all, so you can only have to define methods for events you care about.)
	/// <P>
	/// Create a listener object using the extended class and then register it with
	/// a component using the component's {@code addMouseListener}
	/// {@code addMouseMotionListener}, {@code addMouseWheelListener}
	/// methods.
	/// The relevant method in the listener object is invoked  and the {@code MouseEvent}
	/// or {@code MouseWheelEvent}  is passed to it in following cases:
	/// <ul>
	/// <li>when a mouse button is pressed, released, or clicked (pressed and  released)
	/// <li>when the mouse cursor enters or exits the component
	/// <li>when the mouse wheel rotated, or mouse moved or dragged
	/// </ul>
	/// 
	/// @author Carl Quinn
	/// @author Andrei Dmitriev
	/// </summary>
	/// <seealso cref= MouseEvent </seealso>
	/// <seealso cref= MouseWheelEvent </seealso>
	/// <seealso cref= MouseListener </seealso>
	/// <seealso cref= MouseMotionListener </seealso>
	/// <seealso cref= MouseWheelListener </seealso>
	/// <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/mouselistener.html">Tutorial: Writing a Mouse Listener</a>
	/// 
	/// @since 1.1 </seealso>
	public abstract class MouseAdapter : MouseListener, MouseWheelListener, MouseMotionListener
	{
		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual void MouseClicked(MouseEvent e)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual void MousePressed(MouseEvent e)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual void MouseReleased(MouseEvent e)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual void MouseEntered(MouseEvent e)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual void MouseExited(MouseEvent e)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.6
		/// </summary>
		public virtual void MouseWheelMoved(MouseWheelEvent e)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.6
		/// </summary>
		public virtual void MouseDragged(MouseEvent e)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.6
		/// </summary>
		public virtual void MouseMoved(MouseEvent e)
		{
		}
	}

}