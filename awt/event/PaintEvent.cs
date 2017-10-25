/*
 * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// The component-level paint event.
	/// This event is a special type which is used to ensure that
	/// paint/update method calls are serialized along with the other
	/// events delivered from the event queue.  This event is not
	/// designed to be used with the Event Listener model; programs
	/// should continue to override paint/update methods in order
	/// render themselves properly.
	/// <para>
	/// An unspecified behavior will be caused if the {@code id} parameter
	/// of any particular {@code PaintEvent} instance is not
	/// in the range from {@code PAINT_FIRST} to {@code PAINT_LAST}.
	/// 
	/// @author Amy Fowler
	/// @since 1.1
	/// </para>
	/// </summary>
	public class PaintEvent : ComponentEvent
	{

		/// <summary>
		/// Marks the first integer id for the range of paint event ids.
		/// </summary>
		public const int PAINT_FIRST = 800;

		/// <summary>
		/// Marks the last integer id for the range of paint event ids.
		/// </summary>
		public const int PAINT_LAST = 801;

		/// <summary>
		/// The paint event type.
		/// </summary>
		public const int PAINT = PAINT_FIRST;

		/// <summary>
		/// The update event type.
		/// </summary>
		public static readonly int UPDATE = PAINT_FIRST + 1; //801

		/// <summary>
		/// This is the rectangle that represents the area on the source
		/// component that requires a repaint.
		/// This rectangle should be non null.
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.Rectangle </seealso>
		/// <seealso cref= #setUpdateRect(Rectangle) </seealso>
		/// <seealso cref= #getUpdateRect() </seealso>
		internal Rectangle UpdateRect_Renamed;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 1267492026433337593L;

		/// <summary>
		/// Constructs a <code>PaintEvent</code> object with the specified
		/// source component and type.
		/// <para> This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source">     The object where the event originated </param>
		/// <param name="id">           The integer that identifies the event type.
		///                     For information on allowable values, see
		///                     the class description for <seealso cref="PaintEvent"/> </param>
		/// <param name="updateRect"> The rectangle area which needs to be repainted </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref= #getID() </seealso>
		/// <seealso cref= #getUpdateRect() </seealso>
		public PaintEvent(Component source, int id, Rectangle updateRect) : base(source, id)
		{
			this.UpdateRect_Renamed = updateRect;
		}

		/// <summary>
		/// Returns the rectangle representing the area which needs to be
		/// repainted in response to this event.
		/// </summary>
		public virtual Rectangle UpdateRect
		{
			get
			{
				return UpdateRect_Renamed;
			}
			set
			{
				this.UpdateRect_Renamed = value;
			}
		}


		public override String ParamString()
		{
			String typeStr;
			switch (Id)
			{
			  case PAINT:
				  typeStr = "PAINT";
				  break;
			  case UPDATE:
				  typeStr = "UPDATE";
				  break;
			  default:
				  typeStr = "unknown type";
			  break;
			}
			return typeStr + ",updateRect=" + (UpdateRect_Renamed != null ? UpdateRect_Renamed.ToString() : "null");
		}
	}

}