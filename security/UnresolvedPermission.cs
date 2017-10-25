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


	/// <summary>
	/// The UnresolvedPermission class is used to hold Permissions that
	/// were "unresolved" when the Policy was initialized.
	/// An unresolved permission is one whose actual Permission class
	/// does not yet exist at the time the Policy is initialized (see below).
	/// 
	/// <para>The policy for a Java runtime (specifying
	/// which permissions are available for code from various principals)
	/// is represented by a Policy object.
	/// Whenever a Policy is initialized or refreshed, Permission objects of
	/// appropriate classes are created for all permissions
	/// allowed by the Policy.
	/// 
	/// </para>
	/// <para>Many permission class types
	/// referenced by the policy configuration are ones that exist
	/// locally (i.e., ones that can be found on CLASSPATH).
	/// Objects for such permissions can be instantiated during
	/// Policy initialization. For example, it is always possible
	/// to instantiate a java.io.FilePermission, since the
	/// FilePermission class is found on the CLASSPATH.
	/// 
	/// </para>
	/// <para>Other permission classes may not yet exist during Policy
	/// initialization. For example, a referenced permission class may
	/// be in a JAR file that will later be loaded.
	/// For each such class, an UnresolvedPermission is instantiated.
	/// Thus, an UnresolvedPermission is essentially a "placeholder"
	/// containing information about the permission.
	/// 
	/// </para>
	/// <para>Later, when code calls AccessController.checkPermission
	/// on a permission of a type that was previously unresolved,
	/// but whose class has since been loaded, previously-unresolved
	/// permissions of that type are "resolved". That is,
	/// for each such UnresolvedPermission, a new object of
	/// the appropriate class type is instantiated, based on the
	/// information in the UnresolvedPermission.
	/// 
	/// </para>
	/// <para> To instantiate the new class, UnresolvedPermission assumes
	/// the class provides a zero, one, and/or two-argument constructor.
	/// The zero-argument constructor would be used to instantiate
	/// a permission without a name and without actions.
	/// A one-arg constructor is assumed to take a {@code String}
	/// name as input, and a two-arg constructor is assumed to take a
	/// {@code String} name and {@code String} actions
	/// as input.  UnresolvedPermission may invoke a
	/// constructor with a {@code null} name and/or actions.
	/// If an appropriate permission constructor is not available,
	/// the UnresolvedPermission is ignored and the relevant permission
	/// will not be granted to executing code.
	/// 
	/// </para>
	/// <para> The newly created permission object replaces the
	/// UnresolvedPermission, which is removed.
	/// 
	/// </para>
	/// <para> Note that the {@code getName} method for an
	/// {@code UnresolvedPermission} returns the
	/// {@code type} (class name) for the underlying permission
	/// that has not been resolved.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.security.Permission </seealso>
	/// <seealso cref= java.security.Permissions </seealso>
	/// <seealso cref= java.security.PermissionCollection </seealso>
	/// <seealso cref= java.security.Policy
	/// 
	/// 
	/// @author Roland Schemers </seealso>

	[Serializable]
	public sealed class UnresolvedPermission : Permission
	{

		private const long SerialVersionUID = -4821973115467008846L;

		private static readonly sun.security.util.Debug Debug = sun.security.util.Debug.getInstance("policy,access", "UnresolvedPermission");

		/// <summary>
		/// The class name of the Permission class that will be
		/// created when this unresolved permission is resolved.
		/// 
		/// @serial
		/// </summary>
		private String Type;

		/// <summary>
		/// The permission name.
		/// 
		/// @serial
		/// </summary>
		private String Name;

		/// <summary>
		/// The actions of the permission.
		/// 
		/// @serial
		/// </summary>
		private String Actions_Renamed;

		[NonSerialized]
		private java.security.cert.Certificate[] Certs;

		/// <summary>
		/// Creates a new UnresolvedPermission containing the permission
		/// information needed later to actually create a Permission of the
		/// specified class, when the permission is resolved.
		/// </summary>
		/// <param name="type"> the class name of the Permission class that will be
		/// created when this unresolved permission is resolved. </param>
		/// <param name="name"> the name of the permission. </param>
		/// <param name="actions"> the actions of the permission. </param>
		/// <param name="certs"> the certificates the permission's class was signed with.
		/// This is a list of certificate chains, where each chain is composed of a
		/// signer certificate and optionally its supporting certificate chain.
		/// Each chain is ordered bottom-to-top (i.e., with the signer certificate
		/// first and the (root) certificate authority last). The signer
		/// certificates are copied from the array. Subsequent changes to
		/// the array will not affect this UnsolvedPermission. </param>
		public UnresolvedPermission(String type, String name, String actions, java.security.cert.Certificate[] certs) : base(type)
		{

			if (type == null)
			{
					throw new NullPointerException("type can't be null");
			}

			this.Type = type;
			this.Name = name;
			this.Actions_Renamed = actions;
			if (certs != null)
			{
				// Extract the signer certs from the list of certificates.
				for (int i = 0; i < certs.Length; i++)
				{
					if (!(certs[i] is X509Certificate))
					{
						// there is no concept of signer certs, so we store the
						// entire cert array
						this.Certs = certs.clone();
						break;
					}
				}

				if (this.Certs == null)
				{
					// Go through the list of certs and see if all the certs are
					// signer certs.
					int i = 0;
					int count = 0;
					while (i < certs.Length)
					{
						count++;
						while (((i + 1) < certs.Length) && ((X509Certificate)certs[i]).IssuerDN.Equals(((X509Certificate)certs[i + 1]).SubjectDN))
						{
							i++;
						}
						i++;
					}
					if (count == certs.Length)
					{
						// All the certs are signer certs, so we store the entire
						// array
						this.Certs = certs.clone();
					}

					if (this.Certs == null)
					{
						// extract the signer certs
						List<java.security.cert.Certificate> signerCerts = new List<java.security.cert.Certificate>();
						i = 0;
						while (i < certs.Length)
						{
							signerCerts.Add(certs[i]);
							while (((i + 1) < certs.Length) && ((X509Certificate)certs[i]).IssuerDN.Equals(((X509Certificate)certs[i + 1]).SubjectDN))
							{
								i++;
							}
							i++;
						}
						this.Certs = new java.security.cert.Certificate[signerCerts.Count];
						signerCerts.toArray(this.Certs);
					}
				}
			}
		}


		private static readonly Class[] PARAMS0 = new Class[] { };
		private static readonly Class[] PARAMS1 = new Class[] {typeof(String)};
		private static readonly Class[] PARAMS2 = new Class[] {typeof(String), typeof(String)};

		/// <summary>
		/// try and resolve this permission using the class loader of the permission
		/// that was passed in.
		/// </summary>
		internal Permission Resolve(Permission p, java.security.cert.Certificate[] certs)
		{
			if (this.Certs != null)
			{
				// if p wasn't signed, we don't have a match
				if (certs == null)
				{
					return null;
				}

				// all certs in this.certs must be present in certs
				bool match;
				for (int i = 0; i < this.Certs.Length; i++)
				{
					match = false;
					for (int j = 0; j < certs.Length; j++)
					{
						if (this.Certs[i].Equals(certs[j]))
						{
							match = true;
							break;
						}
					}
					if (!match)
					{
						return null;
					}
				}
			}
			try
			{
				Class pc = p.GetType();

				if (Name == null && Actions_Renamed == null)
				{
					try
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> c = pc.getConstructor(PARAMS0);
						Constructor<?> c = pc.GetConstructor(PARAMS0);
						return (Permission)c.NewInstance(new Object[] {});
					}
					catch (NoSuchMethodException)
					{
						try
						{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> c = pc.getConstructor(PARAMS1);
							Constructor<?> c = pc.GetConstructor(PARAMS1);
							return (Permission) c.NewInstance(new Object[] {Name});
						}
						catch (NoSuchMethodException)
						{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> c = pc.getConstructor(PARAMS2);
							Constructor<?> c = pc.GetConstructor(PARAMS2);
							return (Permission) c.NewInstance(new Object[] {Name, Actions_Renamed});
						}
					}
				}
				else
				{
					if (Name != null && Actions_Renamed == null)
					{
						try
						{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> c = pc.getConstructor(PARAMS1);
							Constructor<?> c = pc.GetConstructor(PARAMS1);
							return (Permission) c.NewInstance(new Object[] {Name});
						}
						catch (NoSuchMethodException)
						{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> c = pc.getConstructor(PARAMS2);
							Constructor<?> c = pc.GetConstructor(PARAMS2);
							return (Permission) c.NewInstance(new Object[] {Name, Actions_Renamed});
						}
					}
					else
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor<?> c = pc.getConstructor(PARAMS2);
						Constructor<?> c = pc.GetConstructor(PARAMS2);
						return (Permission) c.NewInstance(new Object[] {Name, Actions_Renamed});
					}
				}
			}
			catch (NoSuchMethodException nsme)
			{
				if (Debug != null)
				{
					Debug.println("NoSuchMethodException:\n  could not find " + "proper constructor for " + Type);
					Console.WriteLine(nsme.ToString());
					Console.Write(nsme.StackTrace);
				}
				return null;
			}
			catch (Exception e)
			{
				if (Debug != null)
				{
					Debug.println("unable to instantiate " + Name);
					e.PrintStackTrace();
				}
				return null;
			}
		}

		/// <summary>
		/// This method always returns false for unresolved permissions.
		/// That is, an UnresolvedPermission is never considered to
		/// imply another permission.
		/// </summary>
		/// <param name="p"> the permission to check against.
		/// </param>
		/// <returns> false. </returns>
		public override bool Implies(Permission p)
		{
			return false;
		}

		/// <summary>
		/// Checks two UnresolvedPermission objects for equality.
		/// Checks that <i>obj</i> is an UnresolvedPermission, and has
		/// the same type (class) name, permission name, actions, and
		/// certificates as this object.
		/// 
		/// <para> To determine certificate equality, this method only compares
		/// actual signer certificates.  Supporting certificate chains
		/// are not taken into consideration by this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> the object we are testing for equality with this object.
		/// </param>
		/// <returns> true if obj is an UnresolvedPermission, and has the same
		/// type (class) name, permission name, actions, and
		/// certificates as this object. </returns>
		public override bool Equals(Object obj)
		{
			if (obj == this)
			{
				return true;
			}

			if (!(obj is UnresolvedPermission))
			{
				return false;
			}
			UnresolvedPermission that = (UnresolvedPermission) obj;

			// check type
			if (!this.Type.Equals(that.Type))
			{
				return false;
			}

			// check name
			if (this.Name == null)
			{
				if (that.Name != null)
				{
					return false;
				}
			}
			else if (!this.Name.Equals(that.Name))
			{
				return false;
			}

			// check actions
			if (this.Actions_Renamed == null)
			{
				if (that.Actions_Renamed != null)
				{
					return false;
				}
			}
			else
			{
				if (!this.Actions_Renamed.Equals(that.Actions_Renamed))
				{
					return false;
				}
			}

			// check certs
			if ((this.Certs == null && that.Certs != null) || (this.Certs != null && that.Certs == null) || (this.Certs != null && that.Certs != null && this.Certs.Length != that.Certs.Length))
			{
				return false;
			}

			int i, j;
			bool match;

			for (i = 0; this.Certs != null && i < this.Certs.Length; i++)
			{
				match = false;
				for (j = 0; j < that.Certs.Length; j++)
				{
					if (this.Certs[i].Equals(that.Certs[j]))
					{
						match = true;
						break;
					}
				}
				if (!match)
				{
					return false;
				}
			}

			for (i = 0; that.Certs != null && i < that.Certs.Length; i++)
			{
				match = false;
				for (j = 0; j < this.Certs.Length; j++)
				{
					if (that.Certs[i].Equals(this.Certs[j]))
					{
						match = true;
						break;
					}
				}
				if (!match)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns the hash code value for this object.
		/// </summary>
		/// <returns> a hash code value for this object. </returns>

		public override int HashCode()
		{
			int hash = Type.HashCode();
			if (Name != null)
			{
				hash ^= Name.HashCode();
			}
			if (Actions_Renamed != null)
			{
				hash ^= Actions_Renamed.HashCode();
			}
			return hash;
		}

		/// <summary>
		/// Returns the canonical string representation of the actions,
		/// which currently is the empty string "", since there are no actions for
		/// an UnresolvedPermission. That is, the actions for the
		/// permission that will be created when this UnresolvedPermission
		/// is resolved may be non-null, but an UnresolvedPermission
		/// itself is never considered to have any actions.
		/// </summary>
		/// <returns> the empty string "". </returns>
		public override String Actions
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// Get the type (class name) of the underlying permission that
		/// has not been resolved.
		/// </summary>
		/// <returns> the type (class name) of the underlying permission that
		///  has not been resolved
		/// 
		/// @since 1.5 </returns>
		public String UnresolvedType
		{
			get
			{
				return Type;
			}
		}

		/// <summary>
		/// Get the target name of the underlying permission that
		/// has not been resolved.
		/// </summary>
		/// <returns> the target name of the underlying permission that
		///          has not been resolved, or {@code null},
		///          if there is no target name
		/// 
		/// @since 1.5 </returns>
		public String UnresolvedName
		{
			get
			{
				return Name;
			}
		}

		/// <summary>
		/// Get the actions for the underlying permission that
		/// has not been resolved.
		/// </summary>
		/// <returns> the actions for the underlying permission that
		///          has not been resolved, or {@code null}
		///          if there are no actions
		/// 
		/// @since 1.5 </returns>
		public String UnresolvedActions
		{
			get
			{
				return Actions_Renamed;
			}
		}

		/// <summary>
		/// Get the signer certificates (without any supporting chain)
		/// for the underlying permission that has not been resolved.
		/// </summary>
		/// <returns> the signer certificates for the underlying permission that
		/// has not been resolved, or null, if there are no signer certificates.
		/// Returns a new array each time this method is called.
		/// 
		/// @since 1.5 </returns>
		public java.security.cert.Certificate[] UnresolvedCerts
		{
			get
			{
				return (Certs == null) ? null : Certs.clone();
			}
		}

		/// <summary>
		/// Returns a string describing this UnresolvedPermission.  The convention
		/// is to specify the class name, the permission name, and the actions, in
		/// the following format: '(unresolved "ClassName" "name" "actions")'.
		/// </summary>
		/// <returns> information about this UnresolvedPermission. </returns>
		public override String ToString()
		{
			return "(unresolved " + Type + " " + Name + " " + Actions_Renamed + ")";
		}

		/// <summary>
		/// Returns a new PermissionCollection object for storing
		/// UnresolvedPermission  objects.
		/// <para>
		/// </para>
		/// </summary>
		/// <returns> a new PermissionCollection object suitable for
		/// storing UnresolvedPermissions. </returns>

		public override PermissionCollection NewPermissionCollection()
		{
			return new UnresolvedPermissionCollection();
		}

		/// <summary>
		/// Writes this object out to a stream (i.e., serializes it).
		/// 
		/// @serialData An initial {@code String} denoting the
		/// {@code type} is followed by a {@code String} denoting the
		/// {@code name} is followed by a {@code String} denoting the
		/// {@code actions} is followed by an {@code int} indicating the
		/// number of certificates to follow
		/// (a value of "zero" denotes that there are no certificates associated
		/// with this object).
		/// Each certificate is written out starting with a {@code String}
		/// denoting the certificate type, followed by an
		/// {@code int} specifying the length of the certificate encoding,
		/// followed by the certificate encoding itself which is written out as an
		/// array of bytes.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream oos)
		{
			oos.DefaultWriteObject();

			if (Certs == null || Certs.Length == 0)
			{
				oos.WriteInt(0);
			}
			else
			{
				// write out the total number of certs
				oos.WriteInt(Certs.Length);
				// write out each cert, including its type
				for (int i = 0; i < Certs.Length; i++)
				{
					java.security.cert.Certificate cert = Certs[i];
					try
					{
						oos.WriteUTF(cert.Type);
						sbyte[] encoded = cert.Encoded;
						oos.WriteInt(encoded.Length);
						oos.Write(encoded);
					}
					catch (CertificateEncodingException cee)
					{
						throw new IOException(cee.Message);
					}
				}
			}
		}

		/// <summary>
		/// Restores this object from a stream (i.e., deserializes it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream ois)
		{
			CertificateFactory cf;
			Dictionary<String, CertificateFactory> cfs = null;

			ois.DefaultReadObject();

			if (Type == null)
			{
					throw new NullPointerException("type can't be null");
			}

			// process any new-style certs in the stream (if present)
			int size = ois.ReadInt();
			if (size > 0)
			{
				// we know of 3 different cert types: X.509, PGP, SDSI, which
				// could all be present in the stream at the same time
				cfs = new Dictionary<String, CertificateFactory>(3);
				this.Certs = new java.security.cert.Certificate[size];
			}

			for (int i = 0; i < size; i++)
			{
				// read the certificate type, and instantiate a certificate
				// factory of that type (reuse existing factory if possible)
				String certType = ois.ReadUTF();
				if (cfs.ContainsKey(certType))
				{
					// reuse certificate factory
					cf = cfs[certType];
				}
				else
				{
					// create new certificate factory
					try
					{
						cf = CertificateFactory.GetInstance(certType);
					}
					catch (CertificateException)
					{
						throw new ClassNotFoundException("Certificate factory for " + certType + " not found");
					}
					// store the certificate factory so we can reuse it later
					cfs[certType] = cf;
				}
				// parse the certificate
				sbyte[] encoded = null;
				try
				{
					encoded = new sbyte[ois.ReadInt()];
				}
				catch (OutOfMemoryError)
				{
					throw new IOException("Certificate too big");
				}
				ois.ReadFully(encoded);
				ByteArrayInputStream bais = new ByteArrayInputStream(encoded);
				try
				{
					this.Certs[i] = cf.GenerateCertificate(bais);
				}
				catch (CertificateException ce)
				{
					throw new IOException(ce.Message);
				}
				bais.Close();
			}
		}
	}

}