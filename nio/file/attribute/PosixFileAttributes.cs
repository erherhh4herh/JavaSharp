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

namespace java.nio.file.attribute
{

	/// <summary>
	/// File attributes associated with files on file systems used by operating systems
	/// that implement the Portable Operating System Interface (POSIX) family of
	/// standards.
	/// 
	/// <para> The POSIX attributes of a file are retrieved using a {@link
	/// PosixFileAttributeView} by invoking its {@link
	/// PosixFileAttributeView#readAttributes readAttributes} method.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface PosixFileAttributes : BasicFileAttributes
	{
		/// <summary>
		/// Returns the owner of the file.
		/// </summary>
		/// <returns>  the file owner
		/// </returns>
		/// <seealso cref= PosixFileAttributeView#setOwner </seealso>
		UserPrincipal Owner();

		/// <summary>
		/// Returns the group owner of the file.
		/// </summary>
		/// <returns>  the file group owner
		/// </returns>
		/// <seealso cref= PosixFileAttributeView#setGroup </seealso>
		GroupPrincipal Group();

		/// <summary>
		/// Returns the permissions of the file. The file permissions are returned
		/// as a set of <seealso cref="PosixFilePermission"/> elements. The returned set is a
		/// copy of the file permissions and is modifiable. This allows the result
		/// to be modified and passed to the {@link PosixFileAttributeView#setPermissions
		/// setPermissions} method to update the file's permissions.
		/// </summary>
		/// <returns>  the file permissions
		/// </returns>
		/// <seealso cref= PosixFileAttributeView#setPermissions </seealso>
		Set<PosixFilePermission> Permissions();
	}

}