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
	/// The peer interface for <seealso cref="Label"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface LabelPeer : ComponentPeer
	{

		/// <summary>
		/// Sets the text to be displayed on the label.
		/// </summary>
		/// <param name="label"> the text to be displayed on the label
		/// </param>
		/// <seealso cref= Label#setText </seealso>
		String Text {set;}

		/// <summary>
		/// Sets the alignment of the label text.
		/// </summary>
		/// <param name="alignment"> the alignment of the label text
		/// </param>
		/// <seealso cref= Label#setAlignment(int) </seealso>
		/// <seealso cref= Label#CENTER </seealso>
		/// <seealso cref= Label#RIGHT </seealso>
		/// <seealso cref= Label#LEFT </seealso>
		int Alignment {set;}
	}

}