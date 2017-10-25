using System;
using System.Diagnostics;
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


	using GetPropertyAction = sun.security.action.GetPropertyAction;
	using AppContext = sun.awt.AppContext;
	using AWTAccessor = sun.awt.AWTAccessor;
	using ConstrainableGraphics = sun.awt.ConstrainableGraphics;
	using SubRegionShowable = sun.awt.SubRegionShowable;
	using SunToolkit = sun.awt.SunToolkit;
	using WindowClosingListener = sun.awt.WindowClosingListener;
	using CausedFocusEvent = sun.awt.CausedFocusEvent;
	using EmbeddedFrame = sun.awt.EmbeddedFrame;
	using SunDropTargetEvent = sun.awt.dnd.SunDropTargetEvent;
	using CompositionArea = sun.awt.im.CompositionArea;
	using FontManager = sun.font.FontManager;
	using FontManagerFactory = sun.font.FontManagerFactory;
	using SunFontManager = sun.font.SunFontManager;
	using SunGraphics2D = sun.java2d.SunGraphics2D;
	using Region = sun.java2d.pipe.Region;
	using VSyncedBSManager = sun.awt.image.VSyncedBSManager;
	using ExtendedBufferCapabilities = sun.java2d.pipe.hw.ExtendedBufferCapabilities;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.java2d.pipe.hw.ExtendedBufferCapabilities.VSyncType.*;
	using RequestFocusController = sun.awt.RequestFocusController;
	using SunGraphicsEnvironment = sun.java2d.SunGraphicsEnvironment;
	using PlatformLogger = sun.util.logging.PlatformLogger;

	/// <summary>
	/// A <em>component</em> is an object having a graphical representation
	/// that can be displayed on the screen and that can interact with the
	/// user. Examples of components are the buttons, checkboxes, and scrollbars
	/// of a typical graphical user interface. <para>
	/// The <code>Component</code> class is the abstract superclass of
	/// the nonmenu-related Abstract Window Toolkit components. Class
	/// <code>Component</code> can also be extended directly to create a
	/// lightweight component. A lightweight component is a component that is
	/// not associated with a native window. On the contrary, a heavyweight
	/// component is associated with a native window. The <seealso cref="#isLightweight()"/>
	/// method may be used to distinguish between the two kinds of the components.
	/// </para>
	/// <para>
	/// Lightweight and heavyweight components may be mixed in a single component
	/// hierarchy. However, for correct operating of such a mixed hierarchy of
	/// components, the whole hierarchy must be valid. When the hierarchy gets
	/// invalidated, like after changing the bounds of components, or
	/// adding/removing components to/from containers, the whole hierarchy must be
	/// validated afterwards by means of the <seealso cref="Container#validate()"/> method
	/// invoked on the top-most invalid container of the hierarchy.
	/// 
	/// <h3>Serialization</h3>
	/// It is important to note that only AWT listeners which conform
	/// to the <code>Serializable</code> protocol will be saved when
	/// the object is stored.  If an AWT object has listeners that
	/// aren't marked serializable, they will be dropped at
	/// <code>writeObject</code> time.  Developers will need, as always,
	/// to consider the implications of making an object serializable.
	/// One situation to watch out for is this:
	/// <pre>
	///    import java.awt.*;
	///    import java.awt.event.*;
	///    import java.io.Serializable;
	/// 
	///    class MyApp implements ActionListener, Serializable
	///    {
	///        BigObjectThatShouldNotBeSerializedWithAButton bigOne;
	///        Button aButton = new Button();
	/// 
	///        MyApp()
	///        {
	///            // Oops, now aButton has a listener with a reference
	///            // to bigOne!
	///            aButton.addActionListener(this);
	///        }
	/// 
	///        public void actionPerformed(ActionEvent e)
	///        {
	///            System.out.println("Hello There");
	///        }
	///    }
	/// </pre>
	/// In this example, serializing <code>aButton</code> by itself
	/// will cause <code>MyApp</code> and everything it refers to
	/// to be serialized as well.  The problem is that the listener
	/// is serializable by coincidence, not by design.  To separate
	/// the decisions about <code>MyApp</code> and the
	/// <code>ActionListener</code> being serializable one can use a
	/// nested class, as in the following example:
	/// <pre>
	///    import java.awt.*;
	///    import java.awt.event.*;
	///    import java.io.Serializable;
	/// 
	///    class MyApp implements java.io.Serializable
	///    {
	///         BigObjectThatShouldNotBeSerializedWithAButton bigOne;
	///         Button aButton = new Button();
	/// 
	///         static class MyActionListener implements ActionListener
	///         {
	///             public void actionPerformed(ActionEvent e)
	///             {
	///                 System.out.println("Hello There");
	///             }
	///         }
	/// 
	///         MyApp()
	///         {
	///             aButton.addActionListener(new MyActionListener());
	///         }
	///    }
	/// </pre>
	/// </para>
	/// <para>
	/// <b>Note</b>: For more information on the paint mechanisms utilitized
	/// by AWT and Swing, including information on how to write the most
	/// efficient painting code, see
	/// <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
	/// </para>
	/// <para>
	/// For details on the focus subsystem, see
	/// <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
	/// How to Use the Focus Subsystem</a>,
	/// a section in <em>The Java Tutorial</em>, and the
	/// <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a>
	/// for more information.
	/// 
	/// @author      Arthur van Hoff
	/// @author      Sami Shaio
	/// </para>
	/// </summary>
	[Serializable]
	public abstract class Component : ImageObserver, MenuContainer
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			CoalescingEnabled_Renamed = CheckCoalescing();
		}


		private static readonly PlatformLogger Log = PlatformLogger.getLogger("java.awt.Component");
		private static readonly PlatformLogger EventLog = PlatformLogger.getLogger("java.awt.event.Component");
		private static readonly PlatformLogger FocusLog = PlatformLogger.getLogger("java.awt.focus.Component");
		private static readonly PlatformLogger MixingLog = PlatformLogger.getLogger("java.awt.mixing.Component");

		/// <summary>
		/// The peer of the component. The peer implements the component's
		/// behavior. The peer is set when the <code>Component</code> is
		/// added to a container that also is a peer. </summary>
		/// <seealso cref= #addNotify </seealso>
		/// <seealso cref= #removeNotify </seealso>
		[NonSerialized]
		internal ComponentPeer Peer_Renamed;

		/// <summary>
		/// The parent of the object. It may be <code>null</code>
		/// for top-level components. </summary>
		/// <seealso cref= #getParent </seealso>
		[NonSerialized]
		internal Container Parent_Renamed;

		/// <summary>
		/// The <code>AppContext</code> of the component. Applets/Plugin may
		/// change the AppContext.
		/// </summary>
		[NonSerialized]
		internal AppContext AppContext;

		/// <summary>
		/// The x position of the component in the parent's coordinate system.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLocation </seealso>
		internal int x;

		/// <summary>
		/// The y position of the component in the parent's coordinate system.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLocation </seealso>
		internal int y;

		/// <summary>
		/// The width of the component.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getSize </seealso>
		internal int Width_Renamed;

		/// <summary>
		/// The height of the component.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getSize </seealso>
		internal int Height_Renamed;

		/// <summary>
		/// The foreground color for this component.
		/// <code>foreground</code> can be <code>null</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getForeground </seealso>
		/// <seealso cref= #setForeground </seealso>
		internal Color Foreground_Renamed;

		/// <summary>
		/// The background color for this component.
		/// <code>background</code> can be <code>null</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getBackground </seealso>
		/// <seealso cref= #setBackground </seealso>
		internal Color Background_Renamed;

		/// <summary>
		/// The font used by this component.
		/// The <code>font</code> can be <code>null</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getFont </seealso>
		/// <seealso cref= #setFont </seealso>
		internal volatile Font Font_Renamed;

		/// <summary>
		/// The font which the peer is currently using.
		/// (<code>null</code> if no peer exists.)
		/// </summary>
		internal Font PeerFont;

		/// <summary>
		/// The cursor displayed when pointer is over this component.
		/// This value can be <code>null</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getCursor </seealso>
		/// <seealso cref= #setCursor </seealso>
		internal Cursor Cursor_Renamed;

		/// <summary>
		/// The locale for the component.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLocale </seealso>
		/// <seealso cref= #setLocale </seealso>
		internal Locale Locale_Renamed;

		/// <summary>
		/// A reference to a <code>GraphicsConfiguration</code> object
		/// used to describe the characteristics of a graphics
		/// destination.
		/// This value can be <code>null</code>.
		/// 
		/// @since 1.3
		/// @serial </summary>
		/// <seealso cref= GraphicsConfiguration </seealso>
		/// <seealso cref= #getGraphicsConfiguration </seealso>
		[NonSerialized]
		private GraphicsConfiguration GraphicsConfig = null;

		/// <summary>
		/// A reference to a <code>BufferStrategy</code> object
		/// used to manipulate the buffers on this component.
		/// 
		/// @since 1.4 </summary>
		/// <seealso cref= java.awt.image.BufferStrategy </seealso>
		/// <seealso cref= #getBufferStrategy() </seealso>
		[NonSerialized]
		internal BufferStrategy BufferStrategy_Renamed = null;

		/// <summary>
		/// True when the object should ignore all repaint events.
		/// 
		/// @since 1.4
		/// @serial </summary>
		/// <seealso cref= #setIgnoreRepaint </seealso>
		/// <seealso cref= #getIgnoreRepaint </seealso>
		internal bool IgnoreRepaint_Renamed = false;

		/// <summary>
		/// True when the object is visible. An object that is not
		/// visible is not drawn on the screen.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isVisible </seealso>
		/// <seealso cref= #setVisible </seealso>
		internal bool Visible_Renamed = true;

		/// <summary>
		/// True when the object is enabled. An object that is not
		/// enabled does not interact with the user.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isEnabled </seealso>
		/// <seealso cref= #setEnabled </seealso>
		internal bool Enabled_Renamed = true;

		/// <summary>
		/// True when the object is valid. An invalid object needs to
		/// be layed out. This flag is set to false when the object
		/// size is changed.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isValid </seealso>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= #invalidate </seealso>
		private volatile bool Valid_Renamed = false;

		/// <summary>
		/// The <code>DropTarget</code> associated with this component.
		/// 
		/// @since 1.2
		/// @serial </summary>
		/// <seealso cref= #setDropTarget </seealso>
		/// <seealso cref= #getDropTarget </seealso>
		internal DropTarget DropTarget_Renamed;

		/// <summary>
		/// @serial </summary>
		/// <seealso cref= #add </seealso>
		internal List<PopupMenu> Popups;

		/// <summary>
		/// A component's name.
		/// This field can be <code>null</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getName </seealso>
		/// <seealso cref= #setName(String) </seealso>
		private String Name_Renamed;

		/// <summary>
		/// A bool to determine whether the name has
		/// been set explicitly. <code>nameExplicitlySet</code> will
		/// be false if the name has not been set and
		/// true if it has.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getName </seealso>
		/// <seealso cref= #setName(String) </seealso>
		private bool NameExplicitlySet = false;

		/// <summary>
		/// Indicates whether this Component can be focused.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setFocusable </seealso>
		/// <seealso cref= #isFocusable
		/// @since 1.4 </seealso>
		private bool Focusable_Renamed = true;

		private const int FOCUS_TRAVERSABLE_UNKNOWN = 0;
		private const int FOCUS_TRAVERSABLE_DEFAULT = 1;
		private const int FOCUS_TRAVERSABLE_SET = 2;

		/// <summary>
		/// Tracks whether this Component is relying on default focus travesability.
		/// 
		/// @serial
		/// @since 1.4
		/// </summary>
		private int IsFocusTraversableOverridden = FOCUS_TRAVERSABLE_UNKNOWN;

		/// <summary>
		/// The focus traversal keys. These keys will generate focus traversal
		/// behavior for Components for which focus traversal keys are enabled. If a
		/// value of null is specified for a traversal key, this Component inherits
		/// that traversal key from its parent. If all ancestors of this Component
		/// have null specified for that traversal key, then the current
		/// KeyboardFocusManager's default traversal key is used.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setFocusTraversalKeys </seealso>
		/// <seealso cref= #getFocusTraversalKeys
		/// @since 1.4 </seealso>
		internal Set<AWTKeyStroke>[] FocusTraversalKeys;

		private static readonly String[] FocusTraversalKeyPropertyNames = new String[] {"forwardFocusTraversalKeys", "backwardFocusTraversalKeys", "upCycleFocusTraversalKeys", "downCycleFocusTraversalKeys"};

		/// <summary>
		/// Indicates whether focus traversal keys are enabled for this Component.
		/// Components for which focus traversal keys are disabled receive key
		/// events for focus traversal keys. Components for which focus traversal
		/// keys are enabled do not see these events; instead, the events are
		/// automatically converted to traversal operations.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setFocusTraversalKeysEnabled </seealso>
		/// <seealso cref= #getFocusTraversalKeysEnabled
		/// @since 1.4 </seealso>
		private bool FocusTraversalKeysEnabled_Renamed = true;

		/// <summary>
		/// The locking object for AWT component-tree and layout operations.
		/// </summary>
		/// <seealso cref= #getTreeLock </seealso>
		internal static readonly Object LOCK = new AWTTreeLock();
		internal class AWTTreeLock
		{
		}

		/*
		 * The component's AccessControlContext.
		 */
		[NonSerialized]
		private volatile AccessControlContext Acc = AccessController.Context;

		/// <summary>
		/// Minimum size.
		/// (This field perhaps should have been transient).
		/// 
		/// @serial
		/// </summary>
		internal Dimension MinSize;

		/// <summary>
		/// Whether or not setMinimumSize has been invoked with a non-null value.
		/// </summary>
		internal bool MinSizeSet;

		/// <summary>
		/// Preferred size.
		/// (This field perhaps should have been transient).
		/// 
		/// @serial
		/// </summary>
		internal Dimension PrefSize;

		/// <summary>
		/// Whether or not setPreferredSize has been invoked with a non-null value.
		/// </summary>
		internal bool PrefSizeSet;

		/// <summary>
		/// Maximum size
		/// 
		/// @serial
		/// </summary>
		internal Dimension MaxSize;

		/// <summary>
		/// Whether or not setMaximumSize has been invoked with a non-null value.
		/// </summary>
		internal bool MaxSizeSet;

		/// <summary>
		/// The orientation for this component. </summary>
		/// <seealso cref= #getComponentOrientation </seealso>
		/// <seealso cref= #setComponentOrientation </seealso>
		[NonSerialized]
		internal ComponentOrientation ComponentOrientation_Renamed = ComponentOrientation.UNKNOWN;

		/// <summary>
		/// <code>newEventsOnly</code> will be true if the event is
		/// one of the event types enabled for the component.
		/// It will then allow for normal processing to
		/// continue.  If it is false the event is passed
		/// to the component's parent and up the ancestor
		/// tree until the event has been consumed.
		/// 
		/// @serial </summary>
		/// <seealso cref= #dispatchEvent </seealso>
		internal bool NewEventsOnly = false;
		[NonSerialized]
		internal ComponentListener ComponentListener;
		[NonSerialized]
		internal FocusListener FocusListener;
		[NonSerialized]
		internal HierarchyListener HierarchyListener;
		[NonSerialized]
		internal HierarchyBoundsListener HierarchyBoundsListener;
		[NonSerialized]
		internal KeyListener KeyListener;
		[NonSerialized]
		internal MouseListener MouseListener;
		[NonSerialized]
		internal MouseMotionListener MouseMotionListener;
		[NonSerialized]
		internal MouseWheelListener MouseWheelListener;
		[NonSerialized]
		internal InputMethodListener InputMethodListener;

		[NonSerialized]
		internal RuntimeException WindowClosingException = null;

		/// <summary>
		/// Internal, constants for serialization </summary>
		internal const String ActionListenerK = "actionL";
		internal const String AdjustmentListenerK = "adjustmentL";
		internal const String ComponentListenerK = "componentL";
		internal const String ContainerListenerK = "containerL";
		internal const String FocusListenerK = "focusL";
		internal const String ItemListenerK = "itemL";
		internal const String KeyListenerK = "keyL";
		internal const String MouseListenerK = "mouseL";
		internal const String MouseMotionListenerK = "mouseMotionL";
		internal const String MouseWheelListenerK = "mouseWheelL";
		internal const String TextListenerK = "textL";
		internal const String OwnedWindowK = "ownedL";
		internal const String WindowListenerK = "windowL";
		internal const String InputMethodListenerK = "inputMethodL";
		internal const String HierarchyListenerK = "hierarchyL";
		internal const String HierarchyBoundsListenerK = "hierarchyBoundsL";
		internal const String WindowStateListenerK = "windowStateL";
		internal const String WindowFocusListenerK = "windowFocusL";

		/// <summary>
		/// The <code>eventMask</code> is ONLY set by subclasses via
		/// <code>enableEvents</code>.
		/// The mask should NOT be set when listeners are registered
		/// so that we can distinguish the difference between when
		/// listeners request events and subclasses request them.
		/// One bit is used to indicate whether input methods are
		/// enabled; this bit is set by <code>enableInputMethods</code> and is
		/// on by default.
		/// 
		/// @serial </summary>
		/// <seealso cref= #enableInputMethods </seealso>
		/// <seealso cref= AWTEvent </seealso>
		internal long EventMask = AWTEvent.INPUT_METHODS_ENABLED_MASK;

		/// <summary>
		/// Static properties for incremental drawing. </summary>
		/// <seealso cref= #imageUpdate </seealso>
		internal static bool IsInc;
		internal static int IncRate;
		static Component()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			/* initialize JNI field and method ids */
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}

			String s = AccessController.doPrivileged(new GetPropertyAction("awt.image.incrementaldraw"));
			IsInc = (s == null || s.Equals("true"));

			s = AccessController.doPrivileged(new GetPropertyAction("awt.image.redrawrate"));
			IncRate = (s != null) ? Convert.ToInt32(s) : 100;
			AWTAccessor.ComponentAccessor = new ComponentAccessorAnonymousInnerClassHelper();
		}

		private class ComponentAccessorAnonymousInnerClassHelper : AWTAccessor.ComponentAccessor
		{
			public ComponentAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual void SetBackgroundEraseDisabled(Component comp, bool disabled)
			{
				comp.BackgroundEraseDisabled = disabled;
			}
			public virtual bool GetBackgroundEraseDisabled(Component comp)
			{
				return comp.BackgroundEraseDisabled;
			}
			public virtual Rectangle GetBounds(Component comp)
			{
				return new Rectangle(comp.x, comp.y, comp.Width_Renamed, comp.Height_Renamed);
			}
			public virtual void SetMixingCutoutShape(Component comp, Shape shape)
			{
				Region region = shape == null ? null : Region.getInstance(shape, null);

				lock (comp.TreeLock)
				{
					bool needShowing = false;
					bool needHiding = false;

					if (!comp.NonOpaqueForMixing)
					{
						needHiding = true;
					}

					comp.MixingCutoutRegion = region;

					if (!comp.NonOpaqueForMixing)
					{
						needShowing = true;
					}

					if (comp.MixingNeeded)
					{
						if (needHiding)
						{
							comp.MixOnHiding(comp.Lightweight);
						}
						if (needShowing)
						{
							comp.MixOnShowing();
						}
					}
				}
			}

			public virtual void SetGraphicsConfiguration(Component comp, GraphicsConfiguration gc)
			{
				comp.GraphicsConfiguration = gc;
			}
			public virtual bool RequestFocus(Component comp, CausedFocusEvent.Cause cause)
			{
				return comp.RequestFocus(cause);
			}
			public virtual bool CanBeFocusOwner(Component comp)
			{
				return comp.CanBeFocusOwner();
			}

			public virtual bool IsVisible(Component comp)
			{
				return comp.Visible_NoClientCode;
			}
			public virtual RequestFocusController RequestFocusController
			{
				set
				{
					 Component.RequestFocusController = value;
				}
			}
			public virtual AppContext GetAppContext(Component comp)
			{
				 return comp.AppContext;
			}
			public virtual void SetAppContext(Component comp, AppContext appContext)
			{
				 comp.AppContext = appContext;
			}
			public virtual Container GetParent(Component comp)
			{
				return comp.Parent_NoClientCode;
			}
			public virtual void SetParent(Component comp, Container parent)
			{
				comp.Parent_Renamed = parent;
			}
			public virtual void SetSize(Component comp, int width, int height)
			{
				comp.Width_Renamed = width;
				comp.Height_Renamed = height;
			}
			public virtual Point GetLocation(Component comp)
			{
				return comp.Location_NoClientCode();
			}
			public virtual void SetLocation(Component comp, int x, int y)
			{
				comp.x = x;
				comp.y = y;
			}
			public virtual bool IsEnabled(Component comp)
			{
				return comp.EnabledImpl;
			}
			public virtual bool IsDisplayable(Component comp)
			{
				return comp.Peer_Renamed != null;
			}
			public virtual Cursor GetCursor(Component comp)
			{
				return comp.Cursor_NoClientCode;
			}
			public virtual ComponentPeer GetPeer(Component comp)
			{
				return comp.Peer_Renamed;
			}
			public virtual void SetPeer(Component comp, ComponentPeer peer)
			{
				comp.Peer_Renamed = peer;
			}
			public virtual bool IsLightweight(Component comp)
			{
				return (comp.Peer_Renamed is LightweightPeer);
			}
			public virtual bool GetIgnoreRepaint(Component comp)
			{
				return comp.IgnoreRepaint_Renamed;
			}
			public virtual int GetWidth(Component comp)
			{
				return comp.Width_Renamed;
			}
			public virtual int GetHeight(Component comp)
			{
				return comp.Height_Renamed;
			}
			public virtual int GetX(Component comp)
			{
				return comp.x;
			}
			public virtual int GetY(Component comp)
			{
				return comp.y;
			}
			public virtual Color GetForeground(Component comp)
			{
				return comp.Foreground_Renamed;
			}
			public virtual Color GetBackground(Component comp)
			{
				return comp.Background_Renamed;
			}
			public virtual void SetBackground(Component comp, Color background)
			{
				comp.Background_Renamed = background;
			}
			public virtual Font GetFont(Component comp)
			{
				return comp.Font_NoClientCode;
			}
			public virtual void ProcessEvent(Component comp, AWTEvent e)
			{
				comp.ProcessEvent(e);
			}

			public virtual AccessControlContext GetAccessControlContext(Component comp)
			{
				return comp.AccessControlContext;
			}

			public virtual void RevalidateSynchronously(Component comp)
			{
				comp.RevalidateSynchronously();
			}
		}

		/// <summary>
		/// Ease-of-use constant for <code>getAlignmentY()</code>.
		/// Specifies an alignment to the top of the component. </summary>
		/// <seealso cref=     #getAlignmentY </seealso>
		public const float TOP_ALIGNMENT = 0.0f;

		/// <summary>
		/// Ease-of-use constant for <code>getAlignmentY</code> and
		/// <code>getAlignmentX</code>. Specifies an alignment to
		/// the center of the component </summary>
		/// <seealso cref=     #getAlignmentX </seealso>
		/// <seealso cref=     #getAlignmentY </seealso>
		public const float CENTER_ALIGNMENT = 0.5f;

		/// <summary>
		/// Ease-of-use constant for <code>getAlignmentY</code>.
		/// Specifies an alignment to the bottom of the component. </summary>
		/// <seealso cref=     #getAlignmentY </seealso>
		public const float BOTTOM_ALIGNMENT = 1.0f;

		/// <summary>
		/// Ease-of-use constant for <code>getAlignmentX</code>.
		/// Specifies an alignment to the left side of the component. </summary>
		/// <seealso cref=     #getAlignmentX </seealso>
		public const float LEFT_ALIGNMENT = 0.0f;

		/// <summary>
		/// Ease-of-use constant for <code>getAlignmentX</code>.
		/// Specifies an alignment to the right side of the component. </summary>
		/// <seealso cref=     #getAlignmentX </seealso>
		public const float RIGHT_ALIGNMENT = 1.0f;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -7644114512714619750L;

		/// <summary>
		/// If any <code>PropertyChangeListeners</code> have been registered,
		/// the <code>changeSupport</code> field describes them.
		/// 
		/// @serial
		/// @since 1.2 </summary>
		/// <seealso cref= #addPropertyChangeListener </seealso>
		/// <seealso cref= #removePropertyChangeListener </seealso>
		/// <seealso cref= #firePropertyChange </seealso>
		private PropertyChangeSupport ChangeSupport;

		/*
		 * In some cases using "this" as an object to synchronize by
		 * can lead to a deadlock if client code also uses synchronization
		 * by a component object. For every such situation revealed we should
		 * consider possibility of replacing "this" with the package private
		 * objectLock object introduced below. So far there're 3 issues known:
		 * - CR 6708322 (the getName/setName methods);
		 * - CR 6608764 (the PropertyChangeListener machinery);
		 * - CR 7108598 (the Container.paint/KeyboardFocusManager.clearMostRecentFocusOwner methods).
		 *
		 * Note: this field is considered final, though readObject() prohibits
		 * initializing final fields.
		 */
		[NonSerialized]
		private Object ObjectLock_Renamed = new Object();
		internal virtual Object ObjectLock
		{
			get
			{
				return ObjectLock_Renamed;
			}
		}

		/*
		 * Returns the acc this component was constructed with.
		 */
		internal AccessControlContext AccessControlContext
		{
			get
			{
				if (Acc == null)
				{
					throw new SecurityException("Component is missing AccessControlContext");
				}
				return Acc;
			}
		}

		internal bool IsPacked = false;

		/// <summary>
		/// Pseudoparameter for direct Geometry API (setLocation, setBounds setSize
		/// to signal setBounds what's changing. Should be used under TreeLock.
		/// This is only needed due to the inability to change the cross-calling
		/// order of public and deprecated methods.
		/// </summary>
		private int BoundsOp_Renamed = java.awt.peer.ComponentPeer_Fields.DEFAULT_OPERATION;

		/// <summary>
		/// Enumeration of the common ways the baseline of a component can
		/// change as the size changes.  The baseline resize behavior is
		/// primarily for layout managers that need to know how the
		/// position of the baseline changes as the component size changes.
		/// In general the baseline resize behavior will be valid for sizes
		/// greater than or equal to the minimum size (the actual minimum
		/// size; not a developer specified minimum size).  For sizes
		/// smaller than the minimum size the baseline may change in a way
		/// other than the baseline resize behavior indicates.  Similarly,
		/// as the size approaches <code>Integer.MAX_VALUE</code> and/or
		/// <code>Short.MAX_VALUE</code> the baseline may change in a way
		/// other than the baseline resize behavior indicates.
		/// </summary>
		/// <seealso cref= #getBaselineResizeBehavior </seealso>
		/// <seealso cref= #getBaseline(int,int)
		/// @since 1.6 </seealso>
		public enum BaselineResizeBehavior
		{
			/// <summary>
			/// Indicates the baseline remains fixed relative to the
			/// y-origin.  That is, <code>getBaseline</code> returns
			/// the same value regardless of the height or width.  For example, a
			/// <code>JLabel</code> containing non-empty text with a
			/// vertical alignment of <code>TOP</code> should have a
			/// baseline type of <code>CONSTANT_ASCENT</code>.
			/// </summary>
			CONSTANT_ASCENT,

			/// <summary>
			/// Indicates the baseline remains fixed relative to the height
			/// and does not change as the width is varied.  That is, for
			/// any height H the difference between H and
			/// <code>getBaseline(w, H)</code> is the same.  For example, a
			/// <code>JLabel</code> containing non-empty text with a
			/// vertical alignment of <code>BOTTOM</code> should have a
			/// baseline type of <code>CONSTANT_DESCENT</code>.
			/// </summary>
			CONSTANT_DESCENT,

			/// <summary>
			/// Indicates the baseline remains a fixed distance from
			/// the center of the component.  That is, for any height H the
			/// difference between <code>getBaseline(w, H)</code> and
			/// <code>H / 2</code> is the same (plus or minus one depending upon
			/// rounding error).
			/// <para>
			/// Because of possible rounding errors it is recommended
			/// you ask for the baseline with two consecutive heights and use
			/// the return value to determine if you need to pad calculations
			/// by 1.  The following shows how to calculate the baseline for
			/// any height:
			/// <pre>
			///   Dimension preferredSize = component.getPreferredSize();
			///   int baseline = getBaseline(preferredSize.width,
			///                              preferredSize.height);
			///   int nextBaseline = getBaseline(preferredSize.width,
			///                                  preferredSize.height + 1);
			///   // Amount to add to height when calculating where baseline
			///   // lands for a particular height:
			///   int padding = 0;
			///   // Where the baseline is relative to the mid point
			///   int baselineOffset = baseline - height / 2;
			///   if (preferredSize.height % 2 == 0 &amp;&amp;
			///       baseline != nextBaseline) {
			///       padding = 1;
			///   }
			///   else if (preferredSize.height % 2 == 1 &amp;&amp;
			///            baseline == nextBaseline) {
			///       baselineOffset--;
			///       padding = 1;
			///   }
			///   // The following calculates where the baseline lands for
			///   // the height z:
			///   int calculatedBaseline = (z + padding) / 2 + baselineOffset;
			/// </pre>
			/// </para>
			/// </summary>
			CENTER_OFFSET,

			/// <summary>
			/// Indicates the baseline resize behavior can not be expressed using
			/// any of the other constants.  This may also indicate the baseline
			/// varies with the width of the component.  This is also returned
			/// by components that do not have a baseline.
			/// </summary>
			OTHER
		}

		/*
		 * The shape set with the applyCompoundShape() method. It uncludes the result
		 * of the HW/LW mixing related shape computation. It may also include
		 * the user-specified shape of the component.
		 * The 'null' value means the component has normal shape (or has no shape at all)
		 * and applyCompoundShape() will skip the following shape identical to normal.
		 */
		[NonSerialized]
		private Region CompoundShape = null;

		/*
		 * Represents the shape of this lightweight component to be cut out from
		 * heavyweight components should they intersect. Possible values:
		 *    1. null - consider the shape rectangular
		 *    2. EMPTY_REGION - nothing gets cut out (children still get cut out)
		 *    3. non-empty - this shape gets cut out.
		 */
		[NonSerialized]
		private Region MixingCutoutRegion = null;

		/*
		 * Indicates whether addNotify() is complete
		 * (i.e. the peer is created).
		 */
		[NonSerialized]
		private bool IsAddNotifyComplete = false;

		/// <summary>
		/// Should only be used in subclass getBounds to check that part of bounds
		/// is actualy changing
		/// </summary>
		internal virtual int BoundsOp
		{
			get
			{
				Debug.Assert(Thread.holdsLock(TreeLock));
				return BoundsOp_Renamed;
			}
			set
			{
				Debug.Assert(Thread.holdsLock(TreeLock));
				if (value == java.awt.peer.ComponentPeer_Fields.RESET_OPERATION)
				{
					BoundsOp_Renamed = java.awt.peer.ComponentPeer_Fields.DEFAULT_OPERATION;
				}
				else
				{
					if (BoundsOp_Renamed == java.awt.peer.ComponentPeer_Fields.DEFAULT_OPERATION)
					{
						BoundsOp_Renamed = value;
					}
				}
			}
		}


		// Whether this Component has had the background erase flag
		// specified via SunToolkit.disableBackgroundErase(). This is
		// needed in order to make this function work on X11 platforms,
		// where currently there is no chance to interpose on the creation
		// of the peer and therefore the call to XSetBackground.
		[NonSerialized]
		internal bool BackgroundEraseDisabled;


		/// <summary>
		/// Constructs a new component. Class <code>Component</code> can be
		/// extended directly to create a lightweight component that does not
		/// utilize an opaque native window. A lightweight component must be
		/// hosted by a native container somewhere higher up in the component
		/// tree (for example, by a <code>Frame</code> object).
		/// </summary>
		protected internal Component()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			AppContext = AppContext.AppContext;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) void initializeFocusTraversalKeys()
		internal virtual void InitializeFocusTraversalKeys()
		{
			FocusTraversalKeys = new Set[3];
		}

		/// <summary>
		/// Constructs a name for this component.  Called by <code>getName</code>
		/// when the name is <code>null</code>.
		/// </summary>
		internal virtual String ConstructComponentName()
		{
			return null; // For strict compliance with prior platform versions, a Component
						 // that doesn't set its name should return null from
						 // getName()
		}

		/// <summary>
		/// Gets the name of the component. </summary>
		/// <returns> this component's name </returns>
		/// <seealso cref=    #setName
		/// @since JDK1.1 </seealso>
		public virtual String Name
		{
			get
			{
				if (Name_Renamed == null && !NameExplicitlySet)
				{
					lock (ObjectLock)
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
				String oldName;
				lock (ObjectLock)
				{
					oldName = this.Name_Renamed;
					this.Name_Renamed = value;
					NameExplicitlySet = true;
				}
				FirePropertyChange("name", oldName, value);
			}
		}


		/// <summary>
		/// Gets the parent of this component. </summary>
		/// <returns> the parent container of this component
		/// @since JDK1.0 </returns>
		public virtual Container Parent
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
		internal Container Parent_NoClientCode
		{
			get
			{
				return Parent_Renamed;
			}
		}

		// This method is overridden in the Window class to return null,
		//    because the parent field of the Window object contains
		//    the owner of the window, not its parent.
		internal virtual Container Container
		{
			get
			{
				return Parent_NoClientCode;
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// programs should not directly manipulate peers;
		/// replaced by <code>boolean isDisplayable()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual ComponentPeer Peer
		{
			get
			{
				return Peer_Renamed;
			}
		}

		/// <summary>
		/// Associate a <code>DropTarget</code> with this component.
		/// The <code>Component</code> will receive drops only if it
		/// is enabled.
		/// </summary>
		/// <seealso cref= #isEnabled </seealso>
		/// <param name="dt"> The DropTarget </param>

		public virtual DropTarget DropTarget
		{
			set
			{
				lock (this)
				{
					if (value == DropTarget_Renamed || (DropTarget_Renamed != null && DropTarget_Renamed.Equals(value)))
					{
						return;
					}
            
					DropTarget old;
            
					if ((old = DropTarget_Renamed) != null)
					{
						if (Peer_Renamed != null)
						{
							DropTarget_Renamed.RemoveNotify(Peer_Renamed);
						}
            
						DropTarget t = DropTarget_Renamed;
            
						DropTarget_Renamed = null;
            
						try
						{
							t.Component = null;
						}
						catch (IllegalArgumentException)
						{
							// ignore it.
						}
					}
            
					// if we have a new one, and we have a peer, add it!
            
					if ((DropTarget_Renamed = value) != null)
					{
						try
						{
							DropTarget_Renamed.Component = this;
							if (Peer_Renamed != null)
							{
								DropTarget_Renamed.AddNotify(Peer_Renamed);
							}
						}
						catch (IllegalArgumentException)
						{
							if (old != null)
							{
								try
								{
									old.Component = this;
									if (Peer_Renamed != null)
									{
										DropTarget_Renamed.AddNotify(Peer_Renamed);
									}
								}
								catch (IllegalArgumentException)
								{
									// ignore it!
								}
							}
						}
					}
				}
			}
			get
			{
				lock (this)
				{
					return DropTarget_Renamed;
				}
			}
		}



		/// <summary>
		/// Gets the <code>GraphicsConfiguration</code> associated with this
		/// <code>Component</code>.
		/// If the <code>Component</code> has not been assigned a specific
		/// <code>GraphicsConfiguration</code>,
		/// the <code>GraphicsConfiguration</code> of the
		/// <code>Component</code> object's top-level container is
		/// returned.
		/// If the <code>Component</code> has been created, but not yet added
		/// to a <code>Container</code>, this method returns <code>null</code>.
		/// </summary>
		/// <returns> the <code>GraphicsConfiguration</code> used by this
		///          <code>Component</code> or <code>null</code>
		/// @since 1.3 </returns>
		public virtual GraphicsConfiguration GraphicsConfiguration
		{
			get
			{
				lock (TreeLock)
				{
					return GraphicsConfiguration_NoClientCode;
				}
			}
			set
			{
				lock (TreeLock)
				{
					if (UpdateGraphicsData(value))
					{
						RemoveNotify();
						AddNotify();
					}
				}
			}
		}

		internal GraphicsConfiguration GraphicsConfiguration_NoClientCode
		{
			get
			{
				return GraphicsConfig;
			}
		}


		internal virtual bool UpdateGraphicsData(GraphicsConfiguration gc)
		{
			CheckTreeLock();

			if (GraphicsConfig == gc)
			{
				return false;
			}

			GraphicsConfig = gc;

			ComponentPeer peer = Peer;
			if (peer != null)
			{
				return peer.UpdateGraphicsData(gc);
			}
			return false;
		}

		/// <summary>
		/// Checks that this component's <code>GraphicsDevice</code>
		/// <code>idString</code> matches the string argument.
		/// </summary>
		internal virtual void CheckGD(String stringID)
		{
			if (GraphicsConfig != null)
			{
				if (!GraphicsConfig.Device.IDstring.Equals(stringID))
				{
					throw new IllegalArgumentException("adding a container to a container on a different GraphicsDevice");
				}
			}
		}

		/// <summary>
		/// Gets this component's locking object (the object that owns the thread
		/// synchronization monitor) for AWT component-tree and layout
		/// operations. </summary>
		/// <returns> this component's locking object </returns>
		public Object TreeLock
		{
			get
			{
				return LOCK;
			}
		}

		internal void CheckTreeLock()
		{
			if (!Thread.holdsLock(TreeLock))
			{
				throw new IllegalStateException("This function should be called while holding treeLock");
			}
		}

		/// <summary>
		/// Gets the toolkit of this component. Note that
		/// the frame that contains a component controls which
		/// toolkit is used by that component. Therefore if the component
		/// is moved from one frame to another, the toolkit it uses may change. </summary>
		/// <returns>  the toolkit of this component
		/// @since JDK1.0 </returns>
		public virtual Toolkit Toolkit
		{
			get
			{
				return ToolkitImpl;
			}
		}

		/*
		 * This is called by the native code, so client code can't
		 * be called on the toolkit thread.
		 */
		internal Toolkit ToolkitImpl
		{
			get
			{
				Container parent = this.Parent_Renamed;
				if (parent != null)
				{
					return parent.ToolkitImpl;
				}
				return Toolkit.DefaultToolkit;
			}
		}

		/// <summary>
		/// Determines whether this component is valid. A component is valid
		/// when it is correctly sized and positioned within its parent
		/// container and all its children are also valid.
		/// In order to account for peers' size requirements, components are invalidated
		/// before they are first shown on the screen. By the time the parent container
		/// is fully realized, all its components will be valid. </summary>
		/// <returns> <code>true</code> if the component is valid, <code>false</code>
		/// otherwise </returns>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= #invalidate
		/// @since JDK1.0 </seealso>
		public virtual bool Valid
		{
			get
			{
				return (Peer_Renamed != null) && Valid_Renamed;
			}
		}

		/// <summary>
		/// Determines whether this component is displayable. A component is
		/// displayable when it is connected to a native screen resource.
		/// <para>
		/// A component is made displayable either when it is added to
		/// a displayable containment hierarchy or when its containment
		/// hierarchy is made displayable.
		/// A containment hierarchy is made displayable when its ancestor
		/// window is either packed or made visible.
		/// </para>
		/// <para>
		/// A component is made undisplayable either when it is removed from
		/// a displayable containment hierarchy or when its containment hierarchy
		/// is made undisplayable.  A containment hierarchy is made
		/// undisplayable when its ancestor window is disposed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> <code>true</code> if the component is displayable,
		/// <code>false</code> otherwise </returns>
		/// <seealso cref= Container#add(Component) </seealso>
		/// <seealso cref= Window#pack </seealso>
		/// <seealso cref= Window#show </seealso>
		/// <seealso cref= Container#remove(Component) </seealso>
		/// <seealso cref= Window#dispose
		/// @since 1.2 </seealso>
		public virtual bool Displayable
		{
			get
			{
				return Peer != null;
			}
		}

		/// <summary>
		/// Determines whether this component should be visible when its
		/// parent is visible. Components are
		/// initially visible, with the exception of top level components such
		/// as <code>Frame</code> objects. </summary>
		/// <returns> <code>true</code> if the component is visible,
		/// <code>false</code> otherwise </returns>
		/// <seealso cref= #setVisible
		/// @since JDK1.0 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transient public boolean isVisible()
		public virtual bool Visible
		{
			get
			{
				return Visible_NoClientCode;
			}
			set
			{
				Show(value);
			}
		}
		internal bool Visible_NoClientCode
		{
			get
			{
				return Visible_Renamed;
			}
		}

		/// <summary>
		/// Determines whether this component will be displayed on the screen. </summary>
		/// <returns> <code>true</code> if the component and all of its ancestors
		///          until a toplevel window or null parent are visible,
		///          <code>false</code> otherwise </returns>
		internal virtual bool RecursivelyVisible
		{
			get
			{
				return Visible_Renamed && (Parent_Renamed == null || Parent_Renamed.RecursivelyVisible);
			}
		}

		/// <summary>
		/// Determines the bounds of a visible part of the component relative to its
		/// parent.
		/// </summary>
		/// <returns> the visible part of bounds </returns>
		private Rectangle RecursivelyVisibleBounds
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Component container = getContainer();
				Component container = Container;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Rectangle bounds = getBounds();
				Rectangle bounds = Bounds;
				if (container == null)
				{
					// we are top level window or haven't a container, return our bounds
					return bounds;
				}
				// translate the container's bounds to our coordinate space
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Rectangle parentsBounds = container.getRecursivelyVisibleBounds();
				Rectangle parentsBounds = container.RecursivelyVisibleBounds;
				parentsBounds.SetLocation(0, 0);
				return parentsBounds.Intersection(bounds);
			}
		}

		/// <summary>
		/// Translates absolute coordinates into coordinates in the coordinate
		/// space of this component.
		/// </summary>
		internal virtual Point PointRelativeToComponent(Point absolute)
		{
			Point compCoords = LocationOnScreen;
			return new Point(absolute.x - compCoords.x, absolute.y - compCoords.y);
		}

		/// <summary>
		/// Assuming that mouse location is stored in PointerInfo passed
		/// to this method, it finds a Component that is in the same
		/// Window as this Component and is located under the mouse pointer.
		/// If no such Component exists, null is returned.
		/// NOTE: this method should be called under the protection of
		/// tree lock, as it is done in Component.getMousePosition() and
		/// Container.getMousePosition(boolean).
		/// </summary>
		internal virtual Component FindUnderMouseInWindow(PointerInfo pi)
		{
			if (!Showing)
			{
				return null;
			}
			Window win = ContainingWindow;
			if (!Toolkit.DefaultToolkit.MouseInfoPeer.IsWindowUnderMouse(win))
			{
				return null;
			}
			const bool INCLUDE_DISABLED = true;
			Point relativeToWindow = win.PointRelativeToComponent(pi.Location);
			Component inTheSameWindow = win.FindComponentAt(relativeToWindow.x, relativeToWindow.y, INCLUDE_DISABLED);
			return inTheSameWindow;
		}

		/// <summary>
		/// Returns the position of the mouse pointer in this <code>Component</code>'s
		/// coordinate space if the <code>Component</code> is directly under the mouse
		/// pointer, otherwise returns <code>null</code>.
		/// If the <code>Component</code> is not showing on the screen, this method
		/// returns <code>null</code> even if the mouse pointer is above the area
		/// where the <code>Component</code> would be displayed.
		/// If the <code>Component</code> is partially or fully obscured by other
		/// <code>Component</code>s or native windows, this method returns a non-null
		/// value only if the mouse pointer is located above the unobscured part of the
		/// <code>Component</code>.
		/// <para>
		/// For <code>Container</code>s it returns a non-null value if the mouse is
		/// above the <code>Container</code> itself or above any of its descendants.
		/// Use <seealso cref="Container#getMousePosition(boolean)"/> if you need to exclude children.
		/// </para>
		/// <para>
		/// Sometimes the exact mouse coordinates are not important, and the only thing
		/// that matters is whether a specific <code>Component</code> is under the mouse
		/// pointer. If the return value of this method is <code>null</code>, mouse
		/// pointer is not directly above the <code>Component</code>.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
		/// <seealso cref=       #isShowing </seealso>
		/// <seealso cref=       Container#getMousePosition </seealso>
		/// <returns>    mouse coordinates relative to this <code>Component</code>, or null
		/// @since     1.5 </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Point getMousePosition() throws HeadlessException
		public virtual Point MousePosition
		{
			get
			{
				if (GraphicsEnvironment.Headless)
				{
					throw new HeadlessException();
				}
    
				PointerInfo pi = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this)
																			);
    
				lock (TreeLock)
				{
					Component inTheSameWindow = FindUnderMouseInWindow(pi);
					if (!IsSameOrAncestorOf(inTheSameWindow, true))
					{
						return null;
					}
					return PointRelativeToComponent(pi.Location);
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<PointerInfo>
		{
			private readonly Component OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(Component outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual PointerInfo Run()
			{
				return MouseInfo.PointerInfo;
			}
		}

		/// <summary>
		/// Overridden in Container. Must be called under TreeLock.
		/// </summary>
		internal virtual bool IsSameOrAncestorOf(Component comp, bool allowChildren)
		{
			return comp == this;
		}

		/// <summary>
		/// Determines whether this component is showing on screen. This means
		/// that the component must be visible, and it must be in a container
		/// that is visible and showing.
		/// <para>
		/// <strong>Note:</strong> sometimes there is no way to detect whether the
		/// {@code Component} is actually visible to the user.  This can happen when:
		/// <ul>
		/// <li>the component has been added to a visible {@code ScrollPane} but
		/// the {@code Component} is not currently in the scroll pane's view port.
		/// <li>the {@code Component} is obscured by another {@code Component} or
		/// {@code Container}.
		/// </ul>
		/// </para>
		/// </summary>
		/// <returns> <code>true</code> if the component is showing,
		///          <code>false</code> otherwise </returns>
		/// <seealso cref= #setVisible
		/// @since JDK1.0 </seealso>
		public virtual bool Showing
		{
			get
			{
				if (Visible_Renamed && (Peer_Renamed != null))
				{
					Container parent = this.Parent_Renamed;
					return (parent == null) || parent.Showing;
				}
				return false;
			}
		}

		/// <summary>
		/// Determines whether this component is enabled. An enabled component
		/// can respond to user input and generate events. Components are
		/// enabled initially by default. A component may be enabled or disabled by
		/// calling its <code>setEnabled</code> method. </summary>
		/// <returns> <code>true</code> if the component is enabled,
		///          <code>false</code> otherwise </returns>
		/// <seealso cref= #setEnabled
		/// @since JDK1.0 </seealso>
		public virtual bool Enabled
		{
			get
			{
				return EnabledImpl;
			}
			set
			{
				Enable(value);
			}
		}

		/*
		 * This is called by the native code, so client code can't
		 * be called on the toolkit thread.
		 */
		internal bool EnabledImpl
		{
			get
			{
				return Enabled_Renamed;
			}
		}


		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setEnabled(boolean)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Enable()
		{
			if (!Enabled_Renamed)
			{
				lock (TreeLock)
				{
					Enabled_Renamed = true;
					ComponentPeer peer = this.Peer_Renamed;
					if (peer != null)
					{
						peer.Enabled = true;
						if (Visible_Renamed && !RecursivelyVisibleBounds.Empty)
						{
							UpdateCursorImmediately();
						}
					}
				}
				if (AccessibleContext_Renamed != null)
				{
					AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, null, AccessibleState.ENABLED);
				}
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setEnabled(boolean)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Enable(bool b)
		{
			if (b)
			{
				Enable();
			}
			else
			{
				Disable();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setEnabled(boolean)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Disable()
		{
			if (Enabled_Renamed)
			{
				KeyboardFocusManager.ClearMostRecentFocusOwner(this);
				lock (TreeLock)
				{
					Enabled_Renamed = false;
					// A disabled lw container is allowed to contain a focus owner.
					if ((FocusOwner || (ContainsFocus() && !Lightweight)) && KeyboardFocusManager.AutoFocusTransferEnabled)
					{
						// Don't clear the global focus owner. If transferFocus
						// fails, we want the focus to stay on the disabled
						// Component so that keyboard traversal, et. al. still
						// makes sense to the user.
						TransferFocus(false);
					}
					ComponentPeer peer = this.Peer_Renamed;
					if (peer != null)
					{
						peer.Enabled = false;
						if (Visible_Renamed && !RecursivelyVisibleBounds.Empty)
						{
							UpdateCursorImmediately();
						}
					}
				}
				if (AccessibleContext_Renamed != null)
				{
					AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, null, AccessibleState.ENABLED);
				}
			}
		}

		/// <summary>
		/// Returns true if this component is painted to an offscreen image
		/// ("buffer") that's copied to the screen later.  Component
		/// subclasses that support double buffering should override this
		/// method to return true if double buffering is enabled.
		/// </summary>
		/// <returns> false by default </returns>
		public virtual bool DoubleBuffered
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Enables or disables input method support for this component. If input
		/// method support is enabled and the component also processes key events,
		/// incoming events are offered to
		/// the current input method and will only be processed by the component or
		/// dispatched to its listeners if the input method does not consume them.
		/// By default, input method support is enabled.
		/// </summary>
		/// <param name="enable"> true to enable, false to disable </param>
		/// <seealso cref= #processKeyEvent
		/// @since 1.2 </seealso>
		public virtual void EnableInputMethods(bool enable)
		{
			if (enable)
			{
				if ((EventMask & AWTEvent.INPUT_METHODS_ENABLED_MASK) != 0)
				{
					return;
				}

				// If this component already has focus, then activate the
				// input method by dispatching a synthesized focus gained
				// event.
				if (FocusOwner)
				{
					InputContext inputContext = InputContext;
					if (inputContext != null)
					{
						FocusEvent focusGainedEvent = new FocusEvent(this, FocusEvent.FOCUS_GAINED);
						inputContext.DispatchEvent(focusGainedEvent);
					}
				}

				EventMask |= AWTEvent.INPUT_METHODS_ENABLED_MASK;
			}
			else
			{
				if ((EventMask & AWTEvent.INPUT_METHODS_ENABLED_MASK) != 0)
				{
					InputContext inputContext = InputContext;
					if (inputContext != null)
					{
						inputContext.EndComposition();
						inputContext.RemoveNotify(this);
					}
				}
				EventMask &= ~AWTEvent.INPUT_METHODS_ENABLED_MASK;
			}
		}


		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setVisible(boolean)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Show()
		{
			if (!Visible_Renamed)
			{
				lock (TreeLock)
				{
					Visible_Renamed = true;
					MixOnShowing();
					ComponentPeer peer = this.Peer_Renamed;
					if (peer != null)
					{
						peer.Visible = true;
						CreateHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, this, Parent_Renamed, HierarchyEvent.SHOWING_CHANGED, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK));
						if (peer is LightweightPeer)
						{
							Repaint();
						}
						UpdateCursorImmediately();
					}

					if (ComponentListener != null || (EventMask & AWTEvent.COMPONENT_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.COMPONENT_EVENT_MASK))
					{
						ComponentEvent e = new ComponentEvent(this, ComponentEvent.COMPONENT_SHOWN);
						Toolkit.EventQueue.PostEvent(e);
					}
				}
				Container parent = this.Parent_Renamed;
				if (parent != null)
				{
					parent.Invalidate();
				}
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setVisible(boolean)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Show(bool b)
		{
			if (b)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}

		internal virtual bool ContainsFocus()
		{
			return FocusOwner;
		}

		internal virtual void ClearMostRecentFocusOwnerOnHide()
		{
			KeyboardFocusManager.ClearMostRecentFocusOwner(this);
		}

		internal virtual void ClearCurrentFocusCycleRootOnHide()
		{
			/* do nothing */
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setVisible(boolean)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Hide()
		{
			IsPacked = false;

			if (Visible_Renamed)
			{
				ClearCurrentFocusCycleRootOnHide();
				ClearMostRecentFocusOwnerOnHide();
				lock (TreeLock)
				{
					Visible_Renamed = false;
					MixOnHiding(Lightweight);
					if (ContainsFocus() && KeyboardFocusManager.AutoFocusTransferEnabled)
					{
						TransferFocus(true);
					}
					ComponentPeer peer = this.Peer_Renamed;
					if (peer != null)
					{
						peer.Visible = false;
						CreateHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, this, Parent_Renamed, HierarchyEvent.SHOWING_CHANGED, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK));
						if (peer is LightweightPeer)
						{
							Repaint();
						}
						UpdateCursorImmediately();
					}
					if (ComponentListener != null || (EventMask & AWTEvent.COMPONENT_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.COMPONENT_EVENT_MASK))
					{
						ComponentEvent e = new ComponentEvent(this, ComponentEvent.COMPONENT_HIDDEN);
						Toolkit.EventQueue.PostEvent(e);
					}
				}
				Container parent = this.Parent_Renamed;
				if (parent != null)
				{
					parent.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets the foreground color of this component. </summary>
		/// <returns> this component's foreground color; if this component does
		/// not have a foreground color, the foreground color of its parent
		/// is returned </returns>
		/// <seealso cref= #setForeground
		/// @since JDK1.0
		/// @beaninfo
		///       bound: true </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transient public Color getForeground()
		public virtual Color Foreground
		{
			get
			{
				Color foreground = this.Foreground_Renamed;
				if (foreground != null)
				{
					return foreground;
				}
				Container parent = this.Parent_Renamed;
				return (parent != null) ? parent.Foreground : null;
			}
			set
			{
				Color oldColor = Foreground_Renamed;
				ComponentPeer peer = this.Peer_Renamed;
				Foreground_Renamed = value;
				if (peer != null)
				{
					value = Foreground;
					if (value != null)
					{
						peer.Foreground = value;
					}
				}
				// This is a bound property, so report the change to
				// any registered listeners.  (Cheap if there are none.)
				FirePropertyChange("foreground", oldColor, value);
			}
		}


		/// <summary>
		/// Returns whether the foreground color has been explicitly set for this
		/// Component. If this method returns <code>false</code>, this Component is
		/// inheriting its foreground color from an ancestor.
		/// </summary>
		/// <returns> <code>true</code> if the foreground color has been explicitly
		///         set for this Component; <code>false</code> otherwise.
		/// @since 1.4 </returns>
		public virtual bool ForegroundSet
		{
			get
			{
				return (Foreground_Renamed != null);
			}
		}

		/// <summary>
		/// Gets the background color of this component. </summary>
		/// <returns> this component's background color; if this component does
		///          not have a background color,
		///          the background color of its parent is returned </returns>
		/// <seealso cref= #setBackground
		/// @since JDK1.0 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transient public Color getBackground()
		public virtual Color Background
		{
			get
			{
				Color background = this.Background_Renamed;
				if (background != null)
				{
					return background;
				}
				Container parent = this.Parent_Renamed;
				return (parent != null) ? parent.Background : null;
			}
			set
			{
				Color oldColor = Background_Renamed;
				ComponentPeer peer = this.Peer_Renamed;
				Background_Renamed = value;
				if (peer != null)
				{
					value = Background;
					if (value != null)
					{
						peer.Background = value;
					}
				}
				// This is a bound property, so report the change to
				// any registered listeners.  (Cheap if there are none.)
				FirePropertyChange("background", oldColor, value);
			}
		}


		/// <summary>
		/// Returns whether the background color has been explicitly set for this
		/// Component. If this method returns <code>false</code>, this Component is
		/// inheriting its background color from an ancestor.
		/// </summary>
		/// <returns> <code>true</code> if the background color has been explicitly
		///         set for this Component; <code>false</code> otherwise.
		/// @since 1.4 </returns>
		public virtual bool BackgroundSet
		{
			get
			{
				return (Background_Renamed != null);
			}
		}

		/// <summary>
		/// Gets the font of this component. </summary>
		/// <returns> this component's font; if a font has not been set
		/// for this component, the font of its parent is returned </returns>
		/// <seealso cref= #setFont
		/// @since JDK1.0 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transient public Font getFont()
		public virtual Font Font
		{
			get
			{
				return Font_NoClientCode;
			}
			set
			{
				Font oldFont, newFont;
				lock (TreeLock)
				{
					oldFont = Font_Renamed;
					newFont = Font_Renamed = value;
					ComponentPeer peer = this.Peer_Renamed;
					if (peer != null)
					{
						value = Font;
						if (value != null)
						{
							peer.Font = value;
							PeerFont = value;
						}
					}
				}
				// This is a bound property, so report the change to
				// any registered listeners.  (Cheap if there are none.)
				FirePropertyChange("font", oldFont, newFont);
    
				// This could change the preferred size of the Component.
				// Fix for 6213660. Should compare old and new fonts and do not
				// call invalidate() if they are equal.
				if (value != oldFont && (oldFont == null || !oldFont.Equals(value)))
				{
					InvalidateIfValid();
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
				Container parent = this.Parent_Renamed;
				return (parent != null) ? parent.Font_NoClientCode : null;
			}
		}


		/// <summary>
		/// Returns whether the font has been explicitly set for this Component. If
		/// this method returns <code>false</code>, this Component is inheriting its
		/// font from an ancestor.
		/// </summary>
		/// <returns> <code>true</code> if the font has been explicitly set for this
		///         Component; <code>false</code> otherwise.
		/// @since 1.4 </returns>
		public virtual bool FontSet
		{
			get
			{
				return (Font_Renamed != null);
			}
		}

		/// <summary>
		/// Gets the locale of this component. </summary>
		/// <returns> this component's locale; if this component does not
		///          have a locale, the locale of its parent is returned </returns>
		/// <seealso cref= #setLocale </seealso>
		/// <exception cref="IllegalComponentStateException"> if the <code>Component</code>
		///          does not have its own locale and has not yet been added to
		///          a containment hierarchy such that the locale can be determined
		///          from the containing parent
		/// @since  JDK1.1 </exception>
		public virtual Locale Locale
		{
			get
			{
				Locale locale = this.Locale_Renamed;
				if (locale != null)
				{
					return locale;
				}
				Container parent = this.Parent_Renamed;
    
				if (parent == null)
				{
					throw new IllegalComponentStateException("This component must have a parent in order to determine its locale");
				}
				else
				{
					return parent.Locale;
				}
			}
			set
			{
				Locale oldValue = Locale_Renamed;
				Locale_Renamed = value;
    
				// This is a bound property, so report the change to
				// any registered listeners.  (Cheap if there are none.)
				FirePropertyChange("locale", oldValue, value);
    
				// This could change the preferred size of the Component.
				InvalidateIfValid();
			}
		}


		/// <summary>
		/// Gets the instance of <code>ColorModel</code> used to display
		/// the component on the output device. </summary>
		/// <returns> the color model used by this component </returns>
		/// <seealso cref= java.awt.image.ColorModel </seealso>
		/// <seealso cref= java.awt.peer.ComponentPeer#getColorModel() </seealso>
		/// <seealso cref= Toolkit#getColorModel()
		/// @since JDK1.0 </seealso>
		public virtual ColorModel ColorModel
		{
			get
			{
				ComponentPeer peer = this.Peer_Renamed;
				if ((peer != null) && !(peer is LightweightPeer))
				{
					return peer.ColorModel;
				}
				else if (GraphicsEnvironment.Headless)
				{
					return ColorModel.RGBdefault;
				} // else
				return Toolkit.ColorModel;
			}
		}

		/// <summary>
		/// Gets the location of this component in the form of a
		/// point specifying the component's top-left corner.
		/// The location will be relative to the parent's coordinate space.
		/// <para>
		/// Due to the asynchronous nature of native event handling, this
		/// method can return outdated values (for instance, after several calls
		/// of <code>setLocation()</code> in rapid succession).  For this
		/// reason, the recommended method of obtaining a component's position is
		/// within <code>java.awt.event.ComponentListener.componentMoved()</code>,
		/// which is called after the operating system has finished moving the
		/// component.
		/// </para> </summary>
		/// <returns> an instance of <code>Point</code> representing
		///          the top-left corner of the component's bounds in
		///          the coordinate space of the component's parent </returns>
		/// <seealso cref= #setLocation </seealso>
		/// <seealso cref= #getLocationOnScreen
		/// @since JDK1.1 </seealso>
		public virtual Point Location
		{
			get
			{
				return Location();
			}
			set
			{
				SetLocation(value.x, value.y);
			}
		}

		/// <summary>
		/// Gets the location of this component in the form of a point
		/// specifying the component's top-left corner in the screen's
		/// coordinate space. </summary>
		/// <returns> an instance of <code>Point</code> representing
		///          the top-left corner of the component's bounds in the
		///          coordinate space of the screen </returns>
		/// <exception cref="IllegalComponentStateException"> if the
		///          component is not showing on the screen </exception>
		/// <seealso cref= #setLocation </seealso>
		/// <seealso cref= #getLocation </seealso>
		public virtual Point LocationOnScreen
		{
			get
			{
				lock (TreeLock)
				{
					return LocationOnScreen_NoTreeLock;
				}
			}
		}

		/*
		 * a package private version of getLocationOnScreen
		 * used by GlobalCursormanager to update cursor
		 */
		internal Point LocationOnScreen_NoTreeLock
		{
			get
			{
    
				if (Peer_Renamed != null && Showing)
				{
					if (Peer_Renamed is LightweightPeer)
					{
						// lightweight component location needs to be translated
						// relative to a native component.
						Container host = NativeContainer;
						Point pt = host.Peer_Renamed.LocationOnScreen;
						for (Component c = this; c != host; c = c.Parent)
						{
							pt.x += c.x;
							pt.y += c.y;
						}
						return pt;
					}
					else
					{
						Point pt = Peer_Renamed.LocationOnScreen;
						return pt;
					}
				}
				else
				{
					throw new IllegalComponentStateException("component must be showing on the screen to determine its location");
				}
			}
		}


		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getLocation()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Point Location()
		{
			return Location_NoClientCode();
		}

		private Point Location_NoClientCode()
		{
			return new Point(x, y);
		}

		/// <summary>
		/// Moves this component to a new location. The top-left corner of
		/// the new location is specified by the <code>x</code> and <code>y</code>
		/// parameters in the coordinate space of this component's parent.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x"> the <i>x</i>-coordinate of the new location's
		///          top-left corner in the parent's coordinate space </param>
		/// <param name="y"> the <i>y</i>-coordinate of the new location's
		///          top-left corner in the parent's coordinate space </param>
		/// <seealso cref= #getLocation </seealso>
		/// <seealso cref= #setBounds </seealso>
		/// <seealso cref= #invalidate
		/// @since JDK1.1 </seealso>
		public virtual void SetLocation(int x, int y)
		{
			Move(x, y);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setLocation(int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Move(int x, int y)
		{
			lock (TreeLock)
			{
				BoundsOp = java.awt.peer.ComponentPeer_Fields.SET_LOCATION;
				SetBounds(x, y, Width_Renamed, Height_Renamed);
			}
		}


		/// <summary>
		/// Returns the size of this component in the form of a
		/// <code>Dimension</code> object. The <code>height</code>
		/// field of the <code>Dimension</code> object contains
		/// this component's height, and the <code>width</code>
		/// field of the <code>Dimension</code> object contains
		/// this component's width. </summary>
		/// <returns> a <code>Dimension</code> object that indicates the
		///          size of this component </returns>
		/// <seealso cref= #setSize
		/// @since JDK1.1 </seealso>
		public virtual Dimension Size
		{
			get
			{
				return Size();
			}
			set
			{
				Resize(value);
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getSize()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Dimension Size()
		{
			return new Dimension(Width_Renamed, Height_Renamed);
		}

		/// <summary>
		/// Resizes this component so that it has width <code>width</code>
		/// and height <code>height</code>.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy.
		/// 
		/// </para>
		/// </summary>
		/// <param name="width"> the new width of this component in pixels </param>
		/// <param name="height"> the new height of this component in pixels </param>
		/// <seealso cref= #getSize </seealso>
		/// <seealso cref= #setBounds </seealso>
		/// <seealso cref= #invalidate
		/// @since JDK1.1 </seealso>
		public virtual void SetSize(int width, int height)
		{
			Resize(width, height);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setSize(int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Resize(int width, int height)
		{
			lock (TreeLock)
			{
				BoundsOp = java.awt.peer.ComponentPeer_Fields.SET_SIZE;
				SetBounds(x, y, width, height);
			}
		}


		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setSize(Dimension)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Resize(Dimension d)
		{
			SetSize(d.Width_Renamed, d.Height_Renamed);
		}

		/// <summary>
		/// Gets the bounds of this component in the form of a
		/// <code>Rectangle</code> object. The bounds specify this
		/// component's width, height, and location relative to
		/// its parent. </summary>
		/// <returns> a rectangle indicating this component's bounds </returns>
		/// <seealso cref= #setBounds </seealso>
		/// <seealso cref= #getLocation </seealso>
		/// <seealso cref= #getSize </seealso>
		public virtual Rectangle Bounds
		{
			get
			{
				return Bounds();
			}
			set
			{
				SetBounds(value.x, value.y, value.Width_Renamed, value.Height_Renamed);
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getBounds()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Rectangle Bounds()
		{
			return new Rectangle(x, y, Width_Renamed, Height_Renamed);
		}

		/// <summary>
		/// Moves and resizes this component. The new location of the top-left
		/// corner is specified by <code>x</code> and <code>y</code>, and the
		/// new size is specified by <code>width</code> and <code>height</code>.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x"> the new <i>x</i>-coordinate of this component </param>
		/// <param name="y"> the new <i>y</i>-coordinate of this component </param>
		/// <param name="width"> the new <code>width</code> of this component </param>
		/// <param name="height"> the new <code>height</code> of this
		///          component </param>
		/// <seealso cref= #getBounds </seealso>
		/// <seealso cref= #setLocation(int, int) </seealso>
		/// <seealso cref= #setLocation(Point) </seealso>
		/// <seealso cref= #setSize(int, int) </seealso>
		/// <seealso cref= #setSize(Dimension) </seealso>
		/// <seealso cref= #invalidate
		/// @since JDK1.1 </seealso>
		public virtual void SetBounds(int x, int y, int width, int height)
		{
			Reshape(x, y, width, height);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setBounds(int, int, int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Reshape(int x, int y, int width, int height)
		{
			lock (TreeLock)
			{
				try
				{
					BoundsOp = java.awt.peer.ComponentPeer_Fields.SET_BOUNDS;
					bool resized = (this.Width_Renamed != width) || (this.Height_Renamed != height);
					bool moved = (this.x != x) || (this.y != y);
					if (!resized && !moved)
					{
						return;
					}
					int oldX = this.x;
					int oldY = this.y;
					int oldWidth = this.Width_Renamed;
					int oldHeight = this.Height_Renamed;
					this.x = x;
					this.y = y;
					this.Width_Renamed = width;
					this.Height_Renamed = height;

					if (resized)
					{
						IsPacked = false;
					}

					bool needNotify = true;
					MixOnReshaping();
					if (Peer_Renamed != null)
					{
						// LightwightPeer is an empty stub so can skip peer.reshape
						if (!(Peer_Renamed is LightweightPeer))
						{
							ReshapeNativePeer(x, y, width, height, BoundsOp);
							// Check peer actualy changed coordinates
							resized = (oldWidth != this.Width_Renamed) || (oldHeight != this.Height_Renamed);
							moved = (oldX != this.x) || (oldY != this.y);
							// fix for 5025858: do not send ComponentEvents for toplevel
							// windows here as it is done from peer or native code when
							// the window is really resized or moved, otherwise some
							// events may be sent twice
							if (this is Window)
							{
								needNotify = false;
							}
						}
						if (resized)
						{
							Invalidate();
						}
						if (Parent_Renamed != null)
						{
							Parent_Renamed.InvalidateIfValid();
						}
					}
					if (needNotify)
					{
						NotifyNewBounds(resized, moved);
					}
					RepaintParentIfNeeded(oldX, oldY, oldWidth, oldHeight);
				}
				finally
				{
					BoundsOp = java.awt.peer.ComponentPeer_Fields.RESET_OPERATION;
				}
			}
		}

		private void RepaintParentIfNeeded(int oldX, int oldY, int oldWidth, int oldHeight)
		{
			if (Parent_Renamed != null && Peer_Renamed is LightweightPeer && Showing)
			{
				// Have the parent redraw the area this component occupied.
				Parent_Renamed.Repaint(oldX, oldY, oldWidth, oldHeight);
				// Have the parent redraw the area this component *now* occupies.
				Repaint();
			}
		}

		private void ReshapeNativePeer(int x, int y, int width, int height, int op)
		{
			// native peer might be offset by more than direct
			// parent since parent might be lightweight.
			int nativeX = x;
			int nativeY = y;
			for (Component c = Parent_Renamed; (c != null) && (c.Peer_Renamed is LightweightPeer); c = c.Parent_Renamed)
			{
				nativeX += c.x;
				nativeY += c.y;
			}
			Peer_Renamed.SetBounds(nativeX, nativeY, width, height, op);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") private void notifyNewBounds(boolean resized, boolean moved)
		private void NotifyNewBounds(bool resized, bool moved)
		{
			if (ComponentListener != null || (EventMask & AWTEvent.COMPONENT_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.COMPONENT_EVENT_MASK))
			{
					if (resized)
					{
						ComponentEvent e = new ComponentEvent(this, ComponentEvent.COMPONENT_RESIZED);
						Toolkit.EventQueue.PostEvent(e);
					}
					if (moved)
					{
						ComponentEvent e = new ComponentEvent(this, ComponentEvent.COMPONENT_MOVED);
						Toolkit.EventQueue.PostEvent(e);
					}
			}
				else
				{
					if (this is Container && ((Container)this).CountComponents() > 0)
					{
						bool enabledOnToolkit = Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK);
						if (resized)
						{

							((Container)this).CreateChildHierarchyEvents(HierarchyEvent.ANCESTOR_RESIZED, 0, enabledOnToolkit);
						}
						if (moved)
						{
							((Container)this).CreateChildHierarchyEvents(HierarchyEvent.ANCESTOR_MOVED, 0, enabledOnToolkit);
						}
					}
				}
		}



		/// <summary>
		/// Returns the current x coordinate of the components origin.
		/// This method is preferable to writing
		/// <code>component.getBounds().x</code>,
		/// or <code>component.getLocation().x</code> because it doesn't
		/// cause any heap allocations.
		/// </summary>
		/// <returns> the current x coordinate of the components origin
		/// @since 1.2 </returns>
		public virtual int X
		{
			get
			{
				return x;
			}
		}


		/// <summary>
		/// Returns the current y coordinate of the components origin.
		/// This method is preferable to writing
		/// <code>component.getBounds().y</code>,
		/// or <code>component.getLocation().y</code> because it
		/// doesn't cause any heap allocations.
		/// </summary>
		/// <returns> the current y coordinate of the components origin
		/// @since 1.2 </returns>
		public virtual int Y
		{
			get
			{
				return y;
			}
		}


		/// <summary>
		/// Returns the current width of this component.
		/// This method is preferable to writing
		/// <code>component.getBounds().width</code>,
		/// or <code>component.getSize().width</code> because it
		/// doesn't cause any heap allocations.
		/// </summary>
		/// <returns> the current width of this component
		/// @since 1.2 </returns>
		public virtual int Width
		{
			get
			{
				return Width_Renamed;
			}
		}


		/// <summary>
		/// Returns the current height of this component.
		/// This method is preferable to writing
		/// <code>component.getBounds().height</code>,
		/// or <code>component.getSize().height</code> because it
		/// doesn't cause any heap allocations.
		/// </summary>
		/// <returns> the current height of this component
		/// @since 1.2 </returns>
		public virtual int Height
		{
			get
			{
				return Height_Renamed;
			}
		}

		/// <summary>
		/// Stores the bounds of this component into "return value" <b>rv</b> and
		/// return <b>rv</b>.  If rv is <code>null</code> a new
		/// <code>Rectangle</code> is allocated.
		/// This version of <code>getBounds</code> is useful if the caller
		/// wants to avoid allocating a new <code>Rectangle</code> object
		/// on the heap.
		/// </summary>
		/// <param name="rv"> the return value, modified to the components bounds </param>
		/// <returns> rv </returns>
		public virtual Rectangle GetBounds(Rectangle rv)
		{
			if (rv == null)
			{
				return new Rectangle(X, Y, Width, Height);
			}
			else
			{
				rv.SetBounds(X, Y, Width, Height);
				return rv;
			}
		}

		/// <summary>
		/// Stores the width/height of this component into "return value" <b>rv</b>
		/// and return <b>rv</b>.   If rv is <code>null</code> a new
		/// <code>Dimension</code> object is allocated.  This version of
		/// <code>getSize</code> is useful if the caller wants to avoid
		/// allocating a new <code>Dimension</code> object on the heap.
		/// </summary>
		/// <param name="rv"> the return value, modified to the components size </param>
		/// <returns> rv </returns>
		public virtual Dimension GetSize(Dimension rv)
		{
			if (rv == null)
			{
				return new Dimension(Width, Height);
			}
			else
			{
				rv.SetSize(Width, Height);
				return rv;
			}
		}

		/// <summary>
		/// Stores the x,y origin of this component into "return value" <b>rv</b>
		/// and return <b>rv</b>.   If rv is <code>null</code> a new
		/// <code>Point</code> is allocated.
		/// This version of <code>getLocation</code> is useful if the
		/// caller wants to avoid allocating a new <code>Point</code>
		/// object on the heap.
		/// </summary>
		/// <param name="rv"> the return value, modified to the components location </param>
		/// <returns> rv </returns>
		public virtual Point GetLocation(Point rv)
		{
			if (rv == null)
			{
				return new Point(X, Y);
			}
			else
			{
				rv.SetLocation(X, Y);
				return rv;
			}
		}

		/// <summary>
		/// Returns true if this component is completely opaque, returns
		/// false by default.
		/// <para>
		/// An opaque component paints every pixel within its
		/// rectangular region. A non-opaque component paints only some of
		/// its pixels, allowing the pixels underneath it to "show through".
		/// A component that does not fully paint its pixels therefore
		/// provides a degree of transparency.
		/// </para>
		/// <para>
		/// Subclasses that guarantee to always completely paint their
		/// contents should override this method and return true.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if this component is completely opaque </returns>
		/// <seealso cref= #isLightweight
		/// @since 1.2 </seealso>
		public virtual bool Opaque
		{
			get
			{
				if (Peer == null)
				{
					return false;
				}
				else
				{
					return !Lightweight;
				}
			}
		}


		/// <summary>
		/// A lightweight component doesn't have a native toolkit peer.
		/// Subclasses of <code>Component</code> and <code>Container</code>,
		/// other than the ones defined in this package like <code>Button</code>
		/// or <code>Scrollbar</code>, are lightweight.
		/// All of the Swing components are lightweights.
		/// <para>
		/// This method will always return <code>false</code> if this component
		/// is not displayable because it is impossible to determine the
		/// weight of an undisplayable component.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if this component has a lightweight peer; false if
		///         it has a native peer or no peer </returns>
		/// <seealso cref= #isDisplayable
		/// @since 1.2 </seealso>
		public virtual bool Lightweight
		{
			get
			{
				return Peer is LightweightPeer;
			}
		}


		/// <summary>
		/// Sets the preferred size of this component to a constant
		/// value.  Subsequent calls to <code>getPreferredSize</code> will always
		/// return this value.  Setting the preferred size to <code>null</code>
		/// restores the default behavior.
		/// </summary>
		/// <param name="preferredSize"> The new preferred size, or null </param>
		/// <seealso cref= #getPreferredSize </seealso>
		/// <seealso cref= #isPreferredSizeSet
		/// @since 1.5 </seealso>
		public virtual Dimension PreferredSize
		{
			set
			{
				Dimension old;
				// If the preferred size was set, use it as the old value, otherwise
				// use null to indicate we didn't previously have a set preferred
				// size.
				if (PrefSizeSet)
				{
					old = this.PrefSize;
				}
				else
				{
					old = null;
				}
				this.PrefSize = value;
				PrefSizeSet = (value != null);
				FirePropertyChange("preferredSize", old, value);
			}
			get
			{
				return PreferredSize();
			}
		}


		/// <summary>
		/// Returns true if the preferred size has been set to a
		/// non-<code>null</code> value otherwise returns false.
		/// </summary>
		/// <returns> true if <code>setPreferredSize</code> has been invoked
		///         with a non-null value.
		/// @since 1.5 </returns>
		public virtual bool PreferredSizeSet
		{
			get
			{
				return PrefSizeSet;
			}
		}




		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getPreferredSize()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Dimension PreferredSize()
		{
			/* Avoid grabbing the lock if a reasonable cached size value
			 * is available.
			 */
			Dimension dim = PrefSize;
			if (dim == null || !(PreferredSizeSet || Valid))
			{
				lock (TreeLock)
				{
					PrefSize = (Peer_Renamed != null) ? Peer_Renamed.PreferredSize : MinimumSize;
					dim = PrefSize;
				}
			}
			return new Dimension(dim);
		}

		/// <summary>
		/// Sets the minimum size of this component to a constant
		/// value.  Subsequent calls to <code>getMinimumSize</code> will always
		/// return this value.  Setting the minimum size to <code>null</code>
		/// restores the default behavior.
		/// </summary>
		/// <param name="minimumSize"> the new minimum size of this component </param>
		/// <seealso cref= #getMinimumSize </seealso>
		/// <seealso cref= #isMinimumSizeSet
		/// @since 1.5 </seealso>
		public virtual Dimension MinimumSize
		{
			set
			{
				Dimension old;
				// If the minimum size was set, use it as the old value, otherwise
				// use null to indicate we didn't previously have a set minimum
				// size.
				if (MinSizeSet)
				{
					old = this.MinSize;
				}
				else
				{
					old = null;
				}
				this.MinSize = value;
				MinSizeSet = (value != null);
				FirePropertyChange("minimumSize", old, value);
			}
			get
			{
				return MinimumSize();
			}
		}

		/// <summary>
		/// Returns whether or not <code>setMinimumSize</code> has been
		/// invoked with a non-null value.
		/// </summary>
		/// <returns> true if <code>setMinimumSize</code> has been invoked with a
		///              non-null value.
		/// @since 1.5 </returns>
		public virtual bool MinimumSizeSet
		{
			get
			{
				return MinSizeSet;
			}
		}


		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getMinimumSize()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Dimension MinimumSize()
		{
			/* Avoid grabbing the lock if a reasonable cached size value
			 * is available.
			 */
			Dimension dim = MinSize;
			if (dim == null || !(MinimumSizeSet || Valid))
			{
				lock (TreeLock)
				{
					MinSize = (Peer_Renamed != null) ? Peer_Renamed.MinimumSize : Size();
					dim = MinSize;
				}
			}
			return new Dimension(dim);
		}

		/// <summary>
		/// Sets the maximum size of this component to a constant
		/// value.  Subsequent calls to <code>getMaximumSize</code> will always
		/// return this value.  Setting the maximum size to <code>null</code>
		/// restores the default behavior.
		/// </summary>
		/// <param name="maximumSize"> a <code>Dimension</code> containing the
		///          desired maximum allowable size </param>
		/// <seealso cref= #getMaximumSize </seealso>
		/// <seealso cref= #isMaximumSizeSet
		/// @since 1.5 </seealso>
		public virtual Dimension MaximumSize
		{
			set
			{
				// If the maximum size was set, use it as the old value, otherwise
				// use null to indicate we didn't previously have a set maximum
				// size.
				Dimension old;
				if (MaxSizeSet)
				{
					old = this.MaxSize;
				}
				else
				{
					old = null;
				}
				this.MaxSize = value;
				MaxSizeSet = (value != null);
				FirePropertyChange("maximumSize", old, value);
			}
			get
			{
				if (MaximumSizeSet)
				{
					return new Dimension(MaxSize);
				}
				return new Dimension(Short.MaxValue, Short.MaxValue);
			}
		}

		/// <summary>
		/// Returns true if the maximum size has been set to a non-<code>null</code>
		/// value otherwise returns false.
		/// </summary>
		/// <returns> true if <code>maximumSize</code> is non-<code>null</code>,
		///          false otherwise
		/// @since 1.5 </returns>
		public virtual bool MaximumSizeSet
		{
			get
			{
				return MaxSizeSet;
			}
		}


		/// <summary>
		/// Returns the alignment along the x axis.  This specifies how
		/// the component would like to be aligned relative to other
		/// components.  The value should be a number between 0 and 1
		/// where 0 represents alignment along the origin, 1 is aligned
		/// the furthest away from the origin, 0.5 is centered, etc.
		/// </summary>
		public virtual float AlignmentX
		{
			get
			{
				return CENTER_ALIGNMENT;
			}
		}

		/// <summary>
		/// Returns the alignment along the y axis.  This specifies how
		/// the component would like to be aligned relative to other
		/// components.  The value should be a number between 0 and 1
		/// where 0 represents alignment along the origin, 1 is aligned
		/// the furthest away from the origin, 0.5 is centered, etc.
		/// </summary>
		public virtual float AlignmentY
		{
			get
			{
				return CENTER_ALIGNMENT;
			}
		}

		/// <summary>
		/// Returns the baseline.  The baseline is measured from the top of
		/// the component.  This method is primarily meant for
		/// <code>LayoutManager</code>s to align components along their
		/// baseline.  A return value less than 0 indicates this component
		/// does not have a reasonable baseline and that
		/// <code>LayoutManager</code>s should not align this component on
		/// its baseline.
		/// <para>
		/// The default implementation returns -1.  Subclasses that support
		/// baseline should override appropriately.  If a value &gt;= 0 is
		/// returned, then the component has a valid baseline for any
		/// size &gt;= the minimum size and <code>getBaselineResizeBehavior</code>
		/// can be used to determine how the baseline changes with size.
		/// 
		/// </para>
		/// </summary>
		/// <param name="width"> the width to get the baseline for </param>
		/// <param name="height"> the height to get the baseline for </param>
		/// <returns> the baseline or &lt; 0 indicating there is no reasonable
		///         baseline </returns>
		/// <exception cref="IllegalArgumentException"> if width or height is &lt; 0 </exception>
		/// <seealso cref= #getBaselineResizeBehavior </seealso>
		/// <seealso cref= java.awt.FontMetrics
		/// @since 1.6 </seealso>
		public virtual int GetBaseline(int width, int height)
		{
			if (width < 0 || height < 0)
			{
				throw new IllegalArgumentException("Width and height must be >= 0");
			}
			return -1;
		}

		/// <summary>
		/// Returns an enum indicating how the baseline of the component
		/// changes as the size changes.  This method is primarily meant for
		/// layout managers and GUI builders.
		/// <para>
		/// The default implementation returns
		/// <code>BaselineResizeBehavior.OTHER</code>.  Subclasses that have a
		/// baseline should override appropriately.  Subclasses should
		/// never return <code>null</code>; if the baseline can not be
		/// calculated return <code>BaselineResizeBehavior.OTHER</code>.  Callers
		/// should first ask for the baseline using
		/// <code>getBaseline</code> and if a value &gt;= 0 is returned use
		/// this method.  It is acceptable for this method to return a
		/// value other than <code>BaselineResizeBehavior.OTHER</code> even if
		/// <code>getBaseline</code> returns a value less than 0.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an enum indicating how the baseline changes as the component
		///         size changes </returns>
		/// <seealso cref= #getBaseline(int, int)
		/// @since 1.6 </seealso>
		public virtual BaselineResizeBehavior BaselineResizeBehavior
		{
			get
			{
				return BaselineResizeBehavior.OTHER;
			}
		}

		/// <summary>
		/// Prompts the layout manager to lay out this component. This is
		/// usually called when the component (more specifically, container)
		/// is validated. </summary>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= LayoutManager </seealso>
		public virtual void DoLayout()
		{
			Layout();
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>doLayout()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Layout()
		{
		}

		/// <summary>
		/// Validates this component.
		/// <para>
		/// The meaning of the term <i>validating</i> is defined by the ancestors of
		/// this class. See <seealso cref="Container#validate"/> for more details.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=       #invalidate </seealso>
		/// <seealso cref=       #doLayout() </seealso>
		/// <seealso cref=       LayoutManager </seealso>
		/// <seealso cref=       Container#validate
		/// @since     JDK1.0 </seealso>
		public virtual void Validate()
		{
			lock (TreeLock)
			{
				ComponentPeer peer = this.Peer_Renamed;
				bool wasValid = Valid;
				if (!wasValid && peer != null)
				{
					Font newfont = Font;
					Font oldfont = PeerFont;
					if (newfont != oldfont && (oldfont == null || !oldfont.Equals(newfont)))
					{
						peer.Font = newfont;
						PeerFont = newfont;
					}
					peer.Layout();
				}
				Valid_Renamed = true;
				if (!wasValid)
				{
					MixOnValidating();
				}
			}
		}

		/// <summary>
		/// Invalidates this component and its ancestors.
		/// <para>
		/// By default, all the ancestors of the component up to the top-most
		/// container of the hierarchy are marked invalid. If the {@code
		/// java.awt.smartInvalidate} system property is set to {@code true},
		/// invalidation stops on the nearest validate root of this component.
		/// Marking a container <i>invalid</i> indicates that the container needs to
		/// be laid out.
		/// </para>
		/// <para>
		/// This method is called automatically when any layout-related information
		/// changes (e.g. setting the bounds of the component, or adding the
		/// component to a container).
		/// </para>
		/// <para>
		/// This method might be called often, so it should work fast.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=       #validate </seealso>
		/// <seealso cref=       #doLayout </seealso>
		/// <seealso cref=       LayoutManager </seealso>
		/// <seealso cref=       java.awt.Container#isValidateRoot
		/// @since     JDK1.0 </seealso>
		public virtual void Invalidate()
		{
			lock (TreeLock)
			{
				/* Nullify cached layout and size information.
				 * For efficiency, propagate invalidate() upwards only if
				 * some other component hasn't already done so first.
				 */
				Valid_Renamed = false;
				if (!PreferredSizeSet)
				{
					PrefSize = null;
				}
				if (!MinimumSizeSet)
				{
					MinSize = null;
				}
				if (!MaximumSizeSet)
				{
					MaxSize = null;
				}
				InvalidateParent();
			}
		}

		/// <summary>
		/// Invalidates the parent of this component if any.
		/// 
		/// This method MUST BE invoked under the TreeLock.
		/// </summary>
		internal virtual void InvalidateParent()
		{
			if (Parent_Renamed != null)
			{
				Parent_Renamed.InvalidateIfValid();
			}
		}

		/// <summary>
		/// Invalidates the component unless it is already invalid.
		/// </summary>
		internal void InvalidateIfValid()
		{
			if (Valid)
			{
				Invalidate();
			}
		}

		/// <summary>
		/// Revalidates the component hierarchy up to the nearest validate root.
		/// <para>
		/// This method first invalidates the component hierarchy starting from this
		/// component up to the nearest validate root. Afterwards, the component
		/// hierarchy is validated starting from the nearest validate root.
		/// </para>
		/// <para>
		/// This is a convenience method supposed to help application developers
		/// avoid looking for validate roots manually. Basically, it's equivalent to
		/// first calling the <seealso cref="#invalidate()"/> method on this component, and
		/// then calling the <seealso cref="#validate()"/> method on the nearest validate
		/// root.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Container#isValidateRoot
		/// @since 1.7 </seealso>
		public virtual void Revalidate()
		{
			RevalidateSynchronously();
		}

		/// <summary>
		/// Revalidates the component synchronously.
		/// </summary>
		internal void RevalidateSynchronously()
		{
			lock (TreeLock)
			{
				Invalidate();

				Container root = Container;
				if (root == null)
				{
					// There's no parents. Just validate itself.
					Validate();
				}
				else
				{
					while (!root.ValidateRoot)
					{
						if (root.Container == null)
						{
							// If there's no validate roots, we'll validate the
							// topmost container
							break;
						}

						root = root.Container;
					}

					root.Validate();
				}
			}
		}

		/// <summary>
		/// Creates a graphics context for this component. This method will
		/// return <code>null</code> if this component is currently not
		/// displayable. </summary>
		/// <returns> a graphics context for this component, or <code>null</code>
		///             if it has none </returns>
		/// <seealso cref=       #paint
		/// @since     JDK1.0 </seealso>
		public virtual Graphics Graphics
		{
			get
			{
				if (Peer_Renamed is LightweightPeer)
				{
					// This is for a lightweight component, need to
					// translate coordinate spaces and clip relative
					// to the parent.
					if (Parent_Renamed == null)
					{
						return null;
					}
					Graphics g = Parent_Renamed.Graphics;
					if (g == null)
					{
						return null;
					}
					if (g is ConstrainableGraphics)
					{
						((ConstrainableGraphics) g).constrain(x, y, Width_Renamed, Height_Renamed);
					}
					else
					{
						g.Translate(x,y);
						g.SetClip(0, 0, Width_Renamed, Height_Renamed);
					}
					g.Font = Font;
					return g;
				}
				else
				{
					ComponentPeer peer = this.Peer_Renamed;
					return (peer != null) ? peer.Graphics : null;
				}
			}
		}

		internal Graphics Graphics_NoClientCode
		{
			get
			{
				ComponentPeer peer = this.Peer_Renamed;
				if (peer is LightweightPeer)
				{
					// This is for a lightweight component, need to
					// translate coordinate spaces and clip relative
					// to the parent.
					Container parent = this.Parent_Renamed;
					if (parent == null)
					{
						return null;
					}
					Graphics g = parent.Graphics_NoClientCode;
					if (g == null)
					{
						return null;
					}
					if (g is ConstrainableGraphics)
					{
						((ConstrainableGraphics) g).constrain(x, y, Width_Renamed, Height_Renamed);
					}
					else
					{
						g.Translate(x,y);
						g.SetClip(0, 0, Width_Renamed, Height_Renamed);
					}
					g.Font = Font_NoClientCode;
					return g;
				}
				else
				{
					return (peer != null) ? peer.Graphics : null;
				}
			}
		}

		/// <summary>
		/// Gets the font metrics for the specified font.
		/// Warning: Since Font metrics are affected by the
		/// <seealso cref="java.awt.font.FontRenderContext FontRenderContext"/> and
		/// this method does not provide one, it can return only metrics for
		/// the default render context which may not match that used when
		/// rendering on the Component if <seealso cref="Graphics2D"/> functionality is being
		/// used. Instead metrics can be obtained at rendering time by calling
		/// <seealso cref="Graphics#getFontMetrics()"/> or text measurement APIs on the
		/// <seealso cref="Font Font"/> class. </summary>
		/// <param name="font"> the font for which font metrics is to be
		///          obtained </param>
		/// <returns> the font metrics for <code>font</code> </returns>
		/// <seealso cref=       #getFont </seealso>
		/// <seealso cref=       #getPeer </seealso>
		/// <seealso cref=       java.awt.peer.ComponentPeer#getFontMetrics(Font) </seealso>
		/// <seealso cref=       Toolkit#getFontMetrics(Font)
		/// @since     JDK1.0 </seealso>
		public virtual FontMetrics GetFontMetrics(Font font)
		{
			// This is an unsupported hack, but left in for a customer.
			// Do not remove.
			FontManager fm = FontManagerFactory.Instance;
			if (fm is SunFontManager && ((SunFontManager) fm).usePlatformFontMetrics())
			{

				if (Peer_Renamed != null && !(Peer_Renamed is LightweightPeer))
				{
					return Peer_Renamed.GetFontMetrics(font);
				}
			}
			return sun.font.FontDesignMetrics.getMetrics(font);
		}

		/// <summary>
		/// Sets the cursor image to the specified cursor.  This cursor
		/// image is displayed when the <code>contains</code> method for
		/// this component returns true for the current cursor location, and
		/// this Component is visible, displayable, and enabled. Setting the
		/// cursor of a <code>Container</code> causes that cursor to be displayed
		/// within all of the container's subcomponents, except for those
		/// that have a non-<code>null</code> cursor.
		/// <para>
		/// The method may have no visual effect if the Java platform
		/// implementation and/or the native system do not support
		/// changing the mouse cursor shape.
		/// </para>
		/// </summary>
		/// <param name="cursor"> One of the constants defined
		///          by the <code>Cursor</code> class;
		///          if this parameter is <code>null</code>
		///          then this component will inherit
		///          the cursor of its parent </param>
		/// <seealso cref=       #isEnabled </seealso>
		/// <seealso cref=       #isShowing </seealso>
		/// <seealso cref=       #getCursor </seealso>
		/// <seealso cref=       #contains </seealso>
		/// <seealso cref=       Toolkit#createCustomCursor </seealso>
		/// <seealso cref=       Cursor
		/// @since     JDK1.1 </seealso>
		public virtual Cursor Cursor
		{
			set
			{
				this.Cursor_Renamed = value;
				UpdateCursorImmediately();
			}
			get
			{
				return Cursor_NoClientCode;
			}
		}

		/// <summary>
		/// Updates the cursor.  May not be invoked from the native
		/// message pump.
		/// </summary>
		internal void UpdateCursorImmediately()
		{
			if (Peer_Renamed is LightweightPeer)
			{
				Container nativeContainer = NativeContainer;

				if (nativeContainer == null)
				{
					return;
				}

				ComponentPeer cPeer = nativeContainer.Peer;

				if (cPeer != null)
				{
					cPeer.UpdateCursorImmediately();
				}
			}
			else if (Peer_Renamed != null)
			{
				Peer_Renamed.UpdateCursorImmediately();
			}
		}


		internal Cursor Cursor_NoClientCode
		{
			get
			{
				Cursor cursor = this.Cursor_Renamed;
				if (cursor != null)
				{
					return cursor;
				}
				Container parent = this.Parent_Renamed;
				if (parent != null)
				{
					return parent.Cursor_NoClientCode;
				}
				else
				{
					return Cursor.GetPredefinedCursor(Cursor.DEFAULT_CURSOR);
				}
			}
		}

		/// <summary>
		/// Returns whether the cursor has been explicitly set for this Component.
		/// If this method returns <code>false</code>, this Component is inheriting
		/// its cursor from an ancestor.
		/// </summary>
		/// <returns> <code>true</code> if the cursor has been explicitly set for this
		///         Component; <code>false</code> otherwise.
		/// @since 1.4 </returns>
		public virtual bool CursorSet
		{
			get
			{
				return (Cursor_Renamed != null);
			}
		}

		/// <summary>
		/// Paints this component.
		/// <para>
		/// This method is called when the contents of the component should
		/// be painted; such as when the component is first being shown or
		/// is damaged and in need of repair.  The clip rectangle in the
		/// <code>Graphics</code> parameter is set to the area
		/// which needs to be painted.
		/// Subclasses of <code>Component</code> that override this
		/// method need not call <code>super.paint(g)</code>.
		/// </para>
		/// <para>
		/// For performance reasons, <code>Component</code>s with zero width
		/// or height aren't considered to need painting when they are first shown,
		/// and also aren't considered to need repair.
		/// </para>
		/// <para>
		/// <b>Note</b>: For more information on the paint mechanisms utilitized
		/// by AWT and Swing, including information on how to write the most
		/// efficient painting code, see
		/// <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="g"> the graphics context to use for painting </param>
		/// <seealso cref=       #update
		/// @since     JDK1.0 </seealso>
		public virtual void Paint(Graphics g)
		{
		}

		/// <summary>
		/// Updates this component.
		/// <para>
		/// If this component is not a lightweight component, the
		/// AWT calls the <code>update</code> method in response to
		/// a call to <code>repaint</code>.  You can assume that
		/// the background is not cleared.
		/// </para>
		/// <para>
		/// The <code>update</code> method of <code>Component</code>
		/// calls this component's <code>paint</code> method to redraw
		/// this component.  This method is commonly overridden by subclasses
		/// which need to do additional work in response to a call to
		/// <code>repaint</code>.
		/// Subclasses of Component that override this method should either
		/// call <code>super.update(g)</code>, or call <code>paint(g)</code>
		/// directly from their <code>update</code> method.
		/// </para>
		/// <para>
		/// The origin of the graphics context, its
		/// (<code>0</code>,&nbsp;<code>0</code>) coordinate point, is the
		/// top-left corner of this component. The clipping region of the
		/// graphics context is the bounding rectangle of this component.
		/// 
		/// </para>
		/// <para>
		/// <b>Note</b>: For more information on the paint mechanisms utilitized
		/// by AWT and Swing, including information on how to write the most
		/// efficient painting code, see
		/// <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="g"> the specified context to use for updating </param>
		/// <seealso cref=       #paint </seealso>
		/// <seealso cref=       #repaint()
		/// @since     JDK1.0 </seealso>
		public virtual void Update(Graphics g)
		{
			Paint(g);
		}

		/// <summary>
		/// Paints this component and all of its subcomponents.
		/// <para>
		/// The origin of the graphics context, its
		/// (<code>0</code>,&nbsp;<code>0</code>) coordinate point, is the
		/// top-left corner of this component. The clipping region of the
		/// graphics context is the bounding rectangle of this component.
		/// 
		/// </para>
		/// </summary>
		/// <param name="g">   the graphics context to use for painting </param>
		/// <seealso cref=       #paint
		/// @since     JDK1.0 </seealso>
		public virtual void PaintAll(Graphics g)
		{
			if (Showing)
			{
				GraphicsCallback.PeerPaintCallback.Instance.runOneComponent(this, new Rectangle(0, 0, Width_Renamed, Height_Renamed), g, g.Clip, GraphicsCallback.LIGHTWEIGHTS | GraphicsCallback.HEAVYWEIGHTS);
			}
		}

		/// <summary>
		/// Simulates the peer callbacks into java.awt for painting of
		/// lightweight Components. </summary>
		/// <param name="g">   the graphics context to use for painting </param>
		/// <seealso cref=       #paintAll </seealso>
		internal virtual void LightweightPaint(Graphics g)
		{
			Paint(g);
		}

		/// <summary>
		/// Paints all the heavyweight subcomponents.
		/// </summary>
		internal virtual void PaintHeavyweightComponents(Graphics g)
		{
		}

		/// <summary>
		/// Repaints this component.
		/// <para>
		/// If this component is a lightweight component, this method
		/// causes a call to this component's <code>paint</code>
		/// method as soon as possible.  Otherwise, this method causes
		/// a call to this component's <code>update</code> method as soon
		/// as possible.
		/// </para>
		/// <para>
		/// <b>Note</b>: For more information on the paint mechanisms utilitized
		/// by AWT and Swing, including information on how to write the most
		/// efficient painting code, see
		/// <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=       #update(Graphics)
		/// @since     JDK1.0 </seealso>
		public virtual void Repaint()
		{
			Repaint(0, 0, 0, Width_Renamed, Height_Renamed);
		}

		/// <summary>
		/// Repaints the component.  If this component is a lightweight
		/// component, this results in a call to <code>paint</code>
		/// within <code>tm</code> milliseconds.
		/// <para>
		/// <b>Note</b>: For more information on the paint mechanisms utilitized
		/// by AWT and Swing, including information on how to write the most
		/// efficient painting code, see
		/// <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="tm"> maximum time in milliseconds before update </param>
		/// <seealso cref= #paint </seealso>
		/// <seealso cref= #update(Graphics)
		/// @since JDK1.0 </seealso>
		public virtual void Repaint(long tm)
		{
			Repaint(tm, 0, 0, Width_Renamed, Height_Renamed);
		}

		/// <summary>
		/// Repaints the specified rectangle of this component.
		/// <para>
		/// If this component is a lightweight component, this method
		/// causes a call to this component's <code>paint</code> method
		/// as soon as possible.  Otherwise, this method causes a call to
		/// this component's <code>update</code> method as soon as possible.
		/// </para>
		/// <para>
		/// <b>Note</b>: For more information on the paint mechanisms utilitized
		/// by AWT and Swing, including information on how to write the most
		/// efficient painting code, see
		/// <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x">   the <i>x</i> coordinate </param>
		/// <param name="y">   the <i>y</i> coordinate </param>
		/// <param name="width">   the width </param>
		/// <param name="height">  the height </param>
		/// <seealso cref=       #update(Graphics)
		/// @since     JDK1.0 </seealso>
		public virtual void Repaint(int x, int y, int width, int height)
		{
			Repaint(0, x, y, width, height);
		}

		/// <summary>
		/// Repaints the specified rectangle of this component within
		/// <code>tm</code> milliseconds.
		/// <para>
		/// If this component is a lightweight component, this method causes
		/// a call to this component's <code>paint</code> method.
		/// Otherwise, this method causes a call to this component's
		/// <code>update</code> method.
		/// </para>
		/// <para>
		/// <b>Note</b>: For more information on the paint mechanisms utilitized
		/// by AWT and Swing, including information on how to write the most
		/// efficient painting code, see
		/// <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="tm">   maximum time in milliseconds before update </param>
		/// <param name="x">    the <i>x</i> coordinate </param>
		/// <param name="y">    the <i>y</i> coordinate </param>
		/// <param name="width">    the width </param>
		/// <param name="height">   the height </param>
		/// <seealso cref=       #update(Graphics)
		/// @since     JDK1.0 </seealso>
		public virtual void Repaint(long tm, int x, int y, int width, int height)
		{
			if (this.Peer_Renamed is LightweightPeer)
			{
				// Needs to be translated to parent coordinates since
				// a parent native container provides the actual repaint
				// services.  Additionally, the request is restricted to
				// the bounds of the component.
				if (Parent_Renamed != null)
				{
					if (x < 0)
					{
						width += x;
						x = 0;
					}
					if (y < 0)
					{
						height += y;
						y = 0;
					}

					int pwidth = (width > this.Width_Renamed) ? this.Width_Renamed : width;
					int pheight = (height > this.Height_Renamed) ? this.Height_Renamed : height;

					if (pwidth <= 0 || pheight <= 0)
					{
						return;
					}

					int px = this.x + x;
					int py = this.y + y;
					Parent_Renamed.Repaint(tm, px, py, pwidth, pheight);
				}
			}
			else
			{
				if (Visible && (this.Peer_Renamed != null) && (width > 0) && (height > 0))
				{
					PaintEvent e = new PaintEvent(this, PaintEvent.UPDATE, new Rectangle(x, y, width, height));
					SunToolkit.postEvent(SunToolkit.targetToAppContext(this), e);
				}
			}
		}

		/// <summary>
		/// Prints this component. Applications should override this method
		/// for components that must do special processing before being
		/// printed or should be printed differently than they are painted.
		/// <para>
		/// The default implementation of this method calls the
		/// <code>paint</code> method.
		/// </para>
		/// <para>
		/// The origin of the graphics context, its
		/// (<code>0</code>,&nbsp;<code>0</code>) coordinate point, is the
		/// top-left corner of this component. The clipping region of the
		/// graphics context is the bounding rectangle of this component.
		/// </para>
		/// </summary>
		/// <param name="g">   the graphics context to use for printing </param>
		/// <seealso cref=       #paint(Graphics)
		/// @since     JDK1.0 </seealso>
		public virtual void Print(Graphics g)
		{
			Paint(g);
		}

		/// <summary>
		/// Prints this component and all of its subcomponents.
		/// <para>
		/// The origin of the graphics context, its
		/// (<code>0</code>,&nbsp;<code>0</code>) coordinate point, is the
		/// top-left corner of this component. The clipping region of the
		/// graphics context is the bounding rectangle of this component.
		/// </para>
		/// </summary>
		/// <param name="g">   the graphics context to use for printing </param>
		/// <seealso cref=       #print(Graphics)
		/// @since     JDK1.0 </seealso>
		public virtual void PrintAll(Graphics g)
		{
			if (Showing)
			{
				GraphicsCallback.PeerPrintCallback.Instance.runOneComponent(this, new Rectangle(0, 0, Width_Renamed, Height_Renamed), g, g.Clip, GraphicsCallback.LIGHTWEIGHTS | GraphicsCallback.HEAVYWEIGHTS);
			}
		}

		/// <summary>
		/// Simulates the peer callbacks into java.awt for printing of
		/// lightweight Components. </summary>
		/// <param name="g">   the graphics context to use for printing </param>
		/// <seealso cref=       #printAll </seealso>
		internal virtual void LightweightPrint(Graphics g)
		{
			Print(g);
		}

		/// <summary>
		/// Prints all the heavyweight subcomponents.
		/// </summary>
		internal virtual void PrintHeavyweightComponents(Graphics g)
		{
		}

		private Insets Insets_NoClientCode
		{
			get
			{
				ComponentPeer peer = this.Peer_Renamed;
				if (peer is ContainerPeer)
				{
					return (Insets)((ContainerPeer)peer).Insets.Clone();
				}
				return new Insets(0, 0, 0, 0);
			}
		}

		/// <summary>
		/// Repaints the component when the image has changed.
		/// This <code>imageUpdate</code> method of an <code>ImageObserver</code>
		/// is called when more information about an
		/// image which had been previously requested using an asynchronous
		/// routine such as the <code>drawImage</code> method of
		/// <code>Graphics</code> becomes available.
		/// See the definition of <code>imageUpdate</code> for
		/// more information on this method and its arguments.
		/// <para>
		/// The <code>imageUpdate</code> method of <code>Component</code>
		/// incrementally draws an image on the component as more of the bits
		/// of the image are available.
		/// </para>
		/// <para>
		/// If the system property <code>awt.image.incrementaldraw</code>
		/// is missing or has the value <code>true</code>, the image is
		/// incrementally drawn. If the system property has any other value,
		/// then the image is not drawn until it has been completely loaded.
		/// </para>
		/// <para>
		/// Also, if incremental drawing is in effect, the value of the
		/// system property <code>awt.image.redrawrate</code> is interpreted
		/// as an integer to give the maximum redraw rate, in milliseconds. If
		/// the system property is missing or cannot be interpreted as an
		/// integer, the redraw rate is once every 100ms.
		/// </para>
		/// <para>
		/// The interpretation of the <code>x</code>, <code>y</code>,
		/// <code>width</code>, and <code>height</code> arguments depends on
		/// the value of the <code>infoflags</code> argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="img">   the image being observed </param>
		/// <param name="infoflags">   see <code>imageUpdate</code> for more information </param>
		/// <param name="x">   the <i>x</i> coordinate </param>
		/// <param name="y">   the <i>y</i> coordinate </param>
		/// <param name="w">   the width </param>
		/// <param name="h">   the height </param>
		/// <returns>    <code>false</code> if the infoflags indicate that the
		///            image is completely loaded; <code>true</code> otherwise.
		/// </returns>
		/// <seealso cref=     java.awt.image.ImageObserver </seealso>
		/// <seealso cref=     Graphics#drawImage(Image, int, int, Color, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=     Graphics#drawImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=     Graphics#drawImage(Image, int, int, int, int, Color, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=     Graphics#drawImage(Image, int, int, int, int, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=     java.awt.image.ImageObserver#imageUpdate(java.awt.Image, int, int, int, int, int)
		/// @since   JDK1.0 </seealso>
		public virtual bool ImageUpdate(Image img, int infoflags, int x, int y, int w, int h)
		{
			int rate = -1;
			if ((infoflags & (image.ImageObserver_Fields.FRAMEBITS | image.ImageObserver_Fields.ALLBITS)) != 0)
			{
				rate = 0;
			}
			else if ((infoflags & image.ImageObserver_Fields.SOMEBITS) != 0)
			{
				if (IsInc)
				{
					rate = IncRate;
					if (rate < 0)
					{
						rate = 0;
					}
				}
			}
			if (rate >= 0)
			{
				Repaint(rate, 0, 0, Width_Renamed, Height_Renamed);
			}
			return (infoflags & (image.ImageObserver_Fields.ALLBITS | image.ImageObserver_Fields.ABORT)) == 0;
		}

		/// <summary>
		/// Creates an image from the specified image producer. </summary>
		/// <param name="producer">  the image producer </param>
		/// <returns>    the image produced
		/// @since     JDK1.0 </returns>
		public virtual Image CreateImage(ImageProducer producer)
		{
			ComponentPeer peer = this.Peer_Renamed;
			if ((peer != null) && !(peer is LightweightPeer))
			{
				return peer.CreateImage(producer);
			}
			return Toolkit.CreateImage(producer);
		}

		/// <summary>
		/// Creates an off-screen drawable image
		///     to be used for double buffering. </summary>
		/// <param name="width"> the specified width </param>
		/// <param name="height"> the specified height </param>
		/// <returns>    an off-screen drawable image, which can be used for double
		///    buffering.  The return value may be <code>null</code> if the
		///    component is not displayable.  This will always happen if
		///    <code>GraphicsEnvironment.isHeadless()</code> returns
		///    <code>true</code>. </returns>
		/// <seealso cref= #isDisplayable </seealso>
		/// <seealso cref= GraphicsEnvironment#isHeadless
		/// @since     JDK1.0 </seealso>
		public virtual Image CreateImage(int width, int height)
		{
			ComponentPeer peer = this.Peer_Renamed;
			if (peer is LightweightPeer)
			{
				if (Parent_Renamed != null)
				{
					return Parent_Renamed.CreateImage(width, height);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return (peer != null) ? peer.CreateImage(width, height) : null;
			}
		}

		/// <summary>
		/// Creates a volatile off-screen drawable image
		///     to be used for double buffering. </summary>
		/// <param name="width"> the specified width. </param>
		/// <param name="height"> the specified height. </param>
		/// <returns>    an off-screen drawable image, which can be used for double
		///    buffering.  The return value may be <code>null</code> if the
		///    component is not displayable.  This will always happen if
		///    <code>GraphicsEnvironment.isHeadless()</code> returns
		///    <code>true</code>. </returns>
		/// <seealso cref= java.awt.image.VolatileImage </seealso>
		/// <seealso cref= #isDisplayable </seealso>
		/// <seealso cref= GraphicsEnvironment#isHeadless
		/// @since     1.4 </seealso>
		public virtual VolatileImage CreateVolatileImage(int width, int height)
		{
			ComponentPeer peer = this.Peer_Renamed;
			if (peer is LightweightPeer)
			{
				if (Parent_Renamed != null)
				{
					return Parent_Renamed.CreateVolatileImage(width, height);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return (peer != null) ? peer.CreateVolatileImage(width, height) : null;
			}
		}

		/// <summary>
		/// Creates a volatile off-screen drawable image, with the given capabilities.
		/// The contents of this image may be lost at any time due
		/// to operating system issues, so the image must be managed
		/// via the <code>VolatileImage</code> interface. </summary>
		/// <param name="width"> the specified width. </param>
		/// <param name="height"> the specified height. </param>
		/// <param name="caps"> the image capabilities </param>
		/// <exception cref="AWTException"> if an image with the specified capabilities cannot
		/// be created </exception>
		/// <returns> a VolatileImage object, which can be used
		/// to manage surface contents loss and capabilities. </returns>
		/// <seealso cref= java.awt.image.VolatileImage
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.awt.image.VolatileImage createVolatileImage(int width, int height, ImageCapabilities caps) throws AWTException
		public virtual VolatileImage CreateVolatileImage(int width, int height, ImageCapabilities caps)
		{
			// REMIND : check caps
			return CreateVolatileImage(width, height);
		}

		/// <summary>
		/// Prepares an image for rendering on this component.  The image
		/// data is downloaded asynchronously in another thread and the
		/// appropriate screen representation of the image is generated. </summary>
		/// <param name="image">   the <code>Image</code> for which to
		///                    prepare a screen representation </param>
		/// <param name="observer">   the <code>ImageObserver</code> object
		///                       to be notified as the image is being prepared </param>
		/// <returns>    <code>true</code> if the image has already been fully
		///           prepared; <code>false</code> otherwise
		/// @since     JDK1.0 </returns>
		public virtual bool PrepareImage(Image image, ImageObserver observer)
		{
			return PrepareImage(image, -1, -1, observer);
		}

		/// <summary>
		/// Prepares an image for rendering on this component at the
		/// specified width and height.
		/// <para>
		/// The image data is downloaded asynchronously in another thread,
		/// and an appropriately scaled screen representation of the image is
		/// generated.
		/// </para>
		/// </summary>
		/// <param name="image">    the instance of <code>Image</code>
		///            for which to prepare a screen representation </param>
		/// <param name="width">    the width of the desired screen representation </param>
		/// <param name="height">   the height of the desired screen representation </param>
		/// <param name="observer">   the <code>ImageObserver</code> object
		///            to be notified as the image is being prepared </param>
		/// <returns>    <code>true</code> if the image has already been fully
		///          prepared; <code>false</code> otherwise </returns>
		/// <seealso cref=       java.awt.image.ImageObserver
		/// @since     JDK1.0 </seealso>
		public virtual bool PrepareImage(Image image, int width, int height, ImageObserver observer)
		{
			ComponentPeer peer = this.Peer_Renamed;
			if (peer is LightweightPeer)
			{
				return (Parent_Renamed != null) ? Parent_Renamed.PrepareImage(image, width, height, observer) : Toolkit.PrepareImage(image, width, height, observer);
			}
			else
			{
				return (peer != null) ? peer.PrepareImage(image, width, height, observer) : Toolkit.PrepareImage(image, width, height, observer);
			}
		}

		/// <summary>
		/// Returns the status of the construction of a screen representation
		/// of the specified image.
		/// <para>
		/// This method does not cause the image to begin loading. An
		/// application must use the <code>prepareImage</code> method
		/// to force the loading of an image.
		/// </para>
		/// <para>
		/// Information on the flags returned by this method can be found
		/// with the discussion of the <code>ImageObserver</code> interface.
		/// </para>
		/// </summary>
		/// <param name="image">   the <code>Image</code> object whose status
		///            is being checked </param>
		/// <param name="observer">   the <code>ImageObserver</code>
		///            object to be notified as the image is being prepared </param>
		/// <returns>  the bitwise inclusive <b>OR</b> of
		///            <code>ImageObserver</code> flags indicating what
		///            information about the image is currently available </returns>
		/// <seealso cref=      #prepareImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=      Toolkit#checkImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=      java.awt.image.ImageObserver
		/// @since    JDK1.0 </seealso>
		public virtual int CheckImage(Image image, ImageObserver observer)
		{
			return CheckImage(image, -1, -1, observer);
		}

		/// <summary>
		/// Returns the status of the construction of a screen representation
		/// of the specified image.
		/// <para>
		/// This method does not cause the image to begin loading. An
		/// application must use the <code>prepareImage</code> method
		/// to force the loading of an image.
		/// </para>
		/// <para>
		/// The <code>checkImage</code> method of <code>Component</code>
		/// calls its peer's <code>checkImage</code> method to calculate
		/// the flags. If this component does not yet have a peer, the
		/// component's toolkit's <code>checkImage</code> method is called
		/// instead.
		/// </para>
		/// <para>
		/// Information on the flags returned by this method can be found
		/// with the discussion of the <code>ImageObserver</code> interface.
		/// </para>
		/// </summary>
		/// <param name="image">   the <code>Image</code> object whose status
		///                    is being checked </param>
		/// <param name="width">   the width of the scaled version
		///                    whose status is to be checked </param>
		/// <param name="height">  the height of the scaled version
		///                    whose status is to be checked </param>
		/// <param name="observer">   the <code>ImageObserver</code> object
		///                    to be notified as the image is being prepared </param>
		/// <returns>    the bitwise inclusive <b>OR</b> of
		///            <code>ImageObserver</code> flags indicating what
		///            information about the image is currently available </returns>
		/// <seealso cref=      #prepareImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=      Toolkit#checkImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
		/// <seealso cref=      java.awt.image.ImageObserver
		/// @since    JDK1.0 </seealso>
		public virtual int CheckImage(Image image, int width, int height, ImageObserver observer)
		{
			ComponentPeer peer = this.Peer_Renamed;
			if (peer is LightweightPeer)
			{
				return (Parent_Renamed != null) ? Parent_Renamed.CheckImage(image, width, height, observer) : Toolkit.CheckImage(image, width, height, observer);
			}
			else
			{
				return (peer != null) ? peer.CheckImage(image, width, height, observer) : Toolkit.CheckImage(image, width, height, observer);
			}
		}

		/// <summary>
		/// Creates a new strategy for multi-buffering on this component.
		/// Multi-buffering is useful for rendering performance.  This method
		/// attempts to create the best strategy available with the number of
		/// buffers supplied.  It will always create a <code>BufferStrategy</code>
		/// with that number of buffers.
		/// A page-flipping strategy is attempted first, then a blitting strategy
		/// using accelerated buffers.  Finally, an unaccelerated blitting
		/// strategy is used.
		/// <para>
		/// Each time this method is called,
		/// the existing buffer strategy for this component is discarded.
		/// </para>
		/// </summary>
		/// <param name="numBuffers"> number of buffers to create, including the front buffer </param>
		/// <exception cref="IllegalArgumentException"> if numBuffers is less than 1. </exception>
		/// <exception cref="IllegalStateException"> if the component is not displayable </exception>
		/// <seealso cref= #isDisplayable </seealso>
		/// <seealso cref= Window#getBufferStrategy() </seealso>
		/// <seealso cref= Canvas#getBufferStrategy()
		/// @since 1.4 </seealso>
		internal virtual void CreateBufferStrategy(int numBuffers)
		{
			BufferCapabilities bufferCaps;
			if (numBuffers > 1)
			{
				// Try to create a page-flipping strategy
				bufferCaps = new BufferCapabilities(new ImageCapabilities(true), new ImageCapabilities(true), BufferCapabilities.FlipContents.UNDEFINED);
				try
				{
					CreateBufferStrategy(numBuffers, bufferCaps);
					return; // Success
				}
				catch (AWTException)
				{
					// Failed
				}
			}
			// Try a blitting (but still accelerated) strategy
			bufferCaps = new BufferCapabilities(new ImageCapabilities(true), new ImageCapabilities(true), null);
			try
			{
				CreateBufferStrategy(numBuffers, bufferCaps);
				return; // Success
			}
			catch (AWTException)
			{
				// Failed
			}
			// Try an unaccelerated blitting strategy
			bufferCaps = new BufferCapabilities(new ImageCapabilities(false), new ImageCapabilities(false), null);
			try
			{
				CreateBufferStrategy(numBuffers, bufferCaps);
				return; // Success
			}
			catch (AWTException e)
			{
				// Code should never reach here (an unaccelerated blitting
				// strategy should always work)
				throw new InternalError("Could not create a buffer strategy", e);
			}
		}

		/// <summary>
		/// Creates a new strategy for multi-buffering on this component with the
		/// required buffer capabilities.  This is useful, for example, if only
		/// accelerated memory or page flipping is desired (as specified by the
		/// buffer capabilities).
		/// <para>
		/// Each time this method
		/// is called, <code>dispose</code> will be invoked on the existing
		/// <code>BufferStrategy</code>.
		/// </para>
		/// </summary>
		/// <param name="numBuffers"> number of buffers to create </param>
		/// <param name="caps"> the required capabilities for creating the buffer strategy;
		/// cannot be <code>null</code> </param>
		/// <exception cref="AWTException"> if the capabilities supplied could not be
		/// supported or met; this may happen, for example, if there is not enough
		/// accelerated memory currently available, or if page flipping is specified
		/// but not possible. </exception>
		/// <exception cref="IllegalArgumentException"> if numBuffers is less than 1, or if
		/// caps is <code>null</code> </exception>
		/// <seealso cref= Window#getBufferStrategy() </seealso>
		/// <seealso cref= Canvas#getBufferStrategy()
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void createBufferStrategy(int numBuffers, BufferCapabilities caps) throws AWTException
		internal virtual void CreateBufferStrategy(int numBuffers, BufferCapabilities caps)
		{
			// Check arguments
			if (numBuffers < 1)
			{
				throw new IllegalArgumentException("Number of buffers must be at least 1");
			}
			if (caps == null)
			{
				throw new IllegalArgumentException("No capabilities specified");
			}
			// Destroy old buffers
			if (BufferStrategy_Renamed != null)
			{
				BufferStrategy_Renamed.Dispose();
			}
			if (numBuffers == 1)
			{
				BufferStrategy_Renamed = new SingleBufferStrategy(this, caps);
			}
			else
			{
				SunGraphicsEnvironment sge = (SunGraphicsEnvironment) GraphicsEnvironment.LocalGraphicsEnvironment;
				if (!caps.PageFlipping && sge.isFlipStrategyPreferred(Peer_Renamed))
				{
					caps = new ProxyCapabilities(this, caps);
				}
				// assert numBuffers > 1;
				if (caps.PageFlipping)
				{
					BufferStrategy_Renamed = new FlipSubRegionBufferStrategy(this, numBuffers, caps);
				}
				else
				{
					BufferStrategy_Renamed = new BltSubRegionBufferStrategy(this, numBuffers, caps);
				}
			}
		}

		/// <summary>
		/// This is a proxy capabilities class used when a FlipBufferStrategy
		/// is created instead of the requested Blit strategy.
		/// </summary>
		/// <seealso cref= sun.java2d.SunGraphicsEnvironment#isFlipStrategyPreferred(ComponentPeer) </seealso>
		private class ProxyCapabilities : ExtendedBufferCapabilities
		{
			private readonly Component OuterInstance;

			internal BufferCapabilities Orig;
			internal ProxyCapabilities(Component outerInstance, BufferCapabilities orig) : base(orig.FrontBufferCapabilities, orig.BackBufferCapabilities, orig.FlipContents == BufferCapabilities.FlipContents.BACKGROUND ? BufferCapabilities.FlipContents.BACKGROUND : BufferCapabilities.FlipContents.COPIED)
			{
				this.OuterInstance = outerInstance;
				this.Orig = orig;
			}
		}

		/// <returns> the buffer strategy used by this component </returns>
		/// <seealso cref= Window#createBufferStrategy </seealso>
		/// <seealso cref= Canvas#createBufferStrategy
		/// @since 1.4 </seealso>
		internal virtual BufferStrategy BufferStrategy
		{
			get
			{
				return BufferStrategy_Renamed;
			}
		}

		/// <returns> the back buffer currently used by this component's
		/// BufferStrategy.  If there is no BufferStrategy or no
		/// back buffer, this method returns null. </returns>
		internal virtual Image BackBuffer
		{
			get
			{
				if (BufferStrategy_Renamed != null)
				{
					if (BufferStrategy_Renamed is BltBufferStrategy)
					{
						BltBufferStrategy bltBS = (BltBufferStrategy)BufferStrategy_Renamed;
						return bltBS.BackBuffer;
					}
					else if (BufferStrategy_Renamed is FlipBufferStrategy)
					{
						FlipBufferStrategy flipBS = (FlipBufferStrategy)BufferStrategy_Renamed;
						return flipBS.BackBuffer;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Inner class for flipping buffers on a component.  That component must
		/// be a <code>Canvas</code> or <code>Window</code>. </summary>
		/// <seealso cref= Canvas </seealso>
		/// <seealso cref= Window </seealso>
		/// <seealso cref= java.awt.image.BufferStrategy
		/// @author Michael Martak
		/// @since 1.4 </seealso>
		protected internal class FlipBufferStrategy : BufferStrategy
		{
			private readonly Component OuterInstance;

			/// <summary>
			/// The number of buffers
			/// </summary>
			protected internal int NumBuffers; // = 0
			/// <summary>
			/// The buffering capabilities
			/// </summary>
			protected internal BufferCapabilities Caps; // = null
			/// <summary>
			/// The drawing buffer
			/// </summary>
			protected internal Image DrawBuffer; // = null
			/// <summary>
			/// The drawing buffer as a volatile image
			/// </summary>
			protected internal VolatileImage DrawVBuffer; // = null
			/// <summary>
			/// Whether or not the drawing buffer has been recently restored from
			/// a lost state.
			/// </summary>
			protected internal bool ValidatedContents; // = false
			/// <summary>
			/// Size of the back buffers.  (Note: these fields were added in 6.0
			/// but kept package-private to avoid exposing them in the spec.
			/// None of these fields/methods really should have been marked
			/// protected when they were introduced in 1.4, but now we just have
			/// to live with that decision.)
			/// </summary>
			internal int Width;
			internal int Height;

			/// <summary>
			/// Creates a new flipping buffer strategy for this component.
			/// The component must be a <code>Canvas</code> or <code>Window</code>. </summary>
			/// <seealso cref= Canvas </seealso>
			/// <seealso cref= Window </seealso>
			/// <param name="numBuffers"> the number of buffers </param>
			/// <param name="caps"> the capabilities of the buffers </param>
			/// <exception cref="AWTException"> if the capabilities supplied could not be
			/// supported or met </exception>
			/// <exception cref="ClassCastException"> if the component is not a canvas or
			/// window. </exception>
			/// <exception cref="IllegalStateException"> if the component has no peer </exception>
			/// <exception cref="IllegalArgumentException"> if {@code numBuffers} is less than two,
			/// or if {@code BufferCapabilities.isPageFlipping} is not
			/// {@code true}. </exception>
			/// <seealso cref= #createBuffers(int, BufferCapabilities) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected FlipBufferStrategy(int numBuffers, BufferCapabilities caps) throws AWTException
			protected internal FlipBufferStrategy(Component outerInstance, int numBuffers, BufferCapabilities caps)
			{
				this.OuterInstance = outerInstance;
				if (!(outerInstance is Window) && !(outerInstance is Canvas))
				{
					throw new ClassCastException("Component must be a Canvas or Window");
				}
				this.NumBuffers = numBuffers;
				this.Caps = caps;
				CreateBuffers(numBuffers, caps);
			}

			/// <summary>
			/// Creates one or more complex, flipping buffers with the given
			/// capabilities. </summary>
			/// <param name="numBuffers"> number of buffers to create; must be greater than
			/// one </param>
			/// <param name="caps"> the capabilities of the buffers.
			/// <code>BufferCapabilities.isPageFlipping</code> must be
			/// <code>true</code>. </param>
			/// <exception cref="AWTException"> if the capabilities supplied could not be
			/// supported or met </exception>
			/// <exception cref="IllegalStateException"> if the component has no peer </exception>
			/// <exception cref="IllegalArgumentException"> if numBuffers is less than two,
			/// or if <code>BufferCapabilities.isPageFlipping</code> is not
			/// <code>true</code>. </exception>
			/// <seealso cref= java.awt.BufferCapabilities#isPageFlipping() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createBuffers(int numBuffers, BufferCapabilities caps) throws AWTException
			protected internal virtual void CreateBuffers(int numBuffers, BufferCapabilities caps)
			{
				if (numBuffers < 2)
				{
					throw new IllegalArgumentException("Number of buffers cannot be less than two");
				}
				else if (outerInstance.Peer_Renamed == null)
				{
					throw new IllegalStateException("Component must have a valid peer");
				}
				else if (caps == null || !caps.PageFlipping)
				{
					throw new IllegalArgumentException("Page flipping capabilities must be specified");
				}

				// save the current bounds
				Width = outerInstance.Width;
				Height = outerInstance.Height;

				if (DrawBuffer != null)
				{
					// dispose the existing backbuffers
					DrawBuffer = null;
					DrawVBuffer = null;
					DestroyBuffers();
					// ... then recreate the backbuffers
				}

				if (caps is ExtendedBufferCapabilities)
				{
					ExtendedBufferCapabilities ebc = (ExtendedBufferCapabilities)caps;
					if (ebc.VSync == VSYNC_ON)
					{
						// if this buffer strategy is not allowed to be v-synced,
						// change the caps that we pass to the peer but keep on
						// trying to create v-synced buffers;
						// do not throw IAE here in case it is disallowed, see
						// ExtendedBufferCapabilities for more info
						if (!VSyncedBSManager.vsyncAllowed(this))
						{
							caps = ebc.derive(VSYNC_DEFAULT);
						}
					}
				}

				outerInstance.Peer_Renamed.CreateBuffers(numBuffers, caps);
				UpdateInternalBuffers();
			}

			/// <summary>
			/// Updates internal buffers (both volatile and non-volatile)
			/// by requesting the back-buffer from the peer.
			/// </summary>
			internal virtual void UpdateInternalBuffers()
			{
				// get the images associated with the draw buffer
				DrawBuffer = BackBuffer;
				if (DrawBuffer is VolatileImage)
				{
					DrawVBuffer = (VolatileImage)DrawBuffer;
				}
				else
				{
					DrawVBuffer = null;
				}
			}

			/// <returns> direct access to the back buffer, as an image. </returns>
			/// <exception cref="IllegalStateException"> if the buffers have not yet
			/// been created </exception>
			protected internal virtual Image BackBuffer
			{
				get
				{
					if (outerInstance.Peer_Renamed != null)
					{
						return outerInstance.Peer_Renamed.BackBuffer;
					}
					else
					{
						throw new IllegalStateException("Component must have a valid peer");
					}
				}
			}

			/// <summary>
			/// Flipping moves the contents of the back buffer to the front buffer,
			/// either by copying or by moving the video pointer. </summary>
			/// <param name="flipAction"> an integer value describing the flipping action
			/// for the contents of the back buffer.  This should be one of the
			/// values of the <code>BufferCapabilities.FlipContents</code>
			/// property. </param>
			/// <exception cref="IllegalStateException"> if the buffers have not yet
			/// been created </exception>
			/// <seealso cref= java.awt.BufferCapabilities#getFlipContents() </seealso>
			protected internal virtual void Flip(BufferCapabilities.FlipContents flipAction)
			{
				if (outerInstance.Peer_Renamed != null)
				{
					Image backBuffer = BackBuffer;
					if (backBuffer != null)
					{
						outerInstance.Peer_Renamed.Flip(0, 0, backBuffer.GetWidth(null), backBuffer.GetHeight(null), flipAction);
					}
				}
				else
				{
					throw new IllegalStateException("Component must have a valid peer");
				}
			}

			internal virtual void FlipSubRegion(int x1, int y1, int x2, int y2, BufferCapabilities.FlipContents flipAction)
			{
				if (outerInstance.Peer_Renamed != null)
				{
					outerInstance.Peer_Renamed.Flip(x1, y1, x2, y2, flipAction);
				}
				else
				{
					throw new IllegalStateException("Component must have a valid peer");
				}
			}

			/// <summary>
			/// Destroys the buffers created through this object
			/// </summary>
			protected internal virtual void DestroyBuffers()
			{
				VSyncedBSManager.releaseVsync(this);
				if (outerInstance.Peer_Renamed != null)
				{
					outerInstance.Peer_Renamed.DestroyBuffers();
				}
				else
				{
					throw new IllegalStateException("Component must have a valid peer");
				}
			}

			/// <returns> the buffering capabilities of this strategy </returns>
			public override BufferCapabilities Capabilities
			{
				get
				{
					if (Caps is ProxyCapabilities)
					{
						return ((ProxyCapabilities)Caps).Orig;
					}
					else
					{
						return Caps;
					}
				}
			}

			/// <returns> the graphics on the drawing buffer.  This method may not
			/// be synchronized for performance reasons; use of this method by multiple
			/// threads should be handled at the application level.  Disposal of the
			/// graphics object must be handled by the application. </returns>
			public override Graphics DrawGraphics
			{
				get
				{
					Revalidate();
					return DrawBuffer.Graphics;
				}
			}

			/// <summary>
			/// Restore the drawing buffer if it has been lost
			/// </summary>
			protected internal virtual void Revalidate()
			{
				Revalidate(true);
			}

			internal virtual void Revalidate(bool checkSize)
			{
				ValidatedContents = false;

				if (checkSize && (outerInstance.Width != Width || outerInstance.Height != Height))
				{
					// component has been resized; recreate the backbuffers
					try
					{
						CreateBuffers(NumBuffers, Caps);
					}
					catch (AWTException)
					{
						// shouldn't be possible
					}
					ValidatedContents = true;
				}

				// get the buffers from the peer every time since they
				// might have been replaced in response to a display change event
				UpdateInternalBuffers();

				// now validate the backbuffer
				if (DrawVBuffer != null)
				{
					GraphicsConfiguration gc = outerInstance.GraphicsConfiguration_NoClientCode;
					int returnCode = DrawVBuffer.Validate(gc);
					if (returnCode == VolatileImage.IMAGE_INCOMPATIBLE)
					{
						try
						{
							CreateBuffers(NumBuffers, Caps);
						}
						catch (AWTException)
						{
							// shouldn't be possible
						}
						if (DrawVBuffer != null)
						{
							// backbuffers were recreated, so validate again
							DrawVBuffer.Validate(gc);
						}
						ValidatedContents = true;
					}
					else if (returnCode == VolatileImage.IMAGE_RESTORED)
					{
						ValidatedContents = true;
					}
				}
			}

			/// <returns> whether the drawing buffer was lost since the last call to
			/// <code>getDrawGraphics</code> </returns>
			public override bool ContentsLost()
			{
				if (DrawVBuffer == null)
				{
					return false;
				}
				return DrawVBuffer.ContentsLost();
			}

			/// <returns> whether the drawing buffer was recently restored from a lost
			/// state and reinitialized to the default background color (white) </returns>
			public override bool ContentsRestored()
			{
				return ValidatedContents;
			}

			/// <summary>
			/// Makes the next available buffer visible by either blitting or
			/// flipping.
			/// </summary>
			public override void Show()
			{
				Flip(Caps.FlipContents);
			}

			/// <summary>
			/// Makes specified region of the the next available buffer visible
			/// by either blitting or flipping.
			/// </summary>
			internal virtual void ShowSubRegion(int x1, int y1, int x2, int y2)
			{
				FlipSubRegion(x1, y1, x2, y2, Caps.FlipContents);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public override void Dispose()
			{
				if (OuterInstance.BufferStrategy_Renamed == this)
				{
					OuterInstance.BufferStrategy_Renamed = null;
					if (outerInstance.Peer_Renamed != null)
					{
						DestroyBuffers();
					}
				}
			}

		} // Inner class FlipBufferStrategy

		/// <summary>
		/// Inner class for blitting offscreen surfaces to a component.
		/// 
		/// @author Michael Martak
		/// @since 1.4
		/// </summary>
		protected internal class BltBufferStrategy : BufferStrategy
		{
			private readonly Component OuterInstance;


			/// <summary>
			/// The buffering capabilities
			/// </summary>
			protected internal BufferCapabilities Caps; // = null
			/// <summary>
			/// The back buffers
			/// </summary>
			protected internal VolatileImage[] BackBuffers; // = null
			/// <summary>
			/// Whether or not the drawing buffer has been recently restored from
			/// a lost state.
			/// </summary>
			protected internal bool ValidatedContents; // = false
			/// <summary>
			/// Size of the back buffers
			/// </summary>
			protected internal int Width;
			protected internal int Height;

			/// <summary>
			/// Insets for the hosting Component.  The size of the back buffer
			/// is constrained by these.
			/// </summary>
			internal Insets Insets;

			/// <summary>
			/// Creates a new blt buffer strategy around a component </summary>
			/// <param name="numBuffers"> number of buffers to create, including the
			/// front buffer </param>
			/// <param name="caps"> the capabilities of the buffers </param>
			protected internal BltBufferStrategy(Component outerInstance, int numBuffers, BufferCapabilities caps)
			{
				this.OuterInstance = outerInstance;
				this.Caps = caps;
				CreateBackBuffers(numBuffers - 1);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.6
			/// </summary>
			public override void Dispose()
			{
				if (BackBuffers != null)
				{
					for (int counter = BackBuffers.Length - 1; counter >= 0; counter--)
					{
						if (BackBuffers[counter] != null)
						{
							BackBuffers[counter].Flush();
							BackBuffers[counter] = null;
						}
					}
				}
				if (OuterInstance.BufferStrategy_Renamed == this)
				{
					OuterInstance.BufferStrategy_Renamed = null;
				}
			}

			/// <summary>
			/// Creates the back buffers
			/// </summary>
			protected internal virtual void CreateBackBuffers(int numBuffers)
			{
				if (numBuffers == 0)
				{
					BackBuffers = null;
				}
				else
				{
					// save the current bounds
					Width = outerInstance.Width;
					Height = outerInstance.Height;
					Insets = outerInstance.Insets_NoClientCode;
					int iWidth = Width - Insets.Left - Insets.Right;
					int iHeight = Height - Insets.Top - Insets.Bottom;

					// It is possible for the component's width and/or height
					// to be 0 here.  Force the size of the backbuffers to
					// be > 0 so that creating the image won't fail.
					iWidth = System.Math.Max(1, iWidth);
					iHeight = System.Math.Max(1, iHeight);
					if (BackBuffers == null)
					{
						BackBuffers = new VolatileImage[numBuffers];
					}
					else
					{
						// flush any existing backbuffers
						for (int i = 0; i < numBuffers; i++)
						{
							if (BackBuffers[i] != null)
							{
								BackBuffers[i].Flush();
								BackBuffers[i] = null;
							}
						}
					}

					// create the backbuffers
					for (int i = 0; i < numBuffers; i++)
					{
						BackBuffers[i] = outerInstance.CreateVolatileImage(iWidth, iHeight);
					}
				}
			}

			/// <returns> the buffering capabilities of this strategy </returns>
			public override BufferCapabilities Capabilities
			{
				get
				{
					return Caps;
				}
			}

			/// <returns> the draw graphics </returns>
			public override Graphics DrawGraphics
			{
				get
				{
					Revalidate();
					Image backBuffer = BackBuffer;
					if (backBuffer == null)
					{
						return outerInstance.Graphics;
					}
					SunGraphics2D g = (SunGraphics2D)backBuffer.Graphics;
					g.constrain(-Insets.Left, -Insets.Top, backBuffer.GetWidth(null) + Insets.Left, backBuffer.GetHeight(null) + Insets.Top);
					return g;
				}
			}

			/// <returns> direct access to the back buffer, as an image.
			/// If there is no back buffer, returns null. </returns>
			internal virtual Image BackBuffer
			{
				get
				{
					if (BackBuffers != null)
					{
						return BackBuffers[BackBuffers.Length - 1];
					}
					else
					{
						return null;
					}
				}
			}

			/// <summary>
			/// Makes the next available buffer visible.
			/// </summary>
			public override void Show()
			{
				ShowSubRegion(Insets.Left, Insets.Top, Width - Insets.Right, Height - Insets.Bottom);
			}

			/// <summary>
			/// Package-private method to present a specific rectangular area
			/// of this buffer.  This class currently shows only the entire
			/// buffer, by calling showSubRegion() with the full dimensions of
			/// the buffer.  Subclasses (e.g., BltSubRegionBufferStrategy
			/// and FlipSubRegionBufferStrategy) may have region-specific show
			/// methods that call this method with actual sub regions of the
			/// buffer.
			/// </summary>
			internal virtual void ShowSubRegion(int x1, int y1, int x2, int y2)
			{
				if (BackBuffers == null)
				{
					return;
				}
				// Adjust location to be relative to client area.
				x1 -= Insets.Left;
				x2 -= Insets.Left;
				y1 -= Insets.Top;
				y2 -= Insets.Top;
				Graphics g = outerInstance.Graphics_NoClientCode;
				if (g == null)
				{
					// Not showing, bail
					return;
				}
				try
				{
					// First image copy is in terms of Frame's coordinates, need
					// to translate to client area.
					g.Translate(Insets.Left, Insets.Top);
					for (int i = 0; i < BackBuffers.Length; i++)
					{
						g.DrawImage(BackBuffers[i], x1, y1, x2, y2, x1, y1, x2, y2, null);
						g.Dispose();
						g = null;
						g = BackBuffers[i].Graphics;
					}
				}
				finally
				{
					if (g != null)
					{
						g.Dispose();
					}
				}
			}

			/// <summary>
			/// Restore the drawing buffer if it has been lost
			/// </summary>
			protected internal virtual void Revalidate()
			{
				Revalidate(true);
			}

			internal virtual void Revalidate(bool checkSize)
			{
				ValidatedContents = false;

				if (BackBuffers == null)
				{
					return;
				}

				if (checkSize)
				{
					Insets insets = outerInstance.Insets_NoClientCode;
					if (outerInstance.Width != Width || outerInstance.Height != Height || !insets.Equals(this.Insets))
					{
						// component has been resized; recreate the backbuffers
						CreateBackBuffers(BackBuffers.Length);
						ValidatedContents = true;
					}
				}

				// now validate the backbuffer
				GraphicsConfiguration gc = outerInstance.GraphicsConfiguration_NoClientCode;
				int returnCode = BackBuffers[BackBuffers.Length - 1].Validate(gc);
				if (returnCode == VolatileImage.IMAGE_INCOMPATIBLE)
				{
					if (checkSize)
					{
						CreateBackBuffers(BackBuffers.Length);
						// backbuffers were recreated, so validate again
						BackBuffers[BackBuffers.Length - 1].Validate(gc);
					}
					// else case means we're called from Swing on the toolkit
					// thread, don't recreate buffers as that'll deadlock
					// (creating VolatileImages invokes getting GraphicsConfig
					// which grabs treelock).
					ValidatedContents = true;
				}
				else if (returnCode == VolatileImage.IMAGE_RESTORED)
				{
					ValidatedContents = true;
				}
			}

			/// <returns> whether the drawing buffer was lost since the last call to
			/// <code>getDrawGraphics</code> </returns>
			public override bool ContentsLost()
			{
				if (BackBuffers == null)
				{
					return false;
				}
				else
				{
					return BackBuffers[BackBuffers.Length - 1].ContentsLost();
				}
			}

			/// <returns> whether the drawing buffer was recently restored from a lost
			/// state and reinitialized to the default background color (white) </returns>
			public override bool ContentsRestored()
			{
				return ValidatedContents;
			}
		} // Inner class BltBufferStrategy

		/// <summary>
		/// Private class to perform sub-region flipping.
		/// </summary>
		private class FlipSubRegionBufferStrategy : FlipBufferStrategy, SubRegionShowable
		{
			private readonly Component OuterInstance;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected FlipSubRegionBufferStrategy(int numBuffers, BufferCapabilities caps) throws AWTException
			protected internal FlipSubRegionBufferStrategy(Component outerInstance, int numBuffers, BufferCapabilities caps) : base(outerInstance, numBuffers, caps)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual void Show(int x1, int y1, int x2, int y2)
			{
				ShowSubRegion(x1, y1, x2, y2);
			}

			// This is invoked by Swing on the toolkit thread.
			public virtual bool ShowIfNotLost(int x1, int y1, int x2, int y2)
			{
				if (!ContentsLost())
				{
					ShowSubRegion(x1, y1, x2, y2);
					return !ContentsLost();
				}
				return false;
			}
		}

		/// <summary>
		/// Private class to perform sub-region blitting.  Swing will use
		/// this subclass via the SubRegionShowable interface in order to
		/// copy only the area changed during a repaint.
		/// See javax.swing.BufferStrategyPaintManager.
		/// </summary>
		private class BltSubRegionBufferStrategy : BltBufferStrategy, SubRegionShowable
		{
			private readonly Component OuterInstance;


			protected internal BltSubRegionBufferStrategy(Component outerInstance, int numBuffers, BufferCapabilities caps) : base(outerInstance, numBuffers, caps)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual void Show(int x1, int y1, int x2, int y2)
			{
				ShowSubRegion(x1, y1, x2, y2);
			}

			// This method is called by Swing on the toolkit thread.
			public virtual bool ShowIfNotLost(int x1, int y1, int x2, int y2)
			{
				if (!ContentsLost())
				{
					ShowSubRegion(x1, y1, x2, y2);
					return !ContentsLost();
				}
				return false;
			}
		}

		/// <summary>
		/// Inner class for flipping buffers on a component.  That component must
		/// be a <code>Canvas</code> or <code>Window</code>. </summary>
		/// <seealso cref= Canvas </seealso>
		/// <seealso cref= Window </seealso>
		/// <seealso cref= java.awt.image.BufferStrategy
		/// @author Michael Martak
		/// @since 1.4 </seealso>
		private class SingleBufferStrategy : BufferStrategy
		{
			private readonly Component OuterInstance;


			internal BufferCapabilities Caps;

			public SingleBufferStrategy(Component outerInstance, BufferCapabilities caps)
			{
				this.OuterInstance = outerInstance;
				this.Caps = caps;
			}
			public override BufferCapabilities Capabilities
			{
				get
				{
					return Caps;
				}
			}
			public override Graphics DrawGraphics
			{
				get
				{
					return outerInstance.Graphics;
				}
			}
			public override bool ContentsLost()
			{
				return false;
			}
			public override bool ContentsRestored()
			{
				return false;
			}
			public override void Show()
			{
				// Do nothing
			}
		} // Inner class SingleBufferStrategy

		/// <summary>
		/// Sets whether or not paint messages received from the operating system
		/// should be ignored.  This does not affect paint events generated in
		/// software by the AWT, unless they are an immediate response to an
		/// OS-level paint message.
		/// <para>
		/// This is useful, for example, if running under full-screen mode and
		/// better performance is desired, or if page-flipping is used as the
		/// buffer strategy.
		/// 
		/// @since 1.4
		/// </para>
		/// </summary>
		/// <seealso cref= #getIgnoreRepaint </seealso>
		/// <seealso cref= Canvas#createBufferStrategy </seealso>
		/// <seealso cref= Window#createBufferStrategy </seealso>
		/// <seealso cref= java.awt.image.BufferStrategy </seealso>
		/// <seealso cref= GraphicsDevice#setFullScreenWindow </seealso>
		public virtual bool IgnoreRepaint
		{
			set
			{
				this.IgnoreRepaint_Renamed = value;
			}
			get
			{
				return IgnoreRepaint_Renamed;
			}
		}


		/// <summary>
		/// Checks whether this component "contains" the specified point,
		/// where <code>x</code> and <code>y</code> are defined to be
		/// relative to the coordinate system of this component. </summary>
		/// <param name="x">   the <i>x</i> coordinate of the point </param>
		/// <param name="y">   the <i>y</i> coordinate of the point </param>
		/// <seealso cref=       #getComponentAt(int, int)
		/// @since     JDK1.1 </seealso>
		public virtual bool Contains(int x, int y)
		{
			return Inside(x, y);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by contains(int, int). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool Inside(int x, int y)
		{
			return (x >= 0) && (x < Width_Renamed) && (y >= 0) && (y < Height_Renamed);
		}

		/// <summary>
		/// Checks whether this component "contains" the specified point,
		/// where the point's <i>x</i> and <i>y</i> coordinates are defined
		/// to be relative to the coordinate system of this component. </summary>
		/// <param name="p">     the point </param>
		/// <exception cref="NullPointerException"> if {@code p} is {@code null} </exception>
		/// <seealso cref=       #getComponentAt(Point)
		/// @since     JDK1.1 </seealso>
		public virtual bool Contains(Point p)
		{
			return Contains(p.x, p.y);
		}

		/// <summary>
		/// Determines if this component or one of its immediate
		/// subcomponents contains the (<i>x</i>,&nbsp;<i>y</i>) location,
		/// and if so, returns the containing component. This method only
		/// looks one level deep. If the point (<i>x</i>,&nbsp;<i>y</i>) is
		/// inside a subcomponent that itself has subcomponents, it does not
		/// go looking down the subcomponent tree.
		/// <para>
		/// The <code>locate</code> method of <code>Component</code> simply
		/// returns the component itself if the (<i>x</i>,&nbsp;<i>y</i>)
		/// coordinate location is inside its bounding box, and <code>null</code>
		/// otherwise.
		/// </para>
		/// </summary>
		/// <param name="x">   the <i>x</i> coordinate </param>
		/// <param name="y">   the <i>y</i> coordinate </param>
		/// <returns>    the component or subcomponent that contains the
		///                (<i>x</i>,&nbsp;<i>y</i>) location;
		///                <code>null</code> if the location
		///                is outside this component </returns>
		/// <seealso cref=       #contains(int, int)
		/// @since     JDK1.0 </seealso>
		public virtual Component GetComponentAt(int x, int y)
		{
			return Locate(x, y);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by getComponentAt(int, int). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Component Locate(int x, int y)
		{
			return Contains(x, y) ? this : null;
		}

		/// <summary>
		/// Returns the component or subcomponent that contains the
		/// specified point. </summary>
		/// <param name="p">   the point </param>
		/// <seealso cref=       java.awt.Component#contains
		/// @since     JDK1.1 </seealso>
		public virtual Component GetComponentAt(Point p)
		{
			return GetComponentAt(p.x, p.y);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>dispatchEvent(AWTEvent e)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void DeliverEvent(Event e)
		{
			PostEvent(e);
		}

		/// <summary>
		/// Dispatches an event to this component or one of its sub components.
		/// Calls <code>processEvent</code> before returning for 1.1-style
		/// events which have been enabled for the <code>Component</code>. </summary>
		/// <param name="e"> the event </param>
		public void DispatchEvent(AWTEvent e)
		{
			DispatchEventImpl(e);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") void dispatchEventImpl(AWTEvent e)
		internal virtual void DispatchEventImpl(AWTEvent e)
		{
			int id = e.ID;

			// Check that this component belongs to this app-context
			AppContext compContext = AppContext;
			if (compContext != null && !compContext.Equals(AppContext.AppContext))
			{
				if (EventLog.isLoggable(PlatformLogger.Level.FINE))
				{
					EventLog.fine("Event " + e + " is being dispatched on the wrong AppContext");
				}
			}

			if (EventLog.isLoggable(PlatformLogger.Level.FINEST))
			{
				EventLog.finest("{0}", e);
			}

			/*
			 * 0. Set timestamp and modifiers of current event.
			 */
			if (!(e is KeyEvent))
			{
				// Timestamp of a key event is set later in DKFM.preDispatchKeyEvent(KeyEvent).
				EventQueue.CurrentEventAndMostRecentTime = e;
			}

			/*
			 * 1. Pre-dispatchers. Do any necessary retargeting/reordering here
			 *    before we notify AWTEventListeners.
			 */

			if (e is SunDropTargetEvent)
			{
				((SunDropTargetEvent)e).dispatch();
				return;
			}

			if (!e.FocusManagerIsDispatching)
			{
				// Invoke the private focus retargeting method which provides
				// lightweight Component support
				if (e.IsPosted)
				{
					e = KeyboardFocusManager.RetargetFocusEvent(e);
					e.IsPosted = true;
				}

				// Now, with the event properly targeted to a lightweight
				// descendant if necessary, invoke the public focus retargeting
				// and dispatching function
				if (KeyboardFocusManager.CurrentKeyboardFocusManager.DispatchEvent(e))
				{
					return;
				}
			}
			if ((e is FocusEvent) && FocusLog.isLoggable(PlatformLogger.Level.FINEST))
			{
				FocusLog.finest("" + e);
			}
			// MouseWheel may need to be retargeted here so that
			// AWTEventListener sees the event go to the correct
			// Component.  If the MouseWheelEvent needs to go to an ancestor,
			// the event is dispatched to the ancestor, and dispatching here
			// stops.
			if (id == MouseEvent.MOUSE_WHEEL && (!EventTypeEnabled(id)) && (Peer_Renamed != null && !Peer_Renamed.HandlesWheelScrolling()) && (DispatchMouseWheelToAncestor((MouseWheelEvent)e)))
			{
				return;
			}

			/*
			 * 2. Allow the Toolkit to pass this to AWTEventListeners.
			 */
			Toolkit toolkit = Toolkit.DefaultToolkit;
			toolkit.NotifyAWTEventListeners(e);


			/*
			 * 3. If no one has consumed a key event, allow the
			 *    KeyboardFocusManager to process it.
			 */
			if (!e.Consumed)
			{
				if (e is java.awt.@event.KeyEvent)
				{
					KeyboardFocusManager.CurrentKeyboardFocusManager.ProcessKeyEvent(this, (KeyEvent)e);
					if (e.Consumed)
					{
						return;
					}
				}
			}

			/*
			 * 4. Allow input methods to process the event
			 */
			if (AreInputMethodsEnabled())
			{
				// We need to pass on InputMethodEvents since some host
				// input method adapters send them through the Java
				// event queue instead of directly to the component,
				// and the input context also handles the Java composition window
				if (((e is InputMethodEvent) && !(this is CompositionArea)) || (e is InputEvent) || (e is FocusEvent))
				   // Otherwise, we only pass on input and focus events, because
				   // a) input methods shouldn't know about semantic or component-level events
				   // b) passing on the events takes time
				   // c) isConsumed() is always true for semantic events.
				{
					InputContext inputContext = InputContext;


					if (inputContext != null)
					{
						inputContext.DispatchEvent(e);
						if (e.Consumed)
						{
							if ((e is FocusEvent) && FocusLog.isLoggable(PlatformLogger.Level.FINEST))
							{
								FocusLog.finest("3579: Skipping " + e);
							}
							return;
						}
					}
				}
			}
			else
			{
				// When non-clients get focus, we need to explicitly disable the native
				// input method. The native input method is actually not disabled when
				// the active/passive/peered clients loose focus.
				if (id == FocusEvent.FOCUS_GAINED)
				{
					InputContext inputContext = InputContext;
					if (inputContext != null && inputContext is sun.awt.im.InputContext)
					{
						((sun.awt.im.InputContext)inputContext).disableNativeIM();
					}
				}
			}


			/*
			 * 5. Pre-process any special events before delivery
			 */
			switch (id)
			{
				// Handling of the PAINT and UPDATE events is now done in the
				// peer's handleEvent() method so the background can be cleared
				// selectively for non-native components on Windows only.
				// - Fred.Ecks@Eng.sun.com, 5-8-98

			  case KeyEvent.KEY_PRESSED:
			  case KeyEvent.KEY_RELEASED:
				  Container p = (Container)((this is Container) ? this : Parent_Renamed);
				  if (p != null)
				  {
					  p.PreProcessKeyEvent((KeyEvent)e);
					  if (e.Consumed)
					  {
							if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
							{
								FocusLog.finest("Pre-process consumed event");
							}
						  return;
					  }
				  }
				  break;

			  case WindowEvent.WINDOW_CLOSING:
				  if (toolkit is WindowClosingListener)
				  {
					  WindowClosingException = ((WindowClosingListener) toolkit).windowClosingNotify((WindowEvent)e);
					  if (CheckWindowClosingException())
					  {
						  return;
					  }
				  }
				  break;

			  default:
				  break;
			}

			/*
			 * 6. Deliver event for normal processing
			 */
			if (NewEventsOnly)
			{
				// Filtering needs to really be moved to happen at a lower
				// level in order to get maximum performance gain;  it is
				// here temporarily to ensure the API spec is honored.
				//
				if (EventEnabled(e))
				{
					ProcessEvent(e);
				}
			}
			else if (id == MouseEvent.MOUSE_WHEEL)
			{
				// newEventsOnly will be false for a listenerless ScrollPane, but
				// MouseWheelEvents still need to be dispatched to it so scrolling
				// can be done.
				AutoProcessMouseWheel((MouseWheelEvent)e);
			}
			else if (!(e is MouseEvent && !PostsOldMouseEvents()))
			{
				//
				// backward compatibility
				//
				Event olde = e.ConvertToOld();
				if (olde != null)
				{
					int key = olde.Key;
					int modifiers = olde.Modifiers;

					PostEvent(olde);
					if (olde.Consumed)
					{
						e.Consume();
					}
					// if target changed key or modifier values, copy them
					// back to original event
					//
					switch (olde.Id)
					{
					  case Event.KEY_PRESS:
					  case Event.KEY_RELEASE:
					  case Event.KEY_ACTION:
					  case Event.KEY_ACTION_RELEASE:
						  if (olde.Key != key)
						  {
							  ((KeyEvent)e).KeyChar = olde.KeyEventChar;
						  }
						  if (olde.Modifiers != modifiers)
						  {
							  ((KeyEvent)e).Modifiers = olde.Modifiers;
						  }
						  break;
					  default:
						  break;
					}
				}
			}

			/*
			 * 8. Special handling for 4061116 : Hook for browser to close modal
			 *    dialogs.
			 */
			if (id == WindowEvent.WINDOW_CLOSING && !e.Consumed)
			{
				if (toolkit is WindowClosingListener)
				{
					WindowClosingException = ((WindowClosingListener)toolkit).windowClosingDelivered((WindowEvent)e);
					if (CheckWindowClosingException())
					{
						return;
					}
				}
			}

			/*
			 * 9. Allow the peer to process the event.
			 * Except KeyEvents, they will be processed by peer after
			 * all KeyEventPostProcessors
			 * (see DefaultKeyboardFocusManager.dispatchKeyEvent())
			 */
			if (!(e is KeyEvent))
			{
				ComponentPeer tpeer = Peer_Renamed;
				if (e is FocusEvent && (tpeer == null || tpeer is LightweightPeer))
				{
					// if focus owner is lightweight then its native container
					// processes event
					Component source = (Component)e.Source;
					if (source != null)
					{
						Container target = source.NativeContainer;
						if (target != null)
						{
							tpeer = target.Peer;
						}
					}
				}
				if (tpeer != null)
				{
					tpeer.HandleEvent(e);
				}
			}
		} // dispatchEventImpl()

		/*
		 * If newEventsOnly is false, method is called so that ScrollPane can
		 * override it and handle common-case mouse wheel scrolling.  NOP
		 * for Component.
		 */
		internal virtual void AutoProcessMouseWheel(MouseWheelEvent e)
		{
		}

		/*
		 * Dispatch given MouseWheelEvent to the first ancestor for which
		 * MouseWheelEvents are enabled.
		 *
		 * Returns whether or not event was dispatched to an ancestor
		 */
		internal virtual bool DispatchMouseWheelToAncestor(MouseWheelEvent e)
		{
			int newX, newY;
			newX = e.X + X; // Coordinates take into account at least
			newY = e.Y + Y; // the cursor's position relative to this
									  // Component (e.getX()), and this Component's
									  // position relative to its parent.
			MouseWheelEvent newMWE;

			if (EventLog.isLoggable(PlatformLogger.Level.FINEST))
			{
				EventLog.finest("dispatchMouseWheelToAncestor");
				EventLog.finest("orig event src is of " + e.Source.GetType());
			}

			/* parent field for Window refers to the owning Window.
			 * MouseWheelEvents should NOT be propagated into owning Windows
			 */
			lock (TreeLock)
			{
				Container anc = Parent;
				while (anc != null && !anc.EventEnabled(e))
				{
					// fix coordinates to be relative to new event source
					newX += anc.X;
					newY += anc.Y;

					if (!(anc is Window))
					{
						anc = anc.Parent;
					}
					else
					{
						break;
					}
				}

				if (EventLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					EventLog.finest("new event src is " + anc.GetType());
				}

				if (anc != null && anc.EventEnabled(e))
				{
					// Change event to be from new source, with new x,y
					// For now, just create a new event - yucky

					newMWE = new MouseWheelEvent(anc, e.ID, e.When, e.Modifiers, newX, newY, e.XOnScreen, e.YOnScreen, e.ClickCount, e.PopupTrigger, e.ScrollType, e.ScrollAmount, e.WheelRotation, e.PreciseWheelRotation); // y relative to new source -  x relative to new source -  new source
					((AWTEvent)e).CopyPrivateDataInto(newMWE);
					// When dispatching a wheel event to
					// ancestor, there is no need trying to find descendant
					// lightweights to dispatch event to.
					// If we dispatch the event to toplevel ancestor,
					// this could encolse the loop: 6480024.
					anc.DispatchEventToSelf(newMWE);
					if (newMWE.Consumed)
					{
						e.Consume();
					}
					return true;
				}
			}
			return false;
		}

		internal virtual bool CheckWindowClosingException()
		{
			if (WindowClosingException != null)
			{
				if (this is Dialog)
				{
					((Dialog)this).InterruptBlocking();
				}
				else
				{
					WindowClosingException.FillInStackTrace();
					Console.WriteLine(WindowClosingException.ToString());
					Console.Write(WindowClosingException.StackTrace);
					WindowClosingException = null;
				}
				return true;
			}
			return false;
		}

		internal virtual bool AreInputMethodsEnabled()
		{
			// in 1.2, we assume input method support is required for all
			// components that handle key events, but components can turn off
			// input methods by calling enableInputMethods(false).
			return ((EventMask & AWTEvent.INPUT_METHODS_ENABLED_MASK) != 0) && ((EventMask & AWTEvent.KEY_EVENT_MASK) != 0 || KeyListener != null);
		}

		// REMIND: remove when filtering is handled at lower level
		internal virtual bool EventEnabled(AWTEvent e)
		{
			return EventTypeEnabled(e.Id);
		}

		internal virtual bool EventTypeEnabled(int type)
		{
			switch (type)
			{
			  case ComponentEvent.COMPONENT_MOVED:
			  case ComponentEvent.COMPONENT_RESIZED:
			  case ComponentEvent.COMPONENT_SHOWN:
			  case ComponentEvent.COMPONENT_HIDDEN:
				  if ((EventMask & AWTEvent.COMPONENT_EVENT_MASK) != 0 || ComponentListener != null)
				  {
					  return true;
				  }
				  break;
			  case FocusEvent.FOCUS_GAINED:
			  case FocusEvent.FOCUS_LOST:
				  if ((EventMask & AWTEvent.FOCUS_EVENT_MASK) != 0 || FocusListener != null)
				  {
					  return true;
				  }
				  break;
			  case KeyEvent.KEY_PRESSED:
			  case KeyEvent.KEY_RELEASED:
			  case KeyEvent.KEY_TYPED:
				  if ((EventMask & AWTEvent.KEY_EVENT_MASK) != 0 || KeyListener != null)
				  {
					  return true;
				  }
				  break;
			  case MouseEvent.MOUSE_PRESSED:
			  case MouseEvent.MOUSE_RELEASED:
			  case MouseEvent.MOUSE_ENTERED:
			  case MouseEvent.MOUSE_EXITED:
			  case MouseEvent.MOUSE_CLICKED:
				  if ((EventMask & AWTEvent.MOUSE_EVENT_MASK) != 0 || MouseListener != null)
				  {
					  return true;
				  }
				  break;
			  case MouseEvent.MOUSE_MOVED:
			  case MouseEvent.MOUSE_DRAGGED:
				  if ((EventMask & AWTEvent.MOUSE_MOTION_EVENT_MASK) != 0 || MouseMotionListener != null)
				  {
					  return true;
				  }
				  break;
			  case MouseEvent.MOUSE_WHEEL:
				  if ((EventMask & AWTEvent.MOUSE_WHEEL_EVENT_MASK) != 0 || MouseWheelListener != null)
				  {
					  return true;
				  }
				  break;
			  case InputMethodEvent.INPUT_METHOD_TEXT_CHANGED:
			  case InputMethodEvent.CARET_POSITION_CHANGED:
				  if ((EventMask & AWTEvent.INPUT_METHOD_EVENT_MASK) != 0 || InputMethodListener != null)
				  {
					  return true;
				  }
				  break;
			  case HierarchyEvent.HIERARCHY_CHANGED:
				  if ((EventMask & AWTEvent.HIERARCHY_EVENT_MASK) != 0 || HierarchyListener != null)
				  {
					  return true;
				  }
				  break;
			  case HierarchyEvent.ANCESTOR_MOVED:
			  case HierarchyEvent.ANCESTOR_RESIZED:
				  if ((EventMask & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) != 0 || HierarchyBoundsListener != null)
				  {
					  return true;
				  }
				  break;
			  case ActionEvent.ACTION_PERFORMED:
				  if ((EventMask & AWTEvent.ACTION_EVENT_MASK) != 0)
				  {
					  return true;
				  }
				  break;
			  case TextEvent.TEXT_VALUE_CHANGED:
				  if ((EventMask & AWTEvent.TEXT_EVENT_MASK) != 0)
				  {
					  return true;
				  }
				  break;
			  case ItemEvent.ITEM_STATE_CHANGED:
				  if ((EventMask & AWTEvent.ITEM_EVENT_MASK) != 0)
				  {
					  return true;
				  }
				  break;
			  case AdjustmentEvent.ADJUSTMENT_VALUE_CHANGED:
				  if ((EventMask & AWTEvent.ADJUSTMENT_EVENT_MASK) != 0)
				  {
					  return true;
				  }
				  break;
			  default:
				  break;
			}
			//
			// Always pass on events defined by external programs.
			//
			if (type > AWTEvent.RESERVED_ID_MAX)
			{
				return true;
			}
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by dispatchEvent(AWTEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool PostEvent(Event e)
		{
			ComponentPeer peer = this.Peer_Renamed;

			if (HandleEvent(e))
			{
				e.Consume();
				return true;
			}

			Component parent = this.Parent_Renamed;
			int eventx = e.x;
			int eventy = e.y;
			if (parent != null)
			{
				e.Translate(x, y);
				if (parent.PostEvent(e))
				{
					e.Consume();
					return true;
				}
				// restore coords
				e.x = eventx;
				e.y = eventy;
			}
			return false;
		}

		// Event source interfaces

		/// <summary>
		/// Adds the specified component listener to receive component events from
		/// this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the component listener </param>
		/// <seealso cref=      java.awt.event.ComponentEvent </seealso>
		/// <seealso cref=      java.awt.event.ComponentListener </seealso>
		/// <seealso cref=      #removeComponentListener </seealso>
		/// <seealso cref=      #getComponentListeners
		/// @since    JDK1.1 </seealso>
		public virtual void AddComponentListener(ComponentListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				ComponentListener = AWTEventMulticaster.Add(ComponentListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Removes the specified component listener so that it no longer
		/// receives component events from this component. This method performs
		/// no function, nor does it throw an exception, if the listener
		/// specified by the argument was not previously added to this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// </para>
		/// </summary>
		/// <param name="l">   the component listener </param>
		/// <seealso cref=      java.awt.event.ComponentEvent </seealso>
		/// <seealso cref=      java.awt.event.ComponentListener </seealso>
		/// <seealso cref=      #addComponentListener </seealso>
		/// <seealso cref=      #getComponentListeners
		/// @since    JDK1.1 </seealso>
		public virtual void RemoveComponentListener(ComponentListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				ComponentListener = AWTEventMulticaster.Remove(ComponentListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the component listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all <code>ComponentListener</code>s of this component
		///         or an empty array if no component
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref= #addComponentListener </seealso>
		/// <seealso cref= #removeComponentListener
		/// @since 1.4 </seealso>
		public virtual ComponentListener[] ComponentListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(ComponentListener));
				}
			}
		}

		/// <summary>
		/// Adds the specified focus listener to receive focus events from
		/// this component when this component gains input focus.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the focus listener </param>
		/// <seealso cref=      java.awt.event.FocusEvent </seealso>
		/// <seealso cref=      java.awt.event.FocusListener </seealso>
		/// <seealso cref=      #removeFocusListener </seealso>
		/// <seealso cref=      #getFocusListeners
		/// @since    JDK1.1 </seealso>
		public virtual void AddFocusListener(FocusListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				FocusListener = AWTEventMulticaster.Add(FocusListener, l);
				NewEventsOnly = true;
        
				// if this is a lightweight component, enable focus events
				// in the native container.
				if (Peer_Renamed is LightweightPeer)
				{
					Parent_Renamed.ProxyEnableEvents(AWTEvent.FOCUS_EVENT_MASK);
				}
			}
		}

		/// <summary>
		/// Removes the specified focus listener so that it no longer
		/// receives focus events from this component. This method performs
		/// no function, nor does it throw an exception, if the listener
		/// specified by the argument was not previously added to this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the focus listener </param>
		/// <seealso cref=      java.awt.event.FocusEvent </seealso>
		/// <seealso cref=      java.awt.event.FocusListener </seealso>
		/// <seealso cref=      #addFocusListener </seealso>
		/// <seealso cref=      #getFocusListeners
		/// @since    JDK1.1 </seealso>
		public virtual void RemoveFocusListener(FocusListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				FocusListener = AWTEventMulticaster.Remove(FocusListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the focus listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all of this component's <code>FocusListener</code>s
		///         or an empty array if no component
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref= #addFocusListener </seealso>
		/// <seealso cref= #removeFocusListener
		/// @since 1.4 </seealso>
		public virtual FocusListener[] FocusListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(FocusListener));
				}
			}
		}

		/// <summary>
		/// Adds the specified hierarchy listener to receive hierarchy changed
		/// events from this component when the hierarchy to which this container
		/// belongs changes.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the hierarchy listener </param>
		/// <seealso cref=      java.awt.event.HierarchyEvent </seealso>
		/// <seealso cref=      java.awt.event.HierarchyListener </seealso>
		/// <seealso cref=      #removeHierarchyListener </seealso>
		/// <seealso cref=      #getHierarchyListeners
		/// @since    1.3 </seealso>
		public virtual void AddHierarchyListener(HierarchyListener l)
		{
			if (l == null)
			{
				return;
			}
			bool notifyAncestors;
			lock (this)
			{
				notifyAncestors = (HierarchyListener == null && (EventMask & AWTEvent.HIERARCHY_EVENT_MASK) == 0);
				HierarchyListener = AWTEventMulticaster.Add(HierarchyListener, l);
				notifyAncestors = (notifyAncestors && HierarchyListener != null);
				NewEventsOnly = true;
			}
			if (notifyAncestors)
			{
				lock (TreeLock)
				{
					AdjustListeningChildrenOnParent(AWTEvent.HIERARCHY_EVENT_MASK, 1);
				}
			}
		}

		/// <summary>
		/// Removes the specified hierarchy listener so that it no longer
		/// receives hierarchy changed events from this component. This method
		/// performs no function, nor does it throw an exception, if the listener
		/// specified by the argument was not previously added to this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the hierarchy listener </param>
		/// <seealso cref=      java.awt.event.HierarchyEvent </seealso>
		/// <seealso cref=      java.awt.event.HierarchyListener </seealso>
		/// <seealso cref=      #addHierarchyListener </seealso>
		/// <seealso cref=      #getHierarchyListeners
		/// @since    1.3 </seealso>
		public virtual void RemoveHierarchyListener(HierarchyListener l)
		{
			if (l == null)
			{
				return;
			}
			bool notifyAncestors;
			lock (this)
			{
				notifyAncestors = (HierarchyListener != null && (EventMask & AWTEvent.HIERARCHY_EVENT_MASK) == 0);
				HierarchyListener = AWTEventMulticaster.Remove(HierarchyListener, l);
				notifyAncestors = (notifyAncestors && HierarchyListener == null);
			}
			if (notifyAncestors)
			{
				lock (TreeLock)
				{
					AdjustListeningChildrenOnParent(AWTEvent.HIERARCHY_EVENT_MASK, -1);
				}
			}
		}

		/// <summary>
		/// Returns an array of all the hierarchy listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all of this component's <code>HierarchyListener</code>s
		///         or an empty array if no hierarchy
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addHierarchyListener </seealso>
		/// <seealso cref=      #removeHierarchyListener
		/// @since    1.4 </seealso>
		public virtual HierarchyListener[] HierarchyListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(HierarchyListener));
				}
			}
		}

		/// <summary>
		/// Adds the specified hierarchy bounds listener to receive hierarchy
		/// bounds events from this component when the hierarchy to which this
		/// container belongs changes.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the hierarchy bounds listener </param>
		/// <seealso cref=      java.awt.event.HierarchyEvent </seealso>
		/// <seealso cref=      java.awt.event.HierarchyBoundsListener </seealso>
		/// <seealso cref=      #removeHierarchyBoundsListener </seealso>
		/// <seealso cref=      #getHierarchyBoundsListeners
		/// @since    1.3 </seealso>
		public virtual void AddHierarchyBoundsListener(HierarchyBoundsListener l)
		{
			if (l == null)
			{
				return;
			}
			bool notifyAncestors;
			lock (this)
			{
				notifyAncestors = (HierarchyBoundsListener == null && (EventMask & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) == 0);
				HierarchyBoundsListener = AWTEventMulticaster.Add(HierarchyBoundsListener, l);
				notifyAncestors = (notifyAncestors && HierarchyBoundsListener != null);
				NewEventsOnly = true;
			}
			if (notifyAncestors)
			{
				lock (TreeLock)
				{
					AdjustListeningChildrenOnParent(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, 1);
				}
			}
		}

		/// <summary>
		/// Removes the specified hierarchy bounds listener so that it no longer
		/// receives hierarchy bounds events from this component. This method
		/// performs no function, nor does it throw an exception, if the listener
		/// specified by the argument was not previously added to this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the hierarchy bounds listener </param>
		/// <seealso cref=      java.awt.event.HierarchyEvent </seealso>
		/// <seealso cref=      java.awt.event.HierarchyBoundsListener </seealso>
		/// <seealso cref=      #addHierarchyBoundsListener </seealso>
		/// <seealso cref=      #getHierarchyBoundsListeners
		/// @since    1.3 </seealso>
		public virtual void RemoveHierarchyBoundsListener(HierarchyBoundsListener l)
		{
			if (l == null)
			{
				return;
			}
			bool notifyAncestors;
			lock (this)
			{
				notifyAncestors = (HierarchyBoundsListener != null && (EventMask & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) == 0);
				HierarchyBoundsListener = AWTEventMulticaster.Remove(HierarchyBoundsListener, l);
				notifyAncestors = (notifyAncestors && HierarchyBoundsListener == null);
			}
			if (notifyAncestors)
			{
				lock (TreeLock)
				{
					AdjustListeningChildrenOnParent(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, -1);
				}
			}
		}

		// Should only be called while holding the tree lock
		internal virtual int NumListening(long mask)
		{
			// One mask or the other, but not neither or both.
			if (EventLog.isLoggable(PlatformLogger.Level.FINE))
			{
				if ((mask != AWTEvent.HIERARCHY_EVENT_MASK) && (mask != AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK))
				{
					EventLog.fine("Assertion failed");
				}
			}
			if ((mask == AWTEvent.HIERARCHY_EVENT_MASK && (HierarchyListener != null || (EventMask & AWTEvent.HIERARCHY_EVENT_MASK) != 0)) || (mask == AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK && (HierarchyBoundsListener != null || (EventMask & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) != 0)))
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		// Should only be called while holding tree lock
		internal virtual int CountHierarchyMembers()
		{
			return 1;
		}
		// Should only be called while holding the tree lock
		internal virtual int CreateHierarchyEvents(int id, Component changed, Container changedParent, long changeFlags, bool enabledOnToolkit)
		{
			switch (id)
			{
			  case HierarchyEvent.HIERARCHY_CHANGED:
				  if (HierarchyListener != null || (EventMask & AWTEvent.HIERARCHY_EVENT_MASK) != 0 || enabledOnToolkit)
				  {
					  HierarchyEvent e = new HierarchyEvent(this, id, changed, changedParent, changeFlags);
					  DispatchEvent(e);
					  return 1;
				  }
				  break;
			  case HierarchyEvent.ANCESTOR_MOVED:
			  case HierarchyEvent.ANCESTOR_RESIZED:
				  if (EventLog.isLoggable(PlatformLogger.Level.FINE))
				  {
					  if (changeFlags != 0)
					  {
						  EventLog.fine("Assertion (changeFlags == 0) failed");
					  }
				  }
				  if (HierarchyBoundsListener != null || (EventMask & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) != 0 || enabledOnToolkit)
				  {
					  HierarchyEvent e = new HierarchyEvent(this, id, changed, changedParent);
					  DispatchEvent(e);
					  return 1;
				  }
				  break;
			  default:
				  // assert false
				  if (EventLog.isLoggable(PlatformLogger.Level.FINE))
				  {
					  EventLog.fine("This code must never be reached");
				  }
				  break;
			}
			return 0;
		}

		/// <summary>
		/// Returns an array of all the hierarchy bounds listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all of this component's <code>HierarchyBoundsListener</code>s
		///         or an empty array if no hierarchy bounds
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addHierarchyBoundsListener </seealso>
		/// <seealso cref=      #removeHierarchyBoundsListener
		/// @since    1.4 </seealso>
		public virtual HierarchyBoundsListener[] HierarchyBoundsListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(HierarchyBoundsListener));
				}
			}
		}

		/*
		 * Should only be called while holding the tree lock.
		 * It's added only for overriding in java.awt.Window
		 * because parent in Window is owner.
		 */
		internal virtual void AdjustListeningChildrenOnParent(long mask, int num)
		{
			if (Parent_Renamed != null)
			{
				Parent_Renamed.AdjustListeningChildren(mask, num);
			}
		}

		/// <summary>
		/// Adds the specified key listener to receive key events from
		/// this component.
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the key listener. </param>
		/// <seealso cref=      java.awt.event.KeyEvent </seealso>
		/// <seealso cref=      java.awt.event.KeyListener </seealso>
		/// <seealso cref=      #removeKeyListener </seealso>
		/// <seealso cref=      #getKeyListeners
		/// @since    JDK1.1 </seealso>
		public virtual void AddKeyListener(KeyListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				KeyListener = AWTEventMulticaster.Add(KeyListener, l);
				NewEventsOnly = true;
        
				// if this is a lightweight component, enable key events
				// in the native container.
				if (Peer_Renamed is LightweightPeer)
				{
					Parent_Renamed.ProxyEnableEvents(AWTEvent.KEY_EVENT_MASK);
				}
			}
		}

		/// <summary>
		/// Removes the specified key listener so that it no longer
		/// receives key events from this component. This method performs
		/// no function, nor does it throw an exception, if the listener
		/// specified by the argument was not previously added to this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the key listener </param>
		/// <seealso cref=      java.awt.event.KeyEvent </seealso>
		/// <seealso cref=      java.awt.event.KeyListener </seealso>
		/// <seealso cref=      #addKeyListener </seealso>
		/// <seealso cref=      #getKeyListeners
		/// @since    JDK1.1 </seealso>
		public virtual void RemoveKeyListener(KeyListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				KeyListener = AWTEventMulticaster.Remove(KeyListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the key listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all of this component's <code>KeyListener</code>s
		///         or an empty array if no key
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addKeyListener </seealso>
		/// <seealso cref=      #removeKeyListener
		/// @since    1.4 </seealso>
		public virtual KeyListener[] KeyListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(KeyListener));
				}
			}
		}

		/// <summary>
		/// Adds the specified mouse listener to receive mouse events from
		/// this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the mouse listener </param>
		/// <seealso cref=      java.awt.event.MouseEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseListener </seealso>
		/// <seealso cref=      #removeMouseListener </seealso>
		/// <seealso cref=      #getMouseListeners
		/// @since    JDK1.1 </seealso>
		public virtual void AddMouseListener(MouseListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				MouseListener = AWTEventMulticaster.Add(MouseListener,l);
				NewEventsOnly = true;
        
				// if this is a lightweight component, enable mouse events
				// in the native container.
				if (Peer_Renamed is LightweightPeer)
				{
					Parent_Renamed.ProxyEnableEvents(AWTEvent.MOUSE_EVENT_MASK);
				}
			}
		}

		/// <summary>
		/// Removes the specified mouse listener so that it no longer
		/// receives mouse events from this component. This method performs
		/// no function, nor does it throw an exception, if the listener
		/// specified by the argument was not previously added to this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the mouse listener </param>
		/// <seealso cref=      java.awt.event.MouseEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseListener </seealso>
		/// <seealso cref=      #addMouseListener </seealso>
		/// <seealso cref=      #getMouseListeners
		/// @since    JDK1.1 </seealso>
		public virtual void RemoveMouseListener(MouseListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				MouseListener = AWTEventMulticaster.Remove(MouseListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the mouse listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all of this component's <code>MouseListener</code>s
		///         or an empty array if no mouse
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addMouseListener </seealso>
		/// <seealso cref=      #removeMouseListener
		/// @since    1.4 </seealso>
		public virtual MouseListener[] MouseListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(MouseListener));
				}
			}
		}

		/// <summary>
		/// Adds the specified mouse motion listener to receive mouse motion
		/// events from this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the mouse motion listener </param>
		/// <seealso cref=      java.awt.event.MouseEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseMotionListener </seealso>
		/// <seealso cref=      #removeMouseMotionListener </seealso>
		/// <seealso cref=      #getMouseMotionListeners
		/// @since    JDK1.1 </seealso>
		public virtual void AddMouseMotionListener(MouseMotionListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				MouseMotionListener = AWTEventMulticaster.Add(MouseMotionListener,l);
				NewEventsOnly = true;
        
				// if this is a lightweight component, enable mouse events
				// in the native container.
				if (Peer_Renamed is LightweightPeer)
				{
					Parent_Renamed.ProxyEnableEvents(AWTEvent.MOUSE_MOTION_EVENT_MASK);
				}
			}
		}

		/// <summary>
		/// Removes the specified mouse motion listener so that it no longer
		/// receives mouse motion events from this component. This method performs
		/// no function, nor does it throw an exception, if the listener
		/// specified by the argument was not previously added to this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the mouse motion listener </param>
		/// <seealso cref=      java.awt.event.MouseEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseMotionListener </seealso>
		/// <seealso cref=      #addMouseMotionListener </seealso>
		/// <seealso cref=      #getMouseMotionListeners
		/// @since    JDK1.1 </seealso>
		public virtual void RemoveMouseMotionListener(MouseMotionListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				MouseMotionListener = AWTEventMulticaster.Remove(MouseMotionListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the mouse motion listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all of this component's <code>MouseMotionListener</code>s
		///         or an empty array if no mouse motion
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addMouseMotionListener </seealso>
		/// <seealso cref=      #removeMouseMotionListener
		/// @since    1.4 </seealso>
		public virtual MouseMotionListener[] MouseMotionListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(MouseMotionListener));
				}
			}
		}

		/// <summary>
		/// Adds the specified mouse wheel listener to receive mouse wheel events
		/// from this component.  Containers also receive mouse wheel events from
		/// sub-components.
		/// <para>
		/// For information on how mouse wheel events are dispatched, see
		/// the class description for <seealso cref="MouseWheelEvent"/>.
		/// </para>
		/// <para>
		/// If l is <code>null</code>, no exception is thrown and no
		/// action is performed.
		/// </para>
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the mouse wheel listener </param>
		/// <seealso cref=      java.awt.event.MouseWheelEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseWheelListener </seealso>
		/// <seealso cref=      #removeMouseWheelListener </seealso>
		/// <seealso cref=      #getMouseWheelListeners
		/// @since    1.4 </seealso>
		public virtual void AddMouseWheelListener(MouseWheelListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				MouseWheelListener = AWTEventMulticaster.Add(MouseWheelListener,l);
				NewEventsOnly = true;
        
				// if this is a lightweight component, enable mouse events
				// in the native container.
				if (Peer_Renamed is LightweightPeer)
				{
					Parent_Renamed.ProxyEnableEvents(AWTEvent.MOUSE_WHEEL_EVENT_MASK);
				}
			}
		}

		/// <summary>
		/// Removes the specified mouse wheel listener so that it no longer
		/// receives mouse wheel events from this component. This method performs
		/// no function, nor does it throw an exception, if the listener
		/// specified by the argument was not previously added to this component.
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the mouse wheel listener. </param>
		/// <seealso cref=      java.awt.event.MouseWheelEvent </seealso>
		/// <seealso cref=      java.awt.event.MouseWheelListener </seealso>
		/// <seealso cref=      #addMouseWheelListener </seealso>
		/// <seealso cref=      #getMouseWheelListeners
		/// @since    1.4 </seealso>
		public virtual void RemoveMouseWheelListener(MouseWheelListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				MouseWheelListener = AWTEventMulticaster.Remove(MouseWheelListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the mouse wheel listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all of this component's <code>MouseWheelListener</code>s
		///         or an empty array if no mouse wheel
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addMouseWheelListener </seealso>
		/// <seealso cref=      #removeMouseWheelListener
		/// @since    1.4 </seealso>
		public virtual MouseWheelListener[] MouseWheelListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(MouseWheelListener));
				}
			}
		}

		/// <summary>
		/// Adds the specified input method listener to receive
		/// input method events from this component. A component will
		/// only receive input method events from input methods
		/// if it also overrides <code>getInputMethodRequests</code> to return an
		/// <code>InputMethodRequests</code> instance.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="{@docRoot}/java/awt/doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the input method listener </param>
		/// <seealso cref=      java.awt.event.InputMethodEvent </seealso>
		/// <seealso cref=      java.awt.event.InputMethodListener </seealso>
		/// <seealso cref=      #removeInputMethodListener </seealso>
		/// <seealso cref=      #getInputMethodListeners </seealso>
		/// <seealso cref=      #getInputMethodRequests
		/// @since    1.2 </seealso>
		public virtual void AddInputMethodListener(InputMethodListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				InputMethodListener = AWTEventMulticaster.Add(InputMethodListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Removes the specified input method listener so that it no longer
		/// receives input method events from this component. This method performs
		/// no function, nor does it throw an exception, if the listener
		/// specified by the argument was not previously added to this component.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the input method listener </param>
		/// <seealso cref=      java.awt.event.InputMethodEvent </seealso>
		/// <seealso cref=      java.awt.event.InputMethodListener </seealso>
		/// <seealso cref=      #addInputMethodListener </seealso>
		/// <seealso cref=      #getInputMethodListeners
		/// @since    1.2 </seealso>
		public virtual void RemoveInputMethodListener(InputMethodListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				InputMethodListener = AWTEventMulticaster.Remove(InputMethodListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the input method listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all of this component's <code>InputMethodListener</code>s
		///         or an empty array if no input method
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addInputMethodListener </seealso>
		/// <seealso cref=      #removeInputMethodListener
		/// @since    1.4 </seealso>
		public virtual InputMethodListener[] InputMethodListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(InputMethodListener));
				}
			}
		}

		/// <summary>
		/// Returns an array of all the objects currently registered
		/// as <code><em>Foo</em>Listener</code>s
		/// upon this <code>Component</code>.
		/// <code><em>Foo</em>Listener</code>s are registered using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// 
		/// <para>
		/// You can specify the <code>listenerType</code> argument
		/// with a class literal, such as
		/// <code><em>Foo</em>Listener.class</code>.
		/// For example, you can query a
		/// <code>Component</code> <code>c</code>
		/// for its mouse listeners with the following code:
		/// 
		/// <pre>MouseListener[] mls = (MouseListener[])(c.getListeners(MouseListener.class));</pre>
		/// 
		/// If no such listeners exist, this method returns an empty array.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listenerType"> the type of listeners requested; this parameter
		///          should specify an interface that descends from
		///          <code>java.util.EventListener</code> </param>
		/// <returns> an array of all objects registered as
		///          <code><em>Foo</em>Listener</code>s on this component,
		///          or an empty array if no such listeners have been added </returns>
		/// <exception cref="ClassCastException"> if <code>listenerType</code>
		///          doesn't specify a class or interface that implements
		///          <code>java.util.EventListener</code> </exception>
		/// <exception cref="NullPointerException"> if {@code listenerType} is {@code null} </exception>
		/// <seealso cref= #getComponentListeners </seealso>
		/// <seealso cref= #getFocusListeners </seealso>
		/// <seealso cref= #getHierarchyListeners </seealso>
		/// <seealso cref= #getHierarchyBoundsListeners </seealso>
		/// <seealso cref= #getKeyListeners </seealso>
		/// <seealso cref= #getMouseListeners </seealso>
		/// <seealso cref= #getMouseMotionListeners </seealso>
		/// <seealso cref= #getMouseWheelListeners </seealso>
		/// <seealso cref= #getInputMethodListeners </seealso>
		/// <seealso cref= #getPropertyChangeListeners
		/// 
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends java.util.EventListener> T[] getListeners(Class listenerType)
		public virtual T[] getListeners<T>(Class listenerType) where T : java.util.EventListener
		{
			EventListener l = null;
			if (listenerType == typeof(ComponentListener))
			{
				l = ComponentListener;
			}
			else if (listenerType == typeof(FocusListener))
			{
				l = FocusListener;
			}
			else if (listenerType == typeof(HierarchyListener))
			{
				l = HierarchyListener;
			}
			else if (listenerType == typeof(HierarchyBoundsListener))
			{
				l = HierarchyBoundsListener;
			}
			else if (listenerType == typeof(KeyListener))
			{
				l = KeyListener;
			}
			else if (listenerType == typeof(MouseListener))
			{
				l = MouseListener;
			}
			else if (listenerType == typeof(MouseMotionListener))
			{
				l = MouseMotionListener;
			}
			else if (listenerType == typeof(MouseWheelListener))
			{
				l = MouseWheelListener;
			}
			else if (listenerType == typeof(InputMethodListener))
			{
				l = InputMethodListener;
			}
			else if (listenerType == typeof(PropertyChangeListener))
			{
				return (T[])PropertyChangeListeners;
			}
			return AWTEventMulticaster.GetListeners(l, listenerType);
		}

		/// <summary>
		/// Gets the input method request handler which supports
		/// requests from input methods for this component. A component
		/// that supports on-the-spot text input must override this
		/// method to return an <code>InputMethodRequests</code> instance.
		/// At the same time, it also has to handle input method events.
		/// </summary>
		/// <returns> the input method request handler for this component,
		///          <code>null</code> by default </returns>
		/// <seealso cref= #addInputMethodListener
		/// @since 1.2 </seealso>
		public virtual InputMethodRequests InputMethodRequests
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the input context used by this component for handling
		/// the communication with input methods when text is entered
		/// in this component. By default, the input context used for
		/// the parent component is returned. Components may
		/// override this to return a private input context.
		/// </summary>
		/// <returns> the input context used by this component;
		///          <code>null</code> if no context can be determined
		/// @since 1.2 </returns>
		public virtual InputContext InputContext
		{
			get
			{
				Container parent = this.Parent_Renamed;
				if (parent == null)
				{
					return null;
				}
				else
				{
					return parent.InputContext;
				}
			}
		}

		/// <summary>
		/// Enables the events defined by the specified event mask parameter
		/// to be delivered to this component.
		/// <para>
		/// Event types are automatically enabled when a listener for
		/// that event type is added to the component.
		/// </para>
		/// <para>
		/// This method only needs to be invoked by subclasses of
		/// <code>Component</code> which desire to have the specified event
		/// types delivered to <code>processEvent</code> regardless of whether
		/// or not a listener is registered.
		/// </para>
		/// </summary>
		/// <param name="eventsToEnable">   the event mask defining the event types </param>
		/// <seealso cref=        #processEvent </seealso>
		/// <seealso cref=        #disableEvents </seealso>
		/// <seealso cref=        AWTEvent
		/// @since      JDK1.1 </seealso>
		protected internal void EnableEvents(long eventsToEnable)
		{
			long notifyAncestors = 0;
			lock (this)
			{
				if ((eventsToEnable & AWTEvent.HIERARCHY_EVENT_MASK) != 0 && HierarchyListener == null && (EventMask & AWTEvent.HIERARCHY_EVENT_MASK) == 0)
				{
					notifyAncestors |= AWTEvent.HIERARCHY_EVENT_MASK;
				}
				if ((eventsToEnable & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) != 0 && HierarchyBoundsListener == null && (EventMask & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) == 0)
				{
					notifyAncestors |= AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK;
				}
				EventMask |= eventsToEnable;
				NewEventsOnly = true;
			}

			// if this is a lightweight component, enable mouse events
			// in the native container.
			if (Peer_Renamed is LightweightPeer)
			{
				Parent_Renamed.ProxyEnableEvents(EventMask);
			}
			if (notifyAncestors != 0)
			{
				lock (TreeLock)
				{
					AdjustListeningChildrenOnParent(notifyAncestors, 1);
				}
			}
		}

		/// <summary>
		/// Disables the events defined by the specified event mask parameter
		/// from being delivered to this component. </summary>
		/// <param name="eventsToDisable">   the event mask defining the event types </param>
		/// <seealso cref=        #enableEvents
		/// @since      JDK1.1 </seealso>
		protected internal void DisableEvents(long eventsToDisable)
		{
			long notifyAncestors = 0;
			lock (this)
			{
				if ((eventsToDisable & AWTEvent.HIERARCHY_EVENT_MASK) != 0 && HierarchyListener == null && (EventMask & AWTEvent.HIERARCHY_EVENT_MASK) != 0)
				{
					notifyAncestors |= AWTEvent.HIERARCHY_EVENT_MASK;
				}
				if ((eventsToDisable & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) != 0 && HierarchyBoundsListener == null && (EventMask & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) != 0)
				{
					notifyAncestors |= AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK;
				}
				EventMask &= ~eventsToDisable;
			}
			if (notifyAncestors != 0)
			{
				lock (TreeLock)
				{
					AdjustListeningChildrenOnParent(notifyAncestors, -1);
				}
			}
		}

		[NonSerialized]
		internal sun.awt.EventQueueItem[] EventCache;

		/// <seealso cref= #isCoalescingEnabled </seealso>
		/// <seealso cref= #checkCoalescing </seealso>
		[NonSerialized]
		private bool CoalescingEnabled_Renamed;

		/// <summary>
		/// Weak map of known coalesceEvent overriders.
		/// Value indicates whether overriden.
		/// Bootstrap classes are not included.
		/// </summary>
		private static readonly IDictionary<Class, Boolean> CoalesceMap = new java.util.WeakHashMap<Class, Boolean>();

		/// <summary>
		/// Indicates whether this class overrides coalesceEvents.
		/// It is assumed that all classes that are loaded from the bootstrap
		///   do not.
		/// The boostrap class loader is assumed to be represented by null.
		/// We do not check that the method really overrides
		///   (it might be static, private or package private).
		/// </summary>
		 private bool CheckCoalescing()
		 {
			 if (this.GetType().ClassLoader == null)
			 {
				 return false;
			 }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class clazz = getClass();
			 Class clazz = this.GetType();
			 lock (CoalesceMap)
			 {
				 // Check cache.
				 Boolean value = CoalesceMap[clazz];
				 if (value != null)
				 {
					 return value;
				 }

				 // Need to check non-bootstraps.
				 Boolean enabled = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, clazz)
					);
				 CoalesceMap[clazz] = enabled;
				 return enabled;
			 }
		 }

		 private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Boolean>
		 {
			 private readonly Component OuterInstance;

			 private Type Clazz;

			 public PrivilegedActionAnonymousInnerClassHelper(Component outerInstance, Type clazz)
			 {
				 this.OuterInstance = outerInstance;
				 this.Clazz = clazz;
			 }

			 public virtual Boolean Run()
			 {
				 return IsCoalesceEventsOverriden(Clazz);
			 }
		 }

		/// <summary>
		/// Parameter types of coalesceEvents(AWTEvent,AWTEVent).
		/// </summary>
		private static readonly Class[] CoalesceEventsParams = new Class[] {typeof(AWTEvent), typeof(AWTEvent)};

		/// <summary>
		/// Indicates whether a class or its superclasses override coalesceEvents.
		/// Must be called with lock on coalesceMap and privileged. </summary>
		/// <seealso cref= checkCoalescing </seealso>
		private static bool IsCoalesceEventsOverriden(Class clazz)
		{
			Debug.Assert(Thread.holdsLock(CoalesceMap));

			// First check superclass - we may not need to bother ourselves.
			Class superclass = clazz.BaseType;
			if (superclass == null)
			{
				// Only occurs on implementations that
				//   do not use null to represent the bootsrap class loader.
				return false;
			}
			if (superclass.ClassLoader != null)
			{
				Boolean value = CoalesceMap[superclass];
				if (value == null)
				{
					// Not done already - recurse.
					if (IsCoalesceEventsOverriden(superclass))
					{
						CoalesceMap[superclass] = true;
						return true;
					}
				}
				else if (value)
				{
					return true;
				}
			}

			try
			{
				// Throws if not overriden.
				clazz.GetDeclaredMethod("coalesceEvents", CoalesceEventsParams);
				return true;
			}
			catch (NoSuchMethodException)
			{
				// Not present in this class.
				return false;
			}
		}

		/// <summary>
		/// Indicates whether coalesceEvents may do something.
		/// </summary>
		internal bool CoalescingEnabled
		{
			get
			{
				return CoalescingEnabled_Renamed;
			}
		}


		/// <summary>
		/// Potentially coalesce an event being posted with an existing
		/// event.  This method is called by <code>EventQueue.postEvent</code>
		/// if an event with the same ID as the event to be posted is found in
		/// the queue (both events must have this component as their source).
		/// This method either returns a coalesced event which replaces
		/// the existing event (and the new event is then discarded), or
		/// <code>null</code> to indicate that no combining should be done
		/// (add the second event to the end of the queue).  Either event
		/// parameter may be modified and returned, as the other one is discarded
		/// unless <code>null</code> is returned.
		/// <para>
		/// This implementation of <code>coalesceEvents</code> coalesces
		/// two event types: mouse move (and drag) events,
		/// and paint (and update) events.
		/// For mouse move events the last event is always returned, causing
		/// intermediate moves to be discarded.  For paint events, the new
		/// event is coalesced into a complex <code>RepaintArea</code> in the peer.
		/// The new <code>AWTEvent</code> is always returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="existingEvent">  the event already on the <code>EventQueue</code> </param>
		/// <param name="newEvent">       the event being posted to the
		///          <code>EventQueue</code> </param>
		/// <returns> a coalesced event, or <code>null</code> indicating that no
		///          coalescing was done </returns>
		protected internal virtual AWTEvent CoalesceEvents(AWTEvent existingEvent, AWTEvent newEvent)
		{
			return null;
		}

		/// <summary>
		/// Processes events occurring on this component. By default this
		/// method calls the appropriate
		/// <code>process&lt;event&nbsp;type&gt;Event</code>
		/// method for the given class of event.
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the event </param>
		/// <seealso cref=       #processComponentEvent </seealso>
		/// <seealso cref=       #processFocusEvent </seealso>
		/// <seealso cref=       #processKeyEvent </seealso>
		/// <seealso cref=       #processMouseEvent </seealso>
		/// <seealso cref=       #processMouseMotionEvent </seealso>
		/// <seealso cref=       #processInputMethodEvent </seealso>
		/// <seealso cref=       #processHierarchyEvent </seealso>
		/// <seealso cref=       #processMouseWheelEvent
		/// @since     JDK1.1 </seealso>
		protected internal virtual void ProcessEvent(AWTEvent e)
		{
			if (e is FocusEvent)
			{
				ProcessFocusEvent((FocusEvent)e);

			}
			else if (e is MouseEvent)
			{
				switch (e.ID)
				{
				  case MouseEvent.MOUSE_PRESSED:
				  case MouseEvent.MOUSE_RELEASED:
				  case MouseEvent.MOUSE_CLICKED:
				  case MouseEvent.MOUSE_ENTERED:
				  case MouseEvent.MOUSE_EXITED:
					  ProcessMouseEvent((MouseEvent)e);
					  break;
				  case MouseEvent.MOUSE_MOVED:
				  case MouseEvent.MOUSE_DRAGGED:
					  ProcessMouseMotionEvent((MouseEvent)e);
					  break;
				  case MouseEvent.MOUSE_WHEEL:
					  ProcessMouseWheelEvent((MouseWheelEvent)e);
					  break;
				}

			}
			else if (e is KeyEvent)
			{
				ProcessKeyEvent((KeyEvent)e);

			}
			else if (e is ComponentEvent)
			{
				ProcessComponentEvent((ComponentEvent)e);
			}
			else if (e is InputMethodEvent)
			{
				ProcessInputMethodEvent((InputMethodEvent)e);
			}
			else if (e is HierarchyEvent)
			{
				switch (e.ID)
				{
				  case HierarchyEvent.HIERARCHY_CHANGED:
					  ProcessHierarchyEvent((HierarchyEvent)e);
					  break;
				  case HierarchyEvent.ANCESTOR_MOVED:
				  case HierarchyEvent.ANCESTOR_RESIZED:
					  ProcessHierarchyBoundsEvent((HierarchyEvent)e);
					  break;
				}
			}
		}

		/// <summary>
		/// Processes component events occurring on this component by
		/// dispatching them to any registered
		/// <code>ComponentListener</code> objects.
		/// <para>
		/// This method is not called unless component events are
		/// enabled for this component. Component events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>A <code>ComponentListener</code> object is registered
		/// via <code>addComponentListener</code>.
		/// <li>Component events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the component event </param>
		/// <seealso cref=         java.awt.event.ComponentEvent </seealso>
		/// <seealso cref=         java.awt.event.ComponentListener </seealso>
		/// <seealso cref=         #addComponentListener </seealso>
		/// <seealso cref=         #enableEvents
		/// @since       JDK1.1 </seealso>
		protected internal virtual void ProcessComponentEvent(ComponentEvent e)
		{
			ComponentListener listener = ComponentListener;
			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				  case ComponentEvent.COMPONENT_RESIZED:
					  listener.ComponentResized(e);
					  break;
				  case ComponentEvent.COMPONENT_MOVED:
					  listener.ComponentMoved(e);
					  break;
				  case ComponentEvent.COMPONENT_SHOWN:
					  listener.ComponentShown(e);
					  break;
				  case ComponentEvent.COMPONENT_HIDDEN:
					  listener.ComponentHidden(e);
					  break;
				}
			}
		}

		/// <summary>
		/// Processes focus events occurring on this component by
		/// dispatching them to any registered
		/// <code>FocusListener</code> objects.
		/// <para>
		/// This method is not called unless focus events are
		/// enabled for this component. Focus events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>A <code>FocusListener</code> object is registered
		/// via <code>addFocusListener</code>.
		/// <li>Focus events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>
		/// If focus events are enabled for a <code>Component</code>,
		/// the current <code>KeyboardFocusManager</code> determines
		/// whether or not a focus event should be dispatched to
		/// registered <code>FocusListener</code> objects.  If the
		/// events are to be dispatched, the <code>KeyboardFocusManager</code>
		/// calls the <code>Component</code>'s <code>dispatchEvent</code>
		/// method, which results in a call to the <code>Component</code>'s
		/// <code>processFocusEvent</code> method.
		/// </para>
		/// <para>
		/// If focus events are enabled for a <code>Component</code>, calling
		/// the <code>Component</code>'s <code>dispatchEvent</code> method
		/// with a <code>FocusEvent</code> as the argument will result in a
		/// call to the <code>Component</code>'s <code>processFocusEvent</code>
		/// method regardless of the current <code>KeyboardFocusManager</code>.
		/// 
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the focus event </param>
		/// <seealso cref=         java.awt.event.FocusEvent </seealso>
		/// <seealso cref=         java.awt.event.FocusListener </seealso>
		/// <seealso cref=         java.awt.KeyboardFocusManager </seealso>
		/// <seealso cref=         #addFocusListener </seealso>
		/// <seealso cref=         #enableEvents </seealso>
		/// <seealso cref=         #dispatchEvent
		/// @since       JDK1.1 </seealso>
		protected internal virtual void ProcessFocusEvent(FocusEvent e)
		{
			FocusListener listener = FocusListener;
			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				  case FocusEvent.FOCUS_GAINED:
					  listener.FocusGained(e);
					  break;
				  case FocusEvent.FOCUS_LOST:
					  listener.FocusLost(e);
					  break;
				}
			}
		}

		/// <summary>
		/// Processes key events occurring on this component by
		/// dispatching them to any registered
		/// <code>KeyListener</code> objects.
		/// <para>
		/// This method is not called unless key events are
		/// enabled for this component. Key events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>A <code>KeyListener</code> object is registered
		/// via <code>addKeyListener</code>.
		/// <li>Key events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// 
		/// </para>
		/// <para>
		/// If key events are enabled for a <code>Component</code>,
		/// the current <code>KeyboardFocusManager</code> determines
		/// whether or not a key event should be dispatched to
		/// registered <code>KeyListener</code> objects.  The
		/// <code>DefaultKeyboardFocusManager</code> will not dispatch
		/// key events to a <code>Component</code> that is not the focus
		/// owner or is not showing.
		/// </para>
		/// <para>
		/// As of J2SE 1.4, <code>KeyEvent</code>s are redirected to
		/// the focus owner. Please see the
		/// <a href="doc-files/FocusSpec.html">Focus Specification</a>
		/// for further information.
		/// </para>
		/// <para>
		/// Calling a <code>Component</code>'s <code>dispatchEvent</code>
		/// method with a <code>KeyEvent</code> as the argument will
		/// result in a call to the <code>Component</code>'s
		/// <code>processKeyEvent</code> method regardless of the
		/// current <code>KeyboardFocusManager</code> as long as the
		/// component is showing, focused, and enabled, and key events
		/// are enabled on it.
		/// </para>
		/// <para>If the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the key event </param>
		/// <seealso cref=         java.awt.event.KeyEvent </seealso>
		/// <seealso cref=         java.awt.event.KeyListener </seealso>
		/// <seealso cref=         java.awt.KeyboardFocusManager </seealso>
		/// <seealso cref=         java.awt.DefaultKeyboardFocusManager </seealso>
		/// <seealso cref=         #processEvent </seealso>
		/// <seealso cref=         #dispatchEvent </seealso>
		/// <seealso cref=         #addKeyListener </seealso>
		/// <seealso cref=         #enableEvents </seealso>
		/// <seealso cref=         #isShowing
		/// @since       JDK1.1 </seealso>
		protected internal virtual void ProcessKeyEvent(KeyEvent e)
		{
			KeyListener listener = KeyListener;
			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				  case KeyEvent.KEY_TYPED:
					  listener.KeyTyped(e);
					  break;
				  case KeyEvent.KEY_PRESSED:
					  listener.KeyPressed(e);
					  break;
				  case KeyEvent.KEY_RELEASED:
					  listener.KeyReleased(e);
					  break;
				}
			}
		}

		/// <summary>
		/// Processes mouse events occurring on this component by
		/// dispatching them to any registered
		/// <code>MouseListener</code> objects.
		/// <para>
		/// This method is not called unless mouse events are
		/// enabled for this component. Mouse events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>A <code>MouseListener</code> object is registered
		/// via <code>addMouseListener</code>.
		/// <li>Mouse events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the mouse event </param>
		/// <seealso cref=         java.awt.event.MouseEvent </seealso>
		/// <seealso cref=         java.awt.event.MouseListener </seealso>
		/// <seealso cref=         #addMouseListener </seealso>
		/// <seealso cref=         #enableEvents
		/// @since       JDK1.1 </seealso>
		protected internal virtual void ProcessMouseEvent(MouseEvent e)
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
				  case MouseEvent.MOUSE_EXITED:
					  listener.MouseExited(e);
					  break;
				  case MouseEvent.MOUSE_ENTERED:
					  listener.MouseEntered(e);
					  break;
				}
			}
		}

		/// <summary>
		/// Processes mouse motion events occurring on this component by
		/// dispatching them to any registered
		/// <code>MouseMotionListener</code> objects.
		/// <para>
		/// This method is not called unless mouse motion events are
		/// enabled for this component. Mouse motion events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>A <code>MouseMotionListener</code> object is registered
		/// via <code>addMouseMotionListener</code>.
		/// <li>Mouse motion events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the mouse motion event </param>
		/// <seealso cref=         java.awt.event.MouseEvent </seealso>
		/// <seealso cref=         java.awt.event.MouseMotionListener </seealso>
		/// <seealso cref=         #addMouseMotionListener </seealso>
		/// <seealso cref=         #enableEvents
		/// @since       JDK1.1 </seealso>
		protected internal virtual void ProcessMouseMotionEvent(MouseEvent e)
		{
			MouseMotionListener listener = MouseMotionListener;
			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				  case MouseEvent.MOUSE_MOVED:
					  listener.MouseMoved(e);
					  break;
				  case MouseEvent.MOUSE_DRAGGED:
					  listener.MouseDragged(e);
					  break;
				}
			}
		}

		/// <summary>
		/// Processes mouse wheel events occurring on this component by
		/// dispatching them to any registered
		/// <code>MouseWheelListener</code> objects.
		/// <para>
		/// This method is not called unless mouse wheel events are
		/// enabled for this component. Mouse wheel events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>A <code>MouseWheelListener</code> object is registered
		/// via <code>addMouseWheelListener</code>.
		/// <li>Mouse wheel events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>
		/// For information on how mouse wheel events are dispatched, see
		/// the class description for <seealso cref="MouseWheelEvent"/>.
		/// </para>
		/// <para>
		/// Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the mouse wheel event </param>
		/// <seealso cref=         java.awt.event.MouseWheelEvent </seealso>
		/// <seealso cref=         java.awt.event.MouseWheelListener </seealso>
		/// <seealso cref=         #addMouseWheelListener </seealso>
		/// <seealso cref=         #enableEvents
		/// @since       1.4 </seealso>
		protected internal virtual void ProcessMouseWheelEvent(MouseWheelEvent e)
		{
			MouseWheelListener listener = MouseWheelListener;
			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				  case MouseEvent.MOUSE_WHEEL:
					  listener.MouseWheelMoved(e);
					  break;
				}
			}
		}

		internal virtual bool PostsOldMouseEvents()
		{
			return false;
		}

		/// <summary>
		/// Processes input method events occurring on this component by
		/// dispatching them to any registered
		/// <code>InputMethodListener</code> objects.
		/// <para>
		/// This method is not called unless input method events
		/// are enabled for this component. Input method events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>An <code>InputMethodListener</code> object is registered
		/// via <code>addInputMethodListener</code>.
		/// <li>Input method events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the input method event </param>
		/// <seealso cref=         java.awt.event.InputMethodEvent </seealso>
		/// <seealso cref=         java.awt.event.InputMethodListener </seealso>
		/// <seealso cref=         #addInputMethodListener </seealso>
		/// <seealso cref=         #enableEvents
		/// @since       1.2 </seealso>
		protected internal virtual void ProcessInputMethodEvent(InputMethodEvent e)
		{
			InputMethodListener listener = InputMethodListener;
			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				  case InputMethodEvent.INPUT_METHOD_TEXT_CHANGED:
					  listener.InputMethodTextChanged(e);
					  break;
				  case InputMethodEvent.CARET_POSITION_CHANGED:
					  listener.CaretPositionChanged(e);
					  break;
				}
			}
		}

		/// <summary>
		/// Processes hierarchy events occurring on this component by
		/// dispatching them to any registered
		/// <code>HierarchyListener</code> objects.
		/// <para>
		/// This method is not called unless hierarchy events
		/// are enabled for this component. Hierarchy events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>An <code>HierarchyListener</code> object is registered
		/// via <code>addHierarchyListener</code>.
		/// <li>Hierarchy events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the hierarchy event </param>
		/// <seealso cref=         java.awt.event.HierarchyEvent </seealso>
		/// <seealso cref=         java.awt.event.HierarchyListener </seealso>
		/// <seealso cref=         #addHierarchyListener </seealso>
		/// <seealso cref=         #enableEvents
		/// @since       1.3 </seealso>
		protected internal virtual void ProcessHierarchyEvent(HierarchyEvent e)
		{
			HierarchyListener listener = HierarchyListener;
			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				  case HierarchyEvent.HIERARCHY_CHANGED:
					  listener.HierarchyChanged(e);
					  break;
				}
			}
		}

		/// <summary>
		/// Processes hierarchy bounds events occurring on this component by
		/// dispatching them to any registered
		/// <code>HierarchyBoundsListener</code> objects.
		/// <para>
		/// This method is not called unless hierarchy bounds events
		/// are enabled for this component. Hierarchy bounds events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>An <code>HierarchyBoundsListener</code> object is registered
		/// via <code>addHierarchyBoundsListener</code>.
		/// <li>Hierarchy bounds events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the hierarchy event </param>
		/// <seealso cref=         java.awt.event.HierarchyEvent </seealso>
		/// <seealso cref=         java.awt.event.HierarchyBoundsListener </seealso>
		/// <seealso cref=         #addHierarchyBoundsListener </seealso>
		/// <seealso cref=         #enableEvents
		/// @since       1.3 </seealso>
		protected internal virtual void ProcessHierarchyBoundsEvent(HierarchyEvent e)
		{
			HierarchyBoundsListener listener = HierarchyBoundsListener;
			if (listener != null)
			{
				int id = e.ID;
				switch (id)
				{
				  case HierarchyEvent.ANCESTOR_MOVED:
					  listener.AncestorMoved(e);
					  break;
				  case HierarchyEvent.ANCESTOR_RESIZED:
					  listener.AncestorResized(e);
					  break;
				}
			}
		}

		/// @deprecated As of JDK version 1.1
		/// replaced by processEvent(AWTEvent). 
		[Obsolete("As of JDK version 1.1")]
		public virtual bool HandleEvent(Event evt)
		{
			switch (evt.Id)
			{
			  case Event.MOUSE_ENTER:
				  return MouseEnter(evt, evt.x, evt.y);

			  case Event.MOUSE_EXIT:
				  return MouseExit(evt, evt.x, evt.y);

			  case Event.MOUSE_MOVE:
				  return MouseMove(evt, evt.x, evt.y);

			  case Event.MOUSE_DOWN:
				  return MouseDown(evt, evt.x, evt.y);

			  case Event.MOUSE_DRAG:
				  return MouseDrag(evt, evt.x, evt.y);

			  case Event.MOUSE_UP:
				  return MouseUp(evt, evt.x, evt.y);

			  case Event.KEY_PRESS:
			  case Event.KEY_ACTION:
				  return KeyDown(evt, evt.Key);

			  case Event.KEY_RELEASE:
			  case Event.KEY_ACTION_RELEASE:
				  return KeyUp(evt, evt.Key);

			  case Event.ACTION_EVENT:
				  return Action(evt, evt.Arg);
			  case Event.GOT_FOCUS:
				  return GotFocus(evt, evt.Arg);
			  case Event.LOST_FOCUS:
				  return LostFocus(evt, evt.Arg);
			}
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processMouseEvent(MouseEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool MouseDown(Event evt, int x, int y)
		{
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processMouseMotionEvent(MouseEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool MouseDrag(Event evt, int x, int y)
		{
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processMouseEvent(MouseEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool MouseUp(Event evt, int x, int y)
		{
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processMouseMotionEvent(MouseEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool MouseMove(Event evt, int x, int y)
		{
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processMouseEvent(MouseEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool MouseEnter(Event evt, int x, int y)
		{
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processMouseEvent(MouseEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool MouseExit(Event evt, int x, int y)
		{
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processKeyEvent(KeyEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool KeyDown(Event evt, int key)
		{
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processKeyEvent(KeyEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool KeyUp(Event evt, int key)
		{
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// should register this component as ActionListener on component
		/// which fires action events. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool Action(Event evt, Object what)
		{
			return false;
		}

		/// <summary>
		/// Makes this <code>Component</code> displayable by connecting it to a
		/// native screen resource.
		/// This method is called internally by the toolkit and should
		/// not be called directly by programs.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=       #isDisplayable </seealso>
		/// <seealso cref=       #removeNotify </seealso>
		/// <seealso cref= #invalidate
		/// @since JDK1.0 </seealso>
		public virtual void AddNotify()
		{
			lock (TreeLock)
			{
				ComponentPeer peer = this.Peer_Renamed;
				if (peer == null || peer is LightweightPeer)
				{
					if (peer == null)
					{
						// Update both the Component's peer variable and the local
						// variable we use for thread safety.
						this.Peer_Renamed = peer = Toolkit.CreateComponent(this);
					}

					// This is a lightweight component which means it won't be
					// able to get window-related events by itself.  If any
					// have been enabled, then the nearest native container must
					// be enabled.
					if (Parent_Renamed != null)
					{
						long mask = 0;
						if ((MouseListener != null) || ((EventMask & AWTEvent.MOUSE_EVENT_MASK) != 0))
						{
							mask |= AWTEvent.MOUSE_EVENT_MASK;
						}
						if ((MouseMotionListener != null) || ((EventMask & AWTEvent.MOUSE_MOTION_EVENT_MASK) != 0))
						{
							mask |= AWTEvent.MOUSE_MOTION_EVENT_MASK;
						}
						if ((MouseWheelListener != null) || ((EventMask & AWTEvent.MOUSE_WHEEL_EVENT_MASK) != 0))
						{
							mask |= AWTEvent.MOUSE_WHEEL_EVENT_MASK;
						}
						if (FocusListener != null || (EventMask & AWTEvent.FOCUS_EVENT_MASK) != 0)
						{
							mask |= AWTEvent.FOCUS_EVENT_MASK;
						}
						if (KeyListener != null || (EventMask & AWTEvent.KEY_EVENT_MASK) != 0)
						{
							mask |= AWTEvent.KEY_EVENT_MASK;
						}
						if (mask != 0)
						{
							Parent_Renamed.ProxyEnableEvents(mask);
						}
					}
				}
				else
				{
					// It's native. If the parent is lightweight it will need some
					// help.
					Container parent = Container;
					if (parent != null && parent.Lightweight)
					{
						RelocateComponent();
						if (!parent.RecursivelyVisibleUpToHeavyweightContainer)
						{
							peer.Visible = false;
						}
					}
				}
				Invalidate();

				int npopups = (Popups != null? Popups.Count : 0);
				for (int i = 0 ; i < npopups ; i++)
				{
					PopupMenu popup = Popups[i];
					popup.AddNotify();
				}

				if (DropTarget_Renamed != null)
				{
					DropTarget_Renamed.AddNotify(peer);
				}

				PeerFont = Font;

				if (Container != null && !IsAddNotifyComplete)
				{
					Container.IncreaseComponentCount(this);
				}


				// Update stacking order
				UpdateZOrder();

				if (!IsAddNotifyComplete)
				{
					MixOnShowing();
				}

				IsAddNotifyComplete = true;

				if (HierarchyListener != null || (EventMask & AWTEvent.HIERARCHY_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
				{
					HierarchyEvent e = new HierarchyEvent(this, HierarchyEvent.HIERARCHY_CHANGED, this, Parent_Renamed, HierarchyEvent.DISPLAYABILITY_CHANGED | ((RecursivelyVisible) ? HierarchyEvent.SHOWING_CHANGED : 0));
					DispatchEvent(e);
				}
			}
		}

		/// <summary>
		/// Makes this <code>Component</code> undisplayable by destroying it native
		/// screen resource.
		/// <para>
		/// This method is called by the toolkit internally and should
		/// not be called directly by programs. Code overriding
		/// this method should call <code>super.removeNotify</code> as
		/// the first line of the overriding method.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=       #isDisplayable </seealso>
		/// <seealso cref=       #addNotify
		/// @since JDK1.0 </seealso>
		public virtual void RemoveNotify()
		{
			KeyboardFocusManager.ClearMostRecentFocusOwner(this);
			if (KeyboardFocusManager.CurrentKeyboardFocusManager.PermanentFocusOwner == this)
			{
				KeyboardFocusManager.CurrentKeyboardFocusManager.GlobalPermanentFocusOwner = null;
			}

			lock (TreeLock)
			{
				if (FocusOwner && KeyboardFocusManager.IsAutoFocusTransferEnabledFor(this))
				{
					TransferFocus(true);
				}

				if (Container != null && IsAddNotifyComplete)
				{
					Container.DecreaseComponentCount(this);
				}

				int npopups = (Popups != null? Popups.Count : 0);
				for (int i = 0 ; i < npopups ; i++)
				{
					PopupMenu popup = Popups[i];
					popup.RemoveNotify();
				}
				// If there is any input context for this component, notify
				// that this component is being removed. (This has to be done
				// before hiding peer.)
				if ((EventMask & AWTEvent.INPUT_METHODS_ENABLED_MASK) != 0)
				{
					InputContext inputContext = InputContext;
					if (inputContext != null)
					{
						inputContext.RemoveNotify(this);
					}
				}

				ComponentPeer p = Peer_Renamed;
				if (p != null)
				{
					bool isLightweight = Lightweight;

					if (BufferStrategy_Renamed is FlipBufferStrategy)
					{
						((FlipBufferStrategy)BufferStrategy_Renamed).DestroyBuffers();
					}

					if (DropTarget_Renamed != null)
					{
						DropTarget_Renamed.RemoveNotify(Peer_Renamed);
					}

					// Hide peer first to stop system events such as cursor moves.
					if (Visible_Renamed)
					{
						p.Visible = false;
					}

					Peer_Renamed = null; // Stop peer updates.
					PeerFont = null;

					Toolkit.EventQueue.RemoveSourceEvents(this, false);
					KeyboardFocusManager.CurrentKeyboardFocusManager.DiscardKeyEvents(this);

					p.Dispose();

					MixOnHiding(isLightweight);

					IsAddNotifyComplete = false;
					// Nullifying compoundShape means that the component has normal shape
					// (or has no shape at all).
					this.CompoundShape = null;
				}

				if (HierarchyListener != null || (EventMask & AWTEvent.HIERARCHY_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
				{
					HierarchyEvent e = new HierarchyEvent(this, HierarchyEvent.HIERARCHY_CHANGED, this, Parent_Renamed, HierarchyEvent.DISPLAYABILITY_CHANGED | ((RecursivelyVisible) ? HierarchyEvent.SHOWING_CHANGED : 0));
					DispatchEvent(e);
				}
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processFocusEvent(FocusEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool GotFocus(Event evt, Object what)
		{
			return false;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by processFocusEvent(FocusEvent). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool LostFocus(Event evt, Object what)
		{
			return false;
		}

		/// <summary>
		/// Returns whether this <code>Component</code> can become the focus
		/// owner.
		/// </summary>
		/// <returns> <code>true</code> if this <code>Component</code> is
		/// focusable; <code>false</code> otherwise </returns>
		/// <seealso cref= #setFocusable
		/// @since JDK1.1 </seealso>
		/// @deprecated As of 1.4, replaced by <code>isFocusable()</code>. 
		[Obsolete("As of 1.4, replaced by <code>isFocusable()</code>.")]
		public virtual bool FocusTraversable
		{
			get
			{
				if (IsFocusTraversableOverridden == FOCUS_TRAVERSABLE_UNKNOWN)
				{
					IsFocusTraversableOverridden = FOCUS_TRAVERSABLE_DEFAULT;
				}
				return Focusable_Renamed;
			}
		}

		/// <summary>
		/// Returns whether this Component can be focused.
		/// </summary>
		/// <returns> <code>true</code> if this Component is focusable;
		///         <code>false</code> otherwise. </returns>
		/// <seealso cref= #setFocusable
		/// @since 1.4 </seealso>
		public virtual bool Focusable
		{
			get
			{
				return FocusTraversable;
			}
			set
			{
				bool oldFocusable;
				lock (this)
				{
					oldFocusable = this.Focusable_Renamed;
					this.Focusable_Renamed = value;
				}
				IsFocusTraversableOverridden = FOCUS_TRAVERSABLE_SET;
    
				FirePropertyChange("focusable", oldFocusable, value);
				if (oldFocusable && !value)
				{
					if (FocusOwner && KeyboardFocusManager.AutoFocusTransferEnabled)
					{
						TransferFocus(true);
					}
					KeyboardFocusManager.ClearMostRecentFocusOwner(this);
				}
			}
		}


		internal bool FocusTraversableOverridden
		{
			get
			{
				return (IsFocusTraversableOverridden != FOCUS_TRAVERSABLE_DEFAULT);
			}
		}

		/// <summary>
		/// Sets the focus traversal keys for a given traversal operation for this
		/// Component.
		/// <para>
		/// The default values for a Component's focus traversal keys are
		/// implementation-dependent. Sun recommends that all implementations for a
		/// particular native platform use the same default values. The
		/// recommendations for Windows and Unix are listed below. These
		/// recommendations are used in the Sun AWT implementations.
		/// 
		/// <table border=1 summary="Recommended default values for a Component's focus traversal keys">
		/// <tr>
		///    <th>Identifier</th>
		///    <th>Meaning</th>
		///    <th>Default</th>
		/// </tr>
		/// <tr>
		///    <td>KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS</td>
		///    <td>Normal forward keyboard traversal</td>
		///    <td>TAB on KEY_PRESSED, CTRL-TAB on KEY_PRESSED</td>
		/// </tr>
		/// <tr>
		///    <td>KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS</td>
		///    <td>Normal reverse keyboard traversal</td>
		///    <td>SHIFT-TAB on KEY_PRESSED, CTRL-SHIFT-TAB on KEY_PRESSED</td>
		/// </tr>
		/// <tr>
		///    <td>KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS</td>
		///    <td>Go up one focus traversal cycle</td>
		///    <td>none</td>
		/// </tr>
		/// </table>
		/// 
		/// To disable a traversal key, use an empty Set; Collections.EMPTY_SET is
		/// recommended.
		/// </para>
		/// <para>
		/// Using the AWTKeyStroke API, client code can specify on which of two
		/// specific KeyEvents, KEY_PRESSED or KEY_RELEASED, the focus traversal
		/// operation will occur. Regardless of which KeyEvent is specified,
		/// however, all KeyEvents related to the focus traversal key, including the
		/// associated KEY_TYPED event, will be consumed, and will not be dispatched
		/// to any Component. It is a runtime error to specify a KEY_TYPED event as
		/// mapping to a focus traversal operation, or to map the same event to
		/// multiple default focus traversal operations.
		/// </para>
		/// <para>
		/// If a value of null is specified for the Set, this Component inherits the
		/// Set from its parent. If all ancestors of this Component have null
		/// specified for the Set, then the current KeyboardFocusManager's default
		/// Set is used.
		/// </para>
		/// <para>
		/// This method may throw a {@code ClassCastException} if any {@code Object}
		/// in {@code keystrokes} is not an {@code AWTKeyStroke}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
		///        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS </param>
		/// <param name="keystrokes"> the Set of AWTKeyStroke for the specified operation </param>
		/// <seealso cref= #getFocusTraversalKeys </seealso>
		/// <seealso cref= KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
		/// <exception cref="IllegalArgumentException"> if id is not one of
		///         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
		///         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or if keystrokes
		///         contains null, or if any keystroke represents a KEY_TYPED event,
		///         or if any keystroke already maps to another focus traversal
		///         operation for this Component
		/// @since 1.4
		/// @beaninfo
		///       bound: true </exception>
		public virtual void setFocusTraversalKeys<T1>(int id, Set<T1> keystrokes) where T1 : AWTKeyStroke
		{
			if (id < 0 || id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH - 1)
			{
				throw new IllegalArgumentException("invalid focus traversal key identifier");
			}

			SetFocusTraversalKeys_NoIDCheck(id, keystrokes);
		}

		/// <summary>
		/// Returns the Set of focus traversal keys for a given traversal operation
		/// for this Component. (See
		/// <code>setFocusTraversalKeys</code> for a full description of each key.)
		/// <para>
		/// If a Set of traversal keys has not been explicitly defined for this
		/// Component, then this Component's parent's Set is returned. If no Set
		/// has been explicitly defined for any of this Component's ancestors, then
		/// the current KeyboardFocusManager's default Set is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
		///        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS </param>
		/// <returns> the Set of AWTKeyStrokes for the specified operation. The Set
		///         will be unmodifiable, and may be empty. null will never be
		///         returned. </returns>
		/// <seealso cref= #setFocusTraversalKeys </seealso>
		/// <seealso cref= KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
		/// <exception cref="IllegalArgumentException"> if id is not one of
		///         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
		///         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS
		/// @since 1.4 </exception>
		public virtual Set<AWTKeyStroke> GetFocusTraversalKeys(int id)
		{
			if (id < 0 || id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH - 1)
			{
				throw new IllegalArgumentException("invalid focus traversal key identifier");
			}

			return GetFocusTraversalKeys_NoIDCheck(id);
		}

		// We define these methods so that Container does not need to repeat this
		// code. Container cannot call super.<method> because Container allows
		// DOWN_CYCLE_TRAVERSAL_KEY while Component does not. The Component method
		// would erroneously generate an IllegalArgumentException for
		// DOWN_CYCLE_TRAVERSAL_KEY.
		internal void setFocusTraversalKeys_NoIDCheck<T1>(int id, Set<T1> keystrokes) where T1 : AWTKeyStroke
		{
			Set<AWTKeyStroke> oldKeys;

			lock (this)
			{
				if (FocusTraversalKeys == null)
				{
					InitializeFocusTraversalKeys();
				}

				if (keystrokes != null)
				{
					foreach (AWTKeyStroke keystroke in keystrokes)
					{

						if (keystroke == null)
						{
							throw new IllegalArgumentException("cannot set null focus traversal key");
						}

						if (keystroke.KeyChar != KeyEvent.CHAR_UNDEFINED)
						{
							throw new IllegalArgumentException("focus traversal keys cannot map to KEY_TYPED events");
						}

						for (int i = 0; i < FocusTraversalKeys.Length; i++)
						{
							if (i == id)
							{
								continue;
							}

							if (GetFocusTraversalKeys_NoIDCheck(i).Contains(keystroke))
							{
								throw new IllegalArgumentException("focus traversal keys must be unique for a Component");
							}
						}
					}
				}

				oldKeys = FocusTraversalKeys[id];
				FocusTraversalKeys[id] = (keystrokes != null) ? Collections.UnmodifiableSet(new HashSet<AWTKeyStroke>(keystrokes)) : null;
			}

			FirePropertyChange(FocusTraversalKeyPropertyNames[id], oldKeys, keystrokes);
		}
		internal Set<AWTKeyStroke> GetFocusTraversalKeys_NoIDCheck(int id)
		{
			// Okay to return Set directly because it is an unmodifiable view
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Set<AWTKeyStroke> keystrokes = (focusTraversalKeys != null) ? focusTraversalKeys[id] : null;
			Set<AWTKeyStroke> keystrokes = (FocusTraversalKeys != null) ? FocusTraversalKeys[id] : null;

			if (keystrokes != null)
			{
				return keystrokes;
			}
			else
			{
				Container parent = this.Parent_Renamed;
				if (parent != null)
				{
					return parent.GetFocusTraversalKeys(id);
				}
				else
				{
					return KeyboardFocusManager.CurrentKeyboardFocusManager.GetDefaultFocusTraversalKeys(id);
				}
			}
		}

		/// <summary>
		/// Returns whether the Set of focus traversal keys for the given focus
		/// traversal operation has been explicitly defined for this Component. If
		/// this method returns <code>false</code>, this Component is inheriting the
		/// Set from an ancestor, or from the current KeyboardFocusManager.
		/// </summary>
		/// <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
		///        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS </param>
		/// <returns> <code>true</code> if the the Set of focus traversal keys for the
		///         given focus traversal operation has been explicitly defined for
		///         this Component; <code>false</code> otherwise. </returns>
		/// <exception cref="IllegalArgumentException"> if id is not one of
		///         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
		///         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS
		/// @since 1.4 </exception>
		public virtual bool AreFocusTraversalKeysSet(int id)
		{
			if (id < 0 || id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH - 1)
			{
				throw new IllegalArgumentException("invalid focus traversal key identifier");
			}

			return (FocusTraversalKeys != null && FocusTraversalKeys[id] != null);
		}

		/// <summary>
		/// Sets whether focus traversal keys are enabled for this Component.
		/// Components for which focus traversal keys are disabled receive key
		/// events for focus traversal keys. Components for which focus traversal
		/// keys are enabled do not see these events; instead, the events are
		/// automatically converted to traversal operations.
		/// </summary>
		/// <param name="focusTraversalKeysEnabled"> whether focus traversal keys are
		///        enabled for this Component </param>
		/// <seealso cref= #getFocusTraversalKeysEnabled </seealso>
		/// <seealso cref= #setFocusTraversalKeys </seealso>
		/// <seealso cref= #getFocusTraversalKeys
		/// @since 1.4
		/// @beaninfo
		///       bound: true </seealso>
		public virtual bool FocusTraversalKeysEnabled
		{
			set
			{
				bool oldFocusTraversalKeysEnabled;
				lock (this)
				{
					oldFocusTraversalKeysEnabled = this.FocusTraversalKeysEnabled_Renamed;
					this.FocusTraversalKeysEnabled_Renamed = value;
				}
				FirePropertyChange("focusTraversalKeysEnabled", oldFocusTraversalKeysEnabled, value);
			}
			get
			{
				return FocusTraversalKeysEnabled_Renamed;
			}
		}


		/// <summary>
		/// Requests that this Component get the input focus, and that this
		/// Component's top-level ancestor become the focused Window. This
		/// component must be displayable, focusable, visible and all of
		/// its ancestors (with the exception of the top-level Window) must
		/// be visible for the request to be granted. Every effort will be
		/// made to honor the request; however, in some cases it may be
		/// impossible to do so. Developers must never assume that this
		/// Component is the focus owner until this Component receives a
		/// FOCUS_GAINED event. If this request is denied because this
		/// Component's top-level Window cannot become the focused Window,
		/// the request will be remembered and will be granted when the
		/// Window is later focused by the user.
		/// <para>
		/// This method cannot be used to set the focus owner to no Component at
		/// all. Use <code>KeyboardFocusManager.clearGlobalFocusOwner()</code>
		/// instead.
		/// </para>
		/// <para>
		/// Because the focus behavior of this method is platform-dependent,
		/// developers are strongly encouraged to use
		/// <code>requestFocusInWindow</code> when possible.
		/// 
		/// </para>
		/// <para>Note: Not all focus transfers result from invoking this method. As
		/// such, a component may receive focus without this or any of the other
		/// {@code requestFocus} methods of {@code Component} being invoked.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #requestFocusInWindow </seealso>
		/// <seealso cref= java.awt.event.FocusEvent </seealso>
		/// <seealso cref= #addFocusListener </seealso>
		/// <seealso cref= #isFocusable </seealso>
		/// <seealso cref= #isDisplayable </seealso>
		/// <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner
		/// @since JDK1.0 </seealso>
		public virtual void RequestFocus()
		{
			RequestFocusHelper(false, true);
		}

		internal virtual bool RequestFocus(CausedFocusEvent.Cause cause)
		{
			return RequestFocusHelper(false, true, cause);
		}

		/// <summary>
		/// Requests that this <code>Component</code> get the input focus,
		/// and that this <code>Component</code>'s top-level ancestor
		/// become the focused <code>Window</code>. This component must be
		/// displayable, focusable, visible and all of its ancestors (with
		/// the exception of the top-level Window) must be visible for the
		/// request to be granted. Every effort will be made to honor the
		/// request; however, in some cases it may be impossible to do
		/// so. Developers must never assume that this component is the
		/// focus owner until this component receives a FOCUS_GAINED
		/// event. If this request is denied because this component's
		/// top-level window cannot become the focused window, the request
		/// will be remembered and will be granted when the window is later
		/// focused by the user.
		/// <para>
		/// This method returns a boolean value. If <code>false</code> is returned,
		/// the request is <b>guaranteed to fail</b>. If <code>true</code> is
		/// returned, the request will succeed <b>unless</b> it is vetoed, or an
		/// extraordinary event, such as disposal of the component's peer, occurs
		/// before the request can be granted by the native windowing system. Again,
		/// while a return value of <code>true</code> indicates that the request is
		/// likely to succeed, developers must never assume that this component is
		/// the focus owner until this component receives a FOCUS_GAINED event.
		/// </para>
		/// <para>
		/// This method cannot be used to set the focus owner to no component at
		/// all. Use <code>KeyboardFocusManager.clearGlobalFocusOwner</code>
		/// instead.
		/// </para>
		/// <para>
		/// Because the focus behavior of this method is platform-dependent,
		/// developers are strongly encouraged to use
		/// <code>requestFocusInWindow</code> when possible.
		/// </para>
		/// <para>
		/// Every effort will be made to ensure that <code>FocusEvent</code>s
		/// generated as a
		/// result of this request will have the specified temporary value. However,
		/// because specifying an arbitrary temporary state may not be implementable
		/// on all native windowing systems, correct behavior for this method can be
		/// guaranteed only for lightweight <code>Component</code>s.
		/// This method is not intended
		/// for general use, but exists instead as a hook for lightweight component
		/// libraries, such as Swing.
		/// 
		/// </para>
		/// <para>Note: Not all focus transfers result from invoking this method. As
		/// such, a component may receive focus without this or any of the other
		/// {@code requestFocus} methods of {@code Component} being invoked.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporary"> true if the focus change is temporary,
		///        such as when the window loses the focus; for
		///        more information on temporary focus changes see the
		/// <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a> </param>
		/// <returns> <code>false</code> if the focus change request is guaranteed to
		///         fail; <code>true</code> if it is likely to succeed </returns>
		/// <seealso cref= java.awt.event.FocusEvent </seealso>
		/// <seealso cref= #addFocusListener </seealso>
		/// <seealso cref= #isFocusable </seealso>
		/// <seealso cref= #isDisplayable </seealso>
		/// <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner
		/// @since 1.4 </seealso>
		protected internal virtual bool RequestFocus(bool temporary)
		{
			return RequestFocusHelper(temporary, true);
		}

		internal virtual bool RequestFocus(bool temporary, CausedFocusEvent.Cause cause)
		{
			return RequestFocusHelper(temporary, true, cause);
		}
		/// <summary>
		/// Requests that this Component get the input focus, if this
		/// Component's top-level ancestor is already the focused
		/// Window. This component must be displayable, focusable, visible
		/// and all of its ancestors (with the exception of the top-level
		/// Window) must be visible for the request to be granted. Every
		/// effort will be made to honor the request; however, in some
		/// cases it may be impossible to do so. Developers must never
		/// assume that this Component is the focus owner until this
		/// Component receives a FOCUS_GAINED event.
		/// <para>
		/// This method returns a boolean value. If <code>false</code> is returned,
		/// the request is <b>guaranteed to fail</b>. If <code>true</code> is
		/// returned, the request will succeed <b>unless</b> it is vetoed, or an
		/// extraordinary event, such as disposal of the Component's peer, occurs
		/// before the request can be granted by the native windowing system. Again,
		/// while a return value of <code>true</code> indicates that the request is
		/// likely to succeed, developers must never assume that this Component is
		/// the focus owner until this Component receives a FOCUS_GAINED event.
		/// </para>
		/// <para>
		/// This method cannot be used to set the focus owner to no Component at
		/// all. Use <code>KeyboardFocusManager.clearGlobalFocusOwner()</code>
		/// instead.
		/// </para>
		/// <para>
		/// The focus behavior of this method can be implemented uniformly across
		/// platforms, and thus developers are strongly encouraged to use this
		/// method over <code>requestFocus</code> when possible. Code which relies
		/// on <code>requestFocus</code> may exhibit different focus behavior on
		/// different platforms.
		/// 
		/// </para>
		/// <para>Note: Not all focus transfers result from invoking this method. As
		/// such, a component may receive focus without this or any of the other
		/// {@code requestFocus} methods of {@code Component} being invoked.
		/// 
		/// </para>
		/// </summary>
		/// <returns> <code>false</code> if the focus change request is guaranteed to
		///         fail; <code>true</code> if it is likely to succeed </returns>
		/// <seealso cref= #requestFocus </seealso>
		/// <seealso cref= java.awt.event.FocusEvent </seealso>
		/// <seealso cref= #addFocusListener </seealso>
		/// <seealso cref= #isFocusable </seealso>
		/// <seealso cref= #isDisplayable </seealso>
		/// <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner
		/// @since 1.4 </seealso>
		public virtual bool RequestFocusInWindow()
		{
			return RequestFocusHelper(false, false);
		}

		internal virtual bool RequestFocusInWindow(CausedFocusEvent.Cause cause)
		{
			return RequestFocusHelper(false, false, cause);
		}

		/// <summary>
		/// Requests that this <code>Component</code> get the input focus,
		/// if this <code>Component</code>'s top-level ancestor is already
		/// the focused <code>Window</code>.  This component must be
		/// displayable, focusable, visible and all of its ancestors (with
		/// the exception of the top-level Window) must be visible for the
		/// request to be granted. Every effort will be made to honor the
		/// request; however, in some cases it may be impossible to do
		/// so. Developers must never assume that this component is the
		/// focus owner until this component receives a FOCUS_GAINED event.
		/// <para>
		/// This method returns a boolean value. If <code>false</code> is returned,
		/// the request is <b>guaranteed to fail</b>. If <code>true</code> is
		/// returned, the request will succeed <b>unless</b> it is vetoed, or an
		/// extraordinary event, such as disposal of the component's peer, occurs
		/// before the request can be granted by the native windowing system. Again,
		/// while a return value of <code>true</code> indicates that the request is
		/// likely to succeed, developers must never assume that this component is
		/// the focus owner until this component receives a FOCUS_GAINED event.
		/// </para>
		/// <para>
		/// This method cannot be used to set the focus owner to no component at
		/// all. Use <code>KeyboardFocusManager.clearGlobalFocusOwner</code>
		/// instead.
		/// </para>
		/// <para>
		/// The focus behavior of this method can be implemented uniformly across
		/// platforms, and thus developers are strongly encouraged to use this
		/// method over <code>requestFocus</code> when possible. Code which relies
		/// on <code>requestFocus</code> may exhibit different focus behavior on
		/// different platforms.
		/// </para>
		/// <para>
		/// Every effort will be made to ensure that <code>FocusEvent</code>s
		/// generated as a
		/// result of this request will have the specified temporary value. However,
		/// because specifying an arbitrary temporary state may not be implementable
		/// on all native windowing systems, correct behavior for this method can be
		/// guaranteed only for lightweight components. This method is not intended
		/// for general use, but exists instead as a hook for lightweight component
		/// libraries, such as Swing.
		/// 
		/// </para>
		/// <para>Note: Not all focus transfers result from invoking this method. As
		/// such, a component may receive focus without this or any of the other
		/// {@code requestFocus} methods of {@code Component} being invoked.
		/// 
		/// </para>
		/// </summary>
		/// <param name="temporary"> true if the focus change is temporary,
		///        such as when the window loses the focus; for
		///        more information on temporary focus changes see the
		/// <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a> </param>
		/// <returns> <code>false</code> if the focus change request is guaranteed to
		///         fail; <code>true</code> if it is likely to succeed </returns>
		/// <seealso cref= #requestFocus </seealso>
		/// <seealso cref= java.awt.event.FocusEvent </seealso>
		/// <seealso cref= #addFocusListener </seealso>
		/// <seealso cref= #isFocusable </seealso>
		/// <seealso cref= #isDisplayable </seealso>
		/// <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner
		/// @since 1.4 </seealso>
		protected internal virtual bool RequestFocusInWindow(bool temporary)
		{
			return RequestFocusHelper(temporary, false);
		}

		internal virtual bool RequestFocusInWindow(bool temporary, CausedFocusEvent.Cause cause)
		{
			return RequestFocusHelper(temporary, false, cause);
		}

		internal bool RequestFocusHelper(bool temporary, bool focusedWindowChangeAllowed)
		{
			return RequestFocusHelper(temporary, focusedWindowChangeAllowed, CausedFocusEvent.Cause.UNKNOWN);
		}

		internal bool RequestFocusHelper(bool temporary, bool focusedWindowChangeAllowed, CausedFocusEvent.Cause cause)
		{
			// 1) Check if the event being dispatched is a system-generated mouse event.
			AWTEvent currentEvent = EventQueue.CurrentEvent;
			if (currentEvent is MouseEvent && SunToolkit.isSystemGenerated(currentEvent))
			{
				// 2) Sanity check: if the mouse event component source belongs to the same containing window.
				Component source = ((MouseEvent)currentEvent).Component;
				if (source == null || source.ContainingWindow == ContainingWindow)
				{
					FocusLog.finest("requesting focus by mouse event \"in window\"");

					// If both the conditions are fulfilled the focus request should be strictly
					// bounded by the toplevel window. It's assumed that the mouse event activates
					// the window (if it wasn't active) and this makes it possible for a focus
					// request with a strong in-window requirement to change focus in the bounds
					// of the toplevel. If, by any means, due to asynchronous nature of the event
					// dispatching mechanism, the window happens to be natively inactive by the time
					// this focus request is eventually handled, it should not re-activate the
					// toplevel. Otherwise the result may not meet user expectations. See 6981400.
					focusedWindowChangeAllowed = false;
				}
			}
			if (!IsRequestFocusAccepted(temporary, focusedWindowChangeAllowed, cause))
			{
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("requestFocus is not accepted");
				}
				return false;
			}
			// Update most-recent map
			KeyboardFocusManager.MostRecentFocusOwner = this;

			Component window = this;
			while ((window != null) && !(window is Window))
			{
				if (!window.Visible)
				{
					if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
					{
						FocusLog.finest("component is recurively invisible");
					}
					return false;
				}
				window = window.Parent_Renamed;
			}

			ComponentPeer peer = this.Peer_Renamed;
			Component heavyweight = (peer is LightweightPeer) ? NativeContainer : this;
			if (heavyweight == null || !heavyweight.Visible)
			{
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("Component is not a part of visible hierarchy");
				}
				return false;
			}
			peer = heavyweight.Peer_Renamed;
			if (peer == null)
			{
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("Peer is null");
				}
				return false;
			}

			// Focus this Component
			long time = 0;
			if (EventQueue.DispatchThread)
			{
				time = Toolkit.EventQueue.MostRecentKeyEventTime;
			}
			else
			{
				// A focus request made from outside EDT should not be associated with any event
				// and so its time stamp is simply set to the current time.
				time = DateTimeHelperClass.CurrentUnixTimeMillis();
			}

			bool success = peer.RequestFocus(this, temporary, focusedWindowChangeAllowed, time, cause);
			if (!success)
			{
				KeyboardFocusManager.GetCurrentKeyboardFocusManager(AppContext).DequeueKeyEvents(time, this);
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("Peer request failed");
				}
			}
			else
			{
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("Pass for " + this);
				}
			}
			return success;
		}

		private bool IsRequestFocusAccepted(bool temporary, bool focusedWindowChangeAllowed, CausedFocusEvent.Cause cause)
		{
			if (!Focusable || !Visible)
			{
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("Not focusable or not visible");
				}
				return false;
			}

			ComponentPeer peer = this.Peer_Renamed;
			if (peer == null)
			{
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("peer is null");
				}
				return false;
			}

			Window window = ContainingWindow;
			if (window == null || !window.FocusableWindow)
			{
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("Component doesn't have toplevel");
				}
				return false;
			}

			// We have passed all regular checks for focus request,
			// now let's call RequestFocusController and see what it says.
			Component focusOwner = KeyboardFocusManager.GetMostRecentFocusOwner(window);
			if (focusOwner == null)
			{
				// sometimes most recent focus owner may be null, but focus owner is not
				// e.g. we reset most recent focus owner if user removes focus owner
				focusOwner = KeyboardFocusManager.CurrentKeyboardFocusManager.FocusOwner;
				if (focusOwner != null && focusOwner.ContainingWindow != window)
				{
					focusOwner = null;
				}
			}

			if (focusOwner == this || focusOwner == null)
			{
				// Controller is supposed to verify focus transfers and for this it
				// should know both from and to components.  And it shouldn't verify
				// transfers from when these components are equal.
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("focus owner is null or this");
				}
				return true;
			}

			if (CausedFocusEvent.Cause.ACTIVATION == cause)
			{
				// we shouldn't call RequestFocusController in case we are
				// in activation.  We do request focus on component which
				// has got temporary focus lost and then on component which is
				// most recent focus owner.  But most recent focus owner can be
				// changed by requestFocsuXXX() call only, so this transfer has
				// been already approved.
				if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
				{
					FocusLog.finest("cause is activation");
				}
				return true;
			}

			bool ret = Component.RequestFocusController_Renamed.acceptRequestFocus(focusOwner, this, temporary, focusedWindowChangeAllowed, cause);
			if (FocusLog.isLoggable(PlatformLogger.Level.FINEST))
			{
				FocusLog.finest("RequestFocusController returns {0}", ret);
			}

			return ret;
		}

		private static RequestFocusController RequestFocusController_Renamed = new DummyRequestFocusController();

		// Swing access this method through reflection to implement InputVerifier's functionality.
		// Perhaps, we should make this method public (later ;)
		private class DummyRequestFocusController : RequestFocusController
		{
			public virtual bool AcceptRequestFocus(Component from, Component to, bool temporary, bool focusedWindowChangeAllowed, CausedFocusEvent.Cause cause)
			{
				return true;
			}
		}

		internal static RequestFocusController RequestFocusController
		{
			set
			{
				lock (typeof(Component))
				{
					if (value == null)
					{
						RequestFocusController_Renamed = new DummyRequestFocusController();
					}
					else
					{
						RequestFocusController_Renamed = value;
					}
				}
			}
		}

		/// <summary>
		/// Returns the Container which is the focus cycle root of this Component's
		/// focus traversal cycle. Each focus traversal cycle has only a single
		/// focus cycle root and each Component which is not a Container belongs to
		/// only a single focus traversal cycle. Containers which are focus cycle
		/// roots belong to two cycles: one rooted at the Container itself, and one
		/// rooted at the Container's nearest focus-cycle-root ancestor. For such
		/// Containers, this method will return the Container's nearest focus-cycle-
		/// root ancestor.
		/// </summary>
		/// <returns> this Component's nearest focus-cycle-root ancestor </returns>
		/// <seealso cref= Container#isFocusCycleRoot()
		/// @since 1.4 </seealso>
		public virtual Container FocusCycleRootAncestor
		{
			get
			{
				Container rootAncestor = this.Parent_Renamed;
				while (rootAncestor != null && !rootAncestor.FocusCycleRoot)
				{
					rootAncestor = rootAncestor.Parent_Renamed;
				}
				return rootAncestor;
			}
		}

		/// <summary>
		/// Returns whether the specified Container is the focus cycle root of this
		/// Component's focus traversal cycle. Each focus traversal cycle has only
		/// a single focus cycle root and each Component which is not a Container
		/// belongs to only a single focus traversal cycle.
		/// </summary>
		/// <param name="container"> the Container to be tested </param>
		/// <returns> <code>true</code> if the specified Container is a focus-cycle-
		///         root of this Component; <code>false</code> otherwise </returns>
		/// <seealso cref= Container#isFocusCycleRoot()
		/// @since 1.4 </seealso>
		public virtual bool IsFocusCycleRoot(Container container)
		{
			Container rootAncestor = FocusCycleRootAncestor;
			return (rootAncestor == container);
		}

		internal virtual Container TraversalRoot
		{
			get
			{
				return FocusCycleRootAncestor;
			}
		}

		/// <summary>
		/// Transfers the focus to the next component, as though this Component were
		/// the focus owner. </summary>
		/// <seealso cref=       #requestFocus()
		/// @since     JDK1.1 </seealso>
		public virtual void TransferFocus()
		{
			NextFocus();
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by transferFocus(). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void NextFocus()
		{
			TransferFocus(false);
		}

		internal virtual bool TransferFocus(bool clearOnFailure)
		{
			if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
			{
				FocusLog.finer("clearOnFailure = " + clearOnFailure);
			}
			Component toFocus = NextFocusCandidate;
			bool res = false;
			if (toFocus != null && !toFocus.FocusOwner && toFocus != this)
			{
				res = toFocus.RequestFocusInWindow(CausedFocusEvent.Cause.TRAVERSAL_FORWARD);
			}
			if (clearOnFailure && !res)
			{
				if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
				{
					FocusLog.finer("clear global focus owner");
				}
				KeyboardFocusManager.CurrentKeyboardFocusManager.ClearGlobalFocusOwnerPriv();
			}
			if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
			{
				FocusLog.finer("returning result: " + res);
			}
			return res;
		}

		internal Component NextFocusCandidate
		{
			get
			{
				Container rootAncestor = TraversalRoot;
				Component comp = this;
				while (rootAncestor != null && !(rootAncestor.Showing && rootAncestor.CanBeFocusOwner()))
				{
					comp = rootAncestor;
					rootAncestor = comp.FocusCycleRootAncestor;
				}
				if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
				{
					FocusLog.finer("comp = " + comp + ", root = " + rootAncestor);
				}
				Component candidate = null;
				if (rootAncestor != null)
				{
					FocusTraversalPolicy policy = rootAncestor.FocusTraversalPolicy;
					Component toFocus = policy.GetComponentAfter(rootAncestor, comp);
					if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
					{
						FocusLog.finer("component after is " + toFocus);
					}
					if (toFocus == null)
					{
						toFocus = policy.GetDefaultComponent(rootAncestor);
						if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
						{
							FocusLog.finer("default component is " + toFocus);
						}
					}
					if (toFocus == null)
					{
						Applet applet = EmbeddedFrame.getAppletIfAncestorOf(this);
						if (applet != null)
						{
							toFocus = applet;
						}
					}
					candidate = toFocus;
				}
				if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
				{
					FocusLog.finer("Focus transfer candidate: " + candidate);
				}
				return candidate;
			}
		}

		/// <summary>
		/// Transfers the focus to the previous component, as though this Component
		/// were the focus owner. </summary>
		/// <seealso cref=       #requestFocus()
		/// @since     1.4 </seealso>
		public virtual void TransferFocusBackward()
		{
			TransferFocusBackward(false);
		}

		internal virtual bool TransferFocusBackward(bool clearOnFailure)
		{
			Container rootAncestor = TraversalRoot;
			Component comp = this;
			while (rootAncestor != null && !(rootAncestor.Showing && rootAncestor.CanBeFocusOwner()))
			{
				comp = rootAncestor;
				rootAncestor = comp.FocusCycleRootAncestor;
			}
			bool res = false;
			if (rootAncestor != null)
			{
				FocusTraversalPolicy policy = rootAncestor.FocusTraversalPolicy;
				Component toFocus = policy.GetComponentBefore(rootAncestor, comp);
				if (toFocus == null)
				{
					toFocus = policy.GetDefaultComponent(rootAncestor);
				}
				if (toFocus != null)
				{
					res = toFocus.RequestFocusInWindow(CausedFocusEvent.Cause.TRAVERSAL_BACKWARD);
				}
			}
			if (clearOnFailure && !res)
			{
				if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
				{
					FocusLog.finer("clear global focus owner");
				}
				KeyboardFocusManager.CurrentKeyboardFocusManager.ClearGlobalFocusOwnerPriv();
			}
			if (FocusLog.isLoggable(PlatformLogger.Level.FINER))
			{
				FocusLog.finer("returning result: " + res);
			}
			return res;
		}

		/// <summary>
		/// Transfers the focus up one focus traversal cycle. Typically, the focus
		/// owner is set to this Component's focus cycle root, and the current focus
		/// cycle root is set to the new focus owner's focus cycle root. If,
		/// however, this Component's focus cycle root is a Window, then the focus
		/// owner is set to the focus cycle root's default Component to focus, and
		/// the current focus cycle root is unchanged.
		/// </summary>
		/// <seealso cref=       #requestFocus() </seealso>
		/// <seealso cref=       Container#isFocusCycleRoot() </seealso>
		/// <seealso cref=       Container#setFocusCycleRoot(boolean)
		/// @since     1.4 </seealso>
		public virtual void TransferFocusUpCycle()
		{
			Container rootAncestor;
			for (rootAncestor = FocusCycleRootAncestor; rootAncestor != null && !(rootAncestor.Showing && rootAncestor.Focusable && rootAncestor.Enabled); rootAncestor = rootAncestor.FocusCycleRootAncestor)
			{
			}

			if (rootAncestor != null)
			{
				Container rootAncestorRootAncestor = rootAncestor.FocusCycleRootAncestor;
				Container fcr = (rootAncestorRootAncestor != null) ? rootAncestorRootAncestor : rootAncestor;

				KeyboardFocusManager.CurrentKeyboardFocusManager.GlobalCurrentFocusCycleRootPriv = fcr;
				rootAncestor.RequestFocus(CausedFocusEvent.Cause.TRAVERSAL_UP);
			}
			else
			{
				Window window = ContainingWindow;

				if (window != null)
				{
					Component toFocus = window.FocusTraversalPolicy.GetDefaultComponent(window);
					if (toFocus != null)
					{
						KeyboardFocusManager.CurrentKeyboardFocusManager.GlobalCurrentFocusCycleRootPriv = window;
						toFocus.RequestFocus(CausedFocusEvent.Cause.TRAVERSAL_UP);
					}
				}
			}
		}

		/// <summary>
		/// Returns <code>true</code> if this <code>Component</code> is the
		/// focus owner.  This method is obsolete, and has been replaced by
		/// <code>isFocusOwner()</code>.
		/// </summary>
		/// <returns> <code>true</code> if this <code>Component</code> is the
		///         focus owner; <code>false</code> otherwise
		/// @since 1.2 </returns>
		public virtual bool HasFocus()
		{
			return (KeyboardFocusManager.CurrentKeyboardFocusManager.FocusOwner == this);
		}

		/// <summary>
		/// Returns <code>true</code> if this <code>Component</code> is the
		///    focus owner.
		/// </summary>
		/// <returns> <code>true</code> if this <code>Component</code> is the
		///     focus owner; <code>false</code> otherwise
		/// @since 1.4 </returns>
		public virtual bool FocusOwner
		{
			get
			{
				return HasFocus();
			}
		}

		/*
		 * Used to disallow auto-focus-transfer on disposal of the focus owner
		 * in the process of disposing its parent container.
		 */
		private bool AutoFocusTransferOnDisposal_Renamed = true;

		internal virtual bool AutoFocusTransferOnDisposal
		{
			set
			{
				AutoFocusTransferOnDisposal_Renamed = value;
			}
			get
			{
				return AutoFocusTransferOnDisposal_Renamed;
			}
		}


		/// <summary>
		/// Adds the specified popup menu to the component. </summary>
		/// <param name="popup"> the popup menu to be added to the component. </param>
		/// <seealso cref=       #remove(MenuComponent) </seealso>
		/// <exception cref="NullPointerException"> if {@code popup} is {@code null}
		/// @since     JDK1.1 </exception>
		public virtual void Add(PopupMenu popup)
		{
			lock (TreeLock)
			{
				if (popup.Parent_Renamed != null)
				{
					popup.Parent_Renamed.remove(popup);
				}
				if (Popups == null)
				{
					Popups = new List<PopupMenu>();
				}
				Popups.Add(popup);
				popup.Parent_Renamed = this;

				if (Peer_Renamed != null)
				{
					if (popup.Peer_Renamed == null)
					{
						popup.AddNotify();
					}
				}
			}
		}

		/// <summary>
		/// Removes the specified popup menu from the component. </summary>
		/// <param name="popup"> the popup menu to be removed </param>
		/// <seealso cref=       #add(PopupMenu)
		/// @since     JDK1.1 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void remove(MenuComponent popup)
		public virtual void Remove(MenuComponent popup)
		{
			lock (TreeLock)
			{
				if (Popups == null)
				{
					return;
				}
				int index = Popups.IndexOf(popup);
				if (index >= 0)
				{
					PopupMenu pmenu = (PopupMenu)popup;
					if (pmenu.Peer_Renamed != null)
					{
						pmenu.RemoveNotify();
					}
					pmenu.Parent_Renamed = null;
					Popups.RemoveAt(index);
					if (Popups.Count == 0)
					{
						Popups = null;
					}
				}
			}
		}

		/// <summary>
		/// Returns a string representing the state of this component. This
		/// method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>  a string representation of this component's state
		/// @since     JDK1.0 </returns>
		protected internal virtual String ParamString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String thisName = java.util.Objects.toString(getName(), "");
			String thisName = Objects.ToString(Name, "");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String invalid = isValid() ? "" : ",invalid";
			String invalid = Valid ? "" : ",invalid";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String hidden = visible ? "" : ",hidden";
			String hidden = Visible_Renamed ? "" : ",hidden";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String disabled = enabled ? "" : ",disabled";
			String disabled = Enabled_Renamed ? "" : ",disabled";
			return thisName + ',' + x + ',' + y + ',' + Width_Renamed + 'x' + Height_Renamed + invalid + hidden + disabled;
		}

		/// <summary>
		/// Returns a string representation of this component and its values. </summary>
		/// <returns>    a string representation of this component
		/// @since     JDK1.0 </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + '[' + ParamString() + ']';
		}

		/// <summary>
		/// Prints a listing of this component to the standard system output
		/// stream <code>System.out</code>. </summary>
		/// <seealso cref=       java.lang.System#out
		/// @since     JDK1.0 </seealso>
		public virtual void List()
		{
			List(System.out, 0);
		}

		/// <summary>
		/// Prints a listing of this component to the specified output
		/// stream. </summary>
		/// <param name="out">   a print stream </param>
		/// <exception cref="NullPointerException"> if {@code out} is {@code null}
		/// @since    JDK1.0 </exception>
		public virtual void List(PrintStream @out)
		{
			List(@out, 0);
		}

		/// <summary>
		/// Prints out a list, starting at the specified indentation, to the
		/// specified print stream. </summary>
		/// <param name="out">      a print stream </param>
		/// <param name="indent">   number of spaces to indent </param>
		/// <seealso cref=       java.io.PrintStream#println(java.lang.Object) </seealso>
		/// <exception cref="NullPointerException"> if {@code out} is {@code null}
		/// @since     JDK1.0 </exception>
		public virtual void List(PrintStream @out, int indent)
		{
			for (int i = 0 ; i < indent ; i++)
			{
				@out.Print(" ");
			}
			@out.Println(this);
		}

		/// <summary>
		/// Prints a listing to the specified print writer. </summary>
		/// <param name="out">  the print writer to print to </param>
		/// <exception cref="NullPointerException"> if {@code out} is {@code null}
		/// @since JDK1.1 </exception>
		public virtual void List(PrintWriter @out)
		{
			List(@out, 0);
		}

		/// <summary>
		/// Prints out a list, starting at the specified indentation, to
		/// the specified print writer. </summary>
		/// <param name="out"> the print writer to print to </param>
		/// <param name="indent"> the number of spaces to indent </param>
		/// <exception cref="NullPointerException"> if {@code out} is {@code null} </exception>
		/// <seealso cref=       java.io.PrintStream#println(java.lang.Object)
		/// @since JDK1.1 </seealso>
		public virtual void List(PrintWriter @out, int indent)
		{
			for (int i = 0 ; i < indent ; i++)
			{
				@out.Print(" ");
			}
			@out.Println(this);
		}

		/*
		 * Fetches the native container somewhere higher up in the component
		 * tree that contains this component.
		 */
		internal Container NativeContainer
		{
			get
			{
				Container p = Container;
				while (p != null && p.Peer_Renamed is LightweightPeer)
				{
					p = p.Container;
				}
				return p;
			}
		}

		/// <summary>
		/// Adds a PropertyChangeListener to the listener list. The listener is
		/// registered for all bound properties of this class, including the
		/// following:
		/// <ul>
		///    <li>this Component's font ("font")</li>
		///    <li>this Component's background color ("background")</li>
		///    <li>this Component's foreground color ("foreground")</li>
		///    <li>this Component's focusability ("focusable")</li>
		///    <li>this Component's focus traversal keys enabled state
		///        ("focusTraversalKeysEnabled")</li>
		///    <li>this Component's Set of FORWARD_TRAVERSAL_KEYS
		///        ("forwardFocusTraversalKeys")</li>
		///    <li>this Component's Set of BACKWARD_TRAVERSAL_KEYS
		///        ("backwardFocusTraversalKeys")</li>
		///    <li>this Component's Set of UP_CYCLE_TRAVERSAL_KEYS
		///        ("upCycleFocusTraversalKeys")</li>
		///    <li>this Component's preferred size ("preferredSize")</li>
		///    <li>this Component's minimum size ("minimumSize")</li>
		///    <li>this Component's maximum size ("maximumSize")</li>
		///    <li>this Component's name ("name")</li>
		/// </ul>
		/// Note that if this <code>Component</code> is inheriting a bound property, then no
		/// event will be fired in response to a change in the inherited property.
		/// <para>
		/// If <code>listener</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener">  the property change listener to be added
		/// </param>
		/// <seealso cref= #removePropertyChangeListener </seealso>
		/// <seealso cref= #getPropertyChangeListeners </seealso>
		/// <seealso cref= #addPropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
		public virtual void AddPropertyChangeListener(PropertyChangeListener listener)
		{
			lock (ObjectLock)
			{
				if (listener == null)
				{
					return;
				}
				if (ChangeSupport == null)
				{
					ChangeSupport = new PropertyChangeSupport(this);
				}
				ChangeSupport.AddPropertyChangeListener(listener);
			}
		}

		/// <summary>
		/// Removes a PropertyChangeListener from the listener list. This method
		/// should be used to remove PropertyChangeListeners that were registered
		/// for all bound properties of this class.
		/// <para>
		/// If listener is null, no exception is thrown and no action is performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listener"> the PropertyChangeListener to be removed
		/// </param>
		/// <seealso cref= #addPropertyChangeListener </seealso>
		/// <seealso cref= #getPropertyChangeListeners </seealso>
		/// <seealso cref= #removePropertyChangeListener(java.lang.String,java.beans.PropertyChangeListener) </seealso>
		public virtual void RemovePropertyChangeListener(PropertyChangeListener listener)
		{
			lock (ObjectLock)
			{
				if (listener == null || ChangeSupport == null)
				{
					return;
				}
				ChangeSupport.RemovePropertyChangeListener(listener);
			}
		}

		/// <summary>
		/// Returns an array of all the property change listeners
		/// registered on this component.
		/// </summary>
		/// <returns> all of this component's <code>PropertyChangeListener</code>s
		///         or an empty array if no property change
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addPropertyChangeListener </seealso>
		/// <seealso cref=      #removePropertyChangeListener </seealso>
		/// <seealso cref=      #getPropertyChangeListeners(java.lang.String) </seealso>
		/// <seealso cref=      java.beans.PropertyChangeSupport#getPropertyChangeListeners
		/// @since    1.4 </seealso>
		public virtual PropertyChangeListener[] PropertyChangeListeners
		{
			get
			{
				lock (ObjectLock)
				{
					if (ChangeSupport == null)
					{
						return new PropertyChangeListener[0];
					}
					return ChangeSupport.PropertyChangeListeners;
				}
			}
		}

		/// <summary>
		/// Adds a PropertyChangeListener to the listener list for a specific
		/// property. The specified property may be user-defined, or one of the
		/// following:
		/// <ul>
		///    <li>this Component's font ("font")</li>
		///    <li>this Component's background color ("background")</li>
		///    <li>this Component's foreground color ("foreground")</li>
		///    <li>this Component's focusability ("focusable")</li>
		///    <li>this Component's focus traversal keys enabled state
		///        ("focusTraversalKeysEnabled")</li>
		///    <li>this Component's Set of FORWARD_TRAVERSAL_KEYS
		///        ("forwardFocusTraversalKeys")</li>
		///    <li>this Component's Set of BACKWARD_TRAVERSAL_KEYS
		///        ("backwardFocusTraversalKeys")</li>
		///    <li>this Component's Set of UP_CYCLE_TRAVERSAL_KEYS
		///        ("upCycleFocusTraversalKeys")</li>
		/// </ul>
		/// Note that if this <code>Component</code> is inheriting a bound property, then no
		/// event will be fired in response to a change in the inherited property.
		/// <para>
		/// If <code>propertyName</code> or <code>listener</code> is <code>null</code>,
		/// no exception is thrown and no action is taken.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName"> one of the property names listed above </param>
		/// <param name="listener"> the property change listener to be added
		/// </param>
		/// <seealso cref= #removePropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
		/// <seealso cref= #getPropertyChangeListeners(java.lang.String) </seealso>
		/// <seealso cref= #addPropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
		public virtual void AddPropertyChangeListener(String propertyName, PropertyChangeListener listener)
		{
			lock (ObjectLock)
			{
				if (listener == null)
				{
					return;
				}
				if (ChangeSupport == null)
				{
					ChangeSupport = new PropertyChangeSupport(this);
				}
				ChangeSupport.AddPropertyChangeListener(propertyName, listener);
			}
		}

		/// <summary>
		/// Removes a <code>PropertyChangeListener</code> from the listener
		/// list for a specific property. This method should be used to remove
		/// <code>PropertyChangeListener</code>s
		/// that were registered for a specific bound property.
		/// <para>
		/// If <code>propertyName</code> or <code>listener</code> is <code>null</code>,
		/// no exception is thrown and no action is taken.
		/// 
		/// </para>
		/// </summary>
		/// <param name="propertyName"> a valid property name </param>
		/// <param name="listener"> the PropertyChangeListener to be removed
		/// </param>
		/// <seealso cref= #addPropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
		/// <seealso cref= #getPropertyChangeListeners(java.lang.String) </seealso>
		/// <seealso cref= #removePropertyChangeListener(java.beans.PropertyChangeListener) </seealso>
		public virtual void RemovePropertyChangeListener(String propertyName, PropertyChangeListener listener)
		{
			lock (ObjectLock)
			{
				if (listener == null || ChangeSupport == null)
				{
					return;
				}
				ChangeSupport.RemovePropertyChangeListener(propertyName, listener);
			}
		}

		/// <summary>
		/// Returns an array of all the listeners which have been associated
		/// with the named property.
		/// </summary>
		/// <returns> all of the <code>PropertyChangeListener</code>s associated with
		///         the named property; if no such listeners have been added or
		///         if <code>propertyName</code> is <code>null</code>, an empty
		///         array is returned
		/// </returns>
		/// <seealso cref= #addPropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
		/// <seealso cref= #removePropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
		/// <seealso cref= #getPropertyChangeListeners
		/// @since 1.4 </seealso>
		public virtual PropertyChangeListener[] GetPropertyChangeListeners(String propertyName)
		{
			lock (ObjectLock)
			{
				if (ChangeSupport == null)
				{
					return new PropertyChangeListener[0];
				}
				return ChangeSupport.GetPropertyChangeListeners(propertyName);
			}
		}

		/// <summary>
		/// Support for reporting bound property changes for Object properties.
		/// This method can be called when a bound property has changed and it will
		/// send the appropriate PropertyChangeEvent to any registered
		/// PropertyChangeListeners.
		/// </summary>
		/// <param name="propertyName"> the property whose value has changed </param>
		/// <param name="oldValue"> the property's previous value </param>
		/// <param name="newValue"> the property's new value </param>
		protected internal virtual void FirePropertyChange(String propertyName, Object oldValue, Object newValue)
		{
			PropertyChangeSupport changeSupport;
			lock (ObjectLock)
			{
				changeSupport = this.ChangeSupport;
			}
			if (changeSupport == null || (oldValue != null && newValue != null && oldValue.Equals(newValue)))
			{
				return;
			}
			changeSupport.FirePropertyChange(propertyName, oldValue, newValue);
		}

		/// <summary>
		/// Support for reporting bound property changes for boolean properties.
		/// This method can be called when a bound property has changed and it will
		/// send the appropriate PropertyChangeEvent to any registered
		/// PropertyChangeListeners.
		/// </summary>
		/// <param name="propertyName"> the property whose value has changed </param>
		/// <param name="oldValue"> the property's previous value </param>
		/// <param name="newValue"> the property's new value
		/// @since 1.4 </param>
		protected internal virtual void FirePropertyChange(String propertyName, bool oldValue, bool newValue)
		{
			PropertyChangeSupport changeSupport = this.ChangeSupport;
			if (changeSupport == null || oldValue == newValue)
			{
				return;
			}
			changeSupport.FirePropertyChange(propertyName, oldValue, newValue);
		}

		/// <summary>
		/// Support for reporting bound property changes for integer properties.
		/// This method can be called when a bound property has changed and it will
		/// send the appropriate PropertyChangeEvent to any registered
		/// PropertyChangeListeners.
		/// </summary>
		/// <param name="propertyName"> the property whose value has changed </param>
		/// <param name="oldValue"> the property's previous value </param>
		/// <param name="newValue"> the property's new value
		/// @since 1.4 </param>
		protected internal virtual void FirePropertyChange(String propertyName, int oldValue, int newValue)
		{
			PropertyChangeSupport changeSupport = this.ChangeSupport;
			if (changeSupport == null || oldValue == newValue)
			{
				return;
			}
			changeSupport.FirePropertyChange(propertyName, oldValue, newValue);
		}

		/// <summary>
		/// Reports a bound property change.
		/// </summary>
		/// <param name="propertyName"> the programmatic name of the property
		///          that was changed </param>
		/// <param name="oldValue"> the old value of the property (as a byte) </param>
		/// <param name="newValue"> the new value of the property (as a byte) </param>
		/// <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
		///          java.lang.Object)
		/// @since 1.5 </seealso>
		public virtual void FirePropertyChange(String propertyName, sbyte oldValue, sbyte newValue)
		{
			if (ChangeSupport == null || oldValue == newValue)
			{
				return;
			}
			FirePropertyChange(propertyName, Convert.ToByte(oldValue), Convert.ToByte(newValue));
		}

		/// <summary>
		/// Reports a bound property change.
		/// </summary>
		/// <param name="propertyName"> the programmatic name of the property
		///          that was changed </param>
		/// <param name="oldValue"> the old value of the property (as a char) </param>
		/// <param name="newValue"> the new value of the property (as a char) </param>
		/// <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
		///          java.lang.Object)
		/// @since 1.5 </seealso>
		public virtual void FirePropertyChange(String propertyName, char oldValue, char newValue)
		{
			if (ChangeSupport == null || oldValue == newValue)
			{
				return;
			}
			FirePropertyChange(propertyName, new Character(oldValue), new Character(newValue));
		}

		/// <summary>
		/// Reports a bound property change.
		/// </summary>
		/// <param name="propertyName"> the programmatic name of the property
		///          that was changed </param>
		/// <param name="oldValue"> the old value of the property (as a short) </param>
		/// <param name="newValue"> the old value of the property (as a short) </param>
		/// <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
		///          java.lang.Object)
		/// @since 1.5 </seealso>
		public virtual void FirePropertyChange(String propertyName, short oldValue, short newValue)
		{
			if (ChangeSupport == null || oldValue == newValue)
			{
				return;
			}
			FirePropertyChange(propertyName, Convert.ToInt16(oldValue), Convert.ToInt16(newValue));
		}


		/// <summary>
		/// Reports a bound property change.
		/// </summary>
		/// <param name="propertyName"> the programmatic name of the property
		///          that was changed </param>
		/// <param name="oldValue"> the old value of the property (as a long) </param>
		/// <param name="newValue"> the new value of the property (as a long) </param>
		/// <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
		///          java.lang.Object)
		/// @since 1.5 </seealso>
		public virtual void FirePropertyChange(String propertyName, long oldValue, long newValue)
		{
			if (ChangeSupport == null || oldValue == newValue)
			{
				return;
			}
			FirePropertyChange(propertyName, Convert.ToInt64(oldValue), Convert.ToInt64(newValue));
		}

		/// <summary>
		/// Reports a bound property change.
		/// </summary>
		/// <param name="propertyName"> the programmatic name of the property
		///          that was changed </param>
		/// <param name="oldValue"> the old value of the property (as a float) </param>
		/// <param name="newValue"> the new value of the property (as a float) </param>
		/// <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
		///          java.lang.Object)
		/// @since 1.5 </seealso>
		public virtual void FirePropertyChange(String propertyName, float oldValue, float newValue)
		{
			if (ChangeSupport == null || oldValue == newValue)
			{
				return;
			}
			FirePropertyChange(propertyName, Convert.ToSingle(oldValue), Convert.ToSingle(newValue));
		}

		/// <summary>
		/// Reports a bound property change.
		/// </summary>
		/// <param name="propertyName"> the programmatic name of the property
		///          that was changed </param>
		/// <param name="oldValue"> the old value of the property (as a double) </param>
		/// <param name="newValue"> the new value of the property (as a double) </param>
		/// <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
		///          java.lang.Object)
		/// @since 1.5 </seealso>
		public virtual void FirePropertyChange(String propertyName, double oldValue, double newValue)
		{
			if (ChangeSupport == null || oldValue == newValue)
			{
				return;
			}
			FirePropertyChange(propertyName, Convert.ToDouble(oldValue), Convert.ToDouble(newValue));
		}


		// Serialization support.

		/// <summary>
		/// Component Serialized Data Version.
		/// 
		/// @serial
		/// </summary>
		private int ComponentSerializedDataVersion = 4;

		/// <summary>
		/// This hack is for Swing serialization. It will invoke
		/// the Swing package private method <code>compWriteObjectNotify</code>.
		/// </summary>
		private void DoSwingSerialization()
		{
			Package swingPackage = Package.GetPackage("javax.swing");
			// For Swing serialization to correctly work Swing needs to
			// be notified before Component does it's serialization.  This
			// hack accomodates this.
			//
			// Swing classes MUST be loaded by the bootstrap class loader,
			// otherwise we don't consider them.
			for (Class klass = Component.this.GetType(); klass != null; klass = klass.BaseType)
			{
				if (klass.Assembly == swingPackage && klass.ClassLoader == null)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class swingClass = klass;
					Class swingClass = klass;
					// Find the first override of the compWriteObjectNotify method
					Method[] methods = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, swingClass));
					for (int counter = methods.Length - 1; counter >= 0; counter--)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Method method = methods[counter];
						Method method = methods[counter];
						if (method.Name.Equals("compWriteObjectNotify"))
						{
							// We found it, use doPrivileged to make it accessible
							// to use.
							AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this, method));
							// Invoke the method
							try
							{
								method.invoke(this, (Object[]) null);
							}
							catch (IllegalAccessException)
							{
							}
							catch (InvocationTargetException)
							{
							}
							// We're done, bail.
							return;
						}
					}
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Method[]>
		{
			private readonly Component OuterInstance;

			private Type SwingClass;

			public PrivilegedActionAnonymousInnerClassHelper(Component outerInstance, Type swingClass)
			{
				this.OuterInstance = outerInstance;
				this.SwingClass = swingClass;
			}

			public virtual Method[] Run()
			{
				return SwingClass.DeclaredMethods;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Void>
		{
			private readonly Component OuterInstance;

			private Method Method;

			public PrivilegedActionAnonymousInnerClassHelper2(Component outerInstance, Method method)
			{
				this.OuterInstance = outerInstance;
				this.Method = method;
			}

			public virtual Void Run()
			{
				Method.Accessible = true;
				return null;
			}
		}

		/// <summary>
		/// Writes default serializable fields to stream.  Writes
		/// a variety of serializable listeners as optional data.
		/// The non-serializable listeners are detected and
		/// no attempt is made to serialize them.
		/// </summary>
		/// <param name="s"> the <code>ObjectOutputStream</code> to write
		/// @serialData <code>null</code> terminated sequence of
		///   0 or more pairs; the pair consists of a <code>String</code>
		///   and an <code>Object</code>; the <code>String</code> indicates
		///   the type of object and is one of the following (as of 1.4):
		///   <code>componentListenerK</code> indicating an
		///     <code>ComponentListener</code> object;
		///   <code>focusListenerK</code> indicating an
		///     <code>FocusListener</code> object;
		///   <code>keyListenerK</code> indicating an
		///     <code>KeyListener</code> object;
		///   <code>mouseListenerK</code> indicating an
		///     <code>MouseListener</code> object;
		///   <code>mouseMotionListenerK</code> indicating an
		///     <code>MouseMotionListener</code> object;
		///   <code>inputMethodListenerK</code> indicating an
		///     <code>InputMethodListener</code> object;
		///   <code>hierarchyListenerK</code> indicating an
		///     <code>HierarchyListener</code> object;
		///   <code>hierarchyBoundsListenerK</code> indicating an
		///     <code>HierarchyBoundsListener</code> object;
		///   <code>mouseWheelListenerK</code> indicating an
		///     <code>MouseWheelListener</code> object
		/// @serialData an optional <code>ComponentOrientation</code>
		///    (after <code>inputMethodListener</code>, as of 1.2)
		/// </param>
		/// <seealso cref= AWTEventMulticaster#save(java.io.ObjectOutputStream, java.lang.String, java.util.EventListener) </seealso>
		/// <seealso cref= #componentListenerK </seealso>
		/// <seealso cref= #focusListenerK </seealso>
		/// <seealso cref= #keyListenerK </seealso>
		/// <seealso cref= #mouseListenerK </seealso>
		/// <seealso cref= #mouseMotionListenerK </seealso>
		/// <seealso cref= #inputMethodListenerK </seealso>
		/// <seealso cref= #hierarchyListenerK </seealso>
		/// <seealso cref= #hierarchyBoundsListenerK </seealso>
		/// <seealso cref= #mouseWheelListenerK </seealso>
		/// <seealso cref= #readObject(ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			DoSwingSerialization();

			s.DefaultWriteObject();

			AWTEventMulticaster.Save(s, ComponentListenerK, ComponentListener);
			AWTEventMulticaster.Save(s, FocusListenerK, FocusListener);
			AWTEventMulticaster.Save(s, KeyListenerK, KeyListener);
			AWTEventMulticaster.Save(s, MouseListenerK, MouseListener);
			AWTEventMulticaster.Save(s, MouseMotionListenerK, MouseMotionListener);
			AWTEventMulticaster.Save(s, InputMethodListenerK, InputMethodListener);

			s.WriteObject(null);
			s.WriteObject(ComponentOrientation_Renamed);

			AWTEventMulticaster.Save(s, HierarchyListenerK, HierarchyListener);
			AWTEventMulticaster.Save(s, HierarchyBoundsListenerK, HierarchyBoundsListener);
			s.WriteObject(null);

			AWTEventMulticaster.Save(s, MouseWheelListenerK, MouseWheelListener);
			s.WriteObject(null);

		}

		/// <summary>
		/// Reads the <code>ObjectInputStream</code> and if it isn't
		/// <code>null</code> adds a listener to receive a variety
		/// of events fired by the component.
		/// Unrecognized keys or values will be ignored.
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to read </param>
		/// <seealso cref= #writeObject(ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream s)
		{
			ObjectLock_Renamed = new Object();

			Acc = AccessController.Context;

			s.DefaultReadObject();

			AppContext = AppContext.AppContext;
			CoalescingEnabled_Renamed = CheckCoalescing();
			if (ComponentSerializedDataVersion < 4)
			{
				// These fields are non-transient and rely on default
				// serialization. However, the default values are insufficient,
				// so we need to set them explicitly for object data streams prior
				// to 1.4.
				Focusable_Renamed = true;
				IsFocusTraversableOverridden = FOCUS_TRAVERSABLE_UNKNOWN;
				InitializeFocusTraversalKeys();
				FocusTraversalKeysEnabled_Renamed = true;
			}

			Object keyOrNull;
			while (null != (keyOrNull = s.ReadObject()))
			{
				String key = ((String)keyOrNull).intern();

				if (ComponentListenerK == key)
				{
					AddComponentListener((ComponentListener)(s.ReadObject()));
				}

				else if (FocusListenerK == key)
				{
					AddFocusListener((FocusListener)(s.ReadObject()));
				}

				else if (KeyListenerK == key)
				{
					AddKeyListener((KeyListener)(s.ReadObject()));
				}

				else if (MouseListenerK == key)
				{
					AddMouseListener((MouseListener)(s.ReadObject()));
				}

				else if (MouseMotionListenerK == key)
				{
					AddMouseMotionListener((MouseMotionListener)(s.ReadObject()));
				}

				else if (InputMethodListenerK == key)
				{
					AddInputMethodListener((InputMethodListener)(s.ReadObject()));
				}

				else // skip value for unrecognized key
				{
					s.ReadObject();
				}

			}

			// Read the component's orientation if it's present
			Object orient = null;

			try
			{
				orient = s.ReadObject();
			}
			catch (java.io.OptionalDataException e)
			{
				// JDK 1.1 instances will not have this optional data.
				// e.eof will be true to indicate that there is no more
				// data available for this object.
				// If e.eof is not true, throw the exception as it
				// might have been caused by reasons unrelated to
				// componentOrientation.

				if (!e.Eof)
				{
					throw (e);
				}
			}

			if (orient != null)
			{
				ComponentOrientation_Renamed = (ComponentOrientation)orient;
			}
			else
			{
				ComponentOrientation_Renamed = ComponentOrientation.UNKNOWN;
			}

			try
			{
				while (null != (keyOrNull = s.ReadObject()))
				{
					String key = ((String)keyOrNull).intern();

					if (HierarchyListenerK == key)
					{
						AddHierarchyListener((HierarchyListener)(s.ReadObject()));
					}
					else if (HierarchyBoundsListenerK == key)
					{
						AddHierarchyBoundsListener((HierarchyBoundsListener)(s.ReadObject()));
					}
					else
					{
						// skip value for unrecognized key
						s.ReadObject();
					}
				}
			}
			catch (java.io.OptionalDataException e)
			{
				// JDK 1.1/1.2 instances will not have this optional data.
				// e.eof will be true to indicate that there is no more
				// data available for this object.
				// If e.eof is not true, throw the exception as it
				// might have been caused by reasons unrelated to
				// hierarchy and hierarchyBounds listeners.

				if (!e.Eof)
				{
					throw (e);
				}
			}

			try
			{
				while (null != (keyOrNull = s.ReadObject()))
				{
					String key = ((String)keyOrNull).intern();

					if (MouseWheelListenerK == key)
					{
						AddMouseWheelListener((MouseWheelListener)(s.ReadObject()));
					}
					else
					{
						// skip value for unrecognized key
						s.ReadObject();
					}
				}
			}
			catch (java.io.OptionalDataException e)
			{
				// pre-1.3 instances will not have this optional data.
				// e.eof will be true to indicate that there is no more
				// data available for this object.
				// If e.eof is not true, throw the exception as it
				// might have been caused by reasons unrelated to
				// mouse wheel listeners

				if (!e.Eof)
				{
					throw (e);
				}
			}

			if (Popups != null)
			{
				int npopups = Popups.Count;
				for (int i = 0 ; i < npopups ; i++)
				{
					PopupMenu popup = Popups[i];
					popup.Parent_Renamed = this;
				}
			}
		}

		/// <summary>
		/// Sets the language-sensitive orientation that is to be used to order
		/// the elements or text within this component.  Language-sensitive
		/// <code>LayoutManager</code> and <code>Component</code>
		/// subclasses will use this property to
		/// determine how to lay out and draw components.
		/// <para>
		/// At construction time, a component's orientation is set to
		/// <code>ComponentOrientation.UNKNOWN</code>,
		/// indicating that it has not been specified
		/// explicitly.  The UNKNOWN orientation behaves the same as
		/// <code>ComponentOrientation.LEFT_TO_RIGHT</code>.
		/// </para>
		/// <para>
		/// To set the orientation of a single component, use this method.
		/// To set the orientation of an entire component
		/// hierarchy, use
		/// <seealso cref="#applyComponentOrientation applyComponentOrientation"/>.
		/// </para>
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= ComponentOrientation </seealso>
		/// <seealso cref= #invalidate
		/// 
		/// @author Laura Werner, IBM
		/// @beaninfo
		///       bound: true </seealso>
		public virtual ComponentOrientation ComponentOrientation
		{
			set
			{
				ComponentOrientation oldValue = ComponentOrientation_Renamed;
				ComponentOrientation_Renamed = value;
    
				// This is a bound property, so report the change to
				// any registered listeners.  (Cheap if there are none.)
				FirePropertyChange("componentOrientation", oldValue, value);
    
				// This could change the preferred size of the Component.
				InvalidateIfValid();
			}
			get
			{
				return ComponentOrientation_Renamed;
			}
		}


		/// <summary>
		/// Sets the <code>ComponentOrientation</code> property of this component
		/// and all components contained within it.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="orientation"> the new component orientation of this component and
		///        the components contained within it. </param>
		/// <exception cref="NullPointerException"> if <code>orientation</code> is null. </exception>
		/// <seealso cref= #setComponentOrientation </seealso>
		/// <seealso cref= #getComponentOrientation </seealso>
		/// <seealso cref= #invalidate
		/// @since 1.4 </seealso>
		public virtual void ApplyComponentOrientation(ComponentOrientation orientation)
		{
			if (orientation == null)
			{
				throw new NullPointerException();
			}
			ComponentOrientation = orientation;
		}

		internal bool CanBeFocusOwner()
		{
			// It is enabled, visible, focusable.
			if (Enabled && Displayable && Visible && Focusable)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks that this component meets the prerequesites to be focus owner:
		/// - it is enabled, visible, focusable
		/// - it's parents are all enabled and showing
		/// - top-level window is focusable
		/// - if focus cycle root has DefaultFocusTraversalPolicy then it also checks that this policy accepts
		/// this component as focus owner
		/// @since 1.5
		/// </summary>
		internal bool CanBeFocusOwnerRecursively()
		{
			// - it is enabled, visible, focusable
			if (!CanBeFocusOwner())
			{
				return false;
			}

			// - it's parents are all enabled and showing
			lock (TreeLock)
			{
				if (Parent_Renamed != null)
				{
					return Parent_Renamed.CanContainFocusOwner(this);
				}
			}
			return true;
		}

		/// <summary>
		/// Fix the location of the HW component in a LW container hierarchy.
		/// </summary>
		internal void RelocateComponent()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					return;
				}
				int nativeX = x;
				int nativeY = y;
				for (Component cont = Container; cont != null && cont.Lightweight; cont = cont.Container)
				{
					nativeX += cont.x;
					nativeY += cont.y;
				}
				Peer_Renamed.SetBounds(nativeX, nativeY, Width_Renamed, Height_Renamed, java.awt.peer.ComponentPeer_Fields.SET_LOCATION);
			}
		}

		/// <summary>
		/// Returns the <code>Window</code> ancestor of the component. </summary>
		/// <returns> Window ancestor of the component or component by itself if it is Window;
		///         null, if component is not a part of window hierarchy </returns>
		internal virtual Window ContainingWindow
		{
			get
			{
				return SunToolkit.getContainingWindow(this);
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
		 *  Component will contain all of the methods in interface Accessible,
		 *  though it won't actually implement the interface - that will be up
		 *  to the individual objects which extend Component.
		 */

		/// <summary>
		/// The {@code AccessibleContext} associated with this {@code Component}.
		/// </summary>
		protected internal AccessibleContext AccessibleContext_Renamed = null;

		/// <summary>
		/// Gets the <code>AccessibleContext</code> associated
		/// with this <code>Component</code>.
		/// The method implemented by this base
		/// class returns null.  Classes that extend <code>Component</code>
		/// should implement this method to return the
		/// <code>AccessibleContext</code> associated with the subclass.
		/// 
		/// </summary>
		/// <returns> the <code>AccessibleContext</code> of this
		///    <code>Component</code>
		/// @since 1.3 </returns>
		public virtual AccessibleContext AccessibleContext
		{
			get
			{
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// Inner class of Component used to provide default support for
		/// accessibility.  This class is not meant to be used directly by
		/// application developers, but is instead meant only to be
		/// subclassed by component developers.
		/// <para>
		/// The class used to obtain the accessible role for this object.
		/// @since 1.3
		/// </para>
		/// </summary>
		[Serializable]
		protected internal abstract class AccessibleAWTComponent : AccessibleContext, AccessibleComponent
		{
			private readonly Component OuterInstance;


			internal const long SerialVersionUID = 642321655757800191L;

			/// <summary>
			/// Though the class is abstract, this should be called by
			/// all sub-classes.
			/// </summary>
			protected internal AccessibleAWTComponent(Component outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/// <summary>
			/// Number of PropertyChangeListener objects registered. It's used
			/// to add/remove ComponentListener and FocusListener to track
			/// target Component's state.
			/// </summary>
			[NonSerialized]
			internal volatile int PropertyListenersCount = 0;

			protected internal ComponentListener AccessibleAWTComponentHandler = null;
			protected internal FocusListener AccessibleAWTFocusHandler = null;

			/// <summary>
			/// Fire PropertyChange listener, if one is registered,
			/// when shown/hidden..
			/// @since 1.3
			/// </summary>
			protected internal class AccessibleAWTComponentHandler : ComponentListener
			{
				private readonly Component.AccessibleAWTComponent OuterInstance;

				public AccessibleAWTComponentHandler(Component.AccessibleAWTComponent outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public virtual void ComponentHidden(ComponentEvent e)
				{
					if (outerInstance.OuterInstance.AccessibleContext_Renamed != null)
					{
						outerInstance.OuterInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.VISIBLE, null);
					}
				}

				public virtual void ComponentShown(ComponentEvent e)
				{
					if (outerInstance.OuterInstance.AccessibleContext_Renamed != null)
					{
						outerInstance.OuterInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, null, AccessibleState.VISIBLE);
					}
				}

				public virtual void ComponentMoved(ComponentEvent e)
				{
				}

				public virtual void ComponentResized(ComponentEvent e)
				{
				}
			} // inner class AccessibleAWTComponentHandler


			/// <summary>
			/// Fire PropertyChange listener, if one is registered,
			/// when focus events happen
			/// @since 1.3
			/// </summary>
			protected internal class AccessibleAWTFocusHandler : FocusListener
			{
				private readonly Component.AccessibleAWTComponent OuterInstance;

				public AccessibleAWTFocusHandler(Component.AccessibleAWTComponent outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public virtual void FocusGained(FocusEvent @event)
				{
					if (outerInstance.OuterInstance.AccessibleContext_Renamed != null)
					{
						outerInstance.OuterInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, null, AccessibleState.FOCUSED);
					}
				}
				public virtual void FocusLost(FocusEvent @event)
				{
					if (outerInstance.OuterInstance.AccessibleContext_Renamed != null)
					{
						outerInstance.OuterInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.FOCUSED, null);
					}
				}
			} // inner class AccessibleAWTFocusHandler


			/// <summary>
			/// Adds a <code>PropertyChangeListener</code> to the listener list.
			/// </summary>
			/// <param name="listener">  the property change listener to be added </param>
			public virtual void AddPropertyChangeListener(PropertyChangeListener listener)
			{
				if (AccessibleAWTComponentHandler == null)
				{
					AccessibleAWTComponentHandler = new AccessibleAWTComponentHandler(this);
				}
				if (AccessibleAWTFocusHandler == null)
				{
					AccessibleAWTFocusHandler = new AccessibleAWTFocusHandler(this);
				}
				if (PropertyListenersCount++ == 0)
				{
					OuterInstance.AddComponentListener(AccessibleAWTComponentHandler);
					OuterInstance.AddFocusListener(AccessibleAWTFocusHandler);
				}
				base.AddPropertyChangeListener(listener);
			}

			/// <summary>
			/// Remove a PropertyChangeListener from the listener list.
			/// This removes a PropertyChangeListener that was registered
			/// for all properties.
			/// </summary>
			/// <param name="listener">  The PropertyChangeListener to be removed </param>
			public virtual void RemovePropertyChangeListener(PropertyChangeListener listener)
			{
				if (--PropertyListenersCount == 0)
				{
					OuterInstance.RemoveComponentListener(AccessibleAWTComponentHandler);
					OuterInstance.RemoveFocusListener(AccessibleAWTFocusHandler);
				}
				base.RemovePropertyChangeListener(listener);
			}

			// AccessibleContext methods
			//
			/// <summary>
			/// Gets the accessible name of this object.  This should almost never
			/// return <code>java.awt.Component.getName()</code>,
			/// as that generally isn't a localized name,
			/// and doesn't have meaning for the user.  If the
			/// object is fundamentally a text object (e.g. a menu item), the
			/// accessible name should be the text of the object (e.g. "save").
			/// If the object has a tooltip, the tooltip text may also be an
			/// appropriate String to return.
			/// </summary>
			/// <returns> the localized name of the object -- can be
			///         <code>null</code> if this
			///         object does not have a name </returns>
			/// <seealso cref= javax.accessibility.AccessibleContext#setAccessibleName </seealso>
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
			///        <code>null</code> if this object does not have a description </returns>
			/// <seealso cref= javax.accessibility.AccessibleContext#setAccessibleDescription </seealso>
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
			///      describing the role of the object </returns>
			/// <seealso cref= javax.accessibility.AccessibleRole </seealso>
			public virtual AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.AWT_COMPONENT;
				}
			}

			/// <summary>
			/// Gets the state of this object.
			/// </summary>
			/// <returns> an instance of <code>AccessibleStateSet</code>
			///       containing the current state set of the object </returns>
			/// <seealso cref= javax.accessibility.AccessibleState </seealso>
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
			/// <returns> the <code>Accessible</code> parent of this
			///      object -- can be <code>null</code> if this
			///      object does not have an <code>Accessible</code> parent </returns>
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
						Container parent = outerInstance.Parent;
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
			/// <returns> the index of this object in its parent; or -1 if this
			///    object does not have an accessible parent </returns>
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
					return 0; // Components don't have children
				}
			}

			/// <summary>
			/// Returns the nth <code>Accessible</code> child of the object.
			/// </summary>
			/// <param name="i"> zero-based index of child </param>
			/// <returns> the nth <code>Accessible</code> child of the object </returns>
			public virtual Accessible GetAccessibleChild(int i)
			{
				return null; // Components don't have children
			}

			/// <summary>
			/// Returns the locale of this object.
			/// </summary>
			/// <returns> the locale of this object </returns>
			public virtual Locale Locale
			{
				get
				{
					return OuterInstance.Locale;
				}
			}

			/// <summary>
			/// Gets the <code>AccessibleComponent</code> associated
			/// with this object if one exists.
			/// Otherwise return <code>null</code>.
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
			///      otherwise, <code>null</code> </returns>
			public virtual Color Background
			{
				get
				{
					return OuterInstance.Background;
				}
				set
				{
					OuterInstance.Background = value;
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
					return OuterInstance.Foreground;
				}
				set
				{
					OuterInstance.Foreground = value;
				}
			}


			/// <summary>
			/// Gets the <code>Cursor</code> of this object.
			/// </summary>
			/// <returns> the <code>Cursor</code>, if supported,
			///     of the object; otherwise, <code>null</code> </returns>
			public virtual Cursor Cursor
			{
				get
				{
					return OuterInstance.Cursor;
				}
				set
				{
					OuterInstance.Cursor = value;
				}
			}


			/// <summary>
			/// Gets the <code>Font</code> of this object.
			/// </summary>
			/// <returns> the <code>Font</code>, if supported,
			///    for the object; otherwise, <code>null</code> </returns>
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
			/// <returns> the <code>FontMetrics</code>, if supported,
			///     the object; otherwise, <code>null</code> </returns>
			/// <seealso cref= #getFont </seealso>
			public virtual FontMetrics GetFontMetrics(Font f)
			{
				if (f == null)
				{
					return null;
				}
				else
				{
					return OuterInstance.GetFontMetrics(f);
				}
			}

			/// <summary>
			/// Determines if the object is enabled.
			/// </summary>
			/// <returns> true if object is enabled; otherwise, false </returns>
			public virtual bool Enabled
			{
				get
				{
					return OuterInstance.Enabled;
				}
				set
				{
					bool old = OuterInstance.Enabled;
					OuterInstance.Enabled = value;
					if (value != old)
					{
						if (outerInstance.AccessibleContext_Renamed != null)
						{
							if (value)
							{
								outerInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, null, AccessibleState.ENABLED);
							}
							else
							{
								outerInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.ENABLED, null);
							}
						}
					}
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
					return OuterInstance.Visible;
				}
				set
				{
					bool old = OuterInstance.Visible;
					OuterInstance.Visible = value;
					if (value != old)
					{
						if (outerInstance.AccessibleContext_Renamed != null)
						{
							if (value)
							{
								outerInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, null, AccessibleState.VISIBLE);
							}
							else
							{
								outerInstance.AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.VISIBLE, null);
							}
						}
					}
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
					return OuterInstance.Showing;
				}
			}

			/// <summary>
			/// Checks whether the specified point is within this object's bounds,
			/// where the point's x and y coordinates are defined to be relative to
			/// the coordinate system of the object.
			/// </summary>
			/// <param name="p"> the <code>Point</code> relative to the
			///     coordinate system of the object </param>
			/// <returns> true if object contains <code>Point</code>; otherwise false </returns>
			public virtual bool Contains(Point p)
			{
				return OuterInstance.Contains(p);
			}

			/// <summary>
			/// Returns the location of the object on the screen.
			/// </summary>
			/// <returns> location of object on screen -- can be
			///    <code>null</code> if this object is not on the screen </returns>
			public virtual Point LocationOnScreen
			{
				get
				{
					lock (OuterInstance.TreeLock)
					{
						if (OuterInstance.Showing)
						{
							return OuterInstance.LocationOnScreen;
						}
						else
						{
							return null;
						}
					}
				}
			}

			/// <summary>
			/// Gets the location of the object relative to the parent in the form
			/// of a point specifying the object's top-left corner in the screen's
			/// coordinate space.
			/// </summary>
			/// <returns> an instance of Point representing the top-left corner of
			/// the object's bounds in the coordinate space of the screen;
			/// <code>null</code> if this object or its parent are not on the screen </returns>
			public virtual Point Location
			{
				get
				{
					return OuterInstance.Location;
				}
				set
				{
					OuterInstance.Location = value;
				}
			}


			/// <summary>
			/// Gets the bounds of this object in the form of a Rectangle object.
			/// The bounds specify this object's width, height, and location
			/// relative to its parent.
			/// </summary>
			/// <returns> a rectangle indicating this component's bounds;
			///   <code>null</code> if this object is not on the screen </returns>
			public virtual Rectangle Bounds
			{
				get
				{
					return OuterInstance.Bounds;
				}
				set
				{
					OuterInstance.Bounds = value;
				}
			}


			/// <summary>
			/// Returns the size of this object in the form of a
			/// <code>Dimension</code> object. The height field of the
			/// <code>Dimension</code> object contains this objects's
			/// height, and the width field of the <code>Dimension</code>
			/// object contains this object's width.
			/// </summary>
			/// <returns> a <code>Dimension</code> object that indicates
			///     the size of this component; <code>null</code> if
			///     this object is not on the screen </returns>
			public virtual Dimension Size
			{
				get
				{
					return OuterInstance.Size;
				}
				set
				{
					OuterInstance.Size = value;
				}
			}


			/// <summary>
			/// Returns the <code>Accessible</code> child,
			/// if one exists, contained at the local
			/// coordinate <code>Point</code>.  Otherwise returns
			/// <code>null</code>.
			/// </summary>
			/// <param name="p"> the point defining the top-left corner of
			///      the <code>Accessible</code>, given in the
			///      coordinate space of the object's parent </param>
			/// <returns> the <code>Accessible</code>, if it exists,
			///      at the specified location; else <code>null</code> </returns>
			public virtual Accessible GetAccessibleAt(Point p)
			{
				return null; // Components don't have children
			}

			/// <summary>
			/// Returns whether this object can accept focus or not.
			/// </summary>
			/// <returns> true if object can accept focus; otherwise false </returns>
			public virtual bool FocusTraversable
			{
				get
				{
					return OuterInstance.FocusTraversable;
				}
			}

			/// <summary>
			/// Requests focus for this object.
			/// </summary>
			public virtual void RequestFocus()
			{
				OuterInstance.RequestFocus();
			}

			/// <summary>
			/// Adds the specified focus listener to receive focus events from this
			/// component.
			/// </summary>
			/// <param name="l"> the focus listener </param>
			public virtual void AddFocusListener(FocusListener l)
			{
				OuterInstance.AddFocusListener(l);
			}

			/// <summary>
			/// Removes the specified focus listener so it no longer receives focus
			/// events from this component.
			/// </summary>
			/// <param name="l"> the focus listener </param>
			public virtual void RemoveFocusListener(FocusListener l)
			{
				OuterInstance.RemoveFocusListener(l);
			}

		} // inner class AccessibleAWTComponent


		/// <summary>
		/// Gets the index of this object in its accessible parent.
		/// If this object does not have an accessible parent, returns
		/// -1.
		/// </summary>
		/// <returns> the index of this object in its accessible parent </returns>
		internal virtual int AccessibleIndexInParent
		{
			get
			{
				lock (TreeLock)
				{
					int index = -1;
					Container parent = this.Parent;
					if (parent != null && parent is Accessible)
					{
						Component[] ca = parent.Components;
						for (int i = 0; i < ca.Length; i++)
						{
							if (ca[i] is Accessible)
							{
								index++;
							}
							if (this.Equals(ca[i]))
							{
								return index;
							}
						}
					}
					return -1;
				}
			}
		}

		/// <summary>
		/// Gets the current state set of this object.
		/// </summary>
		/// <returns> an instance of <code>AccessibleStateSet</code>
		///    containing the current state set of the object </returns>
		/// <seealso cref= AccessibleState </seealso>
		internal virtual AccessibleStateSet AccessibleStateSet
		{
			get
			{
				lock (TreeLock)
				{
					AccessibleStateSet states = new AccessibleStateSet();
					if (this.Enabled)
					{
						states.add(AccessibleState.ENABLED);
					}
					if (this.FocusTraversable)
					{
						states.add(AccessibleState.FOCUSABLE);
					}
					if (this.Visible)
					{
						states.add(AccessibleState.VISIBLE);
					}
					if (this.Showing)
					{
						states.add(AccessibleState.SHOWING);
					}
					if (this.FocusOwner)
					{
						states.add(AccessibleState.FOCUSED);
					}
					if (this is Accessible)
					{
						AccessibleContext ac = ((Accessible) this).AccessibleContext;
						if (ac != null)
						{
							Accessible ap = ac.AccessibleParent;
							if (ap != null)
							{
								AccessibleContext pac = ap.AccessibleContext;
								if (pac != null)
								{
									AccessibleSelection @as = pac.AccessibleSelection;
									if (@as != null)
									{
										states.add(AccessibleState.SELECTABLE);
										int i = ac.AccessibleIndexInParent;
										if (i >= 0)
										{
											if (@as.isAccessibleChildSelected(i))
											{
												states.add(AccessibleState.SELECTED);
											}
										}
									}
								}
							}
						}
					}
					if (Component.IsInstanceOf(this, "javax.swing.JComponent"))
					{
						if (((javax.swing.JComponent) this).Opaque)
						{
							states.add(AccessibleState.OPAQUE);
						}
					}
					return states;
				}
			}
		}

		/// <summary>
		/// Checks that the given object is instance of the given class. </summary>
		/// <param name="obj"> Object to be checked </param>
		/// <param name="className"> The name of the class. Must be fully-qualified class name. </param>
		/// <returns> true, if this object is instanceof given class,
		///         false, otherwise, or if obj or className is null </returns>
		internal static bool IsInstanceOf(Object obj, String className)
		{
			if (obj == null)
			{
				return false;
			}
			if (className == null)
			{
				return false;
			}

			Class cls = obj.GetType();
			while (cls != null)
			{
				if (cls.Name.Equals(className))
				{
					return true;
				}
				cls = cls.BaseType;
			}
			return false;
		}


		// ************************** MIXING CODE *******************************

		/// <summary>
		/// Check whether we can trust the current bounds of the component.
		/// The return value of false indicates that the container of the
		/// component is invalid, and therefore needs to be layed out, which would
		/// probably mean changing the bounds of its children.
		/// Null-layout of the container or absence of the container mean
		/// the bounds of the component are final and can be trusted.
		/// </summary>
		internal bool AreBoundsValid()
		{
			Container cont = Container;
			return cont == null || cont.Valid || cont.Layout == null;
		}

		/// <summary>
		/// Applies the shape to the component </summary>
		/// <param name="shape"> Shape to be applied to the component </param>
		internal virtual void ApplyCompoundShape(Region shape)
		{
			CheckTreeLock();

			if (!AreBoundsValid())
			{
				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this + "; areBoundsValid = " + AreBoundsValid());
				}
				return;
			}

			if (!Lightweight)
			{
				ComponentPeer peer = Peer;
				if (peer != null)
				{
					// The Region class has some optimizations. That's why
					// we should manually check whether it's empty and
					// substitute the object ourselves. Otherwise we end up
					// with some incorrect Region object with loX being
					// greater than the hiX for instance.
					if (shape.Empty)
					{
						shape = Region.EMPTY_REGION;
					}


					// Note: the shape is not really copied/cloned. We create
					// the Region object ourselves, so there's no any possibility
					// to modify the object outside of the mixing code.
					// Nullifying compoundShape means that the component has normal shape
					// (or has no shape at all).
					if (shape.Equals(NormalShape))
					{
						if (this.CompoundShape == null)
						{
							return;
						}
						this.CompoundShape = null;
						peer.ApplyShape(null);
					}
					else
					{
						if (shape.Equals(AppliedShape))
						{
							return;
						}
						this.CompoundShape = shape;
						Point compAbsolute = LocationOnWindow;
						if (MixingLog.isLoggable(PlatformLogger.Level.FINER))
						{
							MixingLog.fine("this = " + this + "; compAbsolute=" + compAbsolute + "; shape=" + shape);
						}
						peer.ApplyShape(shape.getTranslatedRegion(-compAbsolute.x, -compAbsolute.y));
					}
				}
			}
		}

		/// <summary>
		/// Returns the shape previously set with applyCompoundShape().
		/// If the component is LW or no shape was applied yet,
		/// the method returns the normal shape.
		/// </summary>
		private Region AppliedShape
		{
			get
			{
				CheckTreeLock();
				//XXX: if we allow LW components to have a shape, this must be changed
				return (this.CompoundShape == null || Lightweight) ? NormalShape : this.CompoundShape;
			}
		}

		internal virtual Point LocationOnWindow
		{
			get
			{
				CheckTreeLock();
				Point curLocation = Location;
    
				for (Container parent = Container; parent != null && !(parent is Window); parent = parent.Container)
				{
					curLocation.x += parent.X;
					curLocation.y += parent.Y;
				}
    
				return curLocation;
			}
		}

		/// <summary>
		/// Returns the full shape of the component located in window coordinates
		/// </summary>
		internal Region NormalShape
		{
			get
			{
				CheckTreeLock();
				//XXX: we may take into account a user-specified shape for this component
				Point compAbsolute = LocationOnWindow;
				return Region.getInstanceXYWH(compAbsolute.x, compAbsolute.y, Width, Height);
			}
		}

		/// <summary>
		/// Returns the "opaque shape" of the component.
		/// 
		/// The opaque shape of a lightweight components is the actual shape that
		/// needs to be cut off of the heavyweight components in order to mix this
		/// lightweight component correctly with them.
		/// 
		/// The method is overriden in the java.awt.Container to handle non-opaque
		/// containers containing opaque children.
		/// 
		/// See 6637655 for details.
		/// </summary>
		internal virtual Region OpaqueShape
		{
			get
			{
				CheckTreeLock();
				if (MixingCutoutRegion != null)
				{
					return MixingCutoutRegion;
				}
				else
				{
					return NormalShape;
				}
			}
		}

		internal int SiblingIndexAbove
		{
			get
			{
				CheckTreeLock();
				Container parent = Container;
				if (parent == null)
				{
					return -1;
				}
    
				int nextAbove = parent.GetComponentZOrder(this) - 1;
    
				return nextAbove < 0 ? - 1 : nextAbove;
			}
		}

		internal ComponentPeer HWPeerAboveMe
		{
			get
			{
				CheckTreeLock();
    
				Container cont = Container;
				int indexAbove = SiblingIndexAbove;
    
				while (cont != null)
				{
					for (int i = indexAbove; i > -1; i--)
					{
						Component comp = cont.GetComponent(i);
						if (comp != null && comp.Displayable && !comp.Lightweight)
						{
							return comp.Peer;
						}
					}
					// traversing the hierarchy up to the closest HW container;
					// further traversing may return a component that is not actually
					// a native sibling of this component and this kind of z-order
					// request may not be allowed by the underlying system (6852051).
					if (!cont.Lightweight)
					{
						break;
					}
    
					indexAbove = cont.SiblingIndexAbove;
					cont = cont.Container;
				}
    
				return null;
			}
		}

		internal int SiblingIndexBelow
		{
			get
			{
				CheckTreeLock();
				Container parent = Container;
				if (parent == null)
				{
					return -1;
				}
    
				int nextBelow = parent.GetComponentZOrder(this) + 1;
    
				return nextBelow >= parent.ComponentCount ? - 1 : nextBelow;
			}
		}

		internal bool NonOpaqueForMixing
		{
			get
			{
				return MixingCutoutRegion != null && MixingCutoutRegion.Empty;
			}
		}

		private Region CalculateCurrentShape()
		{
			CheckTreeLock();
			Region s = NormalShape;

			if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
			{
				MixingLog.fine("this = " + this + "; normalShape=" + s);
			}

			if (Container != null)
			{
				Component comp = this;
				Container cont = comp.Container;

				while (cont != null)
				{
					for (int index = comp.SiblingIndexAbove; index != -1; --index)
					{
						/* It is assumed that:
						 *
						 *    getComponent(getContainer().getComponentZOrder(comp)) == comp
						 *
						 * The assumption has been made according to the current
						 * implementation of the Container class.
						 */
						Component c = cont.GetComponent(index);
						if (c.Lightweight && c.Showing)
						{
							s = s.getDifference(c.OpaqueShape);
						}
					}

					if (cont.Lightweight)
					{
						s = s.getIntersection(cont.NormalShape);
					}
					else
					{
						break;
					}

					comp = cont;
					cont = cont.Container;
				}
			}

			if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
			{
				MixingLog.fine("currentShape=" + s);
			}

			return s;
		}

		internal virtual void ApplyCurrentShape()
		{
			CheckTreeLock();
			if (!AreBoundsValid())
			{
				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this + "; areBoundsValid = " + AreBoundsValid());
				}
				return; // Because applyCompoundShape() ignores such components anyway
			}
			if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
			{
				MixingLog.fine("this = " + this);
			}
			ApplyCompoundShape(CalculateCurrentShape());
		}

		internal void SubtractAndApplyShape(Region s)
		{
			CheckTreeLock();

			if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
			{
				MixingLog.fine("this = " + this + "; s=" + s);
			}

			ApplyCompoundShape(AppliedShape.getDifference(s));
		}

		private void ApplyCurrentShapeBelowMe()
		{
			CheckTreeLock();
			Container parent = Container;
			if (parent != null && parent.Showing)
			{
				// First, reapply shapes of my siblings
				parent.RecursiveApplyCurrentShape(SiblingIndexBelow);

				// Second, if my container is non-opaque, reapply shapes of siblings of my container
				Container parent2 = parent.Container;
				while (!parent.Opaque && parent2 != null)
				{
					parent2.RecursiveApplyCurrentShape(parent.SiblingIndexBelow);

					parent = parent2;
					parent2 = parent.Container;
				}
			}
		}

		internal void SubtractAndApplyShapeBelowMe()
		{
			CheckTreeLock();
			Container parent = Container;
			if (parent != null && Showing)
			{
				Region opaqueShape = OpaqueShape;

				// First, cut my siblings
				parent.RecursiveSubtractAndApplyShape(opaqueShape, SiblingIndexBelow);

				// Second, if my container is non-opaque, cut siblings of my container
				Container parent2 = parent.Container;
				while (!parent.Opaque && parent2 != null)
				{
					parent2.RecursiveSubtractAndApplyShape(opaqueShape, parent.SiblingIndexBelow);

					parent = parent2;
					parent2 = parent.Container;
				}
			}
		}

		internal virtual void MixOnShowing()
		{
			lock (TreeLock)
			{
				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this);
				}
				if (!MixingNeeded)
				{
					return;
				}
				if (Lightweight)
				{
					SubtractAndApplyShapeBelowMe();
				}
				else
				{
					ApplyCurrentShape();
				}
			}
		}

		internal virtual void MixOnHiding(bool isLightweight)
		{
			// We cannot be sure that the peer exists at this point, so we need the argument
			//    to find out whether the hiding component is (well, actually was) a LW or a HW.
			lock (TreeLock)
			{
				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this + "; isLightweight = " + isLightweight);
				}
				if (!MixingNeeded)
				{
					return;
				}
				if (isLightweight)
				{
					ApplyCurrentShapeBelowMe();
				}
			}
		}

		internal virtual void MixOnReshaping()
		{
			lock (TreeLock)
			{
				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this);
				}
				if (!MixingNeeded)
				{
					return;
				}
				if (Lightweight)
				{
					ApplyCurrentShapeBelowMe();
				}
				else
				{
					ApplyCurrentShape();
				}
			}
		}

		internal virtual void MixOnZOrderChanging(int oldZorder, int newZorder)
		{
			lock (TreeLock)
			{
				bool becameHigher = newZorder < oldZorder;
				Container parent = Container;

				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this + "; oldZorder=" + oldZorder + "; newZorder=" + newZorder + "; parent=" + parent);
				}
				if (!MixingNeeded)
				{
					return;
				}
				if (Lightweight)
				{
					if (becameHigher)
					{
						if (parent != null && Showing)
						{
							parent.RecursiveSubtractAndApplyShape(OpaqueShape, SiblingIndexBelow, oldZorder);
						}
					}
					else
					{
						if (parent != null)
						{
							parent.RecursiveApplyCurrentShape(oldZorder, newZorder);
						}
					}
				}
				else
				{
					if (becameHigher)
					{
						ApplyCurrentShape();
					}
					else
					{
						if (parent != null)
						{
							Region shape = AppliedShape;

							for (int index = oldZorder; index < newZorder; index++)
							{
								Component c = parent.GetComponent(index);
								if (c.Lightweight && c.Showing)
								{
									shape = shape.getDifference(c.OpaqueShape);
								}
							}
							ApplyCompoundShape(shape);
						}
					}
				}
			}
		}

		internal virtual void MixOnValidating()
		{
			// This method gets overriden in the Container. Obviously, a plain
			// non-container components don't need to handle validation.
		}

		internal bool MixingNeeded
		{
			get
			{
				if (SunToolkit.SunAwtDisableMixing)
				{
					if (MixingLog.isLoggable(PlatformLogger.Level.FINEST))
					{
						MixingLog.finest("this = " + this + "; Mixing disabled via sun.awt.disableMixing");
					}
					return false;
				}
				if (!AreBoundsValid())
				{
					if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
					{
						MixingLog.fine("this = " + this + "; areBoundsValid = " + AreBoundsValid());
					}
					return false;
				}
				Window window = ContainingWindow;
				if (window != null)
				{
					if (!window.HasHeavyweightDescendants() || !window.HasLightweightDescendants() || window.Disposing)
					{
						if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
						{
							MixingLog.fine("containing window = " + window + "; has h/w descendants = " + window.HasHeavyweightDescendants() + "; has l/w descendants = " + window.HasLightweightDescendants() + "; disposing = " + window.Disposing);
						}
						return false;
					}
				}
				else
				{
					if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
					{
						MixingLog.fine("this = " + this + "; containing window is null");
					}
					return false;
				}
				return true;
			}
		}

		// ****************** END OF MIXING CODE ********************************

		// Note that the method is overriden in the Window class,
		// a window doesn't need to be updated in the Z-order.
		internal virtual void UpdateZOrder()
		{
			Peer_Renamed.ZOrder = HWPeerAboveMe;
		}

	}

}