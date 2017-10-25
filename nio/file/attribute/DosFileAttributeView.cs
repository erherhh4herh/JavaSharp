/*
 * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// A file attribute view that provides a view of the legacy "DOS" file attributes.
	/// These attributes are supported by file systems such as the File Allocation
	/// Table (FAT) format commonly used in <em>consumer devices</em>.
	/// 
	/// <para> A {@code DosFileAttributeView} is a <seealso cref="BasicFileAttributeView"/> that
	/// additionally supports access to the set of DOS attribute flags that are used
	/// to indicate if the file is read-only, hidden, a system file, or archived.
	/// 
	/// </para>
	/// <para> Where dynamic access to file attributes is required, the attributes
	/// supported by this attribute view are as defined by {@code
	/// BasicFileAttributeView}, and in addition, the following attributes are
	/// supported:
	/// <blockquote>
	/// <table border="1" cellpadding="8" summary="Supported attributes">
	///   <tr>
	///     <th> Name </th>
	///     <th> Type </th>
	///   </tr>
	///   <tr>
	///     <td> readonly </td>
	///     <td> <seealso cref="Boolean"/> </td>
	///   </tr>
	///   <tr>
	///     <td> hidden </td>
	///     <td> <seealso cref="Boolean"/> </td>
	///   </tr>
	///   <tr>
	///     <td> system </td>
	///     <td> <seealso cref="Boolean"/> </td>
	///   </tr>
	///   <tr>
	///     <td> archive </td>
	///     <td> <seealso cref="Boolean"/> </td>
	///   </tr>
	/// </table>
	/// </blockquote>
	/// 
	/// </para>
	/// <para> The <seealso cref="java.nio.file.Files#getAttribute getAttribute"/> method may
	/// be used to read any of these attributes, or any of the attributes defined by
	/// <seealso cref="BasicFileAttributeView"/> as if by invoking the {@link #readAttributes
	/// readAttributes()} method.
	/// 
	/// </para>
	/// <para> The <seealso cref="java.nio.file.Files#setAttribute setAttribute"/> method may
	/// be used to update the file's last modified time, last access time or create
	/// time attributes as defined by <seealso cref="BasicFileAttributeView"/>. It may also be
	/// used to update the DOS attributes as if by invoking the {@link #setReadOnly
	/// setReadOnly}, <seealso cref="#setHidden setHidden"/>, <seealso cref="#setSystem setSystem"/>, and
	/// <seealso cref="#setArchive setArchive"/> methods respectively.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface DosFileAttributeView : BasicFileAttributeView
	{
		/// <summary>
		/// Returns the name of the attribute view. Attribute views of this type
		/// have the name {@code "dos"}.
		/// </summary>
		String Name();

		/// <exception cref="IOException">                             {@inheritDoc} </exception>
		/// <exception cref="SecurityException">                       {@inheritDoc} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override DosFileAttributes readAttributes() throws java.io.IOException;
		DosFileAttributes ReadAttributes();

		/// <summary>
		/// Updates the value of the read-only attribute.
		/// 
		/// <para> It is implementation specific if the attribute can be updated as an
		/// atomic operation with respect to other file system operations. An
		/// implementation may, for example, require to read the existing value of
		/// the DOS attribute in order to update this attribute.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">
		///          the new value of the attribute
		/// </param>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default, and a security manager is installed,
		///          its  <seealso cref="SecurityManager#checkWrite(String) checkWrite"/> method
		///          is invoked to check write access to the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setReadOnly(boolean value) throws java.io.IOException;
		bool ReadOnly {set;}

		/// <summary>
		/// Updates the value of the hidden attribute.
		/// 
		/// <para> It is implementation specific if the attribute can be updated as an
		/// atomic operation with respect to other file system operations. An
		/// implementation may, for example, require to read the existing value of
		/// the DOS attribute in order to update this attribute.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">
		///          the new value of the attribute
		/// </param>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default, and a security manager is installed,
		///          its  <seealso cref="SecurityManager#checkWrite(String) checkWrite"/> method
		///          is invoked to check write access to the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setHidden(boolean value) throws java.io.IOException;
		bool Hidden {set;}

		/// <summary>
		/// Updates the value of the system attribute.
		/// 
		/// <para> It is implementation specific if the attribute can be updated as an
		/// atomic operation with respect to other file system operations. An
		/// implementation may, for example, require to read the existing value of
		/// the DOS attribute in order to update this attribute.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">
		///          the new value of the attribute
		/// </param>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default, and a security manager is installed,
		///          its  <seealso cref="SecurityManager#checkWrite(String) checkWrite"/> method
		///          is invoked to check write access to the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setSystem(boolean value) throws java.io.IOException;
		bool System {set;}

		/// <summary>
		/// Updates the value of the archive attribute.
		/// 
		/// <para> It is implementation specific if the attribute can be updated as an
		/// atomic operation with respect to other file system operations. An
		/// implementation may, for example, require to read the existing value of
		/// the DOS attribute in order to update this attribute.
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">
		///          the new value of the attribute
		/// </param>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default, and a security manager is installed,
		///          its  <seealso cref="SecurityManager#checkWrite(String) checkWrite"/> method
		///          is invoked to check write access to the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setArchive(boolean value) throws java.io.IOException;
		bool Archive {set;}
	}

}