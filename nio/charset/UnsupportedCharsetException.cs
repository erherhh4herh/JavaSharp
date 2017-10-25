using System;

/*
 * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
 *
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
 *
 */

// -- This file was mechanically generated: Do not edit! -- //

namespace java.nio.charset
{


	/// <summary>
	/// Unchecked exception thrown when no support is available
	/// for a requested charset.
	/// 
	/// @since 1.4
	/// </summary>

	public class UnsupportedCharsetException : IllegalArgumentException
	{

		private new const long SerialVersionUID = 1490765524727386367L;

		private String CharsetName_Renamed;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="charsetName">
		///         The name of the unsupported charset </param>
		public UnsupportedCharsetException(String charsetName) : base(Convert.ToString(charsetName))
		{
		this.CharsetName_Renamed = charsetName;
		}

		/// <summary>
		/// Retrieves the name of the unsupported charset.
		/// </summary>
		/// <returns>  The name of the unsupported charset </returns>
		public virtual String CharsetName
		{
			get
			{
				return CharsetName_Renamed;
			}
		}

	}

}