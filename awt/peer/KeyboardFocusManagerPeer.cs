/*
 * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// The native peer interface for <seealso cref="KeyboardFocusManager"/>.
	/// </summary>
	public interface KeyboardFocusManagerPeer
	{

		/// <summary>
		/// Sets the window that should become the focused window.
		/// </summary>
		/// <param name="win"> the window that should become the focused window
		///  </param>
		Window CurrentFocusedWindow {set;get;}


		/// <summary>
		/// Sets the component that should become the focus owner.
		/// </summary>
		/// <param name="comp"> the component to become the focus owner
		/// </param>
		/// <seealso cref= KeyboardFocusManager#setNativeFocusOwner(Component) </seealso>
		Component CurrentFocusOwner {set;get;}


		/// <summary>
		/// Clears the current global focus owner.
		/// </summary>
		/// <param name="activeWindow">
		/// </param>
		/// <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner() </seealso>
		void ClearGlobalFocusOwner(Window activeWindow);

	}

}