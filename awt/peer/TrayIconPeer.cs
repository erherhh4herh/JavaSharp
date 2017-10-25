/*
 * Copyright (c) 2005, 2007, Oracle and/or its affiliates. All rights reserved.
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
	/// The peer interface for the <seealso cref="TrayIcon"/>. This doesn't need to be
	/// implemented if <seealso cref="SystemTray#isSupported()"/> returns false.
	/// </summary>
	public interface TrayIconPeer
	{

		/// <summary>
		/// Disposes the tray icon and releases and resources held by it.
		/// </summary>
		/// <seealso cref= TrayIcon#removeNotify() </seealso>
		void Dispose();

		/// <summary>
		/// Sets the tool tip for the tray icon.
		/// </summary>
		/// <param name="tooltip"> the tooltip to set
		/// </param>
		/// <seealso cref= TrayIcon#setToolTip(String) </seealso>
		String ToolTip {set;}

		/// <summary>
		/// Updates the icon image. This is supposed to display the current icon
		/// from the TrayIcon component in the actual tray icon.
		/// </summary>
		/// <seealso cref= TrayIcon#setImage(java.awt.Image) </seealso>
		/// <seealso cref= TrayIcon#setImageAutoSize(boolean) </seealso>
		void UpdateImage();

		/// <summary>
		/// Displays a message at the tray icon.
		/// </summary>
		/// <param name="caption"> the message caption </param>
		/// <param name="text"> the actual message text </param>
		/// <param name="messageType"> the message type
		/// </param>
		/// <seealso cref= TrayIcon#displayMessage(String, String, java.awt.TrayIcon.MessageType) </seealso>
		void DisplayMessage(String caption, String text, String messageType);

		/// <summary>
		/// Shows the popup menu of this tray icon at the specified position.
		/// </summary>
		/// <param name="x"> the X location for the popup menu </param>
		/// <param name="y"> the Y location for the popup menu </param>
		void ShowPopupMenu(int x, int y);
	}

}