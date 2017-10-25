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
	/// A specification of {@code CertStore} parameters.
	/// <para>
	/// The purpose of this interface is to group (and provide type safety for)
	/// all {@code CertStore} parameter specifications. All
	/// {@code CertStore} parameter specifications must implement this
	/// interface.
	/// </para>
	/// <para>
	/// Typically, a {@code CertStoreParameters} object is passed as a parameter
	/// to one of the <seealso cref="CertStore#getInstance CertStore.getInstance"/> methods.
	/// The {@code getInstance} method returns a {@code CertStore} that
	/// is used for retrieving {@code Certificate}s and {@code CRL}s. The
	/// {@code CertStore} that is returned is initialized with the specified
	/// parameters. The type of parameters needed may vary between different types
	/// of {@code CertStore}s.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CertStore#getInstance
	/// 
	/// @since       1.4
	/// @author      Steve Hanna </seealso>
	public interface CertStoreParameters : Cloneable
	{

		/// <summary>
		/// Makes a copy of this {@code CertStoreParameters}.
		/// <para>
		/// The precise meaning of "copy" may depend on the class of
		/// the {@code CertStoreParameters} object. A typical implementation
		/// performs a "deep copy" of this object, but this is not an absolute
		/// requirement. Some implementations may perform a "shallow copy" of some
		/// or all of the fields of this object.
		/// </para>
		/// <para>
		/// Note that the {@code CertStore.getInstance} methods make a copy
		/// of the specified {@code CertStoreParameters}. A deep copy
		/// implementation of {@code clone} is safer and more robust, as it
		/// prevents the caller from corrupting a shared {@code CertStore} by
		/// subsequently modifying the contents of its initialization parameters.
		/// However, a shallow copy implementation of {@code clone} is more
		/// appropriate for applications that need to hold a reference to a
		/// parameter contained in the {@code CertStoreParameters}. For example,
		/// a shallow copy clone allows an application to release the resources of
		/// a particular {@code CertStore} initialization parameter immediately,
		/// rather than waiting for the garbage collection mechanism. This should
		/// be done with the utmost care, since the {@code CertStore} may still
		/// be in use by other threads.
		/// </para>
		/// <para>
		/// Each subclass should state the precise behavior of this method so
		/// that users and developers know what to expect.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a copy of this {@code CertStoreParameters} </returns>
		Object Clone();
	}

}