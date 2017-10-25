using System.Collections.Generic;

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
	/// A file attribute view that supports reading or updating a file's Access
	/// Control Lists (ACL) or file owner attributes.
	/// 
	/// <para> ACLs are used to specify access rights to file system objects. An ACL is
	/// an ordered list of <seealso cref="AclEntry access-control-entries"/>, each specifying a
	/// <seealso cref="UserPrincipal"/> and the level of access for that user principal. This
	/// file attribute view defines the <seealso cref="#getAcl() getAcl"/>, and {@link
	/// #setAcl(List) setAcl} methods to read and write ACLs based on the ACL
	/// model specified in <a href="http://www.ietf.org/rfc/rfc3530.txt"><i>RFC&nbsp;3530:
	/// Network File System (NFS) version 4 Protocol</i></a>. This file attribute view
	/// is intended for file system implementations that support the NFSv4 ACL model
	/// or have a <em>well-defined</em> mapping between the NFSv4 ACL model and the ACL
	/// model used by the file system. The details of such mapping are implementation
	/// dependent and are therefore unspecified.
	/// 
	/// </para>
	/// <para> This class also extends {@code FileOwnerAttributeView} so as to define
	/// methods to get and set the file owner.
	/// 
	/// </para>
	/// <para> When a file system provides access to a set of {@link FileStore
	/// file-systems} that are not homogeneous then only some of the file systems may
	/// support ACLs. The {@link FileStore#supportsFileAttributeView
	/// supportsFileAttributeView} method can be used to test if a file system
	/// supports ACLs.
	/// 
	/// <h2>Interoperability</h2>
	/// 
	/// RFC&nbsp;3530 allows for special user identities to be used on platforms that
	/// support the POSIX defined access permissions. The special user identities
	/// are "{@code OWNER@}", "{@code GROUP@}", and "{@code EVERYONE@}". When both
	/// the {@code AclFileAttributeView} and the <seealso cref="PosixFileAttributeView"/>
	/// are supported then these special user identities may be included in ACL {@link
	/// AclEntry entries} that are read or written. The file system's {@link
	/// UserPrincipalLookupService} may be used to obtain a <seealso cref="UserPrincipal"/>
	/// to represent these special identities by invoking the {@link
	/// UserPrincipalLookupService#lookupPrincipalByName lookupPrincipalByName}
	/// method.
	/// 
	/// </para>
	/// <para> <b>Usage Example:</b>
	/// Suppose we wish to add an entry to an existing ACL to grant "joe" access:
	/// <pre>
	///     // lookup "joe"
	///     UserPrincipal joe = file.getFileSystem().getUserPrincipalLookupService()
	///         .lookupPrincipalByName("joe");
	/// 
	///     // get view
	///     AclFileAttributeView view = Files.getFileAttributeView(file, AclFileAttributeView.class);
	/// 
	///     // create ACE to give "joe" read access
	///     AclEntry entry = AclEntry.newBuilder()
	///         .setType(AclEntryType.ALLOW)
	///         .setPrincipal(joe)
	///         .setPermissions(AclEntryPermission.READ_DATA, AclEntryPermission.READ_ATTRIBUTES)
	///         .build();
	/// 
	///     // read ACL, insert ACE, re-write ACL
	///     List&lt;AclEntry&gt; acl = view.getAcl();
	///     acl.add(0, entry);   // insert before any DENY entries
	///     view.setAcl(acl);
	/// </pre>
	/// 
	/// <h2> Dynamic Access </h2>
	/// </para>
	/// <para> Where dynamic access to file attributes is required, the attributes
	/// supported by this attribute view are as follows:
	/// <blockquote>
	/// <table border="1" cellpadding="8" summary="Supported attributes">
	///   <tr>
	///     <th> Name </th>
	///     <th> Type </th>
	///   </tr>
	///   <tr>
	///     <td> "acl" </td>
	///     <td> <seealso cref="List"/>&lt;<seealso cref="AclEntry"/>&gt; </td>
	///   </tr>
	///   <tr>
	///     <td> "owner" </td>
	///     <td> <seealso cref="UserPrincipal"/> </td>
	///   </tr>
	/// </table>
	/// </blockquote>
	/// 
	/// </para>
	/// <para> The <seealso cref="Files#getAttribute getAttribute"/> method may be used to read
	/// the ACL or owner attributes as if by invoking the <seealso cref="#getAcl getAcl"/> or
	/// <seealso cref="#getOwner getOwner"/> methods.
	/// 
	/// </para>
	/// <para> The <seealso cref="Files#setAttribute setAttribute"/> method may be used to
	/// update the ACL or owner attributes as if by invoking the <seealso cref="#setAcl setAcl"/>
	/// or <seealso cref="#setOwner setOwner"/> methods.
	/// 
	/// <h2> Setting the ACL when creating a file </h2>
	/// 
	/// </para>
	/// <para> Implementations supporting this attribute view may also support setting
	/// the initial ACL when creating a file or directory. The initial ACL
	/// may be provided to methods such as <seealso cref="Files#createFile createFile"/> or {@link
	/// Files#createDirectory createDirectory} as an <seealso cref="FileAttribute"/> with {@link
	/// FileAttribute#name name} {@code "acl:acl"} and a {@link FileAttribute#value
	/// value} that is the list of {@code AclEntry} objects.
	/// 
	/// </para>
	/// <para> Where an implementation supports an ACL model that differs from the NFSv4
	/// defined ACL model then setting the initial ACL when creating the file must
	/// translate the ACL to the model supported by the file system. Methods that
	/// create a file should reject (by throwing <seealso cref="IOException IOException"/>)
	/// any attempt to create a file that would be less secure as a result of the
	/// translation.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>

	public interface AclFileAttributeView : FileOwnerAttributeView
	{
		/// <summary>
		/// Returns the name of the attribute view. Attribute views of this type
		/// have the name {@code "acl"}.
		/// </summary>
		String Name();

		/// <summary>
		/// Reads the access control list.
		/// 
		/// <para> When the file system uses an ACL model that differs from the NFSv4
		/// defined ACL model, then this method returns an ACL that is the translation
		/// of the ACL to the NFSv4 ACL model.
		/// 
		/// </para>
		/// <para> The returned list is modifiable so as to facilitate changes to the
		/// existing ACL. The <seealso cref="#setAcl setAcl"/> method is used to update
		/// the file's ACL attribute.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  an ordered list of <seealso cref="AclEntry entries"/> representing the
		///          ACL
		/// </returns>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, and it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
		///          or its <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
		///          denies read access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.util.List<AclEntry> getAcl() throws java.io.IOException;
		IList<AclEntry> Acl {get;set;}

		/// <summary>
		/// Updates (replace) the access control list.
		/// 
		/// <para> Where the file system supports Access Control Lists, and it uses an
		/// ACL model that differs from the NFSv4 defined ACL model, then this method
		/// must translate the ACL to the model supported by the file system. This
		/// method should reject (by throwing <seealso cref="IOException IOException"/>) any
		/// attempt to write an ACL that would appear to make the file more secure
		/// than would be the case if the ACL were updated. Where an implementation
		/// does not support a mapping of <seealso cref="AclEntryType#AUDIT"/> or {@link
		/// AclEntryType#ALARM} entries, then this method ignores these entries when
		/// writing the ACL.
		/// 
		/// </para>
		/// <para> If an ACL entry contains a <seealso cref="AclEntry#principal user-principal"/>
		/// that is not associated with the same provider as this attribute view then
		/// <seealso cref="ProviderMismatchException"/> is thrown. Additional validation, if
		/// any, is implementation dependent.
		/// 
		/// </para>
		/// <para> If the file system supports other security related file attributes
		/// (such as a file {@link PosixFileAttributes#permissions
		/// access-permissions} for example), the updating the access control list
		/// may also cause these security related attributes to be updated.
		/// 
		/// </para>
		/// </summary>
		/// <param name="acl">
		///          the new access control list
		/// </param>
		/// <exception cref="IOException">
		///          if an I/O error occurs or the ACL is invalid </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, a security manager is
		///          installed, it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
		///          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		///          method denies write access to the file. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setAcl(java.util.List<AclEntry> acl) throws java.io.IOException;
	}

}