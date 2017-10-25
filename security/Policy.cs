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

	using GetInstance = sun.security.jca.GetInstance;
	using Debug = sun.security.util.Debug;
	using SecurityConstants = sun.security.util.SecurityConstants;


	/// <summary>
	/// A Policy object is responsible for determining whether code executing
	/// in the Java runtime environment has permission to perform a
	/// security-sensitive operation.
	/// 
	/// <para> There is only one Policy object installed in the runtime at any
	/// given time.  A Policy object can be installed by calling the
	/// {@code setPolicy} method.  The installed Policy object can be
	/// obtained by calling the {@code getPolicy} method.
	/// 
	/// </para>
	/// <para> If no Policy object has been installed in the runtime, a call to
	/// {@code getPolicy} installs an instance of the default Policy
	/// implementation (a default subclass implementation of this abstract class).
	/// The default Policy implementation can be changed by setting the value
	/// of the {@code policy.provider} security property to the fully qualified
	/// name of the desired Policy subclass implementation.
	/// 
	/// </para>
	/// <para> Application code can directly subclass Policy to provide a custom
	/// implementation.  In addition, an instance of a Policy object can be
	/// constructed by invoking one of the {@code getInstance} factory methods
	/// with a standard type.  The default policy type is "JavaPolicy".
	/// 
	/// </para>
	/// <para> Once a Policy instance has been installed (either by default, or by
	/// calling {@code setPolicy}), the Java runtime invokes its
	/// {@code implies} method when it needs to
	/// determine whether executing code (encapsulated in a ProtectionDomain)
	/// can perform SecurityManager-protected operations.  How a Policy object
	/// retrieves its policy data is up to the Policy implementation itself.
	/// The policy data may be stored, for example, in a flat ASCII file,
	/// in a serialized binary file of the Policy class, or in a database.
	/// 
	/// </para>
	/// <para> The {@code refresh} method causes the policy object to
	/// refresh/reload its data.  This operation is implementation-dependent.
	/// For example, if the policy object stores its data in configuration files,
	/// calling {@code refresh} will cause it to re-read the configuration
	/// policy files.  If a refresh operation is not supported, this method does
	/// nothing.  Note that refreshed policy may not have an effect on classes
	/// in a particular ProtectionDomain. This is dependent on the Policy
	/// provider's implementation of the {@code implies}
	/// method and its PermissionCollection caching strategy.
	/// 
	/// @author Roland Schemers
	/// @author Gary Ellison
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.Provider </seealso>
	/// <seealso cref= java.security.ProtectionDomain </seealso>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.Security security properties </seealso>

	public abstract class Policy
	{

		/// <summary>
		/// A read-only empty PermissionCollection instance.
		/// @since 1.6
		/// </summary>
		public static readonly PermissionCollection UNSUPPORTED_EMPTY_COLLECTION = new UnsupportedEmptyCollection();

		// Information about the system-wide policy.
		private class PolicyInfo
		{
			// the system-wide policy
			internal readonly Policy Policy;
			// a flag indicating if the system-wide policy has been initialized
			internal readonly bool Initialized;

			internal PolicyInfo(Policy policy, bool initialized)
			{
				this.Policy = policy;
				this.Initialized = initialized;
			}
		}

		// PolicyInfo is stored in an AtomicReference
		private static AtomicReference<PolicyInfo> Policy_Renamed = new AtomicReference<PolicyInfo>(new PolicyInfo(null, false));

		private static readonly Debug Debug = Debug.getInstance("policy");

		// Cache mapping ProtectionDomain.Key to PermissionCollection
		private WeakHashMap<ProtectionDomain.Key, PermissionCollection> PdMapping;

		/// <summary>
		/// package private for AccessControlContext and ProtectionDomain </summary>
		internal static bool Set
		{
			get
			{
				PolicyInfo pi = Policy_Renamed.Get();
				return pi.Policy != null && pi.Initialized == true;
			}
		}

		private static void CheckPermission(String type)
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(new SecurityPermission("createPolicy." + type));
			}
		}

		/// <summary>
		/// Returns the installed Policy object. This value should not be cached,
		/// as it may be changed by a call to {@code setPolicy}.
		/// This method first calls
		/// {@code SecurityManager.checkPermission} with a
		/// {@code SecurityPermission("getPolicy")} permission
		/// to ensure it's ok to get the Policy object.
		/// </summary>
		/// <returns> the installed Policy.
		/// </returns>
		/// <exception cref="SecurityException">
		///        if a security manager exists and its
		///        {@code checkPermission} method doesn't allow
		///        getting the Policy object.
		/// </exception>
		/// <seealso cref= SecurityManager#checkPermission(Permission) </seealso>
		/// <seealso cref= #setPolicy(java.security.Policy) </seealso>
		public static Policy Policy
		{
			get
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(SecurityConstants.GET_POLICY_PERMISSION);
				}
				return PolicyNoCheck;
			}
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(new SecurityPermission("setPolicy"));
				}
				if (value != null)
				{
					InitPolicy(value);
				}
				lock (typeof(Policy))
				{
					Policy_Renamed.Set(new PolicyInfo(value, value != null));
				}
			}
		}

		/// <summary>
		/// Returns the installed Policy object, skipping the security check.
		/// Used by ProtectionDomain and getPolicy.
		/// </summary>
		/// <returns> the installed Policy. </returns>
		internal static Policy PolicyNoCheck
		{
			get
			{
				PolicyInfo pi = Policy_Renamed.Get();
				// Use double-check idiom to avoid locking if system-wide policy is
				// already initialized
				if (pi.Initialized == false || pi.Policy == null)
				{
					lock (typeof(Policy))
					{
						PolicyInfo pinfo = Policy_Renamed.Get();
						if (pinfo.Policy == null)
						{
							String policy_class = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
							if (policy_class == null)
							{
								policy_class = "sun.security.provider.PolicyFile";
							}
    
							try
							{
								pinfo = new PolicyInfo((Policy) Class.ForName(policy_class).NewInstance(), true);
							}
							catch (Exception e)
							{
								/*
								 * The policy_class seems to be an extension
								 * so we have to bootstrap loading it via a policy
								 * provider that is on the bootclasspath.
								 * If it loads then shift gears to using the configured
								 * provider.
								 */
    
								// install the bootstrap provider to avoid recursion
								Policy polFile = new sun.security.provider.PolicyFile();
								pinfo = new PolicyInfo(polFile, false);
								Policy_Renamed.Set(pinfo);
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String pc = policy_class;
								String pc = policy_class;
								Policy pol = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(e, pc));
								/*
								 * if it loaded install it as the policy provider. Otherwise
								 * continue to use the system default implementation
								 */
								if (pol != null)
								{
									pinfo = new PolicyInfo(pol, true);
								}
								else
								{
									if (Debug != null)
									{
										Debug.println("using sun.security.provider.PolicyFile");
									}
									pinfo = new PolicyInfo(polFile, true);
								}
							}
							Policy_Renamed.Set(pinfo);
						}
						return pinfo.Policy;
					}
				}
				return pi.Policy;
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual String Run()
			{
				return Security.GetProperty("policy.provider");
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Policy>
		{
			private Exception e;
			private string Pc;

			public PrivilegedActionAnonymousInnerClassHelper2(Exception e, string pc)
			{
				this.e = e;
				this.Pc = pc;
			}

			public virtual Policy Run()
			{
				try
				{
					ClassLoader cl = ClassLoader.SystemClassLoader;
					// we want the extension loader
					ClassLoader extcl = null;
					while (cl != null)
					{
						extcl = cl;
						cl = cl.Parent;
					}
					return (extcl != null ? (Policy)Class.ForName(Pc, true, extcl).NewInstance() : null);
				}
				catch (Exception e)
				{
					if (Debug != null)
					{
						Debug.println("policy provider " + Pc + " not available");
						e.PrintStackTrace();
					}
					return null;
				}
			}
		}


		/// <summary>
		/// Initialize superclass state such that a legacy provider can
		/// handle queries for itself.
		/// 
		/// @since 1.4
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void initPolicy(final Policy p)
		private static void InitPolicy(Policy p)
		{
			/*
			 * A policy provider not on the bootclasspath could trigger
			 * security checks fulfilling a call to either Policy.implies
			 * or Policy.getPermissions. If this does occur the provider
			 * must be able to answer for it's own ProtectionDomain
			 * without triggering additional security checks, otherwise
			 * the policy implementation will end up in an infinite
			 * recursion.
			 *
			 * To mitigate this, the provider can collect it's own
			 * ProtectionDomain and associate a PermissionCollection while
			 * it is being installed. The currently installed policy
			 * provider (if there is one) will handle calls to
			 * Policy.implies or Policy.getPermissions during this
			 * process.
			 *
			 * This Policy superclass caches away the ProtectionDomain and
			 * statically binds permissions so that legacy Policy
			 * implementations will continue to function.
			 */

			ProtectionDomain policyDomain = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper3(p));

			/*
			 * Collect the permissions granted to this protection domain
			 * so that the provider can be security checked while processing
			 * calls to Policy.implies or Policy.getPermissions.
			 */
			PermissionCollection policyPerms = null;
			lock (p)
			{
				if (p.PdMapping == null)
				{
					p.PdMapping = new WeakHashMap<>();
				}
			}

			if (policyDomain.CodeSource != null)
			{
				Policy pol = Policy_Renamed.Get().policy;
				if (pol != null)
				{
					policyPerms = pol.GetPermissions(policyDomain);
				}

				if (policyPerms == null) // assume it has all
				{
					policyPerms = new Permissions();
					policyPerms.Add(SecurityConstants.ALL_PERMISSION);
				}

				lock (p.PdMapping)
				{
					// cache of pd to permissions
					p.PdMapping.Put(policyDomain.Key, policyPerms);
				}
			}
			return;
		}

		private class PrivilegedActionAnonymousInnerClassHelper3 : PrivilegedAction<ProtectionDomain>
		{
			private java.security.Policy p;

			public PrivilegedActionAnonymousInnerClassHelper3(java.security.Policy p)
			{
				this.p = p;
			}

			public virtual ProtectionDomain Run()
			{
				return p.GetType().ProtectionDomain;
			}
		}


		/// <summary>
		/// Returns a Policy object of the specified type.
		/// 
		/// <para> This method traverses the list of registered security providers,
		/// starting with the most preferred Provider.
		/// A new Policy object encapsulating the
		/// PolicySpi implementation from the first
		/// Provider that supports the specified type is returned.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the specified Policy type.  See the Policy section in the
		///    <a href=
		///    "{@docRoot}/../technotes/guides/security/StandardNames.html#Policy">
		///    Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		///    for a list of standard Policy types.
		/// </param>
		/// <param name="params"> parameters for the Policy, which may be null.
		/// </param>
		/// <returns> the new Policy object.
		/// </returns>
		/// <exception cref="SecurityException"> if the caller does not have permission
		///          to get a Policy instance for the specified type.
		/// </exception>
		/// <exception cref="NullPointerException"> if the specified type is null.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the specified parameters
		///          are not understood by the PolicySpi implementation
		///          from the selected Provider.
		/// </exception>
		/// <exception cref="NoSuchAlgorithmException"> if no Provider supports a PolicySpi
		///          implementation for the specified type.
		/// </exception>
		/// <seealso cref= Provider
		/// @since 1.6 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Policy getInstance(String type, Policy.Parameters params) throws NoSuchAlgorithmException
		public static Policy GetInstance(String type, Policy.Parameters @params)
		{

			CheckPermission(type);
			try
			{
				GetInstance.Instance instance = GetInstance.getInstance("Policy", typeof(PolicySpi), type, @params);
				return new PolicyDelegate((PolicySpi)instance.impl, instance.provider, type, @params);
			}
			catch (NoSuchAlgorithmException nsae)
			{
				return HandleException(nsae);
			}
		}

		/// <summary>
		/// Returns a Policy object of the specified type.
		/// 
		/// <para> A new Policy object encapsulating the
		/// PolicySpi implementation from the specified provider
		/// is returned.   The specified provider must be registered
		/// in the provider list.
		/// 
		/// </para>
		/// <para> Note that the list of registered providers may be retrieved via
		/// the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the specified Policy type.  See the Policy section in the
		///    <a href=
		///    "{@docRoot}/../technotes/guides/security/StandardNames.html#Policy">
		///    Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		///    for a list of standard Policy types.
		/// </param>
		/// <param name="params"> parameters for the Policy, which may be null.
		/// </param>
		/// <param name="provider"> the provider.
		/// </param>
		/// <returns> the new Policy object.
		/// </returns>
		/// <exception cref="SecurityException"> if the caller does not have permission
		///          to get a Policy instance for the specified type.
		/// </exception>
		/// <exception cref="NullPointerException"> if the specified type is null.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the specified provider
		///          is null or empty,
		///          or if the specified parameters are not understood by
		///          the PolicySpi implementation from the specified provider.
		/// </exception>
		/// <exception cref="NoSuchProviderException"> if the specified provider is not
		///          registered in the security provider list.
		/// </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the specified provider does not
		///          support a PolicySpi implementation for the specified type.
		/// </exception>
		/// <seealso cref= Provider
		/// @since 1.6 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Policy getInstance(String type, Policy.Parameters params, String provider) throws NoSuchProviderException, NoSuchAlgorithmException
		public static Policy GetInstance(String type, Policy.Parameters @params, String provider)
		{

			if (provider == null || provider.Length() == 0)
			{
				throw new IllegalArgumentException("missing provider");
			}

			CheckPermission(type);
			try
			{
				GetInstance.Instance instance = GetInstance.getInstance("Policy", typeof(PolicySpi), type, @params, provider);
				return new PolicyDelegate((PolicySpi)instance.impl, instance.provider, type, @params);
			}
			catch (NoSuchAlgorithmException nsae)
			{
				return HandleException(nsae);
			}
		}

		/// <summary>
		/// Returns a Policy object of the specified type.
		/// 
		/// <para> A new Policy object encapsulating the
		/// PolicySpi implementation from the specified Provider
		/// object is returned.  Note that the specified Provider object
		/// does not have to be registered in the provider list.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the specified Policy type.  See the Policy section in the
		///    <a href=
		///    "{@docRoot}/../technotes/guides/security/StandardNames.html#Policy">
		///    Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		///    for a list of standard Policy types.
		/// </param>
		/// <param name="params"> parameters for the Policy, which may be null.
		/// </param>
		/// <param name="provider"> the Provider.
		/// </param>
		/// <returns> the new Policy object.
		/// </returns>
		/// <exception cref="SecurityException"> if the caller does not have permission
		///          to get a Policy instance for the specified type.
		/// </exception>
		/// <exception cref="NullPointerException"> if the specified type is null.
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the specified Provider is null,
		///          or if the specified parameters are not understood by
		///          the PolicySpi implementation from the specified Provider.
		/// </exception>
		/// <exception cref="NoSuchAlgorithmException"> if the specified Provider does not
		///          support a PolicySpi implementation for the specified type.
		/// </exception>
		/// <seealso cref= Provider
		/// @since 1.6 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Policy getInstance(String type, Policy.Parameters params, Provider provider) throws NoSuchAlgorithmException
		public static Policy GetInstance(String type, Policy.Parameters @params, Provider provider)
		{

			if (provider == null)
			{
				throw new IllegalArgumentException("missing provider");
			}

			CheckPermission(type);
			try
			{
				GetInstance.Instance instance = GetInstance.getInstance("Policy", typeof(PolicySpi), type, @params, provider);
				return new PolicyDelegate((PolicySpi)instance.impl, instance.provider, type, @params);
			}
			catch (NoSuchAlgorithmException nsae)
			{
				return HandleException(nsae);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Policy handleException(NoSuchAlgorithmException nsae) throws NoSuchAlgorithmException
		private static Policy HandleException(NoSuchAlgorithmException nsae)
		{
			Throwable cause = nsae.InnerException;
			if (cause is IllegalArgumentException)
			{
				throw (IllegalArgumentException)cause;
			}
			throw nsae;
		}

		/// <summary>
		/// Return the Provider of this Policy.
		/// 
		/// <para> This Policy instance will only have a Provider if it
		/// was obtained via a call to {@code Policy.getInstance}.
		/// Otherwise this method returns null.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the Provider of this Policy, or null.
		/// 
		/// @since 1.6 </returns>
		public virtual Provider Provider
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Return the type of this Policy.
		/// 
		/// <para> This Policy instance will only have a type if it
		/// was obtained via a call to {@code Policy.getInstance}.
		/// Otherwise this method returns null.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the type of this Policy, or null.
		/// 
		/// @since 1.6 </returns>
		public virtual String Type
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Return Policy parameters.
		/// 
		/// <para> This Policy instance will only have parameters if it
		/// was obtained via a call to {@code Policy.getInstance}.
		/// Otherwise this method returns null.
		/// 
		/// </para>
		/// </summary>
		/// <returns> Policy parameters, or null.
		/// 
		/// @since 1.6 </returns>
		public virtual Policy.Parameters Parameters
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Return a PermissionCollection object containing the set of
		/// permissions granted to the specified CodeSource.
		/// 
		/// <para> Applications are discouraged from calling this method
		/// since this operation may not be supported by all policy implementations.
		/// Applications should solely rely on the {@code implies} method
		/// to perform policy checks.  If an application absolutely must call
		/// a getPermissions method, it should call
		/// {@code getPermissions(ProtectionDomain)}.
		/// 
		/// </para>
		/// <para> The default implementation of this method returns
		/// Policy.UNSUPPORTED_EMPTY_COLLECTION.  This method can be
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
		public virtual PermissionCollection GetPermissions(CodeSource codesource)
		{
			return Policy.UNSUPPORTED_EMPTY_COLLECTION;
		}

		/// <summary>
		/// Return a PermissionCollection object containing the set of
		/// permissions granted to the specified ProtectionDomain.
		/// 
		/// <para> Applications are discouraged from calling this method
		/// since this operation may not be supported by all policy implementations.
		/// Applications should rely on the {@code implies} method
		/// to perform policy checks.
		/// 
		/// </para>
		/// <para> The default implementation of this method first retrieves
		/// the permissions returned via {@code getPermissions(CodeSource)}
		/// (the CodeSource is taken from the specified ProtectionDomain),
		/// as well as the permissions located inside the specified ProtectionDomain.
		/// All of these permissions are then combined and returned in a new
		/// PermissionCollection object.  If {@code getPermissions(CodeSource)}
		/// returns Policy.UNSUPPORTED_EMPTY_COLLECTION, then this method
		/// returns the permissions contained inside the specified ProtectionDomain
		/// in a new PermissionCollection object.
		/// 
		/// </para>
		/// <para> This method can be overridden if the policy implementation
		/// supports returning a set of permissions granted to a ProtectionDomain.
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
		///          Policy.UNSUPPORTED_EMPTY_COLLECTION is returned.
		/// 
		/// @since 1.4 </returns>
		public virtual PermissionCollection GetPermissions(ProtectionDomain domain)
		{
			PermissionCollection pc = null;

			if (domain == null)
			{
				return new Permissions();
			}

			if (PdMapping == null)
			{
				InitPolicy(this);
			}

			lock (PdMapping)
			{
				pc = PdMapping.Get(domain.Key);
			}

			if (pc != null)
			{
				Permissions perms = new Permissions();
				lock (pc)
				{
					for (IEnumerator<Permission> e = pc.Elements(); e.MoveNext();)
					{
						perms.Add(e.Current);
					}
				}
				return perms;
			}

			pc = GetPermissions(domain.CodeSource);
			if (pc == null || pc == UNSUPPORTED_EMPTY_COLLECTION)
			{
				pc = new Permissions();
			}

			AddStaticPerms(pc, domain.Permissions);
			return pc;
		}

		/// <summary>
		/// add static permissions to provided permission collection
		/// </summary>
		private void AddStaticPerms(PermissionCollection perms, PermissionCollection statics)
		{
			if (statics != null)
			{
				lock (statics)
				{
					IEnumerator<Permission> e = statics.Elements();
					while (e.MoveNext())
					{
						perms.Add(e.Current);
					}
				}
			}
		}

		/// <summary>
		/// Evaluates the global policy for the permissions granted to
		/// the ProtectionDomain and tests whether the permission is
		/// granted.
		/// </summary>
		/// <param name="domain"> the ProtectionDomain to test </param>
		/// <param name="permission"> the Permission object to be tested for implication.
		/// </param>
		/// <returns> true if "permission" is a proper subset of a permission
		/// granted to this ProtectionDomain.
		/// </returns>
		/// <seealso cref= java.security.ProtectionDomain
		/// @since 1.4 </seealso>
		public virtual bool Implies(ProtectionDomain domain, Permission permission)
		{
			PermissionCollection pc;

			if (PdMapping == null)
			{
				InitPolicy(this);
			}

			lock (PdMapping)
			{
				pc = PdMapping.Get(domain.Key);
			}

			if (pc != null)
			{
				return pc.Implies(permission);
			}

			pc = GetPermissions(domain);
			if (pc == null)
			{
				return false;
			}

			lock (PdMapping)
			{
				// cache it
				PdMapping.Put(domain.Key, pc);
			}

			return pc.Implies(permission);
		}

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
		public virtual void Refresh()
		{
		}

		/// <summary>
		/// This subclass is returned by the getInstance calls.  All Policy calls
		/// are delegated to the underlying PolicySpi.
		/// </summary>
		private class PolicyDelegate : Policy
		{

			internal PolicySpi Spi;
			internal Provider p;
			internal String Type_Renamed;
			internal Policy.Parameters @params;

			internal PolicyDelegate(PolicySpi spi, Provider p, String type, Policy.Parameters @params)
			{
				this.Spi = spi;
				this.p = p;
				this.Type_Renamed = type;
				this.@params = @params;
			}

			public override String Type
			{
				get
				{
					return Type_Renamed;
				}
			}
			public override Policy.Parameters Parameters
			{
				get
				{
					return @params;
				}
			}
			public override Provider Provider
			{
				get
				{
					return p;
				}
			}
			public override PermissionCollection GetPermissions(CodeSource codesource)
			{
				return Spi.EngineGetPermissions(codesource);
			}
			public override PermissionCollection GetPermissions(ProtectionDomain domain)
			{
				return Spi.EngineGetPermissions(domain);
			}
			public override bool Implies(ProtectionDomain domain, Permission perm)
			{
				return Spi.EngineImplies(domain, perm);
			}
			public override void Refresh()
			{
				Spi.EngineRefresh();
			}
		}

		/// <summary>
		/// This represents a marker interface for Policy parameters.
		/// 
		/// @since 1.6
		/// </summary>
		public interface Parameters
		{
		}

		/// <summary>
		/// This class represents a read-only empty PermissionCollection object that
		/// is returned from the {@code getPermissions(CodeSource)} and
		/// {@code getPermissions(ProtectionDomain)}
		/// methods in the Policy class when those operations are not
		/// supported by the Policy implementation.
		/// </summary>
		private class UnsupportedEmptyCollection : PermissionCollection
		{

			internal const long SerialVersionUID = -8492269157353014774L;

			internal Permissions Perms;

			/// <summary>
			/// Create a read-only empty PermissionCollection object.
			/// </summary>
			public UnsupportedEmptyCollection()
			{
				this.Perms = new Permissions();
				Perms.SetReadOnly();
			}

			/// <summary>
			/// Adds a permission object to the current collection of permission
			/// objects.
			/// </summary>
			/// <param name="permission"> the Permission object to add.
			/// </param>
			/// <exception cref="SecurityException"> - if this PermissionCollection object
			///                                has been marked readonly </exception>
			public override void Add(Permission permission)
			{
				Perms.Add(permission);
			}

			/// <summary>
			/// Checks to see if the specified permission is implied by the
			/// collection of Permission objects held in this PermissionCollection.
			/// </summary>
			/// <param name="permission"> the Permission object to compare.
			/// </param>
			/// <returns> true if "permission" is implied by the permissions in
			/// the collection, false if not. </returns>
			public override bool Implies(Permission permission)
			{
				return Perms.Implies(permission);
			}

			/// <summary>
			/// Returns an enumeration of all the Permission objects in the
			/// collection.
			/// </summary>
			/// <returns> an enumeration of all the Permissions. </returns>
			public override IEnumerator<Permission> Elements()
			{
				return Perms.Elements();
			}
		}
	}

}