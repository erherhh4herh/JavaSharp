/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.image
{


	/// <summary>
	/// The <code>BufferStrategy</code> class represents the mechanism with which
	/// to organize complex memory on a particular <code>Canvas</code> or
	/// <code>Window</code>.  Hardware and software limitations determine whether and
	/// how a particular buffer strategy can be implemented.  These limitations
	/// are detectable through the capabilities of the
	/// <code>GraphicsConfiguration</code> used when creating the
	/// <code>Canvas</code> or <code>Window</code>.
	/// <para>
	/// It is worth noting that the terms <i>buffer</i> and <i>surface</i> are meant
	/// to be synonymous: an area of contiguous memory, either in video device
	/// memory or in system memory.
	/// </para>
	/// <para>
	/// There are several types of complex buffer strategies, including
	/// sequential ring buffering and blit buffering.
	/// Sequential ring buffering (i.e., double or triple
	/// buffering) is the most common; an application draws to a single <i>back
	/// buffer</i> and then moves the contents to the front (display) in a single
	/// step, either by copying the data or moving the video pointer.
	/// Moving the video pointer exchanges the buffers so that the first buffer
	/// drawn becomes the <i>front buffer</i>, or what is currently displayed on the
	/// device; this is called <i>page flipping</i>.
	/// </para>
	/// <para>
	/// Alternatively, the contents of the back buffer can be copied, or
	/// <i>blitted</i> forward in a chain instead of moving the video pointer.
	/// <pre>{@code
	/// Double buffering:
	/// 
	///                    ***********         ***********
	///                    *         * ------> *         *
	/// [To display] <---- * Front B *   Show  * Back B. * <---- Rendering
	///                    *         * <------ *         *
	///                    ***********         ***********
	/// 
	/// Triple buffering:
	/// 
	/// [To      ***********         ***********        ***********
	/// display] *         * --------+---------+------> *         *
	///    <---- * Front B *   Show  * Mid. B. *        * Back B. * <---- Rendering
	///          *         * <------ *         * <----- *         *
	///          ***********         ***********        ***********
	/// 
	/// }</pre>
	/// </para>
	/// <para>
	/// Here is an example of how buffer strategies can be created and used:
	/// <pre><code>
	/// 
	/// // Check the capabilities of the GraphicsConfiguration
	/// ...
	/// 
	/// // Create our component
	/// Window w = new Window(gc);
	/// 
	/// // Show our window
	/// w.setVisible(true);
	/// 
	/// // Create a general double-buffering strategy
	/// w.createBufferStrategy(2);
	/// BufferStrategy strategy = w.getBufferStrategy();
	/// 
	/// // Main loop
	/// while (!done) {
	///     // Prepare for rendering the next frame
	///     // ...
	/// 
	///     // Render single frame
	///     do {
	///         // The following loop ensures that the contents of the drawing buffer
	///         // are consistent in case the underlying surface was recreated
	///         do {
	///             // Get a new graphics context every time through the loop
	///             // to make sure the strategy is validated
	///             Graphics graphics = strategy.getDrawGraphics();
	/// 
	///             // Render to graphics
	///             // ...
	/// 
	///             // Dispose the graphics
	///             graphics.dispose();
	/// 
	///             // Repeat the rendering if the drawing buffer contents
	///             // were restored
	///         } while (strategy.contentsRestored());
	/// 
	///         // Display the buffer
	///         strategy.show();
	/// 
	///         // Repeat the rendering if the drawing buffer was lost
	///     } while (strategy.contentsLost());
	/// }
	/// 
	/// // Dispose the window
	/// w.setVisible(false);
	/// w.dispose();
	/// </code></pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.Window </seealso>
	/// <seealso cref= java.awt.Canvas </seealso>
	/// <seealso cref= java.awt.GraphicsConfiguration </seealso>
	/// <seealso cref= VolatileImage
	/// @author Michael Martak
	/// @since 1.4 </seealso>
	public abstract class BufferStrategy
	{

		/// <summary>
		/// Returns the <code>BufferCapabilities</code> for this
		/// <code>BufferStrategy</code>.
		/// </summary>
		/// <returns> the buffering capabilities of this strategy </returns>
		public abstract BufferCapabilities Capabilities {get;}

		/// <summary>
		/// Creates a graphics context for the drawing buffer.  This method may not
		/// be synchronized for performance reasons; use of this method by multiple
		/// threads should be handled at the application level.  Disposal of the
		/// graphics object obtained must be handled by the application.
		/// </summary>
		/// <returns> a graphics context for the drawing buffer </returns>
		public abstract Graphics DrawGraphics {get;}

		/// <summary>
		/// Returns whether the drawing buffer was lost since the last call to
		/// <code>getDrawGraphics</code>.  Since the buffers in a buffer strategy
		/// are usually type <code>VolatileImage</code>, they may become lost.
		/// For a discussion on lost buffers, see <code>VolatileImage</code>.
		/// </summary>
		/// <returns> Whether or not the drawing buffer was lost since the last call
		/// to <code>getDrawGraphics</code>. </returns>
		/// <seealso cref= java.awt.image.VolatileImage </seealso>
		public abstract bool ContentsLost();

		/// <summary>
		/// Returns whether the drawing buffer was recently restored from a lost
		/// state and reinitialized to the default background color (white).
		/// Since the buffers in a buffer strategy are usually type
		/// <code>VolatileImage</code>, they may become lost.  If a surface has
		/// been recently restored from a lost state since the last call to
		/// <code>getDrawGraphics</code>, it may require repainting.
		/// For a discussion on lost buffers, see <code>VolatileImage</code>.
		/// </summary>
		/// <returns> Whether or not the drawing buffer was restored since the last
		///         call to <code>getDrawGraphics</code>. </returns>
		/// <seealso cref= java.awt.image.VolatileImage </seealso>
		public abstract bool ContentsRestored();

		/// <summary>
		/// Makes the next available buffer visible by either copying the memory
		/// (blitting) or changing the display pointer (flipping).
		/// </summary>
		public abstract void Show();

		/// <summary>
		/// Releases system resources currently consumed by this
		/// <code>BufferStrategy</code> and
		/// removes it from the associated Component.  After invoking this
		/// method, <code>getBufferStrategy</code> will return null.  Trying
		/// to use a <code>BufferStrategy</code> after it has been disposed will
		/// result in undefined behavior.
		/// </summary>
		/// <seealso cref= java.awt.Window#createBufferStrategy </seealso>
		/// <seealso cref= java.awt.Canvas#createBufferStrategy </seealso>
		/// <seealso cref= java.awt.Window#getBufferStrategy </seealso>
		/// <seealso cref= java.awt.Canvas#getBufferStrategy
		/// @since 1.6 </seealso>
		public virtual void Dispose()
		{
		}
	}

}