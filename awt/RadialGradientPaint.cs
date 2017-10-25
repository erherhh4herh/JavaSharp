/*
 * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The {@code RadialGradientPaint} class provides a way to fill a shape with
	/// a circular radial color gradient pattern. The user may specify 2 or more
	/// gradient colors, and this paint will provide an interpolation between
	/// each color.
	/// <para>
	/// The user must specify the circle controlling the gradient pattern,
	/// which is described by a center point and a radius.  The user can also
	/// specify a separate focus point within that circle, which controls the
	/// location of the first color of the gradient.  By default the focus is
	/// set to be the center of the circle.
	/// </para>
	/// <para>
	/// This paint will map the first color of the gradient to the focus point,
	/// and the last color to the perimeter of the circle, interpolating
	/// smoothly for any in-between colors specified by the user.  Any line drawn
	/// from the focus point to the circumference will thus span all the gradient
	/// colors.
	/// </para>
	/// <para>
	/// Specifying a focus point outside of the radius of the circle will cause
	/// the rings of the gradient pattern to be centered on the point just inside
	/// the edge of the circle in the direction of the focus point.
	/// The rendering will internally use this modified location as if it were
	/// the specified focus point.
	/// </para>
	/// <para>
	/// The user must provide an array of floats specifying how to distribute the
	/// colors along the gradient.  These values should range from 0.0 to 1.0 and
	/// act like keyframes along the gradient (they mark where the gradient should
	/// be exactly a particular color).
	/// </para>
	/// <para>
	/// In the event that the user does not set the first keyframe value equal
	/// to 0 and/or the last keyframe value equal to 1, keyframes will be created
	/// at these positions and the first and last colors will be replicated there.
	/// So, if a user specifies the following arrays to construct a gradient:<br>
	/// <pre>
	///     {Color.BLUE, Color.RED}, {.3f, .7f}
	/// </pre>
	/// this will be converted to a gradient with the following keyframes:<br>
	/// <pre>
	///     {Color.BLUE, Color.BLUE, Color.RED, Color.RED}, {0f, .3f, .7f, 1f}
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// The user may also select what action the {@code RadialGradientPaint} object
	/// takes when it is filling the space outside the circle's radius by
	/// setting {@code CycleMethod} to either {@code REFLECTION} or {@code REPEAT}.
	/// The gradient color proportions are equal for any particular line drawn
	/// from the focus point. The following figure shows that the distance AB
	/// is equal to the distance BC, and the distance AD is equal to the distance DE.
	/// <center>
	/// <img src = "doc-files/RadialGradientPaint-3.png" alt="image showing the
	/// distance AB=BC, and AD=DE">
	/// </center>
	/// If the gradient and graphics rendering transforms are uniformly scaled and
	/// the user sets the focus so that it coincides with the center of the circle,
	/// the gradient color proportions are equal for any line drawn from the center.
	/// The following figure shows the distances AB, BC, AD, and DE. They are all equal.
	/// <center>
	/// <img src = "doc-files/RadialGradientPaint-4.png" alt="image showing the
	/// distance of AB, BC, AD, and DE are all equal">
	/// </center>
	/// Note that some minor variations in distances may occur due to sampling at
	/// the granularity of a pixel.
	/// If no cycle method is specified, {@code NO_CYCLE} will be chosen by
	/// default, which means the the last keyframe color will be used to fill the
	/// remaining area.
	/// </para>
	/// <para>
	/// The colorSpace parameter allows the user to specify in which colorspace
	/// the interpolation should be performed, default sRGB or linearized RGB.
	/// 
	/// </para>
	/// <para>
	/// The following code demonstrates typical usage of
	/// {@code RadialGradientPaint}, where the center and focus points are
	/// the same:
	/// <pre>
	///     Point2D center = new Point2D.Float(50, 50);
	///     float radius = 25;
	///     float[] dist = {0.0f, 0.2f, 1.0f};
	///     Color[] colors = {Color.RED, Color.WHITE, Color.BLUE};
	///     RadialGradientPaint p =
	///         new RadialGradientPaint(center, radius, dist, colors);
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// This image demonstrates the example code above, with default
	/// (centered) focus for each of the three cycle methods:
	/// <center>
	/// <img src = "doc-files/RadialGradientPaint-1.png" alt="image showing the
	/// output of the sameple code">
	/// </center>
	/// 
	/// </para>
	/// <para>
	/// It is also possible to specify a non-centered focus point, as
	/// in the following code:
	/// <pre>
	///     Point2D center = new Point2D.Float(50, 50);
	///     float radius = 25;
	///     Point2D focus = new Point2D.Float(40, 40);
	///     float[] dist = {0.0f, 0.2f, 1.0f};
	///     Color[] colors = {Color.RED, Color.WHITE, Color.BLUE};
	///     RadialGradientPaint p =
	///         new RadialGradientPaint(center, radius, focus,
	///                                 dist, colors,
	///                                 CycleMethod.NO_CYCLE);
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// This image demonstrates the previous example code, with non-centered
	/// focus for each of the three cycle methods:
	/// <center>
	/// <img src = "doc-files/RadialGradientPaint-2.png" alt="image showing the
	/// output of the sample code">
	/// </center>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.Paint </seealso>
	/// <seealso cref= java.awt.Graphics2D#setPaint
	/// @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans
	/// @since 1.6 </seealso>
	public sealed class RadialGradientPaint : MultipleGradientPaint
	{

		/// <summary>
		/// Focus point which defines the 0% gradient stop X coordinate. </summary>
		private readonly Point2D Focus;

		/// <summary>
		/// Center of the circle defining the 100% gradient stop X coordinate. </summary>
		private readonly Point2D Center;

		/// <summary>
		/// Radius of the outermost circle defining the 100% gradient stop. </summary>
		private readonly float Radius_Renamed;

		/// <summary>
		/// Constructs a {@code RadialGradientPaint} with a default
		/// {@code NO_CYCLE} repeating method and {@code SRGB} color space,
		/// using the center as the focus point.
		/// </summary>
		/// <param name="cx"> the X coordinate in user space of the center point of the
		///           circle defining the gradient.  The last color of the
		///           gradient is mapped to the perimeter of this circle. </param>
		/// <param name="cy"> the Y coordinate in user space of the center point of the
		///           circle defining the gradient.  The last color of the
		///           gradient is mapped to the perimeter of this circle. </param>
		/// <param name="radius"> the radius of the circle defining the extents of the
		///               color gradient </param>
		/// <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		///                  distribution of colors along the gradient </param>
		/// <param name="colors"> array of colors to use in the gradient.  The first color
		///               is used at the focus point, the last color around the
		///               perimeter of the circle.
		/// </param>
		/// <exception cref="NullPointerException">
		/// if {@code fractions} array is null,
		/// or {@code colors} array is null </exception>
		/// <exception cref="IllegalArgumentException">
		/// if {@code radius} is non-positive,
		/// or {@code fractions.length != colors.length},
		/// or {@code colors} is less than 2 in size,
		/// or a {@code fractions} value is less than 0.0 or greater than 1.0,
		/// or the {@code fractions} are not provided in strictly increasing order </exception>
		public RadialGradientPaint(float cx, float cy, float radius, float[] fractions, Color[] colors) : this(cx, cy, radius, cx, cy, fractions, colors, CycleMethod.NO_CYCLE)
		{
		}

		/// <summary>
		/// Constructs a {@code RadialGradientPaint} with a default
		/// {@code NO_CYCLE} repeating method and {@code SRGB} color space,
		/// using the center as the focus point.
		/// </summary>
		/// <param name="center"> the center point, in user space, of the circle defining
		///               the gradient </param>
		/// <param name="radius"> the radius of the circle defining the extents of the
		///               color gradient </param>
		/// <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		///                  distribution of colors along the gradient </param>
		/// <param name="colors"> array of colors to use in the gradient.  The first color
		///               is used at the focus point, the last color around the
		///               perimeter of the circle.
		/// </param>
		/// <exception cref="NullPointerException">
		/// if {@code center} point is null,
		/// or {@code fractions} array is null,
		/// or {@code colors} array is null </exception>
		/// <exception cref="IllegalArgumentException">
		/// if {@code radius} is non-positive,
		/// or {@code fractions.length != colors.length},
		/// or {@code colors} is less than 2 in size,
		/// or a {@code fractions} value is less than 0.0 or greater than 1.0,
		/// or the {@code fractions} are not provided in strictly increasing order </exception>
		public RadialGradientPaint(Point2D center, float radius, float[] fractions, Color[] colors) : this(center, radius, center, fractions, colors, CycleMethod.NO_CYCLE)
		{
		}

		/// <summary>
		/// Constructs a {@code RadialGradientPaint} with a default
		/// {@code SRGB} color space, using the center as the focus point.
		/// </summary>
		/// <param name="cx"> the X coordinate in user space of the center point of the
		///           circle defining the gradient.  The last color of the
		///           gradient is mapped to the perimeter of this circle. </param>
		/// <param name="cy"> the Y coordinate in user space of the center point of the
		///           circle defining the gradient.  The last color of the
		///           gradient is mapped to the perimeter of this circle. </param>
		/// <param name="radius"> the radius of the circle defining the extents of the
		///               color gradient </param>
		/// <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		///                  distribution of colors along the gradient </param>
		/// <param name="colors"> array of colors to use in the gradient.  The first color
		///               is used at the focus point, the last color around the
		///               perimeter of the circle. </param>
		/// <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		///                    or {@code REPEAT}
		/// </param>
		/// <exception cref="NullPointerException">
		/// if {@code fractions} array is null,
		/// or {@code colors} array is null,
		/// or {@code cycleMethod} is null </exception>
		/// <exception cref="IllegalArgumentException">
		/// if {@code radius} is non-positive,
		/// or {@code fractions.length != colors.length},
		/// or {@code colors} is less than 2 in size,
		/// or a {@code fractions} value is less than 0.0 or greater than 1.0,
		/// or the {@code fractions} are not provided in strictly increasing order </exception>
		public RadialGradientPaint(float cx, float cy, float radius, float[] fractions, Color[] colors, CycleMethod cycleMethod) : this(cx, cy, radius, cx, cy, fractions, colors, cycleMethod)
		{
		}

		/// <summary>
		/// Constructs a {@code RadialGradientPaint} with a default
		/// {@code SRGB} color space, using the center as the focus point.
		/// </summary>
		/// <param name="center"> the center point, in user space, of the circle defining
		///               the gradient </param>
		/// <param name="radius"> the radius of the circle defining the extents of the
		///               color gradient </param>
		/// <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		///                  distribution of colors along the gradient </param>
		/// <param name="colors"> array of colors to use in the gradient.  The first color
		///               is used at the focus point, the last color around the
		///               perimeter of the circle. </param>
		/// <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		///                    or {@code REPEAT}
		/// </param>
		/// <exception cref="NullPointerException">
		/// if {@code center} point is null,
		/// or {@code fractions} array is null,
		/// or {@code colors} array is null,
		/// or {@code cycleMethod} is null </exception>
		/// <exception cref="IllegalArgumentException">
		/// if {@code radius} is non-positive,
		/// or {@code fractions.length != colors.length},
		/// or {@code colors} is less than 2 in size,
		/// or a {@code fractions} value is less than 0.0 or greater than 1.0,
		/// or the {@code fractions} are not provided in strictly increasing order </exception>
		public RadialGradientPaint(Point2D center, float radius, float[] fractions, Color[] colors, CycleMethod cycleMethod) : this(center, radius, center, fractions, colors, cycleMethod)
		{
		}

		/// <summary>
		/// Constructs a {@code RadialGradientPaint} with a default
		/// {@code SRGB} color space.
		/// </summary>
		/// <param name="cx"> the X coordinate in user space of the center point of the
		///           circle defining the gradient.  The last color of the
		///           gradient is mapped to the perimeter of this circle. </param>
		/// <param name="cy"> the Y coordinate in user space of the center point of the
		///           circle defining the gradient.  The last color of the
		///           gradient is mapped to the perimeter of this circle. </param>
		/// <param name="radius"> the radius of the circle defining the extents of the
		///               color gradient </param>
		/// <param name="fx"> the X coordinate of the point in user space to which the
		///           first color is mapped </param>
		/// <param name="fy"> the Y coordinate of the point in user space to which the
		///           first color is mapped </param>
		/// <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		///                  distribution of colors along the gradient </param>
		/// <param name="colors"> array of colors to use in the gradient.  The first color
		///               is used at the focus point, the last color around the
		///               perimeter of the circle. </param>
		/// <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		///                    or {@code REPEAT}
		/// </param>
		/// <exception cref="NullPointerException">
		/// if {@code fractions} array is null,
		/// or {@code colors} array is null,
		/// or {@code cycleMethod} is null </exception>
		/// <exception cref="IllegalArgumentException">
		/// if {@code radius} is non-positive,
		/// or {@code fractions.length != colors.length},
		/// or {@code colors} is less than 2 in size,
		/// or a {@code fractions} value is less than 0.0 or greater than 1.0,
		/// or the {@code fractions} are not provided in strictly increasing order </exception>
		public RadialGradientPaint(float cx, float cy, float radius, float fx, float fy, float[] fractions, Color[] colors, CycleMethod cycleMethod) : this(new Point2D.Float(cx, cy), radius, new Point2D.Float(fx, fy), fractions, colors, cycleMethod)
		{
		}

		/// <summary>
		/// Constructs a {@code RadialGradientPaint} with a default
		/// {@code SRGB} color space.
		/// </summary>
		/// <param name="center"> the center point, in user space, of the circle defining
		///               the gradient.  The last color of the gradient is mapped
		///               to the perimeter of this circle. </param>
		/// <param name="radius"> the radius of the circle defining the extents of the color
		///               gradient </param>
		/// <param name="focus"> the point in user space to which the first color is mapped </param>
		/// <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		///                  distribution of colors along the gradient </param>
		/// <param name="colors"> array of colors to use in the gradient. The first color
		///               is used at the focus point, the last color around the
		///               perimeter of the circle. </param>
		/// <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		///                    or {@code REPEAT}
		/// </param>
		/// <exception cref="NullPointerException">
		/// if one of the points is null,
		/// or {@code fractions} array is null,
		/// or {@code colors} array is null,
		/// or {@code cycleMethod} is null </exception>
		/// <exception cref="IllegalArgumentException">
		/// if {@code radius} is non-positive,
		/// or {@code fractions.length != colors.length},
		/// or {@code colors} is less than 2 in size,
		/// or a {@code fractions} value is less than 0.0 or greater than 1.0,
		/// or the {@code fractions} are not provided in strictly increasing order </exception>
		public RadialGradientPaint(Point2D center, float radius, Point2D focus, float[] fractions, Color[] colors, CycleMethod cycleMethod) : this(center, radius, focus, fractions, colors, cycleMethod, ColorSpaceType.SRGB, new AffineTransform())
		{
		}

		/// <summary>
		/// Constructs a {@code RadialGradientPaint}.
		/// </summary>
		/// <param name="center"> the center point in user space of the circle defining the
		///               gradient.  The last color of the gradient is mapped to
		///               the perimeter of this circle. </param>
		/// <param name="radius"> the radius of the circle defining the extents of the
		///               color gradient </param>
		/// <param name="focus"> the point in user space to which the first color is mapped </param>
		/// <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		///                  distribution of colors along the gradient </param>
		/// <param name="colors"> array of colors to use in the gradient.  The first color
		///               is used at the focus point, the last color around the
		///               perimeter of the circle. </param>
		/// <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		///                    or {@code REPEAT} </param>
		/// <param name="colorSpace"> which color space to use for interpolation,
		///                   either {@code SRGB} or {@code LINEAR_RGB} </param>
		/// <param name="gradientTransform"> transform to apply to the gradient
		/// </param>
		/// <exception cref="NullPointerException">
		/// if one of the points is null,
		/// or {@code fractions} array is null,
		/// or {@code colors} array is null,
		/// or {@code cycleMethod} is null,
		/// or {@code colorSpace} is null,
		/// or {@code gradientTransform} is null </exception>
		/// <exception cref="IllegalArgumentException">
		/// if {@code radius} is non-positive,
		/// or {@code fractions.length != colors.length},
		/// or {@code colors} is less than 2 in size,
		/// or a {@code fractions} value is less than 0.0 or greater than 1.0,
		/// or the {@code fractions} are not provided in strictly increasing order </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({ "centerPoint", "radius", "focusPoint", "fractions", "colors", "cycleMethod", "colorSpace", "transform" }) public RadialGradientPaint(java.awt.geom.Point2D center, float radius, java.awt.geom.Point2D focus, float[] fractions, Color[] colors, CycleMethod cycleMethod, ColorSpaceType colorSpace, java.awt.geom.AffineTransform gradientTransform)
		public RadialGradientPaint(Point2D center, float radius, Point2D focus, float[] fractions, Color[] colors, CycleMethod cycleMethod, ColorSpaceType colorSpace, AffineTransform gradientTransform) : base(fractions, colors, cycleMethod, colorSpace, gradientTransform)
		{

			// check input arguments
			if (center == null)
			{
				throw new NullPointerException("Center point must be non-null");
			}

			if (focus == null)
			{
				throw new NullPointerException("Focus point must be non-null");
			}

			if (radius <= 0)
			{
				throw new IllegalArgumentException("Radius must be greater " + "than zero");
			}

			// copy parameters
			this.Center = new Point2D.Double(center.X, center.Y);
			this.Focus = new Point2D.Double(focus.X, focus.Y);
			this.Radius_Renamed = radius;
		}

		/// <summary>
		/// Constructs a {@code RadialGradientPaint} with a default
		/// {@code SRGB} color space.
		/// The gradient circle of the {@code RadialGradientPaint} is defined
		/// by the given bounding box.
		/// <para>
		/// This constructor is a more convenient way to express the
		/// following (equivalent) code:<br>
		/// 
		/// <pre>
		///     double gw = gradientBounds.getWidth();
		///     double gh = gradientBounds.getHeight();
		///     double cx = gradientBounds.getCenterX();
		///     double cy = gradientBounds.getCenterY();
		///     Point2D center = new Point2D.Double(cx, cy);
		/// 
		///     AffineTransform gradientTransform = new AffineTransform();
		///     gradientTransform.translate(cx, cy);
		///     gradientTransform.scale(gw / 2, gh / 2);
		///     gradientTransform.translate(-cx, -cy);
		/// 
		///     RadialGradientPaint gp =
		///         new RadialGradientPaint(center, 1.0f, center,
		///                                 fractions, colors,
		///                                 cycleMethod,
		///                                 ColorSpaceType.SRGB,
		///                                 gradientTransform);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="gradientBounds"> the bounding box, in user space, of the circle
		///                       defining the outermost extent of the gradient </param>
		/// <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		///                  distribution of colors along the gradient </param>
		/// <param name="colors"> array of colors to use in the gradient.  The first color
		///               is used at the focus point, the last color around the
		///               perimeter of the circle. </param>
		/// <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		///                    or {@code REPEAT}
		/// </param>
		/// <exception cref="NullPointerException">
		/// if {@code gradientBounds} is null,
		/// or {@code fractions} array is null,
		/// or {@code colors} array is null,
		/// or {@code cycleMethod} is null </exception>
		/// <exception cref="IllegalArgumentException">
		/// if {@code gradientBounds} is empty,
		/// or {@code fractions.length != colors.length},
		/// or {@code colors} is less than 2 in size,
		/// or a {@code fractions} value is less than 0.0 or greater than 1.0,
		/// or the {@code fractions} are not provided in strictly increasing order </exception>
		public RadialGradientPaint(Rectangle2D gradientBounds, float[] fractions, Color[] colors, CycleMethod cycleMethod) : this(new Point2D.Double(gradientBounds.CenterX, gradientBounds.CenterY), 1.0f, new Point2D.Double(gradientBounds.CenterX, gradientBounds.CenterY), fractions, colors, cycleMethod, ColorSpaceType.SRGB, CreateGradientTransform(gradientBounds))
		{
			// gradient center/focal point is the center of the bounding box,
			// radius is set to 1.0, and then we set a scale transform
			// to achieve an elliptical gradient defined by the bounding box

			if (gradientBounds.Empty)
			{
				throw new IllegalArgumentException("Gradient bounds must be " + "non-empty");
			}
		}

		private static AffineTransform CreateGradientTransform(Rectangle2D r)
		{
			double cx = r.CenterX;
			double cy = r.CenterY;
			AffineTransform xform = AffineTransform.GetTranslateInstance(cx, cy);
			xform.Scale(r.Width / 2, r.Height / 2);
			xform.Translate(-cx, -cy);
			return xform;
		}

		/// <summary>
		/// Creates and returns a <seealso cref="PaintContext"/> used to
		/// generate a circular radial color gradient pattern.
		/// See the description of the <seealso cref="Paint#createContext createContext"/> method
		/// for information on null parameter handling.
		/// </summary>
		/// <param name="cm"> the preferred <seealso cref="ColorModel"/> which represents the most convenient
		///           format for the caller to receive the pixel data, or {@code null}
		///           if there is no preference. </param>
		/// <param name="deviceBounds"> the device space bounding box
		///                     of the graphics primitive being rendered. </param>
		/// <param name="userBounds"> the user space bounding box
		///                   of the graphics primitive being rendered. </param>
		/// <param name="transform"> the <seealso cref="AffineTransform"/> from user
		///              space into device space. </param>
		/// <param name="hints"> the set of hints that the context object can use to
		///              choose between rendering alternatives. </param>
		/// <returns> the {@code PaintContext} for
		///         generating color patterns. </returns>
		/// <seealso cref= Paint </seealso>
		/// <seealso cref= PaintContext </seealso>
		/// <seealso cref= ColorModel </seealso>
		/// <seealso cref= Rectangle </seealso>
		/// <seealso cref= Rectangle2D </seealso>
		/// <seealso cref= AffineTransform </seealso>
		/// <seealso cref= RenderingHints </seealso>
		public override PaintContext CreateContext(ColorModel cm, Rectangle deviceBounds, Rectangle2D userBounds, AffineTransform transform, RenderingHints hints)
		{
			// avoid modifying the user's transform...
			transform = new AffineTransform(transform);
			// incorporate the gradient transform
			transform.Concatenate(GradientTransform);

			return new RadialGradientPaintContext(this, cm, deviceBounds, userBounds, transform, hints, (float)Center.X, (float)Center.Y, Radius_Renamed, (float)Focus.X, (float)Focus.Y, Fractions_Renamed, Colors_Renamed, CycleMethod_Renamed, ColorSpace_Renamed);
		}

		/// <summary>
		/// Returns a copy of the center point of the radial gradient.
		/// </summary>
		/// <returns> a {@code Point2D} object that is a copy of the center point </returns>
		public Point2D CenterPoint
		{
			get
			{
				return new Point2D.Double(Center.X, Center.Y);
			}
		}

		/// <summary>
		/// Returns a copy of the focus point of the radial gradient.
		/// Note that if the focus point specified when the radial gradient
		/// was constructed lies outside of the radius of the circle, this
		/// method will still return the original focus point even though
		/// the rendering may center the rings of color on a different
		/// point that lies inside the radius.
		/// </summary>
		/// <returns> a {@code Point2D} object that is a copy of the focus point </returns>
		public Point2D FocusPoint
		{
			get
			{
				return new Point2D.Double(Focus.X, Focus.Y);
			}
		}

		/// <summary>
		/// Returns the radius of the circle defining the radial gradient.
		/// </summary>
		/// <returns> the radius of the circle defining the radial gradient </returns>
		public float Radius
		{
			get
			{
				return Radius_Renamed;
			}
		}
	}

}