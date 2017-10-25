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
	/// Comprehensive information about the database as a whole.
	/// <P>
	/// This interface is implemented by driver vendors to let users know the capabilities
	/// of a Database Management System (DBMS) in combination with
	/// the driver based on JDBC&trade; technology
	/// ("JDBC driver") that is used with it.  Different relational DBMSs often support
	/// different features, implement features in different ways, and use different
	/// data types.  In addition, a driver may implement a feature on top of what the
	/// DBMS offers.  Information returned by methods in this interface applies
	/// to the capabilities of a particular driver and a particular DBMS working
	/// together. Note that as used in this documentation, the term "database" is
	/// used generically to refer to both the driver and DBMS.
	/// <P>
	/// A user for this interface is commonly a tool that needs to discover how to
	/// deal with the underlying DBMS.  This is especially true for applications
	/// that are intended to be used with more than one DBMS. For example, a tool might use the method
	/// <code>getTypeInfo</code> to find out what data types can be used in a
	/// <code>CREATE TABLE</code> statement.  Or a user might call the method
	/// <code>supportsCorrelatedSubqueries</code> to see if it is possible to use
	/// a correlated subquery or <code>supportsBatchUpdates</code> to see if it is
	/// possible to use batch updates.
	/// <P>
	/// Some <code>DatabaseMetaData</code> methods return lists of information
	/// in the form of <code>ResultSet</code> objects.
	/// Regular <code>ResultSet</code> methods, such as
	/// <code>getString</code> and <code>getInt</code>, can be used
	/// to retrieve the data from these <code>ResultSet</code> objects.  If
	/// a given form of metadata is not available, an empty <code>ResultSet</code>
	/// will be returned. Additional columns beyond the columns defined to be
	/// returned by the <code>ResultSet</code> object for a given method
	/// can be defined by the JDBC driver vendor and must be accessed
	/// by their <B>column label</B>.
	/// <P>
	/// Some <code>DatabaseMetaData</code> methods take arguments that are
	/// String patterns.  These arguments all have names such as fooPattern.
	/// Within a pattern String, "%" means match any substring of 0 or more
	/// characters, and "_" means match any one character. Only metadata
	/// entries matching the search pattern are returned. If a search pattern
	/// argument is set to <code>null</code>, that argument's criterion will
	/// be dropped from the search.
	/// 
	/// </summary>
	public interface DatabaseMetaData : Wrapper
	{

		//----------------------------------------------------------------------
		// First, a variety of minor information about the target database.

		/// <summary>
		/// Retrieves whether the current user can call all the procedures
		/// returned by the method <code>getProcedures</code>.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean allProceduresAreCallable() throws SQLException;
		bool AllProceduresAreCallable();

		/// <summary>
		/// Retrieves whether the current user can use all the tables returned
		/// by the method <code>getTables</code> in a <code>SELECT</code>
		/// statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean allTablesAreSelectable() throws SQLException;
		bool AllTablesAreSelectable();

		/// <summary>
		/// Retrieves the URL for this DBMS.
		/// </summary>
		/// <returns> the URL for this DBMS or <code>null</code> if it cannot be
		///          generated </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getURL() throws SQLException;
		String URL {get;}

		/// <summary>
		/// Retrieves the user name as known to this database.
		/// </summary>
		/// <returns> the database user name </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getUserName() throws SQLException;
		String UserName {get;}

		/// <summary>
		/// Retrieves whether this database is in read-only mode.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isReadOnly() throws SQLException;
		bool ReadOnly {get;}

		/// <summary>
		/// Retrieves whether <code>NULL</code> values are sorted high.
		/// Sorted high means that <code>NULL</code> values
		/// sort higher than any other value in a domain.  In an ascending order,
		/// if this method returns <code>true</code>,  <code>NULL</code> values
		/// will appear at the end. By contrast, the method
		/// <code>nullsAreSortedAtEnd</code> indicates whether <code>NULL</code> values
		/// are sorted at the end regardless of sort order.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean nullsAreSortedHigh() throws SQLException;
		bool NullsAreSortedHigh();

		/// <summary>
		/// Retrieves whether <code>NULL</code> values are sorted low.
		/// Sorted low means that <code>NULL</code> values
		/// sort lower than any other value in a domain.  In an ascending order,
		/// if this method returns <code>true</code>,  <code>NULL</code> values
		/// will appear at the beginning. By contrast, the method
		/// <code>nullsAreSortedAtStart</code> indicates whether <code>NULL</code> values
		/// are sorted at the beginning regardless of sort order.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean nullsAreSortedLow() throws SQLException;
		bool NullsAreSortedLow();

		/// <summary>
		/// Retrieves whether <code>NULL</code> values are sorted at the start regardless
		/// of sort order.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean nullsAreSortedAtStart() throws SQLException;
		bool NullsAreSortedAtStart();

		/// <summary>
		/// Retrieves whether <code>NULL</code> values are sorted at the end regardless of
		/// sort order.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean nullsAreSortedAtEnd() throws SQLException;
		bool NullsAreSortedAtEnd();

		/// <summary>
		/// Retrieves the name of this database product.
		/// </summary>
		/// <returns> database product name </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getDatabaseProductName() throws SQLException;
		String DatabaseProductName {get;}

		/// <summary>
		/// Retrieves the version number of this database product.
		/// </summary>
		/// <returns> database version number </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getDatabaseProductVersion() throws SQLException;
		String DatabaseProductVersion {get;}

		/// <summary>
		/// Retrieves the name of this JDBC driver.
		/// </summary>
		/// <returns> JDBC driver name </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getDriverName() throws SQLException;
		String DriverName {get;}

		/// <summary>
		/// Retrieves the version number of this JDBC driver as a <code>String</code>.
		/// </summary>
		/// <returns> JDBC driver version </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getDriverVersion() throws SQLException;
		String DriverVersion {get;}

		/// <summary>
		/// Retrieves this JDBC driver's major version number.
		/// </summary>
		/// <returns> JDBC driver major version </returns>
		int DriverMajorVersion {get;}

		/// <summary>
		/// Retrieves this JDBC driver's minor version number.
		/// </summary>
		/// <returns> JDBC driver minor version number </returns>
		int DriverMinorVersion {get;}

		/// <summary>
		/// Retrieves whether this database stores tables in a local file.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean usesLocalFiles() throws SQLException;
		bool UsesLocalFiles();

		/// <summary>
		/// Retrieves whether this database uses a file for each table.
		/// </summary>
		/// <returns> <code>true</code> if this database uses a local file for each table;
		///         <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean usesLocalFilePerTable() throws SQLException;
		bool UsesLocalFilePerTable();

		/// <summary>
		/// Retrieves whether this database treats mixed case unquoted SQL identifiers as
		/// case sensitive and as a result stores them in mixed case.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsMixedCaseIdentifiers() throws SQLException;
		bool SupportsMixedCaseIdentifiers();

		/// <summary>
		/// Retrieves whether this database treats mixed case unquoted SQL identifiers as
		/// case insensitive and stores them in upper case.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean storesUpperCaseIdentifiers() throws SQLException;
		bool StoresUpperCaseIdentifiers();

		/// <summary>
		/// Retrieves whether this database treats mixed case unquoted SQL identifiers as
		/// case insensitive and stores them in lower case.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean storesLowerCaseIdentifiers() throws SQLException;
		bool StoresLowerCaseIdentifiers();

		/// <summary>
		/// Retrieves whether this database treats mixed case unquoted SQL identifiers as
		/// case insensitive and stores them in mixed case.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean storesMixedCaseIdentifiers() throws SQLException;
		bool StoresMixedCaseIdentifiers();

		/// <summary>
		/// Retrieves whether this database treats mixed case quoted SQL identifiers as
		/// case sensitive and as a result stores them in mixed case.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsMixedCaseQuotedIdentifiers() throws SQLException;
		bool SupportsMixedCaseQuotedIdentifiers();

		/// <summary>
		/// Retrieves whether this database treats mixed case quoted SQL identifiers as
		/// case insensitive and stores them in upper case.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean storesUpperCaseQuotedIdentifiers() throws SQLException;
		bool StoresUpperCaseQuotedIdentifiers();

		/// <summary>
		/// Retrieves whether this database treats mixed case quoted SQL identifiers as
		/// case insensitive and stores them in lower case.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean storesLowerCaseQuotedIdentifiers() throws SQLException;
		bool StoresLowerCaseQuotedIdentifiers();

		/// <summary>
		/// Retrieves whether this database treats mixed case quoted SQL identifiers as
		/// case insensitive and stores them in mixed case.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean storesMixedCaseQuotedIdentifiers() throws SQLException;
		bool StoresMixedCaseQuotedIdentifiers();

		/// <summary>
		/// Retrieves the string used to quote SQL identifiers.
		/// This method returns a space " " if identifier quoting is not supported.
		/// </summary>
		/// <returns> the quoting string or a space if quoting is not supported </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getIdentifierQuoteString() throws SQLException;
		String IdentifierQuoteString {get;}

		/// <summary>
		/// Retrieves a comma-separated list of all of this database's SQL keywords
		/// that are NOT also SQL:2003 keywords.
		/// </summary>
		/// <returns> the list of this database's keywords that are not also
		///         SQL:2003 keywords </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getSQLKeywords() throws SQLException;
		String SQLKeywords {get;}

		/// <summary>
		/// Retrieves a comma-separated list of math functions available with
		/// this database.  These are the Open /Open CLI math function names used in
		/// the JDBC function escape clause.
		/// </summary>
		/// <returns> the list of math functions supported by this database </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getNumericFunctions() throws SQLException;
		String NumericFunctions {get;}

		/// <summary>
		/// Retrieves a comma-separated list of string functions available with
		/// this database.  These are the  Open Group CLI string function names used
		/// in the JDBC function escape clause.
		/// </summary>
		/// <returns> the list of string functions supported by this database </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getStringFunctions() throws SQLException;
		String StringFunctions {get;}

		/// <summary>
		/// Retrieves a comma-separated list of system functions available with
		/// this database.  These are the  Open Group CLI system function names used
		/// in the JDBC function escape clause.
		/// </summary>
		/// <returns> a list of system functions supported by this database </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getSystemFunctions() throws SQLException;
		String SystemFunctions {get;}

		/// <summary>
		/// Retrieves a comma-separated list of the time and date functions available
		/// with this database.
		/// </summary>
		/// <returns> the list of time and date functions supported by this database </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getTimeDateFunctions() throws SQLException;
		String TimeDateFunctions {get;}

		/// <summary>
		/// Retrieves the string that can be used to escape wildcard characters.
		/// This is the string that can be used to escape '_' or '%' in
		/// the catalog search parameters that are a pattern (and therefore use one
		/// of the wildcard characters).
		/// 
		/// <P>The '_' character represents any single character;
		/// the '%' character represents any sequence of zero or
		/// more characters.
		/// </summary>
		/// <returns> the string used to escape wildcard characters </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getSearchStringEscape() throws SQLException;
		String SearchStringEscape {get;}

		/// <summary>
		/// Retrieves all the "extra" characters that can be used in unquoted
		/// identifier names (those beyond a-z, A-Z, 0-9 and _).
		/// </summary>
		/// <returns> the string containing the extra characters </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getExtraNameCharacters() throws SQLException;
		String ExtraNameCharacters {get;}

		//--------------------------------------------------------------------
		// Functions describing which features are supported.

		/// <summary>
		/// Retrieves whether this database supports <code>ALTER TABLE</code>
		/// with add column.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsAlterTableWithAddColumn() throws SQLException;
		bool SupportsAlterTableWithAddColumn();

		/// <summary>
		/// Retrieves whether this database supports <code>ALTER TABLE</code>
		/// with drop column.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsAlterTableWithDropColumn() throws SQLException;
		bool SupportsAlterTableWithDropColumn();

		/// <summary>
		/// Retrieves whether this database supports column aliasing.
		/// 
		/// <P>If so, the SQL AS clause can be used to provide names for
		/// computed columns or to provide alias names for columns as
		/// required.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsColumnAliasing() throws SQLException;
		bool SupportsColumnAliasing();

		/// <summary>
		/// Retrieves whether this database supports concatenations between
		/// <code>NULL</code> and non-<code>NULL</code> values being
		/// <code>NULL</code>.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean nullPlusNonNullIsNull() throws SQLException;
		bool NullPlusNonNullIsNull();

		/// <summary>
		/// Retrieves whether this database supports the JDBC scalar function
		/// <code>CONVERT</code> for the conversion of one JDBC type to another.
		/// The JDBC types are the generic SQL data types defined
		/// in <code>java.sql.Types</code>.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsConvert() throws SQLException;
		bool SupportsConvert();

		/// <summary>
		/// Retrieves whether this database supports the JDBC scalar function
		/// <code>CONVERT</code> for conversions between the JDBC types <i>fromType</i>
		/// and <i>toType</i>.  The JDBC types are the generic SQL data types defined
		/// in <code>java.sql.Types</code>.
		/// </summary>
		/// <param name="fromType"> the type to convert from; one of the type codes from
		///        the class <code>java.sql.Types</code> </param>
		/// <param name="toType"> the type to convert to; one of the type codes from
		///        the class <code>java.sql.Types</code> </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= Types </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsConvert(int fromType, int toType) throws SQLException;
		bool SupportsConvert(int fromType, int toType);

		/// <summary>
		/// Retrieves whether this database supports table correlation names.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsTableCorrelationNames() throws SQLException;
		bool SupportsTableCorrelationNames();

		/// <summary>
		/// Retrieves whether, when table correlation names are supported, they
		/// are restricted to being different from the names of the tables.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsDifferentTableCorrelationNames() throws SQLException;
		bool SupportsDifferentTableCorrelationNames();

		/// <summary>
		/// Retrieves whether this database supports expressions in
		/// <code>ORDER BY</code> lists.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsExpressionsInOrderBy() throws SQLException;
		bool SupportsExpressionsInOrderBy();

		/// <summary>
		/// Retrieves whether this database supports using a column that is
		/// not in the <code>SELECT</code> statement in an
		/// <code>ORDER BY</code> clause.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsOrderByUnrelated() throws SQLException;
		bool SupportsOrderByUnrelated();

		/// <summary>
		/// Retrieves whether this database supports some form of
		/// <code>GROUP BY</code> clause.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsGroupBy() throws SQLException;
		bool SupportsGroupBy();

		/// <summary>
		/// Retrieves whether this database supports using a column that is
		/// not in the <code>SELECT</code> statement in a
		/// <code>GROUP BY</code> clause.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsGroupByUnrelated() throws SQLException;
		bool SupportsGroupByUnrelated();

		/// <summary>
		/// Retrieves whether this database supports using columns not included in
		/// the <code>SELECT</code> statement in a <code>GROUP BY</code> clause
		/// provided that all of the columns in the <code>SELECT</code> statement
		/// are included in the <code>GROUP BY</code> clause.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsGroupByBeyondSelect() throws SQLException;
		bool SupportsGroupByBeyondSelect();

		/// <summary>
		/// Retrieves whether this database supports specifying a
		/// <code>LIKE</code> escape clause.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsLikeEscapeClause() throws SQLException;
		bool SupportsLikeEscapeClause();

		/// <summary>
		/// Retrieves whether this database supports getting multiple
		/// <code>ResultSet</code> objects from a single call to the
		/// method <code>execute</code>.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsMultipleResultSets() throws SQLException;
		bool SupportsMultipleResultSets();

		/// <summary>
		/// Retrieves whether this database allows having multiple
		/// transactions open at once (on different connections).
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsMultipleTransactions() throws SQLException;
		bool SupportsMultipleTransactions();

		/// <summary>
		/// Retrieves whether columns in this database may be defined as non-nullable.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsNonNullableColumns() throws SQLException;
		bool SupportsNonNullableColumns();

		/// <summary>
		/// Retrieves whether this database supports the ODBC Minimum SQL grammar.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsMinimumSQLGrammar() throws SQLException;
		bool SupportsMinimumSQLGrammar();

		/// <summary>
		/// Retrieves whether this database supports the ODBC Core SQL grammar.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsCoreSQLGrammar() throws SQLException;
		bool SupportsCoreSQLGrammar();

		/// <summary>
		/// Retrieves whether this database supports the ODBC Extended SQL grammar.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsExtendedSQLGrammar() throws SQLException;
		bool SupportsExtendedSQLGrammar();

		/// <summary>
		/// Retrieves whether this database supports the ANSI92 entry level SQL
		/// grammar.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsANSI92EntryLevelSQL() throws SQLException;
		bool SupportsANSI92EntryLevelSQL();

		/// <summary>
		/// Retrieves whether this database supports the ANSI92 intermediate SQL grammar supported.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsANSI92IntermediateSQL() throws SQLException;
		bool SupportsANSI92IntermediateSQL();

		/// <summary>
		/// Retrieves whether this database supports the ANSI92 full SQL grammar supported.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsANSI92FullSQL() throws SQLException;
		bool SupportsANSI92FullSQL();

		/// <summary>
		/// Retrieves whether this database supports the SQL Integrity
		/// Enhancement Facility.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsIntegrityEnhancementFacility() throws SQLException;
		bool SupportsIntegrityEnhancementFacility();

		/// <summary>
		/// Retrieves whether this database supports some form of outer join.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsOuterJoins() throws SQLException;
		bool SupportsOuterJoins();

		/// <summary>
		/// Retrieves whether this database supports full nested outer joins.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsFullOuterJoins() throws SQLException;
		bool SupportsFullOuterJoins();

		/// <summary>
		/// Retrieves whether this database provides limited support for outer
		/// joins.  (This will be <code>true</code> if the method
		/// <code>supportsFullOuterJoins</code> returns <code>true</code>).
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsLimitedOuterJoins() throws SQLException;
		bool SupportsLimitedOuterJoins();

		/// <summary>
		/// Retrieves the database vendor's preferred term for "schema".
		/// </summary>
		/// <returns> the vendor term for "schema" </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getSchemaTerm() throws SQLException;
		String SchemaTerm {get;}

		/// <summary>
		/// Retrieves the database vendor's preferred term for "procedure".
		/// </summary>
		/// <returns> the vendor term for "procedure" </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getProcedureTerm() throws SQLException;
		String ProcedureTerm {get;}

		/// <summary>
		/// Retrieves the database vendor's preferred term for "catalog".
		/// </summary>
		/// <returns> the vendor term for "catalog" </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getCatalogTerm() throws SQLException;
		String CatalogTerm {get;}

		/// <summary>
		/// Retrieves whether a catalog appears at the start of a fully qualified
		/// table name.  If not, the catalog appears at the end.
		/// </summary>
		/// <returns> <code>true</code> if the catalog name appears at the beginning
		///         of a fully qualified table name; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean isCatalogAtStart() throws SQLException;
		bool CatalogAtStart {get;}

		/// <summary>
		/// Retrieves the <code>String</code> that this database uses as the
		/// separator between a catalog and table name.
		/// </summary>
		/// <returns> the separator string </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getCatalogSeparator() throws SQLException;
		String CatalogSeparator {get;}

		/// <summary>
		/// Retrieves whether a schema name can be used in a data manipulation statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSchemasInDataManipulation() throws SQLException;
		bool SupportsSchemasInDataManipulation();

		/// <summary>
		/// Retrieves whether a schema name can be used in a procedure call statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSchemasInProcedureCalls() throws SQLException;
		bool SupportsSchemasInProcedureCalls();

		/// <summary>
		/// Retrieves whether a schema name can be used in a table definition statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSchemasInTableDefinitions() throws SQLException;
		bool SupportsSchemasInTableDefinitions();

		/// <summary>
		/// Retrieves whether a schema name can be used in an index definition statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSchemasInIndexDefinitions() throws SQLException;
		bool SupportsSchemasInIndexDefinitions();

		/// <summary>
		/// Retrieves whether a schema name can be used in a privilege definition statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSchemasInPrivilegeDefinitions() throws SQLException;
		bool SupportsSchemasInPrivilegeDefinitions();

		/// <summary>
		/// Retrieves whether a catalog name can be used in a data manipulation statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsCatalogsInDataManipulation() throws SQLException;
		bool SupportsCatalogsInDataManipulation();

		/// <summary>
		/// Retrieves whether a catalog name can be used in a procedure call statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsCatalogsInProcedureCalls() throws SQLException;
		bool SupportsCatalogsInProcedureCalls();

		/// <summary>
		/// Retrieves whether a catalog name can be used in a table definition statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsCatalogsInTableDefinitions() throws SQLException;
		bool SupportsCatalogsInTableDefinitions();

		/// <summary>
		/// Retrieves whether a catalog name can be used in an index definition statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsCatalogsInIndexDefinitions() throws SQLException;
		bool SupportsCatalogsInIndexDefinitions();

		/// <summary>
		/// Retrieves whether a catalog name can be used in a privilege definition statement.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsCatalogsInPrivilegeDefinitions() throws SQLException;
		bool SupportsCatalogsInPrivilegeDefinitions();


		/// <summary>
		/// Retrieves whether this database supports positioned <code>DELETE</code>
		/// statements.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsPositionedDelete() throws SQLException;
		bool SupportsPositionedDelete();

		/// <summary>
		/// Retrieves whether this database supports positioned <code>UPDATE</code>
		/// statements.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsPositionedUpdate() throws SQLException;
		bool SupportsPositionedUpdate();

		/// <summary>
		/// Retrieves whether this database supports <code>SELECT FOR UPDATE</code>
		/// statements.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSelectForUpdate() throws SQLException;
		bool SupportsSelectForUpdate();

		/// <summary>
		/// Retrieves whether this database supports stored procedure calls
		/// that use the stored procedure escape syntax.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsStoredProcedures() throws SQLException;
		bool SupportsStoredProcedures();

		/// <summary>
		/// Retrieves whether this database supports subqueries in comparison
		/// expressions.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSubqueriesInComparisons() throws SQLException;
		bool SupportsSubqueriesInComparisons();

		/// <summary>
		/// Retrieves whether this database supports subqueries in
		/// <code>EXISTS</code> expressions.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSubqueriesInExists() throws SQLException;
		bool SupportsSubqueriesInExists();

		/// <summary>
		/// Retrieves whether this database supports subqueries in
		/// <code>IN</code> expressions.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSubqueriesInIns() throws SQLException;
		bool SupportsSubqueriesInIns();

		/// <summary>
		/// Retrieves whether this database supports subqueries in quantified
		/// expressions.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSubqueriesInQuantifieds() throws SQLException;
		bool SupportsSubqueriesInQuantifieds();

		/// <summary>
		/// Retrieves whether this database supports correlated subqueries.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsCorrelatedSubqueries() throws SQLException;
		bool SupportsCorrelatedSubqueries();

		/// <summary>
		/// Retrieves whether this database supports SQL <code>UNION</code>.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsUnion() throws SQLException;
		bool SupportsUnion();

		/// <summary>
		/// Retrieves whether this database supports SQL <code>UNION ALL</code>.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsUnionAll() throws SQLException;
		bool SupportsUnionAll();

		/// <summary>
		/// Retrieves whether this database supports keeping cursors open
		/// across commits.
		/// </summary>
		/// <returns> <code>true</code> if cursors always remain open;
		///       <code>false</code> if they might not remain open </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsOpenCursorsAcrossCommit() throws SQLException;
		bool SupportsOpenCursorsAcrossCommit();

		/// <summary>
		/// Retrieves whether this database supports keeping cursors open
		/// across rollbacks.
		/// </summary>
		/// <returns> <code>true</code> if cursors always remain open;
		///       <code>false</code> if they might not remain open </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsOpenCursorsAcrossRollback() throws SQLException;
		bool SupportsOpenCursorsAcrossRollback();

		/// <summary>
		/// Retrieves whether this database supports keeping statements open
		/// across commits.
		/// </summary>
		/// <returns> <code>true</code> if statements always remain open;
		///       <code>false</code> if they might not remain open </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsOpenStatementsAcrossCommit() throws SQLException;
		bool SupportsOpenStatementsAcrossCommit();

		/// <summary>
		/// Retrieves whether this database supports keeping statements open
		/// across rollbacks.
		/// </summary>
		/// <returns> <code>true</code> if statements always remain open;
		///       <code>false</code> if they might not remain open </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsOpenStatementsAcrossRollback() throws SQLException;
		bool SupportsOpenStatementsAcrossRollback();



		//----------------------------------------------------------------------
		// The following group of methods exposes various limitations
		// based on the target database with the current driver.
		// Unless otherwise specified, a result of zero means there is no
		// limit, or the limit is not known.

		/// <summary>
		/// Retrieves the maximum number of hex characters this database allows in an
		/// inline binary literal.
		/// </summary>
		/// <returns> max the maximum length (in hex characters) for a binary literal;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxBinaryLiteralLength() throws SQLException;
		int MaxBinaryLiteralLength {get;}

		/// <summary>
		/// Retrieves the maximum number of characters this database allows
		/// for a character literal.
		/// </summary>
		/// <returns> the maximum number of characters allowed for a character literal;
		///      a result of zero means that there is no limit or the limit is
		///      not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxCharLiteralLength() throws SQLException;
		int MaxCharLiteralLength {get;}

		/// <summary>
		/// Retrieves the maximum number of characters this database allows
		/// for a column name.
		/// </summary>
		/// <returns> the maximum number of characters allowed for a column name;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxColumnNameLength() throws SQLException;
		int MaxColumnNameLength {get;}

		/// <summary>
		/// Retrieves the maximum number of columns this database allows in a
		/// <code>GROUP BY</code> clause.
		/// </summary>
		/// <returns> the maximum number of columns allowed;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxColumnsInGroupBy() throws SQLException;
		int MaxColumnsInGroupBy {get;}

		/// <summary>
		/// Retrieves the maximum number of columns this database allows in an index.
		/// </summary>
		/// <returns> the maximum number of columns allowed;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxColumnsInIndex() throws SQLException;
		int MaxColumnsInIndex {get;}

		/// <summary>
		/// Retrieves the maximum number of columns this database allows in an
		/// <code>ORDER BY</code> clause.
		/// </summary>
		/// <returns> the maximum number of columns allowed;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxColumnsInOrderBy() throws SQLException;
		int MaxColumnsInOrderBy {get;}

		/// <summary>
		/// Retrieves the maximum number of columns this database allows in a
		/// <code>SELECT</code> list.
		/// </summary>
		/// <returns> the maximum number of columns allowed;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxColumnsInSelect() throws SQLException;
		int MaxColumnsInSelect {get;}

		/// <summary>
		/// Retrieves the maximum number of columns this database allows in a table.
		/// </summary>
		/// <returns> the maximum number of columns allowed;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxColumnsInTable() throws SQLException;
		int MaxColumnsInTable {get;}

		/// <summary>
		/// Retrieves the maximum number of concurrent connections to this
		/// database that are possible.
		/// </summary>
		/// <returns> the maximum number of active connections possible at one time;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxConnections() throws SQLException;
		int MaxConnections {get;}

		/// <summary>
		/// Retrieves the maximum number of characters that this database allows in a
		/// cursor name.
		/// </summary>
		/// <returns> the maximum number of characters allowed in a cursor name;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxCursorNameLength() throws SQLException;
		int MaxCursorNameLength {get;}

		/// <summary>
		/// Retrieves the maximum number of bytes this database allows for an
		/// index, including all of the parts of the index.
		/// </summary>
		/// <returns> the maximum number of bytes allowed; this limit includes the
		///      composite of all the constituent parts of the index;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxIndexLength() throws SQLException;
		int MaxIndexLength {get;}

		/// <summary>
		/// Retrieves the maximum number of characters that this database allows in a
		/// schema name.
		/// </summary>
		/// <returns> the maximum number of characters allowed in a schema name;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxSchemaNameLength() throws SQLException;
		int MaxSchemaNameLength {get;}

		/// <summary>
		/// Retrieves the maximum number of characters that this database allows in a
		/// procedure name.
		/// </summary>
		/// <returns> the maximum number of characters allowed in a procedure name;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxProcedureNameLength() throws SQLException;
		int MaxProcedureNameLength {get;}

		/// <summary>
		/// Retrieves the maximum number of characters that this database allows in a
		/// catalog name.
		/// </summary>
		/// <returns> the maximum number of characters allowed in a catalog name;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxCatalogNameLength() throws SQLException;
		int MaxCatalogNameLength {get;}

		/// <summary>
		/// Retrieves the maximum number of bytes this database allows in
		/// a single row.
		/// </summary>
		/// <returns> the maximum number of bytes allowed for a row; a result of
		///         zero means that there is no limit or the limit is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxRowSize() throws SQLException;
		int MaxRowSize {get;}

		/// <summary>
		/// Retrieves whether the return value for the method
		/// <code>getMaxRowSize</code> includes the SQL data types
		/// <code>LONGVARCHAR</code> and <code>LONGVARBINARY</code>.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean doesMaxRowSizeIncludeBlobs() throws SQLException;
		bool DoesMaxRowSizeIncludeBlobs();

		/// <summary>
		/// Retrieves the maximum number of characters this database allows in
		/// an SQL statement.
		/// </summary>
		/// <returns> the maximum number of characters allowed for an SQL statement;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxStatementLength() throws SQLException;
		int MaxStatementLength {get;}

		/// <summary>
		/// Retrieves the maximum number of active statements to this database
		/// that can be open at the same time.
		/// </summary>
		/// <returns> the maximum number of statements that can be open at one time;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxStatements() throws SQLException;
		int MaxStatements {get;}

		/// <summary>
		/// Retrieves the maximum number of characters this database allows in
		/// a table name.
		/// </summary>
		/// <returns> the maximum number of characters allowed for a table name;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxTableNameLength() throws SQLException;
		int MaxTableNameLength {get;}

		/// <summary>
		/// Retrieves the maximum number of tables this database allows in a
		/// <code>SELECT</code> statement.
		/// </summary>
		/// <returns> the maximum number of tables allowed in a <code>SELECT</code>
		///         statement; a result of zero means that there is no limit or
		///         the limit is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxTablesInSelect() throws SQLException;
		int MaxTablesInSelect {get;}

		/// <summary>
		/// Retrieves the maximum number of characters this database allows in
		/// a user name.
		/// </summary>
		/// <returns> the maximum number of characters allowed for a user name;
		///      a result of zero means that there is no limit or the limit
		///      is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getMaxUserNameLength() throws SQLException;
		int MaxUserNameLength {get;}

		//----------------------------------------------------------------------

		/// <summary>
		/// Retrieves this database's default transaction isolation level.  The
		/// possible values are defined in <code>java.sql.Connection</code>.
		/// </summary>
		/// <returns> the default isolation level </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= Connection </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getDefaultTransactionIsolation() throws SQLException;
		int DefaultTransactionIsolation {get;}

		/// <summary>
		/// Retrieves whether this database supports transactions. If not, invoking the
		/// method <code>commit</code> is a noop, and the isolation level is
		/// <code>TRANSACTION_NONE</code>.
		/// </summary>
		/// <returns> <code>true</code> if transactions are supported;
		///         <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsTransactions() throws SQLException;
		bool SupportsTransactions();

		/// <summary>
		/// Retrieves whether this database supports the given transaction isolation level.
		/// </summary>
		/// <param name="level"> one of the transaction isolation levels defined in
		///         <code>java.sql.Connection</code> </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= Connection </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsTransactionIsolationLevel(int level) throws SQLException;
		bool SupportsTransactionIsolationLevel(int level);

		/// <summary>
		/// Retrieves whether this database supports both data definition and
		/// data manipulation statements within a transaction.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsDataDefinitionAndDataManipulationTransactions() throws SQLException;
		bool SupportsDataDefinitionAndDataManipulationTransactions();
		/// <summary>
		/// Retrieves whether this database supports only data manipulation
		/// statements within a transaction.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsDataManipulationTransactionsOnly() throws SQLException;
		bool SupportsDataManipulationTransactionsOnly();

		/// <summary>
		/// Retrieves whether a data definition statement within a transaction forces
		/// the transaction to commit.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean dataDefinitionCausesTransactionCommit() throws SQLException;
		bool DataDefinitionCausesTransactionCommit();

		/// <summary>
		/// Retrieves whether this database ignores a data definition statement
		/// within a transaction.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean dataDefinitionIgnoredInTransactions() throws SQLException;
		bool DataDefinitionIgnoredInTransactions();

		/// <summary>
		/// Retrieves a description of the stored procedures available in the given
		/// catalog.
		/// <P>
		/// Only procedure descriptions matching the schema and
		/// procedure name criteria are returned.  They are ordered by
		/// <code>PROCEDURE_CAT</code>, <code>PROCEDURE_SCHEM</code>,
		/// <code>PROCEDURE_NAME</code> and <code>SPECIFIC_ NAME</code>.
		/// 
		/// <P>Each procedure description has the the following columns:
		///  <OL>
		///  <LI><B>PROCEDURE_CAT</B> String {@code =>} procedure catalog (may be <code>null</code>)
		///  <LI><B>PROCEDURE_SCHEM</B> String {@code =>} procedure schema (may be <code>null</code>)
		///  <LI><B>PROCEDURE_NAME</B> String {@code =>} procedure name
		///  <LI> reserved for future use
		///  <LI> reserved for future use
		///  <LI> reserved for future use
		///  <LI><B>REMARKS</B> String {@code =>} explanatory comment on the procedure
		///  <LI><B>PROCEDURE_TYPE</B> short {@code =>} kind of procedure:
		///      <UL>
		///      <LI> procedureResultUnknown - Cannot determine if  a return value
		///       will be returned
		///      <LI> procedureNoResult - Does not return a return value
		///      <LI> procedureReturnsResult - Returns a return value
		///      </UL>
		///  <LI><B>SPECIFIC_NAME</B> String  {@code =>} The name which uniquely identifies this
		/// procedure within its schema.
		///  </OL>
		/// <para>
		/// A user may not have permissions to execute any of the procedures that are
		/// returned by <code>getProcedures</code>
		/// 
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="procedureNamePattern"> a procedure name pattern; must match the
		///        procedure name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a procedure description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getProcedures(String catalog, String schemaPattern, String procedureNamePattern) throws SQLException;
		ResultSet GetProcedures(String catalog, String schemaPattern, String procedureNamePattern);

		/// <summary>
		/// Indicates that it is not known whether the procedure returns
		/// a result.
		/// <P>
		/// A possible value for column <code>PROCEDURE_TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getProcedures</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the procedure does not return a result.
		/// <P>
		/// A possible value for column <code>PROCEDURE_TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getProcedures</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the procedure returns a result.
		/// <P>
		/// A possible value for column <code>PROCEDURE_TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getProcedures</code>.
		/// </summary>

		/// <summary>
		/// Retrieves a description of the given catalog's stored procedure parameter
		/// and result columns.
		/// 
		/// <P>Only descriptions matching the schema, procedure and
		/// parameter name criteria are returned.  They are ordered by
		/// PROCEDURE_CAT, PROCEDURE_SCHEM, PROCEDURE_NAME and SPECIFIC_NAME. Within this, the return value,
		/// if any, is first. Next are the parameter descriptions in call
		/// order. The column descriptions follow in column number order.
		/// 
		/// <P>Each row in the <code>ResultSet</code> is a parameter description or
		/// column description with the following fields:
		///  <OL>
		///  <LI><B>PROCEDURE_CAT</B> String {@code =>} procedure catalog (may be <code>null</code>)
		///  <LI><B>PROCEDURE_SCHEM</B> String {@code =>} procedure schema (may be <code>null</code>)
		///  <LI><B>PROCEDURE_NAME</B> String {@code =>} procedure name
		///  <LI><B>COLUMN_NAME</B> String {@code =>} column/parameter name
		///  <LI><B>COLUMN_TYPE</B> Short {@code =>} kind of column/parameter:
		///      <UL>
		///      <LI> procedureColumnUnknown - nobody knows
		///      <LI> procedureColumnIn - IN parameter
		///      <LI> procedureColumnInOut - INOUT parameter
		///      <LI> procedureColumnOut - OUT parameter
		///      <LI> procedureColumnReturn - procedure return value
		///      <LI> procedureColumnResult - result column in <code>ResultSet</code>
		///      </UL>
		///  <LI><B>DATA_TYPE</B> int {@code =>} SQL type from java.sql.Types
		///  <LI><B>TYPE_NAME</B> String {@code =>} SQL type name, for a UDT type the
		///  type name is fully qualified
		///  <LI><B>PRECISION</B> int {@code =>} precision
		///  <LI><B>LENGTH</B> int {@code =>} length in bytes of data
		///  <LI><B>SCALE</B> short {@code =>} scale -  null is returned for data types where
		/// SCALE is not applicable.
		///  <LI><B>RADIX</B> short {@code =>} radix
		///  <LI><B>NULLABLE</B> short {@code =>} can it contain NULL.
		///      <UL>
		///      <LI> procedureNoNulls - does not allow NULL values
		///      <LI> procedureNullable - allows NULL values
		///      <LI> procedureNullableUnknown - nullability unknown
		///      </UL>
		///  <LI><B>REMARKS</B> String {@code =>} comment describing parameter/column
		///  <LI><B>COLUMN_DEF</B> String {@code =>} default value for the column, which should be interpreted as a string when the value is enclosed in single quotes (may be <code>null</code>)
		///      <UL>
		///      <LI> The string NULL (not enclosed in quotes) - if NULL was specified as the default value
		///      <LI> TRUNCATE (not enclosed in quotes)        - if the specified default value cannot be represented without truncation
		///      <LI> NULL                                     - if a default value was not specified
		///      </UL>
		///  <LI><B>SQL_DATA_TYPE</B> int  {@code =>} reserved for future use
		///  <LI><B>SQL_DATETIME_SUB</B> int  {@code =>} reserved for future use
		///  <LI><B>CHAR_OCTET_LENGTH</B> int  {@code =>} the maximum length of binary and character based columns.  For any other datatype the returned value is a
		/// NULL
		///  <LI><B>ORDINAL_POSITION</B> int  {@code =>} the ordinal position, starting from 1, for the input and output parameters for a procedure. A value of 0
		/// is returned if this row describes the procedure's return value.  For result set columns, it is the
		/// ordinal position of the column in the result set starting from 1.  If there are
		/// multiple result sets, the column ordinal positions are implementation
		/// defined.
		///  <LI><B>IS_NULLABLE</B> String  {@code =>} ISO rules are used to determine the nullability for a column.
		///       <UL>
		///       <LI> YES           --- if the column can include NULLs
		///       <LI> NO            --- if the column cannot include NULLs
		///       <LI> empty string  --- if the nullability for the
		/// column is unknown
		///       </UL>
		///  <LI><B>SPECIFIC_NAME</B> String  {@code =>} the name which uniquely identifies this procedure within its schema.
		///  </OL>
		/// 
		/// <P><B>Note:</B> Some databases may not return the column
		/// descriptions for a procedure.
		/// 
		/// <para>The PRECISION column represents the specified column size for the given column.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. Null is returned for data types where the
		/// column size is not applicable.
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="procedureNamePattern"> a procedure name pattern; must match the
		///        procedure name as it is stored in the database </param>
		/// <param name="columnNamePattern"> a column name pattern; must match the column name
		///        as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row describes a stored procedure parameter or
		///      column </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getProcedureColumns(String catalog, String schemaPattern, String procedureNamePattern, String columnNamePattern) throws SQLException;
		ResultSet GetProcedureColumns(String catalog, String schemaPattern, String procedureNamePattern, String columnNamePattern);

		/// <summary>
		/// Indicates that type of the column is unknown.
		/// <P>
		/// A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getProcedureColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the column stores IN parameters.
		/// <P>
		/// A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getProcedureColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the column stores INOUT parameters.
		/// <P>
		/// A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getProcedureColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the column stores OUT parameters.
		/// <P>
		/// A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getProcedureColumns</code>.
		/// </summary>
		/// <summary>
		/// Indicates that the column stores return values.
		/// <P>
		/// A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getProcedureColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the column stores results.
		/// <P>
		/// A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getProcedureColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that <code>NULL</code> values are not allowed.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getProcedureColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that <code>NULL</code> values are allowed.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getProcedureColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that whether <code>NULL</code> values are allowed
		/// is unknown.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getProcedureColumns</code>.
		/// </summary>


		/// <summary>
		/// Retrieves a description of the tables available in the given catalog.
		/// Only table descriptions matching the catalog, schema, table
		/// name and type criteria are returned.  They are ordered by
		/// <code>TABLE_TYPE</code>, <code>TABLE_CAT</code>,
		/// <code>TABLE_SCHEM</code> and <code>TABLE_NAME</code>.
		/// <P>
		/// Each table description has the following columns:
		///  <OL>
		///  <LI><B>TABLE_CAT</B> String {@code =>} table catalog (may be <code>null</code>)
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} table schema (may be <code>null</code>)
		///  <LI><B>TABLE_NAME</B> String {@code =>} table name
		///  <LI><B>TABLE_TYPE</B> String {@code =>} table type.  Typical types are "TABLE",
		///                  "VIEW", "SYSTEM TABLE", "GLOBAL TEMPORARY",
		///                  "LOCAL TEMPORARY", "ALIAS", "SYNONYM".
		///  <LI><B>REMARKS</B> String {@code =>} explanatory comment on the table
		///  <LI><B>TYPE_CAT</B> String {@code =>} the types catalog (may be <code>null</code>)
		///  <LI><B>TYPE_SCHEM</B> String {@code =>} the types schema (may be <code>null</code>)
		///  <LI><B>TYPE_NAME</B> String {@code =>} type name (may be <code>null</code>)
		///  <LI><B>SELF_REFERENCING_COL_NAME</B> String {@code =>} name of the designated
		///                  "identifier" column of a typed table (may be <code>null</code>)
		///  <LI><B>REF_GENERATION</B> String {@code =>} specifies how values in
		///                  SELF_REFERENCING_COL_NAME are created. Values are
		///                  "SYSTEM", "USER", "DERIVED". (may be <code>null</code>)
		///  </OL>
		/// 
		/// <P><B>Note:</B> Some databases may not return information for
		/// all tables.
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="tableNamePattern"> a table name pattern; must match the
		///        table name as it is stored in the database </param>
		/// <param name="types"> a list of table types, which must be from the list of table types
		///         returned from <seealso cref="#getTableTypes"/>,to include; <code>null</code> returns
		/// all types </param>
		/// <returns> <code>ResultSet</code> - each row is a table description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getTables(String catalog, String schemaPattern, String tableNamePattern, String types[]) throws SQLException;
		ResultSet GetTables(String catalog, String schemaPattern, String tableNamePattern, String[] types);

		/// <summary>
		/// Retrieves the schema names available in this database.  The results
		/// are ordered by <code>TABLE_CATALOG</code> and
		/// <code>TABLE_SCHEM</code>.
		/// 
		/// <P>The schema columns are:
		///  <OL>
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} schema name
		///  <LI><B>TABLE_CATALOG</B> String {@code =>} catalog name (may be <code>null</code>)
		///  </OL>
		/// </summary>
		/// <returns> a <code>ResultSet</code> object in which each row is a
		///         schema description </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getSchemas() throws SQLException;
		ResultSet Schemas {get;}

		/// <summary>
		/// Retrieves the catalog names available in this database.  The results
		/// are ordered by catalog name.
		/// 
		/// <P>The catalog column is:
		///  <OL>
		///  <LI><B>TABLE_CAT</B> String {@code =>} catalog name
		///  </OL>
		/// </summary>
		/// <returns> a <code>ResultSet</code> object in which each row has a
		///         single <code>String</code> column that is a catalog name </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getCatalogs() throws SQLException;
		ResultSet Catalogs {get;}

		/// <summary>
		/// Retrieves the table types available in this database.  The results
		/// are ordered by table type.
		/// 
		/// <P>The table type is:
		///  <OL>
		///  <LI><B>TABLE_TYPE</B> String {@code =>} table type.  Typical types are "TABLE",
		///                  "VIEW", "SYSTEM TABLE", "GLOBAL TEMPORARY",
		///                  "LOCAL TEMPORARY", "ALIAS", "SYNONYM".
		///  </OL>
		/// </summary>
		/// <returns> a <code>ResultSet</code> object in which each row has a
		///         single <code>String</code> column that is a table type </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getTableTypes() throws SQLException;
		ResultSet TableTypes {get;}

		/// <summary>
		/// Retrieves a description of table columns available in
		/// the specified catalog.
		/// 
		/// <P>Only column descriptions matching the catalog, schema, table
		/// and column name criteria are returned.  They are ordered by
		/// <code>TABLE_CAT</code>,<code>TABLE_SCHEM</code>,
		/// <code>TABLE_NAME</code>, and <code>ORDINAL_POSITION</code>.
		/// 
		/// <P>Each column description has the following columns:
		///  <OL>
		///  <LI><B>TABLE_CAT</B> String {@code =>} table catalog (may be <code>null</code>)
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} table schema (may be <code>null</code>)
		///  <LI><B>TABLE_NAME</B> String {@code =>} table name
		///  <LI><B>COLUMN_NAME</B> String {@code =>} column name
		///  <LI><B>DATA_TYPE</B> int {@code =>} SQL type from java.sql.Types
		///  <LI><B>TYPE_NAME</B> String {@code =>} Data source dependent type name,
		///  for a UDT the type name is fully qualified
		///  <LI><B>COLUMN_SIZE</B> int {@code =>} column size.
		///  <LI><B>BUFFER_LENGTH</B> is not used.
		///  <LI><B>DECIMAL_DIGITS</B> int {@code =>} the number of fractional digits. Null is returned for data types where
		/// DECIMAL_DIGITS is not applicable.
		///  <LI><B>NUM_PREC_RADIX</B> int {@code =>} Radix (typically either 10 or 2)
		///  <LI><B>NULLABLE</B> int {@code =>} is NULL allowed.
		///      <UL>
		///      <LI> columnNoNulls - might not allow <code>NULL</code> values
		///      <LI> columnNullable - definitely allows <code>NULL</code> values
		///      <LI> columnNullableUnknown - nullability unknown
		///      </UL>
		///  <LI><B>REMARKS</B> String {@code =>} comment describing column (may be <code>null</code>)
		///  <LI><B>COLUMN_DEF</B> String {@code =>} default value for the column, which should be interpreted as a string when the value is enclosed in single quotes (may be <code>null</code>)
		///  <LI><B>SQL_DATA_TYPE</B> int {@code =>} unused
		///  <LI><B>SQL_DATETIME_SUB</B> int {@code =>} unused
		///  <LI><B>CHAR_OCTET_LENGTH</B> int {@code =>} for char types the
		///       maximum number of bytes in the column
		///  <LI><B>ORDINAL_POSITION</B> int {@code =>} index of column in table
		///      (starting at 1)
		///  <LI><B>IS_NULLABLE</B> String  {@code =>} ISO rules are used to determine the nullability for a column.
		///       <UL>
		///       <LI> YES           --- if the column can include NULLs
		///       <LI> NO            --- if the column cannot include NULLs
		///       <LI> empty string  --- if the nullability for the
		/// column is unknown
		///       </UL>
		///  <LI><B>SCOPE_CATALOG</B> String {@code =>} catalog of table that is the scope
		///      of a reference attribute (<code>null</code> if DATA_TYPE isn't REF)
		///  <LI><B>SCOPE_SCHEMA</B> String {@code =>} schema of table that is the scope
		///      of a reference attribute (<code>null</code> if the DATA_TYPE isn't REF)
		///  <LI><B>SCOPE_TABLE</B> String {@code =>} table name that this the scope
		///      of a reference attribute (<code>null</code> if the DATA_TYPE isn't REF)
		///  <LI><B>SOURCE_DATA_TYPE</B> short {@code =>} source type of a distinct type or user-generated
		///      Ref type, SQL type from java.sql.Types (<code>null</code> if DATA_TYPE
		///      isn't DISTINCT or user-generated REF)
		///   <LI><B>IS_AUTOINCREMENT</B> String  {@code =>} Indicates whether this column is auto incremented
		///       <UL>
		///       <LI> YES           --- if the column is auto incremented
		///       <LI> NO            --- if the column is not auto incremented
		///       <LI> empty string  --- if it cannot be determined whether the column is auto incremented
		///       </UL>
		///   <LI><B>IS_GENERATEDCOLUMN</B> String  {@code =>} Indicates whether this is a generated column
		///       <UL>
		///       <LI> YES           --- if this a generated column
		///       <LI> NO            --- if this not a generated column
		///       <LI> empty string  --- if it cannot be determined whether this is a generated column
		///       </UL>
		///  </OL>
		/// 
		/// <para>The COLUMN_SIZE column specifies the column size for the given column.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. Null is returned for data types where the
		/// column size is not applicable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="tableNamePattern"> a table name pattern; must match the
		///        table name as it is stored in the database </param>
		/// <param name="columnNamePattern"> a column name pattern; must match the column
		///        name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a column description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getColumns(String catalog, String schemaPattern, String tableNamePattern, String columnNamePattern) throws SQLException;
		ResultSet GetColumns(String catalog, String schemaPattern, String tableNamePattern, String columnNamePattern);

		/// <summary>
		/// Indicates that the column might not allow <code>NULL</code> values.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code>
		/// in the <code>ResultSet</code> returned by the method
		/// <code>getColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the column definitely allows <code>NULL</code> values.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code>
		/// in the <code>ResultSet</code> returned by the method
		/// <code>getColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the nullability of columns is unknown.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code>
		/// in the <code>ResultSet</code> returned by the method
		/// <code>getColumns</code>.
		/// </summary>

		/// <summary>
		/// Retrieves a description of the access rights for a table's columns.
		/// 
		/// <P>Only privileges matching the column name criteria are
		/// returned.  They are ordered by COLUMN_NAME and PRIVILEGE.
		/// 
		/// <P>Each privilege description has the following columns:
		///  <OL>
		///  <LI><B>TABLE_CAT</B> String {@code =>} table catalog (may be <code>null</code>)
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} table schema (may be <code>null</code>)
		///  <LI><B>TABLE_NAME</B> String {@code =>} table name
		///  <LI><B>COLUMN_NAME</B> String {@code =>} column name
		///  <LI><B>GRANTOR</B> String {@code =>} grantor of access (may be <code>null</code>)
		///  <LI><B>GRANTEE</B> String {@code =>} grantee of access
		///  <LI><B>PRIVILEGE</B> String {@code =>} name of access (SELECT,
		///      INSERT, UPDATE, REFRENCES, ...)
		///  <LI><B>IS_GRANTABLE</B> String {@code =>} "YES" if grantee is permitted
		///      to grant to others; "NO" if not; <code>null</code> if unknown
		///  </OL>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schema"> a schema name; must match the schema name as it is
		///        stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="table"> a table name; must match the table name as it is
		///        stored in the database </param>
		/// <param name="columnNamePattern"> a column name pattern; must match the column
		///        name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a column privilege description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getColumnPrivileges(String catalog, String schema, String table, String columnNamePattern) throws SQLException;
		ResultSet GetColumnPrivileges(String catalog, String schema, String table, String columnNamePattern);

		/// <summary>
		/// Retrieves a description of the access rights for each table available
		/// in a catalog. Note that a table privilege applies to one or
		/// more columns in the table. It would be wrong to assume that
		/// this privilege applies to all columns (this may be true for
		/// some systems but is not true for all.)
		/// 
		/// <P>Only privileges matching the schema and table name
		/// criteria are returned.  They are ordered by
		/// <code>TABLE_CAT</code>,
		/// <code>TABLE_SCHEM</code>, <code>TABLE_NAME</code>,
		/// and <code>PRIVILEGE</code>.
		/// 
		/// <P>Each privilege description has the following columns:
		///  <OL>
		///  <LI><B>TABLE_CAT</B> String {@code =>} table catalog (may be <code>null</code>)
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} table schema (may be <code>null</code>)
		///  <LI><B>TABLE_NAME</B> String {@code =>} table name
		///  <LI><B>GRANTOR</B> String {@code =>} grantor of access (may be <code>null</code>)
		///  <LI><B>GRANTEE</B> String {@code =>} grantee of access
		///  <LI><B>PRIVILEGE</B> String {@code =>} name of access (SELECT,
		///      INSERT, UPDATE, REFRENCES, ...)
		///  <LI><B>IS_GRANTABLE</B> String {@code =>} "YES" if grantee is permitted
		///      to grant to others; "NO" if not; <code>null</code> if unknown
		///  </OL>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="tableNamePattern"> a table name pattern; must match the
		///        table name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a table privilege description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getTablePrivileges(String catalog, String schemaPattern, String tableNamePattern) throws SQLException;
		ResultSet GetTablePrivileges(String catalog, String schemaPattern, String tableNamePattern);

		/// <summary>
		/// Retrieves a description of a table's optimal set of columns that
		/// uniquely identifies a row. They are ordered by SCOPE.
		/// 
		/// <P>Each column description has the following columns:
		///  <OL>
		///  <LI><B>SCOPE</B> short {@code =>} actual scope of result
		///      <UL>
		///      <LI> bestRowTemporary - very temporary, while using row
		///      <LI> bestRowTransaction - valid for remainder of current transaction
		///      <LI> bestRowSession - valid for remainder of current session
		///      </UL>
		///  <LI><B>COLUMN_NAME</B> String {@code =>} column name
		///  <LI><B>DATA_TYPE</B> int {@code =>} SQL data type from java.sql.Types
		///  <LI><B>TYPE_NAME</B> String {@code =>} Data source dependent type name,
		///  for a UDT the type name is fully qualified
		///  <LI><B>COLUMN_SIZE</B> int {@code =>} precision
		///  <LI><B>BUFFER_LENGTH</B> int {@code =>} not used
		///  <LI><B>DECIMAL_DIGITS</B> short  {@code =>} scale - Null is returned for data types where
		/// DECIMAL_DIGITS is not applicable.
		///  <LI><B>PSEUDO_COLUMN</B> short {@code =>} is this a pseudo column
		///      like an Oracle ROWID
		///      <UL>
		///      <LI> bestRowUnknown - may or may not be pseudo column
		///      <LI> bestRowNotPseudo - is NOT a pseudo column
		///      <LI> bestRowPseudo - is a pseudo column
		///      </UL>
		///  </OL>
		/// 
		/// <para>The COLUMN_SIZE column represents the specified column size for the given column.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. Null is returned for data types where the
		/// column size is not applicable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schema"> a schema name; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="table"> a table name; must match the table name as it is stored
		///        in the database </param>
		/// <param name="scope"> the scope of interest; use same values as SCOPE </param>
		/// <param name="nullable"> include columns that are nullable. </param>
		/// <returns> <code>ResultSet</code> - each row is a column description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getBestRowIdentifier(String catalog, String schema, String table, int scope, boolean nullable) throws SQLException;
		ResultSet GetBestRowIdentifier(String catalog, String schema, String table, int scope, bool nullable);

		/// <summary>
		/// Indicates that the scope of the best row identifier is
		/// very temporary, lasting only while the
		/// row is being used.
		/// <P>
		/// A possible value for the column
		/// <code>SCOPE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getBestRowIdentifier</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the scope of the best row identifier is
		/// the remainder of the current transaction.
		/// <P>
		/// A possible value for the column
		/// <code>SCOPE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getBestRowIdentifier</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the scope of the best row identifier is
		/// the remainder of the current session.
		/// <P>
		/// A possible value for the column
		/// <code>SCOPE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getBestRowIdentifier</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the best row identifier may or may not be a pseudo column.
		/// <P>
		/// A possible value for the column
		/// <code>PSEUDO_COLUMN</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getBestRowIdentifier</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the best row identifier is NOT a pseudo column.
		/// <P>
		/// A possible value for the column
		/// <code>PSEUDO_COLUMN</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getBestRowIdentifier</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the best row identifier is a pseudo column.
		/// <P>
		/// A possible value for the column
		/// <code>PSEUDO_COLUMN</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getBestRowIdentifier</code>.
		/// </summary>

		/// <summary>
		/// Retrieves a description of a table's columns that are automatically
		/// updated when any value in a row is updated.  They are
		/// unordered.
		/// 
		/// <P>Each column description has the following columns:
		///  <OL>
		///  <LI><B>SCOPE</B> short {@code =>} is not used
		///  <LI><B>COLUMN_NAME</B> String {@code =>} column name
		///  <LI><B>DATA_TYPE</B> int {@code =>} SQL data type from <code>java.sql.Types</code>
		///  <LI><B>TYPE_NAME</B> String {@code =>} Data source-dependent type name
		///  <LI><B>COLUMN_SIZE</B> int {@code =>} precision
		///  <LI><B>BUFFER_LENGTH</B> int {@code =>} length of column value in bytes
		///  <LI><B>DECIMAL_DIGITS</B> short  {@code =>} scale - Null is returned for data types where
		/// DECIMAL_DIGITS is not applicable.
		///  <LI><B>PSEUDO_COLUMN</B> short {@code =>} whether this is pseudo column
		///      like an Oracle ROWID
		///      <UL>
		///      <LI> versionColumnUnknown - may or may not be pseudo column
		///      <LI> versionColumnNotPseudo - is NOT a pseudo column
		///      <LI> versionColumnPseudo - is a pseudo column
		///      </UL>
		///  </OL>
		/// 
		/// <para>The COLUMN_SIZE column represents the specified column size for the given column.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. Null is returned for data types where the
		/// column size is not applicable.
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schema"> a schema name; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="table"> a table name; must match the table name as it is stored
		///        in the database </param>
		/// <returns> a <code>ResultSet</code> object in which each row is a
		///         column description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getVersionColumns(String catalog, String schema, String table) throws SQLException;
		ResultSet GetVersionColumns(String catalog, String schema, String table);

		/// <summary>
		/// Indicates that this version column may or may not be a pseudo column.
		/// <P>
		/// A possible value for the column
		/// <code>PSEUDO_COLUMN</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getVersionColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that this version column is NOT a pseudo column.
		/// <P>
		/// A possible value for the column
		/// <code>PSEUDO_COLUMN</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getVersionColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that this version column is a pseudo column.
		/// <P>
		/// A possible value for the column
		/// <code>PSEUDO_COLUMN</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getVersionColumns</code>.
		/// </summary>

		/// <summary>
		/// Retrieves a description of the given table's primary key columns.  They
		/// are ordered by COLUMN_NAME.
		/// 
		/// <P>Each primary key column description has the following columns:
		///  <OL>
		///  <LI><B>TABLE_CAT</B> String {@code =>} table catalog (may be <code>null</code>)
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} table schema (may be <code>null</code>)
		///  <LI><B>TABLE_NAME</B> String {@code =>} table name
		///  <LI><B>COLUMN_NAME</B> String {@code =>} column name
		///  <LI><B>KEY_SEQ</B> short {@code =>} sequence number within primary key( a value
		///  of 1 represents the first column of the primary key, a value of 2 would
		///  represent the second column within the primary key).
		///  <LI><B>PK_NAME</B> String {@code =>} primary key name (may be <code>null</code>)
		///  </OL>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schema"> a schema name; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="table"> a table name; must match the table name as it is stored
		///        in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a primary key column description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getPrimaryKeys(String catalog, String schema, String table) throws SQLException;
		ResultSet GetPrimaryKeys(String catalog, String schema, String table);

		/// <summary>
		/// Retrieves a description of the primary key columns that are
		/// referenced by the given table's foreign key columns (the primary keys
		/// imported by a table).  They are ordered by PKTABLE_CAT,
		/// PKTABLE_SCHEM, PKTABLE_NAME, and KEY_SEQ.
		/// 
		/// <P>Each primary key column description has the following columns:
		///  <OL>
		///  <LI><B>PKTABLE_CAT</B> String {@code =>} primary key table catalog
		///      being imported (may be <code>null</code>)
		///  <LI><B>PKTABLE_SCHEM</B> String {@code =>} primary key table schema
		///      being imported (may be <code>null</code>)
		///  <LI><B>PKTABLE_NAME</B> String {@code =>} primary key table name
		///      being imported
		///  <LI><B>PKCOLUMN_NAME</B> String {@code =>} primary key column name
		///      being imported
		///  <LI><B>FKTABLE_CAT</B> String {@code =>} foreign key table catalog (may be <code>null</code>)
		///  <LI><B>FKTABLE_SCHEM</B> String {@code =>} foreign key table schema (may be <code>null</code>)
		///  <LI><B>FKTABLE_NAME</B> String {@code =>} foreign key table name
		///  <LI><B>FKCOLUMN_NAME</B> String {@code =>} foreign key column name
		///  <LI><B>KEY_SEQ</B> short {@code =>} sequence number within a foreign key( a value
		///  of 1 represents the first column of the foreign key, a value of 2 would
		///  represent the second column within the foreign key).
		///  <LI><B>UPDATE_RULE</B> short {@code =>} What happens to a
		///       foreign key when the primary key is updated:
		///      <UL>
		///      <LI> importedNoAction - do not allow update of primary
		///               key if it has been imported
		///      <LI> importedKeyCascade - change imported key to agree
		///               with primary key update
		///      <LI> importedKeySetNull - change imported key to <code>NULL</code>
		///               if its primary key has been updated
		///      <LI> importedKeySetDefault - change imported key to default values
		///               if its primary key has been updated
		///      <LI> importedKeyRestrict - same as importedKeyNoAction
		///                                 (for ODBC 2.x compatibility)
		///      </UL>
		///  <LI><B>DELETE_RULE</B> short {@code =>} What happens to
		///      the foreign key when primary is deleted.
		///      <UL>
		///      <LI> importedKeyNoAction - do not allow delete of primary
		///               key if it has been imported
		///      <LI> importedKeyCascade - delete rows that import a deleted key
		///      <LI> importedKeySetNull - change imported key to NULL if
		///               its primary key has been deleted
		///      <LI> importedKeyRestrict - same as importedKeyNoAction
		///                                 (for ODBC 2.x compatibility)
		///      <LI> importedKeySetDefault - change imported key to default if
		///               its primary key has been deleted
		///      </UL>
		///  <LI><B>FK_NAME</B> String {@code =>} foreign key name (may be <code>null</code>)
		///  <LI><B>PK_NAME</B> String {@code =>} primary key name (may be <code>null</code>)
		///  <LI><B>DEFERRABILITY</B> short {@code =>} can the evaluation of foreign key
		///      constraints be deferred until commit
		///      <UL>
		///      <LI> importedKeyInitiallyDeferred - see SQL92 for definition
		///      <LI> importedKeyInitiallyImmediate - see SQL92 for definition
		///      <LI> importedKeyNotDeferrable - see SQL92 for definition
		///      </UL>
		///  </OL>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schema"> a schema name; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="table"> a table name; must match the table name as it is stored
		///        in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a primary key column description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getExportedKeys </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getImportedKeys(String catalog, String schema, String table) throws SQLException;
		ResultSet GetImportedKeys(String catalog, String schema, String table);

		/// <summary>
		/// For the column <code>UPDATE_RULE</code>,
		/// indicates that
		/// when the primary key is updated, the foreign key (imported key)
		/// is changed to agree with it.
		/// For the column <code>DELETE_RULE</code>,
		/// it indicates that
		/// when the primary key is deleted, rows that imported that key
		/// are deleted.
		/// <P>
		/// A possible value for the columns <code>UPDATE_RULE</code>
		/// and <code>DELETE_RULE</code> in the
		/// <code>ResultSet</code> objects returned by the methods
		/// <code>getImportedKeys</code>,  <code>getExportedKeys</code>,
		/// and <code>getCrossReference</code>.
		/// </summary>

		/// <summary>
		/// For the column <code>UPDATE_RULE</code>, indicates that
		/// a primary key may not be updated if it has been imported by
		/// another table as a foreign key.
		/// For the column <code>DELETE_RULE</code>, indicates that
		/// a primary key may not be deleted if it has been imported by
		/// another table as a foreign key.
		/// <P>
		/// A possible value for the columns <code>UPDATE_RULE</code>
		/// and <code>DELETE_RULE</code> in the
		/// <code>ResultSet</code> objects returned by the methods
		/// <code>getImportedKeys</code>,  <code>getExportedKeys</code>,
		/// and <code>getCrossReference</code>.
		/// </summary>

		/// <summary>
		/// For the columns <code>UPDATE_RULE</code>
		/// and <code>DELETE_RULE</code>, indicates that
		/// when the primary key is updated or deleted, the foreign key (imported key)
		/// is changed to <code>NULL</code>.
		/// <P>
		/// A possible value for the columns <code>UPDATE_RULE</code>
		/// and <code>DELETE_RULE</code> in the
		/// <code>ResultSet</code> objects returned by the methods
		/// <code>getImportedKeys</code>,  <code>getExportedKeys</code>,
		/// and <code>getCrossReference</code>.
		/// </summary>

		/// <summary>
		/// For the columns <code>UPDATE_RULE</code>
		/// and <code>DELETE_RULE</code>, indicates that
		/// if the primary key has been imported, it cannot be updated or deleted.
		/// <P>
		/// A possible value for the columns <code>UPDATE_RULE</code>
		/// and <code>DELETE_RULE</code> in the
		/// <code>ResultSet</code> objects returned by the methods
		/// <code>getImportedKeys</code>,  <code>getExportedKeys</code>,
		/// and <code>getCrossReference</code>.
		/// </summary>

		/// <summary>
		/// For the columns <code>UPDATE_RULE</code>
		/// and <code>DELETE_RULE</code>, indicates that
		/// if the primary key is updated or deleted, the foreign key (imported key)
		/// is set to the default value.
		/// <P>
		/// A possible value for the columns <code>UPDATE_RULE</code>
		/// and <code>DELETE_RULE</code> in the
		/// <code>ResultSet</code> objects returned by the methods
		/// <code>getImportedKeys</code>,  <code>getExportedKeys</code>,
		/// and <code>getCrossReference</code>.
		/// </summary>

		/// <summary>
		/// Indicates deferrability.  See SQL-92 for a definition.
		/// <P>
		/// A possible value for the column <code>DEFERRABILITY</code>
		/// in the <code>ResultSet</code> objects returned by the methods
		/// <code>getImportedKeys</code>,  <code>getExportedKeys</code>,
		/// and <code>getCrossReference</code>.
		/// </summary>

		/// <summary>
		/// Indicates deferrability.  See SQL-92 for a definition.
		/// <P>
		/// A possible value for the column <code>DEFERRABILITY</code>
		/// in the <code>ResultSet</code> objects returned by the methods
		/// <code>getImportedKeys</code>,  <code>getExportedKeys</code>,
		/// and <code>getCrossReference</code>.
		/// </summary>

		/// <summary>
		/// Indicates deferrability.  See SQL-92 for a definition.
		/// <P>
		/// A possible value for the column <code>DEFERRABILITY</code>
		/// in the <code>ResultSet</code> objects returned by the methods
		/// <code>getImportedKeys</code>,  <code>getExportedKeys</code>,
		/// and <code>getCrossReference</code>.
		/// </summary>

		/// <summary>
		/// Retrieves a description of the foreign key columns that reference the
		/// given table's primary key columns (the foreign keys exported by a
		/// table).  They are ordered by FKTABLE_CAT, FKTABLE_SCHEM,
		/// FKTABLE_NAME, and KEY_SEQ.
		/// 
		/// <P>Each foreign key column description has the following columns:
		///  <OL>
		///  <LI><B>PKTABLE_CAT</B> String {@code =>} primary key table catalog (may be <code>null</code>)
		///  <LI><B>PKTABLE_SCHEM</B> String {@code =>} primary key table schema (may be <code>null</code>)
		///  <LI><B>PKTABLE_NAME</B> String {@code =>} primary key table name
		///  <LI><B>PKCOLUMN_NAME</B> String {@code =>} primary key column name
		///  <LI><B>FKTABLE_CAT</B> String {@code =>} foreign key table catalog (may be <code>null</code>)
		///      being exported (may be <code>null</code>)
		///  <LI><B>FKTABLE_SCHEM</B> String {@code =>} foreign key table schema (may be <code>null</code>)
		///      being exported (may be <code>null</code>)
		///  <LI><B>FKTABLE_NAME</B> String {@code =>} foreign key table name
		///      being exported
		///  <LI><B>FKCOLUMN_NAME</B> String {@code =>} foreign key column name
		///      being exported
		///  <LI><B>KEY_SEQ</B> short {@code =>} sequence number within foreign key( a value
		///  of 1 represents the first column of the foreign key, a value of 2 would
		///  represent the second column within the foreign key).
		///  <LI><B>UPDATE_RULE</B> short {@code =>} What happens to
		///       foreign key when primary is updated:
		///      <UL>
		///      <LI> importedNoAction - do not allow update of primary
		///               key if it has been imported
		///      <LI> importedKeyCascade - change imported key to agree
		///               with primary key update
		///      <LI> importedKeySetNull - change imported key to <code>NULL</code> if
		///               its primary key has been updated
		///      <LI> importedKeySetDefault - change imported key to default values
		///               if its primary key has been updated
		///      <LI> importedKeyRestrict - same as importedKeyNoAction
		///                                 (for ODBC 2.x compatibility)
		///      </UL>
		///  <LI><B>DELETE_RULE</B> short {@code =>} What happens to
		///      the foreign key when primary is deleted.
		///      <UL>
		///      <LI> importedKeyNoAction - do not allow delete of primary
		///               key if it has been imported
		///      <LI> importedKeyCascade - delete rows that import a deleted key
		///      <LI> importedKeySetNull - change imported key to <code>NULL</code> if
		///               its primary key has been deleted
		///      <LI> importedKeyRestrict - same as importedKeyNoAction
		///                                 (for ODBC 2.x compatibility)
		///      <LI> importedKeySetDefault - change imported key to default if
		///               its primary key has been deleted
		///      </UL>
		///  <LI><B>FK_NAME</B> String {@code =>} foreign key name (may be <code>null</code>)
		///  <LI><B>PK_NAME</B> String {@code =>} primary key name (may be <code>null</code>)
		///  <LI><B>DEFERRABILITY</B> short {@code =>} can the evaluation of foreign key
		///      constraints be deferred until commit
		///      <UL>
		///      <LI> importedKeyInitiallyDeferred - see SQL92 for definition
		///      <LI> importedKeyInitiallyImmediate - see SQL92 for definition
		///      <LI> importedKeyNotDeferrable - see SQL92 for definition
		///      </UL>
		///  </OL>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in this database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schema"> a schema name; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="table"> a table name; must match the table name as it is stored
		///        in this database </param>
		/// <returns> a <code>ResultSet</code> object in which each row is a
		///         foreign key column description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getImportedKeys </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getExportedKeys(String catalog, String schema, String table) throws SQLException;
		ResultSet GetExportedKeys(String catalog, String schema, String table);

		/// <summary>
		/// Retrieves a description of the foreign key columns in the given foreign key
		/// table that reference the primary key or the columns representing a unique constraint of the  parent table (could be the same or a different table).
		/// The number of columns returned from the parent table must match the number of
		/// columns that make up the foreign key.  They
		/// are ordered by FKTABLE_CAT, FKTABLE_SCHEM, FKTABLE_NAME, and
		/// KEY_SEQ.
		/// 
		/// <P>Each foreign key column description has the following columns:
		///  <OL>
		///  <LI><B>PKTABLE_CAT</B> String {@code =>} parent key table catalog (may be <code>null</code>)
		///  <LI><B>PKTABLE_SCHEM</B> String {@code =>} parent key table schema (may be <code>null</code>)
		///  <LI><B>PKTABLE_NAME</B> String {@code =>} parent key table name
		///  <LI><B>PKCOLUMN_NAME</B> String {@code =>} parent key column name
		///  <LI><B>FKTABLE_CAT</B> String {@code =>} foreign key table catalog (may be <code>null</code>)
		///      being exported (may be <code>null</code>)
		///  <LI><B>FKTABLE_SCHEM</B> String {@code =>} foreign key table schema (may be <code>null</code>)
		///      being exported (may be <code>null</code>)
		///  <LI><B>FKTABLE_NAME</B> String {@code =>} foreign key table name
		///      being exported
		///  <LI><B>FKCOLUMN_NAME</B> String {@code =>} foreign key column name
		///      being exported
		///  <LI><B>KEY_SEQ</B> short {@code =>} sequence number within foreign key( a value
		///  of 1 represents the first column of the foreign key, a value of 2 would
		///  represent the second column within the foreign key).
		///  <LI><B>UPDATE_RULE</B> short {@code =>} What happens to
		///       foreign key when parent key is updated:
		///      <UL>
		///      <LI> importedNoAction - do not allow update of parent
		///               key if it has been imported
		///      <LI> importedKeyCascade - change imported key to agree
		///               with parent key update
		///      <LI> importedKeySetNull - change imported key to <code>NULL</code> if
		///               its parent key has been updated
		///      <LI> importedKeySetDefault - change imported key to default values
		///               if its parent key has been updated
		///      <LI> importedKeyRestrict - same as importedKeyNoAction
		///                                 (for ODBC 2.x compatibility)
		///      </UL>
		///  <LI><B>DELETE_RULE</B> short {@code =>} What happens to
		///      the foreign key when parent key is deleted.
		///      <UL>
		///      <LI> importedKeyNoAction - do not allow delete of parent
		///               key if it has been imported
		///      <LI> importedKeyCascade - delete rows that import a deleted key
		///      <LI> importedKeySetNull - change imported key to <code>NULL</code> if
		///               its primary key has been deleted
		///      <LI> importedKeyRestrict - same as importedKeyNoAction
		///                                 (for ODBC 2.x compatibility)
		///      <LI> importedKeySetDefault - change imported key to default if
		///               its parent key has been deleted
		///      </UL>
		///  <LI><B>FK_NAME</B> String {@code =>} foreign key name (may be <code>null</code>)
		///  <LI><B>PK_NAME</B> String {@code =>} parent key name (may be <code>null</code>)
		///  <LI><B>DEFERRABILITY</B> short {@code =>} can the evaluation of foreign key
		///      constraints be deferred until commit
		///      <UL>
		///      <LI> importedKeyInitiallyDeferred - see SQL92 for definition
		///      <LI> importedKeyInitiallyImmediate - see SQL92 for definition
		///      <LI> importedKeyNotDeferrable - see SQL92 for definition
		///      </UL>
		///  </OL>
		/// </summary>
		/// <param name="parentCatalog"> a catalog name; must match the catalog name
		/// as it is stored in the database; "" retrieves those without a
		/// catalog; <code>null</code> means drop catalog name from the selection criteria </param>
		/// <param name="parentSchema"> a schema name; must match the schema name as
		/// it is stored in the database; "" retrieves those without a schema;
		/// <code>null</code> means drop schema name from the selection criteria </param>
		/// <param name="parentTable"> the name of the table that exports the key; must match
		/// the table name as it is stored in the database </param>
		/// <param name="foreignCatalog"> a catalog name; must match the catalog name as
		/// it is stored in the database; "" retrieves those without a
		/// catalog; <code>null</code> means drop catalog name from the selection criteria </param>
		/// <param name="foreignSchema"> a schema name; must match the schema name as it
		/// is stored in the database; "" retrieves those without a schema;
		/// <code>null</code> means drop schema name from the selection criteria </param>
		/// <param name="foreignTable"> the name of the table that imports the key; must match
		/// the table name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a foreign key column description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getImportedKeys </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getCrossReference(String parentCatalog, String parentSchema, String parentTable, String foreignCatalog, String foreignSchema, String foreignTable) throws SQLException;
		ResultSet GetCrossReference(String parentCatalog, String parentSchema, String parentTable, String foreignCatalog, String foreignSchema, String foreignTable);

		/// <summary>
		/// Retrieves a description of all the data types supported by
		/// this database. They are ordered by DATA_TYPE and then by how
		/// closely the data type maps to the corresponding JDBC SQL type.
		/// 
		/// <P>If the database supports SQL distinct types, then getTypeInfo() will return
		/// a single row with a TYPE_NAME of DISTINCT and a DATA_TYPE of Types.DISTINCT.
		/// If the database supports SQL structured types, then getTypeInfo() will return
		/// a single row with a TYPE_NAME of STRUCT and a DATA_TYPE of Types.STRUCT.
		/// 
		/// <P>If SQL distinct or structured types are supported, then information on the
		/// individual types may be obtained from the getUDTs() method.
		/// 
		/// 
		/// 
		/// <P>Each type description has the following columns:
		///  <OL>
		///  <LI><B>TYPE_NAME</B> String {@code =>} Type name
		///  <LI><B>DATA_TYPE</B> int {@code =>} SQL data type from java.sql.Types
		///  <LI><B>PRECISION</B> int {@code =>} maximum precision
		///  <LI><B>LITERAL_PREFIX</B> String {@code =>} prefix used to quote a literal
		///      (may be <code>null</code>)
		///  <LI><B>LITERAL_SUFFIX</B> String {@code =>} suffix used to quote a literal
		/// (may be <code>null</code>)
		///  <LI><B>CREATE_PARAMS</B> String {@code =>} parameters used in creating
		///      the type (may be <code>null</code>)
		///  <LI><B>NULLABLE</B> short {@code =>} can you use NULL for this type.
		///      <UL>
		///      <LI> typeNoNulls - does not allow NULL values
		///      <LI> typeNullable - allows NULL values
		///      <LI> typeNullableUnknown - nullability unknown
		///      </UL>
		///  <LI><B>CASE_SENSITIVE</B> boolean{@code =>} is it case sensitive.
		///  <LI><B>SEARCHABLE</B> short {@code =>} can you use "WHERE" based on this type:
		///      <UL>
		///      <LI> typePredNone - No support
		///      <LI> typePredChar - Only supported with WHERE .. LIKE
		///      <LI> typePredBasic - Supported except for WHERE .. LIKE
		///      <LI> typeSearchable - Supported for all WHERE ..
		///      </UL>
		///  <LI><B>UNSIGNED_ATTRIBUTE</B> boolean {@code =>} is it unsigned.
		///  <LI><B>FIXED_PREC_SCALE</B> boolean {@code =>} can it be a money value.
		///  <LI><B>AUTO_INCREMENT</B> boolean {@code =>} can it be used for an
		///      auto-increment value.
		///  <LI><B>LOCAL_TYPE_NAME</B> String {@code =>} localized version of type name
		///      (may be <code>null</code>)
		///  <LI><B>MINIMUM_SCALE</B> short {@code =>} minimum scale supported
		///  <LI><B>MAXIMUM_SCALE</B> short {@code =>} maximum scale supported
		///  <LI><B>SQL_DATA_TYPE</B> int {@code =>} unused
		///  <LI><B>SQL_DATETIME_SUB</B> int {@code =>} unused
		///  <LI><B>NUM_PREC_RADIX</B> int {@code =>} usually 2 or 10
		///  </OL>
		/// 
		/// <para>The PRECISION column represents the maximum column size that the server supports for the given datatype.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. Null is returned for data types where the
		/// column size is not applicable.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a <code>ResultSet</code> object in which each row is an SQL
		///         type description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getTypeInfo() throws SQLException;
		ResultSet TypeInfo {get;}

		/// <summary>
		/// Indicates that a <code>NULL</code> value is NOT allowed for this
		/// data type.
		/// <P>
		/// A possible value for column <code>NULLABLE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getTypeInfo</code>.
		/// </summary>

		/// <summary>
		/// Indicates that a <code>NULL</code> value is allowed for this
		/// data type.
		/// <P>
		/// A possible value for column <code>NULLABLE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getTypeInfo</code>.
		/// </summary>

		/// <summary>
		/// Indicates that it is not known whether a <code>NULL</code> value
		/// is allowed for this data type.
		/// <P>
		/// A possible value for column <code>NULLABLE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getTypeInfo</code>.
		/// </summary>

		/// <summary>
		/// Indicates that <code>WHERE</code> search clauses are not supported
		/// for this type.
		/// <P>
		/// A possible value for column <code>SEARCHABLE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getTypeInfo</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the data type
		/// can be only be used in <code>WHERE</code> search clauses
		/// that  use <code>LIKE</code> predicates.
		/// <P>
		/// A possible value for column <code>SEARCHABLE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getTypeInfo</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the data type can be only be used in <code>WHERE</code>
		/// search clauses
		/// that do not use <code>LIKE</code> predicates.
		/// <P>
		/// A possible value for column <code>SEARCHABLE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getTypeInfo</code>.
		/// </summary>

		/// <summary>
		/// Indicates that all <code>WHERE</code> search clauses can be
		/// based on this type.
		/// <P>
		/// A possible value for column <code>SEARCHABLE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getTypeInfo</code>.
		/// </summary>

		/// <summary>
		/// Retrieves a description of the given table's indices and statistics. They are
		/// ordered by NON_UNIQUE, TYPE, INDEX_NAME, and ORDINAL_POSITION.
		/// 
		/// <P>Each index column description has the following columns:
		///  <OL>
		///  <LI><B>TABLE_CAT</B> String {@code =>} table catalog (may be <code>null</code>)
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} table schema (may be <code>null</code>)
		///  <LI><B>TABLE_NAME</B> String {@code =>} table name
		///  <LI><B>NON_UNIQUE</B> boolean {@code =>} Can index values be non-unique.
		///      false when TYPE is tableIndexStatistic
		///  <LI><B>INDEX_QUALIFIER</B> String {@code =>} index catalog (may be <code>null</code>);
		///      <code>null</code> when TYPE is tableIndexStatistic
		///  <LI><B>INDEX_NAME</B> String {@code =>} index name; <code>null</code> when TYPE is
		///      tableIndexStatistic
		///  <LI><B>TYPE</B> short {@code =>} index type:
		///      <UL>
		///      <LI> tableIndexStatistic - this identifies table statistics that are
		///           returned in conjuction with a table's index descriptions
		///      <LI> tableIndexClustered - this is a clustered index
		///      <LI> tableIndexHashed - this is a hashed index
		///      <LI> tableIndexOther - this is some other style of index
		///      </UL>
		///  <LI><B>ORDINAL_POSITION</B> short {@code =>} column sequence number
		///      within index; zero when TYPE is tableIndexStatistic
		///  <LI><B>COLUMN_NAME</B> String {@code =>} column name; <code>null</code> when TYPE is
		///      tableIndexStatistic
		///  <LI><B>ASC_OR_DESC</B> String {@code =>} column sort sequence, "A" {@code =>} ascending,
		///      "D" {@code =>} descending, may be <code>null</code> if sort sequence is not supported;
		///      <code>null</code> when TYPE is tableIndexStatistic
		///  <LI><B>CARDINALITY</B> long {@code =>} When TYPE is tableIndexStatistic, then
		///      this is the number of rows in the table; otherwise, it is the
		///      number of unique values in the index.
		///  <LI><B>PAGES</B> long {@code =>} When TYPE is  tableIndexStatisic then
		///      this is the number of pages used for the table, otherwise it
		///      is the number of pages used for the current index.
		///  <LI><B>FILTER_CONDITION</B> String {@code =>} Filter condition, if any.
		///      (may be <code>null</code>)
		///  </OL>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in this database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schema"> a schema name; must match the schema name
		///        as it is stored in this database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="table"> a table name; must match the table name as it is stored
		///        in this database </param>
		/// <param name="unique"> when true, return only indices for unique values;
		///     when false, return indices regardless of whether unique or not </param>
		/// <param name="approximate"> when true, result is allowed to reflect approximate
		///     or out of data values; when false, results are requested to be
		///     accurate </param>
		/// <returns> <code>ResultSet</code> - each row is an index column description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getIndexInfo(String catalog, String schema, String table, boolean unique, boolean approximate) throws SQLException;
		ResultSet GetIndexInfo(String catalog, String schema, String table, bool unique, bool approximate);

		/// <summary>
		/// Indicates that this column contains table statistics that
		/// are returned in conjunction with a table's index descriptions.
		/// <P>
		/// A possible value for column <code>TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getIndexInfo</code>.
		/// </summary>

		/// <summary>
		/// Indicates that this table index is a clustered index.
		/// <P>
		/// A possible value for column <code>TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getIndexInfo</code>.
		/// </summary>

		/// <summary>
		/// Indicates that this table index is a hashed index.
		/// <P>
		/// A possible value for column <code>TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getIndexInfo</code>.
		/// </summary>

		/// <summary>
		/// Indicates that this table index is not a clustered
		/// index, a hashed index, or table statistics;
		/// it is something other than these.
		/// <P>
		/// A possible value for column <code>TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getIndexInfo</code>.
		/// </summary>

		//--------------------------JDBC 2.0-----------------------------

		/// <summary>
		/// Retrieves whether this database supports the given result set type.
		/// </summary>
		/// <param name="type"> defined in <code>java.sql.ResultSet</code> </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= Connection
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsResultSetType(int type) throws SQLException;
		bool SupportsResultSetType(int type);

		/// <summary>
		/// Retrieves whether this database supports the given concurrency type
		/// in combination with the given result set type.
		/// </summary>
		/// <param name="type"> defined in <code>java.sql.ResultSet</code> </param>
		/// <param name="concurrency"> type defined in <code>java.sql.ResultSet</code> </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= Connection
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsResultSetConcurrency(int type, int concurrency) throws SQLException;
		bool SupportsResultSetConcurrency(int type, int concurrency);

		/// 
		/// <summary>
		/// Retrieves whether for the given type of <code>ResultSet</code> object,
		/// the result set's own updates are visible.
		/// </summary>
		/// <param name="type"> the <code>ResultSet</code> type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <returns> <code>true</code> if updates are visible for the given result set type;
		///        <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean ownUpdatesAreVisible(int type) throws SQLException;
		bool OwnUpdatesAreVisible(int type);

		/// <summary>
		/// Retrieves whether a result set's own deletes are visible.
		/// </summary>
		/// <param name="type"> the <code>ResultSet</code> type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <returns> <code>true</code> if deletes are visible for the given result set type;
		///        <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean ownDeletesAreVisible(int type) throws SQLException;
		bool OwnDeletesAreVisible(int type);

		/// <summary>
		/// Retrieves whether a result set's own inserts are visible.
		/// </summary>
		/// <param name="type"> the <code>ResultSet</code> type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <returns> <code>true</code> if inserts are visible for the given result set type;
		///        <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean ownInsertsAreVisible(int type) throws SQLException;
		bool OwnInsertsAreVisible(int type);

		/// <summary>
		/// Retrieves whether updates made by others are visible.
		/// </summary>
		/// <param name="type"> the <code>ResultSet</code> type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <returns> <code>true</code> if updates made by others
		///        are visible for the given result set type;
		///        <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean othersUpdatesAreVisible(int type) throws SQLException;
		bool OthersUpdatesAreVisible(int type);

		/// <summary>
		/// Retrieves whether deletes made by others are visible.
		/// </summary>
		/// <param name="type"> the <code>ResultSet</code> type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <returns> <code>true</code> if deletes made by others
		///        are visible for the given result set type;
		///        <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean othersDeletesAreVisible(int type) throws SQLException;
		bool OthersDeletesAreVisible(int type);

		/// <summary>
		/// Retrieves whether inserts made by others are visible.
		/// </summary>
		/// <param name="type"> the <code>ResultSet</code> type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <returns> <code>true</code> if inserts made by others
		///         are visible for the given result set type;
		///         <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean othersInsertsAreVisible(int type) throws SQLException;
		bool OthersInsertsAreVisible(int type);

		/// <summary>
		/// Retrieves whether or not a visible row update can be detected by
		/// calling the method <code>ResultSet.rowUpdated</code>.
		/// </summary>
		/// <param name="type"> the <code>ResultSet</code> type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <returns> <code>true</code> if changes are detected by the result set type;
		///         <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean updatesAreDetected(int type) throws SQLException;
		bool UpdatesAreDetected(int type);

		/// <summary>
		/// Retrieves whether or not a visible row delete can be detected by
		/// calling the method <code>ResultSet.rowDeleted</code>.  If the method
		/// <code>deletesAreDetected</code> returns <code>false</code>, it means that
		/// deleted rows are removed from the result set.
		/// </summary>
		/// <param name="type"> the <code>ResultSet</code> type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <returns> <code>true</code> if deletes are detected by the given result set type;
		///         <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean deletesAreDetected(int type) throws SQLException;
		bool DeletesAreDetected(int type);

		/// <summary>
		/// Retrieves whether or not a visible row insert can be detected
		/// by calling the method <code>ResultSet.rowInserted</code>.
		/// </summary>
		/// <param name="type"> the <code>ResultSet</code> type; one of
		///        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		///        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		///        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		/// <returns> <code>true</code> if changes are detected by the specified result
		///         set type; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean insertsAreDetected(int type) throws SQLException;
		bool InsertsAreDetected(int type);

		/// <summary>
		/// Retrieves whether this database supports batch updates.
		/// </summary>
		/// <returns> <code>true</code> if this database supports batch updates;
		///         <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsBatchUpdates() throws SQLException;
		bool SupportsBatchUpdates();

		/// <summary>
		/// Retrieves a description of the user-defined types (UDTs) defined
		/// in a particular schema.  Schema-specific UDTs may have type
		/// <code>JAVA_OBJECT</code>, <code>STRUCT</code>,
		/// or <code>DISTINCT</code>.
		/// 
		/// <P>Only types matching the catalog, schema, type name and type
		/// criteria are returned.  They are ordered by <code>DATA_TYPE</code>,
		/// <code>TYPE_CAT</code>, <code>TYPE_SCHEM</code>  and
		/// <code>TYPE_NAME</code>.  The type name parameter may be a fully-qualified
		/// name.  In this case, the catalog and schemaPattern parameters are
		/// ignored.
		/// 
		/// <P>Each type description has the following columns:
		///  <OL>
		///  <LI><B>TYPE_CAT</B> String {@code =>} the type's catalog (may be <code>null</code>)
		///  <LI><B>TYPE_SCHEM</B> String {@code =>} type's schema (may be <code>null</code>)
		///  <LI><B>TYPE_NAME</B> String {@code =>} type name
		///  <LI><B>CLASS_NAME</B> String {@code =>} Java class name
		///  <LI><B>DATA_TYPE</B> int {@code =>} type value defined in java.sql.Types.
		///     One of JAVA_OBJECT, STRUCT, or DISTINCT
		///  <LI><B>REMARKS</B> String {@code =>} explanatory comment on the type
		///  <LI><B>BASE_TYPE</B> short {@code =>} type code of the source type of a
		///     DISTINCT type or the type that implements the user-generated
		///     reference type of the SELF_REFERENCING_COLUMN of a structured
		///     type as defined in java.sql.Types (<code>null</code> if DATA_TYPE is not
		///     DISTINCT or not STRUCT with REFERENCE_GENERATION = USER_DEFINED)
		///  </OL>
		/// 
		/// <P><B>Note:</B> If the driver does not support UDTs, an empty
		/// result set is returned.
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema pattern name; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="typeNamePattern"> a type name pattern; must match the type name
		///        as it is stored in the database; may be a fully qualified name </param>
		/// <param name="types"> a list of user-defined types (JAVA_OBJECT,
		///        STRUCT, or DISTINCT) to include; <code>null</code> returns all types </param>
		/// <returns> <code>ResultSet</code> object in which each row describes a UDT </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getUDTs(String catalog, String schemaPattern, String typeNamePattern, int[] types) throws SQLException;
		ResultSet GetUDTs(String catalog, String schemaPattern, String typeNamePattern, int[] types);

		/// <summary>
		/// Retrieves the connection that produced this metadata object.
		/// <P> </summary>
		/// <returns> the connection that produced this metadata object </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Connection getConnection() throws SQLException;
		Connection Connection {get;}

		// ------------------- JDBC 3.0 -------------------------

		/// <summary>
		/// Retrieves whether this database supports savepoints.
		/// </summary>
		/// <returns> <code>true</code> if savepoints are supported;
		///         <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsSavepoints() throws SQLException;
		bool SupportsSavepoints();

		/// <summary>
		/// Retrieves whether this database supports named parameters to callable
		/// statements.
		/// </summary>
		/// <returns> <code>true</code> if named parameters are supported;
		///         <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsNamedParameters() throws SQLException;
		bool SupportsNamedParameters();

		/// <summary>
		/// Retrieves whether it is possible to have multiple <code>ResultSet</code> objects
		/// returned from a <code>CallableStatement</code> object
		/// simultaneously.
		/// </summary>
		/// <returns> <code>true</code> if a <code>CallableStatement</code> object
		///         can return multiple <code>ResultSet</code> objects
		///         simultaneously; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a datanase access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsMultipleOpenResults() throws SQLException;
		bool SupportsMultipleOpenResults();

		/// <summary>
		/// Retrieves whether auto-generated keys can be retrieved after
		/// a statement has been executed
		/// </summary>
		/// <returns> <code>true</code> if auto-generated keys can be retrieved
		///         after a statement has executed; <code>false</code> otherwise
		/// <para>If <code>true</code> is returned, the JDBC driver must support the
		/// returning of auto-generated keys for at least SQL INSERT statements
		/// </para>
		/// <para>
		/// </para>
		/// </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsGetGeneratedKeys() throws SQLException;
		bool SupportsGetGeneratedKeys();

		/// <summary>
		/// Retrieves a description of the user-defined type (UDT) hierarchies defined in a
		/// particular schema in this database. Only the immediate super type/
		/// sub type relationship is modeled.
		/// <P>
		/// Only supertype information for UDTs matching the catalog,
		/// schema, and type name is returned. The type name parameter
		/// may be a fully-qualified name. When the UDT name supplied is a
		/// fully-qualified name, the catalog and schemaPattern parameters are
		/// ignored.
		/// <P>
		/// If a UDT does not have a direct super type, it is not listed here.
		/// A row of the <code>ResultSet</code> object returned by this method
		/// describes the designated UDT and a direct supertype. A row has the following
		/// columns:
		///  <OL>
		///  <LI><B>TYPE_CAT</B> String {@code =>} the UDT's catalog (may be <code>null</code>)
		///  <LI><B>TYPE_SCHEM</B> String {@code =>} UDT's schema (may be <code>null</code>)
		///  <LI><B>TYPE_NAME</B> String {@code =>} type name of the UDT
		///  <LI><B>SUPERTYPE_CAT</B> String {@code =>} the direct super type's catalog
		///                           (may be <code>null</code>)
		///  <LI><B>SUPERTYPE_SCHEM</B> String {@code =>} the direct super type's schema
		///                             (may be <code>null</code>)
		///  <LI><B>SUPERTYPE_NAME</B> String {@code =>} the direct super type's name
		///  </OL>
		/// 
		/// <P><B>Note:</B> If the driver does not support type hierarchies, an
		/// empty result set is returned.
		/// </summary>
		/// <param name="catalog"> a catalog name; "" retrieves those without a catalog;
		///        <code>null</code> means drop catalog name from the selection criteria </param>
		/// <param name="schemaPattern"> a schema name pattern; "" retrieves those
		///        without a schema </param>
		/// <param name="typeNamePattern"> a UDT name pattern; may be a fully-qualified
		///        name </param>
		/// <returns> a <code>ResultSet</code> object in which a row gives information
		///         about the designated UDT </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getSuperTypes(String catalog, String schemaPattern, String typeNamePattern) throws SQLException;
		ResultSet GetSuperTypes(String catalog, String schemaPattern, String typeNamePattern);

		/// <summary>
		/// Retrieves a description of the table hierarchies defined in a particular
		/// schema in this database.
		/// 
		/// <P>Only supertable information for tables matching the catalog, schema
		/// and table name are returned. The table name parameter may be a fully-
		/// qualified name, in which case, the catalog and schemaPattern parameters
		/// are ignored. If a table does not have a super table, it is not listed here.
		/// Supertables have to be defined in the same catalog and schema as the
		/// sub tables. Therefore, the type description does not need to include
		/// this information for the supertable.
		/// 
		/// <P>Each type description has the following columns:
		///  <OL>
		///  <LI><B>TABLE_CAT</B> String {@code =>} the type's catalog (may be <code>null</code>)
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} type's schema (may be <code>null</code>)
		///  <LI><B>TABLE_NAME</B> String {@code =>} type name
		///  <LI><B>SUPERTABLE_NAME</B> String {@code =>} the direct super type's name
		///  </OL>
		/// 
		/// <P><B>Note:</B> If the driver does not support type hierarchies, an
		/// empty result set is returned.
		/// </summary>
		/// <param name="catalog"> a catalog name; "" retrieves those without a catalog;
		///        <code>null</code> means drop catalog name from the selection criteria </param>
		/// <param name="schemaPattern"> a schema name pattern; "" retrieves those
		///        without a schema </param>
		/// <param name="tableNamePattern"> a table name pattern; may be a fully-qualified
		///        name </param>
		/// <returns> a <code>ResultSet</code> object in which each row is a type description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getSuperTables(String catalog, String schemaPattern, String tableNamePattern) throws SQLException;
		ResultSet GetSuperTables(String catalog, String schemaPattern, String tableNamePattern);

		/// <summary>
		/// Indicates that <code>NULL</code> values might not be allowed.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code> in the <code>ResultSet</code> object
		/// returned by the method <code>getAttributes</code>.
		/// </summary>

		/// <summary>
		/// Indicates that <code>NULL</code> values are definitely allowed.
		/// <P>
		/// A possible value for the column <code>NULLABLE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getAttributes</code>.
		/// </summary>

		/// <summary>
		/// Indicates that whether <code>NULL</code> values are allowed is not
		/// known.
		/// <P>
		/// A possible value for the column <code>NULLABLE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getAttributes</code>.
		/// </summary>

		/// <summary>
		/// Retrieves a description of the given attribute of the given type
		/// for a user-defined type (UDT) that is available in the given schema
		/// and catalog.
		/// <P>
		/// Descriptions are returned only for attributes of UDTs matching the
		/// catalog, schema, type, and attribute name criteria. They are ordered by
		/// <code>TYPE_CAT</code>, <code>TYPE_SCHEM</code>,
		/// <code>TYPE_NAME</code> and <code>ORDINAL_POSITION</code>. This description
		/// does not contain inherited attributes.
		/// <P>
		/// The <code>ResultSet</code> object that is returned has the following
		/// columns:
		/// <OL>
		///  <LI><B>TYPE_CAT</B> String {@code =>} type catalog (may be <code>null</code>)
		///  <LI><B>TYPE_SCHEM</B> String {@code =>} type schema (may be <code>null</code>)
		///  <LI><B>TYPE_NAME</B> String {@code =>} type name
		///  <LI><B>ATTR_NAME</B> String {@code =>} attribute name
		///  <LI><B>DATA_TYPE</B> int {@code =>} attribute type SQL type from java.sql.Types
		///  <LI><B>ATTR_TYPE_NAME</B> String {@code =>} Data source dependent type name.
		///  For a UDT, the type name is fully qualified. For a REF, the type name is
		///  fully qualified and represents the target type of the reference type.
		///  <LI><B>ATTR_SIZE</B> int {@code =>} column size.  For char or date
		///      types this is the maximum number of characters; for numeric or
		///      decimal types this is precision.
		///  <LI><B>DECIMAL_DIGITS</B> int {@code =>} the number of fractional digits. Null is returned for data types where
		/// DECIMAL_DIGITS is not applicable.
		///  <LI><B>NUM_PREC_RADIX</B> int {@code =>} Radix (typically either 10 or 2)
		///  <LI><B>NULLABLE</B> int {@code =>} whether NULL is allowed
		///      <UL>
		///      <LI> attributeNoNulls - might not allow NULL values
		///      <LI> attributeNullable - definitely allows NULL values
		///      <LI> attributeNullableUnknown - nullability unknown
		///      </UL>
		///  <LI><B>REMARKS</B> String {@code =>} comment describing column (may be <code>null</code>)
		///  <LI><B>ATTR_DEF</B> String {@code =>} default value (may be <code>null</code>)
		///  <LI><B>SQL_DATA_TYPE</B> int {@code =>} unused
		///  <LI><B>SQL_DATETIME_SUB</B> int {@code =>} unused
		///  <LI><B>CHAR_OCTET_LENGTH</B> int {@code =>} for char types the
		///       maximum number of bytes in the column
		///  <LI><B>ORDINAL_POSITION</B> int {@code =>} index of the attribute in the UDT
		///      (starting at 1)
		///  <LI><B>IS_NULLABLE</B> String  {@code =>} ISO rules are used to determine
		/// the nullability for a attribute.
		///       <UL>
		///       <LI> YES           --- if the attribute can include NULLs
		///       <LI> NO            --- if the attribute cannot include NULLs
		///       <LI> empty string  --- if the nullability for the
		/// attribute is unknown
		///       </UL>
		///  <LI><B>SCOPE_CATALOG</B> String {@code =>} catalog of table that is the
		///      scope of a reference attribute (<code>null</code> if DATA_TYPE isn't REF)
		///  <LI><B>SCOPE_SCHEMA</B> String {@code =>} schema of table that is the
		///      scope of a reference attribute (<code>null</code> if DATA_TYPE isn't REF)
		///  <LI><B>SCOPE_TABLE</B> String {@code =>} table name that is the scope of a
		///      reference attribute (<code>null</code> if the DATA_TYPE isn't REF)
		/// <LI><B>SOURCE_DATA_TYPE</B> short {@code =>} source type of a distinct type or user-generated
		///      Ref type,SQL type from java.sql.Types (<code>null</code> if DATA_TYPE
		///      isn't DISTINCT or user-generated REF)
		///  </OL> </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="typeNamePattern"> a type name pattern; must match the
		///        type name as it is stored in the database </param>
		/// <param name="attributeNamePattern"> an attribute name pattern; must match the attribute
		///        name as it is declared in the database </param>
		/// <returns> a <code>ResultSet</code> object in which each row is an
		///         attribute description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getAttributes(String catalog, String schemaPattern, String typeNamePattern, String attributeNamePattern) throws SQLException;
		ResultSet GetAttributes(String catalog, String schemaPattern, String typeNamePattern, String attributeNamePattern);

		/// <summary>
		/// Retrieves whether this database supports the given result set holdability.
		/// </summary>
		/// <param name="holdability"> one of the following constants:
		///          <code>ResultSet.HOLD_CURSORS_OVER_COMMIT</code> or
		///          <code>ResultSet.CLOSE_CURSORS_AT_COMMIT</code> </param>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= Connection
		/// @since 1.4 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsResultSetHoldability(int holdability) throws SQLException;
		bool SupportsResultSetHoldability(int holdability);

		/// <summary>
		/// Retrieves this database's default holdability for <code>ResultSet</code>
		/// objects.
		/// </summary>
		/// <returns> the default holdability; either
		///         <code>ResultSet.HOLD_CURSORS_OVER_COMMIT</code> or
		///         <code>ResultSet.CLOSE_CURSORS_AT_COMMIT</code> </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getResultSetHoldability() throws SQLException;
		int ResultSetHoldability {get;}

		/// <summary>
		/// Retrieves the major version number of the underlying database.
		/// </summary>
		/// <returns> the underlying database's major version </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getDatabaseMajorVersion() throws SQLException;
		int DatabaseMajorVersion {get;}

		/// <summary>
		/// Retrieves the minor version number of the underlying database.
		/// </summary>
		/// <returns> underlying database's minor version </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getDatabaseMinorVersion() throws SQLException;
		int DatabaseMinorVersion {get;}

		/// <summary>
		/// Retrieves the major JDBC version number for this
		/// driver.
		/// </summary>
		/// <returns> JDBC version major number </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getJDBCMajorVersion() throws SQLException;
		int JDBCMajorVersion {get;}

		/// <summary>
		/// Retrieves the minor JDBC version number for this
		/// driver.
		/// </summary>
		/// <returns> JDBC version minor number </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getJDBCMinorVersion() throws SQLException;
		int JDBCMinorVersion {get;}

		/// <summary>
		///  A possible return value for the method
		/// <code>DatabaseMetaData.getSQLStateType</code> which is used to indicate
		/// whether the value returned by the method
		/// <code>SQLException.getSQLState</code> is an
		/// X/Open (now know as Open Group) SQL CLI SQLSTATE value.
		/// <P>
		/// @since 1.4
		/// </summary>

		/// <summary>
		///  A possible return value for the method
		/// <code>DatabaseMetaData.getSQLStateType</code> which is used to indicate
		/// whether the value returned by the method
		/// <code>SQLException.getSQLState</code> is an SQLSTATE value.
		/// <P>
		/// @since 1.6
		/// </summary>

		 /// <summary>
		 ///  A possible return value for the method
		 /// <code>DatabaseMetaData.getSQLStateType</code> which is used to indicate
		 /// whether the value returned by the method
		 /// <code>SQLException.getSQLState</code> is an SQL99 SQLSTATE value.
		 /// <P>
		 /// <b>Note:</b>This constant remains only for compatibility reasons. Developers
		 /// should use the constant <code>sqlStateSQL</code> instead.
		 /// 
		 /// @since 1.4
		 /// </summary>

		/// <summary>
		/// Indicates whether the SQLSTATE returned by <code>SQLException.getSQLState</code>
		/// is X/Open (now known as Open Group) SQL CLI or SQL:2003. </summary>
		/// <returns> the type of SQLSTATE; one of:
		///        sqlStateXOpen or
		///        sqlStateSQL </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getSQLStateType() throws SQLException;
		int SQLStateType {get;}

		/// <summary>
		/// Indicates whether updates made to a LOB are made on a copy or directly
		/// to the LOB. </summary>
		/// <returns> <code>true</code> if updates are made to a copy of the LOB;
		///         <code>false</code> if updates are made directly to the LOB </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean locatorsUpdateCopy() throws SQLException;
		bool LocatorsUpdateCopy();

		/// <summary>
		/// Retrieves whether this database supports statement pooling.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsStatementPooling() throws SQLException;
		bool SupportsStatementPooling();

		//------------------------- JDBC 4.0 -----------------------------------

		/// <summary>
		/// Indicates whether or not this data source supports the SQL <code>ROWID</code> type,
		/// and if so  the lifetime for which a <code>RowId</code> object remains valid.
		/// <para>
		/// The returned int values have the following relationship:
		/// <pre>{@code
		///     ROWID_UNSUPPORTED < ROWID_VALID_OTHER < ROWID_VALID_TRANSACTION
		///         < ROWID_VALID_SESSION < ROWID_VALID_FOREVER
		/// }</pre>
		/// so conditional logic such as
		/// <pre>{@code
		///     if (metadata.getRowIdLifetime() > DatabaseMetaData.ROWID_VALID_TRANSACTION)
		/// }</pre>
		/// can be used. Valid Forever means valid across all Sessions, and valid for
		/// a Session means valid across all its contained Transactions.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the status indicating the lifetime of a <code>RowId</code> </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RowIdLifetime getRowIdLifetime() throws SQLException;
		RowIdLifetime RowIdLifetime {get;}

		/// <summary>
		/// Retrieves the schema names available in this database.  The results
		/// are ordered by <code>TABLE_CATALOG</code> and
		/// <code>TABLE_SCHEM</code>.
		/// 
		/// <P>The schema columns are:
		///  <OL>
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} schema name
		///  <LI><B>TABLE_CATALOG</B> String {@code =>} catalog name (may be <code>null</code>)
		///  </OL>
		/// 
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it is stored
		/// in the database;"" retrieves those without a catalog; null means catalog
		/// name should not be used to narrow down the search. </param>
		/// <param name="schemaPattern"> a schema name; must match the schema name as it is
		/// stored in the database; null means
		/// schema name should not be used to narrow down the search. </param>
		/// <returns> a <code>ResultSet</code> object in which each row is a
		///         schema description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape
		/// @since 1.6 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getSchemas(String catalog, String schemaPattern) throws SQLException;
		ResultSet GetSchemas(String catalog, String schemaPattern);

		/// <summary>
		/// Retrieves whether this database supports invoking user-defined or vendor functions
		/// using the stored procedure escape syntax.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean supportsStoredFunctionsUsingCallSyntax() throws SQLException;
		bool SupportsStoredFunctionsUsingCallSyntax();

		/// <summary>
		/// Retrieves whether a <code>SQLException</code> while autoCommit is <code>true</code> indicates
		/// that all open ResultSets are closed, even ones that are holdable.  When a <code>SQLException</code> occurs while
		/// autocommit is <code>true</code>, it is vendor specific whether the JDBC driver responds with a commit operation, a
		/// rollback operation, or by doing neither a commit nor a rollback.  A potential result of this difference
		/// is in whether or not holdable ResultSets are closed.
		/// </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean autoCommitFailureClosesAllResultSets() throws SQLException;
		bool AutoCommitFailureClosesAllResultSets();
			/// <summary>
			/// Retrieves a list of the client info properties
			/// that the driver supports.  The result set contains the following columns
			/// 
			/// <ol>
			/// <li><b>NAME</b> String{@code =>} The name of the client info property<br>
			/// <li><b>MAX_LEN</b> int{@code =>} The maximum length of the value for the property<br>
			/// <li><b>DEFAULT_VALUE</b> String{@code =>} The default value of the property<br>
			/// <li><b>DESCRIPTION</b> String{@code =>} A description of the property.  This will typically
			///                                              contain information as to where this property is
			///                                              stored in the database.
			/// </ol>
			/// <para>
			/// The <code>ResultSet</code> is sorted by the NAME column
			/// </para>
			/// <para>
			/// </para>
			/// </summary>
			/// <returns>      A <code>ResultSet</code> object; each row is a supported client info
			/// property
			/// <para>
			/// </para>
			/// </returns>
			///  <exception cref="SQLException"> if a database access error occurs
			/// <para>
			/// @since 1.6 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getClientInfoProperties() throws SQLException;
			ResultSet ClientInfoProperties {get;}

		/// <summary>
		/// Retrieves a description of the  system and user functions available
		/// in the given catalog.
		/// <P>
		/// Only system and user function descriptions matching the schema and
		/// function name criteria are returned.  They are ordered by
		/// <code>FUNCTION_CAT</code>, <code>FUNCTION_SCHEM</code>,
		/// <code>FUNCTION_NAME</code> and
		/// <code>SPECIFIC_ NAME</code>.
		/// 
		/// <P>Each function description has the the following columns:
		///  <OL>
		///  <LI><B>FUNCTION_CAT</B> String {@code =>} function catalog (may be <code>null</code>)
		///  <LI><B>FUNCTION_SCHEM</B> String {@code =>} function schema (may be <code>null</code>)
		///  <LI><B>FUNCTION_NAME</B> String {@code =>} function name.  This is the name
		/// used to invoke the function
		///  <LI><B>REMARKS</B> String {@code =>} explanatory comment on the function
		/// <LI><B>FUNCTION_TYPE</B> short {@code =>} kind of function:
		///      <UL>
		///      <LI>functionResultUnknown - Cannot determine if a return value
		///       or table will be returned
		///      <LI> functionNoTable- Does not return a table
		///      <LI> functionReturnsTable - Returns a table
		///      </UL>
		///  <LI><B>SPECIFIC_NAME</B> String  {@code =>} the name which uniquely identifies
		///  this function within its schema.  This is a user specified, or DBMS
		/// generated, name that may be different then the <code>FUNCTION_NAME</code>
		/// for example with overload functions
		///  </OL>
		/// <para>
		/// A user may not have permission to execute any of the functions that are
		/// returned by <code>getFunctions</code>
		/// 
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="functionNamePattern"> a function name pattern; must match the
		///        function name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a function description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape
		/// @since 1.6 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getFunctions(String catalog, String schemaPattern, String functionNamePattern) throws SQLException;
		ResultSet GetFunctions(String catalog, String schemaPattern, String functionNamePattern);
		/// <summary>
		/// Retrieves a description of the given catalog's system or user
		/// function parameters and return type.
		/// 
		/// <P>Only descriptions matching the schema,  function and
		/// parameter name criteria are returned. They are ordered by
		/// <code>FUNCTION_CAT</code>, <code>FUNCTION_SCHEM</code>,
		/// <code>FUNCTION_NAME</code> and
		/// <code>SPECIFIC_ NAME</code>. Within this, the return value,
		/// if any, is first. Next are the parameter descriptions in call
		/// order. The column descriptions follow in column number order.
		/// 
		/// <P>Each row in the <code>ResultSet</code>
		/// is a parameter description, column description or
		/// return type description with the following fields:
		///  <OL>
		///  <LI><B>FUNCTION_CAT</B> String {@code =>} function catalog (may be <code>null</code>)
		///  <LI><B>FUNCTION_SCHEM</B> String {@code =>} function schema (may be <code>null</code>)
		///  <LI><B>FUNCTION_NAME</B> String {@code =>} function name.  This is the name
		/// used to invoke the function
		///  <LI><B>COLUMN_NAME</B> String {@code =>} column/parameter name
		///  <LI><B>COLUMN_TYPE</B> Short {@code =>} kind of column/parameter:
		///      <UL>
		///      <LI> functionColumnUnknown - nobody knows
		///      <LI> functionColumnIn - IN parameter
		///      <LI> functionColumnInOut - INOUT parameter
		///      <LI> functionColumnOut - OUT parameter
		///      <LI> functionColumnReturn - function return value
		///      <LI> functionColumnResult - Indicates that the parameter or column
		///  is a column in the <code>ResultSet</code>
		///      </UL>
		///  <LI><B>DATA_TYPE</B> int {@code =>} SQL type from java.sql.Types
		///  <LI><B>TYPE_NAME</B> String {@code =>} SQL type name, for a UDT type the
		///  type name is fully qualified
		///  <LI><B>PRECISION</B> int {@code =>} precision
		///  <LI><B>LENGTH</B> int {@code =>} length in bytes of data
		///  <LI><B>SCALE</B> short {@code =>} scale -  null is returned for data types where
		/// SCALE is not applicable.
		///  <LI><B>RADIX</B> short {@code =>} radix
		///  <LI><B>NULLABLE</B> short {@code =>} can it contain NULL.
		///      <UL>
		///      <LI> functionNoNulls - does not allow NULL values
		///      <LI> functionNullable - allows NULL values
		///      <LI> functionNullableUnknown - nullability unknown
		///      </UL>
		///  <LI><B>REMARKS</B> String {@code =>} comment describing column/parameter
		///  <LI><B>CHAR_OCTET_LENGTH</B> int  {@code =>} the maximum length of binary
		/// and character based parameters or columns.  For any other datatype the returned value
		/// is a NULL
		///  <LI><B>ORDINAL_POSITION</B> int  {@code =>} the ordinal position, starting
		/// from 1, for the input and output parameters. A value of 0
		/// is returned if this row describes the function's return value.
		/// For result set columns, it is the
		/// ordinal position of the column in the result set starting from 1.
		///  <LI><B>IS_NULLABLE</B> String  {@code =>} ISO rules are used to determine
		/// the nullability for a parameter or column.
		///       <UL>
		///       <LI> YES           --- if the parameter or column can include NULLs
		///       <LI> NO            --- if the parameter or column  cannot include NULLs
		///       <LI> empty string  --- if the nullability for the
		/// parameter  or column is unknown
		///       </UL>
		///  <LI><B>SPECIFIC_NAME</B> String  {@code =>} the name which uniquely identifies
		/// this function within its schema.  This is a user specified, or DBMS
		/// generated, name that may be different then the <code>FUNCTION_NAME</code>
		/// for example with overload functions
		///  </OL>
		/// 
		/// <para>The PRECISION column represents the specified column size for the given
		/// parameter or column.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. Null is returned for data types where the
		/// column size is not applicable.
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="functionNamePattern"> a procedure name pattern; must match the
		///        function name as it is stored in the database </param>
		/// <param name="columnNamePattern"> a parameter name pattern; must match the
		/// parameter or column name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row describes a
		/// user function parameter, column  or return type
		/// </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= #getSearchStringEscape
		/// @since 1.6 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getFunctionColumns(String catalog, String schemaPattern, String functionNamePattern, String columnNamePattern) throws SQLException;
		ResultSet GetFunctionColumns(String catalog, String schemaPattern, String functionNamePattern, String columnNamePattern);


		/// <summary>
		/// Indicates that type of the parameter or column is unknown.
		/// <P>
		/// A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getFunctionColumns</code>.
		/// </summary>

		/// <summary>
		/// Indicates that the parameter or column is an IN parameter.
		/// <P>
		///  A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getFunctionColumns</code>.
		/// @since 1.6
		/// </summary>

		/// <summary>
		/// Indicates that the parameter or column is an INOUT parameter.
		/// <P>
		/// A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getFunctionColumns</code>.
		/// @since 1.6
		/// </summary>

		/// <summary>
		/// Indicates that the parameter or column is an OUT parameter.
		/// <P>
		/// A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getFunctionColumns</code>.
		/// @since 1.6
		/// </summary>
		/// <summary>
		/// Indicates that the parameter or column is a return value.
		/// <P>
		///  A possible value for the column
		/// <code>COLUMN_TYPE</code>
		/// in the <code>ResultSet</code>
		/// returned by the method <code>getFunctionColumns</code>.
		/// @since 1.6
		/// </summary>

		   /// <summary>
		   /// Indicates that the parameter or column is a column in a result set.
		   /// <P>
		   ///  A possible value for the column
		   /// <code>COLUMN_TYPE</code>
		   /// in the <code>ResultSet</code>
		   /// returned by the method <code>getFunctionColumns</code>.
		   /// @since 1.6
		   /// </summary>


		/// <summary>
		/// Indicates that <code>NULL</code> values are not allowed.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getFunctionColumns</code>.
		/// @since 1.6
		/// </summary>

		/// <summary>
		/// Indicates that <code>NULL</code> values are allowed.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getFunctionColumns</code>.
		/// @since 1.6
		/// </summary>

		/// <summary>
		/// Indicates that whether <code>NULL</code> values are allowed
		/// is unknown.
		/// <P>
		/// A possible value for the column
		/// <code>NULLABLE</code>
		/// in the <code>ResultSet</code> object
		/// returned by the method <code>getFunctionColumns</code>.
		/// @since 1.6
		/// </summary>

		/// <summary>
		/// Indicates that it is not known whether the function returns
		/// a result or a table.
		/// <P>
		/// A possible value for column <code>FUNCTION_TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getFunctions</code>.
		/// @since 1.6
		/// </summary>

		/// <summary>
		/// Indicates that the function  does not return a table.
		/// <P>
		/// A possible value for column <code>FUNCTION_TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getFunctions</code>.
		/// @since 1.6
		/// </summary>

		/// <summary>
		/// Indicates that the function  returns a table.
		/// <P>
		/// A possible value for column <code>FUNCTION_TYPE</code> in the
		/// <code>ResultSet</code> object returned by the method
		/// <code>getFunctions</code>.
		/// @since 1.6
		/// </summary>

		//--------------------------JDBC 4.1 -----------------------------

		/// <summary>
		/// Retrieves a description of the pseudo or hidden columns available
		/// in a given table within the specified catalog and schema.
		/// Pseudo or hidden columns may not always be stored within
		/// a table and are not visible in a ResultSet unless they are
		/// specified in the query's outermost SELECT list. Pseudo or hidden
		/// columns may not necessarily be able to be modified. If there are
		/// no pseudo or hidden columns, an empty ResultSet is returned.
		/// 
		/// <P>Only column descriptions matching the catalog, schema, table
		/// and column name criteria are returned.  They are ordered by
		/// <code>TABLE_CAT</code>,<code>TABLE_SCHEM</code>, <code>TABLE_NAME</code>
		/// and <code>COLUMN_NAME</code>.
		/// 
		/// <P>Each column description has the following columns:
		///  <OL>
		///  <LI><B>TABLE_CAT</B> String {@code =>} table catalog (may be <code>null</code>)
		///  <LI><B>TABLE_SCHEM</B> String {@code =>} table schema (may be <code>null</code>)
		///  <LI><B>TABLE_NAME</B> String {@code =>} table name
		///  <LI><B>COLUMN_NAME</B> String {@code =>} column name
		///  <LI><B>DATA_TYPE</B> int {@code =>} SQL type from java.sql.Types
		///  <LI><B>COLUMN_SIZE</B> int {@code =>} column size.
		///  <LI><B>DECIMAL_DIGITS</B> int {@code =>} the number of fractional digits. Null is returned for data types where
		/// DECIMAL_DIGITS is not applicable.
		///  <LI><B>NUM_PREC_RADIX</B> int {@code =>} Radix (typically either 10 or 2)
		///  <LI><B>COLUMN_USAGE</B> String {@code =>} The allowed usage for the column.  The
		///  value returned will correspond to the enum name returned by <seealso cref="PseudoColumnUsage#name PseudoColumnUsage.name()"/>
		///  <LI><B>REMARKS</B> String {@code =>} comment describing column (may be <code>null</code>)
		///  <LI><B>CHAR_OCTET_LENGTH</B> int {@code =>} for char types the
		///       maximum number of bytes in the column
		///  <LI><B>IS_NULLABLE</B> String  {@code =>} ISO rules are used to determine the nullability for a column.
		///       <UL>
		///       <LI> YES           --- if the column can include NULLs
		///       <LI> NO            --- if the column cannot include NULLs
		///       <LI> empty string  --- if the nullability for the column is unknown
		///       </UL>
		///  </OL>
		/// 
		/// <para>The COLUMN_SIZE column specifies the column size for the given column.
		/// For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		/// For datetime datatypes, this is the length in characters of the String representation (assuming the
		/// maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		/// this is the length in bytes. Null is returned for data types where the
		/// column size is not applicable.
		/// 
		/// </para>
		/// </summary>
		/// <param name="catalog"> a catalog name; must match the catalog name as it
		///        is stored in the database; "" retrieves those without a catalog;
		///        <code>null</code> means that the catalog name should not be used to narrow
		///        the search </param>
		/// <param name="schemaPattern"> a schema name pattern; must match the schema name
		///        as it is stored in the database; "" retrieves those without a schema;
		///        <code>null</code> means that the schema name should not be used to narrow
		///        the search </param>
		/// <param name="tableNamePattern"> a table name pattern; must match the
		///        table name as it is stored in the database </param>
		/// <param name="columnNamePattern"> a column name pattern; must match the column
		///        name as it is stored in the database </param>
		/// <returns> <code>ResultSet</code> - each row is a column description </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <seealso cref= PseudoColumnUsage
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ResultSet getPseudoColumns(String catalog, String schemaPattern, String tableNamePattern, String columnNamePattern) throws SQLException;
		ResultSet GetPseudoColumns(String catalog, String schemaPattern, String tableNamePattern, String columnNamePattern);

		/// <summary>
		/// Retrieves whether a generated key will always be returned if the column
		/// name(s) or index(es) specified for the auto generated key column(s)
		/// are valid and the statement succeeds.  The key that is returned may or
		/// may not be based on the column(s) for the auto generated key.
		/// Consult your JDBC driver documentation for additional details. </summary>
		/// <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean generatedKeyAlwaysReturned() throws SQLException;
		bool GeneratedKeyAlwaysReturned();

		//--------------------------JDBC 4.2 -----------------------------

		/// 
		/// <summary>
		/// Retrieves the maximum number of bytes this database allows for
		/// the logical size for a {@code LOB}.
		/// <para>
		/// The default implementation will return {@code 0}
		/// 
		/// </para>
		/// </summary>
		/// <returns> the maximum number of bytes allowed; a result of zero
		/// means that there is no limit or the limit is not known </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default long getMaxLogicalLobSize() throws SQLException
	//	{
	//	}

		/// <summary>
		/// Retrieves whether this database supports REF CURSOR.
		/// <para>
		/// The default implementation will return {@code false}
		/// 
		/// </para>
		/// </summary>
		/// <returns> {@code true} if this database supports REF CURSOR;
		///         {@code false} otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default boolean supportsRefCursors() throws SQLException
	//	{
	//	}

	}

	public static class DatabaseMetaData_Fields
	{
		public const int ProcedureResultUnknown = 0;
		public const int ProcedureNoResult = 1;
		public const int ProcedureReturnsResult = 2;
		public const int ProcedureColumnUnknown = 0;
		public const int ProcedureColumnIn = 1;
		public const int ProcedureColumnInOut = 2;
		public const int ProcedureColumnOut = 4;
		public const int ProcedureColumnReturn = 5;
		public const int ProcedureColumnResult = 3;
		public const int ProcedureNoNulls = 0;
		public const int ProcedureNullable = 1;
		public const int ProcedureNullableUnknown = 2;
		public const int ColumnNoNulls = 0;
		public const int ColumnNullable = 1;
		public const int ColumnNullableUnknown = 2;
		public const int BestRowTemporary = 0;
		public const int BestRowTransaction = 1;
		public const int BestRowSession = 2;
		public const int BestRowUnknown = 0;
		public const int BestRowNotPseudo = 1;
		public const int BestRowPseudo = 2;
		public const int VersionColumnUnknown = 0;
		public const int VersionColumnNotPseudo = 1;
		public const int VersionColumnPseudo = 2;
		public const int ImportedKeyCascade = 0;
		public const int ImportedKeyRestrict = 1;
		public const int ImportedKeySetNull = 2;
		public const int ImportedKeyNoAction = 3;
		public const int ImportedKeySetDefault = 4;
		public const int ImportedKeyInitiallyDeferred = 5;
		public const int ImportedKeyInitiallyImmediate = 6;
		public const int ImportedKeyNotDeferrable = 7;
		public const int TypeNoNulls = 0;
		public const int TypeNullable = 1;
		public const int TypeNullableUnknown = 2;
		public const int TypePredNone = 0;
		public const int TypePredChar = 1;
		public const int TypePredBasic = 2;
		public const int TypeSearchable = 3;
		public const short TableIndexStatistic = 0;
		public const short TableIndexClustered = 1;
		public const short TableIndexHashed = 2;
		public const short TableIndexOther = 3;
		public const short AttributeNoNulls = 0;
		public const short AttributeNullable = 1;
		public const short AttributeNullableUnknown = 2;
		public const int SqlStateXOpen = 1;
		public const int SqlStateSQL = 2;
		public const int SqlStateSQL99 = SqlStateSQL;
		public const int FunctionColumnUnknown = 0;
		public const int FunctionColumnIn = 1;
		public const int FunctionColumnInOut = 2;
		public const int FunctionColumnOut = 3;
		public const int FunctionReturn = 4;
		public const int FunctionColumnResult = 5;
		public const int FunctionNoNulls = 0;
		public const int FunctionNullable = 1;
		public const int FunctionNullableUnknown = 2;
		public const int FunctionResultUnknown = 0;
		public const int FunctionNoTable = 1;
		public const int FunctionReturnsTable = 2;
			public static readonly return 0;
			public static readonly return False;
	}

}