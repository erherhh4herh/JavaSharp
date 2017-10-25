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
	/// A border layout lays out a container, arranging and resizing
	/// its components to fit in five regions:
	/// north, south, east, west, and center.
	/// Each region may contain no more than one component, and
	/// is identified by a corresponding constant:
	/// <code>NORTH</code>, <code>SOUTH</code>, <code>EAST</code>,
	/// <code>WEST</code>, and <code>CENTER</code>.  When adding a
	/// component to a container with a border layout, use one of these
	/// five constants, for example:
	/// <pre>
	///    Panel p = new Panel();
	///    p.setLayout(new BorderLayout());
	///    p.add(new Button("Okay"), BorderLayout.SOUTH);
	/// </pre>
	/// As a convenience, <code>BorderLayout</code> interprets the
	/// absence of a string specification the same as the constant
	/// <code>CENTER</code>:
	/// <pre>
	///    Panel p2 = new Panel();
	///    p2.setLayout(new BorderLayout());
	///    p2.add(new TextArea());  // Same as p.add(new TextArea(), BorderLayout.CENTER);
	/// </pre>
	/// <para>
	/// In addition, <code>BorderLayout</code> supports the relative
	/// positioning constants, <code>PAGE_START</code>, <code>PAGE_END</code>,
	/// <code>LINE_START</code>, and <code>LINE_END</code>.
	/// In a container whose <code>ComponentOrientation</code> is set to
	/// <code>ComponentOrientation.LEFT_TO_RIGHT</code>, these constants map to
	/// <code>NORTH</code>, <code>SOUTH</code>, <code>WEST</code>, and
	/// <code>EAST</code>, respectively.
	/// </para>
	/// <para>
	/// For compatibility with previous releases, <code>BorderLayout</code>
	/// also includes the relative positioning constants <code>BEFORE_FIRST_LINE</code>,
	/// <code>AFTER_LAST_LINE</code>, <code>BEFORE_LINE_BEGINS</code> and
	/// <code>AFTER_LINE_ENDS</code>.  These are equivalent to
	/// <code>PAGE_START</code>, <code>PAGE_END</code>, <code>LINE_START</code>
	/// and <code>LINE_END</code> respectively.  For
	/// consistency with the relative positioning constants used by other
	/// components, the latter constants are preferred.
	/// </para>
	/// <para>
	/// Mixing both absolute and relative positioning constants can lead to
	/// unpredictable results.  If
	/// you use both types, the relative constants will take precedence.
	/// For example, if you add components using both the <code>NORTH</code>
	/// and <code>PAGE_START</code> constants in a container whose
	/// orientation is <code>LEFT_TO_RIGHT</code>, only the
	/// <code>PAGE_START</code> will be layed out.
	/// </para>
	/// <para>
	/// NOTE: Currently (in the Java 2 platform v1.2),
	/// <code>BorderLayout</code> does not support vertical
	/// orientations.  The <code>isVertical</code> setting on the container's
	/// <code>ComponentOrientation</code> is not respected.
	/// </para>
	/// <para>
	/// The components are laid out according to their
	/// preferred sizes and the constraints of the container's size.
	/// The <code>NORTH</code> and <code>SOUTH</code> components may
	/// be stretched horizontally; the <code>EAST</code> and
	/// <code>WEST</code> components may be stretched vertically;
	/// the <code>CENTER</code> component may stretch both horizontally
	/// and vertically to fill any space left over.
	/// </para>
	/// <para>
	/// Here is an example of five buttons in an applet laid out using
	/// the <code>BorderLayout</code> layout manager:
	/// </para>
	/// <para>
	/// <img src="doc-files/BorderLayout-1.gif"
	/// alt="Diagram of an applet demonstrating BorderLayout.
	///      Each section of the BorderLayout contains a Button corresponding to its position in the layout, one of:
	///      North, West, Center, East, or South."
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// The code for this applet is as follows:
	/// 
	/// <hr><blockquote><pre>
	/// import java.awt.*;
	/// import java.applet.Applet;
	/// 
	/// public class buttonDir extends Applet {
	///   public void init() {
	///     setLayout(new BorderLayout());
	///     add(new Button("North"), BorderLayout.NORTH);
	///     add(new Button("South"), BorderLayout.SOUTH);
	///     add(new Button("East"), BorderLayout.EAST);
	///     add(new Button("West"), BorderLayout.WEST);
	///     add(new Button("Center"), BorderLayout.CENTER);
	///   }
	/// }
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// @author      Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.Container#add(String, Component) </seealso>
	/// <seealso cref=         java.awt.ComponentOrientation
	/// @since       JDK1.0 </seealso>
	[Serializable]
	public class BorderLayout : LayoutManager2
	{
		/// <summary>
		/// Constructs a border layout with the horizontal gaps
		/// between components.
		/// The horizontal gap is specified by <code>hgap</code>.
		/// </summary>
		/// <seealso cref= #getHgap() </seealso>
		/// <seealso cref= #setHgap(int)
		/// 
		/// @serial </seealso>
			internal int Hgap_Renamed;

		/// <summary>
		/// Constructs a border layout with the vertical gaps
		/// between components.
		/// The vertical gap is specified by <code>vgap</code>.
		/// </summary>
		/// <seealso cref= #getVgap() </seealso>
		/// <seealso cref= #setVgap(int)
		/// @serial </seealso>
			internal int Vgap_Renamed;

		/// <summary>
		/// Constant to specify components location to be the
		///      north portion of the border layout.
		/// @serial </summary>
		/// <seealso cref= #getChild(String, boolean) </seealso>
		/// <seealso cref= #addLayoutComponent </seealso>
		/// <seealso cref= #getLayoutAlignmentX </seealso>
		/// <seealso cref= #getLayoutAlignmentY </seealso>
		/// <seealso cref= #removeLayoutComponent </seealso>
			internal Component North;
		 /// <summary>
		 /// Constant to specify components location to be the
		 ///      west portion of the border layout.
		 /// @serial </summary>
		 /// <seealso cref= #getChild(String, boolean) </seealso>
		 /// <seealso cref= #addLayoutComponent </seealso>
		 /// <seealso cref= #getLayoutAlignmentX </seealso>
		 /// <seealso cref= #getLayoutAlignmentY </seealso>
		 /// <seealso cref= #removeLayoutComponent </seealso>
			internal Component West;
		/// <summary>
		/// Constant to specify components location to be the
		///      east portion of the border layout.
		/// @serial </summary>
		/// <seealso cref= #getChild(String, boolean) </seealso>
		/// <seealso cref= #addLayoutComponent </seealso>
		/// <seealso cref= #getLayoutAlignmentX </seealso>
		/// <seealso cref= #getLayoutAlignmentY </seealso>
		/// <seealso cref= #removeLayoutComponent </seealso>
			internal Component East;
		/// <summary>
		/// Constant to specify components location to be the
		///      south portion of the border layout.
		/// @serial </summary>
		/// <seealso cref= #getChild(String, boolean) </seealso>
		/// <seealso cref= #addLayoutComponent </seealso>
		/// <seealso cref= #getLayoutAlignmentX </seealso>
		/// <seealso cref= #getLayoutAlignmentY </seealso>
		/// <seealso cref= #removeLayoutComponent </seealso>
		internal Component South;
		/// <summary>
		/// Constant to specify components location to be the
		///      center portion of the border layout.
		/// @serial </summary>
		/// <seealso cref= #getChild(String, boolean) </seealso>
		/// <seealso cref= #addLayoutComponent </seealso>
		/// <seealso cref= #getLayoutAlignmentX </seealso>
		/// <seealso cref= #getLayoutAlignmentY </seealso>
		/// <seealso cref= #removeLayoutComponent </seealso>
			internal Component Center;

		/// 
		/// <summary>
		/// A relative positioning constant, that can be used instead of
		/// north, south, east, west or center.
		/// mixing the two types of constants can lead to unpredictable results.  If
		/// you use both types, the relative constants will take precedence.
		/// For example, if you add components using both the <code>NORTH</code>
		/// and <code>BEFORE_FIRST_LINE</code> constants in a container whose
		/// orientation is <code>LEFT_TO_RIGHT</code>, only the
		/// <code>BEFORE_FIRST_LINE</code> will be layed out.
		/// This will be the same for lastLine, firstItem, lastItem.
		/// @serial
		/// </summary>
		internal Component FirstLine;
		 /// <summary>
		 /// A relative positioning constant, that can be used instead of
		 /// north, south, east, west or center.
		 /// Please read Description for firstLine.
		 /// @serial
		 /// </summary>
			internal Component LastLine;
		 /// <summary>
		 /// A relative positioning constant, that can be used instead of
		 /// north, south, east, west or center.
		 /// Please read Description for firstLine.
		 /// @serial
		 /// </summary>
			internal Component FirstItem;
		/// <summary>
		/// A relative positioning constant, that can be used instead of
		/// north, south, east, west or center.
		/// Please read Description for firstLine.
		/// @serial
		/// </summary>
			internal Component LastItem;

		/// <summary>
		/// The north layout constraint (top of container).
		/// </summary>
		public const String NORTH = "North";

		/// <summary>
		/// The south layout constraint (bottom of container).
		/// </summary>
		public const String SOUTH = "South";

		/// <summary>
		/// The east layout constraint (right side of container).
		/// </summary>
		public const String EAST = "East";

		/// <summary>
		/// The west layout constraint (left side of container).
		/// </summary>
		public const String WEST = "West";

		/// <summary>
		/// The center layout constraint (middle of container).
		/// </summary>
		public const String CENTER = "Center";

		/// <summary>
		/// Synonym for PAGE_START.  Exists for compatibility with previous
		/// versions.  PAGE_START is preferred.
		/// </summary>
		/// <seealso cref= #PAGE_START
		/// @since 1.2 </seealso>
		public const String BEFORE_FIRST_LINE = "First";

		/// <summary>
		/// Synonym for PAGE_END.  Exists for compatibility with previous
		/// versions.  PAGE_END is preferred.
		/// </summary>
		/// <seealso cref= #PAGE_END
		/// @since 1.2 </seealso>
		public const String AFTER_LAST_LINE = "Last";

		/// <summary>
		/// Synonym for LINE_START.  Exists for compatibility with previous
		/// versions.  LINE_START is preferred.
		/// </summary>
		/// <seealso cref= #LINE_START
		/// @since 1.2 </seealso>
		public const String BEFORE_LINE_BEGINS = "Before";

		/// <summary>
		/// Synonym for LINE_END.  Exists for compatibility with previous
		/// versions.  LINE_END is preferred.
		/// </summary>
		/// <seealso cref= #LINE_END
		/// @since 1.2 </seealso>
		public const String AFTER_LINE_ENDS = "After";

		/// <summary>
		/// The component comes before the first line of the layout's content.
		/// For Western, left-to-right and top-to-bottom orientations, this is
		/// equivalent to NORTH.
		/// </summary>
		/// <seealso cref= java.awt.Component#getComponentOrientation
		/// @since 1.4 </seealso>
		public const String PAGE_START = BEFORE_FIRST_LINE;

		/// <summary>
		/// The component comes after the last line of the layout's content.
		/// For Western, left-to-right and top-to-bottom orientations, this is
		/// equivalent to SOUTH.
		/// </summary>
		/// <seealso cref= java.awt.Component#getComponentOrientation
		/// @since 1.4 </seealso>
		public const String PAGE_END = AFTER_LAST_LINE;

		/// <summary>
		/// The component goes at the beginning of the line direction for the
		/// layout. For Western, left-to-right and top-to-bottom orientations,
		/// this is equivalent to WEST.
		/// </summary>
		/// <seealso cref= java.awt.Component#getComponentOrientation
		/// @since 1.4 </seealso>
		public const String LINE_START = BEFORE_LINE_BEGINS;

		/// <summary>
		/// The component goes at the end of the line direction for the
		/// layout. For Western, left-to-right and top-to-bottom orientations,
		/// this is equivalent to EAST.
		/// </summary>
		/// <seealso cref= java.awt.Component#getComponentOrientation
		/// @since 1.4 </seealso>
		public const String LINE_END = AFTER_LINE_ENDS;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = -8658291919501921765L;

		/// <summary>
		/// Constructs a new border layout with
		/// no gaps between components.
		/// </summary>
		public BorderLayout() : this(0, 0)
		{
		}

		/// <summary>
		/// Constructs a border layout with the specified gaps
		/// between components.
		/// The horizontal gap is specified by <code>hgap</code>
		/// and the vertical gap is specified by <code>vgap</code>. </summary>
		/// <param name="hgap">   the horizontal gap. </param>
		/// <param name="vgap">   the vertical gap. </param>
		public BorderLayout(int hgap, int vgap)
		{
			this.Hgap_Renamed = hgap;
			this.Vgap_Renamed = vgap;
		}

		/// <summary>
		/// Returns the horizontal gap between components.
		/// @since   JDK1.1
		/// </summary>
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
		/// Returns the vertical gap between components.
		/// @since   JDK1.1
		/// </summary>
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
		/// Adds the specified component to the layout, using the specified
		/// constraint object.  For border layouts, the constraint must be
		/// one of the following constants:  <code>NORTH</code>,
		/// <code>SOUTH</code>, <code>EAST</code>,
		/// <code>WEST</code>, or <code>CENTER</code>.
		/// <para>
		/// Most applications do not call this method directly. This method
		/// is called when a component is added to a container using the
		/// <code>Container.add</code> method with the same argument types.
		/// </para>
		/// </summary>
		/// <param name="comp">         the component to be added. </param>
		/// <param name="constraints">  an object that specifies how and where
		///                       the component is added to the layout. </param>
		/// <seealso cref=     java.awt.Container#add(java.awt.Component, java.lang.Object) </seealso>
		/// <exception cref="IllegalArgumentException">  if the constraint object is not
		///                 a string, or if it not one of the five specified
		///              constants.
		/// @since   JDK1.1 </exception>
		public virtual void AddLayoutComponent(Component comp, Object constraints)
		{
		  lock (comp.TreeLock)
		  {
			if ((constraints == null) || (constraints is String))
			{
				AddLayoutComponent((String)constraints, comp);
			}
			else
			{
				throw new IllegalArgumentException("cannot add to layout: constraint must be a string (or null)");
			}
		  }
		}

		/// @deprecated  replaced by <code>addLayoutComponent(Component, Object)</code>. 
		[Obsolete(" replaced by <code>addLayoutComponent(Component, Object)</code>.")]
		public virtual void AddLayoutComponent(String name, Component comp)
		{
		  lock (comp.TreeLock)
		  {
			/* Special case:  treat null the same as "Center". */
			if (name == null)
			{
				name = "Center";
			}

			/* Assign the component to one of the known regions of the layout.
			 */
			if ("Center".Equals(name))
			{
				Center = comp;
			}
			else if ("North".Equals(name))
			{
				North = comp;
			}
			else if ("South".Equals(name))
			{
				South = comp;
			}
			else if ("East".Equals(name))
			{
				East = comp;
			}
			else if ("West".Equals(name))
			{
				West = comp;
			}
			else if (BEFORE_FIRST_LINE.Equals(name))
			{
				FirstLine = comp;
			}
			else if (AFTER_LAST_LINE.Equals(name))
			{
				LastLine = comp;
			}
			else if (BEFORE_LINE_BEGINS.Equals(name))
			{
				FirstItem = comp;
			}
			else if (AFTER_LINE_ENDS.Equals(name))
			{
				LastItem = comp;
			}
			else
			{
				throw new IllegalArgumentException("cannot add to layout: unknown constraint: " + name);
			}
		  }
		}

		/// <summary>
		/// Removes the specified component from this border layout. This
		/// method is called when a container calls its <code>remove</code> or
		/// <code>removeAll</code> methods. Most applications do not call this
		/// method directly. </summary>
		/// <param name="comp">   the component to be removed. </param>
		/// <seealso cref=     java.awt.Container#remove(java.awt.Component) </seealso>
		/// <seealso cref=     java.awt.Container#removeAll() </seealso>
		public virtual void RemoveLayoutComponent(Component comp)
		{
		  lock (comp.TreeLock)
		  {
			if (comp == Center)
			{
				Center = null;
			}
			else if (comp == North)
			{
				North = null;
			}
			else if (comp == South)
			{
				South = null;
			}
			else if (comp == East)
			{
				East = null;
			}
			else if (comp == West)
			{
				West = null;
			}
			if (comp == FirstLine)
			{
				FirstLine = null;
			}
			else if (comp == LastLine)
			{
				LastLine = null;
			}
			else if (comp == FirstItem)
			{
				FirstItem = null;
			}
			else if (comp == LastItem)
			{
				LastItem = null;
			}
		  }
		}

		/// <summary>
		/// Gets the component that was added using the given constraint
		/// </summary>
		/// <param name="constraints">  the desired constraint, one of <code>CENTER</code>,
		///                       <code>NORTH</code>, <code>SOUTH</code>,
		///                       <code>WEST</code>, <code>EAST</code>,
		///                       <code>PAGE_START</code>, <code>PAGE_END</code>,
		///                       <code>LINE_START</code>, <code>LINE_END</code> </param>
		/// <returns>  the component at the given location, or <code>null</code> if
		///          the location is empty </returns>
		/// <exception cref="IllegalArgumentException">  if the constraint object is
		///              not one of the nine specified constants </exception>
		/// <seealso cref=     #addLayoutComponent(java.awt.Component, java.lang.Object)
		/// @since 1.5 </seealso>
		public virtual Component GetLayoutComponent(Object constraints)
		{
			if (CENTER.Equals(constraints))
			{
				return Center;
			}
			else if (NORTH.Equals(constraints))
			{
				return North;
			}
			else if (SOUTH.Equals(constraints))
			{
				return South;
			}
			else if (WEST.Equals(constraints))
			{
				return West;
			}
			else if (EAST.Equals(constraints))
			{
				return East;
			}
			else if (PAGE_START.Equals(constraints))
			{
				return FirstLine;
			}
			else if (PAGE_END.Equals(constraints))
			{
				return LastLine;
			}
			else if (LINE_START.Equals(constraints))
			{
				return FirstItem;
			}
			else if (LINE_END.Equals(constraints))
			{
				return LastItem;
			}
			else
			{
				throw new IllegalArgumentException("cannot get component: unknown constraint: " + constraints);
			}
		}


		/// <summary>
		/// Returns the component that corresponds to the given constraint location
		/// based on the target <code>Container</code>'s component orientation.
		/// Components added with the relative constraints <code>PAGE_START</code>,
		/// <code>PAGE_END</code>, <code>LINE_START</code>, and <code>LINE_END</code>
		/// take precedence over components added with the explicit constraints
		/// <code>NORTH</code>, <code>SOUTH</code>, <code>WEST</code>, and <code>EAST</code>.
		/// The <code>Container</code>'s component orientation is used to determine the location of components
		/// added with <code>LINE_START</code> and <code>LINE_END</code>.
		/// </summary>
		/// <param name="constraints">     the desired absolute position, one of <code>CENTER</code>,
		///                          <code>NORTH</code>, <code>SOUTH</code>,
		///                          <code>EAST</code>, <code>WEST</code> </param>
		/// <param name="target">     the {@code Container} used to obtain
		///                     the constraint location based on the target
		///                     {@code Container}'s component orientation. </param>
		/// <returns>  the component at the given location, or <code>null</code> if
		///          the location is empty </returns>
		/// <exception cref="IllegalArgumentException">  if the constraint object is
		///              not one of the five specified constants </exception>
		/// <exception cref="NullPointerException">  if the target parameter is null </exception>
		/// <seealso cref=     #addLayoutComponent(java.awt.Component, java.lang.Object)
		/// @since 1.5 </seealso>
		public virtual Component GetLayoutComponent(Container target, Object constraints)
		{
			bool ltr = target.ComponentOrientation.LeftToRight;
			Component result = null;

			if (NORTH.Equals(constraints))
			{
				result = (FirstLine != null) ? FirstLine : North;
			}
			else if (SOUTH.Equals(constraints))
			{
				result = (LastLine != null) ? LastLine : South;
			}
			else if (WEST.Equals(constraints))
			{
				result = ltr ? FirstItem : LastItem;
				if (result == null)
				{
					result = West;
				}
			}
			else if (EAST.Equals(constraints))
			{
				result = ltr ? LastItem : FirstItem;
				if (result == null)
				{
					result = East;
				}
			}
			else if (CENTER.Equals(constraints))
			{
				result = Center;
			}
			else
			{
				throw new IllegalArgumentException("cannot get component: invalid constraint: " + constraints);
			}

			return result;
		}


		/// <summary>
		/// Gets the constraints for the specified component
		/// </summary>
		/// <param name="comp"> the component to be queried </param>
		/// <returns>  the constraint for the specified component,
		///          or null if component is null or is not present
		///          in this layout </returns>
		/// <seealso cref= #addLayoutComponent(java.awt.Component, java.lang.Object)
		/// @since 1.5 </seealso>
		public virtual Object GetConstraints(Component comp)
		{
			//fix for 6242148 : API method java.awt.BorderLayout.getConstraints(null) should return null
			if (comp == null)
			{
				return null;
			}
			if (comp == Center)
			{
				return CENTER;
			}
			else if (comp == North)
			{
				return NORTH;
			}
			else if (comp == South)
			{
				return SOUTH;
			}
			else if (comp == West)
			{
				return WEST;
			}
			else if (comp == East)
			{
				return EAST;
			}
			else if (comp == FirstLine)
			{
				return PAGE_START;
			}
			else if (comp == LastLine)
			{
				return PAGE_END;
			}
			else if (comp == FirstItem)
			{
				return LINE_START;
			}
			else if (comp == LastItem)
			{
				return LINE_END;
			}
			return null;
		}

		/// <summary>
		/// Determines the minimum size of the <code>target</code> container
		/// using this layout manager.
		/// <para>
		/// This method is called when a container calls its
		/// <code>getMinimumSize</code> method. Most applications do not call
		/// this method directly.
		/// </para>
		/// </summary>
		/// <param name="target">   the container in which to do the layout. </param>
		/// <returns>  the minimum dimensions needed to lay out the subcomponents
		///          of the specified container. </returns>
		/// <seealso cref=     java.awt.Container </seealso>
		/// <seealso cref=     java.awt.BorderLayout#preferredLayoutSize </seealso>
		/// <seealso cref=     java.awt.Container#getMinimumSize() </seealso>
		public virtual Dimension MinimumLayoutSize(Container target)
		{
		  lock (target.TreeLock)
		  {
			Dimension dim = new Dimension(0, 0);

			bool ltr = target.ComponentOrientation.LeftToRight;
			Component c = null;

			if ((c = GetChild(EAST,ltr)) != null)
			{
				Dimension d = c.MinimumSize;
				dim.Width_Renamed += d.Width_Renamed + Hgap_Renamed;
				dim.Height_Renamed = System.Math.Max(d.Height_Renamed, dim.Height_Renamed);
			}
			if ((c = GetChild(WEST,ltr)) != null)
			{
				Dimension d = c.MinimumSize;
				dim.Width_Renamed += d.Width_Renamed + Hgap_Renamed;
				dim.Height_Renamed = System.Math.Max(d.Height_Renamed, dim.Height_Renamed);
			}
			if ((c = GetChild(CENTER,ltr)) != null)
			{
				Dimension d = c.MinimumSize;
				dim.Width_Renamed += d.Width_Renamed;
				dim.Height_Renamed = System.Math.Max(d.Height_Renamed, dim.Height_Renamed);
			}
			if ((c = GetChild(NORTH,ltr)) != null)
			{
				Dimension d = c.MinimumSize;
				dim.Width_Renamed = System.Math.Max(d.Width_Renamed, dim.Width_Renamed);
				dim.Height_Renamed += d.Height_Renamed + Vgap_Renamed;
			}
			if ((c = GetChild(SOUTH,ltr)) != null)
			{
				Dimension d = c.MinimumSize;
				dim.Width_Renamed = System.Math.Max(d.Width_Renamed, dim.Width_Renamed);
				dim.Height_Renamed += d.Height_Renamed + Vgap_Renamed;
			}

			Insets insets = target.Insets;
			dim.Width_Renamed += insets.Left + insets.Right;
			dim.Height_Renamed += insets.Top + insets.Bottom;

			return dim;
		  }
		}

		/// <summary>
		/// Determines the preferred size of the <code>target</code>
		/// container using this layout manager, based on the components
		/// in the container.
		/// <para>
		/// Most applications do not call this method directly. This method
		/// is called when a container calls its <code>getPreferredSize</code>
		/// method.
		/// </para>
		/// </summary>
		/// <param name="target">   the container in which to do the layout. </param>
		/// <returns>  the preferred dimensions to lay out the subcomponents
		///          of the specified container. </returns>
		/// <seealso cref=     java.awt.Container </seealso>
		/// <seealso cref=     java.awt.BorderLayout#minimumLayoutSize </seealso>
		/// <seealso cref=     java.awt.Container#getPreferredSize() </seealso>
		public virtual Dimension PreferredLayoutSize(Container target)
		{
		  lock (target.TreeLock)
		  {
			Dimension dim = new Dimension(0, 0);

			bool ltr = target.ComponentOrientation.LeftToRight;
			Component c = null;

			if ((c = GetChild(EAST,ltr)) != null)
			{
				Dimension d = c.PreferredSize;
				dim.Width_Renamed += d.Width_Renamed + Hgap_Renamed;
				dim.Height_Renamed = System.Math.Max(d.Height_Renamed, dim.Height_Renamed);
			}
			if ((c = GetChild(WEST,ltr)) != null)
			{
				Dimension d = c.PreferredSize;
				dim.Width_Renamed += d.Width_Renamed + Hgap_Renamed;
				dim.Height_Renamed = System.Math.Max(d.Height_Renamed, dim.Height_Renamed);
			}
			if ((c = GetChild(CENTER,ltr)) != null)
			{
				Dimension d = c.PreferredSize;
				dim.Width_Renamed += d.Width_Renamed;
				dim.Height_Renamed = System.Math.Max(d.Height_Renamed, dim.Height_Renamed);
			}
			if ((c = GetChild(NORTH,ltr)) != null)
			{
				Dimension d = c.PreferredSize;
				dim.Width_Renamed = System.Math.Max(d.Width_Renamed, dim.Width_Renamed);
				dim.Height_Renamed += d.Height_Renamed + Vgap_Renamed;
			}
			if ((c = GetChild(SOUTH,ltr)) != null)
			{
				Dimension d = c.PreferredSize;
				dim.Width_Renamed = System.Math.Max(d.Width_Renamed, dim.Width_Renamed);
				dim.Height_Renamed += d.Height_Renamed + Vgap_Renamed;
			}

			Insets insets = target.Insets;
			dim.Width_Renamed += insets.Left + insets.Right;
			dim.Height_Renamed += insets.Top + insets.Bottom;

			return dim;
		  }
		}

		/// <summary>
		/// Returns the maximum dimensions for this layout given the components
		/// in the specified target container. </summary>
		/// <param name="target"> the component which needs to be laid out </param>
		/// <seealso cref= Container </seealso>
		/// <seealso cref= #minimumLayoutSize </seealso>
		/// <seealso cref= #preferredLayoutSize </seealso>
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
		/// </summary>
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
		/// </summary>
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
		/// Lays out the container argument using this border layout.
		/// <para>
		/// This method actually reshapes the components in the specified
		/// container in order to satisfy the constraints of this
		/// <code>BorderLayout</code> object. The <code>NORTH</code>
		/// and <code>SOUTH</code> components, if any, are placed at
		/// the top and bottom of the container, respectively. The
		/// <code>WEST</code> and <code>EAST</code> components are
		/// then placed on the left and right, respectively. Finally,
		/// the <code>CENTER</code> object is placed in any remaining
		/// space in the middle.
		/// </para>
		/// <para>
		/// Most applications do not call this method directly. This method
		/// is called when a container calls its <code>doLayout</code> method.
		/// </para>
		/// </summary>
		/// <param name="target">   the container in which to do the layout. </param>
		/// <seealso cref=     java.awt.Container </seealso>
		/// <seealso cref=     java.awt.Container#doLayout() </seealso>
		public virtual void LayoutContainer(Container target)
		{
		  lock (target.TreeLock)
		  {
			Insets insets = target.Insets;
			int top = insets.Top;
			int bottom = target.Height_Renamed - insets.Bottom;
			int left = insets.Left;
			int right = target.Width_Renamed - insets.Right;

			bool ltr = target.ComponentOrientation.LeftToRight;
			Component c = null;

			if ((c = GetChild(NORTH,ltr)) != null)
			{
				c.SetSize(right - left, c.Height_Renamed);
				Dimension d = c.PreferredSize;
				c.SetBounds(left, top, right - left, d.Height_Renamed);
				top += d.Height_Renamed + Vgap_Renamed;
			}
			if ((c = GetChild(SOUTH,ltr)) != null)
			{
				c.SetSize(right - left, c.Height_Renamed);
				Dimension d = c.PreferredSize;
				c.SetBounds(left, bottom - d.Height_Renamed, right - left, d.Height_Renamed);
				bottom -= d.Height_Renamed + Vgap_Renamed;
			}
			if ((c = GetChild(EAST,ltr)) != null)
			{
				c.SetSize(c.Width_Renamed, bottom - top);
				Dimension d = c.PreferredSize;
				c.SetBounds(right - d.Width_Renamed, top, d.Width_Renamed, bottom - top);
				right -= d.Width_Renamed + Hgap_Renamed;
			}
			if ((c = GetChild(WEST,ltr)) != null)
			{
				c.SetSize(c.Width_Renamed, bottom - top);
				Dimension d = c.PreferredSize;
				c.SetBounds(left, top, d.Width_Renamed, bottom - top);
				left += d.Width_Renamed + Hgap_Renamed;
			}
			if ((c = GetChild(CENTER,ltr)) != null)
			{
				c.SetBounds(left, top, right - left, bottom - top);
			}
		  }
		}

		/// <summary>
		/// Get the component that corresponds to the given constraint location
		/// </summary>
		/// <param name="key">     The desired absolute position,
		///                  either NORTH, SOUTH, EAST, or WEST. </param>
		/// <param name="ltr">     Is the component line direction left-to-right? </param>
		private Component GetChild(String key, bool ltr)
		{
			Component result = null;

			if (key == NORTH)
			{
				result = (FirstLine != null) ? FirstLine : North;
			}
			else if (key == SOUTH)
			{
				result = (LastLine != null) ? LastLine : South;
			}
			else if (key == WEST)
			{
				result = ltr ? FirstItem : LastItem;
				if (result == null)
				{
					result = West;
				}
			}
			else if (key == EAST)
			{
				result = ltr ? LastItem : FirstItem;
				if (result == null)
				{
					result = East;
				}
			}
			else if (key == CENTER)
			{
				result = Center;
			}
			if (result != null && !result.Visible_Renamed)
			{
				result = null;
			}
			return result;
		}

		/// <summary>
		/// Returns a string representation of the state of this border layout. </summary>
		/// <returns>    a string representation of this border layout. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[hgap=" + Hgap_Renamed + ",vgap=" + Vgap_Renamed + "]";
		}
	}

}