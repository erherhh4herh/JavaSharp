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
	using AppContext = sun.awt.AppContext;
	using CausedFocusEvent = sun.awt.CausedFocusEvent;
	using SunToolkit = sun.awt.SunToolkit;
	using IdentityArrayList = sun.awt.util.IdentityArrayList;
	using Disposer = sun.java2d.Disposer;
	using Region = sun.java2d.pipe.Region;
	using GetPropertyAction = sun.security.action.GetPropertyAction;
	using SecurityConstants = sun.security.util.SecurityConstants;
	using PlatformLogger = sun.util.logging.PlatformLogger;

	/// <summary>
	/// A {@code Window} object is a top-level window with no borders and no
	/// menubar.
	/// The default layout for a window is {@code BorderLayout}.
	/// <para>
	/// A window must have either a frame, dialog, or another window defined as its
	/// owner when it's constructed.
	/// </para>
	/// <para>
	/// In a multi-screen environment, you can create a {@code Window}
	/// on a different screen device by constructing the {@code Window}
	/// with <seealso cref="#Window(Window, GraphicsConfiguration)"/>.  The
	/// {@code GraphicsConfiguration} object is one of the
	/// {@code GraphicsConfiguration} objects of the target screen device.
	/// </para>
	/// <para>
	/// In a virtual device multi-screen environment in which the desktop
	/// area could span multiple physical screen devices, the bounds of all
	/// configurations are relative to the virtual device coordinate system.
	/// The origin of the virtual-coordinate system is at the upper left-hand
	/// corner of the primary physical screen.  Depending on the location of
	/// the primary screen in the virtual device, negative coordinates are
	/// possible, as shown in the following figure.
	/// </para>
	/// <para>
	/// <img src="doc-files/MultiScreen.gif"
	/// alt="Diagram shows virtual device containing 4 physical screens. Primary physical screen shows coords (0,0), other screen shows (-80,-100)."
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// In such an environment, when calling {@code setLocation},
	/// you must pass a virtual coordinate to this method.  Similarly,
	/// calling {@code getLocationOnScreen} on a {@code Window} returns
	/// virtual device coordinates.  Call the {@code getBounds} method
	/// of a {@code GraphicsConfiguration} to find its origin in the virtual
	/// coordinate system.
	/// </para>
	/// <para>
	/// The following code sets the location of a {@code Window}
	/// at (10, 10) relative to the origin of the physical screen
	/// of the corresponding {@code GraphicsConfiguration}.  If the
	/// bounds of the {@code GraphicsConfiguration} is not taken
	/// into account, the {@code Window} location would be set
	/// at (10, 10) relative to the virtual-coordinate system and would appear
	/// on the primary physical screen, which might be different from the
	/// physical screen of the specified {@code GraphicsConfiguration}.
	/// 
	/// <pre>
	///      Window w = new Window(Window owner, GraphicsConfiguration gc);
	///      Rectangle bounds = gc.getBounds();
	///      w.setLocation(10 + bounds.x, 10 + bounds.y);
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// Note: the location and size of top-level windows (including
	/// {@code Window}s, {@code Frame}s, and {@code Dialog}s)
	/// are under the control of the desktop's window management system.
	/// Calls to {@code setLocation}, {@code setSize}, and
	/// {@code setBounds} are requests (not directives) which are
	/// forwarded to the window management system.  Every effort will be
	/// made to honor such requests.  However, in some cases the window
	/// management system may ignore such requests, or modify the requested
	/// geometry in order to place and size the {@code Window} in a way
	/// that more closely matches the desktop settings.
	/// </para>
	/// <para>
	/// Due to the asynchronous nature of native event handling, the results
	/// returned by {@code getBounds}, {@code getLocation},
	/// {@code getLocationOnScreen}, and {@code getSize} might not
	/// reflect the actual geometry of the Window on screen until the last
	/// request has been processed.  During the processing of subsequent
	/// requests these values might change accordingly while the window
	/// management system fulfills the requests.
	/// </para>
	/// <para>
	/// An application may set the size and location of an invisible
	/// {@code Window} arbitrarily, but the window management system may
	/// subsequently change its size and/or location when the
	/// {@code Window} is made visible. One or more {@code ComponentEvent}s
	/// will be generated to indicate the new geometry.
	/// </para>
	/// <para>
	/// Windows are capable of generating the following WindowEvents:
	/// WindowOpened, WindowClosed, WindowGainedFocus, WindowLostFocus.
	/// 
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref= WindowEvent </seealso>
	/// <seealso cref= #addWindowListener </seealso>
	/// <seealso cref= java.awt.BorderLayout
	/// @since       JDK1.0 </seealso>
	public class Window : Container, Accessible
	{

		/// <summary>
		/// Enumeration of available <i>window types</i>.
		/// 
		/// A window type defines the generic visual appearance and behavior of a
		/// top-level window. For example, the type may affect the kind of
		/// decorations of a decorated {@code Frame} or {@code Dialog} instance.
		/// <para>
		/// Some platforms may not fully support a certain window type. Depending on
		/// the level of support, some properties of the window type may be
		/// disobeyed.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=   #getType </seealso>
		/// <seealso cref=   #setType
		/// @since 1.7 </seealso>
		public enum Type
		{
			/// <summary>
			/// Represents a <i>normal</i> window.
			/// 
			/// This is the default type for objects of the {@code Window} class or
			/// its descendants. Use this type for regular top-level windows.
			/// </summary>
			NORMAL,

			/// <summary>
			/// Represents a <i>utility</i> window.
			/// 
			/// A utility window is usually a small window such as a toolbar or a
			/// palette. The native system may render the window with smaller
			/// title-bar if the window is either a {@code Frame} or a {@code
			/// Dialog} object, and if it has its decorations enabled.
			/// </summary>
			UTILITY,

			/// <summary>
			/// Represents a <i>popup</i> window.
			/// 
			/// A popup window is a temporary window such as a drop-down menu or a
			/// tooltip. On some platforms, windows of that type may be forcibly
			/// made undecorated even if they are instances of the {@code Frame} or
			/// {@code Dialog} class, and have decorations enabled.
			/// </summary>
			POPUP
		}

		/// <summary>
		/// This represents the warning message that is
		/// to be displayed in a non secure window. ie :
		/// a window that has a security manager installed that denies
		/// {@code AWTPermission("showWindowWithoutWarningBanner")}.
		/// This message can be displayed anywhere in the window.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getWarningString </seealso>
		internal String WarningString_Renamed;

		/// <summary>
		/// {@code icons} is the graphical way we can
		/// represent the frames and dialogs.
		/// {@code Window} can't display icon but it's
		/// being inherited by owned {@code Dialog}s.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getIconImages </seealso>
		/// <seealso cref= #setIconImages </seealso>
		[NonSerialized]
		internal IList<Image> Icons;

		/// <summary>
		/// Holds the reference to the component which last had focus in this window
		/// before it lost focus.
		/// </summary>
		[NonSerialized]
		private Component TemporaryLostComponent_Renamed;

		internal static bool SystemSyncLWRequests = false;
		internal bool SyncLWRequests = false;
		[NonSerialized]
		internal bool BeforeFirstShow = true;
		[NonSerialized]
		private bool Disposing_Renamed = false;
		[NonSerialized]
		internal WindowDisposerRecord DisposerRecord = null;

		internal const int OPENED = 0x01;

		/// <summary>
		/// An Integer value representing the Window State.
		/// 
		/// @serial
		/// @since 1.2 </summary>
		/// <seealso cref= #show </seealso>
		internal int State;

		/// <summary>
		/// A boolean value representing Window always-on-top state
		/// @since 1.5
		/// @serial </summary>
		/// <seealso cref= #setAlwaysOnTop </seealso>
		/// <seealso cref= #isAlwaysOnTop </seealso>
		private bool AlwaysOnTop_Renamed;

		/// <summary>
		/// Contains all the windows that have a peer object associated,
		/// i. e. between addNotify() and removeNotify() calls. The list
		/// of all Window instances can be obtained from AppContext object.
		/// 
		/// @since 1.6
		/// </summary>
		private static readonly IdentityArrayList<Window> AllWindows_Renamed = new IdentityArrayList<Window>();

		/// <summary>
		/// A vector containing all the windows this
		/// window currently owns.
		/// @since 1.2 </summary>
		/// <seealso cref= #getOwnedWindows </seealso>
		[NonSerialized]
		internal List<WeakReference<Window>> OwnedWindowList = new List<WeakReference<Window>>();

		/*
		 * We insert a weak reference into the Vector of all Windows in AppContext
		 * instead of 'this' so that garbage collection can still take place
		 * correctly.
		 */
		[NonSerialized]
		private WeakReference<Window> WeakThis;

		[NonSerialized]
		internal bool ShowWithParent;

		/// <summary>
		/// Contains the modal dialog that blocks this window, or null
		/// if the window is unblocked.
		/// 
		/// @since 1.6
		/// </summary>
		[NonSerialized]
		internal Dialog ModalBlocker_Renamed;

		/// <summary>
		/// @serial
		/// </summary>
		/// <seealso cref= java.awt.Dialog.ModalExclusionType </seealso>
		/// <seealso cref= #getModalExclusionType </seealso>
		/// <seealso cref= #setModalExclusionType
		/// 
		/// @since 1.6 </seealso>
		internal Dialog.ModalExclusionType ModalExclusionType_Renamed;

		[NonSerialized]
		internal WindowListener WindowListener;
		[NonSerialized]
		internal WindowStateListener WindowStateListener;
		[NonSerialized]
		internal WindowFocusListener WindowFocusListener;

		[NonSerialized]
		internal InputContext InputContext_Renamed;
		[NonSerialized]
		private Object InputContextLock = new Object();

		/// <summary>
		/// Unused. Maintained for serialization backward-compatibility.
		/// 
		/// @serial
		/// @since 1.2
		/// </summary>
		private FocusManager FocusMgr;

		/// <summary>
		/// Indicates whether this Window can become the focused Window.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getFocusableWindowState </seealso>
		/// <seealso cref= #setFocusableWindowState
		/// @since 1.4 </seealso>
		private bool FocusableWindowState_Renamed = true;

		/// <summary>
		/// Indicates whether this window should receive focus on
		/// subsequently being shown (with a call to {@code setVisible(true)}), or
		/// being moved to the front (with a call to {@code toFront()}).
		/// 
		/// @serial </summary>
		/// <seealso cref= #setAutoRequestFocus </seealso>
		/// <seealso cref= #isAutoRequestFocus
		/// @since 1.7 </seealso>
		private volatile bool AutoRequestFocus_Renamed = true;

		/*
		 * Indicates that this window is being shown. This flag is set to true at
		 * the beginning of show() and to false at the end of show().
		 *
		 * @see #show()
		 * @see Dialog#shouldBlock
		 */
		[NonSerialized]
		internal bool IsInShow = false;

		/// <summary>
		/// The opacity level of the window
		/// 
		/// @serial </summary>
		/// <seealso cref= #setOpacity(float) </seealso>
		/// <seealso cref= #getOpacity()
		/// @since 1.7 </seealso>
		private float Opacity_Renamed = 1.0f;

		/// <summary>
		/// The shape assigned to this window. This field is set to {@code null} if
		/// no shape is set (rectangular window).
		/// 
		/// @serial </summary>
		/// <seealso cref= #getShape() </seealso>
		/// <seealso cref= #setShape(Shape)
		/// @since 1.7 </seealso>
		private Shape Shape_Renamed = null;

		private const String @base = "win";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 4497834738069338734L;

		private static readonly PlatformLogger Log = PlatformLogger.getLogger("java.awt.Window");

		private static readonly bool LocationByPlatformProp;

		[NonSerialized]
		internal bool IsTrayIconWindow = false;

		/// <summary>
		/// These fields are initialized in the native peer code
		/// or via AWTAccessor's WindowAccessor.
		/// </summary>
		[NonSerialized]
		private volatile int SecurityWarningWidth = 0;
		[NonSerialized]
		private volatile int SecurityWarningHeight = 0;

		/// <summary>
		/// These fields represent the desired location for the security
		/// warning if this window is untrusted.
		/// See com.sun.awt.SecurityWarning for more details.
		/// </summary>
		[NonSerialized]
		private double SecurityWarningPointX = 2.0;
		[NonSerialized]
		private double SecurityWarningPointY = 0.0;
		[NonSerialized]
		private float SecurityWarningAlignmentX = RIGHT_ALIGNMENT;
		[NonSerialized]
		private float SecurityWarningAlignmentY = TOP_ALIGNMENT;

		static Window()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}

			String s = AccessController.doPrivileged(new GetPropertyAction("java.awt.syncLWRequests"));
			SystemSyncLWRequests = (s != null && s.Equals("true"));
			s = AccessController.doPrivileged(new GetPropertyAction("java.awt.Window.locationByPlatform"));
			LocationByPlatformProp = (s != null && s.Equals("true"));
			AWTAccessor.setWindowAccessor(new WindowAccessorAnonymousInnerClassHelper()); // WindowAccessor
		}

		private class WindowAccessorAnonymousInnerClassHelper : AWTAccessor.WindowAccessor
		{
			public WindowAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual float GetOpacity(Window window)
			{
				return window.Opacity_Renamed;
			}
			public virtual void SetOpacity(Window window, float opacity)
			{
				window.Opacity = opacity;
			}
			public virtual Shape GetShape(Window window)
			{
				return window.Shape;
			}
			public virtual void SetShape(Window window, Shape shape)
			{
				window.Shape = shape;
			}
			public virtual void SetOpaque(Window window, bool opaque)
			{
				Color bg = window.Background;
				if (bg == null)
				{
					bg = new Color(0, 0, 0, 0);
				}
				window.Background = new Color(bg.Red, bg.Green, bg.Blue, opaque ? 255 : 0);
			}
			public virtual void UpdateWindow(Window window)
			{
				window.UpdateWindow();
			}

			public virtual Dimension GetSecurityWarningSize(Window window)
			{
				return new Dimension(window.SecurityWarningWidth, window.SecurityWarningHeight);
			}

			public virtual void SetSecurityWarningSize(Window window, int width, int height)
			{
				window.SecurityWarningWidth = width;
				window.SecurityWarningHeight = height;
			}

			public virtual void SetSecurityWarningPosition(Window window, Point2D point, float alignmentX, float alignmentY)
			{
				window.SecurityWarningPointX = point.X;
				window.SecurityWarningPointY = point.Y;
				window.SecurityWarningAlignmentX = alignmentX;
				window.SecurityWarningAlignmentY = alignmentY;

				lock (window.TreeLock)
				{
					WindowPeer peer = (WindowPeer)window.Peer;
					if (peer != null)
					{
						peer.RepositionSecurityWarning();
					}
				}
			}

			public virtual Point2D CalculateSecurityWarningPosition(Window window, double x, double y, double w, double h)
			{
				return window.CalculateSecurityWarningPosition(x, y, w, h);
			}

			public virtual void SetLWRequestStatus(Window changed, bool status)
			{
				changed.SyncLWRequests = status;
			}

			public virtual bool IsAutoRequestFocus(Window w)
			{
				return w.AutoRequestFocus_Renamed;
			}

			public virtual bool IsTrayIconWindow(Window w)
			{
				return w.IsTrayIconWindow;
			}

			public virtual void SetTrayIconWindow(Window w, bool isTrayIconWindow)
			{
				w.IsTrayIconWindow = isTrayIconWindow;
			}
		}

		/// <summary>
		/// Initialize JNI field and method IDs for fields that may be
		///   accessed from C.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		/// <summary>
		/// Constructs a new, initially invisible window in default size with the
		/// specified {@code GraphicsConfiguration}.
		/// <para>
		/// If there is a security manager, then it is invoked to check
		/// {@code AWTPermission("showWindowWithoutWarningBanner")}
		/// to determine whether or not the window must be displayed with
		/// a warning banner.
		/// 
		/// </para>
		/// </summary>
		/// <param name="gc"> the {@code GraphicsConfiguration} of the target screen
		///     device. If {@code gc} is {@code null}, the system default
		///     {@code GraphicsConfiguration} is assumed </param>
		/// <exception cref="IllegalArgumentException"> if {@code gc}
		///    is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     {@code GraphicsEnvironment.isHeadless()} returns {@code true}
		/// </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		internal Window(GraphicsConfiguration gc)
		{
			Init(gc);
		}

		[NonSerialized]
		internal Object Anchor = new Object();
		internal class WindowDisposerRecord : sun.java2d.DisposerRecord
		{
			internal WeakReference<Window> Owner;
			internal readonly WeakReference<Window> WeakThis;
			internal readonly WeakReference<AppContext> Context;

			internal WindowDisposerRecord(AppContext context, Window victim)
			{
				WeakThis = victim.WeakThis;
				this.Context = new WeakReference<AppContext>(context);
			}

			public virtual void UpdateOwner()
			{
				Window victim = WeakThis.get();
				Owner = (victim == null) ? null : new WeakReference<Window>(victim.Owner);
			}

			public virtual void Dispose()
			{
				if (Owner != null)
				{
					Window parent = Owner.get();
					if (parent != null)
					{
						parent.RemoveOwnedWindow(WeakThis);
					}
				}
				AppContext ac = Context.get();
				if (null != ac)
				{
					Window.RemoveFromWindowList(ac, WeakThis);
				}
			}
		}

		private GraphicsConfiguration InitGC(GraphicsConfiguration gc)
		{
			GraphicsEnvironment.CheckHeadless();

			if (gc == null)
			{
				gc = GraphicsEnvironment.LocalGraphicsEnvironment.DefaultScreenDevice.DefaultConfiguration;
			}
			GraphicsConfiguration = gc;

			return gc;
		}

		private void Init(GraphicsConfiguration gc)
		{
			GraphicsEnvironment.CheckHeadless();

			SyncLWRequests = SystemSyncLWRequests;

			WeakThis = new WeakReference<Window>(this);
			AddToWindowList();

			SetWarningString();
			this.Cursor_Renamed = Cursor.GetPredefinedCursor(Cursor.DEFAULT_CURSOR);
			this.Visible_Renamed = false;

			gc = InitGC(gc);

			if (gc.Device.Type != GraphicsDevice.TYPE_RASTER_SCREEN)
			{
				throw new IllegalArgumentException("not a screen device");
			}
			Layout = new BorderLayout();

			/* offset the initial location with the original of the screen */
			/* and any insets                                              */
			Rectangle screenBounds = gc.Bounds;
			Insets screenInsets = Toolkit.GetScreenInsets(gc);
			int x = X + screenBounds.x + screenInsets.Left;
			int y = Y + screenBounds.y + screenInsets.Top;
			if (x != this.x || y != this.y)
			{
				SetLocation(x, y);
				/* reset after setLocation */
				LocationByPlatform = LocationByPlatformProp;
			}

			ModalExclusionType_Renamed = Dialog.ModalExclusionType.NO_EXCLUDE;
			DisposerRecord = new WindowDisposerRecord(AppContext, this);
			Disposer.addRecord(Anchor, DisposerRecord);

			SunToolkit.checkAndSetPolicy(this);
		}

		/// <summary>
		/// Constructs a new, initially invisible window in the default size.
		/// <para>
		/// If there is a security manager set, it is invoked to check
		/// {@code AWTPermission("showWindowWithoutWarningBanner")}.
		/// If that check fails with a {@code SecurityException} then a warning
		/// banner is created.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="HeadlessException"> when
		///     {@code GraphicsEnvironment.isHeadless()} returns {@code true}
		/// </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Window() throws HeadlessException
		internal Window()
		{
			GraphicsEnvironment.CheckHeadless();
			Init((GraphicsConfiguration)null);
		}

		/// <summary>
		/// Constructs a new, initially invisible window with the specified
		/// {@code Frame} as its owner. The window will not be focusable
		/// unless its owner is showing on the screen.
		/// <para>
		/// If there is a security manager set, it is invoked to check
		/// {@code AWTPermission("showWindowWithoutWarningBanner")}.
		/// If that check fails with a {@code SecurityException} then a warning
		/// banner is created.
		/// 
		/// </para>
		/// </summary>
		/// <param name="owner"> the {@code Frame} to act as owner or {@code null}
		///    if this window has no owner </param>
		/// <exception cref="IllegalArgumentException"> if the {@code owner}'s
		///    {@code GraphicsConfiguration} is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///    {@code GraphicsEnvironment.isHeadless} returns {@code true}
		/// </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= #isShowing </seealso>
		public Window(Frame owner) : this(owner == null ? (GraphicsConfiguration)null : owner.GraphicsConfiguration)
		{
			OwnedInit(owner);
		}

		/// <summary>
		/// Constructs a new, initially invisible window with the specified
		/// {@code Window} as its owner. This window will not be focusable
		/// unless its nearest owning {@code Frame} or {@code Dialog}
		/// is showing on the screen.
		/// <para>
		/// If there is a security manager set, it is invoked to check
		/// {@code AWTPermission("showWindowWithoutWarningBanner")}.
		/// If that check fails with a {@code SecurityException} then a
		/// warning banner is created.
		/// 
		/// </para>
		/// </summary>
		/// <param name="owner"> the {@code Window} to act as owner or
		///     {@code null} if this window has no owner </param>
		/// <exception cref="IllegalArgumentException"> if the {@code owner}'s
		///     {@code GraphicsConfiguration} is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     {@code GraphicsEnvironment.isHeadless()} returns
		///     {@code true}
		/// </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       #isShowing
		/// 
		/// @since     1.2 </seealso>
		public Window(Window owner) : this(owner == null ? (GraphicsConfiguration)null : owner.GraphicsConfiguration)
		{
			OwnedInit(owner);
		}

		/// <summary>
		/// Constructs a new, initially invisible window with the specified owner
		/// {@code Window} and a {@code GraphicsConfiguration}
		/// of a screen device. The Window will not be focusable unless
		/// its nearest owning {@code Frame} or {@code Dialog}
		/// is showing on the screen.
		/// <para>
		/// If there is a security manager set, it is invoked to check
		/// {@code AWTPermission("showWindowWithoutWarningBanner")}. If that
		/// check fails with a {@code SecurityException} then a warning banner
		/// is created.
		/// 
		/// </para>
		/// </summary>
		/// <param name="owner"> the window to act as owner or {@code null}
		///     if this window has no owner </param>
		/// <param name="gc"> the {@code GraphicsConfiguration} of the target
		///     screen device; if {@code gc} is {@code null},
		///     the system default {@code GraphicsConfiguration} is assumed </param>
		/// <exception cref="IllegalArgumentException"> if {@code gc}
		///     is not from a screen device </exception>
		/// <exception cref="HeadlessException"> when
		///     {@code GraphicsEnvironment.isHeadless()} returns
		///     {@code true}
		/// </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       GraphicsConfiguration#getBounds </seealso>
		/// <seealso cref=       #isShowing
		/// @since     1.3 </seealso>
		public Window(Window owner, GraphicsConfiguration gc) : this(gc)
		{
			OwnedInit(owner);
		}

		private void OwnedInit(Window owner)
		{
			this.Parent_Renamed = owner;
			if (owner != null)
			{
				owner.AddOwnedWindow(WeakThis);
				if (owner.AlwaysOnTop)
				{
					try
					{
						AlwaysOnTop = true;
					}
					catch (SecurityException)
					{
					}
				}
			}

			// WindowDisposerRecord requires a proper value of parent field.
			DisposerRecord.UpdateOwner();
		}

		/// <summary>
		/// Construct a name for this component.  Called by getName() when the
		/// name is null.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(Window))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Returns the sequence of images to be displayed as the icon for this window.
		/// <para>
		/// This method returns a copy of the internally stored list, so all operations
		/// on the returned object will not affect the window's behavior.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    the copy of icon images' list for this window, or
		///            empty list if this window doesn't have icon images. </returns>
		/// <seealso cref=       #setIconImages </seealso>
		/// <seealso cref=       #setIconImage(Image)
		/// @since     1.6 </seealso>
		public virtual IList<Image> IconImages
		{
			get
			{
				IList<Image> icons = this.Icons;
				if (icons == null || icons.Count == 0)
				{
					return new List<Image>();
				}
				return new List<Image>(icons);
			}
			set
			{
				lock (this)
				{
					this.Icons = (value == null) ? new List<Image>() : new List<Image>(value);
					WindowPeer peer = (WindowPeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.UpdateIconImages();
					}
					// Always send a property change event
					FirePropertyChange("iconImage", null, null);
				}
			}
		}


		/// <summary>
		/// Sets the image to be displayed as the icon for this window.
		/// <para>
		/// This method can be used instead of <seealso cref="#setIconImages setIconImages()"/>
		/// to specify a single image as a window's icon.
		/// </para>
		/// <para>
		/// The following statement:
		/// <pre>
		///     setIconImage(image);
		/// </pre>
		/// is equivalent to:
		/// <pre>
		///     ArrayList&lt;Image&gt; imageList = new ArrayList&lt;Image&gt;();
		///     imageList.add(image);
		///     setIconImages(imageList);
		/// </pre>
		/// </para>
		/// <para>
		/// Note : Native windowing systems may use different images of differing
		/// dimensions to represent a window, depending on the context (e.g.
		/// window decoration, window list, taskbar, etc.). They could also use
		/// just a single image for all contexts or no image at all.
		/// 
		/// </para>
		/// </summary>
		/// <param name="image"> the icon image to be displayed. </param>
		/// <seealso cref=       #setIconImages </seealso>
		/// <seealso cref=       #getIconImages()
		/// @since     1.6 </seealso>
		public virtual Image IconImage
		{
			set
			{
				List<Image> imageList = new List<Image>();
				if (value != null)
				{
					imageList.Add(value);
				}
				IconImages = imageList;
			}
		}

		/// <summary>
		/// Makes this Window displayable by creating the connection to its
		/// native screen resource.
		/// This method is called internally by the toolkit and should
		/// not be called directly by programs. </summary>
		/// <seealso cref= Component#isDisplayable </seealso>
		/// <seealso cref= Container#removeNotify
		/// @since JDK1.0 </seealso>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				Container parent = this.Parent_Renamed;
				if (parent != null && parent.Peer == null)
				{
					parent.AddNotify();
				}
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateWindow(this);
				}
				lock (AllWindows_Renamed)
				{
					AllWindows_Renamed.add(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void RemoveNotify()
		{
			lock (TreeLock)
			{
				lock (AllWindows_Renamed)
				{
					AllWindows_Renamed.remove(this);
				}
				base.RemoveNotify();
			}
		}

		/// <summary>
		/// Causes this Window to be sized to fit the preferred size
		/// and layouts of its subcomponents. The resulting width and
		/// height of the window are automatically enlarged if either
		/// of dimensions is less than the minimum size as specified
		/// by the previous call to the {@code setMinimumSize} method.
		/// <para>
		/// If the window and/or its owner are not displayable yet,
		/// both of them are made displayable before calculating
		/// the preferred size. The Window is validated after its
		/// size is being calculated.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Component#isDisplayable </seealso>
		/// <seealso cref= #setMinimumSize </seealso>
		public virtual void Pack()
		{
			Container parent = this.Parent_Renamed;
			if (parent != null && parent.Peer == null)
			{
				parent.AddNotify();
			}
			if (Peer_Renamed == null)
			{
				AddNotify();
			}
			Dimension newSize = PreferredSize;
			if (Peer_Renamed != null)
			{
				SetClientSize(newSize.Width_Renamed, newSize.Height_Renamed);
			}

			if (BeforeFirstShow)
			{
				IsPacked = true;
			}

			ValidateUnconditionally();
		}

		/// <summary>
		/// Sets the minimum size of this window to a constant
		/// value.  Subsequent calls to {@code getMinimumSize}
		/// will always return this value. If current window's
		/// size is less than {@code minimumSize} the size of the
		/// window is automatically enlarged to honor the minimum size.
		/// <para>
		/// If the {@code setSize} or {@code setBounds} methods
		/// are called afterwards with a width or height less than
		/// that was specified by the {@code setMinimumSize} method
		/// the window is automatically enlarged to meet
		/// the {@code minimumSize} value. The {@code minimumSize}
		/// value also affects the behaviour of the {@code pack} method.
		/// </para>
		/// <para>
		/// The default behavior is restored by setting the minimum size
		/// parameter to the {@code null} value.
		/// </para>
		/// <para>
		/// Resizing operation may be restricted if the user tries
		/// to resize window below the {@code minimumSize} value.
		/// This behaviour is platform-dependent.
		/// 
		/// </para>
		/// </summary>
		/// <param name="minimumSize"> the new minimum size of this window </param>
		/// <seealso cref= Component#setMinimumSize </seealso>
		/// <seealso cref= #getMinimumSize </seealso>
		/// <seealso cref= #isMinimumSizeSet </seealso>
		/// <seealso cref= #setSize(Dimension) </seealso>
		/// <seealso cref= #pack
		/// @since 1.6 </seealso>
		public override Dimension MinimumSize
		{
			set
			{
				lock (TreeLock)
				{
					base.MinimumSize = value;
					Dimension size = Size;
					if (MinimumSizeSet)
					{
						if (size.Width_Renamed < value.Width_Renamed || size.Height_Renamed < value.Height_Renamed)
						{
							int nw = System.Math.Max(Width_Renamed, value.Width_Renamed);
							int nh = System.Math.Max(Height_Renamed, value.Height_Renamed);
							SetSize(nw, nh);
						}
					}
					if (Peer_Renamed != null)
					{
						((WindowPeer)Peer_Renamed).UpdateMinimumSize();
					}
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// The {@code d.width} and {@code d.height} values
		/// are automatically enlarged if either is less than
		/// the minimum size as specified by previous call to
		/// {@code setMinimumSize}.
		/// </para>
		/// <para>
		/// The method changes the geometry-related data. Therefore,
		/// the native windowing system may ignore such requests, or it may modify
		/// the requested data, so that the {@code Window} object is placed and sized
		/// in a way that corresponds closely to the desktop settings.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #getSize </seealso>
		/// <seealso cref= #setBounds </seealso>
		/// <seealso cref= #setMinimumSize
		/// @since 1.6 </seealso>
		public override Dimension Size
		{
			set
			{
				base.Size = value;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// The {@code width} and {@code height} values
		/// are automatically enlarged if either is less than
		/// the minimum size as specified by previous call to
		/// {@code setMinimumSize}.
		/// </para>
		/// <para>
		/// The method changes the geometry-related data. Therefore,
		/// the native windowing system may ignore such requests, or it may modify
		/// the requested data, so that the {@code Window} object is placed and sized
		/// in a way that corresponds closely to the desktop settings.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #getSize </seealso>
		/// <seealso cref= #setBounds </seealso>
		/// <seealso cref= #setMinimumSize
		/// @since 1.6 </seealso>
		public override void SetSize(int width, int height)
		{
			base.SetSize(width, height);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// The method changes the geometry-related data. Therefore,
		/// the native windowing system may ignore such requests, or it may modify
		/// the requested data, so that the {@code Window} object is placed and sized
		/// in a way that corresponds closely to the desktop settings.
		/// </para>
		/// </summary>
		public override void SetLocation(int x, int y)
		{
			base.SetLocation(x, y);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// The method changes the geometry-related data. Therefore,
		/// the native windowing system may ignore such requests, or it may modify
		/// the requested data, so that the {@code Window} object is placed and sized
		/// in a way that corresponds closely to the desktop settings.
		/// </para>
		/// </summary>
		public override Point Location
		{
			set
			{
				base.Location = value;
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by {@code setBounds(int, int, int, int)}. 
		[Obsolete("As of JDK version 1.1,")]
		public override void Reshape(int x, int y, int width, int height)
		{
			if (MinimumSizeSet)
			{
				Dimension minSize = MinimumSize;
				if (width < minSize.Width_Renamed)
				{
					width = minSize.Width_Renamed;
				}
				if (height < minSize.Height_Renamed)
				{
					height = minSize.Height_Renamed;
				}
			}
			base.Reshape(x, y, width, height);
		}

		internal virtual void SetClientSize(int w, int h)
		{
			lock (TreeLock)
			{
				BoundsOp = java.awt.peer.ComponentPeer_Fields.SET_CLIENT_SIZE;
				SetBounds(x, y, w, h);
			}
		}

		private static readonly AtomicBoolean BeforeFirstWindowShown = new AtomicBoolean(true);

		internal void CloseSplashScreen()
		{
			if (IsTrayIconWindow)
			{
				return;
			}
			if (BeforeFirstWindowShown.GetAndSet(false))
			{
				// We don't use SplashScreen.getSplashScreen() to avoid instantiating
				// the object if it hasn't been requested by user code explicitly
				SunToolkit.closeSplashScreen();
				SplashScreen.MarkClosed();
			}
		}

		/// <summary>
		/// Shows or hides this {@code Window} depending on the value of parameter
		/// {@code b}.
		/// <para>
		/// If the method shows the window then the window is also made
		/// focused under the following conditions:
		/// <ul>
		/// <li> The {@code Window} meets the requirements outlined in the
		///      <seealso cref="#isFocusableWindow"/> method.
		/// <li> The {@code Window}'s {@code autoRequestFocus} property is of the {@code true} value.
		/// <li> Native windowing system allows the {@code Window} to get focused.
		/// </ul>
		/// There is an exception for the second condition (the value of the
		/// {@code autoRequestFocus} property). The property is not taken into account if the
		/// window is a modal dialog, which blocks the currently focused window.
		/// </para>
		/// <para>
		/// Developers must never assume that the window is the focused or active window
		/// until it receives a WINDOW_GAINED_FOCUS or WINDOW_ACTIVATED event.
		/// </para>
		/// </summary>
		/// <param name="b">  if {@code true}, makes the {@code Window} visible,
		/// otherwise hides the {@code Window}.
		/// If the {@code Window} and/or its owner
		/// are not yet displayable, both are made displayable.  The
		/// {@code Window} will be validated prior to being made visible.
		/// If the {@code Window} is already visible, this will bring the
		/// {@code Window} to the front.<para>
		/// If {@code false}, hides this {@code Window}, its subcomponents, and all
		/// of its owned children.
		/// The {@code Window} and its subcomponents can be made visible again
		/// with a call to {@code #setVisible(true)}.
		/// </para>
		/// </param>
		/// <seealso cref= java.awt.Component#isDisplayable </seealso>
		/// <seealso cref= java.awt.Component#setVisible </seealso>
		/// <seealso cref= java.awt.Window#toFront </seealso>
		/// <seealso cref= java.awt.Window#dispose </seealso>
		/// <seealso cref= java.awt.Window#setAutoRequestFocus </seealso>
		/// <seealso cref= java.awt.Window#isFocusableWindow </seealso>
		public override bool Visible
		{
			set
			{
				base.Visible = value;
			}
		}

		/// <summary>
		/// Makes the Window visible. If the Window and/or its owner
		/// are not yet displayable, both are made displayable.  The
		/// Window will be validated prior to being made visible.
		/// If the Window is already visible, this will bring the Window
		/// to the front. </summary>
		/// <seealso cref=       Component#isDisplayable </seealso>
		/// <seealso cref=       #toFront </seealso>
		/// @deprecated As of JDK version 1.5, replaced by
		/// <seealso cref="#setVisible(boolean)"/>. 
		[Obsolete("As of JDK version 1.5, replaced by")]
		public override void Show()
		{
			if (Peer_Renamed == null)
			{
				AddNotify();
			}
			ValidateUnconditionally();

			IsInShow = true;
			if (Visible_Renamed)
			{
				ToFront();
			}
			else
			{
				BeforeFirstShow = false;
				CloseSplashScreen();
				Dialog.CheckShouldBeBlocked(this);
				base.Show();
				lock (TreeLock)
				{
					this.LocationByPlatform_Renamed = false;
				}
				for (int i = 0; i < OwnedWindowList.Count; i++)
				{
					Window child = OwnedWindowList[i].get();
					if ((child != null) && child.ShowWithParent)
					{
						child.Show();
						child.ShowWithParent = false;
					} // endif
				} // endfor
				if (!ModalBlocked)
				{
					UpdateChildrenBlocking();
				}
				else
				{
					// fix for 6532736: after this window is shown, its blocker
					// should be raised to front
					ModalBlocker_Renamed.ToFront_NoClientCode();
				}
				if (this is Frame || this is Dialog)
				{
					UpdateChildFocusableWindowState(this);
				}
			}
			IsInShow = false;

			// If first time shown, generate WindowOpened event
			if ((State & OPENED) == 0)
			{
				PostWindowEvent(WindowEvent.WINDOW_OPENED);
				State |= OPENED;
			}
		}

		internal static void UpdateChildFocusableWindowState(Window w)
		{
			if (w.Peer != null && w.Showing)
			{
				((WindowPeer)w.Peer).UpdateFocusableWindowState();
			}
			for (int i = 0; i < w.OwnedWindowList.Count; i++)
			{
				Window child = w.OwnedWindowList[i].get();
				if (child != null)
				{
					UpdateChildFocusableWindowState(child);
				}
			}
		}

		internal virtual void PostWindowEvent(int id)
		{
			lock (this)
			{
				if (WindowListener != null || (EventMask & AWTEvent.WINDOW_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.WINDOW_EVENT_MASK))
				{
					WindowEvent e = new WindowEvent(this, id);
					Toolkit.EventQueue.PostEvent(e);
				}
			}
		}

		/// <summary>
		/// Hide this Window, its subcomponents, and all of its owned children.
		/// The Window and its subcomponents can be made visible again
		/// with a call to {@code show}. </summary>
		/// <seealso cref= #show </seealso>
		/// <seealso cref= #dispose </seealso>
		/// @deprecated As of JDK version 1.5, replaced by
		/// <seealso cref="#setVisible(boolean)"/>. 
		[Obsolete("As of JDK version 1.5, replaced by")]
		public override void Hide()
		{
			lock (OwnedWindowList)
			{
				for (int i = 0; i < OwnedWindowList.Count; i++)
				{
					Window child = OwnedWindowList[i].get();
					if ((child != null) && child.Visible_Renamed)
					{
						child.Hide();
						child.ShowWithParent = true;
					}
				}
			}
			if (ModalBlocked)
			{
				ModalBlocker_Renamed.UnblockWindow(this);
			}
			base.Hide();
			lock (TreeLock)
			{
				this.LocationByPlatform_Renamed = false;
			}
		}

		internal sealed override void ClearMostRecentFocusOwnerOnHide()
		{
			/* do nothing */
		}

		/// <summary>
		/// Releases all of the native screen resources used by this
		/// {@code Window}, its subcomponents, and all of its owned
		/// children. That is, the resources for these {@code Component}s
		/// will be destroyed, any memory they consume will be returned to the
		/// OS, and they will be marked as undisplayable.
		/// <para>
		/// The {@code Window} and its subcomponents can be made displayable
		/// again by rebuilding the native resources with a subsequent call to
		/// {@code pack} or {@code show}. The states of the recreated
		/// {@code Window} and its subcomponents will be identical to the
		/// states of these objects at the point where the {@code Window}
		/// was disposed (not accounting for additional modifications between
		/// those actions).
		/// </para>
		/// <para>
		/// <b>Note</b>: When the last displayable window
		/// within the Java virtual machine (VM) is disposed of, the VM may
		/// terminate.  See <a href="doc-files/AWTThreadIssues.html#Autoshutdown">
		/// AWT Threading Issues</a> for more information.
		/// </para>
		/// </summary>
		/// <seealso cref= Component#isDisplayable </seealso>
		/// <seealso cref= #pack </seealso>
		/// <seealso cref= #show </seealso>
		public virtual void Dispose()
		{
			DoDispose();
		}

		/*
		 * Fix for 4872170.
		 * If dispose() is called on parent then its children have to be disposed as well
		 * as reported in javadoc. So we need to implement this functionality even if a
		 * child overrides dispose() in a wrong way without calling super.dispose().
		 */
		internal virtual void DisposeImpl()
		{
			Dispose();
			if (Peer != null)
			{
				DoDispose();
			}
		}

		internal virtual void DoDispose()
		{
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//		class DisposeAction implements Runnable
	//	{
	//		public void run()
	//		{
	//			disposing = true;
	//			try
	//			{
	//				// Check if this window is the fullscreen window for the
	//				// device. Exit the fullscreen mode prior to disposing
	//				// of the window if that's the case.
	//				GraphicsDevice gd = getGraphicsConfiguration().getDevice();
	//				if (gd.getFullScreenWindow() == Window.this)
	//				{
	//					gd.setFullScreenWindow(null);
	//				}
	//
	//				Object[] ownedWindowArray;
	//				synchronized(ownedWindowList)
	//				{
	//					ownedWindowArray = new Object[ownedWindowList.size()];
	//					ownedWindowList.copyInto(ownedWindowArray);
	//				}
	//				for (int i = 0; i < ownedWindowArray.length; i++)
	//				{
	//					Window child = (Window)(((WeakReference)(ownedWindowArray[i])).get());
	//					if (child != null)
	//					{
	//						child.disposeImpl();
	//					}
	//				}
	//				hide();
	//				beforeFirstShow = true;
	//				removeNotify();
	//				synchronized(inputContextLock)
	//				{
	//					if (inputContext != null)
	//					{
	//						inputContext.dispose();
	//						inputContext = null;
	//					}
	//				}
	//				clearCurrentFocusCycleRootOnHide();
	//			}
	//			finally
	//			{
	//				disposing = false;
	//			}
	//		}
	//	}
			bool fireWindowClosedEvent = Displayable;
			DisposeAction action = new DisposeAction();
			if (EventQueue.DispatchThread)
			{
				action.run();
			}
			else
			{
				try
				{
					EventQueue.InvokeAndWait(this, action);
				}
				catch (InterruptedException e)
				{
					System.Console.Error.WriteLine("Disposal was interrupted:");
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				catch (InvocationTargetException e)
				{
					System.Console.Error.WriteLine("Exception during disposal:");
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
			// Execute outside the Runnable because postWindowEvent is
			// synchronized on (this). We don't need to synchronize the call
			// on the EventQueue anyways.
			if (fireWindowClosedEvent)
			{
				PostWindowEvent(WindowEvent.WINDOW_CLOSED);
			}
		}

		/*
		 * Should only be called while holding the tree lock.
		 * It's overridden here because parent == owner in Window,
		 * and we shouldn't adjust counter on owner
		 */
		internal override void AdjustListeningChildrenOnParent(long mask, int num)
		{
		}

		// Should only be called while holding tree lock
		internal override void AdjustDecendantsOnParent(int num)
		{
			// do nothing since parent == owner and we shouldn't
			// ajust counter on owner
		}

		/// <summary>
		/// If this Window is visible, brings this Window to the front and may make
		/// it the focused Window.
		/// <para>
		/// Places this Window at the top of the stacking order and shows it in
		/// front of any other Windows in this VM. No action will take place if this
		/// Window is not visible. Some platforms do not allow Windows which own
		/// other Windows to appear on top of those owned Windows. Some platforms
		/// may not permit this VM to place its Windows above windows of native
		/// applications, or Windows of other VMs. This permission may depend on
		/// whether a Window in this VM is already focused. Every attempt will be
		/// made to move this Window as high as possible in the stacking order;
		/// however, developers should not assume that this method will move this
		/// Window above all other windows in every situation.
		/// </para>
		/// <para>
		/// Developers must never assume that this Window is the focused or active
		/// Window until this Window receives a WINDOW_GAINED_FOCUS or WINDOW_ACTIVATED
		/// event. On platforms where the top-most window is the focused window, this
		/// method will <b>probably</b> focus this Window (if it is not already focused)
		/// under the following conditions:
		/// <ul>
		/// <li> The window meets the requirements outlined in the
		///      <seealso cref="#isFocusableWindow"/> method.
		/// <li> The window's property {@code autoRequestFocus} is of the
		///      {@code true} value.
		/// <li> Native windowing system allows the window to get focused.
		/// </ul>
		/// On platforms where the stacking order does not typically affect the focused
		/// window, this method will <b>probably</b> leave the focused and active
		/// Windows unchanged.
		/// </para>
		/// <para>
		/// If this method causes this Window to be focused, and this Window is a
		/// Frame or a Dialog, it will also become activated. If this Window is
		/// focused, but it is not a Frame or a Dialog, then the first Frame or
		/// Dialog that is an owner of this Window will be activated.
		/// </para>
		/// <para>
		/// If this window is blocked by modal dialog, then the blocking dialog
		/// is brought to the front and remains above the blocked window.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=       #toBack </seealso>
		/// <seealso cref=       #setAutoRequestFocus </seealso>
		/// <seealso cref=       #isFocusableWindow </seealso>
		public virtual void ToFront()
		{
			ToFront_NoClientCode();
		}

		// This functionality is implemented in a final package-private method
		// to insure that it cannot be overridden by client subclasses.
		internal void ToFront_NoClientCode()
		{
			if (Visible_Renamed)
			{
				WindowPeer peer = (WindowPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.ToFront();
				}
				if (ModalBlocked)
				{
					ModalBlocker_Renamed.ToFront_NoClientCode();
				}
			}
		}

		/// <summary>
		/// If this Window is visible, sends this Window to the back and may cause
		/// it to lose focus or activation if it is the focused or active Window.
		/// <para>
		/// Places this Window at the bottom of the stacking order and shows it
		/// behind any other Windows in this VM. No action will take place is this
		/// Window is not visible. Some platforms do not allow Windows which are
		/// owned by other Windows to appear below their owners. Every attempt will
		/// be made to move this Window as low as possible in the stacking order;
		/// however, developers should not assume that this method will move this
		/// Window below all other windows in every situation.
		/// </para>
		/// <para>
		/// Because of variations in native windowing systems, no guarantees about
		/// changes to the focused and active Windows can be made. Developers must
		/// never assume that this Window is no longer the focused or active Window
		/// until this Window receives a WINDOW_LOST_FOCUS or WINDOW_DEACTIVATED
		/// event. On platforms where the top-most window is the focused window,
		/// this method will <b>probably</b> cause this Window to lose focus. In
		/// that case, the next highest, focusable Window in this VM will receive
		/// focus. On platforms where the stacking order does not typically affect
		/// the focused window, this method will <b>probably</b> leave the focused
		/// and active Windows unchanged.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=       #toFront </seealso>
		public virtual void ToBack()
		{
			ToBack_NoClientCode();
		}

		// This functionality is implemented in a final package-private method
		// to insure that it cannot be overridden by client subclasses.
		internal void ToBack_NoClientCode()
		{
			if (AlwaysOnTop)
			{
				try
				{
					AlwaysOnTop = false;
				}
				catch (SecurityException)
				{
				}
			}
			if (Visible_Renamed)
			{
				WindowPeer peer = (WindowPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.ToBack();
				}
			}
		}

		/// <summary>
		/// Returns the toolkit of this frame. </summary>
		/// <returns>    the toolkit of this window. </returns>
		/// <seealso cref=       Toolkit </seealso>
		/// <seealso cref=       Toolkit#getDefaultToolkit </seealso>
		/// <seealso cref=       Component#getToolkit </seealso>
		public override Toolkit Toolkit
		{
			get
			{
				return Toolkit.DefaultToolkit;
			}
		}

		/// <summary>
		/// Gets the warning string that is displayed with this window.
		/// If this window is insecure, the warning string is displayed
		/// somewhere in the visible area of the window. A window is
		/// insecure if there is a security manager and the security
		/// manager denies
		/// {@code AWTPermission("showWindowWithoutWarningBanner")}.
		/// <para>
		/// If the window is secure, then {@code getWarningString}
		/// returns {@code null}. If the window is insecure, this
		/// method checks for the system property
		/// {@code awt.appletWarning}
		/// and returns the string value of that property.
		/// </para>
		/// </summary>
		/// <returns>    the warning string for this window. </returns>
		public String WarningString
		{
			get
			{
				return WarningString_Renamed;
			}
		}

		private void SetWarningString()
		{
			WarningString_Renamed = null;
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				try
				{
					sm.CheckPermission(SecurityConstants.AWT.TOPLEVEL_WINDOW_PERMISSION);
				}
				catch (SecurityException)
				{
					// make sure the privileged action is only
					// for getting the property! We don't want the
					// above checkPermission call to always succeed!
					WarningString_Renamed = AccessController.doPrivileged(new GetPropertyAction("awt.appletWarning", "Java Applet Window"));
				}
			}
		}

		/// <summary>
		/// Gets the {@code Locale} object that is associated
		/// with this window, if the locale has been set.
		/// If no locale has been set, then the default locale
		/// is returned. </summary>
		/// <returns>    the locale that is set for this window. </returns>
		/// <seealso cref=       java.util.Locale
		/// @since     JDK1.1 </seealso>
		public override Locale Locale
		{
			get
			{
			  if (this.Locale_Renamed == null)
			  {
				return Locale.Default;
			  }
			  return this.Locale_Renamed;
			}
		}

		/// <summary>
		/// Gets the input context for this window. A window always has an input context,
		/// which is shared by subcomponents unless they create and set their own. </summary>
		/// <seealso cref= Component#getInputContext
		/// @since 1.2 </seealso>
		public override InputContext InputContext
		{
			get
			{
				lock (InputContextLock)
				{
					if (InputContext_Renamed == null)
					{
						InputContext_Renamed = InputContext.Instance;
					}
				}
				return InputContext_Renamed;
			}
		}

		/// <summary>
		/// Set the cursor image to a specified cursor.
		/// <para>
		/// The method may have no visual effect if the Java platform
		/// implementation and/or the native system do not support
		/// changing the mouse cursor shape.
		/// </para>
		/// </summary>
		/// <param name="cursor"> One of the constants defined
		///            by the {@code Cursor} class. If this parameter is null
		///            then the cursor for this window will be set to the type
		///            Cursor.DEFAULT_CURSOR. </param>
		/// <seealso cref=       Component#getCursor </seealso>
		/// <seealso cref=       Cursor
		/// @since     JDK1.1 </seealso>
		public override Cursor Cursor
		{
			set
			{
				if (value == null)
				{
					value = Cursor.GetPredefinedCursor(Cursor.DEFAULT_CURSOR);
				}
				base.Cursor = value;
			}
		}

		/// <summary>
		/// Returns the owner of this window.
		/// @since 1.2
		/// </summary>
		public virtual Window Owner
		{
			get
			{
				return Owner_NoClientCode;
			}
		}
		internal Window Owner_NoClientCode
		{
			get
			{
				return (Window)Parent_Renamed;
			}
		}

		/// <summary>
		/// Return an array containing all the windows this
		/// window currently owns.
		/// @since 1.2
		/// </summary>
		public virtual Window[] OwnedWindows
		{
			get
			{
				return OwnedWindows_NoClientCode;
			}
		}
		internal Window[] OwnedWindows_NoClientCode
		{
			get
			{
				Window[] realCopy;
    
				lock (OwnedWindowList)
				{
					// Recall that ownedWindowList is actually a Vector of
					// WeakReferences and calling get() on one of these references
					// may return null. Make two arrays-- one the size of the
					// Vector (fullCopy with size fullSize), and one the size of
					// all non-null get()s (realCopy with size realSize).
					int fullSize = OwnedWindowList.Count;
					int realSize = 0;
					Window[] fullCopy = new Window[fullSize];
    
					for (int i = 0; i < fullSize; i++)
					{
						fullCopy[realSize] = OwnedWindowList[i].get();
    
						if (fullCopy[realSize] != null)
						{
							realSize++;
						}
					}
    
					if (fullSize != realSize)
					{
						realCopy = Arrays.CopyOf(fullCopy, realSize);
					}
					else
					{
						realCopy = fullCopy;
					}
				}
    
				return realCopy;
			}
		}

		internal virtual bool ModalBlocked
		{
			get
			{
				return ModalBlocker_Renamed != null;
			}
		}

		internal virtual void SetModalBlocked(Dialog blocker, bool blocked, bool peerCall)
		{
			this.ModalBlocker_Renamed = blocked ? blocker : null;
			if (peerCall)
			{
				WindowPeer peer = (WindowPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.SetModalBlocked(blocker, blocked);
				}
			}
		}

		internal virtual Dialog ModalBlocker
		{
			get
			{
				return ModalBlocker_Renamed;
			}
		}

		/*
		 * Returns a list of all displayable Windows, i. e. all the
		 * Windows which peer is not null.
		 *
		 * @see #addNotify
		 * @see #removeNotify
		 */
		internal static IdentityArrayList<Window> AllWindows
		{
			get
			{
				lock (AllWindows_Renamed)
				{
					IdentityArrayList<Window> v = new IdentityArrayList<Window>();
					v.addAll(AllWindows_Renamed);
					return v;
				}
			}
		}

		internal static IdentityArrayList<Window> AllUnblockedWindows
		{
			get
			{
				lock (AllWindows_Renamed)
				{
					IdentityArrayList<Window> unblocked = new IdentityArrayList<Window>();
					for (int i = 0; i < AllWindows_Renamed.size(); i++)
					{
						Window w = AllWindows_Renamed.get(i);
						if (!w.ModalBlocked)
						{
							unblocked.add(w);
						}
					}
					return unblocked;
				}
			}
		}

		private static Window[] GetWindows(AppContext appContext)
		{
			lock (typeof(Window))
			{
				Window[] realCopy;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Vector<WeakReference<Window>> windowList = (java.util.Vector<WeakReference<Window>>)appContext.get(Window.class);
				List<WeakReference<Window>> windowList = (List<WeakReference<Window>>)appContext.get(typeof(Window));
				if (windowList != null)
				{
					int fullSize = windowList.Count;
					int realSize = 0;
					Window[] fullCopy = new Window[fullSize];
					for (int i = 0; i < fullSize; i++)
					{
						Window w = windowList[i].get();
						if (w != null)
						{
							fullCopy[realSize++] = w;
						}
					}
					if (fullSize != realSize)
					{
						realCopy = Arrays.CopyOf(fullCopy, realSize);
					}
					else
					{
						realCopy = fullCopy;
					}
				}
				else
				{
					realCopy = new Window[0];
				}
				return realCopy;
			}
		}

		/// <summary>
		/// Returns an array of all {@code Window}s, both owned and ownerless,
		/// created by this application.
		/// If called from an applet, the array includes only the {@code Window}s
		/// accessible by that applet.
		/// <para>
		/// <b>Warning:</b> this method may return system created windows, such
		/// as a print dialog. Applications should not assume the existence of
		/// these dialogs, nor should an application assume anything about these
		/// dialogs such as component positions, {@code LayoutManager}s
		/// or serialization.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Frame#getFrames </seealso>
		/// <seealso cref= Window#getOwnerlessWindows
		/// 
		/// @since 1.6 </seealso>
		public static Window[] Windows
		{
			get
			{
				return GetWindows(AppContext.AppContext);
			}
		}

		/// <summary>
		/// Returns an array of all {@code Window}s created by this application
		/// that have no owner. They include {@code Frame}s and ownerless
		/// {@code Dialog}s and {@code Window}s.
		/// If called from an applet, the array includes only the {@code Window}s
		/// accessible by that applet.
		/// <para>
		/// <b>Warning:</b> this method may return system created windows, such
		/// as a print dialog. Applications should not assume the existence of
		/// these dialogs, nor should an application assume anything about these
		/// dialogs such as component positions, {@code LayoutManager}s
		/// or serialization.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Frame#getFrames </seealso>
		/// <seealso cref= Window#getWindows()
		/// 
		/// @since 1.6 </seealso>
		public static Window[] OwnerlessWindows
		{
			get
			{
				Window[] allWindows = Window.Windows;
    
				int ownerlessCount = 0;
				foreach (Window w in allWindows)
				{
					if (w.Owner == null)
					{
						ownerlessCount++;
					}
				}
    
				Window[] ownerless = new Window[ownerlessCount];
				int c = 0;
				foreach (Window w in allWindows)
				{
					if (w.Owner == null)
					{
						ownerless[c++] = w;
					}
				}
    
				return ownerless;
			}
		}

		internal virtual Window DocumentRoot
		{
			get
			{
				lock (TreeLock)
				{
					Window w = this;
					while (w.Owner != null)
					{
						w = w.Owner;
					}
					return w;
				}
			}
		}

		/// <summary>
		/// Specifies the modal exclusion type for this window. If a window is modal
		/// excluded, it is not blocked by some modal dialogs. See {@link
		/// java.awt.Dialog.ModalExclusionType Dialog.ModalExclusionType} for
		/// possible modal exclusion types.
		/// <para>
		/// If the given type is not supported, {@code NO_EXCLUDE} is used.
		/// </para>
		/// <para>
		/// Note: changing the modal exclusion type for a visible window may have no
		/// effect until it is hidden and then shown again.
		/// 
		/// </para>
		/// </summary>
		/// <param name="exclusionType"> the modal exclusion type for this window; a {@code null}
		///     value is equivalent to {@link Dialog.ModalExclusionType#NO_EXCLUDE
		///     NO_EXCLUDE} </param>
		/// <exception cref="SecurityException"> if the calling thread does not have permission
		///     to set the modal exclusion property to the window with the given
		///     {@code exclusionType} </exception>
		/// <seealso cref= java.awt.Dialog.ModalExclusionType </seealso>
		/// <seealso cref= java.awt.Window#getModalExclusionType </seealso>
		/// <seealso cref= java.awt.Toolkit#isModalExclusionTypeSupported
		/// 
		/// @since 1.6 </seealso>
		public virtual Dialog.ModalExclusionType ModalExclusionType
		{
			set
			{
				if (value == null)
				{
					value = Dialog.ModalExclusionType.NO_EXCLUDE;
				}
				if (!Toolkit.DefaultToolkit.IsModalExclusionTypeSupported(value))
				{
					value = Dialog.ModalExclusionType.NO_EXCLUDE;
				}
				if (ModalExclusionType_Renamed == value)
				{
					return;
				}
				if (value == Dialog.ModalExclusionType.TOOLKIT_EXCLUDE)
				{
					SecurityManager sm = System.SecurityManager;
					if (sm != null)
					{
						sm.CheckPermission(SecurityConstants.AWT.TOOLKIT_MODALITY_PERMISSION);
					}
				}
				ModalExclusionType_Renamed = value;
    
				// if we want on-fly changes, we need to uncomment the lines below
				//   and override the method in Dialog to use modalShow() instead
				//   of updateChildrenBlocking()
		 /*
		        if (isModalBlocked()) {
		            modalBlocker.unblockWindow(this);
		        }
		        Dialog.checkShouldBeBlocked(this);
		        updateChildrenBlocking();
		 */
			}
			get
			{
				return ModalExclusionType_Renamed;
			}
		}


		internal virtual bool IsModalExcluded(Dialog.ModalExclusionType exclusionType)
		{
			if ((ModalExclusionType_Renamed != null) && ModalExclusionType_Renamed.CompareTo(exclusionType) >= 0)
			{
				return true;
			}
			Window owner = Owner_NoClientCode;
			return (owner != null) && owner.IsModalExcluded(exclusionType);
		}

		internal virtual void UpdateChildrenBlocking()
		{
			List<Window> childHierarchy = new List<Window>();
			Window[] ownedWindows = OwnedWindows;
			for (int i = 0; i < ownedWindows.Length; i++)
			{
				childHierarchy.Add(ownedWindows[i]);
			}
			int k = 0;
			while (k < childHierarchy.Count)
			{
				Window w = childHierarchy[k];
				if (w.Visible)
				{
					if (w.ModalBlocked)
					{
						Dialog blocker = w.ModalBlocker;
						blocker.UnblockWindow(w);
					}
					Dialog.CheckShouldBeBlocked(w);
					Window[] wOwned = w.OwnedWindows;
					for (int j = 0; j < wOwned.Length; j++)
					{
						childHierarchy.Add(wOwned[j]);
					}
				}
				k++;
			}
		}

		/// <summary>
		/// Adds the specified window listener to receive window events from
		/// this window.
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the window listener </param>
		/// <seealso cref= #removeWindowListener </seealso>
		/// <seealso cref= #getWindowListeners </seealso>
		public virtual void AddWindowListener(WindowListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				NewEventsOnly = true;
				WindowListener = AWTEventMulticaster.Add(WindowListener, l);
			}
		}

		/// <summary>
		/// Adds the specified window state listener to receive window
		/// events from this window.  If {@code l} is {@code null},
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the window state listener </param>
		/// <seealso cref= #removeWindowStateListener </seealso>
		/// <seealso cref= #getWindowStateListeners
		/// @since 1.4 </seealso>
		public virtual void AddWindowStateListener(WindowStateListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				WindowStateListener = AWTEventMulticaster.Add(WindowStateListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Adds the specified window focus listener to receive window events
		/// from this window.
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the window focus listener </param>
		/// <seealso cref= #removeWindowFocusListener </seealso>
		/// <seealso cref= #getWindowFocusListeners
		/// @since 1.4 </seealso>
		public virtual void AddWindowFocusListener(WindowFocusListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				WindowFocusListener = AWTEventMulticaster.Add(WindowFocusListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Removes the specified window listener so that it no longer
		/// receives window events from this window.
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the window listener </param>
		/// <seealso cref= #addWindowListener </seealso>
		/// <seealso cref= #getWindowListeners </seealso>
		public virtual void RemoveWindowListener(WindowListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				WindowListener = AWTEventMulticaster.Remove(WindowListener, l);
			}
		}

		/// <summary>
		/// Removes the specified window state listener so that it no
		/// longer receives window events from this window.  If
		/// {@code l} is {@code null}, no exception is thrown and
		/// no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the window state listener </param>
		/// <seealso cref= #addWindowStateListener </seealso>
		/// <seealso cref= #getWindowStateListeners
		/// @since 1.4 </seealso>
		public virtual void RemoveWindowStateListener(WindowStateListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				WindowStateListener = AWTEventMulticaster.Remove(WindowStateListener, l);
			}
		}

		/// <summary>
		/// Removes the specified window focus listener so that it no longer
		/// receives window events from this window.
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the window focus listener </param>
		/// <seealso cref= #addWindowFocusListener </seealso>
		/// <seealso cref= #getWindowFocusListeners
		/// @since 1.4 </seealso>
		public virtual void RemoveWindowFocusListener(WindowFocusListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				WindowFocusListener = AWTEventMulticaster.Remove(WindowFocusListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the window listeners
		/// registered on this window.
		/// </summary>
		/// <returns> all of this window's {@code WindowListener}s
		///         or an empty array if no window
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref= #addWindowListener </seealso>
		/// <seealso cref= #removeWindowListener
		/// @since 1.4 </seealso>
		public virtual WindowListener[] WindowListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(WindowListener));
				}
			}
		}

		/// <summary>
		/// Returns an array of all the window focus listeners
		/// registered on this window.
		/// </summary>
		/// <returns> all of this window's {@code WindowFocusListener}s
		///         or an empty array if no window focus
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref= #addWindowFocusListener </seealso>
		/// <seealso cref= #removeWindowFocusListener
		/// @since 1.4 </seealso>
		public virtual WindowFocusListener[] WindowFocusListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(WindowFocusListener));
				}
			}
		}

		/// <summary>
		/// Returns an array of all the window state listeners
		/// registered on this window.
		/// </summary>
		/// <returns> all of this window's {@code WindowStateListener}s
		///         or an empty array if no window state
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref= #addWindowStateListener </seealso>
		/// <seealso cref= #removeWindowStateListener
		/// @since 1.4 </seealso>
		public virtual WindowStateListener[] WindowStateListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(WindowStateListener));
				}
			}
		}


		/// <summary>
		/// Returns an array of all the objects currently registered
		/// as <code><em>Foo</em>Listener</code>s
		/// upon this {@code Window}.
		/// <code><em>Foo</em>Listener</code>s are registered using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// 
		/// <para>
		/// 
		/// You can specify the {@code listenerType} argument
		/// with a class literal, such as
		/// <code><em>Foo</em>Listener.class</code>.
		/// For example, you can query a
		/// {@code Window} {@code w}
		/// for its window listeners with the following code:
		/// 
		/// <pre>WindowListener[] wls = (WindowListener[])(w.getListeners(WindowListener.class));</pre>
		/// 
		/// If no such listeners exist, this method returns an empty array.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listenerType"> the type of listeners requested; this parameter
		///          should specify an interface that descends from
		///          {@code java.util.EventListener} </param>
		/// <returns> an array of all objects registered as
		///          <code><em>Foo</em>Listener</code>s on this window,
		///          or an empty array if no such
		///          listeners have been added </returns>
		/// <exception cref="ClassCastException"> if {@code listenerType}
		///          doesn't specify a class or interface that implements
		///          {@code java.util.EventListener} </exception>
		/// <exception cref="NullPointerException"> if {@code listenerType} is {@code null}
		/// </exception>
		/// <seealso cref= #getWindowListeners
		/// @since 1.3 </seealso>
		public override T[] getListeners<T>(Class listenerType) where T : java.util.EventListener
		{
			EventListener l = null;
			if (listenerType == typeof(WindowFocusListener))
			{
				l = WindowFocusListener;
			}
			else if (listenerType == typeof(WindowStateListener))
			{
				l = WindowStateListener;
			}
			else if (listenerType == typeof(WindowListener))
			{
				l = WindowListener;
			}
			else
			{
				return base.GetListeners(listenerType);
			}
			return AWTEventMulticaster.GetListeners(l, listenerType);
		}

		// REMIND: remove when filtering is handled at lower level
		internal override bool EventEnabled(AWTEvent e)
		{
			switch (e.Id)
			{
			  case WindowEvent.WINDOW_OPENED:
			  case WindowEvent.WINDOW_CLOSING:
			  case WindowEvent.WINDOW_CLOSED:
			  case WindowEvent.WINDOW_ICONIFIED:
			  case WindowEvent.WINDOW_DEICONIFIED:
			  case WindowEvent.WINDOW_ACTIVATED:
			  case WindowEvent.WINDOW_DEACTIVATED:
				if ((EventMask & AWTEvent.WINDOW_EVENT_MASK) != 0 || WindowListener != null)
				{
					return true;
				}
				return false;
			  case WindowEvent.WINDOW_GAINED_FOCUS:
			  case WindowEvent.WINDOW_LOST_FOCUS:
				if ((EventMask & AWTEvent.WINDOW_FOCUS_EVENT_MASK) != 0 || WindowFocusListener != null)
				{
					return true;
				}
				return false;
			  case WindowEvent.WINDOW_STATE_CHANGED:
				if ((EventMask & AWTEvent.WINDOW_STATE_EVENT_MASK) != 0 || WindowStateListener != null)
				{
					return true;
				}
				return false;
			  default:
				break;
			}
			return base.EventEnabled(e);
		}

		/// <summary>
		/// Processes events on this window. If the event is an
		/// {@code WindowEvent}, it invokes the
		/// {@code processWindowEvent} method, else it invokes its
		/// superclass's {@code processEvent}.
		/// <para>Note that if the event parameter is {@code null}
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the event </param>
		protected internal override void ProcessEvent(AWTEvent e)
		{
			if (e is WindowEvent)
			{
				switch (e.ID)
				{
					case WindowEvent.WINDOW_OPENED:
					case WindowEvent.WINDOW_CLOSING:
					case WindowEvent.WINDOW_CLOSED:
					case WindowEvent.WINDOW_ICONIFIED:
					case WindowEvent.WINDOW_DEICONIFIED:
					case WindowEvent.WINDOW_ACTIVATED:
					case WindowEvent.WINDOW_DEACTIVATED:
						ProcessWindowEvent((WindowEvent)e);
						break;
					case WindowEvent.WINDOW_GAINED_FOCUS:
					case WindowEvent.WINDOW_LOST_FOCUS:
						ProcessWindowFocusEvent((WindowEvent)e);
						break;
					case WindowEvent.WINDOW_STATE_CHANGED:
						ProcessWindowStateEvent((WindowEvent)e);
						break;
				}
				return;
			}
			base.ProcessEvent(e);
		}

		/// <summary>
		/// Processes window events occurring on this window by
		/// dispatching them to any registered WindowListener objects.
		/// NOTE: This method will not be called unless window events
		/// are enabled for this component; this happens when one of the
		/// following occurs:
		/// <ul>
		/// <li>A WindowListener object is registered via
		///     {@code addWindowListener}
		/// <li>Window events are enabled via {@code enableEvents}
		/// </ul>
		/// <para>Note that if the event parameter is {@code null}
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the window event </param>
		/// <seealso cref= Component#enableEvents </seealso>
		protected internal virtual void ProcessWindowEvent(WindowEvent e)
		{
			WindowListener listener = WindowListener;
			if (listener != null)
			{
				switch (e.ID)
				{
					case WindowEvent.WINDOW_OPENED:
						listener.WindowOpened(e);
						break;
					case WindowEvent.WINDOW_CLOSING:
						listener.WindowClosing(e);
						break;
					case WindowEvent.WINDOW_CLOSED:
						listener.WindowClosed(e);
						break;
					case WindowEvent.WINDOW_ICONIFIED:
						listener.WindowIconified(e);
						break;
					case WindowEvent.WINDOW_DEICONIFIED:
						listener.WindowDeiconified(e);
						break;
					case WindowEvent.WINDOW_ACTIVATED:
						listener.WindowActivated(e);
						break;
					case WindowEvent.WINDOW_DEACTIVATED:
						listener.WindowDeactivated(e);
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// Processes window focus event occurring on this window by
		/// dispatching them to any registered WindowFocusListener objects.
		/// NOTE: this method will not be called unless window focus events
		/// are enabled for this window. This happens when one of the
		/// following occurs:
		/// <ul>
		/// <li>a WindowFocusListener is registered via
		///     {@code addWindowFocusListener}
		/// <li>Window focus events are enabled via {@code enableEvents}
		/// </ul>
		/// <para>Note that if the event parameter is {@code null}
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the window focus event </param>
		/// <seealso cref= Component#enableEvents
		/// @since 1.4 </seealso>
		protected internal virtual void ProcessWindowFocusEvent(WindowEvent e)
		{
			WindowFocusListener listener = WindowFocusListener;
			if (listener != null)
			{
				switch (e.ID)
				{
					case WindowEvent.WINDOW_GAINED_FOCUS:
						listener.WindowGainedFocus(e);
						break;
					case WindowEvent.WINDOW_LOST_FOCUS:
						listener.WindowLostFocus(e);
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// Processes window state event occurring on this window by
		/// dispatching them to any registered {@code WindowStateListener}
		/// objects.
		/// NOTE: this method will not be called unless window state events
		/// are enabled for this window.  This happens when one of the
		/// following occurs:
		/// <ul>
		/// <li>a {@code WindowStateListener} is registered via
		///    {@code addWindowStateListener}
		/// <li>window state events are enabled via {@code enableEvents}
		/// </ul>
		/// <para>Note that if the event parameter is {@code null}
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the window state event </param>
		/// <seealso cref= java.awt.Component#enableEvents
		/// @since 1.4 </seealso>
		protected internal virtual void ProcessWindowStateEvent(WindowEvent e)
		{
			WindowStateListener listener = WindowStateListener;
			if (listener != null)
			{
				switch (e.ID)
				{
					case WindowEvent.WINDOW_STATE_CHANGED:
						listener.WindowStateChanged(e);
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// Implements a debugging hook -- checks to see if
		/// the user has typed <i>control-shift-F1</i>.  If so,
		/// the list of child windows is dumped to {@code System.out}. </summary>
		/// <param name="e">  the keyboard event </param>
		internal override void PreProcessKeyEvent(KeyEvent e)
		{
			// Dump the list of child windows to System.out.
			if (e.ActionKey && e.KeyCode == KeyEvent.VK_F1 && e.ControlDown && e.ShiftDown && e.ID == KeyEvent.KEY_PRESSED)
			{
				List(System.out, 0);
			}
		}

		internal override void PostProcessKeyEvent(KeyEvent e)
		{
			// Do nothing
		}


		/// <summary>
		/// Sets whether this window should always be above other windows.  If
		/// there are multiple always-on-top windows, their relative order is
		/// unspecified and platform dependent.
		/// <para>
		/// If some other window is already always-on-top then the
		/// relative order between these windows is unspecified (depends on
		/// platform).  No window can be brought to be over the always-on-top
		/// window except maybe another always-on-top window.
		/// </para>
		/// <para>
		/// All windows owned by an always-on-top window inherit this state and
		/// automatically become always-on-top.  If a window ceases to be
		/// always-on-top, the windows that it owns will no longer be
		/// always-on-top.  When an always-on-top window is sent {@link #toBack
		/// toBack}, its always-on-top state is set to {@code false}.
		/// 
		/// </para>
		/// <para> When this method is called on a window with a value of
		/// {@code true}, and the window is visible and the platform
		/// supports always-on-top for this window, the window is immediately
		/// brought forward, "sticking" it in the top-most position. If the
		/// window isn`t currently visible, this method sets the always-on-top
		/// state to {@code true} but does not bring the window forward.
		/// When the window is later shown, it will be always-on-top.
		/// 
		/// </para>
		/// <para> When this method is called on a window with a value of
		/// {@code false} the always-on-top state is set to normal. It may also
		/// cause an unspecified, platform-dependent change in the z-order of
		/// top-level windows, but other always-on-top windows will remain in
		/// top-most position. Calling this method with a value of {@code false}
		/// on a window that has a normal state has no effect.
		/// 
		/// </para>
		/// <para><b>Note</b>: some platforms might not support always-on-top
		/// windows.  To detect if always-on-top windows are supported by the
		/// current platform, use <seealso cref="Toolkit#isAlwaysOnTopSupported()"/> and
		/// <seealso cref="Window#isAlwaysOnTopSupported()"/>.  If always-on-top mode
		/// isn't supported for this window or this window's toolkit does not
		/// support always-on-top windows, calling this method has no effect.
		/// </para>
		/// <para>
		/// If a SecurityManager is installed, the calling thread must be
		/// granted the AWTPermission "setWindowAlwaysOnTop" in
		/// order to set the value of this property. If this
		/// permission is not granted, this method will throw a
		/// SecurityException, and the current value of the property will
		/// be left unchanged.
		/// 
		/// </para>
		/// </summary>
		/// <param name="alwaysOnTop"> true if the window should always be above other
		///        windows </param>
		/// <exception cref="SecurityException"> if the calling thread does not have
		///         permission to set the value of always-on-top property
		/// </exception>
		/// <seealso cref= #isAlwaysOnTop </seealso>
		/// <seealso cref= #toFront </seealso>
		/// <seealso cref= #toBack </seealso>
		/// <seealso cref= AWTPermission </seealso>
		/// <seealso cref= #isAlwaysOnTopSupported </seealso>
		/// <seealso cref= #getToolkit </seealso>
		/// <seealso cref= Toolkit#isAlwaysOnTopSupported
		/// @since 1.5 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void setAlwaysOnTop(boolean alwaysOnTop) throws SecurityException
		public bool AlwaysOnTop
		{
			set
			{
				SecurityManager security = System.SecurityManager;
				if (security != null)
				{
					security.CheckPermission(SecurityConstants.AWT.SET_WINDOW_ALWAYS_ON_TOP_PERMISSION);
				}
    
				bool oldAlwaysOnTop;
				lock (this)
				{
					oldAlwaysOnTop = this.AlwaysOnTop_Renamed;
					this.AlwaysOnTop_Renamed = value;
				}
				if (oldAlwaysOnTop != value)
				{
					if (AlwaysOnTopSupported)
					{
						WindowPeer peer = (WindowPeer)this.Peer_Renamed;
						lock (TreeLock)
						{
							if (peer != null)
							{
								peer.UpdateAlwaysOnTopState();
							}
						}
					}
					FirePropertyChange("alwaysOnTop", oldAlwaysOnTop, value);
				}
				OwnedWindowsAlwaysOnTop = value;
			}
			get
			{
				return AlwaysOnTop_Renamed;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) private void setOwnedWindowsAlwaysOnTop(boolean alwaysOnTop)
		private bool OwnedWindowsAlwaysOnTop
		{
			set
			{
				WeakReference<Window>[] ownedWindowArray;
				lock (OwnedWindowList)
				{
					ownedWindowArray = new WeakReference[OwnedWindowList.Count];
					OwnedWindowList.CopyTo(ownedWindowArray);
				}
    
				foreach (WeakReference<Window> @ref in ownedWindowArray)
				{
					Window window = @ref.get();
					if (window != null)
					{
						try
						{
							window.AlwaysOnTop = value;
						}
						catch (SecurityException)
						{
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns whether the always-on-top mode is supported for this
		/// window. Some platforms may not support always-on-top windows, some
		/// may support only some kinds of top-level windows; for example,
		/// a platform may not support always-on-top modal dialogs.
		/// </summary>
		/// <returns> {@code true}, if the always-on-top mode is supported for
		///         this window and this window's toolkit supports always-on-top windows,
		///         {@code false} otherwise
		/// </returns>
		/// <seealso cref= #setAlwaysOnTop(boolean) </seealso>
		/// <seealso cref= #getToolkit </seealso>
		/// <seealso cref= Toolkit#isAlwaysOnTopSupported
		/// @since 1.6 </seealso>
		public virtual bool AlwaysOnTopSupported
		{
			get
			{
				return Toolkit.DefaultToolkit.AlwaysOnTopSupported;
			}
		}




		/// <summary>
		/// Returns the child Component of this Window that has focus if this Window
		/// is focused; returns null otherwise.
		/// </summary>
		/// <returns> the child Component with focus, or null if this Window is not
		///         focused </returns>
		/// <seealso cref= #getMostRecentFocusOwner </seealso>
		/// <seealso cref= #isFocused </seealso>
		public virtual Component FocusOwner
		{
			get
			{
				return (Focused) ? KeyboardFocusManager.CurrentKeyboardFocusManager.FocusOwner : null;
			}
		}

		/// <summary>
		/// Returns the child Component of this Window that will receive the focus
		/// when this Window is focused. If this Window is currently focused, this
		/// method returns the same Component as {@code getFocusOwner()}. If
		/// this Window is not focused, then the child Component that most recently
		/// requested focus will be returned. If no child Component has ever
		/// requested focus, and this is a focusable Window, then this Window's
		/// initial focusable Component is returned. If no child Component has ever
		/// requested focus, and this is a non-focusable Window, null is returned.
		/// </summary>
		/// <returns> the child Component that will receive focus when this Window is
		///         focused </returns>
		/// <seealso cref= #getFocusOwner </seealso>
		/// <seealso cref= #isFocused </seealso>
		/// <seealso cref= #isFocusableWindow
		/// @since 1.4 </seealso>
		public virtual Component MostRecentFocusOwner
		{
			get
			{
				if (Focused)
				{
					return FocusOwner;
				}
				else
				{
					Component mostRecent = KeyboardFocusManager.GetMostRecentFocusOwner(this);
					if (mostRecent != null)
					{
						return mostRecent;
					}
					else
					{
						return (FocusableWindow) ? FocusTraversalPolicy.GetInitialComponent(this) : null;
					}
				}
			}
		}

		/// <summary>
		/// Returns whether this Window is active. Only a Frame or a Dialog may be
		/// active. The native windowing system may denote the active Window or its
		/// children with special decorations, such as a highlighted title bar. The
		/// active Window is always either the focused Window, or the first Frame or
		/// Dialog that is an owner of the focused Window.
		/// </summary>
		/// <returns> whether this is the active Window. </returns>
		/// <seealso cref= #isFocused
		/// @since 1.4 </seealso>
		public virtual bool Active
		{
			get
			{
				return (KeyboardFocusManager.CurrentKeyboardFocusManager.ActiveWindow == this);
			}
		}

		/// <summary>
		/// Returns whether this Window is focused. If there exists a focus owner,
		/// the focused Window is the Window that is, or contains, that focus owner.
		/// If there is no focus owner, then no Window is focused.
		/// <para>
		/// If the focused Window is a Frame or a Dialog it is also the active
		/// Window. Otherwise, the active Window is the first Frame or Dialog that
		/// is an owner of the focused Window.
		/// 
		/// </para>
		/// </summary>
		/// <returns> whether this is the focused Window. </returns>
		/// <seealso cref= #isActive
		/// @since 1.4 </seealso>
		public virtual bool Focused
		{
			get
			{
				return (KeyboardFocusManager.CurrentKeyboardFocusManager.GlobalFocusedWindow == this);
			}
		}

		/// <summary>
		/// Gets a focus traversal key for this Window. (See {@code
		/// setFocusTraversalKeys} for a full description of each key.)
		/// <para>
		/// If the traversal key has not been explicitly set for this Window,
		/// then this Window's parent's traversal key is returned. If the
		/// traversal key has not been explicitly set for any of this Window's
		/// ancestors, then the current KeyboardFocusManager's default traversal key
		/// is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		///         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		///         KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS </param>
		/// <returns> the AWTKeyStroke for the specified key </returns>
		/// <seealso cref= Container#setFocusTraversalKeys </seealso>
		/// <seealso cref= KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#DOWN_CYCLE_TRAVERSAL_KEYS </seealso>
		/// <exception cref="IllegalArgumentException"> if id is not one of
		///         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		///         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		///         KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Set<AWTKeyStroke> getFocusTraversalKeys(int id)
		public override Set<AWTKeyStroke> GetFocusTraversalKeys(int id)
		{
			if (id < 0 || id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH)
			{
				throw new IllegalArgumentException("invalid focus traversal key identifier");
			}

			// Okay to return Set directly because it is an unmodifiable view
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") java.util.Set keystrokes = (focusTraversalKeys != null) ? focusTraversalKeys[id] : null;
			Set keystrokes = (FocusTraversalKeys != null) ? FocusTraversalKeys[id] : null;

			if (keystrokes != null)
			{
				return keystrokes;
			}
			else
			{
				return KeyboardFocusManager.CurrentKeyboardFocusManager.GetDefaultFocusTraversalKeys(id);
			}
		}

		/// <summary>
		/// Does nothing because Windows must always be roots of a focus traversal
		/// cycle. The passed-in value is ignored.
		/// </summary>
		/// <param name="focusCycleRoot"> this value is ignored </param>
		/// <seealso cref= #isFocusCycleRoot </seealso>
		/// <seealso cref= Container#setFocusTraversalPolicy </seealso>
		/// <seealso cref= Container#getFocusTraversalPolicy
		/// @since 1.4 </seealso>
		public sealed override bool FocusCycleRoot
		{
			set
			{
			}
			get
			{
				return true;
			}
		}


		/// <summary>
		/// Always returns {@code null} because Windows have no ancestors; they
		/// represent the top of the Component hierarchy.
		/// </summary>
		/// <returns> {@code null} </returns>
		/// <seealso cref= Container#isFocusCycleRoot()
		/// @since 1.4 </seealso>
		public sealed override Container FocusCycleRootAncestor
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns whether this Window can become the focused Window, that is,
		/// whether this Window or any of its subcomponents can become the focus
		/// owner. For a Frame or Dialog to be focusable, its focusable Window state
		/// must be set to {@code true}. For a Window which is not a Frame or
		/// Dialog to be focusable, its focusable Window state must be set to
		/// {@code true}, its nearest owning Frame or Dialog must be
		/// showing on the screen, and it must contain at least one Component in
		/// its focus traversal cycle. If any of these conditions is not met, then
		/// neither this Window nor any of its subcomponents can become the focus
		/// owner.
		/// </summary>
		/// <returns> {@code true} if this Window can be the focused Window;
		///         {@code false} otherwise </returns>
		/// <seealso cref= #getFocusableWindowState </seealso>
		/// <seealso cref= #setFocusableWindowState </seealso>
		/// <seealso cref= #isShowing </seealso>
		/// <seealso cref= Component#isFocusable
		/// @since 1.4 </seealso>
		public bool FocusableWindow
		{
			get
			{
				// If a Window/Frame/Dialog was made non-focusable, then it is always
				// non-focusable.
				if (!FocusableWindowState)
				{
					return false;
				}
    
				// All other tests apply only to Windows.
				if (this is Frame || this is Dialog)
				{
					return true;
				}
    
				// A Window must have at least one Component in its root focus
				// traversal cycle to be focusable.
				if (FocusTraversalPolicy.GetDefaultComponent(this) == null)
				{
					return false;
				}
    
				// A Window's nearest owning Frame or Dialog must be showing on the
				// screen.
				for (Window owner = Owner; owner != null; owner = owner.Owner)
				{
					if (owner is Frame || owner is Dialog)
					{
						return owner.Showing;
					}
				}
    
				return false;
			}
		}

		/// <summary>
		/// Returns whether this Window can become the focused Window if it meets
		/// the other requirements outlined in {@code isFocusableWindow}. If
		/// this method returns {@code false}, then
		/// {@code isFocusableWindow} will return {@code false} as well.
		/// If this method returns {@code true}, then
		/// {@code isFocusableWindow} may return {@code true} or
		/// {@code false} depending upon the other requirements which must be
		/// met in order for a Window to be focusable.
		/// <para>
		/// By default, all Windows have a focusable Window state of
		/// {@code true}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> whether this Window can be the focused Window </returns>
		/// <seealso cref= #isFocusableWindow </seealso>
		/// <seealso cref= #setFocusableWindowState </seealso>
		/// <seealso cref= #isShowing </seealso>
		/// <seealso cref= Component#setFocusable
		/// @since 1.4 </seealso>
		public virtual bool FocusableWindowState
		{
			get
			{
				return FocusableWindowState_Renamed;
			}
			set
			{
				bool oldFocusableWindowState;
				lock (this)
				{
					oldFocusableWindowState = this.FocusableWindowState_Renamed;
					this.FocusableWindowState_Renamed = value;
				}
				WindowPeer peer = (WindowPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.UpdateFocusableWindowState();
				}
				FirePropertyChange("focusableWindowState", oldFocusableWindowState, value);
				if (oldFocusableWindowState && !value && Focused)
				{
					for (Window owner = Owner; owner != null; owner = owner.Owner)
					{
							Component toFocus = KeyboardFocusManager.GetMostRecentFocusOwner(owner);
							if (toFocus != null && toFocus.RequestFocus(false, CausedFocusEvent.Cause.ACTIVATION))
							{
								return;
							}
					}
					KeyboardFocusManager.CurrentKeyboardFocusManager.ClearGlobalFocusOwnerPriv();
				}
			}
		}


		/// <summary>
		/// Sets whether this window should receive focus on
		/// subsequently being shown (with a call to <seealso cref="#setVisible setVisible(true)"/>),
		/// or being moved to the front (with a call to <seealso cref="#toFront"/>).
		/// <para>
		/// Note that <seealso cref="#setVisible setVisible(true)"/> may be called indirectly
		/// (e.g. when showing an owner of the window makes the window to be shown).
		/// <seealso cref="#toFront"/> may also be called indirectly (e.g. when
		/// <seealso cref="#setVisible setVisible(true)"/> is called on already visible window).
		/// In all such cases this property takes effect as well.
		/// </para>
		/// <para>
		/// The value of the property is not inherited by owned windows.
		/// 
		/// </para>
		/// </summary>
		/// <param name="autoRequestFocus"> whether this window should be focused on
		///        subsequently being shown or being moved to the front </param>
		/// <seealso cref= #isAutoRequestFocus </seealso>
		/// <seealso cref= #isFocusableWindow </seealso>
		/// <seealso cref= #setVisible </seealso>
		/// <seealso cref= #toFront
		/// @since 1.7 </seealso>
		public virtual bool AutoRequestFocus
		{
			set
			{
				this.AutoRequestFocus_Renamed = value;
			}
			get
			{
				return AutoRequestFocus_Renamed;
			}
		}


		/// <summary>
		/// Adds a PropertyChangeListener to the listener list. The listener is
		/// registered for all bound properties of this class, including the
		/// following:
		/// <ul>
		///    <li>this Window's font ("font")</li>
		///    <li>this Window's background color ("background")</li>
		///    <li>this Window's foreground color ("foreground")</li>
		///    <li>this Window's focusability ("focusable")</li>
		///    <li>this Window's focus traversal keys enabled state
		///        ("focusTraversalKeysEnabled")</li>
		///    <li>this Window's Set of FORWARD_TRAVERSAL_KEYS
		///        ("forwardFocusTraversalKeys")</li>
		///    <li>this Window's Set of BACKWARD_TRAVERSAL_KEYS
		///        ("backwardFocusTraversalKeys")</li>
		///    <li>this Window's Set of UP_CYCLE_TRAVERSAL_KEYS
		///        ("upCycleFocusTraversalKeys")</li>
		///    <li>this Window's Set of DOWN_CYCLE_TRAVERSAL_KEYS
		///        ("downCycleFocusTraversalKeys")</li>
		///    <li>this Window's focus traversal policy ("focusTraversalPolicy")
		///        </li>
		///    <li>this Window's focusable Window state ("focusableWindowState")
		///        </li>
		///    <li>this Window's always-on-top state("alwaysOnTop")</li>
		/// </ul>
		/// Note that if this Window is inheriting a bound property, then no
		/// event will be fired in response to a change in the inherited property.
		/// <para>
		/// If listener is null, no exception is thrown and no action is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">  the PropertyChangeListener to be added
		/// </param>
		/// <seealso cref= Component#removePropertyChangeListener </seealso>
		/// <seealso cref= #addPropertyChangeListener(java.lang.String,java.beans.PropertyChangeListener) </seealso>
		public override void AddPropertyChangeListener(PropertyChangeListener listener)
		{
			base.AddPropertyChangeListener(listener);
		}

		/// <summary>
		/// Adds a PropertyChangeListener to the listener list for a specific
		/// property. The specified property may be user-defined, or one of the
		/// following:
		/// <ul>
		///    <li>this Window's font ("font")</li>
		///    <li>this Window's background color ("background")</li>
		///    <li>this Window's foreground color ("foreground")</li>
		///    <li>this Window's focusability ("focusable")</li>
		///    <li>this Window's focus traversal keys enabled state
		///        ("focusTraversalKeysEnabled")</li>
		///    <li>this Window's Set of FORWARD_TRAVERSAL_KEYS
		///        ("forwardFocusTraversalKeys")</li>
		///    <li>this Window's Set of BACKWARD_TRAVERSAL_KEYS
		///        ("backwardFocusTraversalKeys")</li>
		///    <li>this Window's Set of UP_CYCLE_TRAVERSAL_KEYS
		///        ("upCycleFocusTraversalKeys")</li>
		///    <li>this Window's Set of DOWN_CYCLE_TRAVERSAL_KEYS
		///        ("downCycleFocusTraversalKeys")</li>
		///    <li>this Window's focus traversal policy ("focusTraversalPolicy")
		///        </li>
		///    <li>this Window's focusable Window state ("focusableWindowState")
		///        </li>
		///    <li>this Window's always-on-top state("alwaysOnTop")</li>
		/// </ul>
		/// Note that if this Window is inheriting a bound property, then no
		/// event will be fired in response to a change in the inherited property.
		/// <para>
		/// If listener is null, no exception is thrown and no action is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName"> one of the property names listed above </param>
		/// <param name="listener"> the PropertyChangeListener to be added
		/// </param>
		/// <seealso cref= #addPropertyChangeListener(java.beans.PropertyChangeListener) </seealso>
		/// <seealso cref= Component#removePropertyChangeListener </seealso>
		public override void AddPropertyChangeListener(String propertyName, PropertyChangeListener listener)
		{
			base.AddPropertyChangeListener(propertyName, listener);
		}

		/// <summary>
		/// Indicates if this container is a validate root.
		/// <para>
		/// {@code Window} objects are the validate roots, and, therefore, they
		/// override this method to return {@code true}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true}
		/// @since 1.7 </returns>
		/// <seealso cref= java.awt.Container#isValidateRoot </seealso>
		public override bool ValidateRoot
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Dispatches an event to this window or one of its sub components. </summary>
		/// <param name="e"> the event </param>
		internal override void DispatchEventImpl(AWTEvent e)
		{
			if (e.ID == ComponentEvent.COMPONENT_RESIZED)
			{
				Invalidate();
				Validate();
			}
			base.DispatchEventImpl(e);
		}

		/// @deprecated As of JDK version 1.1
		/// replaced by {@code dispatchEvent(AWTEvent)}. 
		[Obsolete("As of JDK version 1.1")]
		public override bool PostEvent(Event e)
		{
			if (HandleEvent(e))
			{
				e.Consume();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks if this Window is showing on screen. </summary>
		/// <seealso cref= Component#setVisible </seealso>
		public override bool Showing
		{
			get
			{
				return Visible_Renamed;
			}
		}

		internal virtual bool Disposing
		{
			get
			{
				return Disposing_Renamed;
			}
		}

		/// @deprecated As of J2SE 1.4, replaced by
		/// <seealso cref="Component#applyComponentOrientation Component.applyComponentOrientation"/>. 
		[Obsolete("As of J2SE 1.4, replaced by")]
		public virtual void ApplyResourceBundle(ResourceBundle rb)
		{
			ApplyComponentOrientation(ComponentOrientation.GetOrientation(rb));
		}

		/// @deprecated As of J2SE 1.4, replaced by
		/// <seealso cref="Component#applyComponentOrientation Component.applyComponentOrientation"/>. 
		[Obsolete("As of J2SE 1.4, replaced by")]
		public virtual void ApplyResourceBundle(String rbName)
		{
			ApplyResourceBundle(ResourceBundle.GetBundle(rbName));
		}

	   /*
	    * Support for tracking all windows owned by this window
	    */
		internal virtual void AddOwnedWindow(WeakReference<Window> weakWindow)
		{
			if (weakWindow != null)
			{
				lock (OwnedWindowList)
				{
					// this if statement should really be an assert, but we don't
					// have asserts...
					if (!OwnedWindowList.Contains(weakWindow))
					{
						OwnedWindowList.Add(weakWindow);
					}
				}
			}
		}

		internal virtual void RemoveOwnedWindow(WeakReference<Window> weakWindow)
		{
			if (weakWindow != null)
			{
				// synchronized block not required since removeElement is
				// already synchronized
				OwnedWindowList.Remove(weakWindow);
			}
		}

		internal virtual void ConnectOwnedWindow(Window child)
		{
			child.Parent_Renamed = this;
			AddOwnedWindow(child.WeakThis);
			child.DisposerRecord.UpdateOwner();
		}

		private void AddToWindowList()
		{
			lock (typeof(Window))
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Vector<WeakReference<Window>> windowList = (java.util.Vector<WeakReference<Window>>)appContext.get(Window.class);
				List<WeakReference<Window>> windowList = (List<WeakReference<Window>>)AppContext.get(typeof(Window));
				if (windowList == null)
				{
					windowList = new List<WeakReference<Window>>();
					AppContext.put(typeof(Window), windowList);
				}
				windowList.Add(WeakThis);
			}
		}

		private static void RemoveFromWindowList(AppContext context, WeakReference<Window> weakThis)
		{
			lock (typeof(Window))
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Vector<WeakReference<Window>> windowList = (java.util.Vector<WeakReference<Window>>)context.get(Window.class);
				List<WeakReference<Window>> windowList = (List<WeakReference<Window>>)context.get(typeof(Window));
				if (windowList != null)
				{
					windowList.Remove(weakThis);
				}
			}
		}

		private void RemoveFromWindowList()
		{
			RemoveFromWindowList(AppContext, WeakThis);
		}

		/// <summary>
		/// Window type.
		/// 
		/// Synchronization: ObjectLock
		/// </summary>
		private Type Type_Renamed = Type.NORMAL;

		/// <summary>
		/// Sets the type of the window.
		/// 
		/// This method can only be called while the window is not displayable.
		/// </summary>
		/// <exception cref="IllegalComponentStateException"> if the window
		///         is displayable. </exception>
		/// <exception cref="IllegalArgumentException"> if the type is {@code null} </exception>
		/// <seealso cref=    Component#isDisplayable </seealso>
		/// <seealso cref=    #getType
		/// @since 1.7 </seealso>
		public virtual Type Type
		{
			set
			{
				if (value == null)
				{
					throw new IllegalArgumentException("type should not be null.");
				}
				lock (TreeLock)
				{
					if (Displayable)
					{
						throw new IllegalComponentStateException("The window is displayable.");
					}
					lock (ObjectLock)
					{
						this.Type_Renamed = value;
					}
				}
			}
			get
			{
				lock (ObjectLock)
				{
					return Type_Renamed;
				}
			}
		}


		/// <summary>
		/// The window serialized data version.
		/// 
		/// @serial
		/// </summary>
		private int WindowSerializedDataVersion = 2;

		/// <summary>
		/// Writes default serializable fields to stream.  Writes
		/// a list of serializable {@code WindowListener}s and
		/// {@code WindowFocusListener}s as optional data.
		/// Writes a list of child windows as optional data.
		/// Writes a list of icon images as optional data
		/// </summary>
		/// <param name="s"> the {@code ObjectOutputStream} to write
		/// @serialData {@code null} terminated sequence of
		///    0 or more pairs; the pair consists of a {@code String}
		///    and {@code Object}; the {@code String}
		///    indicates the type of object and is one of the following:
		///    {@code windowListenerK} indicating a
		///      {@code WindowListener} object;
		///    {@code windowFocusWindowK} indicating a
		///      {@code WindowFocusListener} object;
		///    {@code ownedWindowK} indicating a child
		///      {@code Window} object
		/// </param>
		/// <seealso cref= AWTEventMulticaster#save(java.io.ObjectOutputStream, java.lang.String, java.util.EventListener) </seealso>
		/// <seealso cref= Component#windowListenerK </seealso>
		/// <seealso cref= Component#windowFocusListenerK </seealso>
		/// <seealso cref= Component#ownedWindowK </seealso>
		/// <seealso cref= #readObject(ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			lock (this)
			{
				// Update old focusMgr fields so that our object stream can be read
				// by previous releases
				FocusMgr = new FocusManager();
				FocusMgr.FocusRoot = this;
				FocusMgr.FocusOwner = MostRecentFocusOwner;

				s.DefaultWriteObject();

				// Clear fields so that we don't keep extra references around
				FocusMgr = null;

				AWTEventMulticaster.Save(s, WindowListenerK, WindowListener);
				AWTEventMulticaster.Save(s, WindowFocusListenerK, WindowFocusListener);
				AWTEventMulticaster.Save(s, WindowStateListenerK, WindowStateListener);
			}

			s.WriteObject(null);

			lock (OwnedWindowList)
			{
				for (int i = 0; i < OwnedWindowList.Count; i++)
				{
					Window child = OwnedWindowList[i].get();
					if (child != null)
					{
						s.WriteObject(OwnedWindowK);
						s.WriteObject(child);
					}
				}
			}
			s.WriteObject(null);

			//write icon array
			if (Icons != null)
			{
				foreach (Image i in Icons)
				{
					if (i is Serializable)
					{
						s.WriteObject(i);
					}
				}
			}
			s.WriteObject(null);
		}

		//
		// Part of deserialization procedure to be called before
		// user's code.
		//
		private void InitDeserializedWindow()
		{
			SetWarningString();
			InputContextLock = new Object();

			// Deserialized Windows are not yet visible.
			Visible_Renamed = false;

			WeakThis = new WeakReference<>(this);

			Anchor = new Object();
			DisposerRecord = new WindowDisposerRecord(AppContext, this);
			Disposer.addRecord(Anchor, DisposerRecord);

			AddToWindowList();
			InitGC(null);
			OwnedWindowList = new List<>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void deserializeResources(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void DeserializeResources(ObjectInputStream s)
		{

				if (WindowSerializedDataVersion < 2)
				{
					// Translate old-style focus tracking to new model. For 1.4 and
					// later releases, we'll rely on the Window's initial focusable
					// Component.
					if (FocusMgr != null)
					{
						if (FocusMgr.FocusOwner != null)
						{
							KeyboardFocusManager.SetMostRecentFocusOwner(this, FocusMgr.FocusOwner);
						}
					}

					// This field is non-transient and relies on default serialization.
					// However, the default value is insufficient, so we need to set
					// it explicitly for object data streams prior to 1.4.
					FocusableWindowState_Renamed = true;


				}

			Object keyOrNull;
			while (null != (keyOrNull = s.ReadObject()))
			{
				String key = ((String)keyOrNull).intern();

				if (WindowListenerK == key)
				{
					AddWindowListener((WindowListener)(s.ReadObject()));
				}
				else if (WindowFocusListenerK == key)
				{
					AddWindowFocusListener((WindowFocusListener)(s.ReadObject()));
				}
				else if (WindowStateListenerK == key)
				{
					AddWindowStateListener((WindowStateListener)(s.ReadObject()));
				} // skip value for unrecognized key
				else
				{
					s.ReadObject();
				}
			}

			try
			{
				while (null != (keyOrNull = s.ReadObject()))
				{
					String key = ((String)keyOrNull).intern();

					if (OwnedWindowK == key)
					{
						ConnectOwnedWindow((Window) s.ReadObject());
					}

					else // skip value for unrecognized key
					{
						s.ReadObject();
					}
				}

				//read icons
				Object obj = s.ReadObject(); //Throws OptionalDataException
											 //for pre1.6 objects.
				Icons = new List<Image>(); //Frame.readObject() assumes
												//pre1.6 version if icons is null.
				while (obj != null)
				{
					if (obj is Image)
					{
						Icons.Add((Image)obj);
					}
					obj = s.ReadObject();
				}
			}
			catch (OptionalDataException)
			{
				// 1.1 serialized form
				// ownedWindowList will be updated by Frame.readObject
			}

		}

		/// <summary>
		/// Reads the {@code ObjectInputStream} and an optional
		/// list of listeners to receive various events fired by
		/// the component; also reads a list of
		/// (possibly {@code null}) child windows.
		/// Unrecognized keys or values will be ignored.
		/// </summary>
		/// <param name="s"> the {@code ObjectInputStream} to read </param>
		/// <exception cref="HeadlessException"> if
		///   {@code GraphicsEnvironment.isHeadless} returns
		///   {@code true} </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= #writeObject </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
			 GraphicsEnvironment.CheckHeadless();
			 InitDeserializedWindow();
			 ObjectInputStream.GetField f = s.ReadFields();

			 SyncLWRequests = f.Get("syncLWRequests", SystemSyncLWRequests);
			 State = f.Get("state", 0);
			 FocusableWindowState_Renamed = f.Get("focusableWindowState", true);
			 WindowSerializedDataVersion = f.Get("windowSerializedDataVersion", 1);
			 LocationByPlatform_Renamed = f.Get("locationByPlatform", LocationByPlatformProp);
			 // Note: 1.4 (or later) doesn't use focusMgr
			 FocusMgr = (FocusManager)f.Get("focusMgr", null);
			 Dialog.ModalExclusionType et = (Dialog.ModalExclusionType) f.Get("modalExclusionType", Dialog.ModalExclusionType.NO_EXCLUDE);
			 ModalExclusionType = et; // since 6.0
			 bool aot = f.Get("alwaysOnTop", false);
			 if (aot)
			 {
				 AlwaysOnTop = aot; // since 1.5; subject to permission check
			 }
			 Shape_Renamed = (Shape)f.Get("shape", null);
			 Opacity_Renamed = (Float)f.Get("opacity", 1.0f);

			 this.SecurityWarningWidth = 0;
			 this.SecurityWarningHeight = 0;
			 this.SecurityWarningPointX = 2.0;
			 this.SecurityWarningPointY = 0.0;
			 this.SecurityWarningAlignmentX = RIGHT_ALIGNMENT;
			 this.SecurityWarningAlignmentY = TOP_ALIGNMENT;

			 DeserializeResources(s);
		}

		/*
		 * --- Accessibility Support ---
		 *
		 */

		/// <summary>
		/// Gets the AccessibleContext associated with this Window.
		/// For windows, the AccessibleContext takes the form of an
		/// AccessibleAWTWindow.
		/// A new AccessibleAWTWindow instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTWindow that serves as the
		///         AccessibleContext of this Window
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTWindow(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// {@code Window} class.  It provides an implementation of the
		/// Java Accessibility API appropriate to window user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTWindow : AccessibleAWTContainer
		{
			private readonly Window OuterInstance;

			public AccessibleAWTWindow(Window outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = 4215068635060671780L;

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object </returns>
			/// <seealso cref= javax.accessibility.AccessibleRole </seealso>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.WINDOW;
				}
			}

			/// <summary>
			/// Get the state of this object.
			/// </summary>
			/// <returns> an instance of AccessibleStateSet containing the current
			/// state set of the object </returns>
			/// <seealso cref= javax.accessibility.AccessibleState </seealso>
			public override AccessibleStateSet AccessibleStateSet
			{
				get
				{
					AccessibleStateSet states = base.AccessibleStateSet;
					if (outerInstance.FocusOwner != null)
					{
						states.add(AccessibleState.ACTIVE);
					}
					return states;
				}
			}

		} // inner class AccessibleAWTWindow

		internal override GraphicsConfiguration GraphicsConfiguration
		{
			set
			{
				if (value == null)
				{
					value = GraphicsEnvironment.LocalGraphicsEnvironment.DefaultScreenDevice.DefaultConfiguration;
				}
				lock (TreeLock)
				{
					base.GraphicsConfiguration = value;
					if (Log.isLoggable(PlatformLogger.Level.FINER))
					{
						Log.finer("+ Window.setGraphicsConfiguration(): new GC is \n+ " + GraphicsConfiguration_NoClientCode + "\n+ this is " + this);
					}
				}
			}
		}

		/// <summary>
		/// Sets the location of the window relative to the specified
		/// component according to the following scenarios.
		/// <para>
		/// The target screen mentioned below is a screen to which
		/// the window should be placed after the setLocationRelativeTo
		/// method is called.
		/// <ul>
		/// <li>If the component is {@code null}, or the {@code
		/// GraphicsConfiguration} associated with this component is
		/// {@code null}, the window is placed in the center of the
		/// screen. The center point can be obtained with the {@link
		/// GraphicsEnvironment#getCenterPoint
		/// GraphicsEnvironment.getCenterPoint} method.
		/// <li>If the component is not {@code null}, but it is not
		/// currently showing, the window is placed in the center of
		/// the target screen defined by the {@code
		/// GraphicsConfiguration} associated with this component.
		/// <li>If the component is not {@code null} and is shown on
		/// the screen, then the window is located in such a way that
		/// the center of the window coincides with the center of the
		/// component.
		/// </ul>
		/// </para>
		/// <para>
		/// If the screens configuration does not allow the window to
		/// be moved from one screen to another, then the window is
		/// only placed at the location determined according to the
		/// above conditions and its {@code GraphicsConfiguration} is
		/// not changed.
		/// </para>
		/// <para>
		/// <b>Note</b>: If the lower edge of the window is out of the screen,
		/// then the window is placed to the side of the {@code Component}
		/// that is closest to the center of the screen. So if the
		/// component is on the right part of the screen, the window
		/// is placed to its left, and vice versa.
		/// </para>
		/// <para>
		/// If after the window location has been calculated, the upper,
		/// left, or right edge of the window is out of the screen,
		/// then the window is located in such a way that the upper,
		/// left, or right edge of the window coincides with the
		/// corresponding edge of the screen. If both left and right
		/// edges of the window are out of the screen, the window is
		/// placed at the left side of the screen. The similar placement
		/// will occur if both top and bottom edges are out of the screen.
		/// In that case, the window is placed at the top side of the screen.
		/// </para>
		/// <para>
		/// The method changes the geometry-related data. Therefore,
		/// the native windowing system may ignore such requests, or it may modify
		/// the requested data, so that the {@code Window} object is placed and sized
		/// in a way that corresponds closely to the desktop settings.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c">  the component in relation to which the window's location
		///           is determined </param>
		/// <seealso cref= java.awt.GraphicsEnvironment#getCenterPoint
		/// @since 1.4 </seealso>
		public virtual Component LocationRelativeTo
		{
			set
			{
				// target location
				int dx = 0, dy = 0;
				// target GC
				GraphicsConfiguration gc = GraphicsConfiguration_NoClientCode;
				Rectangle gcBounds = gc.Bounds;
    
				Dimension windowSize = Size;
    
				// search a top-level of value
				Window componentWindow = SunToolkit.getContainingWindow(value);
				if ((value == null) || (componentWindow == null))
				{
					GraphicsEnvironment ge = GraphicsEnvironment.LocalGraphicsEnvironment;
					gc = ge.DefaultScreenDevice.DefaultConfiguration;
					gcBounds = gc.Bounds;
					Point centerPoint = ge.CenterPoint;
					dx = centerPoint.x - windowSize.Width_Renamed / 2;
					dy = centerPoint.y - windowSize.Height_Renamed / 2;
				}
				else if (!value.Showing)
				{
					gc = componentWindow.GraphicsConfiguration;
					gcBounds = gc.Bounds;
					dx = gcBounds.x + (gcBounds.Width_Renamed - windowSize.Width_Renamed) / 2;
					dy = gcBounds.y + (gcBounds.Height_Renamed - windowSize.Height_Renamed) / 2;
				}
				else
				{
					gc = componentWindow.GraphicsConfiguration;
					gcBounds = gc.Bounds;
					Dimension compSize = value.Size;
					Point compLocation = value.LocationOnScreen;
					dx = compLocation.x + ((compSize.Width_Renamed - windowSize.Width_Renamed) / 2);
					dy = compLocation.y + ((compSize.Height_Renamed - windowSize.Height_Renamed) / 2);
    
					// Adjust for bottom edge being offscreen
					if (dy + windowSize.Height_Renamed > gcBounds.y + gcBounds.Height_Renamed)
					{
						dy = gcBounds.y + gcBounds.Height_Renamed - windowSize.Height_Renamed;
						if (compLocation.x - gcBounds.x + compSize.Width_Renamed / 2 < gcBounds.Width_Renamed / 2)
						{
							dx = compLocation.x + compSize.Width_Renamed;
						}
						else
						{
							dx = compLocation.x - windowSize.Width_Renamed;
						}
					}
				}
    
				// Avoid being placed off the edge of the screen:
				// bottom
				if (dy + windowSize.Height_Renamed > gcBounds.y + gcBounds.Height_Renamed)
				{
					dy = gcBounds.y + gcBounds.Height_Renamed - windowSize.Height_Renamed;
				}
				// top
				if (dy < gcBounds.y)
				{
					dy = gcBounds.y;
				}
				// right
				if (dx + windowSize.Width_Renamed > gcBounds.x + gcBounds.Width_Renamed)
				{
					dx = gcBounds.x + gcBounds.Width_Renamed - windowSize.Width_Renamed;
				}
				// left
				if (dx < gcBounds.x)
				{
					dx = gcBounds.x;
				}
    
				SetLocation(dx, dy);
			}
		}

		/// <summary>
		/// Overridden from Component.  Top-level Windows should not propagate a
		/// MouseWheelEvent beyond themselves into their owning Windows.
		/// </summary>
		internal virtual void DeliverMouseWheelToAncestor(MouseWheelEvent e)
		{
		}

		/// <summary>
		/// Overridden from Component.  Top-level Windows don't dispatch to ancestors
		/// </summary>
		internal override bool DispatchMouseWheelToAncestor(MouseWheelEvent e)
		{
			return false;
		}

		/// <summary>
		/// Creates a new strategy for multi-buffering on this component.
		/// Multi-buffering is useful for rendering performance.  This method
		/// attempts to create the best strategy available with the number of
		/// buffers supplied.  It will always create a {@code BufferStrategy}
		/// with that number of buffers.
		/// A page-flipping strategy is attempted first, then a blitting strategy
		/// using accelerated buffers.  Finally, an unaccelerated blitting
		/// strategy is used.
		/// <para>
		/// Each time this method is called,
		/// the existing buffer strategy for this component is discarded.
		/// </para>
		/// </summary>
		/// <param name="numBuffers"> number of buffers to create </param>
		/// <exception cref="IllegalArgumentException"> if numBuffers is less than 1. </exception>
		/// <exception cref="IllegalStateException"> if the component is not displayable </exception>
		/// <seealso cref= #isDisplayable </seealso>
		/// <seealso cref= #getBufferStrategy
		/// @since 1.4 </seealso>
		public override void CreateBufferStrategy(int numBuffers)
		{
			base.CreateBufferStrategy(numBuffers);
		}

		/// <summary>
		/// Creates a new strategy for multi-buffering on this component with the
		/// required buffer capabilities.  This is useful, for example, if only
		/// accelerated memory or page flipping is desired (as specified by the
		/// buffer capabilities).
		/// <para>
		/// Each time this method
		/// is called, the existing buffer strategy for this component is discarded.
		/// </para>
		/// </summary>
		/// <param name="numBuffers"> number of buffers to create, including the front buffer </param>
		/// <param name="caps"> the required capabilities for creating the buffer strategy;
		/// cannot be {@code null} </param>
		/// <exception cref="AWTException"> if the capabilities supplied could not be
		/// supported or met; this may happen, for example, if there is not enough
		/// accelerated memory currently available, or if page flipping is specified
		/// but not possible. </exception>
		/// <exception cref="IllegalArgumentException"> if numBuffers is less than 1, or if
		/// caps is {@code null} </exception>
		/// <seealso cref= #getBufferStrategy
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void createBufferStrategy(int numBuffers, BufferCapabilities caps) throws AWTException
		public override void CreateBufferStrategy(int numBuffers, BufferCapabilities caps)
		{
			base.CreateBufferStrategy(numBuffers, caps);
		}

		/// <summary>
		/// Returns the {@code BufferStrategy} used by this component.  This
		/// method will return null if a {@code BufferStrategy} has not yet
		/// been created or has been disposed.
		/// </summary>
		/// <returns> the buffer strategy used by this component </returns>
		/// <seealso cref= #createBufferStrategy
		/// @since 1.4 </seealso>
		public override BufferStrategy BufferStrategy
		{
			get
			{
				return base.BufferStrategy;
			}
		}

		internal virtual Component TemporaryLostComponent
		{
			get
			{
				return TemporaryLostComponent_Renamed;
			}
		}
		internal virtual Component SetTemporaryLostComponent(Component component)
		{
			Component previousComp = TemporaryLostComponent_Renamed;
			// Check that "component" is an acceptable focus owner and don't store it otherwise
			// - or later we will have problems with opposite while handling  WINDOW_GAINED_FOCUS
			if (component == null || component.CanBeFocusOwner())
			{
				TemporaryLostComponent_Renamed = component;
			}
			else
			{
				TemporaryLostComponent_Renamed = null;
			}
			return previousComp;
		}

		/// <summary>
		/// Checks whether this window can contain focus owner.
		/// Verifies that it is focusable and as container it can container focus owner.
		/// @since 1.5
		/// </summary>
		internal override bool CanContainFocusOwner(Component focusOwnerCandidate)
		{
			return base.CanContainFocusOwner(focusOwnerCandidate) && FocusableWindow;
		}

		private bool LocationByPlatform_Renamed = LocationByPlatformProp;


		/// <summary>
		/// Sets whether this Window should appear at the default location for the
		/// native windowing system or at the current location (returned by
		/// {@code getLocation}) the next time the Window is made visible.
		/// This behavior resembles a native window shown without programmatically
		/// setting its location.  Most windowing systems cascade windows if their
		/// locations are not explicitly set. The actual location is determined once the
		/// window is shown on the screen.
		/// <para>
		/// This behavior can also be enabled by setting the System Property
		/// "java.awt.Window.locationByPlatform" to "true", though calls to this method
		/// take precedence.
		/// </para>
		/// <para>
		/// Calls to {@code setVisible}, {@code setLocation} and
		/// {@code setBounds} after calling {@code setLocationByPlatform} clear
		/// this property of the Window.
		/// </para>
		/// <para>
		/// For example, after the following code is executed:
		/// <pre>
		/// setLocationByPlatform(true);
		/// setVisible(true);
		/// boolean flag = isLocationByPlatform();
		/// </pre>
		/// The window will be shown at platform's default location and
		/// {@code flag} will be {@code false}.
		/// </para>
		/// <para>
		/// In the following sample:
		/// <pre>
		/// setLocationByPlatform(true);
		/// setLocation(10, 10);
		/// boolean flag = isLocationByPlatform();
		/// setVisible(true);
		/// </pre>
		/// The window will be shown at (10, 10) and {@code flag} will be
		/// {@code false}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="locationByPlatform"> {@code true} if this Window should appear
		///        at the default location, {@code false} if at the current location </param>
		/// <exception cref="IllegalComponentStateException"> if the window
		///         is showing on screen and locationByPlatform is {@code true}. </exception>
		/// <seealso cref= #setLocation </seealso>
		/// <seealso cref= #isShowing </seealso>
		/// <seealso cref= #setVisible </seealso>
		/// <seealso cref= #isLocationByPlatform </seealso>
		/// <seealso cref= java.lang.System#getProperty(String)
		/// @since 1.5 </seealso>
		public virtual bool LocationByPlatform
		{
			set
			{
				lock (TreeLock)
				{
					if (value && Showing)
					{
						throw new IllegalComponentStateException("The window is showing on screen.");
					}
					this.LocationByPlatform_Renamed = value;
				}
			}
			get
			{
				lock (TreeLock)
				{
					return LocationByPlatform_Renamed;
				}
			}
		}


		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// The {@code width} or {@code height} values
		/// are automatically enlarged if either is less than
		/// the minimum size as specified by previous call to
		/// {@code setMinimumSize}.
		/// </para>
		/// <para>
		/// The method changes the geometry-related data. Therefore,
		/// the native windowing system may ignore such requests, or it may modify
		/// the requested data, so that the {@code Window} object is placed and sized
		/// in a way that corresponds closely to the desktop settings.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #getBounds </seealso>
		/// <seealso cref= #setLocation(int, int) </seealso>
		/// <seealso cref= #setLocation(Point) </seealso>
		/// <seealso cref= #setSize(int, int) </seealso>
		/// <seealso cref= #setSize(Dimension) </seealso>
		/// <seealso cref= #setMinimumSize </seealso>
		/// <seealso cref= #setLocationByPlatform </seealso>
		/// <seealso cref= #isLocationByPlatform
		/// @since 1.6 </seealso>
		public override void SetBounds(int x, int y, int width, int height)
		{
			lock (TreeLock)
			{
				if (BoundsOp == java.awt.peer.ComponentPeer_Fields.SET_LOCATION || BoundsOp == java.awt.peer.ComponentPeer_Fields.SET_BOUNDS)
				{
					LocationByPlatform_Renamed = false;
				}
				base.SetBounds(x, y, width, height);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <para>
		/// The {@code r.width} or {@code r.height} values
		/// will be automatically enlarged if either is less than
		/// the minimum size as specified by previous call to
		/// {@code setMinimumSize}.
		/// </para>
		/// <para>
		/// The method changes the geometry-related data. Therefore,
		/// the native windowing system may ignore such requests, or it may modify
		/// the requested data, so that the {@code Window} object is placed and sized
		/// in a way that corresponds closely to the desktop settings.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #getBounds </seealso>
		/// <seealso cref= #setLocation(int, int) </seealso>
		/// <seealso cref= #setLocation(Point) </seealso>
		/// <seealso cref= #setSize(int, int) </seealso>
		/// <seealso cref= #setSize(Dimension) </seealso>
		/// <seealso cref= #setMinimumSize </seealso>
		/// <seealso cref= #setLocationByPlatform </seealso>
		/// <seealso cref= #isLocationByPlatform
		/// @since 1.6 </seealso>
		public override Rectangle Bounds
		{
			set
			{
				SetBounds(value.x, value.y, value.Width_Renamed, value.Height_Renamed);
			}
		}

		/// <summary>
		/// Determines whether this component will be displayed on the screen. </summary>
		/// <returns> {@code true} if the component and all of its ancestors
		///          until a toplevel window are visible, {@code false} otherwise </returns>
		internal override bool RecursivelyVisible
		{
			get
			{
				// 5079694 fix: for a toplevel to be displayed, its parent doesn't have to be visible.
				// We're overriding isRecursivelyVisible to implement this policy.
				return Visible_Renamed;
			}
		}


		// ******************** SHAPES & TRANSPARENCY CODE ********************

		/// <summary>
		/// Returns the opacity of the window.
		/// </summary>
		/// <returns> the opacity of the window
		/// </returns>
		/// <seealso cref= Window#setOpacity(float) </seealso>
		/// <seealso cref= GraphicsDevice.WindowTranslucency
		/// 
		/// @since 1.7 </seealso>
		public virtual float Opacity
		{
			get
			{
				lock (TreeLock)
				{
					return Opacity_Renamed;
				}
			}
			set
			{
				lock (TreeLock)
				{
					if (value < 0.0f || value > 1.0f)
					{
						throw new IllegalArgumentException("The value of opacity should be in the range [0.0f .. 1.0f].");
					}
					if (value < 1.0f)
					{
						GraphicsConfiguration gc = GraphicsConfiguration;
						GraphicsDevice gd = gc.Device;
						if (gc.Device.FullScreenWindow == this)
						{
							throw new IllegalComponentStateException("Setting opacity for full-screen window is not supported.");
						}
						if (!gd.IsWindowTranslucencySupported(GraphicsDevice.WindowTranslucency.TRANSLUCENT))
						{
							throw new UnsupportedOperationException("TRANSLUCENT translucency is not supported.");
						}
					}
					this.Opacity_Renamed = value;
					WindowPeer peer = (WindowPeer)Peer;
					if (peer != null)
					{
						peer.Opacity = value;
					}
				}
			}
		}


		/// <summary>
		/// Returns the shape of the window.
		/// 
		/// The value returned by this method may not be the same as
		/// previously set with {@code setShape(shape)}, but it is guaranteed
		/// to represent the same shape.
		/// </summary>
		/// <returns> the shape of the window or {@code null} if no
		///     shape is specified for the window
		/// </returns>
		/// <seealso cref= Window#setShape(Shape) </seealso>
		/// <seealso cref= GraphicsDevice.WindowTranslucency
		/// 
		/// @since 1.7 </seealso>
		public virtual Shape Shape
		{
			get
			{
				lock (TreeLock)
				{
					return Shape_Renamed == null ? null : new Path2D.Float(Shape_Renamed);
				}
			}
			set
			{
				lock (TreeLock)
				{
					if (value != null)
					{
						GraphicsConfiguration gc = GraphicsConfiguration;
						GraphicsDevice gd = gc.Device;
						if (gc.Device.FullScreenWindow == this)
						{
							throw new IllegalComponentStateException("Setting shape for full-screen window is not supported.");
						}
						if (!gd.IsWindowTranslucencySupported(GraphicsDevice.WindowTranslucency.PERPIXEL_TRANSPARENT))
						{
							throw new UnsupportedOperationException("PERPIXEL_TRANSPARENT translucency is not supported.");
						}
					}
					this.Shape_Renamed = (value == null) ? null : new Path2D.Float(value);
					WindowPeer peer = (WindowPeer)Peer;
					if (peer != null)
					{
						peer.ApplyShape(value == null ? null : Region.getInstance(value, null));
					}
				}
			}
		}


		/// <summary>
		/// Gets the background color of this window.
		/// <para>
		/// Note that the alpha component of the returned color indicates whether
		/// the window is in the non-opaque (per-pixel translucent) mode.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this component's background color
		/// </returns>
		/// <seealso cref= Window#setBackground(Color) </seealso>
		/// <seealso cref= Window#isOpaque </seealso>
		/// <seealso cref= GraphicsDevice.WindowTranslucency </seealso>
		public override Color Background
		{
			get
			{
				return base.Background;
			}
			set
			{
				Color oldBg = Background;
				base.Background = value;
				if (oldBg != null && oldBg.Equals(value))
				{
					return;
				}
				int oldAlpha = oldBg != null ? oldBg.Alpha : 255;
				int alpha = value != null ? value.Alpha : 255;
				if ((oldAlpha == 255) && (alpha < 255)) // non-opaque window
				{
					GraphicsConfiguration gc = GraphicsConfiguration;
					GraphicsDevice gd = gc.Device;
					if (gc.Device.FullScreenWindow == this)
					{
						throw new IllegalComponentStateException("Making full-screen window non opaque is not supported.");
					}
					if (!gc.TranslucencyCapable)
					{
						GraphicsConfiguration capableGC = gd.TranslucencyCapableGC;
						if (capableGC == null)
						{
							throw new UnsupportedOperationException("PERPIXEL_TRANSLUCENT translucency is not supported");
						}
						GraphicsConfiguration = capableGC;
					}
					SetLayersOpaque(this, false);
				}
				else if ((oldAlpha < 255) && (alpha == 255))
				{
					SetLayersOpaque(this, true);
				}
				WindowPeer peer = (WindowPeer)Peer;
				if (peer != null)
				{
					peer.Opaque = alpha == 255;
				}
			}
		}


		/// <summary>
		/// Indicates if the window is currently opaque.
		/// <para>
		/// The method returns {@code false} if the background color of the window
		/// is not {@code null} and the alpha component of the color is less than
		/// {@code 1.0f}. The method returns {@code true} otherwise.
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if the window is opaque, {@code false} otherwise
		/// </returns>
		/// <seealso cref= Window#getBackground </seealso>
		/// <seealso cref= Window#setBackground(Color)
		/// @since 1.7 </seealso>
		public override bool Opaque
		{
			get
			{
				Color bg = Background;
				return bg != null ? bg.Alpha == 255 : true;
			}
		}

		private void UpdateWindow()
		{
			lock (TreeLock)
			{
				WindowPeer peer = (WindowPeer)Peer;
				if (peer != null)
				{
					peer.UpdateWindow();
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @since 1.7
		/// </summary>
		public override void Paint(Graphics g)
		{
			if (!Opaque)
			{
				Graphics gg = g.Create();
				try
				{
					if (gg is Graphics2D)
					{
						gg.Color = Background;
						((Graphics2D)gg).Composite = AlphaComposite.GetInstance(AlphaComposite.SRC);
						gg.FillRect(0, 0, Width, Height);
					}
				}
				finally
				{
					gg.Dispose();
				}
			}
			base.Paint(g);
		}

		private static void SetLayersOpaque(Component component, bool isOpaque)
		{
			// Shouldn't use instanceof to avoid loading Swing classes
			//    if it's a pure AWT application.
			if (SunToolkit.isInstanceOf(component, "javax.swing.RootPaneContainer"))
			{
				javax.swing.RootPaneContainer rpc = (javax.swing.RootPaneContainer)component;
				javax.swing.JRootPane root = rpc.RootPane;
				javax.swing.JLayeredPane lp = root.LayeredPane;
				Container c = root.ContentPane;
				javax.swing.JComponent content = (c is javax.swing.JComponent) ? (javax.swing.JComponent)c : null;
				lp.Opaque = isOpaque;
				root.Opaque = isOpaque;
				if (content != null)
				{
					content.Opaque = isOpaque;

					// Iterate down one level to see whether we have a JApplet
					// (which is also a RootPaneContainer) which requires processing
					int numChildren = content.ComponentCount;
					if (numChildren > 0)
					{
						Component child = content.getComponent(0);
						// It's OK to use instanceof here because we've
						// already loaded the RootPaneContainer class by now
						if (child is javax.swing.RootPaneContainer)
						{
							SetLayersOpaque(child, isOpaque);
						}
					}
				}
			}
		}


		// ************************** MIXING CODE *******************************

		// A window has an owner, but it does NOT have a container
		internal override sealed Container Container
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Applies the shape to the component </summary>
		/// <param name="shape"> Shape to be applied to the component </param>
		internal override sealed void ApplyCompoundShape(Region shape)
		{
			// The shape calculated by mixing code is not intended to be applied
			// to windows or frames
		}

		internal override sealed void ApplyCurrentShape()
		{
			// The shape calculated by mixing code is not intended to be applied
			// to windows or frames
		}

		internal override sealed void MixOnReshaping()
		{
			// The shape calculated by mixing code is not intended to be applied
			// to windows or frames
		}

		internal override sealed Point LocationOnWindow
		{
			get
			{
				return new Point(0, 0);
			}
		}

		// ****************** END OF MIXING CODE ********************************

		/// <summary>
		/// Limit the given double value with the given range.
		/// </summary>
		private static double Limit(double value, double min, double max)
		{
			value = System.Math.Max(value, min);
			value = System.Math.Min(value, max);
			return value;
		}

		/// <summary>
		/// Calculate the position of the security warning.
		/// 
		/// This method gets the window location/size as reported by the native
		/// system since the locally cached values may represent outdated data.
		/// 
		/// The method is used from the native code, or via AWTAccessor.
		/// 
		/// NOTE: this method is invoked on the toolkit thread, and therefore is not
		/// supposed to become public/user-overridable.
		/// </summary>
		private Point2D CalculateSecurityWarningPosition(double x, double y, double w, double h)
		{
			// The position according to the spec of SecurityWarning.setPosition()
			double wx = x + w * SecurityWarningAlignmentX + SecurityWarningPointX;
			double wy = y + h * SecurityWarningAlignmentY + SecurityWarningPointY;

			// First, make sure the warning is not too far from the window bounds
			wx = Window.Limit(wx, x - SecurityWarningWidth - 2, x + w + 2);
			wy = Window.Limit(wy, y - SecurityWarningHeight - 2, y + h + 2);

			// Now make sure the warning window is visible on the screen
			GraphicsConfiguration graphicsConfig = GraphicsConfiguration_NoClientCode;
			Rectangle screenBounds = graphicsConfig.Bounds;
			Insets screenInsets = Toolkit.DefaultToolkit.GetScreenInsets(graphicsConfig);

			wx = Window.Limit(wx, screenBounds.x + screenInsets.Left, screenBounds.x + screenBounds.Width_Renamed - screenInsets.Right - SecurityWarningWidth);
			wy = Window.Limit(wy, screenBounds.y + screenInsets.Top, screenBounds.y + screenBounds.Height_Renamed - screenInsets.Bottom - SecurityWarningHeight);

			return new Point2D.Double(wx, wy);
		}


		// a window doesn't need to be updated in the Z-order.
		internal override void UpdateZOrder()
		{
		}

	} // class Window


	/// <summary>
	/// This class is no longer used, but is maintained for Serialization
	/// backward-compatibility.
	/// </summary>
	[Serializable]
	internal class FocusManager
	{
		internal Container FocusRoot;
		internal Component FocusOwner;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		internal const long SerialVersionUID = 2491878825643557906L;
	}

}