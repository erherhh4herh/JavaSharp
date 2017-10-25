﻿/*
 * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// An event which indicates a change to the <code>Component</code>
	/// hierarchy to which <code>Component</code> belongs.
	/// <ul>
	/// <li>Hierarchy Change Events (HierarchyListener)
	///     <ul>
	///     <li> addition of an ancestor
	///     <li> removal of an ancestor
	///     <li> hierarchy made displayable
	///     <li> hierarchy made undisplayable
	///     <li> hierarchy shown on the screen (both visible and displayable)
	///     <li> hierarchy hidden on the screen (either invisible or undisplayable)
	///     </ul>
	/// <li>Ancestor Reshape Events (HierarchyBoundsListener)
	///     <ul>
	///     <li> an ancestor was resized
	///     <li> an ancestor was moved
	///     </ul>
	/// </ul>
	/// <para>
	/// Hierarchy events are provided for notification purposes ONLY.
	/// The AWT will automatically handle changes to the hierarchy internally so
	/// that GUI layout and displayability works properly regardless of whether a
	/// program is receiving these events or not.
	/// </para>
	/// <para>
	/// This event is generated by a Container object (such as a Panel) when the
	/// Container is added, removed, moved, or resized, and passed down the
	/// hierarchy. It is also generated by a Component object when that object's
	/// <code>addNotify</code>, <code>removeNotify</code>, <code>show</code>, or
	/// <code>hide</code> method is called. The {@code ANCESTOR_MOVED} and
	/// {@code ANCESTOR_RESIZED}
	/// events are dispatched to every <code>HierarchyBoundsListener</code> or
	/// <code>HierarchyBoundsAdapter</code> object which registered to receive
	/// such events using the Component's <code>addHierarchyBoundsListener</code>
	/// method. (<code>HierarchyBoundsAdapter</code> objects implement the <code>
	/// HierarchyBoundsListener</code> interface.) The {@code HIERARCHY_CHANGED} events are
	/// dispatched to every <code>HierarchyListener</code> object which registered
	/// to receive such events using the Component's <code>addHierarchyListener
	/// </code> method. Each such listener object gets this <code>HierarchyEvent
	/// </code> when the event occurs.
	/// </para>
	/// <para>
	/// An unspecified behavior will be caused if the {@code id} parameter
	/// of any particular {@code HierarchyEvent} instance is not
	/// in the range from {@code HIERARCHY_FIRST} to {@code HIERARCHY_LAST}.
	/// <br>
	/// The {@code changeFlags} parameter of any {@code HierarchyEvent} instance takes one of the following
	/// values:
	/// <ul>
	/// <li> {@code HierarchyEvent.PARENT_CHANGED}
	/// <li> {@code HierarchyEvent.DISPLAYABILITY_CHANGED}
	/// <li> {@code HierarchyEvent.SHOWING_CHANGED}
	/// </ul>
	/// Assigning the value different from listed above will cause unspecified behavior.
	/// 
	/// @author      David Mendenhall
	/// </para>
	/// </summary>
	/// <seealso cref=         HierarchyListener </seealso>
	/// <seealso cref=         HierarchyBoundsAdapter </seealso>
	/// <seealso cref=         HierarchyBoundsListener
	/// @since       1.3 </seealso>
	public class HierarchyEvent : AWTEvent
	{
		/*
		 * serialVersionUID
		 */
		private const long SerialVersionUID = -5337576970038043990L;

		/// <summary>
		/// Marks the first integer id for the range of hierarchy event ids.
		/// </summary>
		public const int HIERARCHY_FIRST = 1400; // 1300 used by sun.awt.windows.ModalityEvent

		/// <summary>
		/// The event id indicating that modification was made to the
		/// entire hierarchy tree.
		/// </summary>
		public const int HIERARCHY_CHANGED = HIERARCHY_FIRST;

		/// <summary>
		/// The event id indicating an ancestor-Container was moved.
		/// </summary>
		public static readonly int ANCESTOR_MOVED = 1 + HIERARCHY_FIRST;

		/// <summary>
		/// The event id indicating an ancestor-Container was resized.
		/// </summary>
		public static readonly int ANCESTOR_RESIZED = 2 + HIERARCHY_FIRST;

		/// <summary>
		/// Marks the last integer id for the range of ancestor event ids.
		/// </summary>
		public static readonly int HIERARCHY_LAST = ANCESTOR_RESIZED;

		/// <summary>
		/// A change flag indicates that the <code>HIERARCHY_CHANGED</code> event
		/// was generated by a reparenting operation.
		/// </summary>
		public const int PARENT_CHANGED = 0x1;

		/// <summary>
		/// A change flag indicates that the <code>HIERARCHY_CHANGED</code> event
		/// was generated due to the changing of the hierarchy displayability.
		/// To discern the
		/// current displayability of the hierarchy, call the
		/// <code>Component.isDisplayable</code> method. Displayability changes occur
		/// in response to explicit or implicit calls of the
		/// <code>Component.addNotify</code> and
		/// <code>Component.removeNotify</code> methods.
		/// </summary>
		/// <seealso cref= java.awt.Component#isDisplayable() </seealso>
		/// <seealso cref= java.awt.Component#addNotify() </seealso>
		/// <seealso cref= java.awt.Component#removeNotify() </seealso>
		public const int DISPLAYABILITY_CHANGED = 0x2;

		/// <summary>
		/// A change flag indicates that the <code>HIERARCHY_CHANGED</code> event
		/// was generated due to the changing of the hierarchy showing state.
		/// To discern the
		/// current showing state of the hierarchy, call the
		/// <code>Component.isShowing</code> method. Showing state changes occur
		/// when either the displayability or visibility of the
		/// hierarchy occurs. Visibility changes occur in response to explicit
		/// or implicit calls of the <code>Component.show</code> and
		/// <code>Component.hide</code> methods.
		/// </summary>
		/// <seealso cref= java.awt.Component#isShowing() </seealso>
		/// <seealso cref= java.awt.Component#addNotify() </seealso>
		/// <seealso cref= java.awt.Component#removeNotify() </seealso>
		/// <seealso cref= java.awt.Component#show() </seealso>
		/// <seealso cref= java.awt.Component#hide() </seealso>
		public const int SHOWING_CHANGED = 0x4;

		internal Component Changed_Renamed;
		internal Container ChangedParent_Renamed;
		internal long ChangeFlags_Renamed;

		/// <summary>
		/// Constructs an <code>HierarchyEvent</code> object to identify a
		/// change in the <code>Component</code> hierarchy.
		/// <para>This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">          The <code>Component</code> object that
		///                        originated the event </param>
		/// <param name="id">              An integer indicating the type of event.
		///                        For information on allowable values, see
		///                        the class description for <seealso cref="HierarchyEvent"/> </param>
		/// <param name="changed">         The <code>Component</code> at the top of
		///                        the hierarchy which was changed </param>
		/// <param name="changedParent">   The parent of the <code>changed</code> component.
		///                        This
		///                        may be the parent before or after the
		///                        change, depending on the type of change </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is {@code null} </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref= #getID() </seealso>
		/// <seealso cref= #getChanged() </seealso>
		/// <seealso cref= #getChangedParent() </seealso>
		public HierarchyEvent(Component source, int id, Component changed, Container changedParent) : base(source, id)
		{
			this.Changed_Renamed = changed;
			this.ChangedParent_Renamed = changedParent;
		}

		/// <summary>
		/// Constructs an <code>HierarchyEvent</code> object to identify
		/// a change in the <code>Component</code> hierarchy.
		/// <para> This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">          The <code>Component</code> object that
		///                        originated the event </param>
		/// <param name="id">              An integer indicating the type of event.
		///                        For information on allowable values, see
		///                        the class description for <seealso cref="HierarchyEvent"/> </param>
		/// <param name="changed">         The <code>Component</code> at the top
		///                        of the hierarchy which was changed </param>
		/// <param name="changedParent">   The parent of the <code>changed</code> component.
		///                        This
		///                        may be the parent before or after the
		///                        change, depending on the type of change </param>
		/// <param name="changeFlags">     A bitmask which indicates the type(s) of
		///                        the <code>HIERARCHY_CHANGED</code> events
		///                        represented in this event object.
		///                        For information on allowable values, see
		///                        the class description for <seealso cref="HierarchyEvent"/> </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref= #getID() </seealso>
		/// <seealso cref= #getChanged() </seealso>
		/// <seealso cref= #getChangedParent() </seealso>
		/// <seealso cref= #getChangeFlags() </seealso>
		public HierarchyEvent(Component source, int id, Component changed, Container changedParent, long changeFlags) : base(source, id)
		{
			this.Changed_Renamed = changed;
			this.ChangedParent_Renamed = changedParent;
			this.ChangeFlags_Renamed = changeFlags;
		}

		/// <summary>
		/// Returns the originator of the event.
		/// </summary>
		/// <returns> the <code>Component</code> object that originated
		/// the event, or <code>null</code> if the object is not a
		/// <code>Component</code>. </returns>
		public virtual Component Component
		{
			get
			{
				return (Source_Renamed is Component) ? (Component)Source_Renamed : null;
			}
		}

		/// <summary>
		/// Returns the Component at the top of the hierarchy which was
		/// changed.
		/// </summary>
		/// <returns> the changed Component </returns>
		public virtual Component Changed
		{
			get
			{
				return Changed_Renamed;
			}
		}

		/// <summary>
		/// Returns the parent of the Component returned by <code>
		/// getChanged()</code>. For a HIERARCHY_CHANGED event where the
		/// change was of type PARENT_CHANGED via a call to <code>
		/// Container.add</code>, the parent returned is the parent
		/// after the add operation. For a HIERARCHY_CHANGED event where
		/// the change was of type PARENT_CHANGED via a call to <code>
		/// Container.remove</code>, the parent returned is the parent
		/// before the remove operation. For all other events and types,
		/// the parent returned is the parent during the operation.
		/// </summary>
		/// <returns> the parent of the changed Component </returns>
		public virtual Container ChangedParent
		{
			get
			{
				return ChangedParent_Renamed;
			}
		}

		/// <summary>
		/// Returns a bitmask which indicates the type(s) of
		/// HIERARCHY_CHANGED events represented in this event object.
		/// The bits have been bitwise-ored together.
		/// </summary>
		/// <returns> the bitmask, or 0 if this is not an HIERARCHY_CHANGED
		/// event </returns>
		public virtual long ChangeFlags
		{
			get
			{
				return ChangeFlags_Renamed;
			}
		}

		/// <summary>
		/// Returns a parameter string identifying this event.
		/// This method is useful for event-logging and for debugging.
		/// </summary>
		/// <returns> a string identifying the event and its attributes </returns>
		public override String ParamString()
		{
			String typeStr;
			switch (Id)
			{
			  case ANCESTOR_MOVED:
				  typeStr = "ANCESTOR_MOVED (" + Changed_Renamed + "," + ChangedParent_Renamed + ")";
				  break;
			  case ANCESTOR_RESIZED:
				  typeStr = "ANCESTOR_RESIZED (" + Changed_Renamed + "," + ChangedParent_Renamed + ")";
				  break;
			  case HIERARCHY_CHANGED:
			  {
				  typeStr = "HIERARCHY_CHANGED (";
				  bool first = true;
				  if ((ChangeFlags_Renamed & PARENT_CHANGED) != 0)
				  {
					  first = false;
					  typeStr += "PARENT_CHANGED";
				  }
				  if ((ChangeFlags_Renamed & DISPLAYABILITY_CHANGED) != 0)
				  {
					  if (first)
					  {
						  first = false;
					  }
					  else
					  {
						  typeStr += ",";
					  }
					  typeStr += "DISPLAYABILITY_CHANGED";
				  }
				  if ((ChangeFlags_Renamed & SHOWING_CHANGED) != 0)
				  {
					  if (first)
					  {
						  first = false;
					  }
					  else
					  {
						  typeStr += ",";
					  }
					  typeStr += "SHOWING_CHANGED";
				  }
				  if (!first)
				  {
					  typeStr += ",";
				  }
				  typeStr += Changed_Renamed + "," + ChangedParent_Renamed + ")";
				  break;
			  }
			  default:
				  typeStr = "unknown type";
			  break;
			}
			return typeStr;
		}
	}

}