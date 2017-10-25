using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A class that implements a menu which can be dynamically popped up
	/// at a specified position within a component.
	/// <para>
	/// As the inheritance hierarchy implies, a <code>PopupMenu</code>
	///  can be used anywhere a <code>Menu</code> can be used.
	/// However, if you use a <code>PopupMenu</code> like a <code>Menu</code>
	/// (e.g., you add it to a <code>MenuBar</code>), then you <b>cannot</b>
	/// call <code>show</code> on that <code>PopupMenu</code>.
	/// 
	/// @author      Amy Fowler
	/// </para>
	/// </summary>
	public class PopupMenu : Menu
	{

		private const String @base = "popup";
		internal static int NameCounter = 0;

		[NonSerialized]
		internal bool IsTrayIconPopup = false;

		static PopupMenu()
		{
			AWTAccessor.PopupMenuAccessor = new PopupMenuAccessorAnonymousInnerClassHelper();
		}

		private class PopupMenuAccessorAnonymousInnerClassHelper : AWTAccessor.PopupMenuAccessor
		{
			public PopupMenuAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual bool IsTrayIconPopup(PopupMenu popupMenu)
			{
				return popupMenu.IsTrayIconPopup;
			}
		}

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -4620452533522760060L;

		/// <summary>
		/// Creates a new popup menu with an empty name. </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PopupMenu() throws HeadlessException
		public PopupMenu() : this("")
		{
		}

		/// <summary>
		/// Creates a new popup menu with the specified name.
		/// </summary>
		/// <param name="label"> a non-<code>null</code> string specifying
		///                the popup menu's label </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PopupMenu(String label) throws HeadlessException
		public PopupMenu(String label) : base(label)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override MenuContainer Parent
		{
			get
			{
				if (IsTrayIconPopup)
				{
					return null;
				}
				return base.Parent;
			}
		}

		/// <summary>
		/// Constructs a name for this <code>MenuComponent</code>.
		/// Called by <code>getName</code> when the name is <code>null</code>.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(PopupMenu))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the popup menu's peer.
		/// The peer allows us to change the appearance of the popup menu without
		/// changing any of the popup menu's functionality.
		/// </summary>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				// If our parent is not a Component, then this PopupMenu is
				// really just a plain, old Menu.
				if (Parent_Renamed != null && !(Parent_Renamed is Component))
				{
					base.AddNotify();
				}
				else
				{
					if (Peer_Renamed == null)
					{
						Peer_Renamed = Toolkit.DefaultToolkit.CreatePopupMenu(this);
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
		}

	   /// <summary>
	   /// Shows the popup menu at the x, y position relative to an origin
	   /// component.
	   /// The origin component must be contained within the component
	   /// hierarchy of the popup menu's parent.  Both the origin and the parent
	   /// must be showing on the screen for this method to be valid.
	   /// <para>
	   /// If this <code>PopupMenu</code> is being used as a <code>Menu</code>
	   /// (i.e., it has a non-<code>Component</code> parent),
	   /// then you cannot call this method on the <code>PopupMenu</code>.
	   ///  
	   /// </para>
	   /// </summary>
	   /// <param name="origin"> the component which defines the coordinate space </param>
	   /// <param name="x"> the x coordinate position to popup the menu </param>
	   /// <param name="y"> the y coordinate position to popup the menu </param>
	   /// <exception cref="NullPointerException">  if the parent is <code>null</code> </exception>
	   /// <exception cref="IllegalArgumentException">  if this <code>PopupMenu</code>
	   ///                has a non-<code>Component</code> parent </exception>
	   /// <exception cref="IllegalArgumentException"> if the origin is not in the
	   ///                parent's hierarchy </exception>
	   /// <exception cref="RuntimeException"> if the parent is not showing on screen </exception>
		public virtual void Show(Component origin, int x, int y)
		{
			// Use localParent for thread safety.
			MenuContainer localParent = Parent_Renamed;
			if (localParent == null)
			{
				throw new NullPointerException("parent is null");
			}
			if (!(localParent is Component))
			{
				throw new IllegalArgumentException("PopupMenus with non-Component parents cannot be shown");
			}
			Component compParent = (Component)localParent;
			//Fixed 6278745: Incorrect exception throwing in PopupMenu.show() method
			//Exception was not thrown if compParent was not equal to origin and
			//was not Container
			if (compParent != origin)
			{
				if (compParent is Container)
				{
					if (!((Container)compParent).IsAncestorOf(origin))
					{
						throw new IllegalArgumentException("origin not in parent's hierarchy");
					}
				}
				else
				{
					throw new IllegalArgumentException("origin not in parent's hierarchy");
				}
			}
			if (compParent.Peer == null || !compParent.Showing)
			{
				throw new RuntimeException("parent not showing on screen");
			}
			if (Peer_Renamed == null)
			{
				AddNotify();
			}
			lock (TreeLock)
			{
				if (Peer_Renamed != null)
				{
					((PopupMenuPeer)Peer_Renamed).Show(new Event(origin, 0, Event.MOUSE_DOWN, x, y, 0, 0));
				}
			}
		}


	/////////////////
	// Accessibility support
	////////////////

		/// <summary>
		/// Gets the <code>AccessibleContext</code> associated with this
		/// <code>PopupMenu</code>.
		/// </summary>
		/// <returns> the <code>AccessibleContext</code> of this
		///                <code>PopupMenu</code>
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTPopupMenu(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// Inner class of PopupMenu used to provide default support for
		/// accessibility.  This class is not meant to be used directly by
		/// application developers, but is instead meant only to be
		/// subclassed by menu component developers.
		/// <para>
		/// The class used to obtain the accessible role for this object.
		/// @since 1.3
		/// </para>
		/// </summary>
		protected internal class AccessibleAWTPopupMenu : AccessibleAWTMenu
		{
			private readonly PopupMenu OuterInstance;

			public AccessibleAWTPopupMenu(PopupMenu outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = -4282044795947239955L;

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object </returns>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.POPUP_MENU;
				}
			}

		} // class AccessibleAWTPopupMenu

	}

}