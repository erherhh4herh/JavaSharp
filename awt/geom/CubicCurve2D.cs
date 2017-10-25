using System;

/*
 * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static Math.abs;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static Math.max;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static Math.ulp;

	/// <summary>
	/// The <code>CubicCurve2D</code> class defines a cubic parametric curve
	/// segment in {@code (x,y)} coordinate space.
	/// <para>
	/// This class is only the abstract superclass for all objects which
	/// store a 2D cubic curve segment.
	/// The actual storage representation of the coordinates is left to
	/// the subclass.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public abstract class CubicCurve2D : Shape, Cloneable
	{
		public abstract java.awt.geom.Rectangle2D Bounds2D {get;}

		/// <summary>
		/// A cubic parametric curve segment specified with
		/// {@code float} coordinates.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Float : CubicCurve2D
		{
			/// <summary>
			/// The X coordinate of the start point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float X1_Renamed;

			/// <summary>
			/// The Y coordinate of the start point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Y1_Renamed;

			/// <summary>
			/// The X coordinate of the first control point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Ctrlx1;

			/// <summary>
			/// The Y coordinate of the first control point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Ctrly1;

			/// <summary>
			/// The X coordinate of the second control point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Ctrlx2;

			/// <summary>
			/// The Y coordinate of the second control point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Ctrly2;

			/// <summary>
			/// The X coordinate of the end point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float X2_Renamed;

			/// <summary>
			/// The Y coordinate of the end point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Y2_Renamed;

			/// <summary>
			/// Constructs and initializes a CubicCurve with coordinates
			/// (0, 0, 0, 0, 0, 0, 0, 0).
			/// @since 1.2
			/// </summary>
			public Float()
			{
			}

			/// <summary>
			/// Constructs and initializes a {@code CubicCurve2D} from
			/// the specified {@code float} coordinates.
			/// </summary>
			/// <param name="x1"> the X coordinate for the start point
			///           of the resulting {@code CubicCurve2D} </param>
			/// <param name="y1"> the Y coordinate for the start point
			///           of the resulting {@code CubicCurve2D} </param>
			/// <param name="ctrlx1"> the X coordinate for the first control point
			///               of the resulting {@code CubicCurve2D} </param>
			/// <param name="ctrly1"> the Y coordinate for the first control point
			///               of the resulting {@code CubicCurve2D} </param>
			/// <param name="ctrlx2"> the X coordinate for the second control point
			///               of the resulting {@code CubicCurve2D} </param>
			/// <param name="ctrly2"> the Y coordinate for the second control point
			///               of the resulting {@code CubicCurve2D} </param>
			/// <param name="x2"> the X coordinate for the end point
			///           of the resulting {@code CubicCurve2D} </param>
			/// <param name="y2"> the Y coordinate for the end point
			///           of the resulting {@code CubicCurve2D}
			/// @since 1.2 </param>
			public Float(float x1, float y1, float ctrlx1, float ctrly1, float ctrlx2, float ctrly2, float x2, float y2)
			{
				SetCurve(x1, y1, ctrlx1, ctrly1, ctrlx2, ctrly2, x2, y2);
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
			public override double CtrlX1
			{
				get
				{
					return (double) Ctrlx1;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlY1
			{
				get
				{
					return (double) Ctrly1;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D CtrlP1
			{
				get
				{
					return new Point2D.Float(Ctrlx1, Ctrly1);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlX2
			{
				get
				{
					return (double) Ctrlx2;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlY2
			{
				get
				{
					return (double) Ctrly2;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D CtrlP2
			{
				get
				{
					return new Point2D.Float(Ctrlx2, Ctrly2);
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
			public override void SetCurve(double x1, double y1, double ctrlx1, double ctrly1, double ctrlx2, double ctrly2, double x2, double y2)
			{
				this.X1_Renamed = (float) x1;
				this.Y1_Renamed = (float) y1;
				this.Ctrlx1 = (float) ctrlx1;
				this.Ctrly1 = (float) ctrly1;
				this.Ctrlx2 = (float) ctrlx2;
				this.Ctrly2 = (float) ctrly2;
				this.X2_Renamed = (float) x2;
				this.Y2_Renamed = (float) y2;
			}

			/// <summary>
			/// Sets the location of the end points and control points
			/// of this curve to the specified {@code float} coordinates.
			/// </summary>
			/// <param name="x1"> the X coordinate used to set the start point
			///           of this {@code CubicCurve2D} </param>
			/// <param name="y1"> the Y coordinate used to set the start point
			///           of this {@code CubicCurve2D} </param>
			/// <param name="ctrlx1"> the X coordinate used to set the first control point
			///               of this {@code CubicCurve2D} </param>
			/// <param name="ctrly1"> the Y coordinate used to set the first control point
			///               of this {@code CubicCurve2D} </param>
			/// <param name="ctrlx2"> the X coordinate used to set the second control point
			///               of this {@code CubicCurve2D} </param>
			/// <param name="ctrly2"> the Y coordinate used to set the second control point
			///               of this {@code CubicCurve2D} </param>
			/// <param name="x2"> the X coordinate used to set the end point
			///           of this {@code CubicCurve2D} </param>
			/// <param name="y2"> the Y coordinate used to set the end point
			///           of this {@code CubicCurve2D}
			/// @since 1.2 </param>
			public virtual void SetCurve(float x1, float y1, float ctrlx1, float ctrly1, float ctrlx2, float ctrly2, float x2, float y2)
			{
				this.X1_Renamed = x1;
				this.Y1_Renamed = y1;
				this.Ctrlx1 = ctrlx1;
				this.Ctrly1 = ctrly1;
				this.Ctrlx2 = ctrlx2;
				this.Ctrly2 = ctrly2;
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
					float left = System.Math.Min(System.Math.Min(X1_Renamed, X2_Renamed), System.Math.Min(Ctrlx1, Ctrlx2));
					float top = System.Math.Min(System.Math.Min(Y1_Renamed, Y2_Renamed), System.Math.Min(Ctrly1, Ctrly2));
					float right = System.Math.Max(System.Math.Max(X1_Renamed, X2_Renamed), System.Math.Max(Ctrlx1, Ctrlx2));
					float bottom = System.Math.Max(System.Math.Max(Y1_Renamed, Y2_Renamed), System.Math.Max(Ctrly1, Ctrly2));
					return new Rectangle2D.Float(left, top, right - left, bottom - top);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = -1272015596714244385L;
		}

		/// <summary>
		/// A cubic parametric curve segment specified with
		/// {@code double} coordinates.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Double : CubicCurve2D
		{
			/// <summary>
			/// The X coordinate of the start point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double X1_Renamed;

			/// <summary>
			/// The Y coordinate of the start point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Y1_Renamed;

			/// <summary>
			/// The X coordinate of the first control point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Ctrlx1;

			/// <summary>
			/// The Y coordinate of the first control point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Ctrly1;

			/// <summary>
			/// The X coordinate of the second control point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Ctrlx2;

			/// <summary>
			/// The Y coordinate of the second control point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Ctrly2;

			/// <summary>
			/// The X coordinate of the end point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double X2_Renamed;

			/// <summary>
			/// The Y coordinate of the end point
			/// of the cubic curve segment.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Y2_Renamed;

			/// <summary>
			/// Constructs and initializes a CubicCurve with coordinates
			/// (0, 0, 0, 0, 0, 0, 0, 0).
			/// @since 1.2
			/// </summary>
			public Double()
			{
			}

			/// <summary>
			/// Constructs and initializes a {@code CubicCurve2D} from
			/// the specified {@code double} coordinates.
			/// </summary>
			/// <param name="x1"> the X coordinate for the start point
			///           of the resulting {@code CubicCurve2D} </param>
			/// <param name="y1"> the Y coordinate for the start point
			///           of the resulting {@code CubicCurve2D} </param>
			/// <param name="ctrlx1"> the X coordinate for the first control point
			///               of the resulting {@code CubicCurve2D} </param>
			/// <param name="ctrly1"> the Y coordinate for the first control point
			///               of the resulting {@code CubicCurve2D} </param>
			/// <param name="ctrlx2"> the X coordinate for the second control point
			///               of the resulting {@code CubicCurve2D} </param>
			/// <param name="ctrly2"> the Y coordinate for the second control point
			///               of the resulting {@code CubicCurve2D} </param>
			/// <param name="x2"> the X coordinate for the end point
			///           of the resulting {@code CubicCurve2D} </param>
			/// <param name="y2"> the Y coordinate for the end point
			///           of the resulting {@code CubicCurve2D}
			/// @since 1.2 </param>
			public Double(double x1, double y1, double ctrlx1, double ctrly1, double ctrlx2, double ctrly2, double x2, double y2)
			{
				SetCurve(x1, y1, ctrlx1, ctrly1, ctrlx2, ctrly2, x2, y2);
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
			public override double CtrlX1
			{
				get
				{
					return Ctrlx1;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlY1
			{
				get
				{
					return Ctrly1;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D CtrlP1
			{
				get
				{
					return new Point2D.Double(Ctrlx1, Ctrly1);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlX2
			{
				get
				{
					return Ctrlx2;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double CtrlY2
			{
				get
				{
					return Ctrly2;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Point2D CtrlP2
			{
				get
				{
					return new Point2D.Double(Ctrlx2, Ctrly2);
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
			public override void SetCurve(double x1, double y1, double ctrlx1, double ctrly1, double ctrlx2, double ctrly2, double x2, double y2)
			{
				this.X1_Renamed = x1;
				this.Y1_Renamed = y1;
				this.Ctrlx1 = ctrlx1;
				this.Ctrly1 = ctrly1;
				this.Ctrlx2 = ctrlx2;
				this.Ctrly2 = ctrly2;
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
					double left = System.Math.Min(System.Math.Min(X1_Renamed, X2_Renamed), System.Math.Min(Ctrlx1, Ctrlx2));
					double top = System.Math.Min(System.Math.Min(Y1_Renamed, Y2_Renamed), System.Math.Min(Ctrly1, Ctrly2));
					double right = System.Math.Max(System.Math.Max(X1_Renamed, X2_Renamed), System.Math.Max(Ctrlx1, Ctrlx2));
					double bottom = System.Math.Max(System.Math.Max(Y1_Renamed, Y2_Renamed), System.Math.Max(Ctrly1, Ctrly2));
					return new Rectangle2D.Double(left, top, right - left, bottom - top);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = -4202960122839707295L;
		}

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessor
		/// methods below.
		/// </summary>
		/// <seealso cref= java.awt.geom.CubicCurve2D.Float </seealso>
		/// <seealso cref= java.awt.geom.CubicCurve2D.Double
		/// @since 1.2 </seealso>
		protected internal CubicCurve2D()
		{
		}

		/// <summary>
		/// Returns the X coordinate of the start point in double precision. </summary>
		/// <returns> the X coordinate of the start point of the
		///         {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract double X1 {get;}

		/// <summary>
		/// Returns the Y coordinate of the start point in double precision. </summary>
		/// <returns> the Y coordinate of the start point of the
		///         {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract double Y1 {get;}

		/// <summary>
		/// Returns the start point. </summary>
		/// <returns> a {@code Point2D} that is the start point of
		///         the {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract Point2D P1 {get;}

		/// <summary>
		/// Returns the X coordinate of the first control point in double precision. </summary>
		/// <returns> the X coordinate of the first control point of the
		///         {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract double CtrlX1 {get;}

		/// <summary>
		/// Returns the Y coordinate of the first control point in double precision. </summary>
		/// <returns> the Y coordinate of the first control point of the
		///         {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract double CtrlY1 {get;}

		/// <summary>
		/// Returns the first control point. </summary>
		/// <returns> a {@code Point2D} that is the first control point of
		///         the {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract Point2D CtrlP1 {get;}

		/// <summary>
		/// Returns the X coordinate of the second control point
		/// in double precision. </summary>
		/// <returns> the X coordinate of the second control point of the
		///         {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract double CtrlX2 {get;}

		/// <summary>
		/// Returns the Y coordinate of the second control point
		/// in double precision. </summary>
		/// <returns> the Y coordinate of the second control point of the
		///         {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract double CtrlY2 {get;}

		/// <summary>
		/// Returns the second control point. </summary>
		/// <returns> a {@code Point2D} that is the second control point of
		///         the {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract Point2D CtrlP2 {get;}

		/// <summary>
		/// Returns the X coordinate of the end point in double precision. </summary>
		/// <returns> the X coordinate of the end point of the
		///         {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract double X2 {get;}

		/// <summary>
		/// Returns the Y coordinate of the end point in double precision. </summary>
		/// <returns> the Y coordinate of the end point of the
		///         {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract double Y2 {get;}

		/// <summary>
		/// Returns the end point. </summary>
		/// <returns> a {@code Point2D} that is the end point of
		///         the {@code CubicCurve2D}.
		/// @since 1.2 </returns>
		public abstract Point2D P2 {get;}

		/// <summary>
		/// Sets the location of the end points and control points of this curve
		/// to the specified double coordinates.
		/// </summary>
		/// <param name="x1"> the X coordinate used to set the start point
		///           of this {@code CubicCurve2D} </param>
		/// <param name="y1"> the Y coordinate used to set the start point
		///           of this {@code CubicCurve2D} </param>
		/// <param name="ctrlx1"> the X coordinate used to set the first control point
		///               of this {@code CubicCurve2D} </param>
		/// <param name="ctrly1"> the Y coordinate used to set the first control point
		///               of this {@code CubicCurve2D} </param>
		/// <param name="ctrlx2"> the X coordinate used to set the second control point
		///               of this {@code CubicCurve2D} </param>
		/// <param name="ctrly2"> the Y coordinate used to set the second control point
		///               of this {@code CubicCurve2D} </param>
		/// <param name="x2"> the X coordinate used to set the end point
		///           of this {@code CubicCurve2D} </param>
		/// <param name="y2"> the Y coordinate used to set the end point
		///           of this {@code CubicCurve2D}
		/// @since 1.2 </param>
		public abstract void SetCurve(double x1, double y1, double ctrlx1, double ctrly1, double ctrlx2, double ctrly2, double x2, double y2);

		/// <summary>
		/// Sets the location of the end points and control points of this curve
		/// to the double coordinates at the specified offset in the specified
		/// array. </summary>
		/// <param name="coords"> a double array containing coordinates </param>
		/// <param name="offset"> the index of <code>coords</code> from which to begin
		///          setting the end points and control points of this curve
		///          to the coordinates contained in <code>coords</code>
		/// @since 1.2 </param>
		public virtual void SetCurve(double[] coords, int offset)
		{
			SetCurve(coords[offset + 0], coords[offset + 1], coords[offset + 2], coords[offset + 3], coords[offset + 4], coords[offset + 5], coords[offset + 6], coords[offset + 7]);
		}

		/// <summary>
		/// Sets the location of the end points and control points of this curve
		/// to the specified <code>Point2D</code> coordinates. </summary>
		/// <param name="p1"> the first specified <code>Point2D</code> used to set the
		///          start point of this curve </param>
		/// <param name="cp1"> the second specified <code>Point2D</code> used to set the
		///          first control point of this curve </param>
		/// <param name="cp2"> the third specified <code>Point2D</code> used to set the
		///          second control point of this curve </param>
		/// <param name="p2"> the fourth specified <code>Point2D</code> used to set the
		///          end point of this curve
		/// @since 1.2 </param>
		public virtual void SetCurve(Point2D p1, Point2D cp1, Point2D cp2, Point2D p2)
		{
			SetCurve(p1.X, p1.Y, cp1.X, cp1.Y, cp2.X, cp2.Y, p2.X, p2.Y);
		}

		/// <summary>
		/// Sets the location of the end points and control points of this curve
		/// to the coordinates of the <code>Point2D</code> objects at the specified
		/// offset in the specified array. </summary>
		/// <param name="pts"> an array of <code>Point2D</code> objects </param>
		/// <param name="offset">  the index of <code>pts</code> from which to begin setting
		///          the end points and control points of this curve to the
		///          points contained in <code>pts</code>
		/// @since 1.2 </param>
		public virtual void SetCurve(Point2D[] pts, int offset)
		{
			SetCurve(pts[offset + 0].X, pts[offset + 0].Y, pts[offset + 1].X, pts[offset + 1].Y, pts[offset + 2].X, pts[offset + 2].Y, pts[offset + 3].X, pts[offset + 3].Y);
		}

		/// <summary>
		/// Sets the location of the end points and control points of this curve
		/// to the same as those in the specified <code>CubicCurve2D</code>. </summary>
		/// <param name="c"> the specified <code>CubicCurve2D</code>
		/// @since 1.2 </param>
		public virtual CubicCurve2D Curve
		{
			set
			{
				SetCurve(value.X1, value.Y1, value.CtrlX1, value.CtrlY1, value.CtrlX2, value.CtrlY2, value.X2, value.Y2);
			}
		}

		/// <summary>
		/// Returns the square of the flatness of the cubic curve specified
		/// by the indicated control points. The flatness is the maximum distance
		/// of a control point from the line connecting the end points.
		/// </summary>
		/// <param name="x1"> the X coordinate that specifies the start point
		///           of a {@code CubicCurve2D} </param>
		/// <param name="y1"> the Y coordinate that specifies the start point
		///           of a {@code CubicCurve2D} </param>
		/// <param name="ctrlx1"> the X coordinate that specifies the first control point
		///               of a {@code CubicCurve2D} </param>
		/// <param name="ctrly1"> the Y coordinate that specifies the first control point
		///               of a {@code CubicCurve2D} </param>
		/// <param name="ctrlx2"> the X coordinate that specifies the second control point
		///               of a {@code CubicCurve2D} </param>
		/// <param name="ctrly2"> the Y coordinate that specifies the second control point
		///               of a {@code CubicCurve2D} </param>
		/// <param name="x2"> the X coordinate that specifies the end point
		///           of a {@code CubicCurve2D} </param>
		/// <param name="y2"> the Y coordinate that specifies the end point
		///           of a {@code CubicCurve2D} </param>
		/// <returns> the square of the flatness of the {@code CubicCurve2D}
		///          represented by the specified coordinates.
		/// @since 1.2 </returns>
		public static double GetFlatnessSq(double x1, double y1, double ctrlx1, double ctrly1, double ctrlx2, double ctrly2, double x2, double y2)
		{
			return System.Math.Max(Line2D.PtSegDistSq(x1, y1, x2, y2, ctrlx1, ctrly1), Line2D.PtSegDistSq(x1, y1, x2, y2, ctrlx2, ctrly2));

		}

		/// <summary>
		/// Returns the flatness of the cubic curve specified
		/// by the indicated control points. The flatness is the maximum distance
		/// of a control point from the line connecting the end points.
		/// </summary>
		/// <param name="x1"> the X coordinate that specifies the start point
		///           of a {@code CubicCurve2D} </param>
		/// <param name="y1"> the Y coordinate that specifies the start point
		///           of a {@code CubicCurve2D} </param>
		/// <param name="ctrlx1"> the X coordinate that specifies the first control point
		///               of a {@code CubicCurve2D} </param>
		/// <param name="ctrly1"> the Y coordinate that specifies the first control point
		///               of a {@code CubicCurve2D} </param>
		/// <param name="ctrlx2"> the X coordinate that specifies the second control point
		///               of a {@code CubicCurve2D} </param>
		/// <param name="ctrly2"> the Y coordinate that specifies the second control point
		///               of a {@code CubicCurve2D} </param>
		/// <param name="x2"> the X coordinate that specifies the end point
		///           of a {@code CubicCurve2D} </param>
		/// <param name="y2"> the Y coordinate that specifies the end point
		///           of a {@code CubicCurve2D} </param>
		/// <returns> the flatness of the {@code CubicCurve2D}
		///          represented by the specified coordinates.
		/// @since 1.2 </returns>
		public static double GetFlatness(double x1, double y1, double ctrlx1, double ctrly1, double ctrlx2, double ctrly2, double x2, double y2)
		{
			return System.Math.Sqrt(GetFlatnessSq(x1, y1, ctrlx1, ctrly1, ctrlx2, ctrly2, x2, y2));
		}

		/// <summary>
		/// Returns the square of the flatness of the cubic curve specified
		/// by the control points stored in the indicated array at the
		/// indicated index. The flatness is the maximum distance
		/// of a control point from the line connecting the end points. </summary>
		/// <param name="coords"> an array containing coordinates </param>
		/// <param name="offset"> the index of <code>coords</code> from which to begin
		///          getting the end points and control points of the curve </param>
		/// <returns> the square of the flatness of the <code>CubicCurve2D</code>
		///          specified by the coordinates in <code>coords</code> at
		///          the specified offset.
		/// @since 1.2 </returns>
		public static double GetFlatnessSq(double[] coords, int offset)
		{
			return GetFlatnessSq(coords[offset + 0], coords[offset + 1], coords[offset + 2], coords[offset + 3], coords[offset + 4], coords[offset + 5], coords[offset + 6], coords[offset + 7]);
		}

		/// <summary>
		/// Returns the flatness of the cubic curve specified
		/// by the control points stored in the indicated array at the
		/// indicated index.  The flatness is the maximum distance
		/// of a control point from the line connecting the end points. </summary>
		/// <param name="coords"> an array containing coordinates </param>
		/// <param name="offset"> the index of <code>coords</code> from which to begin
		///          getting the end points and control points of the curve </param>
		/// <returns> the flatness of the <code>CubicCurve2D</code>
		///          specified by the coordinates in <code>coords</code> at
		///          the specified offset.
		/// @since 1.2 </returns>
		public static double GetFlatness(double[] coords, int offset)
		{
			return GetFlatness(coords[offset + 0], coords[offset + 1], coords[offset + 2], coords[offset + 3], coords[offset + 4], coords[offset + 5], coords[offset + 6], coords[offset + 7]);
		}

		/// <summary>
		/// Returns the square of the flatness of this curve.  The flatness is the
		/// maximum distance of a control point from the line connecting the
		/// end points. </summary>
		/// <returns> the square of the flatness of this curve.
		/// @since 1.2 </returns>
		public virtual double FlatnessSq
		{
			get
			{
				return GetFlatnessSq(X1, Y1, CtrlX1, CtrlY1, CtrlX2, CtrlY2, X2, Y2);
			}
		}

		/// <summary>
		/// Returns the flatness of this curve.  The flatness is the
		/// maximum distance of a control point from the line connecting the
		/// end points. </summary>
		/// <returns> the flatness of this curve.
		/// @since 1.2 </returns>
		public virtual double Flatness
		{
			get
			{
				return GetFlatness(X1, Y1, CtrlX1, CtrlY1, CtrlX2, CtrlY2, X2, Y2);
			}
		}

		/// <summary>
		/// Subdivides this cubic curve and stores the resulting two
		/// subdivided curves into the left and right curve parameters.
		/// Either or both of the left and right objects may be the same
		/// as this object or null. </summary>
		/// <param name="left"> the cubic curve object for storing for the left or
		/// first half of the subdivided curve </param>
		/// <param name="right"> the cubic curve object for storing for the right or
		/// second half of the subdivided curve
		/// @since 1.2 </param>
		public virtual void Subdivide(CubicCurve2D left, CubicCurve2D right)
		{
			Subdivide(this, left, right);
		}

		/// <summary>
		/// Subdivides the cubic curve specified by the <code>src</code> parameter
		/// and stores the resulting two subdivided curves into the
		/// <code>left</code> and <code>right</code> curve parameters.
		/// Either or both of the <code>left</code> and <code>right</code> objects
		/// may be the same as the <code>src</code> object or <code>null</code>. </summary>
		/// <param name="src"> the cubic curve to be subdivided </param>
		/// <param name="left"> the cubic curve object for storing the left or
		/// first half of the subdivided curve </param>
		/// <param name="right"> the cubic curve object for storing the right or
		/// second half of the subdivided curve
		/// @since 1.2 </param>
		public static void Subdivide(CubicCurve2D src, CubicCurve2D left, CubicCurve2D right)
		{
			double x1 = src.X1;
			double y1 = src.Y1;
			double ctrlx1 = src.CtrlX1;
			double ctrly1 = src.CtrlY1;
			double ctrlx2 = src.CtrlX2;
			double ctrly2 = src.CtrlY2;
			double x2 = src.X2;
			double y2 = src.Y2;
			double centerx = (ctrlx1 + ctrlx2) / 2.0;
			double centery = (ctrly1 + ctrly2) / 2.0;
			ctrlx1 = (x1 + ctrlx1) / 2.0;
			ctrly1 = (y1 + ctrly1) / 2.0;
			ctrlx2 = (x2 + ctrlx2) / 2.0;
			ctrly2 = (y2 + ctrly2) / 2.0;
			double ctrlx12 = (ctrlx1 + centerx) / 2.0;
			double ctrly12 = (ctrly1 + centery) / 2.0;
			double ctrlx21 = (ctrlx2 + centerx) / 2.0;
			double ctrly21 = (ctrly2 + centery) / 2.0;
			centerx = (ctrlx12 + ctrlx21) / 2.0;
			centery = (ctrly12 + ctrly21) / 2.0;
			if (left != null)
			{
				left.SetCurve(x1, y1, ctrlx1, ctrly1, ctrlx12, ctrly12, centerx, centery);
			}
			if (right != null)
			{
				right.SetCurve(centerx, centery, ctrlx21, ctrly21, ctrlx2, ctrly2, x2, y2);
			}
		}

		/// <summary>
		/// Subdivides the cubic curve specified by the coordinates
		/// stored in the <code>src</code> array at indices <code>srcoff</code>
		/// through (<code>srcoff</code>&nbsp;+&nbsp;7) and stores the
		/// resulting two subdivided curves into the two result arrays at the
		/// corresponding indices.
		/// Either or both of the <code>left</code> and <code>right</code>
		/// arrays may be <code>null</code> or a reference to the same array
		/// as the <code>src</code> array.
		/// Note that the last point in the first subdivided curve is the
		/// same as the first point in the second subdivided curve. Thus,
		/// it is possible to pass the same array for <code>left</code>
		/// and <code>right</code> and to use offsets, such as <code>rightoff</code>
		/// equals (<code>leftoff</code> + 6), in order
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
			double ctrlx1 = src[srcoff + 2];
			double ctrly1 = src[srcoff + 3];
			double ctrlx2 = src[srcoff + 4];
			double ctrly2 = src[srcoff + 5];
			double x2 = src[srcoff + 6];
			double y2 = src[srcoff + 7];
			if (left != null)
			{
				left[leftoff + 0] = x1;
				left[leftoff + 1] = y1;
			}
			if (right != null)
			{
				right[rightoff + 6] = x2;
				right[rightoff + 7] = y2;
			}
			x1 = (x1 + ctrlx1) / 2.0;
			y1 = (y1 + ctrly1) / 2.0;
			x2 = (x2 + ctrlx2) / 2.0;
			y2 = (y2 + ctrly2) / 2.0;
			double centerx = (ctrlx1 + ctrlx2) / 2.0;
			double centery = (ctrly1 + ctrly2) / 2.0;
			ctrlx1 = (x1 + centerx) / 2.0;
			ctrly1 = (y1 + centery) / 2.0;
			ctrlx2 = (x2 + centerx) / 2.0;
			ctrly2 = (y2 + centery) / 2.0;
			centerx = (ctrlx1 + ctrlx2) / 2.0;
			centery = (ctrly1 + ctrly2) / 2.0;
			if (left != null)
			{
				left[leftoff + 2] = x1;
				left[leftoff + 3] = y1;
				left[leftoff + 4] = ctrlx1;
				left[leftoff + 5] = ctrly1;
				left[leftoff + 6] = centerx;
				left[leftoff + 7] = centery;
			}
			if (right != null)
			{
				right[rightoff + 0] = centerx;
				right[rightoff + 1] = centery;
				right[rightoff + 2] = ctrlx2;
				right[rightoff + 3] = ctrly2;
				right[rightoff + 4] = x2;
				right[rightoff + 5] = y2;
			}
		}

		/// <summary>
		/// Solves the cubic whose coefficients are in the <code>eqn</code>
		/// array and places the non-complex roots back into the same array,
		/// returning the number of roots.  The solved cubic is represented
		/// by the equation:
		/// <pre>
		///     eqn = {c, b, a, d}
		///     dx^3 + ax^2 + bx + c = 0
		/// </pre>
		/// A return value of -1 is used to distinguish a constant equation
		/// that might be always 0 or never 0 from an equation that has no
		/// zeroes. </summary>
		/// <param name="eqn"> an array containing coefficients for a cubic </param>
		/// <returns> the number of roots, or -1 if the equation is a constant.
		/// @since 1.2 </returns>
		public static int SolveCubic(double[] eqn)
		{
			return SolveCubic(eqn, eqn);
		}

		/// <summary>
		/// Solve the cubic whose coefficients are in the <code>eqn</code>
		/// array and place the non-complex roots into the <code>res</code>
		/// array, returning the number of roots.
		/// The cubic solved is represented by the equation:
		///     eqn = {c, b, a, d}
		///     dx^3 + ax^2 + bx + c = 0
		/// A return value of -1 is used to distinguish a constant equation,
		/// which may be always 0 or never 0, from an equation which has no
		/// zeroes. </summary>
		/// <param name="eqn"> the specified array of coefficients to use to solve
		///        the cubic equation </param>
		/// <param name="res"> the array that contains the non-complex roots
		///        resulting from the solution of the cubic equation </param>
		/// <returns> the number of roots, or -1 if the equation is a constant
		/// @since 1.3 </returns>
		public static int SolveCubic(double[] eqn, double[] res)
		{
			// From Graphics Gems:
			// http://tog.acm.org/resources/GraphicsGems/gems/Roots3And4.c
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = eqn[3];
			double d = eqn[3];
			if (d == 0)
			{
				return QuadCurve2D.SolveQuadratic(eqn, res);
			}

			/* normal form: x^3 + Ax^2 + Bx + C = 0 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double A = eqn[2] / d;
			double A = eqn[2] / d;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double B = eqn[1] / d;
			double B = eqn[1] / d;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double C = eqn[0] / d;
			double C = eqn[0] / d;


			//  substitute x = y - A/3 to eliminate quadratic term:
			//     x^3 +Px + Q = 0
			//
			// Since we actually need P/3 and Q/2 for all of the
			// calculations that follow, we will calculate
			// p = P/3
			// q = Q/2
			// instead and use those values for simplicity of the code.
			double sq_A = A * A;
			double p = 1.0 / 3 * (-1.0 / 3 * sq_A + B);
			double q = 1.0 / 2 * (2.0 / 27 * A * sq_A - 1.0 / 3 * A * B + C);

			/* use Cardano's formula */

			double cb_p = p * p * p;
			double D = q * q + cb_p;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sub = 1.0/3 * A;
			double sub = 1.0 / 3 * A;

			int num;
			if (D < 0) // Casus irreducibilis: three real solutions
			{
				// see: http://en.wikipedia.org/wiki/Cubic_function#Trigonometric_.28and_hyperbolic.29_method
				double phi = 1.0 / 3 * System.Math.Acos(-q / System.Math.Sqrt(-cb_p));
				double t = 2 * System.Math.Sqrt(-p);

				if (res == eqn)
				{
					eqn = Arrays.CopyOf(eqn, 4);
				}

				res[0] = (t * System.Math.Cos(phi));
				res[1] = (-t * System.Math.Cos(phi + Math.PI / 3));
				res[2] = (-t * System.Math.Cos(phi - Math.PI / 3));
				num = 3;

				for (int i = 0; i < num; ++i)
				{
					res[i] -= sub;
				}

			}
			else
			{
				// Please see the comment in fixRoots marked 'XXX' before changing
				// any of the code in this case.
				double sqrt_D = System.Math.Sqrt(D);
				double u = Math.Cbrt(sqrt_D - q);
				double v = - Math.Cbrt(sqrt_D + q);
				double uv = u + v;

				num = 1;

				double err = 1200000000 * ulp(abs(uv) + abs(sub));
				if (Iszero(D, err) || Within(u, v, err))
				{
					if (res == eqn)
					{
						eqn = Arrays.CopyOf(eqn, 4);
					}
					res[1] = -(uv / 2) - sub;
					num = 2;
				}
				// this must be done after the potential Arrays.copyOf
				res[0] = uv - sub;
			}

			if (num > 1) // num == 3 || num == 2
			{
				num = FixRoots(eqn, res, num);
			}
			if (num > 2 && (res[2] == res[1] || res[2] == res[0]))
			{
				num--;
			}
			if (num > 1 && res[1] == res[0])
			{
				res[1] = res[--num]; // Copies res[2] to res[1] if needed
			}
			return num;
		}

		// preconditions: eqn != res && eqn[3] != 0 && num > 1
		// This method tries to improve the accuracy of the roots of eqn (which
		// should be in res). It also might eliminate roots in res if it decideds
		// that they're not real roots. It will not check for roots that the
		// computation of res might have missed, so this method should only be
		// used when the roots in res have been computed using an algorithm
		// that never underestimates the number of roots (such as solveCubic above)
		private static int FixRoots(double[] eqn, double[] res, int num)
		{
			double[] intervals = new double[] {eqn[1], 2 * eqn[2], 3 * eqn[3]};
			int critCount = QuadCurve2D.SolveQuadratic(intervals, intervals);
			if (critCount == 2 && intervals[0] == intervals[1])
			{
				critCount--;
			}
			if (critCount == 2 && intervals[0] > intervals[1])
			{
				double tmp = intervals[0];
				intervals[0] = intervals[1];
				intervals[1] = tmp;
			}

			// below we use critCount to possibly filter out roots that shouldn't
			// have been computed. We require that eqn[3] != 0, so eqn is a proper
			// cubic, which means that its limits at -/+inf are -/+inf or +/-inf.
			// Therefore, if critCount==2, the curve is shaped like a sideways S,
			// and it could have 1-3 roots. If critCount==0 it is monotonic, and
			// if critCount==1 it is monotonic with a single point where it is
			// flat. In the last 2 cases there can only be 1 root. So in cases
			// where num > 1 but critCount < 2, we eliminate all roots in res
			// except one.

			if (num == 3)
			{
				double xe = GetRootUpperBound(eqn);
				double x0 = -xe;

				Arrays.Sort(res, 0, num);
				if (critCount == 2)
				{
					// this just tries to improve the accuracy of the computed
					// roots using Newton's method.
					res[0] = RefineRootWithHint(eqn, x0, intervals[0], res[0]);
					res[1] = RefineRootWithHint(eqn, intervals[0], intervals[1], res[1]);
					res[2] = RefineRootWithHint(eqn, intervals[1], xe, res[2]);
					return 3;
				}
				else if (critCount == 1)
				{
					// we only need fx0 and fxe for the sign of the polynomial
					// at -inf and +inf respectively, so we don't need to do
					// fx0 = solveEqn(eqn, 3, x0); fxe = solveEqn(eqn, 3, xe)
					double fxe = eqn[3];
					double fx0 = -fxe;

					double x1 = intervals[0];
					double fx1 = SolveEqn(eqn, 3, x1);

					// if critCount == 1 or critCount == 0, but num == 3 then
					// something has gone wrong. This branch and the one below
					// would ideally never execute, but if they do we can't know
					// which of the computed roots is closest to the real root;
					// therefore, we can't use refineRootWithHint. But even if
					// we did know, being here most likely means that the
					// curve is very flat close to two of the computed roots
					// (or maybe even all three). This might make Newton's method
					// fail altogether, which would be a pain to detect and fix.
					// This is why we use a very stable bisection method.
					if (OppositeSigns(fx0, fx1))
					{
						res[0] = BisectRootWithHint(eqn, x0, x1, res[0]);
					}
					else if (OppositeSigns(fx1, fxe))
					{
						res[0] = BisectRootWithHint(eqn, x1, xe, res[2]);
					} // fx1 must be 0
					else
					{
						res[0] = x1;
					}
					// return 1
				}
				else if (critCount == 0)
				{
					res[0] = BisectRootWithHint(eqn, x0, xe, res[1]);
					// return 1
				}
			}
			else if (num == 2 && critCount == 2)
			{
				// XXX: here we assume that res[0] has better accuracy than res[1].
				// This is true because this method is only used from solveCubic
				// which puts in res[0] the root that it would compute anyway even
				// if num==1. If this method is ever used from any other method, or
				// if the solveCubic implementation changes, this assumption should
				// be reevaluated, and the choice of goodRoot might have to become
				// goodRoot = (abs(eqn'(res[0])) > abs(eqn'(res[1]))) ? res[0] : res[1]
				// where eqn' is the derivative of eqn.
				double goodRoot = res[0];
				double badRoot = res[1];
				double x1 = intervals[0];
				double x2 = intervals[1];
				// If a cubic curve really has 2 roots, one of those roots must be
				// at a critical point. That can't be goodRoot, so we compute x to
				// be the farthest critical point from goodRoot. If there are two
				// roots, x must be the second one, so we evaluate eqn at x, and if
				// it is zero (or close enough) we put x in res[1] (or badRoot, if
				// |solveEqn(eqn, 3, badRoot)| < |solveEqn(eqn, 3, x)| but this
				// shouldn't happen often).
				double x = abs(x1 - goodRoot) > abs(x2 - goodRoot) ? x1 : x2;
				double fx = SolveEqn(eqn, 3, x);

				if (Iszero(fx, 10000000 * ulp(x)))
				{
					double badRootVal = SolveEqn(eqn, 3, badRoot);
					res[1] = abs(badRootVal) < abs(fx) ? badRoot : x;
					return 2;
				}
			} // else there can only be one root - goodRoot, and it is already in res[0]

			return 1;
		}

		// use newton's method.
		private static double RefineRootWithHint(double[] eqn, double min, double max, double t)
		{
			if (!InInterval(t, min, max))
			{
				return t;
			}
			double[] deriv = new double[] {eqn[1], 2 * eqn[2], 3 * eqn[3]};
			double origt = t;
			for (int i = 0; i < 3; i++)
			{
				double slope = SolveEqn(deriv, 2, t);
				double y = SolveEqn(eqn, 3, t);
				double delta = - (y / slope);
				double newt = t + delta;

				if (slope == 0 || y == 0 || t == newt)
				{
					break;
				}

				t = newt;
			}
			if (Within(t, origt, 1000 * ulp(origt)) && InInterval(t, min, max))
			{
				return t;
			}
			return origt;
		}

		private static double BisectRootWithHint(double[] eqn, double x0, double xe, double hint)
		{
			double delta1 = System.Math.Min(abs(hint - x0) / 64, 0.0625);
			double delta2 = System.Math.Min(abs(hint - xe) / 64, 0.0625);
			double x02 = hint - delta1;
			double xe2 = hint + delta2;
			double fx02 = SolveEqn(eqn, 3, x02);
			double fxe2 = SolveEqn(eqn, 3, xe2);
			while (OppositeSigns(fx02, fxe2))
			{
				if (x02 >= xe2)
				{
					return x02;
				}
				x0 = x02;
				xe = xe2;
				delta1 /= 64;
				delta2 /= 64;
				x02 = hint - delta1;
				xe2 = hint + delta2;
				fx02 = SolveEqn(eqn, 3, x02);
				fxe2 = SolveEqn(eqn, 3, xe2);
			}
			if (fx02 == 0)
			{
				return x02;
			}
			if (fxe2 == 0)
			{
				return xe2;
			}

			return BisectRoot(eqn, x0, xe);
		}

		private static double BisectRoot(double[] eqn, double x0, double xe)
		{
			double fx0 = SolveEqn(eqn, 3, x0);
			double m = x0 + (xe - x0) / 2;
			while (m != x0 && m != xe)
			{
				double fm = SolveEqn(eqn, 3, m);
				if (fm == 0)
				{
					return m;
				}
				if (OppositeSigns(fx0, fm))
				{
					xe = m;
				}
				else
				{
					fx0 = fm;
					x0 = m;
				}
				m = x0 + (xe - x0) / 2;
			}
			return m;
		}

		private static bool InInterval(double t, double min, double max)
		{
			return min <= t && t <= max;
		}

		private static bool Within(double x, double y, double err)
		{
			double d = y - x;
			return (d <= err && d >= -err);
		}

		private static bool Iszero(double x, double err)
		{
			return Within(x, 0, err);
		}

		private static bool OppositeSigns(double x1, double x2)
		{
			return (x1 < 0 && x2 > 0) || (x1 > 0 && x2 < 0);
		}

		private static double SolveEqn(double[] eqn, int order, double t)
		{
			double v = eqn[order];
			while (--order >= 0)
			{
				v = v * t + eqn[order];
			}
			return v;
		}

		/*
		 * Computes M+1 where M is an upper bound for all the roots in of eqn.
		 * See: http://en.wikipedia.org/wiki/Sturm%27s_theorem#Applications.
		 * The above link doesn't contain a proof, but I [dlila] proved it myself
		 * so the result is reliable. The proof isn't difficult, but it's a bit
		 * long to include here.
		 * Precondition: eqn must represent a cubic polynomial
		 */
		private static double GetRootUpperBound(double[] eqn)
		{
			double d = eqn[3];
			double a = eqn[2];
			double b = eqn[1];
			double c = eqn[0];

			double M = 1 + max(max(abs(a), abs(b)), abs(c)) / abs(d);
			M += ulp(M) + 1;
			return M;
		}


		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public virtual bool Contains(double x, double y)
		{
			if (!(x * 0.0 + y * 0.0 == 0.0))
			{
				/* Either x or y was infinite or NaN.
				 * A NaN always produces a negative response to any test
				 * and Infinity values cannot be "inside" any path so
				 * they should return false as well.
				 */
				return false;
			}
			// We count the "Y" crossings to determine if the point is
			// inside the curve bounded by its closing line.
			double x1 = X1;
			double y1 = Y1;
			double x2 = X2;
			double y2 = Y2;
			int crossings = (Curve.pointCrossingsForLine(x, y, x1, y1, x2, y2) + Curve.pointCrossingsForCubic(x, y, x1, y1, CtrlX1, CtrlY1, CtrlX2, CtrlY2, x2, y2, 0));
			return ((crossings & 1) == 1);
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

			int numCrossings = RectCrossings(x, y, w, h);
			// the intended return value is
			// numCrossings != 0 || numCrossings == Curve.RECT_INTERSECTS
			// but if (numCrossings != 0) numCrossings == INTERSECTS won't matter
			// and if !(numCrossings != 0) then numCrossings == 0, so
			// numCrossings != RECT_INTERSECT
			return numCrossings != 0;
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

			int numCrossings = RectCrossings(x, y, w, h);
			return !(numCrossings == 0 || numCrossings == Curve.RECT_INTERSECTS);
		}

		private int RectCrossings(double x, double y, double w, double h)
		{
			int crossings = 0;
			if (!(X1 == X2 && Y1 == Y2))
			{
				crossings = Curve.rectCrossingsForLine(crossings, x, y, x + w, y + h, X1, Y1, X2, Y2);
				if (crossings == Curve.RECT_INTERSECTS)
				{
					return crossings;
				}
			}
			// we call this with the curve's direction reversed, because we wanted
			// to call rectCrossingsForLine first, because it's cheaper.
			return Curve.rectCrossingsForCubic(crossings, x, y, x + w, y + h, X2, Y2, CtrlX2, CtrlY2, CtrlX1, CtrlY1, X1, Y1, 0);
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
		/// shape.
		/// The iterator for this class is not multi-threaded safe,
		/// which means that this <code>CubicCurve2D</code> class does not
		/// guarantee that modifications to the geometry of this
		/// <code>CubicCurve2D</code> object do not affect any iterations of
		/// that geometry that are already in process. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		/// coordinates as they are returned in the iteration, or <code>null</code>
		/// if untransformed coordinates are desired </param>
		/// <returns>    the <code>PathIterator</code> object that returns the
		///          geometry of the outline of this <code>CubicCurve2D</code>, one
		///          segment at a time.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at)
		{
			return new CubicIterator(this, at);
		}

		/// <summary>
		/// Return an iteration object that defines the boundary of the
		/// flattened shape.
		/// The iterator for this class is not multi-threaded safe,
		/// which means that this <code>CubicCurve2D</code> class does not
		/// guarantee that modifications to the geometry of this
		/// <code>CubicCurve2D</code> object do not affect any iterations of
		/// that geometry that are already in process. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		/// coordinates as they are returned in the iteration, or <code>null</code>
		/// if untransformed coordinates are desired </param>
		/// <param name="flatness"> the maximum amount that the control points
		/// for a given curve can vary from colinear before a subdivided
		/// curve is replaced by a straight line connecting the end points </param>
		/// <returns>    the <code>PathIterator</code> object that returns the
		/// geometry of the outline of this <code>CubicCurve2D</code>,
		/// one segment at a time.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at, double flatness)
		{
			return new FlatteningPathIterator(GetPathIterator(at), flatness);
		}

		/// <summary>
		/// Creates a new object of the same class as this object.
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