/*
 * Copyright (c) 1995, 1998, Oracle and/or its affiliates. All rights reserved.
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
	/// The peer interface for <seealso cref="Scrollbar"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface ScrollbarPeer : ComponentPeer
	{

		/// <summary>
		/// Sets the parameters for the scrollbar.
		/// </summary>
		/// <param name="value"> the current value </param>
		/// <param name="visible"> how much of the whole scale is visible </param>
		/// <param name="minimum"> the minimum value </param>
		/// <param name="maximum"> the maximum value
		/// </param>
		/// <seealso cref= Scrollbar#setValues(int, int, int, int) </seealso>
		void SetValues(int value, int visible, int minimum, int maximum);

		/// <summary>
		/// Sets the line increment of the scrollbar.
		/// </summary>
		/// <param name="l"> the line increment
		/// </param>
		/// <seealso cref= Scrollbar#setLineIncrement(int) </seealso>
		int LineIncrement {set;}

		/// <summary>
		/// Sets the page increment of the scrollbar.
		/// </summary>
		/// <param name="l"> the page increment
		/// </param>
		/// <seealso cref= Scrollbar#setPageIncrement(int) </seealso>
		int PageIncrement {set;}
	}

}