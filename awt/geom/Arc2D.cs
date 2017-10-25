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
	/// <CODE>Arc2D</CODE> is the abstract superclass for all objects that
	/// store a 2D arc defined by a framing rectangle,
	/// start angle, angular extent (length of the arc), and a closure type
	/// (<CODE>OPEN</CODE>, <CODE>CHORD</CODE>, or <CODE>PIE</CODE>).
	/// <para>
	/// <a name="inscribes">
	/// The arc is a partial section of a full ellipse which
	/// inscribes the framing rectangle of its parent <seealso cref="RectangularShape"/>.
	/// </a>
	/// <a name="angles">
	/// The angles are specified relative to the non-square
	/// framing rectangle such that 45 degrees always falls on the line from
	/// the center of the ellipse to the upper right corner of the framing
	/// rectangle.
	/// As a result, if the framing rectangle is noticeably longer along one
	/// axis than the other, the angles to the start and end of the arc segment
	/// will be skewed farther along the longer axis of the frame.
	/// </a>
	/// </para>
	/// <para>
	/// The actual storage representation of the coordinates is left to
	/// the subclass.
	/// 
	/// @author      Jim Graham
	/// @since 1.2
	/// </para>
	/// </summary>
	public abstract class Arc2D : RectangularShape
	{

		/// <summary>
		/// The closure type for an open arc with no path segments
		/// connecting the two ends of the arc segment.
		/// @since 1.2
		/// </summary>
		public const int OPEN = 0;

		/// <summary>
		/// The closure type for an arc closed by drawing a straight
		/// line segment from the start of the arc segment to the end of the
		/// arc segment.
		/// @since 1.2
		/// </summary>
		public const int CHORD = 1;

		/// <summary>
		/// The closure type for an arc closed by drawing straight line
		/// segments from the start of the arc segment to the center
		/// of the full ellipse and from that point to the end of the arc segment.
		/// @since 1.2
		/// </summary>
		public const int PIE = 2;

		/// <summary>
		/// This class defines an arc specified in {@code float} precision.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Float : Arc2D
		{
			/// <summary>
			/// The X coordinate of the upper-left corner of the framing
			/// rectangle of the arc.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float x;

			/// <summary>
			/// The Y coordinate of the upper-left corner of the framing
			/// rectangle of the arc.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float y;

			/// <summary>
			/// The overall width of the full ellipse of which this arc is
			/// a partial section (not considering the
			/// angular extents).
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Width_Renamed;

			/// <summary>
			/// The overall height of the full ellipse of which this arc is
			/// a partial section (not considering the
			/// angular extents).
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Height_Renamed;

			/// <summary>
			/// The starting angle of the arc in degrees.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Start;

			/// <summary>
			/// The angular extent of the arc in degrees.
			/// @since 1.2
			/// @serial
			/// </summary>
			public float Extent;

			/// <summary>
			/// Constructs a new OPEN arc, initialized to location (0, 0),
			/// size (0, 0), angular extents (start = 0, extent = 0).
			/// @since 1.2
			/// </summary>
			public Float() : base(OPEN)
			{
			}

			/// <summary>
			/// Constructs a new arc, initialized to location (0, 0),
			/// size (0, 0), angular extents (start = 0, extent = 0), and
			/// the specified closure type.
			/// </summary>
			/// <param name="type"> The closure type for the arc:
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// @since 1.2 </param>
			public Float(int type) : base(type)
			{
			}

			/// <summary>
			/// Constructs a new arc, initialized to the specified location,
			/// size, angular extents, and closure type.
			/// </summary>
			/// <param name="x"> The X coordinate of the upper-left corner of
			///          the arc's framing rectangle. </param>
			/// <param name="y"> The Y coordinate of the upper-left corner of
			///          the arc's framing rectangle. </param>
			/// <param name="w"> The overall width of the full ellipse of which
			///          this arc is a partial section. </param>
			/// <param name="h"> The overall height of the full ellipse of which this
			///          arc is a partial section. </param>
			/// <param name="start"> The starting angle of the arc in degrees. </param>
			/// <param name="extent"> The angular extent of the arc in degrees. </param>
			/// <param name="type"> The closure type for the arc:
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// @since 1.2 </param>
			public Float(float x, float y, float w, float h, float start, float extent, int type) : base(type)
			{
				this.x = x;
				this.y = y;
				this.Width_Renamed = w;
				this.Height_Renamed = h;
				this.Start = start;
				this.Extent = extent;
			}

			/// <summary>
			/// Constructs a new arc, initialized to the specified location,
			/// size, angular extents, and closure type.
			/// </summary>
			/// <param name="ellipseBounds"> The framing rectangle that defines the
			/// outer boundary of the full ellipse of which this arc is a
			/// partial section. </param>
			/// <param name="start"> The starting angle of the arc in degrees. </param>
			/// <param name="extent"> The angular extent of the arc in degrees. </param>
			/// <param name="type"> The closure type for the arc:
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// @since 1.2 </param>
			public Float(Rectangle2D ellipseBounds, float start, float extent, int type) : base(type)
			{
				this.x = (float) ellipseBounds.X;
				this.y = (float) ellipseBounds.Y;
				this.Width_Renamed = (float) ellipseBounds.Width;
				this.Height_Renamed = (float) ellipseBounds.Height;
				this.Start = start;
				this.Extent = extent;
			}

			/// <summary>
			/// {@inheritDoc}
			/// Note that the arc
			/// <a href="Arc2D.html#inscribes">partially inscribes</a>
			/// the framing rectangle of this {@code RectangularShape}.
			/// 
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
			/// Note that the arc
			/// <a href="Arc2D.html#inscribes">partially inscribes</a>
			/// the framing rectangle of this {@code RectangularShape}.
			/// 
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
			/// Note that the arc
			/// <a href="Arc2D.html#inscribes">partially inscribes</a>
			/// the framing rectangle of this {@code RectangularShape}.
			/// 
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
			/// Note that the arc
			/// <a href="Arc2D.html#inscribes">partially inscribes</a>
			/// the framing rectangle of this {@code RectangularShape}.
			/// 
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
			public override double AngleStart
			{
				get
				{
					return (double) Start;
				}
				set
				{
					this.Start = (float) value;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double AngleExtent
			{
				get
				{
					return (double) Extent;
				}
				set
				{
					this.Extent = (float) value;
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
			public override void SetArc(double x, double y, double w, double h, double angSt, double angExt, int closure)
			{
				this.ArcType = closure;
				this.x = (float) x;
				this.y = (float) y;
				this.Width_Renamed = (float) w;
				this.Height_Renamed = (float) h;
				this.Start = (float) angSt;
				this.Extent = (float) angExt;
			}



			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			protected internal override Rectangle2D MakeBounds(double x, double y, double w, double h)
			{
				return new Rectangle2D.Float((float) x, (float) y, (float) w, (float) h);
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 9130893014586380278L;

			/// <summary>
			/// Writes the default serializable fields to the
			/// <code>ObjectOutputStream</code> followed by a byte
			/// indicating the arc type of this <code>Arc2D</code>
			/// instance.
			/// 
			/// @serialData
			/// <ol>
			/// <li>The default serializable fields.
			/// <li>
			/// followed by a <code>byte</code> indicating the arc type
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// </ol>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
			internal virtual void WriteObject(java.io.ObjectOutputStream s)
			{
				s.DefaultWriteObject();

				s.WriteByte(ArcType);
			}

			/// <summary>
			/// Reads the default serializable fields from the
			/// <code>ObjectInputStream</code> followed by a byte
			/// indicating the arc type of this <code>Arc2D</code>
			/// instance.
			/// 
			/// @serialData
			/// <ol>
			/// <li>The default serializable fields.
			/// <li>
			/// followed by a <code>byte</code> indicating the arc type
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// </ol>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
			internal virtual void ReadObject(java.io.ObjectInputStream s)
			{
				s.DefaultReadObject();

				try
				{
					ArcType = s.ReadByte();
				}
				catch (IllegalArgumentException iae)
				{
					throw new java.io.InvalidObjectException(iae.Message);
				}
			}
		}

		/// <summary>
		/// This class defines an arc specified in {@code double} precision.
		/// @since 1.2
		/// </summary>
		[Serializable]
		public class Double : Arc2D
		{
			/// <summary>
			/// The X coordinate of the upper-left corner of the framing
			/// rectangle of the arc.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double x;

			/// <summary>
			/// The Y coordinate of the upper-left corner of the framing
			/// rectangle of the arc.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double y;

			/// <summary>
			/// The overall width of the full ellipse of which this arc is
			/// a partial section (not considering the angular extents).
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Width_Renamed;

			/// <summary>
			/// The overall height of the full ellipse of which this arc is
			/// a partial section (not considering the angular extents).
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Height_Renamed;

			/// <summary>
			/// The starting angle of the arc in degrees.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Start;

			/// <summary>
			/// The angular extent of the arc in degrees.
			/// @since 1.2
			/// @serial
			/// </summary>
			public double Extent;

			/// <summary>
			/// Constructs a new OPEN arc, initialized to location (0, 0),
			/// size (0, 0), angular extents (start = 0, extent = 0).
			/// @since 1.2
			/// </summary>
			public Double() : base(OPEN)
			{
			}

			/// <summary>
			/// Constructs a new arc, initialized to location (0, 0),
			/// size (0, 0), angular extents (start = 0, extent = 0), and
			/// the specified closure type.
			/// </summary>
			/// <param name="type"> The closure type for the arc:
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// @since 1.2 </param>
			public Double(int type) : base(type)
			{
			}

			/// <summary>
			/// Constructs a new arc, initialized to the specified location,
			/// size, angular extents, and closure type.
			/// </summary>
			/// <param name="x"> The X coordinate of the upper-left corner
			///          of the arc's framing rectangle. </param>
			/// <param name="y"> The Y coordinate of the upper-left corner
			///          of the arc's framing rectangle. </param>
			/// <param name="w"> The overall width of the full ellipse of which this
			///          arc is a partial section. </param>
			/// <param name="h"> The overall height of the full ellipse of which this
			///          arc is a partial section. </param>
			/// <param name="start"> The starting angle of the arc in degrees. </param>
			/// <param name="extent"> The angular extent of the arc in degrees. </param>
			/// <param name="type"> The closure type for the arc:
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// @since 1.2 </param>
			public Double(double x, double y, double w, double h, double start, double extent, int type) : base(type)
			{
				this.x = x;
				this.y = y;
				this.Width_Renamed = w;
				this.Height_Renamed = h;
				this.Start = start;
				this.Extent = extent;
			}

			/// <summary>
			/// Constructs a new arc, initialized to the specified location,
			/// size, angular extents, and closure type.
			/// </summary>
			/// <param name="ellipseBounds"> The framing rectangle that defines the
			/// outer boundary of the full ellipse of which this arc is a
			/// partial section. </param>
			/// <param name="start"> The starting angle of the arc in degrees. </param>
			/// <param name="extent"> The angular extent of the arc in degrees. </param>
			/// <param name="type"> The closure type for the arc:
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// @since 1.2 </param>
			public Double(Rectangle2D ellipseBounds, double start, double extent, int type) : base(type)
			{
				this.x = ellipseBounds.X;
				this.y = ellipseBounds.Y;
				this.Width_Renamed = ellipseBounds.Width;
				this.Height_Renamed = ellipseBounds.Height;
				this.Start = start;
				this.Extent = extent;
			}

			/// <summary>
			/// {@inheritDoc}
			/// Note that the arc
			/// <a href="Arc2D.html#inscribes">partially inscribes</a>
			/// the framing rectangle of this {@code RectangularShape}.
			/// 
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
			/// Note that the arc
			/// <a href="Arc2D.html#inscribes">partially inscribes</a>
			/// the framing rectangle of this {@code RectangularShape}.
			/// 
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
			/// Note that the arc
			/// <a href="Arc2D.html#inscribes">partially inscribes</a>
			/// the framing rectangle of this {@code RectangularShape}.
			/// 
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
			/// Note that the arc
			/// <a href="Arc2D.html#inscribes">partially inscribes</a>
			/// the framing rectangle of this {@code RectangularShape}.
			/// 
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
			public override double AngleStart
			{
				get
				{
					return Start;
				}
				set
				{
					this.Start = value;
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			public override double AngleExtent
			{
				get
				{
					return Extent;
				}
				set
				{
					this.Extent = value;
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
			public override void SetArc(double x, double y, double w, double h, double angSt, double angExt, int closure)
			{
				this.ArcType = closure;
				this.x = x;
				this.y = y;
				this.Width_Renamed = w;
				this.Height_Renamed = h;
				this.Start = angSt;
				this.Extent = angExt;
			}



			/// <summary>
			/// {@inheritDoc}
			/// @since 1.2
			/// </summary>
			protected internal override Rectangle2D MakeBounds(double x, double y, double w, double h)
			{
				return new Rectangle2D.Double(x, y, w, h);
			}

			/*
			 * JDK 1.6 serialVersionUID
			 */
			internal const long SerialVersionUID = 728264085846882001L;

			/// <summary>
			/// Writes the default serializable fields to the
			/// <code>ObjectOutputStream</code> followed by a byte
			/// indicating the arc type of this <code>Arc2D</code>
			/// instance.
			/// 
			/// @serialData
			/// <ol>
			/// <li>The default serializable fields.
			/// <li>
			/// followed by a <code>byte</code> indicating the arc type
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// </ol>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
			internal virtual void WriteObject(java.io.ObjectOutputStream s)
			{
				s.DefaultWriteObject();

				s.WriteByte(ArcType);
			}

			/// <summary>
			/// Reads the default serializable fields from the
			/// <code>ObjectInputStream</code> followed by a byte
			/// indicating the arc type of this <code>Arc2D</code>
			/// instance.
			/// 
			/// @serialData
			/// <ol>
			/// <li>The default serializable fields.
			/// <li>
			/// followed by a <code>byte</code> indicating the arc type
			/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			/// </ol>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.lang.ClassNotFoundException, java.io.IOException
			internal virtual void ReadObject(java.io.ObjectInputStream s)
			{
				s.DefaultReadObject();

				try
				{
					ArcType = s.ReadByte();
				}
				catch (IllegalArgumentException iae)
				{
					throw new java.io.InvalidObjectException(iae.Message);
				}
			}
		}

		private int Type;

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessor
		/// methods below.
		/// <para>
		/// This constructor creates an object with a default closure
		/// type of <seealso cref="#OPEN"/>.  It is provided only to enable
		/// serialization of subclasses.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.awt.geom.Arc2D.Float </seealso>
		/// <seealso cref= java.awt.geom.Arc2D.Double </seealso>
		protected internal Arc2D() : this(OPEN)
		{
		}

		/// <summary>
		/// This is an abstract class that cannot be instantiated directly.
		/// Type-specific implementation subclasses are available for
		/// instantiation and provide a number of formats for storing
		/// the information necessary to satisfy the various accessor
		/// methods below.
		/// </summary>
		/// <param name="type"> The closure type of this arc:
		/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>. </param>
		/// <seealso cref= java.awt.geom.Arc2D.Float </seealso>
		/// <seealso cref= java.awt.geom.Arc2D.Double
		/// @since 1.2 </seealso>
		protected internal Arc2D(int type)
		{
			ArcType = type;
		}

		/// <summary>
		/// Returns the starting angle of the arc.
		/// </summary>
		/// <returns> A double value that represents the starting angle
		/// of the arc in degrees. </returns>
		/// <seealso cref= #setAngleStart
		/// @since 1.2 </seealso>
		public abstract double GetAngleStart();

		/// <summary>
		/// Returns the angular extent of the arc.
		/// </summary>
		/// <returns> A double value that represents the angular extent
		/// of the arc in degrees. </returns>
		/// <seealso cref= #setAngleExtent
		/// @since 1.2 </seealso>
		public abstract double AngleExtent {get;set;}

		/// <summary>
		/// Returns the arc closure type of the arc: <seealso cref="#OPEN"/>,
		/// <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>. </summary>
		/// <returns> One of the integer constant closure types defined
		/// in this class. </returns>
		/// <seealso cref= #setArcType
		/// @since 1.2 </seealso>
		public virtual int ArcType
		{
			get
			{
				return Type;
			}
			set
			{
				if (value < OPEN || value > PIE)
				{
					throw new IllegalArgumentException("invalid type for Arc: " + value);
				}
				this.Type = value;
			}
		}

		/// <summary>
		/// Returns the starting point of the arc.  This point is the
		/// intersection of the ray from the center defined by the
		/// starting angle and the elliptical boundary of the arc.
		/// </summary>
		/// <returns> A <CODE>Point2D</CODE> object representing the
		/// x,y coordinates of the starting point of the arc.
		/// @since 1.2 </returns>
		public virtual Point2D StartPoint
		{
			get
			{
				double angle = Math.ToRadians(-GetAngleStart());
				double x = X + (System.Math.Cos(angle) * 0.5 + 0.5) * Width;
				double y = Y + (System.Math.Sin(angle) * 0.5 + 0.5) * Height;
				return new Point2D.Double(x, y);
			}
		}

		/// <summary>
		/// Returns the ending point of the arc.  This point is the
		/// intersection of the ray from the center defined by the
		/// starting angle plus the angular extent of the arc and the
		/// elliptical boundary of the arc.
		/// </summary>
		/// <returns> A <CODE>Point2D</CODE> object representing the
		/// x,y coordinates  of the ending point of the arc.
		/// @since 1.2 </returns>
		public virtual Point2D EndPoint
		{
			get
			{
				double angle = Math.ToRadians(-GetAngleStart() - AngleExtent);
				double x = X + (System.Math.Cos(angle) * 0.5 + 0.5) * Width;
				double y = Y + (System.Math.Sin(angle) * 0.5 + 0.5) * Height;
				return new Point2D.Double(x, y);
			}
		}

		/// <summary>
		/// Sets the location, size, angular extents, and closure type of
		/// this arc to the specified double values.
		/// </summary>
		/// <param name="x"> The X coordinate of the upper-left corner of the arc. </param>
		/// <param name="y"> The Y coordinate of the upper-left corner of the arc. </param>
		/// <param name="w"> The overall width of the full ellipse of which
		///          this arc is a partial section. </param>
		/// <param name="h"> The overall height of the full ellipse of which
		///          this arc is a partial section. </param>
		/// <param name="angSt"> The starting angle of the arc in degrees. </param>
		/// <param name="angExt"> The angular extent of the arc in degrees. </param>
		/// <param name="closure"> The closure type for the arc:
		/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
		/// @since 1.2 </param>
		public abstract void SetArc(double x, double y, double w, double h, double angSt, double angExt, int closure);

		/// <summary>
		/// Sets the location, size, angular extents, and closure type of
		/// this arc to the specified values.
		/// </summary>
		/// <param name="loc"> The <CODE>Point2D</CODE> representing the coordinates of
		/// the upper-left corner of the arc. </param>
		/// <param name="size"> The <CODE>Dimension2D</CODE> representing the width
		/// and height of the full ellipse of which this arc is
		/// a partial section. </param>
		/// <param name="angSt"> The starting angle of the arc in degrees. </param>
		/// <param name="angExt"> The angular extent of the arc in degrees. </param>
		/// <param name="closure"> The closure type for the arc:
		/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
		/// @since 1.2 </param>
		public virtual void SetArc(Point2D loc, Dimension2D size, double angSt, double angExt, int closure)
		{
			SetArc(loc.X, loc.Y, size.Width, size.Height, angSt, angExt, closure);
		}

		/// <summary>
		/// Sets the location, size, angular extents, and closure type of
		/// this arc to the specified values.
		/// </summary>
		/// <param name="rect"> The framing rectangle that defines the
		/// outer boundary of the full ellipse of which this arc is a
		/// partial section. </param>
		/// <param name="angSt"> The starting angle of the arc in degrees. </param>
		/// <param name="angExt"> The angular extent of the arc in degrees. </param>
		/// <param name="closure"> The closure type for the arc:
		/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
		/// @since 1.2 </param>
		public virtual void SetArc(Rectangle2D rect, double angSt, double angExt, int closure)
		{
			SetArc(rect.X, rect.Y, rect.Width, rect.Height, angSt, angExt, closure);
		}

		/// <summary>
		/// Sets this arc to be the same as the specified arc.
		/// </summary>
		/// <param name="a"> The <CODE>Arc2D</CODE> to use to set the arc's values.
		/// @since 1.2 </param>
		public virtual Arc2D Arc
		{
			set
			{
				SetArc(value.X, value.Y, value.Width, value.Height, value.GetAngleStart(), value.AngleExtent, value.Type);
			}
		}

		/// <summary>
		/// Sets the position, bounds, angular extents, and closure type of
		/// this arc to the specified values. The arc is defined by a center
		/// point and a radius rather than a framing rectangle for the full ellipse.
		/// </summary>
		/// <param name="x"> The X coordinate of the center of the arc. </param>
		/// <param name="y"> The Y coordinate of the center of the arc. </param>
		/// <param name="radius"> The radius of the arc. </param>
		/// <param name="angSt"> The starting angle of the arc in degrees. </param>
		/// <param name="angExt"> The angular extent of the arc in degrees. </param>
		/// <param name="closure"> The closure type for the arc:
		/// <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
		/// @since 1.2 </param>
		public virtual void SetArcByCenter(double x, double y, double radius, double angSt, double angExt, int closure)
		{
			SetArc(x - radius, y - radius, radius * 2.0, radius * 2.0, angSt, angExt, closure);
		}

		/// <summary>
		/// Sets the position, bounds, and angular extents of this arc to the
		/// specified value. The starting angle of the arc is tangent to the
		/// line specified by points (p1, p2), the ending angle is tangent to
		/// the line specified by points (p2, p3), and the arc has the
		/// specified radius.
		/// </summary>
		/// <param name="p1"> The first point that defines the arc. The starting
		/// angle of the arc is tangent to the line specified by points (p1, p2). </param>
		/// <param name="p2"> The second point that defines the arc. The starting
		/// angle of the arc is tangent to the line specified by points (p1, p2).
		/// The ending angle of the arc is tangent to the line specified by
		/// points (p2, p3). </param>
		/// <param name="p3"> The third point that defines the arc. The ending angle
		/// of the arc is tangent to the line specified by points (p2, p3). </param>
		/// <param name="radius"> The radius of the arc.
		/// @since 1.2 </param>
		public virtual void SetArcByTangent(Point2D p1, Point2D p2, Point2D p3, double radius)
		{
			double ang1 = System.Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
			double ang2 = System.Math.Atan2(p3.Y - p2.Y, p3.X - p2.X);
			double diff = ang2 - ang1;
			if (diff > Math.PI)
			{
				ang2 -= Math.PI * 2.0;
			}
			else if (diff < -Math.PI)
			{
				ang2 += Math.PI * 2.0;
			}
			double bisect = (ang1 + ang2) / 2.0;
			double theta = System.Math.Abs(ang2 - bisect);
			double dist = radius / System.Math.Sin(theta);
			double x = p2.X + dist * System.Math.Cos(bisect);
			double y = p2.Y + dist * System.Math.Sin(bisect);
			// REMIND: This needs some work...
			if (ang1 < ang2)
			{
				ang1 -= Math.PI / 2.0;
				ang2 += Math.PI / 2.0;
			}
			else
			{
				ang1 += Math.PI / 2.0;
				ang2 -= Math.PI / 2.0;
			}
			ang1 = Math.ToDegrees(-ang1);
			ang2 = Math.ToDegrees(-ang2);
			diff = ang2 - ang1;
			if (diff < 0)
			{
				diff += 360;
			}
			else
			{
				diff -= 360;
			}
			SetArcByCenter(x, y, radius, ang1, diff, Type);
		}

		/// <summary>
		/// Sets the starting angle of this arc to the specified double
		/// value.
		/// </summary>
		/// <param name="angSt"> The starting angle of the arc in degrees. </param>
		/// <seealso cref= #getAngleStart
		/// @since 1.2 </seealso>
		public abstract void SetAngleStart(double angSt);


		/// <summary>
		/// Sets the starting angle of this arc to the angle that the
		/// specified point defines relative to the center of this arc.
		/// The angular extent of the arc will remain the same.
		/// </summary>
		/// <param name="p"> The <CODE>Point2D</CODE> that defines the starting angle. </param>
		/// <seealso cref= #getAngleStart
		/// @since 1.2 </seealso>
		public virtual void SetAngleStart(Point2D p)
		{
			// Bias the dx and dy by the height and width of the oval.
			double dx = Height * (p.X - CenterX);
			double dy = Width * (p.Y - CenterY);
			SetAngleStart(-Math.ToDegrees(System.Math.Atan2(dy, dx)));
		}

		/// <summary>
		/// Sets the starting angle and angular extent of this arc using two
		/// sets of coordinates. The first set of coordinates is used to
		/// determine the angle of the starting point relative to the arc's
		/// center. The second set of coordinates is used to determine the
		/// angle of the end point relative to the arc's center.
		/// The arc will always be non-empty and extend counterclockwise
		/// from the first point around to the second point.
		/// </summary>
		/// <param name="x1"> The X coordinate of the arc's starting point. </param>
		/// <param name="y1"> The Y coordinate of the arc's starting point. </param>
		/// <param name="x2"> The X coordinate of the arc's ending point. </param>
		/// <param name="y2"> The Y coordinate of the arc's ending point.
		/// @since 1.2 </param>
		public virtual void SetAngles(double x1, double y1, double x2, double y2)
		{
			double x = CenterX;
			double y = CenterY;
			double w = Width;
			double h = Height;
			// Note: reversing the Y equations negates the angle to adjust
			// for the upside down coordinate system.
			// Also we should bias atans by the height and width of the oval.
			double ang1 = System.Math.Atan2(w * (y - y1), h * (x1 - x));
			double ang2 = System.Math.Atan2(w * (y - y2), h * (x2 - x));
			ang2 -= ang1;
			if (ang2 <= 0.0)
			{
				ang2 += Math.PI * 2.0;
			}
			SetAngleStart(Math.ToDegrees(ang1));
			AngleExtent = Math.ToDegrees(ang2);
		}

		/// <summary>
		/// Sets the starting angle and angular extent of this arc using
		/// two points. The first point is used to determine the angle of
		/// the starting point relative to the arc's center.
		/// The second point is used to determine the angle of the end point
		/// relative to the arc's center.
		/// The arc will always be non-empty and extend counterclockwise
		/// from the first point around to the second point.
		/// </summary>
		/// <param name="p1"> The <CODE>Point2D</CODE> that defines the arc's
		/// starting point. </param>
		/// <param name="p2"> The <CODE>Point2D</CODE> that defines the arc's
		/// ending point.
		/// @since 1.2 </param>
		public virtual void SetAngles(Point2D p1, Point2D p2)
		{
			SetAngles(p1.X, p1.Y, p2.X, p2.Y);
		}


		/// <summary>
		/// {@inheritDoc}
		/// Note that the arc
		/// <a href="Arc2D.html#inscribes">partially inscribes</a>
		/// the framing rectangle of this {@code RectangularShape}.
		/// 
		/// @since 1.2
		/// </summary>
		public override void SetFrame(double x, double y, double w, double h)
		{
			SetArc(x, y, w, h, GetAngleStart(), AngleExtent, Type);
		}

		/// <summary>
		/// Returns the high-precision framing rectangle of the arc.  The framing
		/// rectangle contains only the part of this <code>Arc2D</code> that is
		/// in between the starting and ending angles and contains the pie
		/// wedge, if this <code>Arc2D</code> has a <code>PIE</code> closure type.
		/// <para>
		/// This method differs from the
		/// <seealso cref="RectangularShape#getBounds() getBounds"/> in that the
		/// <code>getBounds</code> method only returns the bounds of the
		/// enclosing ellipse of this <code>Arc2D</code> without considering
		/// the starting and ending angles of this <code>Arc2D</code>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <CODE>Rectangle2D</CODE> that represents the arc's
		/// framing rectangle.
		/// @since 1.2 </returns>
		public override Rectangle2D Bounds2D
		{
			get
			{
				if (Empty)
				{
					return MakeBounds(X, Y, Width, Height);
				}
				double x1, y1, x2, y2;
				if (ArcType == PIE)
				{
					x1 = y1 = x2 = y2 = 0.0;
				}
				else
				{
					x1 = y1 = 1.0;
					x2 = y2 = -1.0;
				}
				double angle = 0.0;
				for (int i = 0; i < 6; i++)
				{
					if (i < 4)
					{
						// 0-3 are the four quadrants
						angle += 90.0;
						if (!ContainsAngle(angle))
						{
							continue;
						}
					}
					else if (i == 4)
					{
						// 4 is start angle
						angle = GetAngleStart();
					}
					else
					{
						// 5 is end angle
						angle += AngleExtent;
					}
					double rads = Math.ToRadians(-angle);
					double xe = System.Math.Cos(rads);
					double ye = System.Math.Sin(rads);
					x1 = System.Math.Min(x1, xe);
					y1 = System.Math.Min(y1, ye);
					x2 = System.Math.Max(x2, xe);
					y2 = System.Math.Max(y2, ye);
				}
				double w = Width;
				double h = Height;
				x2 = (x2 - x1) * 0.5 * w;
				y2 = (y2 - y1) * 0.5 * h;
				x1 = X + (x1 * 0.5 + 0.5) * w;
				y1 = Y + (y1 * 0.5 + 0.5) * h;
				return MakeBounds(x1, y1, x2, y2);
			}
		}

		/// <summary>
		/// Constructs a <code>Rectangle2D</code> of the appropriate precision
		/// to hold the parameters calculated to be the framing rectangle
		/// of this arc.
		/// </summary>
		/// <param name="x"> The X coordinate of the upper-left corner of the
		/// framing rectangle. </param>
		/// <param name="y"> The Y coordinate of the upper-left corner of the
		/// framing rectangle. </param>
		/// <param name="w"> The width of the framing rectangle. </param>
		/// <param name="h"> The height of the framing rectangle. </param>
		/// <returns> a <code>Rectangle2D</code> that is the framing rectangle
		///     of this arc.
		/// @since 1.2 </returns>
		protected internal abstract Rectangle2D MakeBounds(double x, double y, double w, double h);

		/*
		 * Normalizes the specified angle into the range -180 to 180.
		 */
		internal static double NormalizeDegrees(double angle)
		{
			if (angle > 180.0)
			{
				if (angle <= (180.0 + 360.0))
				{
					angle = angle - 360.0;
				}
				else
				{
					angle = System.Math.IEEERemainder(angle, 360.0);
					// IEEEremainder can return -180 here for some input values...
					if (angle == -180.0)
					{
						angle = 180.0;
					}
				}
			}
			else if (angle <= -180.0)
			{
				if (angle > (-180.0 - 360.0))
				{
					angle = angle + 360.0;
				}
				else
				{
					angle = System.Math.IEEERemainder(angle, 360.0);
					// IEEEremainder can return -180 here for some input values...
					if (angle == -180.0)
					{
						angle = 180.0;
					}
				}
			}
			return angle;
		}

		/// <summary>
		/// Determines whether or not the specified angle is within the
		/// angular extents of the arc.
		/// </summary>
		/// <param name="angle"> The angle to test.
		/// </param>
		/// <returns> <CODE>true</CODE> if the arc contains the angle,
		/// <CODE>false</CODE> if the arc doesn't contain the angle.
		/// @since 1.2 </returns>
		public virtual bool ContainsAngle(double angle)
		{
			double angExt = AngleExtent;
			bool backwards = (angExt < 0.0);
			if (backwards)
			{
				angExt = -angExt;
			}
			if (angExt >= 360.0)
			{
				return true;
			}
			angle = NormalizeDegrees(angle) - NormalizeDegrees(GetAngleStart());
			if (backwards)
			{
				angle = -angle;
			}
			if (angle < 0.0)
			{
				angle += 360.0;
			}


			return (angle >= 0.0) && (angle < angExt);
		}

		/// <summary>
		/// Determines whether or not the specified point is inside the boundary
		/// of the arc.
		/// </summary>
		/// <param name="x"> The X coordinate of the point to test. </param>
		/// <param name="y"> The Y coordinate of the point to test.
		/// </param>
		/// <returns> <CODE>true</CODE> if the point lies within the bound of
		/// the arc, <CODE>false</CODE> if the point lies outside of the
		/// arc's bounds.
		/// @since 1.2 </returns>
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
			double distSq = (normx * normx + normy * normy);
			if (distSq >= 0.25)
			{
				return false;
			}
			double angExt = System.Math.Abs(AngleExtent);
			if (angExt >= 360.0)
			{
				return true;
			}
			bool inarc = ContainsAngle(-Math.ToDegrees(System.Math.Atan2(normy, normx)));
			if (Type == PIE)
			{
				return inarc;
			}
			// CHORD and OPEN behave the same way
			if (inarc)
			{
				if (angExt >= 180.0)
				{
					return true;
				}
				// point must be outside the "pie triangle"
			}
			else
			{
				if (angExt <= 180.0)
				{
					return false;
				}
				// point must be inside the "pie triangle"
			}
			// The point is inside the pie triangle iff it is on the same
			// side of the line connecting the ends of the arc as the center.
			double angle = Math.ToRadians(-GetAngleStart());
			double x1 = System.Math.Cos(angle);
			double y1 = System.Math.Sin(angle);
			angle += Math.ToRadians(-AngleExtent);
			double x2 = System.Math.Cos(angle);
			double y2 = System.Math.Sin(angle);
			bool inside = (Line2D.RelativeCCW(x1, y1, x2, y2, 2 * normx, 2 * normy) * Line2D.RelativeCCW(x1, y1, x2, y2, 0, 0) >= 0);
			return inarc ?!inside : inside;
		}

		/// <summary>
		/// Determines whether or not the interior of the arc intersects
		/// the interior of the specified rectangle.
		/// </summary>
		/// <param name="x"> The X coordinate of the rectangle's upper-left corner. </param>
		/// <param name="y"> The Y coordinate of the rectangle's upper-left corner. </param>
		/// <param name="w"> The width of the rectangle. </param>
		/// <param name="h"> The height of the rectangle.
		/// </param>
		/// <returns> <CODE>true</CODE> if the arc intersects the rectangle,
		/// <CODE>false</CODE> if the arc doesn't intersect the rectangle.
		/// @since 1.2 </returns>
		public override bool Intersects(double x, double y, double w, double h)
		{

			double aw = Width;
			double ah = Height;

			if (w <= 0 || h <= 0 || aw <= 0 || ah <= 0)
			{
				return false;
			}
			double ext = AngleExtent;
			if (ext == 0)
			{
				return false;
			}

			double ax = X;
			double ay = Y;
			double axw = ax + aw;
			double ayh = ay + ah;
			double xw = x + w;
			double yh = y + h;

			// check bbox
			if (x >= axw || y >= ayh || xw <= ax || yh <= ay)
			{
				return false;
			}

			// extract necessary data
			double axc = CenterX;
			double ayc = CenterY;
			Point2D sp = StartPoint;
			Point2D ep = EndPoint;
			double sx = sp.X;
			double sy = sp.Y;
			double ex = ep.X;
			double ey = ep.Y;

			/*
			 * Try to catch rectangles that intersect arc in areas
			 * outside of rectagle with left top corner coordinates
			 * (min(center x, start point x, end point x),
			 *  min(center y, start point y, end point y))
			 * and rigth bottom corner coordinates
			 * (max(center x, start point x, end point x),
			 *  max(center y, start point y, end point y)).
			 * So we'll check axis segments outside of rectangle above.
			 */
			if (ayc >= y && ayc <= yh) // 0 and 180
			{
				if ((sx < xw && ex < xw && axc < xw && axw > x && ContainsAngle(0)) || (sx > x && ex > x && axc > x && ax < xw && ContainsAngle(180)))
				{
					return true;
				}
			}
			if (axc >= x && axc <= xw) // 90 and 270
			{
				if ((sy > y && ey > y && ayc > y && ay < yh && ContainsAngle(90)) || (sy < yh && ey < yh && ayc < yh && ayh > y && ContainsAngle(270)))
				{
					return true;
				}
			}

			/*
			 * For PIE we should check intersection with pie slices;
			 * also we should do the same for arcs with extent is greater
			 * than 180, because we should cover case of rectangle, which
			 * situated between center of arc and chord, but does not
			 * intersect the chord.
			 */
			Rectangle2D rect = new Rectangle2D.Double(x, y, w, h);
			if (Type == PIE || System.Math.Abs(ext) > 180)
			{
				// for PIE: try to find intersections with pie slices
				if (rect.IntersectsLine(axc, ayc, sx, sy) || rect.IntersectsLine(axc, ayc, ex, ey))
				{
					return true;
				}
			}
			else
			{
				// for CHORD and OPEN: try to find intersections with chord
				if (rect.IntersectsLine(sx, sy, ex, ey))
				{
					return true;
				}
			}

			// finally check the rectangle corners inside the arc
			if (Contains(x, y) || Contains(x + w, y) || Contains(x, y + h) || Contains(x + w, y + h))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether or not the interior of the arc entirely contains
		/// the specified rectangle.
		/// </summary>
		/// <param name="x"> The X coordinate of the rectangle's upper-left corner. </param>
		/// <param name="y"> The Y coordinate of the rectangle's upper-left corner. </param>
		/// <param name="w"> The width of the rectangle. </param>
		/// <param name="h"> The height of the rectangle.
		/// </param>
		/// <returns> <CODE>true</CODE> if the arc contains the rectangle,
		/// <CODE>false</CODE> if the arc doesn't contain the rectangle.
		/// @since 1.2 </returns>
		public override bool Contains(double x, double y, double w, double h)
		{
			return Contains(x, y, w, h, null);
		}

		/// <summary>
		/// Determines whether or not the interior of the arc entirely contains
		/// the specified rectangle.
		/// </summary>
		/// <param name="r"> The <CODE>Rectangle2D</CODE> to test.
		/// </param>
		/// <returns> <CODE>true</CODE> if the arc contains the rectangle,
		/// <CODE>false</CODE> if the arc doesn't contain the rectangle.
		/// @since 1.2 </returns>
		public override bool Contains(Rectangle2D r)
		{
			return Contains(r.X, r.Y, r.Width, r.Height, r);
		}

		private bool Contains(double x, double y, double w, double h, Rectangle2D origrect)
		{
			if (!(Contains(x, y) && Contains(x + w, y) && Contains(x, y + h) && Contains(x + w, y + h)))
			{
				return false;
			}
			// If the shape is convex then we have done all the testing
			// we need.  Only PIE arcs can be concave and then only if
			// the angular extents are greater than 180 degrees.
			if (Type != PIE || System.Math.Abs(AngleExtent) <= 180.0)
			{
				return true;
			}
			// For a PIE shape we have an additional test for the case where
			// the angular extents are greater than 180 degrees and all four
			// rectangular corners are inside the shape but one of the
			// rectangle edges spans across the "missing wedge" of the arc.
			// We can test for this case by checking if the rectangle intersects
			// either of the pie angle segments.
			if (origrect == null)
			{
				origrect = new Rectangle2D.Double(x, y, w, h);
			}
			double halfW = Width / 2.0;
			double halfH = Height / 2.0;
			double xc = X + halfW;
			double yc = Y + halfH;
			double angle = Math.ToRadians(-GetAngleStart());
			double xe = xc + halfW * System.Math.Cos(angle);
			double ye = yc + halfH * System.Math.Sin(angle);
			if (origrect.IntersectsLine(xc, yc, xe, ye))
			{
				return false;
			}
			angle += Math.ToRadians(-AngleExtent);
			xe = xc + halfW * System.Math.Cos(angle);
			ye = yc + halfH * System.Math.Sin(angle);
			return !origrect.IntersectsLine(xc, yc, xe, ye);
		}

		/// <summary>
		/// Returns an iteration object that defines the boundary of the
		/// arc.
		/// This iterator is multithread safe.
		/// <code>Arc2D</code> guarantees that
		/// modifications to the geometry of the arc
		/// do not affect any iterations of that geometry that
		/// are already in process.
		/// </summary>
		/// <param name="at"> an optional <CODE>AffineTransform</CODE> to be applied
		/// to the coordinates as they are returned in the iteration, or null
		/// if the untransformed coordinates are desired.
		/// </param>
		/// <returns> A <CODE>PathIterator</CODE> that defines the arc's boundary.
		/// @since 1.2 </returns>
		public override PathIterator GetPathIterator(AffineTransform at)
		{
			return new ArcIterator(this, at);
		}

		/// <summary>
		/// Returns the hashcode for this <code>Arc2D</code>. </summary>
		/// <returns> the hashcode for this <code>Arc2D</code>.
		/// @since 1.6 </returns>
		public override int HashCode()
		{
			long bits = Double.DoubleToLongBits(X);
			bits += Double.DoubleToLongBits(Y) * 37;
			bits += Double.DoubleToLongBits(Width) * 43;
			bits += Double.DoubleToLongBits(Height) * 47;
			bits += Double.DoubleToLongBits(GetAngleStart()) * 53;
			bits += Double.DoubleToLongBits(AngleExtent) * 59;
			bits += ArcType * 61;
			return (((int) bits) ^ ((int)(bits >> 32)));
		}

		/// <summary>
		/// Determines whether or not the specified <code>Object</code> is
		/// equal to this <code>Arc2D</code>.  The specified
		/// <code>Object</code> is equal to this <code>Arc2D</code>
		/// if it is an instance of <code>Arc2D</code> and if its
		/// location, size, arc extents and type are the same as this
		/// <code>Arc2D</code>. </summary>
		/// <param name="obj">  an <code>Object</code> to be compared with this
		///             <code>Arc2D</code>. </param>
		/// <returns>  <code>true</code> if <code>obj</code> is an instance
		///          of <code>Arc2D</code> and has the same values;
		///          <code>false</code> otherwise.
		/// @since 1.6 </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj is Arc2D)
			{
				Arc2D a2d = (Arc2D) obj;
				return ((X == a2d.X) && (Y == a2d.Y) && (Width == a2d.Width) && (Height == a2d.Height) && (GetAngleStart() == a2d.GetAngleStart()) && (AngleExtent == a2d.AngleExtent) && (ArcType == a2d.ArcType));
			}
			return false;
		}
	}

}