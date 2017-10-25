using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.logging
{


	/// <summary>
	/// The management interface for the logging facility. It is recommended
	/// to use the <seealso cref="java.lang.management.PlatformLoggingMXBean"/> management
	/// interface that implements all attributes defined in this
	/// {@code LoggingMXBean}.  The
	/// {@link java.lang.management.ManagementFactory#getPlatformMXBean(Class)
	/// ManagementFactory.getPlatformMXBean} method can be used to obtain
	/// the {@code PlatformLoggingMXBean} object representing the management
	/// interface for logging.
	/// 
	/// <para>There is a single global instance of the <tt>LoggingMXBean</tt>.
	/// This instance is an <seealso cref="javax.management.MXBean MXBean"/> that
	/// can be obtained by calling the <seealso cref="LogManager#getLoggingMXBean"/>
	/// method or from the
	/// {@link java.lang.management.ManagementFactory#getPlatformMBeanServer
	/// platform <tt>MBeanServer</tt>}.
	/// </para>
	/// <para>
	/// The <seealso cref="javax.management.ObjectName ObjectName"/> that uniquely identifies
	/// the management interface for logging within the {@code MBeanServer} is:
	/// <pre>
	///    <seealso cref="LogManager#LOGGING_MXBEAN_NAME java.util.logging:type=Logging"/>
	/// </pre>
	/// </para>
	/// <para>
	/// The instance registered in the platform {@code MBeanServer}
	/// is also a <seealso cref="java.lang.management.PlatformLoggingMXBean"/>.
	/// 
	/// @author  Ron Mann
	/// @author  Mandy Chung
	/// @since   1.5
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.lang.management.PlatformLoggingMXBean </seealso>
	public interface LoggingMXBean
	{

		/// <summary>
		/// Returns the list of currently registered logger names. This method
		/// calls <seealso cref="LogManager#getLoggerNames"/> and returns a list
		/// of the logger names.
		/// </summary>
		/// <returns> A list of <tt>String</tt> each of which is a
		///         currently registered <tt>Logger</tt> name. </returns>
		IList<String> LoggerNames {get;}

		/// <summary>
		/// Gets the name of the log level associated with the specified logger.
		/// If the specified logger does not exist, <tt>null</tt>
		/// is returned.
		/// This method first finds the logger of the given name and
		/// then returns the name of the log level by calling:
		/// <blockquote>
		///   <seealso cref="Logger#getLevel Logger.getLevel()"/>.<seealso cref="Level#getName getName()"/>;
		/// </blockquote>
		/// 
		/// <para>
		/// If the <tt>Level</tt> of the specified logger is <tt>null</tt>,
		/// which means that this logger's effective level is inherited
		/// from its parent, an empty string will be returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="loggerName"> The name of the <tt>Logger</tt> to be retrieved.
		/// </param>
		/// <returns> The name of the log level of the specified logger; or
		///         an empty string if the log level of the specified logger
		///         is <tt>null</tt>.  If the specified logger does not
		///         exist, <tt>null</tt> is returned.
		/// </returns>
		/// <seealso cref= Logger#getLevel </seealso>
		String GetLoggerLevel(String loggerName);

		/// <summary>
		/// Sets the specified logger to the specified new level.
		/// If the <tt>levelName</tt> is not <tt>null</tt>, the level
		/// of the specified logger is set to the parsed <tt>Level</tt>
		/// matching the <tt>levelName</tt>.
		/// If the <tt>levelName</tt> is <tt>null</tt>, the level
		/// of the specified logger is set to <tt>null</tt> and
		/// the effective level of the logger is inherited from
		/// its nearest ancestor with a specific (non-null) level value.
		/// </summary>
		/// <param name="loggerName"> The name of the <tt>Logger</tt> to be set.
		///                   Must be non-null. </param>
		/// <param name="levelName"> The name of the level to set on the specified logger,
		///                 or <tt>null</tt> if setting the level to inherit
		///                 from its nearest ancestor.
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the specified logger
		/// does not exist, or <tt>levelName</tt> is not a valid level name.
		/// </exception>
		/// <exception cref="SecurityException"> if a security manager exists and if
		/// the caller does not have LoggingPermission("control").
		/// </exception>
		/// <seealso cref= Logger#setLevel </seealso>
		void SetLoggerLevel(String loggerName, String levelName);

		/// <summary>
		/// Returns the name of the parent for the specified logger.
		/// If the specified logger does not exist, <tt>null</tt> is returned.
		/// If the specified logger is the root <tt>Logger</tt> in the namespace,
		/// the result will be an empty string.
		/// </summary>
		/// <param name="loggerName"> The name of a <tt>Logger</tt>.
		/// </param>
		/// <returns> the name of the nearest existing parent logger;
		///         an empty string if the specified logger is the root logger.
		///         If the specified logger does not exist, <tt>null</tt>
		///         is returned. </returns>
		String GetParentLoggerName(String loggerName);
	}

}