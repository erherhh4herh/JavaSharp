/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A class extends <code>AWTEventMulticaster</code> to implement efficient and
	/// thread-safe multi-cast event dispatching for the drag-and-drop events defined
	/// in the java.awt.dnd package.
	/// 
	/// @since       1.4 </summary>
	/// <seealso cref= AWTEventMulticaster </seealso>

	internal class DnDEventMulticaster : AWTEventMulticaster, DragSourceListener, DragSourceMotionListener
	{

		/// <summary>
		/// Creates an event multicaster instance which chains listener-a
		/// with listener-b. Input parameters <code>a</code> and <code>b</code>
		/// should not be <code>null</code>, though implementations may vary in
		/// choosing whether or not to throw <code>NullPointerException</code>
		/// in that case.
		/// </summary>
		/// <param name="a"> listener-a </param>
		/// <param name="b"> listener-b </param>
		protected internal DnDEventMulticaster(EventListener a, EventListener b) : base(a,b)
		{
		}

		/// <summary>
		/// Handles the <code>DragSourceDragEvent</code> by invoking
		/// <code>dragEnter</code> on listener-a and listener-b.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DragEnter(DragSourceDragEvent dsde)
		{
			((DragSourceListener)a).DragEnter(dsde);
			((DragSourceListener)b).DragEnter(dsde);
		}

		/// <summary>
		/// Handles the <code>DragSourceDragEvent</code> by invoking
		/// <code>dragOver</code> on listener-a and listener-b.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DragOver(DragSourceDragEvent dsde)
		{
			((DragSourceListener)a).DragOver(dsde);
			((DragSourceListener)b).DragOver(dsde);
		}

		/// <summary>
		/// Handles the <code>DragSourceDragEvent</code> by invoking
		/// <code>dropActionChanged</code> on listener-a and listener-b.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DropActionChanged(DragSourceDragEvent dsde)
		{
			((DragSourceListener)a).DropActionChanged(dsde);
			((DragSourceListener)b).DropActionChanged(dsde);
		}

		/// <summary>
		/// Handles the <code>DragSourceEvent</code> by invoking
		/// <code>dragExit</code> on listener-a and listener-b.
		/// </summary>
		/// <param name="dse"> the <code>DragSourceEvent</code> </param>
		public virtual void DragExit(DragSourceEvent dse)
		{
			((DragSourceListener)a).DragExit(dse);
			((DragSourceListener)b).DragExit(dse);
		}

		/// <summary>
		/// Handles the <code>DragSourceDropEvent</code> by invoking
		/// <code>dragDropEnd</code> on listener-a and listener-b.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDropEvent</code> </param>
		public virtual void DragDropEnd(DragSourceDropEvent dsde)
		{
			((DragSourceListener)a).DragDropEnd(dsde);
			((DragSourceListener)b).DragDropEnd(dsde);
		}

		/// <summary>
		/// Handles the <code>DragSourceDragEvent</code> by invoking
		/// <code>dragMouseMoved</code> on listener-a and listener-b.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DragMouseMoved(DragSourceDragEvent dsde)
		{
			((DragSourceMotionListener)a).DragMouseMoved(dsde);
			((DragSourceMotionListener)b).DragMouseMoved(dsde);
		}

		/// <summary>
		/// Adds drag-source-listener-a with drag-source-listener-b and
		/// returns the resulting multicast listener.
		/// </summary>
		/// <param name="a"> drag-source-listener-a </param>
		/// <param name="b"> drag-source-listener-b </param>
		public static DragSourceListener Add(DragSourceListener a, DragSourceListener b)
		{
			return (DragSourceListener)AddInternal(a, b);
		}

		/// <summary>
		/// Adds drag-source-motion-listener-a with drag-source-motion-listener-b and
		/// returns the resulting multicast listener.
		/// </summary>
		/// <param name="a"> drag-source-motion-listener-a </param>
		/// <param name="b"> drag-source-motion-listener-b </param>
		public static DragSourceMotionListener Add(DragSourceMotionListener a, DragSourceMotionListener b)
		{
			return (DragSourceMotionListener)AddInternal(a, b);
		}

		/// <summary>
		/// Removes the old drag-source-listener from drag-source-listener-l
		/// and returns the resulting multicast listener.
		/// </summary>
		/// <param name="l"> drag-source-listener-l </param>
		/// <param name="oldl"> the drag-source-listener being removed </param>
		public static DragSourceListener Remove(DragSourceListener l, DragSourceListener oldl)
		{
			return (DragSourceListener)RemoveInternal(l, oldl);
		}

		/// <summary>
		/// Removes the old drag-source-motion-listener from
		/// drag-source-motion-listener-l and returns the resulting multicast
		/// listener.
		/// </summary>
		/// <param name="l"> drag-source-motion-listener-l </param>
		/// <param name="ol"> the drag-source-motion-listener being removed </param>
		public static DragSourceMotionListener Remove(DragSourceMotionListener l, DragSourceMotionListener ol)
		{
			return (DragSourceMotionListener)RemoveInternal(l, ol);
		}

		/// <summary>
		/// Returns the resulting multicast listener from adding listener-a
		/// and listener-b together.
		/// If listener-a is null, it returns listener-b;
		/// If listener-b is null, it returns listener-a
		/// If neither are null, then it creates and returns
		/// a new AWTEventMulticaster instance which chains a with b. </summary>
		/// <param name="a"> event listener-a </param>
		/// <param name="b"> event listener-b </param>
		protected internal static EventListener AddInternal(EventListener a, EventListener b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			return new DnDEventMulticaster(a, b);
		}

		/// <summary>
		/// Removes a listener from this multicaster and returns the
		/// resulting multicast listener. </summary>
		/// <param name="oldl"> the listener to be removed </param>
		protected internal override EventListener Remove(EventListener oldl)
		{
			if (oldl == a)
			{
				return b;
			}
			if (oldl == b)
			{
				return a;
			}
			EventListener a2 = RemoveInternal(a, oldl);
			EventListener b2 = RemoveInternal(b, oldl);
			if (a2 == a && b2 == b)
			{
				return this; // it's not here
			}
			return AddInternal(a2, b2);
		}

		/// <summary>
		/// Returns the resulting multicast listener after removing the
		/// old listener from listener-l.
		/// If listener-l equals the old listener OR listener-l is null,
		/// returns null.
		/// Else if listener-l is an instance of AWTEventMulticaster,
		/// then it removes the old listener from it.
		/// Else, returns listener l. </summary>
		/// <param name="l"> the listener being removed from </param>
		/// <param name="oldl"> the listener being removed </param>
		protected internal static EventListener RemoveInternal(EventListener l, EventListener oldl)
		{
			if (l == oldl || l == null)
			{
				return null;
			}
			else if (l is DnDEventMulticaster)
			{
				return ((DnDEventMulticaster)l).Remove(oldl);
			}
			else
			{
				return l; // it's not here
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void save(java.io.ObjectOutputStream s, String k, java.util.EventListener l) throws java.io.IOException
		protected internal static void Save(ObjectOutputStream s, String k, EventListener l)
		{
			AWTEventMulticaster.Save(s, k, l);
		}
	}

}