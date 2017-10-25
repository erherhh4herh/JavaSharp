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

namespace java.awt.dnd
{

	/// <summary>
	/// A listener interface for receiving mouse motion events during a drag
	/// operation.
	/// <para>
	/// The class that is interested in processing mouse motion events during
	/// a drag operation either implements this interface or extends the abstract
	/// <code>DragSourceAdapter</code> class (overriding only the methods of
	/// interest).
	/// </para>
	/// <para>
	/// Create a listener object using that class and then register it with
	/// a <code>DragSource</code>. Whenever the mouse moves during a drag
	/// operation initiated with this <code>DragSource</code>, that object's
	/// <code>dragMouseMoved</code> method is invoked, and the
	/// <code>DragSourceDragEvent</code> is passed to it.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= DragSourceDragEvent </seealso>
	/// <seealso cref= DragSource </seealso>
	/// <seealso cref= DragSourceListener </seealso>
	/// <seealso cref= DragSourceAdapter
	/// 
	/// @since 1.4 </seealso>

	public interface DragSourceMotionListener : EventListener
	{

		/// <summary>
		/// Called whenever the mouse is moved during a drag operation.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		void DragMouseMoved(DragSourceDragEvent dsde);
	}

}