/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The listener interface for receiving drag gesture events.
	/// This interface is intended for a drag gesture recognition
	/// implementation. See a specification for {@code DragGestureRecognizer}
	/// for details on how to register the listener interface.
	/// Upon recognition of a drag gesture the {@code
	/// DragGestureRecognizer} calls this interface's
	/// <seealso cref="#dragGestureRecognized dragGestureRecognized()"/>
	/// method and passes a {@code DragGestureEvent}.
	/// 
	/// </summary>
	/// <seealso cref= java.awt.dnd.DragGestureRecognizer </seealso>
	/// <seealso cref= java.awt.dnd.DragGestureEvent </seealso>
	/// <seealso cref= java.awt.dnd.DragSource </seealso>

	 public interface DragGestureListener : EventListener
	 {

		/// <summary>
		/// This method is invoked by the {@code DragGestureRecognizer}
		/// when the {@code DragGestureRecognizer} detects a platform-dependent
		/// drag initiating gesture. To initiate the drag and drop operation,
		/// if appropriate, <seealso cref="DragGestureEvent#startDrag startDrag()"/> method on
		/// the {@code DragGestureEvent} has to be invoked.
		/// <P> </summary>
		/// <seealso cref= java.awt.dnd.DragGestureRecognizer </seealso>
		/// <seealso cref= java.awt.dnd.DragGestureEvent </seealso>
		/// <param name="dge"> the <code>DragGestureEvent</code> describing
		/// the gesture that has just occurred </param>

		 void DragGestureRecognized(DragGestureEvent dge);
	 }

}