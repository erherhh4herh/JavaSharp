using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Parameters used as input for the PKIX {@code CertPathValidator}
	/// algorithm.
	/// <para>
	/// A PKIX {@code CertPathValidator} uses these parameters to
	/// validate a {@code CertPath} according to the PKIX certification path
	/// validation algorithm.
	/// 
	/// </para>
	/// <para>To instantiate a {@code PKIXParameters} object, an
	/// application must specify one or more <i>most-trusted CAs</i> as defined by
	/// the PKIX certification path validation algorithm. The most-trusted CAs
	/// can be specified using one of two constructors. An application
	/// can call <seealso cref="#PKIXParameters(Set) PKIXParameters(Set)"/>,
	/// specifying a {@code Set} of {@code TrustAnchor} objects, each
	/// of which identify a most-trusted CA. Alternatively, an application can call
	/// <seealso cref="#PKIXParameters(KeyStore) PKIXParameters(KeyStore)"/>, specifying a
	/// {@code KeyStore} instance containing trusted certificate entries, each
	/// of which will be considered as a most-trusted CA.
	/// </para>
	/// <para>
	/// Once a {@code PKIXParameters} object has been created, other parameters
	/// can be specified (by calling <seealso cref="#setInitialPolicies setInitialPolicies"/>
	/// or <seealso cref="#setDate setDate"/>, for instance) and then the
	/// {@code PKIXParameters} is passed along with the {@code CertPath}
	/// to be validated to {@link CertPathValidator#validate
	/// CertPathValidator.validate}.
	/// </para>
	/// <para>
	/// Any parameter that is not set (or is set to {@code null}) will
	/// be set to the default value for that parameter. The default value for the
	/// {@code date} parameter is {@code null}, which indicates
	/// the current time when the path is validated. The default for the
	/// remaining parameters is the least constrained.
	/// </para>
	/// <para>
	/// <b>Concurrent Access</b>
	/// </para>
	/// <para>
	/// Unless otherwise specified, the methods defined in this class are not
	/// thread-safe. Multiple threads that need to access a single
	/// object concurrently should synchronize amongst themselves and
	/// provide the necessary locking. Multiple threads each manipulating
	/// separate objects need not synchronize.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CertPathValidator
	/// 
	/// @since       1.4
	/// @author      Sean Mullan
	/// @author      Yassir Elley </seealso>
	public class PKIXParameters : CertPathParameters
	{

		private Set<TrustAnchor> UnmodTrustAnchors;
		private DateTime Date_Renamed;
		private IList<PKIXCertPathChecker> CertPathCheckers_Renamed;
		private String SigProvider_Renamed;
		private bool RevocationEnabled_Renamed = true;
		private Set<String> UnmodInitialPolicies;
		private bool ExplicitPolicyRequired_Renamed = false;
		private bool PolicyMappingInhibited_Renamed = false;
		private bool AnyPolicyInhibited_Renamed = false;
		private bool PolicyQualifiersRejected_Renamed = true;
		private IList<CertStore> CertStores_Renamed;
		private CertSelector CertSelector;

		/// <summary>
		/// Creates an instance of {@code PKIXParameters} with the specified
		/// {@code Set} of most-trusted CAs. Each element of the
		/// set is a <seealso cref="TrustAnchor TrustAnchor"/>.
		/// <para>
		/// Note that the {@code Set} is copied to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="trustAnchors"> a {@code Set} of {@code TrustAnchor}s </param>
		/// <exception cref="InvalidAlgorithmParameterException"> if the specified
		/// {@code Set} is empty {@code (trustAnchors.isEmpty() == true)} </exception>
		/// <exception cref="NullPointerException"> if the specified {@code Set} is
		/// {@code null} </exception>
		/// <exception cref="ClassCastException"> if any of the elements in the {@code Set}
		/// are not of type {@code java.security.cert.TrustAnchor} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PKIXParameters(java.util.Set<TrustAnchor> trustAnchors) throws java.security.InvalidAlgorithmParameterException
		public PKIXParameters(Set<TrustAnchor> trustAnchors)
		{
			TrustAnchors = trustAnchors;

			this.UnmodInitialPolicies = System.Linq.Enumerable.Empty<String>();
			this.CertPathCheckers_Renamed = new List<PKIXCertPathChecker>();
			this.CertStores_Renamed = new List<CertStore>();
		}

		/// <summary>
		/// Creates an instance of {@code PKIXParameters} that
		/// populates the set of most-trusted CAs from the trusted
		/// certificate entries contained in the specified {@code KeyStore}.
		/// Only keystore entries that contain trusted {@code X509Certificates}
		/// are considered; all other certificate types are ignored.
		/// </summary>
		/// <param name="keystore"> a {@code KeyStore} from which the set of
		/// most-trusted CAs will be populated </param>
		/// <exception cref="KeyStoreException"> if the keystore has not been initialized </exception>
		/// <exception cref="InvalidAlgorithmParameterException"> if the keystore does
		/// not contain at least one trusted certificate entry </exception>
		/// <exception cref="NullPointerException"> if the keystore is {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PKIXParameters(java.security.KeyStore keystore) throws java.security.KeyStoreException, java.security.InvalidAlgorithmParameterException
		public PKIXParameters(KeyStore keystore)
		{
			if (keystore == null)
			{
				throw new NullPointerException("the keystore parameter must be " + "non-null");
			}
			Set<TrustAnchor> hashSet = new HashSet<TrustAnchor>();
			IEnumerator<String> aliases = keystore.Aliases();
			while (aliases.MoveNext())
			{
				String alias = aliases.Current;
				if (keystore.IsCertificateEntry(alias))
				{
					Certificate cert = keystore.GetCertificate(alias);
					if (cert is X509Certificate)
					{
						hashSet.Add(new TrustAnchor((X509Certificate)cert, null));
					}
				}
			}
			TrustAnchors = hashSet;
			this.UnmodInitialPolicies = System.Linq.Enumerable.Empty<String>();
			this.CertPathCheckers_Renamed = new List<PKIXCertPathChecker>();
			this.CertStores_Renamed = new List<CertStore>();
		}

		/// <summary>
		/// Returns an immutable {@code Set} of the most-trusted
		/// CAs.
		/// </summary>
		/// <returns> an immutable {@code Set} of {@code TrustAnchor}s
		/// (never {@code null})
		/// </returns>
		/// <seealso cref= #setTrustAnchors </seealso>
		public virtual Set<TrustAnchor> TrustAnchors
		{
			get
			{
				return this.UnmodTrustAnchors;
			}
			set
			{
				if (value == null)
				{
					throw new NullPointerException("the trustAnchors parameters must" + " be non-null");
				}
				if (value.Count == 0)
				{
					throw new InvalidAlgorithmParameterException("the trustAnchors " + "parameter must be non-empty");
				}
				for (IEnumerator<TrustAnchor> i = value.Iterator(); i.MoveNext();)
				{
					if (!(i.Current is TrustAnchor))
					{
						throw new ClassCastException("all elements of set must be " + "of type java.security.cert.TrustAnchor");
					}
				}
				this.UnmodTrustAnchors = Collections.UnmodifiableSet(new HashSet<TrustAnchor>(value));
			}
		}


		/// <summary>
		/// Returns an immutable {@code Set} of initial
		/// policy identifiers (OID strings), indicating that any one of these
		/// policies would be acceptable to the certificate user for the purposes of
		/// certification path processing. The default return value is an empty
		/// {@code Set}, which is interpreted as meaning that any policy would
		/// be acceptable.
		/// </summary>
		/// <returns> an immutable {@code Set} of initial policy OIDs in
		/// {@code String} format, or an empty {@code Set} (implying any
		/// policy is acceptable). Never returns {@code null}.
		/// </returns>
		/// <seealso cref= #setInitialPolicies </seealso>
		public virtual Set<String> InitialPolicies
		{
			get
			{
				return this.UnmodInitialPolicies;
			}
			set
			{
				if (value != null)
				{
					for (IEnumerator<String> i = value.Iterator(); i.MoveNext();)
					{
						if (!(i.Current is String))
						{
							throw new ClassCastException("all elements of set must be " + "of type java.lang.String");
						}
					}
					this.UnmodInitialPolicies = Collections.UnmodifiableSet(new HashSet<String>(value));
				}
				else
				{
					this.UnmodInitialPolicies = System.Linq.Enumerable.Empty<String>();
				}
			}
		}


		/// <summary>
		/// Sets the list of {@code CertStore}s to be used in finding
		/// certificates and CRLs. May be {@code null}, in which case
		/// no {@code CertStore}s will be used. The first
		/// {@code CertStore}s in the list may be preferred to those that
		/// appear later.
		/// <para>
		/// Note that the {@code List} is copied to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="stores"> a {@code List} of {@code CertStore}s (or
		/// {@code null}) </param>
		/// <exception cref="ClassCastException"> if any of the elements in the list are
		/// not of type {@code java.security.cert.CertStore}
		/// </exception>
		/// <seealso cref= #getCertStores </seealso>
		public virtual IList<CertStore> CertStores
		{
			set
			{
				if (value == null)
				{
					this.CertStores_Renamed = new List<CertStore>();
				}
				else
				{
					for (IEnumerator<CertStore> i = value.GetEnumerator(); i.MoveNext();)
					{
						if (!(i.Current is CertStore))
						{
							throw new ClassCastException("all elements of list must be " + "of type java.security.cert.CertStore");
						}
					}
					this.CertStores_Renamed = new List<CertStore>(value);
				}
			}
			get
			{
				return Collections.UnmodifiableList(new List<CertStore>(this.CertStores_Renamed));
			}
		}

		/// <summary>
		/// Adds a {@code CertStore} to the end of the list of
		/// {@code CertStore}s used in finding certificates and CRLs.
		/// </summary>
		/// <param name="store"> the {@code CertStore} to add. If {@code null},
		/// the store is ignored (not added to list). </param>
		public virtual void AddCertStore(CertStore store)
		{
			if (store != null)
			{
				this.CertStores_Renamed.Add(store);
			}
		}


		/// <summary>
		/// Sets the RevocationEnabled flag. If this flag is true, the default
		/// revocation checking mechanism of the underlying PKIX service provider
		/// will be used. If this flag is false, the default revocation checking
		/// mechanism will be disabled (not used).
		/// <para>
		/// When a {@code PKIXParameters} object is created, this flag is set
		/// to true. This setting reflects the most common strategy for checking
		/// revocation, since each service provider must support revocation
		/// checking to be PKIX compliant. Sophisticated applications should set
		/// this flag to false when it is not practical to use a PKIX service
		/// provider's default revocation checking mechanism or when an alternative
		/// revocation checking mechanism is to be substituted (by also calling the
		/// <seealso cref="#addCertPathChecker addCertPathChecker"/> or {@link
		/// #setCertPathCheckers setCertPathCheckers} methods).
		/// 
		/// </para>
		/// </summary>
		/// <param name="val"> the new value of the RevocationEnabled flag </param>
		public virtual bool RevocationEnabled
		{
			set
			{
				RevocationEnabled_Renamed = value;
			}
			get
			{
				return RevocationEnabled_Renamed;
			}
		}


		/// <summary>
		/// Sets the ExplicitPolicyRequired flag. If this flag is true, an
		/// acceptable policy needs to be explicitly identified in every certificate.
		/// By default, the ExplicitPolicyRequired flag is false.
		/// </summary>
		/// <param name="val"> {@code true} if explicit policy is to be required,
		/// {@code false} otherwise </param>
		public virtual bool ExplicitPolicyRequired
		{
			set
			{
				ExplicitPolicyRequired_Renamed = value;
			}
			get
			{
				return ExplicitPolicyRequired_Renamed;
			}
		}


		/// <summary>
		/// Sets the PolicyMappingInhibited flag. If this flag is true, policy
		/// mapping is inhibited. By default, policy mapping is not inhibited (the
		/// flag is false).
		/// </summary>
		/// <param name="val"> {@code true} if policy mapping is to be inhibited,
		/// {@code false} otherwise </param>
		public virtual bool PolicyMappingInhibited
		{
			set
			{
				PolicyMappingInhibited_Renamed = value;
			}
			get
			{
				return PolicyMappingInhibited_Renamed;
			}
		}


		/// <summary>
		/// Sets state to determine if the any policy OID should be processed
		/// if it is included in a certificate. By default, the any policy OID
		/// is not inhibited (<seealso cref="#isAnyPolicyInhibited isAnyPolicyInhibited()"/>
		/// returns {@code false}).
		/// </summary>
		/// <param name="val"> {@code true} if the any policy OID is to be
		/// inhibited, {@code false} otherwise </param>
		public virtual bool AnyPolicyInhibited
		{
			set
			{
				AnyPolicyInhibited_Renamed = value;
			}
			get
			{
				return AnyPolicyInhibited_Renamed;
			}
		}


		/// <summary>
		/// Sets the PolicyQualifiersRejected flag. If this flag is true,
		/// certificates that include policy qualifiers in a certificate
		/// policies extension that is marked critical are rejected.
		/// If the flag is false, certificates are not rejected on this basis.
		/// 
		/// <para> When a {@code PKIXParameters} object is created, this flag is
		/// set to true. This setting reflects the most common (and simplest)
		/// strategy for processing policy qualifiers. Applications that want to use
		/// a more sophisticated policy must set this flag to false.
		/// </para>
		/// <para>
		/// Note that the PKIX certification path validation algorithm specifies
		/// that any policy qualifier in a certificate policies extension that is
		/// marked critical must be processed and validated. Otherwise the
		/// certification path must be rejected. If the policyQualifiersRejected flag
		/// is set to false, it is up to the application to validate all policy
		/// qualifiers in this manner in order to be PKIX compliant.
		/// 
		/// </para>
		/// </summary>
		/// <param name="qualifiersRejected"> the new value of the PolicyQualifiersRejected
		/// flag </param>
		/// <seealso cref= #getPolicyQualifiersRejected </seealso>
		/// <seealso cref= PolicyQualifierInfo </seealso>
		public virtual bool PolicyQualifiersRejected
		{
			set
			{
				PolicyQualifiersRejected_Renamed = value;
			}
			get
			{
				return PolicyQualifiersRejected_Renamed;
			}
		}


		/// <summary>
		/// Returns the time for which the validity of the certification path
		/// should be determined. If {@code null}, the current time is used.
		/// <para>
		/// Note that the {@code Date} returned is copied to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the {@code Date}, or {@code null} if not set </returns>
		/// <seealso cref= #setDate </seealso>
		public virtual DateTime Date
		{
			get
			{
				if (Date_Renamed == null)
				{
					return null;
				}
				else
				{
					return (DateTime) this.Date_Renamed.Clone();
				}
			}
			set
			{
				if (value != null)
				{
					this.Date_Renamed = (DateTime) value.Clone();
				}
				else
				{
					value = null;
				}
			}
		}


		/// <summary>
		/// Sets a {@code List} of additional certification path checkers. If
		/// the specified {@code List} contains an object that is not a
		/// {@code PKIXCertPathChecker}, it is ignored.
		/// <para>
		/// Each {@code PKIXCertPathChecker} specified implements
		/// additional checks on a certificate. Typically, these are checks to
		/// process and verify private extensions contained in certificates.
		/// Each {@code PKIXCertPathChecker} should be instantiated with any
		/// initialization parameters needed to execute the check.
		/// </para>
		/// <para>
		/// This method allows sophisticated applications to extend a PKIX
		/// {@code CertPathValidator} or {@code CertPathBuilder}.
		/// Each of the specified {@code PKIXCertPathChecker}s will be called,
		/// in turn, by a PKIX {@code CertPathValidator} or
		/// {@code CertPathBuilder} for each certificate processed or
		/// validated.
		/// </para>
		/// <para>
		/// Regardless of whether these additional {@code PKIXCertPathChecker}s
		/// are set, a PKIX {@code CertPathValidator} or
		/// {@code CertPathBuilder} must perform all of the required PKIX
		/// checks on each certificate. The one exception to this rule is if the
		/// RevocationEnabled flag is set to false (see the {@link
		/// #setRevocationEnabled setRevocationEnabled} method).
		/// </para>
		/// <para>
		/// Note that the {@code List} supplied here is copied and each
		/// {@code PKIXCertPathChecker} in the list is cloned to protect
		/// against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="checkers"> a {@code List} of {@code PKIXCertPathChecker}s.
		/// May be {@code null}, in which case no additional checkers will be
		/// used. </param>
		/// <exception cref="ClassCastException"> if any of the elements in the list
		/// are not of type {@code java.security.cert.PKIXCertPathChecker} </exception>
		/// <seealso cref= #getCertPathCheckers </seealso>
		public virtual IList<PKIXCertPathChecker> CertPathCheckers
		{
			set
			{
				if (value != null)
				{
					IList<PKIXCertPathChecker> tmpList = new List<PKIXCertPathChecker>();
					foreach (PKIXCertPathChecker checker in value)
					{
						tmpList.Add((PKIXCertPathChecker)checker.Clone());
					}
					this.CertPathCheckers_Renamed = tmpList;
				}
				else
				{
					this.CertPathCheckers_Renamed = new List<PKIXCertPathChecker>();
				}
			}
			get
			{
				IList<PKIXCertPathChecker> tmpList = new List<PKIXCertPathChecker>();
				foreach (PKIXCertPathChecker ck in CertPathCheckers_Renamed)
				{
					tmpList.Add((PKIXCertPathChecker)ck.Clone());
				}
				return Collections.UnmodifiableList(tmpList);
			}
		}


		/// <summary>
		/// Adds a {@code PKIXCertPathChecker} to the list of certification
		/// path checkers. See the <seealso cref="#setCertPathCheckers setCertPathCheckers"/>
		/// method for more details.
		/// <para>
		/// Note that the {@code PKIXCertPathChecker} is cloned to protect
		/// against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="checker"> a {@code PKIXCertPathChecker} to add to the list of
		/// checks. If {@code null}, the checker is ignored (not added to list). </param>
		public virtual void AddCertPathChecker(PKIXCertPathChecker checker)
		{
			if (checker != null)
			{
				CertPathCheckers_Renamed.Add((PKIXCertPathChecker)checker.Clone());
			}
		}

		/// <summary>
		/// Returns the signature provider's name, or {@code null}
		/// if not set.
		/// </summary>
		/// <returns> the signature provider's name (or {@code null}) </returns>
		/// <seealso cref= #setSigProvider </seealso>
		public virtual String SigProvider
		{
			get
			{
				return this.SigProvider_Renamed;
			}
			set
			{
				this.SigProvider_Renamed = value;
			}
		}


		/// <summary>
		/// Returns the required constraints on the target certificate.
		/// The constraints are returned as an instance of {@code CertSelector}.
		/// If {@code null}, no constraints are defined.
		/// 
		/// <para>Note that the {@code CertSelector} returned is cloned
		/// to protect against subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code CertSelector} specifying the constraints
		/// on the target certificate (or {@code null}) </returns>
		/// <seealso cref= #setTargetCertConstraints </seealso>
		public virtual CertSelector TargetCertConstraints
		{
			get
			{
				if (CertSelector != null)
				{
					return (CertSelector) CertSelector.Clone();
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (value != null)
				{
					CertSelector = (CertSelector) value.Clone();
				}
				else
				{
					CertSelector = null;
				}
			}
		}


		/// <summary>
		/// Makes a copy of this {@code PKIXParameters} object. Changes
		/// to the copy will not affect the original and vice versa.
		/// </summary>
		/// <returns> a copy of this {@code PKIXParameters} object </returns>
		public virtual Object Clone()
		{
			try
			{
				PKIXParameters copy = (PKIXParameters)base.Clone();

				// must clone these because addCertStore, et al. modify them
				if (CertStores_Renamed != null)
				{
					copy.CertStores_Renamed = new List<CertStore>(CertStores_Renamed);
				}
				if (CertPathCheckers_Renamed != null)
				{
					copy.CertPathCheckers_Renamed = new List<PKIXCertPathChecker>(CertPathCheckers_Renamed.Count);
					foreach (PKIXCertPathChecker checker in CertPathCheckers_Renamed)
					{
						copy.CertPathCheckers_Renamed.Add((PKIXCertPathChecker)checker.Clone());
					}
				}

				// other class fields are immutable to public, don't bother
				// to clone the read-only fields.
				return copy;
			}
			catch (CloneNotSupportedException e)
			{
				/* Cannot happen */
				throw new InternalError(e.ToString(), e);
			}
		}

		/// <summary>
		/// Returns a formatted string describing the parameters.
		/// </summary>
		/// <returns> a formatted string describing the parameters. </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("[\n");

			/* start with trusted anchor info */
			if (UnmodTrustAnchors != null)
			{
				sb.Append("  Trust Anchors: " + UnmodTrustAnchors.ToString() + "\n");
			}

			/* now, append initial state information */
			if (UnmodInitialPolicies != null)
			{
				if (UnmodInitialPolicies.Count == 0)
				{
					sb.Append("  Initial Policy OIDs: any\n");
				}
				else
				{
					sb.Append("  Initial Policy OIDs: [" + UnmodInitialPolicies.ToString() + "]\n");
				}
			}

			/* now, append constraints on all certificates in the path */
			sb.Append("  Validity Date: " + Convert.ToString(Date_Renamed) + "\n");
			sb.Append("  Signature Provider: " + Convert.ToString(SigProvider_Renamed) + "\n");
			sb.Append("  Default Revocation Enabled: " + RevocationEnabled_Renamed + "\n");
			sb.Append("  Explicit Policy Required: " + ExplicitPolicyRequired_Renamed + "\n");
			sb.Append("  Policy Mapping Inhibited: " + PolicyMappingInhibited_Renamed + "\n");
			sb.Append("  Any Policy Inhibited: " + AnyPolicyInhibited_Renamed + "\n");
			sb.Append("  Policy Qualifiers Rejected: " + PolicyQualifiersRejected_Renamed + "\n");

			/* now, append target cert requirements */
			sb.Append("  Target Cert Constraints: " + Convert.ToString(CertSelector) + "\n");

			/* finally, append miscellaneous parameters */
			if (CertPathCheckers_Renamed != null)
			{
				sb.Append("  Certification Path Checkers: [" + CertPathCheckers_Renamed.ToString() + "]\n");
			}
			if (CertStores_Renamed != null)
			{
				sb.Append("  CertStores: [" + CertStores_Renamed.ToString() + "]\n");
			}
			sb.Append("]");
			return sb.ToString();
		}
	}

}