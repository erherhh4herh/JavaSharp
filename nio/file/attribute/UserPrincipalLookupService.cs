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

namespace java.nio.file.attribute
{

	/// <summary>
	/// An object to lookup user and group principals by name. A <seealso cref="UserPrincipal"/>
	/// represents an identity that may be used to determine access rights to objects
	/// in a file system. A <seealso cref="GroupPrincipal"/> represents a <em>group identity</em>.
	/// A {@code UserPrincipalLookupService} defines methods to lookup identities by
	/// name or group name (which are typically user or account names). Whether names
	/// and group names are case sensitive or not depends on the implementation.
	/// The exact definition of a group is implementation specific but typically a
	/// group represents an identity created for administrative purposes so as to
	/// determine the access rights for the members of the group. In particular it is
	/// implementation specific if the <em>namespace</em> for names and groups is the
	/// same or is distinct. To ensure consistent and correct behavior across
	/// platforms it is recommended that this API be used as if the namespaces are
	/// distinct. In other words, the {@link #lookupPrincipalByName
	/// lookupPrincipalByName} should be used to lookup users, and {@link
	/// #lookupPrincipalByGroupName lookupPrincipalByGroupName} should be used to
	/// lookup groups.
	/// 
	/// @since 1.7
	/// </summary>
	/// <seealso cref= java.nio.file.FileSystem#getUserPrincipalLookupService </seealso>

	public abstract class UserPrincipalLookupService
	{

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		protected internal UserPrincipalLookupService()
		{
		}

		/// <summary>
		/// Lookup a user principal by name.
		/// </summary>
		/// <param name="name">
		///          the string representation of the user principal to lookup
		/// </param>
		/// <returns>  a user principal
		/// </returns>
		/// <exception cref="UserPrincipalNotFoundException">
		///          the principal does not exist </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, it checks <seealso cref="RuntimePermission"/><tt>("lookupUserInformation")</tt> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract UserPrincipal lookupPrincipalByName(String name) throws java.io.IOException;
		public abstract UserPrincipal LookupPrincipalByName(String name);

		/// <summary>
		/// Lookup a group principal by group name.
		/// 
		/// <para> Where an implementation does not support any notion of group then
		/// this method always throws <seealso cref="UserPrincipalNotFoundException"/>. Where
		/// the namespace for user accounts and groups is the same, then this method
		/// is identical to invoking {@link #lookupPrincipalByName
		/// lookupPrincipalByName}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="group">
		///          the string representation of the group to lookup
		/// </param>
		/// <returns>  a group principal
		/// </returns>
		/// <exception cref="UserPrincipalNotFoundException">
		///          the principal does not exist or is not a group </exception>
		/// <exception cref="IOException">
		///          if an I/O error occurs </exception>
		/// <exception cref="SecurityException">
		///          In the case of the default provider, and a security manager is
		///          installed, it checks <seealso cref="RuntimePermission"/><tt>("lookupUserInformation")</tt> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract GroupPrincipal lookupPrincipalByGroupName(String group) throws java.io.IOException;
		public abstract GroupPrincipal LookupPrincipalByGroupName(String group);
	}

}