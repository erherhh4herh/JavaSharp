using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using JavaAWTAccess = sun.misc.JavaAWTAccess;
	using SharedSecrets = sun.misc.SharedSecrets;

	/// <summary>
	/// There is a single global LogManager object that is used to
	/// maintain a set of shared state about Loggers and log services.
	/// <para>
	/// This LogManager object:
	/// <ul>
	/// <li> Manages a hierarchical namespace of Logger objects.  All
	///      named Loggers are stored in this namespace.
	/// <li> Manages a set of logging control properties.  These are
	///      simple key-value pairs that can be used by Handlers and
	///      other logging objects to configure themselves.
	/// </ul>
	/// </para>
	/// <para>
	/// The global LogManager object can be retrieved using LogManager.getLogManager().
	/// The LogManager object is created during class initialization and
	/// cannot subsequently be changed.
	/// </para>
	/// <para>
	/// At startup the LogManager class is located using the
	/// java.util.logging.manager system property.
	/// </para>
	/// <para>
	/// The LogManager defines two optional system properties that allow control over
	/// the initial configuration:
	/// <ul>
	/// <li>"java.util.logging.config.class"
	/// <li>"java.util.logging.config.file"
	/// </ul>
	/// These two properties may be specified on the command line to the "java"
	/// command, or as system property definitions passed to JNI_CreateJavaVM.
	/// </para>
	/// <para>
	/// If the "java.util.logging.config.class" property is set, then the
	/// property value is treated as a class name.  The given class will be
	/// loaded, an object will be instantiated, and that object's constructor
	/// is responsible for reading in the initial configuration.  (That object
	/// may use other system properties to control its configuration.)  The
	/// alternate configuration class can use <tt>readConfiguration(InputStream)</tt>
	/// to define properties in the LogManager.
	/// </para>
	/// <para>
	/// If "java.util.logging.config.class" property is <b>not</b> set,
	/// then the "java.util.logging.config.file" system property can be used
	/// to specify a properties file (in java.util.Properties format). The
	/// initial logging configuration will be read from this file.
	/// </para>
	/// <para>
	/// If neither of these properties is defined then the LogManager uses its
	/// default configuration. The default configuration is typically loaded from the
	/// properties file "{@code lib/logging.properties}" in the Java installation
	/// directory.
	/// </para>
	/// <para>
	/// The properties for loggers and Handlers will have names starting
	/// with the dot-separated name for the handler or logger.
	/// </para>
	/// <para>
	/// The global logging properties may include:
	/// <ul>
	/// <li>A property "handlers".  This defines a whitespace or comma separated
	/// list of class names for handler classes to load and register as
	/// handlers on the root Logger (the Logger named "").  Each class
	/// name must be for a Handler class which has a default constructor.
	/// Note that these Handlers may be created lazily, when they are
	/// first used.
	/// 
	/// <li>A property "&lt;logger&gt;.handlers". This defines a whitespace or
	/// comma separated list of class names for handlers classes to
	/// load and register as handlers to the specified logger. Each class
	/// name must be for a Handler class which has a default constructor.
	/// Note that these Handlers may be created lazily, when they are
	/// first used.
	/// 
	/// <li>A property "&lt;logger&gt;.useParentHandlers". This defines a boolean
	/// value. By default every logger calls its parent in addition to
	/// handling the logging message itself, this often result in messages
	/// being handled by the root logger as well. When setting this property
	/// to false a Handler needs to be configured for this logger otherwise
	/// no logging messages are delivered.
	/// 
	/// <li>A property "config".  This property is intended to allow
	/// arbitrary configuration code to be run.  The property defines a
	/// whitespace or comma separated list of class names.  A new instance will be
	/// created for each named class.  The default constructor of each class
	/// may execute arbitrary code to update the logging configuration, such as
	/// setting logger levels, adding handlers, adding filters, etc.
	/// </ul>
	/// </para>
	/// <para>
	/// Note that all classes loaded during LogManager configuration are
	/// first searched on the system class path before any user class path.
	/// That includes the LogManager class, any config classes, and any
	/// handler classes.
	/// </para>
	/// <para>
	/// Loggers are organized into a naming hierarchy based on their
	/// dot separated names.  Thus "a.b.c" is a child of "a.b", but
	/// "a.b1" and a.b2" are peers.
	/// </para>
	/// <para>
	/// All properties whose names end with ".level" are assumed to define
	/// log levels for Loggers.  Thus "foo.level" defines a log level for
	/// the logger called "foo" and (recursively) for any of its children
	/// in the naming hierarchy.  Log Levels are applied in the order they
	/// are defined in the properties file.  Thus level settings for child
	/// nodes in the tree should come after settings for their parents.
	/// The property name ".level" can be used to set the level for the
	/// root of the tree.
	/// </para>
	/// <para>
	/// All methods on the LogManager object are multi-thread safe.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	public class LogManager
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			SystemContext_Renamed = new SystemLoggerContext(this);
			UserContext_Renamed = new LoggerContext(this);
		}

		// The global LogManager object
		private static readonly LogManager Manager;

		// 'props' is assigned within a lock but accessed without it.
		// Declaring it volatile makes sure that another thread will not
		// be able to see a partially constructed 'props' object.
		// (seeing a partially constructed 'props' object can result in
		// NPE being thrown in Hashtable.get(), because it leaves the door
		// open for props.getProperties() to be called before the construcor
		// of Hashtable is actually completed).
		private volatile Properties Props = new Properties();
		private static readonly Level DefaultLevel = Level.INFO;

		// The map of the registered listeners. The map value is the registration
		// count to allow for cases where the same listener is registered many times.
		private readonly Map<Object, Integer> ListenerMap = new HashMap<Object, Integer>();

		// LoggerContext for system loggers and user loggers
		private LoggerContext SystemContext_Renamed;
		private LoggerContext UserContext_Renamed;
		// non final field - make it volatile to make sure that other threads
		// will see the new value once ensureLogManagerInitialized() has finished
		// executing.
		private volatile Logger RootLogger;
		// Have we done the primordial reading of the configuration file?
		// (Must be done after a suitable amount of java.lang.System
		// initialization has been done)
		private volatile bool ReadPrimordialConfiguration_Renamed;
		// Have we initialized global (root) handlers yet?
		// This gets set to false in readConfiguration
		private bool InitializedGlobalHandlers = true;
		// True if JVM death is imminent and the exit hook has been called.
		private bool DeathImminent;

		static LogManager()
		{
			Manager = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper());
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<LogManager>
		{
			public PrivilegedActionAnonymousInnerClassHelper()
			{
			}

			public virtual LogManager Run()
			{
				LogManager mgr = null;
				String cname = null;
				try
				{
					cname = System.getProperty("java.util.logging.manager");
					if (cname != null)
					{
						try
						{
							Class clz = ClassLoader.SystemClassLoader.LoadClass(cname);
							mgr = (LogManager) clz.NewInstance();
						}
						catch (ClassNotFoundException)
						{
							Class clz = Thread.CurrentThread.ContextClassLoader.loadClass(cname);
							mgr = (LogManager) clz.NewInstance();
						}
					}
				}
				catch (Exception ex)
				{
					System.Console.Error.WriteLine("Could not load Logmanager \"" + cname + "\"");
					ex.PrintStackTrace();
				}
				if (mgr == null)
				{
					mgr = new LogManager();
				}
				return mgr;

			}
		}


		// This private class is used as a shutdown hook.
		// It does a "reset" to close all open handlers.
		private class Cleaner : Thread
		{
			private readonly LogManager OuterInstance;


			internal Cleaner(LogManager outerInstance)
			{
				this.OuterInstance = outerInstance;
				/* Set context class loader to null in order to avoid
				 * keeping a strong reference to an application classloader.
				 */
				this.ContextClassLoader = null;
			}

			public override void Run()
			{
				// This is to ensure the LogManager.<clinit> is completed
				// before synchronized block. Otherwise deadlocks are possible.
				LogManager mgr = Manager;

				// If the global handlers haven't been initialized yet, we
				// don't want to initialize them just so we can close them!
				lock (OuterInstance)
				{
					// Note that death is imminent.
					outerInstance.DeathImminent = true;
					outerInstance.InitializedGlobalHandlers = true;
				}

				// Do a reset to close all active handlers.
				outerInstance.Reset();
			}
		}


		/// <summary>
		/// Protected constructor.  This is protected so that container applications
		/// (such as J2EE containers) can subclass the object.  It is non-public as
		/// it is intended that there only be one LogManager object, whose value is
		/// retrieved by calling LogManager.getLogManager.
		/// </summary>
		protected internal LogManager() : this(CheckSubclassPermissions())
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private LogManager(Void @checked)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}

			// Add a shutdown hook to close the global handlers.
			try
			{
				Runtime.Runtime.AddShutdownHook(new Cleaner(this));
			}
			catch (IllegalStateException)
			{
				// If the VM is already shutting down,
				// We do not need to register shutdownHook.
			}
		}

		private static Void CheckSubclassPermissions()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SecurityManager sm = System.getSecurityManager();
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				// These permission will be checked in the LogManager constructor,
				// in order to register the Cleaner() thread as a shutdown hook.
				// Check them here to avoid the penalty of constructing the object
				// etc...
				sm.CheckPermission(new RuntimePermission("shutdownHooks"));
				sm.CheckPermission(new RuntimePermission("setContextClassLoader"));
			}
			return null;
		}

		/// <summary>
		/// Lazy initialization: if this instance of manager is the global
		/// manager then this method will read the initial configuration and
		/// add the root logger and global logger by calling addLogger().
		/// 
		/// Note that it is subtly different from what we do in LoggerContext.
		/// In LoggerContext we're patching up the logger context tree in order to add
		/// the root and global logger *to the context tree*.
		/// 
		/// For this to work, addLogger() must have already have been called
		/// once on the LogManager instance for the default logger being
		/// added.
		/// 
		/// This is why ensureLogManagerInitialized() needs to be called before
		/// any logger is added to any logger context.
		/// 
		/// </summary>
		private bool InitializedCalled = false;
		private volatile bool InitializationDone = false;
		internal void EnsureLogManagerInitialized()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LogManager owner = this;
			LogManager owner = this;
			if (InitializationDone || owner != Manager)
			{
				// we don't want to do this twice, and we don't want to do
				// this on private manager instances.
				return;
			}

			// Maybe another thread has called ensureLogManagerInitialized()
			// before us and is still executing it. If so we will block until
			// the log manager has finished initialized, then acquire the monitor,
			// notice that initializationDone is now true and return.
			// Otherwise - we have come here first! We will acquire the monitor,
			// see that initializationDone is still false, and perform the
			// initialization.
			//
			lock (this)
			{
				// If initializedCalled is true it means that we're already in
				// the process of initializing the LogManager in this thread.
				// There has been a recursive call to ensureLogManagerInitialized().
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isRecursiveInitialization = (initializedCalled == true);
				bool isRecursiveInitialization = (InitializedCalled == true);

				Debug.Assert(InitializedCalled || !InitializationDone, "Initialization can't be done if initialized has not been called!");

				if (isRecursiveInitialization || InitializationDone)
				{
					// If isRecursiveInitialization is true it means that we're
					// already in the process of initializing the LogManager in
					// this thread. There has been a recursive call to
					// ensureLogManagerInitialized(). We should not proceed as
					// it would lead to infinite recursion.
					//
					// If initializationDone is true then it means the manager
					// has finished initializing; just return: we're done.
					return;
				}
				// Calling addLogger below will in turn call requiresDefaultLogger()
				// which will call ensureLogManagerInitialized().
				// We use initializedCalled to break the recursion.
				InitializedCalled = true;
				try
				{
					AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, owner));
				}
				finally
				{
					InitializationDone = true;
				}
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			private readonly LogManager OuterInstance;

			private java.util.logging.LogManager Owner;

			public PrivilegedActionAnonymousInnerClassHelper(LogManager outerInstance, java.util.logging.LogManager owner)
			{
				this.OuterInstance = outerInstance;
				this.Owner = owner;
			}

			public virtual Object Run()
			{
				Debug.Assert(OuterInstance.RootLogger == null);
				Debug.Assert(OuterInstance.InitializedCalled && !OuterInstance.InitializationDone);

				// Read configuration.
				Owner.ReadPrimordialConfiguration();

				// Create and retain Logger for the root of the namespace.
				Owner.RootLogger = new java.util.logging.LogManager.RootLogger(Owner);
				Owner.AddLogger(Owner.RootLogger);
				if (!Owner.RootLogger.LevelInitialized)
				{
					Owner.RootLogger.Level = DefaultLevel;
				}

				// Adding the global Logger.
				// Do not call Logger.getGlobal() here as this might trigger
				// subtle inter-dependency issues.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") final Logger global = Logger.global;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				Logger global = Logger.Global_Renamed;

				// Make sure the global logger will be registered in the
				// global manager
				Owner.AddLogger(global);
				return null;
			}
		}

		/// <summary>
		/// Returns the global LogManager object. </summary>
		/// <returns> the global LogManager object </returns>
		public static LogManager LogManager
		{
			get
			{
				if (Manager != null)
				{
					Manager.EnsureLogManagerInitialized();
				}
				return Manager;
			}
		}

		private void ReadPrimordialConfiguration()
		{
			if (!ReadPrimordialConfiguration_Renamed)
			{
				lock (this)
				{
					if (!ReadPrimordialConfiguration_Renamed)
					{
						// If System.in/out/err are null, it's a good
						// indication that we're still in the
						// bootstrapping phase
						if (System.out == null)
						{
							return;
						}
						ReadPrimordialConfiguration_Renamed = true;

						try
						{
							AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this));
						}
						catch (Exception ex)
						{
							Debug.Assert(false, "Exception raised while reading logging configuration: " + ex);
						}
					}
				}
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Void>
		{
			private readonly LogManager OuterInstance;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(LogManager outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws Exception
			public virtual Void Run()
			{
				outerInstance.ReadConfiguration();

				// Platform loggers begin to delegate to java.util.logging.Logger
				sun.util.logging.PlatformLogger.redirectPlatformLoggers();
				return null;
			}
		}

		/// <summary>
		/// Adds an event listener to be invoked when the logging
		/// properties are re-read. Adding multiple instances of
		/// the same event Listener results in multiple entries
		/// in the property event listener table.
		/// 
		/// <para><b>WARNING:</b> This method is omitted from this class in all subset
		/// Profiles of Java SE that do not include the {@code java.beans} package.
		/// </para>
		/// </summary>
		/// <param name="l">  event listener </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have LoggingPermission("control"). </exception>
		/// <exception cref="NullPointerException"> if the PropertyChangeListener is null. </exception>
		/// @deprecated The dependency on {@code PropertyChangeListener} creates a
		///             significant impediment to future modularization of the Java
		///             platform. This method will be removed in a future release.
		///             The global {@code LogManager} can detect changes to the
		///             logging configuration by overridding the {@link
		///             #readConfiguration readConfiguration} method. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("The dependency on {@code PropertyChangeListener} creates a") public void addPropertyChangeListener(java.beans.PropertyChangeListener l) throws SecurityException
		[Obsolete("The dependency on {@code PropertyChangeListener} creates a")]
		public virtual void AddPropertyChangeListener(PropertyChangeListener l)
		{
			PropertyChangeListener listener = Objects.RequireNonNull(l);
			CheckPermission();
			lock (ListenerMap)
			{
				// increment the registration count if already registered
				Integer value = ListenerMap.Get(listener);
				value = (value == null) ? 1 : (value + 1);
				ListenerMap.Put(listener, value);
			}
		}

		/// <summary>
		/// Removes an event listener for property change events.
		/// If the same listener instance has been added to the listener table
		/// through multiple invocations of <CODE>addPropertyChangeListener</CODE>,
		/// then an equivalent number of
		/// <CODE>removePropertyChangeListener</CODE> invocations are required to remove
		/// all instances of that listener from the listener table.
		/// <P>
		/// Returns silently if the given listener is not found.
		/// 
		/// <para><b>WARNING:</b> This method is omitted from this class in all subset
		/// Profiles of Java SE that do not include the {@code java.beans} package.
		/// </para>
		/// </summary>
		/// <param name="l">  event listener (can be null) </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have LoggingPermission("control"). </exception>
		/// @deprecated The dependency on {@code PropertyChangeListener} creates a
		///             significant impediment to future modularization of the Java
		///             platform. This method will be removed in a future release.
		///             The global {@code LogManager} can detect changes to the
		///             logging configuration by overridding the {@link
		///             #readConfiguration readConfiguration} method. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("The dependency on {@code PropertyChangeListener} creates a") public void removePropertyChangeListener(java.beans.PropertyChangeListener l) throws SecurityException
		[Obsolete("The dependency on {@code PropertyChangeListener} creates a")]
		public virtual void RemovePropertyChangeListener(PropertyChangeListener l)
		{
			CheckPermission();
			if (l != null)
			{
				PropertyChangeListener listener = l;
				lock (ListenerMap)
				{
					Integer value = ListenerMap.Get(listener);
					if (value != null)
					{
						// remove from map if registration count is 1, otherwise
						// just decrement its count
						int i = value.IntValue();
						if (i == 1)
						{
							ListenerMap.Remove(listener);
						}
						else
						{
							Debug.Assert(i > 1);
							ListenerMap.Put(listener, i - 1);
						}
					}
				}
			}
		}

		// LoggerContext maps from AppContext
		private WeakHashMap<Object, LoggerContext> ContextsMap = null;

		// Returns the LoggerContext for the user code (i.e. application or AppContext).
		// Loggers are isolated from each AppContext.
		private LoggerContext UserContext
		{
			get
			{
				LoggerContext context = null;
    
				SecurityManager sm = System.SecurityManager;
				JavaAWTAccess javaAwtAccess = SharedSecrets.JavaAWTAccess;
				if (sm != null && javaAwtAccess != null)
				{
					// for each applet, it has its own LoggerContext isolated from others
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Object ecx = javaAwtAccess.getAppletContext();
					Object ecx = javaAwtAccess.AppletContext;
					if (ecx != null)
					{
						lock (javaAwtAccess)
						{
							// find the AppContext of the applet code
							// will be null if we are in the main app context.
							if (ContextsMap == null)
							{
								ContextsMap = new WeakHashMap<>();
							}
							context = ContextsMap.Get(ecx);
							if (context == null)
							{
								// Create a new LoggerContext for the applet.
								context = new LoggerContext(this);
								ContextsMap.Put(ecx, context);
							}
						}
					}
				}
				// for standalone app, return userContext
				return context != null ? context : UserContext_Renamed;
			}
		}

		// The system context.
		internal LoggerContext SystemContext
		{
			get
			{
				return SystemContext_Renamed;
			}
		}

		private List<LoggerContext> Contexts()
		{
			List<LoggerContext> cxs = new List<LoggerContext>();
			cxs.Add(SystemContext);
			cxs.Add(UserContext);
			return cxs;
		}

		// Find or create a specified logger instance. If a logger has
		// already been created with the given name it is returned.
		// Otherwise a new logger instance is created and registered
		// in the LogManager global namespace.
		// This method will always return a non-null Logger object.
		// Synchronization is not required here. All synchronization for
		// adding a new Logger object is handled by addLogger().
		//
		// This method must delegate to the LogManager implementation to
		// add a new Logger or return the one that has been added previously
		// as a LogManager subclass may override the addLogger, getLogger,
		// readConfiguration, and other methods.
		internal virtual Logger DemandLogger(String name, String resourceBundleName, Class caller)
		{
			Logger result = GetLogger(name);
			if (result == null)
			{
				// only allocate the new logger once
				Logger newLogger = new Logger(name, resourceBundleName, caller, this, false);
				do
				{
					if (AddLogger(newLogger))
					{
						// We successfully added the new Logger that we
						// created above so return it without refetching.
						return newLogger;
					}

					// We didn't add the new Logger that we created above
					// because another thread added a Logger with the same
					// name after our null check above and before our call
					// to addLogger(). We have to refetch the Logger because
					// addLogger() returns a boolean instead of the Logger
					// reference itself. However, if the thread that created
					// the other Logger is not holding a strong reference to
					// the other Logger, then it is possible for the other
					// Logger to be GC'ed after we saw it in addLogger() and
					// before we can refetch it. If it has been GC'ed then
					// we'll just loop around and try again.
					result = GetLogger(name);
				} while (result == null);
			}
			return result;
		}

		internal virtual Logger DemandSystemLogger(String name, String resourceBundleName)
		{
			// Add a system logger in the system context's namespace
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Logger sysLogger = getSystemContext().demandLogger(name, resourceBundleName);
			Logger sysLogger = SystemContext.DemandLogger(name, resourceBundleName);

			// Add the system logger to the LogManager's namespace if not exist
			// so that there is only one single logger of the given name.
			// System loggers are visible to applications unless a logger of
			// the same name has been added.
			Logger logger;
			do
			{
				// First attempt to call addLogger instead of getLogger
				// This would avoid potential bug in custom LogManager.getLogger
				// implementation that adds a logger if does not exist
				if (AddLogger(sysLogger))
				{
					// successfully added the new system logger
					logger = sysLogger;
				}
				else
				{
					logger = GetLogger(name);
				}
			} while (logger == null);

			// LogManager will set the sysLogger's handlers via LogManager.addLogger method.
			if (logger != sysLogger && sysLogger.AccessCheckedHandlers().Length == 0)
			{
				// if logger already exists but handlers not set
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Logger l = logger;
				Logger l = logger;
				AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(this, sysLogger, l));
			}
			return sysLogger;
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Void>
		{
			private readonly LogManager OuterInstance;

			private java.util.logging.Logger SysLogger;
			private java.util.logging.Logger l;

			public PrivilegedActionAnonymousInnerClassHelper2(LogManager outerInstance, java.util.logging.Logger sysLogger, java.util.logging.Logger l)
			{
				this.OuterInstance = outerInstance;
				this.SysLogger = sysLogger;
				this.l = l;
			}

			public virtual Void Run()
			{
				foreach (Handler hdl in l.AccessCheckedHandlers())
				{
					SysLogger.AddHandler(hdl);
				}
				return null;
			}
		}

		// LoggerContext maintains the logger namespace per context.
		// The default LogManager implementation has one system context and user
		// context.  The system context is used to maintain the namespace for
		// all system loggers and is queried by the system code.  If a system logger
		// doesn't exist in the user context, it'll also be added to the user context.
		// The user context is queried by the user code and all other loggers are
		// added in the user context.
		internal class LoggerContext
		{
			private readonly LogManager OuterInstance;

			// Table of named Loggers that maps names to Loggers.
			internal readonly Dictionary<String, LoggerWeakRef> NamedLoggers = new Dictionary<String, LoggerWeakRef>();
			// Tree of named Loggers
			internal readonly LogNode Root;
			internal LoggerContext(LogManager outerInstance)
			{
				this.OuterInstance = outerInstance;
				this.Root = new LogNode(null, this);
			}


			// Tells whether default loggers are required in this context.
			// If true, the default loggers will be lazily added.
			internal bool RequiresDefaultLoggers()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean requiresDefaultLoggers = (getOwner() == manager);
				bool requiresDefaultLoggers = (Owner == Manager);
				if (requiresDefaultLoggers)
				{
					Owner.EnsureLogManagerInitialized();
				}
				return requiresDefaultLoggers;
			}

			// This context's LogManager.
			internal LogManager Owner
			{
				get
				{
					return OuterInstance;
				}
			}

			// This context owner's root logger, which if not null, and if
			// the context requires default loggers, will be added to the context
			// logger's tree.
			internal Logger RootLogger
			{
				get
				{
					return Owner.RootLogger;
				}
			}

			// The global logger, which if not null, and if
			// the context requires default loggers, will be added to the context
			// logger's tree.
			internal Logger GlobalLogger
			{
				get
				{
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("deprecated") final Logger global = Logger.global;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
					Logger global = Logger.Global_Renamed; // avoids initialization cycles.
					return global;
				}
			}

			internal virtual Logger DemandLogger(String name, String resourceBundleName)
			{
				// a LogManager subclass may have its own implementation to add and
				// get a Logger.  So delegate to the LogManager to do the work.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LogManager owner = getOwner();
				LogManager owner = Owner;
				return owner.DemandLogger(name, resourceBundleName, null);
			}


			// Due to subtle deadlock issues getUserContext() no longer
			// calls addLocalLogger(rootLogger);
			// Therefore - we need to add the default loggers later on.
			// Checks that the context is properly initialized
			// This is necessary before calling e.g. find(name)
			// or getLoggerNames()
			//
			internal virtual void EnsureInitialized()
			{
				if (RequiresDefaultLoggers())
				{
					// Ensure that the root and global loggers are set.
					EnsureDefaultLogger(RootLogger);
					EnsureDefaultLogger(GlobalLogger);
				}
			}


			internal virtual Logger FindLogger(String name)
			{
				lock (this)
				{
					// ensure that this context is properly initialized before
					// looking for loggers.
					EnsureInitialized();
					LoggerWeakRef @ref = NamedLoggers.Get(name);
					if (@ref == null)
					{
						return null;
					}
					Logger logger = @ref.get();
					if (logger == null)
					{
						// Hashtable holds stale weak reference
						// to a logger which has been GC-ed.
						@ref.Dispose();
					}
					return logger;
				}
			}

			// This method is called before adding a logger to the
			// context.
			// 'logger' is the context that will be added.
			// This method will ensure that the defaults loggers are added
			// before adding 'logger'.
			//
			internal virtual void EnsureAllDefaultLoggers(Logger logger)
			{
				if (RequiresDefaultLoggers())
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = logger.getName();
					String name = logger.Name;
					if (!name.Empty)
					{
						EnsureDefaultLogger(RootLogger);
						if (!Logger.GLOBAL_LOGGER_NAME.Equals(name))
						{
							EnsureDefaultLogger(GlobalLogger);
						}
					}
				}
			}

			internal virtual void EnsureDefaultLogger(Logger logger)
			{
				// Used for lazy addition of root logger and global logger
				// to a LoggerContext.

				// This check is simple sanity: we do not want that this
				// method be called for anything else than Logger.global
				// or owner.rootLogger.
				if (!RequiresDefaultLoggers() || logger == null || logger != Logger.Global_Renamed && logger != OuterInstance.RootLogger)
				{

					// the case where we have a non null logger which is neither
					// Logger.global nor manager.rootLogger indicates a serious
					// issue - as ensureDefaultLogger should never be called
					// with any other loggers than one of these two (or null - if
					// e.g manager.rootLogger is not yet initialized)...
					Debug.Assert(logger == null);

					return;
				}

				// Adds the logger if it's not already there.
				if (!NamedLoggers.ContainsKey(logger.Name))
				{
					// It is important to prevent addLocalLogger to
					// call ensureAllDefaultLoggers when we're in the process
					// off adding one of those default loggers - as this would
					// immediately cause a stack overflow.
					// Therefore we must pass addDefaultLoggersIfNeeded=false,
					// even if requiresDefaultLoggers is true.
					AddLocalLogger(logger, false);
				}
			}

			internal virtual bool AddLocalLogger(Logger logger)
			{
				// no need to add default loggers if it's not required
				return AddLocalLogger(logger, RequiresDefaultLoggers());
			}

			// Add a logger to this context.  This method will only set its level
			// and process parent loggers.  It doesn't set its handlers.
			internal virtual bool AddLocalLogger(Logger logger, bool addDefaultLoggersIfNeeded)
			{
				lock (this)
				{
					// addDefaultLoggersIfNeeded serves to break recursion when adding
					// default loggers. If we're adding one of the default loggers
					// (we're being called from ensureDefaultLogger()) then
					// addDefaultLoggersIfNeeded will be false: we don't want to
					// call ensureAllDefaultLoggers again.
					//
					// Note: addDefaultLoggersIfNeeded can also be false when
					//       requiresDefaultLoggers is false - since calling
					//       ensureAllDefaultLoggers would have no effect in this case.
					if (addDefaultLoggersIfNeeded)
					{
						EnsureAllDefaultLoggers(logger);
					}
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = logger.getName();
					String name = logger.Name;
					if (name == null)
					{
						throw new NullPointerException();
					}
					LoggerWeakRef @ref = NamedLoggers.Get(name);
					if (@ref != null)
					{
						if (@ref.get() == null)
						{
							// It's possible that the Logger was GC'ed after a
							// drainLoggerRefQueueBounded() call above so allow
							// a new one to be registered.
							@ref.Dispose();
						}
						else
						{
							// We already have a registered logger with the given name.
							return false;
						}
					}
        
					// We're adding a new logger.
					// Note that we are creating a weak reference here.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LogManager owner = getOwner();
					LogManager owner = Owner;
					logger.LogManager = owner;
					@ref = new java.util.logging.LogManager.LoggerWeakRef(owner, logger);
					NamedLoggers.Put(name, @ref);
        
					// Apply any initial level defined for the new logger, unless
					// the logger's level is already initialized
					Level level = owner.GetLevelProperty(name + ".level", null);
					if (level != null && !logger.LevelInitialized)
					{
						DoSetLevel(logger, level);
					}
        
					// instantiation of the handler is done in the LogManager.addLogger
					// implementation as a handler class may be only visible to LogManager
					// subclass for the custom log manager case
					ProcessParentHandlers(logger, name);
        
					// Find the new node and its parent.
					LogNode node = GetNode(name);
					node.LoggerRef = @ref;
					Logger parent = null;
					LogNode nodep = node.Parent;
					while (nodep != null)
					{
						LoggerWeakRef nodeRef = nodep.LoggerRef;
						if (nodeRef != null)
						{
							parent = nodeRef.get();
							if (parent != null)
							{
								break;
							}
						}
						nodep = nodep.Parent;
					}
        
					if (parent != null)
					{
						DoSetParent(logger, parent);
					}
					// Walk over the children and tell them we are their new parent.
					node.WalkAndSetParent(logger);
					// new LogNode is ready so tell the LoggerWeakRef about it
					@ref.Node = node;
					return true;
				}
			}

			internal virtual void RemoveLoggerRef(String name, LoggerWeakRef @ref)
			{
				lock (this)
				{
					NamedLoggers.Remove(name, @ref);
				}
			}

			internal virtual IEnumerator<String> LoggerNames
			{
				get
				{
					lock (this)
					{
						// ensure that this context is properly initialized before
						// returning logger names.
						EnsureInitialized();
						return NamedLoggers.Keys();
					}
				}
			}

			// If logger.getUseParentHandlers() returns 'true' and any of the logger's
			// parents have levels or handlers defined, make sure they are instantiated.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void processParentHandlers(final Logger logger, final String name)
			internal virtual void ProcessParentHandlers(Logger logger, String name)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LogManager owner = getOwner();
				LogManager owner = Owner;
				AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper3(this, logger, name, owner));

				int ix = 1;
				for (;;)
				{
					int ix2 = name.IndexOf(".", ix);
					if (ix2 < 0)
					{
						break;
					}
					String pname = name.Substring(0, ix2);
					if (owner.GetProperty(pname + ".level") != null || owner.GetProperty(pname + ".handlers") != null)
					{
						// This pname has a level/handlers definition.
						// Make sure it exists.
						DemandLogger(pname, null);
					}
					ix = ix2 + 1;
				}
			}

			private class PrivilegedActionAnonymousInnerClassHelper3 : PrivilegedAction<Void>
			{
				private readonly LoggerContext OuterInstance;

				private java.util.logging.Logger Logger;
				private string Name;
				private java.util.logging.LogManager Owner;

				public PrivilegedActionAnonymousInnerClassHelper3(LoggerContext outerInstance, java.util.logging.Logger logger, string name, java.util.logging.LogManager owner)
				{
					this.OuterInstance = outerInstance;
					this.Logger = logger;
					this.Name = name;
					this.Owner = owner;
				}

				public virtual Void Run()
				{
					if (Logger != Owner.rootLogger)
					{
						bool useParent = Owner.GetBooleanProperty(Name + ".useParentHandlers", true);
						if (!useParent)
						{
							Logger.UseParentHandlers = false;
						}
					}
					return null;
				}
			}

			// Gets a node in our tree of logger nodes.
			// If necessary, create it.
			internal virtual LogNode GetNode(String name)
			{
				if (name == null || name.Equals(""))
				{
					return Root;
				}
				LogNode node = Root;
				while (name.Length() > 0)
				{
					int ix = name.IndexOf(".");
					String head;
					if (ix > 0)
					{
						head = name.Substring(0, ix);
						name = name.Substring(ix + 1);
					}
					else
					{
						head = name;
						name = "";
					}
					if (node.Children == null)
					{
						node.Children = new HashMap<>();
					}
					LogNode child = node.Children.Get(head);
					if (child == null)
					{
						child = new LogNode(node, this);
						node.Children.Put(head, child);
					}
					node = child;
				}
				return node;
			}
		}

		internal sealed class SystemLoggerContext : LoggerContext
		{
			private readonly LogManager OuterInstance;

			public SystemLoggerContext(LogManager outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			// Add a system logger in the system context's namespace as well as
			// in the LogManager's namespace if not exist so that there is only
			// one single logger of the given name.  System loggers are visible
			// to applications unless a logger of the same name has been added.
			internal override Logger DemandLogger(String name, String resourceBundleName)
			{
				Logger result = FindLogger(name);
				if (result == null)
				{
					// only allocate the new system logger once
					Logger newLogger = new Logger(name, resourceBundleName, null, Owner, true);
					do
					{
						if (AddLocalLogger(newLogger))
						{
							// We successfully added the new Logger that we
							// created above so return it without refetching.
							result = newLogger;
						}
						else
						{
							// We didn't add the new Logger that we created above
							// because another thread added a Logger with the same
							// name after our null check above and before our call
							// to addLogger(). We have to refetch the Logger because
							// addLogger() returns a boolean instead of the Logger
							// reference itself. However, if the thread that created
							// the other Logger is not holding a strong reference to
							// the other Logger, then it is possible for the other
							// Logger to be GC'ed after we saw it in addLogger() and
							// before we can refetch it. If it has been GC'ed then
							// we'll just loop around and try again.
							result = FindLogger(name);
						}
					} while (result == null);
				}
				return result;
			}
		}

		// Add new per logger handlers.
		// We need to raise privilege here. All our decisions will
		// be made based on the logging configuration, which can
		// only be modified by trusted code.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void loadLoggerHandlers(final Logger logger, final String name, final String handlersPropertyName)
		private void LoadLoggerHandlers(Logger logger, String name, String handlersPropertyName)
		{
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, logger, handlersPropertyName));
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			private readonly LogManager OuterInstance;

			private java.util.logging.Logger Logger;
			private string HandlersPropertyName;

			public PrivilegedActionAnonymousInnerClassHelper(LogManager outerInstance, java.util.logging.Logger logger, string handlersPropertyName)
			{
				this.OuterInstance = outerInstance;
				this.Logger = logger;
				this.HandlersPropertyName = handlersPropertyName;
			}

			public virtual Object Run()
			{
				String[] names = outerInstance.ParseClassNames(HandlersPropertyName);
				for (int i = 0; i < names.Length; i++)
				{
					String word = names[i];
					try
					{
						Class clz = ClassLoader.SystemClassLoader.LoadClass(word);
						Handler hdl = (Handler) clz.NewInstance();
						// Check if there is a property defining the
						// this handler's level.
						String levs = outerInstance.GetProperty(word + ".level");
						if (levs != null)
						{
							Level l = Level.FindLevel(levs);
							if (l != null)
							{
								hdl.Level = l;
							}
							else
							{
								// Probably a bad level. Drop through.
								System.Console.Error.WriteLine("Can't set level for " + word);
							}
						}
						// Add this Handler to the logger
						Logger.AddHandler(hdl);
					}
					catch (Exception ex)
					{
						System.Console.Error.WriteLine("Can't load log handler \"" + word + "\"");
						System.Console.Error.WriteLine("" + ex);
						ex.PrintStackTrace();
					}
				}
				return null;
			}
		}


		// loggerRefQueue holds LoggerWeakRef objects for Logger objects
		// that have been GC'ed.
		private readonly ReferenceQueue<Logger> LoggerRefQueue = new ReferenceQueue<Logger>();

		// Package-level inner class.
		// Helper class for managing WeakReferences to Logger objects.
		//
		// LogManager.namedLoggers
		//     - has weak references to all named Loggers
		//     - namedLoggers keeps the LoggerWeakRef objects for the named
		//       Loggers around until we can deal with the book keeping for
		//       the named Logger that is being GC'ed.
		// LogManager.LogNode.loggerRef
		//     - has a weak reference to a named Logger
		//     - the LogNode will also keep the LoggerWeakRef objects for
		//       the named Loggers around; currently LogNodes never go away.
		// Logger.kids
		//     - has a weak reference to each direct child Logger; this
		//       includes anonymous and named Loggers
		//     - anonymous Loggers are always children of the rootLogger
		//       which is a strong reference; rootLogger.kids keeps the
		//       LoggerWeakRef objects for the anonymous Loggers around
		//       until we can deal with the book keeping.
		//
		internal sealed class LoggerWeakRef : WeakReference<Logger>
		{
			private readonly LogManager OuterInstance;

			internal String Name; // for namedLoggers cleanup
			internal LogNode Node_Renamed; // for loggerRef cleanup
			internal WeakReference<Logger> ParentRef_Renamed; // for kids cleanup
			internal bool Disposed = false; // avoid calling dispose twice

			internal LoggerWeakRef(LogManager outerInstance, Logger logger) : base(logger, outerInstance.LoggerRefQueue)
			{
				this.OuterInstance = outerInstance;

				Name = logger.Name; // save for namedLoggers cleanup
			}

			// dispose of this LoggerWeakRef object
			internal void Dispose()
			{
				// Avoid calling dispose twice. When a Logger is gc'ed, its
				// LoggerWeakRef will be enqueued.
				// However, a new logger of the same name may be added (or looked
				// up) before the queue is drained. When that happens, dispose()
				// will be called by addLocalLogger() or findLogger().
				// Later when the queue is drained, dispose() will be called again
				// for the same LoggerWeakRef. Marking LoggerWeakRef as disposed
				// avoids processing the data twice (even though the code should
				// now be reentrant).
				lock (this)
				{
					// Note to maintainers:
					// Be careful not to call any method that tries to acquire
					// another lock from within this block - as this would surely
					// lead to deadlocks, given that dispose() can be called by
					// multiple threads, and from within different synchronized
					// methods/blocks.
					if (Disposed)
					{
						return;
					}
					Disposed = true;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LogNode n = node;
				LogNode n = Node_Renamed;
				if (n != null)
				{
					// n.loggerRef can only be safely modified from within
					// a lock on LoggerContext. removeLoggerRef is already
					// synchronized on LoggerContext so calling
					// n.context.removeLoggerRef from within this lock is safe.
					lock (n.Context)
					{
						// if we have a LogNode, then we were a named Logger
						// so clear namedLoggers weak ref to us
						n.Context.RemoveLoggerRef(Name, this);
						Name = null; // clear our ref to the Logger's name

						// LogNode may have been reused - so only clear
						// LogNode.loggerRef if LogNode.loggerRef == this
						if (n.LoggerRef == this)
						{
							n.LoggerRef = null; // clear LogNode's weak ref to us
						}
						Node_Renamed = null; // clear our ref to LogNode
					}
				}

				if (ParentRef_Renamed != null)
				{
					// this LoggerWeakRef has or had a parent Logger
					Logger parent = ParentRef_Renamed.get();
					if (parent != null)
					{
						// the parent Logger is still there so clear the
						// parent Logger's weak ref to us
						parent.RemoveChildLogger(this);
					}
					ParentRef_Renamed = null; // clear our weak ref to the parent Logger
				}
			}

			// set the node field to the specified value
			internal LogNode Node
			{
				set
				{
					this.Node_Renamed = value;
				}
			}

			// set the parentRef field to the specified value
			internal WeakReference<Logger> ParentRef
			{
				set
				{
					this.ParentRef_Renamed = value;
				}
			}
		}

		// Package-level method.
		// Drain some Logger objects that have been GC'ed.
		//
		// drainLoggerRefQueueBounded() is called by addLogger() below
		// and by Logger.getAnonymousLogger(String) so we'll drain up to
		// MAX_ITERATIONS GC'ed Loggers for every Logger we add.
		//
		// On a WinXP VMware client, a MAX_ITERATIONS value of 400 gives
		// us about a 50/50 mix in increased weak ref counts versus
		// decreased weak ref counts in the AnonLoggerWeakRefLeak test.
		// Here are stats for cleaning up sets of 400 anonymous Loggers:
		//   - test duration 1 minute
		//   - sample size of 125 sets of 400
		//   - average: 1.99 ms
		//   - minimum: 0.57 ms
		//   - maximum: 25.3 ms
		//
		// The same config gives us a better decreased weak ref count
		// than increased weak ref count in the LoggerWeakRefLeak test.
		// Here are stats for cleaning up sets of 400 named Loggers:
		//   - test duration 2 minutes
		//   - sample size of 506 sets of 400
		//   - average: 0.57 ms
		//   - minimum: 0.02 ms
		//   - maximum: 10.9 ms
		//
		private const int MAX_ITERATIONS = 400;
		internal void DrainLoggerRefQueueBounded()
		{
			for (int i = 0; i < MAX_ITERATIONS; i++)
			{
				if (LoggerRefQueue == null)
				{
					// haven't finished loading LogManager yet
					break;
				}

				LoggerWeakRef @ref = (LoggerWeakRef) LoggerRefQueue.poll();
				if (@ref == null)
				{
					break;
				}
				// a Logger object has been GC'ed so clean it up
				@ref.Dispose();
			}
		}

		/// <summary>
		/// Add a named logger.  This does nothing and returns false if a logger
		/// with the same name is already registered.
		/// <para>
		/// The Logger factory methods call this method to register each
		/// newly created Logger.
		/// </para>
		/// <para>
		/// The application should retain its own reference to the Logger
		/// object to avoid it being garbage collected.  The LogManager
		/// may only retain a weak reference.
		/// 
		/// </para>
		/// </summary>
		/// <param name="logger"> the new logger. </param>
		/// <returns>  true if the argument logger was registered successfully,
		///          false if a logger of that name already exists. </returns>
		/// <exception cref="NullPointerException"> if the logger name is null. </exception>
		public virtual bool AddLogger(Logger logger)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = logger.getName();
			String name = logger.Name;
			if (name == null)
			{
				throw new NullPointerException();
			}
			DrainLoggerRefQueueBounded();
			LoggerContext cx = UserContext;
			if (cx.AddLocalLogger(logger))
			{
				// Do we have a per logger handler too?
				// Note: this will add a 200ms penalty
				LoadLoggerHandlers(logger, name, name + ".handlers");
				return true;
			}
			else
			{
				return false;
			}
		}

		// Private method to set a level on a logger.
		// If necessary, we raise privilege before doing the call.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void doSetLevel(final Logger logger, final Level level)
		private static void DoSetLevel(Logger logger, Level level)
		{
			SecurityManager sm = System.SecurityManager;
			if (sm == null)
			{
				// There is no security manager, so things are easy.
				logger.Level = level;
				return;
			}
			// There is a security manager.  Raise privilege before
			// calling setLevel.
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(logger, level));
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			private java.util.logging.Logger Logger;
			private java.util.logging.Level Level;

			public PrivilegedActionAnonymousInnerClassHelper(java.util.logging.Logger logger, java.util.logging.Level level)
			{
				this.Logger = logger;
				this.Level = level;
			}

			public virtual Object Run()
			{
				Logger.Level = Level;
				return null;
			}
		}

		// Private method to set a parent on a logger.
		// If necessary, we raise privilege before doing the setParent call.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void doSetParent(final Logger logger, final Logger parent)
		private static void DoSetParent(Logger logger, Logger parent)
		{
			SecurityManager sm = System.SecurityManager;
			if (sm == null)
			{
				// There is no security manager, so things are easy.
				logger.Parent = parent;
				return;
			}
			// There is a security manager.  Raise privilege before
			// calling setParent.
			AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(logger, parent));
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<Object>
		{
			private java.util.logging.Logger Logger;
			private java.util.logging.Logger Parent;

			public PrivilegedActionAnonymousInnerClassHelper2(java.util.logging.Logger logger, java.util.logging.Logger parent)
			{
				this.Logger = logger;
				this.Parent = parent;
			}

			public virtual Object Run()
			{
				Logger.Parent = Parent;
				return null;
			}
		}

		/// <summary>
		/// Method to find a named logger.
		/// <para>
		/// Note that since untrusted code may create loggers with
		/// arbitrary names this method should not be relied on to
		/// find Loggers for security sensitive logging.
		/// It is also important to note that the Logger associated with the
		/// String {@code name} may be garbage collected at any time if there
		/// is no strong reference to the Logger. The caller of this method
		/// must check the return value for null in order to properly handle
		/// the case where the Logger has been garbage collected.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="name"> name of the logger </param>
		/// <returns>  matching logger or null if none is found </returns>
		public virtual Logger GetLogger(String name)
		{
			return UserContext.FindLogger(name);
		}

		/// <summary>
		/// Get an enumeration of known logger names.
		/// <para>
		/// Note:  Loggers may be added dynamically as new classes are loaded.
		/// This method only reports on the loggers that are currently registered.
		/// It is also important to note that this method only returns the name
		/// of a Logger, not a strong reference to the Logger itself.
		/// The returned String does nothing to prevent the Logger from being
		/// garbage collected. In particular, if the returned name is passed
		/// to {@code LogManager.getLogger()}, then the caller must check the
		/// return value from {@code LogManager.getLogger()} for null to properly
		/// handle the case where the Logger has been garbage collected in the
		/// time since its name was returned by this method.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <returns>  enumeration of logger name strings </returns>
		public virtual IEnumerator<String> LoggerNames
		{
			get
			{
				return UserContext.LoggerNames;
			}
		}

		/// <summary>
		/// Reinitialize the logging properties and reread the logging configuration.
		/// <para>
		/// The same rules are used for locating the configuration properties
		/// as are used at startup.  So normally the logging properties will
		/// be re-read from the same file that was used at startup.
		/// <P>
		/// Any log level definitions in the new configuration file will be
		/// applied using Logger.setLevel(), if the target Logger exists.
		/// </para>
		/// <para>
		/// A PropertyChangeEvent will be fired after the properties are read.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have LoggingPermission("control"). </exception>
		/// <exception cref="IOException"> if there are IO problems reading the configuration. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readConfiguration() throws IOException, SecurityException
		public virtual void ReadConfiguration()
		{
			CheckPermission();

			// if a configuration class is specified, load it and use it.
			String cname = System.getProperty("java.util.logging.config.class");
			if (cname != null)
			{
				try
				{
					// Instantiate the named class.  It is its constructor's
					// responsibility to initialize the logging configuration, by
					// calling readConfiguration(InputStream) with a suitable stream.
					try
					{
						Class clz = ClassLoader.SystemClassLoader.LoadClass(cname);
						clz.NewInstance();
						return;
					}
					catch (ClassNotFoundException)
					{
						Class clz = Thread.CurrentThread.ContextClassLoader.loadClass(cname);
						clz.NewInstance();
						return;
					}
				}
				catch (Exception ex)
				{
					System.Console.Error.WriteLine("Logging configuration class \"" + cname + "\" failed");
					System.Console.Error.WriteLine("" + ex);
					// keep going and useful config file.
				}
			}

			String fname = System.getProperty("java.util.logging.config.file");
			if (fname == null)
			{
				fname = System.getProperty("java.home");
				if (fname == null)
				{
					throw new Error("Can't find java.home ??");
				}
				File f = new File(fname, "lib");
				f = new File(f, "logging.properties");
				fname = f.CanonicalPath;
			}
			using (final InputStream @in = new FileInputStream(fname))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BufferedInputStream bin = new BufferedInputStream(in);
				BufferedInputStream bin = new BufferedInputStream(@in);
				ReadConfiguration(bin);
			}
		}

		/// <summary>
		/// Reset the logging configuration.
		/// <para>
		/// For all named loggers, the reset operation removes and closes
		/// all Handlers and (except for the root logger) sets the level
		/// to null.  The root logger's level is set to Level.INFO.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have LoggingPermission("control"). </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset() throws SecurityException
		public virtual void Reset()
		{
			CheckPermission();
			lock (this)
			{
				Props = new Properties();
				// Since we are doing a reset we no longer want to initialize
				// the global handlers, if they haven't been initialized yet.
				InitializedGlobalHandlers = true;
			}
			foreach (LoggerContext cx in Contexts())
			{
				IEnumerator<String> enum_ = cx.LoggerNames;
				while (enum_.MoveNext())
				{
					String name = enum_.Current;
					Logger logger = cx.FindLogger(name);
					if (logger != null)
					{
						ResetLogger(logger);
					}
				}
			}
		}

		// Private method to reset an individual target logger.
		private void ResetLogger(Logger logger)
		{
			// Close all the Logger's handlers.
			Handler[] targets = logger.Handlers;
			for (int i = 0; i < targets.Length; i++)
			{
				Handler h = targets[i];
				logger.RemoveHandler(h);
				try
				{
					h.Close();
				}
				catch (Exception)
				{
					// Problems closing a handler?  Keep going...
				}
			}
			String name = logger.Name;
			if (name != null && name.Equals(""))
			{
				// This is the root logger.
				logger.Level = DefaultLevel;
			}
			else
			{
				logger.Level = null;
			}
		}

		// get a list of whitespace separated classnames from a property.
		private String[] ParseClassNames(String propertyName)
		{
			String hands = GetProperty(propertyName);
			if (hands == null)
			{
				return new String[0];
			}
			hands = hands.Trim();
			int ix = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<String> result = new ArrayList<>();
			List<String> result = new List<String>();
			while (ix < hands.Length())
			{
				int end = ix;
				while (end < hands.Length())
				{
					if (char.IsWhiteSpace(hands.CharAt(end)))
					{
						break;
					}
					if (hands.CharAt(end) == ',')
					{
						break;
					}
					end++;
				}
				String word = hands.Substring(ix, end - ix);
				ix = end + 1;
				word = word.Trim();
				if (word.Length() == 0)
				{
					continue;
				}
				result.Add(word);
			}
			return result.ToArray(new String[result.Count]);
		}

		/// <summary>
		/// Reinitialize the logging properties and reread the logging configuration
		/// from the given stream, which should be in java.util.Properties format.
		/// A PropertyChangeEvent will be fired after the properties are read.
		/// <para>
		/// Any log level definitions in the new configuration file will be
		/// applied using Logger.setLevel(), if the target Logger exists.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ins">       stream to read properties from </param>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have LoggingPermission("control"). </exception>
		/// <exception cref="IOException"> if there are problems reading from the stream. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readConfiguration(InputStream ins) throws IOException, SecurityException
		public virtual void ReadConfiguration(InputStream ins)
		{
			CheckPermission();
			Reset();

			// Load the properties
			Props.Load(ins);
			// Instantiate new configuration objects.
			String[] names = ParseClassNames("config");

			for (int i = 0; i < names.Length; i++)
			{
				String word = names[i];
				try
				{
					Class clz = ClassLoader.SystemClassLoader.LoadClass(word);
					clz.NewInstance();
				}
				catch (Exception ex)
				{
					System.Console.Error.WriteLine("Can't load config class \"" + word + "\"");
					System.Console.Error.WriteLine("" + ex);
					// ex.printStackTrace();
				}
			}

			// Set levels on any pre-existing loggers, based on the new properties.
			SetLevelsOnExistingLoggers();

			// Notify any interested parties that our properties have changed.
			// We first take a copy of the listener map so that we aren't holding any
			// locks when calling the listeners.
			Map<Object, Integer> listeners = null;
			lock (ListenerMap)
			{
				if (!ListenerMap.Empty)
				{
					listeners = new HashMap<>(ListenerMap);
				}
			}
			if (listeners != null)
			{
				Debug.Assert(Beans.BeansPresent);
				Object ev = Beans.NewPropertyChangeEvent(typeof(LogManager), null, null, null);
				foreach (Map_Entry<Object, Integer> entry in listeners.EntrySet())
				{
					Object listener = entry.Key;
					int count = entry.Value.IntValue();
					for (int i = 0; i < count; i++)
					{
						Beans.InvokePropertyChange(listener, ev);
					}
				}
			}


			// Note that we need to reinitialize global handles when
			// they are first referenced.
			lock (this)
			{
				InitializedGlobalHandlers = false;
			}
		}

		/// <summary>
		/// Get the value of a logging property.
		/// The method returns null if the property is not found. </summary>
		/// <param name="name">      property name </param>
		/// <returns>          property value </returns>
		public virtual String GetProperty(String name)
		{
			return Props.GetProperty(name);
		}

		// Package private method to get a String property.
		// If the property is not defined we return the given
		// default value.
		internal virtual String GetStringProperty(String name, String defaultValue)
		{
			String val = GetProperty(name);
			if (val == null)
			{
				return defaultValue;
			}
			return val.Trim();
		}

		// Package private method to get an integer property.
		// If the property is not defined or cannot be parsed
		// we return the given default value.
		internal virtual int GetIntProperty(String name, int defaultValue)
		{
			String val = GetProperty(name);
			if (val == null)
			{
				return defaultValue;
			}
			try
			{
				return Convert.ToInt32(val.Trim());
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		// Package private method to get a boolean property.
		// If the property is not defined or cannot be parsed
		// we return the given default value.
		internal virtual bool GetBooleanProperty(String name, bool defaultValue)
		{
			String val = GetProperty(name);
			if (val == null)
			{
				return defaultValue;
			}
			val = val.ToLowerCase();
			if (val.Equals("true") || val.Equals("1"))
			{
				return true;
			}
			else if (val.Equals("false") || val.Equals("0"))
			{
				return false;
			}
			return defaultValue;
		}

		// Package private method to get a Level property.
		// If the property is not defined or cannot be parsed
		// we return the given default value.
		internal virtual Level GetLevelProperty(String name, Level defaultValue)
		{
			String val = GetProperty(name);
			if (val == null)
			{
				return defaultValue;
			}
			Level l = Level.FindLevel(val.Trim());
			return l != null ? l : defaultValue;
		}

		// Package private method to get a filter property.
		// We return an instance of the class named by the "name"
		// property. If the property is not defined or has problems
		// we return the defaultValue.
		internal virtual Filter GetFilterProperty(String name, Filter defaultValue)
		{
			String val = GetProperty(name);
			try
			{
				if (val != null)
				{
					Class clz = ClassLoader.SystemClassLoader.LoadClass(val);
					return (Filter) clz.NewInstance();
				}
			}
			catch (Exception)
			{
				// We got one of a variety of exceptions in creating the
				// class or creating an instance.
				// Drop through.
			}
			// We got an exception.  Return the defaultValue.
			return defaultValue;
		}


		// Package private method to get a formatter property.
		// We return an instance of the class named by the "name"
		// property. If the property is not defined or has problems
		// we return the defaultValue.
		internal virtual Formatter GetFormatterProperty(String name, Formatter defaultValue)
		{
			String val = GetProperty(name);
			try
			{
				if (val != null)
				{
					Class clz = ClassLoader.SystemClassLoader.LoadClass(val);
					return (Formatter) clz.NewInstance();
				}
			}
			catch (Exception)
			{
				// We got one of a variety of exceptions in creating the
				// class or creating an instance.
				// Drop through.
			}
			// We got an exception.  Return the defaultValue.
			return defaultValue;
		}

		// Private method to load the global handlers.
		// We do the real work lazily, when the global handlers
		// are first used.
		private void InitializeGlobalHandlers()
		{
			lock (this)
			{
				if (InitializedGlobalHandlers)
				{
					return;
				}
        
				InitializedGlobalHandlers = true;
        
				if (DeathImminent)
				{
					// Aaargh...
					// The VM is shutting down and our exit hook has been called.
					// Avoid allocating global handlers.
					return;
				}
				LoadLoggerHandlers(RootLogger, null, "handlers");
			}
		}

		private readonly Permission ControlPermission = new LoggingPermission("control", null);

		internal virtual void CheckPermission()
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckPermission(ControlPermission);
			}
		}

		/// <summary>
		/// Check that the current context is trusted to modify the logging
		/// configuration.  This requires LoggingPermission("control").
		/// <para>
		/// If the check fails we throw a SecurityException, otherwise
		/// we return normally.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="SecurityException">  if a security manager exists and if
		///             the caller does not have LoggingPermission("control"). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkAccess() throws SecurityException
		public virtual void CheckAccess()
		{
			CheckPermission();
		}

		// Nested class to represent a node in our tree of named loggers.
		private class LogNode
		{
			internal HashMap<String, LogNode> Children;
			internal LoggerWeakRef LoggerRef;
			internal LogNode Parent;
			internal readonly LoggerContext Context;

			internal LogNode(LogNode parent, LoggerContext context)
			{
				this.Parent = parent;
				this.Context = context;
			}

			// Recursive method to walk the tree below a node and set
			// a new parent logger.
			internal virtual void WalkAndSetParent(Logger parent)
			{
				if (Children == null)
				{
					return;
				}
				Iterator<LogNode> values = Children.Values().Iterator();
				while (values.HasNext())
				{
					LogNode node = values.Next();
					LoggerWeakRef @ref = node.LoggerRef;
					Logger logger = (@ref == null) ? null : @ref.get();
					if (logger == null)
					{
						node.WalkAndSetParent(parent);
					}
					else
					{
						DoSetParent(logger, parent);
					}
				}
			}
		}

		// We use a subclass of Logger for the root logger, so
		// that we only instantiate the global handlers when they
		// are first needed.
		private sealed class RootLogger : Logger
		{
			private readonly LogManager OuterInstance;

			internal RootLogger(LogManager outerInstance) : base("", null, null, outerInstance, true)
			{
				// We do not call the protected Logger two args constructor here,
				// to avoid calling LogManager.getLogManager() from within the
				// RootLogger constructor.
				this.OuterInstance = outerInstance;
			}

			public override void Log(LogRecord record)
			{
				// Make sure that the global handlers have been instantiated.
				outerInstance.InitializeGlobalHandlers();
				base.Log(record);
			}

			public override void AddHandler(Handler h)
			{
				outerInstance.InitializeGlobalHandlers();
				base.AddHandler(h);
			}

			public override void RemoveHandler(Handler h)
			{
				outerInstance.InitializeGlobalHandlers();
				base.RemoveHandler(h);
			}

			internal override Handler[] AccessCheckedHandlers()
			{
				outerInstance.InitializeGlobalHandlers();
				return base.AccessCheckedHandlers();
			}
		}


		// Private method to be called when the configuration has
		// changed to apply any level settings to any pre-existing loggers.
		private void SetLevelsOnExistingLoggers()
		{
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<?> enum_ = props.propertyNames();
				IEnumerator<?> enum_ = Props.PropertyNames();
				while (enum_.MoveNext())
				{
					String key = (String)enum_.Current;
					if (!key.EndsWith(".level"))
					{
						// Not a level definition.
						continue;
					}
					int ix = key.Length() - 6;
					String name = key.Substring(0, ix);
					Level level = GetLevelProperty(key, null);
					if (level == null)
					{
						System.Console.Error.WriteLine("Bad level value for property: " + key);
						continue;
					}
					foreach (LoggerContext cx in Contexts())
					{
						Logger l = cx.FindLogger(name);
						if (l == null)
						{
							continue;
						}
						l.Level = level;
					}
				}
			}
		}

		// Management Support
		private static LoggingMXBean LoggingMXBean_Renamed = null;
		/// <summary>
		/// String representation of the
		/// <seealso cref="javax.management.ObjectName"/> for the management interface
		/// for the logging facility.
		/// </summary>
		/// <seealso cref= java.lang.management.PlatformLoggingMXBean </seealso>
		/// <seealso cref= java.util.logging.LoggingMXBean
		/// 
		/// @since 1.5 </seealso>
		public const String LOGGING_MXBEAN_NAME = "java.util.logging:type=Logging";

		/// <summary>
		/// Returns <tt>LoggingMXBean</tt> for managing loggers.
		/// An alternative way to manage loggers is through the
		/// <seealso cref="java.lang.management.PlatformLoggingMXBean"/> interface
		/// that can be obtained by calling:
		/// <pre>
		///     PlatformLoggingMXBean logging = {@link java.lang.management.ManagementFactory#getPlatformMXBean(Class)
		///         ManagementFactory.getPlatformMXBean}(PlatformLoggingMXBean.class);
		/// </pre>
		/// </summary>
		/// <returns> a <seealso cref="LoggingMXBean"/> object.
		/// </returns>
		/// <seealso cref= java.lang.management.PlatformLoggingMXBean
		/// @since 1.5 </seealso>
		public static LoggingMXBean LoggingMXBean
		{
			get
			{
				lock (typeof(LogManager))
				{
					if (LoggingMXBean_Renamed == null)
					{
						LoggingMXBean_Renamed = new Logging();
					}
					return LoggingMXBean_Renamed;
				}
			}
		}

		/// <summary>
		/// A class that provides access to the java.beans.PropertyChangeListener
		/// and java.beans.PropertyChangeEvent without creating a static dependency
		/// on java.beans. This class can be removed once the addPropertyChangeListener
		/// and removePropertyChangeListener methods are removed.
		/// </summary>
		private class Beans
		{
			internal static readonly Class PropertyChangeListenerClass = GetClass("java.beans.PropertyChangeListener");

			internal static readonly Class PropertyChangeEventClass = GetClass("java.beans.PropertyChangeEvent");

			internal static readonly Method PropertyChangeMethod = GetMethod(PropertyChangeListenerClass, "propertyChange", PropertyChangeEventClass);

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static final Constructor<?> propertyEventCtor = getConstructor(propertyChangeEventClass, Object.class, String.class, Object.class, Object.class);
			internal static readonly Constructor<?> PropertyEventCtor = getConstructor(PropertyChangeEventClass, typeof(Object), typeof(String), typeof(Object), typeof(Object));

			internal static Class GetClass(String name)
			{
				try
				{
					return Class.ForName(name, true, typeof(Beans).ClassLoader);
				}
				catch (ClassNotFoundException)
				{
					return null;
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static Constructor<?> getConstructor(Class c, Class... types)
			internal static Constructor<?> GetConstructor(Class c, params Class[] types)
			{
				try
				{
					return (c == null) ? null : c.GetDeclaredConstructor(types);
				}
				catch (NoSuchMethodException x)
				{
					throw new AssertionError(x);
				}
			}

			internal static Method GetMethod(Class c, String name, params Class[] types)
			{
				try
				{
					return (c == null) ? null : c.GetMethod(name, types);
				}
				catch (NoSuchMethodException e)
				{
					throw new AssertionError(e);
				}
			}

			/// <summary>
			/// Returns {@code true} if java.beans is present.
			/// </summary>
			internal static bool BeansPresent
			{
				get
				{
					return PropertyChangeListenerClass != null && PropertyChangeEventClass != null;
				}
			}

			/// <summary>
			/// Returns a new PropertyChangeEvent with the given source, property
			/// name, old and new values.
			/// </summary>
			internal static Object NewPropertyChangeEvent(Object source, String prop, Object oldValue, Object newValue)
			{
				try
				{
					return PropertyEventCtor.newInstance(source, prop, oldValue, newValue);
				}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
				catch (InstantiationException | IllegalAccessException x)
				{
					throw new AssertionError(x);
				}
				catch (InvocationTargetException x)
				{
					Throwable cause = x.InnerException;
					if (cause is Error)
					{
						throw (Error)cause;
					}
					if (cause is RuntimeException)
					{
						throw (RuntimeException)cause;
					}
					throw new AssertionError(x);
				}
			}

			/// <summary>
			/// Invokes the given PropertyChangeListener's propertyChange method
			/// with the given event.
			/// </summary>
			internal static void InvokePropertyChange(Object listener, Object ev)
			{
				try
				{
					PropertyChangeMethod.invoke(listener, ev);
				}
				catch (IllegalAccessException x)
				{
					throw new AssertionError(x);
				}
				catch (InvocationTargetException x)
				{
					Throwable cause = x.InnerException;
					if (cause is Error)
					{
						throw (Error)cause;
					}
					if (cause is RuntimeException)
					{
						throw (RuntimeException)cause;
					}
					throw new AssertionError(x);
				}
			}
		}
	}

}