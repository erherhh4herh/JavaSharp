/*
 * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	/// An object that can be used to get information about the types
	/// and properties for each parameter marker in a
	/// <code>PreparedStatement</code> object. For some queries and driver
	/// implementations, the data that would be returned by a <code>ParameterMetaData</code>
	/// object may not be available until the <code>PreparedStatement</code> has
	/// been executed.
	/// <para>
	/// Some driver implementations may not be able to provide information about the
	/// types and properties for each parameter marker in a <code>CallableStatement</code>
	/// object.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	public interface ParameterMetaData : Wrapper
	{

		/// <summary>
		/// Retrieves the number of parameters in the <code>PreparedStatement</code>
		/// object for which this <code>ParameterMetaData</code> object contains
		/// information.
		/// </summary>
		/// <returns> the number of parameters </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getParameterCount() throws SQLException;
		int ParameterCount {get;}

		/// <summary>
		/// Retrieves whether null values are allowed in the designated parameter.
		/// </summary>
		/// <param name="param"> the first parameter is 1, the second is 2, ... </param>
		/// <returns> the nullability status of the given parameter; one of
		///        <code>ParameterMetaData.parameterNoNulls</code>,
		///        <code>ParameterMetaData.parameterNullable</code>, or
		///        <code>ParameterMetaData.parameterNullableUnknown</code> </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int isNullable(int param) throws SQLException;
		int IsNullable(int param);

		/// <summary>
		/// The constant indicating that a
		/// parameter will not allow <code>NULL</code> values.
		/// </summary>

		/// <summary>
		/// The constant indicating that a
		/// parameter will allow <code>NULL</code> values.
		/// </summary>

		/// <summary>
		/// The constant indicating that the
		/// nullability of a parameter is unknown.
		/// </summary>

		/// <summary>
		/// Retrieves whether values for the designated parameter can be signed numbers.
		/// </summary>
		/// <param name="param"> the first parameter is 1, the second is 2, ... </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isSigned(int param) throws SQLException;
		bool IsSigned(int param);

		/// <summary>
		/// Retrieves the designated parameter's specified column size.
		/// 
		/// <P>The returned value represents the maximum column size for the given parameter.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. 0 is returned for data types where the
		/// column size is not applicable.
		/// </summary>
		/// <param name="param"> the first parameter is 1, the second is 2, ... </param>
		/// <returns> precision </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getPrecision(int param) throws SQLException;
		int GetPrecision(int param);

		/// <summary>
		/// Retrieves the designated parameter's number of digits to right of the decimal point.
		/// 0 is returned for data types where the scale is not applicable.
		/// </summary>
		/// <param name="param"> the first parameter is 1, the second is 2, ... </param>
		/// <returns> scale </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getScale(int param) throws SQLException;
		int GetScale(int param);

		/// <summary>
		/// Retrieves the designated parameter's SQL type.
		/// </summary>
		/// <param name="param"> the first parameter is 1, the second is 2, ... </param>
		/// <returns> SQL type from <code>java.sql.Types</code> </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
		/// <seealso cref= Types </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getParameterType(int param) throws SQLException;
		int GetParameterType(int param);

		/// <summary>
		/// Retrieves the designated parameter's database-specific type name.
		/// </summary>
		/// <param name="param"> the first parameter is 1, the second is 2, ... </param>
		/// <returns> type the name used by the database. If the parameter type is
		/// a user-defined type, then a fully-qualified type name is returned. </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getParameterTypeName(int param) throws SQLException;
		String GetParameterTypeName(int param);


		/// <summary>
		/// Retrieves the fully-qualified name of the Java class whose instances
		/// should be passed to the method <code>PreparedStatement.setObject</code>.
		/// </summary>
		/// <param name="param"> the first parameter is 1, the second is 2, ... </param>
		/// <returns> the fully-qualified name of the class in the Java programming
		///         language that would be used by the method
		///         <code>PreparedStatement.setObject</code> to set the value
		///         in the specified parameter. This is the class name used
		///         for custom mapping. </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getParameterClassName(int param) throws SQLException;
		String GetParameterClassName(int param);

		/// <summary>
		/// The constant indicating that the mode of the parameter is unknown.
		/// </summary>

		/// <summary>
		/// The constant indicating that the parameter's mode is IN.
		/// </summary>

		/// <summary>
		/// The constant indicating that the parameter's mode is INOUT.
		/// </summary>

		/// <summary>
		/// The constant indicating that the parameter's mode is  OUT.
		/// </summary>

		/// <summary>
		/// Retrieves the designated parameter's mode.
		/// </summary>
		/// <param name="param"> the first parameter is 1, the second is 2, ... </param>
		/// <returns> mode of the parameter; one of
		///        <code>ParameterMetaData.parameterModeIn</code>,
		///        <code>ParameterMetaData.parameterModeOut</code>, or
		///        <code>ParameterMetaData.parameterModeInOut</code>
		///        <code>ParameterMetaData.parameterModeUnknown</code>. </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getParameterMode(int param) throws SQLException;
		int GetParameterMode(int param);
	}

	public static class ParameterMetaData_Fields
	{
		public const int ParameterNoNulls = 0;
		public const int ParameterNullable = 1;
		public const int ParameterNullableUnknown = 2;
		public const int ParameterModeUnknown = 0;
		public const int ParameterModeIn = 1;
		public const int ParameterModeInOut = 2;
		public const int ParameterModeOut = 4;
	}

}