using System.Collections.Generic;

/*
 * Copyright (c) 2009, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.management
{

	/// <summary>
	/// The management interface for the <seealso cref="java.util.logging logging"/> facility.
	/// 
	/// <para>There is a single global instance of the <tt>PlatformLoggingMXBean</tt>.
	/// The {@link java.lang.management.ManagementFactory#getPlatformMXBean(Class)
	/// ManagementFactory.getPlatformMXBean} method can be used to obtain
	/// the {@code PlatformLoggingMXBean} object as follows:
	/// <pre>
	///     PlatformLoggingMXBean logging = ManagementFactory.getPlatformMXBean(PlatformLoggingMXBean.class);
	/// </pre>
	/// The {@code PlatformLoggingMXBean} object is also registered with the
	/// platform {@link java.lang.management.ManagementFactory#getPlatformMBeanServer
	/// MBeanServer}.
	/// The <seealso cref="javax.management.ObjectName ObjectName"/> for uniquely
	/// identifying the {@code PlatformLoggingMXBean} within an MBeanServer is:
	/// <pre>
	///      <seealso cref="java.util.logging.LogManager#LOGGING_MXBEAN_NAME java.util.logging:type=Logging"/>
	/// </pre>
	/// 
	/// </para>
	/// <para>The instance registered in the platform <tt>MBeanServer</tt> with
	/// this {@code ObjectName} implements all attributes defined by
	/// <seealso cref="java.util.logging.LoggingMXBean"/>.
	/// 
	/// @since   1.7
	/// </para>
	/// </summary>
	public interface PlatformLoggingMXBean : PlatformManagedObject
	{

		/// <summary>
		/// Returns the list of the currently registered
		/// <seealso cref="java.util.logging.Logger logger"/> names. This method
		/// calls <seealso cref="java.util.logging.LogManager#getLoggerNames"/> and
		/// returns a list of the logger names.
		/// </summary>
		/// <returns> A list of {@code String} each of which is a
		///         currently registered {@code Logger} name. </returns>
		IList<String> LoggerNames {get;}

		/// <summary>
		/// Gets the name of the log {@link java.util.logging.Logger#getLevel
		/// level} associated with the specified logger.
		/// If the specified logger does not exist, {@code null}
		/// is returned.
		/// This method first finds the logger of the given name and
		/// then returns the name of the log level by calling:
		/// <blockquote>
		///   {@link java.util.logging.Logger#getLevel
		///    Logger.getLevel()}.<seealso cref="java.util.logging.Level#getName getName()"/>;
		/// </blockquote>
		/// 
		/// <para>
		/// If the {@code Level} of the specified logger is {@code null},
		/// which means that this logger's effective level is inherited
		/// from its parent, an empty string will be returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="loggerName"> The name of the {@code Logger} to be retrieved.
		/// </param>
		/// <returns> The name of the log level of the specified logger; or
		///         an empty string if the log level of the specified logger
		///         is {@code null}.  If the specified logger does not
		///         exist, {@code null} is returned.
		/// </returns>
		/// <seealso cref= java.util.logging.Logger#getLevel </seealso>
		String GetLoggerLevel(String loggerName);

		/// <summary>
		/// Sets the specified logger to the specified new
		/// <seealso cref="java.util.logging.Logger#setLevel level"/>.
		/// If the {@code levelName} is not {@code null}, the level
		/// of the specified logger is set to the parsed
		/// <seealso cref="java.util.logging.Level Level"/>
		/// matching the {@code levelName}.
		/// If the {@code levelName} is {@code null}, the level
		/// of the specified logger is set to {@code null} and
		/// the effective level of the logger is inherited from
		/// its nearest ancestor with a specific (non-null) level value.
		/// </summary>
		/// <param name="loggerName"> The name of the {@code Logger} to be set.
		///                   Must be non-null. </param>
		/// <param name="levelName"> The name of the level to set on the specified logger,
		///                 or  {@code null} if setting the level to inherit
		///                 from its nearest ancestor.
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the specified logger
		/// does not exist, or {@code levelName} is not a valid level name.
		/// </exception>
		/// <exception cref="SecurityException"> if a security manager exists and if
		/// the caller does not have LoggingPermission("control").
		/// </exception>
		/// <seealso cref= java.util.logging.Logger#setLevel </seealso>
		void SetLoggerLevel(String loggerName, String levelName);

		/// <summary>
		/// Returns the name of the
		/// <seealso cref="java.util.logging.Logger#getParent parent"/>
		/// for the specified logger.
		/// If the specified logger does not exist, {@code null} is returned.
		/// If the specified logger is the root {@code Logger} in the namespace,
		/// the result will be an empty string.
		/// </summary>
		/// <param name="loggerName"> The name of a {@code Logger}.
		/// </param>
		/// <returns> the name of the nearest existing parent logger;
		///         an empty string if the specified logger is the root logger.
		///         If the specified logger does not exist, {@code null}
		///         is returned. </returns>
		String GetParentLoggerName(String loggerName);
	}

}