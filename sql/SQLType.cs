/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.sql
{

	/// <summary>
	/// An object that is used to identify a generic SQL type, called a JDBC type or
	/// a vendor specific data type.
	/// 
	/// @since 1.8
	/// </summary>
	public interface SQLType
	{

		/// <summary>
		/// Returns the {@code SQLType} name that represents a SQL data type.
		/// </summary>
		/// <returns> The name of this {@code SQLType}. </returns>
		String Name {get;}

		/// <summary>
		/// Returns the name of the vendor that supports this data type. The value
		/// returned typically is the package name for this vendor.
		/// </summary>
		/// <returns> The name of the vendor for this data type </returns>
		String Vendor {get;}

		/// <summary>
		/// Returns the vendor specific type number for the data type.
		/// </summary>
		/// <returns> An Integer representing the vendor specific data type </returns>
		Integer VendorTypeNumber {get;}
	}

}