/*
 * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	/// The DropTargetPeer class is the interface to the platform dependent
	/// DnD facilities. Since the DnD system is based on the native platform's
	/// facilities, a DropTargetPeer will be associated with a ComponentPeer
	/// of the nearsest enclosing native Container (in the case of lightweights)
	/// </para>
	/// 
	/// @since 1.2
	/// 
	/// </summary>

	public interface DropTargetPeer
	{

		/// <summary>
		/// Add the DropTarget to the System
		/// </summary>
		/// <param name="dt"> The DropTarget effected </param>

		void AddDropTarget(DropTarget dt);

		/// <summary>
		/// Remove the DropTarget from the system
		/// </summary>
		/// <param name="dt"> The DropTarget effected </param>

		void RemoveDropTarget(DropTarget dt);
	}

}