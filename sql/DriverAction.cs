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
	/// An interface that must be implemented when a <seealso cref="Driver"/> wants to be
	/// notified by {@code DriverManager}.
	/// <P>
	/// A {@code DriverAction} implementation is not intended to be used
	/// directly by applications. A JDBC Driver  may choose
	/// to create its {@code DriverAction} implementation in a private class
	/// to avoid it being called directly.
	/// <para>
	/// The JDBC driver's static initialization block must call
	/// <seealso cref="DriverManager#registerDriver(java.sql.Driver, java.sql.DriverAction) "/> in order
	/// to inform {@code DriverManager} which {@code DriverAction} implementation to
	/// call when the JDBC driver is de-registered.
	/// @since 1.8
	/// </para>
	/// </summary>
	public interface DriverAction
	{
		/// <summary>
		/// Method called by
		/// <seealso cref="DriverManager#deregisterDriver(Driver) "/>
		///  to notify the JDBC driver that it was de-registered.
		/// <para>
		/// The {@code deregister} method is intended only to be used by JDBC Drivers
		/// and not by applications.  JDBC drivers are recommended to not implement
		/// {@code DriverAction} in a public class.  If there are active
		/// connections to the database at the time that the {@code deregister}
		/// method is called, it is implementation specific as to whether the
		/// connections are closed or allowed to continue. Once this method is
		/// called, it is implementation specific as to whether the driver may
		/// limit the ability to create new connections to the database, invoke
		/// other {@code Driver} methods or throw a {@code SQLException}.
		/// Consult your JDBC driver's documentation for additional information
		/// on its behavior.
		/// </para>
		/// </summary>
		/// <seealso cref= DriverManager#registerDriver(java.sql.Driver, java.sql.DriverAction) </seealso>
		/// <seealso cref= DriverManager#deregisterDriver(Driver)
		/// @since 1.8 </seealso>
		void Deregister();

	}

}