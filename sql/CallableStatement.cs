using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The interface used to execute SQL stored procedures.  The JDBC API
	/// provides a stored procedure SQL escape syntax that allows stored procedures
	/// to be called in a standard way for all RDBMSs. This escape syntax has one
	/// form that includes a result parameter and one that does not. If used, the result
	/// parameter must be registered as an OUT parameter. The other parameters
	/// can be used for input, output or both. Parameters are referred to
	/// sequentially, by number, with the first parameter being 1.
	/// <PRE>
	///   {?= call &lt;procedure-name&gt;[(&lt;arg1&gt;,&lt;arg2&gt;, ...)]}
	///   {call &lt;procedure-name&gt;[(&lt;arg1&gt;,&lt;arg2&gt;, ...)]}
	/// </PRE>
	/// <P>
	/// IN parameter values are set using the <code>set</code> methods inherited from
	/// <seealso cref="PreparedStatement"/>.  The type of all OUT parameters must be
	/// registered prior to executing the stored procedure; their values
	/// are retrieved after execution via the <code>get</code> methods provided here.
	/// <P>
	/// A <code>CallableStatement</code> can return one <seealso cref="ResultSet"/> object or
	/// multiple <code>ResultSet</code> objects.  Multiple
	/// <code>ResultSet</code> objects are handled using operations
	/// inherited from <seealso cref="Statement"/>.
	/// <P>
	/// For maximum portability, a call's <code>ResultSet</code> objects and
	/// update counts should be processed prior to getting the values of output
	/// parameters.
	/// 
	/// </summary>
	/// <seealso cref= Connection#prepareCall </seealso>
	/// <seealso cref= ResultSet </seealso>

	public interface CallableStatement : PreparedStatement
	{

		/// <summary>
		/// Registers the OUT parameter in ordinal position
		/// <code>parameterIndex</code> to the JDBC type
		/// <code>sqlType</code>.  All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// <para>
		/// The JDBC type specified by <code>sqlType</code> for an OUT
		/// parameter determines the Java type that must be used
		/// in the <code>get</code> method to read the value of that parameter.
		/// </para>
		/// <para>
		/// If the JDBC type expected to be returned to this output parameter
		/// is specific to this particular database, <code>sqlType</code>
		/// should be <code>java.sql.Types.OTHER</code>.  The method
		/// <seealso cref="#getObject"/> retrieves the value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <param name="sqlType"> the JDBC type code defined by <code>java.sql.Types</code>.
		///        If the parameter is of JDBC type <code>NUMERIC</code>
		///        or <code>DECIMAL</code>, the version of
		///        <code>registerOutParameter</code> that accepts a scale value
		///        should be used.
		/// </param>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if <code>sqlType</code> is
		/// a <code>ARRAY</code>, <code>BLOB</code>, <code>CLOB</code>,
		/// <code>DATALINK</code>, <code>JAVA_OBJECT</code>, <code>NCHAR</code>,
		/// <code>NCLOB</code>, <code>NVARCHAR</code>, <code>LONGNVARCHAR</code>,
		///  <code>REF</code>, <code>ROWID</code>, <code>SQLXML</code>
		/// or  <code>STRUCT</code> data type and the JDBC driver does not support
		/// this data type </exception>
		/// <seealso cref= Types </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void registerOutParameter(int parameterIndex, int sqlType) throws SQLException;
		void RegisterOutParameter(int parameterIndex, int sqlType);

		/// <summary>
		/// Registers the parameter in ordinal position
		/// <code>parameterIndex</code> to be of JDBC type
		/// <code>sqlType</code>. All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// <para>
		/// The JDBC type specified by <code>sqlType</code> for an OUT
		/// parameter determines the Java type that must be used
		/// in the <code>get</code> method to read the value of that parameter.
		/// </para>
		/// <para>
		/// This version of <code>registerOutParameter</code> should be
		/// used when the parameter is of JDBC type <code>NUMERIC</code>
		/// or <code>DECIMAL</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <param name="sqlType"> the SQL type code defined by <code>java.sql.Types</code>. </param>
		/// <param name="scale"> the desired number of digits to the right of the
		/// decimal point.  It must be greater than or equal to zero. </param>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if <code>sqlType</code> is
		/// a <code>ARRAY</code>, <code>BLOB</code>, <code>CLOB</code>,
		/// <code>DATALINK</code>, <code>JAVA_OBJECT</code>, <code>NCHAR</code>,
		/// <code>NCLOB</code>, <code>NVARCHAR</code>, <code>LONGNVARCHAR</code>,
		///  <code>REF</code>, <code>ROWID</code>, <code>SQLXML</code>
		/// or  <code>STRUCT</code> data type and the JDBC driver does not support
		/// this data type </exception>
		/// <seealso cref= Types </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void registerOutParameter(int parameterIndex, int sqlType, int scale) throws SQLException;
		void RegisterOutParameter(int parameterIndex, int sqlType, int scale);

		/// <summary>
		/// Retrieves whether the last OUT parameter read had the value of
		/// SQL <code>NULL</code>.  Note that this method should be called only after
		/// calling a getter method; otherwise, there is no value to use in
		/// determining whether it is <code>null</code> or not.
		/// </summary>
		/// <returns> <code>true</code> if the last parameter read was SQL
		/// <code>NULL</code>; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean wasNull() throws SQLException;
		bool WasNull();

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>CHAR</code>,
		/// <code>VARCHAR</code>, or <code>LONGVARCHAR</code> parameter as a
		/// <code>String</code> in the Java programming language.
		/// <para>
		/// For the fixed-length type JDBC <code>CHAR</code>,
		/// the <code>String</code> object
		/// returned has exactly the same value the SQL
		/// <code>CHAR</code> value had in the
		/// database, including any padding added by the database.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <returns> the parameter value. If the value is SQL <code>NULL</code>,
		///         the result
		///         is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setString </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getString(int parameterIndex) throws SQLException;
		String GetString(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>BIT</code>
		/// or <code>BOOLEAN</code> parameter as a
		/// <code>boolean</code> in the Java programming language.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>,
		///         the result is <code>false</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setBoolean </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean getBoolean(int parameterIndex) throws SQLException;
		bool GetBoolean(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>TINYINT</code> parameter
		/// as a <code>byte</code> in the Java programming language.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setByte </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: byte getByte(int parameterIndex) throws SQLException;
		sbyte GetByte(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>SMALLINT</code> parameter
		/// as a <code>short</code> in the Java programming language.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setShort </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: short getShort(int parameterIndex) throws SQLException;
		short GetShort(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>INTEGER</code> parameter
		/// as an <code>int</code> in the Java programming language.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setInt </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getInt(int parameterIndex) throws SQLException;
		int GetInt(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>BIGINT</code> parameter
		/// as a <code>long</code> in the Java programming language.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setLong </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long getLong(int parameterIndex) throws SQLException;
		long GetLong(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>FLOAT</code> parameter
		/// as a <code>float</code> in the Java programming language.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		///         is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setFloat </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: float getFloat(int parameterIndex) throws SQLException;
		float GetFloat(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>DOUBLE</code> parameter as a <code>double</code>
		/// in the Java programming language. </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		///         is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setDouble </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double getDouble(int parameterIndex) throws SQLException;
		double GetDouble(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>NUMERIC</code> parameter as a
		/// <code>java.math.BigDecimal</code> object with <i>scale</i> digits to
		/// the right of the decimal point. </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <param name="scale"> the number of digits to the right of the decimal point </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		///         is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// @deprecated use <code>getBigDecimal(int parameterIndex)</code>
		///             or <code>getBigDecimal(String parameterName)</code> 
		/// <seealso cref= #setBigDecimal </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use <code>getBigDecimal(int parameterIndex)</code>") java.math.BigDecimal getBigDecimal(int parameterIndex, int scale) throws SQLException;
		[Obsolete("use <code>getBigDecimal(int parameterIndex)</code>")]
		decimal GetBigDecimal(int parameterIndex, int scale);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>BINARY</code> or
		/// <code>VARBINARY</code> parameter as an array of <code>byte</code>
		/// values in the Java programming language. </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		///         is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setBytes </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: byte[] getBytes(int parameterIndex) throws SQLException;
		sbyte[] GetBytes(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>DATE</code> parameter as a
		/// <code>java.sql.Date</code> object. </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		///         is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setDate </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Date getDate(int parameterIndex) throws SQLException;
		java.sql.Date GetDate(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>TIME</code> parameter as a
		/// <code>java.sql.Time</code> object.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		///         is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setTime </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Time getTime(int parameterIndex) throws SQLException;
		java.sql.Time GetTime(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>TIMESTAMP</code> parameter as a
		/// <code>java.sql.Timestamp</code> object.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		///         is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setTimestamp </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Timestamp getTimestamp(int parameterIndex) throws SQLException;
		java.sql.Timestamp GetTimestamp(int parameterIndex);

		//----------------------------------------------------------------------
		// Advanced features:


		/// <summary>
		/// Retrieves the value of the designated parameter as an <code>Object</code>
		/// in the Java programming language. If the value is an SQL <code>NULL</code>,
		/// the driver returns a Java <code>null</code>.
		/// <para>
		/// This method returns a Java object whose type corresponds to the JDBC
		/// type that was registered for this parameter using the method
		/// <code>registerOutParameter</code>.  By registering the target JDBC
		/// type as <code>java.sql.Types.OTHER</code>, this method can be used
		/// to read database-specific abstract data types.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <returns> A <code>java.lang.Object</code> holding the OUT parameter value </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= Types </seealso>
		/// <seealso cref= #setObject </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object getObject(int parameterIndex) throws SQLException;
		Object GetObject(int parameterIndex);


		//--------------------------JDBC 2.0-----------------------------

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>NUMERIC</code> parameter as a
		/// <code>java.math.BigDecimal</code> object with as many digits to the
		/// right of the decimal point as the value contains. </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <returns> the parameter value in full precision.  If the value is
		/// SQL <code>NULL</code>, the result is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setBigDecimal
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.math.BigDecimal getBigDecimal(int parameterIndex) throws SQLException;
		decimal GetBigDecimal(int parameterIndex);

		/// <summary>
		/// Returns an object representing the value of OUT parameter
		/// <code>parameterIndex</code> and uses <code>map</code> for the custom
		/// mapping of the parameter value.
		/// <para>
		/// This method returns a Java object whose type corresponds to the
		/// JDBC type that was registered for this parameter using the method
		/// <code>registerOutParameter</code>.  By registering the target
		/// JDBC type as <code>java.sql.Types.OTHER</code>, this method can
		/// be used to read database-specific abstract data types.
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2, and so on </param>
		/// <param name="map"> the mapping from SQL type names to Java classes </param>
		/// <returns> a <code>java.lang.Object</code> holding the OUT parameter value </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setObject
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object getObject(int parameterIndex, java.util.Map<String,Class> map) throws SQLException;
		Object GetObject(int parameterIndex, IDictionary<String, Class> map);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>REF(&lt;structured-type&gt;)</code>
		/// parameter as a <seealso cref="java.sql.Ref"/> object in the Java programming language. </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <returns> the parameter value as a <code>Ref</code> object in the
		/// Java programming language.  If the value was SQL <code>NULL</code>, the value
		/// <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Ref getRef(int parameterIndex) throws SQLException;
		Ref GetRef(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>BLOB</code> parameter as a
		/// <seealso cref="java.sql.Blob"/> object in the Java programming language. </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2, and so on </param>
		/// <returns> the parameter value as a <code>Blob</code> object in the
		/// Java programming language.  If the value was SQL <code>NULL</code>, the value
		/// <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Blob getBlob(int parameterIndex) throws SQLException;
		Blob GetBlob(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>CLOB</code> parameter as a
		/// <code>java.sql.Clob</code> object in the Java programming language. </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2, and
		/// so on </param>
		/// <returns> the parameter value as a <code>Clob</code> object in the
		/// Java programming language.  If the value was SQL <code>NULL</code>, the
		/// value <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Clob getClob(int parameterIndex) throws SQLException;
		Clob GetClob(int parameterIndex);

		/// 
		/// <summary>
		/// Retrieves the value of the designated JDBC <code>ARRAY</code> parameter as an
		/// <seealso cref="java.sql.Array"/> object in the Java programming language. </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2, and
		/// so on </param>
		/// <returns> the parameter value as an <code>Array</code> object in
		/// the Java programming language.  If the value was SQL <code>NULL</code>, the
		/// value <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Array getArray(int parameterIndex) throws SQLException;
		Array GetArray(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>DATE</code> parameter as a
		/// <code>java.sql.Date</code> object, using
		/// the given <code>Calendar</code> object
		/// to construct the date.
		/// With a <code>Calendar</code> object, the driver
		/// can calculate the date taking into account a custom timezone and locale.
		/// If no <code>Calendar</code> object is specified, the driver uses the
		/// default timezone and locale.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <param name="cal"> the <code>Calendar</code> object the driver will use
		///            to construct the date </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		///         is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setDate
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Date getDate(int parameterIndex, java.util.Calendar cal) throws SQLException;
		java.sql.Date GetDate(int parameterIndex, DateTime cal);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>TIME</code> parameter as a
		/// <code>java.sql.Time</code> object, using
		/// the given <code>Calendar</code> object
		/// to construct the time.
		/// With a <code>Calendar</code> object, the driver
		/// can calculate the time taking into account a custom timezone and locale.
		/// If no <code>Calendar</code> object is specified, the driver uses the
		/// default timezone and locale.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <param name="cal"> the <code>Calendar</code> object the driver will use
		///            to construct the time </param>
		/// <returns> the parameter value; if the value is SQL <code>NULL</code>, the result
		///         is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setTime
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Time getTime(int parameterIndex, java.util.Calendar cal) throws SQLException;
		java.sql.Time GetTime(int parameterIndex, DateTime cal);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>TIMESTAMP</code> parameter as a
		/// <code>java.sql.Timestamp</code> object, using
		/// the given <code>Calendar</code> object to construct
		/// the <code>Timestamp</code> object.
		/// With a <code>Calendar</code> object, the driver
		/// can calculate the timestamp taking into account a custom timezone and locale.
		/// If no <code>Calendar</code> object is specified, the driver uses the
		/// default timezone and locale.
		/// 
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <param name="cal"> the <code>Calendar</code> object the driver will use
		///            to construct the timestamp </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		///         is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #setTimestamp
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Timestamp getTimestamp(int parameterIndex, java.util.Calendar cal) throws SQLException;
		java.sql.Timestamp GetTimestamp(int parameterIndex, DateTime cal);


		/// <summary>
		/// Registers the designated output parameter.
		/// This version of
		/// the method <code>registerOutParameter</code>
		/// should be used for a user-defined or <code>REF</code> output parameter.  Examples
		/// of user-defined types include: <code>STRUCT</code>, <code>DISTINCT</code>,
		/// <code>JAVA_OBJECT</code>, and named array types.
		/// <para>
		/// All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// </para>
		/// <para>  For a user-defined parameter, the fully-qualified SQL
		/// type name of the parameter should also be given, while a <code>REF</code>
		/// parameter requires that the fully-qualified type name of the
		/// referenced type be given.  A JDBC driver that does not need the
		/// type code and type name information may ignore it.   To be portable,
		/// however, applications should always provide these values for
		/// user-defined and <code>REF</code> parameters.
		/// 
		/// Although it is intended for user-defined and <code>REF</code> parameters,
		/// this method may be used to register a parameter of any JDBC type.
		/// If the parameter does not have a user-defined or <code>REF</code> type, the
		/// <i>typeName</i> parameter is ignored.
		/// 
		/// <P><B>Note:</B> When reading the value of an out parameter, you
		/// must use the getter method whose Java type corresponds to the
		/// parameter's registered SQL type.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,... </param>
		/// <param name="sqlType"> a value from <seealso cref="java.sql.Types"/> </param>
		/// <param name="typeName"> the fully-qualified name of an SQL structured type </param>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if <code>sqlType</code> is
		/// a <code>ARRAY</code>, <code>BLOB</code>, <code>CLOB</code>,
		/// <code>DATALINK</code>, <code>JAVA_OBJECT</code>, <code>NCHAR</code>,
		/// <code>NCLOB</code>, <code>NVARCHAR</code>, <code>LONGNVARCHAR</code>,
		///  <code>REF</code>, <code>ROWID</code>, <code>SQLXML</code>
		/// or  <code>STRUCT</code> data type and the JDBC driver does not support
		/// this data type </exception>
		/// <seealso cref= Types
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void registerOutParameter(int parameterIndex, int sqlType, String typeName) throws SQLException;
		void RegisterOutParameter(int parameterIndex, int sqlType, String typeName);

	  //--------------------------JDBC 3.0-----------------------------

		/// <summary>
		/// Registers the OUT parameter named
		/// <code>parameterName</code> to the JDBC type
		/// <code>sqlType</code>.  All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// <para>
		/// The JDBC type specified by <code>sqlType</code> for an OUT
		/// parameter determines the Java type that must be used
		/// in the <code>get</code> method to read the value of that parameter.
		/// </para>
		/// <para>
		/// If the JDBC type expected to be returned to this output parameter
		/// is specific to this particular database, <code>sqlType</code>
		/// should be <code>java.sql.Types.OTHER</code>.  The method
		/// <seealso cref="#getObject"/> retrieves the value.
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="sqlType"> the JDBC type code defined by <code>java.sql.Types</code>.
		/// If the parameter is of JDBC type <code>NUMERIC</code>
		/// or <code>DECIMAL</code>, the version of
		/// <code>registerOutParameter</code> that accepts a scale value
		/// should be used. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if <code>sqlType</code> is
		/// a <code>ARRAY</code>, <code>BLOB</code>, <code>CLOB</code>,
		/// <code>DATALINK</code>, <code>JAVA_OBJECT</code>, <code>NCHAR</code>,
		/// <code>NCLOB</code>, <code>NVARCHAR</code>, <code>LONGNVARCHAR</code>,
		///  <code>REF</code>, <code>ROWID</code>, <code>SQLXML</code>
		/// or  <code>STRUCT</code> data type and the JDBC driver does not support
		/// this data type or if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
		/// <seealso cref= Types </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void registerOutParameter(String parameterName, int sqlType) throws SQLException;
		void RegisterOutParameter(String parameterName, int sqlType);

		/// <summary>
		/// Registers the parameter named
		/// <code>parameterName</code> to be of JDBC type
		/// <code>sqlType</code>.  All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// <para>
		/// The JDBC type specified by <code>sqlType</code> for an OUT
		/// parameter determines the Java type that must be used
		/// in the <code>get</code> method to read the value of that parameter.
		/// </para>
		/// <para>
		/// This version of <code>registerOutParameter</code> should be
		/// used when the parameter is of JDBC type <code>NUMERIC</code>
		/// or <code>DECIMAL</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="sqlType"> SQL type code defined by <code>java.sql.Types</code>. </param>
		/// <param name="scale"> the desired number of digits to the right of the
		/// decimal point.  It must be greater than or equal to zero. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if <code>sqlType</code> is
		/// a <code>ARRAY</code>, <code>BLOB</code>, <code>CLOB</code>,
		/// <code>DATALINK</code>, <code>JAVA_OBJECT</code>, <code>NCHAR</code>,
		/// <code>NCLOB</code>, <code>NVARCHAR</code>, <code>LONGNVARCHAR</code>,
		///  <code>REF</code>, <code>ROWID</code>, <code>SQLXML</code>
		/// or  <code>STRUCT</code> data type and the JDBC driver does not support
		/// this data type or if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
		/// <seealso cref= Types </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void registerOutParameter(String parameterName, int sqlType, int scale) throws SQLException;
		void RegisterOutParameter(String parameterName, int sqlType, int scale);

		/// <summary>
		/// Registers the designated output parameter.  This version of
		/// the method <code>registerOutParameter</code>
		/// should be used for a user-named or REF output parameter.  Examples
		/// of user-named types include: STRUCT, DISTINCT, JAVA_OBJECT, and
		/// named array types.
		/// <para>
		/// All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// </para>
		/// <para>
		/// For a user-named parameter the fully-qualified SQL
		/// type name of the parameter should also be given, while a REF
		/// parameter requires that the fully-qualified type name of the
		/// referenced type be given.  A JDBC driver that does not need the
		/// type code and type name information may ignore it.   To be portable,
		/// however, applications should always provide these values for
		/// user-named and REF parameters.
		/// 
		/// Although it is intended for user-named and REF parameters,
		/// this method may be used to register a parameter of any JDBC type.
		/// If the parameter does not have a user-named or REF type, the
		/// typeName parameter is ignored.
		/// 
		/// <P><B>Note:</B> When reading the value of an out parameter, you
		/// must use the <code>getXXX</code> method whose Java type XXX corresponds to the
		/// parameter's registered SQL type.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="sqlType"> a value from <seealso cref="java.sql.Types"/> </param>
		/// <param name="typeName"> the fully-qualified name of an SQL structured type </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if <code>sqlType</code> is
		/// a <code>ARRAY</code>, <code>BLOB</code>, <code>CLOB</code>,
		/// <code>DATALINK</code>, <code>JAVA_OBJECT</code>, <code>NCHAR</code>,
		/// <code>NCLOB</code>, <code>NVARCHAR</code>, <code>LONGNVARCHAR</code>,
		///  <code>REF</code>, <code>ROWID</code>, <code>SQLXML</code>
		/// or  <code>STRUCT</code> data type and the JDBC driver does not support
		/// this data type or if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= Types
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void registerOutParameter(String parameterName, int sqlType, String typeName) throws SQLException;
		void RegisterOutParameter(String parameterName, int sqlType, String typeName);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>DATALINK</code> parameter as a
		/// <code>java.net.URL</code> object.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,... </param>
		/// <returns> a <code>java.net.URL</code> object that represents the
		///         JDBC <code>DATALINK</code> value used as the designated
		///         parameter </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs,
		/// this method is called on a closed <code>CallableStatement</code>,
		///            or if the URL being returned is
		///            not a valid URL on the Java platform </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setURL
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.net.URL getURL(int parameterIndex) throws SQLException;
		java.net.URL GetURL(int parameterIndex);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.net.URL</code> object.
		/// The driver converts this to an SQL <code>DATALINK</code> value when
		/// it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="val"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs;
		/// this method is called on a closed <code>CallableStatement</code>
		///            or if a URL is malformed </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getURL
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setURL(String parameterName, java.net.URL val) throws SQLException;
		void SetURL(String parameterName, java.net.URL val);

		/// <summary>
		/// Sets the designated parameter to SQL <code>NULL</code>.
		/// 
		/// <P><B>Note:</B> You must specify the parameter's SQL type.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="sqlType"> the SQL type code defined in <code>java.sql.Types</code> </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setNull(String parameterName, int sqlType) throws SQLException;
		void SetNull(String parameterName, int sqlType);

		/// <summary>
		/// Sets the designated parameter to the given Java <code>boolean</code> value.
		/// The driver converts this
		/// to an SQL <code>BIT</code> or <code>BOOLEAN</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <seealso cref= #getBoolean </seealso>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setBoolean(String parameterName, boolean x) throws SQLException;
		void SetBoolean(String parameterName, bool x);

		/// <summary>
		/// Sets the designated parameter to the given Java <code>byte</code> value.
		/// The driver converts this
		/// to an SQL <code>TINYINT</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getByte
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setByte(String parameterName, byte x) throws SQLException;
		void SetByte(String parameterName, sbyte x);

		/// <summary>
		/// Sets the designated parameter to the given Java <code>short</code> value.
		/// The driver converts this
		/// to an SQL <code>SMALLINT</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getShort
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setShort(String parameterName, short x) throws SQLException;
		void SetShort(String parameterName, short x);

		/// <summary>
		/// Sets the designated parameter to the given Java <code>int</code> value.
		/// The driver converts this
		/// to an SQL <code>INTEGER</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getInt
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setInt(String parameterName, int x) throws SQLException;
		void SetInt(String parameterName, int x);

		/// <summary>
		/// Sets the designated parameter to the given Java <code>long</code> value.
		/// The driver converts this
		/// to an SQL <code>BIGINT</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getLong
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setLong(String parameterName, long x) throws SQLException;
		void SetLong(String parameterName, long x);

		/// <summary>
		/// Sets the designated parameter to the given Java <code>float</code> value.
		/// The driver converts this
		/// to an SQL <code>FLOAT</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getFloat
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setFloat(String parameterName, float x) throws SQLException;
		void SetFloat(String parameterName, float x);

		/// <summary>
		/// Sets the designated parameter to the given Java <code>double</code> value.
		/// The driver converts this
		/// to an SQL <code>DOUBLE</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getDouble
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setDouble(String parameterName, double x) throws SQLException;
		void SetDouble(String parameterName, double x);

		/// <summary>
		/// Sets the designated parameter to the given
		/// <code>java.math.BigDecimal</code> value.
		/// The driver converts this to an SQL <code>NUMERIC</code> value when
		/// it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getBigDecimal
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setBigDecimal(String parameterName, java.math.BigDecimal x) throws SQLException;
		void SetBigDecimal(String parameterName, decimal x);

		/// <summary>
		/// Sets the designated parameter to the given Java <code>String</code> value.
		/// The driver converts this
		/// to an SQL <code>VARCHAR</code> or <code>LONGVARCHAR</code> value
		/// (depending on the argument's
		/// size relative to the driver's limits on <code>VARCHAR</code> values)
		/// when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getString
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setString(String parameterName, String x) throws SQLException;
		void SetString(String parameterName, String x);

		/// <summary>
		/// Sets the designated parameter to the given Java array of bytes.
		/// The driver converts this to an SQL <code>VARBINARY</code> or
		/// <code>LONGVARBINARY</code> (depending on the argument's size relative
		/// to the driver's limits on <code>VARBINARY</code> values) when it sends
		/// it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getBytes
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setBytes(String parameterName, byte x[]) throws SQLException;
		void SetBytes(String parameterName, sbyte[] x);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.sql.Date</code> value
		/// using the default time zone of the virtual machine that is running
		/// the application.
		/// The driver converts this
		/// to an SQL <code>DATE</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getDate
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setDate(String parameterName, java.sql.Date x) throws SQLException;
		void SetDate(String parameterName, java.sql.Date x);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.sql.Time</code> value.
		/// The driver converts this
		/// to an SQL <code>TIME</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getTime
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setTime(String parameterName, java.sql.Time x) throws SQLException;
		void SetTime(String parameterName, java.sql.Time x);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.sql.Timestamp</code> value.
		/// The driver
		/// converts this to an SQL <code>TIMESTAMP</code> value when it sends it to the
		/// database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getTimestamp
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setTimestamp(String parameterName, java.sql.Timestamp x) throws SQLException;
		void SetTimestamp(String parameterName, java.sql.Timestamp x);

		/// <summary>
		/// Sets the designated parameter to the given input stream, which will have
		/// the specified number of bytes.
		/// When a very large ASCII value is input to a <code>LONGVARCHAR</code>
		/// parameter, it may be more practical to send it via a
		/// <code>java.io.InputStream</code>. Data will be read from the stream
		/// as needed until end-of-file is reached.  The JDBC driver will
		/// do any necessary conversion from ASCII to the database char format.
		/// 
		/// <P><B>Note:</B> This stream object can either be a standard
		/// Java stream object or your own subclass that implements the
		/// standard interface.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the Java input stream that contains the ASCII parameter value </param>
		/// <param name="length"> the number of bytes in the stream </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setAsciiStream(String parameterName, java.io.InputStream x, int length) throws SQLException;
		void SetAsciiStream(String parameterName, InputStream x, int length);

		/// <summary>
		/// Sets the designated parameter to the given input stream, which will have
		/// the specified number of bytes.
		/// When a very large binary value is input to a <code>LONGVARBINARY</code>
		/// parameter, it may be more practical to send it via a
		/// <code>java.io.InputStream</code> object. The data will be read from the stream
		/// as needed until end-of-file is reached.
		/// 
		/// <P><B>Note:</B> This stream object can either be a standard
		/// Java stream object or your own subclass that implements the
		/// standard interface.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the java input stream which contains the binary parameter value </param>
		/// <param name="length"> the number of bytes in the stream </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setBinaryStream(String parameterName, java.io.InputStream x, int length) throws SQLException;
		void SetBinaryStream(String parameterName, InputStream x, int length);

		/// <summary>
		/// Sets the value of the designated parameter with the given object.
		/// 
		/// <para>The given Java object will be converted to the given targetSqlType
		/// before being sent to the database.
		/// 
		/// If the object has a custom mapping (is of a class implementing the
		/// interface <code>SQLData</code>),
		/// the JDBC driver should call the method <code>SQLData.writeSQL</code> to write it
		/// to the SQL data stream.
		/// If, on the other hand, the object is of a class implementing
		/// <code>Ref</code>, <code>Blob</code>, <code>Clob</code>,  <code>NClob</code>,
		///  <code>Struct</code>, <code>java.net.URL</code>,
		/// or <code>Array</code>, the driver should pass it to the database as a
		/// value of the corresponding SQL type.
		/// <P>
		/// Note that this method may be used to pass datatabase-
		/// specific abstract data types.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the object containing the input parameter value </param>
		/// <param name="targetSqlType"> the SQL type (as defined in java.sql.Types) to be
		/// sent to the database. The scale argument may further qualify this type. </param>
		/// <param name="scale"> for java.sql.Types.DECIMAL or java.sql.Types.NUMERIC types,
		///          this is the number of digits after the decimal point.  For all other
		///          types, this value will be ignored. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if
		/// the JDBC driver does not support the specified targetSqlType </exception>
		/// <seealso cref= Types </seealso>
		/// <seealso cref= #getObject
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setObject(String parameterName, Object x, int targetSqlType, int scale) throws SQLException;
		void SetObject(String parameterName, Object x, int targetSqlType, int scale);

		/// <summary>
		/// Sets the value of the designated parameter with the given object.
		/// 
		/// This method is similar to {@link #setObject(String parameterName,
		/// Object x, int targetSqlType, int scaleOrLength)},
		/// except that it assumes a scale of zero.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the object containing the input parameter value </param>
		/// <param name="targetSqlType"> the SQL type (as defined in java.sql.Types) to be
		///                      sent to the database </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if
		/// the JDBC driver does not support the specified targetSqlType </exception>
		/// <seealso cref= #getObject
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setObject(String parameterName, Object x, int targetSqlType) throws SQLException;
		void SetObject(String parameterName, Object x, int targetSqlType);

		/// <summary>
		/// Sets the value of the designated parameter with the given object.
		/// 
		/// <para>The JDBC specification specifies a standard mapping from
		/// Java <code>Object</code> types to SQL types.  The given argument
		/// will be converted to the corresponding SQL type before being
		/// sent to the database.
		/// </para>
		/// <para>Note that this method may be used to pass datatabase-
		/// specific abstract data types, by using a driver-specific Java
		/// type.
		/// 
		/// If the object is of a class implementing the interface <code>SQLData</code>,
		/// the JDBC driver should call the method <code>SQLData.writeSQL</code>
		/// to write it to the SQL data stream.
		/// If, on the other hand, the object is of a class implementing
		/// <code>Ref</code>, <code>Blob</code>, <code>Clob</code>,  <code>NClob</code>,
		///  <code>Struct</code>, <code>java.net.URL</code>,
		/// or <code>Array</code>, the driver should pass it to the database as a
		/// value of the corresponding SQL type.
		/// <P>
		/// This method throws an exception if there is an ambiguity, for example, if the
		/// object is of a class implementing more than one of the interfaces named above.
		/// </para>
		/// <para>
		/// <b>Note:</b> Not all databases allow for a non-typed Null to be sent to
		/// the backend. For maximum portability, the <code>setNull</code> or the
		/// <code>setObject(String parameterName, Object x, int sqlType)</code>
		/// method should be used
		/// instead of <code>setObject(String parameterName, Object x)</code>.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the object containing the input parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs,
		/// this method is called on a closed <code>CallableStatement</code> or if the given
		///            <code>Object</code> parameter is ambiguous </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getObject
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setObject(String parameterName, Object x) throws SQLException;
		void SetObject(String parameterName, Object x);


		/// <summary>
		/// Sets the designated parameter to the given <code>Reader</code>
		/// object, which is the given number of characters long.
		/// When a very large UNICODE value is input to a <code>LONGVARCHAR</code>
		/// parameter, it may be more practical to send it via a
		/// <code>java.io.Reader</code> object. The data will be read from the stream
		/// as needed until end-of-file is reached.  The JDBC driver will
		/// do any necessary conversion from UNICODE to the database char format.
		/// 
		/// <P><B>Note:</B> This stream object can either be a standard
		/// Java stream object or your own subclass that implements the
		/// standard interface.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="reader"> the <code>java.io.Reader</code> object that
		///        contains the UNICODE data used as the designated parameter </param>
		/// <param name="length"> the number of characters in the stream </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setCharacterStream(String parameterName, java.io.Reader reader, int length) throws SQLException;
		void SetCharacterStream(String parameterName, Reader reader, int length);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.sql.Date</code> value,
		/// using the given <code>Calendar</code> object.  The driver uses
		/// the <code>Calendar</code> object to construct an SQL <code>DATE</code> value,
		/// which the driver then sends to the database.  With a
		/// a <code>Calendar</code> object, the driver can calculate the date
		/// taking into account a custom timezone.  If no
		/// <code>Calendar</code> object is specified, the driver uses the default
		/// timezone, which is that of the virtual machine running the application.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <param name="cal"> the <code>Calendar</code> object the driver will use
		///            to construct the date </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getDate
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setDate(String parameterName, java.sql.Date x, java.util.Calendar cal) throws SQLException;
		void SetDate(String parameterName, java.sql.Date x, DateTime cal);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.sql.Time</code> value,
		/// using the given <code>Calendar</code> object.  The driver uses
		/// the <code>Calendar</code> object to construct an SQL <code>TIME</code> value,
		/// which the driver then sends to the database.  With a
		/// a <code>Calendar</code> object, the driver can calculate the time
		/// taking into account a custom timezone.  If no
		/// <code>Calendar</code> object is specified, the driver uses the default
		/// timezone, which is that of the virtual machine running the application.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <param name="cal"> the <code>Calendar</code> object the driver will use
		///            to construct the time </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getTime
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setTime(String parameterName, java.sql.Time x, java.util.Calendar cal) throws SQLException;
		void SetTime(String parameterName, java.sql.Time x, DateTime cal);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.sql.Timestamp</code> value,
		/// using the given <code>Calendar</code> object.  The driver uses
		/// the <code>Calendar</code> object to construct an SQL <code>TIMESTAMP</code> value,
		/// which the driver then sends to the database.  With a
		/// a <code>Calendar</code> object, the driver can calculate the timestamp
		/// taking into account a custom timezone.  If no
		/// <code>Calendar</code> object is specified, the driver uses the default
		/// timezone, which is that of the virtual machine running the application.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the parameter value </param>
		/// <param name="cal"> the <code>Calendar</code> object the driver will use
		///            to construct the timestamp </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #getTimestamp
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setTimestamp(String parameterName, java.sql.Timestamp x, java.util.Calendar cal) throws SQLException;
		void SetTimestamp(String parameterName, java.sql.Timestamp x, DateTime cal);

		/// <summary>
		/// Sets the designated parameter to SQL <code>NULL</code>.
		/// This version of the method <code>setNull</code> should
		/// be used for user-defined types and REF type parameters.  Examples
		/// of user-defined types include: STRUCT, DISTINCT, JAVA_OBJECT, and
		/// named array types.
		/// 
		/// <P><B>Note:</B> To be portable, applications must give the
		/// SQL type code and the fully-qualified SQL type name when specifying
		/// a NULL user-defined or REF parameter.  In the case of a user-defined type
		/// the name is the type name of the parameter itself.  For a REF
		/// parameter, the name is the type name of the referenced type.
		/// <para>
		/// Although it is intended for user-defined and Ref parameters,
		/// this method may be used to set a null parameter of any JDBC type.
		/// If the parameter does not have a user-defined or REF type, the given
		/// typeName is ignored.
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="sqlType"> a value from <code>java.sql.Types</code> </param>
		/// <param name="typeName"> the fully-qualified name of an SQL user-defined type;
		///        ignored if the parameter is not a user-defined type or
		///        SQL <code>REF</code> value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setNull(String parameterName, int sqlType, String typeName) throws SQLException;
		void SetNull(String parameterName, int sqlType, String typeName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>CHAR</code>, <code>VARCHAR</code>,
		/// or <code>LONGVARCHAR</code> parameter as a <code>String</code> in
		/// the Java programming language.
		/// <para>
		/// For the fixed-length type JDBC <code>CHAR</code>,
		/// the <code>String</code> object
		/// returned has exactly the same value the SQL
		/// <code>CHAR</code> value had in the
		/// database, including any padding added by the database.
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value. If the value is SQL <code>NULL</code>, the result
		/// is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setString
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getString(String parameterName) throws SQLException;
		String GetString(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>BIT</code> or <code>BOOLEAN</code>
		/// parameter as a
		/// <code>boolean</code> in the Java programming language. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>false</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setBoolean
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean getBoolean(String parameterName) throws SQLException;
		bool GetBoolean(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>TINYINT</code> parameter as a <code>byte</code>
		/// in the Java programming language. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setByte
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: byte getByte(String parameterName) throws SQLException;
		sbyte GetByte(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>SMALLINT</code> parameter as a <code>short</code>
		/// in the Java programming language. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setShort
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: short getShort(String parameterName) throws SQLException;
		short GetShort(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>INTEGER</code> parameter as an <code>int</code>
		/// in the Java programming language.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>,
		///         the result is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setInt
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getInt(String parameterName) throws SQLException;
		int GetInt(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>BIGINT</code> parameter as a <code>long</code>
		/// in the Java programming language.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>,
		///         the result is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setLong
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long getLong(String parameterName) throws SQLException;
		long GetLong(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>FLOAT</code> parameter as a <code>float</code>
		/// in the Java programming language. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>,
		///         the result is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setFloat
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: float getFloat(String parameterName) throws SQLException;
		float GetFloat(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>DOUBLE</code> parameter as a <code>double</code>
		/// in the Java programming language. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>,
		///         the result is <code>0</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setDouble
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double getDouble(String parameterName) throws SQLException;
		double GetDouble(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>BINARY</code> or <code>VARBINARY</code>
		/// parameter as an array of <code>byte</code> values in the Java
		/// programming language. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result is
		///  <code>null</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setBytes
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: byte[] getBytes(String parameterName) throws SQLException;
		sbyte[] GetBytes(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>DATE</code> parameter as a
		/// <code>java.sql.Date</code> object. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setDate
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Date getDate(String parameterName) throws SQLException;
		java.sql.Date GetDate(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>TIME</code> parameter as a
		/// <code>java.sql.Time</code> object. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setTime
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Time getTime(String parameterName) throws SQLException;
		java.sql.Time GetTime(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>TIMESTAMP</code> parameter as a
		/// <code>java.sql.Timestamp</code> object. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result
		/// is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setTimestamp
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Timestamp getTimestamp(String parameterName) throws SQLException;
		java.sql.Timestamp GetTimestamp(String parameterName);

		/// <summary>
		/// Retrieves the value of a parameter as an <code>Object</code> in the Java
		/// programming language. If the value is an SQL <code>NULL</code>, the
		/// driver returns a Java <code>null</code>.
		/// <para>
		/// This method returns a Java object whose type corresponds to the JDBC
		/// type that was registered for this parameter using the method
		/// <code>registerOutParameter</code>.  By registering the target JDBC
		/// type as <code>java.sql.Types.OTHER</code>, this method can be used
		/// to read database-specific abstract data types.
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> A <code>java.lang.Object</code> holding the OUT parameter value. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= Types </seealso>
		/// <seealso cref= #setObject
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object getObject(String parameterName) throws SQLException;
		Object GetObject(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>NUMERIC</code> parameter as a
		/// <code>java.math.BigDecimal</code> object with as many digits to the
		/// right of the decimal point as the value contains. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value in full precision.  If the value is
		/// SQL <code>NULL</code>, the result is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter;  if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setBigDecimal
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.math.BigDecimal getBigDecimal(String parameterName) throws SQLException;
		decimal GetBigDecimal(String parameterName);

		/// <summary>
		/// Returns an object representing the value of OUT parameter
		/// <code>parameterName</code> and uses <code>map</code> for the custom
		/// mapping of the parameter value.
		/// <para>
		/// This method returns a Java object whose type corresponds to the
		/// JDBC type that was registered for this parameter using the method
		/// <code>registerOutParameter</code>.  By registering the target
		/// JDBC type as <code>java.sql.Types.OTHER</code>, this method can
		/// be used to read database-specific abstract data types.
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="map"> the mapping from SQL type names to Java classes </param>
		/// <returns> a <code>java.lang.Object</code> holding the OUT parameter value </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setObject
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object getObject(String parameterName, java.util.Map<String,Class> map) throws SQLException;
		Object GetObject(String parameterName, IDictionary<String, Class> map);

		/// <summary>
		/// Retrieves the value of a JDBC <code>REF(&lt;structured-type&gt;)</code>
		/// parameter as a <seealso cref="java.sql.Ref"/> object in the Java programming language.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value as a <code>Ref</code> object in the
		///         Java programming language.  If the value was SQL <code>NULL</code>,
		///         the value <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Ref getRef(String parameterName) throws SQLException;
		Ref GetRef(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>BLOB</code> parameter as a
		/// <seealso cref="java.sql.Blob"/> object in the Java programming language.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value as a <code>Blob</code> object in the
		///         Java programming language.  If the value was SQL <code>NULL</code>,
		///         the value <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Blob getBlob(String parameterName) throws SQLException;
		Blob GetBlob(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>CLOB</code> parameter as a
		/// <code>java.sql.Clob</code> object in the Java programming language. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value as a <code>Clob</code> object in the
		///         Java programming language.  If the value was SQL <code>NULL</code>,
		///         the value <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Clob getClob(String parameterName) throws SQLException;
		Clob GetClob(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>ARRAY</code> parameter as an
		/// <seealso cref="java.sql.Array"/> object in the Java programming language.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value as an <code>Array</code> object in
		///         Java programming language.  If the value was SQL <code>NULL</code>,
		///         the value <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Array getArray(String parameterName) throws SQLException;
		Array GetArray(String parameterName);

		/// <summary>
		/// Retrieves the value of a JDBC <code>DATE</code> parameter as a
		/// <code>java.sql.Date</code> object, using
		/// the given <code>Calendar</code> object
		/// to construct the date.
		/// With a <code>Calendar</code> object, the driver
		/// can calculate the date taking into account a custom timezone and locale.
		/// If no <code>Calendar</code> object is specified, the driver uses the
		/// default timezone and locale.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="cal"> the <code>Calendar</code> object the driver will use
		///            to construct the date </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>,
		/// the result is <code>null</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setDate
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Date getDate(String parameterName, java.util.Calendar cal) throws SQLException;
		java.sql.Date GetDate(String parameterName, DateTime cal);

		/// <summary>
		/// Retrieves the value of a JDBC <code>TIME</code> parameter as a
		/// <code>java.sql.Time</code> object, using
		/// the given <code>Calendar</code> object
		/// to construct the time.
		/// With a <code>Calendar</code> object, the driver
		/// can calculate the time taking into account a custom timezone and locale.
		/// If no <code>Calendar</code> object is specified, the driver uses the
		/// default timezone and locale.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="cal"> the <code>Calendar</code> object the driver will use
		///            to construct the time </param>
		/// <returns> the parameter value; if the value is SQL <code>NULL</code>, the result is
		/// <code>null</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setTime
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Time getTime(String parameterName, java.util.Calendar cal) throws SQLException;
		java.sql.Time GetTime(String parameterName, DateTime cal);

		/// <summary>
		/// Retrieves the value of a JDBC <code>TIMESTAMP</code> parameter as a
		/// <code>java.sql.Timestamp</code> object, using
		/// the given <code>Calendar</code> object to construct
		/// the <code>Timestamp</code> object.
		/// With a <code>Calendar</code> object, the driver
		/// can calculate the timestamp taking into account a custom timezone and locale.
		/// If no <code>Calendar</code> object is specified, the driver uses the
		/// default timezone and locale.
		/// 
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="cal"> the <code>Calendar</code> object the driver will use
		///            to construct the timestamp </param>
		/// <returns> the parameter value.  If the value is SQL <code>NULL</code>, the result is
		/// <code>null</code>. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setTimestamp
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.sql.Timestamp getTimestamp(String parameterName, java.util.Calendar cal) throws SQLException;
		java.sql.Timestamp GetTimestamp(String parameterName, DateTime cal);

		/// <summary>
		/// Retrieves the value of a JDBC <code>DATALINK</code> parameter as a
		/// <code>java.net.URL</code> object.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value as a <code>java.net.URL</code> object in the
		/// Java programming language.  If the value was SQL <code>NULL</code>, the
		/// value <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs,
		/// this method is called on a closed <code>CallableStatement</code>,
		///            or if there is a problem with the URL </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setURL
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.net.URL getURL(String parameterName) throws SQLException;
		java.net.URL GetURL(String parameterName);

		//------------------------- JDBC 4.0 -----------------------------------

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>ROWID</code> parameter as a
		/// <code>java.sql.RowId</code> object.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,... </param>
		/// <returns> a <code>RowId</code> object that represents the JDBC <code>ROWID</code>
		///     value is used as the designated parameter. If the parameter contains
		/// a SQL <code>NULL</code>, then a <code>null</code> value is returned. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RowId getRowId(int parameterIndex) throws SQLException;
		RowId GetRowId(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>ROWID</code> parameter as a
		/// <code>java.sql.RowId</code> object.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> a <code>RowId</code> object that represents the JDBC <code>ROWID</code>
		///     value is used as the designated parameter. If the parameter contains
		/// a SQL <code>NULL</code>, then a <code>null</code> value is returned. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RowId getRowId(String parameterName) throws SQLException;
		RowId GetRowId(String parameterName);

		 /// <summary>
		 /// Sets the designated parameter to the given <code>java.sql.RowId</code> object. The
		 /// driver converts this to a SQL <code>ROWID</code> when it sends it to the
		 /// database.
		 /// </summary>
		 /// <param name="parameterName"> the name of the parameter </param>
		 /// <param name="x"> the parameter value </param>
		 /// <exception cref="SQLException"> if parameterName does not correspond to a named
		 /// parameter; if a database access error occurs or
		 /// this method is called on a closed <code>CallableStatement</code> </exception>
		 /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		 /// this method
		 /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setRowId(String parameterName, RowId x) throws SQLException;
		void SetRowId(String parameterName, RowId x);

		/// <summary>
		/// Sets the designated parameter to the given <code>String</code> object.
		/// The driver converts this to a SQL <code>NCHAR</code> or
		/// <code>NVARCHAR</code> or <code>LONGNVARCHAR</code> </summary>
		/// <param name="parameterName"> the name of the parameter to be set </param>
		/// <param name="value"> the parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if the driver does not support national
		///         character sets;  if the driver can detect that a data conversion
		///  error could occur; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setNString(String parameterName, String value) throws SQLException;
		void SetNString(String parameterName, String value);

		/// <summary>
		/// Sets the designated parameter to a <code>Reader</code> object. The
		/// <code>Reader</code> reads the data till end-of-file is reached. The
		/// driver does the necessary conversion from Java character format to
		/// the national character set in the database. </summary>
		/// <param name="parameterName"> the name of the parameter to be set </param>
		/// <param name="value"> the parameter value </param>
		/// <param name="length"> the number of characters in the parameter data. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if the driver does not support national
		///         character sets;  if the driver can detect that a data conversion
		///  error could occur; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setNCharacterStream(String parameterName, java.io.Reader value, long length) throws SQLException;
		void SetNCharacterStream(String parameterName, Reader value, long length);

		 /// <summary>
		 /// Sets the designated parameter to a <code>java.sql.NClob</code> object. The object
		 /// implements the <code>java.sql.NClob</code> interface. This <code>NClob</code>
		 /// object maps to a SQL <code>NCLOB</code>. </summary>
		 /// <param name="parameterName"> the name of the parameter to be set </param>
		 /// <param name="value"> the parameter value </param>
		 /// <exception cref="SQLException"> if parameterName does not correspond to a named
		 /// parameter; if the driver does not support national
		 ///         character sets;  if the driver can detect that a data conversion
		 ///  error could occur; if a database access error occurs or
		 /// this method is called on a closed <code>CallableStatement</code> </exception>
		 /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		 /// this method
		 /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setNClob(String parameterName, NClob value) throws SQLException;
		 void SetNClob(String parameterName, NClob value);

		/// <summary>
		/// Sets the designated parameter to a <code>Reader</code> object.  The <code>reader</code> must contain  the number
		/// of characters specified by length otherwise a <code>SQLException</code> will be
		/// generated when the <code>CallableStatement</code> is executed.
		/// This method differs from the <code>setCharacterStream (int, Reader, int)</code> method
		/// because it informs the driver that the parameter value should be sent to
		/// the server as a <code>CLOB</code>.  When the <code>setCharacterStream</code> method is used, the
		/// driver may have to do extra work to determine whether the parameter
		/// data should be send to the server as a <code>LONGVARCHAR</code> or a <code>CLOB</code> </summary>
		/// <param name="parameterName"> the name of the parameter to be set </param>
		/// <param name="reader"> An object that contains the data to set the parameter value to. </param>
		/// <param name="length"> the number of characters in the parameter data. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if the length specified is less than zero;
		/// a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// 
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setClob(String parameterName, java.io.Reader reader, long length) throws SQLException;
		 void SetClob(String parameterName, Reader reader, long length);

		/// <summary>
		/// Sets the designated parameter to a <code>InputStream</code> object.  The <code>inputstream</code> must contain  the number
		/// of characters specified by length, otherwise a <code>SQLException</code> will be
		/// generated when the <code>CallableStatement</code> is executed.
		/// This method differs from the <code>setBinaryStream (int, InputStream, int)</code>
		/// method because it informs the driver that the parameter value should be
		/// sent to the server as a <code>BLOB</code>.  When the <code>setBinaryStream</code> method is used,
		/// the driver may have to do extra work to determine whether the parameter
		/// data should be sent to the server as a <code>LONGVARBINARY</code> or a <code>BLOB</code>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter to be set
		/// the second is 2, ...
		/// </param>
		/// <param name="inputStream"> An object that contains the data to set the parameter
		/// value to. </param>
		/// <param name="length"> the number of bytes in the parameter data. </param>
		/// <exception cref="SQLException">  if parameterName does not correspond to a named
		/// parameter; if the length specified
		/// is less than zero; if the number of bytes in the inputstream does not match
		/// the specified length; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// 
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setBlob(String parameterName, java.io.InputStream inputStream, long length) throws SQLException;
		 void SetBlob(String parameterName, InputStream inputStream, long length);
		/// <summary>
		/// Sets the designated parameter to a <code>Reader</code> object.  The <code>reader</code> must contain  the number
		/// of characters specified by length otherwise a <code>SQLException</code> will be
		/// generated when the <code>CallableStatement</code> is executed.
		/// This method differs from the <code>setCharacterStream (int, Reader, int)</code> method
		/// because it informs the driver that the parameter value should be sent to
		/// the server as a <code>NCLOB</code>.  When the <code>setCharacterStream</code> method is used, the
		/// driver may have to do extra work to determine whether the parameter
		/// data should be send to the server as a <code>LONGNVARCHAR</code> or a <code>NCLOB</code>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter to be set </param>
		/// <param name="reader"> An object that contains the data to set the parameter value to. </param>
		/// <param name="length"> the number of characters in the parameter data. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if the length specified is less than zero;
		/// if the driver does not support national
		///         character sets;  if the driver can detect that a data conversion
		///  error could occur; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setNClob(String parameterName, java.io.Reader reader, long length) throws SQLException;
		 void SetNClob(String parameterName, Reader reader, long length);

		/// <summary>
		/// Retrieves the value of the designated JDBC <code>NCLOB</code> parameter as a
		/// <code>java.sql.NClob</code> object in the Java programming language.
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2, and
		/// so on </param>
		/// <returns> the parameter value as a <code>NClob</code> object in the
		/// Java programming language.  If the value was SQL <code>NULL</code>, the
		/// value <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if the driver does not support national
		///         character sets;  if the driver can detect that a data conversion
		///  error could occur; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: NClob getNClob(int parameterIndex) throws SQLException;
		NClob GetNClob(int parameterIndex);


		/// <summary>
		/// Retrieves the value of a JDBC <code>NCLOB</code> parameter as a
		/// <code>java.sql.NClob</code> object in the Java programming language. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> the parameter value as a <code>NClob</code> object in the
		///         Java programming language.  If the value was SQL <code>NULL</code>,
		///         the value <code>null</code> is returned. </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if the driver does not support national
		///         character sets;  if the driver can detect that a data conversion
		///  error could occur; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: NClob getNClob(String parameterName) throws SQLException;
		NClob GetNClob(String parameterName);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.sql.SQLXML</code> object. The driver converts this to an
		/// <code>SQL XML</code> value when it sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="xmlObject"> a <code>SQLXML</code> object that maps an <code>SQL XML</code> value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs;
		/// this method is called on a closed <code>CallableStatement</code> or
		/// the <code>java.xml.transform.Result</code>,
		///  <code>Writer</code> or <code>OutputStream</code> has not been closed for the <code>SQLXML</code> object </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// 
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setSQLXML(String parameterName, SQLXML xmlObject) throws SQLException;
		void SetSQLXML(String parameterName, SQLXML xmlObject);

		/// <summary>
		/// Retrieves the value of the designated <code>SQL XML</code> parameter as a
		/// <code>java.sql.SQLXML</code> object in the Java programming language. </summary>
		/// <param name="parameterIndex"> index of the first parameter is 1, the second is 2, ... </param>
		/// <returns> a <code>SQLXML</code> object that maps an <code>SQL XML</code> value </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: SQLXML getSQLXML(int parameterIndex) throws SQLException;
		SQLXML GetSQLXML(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated <code>SQL XML</code> parameter as a
		/// <code>java.sql.SQLXML</code> object in the Java programming language. </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> a <code>SQLXML</code> object that maps an <code>SQL XML</code> value </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: SQLXML getSQLXML(String parameterName) throws SQLException;
		SQLXML GetSQLXML(String parameterName);

		/// <summary>
		/// Retrieves the value of the designated <code>NCHAR</code>,
		/// <code>NVARCHAR</code>
		/// or <code>LONGNVARCHAR</code> parameter as
		/// a <code>String</code> in the Java programming language.
		///  <para>
		/// For the fixed-length type JDBC <code>NCHAR</code>,
		/// the <code>String</code> object
		/// returned has exactly the same value the SQL
		/// <code>NCHAR</code> value had in the
		/// database, including any padding added by the database.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> index of the first parameter is 1, the second is 2, ... </param>
		/// <returns> a <code>String</code> object that maps an
		/// <code>NCHAR</code>, <code>NVARCHAR</code> or <code>LONGNVARCHAR</code> value </returns>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
		/// <seealso cref= #setNString </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getNString(int parameterIndex) throws SQLException;
		String GetNString(int parameterIndex);


		/// <summary>
		///  Retrieves the value of the designated <code>NCHAR</code>,
		/// <code>NVARCHAR</code>
		/// or <code>LONGNVARCHAR</code> parameter as
		/// a <code>String</code> in the Java programming language.
		/// <para>
		/// For the fixed-length type JDBC <code>NCHAR</code>,
		/// the <code>String</code> object
		/// returned has exactly the same value the SQL
		/// <code>NCHAR</code> value had in the
		/// database, including any padding added by the database.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> a <code>String</code> object that maps an
		/// <code>NCHAR</code>, <code>NVARCHAR</code> or <code>LONGNVARCHAR</code> value </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
		/// <seealso cref= #setNString </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getNString(String parameterName) throws SQLException;
		String GetNString(String parameterName);

		/// <summary>
		/// Retrieves the value of the designated parameter as a
		/// <code>java.io.Reader</code> object in the Java programming language.
		/// It is intended for use when
		/// accessing  <code>NCHAR</code>,<code>NVARCHAR</code>
		/// and <code>LONGNVARCHAR</code> parameters.
		/// </summary>
		/// <returns> a <code>java.io.Reader</code> object that contains the parameter
		/// value; if the value is SQL <code>NULL</code>, the value returned is
		/// <code>null</code> in the Java programming language. </returns>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.Reader getNCharacterStream(int parameterIndex) throws SQLException;
		Reader GetNCharacterStream(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated parameter as a
		/// <code>java.io.Reader</code> object in the Java programming language.
		/// It is intended for use when
		/// accessing  <code>NCHAR</code>,<code>NVARCHAR</code>
		/// and <code>LONGNVARCHAR</code> parameters.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> a <code>java.io.Reader</code> object that contains the parameter
		/// value; if the value is SQL <code>NULL</code>, the value returned is
		/// <code>null</code> in the Java programming language </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.Reader getNCharacterStream(String parameterName) throws SQLException;
		Reader GetNCharacterStream(String parameterName);

		/// <summary>
		/// Retrieves the value of the designated parameter as a
		/// <code>java.io.Reader</code> object in the Java programming language.
		/// </summary>
		/// <returns> a <code>java.io.Reader</code> object that contains the parameter
		/// value; if the value is SQL <code>NULL</code>, the value returned is
		/// <code>null</code> in the Java programming language. </returns>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
		/// <exception cref="SQLException"> if the parameterIndex is not valid; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code>
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.Reader getCharacterStream(int parameterIndex) throws SQLException;
		Reader GetCharacterStream(int parameterIndex);

		/// <summary>
		/// Retrieves the value of the designated parameter as a
		/// <code>java.io.Reader</code> object in the Java programming language.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <returns> a <code>java.io.Reader</code> object that contains the parameter
		/// value; if the value is SQL <code>NULL</code>, the value returned is
		/// <code>null</code> in the Java programming language </returns>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.io.Reader getCharacterStream(String parameterName) throws SQLException;
		Reader GetCharacterStream(String parameterName);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.sql.Blob</code> object.
		/// The driver converts this to an SQL <code>BLOB</code> value when it
		/// sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> a <code>Blob</code> object that maps an SQL <code>BLOB</code> value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setBlob(String parameterName, Blob x) throws SQLException;
		void SetBlob(String parameterName, Blob x);

		/// <summary>
		/// Sets the designated parameter to the given <code>java.sql.Clob</code> object.
		/// The driver converts this to an SQL <code>CLOB</code> value when it
		/// sends it to the database.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> a <code>Clob</code> object that maps an SQL <code>CLOB</code> value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setClob(String parameterName, Clob x) throws SQLException;
		void SetClob(String parameterName, Clob x);
		/// <summary>
		/// Sets the designated parameter to the given input stream, which will have
		/// the specified number of bytes.
		/// When a very large ASCII value is input to a <code>LONGVARCHAR</code>
		/// parameter, it may be more practical to send it via a
		/// <code>java.io.InputStream</code>. Data will be read from the stream
		/// as needed until end-of-file is reached.  The JDBC driver will
		/// do any necessary conversion from ASCII to the database char format.
		/// 
		/// <P><B>Note:</B> This stream object can either be a standard
		/// Java stream object or your own subclass that implements the
		/// standard interface.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the Java input stream that contains the ASCII parameter value </param>
		/// <param name="length"> the number of bytes in the stream </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setAsciiStream(String parameterName, java.io.InputStream x, long length) throws SQLException;
		void SetAsciiStream(String parameterName, InputStream x, long length);

		/// <summary>
		/// Sets the designated parameter to the given input stream, which will have
		/// the specified number of bytes.
		/// When a very large binary value is input to a <code>LONGVARBINARY</code>
		/// parameter, it may be more practical to send it via a
		/// <code>java.io.InputStream</code> object. The data will be read from the stream
		/// as needed until end-of-file is reached.
		/// 
		/// <P><B>Note:</B> This stream object can either be a standard
		/// Java stream object or your own subclass that implements the
		/// standard interface.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the java input stream which contains the binary parameter value </param>
		/// <param name="length"> the number of bytes in the stream </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setBinaryStream(String parameterName, java.io.InputStream x, long length) throws SQLException;
		void SetBinaryStream(String parameterName, InputStream x, long length);
			/// <summary>
			/// Sets the designated parameter to the given <code>Reader</code>
			/// object, which is the given number of characters long.
			/// When a very large UNICODE value is input to a <code>LONGVARCHAR</code>
			/// parameter, it may be more practical to send it via a
			/// <code>java.io.Reader</code> object. The data will be read from the stream
			/// as needed until end-of-file is reached.  The JDBC driver will
			/// do any necessary conversion from UNICODE to the database char format.
			/// 
			/// <P><B>Note:</B> This stream object can either be a standard
			/// Java stream object or your own subclass that implements the
			/// standard interface.
			/// </summary>
			/// <param name="parameterName"> the name of the parameter </param>
			/// <param name="reader"> the <code>java.io.Reader</code> object that
			///        contains the UNICODE data used as the designated parameter </param>
			/// <param name="length"> the number of characters in the stream </param>
			/// <exception cref="SQLException"> if parameterName does not correspond to a named
			/// parameter; if a database access error occurs or
			/// this method is called on a closed <code>CallableStatement</code> </exception>
			/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
			/// this method
			/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setCharacterStream(String parameterName, java.io.Reader reader, long length) throws SQLException;
		void SetCharacterStream(String parameterName, Reader reader, long length);
		 //--
		/// <summary>
		/// Sets the designated parameter to the given input stream.
		/// When a very large ASCII value is input to a <code>LONGVARCHAR</code>
		/// parameter, it may be more practical to send it via a
		/// <code>java.io.InputStream</code>. Data will be read from the stream
		/// as needed until end-of-file is reached.  The JDBC driver will
		/// do any necessary conversion from ASCII to the database char format.
		/// 
		/// <P><B>Note:</B> This stream object can either be a standard
		/// Java stream object or your own subclass that implements the
		/// standard interface.
		/// <P><B>Note:</B> Consult your JDBC driver documentation to determine if
		/// it might be more efficient to use a version of
		/// <code>setAsciiStream</code> which takes a length parameter.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the Java input stream that contains the ASCII parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setAsciiStream(String parameterName, java.io.InputStream x) throws SQLException;
		void SetAsciiStream(String parameterName, InputStream x);
		/// <summary>
		/// Sets the designated parameter to the given input stream.
		/// When a very large binary value is input to a <code>LONGVARBINARY</code>
		/// parameter, it may be more practical to send it via a
		/// <code>java.io.InputStream</code> object. The data will be read from the
		/// stream as needed until end-of-file is reached.
		/// 
		/// <P><B>Note:</B> This stream object can either be a standard
		/// Java stream object or your own subclass that implements the
		/// standard interface.
		/// <P><B>Note:</B> Consult your JDBC driver documentation to determine if
		/// it might be more efficient to use a version of
		/// <code>setBinaryStream</code> which takes a length parameter.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the java input stream which contains the binary parameter value </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setBinaryStream(String parameterName, java.io.InputStream x) throws SQLException;
		void SetBinaryStream(String parameterName, InputStream x);
		/// <summary>
		/// Sets the designated parameter to the given <code>Reader</code>
		/// object.
		/// When a very large UNICODE value is input to a <code>LONGVARCHAR</code>
		/// parameter, it may be more practical to send it via a
		/// <code>java.io.Reader</code> object. The data will be read from the stream
		/// as needed until end-of-file is reached.  The JDBC driver will
		/// do any necessary conversion from UNICODE to the database char format.
		/// 
		/// <P><B>Note:</B> This stream object can either be a standard
		/// Java stream object or your own subclass that implements the
		/// standard interface.
		/// <P><B>Note:</B> Consult your JDBC driver documentation to determine if
		/// it might be more efficient to use a version of
		/// <code>setCharacterStream</code> which takes a length parameter.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="reader"> the <code>java.io.Reader</code> object that contains the
		///        Unicode data </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setCharacterStream(String parameterName, java.io.Reader reader) throws SQLException;
		void SetCharacterStream(String parameterName, Reader reader);
	  /// <summary>
	  /// Sets the designated parameter to a <code>Reader</code> object. The
	  /// <code>Reader</code> reads the data till end-of-file is reached. The
	  /// driver does the necessary conversion from Java character format to
	  /// the national character set in the database.
	  /// 
	  /// <P><B>Note:</B> This stream object can either be a standard
	  /// Java stream object or your own subclass that implements the
	  /// standard interface.
	  /// <P><B>Note:</B> Consult your JDBC driver documentation to determine if
	  /// it might be more efficient to use a version of
	  /// <code>setNCharacterStream</code> which takes a length parameter.
	  /// </summary>
	  /// <param name="parameterName"> the name of the parameter </param>
	  /// <param name="value"> the parameter value </param>
	  /// <exception cref="SQLException"> if parameterName does not correspond to a named
	  /// parameter; if the driver does not support national
	  ///         character sets;  if the driver can detect that a data conversion
	  ///  error could occur; if a database access error occurs; or
	  /// this method is called on a closed <code>CallableStatement</code> </exception>
	  /// <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setNCharacterStream(String parameterName, java.io.Reader value) throws SQLException;
		 void SetNCharacterStream(String parameterName, Reader value);

		/// <summary>
		/// Sets the designated parameter to a <code>Reader</code> object.
		/// This method differs from the <code>setCharacterStream (int, Reader)</code> method
		/// because it informs the driver that the parameter value should be sent to
		/// the server as a <code>CLOB</code>.  When the <code>setCharacterStream</code> method is used, the
		/// driver may have to do extra work to determine whether the parameter
		/// data should be send to the server as a <code>LONGVARCHAR</code> or a <code>CLOB</code>
		/// 
		/// <P><B>Note:</B> Consult your JDBC driver documentation to determine if
		/// it might be more efficient to use a version of
		/// <code>setClob</code> which takes a length parameter.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="reader"> An object that contains the data to set the parameter value to. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or this method is called on
		/// a closed <code>CallableStatement</code>
		/// </exception>
		/// <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setClob(String parameterName, java.io.Reader reader) throws SQLException;
		 void SetClob(String parameterName, Reader reader);

		/// <summary>
		/// Sets the designated parameter to a <code>InputStream</code> object.
		/// This method differs from the <code>setBinaryStream (int, InputStream)</code>
		/// method because it informs the driver that the parameter value should be
		/// sent to the server as a <code>BLOB</code>.  When the <code>setBinaryStream</code> method is used,
		/// the driver may have to do extra work to determine whether the parameter
		/// data should be send to the server as a <code>LONGVARBINARY</code> or a <code>BLOB</code>
		/// 
		/// <P><B>Note:</B> Consult your JDBC driver documentation to determine if
		/// it might be more efficient to use a version of
		/// <code>setBlob</code> which takes a length parameter.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="inputStream"> An object that contains the data to set the parameter
		/// value to. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
		/// 
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setBlob(String parameterName, java.io.InputStream inputStream) throws SQLException;
		 void SetBlob(String parameterName, InputStream inputStream);
		/// <summary>
		/// Sets the designated parameter to a <code>Reader</code> object.
		/// This method differs from the <code>setCharacterStream (int, Reader)</code> method
		/// because it informs the driver that the parameter value should be sent to
		/// the server as a <code>NCLOB</code>.  When the <code>setCharacterStream</code> method is used, the
		/// driver may have to do extra work to determine whether the parameter
		/// data should be send to the server as a <code>LONGNVARCHAR</code> or a <code>NCLOB</code>
		/// <P><B>Note:</B> Consult your JDBC driver documentation to determine if
		/// it might be more efficient to use a version of
		/// <code>setNClob</code> which takes a length parameter.
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="reader"> An object that contains the data to set the parameter value to. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if the driver does not support national character sets;
		/// if the driver can detect that a data conversion
		///  error could occur;  if a database access error occurs or
		/// this method is called on a closed <code>CallableStatement</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
		/// 
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setNClob(String parameterName, java.io.Reader reader) throws SQLException;
		 void SetNClob(String parameterName, Reader reader);

		//------------------------- JDBC 4.1 -----------------------------------


		/// <summary>
		/// <para>Returns an object representing the value of OUT parameter
		/// {@code parameterIndex} and will convert from the
		/// SQL type of the parameter to the requested Java data type, if the
		/// conversion is supported. If the conversion is not
		/// supported or null is specified for the type, a
		/// <code>SQLException</code> is thrown.
		/// </para>
		/// <para>
		/// At a minimum, an implementation must support the conversions defined in
		/// Appendix B, Table B-3 and conversion of appropriate user defined SQL
		/// types to a Java type which implements {@code SQLData}, or {@code Struct}.
		/// Additional conversions may be supported and are vendor defined.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2, and so on </param>
		/// <param name="type"> Class representing the Java data type to convert the
		/// designated parameter to. </param>
		/// @param <T> the type of the class modeled by this Class object </param>
		/// <returns> an instance of {@code type} holding the OUT parameter value </returns>
		/// <exception cref="SQLException"> if conversion is not supported, type is null or
		///         another error occurs. The getCause() method of the
		/// exception may provide a more detailed exception, for example, if
		/// a conversion error occurs </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T getObject(int parameterIndex, Class type) throws SQLException;
		 T getObject<T>(int parameterIndex, Class type);


		/// <summary>
		/// <para>Returns an object representing the value of OUT parameter
		/// {@code parameterName} and will convert from the
		/// SQL type of the parameter to the requested Java data type, if the
		/// conversion is supported. If the conversion is not
		/// supported  or null is specified for the type, a
		/// <code>SQLException</code> is thrown.
		/// </para>
		/// <para>
		/// At a minimum, an implementation must support the conversions defined in
		/// Appendix B, Table B-3 and conversion of appropriate user defined SQL
		/// types to a Java type which implements {@code SQLData}, or {@code Struct}.
		/// Additional conversions may be supported and are vendor defined.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="type"> Class representing the Java data type to convert
		/// the designated parameter to. </param>
		/// @param <T> the type of the class modeled by this Class object </param>
		/// <returns> an instance of {@code type} holding the OUT parameter
		/// value </returns>
		/// <exception cref="SQLException"> if conversion is not supported, type is null or
		///         another error occurs. The getCause() method of the
		/// exception may provide a more detailed exception, for example, if
		/// a conversion error occurs </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public <T> T getObject(String parameterName, Class type) throws SQLException;
		 T getObject<T>(String parameterName, Class type);

		 //------------------------- JDBC 4.2 -----------------------------------

		 /// <summary>
		 /// <para>Sets the value of the designated parameter with the given object.
		 /// 
		 /// If the second argument is an {@code InputStream} then the stream
		 /// must contain the number of bytes specified by scaleOrLength.
		 /// If the second argument is a {@code Reader} then the reader must
		 /// contain the number of characters specified
		 /// by scaleOrLength. If these conditions are not true the driver
		 /// will generate a
		 /// {@code SQLException} when the prepared statement is executed.
		 /// 
		 /// </para>
		 /// <para>The given Java object will be converted to the given targetSqlType
		 /// before being sent to the database.
		 /// 
		 /// If the object has a custom mapping (is of a class implementing the
		 /// interface {@code SQLData}),
		 /// the JDBC driver should call the method {@code SQLData.writeSQL} to
		 /// write it to the SQL data stream.
		 /// If, on the other hand, the object is of a class implementing
		 /// {@code Ref}, {@code Blob}, {@code Clob},  {@code NClob},
		 ///  {@code Struct}, {@code java.net.URL},
		 /// or {@code Array}, the driver should pass it to the database as a
		 /// value of the corresponding SQL type.
		 /// 
		 /// </para>
		 /// <para>Note that this method may be used to pass database-specific
		 /// abstract data types.
		 /// <P>
		 /// The default implementation will throw {@code SQLFeatureNotSupportedException}
		 /// 
		 /// </para>
		 /// </summary>
		 /// <param name="parameterName"> the name of the parameter </param>
		 /// <param name="x"> the object containing the input parameter value </param>
		 /// <param name="targetSqlType"> the SQL type to be
		 /// sent to the database. The scale argument may further qualify this type. </param>
		 /// <param name="scaleOrLength"> for {@code java.sql.JDBCType.DECIMAL}
		 ///          or {@code java.sql.JDBCType.NUMERIC types},
		 ///          this is the number of digits after the decimal point. For
		 ///          Java Object types {@code InputStream} and {@code Reader},
		 ///          this is the length
		 ///          of the data in the stream or reader.  For all other types,
		 ///          this value will be ignored. </param>
		 /// <exception cref="SQLException"> if parameterName does not correspond to a named
		 /// parameter; if a database access error occurs
		 /// or this method is called on a closed {@code CallableStatement}  or
		 ///            if the Java Object specified by x is an InputStream
		 ///            or Reader object and the value of the scale parameter is less
		 ///            than zero </exception>
		 /// <exception cref="SQLFeatureNotSupportedException"> if
		 /// the JDBC driver does not support the specified targetSqlType </exception>
		 /// <seealso cref= JDBCType </seealso>
		 /// <seealso cref= SQLType
		 /// 
		 /// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		 default void setObject(String parameterName, Object x, SQLType targetSqlType, int scaleOrLength) throws SQLException
	//	 {
	//		throw new SQLFeatureNotSupportedException("setObject not implemented");
	//	}
		/// <summary>
		/// Sets the value of the designated parameter with the given object.
		/// 
		/// This method is similar to {@link #setObject(String parameterName,
		/// Object x, SQLType targetSqlType, int scaleOrLength)},
		/// except that it assumes a scale of zero.
		/// <P>
		/// The default implementation will throw {@code SQLFeatureNotSupportedException}
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="x"> the object containing the input parameter value </param>
		/// <param name="targetSqlType"> the SQL type to be sent to the database </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs
		/// or this method is called on a closed {@code CallableStatement} </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if
		/// the JDBC driver does not support the specified targetSqlType </exception>
		/// <seealso cref= JDBCType </seealso>
		/// <seealso cref= SQLType
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		 default void setObject(String parameterName, Object x, SQLType targetSqlType) throws SQLException
	//	 {
	//		throw new SQLFeatureNotSupportedException("setObject not implemented");
	//	}

		/// <summary>
		/// Registers the OUT parameter in ordinal position
		/// {@code parameterIndex} to the JDBC type
		/// {@code sqlType}.  All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// <para>
		/// The JDBC type specified by {@code sqlType} for an OUT
		/// parameter determines the Java type that must be used
		/// in the {@code get} method to read the value of that parameter.
		/// </para>
		/// <para>
		/// If the JDBC type expected to be returned to this output parameter
		/// is specific to this particular database, {@code sqlType}
		/// may be {@code JDBCType.OTHER} or a {@code SQLType} that is supported by
		/// the JDBC driver.  The method
		/// <seealso cref="#getObject"/> retrieves the value.
		/// <P>
		/// The default implementation will throw {@code SQLFeatureNotSupportedException}
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		///        and so on </param>
		/// <param name="sqlType"> the JDBC type code defined by {@code SQLType} to use to
		/// register the OUT Parameter.
		///        If the parameter is of JDBC type {@code JDBCType.NUMERIC}
		///        or {@code JDBCType.DECIMAL}, the version of
		///        {@code registerOutParameter} that accepts a scale value
		///        should be used.
		/// </param>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed {@code CallableStatement} </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if
		/// the JDBC driver does not support the specified sqlType </exception>
		/// <seealso cref= JDBCType </seealso>
		/// <seealso cref= SQLType
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void registerOutParameter(int parameterIndex, SQLType sqlType) throws SQLException
	//	{
	//		throw new SQLFeatureNotSupportedException("registerOutParameter not implemented");
	//	}

		/// <summary>
		/// Registers the parameter in ordinal position
		/// {@code parameterIndex} to be of JDBC type
		/// {@code sqlType}. All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// <para>
		/// The JDBC type specified by {@code sqlType} for an OUT
		/// parameter determines the Java type that must be used
		/// in the {@code get} method to read the value of that parameter.
		/// </para>
		/// <para>
		/// This version of {@code  registerOutParameter} should be
		/// used when the parameter is of JDBC type {@code JDBCType.NUMERIC}
		/// or {@code JDBCType.DECIMAL}.
		/// <P>
		/// The default implementation will throw {@code SQLFeatureNotSupportedException}
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,
		/// and so on </param>
		/// <param name="sqlType"> the JDBC type code defined by {@code SQLType} to use to
		/// register the OUT Parameter. </param>
		/// <param name="scale"> the desired number of digits to the right of the
		/// decimal point.  It must be greater than or equal to zero. </param>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed {@code CallableStatement} </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if
		/// the JDBC driver does not support the specified sqlType </exception>
		/// <seealso cref= JDBCType </seealso>
		/// <seealso cref= SQLType
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void registerOutParameter(int parameterIndex, SQLType sqlType, int scale) throws SQLException
	//	{
	//		throw new SQLFeatureNotSupportedException("registerOutParameter not implemented");
	//	}
		/// <summary>
		/// Registers the designated output parameter.
		/// This version of
		/// the method {@code  registerOutParameter}
		/// should be used for a user-defined or {@code REF} output parameter.
		/// Examples
		/// of user-defined types include: {@code STRUCT}, {@code DISTINCT},
		/// {@code JAVA_OBJECT}, and named array types.
		/// <para>
		/// All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// </para>
		/// <para>  For a user-defined parameter, the fully-qualified SQL
		/// type name of the parameter should also be given, while a {@code REF}
		/// parameter requires that the fully-qualified type name of the
		/// referenced type be given.  A JDBC driver that does not need the
		/// type code and type name information may ignore it.   To be portable,
		/// however, applications should always provide these values for
		/// user-defined and {@code REF} parameters.
		/// 
		/// Although it is intended for user-defined and {@code REF} parameters,
		/// this method may be used to register a parameter of any JDBC type.
		/// If the parameter does not have a user-defined or {@code REF} type, the
		/// <i>typeName</i> parameter is ignored.
		/// 
		/// <P><B>Note:</B> When reading the value of an out parameter, you
		/// must use the getter method whose Java type corresponds to the
		/// parameter's registered SQL type.
		/// <P>
		/// The default implementation will throw {@code SQLFeatureNotSupportedException}
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterIndex"> the first parameter is 1, the second is 2,... </param>
		/// <param name="sqlType"> the JDBC type code defined by {@code SQLType} to use to
		/// register the OUT Parameter. </param>
		/// <param name="typeName"> the fully-qualified name of an SQL structured type </param>
		/// <exception cref="SQLException"> if the parameterIndex is not valid;
		/// if a database access error occurs or
		/// this method is called on a closed {@code CallableStatement} </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if
		/// the JDBC driver does not support the specified sqlType </exception>
		/// <seealso cref= JDBCType </seealso>
		/// <seealso cref= SQLType
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void registerOutParameter(int parameterIndex, SQLType sqlType, String typeName) throws SQLException
	//	{
	//		throw new SQLFeatureNotSupportedException("registerOutParameter not implemented");
	//	}

		/// <summary>
		/// Registers the OUT parameter named
		/// <code>parameterName</code> to the JDBC type
		/// {@code sqlType}.  All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// <para>
		/// The JDBC type specified by {@code sqlType} for an OUT
		/// parameter determines the Java type that must be used
		/// in the {@code get} method to read the value of that parameter.
		/// </para>
		/// <para>
		/// If the JDBC type expected to be returned to this output parameter
		/// is specific to this particular database, {@code sqlType}
		/// should be {@code JDBCType.OTHER} or a {@code SQLType} that is supported
		/// by the JDBC driver..  The method
		/// <seealso cref="#getObject"/> retrieves the value.
		/// <P>
		/// The default implementation will throw {@code SQLFeatureNotSupportedException}
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="sqlType"> the JDBC type code defined by {@code SQLType} to use to
		/// register the OUT Parameter.
		/// If the parameter is of JDBC type {@code JDBCType.NUMERIC}
		/// or {@code JDBCType.DECIMAL}, the version of
		/// {@code  registerOutParameter} that accepts a scale value
		/// should be used. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed {@code CallableStatement} </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if
		/// the JDBC driver does not support the specified sqlType
		/// or if the JDBC driver does not support
		/// this method
		/// @since 1.8 </exception>
		/// <seealso cref= JDBCType </seealso>
		/// <seealso cref= SQLType </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void registerOutParameter(String parameterName, SQLType sqlType) throws SQLException
	//	{
	//		throw new SQLFeatureNotSupportedException("registerOutParameter not implemented");
	//	}

		/// <summary>
		/// Registers the parameter named
		/// <code>parameterName</code> to be of JDBC type
		/// {@code sqlType}.  All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// <para>
		/// The JDBC type specified by {@code sqlType} for an OUT
		/// parameter determines the Java type that must be used
		/// in the {@code get} method to read the value of that parameter.
		/// </para>
		/// <para>
		/// This version of {@code  registerOutParameter} should be
		/// used when the parameter is of JDBC type {@code JDBCType.NUMERIC}
		/// or {@code JDBCType.DECIMAL}.
		/// <P>
		/// The default implementation will throw {@code SQLFeatureNotSupportedException}
		/// 
		/// </para>
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="sqlType"> the JDBC type code defined by {@code SQLType} to use to
		/// register the OUT Parameter. </param>
		/// <param name="scale"> the desired number of digits to the right of the
		/// decimal point.  It must be greater than or equal to zero. </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed {@code CallableStatement} </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if
		/// the JDBC driver does not support the specified sqlType
		/// or if the JDBC driver does not support
		/// this method
		/// @since 1.8 </exception>
		/// <seealso cref= JDBCType </seealso>
		/// <seealso cref= SQLType </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void registerOutParameter(String parameterName, SQLType sqlType, int scale) throws SQLException
	//	{
	//		throw new SQLFeatureNotSupportedException("registerOutParameter not implemented");
	//	}

		/// <summary>
		/// Registers the designated output parameter.  This version of
		/// the method {@code  registerOutParameter}
		/// should be used for a user-named or REF output parameter.  Examples
		/// of user-named types include: STRUCT, DISTINCT, JAVA_OBJECT, and
		/// named array types.
		/// <para>
		/// All OUT parameters must be registered
		/// before a stored procedure is executed.
		/// </para>
		/// For a user-named parameter the fully-qualified SQL
		/// type name of the parameter should also be given, while a REF
		/// parameter requires that the fully-qualified type name of the
		/// referenced type be given.  A JDBC driver that does not need the
		/// type code and type name information may ignore it.   To be portable,
		/// however, applications should always provide these values for
		/// user-named and REF parameters.
		/// 
		/// Although it is intended for user-named and REF parameters,
		/// this method may be used to register a parameter of any JDBC type.
		/// If the parameter does not have a user-named or REF type, the
		/// typeName parameter is ignored.
		/// 
		/// <P><B>Note:</B> When reading the value of an out parameter, you
		/// must use the {@code getXXX} method whose Java type XXX corresponds to the
		/// parameter's registered SQL type.
		/// <P>
		/// The default implementation will throw {@code SQLFeatureNotSupportedException}
		/// </summary>
		/// <param name="parameterName"> the name of the parameter </param>
		/// <param name="sqlType"> the JDBC type code defined by {@code SQLType} to use to
		/// register the OUT Parameter. </param>
		/// <param name="typeName"> the fully-qualified name of an SQL structured type </param>
		/// <exception cref="SQLException"> if parameterName does not correspond to a named
		/// parameter; if a database access error occurs or
		/// this method is called on a closed {@code CallableStatement} </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if
		/// the JDBC driver does not support the specified sqlType
		/// or if the JDBC driver does not support this method </exception>
		/// <seealso cref= JDBCType </seealso>
		/// <seealso cref= SQLType
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void registerOutParameter(String parameterName, SQLType sqlType, String typeName) throws SQLException
	//	{
	//		throw new SQLFeatureNotSupportedException("registerOutParameter not implemented");
	//	}
	}

}