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
	/// The <code>Scrollbar</code> class embodies a scroll bar, a
	/// familiar user-interface object. A scroll bar provides a
	/// convenient means for allowing a user to select from a
	/// range of values. The following three vertical
	/// scroll bars could be used as slider controls to pick
	/// the red, green, and blue components of a color:
	/// <para>
	/// <img src="doc-files/Scrollbar-1.gif" alt="Image shows 3 vertical sliders, side-by-side."
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// Each scroll bar in this example could be created with
	/// code similar to the following:
	/// 
	/// <hr><blockquote><pre>
	/// redSlider=new Scrollbar(Scrollbar.VERTICAL, 0, 1, 0, 255);
	/// add(redSlider);
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// Alternatively, a scroll bar can represent a range of values. For
	/// example, if a scroll bar is used for scrolling through text, the
	/// width of the "bubble" (also called the "thumb" or "scroll box")
	/// can be used to represent the amount of text that is visible.
	/// Here is an example of a scroll bar that represents a range:
	/// </para>
	/// <para>
	/// <img src="doc-files/Scrollbar-2.gif"
	/// alt="Image shows horizontal slider with starting range of 0 and ending range of 300. The slider thumb is labeled 60."
	/// style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// The value range represented by the bubble in this example
	/// is the <em>visible amount</em>. The horizontal scroll bar
	/// in this example could be created with code like the following:
	/// 
	/// <hr><blockquote><pre>
	/// ranger = new Scrollbar(Scrollbar.HORIZONTAL, 0, 60, 0, 300);
	/// add(ranger);
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// Note that the actual maximum value of the scroll bar is the
	/// <code>maximum</code> minus the <code>visible amount</code>.
	/// In the previous example, because the <code>maximum</code> is
	/// 300 and the <code>visible amount</code> is 60, the actual maximum
	/// value is 240.  The range of the scrollbar track is 0 - 300.
	/// The left side of the bubble indicates the value of the
	/// scroll bar.
	/// </para>
	/// <para>
	/// Normally, the user changes the value of the scroll bar by
	/// making a gesture with the mouse. For example, the user can
	/// drag the scroll bar's bubble up and down, or click in the
	/// scroll bar's unit increment or block increment areas. Keyboard
	/// gestures can also be mapped to the scroll bar. By convention,
	/// the <b>Page&nbsp;Up</b> and <b>Page&nbsp;Down</b>
	/// keys are equivalent to clicking in the scroll bar's block
	/// increment and block decrement areas.
	/// </para>
	/// <para>
	/// When the user changes the value of the scroll bar, the scroll bar
	/// receives an instance of <code>AdjustmentEvent</code>.
	/// The scroll bar processes this event, passing it along to
	/// any registered listeners.
	/// </para>
	/// <para>
	/// Any object that wishes to be notified of changes to the
	/// scroll bar's value should implement
	/// <code>AdjustmentListener</code>, an interface defined in
	/// the package <code>java.awt.event</code>.
	/// Listeners can be added and removed dynamically by calling
	/// the methods <code>addAdjustmentListener</code> and
	/// <code>removeAdjustmentListener</code>.
	/// </para>
	/// <para>
	/// The <code>AdjustmentEvent</code> class defines five types
	/// of adjustment event, listed here:
	/// 
	/// <ul>
	/// <li><code>AdjustmentEvent.TRACK</code> is sent out when the
	/// user drags the scroll bar's bubble.
	/// <li><code>AdjustmentEvent.UNIT_INCREMENT</code> is sent out
	/// when the user clicks in the left arrow of a horizontal scroll
	/// bar, or the top arrow of a vertical scroll bar, or makes the
	/// equivalent gesture from the keyboard.
	/// <li><code>AdjustmentEvent.UNIT_DECREMENT</code> is sent out
	/// when the user clicks in the right arrow of a horizontal scroll
	/// bar, or the bottom arrow of a vertical scroll bar, or makes the
	/// equivalent gesture from the keyboard.
	/// <li><code>AdjustmentEvent.BLOCK_INCREMENT</code> is sent out
	/// when the user clicks in the track, to the left of the bubble
	/// on a horizontal scroll bar, or above the bubble on a vertical
	/// scroll bar. By convention, the <b>Page&nbsp;Up</b>
	/// key is equivalent, if the user is using a keyboard that
	/// defines a <b>Page&nbsp;Up</b> key.
	/// <li><code>AdjustmentEvent.BLOCK_DECREMENT</code> is sent out
	/// when the user clicks in the track, to the right of the bubble
	/// on a horizontal scroll bar, or below the bubble on a vertical
	/// scroll bar. By convention, the <b>Page&nbsp;Down</b>
	/// key is equivalent, if the user is using a keyboard that
	/// defines a <b>Page&nbsp;Down</b> key.
	/// </ul>
	/// </para>
	/// <para>
	/// The JDK&nbsp;1.0 event system is supported for backwards
	/// compatibility, but its use with newer versions of the platform is
	/// discouraged. The five types of adjustment events introduced
	/// with JDK&nbsp;1.1 correspond to the five event types
	/// that are associated with scroll bars in previous platform versions.
	/// The following list gives the adjustment event type,
	/// and the corresponding JDK&nbsp;1.0 event type it replaces.
	/// 
	/// <ul>
	/// <li><code>AdjustmentEvent.TRACK</code> replaces
	/// <code>Event.SCROLL_ABSOLUTE</code>
	/// <li><code>AdjustmentEvent.UNIT_INCREMENT</code> replaces
	/// <code>Event.SCROLL_LINE_UP</code>
	/// <li><code>AdjustmentEvent.UNIT_DECREMENT</code> replaces
	/// <code>Event.SCROLL_LINE_DOWN</code>
	/// <li><code>AdjustmentEvent.BLOCK_INCREMENT</code> replaces
	/// <code>Event.SCROLL_PAGE_UP</code>
	/// <li><code>AdjustmentEvent.BLOCK_DECREMENT</code> replaces
	/// <code>Event.SCROLL_PAGE_DOWN</code>
	/// </ul>
	/// </para>
	/// <para>
	/// <b>Note</b>: We recommend using a <code>Scrollbar</code>
	/// for value selection only.  If you want to implement
	/// a scrollable component inside a container, we recommend you use
	/// a <seealso cref="ScrollPane ScrollPane"/>. If you use a
	/// <code>Scrollbar</code> for this purpose, you are likely to
	/// encounter issues with painting, key handling, sizing and
	/// positioning.
	/// 
	/// @author      Sami Shaio
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.event.AdjustmentEvent </seealso>
	/// <seealso cref=         java.awt.event.AdjustmentListener
	/// @since       JDK1.0 </seealso>
	public class Scrollbar : Component, Adjustable, Accessible
	{

		/// <summary>
		/// A constant that indicates a horizontal scroll bar.
		/// </summary>
		public const int Adjustable_Fields;

		/// <summary>
		/// A constant that indicates a vertical scroll bar.
		/// </summary>
		public const int Adjustable_Fields;

		/// <summary>
		/// The value of the <code>Scrollbar</code>.
		/// This property must be greater than or equal to <code>minimum</code>
		/// and less than or equal to
		/// <code>maximum - visibleAmount</code>
		/// 
		/// @serial </summary>
		/// <seealso cref= #getValue </seealso>
		/// <seealso cref= #setValue </seealso>
		internal int Value_Renamed;

		/// <summary>
		/// The maximum value of the <code>Scrollbar</code>.
		/// This value must be greater than the <code>minimum</code>
		/// value.<br>
		/// 
		/// @serial </summary>
		/// <seealso cref= #getMaximum </seealso>
		/// <seealso cref= #setMaximum </seealso>
		internal int Maximum_Renamed;

		/// <summary>
		/// The minimum value of the <code>Scrollbar</code>.
		/// This value must be less than the <code>maximum</code>
		/// value.<br>
		/// 
		/// @serial </summary>
		/// <seealso cref= #getMinimum </seealso>
		/// <seealso cref= #setMinimum </seealso>
		internal int Minimum_Renamed;

		/// <summary>
		/// The size of the <code>Scrollbar</code>'s bubble.
		/// When a scroll bar is used to select a range of values,
		/// the visibleAmount represents the size of this range.
		/// Depending on platform, this may be visually indicated
		/// by the size of the bubble.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getVisibleAmount </seealso>
		/// <seealso cref= #setVisibleAmount </seealso>
		internal int VisibleAmount_Renamed;

		/// <summary>
		/// The <code>Scrollbar</code>'s orientation--being either horizontal
		/// or vertical.
		/// This value should be specified when the scrollbar is created.<BR>
		/// orientation can be either : <code>VERTICAL</code> or
		/// <code>HORIZONTAL</code> only.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getOrientation </seealso>
		/// <seealso cref= #setOrientation </seealso>
		internal int Orientation_Renamed;

		/// <summary>
		/// The amount by which the scrollbar value will change when going
		/// up or down by a line.
		/// This value must be greater than zero.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getLineIncrement </seealso>
		/// <seealso cref= #setLineIncrement </seealso>
		internal int LineIncrement_Renamed = 1;

		/// <summary>
		/// The amount by which the scrollbar value will change when going
		/// up or down by a page.
		/// This value must be greater than zero.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getPageIncrement </seealso>
		/// <seealso cref= #setPageIncrement </seealso>
		internal int PageIncrement_Renamed = 10;

		/// <summary>
		/// The adjusting status of the <code>Scrollbar</code>.
		/// True if the value is in the process of changing as a result of
		/// actions being taken by the user.
		/// </summary>
		/// <seealso cref= #getValueIsAdjusting </seealso>
		/// <seealso cref= #setValueIsAdjusting
		/// @since 1.4 </seealso>
		[NonSerialized]
		internal bool IsAdjusting;

		[NonSerialized]
		internal AdjustmentListener AdjustmentListener;

		private const String @base = "scrollbar";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 8451667562882310543L;

		/// <summary>
		/// Initialize JNI field and method IDs.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		static Scrollbar()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
		}

		/// <summary>
		/// Constructs a new vertical scroll bar.
		/// The default properties of the scroll bar are listed in
		/// the following table:
		/// 
		/// <table border=1 summary="Scrollbar default properties">
		/// <tr>
		///   <th>Property</th>
		///   <th>Description</th>
		///   <th>Default Value</th>
		/// </tr>
		/// <tr>
		///   <td>orientation</td>
		///   <td>indicates whether the scroll bar is vertical
		///   <br>or horizontal</td>
		///   <td><code>Scrollbar.VERTICAL</code></td>
		/// </tr>
		/// <tr>
		///   <td>value</td>
		///   <td>value which controls the location
		///   <br>of the scroll bar's bubble</td>
		///   <td>0</td>
		/// </tr>
		/// <tr>
		///   <td>visible amount</td>
		///   <td>visible amount of the scroll bar's range,
		///   <br>typically represented by the size of the
		///   <br>scroll bar's bubble</td>
		///   <td>10</td>
		/// </tr>
		/// <tr>
		///   <td>minimum</td>
		///   <td>minimum value of the scroll bar</td>
		///   <td>0</td>
		/// </tr>
		/// <tr>
		///   <td>maximum</td>
		///   <td>maximum value of the scroll bar</td>
		///   <td>100</td>
		/// </tr>
		/// <tr>
		///   <td>unit increment</td>
		///   <td>amount the value changes when the
		///   <br>Line Up or Line Down key is pressed,
		///   <br>or when the end arrows of the scrollbar
		///   <br>are clicked </td>
		///   <td>1</td>
		/// </tr>
		/// <tr>
		///   <td>block increment</td>
		///   <td>amount the value changes when the
		///   <br>Page Up or Page Down key is pressed,
		///   <br>or when the scrollbar track is clicked
		///   <br>on either side of the bubble </td>
		///   <td>10</td>
		/// </tr>
		/// </table>
		/// </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Scrollbar() throws HeadlessException
		public Scrollbar() : this(Adjustable_Fields.VERTICAL, 0, 10, 0, 100)
		{
		}

		/// <summary>
		/// Constructs a new scroll bar with the specified orientation.
		/// <para>
		/// The <code>orientation</code> argument must take one of the two
		/// values <code>Scrollbar.HORIZONTAL</code>,
		/// or <code>Scrollbar.VERTICAL</code>,
		/// indicating a horizontal or vertical scroll bar, respectively.
		/// 
		/// </para>
		/// </summary>
		/// <param name="orientation">   indicates the orientation of the scroll bar </param>
		/// <exception cref="IllegalArgumentException">    when an illegal value for
		///                    the <code>orientation</code> argument is supplied </exception>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Scrollbar(int orientation) throws HeadlessException
		public Scrollbar(int orientation) : this(orientation, 0, 10, 0, 100)
		{
		}

		/// <summary>
		/// Constructs a new scroll bar with the specified orientation,
		/// initial value, visible amount, and minimum and maximum values.
		/// <para>
		/// The <code>orientation</code> argument must take one of the two
		/// values <code>Scrollbar.HORIZONTAL</code>,
		/// or <code>Scrollbar.VERTICAL</code>,
		/// indicating a horizontal or vertical scroll bar, respectively.
		/// </para>
		/// <para>
		/// The parameters supplied to this constructor are subject to the
		/// constraints described in <seealso cref="#setValues(int, int, int, int)"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="orientation">   indicates the orientation of the scroll bar. </param>
		/// <param name="value">     the initial value of the scroll bar </param>
		/// <param name="visible">   the visible amount of the scroll bar, typically
		///                      represented by the size of the bubble </param>
		/// <param name="minimum">   the minimum value of the scroll bar </param>
		/// <param name="maximum">   the maximum value of the scroll bar </param>
		/// <exception cref="IllegalArgumentException">    when an illegal value for
		///                    the <code>orientation</code> argument is supplied </exception>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= #setValues </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Scrollbar(int orientation, int value, int visible, int minimum, int maximum) throws HeadlessException
		public Scrollbar(int orientation, int value, int visible, int minimum, int maximum)
		{
			GraphicsEnvironment.CheckHeadless();
			switch (orientation)
			{
			  case Adjustable_Fields.HORIZONTAL:
			  case Adjustable_Fields.VERTICAL:
				this.Orientation_Renamed = orientation;
				break;
			  default:
				throw new IllegalArgumentException("illegal scrollbar orientation");
			}
			SetValues(value, visible, minimum, maximum);
		}

		/// <summary>
		/// Constructs a name for this component.  Called by <code>getName</code>
		/// when the name is <code>null</code>.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(Scrollbar))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the <code>Scrollbar</code>'s peer.  The peer allows you to modify
		/// the appearance of the <code>Scrollbar</code> without changing any of its
		/// functionality.
		/// </summary>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateScrollbar(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// Returns the orientation of this scroll bar.
		/// </summary>
		/// <returns>    the orientation of this scroll bar, either
		///               <code>Scrollbar.HORIZONTAL</code> or
		///               <code>Scrollbar.VERTICAL</code> </returns>
		/// <seealso cref=       java.awt.Scrollbar#setOrientation </seealso>
		public virtual int Orientation
		{
			get
			{
				return Orientation_Renamed;
			}
			set
			{
				lock (TreeLock)
				{
					if (value == this.Orientation_Renamed)
					{
						return;
					}
					switch (value)
					{
						case Adjustable_Fields.HORIZONTAL:
						case Adjustable_Fields.VERTICAL:
							this.Orientation_Renamed = value;
							break;
						default:
							throw new IllegalArgumentException("illegal scrollbar orientation");
					}
					/* Create a new peer with the specified value. */
					if (Peer_Renamed != null)
					{
						RemoveNotify();
						AddNotify();
						Invalidate();
					}
				}
				if (AccessibleContext_Renamed != null)
				{
					AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, ((value == Adjustable_Fields.VERTICAL) ? AccessibleState.HORIZONTAL : AccessibleState.VERTICAL), ((value == Adjustable_Fields.VERTICAL) ? AccessibleState.VERTICAL : AccessibleState.HORIZONTAL));
				}
			}
		}


		/// <summary>
		/// Gets the current value of this scroll bar.
		/// </summary>
		/// <returns>      the current value of this scroll bar </returns>
		/// <seealso cref=         java.awt.Scrollbar#getMinimum </seealso>
		/// <seealso cref=         java.awt.Scrollbar#getMaximum </seealso>
		public virtual int Value
		{
			get
			{
				return Value_Renamed;
			}
			set
			{
				// Use setValues so that a consistent policy relating
				// minimum, maximum, visible amount, and value is enforced.
				SetValues(value, VisibleAmount_Renamed, Minimum_Renamed, Maximum_Renamed);
			}
		}


		/// <summary>
		/// Gets the minimum value of this scroll bar.
		/// </summary>
		/// <returns>      the minimum value of this scroll bar </returns>
		/// <seealso cref=         java.awt.Scrollbar#getValue </seealso>
		/// <seealso cref=         java.awt.Scrollbar#getMaximum </seealso>
		public virtual int Minimum
		{
			get
			{
				return Minimum_Renamed;
			}
			set
			{
				// No checks are necessary in this method since minimum is
				// the first variable checked in the setValues function.
    
				// Use setValues so that a consistent policy relating
				// minimum, maximum, visible amount, and value is enforced.
				SetValues(Value_Renamed, VisibleAmount_Renamed, value, Maximum_Renamed);
			}
		}


		/// <summary>
		/// Gets the maximum value of this scroll bar.
		/// </summary>
		/// <returns>      the maximum value of this scroll bar </returns>
		/// <seealso cref=         java.awt.Scrollbar#getValue </seealso>
		/// <seealso cref=         java.awt.Scrollbar#getMinimum </seealso>
		public virtual int Maximum
		{
			get
			{
				return Maximum_Renamed;
			}
			set
			{
				// minimum is checked first in setValues, so we need to
				// enforce minimum and maximum checks here.
				if (value == Integer.MinValue)
				{
					value = Integer.MinValue + 1;
				}
    
				if (Minimum_Renamed >= value)
				{
					Minimum_Renamed = value - 1;
				}
    
				// Use setValues so that a consistent policy relating
				// minimum, maximum, visible amount, and value is enforced.
				SetValues(Value_Renamed, VisibleAmount_Renamed, Minimum_Renamed, value);
			}
		}


		/// <summary>
		/// Gets the visible amount of this scroll bar.
		/// <para>
		/// When a scroll bar is used to select a range of values,
		/// the visible amount is used to represent the range of values
		/// that are currently visible.  The size of the scroll bar's
		/// bubble (also called a thumb or scroll box), usually gives a
		/// visual representation of the relationship of the visible
		/// amount to the range of the scroll bar.
		/// Note that depending on platform, the value of the visible amount property
		/// may not be visually indicated by the size of the bubble.
		/// </para>
		/// <para>
		/// The scroll bar's bubble may not be displayed when it is not
		/// moveable (e.g. when it takes up the entire length of the
		/// scroll bar's track, or when the scroll bar is disabled).
		/// Whether the bubble is displayed or not will not affect
		/// the value returned by <code>getVisibleAmount</code>.
		/// 
		/// </para>
		/// </summary>
		/// <returns>      the visible amount of this scroll bar </returns>
		/// <seealso cref=         java.awt.Scrollbar#setVisibleAmount
		/// @since       JDK1.1 </seealso>
		public virtual int VisibleAmount
		{
			get
			{
				return Visible;
			}
			set
			{
				// Use setValues so that a consistent policy relating
				// minimum, maximum, visible amount, and value is enforced.
				SetValues(Value_Renamed, value, Minimum_Renamed, Maximum_Renamed);
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getVisibleAmount()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int Visible
		{
			get
			{
				return VisibleAmount_Renamed;
			}
		}


		/// <summary>
		/// Sets the unit increment for this scroll bar.
		/// <para>
		/// The unit increment is the value that is added or subtracted
		/// when the user activates the unit increment area of the
		/// scroll bar, generally through a mouse or keyboard gesture
		/// that the scroll bar receives as an adjustment event.
		/// The unit increment must be greater than zero.
		/// Attepts to set the unit increment to a value lower than 1
		/// will result in a value of 1 being set.
		/// </para>
		/// <para>
		/// In some operating systems, this property
		/// can be ignored by the underlying controls.
		/// 
		/// </para>
		/// </summary>
		/// <param name="v">  the amount by which to increment or decrement
		///                         the scroll bar's value </param>
		/// <seealso cref=          java.awt.Scrollbar#getUnitIncrement
		/// @since        JDK1.1 </seealso>
		public virtual int UnitIncrement
		{
			set
			{
				LineIncrement = value;
			}
			get
			{
				return LineIncrement;
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setUnitIncrement(int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int LineIncrement
		{
			set
			{
				lock (this)
				{
					int tmp = (value < 1) ? 1 : value;
            
					if (LineIncrement_Renamed == tmp)
					{
						return;
					}
					LineIncrement_Renamed = tmp;
            
					ScrollbarPeer peer = (ScrollbarPeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.LineIncrement = LineIncrement_Renamed;
					}
				}
			}
			get
			{
				return LineIncrement_Renamed;
			}
		}



		/// <summary>
		/// Sets the block increment for this scroll bar.
		/// <para>
		/// The block increment is the value that is added or subtracted
		/// when the user activates the block increment area of the
		/// scroll bar, generally through a mouse or keyboard gesture
		/// that the scroll bar receives as an adjustment event.
		/// The block increment must be greater than zero.
		/// Attepts to set the block increment to a value lower than 1
		/// will result in a value of 1 being set.
		/// 
		/// </para>
		/// </summary>
		/// <param name="v">  the amount by which to increment or decrement
		///                         the scroll bar's value </param>
		/// <seealso cref=          java.awt.Scrollbar#getBlockIncrement
		/// @since        JDK1.1 </seealso>
		public virtual int BlockIncrement
		{
			set
			{
				PageIncrement = value;
			}
			get
			{
				return PageIncrement;
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setBlockIncrement()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int PageIncrement
		{
			set
			{
				lock (this)
				{
					int tmp = (value < 1) ? 1 : value;
            
					if (PageIncrement_Renamed == tmp)
					{
						return;
					}
					PageIncrement_Renamed = tmp;
            
					ScrollbarPeer peer = (ScrollbarPeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.PageIncrement = PageIncrement_Renamed;
					}
				}
			}
			get
			{
				return PageIncrement_Renamed;
			}
		}



		/// <summary>
		/// Sets the values of four properties for this scroll bar:
		/// <code>value</code>, <code>visibleAmount</code>,
		/// <code>minimum</code>, and <code>maximum</code>.
		/// If the values supplied for these properties are inconsistent
		/// or incorrect, they will be changed to ensure consistency.
		/// <para>
		/// This method simultaneously and synchronously sets the values
		/// of four scroll bar properties, assuring that the values of
		/// these properties are mutually consistent. It enforces the
		/// following constraints:
		/// <code>maximum</code> must be greater than <code>minimum</code>,
		/// <code>maximum - minimum</code> must not be greater
		///     than <code>Integer.MAX_VALUE</code>,
		/// <code>visibleAmount</code> must be greater than zero.
		/// <code>visibleAmount</code> must not be greater than
		///     <code>maximum - minimum</code>,
		/// <code>value</code> must not be less than <code>minimum</code>,
		/// and <code>value</code> must not be greater than
		///     <code>maximum - visibleAmount</code>
		/// </para>
		/// <para>
		/// Calling this method does not fire an
		/// <code>AdjustmentEvent</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value"> is the position in the current window </param>
		/// <param name="visible"> is the visible amount of the scroll bar </param>
		/// <param name="minimum"> is the minimum value of the scroll bar </param>
		/// <param name="maximum"> is the maximum value of the scroll bar </param>
		/// <seealso cref=        #setMinimum </seealso>
		/// <seealso cref=        #setMaximum </seealso>
		/// <seealso cref=        #setVisibleAmount </seealso>
		/// <seealso cref=        #setValue </seealso>
		public virtual void SetValues(int value, int visible, int minimum, int maximum)
		{
			int oldValue;
			lock (this)
			{
				if (minimum == Integer.MaxValue)
				{
					minimum = Integer.MaxValue - 1;
				}
				if (maximum <= minimum)
				{
					maximum = minimum + 1;
				}

				long maxMinusMin = (long) maximum - (long) minimum;
				if (maxMinusMin > Integer.MaxValue)
				{
					maxMinusMin = Integer.MaxValue;
					maximum = minimum + (int) maxMinusMin;
				}
				if (visible > (int) maxMinusMin)
				{
					visible = (int) maxMinusMin;
				}
				if (visible < 1)
				{
					visible = 1;
				}

				if (value < minimum)
				{
					value = minimum;
				}
				if (value > maximum - visible)
				{
					value = maximum - visible;
				}

				oldValue = this.Value_Renamed;
				this.Value_Renamed = value;
				this.VisibleAmount_Renamed = visible;
				this.Minimum_Renamed = minimum;
				this.Maximum_Renamed = maximum;
				ScrollbarPeer peer = (ScrollbarPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.SetValues(value, VisibleAmount_Renamed, minimum, maximum);
				}
			}

			if ((oldValue != value) && (AccessibleContext_Renamed != null))
			{
				AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, Convert.ToInt32(oldValue), Convert.ToInt32(value));
			}
		}

		/// <summary>
		/// Returns true if the value is in the process of changing as a
		/// result of actions being taken by the user.
		/// </summary>
		/// <returns> the value of the <code>valueIsAdjusting</code> property </returns>
		/// <seealso cref= #setValueIsAdjusting
		/// @since 1.4 </seealso>
		public virtual bool ValueIsAdjusting
		{
			get
			{
				return IsAdjusting;
			}
			set
			{
				bool oldValue;
    
				lock (this)
				{
					oldValue = IsAdjusting;
					IsAdjusting = value;
				}
    
				if ((oldValue != value) && (AccessibleContext_Renamed != null))
				{
					AccessibleContext_Renamed.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, ((oldValue) ? AccessibleState.BUSY : null), ((value) ? AccessibleState.BUSY : null));
				}
			}
		}




		/// <summary>
		/// Adds the specified adjustment listener to receive instances of
		/// <code>AdjustmentEvent</code> from this scroll bar.
		/// If l is <code>null</code>, no exception is thrown and no
		/// action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the adjustment listener </param>
		/// <seealso cref=          #removeAdjustmentListener </seealso>
		/// <seealso cref=          #getAdjustmentListeners </seealso>
		/// <seealso cref=          java.awt.event.AdjustmentEvent </seealso>
		/// <seealso cref=          java.awt.event.AdjustmentListener
		/// @since        JDK1.1 </seealso>
		public virtual void AddAdjustmentListener(AdjustmentListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				AdjustmentListener = AWTEventMulticaster.Add(AdjustmentListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Removes the specified adjustment listener so that it no longer
		/// receives instances of <code>AdjustmentEvent</code> from this scroll bar.
		/// If l is <code>null</code>, no exception is thrown and no action
		/// is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">    the adjustment listener </param>
		/// <seealso cref=             #addAdjustmentListener </seealso>
		/// <seealso cref=             #getAdjustmentListeners </seealso>
		/// <seealso cref=             java.awt.event.AdjustmentEvent </seealso>
		/// <seealso cref=             java.awt.event.AdjustmentListener
		/// @since           JDK1.1 </seealso>
		public virtual void RemoveAdjustmentListener(AdjustmentListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				AdjustmentListener = AWTEventMulticaster.Remove(AdjustmentListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the adjustment listeners
		/// registered on this scrollbar.
		/// </summary>
		/// <returns> all of this scrollbar's <code>AdjustmentListener</code>s
		///         or an empty array if no adjustment
		///         listeners are currently registered </returns>
		/// <seealso cref=             #addAdjustmentListener </seealso>
		/// <seealso cref=             #removeAdjustmentListener </seealso>
		/// <seealso cref=             java.awt.event.AdjustmentEvent </seealso>
		/// <seealso cref=             java.awt.event.AdjustmentListener
		/// @since 1.4 </seealso>
		public virtual AdjustmentListener[] AdjustmentListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(AdjustmentListener));
				}
			}
		}

		/// <summary>
		/// Returns an array of all the objects currently registered
		/// as <code><em>Foo</em>Listener</code>s
		/// upon this <code>Scrollbar</code>.
		/// <code><em>Foo</em>Listener</code>s are registered using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// <para>
		/// You can specify the <code>listenerType</code> argument
		/// with a class literal,  such as
		/// <code><em>Foo</em>Listener.class</code>.
		/// For example, you can query a
		/// <code>Scrollbar</code> <code>c</code>
		/// for its mouse listeners with the following code:
		/// 
		/// <pre>MouseListener[] mls = (MouseListener[])(c.getListeners(MouseListener.class));</pre>
		/// 
		/// If no such listeners exist, this method returns an empty array.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listenerType"> the type of listeners requested; this parameter
		///          should specify an interface that descends from
		///          <code>java.util.EventListener</code> </param>
		/// <returns> an array of all objects registered as
		///          <code><em>Foo</em>Listener</code>s on this component,
		///          or an empty array if no such listeners have been added </returns>
		/// <exception cref="ClassCastException"> if <code>listenerType</code>
		///          doesn't specify a class or interface that implements
		///          <code>java.util.EventListener</code>
		/// 
		/// @since 1.3 </exception>
		public override T[] getListeners<T>(Class listenerType) where T : java.util.EventListener
		{
			EventListener l = null;
			if (listenerType == typeof(AdjustmentListener))
			{
				l = AdjustmentListener;
			}
			else
			{
				return base.GetListeners(listenerType);
			}
			return AWTEventMulticaster.GetListeners(l, listenerType);
		}

		// REMIND: remove when filtering is done at lower level
		internal override bool EventEnabled(AWTEvent e)
		{
			if (e.Id == AdjustmentEvent.ADJUSTMENT_VALUE_CHANGED)
			{
				if ((EventMask & AWTEvent.ADJUSTMENT_EVENT_MASK) != 0 || AdjustmentListener != null)
				{
					return true;
				}
				return false;
			}
			return base.EventEnabled(e);
		}

		/// <summary>
		/// Processes events on this scroll bar. If the event is an
		/// instance of <code>AdjustmentEvent</code>, it invokes the
		/// <code>processAdjustmentEvent</code> method.
		/// Otherwise, it invokes its superclass's
		/// <code>processEvent</code> method.
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the event </param>
		/// <seealso cref=          java.awt.event.AdjustmentEvent </seealso>
		/// <seealso cref=          java.awt.Scrollbar#processAdjustmentEvent
		/// @since        JDK1.1 </seealso>
		protected internal override void ProcessEvent(AWTEvent e)
		{
			if (e is AdjustmentEvent)
			{
				ProcessAdjustmentEvent((AdjustmentEvent)e);
				return;
			}
			base.ProcessEvent(e);
		}

		/// <summary>
		/// Processes adjustment events occurring on this
		/// scrollbar by dispatching them to any registered
		/// <code>AdjustmentListener</code> objects.
		/// <para>
		/// This method is not called unless adjustment events are
		/// enabled for this component. Adjustment events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>An <code>AdjustmentListener</code> object is registered
		/// via <code>addAdjustmentListener</code>.
		/// <li>Adjustment events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the adjustment event </param>
		/// <seealso cref=         java.awt.event.AdjustmentEvent </seealso>
		/// <seealso cref=         java.awt.event.AdjustmentListener </seealso>
		/// <seealso cref=         java.awt.Scrollbar#addAdjustmentListener </seealso>
		/// <seealso cref=         java.awt.Component#enableEvents
		/// @since       JDK1.1 </seealso>
		protected internal virtual void ProcessAdjustmentEvent(AdjustmentEvent e)
		{
			AdjustmentListener listener = AdjustmentListener;
			if (listener != null)
			{
				listener.AdjustmentValueChanged(e);
			}
		}

		/// <summary>
		/// Returns a string representing the state of this <code>Scrollbar</code>.
		/// This method is intended to be used only for debugging purposes, and the
		/// content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>      the parameter string of this scroll bar </returns>
		protected internal override String ParamString()
		{
			return base.ParamString() + ",val=" + Value_Renamed + ",vis=" + VisibleAmount_Renamed + ",min=" + Minimum_Renamed + ",max=" + Maximum_Renamed + ((Orientation_Renamed == Adjustable_Fields.VERTICAL) ? ",vert" : ",horz") + ",isAdjusting=" + IsAdjusting;
		}


		/* Serialization support.
		 */

		/// <summary>
		/// The scroll bar's serialized Data Version.
		/// 
		/// @serial
		/// </summary>
		private int ScrollbarSerializedDataVersion = 1;

		/// <summary>
		/// Writes default serializable fields to stream.  Writes
		/// a list of serializable <code>AdjustmentListeners</code>
		/// as optional data. The non-serializable listeners are
		/// detected and no attempt is made to serialize them.
		/// </summary>
		/// <param name="s"> the <code>ObjectOutputStream</code> to write
		/// @serialData <code>null</code> terminated sequence of 0
		///   or more pairs; the pair consists of a <code>String</code>
		///   and an <code>Object</code>; the <code>String</code> indicates
		///   the type of object and is one of the following:
		///   <code>adjustmentListenerK</code> indicating an
		///     <code>AdjustmentListener</code> object
		/// </param>
		/// <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		/// <seealso cref= java.awt.Component#adjustmentListenerK </seealso>
		/// <seealso cref= #readObject(ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
		  s.DefaultWriteObject();

		  AWTEventMulticaster.Save(s, AdjustmentListenerK, AdjustmentListener);
		  s.WriteObject(null);
		}

		/// <summary>
		/// Reads the <code>ObjectInputStream</code> and if
		/// it isn't <code>null</code> adds a listener to
		/// receive adjustment events fired by the
		/// <code>Scrollbar</code>.
		/// Unrecognized keys or values will be ignored.
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to read </param>
		/// <exception cref="HeadlessException"> if
		///   <code>GraphicsEnvironment.isHeadless</code> returns
		///   <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= #writeObject(ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
		  GraphicsEnvironment.CheckHeadless();
		  s.DefaultReadObject();

		  Object keyOrNull;
		  while (null != (keyOrNull = s.ReadObject()))
		  {
			String key = ((String)keyOrNull).intern();

			if (AdjustmentListenerK == key)
			{
			  AddAdjustmentListener((AdjustmentListener)(s.ReadObject()));
			}

			else // skip value for unrecognized key
			{
			  s.ReadObject();
			}
		  }
		}


	/////////////////
	// Accessibility support
	////////////////

		/// <summary>
		/// Gets the <code>AccessibleContext</code> associated with this
		/// <code>Scrollbar</code>. For scrollbars, the
		/// <code>AccessibleContext</code> takes the form of an
		/// <code>AccessibleAWTScrollBar</code>. A new
		/// <code>AccessibleAWTScrollBar</code> instance is created if necessary.
		/// </summary>
		/// <returns> an <code>AccessibleAWTScrollBar</code> that serves as the
		///         <code>AccessibleContext</code> of this <code>ScrollBar</code>
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTScrollBar(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>Scrollbar</code> class.  It provides an implementation of
		/// the Java Accessibility API appropriate to scrollbar
		/// user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTScrollBar : AccessibleAWTComponent, AccessibleValue
		{
			private readonly Scrollbar OuterInstance;

			public AccessibleAWTScrollBar(Scrollbar outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = -344337268523697807L;

			/// <summary>
			/// Get the state set of this object.
			/// </summary>
			/// <returns> an instance of <code>AccessibleState</code>
			///     containing the current state of the object </returns>
			/// <seealso cref= AccessibleState </seealso>
			public override AccessibleStateSet AccessibleStateSet
			{
				get
				{
					AccessibleStateSet states = base.AccessibleStateSet;
					if (outerInstance.ValueIsAdjusting)
					{
						states.add(AccessibleState.BUSY);
					}
					if (outerInstance.Orientation == Adjustable_Fields.VERTICAL)
					{
						states.add(AccessibleState.VERTICAL);
					}
					else
					{
						states.add(AccessibleState.HORIZONTAL);
					}
					return states;
				}
			}

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of <code>AccessibleRole</code>
			///     describing the role of the object </returns>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.SCROLL_BAR;
				}
			}

			/// <summary>
			/// Get the <code>AccessibleValue</code> associated with this
			/// object.  In the implementation of the Java Accessibility
			/// API for this class, return this object, which is
			/// responsible for implementing the
			/// <code>AccessibleValue</code> interface on behalf of itself.
			/// </summary>
			/// <returns> this object </returns>
			public virtual AccessibleValue AccessibleValue
			{
				get
				{
					return this;
				}
			}

			/// <summary>
			/// Get the accessible value of this object.
			/// </summary>
			/// <returns> The current value of this object. </returns>
			public virtual Number CurrentAccessibleValue
			{
				get
				{
					return Convert.ToInt32(outerInstance.Value);
				}
			}

			/// <summary>
			/// Set the value of this object as a Number.
			/// </summary>
			/// <returns> True if the value was set. </returns>
			public virtual bool SetCurrentAccessibleValue(Number n)
			{
				if (n is Integer)
				{
					outerInstance.Value = n.IntValue();
					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// Get the minimum accessible value of this object.
			/// </summary>
			/// <returns> The minimum value of this object. </returns>
			public virtual Number MinimumAccessibleValue
			{
				get
				{
					return Convert.ToInt32(outerInstance.Minimum);
				}
			}

			/// <summary>
			/// Get the maximum accessible value of this object.
			/// </summary>
			/// <returns> The maximum value of this object. </returns>
			public virtual Number MaximumAccessibleValue
			{
				get
				{
					return Convert.ToInt32(outerInstance.Maximum);
				}
			}

		} // AccessibleAWTScrollBar

	}

}