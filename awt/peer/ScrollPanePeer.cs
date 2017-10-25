/*
 * Copyright (c) 1996, 2007, Oracle and/or its affiliates. All rights reserved.
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
	/// The peer interface for <seealso cref="ScrollPane"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface ScrollPanePeer : ContainerPeer
	{

		/// <summary>
		/// Returns the height of the horizontal scroll bar.
		/// </summary>
		/// <returns> the height of the horizontal scroll bar
		/// </returns>
		/// <seealso cref= ScrollPane#getHScrollbarHeight() </seealso>
		int HScrollbarHeight {get;}

		/// <summary>
		/// Returns the width of the vertical scroll bar.
		/// </summary>
		/// <returns> the width of the vertical scroll bar
		/// </returns>
		/// <seealso cref= ScrollPane#getVScrollbarWidth() </seealso>
		int VScrollbarWidth {get;}

		/// <summary>
		/// Sets the scroll position of the child.
		/// </summary>
		/// <param name="x"> the X coordinate of the scroll position </param>
		/// <param name="y"> the Y coordinate of the scroll position
		/// </param>
		/// <seealso cref= ScrollPane#setScrollPosition(int, int) </seealso>
		void SetScrollPosition(int x, int y);

		/// <summary>
		/// Called when the child component changes its size.
		/// </summary>
		/// <param name="w"> the new width of the child component </param>
		/// <param name="h"> the new height of the child component
		/// </param>
		/// <seealso cref= ScrollPane#layout() </seealso>
		void ChildResized(int w, int h);

		/// <summary>
		/// Sets the unit increment of one of the scroll pane's adjustables.
		/// </summary>
		/// <param name="adj"> the scroll pane adjustable object </param>
		/// <param name="u"> the unit increment
		/// </param>
		/// <seealso cref= ScrollPaneAdjustable#setUnitIncrement(int) </seealso>
		void SetUnitIncrement(Adjustable adj, int u);

		/// <summary>
		/// Sets the value for one of the scroll pane's adjustables.
		/// </summary>
		/// <param name="adj"> the scroll pane adjustable object </param>
		/// <param name="v"> the value to set </param>
		void SetValue(Adjustable adj, int v);
	}

}