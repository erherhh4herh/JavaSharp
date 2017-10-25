/*
 * Copyright (c) 1995, 2005, Oracle and/or its affiliates. All rights reserved.
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
namespace java.awt.peer
{

	/// <summary>
	/// The peer interface for <seealso cref="Container"/>. This is the parent interface
	/// for all container like widgets.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface ContainerPeer : ComponentPeer
	{

		/// <summary>
		/// Returns the insets of this container. Insets usually is the space that
		/// is occupied by things like borders.
		/// </summary>
		/// <returns> the insets of this container </returns>
		Insets Insets {get;}

		/// <summary>
		/// Notifies the peer that validation of the component tree is about to
		/// begin.
		/// </summary>
		/// <seealso cref= Container#validate() </seealso>
		void BeginValidate();

		/// <summary>
		/// Notifies the peer that validation of the component tree is finished.
		/// </summary>
		/// <seealso cref= Container#validate() </seealso>
		void EndValidate();

		/// <summary>
		/// Notifies the peer that layout is about to begin. This is called
		/// before the container itself and its children are laid out.
		/// </summary>
		/// <seealso cref= Container#validateTree() </seealso>
		void BeginLayout();

		/// <summary>
		/// Notifies the peer that layout is finished. This is called after the
		/// container and its children have been laid out.
		/// </summary>
		/// <seealso cref= Container#validateTree() </seealso>
		void EndLayout();
	}

}