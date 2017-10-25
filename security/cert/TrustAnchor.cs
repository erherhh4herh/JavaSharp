using System;

/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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


	using NameConstraintsExtension = sun.security.x509.NameConstraintsExtension;
	using X500Name = sun.security.x509.X500Name;

	/// <summary>
	/// A trust anchor or most-trusted Certification Authority (CA).
	/// <para>
	/// This class represents a "most-trusted CA", which is used as a trust anchor
	/// for validating X.509 certification paths. A most-trusted CA includes the
	/// public key of the CA, the CA's name, and any constraints upon the set of
	/// paths which may be validated using this key. These parameters can be
	/// specified in the form of a trusted {@code X509Certificate} or as
	/// individual parameters.
	/// </para>
	/// <para>
	/// <b>Concurrent Access</b>
	/// </para>
	/// <para>All {@code TrustAnchor} objects must be immutable and
	/// thread-safe. That is, multiple threads may concurrently invoke the
	/// methods defined in this class on a single {@code TrustAnchor}
	/// object (or more than one) with no ill effects. Requiring
	/// {@code TrustAnchor} objects to be immutable and thread-safe
	/// allows them to be passed around to various pieces of code without
	/// worrying about coordinating access. This stipulation applies to all
	/// public fields and methods of this class and any added or overridden
	/// by subclasses.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= PKIXParameters#PKIXParameters(Set) </seealso>
	/// <seealso cref= PKIXBuilderParameters#PKIXBuilderParameters(Set, CertSelector)
	/// 
	/// @since       1.4
	/// @author      Sean Mullan </seealso>
	public class TrustAnchor
	{

		private readonly PublicKey PubKey;
		private readonly String CaName;
		private readonly X500Principal CaPrincipal;
		private readonly X509Certificate TrustedCert_Renamed;
		private sbyte[] NcBytes;
		private NameConstraintsExtension Nc;

		/// <summary>
		/// Creates an instance of {@code TrustAnchor} with the specified
		/// {@code X509Certificate} and optional name constraints, which
		/// are intended to be used as additional constraints when validating
		/// an X.509 certification path.
		/// <para>
		/// The name constraints are specified as a byte array. This byte array
		/// should contain the DER encoded form of the name constraints, as they
		/// would appear in the NameConstraints structure defined in
		/// <a href="http://www.ietf.org/rfc/rfc3280">RFC 3280</a>
		/// and X.509. The ASN.1 definition of this structure appears below.
		/// 
		/// <pre>{@code
		///  NameConstraints ::= SEQUENCE {
		///       permittedSubtrees       [0]     GeneralSubtrees OPTIONAL,
		///       excludedSubtrees        [1]     GeneralSubtrees OPTIONAL }
		/// 
		///  GeneralSubtrees ::= SEQUENCE SIZE (1..MAX) OF GeneralSubtree
		/// 
		///  GeneralSubtree ::= SEQUENCE {
		///       base                    GeneralName,
		///       minimum         [0]     BaseDistance DEFAULT 0,
		///       maximum         [1]     BaseDistance OPTIONAL }
		/// 
		///  BaseDistance ::= INTEGER (0..MAX)
		/// 
		///  GeneralName ::= CHOICE {
		///       otherName                       [0]     OtherName,
		///       rfc822Name                      [1]     IA5String,
		///       dNSName                         [2]     IA5String,
		///       x400Address                     [3]     ORAddress,
		///       directoryName                   [4]     Name,
		///       ediPartyName                    [5]     EDIPartyName,
		///       uniformResourceIdentifier       [6]     IA5String,
		///       iPAddress                       [7]     OCTET STRING,
		///       registeredID                    [8]     OBJECT IDENTIFIER}
		/// }</pre>
		/// </para>
		/// <para>
		/// Note that the name constraints byte array supplied is cloned to protect
		/// against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="trustedCert"> a trusted {@code X509Certificate} </param>
		/// <param name="nameConstraints"> a byte array containing the ASN.1 DER encoding of
		/// a NameConstraints extension to be used for checking name constraints.
		/// Only the value of the extension is included, not the OID or criticality
		/// flag. Specify {@code null} to omit the parameter. </param>
		/// <exception cref="IllegalArgumentException"> if the name constraints cannot be
		/// decoded </exception>
		/// <exception cref="NullPointerException"> if the specified
		/// {@code X509Certificate} is {@code null} </exception>
		public TrustAnchor(X509Certificate trustedCert, sbyte[] nameConstraints)
		{
			if (trustedCert == null)
			{
				throw new NullPointerException("the trustedCert parameter must " + "be non-null");
			}
			this.TrustedCert_Renamed = trustedCert;
			this.PubKey = null;
			this.CaName = null;
			this.CaPrincipal = null;
			NameConstraints = nameConstraints;
		}

		/// <summary>
		/// Creates an instance of {@code TrustAnchor} where the
		/// most-trusted CA is specified as an X500Principal and public key.
		/// Name constraints are an optional parameter, and are intended to be used
		/// as additional constraints when validating an X.509 certification path.
		/// <para>
		/// The name constraints are specified as a byte array. This byte array
		/// contains the DER encoded form of the name constraints, as they
		/// would appear in the NameConstraints structure defined in RFC 3280
		/// and X.509. The ASN.1 notation for this structure is supplied in the
		/// documentation for
		/// {@link #TrustAnchor(X509Certificate, byte[])
		/// TrustAnchor(X509Certificate trustedCert, byte[] nameConstraints) }.
		/// </para>
		/// <para>
		/// Note that the name constraints byte array supplied here is cloned to
		/// protect against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="caPrincipal"> the name of the most-trusted CA as X500Principal </param>
		/// <param name="pubKey"> the public key of the most-trusted CA </param>
		/// <param name="nameConstraints"> a byte array containing the ASN.1 DER encoding of
		/// a NameConstraints extension to be used for checking name constraints.
		/// Only the value of the extension is included, not the OID or criticality
		/// flag. Specify {@code null} to omit the parameter. </param>
		/// <exception cref="NullPointerException"> if the specified {@code caPrincipal} or
		/// {@code pubKey} parameter is {@code null}
		/// @since 1.5 </exception>
		public TrustAnchor(X500Principal caPrincipal, PublicKey pubKey, sbyte[] nameConstraints)
		{
			if ((caPrincipal == null) || (pubKey == null))
			{
				throw new NullPointerException();
			}
			this.TrustedCert_Renamed = null;
			this.CaPrincipal = caPrincipal;
			this.CaName = caPrincipal.Name;
			this.PubKey = pubKey;
			NameConstraints = nameConstraints;
		}

		/// <summary>
		/// Creates an instance of {@code TrustAnchor} where the
		/// most-trusted CA is specified as a distinguished name and public key.
		/// Name constraints are an optional parameter, and are intended to be used
		/// as additional constraints when validating an X.509 certification path.
		/// <para>
		/// The name constraints are specified as a byte array. This byte array
		/// contains the DER encoded form of the name constraints, as they
		/// would appear in the NameConstraints structure defined in RFC 3280
		/// and X.509. The ASN.1 notation for this structure is supplied in the
		/// documentation for
		/// {@link #TrustAnchor(X509Certificate, byte[])
		/// TrustAnchor(X509Certificate trustedCert, byte[] nameConstraints) }.
		/// </para>
		/// <para>
		/// Note that the name constraints byte array supplied here is cloned to
		/// protect against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="caName"> the X.500 distinguished name of the most-trusted CA in
		/// <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>
		/// {@code String} format </param>
		/// <param name="pubKey"> the public key of the most-trusted CA </param>
		/// <param name="nameConstraints"> a byte array containing the ASN.1 DER encoding of
		/// a NameConstraints extension to be used for checking name constraints.
		/// Only the value of the extension is included, not the OID or criticality
		/// flag. Specify {@code null} to omit the parameter. </param>
		/// <exception cref="IllegalArgumentException"> if the specified
		/// {@code caName} parameter is empty {@code (caName.length() == 0)}
		/// or incorrectly formatted or the name constraints cannot be decoded </exception>
		/// <exception cref="NullPointerException"> if the specified {@code caName} or
		/// {@code pubKey} parameter is {@code null} </exception>
		public TrustAnchor(String caName, PublicKey pubKey, sbyte[] nameConstraints)
		{
			if (pubKey == null)
			{
				throw new NullPointerException("the pubKey parameter must be " + "non-null");
			}
			if (caName == null)
			{
				throw new NullPointerException("the caName parameter must be " + "non-null");
			}
			if (caName.Length() == 0)
			{
				throw new IllegalArgumentException("the caName " + "parameter must be a non-empty String");
			}
			// check if caName is formatted correctly
			this.CaPrincipal = new X500Principal(caName);
			this.PubKey = pubKey;
			this.CaName = caName;
			this.TrustedCert_Renamed = null;
			NameConstraints = nameConstraints;
		}

		/// <summary>
		/// Returns the most-trusted CA certificate.
		/// </summary>
		/// <returns> a trusted {@code X509Certificate} or {@code null}
		/// if the trust anchor was not specified as a trusted certificate </returns>
		public X509Certificate TrustedCert
		{
			get
			{
				return this.TrustedCert_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the most-trusted CA as an X500Principal.
		/// </summary>
		/// <returns> the X.500 distinguished name of the most-trusted CA, or
		/// {@code null} if the trust anchor was not specified as a trusted
		/// public key and name or X500Principal pair
		/// @since 1.5 </returns>
		public X500Principal CA
		{
			get
			{
				return this.CaPrincipal;
			}
		}

		/// <summary>
		/// Returns the name of the most-trusted CA in RFC 2253 {@code String}
		/// format.
		/// </summary>
		/// <returns> the X.500 distinguished name of the most-trusted CA, or
		/// {@code null} if the trust anchor was not specified as a trusted
		/// public key and name or X500Principal pair </returns>
		public String CAName
		{
			get
			{
				return this.CaName;
			}
		}

		/// <summary>
		/// Returns the public key of the most-trusted CA.
		/// </summary>
		/// <returns> the public key of the most-trusted CA, or {@code null}
		/// if the trust anchor was not specified as a trusted public key and name
		/// or X500Principal pair </returns>
		public PublicKey CAPublicKey
		{
			get
			{
				return this.PubKey;
			}
		}

		/// <summary>
		/// Decode the name constraints and clone them if not null.
		/// </summary>
		private sbyte[] NameConstraints
		{
			set
			{
				if (value == null)
				{
					NcBytes = null;
					Nc = null;
				}
				else
				{
					NcBytes = value.clone();
					// validate DER encoding
					try
					{
						Nc = new NameConstraintsExtension(false, value);
					}
					catch (IOException ioe)
					{
						IllegalArgumentException iae = new IllegalArgumentException(ioe.Message);
						iae.InitCause(ioe);
						throw iae;
					}
				}
			}
			get
			{
				return NcBytes == null ? null : NcBytes.clone();
			}
		}


		/// <summary>
		/// Returns a formatted string describing the {@code TrustAnchor}.
		/// </summary>
		/// <returns> a formatted string describing the {@code TrustAnchor} </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("[\n");
			if (PubKey != null)
			{
				sb.Append("  Trusted CA Public Key: " + PubKey.ToString() + "\n");
				sb.Append("  Trusted CA Issuer Name: " + Convert.ToString(CaName) + "\n");
			}
			else
			{
				sb.Append("  Trusted CA cert: " + TrustedCert_Renamed.ToString() + "\n");
			}
			if (Nc != null)
			{
				sb.Append("  Name Constraints: " + Nc.ToString() + "\n");
			}
			return sb.ToString();
		}
	}

}