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
	/// The <code>DragSourceDropEvent</code> is delivered
	/// from the <code>DragSourceContextPeer</code>,
	/// via the <code>DragSourceContext</code>, to the <code>dragDropEnd</code>
	/// method of <code>DragSourceListener</code>s registered with that
	/// <code>DragSourceContext</code> and with its associated
	/// <code>DragSource</code>.
	/// It contains sufficient information for the
	/// originator of the operation
	/// to provide appropriate feedback to the end user
	/// when the operation completes.
	/// <P>
	/// @since 1.2
	/// </summary>

	public class DragSourceDropEvent : DragSourceEvent
	{

		private const long SerialVersionUID = -5571321229470821891L;

		/// <summary>
		/// Construct a <code>DragSourceDropEvent</code> for a drop,
		/// given the
		/// <code>DragSourceContext</code>, the drop action,
		/// and a <code>boolean</code> indicating if the drop was successful.
		/// The coordinates for this <code>DragSourceDropEvent</code>
		/// are not specified, so <code>getLocation</code> will return
		/// <code>null</code> for this event.
		/// <para>
		/// The argument <code>action</code> should be one of <code>DnDConstants</code>
		/// that represents a single action.
		/// This constructor does not throw any exception for invalid <code>action</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dsc"> the <code>DragSourceContext</code>
		/// associated with this <code>DragSourceDropEvent</code> </param>
		/// <param name="action"> the drop action </param>
		/// <param name="success"> a boolean indicating if the drop was successful
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		/// </exception>
		/// <seealso cref= DragSourceEvent#getLocation </seealso>

		public DragSourceDropEvent(DragSourceContext dsc, int action, bool success) : base(dsc)
		{

			DropSuccess_Renamed = success;
			DropAction_Renamed = action;
		}

		/// <summary>
		/// Construct a <code>DragSourceDropEvent</code> for a drop, given the
		/// <code>DragSourceContext</code>, the drop action, a <code>boolean</code>
		/// indicating if the drop was successful, and coordinates.
		/// <para>
		/// The argument <code>action</code> should be one of <code>DnDConstants</code>
		/// that represents a single action.
		/// This constructor does not throw any exception for invalid <code>action</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dsc"> the <code>DragSourceContext</code>
		/// associated with this <code>DragSourceDropEvent</code> </param>
		/// <param name="action"> the drop action </param>
		/// <param name="success"> a boolean indicating if the drop was successful </param>
		/// <param name="x">   the horizontal coordinate for the cursor location </param>
		/// <param name="y">   the vertical coordinate for the cursor location
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		/// 
		/// @since 1.4 </exception>
		public DragSourceDropEvent(DragSourceContext dsc, int action, bool success, int x, int y) : base(dsc, x, y)
		{

			DropSuccess_Renamed = success;
			DropAction_Renamed = action;
		}

		/// <summary>
		/// Construct a <code>DragSourceDropEvent</code>
		/// for a drag that does not result in a drop.
		/// The coordinates for this <code>DragSourceDropEvent</code>
		/// are not specified, so <code>getLocation</code> will return
		/// <code>null</code> for this event.
		/// </summary>
		/// <param name="dsc"> the <code>DragSourceContext</code>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		/// </exception>
		/// <seealso cref= DragSourceEvent#getLocation </seealso>

		public DragSourceDropEvent(DragSourceContext dsc) : base(dsc)
		{

			DropSuccess_Renamed = false;
		}

		/// <summary>
		/// This method returns a <code>boolean</code> indicating
		/// if the drop was successful.
		/// </summary>
		/// <returns> <code>true</code> if the drop target accepted the drop and
		///         successfully performed a drop action;
		///         <code>false</code> if the drop target rejected the drop or
		///         if the drop target accepted the drop, but failed to perform
		///         a drop action. </returns>

		public virtual bool DropSuccess
		{
			get
			{
				return DropSuccess_Renamed;
			}
		}

		/// <summary>
		/// This method returns an <code>int</code> representing
		/// the action performed by the target on the subject of the drop.
		/// </summary>
		/// <returns> the action performed by the target on the subject of the drop
		///         if the drop target accepted the drop and the target drop action
		///         is supported by the drag source; otherwise,
		///         <code>DnDConstants.ACTION_NONE</code>. </returns>

		public virtual int DropAction
		{
			get
			{
				return DropAction_Renamed;
			}
		}

		/*
		 * fields
		 */

		/// <summary>
		/// <code>true</code> if the drop was successful.
		/// 
		/// @serial
		/// </summary>
		private bool DropSuccess_Renamed;

		/// <summary>
		/// The drop action.
		/// 
		/// @serial
		/// </summary>
		private int DropAction_Renamed = DnDConstants.ACTION_NONE;
	}

}