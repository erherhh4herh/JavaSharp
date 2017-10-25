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
	/// The <code>DropTargetDragEvent</code> is delivered to a
	/// <code>DropTargetListener</code> via its
	/// dragEnter() and dragOver() methods.
	/// <para>
	/// The <code>DropTargetDragEvent</code> reports the <i>source drop actions</i>
	/// and the <i>user drop action</i> that reflect the current state of
	/// the drag operation.
	/// </para>
	/// <para>
	/// <i>Source drop actions</i> is a bitwise mask of <code>DnDConstants</code>
	/// that represents the set of drop actions supported by the drag source for
	/// this drag operation.
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

	public class DropTargetDragEvent : DropTargetEvent
	{

		private const long SerialVersionUID = -8422265619058953682L;

		/// <summary>
		/// Construct a <code>DropTargetDragEvent</code> given the
		/// <code>DropTargetContext</code> for this operation,
		/// the location of the "Drag" <code>Cursor</code>'s hotspot
		/// in the <code>Component</code>'s coordinates, the
		/// user drop action, and the source drop actions.
		/// <P> </summary>
		/// <param name="dtc">        The DropTargetContext for this operation </param>
		/// <param name="cursorLocn"> The location of the "Drag" Cursor's
		/// hotspot in Component coordinates </param>
		/// <param name="dropAction"> The user drop action </param>
		/// <param name="srcActions"> The source drop actions
		/// </param>
		/// <exception cref="NullPointerException"> if cursorLocn is null </exception>
		/// <exception cref="IllegalArgumentException"> if dropAction is not one of
		///         <code>DnDConstants</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if srcActions is not
		///         a bitwise mask of <code>DnDConstants</code>. </exception>
		/// <exception cref="IllegalArgumentException"> if dtc is <code>null</code>. </exception>

		public DropTargetDragEvent(DropTargetContext dtc, Point cursorLocn, int dropAction, int srcActions) : base(dtc)
		{

			if (cursorLocn == null)
			{
				throw new NullPointerException("cursorLocn");
			}

			if (dropAction != DnDConstants.ACTION_NONE && dropAction != DnDConstants.ACTION_COPY && dropAction != DnDConstants.ACTION_MOVE && dropAction != DnDConstants.ACTION_LINK)
			{
				throw new IllegalArgumentException("dropAction" + dropAction);
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
		/// This method returns a <code>Point</code>
		/// indicating the <code>Cursor</code>'s current
		/// location within the <code>Component'</code>s
		/// coordinates.
		/// <P> </summary>
		/// <returns> the current cursor location in
		/// <code>Component</code>'s coords. </returns>

		public virtual Point Location
		{
			get
			{
				return Location_Renamed;
			}
		}


		/// <summary>
		/// This method returns the current <code>DataFlavor</code>s from the
		/// <code>DropTargetContext</code>.
		/// <P> </summary>
		/// <returns> current DataFlavors from the DropTargetContext </returns>

		public virtual DataFlavor[] CurrentDataFlavors
		{
			get
			{
				return DropTargetContext.CurrentDataFlavors;
			}
		}

		/// <summary>
		/// This method returns the current <code>DataFlavor</code>s
		/// as a <code>java.util.List</code>
		/// <P> </summary>
		/// <returns> a <code>java.util.List</code> of the Current <code>DataFlavor</code>s </returns>

		public virtual IList<DataFlavor> CurrentDataFlavorsAsList
		{
			get
			{
				return DropTargetContext.CurrentDataFlavorsAsList;
			}
		}

		/// <summary>
		/// This method returns a <code>boolean</code> indicating
		/// if the specified <code>DataFlavor</code> is supported.
		/// <P> </summary>
		/// <param name="df"> the <code>DataFlavor</code> to test
		/// <P> </param>
		/// <returns> if a particular DataFlavor is supported </returns>

		public virtual bool IsDataFlavorSupported(DataFlavor df)
		{
			return DropTargetContext.IsDataFlavorSupported(df);
		}

		/// <summary>
		/// This method returns the source drop actions.
		/// </summary>
		/// <returns> the source drop actions </returns>
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
		/// <returns> the user drop action </returns>
		public virtual int DropAction
		{
			get
			{
				return DropAction_Renamed;
			}
		}

		/// <summary>
		/// This method returns the Transferable object that represents
		/// the data associated with the current drag operation.
		/// </summary>
		/// <returns> the Transferable associated with the drag operation </returns>
		/// <exception cref="InvalidDnDOperationException"> if the data associated with the drag
		///         operation is not available
		/// 
		/// @since 1.5 </exception>
		public virtual Transferable Transferable
		{
			get
			{
				return DropTargetContext.Transferable;
			}
		}

		/// <summary>
		/// Accepts the drag.
		/// 
		/// This method should be called from a
		/// <code>DropTargetListeners</code> <code>dragEnter</code>,
		/// <code>dragOver</code>, and <code>dropActionChanged</code>
		/// methods if the implementation wishes to accept an operation
		/// from the srcActions other than the one selected by
		/// the user as represented by the <code>dropAction</code>.
		/// </summary>
		/// <param name="dragOperation"> the operation accepted by the target </param>
		public virtual void AcceptDrag(int dragOperation)
		{
			DropTargetContext.AcceptDrag(dragOperation);
		}

		/// <summary>
		/// Rejects the drag as a result of examining either the
		/// <code>dropAction</code> or the available <code>DataFlavor</code>
		/// types.
		/// </summary>
		public virtual void RejectDrag()
		{
			DropTargetContext.RejectDrag();
		}

		/*
		 * fields
		 */

		/// <summary>
		/// The location of the drag cursor's hotspot in Component coordinates.
		/// 
		/// @serial
		/// </summary>
		private Point Location_Renamed;

		/// <summary>
		/// The source drop actions.
		/// 
		/// @serial
		/// </summary>
		private int Actions;

		/// <summary>
		/// The user drop action.
		/// 
		/// @serial
		/// </summary>
		private int DropAction_Renamed;
	}

}