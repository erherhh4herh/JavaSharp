/*
 * Copyright (c) 1995, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// The base interface for all kinds of menu components. This is used by
	/// <seealso cref="MenuComponent"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface MenuComponentPeer
	{

		/// <summary>
		/// Disposes the menu component.
		/// </summary>
		/// <seealso cref= MenuComponent#removeNotify() </seealso>
		void Dispose();

		/// <summary>
		/// Sets the font for the menu component.
		/// </summary>
		/// <param name="f"> the font to use for the menu component
		/// </param>
		/// <seealso cref= MenuComponent#setFont(Font) </seealso>
		Font Font {set;}
	}

}