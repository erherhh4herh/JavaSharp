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

	using AppContext = sun.awt.AppContext;
	using SunToolkit = sun.awt.SunToolkit;
	using AWTAccessor = sun.awt.AWTAccessor;

	/// <summary>
	/// A <code>Frame</code> is a top-level window with a title and a border.
	/// <para>
	/// The size of the frame includes any area designated for the
	/// border.  The dimensions of the border area may be obtained
	/// using the <code>getInsets</code> method, however, since
	/// these dimensions are platform-dependent, a valid insets
	/// value cannot be obtained until the frame is made displayable
	/// by either calling <code>pack</code> or <code>show</code>.
	/// Since the border area is included in the overall size of the
	/// frame, the border effectively obscures a portion of the frame,
	/// constraining the area available for rendering and/or displaying
	/// subcomponents to the rectangle which has an upper-left corner
	/// location of <code>(insets.left, insets.top)</code>, and has a size of
	/// <code>width - (insets.left + insets.right)</code> by
	/// <code>height - (insets.top + insets.bottom)</code>.
	/// </para>
	/// <para>
	/// The default layout for a frame is <code>BorderLayout</code>.
	/// </para>
	/// <para>
	/// A frame may have its native decorations (i.e. <code>Frame</code>
	/// and <code>Titlebar</code>) turned off
	/// with <code>setUndecorated</code>. This can only be done while the frame
	/// is not <seealso cref="Component#isDisplayable() displayable"/>.
	/// </para>
	/// <para>
	/// In a multi-screen environment, you can create a <code>Frame</code>
	/// on a different screen device by constructing the <code>Frame</code>
	/// with <seealso cref="#Frame(GraphicsConfiguration)"/> or
	/// <seealso cref="#Frame(String title, GraphicsConfiguration)"/>.  The
	/// <code>GraphicsConfiguration</code> object is one of the
	/// <code>GraphicsConfiguration</code> objects of the target screen
	/// device.
	/// </para>
	/// <para>
	/// In a virtual device multi-screen environment in which the desktop
	/// area could span multiple physical screen devices, the bounds of all
	/// configurations are relative to the virtual-coordinate system.  The
	/// origin of the virtual-coordinate system is at the upper left-hand
	/// corner of the primary physical screen.  Depending on the location
	/// of the primary screen in the virtual device, negative coordinates
	/// are possible, as shown in the following figure.
	/// </para>
	/// <para>
	/// <img src="doc-files/MultiScreen.gif"
	/// alt="Diagram of virtual device encompassing three physical screens and one primary physical screen. The primary physical screen
	/// shows (0,0) coords while a different physical screen shows (-80,-100) coords."
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// In such an environment, when calling <code>setLocation</code>,
	/// you must pass a virtual coordinate to this method.  Similarly,
	/// calling <code>getLocationOnScreen</code> on a <code>Frame</code>
	/// returns virtual device coordinates.  Call the <code>getBounds</code>
	/// method of a <code>GraphicsConfiguration</code> to find its origin in
	/// the virtual coordinate system.
	/// </para>
	/// <para>
	/// The following code sets the
	/// location of the <code>Frame</code> at (10, 10) relative
	/// to the origin of the physical screen of the corresponding
	/// <code>GraphicsConfiguration</code>.  If the bounds of the
	/// <code>GraphicsConfiguration</code> is not taken into account, the
	/// <code>Frame</code> location would be set at (10, 10) relative to the
	/// virtual-coordinate system and would appear on the primary physical
	/// screen, which might be different from the physical screen of the
	/// specified <code>GraphicsConfiguration</code>.
	/// 
	/// <pre>
	///      Frame f = new Frame(GraphicsConfiguration gc);
	///      Rectangle bounds = gc.getBounds();
	///      f.setLocation(10 + bounds.x, 10 + bounds.y);
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// Frames are capable of generating the following types of
	/// <code>WindowEvent</code>s:
	/// <ul>
	/// <li><code>WINDOW_OPENED</code>
	/// <li><code>WINDOW_CLOSING</code>:
	///     <br>If the program doesn't
	///     explicitly hide or dispose the window while processing
	///     this event, the window close operation is canceled.
	/// <li><code>WINDOW_CLOSED</code>
	/// <li><code>WINDOW_ICONIFIED</code>
	/// <li><code>WINDOW_DEICONIFIED</code>
	/// <li><code>WINDOW_ACTIVATED</code>
	/// <li><code>WINDOW_DEACTIVATED</code>
	/// <li><code>WINDOW_GAINED_FOCUS</code>
	/// <li><code>WINDOW_LOST_FOCUS</code>
	/// <li><code>WINDOW_STATE_CHANGED</code>
	/// </ul>
	/// 
	/// @author      Sami Shaio
	/// </para>
	/// </summary>
	/// <seealso cref= WindowEvent </seealso>
	/// <seealso cref= Window#addWindowListener
	/// @since       JDK1.0 </seealso>
	public class Frame : Window, MenuContainer
	{

		/* Note: These are being obsoleted;  programs should use the Cursor class
		 * variables going forward. See Cursor and Component.setCursor.
		 */

	   /// @deprecated   replaced by <code>Cursor.DEFAULT_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.DEFAULT_CURSOR</code>.")]
		public const int DEFAULT_CURSOR = Cursor.DEFAULT_CURSOR;


	   /// @deprecated   replaced by <code>Cursor.CROSSHAIR_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.CROSSHAIR_CURSOR</code>.")]
		public const int CROSSHAIR_CURSOR = Cursor.CROSSHAIR_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.TEXT_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.TEXT_CURSOR</code>.")]
		public const int TEXT_CURSOR = Cursor.TEXT_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.WAIT_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.WAIT_CURSOR</code>.")]
		public const int WAIT_CURSOR = Cursor.WAIT_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.SW_RESIZE_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.SW_RESIZE_CURSOR</code>.")]
		public const int SW_RESIZE_CURSOR = Cursor.SW_RESIZE_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.SE_RESIZE_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.SE_RESIZE_CURSOR</code>.")]
		public const int SE_RESIZE_CURSOR = Cursor.SE_RESIZE_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.NW_RESIZE_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.NW_RESIZE_CURSOR</code>.")]
		public const int NW_RESIZE_CURSOR = Cursor.NW_RESIZE_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.NE_RESIZE_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.NE_RESIZE_CURSOR</code>.")]
		public const int NE_RESIZE_CURSOR = Cursor.NE_RESIZE_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.N_RESIZE_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.N_RESIZE_CURSOR</code>.")]
		public const int N_RESIZE_CURSOR = Cursor.N_RESIZE_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.S_RESIZE_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.S_RESIZE_CURSOR</code>.")]
		public const int S_RESIZE_CURSOR = Cursor.S_RESIZE_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.W_RESIZE_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.W_RESIZE_CURSOR</code>.")]
		public const int W_RESIZE_CURSOR = Cursor.W_RESIZE_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.E_RESIZE_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.E_RESIZE_CURSOR</code>.")]
		public const int E_RESIZE_CURSOR = Cursor.E_RESIZE_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.HAND_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.HAND_CURSOR</code>.")]
		public const int HAND_CURSOR = Cursor.HAND_CURSOR;

	   /// @deprecated   replaced by <code>Cursor.MOVE_CURSOR</code>. 
		[Obsolete("  replaced by <code>Cursor.MOVE_CURSOR</code>.")]
		public const int MOVE_CURSOR = Cursor.MOVE_CURSOR;


		/// <summary>
		/// Frame is in the "normal" state.  This symbolic constant names a
		/// frame state with all state bits cleared. </summary>
		/// <seealso cref= #setExtendedState(int) </seealso>
		/// <seealso cref= #getExtendedState </seealso>
		public const int NORMAL = 0;

		/// <summary>
		/// This state bit indicates that frame is iconified. </summary>
		/// <seealso cref= #setExtendedState(int) </seealso>
		/// <seealso cref= #getExtendedState </seealso>
		public const int ICONIFIED = 1;

		/// <summary>
		/// This state bit indicates that frame is maximized in the
		/// horizontal direction. </summary>
		/// <seealso cref= #setExtendedState(int) </seealso>
		/// <seealso cref= #getExtendedState
		/// @since 1.4 </seealso>
		public const int MAXIMIZED_HORIZ = 2;

		/// <summary>
		/// This state bit indicates that frame is maximized in the
		/// vertical direction. </summary>
		/// <seealso cref= #setExtendedState(int) </seealso>
		/// <seealso cref= #getExtendedState
		/// @since 1.4 </seealso>
		public const int MAXIMIZED_VERT = 4;

		/// <summary>
		/// This state bit mask indicates that frame is fully maximized
		/// (that is both horizontally and vertically).  It is just a
		/// convenience alias for
		/// <code>MAXIMIZED_VERT&nbsp;|&nbsp;MAXIMIZED_HORIZ</code>.
		/// 
		/// <para>Note that the correct test for frame being fully maximized is
		/// <pre>
		///     (state &amp; Frame.MAXIMIZED_BOTH) == Frame.MAXIMIZED_BOTH
		/// </pre>
		/// 
		/// </para>
		/// <para>To test is frame is maximized in <em>some</em> direction use
		/// <pre>
		///     (state &amp; Frame.MAXIMIZED_BOTH) != 0
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #setExtendedState(int) </seealso>
		/// <seealso cref= #getExtendedState
		/// @since 1.4 </seealso>
		public static readonly int MAXIMIZED_BOTH = MAXIMIZED_VERT | MAXIMIZED_HORIZ;

		/// <summary>
		/// Maximized bounds for this frame. </summary>
		/// <seealso cref=     #setMaximizedBounds(Rectangle) </seealso>
		/// <seealso cref=     #getMaximizedBounds
		/// @serial
		/// @since 1.4 </seealso>
		internal Rectangle MaximizedBounds_Renamed;


		/// <summary>
		/// This is the title of the frame.  It can be changed
		/// at any time.  <code>title</code> can be null and if
		/// this is the case the <code>title</code> = "".
		/// 
		/// @serial </summary>
		/// <seealso cref= #getTitle </seealso>
		/// <seealso cref= #setTitle(String) </seealso>
		internal String Title_Renamed = "Untitled";

		/// <summary>
		/// The frames menubar.  If <code>menuBar</code> = null
		/// the frame will not have a menubar.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getMenuBar </seealso>
		/// <seealso cref= #setMenuBar(MenuBar) </seealso>
		internal MenuBar MenuBar_Renamed;

		/// <summary>
		/// This field indicates whether the frame is resizable.
		/// This property can be changed at any time.
		/// <code>resizable</code> will be true if the frame is
		/// resizable, otherwise it will be false.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isResizable() </seealso>
		internal bool Resizable_Renamed = true;

		/// <summary>
		/// This field indicates whether the frame is undecorated.
		/// This property can only be changed while the frame is not displayable.
		/// <code>undecorated</code> will be true if the frame is
		/// undecorated, otherwise it will be false.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setUndecorated(boolean) </seealso>
		/// <seealso cref= #isUndecorated() </seealso>
		/// <seealso cref= Component#isDisplayable()
		/// @since 1.4 </seealso>
		internal bool Undecorated_Renamed = false;

		/// <summary>
		/// <code>mbManagement</code> is only used by the Motif implementation.
		/// 
		/// @serial
		/// </summary>
		internal bool MbManagement = false; // used only by the Motif impl.

		// XXX: uwe: abuse old field for now
		// will need to take care of serialization
		private new int State_Renamed = NORMAL;

		/*
		 * The Windows owned by the Frame.
		 * Note: in 1.2 this has been superceded by Window.ownedWindowList
		 *
		 * @serial
		 * @see java.awt.Window#ownedWindowList
		 */
		internal List<Window> OwnedWindows;

		private const String @base = "frame";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = 2673458971256075116L;

		static Frame()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
			AWTAccessor.setFrameAccessor(new FrameAccessorAnonymousInnerClassHelper()
		   );
		}

		private class FrameAccessorAnonymousInnerClassHelper : AWTAccessor.FrameAccessor
		{
			public FrameAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual void SetExtendedState(Frame frame, int state)
			{
				lock (frame.ObjectLock)
				{
					frame.State_Renamed = state;
				}
			}
			public virtual int GetExtendedState(Frame frame)
			{
				lock (frame.ObjectLock)
				{
					return frame.State_Renamed;
				}
			}
			public virtual Rectangle GetMaximizedBounds(Frame frame)
			{
				lock (frame.ObjectLock)
				{
					return frame.MaximizedBounds_Renamed;
				}
			}
		}

		/// <summary>
		/// Constructs a new instance of <code>Frame</code> that is
		/// initially invisible.  The title of the <code>Frame</code>
		/// is empty. </summary>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		/// <seealso cref= Component#setSize </seealso>
		/// <seealso cref= Component#setVisible(boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Frame() throws HeadlessException
		public Frame() : this("")
		{
		}

		/// <summary>
		/// Constructs a new, initially invisible {@code Frame} with the
		/// specified {@code GraphicsConfiguration}.
		/// </summary>
		/// <param name="gc"> the <code>GraphicsConfiguration</code>
		/// of the target screen device. If <code>gc</code>
		/// is <code>null</code>, the system default
		/// <code>GraphicsConfiguration</code> is assumed. </param>
		/// <exception cref="IllegalArgumentException"> if
		/// <code>gc</code> is not from a screen device. </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless()
		/// @since     1.3 </seealso>
		public Frame(GraphicsConfiguration gc) : this("", gc)
		{
		}

		/// <summary>
		/// Constructs a new, initially invisible <code>Frame</code> object
		/// with the specified title. </summary>
		/// <param name="title"> the title to be displayed in the frame's border.
		///              A <code>null</code> value
		///              is treated as an empty string, "". </param>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		/// <seealso cref= java.awt.Component#setSize </seealso>
		/// <seealso cref= java.awt.Component#setVisible(boolean) </seealso>
		/// <seealso cref= java.awt.GraphicsConfiguration#getBounds </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Frame(String title) throws HeadlessException
		public Frame(String title)
		{
			Init(title, null);
		}

		/// <summary>
		/// Constructs a new, initially invisible <code>Frame</code> object
		/// with the specified title and a
		/// <code>GraphicsConfiguration</code>. </summary>
		/// <param name="title"> the title to be displayed in the frame's border.
		///              A <code>null</code> value
		///              is treated as an empty string, "". </param>
		/// <param name="gc"> the <code>GraphicsConfiguration</code>
		/// of the target screen device.  If <code>gc</code> is
		/// <code>null</code>, the system default
		/// <code>GraphicsConfiguration</code> is assumed. </param>
		/// <exception cref="IllegalArgumentException"> if <code>gc</code>
		/// is not from a screen device. </exception>
		/// <exception cref="HeadlessException"> when
		///     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		/// <seealso cref= java.awt.Component#setSize </seealso>
		/// <seealso cref= java.awt.Component#setVisible(boolean) </seealso>
		/// <seealso cref= java.awt.GraphicsConfiguration#getBounds
		/// @since 1.3 </seealso>
		public Frame(String title, GraphicsConfiguration gc) : base(gc)
		{
			Init(title, gc);
		}

		private void Init(String title, GraphicsConfiguration gc)
		{
			this.Title_Renamed = title;
			SunToolkit.checkAndSetPolicy(this);
		}

		/// <summary>
		/// Construct a name for this component.  Called by getName() when the
		/// name is null.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(Frame))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Makes this Frame displayable by connecting it to
		/// a native screen resource.  Making a frame displayable will
		/// cause any of its children to be made displayable.
		/// This method is called internally by the toolkit and should
		/// not be called directly by programs. </summary>
		/// <seealso cref= Component#isDisplayable </seealso>
		/// <seealso cref= #removeNotify </seealso>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateFrame(this);
				}
				FramePeer p = (FramePeer)Peer_Renamed;
				MenuBar menuBar = this.MenuBar_Renamed;
				if (menuBar != null)
				{
					MbManagement = true;
					menuBar.AddNotify();
					p.MenuBar = menuBar;
				}
				p.MaximizedBounds = MaximizedBounds_Renamed;
				base.AddNotify();
			}
		}

		/// <summary>
		/// Gets the title of the frame.  The title is displayed in the
		/// frame's border. </summary>
		/// <returns>    the title of this frame, or an empty string ("")
		///                if this frame doesn't have a title. </returns>
		/// <seealso cref=       #setTitle(String) </seealso>
		public virtual String Title
		{
			get
			{
				return Title_Renamed;
			}
			set
			{
				String oldTitle = this.Title_Renamed;
				if (value == null)
				{
					value = "";
				}
    
    
				lock (this)
				{
					this.Title_Renamed = value;
					FramePeer peer = (FramePeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.Title = value;
					}
				}
				FirePropertyChange("title", oldTitle, value);
			}
		}


		/// <summary>
		/// Returns the image to be displayed as the icon for this frame.
		/// <para>
		/// This method is obsolete and kept for backward compatibility
		/// only. Use <seealso cref="Window#getIconImages Window.getIconImages()"/> instead.
		/// </para>
		/// <para>
		/// If a list of several images was specified as a Window's icon,
		/// this method will return the first item of the list.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    the icon image for this frame, or <code>null</code>
		///                    if this frame doesn't have an icon image. </returns>
		/// <seealso cref=       #setIconImage(Image) </seealso>
		/// <seealso cref=       Window#getIconImages() </seealso>
		/// <seealso cref=       Window#setIconImages </seealso>
		public virtual Image IconImage
		{
			get
			{
				IList<Image> icons = this.Icons;
				if (icons != null)
				{
					if (icons.Count > 0)
					{
						return icons[0];
					}
				}
				return null;
			}
			set
			{
				base.IconImage = value;
			}
		}


		/// <summary>
		/// Gets the menu bar for this frame. </summary>
		/// <returns>    the menu bar for this frame, or <code>null</code>
		///                   if this frame doesn't have a menu bar. </returns>
		/// <seealso cref=       #setMenuBar(MenuBar) </seealso>
		public virtual MenuBar MenuBar
		{
			get
			{
				return MenuBar_Renamed;
			}
			set
			{
				lock (TreeLock)
				{
					if (MenuBar_Renamed == value)
					{
						return;
					}
					if ((value != null) && (value.Parent_Renamed != null))
					{
						value.Parent_Renamed.remove(value);
					}
					if (MenuBar_Renamed != null)
					{
						Remove(MenuBar_Renamed);
					}
					MenuBar_Renamed = value;
					if (MenuBar_Renamed != null)
					{
						MenuBar_Renamed.Parent_Renamed = this;
    
						FramePeer peer = (FramePeer)this.Peer_Renamed;
						if (peer != null)
						{
							MbManagement = true;
							MenuBar_Renamed.AddNotify();
							InvalidateIfValid();
							peer.MenuBar = MenuBar_Renamed;
						}
					}
				}
			}
		}


		/// <summary>
		/// Indicates whether this frame is resizable by the user.
		/// By default, all frames are initially resizable. </summary>
		/// <returns>    <code>true</code> if the user can resize this frame;
		///                        <code>false</code> otherwise. </returns>
		/// <seealso cref=       java.awt.Frame#setResizable(boolean) </seealso>
		public virtual bool Resizable
		{
			get
			{
				return Resizable_Renamed;
			}
			set
			{
				bool oldResizable = this.Resizable_Renamed;
				bool testvalid = false;
    
				lock (this)
				{
					this.Resizable_Renamed = value;
					FramePeer peer = (FramePeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.Resizable = value;
						testvalid = true;
					}
				}
    
				// On some platforms, changing the value state affects
				// the insets of the Frame. If we could, we'd call invalidate()
				// from the peer, but we need to guarantee that we're not holding
				// the Frame lock when we call invalidate().
				if (testvalid)
				{
					InvalidateIfValid();
				}
				FirePropertyChange("resizable", oldResizable, value);
			}
		}



		/// <summary>
		/// Sets the state of this frame (obsolete).
		/// <para>
		/// In older versions of JDK a frame state could only be NORMAL or
		/// ICONIFIED.  Since JDK 1.4 set of supported frame states is
		/// expanded and frame state is represented as a bitwise mask.
		/// </para>
		/// <para>
		/// For compatibility with applications developed
		/// earlier this method still accepts
		/// {@code Frame.NORMAL} and
		/// {@code Frame.ICONIFIED} only.  The iconic
		/// state of the frame is only changed, other aspects
		/// of frame state are not affected by this method. If
		/// the state passed to this method is neither {@code
		/// Frame.NORMAL} nor {@code Frame.ICONIFIED} the
		/// method performs no actions at all.
		/// </para>
		/// <para>Note that if the state is not supported on a
		/// given platform, neither the state nor the return
		/// value of the <seealso cref="#getState"/> method will be
		/// changed. The application may determine whether a
		/// specific state is supported via the {@link
		/// java.awt.Toolkit#isFrameStateSupported} method.
		/// </para>
		/// <para><b>If the frame is currently visible on the
		/// screen</b> (the <seealso cref="#isShowing"/> method returns
		/// {@code true}), the developer should examine the
		/// return value of the  {@link
		/// java.awt.event.WindowEvent#getNewState} method of
		/// the {@code WindowEvent} received through the
		/// <seealso cref="java.awt.event.WindowStateListener"/> to
		/// determine that the state has actually been
		/// changed.
		/// </para>
		/// <para><b>If the frame is not visible on the
		/// screen</b>, the events may or may not be
		/// generated.  In this case the developer may assume
		/// that the state changes immediately after this
		/// method returns.  Later, when the {@code
		/// setVisible(true)} method is invoked, the frame
		/// will attempt to apply this state. Receiving any
		/// {@link
		/// java.awt.event.WindowEvent#WINDOW_STATE_CHANGED}
		/// events is not guaranteed in this case also.
		/// 
		/// </para>
		/// </summary>
		/// <param name="state"> either <code>Frame.NORMAL</code> or
		///     <code>Frame.ICONIFIED</code>. </param>
		/// <seealso cref= #setExtendedState(int) </seealso>
		/// <seealso cref= java.awt.Window#addWindowStateListener </seealso>
		public virtual int State
		{
			set
			{
				lock (this)
				{
					int current = ExtendedState;
					if (value == ICONIFIED && (current & ICONIFIED) == 0)
					{
						ExtendedState = current | ICONIFIED;
					}
					else if (value == NORMAL && (current & ICONIFIED) != 0)
					{
						ExtendedState = current & ~ICONIFIED;
					}
				}
			}
			get
			{
				lock (this)
				{
					return (ExtendedState & ICONIFIED) != 0 ? ICONIFIED : NORMAL;
				}
			}
		}

		/// <summary>
		/// Sets the state of this frame. The state is
		/// represented as a bitwise mask.
		/// <ul>
		/// <li><code>NORMAL</code>
		/// <br>Indicates that no state bits are set.
		/// <li><code>ICONIFIED</code>
		/// <li><code>MAXIMIZED_HORIZ</code>
		/// <li><code>MAXIMIZED_VERT</code>
		/// <li><code>MAXIMIZED_BOTH</code>
		/// <br>Concatenates <code>MAXIMIZED_HORIZ</code>
		/// and <code>MAXIMIZED_VERT</code>.
		/// </ul>
		/// <para>Note that if the state is not supported on a
		/// given platform, neither the state nor the return
		/// value of the <seealso cref="#getExtendedState"/> method will
		/// be changed. The application may determine whether
		/// a specific state is supported via the {@link
		/// java.awt.Toolkit#isFrameStateSupported} method.
		/// </para>
		/// <para><b>If the frame is currently visible on the
		/// screen</b> (the <seealso cref="#isShowing"/> method returns
		/// {@code true}), the developer should examine the
		/// return value of the {@link
		/// java.awt.event.WindowEvent#getNewState} method of
		/// the {@code WindowEvent} received through the
		/// <seealso cref="java.awt.event.WindowStateListener"/> to
		/// determine that the state has actually been
		/// changed.
		/// </para>
		/// <para><b>If the frame is not visible on the
		/// screen</b>, the events may or may not be
		/// generated.  In this case the developer may assume
		/// that the state changes immediately after this
		/// method returns.  Later, when the {@code
		/// setVisible(true)} method is invoked, the frame
		/// will attempt to apply this state. Receiving any
		/// {@link
		/// java.awt.event.WindowEvent#WINDOW_STATE_CHANGED}
		/// events is not guaranteed in this case also.
		/// 
		/// </para>
		/// </summary>
		/// <param name="state"> a bitwise mask of frame state constants
		/// @since   1.4 </param>
		/// <seealso cref= java.awt.Window#addWindowStateListener </seealso>
		public virtual int ExtendedState
		{
			set
			{
				if (!IsFrameStateSupported(value))
				{
					return;
				}
				lock (ObjectLock)
				{
					this.State_Renamed = value;
				}
				// peer.setState must be called outside of object lock
				// synchronization block to avoid possible deadlock
				FramePeer peer = (FramePeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.State = value;
				}
			}
			get
			{
				lock (ObjectLock)
				{
					return State_Renamed;
				}
			}
		}
		private bool IsFrameStateSupported(int state)
		{
			if (!Toolkit.IsFrameStateSupported(state))
			{
				// * Toolkit.isFrameStateSupported returns always false
				// on compound state even if all parts are supported;
				// * if part of state is not supported, state is not supported;
				// * MAXIMIZED_BOTH is not a compound state.
				if (((state & ICONIFIED) != 0) && !Toolkit.IsFrameStateSupported(ICONIFIED))
				{
					return false;
				}
				else
				{
					state &= ~ICONIFIED;
				}
				return Toolkit.IsFrameStateSupported(state);
			}
			return true;
		}





		/// <summary>
		/// Sets the maximized bounds for this frame.
		/// <para>
		/// When a frame is in maximized state the system supplies some
		/// defaults bounds.  This method allows some or all of those
		/// system supplied values to be overridden.
		/// </para>
		/// <para>
		/// If <code>bounds</code> is <code>null</code>, accept bounds
		/// supplied by the system.  If non-<code>null</code> you can
		/// override some of the system supplied values while accepting
		/// others by setting those fields you want to accept from system
		/// to <code>Integer.MAX_VALUE</code>.
		/// </para>
		/// <para>
		/// Note, the given maximized bounds are used as a hint for the native
		/// system, because the underlying platform may not support setting the
		/// location and/or size of the maximized windows.  If that is the case, the
		/// provided values do not affect the appearance of the frame in the
		/// maximized state.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bounds">  bounds for the maximized state </param>
		/// <seealso cref= #getMaximizedBounds()
		/// @since 1.4 </seealso>
		public virtual Rectangle MaximizedBounds
		{
			set
			{
				lock (ObjectLock)
				{
					this.MaximizedBounds_Renamed = value;
				}
				FramePeer peer = (FramePeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.MaximizedBounds = value;
				}
			}
			get
			{
				lock (ObjectLock)
				{
					return MaximizedBounds_Renamed;
				}
			}
		}



		/// <summary>
		/// Disables or enables decorations for this frame.
		/// <para>
		/// This method can only be called while the frame is not displayable. To
		/// make this frame decorated, it must be opaque and have the default shape,
		/// otherwise the {@code IllegalComponentStateException} will be thrown.
		/// Refer to <seealso cref="Window#setShape"/>, <seealso cref="Window#setOpacity"/> and {@link
		/// Window#setBackground} for details
		/// 
		/// </para>
		/// </summary>
		/// <param name="undecorated"> {@code true} if no frame decorations are to be
		///         enabled; {@code false} if frame decorations are to be enabled
		/// </param>
		/// <exception cref="IllegalComponentStateException"> if the frame is displayable </exception>
		/// <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		///      {@code false}, and this frame does not have the default shape </exception>
		/// <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		///      {@code false}, and this frame opacity is less than {@code 1.0f} </exception>
		/// <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		///      {@code false}, and the alpha value of this frame background
		///      color is less than {@code 1.0f}
		/// </exception>
		/// <seealso cref=    #isUndecorated </seealso>
		/// <seealso cref=    Component#isDisplayable </seealso>
		/// <seealso cref=    Window#getShape </seealso>
		/// <seealso cref=    Window#getOpacity </seealso>
		/// <seealso cref=    Window#getBackground </seealso>
		/// <seealso cref=    javax.swing.JFrame#setDefaultLookAndFeelDecorated(boolean)
		/// 
		/// @since 1.4 </seealso>
		public virtual bool Undecorated
		{
			set
			{
				/* Make sure we don't run in the middle of peer creation.*/
				lock (TreeLock)
				{
					if (Displayable)
					{
						throw new IllegalComponentStateException("The frame is displayable.");
					}
					if (!value)
					{
						if (Opacity < 1.0f)
						{
							throw new IllegalComponentStateException("The frame is not opaque");
						}
						if (Shape != null)
						{
							throw new IllegalComponentStateException("The frame does not have a default shape");
						}
						Color bg = Background;
						if ((bg != null) && (bg.Alpha < 255))
						{
							throw new IllegalComponentStateException("The frame background color is not opaque");
						}
					}
					this.Undecorated_Renamed = value;
				}
			}
			get
			{
				return Undecorated_Renamed;
			}
		}


		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override float Opacity
		{
			set
			{
				lock (TreeLock)
				{
					if ((value < 1.0f) && !Undecorated)
					{
						throw new IllegalComponentStateException("The frame is decorated");
					}
					base.Opacity = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Shape Shape
		{
			set
			{
				lock (TreeLock)
				{
					if ((value != null) && !Undecorated)
					{
						throw new IllegalComponentStateException("The frame is decorated");
					}
					base.Shape = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Color Background
		{
			set
			{
				lock (TreeLock)
				{
					if ((value != null) && (value.Alpha < 255) && !Undecorated)
					{
						throw new IllegalComponentStateException("The frame is decorated");
					}
					base.Background = value;
				}
			}
		}

		/// <summary>
		/// Removes the specified menu bar from this frame. </summary>
		/// <param name="m">   the menu component to remove.
		///           If <code>m</code> is <code>null</code>, then
		///           no action is taken </param>
		public virtual void Remove(MenuComponent m)
		{
			if (m == null)
			{
				return;
			}
			lock (TreeLock)
			{
				if (m == MenuBar_Renamed)
				{
					MenuBar_Renamed = null;
					FramePeer peer = (FramePeer)this.Peer_Renamed;
					if (peer != null)
					{
						MbManagement = true;
						InvalidateIfValid();
						peer.MenuBar = null;
						m.RemoveNotify();
					}
					m.Parent_Renamed = null;
				}
				else
				{
					base.Remove(m);
				}
			}
		}

		/// <summary>
		/// Makes this Frame undisplayable by removing its connection
		/// to its native screen resource. Making a Frame undisplayable
		/// will cause any of its children to be made undisplayable.
		/// This method is called by the toolkit internally and should
		/// not be called directly by programs. </summary>
		/// <seealso cref= Component#isDisplayable </seealso>
		/// <seealso cref= #addNotify </seealso>
		public override void RemoveNotify()
		{
			lock (TreeLock)
			{
				FramePeer peer = (FramePeer)this.Peer_Renamed;
				if (peer != null)
				{
					// get the latest Frame state before disposing
					State;

					if (MenuBar_Renamed != null)
					{
						MbManagement = true;
						peer.MenuBar = null;
						MenuBar_Renamed.RemoveNotify();
					}
				}
				base.RemoveNotify();
			}
		}

		internal override void PostProcessKeyEvent(KeyEvent e)
		{
			if (MenuBar_Renamed != null && MenuBar_Renamed.HandleShortcut(e))
			{
				e.Consume();
				return;
			}
			base.PostProcessKeyEvent(e);
		}

		/// <summary>
		/// Returns a string representing the state of this <code>Frame</code>.
		/// This method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns> the parameter string of this frame </returns>
		protected internal override String ParamString()
		{
			String str = base.ParamString();
			if (Title_Renamed != null)
			{
				str += ",title=" + Title_Renamed;
			}
			if (Resizable_Renamed)
			{
				str += ",resizable";
			}
			int state = ExtendedState;
			if (state == NORMAL)
			{
				str += ",normal";
			}
			else
			{
				if ((state & ICONIFIED) != 0)
				{
					str += ",iconified";
				}
				if ((state & MAXIMIZED_BOTH) == MAXIMIZED_BOTH)
				{
					str += ",maximized";
				}
				else if ((state & MAXIMIZED_HORIZ) != 0)
				{
					str += ",maximized_horiz";
				}
				else if ((state & MAXIMIZED_VERT) != 0)
				{
					str += ",maximized_vert";
				}
			}
			return str;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Component.setCursor(Cursor)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int Cursor
		{
			set
			{
				if (value < DEFAULT_CURSOR || value > MOVE_CURSOR)
				{
					throw new IllegalArgumentException("illegal cursor type");
				}
				Cursor = Cursor.GetPredefinedCursor(value);
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>Component.getCursor()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int CursorType
		{
			get
			{
				return (Cursor.Type);
			}
		}

		/// <summary>
		/// Returns an array of all {@code Frame}s created by this application.
		/// If called from an applet, the array includes only the {@code Frame}s
		/// accessible by that applet.
		/// <para>
		/// <b>Warning:</b> this method may return system created frames, such
		/// as a shared, hidden frame which is used by Swing. Applications
		/// should not assume the existence of these frames, nor should an
		/// application assume anything about these frames such as component
		/// positions, <code>LayoutManager</code>s or serialization.
		/// </para>
		/// <para>
		/// <b>Note</b>: To obtain a list of all ownerless windows, including
		/// ownerless {@code Dialog}s (introduced in release 1.6), use {@link
		/// Window#getOwnerlessWindows Window.getOwnerlessWindows}.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Window#getWindows() </seealso>
		/// <seealso cref= Window#getOwnerlessWindows
		/// 
		/// @since 1.2 </seealso>
		public static Frame[] Frames
		{
			get
			{
				Window[] allWindows = Window.Windows;
    
				int frameCount = 0;
				foreach (Window w in allWindows)
				{
					if (w is Frame)
					{
						frameCount++;
					}
				}
    
				Frame[] frames = new Frame[frameCount];
				int c = 0;
				foreach (Window w in allWindows)
				{
					if (w is Frame)
					{
						frames[c++] = (Frame)w;
					}
				}
    
				return frames;
			}
		}

		/* Serialization support.  If there's a MenuBar we restore
		 * its (transient) parent field here.  Likewise for top level
		 * windows that are "owned" by this frame.
		 */

		/// <summary>
		/// <code>Frame</code>'s Serialized Data Version.
		/// 
		/// @serial
		/// </summary>
		private int FrameSerializedDataVersion = 1;

		/// <summary>
		/// Writes default serializable fields to stream.  Writes
		/// an optional serializable icon <code>Image</code>, which is
		/// available as of 1.4.
		/// </summary>
		/// <param name="s"> the <code>ObjectOutputStream</code> to write
		/// @serialData an optional icon <code>Image</code> </param>
		/// <seealso cref= java.awt.Image </seealso>
		/// <seealso cref= #getIconImage </seealso>
		/// <seealso cref= #setIconImage(Image) </seealso>
		/// <seealso cref= #readObject(ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			s.DefaultWriteObject();
			if (Icons != null && Icons.Count > 0)
			{
				Image icon1 = Icons[0];
				if (icon1 is Serializable)
				{
					s.WriteObject(icon1);
					return;
				}
			}
			s.WriteObject(null);
		}

		/// <summary>
		/// Reads the <code>ObjectInputStream</code>.  Tries
		/// to read an icon <code>Image</code>, which is optional
		/// data available as of 1.4.  If an icon <code>Image</code>
		/// is not available, but anything other than an EOF
		/// is detected, an <code>OptionalDataException</code>
		/// will be thrown.
		/// Unrecognized keys or values will be ignored.
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to read </param>
		/// <exception cref="java.io.OptionalDataException"> if an icon <code>Image</code>
		///   is not available, but anything other than an EOF
		///   is detected </exception>
		/// <exception cref="HeadlessException"> if
		///   <code>GraphicsEnvironment.isHeadless</code> returns
		///   <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		/// <seealso cref= java.awt.Image </seealso>
		/// <seealso cref= #getIconImage </seealso>
		/// <seealso cref= #setIconImage(Image) </seealso>
		/// <seealso cref= #writeObject(ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
		  // HeadlessException is thrown by Window's readObject
		  s.DefaultReadObject();
		  try
		  {
			  Image icon = (Image) s.ReadObject();
			  if (Icons == null)
			  {
				  Icons = new List<Image>();
				  Icons.Add(icon);
			  }
		  }
		  catch (java.io.OptionalDataException e)
		  {
			  // pre-1.4 instances will not have this optional data.
			  // 1.6 and later instances serialize icons in the Window class
			  // e.eof will be true to indicate that there is no more
			  // data available for this object.

			  // If e.eof is not true, throw the exception as it
			  // might have been caused by unrelated reasons.
			  if (!e.Eof)
			  {
				  throw (e);
			  }
		  }

		  if (MenuBar_Renamed != null)
		  {
			MenuBar_Renamed.Parent_Renamed = this;
		  }

		  // Ensure 1.1 serialized Frames can read & hook-up
		  // owned windows properly
		  //
		  if (OwnedWindows != null)
		  {
			  for (int i = 0; i < OwnedWindows.Count; i++)
			  {
				  ConnectOwnedWindow(OwnedWindows[i]);
			  }
			  OwnedWindows = null;
		  }
		}

		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		/*
		 * --- Accessibility Support ---
		 *
		 */

		/// <summary>
		/// Gets the AccessibleContext associated with this Frame.
		/// For frames, the AccessibleContext takes the form of an
		/// AccessibleAWTFrame.
		/// A new AccessibleAWTFrame instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTFrame that serves as the
		///         AccessibleContext of this Frame
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTFrame(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>Frame</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to frame user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTFrame : AccessibleAWTWindow
		{
			private readonly Frame OuterInstance;

			public AccessibleAWTFrame(Frame outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = -6172960752956030250L;

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object </returns>
			/// <seealso cref= AccessibleRole </seealso>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.FRAME;
				}
			}

			/// <summary>
			/// Get the state of this object.
			/// </summary>
			/// <returns> an instance of AccessibleStateSet containing the current
			/// state set of the object </returns>
			/// <seealso cref= AccessibleState </seealso>
			public override AccessibleStateSet AccessibleStateSet
			{
				get
				{
					AccessibleStateSet states = base.AccessibleStateSet;
					if (outerInstance.FocusOwner != null)
					{
						states.add(AccessibleState.ACTIVE);
					}
					if (outerInstance.Resizable)
					{
						states.add(AccessibleState.RESIZABLE);
					}
					return states;
				}
			}


		} // inner class AccessibleAWTFrame

	}

}