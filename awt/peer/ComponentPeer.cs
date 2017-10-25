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

namespace java.awt.peer
{


	using CausedFocusEvent = sun.awt.CausedFocusEvent;
	using Region = sun.java2d.pipe.Region;


	/// <summary>
	/// The peer interface for <seealso cref="Component"/>. This is the top level peer
	/// interface for widgets and defines the bulk of methods for AWT component
	/// peers. Most component peers have to implement this interface (via one
	/// of the subinterfaces), except menu components, which implement
	/// <seealso cref="MenuComponentPeer"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface ComponentPeer
	{

		/// <summary>
		/// Operation for <seealso cref="#setBounds(int, int, int, int, int)"/>, indicating
		/// a change in the component location only.
		/// </summary>
		/// <seealso cref= #setBounds(int, int, int, int, int) </seealso>

		/// <summary>
		/// Operation for <seealso cref="#setBounds(int, int, int, int, int)"/>, indicating
		/// a change in the component size only.
		/// </summary>
		/// <seealso cref= #setBounds(int, int, int, int, int) </seealso>

		/// <summary>
		/// Operation for <seealso cref="#setBounds(int, int, int, int, int)"/>, indicating
		/// a change in the component size and location.
		/// </summary>
		/// <seealso cref= #setBounds(int, int, int, int, int) </seealso>

		/// <summary>
		/// Operation for <seealso cref="#setBounds(int, int, int, int, int)"/>, indicating
		/// a change in the component client size. This is used for setting
		/// the 'inside' size of windows, without the border insets.
		/// </summary>
		/// <seealso cref= #setBounds(int, int, int, int, int) </seealso>

		/// <summary>
		/// Resets the setBounds() operation to DEFAULT_OPERATION. This is not
		/// passed into <seealso cref="#setBounds(int, int, int, int, int)"/>.
		/// 
		/// TODO: This is only used internally and should probably be moved outside
		///       the peer interface.
		/// </summary>
		/// <seealso cref= Component#setBoundsOp </seealso>

		/// <summary>
		/// A flag that is used to suppress checks for embedded frames.
		/// 
		/// TODO: This is only used internally and should probably be moved outside
		///       the peer interface.
		/// </summary>

		/// <summary>
		/// The default operation, which is to set size and location.
		/// 
		/// TODO: This is only used internally and should probably be moved outside
		///       the peer interface.
		/// </summary>
		/// <seealso cref= Component#setBoundsOp </seealso>

		/// <summary>
		/// Determines if a component has been obscured, i.e. by an overlapping
		/// window or similar. This is used by JViewport for optimizing performance.
		/// This doesn't have to be implemented, when
		/// <seealso cref="#canDetermineObscurity()"/> returns {@code false}.
		/// </summary>
		/// <returns> {@code true} when the component has been obscured,
		///         {@code false} otherwise
		/// </returns>
		/// <seealso cref= #canDetermineObscurity() </seealso>
		/// <seealso cref= javax.swing.JViewport#needsRepaintAfterBlit </seealso>
		bool Obscured {get;}

		/// <summary>
		/// Returns {@code true} when the peer can determine if a component
		/// has been obscured, {@code false} false otherwise.
		/// </summary>
		/// <returns> {@code true} when the peer can determine if a component
		///         has been obscured, {@code false} false otherwise
		/// </returns>
		/// <seealso cref= #isObscured() </seealso>
		/// <seealso cref= javax.swing.JViewport#needsRepaintAfterBlit </seealso>
		bool CanDetermineObscurity();

		/// <summary>
		/// Makes a component visible or invisible.
		/// </summary>
		/// <param name="v"> {@code true} to make a component visible,
		///          {@code false} to make it invisible
		/// </param>
		/// <seealso cref= Component#setVisible(boolean) </seealso>
		bool Visible {set;}

		/// <summary>
		/// Enables or disables a component. Disabled components are usually grayed
		/// out and cannot be activated.
		/// </summary>
		/// <param name="e"> {@code true} to enable the component, {@code false}
		///          to disable it
		/// </param>
		/// <seealso cref= Component#setEnabled(boolean) </seealso>
		bool Enabled {set;}

		/// <summary>
		/// Paints the component to the specified graphics context. This is called
		/// by <seealso cref="Component#paintAll(Graphics)"/> to paint the component.
		/// </summary>
		/// <param name="g"> the graphics context to paint to
		/// </param>
		/// <seealso cref= Component#paintAll(Graphics) </seealso>
		void Paint(Graphics g);

		/// <summary>
		/// Prints the component to the specified graphics context. This is called
		/// by <seealso cref="Component#printAll(Graphics)"/> to print the component.
		/// </summary>
		/// <param name="g"> the graphics context to print to
		/// </param>
		/// <seealso cref= Component#printAll(Graphics) </seealso>
		void Print(Graphics g);

		/// <summary>
		/// Sets the location or size or both of the component. The location is
		/// specified relative to the component's parent. The {@code op}
		/// parameter specifies which properties change. If it is
		/// <seealso cref="#SET_LOCATION"/>, then only the location changes (and the size
		/// parameters can be ignored). If {@code op} is <seealso cref="#SET_SIZE"/>,
		/// then only the size changes (and the location can be ignored). If
		/// {@code op} is <seealso cref="#SET_BOUNDS"/>, then both change. There is a
		/// special value <seealso cref="#SET_CLIENT_SIZE"/>, which is used only for
		/// window-like components to set the size of the client (i.e. the 'inner'
		/// size, without the insets of the window borders).
		/// </summary>
		/// <param name="x"> the X location of the component </param>
		/// <param name="y"> the Y location of the component </param>
		/// <param name="width"> the width of the component </param>
		/// <param name="height"> the height of the component </param>
		/// <param name="op"> the operation flag
		/// </param>
		/// <seealso cref= #SET_BOUNDS </seealso>
		/// <seealso cref= #SET_LOCATION </seealso>
		/// <seealso cref= #SET_SIZE </seealso>
		/// <seealso cref= #SET_CLIENT_SIZE </seealso>
		void SetBounds(int x, int y, int width, int height, int op);

		/// <summary>
		/// Called to let the component peer handle events.
		/// </summary>
		/// <param name="e"> the AWT event to handle
		/// </param>
		/// <seealso cref= Component#dispatchEvent(AWTEvent) </seealso>
		void HandleEvent(AWTEvent e);

		/// <summary>
		/// Called to coalesce paint events.
		/// </summary>
		/// <param name="e"> the paint event to consider to coalesce
		/// </param>
		/// <seealso cref= EventQueue#coalescePaintEvent </seealso>
		void CoalescePaintEvent(PaintEvent e);

		/// <summary>
		/// Determines the location of the component on the screen.
		/// </summary>
		/// <returns> the location of the component on the screen
		/// </returns>
		/// <seealso cref= Component#getLocationOnScreen() </seealso>
		Point LocationOnScreen {get;}

		/// <summary>
		/// Determines the preferred size of the component.
		/// </summary>
		/// <returns> the preferred size of the component
		/// </returns>
		/// <seealso cref= Component#getPreferredSize() </seealso>
		Dimension PreferredSize {get;}

		/// <summary>
		/// Determines the minimum size of the component.
		/// </summary>
		/// <returns> the minimum size of the component
		/// </returns>
		/// <seealso cref= Component#getMinimumSize() </seealso>
		Dimension MinimumSize {get;}

		/// <summary>
		/// Returns the color model used by the component.
		/// </summary>
		/// <returns> the color model used by the component
		/// </returns>
		/// <seealso cref= Component#getColorModel() </seealso>
		ColorModel ColorModel {get;}

		/// <summary>
		/// Returns a graphics object to paint on the component.
		/// </summary>
		/// <returns> a graphics object to paint on the component
		/// </returns>
		/// <seealso cref= Component#getGraphics() </seealso>
		// TODO: Maybe change this to force Graphics2D, since many things will
		// break with plain Graphics nowadays.
		Graphics Graphics {get;}

		/// <summary>
		/// Returns a font metrics object to determine the metrics properties of
		/// the specified font.
		/// </summary>
		/// <param name="font"> the font to determine the metrics for
		/// </param>
		/// <returns> a font metrics object to determine the metrics properties of
		///         the specified font
		/// </returns>
		/// <seealso cref= Component#getFontMetrics(Font) </seealso>
		FontMetrics GetFontMetrics(Font font);

		/// <summary>
		/// Disposes all resources held by the component peer. This is called
		/// when the component has been disconnected from the component hierarchy
		/// and is about to be garbage collected.
		/// </summary>
		/// <seealso cref= Component#removeNotify() </seealso>
		void Dispose();

		/// <summary>
		/// Sets the foreground color of this component.
		/// </summary>
		/// <param name="c"> the foreground color to set
		/// </param>
		/// <seealso cref= Component#setForeground(Color) </seealso>
		Color Foreground {set;}

		/// <summary>
		/// Sets the background color of this component.
		/// </summary>
		/// <param name="c"> the background color to set
		/// </param>
		/// <seealso cref= Component#setBackground(Color) </seealso>
		Color Background {set;}

		/// <summary>
		/// Sets the font of this component.
		/// </summary>
		/// <param name="f"> the font of this component
		/// </param>
		/// <seealso cref= Component#setFont(Font) </seealso>
		Font Font {set;}

		/// <summary>
		/// Updates the cursor of the component.
		/// </summary>
		/// <seealso cref= Component#updateCursorImmediately </seealso>
		void UpdateCursorImmediately();

		/// <summary>
		/// Requests focus on this component.
		/// </summary>
		/// <param name="lightweightChild"> the actual lightweight child that requests the
		///        focus </param>
		/// <param name="temporary"> {@code true} if the focus change is temporary,
		///        {@code false} otherwise </param>
		/// <param name="focusedWindowChangeAllowed"> {@code true} if changing the
		///        focus of the containing window is allowed or not </param>
		/// <param name="time"> the time of the focus change request </param>
		/// <param name="cause"> the cause of the focus change request
		/// </param>
		/// <returns> {@code true} if the focus change is guaranteed to be
		///         granted, {@code false} otherwise </returns>
		bool RequestFocus(Component lightweightChild, bool temporary, bool focusedWindowChangeAllowed, long time, CausedFocusEvent.Cause cause);

		/// <summary>
		/// Returns {@code true} when the component takes part in the focus
		/// traversal, {@code false} otherwise.
		/// </summary>
		/// <returns> {@code true} when the component takes part in the focus
		///         traversal, {@code false} otherwise </returns>
		bool Focusable {get;}

		/// <summary>
		/// Creates an image using the specified image producer.
		/// </summary>
		/// <param name="producer"> the image producer from which the image pixels will be
		///        produced
		/// </param>
		/// <returns> the created image
		/// </returns>
		/// <seealso cref= Component#createImage(ImageProducer) </seealso>
		Image CreateImage(ImageProducer producer);

		/// <summary>
		/// Creates an empty image with the specified width and height. This is
		/// generally used as a non-accelerated backbuffer for drawing onto the
		/// component (e.g. by Swing).
		/// </summary>
		/// <param name="width"> the width of the image </param>
		/// <param name="height"> the height of the image
		/// </param>
		/// <returns> the created image
		/// </returns>
		/// <seealso cref= Component#createImage(int, int) </seealso>
		// TODO: Maybe make that return a BufferedImage, because some stuff will
		// break if a different kind of image is returned.
		Image CreateImage(int width, int height);

		/// <summary>
		/// Creates an empty volatile image with the specified width and height.
		/// This is generally used as an accelerated backbuffer for drawing onto
		/// the component (e.g. by Swing).
		/// </summary>
		/// <param name="width"> the width of the image </param>
		/// <param name="height"> the height of the image
		/// </param>
		/// <returns> the created volatile image
		/// </returns>
		/// <seealso cref= Component#createVolatileImage(int, int) </seealso>
		// TODO: Include capabilities here and fix Component#createVolatileImage
		VolatileImage CreateVolatileImage(int width, int height);

		/// <summary>
		/// Prepare the specified image for rendering on this component. This should
		/// start loading the image (if not already loaded) and create an
		/// appropriate screen representation.
		/// </summary>
		/// <param name="img"> the image to prepare </param>
		/// <param name="w"> the width of the screen representation </param>
		/// <param name="h"> the height of the screen representation </param>
		/// <param name="o"> an image observer to observe the progress
		/// </param>
		/// <returns> {@code true} if the image is already fully prepared,
		///         {@code false} otherwise
		/// </returns>
		/// <seealso cref= Component#prepareImage(Image, int, int, ImageObserver) </seealso>
		bool PrepareImage(Image img, int w, int h, ImageObserver o);

		/// <summary>
		/// Determines the status of the construction of the screen representaion
		/// of the specified image.
		/// </summary>
		/// <param name="img"> the image to check </param>
		/// <param name="w"> the target width </param>
		/// <param name="h"> the target height </param>
		/// <param name="o"> the image observer to notify
		/// </param>
		/// <returns> the status as bitwise ORed ImageObserver flags
		/// </returns>
		/// <seealso cref= Component#checkImage(Image, int, int, ImageObserver) </seealso>
		int CheckImage(Image img, int w, int h, ImageObserver o);

		/// <summary>
		/// Returns the graphics configuration that corresponds to this component.
		/// </summary>
		/// <returns> the graphics configuration that corresponds to this component
		/// </returns>
		/// <seealso cref= Component#getGraphicsConfiguration() </seealso>
		GraphicsConfiguration GraphicsConfiguration {get;}

		/// <summary>
		/// Determines if the component handles wheel scrolling itself. Otherwise
		/// it is delegated to the component's parent.
		/// </summary>
		/// <returns> {@code true} if the component handles wheel scrolling,
		///         {@code false} otherwise
		/// </returns>
		/// <seealso cref= Component#dispatchEventImpl(AWTEvent) </seealso>
		bool HandlesWheelScrolling();

		/// <summary>
		/// Create {@code numBuffers} flipping buffers with the specified
		/// buffer capabilities.
		/// </summary>
		/// <param name="numBuffers"> the number of buffers to create </param>
		/// <param name="caps"> the buffer capabilities
		/// </param>
		/// <exception cref="AWTException"> if flip buffering is not supported
		/// </exception>
		/// <seealso cref= Component.FlipBufferStrategy#createBuffers </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void createBuffers(int numBuffers, BufferCapabilities caps) throws AWTException;
		void CreateBuffers(int numBuffers, BufferCapabilities caps);

		/// <summary>
		/// Returns the back buffer as image.
		/// </summary>
		/// <returns> the back buffer as image
		/// </returns>
		/// <seealso cref= Component.FlipBufferStrategy#getBackBuffer </seealso>
		Image BackBuffer {get;}

		/// <summary>
		/// Move the back buffer to the front buffer.
		/// </summary>
		/// <param name="x1"> the area to be flipped, upper left X coordinate </param>
		/// <param name="y1"> the area to be flipped, upper left Y coordinate </param>
		/// <param name="x2"> the area to be flipped, lower right X coordinate </param>
		/// <param name="y2"> the area to be flipped, lower right Y coordinate </param>
		/// <param name="flipAction"> the flip action to perform
		/// </param>
		/// <seealso cref= Component.FlipBufferStrategy#flip </seealso>
		void Flip(int x1, int y1, int x2, int y2, BufferCapabilities.FlipContents flipAction);

		/// <summary>
		/// Destroys all created buffers.
		/// </summary>
		/// <seealso cref= Component.FlipBufferStrategy#destroyBuffers </seealso>
		void DestroyBuffers();

		/// <summary>
		/// Reparents this peer to the new parent referenced by
		/// {@code newContainer} peer. Implementation depends on toolkit and
		/// container.
		/// </summary>
		/// <param name="newContainer"> peer of the new parent container
		/// 
		/// @since 1.5 </param>
		void Reparent(ContainerPeer newContainer);

		/// <summary>
		/// Returns whether this peer supports reparenting to another parent without
		/// destroying the peer.
		/// </summary>
		/// <returns> true if appropriate reparent is supported, false otherwise
		/// 
		/// @since 1.5 </returns>
		bool ReparentSupported {get;}

		/// <summary>
		/// Used by lightweight implementations to tell a ComponentPeer to layout
		/// its sub-elements.  For instance, a lightweight Checkbox needs to layout
		/// the box, as well as the text label.
		/// </summary>
		/// <seealso cref= Component#validate() </seealso>
		void Layout();

		/// <summary>
		/// Applies the shape to the native component window.
		/// @since 1.7
		/// </summary>
		/// <seealso cref= Component#applyCompoundShape </seealso>
		void ApplyShape(Region shape);

		/// <summary>
		/// Lowers this component at the bottom of the above HW peer. If the above parameter
		/// is null then the method places this component at the top of the Z-order.
		/// </summary>
		ComponentPeer ZOrder {set;}

		/// <summary>
		/// Updates internal data structures related to the component's GC.
		/// </summary>
		/// <returns> if the peer needs to be recreated for the changes to take effect
		/// @since 1.7 </returns>
		bool UpdateGraphicsData(GraphicsConfiguration gc);
	}

	public static class ComponentPeer_Fields
	{
		public const int SET_LOCATION = 1;
		public const int SET_SIZE = 2;
		public const int SET_BOUNDS = 3;
		public const int SET_CLIENT_SIZE = 4;
		public const int RESET_OPERATION = 5;
		public static readonly int NO_EMBEDDED_CHECK = (1 << 14);
		public const int DEFAULT_OPERATION = SET_BOUNDS;
	}

}