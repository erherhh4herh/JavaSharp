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

namespace java.net
{

	/// <summary>
	/// A simple interface which provides a mechanism to map
	/// between a file name and a MIME type string.
	/// 
	/// @author  Steven B. Byrne
	/// @since   JDK1.1
	/// </summary>
	public interface FileNameMap
	{

		/// <summary>
		/// Gets the MIME type for the specified file name. </summary>
		/// <param name="fileName"> the specified file name </param>
		/// <returns> a {@code String} indicating the MIME
		/// type for the specified file name. </returns>
		String GetContentTypeFor(String fileName);
	}

}