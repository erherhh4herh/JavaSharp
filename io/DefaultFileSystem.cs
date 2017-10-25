/*
 * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	/// 
	/// <summary>
	/// @since 1.8
	/// </summary>
	internal class DefaultFileSystem
	{

		/// <summary>
		/// Return the FileSystem object for Windows platform.
		/// </summary>
		public static FileSystem FileSystem
		{
			get
			{
				return new WinNTFileSystem();
			}
		}
	}

}