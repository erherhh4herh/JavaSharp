/*
 * Copyright (c) 1995, 2007, Oracle and/or its affiliates. All rights reserved.
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
namespace java.awt.peer
{

	/// <summary>
	/// The peer interface for <seealso cref="Choice"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface ChoicePeer : ComponentPeer
	{

		/// <summary>
		/// Adds an item with the string {@code item} to the combo box list
		/// at index {@code index}.
		/// </summary>
		/// <param name="item"> the label to be added to the list </param>
		/// <param name="index"> the index where to add the item
		/// </param>
		/// <seealso cref= Choice#add(String) </seealso>
		void Add(String item, int index);

		/// <summary>
		/// Removes the item at index {@code index} from the combo box list.
		/// </summary>
		/// <param name="index"> the index where to remove the item
		/// </param>
		/// <seealso cref= Choice#remove(int) </seealso>
		void Remove(int index);

		/// <summary>
		/// Removes all items from the combo box list.
		/// </summary>
		/// <seealso cref= Choice#removeAll() </seealso>
		void RemoveAll();

		/// <summary>
		/// Selects the item at index {@code index}.
		/// </summary>
		/// <param name="index"> the index which should be selected
		/// </param>
		/// <seealso cref= Choice#select(int) </seealso>
		void Select(int index);

	}

}