/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.awt
{

	internal interface EventFilter
	{

		/// <summary>
		/// Enumeration for possible values for <code>acceptEvent(AWTEvent ev)</code> method. </summary>
		/// <seealso cref= EventDispatchThread#pumpEventsForFilter </seealso>

		EventFilter_FilterAction AcceptEvent(AWTEvent ev);
	}

	internal enum EventFilter_FilterAction
	{
		/// <summary>
		/// ACCEPT means that this filter do not filter the event and allowes other
		/// active filters to proceed it. If all the active filters accept the event, it
		/// is dispatched by the <code>EventDispatchThread</code> </summary>
		/// <seealso cref= EventDispatchThread#pumpEventsForFilter </seealso>
		ACCEPT,
		/// <summary>
		/// REJECT means that this filter filter the event. No other filters are queried,
		/// and the event is not dispatched by the <code>EventDispatchedThread</code> </summary>
		/// <seealso cref= EventDispatchThread#pumpEventsForFilter </seealso>
		REJECT,
		/// <summary>
		/// ACCEPT_IMMEDIATELY means that this filter do not filter the event, no other
		/// filters are queried and to proceed it, and it is dispatched by the
		/// <code>EventDispatchThread</code>
		/// It is not recommended to use ACCEPT_IMMEDIATELY as there may be some active
		/// filters not queried yet that do not accept this event. It is primarily used
		/// by modal filters. </summary>
		/// <seealso cref= EventDispatchThread#pumpEventsForFilter </seealso>
		/// <seealso cref= ModalEventFilter </seealso>
		ACCEPT_IMMEDIATELY
	}

}