/*
 * Copyright (c) 1995, 2007, Oracle and/or its affiliates. All rights reserved.
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
	/// The peer interface for <seealso cref="Canvas"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface CanvasPeer : ComponentPeer
	{
		/// <summary>
		/// Requests a GC that best suits this Canvas. The returned GC may differ
		/// from the requested GC passed as the argument to this method. This method
		/// must return a non-null value (given the argument is non-null as well).
		/// 
		/// @since 1.7
		/// </summary>
		GraphicsConfiguration GetAppropriateGraphicsConfiguration(GraphicsConfiguration gc);
	}

}