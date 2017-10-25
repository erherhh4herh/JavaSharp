/*
 * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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
	/// The listener interface for receiving ancestor moved and resized events.
	/// The class that is interested in processing these events either implements
	/// this interface (and all the methods it contains) or extends the abstract
	/// <code>HierarchyBoundsAdapter</code> class (overriding only the method of
	/// interest).
	/// The listener object created from that class is then registered with a
	/// Component using the Component's <code>addHierarchyBoundsListener</code>
	/// method. When the hierarchy to which the Component belongs changes by
	/// the resizing or movement of an ancestor, the relevant method in the listener
	/// object is invoked, and the <code>HierarchyEvent</code> is passed to it.
	/// <para>
	/// Hierarchy events are provided for notification purposes ONLY;
	/// The AWT will automatically handle changes to the hierarchy internally so
	/// that GUI layout works properly regardless of whether a
	/// program registers an <code>HierarchyBoundsListener</code> or not.
	/// 
	/// @author      David Mendenhall
	/// </para>
	/// </summary>
	/// <seealso cref=         HierarchyBoundsAdapter </seealso>
	/// <seealso cref=         HierarchyEvent
	/// @since       1.3 </seealso>
	public interface HierarchyBoundsListener : EventListener
	{
		/// <summary>
		/// Called when an ancestor of the source is moved.
		/// </summary>
		void AncestorMoved(HierarchyEvent e);

		/// <summary>
		/// Called when an ancestor of the source is resized.
		/// </summary>
		void AncestorResized(HierarchyEvent e);
	}

}