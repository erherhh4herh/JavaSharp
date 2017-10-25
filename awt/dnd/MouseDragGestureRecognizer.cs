/*
 * Copyright (c) 1998, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// This abstract subclass of <code>DragGestureRecognizer</code>
	/// defines a <code>DragGestureRecognizer</code>
	/// for mouse-based gestures.
	/// 
	/// Each platform implements its own concrete subclass of this class,
	/// available via the Toolkit.createDragGestureRecognizer() method,
	/// to encapsulate
	/// the recognition of the platform dependent mouse gesture(s) that initiate
	/// a Drag and Drop operation.
	/// <para>
	/// Mouse drag gesture recognizers should honor the
	/// drag gesture motion threshold, available through
	/// <seealso cref="DragSource#getDragThreshold"/>.
	/// A drag gesture should be recognized only when the distance
	/// in either the horizontal or vertical direction between
	/// the location of the latest mouse dragged event and the
	/// location of the corresponding mouse button pressed event
	/// is greater than the drag gesture motion threshold.
	/// </para>
	/// <para>
	/// Drag gesture recognizers created with
	/// <seealso cref="DragSource#createDefaultDragGestureRecognizer"/>
	/// follow this convention.
	/// 
	/// @author Laurence P. G. Cable
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.dnd.DragGestureListener </seealso>
	/// <seealso cref= java.awt.dnd.DragGestureEvent </seealso>
	/// <seealso cref= java.awt.dnd.DragSource </seealso>

	public abstract class MouseDragGestureRecognizer : DragGestureRecognizer, MouseListener, MouseMotionListener
	{

		private const long SerialVersionUID = 6220099344182281120L;

		/// <summary>
		/// Construct a new <code>MouseDragGestureRecognizer</code>
		/// given the <code>DragSource</code> for the
		/// <code>Component</code> c, the <code>Component</code>
		/// to observe, the action(s)
		/// permitted for this drag operation, and
		/// the <code>DragGestureListener</code> to
		/// notify when a drag gesture is detected.
		/// <P> </summary>
		/// <param name="ds">  The DragSource for the Component c </param>
		/// <param name="c">   The Component to observe </param>
		/// <param name="act"> The actions permitted for this Drag </param>
		/// <param name="dgl"> The DragGestureListener to notify when a gesture is detected
		///  </param>

		protected internal MouseDragGestureRecognizer(DragSource ds, Component c, int act, DragGestureListener dgl) : base(ds, c, act, dgl)
		{
		}

		/// <summary>
		/// Construct a new <code>MouseDragGestureRecognizer</code>
		/// given the <code>DragSource</code> for
		/// the <code>Component</code> c,
		/// the <code>Component</code> to observe, and the action(s)
		/// permitted for this drag operation.
		/// <P> </summary>
		/// <param name="ds">  The DragSource for the Component c </param>
		/// <param name="c">   The Component to observe </param>
		/// <param name="act"> The actions permitted for this drag </param>

		protected internal MouseDragGestureRecognizer(DragSource ds, Component c, int act) : this(ds, c, act, null)
		{
		}

		/// <summary>
		/// Construct a new <code>MouseDragGestureRecognizer</code>
		/// given the <code>DragSource</code> for the
		/// <code>Component</code> c, and the
		/// <code>Component</code> to observe.
		/// <P> </summary>
		/// <param name="ds">  The DragSource for the Component c </param>
		/// <param name="c">   The Component to observe </param>

		protected internal MouseDragGestureRecognizer(DragSource ds, Component c) : this(ds, c, DnDConstants.ACTION_NONE)
		{
		}

		/// <summary>
		/// Construct a new <code>MouseDragGestureRecognizer</code>
		/// given the <code>DragSource</code> for the <code>Component</code>.
		/// <P> </summary>
		/// <param name="ds">  The DragSource for the Component </param>

		protected internal MouseDragGestureRecognizer(DragSource ds) : this(ds, null)
		{
		}

		/// <summary>
		/// register this DragGestureRecognizer's Listeners with the Component
		/// </summary>

		protected internal override void RegisterListeners()
		{
			Component_Renamed.AddMouseListener(this);
			Component_Renamed.AddMouseMotionListener(this);
		}

		/// <summary>
		/// unregister this DragGestureRecognizer's Listeners with the Component
		/// 
		/// subclasses must override this method
		/// </summary>


		protected internal override void UnregisterListeners()
		{
			Component_Renamed.RemoveMouseListener(this);
			Component_Renamed.RemoveMouseMotionListener(this);
		}

		/// <summary>
		/// Invoked when the mouse has been clicked on a component.
		/// <P> </summary>
		/// <param name="e"> the <code>MouseEvent</code> </param>

		public virtual void MouseClicked(MouseEvent e)
		{
		}

		/// <summary>
		/// Invoked when a mouse button has been
		/// pressed on a <code>Component</code>.
		/// <P> </summary>
		/// <param name="e"> the <code>MouseEvent</code> </param>

		public virtual void MousePressed(MouseEvent e)
		{
		}

		/// <summary>
		/// Invoked when a mouse button has been released on a component.
		/// <P> </summary>
		/// <param name="e"> the <code>MouseEvent</code> </param>

		public virtual void MouseReleased(MouseEvent e)
		{
		}

		/// <summary>
		/// Invoked when the mouse enters a component.
		/// <P> </summary>
		/// <param name="e"> the <code>MouseEvent</code> </param>

		public virtual void MouseEntered(MouseEvent e)
		{
		}

		/// <summary>
		/// Invoked when the mouse exits a component.
		/// <P> </summary>
		/// <param name="e"> the <code>MouseEvent</code> </param>

		public virtual void MouseExited(MouseEvent e)
		{
		}

		/// <summary>
		/// Invoked when a mouse button is pressed on a component.
		/// <P> </summary>
		/// <param name="e"> the <code>MouseEvent</code> </param>

		public virtual void MouseDragged(MouseEvent e)
		{
		}

		/// <summary>
		/// Invoked when the mouse button has been moved on a component
		/// (with no buttons no down).
		/// <P> </summary>
		/// <param name="e"> the <code>MouseEvent</code> </param>

		public virtual void MouseMoved(MouseEvent e)
		{
		}
	}

}