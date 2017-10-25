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
	/// File attributes associated with a file in a file system that supports
	/// legacy "DOS" attributes.
	/// 
	/// <para> <b>Usage Example:</b>
	/// <pre>
	///    Path file = ...
	///    DosFileAttributes attrs = Files.readAttributes(file, DosFileAttributes.class);
	/// </pre>
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface DosFileAttributes : BasicFileAttributes
	{
		/// <summary>
		/// Returns the value of the read-only attribute.
		/// 
		/// <para> This attribute is often used as a simple access control mechanism
		/// to prevent files from being deleted or updated. Whether the file system
		/// or platform does any enforcement to prevent <em>read-only</em> files
		/// from being updated is implementation specific.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the value of the read-only attribute </returns>
		bool ReadOnly {get;}

		/// <summary>
		/// Returns the value of the hidden attribute.
		/// 
		/// <para> This attribute is often used to indicate if the file is visible to
		/// users.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the value of the hidden attribute </returns>
		bool Hidden {get;}

		/// <summary>
		/// Returns the value of the archive attribute.
		/// 
		/// <para> This attribute is typically used by backup programs.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the value of the archive attribute </returns>
		bool Archive {get;}

		/// <summary>
		/// Returns the value of the system attribute.
		/// 
		/// <para> This attribute is often used to indicate that the file is a component
		/// of the operating system.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the value of the system attribute </returns>
		bool System {get;}
	}

}