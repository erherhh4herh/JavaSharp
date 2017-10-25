using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// <para>This class represents a scope for identities. It is an Identity
	/// itself, and therefore has a name and can have a scope. It can also
	/// optionally have a public key and associated certificates.
	/// 
	/// </para>
	/// <para>An IdentityScope can contain Identity objects of all kinds, including
	/// Signers. All types of Identity objects can be retrieved, added, and
	/// removed using the same methods. Note that it is possible, and in fact
	/// expected, that different types of identity scopes will
	/// apply different policies for their various operations on the
	/// various types of Identities.
	/// 
	/// </para>
	/// <para>There is a one-to-one mapping between keys and identities, and
	/// there can only be one copy of one key per scope. For example, suppose
	/// <b>Acme Software, Inc</b> is a software publisher known to a user.
	/// Suppose it is an Identity, that is, it has a public key, and a set of
	/// associated certificates. It is named in the scope using the name
	/// "Acme Software". No other named Identity in the scope has the same
	/// public  key. Of course, none has the same name as well.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Identity </seealso>
	/// <seealso cref= Signer </seealso>
	/// <seealso cref= Principal </seealso>
	/// <seealso cref= Key
	/// 
	/// @author Benjamin Renaud
	/// </seealso>
	/// @deprecated This class is no longer used. Its functionality has been
	/// replaced by {@code java.security.KeyStore}, the
	/// {@code java.security.cert} package, and
	/// {@code java.security.Principal}. 
	[Obsolete("This class is no longer used. Its functionality has been")]
	public abstract class IdentityScope : Identity
	{

		private const long SerialVersionUID = -2337346281189773310L;

		/* The system's scope */
		private static new IdentityScope Scope;

		// initialize the system scope
		private static void InitializeSystemScope()
		{

			String classname = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());

			if (classname == null)
			{
				return;

			}
			else
			{

				try
				{
					Class.ForName(classname);
				}
				catch (ClassNotFoundException e)
				{
					//Security.error("unable to establish a system scope from " +
					//             classname);
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual String Run()
			{
				return Security.GetProperty("system.scope");
			}
		}

		/// <summary>
		/// This constructor is used for serialization only and should not
		/// be used by subclasses.
		/// </summary>
		protected internal IdentityScope() : this("restoring...")
		{
		}

		/// <summary>
		/// Constructs a new identity scope with the specified name.
		/// </summary>
		/// <param name="name"> the scope name. </param>
		public IdentityScope(String name) : base(name)
		{
		}

		/// <summary>
		/// Constructs a new identity scope with the specified name and scope.
		/// </summary>
		/// <param name="name"> the scope name. </param>
		/// <param name="scope"> the scope for the new identity scope.
		/// </param>
		/// <exception cref="KeyManagementException"> if there is already an identity
		/// with the same name in the scope. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public IdentityScope(String name, IdentityScope scope) throws KeyManagementException
		public IdentityScope(String name, IdentityScope scope) : base(name, scope)
		{
		}

		/// <summary>
		/// Returns the system's identity scope.
		/// </summary>
		/// <returns> the system's identity scope, or {@code null} if none has been
		///         set.
		/// </returns>
		/// <seealso cref= #setSystemScope </seealso>
		public static IdentityScope SystemScope
		{
			get
			{
				if (Scope == null)
				{
					InitializeSystemScope();
				}
				return Scope;
			}
			set
			{
				Check("setSystemScope");
				IdentityScope.Scope = value;
			}
		}



		/// <summary>
		/// Returns the number of identities within this identity scope.
		/// </summary>
		/// <returns> the number of identities within this identity scope. </returns>
		public abstract int Size();

		/// <summary>
		/// Returns the identity in this scope with the specified name (if any).
		/// </summary>
		/// <param name="name"> the name of the identity to be retrieved.
		/// </param>
		/// <returns> the identity named {@code name}, or null if there are
		/// no identities named {@code name} in this scope. </returns>
		public abstract Identity GetIdentity(String name);

		/// <summary>
		/// Retrieves the identity whose name is the same as that of the
		/// specified principal. (Note: Identity implements Principal.)
		/// </summary>
		/// <param name="principal"> the principal corresponding to the identity
		/// to be retrieved.
		/// </param>
		/// <returns> the identity whose name is the same as that of the
		/// principal, or null if there are no identities of the same name
		/// in this scope. </returns>
		public virtual Identity GetIdentity(Principal principal)
		{
			return GetIdentity(principal.Name);
		}

		/// <summary>
		/// Retrieves the identity with the specified public key.
		/// </summary>
		/// <param name="key"> the public key for the identity to be returned.
		/// </param>
		/// <returns> the identity with the given key, or null if there are
		/// no identities in this scope with that key. </returns>
		public abstract Identity GetIdentity(PublicKey key);

		/// <summary>
		/// Adds an identity to this identity scope.
		/// </summary>
		/// <param name="identity"> the identity to be added.
		/// </param>
		/// <exception cref="KeyManagementException"> if the identity is not
		/// valid, a name conflict occurs, another identity has the same
		/// public key as the identity being added, or another exception
		/// occurs.  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void addIdentity(Identity identity) throws KeyManagementException;
		public abstract void AddIdentity(Identity identity);

		/// <summary>
		/// Removes an identity from this identity scope.
		/// </summary>
		/// <param name="identity"> the identity to be removed.
		/// </param>
		/// <exception cref="KeyManagementException"> if the identity is missing,
		/// or another exception occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void removeIdentity(Identity identity) throws KeyManagementException;
		public abstract void RemoveIdentity(Identity identity);

		/// <summary>
		/// Returns an enumeration of all identities in this identity scope.
		/// </summary>
		/// <returns> an enumeration of all identities in this identity scope. </returns>
		public abstract IEnumerator<Identity> Identities();

		/// <summary>
		/// Returns a string representation of this identity scope, including
		/// its name, its scope name, and the number of identities in this
		/// identity scope.
		/// </summary>
		/// <returns> a string representation of this identity scope. </returns>
		public override String ToString()
		{
			return base.ToString() + "[" + Size() + "]";
		}

		private static void Check(String directive)
		{
			SecurityManager security = System.SecurityManager;
			if (security != null)
			{
				security.CheckSecurityAccess(directive);
			}
		}

	}

}