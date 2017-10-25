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
	/// A flow layout arranges components in a directional flow, much
	/// like lines of text in a paragraph. The flow direction is
	/// determined by the container's <code>componentOrientation</code>
	/// property and may be one of two values:
	/// <ul>
	/// <li><code>ComponentOrientation.LEFT_TO_RIGHT</code>
	/// <li><code>ComponentOrientation.RIGHT_TO_LEFT</code>
	/// </ul>
	/// Flow layouts are typically used
	/// to arrange buttons in a panel. It arranges buttons
	/// horizontally until no more buttons fit on the same line.
	/// The line alignment is determined by the <code>align</code>
	/// property. The possible values are:
	/// <ul>
	/// <li><seealso cref="#LEFT LEFT"/>
	/// <li><seealso cref="#RIGHT RIGHT"/>
	/// <li><seealso cref="#CENTER CENTER"/>
	/// <li><seealso cref="#LEADING LEADING"/>
	/// <li><seealso cref="#TRAILING TRAILING"/>
	/// </ul>
	/// <para>
	/// For example, the following picture shows an applet using the flow
	/// layout manager (its default layout manager) to position three buttons:
	/// </para>
	/// <para>
	/// <img src="doc-files/FlowLayout-1.gif"
	/// ALT="Graphic of Layout for Three Buttons"
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// Here is the code for this applet:
	/// 
	/// <hr><blockquote><pre>
	/// import java.awt.*;
	/// import java.applet.Applet;
	/// 
	/// public class myButtons extends Applet {
	///     Button button1, button2, button3;
	///     public void init() {
	///         button1 = new Button("Ok");
	///         button2 = new Button("Open");
	///         button3 = new Button("Close");
	///         add(button1);
	///         add(button2);
	///         add(button3);
	///     }
	/// }
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// A flow layout lets each component assume its natural (preferred) size.
	/// 
	/// @author      Arthur van Hoff
	/// @author      Sami Shaio
	/// @since       JDK1.0
	/// </para>
	/// </summary>
	/// <seealso cref= ComponentOrientation </seealso>
	[Serializable]
	public class FlowLayout : LayoutManager
	{

		/// <summary>
		/// This value indicates that each row of components
		/// should be left-justified.
		/// </summary>
		public const int LEFT = 0;

		/// <summary>
		/// This value indicates that each row of components
		/// should be centered.
		/// </summary>
		public const int CENTER = 1;

		/// <summary>
		/// This value indicates that each row of components
		/// should be right-justified.
		/// </summary>
		public const int RIGHT = 2;

		/// <summary>
		/// This value indicates that each row of components
		/// should be justified to the leading edge of the container's
		/// orientation, for example, to the left in left-to-right orientations.
		/// </summary>
		/// <seealso cref=     java.awt.Component#getComponentOrientation </seealso>
		/// <seealso cref=     java.awt.ComponentOrientation
		/// @since   1.2 </seealso>
		public const int LEADING = 3;

		/// <summary>
		/// This value indicates that each row of components
		/// should be justified to the trailing edge of the container's
		/// orientation, for example, to the right in left-to-right orientations.
		/// </summary>
		/// <seealso cref=     java.awt.Component#getComponentOrientation </seealso>
		/// <seealso cref=     java.awt.ComponentOrientation
		/// @since   1.2 </seealso>
		public const int TRAILING = 4;

		/// <summary>
		/// <code>align</code> is the property that determines
		/// how each row distributes empty space.
		/// It can be one of the following values:
		/// <ul>
		/// <li><code>LEFT</code>
		/// <li><code>RIGHT</code>
		/// <li><code>CENTER</code>
		/// </ul>
		/// 
		/// @serial </summary>
		/// <seealso cref= #getAlignment </seealso>
		/// <seealso cref= #setAlignment </seealso>
		internal int Align; // This is for 1.1 serialization compatibility

		/// <summary>
		/// <code>newAlign</code> is the property that determines
		/// how each row distributes empty space for the Java 2 platform,
		/// v1.2 and greater.
		/// It can be one of the following three values:
		/// <ul>
		/// <li><code>LEFT</code>
		/// <li><code>RIGHT</code>
		/// <li><code>CENTER</code>
		/// <li><code>LEADING</code>
		/// <li><code>TRAILING</code>
		/// </ul>
		/// 
		/// @serial
		/// @since 1.2 </summary>
		/// <seealso cref= #getAlignment </seealso>
		/// <seealso cref= #setAlignment </seealso>
		internal int NewAlign; // This is the one we actually use

		/// <summary>
		/// The flow layout manager allows a seperation of
		/// components with gaps.  The horizontal gap will
		/// specify the space between components and between
		/// the components and the borders of the
		/// <code>Container</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getHgap() </seealso>
		/// <seealso cref= #setHgap(int) </seealso>
		internal int Hgap_Renamed;

		/// <summary>
		/// The flow layout manager allows a seperation of
		/// components with gaps.  The vertical gap will
		/// specify the space between rows and between the
		/// the rows and the borders of the <code>Container</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getHgap() </seealso>
		/// <seealso cref= #setHgap(int) </seealso>
		internal int Vgap_Renamed;

		/// <summary>
		/// If true, components will be aligned on their baseline.
		/// </summary>
		private bool AlignOnBaseline_Renamed;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = -7262534875583282631L;

		/// <summary>
		/// Constructs a new <code>FlowLayout</code> with a centered alignment and a
		/// default 5-unit horizontal and vertical gap.
		/// </summary>
		public FlowLayout() : this(CENTER, 5, 5)
		{
		}

		/// <summary>
		/// Constructs a new <code>FlowLayout</code> with the specified
		/// alignment and a default 5-unit horizontal and vertical gap.
		/// The value of the alignment argument must be one of
		/// <code>FlowLayout.LEFT</code>, <code>FlowLayout.RIGHT</code>,
		/// <code>FlowLayout.CENTER</code>, <code>FlowLayout.LEADING</code>,
		/// or <code>FlowLayout.TRAILING</code>. </summary>
		/// <param name="align"> the alignment value </param>
		public FlowLayout(int align) : this(align, 5, 5)
		{
		}

		/// <summary>
		/// Creates a new flow layout manager with the indicated alignment
		/// and the indicated horizontal and vertical gaps.
		/// <para>
		/// The value of the alignment argument must be one of
		/// <code>FlowLayout.LEFT</code>, <code>FlowLayout.RIGHT</code>,
		/// <code>FlowLayout.CENTER</code>, <code>FlowLayout.LEADING</code>,
		/// or <code>FlowLayout.TRAILING</code>.
		/// </para>
		/// </summary>
		/// <param name="align">   the alignment value </param>
		/// <param name="hgap">    the horizontal gap between components
		///                     and between the components and the
		///                     borders of the <code>Container</code> </param>
		/// <param name="vgap">    the vertical gap between components
		///                     and between the components and the
		///                     borders of the <code>Container</code> </param>
		public FlowLayout(int align, int hgap, int vgap)
		{
			this.Hgap_Renamed = hgap;
			this.Vgap_Renamed = vgap;
			Alignment = align;
		}

		/// <summary>
		/// Gets the alignment for this layout.
		/// Possible values are <code>FlowLayout.LEFT</code>,
		/// <code>FlowLayout.RIGHT</code>, <code>FlowLayout.CENTER</code>,
		/// <code>FlowLayout.LEADING</code>,
		/// or <code>FlowLayout.TRAILING</code>. </summary>
		/// <returns>     the alignment value for this layout </returns>
		/// <seealso cref=        java.awt.FlowLayout#setAlignment
		/// @since      JDK1.1 </seealso>
		public virtual int Alignment
		{
			get
			{
				return NewAlign;
			}
			set
			{
				this.NewAlign = value;
    
				// this.align is used only for serialization compatibility,
				// so set it to a value compatible with the 1.1 version
				// of the class
    
				switch (value)
				{
				case LEADING:
					this.Align = LEFT;
					break;
				case TRAILING:
					this.Align = RIGHT;
					break;
				default:
					this.Align = value;
					break;
				}
			}
		}


		/// <summary>
		/// Gets the horizontal gap between components
		/// and between the components and the borders
		/// of the <code>Container</code>
		/// </summary>
		/// <returns>     the horizontal gap between components
		///             and between the components and the borders
		///             of the <code>Container</code> </returns>
		/// <seealso cref=        java.awt.FlowLayout#setHgap
		/// @since      JDK1.1 </seealso>
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
		/// Gets the vertical gap between components and
		/// between the components and the borders of the
		/// <code>Container</code>.
		/// </summary>
		/// <returns>     the vertical gap between components
		///             and between the components and the borders
		///             of the <code>Container</code> </returns>
		/// <seealso cref=        java.awt.FlowLayout#setVgap
		/// @since      JDK1.1 </seealso>
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
		/// Sets whether or not components should be vertically aligned along their
		/// baseline.  Components that do not have a baseline will be centered.
		/// The default is false.
		/// </summary>
		/// <param name="alignOnBaseline"> whether or not components should be
		///                        vertically aligned on their baseline
		/// @since 1.6 </param>
		public virtual bool AlignOnBaseline
		{
			set
			{
				this.AlignOnBaseline_Renamed = value;
			}
			get
			{
				return AlignOnBaseline_Renamed;
			}
		}


		/// <summary>
		/// Adds the specified component to the layout.
		/// Not used by this class. </summary>
		/// <param name="name"> the name of the component </param>
		/// <param name="comp"> the component to be added </param>
		public virtual void AddLayoutComponent(String name, Component comp)
		{
		}

		/// <summary>
		/// Removes the specified component from the layout.
		/// Not used by this class. </summary>
		/// <param name="comp"> the component to remove </param>
		/// <seealso cref=       java.awt.Container#removeAll </seealso>
		public virtual void RemoveLayoutComponent(Component comp)
		{
		}

		/// <summary>
		/// Returns the preferred dimensions for this layout given the
		/// <i>visible</i> components in the specified target container.
		/// </summary>
		/// <param name="target"> the container that needs to be laid out </param>
		/// <returns>    the preferred dimensions to lay out the
		///            subcomponents of the specified container </returns>
		/// <seealso cref= Container </seealso>
		/// <seealso cref= #minimumLayoutSize </seealso>
		/// <seealso cref=       java.awt.Container#getPreferredSize </seealso>
		public virtual Dimension PreferredLayoutSize(Container target)
		{
		  lock (target.TreeLock)
		  {
			Dimension dim = new Dimension(0, 0);
			int nmembers = target.ComponentCount;
			bool firstVisibleComponent = true;
			bool useBaseline = AlignOnBaseline;
			int maxAscent = 0;
			int maxDescent = 0;

			for (int i = 0 ; i < nmembers ; i++)
			{
				Component m = target.GetComponent(i);
				if (m.Visible)
				{
					Dimension d = m.PreferredSize;
					dim.Height_Renamed = System.Math.Max(dim.Height_Renamed, d.Height_Renamed);
					if (firstVisibleComponent)
					{
						firstVisibleComponent = false;
					}
					else
					{
						dim.Width_Renamed += Hgap_Renamed;
					}
					dim.Width_Renamed += d.Width_Renamed;
					if (useBaseline)
					{
						int baseline = m.GetBaseline(d.Width_Renamed, d.Height_Renamed);
						if (baseline >= 0)
						{
							maxAscent = System.Math.Max(maxAscent, baseline);
							maxDescent = System.Math.Max(maxDescent, d.Height_Renamed - baseline);
						}
					}
				}
			}
			if (useBaseline)
			{
				dim.Height_Renamed = System.Math.Max(maxAscent + maxDescent, dim.Height_Renamed);
			}
			Insets insets = target.Insets;
			dim.Width_Renamed += insets.Left + insets.Right + Hgap_Renamed * 2;
			dim.Height_Renamed += insets.Top + insets.Bottom + Vgap_Renamed * 2;
			return dim;
		  }
		}

		/// <summary>
		/// Returns the minimum dimensions needed to layout the <i>visible</i>
		/// components contained in the specified target container. </summary>
		/// <param name="target"> the container that needs to be laid out </param>
		/// <returns>    the minimum dimensions to lay out the
		///            subcomponents of the specified container </returns>
		/// <seealso cref= #preferredLayoutSize </seealso>
		/// <seealso cref=       java.awt.Container </seealso>
		/// <seealso cref=       java.awt.Container#doLayout </seealso>
		public virtual Dimension MinimumLayoutSize(Container target)
		{
		  lock (target.TreeLock)
		  {
			bool useBaseline = AlignOnBaseline;
			Dimension dim = new Dimension(0, 0);
			int nmembers = target.ComponentCount;
			int maxAscent = 0;
			int maxDescent = 0;
			bool firstVisibleComponent = true;

			for (int i = 0 ; i < nmembers ; i++)
			{
				Component m = target.GetComponent(i);
				if (m.Visible_Renamed)
				{
					Dimension d = m.MinimumSize;
					dim.Height_Renamed = System.Math.Max(dim.Height_Renamed, d.Height_Renamed);
					if (firstVisibleComponent)
					{
						firstVisibleComponent = false;
					}
					else
					{
						dim.Width_Renamed += Hgap_Renamed;
					}
					dim.Width_Renamed += d.Width_Renamed;
					if (useBaseline)
					{
						int baseline = m.GetBaseline(d.Width_Renamed, d.Height_Renamed);
						if (baseline >= 0)
						{
							maxAscent = System.Math.Max(maxAscent, baseline);
							maxDescent = System.Math.Max(maxDescent, dim.Height_Renamed - baseline);
						}
					}
				}
			}

			if (useBaseline)
			{
				dim.Height_Renamed = System.Math.Max(maxAscent + maxDescent, dim.Height_Renamed);
			}

			Insets insets = target.Insets;
			dim.Width_Renamed += insets.Left + insets.Right + Hgap_Renamed * 2;
			dim.Height_Renamed += insets.Top + insets.Bottom + Vgap_Renamed * 2;
			return dim;





		  }
		}

		/// <summary>
		/// Centers the elements in the specified row, if there is any slack. </summary>
		/// <param name="target"> the component which needs to be moved </param>
		/// <param name="x"> the x coordinate </param>
		/// <param name="y"> the y coordinate </param>
		/// <param name="width"> the width dimensions </param>
		/// <param name="height"> the height dimensions </param>
		/// <param name="rowStart"> the beginning of the row </param>
		/// <param name="rowEnd"> the the ending of the row </param>
		/// <param name="useBaseline"> Whether or not to align on baseline. </param>
		/// <param name="ascent"> Ascent for the components. This is only valid if
		///               useBaseline is true. </param>
		/// <param name="descent"> Ascent for the components. This is only valid if
		///               useBaseline is true. </param>
		/// <returns> actual row height </returns>
		private int MoveComponents(Container target, int x, int y, int width, int height, int rowStart, int rowEnd, bool ltr, bool useBaseline, int[] ascent, int[] descent)
		{
			switch (NewAlign)
			{
			case LEFT:
				x += ltr ? 0 : width;
				break;
			case CENTER:
				x += width / 2;
				break;
			case RIGHT:
				x += ltr ? width : 0;
				break;
			case LEADING:
				break;
			case TRAILING:
				x += width;
				break;
			}
			int maxAscent = 0;
			int nonbaselineHeight = 0;
			int baselineOffset = 0;
			if (useBaseline)
			{
				int maxDescent = 0;
				for (int i = rowStart ; i < rowEnd ; i++)
				{
					Component m = target.GetComponent(i);
					if (m.Visible_Renamed)
					{
						if (ascent[i] >= 0)
						{
							maxAscent = System.Math.Max(maxAscent, ascent[i]);
							maxDescent = System.Math.Max(maxDescent, descent[i]);
						}
						else
						{
							nonbaselineHeight = System.Math.Max(m.Height, nonbaselineHeight);
						}
					}
				}
				height = System.Math.Max(maxAscent + maxDescent, nonbaselineHeight);
				baselineOffset = (height - maxAscent - maxDescent) / 2;
			}
			for (int i = rowStart ; i < rowEnd ; i++)
			{
				Component m = target.GetComponent(i);
				if (m.Visible)
				{
					int cy;
					if (useBaseline && ascent[i] >= 0)
					{
						cy = y + baselineOffset + maxAscent - ascent[i];
					}
					else
					{
						cy = y + (height - m.Height_Renamed) / 2;
					}
					if (ltr)
					{
						m.SetLocation(x, cy);
					}
					else
					{
						m.SetLocation(target.Width_Renamed - x - m.Width_Renamed, cy);
					}
					x += m.Width_Renamed + Hgap_Renamed;
				}
			}
			return height;
		}

		/// <summary>
		/// Lays out the container. This method lets each
		/// <i>visible</i> component take
		/// its preferred size by reshaping the components in the
		/// target container in order to satisfy the alignment of
		/// this <code>FlowLayout</code> object.
		/// </summary>
		/// <param name="target"> the specified component being laid out </param>
		/// <seealso cref= Container </seealso>
		/// <seealso cref=       java.awt.Container#doLayout </seealso>
		public virtual void LayoutContainer(Container target)
		{
		  lock (target.TreeLock)
		  {
			Insets insets = target.Insets;
			int maxwidth = target.Width_Renamed - (insets.Left + insets.Right + Hgap_Renamed * 2);
			int nmembers = target.ComponentCount;
			int x = 0, y = insets.Top + Vgap_Renamed;
			int rowh = 0, start = 0;

			bool ltr = target.ComponentOrientation.LeftToRight;

			bool useBaseline = AlignOnBaseline;
			int[] ascent = null;
			int[] descent = null;

			if (useBaseline)
			{
				ascent = new int[nmembers];
				descent = new int[nmembers];
			}

			for (int i = 0 ; i < nmembers ; i++)
			{
				Component m = target.GetComponent(i);
				if (m.Visible)
				{
					Dimension d = m.PreferredSize;
					m.SetSize(d.Width_Renamed, d.Height_Renamed);

					if (useBaseline)
					{
						int baseline = m.GetBaseline(d.Width_Renamed, d.Height_Renamed);
						if (baseline >= 0)
						{
							ascent[i] = baseline;
							descent[i] = d.Height_Renamed - baseline;
						}
						else
						{
							ascent[i] = -1;
						}
					}
					if ((x == 0) || ((x + d.Width_Renamed) <= maxwidth))
					{
						if (x > 0)
						{
							x += Hgap_Renamed;
						}
						x += d.Width_Renamed;
						rowh = System.Math.Max(rowh, d.Height_Renamed);
					}
					else
					{
						rowh = MoveComponents(target, insets.Left + Hgap_Renamed, y, maxwidth - x, rowh, start, i, ltr, useBaseline, ascent, descent);
						x = d.Width_Renamed;
						y += Vgap_Renamed + rowh;
						rowh = d.Height_Renamed;
						start = i;
					}
				}
			}
			MoveComponents(target, insets.Left + Hgap_Renamed, y, maxwidth - x, rowh, start, nmembers, ltr, useBaseline, ascent, descent);
		  }
		}

		//
		// the internal serial version which says which version was written
		// - 0 (default) for versions before the Java 2 platform, v1.2
		// - 1 for version >= Java 2 platform v1.2, which includes "newAlign" field
		//
		private const int CurrentSerialVersion = 1;
		/// <summary>
		/// This represent the <code>currentSerialVersion</code>
		/// which is bein used.  It will be one of two values :
		/// <code>0</code> versions before Java 2 platform v1.2..
		/// <code>1</code> versions after  Java 2 platform v1.2..
		/// 
		/// @serial
		/// @since 1.2
		/// </summary>
		private int SerialVersionOnStream = CurrentSerialVersion;

		/// <summary>
		/// Reads this object out of a serialization stream, handling
		/// objects written by older versions of the class that didn't contain all
		/// of the fields we use now..
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();

			if (SerialVersionOnStream < 1)
			{
				// "newAlign" field wasn't present, so use the old "align" field.
				Alignment = this.Align;
			}
			SerialVersionOnStream = CurrentSerialVersion;
		}

		/// <summary>
		/// Returns a string representation of this <code>FlowLayout</code>
		/// object and its values. </summary>
		/// <returns>     a string representation of this layout </returns>
		public override String ToString()
		{
			String str = "";
			switch (Align)
			{
			  case LEFT:
				  str = ",align=left";
				  break;
			  case CENTER:
				  str = ",align=center";
				  break;
			  case RIGHT:
				  str = ",align=right";
				  break;
			  case LEADING:
				  str = ",align=leading";
				  break;
			  case TRAILING:
				  str = ",align=trailing";
				  break;
			}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[hgap=" + Hgap_Renamed + ",vgap=" + Vgap_Renamed + str + "]";
		}


	}

}