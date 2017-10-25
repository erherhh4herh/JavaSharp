using System;

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

namespace java.awt.dnd
{




	/// <summary>
	/// The <code>DragSourceContext</code> class is responsible for managing the
	/// initiator side of the Drag and Drop protocol. In particular, it is responsible
	/// for managing drag event notifications to the
	/// <seealso cref="DragSourceListener DragSourceListeners"/>
	/// and <seealso cref="DragSourceMotionListener DragSourceMotionListeners"/>, and providing the
	/// <seealso cref="Transferable"/> representing the source data for the drag operation.
	/// <para>
	/// Note that the <code>DragSourceContext</code> itself
	/// implements the <code>DragSourceListener</code> and
	/// <code>DragSourceMotionListener</code> interfaces.
	/// This is to allow the platform peer
	/// (the <seealso cref="DragSourceContextPeer"/> instance)
	/// created by the <seealso cref="DragSource"/> to notify
	/// the <code>DragSourceContext</code> of
	/// state changes in the ongoing operation. This allows the
	/// <code>DragSourceContext</code> object to interpose
	/// itself between the platform and the
	/// listeners provided by the initiator of the drag operation.
	/// </para>
	/// <para>
	/// <a name="defaultCursor"></a>
	/// By default, {@code DragSourceContext} sets the cursor as appropriate
	/// for the current state of the drag and drop operation. For example, if
	/// the user has chosen <seealso cref="DnDConstants#ACTION_MOVE the move action"/>,
	/// and the pointer is over a target that accepts
	/// the move action, the default move cursor is shown. When
	/// the pointer is over an area that does not accept the transfer,
	/// the default "no drop" cursor is shown.
	/// </para>
	/// <para>
	/// This default handling mechanism is disabled when a custom cursor is set
	/// by the <seealso cref="#setCursor"/> method. When the default handling is disabled,
	/// it becomes the responsibility
	/// of the developer to keep the cursor up to date, by listening
	/// to the {@code DragSource} events and calling the {@code setCursor()} method.
	/// Alternatively, you can provide custom cursor behavior by providing
	/// custom implementations of the {@code DragSource}
	/// and the {@code DragSourceContext} classes.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= DragSourceListener </seealso>
	/// <seealso cref= DragSourceMotionListener </seealso>
	/// <seealso cref= DnDConstants
	/// @since 1.2 </seealso>

	[Serializable]
	public class DragSourceContext : DragSourceListener, DragSourceMotionListener
	{

		private const long SerialVersionUID = -115407898692194719L;

		// used by updateCurrentCursor

		/// <summary>
		/// An <code>int</code> used by updateCurrentCursor()
		/// indicating that the <code>Cursor</code> should change
		/// to the default (no drop) <code>Cursor</code>.
		/// </summary>
		protected internal const int DEFAULT = 0;

		/// <summary>
		/// An <code>int</code> used by updateCurrentCursor()
		/// indicating that the <code>Cursor</code>
		/// has entered a <code>DropTarget</code>.
		/// </summary>
		protected internal const int ENTER = 1;

		/// <summary>
		/// An <code>int</code> used by updateCurrentCursor()
		/// indicating that the <code>Cursor</code> is
		/// over a <code>DropTarget</code>.
		/// </summary>
		protected internal const int OVER = 2;

		/// <summary>
		/// An <code>int</code> used by updateCurrentCursor()
		/// indicating that the user operation has changed.
		/// </summary>

		protected internal const int CHANGED = 3;

		/// <summary>
		/// Called from <code>DragSource</code>, this constructor creates a new
		/// <code>DragSourceContext</code> given the
		/// <code>DragSourceContextPeer</code> for this Drag, the
		/// <code>DragGestureEvent</code> that triggered the Drag, the initial
		/// <code>Cursor</code> to use for the Drag, an (optional)
		/// <code>Image</code> to display while the Drag is taking place, the offset
		/// of the <code>Image</code> origin from the hotspot at the instant of the
		/// triggering event, the <code>Transferable</code> subject data, and the
		/// <code>DragSourceListener</code> to use during the Drag and Drop
		/// operation.
		/// <br>
		/// If <code>DragSourceContextPeer</code> is <code>null</code>
		/// <code>NullPointerException</code> is thrown.
		/// <br>
		/// If <code>DragGestureEvent</code> is <code>null</code>
		/// <code>NullPointerException</code> is thrown.
		/// <br>
		/// If <code>Cursor</code> is <code>null</code> no exception is thrown and
		/// the default drag cursor behavior is activated for this drag operation.
		/// <br>
		/// If <code>Image</code> is <code>null</code> no exception is thrown.
		/// <br>
		/// If <code>Image</code> is not <code>null</code> and the offset is
		/// <code>null</code> <code>NullPointerException</code> is thrown.
		/// <br>
		/// If <code>Transferable</code> is <code>null</code>
		/// <code>NullPointerException</code> is thrown.
		/// <br>
		/// If <code>DragSourceListener</code> is <code>null</code> no exception
		/// is thrown.
		/// </summary>
		/// <param name="dscp">       the <code>DragSourceContextPeer</code> for this drag </param>
		/// <param name="trigger">    the triggering event </param>
		/// <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		///                       or {@code null} for the default cursor handling;
		///                       see <a href="DragSourceContext.html#defaultCursor">class level documentation</a>
		///                       for more details on the cursor handling mechanism during drag and drop </param>
		/// <param name="dragImage">  the <code>Image</code> to drag (or <code>null</code>) </param>
		/// <param name="offset">     the offset of the image origin from the hotspot at the
		///                   instant of the triggering event </param>
		/// <param name="t">          the <code>Transferable</code> </param>
		/// <param name="dsl">        the <code>DragSourceListener</code>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the <code>Component</code> associated
		///         with the trigger event is <code>null</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if the <code>DragSource</code> for the
		///         trigger event is <code>null</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if the drag action for the
		///         trigger event is <code>DnDConstants.ACTION_NONE</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if the source actions for the
		///         <code>DragGestureRecognizer</code> associated with the trigger
		///         event are equal to <code>DnDConstants.ACTION_NONE</code>. </exception>
		/// <exception cref="NullPointerException"> if dscp, trigger, or t are null, or
		///         if dragImage is non-null and offset is null </exception>
		public DragSourceContext(DragSourceContextPeer dscp, DragGestureEvent trigger, Cursor dragCursor, Image dragImage, Point offset, Transferable t, DragSourceListener dsl)
		{
			if (dscp == null)
			{
				throw new NullPointerException("DragSourceContextPeer");
			}

			if (trigger == null)
			{
				throw new NullPointerException("Trigger");
			}

			if (trigger.DragSource == null)
			{
				throw new IllegalArgumentException("DragSource");
			}

			if (trigger.Component == null)
			{
				throw new IllegalArgumentException("Component");
			}

			if (trigger.SourceAsDragGestureRecognizer.SourceActions == DnDConstants.ACTION_NONE)
			{
				throw new IllegalArgumentException("source actions");
			}

			if (trigger.DragAction == DnDConstants.ACTION_NONE)
			{
				throw new IllegalArgumentException("no drag action");
			}

			if (t == null)
			{
				throw new NullPointerException("Transferable");
			}

			if (dragImage != null && offset == null)
			{
				throw new NullPointerException("offset");
			}

			Peer = dscp;
			this.Trigger_Renamed = trigger;
			Cursor_Renamed = dragCursor;
			Transferable_Renamed = t;
			Listener = dsl;
			SourceActions_Renamed = trigger.SourceAsDragGestureRecognizer.SourceActions;

			UseCustomCursor = (dragCursor != null);

			UpdateCurrentCursor(trigger.DragAction, SourceActions, DEFAULT);
		}

		/// <summary>
		/// Returns the <code>DragSource</code>
		/// that instantiated this <code>DragSourceContext</code>.
		/// </summary>
		/// <returns> the <code>DragSource</code> that
		///   instantiated this <code>DragSourceContext</code> </returns>

		public virtual DragSource DragSource
		{
			get
			{
				return Trigger_Renamed.DragSource;
			}
		}

		/// <summary>
		/// Returns the <code>Component</code> associated with this
		/// <code>DragSourceContext</code>.
		/// </summary>
		/// <returns> the <code>Component</code> that started the drag </returns>

		public virtual Component Component
		{
			get
			{
				return Trigger_Renamed.Component;
			}
		}

		/// <summary>
		/// Returns the <code>DragGestureEvent</code>
		/// that initially triggered the drag.
		/// </summary>
		/// <returns> the Event that triggered the drag </returns>

		public virtual DragGestureEvent Trigger
		{
			get
			{
				return Trigger_Renamed;
			}
		}

		/// <summary>
		/// Returns a bitwise mask of <code>DnDConstants</code> that
		/// represent the set of drop actions supported by the drag source for the
		/// drag operation associated with this <code>DragSourceContext</code>.
		/// </summary>
		/// <returns> the drop actions supported by the drag source </returns>
		public virtual int SourceActions
		{
			get
			{
				return SourceActions_Renamed;
			}
		}

		/// <summary>
		/// Sets the cursor for this drag operation to the specified
		/// <code>Cursor</code>.  If the specified <code>Cursor</code>
		/// is <code>null</code>, the default drag cursor behavior is
		/// activated for this drag operation, otherwise it is deactivated.
		/// </summary>
		/// <param name="c">     the initial {@code Cursor} for this drag operation,
		///                       or {@code null} for the default cursor handling;
		///                       see {@link Cursor class
		///                       level documentation} for more details
		///                       on the cursor handling during drag and drop
		///  </param>

		public virtual Cursor Cursor
		{
			set
			{
				lock (this)
				{
					UseCustomCursor = (value != null);
					CursorImpl = value;
				}
			}
			get
			{
				return Cursor_Renamed;
			}
		}



		/// <summary>
		/// Add a <code>DragSourceListener</code> to this
		/// <code>DragSourceContext</code> if one has not already been added.
		/// If a <code>DragSourceListener</code> already exists,
		/// this method throws a <code>TooManyListenersException</code>.
		/// <P> </summary>
		/// <param name="dsl"> the <code>DragSourceListener</code> to add.
		/// Note that while <code>null</code> is not prohibited,
		/// it is not acceptable as a parameter.
		/// <P> </param>
		/// <exception cref="TooManyListenersException"> if
		/// a <code>DragSourceListener</code> has already been added </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void addDragSourceListener(DragSourceListener dsl) throws java.util.TooManyListenersException
		public virtual void AddDragSourceListener(DragSourceListener dsl)
		{
			lock (this)
			{
				if (dsl == null)
				{
					return;
				}
        
				if (Equals(dsl))
				{
					throw new IllegalArgumentException("DragSourceContext may not be its own listener");
				}
        
				if (Listener != null)
				{
					throw new TooManyListenersException();
				}
				else
				{
					Listener = dsl;
				}
			}
		}

		/// <summary>
		/// Removes the specified <code>DragSourceListener</code>
		/// from  this <code>DragSourceContext</code>.
		/// </summary>
		/// <param name="dsl"> the <code>DragSourceListener</code> to remove;
		///     note that while <code>null</code> is not prohibited,
		///     it is not acceptable as a parameter </param>

		public virtual void RemoveDragSourceListener(DragSourceListener dsl)
		{
			lock (this)
			{
				if (Listener != null && Listener.Equals(dsl))
				{
					Listener = null;
				}
				else
				{
					throw new IllegalArgumentException();
				}
			}
		}

		/// <summary>
		/// Notifies the peer that the <code>Transferable</code>'s
		/// <code>DataFlavor</code>s have changed.
		/// </summary>

		public virtual void TransferablesFlavorsChanged()
		{
			if (Peer != null)
			{
				Peer.TransferablesFlavorsChanged();
			}
		}

		/// <summary>
		/// Calls <code>dragEnter</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSourceContext</code> and with the associated
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceDragEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DragEnter(DragSourceDragEvent dsde)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DragEnter(dsde);
			}
			DragSource.ProcessDragEnter(dsde);

			UpdateCurrentCursor(SourceActions, dsde.TargetActions, ENTER);
		}

		/// <summary>
		/// Calls <code>dragOver</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSourceContext</code> and with the associated
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceDragEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DragOver(DragSourceDragEvent dsde)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DragOver(dsde);
			}
			DragSource.ProcessDragOver(dsde);

			UpdateCurrentCursor(SourceActions, dsde.TargetActions, OVER);
		}

		/// <summary>
		/// Calls <code>dragExit</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSourceContext</code> and with the associated
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceEvent</code>.
		/// </summary>
		/// <param name="dse"> the <code>DragSourceEvent</code> </param>
		public virtual void DragExit(DragSourceEvent dse)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DragExit(dse);
			}
			DragSource.ProcessDragExit(dse);

			UpdateCurrentCursor(DnDConstants.ACTION_NONE, DnDConstants.ACTION_NONE, DEFAULT);
		}

		/// <summary>
		/// Calls <code>dropActionChanged</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSourceContext</code> and with the associated
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceDragEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		public virtual void DropActionChanged(DragSourceDragEvent dsde)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DropActionChanged(dsde);
			}
			DragSource.ProcessDropActionChanged(dsde);

			UpdateCurrentCursor(SourceActions, dsde.TargetActions, CHANGED);
		}

		/// <summary>
		/// Calls <code>dragDropEnd</code> on the
		/// <code>DragSourceListener</code>s registered with this
		/// <code>DragSourceContext</code> and with the associated
		/// <code>DragSource</code>, and passes them the specified
		/// <code>DragSourceDropEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDropEvent</code> </param>
		public virtual void DragDropEnd(DragSourceDropEvent dsde)
		{
			DragSourceListener dsl = Listener;
			if (dsl != null)
			{
				dsl.DragDropEnd(dsde);
			}
			DragSource.ProcessDragDropEnd(dsde);
		}

		/// <summary>
		/// Calls <code>dragMouseMoved</code> on the
		/// <code>DragSourceMotionListener</code>s registered with the
		/// <code>DragSource</code> associated with this
		/// <code>DragSourceContext</code>, and them passes the specified
		/// <code>DragSourceDragEvent</code>.
		/// </summary>
		/// <param name="dsde"> the <code>DragSourceDragEvent</code>
		/// @since 1.4 </param>
		public virtual void DragMouseMoved(DragSourceDragEvent dsde)
		{
			DragSource.ProcessDragMouseMoved(dsde);
		}

		/// <summary>
		/// Returns the <code>Transferable</code> associated with
		/// this <code>DragSourceContext</code>.
		/// </summary>
		/// <returns> the <code>Transferable</code> </returns>
		public virtual Transferable Transferable
		{
			get
			{
				return Transferable_Renamed;
			}
		}

		/// <summary>
		/// If the default drag cursor behavior is active, this method
		/// sets the default drag cursor for the specified actions
		/// supported by the drag source, the drop target action,
		/// and status, otherwise this method does nothing.
		/// </summary>
		/// <param name="sourceAct"> the actions supported by the drag source </param>
		/// <param name="targetAct"> the drop target action </param>
		/// <param name="status"> one of the fields <code>DEFAULT</code>,
		///               <code>ENTER</code>, <code>OVER</code>,
		///               <code>CHANGED</code> </param>

		protected internal virtual void UpdateCurrentCursor(int sourceAct, int targetAct, int status)
		{
			lock (this)
			{
        
				// if the cursor has been previously set then don't do any defaults
				// processing.
        
				if (UseCustomCursor)
				{
					return;
				}
        
				// do defaults processing
        
				Cursor c = null;
        
				switch (status)
				{
					default:
						targetAct = DnDConstants.ACTION_NONE;
						goto case ENTER;
					case ENTER:
					case OVER:
					case CHANGED:
						int ra = sourceAct & targetAct;
        
						if (ra == DnDConstants.ACTION_NONE) // no drop possible
						{
							if ((sourceAct & DnDConstants.ACTION_LINK) == DnDConstants.ACTION_LINK)
							{
								c = DragSource.DefaultLinkNoDrop;
							}
							else if ((sourceAct & DnDConstants.ACTION_MOVE) == DnDConstants.ACTION_MOVE)
							{
								c = DragSource.DefaultMoveNoDrop;
							}
							else
							{
								c = DragSource.DefaultCopyNoDrop;
							}
						} // drop possible
						else
						{
							if ((ra & DnDConstants.ACTION_LINK) == DnDConstants.ACTION_LINK)
							{
								c = DragSource.DefaultLinkDrop;
							}
							else if ((ra & DnDConstants.ACTION_MOVE) == DnDConstants.ACTION_MOVE)
							{
								c = DragSource.DefaultMoveDrop;
							}
							else
							{
								c = DragSource.DefaultCopyDrop;
							}
						}
					break;
				}
        
				CursorImpl = c;
			}
		}

		private Cursor CursorImpl
		{
			set
			{
				if (Cursor_Renamed == null || !Cursor_Renamed.Equals(value))
				{
					Cursor_Renamed = value;
					if (Peer != null)
					{
						Peer.Cursor = Cursor_Renamed;
					}
				}
			}
		}

		/// <summary>
		/// Serializes this <code>DragSourceContext</code>. This method first
		/// performs default serialization. Next, this object's
		/// <code>Transferable</code> is written out if and only if it can be
		/// serialized. If not, <code>null</code> is written instead. In this case,
		/// a <code>DragSourceContext</code> created from the resulting deserialized
		/// stream will contain a dummy <code>Transferable</code> which supports no
		/// <code>DataFlavor</code>s. Finally, this object's
		/// <code>DragSourceListener</code> is written out if and only if it can be
		/// serialized. If not, <code>null</code> is written instead.
		/// 
		/// @serialData The default serializable fields, in alphabetical order,
		///             followed by either a <code>Transferable</code> instance, or
		///             <code>null</code>, followed by either a
		///             <code>DragSourceListener</code> instance, or
		///             <code>null</code>.
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			s.DefaultWriteObject();

			s.WriteObject(SerializationTester.Test(Transferable_Renamed) ? Transferable_Renamed : null);
			s.WriteObject(SerializationTester.Test(Listener) ? Listener : null);
		}

		/// <summary>
		/// Deserializes this <code>DragSourceContext</code>. This method first
		/// performs default deserialization for all non-<code>transient</code>
		/// fields. This object's <code>Transferable</code> and
		/// <code>DragSourceListener</code> are then deserialized as well by using
		/// the next two objects in the stream. If the resulting
		/// <code>Transferable</code> is <code>null</code>, this object's
		/// <code>Transferable</code> is set to a dummy <code>Transferable</code>
		/// which supports no <code>DataFlavor</code>s.
		/// 
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream s)
		{
			ObjectInputStream.GetField f = s.ReadFields();

			DragGestureEvent newTrigger = (DragGestureEvent)f.Get("trigger", null);
			if (newTrigger == null)
			{
				throw new InvalidObjectException("Null trigger");
			}
			if (newTrigger.DragSource == null)
			{
				throw new InvalidObjectException("Null DragSource");
			}
			if (newTrigger.Component == null)
			{
				throw new InvalidObjectException("Null trigger component");
			}

			int newSourceActions = f.Get("sourceActions", 0) & (DnDConstants.ACTION_COPY_OR_MOVE | DnDConstants.ACTION_LINK);
			if (newSourceActions == DnDConstants.ACTION_NONE)
			{
				throw new InvalidObjectException("Invalid source actions");
			}
			int triggerActions = newTrigger.DragAction;
			if (triggerActions != DnDConstants.ACTION_COPY && triggerActions != DnDConstants.ACTION_MOVE && triggerActions != DnDConstants.ACTION_LINK)
			{
				throw new InvalidObjectException("No drag action");
			}
			Trigger_Renamed = newTrigger;

			Cursor_Renamed = (Cursor)f.Get("cursor", null);
			UseCustomCursor = f.Get("useCustomCursor", false);
			SourceActions_Renamed = newSourceActions;

			Transferable_Renamed = (Transferable)s.ReadObject();
			Listener = (DragSourceListener)s.ReadObject();

			// Implementation assumes 'transferable' is never null.
			if (Transferable_Renamed == null)
			{
				if (EmptyTransferable == null)
				{
					EmptyTransferable = new TransferableAnonymousInnerClassHelper(this);
				}
				Transferable_Renamed = EmptyTransferable;
			}
		}

		private class TransferableAnonymousInnerClassHelper : Transferable
		{
			private readonly DragSourceContext OuterInstance;

			public TransferableAnonymousInnerClassHelper(DragSourceContext outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual DataFlavor[] TransferDataFlavors
			{
				get
				{
					return new DataFlavor[0];
				}
			}
			public virtual bool IsDataFlavorSupported(DataFlavor flavor)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getTransferData(java.awt.datatransfer.DataFlavor flavor) throws java.awt.datatransfer.UnsupportedFlavorException
			public virtual Object GetTransferData(DataFlavor flavor)
			{
				throw new UnsupportedFlavorException(flavor);
			}
		}

		private static Transferable EmptyTransferable;

		/*
		 * fields
		 */

		[NonSerialized]
		private DragSourceContextPeer Peer;

		/// <summary>
		/// The event which triggered the start of the drag.
		/// 
		/// @serial
		/// </summary>
		private DragGestureEvent Trigger_Renamed;

		/// <summary>
		/// The current drag cursor.
		/// 
		/// @serial
		/// </summary>
		private Cursor Cursor_Renamed;

		[NonSerialized]
		private Transferable Transferable_Renamed;

		[NonSerialized]
		private DragSourceListener Listener;

		/// <summary>
		/// <code>true</code> if the custom drag cursor is used instead of the
		/// default one.
		/// 
		/// @serial
		/// </summary>
		private bool UseCustomCursor;

		/// <summary>
		/// A bitwise mask of <code>DnDConstants</code> that represents the set of
		/// drop actions supported by the drag source for the drag operation associated
		/// with this <code>DragSourceContext.</code>
		/// 
		/// @serial
		/// </summary>
		private int SourceActions_Renamed;
	}

}