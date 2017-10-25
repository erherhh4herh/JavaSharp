using System;
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

	using AppContext = sun.awt.AppContext;
	using AWTAccessor = sun.awt.AWTAccessor;


	/// <summary>
	/// The abstract class <code>MenuComponent</code> is the superclass
	/// of all menu-related components. In this respect, the class
	/// <code>MenuComponent</code> is analogous to the abstract superclass
	/// <code>Component</code> for AWT components.
	/// <para>
	/// Menu components receive and process AWT events, just as components do,
	/// through the method <code>processEvent</code>.
	/// 
	/// @author      Arthur van Hoff
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	[Serializable]
	public abstract class MenuComponent
	{

		static MenuComponent()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
			AWTAccessor.MenuComponentAccessor = new MenuComponentAccessorAnonymousInnerClassHelper();
		}

		private class MenuComponentAccessorAnonymousInnerClassHelper : AWTAccessor.MenuComponentAccessor
		{
			public MenuComponentAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual AppContext GetAppContext(MenuComponent menuComp)
			{
				return menuComp.AppContext;
			}
			public virtual void SetAppContext(MenuComponent menuComp, AppContext appContext)
			{
				menuComp.AppContext = appContext;
			}
			public virtual MenuContainer GetParent(MenuComponent menuComp)
			{
				return menuComp.Parent_Renamed;
			}
			public virtual Font GetFont_NoClientCode(MenuComponent menuComp)
			{
				return menuComp.Font_NoClientCode;
			}
		}

		[NonSerialized]
		internal MenuComponentPeer Peer_Renamed;
		[NonSerialized]
		internal MenuContainer Parent_Renamed;

		/// <summary>
		/// The <code>AppContext</code> of the <code>MenuComponent</code>.
		/// This is set in the constructor and never changes.
		/// </summary>
		[NonSerialized]
		internal AppContext AppContext;

		/// <summary>
		/// The menu component's font. This value can be
		/// <code>null</code> at which point a default will be used.
		/// This defaults to <code>null</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setFont(Font) </seealso>
		/// <seealso cref= #getFont() </seealso>
		internal Font Font_Renamed;

		/// <summary>
		/// The menu component's name, which defaults to <code>null</code>.
		/// @serial </summary>
		/// <seealso cref= #getName() </seealso>
		/// <seealso cref= #setName(String) </seealso>
		private String Name_Renamed;

		/// <summary>
		/// A variable to indicate whether a name is explicitly set.
		/// If <code>true</code> the name will be set explicitly.
		/// This defaults to <code>false</code>.
		/// @serial </summary>
		/// <seealso cref= #setName(String) </seealso>
		private bool NameExplicitlySet = false;

		/// <summary>
		/// Defaults to <code>false</code>.
		/// @serial </summary>
		/// <seealso cref= #dispatchEvent(AWTEvent) </seealso>
		internal bool NewEventsOnly = false;

		/*
		 * The menu's AccessControlContext.
		 */
		[NonSerialized]
		private volatile AccessControlContext Acc = AccessController.Context;

		/*
		 * Returns the acc this menu component was constructed with.
		 */
		internal AccessControlContext AccessControlContext
		{
			get
			{
				if (Acc == null)
				{
					throw new SecurityException("MenuComponent is missing AccessControlContext");
				}
				return Acc;
			}
		}

		/*
		 * Internal constants for serialization.
		 */
		internal const String ActionListenerK = Component.ActionListenerK;
		internal const String ItemListenerK = Component.ItemListenerK;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -4536902356223894379L;


		/// <summary>
		/// Creates a <code>MenuComponent</code>. </summary>
		/// <exception cref="HeadlessException"> if
		///    <code>GraphicsEnvironment.isHeadless</code>
		///    returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MenuComponent() throws HeadlessException
		public MenuComponent()
		{
			GraphicsEnvironment.CheckHeadless();
			AppContext = AppContext.AppContext;
		}

		/// <summary>
		/// Constructs a name for this <code>MenuComponent</code>.
		/// Called by <code>getName</code> when the name is <code>null</code>. </summary>
		/// <returns> a name for this <code>MenuComponent</code> </returns>
		internal virtual String ConstructComponentName()
		{
			return null; // For strict compliance with prior platform versions, a MenuComponent
						 // that doesn't set its name should return null from
						 // getName()
		}

		/// <summary>
		/// Gets the name of the menu component. </summary>
		/// <returns>        the name of the menu component </returns>
		/// <seealso cref=           java.awt.MenuComponent#setName(java.lang.String)
		/// @since         JDK1.1 </seealso>
		public virtual String Name
		{
			get
			{
				if (Name_Renamed == null && !NameExplicitlySet)
				{
					lock (this)
					{
						if (Name_Renamed == null && !NameExplicitlySet)
						{
							Name_Renamed = ConstructComponentName();
						}
					}
				}
				return Name_Renamed;
			}
			set
			{
				lock (this)
				{
					this.Name_Renamed = value;
					NameExplicitlySet = true;
				}
			}
		}


		/// <summary>
		/// Returns the parent container for this menu component. </summary>
		/// <returns>    the menu component containing this menu component,
		///                 or <code>null</code> if this menu component
		///                 is the outermost component, the menu bar itself </returns>
		public virtual MenuContainer Parent
		{
			get
			{
				return Parent_NoClientCode;
			}
		}
		// NOTE: This method may be called by privileged threads.
		//       This functionality is implemented in a package-private method
		//       to insure that it cannot be overridden by client subclasses.
		//       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		internal MenuContainer Parent_NoClientCode
		{
			get
			{
				return Parent_Renamed;
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// programs should not directly manipulate peers. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual MenuComponentPeer Peer
		{
			get
			{
				return Peer_Renamed;
			}
		}

		/// <summary>
		/// Gets the font used for this menu component. </summary>
		/// <returns>   the font used in this menu component, if there is one;
		///                  <code>null</code> otherwise </returns>
		/// <seealso cref=     java.awt.MenuComponent#setFont </seealso>
		public virtual Font Font
		{
			get
			{
				Font font = this.Font_Renamed;
				if (font != null)
				{
					return font;
				}
				MenuContainer parent = this.Parent_Renamed;
				if (parent != null)
				{
					return parent.Font;
				}
				return null;
			}
			set
			{
				Font_Renamed = value;
				//Fixed 6312943: NullPointerException in method MenuComponent.setFont(Font)
				MenuComponentPeer peer = this.Peer_Renamed;
				if (peer != null)
				{
					peer.Font = value;
				}
			}
		}

		// NOTE: This method may be called by privileged threads.
		//       This functionality is implemented in a package-private method
		//       to insure that it cannot be overridden by client subclasses.
		//       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		internal Font Font_NoClientCode
		{
			get
			{
				Font font = this.Font_Renamed;
				if (font != null)
				{
					return font;
				}
    
				// The MenuContainer interface does not have getFont_NoClientCode()
				// and it cannot, because it must be package-private. Because of
				// this, we must manually cast classes that implement
				// MenuContainer.
				Object parent = this.Parent_Renamed;
				if (parent != null)
				{
					if (parent is Component)
					{
						font = ((Component)parent).Font_NoClientCode;
					}
					else if (parent is MenuComponent)
					{
						font = ((MenuComponent)parent).Font_NoClientCode;
					}
				}
				return font;
			}
		} // getFont_NoClientCode()



		/// <summary>
		/// Removes the menu component's peer.  The peer allows us to modify the
		/// appearance of the menu component without changing the functionality of
		/// the menu component.
		/// </summary>
		public virtual void RemoveNotify()
		{
			lock (TreeLock)
			{
				MenuComponentPeer p = this.Peer_Renamed;
				if (p != null)
				{
					Toolkit.EventQueue.RemoveSourceEvents(this, true);
					this.Peer_Renamed = null;
					p.Dispose();
				}
			}
		}

		/// <summary>
		/// Posts the specified event to the menu.
		/// This method is part of the Java&nbsp;1.0 event system
		/// and it is maintained only for backwards compatibility.
		/// Its use is discouraged, and it may not be supported
		/// in the future. </summary>
		/// <param name="evt"> the event which is to take place </param>
		/// @deprecated As of JDK version 1.1, replaced by {@link
		/// #dispatchEvent(AWTEvent) dispatchEvent}. 
		[Obsolete("As of JDK version 1.1, replaced by {@link")]
		public virtual bool PostEvent(Event evt)
		{
			MenuContainer parent = this.Parent_Renamed;
			if (parent != null)
			{
				parent.PostEvent(evt);
			}
			return false;
		}

		/// <summary>
		/// Delivers an event to this component or one of its sub components. </summary>
		/// <param name="e"> the event </param>
		public void DispatchEvent(AWTEvent e)
		{
			DispatchEventImpl(e);
		}

		internal virtual void DispatchEventImpl(AWTEvent e)
		{
			EventQueue.CurrentEventAndMostRecentTime = e;

			Toolkit.DefaultToolkit.NotifyAWTEventListeners(e);

			if (NewEventsOnly || (Parent_Renamed != null && Parent_Renamed is MenuComponent && ((MenuComponent)Parent_Renamed).NewEventsOnly))
			{
				if (EventEnabled(e))
				{
					ProcessEvent(e);
				}
				else if (e is ActionEvent && Parent_Renamed != null)
				{
					e.Source = Parent_Renamed;
					((MenuComponent)Parent_Renamed).DispatchEvent(e);
				}

			} // backward compatibility
			else
			{
				Event olde = e.ConvertToOld();
				if (olde != null)
				{
					PostEvent(olde);
				}
			}
		}

		// REMIND: remove when filtering is done at lower level
		internal virtual bool EventEnabled(AWTEvent e)
		{
			return false;
		}
		/// <summary>
		/// Processes events occurring on this menu component.
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the event
		/// @since JDK1.1 </param>
		protected internal virtual void ProcessEvent(AWTEvent e)
		{
		}

		/// <summary>
		/// Returns a string representing the state of this
		/// <code>MenuComponent</code>. This method is intended to be used
		/// only for debugging purposes, and the content and format of the
		/// returned string may vary between implementations. The returned
		/// string may be empty but may not be <code>null</code>.
		/// </summary>
		/// <returns>     the parameter string of this menu component </returns>
		protected internal virtual String ParamString()
		{
			String thisName = Name;
			return (thisName != null? thisName : "");
		}

		/// <summary>
		/// Returns a representation of this menu component as a string. </summary>
		/// <returns>  a string representation of this menu component </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[" + ParamString() + "]";
		}

		/// <summary>
		/// Gets this component's locking object (the object that owns the thread
		/// synchronization monitor) for AWT component-tree and layout
		/// operations. </summary>
		/// <returns> this component's locking object </returns>
		protected internal Object TreeLock
		{
			get
			{
				return Component.LOCK;
			}
		}

		/// <summary>
		/// Reads the menu component from an object input stream.
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to read </param>
		/// <exception cref="HeadlessException"> if
		///   <code>GraphicsEnvironment.isHeadless</code> returns
		///   <code>true</code>
		/// @serial </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
			GraphicsEnvironment.CheckHeadless();

			Acc = AccessController.Context;

			s.DefaultReadObject();

			AppContext = AppContext.AppContext;
		}

		/// <summary>
		/// Initialize JNI field and method IDs.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();


		/*
		 * --- Accessibility Support ---
		 *
		 *  MenuComponent will contain all of the methods in interface Accessible,
		 *  though it won't actually implement the interface - that will be up
		 *  to the individual objects which extend MenuComponent.
		 */

		internal AccessibleContext AccessibleContext_Renamed = null;

		/// <summary>
		/// Gets the <code>AccessibleContext</code> associated with
		/// this <code>MenuComponent</code>.
		/// 
		/// The method implemented by this base class returns <code>null</code>.
		/// Classes that extend <code>MenuComponent</code>
		/// should implement this method to return the
		/// <code>AccessibleContext</code> associated with the subclass.
		/// </summary>
		/// <returns> the <code>AccessibleContext</code> of this
		///     <code>MenuComponent</code>
		/// @since 1.3 </returns>
		public virtual AccessibleContext AccessibleContext
		{
			get
			{
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// Inner class of <code>MenuComponent</code> used to provide
		/// default support for accessibility.  This class is not meant
		/// to be used directly by application developers, but is instead
		/// meant only to be subclassed by menu component developers.
		/// <para>
		/// The class used to obtain the accessible role for this object.
		/// @since 1.3
		/// </para>
		/// </summary>
		[Serializable]
		protected internal abstract class AccessibleAWTMenuComponent : AccessibleContext, AccessibleComponent, AccessibleSelection
		{
			private readonly MenuComponent OuterInstance;

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = -4269533416223798698L;

			/// <summary>
			/// Although the class is abstract, this should be called by
			/// all sub-classes.
			/// </summary>
			protected internal AccessibleAWTMenuComponent(MenuComponent outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			// AccessibleContext methods
			//

			/// <summary>
			/// Gets the <code>AccessibleSelection</code> associated with this
			/// object which allows its <code>Accessible</code> children to be selected.
			/// </summary>
			/// <returns> <code>AccessibleSelection</code> if supported by object;
			///      else return <code>null</code> </returns>
			/// <seealso cref= AccessibleSelection </seealso>
			public virtual AccessibleSelection AccessibleSelection
			{
				get
				{
					return this;
				}
			}

			/// <summary>
			/// Gets the accessible name of this object.  This should almost never
			/// return <code>java.awt.MenuComponent.getName</code>, as that
			/// generally isn't a localized name, and doesn't have meaning for the
			/// user.  If the object is fundamentally a text object (e.g. a menu item), the
			/// accessible name should be the text of the object (e.g. "save").
			/// If the object has a tooltip, the tooltip text may also be an
			/// appropriate String to return.
			/// </summary>
			/// <returns> the localized name of the object -- can be <code>null</code>
			///         if this object does not have a name </returns>
			/// <seealso cref= AccessibleContext#setAccessibleName </seealso>
			public virtual String AccessibleName
			{
				get
				{
					return accessibleName;
				}
			}

			/// <summary>
			/// Gets the accessible description of this object.  This should be
			/// a concise, localized description of what this object is - what
			/// is its meaning to the user.  If the object has a tooltip, the
			/// tooltip text may be an appropriate string to return, assuming
			/// it contains a concise description of the object (instead of just
			/// the name of the object - e.g. a "Save" icon on a toolbar that
			/// had "save" as the tooltip text shouldn't return the tooltip
			/// text as the description, but something like "Saves the current
			/// text document" instead).
			/// </summary>
			/// <returns> the localized description of the object -- can be
			///     <code>null</code> if this object does not have a description </returns>
			/// <seealso cref= AccessibleContext#setAccessibleDescription </seealso>
			public virtual String AccessibleDescription
			{
				get
				{
					return accessibleDescription;
				}
			}

			/// <summary>
			/// Gets the role of this object.
			/// </summary>
			/// <returns> an instance of <code>AccessibleRole</code>
			///     describing the role of the object </returns>
			/// <seealso cref= AccessibleRole </seealso>
			public virtual AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.AWT_COMPONENT; // Non-specific -- overridden in subclasses
				}
			}

			/// <summary>
			/// Gets the state of this object.
			/// </summary>
			/// <returns> an instance of <code>AccessibleStateSet</code>
			///     containing the current state set of the object </returns>
			/// <seealso cref= AccessibleState </seealso>
			public virtual AccessibleStateSet AccessibleStateSet
			{
				get
				{
					return OuterInstance.AccessibleStateSet;
				}
			}

			/// <summary>
			/// Gets the <code>Accessible</code> parent of this object.
			/// If the parent of this object implements <code>Accessible</code>,
			/// this method should simply return <code>getParent</code>.
			/// </summary>
			/// <returns> the <code>Accessible</code> parent of this object -- can
			///    be <code>null</code> if this object does not have an
			///    <code>Accessible</code> parent </returns>
			public virtual Accessible AccessibleParent
			{
				get
				{
					if (accessibleParent != null)
					{
						return accessibleParent;
					}
					else
					{
						MenuContainer parent = OuterInstance.Parent;
						if (parent is Accessible)
						{
							return (Accessible) parent;
						}
					}
					return null;
				}
			}

			/// <summary>
			/// Gets the index of this object in its accessible parent.
			/// </summary>
			/// <returns> the index of this object in its parent; -1 if this
			///     object does not have an accessible parent </returns>
			/// <seealso cref= #getAccessibleParent </seealso>
			public virtual int AccessibleIndexInParent
			{
				get
				{
					return OuterInstance.AccessibleIndexInParent;
				}
			}

			/// <summary>
			/// Returns the number of accessible children in the object.  If all
			/// of the children of this object implement <code>Accessible</code>,
			/// then this method should return the number of children of this object.
			/// </summary>
			/// <returns> the number of accessible children in the object </returns>
			public virtual int AccessibleChildrenCount
			{
				get
				{
					return 0; // MenuComponents don't have children
				}
			}

			/// <summary>
			/// Returns the nth <code>Accessible</code> child of the object.
			/// </summary>
			/// <param name="i"> zero-based index of child </param>
			/// <returns> the nth Accessible child of the object </returns>
			public virtual Accessible GetAccessibleChild(int i)
			{
				return null; // MenuComponents don't have children
			}

			/// <summary>
			/// Returns the locale of this object.
			/// </summary>
			/// <returns> the locale of this object </returns>
			public virtual java.util.Locale Locale
			{
				get
				{
					MenuContainer parent = OuterInstance.Parent;
					if (parent is Component)
					{
						return ((Component)parent).Locale;
					}
					else
					{
						return java.util.Locale.Default;
					}
				}
			}

			/// <summary>
			/// Gets the <code>AccessibleComponent</code> associated with
			/// this object if one exists.  Otherwise return <code>null</code>.
			/// </summary>
			/// <returns> the component </returns>
			public virtual AccessibleComponent AccessibleComponent
			{
				get
				{
					return this;
				}
			}


			// AccessibleComponent methods
			//
			/// <summary>
			/// Gets the background color of this object.
			/// </summary>
			/// <returns> the background color, if supported, of the object;
			///     otherwise, <code>null</code> </returns>
			public virtual Color Background
			{
				get
				{
					return null; // Not supported for MenuComponents
				}
				set
				{
					// Not supported for MenuComponents
				}
			}


			/// <summary>
			/// Gets the foreground color of this object.
			/// </summary>
			/// <returns> the foreground color, if supported, of the object;
			///     otherwise, <code>null</code> </returns>
			public virtual Color Foreground
			{
				get
				{
					return null; // Not supported for MenuComponents
				}
				set
				{
					// Not supported for MenuComponents
				}
			}


			/// <summary>
			/// Gets the <code>Cursor</code> of this object.
			/// </summary>
			/// <returns> the <code>Cursor</code>, if supported, of the object;
			///     otherwise, <code>null</code> </returns>
			public virtual Cursor Cursor
			{
				get
				{
					return null; // Not supported for MenuComponents
				}
				set
				{
					// Not supported for MenuComponents
				}
			}


			/// <summary>
			/// Gets the <code>Font</code> of this object.
			/// </summary>
			/// <returns> the <code>Font</code>,if supported, for the object;
			///     otherwise, <code>null</code> </returns>
			public virtual Font Font
			{
				get
				{
					return OuterInstance.Font;
				}
				set
				{
					OuterInstance.Font = value;
				}
			}


			/// <summary>
			/// Gets the <code>FontMetrics</code> of this object.
			/// </summary>
			/// <param name="f"> the <code>Font</code> </param>
			/// <returns> the FontMetrics, if supported, the object;
			///              otherwise, <code>null</code> </returns>
			/// <seealso cref= #getFont </seealso>
			public virtual FontMetrics GetFontMetrics(Font f)
			{
				return null; // Not supported for MenuComponents
			}

			/// <summary>
			/// Determines if the object is enabled.
			/// </summary>
			/// <returns> true if object is enabled; otherwise, false </returns>
			public virtual bool Enabled
			{
				get
				{
					return true; // Not supported for MenuComponents
				}
				set
				{
					// Not supported for MenuComponents
				}
			}


			/// <summary>
			/// Determines if the object is visible.  Note: this means that the
			/// object intends to be visible; however, it may not in fact be
			/// showing on the screen because one of the objects that this object
			/// is contained by is not visible.  To determine if an object is
			/// showing on the screen, use <code>isShowing</code>.
			/// </summary>
			/// <returns> true if object is visible; otherwise, false </returns>
			public virtual bool Visible
			{
				get
				{
					return true; // Not supported for MenuComponents
				}
				set
				{
					// Not supported for MenuComponents
				}
			}


			/// <summary>
			/// Determines if the object is showing.  This is determined by checking
			/// the visibility of the object and ancestors of the object.  Note:
			/// this will return true even if the object is obscured by another
			/// (for example, it happens to be underneath a menu that was pulled
			/// down).
			/// </summary>
			/// <returns> true if object is showing; otherwise, false </returns>
			public virtual bool Showing
			{
				get
				{
					return true; // Not supported for MenuComponents
				}
			}

			/// <summary>
			/// Checks whether the specified point is within this object's bounds,
			/// where the point's x and y coordinates are defined to be relative to
			/// the coordinate system of the object.
			/// </summary>
			/// <param name="p"> the <code>Point</code> relative to the coordinate
			///     system of the object </param>
			/// <returns> true if object contains <code>Point</code>; otherwise false </returns>
			public virtual bool Contains(Point p)
			{
				return false; // Not supported for MenuComponents
			}

			/// <summary>
			/// Returns the location of the object on the screen.
			/// </summary>
			/// <returns> location of object on screen -- can be <code>null</code>
			///     if this object is not on the screen </returns>
			public virtual Point LocationOnScreen
			{
				get
				{
					return null; // Not supported for MenuComponents
				}
			}

			/// <summary>
			/// Gets the location of the object relative to the parent in the form
			/// of a point specifying the object's top-left corner in the screen's
			/// coordinate space.
			/// </summary>
			/// <returns> an instance of <code>Point</code> representing the
			///    top-left corner of the object's bounds in the coordinate
			///    space of the screen; <code>null</code> if
			///    this object or its parent are not on the screen </returns>
			public virtual Point Location
			{
				get
				{
					return null; // Not supported for MenuComponents
				}
				set
				{
					// Not supported for MenuComponents
				}
			}


			/// <summary>
			/// Gets the bounds of this object in the form of a
			/// <code>Rectangle</code> object.
			/// The bounds specify this object's width, height, and location
			/// relative to its parent.
			/// </summary>
			/// <returns> a rectangle indicating this component's bounds;
			///     <code>null</code> if this object is not on the screen </returns>
			public virtual Rectangle Bounds
			{
				get
				{
					return null; // Not supported for MenuComponents
				}
				set
				{
					// Not supported for MenuComponents
				}
			}


			/// <summary>
			/// Returns the size of this object in the form of a
			/// <code>Dimension</code> object. The height field of
			/// the <code>Dimension</code> object contains this object's
			/// height, and the width field of the <code>Dimension</code>
			/// object contains this object's width.
			/// </summary>
			/// <returns> a <code>Dimension</code> object that indicates the
			///         size of this component; <code>null</code>
			///         if this object is not on the screen </returns>
			public virtual Dimension Size
			{
				get
				{
					return null; // Not supported for MenuComponents
				}
				set
				{
					// Not supported for MenuComponents
				}
			}


			/// <summary>
			/// Returns the <code>Accessible</code> child, if one exists,
			/// contained at the local coordinate <code>Point</code>.
			/// If there is no <code>Accessible</code> child, <code>null</code>
			/// is returned.
			/// </summary>
			/// <param name="p"> the point defining the top-left corner of the
			///    <code>Accessible</code>, given in the coordinate space
			///    of the object's parent </param>
			/// <returns> the <code>Accessible</code>, if it exists,
			///    at the specified location; else <code>null</code> </returns>
			public virtual Accessible GetAccessibleAt(Point p)
			{
				return null; // MenuComponents don't have children
			}

			/// <summary>
			/// Returns whether this object can accept focus or not.
			/// </summary>
			/// <returns> true if object can accept focus; otherwise false </returns>
			public virtual bool FocusTraversable
			{
				get
				{
					return true; // Not supported for MenuComponents
				}
			}

			/// <summary>
			/// Requests focus for this object.
			/// </summary>
			public virtual void RequestFocus()
			{
				// Not supported for MenuComponents
			}

			/// <summary>
			/// Adds the specified focus listener to receive focus events from this
			/// component.
			/// </summary>
			/// <param name="l"> the focus listener </param>
			public virtual void AddFocusListener(java.awt.@event.FocusListener l)
			{
				// Not supported for MenuComponents
			}

			/// <summary>
			/// Removes the specified focus listener so it no longer receives focus
			/// events from this component.
			/// </summary>
			/// <param name="l"> the focus listener </param>
			public virtual void RemoveFocusListener(java.awt.@event.FocusListener l)
			{
				// Not supported for MenuComponents
			}

			// AccessibleSelection methods
			//

			/// <summary>
			/// Returns the number of <code>Accessible</code> children currently selected.
			/// If no children are selected, the return value will be 0.
			/// </summary>
			/// <returns> the number of items currently selected </returns>
			 public virtual int AccessibleSelectionCount
			 {
				 get
				 {
					 return 0; //  To be fully implemented in a future release
				 }
			 }

			/// <summary>
			/// Returns an <code>Accessible</code> representing the specified
			/// selected child in the object.  If there isn't a selection, or there are
			/// fewer children selected than the integer passed in, the return
			/// value will be <code>null</code>.
			/// <para>Note that the index represents the i-th selected child, which
			/// is different from the i-th child.
			/// 
			/// </para>
			/// </summary>
			/// <param name="i"> the zero-based index of selected children </param>
			/// <returns> the i-th selected child </returns>
			/// <seealso cref= #getAccessibleSelectionCount </seealso>
			 public virtual Accessible GetAccessibleSelection(int i)
			 {
				 return null; //  To be fully implemented in a future release
			 }

			/// <summary>
			/// Determines if the current child of this object is selected.
			/// </summary>
			/// <returns> true if the current child of this object is selected;
			///    else false </returns>
			/// <param name="i"> the zero-based index of the child in this
			///      <code>Accessible</code> object </param>
			/// <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			 public virtual bool IsAccessibleChildSelected(int i)
			 {
				 return false; //  To be fully implemented in a future release
			 }

			/// <summary>
			/// Adds the specified <code>Accessible</code> child of the object
			/// to the object's selection.  If the object supports multiple selections,
			/// the specified child is added to any existing selection, otherwise
			/// it replaces any existing selection in the object.  If the
			/// specified child is already selected, this method has no effect.
			/// </summary>
			/// <param name="i"> the zero-based index of the child </param>
			/// <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			 public virtual void AddAccessibleSelection(int i)
			 {
				   //  To be fully implemented in a future release
			 }

			/// <summary>
			/// Removes the specified child of the object from the object's
			/// selection.  If the specified item isn't currently selected, this
			/// method has no effect.
			/// </summary>
			/// <param name="i"> the zero-based index of the child </param>
			/// <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			 public virtual void RemoveAccessibleSelection(int i)
			 {
				   //  To be fully implemented in a future release
			 }

			/// <summary>
			/// Clears the selection in the object, so that no children in the
			/// object are selected.
			/// </summary>
			 public virtual void ClearAccessibleSelection()
			 {
				   //  To be fully implemented in a future release
			 }

			/// <summary>
			/// Causes every child of the object to be selected
			/// if the object supports multiple selections.
			/// </summary>
			 public virtual void SelectAllAccessibleSelection()
			 {
				   //  To be fully implemented in a future release
			 }

		} // inner class AccessibleAWTComponent

		/// <summary>
		/// Gets the index of this object in its accessible parent.
		/// </summary>
		/// <returns> -1 if this object does not have an accessible parent;
		///      otherwise, the index of the child in its accessible parent. </returns>
		internal virtual int AccessibleIndexInParent
		{
			get
			{
				MenuContainer localParent = Parent_Renamed;
				if (!(localParent is MenuComponent))
				{
					// MenuComponents only have accessible index when inside MenuComponents
					return -1;
				}
				MenuComponent localParentMenu = (MenuComponent)localParent;
				return localParentMenu.GetAccessibleChildIndex(this);
			}
		}

		/// <summary>
		/// Gets the index of the child within this MenuComponent.
		/// </summary>
		/// <param name="child"> MenuComponent whose index we are interested in. </param>
		/// <returns> -1 if this object doesn't contain the child,
		///      otherwise, index of the child. </returns>
		internal virtual int GetAccessibleChildIndex(MenuComponent child)
		{
			return -1; // Overridden in subclasses.
		}

		/// <summary>
		/// Gets the state of this object.
		/// </summary>
		/// <returns> an instance of <code>AccessibleStateSet</code>
		///     containing the current state set of the object </returns>
		/// <seealso cref= AccessibleState </seealso>
		internal virtual AccessibleStateSet AccessibleStateSet
		{
			get
			{
				AccessibleStateSet states = new AccessibleStateSet();
				return states;
			}
		}

	}

}