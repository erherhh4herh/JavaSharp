/*
 * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{

	/// <summary>
	/// An object that configures how to open or create a file.
	/// 
	/// <para> Objects of this type are used by methods such as {@link
	/// Files#newOutputStream(Path,OpenOption[]) newOutputStream}, {@link
	/// Files#newByteChannel newByteChannel}, {@link
	/// java.nio.channels.FileChannel#open FileChannel.open}, and {@link
	/// java.nio.channels.AsynchronousFileChannel#open AsynchronousFileChannel.open}
	/// when opening or creating a file.
	/// 
	/// </para>
	/// <para> The <seealso cref="StandardOpenOption"/> enumeration type defines the
	/// <i>standard</i> options.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface OpenOption
	{
	}

}