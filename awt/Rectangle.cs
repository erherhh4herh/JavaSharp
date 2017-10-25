using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A <code>Rectangle</code> specifies an area in a coordinate space that is
	/// enclosed by the <code>Rectangle</code> object's upper-left point
	/// {@code (x,y)}
	/// in the coordinate space, its width, and its height.
	/// <para>
	/// A <code>Rectangle</code> object's <code>width</code> and
	/// <code>height</code> are <code>public</code> fields. The constructors
	/// that create a <code>Rectangle</code>, and the methods that can modify
	/// one, do not prevent setting a negative value for width or height.
	/// </para>
	/// <para>
	/// <a name="Empty">
	/// A {@code Rectangle} whose width or height is exactly zero has location
	/// along those axes with zero dimension, but is otherwise considered empty.
	/// The <seealso cref="#isEmpty"/> method will return true for such a {@code Rectangle}.
	/// Methods which test if an empty {@code Rectangle} contains or intersects
	/// a point or rectangle will always return false if either dimension is zero.
	/// Methods which combine such a {@code Rectangle} with a point or rectangle
	/// will include the location of the {@code Rectangle} on that axis in the
	/// result as if the <seealso cref="#add(Point)"/> method were being called.
	/// </a>
	/// </para>
	/// <para>
	/// <a name="NonExistant">
	/// A {@code Rectangle} whose width or height is negative has neither
	/// location nor dimension along those axes with negative dimensions.
	/// Such a {@code Rectangle} is treated as non-existant along those axes.
	/// Such a {@code Rectangle} is also empty with respect to containment
	/// calculations and methods which test if it contains or intersects a
	/// point or rectangle will always return false.
	/// Methods which combine such a {@code Rectangle} with a point or rectangle
	/// will ignore the {@code Rectangle} entirely in generating the result.
	/// If two {@code Rectangle} objects are combined and each has a negative
	/// dimension, the result will have at least one negative dimension.
	/// </a>
	/// </para>
	/// <para>
	/// Methods which affect only the location of a {@code Rectangle} will
	/// operate on its location regardless of whether or not it has a negative
	/// or zero dimension along either axis.
	/// </para>
	/// <para>
	/// Note that a {@code Rectangle} constructed with the default no-argument
	/// constructor will have dimensions of {@code 0x0} and therefore be empty.
	/// That {@code Rectangle} will still have a location of {@code (0,0)} and
	/// will contribute that location to the union and add operations.
	/// Code attempting to accumulate the bounds of a set of points should
	/// therefore initially construct the {@code Rectangle} with a specifically
	/// negative width and height or it should use the first point in the set
	/// to construct the {@code Rectangle}.
	/// For example:
	/// <pre>{@code
	///     Rectangle bounds = new Rectangle(0, 0, -1, -1);
	///     for (int i = 0; i < points.length; i++) {
	///         bounds.add(points[i]);
	///     }
	/// }</pre>
	/// or if we know that the points array contains at least one point:
	/// <pre>{@code
	///     Rectangle bounds = new Rectangle(points[0]);
	///     for (int i = 1; i < points.length; i++) {
	///         bounds.add(points[i]);
	///     }
	/// }</pre>
	/// </para>
	/// <para>
	/// This class uses 32-bit integers to store its location and dimensions.
	/// Frequently operations may produce a result that exceeds the range of
	/// a 32-bit integer.
	/// The methods will calculate their results in a way that avoids any
	/// 32-bit overflow for intermediate results and then choose the best
	/// representation to store the final results back into the 32-bit fields
	/// which hold the location and dimensions.
	/// The location of the result will be stored into the <seealso cref="#x"/> and
	/// <seealso cref="#y"/> fields by clipping the true result to the nearest 32-bit value.
	/// The values stored into the <seealso cref="#width"/> and <seealso cref="#height"/> dimension
	/// fields will be chosen as the 32-bit values that encompass the largest
	/// part of the true result as possible.
	/// Generally this means that the dimension will be clipped independently
	/// to the range of 32-bit integers except that if the location had to be
	/// moved to store it into its pair of 32-bit fields then the dimensions
	/// will be adjusted relative to the "best representation" of the location.
	/// If the true result had a negative dimension and was therefore
	/// non-existant along one or both axes, the stored dimensions will be
	/// negative numbers in those axes.
	/// If the true result had a location that could be represented within
	/// the range of 32-bit integers, but zero dimension along one or both
	/// axes, then the stored dimensions will be zero in those axes.
	/// 
	/// @author      Sami Shaio
	/// @since 1.0
	/// </para>
	/// </summary>
	[Serializable]
	public class Rectangle : Rectangle2D, Shape
	{

		/// <summary>
		/// The X coordinate of the upper-left corner of the <code>Rectangle</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setLocation(int, int) </seealso>
		/// <seealso cref= #getLocation()
		/// @since 1.0 </seealso>
		public int x;

		/// <summary>
		/// The Y coordinate of the upper-left corner of the <code>Rectangle</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setLocation(int, int) </seealso>
		/// <seealso cref= #getLocation()
		/// @since 1.0 </seealso>
		public int y;

		/// <summary>
		/// The width of the <code>Rectangle</code>.
		/// @serial </summary>
		/// <seealso cref= #setSize(int, int) </seealso>
		/// <seealso cref= #getSize()
		/// @since 1.0 </seealso>
		public int Width_Renamed;

		/// <summary>
		/// The height of the <code>Rectangle</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setSize(int, int) </seealso>
		/// <seealso cref= #getSize()
		/// @since 1.0 </seealso>
		public int Height_Renamed;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = -4345857070255674764L;

		/// <summary>
		/// Initialize JNI field and method IDs
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		static Rectangle()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// Constructs a new <code>Rectangle</code> whose upper-left corner
		/// is at (0,&nbsp;0) in the coordinate space, and whose width and
		/// height are both zero.
		/// </summary>
		public Rectangle() : this(0, 0, 0, 0)
		{
		}

		/// <summary>
		/// Constructs a new <code>Rectangle</code>, initialized to match
		/// the values of the specified <code>Rectangle</code>. </summary>
		/// <param name="r">  the <code>Rectangle</code> from which to copy initial values
		///           to a newly constructed <code>Rectangle</code>
		/// @since 1.1 </param>
		public Rectangle(Rectangle r) : this(r.x, r.y, r.Width_Renamed, r.Height_Renamed)
		{
		}

		/// <summary>
		/// Constructs a new <code>Rectangle</code> whose upper-left corner is
		/// specified as
		/// {@code (x,y)} and whose width and height
		/// are specified by the arguments of the same name. </summary>
		/// <param name="x"> the specified X coordinate </param>
		/// <param name="y"> the specified Y coordinate </param>
		/// <param name="width">    the width of the <code>Rectangle</code> </param>
		/// <param name="height">   the height of the <code>Rectangle</code>
		/// @since 1.0 </param>
		public Rectangle(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.Width_Renamed = width;
			this.Height_Renamed = height;
		}

		/// <summary>
		/// Constructs a new <code>Rectangle</code> whose upper-left corner
		/// is at (0,&nbsp;0) in the coordinate space, and whose width and
		/// height are specified by the arguments of the same name. </summary>
		/// <param name="width"> the width of the <code>Rectangle</code> </param>
		/// <param name="height"> the height of the <code>Rectangle</code> </param>
		public Rectangle(int width, int height) : this(0, 0, width, height)
		{
		}

		/// <summary>
		/// Constructs a new <code>Rectangle</code> whose upper-left corner is
		/// specified by the <seealso cref="Point"/> argument, and
		/// whose width and height are specified by the
		/// <seealso cref="Dimension"/> argument. </summary>
		/// <param name="p"> a <code>Point</code> that is the upper-left corner of
		/// the <code>Rectangle</code> </param>
		/// <param name="d"> a <code>Dimension</code>, representing the
		/// width and height of the <code>Rectangle</code> </param>
		public Rectangle(Point p, Dimension d) : this(p.x, p.y, d.Width_Renamed, d.Height_Renamed)
		{
		}

		/// <summary>
		/// Constructs a new <code>Rectangle</code> whose upper-left corner is the
		/// specified <code>Point</code>, and whose width and height are both zero. </summary>
		/// <param name="p"> a <code>Point</code> that is the top left corner
		/// of the <code>Rectangle</code> </param>
		public Rectangle(Point p) : this(p.x, p.y, 0, 0)
		{
		}

		/// <summary>
		/// Constructs a new <code>Rectangle</code> whose top left corner is
		/// (0,&nbsp;0) and whose width and height are specified
		/// by the <code>Dimension</code> argument. </summary>
		/// <param name="d"> a <code>Dimension</code>, specifying width and height </param>
		public Rectangle(Dimension d) : this(0, 0, d.Width_Renamed, d.Height_Renamed)
		{
		}

		/// <summary>
		/// Returns the X coordinate of the bounding <code>Rectangle</code> in
		/// <code>double</code> precision. </summary>
		/// <returns> the X coordinate of the bounding <code>Rectangle</code>. </returns>
		public override double X
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// Returns the Y coordinate of the bounding <code>Rectangle</code> in
		/// <code>double</code> precision. </summary>
		/// <returns> the Y coordinate of the bounding <code>Rectangle</code>. </returns>
		public override double Y
		{
			get
			{
				return y;
			}
		}

		/// <summary>
		/// Returns the width of the bounding <code>Rectangle</code> in
		/// <code>double</code> precision. </summary>
		/// <returns> the width of the bounding <code>Rectangle</code>. </returns>
		public override double Width
		{
			get
			{
				return Width_Renamed;
			}
		}

		/// <summary>
		/// Returns the height of the bounding <code>Rectangle</code> in
		/// <code>double</code> precision. </summary>
		/// <returns> the height of the bounding <code>Rectangle</code>. </returns>
		public override double Height
		{
			get
			{
				return Height_Renamed;
			}
		}

		/// <summary>
		/// Gets the bounding <code>Rectangle</code> of this <code>Rectangle</code>.
		/// <para>
		/// This method is included for completeness, to parallel the
		/// <code>getBounds</code> method of
		/// <seealso cref="Component"/>.
		/// </para>
		/// </summary>
		/// <returns>    a new <code>Rectangle</code>, equal to the
		/// bounding <code>Rectangle</code> for this <code>Rectangle</code>. </returns>
		/// <seealso cref=       java.awt.Component#getBounds </seealso>
		/// <seealso cref=       #setBounds(Rectangle) </seealso>
		/// <seealso cref=       #setBounds(int, int, int, int)
		/// @since     1.1 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transient public Rectangle getBounds()
		public override Rectangle Bounds
		{
			get
			{
				return new Rectangle(x, y, Width_Renamed, Height_Renamed);
			}
			set
			{
				SetBounds(value.x, value.y, value.Width_Renamed, value.Height_Renamed);
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
				return new Rectangle(x, y, Width_Renamed, Height_Renamed);
			}
		}


		/// <summary>
		/// Sets the bounding <code>Rectangle</code> of this
		/// <code>Rectangle</code> to the specified
		/// <code>x</code>, <code>y</code>, <code>width</code>,
		/// and <code>height</code>.
		/// <para>
		/// This method is included for completeness, to parallel the
		/// <code>setBounds</code> method of <code>Component</code>.
		/// </para>
		/// </summary>
		/// <param name="x"> the new X coordinate for the upper-left
		///                    corner of this <code>Rectangle</code> </param>
		/// <param name="y"> the new Y coordinate for the upper-left
		///                    corner of this <code>Rectangle</code> </param>
		/// <param name="width"> the new width for this <code>Rectangle</code> </param>
		/// <param name="height"> the new height for this <code>Rectangle</code> </param>
		/// <seealso cref=       #getBounds </seealso>
		/// <seealso cref=       java.awt.Component#setBounds(int, int, int, int)
		/// @since     1.1 </seealso>
		public virtual void SetBounds(int x, int y, int width, int height)
		{
			Reshape(x, y, width, height);
		}

		/// <summary>
		/// Sets the bounds of this {@code Rectangle} to the integer bounds
		/// which encompass the specified {@code x}, {@code y}, {@code width},
		/// and {@code height}.
		/// If the parameters specify a {@code Rectangle} that exceeds the
		/// maximum range of integers, the result will be the best
		/// representation of the specified {@code Rectangle} intersected
		/// with the maximum integer bounds. </summary>
		/// <param name="x"> the X coordinate of the upper-left corner of
		///                  the specified rectangle </param>
		/// <param name="y"> the Y coordinate of the upper-left corner of
		///                  the specified rectangle </param>
		/// <param name="width"> the width of the specified rectangle </param>
		/// <param name="height"> the new height of the specified rectangle </param>
		public override void SetRect(double x, double y, double width, double height)
		{
			int newx, newy, neww, newh;

			if (x > 2.0 * Integer.MaxValue)
			{
				// Too far in positive X direction to represent...
				// We cannot even reach the left side of the specified
				// rectangle even with both x & width set to MAX_VALUE.
				// The intersection with the "maximal integer rectangle"
				// is non-existant so we should use a width < 0.
				// REMIND: Should we try to determine a more "meaningful"
				// adjusted value for neww than just "-1"?
				newx = Integer.MaxValue;
				neww = -1;
			}
			else
			{
				newx = Clip(x, false);
				if (width >= 0)
				{
					width += x - newx;
				}
				neww = Clip(width, width >= 0);
			}

			if (y > 2.0 * Integer.MaxValue)
			{
				// Too far in positive Y direction to represent...
				newy = Integer.MaxValue;
				newh = -1;
			}
			else
			{
				newy = Clip(y, false);
				if (height >= 0)
				{
					height += y - newy;
				}
				newh = Clip(height, height >= 0);
			}

			Reshape(newx, newy, neww, newh);
		}
		// Return best integer representation for v, clipped to integer
		// range and floor-ed or ceiling-ed, depending on the boolean.
		private static int Clip(double v, bool doceil)
		{
			if (v <= Integer.MinValue)
			{
				return Integer.MinValue;
			}
			if (v >= Integer.MaxValue)
			{
				return Integer.MaxValue;
			}
			return (int)(doceil ? System.Math.Ceiling(v) : System.Math.Floor(v));
		}

		/// <summary>
		/// Sets the bounding <code>Rectangle</code> of this
		/// <code>Rectangle</code> to the specified
		/// <code>x</code>, <code>y</code>, <code>width</code>,
		/// and <code>height</code>.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="x"> the new X coordinate for the upper-left
		///                    corner of this <code>Rectangle</code> </param>
		/// <param name="y"> the new Y coordinate for the upper-left
		///                    corner of this <code>Rectangle</code> </param>
		/// <param name="width"> the new width for this <code>Rectangle</code> </param>
		/// <param name="height"> the new height for this <code>Rectangle</code> </param>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setBounds(int, int, int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Reshape(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.Width_Renamed = width;
			this.Height_Renamed = height;
		}

		/// <summary>
		/// Returns the location of this <code>Rectangle</code>.
		/// <para>
		/// This method is included for completeness, to parallel the
		/// <code>getLocation</code> method of <code>Component</code>.
		/// </para>
		/// </summary>
		/// <returns> the <code>Point</code> that is the upper-left corner of
		///                  this <code>Rectangle</code>. </returns>
		/// <seealso cref=       java.awt.Component#getLocation </seealso>
		/// <seealso cref=       #setLocation(Point) </seealso>
		/// <seealso cref=       #setLocation(int, int)
		/// @since     1.1 </seealso>
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
		/// Moves this <code>Rectangle</code> to the specified location.
		/// <para>
		/// This method is included for completeness, to parallel the
		/// <code>setLocation</code> method of <code>Component</code>.
		/// </para>
		/// </summary>
		/// <param name="x"> the X coordinate of the new location </param>
		/// <param name="y"> the Y coordinate of the new location </param>
		/// <seealso cref=       #getLocation </seealso>
		/// <seealso cref=       java.awt.Component#setLocation(int, int)
		/// @since     1.1 </seealso>
		public virtual void SetLocation(int x, int y)
		{
			Move(x, y);
		}

		/// <summary>
		/// Moves this <code>Rectangle</code> to the specified location.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="x"> the X coordinate of the new location </param>
		/// <param name="y"> the Y coordinate of the new location </param>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setLocation(int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Move(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Translates this <code>Rectangle</code> the indicated distance,
		/// to the right along the X coordinate axis, and
		/// downward along the Y coordinate axis. </summary>
		/// <param name="dx"> the distance to move this <code>Rectangle</code>
		///                 along the X axis </param>
		/// <param name="dy"> the distance to move this <code>Rectangle</code>
		///                 along the Y axis </param>
		/// <seealso cref=       java.awt.Rectangle#setLocation(int, int) </seealso>
		/// <seealso cref=       java.awt.Rectangle#setLocation(java.awt.Point) </seealso>
		public virtual void Translate(int dx, int dy)
		{
			int oldv = this.x;
			int newv = oldv + dx;
			if (dx < 0)
			{
				// moving leftward
				if (newv > oldv)
				{
					// negative overflow
					// Only adjust width if it was valid (>= 0).
					if (Width_Renamed >= 0)
					{
						// The right edge is now conceptually at
						// newv+width, but we may move newv to prevent
						// overflow.  But we want the right edge to
						// remain at its new location in spite of the
						// clipping.  Think of the following adjustment
						// conceptually the same as:
						// width += newv; newv = MIN_VALUE; width -= newv;
						Width_Renamed += newv - Integer.MinValue;
						// width may go negative if the right edge went past
						// MIN_VALUE, but it cannot overflow since it cannot
						// have moved more than MIN_VALUE and any non-negative
						// number + MIN_VALUE does not overflow.
					}
					newv = Integer.MinValue;
				}
			}
			else
			{
				// moving rightward (or staying still)
				if (newv < oldv)
				{
					// positive overflow
					if (Width_Renamed >= 0)
					{
						// Conceptually the same as:
						// width += newv; newv = MAX_VALUE; width -= newv;
						Width_Renamed += newv - Integer.MaxValue;
						// With large widths and large displacements
						// we may overflow so we need to check it.
						if (Width_Renamed < 0)
						{
							Width_Renamed = Integer.MaxValue;
						}
					}
					newv = Integer.MaxValue;
				}
			}
			this.x = newv;

			oldv = this.y;
			newv = oldv + dy;
			if (dy < 0)
			{
				// moving upward
				if (newv > oldv)
				{
					// negative overflow
					if (Height_Renamed >= 0)
					{
						Height_Renamed += newv - Integer.MinValue;
						// See above comment about no overflow in this case
					}
					newv = Integer.MinValue;
				}
			}
			else
			{
				// moving downward (or staying still)
				if (newv < oldv)
				{
					// positive overflow
					if (Height_Renamed >= 0)
					{
						Height_Renamed += newv - Integer.MaxValue;
						if (Height_Renamed < 0)
						{
							Height_Renamed = Integer.MaxValue;
						}
					}
					newv = Integer.MaxValue;
				}
			}
			this.y = newv;
		}

		/// <summary>
		/// Gets the size of this <code>Rectangle</code>, represented by
		/// the returned <code>Dimension</code>.
		/// <para>
		/// This method is included for completeness, to parallel the
		/// <code>getSize</code> method of <code>Component</code>.
		/// </para>
		/// </summary>
		/// <returns> a <code>Dimension</code>, representing the size of
		///            this <code>Rectangle</code>. </returns>
		/// <seealso cref=       java.awt.Component#getSize </seealso>
		/// <seealso cref=       #setSize(Dimension) </seealso>
		/// <seealso cref=       #setSize(int, int)
		/// @since     1.1 </seealso>
		public virtual Dimension Size
		{
			get
			{
				return new Dimension(Width_Renamed, Height_Renamed);
			}
			set
			{
				SetSize(value.Width_Renamed, value.Height_Renamed);
			}
		}


		/// <summary>
		/// Sets the size of this <code>Rectangle</code> to the specified
		/// width and height.
		/// <para>
		/// This method is included for completeness, to parallel the
		/// <code>setSize</code> method of <code>Component</code>.
		/// </para>
		/// </summary>
		/// <param name="width"> the new width for this <code>Rectangle</code> </param>
		/// <param name="height"> the new height for this <code>Rectangle</code> </param>
		/// <seealso cref=       java.awt.Component#setSize(int, int) </seealso>
		/// <seealso cref=       #getSize
		/// @since     1.1 </seealso>
		public virtual void SetSize(int width, int height)
		{
			Resize(width, height);
		}

		/// <summary>
		/// Sets the size of this <code>Rectangle</code> to the specified
		/// width and height.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="width"> the new width for this <code>Rectangle</code> </param>
		/// <param name="height"> the new height for this <code>Rectangle</code> </param>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setSize(int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Resize(int width, int height)
		{
			this.Width_Renamed = width;
			this.Height_Renamed = height;
		}

		/// <summary>
		/// Checks whether or not this <code>Rectangle</code> contains the
		/// specified <code>Point</code>. </summary>
		/// <param name="p"> the <code>Point</code> to test </param>
		/// <returns>    <code>true</code> if the specified <code>Point</code>
		///            is inside this <code>Rectangle</code>;
		///            <code>false</code> otherwise.
		/// @since     1.1 </returns>
		public virtual bool Contains(Point p)
		{
			return Contains(p.x, p.y);
		}

		/// <summary>
		/// Checks whether or not this <code>Rectangle</code> contains the
		/// point at the specified location {@code (x,y)}.
		/// </summary>
		/// <param name="x"> the specified X coordinate </param>
		/// <param name="y"> the specified Y coordinate </param>
		/// <returns>    <code>true</code> if the point
		///            {@code (x,y)} is inside this
		///            <code>Rectangle</code>;
		///            <code>false</code> otherwise.
		/// @since     1.1 </returns>
		public virtual bool Contains(int x, int y)
		{
			return Inside(x, y);
		}

		/// <summary>
		/// Checks whether or not this <code>Rectangle</code> entirely contains
		/// the specified <code>Rectangle</code>.
		/// </summary>
		/// <param name="r">   the specified <code>Rectangle</code> </param>
		/// <returns>    <code>true</code> if the <code>Rectangle</code>
		///            is contained entirely inside this <code>Rectangle</code>;
		///            <code>false</code> otherwise
		/// @since     1.2 </returns>
		public virtual bool Contains(Rectangle r)
		{
			return Contains(r.x, r.y, r.Width_Renamed, r.Height_Renamed);
		}

		/// <summary>
		/// Checks whether this <code>Rectangle</code> entirely contains
		/// the <code>Rectangle</code>
		/// at the specified location {@code (X,Y)} with the
		/// specified dimensions {@code (W,H)}. </summary>
		/// <param name="X"> the specified X coordinate </param>
		/// <param name="Y"> the specified Y coordinate </param>
		/// <param name="W">   the width of the <code>Rectangle</code> </param>
		/// <param name="H">   the height of the <code>Rectangle</code> </param>
		/// <returns>    <code>true</code> if the <code>Rectangle</code> specified by
		///            {@code (X, Y, W, H)}
		///            is entirely enclosed inside this <code>Rectangle</code>;
		///            <code>false</code> otherwise.
		/// @since     1.1 </returns>
		public virtual bool Contains(int X, int Y, int W, int H)
		{
			int w = this.Width_Renamed;
			int h = this.Height_Renamed;
			if ((w | h | W | H) < 0)
			{
				// At least one of the dimensions is negative...
				return false;
			}
			// Note: if any dimension is zero, tests below must return false...
			int x = this.x;
			int y = this.y;
			if (X < x || Y < y)
			{
				return false;
			}
			w += x;
			W += X;
			if (W <= X)
			{
				// X+W overflowed or W was zero, return false if...
				// either original w or W was zero or
				// x+w did not overflow or
				// the overflowed x+w is smaller than the overflowed X+W
				if (w >= x || W > w)
				{
					return false;
				}
			}
			else
			{
				// X+W did not overflow and W was not zero, return false if...
				// original w was zero or
				// x+w did not overflow and x+w is smaller than X+W
				if (w >= x && W > w)
				{
					return false;
				}
			}
			h += y;
			H += Y;
			if (H <= Y)
			{
				if (h >= y || H > h)
				{
					return false;
				}
			}
			else
			{
				if (h >= y && H > h)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Checks whether or not this <code>Rectangle</code> contains the
		/// point at the specified location {@code (X,Y)}.
		/// </summary>
		/// <param name="X"> the specified X coordinate </param>
		/// <param name="Y"> the specified Y coordinate </param>
		/// <returns>    <code>true</code> if the point
		///            {@code (X,Y)} is inside this
		///            <code>Rectangle</code>;
		///            <code>false</code> otherwise. </returns>
		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>contains(int, int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool Inside(int X, int Y)
		{
			int w = this.Width_Renamed;
			int h = this.Height_Renamed;
			if ((w | h) < 0)
			{
				// At least one of the dimensions is negative...
				return false;
			}
			// Note: if either dimension is zero, tests below must return false...
			int x = this.x;
			int y = this.y;
			if (X < x || Y < y)
			{
				return false;
			}
			w += x;
			h += y;
			//    overflow || intersect
			return ((w < x || w > X) && (h < y || h > Y));
		}

		/// <summary>
		/// Determines whether or not this <code>Rectangle</code> and the specified
		/// <code>Rectangle</code> intersect. Two rectangles intersect if
		/// their intersection is nonempty.
		/// </summary>
		/// <param name="r"> the specified <code>Rectangle</code> </param>
		/// <returns>    <code>true</code> if the specified <code>Rectangle</code>
		///            and this <code>Rectangle</code> intersect;
		///            <code>false</code> otherwise. </returns>
		public virtual bool Intersects(Rectangle r)
		{
			int tw = this.Width_Renamed;
			int th = this.Height_Renamed;
			int rw = r.Width_Renamed;
			int rh = r.Height_Renamed;
			if (rw <= 0 || rh <= 0 || tw <= 0 || th <= 0)
			{
				return false;
			}
			int tx = this.x;
			int ty = this.y;
			int rx = r.x;
			int ry = r.y;
			rw += rx;
			rh += ry;
			tw += tx;
			th += ty;
			//      overflow || intersect
			return ((rw < rx || rw > tx) && (rh < ry || rh > ty) && (tw < tx || tw > rx) && (th < ty || th > ry));
		}

		/// <summary>
		/// Computes the intersection of this <code>Rectangle</code> with the
		/// specified <code>Rectangle</code>. Returns a new <code>Rectangle</code>
		/// that represents the intersection of the two rectangles.
		/// If the two rectangles do not intersect, the result will be
		/// an empty rectangle.
		/// </summary>
		/// <param name="r">   the specified <code>Rectangle</code> </param>
		/// <returns>    the largest <code>Rectangle</code> contained in both the
		///            specified <code>Rectangle</code> and in
		///            this <code>Rectangle</code>; or if the rectangles
		///            do not intersect, an empty rectangle. </returns>
		public virtual Rectangle Intersection(Rectangle r)
		{
			int tx1 = this.x;
			int ty1 = this.y;
			int rx1 = r.x;
			int ry1 = r.y;
			long tx2 = tx1;
			tx2 += this.Width_Renamed;
			long ty2 = ty1;
			ty2 += this.Height_Renamed;
			long rx2 = rx1;
			rx2 += r.Width_Renamed;
			long ry2 = ry1;
			ry2 += r.Height_Renamed;
			if (tx1 < rx1)
			{
				tx1 = rx1;
			}
			if (ty1 < ry1)
			{
				ty1 = ry1;
			}
			if (tx2 > rx2)
			{
				tx2 = rx2;
			}
			if (ty2 > ry2)
			{
				ty2 = ry2;
			}
			tx2 -= tx1;
			ty2 -= ty1;
			// tx2,ty2 will never overflow (they will never be
			// larger than the smallest of the two source w,h)
			// they might underflow, though...
			if (tx2 < Integer.MinValue)
			{
				tx2 = Integer.MinValue;
			}
			if (ty2 < Integer.MinValue)
			{
				ty2 = Integer.MinValue;
			}
			return new Rectangle(tx1, ty1, (int) tx2, (int) ty2);
		}

		/// <summary>
		/// Computes the union of this <code>Rectangle</code> with the
		/// specified <code>Rectangle</code>. Returns a new
		/// <code>Rectangle</code> that
		/// represents the union of the two rectangles.
		/// <para>
		/// If either {@code Rectangle} has any dimension less than zero
		/// the rules for <a href=#NonExistant>non-existant</a> rectangles
		/// apply.
		/// If only one has a dimension less than zero, then the result
		/// will be a copy of the other {@code Rectangle}.
		/// If both have dimension less than zero, then the result will
		/// have at least one dimension less than zero.
		/// </para>
		/// <para>
		/// If the resulting {@code Rectangle} would have a dimension
		/// too large to be expressed as an {@code int}, the result
		/// will have a dimension of {@code Integer.MAX_VALUE} along
		/// that dimension.
		/// </para>
		/// </summary>
		/// <param name="r"> the specified <code>Rectangle</code> </param>
		/// <returns>    the smallest <code>Rectangle</code> containing both
		///            the specified <code>Rectangle</code> and this
		///            <code>Rectangle</code>. </returns>
		public virtual Rectangle Union(Rectangle r)
		{
			long tx2 = this.Width_Renamed;
			long ty2 = this.Height_Renamed;
			if ((tx2 | ty2) < 0)
			{
				// This rectangle has negative dimensions...
				// If r has non-negative dimensions then it is the answer.
				// If r is non-existant (has a negative dimension), then both
				// are non-existant and we can return any non-existant rectangle
				// as an answer.  Thus, returning r meets that criterion.
				// Either way, r is our answer.
				return new Rectangle(r);
			}
			long rx2 = r.Width_Renamed;
			long ry2 = r.Height_Renamed;
			if ((rx2 | ry2) < 0)
			{
				return new Rectangle(this);
			}
			int tx1 = this.x;
			int ty1 = this.y;
			tx2 += tx1;
			ty2 += ty1;
			int rx1 = r.x;
			int ry1 = r.y;
			rx2 += rx1;
			ry2 += ry1;
			if (tx1 > rx1)
			{
				tx1 = rx1;
			}
			if (ty1 > ry1)
			{
				ty1 = ry1;
			}
			if (tx2 < rx2)
			{
				tx2 = rx2;
			}
			if (ty2 < ry2)
			{
				ty2 = ry2;
			}
			tx2 -= tx1;
			ty2 -= ty1;
			// tx2,ty2 will never underflow since both original rectangles
			// were already proven to be non-empty
			// they might overflow, though...
			if (tx2 > Integer.MaxValue)
			{
				tx2 = Integer.MaxValue;
			}
			if (ty2 > Integer.MaxValue)
			{
				ty2 = Integer.MaxValue;
			}
			return new Rectangle(tx1, ty1, (int) tx2, (int) ty2);
		}

		/// <summary>
		/// Adds a point, specified by the integer arguments {@code newx,newy}
		/// to the bounds of this {@code Rectangle}.
		/// <para>
		/// If this {@code Rectangle} has any dimension less than zero,
		/// the rules for <a href=#NonExistant>non-existant</a>
		/// rectangles apply.
		/// In that case, the new bounds of this {@code Rectangle} will
		/// have a location equal to the specified coordinates and
		/// width and height equal to zero.
		/// </para>
		/// <para>
		/// After adding a point, a call to <code>contains</code> with the
		/// added point as an argument does not necessarily return
		/// <code>true</code>. The <code>contains</code> method does not
		/// return <code>true</code> for points on the right or bottom
		/// edges of a <code>Rectangle</code>. Therefore, if the added point
		/// falls on the right or bottom edge of the enlarged
		/// <code>Rectangle</code>, <code>contains</code> returns
		/// <code>false</code> for that point.
		/// If the specified point must be contained within the new
		/// {@code Rectangle}, a 1x1 rectangle should be added instead:
		/// <pre>
		///     r.add(newx, newy, 1, 1);
		/// </pre>
		/// </para>
		/// </summary>
		/// <param name="newx"> the X coordinate of the new point </param>
		/// <param name="newy"> the Y coordinate of the new point </param>
		public virtual void Add(int newx, int newy)
		{
			if ((Width_Renamed | Height_Renamed) < 0)
			{
				this.x = newx;
				this.y = newy;
				this.Width_Renamed = this.Height_Renamed = 0;
				return;
			}
			int x1 = this.x;
			int y1 = this.y;
			long x2 = this.Width_Renamed;
			long y2 = this.Height_Renamed;
			x2 += x1;
			y2 += y1;
			if (x1 > newx)
			{
				x1 = newx;
			}
			if (y1 > newy)
			{
				y1 = newy;
			}
			if (x2 < newx)
			{
				x2 = newx;
			}
			if (y2 < newy)
			{
				y2 = newy;
			}
			x2 -= x1;
			y2 -= y1;
			if (x2 > Integer.MaxValue)
			{
				x2 = Integer.MaxValue;
			}
			if (y2 > Integer.MaxValue)
			{
				y2 = Integer.MaxValue;
			}
			Reshape(x1, y1, (int) x2, (int) y2);
		}

		/// <summary>
		/// Adds the specified {@code Point} to the bounds of this
		/// {@code Rectangle}.
		/// <para>
		/// If this {@code Rectangle} has any dimension less than zero,
		/// the rules for <a href=#NonExistant>non-existant</a>
		/// rectangles apply.
		/// In that case, the new bounds of this {@code Rectangle} will
		/// have a location equal to the coordinates of the specified
		/// {@code Point} and width and height equal to zero.
		/// </para>
		/// <para>
		/// After adding a <code>Point</code>, a call to <code>contains</code>
		/// with the added <code>Point</code> as an argument does not
		/// necessarily return <code>true</code>. The <code>contains</code>
		/// method does not return <code>true</code> for points on the right
		/// or bottom edges of a <code>Rectangle</code>. Therefore if the added
		/// <code>Point</code> falls on the right or bottom edge of the
		/// enlarged <code>Rectangle</code>, <code>contains</code> returns
		/// <code>false</code> for that <code>Point</code>.
		/// If the specified point must be contained within the new
		/// {@code Rectangle}, a 1x1 rectangle should be added instead:
		/// <pre>
		///     r.add(pt.x, pt.y, 1, 1);
		/// </pre>
		/// </para>
		/// </summary>
		/// <param name="pt"> the new <code>Point</code> to add to this
		///           <code>Rectangle</code> </param>
		public virtual void Add(Point pt)
		{
			Add(pt.x, pt.y);
		}

		/// <summary>
		/// Adds a <code>Rectangle</code> to this <code>Rectangle</code>.
		/// The resulting <code>Rectangle</code> is the union of the two
		/// rectangles.
		/// <para>
		/// If either {@code Rectangle} has any dimension less than 0, the
		/// result will have the dimensions of the other {@code Rectangle}.
		/// If both {@code Rectangle}s have at least one dimension less
		/// than 0, the result will have at least one dimension less than 0.
		/// </para>
		/// <para>
		/// If either {@code Rectangle} has one or both dimensions equal
		/// to 0, the result along those axes with 0 dimensions will be
		/// equivalent to the results obtained by adding the corresponding
		/// origin coordinate to the result rectangle along that axis,
		/// similar to the operation of the <seealso cref="#add(Point)"/> method,
		/// but contribute no further dimension beyond that.
		/// </para>
		/// <para>
		/// If the resulting {@code Rectangle} would have a dimension
		/// too large to be expressed as an {@code int}, the result
		/// will have a dimension of {@code Integer.MAX_VALUE} along
		/// that dimension.
		/// </para>
		/// </summary>
		/// <param name="r"> the specified <code>Rectangle</code> </param>
		public virtual void Add(Rectangle r)
		{
			long tx2 = this.Width_Renamed;
			long ty2 = this.Height_Renamed;
			if ((tx2 | ty2) < 0)
			{
				Reshape(r.x, r.y, r.Width_Renamed, r.Height_Renamed);
			}
			long rx2 = r.Width_Renamed;
			long ry2 = r.Height_Renamed;
			if ((rx2 | ry2) < 0)
			{
				return;
			}
			int tx1 = this.x;
			int ty1 = this.y;
			tx2 += tx1;
			ty2 += ty1;
			int rx1 = r.x;
			int ry1 = r.y;
			rx2 += rx1;
			ry2 += ry1;
			if (tx1 > rx1)
			{
				tx1 = rx1;
			}
			if (ty1 > ry1)
			{
				ty1 = ry1;
			}
			if (tx2 < rx2)
			{
				tx2 = rx2;
			}
			if (ty2 < ry2)
			{
				ty2 = ry2;
			}
			tx2 -= tx1;
			ty2 -= ty1;
			// tx2,ty2 will never underflow since both original
			// rectangles were non-empty
			// they might overflow, though...
			if (tx2 > Integer.MaxValue)
			{
				tx2 = Integer.MaxValue;
			}
			if (ty2 > Integer.MaxValue)
			{
				ty2 = Integer.MaxValue;
			}
			Reshape(tx1, ty1, (int) tx2, (int) ty2);
		}

		/// <summary>
		/// Resizes the <code>Rectangle</code> both horizontally and vertically.
		/// <para>
		/// This method modifies the <code>Rectangle</code> so that it is
		/// <code>h</code> units larger on both the left and right side,
		/// and <code>v</code> units larger at both the top and bottom.
		/// </para>
		/// <para>
		/// The new <code>Rectangle</code> has {@code (x - h, y - v)}
		/// as its upper-left corner,
		/// width of {@code (width + 2h)},
		/// and a height of {@code (height + 2v)}.
		/// </para>
		/// <para>
		/// If negative values are supplied for <code>h</code> and
		/// <code>v</code>, the size of the <code>Rectangle</code>
		/// decreases accordingly.
		/// The {@code grow} method will check for integer overflow
		/// and underflow, but does not check whether the resulting
		/// values of {@code width} and {@code height} grow
		/// from negative to non-negative or shrink from non-negative
		/// to negative.
		/// </para>
		/// </summary>
		/// <param name="h"> the horizontal expansion </param>
		/// <param name="v"> the vertical expansion </param>
		public virtual void Grow(int h, int v)
		{
			long x0 = this.x;
			long y0 = this.y;
			long x1 = this.Width_Renamed;
			long y1 = this.Height_Renamed;
			x1 += x0;
			y1 += y0;

			x0 -= h;
			y0 -= v;
			x1 += h;
			y1 += v;

			if (x1 < x0)
			{
				// Non-existant in X direction
				// Final width must remain negative so subtract x0 before
				// it is clipped so that we avoid the risk that the clipping
				// of x0 will reverse the ordering of x0 and x1.
				x1 -= x0;
				if (x1 < Integer.MinValue)
				{
					x1 = Integer.MinValue;
				}
				if (x0 < Integer.MinValue)
				{
					x0 = Integer.MinValue;
				}
				else if (x0 > Integer.MaxValue)
				{
					x0 = Integer.MaxValue;
				}
			} // (x1 >= x0)
			else
			{
				// Clip x0 before we subtract it from x1 in case the clipping
				// affects the representable area of the rectangle.
				if (x0 < Integer.MinValue)
				{
					x0 = Integer.MinValue;
				}
				else if (x0 > Integer.MaxValue)
				{
					x0 = Integer.MaxValue;
				}
				x1 -= x0;
				// The only way x1 can be negative now is if we clipped
				// x0 against MIN and x1 is less than MIN - in which case
				// we want to leave the width negative since the result
				// did not intersect the representable area.
				if (x1 < Integer.MinValue)
				{
					x1 = Integer.MinValue;
				}
				else if (x1 > Integer.MaxValue)
				{
					x1 = Integer.MaxValue;
				}
			}

			if (y1 < y0)
			{
				// Non-existant in Y direction
				y1 -= y0;
				if (y1 < Integer.MinValue)
				{
					y1 = Integer.MinValue;
				}
				if (y0 < Integer.MinValue)
				{
					y0 = Integer.MinValue;
				}
				else if (y0 > Integer.MaxValue)
				{
					y0 = Integer.MaxValue;
				}
			} // (y1 >= y0)
			else
			{
				if (y0 < Integer.MinValue)
				{
					y0 = Integer.MinValue;
				}
				else if (y0 > Integer.MaxValue)
				{
					y0 = Integer.MaxValue;
				}
				y1 -= y0;
				if (y1 < Integer.MinValue)
				{
					y1 = Integer.MinValue;
				}
				else if (y1 > Integer.MaxValue)
				{
					y1 = Integer.MaxValue;
				}
			}

			Reshape((int) x0, (int) y0, (int) x1, (int) y1);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 1.2
		/// </summary>
		public override bool Empty
		{
			get
			{
				return (Width_Renamed <= 0) || (Height_Renamed <= 0);
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
			 * x+w or y+h is done in int, then we may get integer
			 * overflow. By converting to double before the addition
			 * we force the addition to be carried out in double to
			 * avoid overflow in the comparison.
			 *
			 * See bug 4320890 for problems that this can cause.
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
		public override Rectangle2D CreateIntersection(Rectangle2D r)
		{
			if (r is Rectangle)
			{
				return Intersection((Rectangle) r);
			}
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
			if (r is Rectangle)
			{
				return Union((Rectangle) r);
			}
			Rectangle2D dest = new Rectangle2D.Double();
			Rectangle2D.Union(this, r, dest);
			return dest;
		}

		/// <summary>
		/// Checks whether two rectangles are equal.
		/// <para>
		/// The result is <code>true</code> if and only if the argument is not
		/// <code>null</code> and is a <code>Rectangle</code> object that has the
		/// same upper-left corner, width, and height as
		/// this <code>Rectangle</code>.
		/// </para>
		/// </summary>
		/// <param name="obj"> the <code>Object</code> to compare with
		///                this <code>Rectangle</code> </param>
		/// <returns>    <code>true</code> if the objects are equal;
		///            <code>false</code> otherwise. </returns>
		public override bool Equals(Object obj)
		{
			if (obj is Rectangle)
			{
				Rectangle r = (Rectangle)obj;
				return ((x == r.x) && (y == r.y) && (Width_Renamed == r.Width_Renamed) && (Height_Renamed == r.Height_Renamed));
			}
			return base.Equals(obj);
		}

		/// <summary>
		/// Returns a <code>String</code> representing this
		/// <code>Rectangle</code> and its values. </summary>
		/// <returns> a <code>String</code> representing this
		///               <code>Rectangle</code> object's coordinate and size values. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[x=" + x + ",y=" + y + ",width=" + Width_Renamed + ",height=" + Height_Renamed + "]";
		}
	}

}