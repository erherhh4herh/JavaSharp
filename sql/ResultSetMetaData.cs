/*
 * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
	/// and properties of the columns in a <code>ResultSet</code> object.
	/// The following code fragment creates the <code>ResultSet</code> object rs,
	/// creates the <code>ResultSetMetaData</code> object rsmd, and uses rsmd
	/// to find out how many columns rs has and whether the first column in rs
	/// can be used in a <code>WHERE</code> clause.
	/// <PRE>
	/// 
	///     ResultSet rs = stmt.executeQuery("SELECT a, b, c FROM TABLE2");
	///     ResultSetMetaData rsmd = rs.getMetaData();
	///     int numberOfColumns = rsmd.getColumnCount();
	///     boolean b = rsmd.isSearchable(1);
	/// 
	/// </PRE>
	/// </summary>

	public interface ResultSetMetaData : Wrapper
	{

		/// <summary>
		/// Returns the number of columns in this <code>ResultSet</code> object.
		/// </summary>
		/// <returns> the number of columns </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getColumnCount() throws SQLException;
		int ColumnCount {get;}

		/// <summary>
		/// Indicates whether the designated column is automatically numbered.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isAutoIncrement(int column) throws SQLException;
		bool IsAutoIncrement(int column);

		/// <summary>
		/// Indicates whether a column's case matters.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isCaseSensitive(int column) throws SQLException;
		bool IsCaseSensitive(int column);

		/// <summary>
		/// Indicates whether the designated column can be used in a where clause.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isSearchable(int column) throws SQLException;
		bool IsSearchable(int column);

		/// <summary>
		/// Indicates whether the designated column is a cash value.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isCurrency(int column) throws SQLException;
		bool IsCurrency(int column);

		/// <summary>
		/// Indicates the nullability of values in the designated column.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> the nullability status of the given column; one of <code>columnNoNulls</code>,
		///          <code>columnNullable</code> or <code>columnNullableUnknown</code> </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int isNullable(int column) throws SQLException;
		int IsNullable(int column);

		/// <summary>
		/// The constant indicating that a
		/// column does not allow <code>NULL</code> values.
		/// </summary>

		/// <summary>
		/// The constant indicating that a
		/// column allows <code>NULL</code> values.
		/// </summary>

		/// <summary>
		/// The constant indicating that the
		/// nullability of a column's values is unknown.
		/// </summary>

		/// <summary>
		/// Indicates whether values in the designated column are signed numbers.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isSigned(int column) throws SQLException;
		bool IsSigned(int column);

		/// <summary>
		/// Indicates the designated column's normal maximum width in characters.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> the normal maximum number of characters allowed as the width
		///          of the designated column </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getColumnDisplaySize(int column) throws SQLException;
		int GetColumnDisplaySize(int column);

		/// <summary>
		/// Gets the designated column's suggested title for use in printouts and
		/// displays. The suggested title is usually specified by the SQL <code>AS</code>
		/// clause.  If a SQL <code>AS</code> is not specified, the value returned from
		/// <code>getColumnLabel</code> will be the same as the value returned by the
		/// <code>getColumnName</code> method.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> the suggested column title </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getColumnLabel(int column) throws SQLException;
		String GetColumnLabel(int column);

		/// <summary>
		/// Get the designated column's name.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> column name </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getColumnName(int column) throws SQLException;
		String GetColumnName(int column);

		/// <summary>
		/// Get the designated column's table's schema.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> schema name or "" if not applicable </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getSchemaName(int column) throws SQLException;
		String GetSchemaName(int column);

		/// <summary>
		/// Get the designated column's specified column size.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. 0 is returned for data types where the
		/// column size is not applicable.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> precision </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getPrecision(int column) throws SQLException;
		int GetPrecision(int column);

		/// <summary>
		/// Gets the designated column's number of digits to right of the decimal point.
		/// 0 is returned for data types where the scale is not applicable.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> scale </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getScale(int column) throws SQLException;
		int GetScale(int column);

		/// <summary>
		/// Gets the designated column's table name.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> table name or "" if not applicable </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getTableName(int column) throws SQLException;
		String GetTableName(int column);

		/// <summary>
		/// Gets the designated column's table's catalog name.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> the name of the catalog for the table in which the given column
		///          appears or "" if not applicable </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getCatalogName(int column) throws SQLException;
		String GetCatalogName(int column);

		/// <summary>
		/// Retrieves the designated column's SQL type.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> SQL type from java.sql.Types </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= Types </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getColumnType(int column) throws SQLException;
		int GetColumnType(int column);

		/// <summary>
		/// Retrieves the designated column's database-specific type name.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> type name used by the database. If the column type is
		/// a user-defined type, then a fully-qualified type name is returned. </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getColumnTypeName(int column) throws SQLException;
		String GetColumnTypeName(int column);

		/// <summary>
		/// Indicates whether the designated column is definitely not writable.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isReadOnly(int column) throws SQLException;
		bool IsReadOnly(int column);

		/// <summary>
		/// Indicates whether it is possible for a write on the designated column to succeed.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isWritable(int column) throws SQLException;
		bool IsWritable(int column);

		/// <summary>
		/// Indicates whether a write on the designated column will definitely succeed.
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isDefinitelyWritable(int column) throws SQLException;
		bool IsDefinitelyWritable(int column);

		//--------------------------JDBC 2.0-----------------------------------

		/// <summary>
		/// <para>Returns the fully-qualified name of the Java class whose instances
		/// are manufactured if the method <code>ResultSet.getObject</code>
		/// is called to retrieve a value
		/// from the column.  <code>ResultSet.getObject</code> may return a subclass of the
		/// class returned by this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="column"> the first column is 1, the second is 2, ... </param>
		/// <returns> the fully-qualified name of the class in the Java programming
		///         language that would be used by the method
		/// <code>ResultSet.getObject</code> to retrieve the value in the specified
		/// column. This is the class name used for custom mapping. </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getColumnClassName(int column) throws SQLException;
		String GetColumnClassName(int column);
	}

	public static class ResultSetMetaData_Fields
	{
		public const int ColumnNoNulls = 0;
		public const int ColumnNullable = 1;
		public const int ColumnNullableUnknown = 2;
	}

}