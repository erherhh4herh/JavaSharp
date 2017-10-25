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
	/// Parameters used as input for the PKIX {@code CertPathBuilder}
	/// algorithm.
	/// <para>
	/// A PKIX {@code CertPathBuilder} uses these parameters to {@link
	/// CertPathBuilder#build build} a {@code CertPath} which has been
	/// validated according to the PKIX certification path validation algorithm.
	/// 
	/// </para>
	/// <para>To instantiate a {@code PKIXBuilderParameters} object, an
	/// application must specify one or more <i>most-trusted CAs</i> as defined by
	/// the PKIX certification path validation algorithm. The most-trusted CA
	/// can be specified using one of two constructors. An application
	/// can call {@link #PKIXBuilderParameters(Set, CertSelector)
	/// PKIXBuilderParameters(Set, CertSelector)}, specifying a
	/// {@code Set} of {@code TrustAnchor} objects, each of which
	/// identifies a most-trusted CA. Alternatively, an application can call
	/// {@link #PKIXBuilderParameters(KeyStore, CertSelector)
	/// PKIXBuilderParameters(KeyStore, CertSelector)}, specifying a
	/// {@code KeyStore} instance containing trusted certificate entries, each
	/// of which will be considered as a most-trusted CA.
	/// 
	/// </para>
	/// <para>In addition, an application must specify constraints on the target
	/// certificate that the {@code CertPathBuilder} will attempt
	/// to build a path to. The constraints are specified as a
	/// {@code CertSelector} object. These constraints should provide the
	/// {@code CertPathBuilder} with enough search criteria to find the target
	/// certificate. Minimal criteria for an {@code X509Certificate} usually
	/// include the subject name and/or one or more subject alternative names.
	/// If enough criteria is not specified, the {@code CertPathBuilder}
	/// may throw a {@code CertPathBuilderException}.
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
	/// <seealso cref= CertPathBuilder
	/// 
	/// @since       1.4
	/// @author      Sean Mullan </seealso>
	public class PKIXBuilderParameters : PKIXParameters
	{

		private int MaxPathLength_Renamed = 5;

		/// <summary>
		/// Creates an instance of {@code PKIXBuilderParameters} with
		/// the specified {@code Set} of most-trusted CAs.
		/// Each element of the set is a <seealso cref="TrustAnchor TrustAnchor"/>.
		/// 
		/// <para>Note that the {@code Set} is copied to protect against
		/// subsequent modifications.
		/// 
		/// </para>
		/// </summary>
		/// <param name="trustAnchors"> a {@code Set} of {@code TrustAnchor}s </param>
		/// <param name="targetConstraints"> a {@code CertSelector} specifying the
		/// constraints on the target certificate </param>
		/// <exception cref="InvalidAlgorithmParameterException"> if {@code trustAnchors}
		/// is empty {@code (trustAnchors.isEmpty() == true)} </exception>
		/// <exception cref="NullPointerException"> if {@code trustAnchors} is
		/// {@code null} </exception>
		/// <exception cref="ClassCastException"> if any of the elements of
		/// {@code trustAnchors} are not of type
		/// {@code java.security.cert.TrustAnchor} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PKIXBuilderParameters(java.util.Set<TrustAnchor> trustAnchors, CertSelector targetConstraints) throws java.security.InvalidAlgorithmParameterException
		public PKIXBuilderParameters(Set<TrustAnchor> trustAnchors, CertSelector targetConstraints) : base(trustAnchors)
		{
			TargetCertConstraints = targetConstraints;
		}

		/// <summary>
		/// Creates an instance of {@code PKIXBuilderParameters} that
		/// populates the set of most-trusted CAs from the trusted
		/// certificate entries contained in the specified {@code KeyStore}.
		/// Only keystore entries that contain trusted {@code X509Certificate}s
		/// are considered; all other certificate types are ignored.
		/// </summary>
		/// <param name="keystore"> a {@code KeyStore} from which the set of
		/// most-trusted CAs will be populated </param>
		/// <param name="targetConstraints"> a {@code CertSelector} specifying the
		/// constraints on the target certificate </param>
		/// <exception cref="KeyStoreException"> if {@code keystore} has not been
		/// initialized </exception>
		/// <exception cref="InvalidAlgorithmParameterException"> if {@code keystore} does
		/// not contain at least one trusted certificate entry </exception>
		/// <exception cref="NullPointerException"> if {@code keystore} is
		/// {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PKIXBuilderParameters(java.security.KeyStore keystore, CertSelector targetConstraints) throws java.security.KeyStoreException, java.security.InvalidAlgorithmParameterException
		public PKIXBuilderParameters(KeyStore keystore, CertSelector targetConstraints) : base(keystore)
		{
			TargetCertConstraints = targetConstraints;
		}

		/// <summary>
		/// Sets the value of the maximum number of non-self-issued intermediate
		/// certificates that may exist in a certification path. A certificate
		/// is self-issued if the DNs that appear in the subject and issuer
		/// fields are identical and are not empty. Note that the last certificate
		/// in a certification path is not an intermediate certificate, and is not
		/// included in this limit. Usually the last certificate is an end entity
		/// certificate, but it can be a CA certificate. A PKIX
		/// {@code CertPathBuilder} instance must not build
		/// paths longer than the length specified.
		/// 
		/// <para> A value of 0 implies that the path can only contain
		/// a single certificate. A value of -1 implies that the
		/// path length is unconstrained (i.e. there is no maximum).
		/// The default maximum path length, if not specified, is 5.
		/// Setting a value less than -1 will cause an exception to be thrown.
		/// 
		/// </para>
		/// <para> If any of the CA certificates contain the
		/// {@code BasicConstraintsExtension}, the value of the
		/// {@code pathLenConstraint} field of the extension overrides
		/// the maximum path length parameter whenever the result is a
		/// certification path of smaller length.
		/// 
		/// </para>
		/// </summary>
		/// <param name="maxPathLength"> the maximum number of non-self-issued intermediate
		///  certificates that may exist in a certification path </param>
		/// <exception cref="InvalidParameterException"> if {@code maxPathLength} is set
		///  to a value less than -1
		/// </exception>
		/// <seealso cref= #getMaxPathLength </seealso>
		public virtual int MaxPathLength
		{
			set
			{
				if (value < -1)
				{
					throw new InvalidParameterException("the maximum path " + "length parameter can not be less than -1");
				}
				this.MaxPathLength_Renamed = value;
			}
			get
			{
				return MaxPathLength_Renamed;
			}
		}


		/// <summary>
		/// Returns a formatted string describing the parameters.
		/// </summary>
		/// <returns> a formatted string describing the parameters </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("[\n");
			sb.Append(base.ToString());
			sb.Append("  Maximum Path Length: " + MaxPathLength_Renamed + "\n");
			sb.Append("]\n");
			return sb.ToString();
		}
	}

}