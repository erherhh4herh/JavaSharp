/*
 * Copyright (c) 1997, 2010, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>GradientPaint</code> class provides a way to fill
	/// a <seealso cref="Shape"/> with a linear color gradient pattern.
	/// If <seealso cref="Point"/> P1 with <seealso cref="Color"/> C1 and <code>Point</code> P2 with
	/// <code>Color</code> C2 are specified in user space, the
	/// <code>Color</code> on the P1, P2 connecting line is proportionally
	/// changed from C1 to C2.  Any point P not on the extended P1, P2
	/// connecting line has the color of the point P' that is the perpendicular
	/// projection of P on the extended P1, P2 connecting line.
	/// Points on the extended line outside of the P1, P2 segment can be colored
	/// in one of two ways.
	/// <ul>
	/// <li>
	/// If the gradient is cyclic then the points on the extended P1, P2
	/// connecting line cycle back and forth between the colors C1 and C2.
	/// <li>
	/// If the gradient is acyclic then points on the P1 side of the segment
	/// have the constant <code>Color</code> C1 while points on the P2 side
	/// have the constant <code>Color</code> C2.
	/// </ul>
	/// </summary>
	/// <seealso cref= Paint </seealso>
	/// <seealso cref= Graphics2D#setPaint
	/// @version 10 Feb 1997 </seealso>

	public class GradientPaint : Paint
	{
		internal Point2D.Float P1;
		internal Point2D.Float P2;
		internal Color Color1_Renamed;
		internal Color Color2_Renamed;
		internal bool Cyclic_Renamed;

		/// <summary>
		/// Constructs a simple acyclic <code>GradientPaint</code> object. </summary>
		/// <param name="x1"> x coordinate of the first specified
		/// <code>Point</code> in user space </param>
		/// <param name="y1"> y coordinate of the first specified
		/// <code>Point</code> in user space </param>
		/// <param name="color1"> <code>Color</code> at the first specified
		/// <code>Point</code> </param>
		/// <param name="x2"> x coordinate of the second specified
		/// <code>Point</code> in user space </param>
		/// <param name="y2"> y coordinate of the second specified
		/// <code>Point</code> in user space </param>
		/// <param name="color2"> <code>Color</code> at the second specified
		/// <code>Point</code> </param>
		/// <exception cref="NullPointerException"> if either one of colors is null </exception>
		public GradientPaint(float x1, float y1, Color color1, float x2, float y2, Color color2)
		{
			if ((color1 == null) || (color2 == null))
			{
				throw new NullPointerException("Colors cannot be null");
			}

			P1 = new Point2D.Float(x1, y1);
			P2 = new Point2D.Float(x2, y2);
			this.Color1_Renamed = color1;
			this.Color2_Renamed = color2;
		}

		/// <summary>
		/// Constructs a simple acyclic <code>GradientPaint</code> object. </summary>
		/// <param name="pt1"> the first specified <code>Point</code> in user space </param>
		/// <param name="color1"> <code>Color</code> at the first specified
		/// <code>Point</code> </param>
		/// <param name="pt2"> the second specified <code>Point</code> in user space </param>
		/// <param name="color2"> <code>Color</code> at the second specified
		/// <code>Point</code> </param>
		/// <exception cref="NullPointerException"> if either one of colors or points
		/// is null </exception>
		public GradientPaint(Point2D pt1, Color color1, Point2D pt2, Color color2)
		{
			if ((color1 == null) || (color2 == null) || (pt1 == null) || (pt2 == null))
			{
				throw new NullPointerException("Colors and points should be non-null");
			}

			P1 = new Point2D.Float((float)pt1.X, (float)pt1.Y);
			P2 = new Point2D.Float((float)pt2.X, (float)pt2.Y);
			this.Color1_Renamed = color1;
			this.Color2_Renamed = color2;
		}

		/// <summary>
		/// Constructs either a cyclic or acyclic <code>GradientPaint</code>
		/// object depending on the <code>boolean</code> parameter. </summary>
		/// <param name="x1"> x coordinate of the first specified
		/// <code>Point</code> in user space </param>
		/// <param name="y1"> y coordinate of the first specified
		/// <code>Point</code> in user space </param>
		/// <param name="color1"> <code>Color</code> at the first specified
		/// <code>Point</code> </param>
		/// <param name="x2"> x coordinate of the second specified
		/// <code>Point</code> in user space </param>
		/// <param name="y2"> y coordinate of the second specified
		/// <code>Point</code> in user space </param>
		/// <param name="color2"> <code>Color</code> at the second specified
		/// <code>Point</code> </param>
		/// <param name="cyclic"> <code>true</code> if the gradient pattern should cycle
		/// repeatedly between the two colors; <code>false</code> otherwise </param>
		public GradientPaint(float x1, float y1, Color color1, float x2, float y2, Color color2, bool cyclic) : this(x1, y1, color1, x2, y2, color2)
		{
			this.Cyclic_Renamed = cyclic;
		}

		/// <summary>
		/// Constructs either a cyclic or acyclic <code>GradientPaint</code>
		/// object depending on the <code>boolean</code> parameter. </summary>
		/// <param name="pt1"> the first specified <code>Point</code>
		/// in user space </param>
		/// <param name="color1"> <code>Color</code> at the first specified
		/// <code>Point</code> </param>
		/// <param name="pt2"> the second specified <code>Point</code>
		/// in user space </param>
		/// <param name="color2"> <code>Color</code> at the second specified
		/// <code>Point</code> </param>
		/// <param name="cyclic"> <code>true</code> if the gradient pattern should cycle
		/// repeatedly between the two colors; <code>false</code> otherwise </param>
		/// <exception cref="NullPointerException"> if either one of colors or points
		/// is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({ "point1", "color1", "point2", "color2", "cyclic" }) public GradientPaint(java.awt.geom.Point2D pt1, Color color1, java.awt.geom.Point2D pt2, Color color2, boolean cyclic)
		public GradientPaint(Point2D pt1, Color color1, Point2D pt2, Color color2, bool cyclic) : this(pt1, color1, pt2, color2)
		{
			this.Cyclic_Renamed = cyclic;
		}

		/// <summary>
		/// Returns a copy of the point P1 that anchors the first color. </summary>
		/// <returns> a <seealso cref="Point2D"/> object that is a copy of the point
		/// that anchors the first color of this
		/// <code>GradientPaint</code>. </returns>
		public virtual Point2D Point1
		{
			get
			{
				return new Point2D.Float(P1.x, P1.y);
			}
		}

		/// <summary>
		/// Returns the color C1 anchored by the point P1. </summary>
		/// <returns> a <code>Color</code> object that is the color
		/// anchored by P1. </returns>
		public virtual Color Color1
		{
			get
			{
				return Color1_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of the point P2 which anchors the second color. </summary>
		/// <returns> a <seealso cref="Point2D"/> object that is a copy of the point
		/// that anchors the second color of this
		/// <code>GradientPaint</code>. </returns>
		public virtual Point2D Point2
		{
			get
			{
				return new Point2D.Float(P2.x, P2.y);
			}
		}

		/// <summary>
		/// Returns the color C2 anchored by the point P2. </summary>
		/// <returns> a <code>Color</code> object that is the color
		/// anchored by P2. </returns>
		public virtual Color Color2
		{
			get
			{
				return Color2_Renamed;
			}
		}

		/// <summary>
		/// Returns <code>true</code> if the gradient cycles repeatedly
		/// between the two colors C1 and C2. </summary>
		/// <returns> <code>true</code> if the gradient cycles repeatedly
		/// between the two colors; <code>false</code> otherwise. </returns>
		public virtual bool Cyclic
		{
			get
			{
				return Cyclic_Renamed;
			}
		}

		/// <summary>
		/// Creates and returns a <seealso cref="PaintContext"/> used to
		/// generate a linear color gradient pattern.
		/// See the <seealso cref="Paint#createContext specification"/> of the
		/// method in the <seealso cref="Paint"/> interface for information
		/// on null parameter handling.
		/// </summary>
		/// <param name="cm"> the preferred <seealso cref="ColorModel"/> which represents the most convenient
		///           format for the caller to receive the pixel data, or {@code null}
		///           if there is no preference. </param>
		/// <param name="deviceBounds"> the device space bounding box
		///                     of the graphics primitive being rendered. </param>
		/// <param name="userBounds"> the user space bounding box
		///                   of the graphics primitive being rendered. </param>
		/// <param name="xform"> the <seealso cref="AffineTransform"/> from user
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
		public virtual PaintContext CreateContext(ColorModel cm, Rectangle deviceBounds, Rectangle2D userBounds, AffineTransform xform, RenderingHints hints)
		{

			return new GradientPaintContext(cm, P1, P2, xform, Color1_Renamed, Color2_Renamed, Cyclic_Renamed);
		}

		/// <summary>
		/// Returns the transparency mode for this <code>GradientPaint</code>. </summary>
		/// <returns> an integer value representing this <code>GradientPaint</code>
		/// object's transparency mode. </returns>
		/// <seealso cref= Transparency </seealso>
		public virtual int Transparency
		{
			get
			{
				int a1 = Color1_Renamed.Alpha;
				int a2 = Color2_Renamed.Alpha;
				return (((a1 & a2) == 0xff) ? Transparency_Fields.OPAQUE : Transparency_Fields.TRANSLUCENT);
			}
		}

	}

}