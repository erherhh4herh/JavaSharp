using System;
using System.Collections.Generic;
using System.Threading;

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

	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;


	/// <summary>
	/// <P>The basic service for managing a set of JDBC drivers.<br>
	/// <B>NOTE:</B> The <seealso cref="javax.sql.DataSource"/> interface, new in the
	/// JDBC 2.0 API, provides another way to connect to a data source.
	/// The use of a <code>DataSource</code> object is the preferred means of
	/// connecting to a data source.
	/// 
	/// <P>As part of its initialization, the <code>DriverManager</code> class will
	/// attempt to load the driver classes referenced in the "jdbc.drivers"
	/// system property. This allows a user to customize the JDBC Drivers
	/// used by their applications. For example in your
	/// ~/.hotjava/properties file you might specify:
	/// <pre>
	/// <CODE>jdbc.drivers=foo.bah.Driver:wombat.sql.Driver:bad.taste.ourDriver</CODE>
	/// </pre>
	/// <P> The <code>DriverManager</code> methods <code>getConnection</code> and
	/// <code>getDrivers</code> have been enhanced to support the Java Standard Edition
	/// <a href="../../../technotes/guides/jar/jar.html#Service%20Provider">Service Provider</a> mechanism. JDBC 4.0 Drivers must
	/// include the file <code>META-INF/services/java.sql.Driver</code>. This file contains the name of the JDBC drivers
	/// implementation of <code>java.sql.Driver</code>.  For example, to load the <code>my.sql.Driver</code> class,
	/// the <code>META-INF/services/java.sql.Driver</code> file would contain the entry:
	/// <pre>
	/// <code>my.sql.Driver</code>
	/// </pre>
	/// 
	/// <P>Applications no longer need to explicitly load JDBC drivers using <code>Class.forName()</code>. Existing programs
	/// which currently load JDBC drivers using <code>Class.forName()</code> will continue to work without
	/// modification.
	/// 
	/// <P>When the method <code>getConnection</code> is called,
	/// the <code>DriverManager</code> will attempt to
	/// locate a suitable driver from amongst those loaded at
	/// initialization and those loaded explicitly using the same classloader
	/// as the current applet or application.
	/// 
	/// <P>
	/// Starting with the Java 2 SDK, Standard Edition, version 1.3, a
	/// logging stream can be set only if the proper
	/// permission has been granted.  Normally this will be done with
	/// the tool PolicyTool, which can be used to grant <code>permission
	/// java.sql.SQLPermission "setLog"</code>. </summary>
	/// <seealso cref= Driver </seealso>
	/// <seealso cref= Connection </seealso>
	public class DriverManager
	{


		// List of registered JDBC drivers
		private static readonly CopyOnWriteArrayList<DriverInfo> RegisteredDrivers = new CopyOnWriteArrayList<DriverInfo>();
		private static volatile int LoginTimeout_Renamed = 0;
		private static volatile java.io.PrintWriter LogWriter_Renamed = null;
		private static volatile java.io.PrintStream LogStream_Renamed = null;
		// Used in println() to synchronize logWriter
		private static readonly Object LogSync = new Object();

		/* Prevent the DriverManager class from being instantiated. */
		private DriverManager()
		{
		}


		/// <summary>
		/// Load the initial JDBC drivers by checking the System property
		/// jdbc.properties and then use the {@code ServiceLoader} mechanism
		/// </summary>
		static DriverManager()
		{
			LoadInitialDrivers();
			Println("JDBC DriverManager initialized");
		}

		/// <summary>
		/// The <code>SQLPermission</code> constant that allows the
		/// setting of the logging stream.
		/// @since 1.3
		/// </summary>
		internal static readonly SQLPermission SET_LOG_PERMISSION = new SQLPermission("setLog");

		/// <summary>
		/// The {@code SQLPermission} constant that allows the
		/// un-register a registered JDBC driver.
		/// @since 1.8
		/// </summary>
		internal static readonly SQLPermission DEREGISTER_DRIVER_PERMISSION = new SQLPermission("deregisterDriver");

		//--------------------------JDBC 2.0-----------------------------

		/// <summary>
		/// Retrieves the log writer.
		/// 
		/// The <code>getLogWriter</code> and <code>setLogWriter</code>
		/// methods should be used instead
		/// of the <code>get/setlogStream</code> methods, which are deprecated. </summary>
		/// <returns> a <code>java.io.PrintWriter</code> object </returns>
		/// <seealso cref= #setLogWriter
		/// @since 1.2 </seealso>
		public static java.io.PrintWriter LogWriter
		{
			get
			{
					return LogWriter_Renamed;
			}
			set
			{
    
				SecurityManager sec = System.SecurityManager;
				if (sec != null)
				{
					sec.CheckPermission(SET_LOG_PERMISSION);
				}
					LogStream_Renamed = null;
					LogWriter_Renamed = value;
			}
		}



		//---------------------------------------------------------------

		/// <summary>
		/// Attempts to establish a connection to the given database URL.
		/// The <code>DriverManager</code> attempts to select an appropriate driver from
		/// the set of registered JDBC drivers.
		/// <para>
		/// <B>Note:</B> If a property is specified as part of the {@code url} and
		/// is also specified in the {@code Properties} object, it is
		/// implementation-defined as to which value will take precedence.
		/// For maximum portability, an application should only specify a
		/// property once.
		/// 
		/// </para>
		/// </summary>
		/// <param name="url"> a database url of the form
		/// <code> jdbc:<em>subprotocol</em>:<em>subname</em></code> </param>
		/// <param name="info"> a list of arbitrary string tag/value pairs as
		/// connection arguments; normally at least a "user" and
		/// "password" property should be included </param>
		/// <returns> a Connection to the URL </returns>
		/// <exception cref="SQLException"> if a database access error occurs or the url is
		/// {@code null} </exception>
		/// <exception cref="SQLTimeoutException">  when the driver has determined that the
		/// timeout value specified by the {@code setLoginTimeout} method
		/// has been exceeded and has at least tried to cancel the
		/// current database connection attempt </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Connection getConnection(String url, java.util.Properties info) throws SQLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static Connection GetConnection(String url, java.util.Properties info)
		{

			return (GetConnection(url, info, Reflection.CallerClass));
		}

		/// <summary>
		/// Attempts to establish a connection to the given database URL.
		/// The <code>DriverManager</code> attempts to select an appropriate driver from
		/// the set of registered JDBC drivers.
		/// <para>
		/// <B>Note:</B> If the {@code user} or {@code password} property are
		/// also specified as part of the {@code url}, it is
		/// implementation-defined as to which value will take precedence.
		/// For maximum portability, an application should only specify a
		/// property once.
		/// 
		/// </para>
		/// </summary>
		/// <param name="url"> a database url of the form
		/// <code>jdbc:<em>subprotocol</em>:<em>subname</em></code> </param>
		/// <param name="user"> the database user on whose behalf the connection is being
		///   made </param>
		/// <param name="password"> the user's password </param>
		/// <returns> a connection to the URL </returns>
		/// <exception cref="SQLException"> if a database access error occurs or the url is
		/// {@code null} </exception>
		/// <exception cref="SQLTimeoutException">  when the driver has determined that the
		/// timeout value specified by the {@code setLoginTimeout} method
		/// has been exceeded and has at least tried to cancel the
		/// current database connection attempt </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Connection getConnection(String url, String user, String password) throws SQLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static Connection GetConnection(String url, String user, String password)
		{
			java.util.Properties info = new java.util.Properties();

			if (user != null)
			{
				info["user"] = user;
			}
			if (password != null)
			{
				info["password"] = password;
			}

			return (GetConnection(url, info, Reflection.CallerClass));
		}

		/// <summary>
		/// Attempts to establish a connection to the given database URL.
		/// The <code>DriverManager</code> attempts to select an appropriate driver from
		/// the set of registered JDBC drivers.
		/// </summary>
		/// <param name="url"> a database url of the form
		///  <code> jdbc:<em>subprotocol</em>:<em>subname</em></code> </param>
		/// <returns> a connection to the URL </returns>
		/// <exception cref="SQLException"> if a database access error occurs or the url is
		/// {@code null} </exception>
		/// <exception cref="SQLTimeoutException">  when the driver has determined that the
		/// timeout value specified by the {@code setLoginTimeout} method
		/// has been exceeded and has at least tried to cancel the
		/// current database connection attempt </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Connection getConnection(String url) throws SQLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static Connection GetConnection(String url)
		{

			java.util.Properties info = new java.util.Properties();
			return (GetConnection(url, info, Reflection.CallerClass));
		}

		/// <summary>
		/// Attempts to locate a driver that understands the given URL.
		/// The <code>DriverManager</code> attempts to select an appropriate driver from
		/// the set of registered JDBC drivers.
		/// </summary>
		/// <param name="url"> a database URL of the form
		///     <code>jdbc:<em>subprotocol</em>:<em>subname</em></code> </param>
		/// <returns> a <code>Driver</code> object representing a driver
		/// that can connect to the given URL </returns>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Driver getDriver(String url) throws SQLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static Driver GetDriver(String url)
		{

			Println("DriverManager.getDriver(\"" + url + "\")");

			Class callerClass = Reflection.CallerClass;

			// Walk through the loaded registeredDrivers attempting to locate someone
			// who understands the given URL.
			foreach (DriverInfo aDriver in RegisteredDrivers)
			{
				// If the caller does not have permission to load the driver then
				// skip it.
				if (IsDriverAllowed(aDriver.Driver, callerClass))
				{
					try
					{
						if (aDriver.Driver.AcceptsURL(url))
						{
							// Success!
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							Println("getDriver returning " + aDriver.Driver.GetType().FullName);
						return (aDriver.Driver);
						}

					}
					catch (SQLException)
					{
						// Drop through and try the next driver.
					}
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Println("    skipping: " + aDriver.Driver.GetType().FullName);
				}

			}

			Println("getDriver: no suitable driver");
			throw new SQLException("No suitable driver", "08001");
		}


		/// <summary>
		/// Registers the given driver with the {@code DriverManager}.
		/// A newly-loaded driver class should call
		/// the method {@code registerDriver} to make itself
		/// known to the {@code DriverManager}. If the driver is currently
		/// registered, no action is taken.
		/// </summary>
		/// <param name="driver"> the new JDBC Driver that is to be registered with the
		///               {@code DriverManager} </param>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <exception cref="NullPointerException"> if {@code driver} is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized void registerDriver(java.sql.Driver driver) throws SQLException
		public static void RegisterDriver(java.sql.Driver driver)
		{
			lock (typeof(DriverManager))
			{
        
				RegisterDriver(driver, null);
			}
		}

		/// <summary>
		/// Registers the given driver with the {@code DriverManager}.
		/// A newly-loaded driver class should call
		/// the method {@code registerDriver} to make itself
		/// known to the {@code DriverManager}. If the driver is currently
		/// registered, no action is taken.
		/// </summary>
		/// <param name="driver"> the new JDBC Driver that is to be registered with the
		///               {@code DriverManager} </param>
		/// <param name="da">     the {@code DriverAction} implementation to be used when
		///               {@code DriverManager#deregisterDriver} is called </param>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <exception cref="NullPointerException"> if {@code driver} is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized void registerDriver(java.sql.Driver driver, DriverAction da) throws SQLException
		public static void RegisterDriver(java.sql.Driver driver, DriverAction da)
		{
			lock (typeof(DriverManager))
			{
        
				/* Register the driver if it has not already been added to our list */
				if (driver != null)
				{
					RegisteredDrivers.AddIfAbsent(new DriverInfo(driver, da));
				}
				else
				{
					// This is for compatibility with the original DriverManager
					throw new NullPointerException();
				}
        
				Println("registerDriver: " + driver);
        
			}
		}

		/// <summary>
		/// Removes the specified driver from the {@code DriverManager}'s list of
		/// registered drivers.
		/// <para>
		/// If a {@code null} value is specified for the driver to be removed, then no
		/// action is taken.
		/// </para>
		/// <para>
		/// If a security manager exists and its {@code checkPermission} denies
		/// permission, then a {@code SecurityException} will be thrown.
		/// </para>
		/// <para>
		/// If the specified driver is not found in the list of registered drivers,
		/// then no action is taken.  If the driver was found, it will be removed
		/// from the list of registered drivers.
		/// </para>
		/// <para>
		/// If a {@code DriverAction} instance was specified when the JDBC driver was
		/// registered, its deregister method will be called
		/// prior to the driver being removed from the list of registered drivers.
		/// 
		/// </para>
		/// </summary>
		/// <param name="driver"> the JDBC Driver to remove </param>
		/// <exception cref="SQLException"> if a database access error occurs </exception>
		/// <exception cref="SecurityException"> if a security manager exists and its
		/// {@code checkPermission} method denies permission to deregister a driver.
		/// </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static synchronized void deregisterDriver(Driver driver) throws SQLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static void DeregisterDriver(Driver driver)
		{
			lock (typeof(DriverManager))
			{
				if (driver == null)
				{
					return;
				}
        
				SecurityManager sec = System.SecurityManager;
				if (sec != null)
				{
					sec.CheckPermission(DEREGISTER_DRIVER_PERMISSION);
				}
        
				Println("DriverManager.deregisterDriver: " + driver);
        
				DriverInfo aDriver = new DriverInfo(driver, null);
				if (RegisteredDrivers.Contains(aDriver))
				{
					if (IsDriverAllowed(driver, Reflection.CallerClass))
					{
						DriverInfo di = RegisteredDrivers[RegisteredDrivers.IndexOf(aDriver)];
						 // If a DriverAction was specified, Call it to notify the
						 // driver that it has been deregistered
						 if (di.Action() != null)
						 {
							 di.Action().Deregister();
						 }
						 RegisteredDrivers.Remove(aDriver);
					}
					else
					{
						// If the caller does not have permission to load the driver then
						// throw a SecurityException.
						throw new SecurityException();
					}
				}
				else
				{
					Println("    couldn't find driver to unload");
				}
			}
		}

		/// <summary>
		/// Retrieves an Enumeration with all of the currently loaded JDBC drivers
		/// to which the current caller has access.
		/// 
		/// <P><B>Note:</B> The classname of a driver can be found using
		/// <CODE>d.getClass().getName()</CODE>
		/// </summary>
		/// <returns> the list of JDBC Drivers loaded by the caller's class loader </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static java.util.Iterator<Driver> getDrivers()
		public static IEnumerator<Driver> Drivers
		{
			get
			{
				List<Driver> result = new List<Driver>();
    
				Class callerClass = Reflection.CallerClass;
    
				// Walk through the loaded registeredDrivers.
				foreach (DriverInfo aDriver in RegisteredDrivers)
				{
					// If the caller does not have permission to load the driver then
					// skip it.
					if (IsDriverAllowed(aDriver.Driver, callerClass))
					{
						result.Add(aDriver.Driver);
					}
					else
					{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						Println("    skipping: " + aDriver.GetType().FullName);
					}
				}
				return (result.elements());
			}
		}


		/// <summary>
		/// Sets the maximum time in seconds that a driver will wait
		/// while attempting to connect to a database once the driver has
		/// been identified.
		/// </summary>
		/// <param name="seconds"> the login time limit in seconds; zero means there is no limit </param>
		/// <seealso cref= #getLoginTimeout </seealso>
		public static int LoginTimeout
		{
			set
			{
				LoginTimeout_Renamed = value;
			}
			get
			{
				return (LoginTimeout_Renamed);
			}
		}


		/// <summary>
		/// Sets the logging/tracing PrintStream that is used
		/// by the <code>DriverManager</code>
		/// and all drivers.
		/// <P>
		/// In the Java 2 SDK, Standard Edition, version 1.3 release, this method checks
		/// to see that there is an <code>SQLPermission</code> object before setting
		/// the logging stream.  If a <code>SecurityManager</code> exists and its
		/// <code>checkPermission</code> method denies setting the log writer, this
		/// method throws a <code>java.lang.SecurityException</code>.
		/// </summary>
		/// <param name="out"> the new logging/tracing PrintStream; to disable, set to <code>null</code> </param>
		/// @deprecated Use {@code setLogWriter} 
		/// <exception cref="SecurityException"> if a security manager exists and its
		///    <code>checkPermission</code> method denies setting the log stream
		/// </exception>
		/// <seealso cref= SecurityManager#checkPermission </seealso>
		/// <seealso cref= #getLogStream </seealso>
		[Obsolete("Use {@code setLogWriter}")]
		public static java.io.PrintStream LogStream
		{
			set
			{
    
				SecurityManager sec = System.SecurityManager;
				if (sec != null)
				{
					sec.CheckPermission(SET_LOG_PERMISSION);
				}
    
				LogStream_Renamed = value;
				if (value != null)
				{
					LogWriter_Renamed = new java.io.PrintWriter(value);
				}
				else
				{
					LogWriter_Renamed = null;
				}
			}
			get
			{
				return LogStream_Renamed;
			}
		}


		/// <summary>
		/// Prints a message to the current JDBC log stream.
		/// </summary>
		/// <param name="message"> a log or tracing message </param>
		public static void Println(String message)
		{
			lock (LogSync)
			{
				if (LogWriter_Renamed != null)
				{
					LogWriter_Renamed.Println(message);

					// automatic flushing is never enabled, so we must do it ourselves
					LogWriter_Renamed.Flush();
				}
			}
		}

		//------------------------------------------------------------------------

		// Indicates whether the class object that would be created if the code calling
		// DriverManager is accessible.
		private static bool IsDriverAllowed(Driver driver, Class caller)
		{
			ClassLoader callerCL = caller != null ? caller.ClassLoader : null;
			return IsDriverAllowed(driver, callerCL);
		}

		private static bool IsDriverAllowed(Driver driver, ClassLoader classLoader)
		{
			bool result = false;
			if (driver != null)
			{
				Class aClass = null;
				try
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					aClass = Class.ForName(driver.GetType().FullName, true, classLoader);
				}
				catch (Exception)
				{
					result = false;
				}

				 result = (aClass == driver.GetType()) ? true : false;
			}

			return result;
		}

		private static void LoadInitialDrivers()
		{
			String drivers;
			try
			{
				drivers = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
			}
			catch (Exception)
			{
				drivers = null;
			}
			// If the driver is packaged as a Service Provider, load it.
			// Get all the drivers through the classloader
			// exposed as a java.sql.Driver.class service.
			// ServiceLoader.load() replaces the sun.misc.Providers()

			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2());

			Println("DriverManager.initialize: jdbc.drivers = " + drivers);

			if (drivers == null || drivers.Equals(""))
			{
				return;
			}
			String[] driversList = drivers.Split(":");
			Println("number of Drivers:" + driversList.Length);
			foreach (String aDriver in driversList)
			{
				try
				{
					Println("DriverManager.Initialize: loading " + aDriver);
					Class.ForName(aDriver, true, ClassLoader.SystemClassLoader);
				}
				catch (Exception ex)
				{
					Println("DriverManager.Initialize: load failed: " + ex);
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual String Run()
			{
				return System.getProperty("jdbc.drivers");
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Void>
		{
			public PrivilegedActionAnonymousInnerClassHelper2()
			{
			}

			public virtual Void Run()
			{

				ServiceLoader<Driver> loadedDrivers = ServiceLoader.Load(typeof(Driver));
				IEnumerator<Driver> driversIterator = loadedDrivers.Iterator();

				/* Load these drivers, so that they can be instantiated.
				 * It may be the case that the driver class may not be there
				 * i.e. there may be a packaged driver with the service class
				 * as implementation of java.sql.Driver but the actual class
				 * may be missing. In that case a java.util.ServiceConfigurationError
				 * will be thrown at runtime by the VM trying to locate
				 * and load the service.
				 *
				 * Adding a try catch block to catch those runtime errors
				 * if driver not available in classpath but it's
				 * packaged as service and that service is there in classpath.
				 */
				try
				{
					while (driversIterator.MoveNext())
					{
						driversIterator.Current;
					}
				}
				catch (Throwable)
				{
				// Do nothing
				}
				return null;
			}
		}


		//  Worker method called by the public getConnection() methods.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Connection getConnection(String url, java.util.Properties info, Class caller) throws SQLException
		private static Connection GetConnection(String url, java.util.Properties info, Class caller)
		{
			/*
			 * When callerCl is null, we should check the application's
			 * (which is invoking this class indirectly)
			 * classloader, so that the JDBC driver class outside rt.jar
			 * can be loaded from here.
			 */
			ClassLoader callerCL = caller != null ? caller.ClassLoader : null;
			lock (typeof(DriverManager))
			{
				// synchronize loading of the correct classloader.
				if (callerCL == null)
				{
					callerCL = Thread.CurrentThread.ContextClassLoader;
				}
			}

			if (url == null)
			{
				throw new SQLException("The url cannot be null", "08001");
			}

			Println("DriverManager.getConnection(\"" + url + "\")");

			// Walk through the loaded registeredDrivers attempting to make a connection.
			// Remember the first exception that gets raised so we can reraise it.
			SQLException reason = null;

			foreach (DriverInfo aDriver in RegisteredDrivers)
			{
				// If the caller does not have permission to load the driver then
				// skip it.
				if (IsDriverAllowed(aDriver.Driver, callerCL))
				{
					try
					{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						Println("    trying " + aDriver.Driver.GetType().FullName);
						Connection con = aDriver.Driver.Connect(url, info);
						if (con != null)
						{
							// Success!
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							Println("getConnection returning " + aDriver.Driver.GetType().FullName);
							return (con);
						}
					}
					catch (SQLException ex)
					{
						if (reason == null)
						{
							reason = ex;
						}
					}

				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Println("    skipping: " + aDriver.GetType().FullName);
				}

			}

			// if we got here nobody could connect.
			if (reason != null)
			{
				Println("getConnection failed: " + reason);
				throw reason;
			}

			Println("getConnection: no suitable driver found for " + url);
			throw new SQLException("No suitable driver found for " + url, "08001");
		}


	}

	/*
	 * Wrapper class for registered Drivers in order to not expose Driver.equals()
	 * to avoid the capture of the Driver it being compared to as it might not
	 * normally have access.
	 */
	internal class DriverInfo
	{

		internal readonly Driver Driver;
		internal DriverAction Da;
		internal DriverInfo(Driver driver, DriverAction action)
		{
			this.Driver = driver;
			Da = action;
		}

		public override bool Equals(Object other)
		{
			return (other is DriverInfo) && this.Driver == ((DriverInfo) other).Driver;
		}

		public override int HashCode()
		{
			return Driver.HashCode();
		}

		public override String ToString()
		{
			return ("driver[className=" + Driver + "]");
		}

		internal virtual DriverAction Action()
		{
			return Da;
		}
	}

}