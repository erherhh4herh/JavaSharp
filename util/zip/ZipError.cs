/*
 * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.zip
{

	/// <summary>
	/// Signals that an unrecoverable error has occurred.
	/// 
	/// @author  Dave Bristor
	/// @since   1.6
	/// </summary>
	public class ZipError : InternalError
	{
		private new const long SerialVersionUID = 853973422266861979L;

		/// <summary>
		/// Constructs a ZipError with the given detail message. </summary>
		/// <param name="s"> the {@code String} containing a detail message </param>
		public ZipError(String s) : base(s)
		{
		}
	}

}