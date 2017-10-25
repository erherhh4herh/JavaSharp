/*
 * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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
	/// Defines the standard open options.
	/// 
	/// @since 1.7
	/// </summary>

//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: public enum StandardOpenOption implements OpenOption
	public enum StandardOpenOption
	{
		/// <summary>
		/// Open for read access.
		/// </summary>
		READ,

		/// <summary>
		/// Open for write access.
		/// </summary>
		WRITE,

		/// <summary>
		/// If the file is opened for <seealso cref="#WRITE"/> access then bytes will be written
		/// to the end of the file rather than the beginning.
		/// 
		/// <para> If the file is opened for write access by other programs, then it
		/// is file system specific if writing to the end of the file is atomic.
		/// </para>
		/// </summary>
		APPEND,

		/// <summary>
		/// If the file already exists and it is opened for <seealso cref="#WRITE"/>
		/// access, then its length is truncated to 0. This option is ignored
		/// if the file is opened only for <seealso cref="#READ"/> access.
		/// </summary>
		TRUNCATE_EXISTING,

		/// <summary>
		/// Create a new file if it does not exist.
		/// This option is ignored if the <seealso cref="#CREATE_NEW"/> option is also set.
		/// The check for the existence of the file and the creation of the file
		/// if it does not exist is atomic with respect to other file system
		/// operations.
		/// </summary>
		CREATE,

		/// <summary>
		/// Create a new file, failing if the file already exists.
		/// The check for the existence of the file and the creation of the file
		/// if it does not exist is atomic with respect to other file system
		/// operations.
		/// </summary>
		CREATE_NEW,

		/// <summary>
		/// Delete on close. When this option is present then the implementation
		/// makes a <em>best effort</em> attempt to delete the file when closed
		/// by the appropriate {@code close} method. If the {@code close} method is
		/// not invoked then a <em>best effort</em> attempt is made to delete the
		/// file when the Java virtual machine terminates (either normally, as
		/// defined by the Java Language Specification, or where possible, abnormally).
		/// This option is primarily intended for use with <em>work files</em> that
		/// are used solely by a single instance of the Java virtual machine. This
		/// option is not recommended for use when opening files that are open
		/// concurrently by other entities. Many of the details as to when and how
		/// the file is deleted are implementation specific and therefore not
		/// specified. In particular, an implementation may be unable to guarantee
		/// that it deletes the expected file when replaced by an attacker while the
		/// file is open. Consequently, security sensitive applications should take
		/// care when using this option.
		/// 
		/// <para> For security reasons, this option may imply the {@link
		/// LinkOption#NOFOLLOW_LINKS} option. In other words, if the option is present
		/// when opening an existing file that is a symbolic link then it may fail
		/// (by throwing <seealso cref="java.io.IOException"/>).
		/// </para>
		/// </summary>
		DELETE_ON_CLOSE,

		/// <summary>
		/// Sparse file. When used with the <seealso cref="#CREATE_NEW"/> option then this
		/// option provides a <em>hint</em> that the new file will be sparse. The
		/// option is ignored when the file system does not support the creation of
		/// sparse files.
		/// </summary>
		SPARSE,

		/// <summary>
		/// Requires that every update to the file's content or metadata be written
		/// synchronously to the underlying storage device.
		/// </summary>
		/// <seealso cref= <a href="package-summary.html#integrity">Synchronized I/O file integrity</a> </seealso>
		SYNC,

		/// <summary>
		/// Requires that every update to the file's content be written
		/// synchronously to the underlying storage device.
		/// </summary>
		/// <seealso cref= <a href="package-summary.html#integrity">Synchronized I/O file integrity</a> </seealso>
		DSYNC
	}

}