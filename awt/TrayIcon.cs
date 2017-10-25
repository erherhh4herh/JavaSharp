using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt
{

	using AppContext = sun.awt.AppContext;
	using SunToolkit = sun.awt.SunToolkit;
	using AWTAccessor = sun.awt.AWTAccessor;
	using HeadlessToolkit = sun.awt.HeadlessToolkit;

	/// <summary>
	/// A <code>TrayIcon</code> object represents a tray icon that can be
	/// added to the <seealso cref="SystemTray system tray"/>. A
	/// <code>TrayIcon</code> can have a tooltip (text), an image, a popup
	/// menu, and a set of listeners associated with it.
	/// 
	/// <para>A <code>TrayIcon</code> can generate various {@link MouseEvent
	/// MouseEvents} and supports adding corresponding listeners to receive
	/// notification of these events.  <code>TrayIcon</code> processes some
	/// of the events by itself.  For example, by default, when the
	/// right-mouse click is performed on the <code>TrayIcon</code> it
	/// displays the specified popup menu.  When the mouse hovers
	/// over the <code>TrayIcon</code> the tooltip is displayed.
	/// 
	/// </para>
	/// <para><strong>Note:</strong> When the <code>MouseEvent</code> is
	/// dispatched to its registered listeners its <code>component</code>
	/// property will be set to <code>null</code>.  (See {@link
	/// java.awt.event.ComponentEvent#getComponent}) The
	/// <code>source</code> property will be set to this
	/// <code>TrayIcon</code>. (See {@link
	/// java.util.EventObject#getSource})
	/// 
	/// </para>
	/// <para><b>Note:</b> A well-behaved <seealso cref="TrayIcon"/> implementation
	/// will assign different gestures to showing a popup menu and
	/// selecting a tray icon.
	/// 
	/// </para>
	/// <para>A <code>TrayIcon</code> can generate an {@link ActionEvent
	/// ActionEvent}.  On some platforms, this occurs when the user selects
	/// the tray icon using either the mouse or keyboard.
	/// 
	/// </para>
	/// <para>If a SecurityManager is installed, the AWTPermission
	/// {@code accessSystemTray} must be granted in order to create
	/// a {@code TrayIcon}. Otherwise the constructor will throw a
	/// SecurityException.
	/// 
	/// </para>
	/// <para> See the <seealso cref="SystemTray"/> class overview for an example on how
	/// to use the <code>TrayIcon</code> API.
	/// 
	/// @since 1.6
	/// </para>
	/// </summary>
	/// <seealso cref= SystemTray#add </seealso>
	/// <seealso cref= java.awt.event.ComponentEvent#getComponent </seealso>
	/// <seealso cref= java.util.EventObject#getSource
	/// 
	/// @author Bino George
	/// @author Denis Mikhalkin
	/// @author Sharon Zakhour
	/// @author Anton Tarasov </seealso>
	public class TrayIcon
	{

		private Image Image_Renamed;
		private String Tooltip;
		private PopupMenu Popup;
		private bool Autosize;
		private int Id;
		private String ActionCommand_Renamed;

		[NonSerialized]
		private TrayIconPeer Peer;

		[NonSerialized]
		internal MouseListener MouseListener;
		[NonSerialized]
		internal MouseMotionListener MouseMotionListener;
		[NonSerialized]
		internal ActionListener ActionListener;

		/*
		 * The tray icon's AccessControlContext.
		 *
		 * Unlike the acc in Component, this field is made final
		 * because TrayIcon is not serializable.
		 */
		private readonly AccessControlContext Acc = AccessController.Context;

		/*
		 * Returns the acc this tray icon was constructed with.
		 */
		internal AccessControlContext AccessControlContext
		{
			get
			{
				if (Acc == null)
				{
					throw new SecurityException("TrayIcon is missing AccessControlContext");
				}
				return Acc;
			}
		}

		static TrayIcon()
		{
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}

			AWTAccessor.TrayIconAccessor = new TrayIconAccessorAnonymousInnerClassHelper();
		}

		private class TrayIconAccessorAnonymousInnerClassHelper : AWTAccessor.TrayIconAccessor
		{
			public TrayIconAccessorAnonymousInnerClassHelper()
			{
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addNotify(TrayIcon trayIcon) throws AWTException
			public virtual void AddNotify(TrayIcon trayIcon)
			{
				trayIcon.AddNotify();
			}
			public virtual void RemoveNotify(TrayIcon trayIcon)
			{
				trayIcon.RemoveNotify();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private TrayIcon() throws UnsupportedOperationException, HeadlessException, SecurityException
		private TrayIcon()
		{
			SystemTray.CheckSystemTrayAllowed();
			if (GraphicsEnvironment.Headless)
			{
				throw new HeadlessException();
			}
			if (!SystemTray.Supported)
			{
				throw new UnsupportedOperationException();
			}
			SunToolkit.insertTargetMapping(this, AppContext.AppContext);
		}

		/// <summary>
		/// Creates a <code>TrayIcon</code> with the specified image.
		/// </summary>
		/// <param name="image"> the <code>Image</code> to be used </param>
		/// <exception cref="IllegalArgumentException"> if <code>image</code> is
		/// <code>null</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if the system tray isn't
		/// supported by the current platform </exception>
		/// <exception cref="HeadlessException"> if
		/// {@code GraphicsEnvironment.isHeadless()} returns {@code true} </exception>
		/// <exception cref="SecurityException"> if {@code accessSystemTray} permission
		/// is not granted </exception>
		/// <seealso cref= SystemTray#add(TrayIcon) </seealso>
		/// <seealso cref= TrayIcon#TrayIcon(Image, String, PopupMenu) </seealso>
		/// <seealso cref= TrayIcon#TrayIcon(Image, String) </seealso>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= AWTPermission </seealso>
		public TrayIcon(Image image) : this()
		{
			if (image == null)
			{
				throw new IllegalArgumentException("creating TrayIcon with null Image");
			}
			Image = image;
		}

		/// <summary>
		/// Creates a <code>TrayIcon</code> with the specified image and
		/// tooltip text.
		/// </summary>
		/// <param name="image"> the <code>Image</code> to be used </param>
		/// <param name="tooltip"> the string to be used as tooltip text; if the
		/// value is <code>null</code> no tooltip is shown </param>
		/// <exception cref="IllegalArgumentException"> if <code>image</code> is
		/// <code>null</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if the system tray isn't
		/// supported by the current platform </exception>
		/// <exception cref="HeadlessException"> if
		/// {@code GraphicsEnvironment.isHeadless()} returns {@code true} </exception>
		/// <exception cref="SecurityException"> if {@code accessSystemTray} permission
		/// is not granted </exception>
		/// <seealso cref= SystemTray#add(TrayIcon) </seealso>
		/// <seealso cref= TrayIcon#TrayIcon(Image) </seealso>
		/// <seealso cref= TrayIcon#TrayIcon(Image, String, PopupMenu) </seealso>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= AWTPermission </seealso>
		public TrayIcon(Image image, String tooltip) : this(image)
		{
			ToolTip = tooltip;
		}

		/// <summary>
		/// Creates a <code>TrayIcon</code> with the specified image,
		/// tooltip and popup menu.
		/// </summary>
		/// <param name="image"> the <code>Image</code> to be used </param>
		/// <param name="tooltip"> the string to be used as tooltip text; if the
		/// value is <code>null</code> no tooltip is shown </param>
		/// <param name="popup"> the menu to be used for the tray icon's popup
		/// menu; if the value is <code>null</code> no popup menu is shown </param>
		/// <exception cref="IllegalArgumentException"> if <code>image</code> is <code>null</code> </exception>
		/// <exception cref="UnsupportedOperationException"> if the system tray isn't
		/// supported by the current platform </exception>
		/// <exception cref="HeadlessException"> if
		/// {@code GraphicsEnvironment.isHeadless()} returns {@code true} </exception>
		/// <exception cref="SecurityException"> if {@code accessSystemTray} permission
		/// is not granted </exception>
		/// <seealso cref= SystemTray#add(TrayIcon) </seealso>
		/// <seealso cref= TrayIcon#TrayIcon(Image, String) </seealso>
		/// <seealso cref= TrayIcon#TrayIcon(Image) </seealso>
		/// <seealso cref= PopupMenu </seealso>
		/// <seealso cref= MouseListener </seealso>
		/// <seealso cref= #addMouseListener(MouseListener) </seealso>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= AWTPermission </seealso>
		public TrayIcon(Image image, String tooltip, PopupMenu popup) : this(image, tooltip)
		{
			PopupMenu = popup;
		}

		/// <summary>
		/// Sets the image for this <code>TrayIcon</code>.  The previous
		/// tray icon image is discarded without calling the {@link
		/// java.awt.Image#flush} method &#151; you will need to call it
		/// manually.
		/// 
		/// <para> If the image represents an animated image, it will be
		/// animated automatically.
		/// 
		/// </para>
		/// <para> See the <seealso cref="#setImageAutoSize(boolean)"/> property for
		/// details on the size of the displayed image.
		/// 
		/// </para>
		/// <para> Calling this method with the same image that is currently
		/// being used has no effect.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if <code>image</code> is <code>null</code> </exception>
		/// <param name="image"> the non-null <code>Image</code> to be used </param>
		/// <seealso cref= #getImage </seealso>
		/// <seealso cref= Image </seealso>
		/// <seealso cref= SystemTray#add(TrayIcon) </seealso>
		/// <seealso cref= TrayIcon#TrayIcon(Image, String) </seealso>
		public virtual Image Image
		{
			set
			{
				if (value == null)
				{
					throw new NullPointerException("setting null Image");
				}
				this.Image_Renamed = value;
    
				TrayIconPeer peer = this.Peer;
				if (peer != null)
				{
					peer.UpdateImage();
				}
			}
			get
			{
				return Image_Renamed;
			}
		}


		/// <summary>
		/// Sets the popup menu for this <code>TrayIcon</code>.  If
		/// <code>popup</code> is <code>null</code>, no popup menu will be
		/// associated with this <code>TrayIcon</code>.
		/// 
		/// <para>Note that this <code>popup</code> must not be added to any
		/// parent before or after it is set on the tray icon.  If you add
		/// it to some parent, the <code>popup</code> may be removed from
		/// that parent.
		/// 
		/// </para>
		/// <para>The {@code popup} can be set on one {@code TrayIcon} only.
		/// Setting the same popup on multiple {@code TrayIcon}s will cause
		/// an {@code IllegalArgumentException}.
		/// 
		/// </para>
		/// <para><strong>Note:</strong> Some platforms may not support
		/// showing the user-specified popup menu component when the user
		/// right-clicks the tray icon.  In this situation, either no menu
		/// will be displayed or, on some systems, a native version of the
		/// menu may be displayed.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if the {@code popup} is already
		/// set for another {@code TrayIcon} </exception>
		/// <param name="popup"> a <code>PopupMenu</code> or <code>null</code> to
		/// remove any popup menu </param>
		/// <seealso cref= #getPopupMenu </seealso>
		public virtual PopupMenu PopupMenu
		{
			set
			{
				if (value == this.Popup)
				{
					return;
				}
				lock (typeof(TrayIcon))
				{
					if (value != null)
					{
						if (value.IsTrayIconPopup)
						{
							throw new IllegalArgumentException("the PopupMenu is already set for another TrayIcon");
						}
						value.IsTrayIconPopup = true;
					}
					if (this.Popup != null)
					{
						this.Popup.IsTrayIconPopup = false;
					}
					this.Popup = value;
				}
			}
			get
			{
				return Popup;
			}
		}


		/// <summary>
		/// Sets the tooltip string for this <code>TrayIcon</code>. The
		/// tooltip is displayed automatically when the mouse hovers over
		/// the icon.  Setting the tooltip to <code>null</code> removes any
		/// tooltip text.
		/// 
		/// When displayed, the tooltip string may be truncated on some platforms;
		/// the number of characters that may be displayed is platform-dependent.
		/// </summary>
		/// <param name="tooltip"> the string for the tooltip; if the value is
		/// <code>null</code> no tooltip is shown </param>
		/// <seealso cref= #getToolTip </seealso>
		public virtual String ToolTip
		{
			set
			{
				this.Tooltip = value;
    
				TrayIconPeer peer = this.Peer;
				if (peer != null)
				{
					peer.ToolTip = value;
				}
			}
			get
			{
				return Tooltip;
			}
		}


		/// <summary>
		/// Sets the auto-size property.  Auto-size determines whether the
		/// tray image is automatically sized to fit the space allocated
		/// for the image on the tray.  By default, the auto-size property
		/// is set to <code>false</code>.
		/// 
		/// <para> If auto-size is <code>false</code>, and the image size
		/// doesn't match the tray icon space, the image is painted as-is
		/// inside that space &#151; if larger than the allocated space, it will
		/// be cropped.
		/// 
		/// </para>
		/// <para> If auto-size is <code>true</code>, the image is stretched or shrunk to
		/// fit the tray icon space.
		/// 
		/// </para>
		/// </summary>
		/// <param name="autosize"> <code>true</code> to auto-size the image,
		/// <code>false</code> otherwise </param>
		/// <seealso cref= #isImageAutoSize </seealso>
		public virtual bool ImageAutoSize
		{
			set
			{
				this.Autosize = value;
    
				TrayIconPeer peer = this.Peer;
				if (peer != null)
				{
					peer.UpdateImage();
				}
			}
			get
			{
				return Autosize;
			}
		}


		/// <summary>
		/// Adds the specified mouse listener to receive mouse events from
		/// this <code>TrayIcon</code>.  Calling this method with a
		/// <code>null</code> value has no effect.
		/// 
		/// <para><b>Note</b>: The {@code MouseEvent}'s coordinates (received
		/// from the {@code TrayIcon}) are relative to the screen, not the
		/// {@code TrayIcon}.
		/// 
		/// </para>
		/// <para> <b>Note: </b>The <code>MOUSE_ENTERED</code> and
		/// <code>MOUSE_EXITED</code> mouse events are not supported.
		/// </para>
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener"> the mouse listener </param>
		/// <seealso cref=      java.awt.event.MouseEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseListener </seealso>
		/// <seealso cref=      #removeMouseListener(MouseListener) </seealso>
		/// <seealso cref=      #getMouseListeners </seealso>
		public virtual void AddMouseListener(MouseListener listener)
		{
			lock (this)
			{
				if (listener == null)
				{
					return;
				}
				MouseListener = AWTEventMulticaster.Add(MouseListener, listener);
			}
		}

		/// <summary>
		/// Removes the specified mouse listener.  Calling this method with
		/// <code>null</code> or an invalid value has no effect.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">   the mouse listener </param>
		/// <seealso cref=      java.awt.event.MouseEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseListener </seealso>
		/// <seealso cref=      #addMouseListener(MouseListener) </seealso>
		/// <seealso cref=      #getMouseListeners </seealso>
		public virtual void RemoveMouseListener(MouseListener listener)
		{
			lock (this)
			{
				if (listener == null)
				{
					return;
				}
				MouseListener = AWTEventMulticaster.Remove(MouseListener, listener);
			}
		}

		/// <summary>
		/// Returns an array of all the mouse listeners
		/// registered on this <code>TrayIcon</code>.
		/// </summary>
		/// <returns> all of the <code>MouseListeners</code> registered on
		/// this <code>TrayIcon</code> or an empty array if no mouse
		/// listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addMouseListener(MouseListener) </seealso>
		/// <seealso cref=      #removeMouseListener(MouseListener) </seealso>
		/// <seealso cref=      java.awt.event.MouseListener </seealso>
		public virtual MouseListener[] MouseListeners
		{
			get
			{
				lock (this)
				{
					return AWTEventMulticaster.GetListeners(MouseListener, typeof(MouseListener));
				}
			}
		}

		/// <summary>
		/// Adds the specified mouse listener to receive mouse-motion
		/// events from this <code>TrayIcon</code>.  Calling this method
		/// with a <code>null</code> value has no effect.
		/// 
		/// <para><b>Note</b>: The {@code MouseEvent}'s coordinates (received
		/// from the {@code TrayIcon}) are relative to the screen, not the
		/// {@code TrayIcon}.
		/// 
		/// </para>
		/// <para> <b>Note: </b>The <code>MOUSE_DRAGGED</code> mouse event is not supported.
		/// </para>
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">   the mouse listener </param>
		/// <seealso cref=      java.awt.event.MouseEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseMotionListener </seealso>
		/// <seealso cref=      #removeMouseMotionListener(MouseMotionListener) </seealso>
		/// <seealso cref=      #getMouseMotionListeners </seealso>
		public virtual void AddMouseMotionListener(MouseMotionListener listener)
		{
			lock (this)
			{
				if (listener == null)
				{
					return;
				}
				MouseMotionListener = AWTEventMulticaster.Add(MouseMotionListener, listener);
			}
		}

		/// <summary>
		/// Removes the specified mouse-motion listener.  Calling this method with
		/// <code>null</code> or an invalid value has no effect.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">   the mouse listener </param>
		/// <seealso cref=      java.awt.event.MouseEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseMotionListener </seealso>
		/// <seealso cref=      #addMouseMotionListener(MouseMotionListener) </seealso>
		/// <seealso cref=      #getMouseMotionListeners </seealso>
		public virtual void RemoveMouseMotionListener(MouseMotionListener listener)
		{
			lock (this)
			{
				if (listener == null)
				{
					return;
				}
				MouseMotionListener = AWTEventMulticaster.Remove(MouseMotionListener, listener);
			}
		}

		/// <summary>
		/// Returns an array of all the mouse-motion listeners
		/// registered on this <code>TrayIcon</code>.
		/// </summary>
		/// <returns> all of the <code>MouseInputListeners</code> registered on
		/// this <code>TrayIcon</code> or an empty array if no mouse
		/// listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addMouseMotionListener(MouseMotionListener) </seealso>
		/// <seealso cref=      #removeMouseMotionListener(MouseMotionListener) </seealso>
		/// <seealso cref=      java.awt.event.MouseMotionListener </seealso>
		public virtual MouseMotionListener[] MouseMotionListeners
		{
			get
			{
				lock (this)
				{
					return AWTEventMulticaster.GetListeners(MouseMotionListener, typeof(MouseMotionListener));
				}
			}
		}

		/// <summary>
		/// Returns the command name of the action event fired by this tray icon.
		/// </summary>
		/// <returns> the action command name, or <code>null</code> if none exists </returns>
		/// <seealso cref= #addActionListener(ActionListener) </seealso>
		/// <seealso cref= #setActionCommand(String) </seealso>
		public virtual String ActionCommand
		{
			get
			{
				return ActionCommand_Renamed;
			}
			set
			{
				ActionCommand_Renamed = value;
			}
		}


		/// <summary>
		/// Adds the specified action listener to receive
		/// <code>ActionEvent</code>s from this <code>TrayIcon</code>.
		/// Action events usually occur when a user selects the tray icon,
		/// using either the mouse or keyboard.  The conditions in which
		/// action events are generated are platform-dependent.
		/// 
		/// <para>Calling this method with a <code>null</code> value has no
		/// effect.
		/// </para>
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener"> the action listener </param>
		/// <seealso cref=           #removeActionListener </seealso>
		/// <seealso cref=           #getActionListeners </seealso>
		/// <seealso cref=           java.awt.event.ActionListener </seealso>
		/// <seealso cref= #setActionCommand(String) </seealso>
		public virtual void AddActionListener(ActionListener listener)
		{
			lock (this)
			{
				if (listener == null)
				{
					return;
				}
				ActionListener = AWTEventMulticaster.Add(ActionListener, listener);
			}
		}

		/// <summary>
		/// Removes the specified action listener.  Calling this method with
		/// <code>null</code> or an invalid value has no effect.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">   the action listener </param>
		/// <seealso cref=      java.awt.event.ActionEvent </seealso>
		/// <seealso cref=      java.awt.event.ActionListener </seealso>
		/// <seealso cref=      #addActionListener(ActionListener) </seealso>
		/// <seealso cref=      #getActionListeners </seealso>
		/// <seealso cref= #setActionCommand(String) </seealso>
		public virtual void RemoveActionListener(ActionListener listener)
		{
			lock (this)
			{
				if (listener == null)
				{
					return;
				}
				ActionListener = AWTEventMulticaster.Remove(ActionListener, listener);
			}
		}

		/// <summary>
		/// Returns an array of all the action listeners
		/// registered on this <code>TrayIcon</code>.
		/// </summary>
		/// <returns> all of the <code>ActionListeners</code> registered on
		/// this <code>TrayIcon</code> or an empty array if no action
		/// listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addActionListener(ActionListener) </seealso>
		/// <seealso cref=      #removeActionListener(ActionListener) </seealso>
		/// <seealso cref=      java.awt.event.ActionListener </seealso>
		public virtual ActionListener[] ActionListeners
		{
			get
			{
				lock (this)
				{
					return AWTEventMulticaster.GetListeners(ActionListener, typeof(ActionListener));
				}
			}
		}

		/// <summary>
		/// The message type determines which icon will be displayed in the
		/// caption of the message, and a possible system sound a message
		/// may generate upon showing.
		/// </summary>
		/// <seealso cref= TrayIcon </seealso>
		/// <seealso cref= TrayIcon#displayMessage(String, String, MessageType)
		/// @since 1.6 </seealso>
		public enum MessageType
		{
			/// <summary>
			/// An error message </summary>
			ERROR,
			/// <summary>
			/// A warning message </summary>
			WARNING,
			/// <summary>
			/// An information message </summary>
			INFO,
			/// <summary>
			/// Simple message </summary>
			NONE
		}

		/// <summary>
		/// Displays a popup message near the tray icon.  The message will
		/// disappear after a time or if the user clicks on it.  Clicking
		/// on the message may trigger an {@code ActionEvent}.
		/// 
		/// <para>Either the caption or the text may be <code>null</code>, but an
		/// <code>NullPointerException</code> is thrown if both are
		/// <code>null</code>.
		/// 
		/// When displayed, the caption or text strings may be truncated on
		/// some platforms; the number of characters that may be displayed is
		/// platform-dependent.
		/// 
		/// </para>
		/// <para><strong>Note:</strong> Some platforms may not support
		/// showing a message.
		/// 
		/// </para>
		/// </summary>
		/// <param name="caption"> the caption displayed above the text, usually in
		/// bold; may be <code>null</code> </param>
		/// <param name="text"> the text displayed for the particular message; may be
		/// <code>null</code> </param>
		/// <param name="messageType"> an enum indicating the message type </param>
		/// <exception cref="NullPointerException"> if both <code>caption</code>
		/// and <code>text</code> are <code>null</code> </exception>
		public virtual void DisplayMessage(String caption, String text, MessageType messageType)
		{
			if (caption == null && text == null)
			{
				throw new NullPointerException("displaying the message with both caption and text being null");
			}

			TrayIconPeer peer = this.Peer;
			if (peer != null)
			{
				peer.DisplayMessage(caption, text, messageType.name());
			}
		}

		/// <summary>
		/// Returns the size, in pixels, of the space that the tray icon
		/// occupies in the system tray.  For the tray icon that is not yet
		/// added to the system tray, the returned size is equal to the
		/// result of the <seealso cref="SystemTray#getTrayIconSize"/>.
		/// </summary>
		/// <returns> the size of the tray icon, in pixels </returns>
		/// <seealso cref= TrayIcon#setImageAutoSize(boolean) </seealso>
		/// <seealso cref= java.awt.Image </seealso>
		/// <seealso cref= TrayIcon#getSize() </seealso>
		public virtual Dimension Size
		{
			get
			{
				return SystemTray.SystemTray.TrayIconSize;
			}
		}

		// ****************************************************************
		// ****************************************************************

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void addNotify() throws AWTException
		internal virtual void AddNotify()
		{
			lock (this)
			{
				if (Peer == null)
				{
					Toolkit toolkit = Toolkit.DefaultToolkit;
					if (toolkit is SunToolkit)
					{
						Peer = ((SunToolkit)Toolkit.DefaultToolkit).createTrayIcon(this);
					}
					else if (toolkit is HeadlessToolkit)
					{
						Peer = ((HeadlessToolkit)Toolkit.DefaultToolkit).createTrayIcon(this);
					}
				}
			}
			Peer.ToolTip = Tooltip;
		}

		internal virtual void RemoveNotify()
		{
			TrayIconPeer p = null;
			lock (this)
			{
				p = Peer;
				Peer = null;
			}
			if (p != null)
			{
				p.Dispose();
			}
		}

		internal virtual int ID
		{
			set
			{
				this.Id = value;
			}
			get
			{
				return Id;
			}
		}


		internal virtual void DispatchEvent(AWTEvent e)
		{
			EventQueue.CurrentEventAndMostRecentTime = e;
			Toolkit.DefaultToolkit.NotifyAWTEventListeners(e);
			ProcessEvent(e);
		}

		internal virtual void ProcessEvent(AWTEvent e)
		{
			if (e is MouseEvent)
			{
				switch (e.ID)
				{
				case MouseEvent.MOUSE_PRESSED:
				case MouseEvent.MOUSE_RELEASED:
				case MouseEvent.MOUSE_CLICKED:
					ProcessMouseEvent((MouseEvent)e);
					break;
				case MouseEvent.MOUSE_MOVED:
					ProcessMouseMotionEvent((MouseEvent)e);
					break;
				default:
					return;
				}
			}
			else if (e is ActionEvent)
			{
				ProcessActionEvent((ActionEvent)e);
			}
		}

		internal virtual void ProcessMouseEvent(MouseEvent e)
		{
			MouseListener listener = MouseListener;

			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				case MouseEvent.MOUSE_PRESSED:
					listener.MousePressed(e);
					break;
				case MouseEvent.MOUSE_RELEASED:
					listener.MouseReleased(e);
					break;
				case MouseEvent.MOUSE_CLICKED:
					listener.MouseClicked(e);
					break;
				default:
					return;
				}
			}
		}

		internal virtual void ProcessMouseMotionEvent(MouseEvent e)
		{
			MouseMotionListener listener = MouseMotionListener;
			if (listener != null && e.ID == MouseEvent.MOUSE_MOVED)
			{
				listener.MouseMoved(e);
			}
		}

		internal virtual void ProcessActionEvent(ActionEvent e)
		{
			ActionListener listener = ActionListener;
			if (listener != null)
			{
				listener.ActionPerformed(e);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();
	}

}