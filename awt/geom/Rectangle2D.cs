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
	/// The <code>Rectangle2D</code> class describes a rectangle
	/// defined by a location {@code (x,y)} and dimension
	/// {@code (w x h)}.
	/// <para>
	/// This class is only the abstract superclass for all objects that
	/// store a 2D rectangle.
	/// The actual storage representation of the coordinates is left to
	/// the subclass.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public abstract class Rectangle2D : RectangularShape
	{
		/// <summary>
		/// The bitmask that indicates that a point lies to the left of
		/// this <code>Rectangle2D</code>.
		/// @since 1.2
		/// </summary>
		public const int OUT_LEFT = 1;

		/// <summary>
		/// The bitmask that indicates that a point lies above
		/// this <code>Rectangle2D</code>.
		/// @since 1.2
		/// </summary>
		public const int OUT_TOP = 2;

		/// <summary>
		/// The bitmask that indicates that a point lies to the right of
		/// this <code>Rectangle2D</code>.
		/// @since 1.2
		/// </summary>
		public const int OUT_RIGHT = 4;

		/// <summary>
		/// The bitmask that indicates that a point lies below
		/// this <code>Rectangle2D</code>.
		/// @since 1.2
		/// </summary>
		public const int OUT_BOTTOM = 8;

		/// <summary>
		/// The <code>Float</code> class defines a rectangle specified in float
		/// coordinates.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Float : Rectangle2D
		{
			/// <summary>
			/// The X coordinate of this <code>Rectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float x;

			/// <summary>
			/// The Y coordinate of this <code>Rectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float y;

			/// <summary>
			/// The width of this <code>Rectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Width_Renamed;

			/// <summary>
			/// The height of this <code>Rectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Height_Renamed;

			/// <summary>
			/// Constructs a new <code>Rectangle2D</code>, initialized to
			/// location (0.0,&nbsp;0.0) and size (0.0,&nbsp;0.0).
			/// @since 1.2
			/// </summary>
			public Float()
			{
			}

			/// <summary>
			/// Constructs and initializes a <code>Rectangle2D</code>
			/// from the specified <code>float</code> coordinates.
			/// </summary>
			/// <param name="x"> the X coordinate of the upper-left corner
			///          of the newly constructed <code>Rectangle2D</code> </param>
			/// <param name="y"> the Y coordinate of the upper-left corner
			///          of the newly constructed <code>Rectangle2D</code> </param>
			/// <param name="w"> the width of the newly constructed
			///          <code>Rectangle2D</code> </param>
			/// <param name="h"> the height of the newly constructed
			///          <code>Rectangle2D</code>
			/// @since 1.2 </param>
			public Float(float x, float y, float w, float h)
			{
				SetRect(x, y, w, h);
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
					return (Width_Renamed <= 0.0f) || (Height_Renamed <= 0.0f);
				}
			}

			/// <summary>
			/// Sets the location and size of this <code>Rectangle2D</code>
			/// to the specified <code>float</code> values.
			/// </summary>
			/// <param name="x"> the X coordinate of the upper-left corner
			///          of this <code>Rectangle2D</code> </param>
			/// <param name="y"> the Y coordinate of the upper-left corner
			///          of this <code>Rectangle2D</code> </param>
			/// <param name="w"> the width of this <code>Rectangle2D</code> </param>
			/// <param name="h"> the height of this <code>Rectangle2D</code>
			/// @since 1.2 </param>
			public virtual void SetRect(float x, float y, float w, float h)
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
			public override void SetRect(double x, double y, double w, double h)
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
			public override Rectangle2D Rect
			{
				set
				{
					this.x = (float) value.X;
					this.y = (float) value.Y;
					this.Width_Renamed = (float) value.Width;
					this.Height_Renamed = (float) value.Height;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override int Outcode(double x, double y)
			{
				/*
				 * Note on casts to double below.  If the arithmetic of
				 * x+w or y+h is done in float, then some bits may be
				 * lost if the binary exponents of x/y and w/h are not
				 * similar.  By converting to double before the addition
				 * we force the addition to be carried out in double to
				 * avoid rounding error in the comparison.
				 *
				 * See bug 4320890 for problems that this inaccuracy causes.
				 */
				int @out = 0;
				if (this.Width_Renamed <= 0)
				{
					@out |= OUT_LEFT | OUT_RIGHT;
				}
				else if (x < this.x)
				{
					@out |= OUT_LEFT;
				}
				else if (x > this.x + (double) this.Width_Renamed)
				{
					@out |= OUT_RIGHT;
				}
				if (this.Height_Renamed <= 0)
				{
					@out |= OUT_TOP | OUT_BOTTOM;
				}
				else if (y < this.y)
				{
					@out |= OUT_TOP;
				}
				else if (y > this.y + (double) this.Height_Renamed)
				{
					@out |= OUT_BOTTOM;
				}
				return @out;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D Bounds2D
			{
				get
				{
					return new Float(x, y, Width_Renamed, Height_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D CreateIntersection(Rectangle2D r)
			{
				Rectangle2D dest;
				if (r is Float)
				{
					dest = new Rectangle2D.Float();
				}
				else
				{
					dest = new Rectangle2D.Double();
				}
				Rectangle2D.Intersect(this, r, dest);
				return dest;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D CreateUnion(Rectangle2D r)
			{
				Rectangle2D dest;
				if (r is Float)
				{
					dest = new Rectangle2D.Float();
				}
				else
				{
					dest = new Rectangle2D.Double();
				}
				Rectangle2D.Union(this, r, dest);
				return dest;
			}

			/// <summary>
			/// Returns the <code>String</code> representation of this
			/// <code>Rectangle2D</code>. </summary>
			/// <returns> a <code>String</code> representing this
			/// <code>Rectangle2D</code>.
			/// @since 1.2 </returns>
			public override String ToString()
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return this.GetType().FullName + "[x=" + x + ",y=" + y + ",w=" + Width_Renamed + ",h=" + Height_Renamed + "]";
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 3798716824173675777L;
		}

		/// <summary>
		/// The <code>Double</code> class defines a rectangle specified in
		/// double coordinates.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Double : Rectangle2D
		{
			/// <summary>
			/// The X coordinate of this <code>Rectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double x;

			/// <summary>
			/// The Y coordinate of this <code>Rectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double y;

			/// <summary>
			/// The width of this <code>Rectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Width_Renamed;

			/// <summary>
			/// The height of this <code>Rectangle2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Height_Renamed;

			/// <summary>
			/// Constructs a new <code>Rectangle2D</code>, initialized to
			/// location (0,&nbsp;0) and size (0,&nbsp;0).
			/// @since 1.2
			/// </summary>
			public Double()
			{
			}

			/// <summary>
			/// Constructs and initializes a <code>Rectangle2D</code>
			/// from the specified <code>double</code> coordinates.
			/// </summary>
			/// <param name="x"> the X coordinate of the upper-left corner
			///          of the newly constructed <code>Rectangle2D</code> </param>
			/// <param name="y"> the Y coordinate of the upper-left corner
			///          of the newly constructed <code>Rectangle2D</code> </param>
			/// <param name="w"> the width of the newly constructed
			///          <code>Rectangle2D</code> </param>
			/// <param name="h"> the height of the newly constructed
			///          <code>Rectangle2D</code>
			/// @since 1.2 </param>
			public Double(double x, double y, double w, double h)
			{
				SetRect(x, y, w, h);
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
					return (Width_Renamed <= 0.0) || (Height_Renamed <= 0.0);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override void SetRect(double x, double y, double w, double h)
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
			public override Rectangle2D Rect
			{
				set
				{
					this.x = value.X;
					this.y = value.Y;
					this.Width_Renamed = value.Width;
					this.Height_Renamed = value.Height;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override int Outcode(double x, double y)
			{
				int @out = 0;
				if (this.Width_Renamed <= 0)
				{
					@out |= OUT_LEFT | OUT_RIGHT;
				}
				else if (x < this.x)
				{
					@out |= OUT_LEFT;
				}
				else if (x > this.x + this.Width_Renamed)
				{
					@out |= OUT_RIGHT;
				}
				if (this.Height_Renamed <= 0)
				{
					@out |= OUT_TOP | OUT_BOTTOM;
				}
				else if (y < this.y)
				{
					@out |= OUT_TOP;
				}
				else if (y > this.y + this.Height_Renamed)
				{
					@out |= OUT_BOTTOM;
				}
				return @out;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D Bounds2D
			{
				get
				{
					return new Double(x, y, Width_Renamed, Height_Renamed);
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D CreateIntersection(Rectangle2D r)
			{
				Rectangle2D dest = new Rectangle2D.Double();
				Rectangle2D.Intersect(this, r, dest);
				return dest;
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override Rectangle2D CreateUnion(Rectangle2D r)
			{
				Rectangle2D dest = new Rectangle2D.Double();
				Rectangle2D.Union(this, r, dest);
				return dest;
			}

			/// <summary>
			/// Returns the <code>String</code> representation of this
			/// <code>Rectangle2D</code>. </summary>
			/// <returns> a <code>String</code> representing this
			/// <code>Rectangle2D</code>.
			/// @since 1.2 </returns>
			public override String ToString()
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return this.GetType().FullName + "[x=" + x + ",y=" + y + ",w=" + Width_Renamed + ",h=" + Height_Renamed + "]";
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 7771313791441850493L;
		}

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessor
		/// methods below.
		/// </summary>
		/// <seealso cref= java.awt.geom.Rectangle2D.Float </seealso>
		/// <seealso cref= java.awt.geom.Rectangle2D.Double </seealso>
		/// <seealso cref= java.awt.Rectangle
		/// @since 1.2 </seealso>
		protected internal Rectangle2D()
		{
		}

		/// <summary>
		/// Sets the location and size of this <code>Rectangle2D</code>
		/// to the specified <code>double</code> values.
		/// </summary>
		/// <param name="x"> the X coordinate of the upper-left corner
		///          of this <code>Rectangle2D</code> </param>
		/// <param name="y"> the Y coordinate of the upper-left corner
		///          of this <code>Rectangle2D</code> </param>
		/// <param name="w"> the width of this <code>Rectangle2D</code> </param>
		/// <param name="h"> the height of this <code>Rectangle2D</code>
		/// @since 1.2 </param>
		public abstract void SetRect(double x, double y, double w, double h);

		/// <summary>
		/// Sets this <code>Rectangle2D</code> to be the same as the specified
		/// <code>Rectangle2D</code>. </summary>
		/// <param name="r"> the specified <code>Rectangle2D</code>
		/// @since 1.2 </param>
		public virtual Rectangle2D Rect
		{
			set
			{
				SetRect(value.X, value.Y, value.Width, value.Height);
			}
		}

		/// <summary>
		/// Tests if the specified line segment intersects the interior of this
		/// <code>Rectangle2D</code>.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point of the specified
		///           line segment </param>
		/// <param name="y1"> the Y coordinate of the start point of the specified
		///           line segment </param>
		/// <param name="x2"> the X coordinate of the end point of the specified
		///           line segment </param>
		/// <param name="y2"> the Y coordinate of the end point of the specified
		///           line segment </param>
		/// <returns> <code>true</code> if the specified line segment intersects
		/// the interior of this <code>Rectangle2D</code>; <code>false</code>
		/// otherwise.
		/// @since 1.2 </returns>
		public virtual bool IntersectsLine(double x1, double y1, double x2, double y2)
		{
			int out1, out2;
			if ((out2 = Outcode(x2, y2)) == 0)
			{
				return true;
			}
			while ((out1 = Outcode(x1, y1)) != 0)
			{
				if ((out1 & out2) != 0)
				{
					return false;
				}
				if ((out1 & (OUT_LEFT | OUT_RIGHT)) != 0)
				{
					double x = X;
					if ((out1 & OUT_RIGHT) != 0)
					{
						x += Width;
					}
					y1 = y1 + (x - x1) * (y2 - y1) / (x2 - x1);
					x1 = x;
				}
				else
				{
					double y = Y;
					if ((out1 & OUT_BOTTOM) != 0)
					{
						y += Height;
					}
					x1 = x1 + (y - y1) * (x2 - x1) / (y2 - y1);
					y1 = y;
				}
			}
			return true;
		}

		/// <summary>
		/// Tests if the specified line segment intersects the interior of this
		/// <code>Rectangle2D</code>. </summary>
		/// <param name="l"> the specified <seealso cref="Line2D"/> to test for intersection
		/// with the interior of this <code>Rectangle2D</code> </param>
		/// <returns> <code>true</code> if the specified <code>Line2D</code>
		/// intersects the interior of this <code>Rectangle2D</code>;
		/// <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public virtual bool IntersectsLine(Line2D l)
		{
			return IntersectsLine(l.X1, l.Y1, l.X2, l.Y2);
		}

		/// <summary>
		/// Determines where the specified coordinates lie with respect
		/// to this <code>Rectangle2D</code>.
		/// This method computes a binary OR of the appropriate mask values
		/// indicating, for each side of this <code>Rectangle2D</code>,
		/// whether or not the specified coordinates are on the same side
		/// of the edge as the rest of this <code>Rectangle2D</code>. </summary>
		/// <param name="x"> the specified X coordinate </param>
		/// <param name="y"> the specified Y coordinate </param>
		/// <returns> the logical OR of all appropriate out codes. </returns>
		/// <seealso cref= #OUT_LEFT </seealso>
		/// <seealso cref= #OUT_TOP </seealso>
		/// <seealso cref= #OUT_RIGHT </seealso>
		/// <seealso cref= #OUT_BOTTOM
		/// @since 1.2 </seealso>
		public abstract int Outcode(double x, double y);

		/// <summary>
		/// Determines where the specified <seealso cref="Point2D"/> lies with
		/// respect to this <code>Rectangle2D</code>.
		/// This method computes a binary OR of the appropriate mask values
		/// indicating, for each side of this <code>Rectangle2D</code>,
		/// whether or not the specified <code>Point2D</code> is on the same
		/// side of the edge as the rest of this <code>Rectangle2D</code>. </summary>
		/// <param name="p"> the specified <code>Point2D</code> </param>
		/// <returns> the logical OR of all appropriate out codes. </returns>
		/// <seealso cref= #OUT_LEFT </seealso>
		/// <seealso cref= #OUT_TOP </seealso>
		/// <seealso cref= #OUT_RIGHT </seealso>
		/// <seealso cref= #OUT_BOTTOM
		/// @since 1.2 </seealso>
		public virtual int Outcode(Point2D p)
		{
			return Outcode(p.X, p.Y);
		}

		/// <summary>
		/// Sets the location and size of the outer bounds of this
		/// <code>Rectangle2D</code> to the specified rectangular values.
		/// </summary>
		/// <param name="x"> the X coordinate of the upper-left corner
		///          of this <code>Rectangle2D</code> </param>
		/// <param name="y"> the Y coordinate of the upper-left corner
		///          of this <code>Rectangle2D</code> </param>
		/// <param name="w"> the width of this <code>Rectangle2D</code> </param>
		/// <param name="h"> the height of this <code>Rectangle2D</code>
		/// @since 1.2 </param>
		public override void SetFrame(double x, double y, double w, double h)
		{
			SetRect(x, y, w, h);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override Rectangle2D Bounds2D
		{
			get
			{
				return (Rectangle2D) Clone();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override bool Contains(double x, double y)
		{
			double x0 = X;
			double y0 = Y;
			return (x >= x0 && y >= y0 && x < x0 + Width && y < y0 + Height);
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
			double x0 = X;
			double y0 = Y;
			return (x + w > x0 && y + h > y0 && x < x0 + Width && y < y0 + Height);
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
			double x0 = X;
			double y0 = Y;
			return (x >= x0 && y >= y0 && (x + w) <= x0 + Width && (y + h) <= y0 + Height);
		}

		/// <summary>
		/// Returns a new <code>Rectangle2D</code> object representing the
		/// intersection of this <code>Rectangle2D</code> with the specified
		/// <code>Rectangle2D</code>. </summary>
		/// <param name="r"> the <code>Rectangle2D</code> to be intersected with
		/// this <code>Rectangle2D</code> </param>
		/// <returns> the largest <code>Rectangle2D</code> contained in both
		///          the specified <code>Rectangle2D</code> and in this
		///          <code>Rectangle2D</code>.
		/// @since 1.2 </returns>
		public abstract Rectangle2D CreateIntersection(Rectangle2D r);

		/// <summary>
		/// Intersects the pair of specified source <code>Rectangle2D</code>
		/// objects and puts the result into the specified destination
		/// <code>Rectangle2D</code> object.  One of the source rectangles
		/// can also be the destination to avoid creating a third Rectangle2D
		/// object, but in this case the original points of this source
		/// rectangle will be overwritten by this method. </summary>
		/// <param name="src1"> the first of a pair of <code>Rectangle2D</code>
		/// objects to be intersected with each other </param>
		/// <param name="src2"> the second of a pair of <code>Rectangle2D</code>
		/// objects to be intersected with each other </param>
		/// <param name="dest"> the <code>Rectangle2D</code> that holds the
		/// results of the intersection of <code>src1</code> and
		/// <code>src2</code>
		/// @since 1.2 </param>
		public static void Intersect(Rectangle2D src1, Rectangle2D src2, Rectangle2D dest)
		{
			double x1 = System.Math.Max(src1.MinX, src2.MinX);
			double y1 = System.Math.Max(src1.MinY, src2.MinY);
			double x2 = System.Math.Min(src1.MaxX, src2.MaxX);
			double y2 = System.Math.Min(src1.MaxY, src2.MaxY);
			dest.SetFrame(x1, y1, x2 - x1, y2 - y1);
		}

		/// <summary>
		/// Returns a new <code>Rectangle2D</code> object representing the
		/// union of this <code>Rectangle2D</code> with the specified
		/// <code>Rectangle2D</code>. </summary>
		/// <param name="r"> the <code>Rectangle2D</code> to be combined with
		/// this <code>Rectangle2D</code> </param>
		/// <returns> the smallest <code>Rectangle2D</code> containing both
		/// the specified <code>Rectangle2D</code> and this
		/// <code>Rectangle2D</code>.
		/// @since 1.2 </returns>
		public abstract Rectangle2D CreateUnion(Rectangle2D r);

		/// <summary>
		/// Unions the pair of source <code>Rectangle2D</code> objects
		/// and puts the result into the specified destination
		/// <code>Rectangle2D</code> object.  One of the source rectangles
		/// can also be the destination to avoid creating a third Rectangle2D
		/// object, but in this case the original points of this source
		/// rectangle will be overwritten by this method. </summary>
		/// <param name="src1"> the first of a pair of <code>Rectangle2D</code>
		/// objects to be combined with each other </param>
		/// <param name="src2"> the second of a pair of <code>Rectangle2D</code>
		/// objects to be combined with each other </param>
		/// <param name="dest"> the <code>Rectangle2D</code> that holds the
		/// results of the union of <code>src1</code> and
		/// <code>src2</code>
		/// @since 1.2 </param>
		public static void Union(Rectangle2D src1, Rectangle2D src2, Rectangle2D dest)
		{
			double x1 = System.Math.Min(src1.MinX, src2.MinX);
			double y1 = System.Math.Min(src1.MinY, src2.MinY);
			double x2 = System.Math.Max(src1.MaxX, src2.MaxX);
			double y2 = System.Math.Max(src1.MaxY, src2.MaxY);
			dest.SetFrameFromDiagonal(x1, y1, x2, y2);
		}

		/// <summary>
		/// Adds a point, specified by the double precision arguments
		/// <code>newx</code> and <code>newy</code>, to this
		/// <code>Rectangle2D</code>.  The resulting <code>Rectangle2D</code>
		/// is the smallest <code>Rectangle2D</code> that
		/// contains both the original <code>Rectangle2D</code> and the
		/// specified point.
		/// <para>
		/// After adding a point, a call to <code>contains</code> with the
		/// added point as an argument does not necessarily return
		/// <code>true</code>. The <code>contains</code> method does not
		/// return <code>true</code> for points on the right or bottom
		/// edges of a rectangle. Therefore, if the added point falls on
		/// the left or bottom edge of the enlarged rectangle,
		/// <code>contains</code> returns <code>false</code> for that point.
		/// </para>
		/// </summary>
		/// <param name="newx"> the X coordinate of the new point </param>
		/// <param name="newy"> the Y coordinate of the new point
		/// @since 1.2 </param>
		public virtual void Add(double newx, double newy)
		{
			double x1 = System.Math.Min(MinX, newx);
			double x2 = System.Math.Max(MaxX, newx);
			double y1 = System.Math.Min(MinY, newy);
			double y2 = System.Math.Max(MaxY, newy);
			SetRect(x1, y1, x2 - x1, y2 - y1);
		}

		/// <summary>
		/// Adds the <code>Point2D</code> object <code>pt</code> to this
		/// <code>Rectangle2D</code>.
		/// The resulting <code>Rectangle2D</code> is the smallest
		/// <code>Rectangle2D</code> that contains both the original
		/// <code>Rectangle2D</code> and the specified <code>Point2D</code>.
		/// <para>
		/// After adding a point, a call to <code>contains</code> with the
		/// added point as an argument does not necessarily return
		/// <code>true</code>. The <code>contains</code>
		/// method does not return <code>true</code> for points on the right
		/// or bottom edges of a rectangle. Therefore, if the added point falls
		/// on the left or bottom edge of the enlarged rectangle,
		/// <code>contains</code> returns <code>false</code> for that point.
		/// </para>
		/// </summary>
		/// <param name="pt"> the new <code>Point2D</code> to add to this
		/// <code>Rectangle2D</code>.
		/// @since 1.2 </param>
		public virtual void Add(Point2D pt)
		{
			Add(pt.X, pt.Y);
		}

		/// <summary>
		/// Adds a <code>Rectangle2D</code> object to this
		/// <code>Rectangle2D</code>.  The resulting <code>Rectangle2D</code>
		/// is the union of the two <code>Rectangle2D</code> objects. </summary>
		/// <param name="r"> the <code>Rectangle2D</code> to add to this
		/// <code>Rectangle2D</code>.
		/// @since 1.2 </param>
		public virtual void Add(Rectangle2D r)
		{
			double x1 = System.Math.Min(MinX, r.MinX);
			double x2 = System.Math.Max(MaxX, r.MaxX);
			double y1 = System.Math.Min(MinY, r.MinY);
			double y2 = System.Math.Max(MaxY, r.MaxY);
			SetRect(x1, y1, x2 - x1, y2 - y1);
		}

		/// <summary>
		/// Returns an iteration object that defines the boundary of this
		/// <code>Rectangle2D</code>.
		/// The iterator for this class is multi-threaded safe, which means
		/// that this <code>Rectangle2D</code> class guarantees that
		/// modifications to the geometry of this <code>Rectangle2D</code>
		/// object do not affect any iterations of that geometry that
		/// are already in process. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to
		/// the coordinates as they are returned in the iteration, or
		/// <code>null</code> if untransformed coordinates are desired </param>
		/// <returns>    the <code>PathIterator</code> object that returns the
		///          geometry of the outline of this
		///          <code>Rectangle2D</code>, one segment at a time.
		/// @since 1.2 </returns>
		public override PathIterator GetPathIterator(AffineTransform at)
		{
			return new RectIterator(this, at);
		}

		/// <summary>
		/// Returns an iteration object that defines the boundary of the
		/// flattened <code>Rectangle2D</code>.  Since rectangles are already
		/// flat, the <code>flatness</code> parameter is ignored.
		/// The iterator for this class is multi-threaded safe, which means
		/// that this <code>Rectangle2D</code> class guarantees that
		/// modifications to the geometry of this <code>Rectangle2D</code>
		/// object do not affect any iterations of that geometry that
		/// are already in process. </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to
		/// the coordinates as they are returned in the iteration, or
		/// <code>null</code> if untransformed coordinates are desired </param>
		/// <param name="flatness"> the maximum distance that the line segments used to
		/// approximate the curved segments are allowed to deviate from any
		/// point on the original curve.  Since rectangles are already flat,
		/// the <code>flatness</code> parameter is ignored. </param>
		/// <returns>    the <code>PathIterator</code> object that returns the
		///          geometry of the outline of this
		///          <code>Rectangle2D</code>, one segment at a time.
		/// @since 1.2 </returns>
		public override PathIterator GetPathIterator(AffineTransform at, double flatness)
		{
			return new RectIterator(this, at);
		}

		/// <summary>
		/// Returns the hashcode for this <code>Rectangle2D</code>. </summary>
		/// <returns> the hashcode for this <code>Rectangle2D</code>.
		/// @since 1.2 </returns>
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
		/// equal to this <code>Rectangle2D</code>.  The specified
		/// <code>Object</code> is equal to this <code>Rectangle2D</code>
		/// if it is an instance of <code>Rectangle2D</code> and if its
		/// location and size are the same as this <code>Rectangle2D</code>. </summary>
		/// <param name="obj"> an <code>Object</code> to be compared with this
		/// <code>Rectangle2D</code>. </param>
		/// <returns>     <code>true</code> if <code>obj</code> is an instance
		///                     of <code>Rectangle2D</code> and has
		///                     the same values; <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj is Rectangle2D)
			{
				Rectangle2D r2d = (Rectangle2D) obj;
				return ((X == r2d.X) && (Y == r2d.Y) && (Width == r2d.Width) && (Height == r2d.Height));
			}
			return false;
		}
	}

}