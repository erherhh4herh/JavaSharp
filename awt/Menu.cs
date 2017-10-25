using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
namespace java.awt
{

	using AWTAccessor = sun.awt.AWTAccessor;

	/// <summary>
	/// A <code>Menu</code> object is a pull-down menu component
	/// that is deployed from a menu bar.
	/// <para>
	/// A menu can optionally be a <i>tear-off</i> menu. A tear-off menu
	/// can be opened and dragged away from its parent menu bar or menu.
	/// It remains on the screen after the mouse button has been released.
	/// The mechanism for tearing off a menu is platform dependent, since
	/// the look and feel of the tear-off menu is determined by its peer.
	/// On platforms that do not support tear-off menus, the tear-off
	/// property is ignored.
	/// </para>
	/// <para>
	/// Each item in a menu must belong to the <code>MenuItem</code>
	/// class. It can be an instance of <code>MenuItem</code>, a submenu
	/// (an instance of <code>Menu</code>), or a check box (an instance of
	/// <code>CheckboxMenuItem</code>).
	/// 
	/// @author Sami Shaio
	/// </para>
	/// </summary>
	/// <seealso cref=     java.awt.MenuItem </seealso>
	/// <seealso cref=     java.awt.CheckboxMenuItem
	/// @since   JDK1.0 </seealso>
	public class Menu : MenuItem, MenuContainer, Accessible
	{

		static Menu()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}

			AWTAccessor.MenuAccessor = new MenuAccessorAnonymousInnerClassHelper();
		}

		private class MenuAccessorAnonymousInnerClassHelper : AWTAccessor.MenuAccessor
		{
			public MenuAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual List<MenuComponent> GetItems(Menu menu)
			{
				return menu.Items;
			}
		}

		/// <summary>
		/// A vector of the items that will be part of the Menu.
		/// 
		/// @serial </summary>
		/// <seealso cref= #countItems() </seealso>
		internal List<MenuComponent> Items = new List<MenuComponent>();

		/// <summary>
		/// This field indicates whether the menu has the
		/// tear of property or not.  It will be set to
		/// <code>true</code> if the menu has the tear off
		/// property and it will be set to <code>false</code>
		/// if it does not.
		/// A torn off menu can be deleted by a user when
		/// it is no longer needed.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isTearOff() </seealso>
		internal bool TearOff_Renamed;

		/// <summary>
		/// This field will be set to <code>true</code>
		/// if the Menu in question is actually a help
		/// menu.  Otherwise it will be set to <code>
		/// false</code>.
		/// 
		/// @serial
		/// </summary>
		internal bool IsHelpMenu;

		private const String @base = "menu";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = -8809584163345499784L;

		/// <summary>
		/// Constructs a new menu with an empty label. This menu is not
		/// a tear-off menu. </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since      JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Menu() throws HeadlessException
		public Menu() : this("", false)
		{
		}

		/// <summary>
		/// Constructs a new menu with the specified label. This menu is not
		/// a tear-off menu. </summary>
		/// <param name="label"> the menu's label in the menu bar, or in
		///                   another menu of which this menu is a submenu. </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Menu(String label) throws HeadlessException
		public Menu(String label) : this(label, false)
		{
		}

		/// <summary>
		/// Constructs a new menu with the specified label,
		/// indicating whether the menu can be torn off.
		/// <para>
		/// Tear-off functionality may not be supported by all
		/// implementations of AWT.  If a particular implementation doesn't
		/// support tear-off menus, this value is silently ignored.
		/// </para>
		/// </summary>
		/// <param name="label"> the menu's label in the menu bar, or in
		///                   another menu of which this menu is a submenu. </param>
		/// <param name="tearOff">   if <code>true</code>, the menu
		///                   is a tear-off menu. </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since       JDK1.0. </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Menu(String label, boolean tearOff) throws HeadlessException
		public Menu(String label, bool tearOff) : base(label)
		{
			this.TearOff_Renamed = tearOff;
		}

		/// <summary>
		/// Construct a name for this MenuComponent.  Called by getName() when
		/// the name is null.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(Menu))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the menu's peer.  The peer allows us to modify the
		/// appearance of the menu without changing its functionality.
		/// </summary>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.DefaultToolkit.CreateMenu(this);
				}
				int nitems = ItemCount;
				for (int i = 0 ; i < nitems ; i++)
				{
					MenuItem mi = GetItem(i);
					mi.Parent_Renamed = this;
					mi.AddNotify();
				}
			}
		}

		/// <summary>
		/// Removes the menu's peer.  The peer allows us to modify the appearance
		/// of the menu without changing its functionality.
		/// </summary>
		public override void RemoveNotify()
		{
			lock (TreeLock)
			{
				int nitems = ItemCount;
				for (int i = 0 ; i < nitems ; i++)
				{
					GetItem(i).RemoveNotify();
				}
				base.RemoveNotify();
			}
		}

		/// <summary>
		/// Indicates whether this menu is a tear-off menu.
		/// <para>
		/// Tear-off functionality may not be supported by all
		/// implementations of AWT.  If a particular implementation doesn't
		/// support tear-off menus, this value is silently ignored.
		/// </para>
		/// </summary>
		/// <returns>      <code>true</code> if this is a tear-off menu;
		///                         <code>false</code> otherwise. </returns>
		public virtual bool TearOff
		{
			get
			{
				return TearOff_Renamed;
			}
		}

		/// <summary>
		/// Get the number of items in this menu. </summary>
		/// <returns>     the number of items in this menu.
		/// @since      JDK1.1 </returns>
		public virtual int ItemCount
		{
			get
			{
				return CountItems();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getItemCount()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int CountItems()
		{
			return CountItemsImpl();
		}

		/*
		 * This is called by the native code, so client code can't
		 * be called on the toolkit thread.
		 */
		internal int CountItemsImpl()
		{
			return Items.Count;
		}

		/// <summary>
		/// Gets the item located at the specified index of this menu. </summary>
		/// <param name="index"> the position of the item to be returned. </param>
		/// <returns>    the item located at the specified index. </returns>
		public virtual MenuItem GetItem(int index)
		{
			return GetItemImpl(index);
		}

		/*
		 * This is called by the native code, so client code can't
		 * be called on the toolkit thread.
		 */
		internal MenuItem GetItemImpl(int index)
		{
			return (MenuItem)Items[index];
		}

		/// <summary>
		/// Adds the specified menu item to this menu. If the
		/// menu item has been part of another menu, removes it
		/// from that menu.
		/// </summary>
		/// <param name="mi">   the menu item to be added </param>
		/// <returns>      the menu item added </returns>
		/// <seealso cref=         java.awt.Menu#insert(java.lang.String, int) </seealso>
		/// <seealso cref=         java.awt.Menu#insert(java.awt.MenuItem, int) </seealso>
		public virtual MenuItem Add(MenuItem mi)
		{
			lock (TreeLock)
			{
				if (mi.Parent_Renamed != null)
				{
					mi.Parent_Renamed.remove(mi);
				}
				Items.Add(mi);
				mi.Parent_Renamed = this;
				MenuPeer peer = (MenuPeer)this.Peer_Renamed;
				if (peer != null)
				{
					mi.AddNotify();
					peer.AddItem(mi);
				}
				return mi;
			}
		}

		/// <summary>
		/// Adds an item with the specified label to this menu.
		/// </summary>
		/// <param name="label">   the text on the item </param>
		/// <seealso cref=         java.awt.Menu#insert(java.lang.String, int) </seealso>
		/// <seealso cref=         java.awt.Menu#insert(java.awt.MenuItem, int) </seealso>
		public virtual void Add(String label)
		{
			Add(new MenuItem(label));
		}

		/// <summary>
		/// Inserts a menu item into this menu
		/// at the specified position.
		/// </summary>
		/// <param name="menuitem">  the menu item to be inserted. </param>
		/// <param name="index">     the position at which the menu
		///                          item should be inserted. </param>
		/// <seealso cref=           java.awt.Menu#add(java.lang.String) </seealso>
		/// <seealso cref=           java.awt.Menu#add(java.awt.MenuItem) </seealso>
		/// <exception cref="IllegalArgumentException"> if the value of
		///                    <code>index</code> is less than zero
		/// @since         JDK1.1 </exception>

		public virtual void Insert(MenuItem menuitem, int index)
		{
			lock (TreeLock)
			{
				if (index < 0)
				{
					throw new IllegalArgumentException("index less than zero.");
				}

				int nitems = ItemCount;
				List<MenuItem> tempItems = new List<MenuItem>();

				/* Remove the item at index, nitems-index times
				   storing them in a temporary vector in the
				   order they appear on the menu.
				*/
				for (int i = index ; i < nitems; i++)
				{
					tempItems.Add(GetItem(index));
					Remove(index);
				}

				Add(menuitem);

				/* Add the removed items back to the menu, they are
				   already in the correct order in the temp vector.
				*/
				for (int i = 0; i < tempItems.Count ; i++)
				{
					Add(tempItems[i]);
				}
			}
		}

		/// <summary>
		/// Inserts a menu item with the specified label into this menu
		/// at the specified position.  This is a convenience method for
		/// <code>insert(menuItem, index)</code>.
		/// </summary>
		/// <param name="label"> the text on the item </param>
		/// <param name="index"> the position at which the menu item
		///                      should be inserted </param>
		/// <seealso cref=         java.awt.Menu#add(java.lang.String) </seealso>
		/// <seealso cref=         java.awt.Menu#add(java.awt.MenuItem) </seealso>
		/// <exception cref="IllegalArgumentException"> if the value of
		///                    <code>index</code> is less than zero
		/// @since       JDK1.1 </exception>

		public virtual void Insert(String label, int index)
		{
			Insert(new MenuItem(label), index);
		}

		/// <summary>
		/// Adds a separator line, or a hypen, to the menu at the current position. </summary>
		/// <seealso cref=         java.awt.Menu#insertSeparator(int) </seealso>
		public virtual void AddSeparator()
		{
			Add("-");
		}

		/// <summary>
		/// Inserts a separator at the specified position. </summary>
		/// <param name="index"> the position at which the
		///                       menu separator should be inserted. </param>
		/// <exception cref="IllegalArgumentException"> if the value of
		///                       <code>index</code> is less than 0. </exception>
		/// <seealso cref=         java.awt.Menu#addSeparator
		/// @since       JDK1.1 </seealso>

		public virtual void InsertSeparator(int index)
		{
			lock (TreeLock)
			{
				if (index < 0)
				{
					throw new IllegalArgumentException("index less than zero.");
				}

				int nitems = ItemCount;
				List<MenuItem> tempItems = new List<MenuItem>();

				/* Remove the item at index, nitems-index times
				   storing them in a temporary vector in the
				   order they appear on the menu.
				*/
				for (int i = index ; i < nitems; i++)
				{
					tempItems.Add(GetItem(index));
					Remove(index);
				}

				AddSeparator();

				/* Add the removed items back to the menu, they are
				   already in the correct order in the temp vector.
				*/
				for (int i = 0; i < tempItems.Count ; i++)
				{
					Add(tempItems[i]);
				}
			}
		}

		/// <summary>
		/// Removes the menu item at the specified index from this menu. </summary>
		/// <param name="index"> the position of the item to be removed. </param>
		public virtual void Remove(int index)
		{
			lock (TreeLock)
			{
				MenuItem mi = GetItem(index);
				Items.RemoveAt(index);
				MenuPeer peer = (MenuPeer)this.Peer_Renamed;
				if (peer != null)
				{
					mi.RemoveNotify();
					mi.Parent_Renamed = null;
					peer.DelItem(index);
				}
			}
		}

		/// <summary>
		/// Removes the specified menu item from this menu. </summary>
		/// <param name="item"> the item to be removed from the menu.
		///         If <code>item</code> is <code>null</code>
		///         or is not in this menu, this method does
		///         nothing. </param>
		public virtual void Remove(MenuComponent item)
		{
			lock (TreeLock)
			{
				int index = Items.IndexOf(item);
				if (index >= 0)
				{
					Remove(index);
				}
			}
		}

		/// <summary>
		/// Removes all items from this menu.
		/// @since       JDK1.0.
		/// </summary>
		public virtual void RemoveAll()
		{
			lock (TreeLock)
			{
				int nitems = ItemCount;
				for (int i = nitems - 1 ; i >= 0 ; i--)
				{
					Remove(i);
				}
			}
		}

		/*
		 * Post an ActionEvent to the target of the MenuPeer
		 * associated with the specified keyboard event (on
		 * keydown).  Returns true if there is an associated
		 * keyboard event.
		 */
		internal override bool HandleShortcut(KeyEvent e)
		{
			int nitems = ItemCount;
			for (int i = 0 ; i < nitems ; i++)
			{
				MenuItem mi = GetItem(i);
				if (mi.HandleShortcut(e))
				{
					return true;
				}
			}
			return false;
		}

		internal override MenuItem GetShortcutMenuItem(MenuShortcut s)
		{
			int nitems = ItemCount;
			for (int i = 0 ; i < nitems ; i++)
			{
				MenuItem mi = GetItem(i).GetShortcutMenuItem(s);
				if (mi != null)
				{
					return mi;
				}
			}
			return null;
		}

		internal virtual IEnumerator<MenuShortcut> Shortcuts()
		{
			lock (this)
			{
				List<MenuShortcut> shortcuts = new List<MenuShortcut>();
				int nitems = ItemCount;
				for (int i = 0 ; i < nitems ; i++)
				{
					MenuItem mi = GetItem(i);
					if (mi is Menu)
					{
						IEnumerator<MenuShortcut> e = ((Menu)mi).Shortcuts();
						while (e.MoveNext())
						{
							shortcuts.Add(e.Current);
						}
					}
					else
					{
						MenuShortcut ms = mi.Shortcut;
						if (ms != null)
						{
							shortcuts.Add(ms);
						}
					}
				}
				return shortcuts.elements();
			}
		}

		internal override void DeleteShortcut(MenuShortcut s)
		{
			int nitems = ItemCount;
			for (int i = 0 ; i < nitems ; i++)
			{
				GetItem(i).DeleteShortcut(s);
			}
		}


		/* Serialization support.  A MenuContainer is responsible for
		 * restoring the parent fields of its children.
		 */

		/// <summary>
		/// The menu serialized Data Version.
		/// 
		/// @serial
		/// </summary>
		private int MenuSerializedDataVersion = 1;

		/// <summary>
		/// Writes default serializable fields to stream.
		/// </summary>
		/// <param name="s"> the <code>ObjectOutputStream</code> to write </param>
		/// <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		/// <seealso cref= #readObject(ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
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
		/// <seealso cref= #writeObject(ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
		  // HeadlessException will be thrown from MenuComponent's readObject
		  s.DefaultReadObject();
		  for (int i = 0; i < Items.Count; i++)
		  {
			MenuItem item = (MenuItem)Items[i];
			item.Parent_Renamed = this;
		  }
		}

		/// <summary>
		/// Returns a string representing the state of this <code>Menu</code>.
		/// This method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns> the parameter string of this menu </returns>
		public override String ParamString()
		{
			String str = ",tearOff=" + TearOff_Renamed + ",isHelpMenu=" + IsHelpMenu;
			return base.ParamString() + str;
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
		/// Gets the AccessibleContext associated with this Menu.
		/// For menus, the AccessibleContext takes the form of an
		/// AccessibleAWTMenu.
		/// A new AccessibleAWTMenu instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTMenu that serves as the
		///         AccessibleContext of this Menu
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTMenu(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// Defined in MenuComponent. Overridden here.
		/// </summary>
		internal override int GetAccessibleChildIndex(MenuComponent child)
		{
			return Items.IndexOf(child);
		}

		/// <summary>
		/// Inner class of Menu used to provide default support for
		/// accessibility.  This class is not meant to be used directly by
		/// application developers, but is instead meant only to be
		/// subclassed by menu component developers.
		/// <para>
		/// This class implements accessibility support for the
		/// <code>Menu</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to menu user-interface elements.
		/// @since 1.3
		/// </para>
		/// </summary>
		protected internal class AccessibleAWTMenu : AccessibleAWTMenuItem
		{
			private readonly Menu OuterInstance;

			public AccessibleAWTMenu(Menu outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = 5228160894980069094L;

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object </returns>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.MENU;
				}
			}

		} // class AccessibleAWTMenu

	}

}