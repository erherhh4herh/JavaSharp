using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (c) 2000, 2014, Oracle and/or its affiliates. All rights reserved.
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

	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;

	/// <summary>
	/// A Logger object is used to log messages for a specific
	/// system or application component.  Loggers are normally named,
	/// using a hierarchical dot-separated namespace.  Logger names
	/// can be arbitrary strings, but they should normally be based on
	/// the package name or class name of the logged component, such
	/// as java.net or javax.swing.  In addition it is possible to create
	/// "anonymous" Loggers that are not stored in the Logger namespace.
	/// <para>
	/// Logger objects may be obtained by calls on one of the getLogger
	/// factory methods.  These will either create a new Logger or
	/// return a suitable existing Logger. It is important to note that
	/// the Logger returned by one of the {@code getLogger} factory methods
	/// may be garbage collected at any time if a strong reference to the
	/// Logger is not kept.
	/// </para>
	/// <para>
	/// Logging messages will be forwarded to registered Handler
	/// objects, which can forward the messages to a variety of
	/// destinations, including consoles, files, OS logs, etc.
	/// </para>
	/// <para>
	/// Each Logger keeps track of a "parent" Logger, which is its
	/// nearest existing ancestor in the Logger namespace.
	/// </para>
	/// <para>
	/// Each Logger has a "Level" associated with it.  This reflects
	/// a minimum Level that this logger cares about.  If a Logger's
	/// level is set to <tt>null</tt>, then its effective level is inherited
	/// from its parent, which may in turn obtain it recursively from its
	/// parent, and so on up the tree.
	/// </para>
	/// <para>
	/// The log level can be configured based on the properties from the
	/// logging configuration file, as described in the description
	/// of the LogManager class.  However it may also be dynamically changed
	/// by calls on the Logger.setLevel method.  If a logger's level is
	/// changed the change may also affect child loggers, since any child
	/// logger that has <tt>null</tt> as its level will inherit its
	/// effective level from its parent.
	/// </para>
	/// <para>
	/// On each logging call the Logger initially performs a cheap
	/// check of the request level (e.g., SEVERE or FINE) against the
	/// effective log level of the logger.  If the request level is
	/// lower than the log level, the logging call returns immediately.
	/// </para>
	/// <para>
	/// After passing this initial (cheap) test, the Logger will allocate
	/// a LogRecord to describe the logging message.  It will then call a
	/// Filter (if present) to do a more detailed check on whether the
	/// record should be published.  If that passes it will then publish
	/// the LogRecord to its output Handlers.  By default, loggers also
	/// publish to their parent's Handlers, recursively up the tree.
	/// </para>
	/// <para>
	/// Each Logger may have a {@code ResourceBundle} associated with it.
	/// The {@code ResourceBundle} may be specified by name, using the
	/// <seealso cref="#getLogger(java.lang.String, java.lang.String)"/> factory
	/// method, or by value - using the {@link
	/// #setResourceBundle(java.util.ResourceBundle) setResourceBundle} method.
	/// This bundle will be used for localizing logging messages.
	/// If a Logger does not have its own {@code ResourceBundle} or resource bundle
	/// name, then it will inherit the {@code ResourceBundle} or resource bundle name
	/// from its parent, recursively up the tree.
	/// </para>
	/// <para>
	/// Most of the logger output methods take a "msg" argument.  This
	/// msg argument may be either a raw value or a localization key.
	/// During formatting, if the logger has (or inherits) a localization
	/// {@code ResourceBundle} and if the {@code ResourceBundle} has a mapping for
	/// the msg string, then the msg string is replaced by the localized value.
	/// Otherwise the original msg string is used.  Typically, formatters use
	/// java.text.MessageFormat style formatting to format parameters, so
	/// for example a format string "{0} {1}" would format two parameters
	/// as strings.
	/// </para>
	/// <para>
	/// A set of methods alternatively take a "msgSupplier" instead of a "msg"
	/// argument.  These methods take a <seealso cref="Supplier"/>{@code <String>} function
	/// which is invoked to construct the desired log message only when the message
	/// actually is to be logged based on the effective log level thus eliminating
	/// unnecessary message construction. For example, if the developer wants to
	/// log system health status for diagnosis, with the String-accepting version,
	/// the code would look like:
	/// <pre><code>
	/// 
	///   class DiagnosisMessages {
	///     static String systemHealthStatus() {
	///       // collect system health information
	///       ...
	///     }
	///   }
	///   ...
	///   logger.log(Level.FINER, DiagnosisMessages.systemHealthStatus());
	/// </code></pre>
	/// With the above code, the health status is collected unnecessarily even when
	/// the log level FINER is disabled. With the Supplier-accepting version as
	/// below, the status will only be collected when the log level FINER is
	/// enabled.
	/// <pre><code>
	/// 
	///   logger.log(Level.FINER, DiagnosisMessages::systemHealthStatus);
	/// </code></pre>
	/// </para>
	/// <para>
	/// When looking for a {@code ResourceBundle}, the logger will first look at
	/// whether a bundle was specified using {@link
	/// #setResourceBundle(java.util.ResourceBundle) setResourceBundle}, and then
	/// only whether a resource bundle name was specified through the {@link
	/// #getLogger(java.lang.String, java.lang.String) getLogger} factory method.
	/// If no {@code ResourceBundle} or no resource bundle name is found,
	/// then it will use the nearest {@code ResourceBundle} or resource bundle
	/// name inherited from its parent tree.<br>
	/// When a {@code ResourceBundle} was inherited or specified through the
	/// {@link
	/// #setResourceBundle(java.util.ResourceBundle) setResourceBundle} method, then
	/// that {@code ResourceBundle} will be used. Otherwise if the logger only
	/// has or inherited a resource bundle name, then that resource bundle name
	/// will be mapped to a {@code ResourceBundle} object, using the default Locale
	/// at the time of logging.
	/// <br id="ResourceBundleMapping">When mapping resource bundle names to
	/// {@code ResourceBundle} objects, the logger will first try to use the
	/// Thread's {@link java.lang.Thread#getContextClassLoader() context class
	/// loader} to map the given resource bundle name to a {@code ResourceBundle}.
	/// If the thread context class loader is {@code null}, it will try the
	/// <seealso cref="java.lang.ClassLoader#getSystemClassLoader() system class loader"/>
	/// instead.  If the {@code ResourceBundle} is still not found, it will use the
	/// class loader of the first caller of the {@link
	/// #getLogger(java.lang.String, java.lang.String) getLogger} factory method.
	/// </para>
	/// <para>
	/// Formatting (including localization) is the responsibility of
	/// the output Handler, which will typically call a Formatter.
	/// </para>
	/// <para>
	/// Note that formatting need not occur synchronously.  It may be delayed
	/// until a LogRecord is actually written to an external sink.
	/// </para>
	/// <para>
	/// The logging methods are grouped in five main categories:
	/// <ul>
	/// </para>
	/// <li><para>
	///     There are a set of "log" methods that take a log level, a message
	///     string, and optionally some parameters to the message string.
	/// </para>
	/// <li><para>
	///     There are a set of "logp" methods (for "log precise") that are
	///     like the "log" methods, but also take an explicit source class name
	///     and method name.
	/// </para>
	/// <li><para>
	///     There are a set of "logrb" method (for "log with resource bundle")
	///     that are like the "logp" method, but also take an explicit resource
	///     bundle object for use in localizing the log message.
	/// </para>
	/// <li><para>
	///     There are convenience methods for tracing method entries (the
	///     "entering" methods), method returns (the "exiting" methods) and
	///     throwing exceptions (the "throwing" methods).
	/// </para>
	/// <li><para>
	///     Finally, there are a set of convenience methods for use in the
	///     very simplest cases, when a developer simply wants to log a
	///     simple string at a given log level.  These methods are named
	///     after the standard Level names ("severe", "warning", "info", etc.)
	///     and take a single argument, a message string.
	/// </ul>
	/// </para>
	/// <para>
	/// For the methods that do not take an explicit source name and
	/// method name, the Logging framework will make a "best effort"
	/// to determine which class and method called into the logging method.
	/// However, it is important to realize that this automatically inferred
	/// information may only be approximate (or may even be quite wrong!).
	/// Virtual machines are allowed to do extensive optimizations when
	/// JITing and may entirely remove stack frames, making it impossible
	/// to reliably locate the calling class and method.
	/// <P>
	/// All methods on Logger are multi-thread safe.
	/// </para>
	/// <para>
	/// <b>Subclassing Information:</b> Note that a LogManager class may
	/// provide its own implementation of named Loggers for any point in
	/// the namespace.  Therefore, any subclasses of Logger (unless they
	/// are implemented in conjunction with a new LogManager class) should
	/// take care to obtain a Logger instance from the LogManager class and
	/// should delegate operations such as "isLoggable" and "log(LogRecord)"
	/// to that instance.  Note that in order to intercept all logging
	/// output, subclasses need only override the log(LogRecord) method.
	/// All the other logging methods are implemented as calls on this
	/// log(LogRecord) method.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>
	public class Logger
	{
		private static readonly Handler[] EmptyHandlers = new Handler[0];
		private static readonly int OffValue = Level.OFF.IntValue();

		internal const String SYSTEM_LOGGER_RB_NAME = "sun.util.logging.resources.logging";

		// This class is immutable and it is important that it remains so.
		private sealed class LoggerBundle
		{
			internal readonly String ResourceBundleName; // Base name of the bundle.
			internal readonly ResourceBundle UserBundle; // Bundle set through setResourceBundle.
			internal LoggerBundle(String resourceBundleName, ResourceBundle bundle)
			{
				this.ResourceBundleName = resourceBundleName;
				this.UserBundle = bundle;
			}
			internal bool SystemBundle
			{
				get
				{
					return SYSTEM_LOGGER_RB_NAME.Equals(ResourceBundleName);
				}
			}
			internal static LoggerBundle Get(String name, ResourceBundle bundle)
			{
				if (name == null && bundle == null)
				{
					return NO_RESOURCE_BUNDLE;
				}
				else if (SYSTEM_LOGGER_RB_NAME.Equals(name) && bundle == null)
				{
					return SYSTEM_BUNDLE;
				}
				else
				{
					return new LoggerBundle(name, bundle);
				}
			}
		}

		// This instance will be shared by all loggers created by the system
		// code
		private static readonly LoggerBundle SYSTEM_BUNDLE = new LoggerBundle(SYSTEM_LOGGER_RB_NAME, null);

		// This instance indicates that no resource bundle has been specified yet,
		// and it will be shared by all loggers which have no resource bundle.
		private static readonly LoggerBundle NO_RESOURCE_BUNDLE = new LoggerBundle(null, null);

		private volatile LogManager Manager;
		private String Name_Renamed;
		private readonly CopyOnWriteArrayList<Handler> Handlers_Renamed = new CopyOnWriteArrayList<Handler>();
		private volatile LoggerBundle LoggerBundle = NO_RESOURCE_BUNDLE;
		private volatile bool UseParentHandlers_Renamed = true;
		private volatile Filter Filter_Renamed;
		private bool Anonymous;

		// Cache to speed up behavior of findResourceBundle:
		private ResourceBundle Catalog; // Cached resource bundle
		private String CatalogName; // name associated with catalog
		private Locale CatalogLocale; // locale associated with catalog

		// The fields relating to parent-child relationships and levels
		// are managed under a separate lock, the treeLock.
		private static readonly Object TreeLock = new Object();
		// We keep weak references from parents to children, but strong
		// references from children to parents.
		private volatile Logger Parent_Renamed; // our nearest parent.
		private List<LogManager.LoggerWeakRef> Kids; // WeakReferences to loggers that have us as parent
		private volatile Level LevelObject;
		private volatile int LevelValue; // current effective level value
		private WeakReference<ClassLoader> CallersClassLoaderRef_Renamed;
		private readonly bool IsSystemLogger;

		/// <summary>
		/// GLOBAL_LOGGER_NAME is a name for the global logger.
		/// 
		/// @since 1.6
		/// </summary>
		public const String GLOBAL_LOGGER_NAME = "global";

		/// <summary>
		/// Return global logger object with the name Logger.GLOBAL_LOGGER_NAME.
		/// </summary>
		/// <returns> global logger object
		/// @since 1.7 </returns>
		public static Logger Global
		{
			get
			{
				// In order to break a cyclic dependence between the LogManager
				// and Logger static initializers causing deadlocks, the global
				// logger is created with a special constructor that does not
				// initialize its log manager.
				//
				// If an application calls Logger.getGlobal() before any logger
				// has been initialized, it is therefore possible that the
				// LogManager class has not been initialized yet, and therefore
				// Logger.global.manager will be null.
				//
				// In order to finish the initialization of the global logger, we
				// will therefore call LogManager.getLogManager() here.
				//
				// To prevent race conditions we also need to call
				// LogManager.getLogManager() unconditionally here.
				// Indeed we cannot rely on the observed value of global.manager,
				// because global.manager will become not null somewhere during
				// the initialization of LogManager.
				// If two threads are calling getGlobal() concurrently, one thread
				// will see global.manager null and call LogManager.getLogManager(),
				// but the other thread could come in at a time when global.manager
				// is already set although ensureLogManagerInitialized is not finished
				// yet...
				// Calling LogManager.getLogManager() unconditionally will fix that.
    
				LogManager.LogManager;
    
				// Now the global LogManager should be initialized,
				// and the global logger should have been added to
				// it, unless we were called within the constructor of a LogManager
				// subclass installed as LogManager, in which case global.manager
				// would still be null, and global will be lazily initialized later on.
    
				return Global_Renamed;
			}
		}

		/// <summary>
		/// The "global" Logger object is provided as a convenience to developers
		/// who are making casual use of the Logging package.  Developers
		/// who are making serious use of the logging package (for example
		/// in products) should create and use their own Logger objects,
		/// with appropriate names, so that logging can be controlled on a
		/// suitable per-Logger granularity. Developers also need to keep a
		/// strong reference to their Logger objects to prevent them from
		/// being garbage collected.
		/// <para>
		/// </para>
		/// </summary>
		/// @deprecated Initialization of this field is prone to deadlocks.
		/// The field must be initialized by the Logger class initialization
		/// which may cause deadlocks with the LogManager class initialization.
		/// In such cases two class initialization wait for each other to complete.
		/// The preferred way to get the global logger object is via the call
		/// <code>Logger.getGlobal()</code>.
		/// For compatibility with old JDK versions where the
		/// <code>Logger.getGlobal()</code> is not available use the call
		/// <code>Logger.getLogger(Logger.GLOBAL_LOGGER_NAME)</code>
		/// or <code>Logger.getLogger("global")</code>. 
		[Obsolete("Initialization of this field is prone to deadlocks.")]
		public static readonly Logger Global_Renamed = new Logger(GLOBAL_LOGGER_NAME);

		/// <summary>
		/// Protected method to construct a logger for a named subsystem.
		/// <para>
		/// The logger will be initially configured with a null Level
		/// and with useParentHandlers set to true.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">    A name for the logger.  This should
		///                          be a dot-separated name and should normally
		///                          be based on the package name or class name
		///                          of the subsystem, such as java.net
		///                          or javax.swing.  It may be null for anonymous Loggers. </param>
		/// <param name="resourceBundleName">  name of ResourceBundle to be used for localizing
		///                          messages for this logger.  May be null if none
		///                          of the messages require localization. </param>
		/// <exception cref="MissingResourceException"> if the resourceBundleName is non-null and
		///             no corresponding resource can be found. </exception>
		protected internal Logger(String name, String resourceBundleName) : this(name, resourceBundleName, null, LogManager.LogManager, false)
		{
		}

		internal Logger(String name, String resourceBundleName, Class caller, LogManager manager, bool isSystemLogger)
		{
			this.Manager = manager;
			this.IsSystemLogger = isSystemLogger;
			SetupResourceInfo(resourceBundleName, caller);
			this.Name_Renamed = name;
			LevelValue = Level.INFO.IntValue();
		}

		private Class CallersClassLoaderRef
		{
			set
			{
				ClassLoader callersClassLoader = ((value != null) ? value.ClassLoader : null);
				if (callersClassLoader != null)
				{
					this.CallersClassLoaderRef_Renamed = new WeakReference<>(callersClassLoader);
				}
			}
		}

		private ClassLoader CallersClassLoader
		{
			get
			{
				return (CallersClassLoaderRef_Renamed != null) ? CallersClassLoaderRef_Renamed.get() : null;
			}
		}

		// This constructor is used only to create the global Logger.
		// It is needed to break a cyclic dependence between the LogManager
		// and Logger static initializers causing deadlocks.
		private Logger(String name)
		{
			// The manager field is not initialized here.
			this.Name_Renamed = name;
			this.IsSystemLogger = true;
			LevelValue = Level.INFO.IntValue();
		}

		// It is called from LoggerContext.addLocalLogger() when the logger
		// is actually added to a LogManager.
		internal virtual LogManager LogManager
		{
			set
			{
				this.Manager = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkPermission() throws SecurityException
		private void CheckPermission()
		{
			if (!Anonymous)
			{
				if (Manager == null)
				{
					// Complete initialization of the global Logger.
					Manager = LogManager.LogManager;
				}
				Manager.CheckPermission();
			}
		}

		// Until all JDK code converted to call sun.util.logging.PlatformLogger
		// (see 7054233), we need to determine if Logger.getLogger is to add
		// a system logger or user logger.
		//
		// As an interim solution, if the immediate caller whose caller loader is
		// null, we assume it's a system logger and add it to the system context.
		// These system loggers only set the resource bundle to the given
		// resource bundle name (rather than the default system resource bundle).
		private class SystemLoggerHelper
		{
			internal static bool DisableCallerCheck = GetBooleanProperty("sun.util.logging.disableCallerCheck");
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static boolean getBooleanProperty(final String key)
			internal static bool GetBooleanProperty(String key)
			{
				String s = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(key));
				return Convert.ToBoolean(s);
			}

			private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
			{
				private string Key;

				public PrivilegedActionAnonymousInnerClassHelper(string key)
				{
					this.Key = key;
				}

				public virtual String Run()
				{
					return System.getProperty(Key);
				}
			}
		}

		private static Logger DemandLogger(String name, String resourceBundleName, Class caller)
		{
			LogManager manager = LogManager.LogManager;
			SecurityManager sm = System.SecurityManager;
			if (sm != null && !SystemLoggerHelper.DisableCallerCheck)
			{
				if (caller.ClassLoader == null)
				{
					return manager.DemandSystemLogger(name, resourceBundleName);
				}
			}
			return manager.DemandLogger(name, resourceBundleName, caller);
			// ends up calling new Logger(name, resourceBundleName, caller)
			// iff the logger doesn't exist already
		}

		/// <summary>
		/// Find or create a logger for a named subsystem.  If a logger has
		/// already been created with the given name it is returned.  Otherwise
		/// a new logger is created.
		/// <para>
		/// If a new logger is created its log level will be configured
		/// based on the LogManager configuration and it will configured
		/// to also send logging output to its parent's Handlers.  It will
		/// be registered in the LogManager global namespace.
		/// </para>
		/// <para>
		/// Note: The LogManager may only retain a weak reference to the newly
		/// created Logger. It is important to understand that a previously
		/// created Logger with the given name may be garbage collected at any
		/// time if there is no strong reference to the Logger. In particular,
		/// this means that two back-to-back calls like
		/// {@code getLogger("MyLogger").log(...)} may use different Logger
		/// objects named "MyLogger" if there is no strong reference to the
		/// Logger named "MyLogger" elsewhere in the program.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">            A name for the logger.  This should
		///                          be a dot-separated name and should normally
		///                          be based on the package name or class name
		///                          of the subsystem, such as java.net
		///                          or javax.swing </param>
		/// <returns> a suitable Logger </returns>
		/// <exception cref="NullPointerException"> if the name is null. </exception>

		// Synchronization is not required here. All synchronization for
		// adding a new Logger object is handled by LogManager.addLogger().
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Logger getLogger(String name)
		public static Logger GetLogger(String name)
		{
			// This method is intentionally not a wrapper around a call
			// to getLogger(name, resourceBundleName). If it were then
			// this sequence:
			//
			//     getLogger("Foo", "resourceBundleForFoo");
			//     getLogger("Foo");
			//
			// would throw an IllegalArgumentException in the second call
			// because the wrapper would result in an attempt to replace
			// the existing "resourceBundleForFoo" with null.
			return DemandLogger(name, null, Reflection.CallerClass);
		}

		/// <summary>
		/// Find or create a logger for a named subsystem.  If a logger has
		/// already been created with the given name it is returned.  Otherwise
		/// a new logger is created.
		/// <para>
		/// If a new logger is created its log level will be configured
		/// based on the LogManager and it will configured to also send logging
		/// output to its parent's Handlers.  It will be registered in
		/// the LogManager global namespace.
		/// </para>
		/// <para>
		/// Note: The LogManager may only retain a weak reference to the newly
		/// created Logger. It is important to understand that a previously
		/// created Logger with the given name may be garbage collected at any
		/// time if there is no strong reference to the Logger. In particular,
		/// this means that two back-to-back calls like
		/// {@code getLogger("MyLogger", ...).log(...)} may use different Logger
		/// objects named "MyLogger" if there is no strong reference to the
		/// Logger named "MyLogger" elsewhere in the program.
		/// </para>
		/// <para>
		/// If the named Logger already exists and does not yet have a
		/// localization resource bundle then the given resource bundle
		/// name is used.  If the named Logger already exists and has
		/// a different resource bundle name then an IllegalArgumentException
		/// is thrown.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="name">    A name for the logger.  This should
		///                          be a dot-separated name and should normally
		///                          be based on the package name or class name
		///                          of the subsystem, such as java.net
		///                          or javax.swing </param>
		/// <param name="resourceBundleName">  name of ResourceBundle to be used for localizing
		///                          messages for this logger. May be {@code null}
		///                          if none of the messages require localization. </param>
		/// <returns> a suitable Logger </returns>
		/// <exception cref="MissingResourceException"> if the resourceBundleName is non-null and
		///             no corresponding resource can be found. </exception>
		/// <exception cref="IllegalArgumentException"> if the Logger already exists and uses
		///             a different resource bundle name; or if
		///             {@code resourceBundleName} is {@code null} but the named
		///             logger has a resource bundle set. </exception>
		/// <exception cref="NullPointerException"> if the name is null. </exception>

		// Synchronization is not required here. All synchronization for
		// adding a new Logger object is handled by LogManager.addLogger().
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Logger getLogger(String name, String resourceBundleName)
		public static Logger GetLogger(String name, String resourceBundleName)
		{
			Class callerClass = Reflection.CallerClass;
			Logger result = DemandLogger(name, resourceBundleName, callerClass);

			// MissingResourceException or IllegalArgumentException can be
			// thrown by setupResourceInfo().
			// We have to set the callers ClassLoader here in case demandLogger
			// above found a previously created Logger.  This can happen, for
			// example, if Logger.getLogger(name) is called and subsequently
			// Logger.getLogger(name, resourceBundleName) is called.  In this case
			// we won't necessarily have the correct classloader saved away, so
			// we need to set it here, too.

			result.SetupResourceInfo(resourceBundleName, callerClass);
			return result;
		}

		// package-private
		// Add a platform logger to the system context.
		// i.e. caller of sun.util.logging.PlatformLogger.getLogger
		internal static Logger GetPlatformLogger(String name)
		{
			LogManager manager = LogManager.LogManager;

			// all loggers in the system context will default to
			// the system logger's resource bundle
			Logger result = manager.DemandSystemLogger(name, SYSTEM_LOGGER_RB_NAME);
			return result;
		}

		/// <summary>
		/// Create an anonymous Logger.  The newly created Logger is not
		/// registered in the LogManager namespace.  There will be no
		/// access checks on updates to the logger.
		/// <para>
		/// This factory method is primarily intended for use from applets.
		/// Because the resulting Logger is anonymous it can be kept private
		/// by the creating class.  This removes the need for normal security
		/// checks, which in turn allows untrusted applet code to update
		/// the control state of the Logger.  For example an applet can do
		/// a setLevel or an addHandler on an anonymous Logger.
		/// </para>
		/// <para>
		/// Even although the new logger is anonymous, it is configured
		/// to have the root logger ("") as its parent.  This means that
		/// by default it inherits its effective level and handlers
		/// from the root logger. Changing its parent via the
		/// <seealso cref="#setParent(java.util.logging.Logger) setParent"/> method
		/// will still require the security permission specified by that method.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <returns> a newly created private Logger </returns>
		public static Logger AnonymousLogger
		{
			get
			{
				return GetAnonymousLogger(null);
			}
		}

		/// <summary>
		/// Create an anonymous Logger.  The newly created Logger is not
		/// registered in the LogManager namespace.  There will be no
		/// access checks on updates to the logger.
		/// <para>
		/// This factory method is primarily intended for use from applets.
		/// Because the resulting Logger is anonymous it can be kept private
		/// by the creating class.  This removes the need for normal security
		/// checks, which in turn allows untrusted applet code to update
		/// the control state of the Logger.  For example an applet can do
		/// a setLevel or an addHandler on an anonymous Logger.
		/// </para>
		/// <para>
		/// Even although the new logger is anonymous, it is configured
		/// to have the root logger ("") as its parent.  This means that
		/// by default it inherits its effective level and handlers
		/// from the root logger.  Changing its parent via the
		/// <seealso cref="#setParent(java.util.logging.Logger) setParent"/> method
		/// will still require the security permission specified by that method.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="resourceBundleName">  name of ResourceBundle to be used for localizing
		///                          messages for this logger.
		///          May be null if none of the messages require localization. </param>
		/// <returns> a newly created private Logger </returns>
		/// <exception cref="MissingResourceException"> if the resourceBundleName is non-null and
		///             no corresponding resource can be found. </exception>

		// Synchronization is not required here. All synchronization for
		// adding a new anonymous Logger object is handled by doSetParent().
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static Logger getAnonymousLogger(String resourceBundleName)
		public static Logger GetAnonymousLogger(String resourceBundleName)
		{
			LogManager manager = LogManager.LogManager;
			// cleanup some Loggers that have been GC'ed
			manager.DrainLoggerRefQueueBounded();
			Logger result = new Logger(null, resourceBundleName, Reflection.CallerClass, manager, false);
			result.Anonymous = true;
			Logger root = manager.GetLogger("");
			result.DoSetParent(root);
			return result;
		}

		/// <summary>
		/// Retrieve the localization resource bundle for this
		/// logger.
		/// This method will return a {@code ResourceBundle} that was either
		/// set by the {@link
		/// #setResourceBundle(java.util.ResourceBundle) setResourceBundle} method or
		/// <a href="#ResourceBundleMapping">mapped from the
		/// the resource bundle name</a> set via the {@link
		/// Logger#getLogger(java.lang.String, java.lang.String) getLogger} factory
		/// method for the current default locale.
		/// <br>Note that if the result is {@code null}, then the Logger will use a resource
		/// bundle or resource bundle name inherited from its parent.
		/// </summary>
		/// <returns> localization bundle (may be {@code null}) </returns>
		public virtual ResourceBundle ResourceBundle
		{
			get
			{
				return FindResourceBundle(ResourceBundleName, true);
			}
			set
			{
				CheckPermission();
    
				// Will throw NPE if value is null.
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String baseName = value.getBaseBundleName();
				String baseName = value.BaseBundleName;
    
				// value must have a name
				if (baseName == null || baseName.Empty)
				{
					throw new IllegalArgumentException("resource bundle must have a name");
				}
    
				lock (this)
				{
					LoggerBundle lb = LoggerBundle;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final boolean canReplaceResourceBundle = lb.resourceBundleName == null || lb.resourceBundleName.equals(baseName);
					bool canReplaceResourceBundle = lb.ResourceBundleName == null || lb.ResourceBundleName.Equals(baseName);
    
					if (!canReplaceResourceBundle)
					{
						throw new IllegalArgumentException("can't replace resource bundle");
					}
    
    
					LoggerBundle = LoggerBundle.Get(baseName, value);
				}
			}
		}

		/// <summary>
		/// Retrieve the localization resource bundle name for this
		/// logger.
		/// This is either the name specified through the {@link
		/// #getLogger(java.lang.String, java.lang.String) getLogger} factory method,
		/// or the <seealso cref="ResourceBundle#getBaseBundleName() base name"/> of the
		/// ResourceBundle set through {@link
		/// #setResourceBundle(java.util.ResourceBundle) setResourceBundle} method.
		/// <br>Note that if the result is {@code null}, then the Logger will use a resource
		/// bundle or resource bundle name inherited from its parent.
		/// </summary>
		/// <returns> localization bundle name (may be {@code null}) </returns>
		public virtual String ResourceBundleName
		{
			get
			{
				return LoggerBundle.ResourceBundleName;
			}
		}

		/// <summary>
		/// Set a filter to control output on this Logger.
		/// <P>
		/// After passing the initial "level" check, the Logger will
		/// call this Filter to check if a log record should really
		/// be published.
		/// </summary>
		/// <param name="newFilter">  a filter object (may be null) </param>
		/// <exception cref="SecurityException"> if a security manager exists,
		///          this logger is not anonymous, and the caller
		///          does not have LoggingPermission("control"). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFilter(Filter newFilter) throws SecurityException
		public virtual Filter Filter
		{
			set
			{
				CheckPermission();
				Filter_Renamed = value;
			}
			get
			{
				return Filter_Renamed;
			}
		}


		/// <summary>
		/// Log a LogRecord.
		/// <para>
		/// All the other logging methods in this class call through
		/// this method to actually perform any logging.  Subclasses can
		/// override this single method to capture all log activity.
		/// 
		/// </para>
		/// </summary>
		/// <param name="record"> the LogRecord to be published </param>
		public virtual void Log(LogRecord record)
		{
			if (!IsLoggable(record.Level))
			{
				return;
			}
			Filter theFilter = Filter_Renamed;
			if (theFilter != null && !theFilter.IsLoggable(record))
			{
				return;
			}

			// Post the LogRecord to all our Handlers, and then to
			// our parents' handlers, all the way up the tree.

			Logger logger = this;
			while (logger != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Handler[] loggerHandlers = isSystemLogger ? logger.accessCheckedHandlers() : logger.getHandlers();
				Handler[] loggerHandlers = IsSystemLogger ? logger.AccessCheckedHandlers() : logger.Handlers;

				foreach (Handler handler in loggerHandlers)
				{
					handler.Publish(record);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean useParentHdls = isSystemLogger ? logger.useParentHandlers : logger.getUseParentHandlers();
				bool useParentHdls = IsSystemLogger ? logger.UseParentHandlers_Renamed : logger.UseParentHandlers;

				if (!useParentHdls)
				{
					break;
				}

				logger = IsSystemLogger ? logger.Parent_Renamed : logger.Parent;
			}
		}

		// private support method for logging.
		// We fill in the logger name, resource bundle name, and
		// resource bundle and then call "void log(LogRecord)".
		private void DoLog(LogRecord lr)
		{
			lr.LoggerName = Name_Renamed;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LoggerBundle lb = getEffectiveLoggerBundle();
			LoggerBundle lb = EffectiveLoggerBundle;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ResourceBundle bundle = lb.userBundle;
			ResourceBundle bundle = lb.UserBundle;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String ebname = lb.resourceBundleName;
			String ebname = lb.ResourceBundleName;
			if (ebname != null && bundle != null)
			{
				lr.ResourceBundleName = ebname;
				lr.ResourceBundle = bundle;
			}
			Log(lr);
		}


		//================================================================
		// Start of convenience methods WITHOUT className and methodName
		//================================================================

		/// <summary>
		/// Log a message, with no arguments.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		public virtual void Log(Level level, String msg)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			DoLog(lr);
		}

		/// <summary>
		/// Log a message, which is only to be constructed if the logging level
		/// is such that the message will actually be logged.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the message is constructed by invoking the provided
		/// supplier function and forwarded to all the registered output
		/// Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message </param>
		public virtual void Log(Level level, Supplier<String> msgSupplier)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msgSupplier.Get());
			DoLog(lr);
		}

		/// <summary>
		/// Log a message, with one object parameter.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then a corresponding LogRecord is created and forwarded
		/// to all the registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// <param name="param1">  parameter to the message </param>
		public virtual void Log(Level level, String msg, Object param1)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			Object[] @params = new Object[] {param1};
			lr.Parameters = @params;
			DoLog(lr);
		}

		/// <summary>
		/// Log a message, with an array of object arguments.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then a corresponding LogRecord is created and forwarded
		/// to all the registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// <param name="params">  array of parameters to the message </param>
		public virtual void Log(Level level, String msg, Object[] @params)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.Parameters = @params;
			DoLog(lr);
		}

		/// <summary>
		/// Log a message, with associated Throwable information.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the given arguments are stored in a LogRecord
		/// which is forwarded to all registered output handlers.
		/// </para>
		/// <para>
		/// Note that the thrown argument is stored in the LogRecord thrown
		/// property, rather than the LogRecord parameters property.  Thus it is
		/// processed specially by output Formatters and is not treated
		/// as a formatting parameter to the LogRecord message property.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// <param name="thrown">  Throwable associated with log message. </param>
		public virtual void Log(Level level, String msg, Throwable thrown)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.Thrown = thrown;
			DoLog(lr);
		}

		/// <summary>
		/// Log a lazily constructed message, with associated Throwable information.
		/// <para>
		/// If the logger is currently enabled for the given message level then the
		/// message is constructed by invoking the provided supplier function. The
		/// message and the given <seealso cref="Throwable"/> are then stored in a {@link
		/// LogRecord} which is forwarded to all registered output handlers.
		/// </para>
		/// <para>
		/// Note that the thrown argument is stored in the LogRecord thrown
		/// property, rather than the LogRecord parameters property.  Thus it is
		/// processed specially by output Formatters and is not treated
		/// as a formatting parameter to the LogRecord message property.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="thrown">  Throwable associated with log message. </param>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Log(Level level, Throwable thrown, Supplier<String> msgSupplier)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msgSupplier.Get());
			lr.Thrown = thrown;
			DoLog(lr);
		}

		//================================================================
		// Start of convenience methods WITH className and methodName
		//================================================================

		/// <summary>
		/// Log a message, specifying source class and method,
		/// with no arguments.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		public virtual void Logp(Level level, String sourceClass, String sourceMethod, String msg)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			DoLog(lr);
		}

		/// <summary>
		/// Log a lazily constructed message, specifying source class and method,
		/// with no arguments.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the message is constructed by invoking the provided
		/// supplier function and forwarded to all the registered output
		/// Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Logp(Level level, String sourceClass, String sourceMethod, Supplier<String> msgSupplier)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msgSupplier.Get());
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			DoLog(lr);
		}

		/// <summary>
		/// Log a message, specifying source class and method,
		/// with a single object parameter to the log message.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then a corresponding LogRecord is created and forwarded
		/// to all the registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="msg">      The string message (or a key in the message catalog) </param>
		/// <param name="param1">    Parameter to the log message. </param>
		public virtual void Logp(Level level, String sourceClass, String sourceMethod, String msg, Object param1)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			Object[] @params = new Object[] {param1};
			lr.Parameters = @params;
			DoLog(lr);
		}

		/// <summary>
		/// Log a message, specifying source class and method,
		/// with an array of object arguments.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then a corresponding LogRecord is created and forwarded
		/// to all the registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// <param name="params">  Array of parameters to the message </param>
		public virtual void Logp(Level level, String sourceClass, String sourceMethod, String msg, Object[] @params)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			lr.Parameters = @params;
			DoLog(lr);
		}

		/// <summary>
		/// Log a message, specifying source class and method,
		/// with associated Throwable information.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the given arguments are stored in a LogRecord
		/// which is forwarded to all registered output handlers.
		/// </para>
		/// <para>
		/// Note that the thrown argument is stored in the LogRecord thrown
		/// property, rather than the LogRecord parameters property.  Thus it is
		/// processed specially by output Formatters and is not treated
		/// as a formatting parameter to the LogRecord message property.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// <param name="thrown">  Throwable associated with log message. </param>
		public virtual void Logp(Level level, String sourceClass, String sourceMethod, String msg, Throwable thrown)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			lr.Thrown = thrown;
			DoLog(lr);
		}

		/// <summary>
		/// Log a lazily constructed message, specifying source class and method,
		/// with associated Throwable information.
		/// <para>
		/// If the logger is currently enabled for the given message level then the
		/// message is constructed by invoking the provided supplier function. The
		/// message and the given <seealso cref="Throwable"/> are then stored in a {@link
		/// LogRecord} which is forwarded to all registered output handlers.
		/// </para>
		/// <para>
		/// Note that the thrown argument is stored in the LogRecord thrown
		/// property, rather than the LogRecord parameters property.  Thus it is
		/// processed specially by output Formatters and is not treated
		/// as a formatting parameter to the LogRecord message property.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="thrown">  Throwable associated with log message. </param>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Logp(Level level, String sourceClass, String sourceMethod, Throwable thrown, Supplier<String> msgSupplier)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msgSupplier.Get());
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			lr.Thrown = thrown;
			DoLog(lr);
		}


		//=========================================================================
		// Start of convenience methods WITH className, methodName and bundle name.
		//=========================================================================

		// Private support method for logging for "logrb" methods.
		// We fill in the logger name, resource bundle name, and
		// resource bundle and then call "void log(LogRecord)".
		private void DoLog(LogRecord lr, String rbname)
		{
			lr.LoggerName = Name_Renamed;
			if (rbname != null)
			{
				lr.ResourceBundleName = rbname;
				lr.ResourceBundle = FindResourceBundle(rbname, false);
			}
			Log(lr);
		}

		// Private support method for logging for "logrb" methods.
		private void DoLog(LogRecord lr, ResourceBundle rb)
		{
			lr.LoggerName = Name_Renamed;
			if (rb != null)
			{
				lr.ResourceBundleName = rb.BaseBundleName;
				lr.ResourceBundle = rb;
			}
			Log(lr);
		}

		/// <summary>
		/// Log a message, specifying source class, method, and resource bundle name
		/// with no arguments.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// The msg string is localized using the named resource bundle.  If the
		/// resource bundle name is null, or an empty String or invalid
		/// then the msg string is not localized.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="bundleName">     name of resource bundle to localize msg,
		///                         can be null </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// @deprecated Use {@link #logrb(java.util.logging.Level, java.lang.String,
		/// java.lang.String, java.util.ResourceBundle, java.lang.String,
		/// java.lang.Object...)} instead. 
		[Obsolete("Use {@link #logrb(java.util.logging.Level, java.lang.String,")]
		public virtual void Logrb(Level level, String sourceClass, String sourceMethod, String bundleName, String msg)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			DoLog(lr, bundleName);
		}

		/// <summary>
		/// Log a message, specifying source class, method, and resource bundle name,
		/// with a single object parameter to the log message.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then a corresponding LogRecord is created and forwarded
		/// to all the registered output Handler objects.
		/// </para>
		/// <para>
		/// The msg string is localized using the named resource bundle.  If the
		/// resource bundle name is null, or an empty String or invalid
		/// then the msg string is not localized.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="bundleName">     name of resource bundle to localize msg,
		///                         can be null </param>
		/// <param name="msg">      The string message (or a key in the message catalog) </param>
		/// <param name="param1">    Parameter to the log message. </param>
		/// @deprecated Use {@link #logrb(java.util.logging.Level, java.lang.String,
		///   java.lang.String, java.util.ResourceBundle, java.lang.String,
		///   java.lang.Object...)} instead 
		[Obsolete("Use {@link #logrb(java.util.logging.Level, java.lang.String,")]
		public virtual void Logrb(Level level, String sourceClass, String sourceMethod, String bundleName, String msg, Object param1)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			Object[] @params = new Object[] {param1};
			lr.Parameters = @params;
			DoLog(lr, bundleName);
		}

		/// <summary>
		/// Log a message, specifying source class, method, and resource bundle name,
		/// with an array of object arguments.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then a corresponding LogRecord is created and forwarded
		/// to all the registered output Handler objects.
		/// </para>
		/// <para>
		/// The msg string is localized using the named resource bundle.  If the
		/// resource bundle name is null, or an empty String or invalid
		/// then the msg string is not localized.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="bundleName">     name of resource bundle to localize msg,
		///                         can be null. </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// <param name="params">  Array of parameters to the message </param>
		/// @deprecated Use {@link #logrb(java.util.logging.Level, java.lang.String,
		///      java.lang.String, java.util.ResourceBundle, java.lang.String,
		///      java.lang.Object...)} instead. 
		[Obsolete("Use {@link #logrb(java.util.logging.Level, java.lang.String,")]
		public virtual void Logrb(Level level, String sourceClass, String sourceMethod, String bundleName, String msg, Object[] @params)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			lr.Parameters = @params;
			DoLog(lr, bundleName);
		}

		/// <summary>
		/// Log a message, specifying source class, method, and resource bundle,
		/// with an optional list of message parameters.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then a corresponding LogRecord is created and forwarded
		/// to all the registered output Handler objects.
		/// </para>
		/// <para>
		/// The {@code msg} string is localized using the given resource bundle.
		/// If the resource bundle is {@code null}, then the {@code msg} string is not
		/// localized.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    Name of the class that issued the logging request </param>
		/// <param name="sourceMethod">   Name of the method that issued the logging request </param>
		/// <param name="bundle">         Resource bundle to localize {@code msg},
		///                         can be {@code null}. </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// <param name="params">  Parameters to the message (optional, may be none).
		/// @since 1.8 </param>
		public virtual void Logrb(Level level, String sourceClass, String sourceMethod, ResourceBundle bundle, String msg, params Object[] @params)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			if (@params != null && @params.Length != 0)
			{
				lr.Parameters = @params;
			}
			DoLog(lr, bundle);
		}

		/// <summary>
		/// Log a message, specifying source class, method, and resource bundle name,
		/// with associated Throwable information.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the given arguments are stored in a LogRecord
		/// which is forwarded to all registered output handlers.
		/// </para>
		/// <para>
		/// The msg string is localized using the named resource bundle.  If the
		/// resource bundle name is null, or an empty String or invalid
		/// then the msg string is not localized.
		/// </para>
		/// <para>
		/// Note that the thrown argument is stored in the LogRecord thrown
		/// property, rather than the LogRecord parameters property.  Thus it is
		/// processed specially by output Formatters and is not treated
		/// as a formatting parameter to the LogRecord message property.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that issued the logging request </param>
		/// <param name="bundleName">     name of resource bundle to localize msg,
		///                         can be null </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// <param name="thrown">  Throwable associated with log message. </param>
		/// @deprecated Use {@link #logrb(java.util.logging.Level, java.lang.String,
		///     java.lang.String, java.util.ResourceBundle, java.lang.String,
		///     java.lang.Throwable)} instead. 
		[Obsolete("Use {@link #logrb(java.util.logging.Level, java.lang.String,")]
		public virtual void Logrb(Level level, String sourceClass, String sourceMethod, String bundleName, String msg, Throwable thrown)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			lr.Thrown = thrown;
			DoLog(lr, bundleName);
		}

		/// <summary>
		/// Log a message, specifying source class, method, and resource bundle,
		/// with associated Throwable information.
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the given arguments are stored in a LogRecord
		/// which is forwarded to all registered output handlers.
		/// </para>
		/// <para>
		/// The {@code msg} string is localized using the given resource bundle.
		/// If the resource bundle is {@code null}, then the {@code msg} string is not
		/// localized.
		/// </para>
		/// <para>
		/// Note that the thrown argument is stored in the LogRecord thrown
		/// property, rather than the LogRecord parameters property.  Thus it is
		/// processed specially by output Formatters and is not treated
		/// as a formatting parameter to the LogRecord message property.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		/// <param name="sourceClass">    Name of the class that issued the logging request </param>
		/// <param name="sourceMethod">   Name of the method that issued the logging request </param>
		/// <param name="bundle">         Resource bundle to localize {@code msg},
		///                         can be {@code null} </param>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		/// <param name="thrown">  Throwable associated with the log message.
		/// @since 1.8 </param>
		public virtual void Logrb(Level level, String sourceClass, String sourceMethod, ResourceBundle bundle, String msg, Throwable thrown)
		{
			if (!IsLoggable(level))
			{
				return;
			}
			LogRecord lr = new LogRecord(level, msg);
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			lr.Thrown = thrown;
			DoLog(lr, bundle);
		}

		//======================================================================
		// Start of convenience methods for logging method entries and returns.
		//======================================================================

		/// <summary>
		/// Log a method entry.
		/// <para>
		/// This is a convenience method that can be used to log entry
		/// to a method.  A LogRecord with message "ENTRY", log level
		/// FINER, and the given sourceMethod and sourceClass is logged.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that is being entered </param>
		public virtual void Entering(String sourceClass, String sourceMethod)
		{
			Logp(Level.FINER, sourceClass, sourceMethod, "ENTRY");
		}

		/// <summary>
		/// Log a method entry, with one parameter.
		/// <para>
		/// This is a convenience method that can be used to log entry
		/// to a method.  A LogRecord with message "ENTRY {0}", log level
		/// FINER, and the given sourceMethod, sourceClass, and parameter
		/// is logged.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that is being entered </param>
		/// <param name="param1">         parameter to the method being entered </param>
		public virtual void Entering(String sourceClass, String sourceMethod, Object param1)
		{
			Logp(Level.FINER, sourceClass, sourceMethod, "ENTRY {0}", param1);
		}

		/// <summary>
		/// Log a method entry, with an array of parameters.
		/// <para>
		/// This is a convenience method that can be used to log entry
		/// to a method.  A LogRecord with message "ENTRY" (followed by a
		/// format {N} indicator for each entry in the parameter array),
		/// log level FINER, and the given sourceMethod, sourceClass, and
		/// parameters is logged.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of method that is being entered </param>
		/// <param name="params">         array of parameters to the method being entered </param>
		public virtual void Entering(String sourceClass, String sourceMethod, Object[] @params)
		{
			String msg = "ENTRY";
			if (@params == null)
			{
			   Logp(Level.FINER, sourceClass, sourceMethod, msg);
			   return;
			}
			if (!IsLoggable(Level.FINER))
			{
				return;
			}
			for (int i = 0; i < @params.Length; i++)
			{
				msg = msg + " {" + i + "}";
			}
			Logp(Level.FINER, sourceClass, sourceMethod, msg, @params);
		}

		/// <summary>
		/// Log a method return.
		/// <para>
		/// This is a convenience method that can be used to log returning
		/// from a method.  A LogRecord with message "RETURN", log level
		/// FINER, and the given sourceMethod and sourceClass is logged.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of the method </param>
		public virtual void Exiting(String sourceClass, String sourceMethod)
		{
			Logp(Level.FINER, sourceClass, sourceMethod, "RETURN");
		}


		/// <summary>
		/// Log a method return, with result object.
		/// <para>
		/// This is a convenience method that can be used to log returning
		/// from a method.  A LogRecord with message "RETURN {0}", log level
		/// FINER, and the gives sourceMethod, sourceClass, and result
		/// object is logged.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">   name of the method </param>
		/// <param name="result">  Object that is being returned </param>
		public virtual void Exiting(String sourceClass, String sourceMethod, Object result)
		{
			Logp(Level.FINER, sourceClass, sourceMethod, "RETURN {0}", result);
		}

		/// <summary>
		/// Log throwing an exception.
		/// <para>
		/// This is a convenience method to log that a method is
		/// terminating by throwing an exception.  The logging is done
		/// using the FINER level.
		/// </para>
		/// <para>
		/// If the logger is currently enabled for the given message
		/// level then the given arguments are stored in a LogRecord
		/// which is forwarded to all registered output handlers.  The
		/// LogRecord's message is set to "THROW".
		/// </para>
		/// <para>
		/// Note that the thrown argument is stored in the LogRecord thrown
		/// property, rather than the LogRecord parameters property.  Thus it is
		/// processed specially by output Formatters and is not treated
		/// as a formatting parameter to the LogRecord message property.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="sourceClass">    name of class that issued the logging request </param>
		/// <param name="sourceMethod">  name of the method. </param>
		/// <param name="thrown">  The Throwable that is being thrown. </param>
		public virtual void Throwing(String sourceClass, String sourceMethod, Throwable thrown)
		{
			if (!IsLoggable(Level.FINER))
			{
				return;
			}
			LogRecord lr = new LogRecord(Level.FINER, "THROW");
			lr.SourceClassName = sourceClass;
			lr.SourceMethodName = sourceMethod;
			lr.Thrown = thrown;
			DoLog(lr);
		}

		//=======================================================================
		// Start of simple convenience methods using level names as method names
		//=======================================================================

		/// <summary>
		/// Log a SEVERE message.
		/// <para>
		/// If the logger is currently enabled for the SEVERE message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		public virtual void Severe(String msg)
		{
			Log(Level.SEVERE, msg);
		}

		/// <summary>
		/// Log a WARNING message.
		/// <para>
		/// If the logger is currently enabled for the WARNING message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		public virtual void Warning(String msg)
		{
			Log(Level.WARNING, msg);
		}

		/// <summary>
		/// Log an INFO message.
		/// <para>
		/// If the logger is currently enabled for the INFO message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		public virtual void Info(String msg)
		{
			Log(Level.INFO, msg);
		}

		/// <summary>
		/// Log a CONFIG message.
		/// <para>
		/// If the logger is currently enabled for the CONFIG message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		public virtual void Config(String msg)
		{
			Log(Level.CONFIG, msg);
		}

		/// <summary>
		/// Log a FINE message.
		/// <para>
		/// If the logger is currently enabled for the FINE message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		public virtual void Fine(String msg)
		{
			Log(Level.FINE, msg);
		}

		/// <summary>
		/// Log a FINER message.
		/// <para>
		/// If the logger is currently enabled for the FINER message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		public virtual void Finer(String msg)
		{
			Log(Level.FINER, msg);
		}

		/// <summary>
		/// Log a FINEST message.
		/// <para>
		/// If the logger is currently enabled for the FINEST message
		/// level then the given message is forwarded to all the
		/// registered output Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msg">     The string message (or a key in the message catalog) </param>
		public virtual void Finest(String msg)
		{
			Log(Level.FINEST, msg);
		}

		//=======================================================================
		// Start of simple convenience methods using level names as method names
		// and use Supplier<String>
		//=======================================================================

		/// <summary>
		/// Log a SEVERE message, which is only to be constructed if the logging
		/// level is such that the message will actually be logged.
		/// <para>
		/// If the logger is currently enabled for the SEVERE message
		/// level then the message is constructed by invoking the provided
		/// supplier function and forwarded to all the registered output
		/// Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Severe(Supplier<String> msgSupplier)
		{
			Log(Level.SEVERE, msgSupplier);
		}

		/// <summary>
		/// Log a WARNING message, which is only to be constructed if the logging
		/// level is such that the message will actually be logged.
		/// <para>
		/// If the logger is currently enabled for the WARNING message
		/// level then the message is constructed by invoking the provided
		/// supplier function and forwarded to all the registered output
		/// Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Warning(Supplier<String> msgSupplier)
		{
			Log(Level.WARNING, msgSupplier);
		}

		/// <summary>
		/// Log a INFO message, which is only to be constructed if the logging
		/// level is such that the message will actually be logged.
		/// <para>
		/// If the logger is currently enabled for the INFO message
		/// level then the message is constructed by invoking the provided
		/// supplier function and forwarded to all the registered output
		/// Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Info(Supplier<String> msgSupplier)
		{
			Log(Level.INFO, msgSupplier);
		}

		/// <summary>
		/// Log a CONFIG message, which is only to be constructed if the logging
		/// level is such that the message will actually be logged.
		/// <para>
		/// If the logger is currently enabled for the CONFIG message
		/// level then the message is constructed by invoking the provided
		/// supplier function and forwarded to all the registered output
		/// Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Config(Supplier<String> msgSupplier)
		{
			Log(Level.CONFIG, msgSupplier);
		}

		/// <summary>
		/// Log a FINE message, which is only to be constructed if the logging
		/// level is such that the message will actually be logged.
		/// <para>
		/// If the logger is currently enabled for the FINE message
		/// level then the message is constructed by invoking the provided
		/// supplier function and forwarded to all the registered output
		/// Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Fine(Supplier<String> msgSupplier)
		{
			Log(Level.FINE, msgSupplier);
		}

		/// <summary>
		/// Log a FINER message, which is only to be constructed if the logging
		/// level is such that the message will actually be logged.
		/// <para>
		/// If the logger is currently enabled for the FINER message
		/// level then the message is constructed by invoking the provided
		/// supplier function and forwarded to all the registered output
		/// Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Finer(Supplier<String> msgSupplier)
		{
			Log(Level.FINER, msgSupplier);
		}

		/// <summary>
		/// Log a FINEST message, which is only to be constructed if the logging
		/// level is such that the message will actually be logged.
		/// <para>
		/// If the logger is currently enabled for the FINEST message
		/// level then the message is constructed by invoking the provided
		/// supplier function and forwarded to all the registered output
		/// Handler objects.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="msgSupplier">   A function, which when called, produces the
		///                        desired log message
		/// @since   1.8 </param>
		public virtual void Finest(Supplier<String> msgSupplier)
		{
			Log(Level.FINEST, msgSupplier);
		}

		//================================================================
		// End of convenience methods
		//================================================================

		/// <summary>
		/// Set the log level specifying which message levels will be
		/// logged by this logger.  Message levels lower than this
		/// value will be discarded.  The level value Level.OFF
		/// can be used to turn off logging.
		/// <para>
		/// If the new level is null, it means that this node should
		/// inherit its level from its nearest ancestor with a specific
		/// (non-null) level value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newLevel">   the new value for the log level (may be null) </param>
		/// <exception cref="SecurityException"> if a security manager exists,
		///          this logger is not anonymous, and the caller
		///          does not have LoggingPermission("control"). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setLevel(Level newLevel) throws SecurityException
		public virtual Level Level
		{
			set
			{
				CheckPermission();
				lock (TreeLock)
				{
					LevelObject = value;
					UpdateEffectiveLevel();
				}
			}
			get
			{
				return LevelObject;
			}
		}

		internal bool LevelInitialized
		{
			get
			{
				return LevelObject != null;
			}
		}


		/// <summary>
		/// Check if a message of the given level would actually be logged
		/// by this logger.  This check is based on the Loggers effective level,
		/// which may be inherited from its parent.
		/// </summary>
		/// <param name="level">   a message logging level </param>
		/// <returns>  true if the given message level is currently being logged. </returns>
		public virtual bool IsLoggable(Level level)
		{
			if (level.IntValue() < LevelValue || LevelValue == OffValue)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Get the name for this logger. </summary>
		/// <returns> logger name.  Will be null for anonymous Loggers. </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Add a log Handler to receive logging messages.
		/// <para>
		/// By default, Loggers also send their output to their parent logger.
		/// Typically the root Logger is configured with a set of Handlers
		/// that essentially act as default handlers for all loggers.
		/// 
		/// </para>
		/// </summary>
		/// <param name="handler"> a logging Handler </param>
		/// <exception cref="SecurityException"> if a security manager exists,
		///          this logger is not anonymous, and the caller
		///          does not have LoggingPermission("control"). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHandler(Handler handler) throws SecurityException
		public virtual void AddHandler(Handler handler)
		{
			// Check for null handler
			handler.GetType();
			CheckPermission();
			Handlers_Renamed.Add(handler);
		}

		/// <summary>
		/// Remove a log Handler.
		/// <P>
		/// Returns silently if the given Handler is not found or is null
		/// </summary>
		/// <param name="handler"> a logging Handler </param>
		/// <exception cref="SecurityException"> if a security manager exists,
		///          this logger is not anonymous, and the caller
		///          does not have LoggingPermission("control"). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeHandler(Handler handler) throws SecurityException
		public virtual void RemoveHandler(Handler handler)
		{
			CheckPermission();
			if (handler == null)
			{
				return;
			}
			Handlers_Renamed.Remove(handler);
		}

		/// <summary>
		/// Get the Handlers associated with this logger.
		/// <para>
		/// </para>
		/// </summary>
		/// <returns>  an array of all registered Handlers </returns>
		public virtual Handler[] Handlers
		{
			get
			{
				return AccessCheckedHandlers();
			}
		}

		// This method should ideally be marked final - but unfortunately
		// it needs to be overridden by LogManager.RootLogger
		internal virtual Handler[] AccessCheckedHandlers()
		{
			return Handlers_Renamed.ToArray(EmptyHandlers);
		}

		/// <summary>
		/// Specify whether or not this logger should send its output
		/// to its parent Logger.  This means that any LogRecords will
		/// also be written to the parent's Handlers, and potentially
		/// to its parent, recursively up the namespace.
		/// </summary>
		/// <param name="useParentHandlers">   true if output is to be sent to the
		///          logger's parent. </param>
		/// <exception cref="SecurityException"> if a security manager exists,
		///          this logger is not anonymous, and the caller
		///          does not have LoggingPermission("control"). </exception>
		public virtual bool UseParentHandlers
		{
			set
			{
				CheckPermission();
				this.UseParentHandlers_Renamed = value;
			}
			get
			{
				return UseParentHandlers_Renamed;
			}
		}


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static java.util.ResourceBundle findSystemResourceBundle(final java.util.Locale locale)
		private static ResourceBundle FindSystemResourceBundle(Locale locale)
		{
			// the resource bundle is in a restricted package
			return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(locale));
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<ResourceBundle>
		{
			private Locale Locale;

			public PrivilegedActionAnonymousInnerClassHelper(Locale locale)
			{
				this.Locale = locale;
			}

			public virtual ResourceBundle Run()
			{
				try
				{
					return ResourceBundle.GetBundle(SYSTEM_LOGGER_RB_NAME, Locale, ClassLoader.SystemClassLoader);
				}
				catch (MissingResourceException e)
				{
					throw new InternalError(e.ToString());
				}
			}
		}

		/// <summary>
		/// Private utility method to map a resource bundle name to an
		/// actual resource bundle, using a simple one-entry cache.
		/// Returns null for a null name.
		/// May also return null if we can't find the resource bundle and
		/// there is no suitable previous cached value.
		/// </summary>
		/// <param name="name"> the ResourceBundle to locate </param>
		/// <param name="userCallersClassLoader"> if true search using the caller's ClassLoader </param>
		/// <returns> ResourceBundle specified by name or null if not found </returns>
		private ResourceBundle FindResourceBundle(String name, bool useCallersClassLoader)
		{
			lock (this)
			{
				// For all lookups, we first check the thread context class loader
				// if it is set.  If not, we use the system classloader.  If we
				// still haven't found it we use the callersClassLoaderRef if it
				// is set and useCallersClassLoader is true.  We set
				// callersClassLoaderRef initially upon creating the logger with a
				// non-null resource bundle name.
        
				// Return a null bundle for a null name.
				if (name == null)
				{
					return null;
				}
        
				Locale currentLocale = Locale.Default;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LoggerBundle lb = loggerBundle;
				LoggerBundle lb = LoggerBundle;
        
				// Normally we should hit on our simple one entry cache.
				if (lb.UserBundle != null && name.Equals(lb.ResourceBundleName))
				{
					return lb.UserBundle;
				}
				else if (Catalog != null && currentLocale.Equals(CatalogLocale) && name.Equals(CatalogName))
				{
					return Catalog;
				}
        
				if (name.Equals(SYSTEM_LOGGER_RB_NAME))
				{
					Catalog = FindSystemResourceBundle(currentLocale);
					CatalogName = name;
					CatalogLocale = currentLocale;
					return Catalog;
				}
        
				// Use the thread's context ClassLoader.  If there isn't one, use the
				// {@linkplain java.lang.ClassLoader#getSystemClassLoader() system ClassLoader}.
				ClassLoader cl = Thread.CurrentThread.ContextClassLoader;
				if (cl == null)
				{
					cl = ClassLoader.SystemClassLoader;
				}
				try
				{
					Catalog = ResourceBundle.GetBundle(name, currentLocale, cl);
					CatalogName = name;
					CatalogLocale = currentLocale;
					return Catalog;
				}
				catch (MissingResourceException)
				{
					// We can't find the ResourceBundle in the default
					// ClassLoader.  Drop through.
				}
        
				if (useCallersClassLoader)
				{
					// Try with the caller's ClassLoader
					ClassLoader callersClassLoader = CallersClassLoader;
        
					if (callersClassLoader == null || callersClassLoader == cl)
					{
						return null;
					}
        
					try
					{
						Catalog = ResourceBundle.GetBundle(name, currentLocale, callersClassLoader);
						CatalogName = name;
						CatalogLocale = currentLocale;
						return Catalog;
					}
					catch (MissingResourceException)
					{
						return null; // no luck
					}
				}
				else
				{
					return null;
				}
			}
		}

		// Private utility method to initialize our one entry
		// resource bundle name cache and the callers ClassLoader
		// Note: for consistency reasons, we are careful to check
		// that a suitable ResourceBundle exists before setting the
		// resourceBundleName field.
		// Synchronized to prevent races in setting the fields.
		private void SetupResourceInfo(String name, Class callersClass)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LoggerBundle lb = loggerBundle;
				LoggerBundle lb = LoggerBundle;
				if (lb.ResourceBundleName != null)
				{
					// this Logger already has a ResourceBundle
        
					if (lb.ResourceBundleName.Equals(name))
					{
						// the names match so there is nothing more to do
						return;
					}
        
					// cannot change ResourceBundles once they are set
					throw new IllegalArgumentException(lb.ResourceBundleName + " != " + name);
				}
        
				if (name == null)
				{
					return;
				}
        
				CallersClassLoaderRef = callersClass;
				if (IsSystemLogger && CallersClassLoader != null)
				{
					CheckPermission();
				}
				if (FindResourceBundle(name, true) == null)
				{
					// We've failed to find an expected ResourceBundle.
					// unset the caller's ClassLoader since we were unable to find the
					// the bundle using it
					this.CallersClassLoaderRef_Renamed = null;
					throw new MissingResourceException("Can't find " + name + " bundle", name, "");
				}
        
				// if lb.userBundle is not null we won't reach this line.
				Debug.Assert(lb.UserBundle == null);
				LoggerBundle = LoggerBundle.Get(name, null);
			}
		}


		/// <summary>
		/// Return the parent for this Logger.
		/// <para>
		/// This method returns the nearest extant parent in the namespace.
		/// Thus if a Logger is called "a.b.c.d", and a Logger called "a.b"
		/// has been created but no logger "a.b.c" exists, then a call of
		/// getParent on the Logger "a.b.c.d" will return the Logger "a.b".
		/// </para>
		/// <para>
		/// The result will be null if it is called on the root Logger
		/// in the namespace.
		/// 
		/// </para>
		/// </summary>
		/// <returns> nearest existing parent Logger </returns>
		public virtual Logger Parent
		{
			get
			{
				// Note: this used to be synchronized on treeLock.  However, this only
				// provided memory semantics, as there was no guarantee that the caller
				// would synchronize on treeLock (in fact, there is no way for external
				// callers to so synchronize).  Therefore, we have made parent volatile
				// instead.
				return Parent_Renamed;
			}
			set
			{
				if (value == null)
				{
					throw new NullPointerException();
				}
    
				// check permission for all loggers, including anonymous loggers
				if (Manager == null)
				{
					Manager = LogManager.LogManager;
				}
				Manager.CheckPermission();
    
				DoSetParent(value);
			}
		}


		// Private method to do the work for parenting a child
		// Logger onto a parent logger.
		private void DoSetParent(Logger newParent)
		{

			// System.err.println("doSetParent \"" + getName() + "\" \""
			//                              + newParent.getName() + "\"");

			lock (TreeLock)
			{

				// Remove ourself from any previous parent.
				LogManager.LoggerWeakRef @ref = null;
				if (Parent_Renamed != null)
				{
					// assert parent.kids != null;
					for (IEnumerator<LogManager.LoggerWeakRef> iter = Parent_Renamed.Kids.Iterator(); iter.MoveNext();)
					{
						@ref = iter.Current;
						Logger kid = @ref.get();
						if (kid == this)
						{
							// ref is used down below to complete the reparenting
							iter.remove();
							break;
						}
						else
						{
							@ref = null;
						}
					}
					// We have now removed ourself from our parents' kids.
				}

				// Set our new parent.
				Parent_Renamed = newParent;
				if (Parent_Renamed.Kids == null)
				{
					Parent_Renamed.Kids = new List<>(2);
				}
				if (@ref == null)
				{
					// we didn't have a previous parent
					@ref = new java.util.logging.LogManager.LoggerWeakRef(Manager, this);
				}
				@ref.ParentRef = new WeakReference<>(Parent_Renamed);
				Parent_Renamed.Kids.Add(@ref);

				// As a result of the reparenting, the effective level
				// may have changed for us and our children.
				UpdateEffectiveLevel();

			}
		}

		// Package-level method.
		// Remove the weak reference for the specified child Logger from the
		// kid list. We should only be called from LoggerWeakRef.dispose().
		internal void RemoveChildLogger(LogManager.LoggerWeakRef child)
		{
			lock (TreeLock)
			{
				for (IEnumerator<LogManager.LoggerWeakRef> iter = Kids.Iterator(); iter.MoveNext();)
				{
					LogManager.LoggerWeakRef @ref = iter.Current;
					if (@ref == child)
					{
						iter.remove();
						return;
					}
				}
			}
		}

		// Recalculate the effective level for this node and
		// recursively for our children.

		private void UpdateEffectiveLevel()
		{
			// assert Thread.holdsLock(treeLock);

			// Figure out our current effective level.
			int newLevelValue;
			if (LevelObject != null)
			{
				newLevelValue = LevelObject.IntValue();
			}
			else
			{
				if (Parent_Renamed != null)
				{
					newLevelValue = Parent_Renamed.LevelValue;
				}
				else
				{
					// This may happen during initialization.
					newLevelValue = Level.INFO.IntValue();
				}
			}

			// If our effective value hasn't changed, we're done.
			if (LevelValue == newLevelValue)
			{
				return;
			}

			LevelValue = newLevelValue;

			// System.err.println("effective level: \"" + getName() + "\" := " + level);

			// Recursively update the level on each of our kids.
			if (Kids != null)
			{
				for (int i = 0; i < Kids.Size(); i++)
				{
					LogManager.LoggerWeakRef @ref = Kids.Get(i);
					Logger kid = @ref.get();
					if (kid != null)
					{
						kid.UpdateEffectiveLevel();
					}
				}
			}
		}


		// Private method to get the potentially inherited
		// resource bundle and resource bundle name for this Logger.
		// This method never returns null.
		private LoggerBundle EffectiveLoggerBundle
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final LoggerBundle lb = loggerBundle;
				LoggerBundle lb = LoggerBundle;
				if (lb.SystemBundle)
				{
					return SYSTEM_BUNDLE;
				}
    
				// first take care of this logger
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.ResourceBundle b = getResourceBundle();
				ResourceBundle b = ResourceBundle;
				if (b != null && b == lb.UserBundle)
				{
					return lb;
				}
				else if (b != null)
				{
					// either lb.userBundle is null or getResourceBundle() is
					// overriden
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String rbName = getResourceBundleName();
					String rbName = ResourceBundleName;
					return LoggerBundle.Get(rbName, b);
				}
    
				// no resource bundle was specified on this logger, look up the
				// parent stack.
				Logger target = this.Parent_Renamed;
				while (target != null)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final LoggerBundle trb = target.loggerBundle;
					LoggerBundle trb = target.LoggerBundle;
					if (trb.SystemBundle)
					{
						return SYSTEM_BUNDLE;
					}
					if (trb.UserBundle != null)
					{
						return trb;
					}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String rbName = isSystemLogger ? (target.isSystemLogger ? trb.resourceBundleName : null) : target.getResourceBundleName();
					String rbName = IsSystemLogger ? (target.IsSystemLogger ? trb.ResourceBundleName : null) : target.ResourceBundleName;
						// ancestor of a system logger is expected to be a system logger.
						// ignore resource bundle name if it's not.
					if (rbName != null)
					{
						return LoggerBundle.Get(rbName, FindResourceBundle(rbName, true));
					}
					target = IsSystemLogger ? target.Parent_Renamed : target.Parent;
				}
				return NO_RESOURCE_BUNDLE;
			}
		}

	}

}