/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The adjustment event emitted by Adjustable objects like
	/// <seealso cref="java.awt.Scrollbar"/> and <seealso cref="java.awt.ScrollPane"/>.
	/// When the user changes the value of the scrolling component,
	/// it receives an instance of {@code AdjustmentEvent}.
	/// <para>
	/// An unspecified behavior will be caused if the {@code id} parameter
	/// of any particular {@code AdjustmentEvent} instance is not
	/// in the range from {@code ADJUSTMENT_FIRST} to {@code ADJUSTMENT_LAST}.
	/// </para>
	/// <para>
	/// The {@code type} of any {@code AdjustmentEvent} instance takes one of the following
	/// values:
	///                     <ul>
	///                     <li> {@code UNIT_INCREMENT}
	///                     <li> {@code UNIT_DECREMENT}
	///                     <li> {@code BLOCK_INCREMENT}
	///                     <li> {@code BLOCK_DECREMENT}
	///                     <li> {@code TRACK}
	///                     </ul>
	/// Assigning the value different from listed above will cause an unspecified behavior.
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.Adjustable </seealso>
	/// <seealso cref= AdjustmentListener
	/// 
	/// @author Amy Fowler
	/// @since 1.1 </seealso>
	public class AdjustmentEvent : AWTEvent
	{

		/// <summary>
		/// Marks the first integer id for the range of adjustment event ids.
		/// </summary>
		public const int ADJUSTMENT_FIRST = 601;

		/// <summary>
		/// Marks the last integer id for the range of adjustment event ids.
		/// </summary>
		public const int ADJUSTMENT_LAST = 601;

		/// <summary>
		/// The adjustment value changed event.
		/// </summary>
		public const int ADJUSTMENT_VALUE_CHANGED = ADJUSTMENT_FIRST; //Event.SCROLL_LINE_UP

		/// <summary>
		/// The unit increment adjustment type.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int UNIT_INCREMENT = 1;
		public const int UNIT_INCREMENT = 1;

		/// <summary>
		/// The unit decrement adjustment type.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int UNIT_DECREMENT = 2;
		public const int UNIT_DECREMENT = 2;

		/// <summary>
		/// The block decrement adjustment type.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int BLOCK_DECREMENT = 3;
		public const int BLOCK_DECREMENT = 3;

		/// <summary>
		/// The block increment adjustment type.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int BLOCK_INCREMENT = 4;
		public const int BLOCK_INCREMENT = 4;

		/// <summary>
		/// The absolute tracking adjustment type.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int TRACK = 5;
		public const int TRACK = 5;

		/// <summary>
		/// The adjustable object that fired the event.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getAdjustable </seealso>
		internal Adjustable Adjustable_Renamed;

		/// <summary>
		/// <code>value</code> will contain the new value of the
		/// adjustable object.  This value will always be  in a
		/// range associated adjustable object.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getValue </seealso>
		internal int Value_Renamed;

		/// <summary>
		/// The <code>adjustmentType</code> describes how the adjustable
		/// object value has changed.
		/// This value can be increased/decreased by a block or unit amount
		/// where the block is associated with page increments/decrements,
		/// and a unit is associated with line increments/decrements.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getAdjustmentType </seealso>
		internal int AdjustmentType_Renamed;


		/// <summary>
		/// The <code>isAdjusting</code> is true if the event is one
		/// of the series of multiple adjustment events.
		/// 
		/// @since 1.4
		/// @serial </summary>
		/// <seealso cref= #getValueIsAdjusting </seealso>
		internal bool IsAdjusting;


		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = 5700290645205279921L;


		/// <summary>
		/// Constructs an <code>AdjustmentEvent</code> object with the
		/// specified <code>Adjustable</code> source, event type,
		/// adjustment type, and value.
		/// <para> This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> The <code>Adjustable</code> object where the
		///               event originated </param>
		/// <param name="id">     An integer indicating the type of event.
		///                     For information on allowable values, see
		///                     the class description for <seealso cref="AdjustmentEvent"/> </param>
		/// <param name="type">   An integer indicating the adjustment type.
		///                     For information on allowable values, see
		///                     the class description for <seealso cref="AdjustmentEvent"/> </param>
		/// <param name="value">  The current value of the adjustment </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref= #getID() </seealso>
		/// <seealso cref= #getAdjustmentType() </seealso>
		/// <seealso cref= #getValue() </seealso>
		public AdjustmentEvent(Adjustable source, int id, int type, int value) : this(source, id, type, value, false)
		{
		}

		/// <summary>
		/// Constructs an <code>AdjustmentEvent</code> object with the
		/// specified Adjustable source, event type, adjustment type, and value.
		/// <para> This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> The <code>Adjustable</code> object where the
		///               event originated </param>
		/// <param name="id">     An integer indicating the type of event.
		///                     For information on allowable values, see
		///                     the class description for <seealso cref="AdjustmentEvent"/> </param>
		/// <param name="type">   An integer indicating the adjustment type.
		///                     For information on allowable values, see
		///                     the class description for <seealso cref="AdjustmentEvent"/> </param>
		/// <param name="value">  The current value of the adjustment </param>
		/// <param name="isAdjusting"> A boolean that equals <code>true</code> if the event is one
		///               of a series of multiple adjusting events,
		///               otherwise <code>false</code> </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null
		/// @since 1.4 </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref= #getID() </seealso>
		/// <seealso cref= #getAdjustmentType() </seealso>
		/// <seealso cref= #getValue() </seealso>
		/// <seealso cref= #getValueIsAdjusting() </seealso>
		public AdjustmentEvent(Adjustable source, int id, int type, int value, bool isAdjusting) : base(source, id)
		{
			Adjustable_Renamed = source;
			this.AdjustmentType_Renamed = type;
			this.Value_Renamed = value;
			this.IsAdjusting = isAdjusting;
		}

		/// <summary>
		/// Returns the <code>Adjustable</code> object where this event originated.
		/// </summary>
		/// <returns> the <code>Adjustable</code> object where this event originated </returns>
		public virtual Adjustable Adjustable
		{
			get
			{
				return Adjustable_Renamed;
			}
		}

		/// <summary>
		/// Returns the current value in the adjustment event.
		/// </summary>
		/// <returns> the current value in the adjustment event </returns>
		public virtual int Value
		{
			get
			{
				return Value_Renamed;
			}
		}

		/// <summary>
		/// Returns the type of adjustment which caused the value changed
		/// event.  It will have one of the following values:
		/// <ul>
		/// <li><seealso cref="#UNIT_INCREMENT"/>
		/// <li><seealso cref="#UNIT_DECREMENT"/>
		/// <li><seealso cref="#BLOCK_INCREMENT"/>
		/// <li><seealso cref="#BLOCK_DECREMENT"/>
		/// <li><seealso cref="#TRACK"/>
		/// </ul> </summary>
		/// <returns> one of the adjustment values listed above </returns>
		public virtual int AdjustmentType
		{
			get
			{
				return AdjustmentType_Renamed;
			}
		}

		/// <summary>
		/// Returns <code>true</code> if this is one of multiple
		/// adjustment events.
		/// </summary>
		/// <returns> <code>true</code> if this is one of multiple
		///         adjustment events, otherwise returns <code>false</code>
		/// @since 1.4 </returns>
		public virtual bool ValueIsAdjusting
		{
			get
			{
				return IsAdjusting;
			}
		}

		public override String ParamString()
		{
			String typeStr;
			switch (Id)
			{
			  case ADJUSTMENT_VALUE_CHANGED:
				  typeStr = "ADJUSTMENT_VALUE_CHANGED";
				  break;
			  default:
				  typeStr = "unknown type";
			  break;
			}
			String adjTypeStr;
			switch (AdjustmentType_Renamed)
			{
			  case UNIT_INCREMENT:
				  adjTypeStr = "UNIT_INCREMENT";
				  break;
			  case UNIT_DECREMENT:
				  adjTypeStr = "UNIT_DECREMENT";
				  break;
			  case BLOCK_INCREMENT:
				  adjTypeStr = "BLOCK_INCREMENT";
				  break;
			  case BLOCK_DECREMENT:
				  adjTypeStr = "BLOCK_DECREMENT";
				  break;
			  case TRACK:
				  adjTypeStr = "TRACK";
				  break;
			  default:
				  adjTypeStr = "unknown type";
			  break;
			}
			return typeStr + ",adjType=" + adjTypeStr + ",value=" + Value_Renamed + ",isAdjusting=" + IsAdjusting;
		}
	}

}