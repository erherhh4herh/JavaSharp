using System;

/*
 * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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
	/// <para>This class represents identities: real-world objects such as people,
	/// companies or organizations whose identities can be authenticated using
	/// their public keys. Identities may also be more abstract (or concrete)
	/// constructs, such as daemon threads or smart cards.
	/// 
	/// </para>
	/// <para>All Identity objects have a name and a public key. Names are
	/// immutable. Identities may also be scoped. That is, if an Identity is
	/// specified to have a particular scope, then the name and public
	/// key of the Identity are unique within that scope.
	/// 
	/// </para>
	/// <para>An Identity also has a set of certificates (all certifying its own
	/// public key). The Principal names specified in these certificates need
	/// not be the same, only the key.
	/// 
	/// </para>
	/// <para>An Identity can be subclassed, to include postal and email addresses,
	/// telephone numbers, images of faces and logos, and so on.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= IdentityScope </seealso>
	/// <seealso cref= Signer </seealso>
	/// <seealso cref= Principal
	/// 
	/// @author Benjamin Renaud </seealso>
	/// @deprecated This class is no longer used. Its functionality has been
	/// replaced by {@code java.security.KeyStore}, the
	/// {@code java.security.cert} package, and
	/// {@code java.security.Principal}. 
	[Obsolete("This class is no longer used. Its functionality has been"), Serializable]
	public abstract class Identity : Principal
	{

		/// <summary>
		/// use serialVersionUID from JDK 1.1.x for interoperability </summary>
		private const long SerialVersionUID = 3609922007826600659L;

		/// <summary>
		/// The name for this identity.
		/// 
		/// @serial
		/// </summary>
		private String Name_Renamed;

		/// <summary>
		/// The public key for this identity.
		/// 
		/// @serial
		/// </summary>
		private PublicKey PublicKey_Renamed;

		/// <summary>
		/// Generic, descriptive information about the identity.
		/// 
		/// @serial
		/// </summary>
		internal String Info_Renamed = "No further information available.";

		/// <summary>
		/// The scope of the identity.
		/// 
		/// @serial
		/// </summary>
		internal IdentityScope Scope_Renamed;

		/// <summary>
		/// The certificates for this identity.
		/// 
		/// @serial
		/// </summary>
		internal Vector<Certificate> Certificates_Renamed;

		/// <summary>
		/// Constructor for serialization only.
		/// </summary>
		protected internal Identity() : this("restoring...")
		{
		}

		/// <summary>
		/// Constructs an identity with the specified name and scope.
		/// </summary>
		/// <param name="name"> the identity name. </param>
		/// <param name="scope"> the scope of the identity.
		/// </param>
		/// <exception cref="KeyManagementException"> if there is already an identity
		/// with the same name in the scope. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Identity(String name, IdentityScope scope) throws KeyManagementException
		public Identity(String name, IdentityScope scope) : this(name)
		{
			if (scope != null)
			{
				scope.AddIdentity(this);
			}
			this.Scope_Renamed = scope;
		}

		/// <summary>
		/// Constructs an identity with the specified name and no scope.
		/// </summary>
		/// <param name="name"> the identity name. </param>
		public Identity(String name)
		{
			this.Name_Renamed = name;
		}

		/// <summary>
		/// Returns this identity's name.
		/// </summary>
		/// <returns> the name of this identity. </returns>
		public String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Returns this identity's scope.
		/// </summary>
		/// <returns> the scope of this identity. </returns>
		public IdentityScope Scope
		{
			get
			{
				return Scope_Renamed;
			}
		}

		/// <summary>
		/// Returns this identity's public key.
		/// </summary>
		/// <returns> the public key for this identity.
		/// </returns>
		/// <seealso cref= #setPublicKey </seealso>
		public virtual PublicKey PublicKey
		{
			get
			{
				return PublicKey_Renamed;
			}
			set
			{
    
				Check("setIdentityPublicKey");
				this.PublicKey_Renamed = value;
				Certificates_Renamed = new Vector<Certificate>();
			}
		}


		/// <summary>
		/// Specifies a general information string for this identity.
		/// 
		/// <para>First, if there is a security manager, its {@code checkSecurityAccess}
		/// method is called with {@code "setIdentityInfo"}
		/// as its argument to see if it's ok to specify the information string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="info"> the information string.
		/// </param>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkSecurityAccess} method doesn't allow
		/// setting the information string.
		/// </exception>
		/// <seealso cref= #getInfo </seealso>
		/// <seealso cref= SecurityManager#checkSecurityAccess </seealso>
		public virtual String Info
		{
			set
			{
				Check("setIdentityInfo");
				this.Info_Renamed = value;
			}
			get
			{
				return Info_Renamed;
			}
		}


		/// <summary>
		/// Adds a certificate for this identity. If the identity has a public
		/// key, the public key in the certificate must be the same, and if
		/// the identity does not have a public key, the identity's
		/// public key is set to be that specified in the certificate.
		/// 
		/// <para>First, if there is a security manager, its {@code checkSecurityAccess}
		/// method is called with {@code "addIdentityCertificate"}
		/// as its argument to see if it's ok to add a certificate.
		/// 
		/// </para>
		/// </summary>
		/// <param name="certificate"> the certificate to be added.
		/// </param>
		/// <exception cref="KeyManagementException"> if the certificate is not valid,
		/// if the public key in the certificate being added conflicts with
		/// this identity's public key, or if another exception occurs.
		/// </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkSecurityAccess} method doesn't allow
		/// adding a certificate.
		/// </exception>
		/// <seealso cref= SecurityManager#checkSecurityAccess </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addCertificate(Certificate certificate) throws KeyManagementException
		public virtual void AddCertificate(Certificate certificate)
		{

			Check("addIdentityCertificate");

			if (Certificates_Renamed == null)
			{
				Certificates_Renamed = new Vector<Certificate>();
			}
			if (PublicKey_Renamed != null)
			{
				if (!KeyEquals(PublicKey_Renamed, certificate.PublicKey))
				{
					throw new KeyManagementException("public key different from cert public key");
				}
			}
			else
			{
				PublicKey_Renamed = certificate.PublicKey;
			}
			Certificates_Renamed.AddElement(certificate);
		}

		private bool KeyEquals(PublicKey aKey, PublicKey anotherKey)
		{
			String aKeyFormat = aKey.Format;
			String anotherKeyFormat = anotherKey.Format;
			if ((aKeyFormat == null) ^ (anotherKeyFormat == null))
			{
				return Principal_Fields.False;
			}
			if (aKeyFormat != null && anotherKeyFormat != null)
			{
				if (!aKeyFormat.EqualsIgnoreCase(anotherKeyFormat))
				{
					return Principal_Fields.False;
				}
			}
			return System.Array.Equals(aKey.Encoded, anotherKey.Encoded);
		}


		/// <summary>
		/// Removes a certificate from this identity.
		/// 
		/// <para>First, if there is a security manager, its {@code checkSecurityAccess}
		/// method is called with {@code "removeIdentityCertificate"}
		/// as its argument to see if it's ok to remove a certificate.
		/// 
		/// </para>
		/// </summary>
		/// <param name="certificate"> the certificate to be removed.
		/// </param>
		/// <exception cref="KeyManagementException"> if the certificate is
		/// missing, or if another exception occurs.
		/// </exception>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkSecurityAccess} method doesn't allow
		/// removing a certificate.
		/// </exception>
		/// <seealso cref= SecurityManager#checkSecurityAccess </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeCertificate(Certificate certificate) throws KeyManagementException
		public virtual void RemoveCertificate(Certificate certificate)
		{
			Check("removeIdentityCertificate");
			if (Certificates_Renamed != null)
			{
				Certificates_Renamed.RemoveElement(certificate);
			}
		}

		/// <summary>
		/// Returns a copy of all the certificates for this identity.
		/// </summary>
		/// <returns> a copy of all the certificates for this identity. </returns>
		public virtual Certificate[] Certificates()
		{
			if (Certificates_Renamed == null)
			{
				return new Certificate[0];
			}
			int len = Certificates_Renamed.Size();
			Certificate[] certs = new Certificate[len];
			Certificates_Renamed.CopyInto(certs);
			return certs;
		}

		/// <summary>
		/// Tests for equality between the specified object and this identity.
		/// This first tests to see if the entities actually refer to the same
		/// object, in which case it returns true. Next, it checks to see if
		/// the entities have the same name and the same scope. If they do,
		/// the method returns true. Otherwise, it calls
		/// <seealso cref="#identityEquals(Identity) identityEquals"/>, which subclasses should
		/// override.
		/// </summary>
		/// <param name="identity"> the object to test for equality with this identity.
		/// </param>
		/// <returns> true if the objects are considered equal, false otherwise.
		/// </returns>
		/// <seealso cref= #identityEquals </seealso>
		public sealed override bool Equals(Object identity)
		{

			if (identity == this)
			{
				return true;
			}

			if (identity is Identity)
			{
				Identity i = (Identity)identity;
				if (this.FullName().Equals(i.FullName()))
				{
					return true;
				}
				else
				{
					return IdentityEquals(i);
				}
			}
			return Principal_Fields.False;
		}

		/// <summary>
		/// Tests for equality between the specified identity and this identity.
		/// This method should be overriden by subclasses to test for equality.
		/// The default behavior is to return true if the names and public keys
		/// are equal.
		/// </summary>
		/// <param name="identity"> the identity to test for equality with this identity.
		/// </param>
		/// <returns> true if the identities are considered equal, false
		/// otherwise.
		/// </returns>
		/// <seealso cref= #equals </seealso>
		protected internal virtual bool IdentityEquals(Identity identity)
		{
			if (!Name_Renamed.EqualsIgnoreCase(identity.Name_Renamed))
			{
				return Principal_Fields.False;
			}

			if ((PublicKey_Renamed == null) ^ (identity.PublicKey_Renamed == null))
			{
				return Principal_Fields.False;
			}

			if (PublicKey_Renamed != null && identity.PublicKey_Renamed != null)
			{
				if (!PublicKey_Renamed.Equals(identity.PublicKey_Renamed))
				{
					return Principal_Fields.False;
				}
			}

			return true;

		}

		/// <summary>
		/// Returns a parsable name for identity: identityName.scopeName
		/// </summary>
		internal virtual String FullName()
		{
			String parsable = Name_Renamed;
			if (Scope_Renamed != null)
			{
				parsable += "." + Scope_Renamed.Name;
			}
			return parsable;
		}

		/// <summary>
		/// Returns a short string describing this identity, telling its
		/// name and its scope (if any).
		/// 
		/// <para>First, if there is a security manager, its {@code checkSecurityAccess}
		/// method is called with {@code "printIdentity"}
		/// as its argument to see if it's ok to return the string.
		/// 
		/// </para>
		/// </summary>
		/// <returns> information about this identity, such as its name and the
		/// name of its scope (if any).
		/// </returns>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkSecurityAccess} method doesn't allow
		/// returning a string describing this identity.
		/// </exception>
		/// <seealso cref= SecurityManager#checkSecurityAccess </seealso>
		public override String ToString()
		{
			Check("printIdentity");
			String printable = Name_Renamed;
			if (Scope_Renamed != null)
			{
				printable += "[" + Scope_Renamed.Name + "]";
			}
			return printable;
		}

		/// <summary>
		/// Returns a string representation of this identity, with
		/// optionally more details than that provided by the
		/// {@code toString} method without any arguments.
		/// 
		/// <para>First, if there is a security manager, its {@code checkSecurityAccess}
		/// method is called with {@code "printIdentity"}
		/// as its argument to see if it's ok to return the string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="detailed"> whether or not to provide detailed information.
		/// </param>
		/// <returns> information about this identity. If {@code detailed}
		/// is true, then this method returns more information than that
		/// provided by the {@code toString} method without any arguments.
		/// </returns>
		/// <exception cref="SecurityException">  if a security manager exists and its
		/// {@code checkSecurityAccess} method doesn't allow
		/// returning a string describing this identity.
		/// </exception>
		/// <seealso cref= #toString </seealso>
		/// <seealso cref= SecurityManager#checkSecurityAccess </seealso>
		public virtual String ToString(bool detailed)
		{
			String @out = ToString();
			if (detailed)
			{
				@out += "\n";
				@out += PrintKeys();
				@out += "\n" + PrintCertificates();
				if (Info_Renamed != null)
				{
					@out += "\n\t" + Info_Renamed;
				}
				else
				{
					@out += "\n\tno additional information available.";
				}
			}
			return @out;
		}

		internal virtual String PrintKeys()
		{
			String key = "";
			if (PublicKey_Renamed != null)
			{
				key = "\tpublic key initialized";
			}
			else
			{
				key = "\tno public key";
			}
			return key;
		}

		internal virtual String PrintCertificates()
		{
			String @out = "";
			if (Certificates_Renamed == null)
			{
				return "\tno certificates";
			}
			else
			{
				@out += "\tcertificates: \n";

				int i = 1;
				foreach (Certificate cert in Certificates_Renamed)
				{
					@out += "\tcertificate " + i++ + "\tfor  : " + cert.Principal + "\n";
					@out += "\t\t\tfrom : " + cert.Guarantor + "\n";
				}
			}
			return @out;
		}

		/// <summary>
		/// Returns a hashcode for this identity.
		/// </summary>
		/// <returns> a hashcode for this identity. </returns>
		public override int HashCode()
		{
			return Name_Renamed.HashCode();
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