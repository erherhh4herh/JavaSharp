﻿/*
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
	/// A low-level event which indicates that a component moved, changed
	/// size, or changed visibility (also, the root class for the other
	/// component-level events).
	/// <P>
	/// Component events are provided for notification purposes ONLY;
	/// The AWT will automatically handle component moves and resizes
	/// internally so that GUI layout works properly regardless of
	/// whether a program is receiving these events or not.
	/// <P>
	/// In addition to serving as the base class for other component-related
	/// events (InputEvent, FocusEvent, WindowEvent, ContainerEvent),
	/// this class defines the events that indicate changes in
	/// a component's size, position, or visibility.
	/// <P>
	/// This low-level event is generated by a component object (such as a
	/// List) when the component is moved, resized, rendered invisible, or made
	/// visible again. The event is passed to every <code>ComponentListener</code>
	/// or <code>ComponentAdapter</code> object which registered to receive such
	/// events using the component's <code>addComponentListener</code> method.
	/// (<code>ComponentAdapter</code> objects implement the
	/// <code>ComponentListener</code> interface.) Each such listener object
	/// gets this <code>ComponentEvent</code> when the event occurs.
	/// <para>
	/// An unspecified behavior will be caused if the {@code id} parameter
	/// of any particular {@code ComponentEvent} instance is not
	/// in the range from {@code COMPONENT_FIRST} to {@code COMPONENT_LAST}.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ComponentAdapter </seealso>
	/// <seealso cref= ComponentListener </seealso>
	/// <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/componentlistener.html">Tutorial: Writing a Component Listener</a>
	/// 
	/// @author Carl Quinn
	/// @since 1.1 </seealso>
	public class ComponentEvent : AWTEvent
	{

		/// <summary>
		/// The first number in the range of ids used for component events.
		/// </summary>
		public const int COMPONENT_FIRST = 100;

		/// <summary>
		/// The last number in the range of ids used for component events.
		/// </summary>
		public const int COMPONENT_LAST = 103;

	   /// <summary>
	   /// This event indicates that the component's position changed.
	   /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int COMPONENT_MOVED = COMPONENT_FIRST;
		public const int COMPONENT_MOVED = COMPONENT_FIRST;

		/// <summary>
		/// This event indicates that the component's size changed.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int COMPONENT_RESIZED = 1 + COMPONENT_FIRST;
		public static readonly int COMPONENT_RESIZED = 1 + COMPONENT_FIRST;

		/// <summary>
		/// This event indicates that the component was made visible.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int COMPONENT_SHOWN = 2 + COMPONENT_FIRST;
		public static readonly int COMPONENT_SHOWN = 2 + COMPONENT_FIRST;

		/// <summary>
		/// This event indicates that the component was rendered invisible.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int COMPONENT_HIDDEN = 3 + COMPONENT_FIRST;
		public static readonly int COMPONENT_HIDDEN = 3 + COMPONENT_FIRST;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 8101406823902992965L;

		/// <summary>
		/// Constructs a <code>ComponentEvent</code> object.
		/// <para> This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> The <code>Component</code> that originated the event </param>
		/// <param name="id">     An integer indicating the type of event.
		///                     For information on allowable values, see
		///                     the class description for <seealso cref="ComponentEvent"/> </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		/// <seealso cref= #getComponent() </seealso>
		/// <seealso cref= #getID() </seealso>
		public ComponentEvent(Component source, int id) : base(source, id)
		{
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
		/// Returns a parameter string identifying this event.
		/// This method is useful for event-logging and for debugging.
		/// </summary>
		/// <returns> a string identifying the event and its attributes </returns>
		public override String ParamString()
		{
			String typeStr;
			Rectangle b = (Source_Renamed != null ? ((Component)Source_Renamed).Bounds : null);

			switch (Id)
			{
			  case COMPONENT_SHOWN:
				  typeStr = "COMPONENT_SHOWN";
				  break;
			  case COMPONENT_HIDDEN:
				  typeStr = "COMPONENT_HIDDEN";
				  break;
			  case COMPONENT_MOVED:
				  typeStr = "COMPONENT_MOVED (" + b.x + "," + b.y + " " + b.Width_Renamed + "x" + b.Height_Renamed + ")";
				  break;
			  case COMPONENT_RESIZED:
				  typeStr = "COMPONENT_RESIZED (" + b.x + "," + b.y + " " + b.Width_Renamed + "x" + b.Height_Renamed + ")";
				  break;
			  default:
				  typeStr = "unknown type";
			  break;
			}
			return typeStr;
		}
	}

}