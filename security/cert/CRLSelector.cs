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
	/// A selector that defines a set of criteria for selecting {@code CRL}s.
	/// Classes that implement this interface are often used to specify
	/// which {@code CRL}s should be retrieved from a {@code CertStore}.
	/// <para>
	/// <b>Concurrent Access</b>
	/// </para>
	/// <para>
	/// Unless otherwise specified, the methods defined in this interface are not
	/// thread-safe. Multiple threads that need to access a single
	/// object concurrently should synchronize amongst themselves and
	/// provide the necessary locking. Multiple threads each manipulating
	/// separate objects need not synchronize.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CRL </seealso>
	/// <seealso cref= CertStore </seealso>
	/// <seealso cref= CertStore#getCRLs
	/// 
	/// @author      Steve Hanna
	/// @since       1.4 </seealso>
	public interface CRLSelector : Cloneable
	{

		/// <summary>
		/// Decides whether a {@code CRL} should be selected.
		/// </summary>
		/// <param name="crl">     the {@code CRL} to be checked </param>
		/// <returns>  {@code true} if the {@code CRL} should be selected,
		/// {@code false} otherwise </returns>
		bool Match(CRL crl);

		/// <summary>
		/// Makes a copy of this {@code CRLSelector}. Changes to the
		/// copy will not affect the original and vice versa.
		/// </summary>
		/// <returns> a copy of this {@code CRLSelector} </returns>
		Object Clone();
	}

}