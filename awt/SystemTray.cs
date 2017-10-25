using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	using HeadlessToolkit = sun.awt.HeadlessToolkit;
	using SecurityConstants = sun.security.util.SecurityConstants;
	using AWTAccessor = sun.awt.AWTAccessor;

	/// <summary>
	/// The <code>SystemTray</code> class represents the system tray for a
	/// desktop.  On Microsoft Windows it is referred to as the "Taskbar
	/// Status Area", on Gnome it is referred to as the "Notification
	/// Area", on KDE it is referred to as the "System Tray".  The system
	/// tray is shared by all applications running on the desktop.
	/// 
	/// <para> On some platforms the system tray may not be present or may not
	/// be supported, in this case <seealso cref="SystemTray#getSystemTray()"/>
	/// throws <seealso cref="UnsupportedOperationException"/>.  To detect whether the
	/// system tray is supported, use <seealso cref="SystemTray#isSupported"/>.
	/// 
	/// </para>
	/// <para>The <code>SystemTray</code> may contain one or more {@link
	/// TrayIcon TrayIcons}, which are added to the tray using the {@link
	/// #add} method, and removed when no longer needed, using the
	/// <seealso cref="#remove"/>.  <code>TrayIcon</code> consists of an
	/// image, a popup menu and a set of associated listeners.  Please see
	/// the <seealso cref="TrayIcon"/> class for details.
	/// 
	/// </para>
	/// <para>Every Java application has a single <code>SystemTray</code>
	/// instance that allows the app to interface with the system tray of
	/// the desktop while the app is running.  The <code>SystemTray</code>
	/// instance can be obtained from the <seealso cref="#getSystemTray"/> method.
	/// An application may not create its own instance of
	/// <code>SystemTray</code>.
	/// 
	/// </para>
	/// <para>The following code snippet demonstrates how to access
	/// and customize the system tray:
	/// <pre>
	/// <code>
	///     <seealso cref="TrayIcon"/> trayIcon = null;
	///     if (SystemTray.isSupported()) {
	///         // get the SystemTray instance
	///         SystemTray tray = SystemTray.<seealso cref="#getSystemTray"/>;
	///         // load an image
	///         <seealso cref="java.awt.Image"/> image = <seealso cref="java.awt.Toolkit#getImage(String) Toolkit.getDefaultToolkit().getImage"/>(...);
	///         // create a action listener to listen for default action executed on the tray icon
	///         <seealso cref="java.awt.event.ActionListener"/> listener = new <seealso cref="java.awt.event.ActionListener ActionListener"/>() {
	///             public void <seealso cref="java.awt.event.ActionListener#actionPerformed actionPerformed"/>(<seealso cref="java.awt.event.ActionEvent"/> e) {
	///                 // execute default action of the application
	///                 // ...
	///             }
	///         };
	///         // create a popup menu
	///         <seealso cref="java.awt.PopupMenu"/> popup = new <seealso cref="java.awt.PopupMenu#PopupMenu PopupMenu"/>();
	///         // create menu item for the default action
	///         MenuItem defaultItem = new MenuItem(...);
	///         defaultItem.addActionListener(listener);
	///         popup.add(defaultItem);
	///         /// ... add other items
	///         // construct a TrayIcon
	///         trayIcon = new <seealso cref="TrayIcon#TrayIcon(java.awt.Image, String, java.awt.PopupMenu) TrayIcon"/>(image, "Tray Demo", popup);
	///         // set the TrayIcon properties
	///         trayIcon.<seealso cref="TrayIcon#addActionListener(java.awt.event.ActionListener) addActionListener"/>(listener);
	///         // ...
	///         // add the tray image
	///         try {
	///             tray.<seealso cref="SystemTray#add(TrayIcon) add"/>(trayIcon);
	///         } catch (AWTException e) {
	///             System.err.println(e);
	///         }
	///         // ...
	///     } else {
	///         // disable tray option in your application or
	///         // perform other actions
	///         ...
	///     }
	///     // ...
	///     // some time later
	///     // the application state has changed - update the image
	///     if (trayIcon != null) {
	///         trayIcon.<seealso cref="TrayIcon#setImage(java.awt.Image) setImage"/>(updatedImage);
	///     }
	///     // ...
	/// </code>
	/// </pre>
	/// 
	/// @since 1.6
	/// </para>
	/// </summary>
	/// <seealso cref= TrayIcon
	/// 
	/// @author Bino George
	/// @author Denis Mikhalkin
	/// @author Sharon Zakhour
	/// @author Anton Tarasov </seealso>
	public class SystemTray
	{
		private static SystemTray SystemTray_Renamed;
		private int CurrentIconID = 0; // each TrayIcon added gets a unique ID

		[NonSerialized]
		private SystemTrayPeer Peer;

		private static readonly TrayIcon[] EMPTY_TRAY_ARRAY = new TrayIcon[0];

		static SystemTray()
		{
			AWTAccessor.SystemTrayAccessor = new SystemTrayAccessorAnonymousInnerClassHelper();
		}

		private class SystemTrayAccessorAnonymousInnerClassHelper : AWTAccessor.SystemTrayAccessor
		{
			public SystemTrayAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual void FirePropertyChange(SystemTray tray, String propertyName, Object oldValue, Object newValue)
			{
				tray.FirePropertyChange(propertyName, oldValue, newValue);
			}
		}

		/// <summary>
		/// Private <code>SystemTray</code> constructor.
		/// 
		/// </summary>
		private SystemTray()
		{
			AddNotify();
		}

		/// <summary>
		/// Gets the <code>SystemTray</code> instance that represents the
		/// desktop's tray area.  This always returns the same instance per
		/// application.  On some platforms the system tray may not be
		/// supported.  You may use the <seealso cref="#isSupported"/> method to
		/// check if the system tray is supported.
		/// 
		/// <para>If a SecurityManager is installed, the AWTPermission
		/// {@code accessSystemTray} must be granted in order to get the
		/// {@code SystemTray} instance. Otherwise this method will throw a
		/// SecurityException.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <code>SystemTray</code> instance that represents
		/// the desktop's tray area </returns>
		/// <exception cref="UnsupportedOperationException"> if the system tray isn't
		/// supported by the current platform </exception>
		/// <exception cref="HeadlessException"> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		/// <exception cref="SecurityException"> if {@code accessSystemTray} permission
		/// is not granted </exception>
		/// <seealso cref= #add(TrayIcon) </seealso>
		/// <seealso cref= TrayIcon </seealso>
		/// <seealso cref= #isSupported </seealso>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= AWTPermission </seealso>
		public static SystemTray SystemTray
		{
			get
			{
				CheckSystemTrayAllowed();
				if (GraphicsEnvironment.Headless)
				{
					throw new HeadlessException();
				}
    
				InitializeSystemTrayIfNeeded();
    
				if (!Supported)
				{
					throw new UnsupportedOperationException("The system tray is not supported on the current platform.");
				}
    
				return SystemTray_Renamed;
			}
		}

		/// <summary>
		/// Returns whether the system tray is supported on the current
		/// platform.  In addition to displaying the tray icon, minimal
		/// system tray support includes either a popup menu (see {@link
		/// TrayIcon#setPopupMenu(PopupMenu)}) or an action event (see
		/// <seealso cref="TrayIcon#addActionListener(ActionListener)"/>).
		/// 
		/// <para>Developers should not assume that all of the system tray
		/// functionality is supported.  To guarantee that the tray icon's
		/// default action is always accessible, add the default action to
		/// both the action listener and the popup menu.  See the {@link
		/// SystemTray example} for an example of how to do this.
		/// 
		/// </para>
		/// <para><b>Note</b>: When implementing <code>SystemTray</code> and
		/// <code>TrayIcon</code> it is <em>strongly recommended</em> that
		/// you assign different gestures to the popup menu and an action
		/// event.  Overloading a gesture for both purposes is confusing
		/// and may prevent the user from accessing one or the other.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #getSystemTray </seealso>
		/// <returns> <code>false</code> if no system tray access is supported; this
		/// method returns <code>true</code> if the minimal system tray access is
		/// supported but does not guarantee that all system tray
		/// functionality is supported for the current platform </returns>
		public static bool Supported
		{
			get
			{
				Toolkit toolkit = Toolkit.DefaultToolkit;
				if (toolkit is SunToolkit)
				{
					// connecting tray to native resource
					InitializeSystemTrayIfNeeded();
					return ((SunToolkit)toolkit).TraySupported;
				}
				else if (toolkit is HeadlessToolkit)
				{
					// skip initialization as the init routine
					// throws HeadlessException
					return ((HeadlessToolkit)toolkit).TraySupported;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Adds a <code>TrayIcon</code> to the <code>SystemTray</code>.
		/// The tray icon becomes visible in the system tray once it is
		/// added.  The order in which icons are displayed in a tray is not
		/// specified - it is platform and implementation-dependent.
		/// 
		/// <para> All icons added by the application are automatically
		/// removed from the <code>SystemTray</code> upon application exit
		/// and also when the desktop system tray becomes unavailable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="trayIcon"> the <code>TrayIcon</code> to be added </param>
		/// <exception cref="NullPointerException"> if <code>trayIcon</code> is
		/// <code>null</code> </exception>
		/// <exception cref="IllegalArgumentException"> if the same instance of
		/// a <code>TrayIcon</code> is added more than once </exception>
		/// <exception cref="AWTException"> if the desktop system tray is missing </exception>
		/// <seealso cref= #remove(TrayIcon) </seealso>
		/// <seealso cref= #getSystemTray </seealso>
		/// <seealso cref= TrayIcon </seealso>
		/// <seealso cref= java.awt.Image </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(TrayIcon trayIcon) throws AWTException
		public virtual void Add(TrayIcon trayIcon)
		{
			if (trayIcon == null)
			{
				throw new NullPointerException("adding null TrayIcon");
			}
			TrayIcon[] oldArray = null, newArray = null;
			List<TrayIcon> icons = null;
			lock (this)
			{
				oldArray = SystemTray_Renamed.TrayIcons;
				icons = (List<TrayIcon>)AppContext.AppContext.get(typeof(TrayIcon));
				if (icons == null)
				{
					icons = new List<TrayIcon>(3);
					AppContext.AppContext.put(typeof(TrayIcon), icons);

				}
				else if (icons.Contains(trayIcon))
				{
					throw new IllegalArgumentException("adding TrayIcon that is already added");
				}
				icons.Add(trayIcon);
				newArray = SystemTray_Renamed.TrayIcons;

				trayIcon.ID = ++CurrentIconID;
			}
			try
			{
				trayIcon.AddNotify();
			}
			catch (AWTException e)
			{
				icons.Remove(trayIcon);
				throw e;
			}
			FirePropertyChange("trayIcons", oldArray, newArray);
		}

		/// <summary>
		/// Removes the specified <code>TrayIcon</code> from the
		/// <code>SystemTray</code>.
		/// 
		/// <para> All icons added by the application are automatically
		/// removed from the <code>SystemTray</code> upon application exit
		/// and also when the desktop system tray becomes unavailable.
		/// 
		/// </para>
		/// <para> If <code>trayIcon</code> is <code>null</code> or was not
		/// added to the system tray, no exception is thrown and no action
		/// is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="trayIcon"> the <code>TrayIcon</code> to be removed </param>
		/// <seealso cref= #add(TrayIcon) </seealso>
		/// <seealso cref= TrayIcon </seealso>
		public virtual void Remove(TrayIcon trayIcon)
		{
			if (trayIcon == null)
			{
				return;
			}
			TrayIcon[] oldArray = null, newArray = null;
			lock (this)
			{
				oldArray = SystemTray_Renamed.TrayIcons;
				List<TrayIcon> icons = (List<TrayIcon>)AppContext.AppContext.get(typeof(TrayIcon));
				// TrayIcon with no peer is not contained in the array.
				if (icons == null || !icons.Remove(trayIcon))
				{
					return;
				}
				trayIcon.RemoveNotify();
				newArray = SystemTray_Renamed.TrayIcons;
			}
			FirePropertyChange("trayIcons", oldArray, newArray);
		}

		/// <summary>
		/// Returns an array of all icons added to the tray by this
		/// application.  You can't access the icons added by another
		/// application.  Some browsers partition applets in different
		/// code bases into separate contexts, and establish walls between
		/// these contexts.  In such a scenario, only the tray icons added
		/// from this context will be returned.
		/// 
		/// <para> The returned array is a copy of the actual array and may be
		/// modified in any way without affecting the system tray.  To
		/// remove a <code>TrayIcon</code> from the
		/// <code>SystemTray</code>, use the {@link
		/// #remove(TrayIcon)} method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of all tray icons added to this tray, or an
		/// empty array if none has been added </returns>
		/// <seealso cref= #add(TrayIcon) </seealso>
		/// <seealso cref= TrayIcon </seealso>
		public virtual TrayIcon[] TrayIcons
		{
			get
			{
				List<TrayIcon> icons = (List<TrayIcon>)AppContext.AppContext.get(typeof(TrayIcon));
				if (icons != null)
				{
					return (TrayIcon[])icons.ToArray();
				}
				return EMPTY_TRAY_ARRAY;
			}
		}

		/// <summary>
		/// Returns the size, in pixels, of the space that a tray icon will
		/// occupy in the system tray.  Developers may use this methods to
		/// acquire the preferred size for the image property of a tray icon
		/// before it is created.  For convenience, there is a similar
		/// method <seealso cref="TrayIcon#getSize"/> in the <code>TrayIcon</code> class.
		/// </summary>
		/// <returns> the default size of a tray icon, in pixels </returns>
		/// <seealso cref= TrayIcon#setImageAutoSize(boolean) </seealso>
		/// <seealso cref= java.awt.Image </seealso>
		/// <seealso cref= TrayIcon#getSize() </seealso>
		public virtual Dimension TrayIconSize
		{
			get
			{
				return Peer.TrayIconSize;
			}
		}

		/// <summary>
		/// Adds a {@code PropertyChangeListener} to the list of listeners for the
		/// specific property. The following properties are currently supported:
		/// 
		/// <table border=1 summary="SystemTray properties">
		/// <tr>
		///    <th>Property</th>
		///    <th>Description</th>
		/// </tr>
		/// <tr>
		///    <td>{@code trayIcons}</td>
		///    <td>The {@code SystemTray}'s array of {@code TrayIcon} objects.
		///        The array is accessed via the <seealso cref="#getTrayIcons"/> method.<br>
		///        This property is changed when a tray icon is added to (or removed
		///        from) the system tray.<br> For example, this property is changed
		///        when the system tray becomes unavailable on the desktop<br>
		///        and the tray icons are automatically removed.</td>
		/// </tr>
		/// <tr>
		///    <td>{@code systemTray}</td>
		///    <td>This property contains {@code SystemTray} instance when the system tray
		///        is available or <code>null</code> otherwise.<br> This property is changed
		///        when the system tray becomes available or unavailable on the desktop.<br>
		///        The property is accessed by the <seealso cref="#getSystemTray"/> method.</td>
		/// </tr>
		/// </table>
		/// <para>
		/// The {@code listener} listens to property changes only in this context.
		/// </para>
		/// <para>
		/// If {@code listener} is {@code null}, no exception is thrown
		/// and no action is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName"> the specified property </param>
		/// <param name="listener"> the property change listener to be added
		/// </param>
		/// <seealso cref= #removePropertyChangeListener </seealso>
		/// <seealso cref= #getPropertyChangeListeners </seealso>
		public virtual void AddPropertyChangeListener(String propertyName, PropertyChangeListener listener)
		{
			lock (this)
			{
				if (listener == null)
				{
					return;
				}
				CurrentChangeSupport.AddPropertyChangeListener(propertyName, listener);
			}
		}

		/// <summary>
		/// Removes a {@code PropertyChangeListener} from the listener list
		/// for a specific property.
		/// <para>
		/// The {@code PropertyChangeListener} must be from this context.
		/// </para>
		/// <para>
		/// If {@code propertyName} or {@code listener} is {@code null} or invalid,
		/// no exception is thrown and no action is taken.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName"> the specified property </param>
		/// <param name="listener"> the PropertyChangeListener to be removed
		/// </param>
		/// <seealso cref= #addPropertyChangeListener </seealso>
		/// <seealso cref= #getPropertyChangeListeners </seealso>
		public virtual void RemovePropertyChangeListener(String propertyName, PropertyChangeListener listener)
		{
			lock (this)
			{
				if (listener == null)
				{
					return;
				}
				CurrentChangeSupport.RemovePropertyChangeListener(propertyName, listener);
			}
		}

		/// <summary>
		/// Returns an array of all the listeners that have been associated
		/// with the named property.
		/// <para>
		/// Only the listeners in this context are returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName"> the specified property </param>
		/// <returns> all of the {@code PropertyChangeListener}s associated with
		///         the named property; if no such listeners have been added or
		///         if {@code propertyName} is {@code null} or invalid, an empty
		///         array is returned
		/// </returns>
		/// <seealso cref= #addPropertyChangeListener </seealso>
		/// <seealso cref= #removePropertyChangeListener </seealso>
		public virtual PropertyChangeListener[] GetPropertyChangeListeners(String propertyName)
		{
			lock (this)
			{
				return CurrentChangeSupport.GetPropertyChangeListeners(propertyName);
			}
		}


		// ***************************************************************
		// ***************************************************************


		/// <summary>
		/// Support for reporting bound property changes for Object properties.
		/// This method can be called when a bound property has changed and it will
		/// send the appropriate PropertyChangeEvent to any registered
		/// PropertyChangeListeners.
		/// </summary>
		/// <param name="propertyName"> the property whose value has changed </param>
		/// <param name="oldValue"> the property's previous value </param>
		/// <param name="newValue"> the property's new value </param>
		private void FirePropertyChange(String propertyName, Object oldValue, Object newValue)
		{
			if (oldValue != null && newValue != null && oldValue.Equals(newValue))
			{
				return;
			}
			CurrentChangeSupport.FirePropertyChange(propertyName, oldValue, newValue);
		}

		/// <summary>
		/// Returns the current PropertyChangeSupport instance for the
		/// calling thread's context.
		/// </summary>
		/// <returns> this thread's context's PropertyChangeSupport </returns>
		private PropertyChangeSupport CurrentChangeSupport
		{
			get
			{
				lock (this)
				{
					PropertyChangeSupport changeSupport = (PropertyChangeSupport)AppContext.AppContext.get(typeof(SystemTray));
            
					if (changeSupport == null)
					{
						changeSupport = new PropertyChangeSupport(this);
						AppContext.AppContext.put(typeof(SystemTray), changeSupport);
					}
					return changeSupport;
				}
			}
		}

		internal virtual void AddNotify()
		{
			lock (this)
			{
				if (Peer == null)
				{
					Toolkit toolkit = Toolkit.DefaultToolkit;
					if (toolkit is SunToolkit)
					{
						Peer = ((SunToolkit)Toolkit.DefaultToolkit).createSystemTray(this);
					}
					else if (toolkit is HeadlessToolkit)
					{
						Peer = ((HeadlessToolkit)Toolkit.DefaultToolkit).createSystemTray(this);
					}
				}
			}
		}

		internal static void CheckSystemTrayAllowed()
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckPermission(SecurityConstants.AWT.ACCESS_SYSTEM_TRAY_PERMISSION);
			}
		}

		private static void InitializeSystemTrayIfNeeded()
		{
			lock (typeof(SystemTray))
			{
				if (SystemTray_Renamed == null)
				{
					SystemTray_Renamed = new SystemTray();
				}
			}
		}
	}

}