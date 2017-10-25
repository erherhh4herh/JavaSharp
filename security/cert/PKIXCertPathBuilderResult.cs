using System;

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
	/// This class represents the successful result of the PKIX certification
	/// path builder algorithm. All certification paths that are built and
	/// returned using this algorithm are also validated according to the PKIX
	/// certification path validation algorithm.
	/// 
	/// <para>Instances of {@code PKIXCertPathBuilderResult} are returned by
	/// the {@code build} method of {@code CertPathBuilder}
	/// objects implementing the PKIX algorithm.
	/// 
	/// </para>
	/// <para>All {@code PKIXCertPathBuilderResult} objects contain the
	/// certification path constructed by the build algorithm, the
	/// valid policy tree and subject public key resulting from the build
	/// algorithm, and a {@code TrustAnchor} describing the certification
	/// authority (CA) that served as a trust anchor for the certification path.
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
	/// <seealso cref= CertPathBuilderResult
	/// 
	/// @since       1.4
	/// @author      Anne Anderson </seealso>
	public class PKIXCertPathBuilderResult : PKIXCertPathValidatorResult, CertPathBuilderResult
	{

		private CertPath CertPath_Renamed;

		/// <summary>
		/// Creates an instance of {@code PKIXCertPathBuilderResult}
		/// containing the specified parameters.
		/// </summary>
		/// <param name="certPath"> the validated {@code CertPath} </param>
		/// <param name="trustAnchor"> a {@code TrustAnchor} describing the CA that
		/// served as a trust anchor for the certification path </param>
		/// <param name="policyTree"> the immutable valid policy tree, or {@code null}
		/// if there are no valid policies </param>
		/// <param name="subjectPublicKey"> the public key of the subject </param>
		/// <exception cref="NullPointerException"> if the {@code certPath},
		/// {@code trustAnchor} or {@code subjectPublicKey} parameters
		/// are {@code null} </exception>
		public PKIXCertPathBuilderResult(CertPath certPath, TrustAnchor trustAnchor, PolicyNode policyTree, PublicKey subjectPublicKey) : base(trustAnchor, policyTree, subjectPublicKey)
		{
			if (certPath == null)
			{
				throw new NullPointerException("certPath must be non-null");
			}
			this.CertPath_Renamed = certPath;
		}

		/// <summary>
		/// Returns the built and validated certification path. The
		/// {@code CertPath} object does not include the trust anchor.
		/// Instead, use the <seealso cref="#getTrustAnchor() getTrustAnchor()"/> method to
		/// obtain the {@code TrustAnchor} that served as the trust anchor
		/// for the certification path.
		/// </summary>
		/// <returns> the built and validated {@code CertPath} (never
		/// {@code null}) </returns>
		public virtual CertPath CertPath
		{
			get
			{
				return CertPath_Renamed;
			}
		}

		/// <summary>
		/// Return a printable representation of this
		/// {@code PKIXCertPathBuilderResult}.
		/// </summary>
		/// <returns> a {@code String} describing the contents of this
		///         {@code PKIXCertPathBuilderResult} </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("PKIXCertPathBuilderResult: [\n");
			sb.Append("  Certification Path: " + CertPath_Renamed + "\n");
			sb.Append("  Trust Anchor: " + TrustAnchor.ToString() + "\n");
			sb.Append("  Policy Tree: " + Convert.ToString(PolicyTree) + "\n");
			sb.Append("  Subject Public Key: " + PublicKey + "\n");
			sb.Append("]");
			return sb.ToString();
		}
	}

}