using System.Collections.Generic;

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
	/// The <code>DropTargetDropEvent</code> is delivered
	/// via the <code>DropTargetListener</code> drop() method.
	/// <para>
	/// The <code>DropTargetDropEvent</code> reports the <i>source drop actions</i>
	/// and the <i>user drop action</i> that reflect the current state of the
	/// drag-and-drop operation.
	/// </para>
	/// <para>
	/// <i>Source drop actions</i> is a bitwise mask of <code>DnDConstants</code>
	/// that represents the set of drop actions supported by the drag source for
	/// this drag-and-drop operation.
	/// </para>
	/// <para>
	/// <i>User drop action</i> depends on the drop actions supported by the drag
	/// source and the drop action selected by the user. The user can select a drop
	/// action by pressing modifier keys during the drag operation:
	/// <pre>
	///   Ctrl + Shift -&gt; ACTION_LINK
	///   Ctrl         -&gt; ACTION_COPY
	///   Shift        -&gt; ACTION_MOVE
	/// </pre>
	/// If the user selects a drop action, the <i>user drop action</i> is one of
	/// <code>DnDConstants</code> that represents the selected drop action if this
	/// drop action is supported by the drag source or
	/// <code>DnDConstants.ACTION_NONE</code> if this drop action is not supported
	/// by the drag source.
	/// </para>
	/// <para>
	/// If the user doesn't select a drop action, the set of
	/// <code>DnDConstants</code> that represents the set of drop actions supported
	/// by the drag source is searched for <code>DnDConstants.ACTION_MOVE</code>,
	/// then for <code>DnDConstants.ACTION_COPY</code>, then for
	/// <code>DnDConstants.ACTION_LINK</code> and the <i>user drop action</i> is the
	/// first constant found. If no constant is found the <i>user drop action</i>
	/// is <code>DnDConstants.ACTION_NONE</code>.
	/// 
	/// @since 1.2
	/// </para>
	/// </summary>

	public class DropTargetDropEvent : DropTargetEvent
	{

		private const long SerialVersionUID = -1721911170440459322L;

		/// <summary>
		/// Construct a <code>DropTargetDropEvent</code> given
		/// the <code>DropTargetContext</code> for this operation,
		/// the location of the drag <code>Cursor</code>'s
		/// hotspot in the <code>Component</code>'s coordinates,
		/// the currently
		/// selected user drop action, and the current set of
		/// actions supported by the source.
		/// By default, this constructor
		/// assumes that the target is not in the same virtual machine as
		/// the source; that is, <seealso cref="#isLocalTransfer()"/> will
		/// return <code>false</code>.
		/// <P> </summary>
		/// <param name="dtc">        The <code>DropTargetContext</code> for this operation </param>
		/// <param name="cursorLocn"> The location of the "Drag" Cursor's
		/// hotspot in <code>Component</code> coordinates </param>
		/// <param name="dropAction"> the user drop action. </param>
		/// <param name="srcActions"> the source drop actions.
		/// </param>
		/// <exception cref="NullPointerException">
		/// if cursorLocn is <code>null</code> </exception>
		/// <exception cref="IllegalArgumentException">
		///         if dropAction is not one of  <code>DnDConstants</code>. </exception>
		/// <exception cref="IllegalArgumentException">
		///         if srcActions is not a bitwise mask of <code>DnDConstants</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if dtc is <code>null</code>. </exception>

		public DropTargetDropEvent(DropTargetContext dtc, Point cursorLocn, int dropAction, int srcActions) : base(dtc)
		{

			if (cursorLocn == null)
			{
				throw new NullPointerException("cursorLocn");
			}

			if (dropAction != DnDConstants.ACTION_NONE && dropAction != DnDConstants.ACTION_COPY && dropAction != DnDConstants.ACTION_MOVE && dropAction != DnDConstants.ACTION_LINK)
			{
				throw new IllegalArgumentException("dropAction = " + dropAction);
			}

			if ((srcActions & ~(DnDConstants.ACTION_COPY_OR_MOVE | DnDConstants.ACTION_LINK)) != 0)
			{
				throw new IllegalArgumentException("srcActions");
			}

			Location_Renamed = cursorLocn;
			Actions = srcActions;
			this.DropAction_Renamed = dropAction;
		}

		/// <summary>
		/// Construct a <code>DropTargetEvent</code> given the
		/// <code>DropTargetContext</code> for this operation,
		/// the location of the drag <code>Cursor</code>'s hotspot
		/// in the <code>Component</code>'s
		/// coordinates, the currently selected user drop action,
		/// the current set of actions supported by the source,
		/// and a <code>boolean</code> indicating if the source is in the same JVM
		/// as the target.
		/// <P> </summary>
		/// <param name="dtc">        The DropTargetContext for this operation </param>
		/// <param name="cursorLocn"> The location of the "Drag" Cursor's
		/// hotspot in Component's coordinates </param>
		/// <param name="dropAction"> the user drop action. </param>
		/// <param name="srcActions"> the source drop actions. </param>
		/// <param name="isLocal">  True if the source is in the same JVM as the target
		/// </param>
		/// <exception cref="NullPointerException">
		///         if cursorLocn is  <code>null</code> </exception>
		/// <exception cref="IllegalArgumentException">
		///         if dropAction is not one of <code>DnDConstants</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if srcActions is not a bitwise mask of <code>DnDConstants</code>. </exception>
		/// <exception cref="IllegalArgumentException">  if dtc is <code>null</code>. </exception>

		public DropTargetDropEvent(DropTargetContext dtc, Point cursorLocn, int dropAction, int srcActions, bool isLocal) : this(dtc, cursorLocn, dropAction, srcActions)
		{

			IsLocalTx = isLocal;
		}

		/// <summary>
		/// This method returns a <code>Point</code>
		/// indicating the <code>Cursor</code>'s current
		/// location in the <code>Component</code>'s coordinates.
		/// <P> </summary>
		/// <returns> the current <code>Cursor</code> location in Component's coords. </returns>

		public virtual Point Location
		{
			get
			{
				return Location_Renamed;
			}
		}


		/// <summary>
		/// This method returns the current DataFlavors.
		/// <P> </summary>
		/// <returns> current DataFlavors </returns>

		public virtual DataFlavor[] CurrentDataFlavors
		{
			get
			{
				return DropTargetContext.CurrentDataFlavors;
			}
		}

		/// <summary>
		/// This method returns the currently available
		/// <code>DataFlavor</code>s as a <code>java.util.List</code>.
		/// <P> </summary>
		/// <returns> the currently available DataFlavors as a java.util.List </returns>

		public virtual IList<DataFlavor> CurrentDataFlavorsAsList
		{
			get
			{
				return DropTargetContext.CurrentDataFlavorsAsList;
			}
		}

		/// <summary>
		/// This method returns a <code>boolean</code> indicating if the
		/// specified <code>DataFlavor</code> is available
		/// from the source.
		/// <P> </summary>
		/// <param name="df"> the <code>DataFlavor</code> to test
		/// <P> </param>
		/// <returns> if the DataFlavor specified is available from the source </returns>

		public virtual bool IsDataFlavorSupported(DataFlavor df)
		{
			return DropTargetContext.IsDataFlavorSupported(df);
		}

		/// <summary>
		/// This method returns the source drop actions.
		/// </summary>
		/// <returns> the source drop actions. </returns>
		public virtual int SourceActions
		{
			get
			{
				return Actions;
			}
		}

		/// <summary>
		/// This method returns the user drop action.
		/// </summary>
		/// <returns> the user drop actions. </returns>
		public virtual int DropAction
		{
			get
			{
				return DropAction_Renamed;
			}
		}

		/// <summary>
		/// This method returns the <code>Transferable</code> object
		/// associated with the drop.
		/// <P> </summary>
		/// <returns> the <code>Transferable</code> associated with the drop </returns>

		public virtual Transferable Transferable
		{
			get
			{
				return DropTargetContext.Transferable;
			}
		}

		/// <summary>
		/// accept the drop, using the specified action.
		/// <P> </summary>
		/// <param name="dropAction"> the specified action </param>

		public virtual void AcceptDrop(int dropAction)
		{
			DropTargetContext.AcceptDrop(dropAction);
		}

		/// <summary>
		/// reject the Drop.
		/// </summary>

		public virtual void RejectDrop()
		{
			DropTargetContext.RejectDrop();
		}

		/// <summary>
		/// This method notifies the <code>DragSource</code>
		/// that the drop transfer(s) are completed.
		/// <P> </summary>
		/// <param name="success"> a <code>boolean</code> indicating that the drop transfer(s) are completed. </param>

		public virtual void DropComplete(bool success)
		{
			DropTargetContext.DropComplete(success);
		}

		/// <summary>
		/// This method returns an <code>int</code> indicating if
		/// the source is in the same JVM as the target.
		/// <P> </summary>
		/// <returns> if the Source is in the same JVM </returns>

		public virtual bool LocalTransfer
		{
			get
			{
				return IsLocalTx;
			}
		}

		/*
		 * fields
		 */

		private static readonly Point Zero = new Point(0,0);

		/// <summary>
		/// The location of the drag cursor's hotspot in Component coordinates.
		/// 
		/// @serial
		/// </summary>
		private Point Location_Renamed = Zero;

		/// <summary>
		/// The source drop actions.
		/// 
		/// @serial
		/// </summary>
		private int Actions = DnDConstants.ACTION_NONE;

		/// <summary>
		/// The user drop action.
		/// 
		/// @serial
		/// </summary>
		private int DropAction_Renamed = DnDConstants.ACTION_NONE;

		/// <summary>
		/// <code>true</code> if the source is in the same JVM as the target.
		/// 
		/// @serial
		/// </summary>
		private bool IsLocalTx = false;
	}

}