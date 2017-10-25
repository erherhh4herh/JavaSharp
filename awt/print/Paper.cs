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

namespace java.awt.print
{

	/// <summary>
	/// The <code>Paper</code> class describes the physical characteristics of
	/// a piece of paper.
	/// <para>
	/// When creating a <code>Paper</code> object, it is the application's
	/// responsibility to ensure that the paper size and the imageable area
	/// are compatible.  For example, if the paper size is changed from
	/// 11 x 17 to 8.5 x 11, the application might need to reduce the
	/// imageable area so that whatever is printed fits on the page.
	/// </para>
	/// <para>
	/// </para>
	/// </summary>
	/// <seealso cref= #setSize(double, double) </seealso>
	/// <seealso cref= #setImageableArea(double, double, double, double) </seealso>
	public class Paper : Cloneable
	{

	 /* Private Class Variables */

		private const int INCH = 72;
		private static readonly double LETTER_WIDTH = 8.5 * INCH;
		private static readonly double LETTER_HEIGHT = 11 * INCH;

	 /* Instance Variables */

		/// <summary>
		/// The height of the physical page in 1/72nds
		/// of an inch. The number is stored as a floating
		/// point value rather than as an integer
		/// to facilitate the conversion from metric
		/// units to 1/72nds of an inch and then back.
		/// (This may or may not be a good enough reason
		/// for a float).
		/// </summary>
		private double MHeight;

		/// <summary>
		/// The width of the physical page in 1/72nds
		/// of an inch.
		/// </summary>
		private double MWidth;

		/// <summary>
		/// The area of the page on which drawing will
		/// be visable. The area outside of this
		/// rectangle but on the Page generally
		/// reflects the printer's hardware margins.
		/// The origin of the physical page is
		/// at (0, 0) with this rectangle provided
		/// in that coordinate system.
		/// </summary>
		private Rectangle2D MImageableArea;

	 /* Constructors */

		/// <summary>
		/// Creates a letter sized piece of paper
		/// with one inch margins.
		/// </summary>
		public Paper()
		{
			MHeight = LETTER_HEIGHT;
			MWidth = LETTER_WIDTH;
			MImageableArea = new Rectangle2D.Double(INCH, INCH, MWidth - 2 * INCH, MHeight - 2 * INCH);
		}

	 /* Instance Methods */

		/// <summary>
		/// Creates a copy of this <code>Paper</code> with the same contents
		/// as this <code>Paper</code>. </summary>
		/// <returns> a copy of this <code>Paper</code>. </returns>
		public virtual Object Clone()
		{

			Paper newPaper;

			try
			{
				/* It's okay to copy the reference to the imageable
				 * area into the clone since we always return a copy
				 * of the imageable area when asked for it.
				 */
				newPaper = (Paper) base.Clone();

			}
			catch (CloneNotSupportedException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				newPaper = null; // should never happen.
			}

			return newPaper;
		}

		/// <summary>
		/// Returns the height of the page in 1/72nds of an inch. </summary>
		/// <returns> the height of the page described by this
		///          <code>Paper</code>. </returns>
		public virtual double Height
		{
			get
			{
				return MHeight;
			}
		}

		/// <summary>
		/// Sets the width and height of this <code>Paper</code>
		/// object, which represents the properties of the page onto
		/// which printing occurs.
		/// The dimensions are supplied in 1/72nds of
		/// an inch. </summary>
		/// <param name="width"> the value to which to set this <code>Paper</code>
		/// object's width </param>
		/// <param name="height"> the value to which to set this <code>Paper</code>
		/// object's height </param>
		public virtual void SetSize(double width, double height)
		{
			MWidth = width;
			MHeight = height;
		}

		/// <summary>
		/// Returns the width of the page in 1/72nds
		/// of an inch. </summary>
		/// <returns> the width of the page described by this
		/// <code>Paper</code>. </returns>
		public virtual double Width
		{
			get
			{
				return MWidth;
			}
		}

		/// <summary>
		/// Sets the imageable area of this <code>Paper</code>.  The
		/// imageable area is the area on the page in which printing
		/// occurs. </summary>
		/// <param name="x"> the X coordinate to which to set the
		/// upper-left corner of the imageable area of this <code>Paper</code> </param>
		/// <param name="y"> the Y coordinate to which to set the
		/// upper-left corner of the imageable area of this <code>Paper</code> </param>
		/// <param name="width"> the value to which to set the width of the
		/// imageable area of this <code>Paper</code> </param>
		/// <param name="height"> the value to which to set the height of the
		/// imageable area of this <code>Paper</code> </param>
		public virtual void SetImageableArea(double x, double y, double width, double height)
		{
			MImageableArea = new Rectangle2D.Double(x, y, width,height);
		}

		/// <summary>
		/// Returns the x coordinate of the upper-left corner of this
		/// <code>Paper</code> object's imageable area. </summary>
		/// <returns> the x coordinate of the imageable area. </returns>
		public virtual double ImageableX
		{
			get
			{
				return MImageableArea.X;
			}
		}

		/// <summary>
		/// Returns the y coordinate of the upper-left corner of this
		/// <code>Paper</code> object's imageable area. </summary>
		/// <returns> the y coordinate of the imageable area. </returns>
		public virtual double ImageableY
		{
			get
			{
				return MImageableArea.Y;
			}
		}

		/// <summary>
		/// Returns the width of this <code>Paper</code> object's imageable
		/// area. </summary>
		/// <returns> the width of the imageable area. </returns>
		public virtual double ImageableWidth
		{
			get
			{
				return MImageableArea.Width;
			}
		}

		/// <summary>
		/// Returns the height of this <code>Paper</code> object's imageable
		/// area. </summary>
		/// <returns> the height of the imageable area. </returns>
		public virtual double ImageableHeight
		{
			get
			{
				return MImageableArea.Height;
			}
		}
	}

}