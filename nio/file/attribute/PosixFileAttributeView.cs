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
	/// A file attribute view that provides a view of the file attributes commonly
	/// associated with files on file systems used by operating systems that implement
	/// the Portable Operating System Interface (POSIX) family of standards.
	/// 
	/// <para> Operating systems that implement the <a href="http://www.opengroup.org">
	/// POSIX</a> family of standards commonly use file systems that have a
	/// file <em>owner</em>, <em>group-owner</em>, and related <em>access
	/// permissions</em>. This file attribute view provides read and write access
	/// to these attributes.
	/// 
	/// </para>
	/// <para> The <seealso cref="#readAttributes() readAttributes"/> method is used to read the
	/// file's attributes. The file <seealso cref="PosixFileAttributes#owner() owner"/> is
	/// represented by a <seealso cref="UserPrincipal"/> that is the identity of the file owner
	/// for the purposes of access control. The {@link PosixFileAttributes#group()
	/// group-owner}, represented by a <seealso cref="GroupPrincipal"/>, is the identity of the
	/// group owner, where a group is an identity created for administrative purposes
	/// so as to determine the access rights for the members of the group.
	/// 
	/// </para>
	/// <para> The <seealso cref="PosixFileAttributes#permissions() permissions"/> attribute is a
	/// set of access permissions. This file attribute view provides access to the nine
	/// permission defined by the <seealso cref="PosixFilePermission"/> class.
	/// These nine permission bits determine the <em>read</em>, <em>write</em>, and
	/// <em>execute</em> access for the file owner, group, and others (others
	/// meaning identities other than the owner and members of the group). Some
	/// operating systems and file systems may provide additional permission bits
	/// but access to these other bits is not defined by this class in this release.
	/// 
	/// </para>
	/// <para> <b>Usage Example:</b>
	/// Suppose we need to print out the owner and access permissions of a file:
	/// <pre>
	///     Path file = ...
	///     PosixFileAttributes attrs = Files.getFileAttributeView(file, PosixFileAttributeView.class)
	///         .readAttributes();
	///     System.out.format("%s %s%n",
	///         attrs.owner().getName(),
	///         PosixFilePermissions.toString(attrs.permissions()));
	/// </pre>
	/// 
	/// <h2> Dynamic Access </h2>
	/// </para>
	/// <para> Where dynamic access to file attributes is required, the attributes
	/// supported by this attribute view are as defined by {@link
	/// BasicFileAttributeView} and <seealso cref="FileOwnerAttributeView"/>, and in addition,
	/// the following attributes are supported:
	/// <blockquote>
	/// <table border="1" cellpadding="8" summary="Supported attributes">
	///   <tr>
	///     <th> Name </th>
	///     <th> Type </th>
	///   </tr>
	///  <tr>
	///     <td> "permissions" </td>
	///     <td> <seealso cref="Set"/>&lt;<seealso cref="PosixFilePermission"/>&gt; </td>
	///   </tr>
	///   <tr>
	///     <td> "group" </td>
	///     <td> <seealso cref="GroupPrincipal"/> </td>
	///   </tr>
	/// </table>
	/// </blockquote>
	/// 
	/// </para>
	/// <para> The <seealso cref="Files#getAttribute getAttribute"/> method may be used to read
	/// any of these attributes, or any of the attributes defined by {@link
	/// BasicFileAttributeView} as if by invoking the {@link #readAttributes
	/// readAttributes()} method.
	/// 
	/// </para>
	/// <para> The <seealso cref="Files#setAttribute setAttribute"/> method may be used to update
	/// the file's last modified time, last access time or create time attributes as
	/// defined by <seealso cref="BasicFileAttributeView"/>. It may also be used to update
	/// the permissions, owner, or group-owner as if by invoking the {@link
	/// #setPermissions setPermissions}, <seealso cref="#setOwner setOwner"/>, and {@link
	/// #setGroup setGroup} methods respectively.
	/// 
	/// <h2> Setting Initial Permissions </h2>
	/// </para>
	/// <para> Implementations supporting this attribute view may also support setting
	/// the initial permissions when creating a file or directory. The
	/// initial permissions are provided to the <seealso cref="Files#createFile createFile"/>
	/// or <seealso cref="Files#createDirectory createDirectory"/> methods as a {@link
	/// FileAttribute} with <seealso cref="FileAttribute#name name"/> {@code "posix:permissions"}
	/// and a <seealso cref="FileAttribute#value value"/> that is the set of permissions. The
	/// following example uses the {@link PosixFilePermissions#asFileAttribute
	/// asFileAttribute} method to construct a {@code FileAttribute} when creating a
	/// file:
	/// 
	/// <pre>
	///     Path path = ...
	///     Set&lt;PosixFilePermission&gt; perms =
	///         EnumSet.of(OWNER_READ, OWNER_WRITE, OWNER_EXECUTE, GROUP_READ);
	///     Files.createFile(path, PosixFilePermissions.asFileAttribute(perms));
	/// </pre>
	/// 
	/// </para>
	/// <para> When the access permissions are set at file creation time then the actual
	/// value of the permissions may differ that the value of the attribute object.
	/// The reasons for this are implementation specific. On UNIX systems, for
	/// example, a process has a <em>umask</em> that impacts the permission bits
	/// of newly created files. Where an implementation supports the setting of
	/// the access permissions, and the underlying file system supports access
	/// permissions, then it is required that the value of the actual access
	/// permissions will be equal or less than the value of the attribute
	/// provided to the <seealso cref="Files#createFile createFile"/> or {@link
	/// Files#createDirectory createDirectory} methods. In other words, the file may
	/// be more secure than requested.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface PosixFileAttributeView : BasicFileAttributeView, FileOwnerAttributeView
	{
		/// <summary>
		/// Returns the name of the attribute view. Attribute views of this type
		/// have the name {@code "posix"}.
		/// </summary>
		String Name();

		/// <exception cref="IOException">                {@inheritDoc} </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, and it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
		///          denies read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override PosixFileAttributes readAttributes() throws java.io.IOException;
		PosixFileAttributes ReadAttributes();

		/// <summary>
		/// Updates the file permissions.
		/// </summary>
		/// <param name="perms">
		///          the new set of permissions
		/// </param>
		/// <exception cref="ClassCastException">
		///          if the sets contains elements that are not of type {@code
		///          PosixFilePermission} </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, and it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
		///          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setPermissions(java.util.Set<PosixFilePermission> perms) throws java.io.IOException;
		Set<PosixFilePermission> Permissions {set;}

		/// <summary>
		/// Updates the file group-owner.
		/// </summary>
		/// <param name="group">
		///          the new file group-owner
		/// </param>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
		///          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setGroup(GroupPrincipal group) throws java.io.IOException;
		GroupPrincipal Group {set;}
	}

}