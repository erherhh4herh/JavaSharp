/*
 * Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The {@code PKIXReason} enumerates the potential PKIX-specific reasons
	/// that an X.509 certification path may be invalid according to the PKIX
	/// (RFC 3280) standard. These reasons are in addition to those of the
	/// {@code CertPathValidatorException.BasicReason} enumeration.
	/// 
	/// @since 1.7
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: public enum PKIXReason implements CertPathValidatorException.Reason
	public enum PKIXReason
	{
		/// <summary>
		/// The certificate does not chain correctly.
		/// </summary>
		NAME_CHAINING,

		/// <summary>
		/// The certificate's key usage is invalid.
		/// </summary>
		INVALID_KEY_USAGE,

		/// <summary>
		/// The policy constraints have been violated.
		/// </summary>
		INVALID_POLICY,

		/// <summary>
		/// No acceptable trust anchor found.
		/// </summary>
		NO_TRUST_ANCHOR,

		/// <summary>
		/// The certificate contains one or more unrecognized critical
		/// extensions.
		/// </summary>
		UNRECOGNIZED_CRIT_EXT,

		/// <summary>
		/// The certificate is not a CA certificate.
		/// </summary>
		NOT_CA_CERT,

		/// <summary>
		/// The path length constraint has been violated.
		/// </summary>
		PATH_TOO_LONG,

		/// <summary>
		/// The name constraints have been violated.
		/// </summary>
		INVALID_NAME
	}

}