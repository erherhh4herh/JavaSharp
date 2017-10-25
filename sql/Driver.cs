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
	/// The interface that every driver class must implement.
	/// <P>The Java SQL framework allows for multiple database drivers.
	/// 
	/// <P>Each driver should supply a class that implements
	/// the Driver interface.
	/// 
	/// <P>The DriverManager will try to load as many drivers as it can
	/// find and then for any given connection request, it will ask each
	/// driver in turn to try to connect to the target URL.
	/// 
	/// <P>It is strongly recommended that each Driver class should be
	/// small and standalone so that the Driver class can be loaded and
	/// queried without bringing in vast quantities of supporting code.
	/// 
	/// <P>When a Driver class is loaded, it should create an instance of
	/// itself and register it with the DriverManager. This means that a
	/// user can load and register a driver by calling:
	/// <para>
	/// {@code Class.forName("foo.bah.Driver")}
	/// </para>
	/// <para>
	/// A JDBC driver may create a <seealso cref="DriverAction"/> implementation in order
	/// to receive notifications when <seealso cref="DriverManager#deregisterDriver"/> has
	/// been called.
	/// </para>
	/// </summary>
	/// <seealso cref= DriverManager </seealso>
	/// <seealso cref= Connection </seealso>
	/// <seealso cref= DriverAction </seealso>
	public interface Driver
	{

		/// <summary>
		/// Attempts to make a database connection to the given URL.
		/// The driver should return "null" if it realizes it is the wrong kind
		/// of driver to connect to the given URL.  This will be common, as when
		/// the JDBC driver manager is asked to connect to a given URL it passes
		/// the URL to each loaded driver in turn.
		/// 
		/// <P>The driver should throw an <code>SQLException</code> if it is the right
		/// driver to connect to the given URL but has trouble connecting to
		/// the database.
		/// 
		/// <P>The {@code Properties} argument can be used to pass
		/// arbitrary string tag/value pairs as connection arguments.
		/// Normally at least "user" and "password" properties should be
		/// included in the {@code Properties} object.
		/// <para>
		/// <B>Note:</B> If a property is specified as part of the {@code url} and
		/// is also specified in the {@code Properties} object, it is
		/// implementation-defined as to which value will take precedence. For
		/// maximum portability, an application should only specify a property once.
		/// 
		/// </para>
		/// </summary>
		/// <param name="url"> the URL of the database to which to connect </param>
		/// <param name="info"> a list of arbitrary string tag/value pairs as
		/// connection arguments. Normally at least a "user" and
		/// "password" property should be included. </param>
		/// <returns> a <code>Connection</code> object that represents a
		///         connection to the URL </returns>
		/// <exception cref="SQLException"> if a database access error occurs or the url is
		/// {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Connection connect(String url, java.util.Properties info) throws SQLException;
		Connection Connect(String url, java.util.Properties info);

		/// <summary>
		/// Retrieves whether the driver thinks that it can open a connection
		/// to the given URL.  Typically drivers will return <code>true</code> if they
		/// understand the sub-protocol specified in the URL and <code>false</code> if
		/// they do not.
		/// </summary>
		/// <param name="url"> the URL of the database </param>
		/// <returns> <code>true</code> if this driver understands the given URL;
		///         <code>false</code> otherwise </returns>
		/// <exception cref="SQLException"> if a database access error occurs or the url is
		/// {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: boolean acceptsURL(String url) throws SQLException;
		bool AcceptsURL(String url);


		/// <summary>
		/// Gets information about the possible properties for this driver.
		/// <P>
		/// The <code>getPropertyInfo</code> method is intended to allow a generic
		/// GUI tool to discover what properties it should prompt
		/// a human for in order to get
		/// enough information to connect to a database.  Note that depending on
		/// the values the human has supplied so far, additional values may become
		/// necessary, so it may be necessary to iterate though several calls
		/// to the <code>getPropertyInfo</code> method.
		/// </summary>
		/// <param name="url"> the URL of the database to which to connect </param>
		/// <param name="info"> a proposed list of tag/value pairs that will be sent on
		///          connect open </param>
		/// <returns> an array of <code>DriverPropertyInfo</code> objects describing
		///          possible properties.  This array may be an empty array if
		///          no properties are required. </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: DriverPropertyInfo[] getPropertyInfo(String url, java.util.Properties info) throws SQLException;
		DriverPropertyInfo[] GetPropertyInfo(String url, java.util.Properties info);


		/// <summary>
		/// Retrieves the driver's major version number. Initially this should be 1.
		/// </summary>
		/// <returns> this driver's major version number </returns>
		int MajorVersion {get;}

		/// <summary>
		/// Gets the driver's minor version number. Initially this should be 0. </summary>
		/// <returns> this driver's minor version number </returns>
		int MinorVersion {get;}


		/// <summary>
		/// Reports whether this driver is a genuine JDBC
		/// Compliant&trade; driver.
		/// A driver may only report <code>true</code> here if it passes the JDBC
		/// compliance tests; otherwise it is required to return <code>false</code>.
		/// <P>
		/// JDBC compliance requires full support for the JDBC API and full support
		/// for SQL 92 Entry Level.  It is expected that JDBC compliant drivers will
		/// be available for all the major commercial databases.
		/// <P>
		/// This method is not intended to encourage the development of non-JDBC
		/// compliant drivers, but is a recognition of the fact that some vendors
		/// are interested in using the JDBC API and framework for lightweight
		/// databases that do not support full database functionality, or for
		/// special databases such as document information retrieval where a SQL
		/// implementation may not be feasible. </summary>
		/// <returns> <code>true</code> if this driver is JDBC Compliant; <code>false</code>
		///         otherwise </returns>
		bool JdbcCompliant();

		//------------------------- JDBC 4.1 -----------------------------------

		/// <summary>
		/// Return the parent Logger of all the Loggers used by this driver. This
		/// should be the Logger farthest from the root Logger that is
		/// still an ancestor of all of the Loggers used by this driver. Configuring
		/// this Logger will affect all of the log messages generated by the driver.
		/// In the worst case, this may be the root Logger.
		/// </summary>
		/// <returns> the parent Logger for this driver </returns>
		/// <exception cref="SQLFeatureNotSupportedException"> if the driver does not use
		/// {@code java.util.logging}.
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.logging.Logger getParentLogger() throws SQLFeatureNotSupportedException;
		Logger ParentLogger {get;}
	}

}