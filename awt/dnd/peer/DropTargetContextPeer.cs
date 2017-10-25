/*
 * Copyright (c) 1997, 2007, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.dnd.peer
{


	/// <summary>
	/// <para>
	/// This interface is exposed by the underlying window system platform to
	/// enable control of platform DnD operations
	/// </para>
	/// 
	/// @since 1.2
	/// 
	/// </summary>

	public interface DropTargetContextPeer
	{

		/// <summary>
		/// update the peer's notion of the Target's actions
		/// </summary>

		int TargetActions {set;get;}



		/// <summary>
		/// get the DropTarget associated with this peer
		/// </summary>

		DropTarget DropTarget {get;}

		/// <summary>
		/// get the (remote) DataFlavors from the peer
		/// </summary>

		DataFlavor[] TransferDataFlavors {get;}

		/// <summary>
		/// get an input stream to the remote data
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.awt.datatransfer.Transferable getTransferable() throws java.awt.dnd.InvalidDnDOperationException;
		Transferable Transferable {get;}

		/// <returns> if the DragSource Transferable is in the same JVM as the Target </returns>

		bool TransferableJVMLocal {get;}

		/// <summary>
		/// accept the Drag
		/// </summary>

		void AcceptDrag(int dragAction);

		/// <summary>
		/// reject the Drag
		/// </summary>

		void RejectDrag();

		/// <summary>
		/// accept the Drop
		/// </summary>

		void AcceptDrop(int dropAction);

		/// <summary>
		/// reject the Drop
		/// </summary>

		void RejectDrop();

		/// <summary>
		/// signal complete
		/// </summary>

		void DropComplete(bool success);

	}

}