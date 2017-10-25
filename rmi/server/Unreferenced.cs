/*
 * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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

namespace java.rmi.server
{

	/// <summary>
	/// A remote object implementation should implement the
	/// <code>Unreferenced</code> interface to receive notification when there are
	/// no more clients that reference that remote object.
	/// 
	/// @author  Ann Wollrath
	/// @author  Roger Riggs
	/// @since   JDK1.1
	/// </summary>
	public interface Unreferenced
	{
		/// <summary>
		/// Called by the RMI runtime sometime after the runtime determines that
		/// the reference list, the list of clients referencing the remote object,
		/// becomes empty.
		/// @since JDK1.1
		/// </summary>
		void Unreferenced();
	}

}