using System;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.dnd
{

	using SunDragSourceContextPeer = sun.awt.dnd.SunDragSourceContextPeer;
	using GetIntegerAction = sun.security.action.GetIntegerAction;


	/// <summary>
	/// The <code>DragSource</code> is the entity responsible
	/// for the initiation of the Drag
	/// and Drop operation, and may be used in a number of scenarios:
	/// <UL>
	/// <LI>1 default instance per JVM for the lifetime of that JVM.
	/// <LI>1 instance per class of potential Drag Initiator object (e.g
	/// TextField). [implementation dependent]
	/// <LI>1 per instance of a particular
	/// <code>Component</code>, or application specific
	/// object associated with a <code>Component</code>
	/// instance in the GUI. [implementation dependent]
	/// <LI>Some other arbitrary association. [implementation dependent]
	/// </UL>
	/// 
	/// Once the <code>DragSource</code> is
	/// obtained, a <code>DragGestureRecognizer</code> should
	/// also be obtained to associate the <code>DragSource</code>
	/// with a particular
	/// <code>Component</code>.
	/// <P>
	/// The initial interpretation of the user's gesture,
	/// and the subsequent starting of the drag operation
	/// are the responsibility of the implementing
	/// <code>Component</code>, which is usually
	/// implemented by a <code>DragGestureRecognizer</code>.
	/// <P>
	/// When a drag gesture occurs, the
	/// <code>DragSource</code>'s
	/// startDrag() method shall be
	/// invoked in order to cause processing
	/// of the user's navigational
	/// gestures and delivery of Drag and Drop
	/// protocol notifications. A
	/// <code>DragSource</code> shall only
	/// permit a single Drag and Drop operation to be
	/// current at any one time, and shall
	/// reject any further startDrag() requests
	/// by throwing an <code>IllegalDnDOperationException</code>
	/// until such time as the extant operation is complete.
	/// <P>
	/// The startDrag() method invokes the
	/// createDragSourceContext() method to
	/// instantiate an appropriate
	/// <code>DragSourceContext</code>
	/// and associate the <code>DragSourceContextPeer</code>
	/// with that.
	/// <P>
	/// If the Drag and Drop System is
	/// unable to initiate a drag operation for
	/// some reason, the startDrag() method throws
	/// a <code>java.awt.dnd.InvalidDnDOperationException</code>
	/// to signal such a condition. Typically this
	/// exception is thrown when the underlying platform
	/// system is either not in a state to
	/// initiate a drag, or the parameters specified are invalid.
	/// <P>
	/// Note that during the drag, the
	/// set of operations exposed by the source
	/// at the start of the drag operation may not change
	/// until the operation is complete.
	/// The operation(s) are constant for the
	/// duration of the operation with respect to the
	/// <code>DragSource</code>.
	/// 
	/// @since 1.2
	/// </summary>

	[Serializable]
	public class DragSource
	{

		private const long SerialVersionUID = 6236096958971414066L;

		/*
		 * load a system default cursor
		 */

		private static Cursor Load(String name)
		{
			if (GraphicsEnvironment.Headless)
			{
				return null;
			}

			try
			{
				return (Cursor)Toolkit.DefaultToolkit.GetDesktopProperty(name);
			}
			catch (Exception e)
			{
				e.PrintStackTrace();

				throw new RuntimeException("failed to load system cursor: " + name + " : " + e.Message);
			}
		}


		/// <summary>
		/// The default <code>Cursor</code> to use with a copy operation indicating
		/// that a drop is currently allowed. <code>null</code> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		/// </summary>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		public static readonly Cursor DefaultCopyDrop = Load("DnD.Cursor.CopyDrop");

		/// <summary>
		/// The default <code>Cursor</code> to use with a move operation indicating
		/// that a drop is currently allowed. <code>null</code> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		/// </summary>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		public static readonly Cursor DefaultMoveDrop = Load("DnD.Cursor.MoveDrop");

		/// <summary>
		/// The default <code>Cursor</code> to use with a link operation indicating
		/// that a drop is currently allowed. <code>null</code> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		/// </summary>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		public static readonly Cursor DefaultLinkDrop = Load("DnD.Cursor.LinkDrop");

		/// <summary>
		/// The default <code>Cursor</code> to use with a copy operation indicating
		/// that a drop is currently not allowed. <code>null</code> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		/// </summary>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		public static readonly Cursor DefaultCopyNoDrop = Load("DnD.Cursor.CopyNoDrop");

		/// <summary>
		/// The default <code>Cursor</code> to use with a move operation indicating
		/// that a drop is currently not allowed. <code>null</code> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		/// </summary>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		public static readonly Cursor DefaultMoveNoDrop = Load("DnD.Cursor.MoveNoDrop");

		/// <summary>
		/// The default <code>Cursor</code> to use with a link operation indicating
		/// that a drop is currently not allowed. <code>null</code> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		/// </summary>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		public static readonly Cursor DefaultLinkNoDrop = Load("DnD.Cursor.LinkNoDrop");

		private static readonly DragSource Dflt = (GraphicsEnvironment.Headless) ? null : new DragSource();

		/// <summary>
		/// Internal constants for serialization.
		/// </summary>
		internal const String DragSourceListenerK = "dragSourceL";
		internal const String DragSourceMotionListenerK = "dragSourceMotionL";

		/// <summary>
		/// Gets the <code>DragSource</code> object associated with
		/// the underlying platform.
		/// </summary>
		/// <returns> the platform DragSource </returns>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		public static DragSource DefaultDragSource
		{
			get
			{
				if (GraphicsEnvironment.Headless)
				{
					throw new HeadlessException();
				}
				else
				{
					return Dflt;
				}
			}
		}

		/// <summary>
		/// Reports
		/// whether or not drag
		/// <code>Image</code> support
		/// is available on the underlying platform.
		/// <P> </summary>
		/// <returns> if the Drag Image support is available on this platform </returns>

		public static bool DragImageSupported
		{
			get
			{
				Toolkit t = Toolkit.DefaultToolkit;
    
				Boolean supported;
    
				try
				{
					supported = (Boolean)Toolkit.DefaultToolkit.GetDesktopProperty("DnD.isDragImageSupported");
    
					return supported.BooleanValue();
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Creates a new <code>DragSource</code>.
		/// </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///            returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DragSource() throws java.awt.HeadlessException
		public DragSource()
		{
			if (GraphicsEnvironment.Headless)
			{
				throw new HeadlessException();
			}
		}

		/// <summary>
		/// Start a drag, given the <code>DragGestureEvent</code>
		/// that initiated the drag, the initial
		/// <code>Cursor</code> to use,
		/// the <code>Image</code> to drag,
		/// the offset of the <code>Image</code> origin
		/// from the hotspot of the <code>Cursor</code> at
		/// the instant of the trigger,
		/// the <code>Transferable</code> subject data
		/// of the drag, the <code>DragSourceListener</code>,
		/// and the <code>FlavorMap</code>.
		/// <P> </summary>
		/// <param name="trigger">        the <code>DragGestureEvent</code> that initiated the drag </param>
		/// <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		///                       or {@code null} for the default cursor handling;
		///                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		///                       for more details on the cursor handling mechanism during drag and drop </param>
		/// <param name="dragImage">      the image to drag or {@code null} </param>
		/// <param name="imageOffset">    the offset of the <code>Image</code> origin from the hotspot
		///                       of the <code>Cursor</code> at the instant of the trigger </param>
		/// <param name="transferable">   the subject data of the drag </param>
		/// <param name="dsl">            the <code>DragSourceListener</code> </param>
		/// <param name="flavorMap">      the <code>FlavorMap</code> to use, or <code>null</code>
		/// <P> </param>
		/// <exception cref="java.awt.dnd.InvalidDnDOperationException">
		///    if the Drag and Drop
		///    system is unable to initiate a drag operation, or if the user
		///    attempts to start a drag while an existing drag operation
		///    is still executing </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDrag(DragGestureEvent trigger, java.awt.Cursor dragCursor, java.awt.Image dragImage, java.awt.Point imageOffset, java.awt.datatransfer.Transferable transferable, DragSourceListener dsl, java.awt.datatransfer.FlavorMap flavorMap) throws InvalidDnDOperationException
		public virtual void StartDrag(DragGestureEvent trigger, Cursor dragCursor, Image dragImage, Point imageOffset, Transferable transferable, DragSourceListener dsl, FlavorMap flavorMap)
		{

			SunDragSourceContextPeer.DragDropInProgress = true;

			try
			{
				if (flavorMap != null)
				{
					this.FlavorMap_Renamed = flavorMap;
				}

				DragSourceContextPeer dscp = Toolkit.DefaultToolkit.CreateDragSourceContextPeer(trigger);

				DragSourceContext dsc = CreateDragSourceContext(dscp, trigger, dragCursor, dragImage, imageOffset, transferable, dsl);

				if (dsc == null)
				{
					throw new InvalidDnDOperationException();
				}

				dscp.StartDrag(dsc, dsc.Cursor, dragImage, imageOffset); // may throw
			}
			catch (RuntimeException e)
			{
				SunDragSourceContextPeer.DragDropInProgress = false;
				throw e;
			}
		}

		/// <summary>
		/// Start a drag, given the <code>DragGestureEvent</code>
		/// that initiated the drag, the initial
		/// <code>Cursor</code> to use,
		/// the <code>Transferable</code> subject data
		/// of the drag, the <code>DragSourceListener</code>,
		/// and the <code>FlavorMap</code>.
		/// <P> </summary>
		/// <param name="trigger">        the <code>DragGestureEvent</code> that
		/// initiated the drag </param>
		/// <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		///                       or {@code null} for the default cursor handling;
		///                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		///                       for more details on the cursor handling mechanism during drag and drop </param>
		/// <param name="transferable">   the subject data of the drag </param>
		/// <param name="dsl">            the <code>DragSourceListener</code> </param>
		/// <param name="flavorMap">      the <code>FlavorMap</code> to use or <code>null</code>
		/// <P> </param>
		/// <exception cref="java.awt.dnd.InvalidDnDOperationException">
		///    if the Drag and Drop
		///    system is unable to initiate a drag operation, or if the user
		///    attempts to start a drag while an existing drag operation
		///    is still executing </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDrag(DragGestureEvent trigger, java.awt.Cursor dragCursor, java.awt.datatransfer.Transferable transferable, DragSourceListener dsl, java.awt.datatransfer.FlavorMap flavorMap) throws InvalidDnDOperationException
		public virtual void StartDrag(DragGestureEvent trigger, Cursor dragCursor, Transferable transferable, DragSourceListener dsl, FlavorMap flavorMap)
		{
			StartDrag(trigger, dragCursor, null, null, transferable, dsl, flavorMap);
		}

		/// <summary>
		/// Start a drag, given the <code>DragGestureEvent</code>
		/// that initiated the drag, the initial <code>Cursor</code>
		/// to use,
		/// the <code>Image</code> to drag,
		/// the offset of the <code>Image</code> origin
		/// from the hotspot of the <code>Cursor</code>
		/// at the instant of the trigger,
		/// the subject data of the drag, and
		/// the <code>DragSourceListener</code>.
		/// <P> </summary>
		/// <param name="trigger">           the <code>DragGestureEvent</code> that initiated the drag </param>
		/// <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		///                       or {@code null} for the default cursor handling;
		///                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		///                       for more details on the cursor handling mechanism during drag and drop </param>
		/// <param name="dragImage">         the <code>Image</code> to drag or <code>null</code> </param>
		/// <param name="dragOffset">        the offset of the <code>Image</code> origin from the hotspot
		///                          of the <code>Cursor</code> at the instant of the trigger </param>
		/// <param name="transferable">      the subject data of the drag </param>
		/// <param name="dsl">               the <code>DragSourceListener</code>
		/// <P> </param>
		/// <exception cref="java.awt.dnd.InvalidDnDOperationException">
		///    if the Drag and Drop
		///    system is unable to initiate a drag operation, or if the user
		///    attempts to start a drag while an existing drag operation
		///    is still executing </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDrag(DragGestureEvent trigger, java.awt.Cursor dragCursor, java.awt.Image dragImage, java.awt.Point dragOffset, java.awt.datatransfer.Transferable transferable, DragSourceListener dsl) throws InvalidDnDOperationException
		public virtual void StartDrag(DragGestureEvent trigger, Cursor dragCursor, Image dragImage, Point dragOffset, Transferable transferable, DragSourceListener dsl)
		{
			StartDrag(trigger, dragCursor, dragImage, dragOffset, transferable, dsl, null);
		}

		/// <summary>
		/// Start a drag, given the <code>DragGestureEvent</code>
		/// that initiated the drag, the initial
		/// <code>Cursor</code> to
		/// use,
		/// the <code>Transferable</code> subject data
		/// of the drag, and the <code>DragSourceListener</code>.
		/// <P> </summary>
		/// <param name="trigger">           the <code>DragGestureEvent</code> that initiated the drag </param>
		/// <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		///                       or {@code null} for the default cursor handling;
		///                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a> class
		///                       for more details on the cursor handling mechanism during drag and drop </param>
		/// <param name="transferable">      the subject data of the drag </param>
		/// <param name="dsl">               the <code>DragSourceListener</code>
		/// <P> </param>
		/// <exception cref="java.awt.dnd.InvalidDnDOperationException">
		///    if the Drag and Drop
		///    system is unable to initiate a drag operation, or if the user
		///    attempts to start a drag while an existing drag operation
		///    is still executing </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDrag(DragGestureEvent trigger, java.awt.Cursor dragCursor, java.awt.datatransfer.Transferable transferable, DragSourceListener dsl) throws InvalidDnDOperationException
		public virtual void StartDrag(DragGestureEvent trigger, Cursor dragCursor, Transferable transferable, DragSourceListener dsl)
		{
			StartDrag(trigger, dragCursor, null, null, transferable, dsl, null);
		}

		/// <summary>
		/// Creates the {@code DragSourceContext} to handle the current drag
		/// operation.
		/// <para>
		/// To incorporate a new <code>DragSourceContext</code>
		/// subclass, subclass <code>DragSource</code> and
		/// override this method.
		/// </para>
		/// <para>
		/// If <code>dragImage</code> is <code>null</code>, no image is used
		/// to represent the drag over feedback for this drag operation, but
		/// <code>NullPointerException</code> is not thrown.
		/// </para>
		/// <para>
		/// If <code>dsl</code> is <code>null</code>, no drag source listener
		/// is registered with the created <code>DragSourceContext</code>,
		/// but <code>NullPointerException</code> is not thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dscp">          The <code>DragSourceContextPeer</code> for this drag </param>
		/// <param name="dgl">           The <code>DragGestureEvent</code> that triggered the
		///                      drag </param>
		/// <param name="dragCursor">     The initial {@code Cursor} for this drag operation
		///                       or {@code null} for the default cursor handling;
		///                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a> class
		///                       for more details on the cursor handling mechanism during drag and drop </param>
		/// <param name="dragImage">     The <code>Image</code> to drag or <code>null</code> </param>
		/// <param name="imageOffset">   The offset of the <code>Image</code> origin from the
		///                      hotspot of the cursor at the instant of the trigger </param>
		/// <param name="t">             The subject data of the drag </param>
		/// <param name="dsl">           The <code>DragSourceListener</code>
		/// </param>
		/// <returns> the <code>DragSourceContext</code>
		/// </returns>
		/// <exception cref="NullPointerException"> if <code>dscp</code> is <code>null</code> </exception>
		/// <exception cref="NullPointerException"> if <code>dgl</code> is <code>null</code> </exception>
		/// <exception cref="NullPointerException"> if <code>dragImage</code> is not
		///    <code>null</code> and <code>imageOffset</code> is <code>null</code> </exception>
		/// <exception cref="NullPointerException"> if <code>t</code> is <code>null</code> </exception>
		/// <exception cref="IllegalArgumentException"> if the <code>Component</code>
		///         associated with the trigger event is <code>null</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if the <code>DragSource</code> for the
		///         trigger event is <code>null</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if the drag action for the
		///         trigger event is <code>DnDConstants.ACTION_NONE</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if the source actions for the
		///         <code>DragGestureRecognizer</code> associated with the trigger
		///         event are equal to <code>DnDConstants.ACTION_NONE</code>. </exception>

		protected internal virtual DragSourceContext CreateDragSourceContext(DragSourceContextPeer dscp, DragGestureEvent dgl, Cursor dragCursor, Image dragImage, Point imageOffset, Transferable t, DragSourceListener dsl)
		{
			return new DragSourceContext(dscp, dgl, dragCursor, dragImage, imageOffset, t, dsl);
		}

		/// <summary>
		/// This method returns the
		/// <code>FlavorMap</code> for this <code>DragSource</code>.
		/// <P> </summary>
		/// <returns> the <code>FlavorMap</code> for this <code>DragSource</code> </returns>

		public virtual FlavorMap FlavorMap
		{
			get
			{
				return FlavorMap_Renamed;
			}
		}

		/// <summary>
		/// Creates a new <code>DragGestureRecognizer</code>
		/// that implements the specified
		/// abstract subclass of
		/// <code>DragGestureRecognizer</code>, and
		/// sets the specified <code>Component</code>
		/// and <code>DragGestureListener</code> on
		/// the newly created object.
		/// <P> </summary>
		/// <param name="recognizerAbstractClass"> the requested abstract type </param>
		/// <param name="actions">                 the permitted source drag actions </param>
		/// <param name="c">                       the <code>Component</code> target </param>
		/// <param name="dgl">        the <code>DragGestureListener</code> to notify
		/// <P> </param>
		/// <returns> the new <code>DragGestureRecognizer</code> or <code>null</code>
		///    if the <code>Toolkit.createDragGestureRecognizer</code> method
		///    has no implementation available for
		///    the requested <code>DragGestureRecognizer</code>
		///    subclass and returns <code>null</code> </returns>

		public virtual T createDragGestureRecognizer<T>(Class recognizerAbstractClass, Component c, int actions, DragGestureListener dgl) where T : DragGestureRecognizer
		{
			return Toolkit.DefaultToolkit.CreateDragGestureRecognizer(recognizerAbstractClass, this, c, actions, dgl);
		}


		/// <summary>
		/// Creates a new <code>DragGestureRecognizer</code>
		/// that implements the default
		/// abstract subclass of <code>DragGestureRecognizer</code>
		/// for this <code>DragSource</code>,
		/// and sets the specified <code>Component</code>
		/// and <code>DragGestureListener</code> on the
		/// newly created object.
		/// 
		/// For this <code>DragSource</code>
		/// the default is <code>MouseDragGestureRecognizer</code>.
		/// <P> </summary>
		/// <param name="c">       the <code>Component</code> target for the recognizer </param>
		/// <param name="actions"> the permitted source actions </param>
		/// <param name="dgl">     the <code>DragGestureListener</code> to notify
		/// <P> </param>
		/// <returns> the new <code>DragGestureRecognizer</code> or <code>null</code>
		///    if the <code>Toolkit.createDragGestureRecognizer</code> method
		///    has no implementation available for
		///    the requested <code>DragGestureRecognizer</code>
		///    subclass and returns <code>null</code> </returns>

		public virtual DragGestureRecognizer CreateDefaultDragGestureRecognizer(Component c, int actions, DragGestureListener dgl)
		{
			return Toolkit.DefaultToolkit.CreateDragGestureRecognizer(typeof(MouseDragGestureRecognizer), this, c, actions, dgl);
		}

		/// <summary>
		/// Adds the specified <code>DragSourceListener</code> to this
		/// <code>DragSource</code> to receive drag source events during drag
		/// operations intiated with this <code>DragSource</code>.
		/// If a <code>null</code> listener is specified, no action is taken and no
		/// exception is thrown.
		/// </summary>
		/// <param name="dsl"> the <code>DragSourceListener</code> to add
		/// </param>
		/// <seealso cref=      #removeDragSourceListener </seealso>
		/// <seealso cref=      #getDragSourceListeners
		/// @since 1.4 </seealso>
		public virtual void AddDragSourceListener(DragSourceListener dsl)
		{
			if (dsl != null)
			{
				lock (this)
				{
					Listener = DnDEventMulticaster.Add(Listener, dsl);
				}
			}
		}

		/// <summary>
		/// Removes the specified <code>DragSourceListener</code> from this
		/// <code>DragSource</code>.
		/// If a <code>null</code> listener is specified, no action is taken and no
		/// exception is thrown.
		/// If the listener specified by the argument was not previously added to
		/// this <code>DragSource</code>, no action is taken and no exception
		/// is thrown.
		/// </summary>
		/// <param name="dsl"> the <code>DragSourceListener</code> to remove
		/// </param>
		/// <seealso cref=      #addDragSourceListener </seealso>
		/// <seealso cref=      #getDragSourceListeners
		/// @since 1.4 </seealso>
		public virtual void RemoveDragSourceListener(DragSourceListener dsl)
		{
			if (dsl != null)
			{
				lock (this)
				{
					Listener = DnDEventMulticaster.Remove(Listener, dsl);
				}
			}
		}

		/// <summary>
		/// Gets all the <code>DragSourceListener</code>s
		/// registered with this <code>DragSource</code>.
		/// </summary>
		/// <returns> all of this <code>DragSource</code>'s
		///         <code>DragSourceListener</code>s or an empty array if no
		///         such listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addDragSourceListener </seealso>
		/// <seealso cref=      #removeDragSourceListener
		/// @since    1.4 </seealso>
		public virtual DragSourceListener[] DragSourceListeners
		{
			get
			{
				return GetListeners(typeof(DragSourceListener));
			}
		}

		/// <summary>
		/// Adds the specified <code>DragSourceMotionListener</code> to this
		/// <code>DragSource</code> to receive drag motion events during drag
		/// operations intiated with this <code>DragSource</code>.
		/// If a <code>null</code> listener is specified, no action is taken and no
		/// exception is thrown.
		/// </summary>
		/// <param name="dsml"> the <code>DragSourceMotionListener</code> to add
		/// </param>
		/// <seealso cref=      #removeDragSourceMotionListener </seealso>
		/// <seealso cref=      #getDragSourceMotionListeners
		/// @since 1.4 </seealso>
		public virtual void AddDragSourceMotionListener(DragSourceMotionListener dsml)
		{
			if (dsml != null)
			{
				lock (this)
				{
					MotionListener = DnDEventMulticaster.Add(MotionListener, dsml);
				}
			}
		}

		/// <summary>
		/// Removes the specified <code>DragSourceMotionListener</code> from this
		/// <code>DragSource</code>.
		/// If a <code>null</code> listener is specified, no action is taken and no
		/// exception is thrown.
		/// If the listener specified by the argument was not previously added to
		/// this <code>DragSource</code>, no action is taken and no exception
		/// is thrown.
		/// </summary>
		/// <param name="dsml"> the <code>DragSourceMotionListener</code> to remove
		/// </param>
		/// <seealso cref=      #addDragSourceMotionListener </seealso>
		/// <seealso cref=      #getDragSourceMotionListeners
		/// @since 1.4 </seealso>
		public virtual void RemoveDragSourceMotionListener(DragSourceMotionListener dsml)
		{
			if (dsml != null)
			{
				lock (this)
				{
					MotionListener = DnDEventMulticaster.Remove(MotionListener, dsml);
				}
			}
		}

		/// <summary>
		/// Gets all of the  <code>DragSourceMotionListener</code>s
		/// registered with this <code>DragSource</code>.
		/// </summary>
		/// <returns> all of this <code>DragSource</code>'s
		///         <code>DragSourceMotionListener</code>s or an empty array if no
		///         such listeners are currently registered
		/// </returns>
		/// <seealso cref=      #addDragSourceMotionListener </seealso>
		/// <seealso cref=      #removeDragSourceMotionListener
		/// @since    1.4 </seealso>
		public virtual DragSourceMotionListener[] DragSourceMotionListeners
		{
			get
			{
				return GetListeners(typeof(DragSourceMotionListener));
			}
		}

		/// <summary>
		/// Gets all the objects currently registered as
		/// <code><em>Foo</em>Listener</code>s upon this <code>DragSource</code>.
		/// <code><em>Foo</em>Listener</code>s are registered using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// </summary>
		/// <param name="listenerType"> the type of listeners requested; this parameter
		///          should specify an interface that descends from
		///          <code>java.util.EventListener</code> </param>
		/// <returns> an array of all objects registered as
		///          <code><em>Foo</em>Listener</code>s on this
		///          <code>DragSource</code>, or an empty array if no such listeners
		///          have been added </returns>
		/// <exception cref="ClassCastException"> if <code>listenerType</code>
		///          doesn't specify a class or interface that implements
		///          <code>java.util.EventListener</code>
		/// </exception>
		/// <seealso cref= #getDragSourceListeners </seealso>
		/// <seealso cref= #getDragSourceMotionListeners
		/// @since 1.4 </seealso>
		public virtual T[] getListeners<T>(Class listenerType) where T : java.util.EventListener
		{
			EventListener l = null;
			if (listenerType == typeof(DragSourceListener))
			{
				l = Listener;
			}
			else if (listenerType == typeof(DragSourceMotionListener))
			{
				l = MotionListener;
			}
			return DnDEventMulticaster.GetListeners(l, listenerType);
		}

		/// <summary>
		/// This method calls <code>dragEnter</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceDragEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		internal virtual void ProcessDragEnter(DragSourceDragEvent dsde)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DragEnter(dsde);
			}
		}

		/// <summary>
		/// This method calls <code>dragOver</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceDragEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		internal virtual void ProcessDragOver(DragSourceDragEvent dsde)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DragOver(dsde);
			}
		}

		/// <summary>
		/// This method calls <code>dropActionChanged</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceDragEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		internal virtual void ProcessDropActionChanged(DragSourceDragEvent dsde)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DropActionChanged(dsde);
			}
		}

		/// <summary>
		/// This method calls <code>dragExit</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceEvent</code>.
		/// </summary>
		/// <param name="dse"> the <code>DragSourceEvent</code> </param>
		internal virtual void ProcessDragExit(DragSourceEvent dse)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DragExit(dse);
			}
		}

		/// <summary>
		/// This method calls <code>dragDropEnd</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceDropEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceEvent</code> </param>
		internal virtual void ProcessDragDropEnd(DragSourceDropEvent dsde)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DragDropEnd(dsde);
			}
		}

		/// <summary>
		/// This method calls <code>dragMouseMoved</code> on the
		/// <code>DragSourceMotionListener</code>s registered with this
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceDragEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceEvent</code> </param>
		internal virtual void ProcessDragMouseMoved(DragSourceDragEvent dsde)
		{
			DragSourceMotionListener dsml = MotionListener;
			if (dsml != null)
			{
				dsml.DragMouseMoved(dsde);
			}
		}

		/// <summary>
		/// Serializes this <code>DragSource</code>. This method first performs
		/// default serialization. Next, it writes out this object's
		/// <code>FlavorMap</code> if and only if it can be serialized. If not,
		/// <code>null</code> is written instead. Next, it writes out
		/// <code>Serializable</code> listeners registered with this
		/// object. Listeners are written in a <code>null</code>-terminated sequence
		/// of 0 or more pairs. The pair consists of a <code>String</code> and an
		/// <code>Object</code>; the <code>String</code> indicates the type of the
		/// <code>Object</code> and is one of the following:
		/// <ul>
		/// <li><code>dragSourceListenerK</code> indicating a
		///     <code>DragSourceListener</code> object;
		/// <li><code>dragSourceMotionListenerK</code> indicating a
		///     <code>DragSourceMotionListener</code> object.
		/// </ul>
		/// 
		/// @serialData Either a <code>FlavorMap</code> instance, or
		///      <code>null</code>, followed by a <code>null</code>-terminated
		///      sequence of 0 or more pairs; the pair consists of a
		///      <code>String</code> and an <code>Object</code>; the
		///      <code>String</code> indicates the type of the <code>Object</code>
		///      and is one of the following:
		///      <ul>
		///      <li><code>dragSourceListenerK</code> indicating a
		///          <code>DragSourceListener</code> object;
		///      <li><code>dragSourceMotionListenerK</code> indicating a
		///          <code>DragSourceMotionListener</code> object.
		///      </ul>.
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			s.DefaultWriteObject();

			s.WriteObject(SerializationTester.Test(FlavorMap_Renamed) ? FlavorMap_Renamed : null);

			DnDEventMulticaster.Save(s, DragSourceListenerK, Listener);
			DnDEventMulticaster.Save(s, DragSourceMotionListenerK, MotionListener);
			s.WriteObject(null);
		}

		/// <summary>
		/// Deserializes this <code>DragSource</code>. This method first performs
		/// default deserialization. Next, this object's <code>FlavorMap</code> is
		/// deserialized by using the next object in the stream.
		/// If the resulting <code>FlavorMap</code> is <code>null</code>, this
		/// object's <code>FlavorMap</code> is set to the default FlavorMap for
		/// this thread's <code>ClassLoader</code>.
		/// Next, this object's listeners are deserialized by reading a
		/// <code>null</code>-terminated sequence of 0 or more key/value pairs
		/// from the stream:
		/// <ul>
		/// <li>If a key object is a <code>String</code> equal to
		/// <code>dragSourceListenerK</code>, a <code>DragSourceListener</code> is
		/// deserialized using the corresponding value object and added to this
		/// <code>DragSource</code>.
		/// <li>If a key object is a <code>String</code> equal to
		/// <code>dragSourceMotionListenerK</code>, a
		/// <code>DragSourceMotionListener</code> is deserialized using the
		/// corresponding value object and added to this <code>DragSource</code>.
		/// <li>Otherwise, the key/value pair is skipped.
		/// </ul>
		/// </summary>
		/// <seealso cref= java.awt.datatransfer.SystemFlavorMap#getDefaultFlavorMap
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream s)
		{
			s.DefaultReadObject();

			// 'flavorMap' was written explicitly
			FlavorMap_Renamed = (FlavorMap)s.ReadObject();

			// Implementation assumes 'flavorMap' is never null.
			if (FlavorMap_Renamed == null)
			{
				FlavorMap_Renamed = SystemFlavorMap.DefaultFlavorMap;
			}

			Object keyOrNull;
			while (null != (keyOrNull = s.ReadObject()))
			{
				String key = ((String)keyOrNull).intern();

				if (DragSourceListenerK == key)
				{
					AddDragSourceListener((DragSourceListener)(s.ReadObject()));
				}
				else if (DragSourceMotionListenerK == key)
				{
					AddDragSourceMotionListener((DragSourceMotionListener)(s.ReadObject()));
				}
				else
				{
					// skip value for unrecognized key
					s.ReadObject();
				}
			}
		}

		/// <summary>
		/// Returns the drag gesture motion threshold. The drag gesture motion threshold
		/// defines the recommended behavior for <seealso cref="MouseDragGestureRecognizer"/>s.
		/// <para>
		/// If the system property <code>awt.dnd.drag.threshold</code> is set to
		/// a positive integer, this method returns the value of the system property;
		/// otherwise if a pertinent desktop property is available and supported by
		/// the implementation of the Java platform, this method returns the value of
		/// that property; otherwise this method returns some default value.
		/// The pertinent desktop property can be queried using
		/// <code>java.awt.Toolkit.getDesktopProperty("DnD.gestureMotionThreshold")</code>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the drag gesture motion threshold </returns>
		/// <seealso cref= MouseDragGestureRecognizer
		/// @since 1.5 </seealso>
		public static int DragThreshold
		{
			get
			{
				int ts = (int)AccessController.doPrivileged(new GetIntegerAction("awt.dnd.drag.threshold", 0));
				if (ts > 0)
				{
					return ts;
				}
				else
				{
					Integer td = (Integer)Toolkit.DefaultToolkit.GetDesktopProperty("DnD.gestureMotionThreshold");
					if (td != null)
					{
						return td.IntValue();
					}
				}
				return 5;
			}
		}

		/*
		 * fields
		 */

		[NonSerialized]
		private FlavorMap FlavorMap_Renamed = SystemFlavorMap.DefaultFlavorMap;

		[NonSerialized]
		private DragSourceListener Listener;

		[NonSerialized]
		private DragSourceMotionListener MotionListener;
	}

}