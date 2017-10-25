using System;

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
	/// The output stream for writing the attributes of a user-defined
	/// type back to the database.  This interface, used
	/// only for custom mapping, is used by the driver, and its
	/// methods are never directly invoked by a programmer.
	/// <para>When an object of a class implementing the interface
	/// <code>SQLData</code> is passed as an argument to an SQL statement, the
	/// JDBC driver calls the method <code>SQLData.getSQLType</code> to
	/// determine the  kind of SQL
	/// datum being passed to the database.
	/// The driver then creates an instance of <code>SQLOutput</code> and
	/// passes it to the method <code>SQLData.writeSQL</code>.
	/// The method <code>writeSQL</code> in turn calls the
	/// appropriate <code>SQLOutput</code> <i>writer</i> methods
	/// <code>writeBoolean</code>, <code>writeCharacterStream</code>, and so on)
	/// to write data from the <code>SQLData</code> object to
	/// the <code>SQLOutput</code> output stream as the
	/// representation of an SQL user-defined type.
	/// @since 1.2
	/// </para>
	/// </summary>

	 public interface SQLOutput
	 {

	  //================================================================
	  // Methods for writing attributes to the stream of SQL data.
	  // These methods correspond to the column-accessor methods of
	  // java.sql.ResultSet.
	  //================================================================

	  /// <summary>
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeString(String x) throws SQLException;
	  void WriteString(String x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a Java boolean.
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeBoolean(boolean x) throws SQLException;
	  void WriteBoolean(bool x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a Java byte.
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeByte(byte x) throws SQLException;
	  void WriteByte(sbyte x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a Java short.
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeShort(short x) throws SQLException;
	  void WriteShort(short x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a Java int.
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeInt(int x) throws SQLException;
	  void WriteInt(int x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a Java long.
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeLong(long x) throws SQLException;
	  void WriteLong(long x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a Java float.
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeFloat(float x) throws SQLException;
	  void WriteFloat(float x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a Java double.
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeDouble(double x) throws SQLException;
	  void WriteDouble(double x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a java.math.BigDecimal object.
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeBigDecimal(java.math.BigDecimal x) throws SQLException;
	  void WriteBigDecimal(decimal x);

	  /// <summary>
	  /// Writes the next attribute to the stream as an array of bytes.
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeBytes(byte[] x) throws SQLException;
	  void WriteBytes(sbyte[] x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a java.sql.Date object.
	  /// Writes the next attribute to the stream as a <code>java.sql.Date</code> object
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeDate(java.sql.Date x) throws SQLException;
	  void WriteDate(java.sql.Date x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a java.sql.Time object.
	  /// Writes the next attribute to the stream as a <code>java.sql.Date</code> object
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeTime(java.sql.Time x) throws SQLException;
	  void WriteTime(java.sql.Time x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a java.sql.Timestamp object.
	  /// Writes the next attribute to the stream as a <code>java.sql.Date</code> object
	  /// in the Java programming language.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeTimestamp(java.sql.Timestamp x) throws SQLException;
	  void WriteTimestamp(java.sql.Timestamp x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a stream of Unicode characters.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeCharacterStream(java.io.Reader x) throws SQLException;
	  void WriteCharacterStream(java.io.Reader x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a stream of ASCII characters.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeAsciiStream(java.io.InputStream x) throws SQLException;
	  void WriteAsciiStream(java.io.InputStream x);

	  /// <summary>
	  /// Writes the next attribute to the stream as a stream of uninterpreted
	  /// bytes.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeBinaryStream(java.io.InputStream x) throws SQLException;
	  void WriteBinaryStream(java.io.InputStream x);

	  //================================================================
	  // Methods for writing items of SQL user-defined types to the stream.
	  // These methods pass objects to the database as values of SQL
	  // Structured Types, Distinct Types, Constructed Types, and Locator
	  // Types.  They decompose the Java object(s) and write leaf data
	  // items using the methods above.
	  //================================================================

	  /// <summary>
	  /// Writes to the stream the data contained in the given
	  /// <code>SQLData</code> object.
	  /// When the <code>SQLData</code> object is <code>null</code>, this
	  /// method writes an SQL <code>NULL</code> to the stream.
	  /// Otherwise, it calls the <code>SQLData.writeSQL</code>
	  /// method of the given object, which
	  /// writes the object's attributes to the stream.
	  /// The implementation of the method <code>SQLData.writeSQL</code>
	  /// calls the appropriate <code>SQLOutput</code> writer method(s)
	  /// for writing each of the object's attributes in order.
	  /// The attributes must be read from an <code>SQLInput</code>
	  /// input stream and written to an <code>SQLOutput</code>
	  /// output stream in the same order in which they were
	  /// listed in the SQL definition of the user-defined type.
	  /// </summary>
	  /// <param name="x"> the object representing data of an SQL structured or
	  /// distinct type </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeObject(SQLData x) throws SQLException;
	  void WriteObject(SQLData x);

	  /// <summary>
	  /// Writes an SQL <code>REF</code> value to the stream.
	  /// </summary>
	  /// <param name="x"> a <code>Ref</code> object representing data of an SQL
	  /// <code>REF</code> value </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeRef(Ref x) throws SQLException;
	  void WriteRef(Ref x);

	  /// <summary>
	  /// Writes an SQL <code>BLOB</code> value to the stream.
	  /// </summary>
	  /// <param name="x"> a <code>Blob</code> object representing data of an SQL
	  /// <code>BLOB</code> value
	  /// </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeBlob(Blob x) throws SQLException;
	  void WriteBlob(Blob x);

	  /// <summary>
	  /// Writes an SQL <code>CLOB</code> value to the stream.
	  /// </summary>
	  /// <param name="x"> a <code>Clob</code> object representing data of an SQL
	  /// <code>CLOB</code> value
	  /// </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeClob(Clob x) throws SQLException;
	  void WriteClob(Clob x);

	  /// <summary>
	  /// Writes an SQL structured type value to the stream.
	  /// </summary>
	  /// <param name="x"> a <code>Struct</code> object representing data of an SQL
	  /// structured type
	  /// </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeStruct(Struct x) throws SQLException;
	  void WriteStruct(Struct x);

	  /// <summary>
	  /// Writes an SQL <code>ARRAY</code> value to the stream.
	  /// </summary>
	  /// <param name="x"> an <code>Array</code> object representing data of an SQL
	  /// <code>ARRAY</code> type
	  /// </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeArray(Array x) throws SQLException;
	  void WriteArray(Array x);

		 //--------------------------- JDBC 3.0 ------------------------

		 /// <summary>
		 /// Writes a SQL <code>DATALINK</code> value to the stream.
		 /// </summary>
		 /// <param name="x"> a <code>java.net.URL</code> object representing the data
		 /// of SQL DATALINK type
		 /// </param>
		 /// <exception cref="SQLException"> if a database access error occurs </exception>
		 /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		 /// this method
		 /// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeURL(java.net.URL x) throws SQLException;
		 void WriteURL(java.net.URL x);

		 //--------------------------- JDBC 4.0 ------------------------

	  /// <summary>
	  /// Writes the next attribute to the stream as a <code>String</code>
	  /// in the Java programming language. The driver converts this to a
	  /// SQL <code>NCHAR</code> or
	  /// <code>NVARCHAR</code> or <code>LONGNVARCHAR</code> value
	  /// (depending on the argument's
	  /// size relative to the driver's limits on <code>NVARCHAR</code> values)
	  /// when it sends it to the stream.
	  /// </summary>
	  /// <param name="x"> the value to pass to the database </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeNString(String x) throws SQLException;
	  void WriteNString(String x);

	  /// <summary>
	  /// Writes an SQL <code>NCLOB</code> value to the stream.
	  /// </summary>
	  /// <param name="x"> a <code>NClob</code> object representing data of an SQL
	  /// <code>NCLOB</code> value
	  /// </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeNClob(NClob x) throws SQLException;
	  void WriteNClob(NClob x);


	  /// <summary>
	  /// Writes an SQL <code>ROWID</code> value to the stream.
	  /// </summary>
	  /// <param name="x"> a <code>RowId</code> object representing data of an SQL
	  /// <code>ROWID</code> value
	  /// </param>
	  /// <exception cref="SQLException"> if a database access error occurs </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeRowId(RowId x) throws SQLException;
	  void WriteRowId(RowId x);


	  /// <summary>
	  /// Writes an SQL <code>XML</code> value to the stream.
	  /// </summary>
	  /// <param name="x"> a <code>SQLXML</code> object representing data of an SQL
	  /// <code>XML</code> value
	  /// </param>
	  /// <exception cref="SQLException"> if a database access error occurs,
	  /// the <code>java.xml.transform.Result</code>,
	  ///  <code>Writer</code> or <code>OutputStream</code> has not been closed for the <code>SQLXML</code> object or
	  ///  if there is an error processing the XML value.  The <code>getCause</code> method
	  ///  of the exception may provide a more detailed exception, for example, if the
	  ///  stream does not contain valid XML. </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  /// this method
	  /// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeSQLXML(SQLXML x) throws SQLException;
	  void WriteSQLXML(SQLXML x);

	  //--------------------------JDBC 4.2 -----------------------------

	  /// <summary>
	  /// Writes to the stream the data contained in the given object. The
	  /// object will be converted to the specified targetSqlType
	  /// before being sent to the stream.
	  /// <para>
	  /// When the {@code object} is {@code null}, this
	  /// method writes an SQL {@code NULL} to the stream.
	  /// </para>
	  /// <para>
	  /// If the object has a custom mapping (is of a class implementing the
	  /// interface {@code SQLData}),
	  /// the JDBC driver should call the method {@code SQLData.writeSQL} to
	  /// write it to the SQL data stream.
	  /// If, on the other hand, the object is of a class implementing
	  /// {@code Ref}, {@code Blob}, {@code Clob},  {@code NClob},
	  ///  {@code Struct}, {@code java.net.URL},
	  /// or {@code Array}, the driver should pass it to the database as a
	  /// value of the corresponding SQL type.
	  /// <P>
	  /// The default implementation will throw {@code SQLFeatureNotSupportedException}
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="x"> the object containing the input parameter value </param>
	  /// <param name="targetSqlType"> the SQL type to be sent to the database. </param>
	  /// <exception cref="SQLException"> if a database access error occurs  or
	  ///            if the Java Object specified by x is an InputStream
	  ///            or Reader object and the value of the scale parameter is less
	  ///            than zero </exception>
	  /// <exception cref="SQLFeatureNotSupportedException"> if
	  /// the JDBC driver does not support this data type </exception>
	  /// <seealso cref= JDBCType </seealso>
	  /// <seealso cref= SQLType
	  /// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//	  default void writeObject(Object x, SQLType targetSqlType) throws SQLException
	//  {
	//		throw new SQLFeatureNotSupportedException();
	//  }

	 }


}