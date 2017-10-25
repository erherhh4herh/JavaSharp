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
	/// The peer interface for <seealso cref="Button"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface ButtonPeer : ComponentPeer
	{

		/// <summary>
		/// Sets the label that is displayed on the button. Can be {@code null}
		/// when the button should not display a label.
		/// </summary>
		/// <param name="label"> the label string to set
		/// </param>
		/// <seealso cref= Button#setLabel </seealso>
		String Label {set;}
	}

}