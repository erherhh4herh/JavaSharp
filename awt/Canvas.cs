/*
 * Copyright (c) 1995, 2010, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// A <code>Canvas</code> component represents a blank rectangular
	/// area of the screen onto which the application can draw or from
	/// which the application can trap input events from the user.
	/// <para>
	/// An application must subclass the <code>Canvas</code> class in
	/// order to get useful functionality such as creating a custom
	/// component. The <code>paint</code> method must be overridden
	/// in order to perform custom graphics on the canvas.
	/// 
	/// @author      Sami Shaio
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	public class Canvas : Component, Accessible
	{

		private const String @base = "canvas";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = -2284879212465893870L;

		/// <summary>
		/// Constructs a new Canvas.
		/// </summary>
		public Canvas()
		{
		}

		/// <summary>
		/// Constructs a new Canvas given a GraphicsConfiguration object.
		/// </summary>
		/// <param name="config"> a reference to a GraphicsConfiguration object.
		/// </param>
		/// <seealso cref= GraphicsConfiguration </seealso>
		public Canvas(GraphicsConfiguration config) : this()
		{
			GraphicsConfiguration = config;
		}

		internal override GraphicsConfiguration GraphicsConfiguration
		{
			set
			{
				lock (TreeLock)
				{
					CanvasPeer peer = (CanvasPeer)Peer;
					if (peer != null)
					{
						value = peer.GetAppropriateGraphicsConfiguration(value);
					}
					base.GraphicsConfiguration = value;
				}
			}
		}

		/// <summary>
		/// Construct a name for this component.  Called by getName() when the
		/// name is null.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(Canvas))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the peer of the canvas.  This peer allows you to change the
		/// user interface of the canvas without changing its functionality. </summary>
		/// <seealso cref=     java.awt.Toolkit#createCanvas(java.awt.Canvas) </seealso>
		/// <seealso cref=     java.awt.Component#getToolkit() </seealso>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateCanvas(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// Paints this canvas.
		/// <para>
		/// Most applications that subclass <code>Canvas</code> should
		/// override this method in order to perform some useful operation
		/// (typically, custom painting of the canvas).
		/// The default operation is simply to clear the canvas.
		/// Applications that override this method need not call
		/// super.paint(g).
		/// 
		/// </para>
		/// </summary>
		/// <param name="g">   the specified Graphics context </param>
		/// <seealso cref=        #update(Graphics) </seealso>
		/// <seealso cref=        Component#paint(Graphics) </seealso>
		public override void Paint(Graphics g)
		{
			g.ClearRect(0, 0, Width_Renamed, Height_Renamed);
		}

		/// <summary>
		/// Updates this canvas.
		/// <para>
		/// This method is called in response to a call to <code>repaint</code>.
		/// The canvas is first cleared by filling it with the background
		/// color, and then completely redrawn by calling this canvas's
		/// <code>paint</code> method.
		/// Note: applications that override this method should either call
		/// super.update(g) or incorporate the functionality described
		/// above into their own code.
		/// 
		/// </para>
		/// </summary>
		/// <param name="g"> the specified Graphics context </param>
		/// <seealso cref=   #paint(Graphics) </seealso>
		/// <seealso cref=   Component#update(Graphics) </seealso>
		public override void Update(Graphics g)
		{
			g.ClearRect(0, 0, Width_Renamed, Height_Renamed);
			Paint(g);
		}

		internal override bool PostsOldMouseEvents()
		{
			return true;
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
		/// <param name="numBuffers"> number of buffers to create </param>
		/// <param name="caps"> the required capabilities for creating the buffer strategy;
		/// cannot be <code>null</code> </param>
		/// <exception cref="AWTException"> if the capabilities supplied could not be
		/// supported or met; this may happen, for example, if there is not enough
		/// accelerated memory currently available, or if page flipping is specified
		/// but not possible. </exception>
		/// <exception cref="IllegalArgumentException"> if numBuffers is less than 1, or if
		/// caps is <code>null</code> </exception>
		/// <seealso cref= #getBufferStrategy
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void createBufferStrategy(int numBuffers, BufferCapabilities caps) throws AWTException
		public override void CreateBufferStrategy(int numBuffers, BufferCapabilities caps)
		{
			base.CreateBufferStrategy(numBuffers, caps);
		}

		/// <summary>
		/// Returns the <code>BufferStrategy</code> used by this component.  This
		/// method will return null if a <code>BufferStrategy</code> has not yet
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

		/*
		 * --- Accessibility Support ---
		 *
		 */

		/// <summary>
		/// Gets the AccessibleContext associated with this Canvas.
		/// For canvases, the AccessibleContext takes the form of an
		/// AccessibleAWTCanvas.
		/// A new AccessibleAWTCanvas instance is created if necessary.
		/// </summary>
		/// <returns> an AccessibleAWTCanvas that serves as the
		///         AccessibleContext of this Canvas
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTCanvas(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>Canvas</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to canvas user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTCanvas : AccessibleAWTComponent
		{
			private readonly Canvas OuterInstance;

			public AccessibleAWTCanvas(Canvas outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			internal const long SerialVersionUID = -6325592262103146699L;

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
					return AccessibleRole.CANVAS;
				}
			}

		} // inner class AccessibleAWTCanvas
	}

}