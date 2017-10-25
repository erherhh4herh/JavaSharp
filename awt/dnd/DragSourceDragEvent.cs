/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>DragSourceDragEvent</code> is
	/// delivered from the <code>DragSourceContextPeer</code>,
	/// via the <code>DragSourceContext</code>, to the <code>DragSourceListener</code>
	/// registered with that <code>DragSourceContext</code> and with its associated
	/// <code>DragSource</code>.
	/// <para>
	/// The <code>DragSourceDragEvent</code> reports the <i>target drop action</i>
	/// and the <i>user drop action</i> that reflect the current state of
	/// the drag operation.
	/// </para>
	/// <para>
	/// <i>Target drop action</i> is one of <code>DnDConstants</code> that represents
	/// the drop action selected by the current drop target if this drop action is
	/// supported by the drag source or <code>DnDConstants.ACTION_NONE</code> if this
	/// drop action is not supported by the drag source.
	/// </para>
	/// <para>
	/// <i>User drop action</i> depends on the drop actions supported by the drag
	/// source and the drop action selected by the user. The user can select a drop
	/// action by pressing modifier keys during the drag operation:
	/// <pre>
	///   Ctrl + Shift -&gt; ACTION_LINK
	///   Ctrl         -&gt; ACTION_COPY
	///   Shift        -&gt; ACTION_MOVE
	/// </pre>
	/// If the user selects a drop action, the <i>user drop action</i> is one of
	/// <code>DnDConstants</code> that represents the selected drop action if this
	/// drop action is supported by the drag source or
	/// <code>DnDConstants.ACTION_NONE</code> if this drop action is not supported
	/// by the drag source.
	/// </para>
	/// <para>
	/// If the user doesn't select a drop action, the set of
	/// <code>DnDConstants</code> that represents the set of drop actions supported
	/// by the drag source is searched for <code>DnDConstants.ACTION_MOVE</code>,
	/// then for <code>DnDConstants.ACTION_COPY</code>, then for
	/// <code>DnDConstants.ACTION_LINK</code> and the <i>user drop action</i> is the
	/// first constant found. If no constant is found the <i>user drop action</i>
	/// is <code>DnDConstants.ACTION_NONE</code>.
	/// 
	/// @since 1.2
	/// 
	/// </para>
	/// </summary>

	public class DragSourceDragEvent : DragSourceEvent
	{

		private const long SerialVersionUID = 481346297933902471L;

		/// <summary>
		/// Constructs a <code>DragSourceDragEvent</code>.
		/// This class is typically
		/// instantiated by the <code>DragSourceContextPeer</code>
		/// rather than directly
		/// by client code.
		/// The coordinates for this <code>DragSourceDragEvent</code>
		/// are not specified, so <code>getLocation</code> will return
		/// <code>null</code> for this event.
		/// <para>
		/// The arguments <code>dropAction</code> and <code>action</code> should
		/// be one of <code>DnDConstants</code> that represents a single action.
		/// The argument <code>modifiers</code> should be either a bitwise mask
		/// of old <code>java.awt.event.InputEvent.*_MASK</code> constants or a
		/// bitwise mask of extended <code>java.awt.event.InputEvent.*_DOWN_MASK</code>
		/// constants.
		/// This constructor does not throw any exception for invalid <code>dropAction</code>,
		/// <code>action</code> and <code>modifiers</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dsc"> the <code>DragSourceContext</code> that is to manage
		///            notifications for this event. </param>
		/// <param name="dropAction"> the user drop action. </param>
		/// <param name="action"> the target drop action. </param>
		/// <param name="modifiers"> the modifier keys down during event (shift, ctrl,
		///        alt, meta)
		///        Either extended _DOWN_MASK or old _MASK modifiers
		///        should be used, but both models should not be mixed
		///        in one event. Use of the extended modifiers is
		///        preferred.
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		/// </exception>
		/// <seealso cref= java.awt.event.InputEvent </seealso>
		/// <seealso cref= DragSourceEvent#getLocation </seealso>

		public DragSourceDragEvent(DragSourceContext dsc, int dropAction, int action, int modifiers) : base(dsc)
		{

			TargetActions_Renamed = action;
			GestureModifiers_Renamed = modifiers;
			this.DropAction_Renamed = dropAction;
			if ((modifiers & ~(JDK_1_3_MODIFIERS | JDK_1_4_MODIFIERS)) != 0)
			{
				InvalidModifiers = true;
			}
			else if ((GestureModifiers != 0) && (GestureModifiersEx == 0))
			{
				SetNewModifiers();
			}
			else if ((GestureModifiers == 0) && (GestureModifiersEx != 0))
			{
				SetOldModifiers();
			}
			else
			{
				InvalidModifiers = true;
			}
		}

		/// <summary>
		/// Constructs a <code>DragSourceDragEvent</code> given the specified
		/// <code>DragSourceContext</code>, user drop action, target drop action,
		/// modifiers and coordinates.
		/// <para>
		/// The arguments <code>dropAction</code> and <code>action</code> should
		/// be one of <code>DnDConstants</code> that represents a single action.
		/// The argument <code>modifiers</code> should be either a bitwise mask
		/// of old <code>java.awt.event.InputEvent.*_MASK</code> constants or a
		/// bitwise mask of extended <code>java.awt.event.InputEvent.*_DOWN_MASK</code>
		/// constants.
		/// This constructor does not throw any exception for invalid <code>dropAction</code>,
		/// <code>action</code> and <code>modifiers</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dsc"> the <code>DragSourceContext</code> associated with this
		///        event. </param>
		/// <param name="dropAction"> the user drop action. </param>
		/// <param name="action"> the target drop action. </param>
		/// <param name="modifiers"> the modifier keys down during event (shift, ctrl,
		///        alt, meta)
		///        Either extended _DOWN_MASK or old _MASK modifiers
		///        should be used, but both models should not be mixed
		///        in one event. Use of the extended modifiers is
		///        preferred. </param>
		/// <param name="x">   the horizontal coordinate for the cursor location </param>
		/// <param name="y">   the vertical coordinate for the cursor location
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		/// </exception>
		/// <seealso cref= java.awt.event.InputEvent
		/// @since 1.4 </seealso>
		public DragSourceDragEvent(DragSourceContext dsc, int dropAction, int action, int modifiers, int x, int y) : base(dsc, x, y)
		{

			TargetActions_Renamed = action;
			GestureModifiers_Renamed = modifiers;
			this.DropAction_Renamed = dropAction;
			if ((modifiers & ~(JDK_1_3_MODIFIERS | JDK_1_4_MODIFIERS)) != 0)
			{
				InvalidModifiers = true;
			}
			else if ((GestureModifiers != 0) && (GestureModifiersEx == 0))
			{
				SetNewModifiers();
			}
			else if ((GestureModifiers == 0) && (GestureModifiersEx != 0))
			{
				SetOldModifiers();
			}
			else
			{
				InvalidModifiers = true;
			}
		}

		/// <summary>
		/// This method returns the target drop action.
		/// </summary>
		/// <returns> the target drop action. </returns>
		public virtual int TargetActions
		{
			get
			{
				return TargetActions_Renamed;
			}
		}


		private static readonly int JDK_1_3_MODIFIERS = InputEvent.SHIFT_DOWN_MASK - 1;
		private static readonly int JDK_1_4_MODIFIERS = ((InputEvent.ALT_GRAPH_DOWN_MASK << 1) - 1) & ~JDK_1_3_MODIFIERS;

		/// <summary>
		/// This method returns an <code>int</code> representing
		/// the current state of the input device modifiers
		/// associated with the user's gesture. Typically these
		/// would be mouse buttons or keyboard modifiers.
		/// <P>
		/// If the <code>modifiers</code> passed to the constructor
		/// are invalid, this method returns them unchanged.
		/// </summary>
		/// <returns> the current state of the input device modifiers </returns>

		public virtual int GestureModifiers
		{
			get
			{
				return InvalidModifiers ? GestureModifiers_Renamed : GestureModifiers_Renamed & JDK_1_3_MODIFIERS;
			}
		}

		/// <summary>
		/// This method returns an <code>int</code> representing
		/// the current state of the input device extended modifiers
		/// associated with the user's gesture.
		/// See <seealso cref="InputEvent#getModifiersEx"/>
		/// <P>
		/// If the <code>modifiers</code> passed to the constructor
		/// are invalid, this method returns them unchanged.
		/// </summary>
		/// <returns> the current state of the input device extended modifiers
		/// @since 1.4 </returns>

		public virtual int GestureModifiersEx
		{
			get
			{
				return InvalidModifiers ? GestureModifiers_Renamed : GestureModifiers_Renamed & JDK_1_4_MODIFIERS;
			}
		}

		/// <summary>
		/// This method returns the user drop action.
		/// </summary>
		/// <returns> the user drop action. </returns>
		public virtual int UserAction
		{
			get
			{
				return DropAction_Renamed;
			}
		}

		/// <summary>
		/// This method returns the logical intersection of
		/// the target drop action and the set of drop actions supported by
		/// the drag source.
		/// </summary>
		/// <returns> the logical intersection of the target drop action and
		///         the set of drop actions supported by the drag source. </returns>
		public virtual int DropAction
		{
			get
			{
				return TargetActions_Renamed & DragSourceContext.SourceActions;
			}
		}

		/*
		 * fields
		 */

		/// <summary>
		/// The target drop action.
		/// 
		/// @serial
		/// </summary>
		private int TargetActions_Renamed = DnDConstants.ACTION_NONE;

		/// <summary>
		/// The user drop action.
		/// 
		/// @serial
		/// </summary>
		private int DropAction_Renamed = DnDConstants.ACTION_NONE;

		/// <summary>
		/// The state of the input device modifiers associated with the user
		/// gesture.
		/// 
		/// @serial
		/// </summary>
		private int GestureModifiers_Renamed = 0;

		/// <summary>
		/// Indicates whether the <code>gestureModifiers</code> are invalid.
		/// 
		/// @serial
		/// </summary>
		private bool InvalidModifiers;

		/// <summary>
		/// Sets new modifiers by the old ones.
		/// The mouse modifiers have higher priority than overlaying key
		/// modifiers.
		/// </summary>
		private void SetNewModifiers()
		{
			if ((GestureModifiers_Renamed & InputEvent.BUTTON1_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.BUTTON1_DOWN_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.BUTTON2_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.BUTTON2_DOWN_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.BUTTON3_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.BUTTON3_DOWN_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.SHIFT_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.SHIFT_DOWN_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.CTRL_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.CTRL_DOWN_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.ALT_GRAPH_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.ALT_GRAPH_DOWN_MASK;
			}
		}

		/// <summary>
		/// Sets old modifiers by the new ones.
		/// </summary>
		private void SetOldModifiers()
		{
			if ((GestureModifiers_Renamed & InputEvent.BUTTON1_DOWN_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.BUTTON1_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.BUTTON2_DOWN_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.BUTTON2_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.BUTTON3_DOWN_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.BUTTON3_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.SHIFT_DOWN_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.SHIFT_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.CTRL_DOWN_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.CTRL_MASK;
			}
			if ((GestureModifiers_Renamed & InputEvent.ALT_GRAPH_DOWN_MASK) != 0)
			{
				GestureModifiers_Renamed |= InputEvent.ALT_GRAPH_MASK;
			}
		}
	}

}