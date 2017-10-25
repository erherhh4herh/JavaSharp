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
	/// This class is the base class for
	/// <code>DragSourceDragEvent</code> and
	/// <code>DragSourceDropEvent</code>.
	/// <para>
	/// <code>DragSourceEvent</code>s are generated whenever the drag enters, moves
	/// over, or exits a drop site, when the drop action changes, and when the drag
	/// ends. The location for the generated <code>DragSourceEvent</code> specifies
	/// the mouse cursor location in screen coordinates at the moment this event
	/// occurred.
	/// </para>
	/// <para>
	/// In a multi-screen environment without a virtual device, the cursor location is
	/// specified in the coordinate system of the <i>initiator</i>
	/// <code>GraphicsConfiguration</code>. The <i>initiator</i>
	/// <code>GraphicsConfiguration</code> is the <code>GraphicsConfiguration</code>
	/// of the <code>Component</code> on which the drag gesture for the current drag
	/// operation was recognized. If the cursor location is outside the bounds of
	/// the initiator <code>GraphicsConfiguration</code>, the reported coordinates are
	/// clipped to fit within the bounds of that <code>GraphicsConfiguration</code>.
	/// </para>
	/// <para>
	/// In a multi-screen environment with a virtual device, the location is specified
	/// in the corresponding virtual coordinate system. If the cursor location is
	/// outside the bounds of the virtual device the reported coordinates are
	/// clipped to fit within the bounds of the virtual device.
	/// 
	/// @since 1.2
	/// </para>
	/// </summary>

	public class DragSourceEvent : EventObject
	{

		private const long SerialVersionUID = -763287114604032641L;

		/// <summary>
		/// The <code>boolean</code> indicating whether the cursor location
		/// is specified for this event.
		/// 
		/// @serial
		/// </summary>
		private readonly bool LocationSpecified;

		/// <summary>
		/// The horizontal coordinate for the cursor location at the moment this
		/// event occurred if the cursor location is specified for this event;
		/// otherwise zero.
		/// 
		/// @serial
		/// </summary>
		private readonly int x;

		/// <summary>
		/// The vertical coordinate for the cursor location at the moment this event
		/// occurred if the cursor location is specified for this event;
		/// otherwise zero.
		/// 
		/// @serial
		/// </summary>
		private readonly int y;

		/// <summary>
		/// Construct a <code>DragSourceEvent</code>
		/// given a specified <code>DragSourceContext</code>.
		/// The coordinates for this <code>DragSourceEvent</code>
		/// are not specified, so <code>getLocation</code> will return
		/// <code>null</code> for this event.
		/// </summary>
		/// <param name="dsc"> the <code>DragSourceContext</code>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		/// </exception>
		/// <seealso cref= #getLocation </seealso>

		public DragSourceEvent(DragSourceContext dsc) : base(dsc)
		{
			LocationSpecified = false;
			this.x = 0;
			this.y = 0;
		}

		/// <summary>
		/// Construct a <code>DragSourceEvent</code> given a specified
		/// <code>DragSourceContext</code>, and coordinates of the cursor
		/// location.
		/// </summary>
		/// <param name="dsc"> the <code>DragSourceContext</code> </param>
		/// <param name="x">   the horizontal coordinate for the cursor location </param>
		/// <param name="y">   the vertical coordinate for the cursor location
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		/// 
		/// @since 1.4 </exception>
		public DragSourceEvent(DragSourceContext dsc, int x, int y) : base(dsc)
		{
			LocationSpecified = true;
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// This method returns the <code>DragSourceContext</code> that
		/// originated the event.
		/// <P> </summary>
		/// <returns> the <code>DragSourceContext</code> that originated the event </returns>

		public virtual DragSourceContext DragSourceContext
		{
			get
			{
				return (DragSourceContext)Source;
			}
		}

		/// <summary>
		/// This method returns a <code>Point</code> indicating the cursor
		/// location in screen coordinates at the moment this event occurred, or
		/// <code>null</code> if the cursor location is not specified for this
		/// event.
		/// </summary>
		/// <returns> the <code>Point</code> indicating the cursor location
		///         or <code>null</code> if the cursor location is not specified
		/// @since 1.4 </returns>
		public virtual Point Location
		{
			get
			{
				if (LocationSpecified)
				{
					return new Point(x, y);
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// This method returns the horizontal coordinate of the cursor location in
		/// screen coordinates at the moment this event occurred, or zero if the
		/// cursor location is not specified for this event.
		/// </summary>
		/// <returns> an integer indicating the horizontal coordinate of the cursor
		///         location or zero if the cursor location is not specified
		/// @since 1.4 </returns>
		public virtual int X
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// This method returns the vertical coordinate of the cursor location in
		/// screen coordinates at the moment this event occurred, or zero if the
		/// cursor location is not specified for this event.
		/// </summary>
		/// <returns> an integer indicating the vertical coordinate of the cursor
		///         location or zero if the cursor location is not specified
		/// @since 1.4 </returns>
		public virtual int Y
		{
			get
			{
				return y;
			}
		}
	}

}