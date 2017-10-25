/*
 * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// The peer interface for <seealso cref="CheckboxMenuItem"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface CheckboxMenuItemPeer : MenuItemPeer
	{

		/// <summary>
		/// Sets the state of the checkbox to be checked {@code true} or
		/// unchecked {@code false}.
		/// </summary>
		/// <param name="state"> the state to set on the checkbox
		/// </param>
		/// <seealso cref= CheckboxMenuItem#setState(boolean) </seealso>
		bool State {set;}
	}

}