using System;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using AWTAccessor = sun.awt.AWTAccessor;



	/// <summary>
	/// This class represents the state of a horizontal or vertical
	/// scrollbar of a <code>ScrollPane</code>.  Objects of this class are
	/// returned by <code>ScrollPane</code> methods.
	/// 
	/// @since       1.4
	/// </summary>
	[Serializable]
	public class ScrollPaneAdjustable : Adjustable
	{

		/// <summary>
		/// The <code>ScrollPane</code> this object is a scrollbar of.
		/// @serial
		/// </summary>
		private ScrollPane Sp;

		/// <summary>
		/// Orientation of this scrollbar.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getOrientation </seealso>
		/// <seealso cref= java.awt.Adjustable#HORIZONTAL </seealso>
		/// <seealso cref= java.awt.Adjustable#VERTICAL </seealso>
		private int Orientation_Renamed;

		/// <summary>
		/// The value of this scrollbar.
		/// <code>value</code> should be greater than <code>minimum</code>
		/// and less than <code>maximum</code>
		/// 
		/// @serial </summary>
		/// <seealso cref= #getValue </seealso>
		/// <seealso cref= #setValue </seealso>
		private int Value_Renamed;

		/// <summary>
		/// The minimum value of this scrollbar.
		/// This value can only be set by the <code>ScrollPane</code>.
		/// <para>
		/// <strong>ATTN:</strong> In current implementation
		/// <code>minimum</code> is always <code>0</code>.  This field can
		/// only be altered via <code>setSpan</code> method and
		/// <code>ScrollPane</code> always calls that method with
		/// <code>0</code> for the minimum.  <code>getMinimum</code> method
		/// always returns <code>0</code> without checking this field.
		/// 
		/// @serial
		/// </para>
		/// </summary>
		/// <seealso cref= #getMinimum </seealso>
		/// <seealso cref= #setSpan(int, int, int) </seealso>
		private int Minimum_Renamed;

		/// <summary>
		/// The maximum value of this scrollbar.
		/// This value can only be set by the <code>ScrollPane</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getMaximum </seealso>
		/// <seealso cref= #setSpan(int, int, int) </seealso>
		private int Maximum_Renamed;

		/// <summary>
		/// The size of the visible portion of this scrollbar.
		/// This value can only be set by the <code>ScrollPane</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getVisibleAmount </seealso>
		/// <seealso cref= #setSpan(int, int, int) </seealso>
		private int VisibleAmount_Renamed;

		/// <summary>
		/// The adjusting status of the <code>Scrollbar</code>.
		/// True if the value is in the process of changing as a result of
		/// actions being taken by the user.
		/// </summary>
		/// <seealso cref= #getValueIsAdjusting </seealso>
		/// <seealso cref= #setValueIsAdjusting
		/// @since 1.4 </seealso>
		[NonSerialized]
		private bool IsAdjusting;

		/// <summary>
		/// The amount by which the scrollbar value will change when going
		/// up or down by a line.
		/// This value should be a non negative integer.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getUnitIncrement </seealso>
		/// <seealso cref= #setUnitIncrement </seealso>
		private int UnitIncrement_Renamed = 1;

		/// <summary>
		/// The amount by which the scrollbar value will change when going
		/// up or down by a page.
		/// This value should be a non negative integer.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getBlockIncrement </seealso>
		/// <seealso cref= #setBlockIncrement </seealso>
		private int BlockIncrement_Renamed = 1;

		private AdjustmentListener AdjustmentListener;

		/// <summary>
		/// Error message for <code>AWTError</code> reported when one of
		/// the public but unsupported methods is called.
		/// </summary>
		private const String SCROLLPANE_ONLY = "Can be set by scrollpane only";


		/// <summary>
		/// Initialize JNI field and method ids.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		static ScrollPaneAdjustable()
		{
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
			AWTAccessor.ScrollPaneAdjustableAccessor = new ScrollPaneAdjustableAccessorAnonymousInnerClassHelper();
		}

		private class ScrollPaneAdjustableAccessorAnonymousInnerClassHelper : AWTAccessor.ScrollPaneAdjustableAccessor
		{
			public ScrollPaneAdjustableAccessorAnonymousInnerClassHelper()
			{
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setTypedValue(final ScrollPaneAdjustable adj, final int v, final int type)
			public virtual void SetTypedValue(ScrollPaneAdjustable adj, int v, int type)
			{
				adj.SetTypedValue(v, type);
			}
		}

		/// <summary>
		/// JDK 1.1 serialVersionUID.
		/// </summary>
		private const long SerialVersionUID = -3359745691033257079L;


		/// <summary>
		/// Constructs a new object to represent specified scrollabar
		/// of the specified <code>ScrollPane</code>.
		/// Only ScrollPane creates instances of this class. </summary>
		/// <param name="sp">           <code>ScrollPane</code> </param>
		/// <param name="l">            <code>AdjustmentListener</code> to add upon creation. </param>
		/// <param name="orientation">  specifies which scrollbar this object represents,
		///                     can be either  <code>Adjustable.HORIZONTAL</code>
		///                     or <code>Adjustable.VERTICAL</code>. </param>
		internal ScrollPaneAdjustable(ScrollPane sp, AdjustmentListener l, int orientation)
		{
			this.Sp = sp;
			this.Orientation_Renamed = orientation;
			AddAdjustmentListener(l);
		}

		/// <summary>
		/// This is called by the scrollpane itself to update the
		/// <code>minimum</code>, <code>maximum</code> and
		/// <code>visible</code> values.  The scrollpane is the only one
		/// that should be changing these since it is the source of these
		/// values.
		/// </summary>
		internal virtual void SetSpan(int min, int max, int visible)
		{
			// adjust the values to be reasonable
			Minimum_Renamed = min;
			Maximum_Renamed = System.Math.Max(max, Minimum_Renamed + 1);
			VisibleAmount_Renamed = System.Math.Min(visible, Maximum_Renamed - Minimum_Renamed);
			VisibleAmount_Renamed = System.Math.Max(VisibleAmount_Renamed, 1);
			BlockIncrement_Renamed = System.Math.Max((int)(visible * .90), 1);
			Value = Value_Renamed;
		}

		/// <summary>
		/// Returns the orientation of this scrollbar. </summary>
		/// <returns>    the orientation of this scrollbar, either
		///            <code>Adjustable.HORIZONTAL</code> or
		///            <code>Adjustable.VERTICAL</code> </returns>
		public virtual int Orientation
		{
			get
			{
				return Orientation_Renamed;
			}
		}

		/// <summary>
		/// This method should <strong>NOT</strong> be called by user code.
		/// This method is public for this class to properly implement
		/// <code>Adjustable</code> interface.
		/// </summary>
		/// <exception cref="AWTError"> Always throws an error when called. </exception>
		public virtual int Minimum
		{
			set
			{
				throw new AWTError(SCROLLPANE_ONLY);
			}
			get
			{
				// XXX: This relies on setSpan always being called with 0 for
				// the minimum (which is currently true).
				return 0;
			}
		}


		/// <summary>
		/// This method should <strong>NOT</strong> be called by user code.
		/// This method is public for this class to properly implement
		/// <code>Adjustable</code> interface.
		/// </summary>
		/// <exception cref="AWTError"> Always throws an error when called. </exception>
		public virtual int Maximum
		{
			set
			{
				throw new AWTError(SCROLLPANE_ONLY);
			}
			get
			{
				return Maximum_Renamed;
			}
		}


		public virtual int UnitIncrement
		{
			set
			{
				lock (this)
				{
					if (value != UnitIncrement_Renamed)
					{
						UnitIncrement_Renamed = value;
						if (Sp.Peer_Renamed != null)
						{
							ScrollPanePeer peer = (ScrollPanePeer) Sp.Peer_Renamed;
							peer.SetUnitIncrement(this, value);
						}
					}
				}
			}
			get
			{
				return UnitIncrement_Renamed;
			}
		}


		public virtual int BlockIncrement
		{
			set
			{
				lock (this)
				{
					BlockIncrement_Renamed = value;
				}
			}
			get
			{
				return BlockIncrement_Renamed;
			}
		}


		/// <summary>
		/// This method should <strong>NOT</strong> be called by user code.
		/// This method is public for this class to properly implement
		/// <code>Adjustable</code> interface.
		/// </summary>
		/// <exception cref="AWTError"> Always throws an error when called. </exception>
		public virtual int VisibleAmount
		{
			set
			{
				throw new AWTError(SCROLLPANE_ONLY);
			}
			get
			{
				return VisibleAmount_Renamed;
			}
		}



		/// <summary>
		/// Sets the <code>valueIsAdjusting</code> property.
		/// </summary>
		/// <param name="b"> new adjustment-in-progress status </param>
		/// <seealso cref= #getValueIsAdjusting
		/// @since 1.4 </seealso>
		public virtual bool ValueIsAdjusting
		{
			set
			{
				if (IsAdjusting != value)
				{
					IsAdjusting = value;
					AdjustmentEvent e = new AdjustmentEvent(this, AdjustmentEvent.ADJUSTMENT_VALUE_CHANGED, AdjustmentEvent.TRACK, Value_Renamed, value);
					AdjustmentListener.AdjustmentValueChanged(e);
				}
			}
			get
			{
				return IsAdjusting;
			}
		}


		/// <summary>
		/// Sets the value of this scrollbar to the specified value.
		/// <para>
		/// If the value supplied is less than the current minimum or
		/// greater than the current maximum, then one of those values is
		/// substituted, as appropriate.
		/// 
		/// </para>
		/// </summary>
		/// <param name="v"> the new value of the scrollbar </param>
		public virtual int Value
		{
			set
			{
				SetTypedValue(value, AdjustmentEvent.TRACK);
			}
			get
			{
				return Value_Renamed;
			}
		}

		/// <summary>
		/// Sets the value of this scrollbar to the specified value.
		/// <para>
		/// If the value supplied is less than the current minimum or
		/// greater than the current maximum, then one of those values is
		/// substituted, as appropriate. Also, creates and dispatches
		/// the AdjustementEvent with specified type and value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="v"> the new value of the scrollbar </param>
		/// <param name="type"> the type of the scrolling operation occurred </param>
		private void SetTypedValue(int v, int type)
		{
			v = System.Math.Max(v, Minimum_Renamed);
			v = System.Math.Min(v, Maximum_Renamed - VisibleAmount_Renamed);

			if (v != Value_Renamed)
			{
				Value_Renamed = v;
				// Synchronously notify the listeners so that they are
				// guaranteed to be up-to-date with the Adjustable before
				// it is mutated again.
				AdjustmentEvent e = new AdjustmentEvent(this, AdjustmentEvent.ADJUSTMENT_VALUE_CHANGED, type, Value_Renamed, IsAdjusting);
				AdjustmentListener.AdjustmentValueChanged(e);
			}
		}


		/// <summary>
		/// Adds the specified adjustment listener to receive adjustment
		/// events from this <code>ScrollPaneAdjustable</code>.
		/// If <code>l</code> is <code>null</code>, no exception is thrown
		/// and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   the adjustment listener. </param>
		/// <seealso cref=      #removeAdjustmentListener </seealso>
		/// <seealso cref=      #getAdjustmentListeners </seealso>
		/// <seealso cref=      java.awt.event.AdjustmentListener </seealso>
		/// <seealso cref=      java.awt.event.AdjustmentEvent </seealso>
		public virtual void AddAdjustmentListener(AdjustmentListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				AdjustmentListener = AWTEventMulticaster.Add(AdjustmentListener, l);
			}
		}

		/// <summary>
		/// Removes the specified adjustment listener so that it no longer
		/// receives adjustment events from this <code>ScrollPaneAdjustable</code>.
		/// If <code>l</code> is <code>null</code>, no exception is thrown
		/// and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">     the adjustment listener. </param>
		/// <seealso cref=           #addAdjustmentListener </seealso>
		/// <seealso cref=           #getAdjustmentListeners </seealso>
		/// <seealso cref=           java.awt.event.AdjustmentListener </seealso>
		/// <seealso cref=           java.awt.event.AdjustmentEvent
		/// @since         JDK1.1 </seealso>
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
		/// registered on this <code>ScrollPaneAdjustable</code>.
		/// </summary>
		/// <returns> all of this <code>ScrollPaneAdjustable</code>'s
		///         <code>AdjustmentListener</code>s
		///         or an empty array if no adjustment
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=           #addAdjustmentListener </seealso>
		/// <seealso cref=           #removeAdjustmentListener </seealso>
		/// <seealso cref=           java.awt.event.AdjustmentListener </seealso>
		/// <seealso cref=           java.awt.event.AdjustmentEvent
		/// @since 1.4 </seealso>
		public virtual AdjustmentListener[] AdjustmentListeners
		{
			get
			{
				lock (this)
				{
					return (AdjustmentListener[])(AWTEventMulticaster.GetListeners(AdjustmentListener, typeof(AdjustmentListener)));
				}
			}
		}

		/// <summary>
		/// Returns a string representation of this scrollbar and its values. </summary>
		/// <returns>    a string representation of this scrollbar. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[" + ParamString() + "]";
		}

		/// <summary>
		/// Returns a string representing the state of this scrollbar.
		/// This method is intended to be used only for debugging purposes,
		/// and the content and format of the returned string may vary
		/// between implementations.  The returned string may be empty but
		/// may not be <code>null</code>.
		/// </summary>
		/// <returns>      the parameter string of this scrollbar. </returns>
		public virtual String ParamString()
		{
			return ((Orientation_Renamed == Adjustable_Fields.VERTICAL ? "vertical," :"horizontal,") + "[0.." + Maximum_Renamed + "]" + ",val=" + Value_Renamed + ",vis=" + VisibleAmount_Renamed + ",unit=" + UnitIncrement_Renamed + ",block=" + BlockIncrement_Renamed + ",isAdjusting=" + IsAdjusting);
		}
	}

}