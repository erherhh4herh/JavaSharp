using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using JavaSecurityProtectionDomainAccess = sun.misc.JavaSecurityProtectionDomainAccess;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.misc.JavaSecurityProtectionDomainAccess.ProtectionDomainCache;
	using Debug = sun.security.util.Debug;
	using SecurityConstants = sun.security.util.SecurityConstants;
	using JavaSecurityAccess = sun.misc.JavaSecurityAccess;
	using SharedSecrets = sun.misc.SharedSecrets;

	/// 
	/// <summary>
	/// <para>
	/// This ProtectionDomain class encapsulates the characteristics of a domain,
	/// which encloses a set of classes whose instances are granted a set
	/// of permissions when being executed on behalf of a given set of Principals.
	/// </para>
	/// <para>
	/// A static set of permissions can be bound to a ProtectionDomain when it is
	/// constructed; such permissions are granted to the domain regardless of the
	/// Policy in force. However, to support dynamic security policies, a
	/// ProtectionDomain can also be constructed such that it is dynamically
	/// mapped to a set of permissions by the current Policy whenever a permission
	/// is checked.
	/// </para>
	/// <para>
	/// 
	/// @author Li Gong
	/// @author Roland Schemers
	/// @author Gary Ellison
	/// </para>
	/// </summary>

	public class ProtectionDomain
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			Key = new Key(this);
		}

		private class JavaSecurityAccessImpl : JavaSecurityAccess
		{

			internal JavaSecurityAccessImpl()
			{
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public <T> T doIntersectionPrivilege(PrivilegedAction<T> action, final AccessControlContext stack, final AccessControlContext context)
			public override T doIntersectionPrivilege<T>(PrivilegedAction<T> action, AccessControlContext stack, AccessControlContext context)
			{
				if (action == null)
				{
					throw new NullPointerException();
				}

				return AccessController.doPrivileged(action, GetCombinedACC(context, stack));
			}

			public override T doIntersectionPrivilege<T>(PrivilegedAction<T> action, AccessControlContext context)
			{
				return DoIntersectionPrivilege(action, AccessController.Context, context);
			}

			internal static AccessControlContext GetCombinedACC(AccessControlContext context, AccessControlContext stack)
			{
				AccessControlContext acc = new AccessControlContext(context, stack.Combiner, true);

				return (new AccessControlContext(stack.Context, acc)).Optimize();
			}
		}

		static ProtectionDomain()
		{
			// Set up JavaSecurityAccess in SharedSecrets
			SharedSecrets.JavaSecurityAccess = new JavaSecurityAccessImpl();
			SharedSecrets.JavaSecurityProtectionDomainAccess = new JavaSecurityProtectionDomainAccessAnonymousInnerClassHelper();
		}

		private class JavaSecurityProtectionDomainAccessAnonymousInnerClassHelper : JavaSecurityProtectionDomainAccess
		{
			public JavaSecurityProtectionDomainAccessAnonymousInnerClassHelper()
			{
			}

			public virtual ProtectionDomainCache ProtectionDomainCache
			{
				get
				{
					return new ProtectionDomainCacheAnonymousInnerClassHelper(this);
				}
			}

			private class ProtectionDomainCacheAnonymousInnerClassHelper : ProtectionDomainCache
			{
				private readonly JavaSecurityProtectionDomainAccessAnonymousInnerClassHelper OuterInstance;

				public ProtectionDomainCacheAnonymousInnerClassHelper(JavaSecurityProtectionDomainAccessAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
					map = Collections.SynchronizedMap(new WeakHashMap<Key, PermissionCollection>());
				}

				private readonly IDictionary<Key, PermissionCollection> map;
				public virtual void Put(ProtectionDomain pd, PermissionCollection pc)
				{
					map.put((pd == null ? null : pd.Key), pc);
				}
				public virtual PermissionCollection Get(ProtectionDomain pd)
				{
					return pd == null ? map.get(null) : map.get(pd.Key);
				}
			}
		}

		/* CodeSource */
		private CodeSource Codesource;

		/* ClassLoader the protection domain was consed from */
		private ClassLoader Classloader;

		/* Principals running-as within this protection domain */
		private Principal[] Principals_Renamed;

		/* the rights this protection domain is granted */
		private PermissionCollection Permissions_Renamed;

		/* if the permissions object has AllPermission */
		private bool HasAllPerm = false;

		/* the PermissionCollection is static (pre 1.4 constructor)
		   or dynamic (via a policy refresh) */
		private bool StaticPermissions;

		/*
		 * An object used as a key when the ProtectionDomain is stored in a Map.
		 */
		internal Key Key;

		private static readonly Debug Debug = Debug.getInstance("domain");

		/// <summary>
		/// Creates a new ProtectionDomain with the given CodeSource and
		/// Permissions. If the permissions object is not null, then
		///  {@code setReadOnly())} will be called on the passed in
		/// Permissions object. The only permissions granted to this domain
		/// are the ones specified; the current Policy will not be consulted.
		/// </summary>
		/// <param name="codesource"> the codesource associated with this domain </param>
		/// <param name="permissions"> the permissions granted to this domain </param>
		public ProtectionDomain(CodeSource codesource, PermissionCollection permissions)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.Codesource = codesource;
			if (permissions != null)
			{
				this.Permissions_Renamed = permissions;
				this.Permissions_Renamed.SetReadOnly();
				if (permissions is Permissions && ((Permissions)permissions).AllPermission != null)
				{
					HasAllPerm = true;
				}
			}
			this.Classloader = null;
			this.Principals_Renamed = new Principal[0];
			StaticPermissions = true;
		}

		/// <summary>
		/// Creates a new ProtectionDomain qualified by the given CodeSource,
		/// Permissions, ClassLoader and array of Principals. If the
		/// permissions object is not null, then {@code setReadOnly()}
		/// will be called on the passed in Permissions object.
		/// The permissions granted to this domain are dynamic; they include
		/// both the static permissions passed to this constructor, and any
		/// permissions granted to this domain by the current Policy at the
		/// time a permission is checked.
		/// <para>
		/// This constructor is typically used by
		/// <seealso cref="SecureClassLoader ClassLoaders"/>
		/// and <seealso cref="DomainCombiner DomainCombiners"/> which delegate to
		/// {@code Policy} to actively associate the permissions granted to
		/// this domain. This constructor affords the
		/// Policy provider the opportunity to augment the supplied
		/// PermissionCollection to reflect policy changes.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codesource"> the CodeSource associated with this domain </param>
		/// <param name="permissions"> the permissions granted to this domain </param>
		/// <param name="classloader"> the ClassLoader associated with this domain </param>
		/// <param name="principals"> the array of Principals associated with this
		/// domain. The contents of the array are copied to protect against
		/// subsequent modification. </param>
		/// <seealso cref= Policy#refresh </seealso>
		/// <seealso cref= Policy#getPermissions(ProtectionDomain)
		/// @since 1.4 </seealso>
		public ProtectionDomain(CodeSource codesource, PermissionCollection permissions, ClassLoader classloader, Principal[] principals)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.Codesource = codesource;
			if (permissions != null)
			{
				this.Permissions_Renamed = permissions;
				this.Permissions_Renamed.SetReadOnly();
				if (permissions is Permissions && ((Permissions)permissions).AllPermission != null)
				{
					HasAllPerm = true;
				}
			}
			this.Classloader = classloader;
			this.Principals_Renamed = (principals != null ? principals.clone(): new Principal[0]);
			StaticPermissions = false;
		}

		/// <summary>
		/// Returns the CodeSource of this domain. </summary>
		/// <returns> the CodeSource of this domain which may be null.
		/// @since 1.2 </returns>
		public CodeSource CodeSource
		{
			get
			{
				return this.Codesource;
			}
		}


		/// <summary>
		/// Returns the ClassLoader of this domain. </summary>
		/// <returns> the ClassLoader of this domain which may be null.
		/// 
		/// @since 1.4 </returns>
		public ClassLoader ClassLoader
		{
			get
			{
				return this.Classloader;
			}
		}


		/// <summary>
		/// Returns an array of principals for this domain. </summary>
		/// <returns> a non-null array of principals for this domain.
		/// Returns a new array each time this method is called.
		/// 
		/// @since 1.4 </returns>
		public Principal[] Principals
		{
			get
			{
				return this.Principals_Renamed.clone();
			}
		}

		/// <summary>
		/// Returns the static permissions granted to this domain.
		/// </summary>
		/// <returns> the static set of permissions for this domain which may be null. </returns>
		/// <seealso cref= Policy#refresh </seealso>
		/// <seealso cref= Policy#getPermissions(ProtectionDomain) </seealso>
		public PermissionCollection Permissions
		{
			get
			{
				return Permissions_Renamed;
			}
		}

		/// <summary>
		/// Check and see if this ProtectionDomain implies the permissions
		/// expressed in the Permission object.
		/// <para>
		/// The set of permissions evaluated is a function of whether the
		/// ProtectionDomain was constructed with a static set of permissions
		/// or it was bound to a dynamically mapped set of permissions.
		/// </para>
		/// <para>
		/// If the ProtectionDomain was constructed to a
		/// {@link #ProtectionDomain(CodeSource, PermissionCollection)
		/// statically bound} PermissionCollection then the permission will
		/// only be checked against the PermissionCollection supplied at
		/// construction.
		/// </para>
		/// <para>
		/// However, if the ProtectionDomain was constructed with
		/// the constructor variant which supports
		/// {@link #ProtectionDomain(CodeSource, PermissionCollection,
		/// ClassLoader, java.security.Principal[]) dynamically binding}
		/// permissions, then the permission will be checked against the
		/// combination of the PermissionCollection supplied at construction and
		/// the current Policy binding.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="permission"> the Permission object to check.
		/// </param>
		/// <returns> true if "permission" is implicit to this ProtectionDomain. </returns>
		public virtual bool Implies(Permission permission)
		{

			if (HasAllPerm)
			{
				// internal permission collection already has AllPermission -
				// no need to go to policy
				return true;
			}

			if (!StaticPermissions && Policy.PolicyNoCheck.Implies(this, permission))
			{
				return true;
			}
			if (Permissions_Renamed != null)
			{
				return Permissions_Renamed.Implies(permission);
			}

			return false;
		}

		// called by the VM -- do not remove
		internal virtual bool ImpliesCreateAccessControlContext()
		{
			return Implies(SecurityConstants.CREATE_ACC_PERMISSION);
		}

		/// <summary>
		/// Convert a ProtectionDomain to a String.
		/// </summary>
		public override String ToString()
		{
			String pals = "<no principals>";
			if (Principals_Renamed != null && Principals_Renamed.Length > 0)
			{
				StringBuilder palBuf = new StringBuilder("(principals ");

				for (int i = 0; i < Principals_Renamed.Length; i++)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					palBuf.Append(Principals_Renamed[i].GetType().FullName + " \"" + Principals_Renamed[i].Name + "\"");
					if (i < Principals_Renamed.Length - 1)
					{
						palBuf.Append(",\n");
					}
					else
					{
						palBuf.Append(")\n");
					}
				}
				pals = palBuf.ToString();
			}

			// Check if policy is set; we don't want to load
			// the policy prematurely here
			PermissionCollection pc = Policy.Set && SeeAllp() ? MergePermissions(): Permissions;

			return "ProtectionDomain " + " " + Codesource + "\n" + " " + Classloader + "\n" + " " + pals + "\n" + " " + pc + "\n";
		}

		/// <summary>
		/// Return true (merge policy permissions) in the following cases:
		/// 
		/// . SecurityManager is null
		/// 
		/// . SecurityManager is not null,
		///          debug is not null,
		///          SecurityManager impelmentation is in bootclasspath,
		///          Policy implementation is in bootclasspath
		///          (the bootclasspath restrictions avoid recursion)
		/// 
		/// . SecurityManager is not null,
		///          debug is null,
		///          caller has Policy.getPolicy permission
		/// </summary>
		private static bool SeeAllp()
		{
			SecurityManager sm = System.SecurityManager;

			if (sm == null)
			{
				return true;
			}
			else
			{
				if (Debug != null)
				{
					if (sm.GetType().ClassLoader == null && Policy.PolicyNoCheck.GetType().ClassLoader == null)
					{
						return true;
					}
				}
				else
				{
					try
					{
						sm.CheckPermission(SecurityConstants.GET_POLICY_PERMISSION);
						return true;
					}
					catch (SecurityException)
					{
						// fall thru and return false
					}
				}
			}

			return false;
		}

		private PermissionCollection MergePermissions()
		{
			if (StaticPermissions)
			{
				return Permissions_Renamed;
			}

			PermissionCollection perms = java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this));

			Permissions mergedPerms = new Permissions();
			int swag = 32;
			int vcap = 8;
			IEnumerator<Permission> e;
			IList<Permission> pdVector = new List<Permission>(vcap);
			IList<Permission> plVector = new List<Permission>(swag);

			//
			// Build a vector of domain permissions for subsequent merge
			if (Permissions_Renamed != null)
			{
				lock (Permissions_Renamed)
				{
					e = Permissions_Renamed.Elements();
					while (e.MoveNext())
					{
						pdVector.Add(e.Current);
					}
				}
			}

			//
			// Build a vector of Policy permissions for subsequent merge
			if (perms != null)
			{
				lock (perms)
				{
					e = perms.Elements();
					while (e.MoveNext())
					{
						plVector.Add(e.Current);
						vcap++;
					}
				}
			}

			if (perms != null && Permissions_Renamed != null)
			{
				//
				// Weed out the duplicates from the policy. Unless a refresh
				// has occurred since the pd was consed this should result in
				// an empty vector.
				lock (Permissions_Renamed)
				{
					e = Permissions_Renamed.Elements(); // domain vs policy
					while (e.MoveNext())
					{
						Permission pdp = e.Current;
						Class pdpClass = pdp.GetType();
						String pdpActions = pdp.Actions;
						String pdpName = pdp.Name;
						for (int i = 0; i < plVector.Count; i++)
						{
							Permission pp = plVector[i];
							if (pdpClass.isInstance(pp))
							{
								// The equals() method on some permissions
								// have some side effects so this manual
								// comparison is sufficient.
								if (pdpName.Equals(pp.Name) && pdpActions.Equals(pp.Actions))
								{
									plVector.RemoveAt(i);
									break;
								}
							}
						}
					}
				}
			}

			if (perms != null)
			{
				// the order of adding to merged perms and permissions
				// needs to preserve the bugfix 4301064

				for (int i = plVector.Count - 1; i >= 0; i--)
				{
					mergedPerms.Add(plVector[i]);
				}
			}
			if (Permissions_Renamed != null)
			{
				for (int i = pdVector.Count - 1; i >= 0; i--)
				{
					mergedPerms.Add(pdVector[i]);
				}
			}

			return mergedPerms;
		}

		private class PrivilegedActionAnonymousInnerClassHelper : java.security.PrivilegedAction<PermissionCollection>
		{
			private readonly ProtectionDomain OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(ProtectionDomain outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual PermissionCollection Run()
			{
				Policy p = Policy.PolicyNoCheck;
				return p.GetPermissions(OuterInstance);
			}
		}

		/// <summary>
		/// Used for storing ProtectionDomains as keys in a Map.
		/// </summary>
		[Serializable]
		internal sealed class Key
		{
			private readonly ProtectionDomain OuterInstance;

			public Key(ProtectionDomain outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

		}

	}

}