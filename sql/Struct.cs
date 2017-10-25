using System.Collections.Generic;

/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// <para>The standard mapping in the Java programming language for an SQL
	/// structured type. A <code>Struct</code> object contains a
	/// value for each attribute of the SQL structured type that
	/// it represents.
	/// By default, an instance of<code>Struct</code> is valid as long as the
	/// application has a reference to it.
	/// </para>
	/// <para>
	/// All methods on the <code>Struct</code> interface must be fully implemented if the
	/// JDBC driver supports the data type.
	/// @since 1.2
	/// </para>
	/// </summary>

	public interface Struct
	{

	  /// <summary>
	  /// Retrieves the SQL type name of the SQL structured type
	  /// that this <code>Struct</code> object represents.
	  /// </summary>
	  /// <returns> the fully-qualified type name of the SQL structured
	  ///          type for which this <code>Struct</code> object
	  ///          is the generic representation </returns>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getSQLTypeName() throws SQLException;
	  String SQLTypeName {get;}

	  /// <summary>
	  /// Produces the ordered values of the attributes of the SQL
	  /// structured type that this <code>Struct</code> object represents.
	  /// As individual attributes are processed, this method uses the type map
	  /// associated with the
	  /// connection for customizations of the type mappings.
	  /// If there is no
	  /// entry in the connection's type map that matches the structured
	  /// type that an attribute represents,
	  /// the driver uses the standard mapping.
	  /// <para>
	  /// Conceptually, this method calls the method
	  /// <code>getObject</code> on each attribute
	  /// of the structured type and returns a Java array containing
	  /// the result.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> an array containing the ordered attribute values </returns>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object[] getAttributes() throws SQLException;
	  Object[] Attributes {get;}

	  /// <summary>
	  /// Produces the ordered values of the attributes of the SQL
	  /// structured type that this <code>Struct</code> object represents.
	  ///  As individual attributes are processed, this method uses the given type map
	  /// for customizations of the type mappings.
	  /// If there is no
	  /// entry in the given type map that matches the structured
	  /// type that an attribute represents,
	  /// the driver uses the standard mapping. This method never
	  /// uses the type map associated with the connection.
	  /// <para>
	  /// Conceptually, this method calls the method
	  /// <code>getObject</code> on each attribute
	  /// of the structured type and returns a Java array containing
	  /// the result.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="map"> a mapping of SQL type names to Java classes </param>
	  /// <returns> an array containing the ordered attribute values </returns>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object[] getAttributes(java.util.Map<String,Class> map) throws SQLException;
	  Object[] GetAttributes(IDictionary<String, Class> map);
	}

}