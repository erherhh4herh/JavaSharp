using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2007, 2014, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security.cert
{


	using ObjectIdentifier = sun.security.util.ObjectIdentifier;
	using InvalidityDateExtension = sun.security.x509.InvalidityDateExtension;

	/// <summary>
	/// An exception that indicates an X.509 certificate is revoked. A
	/// {@code CertificateRevokedException} contains additional information
	/// about the revoked certificate, such as the date on which the
	/// certificate was revoked and the reason it was revoked.
	/// 
	/// @author Sean Mullan
	/// @since 1.7 </summary>
	/// <seealso cref= CertPathValidatorException </seealso>
	public class CertificateRevokedException : CertificateException
	{

		private new const long SerialVersionUID = 7839996631571608627L;

		/// <summary>
		/// @serial the date on which the certificate was revoked
		/// </summary>
		private DateTime RevocationDate_Renamed;
		/// <summary>
		/// @serial the revocation reason
		/// </summary>
		private readonly CRLReason Reason;
		/// <summary>
		/// @serial the {@code X500Principal} that represents the name of the
		/// authority that signed the certificate's revocation status information
		/// </summary>
		private readonly X500Principal Authority;

		[NonSerialized]
		private IDictionary<String, Extension> Extensions_Renamed;

		/// <summary>
		/// Constructs a {@code CertificateRevokedException} with
		/// the specified revocation date, reason code, authority name, and map
		/// of extensions.
		/// </summary>
		/// <param name="revocationDate"> the date on which the certificate was revoked. The
		///    date is copied to protect against subsequent modification. </param>
		/// <param name="reason"> the revocation reason </param>
		/// <param name="extensions"> a map of X.509 Extensions. Each key is an OID String
		///    that maps to the corresponding Extension. The map is copied to
		///    prevent subsequent modification. </param>
		/// <param name="authority"> the {@code X500Principal} that represents the name
		///    of the authority that signed the certificate's revocation status
		///    information </param>
		/// <exception cref="NullPointerException"> if {@code revocationDate},
		///    {@code reason}, {@code authority}, or
		///    {@code extensions} is {@code null} </exception>
		public CertificateRevokedException(DateTime revocationDate, CRLReason reason, X500Principal authority, IDictionary<String, Extension> extensions)
		{
			if (revocationDate == null || reason == null || authority == null || extensions == null)
			{
				throw new NullPointerException();
			}
			this.RevocationDate_Renamed = new DateTime(revocationDate.Ticks);
			this.Reason = reason;
			this.Authority = authority;
			// make sure Map only contains correct types
			this.Extensions_Renamed = Collections.CheckedMap(new Dictionary<>(), typeof(String), typeof(Extension));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			this.Extensions_Renamed.putAll(extensions);
		}

		/// <summary>
		/// Returns the date on which the certificate was revoked. A new copy is
		/// returned each time the method is invoked to protect against subsequent
		/// modification.
		/// </summary>
		/// <returns> the revocation date </returns>
		public virtual DateTime RevocationDate
		{
			get
			{
				return (DateTime) RevocationDate_Renamed.Clone();
			}
		}

		/// <summary>
		/// Returns the reason the certificate was revoked.
		/// </summary>
		/// <returns> the revocation reason </returns>
		public virtual CRLReason RevocationReason
		{
			get
			{
				return Reason;
			}
		}

		/// <summary>
		/// Returns the name of the authority that signed the certificate's
		/// revocation status information.
		/// </summary>
		/// <returns> the {@code X500Principal} that represents the name of the
		///     authority that signed the certificate's revocation status information </returns>
		public virtual X500Principal AuthorityName
		{
			get
			{
				return Authority;
			}
		}

		/// <summary>
		/// Returns the invalidity date, as specified in the Invalidity Date
		/// extension of this {@code CertificateRevokedException}. The
		/// invalidity date is the date on which it is known or suspected that the
		/// private key was compromised or that the certificate otherwise became
		/// invalid. This implementation calls {@code getExtensions()} and
		/// checks the returned map for an entry for the Invalidity Date extension
		/// OID ("2.5.29.24"). If found, it returns the invalidity date in the
		/// extension; otherwise null. A new Date object is returned each time the
		/// method is invoked to protect against subsequent modification.
		/// </summary>
		/// <returns> the invalidity date, or {@code null} if not specified </returns>
		public virtual DateTime InvalidityDate
		{
			get
			{
				Extension ext = Extensions["2.5.29.24"];
				if (ext == null)
				{
					return null;
				}
				else
				{
					try
					{
						DateTime invalidity = InvalidityDateExtension.toImpl(ext).get("DATE");
						return new DateTime(invalidity.Ticks);
					}
					catch (IOException)
					{
						return null;
					}
				}
			}
		}

		/// <summary>
		/// Returns a map of X.509 extensions containing additional information
		/// about the revoked certificate, such as the Invalidity Date
		/// Extension. Each key is an OID String that maps to the corresponding
		/// Extension.
		/// </summary>
		/// <returns> an unmodifiable map of X.509 extensions, or an empty map
		///    if there are no extensions </returns>
		public virtual IDictionary<String, Extension> Extensions
		{
			get
			{
				return Collections.UnmodifiableMap(Extensions_Renamed);
			}
		}

		public override String Message
		{
			get
			{
				return "Certificate has been revoked, reason: " + Reason + ", revocation date: " + RevocationDate_Renamed + ", authority: " + Authority + ", extension OIDs: " + Extensions_Renamed.Keys;
			}
		}

		/// <summary>
		/// Serialize this {@code CertificateRevokedException} instance.
		/// 
		/// @serialData the size of the extensions map (int), followed by all of
		/// the extensions in the map, in no particular order. For each extension,
		/// the following data is emitted: the OID String (Object), the criticality
		/// flag (boolean), the length of the encoded extension value byte array
		/// (int), and the encoded extension value bytes.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException
		private void WriteObject(ObjectOutputStream oos)
		{
			// Write out the non-transient fields
			// (revocationDate, reason, authority)
			oos.DefaultWriteObject();

			// Write out the size (number of mappings) of the extensions map
			oos.WriteInt(Extensions_Renamed.Count);

			// For each extension in the map, the following are emitted (in order):
			// the OID String (Object), the criticality flag (boolean), the length
			// of the encoded extension value byte array (int), and the encoded
			// extension value byte array. The extensions themselves are emitted
			// in no particular order.
			foreach (java.util.Map_Entry<String, Extension> entry in Extensions_Renamed)
			{
				Extension ext = entry.Value;
				oos.WriteObject(ext.Id);
				oos.WriteBoolean(ext.Critical);
				sbyte[] extVal = ext.Value;
				oos.WriteInt(extVal.Length);
				oos.Write(extVal);
			}
		}

		/// <summary>
		/// Deserialize the {@code CertificateRevokedException} instance.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream ois)
		{
			// Read in the non-transient fields
			// (revocationDate, reason, authority)
			ois.DefaultReadObject();

			// Defensively copy the revocation date
			RevocationDate_Renamed = new DateTime(RevocationDate_Renamed.Ticks);

			// Read in the size (number of mappings) of the extensions map
			// and create the extensions map
			int size = ois.ReadInt();
			if (size == 0)
			{
				Extensions_Renamed = Collections.EmptyMap();
			}
			else
			{
				Extensions_Renamed = new Dictionary<String, Extension>(size);
			}

			// Read in the extensions and put the mappings in the extensions map
			for (int i = 0; i < size; i++)
			{
				String oid = (String) ois.ReadObject();
				bool critical = ois.ReadBoolean();
				int length = ois.ReadInt();
				sbyte[] extVal = new sbyte[length];
				ois.ReadFully(extVal);
				Extension ext = sun.security.x509.Extension.newExtension(new ObjectIdentifier(oid), critical, extVal);
				Extensions_Renamed[oid] = ext;
			}
		}
	}

}