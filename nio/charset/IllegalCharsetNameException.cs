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
	/// Unchecked exception thrown when a string that is not a
	/// <a href=Charset.html#names>legal charset name</a> is used as such.
	/// 
	/// @since 1.4
	/// </summary>

	public class IllegalCharsetNameException : IllegalArgumentException
	{

		private new const long SerialVersionUID = 1457525358470002989L;

		private String CharsetName_Renamed;

		/// <summary>
		/// Constructs an instance of this class.
		/// </summary>
		/// <param name="charsetName">
		///         The illegal charset name </param>
		public IllegalCharsetNameException(String charsetName) : base(Convert.ToString(charsetName))
		{
		this.CharsetName_Renamed = charsetName;
		}

		/// <summary>
		/// Retrieves the illegal charset name.
		/// </summary>
		/// <returns>  The illegal charset name </returns>
		public virtual String CharsetName
		{
			get
			{
				return CharsetName_Renamed;
			}
		}

	}

}