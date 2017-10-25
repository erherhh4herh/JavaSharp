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
	/// An object that configures how to copy or move a file.
	/// 
	/// <para> Objects of this type may be used with the {@link
	/// Files#copy(Path,Path,CopyOption[]) Files.copy(Path,Path,CopyOption...)},
	/// {@link Files#copy(java.io.InputStream,Path,CopyOption[])
	/// Files.copy(InputStream,Path,CopyOption...)} and {@link Files#move
	/// Files.move(Path,Path,CopyOption...)} methods to configure how a file is
	/// copied or moved.
	/// 
	/// </para>
	/// <para> The <seealso cref="StandardCopyOption"/> enumeration type defines the
	/// <i>standard</i> options.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface CopyOption
	{
	}

}