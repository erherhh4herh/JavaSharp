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
	/// A low-level event which indicates that a container's contents
	/// changed because a component was added or removed.
	/// <P>
	/// Container events are provided for notification purposes ONLY;
	/// The AWT will automatically handle changes to the containers
	/// contents internally so that the program works properly regardless of
	/// whether the program is receiving these events or not.
	/// <P>
	/// This low-level event is generated by a container object (such as a
	/// Panel) when a component is added to it or removed from it.
	/// The event is passed to every <code>ContainerListener</code>
	/// or <code>ContainerAdapter</code> object which registered to receive such
	/// events using the component's <code>addContainerListener</code> method.
	/// (<code>ContainerAdapter</code> objects implement the
	/// <code>ContainerListener</code> interface.) Each such listener object
	/// gets this <code>ContainerEvent</code> when the event occurs.
	/// <para>
	/// An unspecified behavior will be caused if the {@code id} parameter
	/// of any particular {@code ContainerEvent} instance is not
	/// in the range from {@code CONTAINER_FIRST} to {@code CONTAINER_LAST}.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ContainerAdapter </seealso>
	/// <seealso cref= ContainerListener </seealso>
	/// <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/containerlistener.html">Tutorial: Writing a Container Listener</a>
	/// 
	/// @author Tim Prinzing
	/// @author Amy Fowler
	/// @since 1.1 </seealso>
	public class ContainerEvent : ComponentEvent
	{

		/// <summary>
		/// The first number in the range of ids used for container events.
		/// </summary>
		public const int CONTAINER_FIRST = 300;

		/// <summary>
		/// The last number in the range of ids used for container events.
		/// </summary>
		public const int CONTAINER_LAST = 301;

	   /// <summary>
	   /// This event indicates that a component was added to the container.
	   /// </summary>
		public const int COMPONENT_ADDED = CONTAINER_FIRST;

		/// <summary>
		/// This event indicates that a component was removed from the container.
		/// </summary>
		public static readonly int COMPONENT_REMOVED = 1 + CONTAINER_FIRST;

		/// <summary>
		/// The non-null component that is being added or
		/// removed from the Container.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getChild() </seealso>
		internal Component Child_Renamed;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -4114942250539772041L;

		/// <summary>
		/// Constructs a <code>ContainerEvent</code> object.
		/// <para> This method throws an
		/// <code>IllegalArgumentException</code> if <code>source</code>
		/// is <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> The <code>Component</code> object (container)
		///               that originated the event </param>
		/// <param name="id">     An integer indicating the type of event.
		///                     For information on allowable values, see
		///                     the class description for <seealso cref="ContainerEvent"/> </param>
		/// <param name="child">  the component that was added or removed </param>
		/// <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		/// <seealso cref= #getContainer() </seealso>
		/// <seealso cref= #getID() </seealso>
		/// <seealso cref= #getChild() </seealso>
		public ContainerEvent(Component source, int id, Component child) : base(source, id)
		{
			this.Child_Renamed = child;
		}

		/// <summary>
		/// Returns the originator of the event.
		/// </summary>
		/// <returns> the <code>Container</code> object that originated
		/// the event, or <code>null</code> if the object is not a
		/// <code>Container</code>. </returns>
		public virtual Container Container
		{
			get
			{
				return (Source_Renamed is Container) ? (Container)Source_Renamed : null;
			}
		}

		/// <summary>
		/// Returns the component that was affected by the event.
		/// </summary>
		/// <returns> the Component object that was added or removed </returns>
		public virtual Component Child
		{
			get
			{
				return Child_Renamed;
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
			  case COMPONENT_ADDED:
				  typeStr = "COMPONENT_ADDED";
				  break;
			  case COMPONENT_REMOVED:
				  typeStr = "COMPONENT_REMOVED";
				  break;
			  default:
				  typeStr = "unknown type";
			  break;
			}
			return typeStr + ",child=" + Child_Renamed.Name;
		}
	}

}