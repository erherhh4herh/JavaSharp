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
	/// A <code>Transferable</code> which implements the capability required
	/// to transfer a <code>String</code>.
	/// 
	/// This <code>Transferable</code> properly supports
	/// <code>DataFlavor.stringFlavor</code>
	/// and all equivalent flavors. Support for
	/// <code>DataFlavor.plainTextFlavor</code>
	/// and all equivalent flavors is <b>deprecated</b>. No other
	/// <code>DataFlavor</code>s are supported.
	/// </summary>
	/// <seealso cref= java.awt.datatransfer.DataFlavor#stringFlavor </seealso>
	/// <seealso cref= java.awt.datatransfer.DataFlavor#plainTextFlavor </seealso>
	public class StringSelection : Transferable, ClipboardOwner
	{

		private const int STRING = 0;
		private const int PLAIN_TEXT = 1;

		private static readonly DataFlavor[] Flavors = new DataFlavor[] {DataFlavor.StringFlavor, DataFlavor.PlainTextFlavor};

		private String Data;

		/// <summary>
		/// Creates a <code>Transferable</code> capable of transferring
		/// the specified <code>String</code>.
		/// </summary>
		public StringSelection(String data)
		{
			this.Data = data;
		}

		/// <summary>
		/// Returns an array of flavors in which this <code>Transferable</code>
		/// can provide the data. <code>DataFlavor.stringFlavor</code>
		/// is properly supported.
		/// Support for <code>DataFlavor.plainTextFlavor</code> is
		/// <b>deprecated</b>.
		/// </summary>
		/// <returns> an array of length two, whose elements are <code>DataFlavor.
		///         stringFlavor</code> and <code>DataFlavor.plainTextFlavor</code> </returns>
		public virtual DataFlavor[] TransferDataFlavors
		{
			get
			{
				// returning flavors itself would allow client code to modify
				// our internal behavior
				return (DataFlavor[])Flavors.clone();
			}
		}

		/// <summary>
		/// Returns whether the requested flavor is supported by this
		/// <code>Transferable</code>.
		/// </summary>
		/// <param name="flavor"> the requested flavor for the data </param>
		/// <returns> true if <code>flavor</code> is equal to
		///   <code>DataFlavor.stringFlavor</code> or
		///   <code>DataFlavor.plainTextFlavor</code>; false if <code>flavor</code>
		///   is not one of the above flavors </returns>
		/// <exception cref="NullPointerException"> if flavor is <code>null</code> </exception>
		public virtual bool IsDataFlavorSupported(DataFlavor flavor)
		{
			// JCK Test StringSelection0003: if 'flavor' is null, throw NPE
			for (int i = 0; i < Flavors.Length; i++)
			{
				if (flavor.Equals(Flavors[i]))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the <code>Transferable</code>'s data in the requested
		/// <code>DataFlavor</code> if possible. If the desired flavor is
		/// <code>DataFlavor.stringFlavor</code>, or an equivalent flavor,
		/// the <code>String</code> representing the selection is
		/// returned. If the desired flavor is
		/// <code>DataFlavor.plainTextFlavor</code>,
		/// or an equivalent flavor, a <code>Reader</code> is returned.
		/// <b>Note:</b> The behavior of this method for
		/// <code>DataFlavor.plainTextFlavor</code>
		/// and equivalent <code>DataFlavor</code>s is inconsistent with the
		/// definition of <code>DataFlavor.plainTextFlavor</code>.
		/// </summary>
		/// <param name="flavor"> the requested flavor for the data </param>
		/// <returns> the data in the requested flavor, as outlined above </returns>
		/// <exception cref="UnsupportedFlavorException"> if the requested data flavor is
		///         not equivalent to either <code>DataFlavor.stringFlavor</code>
		///         or <code>DataFlavor.plainTextFlavor</code> </exception>
		/// <exception cref="IOException"> if an IOException occurs while retrieving the data.
		///         By default, StringSelection never throws this exception, but a
		///         subclass may. </exception>
		/// <exception cref="NullPointerException"> if flavor is <code>null</code> </exception>
		/// <seealso cref= java.io.Reader </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getTransferData(DataFlavor flavor) throws UnsupportedFlavorException, IOException
		public virtual Object GetTransferData(DataFlavor flavor)
		{
			// JCK Test StringSelection0007: if 'flavor' is null, throw NPE
			if (flavor.Equals(Flavors[STRING]))
			{
				return (Object)Data;
			}
			else if (flavor.Equals(Flavors[PLAIN_TEXT]))
			{
				return new StringReader(Data == null ? "" : Data);
			}
			else
			{
				throw new UnsupportedFlavorException(flavor);
			}
		}

		public virtual void LostOwnership(Clipboard clipboard, Transferable contents)
		{
		}
	}

}