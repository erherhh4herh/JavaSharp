using System;

/*
 * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	/// A point representing a location in {@code (x,y)} coordinate space,
	/// specified in integer precision.
	/// 
	/// @author      Sami Shaio
	/// @since       1.0
	/// </summary>
	[Serializable]
	public class Point : Point2D
	{
		/// <summary>
		/// The X coordinate of this <code>Point</code>.
		/// If no X coordinate is set it will default to 0.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLocation() </seealso>
		/// <seealso cref= #move(int, int)
		/// @since 1.0 </seealso>
		public int x;

		/// <summary>
		/// The Y coordinate of this <code>Point</code>.
		/// If no Y coordinate is set it will default to 0.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLocation() </seealso>
		/// <seealso cref= #move(int, int)
		/// @since 1.0 </seealso>
		public int y;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -5276940640259749850L;

		/// <summary>
		/// Constructs and initializes a point at the origin
		/// (0,&nbsp;0) of the coordinate space.
		/// @since       1.1
		/// </summary>
		public Point() : this(0, 0)
		{
		}

		/// <summary>
		/// Constructs and initializes a point with the same location as
		/// the specified <code>Point</code> object. </summary>
		/// <param name="p"> a point
		/// @since       1.1 </param>
		public Point(Point p) : this(p.x, p.y)
		{
		}

		/// <summary>
		/// Constructs and initializes a point at the specified
		/// {@code (x,y)} location in the coordinate space. </summary>
		/// <param name="x"> the X coordinate of the newly constructed <code>Point</code> </param>
		/// <param name="y"> the Y coordinate of the newly constructed <code>Point</code>
		/// @since 1.0 </param>
		public Point(int x, int y)
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
		/// Returns the location of this point.
		/// This method is included for completeness, to parallel the
		/// <code>getLocation</code> method of <code>Component</code>. </summary>
		/// <returns>      a copy of this point, at the same location </returns>
		/// <seealso cref=         java.awt.Component#getLocation </seealso>
		/// <seealso cref=         java.awt.Point#setLocation(java.awt.Point) </seealso>
		/// <seealso cref=         java.awt.Point#setLocation(int, int)
		/// @since       1.1 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transient public Point getLocation()
		public virtual Point Location
		{
			get
			{
				return new Point(x, y);
			}
			set
			{
				SetLocation(value.x, value.y);
			}
		}


		/// <summary>
		/// Changes the point to have the specified location.
		/// <para>
		/// This method is included for completeness, to parallel the
		/// <code>setLocation</code> method of <code>Component</code>.
		/// Its behavior is identical with <code>move(int,&nbsp;int)</code>.
		/// </para>
		/// </summary>
		/// <param name="x"> the X coordinate of the new location </param>
		/// <param name="y"> the Y coordinate of the new location </param>
		/// <seealso cref=         java.awt.Component#setLocation(int, int) </seealso>
		/// <seealso cref=         java.awt.Point#getLocation </seealso>
		/// <seealso cref=         java.awt.Point#move(int, int)
		/// @since       1.1 </seealso>
		public virtual void SetLocation(int x, int y)
		{
			Move(x, y);
		}

		/// <summary>
		/// Sets the location of this point to the specified double coordinates.
		/// The double values will be rounded to integer values.
		/// Any number smaller than <code>Integer.MIN_VALUE</code>
		/// will be reset to <code>MIN_VALUE</code>, and any number
		/// larger than <code>Integer.MAX_VALUE</code> will be
		/// reset to <code>MAX_VALUE</code>.
		/// </summary>
		/// <param name="x"> the X coordinate of the new location </param>
		/// <param name="y"> the Y coordinate of the new location </param>
		/// <seealso cref= #getLocation </seealso>
		public override void SetLocation(double x, double y)
		{
			this.x = (int) System.Math.Floor(x + 0.5);
			this.y = (int) System.Math.Floor(y + 0.5);
		}

		/// <summary>
		/// Moves this point to the specified location in the
		/// {@code (x,y)} coordinate plane. This method
		/// is identical with <code>setLocation(int,&nbsp;int)</code>. </summary>
		/// <param name="x"> the X coordinate of the new location </param>
		/// <param name="y"> the Y coordinate of the new location </param>
		/// <seealso cref=         java.awt.Component#setLocation(int, int) </seealso>
		public virtual void Move(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Translates this point, at location {@code (x,y)},
		/// by {@code dx} along the {@code x} axis and {@code dy}
		/// along the {@code y} axis so that it now represents the point
		/// {@code (x+dx,y+dy)}.
		/// </summary>
		/// <param name="dx">   the distance to move this point
		///                            along the X axis </param>
		/// <param name="dy">    the distance to move this point
		///                            along the Y axis </param>
		public virtual void Translate(int dx, int dy)
		{
			this.x += dx;
			this.y += dy;
		}

		/// <summary>
		/// Determines whether or not two points are equal. Two instances of
		/// <code>Point2D</code> are equal if the values of their
		/// <code>x</code> and <code>y</code> member fields, representing
		/// their position in the coordinate space, are the same. </summary>
		/// <param name="obj"> an object to be compared with this <code>Point2D</code> </param>
		/// <returns> <code>true</code> if the object to be compared is
		///         an instance of <code>Point2D</code> and has
		///         the same values; <code>false</code> otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj is Point)
			{
				Point pt = (Point)obj;
				return (x == pt.x) && (y == pt.y);
			}
			return base.Equals(obj);
		}

		/// <summary>
		/// Returns a string representation of this point and its location
		/// in the {@code (x,y)} coordinate space. This method is
		/// intended to be used only for debugging purposes, and the content
		/// and format of the returned string may vary between implementations.
		/// The returned string may be empty but may not be <code>null</code>.
		/// </summary>
		/// <returns>  a string representation of this point </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[x=" + x + ",y=" + y + "]";
		}
	}

}