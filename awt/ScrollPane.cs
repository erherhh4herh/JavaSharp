using System;
using System.Runtime.InteropServices;

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

	using ScrollPaneWheelScroller = sun.awt.ScrollPaneWheelScroller;
	using SunToolkit = sun.awt.SunToolkit;


	/// <summary>
	/// A container class which implements automatic horizontal and/or
	/// vertical scrolling for a single child component.  The display
	/// policy for the scrollbars can be set to:
	/// <OL>
	/// <LI>as needed: scrollbars created and shown only when needed by scrollpane
	/// <LI>always: scrollbars created and always shown by the scrollpane
	/// <LI>never: scrollbars never created or shown by the scrollpane
	/// </OL>
	/// <P>
	/// The state of the horizontal and vertical scrollbars is represented
	/// by two <code>ScrollPaneAdjustable</code> objects (one for each
	/// dimension) which implement the <code>Adjustable</code> interface.
	/// The API provides methods to access those objects such that the
	/// attributes on the Adjustable object (such as unitIncrement, value,
	/// etc.) can be manipulated.
	/// <P>
	/// Certain adjustable properties (minimum, maximum, blockIncrement,
	/// and visibleAmount) are set internally by the scrollpane in accordance
	/// with the geometry of the scrollpane and its child and these should
	/// not be set by programs using the scrollpane.
	/// <P>
	/// If the scrollbar display policy is defined as "never", then the
	/// scrollpane can still be programmatically scrolled using the
	/// setScrollPosition() method and the scrollpane will move and clip
	/// the child's contents appropriately.  This policy is useful if the
	/// program needs to create and manage its own adjustable controls.
	/// <P>
	/// The placement of the scrollbars is controlled by platform-specific
	/// properties set by the user outside of the program.
	/// <P>
	/// The initial size of this container is set to 100x100, but can
	/// be reset using setSize().
	/// <P>
	/// Scrolling with the wheel on a wheel-equipped mouse is enabled by default.
	/// This can be disabled using <code>setWheelScrollingEnabled</code>.
	/// Wheel scrolling can be customized by setting the block and
	/// unit increment of the horizontal and vertical Adjustables.
	/// For information on how mouse wheel events are dispatched, see
	/// the class description for <seealso cref="MouseWheelEvent"/>.
	/// <P>
	/// Insets are used to define any space used by scrollbars and any
	/// borders created by the scroll pane. getInsets() can be used
	/// to get the current value for the insets.  If the value of
	/// scrollbarsAlwaysVisible is false, then the value of the insets
	/// will change dynamically depending on whether the scrollbars are
	/// currently visible or not.
	/// 
	/// @author      Tom Ball
	/// @author      Amy Fowler
	/// @author      Tim Prinzing
	/// </summary>
	public class ScrollPane : Container, Accessible
	{


		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		static ScrollPane()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// Specifies that horizontal/vertical scrollbar should be shown
		/// only when the size of the child exceeds the size of the scrollpane
		/// in the horizontal/vertical dimension.
		/// </summary>
		public const int SCROLLBARS_AS_NEEDED = 0;

		/// <summary>
		/// Specifies that horizontal/vertical scrollbars should always be
		/// shown regardless of the respective sizes of the scrollpane and child.
		/// </summary>
		public const int SCROLLBARS_ALWAYS = 1;

		/// <summary>
		/// Specifies that horizontal/vertical scrollbars should never be shown
		/// regardless of the respective sizes of the scrollpane and child.
		/// </summary>
		public const int SCROLLBARS_NEVER = 2;

		/// <summary>
		/// There are 3 ways in which a scroll bar can be displayed.
		/// This integer will represent one of these 3 displays -
		/// (SCROLLBARS_ALWAYS, SCROLLBARS_AS_NEEDED, SCROLLBARS_NEVER)
		/// 
		/// @serial </summary>
		/// <seealso cref= #getScrollbarDisplayPolicy </seealso>
		private int ScrollbarDisplayPolicy_Renamed;

		/// <summary>
		/// An adjustable vertical scrollbar.
		/// It is important to note that you must <em>NOT</em> call 3
		/// <code>Adjustable</code> methods, namely:
		/// <code>setMinimum()</code>, <code>setMaximum()</code>,
		/// <code>setVisibleAmount()</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getVAdjustable </seealso>
		private ScrollPaneAdjustable VAdjustable_Renamed;

		/// <summary>
		/// An adjustable horizontal scrollbar.
		/// It is important to note that you must <em>NOT</em> call 3
		/// <code>Adjustable</code> methods, namely:
		/// <code>setMinimum()</code>, <code>setMaximum()</code>,
		/// <code>setVisibleAmount()</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getHAdjustable </seealso>
		private ScrollPaneAdjustable HAdjustable_Renamed;

		private const String @base = "scrollpane";
		private static int NameCounter = 0;

		private const bool DefaultWheelScroll = true;

		/// <summary>
		/// Indicates whether or not scrolling should take place when a
		/// MouseWheelEvent is received.
		/// 
		/// @serial
		/// @since 1.4
		/// </summary>
		private bool WheelScrollingEnabled_Renamed = DefaultWheelScroll;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 7956609840827222915L;

		/// <summary>
		/// Create a new scrollpane container with a scrollbar display
		/// policy of "as needed". </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///     returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ScrollPane() throws HeadlessException
		public ScrollPane() : this(SCROLLBARS_AS_NEEDED)
		{
		}

		/// <summary>
		/// Create a new scrollpane container. </summary>
		/// <param name="scrollbarDisplayPolicy"> policy for when scrollbars should be shown </param>
		/// <exception cref="IllegalArgumentException"> if the specified scrollbar
		///     display policy is invalid </exception>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		///     returns true </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({"scrollbarDisplayPolicy"}) public ScrollPane(int scrollbarDisplayPolicy) throws HeadlessException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public ScrollPane(int scrollbarDisplayPolicy)
		{
			GraphicsEnvironment.CheckHeadless();
			this.LayoutMgr = null;
			this.Width_Renamed = 100;
			this.Height_Renamed = 100;
			switch (scrollbarDisplayPolicy)
			{
				case SCROLLBARS_NEVER:
				case SCROLLBARS_AS_NEEDED:
				case SCROLLBARS_ALWAYS:
					this.ScrollbarDisplayPolicy_Renamed = scrollbarDisplayPolicy;
					break;
				default:
					throw new IllegalArgumentException("illegal scrollbar display policy");
			}

			VAdjustable_Renamed = new ScrollPaneAdjustable(this, new PeerFixer(this, this), Adjustable_Fields.VERTICAL);
			HAdjustable_Renamed = new ScrollPaneAdjustable(this, new PeerFixer(this, this), Adjustable_Fields.HORIZONTAL);
			WheelScrollingEnabled = DefaultWheelScroll;
		}

		/// <summary>
		/// Construct a name for this component.  Called by getName() when the
		/// name is null.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(ScrollPane))
			{
				return @base + NameCounter++;
			}
		}

		// The scrollpane won't work with a windowless child... it assumes
		// it is moving a child window around so the windowless child is
		// wrapped with a window.
		private void AddToPanel(Component comp, Object constraints, int index)
		{
			Panel child = new Panel();
			child.Layout = new BorderLayout();
			child.Add(comp);
			base.AddImpl(child, constraints, index);
			Validate();
		}

		/// <summary>
		/// Adds the specified component to this scroll pane container.
		/// If the scroll pane has an existing child component, that
		/// component is removed and the new one is added. </summary>
		/// <param name="comp"> the component to be added </param>
		/// <param name="constraints">  not applicable </param>
		/// <param name="index"> position of child component (must be &lt;= 0) </param>
		protected internal sealed override void AddImpl(Component comp, Object constraints, int index)
		{
			lock (TreeLock)
			{
				if (ComponentCount > 0)
				{
					Remove(0);
				}
				if (index > 0)
				{
					throw new IllegalArgumentException("position greater than 0");
				}

				if (!SunToolkit.isLightweightOrUnknown(comp))
				{
					base.AddImpl(comp, constraints, index);
				}
				else
				{
					AddToPanel(comp, constraints, index);
				}
			}
		}

		/// <summary>
		/// Returns the display policy for the scrollbars. </summary>
		/// <returns> the display policy for the scrollbars </returns>
		public virtual int ScrollbarDisplayPolicy
		{
			get
			{
				return ScrollbarDisplayPolicy_Renamed;
			}
		}

		/// <summary>
		/// Returns the current size of the scroll pane's view port. </summary>
		/// <returns> the size of the view port in pixels </returns>
		public virtual Dimension ViewportSize
		{
			get
			{
				Insets i = Insets;
				return new Dimension(Width_Renamed - i.Right - i.Left, Height_Renamed - i.Top - i.Bottom);
			}
		}

		/// <summary>
		/// Returns the height that would be occupied by a horizontal
		/// scrollbar, which is independent of whether it is currently
		/// displayed by the scroll pane or not. </summary>
		/// <returns> the height of a horizontal scrollbar in pixels </returns>
		public virtual int HScrollbarHeight
		{
			get
			{
				int h = 0;
				if (ScrollbarDisplayPolicy_Renamed != SCROLLBARS_NEVER)
				{
					ScrollPanePeer peer = (ScrollPanePeer)this.Peer_Renamed;
					if (peer != null)
					{
						h = peer.HScrollbarHeight;
					}
				}
				return h;
			}
		}

		/// <summary>
		/// Returns the width that would be occupied by a vertical
		/// scrollbar, which is independent of whether it is currently
		/// displayed by the scroll pane or not. </summary>
		/// <returns> the width of a vertical scrollbar in pixels </returns>
		public virtual int VScrollbarWidth
		{
			get
			{
				int w = 0;
				if (ScrollbarDisplayPolicy_Renamed != SCROLLBARS_NEVER)
				{
					ScrollPanePeer peer = (ScrollPanePeer)this.Peer_Renamed;
					if (peer != null)
					{
						w = peer.VScrollbarWidth;
					}
				}
				return w;
			}
		}

		/// <summary>
		/// Returns the <code>ScrollPaneAdjustable</code> object which
		/// represents the state of the vertical scrollbar.
		/// The declared return type of this method is
		/// <code>Adjustable</code> to maintain backward compatibility. </summary>
		/// <seealso cref= java.awt.ScrollPaneAdjustable </seealso>
		public virtual Adjustable VAdjustable
		{
			get
			{
				return VAdjustable_Renamed;
			}
		}

		/// <summary>
		/// Returns the <code>ScrollPaneAdjustable</code> object which
		/// represents the state of the horizontal scrollbar.
		/// The declared return type of this method is
		/// <code>Adjustable</code> to maintain backward compatibility. </summary>
		/// <seealso cref= java.awt.ScrollPaneAdjustable </seealso>
		public virtual Adjustable HAdjustable
		{
			get
			{
				return HAdjustable_Renamed;
			}
		}

		/// <summary>
		/// Scrolls to the specified position within the child component.
		/// A call to this method is only valid if the scroll pane contains
		/// a child.  Specifying a position outside of the legal scrolling bounds
		/// of the child will scroll to the closest legal position.
		/// Legal bounds are defined to be the rectangle:
		/// x = 0, y = 0, width = (child width - view port width),
		/// height = (child height - view port height).
		/// This is a convenience method which interfaces with the Adjustable
		/// objects which represent the state of the scrollbars. </summary>
		/// <param name="x"> the x position to scroll to </param>
		/// <param name="y"> the y position to scroll to </param>
		/// <exception cref="NullPointerException"> if the scrollpane does not contain
		///     a child </exception>
		public virtual void SetScrollPosition(int x, int y)
		{
			lock (TreeLock)
			{
				if (ComponentCount == 0)
				{
					throw new NullPointerException("child is null");
				}
				HAdjustable_Renamed.Value = x;
				VAdjustable_Renamed.Value = y;
			}
		}

		/// <summary>
		/// Scrolls to the specified position within the child component.
		/// A call to this method is only valid if the scroll pane contains
		/// a child and the specified position is within legal scrolling bounds
		/// of the child.  Specifying a position outside of the legal scrolling
		/// bounds of the child will scroll to the closest legal position.
		/// Legal bounds are defined to be the rectangle:
		/// x = 0, y = 0, width = (child width - view port width),
		/// height = (child height - view port height).
		/// This is a convenience method which interfaces with the Adjustable
		/// objects which represent the state of the scrollbars. </summary>
		/// <param name="p"> the Point representing the position to scroll to </param>
		/// <exception cref="NullPointerException"> if {@code p} is {@code null} </exception>
		public virtual Point ScrollPosition
		{
			set
			{
				SetScrollPosition(value.x, value.y);
			}
			get
			{
				lock (TreeLock)
				{
					if (ComponentCount == 0)
					{
						throw new NullPointerException("child is null");
					}
					return new Point(HAdjustable_Renamed.Value, VAdjustable_Renamed.Value);
				}
			}
		}


		/// <summary>
		/// Sets the layout manager for this container.  This method is
		/// overridden to prevent the layout mgr from being set. </summary>
		/// <param name="mgr"> the specified layout manager </param>
		public sealed override LayoutManager Layout
		{
			set
			{
				throw new AWTError("ScrollPane controls layout");
			}
		}

		/// <summary>
		/// Lays out this container by resizing its child to its preferred size.
		/// If the new preferred size of the child causes the current scroll
		/// position to be invalid, the scroll position is set to the closest
		/// valid position.
		/// </summary>
		/// <seealso cref= Component#validate </seealso>
		public override void DoLayout()
		{
			Layout();
		}

		/// <summary>
		/// Determine the size to allocate the child component.
		/// If the viewport area is bigger than the preferred size
		/// of the child then the child is allocated enough
		/// to fill the viewport, otherwise the child is given
		/// it's preferred size.
		/// </summary>
		internal virtual Dimension CalculateChildSize()
		{
			//
			// calculate the view size, accounting for border but not scrollbars
			// - don't use right/bottom insets since they vary depending
			//   on whether or not scrollbars were displayed on last resize
			//
			Dimension size = Size;
			Insets insets = Insets;
			int viewWidth = size.Width_Renamed - insets.Left * 2;
			int viewHeight = size.Height_Renamed - insets.Top * 2;

			//
			// determine whether or not horz or vert scrollbars will be displayed
			//
			bool vbarOn;
			bool hbarOn;
			Component child = GetComponent(0);
			Dimension childSize = new Dimension(child.PreferredSize);

			if (ScrollbarDisplayPolicy_Renamed == SCROLLBARS_AS_NEEDED)
			{
				vbarOn = childSize.Height_Renamed > viewHeight;
				hbarOn = childSize.Width_Renamed > viewWidth;
			}
			else if (ScrollbarDisplayPolicy_Renamed == SCROLLBARS_ALWAYS)
			{
				vbarOn = hbarOn = true;
			} // SCROLLBARS_NEVER
			else
			{
				vbarOn = hbarOn = false;
			}

			//
			// adjust predicted view size to account for scrollbars
			//
			int vbarWidth = VScrollbarWidth;
			int hbarHeight = HScrollbarHeight;
			if (vbarOn)
			{
				viewWidth -= vbarWidth;
			}
			if (hbarOn)
			{
				viewHeight -= hbarHeight;
			}

			//
			// if child is smaller than view, size it up
			//
			if (childSize.Width_Renamed < viewWidth)
			{
				childSize.Width_Renamed = viewWidth;
			}
			if (childSize.Height_Renamed < viewHeight)
			{
				childSize.Height_Renamed = viewHeight;
			}

			return childSize;
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>doLayout()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public override void Layout()
		{
			if (ComponentCount == 0)
			{
				return;
			}
			Component c = GetComponent(0);
			Point p = ScrollPosition;
			Dimension cs = CalculateChildSize();
			Dimension vs = ViewportSize;

			c.Reshape(- p.x, - p.y, cs.Width_Renamed, cs.Height_Renamed);
			ScrollPanePeer peer = (ScrollPanePeer)this.Peer_Renamed;
			if (peer != null)
			{
				peer.ChildResized(cs.Width_Renamed, cs.Height_Renamed);
			}

			// update adjustables... the viewport size may have changed
			// with the scrollbars coming or going so the viewport size
			// is updated before the adjustables.
			vs = ViewportSize;
			HAdjustable_Renamed.SetSpan(0, cs.Width_Renamed, vs.Width_Renamed);
			VAdjustable_Renamed.SetSpan(0, cs.Height_Renamed, vs.Height_Renamed);
		}

		/// <summary>
		/// Prints the component in this scroll pane. </summary>
		/// <param name="g"> the specified Graphics window </param>
		/// <seealso cref= Component#print </seealso>
		/// <seealso cref= Component#printAll </seealso>
		public override void PrintComponents(Graphics g)
		{
			if (ComponentCount == 0)
			{
				return;
			}
			Component c = GetComponent(0);
			Point p = c.Location;
			Dimension vs = ViewportSize;
			Insets i = Insets;

			Graphics cg = g.Create();
			try
			{
				cg.ClipRect(i.Left, i.Top, vs.Width_Renamed, vs.Height_Renamed);
				cg.Translate(p.x, p.y);
				c.PrintAll(cg);
			}
			finally
			{
				cg.Dispose();
			}
		}

		/// <summary>
		/// Creates the scroll pane's peer.
		/// </summary>
		public override void AddNotify()
		{
			lock (TreeLock)
			{

				int vAdjustableValue = 0;
				int hAdjustableValue = 0;

				// Bug 4124460. Save the current adjustable values,
				// so they can be restored after addnotify. Set the
				// adjustables to 0, to prevent crashes for possible
				// negative values.
				if (ComponentCount > 0)
				{
					vAdjustableValue = VAdjustable_Renamed.Value;
					hAdjustableValue = HAdjustable_Renamed.Value;
					VAdjustable_Renamed.Value = 0;
					HAdjustable_Renamed.Value = 0;
				}

				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateScrollPane(this);
				}
				base.AddNotify();

				// Bug 4124460. Restore the adjustable values.
				if (ComponentCount > 0)
				{
					VAdjustable_Renamed.Value = vAdjustableValue;
					HAdjustable_Renamed.Value = hAdjustableValue;
				}
			}
		}

		/// <summary>
		/// Returns a string representing the state of this
		/// <code>ScrollPane</code>. This
		/// method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns> the parameter string of this scroll pane </returns>
		public override String ParamString()
		{
			String sdpStr;
			switch (ScrollbarDisplayPolicy_Renamed)
			{
				case SCROLLBARS_AS_NEEDED:
					sdpStr = "as-needed";
					break;
				case SCROLLBARS_ALWAYS:
					sdpStr = "always";
					break;
				case SCROLLBARS_NEVER:
					sdpStr = "never";
					break;
				default:
					sdpStr = "invalid display policy";
				break;
			}
			Point p = (ComponentCount > 0)? ScrollPosition : new Point(0,0);
			Insets i = Insets;
			return base.ParamString() + ",ScrollPosition=(" + p.x + "," + p.y + ")" + ",Insets=(" + i.Top + "," + i.Left + "," + i.Bottom + "," + i.Right + ")" + ",ScrollbarDisplayPolicy=" + sdpStr + ",wheelScrollingEnabled=" + WheelScrollingEnabled;
		}

		internal override void AutoProcessMouseWheel(MouseWheelEvent e)
		{
			ProcessMouseWheelEvent(e);
		}

		/// <summary>
		/// Process mouse wheel events that are delivered to this
		/// <code>ScrollPane</code> by scrolling an appropriate amount.
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e">  the mouse wheel event
		/// @since 1.4 </param>
		protected internal override void ProcessMouseWheelEvent(MouseWheelEvent e)
		{
			if (WheelScrollingEnabled)
			{
				ScrollPaneWheelScroller.handleWheelScrolling(this, e);
				e.Consume();
			}
			base.ProcessMouseWheelEvent(e);
		}

		/// <summary>
		/// If wheel scrolling is enabled, we return true for MouseWheelEvents
		/// @since 1.4
		/// </summary>
		protected internal override bool EventTypeEnabled(int type)
		{
			if (type == MouseEvent.MOUSE_WHEEL && WheelScrollingEnabled)
			{
				return true;
			}
			else
			{
				return base.EventTypeEnabled(type);
			}
		}

		/// <summary>
		/// Enables/disables scrolling in response to movement of the mouse wheel.
		/// Wheel scrolling is enabled by default.
		/// </summary>
		/// <param name="handleWheel">   <code>true</code> if scrolling should be done
		///                      automatically for a MouseWheelEvent,
		///                      <code>false</code> otherwise. </param>
		/// <seealso cref= #isWheelScrollingEnabled </seealso>
		/// <seealso cref= java.awt.event.MouseWheelEvent </seealso>
		/// <seealso cref= java.awt.event.MouseWheelListener
		/// @since 1.4 </seealso>
		public virtual bool WheelScrollingEnabled
		{
			set
			{
				WheelScrollingEnabled_Renamed = value;
			}
			get
			{
				return WheelScrollingEnabled_Renamed;
			}
		}



		/// <summary>
		/// Writes default serializable fields to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			// 4352819: We only need this degenerate writeObject to make
			// it safe for future versions of this class to write optional
			// data to the stream.
			s.DefaultWriteObject();
		}

		/// <summary>
		/// Reads default serializable fields to stream. </summary>
		/// <exception cref="HeadlessException"> if
		/// <code>GraphicsEnvironment.isHeadless()</code> returns
		/// <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
			GraphicsEnvironment.CheckHeadless();
			// 4352819: Gotcha!  Cannot use s.defaultReadObject here and
			// then continue with reading optional data.  Use GetField instead.
			ObjectInputStream.GetField f = s.ReadFields();

			// Old fields
			ScrollbarDisplayPolicy_Renamed = f.Get("scrollbarDisplayPolicy", SCROLLBARS_AS_NEEDED);
			HAdjustable_Renamed = (ScrollPaneAdjustable)f.Get("hAdjustable", null);
			VAdjustable_Renamed = (ScrollPaneAdjustable)f.Get("vAdjustable", null);

			// Since 1.4
			WheelScrollingEnabled_Renamed = f.Get("wheelScrollingEnabled", DefaultWheelScroll);

	//      // Note to future maintainers
	//      if (f.defaulted("wheelScrollingEnabled")) {
	//          // We are reading pre-1.4 stream that doesn't have
	//          // optional data, not even the TC_ENDBLOCKDATA marker.
	//          // Reading anything after this point is unsafe as we will
	//          // read unrelated objects further down the stream (4352819).
	//      }
	//      else {
	//          // Reading data from 1.4 or later, it's ok to try to read
	//          // optional data as OptionalDataException with eof == true
	//          // will be correctly reported
	//      }
		}

		[Serializable]
		internal class PeerFixer : AdjustmentListener
		{
			private readonly ScrollPane OuterInstance;

			internal const long SerialVersionUID = 1043664721353696630L;

			internal PeerFixer(ScrollPane outerInstance, ScrollPane scroller)
			{
				this.OuterInstance = outerInstance;
				this.Scroller = scroller;
			}

			/// <summary>
			/// Invoked when the value of the adjustable has changed.
			/// </summary>
			public virtual void AdjustmentValueChanged(AdjustmentEvent e)
			{
				Adjustable adj = e.Adjustable;
				int value = e.Value;
				ScrollPanePeer peer = (ScrollPanePeer) Scroller.Peer_Renamed;
				if (peer != null)
				{
					peer.SetValue(adj, value);
				}

				Component c = Scroller.GetComponent(0);
				switch (adj.Orientation)
				{
				case Adjustable_Fields.VERTICAL:
					c.Move(c.Location.x, -(value));
					break;
				case Adjustable_Fields.HORIZONTAL:
					c.Move(-(value), c.Location.y);
					break;
				default:
					throw new IllegalArgumentException("Illegal adjustable orientation");
				}
			}

			internal ScrollPane Scroller;
		}


	/////////////////
	// Accessibility support
	////////////////

		/// <summary>
		/// Gets the AccessibleContext associated with this ScrollPane.
		/// For scroll panes, the AccessibleContext takes the form of an
		/// AccessibleAWTScrollPane.
		/// A new AccessibleAWTScrollPane instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTScrollPane that serves as the
		///         AccessibleContext of this ScrollPane
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTScrollPane(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>ScrollPane</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to scroll pane user-interface
		/// elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTScrollPane : AccessibleAWTContainer
		{
			private readonly ScrollPane OuterInstance;

			public AccessibleAWTScrollPane(ScrollPane outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = 6100703663886637L;

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
					return AccessibleRole.SCROLL_PANE;
				}
			}

		} // class AccessibleAWTScrollPane

	}

	/*
	 * In JDK 1.1.1, the pkg private class java.awt.PeerFixer was moved to
	 * become an inner class of ScrollPane, which broke serialization
	 * for ScrollPane objects using JDK 1.1.
	 * Instead of moving it back out here, which would break all JDK 1.1.x
	 * releases, we keep PeerFixer in both places. Because of the scoping rules,
	 * the PeerFixer that is used in ScrollPane will be the one that is the
	 * inner class. This pkg private PeerFixer class below will only be used
	 * if the Java 2 platform is used to deserialize ScrollPane objects that were serialized
	 * using JDK1.1
	 */
	[Serializable]
	internal class PeerFixer : AdjustmentListener
	{
		/*
		 * serialVersionUID
		 */
		private const long SerialVersionUID = 7051237413532574756L;

		internal PeerFixer(ScrollPane scroller)
		{
			this.Scroller = scroller;
		}

		/// <summary>
		/// Invoked when the value of the adjustable has changed.
		/// </summary>
		public virtual void AdjustmentValueChanged(AdjustmentEvent e)
		{
			Adjustable adj = e.Adjustable;
			int value = e.Value;
			ScrollPanePeer peer = (ScrollPanePeer) Scroller.Peer_Renamed;
			if (peer != null)
			{
				peer.SetValue(adj, value);
			}

			Component c = Scroller.GetComponent(0);
			switch (adj.Orientation)
			{
			case Adjustable_Fields.VERTICAL:
				c.Move(c.Location.x, -(value));
				break;
			case Adjustable_Fields.HORIZONTAL:
				c.Move(-(value), c.Location.y);
				break;
			default:
				throw new IllegalArgumentException("Illegal adjustable orientation");
			}
		}

		private ScrollPane Scroller;
	}

}