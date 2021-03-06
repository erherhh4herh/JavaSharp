﻿/*
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
	/// A semantic event which indicates that an object's text changed.
	/// This high-level event is generated by an object (such as a TextComponent)
	/// when its text changes. The event is passed to
	/// every <code>TextListener</code> object which registered to receive such
	/// events using the component's <code>addTextListener</code> method.
	/// <P>
	/// The object that implements the <code>TextListener</code> interface gets
	/// this <code>TextEvent</code> when the event occurs. The listener is
	/// spared the details of processing individual mouse movements and key strokes
	/// Instead, it can process a "meaningful" (semantic) event like "text changed".
	/// <para>
	/// An unspecified behavior will be caused if the {@code id} parameter
	/// of any particular {@code TextEvent} instance is not
	/// in the range from {@code TEXT_FIRST} to {@code TEXT_LAST}.
	/// 
	/// @author Georges Saab
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.TextComponent </seealso>
	/// <seealso cref= TextListener
	/// 
	/// @since 1.1 </seealso>

	public class TextEvent : AWTEvent
	{

		/// <summary>
		/// The first number in the range of ids used for text events.
		/// </summary>
		public const int TEXT_FIRST = 900;

		/// <summary>
		/// The last number in the range of ids used for text events.
		/// </summary>
		public const int TEXT_LAST = 900;

		/// <summary>
		/// This event id indicates that object's text changed.
		/// </summary>
		public const int TEXT_VALUE_CHANGED = TEXT_FIRST;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 6269902291250941179L;

		/// <summary>
		/// Constructs a <code>TextEvent</code> object.
		/// <para> This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> The (<code>TextComponent</code>) object that
		///               originated the event </param>
		/// <param name="id">     An integer that identifies the event type.
		///                     For information on allowable values, see
		///                     the class description for <seealso cref="TextEvent"/> </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		/// <seealso cref= #getSource() </seealso>
		/// <seealso cref= #getID() </seealso>
		public TextEvent(Object source, int id) : base(source, id)
		{
		}


		/// <summary>
		/// Returns a parameter string identifying this text event.
		/// This method is useful for event-logging and for debugging.
		/// </summary>
		/// <returns> a string identifying the event and its attributes </returns>
		public override String ParamString()
		{
			String typeStr;
			switch (Id)
			{
			  case TEXT_VALUE_CHANGED:
				  typeStr = "TEXT_VALUE_CHANGED";
				  break;
			  default:
				  typeStr = "unknown type";
			  break;
			}
			return typeStr;
		}
	}

}