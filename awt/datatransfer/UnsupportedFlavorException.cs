using System;

/*
 * Copyright (c) 1996, 2000, Oracle and/or its affiliates. All rights reserved.
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
	/// Signals that the requested data is not supported in this flavor. </summary>
	/// <seealso cref= Transferable#getTransferData
	/// 
	/// @author      Amy Fowler </seealso>
	public class UnsupportedFlavorException : Exception
	{

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private new const long SerialVersionUID = 5383814944251665601L;

		/// <summary>
		/// Constructs an UnsupportedFlavorException.
		/// </summary>
		/// <param name="flavor"> the flavor object which caused the exception. May
		///        be <code>null</code>. </param>
		public UnsupportedFlavorException(DataFlavor flavor) : base((flavor != null) ? flavor.HumanPresentableName : null)
		{
		}
	}

}