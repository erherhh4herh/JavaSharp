using System;
using System.Collections.Generic;

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
	/// The <code>GridBagLayout</code> class is a flexible layout
	/// manager that aligns components vertically, horizontally or along their
	/// baseline without requiring that the components be of the same size.
	/// Each <code>GridBagLayout</code> object maintains a dynamic,
	/// rectangular grid of cells, with each component occupying
	/// one or more cells, called its <em>display area</em>.
	/// <para>
	/// Each component managed by a <code>GridBagLayout</code> is associated with
	/// an instance of <seealso cref="GridBagConstraints"/>.  The constraints object
	/// specifies where a component's display area should be located on the grid
	/// and how the component should be positioned within its display area.  In
	/// addition to its constraints object, the <code>GridBagLayout</code> also
	/// considers each component's minimum and preferred sizes in order to
	/// determine a component's size.
	/// </para>
	/// <para>
	/// The overall orientation of the grid depends on the container's
	/// <seealso cref="ComponentOrientation"/> property.  For horizontal left-to-right
	/// orientations, grid coordinate (0,0) is in the upper left corner of the
	/// container with x increasing to the right and y increasing downward.  For
	/// horizontal right-to-left orientations, grid coordinate (0,0) is in the upper
	/// right corner of the container with x increasing to the left and y
	/// increasing downward.
	/// </para>
	/// <para>
	/// To use a grid bag layout effectively, you must customize one or more
	/// of the <code>GridBagConstraints</code> objects that are associated
	/// with its components. You customize a <code>GridBagConstraints</code>
	/// object by setting one or more of its instance variables:
	/// 
	/// <dl>
	/// <dt><seealso cref="GridBagConstraints#gridx"/>,
	/// <seealso cref="GridBagConstraints#gridy"/>
	/// <dd>Specifies the cell containing the leading corner of the component's
	/// display area, where the cell at the origin of the grid has address
	/// <code>gridx&nbsp;=&nbsp;0</code>,
	/// <code>gridy&nbsp;=&nbsp;0</code>.  For horizontal left-to-right layout,
	/// a component's leading corner is its upper left.  For horizontal
	/// right-to-left layout, a component's leading corner is its upper right.
	/// Use <code>GridBagConstraints.RELATIVE</code> (the default value)
	/// to specify that the component be placed immediately following
	/// (along the x axis for <code>gridx</code> or the y axis for
	/// <code>gridy</code>) the component that was added to the container
	/// just before this component was added.
	/// <dt><seealso cref="GridBagConstraints#gridwidth"/>,
	/// <seealso cref="GridBagConstraints#gridheight"/>
	/// <dd>Specifies the number of cells in a row (for <code>gridwidth</code>)
	/// or column (for <code>gridheight</code>)
	/// in the component's display area.
	/// The default value is 1.
	/// Use <code>GridBagConstraints.REMAINDER</code> to specify
	/// that the component's display area will be from <code>gridx</code>
	/// to the last cell in the row (for <code>gridwidth</code>)
	/// or from <code>gridy</code> to the last cell in the column
	/// (for <code>gridheight</code>).
	/// 
	/// Use <code>GridBagConstraints.RELATIVE</code> to specify
	/// that the component's display area will be from <code>gridx</code>
	/// to the next to the last cell in its row (for <code>gridwidth</code>
	/// or from <code>gridy</code> to the next to the last cell in its
	/// column (for <code>gridheight</code>).
	/// 
	/// <dt><seealso cref="GridBagConstraints#fill"/>
	/// <dd>Used when the component's display area
	/// is larger than the component's requested size
	/// to determine whether (and how) to resize the component.
	/// Possible values are
	/// <code>GridBagConstraints.NONE</code> (the default),
	/// <code>GridBagConstraints.HORIZONTAL</code>
	/// (make the component wide enough to fill its display area
	/// horizontally, but don't change its height),
	/// <code>GridBagConstraints.VERTICAL</code>
	/// (make the component tall enough to fill its display area
	/// vertically, but don't change its width), and
	/// <code>GridBagConstraints.BOTH</code>
	/// (make the component fill its display area entirely).
	/// <dt><seealso cref="GridBagConstraints#ipadx"/>,
	/// <seealso cref="GridBagConstraints#ipady"/>
	/// <dd>Specifies the component's internal padding within the layout,
	/// how much to add to the minimum size of the component.
	/// The width of the component will be at least its minimum width
	/// plus <code>ipadx</code> pixels. Similarly, the height of
	/// the component will be at least the minimum height plus
	/// <code>ipady</code> pixels.
	/// <dt><seealso cref="GridBagConstraints#insets"/>
	/// <dd>Specifies the component's external padding, the minimum
	/// amount of space between the component and the edges of its display area.
	/// <dt><seealso cref="GridBagConstraints#anchor"/>
	/// <dd>Specifies where the component should be positioned in its display area.
	/// There are three kinds of possible values: absolute, orientation-relative,
	/// and baseline-relative
	/// Orientation relative values are interpreted relative to the container's
	/// <code>ComponentOrientation</code> property while absolute values
	/// are not.  Baseline relative values are calculated relative to the
	/// baseline.  Valid values are:
	/// 
	/// <center><table BORDER=0 WIDTH=800
	///        SUMMARY="absolute, relative and baseline values as described above">
	/// <tr>
	/// <th><P style="text-align:left">Absolute Values</th>
	/// <th><P style="text-align:left">Orientation Relative Values</th>
	/// <th><P style="text-align:left">Baseline Relative Values</th>
	/// </tr>
	/// <tr>
	/// <td>
	/// <ul style="list-style-type:none">
	/// <li><code>GridBagConstraints.NORTH</code></li>
	/// <li><code>GridBagConstraints.SOUTH</code></li>
	/// <li><code>GridBagConstraints.WEST</code></li>
	/// <li><code>GridBagConstraints.EAST</code></li>
	/// <li><code>GridBagConstraints.NORTHWEST</code></li>
	/// <li><code>GridBagConstraints.NORTHEAST</code></li>
	/// <li><code>GridBagConstraints.SOUTHWEST</code></li>
	/// <li><code>GridBagConstraints.SOUTHEAST</code></li>
	/// <li><code>GridBagConstraints.CENTER</code> (the default)</li>
	/// </ul>
	/// </td>
	/// <td>
	/// <ul style="list-style-type:none">
	/// <li><code>GridBagConstraints.PAGE_START</code></li>
	/// <li><code>GridBagConstraints.PAGE_END</code></li>
	/// <li><code>GridBagConstraints.LINE_START</code></li>
	/// <li><code>GridBagConstraints.LINE_END</code></li>
	/// <li><code>GridBagConstraints.FIRST_LINE_START</code></li>
	/// <li><code>GridBagConstraints.FIRST_LINE_END</code></li>
	/// <li><code>GridBagConstraints.LAST_LINE_START</code></li>
	/// <li><code>GridBagConstraints.LAST_LINE_END</code></li>
	/// </ul>
	/// </td>
	/// <td>
	/// <ul style="list-style-type:none">
	/// <li><code>GridBagConstraints.BASELINE</code></li>
	/// <li><code>GridBagConstraints.BASELINE_LEADING</code></li>
	/// <li><code>GridBagConstraints.BASELINE_TRAILING</code></li>
	/// <li><code>GridBagConstraints.ABOVE_BASELINE</code></li>
	/// <li><code>GridBagConstraints.ABOVE_BASELINE_LEADING</code></li>
	/// <li><code>GridBagConstraints.ABOVE_BASELINE_TRAILING</code></li>
	/// <li><code>GridBagConstraints.BELOW_BASELINE</code></li>
	/// <li><code>GridBagConstraints.BELOW_BASELINE_LEADING</code></li>
	/// <li><code>GridBagConstraints.BELOW_BASELINE_TRAILING</code></li>
	/// </ul>
	/// </td>
	/// </tr>
	/// </table></center>
	/// <dt><seealso cref="GridBagConstraints#weightx"/>,
	/// <seealso cref="GridBagConstraints#weighty"/>
	/// <dd>Used to determine how to distribute space, which is
	/// important for specifying resizing behavior.
	/// Unless you specify a weight for at least one component
	/// in a row (<code>weightx</code>) and column (<code>weighty</code>),
	/// all the components clump together in the center of their container.
	/// This is because when the weight is zero (the default),
	/// the <code>GridBagLayout</code> object puts any extra space
	/// between its grid of cells and the edges of the container.
	/// </dl>
	/// </para>
	/// <para>
	/// Each row may have a baseline; the baseline is determined by the
	/// components in that row that have a valid baseline and are aligned
	/// along the baseline (the component's anchor value is one of {@code
	/// BASELINE}, {@code BASELINE_LEADING} or {@code BASELINE_TRAILING}).
	/// If none of the components in the row has a valid baseline, the row
	/// does not have a baseline.
	/// </para>
	/// <para>
	/// If a component spans rows it is aligned either to the baseline of
	/// the start row (if the baseline-resize behavior is {@code
	/// CONSTANT_ASCENT}) or the end row (if the baseline-resize behavior
	/// is {@code CONSTANT_DESCENT}).  The row that the component is
	/// aligned to is called the <em>prevailing row</em>.
	/// </para>
	/// <para>
	/// The following figure shows a baseline layout and includes a
	/// component that spans rows:
	/// <center><table summary="Baseline Layout">
	/// <tr ALIGN=CENTER>
	/// <td>
	/// <img src="doc-files/GridBagLayout-baseline.png"
	///  alt="The following text describes this graphic (Figure 1)." style="float:center">
	/// </td>
	/// </table></center>
	/// This layout consists of three components:
	/// <ul><li>A panel that starts in row 0 and ends in row 1.  The panel
	///   has a baseline-resize behavior of <code>CONSTANT_DESCENT</code> and has
	///   an anchor of <code>BASELINE</code>.  As the baseline-resize behavior
	///   is <code>CONSTANT_DESCENT</code> the prevailing row for the panel is
	///   row 1.
	/// <li>Two buttons, each with a baseline-resize behavior of
	///   <code>CENTER_OFFSET</code> and an anchor of <code>BASELINE</code>.
	/// </ul>
	/// Because the second button and the panel share the same prevailing row,
	/// they are both aligned along their baseline.
	/// </para>
	/// <para>
	/// Components positioned using one of the baseline-relative values resize
	/// differently than when positioned using an absolute or orientation-relative
	/// value.  How components change is dictated by how the baseline of the
	/// prevailing row changes.  The baseline is anchored to the
	/// bottom of the display area if any components with the same prevailing row
	/// have a baseline-resize behavior of <code>CONSTANT_DESCENT</code>,
	/// otherwise the baseline is anchored to the top of the display area.
	/// The following rules dictate the resize behavior:
	/// <ul>
	/// <li>Resizable components positioned above the baseline can only
	/// grow as tall as the baseline.  For example, if the baseline is at 100
	/// and anchored at the top, a resizable component positioned above the
	/// baseline can never grow more than 100 units.
	/// <li>Similarly, resizable components positioned below the baseline can
	/// only grow as high as the difference between the display height and the
	/// baseline.
	/// <li>Resizable components positioned on the baseline with a
	/// baseline-resize behavior of <code>OTHER</code> are only resized if
	/// the baseline at the resized size fits within the display area.  If
	/// the baseline is such that it does not fit within the display area
	/// the component is not resized.
	/// <li>Components positioned on the baseline that do not have a
	/// baseline-resize behavior of <code>OTHER</code>
	/// can only grow as tall as {@code display height - baseline + baseline of component}.
	/// </ul>
	/// If you position a component along the baseline, but the
	/// component does not have a valid baseline, it will be vertically centered
	/// in its space.  Similarly if you have positioned a component relative
	/// to the baseline and none of the components in the row have a valid
	/// baseline the component is vertically centered.
	/// </para>
	/// <para>
	/// The following figures show ten components (all buttons)
	/// managed by a grid bag layout.  Figure 2 shows the layout for a horizontal,
	/// left-to-right container and Figure 3 shows the layout for a horizontal,
	/// right-to-left container.
	/// 
	/// <center><table WIDTH=600 summary="layout">
	/// <tr ALIGN=CENTER>
	/// <td>
	/// <img src="doc-files/GridBagLayout-1.gif" alt="The preceding text describes this graphic (Figure 1)." style="float:center; margin: 7px 10px;">
	/// </td>
	/// <td>
	/// <img src="doc-files/GridBagLayout-2.gif" alt="The preceding text describes this graphic (Figure 2)." style="float:center; margin: 7px 10px;">
	/// </td>
	/// <tr ALIGN=CENTER>
	/// <td>Figure 2: Horizontal, Left-to-Right</td>
	/// <td>Figure 3: Horizontal, Right-to-Left</td>
	/// </tr>
	/// </table></center>
	/// </para>
	/// <para>
	/// Each of the ten components has the <code>fill</code> field
	/// of its associated <code>GridBagConstraints</code> object
	/// set to <code>GridBagConstraints.BOTH</code>.
	/// In addition, the components have the following non-default constraints:
	/// 
	/// <ul>
	/// <li>Button1, Button2, Button3: <code>weightx&nbsp;=&nbsp;1.0</code>
	/// <li>Button4: <code>weightx&nbsp;=&nbsp;1.0</code>,
	/// <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.REMAINDER</code>
	/// <li>Button5: <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.REMAINDER</code>
	/// <li>Button6: <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.RELATIVE</code>
	/// <li>Button7: <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.REMAINDER</code>
	/// <li>Button8: <code>gridheight&nbsp;=&nbsp;2</code>,
	/// <code>weighty&nbsp;=&nbsp;1.0</code>
	/// <li>Button9, Button 10:
	/// <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.REMAINDER</code>
	/// </ul>
	/// </para>
	/// <para>
	/// Here is the code that implements the example shown above:
	/// 
	/// <hr><blockquote><pre>
	/// import java.awt.*;
	/// import java.util.*;
	/// import java.applet.Applet;
	/// 
	/// public class GridBagEx1 extends Applet {
	/// 
	///     protected void makebutton(String name,
	///                               GridBagLayout gridbag,
	///                               GridBagConstraints c) {
	///         Button button = new Button(name);
	///         gridbag.setConstraints(button, c);
	///         add(button);
	///     }
	/// 
	///     public void init() {
	///         GridBagLayout gridbag = new GridBagLayout();
	///         GridBagConstraints c = new GridBagConstraints();
	/// 
	///         setFont(new Font("SansSerif", Font.PLAIN, 14));
	///         setLayout(gridbag);
	/// 
	///         c.fill = GridBagConstraints.BOTH;
	///         c.weightx = 1.0;
	///         makebutton("Button1", gridbag, c);
	///         makebutton("Button2", gridbag, c);
	///         makebutton("Button3", gridbag, c);
	/// 
	///         c.gridwidth = GridBagConstraints.REMAINDER; //end row
	///         makebutton("Button4", gridbag, c);
	/// 
	///         c.weightx = 0.0;                //reset to the default
	///         makebutton("Button5", gridbag, c); //another row
	/// 
	///         c.gridwidth = GridBagConstraints.RELATIVE; //next-to-last in row
	///         makebutton("Button6", gridbag, c);
	/// 
	///         c.gridwidth = GridBagConstraints.REMAINDER; //end row
	///         makebutton("Button7", gridbag, c);
	/// 
	///         c.gridwidth = 1;                //reset to the default
	///         c.gridheight = 2;
	///         c.weighty = 1.0;
	///         makebutton("Button8", gridbag, c);
	/// 
	///         c.weighty = 0.0;                //reset to the default
	///         c.gridwidth = GridBagConstraints.REMAINDER; //end row
	///         c.gridheight = 1;               //reset to the default
	///         makebutton("Button9", gridbag, c);
	///         makebutton("Button10", gridbag, c);
	/// 
	///         setSize(300, 100);
	///     }
	/// 
	///     public static void main(String args[]) {
	///         Frame f = new Frame("GridBag Layout Example");
	///         GridBagEx1 ex1 = new GridBagEx1();
	/// 
	///         ex1.init();
	/// 
	///         f.add("Center", ex1);
	///         f.pack();
	///         f.setSize(f.getPreferredSize());
	///         f.show();
	///     }
	/// }
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// @author Doug Stein
	/// @author Bill Spitzak (orignial NeWS &amp; OLIT implementation)
	/// </para>
	/// </summary>
	/// <seealso cref=       java.awt.GridBagConstraints </seealso>
	/// <seealso cref=       java.awt.GridBagLayoutInfo </seealso>
	/// <seealso cref=       java.awt.ComponentOrientation
	/// @since JDK1.0 </seealso>
	[Serializable]
	public class GridBagLayout : LayoutManager2
	{

		internal const int EMPIRICMULTIPLIER = 2;
		/// <summary>
		/// This field is no longer used to reserve arrays and kept for backward
		/// compatibility. Previously, this was
		/// the maximum number of grid positions (both horizontal and
		/// vertical) that could be laid out by the grid bag layout.
		/// Current implementation doesn't impose any limits
		/// on the size of a grid.
		/// </summary>
		protected internal const int MAXGRIDSIZE = 512;

		/// <summary>
		/// The smallest grid that can be laid out by the grid bag layout.
		/// </summary>
		protected internal const int MINSIZE = 1;
		/// <summary>
		/// The preferred grid size that can be laid out by the grid bag layout.
		/// </summary>
		protected internal const int PREFERREDSIZE = 2;

		/// <summary>
		/// This hashtable maintains the association between
		/// a component and its gridbag constraints.
		/// The Keys in <code>comptable</code> are the components and the
		/// values are the instances of <code>GridBagConstraints</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= java.awt.GridBagConstraints </seealso>
		protected internal Dictionary<Component, GridBagConstraints> Comptable;

		/// <summary>
		/// This field holds a gridbag constraints instance
		/// containing the default values, so if a component
		/// does not have gridbag constraints associated with
		/// it, then the component will be assigned a
		/// copy of the <code>defaultConstraints</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getConstraints(Component) </seealso>
		/// <seealso cref= #setConstraints(Component, GridBagConstraints) </seealso>
		/// <seealso cref= #lookupConstraints(Component) </seealso>
		protected internal GridBagConstraints DefaultConstraints;

		/// <summary>
		/// This field holds the layout information
		/// for the gridbag.  The information in this field
		/// is based on the most recent validation of the
		/// gridbag.
		/// If <code>layoutInfo</code> is <code>null</code>
		/// this indicates that there are no components in
		/// the gridbag or if there are components, they have
		/// not yet been validated.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLayoutInfo(Container, int) </seealso>
		protected internal GridBagLayoutInfo LayoutInfo;

		/// <summary>
		/// This field holds the overrides to the column minimum
		/// width.  If this field is non-<code>null</code> the values are
		/// applied to the gridbag after all of the minimum columns
		/// widths have been calculated.
		/// If columnWidths has more elements than the number of
		/// columns, columns are added to the gridbag to match
		/// the number of elements in columnWidth.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLayoutDimensions() </seealso>
		public int[] ColumnWidths;

		/// <summary>
		/// This field holds the overrides to the row minimum
		/// heights.  If this field is non-<code>null</code> the values are
		/// applied to the gridbag after all of the minimum row
		/// heights have been calculated.
		/// If <code>rowHeights</code> has more elements than the number of
		/// rows, rows are added to the gridbag to match
		/// the number of elements in <code>rowHeights</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLayoutDimensions() </seealso>
		public int[] RowHeights;

		/// <summary>
		/// This field holds the overrides to the column weights.
		/// If this field is non-<code>null</code> the values are
		/// applied to the gridbag after all of the columns
		/// weights have been calculated.
		/// If <code>columnWeights[i]</code> &gt; weight for column i, then
		/// column i is assigned the weight in <code>columnWeights[i]</code>.
		/// If <code>columnWeights</code> has more elements than the number
		/// of columns, the excess elements are ignored - they do
		/// not cause more columns to be created.
		/// 
		/// @serial
		/// </summary>
		public double[] ColumnWeights;

		/// <summary>
		/// This field holds the overrides to the row weights.
		/// If this field is non-<code>null</code> the values are
		/// applied to the gridbag after all of the rows
		/// weights have been calculated.
		/// If <code>rowWeights[i]</code> &gt; weight for row i, then
		/// row i is assigned the weight in <code>rowWeights[i]</code>.
		/// If <code>rowWeights</code> has more elements than the number
		/// of rows, the excess elements are ignored - they do
		/// not cause more rows to be created.
		/// 
		/// @serial
		/// </summary>
		public double[] RowWeights;

		/// <summary>
		/// The component being positioned.  This is set before calling into
		/// <code>adjustForGravity</code>.
		/// </summary>
		private Component ComponentAdjusting;

		/// <summary>
		/// Creates a grid bag layout manager.
		/// </summary>
		public GridBagLayout()
		{
			Comptable = new Dictionary<Component, GridBagConstraints>();
			DefaultConstraints = new GridBagConstraints();
		}

		/// <summary>
		/// Sets the constraints for the specified component in this layout. </summary>
		/// <param name="comp"> the component to be modified </param>
		/// <param name="constraints"> the constraints to be applied </param>
		public virtual void SetConstraints(Component comp, GridBagConstraints constraints)
		{
			Comptable[comp] = (GridBagConstraints)constraints.Clone();
		}

		/// <summary>
		/// Gets the constraints for the specified component.  A copy of
		/// the actual <code>GridBagConstraints</code> object is returned. </summary>
		/// <param name="comp"> the component to be queried </param>
		/// <returns>      the constraint for the specified component in this
		///                  grid bag layout; a copy of the actual constraint
		///                  object is returned </returns>
		public virtual GridBagConstraints GetConstraints(Component comp)
		{
			GridBagConstraints constraints = Comptable[comp];
			if (constraints == null)
			{
				SetConstraints(comp, DefaultConstraints);
				constraints = Comptable[comp];
			}
			return (GridBagConstraints)constraints.Clone();
		}

		/// <summary>
		/// Retrieves the constraints for the specified component.
		/// The return value is not a copy, but is the actual
		/// <code>GridBagConstraints</code> object used by the layout mechanism.
		/// <para>
		/// If <code>comp</code> is not in the <code>GridBagLayout</code>,
		/// a set of default <code>GridBagConstraints</code> are returned.
		/// A <code>comp</code> value of <code>null</code> is invalid
		/// and returns <code>null</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="comp"> the component to be queried </param>
		/// <returns>      the constraints for the specified component </returns>
		protected internal virtual GridBagConstraints LookupConstraints(Component comp)
		{
			GridBagConstraints constraints = Comptable[comp];
			if (constraints == null)
			{
				SetConstraints(comp, DefaultConstraints);
				constraints = Comptable[comp];
			}
			return constraints;
		}

		/// <summary>
		/// Removes the constraints for the specified component in this layout </summary>
		/// <param name="comp"> the component to be modified </param>
		private void RemoveConstraints(Component comp)
		{
			Comptable.Remove(comp);
		}

		/// <summary>
		/// Determines the origin of the layout area, in the graphics coordinate
		/// space of the target container.  This value represents the pixel
		/// coordinates of the top-left corner of the layout area regardless of
		/// the <code>ComponentOrientation</code> value of the container.  This
		/// is distinct from the grid origin given by the cell coordinates (0,0).
		/// Most applications do not call this method directly. </summary>
		/// <returns>     the graphics origin of the cell in the top-left
		///             corner of the layout grid </returns>
		/// <seealso cref=        java.awt.ComponentOrientation
		/// @since      JDK1.1 </seealso>
		public virtual Point LayoutOrigin
		{
			get
			{
				Point origin = new Point(0,0);
				if (LayoutInfo != null)
				{
					origin.x = LayoutInfo.Startx;
					origin.y = LayoutInfo.Starty;
				}
				return origin;
			}
		}

		/// <summary>
		/// Determines column widths and row heights for the layout grid.
		/// <para>
		/// Most applications do not call this method directly.
		/// </para>
		/// </summary>
		/// <returns>     an array of two arrays, containing the widths
		///                       of the layout columns and
		///                       the heights of the layout rows
		/// @since      JDK1.1 </returns>
		public virtual int [][] LayoutDimensions
		{
			get
			{
				if (LayoutInfo == null)
				{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: return new int[2][0];
					return RectangularArrays.ReturnRectangularIntArray(2, 0);
				}
    
				int[][] dim = new int [2][];
				dim[0] = new int[LayoutInfo.Width];
				dim[1] = new int[LayoutInfo.Height];
    
				System.Array.Copy(LayoutInfo.MinWidth, 0, dim[0], 0, LayoutInfo.Width);
				System.Array.Copy(LayoutInfo.MinHeight, 0, dim[1], 0, LayoutInfo.Height);
    
				return dim;
			}
		}

		/// <summary>
		/// Determines the weights of the layout grid's columns and rows.
		/// Weights are used to calculate how much a given column or row
		/// stretches beyond its preferred size, if the layout has extra
		/// room to fill.
		/// <para>
		/// Most applications do not call this method directly.
		/// </para>
		/// </summary>
		/// <returns>      an array of two arrays, representing the
		///                    horizontal weights of the layout columns
		///                    and the vertical weights of the layout rows
		/// @since       JDK1.1 </returns>
		public virtual double [][] LayoutWeights
		{
			get
			{
				if (LayoutInfo == null)
				{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: return new double[2][0];
					return RectangularArrays.ReturnRectangularDoubleArray(2, 0);
				}
    
				double[][] weights = new double [2][];
				weights[0] = new double[LayoutInfo.Width];
				weights[1] = new double[LayoutInfo.Height];
    
				System.Array.Copy(LayoutInfo.WeightX, 0, weights[0], 0, LayoutInfo.Width);
				System.Array.Copy(LayoutInfo.WeightY, 0, weights[1], 0, LayoutInfo.Height);
    
				return weights;
			}
		}

		/// <summary>
		/// Determines which cell in the layout grid contains the point
		/// specified by <code>(x,&nbsp;y)</code>. Each cell is identified
		/// by its column index (ranging from 0 to the number of columns
		/// minus 1) and its row index (ranging from 0 to the number of
		/// rows minus 1).
		/// <para>
		/// If the <code>(x,&nbsp;y)</code> point lies
		/// outside the grid, the following rules are used.
		/// The column index is returned as zero if <code>x</code> lies to the
		/// left of the layout for a left-to-right container or to the right of
		/// the layout for a right-to-left container.  The column index is returned
		/// as the number of columns if <code>x</code> lies
		/// to the right of the layout in a left-to-right container or to the left
		/// in a right-to-left container.
		/// The row index is returned as zero if <code>y</code> lies above the
		/// layout, and as the number of rows if <code>y</code> lies
		/// below the layout.  The orientation of a container is determined by its
		/// <code>ComponentOrientation</code> property.
		/// </para>
		/// </summary>
		/// <param name="x">    the <i>x</i> coordinate of a point </param>
		/// <param name="y">    the <i>y</i> coordinate of a point </param>
		/// <returns>     an ordered pair of indexes that indicate which cell
		///             in the layout grid contains the point
		///             (<i>x</i>,&nbsp;<i>y</i>). </returns>
		/// <seealso cref=        java.awt.ComponentOrientation
		/// @since      JDK1.1 </seealso>
		public virtual Point Location(int x, int y)
		{
			Point loc = new Point(0,0);
			int i, d;

			if (LayoutInfo == null)
			{
				return loc;
			}

			d = LayoutInfo.Startx;
			if (!RightToLeft)
			{
				for (i = 0; i < LayoutInfo.Width; i++)
				{
					d += LayoutInfo.MinWidth[i];
					if (d > x)
					{
						break;
					}
				}
			}
			else
			{
				for (i = LayoutInfo.Width - 1; i >= 0; i--)
				{
					if (d > x)
					{
						break;
					}
					d += LayoutInfo.MinWidth[i];
				}
				i++;
			}
			loc.x = i;

			d = LayoutInfo.Starty;
			for (i = 0; i < LayoutInfo.Height; i++)
			{
				d += LayoutInfo.MinHeight[i];
				if (d > y)
				{
					break;
				}
			}
			loc.y = i;

			return loc;
		}

		/// <summary>
		/// Has no effect, since this layout manager does not use a per-component string.
		/// </summary>
		public virtual void AddLayoutComponent(String name, Component comp)
		{
		}

		/// <summary>
		/// Adds the specified component to the layout, using the specified
		/// <code>constraints</code> object.  Note that constraints
		/// are mutable and are, therefore, cloned when cached.
		/// </summary>
		/// <param name="comp">         the component to be added </param>
		/// <param name="constraints">  an object that determines how
		///                          the component is added to the layout </param>
		/// <exception cref="IllegalArgumentException"> if <code>constraints</code>
		///            is not a <code>GridBagConstraint</code> </exception>
		public virtual void AddLayoutComponent(Component comp, Object constraints)
		{
			if (constraints is GridBagConstraints)
			{
				SetConstraints(comp, (GridBagConstraints)constraints);
			}
			else if (constraints != null)
			{
				throw new IllegalArgumentException("cannot add to layout: constraints must be a GridBagConstraint");
			}
		}

		/// <summary>
		/// Removes the specified component from this layout.
		/// <para>
		/// Most applications do not call this method directly.
		/// </para>
		/// </summary>
		/// <param name="comp">   the component to be removed. </param>
		/// <seealso cref=      java.awt.Container#remove(java.awt.Component) </seealso>
		/// <seealso cref=      java.awt.Container#removeAll() </seealso>
		public virtual void RemoveLayoutComponent(Component comp)
		{
			RemoveConstraints(comp);
		}

		/// <summary>
		/// Determines the preferred size of the <code>parent</code>
		/// container using this grid bag layout.
		/// <para>
		/// Most applications do not call this method directly.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">   the container in which to do the layout </param>
		/// <seealso cref=       java.awt.Container#getPreferredSize </seealso>
		/// <returns> the preferred size of the <code>parent</code>
		///  container </returns>
		public virtual Dimension PreferredLayoutSize(Container parent)
		{
			GridBagLayoutInfo info = GetLayoutInfo(parent, PREFERREDSIZE);
			return GetMinSize(parent, info);
		}

		/// <summary>
		/// Determines the minimum size of the <code>parent</code> container
		/// using this grid bag layout.
		/// <para>
		/// Most applications do not call this method directly.
		/// </para>
		/// </summary>
		/// <param name="parent">   the container in which to do the layout </param>
		/// <seealso cref=       java.awt.Container#doLayout </seealso>
		/// <returns> the minimum size of the <code>parent</code> container </returns>
		public virtual Dimension MinimumLayoutSize(Container parent)
		{
			GridBagLayoutInfo info = GetLayoutInfo(parent, MINSIZE);
			return GetMinSize(parent, info);
		}

		/// <summary>
		/// Returns the maximum dimensions for this layout given the components
		/// in the specified target container. </summary>
		/// <param name="target"> the container which needs to be laid out </param>
		/// <seealso cref= Container </seealso>
		/// <seealso cref= #minimumLayoutSize(Container) </seealso>
		/// <seealso cref= #preferredLayoutSize(Container) </seealso>
		/// <returns> the maximum dimensions for this layout </returns>
		public virtual Dimension MaximumLayoutSize(Container target)
		{
			return new Dimension(Integer.MaxValue, Integer.MaxValue);
		}

		/// <summary>
		/// Returns the alignment along the x axis.  This specifies how
		/// the component would like to be aligned relative to other
		/// components.  The value should be a number between 0 and 1
		/// where 0 represents alignment along the origin, 1 is aligned
		/// the furthest away from the origin, 0.5 is centered, etc.
		/// <para>
		/// </para>
		/// </summary>
		/// <returns> the value <code>0.5f</code> to indicate centered </returns>
		public virtual float GetLayoutAlignmentX(Container parent)
		{
			return 0.5f;
		}

		/// <summary>
		/// Returns the alignment along the y axis.  This specifies how
		/// the component would like to be aligned relative to other
		/// components.  The value should be a number between 0 and 1
		/// where 0 represents alignment along the origin, 1 is aligned
		/// the furthest away from the origin, 0.5 is centered, etc.
		/// <para>
		/// </para>
		/// </summary>
		/// <returns> the value <code>0.5f</code> to indicate centered </returns>
		public virtual float GetLayoutAlignmentY(Container parent)
		{
			return 0.5f;
		}

		/// <summary>
		/// Invalidates the layout, indicating that if the layout manager
		/// has cached information it should be discarded.
		/// </summary>
		public virtual void InvalidateLayout(Container target)
		{
		}

		/// <summary>
		/// Lays out the specified container using this grid bag layout.
		/// This method reshapes components in the specified container in
		/// order to satisfy the constraints of this <code>GridBagLayout</code>
		/// object.
		/// <para>
		/// Most applications do not call this method directly.
		/// </para>
		/// </summary>
		/// <param name="parent"> the container in which to do the layout </param>
		/// <seealso cref= java.awt.Container </seealso>
		/// <seealso cref= java.awt.Container#doLayout </seealso>
		public virtual void LayoutContainer(Container parent)
		{
			ArrangeGrid(parent);
		}

		/// <summary>
		/// Returns a string representation of this grid bag layout's values. </summary>
		/// <returns>     a string representation of this grid bag layout. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName;
		}

		/// <summary>
		/// Print the layout information.  Useful for debugging.
		/// </summary>

		/* DEBUG
		 *
		 *  protected void dumpLayoutInfo(GridBagLayoutInfo s) {
		 *    int x;
		 *
		 *    System.out.println("Col\tWidth\tWeight");
		 *    for (x=0; x<s.width; x++) {
		 *      System.out.println(x + "\t" +
		 *                   s.minWidth[x] + "\t" +
		 *                   s.weightX[x]);
		 *    }
		 *    System.out.println("Row\tHeight\tWeight");
		 *    for (x=0; x<s.height; x++) {
		 *      System.out.println(x + "\t" +
		 *                   s.minHeight[x] + "\t" +
		 *                   s.weightY[x]);
		 *    }
		 *  }
		 */

		/// <summary>
		/// Print the layout constraints.  Useful for debugging.
		/// </summary>

		/* DEBUG
		 *
		 *  protected void dumpConstraints(GridBagConstraints constraints) {
		 *    System.out.println(
		 *                 "wt " +
		 *                 constraints.weightx +
		 *                 " " +
		 *                 constraints.weighty +
		 *                 ", " +
		 *
		 *                 "box " +
		 *                 constraints.gridx +
		 *                 " " +
		 *                 constraints.gridy +
		 *                 " " +
		 *                 constraints.gridwidth +
		 *                 " " +
		 *                 constraints.gridheight +
		 *                 ", " +
		 *
		 *                 "min " +
		 *                 constraints.minWidth +
		 *                 " " +
		 *                 constraints.minHeight +
		 *                 ", " +
		 *
		 *                 "pad " +
		 *                 constraints.insets.bottom +
		 *                 " " +
		 *                 constraints.insets.left +
		 *                 " " +
		 *                 constraints.insets.right +
		 *                 " " +
		 *                 constraints.insets.top +
		 *                 " " +
		 *                 constraints.ipadx +
		 *                 " " +
		 *                 constraints.ipady);
		 *  }
		 */

		/// <summary>
		/// Fills in an instance of <code>GridBagLayoutInfo</code> for the
		/// current set of managed children. This requires three passes through the
		/// set of children:
		/// 
		/// <ol>
		/// <li>Figure out the dimensions of the layout grid.
		/// <li>Determine which cells the components occupy.
		/// <li>Distribute the weights and min sizes among the rows/columns.
		/// </ol>
		/// 
		/// This also caches the minsizes for all the children when they are
		/// first encountered (so subsequent loops don't need to ask again).
		/// <para>
		/// This method should only be used internally by
		/// <code>GridBagLayout</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">  the layout container </param>
		/// <param name="sizeflag"> either <code>PREFERREDSIZE</code> or
		///   <code>MINSIZE</code> </param>
		/// <returns> the <code>GridBagLayoutInfo</code> for the set of children
		/// @since 1.4 </returns>
		protected internal virtual GridBagLayoutInfo GetLayoutInfo(Container parent, int sizeflag)
		{
			return GetLayoutInfo(parent, sizeflag);
		}

		/*
		 * Calculate maximum array sizes to allocate arrays without ensureCapacity
		 * we may use preCalculated sizes in whole class because of upper estimation of
		 * maximumArrayXIndex and maximumArrayYIndex.
		 */

		private long[] PreInitMaximumArraySizes(Container parent)
		{
			Component[] components = parent.Components;
			Component comp;
			GridBagConstraints constraints;
			int curX, curY;
			int curWidth, curHeight;
			int preMaximumArrayXIndex = 0;
			int preMaximumArrayYIndex = 0;
			long[] returnArray = new long[2];

			for (int compId = 0 ; compId < components.Length ; compId++)
			{
				comp = components[compId];
				if (!comp.Visible)
				{
					continue;
				}

				constraints = LookupConstraints(comp);
				curX = constraints.Gridx;
				curY = constraints.Gridy;
				curWidth = constraints.Gridwidth;
				curHeight = constraints.Gridheight;

				// -1==RELATIVE, means that column|row equals to previously added component,
				// since each next Component with gridx|gridy == RELATIVE starts from
				// previous position, so we should start from previous component which
				// already used in maximumArray[X|Y]Index calculation. We could just increase
				// maximum by 1 to handle situation when component with gridx=-1 was added.
				if (curX < 0)
				{
					curX = ++preMaximumArrayYIndex;
				}
				if (curY < 0)
				{
					curY = ++preMaximumArrayXIndex;
				}
				// gridwidth|gridheight may be equal to RELATIVE (-1) or REMAINDER (0)
				// in any case using 1 instead of 0 or -1 should be sufficient to for
				// correct maximumArraySizes calculation
				if (curWidth <= 0)
				{
					curWidth = 1;
				}
				if (curHeight <= 0)
				{
					curHeight = 1;
				}

				preMaximumArrayXIndex = System.Math.Max(curY + curHeight, preMaximumArrayXIndex);
				preMaximumArrayYIndex = System.Math.Max(curX + curWidth, preMaximumArrayYIndex);
			} //for (components) loop
			// Must specify index++ to allocate well-working arrays.
			/* fix for 4623196.
			 * now return long array instead of Point
			 */
			returnArray[0] = preMaximumArrayXIndex;
			returnArray[1] = preMaximumArrayYIndex;
			return returnArray;
		} //PreInitMaximumSizes

		/// <summary>
		/// This method is obsolete and supplied for backwards
		/// compatibility only; new code should call {@link
		/// #getLayoutInfo(java.awt.Container, int) getLayoutInfo} instead.
		/// This method is the same as <code>getLayoutInfo</code>;
		/// refer to <code>getLayoutInfo</code> for details on parameters
		/// and return value.
		/// </summary>
		protected internal virtual GridBagLayoutInfo GetLayoutInfo(Container parent, int sizeflag)
		{
			lock (parent.TreeLock)
			{
				GridBagLayoutInfo r;
				Component comp;
				GridBagConstraints constraints;
				Dimension d;
				Component[] components = parent.Components;
				// Code below will address index curX+curWidth in the case of yMaxArray, weightY
				// ( respectively curY+curHeight for xMaxArray, weightX ) where
				//  curX in 0 to preInitMaximumArraySizes.y
				// Thus, the maximum index that could
				// be calculated in the following code is curX+curX.
				// EmpericMultier equals 2 because of this.

				int layoutWidth, layoutHeight;
				int[] xMaxArray;
				int[] yMaxArray;
				int compindex, i, k, px, py, pixels_diff, nextSize;
				int curX = 0; // constraints.gridx
				int curY = 0; // constraints.gridy
				int curWidth = 1; // constraints.gridwidth
				int curHeight = 1; // constraints.gridheight
				int curRow, curCol;
				double weight_diff, weight;
				int maximumArrayXIndex = 0;
				int maximumArrayYIndex = 0;
				int anchor;

				/*
				 * Pass #1
				 *
				 * Figure out the dimensions of the layout grid (use a value of 1 for
				 * zero or negative widths and heights).
				 */

				layoutWidth = layoutHeight = 0;
				curRow = curCol = -1;
				long[] arraySizes = PreInitMaximumArraySizes(parent);

				/* fix for 4623196.
				 * If user try to create a very big grid we can
				 * get NegativeArraySizeException because of integer value
				 * overflow (EMPIRICMULTIPLIER*gridSize might be more then Integer.MAX_VALUE).
				 * We need to detect this situation and try to create a
				 * grid with Integer.MAX_VALUE size instead.
				 */
				maximumArrayXIndex = (EMPIRICMULTIPLIER * arraySizes[0] > Integer.MaxValue)? Integer.MaxValue : EMPIRICMULTIPLIER * (int)arraySizes[0];
				maximumArrayYIndex = (EMPIRICMULTIPLIER * arraySizes[1] > Integer.MaxValue)? Integer.MaxValue : EMPIRICMULTIPLIER * (int)arraySizes[1];

				if (RowHeights != null)
				{
					maximumArrayXIndex = System.Math.Max(maximumArrayXIndex, RowHeights.Length);
				}
				if (ColumnWidths != null)
				{
					maximumArrayYIndex = System.Math.Max(maximumArrayYIndex, ColumnWidths.Length);
				}

				xMaxArray = new int[maximumArrayXIndex];
				yMaxArray = new int[maximumArrayYIndex];

				bool hasBaseline = false;
				for (compindex = 0 ; compindex < components.Length ; compindex++)
				{
					comp = components[compindex];
					if (!comp.Visible)
					{
						continue;
					}
					constraints = LookupConstraints(comp);

					curX = constraints.Gridx;
					curY = constraints.Gridy;
					curWidth = constraints.Gridwidth;
					if (curWidth <= 0)
					{
						curWidth = 1;
					}
					curHeight = constraints.Gridheight;
					if (curHeight <= 0)
					{
						curHeight = 1;
					}

					/* If x or y is negative, then use relative positioning: */
					if (curX < 0 && curY < 0)
					{
						if (curRow >= 0)
						{
							curY = curRow;
						}
						else if (curCol >= 0)
						{
							curX = curCol;
						}
						else
						{
							curY = 0;
						}
					}
					if (curX < 0)
					{
						px = 0;
						for (i = curY; i < (curY + curHeight); i++)
						{
							px = System.Math.Max(px, xMaxArray[i]);
						}

						curX = px - curX - 1;
						if (curX < 0)
						{
							curX = 0;
						}
					}
					else if (curY < 0)
					{
						py = 0;
						for (i = curX; i < (curX + curWidth); i++)
						{
							py = System.Math.Max(py, yMaxArray[i]);
						}
						curY = py - curY - 1;
						if (curY < 0)
						{
							curY = 0;
						}
					}

					/* Adjust the grid width and height
					 *  fix for 5005945: unneccessary loops removed
					 */
					px = curX + curWidth;
					if (layoutWidth < px)
					{
						layoutWidth = px;
					}
					py = curY + curHeight;
					if (layoutHeight < py)
					{
						layoutHeight = py;
					}

					/* Adjust xMaxArray and yMaxArray */
					for (i = curX; i < (curX + curWidth); i++)
					{
						yMaxArray[i] = py;
					}
					for (i = curY; i < (curY + curHeight); i++)
					{
						xMaxArray[i] = px;
					}


					/* Cache the current slave's size. */
					if (sizeflag == PREFERREDSIZE)
					{
						d = comp.PreferredSize;
					}
					else
					{
						d = comp.MinimumSize;
					}
					constraints.MinWidth = d.Width_Renamed;
					constraints.MinHeight = d.Height_Renamed;
					if (CalculateBaseline(comp, constraints, d))
					{
						hasBaseline = true;
					}

					/* Zero width and height must mean that this is the last item (or
					 * else something is wrong). */
					if (constraints.Gridheight == 0 && constraints.Gridwidth == 0)
					{
						curRow = curCol = -1;
					}

					/* Zero width starts a new row */
					if (constraints.Gridheight == 0 && curRow < 0)
					{
						curCol = curX + curWidth;
					}

					/* Zero height starts a new column */
					else if (constraints.Gridwidth == 0 && curCol < 0)
					{
						curRow = curY + curHeight;
					}
				} //for (components) loop


				/*
				 * Apply minimum row/column dimensions
				 */
				if (ColumnWidths != null && layoutWidth < ColumnWidths.Length)
				{
					layoutWidth = ColumnWidths.Length;
				}
				if (RowHeights != null && layoutHeight < RowHeights.Length)
				{
					layoutHeight = RowHeights.Length;
				}

				r = new GridBagLayoutInfo(layoutWidth, layoutHeight);

				/*
				 * Pass #2
				 *
				 * Negative values for gridX are filled in with the current x value.
				 * Negative values for gridY are filled in with the current y value.
				 * Negative or zero values for gridWidth and gridHeight end the current
				 *  row or column, respectively.
				 */

				curRow = curCol = -1;

				Arrays.Fill(xMaxArray, 0);
				Arrays.Fill(yMaxArray, 0);

				int[] maxAscent = null;
				int[] maxDescent = null;
				short[] baselineType = null;

				if (hasBaseline)
				{
					r.MaxAscent = maxAscent = new int[layoutHeight];
					r.MaxDescent = maxDescent = new int[layoutHeight];
					r.BaselineType = baselineType = new short[layoutHeight];
					r.HasBaseline_Renamed = true;
				}


				for (compindex = 0 ; compindex < components.Length ; compindex++)
				{
					comp = components[compindex];
					if (!comp.Visible)
					{
						continue;
					}
					constraints = LookupConstraints(comp);

					curX = constraints.Gridx;
					curY = constraints.Gridy;
					curWidth = constraints.Gridwidth;
					curHeight = constraints.Gridheight;

					/* If x or y is negative, then use relative positioning: */
					if (curX < 0 && curY < 0)
					{
						if (curRow >= 0)
						{
							curY = curRow;
						}
						else if (curCol >= 0)
						{
							curX = curCol;
						}
						else
						{
							curY = 0;
						}
					}

					if (curX < 0)
					{
						if (curHeight <= 0)
						{
							curHeight += r.Height - curY;
							if (curHeight < 1)
							{
								curHeight = 1;
							}
						}

						px = 0;
						for (i = curY; i < (curY + curHeight); i++)
						{
							px = System.Math.Max(px, xMaxArray[i]);
						}

						curX = px - curX - 1;
						if (curX < 0)
						{
							curX = 0;
						}
					}
					else if (curY < 0)
					{
						if (curWidth <= 0)
						{
							curWidth += r.Width - curX;
							if (curWidth < 1)
							{
								curWidth = 1;
							}
						}

						py = 0;
						for (i = curX; i < (curX + curWidth); i++)
						{
							py = System.Math.Max(py, yMaxArray[i]);
						}

						curY = py - curY - 1;
						if (curY < 0)
						{
							curY = 0;
						}
					}

					if (curWidth <= 0)
					{
						curWidth += r.Width - curX;
						if (curWidth < 1)
						{
							curWidth = 1;
						}
					}

					if (curHeight <= 0)
					{
						curHeight += r.Height - curY;
						if (curHeight < 1)
						{
							curHeight = 1;
						}
					}

					px = curX + curWidth;
					py = curY + curHeight;

					for (i = curX; i < (curX + curWidth); i++)
					{
						yMaxArray[i] = py;
					}
					for (i = curY; i < (curY + curHeight); i++)
					{
						xMaxArray[i] = px;
					}

					/* Make negative sizes start a new row/column */
					if (constraints.Gridheight == 0 && constraints.Gridwidth == 0)
					{
						curRow = curCol = -1;
					}
					if (constraints.Gridheight == 0 && curRow < 0)
					{
						curCol = curX + curWidth;
					}
					else if (constraints.Gridwidth == 0 && curCol < 0)
					{
						curRow = curY + curHeight;
					}

					/* Assign the new values to the gridbag slave */
					constraints.TempX = curX;
					constraints.TempY = curY;
					constraints.TempWidth = curWidth;
					constraints.TempHeight = curHeight;

					anchor = constraints.Anchor;
					if (hasBaseline)
					{
						switch (anchor)
						{
						case GridBagConstraints.BASELINE:
						case GridBagConstraints.BASELINE_LEADING:
						case GridBagConstraints.BASELINE_TRAILING:
							if (constraints.Ascent >= 0)
							{
								if (curHeight == 1)
								{
									maxAscent[curY] = System.Math.Max(maxAscent[curY], constraints.Ascent);
									maxDescent[curY] = System.Math.Max(maxDescent[curY], constraints.Descent);
								}
								else
								{
									if (constraints.BaselineResizeBehavior == Component.BaselineResizeBehavior.CONSTANT_DESCENT)
									{
										maxDescent[curY + curHeight - 1] = System.Math.Max(maxDescent[curY + curHeight - 1], constraints.Descent);
									}
									else
									{
										maxAscent[curY] = System.Math.Max(maxAscent[curY], constraints.Ascent);
									}
								}
								if (constraints.BaselineResizeBehavior == Component.BaselineResizeBehavior.CONSTANT_DESCENT)
								{
									baselineType[curY + curHeight - 1] |= (short)(1 << constraints.BaselineResizeBehavior.ordinal());
								}
								else
								{
									baselineType[curY] |= (short)(1 << constraints.BaselineResizeBehavior.ordinal());
								}
							}
							break;
						case GridBagConstraints.ABOVE_BASELINE:
						case GridBagConstraints.ABOVE_BASELINE_LEADING:
						case GridBagConstraints.ABOVE_BASELINE_TRAILING:
							// Component positioned above the baseline.
							// To make the bottom edge of the component aligned
							// with the baseline the bottom inset is
							// added to the descent, the rest to the ascent.
							pixels_diff = constraints.MinHeight + constraints.Insets.Top + constraints.Ipady;
							maxAscent[curY] = System.Math.Max(maxAscent[curY], pixels_diff);
							maxDescent[curY] = System.Math.Max(maxDescent[curY], constraints.Insets.Bottom);
							break;
						case GridBagConstraints.BELOW_BASELINE:
						case GridBagConstraints.BELOW_BASELINE_LEADING:
						case GridBagConstraints.BELOW_BASELINE_TRAILING:
							// Component positioned below the baseline.
							// To make the top edge of the component aligned
							// with the baseline the top inset is
							// added to the ascent, the rest to the descent.
							pixels_diff = constraints.MinHeight + constraints.Insets.Bottom + constraints.Ipady;
							maxDescent[curY] = System.Math.Max(maxDescent[curY], pixels_diff);
							maxAscent[curY] = System.Math.Max(maxAscent[curY], constraints.Insets.Top);
							break;
						}
					}
				}

				r.WeightX = new double[maximumArrayYIndex];
				r.WeightY = new double[maximumArrayXIndex];
				r.MinWidth = new int[maximumArrayYIndex];
				r.MinHeight = new int[maximumArrayXIndex];


				/*
				 * Apply minimum row/column dimensions and weights
				 */
				if (ColumnWidths != null)
				{
					System.Array.Copy(ColumnWidths, 0, r.MinWidth, 0, ColumnWidths.Length);
				}
				if (RowHeights != null)
				{
					System.Array.Copy(RowHeights, 0, r.MinHeight, 0, RowHeights.Length);
				}
				if (ColumnWeights != null)
				{
					System.Array.Copy(ColumnWeights, 0, r.WeightX, 0, System.Math.Min(r.WeightX.Length, ColumnWeights.Length));
				}
				if (RowWeights != null)
				{
					System.Array.Copy(RowWeights, 0, r.WeightY, 0, System.Math.Min(r.WeightY.Length, RowWeights.Length));
				}

				/*
				 * Pass #3
				 *
				 * Distribute the minimun widths and weights:
				 */

				nextSize = Integer.MaxValue;

				for (i = 1; i != Integer.MaxValue; i = nextSize, nextSize = Integer.MaxValue)
				{
					for (compindex = 0 ; compindex < components.Length ; compindex++)
					{
						comp = components[compindex];
						if (!comp.Visible)
						{
							continue;
						}
						constraints = LookupConstraints(comp);

						if (constraints.TempWidth == i)
						{
							px = constraints.TempX + constraints.TempWidth; // right column

							/*
							 * Figure out if we should use this slave\'s weight.  If the weight
							 * is less than the total weight spanned by the width of the cell,
							 * then discard the weight.  Otherwise split the difference
							 * according to the existing weights.
							 */

							weight_diff = constraints.Weightx;
							for (k = constraints.TempX; k < px; k++)
							{
								weight_diff -= r.WeightX[k];
							}
							if (weight_diff > 0.0)
							{
								weight = 0.0;
								for (k = constraints.TempX; k < px; k++)
								{
									weight += r.WeightX[k];
								}
								for (k = constraints.TempX; weight > 0.0 && k < px; k++)
								{
									double wt = r.WeightX[k];
									double dx = (wt * weight_diff) / weight;
									r.WeightX[k] += dx;
									weight_diff -= dx;
									weight -= wt;
								}
								/* Assign the remainder to the rightmost cell */
								r.WeightX[px - 1] += weight_diff;
							}

							/*
							 * Calculate the minWidth array values.
							 * First, figure out how wide the current slave needs to be.
							 * Then, see if it will fit within the current minWidth values.
							 * If it will not fit, add the difference according to the
							 * weightX array.
							 */

							pixels_diff = constraints.MinWidth + constraints.Ipadx + constraints.Insets.Left + constraints.Insets.Right;

							for (k = constraints.TempX; k < px; k++)
							{
								pixels_diff -= r.MinWidth[k];
							}
							if (pixels_diff > 0)
							{
								weight = 0.0;
								for (k = constraints.TempX; k < px; k++)
								{
									weight += r.WeightX[k];
								}
								for (k = constraints.TempX; weight > 0.0 && k < px; k++)
								{
									double wt = r.WeightX[k];
									int dx = (int)((wt * ((double)pixels_diff)) / weight);
									r.MinWidth[k] += dx;
									pixels_diff -= dx;
									weight -= wt;
								}
								/* Any leftovers go into the rightmost cell */
								r.MinWidth[px - 1] += pixels_diff;
							}
						}
						else if (constraints.TempWidth > i && constraints.TempWidth < nextSize)
						{
							nextSize = constraints.TempWidth;
						}


						if (constraints.TempHeight == i)
						{
							py = constraints.TempY + constraints.TempHeight; // bottom row

							/*
							 * Figure out if we should use this slave's weight.  If the weight
							 * is less than the total weight spanned by the height of the cell,
							 * then discard the weight.  Otherwise split it the difference
							 * according to the existing weights.
							 */

							weight_diff = constraints.Weighty;
							for (k = constraints.TempY; k < py; k++)
							{
								weight_diff -= r.WeightY[k];
							}
							if (weight_diff > 0.0)
							{
								weight = 0.0;
								for (k = constraints.TempY; k < py; k++)
								{
									weight += r.WeightY[k];
								}
								for (k = constraints.TempY; weight > 0.0 && k < py; k++)
								{
									double wt = r.WeightY[k];
									double dy = (wt * weight_diff) / weight;
									r.WeightY[k] += dy;
									weight_diff -= dy;
									weight -= wt;
								}
								/* Assign the remainder to the bottom cell */
								r.WeightY[py - 1] += weight_diff;
							}

							/*
							 * Calculate the minHeight array values.
							 * First, figure out how tall the current slave needs to be.
							 * Then, see if it will fit within the current minHeight values.
							 * If it will not fit, add the difference according to the
							 * weightY array.
							 */

							pixels_diff = -1;
							if (hasBaseline)
							{
								switch (constraints.Anchor)
								{
								case GridBagConstraints.BASELINE:
								case GridBagConstraints.BASELINE_LEADING:
								case GridBagConstraints.BASELINE_TRAILING:
									if (constraints.Ascent >= 0)
									{
										if (constraints.TempHeight == 1)
										{
											pixels_diff = maxAscent[constraints.TempY] + maxDescent[constraints.TempY];
										}
										else if (constraints.BaselineResizeBehavior != Component.BaselineResizeBehavior.CONSTANT_DESCENT)
										{
											pixels_diff = maxAscent[constraints.TempY] + constraints.Descent;
										}
										else
										{
											pixels_diff = constraints.Ascent + maxDescent[constraints.TempY + constraints.TempHeight - 1];
										}
									}
									break;
								case GridBagConstraints.ABOVE_BASELINE:
								case GridBagConstraints.ABOVE_BASELINE_LEADING:
								case GridBagConstraints.ABOVE_BASELINE_TRAILING:
									pixels_diff = constraints.Insets.Top + constraints.MinHeight + constraints.Ipady + maxDescent[constraints.TempY];
									break;
								case GridBagConstraints.BELOW_BASELINE:
								case GridBagConstraints.BELOW_BASELINE_LEADING:
								case GridBagConstraints.BELOW_BASELINE_TRAILING:
									pixels_diff = maxAscent[constraints.TempY] + constraints.MinHeight + constraints.Insets.Bottom + constraints.Ipady;
									break;
								}
							}
							if (pixels_diff == -1)
							{
								pixels_diff = constraints.MinHeight + constraints.Ipady + constraints.Insets.Top + constraints.Insets.Bottom;
							}
							for (k = constraints.TempY; k < py; k++)
							{
								pixels_diff -= r.MinHeight[k];
							}
							if (pixels_diff > 0)
							{
								weight = 0.0;
								for (k = constraints.TempY; k < py; k++)
								{
									weight += r.WeightY[k];
								}
								for (k = constraints.TempY; weight > 0.0 && k < py; k++)
								{
									double wt = r.WeightY[k];
									int dy = (int)((wt * ((double)pixels_diff)) / weight);
									r.MinHeight[k] += dy;
									pixels_diff -= dy;
									weight -= wt;
								}
								/* Any leftovers go into the bottom cell */
								r.MinHeight[py - 1] += pixels_diff;
							}
						}
						else if (constraints.TempHeight > i && constraints.TempHeight < nextSize)
						{
							nextSize = constraints.TempHeight;
						}
					}
				}
				return r;
			}
		} //getLayoutInfo()

		/// <summary>
		/// Calculate the baseline for the specified component.
		/// If {@code c} is positioned along it's baseline, the baseline is
		/// obtained and the {@code constraints} ascent, descent and
		/// baseline resize behavior are set from the component; and true is
		/// returned. Otherwise false is returned.
		/// </summary>
		private bool CalculateBaseline(Component c, GridBagConstraints constraints, Dimension size)
		{
			int anchor = constraints.Anchor;
			if (anchor == GridBagConstraints.BASELINE || anchor == GridBagConstraints.BASELINE_LEADING || anchor == GridBagConstraints.BASELINE_TRAILING)
			{
				// Apply the padding to the component, then ask for the baseline.
				int w = size.Width_Renamed + constraints.Ipadx;
				int h = size.Height_Renamed + constraints.Ipady;
				constraints.Ascent = c.GetBaseline(w, h);
				if (constraints.Ascent >= 0)
				{
					// Component has a baseline
					int baseline = constraints.Ascent;
					// Adjust the ascent and descent to include the insets.
					constraints.Descent = h - constraints.Ascent + constraints.Insets.Bottom;
					constraints.Ascent += constraints.Insets.Top;
					constraints.BaselineResizeBehavior = c.BaselineResizeBehavior;
					constraints.CenterPadding = 0;
					if (constraints.BaselineResizeBehavior == Component.BaselineResizeBehavior.CENTER_OFFSET)
					{
						// Component has a baseline resize behavior of
						// CENTER_OFFSET, calculate centerPadding and
						// centerOffset (see the description of
						// CENTER_OFFSET in the enum for detais on this
						// algorithm).
						int nextBaseline = c.GetBaseline(w, h + 1);
						constraints.CenterOffset = baseline - h / 2;
						if (h % 2 == 0)
						{
							if (baseline != nextBaseline)
							{
								constraints.CenterPadding = 1;
							}
						}
						else if (baseline == nextBaseline)
						{
							constraints.CenterOffset--;
							constraints.CenterPadding = 1;
						}
					}
				}
				return true;
			}
			else
			{
				constraints.Ascent = -1;
				return false;
			}
		}

		/// <summary>
		/// Adjusts the x, y, width, and height fields to the correct
		/// values depending on the constraint geometry and pads.
		/// This method should only be used internally by
		/// <code>GridBagLayout</code>.
		/// </summary>
		/// <param name="constraints"> the constraints to be applied </param>
		/// <param name="r"> the <code>Rectangle</code> to be adjusted
		/// @since 1.4 </param>
		protected internal virtual void AdjustForGravity(GridBagConstraints constraints, Rectangle r)
		{
			AdjustForGravity(constraints, r);
		}

		/// <summary>
		/// This method is obsolete and supplied for backwards
		/// compatibility only; new code should call {@link
		/// #adjustForGravity(java.awt.GridBagConstraints, java.awt.Rectangle)
		/// adjustForGravity} instead.
		/// This method is the same as <code>adjustForGravity</code>;
		/// refer to <code>adjustForGravity</code> for details
		/// on parameters.
		/// </summary>
		protected internal virtual void AdjustForGravity(GridBagConstraints constraints, Rectangle r)
		{
			int diffx, diffy;
			int cellY = r.y;
			int cellHeight = r.Height_Renamed;

			if (!RightToLeft)
			{
				r.x += constraints.Insets.Left;
			}
			else
			{
				r.x -= r.Width_Renamed - constraints.Insets.Right;
			}
			r.Width_Renamed -= (constraints.Insets.Left + constraints.Insets.Right);
			r.y += constraints.Insets.Top;
			r.Height_Renamed -= (constraints.Insets.Top + constraints.Insets.Bottom);

			diffx = 0;
			if ((constraints.Fill != GridBagConstraints.HORIZONTAL && constraints.Fill != GridBagConstraints.BOTH) && (r.Width_Renamed > (constraints.MinWidth + constraints.Ipadx)))
			{
				diffx = r.Width_Renamed - (constraints.MinWidth + constraints.Ipadx);
				r.Width_Renamed = constraints.MinWidth + constraints.Ipadx;
			}

			diffy = 0;
			if ((constraints.Fill != GridBagConstraints.VERTICAL && constraints.Fill != GridBagConstraints.BOTH) && (r.Height_Renamed > (constraints.MinHeight + constraints.Ipady)))
			{
				diffy = r.Height_Renamed - (constraints.MinHeight + constraints.Ipady);
				r.Height_Renamed = constraints.MinHeight + constraints.Ipady;
			}

			switch (constraints.Anchor)
			{
			  case GridBagConstraints.BASELINE:
				  r.x += diffx / 2;
				  AlignOnBaseline(constraints, r, cellY, cellHeight);
				  break;
			  case GridBagConstraints.BASELINE_LEADING:
				  if (RightToLeft)
				  {
					  r.x += diffx;
				  }
				  AlignOnBaseline(constraints, r, cellY, cellHeight);
				  break;
			  case GridBagConstraints.BASELINE_TRAILING:
				  if (!RightToLeft)
				  {
					  r.x += diffx;
				  }
				  AlignOnBaseline(constraints, r, cellY, cellHeight);
				  break;
			  case GridBagConstraints.ABOVE_BASELINE:
				  r.x += diffx / 2;
				  AlignAboveBaseline(constraints, r, cellY, cellHeight);
				  break;
			  case GridBagConstraints.ABOVE_BASELINE_LEADING:
				  if (RightToLeft)
				  {
					  r.x += diffx;
				  }
				  AlignAboveBaseline(constraints, r, cellY, cellHeight);
				  break;
			  case GridBagConstraints.ABOVE_BASELINE_TRAILING:
				  if (!RightToLeft)
				  {
					  r.x += diffx;
				  }
				  AlignAboveBaseline(constraints, r, cellY, cellHeight);
				  break;
			  case GridBagConstraints.BELOW_BASELINE:
				  r.x += diffx / 2;
				  AlignBelowBaseline(constraints, r, cellY, cellHeight);
				  break;
			  case GridBagConstraints.BELOW_BASELINE_LEADING:
				  if (RightToLeft)
				  {
					  r.x += diffx;
				  }
				  AlignBelowBaseline(constraints, r, cellY, cellHeight);
				  break;
			  case GridBagConstraints.BELOW_BASELINE_TRAILING:
				  if (!RightToLeft)
				  {
					  r.x += diffx;
				  }
				  AlignBelowBaseline(constraints, r, cellY, cellHeight);
				  break;
			  case GridBagConstraints.CENTER:
				  r.x += diffx / 2;
				  r.y += diffy / 2;
				  break;
			  case GridBagConstraints.PAGE_START:
			  case GridBagConstraints.NORTH:
				  r.x += diffx / 2;
				  break;
			  case GridBagConstraints.NORTHEAST:
				  r.x += diffx;
				  break;
			  case GridBagConstraints.EAST:
				  r.x += diffx;
				  r.y += diffy / 2;
				  break;
			  case GridBagConstraints.SOUTHEAST:
				  r.x += diffx;
				  r.y += diffy;
				  break;
			  case GridBagConstraints.PAGE_END:
			  case GridBagConstraints.SOUTH:
				  r.x += diffx / 2;
				  r.y += diffy;
				  break;
			  case GridBagConstraints.SOUTHWEST:
				  r.y += diffy;
				  break;
			  case GridBagConstraints.WEST:
				  r.y += diffy / 2;
				  break;
			  case GridBagConstraints.NORTHWEST:
				  break;
			  case GridBagConstraints.LINE_START:
				  if (RightToLeft)
				  {
					  r.x += diffx;
				  }
				  r.y += diffy / 2;
				  break;
			  case GridBagConstraints.LINE_END:
				  if (!RightToLeft)
				  {
					  r.x += diffx;
				  }
				  r.y += diffy / 2;
				  break;
			  case GridBagConstraints.FIRST_LINE_START:
				  if (RightToLeft)
				  {
					  r.x += diffx;
				  }
				  break;
			  case GridBagConstraints.FIRST_LINE_END:
				  if (!RightToLeft)
				  {
					  r.x += diffx;
				  }
				  break;
			  case GridBagConstraints.LAST_LINE_START:
				  if (RightToLeft)
				  {
					  r.x += diffx;
				  }
				  r.y += diffy;
				  break;
			  case GridBagConstraints.LAST_LINE_END:
				  if (!RightToLeft)
				  {
					  r.x += diffx;
				  }
				  r.y += diffy;
				  break;
			  default:
				  throw new IllegalArgumentException("illegal anchor value");
			}
		}

		/// <summary>
		/// Positions on the baseline.
		/// </summary>
		/// <param name="cellY"> the location of the row, does not include insets </param>
		/// <param name="cellHeight"> the height of the row, does not take into account
		///        insets </param>
		/// <param name="r"> available bounds for the component, is padded by insets and
		///        ipady </param>
		private void AlignOnBaseline(GridBagConstraints cons, Rectangle r, int cellY, int cellHeight)
		{
			if (cons.Ascent >= 0)
			{
				if (cons.BaselineResizeBehavior == Component.BaselineResizeBehavior.CONSTANT_DESCENT)
				{
					// Anchor to the bottom.
					// Baseline is at (cellY + cellHeight - maxDescent).
					// Bottom of component (maxY) is at baseline + descent
					// of component. We need to subtract the bottom inset here
					// as the descent in the constraints object includes the
					// bottom inset.
					int maxY = cellY + cellHeight - LayoutInfo.MaxDescent[cons.TempY + cons.TempHeight - 1] + cons.Descent - cons.Insets.Bottom;
					if (!cons.VerticallyResizable)
					{
						// Component not resizable, calculate y location
						// from maxY - height.
						r.y = maxY - cons.MinHeight;
						r.Height_Renamed = cons.MinHeight;
					}
					else
					{
						// Component is resizable. As brb is constant descent,
						// can expand component to fill region above baseline.
						// Subtract out the top inset so that components insets
						// are honored.
						r.Height_Renamed = maxY - cellY - cons.Insets.Top;
					}
				}
				else
				{
					// BRB is not constant_descent
					int baseline; // baseline for the row, relative to cellY
					// Component baseline, includes insets.top
					int ascent = cons.Ascent;
					if (LayoutInfo.HasConstantDescent(cons.TempY))
					{
						// Mixed ascent/descent in same row, calculate position
						// off maxDescent
						baseline = cellHeight - LayoutInfo.MaxDescent[cons.TempY];
					}
					else
					{
						// Only ascents/unknown in this row, anchor to top
						baseline = LayoutInfo.MaxAscent[cons.TempY];
					}
					if (cons.BaselineResizeBehavior == Component.BaselineResizeBehavior.OTHER)
					{
						// BRB is other, which means we can only determine
						// the baseline by asking for it again giving the
						// size we plan on using for the component.
						bool fits = false;
						ascent = ComponentAdjusting.GetBaseline(r.Width_Renamed, r.Height_Renamed);
						if (ascent >= 0)
						{
							// Component has a baseline, pad with top inset
							// (this follows from calculateBaseline which
							// does the same).
							ascent += cons.Insets.Top;
						}
						if (ascent >= 0 && ascent <= baseline)
						{
							// Components baseline fits within rows baseline.
							// Make sure the descent fits within the space as well.
							if (baseline + (r.Height_Renamed - ascent - cons.Insets.Top) <= cellHeight - cons.Insets.Bottom)
							{
								// It fits, we're good.
								fits = true;
							}
							else if (cons.VerticallyResizable)
							{
								// Doesn't fit, but it's resizable.  Try
								// again assuming we'll get ascent again.
								int ascent2 = ComponentAdjusting.GetBaseline(r.Width_Renamed, cellHeight - cons.Insets.Bottom - baseline + ascent);
								if (ascent2 >= 0)
								{
									ascent2 += cons.Insets.Top;
								}
								if (ascent2 >= 0 && ascent2 <= ascent)
								{
									// It'll fit
									r.Height_Renamed = cellHeight - cons.Insets.Bottom - baseline + ascent;
									ascent = ascent2;
									fits = true;
								}
							}
						}
						if (!fits)
						{
							// Doesn't fit, use min size and original ascent
							ascent = cons.Ascent;
							r.Width_Renamed = cons.MinWidth;
							r.Height_Renamed = cons.MinHeight;
						}
					}
					// Reset the components y location based on
					// components ascent and baseline for row. Because ascent
					// includes the baseline
					r.y = cellY + baseline - ascent + cons.Insets.Top;
					if (cons.VerticallyResizable)
					{
						switch (cons.BaselineResizeBehavior)
						{
						case java.awt.Component.BaselineResizeBehavior.CONSTANT_ASCENT:
							r.Height_Renamed = System.Math.Max(cons.MinHeight,cellY + cellHeight - r.y - cons.Insets.Bottom);
							break;
						case java.awt.Component.BaselineResizeBehavior.CENTER_OFFSET:
						{
								int upper = r.y - cellY - cons.Insets.Top;
								int lower = cellY + cellHeight - r.y - cons.MinHeight - cons.Insets.Bottom;
								int delta = System.Math.Min(upper, lower);
								delta += delta;
								if (delta > 0 && (cons.MinHeight + cons.CenterPadding + delta) / 2 + cons.CenterOffset != baseline)
								{
									// Off by 1
									delta--;
								}
								r.Height_Renamed = cons.MinHeight + delta;
								r.y = cellY + baseline - (r.Height_Renamed + cons.CenterPadding) / 2 - cons.CenterOffset;
						}
							break;
						case java.awt.Component.BaselineResizeBehavior.OTHER:
							// Handled above
							break;
						default:
							break;
						}
					}
				}
			}
			else
			{
				CenterVertically(cons, r, cellHeight);
			}
		}

		/// <summary>
		/// Positions the specified component above the baseline. That is
		/// the bottom edge of the component will be aligned along the baseline.
		/// If the row does not have a baseline, this centers the component.
		/// </summary>
		private void AlignAboveBaseline(GridBagConstraints cons, Rectangle r, int cellY, int cellHeight)
		{
			if (LayoutInfo.HasBaseline(cons.TempY))
			{
				int maxY; // Baseline for the row
				if (LayoutInfo.HasConstantDescent(cons.TempY))
				{
					// Prefer descent
					maxY = cellY + cellHeight - LayoutInfo.MaxDescent[cons.TempY];
				}
				else
				{
					// Prefer ascent
					maxY = cellY + LayoutInfo.MaxAscent[cons.TempY];
				}
				if (cons.VerticallyResizable)
				{
					// Component is resizable. Top edge is offset by top
					// inset, bottom edge on baseline.
					r.y = cellY + cons.Insets.Top;
					r.Height_Renamed = maxY - r.y;
				}
				else
				{
					// Not resizable.
					r.Height_Renamed = cons.MinHeight + cons.Ipady;
					r.y = maxY - r.Height_Renamed;
				}
			}
			else
			{
				CenterVertically(cons, r, cellHeight);
			}
		}

		/// <summary>
		/// Positions below the baseline.
		/// </summary>
		private void AlignBelowBaseline(GridBagConstraints cons, Rectangle r, int cellY, int cellHeight)
		{
			if (LayoutInfo.HasBaseline(cons.TempY))
			{
				if (LayoutInfo.HasConstantDescent(cons.TempY))
				{
					// Prefer descent
					r.y = cellY + cellHeight - LayoutInfo.MaxDescent[cons.TempY];
				}
				else
				{
					// Prefer ascent
					r.y = cellY + LayoutInfo.MaxAscent[cons.TempY];
				}
				if (cons.VerticallyResizable)
				{
					r.Height_Renamed = cellY + cellHeight - r.y - cons.Insets.Bottom;
				}
			}
			else
			{
				CenterVertically(cons, r, cellHeight);
			}
		}

		private void CenterVertically(GridBagConstraints cons, Rectangle r, int cellHeight)
		{
			if (!cons.VerticallyResizable)
			{
				r.y += System.Math.Max(0, (cellHeight - cons.Insets.Top - cons.Insets.Bottom - cons.MinHeight - cons.Ipady) / 2);
			}
		}

		/// <summary>
		/// Figures out the minimum size of the
		/// master based on the information from <code>getLayoutInfo</code>.
		/// This method should only be used internally by
		/// <code>GridBagLayout</code>.
		/// </summary>
		/// <param name="parent"> the layout container </param>
		/// <param name="info"> the layout info for this parent </param>
		/// <returns> a <code>Dimension</code> object containing the
		///   minimum size
		/// @since 1.4 </returns>
		protected internal virtual Dimension GetMinSize(Container parent, GridBagLayoutInfo info)
		{
			return GetMinSize(parent, info);
		}

		/// <summary>
		/// This method is obsolete and supplied for backwards
		/// compatibility only; new code should call {@link
		/// #getMinSize(java.awt.Container, GridBagLayoutInfo) getMinSize} instead.
		/// This method is the same as <code>getMinSize</code>;
		/// refer to <code>getMinSize</code> for details on parameters
		/// and return value.
		/// </summary>
		protected internal virtual Dimension GetMinSize(Container parent, GridBagLayoutInfo info)
		{
			Dimension d = new Dimension();
			int i, t;
			Insets insets = parent.Insets;

			t = 0;
			for (i = 0; i < info.Width; i++)
			{
				t += info.MinWidth[i];
			}
			d.Width_Renamed = t + insets.Left + insets.Right;

			t = 0;
			for (i = 0; i < info.Height; i++)
			{
				t += info.MinHeight[i];
			}
			d.Height_Renamed = t + insets.Top + insets.Bottom;

			return d;
		}

		[NonSerialized]
		internal bool RightToLeft = false;

		/// <summary>
		/// Lays out the grid.
		/// This method should only be used internally by
		/// <code>GridBagLayout</code>.
		/// </summary>
		/// <param name="parent"> the layout container
		/// @since 1.4 </param>
		protected internal virtual void ArrangeGrid(Container parent)
		{
			ArrangeGrid(parent);
		}

		/// <summary>
		/// This method is obsolete and supplied for backwards
		/// compatibility only; new code should call {@link
		/// #arrangeGrid(Container) arrangeGrid} instead.
		/// This method is the same as <code>arrangeGrid</code>;
		/// refer to <code>arrangeGrid</code> for details on the
		/// parameter.
		/// </summary>
		protected internal virtual void ArrangeGrid(Container parent)
		{
			Component comp;
			int compindex;
			GridBagConstraints constraints;
			Insets insets = parent.Insets;
			Component[] components = parent.Components;
			Dimension d;
			Rectangle r = new Rectangle();
			int i, diffw, diffh;
			double weight;
			GridBagLayoutInfo info;

			RightToLeft = !parent.ComponentOrientation.LeftToRight;

			/*
			 * If the parent has no slaves anymore, then don't do anything
			 * at all:  just leave the parent's size as-is.
			 */
			if (components.Length == 0 && (ColumnWidths == null || ColumnWidths.Length == 0) && (RowHeights == null || RowHeights.Length == 0))
			{
				return;
			}

			/*
			 * Pass #1: scan all the slaves to figure out the total amount
			 * of space needed.
			 */

			info = GetLayoutInfo(parent, PREFERREDSIZE);
			d = GetMinSize(parent, info);

			if (parent.Width_Renamed < d.Width_Renamed || parent.Height_Renamed < d.Height_Renamed)
			{
				info = GetLayoutInfo(parent, MINSIZE);
				d = GetMinSize(parent, info);
			}

			LayoutInfo = info;
			r.Width_Renamed = d.Width_Renamed;
			r.Height_Renamed = d.Height_Renamed;

			/*
			 * DEBUG
			 *
			 * DumpLayoutInfo(info);
			 * for (compindex = 0 ; compindex < components.length ; compindex++) {
			 * comp = components[compindex];
			 * if (!comp.isVisible())
			 *      continue;
			 * constraints = lookupConstraints(comp);
			 * DumpConstraints(constraints);
			 * }
			 * System.out.println("minSize " + r.width + " " + r.height);
			 */

			/*
			 * If the current dimensions of the window don't match the desired
			 * dimensions, then adjust the minWidth and minHeight arrays
			 * according to the weights.
			 */

			diffw = parent.Width_Renamed - r.Width_Renamed;
			if (diffw != 0)
			{
				weight = 0.0;
				for (i = 0; i < info.Width; i++)
				{
					weight += info.WeightX[i];
				}
				if (weight > 0.0)
				{
					for (i = 0; i < info.Width; i++)
					{
						int dx = (int)((((double)diffw) * info.WeightX[i]) / weight);
						info.MinWidth[i] += dx;
						r.Width_Renamed += dx;
						if (info.MinWidth[i] < 0)
						{
							r.Width_Renamed -= info.MinWidth[i];
							info.MinWidth[i] = 0;
						}
					}
				}
				diffw = parent.Width_Renamed - r.Width_Renamed;
			}

			else
			{
				diffw = 0;
			}

			diffh = parent.Height_Renamed - r.Height_Renamed;
			if (diffh != 0)
			{
				weight = 0.0;
				for (i = 0; i < info.Height; i++)
				{
					weight += info.WeightY[i];
				}
				if (weight > 0.0)
				{
					for (i = 0; i < info.Height; i++)
					{
						int dy = (int)((((double)diffh) * info.WeightY[i]) / weight);
						info.MinHeight[i] += dy;
						r.Height_Renamed += dy;
						if (info.MinHeight[i] < 0)
						{
							r.Height_Renamed -= info.MinHeight[i];
							info.MinHeight[i] = 0;
						}
					}
				}
				diffh = parent.Height_Renamed - r.Height_Renamed;
			}

			else
			{
				diffh = 0;
			}

			/*
			 * DEBUG
			 *
			 * System.out.println("Re-adjusted:");
			 * DumpLayoutInfo(info);
			 */

			/*
			 * Now do the actual layout of the slaves using the layout information
			 * that has been collected.
			 */

			info.Startx = diffw / 2 + insets.Left;
			info.Starty = diffh / 2 + insets.Top;

			for (compindex = 0 ; compindex < components.Length ; compindex++)
			{
				comp = components[compindex];
				if (!comp.Visible)
				{
					continue;
				}
				constraints = LookupConstraints(comp);

				if (!RightToLeft)
				{
					r.x = info.Startx;
					for (i = 0; i < constraints.TempX; i++)
					{
						r.x += info.MinWidth[i];
					}
				}
				else
				{
					r.x = parent.Width_Renamed - (diffw / 2 + insets.Right);
					for (i = 0; i < constraints.TempX; i++)
					{
						r.x -= info.MinWidth[i];
					}
				}

				r.y = info.Starty;
				for (i = 0; i < constraints.TempY; i++)
				{
					r.y += info.MinHeight[i];
				}

				r.Width_Renamed = 0;
				for (i = constraints.TempX; i < (constraints.TempX + constraints.TempWidth); i++)
				{
					r.Width_Renamed += info.MinWidth[i];
				}

				r.Height_Renamed = 0;
				for (i = constraints.TempY; i < (constraints.TempY + constraints.TempHeight); i++)
				{
					r.Height_Renamed += info.MinHeight[i];
				}

				ComponentAdjusting = comp;
				AdjustForGravity(constraints, r);

				/* fix for 4408108 - components were being created outside of the container */
				/* fix for 4969409 "-" replaced by "+"  */
				if (r.x < 0)
				{
					r.Width_Renamed += r.x;
					r.x = 0;
				}

				if (r.y < 0)
				{
					r.Height_Renamed += r.y;
					r.y = 0;
				}

				/*
				 * If the window is too small to be interesting then
				 * unmap it.  Otherwise configure it and then make sure
				 * it's mapped.
				 */

				if ((r.Width_Renamed <= 0) || (r.Height_Renamed <= 0))
				{
					comp.SetBounds(0, 0, 0, 0);
				}
				else
				{
					if (comp.x != r.x || comp.y != r.y || comp.Width_Renamed != r.Width_Renamed || comp.Height_Renamed != r.Height_Renamed)
					{
						comp.SetBounds(r.x, r.y, r.Width_Renamed, r.Height_Renamed);
					}
				}
			}
		}

		// Added for serial backwards compatibility (4348425)
		internal const long SerialVersionUID = 8838754796412211005L;
	}

}