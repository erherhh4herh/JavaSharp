/*
 * Copyright (c) 2001, 2007, Oracle and/or its affiliates. All rights reserved.
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
	/// A class which extends the {@code EventListenerProxy}
	/// specifically for adding an {@code AWTEventListener}
	/// for a specific event mask.
	/// Instances of this class can be added as {@code AWTEventListener}s
	/// to a {@code Toolkit} object.
	/// <para>
	/// The {@code getAWTEventListeners} method of {@code Toolkit}
	/// can return a mixture of {@code AWTEventListener}
	/// and {@code AWTEventListenerProxy} objects.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.Toolkit </seealso>
	/// <seealso cref= java.util.EventListenerProxy
	/// @since 1.4 </seealso>
	public class AWTEventListenerProxy : EventListenerProxy<AWTEventListener>, AWTEventListener
	{

		private readonly long EventMask_Renamed;

		/// <summary>
		/// Constructor which binds the {@code AWTEventListener}
		/// to a specific event mask.
		/// </summary>
		/// <param name="eventMask">  the bitmap of event types to receive </param>
		/// <param name="listener">   the listener object </param>
		public AWTEventListenerProxy(long eventMask, AWTEventListener listener) : base(listener)
		{
			this.EventMask_Renamed = eventMask;
		}

		/// <summary>
		/// Forwards the AWT event to the listener delegate.
		/// </summary>
		/// <param name="event">  the AWT event </param>
		public virtual void EventDispatched(AWTEvent @event)
		{
			Listener.EventDispatched(@event);
		}

		/// <summary>
		/// Returns the event mask associated with the listener.
		/// </summary>
		/// <returns> the event mask associated with the listener </returns>
		public virtual long EventMask
		{
			get
			{
				return this.EventMask_Renamed;
			}
		}
	}

}