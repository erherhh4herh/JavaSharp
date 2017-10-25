/*
 * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// The peer interface for <seealso cref="TexTArea"/>.
	/// 
	/// The peer interfaces are intended only for use in porting
	/// the AWT. They are not intended for use by application
	/// developers, and developers should not implement peers
	/// nor invoke any of the peer methods directly on the peer
	/// instances.
	/// </summary>
	public interface TextAreaPeer : TextComponentPeer
	{

		/// <summary>
		/// Inserts the specified text at the specified position in the document.
		/// </summary>
		/// <param name="text"> the text to insert </param>
		/// <param name="pos"> the position to insert
		/// </param>
		/// <seealso cref= TextArea#insert(String, int) </seealso>
		void Insert(String text, int pos);

		/// <summary>
		/// Replaces a range of text by the specified string.
		/// </summary>
		/// <param name="text"> the replacement string </param>
		/// <param name="start"> the begin of the range to replace </param>
		/// <param name="end"> the end of the range to replace
		/// </param>
		/// <seealso cref= TextArea#replaceRange(String, int, int) </seealso>
		void ReplaceRange(String text, int start, int end);

		/// <summary>
		/// Returns the preferred size of a textarea with the specified number of
		/// columns and rows.
		/// </summary>
		/// <param name="rows"> the number of rows </param>
		/// <param name="columns"> the number of columns
		/// </param>
		/// <returns> the preferred size of a textarea
		/// </returns>
		/// <seealso cref= TextArea#getPreferredSize(int, int) </seealso>
		Dimension GetPreferredSize(int rows, int columns);

		/// <summary>
		/// Returns the minimum size of a textarea with the specified number of
		/// columns and rows.
		/// </summary>
		/// <param name="rows"> the number of rows </param>
		/// <param name="columns"> the number of columns
		/// </param>
		/// <returns> the minimum size of a textarea
		/// </returns>
		/// <seealso cref= TextArea#getMinimumSize(int, int) </seealso>
		Dimension GetMinimumSize(int rows, int columns);

	}

}