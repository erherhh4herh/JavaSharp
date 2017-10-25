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

namespace java.awt.geom
{

	using Curve = sun.awt.geom.Curve;

	/// <summary>
	/// The <code>QuadCurve2D</code> class defines a quadratic parametric curve
	/// segment in {@code (x,y)} coordinate space.
	/// <para>
	/// This class is only the abstract superclass for all objects that
	/// store a 2D quadratic curve segment.
	/// The actual storage representation of the coordinates is left to
	/// the subclass.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public abstract class QuadCurve2D : Shape, Cloneable
	{
		public abstract java.awt.geom.Rectangle2D Bounds2D {get;}

		/// <summary>
		/// A quadratic parametric curve segment specified with
		/// {@code float} coordinates.
		/// 
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Float : QuadCurve2D
		{
			/// <summary>
			/// The X coordinate of the start point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float X1_Renamed;

			/// <summary>
			/// The Y coordinate of the start point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Y1_Renamed;

			/// <summary>
			/// The X coordinate of the control point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Ctrlx;

			/// <summary>
			/// The Y coordinate of the control point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Ctrly;

			/// <summary>
			/// The X coordinate of the end point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float X2_Renamed;

			/// <summary>
			/// The Y coordinate of the end point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Y2_Renamed;

			/// <summary>
			/// Constructs and initializes a <code>QuadCurve2D</code> with
			/// coordinates (0, 0, 0, 0, 0, 0).
			/// @since 1.2
			/// </summary>
			public Float()
			{
			}

			/// <summary>
			/// Constructs and initializes a <code>QuadCurve2D</code> from the
			/// specified {@code float} coordinates.
			/// </summary>
			/// <param name="x1"> the X coordinate of the start point </param>
			/// <param name="y1"> the Y coordinate of the start point </param>
			/// <param name="ctrlx"> the X coordinate of the control point </param>
			/// <param name="ctrly"> the Y coordinate of the control point </param>
			/// <param name="x2"> the X coordinate of the end point </param>
			/// <param name="y2"> the Y coordinate of the end point
			/// @since 1.2 </param>
			public Float(float x1, float y1, float ctrlx, float ctrly, float x2, float y2)
			{
				SetCurve(x1, y1, ctrlx, ctrly, x2, y2);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X1
			{
				get
				{
					return (double) X1_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y1
			{
				get
				{
					return (double) Y1_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D P1
			{
				get
				{
					return new Point2D.Float(X1_Renamed, Y1_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlX
			{
				get
				{
					return (double) Ctrlx;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlY
			{
				get
				{
					return (double) Ctrly;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D CtrlPt
			{
				get
				{
					return new Point2D.Float(Ctrlx, Ctrly);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X2
			{
				get
				{
					return (double) X2_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y2
			{
				get
				{
					return (double) Y2_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D P2
			{
				get
				{
					return new Point2D.Float(X2_Renamed, Y2_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override void SetCurve(double x1, double y1, double ctrlx, double ctrly, double x2, double y2)
			{
				this.X1_Renamed = (float) x1;
				this.Y1_Renamed = (float) y1;
				this.Ctrlx = (float) ctrlx;
				this.Ctrly = (float) ctrly;
				this.X2_Renamed = (float) x2;
				this.Y2_Renamed = (float) y2;
			}

			/// <summary>
			/// Sets the location of the end points and control point of this curve
			/// to the specified {@code float} coordinates.
			/// </summary>
			/// <param name="x1"> the X coordinate of the start point </param>
			/// <param name="y1"> the Y coordinate of the start point </param>
			/// <param name="ctrlx"> the X coordinate of the control point </param>
			/// <param name="ctrly"> the Y coordinate of the control point </param>
			/// <param name="x2"> the X coordinate of the end point </param>
			/// <param name="y2"> the Y coordinate of the end point
			/// @since 1.2 </param>
			public virtual void SetCurve(float x1, float y1, float ctrlx, float ctrly, float x2, float y2)
			{
				this.X1_Renamed = x1;
				this.Y1_Renamed = y1;
				this.Ctrlx = ctrlx;
				this.Ctrly = ctrly;
				this.X2_Renamed = x2;
				this.Y2_Renamed = y2;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D Bounds2D
			{
				get
				{
					float left = System.Math.Min(System.Math.Min(X1_Renamed, X2_Renamed), Ctrlx);
					float top = System.Math.Min(System.Math.Min(Y1_Renamed, Y2_Renamed), Ctrly);
					float right = System.Math.Max(System.Math.Max(X1_Renamed, X2_Renamed), Ctrlx);
					float bottom = System.Math.Max(System.Math.Max(Y1_Renamed, Y2_Renamed), Ctrly);
					return new Rectangle2D.Float(left, top, right - left, bottom - top);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = -8511188402130719609L;
		}

		/// <summary>
		/// A quadratic parametric curve segment specified with
		/// {@code double} coordinates.
		/// 
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Double : QuadCurve2D
		{
			/// <summary>
			/// The X coordinate of the start point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double X1_Renamed;

			/// <summary>
			/// The Y coordinate of the start point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Y1_Renamed;

			/// <summary>
			/// The X coordinate of the control point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Ctrlx;

			/// <summary>
			/// The Y coordinate of the control point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Ctrly;

			/// <summary>
			/// The X coordinate of the end point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double X2_Renamed;

			/// <summary>
			/// The Y coordinate of the end point of the quadratic curve
			/// segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Y2_Renamed;

			/// <summary>
			/// Constructs and initializes a <code>QuadCurve2D</code> with
			/// coordinates (0, 0, 0, 0, 0, 0).
			/// @since 1.2
			/// </summary>
			public Double()
			{
			}

			/// <summary>
			/// Constructs and initializes a <code>QuadCurve2D</code> from the
			/// specified {@code double} coordinates.
			/// </summary>
			/// <param name="x1"> the X coordinate of the start point </param>
			/// <param name="y1"> the Y coordinate of the start point </param>
			/// <param name="ctrlx"> the X coordinate of the control point </param>
			/// <param name="ctrly"> the Y coordinate of the control point </param>
			/// <param name="x2"> the X coordinate of the end point </param>
			/// <param name="y2"> the Y coordinate of the end point
			/// @since 1.2 </param>
			public Double(double x1, double y1, double ctrlx, double ctrly, double x2, double y2)
			{
				SetCurve(x1, y1, ctrlx, ctrly, x2, y2);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X1
			{
				get
				{
					return X1_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y1
			{
				get
				{
					return Y1_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D P1
			{
				get
				{
					return new Point2D.Double(X1_Renamed, Y1_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlX
			{
				get
				{
					return Ctrlx;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlY
			{
				get
				{
					return Ctrly;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D CtrlPt
			{
				get
				{
					return new Point2D.Double(Ctrlx, Ctrly);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X2
			{
				get
				{
					return X2_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y2
			{
				get
				{
					return Y2_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D P2
			{
				get
				{
					return new Point2D.Double(X2_Renamed, Y2_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override void SetCurve(double x1, double y1, double ctrlx, double ctrly, double x2, double y2)
			{
				this.X1_Renamed = x1;
				this.Y1_Renamed = y1;
				this.Ctrlx = ctrlx;
				this.Ctrly = ctrly;
				this.X2_Renamed = x2;
				this.Y2_Renamed = y2;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D Bounds2D
			{
				get
				{
					double left = System.Math.Min(System.Math.Min(X1_Renamed, X2_Renamed), Ctrlx);
					double top = System.Math.Min(System.Math.Min(Y1_Renamed, Y2_Renamed), Ctrly);
					double right = System.Math.Max(System.Math.Max(X1_Renamed, X2_Renamed), Ctrlx);
					double bottom = System.Math.Max(System.Math.Max(Y1_Renamed, Y2_Renamed), Ctrly);
					return new Rectangle2D.Double(left, top, right - left, bottom - top);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 4217149928428559721L;
		}

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessor
		/// methods below.
		/// </summary>
		/// <seealso cref= java.awt.geom.QuadCurve2D.Float </seealso>
		/// <seealso cref= java.awt.geom.QuadCurve2D.Double
		/// @since 1.2 </seealso>
		protected internal QuadCurve2D()
		{
		}

		/// <summary>
		/// Returns the X coordinate of the start point in
		/// <code>double</code> in precision. </summary>
		/// <returns> the X coordinate of the start point.
		/// @since 1.2 </returns>
		public abstract double X1 {get;}

		/// <summary>
		/// Returns the Y coordinate of the start point in
		/// <code>double</code> precision. </summary>
		/// <returns> the Y coordinate of the start point.
		/// @since 1.2 </returns>
		public abstract double Y1 {get;}

		/// <summary>
		/// Returns the start point. </summary>
		/// <returns> a <code>Point2D</code> that is the start point of this
		///          <code>QuadCurve2D</code>.
		/// @since 1.2 </returns>
		public abstract Point2D P1 {get;}

		/// <summary>
		/// Returns the X coordinate of the control point in
		/// <code>double</code> precision. </summary>
		/// <returns> X coordinate the control point
		/// @since 1.2 </returns>
		public abstract double CtrlX {get;}

		/// <summary>
		/// Returns the Y coordinate of the control point in
		/// <code>double</code> precision. </summary>
		/// <returns> the Y coordinate of the control point.
		/// @since 1.2 </returns>
		public abstract double CtrlY {get;}

		/// <summary>
		/// Returns the control point. </summary>
		/// <returns> a <code>Point2D</code> that is the control point of this
		///          <code>Point2D</code>.
		/// @since 1.2 </returns>
		public abstract Point2D CtrlPt {get;}

		/// <summary>
		/// Returns the X coordinate of the end point in
		/// <code>double</code> precision. </summary>
		/// <returns> the x coordinate of the end point.
		/// @since 1.2 </returns>
		public abstract double X2 {get;}

		/// <summary>
		/// Returns the Y coordinate of the end point in
		/// <code>double</code> precision. </summary>
		/// <returns> the Y coordinate of the end point.
		/// @since 1.2 </returns>
		public abstract double Y2 {get;}

		/// <summary>
		/// Returns the end point. </summary>
		/// <returns> a <code>Point</code> object that is the end point
		///          of this <code>Point2D</code>.
		/// @since 1.2 </returns>
		public abstract Point2D P2 {get;}

		/// <summary>
		/// Sets the location of the end points and control point of this curve
		/// to the specified <code>double</code> coordinates.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point </param>
		/// <param name="y1"> the Y coordinate of the start point </param>
		/// <param name="ctrlx"> the X coordinate of the control point </param>
		/// <param name="ctrly"> the Y coordinate of the control point </param>
		/// <param name="x2"> the X coordinate of the end point </param>
		/// <param name="y2"> the Y coordinate of the end point
		/// @since 1.2 </param>
		public abstract void SetCurve(double x1, double y1, double ctrlx, double ctrly, double x2, double y2);

		/// <summary>
		/// Sets the location of the end points and control points of this
		/// <code>QuadCurve2D</code> to the <code>double</code> coordinates at
		/// the specified offset in the specified array. </summary>
		/// <param name="coords"> the array containing coordinate values </param>
		/// <param name="offset"> the index into the array from which to start
		///          getting the coordinate values and assigning them to this
		///          <code>QuadCurve2D</code>
		/// @since 1.2 </param>
		public virtual void SetCurve(double[] coords, int offset)
		{
			SetCurve(coords[offset + 0], coords[offset + 1], coords[offset + 2], coords[offset + 3], coords[offset + 4], coords[offset + 5]);
		}

		/// <summary>
		/// Sets the location of the end points and control point of this
		/// <code>QuadCurve2D</code> to the specified <code>Point2D</code>
		/// coordinates. </summary>
		/// <param name="p1"> the start point </param>
		/// <param name="cp"> the control point </param>
		/// <param name="p2"> the end point
		/// @since 1.2 </param>
		public virtual void SetCurve(Point2D p1, Point2D cp, Point2D p2)
		{
			SetCurve(p1.X, p1.Y, cp.X, cp.Y, p2.X, p2.Y);
		}

		/// <summary>
		/// Sets the location of the end points and control points of this
		/// <code>QuadCurve2D</code> to the coordinates of the
		/// <code>Point2D</code> objects at the specified offset in
		/// the specified array. </summary>
		/// <param name="pts"> an array containing <code>Point2D</code> that define
		///          coordinate values </param>
		/// <param name="offset"> the index into <code>pts</code> from which to start
		///          getting the coordinate values and assigning them to this
		///          <code>QuadCurve2D</code>
		/// @since 1.2 </param>
		public virtual void SetCurve(Point2D[] pts, int offset)
		{
			SetCurve(pts[offset + 0].X, pts[offset + 0].Y, pts[offset + 1].X, pts[offset + 1].Y, pts[offset + 2].X, pts[offset + 2].Y);
		}

		/// <summary>
		/// Sets the location of the end points and control point of this
		/// <code>QuadCurve2D</code> to the same as those in the specified
		/// <code>QuadCurve2D</code>. </summary>
		/// <param name="c"> the specified <code>QuadCurve2D</code>
		/// @since 1.2 </param>
		public virtual QuadCurve2D Curve
		{
			set
			{
				SetCurve(value.X1, value.Y1, value.CtrlX, value.CtrlY, value.X2, value.Y2);
			}
		}

		/// <summary>
		/// Returns the square of the flatness, or maximum distance of a
		/// control point from the line connecting the end points, of the
		/// quadratic curve specified by the indicated control points.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point </param>
		/// <param name="y1"> the Y coordinate of the start point </param>
		/// <param name="ctrlx"> the X coordinate of the control point </param>
		/// <param name="ctrly"> the Y coordinate of the control point </param>
		/// <param name="x2"> the X coordinate of the end point </param>
		/// <param name="y2"> the Y coordinate of the end point </param>
		/// <returns> the square of the flatness of the quadratic curve
		///          defined by the specified coordinates.
		/// @since 1.2 </returns>
		public static double GetFlatnessSq(double x1, double y1, double ctrlx, double ctrly, double x2, double y2)
		{
			return Line2D.PtSegDistSq(x1, y1, x2, y2, ctrlx, ctrly);
		}

		/// <summary>
		/// Returns the flatness, or maximum distance of a
		/// control point from the line connecting the end points, of the
		/// quadratic curve specified by the indicated control points.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point </param>
		/// <param name="y1"> the Y coordinate of the start point </param>
		/// <param name="ctrlx"> the X coordinate of the control point </param>
		/// <param name="ctrly"> the Y coordinate of the control point </param>
		/// <param name="x2"> the X coordinate of the end point </param>
		/// <param name="y2"> the Y coordinate of the end point </param>
		/// <returns> the flatness of the quadratic curve defined by the
		///          specified coordinates.
		/// @since 1.2 </returns>
		public static double GetFlatness(double x1, double y1, double ctrlx, double ctrly, double x2, double y2)
		{
			return Line2D.PtSegDist(x1, y1, x2, y2, ctrlx, ctrly);
		}

		/// <summary>
		/// Returns the square of the flatness, or maximum distance of a
		/// control point from the line connecting the end points, of the
		/// quadratic curve specified by the control points stored in the
		/// indicated array at the indicated index. </summary>
		/// <param name="coords"> an array containing coordinate values </param>
		/// <param name="offset"> the index into <code>coords</code> from which to
		///          to start getting the values from the array </param>
		/// <returns> the flatness of the quadratic curve that is defined by the
		///          values in the specified array at the specified index.
		/// @since 1.2 </returns>
		public static double GetFlatnessSq(double[] coords, int offset)
		{
			return Line2D.PtSegDistSq(coords[offset + 0], coords[offset + 1], coords[offset + 4], coords[offset + 5], coords[offset + 2], coords[offset + 3]);
		}

		/// <summary>
		/// Returns the flatness, or maximum distance of a
		/// control point from the line connecting the end points, of the
		/// quadratic curve specified by the control points stored in the
		/// indicated array at the indicated index. </summary>
		/// <param name="coords"> an array containing coordinate values </param>
		/// <param name="offset"> the index into <code>coords</code> from which to
		///          start getting the coordinate values </param>
		/// <returns> the flatness of a quadratic curve defined by the
		///          specified array at the specified offset.
		/// @since 1.2 </returns>
		public static double GetFlatness(double[] coords, int offset)
		{
			return Line2D.PtSegDist(coords[offset + 0], coords[offset + 1], coords[offset + 4], coords[offset + 5], coords[offset + 2], coords[offset + 3]);
		}

		/// <summary>
		/// Returns the square of the flatness, or maximum distance of a
		/// control point from the line connecting the end points, of this
		/// <code>QuadCurve2D</code>. </summary>
		/// <returns> the square of the flatness of this
		///          <code>QuadCurve2D</code>.
		/// @since 1.2 </returns>
		public virtual double FlatnessSq
		{
			get
			{
				return Line2D.PtSegDistSq(X1, Y1, X2, Y2, CtrlX, CtrlY);
			}
		}

		/// <summary>
		/// Returns the flatness, or maximum distance of a
		/// control point from the line connecting the end points, of this
		/// <code>QuadCurve2D</code>. </summary>
		/// <returns> the flatness of this <code>QuadCurve2D</code>.
		/// @since 1.2 </returns>
		public virtual double Flatness
		{
			get
			{
				return Line2D.PtSegDist(X1, Y1, X2, Y2, CtrlX, CtrlY);
			}
		}

		/// <summary>
		/// Subdivides this <code>QuadCurve2D</code> and stores the resulting
		/// two subdivided curves into the <code>left</code> and
		/// <code>right</code> curve parameters.
		/// Either or both of the <code>left</code> and <code>right</code>
		/// objects can be the same as this <code>QuadCurve2D</code> or
		/// <code>null</code>. </summary>
		/// <param name="left"> the <code>QuadCurve2D</code> object for storing the
		/// left or first half of the subdivided curve </param>
		/// <param name="right"> the <code>QuadCurve2D</code> object for storing the
		/// right or second half of the subdivided curve
		/// @since 1.2 </param>
		public virtual void Subdivide(QuadCurve2D left, QuadCurve2D right)
		{
			Subdivide(this, left, right);
		}

		/// <summary>
		/// Subdivides the quadratic curve specified by the <code>src</code>
		/// parameter and stores the resulting two subdivided curves into the
		/// <code>left</code> and <code>right</code> curve parameters.
		/// Either or both of the <code>left</code> and <code>right</code>
		/// objects can be the same as the <code>src</code> object or
		/// <code>null</code>. </summary>
		/// <param name="src"> the quadratic curve to be subdivided </param>
		/// <param name="left"> the <code>QuadCurve2D</code> object for storing the
		///          left or first half of the subdivided curve </param>
		/// <param name="right"> the <code>QuadCurve2D</code> object for storing the
		///          right or second half of the subdivided curve
		/// @since 1.2 </param>
		public static void Subdivide(QuadCurve2D src, QuadCurve2D left, QuadCurve2D right)
		{
			double x1 = src.X1;
			double y1 = src.Y1;
			double ctrlx = src.CtrlX;
			double ctrly = src.CtrlY;
			double x2 = src.X2;
			double y2 = src.Y2;
			double ctrlx1 = (x1 + ctrlx) / 2.0;
			double ctrly1 = (y1 + ctrly) / 2.0;
			double ctrlx2 = (x2 + ctrlx) / 2.0;
			double ctrly2 = (y2 + ctrly) / 2.0;
			ctrlx = (ctrlx1 + ctrlx2) / 2.0;
			ctrly = (ctrly1 + ctrly2) / 2.0;
			if (left != null)
			{
				left.SetCurve(x1, y1, ctrlx1, ctrly1, ctrlx, ctrly);
			}
			if (right != null)
			{
				right.SetCurve(ctrlx, ctrly, ctrlx2, ctrly2, x2, y2);
			}
		}

		/// <summary>
		/// Subdivides the quadratic curve specified by the coordinates
		/// stored in the <code>src</code> array at indices
		/// <code>srcoff</code> through <code>srcoff</code>&nbsp;+&nbsp;5
		/// and stores the resulting two subdivided curves into the two
		/// result arrays at the corresponding indices.
		/// Either or both of the <code>left</code> and <code>right</code>
		/// arrays can be <code>null</code> or a reference to the same array
		/// and offset as the <code>src</code> array.
		/// Note that the last point in the first subdivided curve is the
		/// same as the first point in the second subdivided curve.  Thus,
		/// it is possible to pass the same array for <code>left</code> and
		/// <code>right</code> and to use offsets such that
		/// <code>rightoff</code> equals <code>leftoff</code> + 4 in order
		/// to avoid allocating extra storage for this common point. </summary>
		/// <param name="src"> the array holding the coordinates for the source curve </param>
		/// <param name="srcoff"> the offset into the array of the beginning of the
		/// the 6 source coordinates </param>
		/// <param name="left"> the array for storing the coordinates for the first
		/// half of the subdivided curve </param>
		/// <param name="leftoff"> the offset into the array of the beginning of the
		/// the 6 left coordinates </param>
		/// <param name="right"> the array for storing the coordinates for the second
		/// half of the subdivided curve </param>
		/// <param name="rightoff"> the offset into the array of the beginning of the
		/// the 6 right coordinates
		/// @since 1.2 </param>
		public static void Subdivide(double[] src, int srcoff, double[] left, int leftoff, double[] right, int rightoff)
		{
			double x1 = src[srcoff + 0];
			double y1 = src[srcoff + 1];
			double ctrlx = src[srcoff + 2];
			double ctrly = src[srcoff + 3];
			double x2 = src[srcoff + 4];
			double y2 = src[srcoff + 5];
			if (left != null)
			{
				left[leftoff + 0] = x1;
				left[leftoff + 1] = y1;
			}
			if (right != null)
			{
				right[rightoff + 4] = x2;
				right[rightoff + 5] = y2;
			}
			x1 = (x1 + ctrlx) / 2.0;
			y1 = (y1 + ctrly) / 2.0;
			x2 = (x2 + ctrlx) / 2.0;
			y2 = (y2 + ctrly) / 2.0;
			ctrlx = (x1 + x2) / 2.0;
			ctrly = (y1 + y2) / 2.0;
			if (left != null)
			{
				left[leftoff + 2] = x1;
				left[leftoff + 3] = y1;
				left[leftoff + 4] = ctrlx;
				left[leftoff + 5] = ctrly;
			}
			if (right != null)
			{
				right[rightoff + 0] = ctrlx;
				right[rightoff + 1] = ctrly;
				right[rightoff + 2] = x2;
				right[rightoff + 3] = y2;
			}
		}

		/// <summary>
		/// Solves the quadratic whose coefficients are in the <code>eqn</code>
		/// array and places the non-complex roots back into the same array,
		/// returning the number of roots.  The quadratic solved is represented
		/// by the equation:
		/// <pre>
		///     eqn = {C, B, A};
		///     ax^2 + bx + c = 0
		/// </pre>
		/// A return value of <code>-1</code> is used to distinguish a constant
		/// equation, which might be always 0 or never 0, from an equation that
		/// has no zeroes. </summary>
		/// <param name="eqn"> the array that contains the quadratic coefficients </param>
		/// <returns> the number of roots, or <code>-1</code> if the equation is
		///          a constant
		/// @since 1.2 </returns>
		public static int SolveQuadratic(double[] eqn)
		{
			return SolveQuadratic(eqn, eqn);
		}

		/// <summary>
		/// Solves the quadratic whose coefficients are in the <code>eqn</code>
		/// array and places the non-complex roots into the <code>res</code>
		/// array, returning the number of roots.
		/// The quadratic solved is represented by the equation:
		/// <pre>
		///     eqn = {C, B, A};
		///     ax^2 + bx + c = 0
		/// </pre>
		/// A return value of <code>-1</code> is used to distinguish a constant
		/// equation, which might be always 0 or never 0, from an equation that
		/// has no zeroes. </summary>
		/// <param name="eqn"> the specified array of coefficients to use to solve
		///        the quadratic equation </param>
		/// <param name="res"> the array that contains the non-complex roots
		///        resulting from the solution of the quadratic equation </param>
		/// <returns> the number of roots, or <code>-1</code> if the equation is
		///  a constant.
		/// @since 1.3 </returns>
		public static int SolveQuadratic(double[] eqn, double[] res)
		{
			double a = eqn[2];
			double b = eqn[1];
			double c = eqn[0];
			int roots = 0;
			if (a == 0.0)
			{
				// The quadratic parabola has degenerated to a line.
				if (b == 0.0)
				{
					// The line has degenerated to a constant.
					return -1;
				}
				res[roots++] = -c / b;
			}
			else
			{
				// From Numerical Recipes, 5.6, Quadratic and Cubic Equations
				double d = b * b - 4.0 * a * c;
				if (d < 0.0)
				{
					// If d < 0.0, then there are no roots
					return 0;
				}
				d = System.Math.Sqrt(d);
				// For accuracy, calculate one root using:
				//     (-b +/- d) / 2a
				// and the other using:
				//     2c / (-b +/- d)
				// Choose the sign of the +/- so that b+d gets larger in magnitude
				if (b < 0.0)
				{
					d = -d;
				}
				double q = (b + d) / -2.0;
				// We already tested a for being 0 above
				res[roots++] = q / a;
				if (q != 0.0)
				{
					res[roots++] = c / q;
				}
			}
			return roots;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(double x, double y)
		{

			double x1 = X1;
			double y1 = Y1;
			double xc = CtrlX;
			double yc = CtrlY;
			double x2 = X2;
			double y2 = Y2;

			/*
			 * We have a convex shape bounded by quad curve Pc(t)
			 * and ine Pl(t).
			 *
			 *     P1 = (x1, y1) - start point of curve
			 *     P2 = (x2, y2) - end point of curve
			 *     Pc = (xc, yc) - control point
			 *
			 *     Pq(t) = P1*(1 - t)^2 + 2*Pc*t*(1 - t) + P2*t^2 =
			 *           = (P1 - 2*Pc + P2)*t^2 + 2*(Pc - P1)*t + P1
			 *     Pl(t) = P1*(1 - t) + P2*t
			 *     t = [0:1]
			 *
			 *     P = (x, y) - point of interest
			 *
			 * Let's look at second derivative of quad curve equation:
			 *
			 *     Pq''(t) = 2 * (P1 - 2 * Pc + P2) = Pq''
			 *     It's constant vector.
			 *
			 * Let's draw a line through P to be parallel to this
			 * vector and find the intersection of the quad curve
			 * and the line.
			 *
			 * Pq(t) is point of intersection if system of equations
			 * below has the solution.
			 *
			 *     L(s) = P + Pq''*s == Pq(t)
			 *     Pq''*s + (P - Pq(t)) == 0
			 *
			 *     | xq''*s + (x - xq(t)) == 0
			 *     | yq''*s + (y - yq(t)) == 0
			 *
			 * This system has the solution if rank of its matrix equals to 1.
			 * That is, determinant of the matrix should be zero.
			 *
			 *     (y - yq(t))*xq'' == (x - xq(t))*yq''
			 *
			 * Let's solve this equation with 't' variable.
			 * Also let kx = x1 - 2*xc + x2
			 *          ky = y1 - 2*yc + y2
			 *
			 *     t0q = (1/2)*((x - x1)*ky - (y - y1)*kx) /
			 *                 ((xc - x1)*ky - (yc - y1)*kx)
			 *
			 * Let's do the same for our line Pl(t):
			 *
			 *     t0l = ((x - x1)*ky - (y - y1)*kx) /
			 *           ((x2 - x1)*ky - (y2 - y1)*kx)
			 *
			 * It's easy to check that t0q == t0l. This fact means
			 * we can compute t0 only one time.
			 *
			 * In case t0 < 0 or t0 > 1, we have an intersections outside
			 * of shape bounds. So, P is definitely out of shape.
			 *
			 * In case t0 is inside [0:1], we should calculate Pq(t0)
			 * and Pl(t0). We have three points for now, and all of them
			 * lie on one line. So, we just need to detect, is our point
			 * of interest between points of intersections or not.
			 *
			 * If the denominator in the t0q and t0l equations is
			 * zero, then the points must be collinear and so the
			 * curve is degenerate and encloses no area.  Thus the
			 * result is false.
			 */
			double kx = x1 - 2 * xc + x2;
			double ky = y1 - 2 * yc + y2;
			double dx = x - x1;
			double dy = y - y1;
			double dxl = x2 - x1;
			double dyl = y2 - y1;

			double t0 = (dx * ky - dy * kx) / (dxl * ky - dyl * kx);
			if (t0 < 0 || t0 > 1 || t0 != t0)
			{
				return false;
			}

			double xb = kx * t0 * t0 + 2 * (xc - x1) * t0 + x1;
			double yb = ky * t0 * t0 + 2 * (yc - y1) * t0 + y1;
			double xl = dxl * t0 + x1;
			double yl = dyl * t0 + y1;

			return (x >= xb && x < xl) || (x >= xl && x < xb) || (y >= yb && y < yl) || (y >= yl && y < yb);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(Point2D p)
		{
			return Contains(p.X, p.Y);
		}

		/// <summary>
		/// Fill an array with the coefficients of the parametric equation
		/// in t, ready for solving against val with solveQuadratic.
		/// We currently have:
		///     val = Py(t) = C1*(1-t)^2 + 2*CP*t*(1-t) + C2*t^2
		///                 = C1 - 2*C1*t + C1*t^2 + 2*CP*t - 2*CP*t^2 + C2*t^2
		///                 = C1 + (2*CP - 2*C1)*t + (C1 - 2*CP + C2)*t^2
		///               0 = (C1 - val) + (2*CP - 2*C1)*t + (C1 - 2*CP + C2)*t^2
		///               0 = C + Bt + At^2
		///     C = C1 - val
		///     B = 2*CP - 2*C1
		///     A = C1 - 2*CP + C2
		/// </summary>
		private static void FillEqn(double[] eqn, double val, double c1, double cp, double c2)
		{
			eqn[0] = c1 - val;
			eqn[1] = cp + cp - c1 - c1;
			eqn[2] = c1 - cp - cp + c2;
			return;
		}

		/// <summary>
		/// Evaluate the t values in the first num slots of the vals[] array
		/// and place the evaluated values back into the same array.  Only
		/// evaluate t values that are within the range &lt;0, 1&gt;, including
		/// the 0 and 1 ends of the range iff the include0 or include1
		/// booleans are true.  If an "inflection" equation is handed in,
		/// then any points which represent a point of inflection for that
		/// quadratic equation are also ignored.
		/// </summary>
		private static int EvalQuadratic(double[] vals, int num, bool include0, bool include1, double[] inflect, double c1, double ctrl, double c2)
		{
			int j = 0;
			for (int i = 0; i < num; i++)
			{
				double t = vals[i];
				if ((include0 ? t >= 0 : t > 0) && (include1 ? t <= 1 : t < 1) && (inflect == null || inflect[1] + 2 * inflect[2] * t != 0))
				{
					double u = 1 - t;
					vals[j++] = c1 * u * u + 2 * ctrl * t * u + c2 * t * t;
				}
			}
			return j;
		}

		private const int BELOW = -2;
		private const int LOWEDGE = -1;
		private const int INSIDE = 0;
		private const int HIGHEDGE = 1;
		private const int ABOVE = 2;

		/// <summary>
		/// Determine where coord lies with respect to the range from
		/// low to high.  It is assumed that low &lt;= high.  The return
		/// value is one of the 5 values BELOW, LOWEDGE, INSIDE, HIGHEDGE,
		/// or ABOVE.
		/// </summary>
		private static int GetTag(double coord, double low, double high)
		{
			if (coord <= low)
			{
				return (coord < low ? BELOW : LOWEDGE);
			}
			if (coord >= high)
			{
				return (coord > high ? ABOVE : HIGHEDGE);
			}
			return INSIDE;
		}

		/// <summary>
		/// Determine if the pttag represents a coordinate that is already
		/// in its test range, or is on the border with either of the two
		/// opttags representing another coordinate that is "towards the
		/// inside" of that test range.  In other words, are either of the
		/// two "opt" points "drawing the pt inward"?
		/// </summary>
		private static bool Inwards(int pttag, int opt1tag, int opt2tag)
		{
			switch (pttag)
			{
			case BELOW:
			case ABOVE:
			default:
				return false;
			case LOWEDGE:
				return (opt1tag >= INSIDE || opt2tag >= INSIDE);
			case INSIDE:
				return true;
			case HIGHEDGE:
				return (opt1tag <= INSIDE || opt2tag <= INSIDE);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Intersects(double x, double y, double w, double h)
		{
			// Trivially reject non-existant rectangles
			if (w <= 0 || h <= 0)
			{
				return false;
			}

			// Trivially accept if either endpoint is inside the rectangle
			// (not on its border since it may end there and not go inside)
			// Record where they lie with respect to the rectangle.
			//     -1 => left, 0 => inside, 1 => right
			double x1 = X1;
			double y1 = Y1;
			int x1tag = GetTag(x1, x, x + w);
			int y1tag = GetTag(y1, y, y + h);
			if (x1tag == INSIDE && y1tag == INSIDE)
			{
				return true;
			}
			double x2 = X2;
			double y2 = Y2;
			int x2tag = GetTag(x2, x, x + w);
			int y2tag = GetTag(y2, y, y + h);
			if (x2tag == INSIDE && y2tag == INSIDE)
			{
				return true;
			}
			double ctrlx = CtrlX;
			double ctrly = CtrlY;
			int ctrlxtag = GetTag(ctrlx, x, x + w);
			int ctrlytag = GetTag(ctrly, y, y + h);

			// Trivially reject if all points are entirely to one side of
			// the rectangle.
			if (x1tag < INSIDE && x2tag < INSIDE && ctrlxtag < INSIDE)
			{
				return false; // All points left
			}
			if (y1tag < INSIDE && y2tag < INSIDE && ctrlytag < INSIDE)
			{
				return false; // All points above
			}
			if (x1tag > INSIDE && x2tag > INSIDE && ctrlxtag > INSIDE)
			{
				return false; // All points right
			}
			if (y1tag > INSIDE && y2tag > INSIDE && ctrlytag > INSIDE)
			{
				return false; // All points below
			}

			// Test for endpoints on the edge where either the segment
			// or the curve is headed "inwards" from them
			// Note: These tests are a superset of the fast endpoint tests
			//       above and thus repeat those tests, but take more time
			//       and cover more cases
			if (Inwards(x1tag, x2tag, ctrlxtag) && Inwards(y1tag, y2tag, ctrlytag))
			{
				// First endpoint on border with either edge moving inside
				return true;
			}
			if (Inwards(x2tag, x1tag, ctrlxtag) && Inwards(y2tag, y1tag, ctrlytag))
			{
				// Second endpoint on border with either edge moving inside
				return true;
			}

			// Trivially accept if endpoints span directly across the rectangle
			bool xoverlap = (x1tag * x2tag <= 0);
			bool yoverlap = (y1tag * y2tag <= 0);
			if (x1tag == INSIDE && x2tag == INSIDE && yoverlap)
			{
				return true;
			}
			if (y1tag == INSIDE && y2tag == INSIDE && xoverlap)
			{
				return true;
			}

			// We now know that both endpoints are outside the rectangle
			// but the 3 points are not all on one side of the rectangle.
			// Therefore the curve cannot be contained inside the rectangle,
			// but the rectangle might be contained inside the curve, or
			// the curve might intersect the boundary of the rectangle.

			double[] eqn = new double[3];
			double[] res = new double[3];
			if (!yoverlap)
			{
				// Both Y coordinates for the closing segment are above or
				// below the rectangle which means that we can only intersect
				// if the curve crosses the top (or bottom) of the rectangle
				// in more than one place and if those crossing locations
				// span the horizontal range of the rectangle.
				FillEqn(eqn, (y1tag < INSIDE ? y : y + h), y1, ctrly, y2);
				return (SolveQuadratic(eqn, res) == 2 && EvalQuadratic(res, 2, true, true, null, x1, ctrlx, x2) == 2 && GetTag(res[0], x, x + w) * GetTag(res[1], x, x + w) <= 0);
			}

			// Y ranges overlap.  Now we examine the X ranges
			if (!xoverlap)
			{
				// Both X coordinates for the closing segment are left of
				// or right of the rectangle which means that we can only
				// intersect if the curve crosses the left (or right) edge
				// of the rectangle in more than one place and if those
				// crossing locations span the vertical range of the rectangle.
				FillEqn(eqn, (x1tag < INSIDE ? x : x + w), x1, ctrlx, x2);
				return (SolveQuadratic(eqn, res) == 2 && EvalQuadratic(res, 2, true, true, null, y1, ctrly, y2) == 2 && GetTag(res[0], y, y + h) * GetTag(res[1], y, y + h) <= 0);
			}

			// The X and Y ranges of the endpoints overlap the X and Y
			// ranges of the rectangle, now find out how the endpoint
			// line segment intersects the Y range of the rectangle
			double dx = x2 - x1;
			double dy = y2 - y1;
			double k = y2 * x1 - x2 * y1;
			int c1tag, c2tag;
			if (y1tag == INSIDE)
			{
				c1tag = x1tag;
			}
			else
			{
				c1tag = GetTag((k + dx * (y1tag < INSIDE ? y : y + h)) / dy, x, x + w);
			}
			if (y2tag == INSIDE)
			{
				c2tag = x2tag;
			}
			else
			{
				c2tag = GetTag((k + dx * (y2tag < INSIDE ? y : y + h)) / dy, x, x + w);
			}
			// If the part of the line segment that intersects the Y range
			// of the rectangle crosses it horizontally - trivially accept
			if (c1tag * c2tag <= 0)
			{
				return true;
			}

			// Now we know that both the X and Y ranges intersect and that
			// the endpoint line segment does not directly cross the rectangle.
			//
			// We can almost treat this case like one of the cases above
			// where both endpoints are to one side, except that we will
			// only get one intersection of the curve with the vertical
			// side of the rectangle.  This is because the endpoint segment
			// accounts for the other intersection.
			//
			// (Remember there is overlap in both the X and Y ranges which
			//  means that the segment must cross at least one vertical edge
			//  of the rectangle - in particular, the "near vertical side" -
			//  leaving only one intersection for the curve.)
			//
			// Now we calculate the y tags of the two intersections on the
			// "near vertical side" of the rectangle.  We will have one with
			// the endpoint segment, and one with the curve.  If those two
			// vertical intersections overlap the Y range of the rectangle,
			// we have an intersection.  Otherwise, we don't.

			// c1tag = vertical intersection class of the endpoint segment
			//
			// Choose the y tag of the endpoint that was not on the same
			// side of the rectangle as the subsegment calculated above.
			// Note that we can "steal" the existing Y tag of that endpoint
			// since it will be provably the same as the vertical intersection.
			c1tag = ((c1tag * x1tag <= 0) ? y1tag : y2tag);

			// c2tag = vertical intersection class of the curve
			//
			// We have to calculate this one the straightforward way.
			// Note that the c2tag can still tell us which vertical edge
			// to test against.
			FillEqn(eqn, (c2tag < INSIDE ? x : x + w), x1, ctrlx, x2);
			int num = SolveQuadratic(eqn, res);

			// Note: We should be able to assert(num == 2); since the
			// X range "crosses" (not touches) the vertical boundary,
			// but we pass num to evalQuadratic for completeness.
			EvalQuadratic(res, num, true, true, null, y1, ctrly, y2);

			// Note: We can assert(num evals == 1); since one of the
			// 2 crossings will be out of the [0,1] range.
			c2tag = GetTag(res[0], y, y + h);

			// Finally, we have an intersection if the two crossings
			// overlap the Y range of the rectangle.
			return (c1tag * c2tag <= 0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Intersects(Rectangle2D r)
		{
			return Intersects(r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(double x, double y, double w, double h)
		{
			if (w <= 0 || h <= 0)
			{
				return false;
			}
			// Assertion: Quadratic curves closed by connecting their
			// endpoints are always convex.
			return (Contains(x, y) && Contains(x + w, y) && Contains(x + w, y + h) && Contains(x, y + h));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(Rectangle2D r)
		{
			return Contains(r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual Rectangle Bounds
		{
			get
			{
				return Bounds2D.Bounds;
			}
		}

		/// <summary>
		/// Returns an iteration object that defines the boundary of the
		/// shape of this <code>QuadCurve2D</code>.
		/// The iterator for this class is not multi-threaded safe,
		/// which means that this <code>QuadCurve2D</code> class does not
		/// guarantee that modifications to the geometry of this
		/// <code>QuadCurve2D</code> object do not affect any iterations of
		/// that geometry that are already in process. </summary>
		/// <param name="at"> an optional <seealso cref="AffineTransform"/> to apply to the
		///          shape boundary </param>
		/// <returns> a <seealso cref="PathIterator"/> object that defines the boundary
		///          of the shape.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at)
		{
			return new QuadIterator(this, at);
		}

		/// <summary>
		/// Returns an iteration object that defines the boundary of the
		/// flattened shape of this <code>QuadCurve2D</code>.
		/// The iterator for this class is not multi-threaded safe,
		/// which means that this <code>QuadCurve2D</code> class does not
		/// guarantee that modifications to the geometry of this
		/// <code>QuadCurve2D</code> object do not affect any iterations of
		/// that geometry that are already in process. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to apply
		///          to the boundary of the shape </param>
		/// <param name="flatness"> the maximum distance that the control points for a
		///          subdivided curve can be with respect to a line connecting
		///          the end points of this curve before this curve is
		///          replaced by a straight line connecting the end points. </param>
		/// <returns> a <code>PathIterator</code> object that defines the
		///          flattened boundary of the shape.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at, double flatness)
		{
			return new FlatteningPathIterator(GetPathIterator(at), flatness);
		}

		/// <summary>
		/// Creates a new object of the same class and with the same contents
		/// as this object.
		/// </summary>
		/// <returns>     a clone of this instance. </returns>
		/// <exception cref="OutOfMemoryError">            if there is not enough memory. </exception>
		/// <seealso cref=        java.lang.Cloneable
		/// @since      1.2 </seealso>
		public virtual Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError(e);
			}
		}
	}

}