/*
 * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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


namespace java.security
{

	/// <summary>
	/// This class defines the <i>Service Provider Interface</i> (<b>SPI</b>)
	/// for the {@code Policy} class.
	/// All the abstract methods in this class must be implemented by each
	/// service provider who wishes to supply a Policy implementation.
	/// 
	/// <para> Subclass implementations of this abstract class must provide
	/// a public constructor that takes a {@code Policy.Parameters}
	/// object as an input parameter.  This constructor also must throw
	/// an IllegalArgumentException if it does not understand the
	/// {@code Policy.Parameters} input.
	/// 
	/// 
	/// @since 1.6
	/// </para>
	/// </summary>

	public abstract class PolicySpi
	{

		/// <summary>
		/// Check whether the policy has granted a Permission to a ProtectionDomain.
		/// </summary>
		/// <param name="domain"> the ProtectionDomain to check.
		/// </param>
		/// <param name="permission"> check whether this permission is granted to the
		///          specified domain.
		/// </param>
		/// <returns> boolean true if the permission is granted to the domain. </returns>
		protected internal abstract bool EngineImplies(ProtectionDomain domain, Permission permission);

		/// <summary>
		/// Refreshes/reloads the policy configuration. The behavior of this method
		/// depends on the implementation. For example, calling {@code refresh}
		/// on a file-based policy will cause the file to be re-read.
		/// 
		/// <para> The default implementation of this method does nothing.
		/// This method should be overridden if a refresh operation is supported
		/// by the policy implementation.
		/// </para>
		/// </summary>
		protected internal virtual void EngineRefresh()
		{
		}

		/// <summary>
		/// Return a PermissionCollection object containing the set of
		/// permissions granted to the specified CodeSource.
		/// 
		/// <para> The default implementation of this method returns
		/// Policy.UNSUPPORTED_EMPTY_COLLECTION object.  This method can be
		/// overridden if the policy implementation can return a set of
		/// permissions granted to a CodeSource.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codesource"> the CodeSource to which the returned
		///          PermissionCollection has been granted.
		/// </param>
		/// <returns> a set of permissions granted to the specified CodeSource.
		///          If this operation is supported, the returned
		///          set of permissions must be a new mutable instance
		///          and it must support heterogeneous Permission types.
		///          If this operation is not supported,
		///          Policy.UNSUPPORTED_EMPTY_COLLECTION is returned. </returns>
		protected internal virtual PermissionCollection EngineGetPermissions(CodeSource codesource)
		{
			return Policy.UNSUPPORTED_EMPTY_COLLECTION;
		}

		/// <summary>
		/// Return a PermissionCollection object containing the set of
		/// permissions granted to the specified ProtectionDomain.
		/// 
		/// <para> The default implementation of this method returns
		/// Policy.UNSUPPORTED_EMPTY_COLLECTION object.  This method can be
		/// overridden if the policy implementation can return a set of
		/// permissions granted to a ProtectionDomain.
		/// 
		/// </para>
		/// </summary>
		/// <param name="domain"> the ProtectionDomain to which the returned
		///          PermissionCollection has been granted.
		/// </param>
		/// <returns> a set of permissions granted to the specified ProtectionDomain.
		///          If this operation is supported, the returned
		///          set of permissions must be a new mutable instance
		///          and it must support heterogeneous Permission types.
		///          If this operation is not supported,
		///          Policy.UNSUPPORTED_EMPTY_COLLECTION is returned. </returns>
		protected internal virtual PermissionCollection EngineGetPermissions(ProtectionDomain domain)
		{
			return Policy.UNSUPPORTED_EMPTY_COLLECTION;
		}
	}

}