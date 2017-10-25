using System;

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
	/// The <code>GridLayout</code> class is a layout manager that
	/// lays out a container's components in a rectangular grid.
	/// The container is divided into equal-sized rectangles,
	/// and one component is placed in each rectangle.
	/// For example, the following is an applet that lays out six buttons
	/// into three rows and two columns:
	/// 
	/// <hr><blockquote>
	/// <pre>
	/// import java.awt.*;
	/// import java.applet.Applet;
	/// public class ButtonGrid extends Applet {
	///     public void init() {
	///         setLayout(new GridLayout(3,2));
	///         add(new Button("1"));
	///         add(new Button("2"));
	///         add(new Button("3"));
	///         add(new Button("4"));
	///         add(new Button("5"));
	///         add(new Button("6"));
	///     }
	/// }
	/// </pre></blockquote><hr>
	/// <para>
	/// If the container's <code>ComponentOrientation</code> property is horizontal
	/// and left-to-right, the above example produces the output shown in Figure 1.
	/// If the container's <code>ComponentOrientation</code> property is horizontal
	/// and right-to-left, the example produces the output shown in Figure 2.
	/// 
	/// <table style="float:center" WIDTH=600 summary="layout">
	/// <tr ALIGN=CENTER>
	/// <td><img SRC="doc-files/GridLayout-1.gif"
	///      alt="Shows 6 buttons in rows of 2. Row 1 shows buttons 1 then 2.
	/// Row 2 shows buttons 3 then 4. Row 3 shows buttons 5 then 6.">
	/// </td>
	/// 
	/// <td ALIGN=CENTER><img SRC="doc-files/GridLayout-2.gif"
	///                   alt="Shows 6 buttons in rows of 2. Row 1 shows buttons 2 then 1.
	/// Row 2 shows buttons 4 then 3. Row 3 shows buttons 6 then 5.">
	/// </td>
	/// </tr>
	/// 
	/// <tr ALIGN=CENTER>
	/// <td>Figure 1: Horizontal, Left-to-Right</td>
	/// 
	/// <td>Figure 2: Horizontal, Right-to-Left</td>
	/// </tr>
	/// </table>
	/// </para>
	/// <para>
	/// When both the number of rows and the number of columns have
	/// been set to non-zero values, either by a constructor or
	/// by the <tt>setRows</tt> and <tt>setColumns</tt> methods, the number of
	/// columns specified is ignored.  Instead, the number of
	/// columns is determined from the specified number of rows
	/// and the total number of components in the layout. So, for
	/// example, if three rows and two columns have been specified
	/// and nine components are added to the layout, they will
	/// be displayed as three rows of three columns.  Specifying
	/// the number of columns affects the layout only when the
	/// number of rows is set to zero.
	/// 
	/// @author  Arthur van Hoff
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	[Serializable]
	public class GridLayout : LayoutManager
	{
		/*
		 * serialVersionUID
		 */
		private const long SerialVersionUID = -7411804673224730901L;

		/// <summary>
		/// This is the horizontal gap (in pixels) which specifies the space
		/// between columns.  They can be changed at any time.
		/// This should be a non-negative integer.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getHgap() </seealso>
		/// <seealso cref= #setHgap(int) </seealso>
		internal int Hgap_Renamed;
		/// <summary>
		/// This is the vertical gap (in pixels) which specifies the space
		/// between rows.  They can be changed at any time.
		/// This should be a non negative integer.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getVgap() </seealso>
		/// <seealso cref= #setVgap(int) </seealso>
		internal int Vgap_Renamed;
		/// <summary>
		/// This is the number of rows specified for the grid.  The number
		/// of rows can be changed at any time.
		/// This should be a non negative integer, where '0' means
		/// 'any number' meaning that the number of Rows in that
		/// dimension depends on the other dimension.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getRows() </seealso>
		/// <seealso cref= #setRows(int) </seealso>
		internal int Rows_Renamed;
		/// <summary>
		/// This is the number of columns specified for the grid.  The number
		/// of columns can be changed at any time.
		/// This should be a non negative integer, where '0' means
		/// 'any number' meaning that the number of Columns in that
		/// dimension depends on the other dimension.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getColumns() </seealso>
		/// <seealso cref= #setColumns(int) </seealso>
		internal int Cols;

		/// <summary>
		/// Creates a grid layout with a default of one column per component,
		/// in a single row.
		/// @since JDK1.1
		/// </summary>
		public GridLayout() : this(1, 0, 0, 0)
		{
		}

		/// <summary>
		/// Creates a grid layout with the specified number of rows and
		/// columns. All components in the layout are given equal size.
		/// <para>
		/// One, but not both, of <code>rows</code> and <code>cols</code> can
		/// be zero, which means that any number of objects can be placed in a
		/// row or in a column.
		/// </para>
		/// </summary>
		/// <param name="rows">   the rows, with the value zero meaning
		///                   any number of rows. </param>
		/// <param name="cols">   the columns, with the value zero meaning
		///                   any number of columns. </param>
		public GridLayout(int rows, int cols) : this(rows, cols, 0, 0)
		{
		}

		/// <summary>
		/// Creates a grid layout with the specified number of rows and
		/// columns. All components in the layout are given equal size.
		/// <para>
		/// In addition, the horizontal and vertical gaps are set to the
		/// specified values. Horizontal gaps are placed between each
		/// of the columns. Vertical gaps are placed between each of
		/// the rows.
		/// </para>
		/// <para>
		/// One, but not both, of <code>rows</code> and <code>cols</code> can
		/// be zero, which means that any number of objects can be placed in a
		/// row or in a column.
		/// </para>
		/// <para>
		/// All <code>GridLayout</code> constructors defer to this one.
		/// </para>
		/// </summary>
		/// <param name="rows">   the rows, with the value zero meaning
		///                   any number of rows </param>
		/// <param name="cols">   the columns, with the value zero meaning
		///                   any number of columns </param>
		/// <param name="hgap">   the horizontal gap </param>
		/// <param name="vgap">   the vertical gap </param>
		/// <exception cref="IllegalArgumentException">  if the value of both
		///                  <code>rows</code> and <code>cols</code> is
		///                  set to zero </exception>
		public GridLayout(int rows, int cols, int hgap, int vgap)
		{
			if ((rows == 0) && (cols == 0))
			{
				throw new IllegalArgumentException("rows and cols cannot both be zero");
			}
			this.Rows_Renamed = rows;
			this.Cols = cols;
			this.Hgap_Renamed = hgap;
			this.Vgap_Renamed = vgap;
		}

		/// <summary>
		/// Gets the number of rows in this layout. </summary>
		/// <returns>    the number of rows in this layout
		/// @since     JDK1.1 </returns>
		public virtual int Rows
		{
			get
			{
				return Rows_Renamed;
			}
			set
			{
				if ((value == 0) && (this.Cols == 0))
				{
					throw new IllegalArgumentException("rows and cols cannot both be zero");
				}
				this.Rows_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the number of columns in this layout. </summary>
		/// <returns>     the number of columns in this layout
		/// @since      JDK1.1 </returns>
		public virtual int Columns
		{
			get
			{
				return Cols;
			}
			set
			{
				if ((value == 0) && (this.Rows_Renamed == 0))
				{
					throw new IllegalArgumentException("rows and cols cannot both be zero");
				}
				this.Cols = value;
			}
		}


		/// <summary>
		/// Gets the horizontal gap between components. </summary>
		/// <returns>       the horizontal gap between components
		/// @since        JDK1.1 </returns>
		public virtual int Hgap
		{
			get
			{
				return Hgap_Renamed;
			}
			set
			{
				this.Hgap_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the vertical gap between components. </summary>
		/// <returns>       the vertical gap between components
		/// @since        JDK1.1 </returns>
		public virtual int Vgap
		{
			get
			{
				return Vgap_Renamed;
			}
			set
			{
				this.Vgap_Renamed = value;
			}
		}


		/// <summary>
		/// Adds the specified component with the specified name to the layout. </summary>
		/// <param name="name"> the name of the component </param>
		/// <param name="comp"> the component to be added </param>
		public virtual void AddLayoutComponent(String name, Component comp)
		{
		}

		/// <summary>
		/// Removes the specified component from the layout. </summary>
		/// <param name="comp"> the component to be removed </param>
		public virtual void RemoveLayoutComponent(Component comp)
		{
		}

		/// <summary>
		/// Determines the preferred size of the container argument using
		/// this grid layout.
		/// <para>
		/// The preferred width of a grid layout is the largest preferred
		/// width of all of the components in the container times the number of
		/// columns, plus the horizontal padding times the number of columns
		/// minus one, plus the left and right insets of the target container.
		/// </para>
		/// <para>
		/// The preferred height of a grid layout is the largest preferred
		/// height of all of the components in the container times the number of
		/// rows, plus the vertical padding times the number of rows minus one,
		/// plus the top and bottom insets of the target container.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">   the container in which to do the layout </param>
		/// <returns>    the preferred dimensions to lay out the
		///                      subcomponents of the specified container </returns>
		/// <seealso cref=       java.awt.GridLayout#minimumLayoutSize </seealso>
		/// <seealso cref=       java.awt.Container#getPreferredSize() </seealso>
		public virtual Dimension PreferredLayoutSize(Container parent)
		{
		  lock (parent.TreeLock)
		  {
			Insets insets = parent.Insets;
			int ncomponents = parent.ComponentCount;
			int nrows = Rows_Renamed;
			int ncols = Cols;

			if (nrows > 0)
			{
				ncols = (ncomponents + nrows - 1) / nrows;
			}
			else
			{
				nrows = (ncomponents + ncols - 1) / ncols;
			}
			int w = 0;
			int h = 0;
			for (int i = 0 ; i < ncomponents ; i++)
			{
				Component comp = parent.GetComponent(i);
				Dimension d = comp.PreferredSize;
				if (w < d.Width_Renamed)
				{
					w = d.Width_Renamed;
				}
				if (h < d.Height_Renamed)
				{
					h = d.Height_Renamed;
				}
			}
			return new Dimension(insets.Left + insets.Right + ncols * w + (ncols - 1) * Hgap_Renamed, insets.Top + insets.Bottom + nrows * h + (nrows - 1) * Vgap_Renamed);
		  }
		}

		/// <summary>
		/// Determines the minimum size of the container argument using this
		/// grid layout.
		/// <para>
		/// The minimum width of a grid layout is the largest minimum width
		/// of all of the components in the container times the number of columns,
		/// plus the horizontal padding times the number of columns minus one,
		/// plus the left and right insets of the target container.
		/// </para>
		/// <para>
		/// The minimum height of a grid layout is the largest minimum height
		/// of all of the components in the container times the number of rows,
		/// plus the vertical padding times the number of rows minus one, plus
		/// the top and bottom insets of the target container.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">   the container in which to do the layout </param>
		/// <returns>      the minimum dimensions needed to lay out the
		///                      subcomponents of the specified container </returns>
		/// <seealso cref=         java.awt.GridLayout#preferredLayoutSize </seealso>
		/// <seealso cref=         java.awt.Container#doLayout </seealso>
		public virtual Dimension MinimumLayoutSize(Container parent)
		{
		  lock (parent.TreeLock)
		  {
			Insets insets = parent.Insets;
			int ncomponents = parent.ComponentCount;
			int nrows = Rows_Renamed;
			int ncols = Cols;

			if (nrows > 0)
			{
				ncols = (ncomponents + nrows - 1) / nrows;
			}
			else
			{
				nrows = (ncomponents + ncols - 1) / ncols;
			}
			int w = 0;
			int h = 0;
			for (int i = 0 ; i < ncomponents ; i++)
			{
				Component comp = parent.GetComponent(i);
				Dimension d = comp.MinimumSize;
				if (w < d.Width_Renamed)
				{
					w = d.Width_Renamed;
				}
				if (h < d.Height_Renamed)
				{
					h = d.Height_Renamed;
				}
			}
			return new Dimension(insets.Left + insets.Right + ncols * w + (ncols - 1) * Hgap_Renamed, insets.Top + insets.Bottom + nrows * h + (nrows - 1) * Vgap_Renamed);
		  }
		}

		/// <summary>
		/// Lays out the specified container using this layout.
		/// <para>
		/// This method reshapes the components in the specified target
		/// container in order to satisfy the constraints of the
		/// <code>GridLayout</code> object.
		/// </para>
		/// <para>
		/// The grid layout manager determines the size of individual
		/// components by dividing the free space in the container into
		/// equal-sized portions according to the number of rows and columns
		/// in the layout. The container's free space equals the container's
		/// size minus any insets and any specified horizontal or vertical
		/// gap. All components in a grid layout are given the same size.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">   the container in which to do the layout </param>
		/// <seealso cref=        java.awt.Container </seealso>
		/// <seealso cref=        java.awt.Container#doLayout </seealso>
		public virtual void LayoutContainer(Container parent)
		{
		  lock (parent.TreeLock)
		  {
			Insets insets = parent.Insets;
			int ncomponents = parent.ComponentCount;
			int nrows = Rows_Renamed;
			int ncols = Cols;
			bool ltr = parent.ComponentOrientation.LeftToRight;

			if (ncomponents == 0)
			{
				return;
			}
			if (nrows > 0)
			{
				ncols = (ncomponents + nrows - 1) / nrows;
			}
			else
			{
				nrows = (ncomponents + ncols - 1) / ncols;
			}
			// 4370316. To position components in the center we should:
			// 1. get an amount of extra space within Container
			// 2. incorporate half of that value to the left/top position
			// Note that we use trancating division for widthOnComponent
			// The reminder goes to extraWidthAvailable
			int totalGapsWidth = (ncols - 1) * Hgap_Renamed;
			int widthWOInsets = parent.Width_Renamed - (insets.Left + insets.Right);
			int widthOnComponent = (widthWOInsets - totalGapsWidth) / ncols;
			int extraWidthAvailable = (widthWOInsets - (widthOnComponent * ncols + totalGapsWidth)) / 2;

			int totalGapsHeight = (nrows - 1) * Vgap_Renamed;
			int heightWOInsets = parent.Height_Renamed - (insets.Top + insets.Bottom);
			int heightOnComponent = (heightWOInsets - totalGapsHeight) / nrows;
			int extraHeightAvailable = (heightWOInsets - (heightOnComponent * nrows + totalGapsHeight)) / 2;
			if (ltr)
			{
				for (int c = 0, x = insets.Left + extraWidthAvailable; c < ncols ; c++, x += widthOnComponent + Hgap_Renamed)
				{
					for (int r = 0, y = insets.Top + extraHeightAvailable; r < nrows ; r++, y += heightOnComponent + Vgap_Renamed)
					{
						int i = r * ncols + c;
						if (i < ncomponents)
						{
							parent.GetComponent(i).SetBounds(x, y, widthOnComponent, heightOnComponent);
						}
					}
				}
			}
			else
			{
				for (int c = 0, x = (parent.Width_Renamed - insets.Right - widthOnComponent) - extraWidthAvailable; c < ncols ; c++, x -= widthOnComponent + Hgap_Renamed)
				{
					for (int r = 0, y = insets.Top + extraHeightAvailable; r < nrows ; r++, y += heightOnComponent + Vgap_Renamed)
					{
						int i = r * ncols + c;
						if (i < ncomponents)
						{
							parent.GetComponent(i).SetBounds(x, y, widthOnComponent, heightOnComponent);
						}
					}
				}
			}
		  }
		}

		/// <summary>
		/// Returns the string representation of this grid layout's values. </summary>
		/// <returns>     a string representation of this grid layout </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[hgap=" + Hgap_Renamed + ",vgap=" + Vgap_Renamed + ",rows=" + Rows_Renamed + ",cols=" + Cols + "]";
		}
	}

}