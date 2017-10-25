/*
 * Copyright (c) 1996, 2000, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// The interface for objects which contain a set of items for
	/// which zero or more can be selected.
	/// 
	/// @author Amy Fowler
	/// </summary>

	public interface ItemSelectable
	{

		/// <summary>
		/// Returns the selected items or <code>null</code> if no
		/// items are selected.
		/// </summary>
		Object[] SelectedObjects {get;}

		/// <summary>
		/// Adds a listener to receive item events when the state of an item is
		/// changed by the user. Item events are not sent when an item's
		/// state is set programmatically.  If <code>l</code> is
		/// <code>null</code>, no exception is thrown and no action is performed.
		/// </summary>
		/// <param name="l"> the listener to receive events </param>
		/// <seealso cref= ItemEvent </seealso>
		void AddItemListener(ItemListener l);

		/// <summary>
		/// Removes an item listener.
		/// If <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// </summary>
		/// <param name="l"> the listener being removed </param>
		/// <seealso cref= ItemEvent </seealso>
		void RemoveItemListener(ItemListener l);
	}

}