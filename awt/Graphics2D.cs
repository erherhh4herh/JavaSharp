using System.Collections.Generic;

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


	/// <summary>
	/// This <code>Graphics2D</code> class extends the
	/// <seealso cref="Graphics"/> class to provide more sophisticated
	/// control over geometry, coordinate transformations, color management,
	/// and text layout.  This is the fundamental class for rendering
	/// 2-dimensional shapes, text and images on the  Java(tm) platform.
	/// <para>
	/// <h2>Coordinate Spaces</h2>
	/// All coordinates passed to a <code>Graphics2D</code> object are specified
	/// in a device-independent coordinate system called User Space, which is
	/// used by applications.  The <code>Graphics2D</code> object contains
	/// an <seealso cref="AffineTransform"/> object as part of its rendering state
	/// that defines how to convert coordinates from user space to
	/// device-dependent coordinates in Device Space.
	/// </para>
	/// <para>
	/// Coordinates in device space usually refer to individual device pixels
	/// and are aligned on the infinitely thin gaps between these pixels.
	/// Some <code>Graphics2D</code> objects can be used to capture rendering
	/// operations for storage into a graphics metafile for playback on a
	/// concrete device of unknown physical resolution at a later time.  Since
	/// the resolution might not be known when the rendering operations are
	/// captured, the <code>Graphics2D</code> <code>Transform</code> is set up
	/// to transform user coordinates to a virtual device space that
	/// approximates the expected resolution of the target device. Further
	/// transformations might need to be applied at playback time if the
	/// estimate is incorrect.
	/// </para>
	/// <para>
	/// Some of the operations performed by the rendering attribute objects
	/// occur in the device space, but all <code>Graphics2D</code> methods take
	/// user space coordinates.
	/// </para>
	/// <para>
	/// Every <code>Graphics2D</code> object is associated with a target that
	/// defines where rendering takes place. A
	/// <seealso cref="GraphicsConfiguration"/> object defines the characteristics
	/// of the rendering target, such as pixel format and resolution.
	/// The same rendering target is used throughout the life of a
	/// <code>Graphics2D</code> object.
	/// </para>
	/// <para>
	/// When creating a <code>Graphics2D</code> object,  the
	/// <code>GraphicsConfiguration</code>
	/// specifies the <a name="deftransform">default transform</a> for
	/// the target of the <code>Graphics2D</code> (a
	/// <seealso cref="Component"/> or <seealso cref="Image"/>).  This default transform maps the
	/// user space coordinate system to screen and printer device coordinates
	/// such that the origin maps to the upper left hand corner of the
	/// target region of the device with increasing X coordinates extending
	/// to the right and increasing Y coordinates extending downward.
	/// The scaling of the default transform is set to identity for those devices
	/// that are close to 72 dpi, such as screen devices.
	/// The scaling of the default transform is set to approximately 72 user
	/// space coordinates per square inch for high resolution devices, such as
	/// printers.  For image buffers, the default transform is the
	/// <code>Identity</code> transform.
	/// 
	/// <h2>Rendering Process</h2>
	/// The Rendering Process can be broken down into four phases that are
	/// controlled by the <code>Graphics2D</code> rendering attributes.
	/// The renderer can optimize many of these steps, either by caching the
	/// results for future calls, by collapsing multiple virtual steps into
	/// a single operation, or by recognizing various attributes as common
	/// simple cases that can be eliminated by modifying other parts of the
	/// operation.
	/// </para>
	/// <para>
	/// The steps in the rendering process are:
	/// <ol>
	/// <li>
	/// Determine what to render.
	/// <li>
	/// Constrain the rendering operation to the current <code>Clip</code>.
	/// The <code>Clip</code> is specified by a <seealso cref="Shape"/> in user
	/// space and is controlled by the program using the various clip
	/// manipulation methods of <code>Graphics</code> and
	/// <code>Graphics2D</code>.  This <i>user clip</i>
	/// is transformed into device space by the current
	/// <code>Transform</code> and combined with the
	/// <i>device clip</i>, which is defined by the visibility of windows and
	/// device extents.  The combination of the user clip and device clip
	/// defines the <i>composite clip</i>, which determines the final clipping
	/// region.  The user clip is not modified by the rendering
	/// system to reflect the resulting composite clip.
	/// <li>
	/// Determine what colors to render.
	/// <li>
	/// Apply the colors to the destination drawing surface using the current
	/// <seealso cref="Composite"/> attribute in the <code>Graphics2D</code> context.
	/// </ol>
	/// <br>
	/// The three types of rendering operations, along with details of each
	/// of their particular rendering processes are:
	/// <ol>
	/// <li>
	/// <b><a name="rendershape"><code>Shape</code> operations</a></b>
	/// <ol>
	/// <li>
	/// If the operation is a <code>draw(Shape)</code> operation, then
	/// the  <seealso cref="Stroke#createStrokedShape(Shape) createStrokedShape"/>
	/// method on the current <seealso cref="Stroke"/> attribute in the
	/// <code>Graphics2D</code> context is used to construct a new
	/// <code>Shape</code> object that contains the outline of the specified
	/// <code>Shape</code>.
	/// <li>
	/// The <code>Shape</code> is transformed from user space to device space
	/// using the current <code>Transform</code>
	/// in the <code>Graphics2D</code> context.
	/// <li>
	/// The outline of the <code>Shape</code> is extracted using the
	/// <seealso cref="Shape#getPathIterator(AffineTransform) getPathIterator"/> method of
	/// <code>Shape</code>, which returns a
	/// <seealso cref="java.awt.geom.PathIterator PathIterator"/>
	/// object that iterates along the boundary of the <code>Shape</code>.
	/// <li>
	/// If the <code>Graphics2D</code> object cannot handle the curved segments
	/// that the <code>PathIterator</code> object returns then it can call the
	/// alternate
	/// <seealso cref="Shape#getPathIterator(AffineTransform, double) getPathIterator"/>
	/// method of <code>Shape</code>, which flattens the <code>Shape</code>.
	/// <li>
	/// The current <seealso cref="Paint"/> in the <code>Graphics2D</code> context
	/// is queried for a <seealso cref="PaintContext"/>, which specifies the
	/// colors to render in device space.
	/// </ol>
	/// <li>
	/// <b><a name=rendertext>Text operations</a></b>
	/// <ol>
	/// <li>
	/// The following steps are used to determine the set of glyphs required
	/// to render the indicated <code>String</code>:
	/// <ol>
	/// <li>
	/// If the argument is a <code>String</code>, then the current
	/// <code>Font</code> in the <code>Graphics2D</code> context is asked to
	/// convert the Unicode characters in the <code>String</code> into a set of
	/// glyphs for presentation with whatever basic layout and shaping
	/// algorithms the font implements.
	/// <li>
	/// If the argument is an
	/// <seealso cref="AttributedCharacterIterator"/>,
	/// the iterator is asked to convert itself to a
	/// <seealso cref="java.awt.font.TextLayout TextLayout"/>
	/// using its embedded font attributes. The <code>TextLayout</code>
	/// implements more sophisticated glyph layout algorithms that
	/// perform Unicode bi-directional layout adjustments automatically
	/// for multiple fonts of differing writing directions.
	/// <li>
	/// If the argument is a
	/// <seealso cref="GlyphVector"/>, then the
	/// <code>GlyphVector</code> object already contains the appropriate
	/// font-specific glyph codes with explicit coordinates for the position of
	/// each glyph.
	/// </ol>
	/// <li>
	/// The current <code>Font</code> is queried to obtain outlines for the
	/// indicated glyphs.  These outlines are treated as shapes in user space
	/// relative to the position of each glyph that was determined in step 1.
	/// <li>
	/// The character outlines are filled as indicated above
	/// under <a href="#rendershape"><code>Shape</code> operations</a>.
	/// <li>
	/// The current <code>Paint</code> is queried for a
	/// <code>PaintContext</code>, which specifies
	/// the colors to render in device space.
	/// </ol>
	/// <li>
	/// <b><a name= renderingimage><code>Image</code> Operations</a></b>
	/// <ol>
	/// <li>
	/// The region of interest is defined by the bounding box of the source
	/// <code>Image</code>.
	/// This bounding box is specified in Image Space, which is the
	/// <code>Image</code> object's local coordinate system.
	/// <li>
	/// If an <code>AffineTransform</code> is passed to
	/// <seealso cref="#drawImage(java.awt.Image, java.awt.geom.AffineTransform, java.awt.image.ImageObserver) drawImage(Image, AffineTransform, ImageObserver)"/>,
	/// the <code>AffineTransform</code> is used to transform the bounding
	/// box from image space to user space. If no <code>AffineTransform</code>
	/// is supplied, the bounding box is treated as if it is already in user space.
	/// <li>
	/// The bounding box of the source <code>Image</code> is transformed from user
	/// space into device space using the current <code>Transform</code>.
	/// Note that the result of transforming the bounding box does not
	/// necessarily result in a rectangular region in device space.
	/// <li>
	/// The <code>Image</code> object determines what colors to render,
	/// sampled according to the source to destination
	/// coordinate mapping specified by the current <code>Transform</code> and the
	/// optional image transform.
	/// </ol>
	/// </ol>
	/// 
	/// <h2>Default Rendering Attributes</h2>
	/// The default values for the <code>Graphics2D</code> rendering attributes are:
	/// <dl compact>
	/// <dt><i><code>Paint</code></i>
	/// <dd>The color of the <code>Component</code>.
	/// <dt><i><code>Font</code></i>
	/// <dd>The <code>Font</code> of the <code>Component</code>.
	/// <dt><i><code>Stroke</code></i>
	/// <dd>A square pen with a linewidth of 1, no dashing, miter segment joins
	/// and square end caps.
	/// <dt><i><code>Transform</code></i>
	/// <dd>The
	/// <seealso cref="GraphicsConfiguration#getDefaultTransform() getDefaultTransform"/>
	/// for the <code>GraphicsConfiguration</code> of the <code>Component</code>.
	/// <dt><i><code>Composite</code></i>
	/// <dd>The <seealso cref="AlphaComposite#SRC_OVER"/> rule.
	/// <dt><i><code>Clip</code></i>
	/// <dd>No rendering <code>Clip</code>, the output is clipped to the
	/// <code>Component</code>.
	/// </dl>
	/// 
	/// <h2>Rendering Compatibility Issues</h2>
	/// The JDK(tm) 1.1 rendering model is based on a pixelization model
	/// that specifies that coordinates
	/// are infinitely thin, lying between the pixels.  Drawing operations are
	/// performed using a one-pixel wide pen that fills the
	/// pixel below and to the right of the anchor point on the path.
	/// The JDK 1.1 rendering model is consistent with the
	/// capabilities of most of the existing class of platform
	/// renderers that need  to resolve integer coordinates to a
	/// discrete pen that must fall completely on a specified number of pixels.
	/// </para>
	/// <para>
	/// The Java 2D(tm) (Java(tm) 2 platform) API supports antialiasing renderers.
	/// A pen with a width of one pixel does not need to fall
	/// completely on pixel N as opposed to pixel N+1.  The pen can fall
	/// partially on both pixels. It is not necessary to choose a bias
	/// direction for a wide pen since the blending that occurs along the
	/// pen traversal edges makes the sub-pixel position of the pen
	/// visible to the user.  On the other hand, when antialiasing is
	/// turned off by setting the
	/// <seealso cref="RenderingHints#KEY_ANTIALIASING KEY_ANTIALIASING"/> hint key
	/// to the
	/// <seealso cref="RenderingHints#VALUE_ANTIALIAS_OFF VALUE_ANTIALIAS_OFF"/>
	/// hint value, the renderer might need
	/// to apply a bias to determine which pixel to modify when the pen
	/// is straddling a pixel boundary, such as when it is drawn
	/// along an integer coordinate in device space.  While the capabilities
	/// of an antialiasing renderer make it no longer necessary for the
	/// rendering model to specify a bias for the pen, it is desirable for the
	/// antialiasing and non-antialiasing renderers to perform similarly for
	/// the common cases of drawing one-pixel wide horizontal and vertical
	/// lines on the screen.  To ensure that turning on antialiasing by
	/// setting the
	/// <seealso cref="RenderingHints#KEY_ANTIALIASING KEY_ANTIALIASING"/> hint
	/// key to
	/// <seealso cref="RenderingHints#VALUE_ANTIALIAS_ON VALUE_ANTIALIAS_ON"/>
	/// does not cause such lines to suddenly become twice as wide and half
	/// as opaque, it is desirable to have the model specify a path for such
	/// lines so that they completely cover a particular set of pixels to help
	/// increase their crispness.
	/// </para>
	/// <para>
	/// Java 2D API maintains compatibility with JDK 1.1 rendering
	/// behavior, such that legacy operations and existing renderer
	/// behavior is unchanged under Java 2D API.  Legacy
	/// methods that map onto general <code>draw</code> and
	/// <code>fill</code> methods are defined, which clearly indicates
	/// how <code>Graphics2D</code> extends <code>Graphics</code> based
	/// on settings of <code>Stroke</code> and <code>Transform</code>
	/// attributes and rendering hints.  The definition
	/// performs identically under default attribute settings.
	/// For example, the default <code>Stroke</code> is a
	/// <code>BasicStroke</code> with a width of 1 and no dashing and the
	/// default Transform for screen drawing is an Identity transform.
	/// </para>
	/// <para>
	/// The following two rules provide predictable rendering behavior whether
	/// aliasing or antialiasing is being used.
	/// <ul>
	/// <li> Device coordinates are defined to be between device pixels which
	/// avoids any inconsistent results between aliased and antialiased
	/// rendering.  If coordinates were defined to be at a pixel's center, some
	/// of the pixels covered by a shape, such as a rectangle, would only be
	/// half covered.
	/// With aliased rendering, the half covered pixels would either be
	/// rendered inside the shape or outside the shape.  With anti-aliased
	/// rendering, the pixels on the entire edge of the shape would be half
	/// covered.  On the other hand, since coordinates are defined to be
	/// between pixels, a shape like a rectangle would have no half covered
	/// pixels, whether or not it is rendered using antialiasing.
	/// <li> Lines and paths stroked using the <code>BasicStroke</code>
	/// object may be "normalized" to provide consistent rendering of the
	/// outlines when positioned at various points on the drawable and
	/// whether drawn with aliased or antialiased rendering.  This
	/// normalization process is controlled by the
	/// <seealso cref="RenderingHints#KEY_STROKE_CONTROL KEY_STROKE_CONTROL"/> hint.
	/// The exact normalization algorithm is not specified, but the goals
	/// of this normalization are to ensure that lines are rendered with
	/// consistent visual appearance regardless of how they fall on the
	/// pixel grid and to promote more solid horizontal and vertical
	/// lines in antialiased mode so that they resemble their non-antialiased
	/// counterparts more closely.  A typical normalization step might
	/// promote antialiased line endpoints to pixel centers to reduce the
	/// amount of blending or adjust the subpixel positioning of
	/// non-antialiased lines so that the floating point line widths
	/// round to even or odd pixel counts with equal likelihood.  This
	/// process can move endpoints by up to half a pixel (usually towards
	/// positive infinity along both axes) to promote these consistent
	/// results.
	/// </ul>
	/// </para>
	/// <para>
	/// The following definitions of general legacy methods
	/// perform identically to previously specified behavior under default
	/// attribute settings:
	/// <ul>
	/// <li>
	/// For <code>fill</code> operations, including <code>fillRect</code>,
	/// <code>fillRoundRect</code>, <code>fillOval</code>,
	/// <code>fillArc</code>, <code>fillPolygon</code>, and
	/// <code>clearRect</code>, <seealso cref="#fill(Shape) fill"/> can now be called
	/// with the desired <code>Shape</code>.  For example, when filling a
	/// rectangle:
	/// <pre>
	/// fill(new Rectangle(x, y, w, h));
	/// </pre>
	/// is called.
	/// </para>
	/// <para>
	/// <li>
	/// Similarly, for draw operations, including <code>drawLine</code>,
	/// <code>drawRect</code>, <code>drawRoundRect</code>,
	/// <code>drawOval</code>, <code>drawArc</code>, <code>drawPolyline</code>,
	/// and <code>drawPolygon</code>, <seealso cref="#draw(Shape) draw"/> can now be
	/// called with the desired <code>Shape</code>.  For example, when drawing a
	/// rectangle:
	/// <pre>
	/// draw(new Rectangle(x, y, w, h));
	/// </pre>
	/// is called.
	/// </para>
	/// <para>
	/// <li>
	/// The <code>draw3DRect</code> and <code>fill3DRect</code> methods were
	/// implemented in terms of the <code>drawLine</code> and
	/// <code>fillRect</code> methods in the <code>Graphics</code> class which
	/// would predicate their behavior upon the current <code>Stroke</code>
	/// and <code>Paint</code> objects in a <code>Graphics2D</code> context.
	/// This class overrides those implementations with versions that use
	/// the current <code>Color</code> exclusively, overriding the current
	/// <code>Paint</code> and which uses <code>fillRect</code> to describe
	/// the exact same behavior as the preexisting methods regardless of the
	/// setting of the current <code>Stroke</code>.
	/// </ul>
	/// The <code>Graphics</code> class defines only the <code>setColor</code>
	/// method to control the color to be painted.  Since the Java 2D API extends
	/// the <code>Color</code> object to implement the new <code>Paint</code>
	/// interface, the existing
	/// <code>setColor</code> method is now a convenience method for setting the
	/// current <code>Paint</code> attribute to a <code>Color</code> object.
	/// <code>setColor(c)</code> is equivalent to <code>setPaint(c)</code>.
	/// </para>
	/// <para>
	/// The <code>Graphics</code> class defines two methods for controlling
	/// how colors are applied to the destination.
	/// <ol>
	/// <li>
	/// The <code>setPaintMode</code> method is implemented as a convenience
	/// method to set the default <code>Composite</code>, equivalent to
	/// <code>setComposite(new AlphaComposite.SrcOver)</code>.
	/// <li>
	/// The <code>setXORMode(Color xorcolor)</code> method is implemented
	/// as a convenience method to set a special <code>Composite</code> object that
	/// ignores the <code>Alpha</code> components of source colors and sets the
	/// destination color to the value:
	/// <pre>
	/// dstpixel = (PixelOf(srccolor) ^ PixelOf(xorcolor) ^ dstpixel);
	/// </pre>
	/// </ol>
	/// 
	/// @author Jim Graham
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.RenderingHints </seealso>
	public abstract class Graphics2D : Graphics
	{

		/// <summary>
		/// Constructs a new <code>Graphics2D</code> object.  Since
		/// <code>Graphics2D</code> is an abstract class, and since it must be
		/// customized by subclasses for different output devices,
		/// <code>Graphics2D</code> objects cannot be created directly.
		/// Instead, <code>Graphics2D</code> objects must be obtained from another
		/// <code>Graphics2D</code> object, created by a
		/// <code>Component</code>, or obtained from images such as
		/// <seealso cref="BufferedImage"/> objects. </summary>
		/// <seealso cref= java.awt.Component#getGraphics </seealso>
		/// <seealso cref= java.awt.Graphics#create </seealso>
		protected internal Graphics2D()
		{
		}

		/// <summary>
		/// Draws a 3-D highlighted outline of the specified rectangle.
		/// The edges of the rectangle are highlighted so that they
		/// appear to be beveled and lit from the upper left corner.
		/// <para>
		/// The colors used for the highlighting effect are determined
		/// based on the current color.
		/// The resulting rectangle covers an area that is
		/// <code>width&nbsp;+&nbsp;1</code> pixels wide
		/// by <code>height&nbsp;+&nbsp;1</code> pixels tall.  This method
		/// uses the current <code>Color</code> exclusively and ignores
		/// the current <code>Paint</code>.
		/// </para>
		/// </summary>
		/// <param name="x"> the x coordinate of the rectangle to be drawn. </param>
		/// <param name="y"> the y coordinate of the rectangle to be drawn. </param>
		/// <param name="width"> the width of the rectangle to be drawn. </param>
		/// <param name="height"> the height of the rectangle to be drawn. </param>
		/// <param name="raised"> a boolean that determines whether the rectangle
		///                      appears to be raised above the surface
		///                      or sunk into the surface. </param>
		/// <seealso cref=         java.awt.Graphics#fill3DRect </seealso>
		public override void Draw3DRect(int x, int y, int width, int height, bool raised)
		{
			Paint p = Paint;
			Color c = Color;
			Color brighter = c.Brighter();
			Color darker = c.Darker();

			Color = raised ? brighter : darker;
			//drawLine(x, y, x, y + height);
			FillRect(x, y, 1, height + 1);
			//drawLine(x + 1, y, x + width - 1, y);
			FillRect(x + 1, y, width - 1, 1);
			Color = raised ? darker : brighter;
			//drawLine(x + 1, y + height, x + width, y + height);
			FillRect(x + 1, y + height, width, 1);
			//drawLine(x + width, y, x + width, y + height - 1);
			FillRect(x + width, y, 1, height);
			Paint = p;
		}

		/// <summary>
		/// Paints a 3-D highlighted rectangle filled with the current color.
		/// The edges of the rectangle are highlighted so that it appears
		/// as if the edges were beveled and lit from the upper left corner.
		/// The colors used for the highlighting effect and for filling are
		/// determined from the current <code>Color</code>.  This method uses
		/// the current <code>Color</code> exclusively and ignores the current
		/// <code>Paint</code>. </summary>
		/// <param name="x"> the x coordinate of the rectangle to be filled. </param>
		/// <param name="y"> the y coordinate of the rectangle to be filled. </param>
		/// <param name="width"> the width of the rectangle to be filled. </param>
		/// <param name="height"> the height of the rectangle to be filled. </param>
		/// <param name="raised"> a boolean value that determines whether the
		///                      rectangle appears to be raised above the surface
		///                      or etched into the surface. </param>
		/// <seealso cref=         java.awt.Graphics#draw3DRect </seealso>
		public override void Fill3DRect(int x, int y, int width, int height, bool raised)
		{
			Paint p = Paint;
			Color c = Color;
			Color brighter = c.Brighter();
			Color darker = c.Darker();

			if (!raised)
			{
				Color = darker;
			}
			else if (p != c)
			{
				Color = c;
			}
			FillRect(x + 1, y + 1, width - 2, height - 2);
			Color = raised ? brighter : darker;
			//drawLine(x, y, x, y + height - 1);
			FillRect(x, y, 1, height);
			//drawLine(x + 1, y, x + width - 2, y);
			FillRect(x + 1, y, width - 2, 1);
			Color = raised ? darker : brighter;
			//drawLine(x + 1, y + height - 1, x + width - 1, y + height - 1);
			FillRect(x + 1, y + height - 1, width - 1, 1);
			//drawLine(x + width - 1, y, x + width - 1, y + height - 2);
			FillRect(x + width - 1, y, 1, height - 1);
			Paint = p;
		}

		/// <summary>
		/// Strokes the outline of a <code>Shape</code> using the settings of the
		/// current <code>Graphics2D</code> context.  The rendering attributes
		/// applied include the <code>Clip</code>, <code>Transform</code>,
		/// <code>Paint</code>, <code>Composite</code> and
		/// <code>Stroke</code> attributes. </summary>
		/// <param name="s"> the <code>Shape</code> to be rendered </param>
		/// <seealso cref= #setStroke </seealso>
		/// <seealso cref= #setPaint </seealso>
		/// <seealso cref= java.awt.Graphics#setColor </seealso>
		/// <seealso cref= #transform </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #clip </seealso>
		/// <seealso cref= #setClip </seealso>
		/// <seealso cref= #setComposite </seealso>
		public abstract void Draw(Shape s);

		/// <summary>
		/// Renders an image, applying a transform from image space into user space
		/// before drawing.
		/// The transformation from user space into device space is done with
		/// the current <code>Transform</code> in the <code>Graphics2D</code>.
		/// The specified transformation is applied to the image before the
		/// transform attribute in the <code>Graphics2D</code> context is applied.
		/// The rendering attributes applied include the <code>Clip</code>,
		/// <code>Transform</code>, and <code>Composite</code> attributes.
		/// Note that no rendering is done if the specified transform is
		/// noninvertible. </summary>
		/// <param name="img"> the specified image to be rendered.
		///            This method does nothing if <code>img</code> is null. </param>
		/// <param name="xform"> the transformation from image space into user space </param>
		/// <param name="obs"> the <seealso cref="ImageObserver"/>
		/// to be notified as more of the <code>Image</code>
		/// is converted </param>
		/// <returns> <code>true</code> if the <code>Image</code> is
		/// fully loaded and completely rendered, or if it's null;
		/// <code>false</code> if the <code>Image</code> is still being loaded. </returns>
		/// <seealso cref= #transform </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #setComposite </seealso>
		/// <seealso cref= #clip </seealso>
		/// <seealso cref= #setClip </seealso>
		public abstract bool DrawImage(Image img, AffineTransform xform, ImageObserver obs);

		/// <summary>
		/// Renders a <code>BufferedImage</code> that is
		/// filtered with a
		/// <seealso cref="BufferedImageOp"/>.
		/// The rendering attributes applied include the <code>Clip</code>,
		/// <code>Transform</code>
		/// and <code>Composite</code> attributes.  This is equivalent to:
		/// <pre>
		/// img1 = op.filter(img, null);
		/// drawImage(img1, new AffineTransform(1f,0f,0f,1f,x,y), null);
		/// </pre> </summary>
		/// <param name="op"> the filter to be applied to the image before rendering </param>
		/// <param name="img"> the specified <code>BufferedImage</code> to be rendered.
		///            This method does nothing if <code>img</code> is null. </param>
		/// <param name="x"> the x coordinate of the location in user space where
		/// the upper left corner of the image is rendered </param>
		/// <param name="y"> the y coordinate of the location in user space where
		/// the upper left corner of the image is rendered
		/// </param>
		/// <seealso cref= #transform </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #setComposite </seealso>
		/// <seealso cref= #clip </seealso>
		/// <seealso cref= #setClip </seealso>
		public abstract void DrawImage(BufferedImage img, BufferedImageOp op, int x, int y);

		/// <summary>
		/// Renders a <seealso cref="RenderedImage"/>,
		/// applying a transform from image
		/// space into user space before drawing.
		/// The transformation from user space into device space is done with
		/// the current <code>Transform</code> in the <code>Graphics2D</code>.
		/// The specified transformation is applied to the image before the
		/// transform attribute in the <code>Graphics2D</code> context is applied.
		/// The rendering attributes applied include the <code>Clip</code>,
		/// <code>Transform</code>, and <code>Composite</code> attributes. Note
		/// that no rendering is done if the specified transform is
		/// noninvertible. </summary>
		/// <param name="img"> the image to be rendered. This method does
		///            nothing if <code>img</code> is null. </param>
		/// <param name="xform"> the transformation from image space into user space </param>
		/// <seealso cref= #transform </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #setComposite </seealso>
		/// <seealso cref= #clip </seealso>
		/// <seealso cref= #setClip </seealso>
		public abstract void DrawRenderedImage(RenderedImage img, AffineTransform xform);

		/// <summary>
		/// Renders a
		/// <seealso cref="RenderableImage"/>,
		/// applying a transform from image space into user space before drawing.
		/// The transformation from user space into device space is done with
		/// the current <code>Transform</code> in the <code>Graphics2D</code>.
		/// The specified transformation is applied to the image before the
		/// transform attribute in the <code>Graphics2D</code> context is applied.
		/// The rendering attributes applied include the <code>Clip</code>,
		/// <code>Transform</code>, and <code>Composite</code> attributes. Note
		/// that no rendering is done if the specified transform is
		/// noninvertible.
		/// <para>
		/// Rendering hints set on the <code>Graphics2D</code> object might
		/// be used in rendering the <code>RenderableImage</code>.
		/// If explicit control is required over specific hints recognized by a
		/// specific <code>RenderableImage</code>, or if knowledge of which hints
		/// are used is required, then a <code>RenderedImage</code> should be
		/// obtained directly from the <code>RenderableImage</code>
		/// and rendered using
		/// <seealso cref="#drawRenderedImage(RenderedImage, AffineTransform) drawRenderedImage"/>.
		/// </para>
		/// </summary>
		/// <param name="img"> the image to be rendered. This method does
		///            nothing if <code>img</code> is null. </param>
		/// <param name="xform"> the transformation from image space into user space </param>
		/// <seealso cref= #transform </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #setComposite </seealso>
		/// <seealso cref= #clip </seealso>
		/// <seealso cref= #setClip </seealso>
		/// <seealso cref= #drawRenderedImage </seealso>
		public abstract void DrawRenderableImage(RenderableImage img, AffineTransform xform);

		/// <summary>
		/// Renders the text of the specified <code>String</code>, using the
		/// current text attribute state in the <code>Graphics2D</code> context.
		/// The baseline of the
		/// first character is at position (<i>x</i>,&nbsp;<i>y</i>) in
		/// the User Space.
		/// The rendering attributes applied include the <code>Clip</code>,
		/// <code>Transform</code>, <code>Paint</code>, <code>Font</code> and
		/// <code>Composite</code> attributes.  For characters in script
		/// systems such as Hebrew and Arabic, the glyphs can be rendered from
		/// right to left, in which case the coordinate supplied is the
		/// location of the leftmost character on the baseline. </summary>
		/// <param name="str"> the string to be rendered </param>
		/// <param name="x"> the x coordinate of the location where the
		/// <code>String</code> should be rendered </param>
		/// <param name="y"> the y coordinate of the location where the
		/// <code>String</code> should be rendered </param>
		/// <exception cref="NullPointerException"> if <code>str</code> is
		///         <code>null</code> </exception>
		/// <seealso cref=         java.awt.Graphics#drawBytes </seealso>
		/// <seealso cref=         java.awt.Graphics#drawChars
		/// @since       JDK1.0 </seealso>
		public override abstract void DrawString(String str, int x, int y);

		/// <summary>
		/// Renders the text specified by the specified <code>String</code>,
		/// using the current text attribute state in the <code>Graphics2D</code> context.
		/// The baseline of the first character is at position
		/// (<i>x</i>,&nbsp;<i>y</i>) in the User Space.
		/// The rendering attributes applied include the <code>Clip</code>,
		/// <code>Transform</code>, <code>Paint</code>, <code>Font</code> and
		/// <code>Composite</code> attributes. For characters in script systems
		/// such as Hebrew and Arabic, the glyphs can be rendered from right to
		/// left, in which case the coordinate supplied is the location of the
		/// leftmost character on the baseline. </summary>
		/// <param name="str"> the <code>String</code> to be rendered </param>
		/// <param name="x"> the x coordinate of the location where the
		/// <code>String</code> should be rendered </param>
		/// <param name="y"> the y coordinate of the location where the
		/// <code>String</code> should be rendered </param>
		/// <exception cref="NullPointerException"> if <code>str</code> is
		///         <code>null</code> </exception>
		/// <seealso cref= #setPaint </seealso>
		/// <seealso cref= java.awt.Graphics#setColor </seealso>
		/// <seealso cref= java.awt.Graphics#setFont </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #setComposite </seealso>
		/// <seealso cref= #setClip </seealso>
		public abstract void DrawString(String str, float x, float y);

		/// <summary>
		/// Renders the text of the specified iterator applying its attributes
		/// in accordance with the specification of the <seealso cref="TextAttribute"/> class.
		/// <para>
		/// The baseline of the first character is at position
		/// (<i>x</i>,&nbsp;<i>y</i>) in User Space.
		/// For characters in script systems such as Hebrew and Arabic,
		/// the glyphs can be rendered from right to left, in which case the
		/// coordinate supplied is the location of the leftmost character
		/// on the baseline.
		/// </para>
		/// </summary>
		/// <param name="iterator"> the iterator whose text is to be rendered </param>
		/// <param name="x"> the x coordinate where the iterator's text is to be
		/// rendered </param>
		/// <param name="y"> the y coordinate where the iterator's text is to be
		/// rendered </param>
		/// <exception cref="NullPointerException"> if <code>iterator</code> is
		///         <code>null</code> </exception>
		/// <seealso cref= #setPaint </seealso>
		/// <seealso cref= java.awt.Graphics#setColor </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #setComposite </seealso>
		/// <seealso cref= #setClip </seealso>
		public override abstract void DrawString(AttributedCharacterIterator iterator, int x, int y);

		/// <summary>
		/// Renders the text of the specified iterator applying its attributes
		/// in accordance with the specification of the <seealso cref="TextAttribute"/> class.
		/// <para>
		/// The baseline of the first character is at position
		/// (<i>x</i>,&nbsp;<i>y</i>) in User Space.
		/// For characters in script systems such as Hebrew and Arabic,
		/// the glyphs can be rendered from right to left, in which case the
		/// coordinate supplied is the location of the leftmost character
		/// on the baseline.
		/// </para>
		/// </summary>
		/// <param name="iterator"> the iterator whose text is to be rendered </param>
		/// <param name="x"> the x coordinate where the iterator's text is to be
		/// rendered </param>
		/// <param name="y"> the y coordinate where the iterator's text is to be
		/// rendered </param>
		/// <exception cref="NullPointerException"> if <code>iterator</code> is
		///         <code>null</code> </exception>
		/// <seealso cref= #setPaint </seealso>
		/// <seealso cref= java.awt.Graphics#setColor </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #setComposite </seealso>
		/// <seealso cref= #setClip </seealso>
		public abstract void DrawString(AttributedCharacterIterator iterator, float x, float y);

		/// <summary>
		/// Renders the text of the specified
		/// <seealso cref="GlyphVector"/> using
		/// the <code>Graphics2D</code> context's rendering attributes.
		/// The rendering attributes applied include the <code>Clip</code>,
		/// <code>Transform</code>, <code>Paint</code>, and
		/// <code>Composite</code> attributes.  The <code>GlyphVector</code>
		/// specifies individual glyphs from a <seealso cref="Font"/>.
		/// The <code>GlyphVector</code> can also contain the glyph positions.
		/// This is the fastest way to render a set of characters to the
		/// screen. </summary>
		/// <param name="g"> the <code>GlyphVector</code> to be rendered </param>
		/// <param name="x"> the x position in User Space where the glyphs should
		/// be rendered </param>
		/// <param name="y"> the y position in User Space where the glyphs should
		/// be rendered </param>
		/// <exception cref="NullPointerException"> if <code>g</code> is <code>null</code>.
		/// </exception>
		/// <seealso cref= java.awt.Font#createGlyphVector </seealso>
		/// <seealso cref= java.awt.font.GlyphVector </seealso>
		/// <seealso cref= #setPaint </seealso>
		/// <seealso cref= java.awt.Graphics#setColor </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #setComposite </seealso>
		/// <seealso cref= #setClip </seealso>
		public abstract void DrawGlyphVector(GlyphVector g, float x, float y);

		/// <summary>
		/// Fills the interior of a <code>Shape</code> using the settings of the
		/// <code>Graphics2D</code> context. The rendering attributes applied
		/// include the <code>Clip</code>, <code>Transform</code>,
		/// <code>Paint</code>, and <code>Composite</code>. </summary>
		/// <param name="s"> the <code>Shape</code> to be filled </param>
		/// <seealso cref= #setPaint </seealso>
		/// <seealso cref= java.awt.Graphics#setColor </seealso>
		/// <seealso cref= #transform </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #setComposite </seealso>
		/// <seealso cref= #clip </seealso>
		/// <seealso cref= #setClip </seealso>
		public abstract void Fill(Shape s);

		/// <summary>
		/// Checks whether or not the specified <code>Shape</code> intersects
		/// the specified <seealso cref="Rectangle"/>, which is in device
		/// space. If <code>onStroke</code> is false, this method checks
		/// whether or not the interior of the specified <code>Shape</code>
		/// intersects the specified <code>Rectangle</code>.  If
		/// <code>onStroke</code> is <code>true</code>, this method checks
		/// whether or not the <code>Stroke</code> of the specified
		/// <code>Shape</code> outline intersects the specified
		/// <code>Rectangle</code>.
		/// The rendering attributes taken into account include the
		/// <code>Clip</code>, <code>Transform</code>, and <code>Stroke</code>
		/// attributes. </summary>
		/// <param name="rect"> the area in device space to check for a hit </param>
		/// <param name="s"> the <code>Shape</code> to check for a hit </param>
		/// <param name="onStroke"> flag used to choose between testing the
		/// stroked or the filled shape.  If the flag is <code>true</code>, the
		/// <code>Stroke</code> outline is tested.  If the flag is
		/// <code>false</code>, the filled <code>Shape</code> is tested. </param>
		/// <returns> <code>true</code> if there is a hit; <code>false</code>
		/// otherwise. </returns>
		/// <seealso cref= #setStroke </seealso>
		/// <seealso cref= #fill </seealso>
		/// <seealso cref= #draw </seealso>
		/// <seealso cref= #transform </seealso>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= #clip </seealso>
		/// <seealso cref= #setClip </seealso>
		public abstract bool Hit(Rectangle rect, Shape s, bool onStroke);

		/// <summary>
		/// Returns the device configuration associated with this
		/// <code>Graphics2D</code>. </summary>
		/// <returns> the device configuration of this <code>Graphics2D</code>. </returns>
		public abstract GraphicsConfiguration DeviceConfiguration {get;}

		/// <summary>
		/// Sets the <code>Composite</code> for the <code>Graphics2D</code> context.
		/// The <code>Composite</code> is used in all drawing methods such as
		/// <code>drawImage</code>, <code>drawString</code>, <code>draw</code>,
		/// and <code>fill</code>.  It specifies how new pixels are to be combined
		/// with the existing pixels on the graphics device during the rendering
		/// process.
		/// <para>If this <code>Graphics2D</code> context is drawing to a
		/// <code>Component</code> on the display screen and the
		/// <code>Composite</code> is a custom object rather than an
		/// instance of the <code>AlphaComposite</code> class, and if
		/// there is a security manager, its <code>checkPermission</code>
		/// method is called with an <code>AWTPermission("readDisplayPixels")</code>
		/// permission.
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">
		///         if a custom <code>Composite</code> object is being
		///         used to render to the screen and a security manager
		///         is set and its <code>checkPermission</code> method
		///         does not allow the operation. </exception>
		/// <param name="comp"> the <code>Composite</code> object to be used for rendering </param>
		/// <seealso cref= java.awt.Graphics#setXORMode </seealso>
		/// <seealso cref= java.awt.Graphics#setPaintMode </seealso>
		/// <seealso cref= #getComposite </seealso>
		/// <seealso cref= AlphaComposite </seealso>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= java.awt.AWTPermission </seealso>
		public abstract Composite Composite {set;get;}

		/// <summary>
		/// Sets the <code>Paint</code> attribute for the
		/// <code>Graphics2D</code> context.  Calling this method
		/// with a <code>null</code> <code>Paint</code> object does
		/// not have any effect on the current <code>Paint</code> attribute
		/// of this <code>Graphics2D</code>. </summary>
		/// <param name="paint"> the <code>Paint</code> object to be used to generate
		/// color during the rendering process, or <code>null</code> </param>
		/// <seealso cref= java.awt.Graphics#setColor </seealso>
		/// <seealso cref= #getPaint </seealso>
		/// <seealso cref= GradientPaint </seealso>
		/// <seealso cref= TexturePaint </seealso>
		public abstract Paint Paint {set;get;}

		/// <summary>
		/// Sets the <code>Stroke</code> for the <code>Graphics2D</code> context. </summary>
		/// <param name="s"> the <code>Stroke</code> object to be used to stroke a
		/// <code>Shape</code> during the rendering process </param>
		/// <seealso cref= BasicStroke </seealso>
		/// <seealso cref= #getStroke </seealso>
		public abstract Stroke Stroke {set;get;}

		/// <summary>
		/// Sets the value of a single preference for the rendering algorithms.
		/// Hint categories include controls for rendering quality and overall
		/// time/quality trade-off in the rendering process.  Refer to the
		/// <code>RenderingHints</code> class for definitions of some common
		/// keys and values. </summary>
		/// <param name="hintKey"> the key of the hint to be set. </param>
		/// <param name="hintValue"> the value indicating preferences for the specified
		/// hint category. </param>
		/// <seealso cref= #getRenderingHint(RenderingHints.Key) </seealso>
		/// <seealso cref= RenderingHints </seealso>
		public abstract void SetRenderingHint(Key hintKey, Object hintValue);

		/// <summary>
		/// Returns the value of a single preference for the rendering algorithms.
		/// Hint categories include controls for rendering quality and overall
		/// time/quality trade-off in the rendering process.  Refer to the
		/// <code>RenderingHints</code> class for definitions of some common
		/// keys and values. </summary>
		/// <param name="hintKey"> the key corresponding to the hint to get. </param>
		/// <returns> an object representing the value for the specified hint key.
		/// Some of the keys and their associated values are defined in the
		/// <code>RenderingHints</code> class. </returns>
		/// <seealso cref= RenderingHints </seealso>
		/// <seealso cref= #setRenderingHint(RenderingHints.Key, Object) </seealso>
		public abstract Object GetRenderingHint(Key hintKey);

		/// <summary>
		/// Replaces the values of all preferences for the rendering
		/// algorithms with the specified <code>hints</code>.
		/// The existing values for all rendering hints are discarded and
		/// the new set of known hints and values are initialized from the
		/// specified <seealso cref="Map"/> object.
		/// Hint categories include controls for rendering quality and
		/// overall time/quality trade-off in the rendering process.
		/// Refer to the <code>RenderingHints</code> class for definitions of
		/// some common keys and values. </summary>
		/// <param name="hints"> the rendering hints to be set </param>
		/// <seealso cref= #getRenderingHints </seealso>
		/// <seealso cref= RenderingHints </seealso>
		public abstract void setRenderingHints<T1>(IDictionary<T1> hints);

		/// <summary>
		/// Sets the values of an arbitrary number of preferences for the
		/// rendering algorithms.
		/// Only values for the rendering hints that are present in the
		/// specified <code>Map</code> object are modified.
		/// All other preferences not present in the specified
		/// object are left unmodified.
		/// Hint categories include controls for rendering quality and
		/// overall time/quality trade-off in the rendering process.
		/// Refer to the <code>RenderingHints</code> class for definitions of
		/// some common keys and values. </summary>
		/// <param name="hints"> the rendering hints to be set </param>
		/// <seealso cref= RenderingHints </seealso>
		public abstract void addRenderingHints<T1>(IDictionary<T1> hints);

		/// <summary>
		/// Gets the preferences for the rendering algorithms.  Hint categories
		/// include controls for rendering quality and overall time/quality
		/// trade-off in the rendering process.
		/// Returns all of the hint key/value pairs that were ever specified in
		/// one operation.  Refer to the
		/// <code>RenderingHints</code> class for definitions of some common
		/// keys and values. </summary>
		/// <returns> a reference to an instance of <code>RenderingHints</code>
		/// that contains the current preferences. </returns>
		/// <seealso cref= RenderingHints </seealso>
		/// <seealso cref= #setRenderingHints(Map) </seealso>
		public abstract RenderingHints GetRenderingHints();

		/// <summary>
		/// Translates the origin of the <code>Graphics2D</code> context to the
		/// point (<i>x</i>,&nbsp;<i>y</i>) in the current coordinate system.
		/// Modifies the <code>Graphics2D</code> context so that its new origin
		/// corresponds to the point (<i>x</i>,&nbsp;<i>y</i>) in the
		/// <code>Graphics2D</code> context's former coordinate system.  All
		/// coordinates used in subsequent rendering operations on this graphics
		/// context are relative to this new origin. </summary>
		/// <param name="x"> the specified x coordinate </param>
		/// <param name="y"> the specified y coordinate
		/// @since   JDK1.0 </param>
		public override abstract void Translate(int x, int y);

		/// <summary>
		/// Concatenates the current
		/// <code>Graphics2D</code> <code>Transform</code>
		/// with a translation transform.
		/// Subsequent rendering is translated by the specified
		/// distance relative to the previous position.
		/// This is equivalent to calling transform(T), where T is an
		/// <code>AffineTransform</code> represented by the following matrix:
		/// <pre>
		///          [   1    0    tx  ]
		///          [   0    1    ty  ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="tx"> the distance to translate along the x-axis </param>
		/// <param name="ty"> the distance to translate along the y-axis </param>
		public abstract void Translate(double tx, double ty);

		/// <summary>
		/// Concatenates the current <code>Graphics2D</code>
		/// <code>Transform</code> with a rotation transform.
		/// Subsequent rendering is rotated by the specified radians relative
		/// to the previous origin.
		/// This is equivalent to calling <code>transform(R)</code>, where R is an
		/// <code>AffineTransform</code> represented by the following matrix:
		/// <pre>
		///          [   cos(theta)    -sin(theta)    0   ]
		///          [   sin(theta)     cos(theta)    0   ]
		///          [       0              0         1   ]
		/// </pre>
		/// Rotating with a positive angle theta rotates points on the positive
		/// x axis toward the positive y axis. </summary>
		/// <param name="theta"> the angle of rotation in radians </param>
		public abstract void Rotate(double theta);

		/// <summary>
		/// Concatenates the current <code>Graphics2D</code>
		/// <code>Transform</code> with a translated rotation
		/// transform.  Subsequent rendering is transformed by a transform
		/// which is constructed by translating to the specified location,
		/// rotating by the specified radians, and translating back by the same
		/// amount as the original translation.  This is equivalent to the
		/// following sequence of calls:
		/// <pre>
		///          translate(x, y);
		///          rotate(theta);
		///          translate(-x, -y);
		/// </pre>
		/// Rotating with a positive angle theta rotates points on the positive
		/// x axis toward the positive y axis. </summary>
		/// <param name="theta"> the angle of rotation in radians </param>
		/// <param name="x"> the x coordinate of the origin of the rotation </param>
		/// <param name="y"> the y coordinate of the origin of the rotation </param>
		public abstract void Rotate(double theta, double x, double y);

		/// <summary>
		/// Concatenates the current <code>Graphics2D</code>
		/// <code>Transform</code> with a scaling transformation
		/// Subsequent rendering is resized according to the specified scaling
		/// factors relative to the previous scaling.
		/// This is equivalent to calling <code>transform(S)</code>, where S is an
		/// <code>AffineTransform</code> represented by the following matrix:
		/// <pre>
		///          [   sx   0    0   ]
		///          [   0    sy   0   ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="sx"> the amount by which X coordinates in subsequent
		/// rendering operations are multiplied relative to previous
		/// rendering operations. </param>
		/// <param name="sy"> the amount by which Y coordinates in subsequent
		/// rendering operations are multiplied relative to previous
		/// rendering operations. </param>
		public abstract void Scale(double sx, double sy);

		/// <summary>
		/// Concatenates the current <code>Graphics2D</code>
		/// <code>Transform</code> with a shearing transform.
		/// Subsequent renderings are sheared by the specified
		/// multiplier relative to the previous position.
		/// This is equivalent to calling <code>transform(SH)</code>, where SH
		/// is an <code>AffineTransform</code> represented by the following
		/// matrix:
		/// <pre>
		///          [   1   shx   0   ]
		///          [  shy   1    0   ]
		///          [   0    0    1   ]
		/// </pre> </summary>
		/// <param name="shx"> the multiplier by which coordinates are shifted in
		/// the positive X axis direction as a function of their Y coordinate </param>
		/// <param name="shy"> the multiplier by which coordinates are shifted in
		/// the positive Y axis direction as a function of their X coordinate </param>
		public abstract void Shear(double shx, double shy);

		/// <summary>
		/// Composes an <code>AffineTransform</code> object with the
		/// <code>Transform</code> in this <code>Graphics2D</code> according
		/// to the rule last-specified-first-applied.  If the current
		/// <code>Transform</code> is Cx, the result of composition
		/// with Tx is a new <code>Transform</code> Cx'.  Cx' becomes the
		/// current <code>Transform</code> for this <code>Graphics2D</code>.
		/// Transforming a point p by the updated <code>Transform</code> Cx' is
		/// equivalent to first transforming p by Tx and then transforming
		/// the result by the original <code>Transform</code> Cx.  In other
		/// words, Cx'(p) = Cx(Tx(p)).  A copy of the Tx is made, if necessary,
		/// so further modifications to Tx do not affect rendering. </summary>
		/// <param name="Tx"> the <code>AffineTransform</code> object to be composed with
		/// the current <code>Transform</code> </param>
		/// <seealso cref= #setTransform </seealso>
		/// <seealso cref= AffineTransform </seealso>
		public abstract void Transform(AffineTransform Tx);

		/// <summary>
		/// Overwrites the Transform in the <code>Graphics2D</code> context.
		/// WARNING: This method should <b>never</b> be used to apply a new
		/// coordinate transform on top of an existing transform because the
		/// <code>Graphics2D</code> might already have a transform that is
		/// needed for other purposes, such as rendering Swing
		/// components or applying a scaling transformation to adjust for the
		/// resolution of a printer.
		/// <para>To add a coordinate transform, use the
		/// <code>transform</code>, <code>rotate</code>, <code>scale</code>,
		/// or <code>shear</code> methods.  The <code>setTransform</code>
		/// method is intended only for restoring the original
		/// <code>Graphics2D</code> transform after rendering, as shown in this
		/// example:
		/// <pre>
		/// // Get the current transform
		/// AffineTransform saveAT = g2.getTransform();
		/// // Perform transformation
		/// g2d.transform(...);
		/// // Render
		/// g2d.draw(...);
		/// // Restore original transform
		/// g2d.setTransform(saveAT);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="Tx"> the <code>AffineTransform</code> that was retrieved
		///           from the <code>getTransform</code> method </param>
		/// <seealso cref= #transform </seealso>
		/// <seealso cref= #getTransform </seealso>
		/// <seealso cref= AffineTransform </seealso>
		public abstract AffineTransform Transform {set;get;}




		/// <summary>
		/// Sets the background color for the <code>Graphics2D</code> context.
		/// The background color is used for clearing a region.
		/// When a <code>Graphics2D</code> is constructed for a
		/// <code>Component</code>, the background color is
		/// inherited from the <code>Component</code>. Setting the background color
		/// in the <code>Graphics2D</code> context only affects the subsequent
		/// <code>clearRect</code> calls and not the background color of the
		/// <code>Component</code>.  To change the background
		/// of the <code>Component</code>, use appropriate methods of
		/// the <code>Component</code>. </summary>
		/// <param name="color"> the background color that is used in
		/// subsequent calls to <code>clearRect</code> </param>
		/// <seealso cref= #getBackground </seealso>
		/// <seealso cref= java.awt.Graphics#clearRect </seealso>
		public abstract Color Background {set;get;}



		/// <summary>
		/// Intersects the current <code>Clip</code> with the interior of the
		/// specified <code>Shape</code> and sets the <code>Clip</code> to the
		/// resulting intersection.  The specified <code>Shape</code> is
		/// transformed with the current <code>Graphics2D</code>
		/// <code>Transform</code> before being intersected with the current
		/// <code>Clip</code>.  This method is used to make the current
		/// <code>Clip</code> smaller.
		/// To make the <code>Clip</code> larger, use <code>setClip</code>.
		/// The <i>user clip</i> modified by this method is independent of the
		/// clipping associated with device bounds and visibility.  If no clip has
		/// previously been set, or if the clip has been cleared using
		/// <seealso cref="Graphics#setClip(Shape) setClip"/> with a <code>null</code>
		/// argument, the specified <code>Shape</code> becomes the new
		/// user clip. </summary>
		/// <param name="s"> the <code>Shape</code> to be intersected with the current
		///          <code>Clip</code>.  If <code>s</code> is <code>null</code>,
		///          this method clears the current <code>Clip</code>. </param>
		 public abstract void Clip(Shape s);

		 /// <summary>
		 /// Get the rendering context of the <code>Font</code> within this
		 /// <code>Graphics2D</code> context.
		 /// The <seealso cref="FontRenderContext"/>
		 /// encapsulates application hints such as anti-aliasing and
		 /// fractional metrics, as well as target device specific information
		 /// such as dots-per-inch.  This information should be provided by the
		 /// application when using objects that perform typographical
		 /// formatting, such as <code>Font</code> and
		 /// <code>TextLayout</code>.  This information should also be provided
		 /// by applications that perform their own layout and need accurate
		 /// measurements of various characteristics of glyphs such as advance
		 /// and line height when various rendering hints have been applied to
		 /// the text rendering.
		 /// </summary>
		 /// <returns> a reference to an instance of FontRenderContext. </returns>
		 /// <seealso cref= java.awt.font.FontRenderContext </seealso>
		 /// <seealso cref= java.awt.Font#createGlyphVector </seealso>
		 /// <seealso cref= java.awt.font.TextLayout
		 /// @since     1.2 </seealso>

		public abstract FontRenderContext FontRenderContext {get;}

	}

}