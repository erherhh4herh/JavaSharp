using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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
	/// A <code>DropTargetContext</code> is created
	/// whenever the logical cursor associated
	/// with a Drag and Drop operation coincides with the visible geometry of
	/// a <code>Component</code> associated with a <code>DropTarget</code>.
	/// The <code>DropTargetContext</code> provides
	/// the mechanism for a potential receiver
	/// of a drop operation to both provide the end user with the appropriate
	/// drag under feedback, but also to effect the subsequent data transfer
	/// if appropriate.
	/// 
	/// @since 1.2
	/// </summary>

	[Serializable]
	public class DropTargetContext
	{

		private const long SerialVersionUID = -634158968993743371L;

		/// <summary>
		/// Construct a <code>DropTargetContext</code>
		/// given a specified <code>DropTarget</code>.
		/// <P> </summary>
		/// <param name="dt"> the DropTarget to associate with </param>

		internal DropTargetContext(DropTarget dt) : base()
		{

			DropTarget_Renamed = dt;
		}

		/// <summary>
		/// This method returns the <code>DropTarget</code> associated with this
		/// <code>DropTargetContext</code>.
		/// <P> </summary>
		/// <returns> the <code>DropTarget</code> associated with this <code>DropTargetContext</code> </returns>

		public virtual DropTarget DropTarget
		{
			get
			{
				return DropTarget_Renamed;
			}
		}

		/// <summary>
		/// This method returns the <code>Component</code> associated with
		/// this <code>DropTargetContext</code>.
		/// <P> </summary>
		/// <returns> the Component associated with this Context </returns>

		public virtual Component Component
		{
			get
			{
				return DropTarget_Renamed.Component;
			}
		}

		/// <summary>
		/// Called when associated with the <code>DropTargetContextPeer</code>.
		/// <P> </summary>
		/// <param name="dtcp"> the <code>DropTargetContextPeer</code> </param>

		public virtual void AddNotify(DropTargetContextPeer dtcp)
		{
			DropTargetContextPeer_Renamed = dtcp;
		}

		/// <summary>
		/// Called when disassociated with the <code>DropTargetContextPeer</code>.
		/// </summary>

		public virtual void RemoveNotify()
		{
			DropTargetContextPeer_Renamed = null;
			Transferable_Renamed = null;
		}

		/// <summary>
		/// This method sets the current actions acceptable to
		/// this <code>DropTarget</code>.
		/// <P> </summary>
		/// <param name="actions"> an <code>int</code> representing the supported action(s) </param>

		protected internal virtual int TargetActions
		{
			set
			{
				DropTargetContextPeer peer = DropTargetContextPeer;
				if (peer != null)
				{
					lock (peer)
					{
						peer.TargetActions = value;
						DropTarget.DoSetDefaultActions(value);
					}
				}
				else
				{
					DropTarget.DoSetDefaultActions(value);
				}
			}
			get
			{
				DropTargetContextPeer peer = DropTargetContextPeer;
				return ((peer != null) ? peer.TargetActions : DropTarget_Renamed.DefaultActions);
			}
		}



		/// <summary>
		/// This method signals that the drop is completed and
		/// if it was successful or not.
		/// <P> </summary>
		/// <param name="success"> true for success, false if not
		/// <P> </param>
		/// <exception cref="InvalidDnDOperationException"> if a drop is not outstanding/extant </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void dropComplete(boolean success) throws InvalidDnDOperationException
		public virtual void DropComplete(bool success)
		{
			DropTargetContextPeer peer = DropTargetContextPeer;
			if (peer != null)
			{
				peer.DropComplete(success);
			}
		}

		/// <summary>
		/// accept the Drag.
		/// <P> </summary>
		/// <param name="dragOperation"> the supported action(s) </param>

		protected internal virtual void AcceptDrag(int dragOperation)
		{
			DropTargetContextPeer peer = DropTargetContextPeer;
			if (peer != null)
			{
				peer.AcceptDrag(dragOperation);
			}
		}

		/// <summary>
		/// reject the Drag.
		/// </summary>

		protected internal virtual void RejectDrag()
		{
			DropTargetContextPeer peer = DropTargetContextPeer;
			if (peer != null)
			{
				peer.RejectDrag();
			}
		}

		/// <summary>
		/// called to signal that the drop is acceptable
		/// using the specified operation.
		/// must be called during DropTargetListener.drop method invocation.
		/// <P> </summary>
		/// <param name="dropOperation"> the supported action(s) </param>

		protected internal virtual void AcceptDrop(int dropOperation)
		{
			DropTargetContextPeer peer = DropTargetContextPeer;
			if (peer != null)
			{
				peer.AcceptDrop(dropOperation);
			}
		}

		/// <summary>
		/// called to signal that the drop is unacceptable.
		/// must be called during DropTargetListener.drop method invocation.
		/// </summary>

		protected internal virtual void RejectDrop()
		{
			DropTargetContextPeer peer = DropTargetContextPeer;
			if (peer != null)
			{
				peer.RejectDrop();
			}
		}

		/// <summary>
		/// get the available DataFlavors of the
		/// <code>Transferable</code> operand of this operation.
		/// <P> </summary>
		/// <returns> a <code>DataFlavor[]</code> containing the
		/// supported <code>DataFlavor</code>s of the
		/// <code>Transferable</code> operand. </returns>

		protected internal virtual DataFlavor[] CurrentDataFlavors
		{
			get
			{
				DropTargetContextPeer peer = DropTargetContextPeer;
				return peer != null ? peer.TransferDataFlavors : new DataFlavor[0];
			}
		}

		/// <summary>
		/// This method returns a the currently available DataFlavors
		/// of the <code>Transferable</code> operand
		/// as a <code>java.util.List</code>.
		/// <P> </summary>
		/// <returns> the currently available
		/// DataFlavors as a <code>java.util.List</code> </returns>

		protected internal virtual IList<DataFlavor> CurrentDataFlavorsAsList
		{
			get
			{
				return Arrays.AsList(CurrentDataFlavors);
			}
		}

		/// <summary>
		/// This method returns a <code>boolean</code>
		/// indicating if the given <code>DataFlavor</code> is
		/// supported by this <code>DropTargetContext</code>.
		/// <P> </summary>
		/// <param name="df"> the <code>DataFlavor</code>
		/// <P> </param>
		/// <returns> if the <code>DataFlavor</code> specified is supported </returns>

		protected internal virtual bool IsDataFlavorSupported(DataFlavor df)
		{
			return CurrentDataFlavorsAsList.Contains(df);
		}

		/// <summary>
		/// get the Transferable (proxy) operand of this operation
		/// <P> </summary>
		/// <exception cref="InvalidDnDOperationException"> if a drag is not outstanding/extant
		/// <P> </exception>
		/// <returns> the <code>Transferable</code> </returns>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.awt.datatransfer.Transferable getTransferable() throws InvalidDnDOperationException
		protected internal virtual Transferable Transferable
		{
			get
			{
				DropTargetContextPeer peer = DropTargetContextPeer;
				if (peer == null)
				{
					throw new InvalidDnDOperationException();
				}
				else
				{
					if (Transferable_Renamed == null)
					{
						Transferable t = peer.Transferable;
						bool isLocal = peer.TransferableJVMLocal;
						lock (this)
						{
							if (Transferable_Renamed == null)
							{
								Transferable_Renamed = CreateTransferableProxy(t, isLocal);
							}
						}
					}
    
					return Transferable_Renamed;
				}
			}
		}

		/// <summary>
		/// Get the <code>DropTargetContextPeer</code>
		/// <P> </summary>
		/// <returns> the platform peer </returns>

		internal virtual DropTargetContextPeer DropTargetContextPeer
		{
			get
			{
				return DropTargetContextPeer_Renamed;
			}
		}

		/// <summary>
		/// Creates a TransferableProxy to proxy for the specified
		/// Transferable.
		/// </summary>
		/// <param name="t"> the <tt>Transferable</tt> to be proxied </param>
		/// <param name="local"> <tt>true</tt> if <tt>t</tt> represents
		///        the result of a local drag-n-drop operation. </param>
		/// <returns> the new <tt>TransferableProxy</tt> instance. </returns>
		protected internal virtual Transferable CreateTransferableProxy(Transferable t, bool local)
		{
			return new TransferableProxy(this, t, local);
		}

	/// <summary>
	///************************************************************************* </summary>


		/// <summary>
		/// <code>TransferableProxy</code> is a helper inner class that implements
		/// <code>Transferable</code> interface and serves as a proxy for another
		/// <code>Transferable</code> object which represents data transfer for
		/// a particular drag-n-drop operation.
		/// <para>
		/// The proxy forwards all requests to the encapsulated transferable
		/// and automatically performs additional conversion on the data
		/// returned by the encapsulated transferable in case of local transfer.
		/// </para>
		/// </summary>

		protected internal class TransferableProxy : Transferable
		{
			private readonly DropTargetContext OuterInstance;


			/// <summary>
			/// Constructs a <code>TransferableProxy</code> given
			/// a specified <code>Transferable</code> object representing
			/// data transfer for a particular drag-n-drop operation and
			/// a <code>boolean</code> which indicates whether the
			/// drag-n-drop operation is local (within the same JVM).
			/// <para>
			/// </para>
			/// </summary>
			/// <param name="t"> the <code>Transferable</code> object </param>
			/// <param name="local"> <code>true</code>, if <code>t</code> represents
			///        the result of local drag-n-drop operation </param>
			internal TransferableProxy(DropTargetContext outerInstance, Transferable t, bool local)
			{
				this.OuterInstance = outerInstance;
				Proxy = new sun.awt.datatransfer.TransferableProxy(t, local);
				Transferable = t;
				IsLocal = local;
			}

			/// <summary>
			/// Returns an array of DataFlavor objects indicating the flavors
			/// the data can be provided in by the encapsulated transferable.
			/// <para>
			/// </para>
			/// </summary>
			/// <returns> an array of data flavors in which the data can be
			///         provided by the encapsulated transferable </returns>
			public virtual DataFlavor[] TransferDataFlavors
			{
				get
				{
					return Proxy.TransferDataFlavors;
				}
			}

			/// <summary>
			/// Returns whether or not the specified data flavor is supported by
			/// the encapsulated transferable. </summary>
			/// <param name="flavor"> the requested flavor for the data </param>
			/// <returns> <code>true</code> if the data flavor is supported,
			///         <code>false</code> otherwise </returns>
			public virtual bool IsDataFlavorSupported(DataFlavor flavor)
			{
				return Proxy.isDataFlavorSupported(flavor);
			}

			/// <summary>
			/// Returns an object which represents the data provided by
			/// the encapsulated transferable for the requested data flavor.
			/// <para>
			/// In case of local transfer a serialized copy of the object
			/// returned by the encapsulated transferable is provided when
			/// the data is requested in application/x-java-serialized-object
			/// data flavor.
			/// 
			/// </para>
			/// </summary>
			/// <param name="df"> the requested flavor for the data </param>
			/// <exception cref="IOException"> if the data is no longer available
			///              in the requested flavor. </exception>
			/// <exception cref="UnsupportedFlavorException"> if the requested data flavor is
			///              not supported. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getTransferData(java.awt.datatransfer.DataFlavor df) throws java.awt.datatransfer.UnsupportedFlavorException, java.io.IOException
			public virtual Object GetTransferData(DataFlavor df)
			{
				return Proxy.getTransferData(df);
			}

			/*
			 * fields
			 */

			// We don't need to worry about client code changing the values of
			// these variables. Since TransferableProxy is a protected class, only
			// subclasses of DropTargetContext can access it. And DropTargetContext
			// cannot be subclassed by client code because it does not have a
			// public constructor.

			/// <summary>
			/// The encapsulated <code>Transferable</code> object.
			/// </summary>
			protected internal Transferable Transferable;

			/// <summary>
			/// A <code>boolean</code> indicating if the encapsulated
			/// <code>Transferable</code> object represents the result
			/// of local drag-n-drop operation (within the same JVM).
			/// </summary>
			protected internal bool IsLocal;

			internal sun.awt.datatransfer.TransferableProxy Proxy;
		}

	/// <summary>
	///************************************************************************* </summary>

		/*
		 * fields
		 */

		/// <summary>
		/// The DropTarget associated with this DropTargetContext.
		/// 
		/// @serial
		/// </summary>
		private DropTarget DropTarget_Renamed;

		[NonSerialized]
		private DropTargetContextPeer DropTargetContextPeer_Renamed;

		[NonSerialized]
		private Transferable Transferable_Renamed;
	}

}