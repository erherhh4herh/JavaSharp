/*
 * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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
	/// The peer interface for <seealso cref="PopupMenu"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface PopupMenuPeer : MenuPeer
	{

		/// <summary>
		/// Shows the popup menu.
		/// </summary>
		/// <param name="e"> a synthetic event describing the origin and location of the
		///        popup menu
		/// </param>
		/// <seealso cref= PopupMenu#show(java.awt.Component, int, int) </seealso>
		void Show(Event e);
	}

}