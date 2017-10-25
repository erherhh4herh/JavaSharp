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
	/// The listener interface for receiving item events.
	/// The class that is interested in processing an item event
	/// implements this interface. The object created with that
	/// class is then registered with a component using the
	/// component's <code>addItemListener</code> method. When an
	/// item-selection event occurs, the listener object's
	/// <code>itemStateChanged</code> method is invoked.
	/// 
	/// @author Amy Fowler
	/// </summary>
	/// <seealso cref= java.awt.ItemSelectable </seealso>
	/// <seealso cref= ItemEvent </seealso>
	/// <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/itemlistener.html">Tutorial: Writing an Item Listener</a>
	/// 
	/// @since 1.1 </seealso>
	public interface ItemListener : EventListener
	{

		/// <summary>
		/// Invoked when an item has been selected or deselected by the user.
		/// The code written for this method performs the operations
		/// that need to occur when an item is selected (or deselected).
		/// </summary>
		void ItemStateChanged(ItemEvent e);

	}

}