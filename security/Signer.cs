using System;

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
	/// This class is used to represent an Identity that can also digitally
	/// sign data.
	/// 
	/// <para>The management of a signer's private keys is an important and
	/// sensitive issue that should be handled by subclasses as appropriate
	/// to their intended use.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Identity
	/// 
	/// @author Benjamin Renaud
	/// </seealso>
	/// @deprecated This class is no longer used. Its functionality has been
	/// replaced by {@code java.security.KeyStore}, the
	/// {@code java.security.cert} package, and
	/// {@code java.security.Principal}. 
	[Obsolete("This class is no longer used. Its functionality has been")]
	public abstract class Signer : Identity
	{

		private const long SerialVersionUID = -1763464102261361480L;

		/// <summary>
		/// The signer's private key.
		/// 
		/// @serial
		/// </summary>
		private PrivateKey PrivateKey_Renamed;

		/// <summary>
		/// Creates a signer. This constructor should only be used for
		/// serialization.
		/// </summary>
		protected internal Signer() : base()
		{
		}


		/// <summary>
		/// Creates a signer with the specified identity name.
		/// </summary>
		/// <param name="name"> the identity name. </param>
		public Signer(String name) : base(name)
		{
		}

		/// <summary>
		/// Creates a signer with the specified identity name and scope.
		/// </summary>
		/// <param name="name"> the identity name.
		/// </param>
		/// <param name="scope"> the scope of the identity.
		/// </param>
		/// <exception cref="KeyManagementException"> if there is already an identity
		/// with the same name in the scope. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Signer(String name, IdentityScope scope) throws KeyManagementException
		public Signer(String name, IdentityScope scope) : base(name, scope)
		{
		}

		/// <summary>
		/// Returns this signer's private key.
		/// 
		/// <para>First, if there is a security manager, its {@code checkSecurityAccess}
		/// method is called with {@code "getSignerPrivateKey"}
		/// as its argument to see if it's ok to return the private key.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this signer's private key, or null if the private key has
		/// not yet been set.
		/// </returns>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkSecurityAccess} method doesn't allow
		/// returning the private key.
		/// </exception>
		/// <seealso cref= SecurityManager#checkSecurityAccess </seealso>
		public virtual PrivateKey PrivateKey
		{
			get
			{
				Check("getSignerPrivateKey");
				return PrivateKey_Renamed;
			}
		}

	   /// <summary>
	   /// Sets the key pair (public key and private key) for this signer.
	   ///  
	   /// <para>First, if there is a security manager, its {@code checkSecurityAccess}
	   /// method is called with {@code "setSignerKeyPair"}
	   /// as its argument to see if it's ok to set the key pair.
	   ///  
	   /// </para>
	   /// </summary>
	   /// <param name="pair"> an initialized key pair.
	   /// </param>
	   /// <exception cref="InvalidParameterException"> if the key pair is not
	   /// properly initialized. </exception>
	   /// <exception cref="KeyException"> if the key pair cannot be set for any
	   /// other reason. </exception>
	   /// <exception cref="SecurityException">  if a security manager exists and its
	   /// {@code checkSecurityAccess} method doesn't allow
	   /// setting the key pair.
	   /// </exception>
	   /// <seealso cref= SecurityManager#checkSecurityAccess </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void setKeyPair(KeyPair pair) throws InvalidParameterException, KeyException
		public KeyPair KeyPair
		{
			set
			{
				Check("setSignerKeyPair");
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final PublicKey pub = value.getPublic();
				PublicKey pub = value.Public;
				PrivateKey priv = value.Private;
    
				if (pub == null || priv == null)
				{
					throw new InvalidParameterException();
				}
				try
				{
					AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this, pub));
				}
				catch (PrivilegedActionException pae)
				{
					throw (KeyManagementException) pae.Exception;
				}
				PrivateKey_Renamed = priv;
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Void>
		{
			private readonly Signer OuterInstance;

			private java.security.PublicKey Pub;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(Signer outerInstance, java.security.PublicKey pub)
			{
				this.OuterInstance = outerInstance;
				this.Pub = pub;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws KeyManagementException
			public virtual Void Run()
			{
				outerInstance.PublicKey = Pub;
				return null;
			}
		}

		internal override String PrintKeys()
		{
			String keys = "";
			PublicKey publicKey = PublicKey;
			if (publicKey != null && PrivateKey_Renamed != null)
			{
				keys = "\tpublic and private keys initialized";

			}
			else
			{
				keys = "\tno keys";
			}
			return keys;
		}

		/// <summary>
		/// Returns a string of information about the signer.
		/// </summary>
		/// <returns> a string of information about the signer. </returns>
		public override String ToString()
		{
			return "[Signer]" + base.ToString();
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