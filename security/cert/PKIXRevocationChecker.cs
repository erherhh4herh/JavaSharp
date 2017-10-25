using System.Collections.Generic;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// A {@code PKIXCertPathChecker} for checking the revocation status of
	/// certificates with the PKIX algorithm.
	/// 
	/// <para>A {@code PKIXRevocationChecker} checks the revocation status of
	/// certificates with the Online Certificate Status Protocol (OCSP) or
	/// Certificate Revocation Lists (CRLs). OCSP is described in RFC 2560 and
	/// is a network protocol for determining the status of a certificate. A CRL
	/// is a time-stamped list identifying revoked certificates, and RFC 5280
	/// describes an algorithm for determining the revocation status of certificates
	/// using CRLs.
	/// 
	/// </para>
	/// <para>Each {@code PKIXRevocationChecker} must be able to check the revocation
	/// status of certificates with OCSP and CRLs. By default, OCSP is the
	/// preferred mechanism for checking revocation status, with CRLs as the
	/// fallback mechanism. However, this preference can be switched to CRLs with
	/// the <seealso cref="Option#PREFER_CRLS PREFER_CRLS"/> option. In addition, the fallback
	/// mechanism can be disabled with the <seealso cref="Option#NO_FALLBACK NO_FALLBACK"/>
	/// option.
	/// 
	/// </para>
	/// <para>A {@code PKIXRevocationChecker} is obtained by calling the
	/// <seealso cref="CertPathValidator#getRevocationChecker getRevocationChecker"/> method
	/// of a PKIX {@code CertPathValidator}. Additional parameters and options
	/// specific to revocation can be set (by calling the
	/// <seealso cref="#setOcspResponder setOcspResponder"/> method for instance). The
	/// {@code PKIXRevocationChecker} is added to a {@code PKIXParameters} object
	/// using the <seealso cref="PKIXParameters#addCertPathChecker addCertPathChecker"/>
	/// or <seealso cref="PKIXParameters#setCertPathCheckers setCertPathCheckers"/> method,
	/// and then the {@code PKIXParameters} is passed along with the {@code CertPath}
	/// to be validated to the <seealso cref="CertPathValidator#validate validate"/> method
	/// of a PKIX {@code CertPathValidator}. When supplying a revocation checker in
	/// this manner, it will be used to check revocation irrespective of the setting
	/// of the <seealso cref="PKIXParameters#isRevocationEnabled RevocationEnabled"/> flag.
	/// Similarly, a {@code PKIXRevocationChecker} may be added to a
	/// {@code PKIXBuilderParameters} object for use with a PKIX
	/// {@code CertPathBuilder}.
	/// 
	/// </para>
	/// <para>Note that when a {@code PKIXRevocationChecker} is added to
	/// {@code PKIXParameters}, it clones the {@code PKIXRevocationChecker};
	/// thus any subsequent modifications to the {@code PKIXRevocationChecker}
	/// have no effect.
	/// 
	/// </para>
	/// <para>Any parameter that is not set (or is set to {@code null}) will be set to
	/// the default value for that parameter.
	/// 
	/// </para>
	/// <para><b>Concurrent Access</b>
	/// 
	/// </para>
	/// <para>Unless otherwise specified, the methods defined in this class are not
	/// thread-safe. Multiple threads that need to access a single object
	/// concurrently should synchronize amongst themselves and provide the
	/// necessary locking. Multiple threads each manipulating separate objects
	/// need not synchronize.
	/// 
	/// @since 1.8
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc2560.txt"><i>RFC&nbsp;2560: X.509
	/// Internet Public Key Infrastructure Online Certificate Status Protocol -
	/// OCSP</i></a>, <br><a
	/// href="http://www.ietf.org/rfc/rfc5280.txt"><i>RFC&nbsp;5280: Internet X.509
	/// Public Key Infrastructure Certificate and Certificate Revocation List (CRL)
	/// Profile</i></a> </seealso>
	public abstract class PKIXRevocationChecker : PKIXCertPathChecker
	{
		private URI OcspResponder_Renamed;
		private X509Certificate OcspResponderCert_Renamed;
		private IList<Extension> OcspExtensions_Renamed = System.Linq.Enumerable.Empty<Extension>();
		private IDictionary<X509Certificate, sbyte[]> OcspResponses_Renamed = Collections.EmptyMap();
		private Set<Option> Options_Renamed = Collections.EmptySet();

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected internal PKIXRevocationChecker()
		{
		}

		/// <summary>
		/// Sets the URI that identifies the location of the OCSP responder. This
		/// overrides the {@code ocsp.responderURL} security property and any
		/// responder specified in a certificate's Authority Information Access
		/// Extension, as defined in RFC 5280.
		/// </summary>
		/// <param name="uri"> the responder URI </param>
		public virtual URI OcspResponder
		{
			set
			{
				this.OcspResponder_Renamed = value;
			}
			get
			{
				return OcspResponder_Renamed;
			}
		}


		/// <summary>
		/// Sets the OCSP responder's certificate. This overrides the
		/// {@code ocsp.responderCertSubjectName},
		/// {@code ocsp.responderCertIssuerName},
		/// and {@code ocsp.responderCertSerialNumber} security properties.
		/// </summary>
		/// <param name="cert"> the responder's certificate </param>
		public virtual X509Certificate OcspResponderCert
		{
			set
			{
				this.OcspResponderCert_Renamed = value;
			}
			get
			{
				return OcspResponderCert_Renamed;
			}
		}


		// request extensions; single extensions not supported
		/// <summary>
		/// Sets the optional OCSP request extensions.
		/// </summary>
		/// <param name="extensions"> a list of extensions. The list is copied to protect
		///        against subsequent modification. </param>
		public virtual IList<Extension> OcspExtensions
		{
			set
			{
				this.OcspExtensions_Renamed = (value == null) ? System.Linq.Enumerable.Empty<Extension>() : new List<Extension>(value);
			}
			get
			{
				return Collections.UnmodifiableList(OcspExtensions_Renamed);
			}
		}


		/// <summary>
		/// Sets the OCSP responses. These responses are used to determine
		/// the revocation status of the specified certificates when OCSP is used.
		/// </summary>
		/// <param name="responses"> a map of OCSP responses. Each key is an
		///        {@code X509Certificate} that maps to the corresponding
		///        DER-encoded OCSP response for that certificate. A deep copy of
		///        the map is performed to protect against subsequent modification. </param>
		public virtual IDictionary<X509Certificate, sbyte[]> OcspResponses
		{
			set
			{
				if (value == null)
				{
					this.OcspResponses_Renamed = System.Linq.Enumerable.Empty<X509Certificate, sbyte[]>();
				}
				else
				{
					IDictionary<X509Certificate, sbyte[]> copy = new Dictionary<X509Certificate, sbyte[]>(value.Count);
					foreach (Map_Entry<X509Certificate, sbyte[]> e in value)
					{
						copy[e.Key] = e.Value.clone();
					}
					this.OcspResponses_Renamed = copy;
				}
			}
			get
			{
				IDictionary<X509Certificate, sbyte[]> copy = new Dictionary<X509Certificate, sbyte[]>(OcspResponses_Renamed.Count);
				foreach (Map_Entry<X509Certificate, sbyte[]> e in OcspResponses_Renamed)
				{
					copy[e.Key] = e.Value.clone();
				}
				return copy;
			}
		}


		/// <summary>
		/// Sets the revocation options.
		/// </summary>
		/// <param name="options"> a set of revocation options. The set is copied to protect
		///        against subsequent modification. </param>
		public virtual Set<Option> Options
		{
			set
			{
				this.Options_Renamed = (value == null) ? System.Linq.Enumerable.Empty<Option>() : new HashSet<Option>(value);
			}
			get
			{
				return Collections.UnmodifiableSet(Options_Renamed);
			}
		}


		/// <summary>
		/// Returns a list containing the exceptions that are ignored by the
		/// revocation checker when the <seealso cref="Option#SOFT_FAIL SOFT_FAIL"/> option
		/// is set. The list is cleared each time <seealso cref="#init init"/> is called.
		/// The list is ordered in ascending order according to the certificate
		/// index returned by <seealso cref="CertPathValidatorException#getIndex getIndex"/>
		/// method of each entry.
		/// <para>
		/// An implementation of {@code PKIXRevocationChecker} is responsible for
		/// adding the ignored exceptions to the list.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an unmodifiable list containing the ignored exceptions. The list
		///         is empty if no exceptions have been ignored. </returns>
		public abstract IList<CertPathValidatorException> SoftFailExceptions {get;}

		public override PKIXRevocationChecker Clone()
		{
			PKIXRevocationChecker copy = (PKIXRevocationChecker)base.Clone();
			copy.OcspExtensions_Renamed = new List<>(OcspExtensions_Renamed);
			copy.OcspResponses_Renamed = new Dictionary<>(OcspResponses_Renamed);
			// deep-copy the encoded responses, since they are mutable
			foreach (Map_Entry<X509Certificate, sbyte[]> entry in copy.OcspResponses_Renamed)
			{
				sbyte[] encoded = entry.Value;
				entry.Value = encoded.clone();
			}
			copy.Options_Renamed = new HashSet<>(Options_Renamed);
			return copy;
		}

		/// <summary>
		/// Various revocation options that can be specified for the revocation
		/// checking mechanism.
		/// </summary>
		public enum Option
		{
			/// <summary>
			/// Only check the revocation status of end-entity certificates.
			/// </summary>
			ONLY_END_ENTITY,
			/// <summary>
			/// Prefer CRLs to OSCP. The default behavior is to prefer OCSP. Each
			/// PKIX implementation should document further details of their
			/// specific preference rules and fallback policies.
			/// </summary>
			PREFER_CRLS,
			/// <summary>
			/// Disable the fallback mechanism.
			/// </summary>
			NO_FALLBACK,
			/// <summary>
			/// Allow revocation check to succeed if the revocation status cannot be
			/// determined for one of the following reasons:
			/// <ul>
			///  <li>The CRL or OCSP response cannot be obtained because of a
			///      network error.
			///  <li>The OCSP responder returns one of the following errors
			///      specified in section 2.3 of RFC 2560: internalError or tryLater.
			/// </ul><br>
			/// Note that these conditions apply to both OCSP and CRLs, and unless
			/// the {@code NO_FALLBACK} option is set, the revocation check is
			/// allowed to succeed only if both mechanisms fail under one of the
			/// conditions as stated above.
			/// Exceptions that cause the network errors are ignored but can be
			/// later retrieved by calling the
			/// <seealso cref="#getSoftFailExceptions getSoftFailExceptions"/> method.
			/// </summary>
			SOFT_FAIL
		}
	}

}