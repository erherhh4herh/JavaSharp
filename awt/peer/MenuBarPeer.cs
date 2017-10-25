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
	/// The peer interface for <seealso cref="MenuBar"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface MenuBarPeer : MenuComponentPeer
	{

		/// <summary>
		/// Adds a menu to the menu bar.
		/// </summary>
		/// <param name="m"> the menu to add
		/// </param>
		/// <seealso cref= MenuBar#add(Menu) </seealso>
		void AddMenu(Menu m);

		/// <summary>
		/// Deletes a menu from the menu bar.
		/// </summary>
		/// <param name="index"> the index of the menu to remove
		/// </param>
		/// <seealso cref= MenuBar#remove(int) </seealso>
		void DelMenu(int index);

		/// <summary>
		/// Adds a help menu to the menu bar.
		/// </summary>
		/// <param name="m"> the help menu to add
		/// </param>
		/// <seealso cref= MenuBar#setHelpMenu(Menu) </seealso>
		void AddHelpMenu(Menu m);
	}

}