/*
 * Copyright (c) 1996, 2002, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.datatransfer
{

	/// <summary>
	/// Defines the interface for classes that will provide data to
	/// a clipboard. An instance of this interface becomes the owner
	/// of the contents of a clipboard (clipboard owner) if it is
	/// passed as an argument to
	/// <seealso cref="java.awt.datatransfer.Clipboard#setContents"/> method of
	/// the clipboard and this method returns successfully.
	/// The instance remains the clipboard owner until another application
	/// or another object within this application asserts ownership
	/// of this clipboard.
	/// </summary>
	/// <seealso cref= java.awt.datatransfer.Clipboard
	/// 
	/// @author      Amy Fowler </seealso>

	public interface ClipboardOwner
	{

		/// <summary>
		/// Notifies this object that it is no longer the clipboard owner.
		/// This method will be called when another application or another
		/// object within this application asserts ownership of the clipboard.
		/// </summary>
		/// <param name="clipboard"> the clipboard that is no longer owned </param>
		/// <param name="contents"> the contents which this owner had placed on the clipboard </param>
		void LostOwnership(Clipboard clipboard, Transferable contents);

	}

}