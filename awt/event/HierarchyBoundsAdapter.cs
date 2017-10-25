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
	/// An abstract adapter class for receiving ancestor moved and resized events.
	/// The methods in this class are empty. This class exists as a
	/// convenience for creating listener objects.
	/// <para>
	/// Extend this class and override the method for the event of interest. (If
	/// you implement the <code>HierarchyBoundsListener</code> interface, you have
	/// to define both methods in it. This abstract class defines null methods for
	/// them both, so you only have to define the method for the event you care
	/// about.)
	/// </para>
	/// <para>
	/// Create a listener object using your class and then register it with a
	/// Component using the Component's <code>addHierarchyBoundsListener</code>
	/// method. When the hierarchy to which the Component belongs changes by
	/// resize or movement of an ancestor, the relevant method in the listener
	/// object is invoked, and the <code>HierarchyEvent</code> is passed to it.
	/// 
	/// @author      David Mendenhall
	/// </para>
	/// </summary>
	/// <seealso cref=         HierarchyBoundsListener </seealso>
	/// <seealso cref=         HierarchyEvent
	/// @since       1.3 </seealso>
	public abstract class HierarchyBoundsAdapter : HierarchyBoundsListener
	{
		/// <summary>
		/// Called when an ancestor of the source is moved.
		/// </summary>
		public virtual void AncestorMoved(HierarchyEvent e)
		{
		}

		/// <summary>
		/// Called when an ancestor of the source is resized.
		/// </summary>
		public virtual void AncestorResized(HierarchyEvent e)
		{
		}
	}

}