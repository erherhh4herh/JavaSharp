using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	using ManagementFactoryHelper = sun.management.ManagementFactoryHelper;
	using ExtendedPlatformComponent = sun.management.ExtendedPlatformComponent;

	/// <summary>
	/// The {@code ManagementFactory} class is a factory class for getting
	/// managed beans for the Java platform.
	/// This class consists of static methods each of which returns
	/// one or more <i>platform MXBeans</i> representing
	/// the management interface of a component of the Java virtual
	/// machine.
	/// 
	/// <h3><a name="MXBean">Platform MXBeans</a></h3>
	/// <para>
	/// A platform MXBean is a <i>managed bean</i> that
	/// conforms to the <a href="../../../javax/management/package-summary.html">JMX</a>
	/// Instrumentation Specification and only uses a set of basic data types.
	/// A JMX management application and the {@linkplain
	/// #getPlatformMBeanServer platform MBeanServer}
	/// can interoperate without requiring classes for MXBean specific
	/// data types.
	/// The data types being transmitted between the JMX connector
	/// server and the connector client are
	/// <seealso cref="javax.management.openmbean.OpenType open types"/>
	/// and this allows interoperation across versions.
	/// See <a href="../../../javax/management/MXBean.html#MXBean-spec">
	/// the specification of MXBeans</a> for details.
	/// 
	/// <a name="MXBeanNames"></a>
	/// </para>
	/// <para>Each platform MXBean is a <seealso cref="PlatformManagedObject"/>
	/// and it has a unique
	/// <seealso cref="javax.management.ObjectName ObjectName"/> for
	/// registration in the platform {@code MBeanServer} as returned by
	/// by the <seealso cref="PlatformManagedObject#getObjectName getObjectName"/>
	/// method.
	/// 
	/// </para>
	/// <para>
	/// An application can access a platform MXBean in the following ways:
	/// <h4>1. Direct access to an MXBean interface</h4>
	/// <blockquote>
	/// <ul>
	///     <li>Get an MXBean instance by calling the
	///         <seealso cref="#getPlatformMXBean(Class) getPlatformMXBean"/> or
	///         <seealso cref="#getPlatformMXBeans(Class) getPlatformMXBeans"/> method
	///         and access the MXBean locally in the running
	///         virtual machine.
	///         </li>
	///     <li>Construct an MXBean proxy instance that forwards the
	///         method calls to a given <seealso cref="MBeanServer MBeanServer"/> by calling
	///         the <seealso cref="#getPlatformMXBean(MBeanServerConnection, Class)"/> or
	///         <seealso cref="#getPlatformMXBeans(MBeanServerConnection, Class)"/> method.
	///         The <seealso cref="#newPlatformMXBeanProxy newPlatformMXBeanProxy"/> method
	///         can also be used to construct an MXBean proxy instance of
	///         a given {@code ObjectName}.
	///         A proxy is typically constructed to remotely access
	///         an MXBean of another running virtual machine.
	///         </li>
	/// </ul>
	/// <h4>2. Indirect access to an MXBean interface via MBeanServer</h4>
	/// <ul>
	///     <li>Go through the platform {@code MBeanServer} to access MXBeans
	///         locally or a specific <tt>MBeanServerConnection</tt> to access
	///         MXBeans remotely.
	///         The attributes and operations of an MXBean use only
	///         <em>JMX open types</em> which include basic data types,
	///         <seealso cref="javax.management.openmbean.CompositeData CompositeData"/>,
	///         and <seealso cref="javax.management.openmbean.TabularData TabularData"/>
	///         defined in
	///         <seealso cref="javax.management.openmbean.OpenType OpenType"/>.
	///         The mapping is specified in
	///         the <seealso cref="javax.management.MXBean MXBean"/> specification
	///         for details.
	///        </li>
	/// </ul>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>
	/// The <seealso cref="#getPlatformManagementInterfaces getPlatformManagementInterfaces"/>
	/// method returns all management interfaces supported in the Java virtual machine
	/// including the standard management interfaces listed in the tables
	/// below as well as the management interfaces extended by the JDK implementation.
	/// </para>
	/// <para>
	/// A Java virtual machine has a single instance of the following management
	/// interfaces:
	/// 
	/// <blockquote>
	/// <table border summary="The list of Management Interfaces and their single instances">
	/// <tr>
	/// <th>Management Interface</th>
	/// <th>ObjectName</th>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="ClassLoadingMXBean"/> </td>
	/// <td> {@link #CLASS_LOADING_MXBEAN_NAME
	///             java.lang:type=ClassLoading}</td>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="MemoryMXBean"/> </td>
	/// <td> {@link #MEMORY_MXBEAN_NAME
	///             java.lang:type=Memory}</td>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="ThreadMXBean"/> </td>
	/// <td> {@link #THREAD_MXBEAN_NAME
	///             java.lang:type=Threading}</td>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="RuntimeMXBean"/> </td>
	/// <td> {@link #RUNTIME_MXBEAN_NAME
	///             java.lang:type=Runtime}</td>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="OperatingSystemMXBean"/> </td>
	/// <td> {@link #OPERATING_SYSTEM_MXBEAN_NAME
	///             java.lang:type=OperatingSystem}</td>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="PlatformLoggingMXBean"/> </td>
	/// <td> {@link java.util.logging.LogManager#LOGGING_MXBEAN_NAME
	///             java.util.logging:type=Logging}</td>
	/// </tr>
	/// </table>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>
	/// A Java virtual machine has zero or a single instance of
	/// the following management interfaces.
	/// 
	/// <blockquote>
	/// <table border summary="The list of Management Interfaces and their single instances">
	/// <tr>
	/// <th>Management Interface</th>
	/// <th>ObjectName</th>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="CompilationMXBean"/> </td>
	/// <td> {@link #COMPILATION_MXBEAN_NAME
	///             java.lang:type=Compilation}</td>
	/// </tr>
	/// </table>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>
	/// A Java virtual machine may have one or more instances of the following
	/// management interfaces.
	/// <blockquote>
	/// <table border summary="The list of Management Interfaces and their single instances">
	/// <tr>
	/// <th>Management Interface</th>
	/// <th>ObjectName</th>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="GarbageCollectorMXBean"/> </td>
	/// <td> {@link #GARBAGE_COLLECTOR_MXBEAN_DOMAIN_TYPE
	///             java.lang:type=GarbageCollector}<tt>,name=</tt><i>collector's name</i></td>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="MemoryManagerMXBean"/> </td>
	/// <td> {@link #MEMORY_MANAGER_MXBEAN_DOMAIN_TYPE
	///             java.lang:type=MemoryManager}<tt>,name=</tt><i>manager's name</i></td>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="MemoryPoolMXBean"/> </td>
	/// <td> {@link #MEMORY_POOL_MXBEAN_DOMAIN_TYPE
	///             java.lang:type=MemoryPool}<tt>,name=</tt><i>pool's name</i></td>
	/// </tr>
	/// <tr>
	/// <td> <seealso cref="BufferPoolMXBean"/> </td>
	/// <td> {@code java.nio:type=BufferPool,name=}<i>pool name</i></td>
	/// </tr>
	/// </table>
	/// </blockquote>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= <a href="../../../javax/management/package-summary.html">
	///      JMX Specification</a> </seealso>
	/// <seealso cref= <a href="package-summary.html#examples">
	///      Ways to Access Management Metrics</a> </seealso>
	/// <seealso cref= javax.management.MXBean
	/// 
	/// @author  Mandy Chung
	/// @since   1.5 </seealso>
	public class ManagementFactory
	{
		// A class with only static fields and methods.
		private ManagementFactory()
		{
		};

		/// <summary>
		/// String representation of the
		/// <tt>ObjectName</tt> for the <seealso cref="ClassLoadingMXBean"/>.
		/// </summary>
		public const String CLASS_LOADING_MXBEAN_NAME = "java.lang:type=ClassLoading";

		/// <summary>
		/// String representation of the
		/// <tt>ObjectName</tt> for the <seealso cref="CompilationMXBean"/>.
		/// </summary>
		public const String COMPILATION_MXBEAN_NAME = "java.lang:type=Compilation";

		/// <summary>
		/// String representation of the
		/// <tt>ObjectName</tt> for the <seealso cref="MemoryMXBean"/>.
		/// </summary>
		public const String MEMORY_MXBEAN_NAME = "java.lang:type=Memory";

		/// <summary>
		/// String representation of the
		/// <tt>ObjectName</tt> for the <seealso cref="OperatingSystemMXBean"/>.
		/// </summary>
		public const String OPERATING_SYSTEM_MXBEAN_NAME = "java.lang:type=OperatingSystem";

		/// <summary>
		/// String representation of the
		/// <tt>ObjectName</tt> for the <seealso cref="RuntimeMXBean"/>.
		/// </summary>
		public const String RUNTIME_MXBEAN_NAME = "java.lang:type=Runtime";

		/// <summary>
		/// String representation of the
		/// <tt>ObjectName</tt> for the <seealso cref="ThreadMXBean"/>.
		/// </summary>
		public const String THREAD_MXBEAN_NAME = "java.lang:type=Threading";

		/// <summary>
		/// The domain name and the type key property in
		/// the <tt>ObjectName</tt> for a <seealso cref="GarbageCollectorMXBean"/>.
		/// The unique <tt>ObjectName</tt> for a <tt>GarbageCollectorMXBean</tt>
		/// can be formed by appending this string with
		/// "<tt>,name=</tt><i>collector's name</i>".
		/// </summary>
		public const String GARBAGE_COLLECTOR_MXBEAN_DOMAIN_TYPE = "java.lang:type=GarbageCollector";

		/// <summary>
		/// The domain name and the type key property in
		/// the <tt>ObjectName</tt> for a <seealso cref="MemoryManagerMXBean"/>.
		/// The unique <tt>ObjectName</tt> for a <tt>MemoryManagerMXBean</tt>
		/// can be formed by appending this string with
		/// "<tt>,name=</tt><i>manager's name</i>".
		/// </summary>
		public const String MEMORY_MANAGER_MXBEAN_DOMAIN_TYPE = "java.lang:type=MemoryManager";

		/// <summary>
		/// The domain name and the type key property in
		/// the <tt>ObjectName</tt> for a <seealso cref="MemoryPoolMXBean"/>.
		/// The unique <tt>ObjectName</tt> for a <tt>MemoryPoolMXBean</tt>
		/// can be formed by appending this string with
		/// <tt>,name=</tt><i>pool's name</i>.
		/// </summary>
		public const String MEMORY_POOL_MXBEAN_DOMAIN_TYPE = "java.lang:type=MemoryPool";

		/// <summary>
		/// Returns the managed bean for the class loading system of
		/// the Java virtual machine.
		/// </summary>
		/// <returns> a <seealso cref="ClassLoadingMXBean"/> object for
		/// the Java virtual machine. </returns>
		public static ClassLoadingMXBean ClassLoadingMXBean
		{
			get
			{
				return ManagementFactoryHelper.ClassLoadingMXBean;
			}
		}

		/// <summary>
		/// Returns the managed bean for the memory system of
		/// the Java virtual machine.
		/// </summary>
		/// <returns> a <seealso cref="MemoryMXBean"/> object for the Java virtual machine. </returns>
		public static MemoryMXBean MemoryMXBean
		{
			get
			{
				return ManagementFactoryHelper.MemoryMXBean;
			}
		}

		/// <summary>
		/// Returns the managed bean for the thread system of
		/// the Java virtual machine.
		/// </summary>
		/// <returns> a <seealso cref="ThreadMXBean"/> object for the Java virtual machine. </returns>
		public static ThreadMXBean ThreadMXBean
		{
			get
			{
				return ManagementFactoryHelper.ThreadMXBean;
			}
		}

		/// <summary>
		/// Returns the managed bean for the runtime system of
		/// the Java virtual machine.
		/// </summary>
		/// <returns> a <seealso cref="RuntimeMXBean"/> object for the Java virtual machine.
		///  </returns>
		public static RuntimeMXBean RuntimeMXBean
		{
			get
			{
				return ManagementFactoryHelper.RuntimeMXBean;
			}
		}

		/// <summary>
		/// Returns the managed bean for the compilation system of
		/// the Java virtual machine.  This method returns <tt>null</tt>
		/// if the Java virtual machine has no compilation system.
		/// </summary>
		/// <returns> a <seealso cref="CompilationMXBean"/> object for the Java virtual
		///   machine or <tt>null</tt> if the Java virtual machine has
		///   no compilation system. </returns>
		public static CompilationMXBean CompilationMXBean
		{
			get
			{
				return ManagementFactoryHelper.CompilationMXBean;
			}
		}

		/// <summary>
		/// Returns the managed bean for the operating system on which
		/// the Java virtual machine is running.
		/// </summary>
		/// <returns> an <seealso cref="OperatingSystemMXBean"/> object for
		/// the Java virtual machine. </returns>
		public static OperatingSystemMXBean OperatingSystemMXBean
		{
			get
			{
				return ManagementFactoryHelper.OperatingSystemMXBean;
			}
		}

		/// <summary>
		/// Returns a list of <seealso cref="MemoryPoolMXBean"/> objects in the
		/// Java virtual machine.
		/// The Java virtual machine can have one or more memory pools.
		/// It may add or remove memory pools during execution.
		/// </summary>
		/// <returns> a list of <tt>MemoryPoolMXBean</tt> objects.
		///  </returns>
		public static IList<MemoryPoolMXBean> MemoryPoolMXBeans
		{
			get
			{
				return ManagementFactoryHelper.MemoryPoolMXBeans;
			}
		}

		/// <summary>
		/// Returns a list of <seealso cref="MemoryManagerMXBean"/> objects
		/// in the Java virtual machine.
		/// The Java virtual machine can have one or more memory managers.
		/// It may add or remove memory managers during execution.
		/// </summary>
		/// <returns> a list of <tt>MemoryManagerMXBean</tt> objects.
		///  </returns>
		public static IList<MemoryManagerMXBean> MemoryManagerMXBeans
		{
			get
			{
				return ManagementFactoryHelper.MemoryManagerMXBeans;
			}
		}


		/// <summary>
		/// Returns a list of <seealso cref="GarbageCollectorMXBean"/> objects
		/// in the Java virtual machine.
		/// The Java virtual machine may have one or more
		/// <tt>GarbageCollectorMXBean</tt> objects.
		/// It may add or remove <tt>GarbageCollectorMXBean</tt>
		/// during execution.
		/// </summary>
		/// <returns> a list of <tt>GarbageCollectorMXBean</tt> objects.
		///  </returns>
		public static IList<GarbageCollectorMXBean> GarbageCollectorMXBeans
		{
			get
			{
				return ManagementFactoryHelper.GarbageCollectorMXBeans;
			}
		}

		private static MBeanServer PlatformMBeanServer_Renamed;
		/// <summary>
		/// Returns the platform <seealso cref="javax.management.MBeanServer MBeanServer"/>.
		/// On the first call to this method, it first creates the platform
		/// {@code MBeanServer} by calling the
		/// {@link javax.management.MBeanServerFactory#createMBeanServer
		/// MBeanServerFactory.createMBeanServer}
		/// method and registers each platform MXBean in this platform
		/// {@code MBeanServer} with its
		/// <seealso cref="PlatformManagedObject#getObjectName ObjectName"/>.
		/// This method, in subsequent calls, will simply return the
		/// initially created platform {@code MBeanServer}.
		/// <para>
		/// MXBeans that get created and destroyed dynamically, for example,
		/// memory <seealso cref="MemoryPoolMXBean pools"/> and
		/// <seealso cref="MemoryManagerMXBean managers"/>,
		/// will automatically be registered and deregistered into the platform
		/// {@code MBeanServer}.
		/// </para>
		/// <para>
		/// If the system property {@code javax.management.builder.initial}
		/// is set, the platform {@code MBeanServer} creation will be done
		/// by the specified <seealso cref="javax.management.MBeanServerBuilder"/>.
		/// </para>
		/// <para>
		/// It is recommended that this platform MBeanServer also be used
		/// to register other application managed beans
		/// besides the platform MXBeans.
		/// This will allow all MBeans to be published through the same
		/// {@code MBeanServer} and hence allow for easier network publishing
		/// and discovery.
		/// Name conflicts with the platform MXBeans should be avoided.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the platform {@code MBeanServer}; the platform
		///         MXBeans are registered into the platform {@code MBeanServer}
		///         at the first time this method is called.
		/// </returns>
		/// <exception cref="SecurityException"> if there is a security manager
		/// and the caller does not have the permission required by
		/// <seealso cref="javax.management.MBeanServerFactory#createMBeanServer"/>.
		/// </exception>
		/// <seealso cref= javax.management.MBeanServerFactory </seealso>
		/// <seealso cref= javax.management.MBeanServerFactory#createMBeanServer </seealso>
		public static MBeanServer PlatformMBeanServer
		{
			get
			{
				lock (typeof(ManagementFactory))
				{
					SecurityManager sm = System.SecurityManager;
					if (sm != null)
					{
						Permission perm = new MBeanServerPermission("createMBeanServer");
						sm.CheckPermission(perm);
					}
            
					if (PlatformMBeanServer_Renamed == null)
					{
						PlatformMBeanServer_Renamed = MBeanServerFactory.createMBeanServer();
						foreach (PlatformComponent pc in PlatformComponent.values())
						{
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: java.util.List<? extends PlatformManagedObject> list = pc.getMXBeans(pc.getMXBeanInterface());
							IList<?> list = pc.getMXBeans(pc.MXBeanInterface);
							foreach (PlatformManagedObject o in list)
							{
								// Each PlatformComponent represents one management
								// interface. Some MXBean may extend another one.
								// The MXBean instances for one platform component
								// (returned by pc.getMXBeans()) might be also
								// the MXBean instances for another platform component.
								// e.g. com.sun.management.GarbageCollectorMXBean
								//
								// So need to check if an MXBean instance is registered
								// before registering into the platform MBeanServer
								if (!PlatformMBeanServer_Renamed.isRegistered(o.ObjectName))
								{
									AddMXBean(PlatformMBeanServer_Renamed, o);
								}
							}
						}
						Dictionary<ObjectName, DynamicMBean> dynmbeans = ManagementFactoryHelper.PlatformDynamicMBeans;
						foreach (java.util.Map_Entry<ObjectName, DynamicMBean> e in dynmbeans)
						{
							AddDynamicMBean(PlatformMBeanServer_Renamed, e.Value, e.Key);
						}
						foreach (PlatformManagedObject o in ExtendedPlatformComponent.MXBeans)
						{
							if (!PlatformMBeanServer_Renamed.isRegistered(o.ObjectName))
							{
								AddMXBean(PlatformMBeanServer_Renamed, o);
							}
						}
					}
					return PlatformMBeanServer_Renamed;
				}
			}
		}

		/// <summary>
		/// Returns a proxy for a platform MXBean interface of a
		/// given <a href="#MXBeanNames">MXBean name</a>
		/// that forwards its method calls through the given
		/// <tt>MBeanServerConnection</tt>.
		/// 
		/// <para>This method is equivalent to:
		/// <blockquote>
		/// {@link java.lang.reflect.Proxy#newProxyInstance
		///        Proxy.newProxyInstance}<tt>(mxbeanInterface.getClassLoader(),
		///        new Class[] { mxbeanInterface }, handler)</tt>
		/// </blockquote>
		/// 
		/// where <tt>handler</tt> is an {@link java.lang.reflect.InvocationHandler
		/// InvocationHandler} to which method invocations to the MXBean interface
		/// are dispatched. This <tt>handler</tt> converts an input parameter
		/// from an MXBean data type to its mapped open type before forwarding
		/// to the <tt>MBeanServer</tt> and converts a return value from
		/// an MXBean method call through the <tt>MBeanServer</tt>
		/// from an open type to the corresponding return type declared in
		/// the MXBean interface.
		/// 
		/// </para>
		/// <para>
		/// If the MXBean is a notification emitter (i.e.,
		/// it implements
		/// <seealso cref="javax.management.NotificationEmitter NotificationEmitter"/>),
		/// both the <tt>mxbeanInterface</tt> and <tt>NotificationEmitter</tt>
		/// will be implemented by this proxy.
		/// 
		/// </para>
		/// <para>
		/// <b>Notes:</b>
		/// <ol>
		/// <li>Using an MXBean proxy is a convenience remote access to
		/// a platform MXBean of a running virtual machine.  All method
		/// calls to the MXBean proxy are forwarded to an
		/// <tt>MBeanServerConnection</tt> where
		/// <seealso cref="java.io.IOException IOException"/> may be thrown
		/// when the communication problem occurs with the connector server.
		/// An application remotely accesses the platform MXBeans using
		/// proxy should prepare to catch <tt>IOException</tt> as if
		/// accessing with the <tt>MBeanServerConnector</tt> interface.</li>
		/// 
		/// <li>When a client application is designed to remotely access MXBeans
		/// for a running virtual machine whose version is different than
		/// the version on which the application is running,
		/// it should prepare to catch
		/// <seealso cref="java.io.InvalidObjectException InvalidObjectException"/>
		/// which is thrown when an MXBean proxy receives a name of an
		/// enum constant which is missing in the enum class loaded in
		/// the client application. </li>
		/// 
		/// <li>{@link javax.management.MBeanServerInvocationHandler
		/// MBeanServerInvocationHandler} or its
		/// {@link javax.management.MBeanServerInvocationHandler#newProxyInstance
		/// newProxyInstance} method cannot be used to create
		/// a proxy for a platform MXBean. The proxy object created
		/// by <tt>MBeanServerInvocationHandler</tt> does not handle
		/// the properties of the platform MXBeans described in
		/// the <a href="#MXBean">class specification</a>.
		/// </li>
		/// </ol>
		/// 
		/// </para>
		/// </summary>
		/// <param name="connection"> the <tt>MBeanServerConnection</tt> to forward to. </param>
		/// <param name="mxbeanName"> the name of a platform MXBean within
		/// <tt>connection</tt> to forward to. <tt>mxbeanName</tt> must be
		/// in the format of <seealso cref="ObjectName ObjectName"/>. </param>
		/// <param name="mxbeanInterface"> the MXBean interface to be implemented
		/// by the proxy. </param>
		/// @param <T> an {@code mxbeanInterface} type parameter
		/// </param>
		/// <returns> a proxy for a platform MXBean interface of a
		/// given <a href="#MXBeanNames">MXBean name</a>
		/// that forwards its method calls through the given
		/// <tt>MBeanServerConnection</tt>, or {@code null} if not exist.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if
		/// <ul>
		/// <li><tt>mxbeanName</tt> is not with a valid
		///     <seealso cref="ObjectName ObjectName"/> format, or</li>
		/// <li>the named MXBean in the <tt>connection</tt> is
		///     not a MXBean provided by the platform, or</li>
		/// <li>the named MXBean is not registered in the
		///     <tt>MBeanServerConnection</tt>, or</li>
		/// <li>the named MXBean is not an instance of the given
		///     <tt>mxbeanInterface</tt></li>
		/// </ul>
		/// </exception>
		/// <exception cref="java.io.IOException"> if a communication problem
		/// occurred when accessing the <tt>MBeanServerConnection</tt>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T> T newPlatformMXBeanProxy(javax.management.MBeanServerConnection connection, String mxbeanName, Class mxbeanInterface) throws java.io.IOException
		public static T newPlatformMXBeanProxy<T>(MBeanServerConnection connection, String mxbeanName, Class mxbeanInterface)
		{

			// Only allow MXBean interfaces from rt.jar loaded by the
			// bootstrap class loader
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class cls = mxbeanInterface;
			Class cls = mxbeanInterface;
			ClassLoader loader = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(cls));
			if (!sun.misc.VM.isSystemDomainLoader(loader))
			{
				throw new IllegalArgumentException(mxbeanName + " is not a platform MXBean");
			}

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.management.ObjectName objName = new javax.management.ObjectName(mxbeanName);
				ObjectName objName = new ObjectName(mxbeanName);
				// skip the isInstanceOf check for LoggingMXBean
				String intfName = mxbeanInterface.Name;
				if (!connection.isInstanceOf(objName, intfName))
				{
					throw new IllegalArgumentException(mxbeanName + " is not an instance of " + mxbeanInterface);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class[] interfaces;
				Class[] interfaces;
				// check if the registered MBean is a notification emitter
				bool emitter = connection.isInstanceOf(objName, NOTIF_EMITTER);

				// create an MXBean proxy
				return JMX.newMXBeanProxy(connection, objName, mxbeanInterface, emitter);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (InstanceNotFoundException | MalformedObjectNameException e)
			{
				throw new IllegalArgumentException(e);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<ClassLoader>
		{
			private Type Cls;

			public PrivilegedActionAnonymousInnerClassHelper(Type cls)
			{
				this.Cls = cls;
			}

			public virtual ClassLoader Run()
			{
				return Cls.ClassLoader;
			}
		}

		/// <summary>
		/// Returns the platform MXBean implementing
		/// the given {@code mxbeanInterface} which is specified
		/// to have one single instance in the Java virtual machine.
		/// This method may return {@code null} if the management interface
		/// is not implemented in the Java virtual machine (for example,
		/// a Java virtual machine with no compilation system does not
		/// implement <seealso cref="CompilationMXBean"/>);
		/// otherwise, this method is equivalent to calling:
		/// <pre>
		///    {@link #getPlatformMXBeans(Class)
		///      getPlatformMXBeans(mxbeanInterface)}.get(0);
		/// </pre>
		/// </summary>
		/// <param name="mxbeanInterface"> a management interface for a platform
		///     MXBean with one single instance in the Java virtual machine
		///     if implemented. </param>
		/// @param <T> an {@code mxbeanInterface} type parameter
		/// </param>
		/// <returns> the platform MXBean that implements
		/// {@code mxbeanInterface}, or {@code null} if not exist.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if {@code mxbeanInterface}
		/// is not a platform management interface or
		/// not a singleton platform MXBean.
		/// 
		/// @since 1.7 </exception>
		public static T getPlatformMXBean<T>(Class mxbeanInterface) where T : PlatformManagedObject
		{
			PlatformComponent pc = PlatformComponent.getPlatformComponent(mxbeanInterface);
			if (pc == null)
			{
				T mbean = ExtendedPlatformComponent.getMXBean(mxbeanInterface);
				if (mbean != null)
				{
					return mbean;
				}
				throw new IllegalArgumentException(mxbeanInterface.Name + " is not a platform management interface");
			}
			if (!pc.Singleton)
			{
				throw new IllegalArgumentException(mxbeanInterface.Name + " can have zero or more than one instances");
			}

			return pc.getSingletonMXBean(mxbeanInterface);
		}

		/// <summary>
		/// Returns the list of platform MXBeans implementing
		/// the given {@code mxbeanInterface} in the Java
		/// virtual machine.
		/// The returned list may contain zero, one, or more instances.
		/// The number of instances in the returned list is defined
		/// in the specification of the given management interface.
		/// The order is undefined and there is no guarantee that
		/// the list returned is in the same order as previous invocations.
		/// </summary>
		/// <param name="mxbeanInterface"> a management interface for a platform
		///                        MXBean </param>
		/// @param <T> an {@code mxbeanInterface} type parameter
		/// </param>
		/// <returns> the list of platform MXBeans that implement
		/// {@code mxbeanInterface}.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if {@code mxbeanInterface}
		/// is not a platform management interface.
		/// 
		/// @since 1.7 </exception>
		public static IList<T> getPlatformMXBeans<T>(Class mxbeanInterface) where T : PlatformManagedObject
		{
			PlatformComponent pc = PlatformComponent.getPlatformComponent(mxbeanInterface);
			if (pc == null)
			{
				T mbean = ExtendedPlatformComponent.getMXBean(mxbeanInterface);
				if (mbean != null)
				{
					return Collections.SingletonList(mbean);
				}
				throw new IllegalArgumentException(mxbeanInterface.Name + " is not a platform management interface");
			}
			return Collections.UnmodifiableList(pc.getMXBeans(mxbeanInterface));
		}

		/// <summary>
		/// Returns the platform MXBean proxy for
		/// {@code mxbeanInterface} which is specified to have one single
		/// instance in a Java virtual machine and the proxy will
		/// forward the method calls through the given {@code MBeanServerConnection}.
		/// This method may return {@code null} if the management interface
		/// is not implemented in the Java virtual machine being monitored
		/// (for example, a Java virtual machine with no compilation system
		/// does not implement <seealso cref="CompilationMXBean"/>);
		/// otherwise, this method is equivalent to calling:
		/// <pre>
		///     {@link #getPlatformMXBeans(MBeanServerConnection, Class)
		///        getPlatformMXBeans(connection, mxbeanInterface)}.get(0);
		/// </pre>
		/// </summary>
		/// <param name="connection"> the {@code MBeanServerConnection} to forward to. </param>
		/// <param name="mxbeanInterface"> a management interface for a platform
		///     MXBean with one single instance in the Java virtual machine
		///     being monitored, if implemented. </param>
		/// @param <T> an {@code mxbeanInterface} type parameter
		/// </param>
		/// <returns> the platform MXBean proxy for
		/// forwarding the method calls of the {@code mxbeanInterface}
		/// through the given {@code MBeanServerConnection},
		/// or {@code null} if not exist.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if {@code mxbeanInterface}
		/// is not a platform management interface or
		/// not a singleton platform MXBean. </exception>
		/// <exception cref="java.io.IOException"> if a communication problem
		/// occurred when accessing the {@code MBeanServerConnection}.
		/// </exception>
		/// <seealso cref= #newPlatformMXBeanProxy
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T extends PlatformManagedObject> T getPlatformMXBean(javax.management.MBeanServerConnection connection, Class mxbeanInterface) throws java.io.IOException
		public static T getPlatformMXBean<T>(MBeanServerConnection connection, Class mxbeanInterface) where T : PlatformManagedObject
		{
			PlatformComponent pc = PlatformComponent.getPlatformComponent(mxbeanInterface);
			if (pc == null)
			{
				T mbean = ExtendedPlatformComponent.getMXBean(mxbeanInterface);
				if (mbean != null)
				{
					ObjectName on = mbean.ObjectName;
					return ManagementFactory.NewPlatformMXBeanProxy(connection, on.CanonicalName, mxbeanInterface);
				}
				throw new IllegalArgumentException(mxbeanInterface.Name + " is not a platform management interface");
			}
			if (!pc.Singleton)
			{
				throw new IllegalArgumentException(mxbeanInterface.Name + " can have zero or more than one instances");
			}
			return pc.getSingletonMXBean(connection, mxbeanInterface);
		}

		/// <summary>
		/// Returns the list of the platform MXBean proxies for
		/// forwarding the method calls of the {@code mxbeanInterface}
		/// through the given {@code MBeanServerConnection}.
		/// The returned list may contain zero, one, or more instances.
		/// The number of instances in the returned list is defined
		/// in the specification of the given management interface.
		/// The order is undefined and there is no guarantee that
		/// the list returned is in the same order as previous invocations.
		/// </summary>
		/// <param name="connection"> the {@code MBeanServerConnection} to forward to. </param>
		/// <param name="mxbeanInterface"> a management interface for a platform
		///                        MXBean </param>
		/// @param <T> an {@code mxbeanInterface} type parameter
		/// </param>
		/// <returns> the list of platform MXBean proxies for
		/// forwarding the method calls of the {@code mxbeanInterface}
		/// through the given {@code MBeanServerConnection}.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if {@code mxbeanInterface}
		/// is not a platform management interface.
		/// </exception>
		/// <exception cref="java.io.IOException"> if a communication problem
		/// occurred when accessing the {@code MBeanServerConnection}.
		/// </exception>
		/// <seealso cref= #newPlatformMXBeanProxy
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T extends PlatformManagedObject> java.util.List<T> getPlatformMXBeans(javax.management.MBeanServerConnection connection, Class mxbeanInterface) throws java.io.IOException
		public static IList<T> getPlatformMXBeans<T>(MBeanServerConnection connection, Class mxbeanInterface) where T : PlatformManagedObject
		{
			PlatformComponent pc = PlatformComponent.getPlatformComponent(mxbeanInterface);
			if (pc == null)
			{
				T mbean = ExtendedPlatformComponent.getMXBean(mxbeanInterface);
				if (mbean != null)
				{
					ObjectName on = mbean.ObjectName;
					T proxy = ManagementFactory.NewPlatformMXBeanProxy(connection, on.CanonicalName, mxbeanInterface);
					return Collections.SingletonList(proxy);
				}
				throw new IllegalArgumentException(mxbeanInterface.Name + " is not a platform management interface");
			}
			return Collections.UnmodifiableList(pc.getMXBeans(connection, mxbeanInterface));
		}

		/// <summary>
		/// Returns the set of {@code Class} objects, subinterface of
		/// <seealso cref="PlatformManagedObject"/>, representing
		/// all management interfaces for
		/// monitoring and managing the Java platform.
		/// </summary>
		/// <returns> the set of {@code Class} objects, subinterface of
		/// <seealso cref="PlatformManagedObject"/> representing
		/// the management interfaces for
		/// monitoring and managing the Java platform.
		/// 
		/// @since 1.7 </returns>
		public static Set<Class> PlatformManagementInterfaces
		{
			get
			{
				Set<Class> result = new HashSet<Class>();
				foreach (PlatformComponent component in PlatformComponent.values())
				{
					result.Add(component.MXBeanInterface);
				}
				return Collections.UnmodifiableSet(result);
			}
		}

		private const String NOTIF_EMITTER = "javax.management.NotificationEmitter";

		/// <summary>
		/// Registers an MXBean.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void addMXBean(final javax.management.MBeanServer mbs, final PlatformManagedObject pmo)
		private static void AddMXBean(MBeanServer mbs, PlatformManagedObject pmo)
		{
			// Make DynamicMBean out of MXBean by wrapping it with a StandardMBean
			try
			{
				AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(mbs, pmo));
			}
			catch (PrivilegedActionException e)
			{
				throw new RuntimeException(e.Exception);
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Void>
		{
			private MBeanServer Mbs;
			private java.lang.management.PlatformManagedObject Pmo;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(MBeanServer mbs, java.lang.management.PlatformManagedObject pmo)
			{
				this.Mbs = mbs;
				this.Pmo = pmo;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws javax.management.InstanceAlreadyExistsException, javax.management.MBeanRegistrationException, javax.management.NotCompliantMBeanException
			public virtual Void Run()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.management.DynamicMBean dmbean;
				DynamicMBean dmbean;
				if (Pmo is DynamicMBean)
				{
					dmbean = typeof(DynamicMBean).cast(Pmo);
				}
				else if (Pmo is NotificationEmitter)
				{
					dmbean = new StandardEmitterMBean(Pmo, null, true, (NotificationEmitter) Pmo);
				}
				else
				{
					dmbean = new StandardMBean(Pmo, null, true);
				}

				Mbs.registerMBean(dmbean, Pmo.ObjectName);
				return null;
			}
		}

		/// <summary>
		/// Registers a DynamicMBean.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void addDynamicMBean(final javax.management.MBeanServer mbs, final javax.management.DynamicMBean dmbean, final javax.management.ObjectName on)
		private static void AddDynamicMBean(MBeanServer mbs, DynamicMBean dmbean, ObjectName on)
		{
			try
			{
				AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper2(mbs, dmbean, on));
			}
			catch (PrivilegedActionException e)
			{
				throw new RuntimeException(e.Exception);
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper2 : PrivilegedExceptionAction<Void>
		{
			private MBeanServer Mbs;
			private DynamicMBean Dmbean;
			private ObjectName On;

			public PrivilegedExceptionActionAnonymousInnerClassHelper2(MBeanServer mbs, DynamicMBean dmbean, ObjectName on)
			{
				this.Mbs = mbs;
				this.Dmbean = dmbean;
				this.On = on;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void run() throws javax.management.InstanceAlreadyExistsException, javax.management.MBeanRegistrationException, javax.management.NotCompliantMBeanException
			public virtual Void Run()
			{
				Mbs.registerMBean(Dmbean, On);
				return null;
			}
		}
	}

}