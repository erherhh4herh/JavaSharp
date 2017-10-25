using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2015, Oracle and/or its affiliates. All rights reserved.
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

	using AWTAccessor = sun.awt.AWTAccessor;

	/// <summary>
	/// The <code>MenuBar</code> class encapsulates the platform's
	/// concept of a menu bar bound to a frame. In order to associate
	/// the menu bar with a <code>Frame</code> object, call the
	/// frame's <code>setMenuBar</code> method.
	/// <para>
	/// <A NAME="mbexample"></A><!-- target for cross references -->
	/// This is what a menu bar might look like:
	/// </para>
	/// <para>
	/// <img src="doc-files/MenuBar-1.gif"
	/// alt="Diagram of MenuBar containing 2 menus: Examples and Options.
	/// Examples menu is expanded showing items: Basic, Simple, Check, and More Examples."
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// A menu bar handles keyboard shortcuts for menu items, passing them
	/// along to its child menus.
	/// (Keyboard shortcuts, which are optional, provide the user with
	/// an alternative to the mouse for invoking a menu item and the
	/// action that is associated with it.)
	/// Each menu item can maintain an instance of <code>MenuShortcut</code>.
	/// The <code>MenuBar</code> class defines several methods,
	/// <seealso cref="MenuBar#shortcuts"/> and
	/// <seealso cref="MenuBar#getShortcutMenuItem"/>
	/// that retrieve information about the shortcuts a given
	/// menu bar is managing.
	/// 
	/// @author Sami Shaio
	/// </para>
	/// </summary>
	/// <seealso cref=        java.awt.Frame </seealso>
	/// <seealso cref=        java.awt.Frame#setMenuBar(java.awt.MenuBar) </seealso>
	/// <seealso cref=        java.awt.Menu </seealso>
	/// <seealso cref=        java.awt.MenuItem </seealso>
	/// <seealso cref=        java.awt.MenuShortcut
	/// @since      JDK1.0 </seealso>
	public class MenuBar : MenuComponent, MenuContainer, Accessible
	{

		static MenuBar()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
			AWTAccessor.MenuBarAccessor = new MenuBarAccessorAnonymousInnerClassHelper();
		}

		private class MenuBarAccessorAnonymousInnerClassHelper : AWTAccessor.MenuBarAccessor
		{
			public MenuBarAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual Menu GetHelpMenu(MenuBar menuBar)
			{
				return menuBar.HelpMenu_Renamed;
			}

			public virtual List<Menu> GetMenus(MenuBar menuBar)
			{
				return menuBar.Menus;
			}
		}

		/// <summary>
		/// This field represents a vector of the
		/// actual menus that will be part of the MenuBar.
		/// 
		/// @serial </summary>
		/// <seealso cref= #countMenus() </seealso>
		internal List<Menu> Menus = new List<Menu>();

		/// <summary>
		/// This menu is a special menu dedicated to
		/// help.  The one thing to note about this menu
		/// is that on some platforms it appears at the
		/// right edge of the menubar.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getHelpMenu() </seealso>
		/// <seealso cref= #setHelpMenu(Menu) </seealso>
		internal Menu HelpMenu_Renamed;

		private const String @base = "menubar";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = -4930327919388951260L;

		/// <summary>
		/// Creates a new menu bar. </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MenuBar() throws HeadlessException
		public MenuBar()
		{
		}

		/// <summary>
		/// Construct a name for this MenuComponent.  Called by getName() when
		/// the name is null.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(MenuBar))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the menu bar's peer.  The peer allows us to change the
		/// appearance of the menu bar without changing any of the menu bar's
		/// functionality.
		/// </summary>
		public virtual void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.DefaultToolkit.CreateMenuBar(this);
				}

				int nmenus = MenuCount;
				for (int i = 0 ; i < nmenus ; i++)
				{
					GetMenu(i).AddNotify();
				}
			}
		}

		/// <summary>
		/// Removes the menu bar's peer.  The peer allows us to change the
		/// appearance of the menu bar without changing any of the menu bar's
		/// functionality.
		/// </summary>
		public override void RemoveNotify()
		{
			lock (TreeLock)
			{
				int nmenus = MenuCount;
				for (int i = 0 ; i < nmenus ; i++)
				{
					GetMenu(i).RemoveNotify();
				}
				base.RemoveNotify();
			}
		}

		/// <summary>
		/// Gets the help menu on the menu bar. </summary>
		/// <returns>    the help menu on this menu bar. </returns>
		public virtual Menu HelpMenu
		{
			get
			{
				return HelpMenu_Renamed;
			}
			set
			{
				lock (TreeLock)
				{
					if (HelpMenu_Renamed == value)
					{
						return;
					}
					if (HelpMenu_Renamed != null)
					{
						Remove(HelpMenu_Renamed);
					}
					HelpMenu_Renamed = value;
					if (value != null)
					{
						if (value.Parent_Renamed != this)
						{
							Add(value);
						}
						value.IsHelpMenu = true;
						value.Parent_Renamed = this;
						MenuBarPeer peer = (MenuBarPeer)this.Peer_Renamed;
						if (peer != null)
						{
							if (value.Peer_Renamed == null)
							{
								value.AddNotify();
							}
							peer.AddHelpMenu(value);
						}
					}
				}
			}
		}


		/// <summary>
		/// Adds the specified menu to the menu bar.
		/// If the menu has been part of another menu bar,
		/// removes it from that menu bar.
		/// </summary>
		/// <param name="m">   the menu to be added </param>
		/// <returns>       the menu added </returns>
		/// <seealso cref=          java.awt.MenuBar#remove(int) </seealso>
		/// <seealso cref=          java.awt.MenuBar#remove(java.awt.MenuComponent) </seealso>
		public virtual Menu Add(Menu m)
		{
			lock (TreeLock)
			{
				if (m.Parent_Renamed != null)
				{
					m.Parent_Renamed.remove(m);
				}
				Menus.Add(m);
				m.Parent_Renamed = this;

				MenuBarPeer peer = (MenuBarPeer)this.Peer_Renamed;
				if (peer != null)
				{
					if (m.Peer_Renamed == null)
					{
						m.AddNotify();
					}
					peer.AddMenu(m);
				}
				return m;
			}
		}

		/// <summary>
		/// Removes the menu located at the specified
		/// index from this menu bar. </summary>
		/// <param name="index">   the position of the menu to be removed. </param>
		/// <seealso cref=          java.awt.MenuBar#add(java.awt.Menu) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void remove(final int index)
		public virtual void Remove(int index)
		{
			lock (TreeLock)
			{
				Menu m = GetMenu(index);
				Menus.RemoveAt(index);
				MenuBarPeer peer = (MenuBarPeer)this.Peer_Renamed;
				if (peer != null)
				{
					m.RemoveNotify();
					m.Parent_Renamed = null;
					peer.DelMenu(index);
				}
				if (HelpMenu_Renamed == m)
				{
					HelpMenu_Renamed = null;
					m.IsHelpMenu = false;
				}
			}
		}

		/// <summary>
		/// Removes the specified menu component from this menu bar. </summary>
		/// <param name="m"> the menu component to be removed. </param>
		/// <seealso cref=          java.awt.MenuBar#add(java.awt.Menu) </seealso>
		public virtual void Remove(MenuComponent m)
		{
			lock (TreeLock)
			{
				int index = Menus.IndexOf(m);
				if (index >= 0)
				{
					Remove(index);
				}
			}
		}

		/// <summary>
		/// Gets the number of menus on the menu bar. </summary>
		/// <returns>     the number of menus on the menu bar.
		/// @since      JDK1.1 </returns>
		public virtual int MenuCount
		{
			get
			{
				return CountMenus();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getMenuCount()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int CountMenus()
		{
			return MenuCountImpl;
		}

		/*
		 * This is called by the native code, so client code can't
		 * be called on the toolkit thread.
		 */
		internal int MenuCountImpl
		{
			get
			{
				return Menus.Count;
			}
		}

		/// <summary>
		/// Gets the specified menu. </summary>
		/// <param name="i"> the index position of the menu to be returned. </param>
		/// <returns>     the menu at the specified index of this menu bar. </returns>
		public virtual Menu GetMenu(int i)
		{
			return GetMenuImpl(i);
		}

		/*
		 * This is called by the native code, so client code can't
		 * be called on the toolkit thread.
		 */
		internal Menu GetMenuImpl(int i)
		{
			return Menus[i];
		}

		/// <summary>
		/// Gets an enumeration of all menu shortcuts this menu bar
		/// is managing. </summary>
		/// <returns>      an enumeration of menu shortcuts that this
		///                      menu bar is managing. </returns>
		/// <seealso cref=         java.awt.MenuShortcut
		/// @since       JDK1.1 </seealso>
		public virtual IEnumerator<MenuShortcut> Shortcuts()
		{
			lock (this)
			{
				List<MenuShortcut> shortcuts = new List<MenuShortcut>();
				int nmenus = MenuCount;
				for (int i = 0 ; i < nmenus ; i++)
				{
					IEnumerator<MenuShortcut> e = GetMenu(i).Shortcuts();
					while (e.MoveNext())
					{
						shortcuts.Add(e.Current);
					}
				}
				return shortcuts.elements();
			}
		}

		/// <summary>
		/// Gets the instance of <code>MenuItem</code> associated
		/// with the specified <code>MenuShortcut</code> object,
		/// or <code>null</code> if none of the menu items being managed
		/// by this menu bar is associated with the specified menu
		/// shortcut. </summary>
		/// <param name="s"> the specified menu shortcut. </param>
		/// <seealso cref=          java.awt.MenuItem </seealso>
		/// <seealso cref=          java.awt.MenuShortcut
		/// @since        JDK1.1 </seealso>
		 public virtual MenuItem GetShortcutMenuItem(MenuShortcut s)
		 {
			int nmenus = MenuCount;
			for (int i = 0 ; i < nmenus ; i++)
			{
				MenuItem mi = GetMenu(i).GetShortcutMenuItem(s);
				if (mi != null)
				{
					return mi;
				}
			}
			return null; // MenuShortcut wasn't found
		 }

		/*
		 * Post an ACTION_EVENT to the target of the MenuPeer
		 * associated with the specified keyboard event (on
		 * keydown).  Returns true if there is an associated
		 * keyboard event.
		 */
		internal virtual bool HandleShortcut(KeyEvent e)
		{
			// Is it a key event?
			int id = e.ID;
			if (id != KeyEvent.KEY_PRESSED && id != KeyEvent.KEY_RELEASED)
			{
				return false;
			}

			// Is the accelerator modifier key pressed?
			int accelKey = Toolkit.DefaultToolkit.MenuShortcutKeyMask;
			if ((e.Modifiers & accelKey) == 0)
			{
				return false;
			}

			// Pass MenuShortcut on to child menus.
			int nmenus = MenuCount;
			for (int i = 0 ; i < nmenus ; i++)
			{
				Menu m = GetMenu(i);
				if (m.HandleShortcut(e))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Deletes the specified menu shortcut. </summary>
		/// <param name="s"> the menu shortcut to delete.
		/// @since     JDK1.1 </param>
		public virtual void DeleteShortcut(MenuShortcut s)
		{
			int nmenus = MenuCount;
			for (int i = 0 ; i < nmenus ; i++)
			{
				GetMenu(i).DeleteShortcut(s);
			}
		}

		/* Serialization support.  Restore the (transient) parent
		 * fields of Menubar menus here.
		 */

		/// <summary>
		/// The MenuBar's serialized data version.
		/// 
		/// @serial
		/// </summary>
		private int MenuBarSerializedDataVersion = 1;

		/// <summary>
		/// Writes default serializable fields to stream.
		/// </summary>
		/// <param name="s"> the <code>ObjectOutputStream</code> to write </param>
		/// <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		/// <seealso cref= #readObject(java.io.ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
		  s.DefaultWriteObject();
		}

		/// <summary>
		/// Reads the <code>ObjectInputStream</code>.
		/// Unrecognized keys or values will be ignored.
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to read </param>
		/// <exception cref="HeadlessException"> if
		///   <code>GraphicsEnvironment.isHeadless</code> returns
		///   <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= #writeObject(java.io.ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
		  // HeadlessException will be thrown from MenuComponent's readObject
		  s.DefaultReadObject();
		  for (int i = 0; i < Menus.Count; i++)
		  {
			Menu m = Menus[i];
			m.Parent_Renamed = this;
		  }
		}

		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();


	/////////////////
	// Accessibility support
	////////////////

		/// <summary>
		/// Gets the AccessibleContext associated with this MenuBar.
		/// For menu bars, the AccessibleContext takes the form of an
		/// AccessibleAWTMenuBar.
		/// A new AccessibleAWTMenuBar instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTMenuBar that serves as the
		///         AccessibleContext of this MenuBar
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTMenuBar(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// Defined in MenuComponent. Overridden here.
		/// </summary>
		internal override int GetAccessibleChildIndex(MenuComponent child)
		{
			return Menus.IndexOf(child);
		}

		/// <summary>
		/// Inner class of MenuBar used to provide default support for
		/// accessibility.  This class is not meant to be used directly by
		/// application developers, but is instead meant only to be
		/// subclassed by menu component developers.
		/// <para>
		/// This class implements accessibility support for the
		/// <code>MenuBar</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to menu bar user-interface elements.
		/// @since 1.3
		/// </para>
		/// </summary>
		protected internal class AccessibleAWTMenuBar : AccessibleAWTMenuComponent
		{
			private readonly MenuBar OuterInstance;

			public AccessibleAWTMenuBar(MenuBar outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = -8577604491830083815L;

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object
			/// @since 1.4 </returns>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.MENU_BAR;
				}
			}

		} // class AccessibleAWTMenuBar

	}

}