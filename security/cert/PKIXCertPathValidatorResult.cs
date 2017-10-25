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
	/// path validation algorithm.
	/// 
	/// <para>Instances of {@code PKIXCertPathValidatorResult} are returned by the
	/// <seealso cref="CertPathValidator#validate validate"/> method of
	/// {@code CertPathValidator} objects implementing the PKIX algorithm.
	/// 
	/// </para>
	/// <para> All {@code PKIXCertPathValidatorResult} objects contain the
	/// valid policy tree and subject public key resulting from the
	/// validation algorithm, as well as a {@code TrustAnchor} describing
	/// the certification authority (CA) that served as a trust anchor for the
	/// certification path.
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
	/// <seealso cref= CertPathValidatorResult
	/// 
	/// @since       1.4
	/// @author      Yassir Elley
	/// @author      Sean Mullan </seealso>
	public class PKIXCertPathValidatorResult : CertPathValidatorResult
	{

		private TrustAnchor TrustAnchor_Renamed;
		private PolicyNode PolicyTree_Renamed;
		private PublicKey SubjectPublicKey;

		/// <summary>
		/// Creates an instance of {@code PKIXCertPathValidatorResult}
		/// containing the specified parameters.
		/// </summary>
		/// <param name="trustAnchor"> a {@code TrustAnchor} describing the CA that
		/// served as a trust anchor for the certification path </param>
		/// <param name="policyTree"> the immutable valid policy tree, or {@code null}
		/// if there are no valid policies </param>
		/// <param name="subjectPublicKey"> the public key of the subject </param>
		/// <exception cref="NullPointerException"> if the {@code subjectPublicKey} or
		/// {@code trustAnchor} parameters are {@code null} </exception>
		public PKIXCertPathValidatorResult(TrustAnchor trustAnchor, PolicyNode policyTree, PublicKey subjectPublicKey)
		{
			if (subjectPublicKey == null)
			{
				throw new NullPointerException("subjectPublicKey must be non-null");
			}
			if (trustAnchor == null)
			{
				throw new NullPointerException("trustAnchor must be non-null");
			}
			this.TrustAnchor_Renamed = trustAnchor;
			this.PolicyTree_Renamed = policyTree;
			this.SubjectPublicKey = subjectPublicKey;
		}

		/// <summary>
		/// Returns the {@code TrustAnchor} describing the CA that served
		/// as a trust anchor for the certification path.
		/// </summary>
		/// <returns> the {@code TrustAnchor} (never {@code null}) </returns>
		public virtual TrustAnchor TrustAnchor
		{
			get
			{
				return TrustAnchor_Renamed;
			}
		}

		/// <summary>
		/// Returns the root node of the valid policy tree resulting from the
		/// PKIX certification path validation algorithm. The
		/// {@code PolicyNode} object that is returned and any objects that
		/// it returns through public methods are immutable.
		/// 
		/// <para>Most applications will not need to examine the valid policy tree.
		/// They can achieve their policy processing goals by setting the
		/// policy-related parameters in {@code PKIXParameters}. However, more
		/// sophisticated applications, especially those that process policy
		/// qualifiers, may need to traverse the valid policy tree using the
		/// <seealso cref="PolicyNode#getParent PolicyNode.getParent"/> and
		/// <seealso cref="PolicyNode#getChildren PolicyNode.getChildren"/> methods.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the root node of the valid policy tree, or {@code null}
		/// if there are no valid policies </returns>
		public virtual PolicyNode PolicyTree
		{
			get
			{
				return PolicyTree_Renamed;
			}
		}

		/// <summary>
		/// Returns the public key of the subject (target) of the certification
		/// path, including any inherited public key parameters if applicable.
		/// </summary>
		/// <returns> the public key of the subject (never {@code null}) </returns>
		public virtual PublicKey PublicKey
		{
			get
			{
				return SubjectPublicKey;
			}
		}

		/// <summary>
		/// Returns a copy of this object.
		/// </summary>
		/// <returns> the copy </returns>
		public virtual Object Clone()
		{
			try
			{
				return base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				/* Cannot happen */
				throw new InternalError(e.ToString(), e);
			}
		}

		/// <summary>
		/// Return a printable representation of this
		/// {@code PKIXCertPathValidatorResult}.
		/// </summary>
		/// <returns> a {@code String} describing the contents of this
		///         {@code PKIXCertPathValidatorResult} </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("PKIXCertPathValidatorResult: [\n");
			sb.Append("  Trust Anchor: " + TrustAnchor_Renamed.ToString() + "\n");
			sb.Append("  Policy Tree: " + Convert.ToString(PolicyTree_Renamed) + "\n");
			sb.Append("  Subject Public Key: " + SubjectPublicKey + "\n");
			sb.Append("]");
			return sb.ToString();
		}
	}

}