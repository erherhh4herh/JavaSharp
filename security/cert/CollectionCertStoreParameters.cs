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
	/// Parameters used as input for the Collection {@code CertStore}
	/// algorithm.
	/// <para>
	/// This class is used to provide necessary configuration parameters
	/// to implementations of the Collection {@code CertStore}
	/// algorithm. The only parameter included in this class is the
	/// {@code Collection} from which the {@code CertStore} will
	/// retrieve certificates and CRLs.
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
	/// @since       1.4
	/// @author      Steve Hanna
	/// </para>
	/// </summary>
	/// <seealso cref=         java.util.Collection </seealso>
	/// <seealso cref=         CertStore </seealso>
	public class CollectionCertStoreParameters : CertStoreParameters
	{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private java.util.Collection<?> coll;
		private ICollection<?> Coll;

		/// <summary>
		/// Creates an instance of {@code CollectionCertStoreParameters}
		/// which will allow certificates and CRLs to be retrieved from the
		/// specified {@code Collection}. If the specified
		/// {@code Collection} contains an object that is not a
		/// {@code Certificate} or {@code CRL}, that object will be
		/// ignored by the Collection {@code CertStore}.
		/// <para>
		/// The {@code Collection} is <b>not</b> copied. Instead, a
		/// reference is used. This allows the caller to subsequently add or
		/// remove {@code Certificates} or {@code CRL}s from the
		/// {@code Collection}, thus changing the set of
		/// {@code Certificates} or {@code CRL}s available to the
		/// Collection {@code CertStore}. The Collection {@code CertStore}
		/// will not modify the contents of the {@code Collection}.
		/// </para>
		/// <para>
		/// If the {@code Collection} will be modified by one thread while
		/// another thread is calling a method of a Collection {@code CertStore}
		/// that has been initialized with this {@code Collection}, the
		/// {@code Collection} must have fail-fast iterators.
		/// 
		/// </para>
		/// </summary>
		/// <param name="collection"> a {@code Collection} of
		///        {@code Certificate}s and {@code CRL}s </param>
		/// <exception cref="NullPointerException"> if {@code collection} is
		/// {@code null} </exception>
		public CollectionCertStoreParameters<T1>(ICollection<T1> collection)
		{
			if (collection == null)
			{
				throw new NullPointerException();
			}
			Coll = collection;
		}

		/// <summary>
		/// Creates an instance of {@code CollectionCertStoreParameters} with
		/// the default parameter values (an empty and immutable
		/// {@code Collection}).
		/// </summary>
		public CollectionCertStoreParameters()
		{
			Coll = Collections.EMPTY_SET;
		}

		/// <summary>
		/// Returns the {@code Collection} from which {@code Certificate}s
		/// and {@code CRL}s are retrieved. This is <b>not</b> a copy of the
		/// {@code Collection}, it is a reference. This allows the caller to
		/// subsequently add or remove {@code Certificates} or
		/// {@code CRL}s from the {@code Collection}.
		/// </summary>
		/// <returns> the {@code Collection} (never null) </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.Collection<?> getCollection()
		public virtual ICollection<?> Collection
		{
			get
			{
				return Coll;
			}
		}

		/// <summary>
		/// Returns a copy of this object. Note that only a reference to the
		/// {@code Collection} is copied, and not the contents.
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
		/// Returns a formatted string describing the parameters.
		/// </summary>
		/// <returns> a formatted string describing the parameters </returns>
		public override String ToString()
		{
			StringBuffer sb = new StringBuffer();
			sb.Append("CollectionCertStoreParameters: [\n");
			sb.Append("  collection: " + Coll + "\n");
			sb.Append("]");
			return sb.ToString();
		}
	}

}