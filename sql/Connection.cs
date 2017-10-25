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
	/// <P>A connection (session) with a specific
	/// database. SQL statements are executed and results are returned
	/// within the context of a connection.
	/// <P>
	/// A <code>Connection</code> object's database is able to provide information
	/// describing its tables, its supported SQL grammar, its stored
	/// procedures, the capabilities of this connection, and so on. This
	/// information is obtained with the <code>getMetaData</code> method.
	/// 
	/// <P><B>Note:</B> When configuring a <code>Connection</code>, JDBC applications
	///  should use the appropriate <code>Connection</code> method such as
	///  <code>setAutoCommit</code> or <code>setTransactionIsolation</code>.
	///  Applications should not invoke SQL commands directly to change the connection's
	///   configuration when there is a JDBC method available.  By default a <code>Connection</code> object is in
	/// auto-commit mode, which means that it automatically commits changes
	/// after executing each statement. If auto-commit mode has been
	/// disabled, the method <code>commit</code> must be called explicitly in
	/// order to commit changes; otherwise, database changes will not be saved.
	/// <P>
	/// A new <code>Connection</code> object created using the JDBC 2.1 core API
	/// has an initially empty type map associated with it. A user may enter a
	/// custom mapping for a UDT in this type map.
	/// When a UDT is retrieved from a data source with the
	/// method <code>ResultSet.getObject</code>, the <code>getObject</code> method
	/// will check the connection's type map to see if there is an entry for that
	/// UDT.  If so, the <code>getObject</code> method will map the UDT to the
	/// class indicated.  If there is no entry, the UDT will be mapped using the
	/// standard mapping.
	/// <para>
	/// A user may create a new type map, which is a <code>java.util.Map</code>
	/// object, make an entry in it, and pass it to the <code>java.sql</code>
	/// methods that can perform custom mapping.  In this case, the method
	/// will use the given type map instead of the one associated with
	/// the connection.
	/// </para>
	/// <para>
	/// For example, the following code fragment specifies that the SQL
	/// type <code>ATHLETES</code> will be mapped to the class
	/// <code>Athletes</code> in the Java programming language.
	/// The code fragment retrieves the type map for the <code>Connection
	/// </code> object <code>con</code>, inserts the entry into it, and then sets
	/// the type map with the new entry as the connection's type map.
	/// <pre>
	///      java.util.Map map = con.getTypeMap();
	///      map.put("mySchemaName.ATHLETES", Class.forName("Athletes"));
	///      con.setTypeMap(map);
	/// </pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= DriverManager#getConnection </seealso>
	/// <seealso cref= Statement </seealso>
	/// <seealso cref= ResultSet </seealso>
	/// <seealso cref= DatabaseMetaData </seealso>
	public interface Connection : Wrapper, AutoCloseable
	{

		/// <summary>
		/// Creates a <code>Statement</code> object for sending
		/// SQL statements to the database.
		/// SQL statements without parameters are normally
		/// executed using <code>Statement</code> objects. If the same SQL statement
		/// is executed many times, it may be more efficient to use a
		/// <code>PreparedStatement</code> object.
		/// <P>
		/// Result sets created using the returned <code>Statement</code>
		/// object will by default be type <code>TYPE_FORWARD_ONLY</code>
		/// and have a concurrency level of <code>CONCUR_READ_ONLY</code>.
		/// The holdability of the created result sets can be determined by
		/// calling <seealso cref="#getHoldability"/>.
		/// </summary>
		/// <returns> a new default <code>Statement</code> object </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Statement createStatement() throws SQLException;
		Statement CreateStatement();

		/// <summary>
		/// Creates a <code>PreparedStatement</code> object for sending
		/// parameterized SQL statements to the database.
		/// <P>
		/// A SQL statement with or without IN parameters can be
		/// pre-compiled and stored in a <code>PreparedStatement</code> object. This
		/// object can then be used to efficiently execute this statement
		/// multiple times.
		/// 
		/// <P><B>Note:</B> This method is optimized for handling
		/// parametric SQL statements that benefit from precompilation. If
		/// the driver supports precompilation,
		/// the method <code>prepareStatement</code> will send
		/// the statement to the database for precompilation. Some drivers
		/// may not support precompilation. In this case, the statement may
		/// not be sent to the database until the <code>PreparedStatement</code>
		/// object is executed.  This has no direct effect on users; however, it does
		/// affect which methods throw certain <code>SQLException</code> objects.
		/// <P>
		/// Result sets created using the returned <code>PreparedStatement</code>
		/// object will by default be type <code>TYPE_FORWARD_ONLY</code>
		/// and have a concurrency level of <code>CONCUR_READ_ONLY</code>.
		/// The holdability of the created result sets can be determined by
		/// calling <seealso cref="#getHoldability"/>.
		/// </summary>
		/// <param name="sql"> an SQL statement that may contain one or more '?' IN
		/// parameter placeholders </param>
		/// <returns> a new default <code>PreparedStatement</code> object containing the
		/// pre-compiled SQL statement </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: PreparedStatement prepareStatement(String sql) throws SQLException;
		PreparedStatement PrepareStatement(String sql);

		/// <summary>
		/// Creates a <code>CallableStatement</code> object for calling
		/// database stored procedures.
		/// The <code>CallableStatement</code> object provides
		/// methods for setting up its IN and OUT parameters, and
		/// methods for executing the call to a stored procedure.
		/// 
		/// <P><B>Note:</B> This method is optimized for handling stored
		/// procedure call statements. Some drivers may send the call
		/// statement to the database when the method <code>prepareCall</code>
		/// is done; others
		/// may wait until the <code>CallableStatement</code> object
		/// is executed. This has no
		/// direct effect on users; however, it does affect which method
		/// throws certain SQLExceptions.
		/// <P>
		/// Result sets created using the returned <code>CallableStatement</code>
		/// object will by default be type <code>TYPE_FORWARD_ONLY</code>
		/// and have a concurrency level of <code>CONCUR_READ_ONLY</code>.
		/// The holdability of the created result sets can be determined by
		/// calling <seealso cref="#getHoldability"/>.
		/// </summary>
		/// <param name="sql"> an SQL statement that may contain one or more '?'
		/// parameter placeholders. Typically this statement is specified using JDBC
		/// call escape syntax. </param>
		/// <returns> a new default <code>CallableStatement</code> object containing the
		/// pre-compiled SQL statement </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: CallableStatement prepareCall(String sql) throws SQLException;
		CallableStatement PrepareCall(String sql);

		/// <summary>
		/// Converts the given SQL statement into the system's native SQL grammar.
		/// A driver may convert the JDBC SQL grammar into its system's
		/// native SQL grammar prior to sending it. This method returns the
		/// native form of the statement that the driver would have sent.
		/// </summary>
		/// <param name="sql"> an SQL statement that may contain one or more '?'
		/// parameter placeholders </param>
		/// <returns> the native form of this statement </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String nativeSQL(String sql) throws SQLException;
		String NativeSQL(String sql);

		/// <summary>
		/// Sets this connection's auto-commit mode to the given state.
		/// If a connection is in auto-commit mode, then all its SQL
		/// statements will be executed and committed as individual
		/// transactions.  Otherwise, its SQL statements are grouped into
		/// transactions that are terminated by a call to either
		/// the method <code>commit</code> or the method <code>rollback</code>.
		/// By default, new connections are in auto-commit
		/// mode.
		/// <P>
		/// The commit occurs when the statement completes. The time when the statement
		/// completes depends on the type of SQL Statement:
		/// <ul>
		/// <li>For DML statements, such as Insert, Update or Delete, and DDL statements,
		/// the statement is complete as soon as it has finished executing.
		/// <li>For Select statements, the statement is complete when the associated result
		/// set is closed.
		/// <li>For <code>CallableStatement</code> objects or for statements that return
		/// multiple results, the statement is complete
		/// when all of the associated result sets have been closed, and all update
		/// counts and output parameters have been retrieved.
		/// </ul>
		/// <P>
		/// <B>NOTE:</B>  If this method is called during a transaction and the
		/// auto-commit mode is changed, the transaction is committed.  If
		/// <code>setAutoCommit</code> is called and the auto-commit mode is
		/// not changed, the call is a no-op.
		/// </summary>
		/// <param name="autoCommit"> <code>true</code> to enable auto-commit mode;
		///         <code>false</code> to disable it </param>
		/// <exception cref="SQLException"> if a database access error occurs,
		///  setAutoCommit(true) is called while participating in a distributed transaction,
		/// or this method is called on a closed connection </exception>
		/// <seealso cref= #getAutoCommit </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setAutoCommit(boolean autoCommit) throws SQLException;
		bool AutoCommit {set;get;}

		/// <summary>
		/// Retrieves the current auto-commit mode for this <code>Connection</code>
		/// object.
		/// </summary>
		/// <returns> the current state of this <code>Connection</code> object's
		///         auto-commit mode </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
		/// <seealso cref= #setAutoCommit </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean getAutoCommit() throws SQLException;

		/// <summary>
		/// Makes all changes made since the previous
		/// commit/rollback permanent and releases any database locks
		/// currently held by this <code>Connection</code> object.
		/// This method should be
		/// used only when auto-commit mode has been disabled.
		/// </summary>
		/// <exception cref="SQLException"> if a database access error occurs,
		/// this method is called while participating in a distributed transaction,
		/// if this method is called on a closed connection or this
		///            <code>Connection</code> object is in auto-commit mode </exception>
		/// <seealso cref= #setAutoCommit </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void commit() throws SQLException;
		void Commit();

		/// <summary>
		/// Undoes all changes made in the current transaction
		/// and releases any database locks currently held
		/// by this <code>Connection</code> object. This method should be
		/// used only when auto-commit mode has been disabled.
		/// </summary>
		/// <exception cref="SQLException"> if a database access error occurs,
		/// this method is called while participating in a distributed transaction,
		/// this method is called on a closed connection or this
		///            <code>Connection</code> object is in auto-commit mode </exception>
		/// <seealso cref= #setAutoCommit </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void rollback() throws SQLException;
		void Rollback();

		/// <summary>
		/// Releases this <code>Connection</code> object's database and JDBC resources
		/// immediately instead of waiting for them to be automatically released.
		/// <P>
		/// Calling the method <code>close</code> on a <code>Connection</code>
		/// object that is already closed is a no-op.
		/// <P>
		/// It is <b>strongly recommended</b> that an application explicitly
		/// commits or rolls back an active transaction prior to calling the
		/// <code>close</code> method.  If the <code>close</code> method is called
		/// and there is an active transaction, the results are implementation-defined.
		/// <P>
		/// </summary>
		/// <exception cref="SQLException"> SQLException if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void close() throws SQLException;
		void Close();

		/// <summary>
		/// Retrieves whether this <code>Connection</code> object has been
		/// closed.  A connection is closed if the method <code>close</code>
		/// has been called on it or if certain fatal errors have occurred.
		/// This method is guaranteed to return <code>true</code> only when
		/// it is called after the method <code>Connection.close</code> has
		/// been called.
		/// <P>
		/// This method generally cannot be called to determine whether a
		/// connection to a database is valid or invalid.  A typical client
		/// can determine that a connection is invalid by catching any
		/// exceptions that might be thrown when an operation is attempted.
		/// </summary>
		/// <returns> <code>true</code> if this <code>Connection</code> object
		///         is closed; <code>false</code> if it is still open </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isClosed() throws SQLException;
		bool Closed {get;}

		//======================================================================
		// Advanced features:

		/// <summary>
		/// Retrieves a <code>DatabaseMetaData</code> object that contains
		/// metadata about the database to which this
		/// <code>Connection</code> object represents a connection.
		/// The metadata includes information about the database's
		/// tables, its supported SQL grammar, its stored
		/// procedures, the capabilities of this connection, and so on.
		/// </summary>
		/// <returns> a <code>DatabaseMetaData</code> object for this
		///         <code>Connection</code> object </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: DatabaseMetaData getMetaData() throws SQLException;
		DatabaseMetaData MetaData {get;}

		/// <summary>
		/// Puts this connection in read-only mode as a hint to the driver to enable
		/// database optimizations.
		/// 
		/// <P><B>Note:</B> This method cannot be called during a transaction.
		/// </summary>
		/// <param name="readOnly"> <code>true</code> enables read-only mode;
		///        <code>false</code> disables it </param>
		/// <exception cref="SQLException"> if a database access error occurs, this
		///  method is called on a closed connection or this
		///            method is called during a transaction </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setReadOnly(boolean readOnly) throws SQLException;
		bool ReadOnly {set;get;}

		/// <summary>
		/// Retrieves whether this <code>Connection</code>
		/// object is in read-only mode.
		/// </summary>
		/// <returns> <code>true</code> if this <code>Connection</code> object
		///         is read-only; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> SQLException if a database access error occurs
		/// or this method is called on a closed connection </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isReadOnly() throws SQLException;

		/// <summary>
		/// Sets the given catalog name in order to select
		/// a subspace of this <code>Connection</code> object's database
		/// in which to work.
		/// <P>
		/// If the driver does not support catalogs, it will
		/// silently ignore this request.
		/// <para>
		/// Calling {@code setCatalog} has no effect on previously created or prepared
		/// {@code Statement} objects. It is implementation defined whether a DBMS
		/// prepare operation takes place immediately when the {@code Connection}
		/// method {@code prepareStatement} or {@code prepareCall} is invoked.
		/// For maximum portability, {@code setCatalog} should be called before a
		/// {@code Statement} is created or prepared.
		/// 
		/// </para>
		/// </summary>
		/// <param name="catalog"> the name of a catalog (subspace in this
		///        <code>Connection</code> object's database) in which to work </param>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
		/// <seealso cref= #getCatalog </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setCatalog(String catalog) throws SQLException;
		String Catalog {set;get;}

		/// <summary>
		/// Retrieves this <code>Connection</code> object's current catalog name.
		/// </summary>
		/// <returns> the current catalog name or <code>null</code> if there is none </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
		/// <seealso cref= #setCatalog </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getCatalog() throws SQLException;

		/// <summary>
		/// A constant indicating that transactions are not supported.
		/// </summary>

		/// <summary>
		/// A constant indicating that
		/// dirty reads, non-repeatable reads and phantom reads can occur.
		/// This level allows a row changed by one transaction to be read
		/// by another transaction before any changes in that row have been
		/// committed (a "dirty read").  If any of the changes are rolled back,
		/// the second transaction will have retrieved an invalid row.
		/// </summary>

		/// <summary>
		/// A constant indicating that
		/// dirty reads are prevented; non-repeatable reads and phantom
		/// reads can occur.  This level only prohibits a transaction
		/// from reading a row with uncommitted changes in it.
		/// </summary>

		/// <summary>
		/// A constant indicating that
		/// dirty reads and non-repeatable reads are prevented; phantom
		/// reads can occur.  This level prohibits a transaction from
		/// reading a row with uncommitted changes in it, and it also
		/// prohibits the situation where one transaction reads a row,
		/// a second transaction alters the row, and the first transaction
		/// rereads the row, getting different values the second time
		/// (a "non-repeatable read").
		/// </summary>

		/// <summary>
		/// A constant indicating that
		/// dirty reads, non-repeatable reads and phantom reads are prevented.
		/// This level includes the prohibitions in
		/// <code>TRANSACTION_REPEATABLE_READ</code> and further prohibits the
		/// situation where one transaction reads all rows that satisfy
		/// a <code>WHERE</code> condition, a second transaction inserts a row that
		/// satisfies that <code>WHERE</code> condition, and the first transaction
		/// rereads for the same condition, retrieving the additional
		/// "phantom" row in the second read.
		/// </summary>

		/// <summary>
		/// Attempts to change the transaction isolation level for this
		/// <code>Connection</code> object to the one given.
		/// The constants defined in the interface <code>Connection</code>
		/// are the possible transaction isolation levels.
		/// <P>
		/// <B>Note:</B> If this method is called during a transaction, the result
		/// is implementation-defined.
		/// </summary>
		/// <param name="level"> one of the following <code>Connection</code> constants:
		///        <code>Connection.TRANSACTION_READ_UNCOMMITTED</code>,
		///        <code>Connection.TRANSACTION_READ_COMMITTED</code>,
		///        <code>Connection.TRANSACTION_REPEATABLE_READ</code>, or
		///        <code>Connection.TRANSACTION_SERIALIZABLE</code>.
		///        (Note that <code>Connection.TRANSACTION_NONE</code> cannot be used
		///        because it specifies that transactions are not supported.) </param>
		/// <exception cref="SQLException"> if a database access error occurs, this
		/// method is called on a closed connection
		///            or the given parameter is not one of the <code>Connection</code>
		///            constants </exception>
		/// <seealso cref= DatabaseMetaData#supportsTransactionIsolationLevel </seealso>
		/// <seealso cref= #getTransactionIsolation </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setTransactionIsolation(int level) throws SQLException;
		int TransactionIsolation {set;get;}

		/// <summary>
		/// Retrieves this <code>Connection</code> object's current
		/// transaction isolation level.
		/// </summary>
		/// <returns> the current transaction isolation level, which will be one
		///         of the following constants:
		///        <code>Connection.TRANSACTION_READ_UNCOMMITTED</code>,
		///        <code>Connection.TRANSACTION_READ_COMMITTED</code>,
		///        <code>Connection.TRANSACTION_REPEATABLE_READ</code>,
		///        <code>Connection.TRANSACTION_SERIALIZABLE</code>, or
		///        <code>Connection.TRANSACTION_NONE</code>. </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
		/// <seealso cref= #setTransactionIsolation </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getTransactionIsolation() throws SQLException;

		/// <summary>
		/// Retrieves the first warning reported by calls on this
		/// <code>Connection</code> object.  If there is more than one
		/// warning, subsequent warnings will be chained to the first one
		/// and can be retrieved by calling the method
		/// <code>SQLWarning.getNextWarning</code> on the warning
		/// that was retrieved previously.
		/// <P>
		/// This method may not be
		/// called on a closed connection; doing so will cause an
		/// <code>SQLException</code> to be thrown.
		/// 
		/// <P><B>Note:</B> Subsequent warnings will be chained to this
		/// SQLWarning.
		/// </summary>
		/// <returns> the first <code>SQLWarning</code> object or <code>null</code>
		///         if there are none </returns>
		/// <exception cref="SQLException"> if a database access error occurs or
		///            this method is called on a closed connection </exception>
		/// <seealso cref= SQLWarning </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: SQLWarning getWarnings() throws SQLException;
		SQLWarning Warnings {get;}

		/// <summary>
		/// Clears all warnings reported for this <code>Connection</code> object.
		/// After a call to this method, the method <code>getWarnings</code>
		/// returns <code>null</code> until a new warning is
		/// reported for this <code>Connection</code> object.
		/// </summary>
		/// <exception cref="SQLException"> SQLException if a database access error occurs
		/// or this method is called on a closed connection </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void clearWarnings() throws SQLException;
		void ClearWarnings();


		//--------------------------JDBC 2.0-----------------------------

		/// <summary>
		/// Creates a <code>Statement</code> object that will generate
		/// <code>ResultSet</code> objects with the given type and concurrency.
		/// This method is the same as the <code>createStatement</code> method
		/// above, but it allows the default result set
		/// type and concurrency to be overridden.
		/// The holdability of the created result sets can be determined by
		/// calling <seealso cref="#getHoldability"/>.
		/// </summary>
		/// <param name="resultSetType"> a result set type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <param name="resultSetConcurrency"> a concurrency type; one of
		///        <code>ResultSet.CONCUR_READ_ONLY</code> or
		///        <code>ResultSet.CONCUR_UPDATABLE</code> </param>
		/// <returns> a new <code>Statement</code> object that will generate
		///         <code>ResultSet</code> objects with the given type and
		///         concurrency </returns>
		/// <exception cref="SQLException"> if a database access error occurs, this
		/// method is called on a closed connection
		///         or the given parameters are not <code>ResultSet</code>
		///         constants indicating type and concurrency </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method or this method is not supported for the specified result
		/// set type and result set concurrency.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Statement createStatement(int resultSetType, int resultSetConcurrency) throws SQLException;
		Statement CreateStatement(int resultSetType, int resultSetConcurrency);

		/// 
		/// <summary>
		/// Creates a <code>PreparedStatement</code> object that will generate
		/// <code>ResultSet</code> objects with the given type and concurrency.
		/// This method is the same as the <code>prepareStatement</code> method
		/// above, but it allows the default result set
		/// type and concurrency to be overridden.
		/// The holdability of the created result sets can be determined by
		/// calling <seealso cref="#getHoldability"/>.
		/// </summary>
		/// <param name="sql"> a <code>String</code> object that is the SQL statement to
		///            be sent to the database; may contain one or more '?' IN
		///            parameters </param>
		/// <param name="resultSetType"> a result set type; one of
		///         <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///         <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///         <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <param name="resultSetConcurrency"> a concurrency type; one of
		///         <code>ResultSet.CONCUR_READ_ONLY</code> or
		///         <code>ResultSet.CONCUR_UPDATABLE</code> </param>
		/// <returns> a new PreparedStatement object containing the
		/// pre-compiled SQL statement that will produce <code>ResultSet</code>
		/// objects with the given type and concurrency </returns>
		/// <exception cref="SQLException"> if a database access error occurs, this
		/// method is called on a closed connection
		///         or the given parameters are not <code>ResultSet</code>
		///         constants indicating type and concurrency </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method or this method is not supported for the specified result
		/// set type and result set concurrency.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: PreparedStatement prepareStatement(String sql, int resultSetType, int resultSetConcurrency) throws SQLException;
		PreparedStatement PrepareStatement(String sql, int resultSetType, int resultSetConcurrency);

		/// <summary>
		/// Creates a <code>CallableStatement</code> object that will generate
		/// <code>ResultSet</code> objects with the given type and concurrency.
		/// This method is the same as the <code>prepareCall</code> method
		/// above, but it allows the default result set
		/// type and concurrency to be overridden.
		/// The holdability of the created result sets can be determined by
		/// calling <seealso cref="#getHoldability"/>.
		/// </summary>
		/// <param name="sql"> a <code>String</code> object that is the SQL statement to
		///            be sent to the database; may contain on or more '?' parameters </param>
		/// <param name="resultSetType"> a result set type; one of
		///         <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///         <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///         <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <param name="resultSetConcurrency"> a concurrency type; one of
		///         <code>ResultSet.CONCUR_READ_ONLY</code> or
		///         <code>ResultSet.CONCUR_UPDATABLE</code> </param>
		/// <returns> a new <code>CallableStatement</code> object containing the
		/// pre-compiled SQL statement that will produce <code>ResultSet</code>
		/// objects with the given type and concurrency </returns>
		/// <exception cref="SQLException"> if a database access error occurs, this method
		/// is called on a closed connection
		///         or the given parameters are not <code>ResultSet</code>
		///         constants indicating type and concurrency </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method or this method is not supported for the specified result
		/// set type and result set concurrency.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: CallableStatement prepareCall(String sql, int resultSetType, int resultSetConcurrency) throws SQLException;
		CallableStatement PrepareCall(String sql, int resultSetType, int resultSetConcurrency);

		/// <summary>
		/// Retrieves the <code>Map</code> object associated with this
		/// <code>Connection</code> object.
		/// Unless the application has added an entry, the type map returned
		/// will be empty.
		/// <para>
		/// You must invoke <code>setTypeMap</code> after making changes to the
		/// <code>Map</code> object returned from
		///  <code>getTypeMap</code> as a JDBC driver may create an internal
		/// copy of the <code>Map</code> object passed to <code>setTypeMap</code>:
		/// 
		/// <pre>
		///      Map&lt;String,Class&lt;?&gt;&gt; myMap = con.getTypeMap();
		///      myMap.put("mySchemaName.ATHLETES", Athletes.class);
		///      con.setTypeMap(myMap);
		/// </pre>
		/// </para>
		/// </summary>
		/// <returns> the <code>java.util.Map</code> object associated
		///         with this <code>Connection</code> object </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.2 </exception>
		/// <seealso cref= #setTypeMap </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.util.Map<String,Class> getTypeMap() throws SQLException;
		IDictionary<String, Class> TypeMap {get;set;}

		/// <summary>
		/// Installs the given <code>TypeMap</code> object as the type map for
		/// this <code>Connection</code> object.  The type map will be used for the
		/// custom mapping of SQL structured types and distinct types.
		/// <para>
		/// You must set the the values for the <code>TypeMap</code> prior to
		/// callng <code>setMap</code> as a JDBC driver may create an internal copy
		/// of the <code>TypeMap</code>:
		/// 
		/// <pre>
		///      Map myMap&lt;String,Class&lt;?&gt;&gt; = new HashMap&lt;String,Class&lt;?&gt;&gt;();
		///      myMap.put("mySchemaName.ATHLETES", Athletes.class);
		///      con.setTypeMap(myMap);
		/// </pre>
		/// </para>
		/// </summary>
		/// <param name="map"> the <code>java.util.Map</code> object to install
		///        as the replacement for this <code>Connection</code>
		///        object's default type map </param>
		/// <exception cref="SQLException"> if a database access error occurs, this
		/// method is called on a closed connection or
		///        the given parameter is not a <code>java.util.Map</code>
		///        object </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.2 </exception>
		/// <seealso cref= #getTypeMap </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setTypeMap(java.util.Map<String,Class> map) throws SQLException;

		//--------------------------JDBC 3.0-----------------------------


		/// <summary>
		/// Changes the default holdability of <code>ResultSet</code> objects
		/// created using this <code>Connection</code> object to the given
		/// holdability.  The default holdability of <code>ResultSet</code> objects
		/// can be be determined by invoking
		/// <seealso cref="DatabaseMetaData#getResultSetHoldability"/>.
		/// </summary>
		/// <param name="holdability"> a <code>ResultSet</code> holdability constant; one of
		///        <code>ResultSet.HOLD_CURSORS_OVER_COMMIT</code> or
		///        <code>ResultSet.CLOSE_CURSORS_AT_COMMIT</code> </param>
		/// <exception cref="SQLException"> if a database access occurs, this method is called
		/// on a closed connection, or the given parameter
		///         is not a <code>ResultSet</code> constant indicating holdability </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the given holdability is not supported </exception>
		/// <seealso cref= #getHoldability </seealso>
		/// <seealso cref= DatabaseMetaData#getResultSetHoldability </seealso>
		/// <seealso cref= ResultSet
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setHoldability(int holdability) throws SQLException;
		int Holdability {set;get;}

		/// <summary>
		/// Retrieves the current holdability of <code>ResultSet</code> objects
		/// created using this <code>Connection</code> object.
		/// </summary>
		/// <returns> the holdability, one of
		///        <code>ResultSet.HOLD_CURSORS_OVER_COMMIT</code> or
		///        <code>ResultSet.CLOSE_CURSORS_AT_COMMIT</code> </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
		/// <seealso cref= #setHoldability </seealso>
		/// <seealso cref= DatabaseMetaData#getResultSetHoldability </seealso>
		/// <seealso cref= ResultSet
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getHoldability() throws SQLException;

		/// <summary>
		/// Creates an unnamed savepoint in the current transaction and
		/// returns the new <code>Savepoint</code> object that represents it.
		/// 
		/// <para> if setSavepoint is invoked outside of an active transaction, a transaction will be started at this newly created
		/// savepoint.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the new <code>Savepoint</code> object </returns>
		/// <exception cref="SQLException"> if a database access error occurs,
		/// this method is called while participating in a distributed transaction,
		/// this method is called on a closed connection
		///            or this <code>Connection</code> object is currently in
		///            auto-commit mode </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= Savepoint
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Savepoint setSavepoint() throws SQLException;
		Savepoint SetSavepoint();

		/// <summary>
		/// Creates a savepoint with the given name in the current transaction
		/// and returns the new <code>Savepoint</code> object that represents it.
		/// 
		/// <para> if setSavepoint is invoked outside of an active transaction, a transaction will be started at this newly created
		/// savepoint.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> a <code>String</code> containing the name of the savepoint </param>
		/// <returns> the new <code>Savepoint</code> object </returns>
		/// <exception cref="SQLException"> if a database access error occurs,
		/// this method is called while participating in a distributed transaction,
		/// this method is called on a closed connection
		///            or this <code>Connection</code> object is currently in
		///            auto-commit mode </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= Savepoint
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Savepoint setSavepoint(String name) throws SQLException;
		Savepoint SetSavepoint(String name);

		/// <summary>
		/// Undoes all changes made after the given <code>Savepoint</code> object
		/// was set.
		/// <P>
		/// This method should be used only when auto-commit has been disabled.
		/// </summary>
		/// <param name="savepoint"> the <code>Savepoint</code> object to roll back to </param>
		/// <exception cref="SQLException"> if a database access error occurs,
		/// this method is called while participating in a distributed transaction,
		/// this method is called on a closed connection,
		///            the <code>Savepoint</code> object is no longer valid,
		///            or this <code>Connection</code> object is currently in
		///            auto-commit mode </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= Savepoint </seealso>
		/// <seealso cref= #rollback
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void rollback(Savepoint savepoint) throws SQLException;
		void Rollback(Savepoint savepoint);

		/// <summary>
		/// Removes the specified <code>Savepoint</code>  and subsequent <code>Savepoint</code> objects from the current
		/// transaction. Any reference to the savepoint after it have been removed
		/// will cause an <code>SQLException</code> to be thrown.
		/// </summary>
		/// <param name="savepoint"> the <code>Savepoint</code> object to be removed </param>
		/// <exception cref="SQLException"> if a database access error occurs, this
		///  method is called on a closed connection or
		///            the given <code>Savepoint</code> object is not a valid
		///            savepoint in the current transaction </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void releaseSavepoint(Savepoint savepoint) throws SQLException;
		void ReleaseSavepoint(Savepoint savepoint);

		/// <summary>
		/// Creates a <code>Statement</code> object that will generate
		/// <code>ResultSet</code> objects with the given type, concurrency,
		/// and holdability.
		/// This method is the same as the <code>createStatement</code> method
		/// above, but it allows the default result set
		/// type, concurrency, and holdability to be overridden.
		/// </summary>
		/// <param name="resultSetType"> one of the following <code>ResultSet</code>
		///        constants:
		///         <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///         <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///         <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <param name="resultSetConcurrency"> one of the following <code>ResultSet</code>
		///        constants:
		///         <code>ResultSet.CONCUR_READ_ONLY</code> or
		///         <code>ResultSet.CONCUR_UPDATABLE</code> </param>
		/// <param name="resultSetHoldability"> one of the following <code>ResultSet</code>
		///        constants:
		///         <code>ResultSet.HOLD_CURSORS_OVER_COMMIT</code> or
		///         <code>ResultSet.CLOSE_CURSORS_AT_COMMIT</code> </param>
		/// <returns> a new <code>Statement</code> object that will generate
		///         <code>ResultSet</code> objects with the given type,
		///         concurrency, and holdability </returns>
		/// <exception cref="SQLException"> if a database access error occurs, this
		/// method is called on a closed connection
		///            or the given parameters are not <code>ResultSet</code>
		///            constants indicating type, concurrency, and holdability </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method or this method is not supported for the specified result
		/// set type, result set holdability and result set concurrency. </exception>
		/// <seealso cref= ResultSet
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Statement createStatement(int resultSetType, int resultSetConcurrency, int resultSetHoldability) throws SQLException;
		Statement CreateStatement(int resultSetType, int resultSetConcurrency, int resultSetHoldability);

		/// <summary>
		/// Creates a <code>PreparedStatement</code> object that will generate
		/// <code>ResultSet</code> objects with the given type, concurrency,
		/// and holdability.
		/// <P>
		/// This method is the same as the <code>prepareStatement</code> method
		/// above, but it allows the default result set
		/// type, concurrency, and holdability to be overridden.
		/// </summary>
		/// <param name="sql"> a <code>String</code> object that is the SQL statement to
		///            be sent to the database; may contain one or more '?' IN
		///            parameters </param>
		/// <param name="resultSetType"> one of the following <code>ResultSet</code>
		///        constants:
		///         <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///         <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///         <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <param name="resultSetConcurrency"> one of the following <code>ResultSet</code>
		///        constants:
		///         <code>ResultSet.CONCUR_READ_ONLY</code> or
		///         <code>ResultSet.CONCUR_UPDATABLE</code> </param>
		/// <param name="resultSetHoldability"> one of the following <code>ResultSet</code>
		///        constants:
		///         <code>ResultSet.HOLD_CURSORS_OVER_COMMIT</code> or
		///         <code>ResultSet.CLOSE_CURSORS_AT_COMMIT</code> </param>
		/// <returns> a new <code>PreparedStatement</code> object, containing the
		///         pre-compiled SQL statement, that will generate
		///         <code>ResultSet</code> objects with the given type,
		///         concurrency, and holdability </returns>
		/// <exception cref="SQLException"> if a database access error occurs, this
		/// method is called on a closed connection
		///            or the given parameters are not <code>ResultSet</code>
		///            constants indicating type, concurrency, and holdability </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method or this method is not supported for the specified result
		/// set type, result set holdability and result set concurrency. </exception>
		/// <seealso cref= ResultSet
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: PreparedStatement prepareStatement(String sql, int resultSetType, int resultSetConcurrency, int resultSetHoldability) throws SQLException;
		PreparedStatement PrepareStatement(String sql, int resultSetType, int resultSetConcurrency, int resultSetHoldability);

		/// <summary>
		/// Creates a <code>CallableStatement</code> object that will generate
		/// <code>ResultSet</code> objects with the given type and concurrency.
		/// This method is the same as the <code>prepareCall</code> method
		/// above, but it allows the default result set
		/// type, result set concurrency type and holdability to be overridden.
		/// </summary>
		/// <param name="sql"> a <code>String</code> object that is the SQL statement to
		///            be sent to the database; may contain on or more '?' parameters </param>
		/// <param name="resultSetType"> one of the following <code>ResultSet</code>
		///        constants:
		///         <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///         <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///         <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <param name="resultSetConcurrency"> one of the following <code>ResultSet</code>
		///        constants:
		///         <code>ResultSet.CONCUR_READ_ONLY</code> or
		///         <code>ResultSet.CONCUR_UPDATABLE</code> </param>
		/// <param name="resultSetHoldability"> one of the following <code>ResultSet</code>
		///        constants:
		///         <code>ResultSet.HOLD_CURSORS_OVER_COMMIT</code> or
		///         <code>ResultSet.CLOSE_CURSORS_AT_COMMIT</code> </param>
		/// <returns> a new <code>CallableStatement</code> object, containing the
		///         pre-compiled SQL statement, that will generate
		///         <code>ResultSet</code> objects with the given type,
		///         concurrency, and holdability </returns>
		/// <exception cref="SQLException"> if a database access error occurs, this
		/// method is called on a closed connection
		///            or the given parameters are not <code>ResultSet</code>
		///            constants indicating type, concurrency, and holdability </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method or this method is not supported for the specified result
		/// set type, result set holdability and result set concurrency. </exception>
		/// <seealso cref= ResultSet
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: CallableStatement prepareCall(String sql, int resultSetType, int resultSetConcurrency, int resultSetHoldability) throws SQLException;
		CallableStatement PrepareCall(String sql, int resultSetType, int resultSetConcurrency, int resultSetHoldability);


		/// <summary>
		/// Creates a default <code>PreparedStatement</code> object that has
		/// the capability to retrieve auto-generated keys. The given constant
		/// tells the driver whether it should make auto-generated keys
		/// available for retrieval.  This parameter is ignored if the SQL statement
		/// is not an <code>INSERT</code> statement, or an SQL statement able to return
		/// auto-generated keys (the list of such statements is vendor-specific).
		/// <P>
		/// <B>Note:</B> This method is optimized for handling
		/// parametric SQL statements that benefit from precompilation. If
		/// the driver supports precompilation,
		/// the method <code>prepareStatement</code> will send
		/// the statement to the database for precompilation. Some drivers
		/// may not support precompilation. In this case, the statement may
		/// not be sent to the database until the <code>PreparedStatement</code>
		/// object is executed.  This has no direct effect on users; however, it does
		/// affect which methods throw certain SQLExceptions.
		/// <P>
		/// Result sets created using the returned <code>PreparedStatement</code>
		/// object will by default be type <code>TYPE_FORWARD_ONLY</code>
		/// and have a concurrency level of <code>CONCUR_READ_ONLY</code>.
		/// The holdability of the created result sets can be determined by
		/// calling <seealso cref="#getHoldability"/>.
		/// </summary>
		/// <param name="sql"> an SQL statement that may contain one or more '?' IN
		///        parameter placeholders </param>
		/// <param name="autoGeneratedKeys"> a flag indicating whether auto-generated keys
		///        should be returned; one of
		///        <code>Statement.RETURN_GENERATED_KEYS</code> or
		///        <code>Statement.NO_GENERATED_KEYS</code> </param>
		/// <returns> a new <code>PreparedStatement</code> object, containing the
		///         pre-compiled SQL statement, that will have the capability of
		///         returning auto-generated keys </returns>
		/// <exception cref="SQLException"> if a database access error occurs, this
		///  method is called on a closed connection
		///         or the given parameter is not a <code>Statement</code>
		///         constant indicating whether auto-generated keys should be
		///         returned </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method with a constant of Statement.RETURN_GENERATED_KEYS
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: PreparedStatement prepareStatement(String sql, int autoGeneratedKeys) throws SQLException;
		PreparedStatement PrepareStatement(String sql, int autoGeneratedKeys);

		/// <summary>
		/// Creates a default <code>PreparedStatement</code> object capable
		/// of returning the auto-generated keys designated by the given array.
		/// This array contains the indexes of the columns in the target
		/// table that contain the auto-generated keys that should be made
		/// available.  The driver will ignore the array if the SQL statement
		/// is not an <code>INSERT</code> statement, or an SQL statement able to return
		/// auto-generated keys (the list of such statements is vendor-specific).
		/// <para>
		/// An SQL statement with or without IN parameters can be
		/// pre-compiled and stored in a <code>PreparedStatement</code> object. This
		/// object can then be used to efficiently execute this statement
		/// multiple times.
		/// <P>
		/// <B>Note:</B> This method is optimized for handling
		/// parametric SQL statements that benefit from precompilation. If
		/// the driver supports precompilation,
		/// the method <code>prepareStatement</code> will send
		/// the statement to the database for precompilation. Some drivers
		/// may not support precompilation. In this case, the statement may
		/// not be sent to the database until the <code>PreparedStatement</code>
		/// object is executed.  This has no direct effect on users; however, it does
		/// affect which methods throw certain SQLExceptions.
		/// <P>
		/// Result sets created using the returned <code>PreparedStatement</code>
		/// object will by default be type <code>TYPE_FORWARD_ONLY</code>
		/// and have a concurrency level of <code>CONCUR_READ_ONLY</code>.
		/// The holdability of the created result sets can be determined by
		/// calling <seealso cref="#getHoldability"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="sql"> an SQL statement that may contain one or more '?' IN
		///        parameter placeholders </param>
		/// <param name="columnIndexes"> an array of column indexes indicating the columns
		///        that should be returned from the inserted row or rows </param>
		/// <returns> a new <code>PreparedStatement</code> object, containing the
		///         pre-compiled statement, that is capable of returning the
		///         auto-generated keys designated by the given array of column
		///         indexes </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// 
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: PreparedStatement prepareStatement(String sql, int columnIndexes[]) throws SQLException;
		PreparedStatement PrepareStatement(String sql, int[] columnIndexes);

		/// <summary>
		/// Creates a default <code>PreparedStatement</code> object capable
		/// of returning the auto-generated keys designated by the given array.
		/// This array contains the names of the columns in the target
		/// table that contain the auto-generated keys that should be returned.
		/// The driver will ignore the array if the SQL statement
		/// is not an <code>INSERT</code> statement, or an SQL statement able to return
		/// auto-generated keys (the list of such statements is vendor-specific).
		/// <P>
		/// An SQL statement with or without IN parameters can be
		/// pre-compiled and stored in a <code>PreparedStatement</code> object. This
		/// object can then be used to efficiently execute this statement
		/// multiple times.
		/// <P>
		/// <B>Note:</B> This method is optimized for handling
		/// parametric SQL statements that benefit from precompilation. If
		/// the driver supports precompilation,
		/// the method <code>prepareStatement</code> will send
		/// the statement to the database for precompilation. Some drivers
		/// may not support precompilation. In this case, the statement may
		/// not be sent to the database until the <code>PreparedStatement</code>
		/// object is executed.  This has no direct effect on users; however, it does
		/// affect which methods throw certain SQLExceptions.
		/// <P>
		/// Result sets created using the returned <code>PreparedStatement</code>
		/// object will by default be type <code>TYPE_FORWARD_ONLY</code>
		/// and have a concurrency level of <code>CONCUR_READ_ONLY</code>.
		/// The holdability of the created result sets can be determined by
		/// calling <seealso cref="#getHoldability"/>.
		/// </summary>
		/// <param name="sql"> an SQL statement that may contain one or more '?' IN
		///        parameter placeholders </param>
		/// <param name="columnNames"> an array of column names indicating the columns
		///        that should be returned from the inserted row or rows </param>
		/// <returns> a new <code>PreparedStatement</code> object, containing the
		///         pre-compiled statement, that is capable of returning the
		///         auto-generated keys designated by the given array of column
		///         names </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method
		/// 
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: PreparedStatement prepareStatement(String sql, String columnNames[]) throws SQLException;
		PreparedStatement PrepareStatement(String sql, String[] columnNames);

		/// <summary>
		/// Constructs an object that implements the <code>Clob</code> interface. The object
		/// returned initially contains no data.  The <code>setAsciiStream</code>,
		/// <code>setCharacterStream</code> and <code>setString</code> methods of
		/// the <code>Clob</code> interface may be used to add data to the <code>Clob</code>. </summary>
		/// <returns> An object that implements the <code>Clob</code> interface </returns>
		/// <exception cref="SQLException"> if an object that implements the
		/// <code>Clob</code> interface can not be constructed, this method is
		/// called on a closed connection or a database access error occurs. </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this data type
		/// 
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Clob createClob() throws SQLException;
		Clob CreateClob();

		/// <summary>
		/// Constructs an object that implements the <code>Blob</code> interface. The object
		/// returned initially contains no data.  The <code>setBinaryStream</code> and
		/// <code>setBytes</code> methods of the <code>Blob</code> interface may be used to add data to
		/// the <code>Blob</code>. </summary>
		/// <returns>  An object that implements the <code>Blob</code> interface </returns>
		/// <exception cref="SQLException"> if an object that implements the
		/// <code>Blob</code> interface can not be constructed, this method is
		/// called on a closed connection or a database access error occurs. </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this data type
		/// 
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Blob createBlob() throws SQLException;
		Blob CreateBlob();

		/// <summary>
		/// Constructs an object that implements the <code>NClob</code> interface. The object
		/// returned initially contains no data.  The <code>setAsciiStream</code>,
		/// <code>setCharacterStream</code> and <code>setString</code> methods of the <code>NClob</code> interface may
		/// be used to add data to the <code>NClob</code>. </summary>
		/// <returns> An object that implements the <code>NClob</code> interface </returns>
		/// <exception cref="SQLException"> if an object that implements the
		/// <code>NClob</code> interface can not be constructed, this method is
		/// called on a closed connection or a database access error occurs. </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this data type
		/// 
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: NClob createNClob() throws SQLException;
		NClob CreateNClob();

		/// <summary>
		/// Constructs an object that implements the <code>SQLXML</code> interface. The object
		/// returned initially contains no data. The <code>createXmlStreamWriter</code> object and
		/// <code>setString</code> method of the <code>SQLXML</code> interface may be used to add data to the <code>SQLXML</code>
		/// object. </summary>
		/// <returns> An object that implements the <code>SQLXML</code> interface </returns>
		/// <exception cref="SQLException"> if an object that implements the <code>SQLXML</code> interface can not
		/// be constructed, this method is
		/// called on a closed connection or a database access error occurs. </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this data type
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: SQLXML createSQLXML() throws SQLException;
		SQLXML CreateSQLXML();

			/// <summary>
			/// Returns true if the connection has not been closed and is still valid.
			/// The driver shall submit a query on the connection or use some other
			/// mechanism that positively verifies the connection is still valid when
			/// this method is called.
			/// <para>
			/// The query submitted by the driver to validate the connection shall be
			/// executed in the context of the current transaction.
			/// 
			/// </para>
			/// </summary>
			/// <param name="timeout"> -             The time in seconds to wait for the database operation
			///                                              used to validate the connection to complete.  If
			///                                              the timeout period expires before the operation
			///                                              completes, this method returns false.  A value of
			///                                              0 indicates a timeout is not applied to the
			///                                              database operation.
			/// <para>
			/// </para>
			/// </param>
			/// <returns> true if the connection is valid, false otherwise </returns>
			/// <exception cref="SQLException"> if the value supplied for <code>timeout</code>
			/// is less then 0
			/// @since 1.6
			/// </exception>
			/// <seealso cref= java.sql.DatabaseMetaData#getClientInfoProperties </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isValid(int timeout) throws SQLException;
			 bool IsValid(int timeout);

			/// <summary>
			/// Sets the value of the client info property specified by name to the
			/// value specified by value.
			/// <para>
			/// Applications may use the <code>DatabaseMetaData.getClientInfoProperties</code>
			/// method to determine the client info properties supported by the driver
			/// and the maximum length that may be specified for each property.
			/// </para>
			/// <para>
			/// The driver stores the value specified in a suitable location in the
			/// database.  For example in a special register, session parameter, or
			/// system table column.  For efficiency the driver may defer setting the
			/// value in the database until the next time a statement is executed or
			/// prepared.  Other than storing the client information in the appropriate
			/// place in the database, these methods shall not alter the behavior of
			/// the connection in anyway.  The values supplied to these methods are
			/// used for accounting, diagnostics and debugging purposes only.
			/// </para>
			/// <para>
			/// The driver shall generate a warning if the client info name specified
			/// is not recognized by the driver.
			/// </para>
			/// <para>
			/// If the value specified to this method is greater than the maximum
			/// length for the property the driver may either truncate the value and
			/// generate a warning or generate a <code>SQLClientInfoException</code>.  If the driver
			/// generates a <code>SQLClientInfoException</code>, the value specified was not set on the
			/// connection.
			/// </para>
			/// <para>
			/// The following are standard client info properties.  Drivers are not
			/// required to support these properties however if the driver supports a
			/// client info property that can be described by one of the standard
			/// properties, the standard property name should be used.
			/// 
			/// <ul>
			/// <li>ApplicationName  -       The name of the application currently utilizing
			///                                                      the connection</li>
			/// <li>ClientUser               -       The name of the user that the application using
			///                                                      the connection is performing work for.  This may
			///                                                      not be the same as the user name that was used
			///                                                      in establishing the connection.</li>
			/// <li>ClientHostname   -       The hostname of the computer the application
			///                                                      using the connection is running on.</li>
			/// </ul>
			/// </para>
			/// <para>
			/// </para>
			/// </summary>
			/// <param name="name">          The name of the client info property to set </param>
			/// <param name="value">         The value to set the client info property to.  If the
			///                                      value is null, the current value of the specified
			///                                      property is cleared.
			/// <para>
			/// </para>
			/// </param>
			/// <exception cref="SQLClientInfoException"> if the database server returns an error while
			///                      setting the client info value on the database server or this method
			/// is called on a closed connection
			/// <para>
			/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setClientInfo(String name, String value) throws SQLClientInfoException;
			 void SetClientInfo(String name, String value);

			/// <summary>
			/// Sets the value of the connection's client info properties.  The
			/// <code>Properties</code> object contains the names and values of the client info
			/// properties to be set.  The set of client info properties contained in
			/// the properties list replaces the current set of client info properties
			/// on the connection.  If a property that is currently set on the
			/// connection is not present in the properties list, that property is
			/// cleared.  Specifying an empty properties list will clear all of the
			/// properties on the connection.  See <code>setClientInfo (String, String)</code> for
			/// more information.
			/// <para>
			/// If an error occurs in setting any of the client info properties, a
			/// <code>SQLClientInfoException</code> is thrown. The <code>SQLClientInfoException</code>
			/// contains information indicating which client info properties were not set.
			/// The state of the client information is unknown because
			/// some databases do not allow multiple client info properties to be set
			/// atomically.  For those databases, one or more properties may have been
			/// set before the error occurred.
			/// </para>
			/// <para>
			/// 
			/// </para>
			/// </summary>
			/// <param name="properties">                the list of client info properties to set
			/// <para>
			/// </para>
			/// </param>
			/// <seealso cref= java.sql.Connection#setClientInfo(String, String) setClientInfo(String, String)
			/// @since 1.6
			/// <para>
			/// </para>
			/// </seealso>
			/// <exception cref="SQLClientInfoException"> if the database server returns an error while
			///                  setting the clientInfo values on the database server or this method
			/// is called on a closed connection
			///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setClientInfo(java.util.Properties properties) throws SQLClientInfoException;
			 Properties ClientInfo {set;get;}

			/// <summary>
			/// Returns the value of the client info property specified by name.  This
			/// method may return null if the specified client info property has not
			/// been set and does not have a default value.  This method will also
			/// return null if the specified client info property name is not supported
			/// by the driver.
			/// <para>
			/// Applications may use the <code>DatabaseMetaData.getClientInfoProperties</code>
			/// method to determine the client info properties supported by the driver.
			/// </para>
			/// <para>
			/// </para>
			/// </summary>
			/// <param name="name">          The name of the client info property to retrieve
			/// <para>
			/// </para>
			/// </param>
			/// <returns>                      The value of the client info property specified
			/// <para>
			/// </para>
			/// </returns>
			/// <exception cref="SQLException">         if the database server returns an error when
			///                                                      fetching the client info value from the database
			/// or this method is called on a closed connection
			/// <para>
			/// @since 1.6
			/// 
			/// </para>
			/// </exception>
			/// <seealso cref= java.sql.DatabaseMetaData#getClientInfoProperties </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getClientInfo(String name) throws SQLException;
			 String GetClientInfo(String name);

			/// <summary>
			/// Returns a list containing the name and current value of each client info
			/// property supported by the driver.  The value of a client info property
			/// may be null if the property has not been set and does not have a
			/// default value.
			/// <para>
			/// </para>
			/// </summary>
			/// <returns>      A <code>Properties</code> object that contains the name and current value of
			///                      each of the client info properties supported by the driver.
			/// <para>
			/// </para>
			/// </returns>
			/// <exception cref="SQLException"> if the database server returns an error when
			///                      fetching the client info values from the database
			/// or this method is called on a closed connection
			/// <para>
			/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.util.Properties getClientInfo() throws SQLException;

	/// <summary>
	/// Factory method for creating Array objects.
	/// <para>
	/// <b>Note: </b>When <code>createArrayOf</code> is used to create an array object
	/// that maps to a primitive data type, then it is implementation-defined
	/// whether the <code>Array</code> object is an array of that primitive
	/// data type or an array of <code>Object</code>.
	/// </para>
	/// <para>
	/// <b>Note: </b>The JDBC driver is responsible for mapping the elements
	/// <code>Object</code> array to the default JDBC SQL type defined in
	/// java.sql.Types for the given class of <code>Object</code>. The default
	/// mapping is specified in Appendix B of the JDBC specification.  If the
	/// resulting JDBC type is not the appropriate type for the given typeName then
	/// it is implementation defined whether an <code>SQLException</code> is
	/// thrown or the driver supports the resulting conversion.
	///  
	/// </para>
	/// </summary>
	/// <param name="typeName"> the SQL name of the type the elements of the array map to. The typeName is a
	/// database-specific name which may be the name of a built-in type, a user-defined type or a standard  SQL type supported by this database. This
	///  is the value returned by <code>Array.getBaseTypeName</code> </param>
	/// <param name="elements"> the elements that populate the returned object </param>
	/// <returns> an Array object whose elements map to the specified SQL type </returns>
	/// <exception cref="SQLException"> if a database error occurs, the JDBC type is not
	///  appropriate for the typeName and the conversion is not supported, the typeName is null or this method is called on a closed connection </exception>
	/// <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this data type
	/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Array createArrayOf(String typeName, Object[] elements) throws SQLException;
	 Array CreateArrayOf(String typeName, Object[] elements);

	/// <summary>
	/// Factory method for creating Struct objects.
	/// </summary>
	/// <param name="typeName"> the SQL type name of the SQL structured type that this <code>Struct</code>
	/// object maps to. The typeName is the name of  a user-defined type that
	/// has been defined for this database. It is the value returned by
	/// <code>Struct.getSQLTypeName</code>.
	/// </param>
	/// <param name="attributes"> the attributes that populate the returned object </param>
	///  <returns> a Struct object that maps to the given SQL type and is populated with the given attributes </returns>
	/// <exception cref="SQLException"> if a database error occurs, the typeName is null or this method is called on a closed connection </exception>
	/// <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this data type
	/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Struct createStruct(String typeName, Object[] attributes) throws SQLException;
	 Struct CreateStruct(String typeName, Object[] attributes);

	   //--------------------------JDBC 4.1 -----------------------------

	   /// <summary>
	   /// Sets the given schema name to access.
	   /// <P>
	   /// If the driver does not support schemas, it will
	   /// silently ignore this request.
	   /// <para>
	   /// Calling {@code setSchema} has no effect on previously created or prepared
	   /// {@code Statement} objects. It is implementation defined whether a DBMS
	   /// prepare operation takes place immediately when the {@code Connection}
	   /// method {@code prepareStatement} or {@code prepareCall} is invoked.
	   /// For maximum portability, {@code setSchema} should be called before a
	   /// {@code Statement} is created or prepared.
	   /// 
	   /// </para>
	   /// </summary>
	   /// <param name="schema"> the name of a schema  in which to work </param>
	   /// <exception cref="SQLException"> if a database access error occurs
	   /// or this method is called on a closed connection </exception>
	   /// <seealso cref= #getSchema
	   /// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setSchema(String schema) throws SQLException;
		String Schema {set;get;}

		/// <summary>
		/// Retrieves this <code>Connection</code> object's current schema name.
		/// </summary>
		/// <returns> the current schema name or <code>null</code> if there is none </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// or this method is called on a closed connection </exception>
		/// <seealso cref= #setSchema
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getSchema() throws SQLException;

		/// <summary>
		/// Terminates an open connection.  Calling <code>abort</code> results in:
		/// <ul>
		/// <li>The connection marked as closed
		/// <li>Closes any physical connection to the database
		/// <li>Releases resources used by the connection
		/// <li>Insures that any thread that is currently accessing the connection
		/// will either progress to completion or throw an <code>SQLException</code>.
		/// </ul>
		/// <para>
		/// Calling <code>abort</code> marks the connection closed and releases any
		/// resources. Calling <code>abort</code> on a closed connection is a
		/// no-op.
		/// </para>
		/// <para>
		/// It is possible that the aborting and releasing of the resources that are
		/// held by the connection can take an extended period of time.  When the
		/// <code>abort</code> method returns, the connection will have been marked as
		/// closed and the <code>Executor</code> that was passed as a parameter to abort
		/// may still be executing tasks to release resources.
		/// </para>
		/// <para>
		/// This method checks to see that there is an <code>SQLPermission</code>
		/// object before allowing the method to proceed.  If a
		/// <code>SecurityManager</code> exists and its
		/// <code>checkPermission</code> method denies calling <code>abort</code>,
		/// this method throws a
		/// <code>java.lang.SecurityException</code>.
		/// </para>
		/// </summary>
		/// <param name="executor">  The <code>Executor</code>  implementation which will
		/// be used by <code>abort</code>. </param>
		/// <exception cref="java.sql.SQLException"> if a database access error occurs or
		/// the {@code executor} is {@code null}, </exception>
		/// <exception cref="java.lang.SecurityException"> if a security manager exists and its
		///    <code>checkPermission</code> method denies calling <code>abort</code> </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= Executor
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void abort(java.util.concurrent.Executor executor) throws SQLException;
		void Abort(Executor executor);

		/// 
		/// <summary>
		/// Sets the maximum period a <code>Connection</code> or
		/// objects created from the <code>Connection</code>
		/// will wait for the database to reply to any one request. If any
		///  request remains unanswered, the waiting method will
		/// return with a <code>SQLException</code>, and the <code>Connection</code>
		/// or objects created from the <code>Connection</code>  will be marked as
		/// closed. Any subsequent use of
		/// the objects, with the exception of the <code>close</code>,
		/// <code>isClosed</code> or <code>Connection.isValid</code>
		/// methods, will result in  a <code>SQLException</code>.
		/// <para>
		/// <b>Note</b>: This method is intended to address a rare but serious
		/// condition where network partitions can cause threads issuing JDBC calls
		/// to hang uninterruptedly in socket reads, until the OS TCP-TIMEOUT
		/// (typically 10 minutes). This method is related to the
		/// <seealso cref="#abort abort() "/> method which provides an administrator
		/// thread a means to free any such threads in cases where the
		/// JDBC connection is accessible to the administrator thread.
		/// The <code>setNetworkTimeout</code> method will cover cases where
		/// there is no administrator thread, or it has no access to the
		/// connection. This method is severe in it's effects, and should be
		/// given a high enough value so it is never triggered before any more
		/// normal timeouts, such as transaction timeouts.
		/// </para>
		/// <para>
		/// JDBC driver implementations  may also choose to support the
		/// {@code setNetworkTimeout} method to impose a limit on database
		/// response time, in environments where no network is present.
		/// </para>
		/// <para>
		/// Drivers may internally implement some or all of their API calls with
		/// multiple internal driver-database transmissions, and it is left to the
		/// driver implementation to determine whether the limit will be
		/// applied always to the response to the API call, or to any
		/// single  request made during the API call.
		/// </para>
		/// <para>
		/// 
		/// This method can be invoked more than once, such as to set a limit for an
		/// area of JDBC code, and to reset to the default on exit from this area.
		/// Invocation of this method has no impact on already outstanding
		/// requests.
		/// </para>
		/// <para>
		/// The {@code Statement.setQueryTimeout()} timeout value is independent of the
		/// timeout value specified in {@code setNetworkTimeout}. If the query timeout
		/// expires  before the network timeout then the
		/// statement execution will be canceled. If the network is still
		/// active the result will be that both the statement and connection
		/// are still usable. However if the network timeout expires before
		/// the query timeout or if the statement timeout fails due to network
		/// problems, the connection will be marked as closed, any resources held by
		/// the connection will be released and both the connection and
		/// statement will be unusable.
		/// </para>
		/// <para>
		/// When the driver determines that the {@code setNetworkTimeout} timeout
		/// value has expired, the JDBC driver marks the connection
		/// closed and releases any resources held by the connection.
		/// </para>
		/// <para>
		/// 
		/// This method checks to see that there is an <code>SQLPermission</code>
		/// object before allowing the method to proceed.  If a
		/// <code>SecurityManager</code> exists and its
		/// <code>checkPermission</code> method denies calling
		/// <code>setNetworkTimeout</code>, this method throws a
		/// <code>java.lang.SecurityException</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="executor">  The <code>Executor</code>  implementation which will
		/// be used by <code>setNetworkTimeout</code>. </param>
		/// <param name="milliseconds"> The time in milliseconds to wait for the database
		/// operation
		///  to complete.  If the JDBC driver does not support milliseconds, the
		/// JDBC driver will round the value up to the nearest second.  If the
		/// timeout period expires before the operation
		/// completes, a SQLException will be thrown.
		/// A value of 0 indicates that there is not timeout for database operations. </param>
		/// <exception cref="java.sql.SQLException"> if a database access error occurs, this
		/// method is called on a closed connection,
		/// the {@code executor} is {@code null},
		/// or the value specified for <code>seconds</code> is less than 0. </exception>
		/// <exception cref="java.lang.SecurityException"> if a security manager exists and its
		///    <code>checkPermission</code> method denies calling
		/// <code>setNetworkTimeout</code>. </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= Statement#setQueryTimeout </seealso>
		/// <seealso cref= #getNetworkTimeout </seealso>
		/// <seealso cref= #abort </seealso>
		/// <seealso cref= Executor
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setNetworkTimeout(java.util.concurrent.Executor executor, int milliseconds) throws SQLException;
		void SetNetworkTimeout(Executor executor, int milliseconds);


		/// <summary>
		/// Retrieves the number of milliseconds the driver will
		/// wait for a database request to complete.
		/// If the limit is exceeded, a
		/// <code>SQLException</code> is thrown.
		/// </summary>
		/// <returns> the current timeout limit in milliseconds; zero means there is
		///         no limit </returns>
		/// <exception cref="SQLException"> if a database access error occurs or
		/// this method is called on a closed <code>Connection</code> </exception>
		/// <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		/// this method </exception>
		/// <seealso cref= #setNetworkTimeout
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getNetworkTimeout() throws SQLException;
		int NetworkTimeout {get;}
	}

	public static class Connection_Fields
	{
		public const int TRANSACTION_NONE = 0;
		public const int TRANSACTION_READ_UNCOMMITTED = 1;
		public const int TRANSACTION_READ_COMMITTED = 2;
		public const int TRANSACTION_REPEATABLE_READ = 4;
		public const int TRANSACTION_SERIALIZABLE = 8;
	}

}