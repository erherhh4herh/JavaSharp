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
	/// The <code>GridBagConstraints</code> class specifies constraints
	/// for components that are laid out using the
	/// <code>GridBagLayout</code> class.
	/// 
	/// @author Doug Stein
	/// @author Bill Spitzak (orignial NeWS &amp; OLIT implementation) </summary>
	/// <seealso cref= java.awt.GridBagLayout
	/// @since JDK1.0 </seealso>
	[Serializable]
	public class GridBagConstraints : Cloneable
	{

		/// <summary>
		/// Specifies that this component is the next-to-last component in its
		/// column or row (<code>gridwidth</code>, <code>gridheight</code>),
		/// or that this component be placed next to the previously added
		/// component (<code>gridx</code>, <code>gridy</code>). </summary>
		/// <seealso cref=      java.awt.GridBagConstraints#gridwidth </seealso>
		/// <seealso cref=      java.awt.GridBagConstraints#gridheight </seealso>
		/// <seealso cref=      java.awt.GridBagConstraints#gridx </seealso>
		/// <seealso cref=      java.awt.GridBagConstraints#gridy </seealso>
		public const int RELATIVE = -1;

		/// <summary>
		/// Specifies that this component is the
		/// last component in its column or row.
		/// </summary>
		public const int REMAINDER = 0;

		/// <summary>
		/// Do not resize the component.
		/// </summary>
		public const int NONE = 0;

		/// <summary>
		/// Resize the component both horizontally and vertically.
		/// </summary>
		public const int BOTH = 1;

		/// <summary>
		/// Resize the component horizontally but not vertically.
		/// </summary>
		public const int HORIZONTAL = 2;

		/// <summary>
		/// Resize the component vertically but not horizontally.
		/// </summary>
		public const int VERTICAL = 3;

		/// <summary>
		/// Put the component in the center of its display area.
		/// </summary>
		public const int CENTER = 10;

		/// <summary>
		/// Put the component at the top of its display area,
		/// centered horizontally.
		/// </summary>
		public const int NORTH = 11;

		/// <summary>
		/// Put the component at the top-right corner of its display area.
		/// </summary>
		public const int NORTHEAST = 12;

		/// <summary>
		/// Put the component on the right side of its display area,
		/// centered vertically.
		/// </summary>
		public const int EAST = 13;

		/// <summary>
		/// Put the component at the bottom-right corner of its display area.
		/// </summary>
		public const int SOUTHEAST = 14;

		/// <summary>
		/// Put the component at the bottom of its display area, centered
		/// horizontally.
		/// </summary>
		public const int SOUTH = 15;

		/// <summary>
		/// Put the component at the bottom-left corner of its display area.
		/// </summary>
		public const int SOUTHWEST = 16;

		/// <summary>
		/// Put the component on the left side of its display area,
		/// centered vertically.
		/// </summary>
		public const int WEST = 17;

		/// <summary>
		/// Put the component at the top-left corner of its display area.
		/// </summary>
		public const int NORTHWEST = 18;

		/// <summary>
		/// Place the component centered along the edge of its display area
		/// associated with the start of a page for the current
		/// {@code ComponentOrientation}.  Equal to NORTH for horizontal
		/// orientations.
		/// </summary>
		public const int PAGE_START = 19;

		/// <summary>
		/// Place the component centered along the edge of its display area
		/// associated with the end of a page for the current
		/// {@code ComponentOrientation}.  Equal to SOUTH for horizontal
		/// orientations.
		/// </summary>
		public const int PAGE_END = 20;

		/// <summary>
		/// Place the component centered along the edge of its display area where
		/// lines of text would normally begin for the current
		/// {@code ComponentOrientation}.  Equal to WEST for horizontal,
		/// left-to-right orientations and EAST for horizontal, right-to-left
		/// orientations.
		/// </summary>
		public const int LINE_START = 21;

		/// <summary>
		/// Place the component centered along the edge of its display area where
		/// lines of text would normally end for the current
		/// {@code ComponentOrientation}.  Equal to EAST for horizontal,
		/// left-to-right orientations and WEST for horizontal, right-to-left
		/// orientations.
		/// </summary>
		public const int LINE_END = 22;

		/// <summary>
		/// Place the component in the corner of its display area where
		/// the first line of text on a page would normally begin for the current
		/// {@code ComponentOrientation}.  Equal to NORTHWEST for horizontal,
		/// left-to-right orientations and NORTHEAST for horizontal, right-to-left
		/// orientations.
		/// </summary>
		public const int FIRST_LINE_START = 23;

		/// <summary>
		/// Place the component in the corner of its display area where
		/// the first line of text on a page would normally end for the current
		/// {@code ComponentOrientation}.  Equal to NORTHEAST for horizontal,
		/// left-to-right orientations and NORTHWEST for horizontal, right-to-left
		/// orientations.
		/// </summary>
		public const int FIRST_LINE_END = 24;

		/// <summary>
		/// Place the component in the corner of its display area where
		/// the last line of text on a page would normally start for the current
		/// {@code ComponentOrientation}.  Equal to SOUTHWEST for horizontal,
		/// left-to-right orientations and SOUTHEAST for horizontal, right-to-left
		/// orientations.
		/// </summary>
		public const int LAST_LINE_START = 25;

		/// <summary>
		/// Place the component in the corner of its display area where
		/// the last line of text on a page would normally end for the current
		/// {@code ComponentOrientation}.  Equal to SOUTHEAST for horizontal,
		/// left-to-right orientations and SOUTHWEST for horizontal, right-to-left
		/// orientations.
		/// </summary>
		public const int LAST_LINE_END = 26;

		/// <summary>
		/// Possible value for the <code>anchor</code> field.  Specifies
		/// that the component should be horizontally centered and
		/// vertically aligned along the baseline of the prevailing row.
		/// If the component does not have a baseline it will be vertically
		/// centered.
		/// 
		/// @since 1.6
		/// </summary>
		public const int BASELINE = 0x100;

		/// <summary>
		/// Possible value for the <code>anchor</code> field.  Specifies
		/// that the component should be horizontally placed along the
		/// leading edge.  For components with a left-to-right orientation,
		/// the leading edge is the left edge.  Vertically the component is
		/// aligned along the baseline of the prevailing row.  If the
		/// component does not have a baseline it will be vertically
		/// centered.
		/// 
		/// @since 1.6
		/// </summary>
		public const int BASELINE_LEADING = 0x200;

		/// <summary>
		/// Possible value for the <code>anchor</code> field.  Specifies
		/// that the component should be horizontally placed along the
		/// trailing edge.  For components with a left-to-right
		/// orientation, the trailing edge is the right edge.  Vertically
		/// the component is aligned along the baseline of the prevailing
		/// row.  If the component does not have a baseline it will be
		/// vertically centered.
		/// 
		/// @since 1.6
		/// </summary>
		public const int BASELINE_TRAILING = 0x300;

		/// <summary>
		/// Possible value for the <code>anchor</code> field.  Specifies
		/// that the component should be horizontally centered.  Vertically
		/// the component is positioned so that its bottom edge touches
		/// the baseline of the starting row.  If the starting row does not
		/// have a baseline it will be vertically centered.
		/// 
		/// @since 1.6
		/// </summary>
		public const int ABOVE_BASELINE = 0x400;

		/// <summary>
		/// Possible value for the <code>anchor</code> field.  Specifies
		/// that the component should be horizontally placed along the
		/// leading edge.  For components with a left-to-right orientation,
		/// the leading edge is the left edge.  Vertically the component is
		/// positioned so that its bottom edge touches the baseline of the
		/// starting row.  If the starting row does not have a baseline it
		/// will be vertically centered.
		/// 
		/// @since 1.6
		/// </summary>
		public const int ABOVE_BASELINE_LEADING = 0x500;

		/// <summary>
		/// Possible value for the <code>anchor</code> field.  Specifies
		/// that the component should be horizontally placed along the
		/// trailing edge.  For components with a left-to-right
		/// orientation, the trailing edge is the right edge.  Vertically
		/// the component is positioned so that its bottom edge touches
		/// the baseline of the starting row.  If the starting row does not
		/// have a baseline it will be vertically centered.
		/// 
		/// @since 1.6
		/// </summary>
		public const int ABOVE_BASELINE_TRAILING = 0x600;

		/// <summary>
		/// Possible value for the <code>anchor</code> field.  Specifies
		/// that the component should be horizontally centered.  Vertically
		/// the component is positioned so that its top edge touches the
		/// baseline of the starting row.  If the starting row does not
		/// have a baseline it will be vertically centered.
		/// 
		/// @since 1.6
		/// </summary>
		public const int BELOW_BASELINE = 0x700;

		/// <summary>
		/// Possible value for the <code>anchor</code> field.  Specifies
		/// that the component should be horizontally placed along the
		/// leading edge.  For components with a left-to-right orientation,
		/// the leading edge is the left edge.  Vertically the component is
		/// positioned so that its top edge touches the baseline of the
		/// starting row.  If the starting row does not have a baseline it
		/// will be vertically centered.
		/// 
		/// @since 1.6
		/// </summary>
		public const int BELOW_BASELINE_LEADING = 0x800;

		/// <summary>
		/// Possible value for the <code>anchor</code> field.  Specifies
		/// that the component should be horizontally placed along the
		/// trailing edge.  For components with a left-to-right
		/// orientation, the trailing edge is the right edge.  Vertically
		/// the component is positioned so that its top edge touches the
		/// baseline of the starting row.  If the starting row does not
		/// have a baseline it will be vertically centered.
		/// 
		/// @since 1.6
		/// </summary>
		public const int BELOW_BASELINE_TRAILING = 0x900;

		/// <summary>
		/// Specifies the cell containing the leading edge of the component's
		/// display area, where the first cell in a row has <code>gridx=0</code>.
		/// The leading edge of a component's display area is its left edge for
		/// a horizontal, left-to-right container and its right edge for a
		/// horizontal, right-to-left container.
		/// The value
		/// <code>RELATIVE</code> specifies that the component be placed
		/// immediately following the component that was added to the container
		/// just before this component was added.
		/// <para>
		/// The default value is <code>RELATIVE</code>.
		/// <code>gridx</code> should be a non-negative value.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#gridy </seealso>
		/// <seealso cref= java.awt.ComponentOrientation </seealso>
		public int Gridx;

		/// <summary>
		/// Specifies the cell at the top of the component's display area,
		/// where the topmost cell has <code>gridy=0</code>. The value
		/// <code>RELATIVE</code> specifies that the component be placed just
		/// below the component that was added to the container just before
		/// this component was added.
		/// <para>
		/// The default value is <code>RELATIVE</code>.
		/// <code>gridy</code> should be a non-negative value.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#gridx </seealso>
		public int Gridy;

		/// <summary>
		/// Specifies the number of cells in a row for the component's
		/// display area.
		/// <para>
		/// Use <code>REMAINDER</code> to specify that the component's
		/// display area will be from <code>gridx</code> to the last
		/// cell in the row.
		/// Use <code>RELATIVE</code> to specify that the component's
		/// display area will be from <code>gridx</code> to the next
		/// to the last one in its row.
		/// </para>
		/// <para>
		/// <code>gridwidth</code> should be non-negative and the default
		/// value is 1.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#gridheight </seealso>
		public int Gridwidth;

		/// <summary>
		/// Specifies the number of cells in a column for the component's
		/// display area.
		/// <para>
		/// Use <code>REMAINDER</code> to specify that the component's
		/// display area will be from <code>gridy</code> to the last
		/// cell in the column.
		/// Use <code>RELATIVE</code> to specify that the component's
		/// display area will be from <code>gridy</code> to the next
		/// to the last one in its column.
		/// </para>
		/// <para>
		/// <code>gridheight</code> should be a non-negative value and the
		/// default value is 1.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#gridwidth </seealso>
		public int Gridheight;

		/// <summary>
		/// Specifies how to distribute extra horizontal space.
		/// <para>
		/// The grid bag layout manager calculates the weight of a column to
		/// be the maximum <code>weightx</code> of all the components in a
		/// column. If the resulting layout is smaller horizontally than the area
		/// it needs to fill, the extra space is distributed to each column in
		/// proportion to its weight. A column that has a weight of zero receives
		/// no extra space.
		/// </para>
		/// <para>
		/// If all the weights are zero, all the extra space appears between
		/// the grids of the cell and the left and right edges.
		/// </para>
		/// <para>
		/// The default value of this field is <code>0</code>.
		/// <code>weightx</code> should be a non-negative value.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#weighty </seealso>
		public double Weightx;

		/// <summary>
		/// Specifies how to distribute extra vertical space.
		/// <para>
		/// The grid bag layout manager calculates the weight of a row to be
		/// the maximum <code>weighty</code> of all the components in a row.
		/// If the resulting layout is smaller vertically than the area it
		/// needs to fill, the extra space is distributed to each row in
		/// proportion to its weight. A row that has a weight of zero receives no
		/// extra space.
		/// </para>
		/// <para>
		/// If all the weights are zero, all the extra space appears between
		/// the grids of the cell and the top and bottom edges.
		/// </para>
		/// <para>
		/// The default value of this field is <code>0</code>.
		/// <code>weighty</code> should be a non-negative value.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#weightx </seealso>
		public double Weighty;

		/// <summary>
		/// This field is used when the component is smaller than its
		/// display area. It determines where, within the display area, to
		/// place the component.
		/// <para> There are three kinds of possible values: orientation
		/// relative, baseline relative and absolute.  Orientation relative
		/// values are interpreted relative to the container's component
		/// orientation property, baseline relative values are interpreted
		/// relative to the baseline and absolute values are not.  The
		/// absolute values are:
		/// <code>CENTER</code>, <code>NORTH</code>, <code>NORTHEAST</code>,
		/// <code>EAST</code>, <code>SOUTHEAST</code>, <code>SOUTH</code>,
		/// <code>SOUTHWEST</code>, <code>WEST</code>, and <code>NORTHWEST</code>.
		/// The orientation relative values are: <code>PAGE_START</code>,
		/// <code>PAGE_END</code>,
		/// <code>LINE_START</code>, <code>LINE_END</code>,
		/// <code>FIRST_LINE_START</code>, <code>FIRST_LINE_END</code>,
		/// <code>LAST_LINE_START</code> and <code>LAST_LINE_END</code>.  The
		/// baseline relative values are:
		/// <code>BASELINE</code>, <code>BASELINE_LEADING</code>,
		/// <code>BASELINE_TRAILING</code>,
		/// <code>ABOVE_BASELINE</code>, <code>ABOVE_BASELINE_LEADING</code>,
		/// <code>ABOVE_BASELINE_TRAILING</code>,
		/// <code>BELOW_BASELINE</code>, <code>BELOW_BASELINE_LEADING</code>,
		/// and <code>BELOW_BASELINE_TRAILING</code>.
		/// The default value is <code>CENTER</code>.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		/// <seealso cref= java.awt.ComponentOrientation </seealso>
		public int Anchor;

		/// <summary>
		/// This field is used when the component's display area is larger
		/// than the component's requested size. It determines whether to
		/// resize the component, and if so, how.
		/// <para>
		/// The following values are valid for <code>fill</code>:
		/// 
		/// <ul>
		/// <li>
		/// <code>NONE</code>: Do not resize the component.
		/// <li>
		/// <code>HORIZONTAL</code>: Make the component wide enough to fill
		///         its display area horizontally, but do not change its height.
		/// <li>
		/// <code>VERTICAL</code>: Make the component tall enough to fill its
		///         display area vertically, but do not change its width.
		/// <li>
		/// <code>BOTH</code>: Make the component fill its display area
		///         entirely.
		/// </ul>
		/// </para>
		/// <para>
		/// The default value is <code>NONE</code>.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		public int Fill;

		/// <summary>
		/// This field specifies the external padding of the component, the
		/// minimum amount of space between the component and the edges of its
		/// display area.
		/// <para>
		/// The default value is <code>new Insets(0, 0, 0, 0)</code>.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		public Insets Insets;

		/// <summary>
		/// This field specifies the internal padding of the component, how much
		/// space to add to the minimum width of the component. The width of
		/// the component is at least its minimum width plus
		/// <code>ipadx</code> pixels.
		/// <para>
		/// The default value is <code>0</code>.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#ipady </seealso>
		public int Ipadx;

		/// <summary>
		/// This field specifies the internal padding, that is, how much
		/// space to add to the minimum height of the component. The height of
		/// the component is at least its minimum height plus
		/// <code>ipady</code> pixels.
		/// <para>
		/// The default value is 0.
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #clone() </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#ipadx </seealso>
		public int Ipady;

		/// <summary>
		/// Temporary place holder for the x coordinate.
		/// @serial
		/// </summary>
		internal int TempX;
		/// <summary>
		/// Temporary place holder for the y coordinate.
		/// @serial
		/// </summary>
		internal int TempY;
		/// <summary>
		/// Temporary place holder for the Width of the component.
		/// @serial
		/// </summary>
		internal int TempWidth;
		/// <summary>
		/// Temporary place holder for the Height of the component.
		/// @serial
		/// </summary>
		internal int TempHeight;
		/// <summary>
		/// The minimum width of the component.  It is used to calculate
		/// <code>ipady</code>, where the default will be 0.
		/// @serial </summary>
		/// <seealso cref= #ipady </seealso>
		internal int MinWidth;
		/// <summary>
		/// The minimum height of the component. It is used to calculate
		/// <code>ipadx</code>, where the default will be 0.
		/// @serial </summary>
		/// <seealso cref= #ipadx </seealso>
		internal int MinHeight;

		// The following fields are only used if the anchor is
		// one of BASELINE, BASELINE_LEADING or BASELINE_TRAILING.
		// ascent and descent include the insets and ipady values.
		[NonSerialized]
		internal int Ascent;
		[NonSerialized]
		internal int Descent;
		[NonSerialized]
		internal Component.BaselineResizeBehavior BaselineResizeBehavior;
		// The folllowing two fields are used if the baseline type is
		// CENTER_OFFSET.
		// centerPadding is either 0 or 1 and indicates if
		// the height needs to be padded by one when calculating where the
		// baseline lands
		[NonSerialized]
		internal int CenterPadding;
		// Where the baseline lands relative to the center of the component.
		[NonSerialized]
		internal int CenterOffset;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = -1000070633030801713L;

		/// <summary>
		/// Creates a <code>GridBagConstraint</code> object with
		/// all of its fields set to their default value.
		/// </summary>
		public GridBagConstraints()
		{
			Gridx = RELATIVE;
			Gridy = RELATIVE;
			Gridwidth = 1;
			Gridheight = 1;

			Weightx = 0;
			Weighty = 0;
			Anchor = CENTER;
			Fill = NONE;

			Insets = new Insets(0, 0, 0, 0);
			Ipadx = 0;
			Ipady = 0;
		}

		/// <summary>
		/// Creates a <code>GridBagConstraints</code> object with
		/// all of its fields set to the passed-in arguments.
		/// 
		/// Note: Because the use of this constructor hinders readability
		/// of source code, this constructor should only be used by
		/// automatic source code generation tools.
		/// </summary>
		/// <param name="gridx">     The initial gridx value. </param>
		/// <param name="gridy">     The initial gridy value. </param>
		/// <param name="gridwidth"> The initial gridwidth value. </param>
		/// <param name="gridheight">        The initial gridheight value. </param>
		/// <param name="weightx">   The initial weightx value. </param>
		/// <param name="weighty">   The initial weighty value. </param>
		/// <param name="anchor">    The initial anchor value. </param>
		/// <param name="fill">      The initial fill value. </param>
		/// <param name="insets">    The initial insets value. </param>
		/// <param name="ipadx">     The initial ipadx value. </param>
		/// <param name="ipady">     The initial ipady value.
		/// </param>
		/// <seealso cref= java.awt.GridBagConstraints#gridx </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#gridy </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#gridwidth </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#gridheight </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#weightx </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#weighty </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#anchor </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#fill </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#insets </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#ipadx </seealso>
		/// <seealso cref= java.awt.GridBagConstraints#ipady
		/// 
		/// @since 1.2 </seealso>
		public GridBagConstraints(int gridx, int gridy, int gridwidth, int gridheight, double weightx, double weighty, int anchor, int fill, Insets insets, int ipadx, int ipady)
		{
			this.Gridx = gridx;
			this.Gridy = gridy;
			this.Gridwidth = gridwidth;
			this.Gridheight = gridheight;
			this.Fill = fill;
			this.Ipadx = ipadx;
			this.Ipady = ipady;
			this.Insets = insets;
			this.Anchor = anchor;
			this.Weightx = weightx;
			this.Weighty = weighty;
		}

		/// <summary>
		/// Creates a copy of this grid bag constraint. </summary>
		/// <returns>     a copy of this grid bag constraint </returns>
		public virtual Object Clone()
		{
			try
			{
				GridBagConstraints c = (GridBagConstraints)base.Clone();
				c.Insets = (Insets)Insets.Clone();
				return c;
			}
			catch (CloneNotSupportedException e)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError(e);
			}
		}

		internal virtual bool VerticallyResizable
		{
			get
			{
				return (Fill == BOTH || Fill == VERTICAL);
			}
		}
	}

}