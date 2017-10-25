using System;

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

namespace java.security.cert
{


	using X509CRLEntryImpl = sun.security.x509.X509CRLEntryImpl;

	/// <summary>
	/// <para>Abstract class for a revoked certificate in a CRL (Certificate
	/// Revocation List).
	/// 
	/// The ASN.1 definition for <em>revokedCertificates</em> is:
	/// <pre>
	/// revokedCertificates    SEQUENCE OF SEQUENCE  {
	///     userCertificate    CertificateSerialNumber,
	///     revocationDate     ChoiceOfTime,
	///     crlEntryExtensions Extensions OPTIONAL
	///                        -- if present, must be v2
	/// }  OPTIONAL
	/// 
	/// CertificateSerialNumber  ::=  INTEGER
	/// 
	/// Extensions  ::=  SEQUENCE SIZE (1..MAX) OF Extension
	/// 
	/// Extension  ::=  SEQUENCE  {
	///     extnId        OBJECT IDENTIFIER,
	///     critical      BOOLEAN DEFAULT FALSE,
	///     extnValue     OCTET STRING
	///                   -- contains a DER encoding of a value
	///                   -- of the type registered for use with
	///                   -- the extnId object identifier value
	/// }
	/// </pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= X509CRL </seealso>
	/// <seealso cref= X509Extension
	/// 
	/// @author Hemma Prafullchandra </seealso>

	public abstract class X509CRLEntry : X509Extension
	{
		public abstract sbyte[] GetExtensionValue(string oid);
		public abstract java.util.Set<String> NonCriticalExtensionOIDs {get;}
		public abstract java.util.Set<String> CriticalExtensionOIDs {get;}
		public abstract bool HasUnsupportedCriticalExtension();

		/// <summary>
		/// Compares this CRL entry for equality with the given
		/// object. If the {@code other} object is an
		/// {@code instanceof} {@code X509CRLEntry}, then
		/// its encoded form (the inner SEQUENCE) is retrieved and compared
		/// with the encoded form of this CRL entry.
		/// </summary>
		/// <param name="other"> the object to test for equality with this CRL entry. </param>
		/// <returns> true iff the encoded forms of the two CRL entries
		/// match, false otherwise. </returns>
		public override bool Equals(Object other)
		{
			if (this == other)
			{
				return true;
			}
			if (!(other is X509CRLEntry))
			{
				return false;
			}
			try
			{
				sbyte[] thisCRLEntry = this.Encoded;
				sbyte[] otherCRLEntry = ((X509CRLEntry)other).Encoded;

				if (thisCRLEntry.Length != otherCRLEntry.Length)
				{
					return false;
				}
				for (int i = 0; i < thisCRLEntry.Length; i++)
				{
					 if (thisCRLEntry[i] != otherCRLEntry[i])
					 {
						 return false;
					 }
				}
			}
			catch (CRLException)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Returns a hashcode value for this CRL entry from its
		/// encoded form.
		/// </summary>
		/// <returns> the hashcode value. </returns>
		public override int HashCode()
		{
			int retval = 0;
			try
			{
				sbyte[] entryData = this.Encoded;
				for (int i = 1; i < entryData.Length; i++)
				{
					 retval += entryData[i] * i;
				}

			}
			catch (CRLException)
			{
				return (retval);
			}
			return (retval);
		}

		/// <summary>
		/// Returns the ASN.1 DER-encoded form of this CRL Entry,
		/// that is the inner SEQUENCE.
		/// </summary>
		/// <returns> the encoded form of this certificate </returns>
		/// <exception cref="CRLException"> if an encoding error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract byte[] getEncoded() throws CRLException;
		public abstract sbyte[] Encoded {get;}

		/// <summary>
		/// Gets the serial number from this X509CRLEntry,
		/// the <em>userCertificate</em>.
		/// </summary>
		/// <returns> the serial number. </returns>
		public abstract System.Numerics.BigInteger SerialNumber {get;}

		/// <summary>
		/// Get the issuer of the X509Certificate described by this entry. If
		/// the certificate issuer is also the CRL issuer, this method returns
		/// null.
		/// 
		/// <para>This method is used with indirect CRLs. The default implementation
		/// always returns null. Subclasses that wish to support indirect CRLs
		/// should override it.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the issuer of the X509Certificate described by this entry
		/// or null if it is issued by the CRL issuer.
		/// 
		/// @since 1.5 </returns>
		public virtual X500Principal CertificateIssuer
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the revocation date from this X509CRLEntry,
		/// the <em>revocationDate</em>.
		/// </summary>
		/// <returns> the revocation date. </returns>
		public abstract DateTime RevocationDate {get;}

		/// <summary>
		/// Returns true if this CRL entry has extensions.
		/// </summary>
		/// <returns> true if this entry has extensions, false otherwise. </returns>
		public abstract bool HasExtensions();

		/// <summary>
		/// Returns a string representation of this CRL entry.
		/// </summary>
		/// <returns> a string representation of this CRL entry. </returns>
		public override abstract String ToString();

		/// <summary>
		/// Returns the reason the certificate has been revoked, as specified
		/// in the Reason Code extension of this CRL entry.
		/// </summary>
		/// <returns> the reason the certificate has been revoked, or
		///    {@code null} if this CRL entry does not have
		///    a Reason Code extension
		/// @since 1.7 </returns>
		public virtual CRLReason RevocationReason
		{
			get
			{
				if (!HasExtensions())
				{
					return null;
				}
				return X509CRLEntryImpl.getRevocationReason(this);
			}
		}
	}

}