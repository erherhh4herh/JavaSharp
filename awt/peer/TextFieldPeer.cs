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
	/// The peer interface for <seealso cref="TextField"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface TextFieldPeer : TextComponentPeer
	{

		/// <summary>
		/// Sets the echo character.
		/// </summary>
		/// <param name="echoChar"> the echo character to set
		/// </param>
		/// <seealso cref= TextField#getEchoChar() </seealso>
		char EchoChar {set;}

		/// <summary>
		/// Returns the preferred size of the text field with the specified number
		/// of columns.
		/// </summary>
		/// <param name="columns"> the number of columns
		/// </param>
		/// <returns> the preferred size of the text field
		/// </returns>
		/// <seealso cref= TextField#getPreferredSize(int) </seealso>
		Dimension GetPreferredSize(int columns);

		/// <summary>
		/// Returns the minimum size of the text field with the specified number
		/// of columns.
		/// </summary>
		/// <param name="columns"> the number of columns
		/// </param>
		/// <returns> the minimum size of the text field
		/// </returns>
		/// <seealso cref= TextField#getMinimumSize(int) </seealso>
		Dimension GetMinimumSize(int columns);

	}

}