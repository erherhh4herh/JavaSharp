using System;

/*
 * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.image
{


	/// <summary>
	/// The <code>RasterFormatException</code> is thrown if there is
	/// invalid layout information in the <seealso cref="Raster"/>.
	/// </summary>
	public class RasterFormatException : Exception
	{

		/// <summary>
		/// Constructs a new <code>RasterFormatException</code> with the
		/// specified message. </summary>
		/// <param name="s"> the message to generate when a
		/// <code>RasterFormatException</code> is thrown </param>
		public RasterFormatException(String s) : base(s)
		{
		}
	}

}