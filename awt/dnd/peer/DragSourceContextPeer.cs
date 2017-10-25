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
	/// This interface is supplied by the underlying window system platform to
	/// expose the behaviors of the Drag and Drop system to an originator of
	/// the same
	/// </para>
	/// 
	/// @since 1.2
	/// 
	/// </summary>

	public interface DragSourceContextPeer
	{

		/// <summary>
		/// start a drag
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void startDrag(java.awt.dnd.DragSourceContext dsc, java.awt.Cursor c, java.awt.Image dragImage, java.awt.Point imageOffset) throws java.awt.dnd.InvalidDnDOperationException;
		void StartDrag(DragSourceContext dsc, Cursor c, Image dragImage, Point imageOffset);

		/// <summary>
		/// return the current drag cursor
		/// </summary>

		Cursor Cursor {get;set;}

		/// <summary>
		/// set the current drag cursor
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setCursor(java.awt.Cursor c) throws java.awt.dnd.InvalidDnDOperationException;

		/// <summary>
		/// notify the peer that the Transferables DataFlavors have changed
		/// </summary>

		void TransferablesFlavorsChanged();
	}

}