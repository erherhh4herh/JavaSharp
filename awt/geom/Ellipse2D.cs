using System;

/*
 * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// The <code>Ellipse2D</code> class describes an ellipse that is defined
	/// by a framing rectangle.
	/// <para>
	/// This class is only the abstract superclass for all objects which
	/// store a 2D ellipse.
	/// The actual storage representation of the coordinates is left to
	/// the subclass.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public abstract class Ellipse2D : RectangularShape
	{

		/// <summary>
		/// The <code>Float</code> class defines an ellipse specified
		/// in <code>float</code> precision.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Float : Ellipse2D
		{
			/// <summary>
			/// The X coordinate of the upper-left corner of the
			/// framing rectangle of this {@code Ellipse2D}.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float x;

			/// <summary>
			/// The Y coordinate of the upper-left corner of the
			/// framing rectangle of this {@code Ellipse2D}.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float y;

			/// <summary>
			/// The overall width of this <code>Ellipse2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Width_Renamed;

			/// <summary>
			/// The overall height of this <code>Ellipse2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Height_Renamed;

			/// <summary>
			/// Constructs a new <code>Ellipse2D</code>, initialized to
			/// location (0,&nbsp;0) and size (0,&nbsp;0).
			/// @since 1.2
			/// </summary>
			public Float()
			{
			}

			/// <summary>
			/// Constructs and initializes an <code>Ellipse2D</code> from the
			/// specified coordinates.
			/// </summary>
			/// <param name="x"> the X coordinate of the upper-left corner
			///          of the framing rectangle </param>
			/// <param name="y"> the Y coordinate of the upper-left corner
			///          of the framing rectangle </param>
			/// <param name="w"> the width of the framing rectangle </param>
			/// <param name="h"> the height of the framing rectangle
			/// @since 1.2 </param>
			public Float(float x, float y, float w, float h)
			{
				SetFrame(x, y, w, h);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X
			{
				get
				{
					return (double) x;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y
			{
				get
				{
					return (double) y;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Width
			{
				get
				{
					return (double) Width_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Height
			{
				get
				{
					return (double) Height_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override bool Empty
			{
				get
				{
					return (Width_Renamed <= 0.0 || Height_Renamed <= 0.0);
				}
			}

			/// <summary>
			/// Sets the location and size of the framing rectangle of this
			/// <code>Shape</code> to the specified rectangular values.
			/// </summary>
			/// <param name="x"> the X coordinate of the upper-left corner of the
			///              specified rectangular shape </param>
			/// <param name="y"> the Y coordinate of the upper-left corner of the
			///              specified rectangular shape </param>
			/// <param name="w"> the width of the specified rectangular shape </param>
			/// <param name="h"> the height of the specified rectangular shape
			/// @since 1.2 </param>
			public virtual void SetFrame(float x, float y, float w, float h)
			{
				this.x = x;
				this.y = y;
				this.Width_Renamed = w;
				this.Height_Renamed = h;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override void SetFrame(double x, double y, double w, double h)
			{
				this.x = (float) x;
				this.y = (float) y;
				this.Width_Renamed = (float) w;
				this.Height_Renamed = (float) h;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public virtual Rectangle2D Bounds2D
			{
				get
				{
					return new Rectangle2D.Float(x, y, Width_Renamed, Height_Renamed);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = -6633761252372475977L;
		}

		/// <summary>
		/// The <code>Double</code> class defines an ellipse specified
		/// in <code>double</code> precision.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Double : Ellipse2D
		{
			/// <summary>
			/// The X coordinate of the upper-left corner of the
			/// framing rectangle of this {@code Ellipse2D}.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double x;

			/// <summary>
			/// The Y coordinate of the upper-left corner of the
			/// framing rectangle of this {@code Ellipse2D}.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double y;

			/// <summary>
			/// The overall width of this <code>Ellipse2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Width_Renamed;

			/// <summary>
			/// The overall height of the <code>Ellipse2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Height_Renamed;

			/// <summary>
			/// Constructs a new <code>Ellipse2D</code>, initialized to
			/// location (0,&nbsp;0) and size (0,&nbsp;0).
			/// @since 1.2
			/// </summary>
			public Double()
			{
			}

			/// <summary>
			/// Constructs and initializes an <code>Ellipse2D</code> from the
			/// specified coordinates.
			/// </summary>
			/// <param name="x"> the X coordinate of the upper-left corner
			///        of the framing rectangle </param>
			/// <param name="y"> the Y coordinate of the upper-left corner
			///        of the framing rectangle </param>
			/// <param name="w"> the width of the framing rectangle </param>
			/// <param name="h"> the height of the framing rectangle
			/// @since 1.2 </param>
			public Double(double x, double y, double w, double h)
			{
				SetFrame(x, y, w, h);
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double X
			{
				get
				{
					return x;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Y
			{
				get
				{
					return y;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Width
			{
				get
				{
					return Width_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double Height
			{
				get
				{
					return Height_Renamed;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override bool Empty
			{
				get
				{
					return (Width_Renamed <= 0.0 || Height_Renamed <= 0.0);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override void SetFrame(double x, double y, double w, double h)
			{
				this.x = x;
				this.y = y;
				this.Width_Renamed = w;
				this.Height_Renamed = h;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public virtual Rectangle2D Bounds2D
			{
				get
				{
					return new Rectangle2D.Double(x, y, Width_Renamed, Height_Renamed);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 5555464816372320683L;
		}

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessor
		/// methods below.
		/// </summary>
		/// <seealso cref= java.awt.geom.Ellipse2D.Float </seealso>
		/// <seealso cref= java.awt.geom.Ellipse2D.Double
		/// @since 1.2 </seealso>
		protected internal Ellipse2D()
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override bool Contains(double x, double y)
		{
			// Normalize the coordinates compared to the ellipse
			// having a center at 0,0 and a radius of 0.5.
			double ellw = Width;
			if (ellw <= 0.0)
			{
				return false;
			}
			double normx = (x - X) / ellw - 0.5;
			double ellh = Height;
			if (ellh <= 0.0)
			{
				return false;
			}
			double normy = (y - Y) / ellh - 0.5;
			return (normx * normx + normy * normy) < 0.25;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override bool Intersects(double x, double y, double w, double h)
		{
			if (w <= 0.0 || h <= 0.0)
			{
				return false;
			}
			// Normalize the rectangular coordinates compared to the ellipse
			// having a center at 0,0 and a radius of 0.5.
			double ellw = Width;
			if (ellw <= 0.0)
			{
				return false;
			}
			double normx0 = (x - X) / ellw - 0.5;
			double normx1 = normx0 + w / ellw;
			double ellh = Height;
			if (ellh <= 0.0)
			{
				return false;
			}
			double normy0 = (y - Y) / ellh - 0.5;
			double normy1 = normy0 + h / ellh;
			// find nearest x (left edge, right edge, 0.0)
			// find nearest y (top edge, bottom edge, 0.0)
			// if nearest x,y is inside circle of radius 0.5, then intersects
			double nearx, neary;
			if (normx0 > 0.0)
			{
				// center to left of X extents
				nearx = normx0;
			}
			else if (normx1 < 0.0)
			{
				// center to right of X extents
				nearx = normx1;
			}
			else
			{
				nearx = 0.0;
			}
			if (normy0 > 0.0)
			{
				// center above Y extents
				neary = normy0;
			}
			else if (normy1 < 0.0)
			{
				// center below Y extents
				neary = normy1;
			}
			else
			{
				neary = 0.0;
			}
			return (nearx * nearx + neary * neary) < 0.25;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override bool Contains(double x, double y, double w, double h)
		{
			return (Contains(x, y) && Contains(x + w, y) && Contains(x, y + h) && Contains(x + w, y + h));
		}

		/// <summary>
		/// Returns an iteration object that defines the boundary of this
		/// <code>Ellipse2D</code>.
		/// The iterator for this class is multi-threaded safe, which means
		/// that this <code>Ellipse2D</code> class guarantees that
		/// modifications to the geometry of this <code>Ellipse2D</code>
		/// object do not affect any iterations of that geometry that
		/// are already in process. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to
		/// the coordinates as they are returned in the iteration, or
		/// <code>null</code> if untransformed coordinates are desired </param>
		/// <returns>    the <code>PathIterator</code> object that returns the
		///          geometry of the outline of this <code>Ellipse2D</code>,
		///          one segment at a time.
		/// @since 1.2 </returns>
		public override PathIterator GetPathIterator(AffineTransform at)
		{
			return new EllipseIterator(this, at);
		}

		/// <summary>
		/// Returns the hashcode for this <code>Ellipse2D</code>. </summary>
		/// <returns> the hashcode for this <code>Ellipse2D</code>.
		/// @since 1.6 </returns>
		public override int HashCode()
		{
			long bits = Double.DoubleToLongBits(X);
			bits += Double.DoubleToLongBits(Y) * 37;
			bits += Double.DoubleToLongBits(Width) * 43;
			bits += Double.DoubleToLongBits(Height) * 47;
			return (((int) bits) ^ ((int)(bits >> 32)));
		}

		/// <summary>
		/// Determines whether or not the specified <code>Object</code> is
		/// equal to this <code>Ellipse2D</code>.  The specified
		/// <code>Object</code> is equal to this <code>Ellipse2D</code>
		/// if it is an instance of <code>Ellipse2D</code> and if its
		/// location and size are the same as this <code>Ellipse2D</code>. </summary>
		/// <param name="obj">  an <code>Object</code> to be compared with this
		///             <code>Ellipse2D</code>. </param>
		/// <returns>  <code>true</code> if <code>obj</code> is an instance
		///          of <code>Ellipse2D</code> and has the same values;
		///          <code>false</code> otherwise.
		/// @since 1.6 </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj is Ellipse2D)
			{
				Ellipse2D e2d = (Ellipse2D) obj;
				return ((X == e2d.X) && (Y == e2d.Y) && (Width == e2d.Width) && (Height == e2d.Height));
			}
			return false;
		}
	}

}