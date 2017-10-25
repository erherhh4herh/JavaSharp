using System;
using System.Collections;
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
	/// A <code>DragGestureEvent</code> is passed
	/// to <code>DragGestureListener</code>'s
	/// dragGestureRecognized() method
	/// when a particular <code>DragGestureRecognizer</code> detects that a
	/// platform dependent drag initiating gesture has occurred
	/// on the <code>Component</code> that it is tracking.
	/// 
	/// The {@code action} field of any {@code DragGestureEvent} instance should take one of the following
	/// values:
	/// <ul>
	/// <li> {@code DnDConstants.ACTION_COPY}
	/// <li> {@code DnDConstants.ACTION_MOVE}
	/// <li> {@code DnDConstants.ACTION_LINK}
	/// </ul>
	/// Assigning the value different from listed above will cause an unspecified behavior.
	/// </summary>
	/// <seealso cref= java.awt.dnd.DragGestureRecognizer </seealso>
	/// <seealso cref= java.awt.dnd.DragGestureListener </seealso>
	/// <seealso cref= java.awt.dnd.DragSource </seealso>
	/// <seealso cref= java.awt.dnd.DnDConstants </seealso>

	public class DragGestureEvent : EventObject
	{

		private const long SerialVersionUID = 9080172649166731306L;

		/// <summary>
		/// Constructs a <code>DragGestureEvent</code> object given by the
		/// <code>DragGestureRecognizer</code> instance firing this event,
		/// an {@code act} parameter representing
		/// the user's preferred action, an {@code ori} parameter
		/// indicating the origin of the drag, and a {@code List} of
		/// events that comprise the gesture({@code evs} parameter).
		/// <P> </summary>
		/// <param name="dgr"> The <code>DragGestureRecognizer</code> firing this event </param>
		/// <param name="act"> The user's preferred action.
		///            For information on allowable values, see
		///            the class description for <seealso cref="DragGestureEvent"/> </param>
		/// <param name="ori"> The origin of the drag </param>
		/// <param name="evs"> The <code>List</code> of events that comprise the gesture
		/// <P> </param>
		/// <exception cref="IllegalArgumentException"> if any parameter equals {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if the act parameter does not comply with
		///                                  the values given in the class
		///                                  description for <seealso cref="DragGestureEvent"/> </exception>
		/// <seealso cref= java.awt.dnd.DnDConstants </seealso>

		public DragGestureEvent<T1>(DragGestureRecognizer dgr, int act, Point ori, IList<T1> evs) where T1 : java.awt.@event.InputEvent : base(dgr)
		{

			if ((Component_Renamed = dgr.Component) == null)
			{
				throw new IllegalArgumentException("null component");
			}
			if ((DragSource_Renamed = dgr.DragSource) == null)
			{
				throw new IllegalArgumentException("null DragSource");
			}

			if (evs == null || evs.Count == 0)
			{
				throw new IllegalArgumentException("null or empty list of events");
			}

			if (act != DnDConstants.ACTION_COPY && act != DnDConstants.ACTION_MOVE && act != DnDConstants.ACTION_LINK)
			{
				throw new IllegalArgumentException("bad action");
			}

			if (ori == null)
			{
				throw new IllegalArgumentException("null origin");
			}

			Events = evs;
			Action = act;
			Origin = ori;
		}

		/// <summary>
		/// Returns the source as a <code>DragGestureRecognizer</code>.
		/// <P> </summary>
		/// <returns> the source as a <code>DragGestureRecognizer</code> </returns>

		public virtual DragGestureRecognizer SourceAsDragGestureRecognizer
		{
			get
			{
				return (DragGestureRecognizer)Source;
			}
		}

		/// <summary>
		/// Returns the <code>Component</code> associated
		/// with this <code>DragGestureEvent</code>.
		/// <P> </summary>
		/// <returns> the Component </returns>

		public virtual Component Component
		{
			get
			{
				return Component_Renamed;
			}
		}

		/// <summary>
		/// Returns the <code>DragSource</code>.
		/// <P> </summary>
		/// <returns> the <code>DragSource</code> </returns>

		public virtual DragSource DragSource
		{
			get
			{
				return DragSource_Renamed;
			}
		}

		/// <summary>
		/// Returns a <code>Point</code> in the coordinates
		/// of the <code>Component</code> over which the drag originated.
		/// <P> </summary>
		/// <returns> the Point where the drag originated in Component coords. </returns>

		public virtual Point DragOrigin
		{
			get
			{
				return Origin;
			}
		}

		/// <summary>
		/// Returns an <code>Iterator</code> for the events
		/// comprising the gesture.
		/// <P> </summary>
		/// <returns> an Iterator for the events comprising the gesture </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Iterator<java.awt.event.InputEvent> iterator()
		public virtual IEnumerator<InputEvent> Iterator()
		{
			return Events.GetEnumerator();
		}

		/// <summary>
		/// Returns an <code>Object</code> array of the
		/// events comprising the drag gesture.
		/// <P> </summary>
		/// <returns> an array of the events comprising the gesture </returns>

		public virtual Object[] ToArray()
		{
			return Events.ToArray();
		}

		/// <summary>
		/// Returns an array of the events comprising the drag gesture.
		/// <P> </summary>
		/// <param name="array"> the array of <code>EventObject</code> sub(types)
		/// <P> </param>
		/// <returns> an array of the events comprising the gesture </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Object[] toArray(Object[] array)
		public virtual Object[] ToArray(Object[] array)
		{
			return Events.toArray(array);
		}

		/// <summary>
		/// Returns an <code>int</code> representing the
		/// action selected by the user.
		/// <P> </summary>
		/// <returns> the action selected by the user </returns>

		public virtual int DragAction
		{
			get
			{
				return Action;
			}
		}

		/// <summary>
		/// Returns the initial event that triggered the gesture.
		/// <P> </summary>
		/// <returns> the first "triggering" event in the sequence of the gesture </returns>

		public virtual InputEvent TriggerEvent
		{
			get
			{
				return SourceAsDragGestureRecognizer.TriggerEvent;
			}
		}

		/// <summary>
		/// Starts the drag operation given the <code>Cursor</code> for this drag
		/// operation and the <code>Transferable</code> representing the source data
		/// for this drag operation.
		/// <br>
		/// If a <code>null</code> <code>Cursor</code> is specified no exception will
		/// be thrown and default drag cursors will be used instead.
		/// <br>
		/// If a <code>null</code> <code>Transferable</code> is specified
		/// <code>NullPointerException</code> will be thrown. </summary>
		/// <param name="dragCursor">     The initial {@code Cursor} for this drag operation
		///                       or {@code null} for the default cursor handling;
		///                       see
		///                       <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		///                       for more details on the cursor handling mechanism
		///                       during drag and drop </param>
		/// <param name="transferable"> The <code>Transferable</code> representing the source
		///                     data for this drag operation.
		/// </param>
		/// <exception cref="InvalidDnDOperationException"> if the Drag and Drop
		///         system is unable to initiate a drag operation, or if the user
		///         attempts to start a drag while an existing drag operation is
		///         still executing. </exception>
		/// <exception cref="NullPointerException"> if the {@code Transferable} is {@code null}
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDrag(java.awt.Cursor dragCursor, java.awt.datatransfer.Transferable transferable) throws InvalidDnDOperationException
		public virtual void StartDrag(Cursor dragCursor, Transferable transferable)
		{
			DragSource_Renamed.StartDrag(this, dragCursor, transferable, null);
		}

		/// <summary>
		/// Starts the drag given the initial <code>Cursor</code> to display,
		/// the <code>Transferable</code> object,
		/// and the <code>DragSourceListener</code> to use.
		/// <P> </summary>
		/// <param name="dragCursor">     The initial {@code Cursor} for this drag operation
		///                       or {@code null} for the default cursor handling;
		///                       see
		///                       <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		///                       for more details on the cursor handling mechanism
		///                       during drag and drop </param>
		/// <param name="transferable"> The source's Transferable </param>
		/// <param name="dsl">          The source's DragSourceListener
		/// <P> </param>
		/// <exception cref="InvalidDnDOperationException"> if
		/// the Drag and Drop system is unable to
		/// initiate a drag operation, or if the user
		/// attempts to start a drag while an existing
		/// drag operation is still executing. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDrag(java.awt.Cursor dragCursor, java.awt.datatransfer.Transferable transferable, DragSourceListener dsl) throws InvalidDnDOperationException
		public virtual void StartDrag(Cursor dragCursor, Transferable transferable, DragSourceListener dsl)
		{
			DragSource_Renamed.StartDrag(this, dragCursor, transferable, dsl);
		}

		/// <summary>
		/// Start the drag given the initial <code>Cursor</code> to display,
		/// a drag <code>Image</code>, the offset of
		/// the <code>Image</code>,
		/// the <code>Transferable</code> object, and
		/// the <code>DragSourceListener</code> to use.
		/// <P> </summary>
		/// <param name="dragCursor">     The initial {@code Cursor} for this drag operation
		///                       or {@code null} for the default cursor handling;
		///                       see
		///                       <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		///                       for more details on the cursor handling mechanism
		///                       during drag and drop </param>
		/// <param name="dragImage">    The source's dragImage </param>
		/// <param name="imageOffset">  The dragImage's offset </param>
		/// <param name="transferable"> The source's Transferable </param>
		/// <param name="dsl">          The source's DragSourceListener
		/// <P> </param>
		/// <exception cref="InvalidDnDOperationException"> if
		/// the Drag and Drop system is unable to
		/// initiate a drag operation, or if the user
		/// attempts to start a drag while an existing
		/// drag operation is still executing. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDrag(java.awt.Cursor dragCursor, java.awt.Image dragImage, java.awt.Point imageOffset, java.awt.datatransfer.Transferable transferable, DragSourceListener dsl) throws InvalidDnDOperationException
		public virtual void StartDrag(Cursor dragCursor, Image dragImage, Point imageOffset, Transferable transferable, DragSourceListener dsl)
		{
			DragSource_Renamed.StartDrag(this, dragCursor, dragImage, imageOffset, transferable, dsl);
		}

		/// <summary>
		/// Serializes this <code>DragGestureEvent</code>. Performs default
		/// serialization and then writes out this object's <code>List</code> of
		/// gesture events if and only if the <code>List</code> can be serialized.
		/// If not, <code>null</code> is written instead. In this case, a
		/// <code>DragGestureEvent</code> created from the resulting deserialized
		/// stream will contain an empty <code>List</code> of gesture events.
		/// 
		/// @serialData The default serializable fields, in alphabetical order,
		///             followed by either a <code>List</code> instance, or
		///             <code>null</code>.
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			s.DefaultWriteObject();

			s.WriteObject(SerializationTester.Test(Events) ? Events : null);
		}

		/// <summary>
		/// Deserializes this <code>DragGestureEvent</code>. This method first
		/// performs default deserialization for all non-<code>transient</code>
		/// fields. An attempt is then made to deserialize this object's
		/// <code>List</code> of gesture events as well. This is first attempted
		/// by deserializing the field <code>events</code>, because, in releases
		/// prior to 1.4, a non-<code>transient</code> field of this name stored the
		/// <code>List</code> of gesture events. If this fails, the next object in
		/// the stream is used instead. If the resulting <code>List</code> is
		/// <code>null</code>, this object's <code>List</code> of gesture events
		/// is set to an empty <code>List</code>.
		/// 
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream s)
		{
			ObjectInputStream.GetField f = s.ReadFields();

			DragSource newDragSource = (DragSource)f.Get("dragSource", null);
			if (newDragSource == null)
			{
				throw new InvalidObjectException("null DragSource");
			}
			DragSource_Renamed = newDragSource;

			Component newComponent = (Component)f.Get("component", null);
			if (newComponent == null)
			{
				throw new InvalidObjectException("null component");
			}
			Component_Renamed = newComponent;

			Point newOrigin = (Point)f.Get("origin", null);
			if (newOrigin == null)
			{
				throw new InvalidObjectException("null origin");
			}
			Origin = newOrigin;

			int newAction = f.Get("action", 0);
			if (newAction != DnDConstants.ACTION_COPY && newAction != DnDConstants.ACTION_MOVE && newAction != DnDConstants.ACTION_LINK)
			{
				throw new InvalidObjectException("bad action");
			}
			Action = newAction;

			// Pre-1.4 support. 'events' was previously non-transient
			IList newEvents;
			try
			{
				newEvents = (IList)f.Get("events", null);
			}
			catch (IllegalArgumentException)
			{
				// 1.4-compatible byte stream. 'events' was written explicitly
				newEvents = (IList)s.ReadObject();
			}

			// Implementation assumes 'events' is never null.
			if (newEvents != null && newEvents.Count == 0)
			{
				// Constructor treats empty events list as invalid value
				// Throw exception if serialized list is empty
				throw new InvalidObjectException("empty list of events");
			}
			else if (newEvents == null)
			{
				newEvents = Collections.EmptyList();
			}
			Events = newEvents;
		}

		/*
		 * fields
		 */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") private transient java.util.List events;
		[NonSerialized]
		private IList Events;

		/// <summary>
		/// The DragSource associated with this DragGestureEvent.
		/// 
		/// @serial
		/// </summary>
		private DragSource DragSource_Renamed;

		/// <summary>
		/// The Component associated with this DragGestureEvent.
		/// 
		/// @serial
		/// </summary>
		private Component Component_Renamed;

		/// <summary>
		/// The origin of the drag.
		/// 
		/// @serial
		/// </summary>
		private Point Origin;

		/// <summary>
		/// The user's preferred action.
		/// 
		/// @serial
		/// </summary>
		private int Action;
	}

}