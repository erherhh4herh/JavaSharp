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
	/// A specification of the result of a certification path validator algorithm.
	/// <para>
	/// The purpose of this interface is to group (and provide type safety
	/// for) all certification path validator results. All results returned
	/// by the <seealso cref="CertPathValidator#validate CertPathValidator.validate"/>
	/// method must implement this interface.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CertPathValidator
	/// 
	/// @since       1.4
	/// @author      Yassir Elley </seealso>
	public interface CertPathValidatorResult : Cloneable
	{

		/// <summary>
		/// Makes a copy of this {@code CertPathValidatorResult}. Changes to the
		/// copy will not affect the original and vice versa.
		/// </summary>
		/// <returns> a copy of this {@code CertPathValidatorResult} </returns>
		Object Clone();
	}

}