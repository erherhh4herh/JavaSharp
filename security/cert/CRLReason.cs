﻿/*
 * Copyright (c) 2007, Oracle and/or its affiliates. All rights reserved.
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
	/// The CRLReason enumeration specifies the reason that a certificate
	/// is revoked, as defined in <a href="http://www.ietf.org/rfc/rfc3280.txt">
	/// RFC 3280: Internet X.509 Public Key Infrastructure Certificate and CRL
	/// Profile</a>.
	/// 
	/// @author Sean Mullan
	/// @since 1.7 </summary>
	/// <seealso cref= X509CRLEntry#getRevocationReason </seealso>
	/// <seealso cref= CertificateRevokedException#getRevocationReason </seealso>
	public enum CRLReason
	{
		/// <summary>
		/// This reason indicates that it is unspecified as to why the
		/// certificate has been revoked.
		/// </summary>
		UNSPECIFIED,

		/// <summary>
		/// This reason indicates that it is known or suspected that the
		/// certificate subject's private key has been compromised. It applies
		/// to end-entity certificates only.
		/// </summary>
		KEY_COMPROMISE,

		/// <summary>
		/// This reason indicates that it is known or suspected that the
		/// certificate subject's private key has been compromised. It applies
		/// to certificate authority (CA) certificates only.
		/// </summary>
		CA_COMPROMISE,

		/// <summary>
		/// This reason indicates that the subject's name or other information
		/// has changed.
		/// </summary>
		AFFILIATION_CHANGED,

		/// <summary>
		/// This reason indicates that the certificate has been superseded.
		/// </summary>
		SUPERSEDED,

		/// <summary>
		/// This reason indicates that the certificate is no longer needed.
		/// </summary>
		CESSATION_OF_OPERATION,

		/// <summary>
		/// This reason indicates that the certificate has been put on hold.
		/// </summary>
		CERTIFICATE_HOLD,

		/// <summary>
		/// Unused reason.
		/// </summary>
		UNUSED,

		/// <summary>
		/// This reason indicates that the certificate was previously on hold
		/// and should be removed from the CRL. It is for use with delta CRLs.
		/// </summary>
		REMOVE_FROM_CRL,

		/// <summary>
		/// This reason indicates that the privileges granted to the subject of
		/// the certificate have been withdrawn.
		/// </summary>
		PRIVILEGE_WITHDRAWN,

		/// <summary>
		/// This reason indicates that it is known or suspected that the
		/// certificate subject's private key has been compromised. It applies
		/// to authority attribute (AA) certificates only.
		/// </summary>
		AA_COMPROMISE
	}

}