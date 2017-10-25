/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The peer interface for <seealso cref="Window"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface WindowPeer : ContainerPeer
	{

		/// <summary>
		/// Makes this window the topmost window on the desktop.
		/// </summary>
		/// <seealso cref= Window#toFront() </seealso>
		void ToFront();

		/// <summary>
		/// Makes this window the bottommost window on the desktop.
		/// </summary>
		/// <seealso cref= Window#toBack() </seealso>
		void ToBack();

		/// <summary>
		/// Updates the window's always-on-top state.
		/// Sets if the window should always stay
		/// on top of all other windows or not.
		/// </summary>
		/// <seealso cref= Window#getAlwaysOnTop() </seealso>
		/// <seealso cref= Window#setAlwaysOnTop(boolean) </seealso>
		void UpdateAlwaysOnTopState();

		/// <summary>
		/// Updates the window's focusable state.
		/// </summary>
		/// <seealso cref= Window#setFocusableWindowState(boolean) </seealso>
		void UpdateFocusableWindowState();

		/// <summary>
		/// Sets if this window is blocked by a modal dialog or not.
		/// </summary>
		/// <param name="blocker"> the blocking modal dialog </param>
		/// <param name="blocked"> {@code true} to block the window, {@code false}
		///        to unblock it </param>
		void SetModalBlocked(Dialog blocker, bool blocked);

		/// <summary>
		/// Updates the minimum size on the peer.
		/// </summary>
		/// <seealso cref= Window#setMinimumSize(Dimension) </seealso>
		void UpdateMinimumSize();

		/// <summary>
		/// Updates the icons for the window.
		/// </summary>
		/// <seealso cref= Window#setIconImages(java.util.List) </seealso>
		void UpdateIconImages();

		/// <summary>
		/// Sets the level of opacity for the window.
		/// </summary>
		/// <seealso cref= Window#setOpacity(float) </seealso>
		float Opacity {set;}

		/// <summary>
		/// Enables the per-pixel alpha support for the window.
		/// </summary>
		/// <seealso cref= Window#setBackground(Color) </seealso>
		bool Opaque {set;}

		/// <summary>
		/// Updates the native part of non-opaque window.
		/// </summary>
		/// <seealso cref= Window#setBackground(Color) </seealso>
		void UpdateWindow();

		/// <summary>
		/// Instructs the peer to update the position of the security warning.
		/// </summary>
		void RepositionSecurityWarning();
	}

}