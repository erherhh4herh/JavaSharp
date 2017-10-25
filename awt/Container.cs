using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
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






	using PlatformLogger = sun.util.logging.PlatformLogger;

	using AppContext = sun.awt.AppContext;
	using AWTAccessor = sun.awt.AWTAccessor;
	using CausedFocusEvent = sun.awt.CausedFocusEvent;
	using PeerEvent = sun.awt.PeerEvent;
	using SunToolkit = sun.awt.SunToolkit;

	using SunDropTargetEvent = sun.awt.dnd.SunDropTargetEvent;

	using Region = sun.java2d.pipe.Region;

	using GetBooleanAction = sun.security.action.GetBooleanAction;

	/// <summary>
	/// A generic Abstract Window Toolkit(AWT) container object is a component
	/// that can contain other AWT components.
	/// <para>
	/// Components added to a container are tracked in a list.  The order
	/// of the list will define the components' front-to-back stacking order
	/// within the container.  If no index is specified when adding a
	/// component to a container, it will be added to the end of the list
	/// (and hence to the bottom of the stacking order).
	/// </para>
	/// <para>
	/// <b>Note</b>: For details on the focus subsystem, see
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
	/// <seealso cref=       #add(java.awt.Component, int) </seealso>
	/// <seealso cref=       #getComponent(int) </seealso>
	/// <seealso cref=       LayoutManager
	/// @since     JDK1.0 </seealso>
	public class Container : Component
	{

		private static readonly PlatformLogger Log = PlatformLogger.getLogger("java.awt.Container");
		private static readonly PlatformLogger EventLog = PlatformLogger.getLogger("java.awt.event.Container");

		private static readonly Component[] EMPTY_ARRAY = new Component[0];

		/// <summary>
		/// The components in this container. </summary>
		/// <seealso cref= #add </seealso>
		/// <seealso cref= #getComponents </seealso>
		private IList<Component> Component = new List<Component>();

		/// <summary>
		/// Layout manager for this container. </summary>
		/// <seealso cref= #doLayout </seealso>
		/// <seealso cref= #setLayout </seealso>
		/// <seealso cref= #getLayout </seealso>
		internal LayoutManager LayoutMgr;

		/// <summary>
		/// Event router for lightweight components.  If this container
		/// is native, this dispatcher takes care of forwarding and
		/// retargeting the events to lightweight components contained
		/// (if any).
		/// </summary>
		private LightweightDispatcher Dispatcher;

		/// <summary>
		/// The focus traversal policy that will manage keyboard traversal of this
		/// Container's children, if this Container is a focus cycle root. If the
		/// value is null, this Container inherits its policy from its focus-cycle-
		/// root ancestor. If all such ancestors of this Container have null
		/// policies, then the current KeyboardFocusManager's default policy is
		/// used. If the value is non-null, this policy will be inherited by all
		/// focus-cycle-root children that have no keyboard-traversal policy of
		/// their own (as will, recursively, their focus-cycle-root children).
		/// <para>
		/// If this Container is not a focus cycle root, the value will be
		/// remembered, but will not be used or inherited by this or any other
		/// Containers until this Container is made a focus cycle root.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #setFocusTraversalPolicy </seealso>
		/// <seealso cref= #getFocusTraversalPolicy
		/// @since 1.4 </seealso>
		[NonSerialized]
		private FocusTraversalPolicy FocusTraversalPolicy_Renamed;

		/// <summary>
		/// Indicates whether this Component is the root of a focus traversal cycle.
		/// Once focus enters a traversal cycle, typically it cannot leave it via
		/// focus traversal unless one of the up- or down-cycle keys is pressed.
		/// Normal traversal is limited to this Container, and all of this
		/// Container's descendants that are not descendants of inferior focus cycle
		/// roots.
		/// </summary>
		/// <seealso cref= #setFocusCycleRoot </seealso>
		/// <seealso cref= #isFocusCycleRoot
		/// @since 1.4 </seealso>
		private bool FocusCycleRoot_Renamed = false;


		/// <summary>
		/// Stores the value of focusTraversalPolicyProvider property.
		/// @since 1.5 </summary>
		/// <seealso cref= #setFocusTraversalPolicyProvider </seealso>
		private bool FocusTraversalPolicyProvider_Renamed;

		// keeps track of the threads that are printing this component
		[NonSerialized]
		private Set<Thread> PrintingThreads;
		// True if there is at least one thread that's printing this component
		[NonSerialized]
		private bool Printing = false;

		[NonSerialized]
		internal ContainerListener ContainerListener;

		/* HierarchyListener and HierarchyBoundsListener support */
		[NonSerialized]
		internal int ListeningChildren;
		[NonSerialized]
		internal int ListeningBoundsChildren;
		[NonSerialized]
		internal int DescendantsCount;

		/* Non-opaque window support -- see Window.setLayersOpaque */
		[NonSerialized]
		internal Color PreserveBackgroundColor = null;

		/// <summary>
		/// JDK 1.1 serialVersionUID
		/// </summary>
		private const long SerialVersionUID = 4613797578919906343L;

		/// <summary>
		/// A constant which toggles one of the controllable behaviors
		/// of <code>getMouseEventTarget</code>. It is used to specify whether
		/// the method can return the Container on which it is originally called
		/// in case if none of its children are the current mouse event targets.
		/// </summary>
		/// <seealso cref= #getMouseEventTarget(int, int, boolean) </seealso>
		internal const bool INCLUDE_SELF = true;

		/// <summary>
		/// A constant which toggles one of the controllable behaviors
		/// of <code>getMouseEventTarget</code>. It is used to specify whether
		/// the method should search only lightweight components.
		/// </summary>
		/// <seealso cref= #getMouseEventTarget(int, int, boolean) </seealso>
		internal const bool SEARCH_HEAVYWEIGHTS = true;

		/*
		 * Number of HW or LW components in this container (including
		 * all descendant containers).
		 */
		[NonSerialized]
		private int NumOfHWComponents = 0;
		[NonSerialized]
		private int NumOfLWComponents = 0;

		private static readonly PlatformLogger MixingLog = PlatformLogger.getLogger("java.awt.mixing.Container");

		/// <summary>
		/// @serialField ncomponents                     int
		///       The number of components in this container.
		///       This value can be null.
		/// @serialField component                       Component[]
		///       The components in this container.
		/// @serialField layoutMgr                       LayoutManager
		///       Layout manager for this container.
		/// @serialField dispatcher                      LightweightDispatcher
		///       Event router for lightweight components.  If this container
		///       is native, this dispatcher takes care of forwarding and
		///       retargeting the events to lightweight components contained
		///       (if any).
		/// @serialField maxSize                         Dimension
		///       Maximum size of this Container.
		/// @serialField focusCycleRoot                  boolean
		///       Indicates whether this Component is the root of a focus traversal cycle.
		///       Once focus enters a traversal cycle, typically it cannot leave it via
		///       focus traversal unless one of the up- or down-cycle keys is pressed.
		///       Normal traversal is limited to this Container, and all of this
		///       Container's descendants that are not descendants of inferior focus cycle
		///       roots.
		/// @serialField containerSerializedDataVersion  int
		///       Container Serial Data Version.
		/// @serialField focusTraversalPolicyProvider    boolean
		///       Stores the value of focusTraversalPolicyProvider property.
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("ncomponents", Integer.TYPE), new ObjectStreamField("component", typeof(Component[])), new ObjectStreamField("layoutMgr", typeof(LayoutManager)), new ObjectStreamField("dispatcher", typeof(LightweightDispatcher)), new ObjectStreamField("maxSize", typeof(Dimension)), new ObjectStreamField("focusCycleRoot", Boolean.TYPE), new ObjectStreamField("containerSerializedDataVersion", Integer.TYPE), new ObjectStreamField("focusTraversalPolicyProvider", Boolean.TYPE)};

		static Container()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}

			AWTAccessor.ContainerAccessor = new ContainerAccessorAnonymousInnerClassHelper();
			// Don't lazy-read because every app uses invalidate()
			IsJavaAwtSmartInvalidate = AccessController.doPrivileged(new GetBooleanAction("java.awt.smartInvalidate"));
		}

		private class ContainerAccessorAnonymousInnerClassHelper : AWTAccessor.ContainerAccessor
		{
			public ContainerAccessorAnonymousInnerClassHelper()
			{
			}

			public override void ValidateUnconditionally(Container cont)
			{
				cont.ValidateUnconditionally();
			}

			public override Component FindComponentAt(Container cont, int x, int y, bool ignoreEnabled)
			{
				return cont.FindComponentAt(x, y, ignoreEnabled);
			}
		}

		/// <summary>
		/// Initialize JNI field and method IDs for fields that may be
		///   called from C.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		/// <summary>
		/// Constructs a new Container. Containers can be extended directly,
		/// but are lightweight in this case and must be contained by a parent
		/// somewhere higher up in the component tree that is native.
		/// (such as Frame for example).
		/// </summary>
		public Container()
		{
		}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked","rawtypes"}) void initializeFocusTraversalKeys()
		internal override void InitializeFocusTraversalKeys()
		{
			FocusTraversalKeys = new Set[4];
		}

		/// <summary>
		/// Gets the number of components in this panel.
		/// <para>
		/// Note: This method should be called under AWT tree lock.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    the number of components in this panel. </returns>
		/// <seealso cref=       #getComponent
		/// @since     JDK1.1 </seealso>
		/// <seealso cref= Component#getTreeLock() </seealso>
		public virtual int ComponentCount
		{
			get
			{
				return CountComponents();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by getComponentCount(). 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int CountComponents()
		{
			// This method is not synchronized under AWT tree lock.
			// Instead, the calling code is responsible for the
			// synchronization. See 6784816 for details.
			return Component.Count;
		}

		/// <summary>
		/// Gets the nth component in this container.
		/// <para>
		/// Note: This method should be called under AWT tree lock.
		/// 
		/// </para>
		/// </summary>
		/// <param name="n">   the index of the component to get. </param>
		/// <returns>     the n<sup>th</sup> component in this container. </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///                 if the n<sup>th</sup> value does not exist. </exception>
		/// <seealso cref= Component#getTreeLock() </seealso>
		public virtual Component GetComponent(int n)
		{
			// This method is not synchronized under AWT tree lock.
			// Instead, the calling code is responsible for the
			// synchronization. See 6784816 for details.
			try
			{
				return Component[n];
			}
			catch (IndexOutOfBoundsException)
			{
				throw new ArrayIndexOutOfBoundsException("No such child: " + n);
			}
		}

		/// <summary>
		/// Gets all the components in this container.
		/// <para>
		/// Note: This method should be called under AWT tree lock.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    an array of all the components in this container. </returns>
		/// <seealso cref= Component#getTreeLock() </seealso>
		public virtual Component[] Components
		{
			get
			{
				// This method is not synchronized under AWT tree lock.
				// Instead, the calling code is responsible for the
				// synchronization. See 6784816 for details.
				return Components_NoClientCode;
			}
		}

		// NOTE: This method may be called by privileged threads.
		//       This functionality is implemented in a package-private method
		//       to insure that it cannot be overridden by client subclasses.
		//       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		internal Component[] Components_NoClientCode
		{
			get
			{
				return Component.toArray(EMPTY_ARRAY);
			}
		}

		/*
		 * Wrapper for getComponents() method with a proper synchronization.
		 */
		internal virtual Component[] ComponentsSync
		{
			get
			{
				lock (TreeLock)
				{
					return Components;
				}
			}
		}

		/// <summary>
		/// Determines the insets of this container, which indicate the size
		/// of the container's border.
		/// <para>
		/// A <code>Frame</code> object, for example, has a top inset that
		/// corresponds to the height of the frame's title bar.
		/// </para>
		/// </summary>
		/// <returns>    the insets of this container. </returns>
		/// <seealso cref=       Insets </seealso>
		/// <seealso cref=       LayoutManager
		/// @since     JDK1.1 </seealso>
		public virtual Insets Insets
		{
			get
			{
				return Insets();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getInsets()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Insets Insets()
		{
			ComponentPeer peer = this.Peer_Renamed;
			if (peer is ContainerPeer)
			{
				ContainerPeer cpeer = (ContainerPeer)peer;
				return (Insets)cpeer.Insets.Clone();
			}
			return new Insets(0, 0, 0, 0);
		}

		/// <summary>
		/// Appends the specified component to the end of this container.
		/// This is a convenience method for <seealso cref="#addImpl"/>.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy. If the container has already been
		/// displayed, the hierarchy must be validated thereafter in order to
		/// display the added component.
		/// 
		/// </para>
		/// </summary>
		/// <param name="comp">   the component to be added </param>
		/// <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		/// <seealso cref= #addImpl </seealso>
		/// <seealso cref= #invalidate </seealso>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		/// <returns>    the component argument </returns>
		public virtual Component Add(Component comp)
		{
			AddImpl(comp, null, -1);
			return comp;
		}

		/// <summary>
		/// Adds the specified component to this container.
		/// This is a convenience method for <seealso cref="#addImpl"/>.
		/// <para>
		/// This method is obsolete as of 1.1.  Please use the
		/// method <code>add(Component, Object)</code> instead.
		/// </para>
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy. If the container has already been
		/// displayed, the hierarchy must be validated thereafter in order to
		/// display the added component.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		/// <seealso cref= #add(Component, Object) </seealso>
		/// <seealso cref= #invalidate </seealso>
		public virtual Component Add(String name, Component comp)
		{
			AddImpl(comp, name, -1);
			return comp;
		}

		/// <summary>
		/// Adds the specified component to this container at the given
		/// position.
		/// This is a convenience method for <seealso cref="#addImpl"/>.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy. If the container has already been
		/// displayed, the hierarchy must be validated thereafter in order to
		/// display the added component.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="comp">   the component to be added </param>
		/// <param name="index">    the position at which to insert the component,
		///                   or <code>-1</code> to append the component to the end </param>
		/// <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if {@code index} is invalid (see
		///            <seealso cref="#addImpl"/> for details) </exception>
		/// <returns>    the component <code>comp</code> </returns>
		/// <seealso cref= #addImpl </seealso>
		/// <seealso cref= #remove </seealso>
		/// <seealso cref= #invalidate </seealso>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		public virtual Component Add(Component comp, int index)
		{
			AddImpl(comp, null, index);
			return comp;
		}

		/// <summary>
		/// Checks that the component
		/// isn't supposed to be added into itself.
		/// </summary>
		private void CheckAddToSelf(Component comp)
		{
			if (comp is Container)
			{
				for (Container cn = this; cn != null; cn = cn.Parent_Renamed)
				{
					if (cn == comp)
					{
						throw new IllegalArgumentException("adding container's parent to itself");
					}
				}
			}
		}

		/// <summary>
		/// Checks that the component is not a Window instance.
		/// </summary>
		private void CheckNotAWindow(Component comp)
		{
			if (comp is Window)
			{
				throw new IllegalArgumentException("adding a window to a container");
			}
		}

		/// <summary>
		/// Checks that the component comp can be added to this container
		/// Checks :  index in bounds of container's size,
		/// comp is not one of this container's parents,
		/// and comp is not a window.
		/// Comp and container must be on the same GraphicsDevice.
		/// if comp is container, all sub-components must be on
		/// same GraphicsDevice.
		/// 
		/// @since 1.5
		/// </summary>
		private void CheckAdding(Component comp, int index)
		{
			CheckTreeLock();

			GraphicsConfiguration thisGC = GraphicsConfiguration;

			if (index > Component.Count || index < 0)
			{
				throw new IllegalArgumentException("illegal component position");
			}
			if (comp.Parent_Renamed == this)
			{
				if (index == Component.Count)
				{
					throw new IllegalArgumentException("illegal component position " + index + " should be less then " + Component.Count);
				}
			}
			CheckAddToSelf(comp);
			CheckNotAWindow(comp);

			Window thisTopLevel = ContainingWindow;
			Window compTopLevel = comp.ContainingWindow;
			if (thisTopLevel != compTopLevel)
			{
				throw new IllegalArgumentException("component and container should be in the same top-level window");
			}
			if (thisGC != null)
			{
				comp.CheckGD(thisGC.Device.IDstring);
			}
		}

		/// <summary>
		/// Removes component comp from this container without making unneccessary changes
		/// and generating unneccessary events. This function intended to perform optimized
		/// remove, for example, if newParent and current parent are the same it just changes
		/// index without calling removeNotify.
		/// Note: Should be called while holding treeLock
		/// Returns whether removeNotify was invoked
		/// @since: 1.5
		/// </summary>
		private bool RemoveDelicately(Component comp, Container newParent, int newIndex)
		{
			CheckTreeLock();

			int index = GetComponentZOrder(comp);
			bool needRemoveNotify = IsRemoveNotifyNeeded(comp, this, newParent);
			if (needRemoveNotify)
			{
				comp.RemoveNotify();
			}
			if (newParent != this)
			{
				if (LayoutMgr != null)
				{
					LayoutMgr.RemoveLayoutComponent(comp);
				}
				AdjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, -comp.NumListening(AWTEvent.HIERARCHY_EVENT_MASK));
				AdjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, -comp.NumListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK));
				AdjustDescendants(-(comp.CountHierarchyMembers()));

				comp.Parent_Renamed = null;
				if (needRemoveNotify)
				{
					comp.GraphicsConfiguration = null;
				}
				Component.RemoveAt(index);

				InvalidateIfValid();
			}
			else
			{
				// We should remove component and then
				// add it by the newIndex without newIndex decrement if even we shift components to the left
				// after remove. Consult the rules below:
				// 2->4: 012345 -> 013425, 2->5: 012345 -> 013452
				// 4->2: 012345 -> 014235
				Component.RemoveAt(index);
				Component.Insert(newIndex, comp);
			}
			if (comp.Parent_Renamed == null) // was actually removed
			{
				if (ContainerListener != null || (EventMask & AWTEvent.CONTAINER_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK))
				{
					ContainerEvent e = new ContainerEvent(this, ContainerEvent.COMPONENT_REMOVED, comp);
					DispatchEvent(e);

				}
				comp.CreateHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, this, HierarchyEvent.PARENT_CHANGED, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK));
				if (Peer_Renamed != null && LayoutMgr == null && Visible)
				{
					UpdateCursorImmediately();
				}
			}
			return needRemoveNotify;
		}

		/// <summary>
		/// Checks whether this container can contain component which is focus owner.
		/// Verifies that container is enable and showing, and if it is focus cycle root
		/// its FTP allows component to be focus owner
		/// @since 1.5
		/// </summary>
		internal virtual bool CanContainFocusOwner(Component focusOwnerCandidate)
		{
			if (!(Enabled && Displayable && Visible && Focusable))
			{
				return false;
			}
			if (FocusCycleRoot)
			{
				FocusTraversalPolicy policy = FocusTraversalPolicy;
				if (policy is DefaultFocusTraversalPolicy)
				{
					if (!((DefaultFocusTraversalPolicy)policy).Accept(focusOwnerCandidate))
					{
						return false;
					}
				}
			}
			lock (TreeLock)
			{
				if (Parent_Renamed != null)
				{
					return Parent_Renamed.CanContainFocusOwner(focusOwnerCandidate);
				}
			}
			return true;
		}

		/// <summary>
		/// Checks whether or not this container has heavyweight children.
		/// Note: Should be called while holding tree lock </summary>
		/// <returns> true if there is at least one heavyweight children in a container, false otherwise
		/// @since 1.5 </returns>
		internal bool HasHeavyweightDescendants()
		{
			CheckTreeLock();
			return NumOfHWComponents > 0;
		}

		/// <summary>
		/// Checks whether or not this container has lightweight children.
		/// Note: Should be called while holding tree lock </summary>
		/// <returns> true if there is at least one lightweight children in a container, false otherwise
		/// @since 1.7 </returns>
		internal bool HasLightweightDescendants()
		{
			CheckTreeLock();
			return NumOfLWComponents > 0;
		}

		/// <summary>
		/// Returns closest heavyweight component to this container. If this container is heavyweight
		/// returns this.
		/// @since 1.5
		/// </summary>
		internal virtual Container HeavyweightContainer
		{
			get
			{
				CheckTreeLock();
				if (Peer_Renamed != null && !(Peer_Renamed is LightweightPeer))
				{
					return this;
				}
				else
				{
					return NativeContainer;
				}
			}
		}

		/// <summary>
		/// Detects whether or not remove from current parent and adding to new parent requires call of
		/// removeNotify on the component. Since removeNotify destroys native window this might (not)
		/// be required. For example, if new container and old containers are the same we don't need to
		/// destroy native window.
		/// @since: 1.5
		/// </summary>
		private static bool IsRemoveNotifyNeeded(Component comp, Container oldContainer, Container newContainer)
		{
			if (oldContainer == null) // Component didn't have parent - no removeNotify
			{
				return false;
			}
			if (comp.Peer_Renamed == null) // Component didn't have peer - no removeNotify
			{
				return false;
			}
			if (newContainer.Peer_Renamed == null)
			{
				// Component has peer but new Container doesn't - call removeNotify
				return true;
			}

			// If component is lightweight non-Container or lightweight Container with all but heavyweight
			// children there is no need to call remove notify
			if (comp.Lightweight)
			{
				bool isContainer = comp is Container;

				if (!isContainer || (isContainer && !((Container)comp).HasHeavyweightDescendants()))
				{
					return false;
				}
			}

			// If this point is reached, then the comp is either a HW or a LW container with HW descendants.

			// All three components have peers, check for peer change
			Container newNativeContainer = oldContainer.HeavyweightContainer;
			Container oldNativeContainer = newContainer.HeavyweightContainer;
			if (newNativeContainer != oldNativeContainer)
			{
				// Native containers change - check whether or not current platform supports
				// changing of widget hierarchy on native level without recreation.
				// The current implementation forbids reparenting of LW containers with HW descendants
				// into another native container w/o destroying the peers. Actually such an operation
				// is quite rare. If we ever need to save the peers, we'll have to slightly change the
				// addDelicately() method in order to handle such LW containers recursively, reparenting
				// each HW descendant independently.
				return !comp.Peer_Renamed.ReparentSupported;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Moves the specified component to the specified z-order index in
		/// the container. The z-order determines the order that components
		/// are painted; the component with the highest z-order paints first
		/// and the component with the lowest z-order paints last.
		/// Where components overlap, the component with the lower
		/// z-order paints over the component with the higher z-order.
		/// <para>
		/// If the component is a child of some other container, it is
		/// removed from that container before being added to this container.
		/// The important difference between this method and
		/// <code>java.awt.Container.add(Component, int)</code> is that this method
		/// doesn't call <code>removeNotify</code> on the component while
		/// removing it from its previous container unless necessary and when
		/// allowed by the underlying native windowing system. This way, if the
		/// component has the keyboard focus, it maintains the focus when
		/// moved to the new position.
		/// </para>
		/// <para>
		/// This property is guaranteed to apply only to lightweight
		/// non-<code>Container</code> components.
		/// </para>
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy.
		/// </para>
		/// <para>
		/// <b>Note</b>: Not all platforms support changing the z-order of
		/// heavyweight components from one container into another without
		/// the call to <code>removeNotify</code>. There is no way to detect
		/// whether a platform supports this, so developers shouldn't make
		/// any assumptions.
		/// 
		/// </para>
		/// </summary>
		/// <param name="comp"> the component to be moved </param>
		/// <param name="index"> the position in the container's list to
		///            insert the component, where <code>getComponentCount()</code>
		///            appends to the end </param>
		/// <exception cref="NullPointerException"> if <code>comp</code> is
		///            <code>null</code> </exception>
		/// <exception cref="IllegalArgumentException"> if <code>comp</code> is one of the
		///            container's parents </exception>
		/// <exception cref="IllegalArgumentException"> if <code>index</code> is not in
		///            the range <code>[0, getComponentCount()]</code> for moving
		///            between containers, or not in the range
		///            <code>[0, getComponentCount()-1]</code> for moving inside
		///            a container </exception>
		/// <exception cref="IllegalArgumentException"> if adding a container to itself </exception>
		/// <exception cref="IllegalArgumentException"> if adding a <code>Window</code>
		///            to a container </exception>
		/// <seealso cref= #getComponentZOrder(java.awt.Component) </seealso>
		/// <seealso cref= #invalidate
		/// @since 1.5 </seealso>
		public virtual void SetComponentZOrder(Component comp, int index)
		{
			 lock (TreeLock)
			 {
				 // Store parent because remove will clear it
				 Container curParent = comp.Parent_Renamed;
				 int oldZindex = GetComponentZOrder(comp);

				 if (curParent == this && index == oldZindex)
				 {
					 return;
				 }
				 CheckAdding(comp, index);

				 bool peerRecreated = (curParent != null) ? curParent.RemoveDelicately(comp, this, index) : false;

				 AddDelicately(comp, curParent, index);

				 // If the oldZindex == -1, the component gets inserted,
				 // rather than it changes its z-order.
				 if (!peerRecreated && oldZindex != -1)
				 {
					 // The new 'index' cannot be == -1.
					 // It gets checked at the checkAdding() method.
					 // Therefore both oldZIndex and index denote
					 // some existing positions at this point and
					 // this is actually a Z-order changing.
					 comp.MixOnZOrderChanging(oldZindex, index);
				 }
			 }
		}

		/// <summary>
		/// Traverses the tree of components and reparents children heavyweight component
		/// to new heavyweight parent.
		/// @since 1.5
		/// </summary>
		private void ReparentTraverse(ContainerPeer parentPeer, Container child)
		{
			CheckTreeLock();

			for (int i = 0; i < child.ComponentCount; i++)
			{
				Component comp = child.GetComponent(i);
				if (comp.Lightweight)
				{
					// If components is lightweight check if it is container
					// If it is container it might contain heavyweight children we need to reparent
					if (comp is Container)
					{
						ReparentTraverse(parentPeer, (Container)comp);
					}
				}
				else
				{
					// Q: Need to update NativeInLightFixer?
					comp.Peer.Reparent(parentPeer);
				}
			}
		}

		/// <summary>
		/// Reparents child component peer to this container peer.
		/// Container must be heavyweight.
		/// @since 1.5
		/// </summary>
		private void ReparentChild(Component comp)
		{
			CheckTreeLock();
			if (comp == null)
			{
				return;
			}
			if (comp.Lightweight)
			{
				// If component is lightweight container we need to reparent all its explicit  heavyweight children
				if (comp is Container)
				{
					// Traverse component's tree till depth-first until encountering heavyweight component
					ReparentTraverse((ContainerPeer)Peer, (Container)comp);
				}
			}
			else
			{
				comp.Peer.Reparent((ContainerPeer)Peer);
			}
		}

		/// <summary>
		/// Adds component to this container. Tries to minimize side effects of this adding -
		/// doesn't call remove notify if it is not required.
		/// @since 1.5
		/// </summary>
		private void AddDelicately(Component comp, Container curParent, int index)
		{
			CheckTreeLock();

			// Check if moving between containers
			if (curParent != this)
			{
				//index == -1 means add to the end.
				if (index == -1)
				{
					Component.Add(comp);
				}
				else
				{
					Component.Insert(index, comp);
				}
				comp.Parent_Renamed = this;
				comp.GraphicsConfiguration = GraphicsConfiguration;

				AdjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, comp.NumListening(AWTEvent.HIERARCHY_EVENT_MASK));
				AdjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, comp.NumListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK));
				AdjustDescendants(comp.CountHierarchyMembers());
			}
			else
			{
				if (index < Component.Count)
				{
					Component[index] = comp;
				}
			}

			InvalidateIfValid();
			if (Peer_Renamed != null)
			{
				if (comp.Peer_Renamed == null) // Remove notify was called or it didn't have peer - create new one
				{
					comp.AddNotify();
				} // Both container and child have peers, it means child peer should be reparented.
				else
				{
					// In both cases we need to reparent native widgets.
					Container newNativeContainer = HeavyweightContainer;
					Container oldNativeContainer = curParent.HeavyweightContainer;
					if (oldNativeContainer != newNativeContainer)
					{
						// Native container changed - need to reparent native widgets
						newNativeContainer.ReparentChild(comp);
					}
					comp.UpdateZOrder();

					if (!comp.Lightweight && Lightweight)
					{
						// If component is heavyweight and one of the containers is lightweight
						// the location of the component should be fixed.
						comp.RelocateComponent();
					}
				}
			}
			if (curParent != this)
			{
				/* Notify the layout manager of the added component. */
				if (LayoutMgr != null)
				{
					if (LayoutMgr is LayoutManager2)
					{
						((LayoutManager2)LayoutMgr).AddLayoutComponent(comp, null);
					}
					else
					{
						LayoutMgr.AddLayoutComponent(null, comp);
					}
				}
				if (ContainerListener != null || (EventMask & AWTEvent.CONTAINER_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK))
				{
					ContainerEvent e = new ContainerEvent(this, ContainerEvent.COMPONENT_ADDED, comp);
					DispatchEvent(e);
				}
				comp.CreateHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, this, HierarchyEvent.PARENT_CHANGED, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK));

				// If component is focus owner or parent container of focus owner check that after reparenting
				// focus owner moved out if new container prohibit this kind of focus owner.
				if (comp.FocusOwner && !comp.CanBeFocusOwnerRecursively())
				{
					comp.TransferFocus();
				}
				else if (comp is Container)
				{
					Component focusOwner = KeyboardFocusManager.CurrentKeyboardFocusManager.FocusOwner;
					if (focusOwner != null && IsParentOf(focusOwner) && !focusOwner.CanBeFocusOwnerRecursively())
					{
						focusOwner.TransferFocus();
					}
				}
			}
			else
			{
				comp.CreateHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, this, HierarchyEvent.HIERARCHY_CHANGED, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK));
			}

			if (Peer_Renamed != null && LayoutMgr == null && Visible)
			{
				UpdateCursorImmediately();
			}
		}

		/// <summary>
		/// Returns the z-order index of the component inside the container.
		/// The higher a component is in the z-order hierarchy, the lower
		/// its index.  The component with the lowest z-order index is
		/// painted last, above all other child components.
		/// </summary>
		/// <param name="comp"> the component being queried </param>
		/// <returns>  the z-order index of the component; otherwise
		///          returns -1 if the component is <code>null</code>
		///          or doesn't belong to the container </returns>
		/// <seealso cref= #setComponentZOrder(java.awt.Component, int)
		/// @since 1.5 </seealso>
		public virtual int GetComponentZOrder(Component comp)
		{
			if (comp == null)
			{
				return -1;
			}
			lock (TreeLock)
			{
				// Quick check - container should be immediate parent of the component
				if (comp.Parent_Renamed != this)
				{
					return -1;
				}
				return Component.IndexOf(comp);
			}
		}

		/// <summary>
		/// Adds the specified component to the end of this container.
		/// Also notifies the layout manager to add the component to
		/// this container's layout using the specified constraints object.
		/// This is a convenience method for <seealso cref="#addImpl"/>.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy. If the container has already been
		/// displayed, the hierarchy must be validated thereafter in order to
		/// display the added component.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="comp"> the component to be added </param>
		/// <param name="constraints"> an object expressing
		///                  layout constraints for this component </param>
		/// <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		/// <seealso cref= #addImpl </seealso>
		/// <seealso cref= #invalidate </seealso>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		/// <seealso cref=       LayoutManager
		/// @since     JDK1.1 </seealso>
		public virtual void Add(Component comp, Object constraints)
		{
			AddImpl(comp, constraints, -1);
		}

		/// <summary>
		/// Adds the specified component to this container with the specified
		/// constraints at the specified index.  Also notifies the layout
		/// manager to add the component to the this container's layout using
		/// the specified constraints object.
		/// This is a convenience method for <seealso cref="#addImpl"/>.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy. If the container has already been
		/// displayed, the hierarchy must be validated thereafter in order to
		/// display the added component.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="comp"> the component to be added </param>
		/// <param name="constraints"> an object expressing layout constraints for this </param>
		/// <param name="index"> the position in the container's list at which to insert
		/// the component; <code>-1</code> means insert at the end
		/// component </param>
		/// <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if {@code index} is invalid (see
		///            <seealso cref="#addImpl"/> for details) </exception>
		/// <seealso cref= #addImpl </seealso>
		/// <seealso cref= #invalidate </seealso>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		/// <seealso cref= #remove </seealso>
		/// <seealso cref= LayoutManager </seealso>
		public virtual void Add(Component comp, Object constraints, int index)
		{
		   AddImpl(comp, constraints, index);
		}

		/// <summary>
		/// Adds the specified component to this container at the specified
		/// index. This method also notifies the layout manager to add
		/// the component to this container's layout using the specified
		/// constraints object via the <code>addLayoutComponent</code>
		/// method.
		/// <para>
		/// The constraints are
		/// defined by the particular layout manager being used.  For
		/// example, the <code>BorderLayout</code> class defines five
		/// constraints: <code>BorderLayout.NORTH</code>,
		/// <code>BorderLayout.SOUTH</code>, <code>BorderLayout.EAST</code>,
		/// <code>BorderLayout.WEST</code>, and <code>BorderLayout.CENTER</code>.
		/// </para>
		/// <para>
		/// The <code>GridBagLayout</code> class requires a
		/// <code>GridBagConstraints</code> object.  Failure to pass
		/// the correct type of constraints object results in an
		/// <code>IllegalArgumentException</code>.
		/// </para>
		/// <para>
		/// If the current layout manager implements {@code LayoutManager2}, then
		/// <seealso cref="LayoutManager2#addLayoutComponent(Component,Object)"/> is invoked on
		/// it. If the current layout manager does not implement
		/// {@code LayoutManager2}, and constraints is a {@code String}, then
		/// <seealso cref="LayoutManager#addLayoutComponent(String,Component)"/> is invoked on it.
		/// </para>
		/// <para>
		/// If the component is not an ancestor of this container and has a non-null
		/// parent, it is removed from its current parent before it is added to this
		/// container.
		/// </para>
		/// <para>
		/// This is the method to override if a program needs to track
		/// every add request to a container as all other add methods defer
		/// to this one. An overriding method should
		/// usually include a call to the superclass's version of the method:
		/// 
		/// <blockquote>
		/// <code>super.addImpl(comp, constraints, index)</code>
		/// </blockquote>
		/// </para>
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy. If the container has already been
		/// displayed, the hierarchy must be validated thereafter in order to
		/// display the added component.
		/// 
		/// </para>
		/// </summary>
		/// <param name="comp">       the component to be added </param>
		/// <param name="constraints"> an object expressing layout constraints
		///                 for this component </param>
		/// <param name="index"> the position in the container's list at which to
		///                 insert the component, where <code>-1</code>
		///                 means append to the end </param>
		/// <exception cref="IllegalArgumentException"> if {@code index} is invalid;
		///            if {@code comp} is a child of this container, the valid
		///            range is {@code [-1, getComponentCount()-1]}; if component is
		///            not a child of this container, the valid range is
		///            {@code [-1, getComponentCount()]}
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if {@code comp} is an ancestor of
		///                                     this container </exception>
		/// <exception cref="IllegalArgumentException"> if adding a window to a container </exception>
		/// <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		/// <seealso cref=       #add(Component) </seealso>
		/// <seealso cref=       #add(Component, int) </seealso>
		/// <seealso cref=       #add(Component, java.lang.Object) </seealso>
		/// <seealso cref= #invalidate </seealso>
		/// <seealso cref=       LayoutManager </seealso>
		/// <seealso cref=       LayoutManager2
		/// @since     JDK1.1 </seealso>
		protected internal virtual void AddImpl(Component comp, Object constraints, int index)
		{
			lock (TreeLock)
			{
				/* Check for correct arguments:  index in bounds,
				 * comp cannot be one of this container's parents,
				 * and comp cannot be a window.
				 * comp and container must be on the same GraphicsDevice.
				 * if comp is container, all sub-components must be on
				 * same GraphicsDevice.
				 */
				GraphicsConfiguration thisGC = this.GraphicsConfiguration;

				if (index > Component.Count || (index < 0 && index != -1))
				{
					throw new IllegalArgumentException("illegal component position");
				}
				CheckAddToSelf(comp);
				CheckNotAWindow(comp);
				if (thisGC != null)
				{
					comp.CheckGD(thisGC.Device.IDstring);
				}

				/* Reparent the component and tidy up the tree's state. */
				if (comp.Parent_Renamed != null)
				{
					comp.Parent_Renamed.remove(comp);
						if (index > Component.Count)
						{
							throw new IllegalArgumentException("illegal component position");
						}
				}

				//index == -1 means add to the end.
				if (index == -1)
				{
					Component.Add(comp);
				}
				else
				{
					Component.Insert(index, comp);
				}
				comp.Parent_Renamed = this;
				comp.GraphicsConfiguration = thisGC;

				AdjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, comp.NumListening(AWTEvent.HIERARCHY_EVENT_MASK));
				AdjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, comp.NumListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK));
				AdjustDescendants(comp.CountHierarchyMembers());

				InvalidateIfValid();
				if (Peer_Renamed != null)
				{
					comp.AddNotify();
				}

				/* Notify the layout manager of the added component. */
				if (LayoutMgr != null)
				{
					if (LayoutMgr is LayoutManager2)
					{
						((LayoutManager2)LayoutMgr).AddLayoutComponent(comp, constraints);
					}
					else if (constraints is String)
					{
						LayoutMgr.AddLayoutComponent((String)constraints, comp);
					}
				}
				if (ContainerListener != null || (EventMask & AWTEvent.CONTAINER_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK))
				{
					ContainerEvent e = new ContainerEvent(this, ContainerEvent.COMPONENT_ADDED, comp);
					DispatchEvent(e);
				}

				comp.CreateHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, this, HierarchyEvent.PARENT_CHANGED, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK));
				if (Peer_Renamed != null && LayoutMgr == null && Visible)
				{
					UpdateCursorImmediately();
				}
			}
		}

		internal override bool UpdateGraphicsData(GraphicsConfiguration gc)
		{
			CheckTreeLock();

			bool ret = base.UpdateGraphicsData(gc);

			foreach (Component comp in Component)
			{
				if (comp != null)
				{
					ret |= comp.UpdateGraphicsData(gc);
				}
			}
			return ret;
		}

		/// <summary>
		/// Checks that all Components that this Container contains are on
		/// the same GraphicsDevice as this Container.  If not, throws an
		/// IllegalArgumentException.
		/// </summary>
		internal override void CheckGD(String stringID)
		{
			foreach (Component comp in Component)
			{
				if (comp != null)
				{
					comp.CheckGD(stringID);
				}
			}
		}

		/// <summary>
		/// Removes the component, specified by <code>index</code>,
		/// from this container.
		/// This method also notifies the layout manager to remove the
		/// component from this container's layout via the
		/// <code>removeLayoutComponent</code> method.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy. If the container has already been
		/// displayed, the hierarchy must be validated thereafter in order to
		/// reflect the changes.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="index">   the index of the component to be removed </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code index} is not in
		///         range {@code [0, getComponentCount()-1]} </exception>
		/// <seealso cref= #add </seealso>
		/// <seealso cref= #invalidate </seealso>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= #getComponentCount
		/// @since JDK1.1 </seealso>
		public virtual void Remove(int index)
		{
			lock (TreeLock)
			{
				if (index < 0 || index >= Component.Count)
				{
					throw new ArrayIndexOutOfBoundsException(index);
				}
				Component comp = Component[index];
				if (Peer_Renamed != null)
				{
					comp.RemoveNotify();
				}
				if (LayoutMgr != null)
				{
					LayoutMgr.RemoveLayoutComponent(comp);
				}

				AdjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, -comp.NumListening(AWTEvent.HIERARCHY_EVENT_MASK));
				AdjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, -comp.NumListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK));
				AdjustDescendants(-(comp.CountHierarchyMembers()));

				comp.Parent_Renamed = null;
				Component.RemoveAt(index);
				comp.GraphicsConfiguration = null;

				InvalidateIfValid();
				if (ContainerListener != null || (EventMask & AWTEvent.CONTAINER_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK))
				{
					ContainerEvent e = new ContainerEvent(this, ContainerEvent.COMPONENT_REMOVED, comp);
					DispatchEvent(e);
				}

				comp.CreateHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, this, HierarchyEvent.PARENT_CHANGED, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK));
				if (Peer_Renamed != null && LayoutMgr == null && Visible)
				{
					UpdateCursorImmediately();
				}
			}
		}

		/// <summary>
		/// Removes the specified component from this container.
		/// This method also notifies the layout manager to remove the
		/// component from this container's layout via the
		/// <code>removeLayoutComponent</code> method.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy. If the container has already been
		/// displayed, the hierarchy must be validated thereafter in order to
		/// reflect the changes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="comp"> the component to be removed </param>
		/// <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		/// <seealso cref= #add </seealso>
		/// <seealso cref= #invalidate </seealso>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= #remove(int) </seealso>
		public virtual void Remove(Component comp)
		{
			lock (TreeLock)
			{
				if (comp.Parent_Renamed == this)
				{
					int index = Component.IndexOf(comp);
					if (index >= 0)
					{
						Remove(index);
					}
				}
			}
		}

		/// <summary>
		/// Removes all the components from this container.
		/// This method also notifies the layout manager to remove the
		/// components from this container's layout via the
		/// <code>removeLayoutComponent</code> method.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy. If the container has already been
		/// displayed, the hierarchy must be validated thereafter in order to
		/// reflect the changes.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #add </seealso>
		/// <seealso cref= #remove </seealso>
		/// <seealso cref= #invalidate </seealso>
		public virtual void RemoveAll()
		{
			lock (TreeLock)
			{
				AdjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, -ListeningChildren);
				AdjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, -ListeningBoundsChildren);
				AdjustDescendants(-DescendantsCount);

				while (Component.Count > 0)
				{
					Component comp = Component.Remove(Component.Count - 1);

					if (Peer_Renamed != null)
					{
						comp.RemoveNotify();
					}
					if (LayoutMgr != null)
					{
						LayoutMgr.RemoveLayoutComponent(comp);
					}
					comp.Parent_Renamed = null;
					comp.GraphicsConfiguration = null;
					if (ContainerListener != null || (EventMask & AWTEvent.CONTAINER_EVENT_MASK) != 0 || Toolkit.EnabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK))
					{
						ContainerEvent e = new ContainerEvent(this, ContainerEvent.COMPONENT_REMOVED, comp);
						DispatchEvent(e);
					}

					comp.CreateHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, this, HierarchyEvent.PARENT_CHANGED, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK));
				}
				if (Peer_Renamed != null && LayoutMgr == null && Visible)
				{
					UpdateCursorImmediately();
				}
				InvalidateIfValid();
			}
		}

		// Should only be called while holding tree lock
		internal override int NumListening(long mask)
		{
			int superListening = base.NumListening(mask);

			if (mask == AWTEvent.HIERARCHY_EVENT_MASK)
			{
				if (EventLog.isLoggable(PlatformLogger.Level.FINE))
				{
					// Verify listeningChildren is correct
					int sum = 0;
					foreach (Component comp in Component)
					{
						sum += comp.NumListening(mask);
					}
					if (ListeningChildren != sum)
					{
						EventLog.fine("Assertion (listeningChildren == sum) failed");
					}
				}
				return ListeningChildren + superListening;
			}
			else if (mask == AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK)
			{
				if (EventLog.isLoggable(PlatformLogger.Level.FINE))
				{
					// Verify listeningBoundsChildren is correct
					int sum = 0;
					foreach (Component comp in Component)
					{
						sum += comp.NumListening(mask);
					}
					if (ListeningBoundsChildren != sum)
					{
						EventLog.fine("Assertion (listeningBoundsChildren == sum) failed");
					}
				}
				return ListeningBoundsChildren + superListening;
			}
			else
			{
				// assert false;
				if (EventLog.isLoggable(PlatformLogger.Level.FINE))
				{
					EventLog.fine("This code must never be reached");
				}
				return superListening;
			}
		}

		// Should only be called while holding tree lock
		internal virtual void AdjustListeningChildren(long mask, int num)
		{
			if (EventLog.isLoggable(PlatformLogger.Level.FINE))
			{
				bool toAssert = (mask == AWTEvent.HIERARCHY_EVENT_MASK || mask == AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK || mask == (AWTEvent.HIERARCHY_EVENT_MASK | AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK));
				if (!toAssert)
				{
					EventLog.fine("Assertion failed");
				}
			}

			if (num == 0)
			{
				return;
			}

			if ((mask & AWTEvent.HIERARCHY_EVENT_MASK) != 0)
			{
				ListeningChildren += num;
			}
			if ((mask & AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) != 0)
			{
				ListeningBoundsChildren += num;
			}

			AdjustListeningChildrenOnParent(mask, num);
		}

		// Should only be called while holding tree lock
		internal virtual void AdjustDescendants(int num)
		{
			if (num == 0)
			{
				return;
			}

			DescendantsCount += num;
			AdjustDecendantsOnParent(num);
		}

		// Should only be called while holding tree lock
		internal virtual void AdjustDecendantsOnParent(int num)
		{
			if (Parent_Renamed != null)
			{
				Parent_Renamed.AdjustDescendants(num);
			}
		}

		// Should only be called while holding tree lock
		internal override int CountHierarchyMembers()
		{
			if (Log.isLoggable(PlatformLogger.Level.FINE))
			{
				// Verify descendantsCount is correct
				int sum = 0;
				foreach (Component comp in Component)
				{
					sum += comp.CountHierarchyMembers();
				}
				if (DescendantsCount != sum)
				{
					Log.fine("Assertion (descendantsCount == sum) failed");
				}
			}
			return DescendantsCount + 1;
		}

		private int GetListenersCount(int id, bool enabledOnToolkit)
		{
			CheckTreeLock();
			if (enabledOnToolkit)
			{
				return DescendantsCount;
			}
			switch (id)
			{
			  case HierarchyEvent.HIERARCHY_CHANGED:
				return ListeningChildren;
			  case HierarchyEvent.ANCESTOR_MOVED:
			  case HierarchyEvent.ANCESTOR_RESIZED:
				return ListeningBoundsChildren;
			  default:
				return 0;
			}
		}

		internal sealed override int CreateHierarchyEvents(int id, Component changed, Container changedParent, long changeFlags, bool enabledOnToolkit)
		{
			CheckTreeLock();
			int listeners = GetListenersCount(id, enabledOnToolkit);

			for (int count = listeners, i = 0; count > 0; i++)
			{
				count -= Component[i].CreateHierarchyEvents(id, changed, changedParent, changeFlags, enabledOnToolkit);
			}
			return listeners + base.CreateHierarchyEvents(id, changed, changedParent, changeFlags, enabledOnToolkit);
		}

		internal void CreateChildHierarchyEvents(int id, long changeFlags, bool enabledOnToolkit)
		{
			CheckTreeLock();
			if (Component.Count == 0)
			{
				return;
			}
			int listeners = GetListenersCount(id, enabledOnToolkit);

			for (int count = listeners, i = 0; count > 0; i++)
			{
				count -= Component[i].CreateHierarchyEvents(id, this, Parent_Renamed, changeFlags, enabledOnToolkit);
			}
		}

		/// <summary>
		/// Gets the layout manager for this container. </summary>
		/// <seealso cref= #doLayout </seealso>
		/// <seealso cref= #setLayout </seealso>
		public virtual LayoutManager Layout
		{
			get
			{
				return LayoutMgr;
			}
			set
			{
				LayoutMgr = value;
				InvalidateIfValid();
			}
		}


		/// <summary>
		/// Causes this container to lay out its components.  Most programs
		/// should not call this method directly, but should invoke
		/// the <code>validate</code> method instead. </summary>
		/// <seealso cref= LayoutManager#layoutContainer </seealso>
		/// <seealso cref= #setLayout </seealso>
		/// <seealso cref= #validate
		/// @since JDK1.1 </seealso>
		public override void DoLayout()
		{
			Layout();
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>doLayout()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public override void Layout()
		{
			LayoutManager layoutMgr = this.LayoutMgr;
			if (layoutMgr != null)
			{
				layoutMgr.LayoutContainer(this);
			}
		}

		/// <summary>
		/// Indicates if this container is a <i>validate root</i>.
		/// <para>
		/// Layout-related changes, such as bounds of the validate root descendants,
		/// do not affect the layout of the validate root parent. This peculiarity
		/// enables the {@code invalidate()} method to stop invalidating the
		/// component hierarchy when the method encounters a validate root. However,
		/// to preserve backward compatibility this new optimized behavior is
		/// enabled only when the {@code java.awt.smartInvalidate} system property
		/// value is set to {@code true}.
		/// </para>
		/// <para>
		/// If a component hierarchy contains validate roots and the new optimized
		/// {@code invalidate()} behavior is enabled, the {@code validate()} method
		/// must be invoked on the validate root of a previously invalidated
		/// component to restore the validity of the hierarchy later. Otherwise,
		/// calling the {@code validate()} method on the top-level container (such
		/// as a {@code Frame} object) should be used to restore the validity of the
		/// component hierarchy.
		/// </para>
		/// <para>
		/// The {@code Window} class and the {@code Applet} class are the validate
		/// roots in AWT.  Swing introduces more validate roots.
		/// 
		/// </para>
		/// </summary>
		/// <returns> whether this container is a validate root </returns>
		/// <seealso cref= #invalidate </seealso>
		/// <seealso cref= java.awt.Component#invalidate </seealso>
		/// <seealso cref= javax.swing.JComponent#isValidateRoot </seealso>
		/// <seealso cref= javax.swing.JComponent#revalidate
		/// @since 1.7 </seealso>
		public virtual bool ValidateRoot
		{
			get
			{
				return false;
			}
		}

		private static readonly bool IsJavaAwtSmartInvalidate;

		/// <summary>
		/// Invalidates the parent of the container unless the container
		/// is a validate root.
		/// </summary>
		internal override void InvalidateParent()
		{
			if (!IsJavaAwtSmartInvalidate || !ValidateRoot)
			{
				base.InvalidateParent();
			}
		}

		/// <summary>
		/// Invalidates the container.
		/// <para>
		/// If the {@code LayoutManager} installed on this container is an instance
		/// of the {@code LayoutManager2} interface, then
		/// the <seealso cref="LayoutManager2#invalidateLayout(Container)"/> method is invoked
		/// on it supplying this {@code Container} as the argument.
		/// </para>
		/// <para>
		/// Afterwards this method marks this container invalid, and invalidates its
		/// ancestors. See the <seealso cref="Component#invalidate"/> method for more details.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #validate </seealso>
		/// <seealso cref= #layout </seealso>
		/// <seealso cref= LayoutManager2 </seealso>
		public override void Invalidate()
		{
			LayoutManager layoutMgr = this.LayoutMgr;
			if (layoutMgr is LayoutManager2)
			{
				LayoutManager2 lm = (LayoutManager2) layoutMgr;
				lm.InvalidateLayout(this);
			}
			base.Invalidate();
		}

		/// <summary>
		/// Validates this container and all of its subcomponents.
		/// <para>
		/// Validating a container means laying out its subcomponents.
		/// Layout-related changes, such as setting the bounds of a component, or
		/// adding a component to the container, invalidate the container
		/// automatically.  Note that the ancestors of the container may be
		/// invalidated also (see <seealso cref="Component#invalidate"/> for details.)
		/// Therefore, to restore the validity of the hierarchy, the {@code
		/// validate()} method should be invoked on the top-most invalid
		/// container of the hierarchy.
		/// </para>
		/// <para>
		/// Validating the container may be a quite time-consuming operation. For
		/// performance reasons a developer may postpone the validation of the
		/// hierarchy till a set of layout-related operations completes, e.g. after
		/// adding all the children to the container.
		/// </para>
		/// <para>
		/// If this {@code Container} is not valid, this method invokes
		/// the {@code validateTree} method and marks this {@code Container}
		/// as valid. Otherwise, no action is performed.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #add(java.awt.Component) </seealso>
		/// <seealso cref= #invalidate </seealso>
		/// <seealso cref= Container#isValidateRoot </seealso>
		/// <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		/// <seealso cref= #validateTree </seealso>
		public override void Validate()
		{
			bool updateCur = false;
			lock (TreeLock)
			{
				if ((!Valid || DescendUnconditionallyWhenValidating) && Peer_Renamed != null)
				{
					ContainerPeer p = null;
					if (Peer_Renamed is ContainerPeer)
					{
						p = (ContainerPeer) Peer_Renamed;
					}
					if (p != null)
					{
						p.BeginValidate();
					}
					ValidateTree();
					if (p != null)
					{
						p.EndValidate();
						// Avoid updating cursor if this is an internal call.
						// See validateUnconditionally() for details.
						if (!DescendUnconditionallyWhenValidating)
						{
							updateCur = Visible;
						}
					}
				}
			}
			if (updateCur)
			{
				UpdateCursorImmediately();
			}
		}

		/// <summary>
		/// Indicates whether valid containers should also traverse their
		/// children and call the validateTree() method on them.
		/// 
		/// Synchronization: TreeLock.
		/// 
		/// The field is allowed to be static as long as the TreeLock itself is
		/// static.
		/// </summary>
		/// <seealso cref= #validateUnconditionally() </seealso>
		private static bool DescendUnconditionallyWhenValidating = false;

		/// <summary>
		/// Unconditionally validate the component hierarchy.
		/// </summary>
		internal void ValidateUnconditionally()
		{
			bool updateCur = false;
			lock (TreeLock)
			{
				DescendUnconditionallyWhenValidating = true;

				Validate();
				if (Peer_Renamed is ContainerPeer)
				{
					updateCur = Visible;
				}

				DescendUnconditionallyWhenValidating = false;
			}
			if (updateCur)
			{
				UpdateCursorImmediately();
			}
		}

		/// <summary>
		/// Recursively descends the container tree and recomputes the
		/// layout for any subtrees marked as needing it (those marked as
		/// invalid).  Synchronization should be provided by the method
		/// that calls this one:  <code>validate</code>.
		/// </summary>
		/// <seealso cref= #doLayout </seealso>
		/// <seealso cref= #validate </seealso>
		protected internal virtual void ValidateTree()
		{
			CheckTreeLock();
			if (!Valid || DescendUnconditionallyWhenValidating)
			{
				if (Peer_Renamed is ContainerPeer)
				{
					((ContainerPeer)Peer_Renamed).BeginLayout();
				}
				if (!Valid)
				{
					DoLayout();
				}
				for (int i = 0; i < Component.Count; i++)
				{
					Component comp = Component[i];
					if ((comp is Container) && !(comp is Window) && (!comp.Valid || DescendUnconditionallyWhenValidating))
					{
						((Container)comp).ValidateTree();
					}
					else
					{
						comp.Validate();
					}
				}
				if (Peer_Renamed is ContainerPeer)
				{
					((ContainerPeer)Peer_Renamed).EndLayout();
				}
			}
			base.Validate();
		}

		/// <summary>
		/// Recursively descends the container tree and invalidates all
		/// contained components.
		/// </summary>
		internal virtual void InvalidateTree()
		{
			lock (TreeLock)
			{
				for (int i = 0; i < Component.Count; i++)
				{
					Component comp = Component[i];
					if (comp is Container)
					{
						((Container)comp).InvalidateTree();
					}
					else
					{
						comp.InvalidateIfValid();
					}
				}
				InvalidateIfValid();
			}
		}

		/// <summary>
		/// Sets the font of this container.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy.
		/// 
		/// </para>
		/// </summary>
		/// <param name="f"> The font to become this container's font. </param>
		/// <seealso cref= Component#getFont </seealso>
		/// <seealso cref= #invalidate
		/// @since JDK1.0 </seealso>
		public override Font Font
		{
			set
			{
				bool shouldinvalidate = false;
    
				Font oldfont = Font;
				base.Font = value;
				Font newfont = Font;
				if (newfont != oldfont && (oldfont == null || !oldfont.Equals(newfont)))
				{
					InvalidateTree();
				}
			}
		}

		/// <summary>
		/// Returns the preferred size of this container.  If the preferred size has
		/// not been set explicitly by <seealso cref="Component#setPreferredSize(Dimension)"/>
		/// and this {@code Container} has a {@code non-null} <seealso cref="LayoutManager"/>,
		/// then <seealso cref="LayoutManager#preferredLayoutSize(Container)"/>
		/// is used to calculate the preferred size.
		/// 
		/// <para>Note: some implementations may cache the value returned from the
		/// {@code LayoutManager}.  Implementations that cache need not invoke
		/// {@code preferredLayoutSize} on the {@code LayoutManager} every time
		/// this method is invoked, rather the {@code LayoutManager} will only
		/// be queried after the {@code Container} becomes invalid.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    an instance of <code>Dimension</code> that represents
		///                the preferred size of this container. </returns>
		/// <seealso cref=       #getMinimumSize </seealso>
		/// <seealso cref=       #getMaximumSize </seealso>
		/// <seealso cref=       #getLayout </seealso>
		/// <seealso cref=       LayoutManager#preferredLayoutSize(Container) </seealso>
		/// <seealso cref=       Component#getPreferredSize </seealso>
		public override Dimension PreferredSize
		{
			get
			{
				return PreferredSize();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getPreferredSize()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public override Dimension PreferredSize()
		{
			/* Avoid grabbing the lock if a reasonable cached size value
			 * is available.
			 */
			Dimension dim = PrefSize;
			if (dim == null || !(PreferredSizeSet || Valid))
			{
				lock (TreeLock)
				{
					PrefSize = (LayoutMgr != null) ? LayoutMgr.PreferredLayoutSize(this) : base.PreferredSize();
					dim = PrefSize;
				}
			}
			if (dim != null)
			{
				return new Dimension(dim);
			}
			else
			{
				return dim;
			}
		}

		/// <summary>
		/// Returns the minimum size of this container.  If the minimum size has
		/// not been set explicitly by <seealso cref="Component#setMinimumSize(Dimension)"/>
		/// and this {@code Container} has a {@code non-null} <seealso cref="LayoutManager"/>,
		/// then <seealso cref="LayoutManager#minimumLayoutSize(Container)"/>
		/// is used to calculate the minimum size.
		/// 
		/// <para>Note: some implementations may cache the value returned from the
		/// {@code LayoutManager}.  Implementations that cache need not invoke
		/// {@code minimumLayoutSize} on the {@code LayoutManager} every time
		/// this method is invoked, rather the {@code LayoutManager} will only
		/// be queried after the {@code Container} becomes invalid.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    an instance of <code>Dimension</code> that represents
		///                the minimum size of this container. </returns>
		/// <seealso cref=       #getPreferredSize </seealso>
		/// <seealso cref=       #getMaximumSize </seealso>
		/// <seealso cref=       #getLayout </seealso>
		/// <seealso cref=       LayoutManager#minimumLayoutSize(Container) </seealso>
		/// <seealso cref=       Component#getMinimumSize
		/// @since     JDK1.1 </seealso>
		public override Dimension MinimumSize
		{
			get
			{
				return MinimumSize();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getMinimumSize()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public override Dimension MinimumSize()
		{
			/* Avoid grabbing the lock if a reasonable cached size value
			 * is available.
			 */
			Dimension dim = MinSize;
			if (dim == null || !(MinimumSizeSet || Valid))
			{
				lock (TreeLock)
				{
					MinSize = (LayoutMgr != null) ? LayoutMgr.MinimumLayoutSize(this) : base.MinimumSize();
					dim = MinSize;
				}
			}
			if (dim != null)
			{
				return new Dimension(dim);
			}
			else
			{
				return dim;
			}
		}

		/// <summary>
		/// Returns the maximum size of this container.  If the maximum size has
		/// not been set explicitly by <seealso cref="Component#setMaximumSize(Dimension)"/>
		/// and the <seealso cref="LayoutManager"/> installed on this {@code Container}
		/// is an instance of <seealso cref="LayoutManager2"/>, then
		/// <seealso cref="LayoutManager2#maximumLayoutSize(Container)"/>
		/// is used to calculate the maximum size.
		/// 
		/// <para>Note: some implementations may cache the value returned from the
		/// {@code LayoutManager2}.  Implementations that cache need not invoke
		/// {@code maximumLayoutSize} on the {@code LayoutManager2} every time
		/// this method is invoked, rather the {@code LayoutManager2} will only
		/// be queried after the {@code Container} becomes invalid.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    an instance of <code>Dimension</code> that represents
		///                the maximum size of this container. </returns>
		/// <seealso cref=       #getPreferredSize </seealso>
		/// <seealso cref=       #getMinimumSize </seealso>
		/// <seealso cref=       #getLayout </seealso>
		/// <seealso cref=       LayoutManager2#maximumLayoutSize(Container) </seealso>
		/// <seealso cref=       Component#getMaximumSize </seealso>
		public override Dimension MaximumSize
		{
			get
			{
				/* Avoid grabbing the lock if a reasonable cached size value
				 * is available.
				 */
				Dimension dim = MaxSize;
				if (dim == null || !(MaximumSizeSet || Valid))
				{
					lock (TreeLock)
					{
					   if (LayoutMgr is LayoutManager2)
					   {
							LayoutManager2 lm = (LayoutManager2) LayoutMgr;
							MaxSize = lm.MaximumLayoutSize(this);
					   }
					   else
					   {
							MaxSize = base.MaximumSize;
					   }
					   dim = MaxSize;
					}
				}
				if (dim != null)
				{
					return new Dimension(dim);
				}
				else
				{
					return dim;
				}
			}
		}

		/// <summary>
		/// Returns the alignment along the x axis.  This specifies how
		/// the component would like to be aligned relative to other
		/// components.  The value should be a number between 0 and 1
		/// where 0 represents alignment along the origin, 1 is aligned
		/// the furthest away from the origin, 0.5 is centered, etc.
		/// </summary>
		public override float AlignmentX
		{
			get
			{
				float xAlign;
				if (LayoutMgr is LayoutManager2)
				{
					lock (TreeLock)
					{
						LayoutManager2 lm = (LayoutManager2) LayoutMgr;
						xAlign = lm.GetLayoutAlignmentX(this);
					}
				}
				else
				{
					xAlign = base.AlignmentX;
				}
				return xAlign;
			}
		}

		/// <summary>
		/// Returns the alignment along the y axis.  This specifies how
		/// the component would like to be aligned relative to other
		/// components.  The value should be a number between 0 and 1
		/// where 0 represents alignment along the origin, 1 is aligned
		/// the furthest away from the origin, 0.5 is centered, etc.
		/// </summary>
		public override float AlignmentY
		{
			get
			{
				float yAlign;
				if (LayoutMgr is LayoutManager2)
				{
					lock (TreeLock)
					{
						LayoutManager2 lm = (LayoutManager2) LayoutMgr;
						yAlign = lm.GetLayoutAlignmentY(this);
					}
				}
				else
				{
					yAlign = base.AlignmentY;
				}
				return yAlign;
			}
		}

		/// <summary>
		/// Paints the container. This forwards the paint to any lightweight
		/// components that are children of this container. If this method is
		/// reimplemented, super.paint(g) should be called so that lightweight
		/// components are properly rendered. If a child component is entirely
		/// clipped by the current clipping setting in g, paint() will not be
		/// forwarded to that child.
		/// </summary>
		/// <param name="g"> the specified Graphics window </param>
		/// <seealso cref=   Component#update(Graphics) </seealso>
		public override void Paint(Graphics g)
		{
			if (Showing)
			{
				lock (ObjectLock)
				{
					if (Printing)
					{
						if (PrintingThreads.Contains(Thread.CurrentThread))
						{
							return;
						}
					}
				}

				// The container is showing on screen and
				// this paint() is not called from print().
				// Paint self and forward the paint to lightweight subcomponents.

				// super.paint(); -- Don't bother, since it's a NOP.

				GraphicsCallback.PaintCallback.Instance.runComponents(ComponentsSync, g, GraphicsCallback.LIGHTWEIGHTS);
			}
		}

		/// <summary>
		/// Updates the container.  This forwards the update to any lightweight
		/// components that are children of this container.  If this method is
		/// reimplemented, super.update(g) should be called so that lightweight
		/// components are properly rendered.  If a child component is entirely
		/// clipped by the current clipping setting in g, update() will not be
		/// forwarded to that child.
		/// </summary>
		/// <param name="g"> the specified Graphics window </param>
		/// <seealso cref=   Component#update(Graphics) </seealso>
		public override void Update(Graphics g)
		{
			if (Showing)
			{
				if (!(Peer_Renamed is LightweightPeer))
				{
					g.ClearRect(0, 0, Width_Renamed, Height_Renamed);
				}
				Paint(g);
			}
		}

		/// <summary>
		/// Prints the container. This forwards the print to any lightweight
		/// components that are children of this container. If this method is
		/// reimplemented, super.print(g) should be called so that lightweight
		/// components are properly rendered. If a child component is entirely
		/// clipped by the current clipping setting in g, print() will not be
		/// forwarded to that child.
		/// </summary>
		/// <param name="g"> the specified Graphics window </param>
		/// <seealso cref=   Component#update(Graphics) </seealso>
		public override void Print(Graphics g)
		{
			if (Showing)
			{
				Thread t = Thread.CurrentThread;
				try
				{
					lock (ObjectLock)
					{
						if (PrintingThreads == null)
						{
							PrintingThreads = new HashSet<>();
						}
						PrintingThreads.Add(t);
						Printing = true;
					}
					base.Print(g); // By default, Component.print() calls paint()
				}
				finally
				{
					lock (ObjectLock)
					{
						PrintingThreads.Remove(t);
						Printing = PrintingThreads.Count > 0;
					}
				}

				GraphicsCallback.PrintCallback.Instance.runComponents(ComponentsSync, g, GraphicsCallback.LIGHTWEIGHTS);
			}
		}

		/// <summary>
		/// Paints each of the components in this container. </summary>
		/// <param name="g">   the graphics context. </param>
		/// <seealso cref=       Component#paint </seealso>
		/// <seealso cref=       Component#paintAll </seealso>
		public virtual void PaintComponents(Graphics g)
		{
			if (Showing)
			{
				GraphicsCallback.PaintAllCallback.Instance.runComponents(ComponentsSync, g, GraphicsCallback.TWO_PASSES);
			}
		}

		/// <summary>
		/// Simulates the peer callbacks into java.awt for printing of
		/// lightweight Containers. </summary>
		/// <param name="g">   the graphics context to use for printing. </param>
		/// <seealso cref=       Component#printAll </seealso>
		/// <seealso cref=       #printComponents </seealso>
		internal override void LightweightPaint(Graphics g)
		{
			base.LightweightPaint(g);
			PaintHeavyweightComponents(g);
		}

		/// <summary>
		/// Prints all the heavyweight subcomponents.
		/// </summary>
		internal override void PaintHeavyweightComponents(Graphics g)
		{
			if (Showing)
			{
				GraphicsCallback.PaintHeavyweightComponentsCallback.Instance.runComponents(ComponentsSync, g, GraphicsCallback.LIGHTWEIGHTS | GraphicsCallback.HEAVYWEIGHTS);
			}
		}

		/// <summary>
		/// Prints each of the components in this container. </summary>
		/// <param name="g">   the graphics context. </param>
		/// <seealso cref=       Component#print </seealso>
		/// <seealso cref=       Component#printAll </seealso>
		public virtual void PrintComponents(Graphics g)
		{
			if (Showing)
			{
				GraphicsCallback.PrintAllCallback.Instance.runComponents(ComponentsSync, g, GraphicsCallback.TWO_PASSES);
			}
		}

		/// <summary>
		/// Simulates the peer callbacks into java.awt for printing of
		/// lightweight Containers. </summary>
		/// <param name="g">   the graphics context to use for printing. </param>
		/// <seealso cref=       Component#printAll </seealso>
		/// <seealso cref=       #printComponents </seealso>
		internal override void LightweightPrint(Graphics g)
		{
			base.LightweightPrint(g);
			PrintHeavyweightComponents(g);
		}

		/// <summary>
		/// Prints all the heavyweight subcomponents.
		/// </summary>
		internal override void PrintHeavyweightComponents(Graphics g)
		{
			if (Showing)
			{
				GraphicsCallback.PrintHeavyweightComponentsCallback.Instance.runComponents(ComponentsSync, g, GraphicsCallback.LIGHTWEIGHTS | GraphicsCallback.HEAVYWEIGHTS);
			}
		}

		/// <summary>
		/// Adds the specified container listener to receive container events
		/// from this container.
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the container listener
		/// </param>
		/// <seealso cref= #removeContainerListener </seealso>
		/// <seealso cref= #getContainerListeners </seealso>
		public virtual void AddContainerListener(ContainerListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				ContainerListener = AWTEventMulticaster.Add(ContainerListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Removes the specified container listener so it no longer receives
		/// container events from this container.
		/// If l is null, no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the container listener
		/// </param>
		/// <seealso cref= #addContainerListener </seealso>
		/// <seealso cref= #getContainerListeners </seealso>
		public virtual void RemoveContainerListener(ContainerListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				ContainerListener = AWTEventMulticaster.Remove(ContainerListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the container listeners
		/// registered on this container.
		/// </summary>
		/// <returns> all of this container's <code>ContainerListener</code>s
		///         or an empty array if no container
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref= #addContainerListener </seealso>
		/// <seealso cref= #removeContainerListener
		/// @since 1.4 </seealso>
		public virtual ContainerListener[] ContainerListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(ContainerListener));
				}
			}
		}

		/// <summary>
		/// Returns an array of all the objects currently registered
		/// as <code><em>Foo</em>Listener</code>s
		/// upon this <code>Container</code>.
		/// <code><em>Foo</em>Listener</code>s are registered using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// 
		/// <para>
		/// You can specify the <code>listenerType</code> argument
		/// with a class literal, such as
		/// <code><em>Foo</em>Listener.class</code>.
		/// For example, you can query a
		/// <code>Container</code> <code>c</code>
		/// for its container listeners with the following code:
		/// 
		/// <pre>ContainerListener[] cls = (ContainerListener[])(c.getListeners(ContainerListener.class));</pre>
		/// 
		/// If no such listeners exist, this method returns an empty array.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listenerType"> the type of listeners requested; this parameter
		///          should specify an interface that descends from
		///          <code>java.util.EventListener</code> </param>
		/// <returns> an array of all objects registered as
		///          <code><em>Foo</em>Listener</code>s on this container,
		///          or an empty array if no such listeners have been added </returns>
		/// <exception cref="ClassCastException"> if <code>listenerType</code>
		///          doesn't specify a class or interface that implements
		///          <code>java.util.EventListener</code> </exception>
		/// <exception cref="NullPointerException"> if {@code listenerType} is {@code null}
		/// </exception>
		/// <seealso cref= #getContainerListeners
		/// 
		/// @since 1.3 </seealso>
		public override T[] getListeners<T>(Class listenerType) where T : java.util.EventListener
		{
			EventListener l = null;
			if (listenerType == typeof(ContainerListener))
			{
				l = ContainerListener;
			}
			else
			{
				return base.GetListeners(listenerType);
			}
			return AWTEventMulticaster.GetListeners(l, listenerType);
		}

		// REMIND: remove when filtering is done at lower level
		internal override bool EventEnabled(AWTEvent e)
		{
			int id = e.ID;

			if (id == ContainerEvent.COMPONENT_ADDED || id == ContainerEvent.COMPONENT_REMOVED)
			{
				if ((EventMask & AWTEvent.CONTAINER_EVENT_MASK) != 0 || ContainerListener != null)
				{
					return true;
				}
				return false;
			}
			return base.EventEnabled(e);
		}

		/// <summary>
		/// Processes events on this container. If the event is a
		/// <code>ContainerEvent</code>, it invokes the
		/// <code>processContainerEvent</code> method, else it invokes
		/// its superclass's <code>processEvent</code>.
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the event </param>
		protected internal override void ProcessEvent(AWTEvent e)
		{
			if (e is ContainerEvent)
			{
				ProcessContainerEvent((ContainerEvent)e);
				return;
			}
			base.ProcessEvent(e);
		}

		/// <summary>
		/// Processes container events occurring on this container by
		/// dispatching them to any registered ContainerListener objects.
		/// NOTE: This method will not be called unless container events
		/// are enabled for this component; this happens when one of the
		/// following occurs:
		/// <ul>
		/// <li>A ContainerListener object is registered via
		///     <code>addContainerListener</code>
		/// <li>Container events are enabled via <code>enableEvents</code>
		/// </ul>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the container event </param>
		/// <seealso cref= Component#enableEvents </seealso>
		protected internal virtual void ProcessContainerEvent(ContainerEvent e)
		{
			ContainerListener listener = ContainerListener;
			if (listener != null)
			{
				switch (e.ID)
				{
				  case ContainerEvent.COMPONENT_ADDED:
					listener.ComponentAdded(e);
					break;
				  case ContainerEvent.COMPONENT_REMOVED:
					listener.ComponentRemoved(e);
					break;
				}
			}
		}

		/*
		 * Dispatches an event to this component or one of its sub components.
		 * Create ANCESTOR_RESIZED and ANCESTOR_MOVED events in response to
		 * COMPONENT_RESIZED and COMPONENT_MOVED events. We have to do this
		 * here instead of in processComponentEvent because ComponentEvents
		 * may not be enabled for this Container.
		 * @param e the event
		 */
		internal override void DispatchEventImpl(AWTEvent e)
		{
			if ((Dispatcher != null) && Dispatcher.DispatchEvent(e))
			{
				// event was sent to a lightweight component.  The
				// native-produced event sent to the native container
				// must be properly disposed of by the peer, so it
				// gets forwarded.  If the native host has been removed
				// as a result of the sending the lightweight event,
				// the peer reference will be null.
				e.Consume();
				if (Peer_Renamed != null)
				{
					Peer_Renamed.HandleEvent(e);
				}
				return;
			}

			base.DispatchEventImpl(e);

			lock (TreeLock)
			{
				switch (e.ID)
				{
				  case ComponentEvent.COMPONENT_RESIZED:
					CreateChildHierarchyEvents(HierarchyEvent.ANCESTOR_RESIZED, 0, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK));
					break;
				  case ComponentEvent.COMPONENT_MOVED:
					CreateChildHierarchyEvents(HierarchyEvent.ANCESTOR_MOVED, 0, Toolkit.EnabledOnToolkit(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK));
					break;
				  default:
					break;
				}
			}
		}

		/*
		 * Dispatches an event to this component, without trying to forward
		 * it to any subcomponents
		 * @param e the event
		 */
		internal virtual void DispatchEventToSelf(AWTEvent e)
		{
			base.DispatchEventImpl(e);
		}

		/// <summary>
		/// Fetchs the top-most (deepest) lightweight component that is interested
		/// in receiving mouse events.
		/// </summary>
		internal virtual Component GetMouseEventTarget(int x, int y, bool includeSelf)
		{
			return GetMouseEventTarget(x, y, includeSelf, MouseEventTargetFilter.FILTER, !SEARCH_HEAVYWEIGHTS);
		}

		/// <summary>
		/// Fetches the top-most (deepest) component to receive SunDropTargetEvents.
		/// </summary>
		internal virtual Component GetDropTargetEventTarget(int x, int y, bool includeSelf)
		{
			return GetMouseEventTarget(x, y, includeSelf, DropTargetEventTargetFilter.FILTER, SEARCH_HEAVYWEIGHTS);
		}

		/// <summary>
		/// A private version of getMouseEventTarget which has two additional
		/// controllable behaviors. This method searches for the top-most
		/// descendant of this container that contains the given coordinates
		/// and is accepted by the given filter. The search will be constrained to
		/// lightweight descendants if the last argument is <code>false</code>.
		/// </summary>
		/// <param name="filter"> EventTargetFilter instance to determine whether the
		///        given component is a valid target for this event. </param>
		/// <param name="searchHeavyweights"> if <code>false</code>, the method
		///        will bypass heavyweight components during the search. </param>
		private Component GetMouseEventTarget(int x, int y, bool includeSelf, EventTargetFilter filter, bool searchHeavyweights)
		{
			Component comp = null;
			if (searchHeavyweights)
			{
				comp = GetMouseEventTargetImpl(x, y, includeSelf, filter, SEARCH_HEAVYWEIGHTS, searchHeavyweights);
			}

			if (comp == null || comp == this)
			{
				comp = GetMouseEventTargetImpl(x, y, includeSelf, filter, !SEARCH_HEAVYWEIGHTS, searchHeavyweights);
			}

			return comp;
		}

		/// <summary>
		/// A private version of getMouseEventTarget which has three additional
		/// controllable behaviors. This method searches for the top-most
		/// descendant of this container that contains the given coordinates
		/// and is accepted by the given filter. The search will be constrained to
		/// descendants of only lightweight children or only heavyweight children
		/// of this container depending on searchHeavyweightChildren. The search will
		/// be constrained to only lightweight descendants of the searched children
		/// of this container if searchHeavyweightDescendants is <code>false</code>.
		/// </summary>
		/// <param name="filter"> EventTargetFilter instance to determine whether the
		///        selected component is a valid target for this event. </param>
		/// <param name="searchHeavyweightChildren"> if <code>true</code>, the method
		///        will bypass immediate lightweight children during the search.
		///        If <code>false</code>, the methods will bypass immediate
		///        heavyweight children during the search. </param>
		/// <param name="searchHeavyweightDescendants"> if <code>false</code>, the method
		///        will bypass heavyweight descendants which are not immediate
		///        children during the search. If <code>true</code>, the method
		///        will traverse both lightweight and heavyweight descendants during
		///        the search. </param>
		private Component GetMouseEventTargetImpl(int x, int y, bool includeSelf, EventTargetFilter filter, bool searchHeavyweightChildren, bool searchHeavyweightDescendants)
		{
			lock (TreeLock)
			{

				for (int i = 0; i < Component.Count; i++)
				{
					Component comp = Component[i];
					if (comp != null && comp.Visible_Renamed && ((!searchHeavyweightChildren && comp.Peer_Renamed is LightweightPeer) || (searchHeavyweightChildren && !(comp.Peer_Renamed is LightweightPeer))) && comp.Contains(x - comp.x, y - comp.y))
					{

						// found a component that intersects the point, see if there
						// is a deeper possibility.
						if (comp is Container)
						{
							Container child = (Container) comp;
							Component deeper = child.GetMouseEventTarget(x - child.x, y - child.y, includeSelf, filter, searchHeavyweightDescendants);
							if (deeper != null)
							{
								return deeper;
							}
						}
						else
						{
							if (filter.Accept(comp))
							{
								// there isn't a deeper target, but this component
								// is a target
								return comp;
							}
						}
					}
				}

				bool isPeerOK;
				bool isMouseOverMe;

				isPeerOK = (Peer_Renamed is LightweightPeer) || includeSelf;
				isMouseOverMe = Contains(x,y);

				// didn't find a child target, return this component if it's
				// a possible target
				if (isMouseOverMe && isPeerOK && filter.Accept(this))
				{
					return this;
				}
				// no possible target
				return null;
			}
		}

		internal interface EventTargetFilter
		{
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: boolean accept(final Component comp);
			bool Accept(Component comp);
		}

		internal class MouseEventTargetFilter : EventTargetFilter
		{
			internal static readonly EventTargetFilter FILTER = new MouseEventTargetFilter();

			internal MouseEventTargetFilter()
			{
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean accept(final Component comp)
			public virtual bool Accept(Component comp)
			{
				return (comp.EventMask & AWTEvent.MOUSE_MOTION_EVENT_MASK) != 0 || (comp.EventMask & AWTEvent.MOUSE_EVENT_MASK) != 0 || (comp.EventMask & AWTEvent.MOUSE_WHEEL_EVENT_MASK) != 0 || comp.MouseListener != null || comp.MouseMotionListener != null || comp.MouseWheelListener != null;
			}
		}

		internal class DropTargetEventTargetFilter : EventTargetFilter
		{
			internal static readonly EventTargetFilter FILTER = new DropTargetEventTargetFilter();

			internal DropTargetEventTargetFilter()
			{
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean accept(final Component comp)
			public virtual bool Accept(Component comp)
			{
				DropTarget dt = comp.DropTarget;
				return dt != null && dt.Active;
			}
		}

		/// <summary>
		/// This is called by lightweight components that want the containing
		/// windowed parent to enable some kind of events on their behalf.
		/// This is needed for events that are normally only dispatched to
		/// windows to be accepted so that they can be forwarded downward to
		/// the lightweight component that has enabled them.
		/// </summary>
		internal virtual void ProxyEnableEvents(long events)
		{
			if (Peer_Renamed is LightweightPeer)
			{
				// this container is lightweight.... continue sending it
				// upward.
				if (Parent_Renamed != null)
				{
					Parent_Renamed.ProxyEnableEvents(events);
				}
			}
			else
			{
				// This is a native container, so it needs to host
				// one of it's children.  If this function is called before
				// a peer has been created we don't yet have a dispatcher
				// because it has not yet been determined if this instance
				// is lightweight.
				if (Dispatcher != null)
				{
					Dispatcher.EnableEvents(events);
				}
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>dispatchEvent(AWTEvent e)</code> 
		[Obsolete("As of JDK version 1.1,")]
		public override void DeliverEvent(Event e)
		{
			Component comp = GetComponentAt(e.x, e.y);
			if ((comp != null) && (comp != this))
			{
				e.Translate(-comp.x, -comp.y);
				comp.DeliverEvent(e);
			}
			else
			{
				PostEvent(e);
			}
		}

		/// <summary>
		/// Locates the component that contains the x,y position.  The
		/// top-most child component is returned in the case where there
		/// is overlap in the components.  This is determined by finding
		/// the component closest to the index 0 that claims to contain
		/// the given point via Component.contains(), except that Components
		/// which have native peers take precedence over those which do not
		/// (i.e., lightweight Components).
		/// </summary>
		/// <param name="x"> the <i>x</i> coordinate </param>
		/// <param name="y"> the <i>y</i> coordinate </param>
		/// <returns> null if the component does not contain the position.
		/// If there is no child component at the requested point and the
		/// point is within the bounds of the container the container itself
		/// is returned; otherwise the top-most child is returned. </returns>
		/// <seealso cref= Component#contains
		/// @since JDK1.1 </seealso>
		public override Component GetComponentAt(int x, int y)
		{
			return Locate(x, y);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getComponentAt(int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public override Component Locate(int x, int y)
		{
			if (!Contains(x, y))
			{
				return null;
			}
			Component lightweight = null;
			lock (TreeLock)
			{
				// Optimized version of two passes:
				// see comment in sun.awt.SunGraphicsCallback
				foreach (Component comp in Component)
				{
					if (comp.contains(x - comp.x, y - comp.y))
					{
						if (!comp.Lightweight)
						{
							// return heavyweight component as soon as possible
							return comp;
						}
						if (lightweight == null)
						{
							// save and return later the first lightweight component
							lightweight = comp;
						}
					}
				}
			}
			return lightweight != null ? lightweight : this;
		}

		/// <summary>
		/// Gets the component that contains the specified point. </summary>
		/// <param name="p">   the point. </param>
		/// <returns>     returns the component that contains the point,
		///                 or <code>null</code> if the component does
		///                 not contain the point. </returns>
		/// <seealso cref=        Component#contains
		/// @since      JDK1.1 </seealso>
		public override Component GetComponentAt(Point p)
		{
			return GetComponentAt(p.x, p.y);
		}

		/// <summary>
		/// Returns the position of the mouse pointer in this <code>Container</code>'s
		/// coordinate space if the <code>Container</code> is under the mouse pointer,
		/// otherwise returns <code>null</code>.
		/// This method is similar to <seealso cref="Component#getMousePosition()"/> with the exception
		/// that it can take the <code>Container</code>'s children into account.
		/// If <code>allowChildren</code> is <code>false</code>, this method will return
		/// a non-null value only if the mouse pointer is above the <code>Container</code>
		/// directly, not above the part obscured by children.
		/// If <code>allowChildren</code> is <code>true</code>, this method returns
		/// a non-null value if the mouse pointer is above <code>Container</code> or any
		/// of its descendants.
		/// </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
		/// <param name="allowChildren"> true if children should be taken into account </param>
		/// <seealso cref=       Component#getMousePosition </seealso>
		/// <returns>    mouse coordinates relative to this <code>Component</code>, or null
		/// @since     1.5 </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Point getMousePosition(boolean allowChildren) throws HeadlessException
		public virtual Point GetMousePosition(bool allowChildren)
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
				if (IsSameOrAncestorOf(inTheSameWindow, allowChildren))
				{
					return PointRelativeToComponent(pi.Location);
				}
				return null;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<PointerInfo>
		{
			private readonly Container OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(Container outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual PointerInfo Run()
			{
				return MouseInfo.PointerInfo;
			}
		}

		internal override bool IsSameOrAncestorOf(Component comp, bool allowChildren)
		{
			return this == comp || (allowChildren && IsParentOf(comp));
		}

		/// <summary>
		/// Locates the visible child component that contains the specified
		/// position.  The top-most child component is returned in the case
		/// where there is overlap in the components.  If the containing child
		/// component is a Container, this method will continue searching for
		/// the deepest nested child component.  Components which are not
		/// visible are ignored during the search.<para>
		/// 
		/// The findComponentAt method is different from getComponentAt in
		/// that getComponentAt only searches the Container's immediate
		/// children; if the containing component is a Container,
		/// findComponentAt will search that child to find a nested component.
		/// 
		/// </para>
		/// </summary>
		/// <param name="x"> the <i>x</i> coordinate </param>
		/// <param name="y"> the <i>y</i> coordinate </param>
		/// <returns> null if the component does not contain the position.
		/// If there is no child component at the requested point and the
		/// point is within the bounds of the container the container itself
		/// is returned. </returns>
		/// <seealso cref= Component#contains </seealso>
		/// <seealso cref= #getComponentAt
		/// @since 1.2 </seealso>
		public virtual Component FindComponentAt(int x, int y)
		{
			return FindComponentAt(x, y, true);
		}

		/// <summary>
		/// Private version of findComponentAt which has a controllable
		/// behavior. Setting 'ignoreEnabled' to 'false' bypasses disabled
		/// Components during the search. This behavior is used by the
		/// lightweight cursor support in sun.awt.GlobalCursorManager.
		/// 
		/// The addition of this feature is temporary, pending the
		/// adoption of new, public API which exports this feature.
		/// </summary>
		internal Component FindComponentAt(int x, int y, bool ignoreEnabled)
		{
			lock (TreeLock)
			{
				if (RecursivelyVisible)
				{
					return FindComponentAtImpl(x, y, ignoreEnabled);
				}
			}
			return null;
		}

		internal Component FindComponentAtImpl(int x, int y, bool ignoreEnabled)
		{
			// checkTreeLock(); commented for a performance reason

			if (!(Contains(x, y) && Visible_Renamed && (ignoreEnabled || Enabled_Renamed)))
			{
				return null;
			}
			Component lightweight = null;
			// Optimized version of two passes:
			// see comment in sun.awt.SunGraphicsCallback
			foreach (Component comp in Component)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int x1 = x - comp.x;
				int x1 = x - comp.x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int y1 = y - comp.y;
				int y1 = y - comp.y;
				if (!comp.contains(x1, y1))
				{
					continue; // fast path
				}
				if (!comp.Lightweight)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Component child = getChildAt(comp, x1, y1, ignoreEnabled);
					Component child = GetChildAt(comp, x1, y1, ignoreEnabled);
					if (child != null)
					{
						// return heavyweight component as soon as possible
						return child;
					}
				}
				else
				{
					if (lightweight == null)
					{
						// save and return later the first lightweight component
						lightweight = GetChildAt(comp, x1, y1, ignoreEnabled);
					}
				}
			}
			return lightweight != null ? lightweight : this;
		}

		/// <summary>
		/// Helper method for findComponentAtImpl. Finds a child component using
		/// findComponentAtImpl for Container and getComponentAt for Component.
		/// </summary>
		private static Component GetChildAt(Component comp, int x, int y, bool ignoreEnabled)
		{
			if (comp is Container)
			{
				comp = ((Container) comp).FindComponentAtImpl(x, y, ignoreEnabled);
			}
			else
			{
				comp = comp.GetComponentAt(x, y);
			}
			if (comp != null && comp.Visible_Renamed && (ignoreEnabled || comp.Enabled_Renamed))
			{
				return comp;
			}
			return null;
		}

		/// <summary>
		/// Locates the visible child component that contains the specified
		/// point.  The top-most child component is returned in the case
		/// where there is overlap in the components.  If the containing child
		/// component is a Container, this method will continue searching for
		/// the deepest nested child component.  Components which are not
		/// visible are ignored during the search.<para>
		/// 
		/// The findComponentAt method is different from getComponentAt in
		/// that getComponentAt only searches the Container's immediate
		/// children; if the containing component is a Container,
		/// findComponentAt will search that child to find a nested component.
		/// 
		/// </para>
		/// </summary>
		/// <param name="p">   the point. </param>
		/// <returns> null if the component does not contain the position.
		/// If there is no child component at the requested point and the
		/// point is within the bounds of the container the container itself
		/// is returned. </returns>
		/// <exception cref="NullPointerException"> if {@code p} is {@code null} </exception>
		/// <seealso cref= Component#contains </seealso>
		/// <seealso cref= #getComponentAt
		/// @since 1.2 </seealso>
		public virtual Component FindComponentAt(Point p)
		{
			return FindComponentAt(p.x, p.y);
		}

		/// <summary>
		/// Makes this Container displayable by connecting it to
		/// a native screen resource.  Making a container displayable will
		/// cause all of its children to be made displayable.
		/// This method is called internally by the toolkit and should
		/// not be called directly by programs. </summary>
		/// <seealso cref= Component#isDisplayable </seealso>
		/// <seealso cref= #removeNotify </seealso>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				// addNotify() on the children may cause proxy event enabling
				// on this instance, so we first call super.addNotify() and
				// possibly create an lightweight event dispatcher before calling
				// addNotify() on the children which may be lightweight.
				base.AddNotify();
				if (!(Peer_Renamed is LightweightPeer))
				{
					Dispatcher = new LightweightDispatcher(this);
				}

				// We shouldn't use iterator because of the Swing menu
				// implementation specifics:
				// the menu is being assigned as a child to JLayeredPane
				// instead of particular component so always affect
				// collection of component if menu is becoming shown or hidden.
				for (int i = 0; i < Component.Count; i++)
				{
					Component[i].AddNotify();
				}
			}
		}

		/// <summary>
		/// Makes this Container undisplayable by removing its connection
		/// to its native screen resource.  Making a container undisplayable
		/// will cause all of its children to be made undisplayable.
		/// This method is called by the toolkit internally and should
		/// not be called directly by programs. </summary>
		/// <seealso cref= Component#isDisplayable </seealso>
		/// <seealso cref= #addNotify </seealso>
		public override void RemoveNotify()
		{
			lock (TreeLock)
			{
				// We shouldn't use iterator because of the Swing menu
				// implementation specifics:
				// the menu is being assigned as a child to JLayeredPane
				// instead of particular component so always affect
				// collection of component if menu is becoming shown or hidden.
				for (int i = Component.Count - 1 ; i >= 0 ; i--)
				{
					Component comp = Component[i];
					if (comp != null)
					{
						// Fix for 6607170.
						// We want to suppress focus change on disposal
						// of the focused component. But because of focus
						// is asynchronous, we should suppress focus change
						// on every component in case it receives native focus
						// in the process of disposal.
						comp.AutoFocusTransferOnDisposal = false;
						comp.RemoveNotify();
						comp.AutoFocusTransferOnDisposal = true;
					}
				}
				// If some of the children had focus before disposal then it still has.
				// Auto-transfer focus to the next (or previous) component if auto-transfer
				// is enabled.
				if (ContainsFocus() && KeyboardFocusManager.IsAutoFocusTransferEnabledFor(this))
				{
					if (!TransferFocus(false))
					{
						TransferFocusBackward(true);
					}
				}
				if (Dispatcher != null)
				{
					Dispatcher.Dispose();
					Dispatcher = null;
				}
				base.RemoveNotify();
			}
		}

		/// <summary>
		/// Checks if the component is contained in the component hierarchy of
		/// this container. </summary>
		/// <param name="c"> the component </param>
		/// <returns>     <code>true</code> if it is an ancestor;
		///             <code>false</code> otherwise.
		/// @since      JDK1.1 </returns>
		public virtual bool IsAncestorOf(Component c)
		{
			Container p;
			if (c == null || ((p = c.Parent) == null))
			{
				return false;
			}
			while (p != null)
			{
				if (p == this)
				{
					return true;
				}
				p = p.Parent;
			}
			return false;
		}

		/*
		 * The following code was added to support modal JInternalFrames
		 * Unfortunately this code has to be added here so that we can get access to
		 * some private AWT classes like SequencedEvent.
		 *
		 * The native container of the LW component has this field set
		 * to tell it that it should block Mouse events for all LW
		 * children except for the modal component.
		 *
		 * In the case of nested Modal components, we store the previous
		 * modal component in the new modal components value of modalComp;
		 */

		[NonSerialized]
		internal Component ModalComp;
		[NonSerialized]
		internal AppContext ModalAppContext;

		private void StartLWModal()
		{
			// Store the app context on which this component is being shown.
			// Event dispatch thread of this app context will be sleeping until
			// we wake it by any event from hideAndDisposeHandler().
			ModalAppContext = AppContext.AppContext;

			// keep the KeyEvents from being dispatched
			// until the focus has been transfered
			long time = Toolkit.EventQueue.MostRecentKeyEventTime;
			Component predictedFocusOwner = (Component.IsInstanceOf(this, "javax.swing.JInternalFrame")) ? ((javax.swing.JInternalFrame)(this)).MostRecentFocusOwner : null;
			if (predictedFocusOwner != null)
			{
				KeyboardFocusManager.CurrentKeyboardFocusManager.EnqueueKeyEvents(time, predictedFocusOwner);
			}
			// We have two mechanisms for blocking: 1. If we're on the
			// EventDispatchThread, start a new event pump. 2. If we're
			// on any other thread, call wait() on the treelock.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Container nativeContainer;
			Container nativeContainer;
			lock (TreeLock)
			{
				nativeContainer = HeavyweightContainer;
				if (nativeContainer.ModalComp != null)
				{
					this.ModalComp = nativeContainer.ModalComp;
					nativeContainer.ModalComp = this;
					return;
				}
				else
				{
					nativeContainer.ModalComp = this;
				}
			}

			Runnable pumpEventsForHierarchy = new RunnableAnonymousInnerClassHelper(this, nativeContainer);

			if (EventQueue.DispatchThread)
			{
				SequencedEvent currentSequencedEvent = KeyboardFocusManager.CurrentKeyboardFocusManager.CurrentSequencedEvent;
				if (currentSequencedEvent != null)
				{
					currentSequencedEvent.Dispose();
				}

				pumpEventsForHierarchy.Run();
			}
			else
			{
				lock (TreeLock)
				{
					Toolkit.EventQueue.PostEvent(new PeerEvent(this, pumpEventsForHierarchy, PeerEvent.PRIORITY_EVENT));
					while ((WindowClosingException == null) && (nativeContainer.ModalComp != null))
					{
						try
						{
							TreeLock.Wait();
						}
						catch (InterruptedException)
						{
							break;
						}
					}
				}
			}
			if (WindowClosingException != null)
			{
				WindowClosingException.FillInStackTrace();
				throw WindowClosingException;
			}
			if (predictedFocusOwner != null)
			{
				KeyboardFocusManager.CurrentKeyboardFocusManager.DequeueKeyEvents(time, predictedFocusOwner);
			}
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			private readonly Container OuterInstance;

			private java.awt.Container NativeContainer;

			public RunnableAnonymousInnerClassHelper(Container outerInstance, java.awt.Container nativeContainer)
			{
				this.OuterInstance = outerInstance;
				this.NativeContainer = nativeContainer;
			}

			public virtual void Run()
			{
				EventDispatchThread dispatchThread = (EventDispatchThread)Thread.CurrentThread;
				dispatchThread.PumpEventsForHierarchy(new ConditionalAnonymousInnerClassHelper(this), OuterInstance);
			}

			private class ConditionalAnonymousInnerClassHelper : Conditional
			{
				private readonly RunnableAnonymousInnerClassHelper OuterInstance;

				public ConditionalAnonymousInnerClassHelper(RunnableAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public virtual bool Evaluate()
				{
				return ((OuterInstance.OuterInstance.WindowClosingException == null) && (OuterInstance.NativeContainer.modalComp != null));
				}
			}
		}

		private void StopLWModal()
		{
			lock (TreeLock)
			{
				if (ModalAppContext != null)
				{
					Container nativeContainer = HeavyweightContainer;
					if (nativeContainer != null)
					{
						if (this.ModalComp != null)
						{
							nativeContainer.ModalComp = this.ModalComp;
							this.ModalComp = null;
							return;
						}
						else
						{
							nativeContainer.ModalComp = null;
						}
					}
					// Wake up event dispatch thread on which the dialog was
					// initially shown
					SunToolkit.postEvent(ModalAppContext, new PeerEvent(this, new WakingRunnable(), PeerEvent.PRIORITY_EVENT));
				}
				EventQueue.InvokeLater(new WakingRunnable());
				Monitor.PulseAll(TreeLock);
			}
		}

		internal sealed class WakingRunnable : Runnable
		{
			public void Run()
			{
			}
		}

		/* End of JOptionPane support code */

		/// <summary>
		/// Returns a string representing the state of this <code>Container</code>.
		/// This method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>    the parameter string of this container </returns>
		protected internal override String ParamString()
		{
			String str = base.ParamString();
			LayoutManager layoutMgr = this.LayoutMgr;
			if (layoutMgr != null)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				str += ",layout=" + layoutMgr.GetType().FullName;
			}
			return str;
		}

		/// <summary>
		/// Prints a listing of this container to the specified output
		/// stream. The listing starts at the specified indentation.
		/// <para>
		/// The immediate children of the container are printed with
		/// an indentation of <code>indent+1</code>.  The children
		/// of those children are printed at <code>indent+2</code>
		/// and so on.
		/// 
		/// </para>
		/// </summary>
		/// <param name="out">      a print stream </param>
		/// <param name="indent">   the number of spaces to indent </param>
		/// <exception cref="NullPointerException"> if {@code out} is {@code null} </exception>
		/// <seealso cref=      Component#list(java.io.PrintStream, int)
		/// @since    JDK1.0 </seealso>
		public override void List(PrintStream @out, int indent)
		{
			base.List(@out, indent);
			lock (TreeLock)
			{
				for (int i = 0; i < Component.Count; i++)
				{
					Component comp = Component[i];
					if (comp != null)
					{
						comp.List(@out, indent + 1);
					}
				}
			}
		}

		/// <summary>
		/// Prints out a list, starting at the specified indentation,
		/// to the specified print writer.
		/// <para>
		/// The immediate children of the container are printed with
		/// an indentation of <code>indent+1</code>.  The children
		/// of those children are printed at <code>indent+2</code>
		/// and so on.
		/// 
		/// </para>
		/// </summary>
		/// <param name="out">      a print writer </param>
		/// <param name="indent">   the number of spaces to indent </param>
		/// <exception cref="NullPointerException"> if {@code out} is {@code null} </exception>
		/// <seealso cref=      Component#list(java.io.PrintWriter, int)
		/// @since    JDK1.1 </seealso>
		public override void List(PrintWriter @out, int indent)
		{
			base.List(@out, indent);
			lock (TreeLock)
			{
				for (int i = 0; i < Component.Count; i++)
				{
					Component comp = Component[i];
					if (comp != null)
					{
						comp.List(@out, indent + 1);
					}
				}
			}
		}

		/// <summary>
		/// Sets the focus traversal keys for a given traversal operation for this
		/// Container.
		/// <para>
		/// The default values for a Container's focus traversal keys are
		/// implementation-dependent. Sun recommends that all implementations for a
		/// particular native platform use the same default values. The
		/// recommendations for Windows and Unix are listed below. These
		/// recommendations are used in the Sun AWT implementations.
		/// 
		/// <table border=1 summary="Recommended default values for a Container's focus traversal keys">
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
		/// <tr>
		///    <td>KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS<td>
		///    <td>Go down one focus traversal cycle</td>
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
		/// to any Container. It is a runtime error to specify a KEY_TYPED event as
		/// mapping to a focus traversal operation, or to map the same event to
		/// multiple default focus traversal operations.
		/// </para>
		/// <para>
		/// If a value of null is specified for the Set, this Container inherits the
		/// Set from its parent. If all ancestors of this Container have null
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
		///        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		///        KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS </param>
		/// <param name="keystrokes"> the Set of AWTKeyStroke for the specified operation </param>
		/// <seealso cref= #getFocusTraversalKeys </seealso>
		/// <seealso cref= KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
		/// <seealso cref= KeyboardFocusManager#DOWN_CYCLE_TRAVERSAL_KEYS </seealso>
		/// <exception cref="IllegalArgumentException"> if id is not one of
		///         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		///         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		///         KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS, or if keystrokes
		///         contains null, or if any keystroke represents a KEY_TYPED event,
		///         or if any keystroke already maps to another focus traversal
		///         operation for this Container
		/// @since 1.4
		/// @beaninfo
		///       bound: true </exception>
		public override void setFocusTraversalKeys<T1>(int id, Set<T1> keystrokes) where T1 : AWTKeyStroke
		{
			if (id < 0 || id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH)
			{
				throw new IllegalArgumentException("invalid focus traversal key identifier");
			}

			// Don't call super.setFocusTraversalKey. The Component parameter check
			// does not allow DOWN_CYCLE_TRAVERSAL_KEYS, but we do.
			SetFocusTraversalKeys_NoIDCheck(id, keystrokes);
		}

		/// <summary>
		/// Returns the Set of focus traversal keys for a given traversal operation
		/// for this Container. (See
		/// <code>setFocusTraversalKeys</code> for a full description of each key.)
		/// <para>
		/// If a Set of traversal keys has not been explicitly defined for this
		/// Container, then this Container's parent's Set is returned. If no Set
		/// has been explicitly defined for any of this Container's ancestors, then
		/// the current KeyboardFocusManager's default Set is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		///        KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS </param>
		/// <returns> the Set of AWTKeyStrokes for the specified operation. The Set
		///         will be unmodifiable, and may be empty. null will never be
		///         returned. </returns>
		/// <seealso cref= #setFocusTraversalKeys </seealso>
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
		public override Set<AWTKeyStroke> GetFocusTraversalKeys(int id)
		{
			if (id < 0 || id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH)
			{
				throw new IllegalArgumentException("invalid focus traversal key identifier");
			}

			// Don't call super.getFocusTraversalKey. The Component parameter check
			// does not allow DOWN_CYCLE_TRAVERSAL_KEY, but we do.
			return GetFocusTraversalKeys_NoIDCheck(id);
		}

		/// <summary>
		/// Returns whether the Set of focus traversal keys for the given focus
		/// traversal operation has been explicitly defined for this Container. If
		/// this method returns <code>false</code>, this Container is inheriting the
		/// Set from an ancestor, or from the current KeyboardFocusManager.
		/// </summary>
		/// <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		///        KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS </param>
		/// <returns> <code>true</code> if the the Set of focus traversal keys for the
		///         given focus traversal operation has been explicitly defined for
		///         this Component; <code>false</code> otherwise. </returns>
		/// <exception cref="IllegalArgumentException"> if id is not one of
		///         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		///        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		///        KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS
		/// @since 1.4 </exception>
		public override bool AreFocusTraversalKeysSet(int id)
		{
			if (id < 0 || id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH)
			{
				throw new IllegalArgumentException("invalid focus traversal key identifier");
			}

			return (FocusTraversalKeys != null && FocusTraversalKeys[id] != null);
		}

		/// <summary>
		/// Returns whether the specified Container is the focus cycle root of this
		/// Container's focus traversal cycle. Each focus traversal cycle has only
		/// a single focus cycle root and each Container which is not a focus cycle
		/// root belongs to only a single focus traversal cycle. Containers which
		/// are focus cycle roots belong to two cycles: one rooted at the Container
		/// itself, and one rooted at the Container's nearest focus-cycle-root
		/// ancestor. This method will return <code>true</code> for both such
		/// Containers in this case.
		/// </summary>
		/// <param name="container"> the Container to be tested </param>
		/// <returns> <code>true</code> if the specified Container is a focus-cycle-
		///         root of this Container; <code>false</code> otherwise </returns>
		/// <seealso cref= #isFocusCycleRoot()
		/// @since 1.4 </seealso>
		public override bool IsFocusCycleRoot(Container container)
		{
			if (FocusCycleRoot && container == this)
			{
				return true;
			}
			else
			{
				return base.IsFocusCycleRoot(container);
			}
		}

		private Container FindTraversalRoot()
		{
			// I potentially have two roots, myself and my root parent
			// If I am the current root, then use me
			// If none of my parents are roots, then use me
			// If my root parent is the current root, then use my root parent
			// If neither I nor my root parent is the current root, then
			// use my root parent (a guess)

			Container currentFocusCycleRoot = KeyboardFocusManager.CurrentKeyboardFocusManager.CurrentFocusCycleRoot;
			Container root;

			if (currentFocusCycleRoot == this)
			{
				root = this;
			}
			else
			{
				root = FocusCycleRootAncestor;
				if (root == null)
				{
					root = this;
				}
			}

			if (root != currentFocusCycleRoot)
			{
				KeyboardFocusManager.CurrentKeyboardFocusManager.GlobalCurrentFocusCycleRootPriv = root;
			}
			return root;
		}

		internal sealed override bool ContainsFocus()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Component focusOwner = KeyboardFocusManager.getCurrentKeyboardFocusManager().getFocusOwner();
			Component focusOwner = KeyboardFocusManager.CurrentKeyboardFocusManager.FocusOwner;
			return IsParentOf(focusOwner);
		}

		/// <summary>
		/// Check if this component is the child of this container or its children.
		/// Note: this function acquires treeLock
		/// Note: this function traverses children tree only in one Window. </summary>
		/// <param name="comp"> a component in test, must not be null </param>
		private bool IsParentOf(Component comp)
		{
			lock (TreeLock)
			{
				while (comp != null && comp != this && !(comp is Window))
				{
					comp = comp.Parent;
				}
				return (comp == this);
			}
		}

		internal override void ClearMostRecentFocusOwnerOnHide()
		{
			bool reset = false;
			Window window = null;

			lock (TreeLock)
			{
				window = ContainingWindow;
				if (window != null)
				{
					Component comp = KeyboardFocusManager.GetMostRecentFocusOwner(window);
					reset = ((comp == this) || IsParentOf(comp));
					// This synchronized should always be the second in a pair
					// (tree lock, KeyboardFocusManager.class)
					lock (typeof(KeyboardFocusManager))
					{
						Component storedComp = window.TemporaryLostComponent;
						if (IsParentOf(storedComp) || storedComp == this)
						{
							window.TemporaryLostComponent = null;
						}
					}
				}
			}

			if (reset)
			{
				KeyboardFocusManager.SetMostRecentFocusOwner(window, null);
			}
		}

		internal override void ClearCurrentFocusCycleRootOnHide()
		{
			KeyboardFocusManager kfm = KeyboardFocusManager.CurrentKeyboardFocusManager;
			Container cont = kfm.CurrentFocusCycleRoot;

			if (cont == this || IsParentOf(cont))
			{
				kfm.GlobalCurrentFocusCycleRootPriv = null;
			}
		}

		internal sealed override Container TraversalRoot
		{
			get
			{
				if (FocusCycleRoot)
				{
					return FindTraversalRoot();
				}
    
				return base.TraversalRoot;
			}
		}

		/// <summary>
		/// Sets the focus traversal policy that will manage keyboard traversal of
		/// this Container's children, if this Container is a focus cycle root. If
		/// the argument is null, this Container inherits its policy from its focus-
		/// cycle-root ancestor. If the argument is non-null, this policy will be
		/// inherited by all focus-cycle-root children that have no keyboard-
		/// traversal policy of their own (as will, recursively, their focus-cycle-
		/// root children).
		/// <para>
		/// If this Container is not a focus cycle root, the policy will be
		/// remembered, but will not be used or inherited by this or any other
		/// Containers until this Container is made a focus cycle root.
		/// 
		/// </para>
		/// </summary>
		/// <param name="policy"> the new focus traversal policy for this Container </param>
		/// <seealso cref= #getFocusTraversalPolicy </seealso>
		/// <seealso cref= #setFocusCycleRoot </seealso>
		/// <seealso cref= #isFocusCycleRoot
		/// @since 1.4
		/// @beaninfo
		///       bound: true </seealso>
		public virtual FocusTraversalPolicy FocusTraversalPolicy
		{
			set
			{
				FocusTraversalPolicy oldPolicy;
				lock (this)
				{
					oldPolicy = this.FocusTraversalPolicy_Renamed;
					this.FocusTraversalPolicy_Renamed = value;
				}
				FirePropertyChange("focusTraversalPolicy", oldPolicy, value);
			}
			get
			{
				if (!FocusTraversalPolicyProvider && !FocusCycleRoot)
				{
					return null;
				}
    
				FocusTraversalPolicy policy = this.FocusTraversalPolicy_Renamed;
				if (policy != null)
				{
					return policy;
				}
    
				Container rootAncestor = FocusCycleRootAncestor;
				if (rootAncestor != null)
				{
					return rootAncestor.FocusTraversalPolicy;
				}
				else
				{
					return KeyboardFocusManager.CurrentKeyboardFocusManager.DefaultFocusTraversalPolicy;
				}
			}
		}


		/// <summary>
		/// Returns whether the focus traversal policy has been explicitly set for
		/// this Container. If this method returns <code>false</code>, this
		/// Container will inherit its focus traversal policy from an ancestor.
		/// </summary>
		/// <returns> <code>true</code> if the focus traversal policy has been
		///         explicitly set for this Container; <code>false</code> otherwise.
		/// @since 1.4 </returns>
		public virtual bool FocusTraversalPolicySet
		{
			get
			{
				return (FocusTraversalPolicy_Renamed != null);
			}
		}

		/// <summary>
		/// Sets whether this Container is the root of a focus traversal cycle. Once
		/// focus enters a traversal cycle, typically it cannot leave it via focus
		/// traversal unless one of the up- or down-cycle keys is pressed. Normal
		/// traversal is limited to this Container, and all of this Container's
		/// descendants that are not descendants of inferior focus cycle roots. Note
		/// that a FocusTraversalPolicy may bend these restrictions, however. For
		/// example, ContainerOrderFocusTraversalPolicy supports implicit down-cycle
		/// traversal.
		/// <para>
		/// The alternative way to specify the traversal order of this Container's
		/// children is to make this Container a
		/// <a href="doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus traversal policy provider</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="focusCycleRoot"> indicates whether this Container is the root of a
		///        focus traversal cycle </param>
		/// <seealso cref= #isFocusCycleRoot() </seealso>
		/// <seealso cref= #setFocusTraversalPolicy </seealso>
		/// <seealso cref= #getFocusTraversalPolicy </seealso>
		/// <seealso cref= ContainerOrderFocusTraversalPolicy </seealso>
		/// <seealso cref= #setFocusTraversalPolicyProvider
		/// @since 1.4
		/// @beaninfo
		///       bound: true </seealso>
		public virtual bool FocusCycleRoot
		{
			set
			{
				bool oldFocusCycleRoot;
				lock (this)
				{
					oldFocusCycleRoot = this.FocusCycleRoot_Renamed;
					this.FocusCycleRoot_Renamed = value;
				}
				FirePropertyChange("focusCycleRoot", oldFocusCycleRoot, value);
			}
			get
			{
				return FocusCycleRoot_Renamed;
			}
		}


		/// <summary>
		/// Sets whether this container will be used to provide focus
		/// traversal policy. Container with this property as
		/// <code>true</code> will be used to acquire focus traversal policy
		/// instead of closest focus cycle root ancestor. </summary>
		/// <param name="provider"> indicates whether this container will be used to
		///                provide focus traversal policy </param>
		/// <seealso cref= #setFocusTraversalPolicy </seealso>
		/// <seealso cref= #getFocusTraversalPolicy </seealso>
		/// <seealso cref= #isFocusTraversalPolicyProvider
		/// @since 1.5
		/// @beaninfo
		///        bound: true </seealso>
		public bool FocusTraversalPolicyProvider
		{
			set
			{
				bool oldProvider;
				lock (this)
				{
					oldProvider = FocusTraversalPolicyProvider_Renamed;
					FocusTraversalPolicyProvider_Renamed = value;
				}
				FirePropertyChange("focusTraversalPolicyProvider", oldProvider, value);
			}
			get
			{
				return FocusTraversalPolicyProvider_Renamed;
			}
		}


		/// <summary>
		/// Transfers the focus down one focus traversal cycle. If this Container is
		/// a focus cycle root, then the focus owner is set to this Container's
		/// default Component to focus, and the current focus cycle root is set to
		/// this Container. If this Container is not a focus cycle root, then no
		/// focus traversal operation occurs.
		/// </summary>
		/// <seealso cref=       Component#requestFocus() </seealso>
		/// <seealso cref=       #isFocusCycleRoot </seealso>
		/// <seealso cref=       #setFocusCycleRoot
		/// @since     1.4 </seealso>
		public virtual void TransferFocusDownCycle()
		{
			if (FocusCycleRoot)
			{
				KeyboardFocusManager.CurrentKeyboardFocusManager.GlobalCurrentFocusCycleRootPriv = this;
				Component toFocus = FocusTraversalPolicy.GetDefaultComponent(this);
				if (toFocus != null)
				{
					toFocus.RequestFocus(CausedFocusEvent.Cause.TRAVERSAL_DOWN);
				}
			}
		}

		internal virtual void PreProcessKeyEvent(KeyEvent e)
		{
			Container parent = this.Parent_Renamed;
			if (parent != null)
			{
				parent.PreProcessKeyEvent(e);
			}
		}

		internal virtual void PostProcessKeyEvent(KeyEvent e)
		{
			Container parent = this.Parent_Renamed;
			if (parent != null)
			{
				parent.PostProcessKeyEvent(e);
			}
		}

		internal override bool PostsOldMouseEvents()
		{
			return true;
		}

		/// <summary>
		/// Sets the <code>ComponentOrientation</code> property of this container
		/// and all components contained within it.
		/// <para>
		/// This method changes layout-related information, and therefore,
		/// invalidates the component hierarchy.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> the new component orientation of this container and
		///        the components contained within it. </param>
		/// <exception cref="NullPointerException"> if <code>orientation</code> is null. </exception>
		/// <seealso cref= Component#setComponentOrientation </seealso>
		/// <seealso cref= Component#getComponentOrientation </seealso>
		/// <seealso cref= #invalidate
		/// @since 1.4 </seealso>
		public override void ApplyComponentOrientation(ComponentOrientation o)
		{
			base.ApplyComponentOrientation(o);
			lock (TreeLock)
			{
				for (int i = 0; i < Component.Count; i++)
				{
					Component comp = Component[i];
					comp.ApplyComponentOrientation(o);
				}
			}
		}

		/// <summary>
		/// Adds a PropertyChangeListener to the listener list. The listener is
		/// registered for all bound properties of this class, including the
		/// following:
		/// <ul>
		///    <li>this Container's font ("font")</li>
		///    <li>this Container's background color ("background")</li>
		///    <li>this Container's foreground color ("foreground")</li>
		///    <li>this Container's focusability ("focusable")</li>
		///    <li>this Container's focus traversal keys enabled state
		///        ("focusTraversalKeysEnabled")</li>
		///    <li>this Container's Set of FORWARD_TRAVERSAL_KEYS
		///        ("forwardFocusTraversalKeys")</li>
		///    <li>this Container's Set of BACKWARD_TRAVERSAL_KEYS
		///        ("backwardFocusTraversalKeys")</li>
		///    <li>this Container's Set of UP_CYCLE_TRAVERSAL_KEYS
		///        ("upCycleFocusTraversalKeys")</li>
		///    <li>this Container's Set of DOWN_CYCLE_TRAVERSAL_KEYS
		///        ("downCycleFocusTraversalKeys")</li>
		///    <li>this Container's focus traversal policy ("focusTraversalPolicy")
		///        </li>
		///    <li>this Container's focus-cycle-root state ("focusCycleRoot")</li>
		/// </ul>
		/// Note that if this Container is inheriting a bound property, then no
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
		/// following defaults:
		/// <ul>
		///    <li>this Container's font ("font")</li>
		///    <li>this Container's background color ("background")</li>
		///    <li>this Container's foreground color ("foreground")</li>
		///    <li>this Container's focusability ("focusable")</li>
		///    <li>this Container's focus traversal keys enabled state
		///        ("focusTraversalKeysEnabled")</li>
		///    <li>this Container's Set of FORWARD_TRAVERSAL_KEYS
		///        ("forwardFocusTraversalKeys")</li>
		///    <li>this Container's Set of BACKWARD_TRAVERSAL_KEYS
		///        ("backwardFocusTraversalKeys")</li>
		///    <li>this Container's Set of UP_CYCLE_TRAVERSAL_KEYS
		///        ("upCycleFocusTraversalKeys")</li>
		///    <li>this Container's Set of DOWN_CYCLE_TRAVERSAL_KEYS
		///        ("downCycleFocusTraversalKeys")</li>
		///    <li>this Container's focus traversal policy ("focusTraversalPolicy")
		///        </li>
		///    <li>this Container's focus-cycle-root state ("focusCycleRoot")</li>
		///    <li>this Container's focus-traversal-policy-provider state("focusTraversalPolicyProvider")</li>
		///    <li>this Container's focus-traversal-policy-provider state("focusTraversalPolicyProvider")</li>
		/// </ul>
		/// Note that if this Container is inheriting a bound property, then no
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

		// Serialization support. A Container is responsible for restoring the
		// parent fields of its component children.

		/// <summary>
		/// Container Serial Data Version.
		/// </summary>
		private int ContainerSerializedDataVersion = 1;

		/// <summary>
		/// Serializes this <code>Container</code> to the specified
		/// <code>ObjectOutputStream</code>.
		/// <ul>
		///    <li>Writes default serializable fields to the stream.</li>
		///    <li>Writes a list of serializable ContainerListener(s) as optional
		///        data. The non-serializable ContainerListner(s) are detected and
		///        no attempt is made to serialize them.</li>
		///    <li>Write this Container's FocusTraversalPolicy if and only if it
		///        is Serializable; otherwise, <code>null</code> is written.</li>
		/// </ul>
		/// </summary>
		/// <param name="s"> the <code>ObjectOutputStream</code> to write
		/// @serialData <code>null</code> terminated sequence of 0 or more pairs;
		///   the pair consists of a <code>String</code> and <code>Object</code>;
		///   the <code>String</code> indicates the type of object and
		///   is one of the following:
		///   <code>containerListenerK</code> indicating an
		///     <code>ContainerListener</code> object;
		///   the <code>Container</code>'s <code>FocusTraversalPolicy</code>,
		///     or <code>null</code>
		/// </param>
		/// <seealso cref= AWTEventMulticaster#save(java.io.ObjectOutputStream, java.lang.String, java.util.EventListener) </seealso>
		/// <seealso cref= Container#containerListenerK </seealso>
		/// <seealso cref= #readObject(ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			ObjectOutputStream.PutField f = s.PutFields();
			f.Put("ncomponents", Component.Count);
			f.Put("component", Component.toArray(EMPTY_ARRAY));
			f.Put("layoutMgr", LayoutMgr);
			f.Put("dispatcher", Dispatcher);
			f.Put("maxSize", MaxSize);
			f.Put("focusCycleRoot", FocusCycleRoot_Renamed);
			f.Put("containerSerializedDataVersion", ContainerSerializedDataVersion);
			f.Put("focusTraversalPolicyProvider", FocusTraversalPolicyProvider_Renamed);
			s.WriteFields();

			AWTEventMulticaster.Save(s, ContainerListenerK, ContainerListener);
			s.WriteObject(null);

			if (FocusTraversalPolicy_Renamed is java.io.Serializable)
			{
				s.WriteObject(FocusTraversalPolicy_Renamed);
			}
			else
			{
				s.WriteObject(null);
			}
		}

		/// <summary>
		/// Deserializes this <code>Container</code> from the specified
		/// <code>ObjectInputStream</code>.
		/// <ul>
		///    <li>Reads default serializable fields from the stream.</li>
		///    <li>Reads a list of serializable ContainerListener(s) as optional
		///        data. If the list is null, no Listeners are installed.</li>
		///    <li>Reads this Container's FocusTraversalPolicy, which may be null,
		///        as optional data.</li>
		/// </ul>
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to read
		/// @serial </param>
		/// <seealso cref= #addContainerListener </seealso>
		/// <seealso cref= #writeObject(ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream s)
		{
			ObjectInputStream.GetField f = s.ReadFields();
			Component[] tmpComponent = (Component[])f.Get("component", EMPTY_ARRAY);
			int ncomponents = (Integer) f.Get("ncomponents", 0);
			Component = new List<Component>(ncomponents);
			for (int i = 0; i < ncomponents; ++i)
			{
				Component.Add(tmpComponent[i]);
			}
			LayoutMgr = (LayoutManager)f.Get("layoutMgr", null);
			Dispatcher = (LightweightDispatcher)f.Get("dispatcher", null);
			// Old stream. Doesn't contain maxSize among Component's fields.
			if (MaxSize == null)
			{
				MaxSize = (Dimension)f.Get("maxSize", null);
			}
			FocusCycleRoot_Renamed = f.Get("focusCycleRoot", false);
			ContainerSerializedDataVersion = f.Get("containerSerializedDataVersion", 1);
			FocusTraversalPolicyProvider_Renamed = f.Get("focusTraversalPolicyProvider", false);
			IList<Component> component = this.Component;
			foreach (Component comp in component)
			{
				comp.Parent_Renamed = this;
				AdjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, comp.NumListening(AWTEvent.HIERARCHY_EVENT_MASK));
				AdjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, comp.NumListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK));
				AdjustDescendants(comp.CountHierarchyMembers());
			}

			Object keyOrNull;
			while (null != (keyOrNull = s.ReadObject()))
			{
				String key = ((String)keyOrNull).intern();

				if (ContainerListenerK == key)
				{
					AddContainerListener((ContainerListener)(s.ReadObject()));
				}
				else
				{
					// skip value for unrecognized key
					s.ReadObject();
				}
			}

			try
			{
				Object policy = s.ReadObject();
				if (policy is FocusTraversalPolicy)
				{
					FocusTraversalPolicy_Renamed = (FocusTraversalPolicy)policy;
				}
			}
			catch (java.io.OptionalDataException e)
			{
				// JDK 1.1/1.2/1.3 instances will not have this optional data.
				// e.eof will be true to indicate that there is no more data
				// available for this object. If e.eof is not true, throw the
				// exception as it might have been caused by reasons unrelated to
				// focusTraversalPolicy.

				if (!e.Eof)
				{
					throw e;
				}
			}
		}

		/*
		 * --- Accessibility Support ---
		 */

		/// <summary>
		/// Inner class of Container used to provide default support for
		/// accessibility.  This class is not meant to be used directly by
		/// application developers, but is instead meant only to be
		/// subclassed by container developers.
		/// <para>
		/// The class used to obtain the accessible role for this object,
		/// as well as implementing many of the methods in the
		/// AccessibleContainer interface.
		/// @since 1.3
		/// </para>
		/// </summary>
		protected internal class AccessibleAWTContainer : AccessibleAWTComponent
		{
			private readonly Container OuterInstance;

			public AccessibleAWTContainer(Container outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}


			/// <summary>
			/// JDK1.3 serialVersionUID
			/// </summary>
			internal const long SerialVersionUID = 5081320404842566097L;

			/// <summary>
			/// Returns the number of accessible children in the object.  If all
			/// of the children of this object implement <code>Accessible</code>,
			/// then this method should return the number of children of this object.
			/// </summary>
			/// <returns> the number of accessible children in the object </returns>
			public override int AccessibleChildrenCount
			{
				get
				{
					return OuterInstance.AccessibleChildrenCount;
				}
			}

			/// <summary>
			/// Returns the nth <code>Accessible</code> child of the object.
			/// </summary>
			/// <param name="i"> zero-based index of child </param>
			/// <returns> the nth <code>Accessible</code> child of the object </returns>
			public override Accessible GetAccessibleChild(int i)
			{
				return OuterInstance.GetAccessibleChild(i);
			}

			/// <summary>
			/// Returns the <code>Accessible</code> child, if one exists,
			/// contained at the local coordinate <code>Point</code>.
			/// </summary>
			/// <param name="p"> the point defining the top-left corner of the
			///    <code>Accessible</code>, given in the coordinate space
			///    of the object's parent </param>
			/// <returns> the <code>Accessible</code>, if it exists,
			///    at the specified location; else <code>null</code> </returns>
			public override Accessible GetAccessibleAt(Point p)
			{
				return OuterInstance.GetAccessibleAt(p);
			}

			/// <summary>
			/// Number of PropertyChangeListener objects registered. It's used
			/// to add/remove ContainerListener to track target Container's state.
			/// </summary>
			[NonSerialized]
			internal volatile int PropertyListenersCount = 0;

			protected internal ContainerListener AccessibleContainerHandler = null;

			/// <summary>
			/// Fire <code>PropertyChange</code> listener, if one is registered,
			/// when children are added or removed.
			/// @since 1.3
			/// </summary>
			protected internal class AccessibleContainerHandler : ContainerListener
			{
				private readonly Container.AccessibleAWTContainer OuterInstance;

				public AccessibleContainerHandler(Container.AccessibleAWTContainer outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public virtual void ComponentAdded(ContainerEvent e)
				{
					Component c = e.Child;
					if (c != null && c is Accessible)
					{
						OuterInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_CHILD_PROPERTY, null, ((Accessible) c).AccessibleContext);
					}
				}
				public virtual void ComponentRemoved(ContainerEvent e)
				{
					Component c = e.Child;
					if (c != null && c is Accessible)
					{
						OuterInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_CHILD_PROPERTY, ((Accessible) c).AccessibleContext, null);
					}
				}
			}

			/// <summary>
			/// Adds a PropertyChangeListener to the listener list.
			/// </summary>
			/// <param name="listener">  the PropertyChangeListener to be added </param>
			public override void AddPropertyChangeListener(PropertyChangeListener listener)
			{
				if (AccessibleContainerHandler == null)
				{
					AccessibleContainerHandler = new AccessibleContainerHandler(this);
				}
				if (PropertyListenersCount++ == 0)
				{
					OuterInstance.AddContainerListener(AccessibleContainerHandler);
				}
				base.AddPropertyChangeListener(listener);
			}

			/// <summary>
			/// Remove a PropertyChangeListener from the listener list.
			/// This removes a PropertyChangeListener that was registered
			/// for all properties.
			/// </summary>
			/// <param name="listener"> the PropertyChangeListener to be removed </param>
			public override void RemovePropertyChangeListener(PropertyChangeListener listener)
			{
				if (--PropertyListenersCount == 0)
				{
					OuterInstance.RemoveContainerListener(AccessibleContainerHandler);
				}
				base.RemovePropertyChangeListener(listener);
			}

		} // inner class AccessibleAWTContainer

		/// <summary>
		/// Returns the <code>Accessible</code> child contained at the local
		/// coordinate <code>Point</code>, if one exists.  Otherwise
		/// returns <code>null</code>.
		/// </summary>
		/// <param name="p"> the point defining the top-left corner of the
		///    <code>Accessible</code>, given in the coordinate space
		///    of the object's parent </param>
		/// <returns> the <code>Accessible</code> at the specified location,
		///    if it exists; otherwise <code>null</code> </returns>
		internal virtual Accessible GetAccessibleAt(Point p)
		{
			lock (TreeLock)
			{
				if (this is Accessible)
				{
					Accessible a = (Accessible)this;
					AccessibleContext ac = a.AccessibleContext;
					if (ac != null)
					{
						AccessibleComponent acmp;
						Point location;
						int nchildren = ac.AccessibleChildrenCount;
						for (int i = 0; i < nchildren; i++)
						{
							a = ac.getAccessibleChild(i);
							if ((a != null))
							{
								ac = a.AccessibleContext;
								if (ac != null)
								{
									acmp = ac.AccessibleComponent;
									if ((acmp != null) && (acmp.Showing))
									{
										location = acmp.Location;
										Point np = new Point(p.x - location.x, p.y - location.y);
										if (acmp.contains(np))
										{
											return a;
										}
									}
								}
							}
						}
					}
					return (Accessible)this;
				}
				else
				{
					Component ret = this;
					if (!this.Contains(p.x,p.y))
					{
						ret = null;
					}
					else
					{
						int ncomponents = this.ComponentCount;
						for (int i = 0; i < ncomponents; i++)
						{
							Component comp = this.GetComponent(i);
							if ((comp != null) && comp.Showing)
							{
								Point location = comp.Location;
								if (comp.Contains(p.x - location.x,p.y - location.y))
								{
									ret = comp;
								}
							}
						}
					}
					if (ret is Accessible)
					{
						return (Accessible) ret;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Returns the number of accessible children in the object.  If all
		/// of the children of this object implement <code>Accessible</code>,
		/// then this method should return the number of children of this object.
		/// </summary>
		/// <returns> the number of accessible children in the object </returns>
		internal virtual int AccessibleChildrenCount
		{
			get
			{
				lock (TreeLock)
				{
					int count = 0;
					Component[] children = this.Components;
					for (int i = 0; i < children.Length; i++)
					{
						if (children[i] is Accessible)
						{
							count++;
						}
					}
					return count;
				}
			}
		}

		/// <summary>
		/// Returns the nth <code>Accessible</code> child of the object.
		/// </summary>
		/// <param name="i"> zero-based index of child </param>
		/// <returns> the nth <code>Accessible</code> child of the object </returns>
		internal virtual Accessible GetAccessibleChild(int i)
		{
			lock (TreeLock)
			{
				Component[] children = this.Components;
				int count = 0;
				for (int j = 0; j < children.Length; j++)
				{
					if (children[j] is Accessible)
					{
						if (count == i)
						{
							return (Accessible) children[j];
						}
						else
						{
							count++;
						}
					}
				}
				return null;
			}
		}

		// ************************** MIXING CODE *******************************

		internal void IncreaseComponentCount(Component c)
		{
			lock (TreeLock)
			{
				if (!c.Displayable)
				{
					throw new IllegalStateException("Peer does not exist while invoking the increaseComponentCount() method");
				}

				int addHW = 0;
				int addLW = 0;

				if (c is Container)
				{
					addLW = ((Container)c).NumOfLWComponents;
					addHW = ((Container)c).NumOfHWComponents;
				}
				if (c.Lightweight)
				{
					addLW++;
				}
				else
				{
					addHW++;
				}

				for (Container cont = this; cont != null; cont = cont.Container)
				{
					cont.NumOfLWComponents += addLW;
					cont.NumOfHWComponents += addHW;
				}
			}
		}

		internal void DecreaseComponentCount(Component c)
		{
			lock (TreeLock)
			{
				if (!c.Displayable)
				{
					throw new IllegalStateException("Peer does not exist while invoking the decreaseComponentCount() method");
				}

				int subHW = 0;
				int subLW = 0;

				if (c is Container)
				{
					subLW = ((Container)c).NumOfLWComponents;
					subHW = ((Container)c).NumOfHWComponents;
				}
				if (c.Lightweight)
				{
					subLW++;
				}
				else
				{
					subHW++;
				}

				for (Container cont = this; cont != null; cont = cont.Container)
				{
					cont.NumOfLWComponents -= subLW;
					cont.NumOfHWComponents -= subHW;
				}
			}
		}

		private int TopmostComponentIndex
		{
			get
			{
				CheckTreeLock();
				if (ComponentCount > 0)
				{
					return 0;
				}
				return -1;
			}
		}

		private int BottommostComponentIndex
		{
			get
			{
				CheckTreeLock();
				if (ComponentCount > 0)
				{
					return ComponentCount - 1;
				}
				return -1;
			}
		}

		/*
		 * This method is overriden to handle opaque children in non-opaque
		 * containers.
		 */
		internal override sealed Region OpaqueShape
		{
			get
			{
				CheckTreeLock();
				if (Lightweight && NonOpaqueForMixing && HasLightweightDescendants())
				{
					Region s = Region.EMPTY_REGION;
					for (int index = 0; index < ComponentCount; index++)
					{
						Component c = GetComponent(index);
						if (c.Lightweight && c.Showing)
						{
							s = s.getUnion(c.OpaqueShape);
						}
					}
					return s.getIntersection(NormalShape);
				}
				return base.OpaqueShape;
			}
		}

		internal void RecursiveSubtractAndApplyShape(Region shape)
		{
			RecursiveSubtractAndApplyShape(shape, TopmostComponentIndex, BottommostComponentIndex);
		}

		internal void RecursiveSubtractAndApplyShape(Region shape, int fromZorder)
		{
			RecursiveSubtractAndApplyShape(shape, fromZorder, BottommostComponentIndex);
		}

		internal void RecursiveSubtractAndApplyShape(Region shape, int fromZorder, int toZorder)
		{
			CheckTreeLock();
			if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
			{
				MixingLog.fine("this = " + this + "; shape=" + shape + "; fromZ=" + fromZorder + "; toZ=" + toZorder);
			}
			if (fromZorder == -1)
			{
				return;
			}
			if (shape.Empty)
			{
				return;
			}
			// An invalid container with not-null layout should be ignored
			// by the mixing code, the container will be validated later
			// and the mixing code will be executed later.
			if (Layout != null && !Valid)
			{
				return;
			}
			for (int index = fromZorder; index <= toZorder; index++)
			{
				Component comp = GetComponent(index);
				if (!comp.Lightweight)
				{
					comp.SubtractAndApplyShape(shape);
				}
				else if (comp is Container && ((Container)comp).HasHeavyweightDescendants() && comp.Showing)
				{
					((Container)comp).RecursiveSubtractAndApplyShape(shape);
				}
			}
		}

		internal void RecursiveApplyCurrentShape()
		{
			RecursiveApplyCurrentShape(TopmostComponentIndex, BottommostComponentIndex);
		}

		internal void RecursiveApplyCurrentShape(int fromZorder)
		{
			RecursiveApplyCurrentShape(fromZorder, BottommostComponentIndex);
		}

		internal void RecursiveApplyCurrentShape(int fromZorder, int toZorder)
		{
			CheckTreeLock();
			if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
			{
				MixingLog.fine("this = " + this + "; fromZ=" + fromZorder + "; toZ=" + toZorder);
			}
			if (fromZorder == -1)
			{
				return;
			}
			// An invalid container with not-null layout should be ignored
			// by the mixing code, the container will be validated later
			// and the mixing code will be executed later.
			if (Layout != null && !Valid)
			{
				return;
			}
			for (int index = fromZorder; index <= toZorder; index++)
			{
				Component comp = GetComponent(index);
				if (!comp.Lightweight)
				{
					comp.ApplyCurrentShape();
				}
				if (comp is Container && ((Container)comp).HasHeavyweightDescendants())
				{
					((Container)comp).RecursiveApplyCurrentShape();
				}
			}
		}

		private void RecursiveShowHeavyweightChildren()
		{
			if (!HasHeavyweightDescendants() || !Visible)
			{
				return;
			}
			for (int index = 0; index < ComponentCount; index++)
			{
				Component comp = GetComponent(index);
				if (comp.Lightweight)
				{
					if (comp is Container)
					{
						((Container)comp).RecursiveShowHeavyweightChildren();
					}
				}
				else
				{
					if (comp.Visible)
					{
						ComponentPeer peer = comp.Peer;
						if (peer != null)
						{
							peer.Visible = true;
						}
					}
				}
			}
		}

		private void RecursiveHideHeavyweightChildren()
		{
			if (!HasHeavyweightDescendants())
			{
				return;
			}
			for (int index = 0; index < ComponentCount; index++)
			{
				Component comp = GetComponent(index);
				if (comp.Lightweight)
				{
					if (comp is Container)
					{
						((Container)comp).RecursiveHideHeavyweightChildren();
					}
				}
				else
				{
					if (comp.Visible)
					{
						ComponentPeer peer = comp.Peer;
						if (peer != null)
						{
							peer.Visible = false;
						}
					}
				}
			}
		}

		private void RecursiveRelocateHeavyweightChildren(Point origin)
		{
			for (int index = 0; index < ComponentCount; index++)
			{
				Component comp = GetComponent(index);
				if (comp.Lightweight)
				{
					if (comp is Container && ((Container)comp).HasHeavyweightDescendants())
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Point newOrigin = new Point(origin);
						Point newOrigin = new Point(origin);
						newOrigin.Translate(comp.X, comp.Y);
						((Container)comp).RecursiveRelocateHeavyweightChildren(newOrigin);
					}
				}
				else
				{
					ComponentPeer peer = comp.Peer;
					if (peer != null)
					{
						peer.SetBounds(origin.x + comp.X, origin.y + comp.Y, comp.Width, comp.Height, java.awt.peer.ComponentPeer_Fields.SET_LOCATION);
					}
				}
			}
		}

		/// <summary>
		/// Checks if the container and its direct lightweight containers are
		/// visible.
		/// 
		/// Consider the heavyweight container hides or shows the HW descendants
		/// automatically. Therefore we care of LW containers' visibility only.
		/// 
		/// This method MUST be invoked under the TreeLock.
		/// </summary>
		internal bool RecursivelyVisibleUpToHeavyweightContainer
		{
			get
			{
				if (!Lightweight)
				{
					return true;
				}
    
				for (Container cont = this; cont != null && cont.Lightweight; cont = cont.Container)
				{
					if (!cont.Visible)
					{
						return false;
					}
				}
				return true;
			}
		}

		internal override void MixOnShowing()
		{
			lock (TreeLock)
			{
				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this);
				}

				bool isLightweight = Lightweight;

				if (isLightweight && RecursivelyVisibleUpToHeavyweightContainer)
				{
					RecursiveShowHeavyweightChildren();
				}

				if (!MixingNeeded)
				{
					return;
				}

				if (!isLightweight || (isLightweight && HasHeavyweightDescendants()))
				{
					RecursiveApplyCurrentShape();
				}

				base.MixOnShowing();
			}
		}

		internal override void MixOnHiding(bool isLightweight)
		{
			lock (TreeLock)
			{
				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this + "; isLightweight=" + isLightweight);
				}
				if (isLightweight)
				{
					RecursiveHideHeavyweightChildren();
				}
				base.MixOnHiding(isLightweight);
			}
		}

		internal override void MixOnReshaping()
		{
			lock (TreeLock)
			{
				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this);
				}

				bool isMixingNeeded = MixingNeeded;

				if (Lightweight && HasHeavyweightDescendants())
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Point origin = new Point(getX(), getY());
					Point origin = new Point(X, Y);
					for (Container cont = Container; cont != null && cont.Lightweight; cont = cont.Container)
					{
						origin.Translate(cont.X, cont.Y);
					}

					RecursiveRelocateHeavyweightChildren(origin);

					if (!isMixingNeeded)
					{
						return;
					}

					RecursiveApplyCurrentShape();
				}

				if (!isMixingNeeded)
				{
					return;
				}

				base.MixOnReshaping();
			}
		}

		internal override void MixOnZOrderChanging(int oldZorder, int newZorder)
		{
			lock (TreeLock)
			{
				if (MixingLog.isLoggable(PlatformLogger.Level.FINE))
				{
					MixingLog.fine("this = " + this + "; oldZ=" + oldZorder + "; newZ=" + newZorder);
				}

				if (!MixingNeeded)
				{
					return;
				}

				bool becameHigher = newZorder < oldZorder;

				if (becameHigher && Lightweight && HasHeavyweightDescendants())
				{
					RecursiveApplyCurrentShape();
				}
				base.MixOnZOrderChanging(oldZorder, newZorder);
			}
		}

		internal override void MixOnValidating()
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

				if (HasHeavyweightDescendants())
				{
					RecursiveApplyCurrentShape();
				}

				if (Lightweight && NonOpaqueForMixing)
				{
					SubtractAndApplyShapeBelowMe();
				}

				base.MixOnValidating();
			}
		}

		// ****************** END OF MIXING CODE ********************************
	}


	/// <summary>
	/// Class to manage the dispatching of MouseEvents to the lightweight descendants
	/// and SunDropTargetEvents to both lightweight and heavyweight descendants
	/// contained by a native container.
	/// 
	/// NOTE: the class name is not appropriate anymore, but we cannot change it
	/// because we must keep serialization compatibility.
	/// 
	/// @author Timothy Prinzing
	/// </summary>
	[Serializable]
	internal class LightweightDispatcher : AWTEventListener
	{

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 5184291520170872969L;
		/*
		 * Our own mouse event for when we're dragged over from another hw
		 * container
		 */
		private const int LWD_MOUSE_DRAGGED_OVER = 1500;

		private static readonly PlatformLogger EventLog = PlatformLogger.getLogger("java.awt.event.LightweightDispatcher");

		private static readonly int BUTTONS_DOWN_MASK;

		static LightweightDispatcher()
		{
			int[] buttonsDownMask = AWTAccessor.InputEventAccessor.ButtonDownMasks;
			int mask = 0;
			foreach (int buttonDownMask in buttonsDownMask)
			{
				mask |= buttonDownMask;
			}
			BUTTONS_DOWN_MASK = mask;
		}

		internal LightweightDispatcher(Container nativeContainer)
		{
			this.NativeContainer = nativeContainer;
			MouseEventTarget = new WeakReference<>(null);
			TargetLastEntered = new WeakReference<>(null);
			TargetLastEnteredDT = new WeakReference<>(null);
			EventMask = 0;
		}

		/*
		 * Clean up any resources allocated when dispatcher was created;
		 * should be called from Container.removeNotify
		 */
		internal virtual void Dispose()
		{
			//System.out.println("Disposing lw dispatcher");
			StopListeningForOtherDrags();
			MouseEventTarget.clear();
			TargetLastEntered.clear();
			TargetLastEnteredDT.clear();
		}

		/// <summary>
		/// Enables events to subcomponents.
		/// </summary>
		internal virtual void EnableEvents(long events)
		{
			EventMask |= events;
		}

		/// <summary>
		/// Dispatches an event to a sub-component if necessary, and
		/// returns whether or not the event was forwarded to a
		/// sub-component.
		/// </summary>
		/// <param name="e"> the event </param>
		internal virtual bool DispatchEvent(AWTEvent e)
		{
			bool ret = false;

			/*
			 * Fix for BugTraq Id 4389284.
			 * Dispatch SunDropTargetEvents regardless of eventMask value.
			 * Do not update cursor on dispatching SunDropTargetEvents.
			 */
			if (e is SunDropTargetEvent)
			{

				SunDropTargetEvent sdde = (SunDropTargetEvent) e;
				ret = ProcessDropTargetEvent(sdde);

			}
			else
			{
				if (e is MouseEvent && (EventMask & MOUSE_MASK) != 0)
				{
					MouseEvent me = (MouseEvent) e;
					ret = ProcessMouseEvent(me);
				}

				if (e.ID == MouseEvent.MOUSE_MOVED)
				{
					NativeContainer.UpdateCursorImmediately();
				}
			}

			return ret;
		}

		/* This method effectively returns whether or not a mouse button was down
		 * just BEFORE the event happened.  A better method name might be
		 * wasAMouseButtonDownBeforeThisEvent().
		 */
		private bool IsMouseGrab(MouseEvent e)
		{
			int modifiers = e.ModifiersEx;

			if (e.ID == MouseEvent.MOUSE_PRESSED || e.ID == MouseEvent.MOUSE_RELEASED)
			{
				modifiers ^= InputEvent.GetMaskForButton(e.Button);
			}
			/* modifiers now as just before event */
			return ((modifiers & BUTTONS_DOWN_MASK) != 0);
		}

		/// <summary>
		/// This method attempts to distribute a mouse event to a lightweight
		/// component.  It tries to avoid doing any unnecessary probes down
		/// into the component tree to minimize the overhead of determining
		/// where to route the event, since mouse movement events tend to
		/// come in large and frequent amounts.
		/// </summary>
		private bool ProcessMouseEvent(MouseEvent e)
		{
			int id = e.ID;
			Component mouseOver = NativeContainer.GetMouseEventTarget(e.X, e.Y, Container.INCLUDE_SELF); // sensitive to mouse events

			TrackMouseEnterExit(mouseOver, e);

			Component met = MouseEventTarget.get();
			// 4508327 : MOUSE_CLICKED should only go to the recipient of
			// the accompanying MOUSE_PRESSED, so don't reset mouseEventTarget on a
			// MOUSE_CLICKED.
			if (!IsMouseGrab(e) && id != MouseEvent.MOUSE_CLICKED)
			{
				met = (mouseOver != NativeContainer) ? mouseOver : null;
				MouseEventTarget = new WeakReference<>(met);
			}

			if (met != null)
			{
				switch (id)
				{
					case MouseEvent.MOUSE_ENTERED:
					case MouseEvent.MOUSE_EXITED:
						break;
					case MouseEvent.MOUSE_PRESSED:
						RetargetMouseEvent(met, id, e);
						break;
					case MouseEvent.MOUSE_RELEASED:
						RetargetMouseEvent(met, id, e);
						break;
					case MouseEvent.MOUSE_CLICKED:
						// 4508327: MOUSE_CLICKED should never be dispatched to a Component
						// other than that which received the MOUSE_PRESSED event.  If the
						// mouse is now over a different Component, don't dispatch the event.
						// The previous fix for a similar problem was associated with bug
						// 4155217.
						if (mouseOver == met)
						{
							RetargetMouseEvent(mouseOver, id, e);
						}
						break;
					case MouseEvent.MOUSE_MOVED:
						RetargetMouseEvent(met, id, e);
						break;
					case MouseEvent.MOUSE_DRAGGED:
						if (IsMouseGrab(e))
						{
							RetargetMouseEvent(met, id, e);
						}
						break;
					case MouseEvent.MOUSE_WHEEL:
						// This may send it somewhere that doesn't have MouseWheelEvents
						// enabled.  In this case, Component.dispatchEventImpl() will
						// retarget the event to a parent that DOES have the events enabled.
						if (EventLog.isLoggable(PlatformLogger.Level.FINEST) && (mouseOver != null))
						{
							EventLog.finest("retargeting mouse wheel to " + mouseOver.Name + ", " + mouseOver.GetType());
						}
						RetargetMouseEvent(mouseOver, id, e);
						break;
				}
				//Consuming of wheel events is implemented in "retargetMouseEvent".
				if (id != MouseEvent.MOUSE_WHEEL)
				{
					e.Consume();
				}
			}
			return e.Consumed;
		}

		private bool ProcessDropTargetEvent(SunDropTargetEvent e)
		{
			int id = e.ID;
			int x = e.X;
			int y = e.Y;

			/*
			 * Fix for BugTraq ID 4395290.
			 * It is possible that SunDropTargetEvent's Point is outside of the
			 * native container bounds. In this case we truncate coordinates.
			 */
			if (!NativeContainer.Contains(x, y))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dimension d = nativeContainer.getSize();
				Dimension d = NativeContainer.Size;
				if (d.Width_Renamed <= x)
				{
					x = d.Width_Renamed - 1;
				}
				else if (x < 0)
				{
					x = 0;
				}
				if (d.Height_Renamed <= y)
				{
					y = d.Height_Renamed - 1;
				}
				else if (y < 0)
				{
					y = 0;
				}
			}
			Component mouseOver = NativeContainer.GetDropTargetEventTarget(x, y, Container.INCLUDE_SELF); // not necessarily sensitive to mouse events
			TrackMouseEnterExit(mouseOver, e);

			if (mouseOver != NativeContainer && mouseOver != null)
			{
				switch (id)
				{
				case SunDropTargetEvent.MOUSE_ENTERED:
				case SunDropTargetEvent.MOUSE_EXITED:
					break;
				default:
					RetargetMouseEvent(mouseOver, id, e);
					e.consume();
					break;
				}
			}
			return e.Consumed;
		}

		/*
		 * Generates dnd enter/exit events as mouse moves over lw components
		 * @param targetOver       Target mouse is over (including native container)
		 * @param e                SunDropTarget mouse event in native container
		 */
		private void TrackDropTargetEnterExit(Component targetOver, MouseEvent e)
		{
			int id = e.ID;
			if (id == MouseEvent.MOUSE_ENTERED && IsMouseDTInNativeContainer)
			{
				// This can happen if a lightweight component which initiated the
				// drag has an associated drop target. MOUSE_ENTERED comes when the
				// mouse is in the native container already. To propagate this event
				// properly we should null out targetLastEntered.
				TargetLastEnteredDT.clear();
			}
			else if (id == MouseEvent.MOUSE_ENTERED)
			{
				IsMouseDTInNativeContainer = true;
			}
			else if (id == MouseEvent.MOUSE_EXITED)
			{
				IsMouseDTInNativeContainer = false;
			}
			Component tle = RetargetMouseEnterExit(targetOver, e, TargetLastEnteredDT.get(), IsMouseDTInNativeContainer);
			TargetLastEnteredDT = new WeakReference<>(tle);
		}

		/*
		 * Generates enter/exit events as mouse moves over lw components
		 * @param targetOver        Target mouse is over (including native container)
		 * @param e                 Mouse event in native container
		 */
		private void TrackMouseEnterExit(Component targetOver, MouseEvent e)
		{
			if (e is SunDropTargetEvent)
			{
				TrackDropTargetEnterExit(targetOver, e);
				return;
			}
			int id = e.ID;

			if (id != MouseEvent.MOUSE_EXITED && id != MouseEvent.MOUSE_DRAGGED && id != LWD_MOUSE_DRAGGED_OVER && !IsMouseInNativeContainer)
			{
				// any event but an exit or drag means we're in the native container
				IsMouseInNativeContainer = true;
				StartListeningForOtherDrags();
			}
			else if (id == MouseEvent.MOUSE_EXITED)
			{
				IsMouseInNativeContainer = false;
				StopListeningForOtherDrags();
			}
			Component tle = RetargetMouseEnterExit(targetOver, e, TargetLastEntered.get(), IsMouseInNativeContainer);
			TargetLastEntered = new WeakReference<>(tle);
		}

		private Component RetargetMouseEnterExit(Component targetOver, MouseEvent e, Component lastEntered, bool inNativeContainer)
		{
			int id = e.ID;
			Component targetEnter = inNativeContainer ? targetOver : null;

			if (lastEntered != targetEnter)
			{
				if (lastEntered != null)
				{
					RetargetMouseEvent(lastEntered, MouseEvent.MOUSE_EXITED, e);
				}
				if (id == MouseEvent.MOUSE_EXITED)
				{
					// consume native exit event if we generate one
					e.Consume();
				}

				if (targetEnter != null)
				{
					RetargetMouseEvent(targetEnter, MouseEvent.MOUSE_ENTERED, e);
				}
				if (id == MouseEvent.MOUSE_ENTERED)
				{
					// consume native enter event if we generate one
					e.Consume();
				}
			}
			return targetEnter;
		}

		/*
		 * Listens to global mouse drag events so even drags originating
		 * from other heavyweight containers will generate enter/exit
		 * events in this container
		 */
		private void StartListeningForOtherDrags()
		{
			//System.out.println("Adding AWTEventListener");
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this)
		   );
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<Object>
		{
			private readonly LightweightDispatcher OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(LightweightDispatcher outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Object Run()
			{
				OuterInstance.NativeContainer.Toolkit.AddAWTEventListener(OuterInstance, AWTEvent.MOUSE_EVENT_MASK | AWTEvent.MOUSE_MOTION_EVENT_MASK);
				return null;
			}
		}

		private void StopListeningForOtherDrags()
		{
			//System.out.println("Removing AWTEventListener");
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this)
		   );
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : java.security.PrivilegedAction<Object>
		{
			private readonly LightweightDispatcher OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper2(LightweightDispatcher outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Object Run()
			{
				OuterInstance.NativeContainer.Toolkit.RemoveAWTEventListener(OuterInstance);
				return null;
			}
		}

		/*
		 * (Implementation of AWTEventListener)
		 * Listen for drag events posted in other hw components so we can
		 * track enter/exit regardless of where a drag originated
		 */
		public virtual void EventDispatched(AWTEvent e)
		{
			bool isForeignDrag = (e is MouseEvent) && !(e is SunDropTargetEvent) && (e.Id == MouseEvent.MOUSE_DRAGGED) && (e.Source != NativeContainer);

			if (!isForeignDrag)
			{
				// only interested in drags from other hw components
				return;
			}

			MouseEvent srcEvent = (MouseEvent)e;
			MouseEvent me;

			lock (NativeContainer.TreeLock)
			{
				Component srcComponent = srcEvent.Component;

				// component may have disappeared since drag event posted
				// (i.e. Swing hierarchical menus)
				if (!srcComponent.Showing)
				{
					return;
				}

				// see 5083555
				// check if srcComponent is in any modal blocked window
				Component c = NativeContainer;
				while ((c != null) && !(c is Window))
				{
					c = c.Parent_NoClientCode;
				}
				if ((c == null) || ((Window)c).ModalBlocked)
				{
					return;
				}

				//
				// create an internal 'dragged-over' event indicating
				// we are being dragged over from another hw component
				//
				me = new MouseEvent(NativeContainer, LWD_MOUSE_DRAGGED_OVER, srcEvent.When, srcEvent.ModifiersEx | srcEvent.Modifiers, srcEvent.X, srcEvent.Y, srcEvent.XOnScreen, srcEvent.YOnScreen, srcEvent.ClickCount, srcEvent.PopupTrigger, srcEvent.Button);
				((AWTEvent)srcEvent).CopyPrivateDataInto(me);
				// translate coordinates to this native container
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Point ptSrcOrigin = srcComponent.getLocationOnScreen();
				Point ptSrcOrigin = srcComponent.LocationOnScreen;

				if (AppContext.AppContext != NativeContainer.AppContext)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MouseEvent mouseEvent = me;
					MouseEvent mouseEvent = me;
					Runnable r = new RunnableAnonymousInnerClassHelper(this, ptSrcOrigin, mouseEvent);
					SunToolkit.executeOnEventHandlerThread(NativeContainer, r);
					return;
				}
				else
				{
					if (!NativeContainer.Showing)
					{
						return;
					}

					Point ptDstOrigin = NativeContainer.LocationOnScreen;
					me.TranslatePoint(ptSrcOrigin.x - ptDstOrigin.x, ptSrcOrigin.y - ptDstOrigin.y);
				}
			}
			//System.out.println("Track event: " + me);
			// feed the 'dragged-over' event directly to the enter/exit
			// code (not a real event so don't pass it to dispatchEvent)
			Component targetOver = NativeContainer.GetMouseEventTarget(me.X, me.Y, Container.INCLUDE_SELF);
			TrackMouseEnterExit(targetOver, me);
		}

		private class RunnableAnonymousInnerClassHelper : Runnable
		{
			private readonly LightweightDispatcher OuterInstance;

			private java.awt.Point PtSrcOrigin;
			private java.awt.@event.MouseEvent MouseEvent;

			public RunnableAnonymousInnerClassHelper(LightweightDispatcher outerInstance, java.awt.Point ptSrcOrigin, java.awt.@event.MouseEvent mouseEvent)
			{
				this.OuterInstance = outerInstance;
				this.PtSrcOrigin = ptSrcOrigin;
				this.MouseEvent = mouseEvent;
			}

			public virtual void Run()
			{
				if (!OuterInstance.NativeContainer.Showing)
				{
					return;
				}

				Point ptDstOrigin = OuterInstance.NativeContainer.LocationOnScreen;
				MouseEvent.TranslatePoint(PtSrcOrigin.x - ptDstOrigin.x, PtSrcOrigin.y - ptDstOrigin.y);
				Component targetOver = OuterInstance.NativeContainer.GetMouseEventTarget(MouseEvent.X, MouseEvent.Y, Container.INCLUDE_SELF);
				outerInstance.TrackMouseEnterExit(targetOver, MouseEvent);
			}
		}

		/// <summary>
		/// Sends a mouse event to the current mouse event recipient using
		/// the given event (sent to the windowed host) as a srcEvent.  If
		/// the mouse event target is still in the component tree, the
		/// coordinates of the event are translated to those of the target.
		/// If the target has been removed, we don't bother to send the
		/// message.
		/// </summary>
		internal virtual void RetargetMouseEvent(Component target, int id, MouseEvent e)
		{
			if (target == null)
			{
				return; // mouse is over another hw component or target is disabled
			}

			int x = e.X, y = e.Y;
			Component component;

			for (component = target; component != null && component != NativeContainer; component = component.Parent)
			{
				x -= component.x;
				y -= component.y;
			}
			MouseEvent retargeted;
			if (component != null)
			{
				if (e is SunDropTargetEvent)
				{
					retargeted = new SunDropTargetEvent(target, id, x, y, ((SunDropTargetEvent)e).Dispatcher);
				}
				else if (id == MouseEvent.MOUSE_WHEEL)
				{
					retargeted = new MouseWheelEvent(target, id, e.When, e.ModifiersEx | e.Modifiers, x, y, e.XOnScreen, e.YOnScreen, e.ClickCount, e.PopupTrigger, ((MouseWheelEvent)e).ScrollType, ((MouseWheelEvent)e).ScrollAmount, ((MouseWheelEvent)e).WheelRotation, ((MouseWheelEvent)e).PreciseWheelRotation);
				}
				else
				{
					retargeted = new MouseEvent(target, id, e.When, e.ModifiersEx | e.Modifiers, x, y, e.XOnScreen, e.YOnScreen, e.ClickCount, e.PopupTrigger, e.Button);
				}

				((AWTEvent)e).CopyPrivateDataInto(retargeted);

				if (target == NativeContainer)
				{
					// avoid recursively calling LightweightDispatcher...
					((Container)target).DispatchEventToSelf(retargeted);
				}
				else
				{
					Debug.Assert(AppContext.AppContext == target.AppContext);

					if (NativeContainer.ModalComp != null)
					{
						if (((Container)NativeContainer.ModalComp).IsAncestorOf(target))
						{
							target.DispatchEvent(retargeted);
						}
						else
						{
							e.Consume();
						}
					}
					else
					{
						target.DispatchEvent(retargeted);
					}
				}
				if (id == MouseEvent.MOUSE_WHEEL && retargeted.Consumed)
				{
					//An exception for wheel bubbling to the native system.
					//In "processMouseEvent" total event consuming for wheel events is skipped.
					//Protection from bubbling of Java-accepted wheel events.
					e.Consume();
				}
			}
		}

		// --- member variables -------------------------------

		/// <summary>
		/// The windowed container that might be hosting events for
		/// subcomponents.
		/// </summary>
		private Container NativeContainer;

		/// <summary>
		/// This variable is not used, but kept for serialization compatibility
		/// </summary>
		private Component Focus;

		/// <summary>
		/// The current subcomponent being hosted by this windowed
		/// component that has events being forwarded to it.  If this
		/// is null, there are currently no events being forwarded to
		/// a subcomponent.
		/// </summary>
		[NonSerialized]
		private WeakReference<Component> MouseEventTarget;

		/// <summary>
		/// The last component entered by the {@code MouseEvent}.
		/// </summary>
		[NonSerialized]
		private WeakReference<Component> TargetLastEntered;

		/// <summary>
		/// The last component entered by the {@code SunDropTargetEvent}.
		/// </summary>
		[NonSerialized]
		private WeakReference<Component> TargetLastEnteredDT;

		/// <summary>
		/// Is the mouse over the native container.
		/// </summary>
		[NonSerialized]
		private bool IsMouseInNativeContainer = false;

		/// <summary>
		/// Is DnD over the native container.
		/// </summary>
		[NonSerialized]
		private bool IsMouseDTInNativeContainer = false;

		/// <summary>
		/// This variable is not used, but kept for serialization compatibility
		/// </summary>
		private Cursor NativeCursor;

		/// <summary>
		/// The event mask for contained lightweight components.  Lightweight
		/// components need a windowed container to host window-related
		/// events.  This separate mask indicates events that have been
		/// requested by contained lightweight components without effecting
		/// the mask of the windowed component itself.
		/// </summary>
		private long EventMask;

		/// <summary>
		/// The kind of events routed to lightweight components from windowed
		/// hosts.
		/// </summary>
		private static readonly long PROXY_EVENT_MASK = AWTEvent.FOCUS_EVENT_MASK | AWTEvent.KEY_EVENT_MASK | AWTEvent.MOUSE_EVENT_MASK | AWTEvent.MOUSE_MOTION_EVENT_MASK | AWTEvent.MOUSE_WHEEL_EVENT_MASK;

		private static readonly long MOUSE_MASK = AWTEvent.MOUSE_EVENT_MASK | AWTEvent.MOUSE_MOTION_EVENT_MASK | AWTEvent.MOUSE_WHEEL_EVENT_MASK;
	}

}