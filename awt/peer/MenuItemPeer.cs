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
	/// The peer interface for menu items. This is used by <seealso cref="MenuItem"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface MenuItemPeer : MenuComponentPeer
	{

		/// <summary>
		/// Sets the label to be displayed in this menu item.
		/// </summary>
		/// <param name="label"> the label to be displayed </param>
		String Label {set;}

		/// <summary>
		/// Enables or disables the menu item.
		/// </summary>
		/// <param name="e"> {@code true} to enable the menu item, {@code false}
		///        to disable it </param>
		bool Enabled {set;}

	}

}