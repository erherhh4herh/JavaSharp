using System;

/*
 * Copyright (c) 2001, 2007, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.charset
{


	/// <summary>
	/// Error thrown when the <seealso cref="CharsetDecoder#decodeLoop decodeLoop"/> method of
	/// a <seealso cref="CharsetDecoder"/>, or the {@link CharsetEncoder#encodeLoop
	/// encodeLoop} method of a <seealso cref="CharsetEncoder"/>, throws an unexpected
	/// exception.
	/// 
	/// @since 1.4
	/// </summary>

	public class CoderMalfunctionError : Error
	{

		private new const long SerialVersionUID = -1151412348057794301L;

		/// <summary>
		/// Initializes an instance of this class.
		/// </summary>
		/// <param name="cause">
		///         The unexpected exception that was thrown </param>
		public CoderMalfunctionError(Exception cause) : base(cause)
		{
		}

	}

}