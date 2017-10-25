/*
 * Copyright (c) 2001, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.dnd
{

	/// <summary>
	/// An abstract adapter class for receiving drag source events. The methods in
	/// this class are empty. This class exists only as a convenience for creating
	/// listener objects.
	/// <para>
	/// Extend this class to create a <code>DragSourceEvent</code> listener
	/// and override the methods for the events of interest. (If you implement the
	/// <code>DragSourceListener</code> interface, you have to define all of
	/// the methods in it. This abstract class defines null methods for them
	/// all, so you only have to define methods for events you care about.)
	/// </para>
	/// <para>
	/// Create a listener object using the extended class and then register it with
	/// a <code>DragSource</code>. When the drag enters, moves over, or exits
	/// a drop site, when the drop action changes, and when the drag ends, the
	/// relevant method in the listener object is invoked, and the
	/// <code>DragSourceEvent</code> is passed to it.
	/// </para>
	/// <para>
	/// The drop site is <i>associated with the previous <code>dragEnter()</code>
	/// invocation</i> if the latest invocation of <code>dragEnter()</code> on this
	/// adapter corresponds to that drop site and is not followed by a
	/// <code>dragExit()</code> invocation on this adapter.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= DragSourceEvent </seealso>
	/// <seealso cref= DragSourceListener </seealso>
	/// <seealso cref= DragSourceMotionListener
	/// 
	/// @author David Mendenhall
	/// @since 1.4 </seealso>
	public abstract class DragSourceAdapter : DragSourceListener, DragSourceMotionListener
	{

		/// <summary>
		/// Called as the cursor's hotspot enters a platform-dependent drop site.
		/// This method is invoked when all the following conditions are true:
		/// <UL>
		/// <LI>The cursor's hotspot enters the operable part of
		/// a platform-dependent drop site.
		/// <LI>The drop site is active.
		/// <LI>The drop site accepts the drag.
		/// </UL>
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DragEnter(DragSourceDragEvent dsde)
		{
		}

		/// <summary>
		/// Called as the cursor's hotspot moves over a platform-dependent drop site.
		/// This method is invoked when all the following conditions are true:
		/// <UL>
		/// <LI>The cursor's hotspot has moved, but still intersects the
		/// operable part of the drop site associated with the previous
		/// dragEnter() invocation.
		/// <LI>The drop site is still active.
		/// <LI>The drop site accepts the drag.
		/// </UL>
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DragOver(DragSourceDragEvent dsde)
		{
		}

		/// <summary>
		/// Called whenever the mouse is moved during a drag operation.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DragMouseMoved(DragSourceDragEvent dsde)
		{
		}

		/// <summary>
		/// Called when the user has modified the drop gesture.
		/// This method is invoked when the state of the input
		/// device(s) that the user is interacting with changes.
		/// Such devices are typically the mouse buttons or keyboard
		/// modifiers that the user is interacting with.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DropActionChanged(DragSourceDragEvent dsde)
		{
		}

		/// <summary>
		/// Called as the cursor's hotspot exits a platform-dependent drop site.
		/// This method is invoked when any of the following conditions are true:
		/// <UL>
		/// <LI>The cursor's hotspot no longer intersects the operable part
		/// of the drop site associated with the previous dragEnter() invocation.
		/// </UL>
		/// OR
		/// <UL>
		/// <LI>The drop site associated with the previous dragEnter() invocation
		/// is no longer active.
		/// </UL>
		/// OR
		/// <UL>
		/// <LI> The drop site associated with the previous dragEnter() invocation
		/// has rejected the drag.
		/// </UL>
		/// </summary>
		/// <param name="dse"> the <code>DragSourceEvent</code> </param>
		public virtual void DragExit(DragSourceEvent dse)
		{
		}

		/// <summary>
		/// This method is invoked to signify that the Drag and Drop
		/// operation is complete. The getDropSuccess() method of
		/// the <code>DragSourceDropEvent</code> can be used to
		/// determine the termination state. The getDropAction() method
		/// returns the operation that the drop site selected
		/// to apply to the Drop operation. Once this method is complete, the
		/// current <code>DragSourceContext</code> and
		/// associated resources become invalid.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDropEvent</code> </param>
		public virtual void DragDropEnd(DragSourceDropEvent dsde)
		{
		}
	}

}