/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The <code>PageFormat</code> class describes the size and
	/// orientation of a page to be printed.
	/// </summary>
	public class PageFormat : Cloneable
	{

	 /* Class Constants */

		/// <summary>
		///  The origin is at the bottom left of the paper with
		///  x running bottom to top and y running left to right.
		///  Note that this is not the Macintosh landscape but
		///  is the Window's and PostScript landscape.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int LANDSCAPE = 0;
		public const int LANDSCAPE = 0;

		/// <summary>
		///  The origin is at the top left of the paper with
		///  x running to the right and y running down the
		///  paper.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int PORTRAIT = 1;
		public const int PORTRAIT = 1;

		/// <summary>
		///  The origin is at the top right of the paper with x
		///  running top to bottom and y running right to left.
		///  Note that this is the Macintosh landscape.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Native public static final int REVERSE_LANDSCAPE = 2;
		public const int REVERSE_LANDSCAPE = 2;

	 /* Instance Variables */

		/// <summary>
		/// A description of the physical piece of paper.
		/// </summary>
		private Paper MPaper;

		/// <summary>
		/// The orientation of the current page. This will be
		/// one of the constants: PORTRIAT, LANDSCAPE, or
		/// REVERSE_LANDSCAPE,
		/// </summary>
		private int MOrientation = PORTRAIT;

	 /* Constructors */

		/// <summary>
		/// Creates a default, portrait-oriented
		/// <code>PageFormat</code>.
		/// </summary>
		public PageFormat()
		{
			MPaper = new Paper();
		}

	 /* Instance Methods */

		/// <summary>
		/// Makes a copy of this <code>PageFormat</code> with the same
		/// contents as this <code>PageFormat</code>. </summary>
		/// <returns> a copy of this <code>PageFormat</code>. </returns>
		public virtual Object Clone()
		{
			PageFormat newPage;

			try
			{
				newPage = (PageFormat) base.Clone();
				newPage.MPaper = (Paper)MPaper.Clone();

			}
			catch (CloneNotSupportedException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				newPage = null; // should never happen.
			}

			return newPage;
		}


		/// <summary>
		/// Returns the width, in 1/72nds of an inch, of the page.
		/// This method takes into account the orientation of the
		/// page when determining the width. </summary>
		/// <returns> the width of the page. </returns>
		public virtual double Width
		{
			get
			{
				double width;
				int orientation = Orientation;
    
				if (orientation == PORTRAIT)
				{
					width = MPaper.Width;
				}
				else
				{
					width = MPaper.Height;
				}
    
				return width;
			}
		}

		/// <summary>
		/// Returns the height, in 1/72nds of an inch, of the page.
		/// This method takes into account the orientation of the
		/// page when determining the height. </summary>
		/// <returns> the height of the page. </returns>
		public virtual double Height
		{
			get
			{
				double height;
				int orientation = Orientation;
    
				if (orientation == PORTRAIT)
				{
					height = MPaper.Height;
				}
				else
				{
					height = MPaper.Width;
				}
    
				return height;
			}
		}

		/// <summary>
		/// Returns the x coordinate of the upper left point of the
		/// imageable area of the <code>Paper</code> object
		/// associated with this <code>PageFormat</code>.
		/// This method takes into account the
		/// orientation of the page. </summary>
		/// <returns> the x coordinate of the upper left point of the
		/// imageable area of the <code>Paper</code> object
		/// associated with this <code>PageFormat</code>. </returns>
		public virtual double ImageableX
		{
			get
			{
				double x;
    
				switch (Orientation)
				{
    
				case LANDSCAPE:
					x = MPaper.Height - (MPaper.ImageableY + MPaper.ImageableHeight);
					break;
    
				case PORTRAIT:
					x = MPaper.ImageableX;
					break;
    
				case REVERSE_LANDSCAPE:
					x = MPaper.ImageableY;
					break;
    
				default:
					/* This should never happen since it signifies that the
					 * PageFormat is in an invalid orientation.
					 */
					throw new InternalError("unrecognized orientation");
    
				}
    
				return x;
			}
		}

		/// <summary>
		/// Returns the y coordinate of the upper left point of the
		/// imageable area of the <code>Paper</code> object
		/// associated with this <code>PageFormat</code>.
		/// This method takes into account the
		/// orientation of the page. </summary>
		/// <returns> the y coordinate of the upper left point of the
		/// imageable area of the <code>Paper</code> object
		/// associated with this <code>PageFormat</code>. </returns>
		public virtual double ImageableY
		{
			get
			{
				double y;
    
				switch (Orientation)
				{
    
				case LANDSCAPE:
					y = MPaper.ImageableX;
					break;
    
				case PORTRAIT:
					y = MPaper.ImageableY;
					break;
    
				case REVERSE_LANDSCAPE:
					y = MPaper.Width - (MPaper.ImageableX + MPaper.ImageableWidth);
					break;
    
				default:
					/* This should never happen since it signifies that the
					 * PageFormat is in an invalid orientation.
					 */
					throw new InternalError("unrecognized orientation");
    
				}
    
				return y;
			}
		}

		/// <summary>
		/// Returns the width, in 1/72nds of an inch, of the imageable
		/// area of the page. This method takes into account the orientation
		/// of the page. </summary>
		/// <returns> the width of the page. </returns>
		public virtual double ImageableWidth
		{
			get
			{
				double width;
    
				if (Orientation == PORTRAIT)
				{
					width = MPaper.ImageableWidth;
				}
				else
				{
					width = MPaper.ImageableHeight;
				}
    
				return width;
			}
		}

		/// <summary>
		/// Return the height, in 1/72nds of an inch, of the imageable
		/// area of the page. This method takes into account the orientation
		/// of the page. </summary>
		/// <returns> the height of the page. </returns>
		public virtual double ImageableHeight
		{
			get
			{
				double height;
    
				if (Orientation == PORTRAIT)
				{
					height = MPaper.ImageableHeight;
				}
				else
				{
					height = MPaper.ImageableWidth;
				}
    
				return height;
			}
		}


		/// <summary>
		/// Returns a copy of the <seealso cref="Paper"/> object associated
		/// with this <code>PageFormat</code>.  Changes made to the
		/// <code>Paper</code> object returned from this method do not
		/// affect the <code>Paper</code> object of this
		/// <code>PageFormat</code>.  To update the <code>Paper</code>
		/// object of this <code>PageFormat</code>, create a new
		/// <code>Paper</code> object and set it into this
		/// <code>PageFormat</code> by using the <seealso cref="#setPaper(Paper)"/>
		/// method. </summary>
		/// <returns> a copy of the <code>Paper</code> object associated
		///          with this <code>PageFormat</code>. </returns>
		/// <seealso cref= #setPaper </seealso>
		public virtual Paper Paper
		{
			get
			{
				return (Paper)MPaper.Clone();
			}
			set
			{
				 MPaper = (Paper)value.Clone();
			}
		}


		/// <summary>
		/// Sets the page orientation. <code>orientation</code> must be
		/// one of the constants: PORTRAIT, LANDSCAPE,
		/// or REVERSE_LANDSCAPE. </summary>
		/// <param name="orientation"> the new orientation for the page </param>
		/// <exception cref="IllegalArgumentException"> if
		///          an unknown orientation was requested </exception>
		/// <seealso cref= #getOrientation </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOrientation(int orientation) throws IllegalArgumentException
		public virtual int Orientation
		{
			set
			{
				if (0 <= value && value <= REVERSE_LANDSCAPE)
				{
					MOrientation = value;
				}
				else
				{
					throw new IllegalArgumentException();
				}
			}
			get
			{
				return MOrientation;
			}
		}


		/// <summary>
		/// Returns a transformation matrix that translates user
		/// space rendering to the requested orientation
		/// of the page.  The values are placed into the
		/// array as
		/// {&nbsp;m00,&nbsp;m10,&nbsp;m01,&nbsp;m11,&nbsp;m02,&nbsp;m12} in
		/// the form required by the <seealso cref="AffineTransform"/>
		/// constructor. </summary>
		/// <returns> the matrix used to translate user space rendering
		/// to the orientation of the page. </returns>
		/// <seealso cref= java.awt.geom.AffineTransform </seealso>
		public virtual double[] Matrix
		{
			get
			{
				double[] matrix = new double[6];
    
				switch (MOrientation)
				{
    
				case LANDSCAPE:
					matrix[0] = 0;
					matrix[1] = -1;
					matrix[2] = 1;
					matrix[3] = 0;
					matrix[4] = 0;
					matrix[5] = MPaper.Height;
					break;
    
				case PORTRAIT:
					matrix[0] = 1;
					matrix[1] = 0;
					matrix[2] = 0;
					matrix[3] = 1;
					matrix[4] = 0;
					matrix[5] = 0;
					break;
    
				case REVERSE_LANDSCAPE:
					matrix[0] = 0;
					matrix[1] = 1;
					matrix[2] = -1;
					matrix[3] = 0;
					matrix[4] = MPaper.Width;
					matrix[5] = 0;
					break;
    
				default:
					throw new IllegalArgumentException();
				}
    
				return matrix;
			}
		}
	}

}