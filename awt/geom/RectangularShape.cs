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


	/// <summary>
	/// <code>RectangularShape</code> is the base class for a number of
	/// <seealso cref="Shape"/> objects whose geometry is defined by a rectangular frame.
	/// This class does not directly specify any specific geometry by
	/// itself, but merely provides manipulation methods inherited by
	/// a whole category of <code>Shape</code> objects.
	/// The manipulation methods provided by this class can be used to
	/// query and modify the rectangular frame, which provides a reference
	/// for the subclasses to define their geometry.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </summary>
	public abstract class RectangularShape : Shape, Cloneable
	{
		public abstract java.awt.geom.PathIterator GetPathIterator(AffineTransform at);
		public abstract bool Contains(double x, double y, double w, double h);
		public abstract bool Intersects(double x, double y, double w, double h);
		public abstract bool Contains(double x, double y);
		public abstract java.awt.geom.Rectangle2D Bounds2D {get;}

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// </summary>
		/// <seealso cref= Arc2D </seealso>
		/// <seealso cref= Ellipse2D </seealso>
		/// <seealso cref= Rectangle2D </seealso>
		/// <seealso cref= RoundRectangle2D
		/// @since 1.2 </seealso>
		protected internal RectangularShape()
		{
		}

		/// <summary>
		/// Returns the X coordinate of the upper-left corner of
		/// the framing rectangle in <code>double</code> precision. </summary>
		/// <returns> the X coordinate of the upper-left corner of
		/// the framing rectangle.
		/// @since 1.2 </returns>
		public abstract double X {get;}

		/// <summary>
		/// Returns the Y coordinate of the upper-left corner of
		/// the framing rectangle in <code>double</code> precision. </summary>
		/// <returns> the Y coordinate of the upper-left corner of
		/// the framing rectangle.
		/// @since 1.2 </returns>
		public abstract double Y {get;}

		/// <summary>
		/// Returns the width of the framing rectangle in
		/// <code>double</code> precision. </summary>
		/// <returns> the width of the framing rectangle.
		/// @since 1.2 </returns>
		public abstract double Width {get;}

		/// <summary>
		/// Returns the height of the framing rectangle
		/// in <code>double</code> precision. </summary>
		/// <returns> the height of the framing rectangle.
		/// @since 1.2 </returns>
		public abstract double Height {get;}

		/// <summary>
		/// Returns the smallest X coordinate of the framing
		/// rectangle of the <code>Shape</code> in <code>double</code>
		/// precision. </summary>
		/// <returns> the smallest X coordinate of the framing
		///          rectangle of the <code>Shape</code>.
		/// @since 1.2 </returns>
		public virtual double MinX
		{
			get
			{
				return X;
			}
		}

		/// <summary>
		/// Returns the smallest Y coordinate of the framing
		/// rectangle of the <code>Shape</code> in <code>double</code>
		/// precision. </summary>
		/// <returns> the smallest Y coordinate of the framing
		///          rectangle of the <code>Shape</code>.
		/// @since 1.2 </returns>
		public virtual double MinY
		{
			get
			{
				return Y;
			}
		}

		/// <summary>
		/// Returns the largest X coordinate of the framing
		/// rectangle of the <code>Shape</code> in <code>double</code>
		/// precision. </summary>
		/// <returns> the largest X coordinate of the framing
		///          rectangle of the <code>Shape</code>.
		/// @since 1.2 </returns>
		public virtual double MaxX
		{
			get
			{
				return X + Width;
			}
		}

		/// <summary>
		/// Returns the largest Y coordinate of the framing
		/// rectangle of the <code>Shape</code> in <code>double</code>
		/// precision. </summary>
		/// <returns> the largest Y coordinate of the framing
		///          rectangle of the <code>Shape</code>.
		/// @since 1.2 </returns>
		public virtual double MaxY
		{
			get
			{
				return Y + Height;
			}
		}

		/// <summary>
		/// Returns the X coordinate of the center of the framing
		/// rectangle of the <code>Shape</code> in <code>double</code>
		/// precision. </summary>
		/// <returns> the X coordinate of the center of the framing rectangle
		///          of the <code>Shape</code>.
		/// @since 1.2 </returns>
		public virtual double CenterX
		{
			get
			{
				return X + Width / 2.0;
			}
		}

		/// <summary>
		/// Returns the Y coordinate of the center of the framing
		/// rectangle of the <code>Shape</code> in <code>double</code>
		/// precision. </summary>
		/// <returns> the Y coordinate of the center of the framing rectangle
		///          of the <code>Shape</code>.
		/// @since 1.2 </returns>
		public virtual double CenterY
		{
			get
			{
				return Y + Height / 2.0;
			}
		}

		/// <summary>
		/// Returns the framing <seealso cref="Rectangle2D"/>
		/// that defines the overall shape of this object. </summary>
		/// <returns> a <code>Rectangle2D</code>, specified in
		/// <code>double</code> coordinates. </returns>
		/// <seealso cref= #setFrame(double, double, double, double) </seealso>
		/// <seealso cref= #setFrame(Point2D, Dimension2D) </seealso>
		/// <seealso cref= #setFrame(Rectangle2D)
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transient public Rectangle2D getFrame()
		public virtual Rectangle2D Frame
		{
			get
			{
				return new Rectangle2D.Double(X, Y, Width, Height);
			}
			set
			{
				SetFrame(value.X, value.Y, value.Width, value.Height);
			}
		}

		/// <summary>
		/// Determines whether the <code>RectangularShape</code> is empty.
		/// When the <code>RectangularShape</code> is empty, it encloses no
		/// area. </summary>
		/// <returns> <code>true</code> if the <code>RectangularShape</code> is empty;
		///          <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public abstract bool Empty {get;}

		/// <summary>
		/// Sets the location and size of the framing rectangle of this
		/// <code>Shape</code> to the specified rectangular values.
		/// </summary>
		/// <param name="x"> the X coordinate of the upper-left corner of the
		///          specified rectangular shape </param>
		/// <param name="y"> the Y coordinate of the upper-left corner of the
		///          specified rectangular shape </param>
		/// <param name="w"> the width of the specified rectangular shape </param>
		/// <param name="h"> the height of the specified rectangular shape </param>
		/// <seealso cref= #getFrame
		/// @since 1.2 </seealso>
		public abstract void SetFrame(double x, double y, double w, double h);

		/// <summary>
		/// Sets the location and size of the framing rectangle of this
		/// <code>Shape</code> to the specified <seealso cref="Point2D"/> and
		/// <seealso cref="Dimension2D"/>, respectively.  The framing rectangle is used
		/// by the subclasses of <code>RectangularShape</code> to define
		/// their geometry. </summary>
		/// <param name="loc"> the specified <code>Point2D</code> </param>
		/// <param name="size"> the specified <code>Dimension2D</code> </param>
		/// <seealso cref= #getFrame
		/// @since 1.2 </seealso>
		public virtual void SetFrame(Point2D loc, Dimension2D size)
		{
			SetFrame(loc.X, loc.Y, size.Width, size.Height);
		}


		/// <summary>
		/// Sets the diagonal of the framing rectangle of this <code>Shape</code>
		/// based on the two specified coordinates.  The framing rectangle is
		/// used by the subclasses of <code>RectangularShape</code> to define
		/// their geometry.
		/// </summary>
		/// <param name="x1"> the X coordinate of the start point of the specified diagonal </param>
		/// <param name="y1"> the Y coordinate of the start point of the specified diagonal </param>
		/// <param name="x2"> the X coordinate of the end point of the specified diagonal </param>
		/// <param name="y2"> the Y coordinate of the end point of the specified diagonal
		/// @since 1.2 </param>
		public virtual void SetFrameFromDiagonal(double x1, double y1, double x2, double y2)
		{
			if (x2 < x1)
			{
				double t = x1;
				x1 = x2;
				x2 = t;
			}
			if (y2 < y1)
			{
				double t = y1;
				y1 = y2;
				y2 = t;
			}
			SetFrame(x1, y1, x2 - x1, y2 - y1);
		}

		/// <summary>
		/// Sets the diagonal of the framing rectangle of this <code>Shape</code>
		/// based on two specified <code>Point2D</code> objects.  The framing
		/// rectangle is used by the subclasses of <code>RectangularShape</code>
		/// to define their geometry.
		/// </summary>
		/// <param name="p1"> the start <code>Point2D</code> of the specified diagonal </param>
		/// <param name="p2"> the end <code>Point2D</code> of the specified diagonal
		/// @since 1.2 </param>
		public virtual void SetFrameFromDiagonal(Point2D p1, Point2D p2)
		{
			SetFrameFromDiagonal(p1.X, p1.Y, p2.X, p2.Y);
		}

		/// <summary>
		/// Sets the framing rectangle of this <code>Shape</code>
		/// based on the specified center point coordinates and corner point
		/// coordinates.  The framing rectangle is used by the subclasses of
		/// <code>RectangularShape</code> to define their geometry.
		/// </summary>
		/// <param name="centerX"> the X coordinate of the specified center point </param>
		/// <param name="centerY"> the Y coordinate of the specified center point </param>
		/// <param name="cornerX"> the X coordinate of the specified corner point </param>
		/// <param name="cornerY"> the Y coordinate of the specified corner point
		/// @since 1.2 </param>
		public virtual void SetFrameFromCenter(double centerX, double centerY, double cornerX, double cornerY)
		{
			double halfW = System.Math.Abs(cornerX - centerX);
			double halfH = System.Math.Abs(cornerY - centerY);
			SetFrame(centerX - halfW, centerY - halfH, halfW * 2.0, halfH * 2.0);
		}

		/// <summary>
		/// Sets the framing rectangle of this <code>Shape</code> based on a
		/// specified center <code>Point2D</code> and corner
		/// <code>Point2D</code>.  The framing rectangle is used by the subclasses
		/// of <code>RectangularShape</code> to define their geometry. </summary>
		/// <param name="center"> the specified center <code>Point2D</code> </param>
		/// <param name="corner"> the specified corner <code>Point2D</code>
		/// @since 1.2 </param>
		public virtual void SetFrameFromCenter(Point2D center, Point2D corner)
		{
			SetFrameFromCenter(center.X, center.Y, corner.X, corner.Y);
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
		public virtual bool Intersects(Rectangle2D r)
		{
			return Intersects(r.X, r.Y, r.Width, r.Height);
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
				double width = Width;
				double height = Height;
				if (width < 0 || height < 0)
				{
					return new Rectangle();
				}
				double x = X;
				double y = Y;
				double x1 = System.Math.Floor(x);
				double y1 = System.Math.Floor(y);
				double x2 = System.Math.Ceiling(x + width);
				double y2 = System.Math.Ceiling(y + height);
				return new Rectangle((int) x1, (int) y1, (int)(x2 - x1), (int)(y2 - y1));
			}
		}

		/// <summary>
		/// Returns an iterator object that iterates along the
		/// <code>Shape</code> object's boundary and provides access to a
		/// flattened view of the outline of the <code>Shape</code>
		/// object's geometry.
		/// <para>
		/// Only SEG_MOVETO, SEG_LINETO, and SEG_CLOSE point types will
		/// be returned by the iterator.
		/// </para>
		/// <para>
		/// The amount of subdivision of the curved segments is controlled
		/// by the <code>flatness</code> parameter, which specifies the
		/// maximum distance that any point on the unflattened transformed
		/// curve can deviate from the returned flattened path segments.
		/// An optional <seealso cref="AffineTransform"/> can
		/// be specified so that the coordinates returned in the iteration are
		/// transformed accordingly.
		/// </para>
		/// </summary>
		/// <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		///          coordinates as they are returned in the iteration,
		///          or <code>null</code> if untransformed coordinates are desired. </param>
		/// <param name="flatness"> the maximum distance that the line segments used to
		///          approximate the curved segments are allowed to deviate
		///          from any point on the original curve </param>
		/// <returns> a <code>PathIterator</code> object that provides access to
		///          the <code>Shape</code> object's flattened geometry.
		/// @since 1.2 </returns>
		public virtual PathIterator GetPathIterator(AffineTransform at, double flatness)
		{
			return new FlatteningPathIterator(GetPathIterator(at), flatness);
		}

		/// <summary>
		/// Creates a new object of the same class and with the same
		/// contents as this object. </summary>
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