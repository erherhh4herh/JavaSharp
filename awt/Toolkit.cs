using System;
using System.Diagnostics;
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


	using AppContext = sun.awt.AppContext;

	using HeadlessToolkit = sun.awt.HeadlessToolkit;
	using NullComponentPeer = sun.awt.NullComponentPeer;
	using PeerEvent = sun.awt.PeerEvent;
	using SunToolkit = sun.awt.SunToolkit;
	using AWTAccessor = sun.awt.AWTAccessor;
	using SecurityConstants = sun.security.util.SecurityConstants;

	using CoreResourceBundleControl = sun.util.CoreResourceBundleControl;

	/// <summary>
	/// This class is the abstract superclass of all actual
	/// implementations of the Abstract Window Toolkit. Subclasses of
	/// the <code>Toolkit</code> class are used to bind the various components
	/// to particular native toolkit implementations.
	/// <para>
	/// Many GUI events may be delivered to user
	/// asynchronously, if the opposite is not specified explicitly.
	/// As well as
	/// many GUI operations may be performed asynchronously.
	/// This fact means that if the state of a component is set, and then
	/// the state immediately queried, the returned value may not yet
	/// reflect the requested change.  This behavior includes, but is not
	/// limited to:
	/// <ul>
	/// <li>Scrolling to a specified position.
	/// <br>For example, calling <code>ScrollPane.setScrollPosition</code>
	///     and then <code>getScrollPosition</code> may return an incorrect
	///     value if the original request has not yet been processed.
	/// 
	/// <li>Moving the focus from one component to another.
	/// <br>For more information, see
	/// <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html#transferTiming">Timing
	/// Focus Transfers</a>, a section in
	/// <a href="http://java.sun.com/docs/books/tutorial/uiswing/">The Swing
	/// Tutorial</a>.
	/// 
	/// <li>Making a top-level container visible.
	/// <br>Calling <code>setVisible(true)</code> on a <code>Window</code>,
	///     <code>Frame</code> or <code>Dialog</code> may occur
	///     asynchronously.
	/// 
	/// <li>Setting the size or location of a top-level container.
	/// <br>Calls to <code>setSize</code>, <code>setBounds</code> or
	///     <code>setLocation</code> on a <code>Window</code>,
	///     <code>Frame</code> or <code>Dialog</code> are forwarded
	///     to the underlying window management system and may be
	///     ignored or modified.  See <seealso cref="java.awt.Window"/> for
	///     more information.
	/// </ul>
	/// </para>
	/// <para>
	/// Most applications should not call any of the methods in this
	/// class directly. The methods defined by <code>Toolkit</code> are
	/// the "glue" that joins the platform-independent classes in the
	/// <code>java.awt</code> package with their counterparts in
	/// <code>java.awt.peer</code>. Some methods defined by
	/// <code>Toolkit</code> query the native operating system directly.
	/// 
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff
	/// @author      Fred Ecks
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	public abstract class Toolkit
	{
		private bool InstanceFieldsInitialized = false;

		public Toolkit()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			DesktopPropsSupport = Toolkit.CreatePropertyChangeSupport(this);
		}


		/// <summary>
		/// Creates this toolkit's implementation of the <code>Desktop</code>
		/// using the specified peer interface. </summary>
		/// <param name="target"> the desktop to be implemented </param>
		/// <returns>    this toolkit's implementation of the <code>Desktop</code> </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Desktop </seealso>
		/// <seealso cref=       java.awt.peer.DesktopPeer
		/// @since 1.6 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract DesktopPeer createDesktopPeer(Desktop target) throws HeadlessException;
		protected internal abstract DesktopPeer CreateDesktopPeer(Desktop target);


		/// <summary>
		/// Creates this toolkit's implementation of <code>Button</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the button to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Button</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Button </seealso>
		/// <seealso cref=       java.awt.peer.ButtonPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract ButtonPeer createButton(Button target) throws HeadlessException;
		protected internal abstract ButtonPeer CreateButton(Button target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>TextField</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the text field to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>TextField</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.TextField </seealso>
		/// <seealso cref=       java.awt.peer.TextFieldPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract TextFieldPeer createTextField(TextField target) throws HeadlessException;
		protected internal abstract TextFieldPeer CreateTextField(TextField target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Label</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the label to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Label</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Label </seealso>
		/// <seealso cref=       java.awt.peer.LabelPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract LabelPeer createLabel(Label target) throws HeadlessException;
		protected internal abstract LabelPeer CreateLabel(Label target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>List</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the list to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>List</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.List </seealso>
		/// <seealso cref=       java.awt.peer.ListPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract ListPeer createList(java.awt.List target) throws HeadlessException;
		protected internal abstract ListPeer CreateList(java.awt.List target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Checkbox</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the check box to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Checkbox</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Checkbox </seealso>
		/// <seealso cref=       java.awt.peer.CheckboxPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract CheckboxPeer createCheckbox(Checkbox target) throws HeadlessException;
		protected internal abstract CheckboxPeer CreateCheckbox(Checkbox target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Scrollbar</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the scroll bar to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Scrollbar</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Scrollbar </seealso>
		/// <seealso cref=       java.awt.peer.ScrollbarPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract ScrollbarPeer createScrollbar(Scrollbar target) throws HeadlessException;
		protected internal abstract ScrollbarPeer CreateScrollbar(Scrollbar target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>ScrollPane</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the scroll pane to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>ScrollPane</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.ScrollPane </seealso>
		/// <seealso cref=       java.awt.peer.ScrollPanePeer
		/// @since     JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract ScrollPanePeer createScrollPane(ScrollPane target) throws HeadlessException;
		protected internal abstract ScrollPanePeer CreateScrollPane(ScrollPane target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>TextArea</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the text area to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>TextArea</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.TextArea </seealso>
		/// <seealso cref=       java.awt.peer.TextAreaPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract TextAreaPeer createTextArea(TextArea target) throws HeadlessException;
		protected internal abstract TextAreaPeer CreateTextArea(TextArea target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Choice</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the choice to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Choice</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Choice </seealso>
		/// <seealso cref=       java.awt.peer.ChoicePeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract ChoicePeer createChoice(Choice target) throws HeadlessException;
		protected internal abstract ChoicePeer CreateChoice(Choice target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Frame</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the frame to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Frame</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Frame </seealso>
		/// <seealso cref=       java.awt.peer.FramePeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract FramePeer createFrame(Frame target) throws HeadlessException;
		protected internal abstract FramePeer CreateFrame(Frame target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Canvas</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the canvas to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Canvas</code>. </returns>
		/// <seealso cref=       java.awt.Canvas </seealso>
		/// <seealso cref=       java.awt.peer.CanvasPeer </seealso>
		protected internal abstract CanvasPeer CreateCanvas(Canvas target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Panel</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the panel to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Panel</code>. </returns>
		/// <seealso cref=       java.awt.Panel </seealso>
		/// <seealso cref=       java.awt.peer.PanelPeer </seealso>
		protected internal abstract PanelPeer CreatePanel(Panel target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Window</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the window to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Window</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Window </seealso>
		/// <seealso cref=       java.awt.peer.WindowPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract WindowPeer createWindow(Window target) throws HeadlessException;
		protected internal abstract WindowPeer CreateWindow(Window target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Dialog</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the dialog to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Dialog</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Dialog </seealso>
		/// <seealso cref=       java.awt.peer.DialogPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract DialogPeer createDialog(Dialog target) throws HeadlessException;
		protected internal abstract DialogPeer CreateDialog(Dialog target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>MenuBar</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the menu bar to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>MenuBar</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.MenuBar </seealso>
		/// <seealso cref=       java.awt.peer.MenuBarPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract MenuBarPeer createMenuBar(MenuBar target) throws HeadlessException;
		protected internal abstract MenuBarPeer CreateMenuBar(MenuBar target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>Menu</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the menu to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>Menu</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.Menu </seealso>
		/// <seealso cref=       java.awt.peer.MenuPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract MenuPeer createMenu(Menu target) throws HeadlessException;
		protected internal abstract MenuPeer CreateMenu(Menu target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>PopupMenu</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the popup menu to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>PopupMenu</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.PopupMenu </seealso>
		/// <seealso cref=       java.awt.peer.PopupMenuPeer
		/// @since     JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract PopupMenuPeer createPopupMenu(PopupMenu target) throws HeadlessException;
		protected internal abstract PopupMenuPeer CreatePopupMenu(PopupMenu target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>MenuItem</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the menu item to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>MenuItem</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.MenuItem </seealso>
		/// <seealso cref=       java.awt.peer.MenuItemPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract MenuItemPeer createMenuItem(MenuItem target) throws HeadlessException;
		protected internal abstract MenuItemPeer CreateMenuItem(MenuItem target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>FileDialog</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the file dialog to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>FileDialog</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.FileDialog </seealso>
		/// <seealso cref=       java.awt.peer.FileDialogPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract FileDialogPeer createFileDialog(FileDialog target) throws HeadlessException;
		protected internal abstract FileDialogPeer CreateFileDialog(FileDialog target);

		/// <summary>
		/// Creates this toolkit's implementation of <code>CheckboxMenuItem</code> using
		/// the specified peer interface. </summary>
		/// <param name="target"> the checkbox menu item to be implemented. </param>
		/// <returns>    this toolkit's implementation of <code>CheckboxMenuItem</code>. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.CheckboxMenuItem </seealso>
		/// <seealso cref=       java.awt.peer.CheckboxMenuItemPeer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract CheckboxMenuItemPeer createCheckboxMenuItem(CheckboxMenuItem target) throws HeadlessException;
		protected internal abstract CheckboxMenuItemPeer CreateCheckboxMenuItem(CheckboxMenuItem target);

		/// <summary>
		/// Obtains this toolkit's implementation of helper class for
		/// <code>MouseInfo</code> operations. </summary>
		/// <returns>    this toolkit's implementation of  helper for <code>MouseInfo</code> </returns>
		/// <exception cref="UnsupportedOperationException"> if this operation is not implemented </exception>
		/// <seealso cref=       java.awt.peer.MouseInfoPeer </seealso>
		/// <seealso cref=       java.awt.MouseInfo
		/// @since 1.5 </seealso>
		protected internal virtual MouseInfoPeer MouseInfoPeer
		{
			get
			{
				throw new UnsupportedOperationException("Not implemented");
			}
		}

		private static LightweightPeer LightweightMarker;

		/// <summary>
		/// Creates a peer for a component or container.  This peer is windowless
		/// and allows the Component and Container classes to be extended directly
		/// to create windowless components that are defined entirely in java.
		/// </summary>
		/// <param name="target"> The Component to be created. </param>
		protected internal virtual LightweightPeer CreateComponent(Component target)
		{
			if (LightweightMarker == null)
			{
				LightweightMarker = new NullComponentPeer();
			}
			return LightweightMarker;
		}

		/// <summary>
		/// Creates this toolkit's implementation of <code>Font</code> using
		/// the specified peer interface. </summary>
		/// <param name="name"> the font to be implemented </param>
		/// <param name="style"> the style of the font, such as <code>PLAIN</code>,
		///            <code>BOLD</code>, <code>ITALIC</code>, or a combination </param>
		/// <returns>    this toolkit's implementation of <code>Font</code> </returns>
		/// <seealso cref=       java.awt.Font </seealso>
		/// <seealso cref=       java.awt.peer.FontPeer </seealso>
		/// <seealso cref=       java.awt.GraphicsEnvironment#getAllFonts </seealso>
		/// @deprecated  see java.awt.GraphicsEnvironment#getAllFonts 
		[Obsolete(" see java.awt.GraphicsEnvironment#getAllFonts")]
		protected internal abstract FontPeer GetFontPeer(String name, int style);

		// The following method is called by the private method
		// <code>updateSystemColors</code> in <code>SystemColor</code>.

		/// <summary>
		/// Fills in the integer array that is supplied as an argument
		/// with the current system color values.
		/// </summary>
		/// <param name="systemColors"> an integer array. </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since     JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void loadSystemColors(int[] systemColors) throws HeadlessException
		protected internal virtual void LoadSystemColors(int[] systemColors)
		{
			GraphicsEnvironment.CheckHeadless();
		}

		/// <summary>
		/// Controls whether the layout of Containers is validated dynamically
		/// during resizing, or statically, after resizing is complete.
		/// Use {@code isDynamicLayoutActive()} to detect if this feature enabled
		/// in this program and is supported by this operating system
		/// and/or window manager.
		/// Note that this feature is supported not on all platforms, and
		/// conversely, that this feature cannot be turned off on some platforms.
		/// On these platforms where dynamic layout during resizing is not supported
		/// (or is always supported), setting this property has no effect.
		/// Note that this feature can be set or unset as a property of the
		/// operating system or window manager on some platforms.  On such
		/// platforms, the dynamic resize property must be set at the operating
		/// system or window manager level before this method can take effect.
		/// This method does not change support or settings of the underlying
		/// operating system or
		/// window manager.  The OS/WM support can be
		/// queried using getDesktopProperty("awt.dynamicLayoutSupported") method.
		/// </summary>
		/// <param name="dynamic">  If true, Containers should re-layout their
		///            components as the Container is being resized.  If false,
		///            the layout will be validated after resizing is completed. </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true </exception>
		/// <seealso cref=       #isDynamicLayoutSet() </seealso>
		/// <seealso cref=       #isDynamicLayoutActive() </seealso>
		/// <seealso cref=       #getDesktopProperty(String propertyName) </seealso>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDynamicLayout(final boolean dynamic) throws HeadlessException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool DynamicLayout
		{
			set
			{
				GraphicsEnvironment.CheckHeadless();
				if (this != DefaultToolkit)
				{
					DefaultToolkit.DynamicLayout = value;
				}
			}
		}

		/// <summary>
		/// Returns whether the layout of Containers is validated dynamically
		/// during resizing, or statically, after resizing is complete.
		/// Note: this method returns the value that was set programmatically;
		/// it does not reflect support at the level of the operating system
		/// or window manager for dynamic layout on resizing, or the current
		/// operating system or window manager settings.  The OS/WM support can
		/// be queried using getDesktopProperty("awt.dynamicLayoutSupported").
		/// </summary>
		/// <returns>    true if validation of Containers is done dynamically,
		///            false if validation is done after resizing is finished. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true </exception>
		/// <seealso cref=       #setDynamicLayout(boolean dynamic) </seealso>
		/// <seealso cref=       #isDynamicLayoutActive() </seealso>
		/// <seealso cref=       #getDesktopProperty(String propertyName) </seealso>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean isDynamicLayoutSet() throws HeadlessException
		protected internal virtual bool DynamicLayoutSet
		{
			get
			{
				GraphicsEnvironment.CheckHeadless();
    
				if (this != Toolkit.DefaultToolkit)
				{
					return Toolkit.DefaultToolkit.DynamicLayoutSet;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Returns whether dynamic layout of Containers on resize is
		/// currently active (both set in program
		/// ( {@code isDynamicLayoutSet()} )
		/// , and supported
		/// by the underlying operating system and/or window manager).
		/// If dynamic layout is currently inactive then Containers
		/// re-layout their components when resizing is completed. As a result
		/// the {@code Component.validate()} method will be invoked only
		/// once per resize.
		/// If dynamic layout is currently active then Containers
		/// re-layout their components on every native resize event and
		/// the {@code validate()} method will be invoked each time.
		/// The OS/WM support can be queried using
		/// the getDesktopProperty("awt.dynamicLayoutSupported") method.
		/// </summary>
		/// <returns>    true if dynamic layout of Containers on resize is
		///            currently active, false otherwise. </returns>
		/// <exception cref="HeadlessException"> if the GraphicsEnvironment.isHeadless()
		///            method returns true </exception>
		/// <seealso cref=       #setDynamicLayout(boolean dynamic) </seealso>
		/// <seealso cref=       #isDynamicLayoutSet() </seealso>
		/// <seealso cref=       #getDesktopProperty(String propertyName) </seealso>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isDynamicLayoutActive() throws HeadlessException
		public virtual bool DynamicLayoutActive
		{
			get
			{
				GraphicsEnvironment.CheckHeadless();
    
				if (this != Toolkit.DefaultToolkit)
				{
					return Toolkit.DefaultToolkit.DynamicLayoutActive;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the size of the screen.  On systems with multiple displays, the
		/// primary display is used.  Multi-screen aware display dimensions are
		/// available from <code>GraphicsConfiguration</code> and
		/// <code>GraphicsDevice</code>. </summary>
		/// <returns>    the size of this toolkit's screen, in pixels. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsConfiguration#getBounds </seealso>
		/// <seealso cref=       java.awt.GraphicsDevice#getDisplayMode </seealso>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Dimension getScreenSize() throws HeadlessException;
		public abstract Dimension ScreenSize {get;}

		/// <summary>
		/// Returns the screen resolution in dots-per-inch. </summary>
		/// <returns>    this toolkit's screen resolution, in dots-per-inch. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int getScreenResolution() throws HeadlessException;
		public abstract int ScreenResolution {get;}

		/// <summary>
		/// Gets the insets of the screen. </summary>
		/// <param name="gc"> a <code>GraphicsConfiguration</code> </param>
		/// <returns>    the insets of this toolkit's screen, in pixels. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Insets getScreenInsets(GraphicsConfiguration gc) throws HeadlessException
		public virtual Insets GetScreenInsets(GraphicsConfiguration gc)
		{
			GraphicsEnvironment.CheckHeadless();
			if (this != Toolkit.DefaultToolkit)
			{
				return Toolkit.DefaultToolkit.GetScreenInsets(gc);
			}
			else
			{
				return new Insets(0, 0, 0, 0);
			}
		}

		/// <summary>
		/// Determines the color model of this toolkit's screen.
		/// <para>
		/// <code>ColorModel</code> is an abstract class that
		/// encapsulates the ability to translate between the
		/// pixel values of an image and its red, green, blue,
		/// and alpha components.
		/// </para>
		/// <para>
		/// This toolkit method is called by the
		/// <code>getColorModel</code> method
		/// of the <code>Component</code> class.
		/// </para>
		/// </summary>
		/// <returns>    the color model of this toolkit's screen. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.image.ColorModel </seealso>
		/// <seealso cref=       java.awt.Component#getColorModel </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.awt.image.ColorModel getColorModel() throws HeadlessException;
		public abstract ColorModel ColorModel {get;}

		/// <summary>
		/// Returns the names of the available fonts in this toolkit.<para>
		/// For 1.1, the following font names are deprecated (the replacement
		/// name follows):
		/// <ul>
		/// <li>TimesRoman (use Serif)
		/// <li>Helvetica (use SansSerif)
		/// <li>Courier (use Monospaced)
		/// </para>
		/// </ul><para>
		/// The ZapfDingbats fontname is also deprecated in 1.1 but the characters
		/// are defined in Unicode starting at 0x2700, and as of 1.1 Java supports
		/// those characters.
		/// </para>
		/// </summary>
		/// <returns>    the names of the available fonts in this toolkit. </returns>
		/// @deprecated see <seealso cref="java.awt.GraphicsEnvironment#getAvailableFontFamilyNames()"/> 
		/// <seealso cref= java.awt.GraphicsEnvironment#getAvailableFontFamilyNames() </seealso>
		[Obsolete("see <seealso cref="java.awt.GraphicsEnvironment#getAvailableFontFamilyNames()"/>")]
		public abstract String[] FontList {get;}

		/// <summary>
		/// Gets the screen device metrics for rendering of the font. </summary>
		/// <param name="font">   a font </param>
		/// <returns>    the screen metrics of the specified font in this toolkit </returns>
		/// @deprecated  As of JDK version 1.2, replaced by the <code>Font</code>
		///          method <code>getLineMetrics</code>. 
		/// <seealso cref= java.awt.font.LineMetrics </seealso>
		/// <seealso cref= java.awt.Font#getLineMetrics </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#getScreenDevices </seealso>
		[Obsolete(" As of JDK version 1.2, replaced by the <code>Font</code>")]
		public abstract FontMetrics GetFontMetrics(Font font);

		/// <summary>
		/// Synchronizes this toolkit's graphics state. Some window systems
		/// may do buffering of graphics events.
		/// <para>
		/// This method ensures that the display is up-to-date. It is useful
		/// for animation.
		/// </para>
		/// </summary>
		public abstract void Sync();

		/// <summary>
		/// The default toolkit.
		/// </summary>
		private static Toolkit Toolkit;

		/// <summary>
		/// Used internally by the assistive technologies functions; set at
		/// init time and used at load time
		/// </summary>
		private static String AtNames;

		/// <summary>
		/// Initializes properties related to assistive technologies.
		/// These properties are used both in the loadAssistiveProperties()
		/// function below, as well as other classes in the jdk that depend
		/// on the properties (such as the use of the screen_magnifier_present
		/// property in Java2D hardware acceleration initialization).  The
		/// initialization of the properties must be done before the platform-
		/// specific Toolkit class is instantiated so that all necessary
		/// properties are set up properly before any classes dependent upon them
		/// are initialized.
		/// </summary>
		private static void InitAssistiveTechnologies()
		{

			// Get accessibility properties
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String sep = java.io.File.separator;
			String sep = File.separator;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Properties properties = new Properties();
			Properties properties = new Properties();


			AtNames = java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(sep, properties));
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<String>
		{
			private string Sep;
			private java.util.Properties Properties;

			public PrivilegedActionAnonymousInnerClassHelper(string sep, java.util.Properties properties)
			{
				this.Sep = sep;
				this.Properties = properties;
			}

			public virtual String Run()
			{

				// Try loading the per-user accessibility properties file.
				try
				{
					File propsFile = new File(System.getProperty("user.home") + Sep + ".accessibility.properties");
					FileInputStream @in = new FileInputStream(propsFile);

					// Inputstream has been buffered in Properties class
					Properties.Load(@in);
					@in.Close();
				}
				catch (Exception)
				{
					// Per-user accessibility properties file does not exist
				}

				// Try loading the system-wide accessibility properties
				// file only if a per-user accessibility properties
				// file does not exist or is empty.
				if (Properties.Count == 0)
				{
					try
					{
						File propsFile = new File(System.getProperty("java.home") + Sep + "lib" + Sep + "accessibility.properties");
						FileInputStream @in = new FileInputStream(propsFile);

						// Inputstream has been buffered in Properties class
						Properties.Load(@in);
						@in.Close();
					}
					catch (Exception)
					{
						// System-wide accessibility properties file does
						// not exist;
					}
				}

				// Get whether a screen magnifier is present.  First check
				// the system property and then check the properties file.
				String magPresent = System.getProperty("javax.accessibility.screen_magnifier_present");
				if (magPresent == null)
				{
					magPresent = Properties.GetProperty("screen_magnifier_present", null);
					if (magPresent != null)
					{
						System.setProperty("javax.accessibility.screen_magnifier_present", magPresent);
					}
				}

				// Get the names of any assistive technolgies to load.  First
				// check the system property and then check the properties
				// file.
				String classNames = System.getProperty("javax.accessibility.assistive_technologies");
				if (classNames == null)
				{
					classNames = Properties.GetProperty("assistive_technologies", null);
					if (classNames != null)
					{
						System.setProperty("javax.accessibility.assistive_technologies", classNames);
					}
				}
				return classNames;
			}
		}

		/// <summary>
		/// Loads additional classes into the VM, using the property
		/// 'assistive_technologies' specified in the Sun reference
		/// implementation by a line in the 'accessibility.properties'
		/// file.  The form is "assistive_technologies=..." where
		/// the "..." is a comma-separated list of assistive technology
		/// classes to load.  Each class is loaded in the order given
		/// and a single instance of each is created using
		/// Class.forName(class).newInstance().  All errors are handled
		/// via an AWTError exception.
		/// 
		/// <para>The assumption is made that assistive technology classes are supplied
		/// as part of INSTALLED (as opposed to: BUNDLED) extensions or specified
		/// on the class path
		/// (and therefore can be loaded using the class loader returned by
		/// a call to <code>ClassLoader.getSystemClassLoader</code>, whose
		/// delegation parent is the extension class loader for installed
		/// extensions).
		/// </para>
		/// </summary>
		private static void LoadAssistiveTechnologies()
		{
			// Load any assistive technologies
			if (AtNames != null)
			{
				ClassLoader cl = ClassLoader.SystemClassLoader;
				StringTokenizer parser = new StringTokenizer(AtNames," ,");
				String atName;
				while (parser.HasMoreTokens())
				{
					atName = parser.NextToken();
					try
					{
						Class clazz;
						if (cl != null)
						{
							clazz = cl.LoadClass(atName);
						}
						else
						{
							clazz = Class.ForName(atName);
						}
						clazz.NewInstance();
					}
					catch (ClassNotFoundException)
					{
						throw new AWTError("Assistive Technology not found: " + atName);
					}
					catch (InstantiationException)
					{
						throw new AWTError("Could not instantiate Assistive" + " Technology: " + atName);
					}
					catch (IllegalAccessException)
					{
						throw new AWTError("Could not access Assistive" + " Technology: " + atName);
					}
					catch (Exception e)
					{
						throw new AWTError("Error trying to install Assistive" + " Technology: " + atName + " " + e);
					}
				}
			}
		}

		/// <summary>
		/// Gets the default toolkit.
		/// <para>
		/// If a system property named <code>"java.awt.headless"</code> is set
		/// to <code>true</code> then the headless implementation
		/// of <code>Toolkit</code> is used.
		/// </para>
		/// <para>
		/// If there is no <code>"java.awt.headless"</code> or it is set to
		/// <code>false</code> and there is a system property named
		/// <code>"awt.toolkit"</code>,
		/// that property is treated as the name of a class that is a subclass
		/// of <code>Toolkit</code>;
		/// otherwise the default platform-specific implementation of
		/// <code>Toolkit</code> is used.
		/// </para>
		/// <para>
		/// Also loads additional classes into the VM, using the property
		/// 'assistive_technologies' specified in the Sun reference
		/// implementation by a line in the 'accessibility.properties'
		/// file.  The form is "assistive_technologies=..." where
		/// the "..." is a comma-separated list of assistive technology
		/// classes to load.  Each class is loaded in the order given
		/// and a single instance of each is created using
		/// Class.forName(class).newInstance().  This is done just after
		/// the AWT toolkit is created.  All errors are handled via an
		/// AWTError exception.
		/// </para>
		/// </summary>
		/// <returns>    the default toolkit. </returns>
		/// <exception cref="AWTError">  if a toolkit could not be found, or
		///                 if one could not be accessed or instantiated. </exception>
		public static Toolkit DefaultToolkit
		{
			get
			{
				lock (typeof(Toolkit))
				{
					if (Toolkit == null)
					{
						java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2());
						LoadAssistiveTechnologies();
					}
					return Toolkit;
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : java.security.PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper2()
			{
			}

			public virtual Void Run()
			{
				Class cls = null;
				String nm = System.getProperty("awt.toolkit");
				try
				{
					cls = Class.ForName(nm);
				}
				catch (ClassNotFoundException)
				{
					ClassLoader cl = ClassLoader.SystemClassLoader;
					if (cl != null)
					{
						try
						{
							cls = cl.LoadClass(nm);
						}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final ClassNotFoundException ignored)
						catch (assNotFoundException)
						{
							throw new AWTError("Toolkit not found: " + nm);
						}
					}
				}
				try
				{
					if (cls != null)
					{
						Toolkit = (Toolkit)cls.NewInstance();
						if (GraphicsEnvironment.Headless)
						{
							Toolkit = new HeadlessToolkit(Toolkit);
						}
					}
				}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final InstantiationException ignored)
				catch (stantiationException)
				{
					throw new AWTError("Could not instantiate Toolkit: " + nm);
				}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final IllegalAccessException ignored)
				catch (legalAccessException)
				{
					throw new AWTError("Could not access Toolkit: " + nm);
				}
				return null;
			}
		}

		/// <summary>
		/// Returns an image which gets pixel data from the specified file,
		/// whose format can be either GIF, JPEG or PNG.
		/// The underlying toolkit attempts to resolve multiple requests
		/// with the same filename to the same returned Image.
		/// <para>
		/// Since the mechanism required to facilitate this sharing of
		/// <code>Image</code> objects may continue to hold onto images
		/// that are no longer in use for an indefinite period of time,
		/// developers are encouraged to implement their own caching of
		/// images by using the <seealso cref="#createImage(java.lang.String) createImage"/>
		/// variant wherever available.
		/// If the image data contained in the specified file changes,
		/// the <code>Image</code> object returned from this method may
		/// still contain stale information which was loaded from the
		/// file after a prior call.
		/// Previously loaded image data can be manually discarded by
		/// calling the <seealso cref="Image#flush flush"/> method on the
		/// returned <code>Image</code>.
		/// </para>
		/// <para>
		/// This method first checks if there is a security manager installed.
		/// If so, the method calls the security manager's
		/// <code>checkRead</code> method with the file specified to ensure
		/// that the access to the image is allowed.
		/// </para>
		/// </summary>
		/// <param name="filename">   the name of a file containing pixel data
		///                         in a recognized file format. </param>
		/// <returns>    an image which gets its pixel data from
		///                         the specified file. </returns>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///                            checkRead method doesn't allow the operation. </exception>
		/// <seealso cref= #createImage(java.lang.String) </seealso>
		public abstract Image GetImage(String filename);

		/// <summary>
		/// Returns an image which gets pixel data from the specified URL.
		/// The pixel data referenced by the specified URL must be in one
		/// of the following formats: GIF, JPEG or PNG.
		/// The underlying toolkit attempts to resolve multiple requests
		/// with the same URL to the same returned Image.
		/// <para>
		/// Since the mechanism required to facilitate this sharing of
		/// <code>Image</code> objects may continue to hold onto images
		/// that are no longer in use for an indefinite period of time,
		/// developers are encouraged to implement their own caching of
		/// images by using the <seealso cref="#createImage(java.net.URL) createImage"/>
		/// variant wherever available.
		/// If the image data stored at the specified URL changes,
		/// the <code>Image</code> object returned from this method may
		/// still contain stale information which was fetched from the
		/// URL after a prior call.
		/// Previously loaded image data can be manually discarded by
		/// calling the <seealso cref="Image#flush flush"/> method on the
		/// returned <code>Image</code>.
		/// </para>
		/// <para>
		/// This method first checks if there is a security manager installed.
		/// If so, the method calls the security manager's
		/// <code>checkPermission</code> method with the
		/// url.openConnection().getPermission() permission to ensure
		/// that the access to the image is allowed. For compatibility
		/// with pre-1.2 security managers, if the access is denied with
		/// <code>FilePermission</code> or <code>SocketPermission</code>,
		/// the method throws the <code>SecurityException</code>
		/// if the corresponding 1.1-style SecurityManager.checkXXX method
		/// also denies permission.
		/// </para>
		/// </summary>
		/// <param name="url">   the URL to use in fetching the pixel data. </param>
		/// <returns>    an image which gets its pixel data from
		///                         the specified URL. </returns>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///                            checkPermission method doesn't allow
		///                            the operation. </exception>
		/// <seealso cref= #createImage(java.net.URL) </seealso>
		public abstract Image GetImage(URL url);

		/// <summary>
		/// Returns an image which gets pixel data from the specified file.
		/// The returned Image is a new object which will not be shared
		/// with any other caller of this method or its getImage variant.
		/// <para>
		/// This method first checks if there is a security manager installed.
		/// If so, the method calls the security manager's
		/// <code>checkRead</code> method with the specified file to ensure
		/// that the image creation is allowed.
		/// </para>
		/// </summary>
		/// <param name="filename">   the name of a file containing pixel data
		///                         in a recognized file format. </param>
		/// <returns>    an image which gets its pixel data from
		///                         the specified file. </returns>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///                            checkRead method doesn't allow the operation. </exception>
		/// <seealso cref= #getImage(java.lang.String) </seealso>
		public abstract Image CreateImage(String filename);

		/// <summary>
		/// Returns an image which gets pixel data from the specified URL.
		/// The returned Image is a new object which will not be shared
		/// with any other caller of this method or its getImage variant.
		/// <para>
		/// This method first checks if there is a security manager installed.
		/// If so, the method calls the security manager's
		/// <code>checkPermission</code> method with the
		/// url.openConnection().getPermission() permission to ensure
		/// that the image creation is allowed. For compatibility
		/// with pre-1.2 security managers, if the access is denied with
		/// <code>FilePermission</code> or <code>SocketPermission</code>,
		/// the method throws <code>SecurityException</code>
		/// if the corresponding 1.1-style SecurityManager.checkXXX method
		/// also denies permission.
		/// </para>
		/// </summary>
		/// <param name="url">   the URL to use in fetching the pixel data. </param>
		/// <returns>    an image which gets its pixel data from
		///                         the specified URL. </returns>
		/// <exception cref="SecurityException">  if a security manager exists and its
		///                            checkPermission method doesn't allow
		///                            the operation. </exception>
		/// <seealso cref= #getImage(java.net.URL) </seealso>
		public abstract Image CreateImage(URL url);

		/// <summary>
		/// Prepares an image for rendering.
		/// <para>
		/// If the values of the width and height arguments are both
		/// <code>-1</code>, this method prepares the image for rendering
		/// on the default screen; otherwise, this method prepares an image
		/// for rendering on the default screen at the specified width and height.
		/// </para>
		/// <para>
		/// The image data is downloaded asynchronously in another thread,
		/// and an appropriately scaled screen representation of the image is
		/// generated.
		/// </para>
		/// <para>
		/// This method is called by components <code>prepareImage</code>
		/// methods.
		/// </para>
		/// <para>
		/// Information on the flags returned by this method can be found
		/// with the definition of the <code>ImageObserver</code> interface.
		/// 
		/// </para>
		/// </summary>
		/// <param name="image">      the image for which to prepare a
		///                           screen representation. </param>
		/// <param name="width">      the width of the desired screen
		///                           representation, or <code>-1</code>. </param>
		/// <param name="height">     the height of the desired screen
		///                           representation, or <code>-1</code>. </param>
		/// <param name="observer">   the <code>ImageObserver</code>
		///                           object to be notified as the
		///                           image is being prepared. </param>
		/// <returns>    <code>true</code> if the image has already been
		///                 fully prepared; <code>false</code> otherwise. </returns>
		/// <seealso cref=       java.awt.Component#prepareImage(java.awt.Image,
		///                 java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=       java.awt.Component#prepareImage(java.awt.Image,
		///                 int, int, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=       java.awt.image.ImageObserver </seealso>
		public abstract bool PrepareImage(Image image, int width, int height, ImageObserver observer);

		/// <summary>
		/// Indicates the construction status of a specified image that is
		/// being prepared for display.
		/// <para>
		/// If the values of the width and height arguments are both
		/// <code>-1</code>, this method returns the construction status of
		/// a screen representation of the specified image in this toolkit.
		/// Otherwise, this method returns the construction status of a
		/// scaled representation of the image at the specified width
		/// and height.
		/// </para>
		/// <para>
		/// This method does not cause the image to begin loading.
		/// An application must call <code>prepareImage</code> to force
		/// the loading of an image.
		/// </para>
		/// <para>
		/// This method is called by the component's <code>checkImage</code>
		/// methods.
		/// </para>
		/// <para>
		/// Information on the flags returned by this method can be found
		/// with the definition of the <code>ImageObserver</code> interface.
		/// </para>
		/// </summary>
		/// <param name="image">   the image whose status is being checked. </param>
		/// <param name="width">   the width of the scaled version whose status is
		///                 being checked, or <code>-1</code>. </param>
		/// <param name="height">  the height of the scaled version whose status
		///                 is being checked, or <code>-1</code>. </param>
		/// <param name="observer">   the <code>ImageObserver</code> object to be
		///                 notified as the image is being prepared. </param>
		/// <returns>    the bitwise inclusive <strong>OR</strong> of the
		///                 <code>ImageObserver</code> flags for the
		///                 image data that is currently available. </returns>
		/// <seealso cref=       java.awt.Toolkit#prepareImage(java.awt.Image,
		///                 int, int, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=       java.awt.Component#checkImage(java.awt.Image,
		///                 java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=       java.awt.Component#checkImage(java.awt.Image,
		///                 int, int, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=       java.awt.image.ImageObserver </seealso>
		public abstract int CheckImage(Image image, int width, int height, ImageObserver observer);

		/// <summary>
		/// Creates an image with the specified image producer. </summary>
		/// <param name="producer"> the image producer to be used. </param>
		/// <returns>    an image with the specified image producer. </returns>
		/// <seealso cref=       java.awt.Image </seealso>
		/// <seealso cref=       java.awt.image.ImageProducer </seealso>
		/// <seealso cref=       java.awt.Component#createImage(java.awt.image.ImageProducer) </seealso>
		public abstract Image CreateImage(ImageProducer producer);

		/// <summary>
		/// Creates an image which decodes the image stored in the specified
		/// byte array.
		/// <para>
		/// The data must be in some image format, such as GIF or JPEG,
		/// that is supported by this toolkit.
		/// </para>
		/// </summary>
		/// <param name="imagedata">   an array of bytes, representing
		///                         image data in a supported image format. </param>
		/// <returns>    an image.
		/// @since     JDK1.1 </returns>
		public virtual Image CreateImage(sbyte[] imagedata)
		{
			return CreateImage(imagedata, 0, imagedata.Length);
		}

		/// <summary>
		/// Creates an image which decodes the image stored in the specified
		/// byte array, and at the specified offset and length.
		/// The data must be in some image format, such as GIF or JPEG,
		/// that is supported by this toolkit. </summary>
		/// <param name="imagedata">   an array of bytes, representing
		///                         image data in a supported image format. </param>
		/// <param name="imageoffset">  the offset of the beginning
		///                         of the data in the array. </param>
		/// <param name="imagelength">  the length of the data in the array. </param>
		/// <returns>    an image.
		/// @since     JDK1.1 </returns>
		public abstract Image CreateImage(sbyte[] imagedata, int imageoffset, int imagelength);

		/// <summary>
		/// Gets a <code>PrintJob</code> object which is the result of initiating
		/// a print operation on the toolkit's platform.
		/// <para>
		/// Each actual implementation of this method should first check if there
		/// is a security manager installed. If there is, the method should call
		/// the security manager's <code>checkPrintJobAccess</code> method to
		/// ensure initiation of a print operation is allowed. If the default
		/// implementation of <code>checkPrintJobAccess</code> is used (that is,
		/// that method is not overriden), then this results in a call to the
		/// security manager's <code>checkPermission</code> method with a <code>
		/// RuntimePermission("queuePrintJob")</code> permission.
		/// 
		/// </para>
		/// </summary>
		/// <param name="frame"> the parent of the print dialog. May not be null. </param>
		/// <param name="jobtitle"> the title of the PrintJob. A null title is equivalent
		///          to "". </param>
		/// <param name="props"> a Properties object containing zero or more properties.
		///          Properties are not standardized and are not consistent across
		///          implementations. Because of this, PrintJobs which require job
		///          and page control should use the version of this function which
		///          takes JobAttributes and PageAttributes objects. This object
		///          may be updated to reflect the user's job choices on exit. May
		///          be null. </param>
		/// <returns>  a <code>PrintJob</code> object, or <code>null</code> if the
		///          user cancelled the print job. </returns>
		/// <exception cref="NullPointerException"> if frame is null </exception>
		/// <exception cref="SecurityException"> if this thread is not allowed to initiate a
		///          print job request </exception>
		/// <seealso cref=     java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=     java.awt.PrintJob </seealso>
		/// <seealso cref=     java.lang.RuntimePermission
		/// @since   JDK1.1 </seealso>
		public abstract PrintJob GetPrintJob(Frame frame, String jobtitle, Properties props);

		/// <summary>
		/// Gets a <code>PrintJob</code> object which is the result of initiating
		/// a print operation on the toolkit's platform.
		/// <para>
		/// Each actual implementation of this method should first check if there
		/// is a security manager installed. If there is, the method should call
		/// the security manager's <code>checkPrintJobAccess</code> method to
		/// ensure initiation of a print operation is allowed. If the default
		/// implementation of <code>checkPrintJobAccess</code> is used (that is,
		/// that method is not overriden), then this results in a call to the
		/// security manager's <code>checkPermission</code> method with a <code>
		/// RuntimePermission("queuePrintJob")</code> permission.
		/// 
		/// </para>
		/// </summary>
		/// <param name="frame"> the parent of the print dialog. May not be null. </param>
		/// <param name="jobtitle"> the title of the PrintJob. A null title is equivalent
		///          to "". </param>
		/// <param name="jobAttributes"> a set of job attributes which will control the
		///          PrintJob. The attributes will be updated to reflect the user's
		///          choices as outlined in the JobAttributes documentation. May be
		///          null. </param>
		/// <param name="pageAttributes"> a set of page attributes which will control the
		///          PrintJob. The attributes will be applied to every page in the
		///          job. The attributes will be updated to reflect the user's
		///          choices as outlined in the PageAttributes documentation. May be
		///          null. </param>
		/// <returns>  a <code>PrintJob</code> object, or <code>null</code> if the
		///          user cancelled the print job. </returns>
		/// <exception cref="NullPointerException"> if frame is null </exception>
		/// <exception cref="IllegalArgumentException"> if pageAttributes specifies differing
		///          cross feed and feed resolutions. Also if this thread has
		///          access to the file system and jobAttributes specifies
		///          print to file, and the specified destination file exists but
		///          is a directory rather than a regular file, does not exist but
		///          cannot be created, or cannot be opened for any other reason.
		///          However in the case of print to file, if a dialog is also
		///          requested to be displayed then the user will be given an
		///          opportunity to select a file and proceed with printing.
		///          The dialog will ensure that the selected output file
		///          is valid before returning from this method. </exception>
		/// <exception cref="SecurityException"> if this thread is not allowed to initiate a
		///          print job request, or if jobAttributes specifies print to file,
		///          and this thread is not allowed to access the file system </exception>
		/// <seealso cref=     java.awt.PrintJob </seealso>
		/// <seealso cref=     java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=     java.lang.RuntimePermission </seealso>
		/// <seealso cref=     java.awt.JobAttributes </seealso>
		/// <seealso cref=     java.awt.PageAttributes
		/// @since   1.3 </seealso>
		public virtual PrintJob GetPrintJob(Frame frame, String jobtitle, JobAttributes jobAttributes, PageAttributes pageAttributes)
		{
			// Override to add printing support with new job/page control classes

			if (this != Toolkit.DefaultToolkit)
			{
				return Toolkit.DefaultToolkit.GetPrintJob(frame, jobtitle, jobAttributes, pageAttributes);
			}
			else
			{
				return GetPrintJob(frame, jobtitle, null);
			}
		}

		/// <summary>
		/// Emits an audio beep depending on native system settings and hardware
		/// capabilities.
		/// @since     JDK1.1
		/// </summary>
		public abstract void Beep();

		/// <summary>
		/// Gets the singleton instance of the system Clipboard which interfaces
		/// with clipboard facilities provided by the native platform. This
		/// clipboard enables data transfer between Java programs and native
		/// applications which use native clipboard facilities.
		/// <para>
		/// In addition to any and all formats specified in the flavormap.properties
		/// file, or other file specified by the <code>AWT.DnD.flavorMapFileURL
		/// </code> Toolkit property, text returned by the system Clipboard's <code>
		/// getTransferData()</code> method is available in the following flavors:
		/// <ul>
		/// <li>DataFlavor.stringFlavor</li>
		/// <li>DataFlavor.plainTextFlavor (<b>deprecated</b>)</li>
		/// </ul>
		/// As with <code>java.awt.datatransfer.StringSelection</code>, if the
		/// requested flavor is <code>DataFlavor.plainTextFlavor</code>, or an
		/// equivalent flavor, a Reader is returned. <b>Note:</b> The behavior of
		/// the system Clipboard's <code>getTransferData()</code> method for <code>
		/// DataFlavor.plainTextFlavor</code>, and equivalent DataFlavors, is
		/// inconsistent with the definition of <code>DataFlavor.plainTextFlavor
		/// </code>. Because of this, support for <code>
		/// DataFlavor.plainTextFlavor</code>, and equivalent flavors, is
		/// <b>deprecated</b>.
		/// </para>
		/// <para>
		/// Each actual implementation of this method should first check if there
		/// is a security manager installed. If there is, the method should call
		/// the security manager's {@link SecurityManager#checkPermission
		/// checkPermission} method to check {@code AWTPermission("accessClipboard")}.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    the system Clipboard </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.datatransfer.Clipboard </seealso>
		/// <seealso cref=       java.awt.datatransfer.StringSelection </seealso>
		/// <seealso cref=       java.awt.datatransfer.DataFlavor#stringFlavor </seealso>
		/// <seealso cref=       java.awt.datatransfer.DataFlavor#plainTextFlavor </seealso>
		/// <seealso cref=       java.io.Reader </seealso>
		/// <seealso cref=       java.awt.AWTPermission
		/// @since     JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.awt.datatransfer.Clipboard getSystemClipboard() throws HeadlessException;
		public abstract Clipboard SystemClipboard {get;}

		/// <summary>
		/// Gets the singleton instance of the system selection as a
		/// <code>Clipboard</code> object. This allows an application to read and
		/// modify the current, system-wide selection.
		/// <para>
		/// An application is responsible for updating the system selection whenever
		/// the user selects text, using either the mouse or the keyboard.
		/// Typically, this is implemented by installing a
		/// <code>FocusListener</code> on all <code>Component</code>s which support
		/// text selection, and, between <code>FOCUS_GAINED</code> and
		/// <code>FOCUS_LOST</code> events delivered to that <code>Component</code>,
		/// updating the system selection <code>Clipboard</code> when the selection
		/// changes inside the <code>Component</code>. Properly updating the system
		/// selection ensures that a Java application will interact correctly with
		/// native applications and other Java applications running simultaneously
		/// on the system. Note that <code>java.awt.TextComponent</code> and
		/// <code>javax.swing.text.JTextComponent</code> already adhere to this
		/// policy. When using these classes, and their subclasses, developers need
		/// not write any additional code.
		/// </para>
		/// <para>
		/// Some platforms do not support a system selection <code>Clipboard</code>.
		/// On those platforms, this method will return <code>null</code>. In such a
		/// case, an application is absolved from its responsibility to update the
		/// system selection <code>Clipboard</code> as described above.
		/// </para>
		/// <para>
		/// Each actual implementation of this method should first check if there
		/// is a security manager installed. If there is, the method should call
		/// the security manager's {@link SecurityManager#checkPermission
		/// checkPermission} method to check {@code AWTPermission("accessClipboard")}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the system selection as a <code>Clipboard</code>, or
		///         <code>null</code> if the native platform does not support a
		///         system selection <code>Clipboard</code> </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true
		/// </exception>
		/// <seealso cref= java.awt.datatransfer.Clipboard </seealso>
		/// <seealso cref= java.awt.event.FocusListener </seealso>
		/// <seealso cref= java.awt.event.FocusEvent#FOCUS_GAINED </seealso>
		/// <seealso cref= java.awt.event.FocusEvent#FOCUS_LOST </seealso>
		/// <seealso cref= TextComponent </seealso>
		/// <seealso cref= javax.swing.text.JTextComponent </seealso>
		/// <seealso cref= AWTPermission </seealso>
		/// <seealso cref= GraphicsEnvironment#isHeadless
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.awt.datatransfer.Clipboard getSystemSelection() throws HeadlessException
		public virtual Clipboard SystemSelection
		{
			get
			{
				GraphicsEnvironment.CheckHeadless();
    
				if (this != Toolkit.DefaultToolkit)
				{
					return Toolkit.DefaultToolkit.SystemSelection;
				}
				else
				{
					GraphicsEnvironment.CheckHeadless();
					return null;
				}
			}
		}

		/// <summary>
		/// Determines which modifier key is the appropriate accelerator
		/// key for menu shortcuts.
		/// <para>
		/// Menu shortcuts, which are embodied in the
		/// <code>MenuShortcut</code> class, are handled by the
		/// <code>MenuBar</code> class.
		/// </para>
		/// <para>
		/// By default, this method returns <code>Event.CTRL_MASK</code>.
		/// Toolkit implementations should override this method if the
		/// <b>Control</b> key isn't the correct key for accelerators.
		/// </para>
		/// </summary>
		/// <returns>    the modifier mask on the <code>Event</code> class
		///                 that is used for menu shortcuts on this toolkit. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.MenuBar </seealso>
		/// <seealso cref=       java.awt.MenuShortcut
		/// @since     JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMenuShortcutKeyMask() throws HeadlessException
		public virtual int MenuShortcutKeyMask
		{
			get
			{
				GraphicsEnvironment.CheckHeadless();
    
				return Event.CTRL_MASK;
			}
		}

		/// <summary>
		/// Returns whether the given locking key on the keyboard is currently in
		/// its "on" state.
		/// Valid key codes are
		/// <seealso cref="java.awt.event.KeyEvent#VK_CAPS_LOCK VK_CAPS_LOCK"/>,
		/// <seealso cref="java.awt.event.KeyEvent#VK_NUM_LOCK VK_NUM_LOCK"/>,
		/// <seealso cref="java.awt.event.KeyEvent#VK_SCROLL_LOCK VK_SCROLL_LOCK"/>, and
		/// <seealso cref="java.awt.event.KeyEvent#VK_KANA_LOCK VK_KANA_LOCK"/>.
		/// </summary>
		/// <exception cref="java.lang.IllegalArgumentException"> if <code>keyCode</code>
		/// is not one of the valid key codes </exception>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the host system doesn't
		/// allow getting the state of this key programmatically, or if the keyboard
		/// doesn't have this key </exception>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getLockingKeyState(int keyCode) throws UnsupportedOperationException
		public virtual bool GetLockingKeyState(int keyCode)
		{
			GraphicsEnvironment.CheckHeadless();

			if (!(keyCode == KeyEvent.VK_CAPS_LOCK || keyCode == KeyEvent.VK_NUM_LOCK || keyCode == KeyEvent.VK_SCROLL_LOCK || keyCode == KeyEvent.VK_KANA_LOCK))
			{
				throw new IllegalArgumentException("invalid key for Toolkit.getLockingKeyState");
			}
			throw new UnsupportedOperationException("Toolkit.getLockingKeyState");
		}

		/// <summary>
		/// Sets the state of the given locking key on the keyboard.
		/// Valid key codes are
		/// <seealso cref="java.awt.event.KeyEvent#VK_CAPS_LOCK VK_CAPS_LOCK"/>,
		/// <seealso cref="java.awt.event.KeyEvent#VK_NUM_LOCK VK_NUM_LOCK"/>,
		/// <seealso cref="java.awt.event.KeyEvent#VK_SCROLL_LOCK VK_SCROLL_LOCK"/>, and
		/// <seealso cref="java.awt.event.KeyEvent#VK_KANA_LOCK VK_KANA_LOCK"/>.
		/// <para>
		/// Depending on the platform, setting the state of a locking key may
		/// involve event processing and therefore may not be immediately
		/// observable through getLockingKeyState.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="java.lang.IllegalArgumentException"> if <code>keyCode</code>
		/// is not one of the valid key codes </exception>
		/// <exception cref="java.lang.UnsupportedOperationException"> if the host system doesn't
		/// allow setting the state of this key programmatically, or if the keyboard
		/// doesn't have this key </exception>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setLockingKeyState(int keyCode, boolean on) throws UnsupportedOperationException
		public virtual void SetLockingKeyState(int keyCode, bool on)
		{
			GraphicsEnvironment.CheckHeadless();

			if (!(keyCode == KeyEvent.VK_CAPS_LOCK || keyCode == KeyEvent.VK_NUM_LOCK || keyCode == KeyEvent.VK_SCROLL_LOCK || keyCode == KeyEvent.VK_KANA_LOCK))
			{
				throw new IllegalArgumentException("invalid key for Toolkit.setLockingKeyState");
			}
			throw new UnsupportedOperationException("Toolkit.setLockingKeyState");
		}

		/// <summary>
		/// Give native peers the ability to query the native container
		/// given a native component (eg the direct parent may be lightweight).
		/// </summary>
		protected internal static Container GetNativeContainer(Component c)
		{
			return c.NativeContainer;
		}

		/// <summary>
		/// Creates a new custom cursor object.
		/// If the image to display is invalid, the cursor will be hidden (made
		/// completely transparent), and the hotspot will be set to (0, 0).
		/// 
		/// <para>Note that multi-frame images are invalid and may cause this
		/// method to hang.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cursor"> the image to display when the cursor is activated </param>
		/// <param name="hotSpot"> the X and Y of the large cursor's hot spot; the
		///   hotSpot values must be less than the Dimension returned by
		///   <code>getBestCursorSize</code> </param>
		/// <param name="name"> a localized description of the cursor, for Java Accessibility use </param>
		/// <exception cref="IndexOutOfBoundsException"> if the hotSpot values are outside
		///   the bounds of the cursor </exception>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Cursor createCustomCursor(Image cursor, Point hotSpot, String name) throws IndexOutOfBoundsException, HeadlessException
		public virtual Cursor CreateCustomCursor(Image cursor, Point hotSpot, String name)
		{
			// Override to implement custom cursor support.
			if (this != Toolkit.DefaultToolkit)
			{
				return Toolkit.DefaultToolkit.CreateCustomCursor(cursor, hotSpot, name);
			}
			else
			{
				return new Cursor(Cursor.DEFAULT_CURSOR);
			}
		}

		/// <summary>
		/// Returns the supported cursor dimension which is closest to the desired
		/// sizes.  Systems which only support a single cursor size will return that
		/// size regardless of the desired sizes.  Systems which don't support custom
		/// cursors will return a dimension of 0, 0. <para>
		/// Note:  if an image is used whose dimensions don't match a supported size
		/// (as returned by this method), the Toolkit implementation will attempt to
		/// resize the image to a supported size.
		/// Since converting low-resolution images is difficult,
		/// no guarantees are made as to the quality of a cursor image which isn't a
		/// supported size.  It is therefore recommended that this method
		/// be called and an appropriate image used so no image conversion is made.
		/// 
		/// </para>
		/// </summary>
		/// <param name="preferredWidth"> the preferred cursor width the component would like
		/// to use. </param>
		/// <param name="preferredHeight"> the preferred cursor height the component would like
		/// to use. </param>
		/// <returns>    the closest matching supported cursor size, or a dimension of 0,0 if
		/// the Toolkit implementation doesn't support custom cursors. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Dimension getBestCursorSize(int preferredWidth, int preferredHeight) throws HeadlessException
		public virtual Dimension GetBestCursorSize(int preferredWidth, int preferredHeight)
		{
			GraphicsEnvironment.CheckHeadless();

			// Override to implement custom cursor support.
			if (this != Toolkit.DefaultToolkit)
			{
				return Toolkit.DefaultToolkit.GetBestCursorSize(preferredWidth, preferredHeight);
			}
			else
			{
				return new Dimension(0, 0);
			}
		}

		/// <summary>
		/// Returns the maximum number of colors the Toolkit supports in a custom cursor
		/// palette.<para>
		/// Note: if an image is used which has more colors in its palette than
		/// the supported maximum, the Toolkit implementation will attempt to flatten the
		/// palette to the maximum.  Since converting low-resolution images is difficult,
		/// no guarantees are made as to the quality of a cursor image which has more
		/// colors than the system supports.  It is therefore recommended that this method
		/// be called and an appropriate image used so no image conversion is made.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    the maximum number of colors, or zero if custom cursors are not
		/// supported by this Toolkit implementation. </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getMaximumCursorColors() throws HeadlessException
		public virtual int MaximumCursorColors
		{
			get
			{
				GraphicsEnvironment.CheckHeadless();
    
				// Override to implement custom cursor support.
				if (this != Toolkit.DefaultToolkit)
				{
					return Toolkit.DefaultToolkit.MaximumCursorColors;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Returns whether Toolkit supports this state for
		/// <code>Frame</code>s.  This method tells whether the <em>UI
		/// concept</em> of, say, maximization or iconification is
		/// supported.  It will always return false for "compound" states
		/// like <code>Frame.ICONIFIED|Frame.MAXIMIZED_VERT</code>.
		/// In other words, the rule of thumb is that only queries with a
		/// single frame state constant as an argument are meaningful.
		/// <para>Note that supporting a given concept is a platform-
		/// dependent feature. Due to native limitations the Toolkit
		/// object may report a particular state as supported, however at
		/// the same time the Toolkit object will be unable to apply the
		/// state to a given frame.  This circumstance has two following
		/// consequences:
		/// <ul>
		/// <li>Only the return value of {@code false} for the present
		/// method actually indicates that the given state is not
		/// supported. If the method returns {@code true} the given state
		/// may still be unsupported and/or unavailable for a particular
		/// frame.
		/// <li>The developer should consider examining the value of the
		/// <seealso cref="java.awt.event.WindowEvent#getNewState"/> method of the
		/// {@code WindowEvent} received through the {@link
		/// java.awt.event.WindowStateListener}, rather than assuming
		/// that the state given to the {@code setExtendedState()} method
		/// will be definitely applied. For more information see the
		/// documentation for the <seealso cref="Frame#setExtendedState"/> method.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="state"> one of named frame state constants. </param>
		/// <returns> <code>true</code> is this frame state is supported by
		///     this Toolkit implementation, <code>false</code> otherwise. </returns>
		/// <exception cref="HeadlessException">
		///     if <code>GraphicsEnvironment.isHeadless()</code>
		///     returns <code>true</code>. </exception>
		/// <seealso cref= java.awt.Window#addWindowStateListener
		/// @since   1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isFrameStateSupported(int state) throws HeadlessException
		public virtual bool IsFrameStateSupported(int state)
		{
			GraphicsEnvironment.CheckHeadless();

			if (this != Toolkit.DefaultToolkit)
			{
				return Toolkit.DefaultToolkit.IsFrameStateSupported(state);
			}
			else
			{
				return (state == Frame.NORMAL); // others are not guaranteed
			}
		}

		/// <summary>
		/// Support for I18N: any visible strings should be stored in
		/// sun.awt.resources.awt.properties.  The ResourceBundle is stored
		/// here, so that only one copy is maintained.
		/// </summary>
		private static ResourceBundle Resources;
		private static ResourceBundle PlatformResources_Renamed;

		// called by platform toolkit
		private static ResourceBundle PlatformResources
		{
			set
			{
				PlatformResources_Renamed = value;
			}
		}

		/// <summary>
		/// Initialize JNI field and method ids
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		/// <summary>
		/// WARNING: This is a temporary workaround for a problem in the
		/// way the AWT loads native libraries. A number of classes in the
		/// AWT package have a native method, initIDs(), which initializes
		/// the JNI field and method ids used in the native portion of
		/// their implementation.
		/// 
		/// Since the use and storage of these ids is done by the
		/// implementation libraries, the implementation of these method is
		/// provided by the particular AWT implementations (for example,
		/// "Toolkit"s/Peer), such as Motif, Microsoft Windows, or Tiny. The
		/// problem is that this means that the native libraries must be
		/// loaded by the java.* classes, which do not necessarily know the
		/// names of the libraries to load. A better way of doing this
		/// would be to provide a separate library which defines java.awt.*
		/// initIDs, and exports the relevant symbols out to the
		/// implementation libraries.
		/// 
		/// For now, we know it's done by the implementation, and we assume
		/// that the name of the library is "awt".  -br.
		/// 
		/// If you change loadLibraries(), please add the change to
		/// java.awt.image.ColorModel.loadLibraries(). Unfortunately,
		/// classes can be loaded in java.awt.image that depend on
		/// libawt and there is no way to call Toolkit.loadLibraries()
		/// directly.  -hung
		/// </summary>
		private static bool Loaded = false;
		internal static void LoadLibraries()
		{
			if (!Loaded)
			{
				java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper3());
				Loaded = true;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper3 : java.security.PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper3()
			{
			}

			public virtual Void Run()
			{
//JAVA TO C# CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
//				System.loadLibrary("awt");
				return null;
			}
		}

		static Toolkit()
		{
			AWTAccessor.ToolkitAccessor = new ToolkitAccessorAnonymousInnerClassHelper();

			java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper4());

			// ensure that the proper libraries are loaded
			LoadLibraries();
			InitAssistiveTechnologies();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		private class ToolkitAccessorAnonymousInnerClassHelper : AWTAccessor.ToolkitAccessor
		{
			public ToolkitAccessorAnonymousInnerClassHelper()
			{
			}

			public override ResourceBundle PlatformResources
			{
				set
				{
					Toolkit.PlatformResources = value;
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper4 : java.security.PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper4()
			{
			}

			public virtual Void Run()
			{
				try
				{
					Resources = ResourceBundle.GetBundle("sun.awt.resources.awt", CoreResourceBundleControl.RBControlInstance);
				}
				catch (MissingResourceException)
				{
					// No resource file; defaults will be used.
				}
				return null;
			}
		}

		/// <summary>
		/// Gets a property with the specified key and default.
		/// This method returns defaultValue if the property is not found.
		/// </summary>
		public static String GetProperty(String key, String defaultValue)
		{
			// first try platform specific bundle
			if (PlatformResources_Renamed != null)
			{
				try
				{
					return PlatformResources_Renamed.GetString(key);
				}
				catch (MissingResourceException)
				{
				}
			}

			// then shared one
			if (Resources != null)
			{
				try
				{
					return Resources.GetString(key);
				}
				catch (MissingResourceException)
				{
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Get the application's or applet's EventQueue instance.
		/// Depending on the Toolkit implementation, different EventQueues
		/// may be returned for different applets.  Applets should
		/// therefore not assume that the EventQueue instance returned
		/// by this method will be shared by other applets or the system.
		/// 
		/// <para> If there is a security manager then its
		/// <seealso cref="SecurityManager#checkPermission checkPermission"/> method
		/// is called to check {@code AWTPermission("accessEventQueue")}.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    the <code>EventQueue</code> object </returns>
		/// <exception cref="SecurityException">
		///          if a security manager is set and it denies access to
		///          the {@code EventQueue} </exception>
		/// <seealso cref=     java.awt.AWTPermission </seealso>
		public EventQueue SystemEventQueue
		{
			get
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckPermission(SecurityConstants.AWT.CHECK_AWT_EVENTQUEUE_PERMISSION);
				}
				return SystemEventQueueImpl;
			}
		}

		/// <summary>
		/// Gets the application's or applet's <code>EventQueue</code>
		/// instance, without checking access.  For security reasons,
		/// this can only be called from a <code>Toolkit</code> subclass. </summary>
		/// <returns> the <code>EventQueue</code> object </returns>
		protected internal abstract EventQueue SystemEventQueueImpl {get;}

		/* Accessor method for use by AWT package routines. */
		internal static EventQueue EventQueue
		{
			get
			{
				return DefaultToolkit.SystemEventQueueImpl;
			}
		}

		/// <summary>
		/// Creates the peer for a DragSourceContext.
		/// Always throws InvalidDndOperationException if
		/// GraphicsEnvironment.isHeadless() returns true. </summary>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.awt.dnd.peer.DragSourceContextPeer createDragSourceContextPeer(java.awt.dnd.DragGestureEvent dge) throws java.awt.dnd.InvalidDnDOperationException;
		public abstract DragSourceContextPeer CreateDragSourceContextPeer(DragGestureEvent dge);

		/// <summary>
		/// Creates a concrete, platform dependent, subclass of the abstract
		/// DragGestureRecognizer class requested, and associates it with the
		/// DragSource, Component and DragGestureListener specified.
		/// 
		/// subclasses should override this to provide their own implementation
		/// </summary>
		/// <param name="abstractRecognizerClass"> The abstract class of the required recognizer </param>
		/// <param name="ds">                      The DragSource </param>
		/// <param name="c">                       The Component target for the DragGestureRecognizer </param>
		/// <param name="srcActions">              The actions permitted for the gesture </param>
		/// <param name="dgl">                     The DragGestureListener
		/// </param>
		/// <returns> the new object or null.  Always returns null if
		/// GraphicsEnvironment.isHeadless() returns true. </returns>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		public virtual T createDragGestureRecognizer<T>(Class abstractRecognizerClass, DragSource ds, Component c, int srcActions, DragGestureListener dgl) where T : java.awt.dnd.DragGestureRecognizer
		{
			return null;
		}

		/// <summary>
		/// Obtains a value for the specified desktop property.
		/// 
		/// A desktop property is a uniquely named value for a resource that
		/// is Toolkit global in nature. Usually it also is an abstract
		/// representation for an underlying platform dependent desktop setting.
		/// For more information on desktop properties supported by the AWT see
		/// <a href="doc-files/DesktopProperties.html">AWT Desktop Properties</a>.
		/// </summary>
		public Object GetDesktopProperty(String propertyName)
		{
			lock (this)
			{
				// This is a workaround for headless toolkits.  It would be
				// better to override this method but it is declared final.
				// "this instanceof" syntax defeats polymorphism.
				// --mm, 03/03/00
				if (this is HeadlessToolkit)
				{
					return ((HeadlessToolkit)this).UnderlyingToolkit.getDesktopProperty(propertyName);
				}
        
				if (DesktopProperties.Empty)
				{
					InitializeDesktopProperties();
				}
        
				Object value;
        
				// This property should never be cached
				if (propertyName.Equals("awt.dynamicLayoutSupported"))
				{
					return DefaultToolkit.LazilyLoadDesktopProperty(propertyName);
				}
        
				value = DesktopProperties.Get(propertyName);
        
				if (value == null)
				{
					value = LazilyLoadDesktopProperty(propertyName);
        
					if (value != null)
					{
						SetDesktopProperty(propertyName, value);
					}
				}
        
				/* for property "awt.font.desktophints" */
				if (value is RenderingHints)
				{
					value = ((RenderingHints)value).Clone();
				}
        
				return value;
			}
		}

		/// <summary>
		/// Sets the named desktop property to the specified value and fires a
		/// property change event to notify any listeners that the value has changed.
		/// </summary>
		protected internal void SetDesktopProperty(String name, Object newValue)
		{
			// This is a workaround for headless toolkits.  It would be
			// better to override this method but it is declared final.
			// "this instanceof" syntax defeats polymorphism.
			// --mm, 03/03/00
			if (this is HeadlessToolkit)
			{
				((HeadlessToolkit)this).UnderlyingToolkit.setDesktopProperty(name, newValue);
				return;
			}
			Object oldValue;

			lock (this)
			{
				oldValue = DesktopProperties.Get(name);
				DesktopProperties.Put(name, newValue);
			}

			// Don't fire change event if old and new values are null.
			// It helps to avoid recursive resending of WM_THEMECHANGED
			if (oldValue != null || newValue != null)
			{
				DesktopPropsSupport.FirePropertyChange(name, oldValue, newValue);
			}
		}

		/// <summary>
		/// an opportunity to lazily evaluate desktop property values.
		/// </summary>
		protected internal virtual Object LazilyLoadDesktopProperty(String name)
		{
			return null;
		}

		/// <summary>
		/// initializeDesktopProperties
		/// </summary>
		protected internal virtual void InitializeDesktopProperties()
		{
		}

		/// <summary>
		/// Adds the specified property change listener for the named desktop
		/// property. When a <seealso cref="java.beans.PropertyChangeListenerProxy"/> object is added,
		/// its property name is ignored, and the wrapped listener is added.
		/// If {@code name} is {@code null} or {@code pcl} is {@code null},
		/// no exception is thrown and no action is performed.
		/// </summary>
		/// <param name="name"> The name of the property to listen for </param>
		/// <param name="pcl"> The property change listener </param>
		/// <seealso cref= PropertyChangeSupport#addPropertyChangeListener(String,
		///            PropertyChangeListener)
		/// @since   1.2 </seealso>
		public virtual void AddPropertyChangeListener(String name, PropertyChangeListener pcl)
		{
			DesktopPropsSupport.AddPropertyChangeListener(name, pcl);
		}

		/// <summary>
		/// Removes the specified property change listener for the named
		/// desktop property. When a <seealso cref="java.beans.PropertyChangeListenerProxy"/> object
		/// is removed, its property name is ignored, and
		/// the wrapped listener is removed.
		/// If {@code name} is {@code null} or {@code pcl} is {@code null},
		/// no exception is thrown and no action is performed.
		/// </summary>
		/// <param name="name"> The name of the property to remove </param>
		/// <param name="pcl"> The property change listener </param>
		/// <seealso cref= PropertyChangeSupport#removePropertyChangeListener(String,
		///            PropertyChangeListener)
		/// @since   1.2 </seealso>
		public virtual void RemovePropertyChangeListener(String name, PropertyChangeListener pcl)
		{
			DesktopPropsSupport.RemovePropertyChangeListener(name, pcl);
		}

		/// <summary>
		/// Returns an array of all the property change listeners
		/// registered on this toolkit. The returned array
		/// contains <seealso cref="java.beans.PropertyChangeListenerProxy"/> objects
		/// that associate listeners with the names of desktop properties.
		/// </summary>
		/// <returns> all of this toolkit's <seealso cref="PropertyChangeListener"/>
		///         objects wrapped in {@code java.beans.PropertyChangeListenerProxy} objects
		///         or an empty array  if no listeners are added
		/// </returns>
		/// <seealso cref= PropertyChangeSupport#getPropertyChangeListeners()
		/// @since 1.4 </seealso>
		public virtual PropertyChangeListener[] PropertyChangeListeners
		{
			get
			{
				return DesktopPropsSupport.PropertyChangeListeners;
			}
		}

		/// <summary>
		/// Returns an array of all property change listeners
		/// associated with the specified name of a desktop property.
		/// </summary>
		/// <param name="propertyName"> the named property </param>
		/// <returns> all of the {@code PropertyChangeListener} objects
		///         associated with the specified name of a desktop property
		///         or an empty array if no such listeners are added
		/// </returns>
		/// <seealso cref= PropertyChangeSupport#getPropertyChangeListeners(String)
		/// @since 1.4 </seealso>
		public virtual PropertyChangeListener[] GetPropertyChangeListeners(String propertyName)
		{
			return DesktopPropsSupport.GetPropertyChangeListeners(propertyName);
		}

		protected internal readonly Map<String, Object> DesktopProperties = new HashMap<String, Object>();
		protected internal PropertyChangeSupport DesktopPropsSupport;

		/// <summary>
		/// Returns whether the always-on-top mode is supported by this toolkit.
		/// To detect whether the always-on-top mode is supported for a
		/// particular Window, use <seealso cref="Window#isAlwaysOnTopSupported"/>. </summary>
		/// <returns> <code>true</code>, if current toolkit supports the always-on-top mode,
		///     otherwise returns <code>false</code> </returns>
		/// <seealso cref= Window#isAlwaysOnTopSupported </seealso>
		/// <seealso cref= Window#setAlwaysOnTop(boolean)
		/// @since 1.6 </seealso>
		public virtual bool AlwaysOnTopSupported
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Returns whether the given modality type is supported by this toolkit. If
		/// a dialog with unsupported modality type is created, then
		/// <code>Dialog.ModalityType.MODELESS</code> is used instead.
		/// </summary>
		/// <param name="modalityType"> modality type to be checked for support by this toolkit
		/// </param>
		/// <returns> <code>true</code>, if current toolkit supports given modality
		///     type, <code>false</code> otherwise
		/// </returns>
		/// <seealso cref= java.awt.Dialog.ModalityType </seealso>
		/// <seealso cref= java.awt.Dialog#getModalityType </seealso>
		/// <seealso cref= java.awt.Dialog#setModalityType
		/// 
		/// @since 1.6 </seealso>
		public abstract bool IsModalityTypeSupported(Dialog.ModalityType modalityType);

		/// <summary>
		/// Returns whether the given modal exclusion type is supported by this
		/// toolkit. If an unsupported modal exclusion type property is set on a window,
		/// then <code>Dialog.ModalExclusionType.NO_EXCLUDE</code> is used instead.
		/// </summary>
		/// <param name="modalExclusionType"> modal exclusion type to be checked for support by this toolkit
		/// </param>
		/// <returns> <code>true</code>, if current toolkit supports given modal exclusion
		///     type, <code>false</code> otherwise
		/// </returns>
		/// <seealso cref= java.awt.Dialog.ModalExclusionType </seealso>
		/// <seealso cref= java.awt.Window#getModalExclusionType </seealso>
		/// <seealso cref= java.awt.Window#setModalExclusionType
		/// 
		/// @since 1.6 </seealso>
		public abstract bool IsModalExclusionTypeSupported(Dialog.ModalExclusionType modalExclusionType);

		// 8014718: logging has been removed from SunToolkit

		private const int LONG_BITS = 64;
		private int[] Calls = new int[LONG_BITS];
		private static volatile long EnabledOnToolkitMask;
		private AWTEventListener EventListener = null;
		private WeakHashMap<AWTEventListener, SelectiveAWTEventListener> Listener2SelectiveListener = new WeakHashMap<AWTEventListener, SelectiveAWTEventListener>();

		/*
		 * Extracts a "pure" AWTEventListener from a AWTEventListenerProxy,
		 * if the listener is proxied.
		 */
		private static AWTEventListener DeProxyAWTEventListener(AWTEventListener l)
		{
			AWTEventListener localL = l;

			if (localL == null)
			{
				return null;
			}
			// if user passed in a AWTEventListenerProxy object, extract
			// the listener
			if (l is AWTEventListenerProxy)
			{
				localL = ((AWTEventListenerProxy)l).Listener;
			}
			return localL;
		}

		/// <summary>
		/// Adds an AWTEventListener to receive all AWTEvents dispatched
		/// system-wide that conform to the given <code>eventMask</code>.
		/// <para>
		/// First, if there is a security manager, its <code>checkPermission</code>
		/// method is called with an
		/// <code>AWTPermission("listenToAllAWTEvents")</code> permission.
		/// This may result in a SecurityException.
		/// </para>
		/// <para>
		/// <code>eventMask</code> is a bitmask of event types to receive.
		/// It is constructed by bitwise OR-ing together the event masks
		/// defined in <code>AWTEvent</code>.
		/// </para>
		/// <para>
		/// Note:  event listener use is not recommended for normal
		/// application use, but are intended solely to support special
		/// purpose facilities including support for accessibility,
		/// event record/playback, and diagnostic tracing.
		/// 
		/// If listener is null, no exception is thrown and no action is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">   the event listener. </param>
		/// <param name="eventMask">  the bitmask of event types to receive </param>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        <code>checkPermission</code> method doesn't allow the operation. </exception>
		/// <seealso cref=      #removeAWTEventListener </seealso>
		/// <seealso cref=      #getAWTEventListeners </seealso>
		/// <seealso cref=      SecurityManager#checkPermission </seealso>
		/// <seealso cref=      java.awt.AWTEvent </seealso>
		/// <seealso cref=      java.awt.AWTPermission </seealso>
		/// <seealso cref=      java.awt.event.AWTEventListener </seealso>
		/// <seealso cref=      java.awt.event.AWTEventListenerProxy
		/// @since    1.2 </seealso>
		public virtual void AddAWTEventListener(AWTEventListener listener, long eventMask)
		{
			AWTEventListener localL = DeProxyAWTEventListener(listener);

			if (localL == null)
			{
				return;
			}
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
			  security.CheckPermission(SecurityConstants.AWT.ALL_AWT_EVENTS_PERMISSION);
			}
			lock (this)
			{
				SelectiveAWTEventListener selectiveListener = Listener2SelectiveListener.Get(localL);

				if (selectiveListener == null)
				{
					// Create a new selectiveListener.
					selectiveListener = new SelectiveAWTEventListener(this, localL, eventMask);
					Listener2SelectiveListener.Put(localL, selectiveListener);
					EventListener = ToolkitEventMulticaster.Add(EventListener, selectiveListener);
				}
				// OR the eventMask into the selectiveListener's event mask.
				selectiveListener.OrEventMasks(eventMask);

				EnabledOnToolkitMask |= eventMask;

				long mask = eventMask;
				for (int i = 0; i < LONG_BITS; i++)
				{
					// If no bits are set, break out of loop.
					if (mask == 0)
					{
						break;
					}
					if ((mask & 1L) != 0) // Always test bit 0.
					{
						Calls[i]++;
					}
					mask = (long)((ulong)mask >> 1); // Right shift, fill with zeros on left.
				}
			}
		}

		/// <summary>
		/// Removes an AWTEventListener from receiving dispatched AWTEvents.
		/// <para>
		/// First, if there is a security manager, its <code>checkPermission</code>
		/// method is called with an
		/// <code>AWTPermission("listenToAllAWTEvents")</code> permission.
		/// This may result in a SecurityException.
		/// </para>
		/// <para>
		/// Note:  event listener use is not recommended for normal
		/// application use, but are intended solely to support special
		/// purpose facilities including support for accessibility,
		/// event record/playback, and diagnostic tracing.
		/// 
		/// If listener is null, no exception is thrown and no action is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">   the event listener. </param>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        <code>checkPermission</code> method doesn't allow the operation. </exception>
		/// <seealso cref=      #addAWTEventListener </seealso>
		/// <seealso cref=      #getAWTEventListeners </seealso>
		/// <seealso cref=      SecurityManager#checkPermission </seealso>
		/// <seealso cref=      java.awt.AWTEvent </seealso>
		/// <seealso cref=      java.awt.AWTPermission </seealso>
		/// <seealso cref=      java.awt.event.AWTEventListener </seealso>
		/// <seealso cref=      java.awt.event.AWTEventListenerProxy
		/// @since    1.2 </seealso>
		public virtual void RemoveAWTEventListener(AWTEventListener listener)
		{
			AWTEventListener localL = DeProxyAWTEventListener(listener);

			if (listener == null)
			{
				return;
			}
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckPermission(SecurityConstants.AWT.ALL_AWT_EVENTS_PERMISSION);
			}

			lock (this)
			{
				SelectiveAWTEventListener selectiveListener = Listener2SelectiveListener.Get(localL);

				if (selectiveListener != null)
				{
					Listener2SelectiveListener.Remove(localL);
					int[] listenerCalls = selectiveListener.Calls;
					for (int i = 0; i < LONG_BITS; i++)
					{
						Calls[i] -= listenerCalls[i];
						Debug.Assert(Calls[i] >= 0, "Negative Listeners count");

						if (Calls[i] == 0)
						{
							EnabledOnToolkitMask &= ~(1L << i);
						}
					}
				}
				EventListener = ToolkitEventMulticaster.Remove(EventListener, (selectiveListener == null) ? localL : selectiveListener);
			}
		}

		internal static bool EnabledOnToolkit(long eventMask)
		{
			return (EnabledOnToolkitMask & eventMask) != 0;
		}

		internal virtual int CountAWTEventListeners(long eventMask)
		{
			lock (this)
			{
				int ci = 0;
				for (; eventMask != 0; eventMask = (long)((ulong)eventMask >> 1), ci++)
				{
				}
				ci--;
				return Calls[ci];
			}
		}
		/// <summary>
		/// Returns an array of all the <code>AWTEventListener</code>s
		/// registered on this toolkit.
		/// If there is a security manager, its {@code checkPermission}
		/// method is called with an
		/// {@code AWTPermission("listenToAllAWTEvents")} permission.
		/// This may result in a SecurityException.
		/// Listeners can be returned
		/// within <code>AWTEventListenerProxy</code> objects, which also contain
		/// the event mask for the given listener.
		/// Note that listener objects
		/// added multiple times appear only once in the returned array.
		/// </summary>
		/// <returns> all of the <code>AWTEventListener</code>s or an empty
		///         array if no listeners are currently registered </returns>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        <code>checkPermission</code> method doesn't allow the operation. </exception>
		/// <seealso cref=      #addAWTEventListener </seealso>
		/// <seealso cref=      #removeAWTEventListener </seealso>
		/// <seealso cref=      SecurityManager#checkPermission </seealso>
		/// <seealso cref=      java.awt.AWTEvent </seealso>
		/// <seealso cref=      java.awt.AWTPermission </seealso>
		/// <seealso cref=      java.awt.event.AWTEventListener </seealso>
		/// <seealso cref=      java.awt.event.AWTEventListenerProxy
		/// @since 1.4 </seealso>
		public virtual AWTEventListener[] AWTEventListeners
		{
			get
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckPermission(SecurityConstants.AWT.ALL_AWT_EVENTS_PERMISSION);
				}
				lock (this)
				{
					EventListener[] la = ToolkitEventMulticaster.GetListeners(EventListener,typeof(AWTEventListener));
    
					AWTEventListener[] ret = new AWTEventListener[la.Length];
					for (int i = 0; i < la.Length; i++)
					{
						SelectiveAWTEventListener sael = (SelectiveAWTEventListener)la[i];
						AWTEventListener tempL = sael.Listener;
						//assert tempL is not an AWTEventListenerProxy - we should
						// have weeded them all out
						// don't want to wrap a proxy inside a proxy
						ret[i] = new AWTEventListenerProxy(sael.EventMask, tempL);
					}
					return ret;
				}
			}
		}

		/// <summary>
		/// Returns an array of all the <code>AWTEventListener</code>s
		/// registered on this toolkit which listen to all of the event
		/// types specified in the {@code eventMask} argument.
		/// If there is a security manager, its {@code checkPermission}
		/// method is called with an
		/// {@code AWTPermission("listenToAllAWTEvents")} permission.
		/// This may result in a SecurityException.
		/// Listeners can be returned
		/// within <code>AWTEventListenerProxy</code> objects, which also contain
		/// the event mask for the given listener.
		/// Note that listener objects
		/// added multiple times appear only once in the returned array.
		/// </summary>
		/// <param name="eventMask"> the bitmask of event types to listen for </param>
		/// <returns> all of the <code>AWTEventListener</code>s registered
		///         on this toolkit for the specified
		///         event types, or an empty array if no such listeners
		///         are currently registered </returns>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        <code>checkPermission</code> method doesn't allow the operation. </exception>
		/// <seealso cref=      #addAWTEventListener </seealso>
		/// <seealso cref=      #removeAWTEventListener </seealso>
		/// <seealso cref=      SecurityManager#checkPermission </seealso>
		/// <seealso cref=      java.awt.AWTEvent </seealso>
		/// <seealso cref=      java.awt.AWTPermission </seealso>
		/// <seealso cref=      java.awt.event.AWTEventListener </seealso>
		/// <seealso cref=      java.awt.event.AWTEventListenerProxy
		/// @since 1.4 </seealso>
		public virtual AWTEventListener[] GetAWTEventListeners(long eventMask)
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckPermission(SecurityConstants.AWT.ALL_AWT_EVENTS_PERMISSION);
			}
			lock (this)
			{
				EventListener[] la = ToolkitEventMulticaster.GetListeners(EventListener,typeof(AWTEventListener));

				IList<AWTEventListenerProxy> list = new List<AWTEventListenerProxy>(la.Length);

				for (int i = 0; i < la.Length; i++)
				{
					SelectiveAWTEventListener sael = (SelectiveAWTEventListener)la[i];
					if ((sael.EventMask & eventMask) == eventMask)
					{
						//AWTEventListener tempL = sael.getListener();
						list.Add(new AWTEventListenerProxy(sael.EventMask, sael.Listener));
					}
				}
				return list.ToArray();
			}
		}

		/*
		 * This method notifies any AWTEventListeners that an event
		 * is about to be dispatched.
		 *
		 * @param theEvent the event which will be dispatched.
		 */
		internal virtual void NotifyAWTEventListeners(AWTEvent theEvent)
		{
			// This is a workaround for headless toolkits.  It would be
			// better to override this method but it is declared package private.
			// "this instanceof" syntax defeats polymorphism.
			// --mm, 03/03/00
			if (this is HeadlessToolkit)
			{
				((HeadlessToolkit)this).UnderlyingToolkit.notifyAWTEventListeners(theEvent);
				return;
			}

			AWTEventListener eventListener = this.EventListener;
			if (eventListener != null)
			{
				eventListener.EventDispatched(theEvent);
			}
		}

		private class ToolkitEventMulticaster : AWTEventMulticaster, AWTEventListener
		{
			// Implementation cloned from AWTEventMulticaster.

			internal ToolkitEventMulticaster(AWTEventListener a, AWTEventListener b) : base(a, b)
			{
			}

			internal static AWTEventListener Add(AWTEventListener a, AWTEventListener b)
			{
				if (a == null)
				{
					return b;
				}
				if (b == null)
				{
					return a;
				}
				return new ToolkitEventMulticaster(a, b);
			}

			internal static AWTEventListener Remove(AWTEventListener l, AWTEventListener oldl)
			{
				return (AWTEventListener) RemoveInternal(l, oldl);
			}

			// #4178589: must overload remove(EventListener) to call our add()
			// instead of the static addInternal() so we allocate a
			// ToolkitEventMulticaster instead of an AWTEventMulticaster.
			// Note: this method is called by AWTEventListener.removeInternal(),
			// so its method signature must match AWTEventListener.remove().
			protected internal override EventListener Remove(EventListener oldl)
			{
				if (oldl == a)
				{
					return b;
				}
				if (oldl == b)
				{
					return a;
				}
				AWTEventListener a2 = (AWTEventListener)RemoveInternal(a, oldl);
				AWTEventListener b2 = (AWTEventListener)RemoveInternal(b, oldl);
				if (a2 == a && b2 == b)
				{
					return this; // it's not here
				}
				return Add(a2, b2);
			}

			public virtual void EventDispatched(AWTEvent @event)
			{
				((AWTEventListener)a).EventDispatched(@event);
				((AWTEventListener)b).EventDispatched(@event);
			}
		}

		private class SelectiveAWTEventListener : AWTEventListener
		{
			private readonly Toolkit OuterInstance;

			internal AWTEventListener Listener_Renamed;
			internal long EventMask_Renamed;
			// This array contains the number of times to call the eventlistener
			// for each event type.
			internal int[] Calls_Renamed = new int[Toolkit.LONG_BITS];

			public virtual AWTEventListener Listener
			{
				get
				{
					return Listener_Renamed;
				}
			}
			public virtual long EventMask
			{
				get
				{
					return EventMask_Renamed;
				}
			}
			public virtual int[] Calls
			{
				get
				{
					return Calls_Renamed;
				}
			}

			public virtual void OrEventMasks(long mask)
			{
				EventMask_Renamed |= mask;
				// For each event bit set in mask, increment its call count.
				for (int i = 0; i < Toolkit.LONG_BITS; i++)
				{
					// If no bits are set, break out of loop.
					if (mask == 0)
					{
						break;
					}
					if ((mask & 1L) != 0) // Always test bit 0.
					{
						Calls_Renamed[i]++;
					}
					mask = (long)((ulong)mask >> 1); // Right shift, fill with zeros on left.
				}
			}

			internal SelectiveAWTEventListener(Toolkit outerInstance, AWTEventListener l, long mask)
			{
				this.OuterInstance = outerInstance;
				Listener_Renamed = l;
				EventMask_Renamed = mask;
			}

			public virtual void EventDispatched(AWTEvent @event)
			{
				long eventBit = 0; // Used to save the bit of the event type.
				if (((eventBit = EventMask_Renamed & AWTEvent.COMPONENT_EVENT_MASK) != 0 && @event.Id >= ComponentEvent.COMPONENT_FIRST && @event.Id <= ComponentEvent.COMPONENT_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.CONTAINER_EVENT_MASK) != 0 && @event.Id >= ContainerEvent.CONTAINER_FIRST && @event.Id <= ContainerEvent.CONTAINER_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.FOCUS_EVENT_MASK) != 0 && @event.Id >= FocusEvent.FOCUS_FIRST && @event.Id <= FocusEvent.FOCUS_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.KEY_EVENT_MASK) != 0 && @event.Id >= KeyEvent.KEY_FIRST && @event.Id <= KeyEvent.KEY_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.MOUSE_WHEEL_EVENT_MASK) != 0 && @event.Id == MouseEvent.MOUSE_WHEEL) || ((eventBit = EventMask_Renamed & AWTEvent.MOUSE_MOTION_EVENT_MASK) != 0 && (@event.Id == MouseEvent.MOUSE_MOVED || @event.Id == MouseEvent.MOUSE_DRAGGED)) || ((eventBit = EventMask_Renamed & AWTEvent.MOUSE_EVENT_MASK) != 0 && @event.Id != MouseEvent.MOUSE_MOVED && @event.Id != MouseEvent.MOUSE_DRAGGED && @event.Id != MouseEvent.MOUSE_WHEEL && @event.Id >= MouseEvent.MOUSE_FIRST && @event.Id <= MouseEvent.MOUSE_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.WINDOW_EVENT_MASK) != 0 && (@event.Id >= WindowEvent.WINDOW_FIRST && @event.Id <= WindowEvent.WINDOW_LAST)) || ((eventBit = EventMask_Renamed & AWTEvent.ACTION_EVENT_MASK) != 0 && @event.Id >= ActionEvent.ACTION_FIRST && @event.Id <= ActionEvent.ACTION_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.ADJUSTMENT_EVENT_MASK) != 0 && @event.Id >= AdjustmentEvent.ADJUSTMENT_FIRST && @event.Id <= AdjustmentEvent.ADJUSTMENT_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.ITEM_EVENT_MASK) != 0 && @event.Id >= ItemEvent.ITEM_FIRST && @event.Id <= ItemEvent.ITEM_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.TEXT_EVENT_MASK) != 0 && @event.Id >= TextEvent.TEXT_FIRST && @event.Id <= TextEvent.TEXT_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.INPUT_METHOD_EVENT_MASK) != 0 && @event.Id >= InputMethodEvent.INPUT_METHOD_FIRST && @event.Id <= InputMethodEvent.INPUT_METHOD_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.PAINT_EVENT_MASK) != 0 && @event.Id >= PaintEvent.PAINT_FIRST && @event.Id <= PaintEvent.PAINT_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.INVOCATION_EVENT_MASK) != 0 && @event.Id >= InvocationEvent.INVOCATION_FIRST && @event.Id <= InvocationEvent.INVOCATION_LAST) || ((eventBit = EventMask_Renamed & AWTEvent.HIERARCHY_EVENT_MASK) != 0 && @event.Id == HierarchyEvent.HIERARCHY_CHANGED) || ((eventBit = EventMask_Renamed & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) != 0 && (@event.Id == HierarchyEvent.ANCESTOR_MOVED || @event.Id == HierarchyEvent.ANCESTOR_RESIZED)) || ((eventBit = EventMask_Renamed & AWTEvent.WINDOW_STATE_EVENT_MASK) != 0 && @event.Id == WindowEvent.WINDOW_STATE_CHANGED) || ((eventBit = EventMask_Renamed & AWTEvent.WINDOW_FOCUS_EVENT_MASK) != 0 && (@event.Id == WindowEvent.WINDOW_GAINED_FOCUS || @event.Id == WindowEvent.WINDOW_LOST_FOCUS)) || ((eventBit = EventMask_Renamed & SunToolkit.GRAB_EVENT_MASK) != 0 && (@event is sun.awt.UngrabEvent)))
				{
					// Get the index of the call count for this event type.
					// Instead of using Math.log(...) we will calculate it with
					// bit shifts. That's what previous implementation looked like:
					//
					// int ci = (int) (Math.log(eventBit)/Math.log(2));
					int ci = 0;
					for (long eMask = eventBit; eMask != 0; eMask = (long)((ulong)eMask >> 1), ci++)
					{
					}
					ci--;
					// Call the listener as many times as it was added for this
					// event type.
					for (int i = 0; i < Calls_Renamed[ci]; i++)
					{
						Listener_Renamed.EventDispatched(@event);
					}
				}
			}
		}

		/// <summary>
		/// Returns a map of visual attributes for the abstract level description
		/// of the given input method highlight, or null if no mapping is found.
		/// The style field of the input method highlight is ignored. The map
		/// returned is unmodifiable. </summary>
		/// <param name="highlight"> input method highlight </param>
		/// <returns> style attribute map, or <code>null</code> </returns>
		/// <exception cref="HeadlessException"> if
		///     <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Map<java.awt.font.TextAttribute,?> mapInputMethodHighlight(java.awt.im.InputMethodHighlight highlight) throws HeadlessException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public abstract Map<java.awt.font.TextAttribute, ?> MapInputMethodHighlight(InputMethodHighlight highlight);

		private static PropertyChangeSupport CreatePropertyChangeSupport(Toolkit toolkit)
		{
			if (toolkit is SunToolkit || toolkit is HeadlessToolkit)
			{
				return new DesktopPropertyChangeSupport(toolkit);
			}
			else
			{
				return new PropertyChangeSupport(toolkit);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") private static class DesktopPropertyChangeSupport extends java.beans.PropertyChangeSupport
		private class DesktopPropertyChangeSupport : PropertyChangeSupport
		{

			internal static readonly StringBuilder PROP_CHANGE_SUPPORT_KEY = new StringBuilder("desktop property change support key");
			internal readonly Object Source;

			public DesktopPropertyChangeSupport(Object sourceBean) : base(sourceBean)
			{
				Source = sourceBean;
			}

			public override void AddPropertyChangeListener(String propertyName, PropertyChangeListener listener)
			{
				lock (this)
				{
					PropertyChangeSupport pcs = (PropertyChangeSupport) AppContext.AppContext.get(PROP_CHANGE_SUPPORT_KEY);
					if (null == pcs)
					{
						pcs = new PropertyChangeSupport(Source);
						AppContext.AppContext.put(PROP_CHANGE_SUPPORT_KEY, pcs);
					}
					pcs.AddPropertyChangeListener(propertyName, listener);
				}
			}

			public override void RemovePropertyChangeListener(String propertyName, PropertyChangeListener listener)
			{
				lock (this)
				{
					PropertyChangeSupport pcs = (PropertyChangeSupport) AppContext.AppContext.get(PROP_CHANGE_SUPPORT_KEY);
					if (null != pcs)
					{
						pcs.RemovePropertyChangeListener(propertyName, listener);
					}
				}
			}

			public override PropertyChangeListener[] PropertyChangeListeners
			{
				get
				{
					lock (this)
					{
						PropertyChangeSupport pcs = (PropertyChangeSupport) AppContext.AppContext.get(PROP_CHANGE_SUPPORT_KEY);
						if (null != pcs)
						{
							return pcs.PropertyChangeListeners;
						}
						else
						{
							return new PropertyChangeListener[0];
						}
					}
				}
			}

			public override PropertyChangeListener[] GetPropertyChangeListeners(String propertyName)
			{
				lock (this)
				{
					PropertyChangeSupport pcs = (PropertyChangeSupport) AppContext.AppContext.get(PROP_CHANGE_SUPPORT_KEY);
					if (null != pcs)
					{
						return pcs.GetPropertyChangeListeners(propertyName);
					}
					else
					{
						return new PropertyChangeListener[0];
					}
				}
			}

			public override void AddPropertyChangeListener(PropertyChangeListener listener)
			{
				lock (this)
				{
					PropertyChangeSupport pcs = (PropertyChangeSupport) AppContext.AppContext.get(PROP_CHANGE_SUPPORT_KEY);
					if (null == pcs)
					{
						pcs = new PropertyChangeSupport(Source);
						AppContext.AppContext.put(PROP_CHANGE_SUPPORT_KEY, pcs);
					}
					pcs.AddPropertyChangeListener(listener);
				}
			}

			public override void RemovePropertyChangeListener(PropertyChangeListener listener)
			{
				lock (this)
				{
					PropertyChangeSupport pcs = (PropertyChangeSupport) AppContext.AppContext.get(PROP_CHANGE_SUPPORT_KEY);
					if (null != pcs)
					{
						pcs.RemovePropertyChangeListener(listener);
					}
				}
			}

			/*
			 * we do expect that all other fireXXX() methods of java.beans.PropertyChangeSupport
			 * use this method.  If this will be changed we will need to change this class.
			 */
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void firePropertyChange(final java.beans.PropertyChangeEvent evt)
			public override void FirePropertyChange(PropertyChangeEvent evt)
			{
				Object oldValue = evt.OldValue;
				Object newValue = evt.NewValue;
				String propertyName = evt.PropertyName;
				if (oldValue != null && newValue != null && oldValue.Equals(newValue))
				{
					return;
				}
				Runnable updater = new RunnableAnonymousInnerClassHelper(this, evt);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final sun.awt.AppContext currentAppContext = sun.awt.AppContext.getAppContext();
				AppContext currentAppContext = AppContext.AppContext;
				foreach (AppContext appContext in AppContext.AppContexts)
				{
					if (null == appContext || appContext.Disposed)
					{
						continue;
					}
					if (currentAppContext == appContext)
					{
						updater.Run();
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final sun.awt.PeerEvent e = new sun.awt.PeerEvent(source, updater, sun.awt.PeerEvent.ULTIMATE_PRIORITY_EVENT);
						PeerEvent e = new PeerEvent(Source, updater, PeerEvent.ULTIMATE_PRIORITY_EVENT);
						SunToolkit.postEvent(appContext, e);
					}
				}
			}

			private class RunnableAnonymousInnerClassHelper : Runnable
			{
				private readonly DesktopPropertyChangeSupport OuterInstance;

				private PropertyChangeEvent Evt;

				public RunnableAnonymousInnerClassHelper(DesktopPropertyChangeSupport outerInstance, PropertyChangeEvent evt)
				{
					this.OuterInstance = outerInstance;
					this.Evt = evt;
				}

				public virtual void Run()
				{
					PropertyChangeSupport pcs = (PropertyChangeSupport) AppContext.AppContext.get(PROP_CHANGE_SUPPORT_KEY);
					if (null != pcs)
					{
						pcs.FirePropertyChange(Evt);
					}
				}
			}
		}

		/// <summary>
		/// Reports whether events from extra mouse buttons are allowed to be processed and posted into
		/// {@code EventQueue}.
		/// <br>
		/// To change the returned value it is necessary to set the {@code sun.awt.enableExtraMouseButtons}
		/// property before the {@code Toolkit} class initialization. This setting could be done on the application
		/// startup by the following command:
		/// <pre>
		/// java -Dsun.awt.enableExtraMouseButtons=false Application
		/// </pre>
		/// Alternatively, the property could be set in the application by using the following code:
		/// <pre>
		/// System.setProperty("sun.awt.enableExtraMouseButtons", "true");
		/// </pre>
		/// before the {@code Toolkit} class initialization.
		/// If not set by the time of the {@code Toolkit} class initialization, this property will be
		/// initialized with {@code true}.
		/// Changing this value after the {@code Toolkit} class initialization will have no effect.
		/// <para>
		/// </para>
		/// </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
		/// <returns> {@code true} if events from extra mouse buttons are allowed to be processed and posted;
		///         {@code false} otherwise </returns>
		/// <seealso cref= System#getProperty(String propertyName) </seealso>
		/// <seealso cref= System#setProperty(String propertyName, String value) </seealso>
		/// <seealso cref= java.awt.EventQueue
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean areExtraMouseButtonsEnabled() throws HeadlessException
		public virtual bool AreExtraMouseButtonsEnabled()
		{
			GraphicsEnvironment.CheckHeadless();

			return Toolkit.DefaultToolkit.AreExtraMouseButtonsEnabled();
		}
	}

}