/*
 * Copyright (c) 1995, 1998, Oracle and/or its affiliates. All rights reserved.
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
	/// The peer interface for menus. This is used by <seealso cref="Menu"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface MenuPeer : MenuItemPeer
	{

		/// <summary>
		/// Adds a separator (e.g. a horizontal line or similar) to the menu.
		/// </summary>
		/// <seealso cref= Menu#addSeparator() </seealso>
		void AddSeparator();

		/// <summary>
		/// Adds the specified menu item to the menu.
		/// </summary>
		/// <param name="item"> the menu item to add
		/// </param>
		/// <seealso cref= Menu#add(MenuItem) </seealso>
		void AddItem(MenuItem item);

		/// <summary>
		/// Removes the menu item at the specified index.
		/// </summary>
		/// <param name="index"> the index of the item to remove
		/// </param>
		/// <seealso cref= Menu#remove(int) </seealso>
		void DelItem(int index);
	}

}