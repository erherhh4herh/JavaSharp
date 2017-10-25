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
	/// The <code>RoundRectangle2D</code> class defines a rectangle with
	/// rounded corners defined by a location {@code (x,y)}, a
	/// dimension {@code (w x h)}, and the width and height of an arc
	/// with which to round the corners.
	/// <para>
	/// This class is the abstract superclass for all objects that
	/// store a 2D rounded rectangle.
	/// The actual storage representation of the coordinates is left to
	/// the subclass.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public abstract class RoundRectangle2D : RectangularShape
	{

		/// <summary>
		/// The <code>Float</code> class defines a rectangle with rounded
		/// corners all specified in <code>float</code> coordinates.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Float : RoundRectangle2D
		{
			/// <summary>
			/// The X coordinate of this <code>RoundRectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float x;

			/// <summary>
			/// The Y coordinate of this <code>RoundRectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float y;

			/// <summary>
			/// The width of this <code>RoundRectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Width_Renamed;

			/// <summary>
			/// The height of this <code>RoundRectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Height_Renamed;

			/// <summary>
			/// The width of the arc that rounds off the corners.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Arcwidth;

			/// <summary>
			/// The height of the arc that rounds off the corners.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Archeight;

			/// <summary>
			/// Constructs a new <code>RoundRectangle2D</code>, initialized to
			/// location (0.0,&nbsp;0.0), size (0.0,&nbsp;0.0), and corner arcs
			/// of radius 0.0.
			/// @since 1.2
			/// </summary>
			public Float()
			{
			}

			/// <summary>
			/// Constructs and initializes a <code>RoundRectangle2D</code>
			/// from the specified <code>float</code> coordinates.
			/// </summary>
			/// <param name="x"> the X coordinate of the newly
			///          constructed <code>RoundRectangle2D</code> </param>
			/// <param name="y"> the Y coordinate of the newly
			///          constructed <code>RoundRectangle2D</code> </param>
			/// <param name="w"> the width to which to set the newly
			///          constructed <code>RoundRectangle2D</code> </param>
			/// <param name="h"> the height to which to set the newly
			///          constructed <code>RoundRectangle2D</code> </param>
			/// <param name="arcw"> the width of the arc to use to round off the
			///             corners of the newly constructed
			///             <code>RoundRectangle2D</code> </param>
			/// <param name="arch"> the height of the arc to use to round off the
			///             corners of the newly constructed
			///             <code>RoundRectangle2D</code>
			/// @since 1.2 </param>
			public Float(float x, float y, float w, float h, float arcw, float arch)
			{
				SetRoundRect(x, y, w, h, arcw, arch);
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
			public override double ArcWidth
			{
				get
				{
					return (double) Arcwidth;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double ArcHeight
			{
				get
				{
					return (double) Archeight;
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
					return (Width_Renamed <= 0.0f) || (Height_Renamed <= 0.0f);
				}
			}

			/// <summary>
			/// Sets the location, size, and corner radii of this
			/// <code>RoundRectangle2D</code> to the specified
			/// <code>float</code> values.
			/// </summary>
			/// <param name="x"> the X coordinate to which to set the
			///          location of this <code>RoundRectangle2D</code> </param>
			/// <param name="y"> the Y coordinate to which to set the
			///          location of this <code>RoundRectangle2D</code> </param>
			/// <param name="w"> the width to which to set this
			///          <code>RoundRectangle2D</code> </param>
			/// <param name="h"> the height to which to set this
			///          <code>RoundRectangle2D</code> </param>
			/// <param name="arcw"> the width to which to set the arc of this
			///             <code>RoundRectangle2D</code> </param>
			/// <param name="arch"> the height to which to set the arc of this
			///             <code>RoundRectangle2D</code>
			/// @since 1.2 </param>
			public virtual void SetRoundRect(float x, float y, float w, float h, float arcw, float arch)
			{
				this.x = x;
				this.y = y;
				this.Width_Renamed = w;
				this.Height_Renamed = h;
				this.Arcwidth = arcw;
				this.Archeight = arch;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override void SetRoundRect(double x, double y, double w, double h, double arcw, double arch)
			{
				this.x = (float) x;
				this.y = (float) y;
				this.Width_Renamed = (float) w;
				this.Height_Renamed = (float) h;
				this.Arcwidth = (float) arcw;
				this.Archeight = (float) arch;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override RoundRectangle2D RoundRect
			{
				set
				{
					this.x = (float) value.X;
					this.y = (float) value.Y;
					this.Width_Renamed = (float) value.Width;
					this.Height_Renamed = (float) value.Height;
					this.Arcwidth = (float) value.ArcWidth;
					this.Archeight = (float) value.ArcHeight;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D Bounds2D
			{
				get
				{
					return new Rectangle2D.Float(x, y, Width_Renamed, Height_Renamed);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = -3423150618393866922L;
		}

		/// <summary>
		/// The <code>Double</code> class defines a rectangle with rounded
		/// corners all specified in <code>double</code> coordinates.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Double : RoundRectangle2D
		{
			/// <summary>
			/// The X coordinate of this <code>RoundRectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double x;

			/// <summary>
			/// The Y coordinate of this <code>RoundRectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double y;

			/// <summary>
			/// The width of this <code>RoundRectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Width_Renamed;

			/// <summary>
			/// The height of this <code>RoundRectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Height_Renamed;

			/// <summary>
			/// The width of the arc that rounds off the corners.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Arcwidth;

			/// <summary>
			/// The height of the arc that rounds off the corners.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Archeight;

			/// <summary>
			/// Constructs a new <code>RoundRectangle2D</code>, initialized to
			/// location (0.0,&nbsp;0.0), size (0.0,&nbsp;0.0), and corner arcs
			/// of radius 0.0.
			/// @since 1.2
			/// </summary>
			public Double()
			{
			}

			/// <summary>
			/// Constructs and initializes a <code>RoundRectangle2D</code>
			/// from the specified <code>double</code> coordinates.
			/// </summary>
			/// <param name="x"> the X coordinate of the newly
			///          constructed <code>RoundRectangle2D</code> </param>
			/// <param name="y"> the Y coordinate of the newly
			///          constructed <code>RoundRectangle2D</code> </param>
			/// <param name="w"> the width to which to set the newly
			///          constructed <code>RoundRectangle2D</code> </param>
			/// <param name="h"> the height to which to set the newly
			///          constructed <code>RoundRectangle2D</code> </param>
			/// <param name="arcw"> the width of the arc to use to round off the
			///             corners of the newly constructed
			///             <code>RoundRectangle2D</code> </param>
			/// <param name="arch"> the height of the arc to use to round off the
			///             corners of the newly constructed
			///             <code>RoundRectangle2D</code>
			/// @since 1.2 </param>
			public Double(double x, double y, double w, double h, double arcw, double arch)
			{
				SetRoundRect(x, y, w, h, arcw, arch);
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
			public override double ArcWidth
			{
				get
				{
					return Arcwidth;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double ArcHeight
			{
				get
				{
					return Archeight;
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
					return (Width_Renamed <= 0.0f) || (Height_Renamed <= 0.0f);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override void SetRoundRect(double x, double y, double w, double h, double arcw, double arch)
			{
				this.x = x;
				this.y = y;
				this.Width_Renamed = w;
				this.Height_Renamed = h;
				this.Arcwidth = arcw;
				this.Archeight = arch;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override RoundRectangle2D RoundRect
			{
				set
				{
					this.x = value.X;
					this.y = value.Y;
					this.Width_Renamed = value.Width;
					this.Height_Renamed = value.Height;
					this.Arcwidth = value.ArcWidth;
					this.Archeight = value.ArcHeight;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D Bounds2D
			{
				get
				{
					return new Rectangle2D.Double(x, y, Width_Renamed, Height_Renamed);
				}
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 1048939333485206117L;
		}

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessor
		/// methods below.
		/// </summary>
		/// <seealso cref= java.awt.geom.RoundRectangle2D.Float </seealso>
		/// <seealso cref= java.awt.geom.RoundRectangle2D.Double
		/// @since 1.2 </seealso>
		protected internal RoundRectangle2D()
		{
		}

		/// <summary>
		/// Gets the width of the arc that rounds off the corners. </summary>
		/// <returns> the width of the arc that rounds off the corners
		/// of this <code>RoundRectangle2D</code>.
		/// @since 1.2 </returns>
		public abstract double ArcWidth {get;}

		/// <summary>
		/// Gets the height of the arc that rounds off the corners. </summary>
		/// <returns> the height of the arc that rounds off the corners
		/// of this <code>RoundRectangle2D</code>.
		/// @since 1.2 </returns>
		public abstract double ArcHeight {get;}

		/// <summary>
		/// Sets the location, size, and corner radii of this
		/// <code>RoundRectangle2D</code> to the specified
		/// <code>double</code> values.
		/// </summary>
		/// <param name="x"> the X coordinate to which to set the
		///          location of this <code>RoundRectangle2D</code> </param>
		/// <param name="y"> the Y coordinate to which to set the
		///          location of this <code>RoundRectangle2D</code> </param>
		/// <param name="w"> the width to which to set this
		///          <code>RoundRectangle2D</code> </param>
		/// <param name="h"> the height to which to set this
		///          <code>RoundRectangle2D</code> </param>
		/// <param name="arcWidth"> the width to which to set the arc of this
		///                 <code>RoundRectangle2D</code> </param>
		/// <param name="arcHeight"> the height to which to set the arc of this
		///                  <code>RoundRectangle2D</code>
		/// @since 1.2 </param>
		public abstract void SetRoundRect(double x, double y, double w, double h, double arcWidth, double arcHeight);

		/// <summary>
		/// Sets this <code>RoundRectangle2D</code> to be the same as the
		/// specified <code>RoundRectangle2D</code>. </summary>
		/// <param name="rr"> the specified <code>RoundRectangle2D</code>
		/// @since 1.2 </param>
		public virtual RoundRectangle2D RoundRect
		{
			set
			{
				SetRoundRect(value.X, value.Y, value.Width, value.Height, value.ArcWidth, value.ArcHeight);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override void SetFrame(double x, double y, double w, double h)
		{
			SetRoundRect(x, y, w, h, ArcWidth, ArcHeight);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override bool Contains(double x, double y)
		{
			if (Empty)
			{
				return false;
			}
			double rrx0 = X;
			double rry0 = Y;
			double rrx1 = rrx0 + Width;
			double rry1 = rry0 + Height;
			// Check for trivial rejection - point is outside bounding rectangle
			if (x < rrx0 || y < rry0 || x >= rrx1 || y >= rry1)
			{
				return false;
			}
			double aw = System.Math.Min(Width, System.Math.Abs(ArcWidth)) / 2.0;
			double ah = System.Math.Min(Height, System.Math.Abs(ArcHeight)) / 2.0;
			// Check which corner point is in and do circular containment
			// test - otherwise simple acceptance
			if (x >= (rrx0 += aw) && x < (rrx0 = rrx1 - aw))
			{
				return true;
			}
			if (y >= (rry0 += ah) && y < (rry0 = rry1 - ah))
			{
				return true;
			}
			x = (x - rrx0) / aw;
			y = (y - rry0) / ah;
			return (x * x + y * y <= 1.0);
		}

		private int Classify(double coord, double left, double right, double arcsize)
		{
			if (coord < left)
			{
				return 0;
			}
			else if (coord < left + arcsize)
			{
				return 1;
			}
			else if (coord < right - arcsize)
			{
				return 2;
			}
			else if (coord < right)
			{
				return 3;
			}
			else
			{
				return 4;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override bool Intersects(double x, double y, double w, double h)
		{
			if (Empty || w <= 0 || h <= 0)
			{
				return false;
			}
			double rrx0 = X;
			double rry0 = Y;
			double rrx1 = rrx0 + Width;
			double rry1 = rry0 + Height;
			// Check for trivial rejection - bounding rectangles do not intersect
			if (x + w <= rrx0 || x >= rrx1 || y + h <= rry0 || y >= rry1)
			{
				return false;
			}
			double aw = System.Math.Min(Width, System.Math.Abs(ArcWidth)) / 2.0;
			double ah = System.Math.Min(Height, System.Math.Abs(ArcHeight)) / 2.0;
			int x0class = Classify(x, rrx0, rrx1, aw);
			int x1class = Classify(x + w, rrx0, rrx1, aw);
			int y0class = Classify(y, rry0, rry1, ah);
			int y1class = Classify(y + h, rry0, rry1, ah);
			// Trivially accept if any point is inside inner rectangle
			if (x0class == 2 || x1class == 2 || y0class == 2 || y1class == 2)
			{
				return true;
			}
			// Trivially accept if either edge spans inner rectangle
			if ((x0class < 2 && x1class > 2) || (y0class < 2 && y1class > 2))
			{
				return true;
			}
			// Since neither edge spans the center, then one of the corners
			// must be in one of the rounded edges.  We detect this case if
			// a [xy]0class is 3 or a [xy]1class is 1.  One of those two cases
			// must be true for each direction.
			// We now find a "nearest point" to test for being inside a rounded
			// corner.
			x = (x1class == 1) ? (x = x + w - (rrx0 + aw)) : (x = x - (rrx1 - aw));
			y = (y1class == 1) ? (y = y + h - (rry0 + ah)) : (y = y - (rry1 - ah));
			x = x / aw;
			y = y / ah;
			return (x * x + y * y <= 1.0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override bool Contains(double x, double y, double w, double h)
		{
			if (Empty || w <= 0 || h <= 0)
			{
				return false;
			}
			return (Contains(x, y) && Contains(x + w, y) && Contains(x, y + h) && Contains(x + w, y + h));
		}

		/// <summary>
		/// Returns an iteration object that defines the boundary of this
		/// <code>RoundRectangle2D</code>.
		/// The iterator for this class is multi-threaded safe, which means
		/// that this <code>RoundRectangle2D</code> class guarantees that
		/// modifications to the geometry of this <code>RoundRectangle2D</code>
		/// object do not affect any iterations of that geometry that
		/// are already in process. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to
		/// the coordinates as they are returned in the iteration, or
		/// <code>null</code> if untransformed coordinates are desired </param>
		/// <returns>    the <code>PathIterator</code> object that returns the
		///          geometry of the outline of this
		///          <code>RoundRectangle2D</code>, one segment at a time.
		/// @since 1.2 </returns>
		public override PathIterator GetPathIterator(AffineTransform at)
		{
			return new RoundRectIterator(this, at);
		}

		/// <summary>
		/// Returns the hashcode for this <code>RoundRectangle2D</code>. </summary>
		/// <returns> the hashcode for this <code>RoundRectangle2D</code>.
		/// @since 1.6 </returns>
		public override int HashCode()
		{
			long bits = Double.DoubleToLongBits(X);
			bits += Double.DoubleToLongBits(Y) * 37;
			bits += Double.DoubleToLongBits(Width) * 43;
			bits += Double.DoubleToLongBits(Height) * 47;
			bits += Double.DoubleToLongBits(ArcWidth) * 53;
			bits += Double.DoubleToLongBits(ArcHeight) * 59;
			return (((int) bits) ^ ((int)(bits >> 32)));
		}

		/// <summary>
		/// Determines whether or not the specified <code>Object</code> is
		/// equal to this <code>RoundRectangle2D</code>.  The specified
		/// <code>Object</code> is equal to this <code>RoundRectangle2D</code>
		/// if it is an instance of <code>RoundRectangle2D</code> and if its
		/// location, size, and corner arc dimensions are the same as this
		/// <code>RoundRectangle2D</code>. </summary>
		/// <param name="obj">  an <code>Object</code> to be compared with this
		///             <code>RoundRectangle2D</code>. </param>
		/// <returns>  <code>true</code> if <code>obj</code> is an instance
		///          of <code>RoundRectangle2D</code> and has the same values;
		///          <code>false</code> otherwise.
		/// @since 1.6 </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj is RoundRectangle2D)
			{
				RoundRectangle2D rr2d = (RoundRectangle2D) obj;
				return ((X == rr2d.X) && (Y == rr2d.Y) && (Width == rr2d.Width) && (Height == rr2d.Height) && (ArcWidth == rr2d.ArcWidth) && (ArcHeight == rr2d.ArcHeight));
			}
			return false;
		}
	}

}