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
	/// The listener interface for receiving keyboard focus events on
	/// a component.
	/// The class that is interested in processing a focus event
	/// either implements this interface (and all the methods it
	/// contains) or extends the abstract <code>FocusAdapter</code> class
	/// (overriding only the methods of interest).
	/// The listener object created from that class is then registered with a
	/// component using the component's <code>addFocusListener</code>
	/// method. When the component gains or loses the keyboard focus,
	/// the relevant method in the listener object
	/// is invoked, and the <code>FocusEvent</code> is passed to it.
	/// </summary>
	/// <seealso cref= FocusAdapter </seealso>
	/// <seealso cref= FocusEvent </seealso>
	/// <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/focuslistener.html">Tutorial: Writing a Focus Listener</a>
	/// 
	/// @author Carl Quinn
	/// @since 1.1 </seealso>
	public interface FocusListener : EventListener
	{

		/// <summary>
		/// Invoked when a component gains the keyboard focus.
		/// </summary>
		void FocusGained(FocusEvent e);

		/// <summary>
		/// Invoked when a component loses the keyboard focus.
		/// </summary>
		void FocusLost(FocusEvent e);
	}

}