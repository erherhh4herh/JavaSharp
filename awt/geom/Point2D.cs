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
	/// The <code>Point2D</code> class defines a point representing a location
	/// in {@code (x,y)} coordinate space.
	/// <para>
	/// This class is only the abstract superclass for all objects that
	/// store a 2D coordinate.
	/// The actual storage representation of the coordinates is left to
	/// the subclass.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public abstract class Point2D : Cloneable
	{

		/// <summary>
		/// The <code>Float</code> class defines a point specified in float
		/// precision.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Float : Point2D
		{
			/// <summary>
			/// The X coordinate of this <code>Point2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float x;

			/// <summary>
			/// The Y coordinate of this <code>Point2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float y;

			/// <summary>
			/// Constructs and initializes a <code>Point2D</code> with
			/// coordinates (0,&nbsp;0).
			/// @since 1.2
			/// </summary>
			public Float()
			{
			}

			/// <summary>
			/// Constructs and initializes a <code>Point2D</code> with
			/// the specified coordinates.
			/// </summary>
			/// <param name="x"> the X coordinate of the newly
			///          constructed <code>Point2D</code> </param>
			/// <param name="y"> the Y coordinate of the newly
			///          constructed <code>Point2D</code>
			/// @since 1.2 </param>
			public Float(float x, float y)
			{
				this.x = x;
				this.y = y;
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
			public override void SetLocation(double x, double y)
			{
				this.x = (float) x;
				this.y = (float) y;
			}

			/// <summary>
			/// Sets the location of this <code>Point2D</code> to the
			/// specified <code>float</code> coordinates.
			/// </summary>
			/// <param name="x"> the new X coordinate of this {@code Point2D} </param>
			/// <param name="y"> the new Y coordinate of this {@code Point2D}
			/// @since 1.2 </param>
			public virtual void SetLocation(float x, float y)
			{
				this.x = x;
				this.y = y;
			}

			/// <summary>
			/// Returns a <code>String</code> that represents the value
			/// of this <code>Point2D</code>. </summary>
			/// <returns> a string representation of this <code>Point2D</code>.
			/// @since 1.2 </returns>
			public override String ToString()
			{
				return "Point2D.Float[" + x + ", " + y + "]";
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = -2870572449815403710L;
		}

		/// <summary>
		/// The <code>Double</code> class defines a point specified in
		/// <code>double</code> precision.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Double : Point2D
		{
			/// <summary>
			/// The X coordinate of this <code>Point2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double x;

			/// <summary>
			/// The Y coordinate of this <code>Point2D</code>.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double y;

			/// <summary>
			/// Constructs and initializes a <code>Point2D</code> with
			/// coordinates (0,&nbsp;0).
			/// @since 1.2
			/// </summary>
			public Double()
			{
			}

			/// <summary>
			/// Constructs and initializes a <code>Point2D</code> with the
			/// specified coordinates.
			/// </summary>
			/// <param name="x"> the X coordinate of the newly
			///          constructed <code>Point2D</code> </param>
			/// <param name="y"> the Y coordinate of the newly
			///          constructed <code>Point2D</code>
			/// @since 1.2 </param>
			public Double(double x, double y)
			{
				this.x = x;
				this.y = y;
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
			public override void SetLocation(double x, double y)
			{
				this.x = x;
				this.y = y;
			}

			/// <summary>
			/// Returns a <code>String</code> that represents the value
			/// of this <code>Point2D</code>. </summary>
			/// <returns> a string representation of this <code>Point2D</code>.
			/// @since 1.2 </returns>
			public override String ToString()
			{
				return "Point2D.Double[" + x + ", " + y + "]";
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 6150783262733311327L;
		}

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessor
		/// methods below.
		/// </summary>
		/// <seealso cref= java.awt.geom.Point2D.Float </seealso>
		/// <seealso cref= java.awt.geom.Point2D.Double </seealso>
		/// <seealso cref= java.awt.Point
		/// @since 1.2 </seealso>
		protected internal Point2D()
		{
		}

		/// <summary>
		/// Returns the X coordinate of this <code>Point2D</code> in
		/// <code>double</code> precision. </summary>
		/// <returns> the X coordinate of this <code>Point2D</code>.
		/// @since 1.2 </returns>
		public abstract double X {get;}

		/// <summary>
		/// Returns the Y coordinate of this <code>Point2D</code> in
		/// <code>double</code> precision. </summary>
		/// <returns> the Y coordinate of this <code>Point2D</code>.
		/// @since 1.2 </returns>
		public abstract double Y {get;}

		/// <summary>
		/// Sets the location of this <code>Point2D</code> to the
		/// specified <code>double</code> coordinates.
		/// </summary>
		/// <param name="x"> the new X coordinate of this {@code Point2D} </param>
		/// <param name="y"> the new Y coordinate of this {@code Point2D}
		/// @since 1.2 </param>
		public abstract void SetLocation(double x, double y);

		/// <summary>
		/// Sets the location of this <code>Point2D</code> to the same
		/// coordinates as the specified <code>Point2D</code> object. </summary>
		/// <param name="p"> the specified <code>Point2D</code> to which to set
		/// this <code>Point2D</code>
		/// @since 1.2 </param>
		public virtual Point2D Location
		{
			set
			{
				SetLocation(value.X, value.Y);
			}
		}

		/// <summary>
		/// Returns the square of the distance between two points.
		/// </summary>
		/// <param name="x1"> the X coordinate of the first specified point </param>
		/// <param name="y1"> the Y coordinate of the first specified point </param>
		/// <param name="x2"> the X coordinate of the second specified point </param>
		/// <param name="y2"> the Y coordinate of the second specified point </param>
		/// <returns> the square of the distance between the two
		/// sets of specified coordinates.
		/// @since 1.2 </returns>
		public static double DistanceSq(double x1, double y1, double x2, double y2)
		{
			x1 -= x2;
			y1 -= y2;
			return (x1 * x1 + y1 * y1);
		}

		/// <summary>
		/// Returns the distance between two points.
		/// </summary>
		/// <param name="x1"> the X coordinate of the first specified point </param>
		/// <param name="y1"> the Y coordinate of the first specified point </param>
		/// <param name="x2"> the X coordinate of the second specified point </param>
		/// <param name="y2"> the Y coordinate of the second specified point </param>
		/// <returns> the distance between the two sets of specified
		/// coordinates.
		/// @since 1.2 </returns>
		public static double Distance(double x1, double y1, double x2, double y2)
		{
			x1 -= x2;
			y1 -= y2;
			return System.Math.Sqrt(x1 * x1 + y1 * y1);
		}

		/// <summary>
		/// Returns the square of the distance from this
		/// <code>Point2D</code> to a specified point.
		/// </summary>
		/// <param name="px"> the X coordinate of the specified point to be measured
		///           against this <code>Point2D</code> </param>
		/// <param name="py"> the Y coordinate of the specified point to be measured
		///           against this <code>Point2D</code> </param>
		/// <returns> the square of the distance between this
		/// <code>Point2D</code> and the specified point.
		/// @since 1.2 </returns>
		public virtual double DistanceSq(double px, double py)
		{
			px -= X;
			py -= Y;
			return (px * px + py * py);
		}

		/// <summary>
		/// Returns the square of the distance from this
		/// <code>Point2D</code> to a specified <code>Point2D</code>.
		/// </summary>
		/// <param name="pt"> the specified point to be measured
		///           against this <code>Point2D</code> </param>
		/// <returns> the square of the distance between this
		/// <code>Point2D</code> to a specified <code>Point2D</code>.
		/// @since 1.2 </returns>
		public virtual double DistanceSq(Point2D pt)
		{
			double px = pt.X - this.X;
			double py = pt.Y - this.Y;
			return (px * px + py * py);
		}

		/// <summary>
		/// Returns the distance from this <code>Point2D</code> to
		/// a specified point.
		/// </summary>
		/// <param name="px"> the X coordinate of the specified point to be measured
		///           against this <code>Point2D</code> </param>
		/// <param name="py"> the Y coordinate of the specified point to be measured
		///           against this <code>Point2D</code> </param>
		/// <returns> the distance between this <code>Point2D</code>
		/// and a specified point.
		/// @since 1.2 </returns>
		public virtual double Distance(double px, double py)
		{
			px -= X;
			py -= Y;
			return System.Math.Sqrt(px * px + py * py);
		}

		/// <summary>
		/// Returns the distance from this <code>Point2D</code> to a
		/// specified <code>Point2D</code>.
		/// </summary>
		/// <param name="pt"> the specified point to be measured
		///           against this <code>Point2D</code> </param>
		/// <returns> the distance between this <code>Point2D</code> and
		/// the specified <code>Point2D</code>.
		/// @since 1.2 </returns>
		public virtual double Distance(Point2D pt)
		{
			double px = pt.X - this.X;
			double py = pt.Y - this.Y;
			return System.Math.Sqrt(px * px + py * py);
		}

		/// <summary>
		/// Creates a new object of the same class and with the
		/// same contents as this object. </summary>
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

		/// <summary>
		/// Returns the hashcode for this <code>Point2D</code>. </summary>
		/// <returns>      a hash code for this <code>Point2D</code>. </returns>
		public override int HashCode()
		{
			long bits = Double.DoubleToLongBits(X);
			bits ^= Double.DoubleToLongBits(Y) * 31;
			return (((int) bits) ^ ((int)(bits >> 32)));
		}

		/// <summary>
		/// Determines whether or not two points are equal. Two instances of
		/// <code>Point2D</code> are equal if the values of their
		/// <code>x</code> and <code>y</code> member fields, representing
		/// their position in the coordinate space, are the same. </summary>
		/// <param name="obj"> an object to be compared with this <code>Point2D</code> </param>
		/// <returns> <code>true</code> if the object to be compared is
		///         an instance of <code>Point2D</code> and has
		///         the same values; <code>false</code> otherwise.
		/// @since 1.2 </returns>
		public override bool Equals(Object obj)
		{
			if (obj is Point2D)
			{
				Point2D p2d = (Point2D) obj;
				return (X == p2d.X) && (Y == p2d.Y);
			}
			return base.Equals(obj);
		}
	}

}