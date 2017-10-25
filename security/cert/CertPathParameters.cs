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
	/// A specification of certification path algorithm parameters.
	/// The purpose of this interface is to group (and provide type safety for)
	/// all {@code CertPath} parameter specifications. All
	/// {@code CertPath} parameter specifications must implement this
	/// interface.
	/// 
	/// @author      Yassir Elley </summary>
	/// <seealso cref=         CertPathValidator#validate(CertPath, CertPathParameters) </seealso>
	/// <seealso cref=         CertPathBuilder#build(CertPathParameters)
	/// @since       1.4 </seealso>
	public interface CertPathParameters : Cloneable
	{

	  /// <summary>
	  /// Makes a copy of this {@code CertPathParameters}. Changes to the
	  /// copy will not affect the original and vice versa.
	  /// </summary>
	  /// <returns> a copy of this {@code CertPathParameters} </returns>
	  Object Clone();
	}

}