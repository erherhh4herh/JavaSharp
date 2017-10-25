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

	using HexDumpEncoder = sun.misc.HexDumpEncoder;
	using DerValue = sun.security.util.DerValue;

	/// <summary>
	/// An immutable policy qualifier represented by the ASN.1 PolicyQualifierInfo
	/// structure.
	/// 
	/// <para>The ASN.1 definition is as follows:
	/// <pre>
	///   PolicyQualifierInfo ::= SEQUENCE {
	///        policyQualifierId       PolicyQualifierId,
	///        qualifier               ANY DEFINED BY policyQualifierId }
	/// </pre>
	/// </para>
	/// <para>
	/// A certificate policies extension, if present in an X.509 version 3
	/// certificate, contains a sequence of one or more policy information terms,
	/// each of which consists of an object identifier (OID) and optional
	/// qualifiers. In an end-entity certificate, these policy information terms
	/// indicate the policy under which the certificate has been issued and the
	/// purposes for which the certificate may be used. In a CA certificate, these
	/// policy information terms limit the set of policies for certification paths
	/// which include this certificate.
	/// </para>
	/// <para>
	/// A {@code Set} of {@code PolicyQualifierInfo} objects are returned
	/// by the <seealso cref="PolicyNode#getPolicyQualifiers PolicyNode.getPolicyQualifiers"/>
	/// method. This allows applications with specific policy requirements to
	/// process and validate each policy qualifier. Applications that need to
	/// process policy qualifiers should explicitly set the
	/// {@code policyQualifiersRejected} flag to false (by calling the
	/// {@link PKIXParameters#setPolicyQualifiersRejected
	/// PKIXParameters.setPolicyQualifiersRejected} method) before validating
	/// a certification path.
	/// 
	/// </para>
	/// <para>Note that the PKIX certification path validation algorithm specifies
	/// that any policy qualifier in a certificate policies extension that is
	/// marked critical must be processed and validated. Otherwise the
	/// certification path must be rejected. If the
	/// {@code policyQualifiersRejected} flag is set to false, it is up to
	/// the application to validate all policy qualifiers in this manner in order
	/// to be PKIX compliant.
	/// 
	/// </para>
	/// <para><b>Concurrent Access</b>
	/// 
	/// </para>
	/// <para>All {@code PolicyQualifierInfo} objects must be immutable and
	/// thread-safe. That is, multiple threads may concurrently invoke the
	/// methods defined in this class on a single {@code PolicyQualifierInfo}
	/// object (or more than one) with no ill effects. Requiring
	/// {@code PolicyQualifierInfo} objects to be immutable and thread-safe
	/// allows them to be passed around to various pieces of code without
	/// worrying about coordinating access.
	/// 
	/// @author      seth proctor
	/// @author      Sean Mullan
	/// @since       1.4
	/// </para>
	/// </summary>
	public class PolicyQualifierInfo
	{

		private sbyte[] MEncoded;
		private String MId;
		private sbyte[] MData;
		private String PqiString;

		/// <summary>
		/// Creates an instance of {@code PolicyQualifierInfo} from the
		/// encoded bytes. The encoded byte array is copied on construction.
		/// </summary>
		/// <param name="encoded"> a byte array containing the qualifier in DER encoding </param>
		/// <exception cref="IOException"> thrown if the byte array does not represent a
		/// valid and parsable policy qualifier </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PolicyQualifierInfo(byte[] encoded) throws java.io.IOException
		public PolicyQualifierInfo(sbyte[] encoded)
		{
			MEncoded = encoded.clone();

			DerValue val = new DerValue(MEncoded);
			if (val.tag != DerValue.tag_Sequence)
			{
				throw new IOException("Invalid encoding for PolicyQualifierInfo");
			}

			MId = (val.data.DerValue).OID.ToString();
			sbyte[] tmp = val.data.toByteArray();
			if (tmp == null)
			{
				MData = null;
			}
			else
			{
				MData = new sbyte[tmp.Length];
				System.Array.Copy(tmp, 0, MData, 0, tmp.Length);
			}
		}

		/// <summary>
		/// Returns the {@code policyQualifierId} field of this
		/// {@code PolicyQualifierInfo}. The {@code policyQualifierId}
		/// is an Object Identifier (OID) represented by a set of nonnegative
		/// integers separated by periods.
		/// </summary>
		/// <returns> the OID (never {@code null}) </returns>
		public String PolicyQualifierId
		{
			get
			{
				return MId;
			}
		}

		/// <summary>
		/// Returns the ASN.1 DER encoded form of this
		/// {@code PolicyQualifierInfo}.
		/// </summary>
		/// <returns> the ASN.1 DER encoded bytes (never {@code null}).
		/// Note that a copy is returned, so the data is cloned each time
		/// this method is called. </returns>
		public sbyte[] Encoded
		{
			get
			{
				return MEncoded.clone();
			}
		}

		/// <summary>
		/// Returns the ASN.1 DER encoded form of the {@code qualifier}
		/// field of this {@code PolicyQualifierInfo}.
		/// </summary>
		/// <returns> the ASN.1 DER encoded bytes of the {@code qualifier}
		/// field. Note that a copy is returned, so the data is cloned each
		/// time this method is called. </returns>
		public sbyte[] PolicyQualifier
		{
			get
			{
				return (MData == null ? null : MData.clone());
			}
		}

		/// <summary>
		/// Return a printable representation of this
		/// {@code PolicyQualifierInfo}.
		/// </summary>
		/// <returns> a {@code String} describing the contents of this
		///         {@code PolicyQualifierInfo} </returns>
		public override String ToString()
		{
			if (PqiString != null)
			{
				return PqiString;
			}
			HexDumpEncoder enc = new HexDumpEncoder();
			StringBuffer sb = new StringBuffer();
			sb.Append("PolicyQualifierInfo: [\n");
			sb.Append("  qualifierID: " + MId + "\n");
			sb.Append("  qualifier: " + (MData == null ? "null" : enc.encodeBuffer(MData)) + "\n");
			sb.Append("]");
			PqiString = sb.ToString();
			return PqiString;
		}
	}

}