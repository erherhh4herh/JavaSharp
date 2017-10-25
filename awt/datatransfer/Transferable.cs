/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Defines the interface for classes that can be used to provide data
	/// for a transfer operation.
	/// <para>
	/// For information on using data transfer with Swing, see
	/// <a href="https://docs.oracle.com/javase/tutorial/uiswing/dnd/index.html">
	/// How to Use Drag and Drop and Data Transfer</a>,
	/// a section in <em>The Java Tutorial</em>, for more information.
	/// 
	/// @author      Amy Fowler
	/// </para>
	/// </summary>

	public interface Transferable
	{

		/// <summary>
		/// Returns an array of DataFlavor objects indicating the flavors the data
		/// can be provided in.  The array should be ordered according to preference
		/// for providing the data (from most richly descriptive to least descriptive). </summary>
		/// <returns> an array of data flavors in which this data can be transferred </returns>
		DataFlavor[] TransferDataFlavors {get;}

		/// <summary>
		/// Returns whether or not the specified data flavor is supported for
		/// this object. </summary>
		/// <param name="flavor"> the requested flavor for the data </param>
		/// <returns> boolean indicating whether or not the data flavor is supported </returns>
		bool IsDataFlavorSupported(DataFlavor flavor);

		/// <summary>
		/// Returns an object which represents the data to be transferred.  The class
		/// of the object returned is defined by the representation class of the flavor.
		/// </summary>
		/// <param name="flavor"> the requested flavor for the data </param>
		/// <seealso cref= DataFlavor#getRepresentationClass </seealso>
		/// <exception cref="IOException">                if the data is no longer available
		///              in the requested flavor. </exception>
		/// <exception cref="UnsupportedFlavorException"> if the requested data flavor is
		///              not supported. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getTransferData(DataFlavor flavor) throws UnsupportedFlavorException, java.io.IOException;
		Object GetTransferData(DataFlavor flavor);

	}

}