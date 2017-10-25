/*
 * Copyright (c) 1998, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// The listener interface for receiving notification of events
	/// dispatched to objects that are instances of Component or
	/// MenuComponent or their subclasses.  Unlike the other EventListeners
	/// in this package, AWTEventListeners passively observe events
	/// being dispatched in the AWT, system-wide.  Most applications
	/// should never use this class; applications which might use
	/// AWTEventListeners include event recorders for automated testing,
	/// and facilities such as the Java Accessibility package.
	/// <para>
	/// The class that is interested in monitoring AWT events
	/// implements this interface, and the object created with that
	/// class is registered with the Toolkit, using the Toolkit's
	/// <code>addAWTEventListener</code> method.  When an event is
	/// dispatched anywhere in the AWT, that object's
	/// <code>eventDispatched</code> method is invoked.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.AWTEvent </seealso>
	/// <seealso cref= java.awt.Toolkit#addAWTEventListener </seealso>
	/// <seealso cref= java.awt.Toolkit#removeAWTEventListener
	/// 
	/// @author Fred Ecks
	/// @since 1.2 </seealso>
	public interface AWTEventListener : EventListener
	{

		/// <summary>
		/// Invoked when an event is dispatched in the AWT.
		/// </summary>
		void EventDispatched(AWTEvent @event);

	}

}