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
	/// The <i>Service Provider Interface</i> (<b>SPI</b>)
	/// for the <seealso cref="CertStore CertStore"/> class. All {@code CertStore}
	/// implementations must include a class (the SPI class) that extends
	/// this class ({@code CertStoreSpi}), provides a constructor with
	/// a single argument of type {@code CertStoreParameters}, and implements
	/// all of its methods. In general, instances of this class should only be
	/// accessed through the {@code CertStore} class.
	/// For details, see the Java Cryptography Architecture.
	/// <para>
	/// <b>Concurrent Access</b>
	/// </para>
	/// <para>
	/// The public methods of all {@code CertStoreSpi} objects must be
	/// thread-safe. That is, multiple threads may concurrently invoke these
	/// methods on a single {@code CertStoreSpi} object (or more than one)
	/// with no ill effects. This allows a {@code CertPathBuilder} to search
	/// for a CRL while simultaneously searching for further certificates, for
	/// instance.
	/// </para>
	/// <para>
	/// Simple {@code CertStoreSpi} implementations will probably ensure
	/// thread safety by adding a {@code synchronized} keyword to their
	/// {@code engineGetCertificates} and {@code engineGetCRLs} methods.
	/// More sophisticated ones may allow truly concurrent access.
	/// 
	/// @since       1.4
	/// @author      Steve Hanna
	/// </para>
	/// </summary>
	public abstract class CertStoreSpi
	{

		/// <summary>
		/// The sole constructor.
		/// </summary>
		/// <param name="params"> the initialization parameters (may be {@code null}) </param>
		/// <exception cref="InvalidAlgorithmParameterException"> if the initialization
		/// parameters are inappropriate for this {@code CertStoreSpi} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CertStoreSpi(CertStoreParameters params) throws java.security.InvalidAlgorithmParameterException
		public CertStoreSpi(CertStoreParameters @params)
		{
		}

		/// <summary>
		/// Returns a {@code Collection} of {@code Certificate}s that
		/// match the specified selector. If no {@code Certificate}s
		/// match the selector, an empty {@code Collection} will be returned.
		/// <para>
		/// For some {@code CertStore} types, the resulting
		/// {@code Collection} may not contain <b>all</b> of the
		/// {@code Certificate}s that match the selector. For instance,
		/// an LDAP {@code CertStore} may not search all entries in the
		/// directory. Instead, it may just search entries that are likely to
		/// contain the {@code Certificate}s it is looking for.
		/// </para>
		/// <para>
		/// Some {@code CertStore} implementations (especially LDAP
		/// {@code CertStore}s) may throw a {@code CertStoreException}
		/// unless a non-null {@code CertSelector} is provided that includes
		/// specific criteria that can be used to find the certificates. Issuer
		/// and/or subject names are especially useful criteria.
		/// 
		/// </para>
		/// </summary>
		/// <param name="selector"> A {@code CertSelector} used to select which
		///  {@code Certificate}s should be returned. Specify {@code null}
		///  to return all {@code Certificate}s (if supported). </param>
		/// <returns> A {@code Collection} of {@code Certificate}s that
		///         match the specified selector (never {@code null}) </returns>
		/// <exception cref="CertStoreException"> if an exception occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.util.Collection<? extends Certificate> engineGetCertificates(CertSelector selector) throws CertStoreException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.util.Collection<? extends Certificate> engineGetCertificates(CertSelector selector) throws CertStoreException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public abstract ICollection<?> EngineGetCertificates(CertSelector selector) where ? : Certificate;

		/// <summary>
		/// Returns a {@code Collection} of {@code CRL}s that
		/// match the specified selector. If no {@code CRL}s
		/// match the selector, an empty {@code Collection} will be returned.
		/// <para>
		/// For some {@code CertStore} types, the resulting
		/// {@code Collection} may not contain <b>all</b> of the
		/// {@code CRL}s that match the selector. For instance,
		/// an LDAP {@code CertStore} may not search all entries in the
		/// directory. Instead, it may just search entries that are likely to
		/// contain the {@code CRL}s it is looking for.
		/// </para>
		/// <para>
		/// Some {@code CertStore} implementations (especially LDAP
		/// {@code CertStore}s) may throw a {@code CertStoreException}
		/// unless a non-null {@code CRLSelector} is provided that includes
		/// specific criteria that can be used to find the CRLs. Issuer names
		/// and/or the certificate to be checked are especially useful.
		/// 
		/// </para>
		/// </summary>
		/// <param name="selector"> A {@code CRLSelector} used to select which
		///  {@code CRL}s should be returned. Specify {@code null}
		///  to return all {@code CRL}s (if supported). </param>
		/// <returns> A {@code Collection} of {@code CRL}s that
		///         match the specified selector (never {@code null}) </returns>
		/// <exception cref="CertStoreException"> if an exception occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.util.Collection<? extends CRL> engineGetCRLs(CRLSelector selector) throws CertStoreException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.util.Collection<? extends CRL> engineGetCRLs(CRLSelector selector) throws CertStoreException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public abstract ICollection<?> EngineGetCRLs(CRLSelector selector) where ? : CRL;
	}

}