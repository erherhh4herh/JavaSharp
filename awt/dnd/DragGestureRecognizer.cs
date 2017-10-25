using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.dnd
{




	/// <summary>
	/// The <code>DragGestureRecognizer</code> is an
	/// abstract base class for the specification
	/// of a platform-dependent listener that can be associated with a particular
	/// <code>Component</code> in order to
	/// identify platform-dependent drag initiating gestures.
	/// <para>
	/// The appropriate <code>DragGestureRecognizer</code>
	/// subclass instance is obtained from the
	/// <seealso cref="DragSource"/> associated with
	/// a particular <code>Component</code>, or from the <code>Toolkit</code> object via its
	/// <seealso cref="java.awt.Toolkit#createDragGestureRecognizer createDragGestureRecognizer()"/>
	/// method.
	/// </para>
	/// <para>
	/// Once the <code>DragGestureRecognizer</code>
	/// is associated with a particular <code>Component</code>
	/// it will register the appropriate listener interfaces on that
	/// <code>Component</code>
	/// in order to track the input events delivered to the <code>Component</code>.
	/// </para>
	/// <para>
	/// Once the <code>DragGestureRecognizer</code> identifies a sequence of events
	/// on the <code>Component</code> as a drag initiating gesture, it will notify
	/// its unicast <code>DragGestureListener</code> by
	/// invoking its
	/// <seealso cref="java.awt.dnd.DragGestureListener#dragGestureRecognized gestureRecognized()"/>
	/// method.
	/// <P>
	/// When a concrete <code>DragGestureRecognizer</code>
	/// instance detects a drag initiating
	/// gesture on the <code>Component</code> it is associated with,
	/// it fires a <seealso cref="DragGestureEvent"/> to
	/// the <code>DragGestureListener</code> registered on
	/// its unicast event source for <code>DragGestureListener</code>
	/// events. This <code>DragGestureListener</code> is responsible
	/// for causing the associated
	/// <code>DragSource</code> to start the Drag and Drop operation (if
	/// appropriate).
	/// <P>
	/// @author Laurence P. G. Cable
	/// </para>
	/// </summary>
	/// <seealso cref= java.awt.dnd.DragGestureListener </seealso>
	/// <seealso cref= java.awt.dnd.DragGestureEvent </seealso>
	/// <seealso cref= java.awt.dnd.DragSource </seealso>

	[Serializable]
	public abstract class DragGestureRecognizer
	{

		private const long SerialVersionUID = 8996673345831063337L;

		/// <summary>
		/// Construct a new <code>DragGestureRecognizer</code>
		/// given the <code>DragSource</code> to be used
		/// in this Drag and Drop operation, the <code>Component</code>
		/// this <code>DragGestureRecognizer</code> should "observe"
		/// for drag initiating gestures, the action(s) supported
		/// for this Drag and Drop operation, and the
		/// <code>DragGestureListener</code> to notify
		/// once a drag initiating gesture has been detected.
		/// <P> </summary>
		/// <param name="ds">  the <code>DragSource</code> this
		/// <code>DragGestureRecognizer</code>
		/// will use to process the Drag and Drop operation
		/// </param>
		/// <param name="c"> the <code>Component</code>
		/// this <code>DragGestureRecognizer</code>
		/// should "observe" the event stream to,
		/// in order to detect a drag initiating gesture.
		/// If this value is <code>null</code>, the
		/// <code>DragGestureRecognizer</code>
		/// is not associated with any <code>Component</code>.
		/// </param>
		/// <param name="sa">  the set (logical OR) of the
		/// <code>DnDConstants</code>
		/// that this Drag and Drop operation will support
		/// </param>
		/// <param name="dgl"> the <code>DragGestureRecognizer</code>
		/// to notify when a drag gesture is detected
		/// <P> </param>
		/// <exception cref="IllegalArgumentException">
		/// if ds is <code>null</code>. </exception>

		protected internal DragGestureRecognizer(DragSource ds, Component c, int sa, DragGestureListener dgl) : base()
		{

			if (ds == null)
			{
				throw new IllegalArgumentException("null DragSource");
			}

			DragSource_Renamed = ds;
			Component_Renamed = c;
			SourceActions_Renamed = sa & (DnDConstants.ACTION_COPY_OR_MOVE | DnDConstants.ACTION_LINK);

			try
			{
				if (dgl != null)
				{
					AddDragGestureListener(dgl);
				}
			}
			catch (TooManyListenersException)
			{
				// cant happen ...
			}
		}

		/// <summary>
		/// Construct a new <code>DragGestureRecognizer</code>
		/// given the <code>DragSource</code> to be used in this
		/// Drag and Drop
		/// operation, the <code>Component</code> this
		/// <code>DragGestureRecognizer</code> should "observe"
		/// for drag initiating gestures, and the action(s)
		/// supported for this Drag and Drop operation.
		/// <P> </summary>
		/// <param name="ds">  the <code>DragSource</code> this
		/// <code>DragGestureRecognizer</code> will use to
		/// process the Drag and Drop operation
		/// </param>
		/// <param name="c">   the <code>Component</code> this
		/// <code>DragGestureRecognizer</code> should "observe" the event
		/// stream to, in order to detect a drag initiating gesture.
		/// If this value is <code>null</code>, the
		/// <code>DragGestureRecognizer</code>
		/// is not associated with any <code>Component</code>.
		/// </param>
		/// <param name="sa"> the set (logical OR) of the <code>DnDConstants</code>
		/// that this Drag and Drop operation will support
		/// <P> </param>
		/// <exception cref="IllegalArgumentException">
		/// if ds is <code>null</code>. </exception>

		protected internal DragGestureRecognizer(DragSource ds, Component c, int sa) : this(ds, c, sa, null)
		{
		}

		/// <summary>
		/// Construct a new <code>DragGestureRecognizer</code>
		/// given the <code>DragSource</code> to be used
		/// in this Drag and Drop operation, and
		/// the <code>Component</code> this
		/// <code>DragGestureRecognizer</code>
		/// should "observe" for drag initiating gestures.
		/// <P> </summary>
		/// <param name="ds"> the <code>DragSource</code> this
		/// <code>DragGestureRecognizer</code>
		/// will use to process the Drag and Drop operation
		/// </param>
		/// <param name="c"> the <code>Component</code>
		/// this <code>DragGestureRecognizer</code>
		/// should "observe" the event stream to,
		/// in order to detect a drag initiating gesture.
		/// If this value is <code>null</code>,
		/// the <code>DragGestureRecognizer</code>
		/// is not associated with any <code>Component</code>.
		/// <P> </param>
		/// <exception cref="IllegalArgumentException">
		/// if ds is <code>null</code>. </exception>

		protected internal DragGestureRecognizer(DragSource ds, Component c) : this(ds, c, DnDConstants.ACTION_NONE)
		{
		}

		/// <summary>
		/// Construct a new <code>DragGestureRecognizer</code>
		/// given the <code>DragSource</code> to be used in this
		/// Drag and Drop operation.
		/// <P> </summary>
		/// <param name="ds"> the <code>DragSource</code> this
		/// <code>DragGestureRecognizer</code> will
		/// use to process the Drag and Drop operation
		/// <P> </param>
		/// <exception cref="IllegalArgumentException">
		/// if ds is <code>null</code>. </exception>

		protected internal DragGestureRecognizer(DragSource ds) : this(ds, null)
		{
		}

		/// <summary>
		/// register this DragGestureRecognizer's Listeners with the Component
		/// 
		/// subclasses must override this method
		/// </summary>

		protected internal abstract void RegisterListeners();

		/// <summary>
		/// unregister this DragGestureRecognizer's Listeners with the Component
		/// 
		/// subclasses must override this method
		/// </summary>

		protected internal abstract void UnregisterListeners();

		/// <summary>
		/// This method returns the <code>DragSource</code>
		/// this <code>DragGestureRecognizer</code>
		/// will use in order to process the Drag and Drop
		/// operation.
		/// <P> </summary>
		/// <returns> the DragSource </returns>

		public virtual DragSource DragSource
		{
			get
			{
				return DragSource_Renamed;
			}
		}

		/// <summary>
		/// This method returns the <code>Component</code>
		/// that is to be "observed" by the
		/// <code>DragGestureRecognizer</code>
		/// for drag initiating gestures.
		/// <P> </summary>
		/// <returns> The Component this DragGestureRecognizer
		/// is associated with </returns>

		public virtual Component Component
		{
			get
			{
				lock (this)
				{
					return Component_Renamed;
				}
			}
			set
			{
				lock (this)
				{
					if (Component_Renamed != null && DragGestureListener != null)
					{
						UnregisterListeners();
					}
            
					Component_Renamed = value;
            
					if (Component_Renamed != null && DragGestureListener != null)
					{
						RegisterListeners();
					}
				}
			}
		}



		/// <summary>
		/// This method returns an int representing the
		/// type of action(s) this Drag and Drop
		/// operation will support.
		/// <P> </summary>
		/// <returns> the currently permitted source action(s) </returns>

		public virtual int SourceActions
		{
			get
			{
				lock (this)
				{
					return SourceActions_Renamed;
				}
			}
			set
			{
				lock (this)
				{
					SourceActions_Renamed = value & (DnDConstants.ACTION_COPY_OR_MOVE | DnDConstants.ACTION_LINK);
				}
			}
		}



		/// <summary>
		/// This method returns the first event in the
		/// series of events that initiated
		/// the Drag and Drop operation.
		/// <P> </summary>
		/// <returns> the initial event that triggered the drag gesture </returns>

		public virtual InputEvent TriggerEvent
		{
			get
			{
				return Events.Count == 0 ? null : Events[0];
			}
		}

		/// <summary>
		/// Reset the Recognizer, if its currently recognizing a gesture, ignore
		/// it.
		/// </summary>

		public virtual void ResetRecognizer()
		{
			Events.Clear();
		}

		/// <summary>
		/// Register a new <code>DragGestureListener</code>.
		/// <P> </summary>
		/// <param name="dgl"> the <code>DragGestureListener</code> to register
		/// with this <code>DragGestureRecognizer</code>.
		/// <P> </param>
		/// <exception cref="java.util.TooManyListenersException"> if a
		/// <code>DragGestureListener</code> has already been added. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void addDragGestureListener(DragGestureListener dgl) throws java.util.TooManyListenersException
		public virtual void AddDragGestureListener(DragGestureListener dgl)
		{
			lock (this)
			{
				if (DragGestureListener != null)
				{
					throw new TooManyListenersException();
				}
				else
				{
					DragGestureListener = dgl;
        
					if (Component_Renamed != null)
					{
						RegisterListeners();
					}
				}
			}
		}

		/// <summary>
		/// unregister the current DragGestureListener
		/// <P> </summary>
		/// <param name="dgl"> the <code>DragGestureListener</code> to unregister
		/// from this <code>DragGestureRecognizer</code>
		/// <P> </param>
		/// <exception cref="IllegalArgumentException"> if
		/// dgl is not (equal to) the currently registered <code>DragGestureListener</code>. </exception>

		public virtual void RemoveDragGestureListener(DragGestureListener dgl)
		{
			lock (this)
			{
				if (DragGestureListener == null || !DragGestureListener.Equals(dgl))
				{
					throw new IllegalArgumentException();
				}
				else
				{
					DragGestureListener = null;
        
					if (Component_Renamed != null)
					{
						UnregisterListeners();
					}
				}
			}
		}

		/// <summary>
		/// Notify the DragGestureListener that a Drag and Drop initiating
		/// gesture has occurred. Then reset the state of the Recognizer.
		/// <P> </summary>
		/// <param name="dragAction"> The action initially selected by the users gesture </param>
		/// <param name="p">          The point (in Component coords) where the gesture originated </param>
		protected internal virtual void FireDragGestureRecognized(int dragAction, Point p)
		{
			lock (this)
			{
				try
				{
					if (DragGestureListener != null)
					{
						DragGestureListener.DragGestureRecognized(new DragGestureEvent(this, dragAction, p, Events));
					}
				}
				finally
				{
					Events.Clear();
				}
			}
		}

		/// <summary>
		/// Listeners registered on the Component by this Recognizer shall record
		/// all Events that are recognized as part of the series of Events that go
		/// to comprise a Drag and Drop initiating gesture via this API.
		/// <P>
		/// This method is used by a <code>DragGestureRecognizer</code>
		/// implementation to add an <code>InputEvent</code>
		/// subclass (that it believes is one in a series
		/// of events that comprise a Drag and Drop operation)
		/// to the array of events that this
		/// <code>DragGestureRecognizer</code> maintains internally.
		/// <P> </summary>
		/// <param name="awtie"> the <code>InputEvent</code>
		/// to add to this <code>DragGestureRecognizer</code>'s
		/// internal array of events. Note that <code>null</code>
		/// is not a valid value, and will be ignored. </param>

		protected internal virtual void AppendEvent(InputEvent awtie)
		{
			lock (this)
			{
				Events.Add(awtie);
			}
		}

		/// <summary>
		/// Serializes this <code>DragGestureRecognizer</code>. This method first
		/// performs default serialization. Then, this object's
		/// <code>DragGestureListener</code> is written out if and only if it can be
		/// serialized. If not, <code>null</code> is written instead.
		/// 
		/// @serialData The default serializable fields, in alphabetical order,
		///             followed by either a <code>DragGestureListener</code>, or
		///             <code>null</code>.
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			s.DefaultWriteObject();

			s.WriteObject(SerializationTester.Test(DragGestureListener) ? DragGestureListener : null);
		}

		/// <summary>
		/// Deserializes this <code>DragGestureRecognizer</code>. This method first
		/// performs default deserialization for all non-<code>transient</code>
		/// fields. This object's <code>DragGestureListener</code> is then
		/// deserialized as well by using the next object in the stream.
		/// 
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		private void ReadObject(ObjectInputStream s)
		{
			ObjectInputStream.GetField f = s.ReadFields();

			DragSource newDragSource = (DragSource)f.Get("dragSource", null);
			if (newDragSource == null)
			{
				throw new InvalidObjectException("null DragSource");
			}
			DragSource_Renamed = newDragSource;

			Component_Renamed = (Component)f.Get("component", null);
			SourceActions_Renamed = f.Get("sourceActions", 0) & (DnDConstants.ACTION_COPY_OR_MOVE | DnDConstants.ACTION_LINK);
			Events = (List<InputEvent>)f.Get("events", new List<>(1));

			DragGestureListener = (DragGestureListener)s.ReadObject();
		}

		/*
		 * fields
		 */

		/// <summary>
		/// The <code>DragSource</code>
		/// associated with this
		/// <code>DragGestureRecognizer</code>.
		/// 
		/// @serial
		/// </summary>
		protected internal DragSource DragSource_Renamed;

		/// <summary>
		/// The <code>Component</code>
		/// associated with this <code>DragGestureRecognizer</code>.
		/// 
		/// @serial
		/// </summary>
		protected internal Component Component_Renamed;

		/// <summary>
		/// The <code>DragGestureListener</code>
		/// associated with this <code>DragGestureRecognizer</code>.
		/// </summary>
		[NonSerialized]
		protected internal DragGestureListener DragGestureListener;

	  /// <summary>
	  /// An <code>int</code> representing
	  /// the type(s) of action(s) used
	  /// in this Drag and Drop operation.
	  /// 
	  /// @serial
	  /// </summary>
	  protected internal int SourceActions_Renamed;

	   /// <summary>
	   /// The list of events (in order) that
	   /// the <code>DragGestureRecognizer</code>
	   /// "recognized" as a "gesture" that triggers a drag.
	   /// 
	   /// @serial
	   /// </summary>
	   protected internal List<InputEvent> Events = new List<InputEvent>(1);
	}

}