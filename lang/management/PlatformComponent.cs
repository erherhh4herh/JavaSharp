using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2008, 2012, Oracle and/or its affiliates. All rights reserved.
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


	using HotSpotDiagnosticMXBean = com.sun.management.HotSpotDiagnosticMXBean;
	using UnixOperatingSystemMXBean = com.sun.management.UnixOperatingSystemMXBean;

	using ManagementFactoryHelper = sun.management.ManagementFactoryHelper;
	using Util = sun.management.Util;

	/// <summary>
	/// This enum class defines the list of platform components
	/// that provides monitoring and management support.
	/// Each enum represents one MXBean interface. A MXBean
	/// instance could implement one or more MXBean interfaces.
	/// 
	/// For example, com.sun.management.GarbageCollectorMXBean
	/// extends java.lang.management.GarbageCollectorMXBean
	/// and there is one set of garbage collection MXBean instances,
	/// each of which implements both c.s.m. and j.l.m. interfaces.
	/// There are two separate enums GARBAGE_COLLECTOR
	/// and SUN_GARBAGE_COLLECTOR so that ManagementFactory.getPlatformMXBeans(Class)
	/// will return the list of MXBeans of the specified type.
	/// 
	/// To add a new MXBean interface for the Java platform,
	/// add a new enum constant and implement the MXBeanFetcher.
	/// </summary>
	internal sealed class PlatformComponent
	{

		/// <summary>
		/// Class loading system of the Java virtual machine.
		/// </summary>
		CLASS_LOADING("java.lang.management.ClassLoadingMXBean",
		public static readonly PlatformComponent CLASS_LOADING("java.lang.management.ClassLoadingMXBean" = new PlatformComponent("CLASS_LOADING("java.lang.management.ClassLoadingMXBean"", InnerEnum.CLASS_LOADING("java.lang.management.ClassLoadingMXBean");
			"java.lang",
			"ClassLoading",
			defaultKeyProperties = ,
			public static readonly PlatformComponent "java.lang", "ClassLoading", defaultKeyProperties = new PlatformComponent(""java.lang", "ClassLoading", defaultKeyProperties", InnerEnum."java.lang", "ClassLoading", defaultKeyProperties,);
			true, // singleton
			public static readonly PlatformComponent true = new PlatformComponent("true", InnerEnum.true);
			new MXBeanFetcherAnonymousInnerClassHelper()),
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			new MXBeanFetcher<ClassLoadingMXBean>()

		/// <summary>
		/// Compilation system of the Java virtual machine.
		/// </summary>
		public static readonly PlatformComponent COMPILATION = new PlatformComponent("COMPILATION", InnerEnum.COMPILATION, "java.lang.management.CompilationMXBean", "java.lang", "Compilation", defaultKeyProperties(), true, new MXBeanFetcherAnonymousInnerClassHelper2());

		/// <summary>
		/// Memory system of the Java virtual machine.
		/// </summary>
		public static readonly PlatformComponent MEMORY = new PlatformComponent("MEMORY", InnerEnum.MEMORY, "java.lang.management.MemoryMXBean", "java.lang", "Memory", defaultKeyProperties(), true, new MXBeanFetcherAnonymousInnerClassHelper3());

		/// <summary>
		/// Garbage Collector in the Java virtual machine.
		/// </summary>
		public static readonly PlatformComponent GARBAGE_COLLECTOR = new PlatformComponent("GARBAGE_COLLECTOR", InnerEnum.GARBAGE_COLLECTOR, "java.lang.management.GarbageCollectorMXBean", "java.lang", "GarbageCollector", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper4());

		/// <summary>
		/// Memory manager in the Java virtual machine.
		/// </summary>
		MEMORY_MANAGER("java.lang.management.MemoryManagerMXBean", "java.lang", "MemoryManager", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper5(), // zero or more instances
		public static readonly PlatformComponent MEMORY_MANAGER("java.lang.management.MemoryManagerMXBean", "java.lang", "MemoryManager", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper5 = new PlatformComponent("MEMORY_MANAGER("java.lang.management.MemoryManagerMXBean", "java.lang", "MemoryManager", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper5", InnerEnum.MEMORY_MANAGER("java.lang.management.MemoryManagerMXBean", "java.lang", "MemoryManager", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper5, );
			GARBAGE_COLLECTOR),
			MEMORY_POOL = "java.lang.management.MemoryPoolMXBean", "java.lang", "MemoryPool", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper6(), // zero or more instances
			public static readonly PlatformComponent GARBAGE_COLLECTOR), MEMORY_POOL = new PlatformComponent("GARBAGE_COLLECTOR), MEMORY_POOL", InnerEnum.GARBAGE_COLLECTOR), MEMORY_POOL, "java.lang.management.MemoryPoolMXBean", "java.lang", "MemoryPool", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper6());

		/// <summary>
		/// Operating system on which the Java virtual machine is running
		/// </summary>
		public static readonly PlatformComponent OPERATING_SYSTEM = new PlatformComponent("OPERATING_SYSTEM", InnerEnum.OPERATING_SYSTEM, "java.lang.management.OperatingSystemMXBean", "java.lang", "OperatingSystem", defaultKeyProperties(), true, new MXBeanFetcherAnonymousInnerClassHelper7());

		/// <summary>
		/// Runtime system of the Java virtual machine.
		/// </summary>
		public static readonly PlatformComponent RUNTIME = new PlatformComponent("RUNTIME", InnerEnum.RUNTIME, "java.lang.management.RuntimeMXBean", "java.lang", "Runtime", defaultKeyProperties(), true, new MXBeanFetcherAnonymousInnerClassHelper8());

		/// <summary>
		/// Threading system of the Java virtual machine.
		/// </summary>
		public static readonly PlatformComponent THREADING = new PlatformComponent("THREADING", InnerEnum.THREADING, "java.lang.management.ThreadMXBean", "java.lang", "Threading", defaultKeyProperties(), true, new MXBeanFetcherAnonymousInnerClassHelper9());


		/// <summary>
		/// Logging facility.
		/// </summary>
		public static readonly PlatformComponent LOGGING = new PlatformComponent("LOGGING", InnerEnum.LOGGING, "java.lang.management.PlatformLoggingMXBean", "java.util.logging", "Logging", defaultKeyProperties(), true, new MXBeanFetcherAnonymousInnerClassHelper10());

		/// <summary>
		/// Buffer pools.
		/// </summary>
		public static readonly PlatformComponent BUFFER_POOL = new PlatformComponent("BUFFER_POOL", InnerEnum.BUFFER_POOL, "java.lang.management.BufferPoolMXBean", "java.nio", "BufferPool", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper11());


		// Sun Platform Extension

		/// <summary>
		/// Sun extension garbage collector that performs collections in cycles.
		/// </summary>
		public static readonly PlatformComponent SUN_GARBAGE_COLLECTOR = new PlatformComponent("SUN_GARBAGE_COLLECTOR", InnerEnum.SUN_GARBAGE_COLLECTOR, "com.sun.management.GarbageCollectorMXBean", "java.lang", "GarbageCollector", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper12());

		/// <summary>
		/// Sun extension operating system on which the Java virtual machine
		/// is running.
		/// </summary>
		public static readonly PlatformComponent SUN_OPERATING_SYSTEM = new PlatformComponent("SUN_OPERATING_SYSTEM", InnerEnum.SUN_OPERATING_SYSTEM, "com.sun.management.OperatingSystemMXBean", "java.lang", "OperatingSystem", defaultKeyProperties(), true, new MXBeanFetcherAnonymousInnerClassHelper13());

		/// <summary>
		/// Unix operating system.
		/// </summary>
		public static readonly PlatformComponent SUN_UNIX_OPERATING_SYSTEM = new PlatformComponent("SUN_UNIX_OPERATING_SYSTEM", InnerEnum.SUN_UNIX_OPERATING_SYSTEM, "com.sun.management.UnixOperatingSystemMXBean", "java.lang", "OperatingSystem", defaultKeyProperties(), true, new MXBeanFetcherAnonymousInnerClassHelper14());

		/// <summary>
		/// Diagnostic support for the HotSpot Virtual Machine.
		/// </summary>
		public static readonly PlatformComponent HOTSPOT_DIAGNOSTIC = new PlatformComponent("HOTSPOT_DIAGNOSTIC", InnerEnum.HOTSPOT_DIAGNOSTIC, "com.sun.management.HotSpotDiagnosticMXBean", "com.sun.management", "HotSpotDiagnostic", defaultKeyProperties(), true, new MXBeanFetcherAnonymousInnerClassHelper15());

		private static readonly IList<PlatformComponent> valueList = new List<PlatformComponent>();

		static PlatformComponent()
		{
			valueList.Add(CLASS_LOADING("java.lang.management.ClassLoadingMXBean");
			valueList.Add("java.lang", "ClassLoading", defaultKeyProperties);
			valueList.Add(true);
			valueList.Add(COMPILATION);
			valueList.Add(MEMORY);
			valueList.Add(GARBAGE_COLLECTOR);
			valueList.Add(MEMORY_MANAGER("java.lang.management.MemoryManagerMXBean", "java.lang", "MemoryManager", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper5);
			valueList.Add(GARBAGE_COLLECTOR), MEMORY_POOL);
			valueList.Add(OPERATING_SYSTEM);
			valueList.Add(RUNTIME);
			valueList.Add(THREADING);
			valueList.Add(LOGGING);
			valueList.Add(BUFFER_POOL);
			valueList.Add(SUN_GARBAGE_COLLECTOR);
			valueList.Add(SUN_OPERATING_SYSTEM);
			valueList.Add(SUN_UNIX_OPERATING_SYSTEM);
			valueList.Add(HOTSPOT_DIAGNOSTIC);
		}

		public enum InnerEnum
		{
			CLASS_LOADING("java.lang.management.ClassLoadingMXBean",
			"java.lang", "ClassLoading", defaultKeyProperties,
			true,
			COMPILATION,
			MEMORY,
			GARBAGE_COLLECTOR,
			MEMORY_MANAGER("java.lang.management.MemoryManagerMXBean", "java.lang", "MemoryManager", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper5,
			GARBAGE_COLLECTOR), MEMORY_POOL,
			OPERATING_SYSTEM,
			RUNTIME,
			THREADING,
			LOGGING,
			BUFFER_POOL,
			SUN_GARBAGE_COLLECTOR,
			SUN_OPERATING_SYSTEM,
			SUN_UNIX_OPERATING_SYSTEM,
			HOTSPOT_DIAGNOSTIC
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;


		/// <summary>
		/// A task that returns the MXBeans for a component.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		interface MXBeanFetcher<T extends PlatformManagedObject>
		{
			public = 
			public static readonly PlatformComponent public java.util.List<T> getMXBeans = new PlatformComponent("public java.util.List<T> getMXBeans", InnerEnum.public java.util.List<T> getMXBeans,);

			private static readonly IList<PlatformComponent> valueList = new List<PlatformComponent>();

			static PlatformComponent()
			{
				valueList.Add(CLASS_LOADING("java.lang.management.ClassLoadingMXBean");
				valueList.Add("java.lang", "ClassLoading", defaultKeyProperties);
				valueList.Add(true);
				valueList.Add(COMPILATION);
				valueList.Add(MEMORY);
				valueList.Add(GARBAGE_COLLECTOR);
				valueList.Add(MEMORY_MANAGER("java.lang.management.MemoryManagerMXBean", "java.lang", "MemoryManager", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper5);
				valueList.Add(GARBAGE_COLLECTOR), MEMORY_POOL);
				valueList.Add(OPERATING_SYSTEM);
				valueList.Add(RUNTIME);
				valueList.Add(THREADING);
				valueList.Add(LOGGING);
				valueList.Add(BUFFER_POOL);
				valueList.Add(SUN_GARBAGE_COLLECTOR);
				valueList.Add(SUN_OPERATING_SYSTEM);
				valueList.Add(SUN_UNIX_OPERATING_SYSTEM);
				valueList.Add(HOTSPOT_DIAGNOSTIC);
				valueList.Add(public java.util.List<T> getMXBeans);
			}

			public enum InnerEnum
			{
				CLASS_LOADING("java.lang.management.ClassLoadingMXBean",
				"java.lang", "ClassLoading", defaultKeyProperties,
				true,
				COMPILATION,
				MEMORY,
				GARBAGE_COLLECTOR,
				MEMORY_MANAGER("java.lang.management.MemoryManagerMXBean", "java.lang", "MemoryManager", keyProperties("name"), false, new MXBeanFetcherAnonymousInnerClassHelper5,
				GARBAGE_COLLECTOR), MEMORY_POOL,
				OPERATING_SYSTEM,
				RUNTIME,
				THREADING,
				LOGGING,
				BUFFER_POOL,
				SUN_GARBAGE_COLLECTOR,
				SUN_OPERATING_SYSTEM,
				SUN_UNIX_OPERATING_SYSTEM,
				HOTSPOT_DIAGNOSTIC,
				public java.util.List<T> getMXBeans
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;
		}

		/*
		 * Returns a list of the GC MXBeans of the given type.
		 */
		private static java.util.List<T> getGcMXBeanList<T>(Class gcMXBeanIntf) where T : GarbageCollectorMXBean
		{
			IList<GarbageCollectorMXBean> list = ManagementFactoryHelper.GarbageCollectorMXBeans;
			IList<T> result = new List<T>(list.Count);
			foreach (GarbageCollectorMXBean m in list)
			{
				if (gcMXBeanIntf.isInstance(m))
				{
					result.Add(gcMXBeanIntf.Cast(m));
				}
			}
			return result;
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper extends MXBeanFetcher<ClassLoadingMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper()
			{
			}

			public java.util.List<ClassLoadingMXBean> MXBeans
			{
				get
				{
					return Collections.SingletonList(ManagementFactoryHelper.ClassLoadingMXBean);
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper2 extends MXBeanFetcher<CompilationMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper2()
			{
			}

			public java.util.List<CompilationMXBean> MXBeans
			{
				get
				{
					CompilationMXBean m = ManagementFactoryHelper.CompilationMXBean;
					if (m == null)
					{
					   return Collections.EmptyList();
					}
					else
					{
					   return Collections.SingletonList(m);
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper3 extends MXBeanFetcher<MemoryMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper3()
			{
			}

			public java.util.List<MemoryMXBean> MXBeans
			{
				get
				{
					return Collections.SingletonList(ManagementFactoryHelper.MemoryMXBean);
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper4 extends MXBeanFetcher<GarbageCollectorMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper4()
			{
			}

			public java.util.List<GarbageCollectorMXBean> MXBeans
			{
				get
				{
					return ManagementFactoryHelper.GarbageCollectorMXBeans;
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper5 extends MXBeanFetcher<MemoryManagerMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper5()
			{
			}

			public java.util.List<MemoryManagerMXBean> MXBeans
			{
				get
				{
					return ManagementFactoryHelper.MemoryManagerMXBeans;
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper6 extends MXBeanFetcher<MemoryPoolMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper6()
			{
			}

			/// <summary>
			/// Memory pool in the Java virtual machine.
			/// </summary>
			public java.util.List<MemoryPoolMXBean> MXBeans
			{
				get
				{
					return ManagementFactoryHelper.MemoryPoolMXBeans;
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper7 extends MXBeanFetcher<OperatingSystemMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper7()
			{
			}

			public java.util.List<OperatingSystemMXBean> MXBeans
			{
				get
				{
					return Collections.SingletonList(ManagementFactoryHelper.OperatingSystemMXBean);
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper8 extends MXBeanFetcher<RuntimeMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper8()
			{
			}

			public java.util.List<RuntimeMXBean> MXBeans
			{
				get
				{
					return Collections.SingletonList(ManagementFactoryHelper.RuntimeMXBean);
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper9 extends MXBeanFetcher<ThreadMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper9()
			{
			}

			public java.util.List<ThreadMXBean> MXBeans
			{
				get
				{
					return Collections.SingletonList(ManagementFactoryHelper.ThreadMXBean);
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper10 extends MXBeanFetcher<PlatformLoggingMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper10()
			{
			}

			public java.util.List<PlatformLoggingMXBean> MXBeans
			{
				get
				{
					PlatformLoggingMXBean m = ManagementFactoryHelper.PlatformLoggingMXBean;
					if (m == null)
					{
					   return Collections.EmptyList();
					}
					else
					{
					   return Collections.SingletonList(m);
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper11 extends MXBeanFetcher<BufferPoolMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper11()
			{
			}

			public java.util.List<BufferPoolMXBean> MXBeans
			{
				get
				{
					return ManagementFactoryHelper.BufferPoolMXBeans;
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper12 extends MXBeanFetcher<com.sun.management.GarbageCollectorMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper12()
			{
			}

			public java.util.List<com.sun.management.GarbageCollectorMXBean> MXBeans
			{
				get
				{
					return getGcMXBeanList(typeof(com.sun.management.GarbageCollectorMXBean));
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper13 extends MXBeanFetcher<com.sun.management.OperatingSystemMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper13()
			{
			}

			public java.util.List<com.sun.management.OperatingSystemMXBean> MXBeans
			{
				get
				{
					return getOSMXBeanList(typeof(com.sun.management.OperatingSystemMXBean));
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper14 extends MXBeanFetcher<com.sun.management.UnixOperatingSystemMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper14()
			{
			}

			public java.util.List<com.sun.management.UnixOperatingSystemMXBean> MXBeans
			{
				get
				{
					return getOSMXBeanList(typeof(UnixOperatingSystemMXBean));
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		private static class MXBeanFetcherAnonymousInnerClassHelper15 extends MXBeanFetcher<com.sun.management.HotSpotDiagnosticMXBean>
		{
			public MXBeanFetcherAnonymousInnerClassHelper15()
			{
			}

			public java.util.List<com.sun.management.HotSpotDiagnosticMXBean> MXBeans
			{
				get
				{
					return Collections.SingletonList(ManagementFactoryHelper.DiagnosticMXBean);
				}
			}
		}

		/*
		 * Returns the OS mxbean instance of the given type.
		 */
		private static java.util.List<T> getOSMXBeanList<T>(Class osMXBeanIntf)
		{
			OperatingSystemMXBean m = ManagementFactoryHelper.OperatingSystemMXBean;
			if (osMXBeanIntf.isInstance(m))
			{
				return Collections.SingletonList(osMXBeanIntf.Cast(m));
			}
			else
			{
				return Collections.EmptyList();
			}
		}

		private readonly String mxbeanInterfaceName;
		private readonly String domain;
		private readonly String type;
		private readonly java.util.Set<String> keyProperties;
		private readonly MXBeanFetcher<JavaToDotNetGenericWildcard> fetcher;
		private readonly PlatformComponent[] subComponents;
		private readonly bool singleton;

		private PlatformComponent<T1>(string name, InnerEnum innerEnum, String intfName, String domain, String type, java.util.Set<String> keyProperties, bool singleton, MXBeanFetcher<T1> fetcher, params PlatformComponent[] subComponents)
		{
			this.mxbeanInterfaceName = intfName;
			this.domain = domain;
			this.type = type;
			this.keyProperties = keyProperties;
			this.singleton = singleton;
			this.fetcher = fetcher;
			this.subComponents = subComponents;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		private static java.util.Set<String> defaultKeyProps;
		private static java.util.Set<String> DefaultKeyProperties()
		{
			if (defaultKeyProps == null)
			{
				defaultKeyProps = Collections.Singleton("type");
			}
			return defaultKeyProps;
		}

		private static java.util.Set<String> KeyProperties(params String[] keyNames)
		{
			Set<String> set = new HashSet<String>();
			set.Add("type");
			foreach (String s in keyNames)
			{
				set.Add(s);
			}
			return set;
		}

		internal boolean Singleton
		{
			get
			{
				return singleton;
			}
		}

		internal String MXBeanInterfaceName
		{
			get
			{
				return mxbeanInterfaceName;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Class getMXBeanInterface()
		internal Class MXBeanInterface
		{
			get
			{
				try
				{
					// Lazy loading the MXBean interface only when it is needed
					return (Class) Class.ForName(mxbeanInterfaceName, false, typeof(PlatformManagedObject).ClassLoader);
				}
				catch (ClassNotFoundException x)
				{
					throw new AssertionError(x);
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") <T extends PlatformManagedObject> java.util.List<T> getMXBeans(Class mxbeanInterface)
		internal java.util.List<T> getMXBeans<T>(Class mxbeanInterface)
		{
			return (IList<T>) fetcher.MXBeans;
		}

		internal T getSingletonMXBean<T>(Class mxbeanInterface)
		{
			if (!singleton)
			{
				throw new IllegalArgumentException(mxbeanInterfaceName + " can have zero or more than one instances");
			}

			IList<T> list = getMXBeans(mxbeanInterface);
			Debug.Assert(list.Count == 1);
			return list.Count == 0 ? null : list[0];
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T extends PlatformManagedObject> T getSingletonMXBean(javax.management.MBeanServerConnection mbs, Class mxbeanInterface) throws java.io.IOException
		internal T getSingletonMXBean<T>(javax.management.MBeanServerConnection mbs, Class mxbeanInterface)
		{
			if (!singleton)
			{
				throw new IllegalArgumentException(mxbeanInterfaceName + " can have zero or more than one instances");
			}

			// ObjectName of a singleton MXBean contains only domain and type
			Debug.Assert(keyProperties.size() == 1);
			String on = domain + ":type=" + type;
			return ManagementFactory.NewPlatformMXBeanProxy(mbs, on, mxbeanInterface);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: <T extends PlatformManagedObject> java.util.List<T> getMXBeans(javax.management.MBeanServerConnection mbs, Class mxbeanInterface) throws java.io.IOException
		internal java.util.List<T> getMXBeans<T>(javax.management.MBeanServerConnection mbs, Class mxbeanInterface)
		{
			IList<T> result = new List<T>();
			foreach (ObjectName on in getObjectNames(mbs))
			{
				result.Add(ManagementFactory.NewPlatformMXBeanProxy(mbs, on.CanonicalName, mxbeanInterface));
			}
			return result;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.util.Set<javax.management.ObjectName> getObjectNames(javax.management.MBeanServerConnection mbs) throws java.io.IOException
		private java.util.Set<javax.management.ObjectName> GetObjectNames(javax.management.MBeanServerConnection mbs)
		{
			String domainAndType = domain + ":type=" + type;
			if (keyProperties.size() > 1)
			{
				// if there are more than 1 key properties (i.e. other than "type")
				domainAndType += ",*";
			}
			ObjectName on = Util.newObjectName(domainAndType);
			Set<ObjectName> set = mbs.queryNames(on, null);
			foreach (PlatformComponent pc in subComponents)
			{
				set.AddAll(pc.getObjectNames(mbs));
			}
			return set;
		}

		// a map from MXBean interface name to PlatformComponent
		private static IDictionary<String, PlatformComponent> enumMap;
		private static void EnsureInitialized()
		{
			lock (typeof(PlatformComponent))
			{
				if (enumMap == null)
				{
					enumMap = new Dictionary<>();
					foreach (PlatformComponent pc in PlatformComponent.values())
					{
						// Use String as the key rather than Class<?> to avoid
						// causing unnecessary class loading of management interface
						enumMap.put(pc.MXBeanInterfaceName, pc);
					}
				}
			}
		}

		internal static boolean IsPlatformMXBean(String cn)
		{
			ensureInitialized();
			return enumMap.containsKey(cn);
		}

		internal static PlatformComponent getPlatformComponent<T>(Class mxbeanInterface)
		{
			ensureInitialized();
			String cn = mxbeanInterface.Name;
			PlatformComponent pc = enumMap.get(cn);
			if (pc != null && pc.MXBeanInterface == mxbeanInterface)
			{
				return pc;
			}
			return null;
		}

		private const long serialVersionUID = 6992337162326171013L;

		public static IList<PlatformComponent> values()
		{
			return valueList;
		}

		public InnerEnum InnerEnumValue()
		{
			return innerEnumValue;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static PlatformComponent valueOf(string name)
		{
			foreach (PlatformComponent enumInstance in PlatformComponent.values())
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}