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
	/// The listener interface for receiving text events.
	/// 
	/// The class that is interested in processing a text event
	/// implements this interface. The object created with that
	/// class is then registered with a component using the
	/// component's <code>addTextListener</code> method. When the
	/// component's text changes, the listener object's
	/// <code>textValueChanged</code> method is invoked.
	/// 
	/// @author Georges Saab
	/// </summary>
	/// <seealso cref= TextEvent
	/// 
	/// @since 1.1 </seealso>
	public interface TextListener : EventListener
	{

		/// <summary>
		/// Invoked when the value of the text has changed.
		/// The code written for this method performs the operations
		/// that need to occur when text changes.
		/// </summary>
		void TextValueChanged(TextEvent e);

	}

}