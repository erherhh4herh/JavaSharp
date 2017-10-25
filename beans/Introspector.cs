using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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

namespace java.beans
{

	using TypeResolver = com.sun.beans.TypeResolver;
	using WeakCache = com.sun.beans.WeakCache;
	using ClassFinder = com.sun.beans.finder.ClassFinder;
	using MethodFinder = com.sun.beans.finder.MethodFinder;



	using ReflectUtil = sun.reflect.misc.ReflectUtil;

	/// <summary>
	/// The Introspector class provides a standard way for tools to learn about
	/// the properties, events, and methods supported by a target Java Bean.
	/// <para>
	/// For each of those three kinds of information, the Introspector will
	/// separately analyze the bean's class and superclasses looking for
	/// either explicit or implicit information and use that information to
	/// build a BeanInfo object that comprehensively describes the target bean.
	/// </para>
	/// <para>
	/// For each class "Foo", explicit information may be available if there exists
	/// a corresponding "FooBeanInfo" class that provides a non-null value when
	/// queried for the information.   We first look for the BeanInfo class by
	/// taking the full package-qualified name of the target bean class and
	/// appending "BeanInfo" to form a new class name.  If this fails, then
	/// we take the final classname component of this name, and look for that
	/// class in each of the packages specified in the BeanInfo package search
	/// path.
	/// </para>
	/// <para>
	/// Thus for a class such as "sun.xyz.OurButton" we would first look for a
	/// BeanInfo class called "sun.xyz.OurButtonBeanInfo" and if that failed we'd
	/// look in each package in the BeanInfo search path for an OurButtonBeanInfo
	/// class.  With the default search path, this would mean looking for
	/// "sun.beans.infos.OurButtonBeanInfo".
	/// </para>
	/// <para>
	/// If a class provides explicit BeanInfo about itself then we add that to
	/// the BeanInfo information we obtained from analyzing any derived classes,
	/// but we regard the explicit information as being definitive for the current
	/// class and its base classes, and do not proceed any further up the superclass
	/// chain.
	/// </para>
	/// <para>
	/// If we don't find explicit BeanInfo on a class, we use low-level
	/// reflection to study the methods of the class and apply standard design
	/// patterns to identify property accessors, event sources, or public
	/// methods.  We then proceed to analyze the class's superclass and add
	/// in the information from it (and possibly on up the superclass chain).
	/// </para>
	/// <para>
	/// For more information about introspection and design patterns, please
	/// consult the
	///  <a href="http://www.oracle.com/technetwork/java/javase/documentation/spec-136004.html">JavaBeans&trade; specification</a>.
	/// </para>
	/// </summary>

	public class Introspector
	{

		// Flags that can be used to control getBeanInfo:
		/// <summary>
		/// Flag to indicate to use of all beaninfo.
		/// </summary>
		public const int USE_ALL_BEANINFO = 1;
		/// <summary>
		/// Flag to indicate to ignore immediate beaninfo.
		/// </summary>
		public const int IGNORE_IMMEDIATE_BEANINFO = 2;
		/// <summary>
		/// Flag to indicate to ignore all beaninfo.
		/// </summary>
		public const int IGNORE_ALL_BEANINFO = 3;

		// Static Caches to speed up introspection.
		private static readonly WeakCache<Class, Method[]> DeclaredMethodCache = new WeakCache<Class, Method[]>();

		private Class BeanClass;
		private BeanInfo ExplicitBeanInfo;
		private BeanInfo SuperBeanInfo;
		private BeanInfo[] AdditionalBeanInfo;

		private bool PropertyChangeSource = false;
		private static Class EventListenerType = typeof(EventListener);

		// These should be removed.
		private String DefaultEventName;
		private String DefaultPropertyName;
		private int DefaultEventIndex = -1;
		private int DefaultPropertyIndex = -1;

		// Methods maps from Method names to MethodDescriptors
		private IDictionary<String, MethodDescriptor> Methods;

		// properties maps from String names to PropertyDescriptors
		private IDictionary<String, PropertyDescriptor> Properties;

		// events maps from String names to EventSetDescriptors
		private IDictionary<String, EventSetDescriptor> Events;

		private static readonly EventSetDescriptor[] EMPTY_EVENTSETDESCRIPTORS = new EventSetDescriptor[0];

		internal const String ADD_PREFIX = "add";
		internal const String REMOVE_PREFIX = "remove";
		internal const String GET_PREFIX = "get";
		internal const String SET_PREFIX = "set";
		internal const String IS_PREFIX = "is";

		//======================================================================
		//                          Public methods
		//======================================================================

		/// <summary>
		/// Introspect on a Java Bean and learn about all its properties, exposed
		/// methods, and events.
		/// <para>
		/// If the BeanInfo class for a Java Bean has been previously Introspected
		/// then the BeanInfo class is retrieved from the BeanInfo cache.
		/// 
		/// </para>
		/// </summary>
		/// <param name="beanClass">  The bean class to be analyzed. </param>
		/// <returns>  A BeanInfo object describing the target bean. </returns>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
		/// <seealso cref= #flushCaches </seealso>
		/// <seealso cref= #flushFromCaches </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static BeanInfo getBeanInfo(Class beanClass) throws IntrospectionException
		public static BeanInfo GetBeanInfo(Class beanClass)
		{
			if (!ReflectUtil.isPackageAccessible(beanClass))
			{
				return (new Introspector(beanClass, null, USE_ALL_BEANINFO)).BeanInfo;
			}
			ThreadGroupContext context = ThreadGroupContext.Context;
			BeanInfo beanInfo;
			lock (DeclaredMethodCache)
			{
				beanInfo = context.GetBeanInfo(beanClass);
			}
			if (beanInfo == null)
			{
				beanInfo = (new Introspector(beanClass, null, USE_ALL_BEANINFO)).BeanInfo;
				lock (DeclaredMethodCache)
				{
					context.PutBeanInfo(beanClass, beanInfo);
				}
			}
			return beanInfo;
		}

		/// <summary>
		/// Introspect on a Java bean and learn about all its properties, exposed
		/// methods, and events, subject to some control flags.
		/// <para>
		/// If the BeanInfo class for a Java Bean has been previously Introspected
		/// based on the same arguments then the BeanInfo class is retrieved
		/// from the BeanInfo cache.
		/// 
		/// </para>
		/// </summary>
		/// <param name="beanClass">  The bean class to be analyzed. </param>
		/// <param name="flags">  Flags to control the introspection.
		///     If flags == USE_ALL_BEANINFO then we use all of the BeanInfo
		///          classes we can discover.
		///     If flags == IGNORE_IMMEDIATE_BEANINFO then we ignore any
		///           BeanInfo associated with the specified beanClass.
		///     If flags == IGNORE_ALL_BEANINFO then we ignore all BeanInfo
		///           associated with the specified beanClass or any of its
		///           parent classes. </param>
		/// <returns>  A BeanInfo object describing the target bean. </returns>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static BeanInfo getBeanInfo(Class beanClass, int flags) throws IntrospectionException
		public static BeanInfo GetBeanInfo(Class beanClass, int flags)
		{
			return GetBeanInfo(beanClass, null, flags);
		}

		/// <summary>
		/// Introspect on a Java bean and learn all about its properties, exposed
		/// methods, below a given "stop" point.
		/// <para>
		/// If the BeanInfo class for a Java Bean has been previously Introspected
		/// based on the same arguments, then the BeanInfo class is retrieved
		/// from the BeanInfo cache.
		/// </para>
		/// </summary>
		/// <returns> the BeanInfo for the bean </returns>
		/// <param name="beanClass"> The bean class to be analyzed. </param>
		/// <param name="stopClass"> The baseclass at which to stop the analysis.  Any
		///    methods/properties/events in the stopClass or in its baseclasses
		///    will be ignored in the analysis. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static BeanInfo getBeanInfo(Class beanClass, Class stopClass) throws IntrospectionException
		public static BeanInfo GetBeanInfo(Class beanClass, Class stopClass)
		{
			return GetBeanInfo(beanClass, stopClass, USE_ALL_BEANINFO);
		}

		/// <summary>
		/// Introspect on a Java Bean and learn about all its properties,
		/// exposed methods and events, below a given {@code stopClass} point
		/// subject to some control {@code flags}.
		/// <dl>
		///  <dt>USE_ALL_BEANINFO</dt>
		///  <dd>Any BeanInfo that can be discovered will be used.</dd>
		///  <dt>IGNORE_IMMEDIATE_BEANINFO</dt>
		///  <dd>Any BeanInfo associated with the specified {@code beanClass} will be ignored.</dd>
		///  <dt>IGNORE_ALL_BEANINFO</dt>
		///  <dd>Any BeanInfo associated with the specified {@code beanClass}
		///      or any of its parent classes will be ignored.</dd>
		/// </dl>
		/// Any methods/properties/events in the {@code stopClass}
		/// or in its parent classes will be ignored in the analysis.
		/// <para>
		/// If the BeanInfo class for a Java Bean has been
		/// previously introspected based on the same arguments then
		/// the BeanInfo class is retrieved from the BeanInfo cache.
		/// 
		/// </para>
		/// </summary>
		/// <param name="beanClass">  the bean class to be analyzed </param>
		/// <param name="stopClass">  the parent class at which to stop the analysis </param>
		/// <param name="flags">      flags to control the introspection </param>
		/// <returns> a BeanInfo object describing the target bean </returns>
		/// <exception cref="IntrospectionException"> if an exception occurs during introspection
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static BeanInfo getBeanInfo(Class beanClass, Class stopClass, int flags) throws IntrospectionException
		public static BeanInfo GetBeanInfo(Class beanClass, Class stopClass, int flags)
		{
			BeanInfo bi;
			if (stopClass == null && flags == USE_ALL_BEANINFO)
			{
				// Same parameters to take advantage of caching.
				bi = GetBeanInfo(beanClass);
			}
			else
			{
				bi = (new Introspector(beanClass, stopClass, flags)).BeanInfo;
			}
			return bi;

			// Old behaviour: Make an independent copy of the BeanInfo.
			//return new GenericBeanInfo(bi);
		}


		/// <summary>
		/// Utility method to take a string and convert it to normal Java variable
		/// name capitalization.  This normally means converting the first
		/// character from upper case to lower case, but in the (unusual) special
		/// case when there is more than one character and both the first and
		/// second characters are upper case, we leave it alone.
		/// <para>
		/// Thus "FooBah" becomes "fooBah" and "X" becomes "x", but "URL" stays
		/// as "URL".
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> The string to be decapitalized. </param>
		/// <returns>  The decapitalized version of the string. </returns>
		public static String Decapitalize(String name)
		{
			if (name == null || name.Length() == 0)
			{
				return name;
			}
			if (name.Length() > 1 && char.IsUpper(name.CharAt(1)) && char.IsUpper(name.CharAt(0)))
			{
				return name;
			}
			char[] chars = name.ToCharArray();
			chars[0] = char.ToLower(chars[0]);
			return new String(chars);
		}

		/// <summary>
		/// Gets the list of package names that will be used for
		///          finding BeanInfo classes.
		/// </summary>
		/// <returns>  The array of package names that will be searched in
		///          order to find BeanInfo classes. The default value
		///          for this array is implementation-dependent; e.g.
		///          Sun implementation initially sets to {"sun.beans.infos"}. </returns>

		public static String[] BeanInfoSearchPath
		{
			get
			{
				return ThreadGroupContext.Context.BeanInfoFinder.Packages;
			}
			set
			{
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPropertiesAccess();
				}
				ThreadGroupContext.Context.BeanInfoFinder.Packages = value;
			}
		}




		/// <summary>
		/// Flush all of the Introspector's internal caches.  This method is
		/// not normally required.  It is normally only needed by advanced
		/// tools that update existing "Class" objects in-place and need
		/// to make the Introspector re-analyze existing Class objects.
		/// </summary>

		public static void FlushCaches()
		{
			lock (DeclaredMethodCache)
			{
				ThreadGroupContext.Context.ClearBeanInfoCache();
				DeclaredMethodCache.clear();
			}
		}

		/// <summary>
		/// Flush the Introspector's internal cached information for a given class.
		/// This method is not normally required.  It is normally only needed
		/// by advanced tools that update existing "Class" objects in-place
		/// and need to make the Introspector re-analyze an existing Class object.
		/// 
		/// Note that only the direct state associated with the target Class
		/// object is flushed.  We do not flush state for other Class objects
		/// with the same name, nor do we flush state for any related Class
		/// objects (such as subclasses), even though their state may include
		/// information indirectly obtained from the target Class object.
		/// </summary>
		/// <param name="clz">  Class object to be flushed. </param>
		/// <exception cref="NullPointerException"> If the Class object is null. </exception>
		public static void FlushFromCaches(Class clz)
		{
			if (clz == null)
			{
				throw new NullPointerException();
			}
			lock (DeclaredMethodCache)
			{
				ThreadGroupContext.Context.RemoveBeanInfo(clz);
				DeclaredMethodCache.put(clz, null);
			}
		}

		//======================================================================
		//                  Private implementation methods
		//======================================================================

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Introspector(Class beanClass, Class stopClass, int flags) throws IntrospectionException
		private Introspector(Class beanClass, Class stopClass, int flags)
		{
			this.BeanClass = beanClass;

			// Check stopClass is a superClass of startClass.
			if (stopClass != null)
			{
				bool isSuper = false;
				for (Class c = beanClass.BaseType; c != null; c = c.BaseType)
				{
					if (c == stopClass)
					{
						isSuper = true;
					}
				}
				if (!isSuper)
				{
					throw new IntrospectionException(stopClass.Name + " not superclass of " + beanClass.Name);
				}
			}

			if (flags == USE_ALL_BEANINFO)
			{
				ExplicitBeanInfo = FindExplicitBeanInfo(beanClass);
			}

			Class superClass = beanClass.BaseType;
			if (superClass != stopClass)
			{
				int newFlags = flags;
				if (newFlags == IGNORE_IMMEDIATE_BEANINFO)
				{
					newFlags = USE_ALL_BEANINFO;
				}
				SuperBeanInfo = GetBeanInfo(superClass, stopClass, newFlags);
			}
			if (ExplicitBeanInfo != null)
			{
				AdditionalBeanInfo = ExplicitBeanInfo.AdditionalBeanInfo;
			}
			if (AdditionalBeanInfo == null)
			{
				AdditionalBeanInfo = new BeanInfo[0];
			}
		}

		/// <summary>
		/// Constructs a GenericBeanInfo class from the state of the Introspector
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private BeanInfo getBeanInfo() throws IntrospectionException
		private BeanInfo BeanInfo
		{
			get
			{
    
				// the evaluation order here is import, as we evaluate the
				// event sets and locate PropertyChangeListeners before we
				// look for properties.
				BeanDescriptor bd = TargetBeanDescriptor;
				MethodDescriptor[] mds = TargetMethodInfo;
				EventSetDescriptor[] esds = TargetEventInfo;
				PropertyDescriptor[] pds = TargetPropertyInfo;
    
				int defaultEvent = TargetDefaultEventIndex;
				int defaultProperty = TargetDefaultPropertyIndex;
    
				return new GenericBeanInfo(bd, esds, defaultEvent, pds, defaultProperty, mds, ExplicitBeanInfo);
    
			}
		}

		/// <summary>
		/// Looks for an explicit BeanInfo class that corresponds to the Class.
		/// First it looks in the existing package that the Class is defined in,
		/// then it checks to see if the class is its own BeanInfo. Finally,
		/// the BeanInfo search path is prepended to the class and searched.
		/// </summary>
		/// <param name="beanClass">  the class type of the bean </param>
		/// <returns> Instance of an explicit BeanInfo class or null if one isn't found. </returns>
		private static BeanInfo FindExplicitBeanInfo(Class beanClass)
		{
			return ThreadGroupContext.Context.BeanInfoFinder.find(beanClass);
		}

		/// <returns> An array of PropertyDescriptors describing the editable
		/// properties supported by the target bean. </returns>

		private PropertyDescriptor[] TargetPropertyInfo
		{
			get
			{
    
				// Check if the bean has its own BeanInfo that will provide
				// explicit information.
				PropertyDescriptor[] explicitProperties = null;
				if (ExplicitBeanInfo != null)
				{
					explicitProperties = GetPropertyDescriptors(this.ExplicitBeanInfo);
				}
    
				if (explicitProperties == null && SuperBeanInfo != null)
				{
					// We have no explicit BeanInfo properties.  Check with our parent.
					AddPropertyDescriptors(GetPropertyDescriptors(this.SuperBeanInfo));
				}
    
				for (int i = 0; i < AdditionalBeanInfo.Length; i++)
				{
					AddPropertyDescriptors(AdditionalBeanInfo[i].PropertyDescriptors);
				}
    
				if (explicitProperties != null)
				{
					// Add the explicit BeanInfo data to our results.
					AddPropertyDescriptors(explicitProperties);
    
				}
				else
				{
    
					// Apply some reflection to the current class.
    
					// First get an array of all the public methods at this level
					Method[] methodList = GetPublicDeclaredMethods(BeanClass);
    
					// Now analyze each method.
					for (int i = 0; i < methodList.Length; i++)
					{
						Method method = methodList[i];
						if (method == null)
						{
							continue;
						}
						// skip static methods.
						int mods = method.Modifiers;
						if (Modifier.isStatic(mods))
						{
							continue;
						}
						String name = method.Name;
						Class[] argTypes = method.ParameterTypes;
						Class resultType = method.ReturnType;
						int argCount = argTypes.Length;
						PropertyDescriptor pd = null;
    
						if (name.Length() <= 3 && !name.StartsWith(IS_PREFIX))
						{
							// Optimization. Don't bother with invalid propertyNames.
							continue;
						}
    
						try
						{
    
							if (argCount == 0)
							{
								if (name.StartsWith(GET_PREFIX))
								{
									// Simple getter
									pd = new PropertyDescriptor(this.BeanClass, name.Substring(3), method, null);
								}
								else if (resultType == typeof(bool) && name.StartsWith(IS_PREFIX))
								{
									// Boolean getter
									pd = new PropertyDescriptor(this.BeanClass, name.Substring(2), method, null);
								}
							}
							else if (argCount == 1)
							{
								if (typeof(int).Equals(argTypes[0]) && name.StartsWith(GET_PREFIX))
								{
									pd = new IndexedPropertyDescriptor(this.BeanClass, name.Substring(3), null, null, method, null);
								}
								else if (typeof(void).Equals(resultType) && name.StartsWith(SET_PREFIX))
								{
									// Simple setter
									pd = new PropertyDescriptor(this.BeanClass, name.Substring(3), null, method);
									if (ThrowsException(method, typeof(PropertyVetoException)))
									{
										pd.Constrained = true;
									}
								}
							}
							else if (argCount == 2)
							{
									if (typeof(void).Equals(resultType) && typeof(int).Equals(argTypes[0]) && name.StartsWith(SET_PREFIX))
									{
									pd = new IndexedPropertyDescriptor(this.BeanClass, name.Substring(3), null, null, null, method);
									if (ThrowsException(method, typeof(PropertyVetoException)))
									{
										pd.Constrained = true;
									}
									}
							}
						}
						catch (IntrospectionException)
						{
							// This happens if a PropertyDescriptor or IndexedPropertyDescriptor
							// constructor fins that the method violates details of the deisgn
							// pattern, e.g. by having an empty name, or a getter returning
							// void , or whatever.
							pd = null;
						}
    
						if (pd != null)
						{
							// If this class or one of its base classes is a PropertyChange
							// source, then we assume that any properties we discover are "bound".
							if (PropertyChangeSource)
							{
								pd.Bound = true;
							}
							AddPropertyDescriptor(pd);
						}
					}
				}
				ProcessPropertyDescriptors();
    
				// Allocate and populate the result array.
				PropertyDescriptor[] result = Properties.Values.toArray(new PropertyDescriptor[Properties.Count]);
    
				// Set the default index.
				if (DefaultPropertyName != null)
				{
					for (int i = 0; i < result.Length; i++)
					{
						if (DefaultPropertyName.Equals(result[i].Name))
						{
							DefaultPropertyIndex = i;
						}
					}
				}
    
				return result;
			}
		}

		private Dictionary<String, IList<PropertyDescriptor>> PdStore = new Dictionary<String, IList<PropertyDescriptor>>();

		/// <summary>
		/// Adds the property descriptor to the list store.
		/// </summary>
		private void AddPropertyDescriptor(PropertyDescriptor pd)
		{
			String propName = pd.Name;
			IList<PropertyDescriptor> list = PdStore[propName];
			if (list == null)
			{
				list = new List<>();
				PdStore[propName] = list;
			}
			if (this.BeanClass != pd.Class0)
			{
				// replace existing property descriptor
				// only if we have types to resolve
				// in the context of this.beanClass
				Method read = pd.ReadMethod;
				Method write = pd.WriteMethod;
				bool cls = true;
				if (read != null)
				{
					cls = cls && read.GenericReturnType is Class;
				}
				if (write != null)
				{
					cls = cls && write.GenericParameterTypes[0] is Class;
				}
				if (pd is IndexedPropertyDescriptor)
				{
					IndexedPropertyDescriptor ipd = (IndexedPropertyDescriptor) pd;
					Method readI = ipd.IndexedReadMethod;
					Method writeI = ipd.IndexedWriteMethod;
					if (readI != null)
					{
						cls = cls && readI.GenericReturnType is Class;
					}
					if (writeI != null)
					{
						cls = cls && writeI.GenericParameterTypes[1] is Class;
					}
					if (!cls)
					{
						pd = new IndexedPropertyDescriptor(ipd);
						pd.UpdateGenericsFor(this.BeanClass);
					}
				}
				else if (!cls)
				{
					pd = new PropertyDescriptor(pd);
					pd.UpdateGenericsFor(this.BeanClass);
				}
			}
			list.Add(pd);
		}

		private void AddPropertyDescriptors(PropertyDescriptor[] descriptors)
		{
			if (descriptors != null)
			{
				foreach (PropertyDescriptor descriptor in descriptors)
				{
					AddPropertyDescriptor(descriptor);
				}
			}
		}

		private PropertyDescriptor[] GetPropertyDescriptors(BeanInfo info)
		{
			PropertyDescriptor[] descriptors = info.PropertyDescriptors;
			int index = info.DefaultPropertyIndex;
			if ((0 <= index) && (index < descriptors.Length))
			{
				this.DefaultPropertyName = descriptors[index].Name;
			}
			return descriptors;
		}

		/// <summary>
		/// Populates the property descriptor table by merging the
		/// lists of Property descriptors.
		/// </summary>
		private void ProcessPropertyDescriptors()
		{
			if (Properties == null)
			{
				Properties = new SortedDictionary<>();
			}

			IList<PropertyDescriptor> list;

			PropertyDescriptor pd, gpd, spd;
			IndexedPropertyDescriptor ipd, igpd, ispd;

			IEnumerator<IList<PropertyDescriptor>> it = PdStore.Values.GetEnumerator();
			while (it.MoveNext())
			{
				pd = null;
				gpd = null;
				spd = null;
				ipd = null;
				igpd = null;
				ispd = null;

				list = it.Current;

				// First pass. Find the latest getter method. Merge properties
				// of previous getter methods.
				for (int i = 0; i < list.Count; i++)
				{
					pd = list[i];
					if (pd is IndexedPropertyDescriptor)
					{
						ipd = (IndexedPropertyDescriptor)pd;
						if (ipd.IndexedReadMethod != null)
						{
							if (igpd != null)
							{
								igpd = new IndexedPropertyDescriptor(igpd, ipd);
							}
							else
							{
								igpd = ipd;
							}
						}
					}
					else
					{
						if (pd.ReadMethod != null)
						{
							String pdName = pd.ReadMethod.Name;
							if (gpd != null)
							{
								// Don't replace the existing read
								// method if it starts with "is"
								String gpdName = gpd.ReadMethod.Name;
								if (gpdName.Equals(pdName) || !gpdName.StartsWith(IS_PREFIX))
								{
									gpd = new PropertyDescriptor(gpd, pd);
								}
							}
							else
							{
								gpd = pd;
							}
						}
					}
				}

				// Second pass. Find the latest setter method which
				// has the same type as the getter method.
				for (int i = 0; i < list.Count; i++)
				{
					pd = list[i];
					if (pd is IndexedPropertyDescriptor)
					{
						ipd = (IndexedPropertyDescriptor)pd;
						if (ipd.IndexedWriteMethod != null)
						{
							if (igpd != null)
							{
								if (IsAssignable(igpd.IndexedPropertyType, ipd.IndexedPropertyType))
								{
									if (ispd != null)
									{
										ispd = new IndexedPropertyDescriptor(ispd, ipd);
									}
									else
									{
										ispd = ipd;
									}
								}
							}
							else
							{
								if (ispd != null)
								{
									ispd = new IndexedPropertyDescriptor(ispd, ipd);
								}
								else
								{
									ispd = ipd;
								}
							}
						}
					}
					else
					{
						if (pd.WriteMethod != null)
						{
							if (gpd != null)
							{
								if (IsAssignable(gpd.PropertyType, pd.PropertyType))
								{
									if (spd != null)
									{
										spd = new PropertyDescriptor(spd, pd);
									}
									else
									{
										spd = pd;
									}
								}
							}
							else
							{
								if (spd != null)
								{
									spd = new PropertyDescriptor(spd, pd);
								}
								else
								{
									spd = pd;
								}
							}
						}
					}
				}

				// At this stage we should have either PDs or IPDs for the
				// representative getters and setters. The order at which the
				// property descriptors are determined represent the
				// precedence of the property ordering.
				pd = null;
				ipd = null;

				if (igpd != null && ispd != null)
				{
					// Complete indexed properties set
					// Merge any classic property descriptors
					if ((gpd == spd) || (gpd == null))
					{
						pd = spd;
					}
					else if (spd == null)
					{
						pd = gpd;
					}
					else if (spd is IndexedPropertyDescriptor)
					{
						pd = MergePropertyWithIndexedProperty(gpd, (IndexedPropertyDescriptor) spd);
					}
					else if (gpd is IndexedPropertyDescriptor)
					{
						pd = MergePropertyWithIndexedProperty(spd, (IndexedPropertyDescriptor) gpd);
					}
					else
					{
						pd = MergePropertyDescriptor(gpd, spd);
					}
					if (igpd == ispd)
					{
						ipd = igpd;
					}
					else
					{
						ipd = MergePropertyDescriptor(igpd, ispd);
					}
					if (pd == null)
					{
						pd = ipd;
					}
					else
					{
						Class propType = pd.PropertyType;
						Class ipropType = ipd.IndexedPropertyType;
						if (propType.Array && propType.ComponentType == ipropType)
						{
							pd = ipd.Class0.IsSubclassOf(pd.Class0) ? new IndexedPropertyDescriptor(pd, ipd) : new IndexedPropertyDescriptor(ipd, pd);
						}
						else if (ipd.Class0.IsSubclassOf(pd.Class0))
						{
							pd = ipd.Class0.IsSubclassOf(pd.Class0) ? new PropertyDescriptor(pd, ipd) : new PropertyDescriptor(ipd, pd);
						}
						else
						{
							pd = ipd;
						}
					}
				}
				else if (gpd != null && spd != null)
				{
					if (igpd != null)
					{
						gpd = MergePropertyWithIndexedProperty(gpd, igpd);
					}
					if (ispd != null)
					{
						spd = MergePropertyWithIndexedProperty(spd, ispd);
					}
					// Complete simple properties set
					if (gpd == spd)
					{
						pd = gpd;
					}
					else if (spd is IndexedPropertyDescriptor)
					{
						pd = MergePropertyWithIndexedProperty(gpd, (IndexedPropertyDescriptor) spd);
					}
					else if (gpd is IndexedPropertyDescriptor)
					{
						pd = MergePropertyWithIndexedProperty(spd, (IndexedPropertyDescriptor) gpd);
					}
					else
					{
						pd = MergePropertyDescriptor(gpd, spd);
					}
				}
				else if (ispd != null)
				{
					// indexed setter
					pd = ispd;
					// Merge any classic property descriptors
					if (spd != null)
					{
						pd = MergePropertyDescriptor(ispd, spd);
					}
					if (gpd != null)
					{
						pd = MergePropertyDescriptor(ispd, gpd);
					}
				}
				else if (igpd != null)
				{
					// indexed getter
					pd = igpd;
					// Merge any classic property descriptors
					if (gpd != null)
					{
						pd = MergePropertyDescriptor(igpd, gpd);
					}
					if (spd != null)
					{
						pd = MergePropertyDescriptor(igpd, spd);
					}
				}
				else if (spd != null)
				{
					// simple setter
					pd = spd;
				}
				else if (gpd != null)
				{
					// simple getter
					pd = gpd;
				}

				// Very special case to ensure that an IndexedPropertyDescriptor
				// doesn't contain less information than the enclosed
				// PropertyDescriptor. If it does, then recreate as a
				// PropertyDescriptor. See 4168833
				if (pd is IndexedPropertyDescriptor)
				{
					ipd = (IndexedPropertyDescriptor)pd;
					if (ipd.IndexedReadMethod == null && ipd.IndexedWriteMethod == null)
					{
						pd = new PropertyDescriptor(ipd);
					}
				}

				// Find the first property descriptor
				// which does not have getter and setter methods.
				// See regression bug 4984912.
				if ((pd == null) && (list.Count > 0))
				{
					pd = list[0];
				}

				if (pd != null)
				{
					Properties[pd.Name] = pd;
				}
			}
		}

		private static bool IsAssignable(Class current, Class candidate)
		{
			return ((current == null) || (candidate == null)) ? current == candidate : candidate.IsSubclassOf(current);
		}

		private PropertyDescriptor MergePropertyWithIndexedProperty(PropertyDescriptor pd, IndexedPropertyDescriptor ipd)
		{
			Class type = pd.PropertyType;
			if (type.Array && (type.ComponentType == ipd.IndexedPropertyType))
			{
				return ipd.Class0.IsSubclassOf(pd.Class0) ? new IndexedPropertyDescriptor(pd, ipd) : new IndexedPropertyDescriptor(ipd, pd);
			}
			return pd;
		}

		/// <summary>
		/// Adds the property descriptor to the indexedproperty descriptor only if the
		/// types are the same.
		/// 
		/// The most specific property descriptor will take precedence.
		/// </summary>
		private PropertyDescriptor MergePropertyDescriptor(IndexedPropertyDescriptor ipd, PropertyDescriptor pd)
		{
			PropertyDescriptor result = null;

			Class propType = pd.PropertyType;
			Class ipropType = ipd.IndexedPropertyType;

			if (propType.Array && propType.ComponentType == ipropType)
			{
				if (ipd.Class0.IsSubclassOf(pd.Class0))
				{
					result = new IndexedPropertyDescriptor(pd, ipd);
				}
				else
				{
					result = new IndexedPropertyDescriptor(ipd, pd);
				}
			}
			else if ((ipd.ReadMethod == null) && (ipd.WriteMethod == null))
			{
				if (ipd.Class0.IsSubclassOf(pd.Class0))
				{
					result = new PropertyDescriptor(pd, ipd);
				}
				else
				{
					result = new PropertyDescriptor(ipd, pd);
				}
			}
			else
			{
				// Cannot merge the pd because of type mismatch
				// Return the most specific pd
				if (ipd.Class0.IsSubclassOf(pd.Class0))
				{
					result = ipd;
				}
				else
				{
					result = pd;
					// Try to add methods which may have been lost in the type change
					// See 4168833
					Method write = result.WriteMethod;
					Method read = result.ReadMethod;

					if (read == null && write != null)
					{
						read = FindMethod(result.Class0, GET_PREFIX + NameGenerator.Capitalize(result.Name), 0);
						if (read != null)
						{
							try
							{
								result.ReadMethod = read;
							}
							catch (IntrospectionException)
							{
								// no consequences for failure.
							}
						}
					}
					if (write == null && read != null)
					{
						write = FindMethod(result.Class0, SET_PREFIX + NameGenerator.Capitalize(result.Name), 1, new Class[] {FeatureDescriptor.GetReturnType(result.Class0, read)});
						if (write != null)
						{
							try
							{
								result.WriteMethod = write;
							}
							catch (IntrospectionException)
							{
								// no consequences for failure.
							}
						}
					}
				}
			}
			return result;
		}

		// Handle regular pd merge
		private PropertyDescriptor MergePropertyDescriptor(PropertyDescriptor pd1, PropertyDescriptor pd2)
		{
			if (pd2.Class0.IsSubclassOf(pd1.Class0))
			{
				return new PropertyDescriptor(pd1, pd2);
			}
			else
			{
				return new PropertyDescriptor(pd2, pd1);
			}
		}

		// Handle regular ipd merge
		private IndexedPropertyDescriptor MergePropertyDescriptor(IndexedPropertyDescriptor ipd1, IndexedPropertyDescriptor ipd2)
		{
			if (ipd2.Class0.IsSubclassOf(ipd1.Class0))
			{
				return new IndexedPropertyDescriptor(ipd1, ipd2);
			}
			else
			{
				return new IndexedPropertyDescriptor(ipd2, ipd1);
			}
		}

		/// <returns> An array of EventSetDescriptors describing the kinds of
		/// events fired by the target bean. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private EventSetDescriptor[] getTargetEventInfo() throws IntrospectionException
		private EventSetDescriptor[] TargetEventInfo
		{
			get
			{
				if (Events == null)
				{
					Events = new Dictionary<>();
				}
    
				// Check if the bean has its own BeanInfo that will provide
				// explicit information.
				EventSetDescriptor[] explicitEvents = null;
				if (ExplicitBeanInfo != null)
				{
					explicitEvents = ExplicitBeanInfo.EventSetDescriptors;
					int ix = ExplicitBeanInfo.DefaultEventIndex;
					if (ix >= 0 && ix < explicitEvents.Length)
					{
						DefaultEventName = explicitEvents[ix].Name;
					}
				}
    
				if (explicitEvents == null && SuperBeanInfo != null)
				{
					// We have no explicit BeanInfo events.  Check with our parent.
					EventSetDescriptor[] supers = SuperBeanInfo.EventSetDescriptors;
					for (int i = 0 ; i < supers.Length; i++)
					{
						AddEvent(supers[i]);
					}
					int ix = SuperBeanInfo.DefaultEventIndex;
					if (ix >= 0 && ix < supers.Length)
					{
						DefaultEventName = supers[ix].Name;
					}
				}
    
				for (int i = 0; i < AdditionalBeanInfo.Length; i++)
				{
					EventSetDescriptor[] additional = AdditionalBeanInfo[i].EventSetDescriptors;
					if (additional != null)
					{
						for (int j = 0 ; j < additional.Length; j++)
						{
							AddEvent(additional[j]);
						}
					}
				}
    
				if (explicitEvents != null)
				{
					// Add the explicit explicitBeanInfo data to our results.
					for (int i = 0 ; i < explicitEvents.Length; i++)
					{
						AddEvent(explicitEvents[i]);
					}
    
				}
				else
				{
    
					// Apply some reflection to the current class.
    
					// Get an array of all the public beans methods at this level
					Method[] methodList = GetPublicDeclaredMethods(BeanClass);
    
					// Find all suitable "add", "remove" and "get" Listener methods
					// The name of the listener type is the key for these hashtables
					// i.e, ActionListener
					IDictionary<String, Method> adds = null;
					IDictionary<String, Method> removes = null;
					IDictionary<String, Method> gets = null;
    
					for (int i = 0; i < methodList.Length; i++)
					{
						Method method = methodList[i];
						if (method == null)
						{
							continue;
						}
						// skip static methods.
						int mods = method.Modifiers;
						if (Modifier.isStatic(mods))
						{
							continue;
						}
						String name = method.Name;
						// Optimization avoid getParameterTypes
						if (!name.StartsWith(ADD_PREFIX) && !name.StartsWith(REMOVE_PREFIX) && !name.StartsWith(GET_PREFIX))
						{
							continue;
						}
    
						if (name.StartsWith(ADD_PREFIX))
						{
							Class returnType = method.ReturnType;
							if (returnType == typeof(void))
							{
								Type[] parameterTypes = method.GenericParameterTypes;
								if (parameterTypes.Length == 1)
								{
									Class type = TypeResolver.erase(TypeResolver.resolveInClass(BeanClass, parameterTypes[0]));
									if (Introspector.IsSubclass(type, EventListenerType))
									{
										String listenerName = name.Substring(3);
										if (listenerName.Length() > 0 && type.Name.EndsWith(listenerName))
										{
											if (adds == null)
											{
												adds = new Dictionary<>();
											}
											adds[listenerName] = method;
										}
									}
								}
							}
						}
						else if (name.StartsWith(REMOVE_PREFIX))
						{
							Class returnType = method.ReturnType;
							if (returnType == typeof(void))
							{
								Type[] parameterTypes = method.GenericParameterTypes;
								if (parameterTypes.Length == 1)
								{
									Class type = TypeResolver.erase(TypeResolver.resolveInClass(BeanClass, parameterTypes[0]));
									if (Introspector.IsSubclass(type, EventListenerType))
									{
										String listenerName = name.Substring(6);
										if (listenerName.Length() > 0 && type.Name.EndsWith(listenerName))
										{
											if (removes == null)
											{
												removes = new Dictionary<>();
											}
											removes[listenerName] = method;
										}
									}
								}
							}
						}
						else if (name.StartsWith(GET_PREFIX))
						{
							Class[] parameterTypes = method.ParameterTypes;
							if (parameterTypes.Length == 0)
							{
								Class returnType = FeatureDescriptor.GetReturnType(BeanClass, method);
								if (returnType.Array)
								{
									Class type = returnType.ComponentType;
									if (Introspector.IsSubclass(type, EventListenerType))
									{
										String listenerName = name.Substring(3, name.Length() - 1 - 3);
										if (listenerName.Length() > 0 && type.Name.EndsWith(listenerName))
										{
											if (gets == null)
											{
												gets = new Dictionary<>();
											}
											gets[listenerName] = method;
										}
									}
								}
							}
						}
					}
    
					if (adds != null && removes != null)
					{
						// Now look for matching addFooListener+removeFooListener pairs.
						// Bonus if there is a matching getFooListeners method as well.
						IEnumerator<String> keys = adds.Keys.GetEnumerator();
						while (keys.MoveNext())
						{
							String listenerName = keys.Current;
							// Skip any "add" which doesn't have a matching "remove" or
							// a listener name that doesn't end with Listener
							if (removes[listenerName] == null || !listenerName.EndsWith("Listener"))
							{
								continue;
							}
							String eventName = Decapitalize(listenerName.Substring(0, listenerName.Length() - 8));
							Method addMethod = adds[listenerName];
							Method removeMethod = removes[listenerName];
							Method getMethod = null;
							if (gets != null)
							{
								getMethod = gets[listenerName];
							}
							Class argType = FeatureDescriptor.GetParameterTypes(BeanClass, addMethod)[0];
    
							// generate a list of Method objects for each of the target methods:
							Method[] allMethods = GetPublicDeclaredMethods(argType);
							IList<Method> validMethods = new List<Method>(allMethods.Length);
							for (int i = 0; i < allMethods.Length; i++)
							{
								if (allMethods[i] == null)
								{
									continue;
								}
    
								if (IsEventHandler(allMethods[i]))
								{
									validMethods.Add(allMethods[i]);
								}
							}
							Method[] methods = validMethods.ToArray();
    
							EventSetDescriptor esd = new EventSetDescriptor(eventName, argType, methods, addMethod, removeMethod, getMethod);
    
							// If the adder method throws the TooManyListenersException then it
							// is a Unicast event source.
							if (ThrowsException(addMethod, typeof(java.util.TooManyListenersException)))
							{
								esd.Unicast = true;
							}
							AddEvent(esd);
						}
					} // if (adds != null ...
				}
				EventSetDescriptor[] result;
				if (Events.Count == 0)
				{
					result = EMPTY_EVENTSETDESCRIPTORS;
				}
				else
				{
					// Allocate and populate the result array.
					result = new EventSetDescriptor[Events.Count];
					result = Events.Values.toArray(result);
    
					// Set the default index.
					if (DefaultEventName != null)
					{
						for (int i = 0; i < result.Length; i++)
						{
							if (DefaultEventName.Equals(result[i].Name))
							{
								DefaultEventIndex = i;
							}
						}
					}
				}
				return result;
			}
		}

		private void AddEvent(EventSetDescriptor esd)
		{
			String key = esd.Name;
			if (esd.Name.Equals("propertyChange"))
			{
				PropertyChangeSource = true;
			}
			EventSetDescriptor old = Events[key];
			if (old == null)
			{
				Events[key] = esd;
				return;
			}
			EventSetDescriptor composite = new EventSetDescriptor(old, esd);
			Events[key] = composite;
		}

		/// <returns> An array of MethodDescriptors describing the private
		/// methods supported by the target bean. </returns>
		private MethodDescriptor[] TargetMethodInfo
		{
			get
			{
				if (Methods == null)
				{
					Methods = new Dictionary<>(100);
				}
    
				// Check if the bean has its own BeanInfo that will provide
				// explicit information.
				MethodDescriptor[] explicitMethods = null;
				if (ExplicitBeanInfo != null)
				{
					explicitMethods = ExplicitBeanInfo.MethodDescriptors;
				}
    
				if (explicitMethods == null && SuperBeanInfo != null)
				{
					// We have no explicit BeanInfo methods.  Check with our parent.
					MethodDescriptor[] supers = SuperBeanInfo.MethodDescriptors;
					for (int i = 0 ; i < supers.Length; i++)
					{
						AddMethod(supers[i]);
					}
				}
    
				for (int i = 0; i < AdditionalBeanInfo.Length; i++)
				{
					MethodDescriptor[] additional = AdditionalBeanInfo[i].MethodDescriptors;
					if (additional != null)
					{
						for (int j = 0 ; j < additional.Length; j++)
						{
							AddMethod(additional[j]);
						}
					}
				}
    
				if (explicitMethods != null)
				{
					// Add the explicit explicitBeanInfo data to our results.
					for (int i = 0 ; i < explicitMethods.Length; i++)
					{
						AddMethod(explicitMethods[i]);
					}
    
				}
				else
				{
    
					// Apply some reflection to the current class.
    
					// First get an array of all the beans methods at this level
					Method[] methodList = GetPublicDeclaredMethods(BeanClass);
    
					// Now analyze each method.
					for (int i = 0; i < methodList.Length; i++)
					{
						Method method = methodList[i];
						if (method == null)
						{
							continue;
						}
						MethodDescriptor md = new MethodDescriptor(method);
						AddMethod(md);
					}
				}
    
				// Allocate and populate the result array.
				MethodDescriptor[] result = new MethodDescriptor[Methods.Count];
				result = Methods.Values.toArray(result);
    
				return result;
			}
		}

		private void AddMethod(MethodDescriptor md)
		{
			// We have to be careful here to distinguish method by both name
			// and argument lists.
			// This method gets called a *lot, so we try to be efficient.
			String name = md.Name;

			MethodDescriptor old = Methods[name];
			if (old == null)
			{
				// This is the common case.
				Methods[name] = md;
				return;
			}

			// We have a collision on method names.  This is rare.

			// Check if old and md have the same type.
			String[] p1 = md.ParamNames;
			String[] p2 = old.ParamNames;

			bool match = false;
			if (p1.Length == p2.Length)
			{
				match = true;
				for (int i = 0; i < p1.Length; i++)
				{
					if (p1[i] != p2[i])
					{
						match = false;
						break;
					}
				}
			}
			if (match)
			{
				MethodDescriptor composite = new MethodDescriptor(old, md);
				Methods[name] = composite;
				return;
			}

			// We have a collision on method names with different type signatures.
			// This is very rare.

			String longKey = MakeQualifiedMethodName(name, p1);
			old = Methods[longKey];
			if (old == null)
			{
				Methods[longKey] = md;
				return;
			}
			MethodDescriptor composite = new MethodDescriptor(old, md);
			Methods[longKey] = composite;
		}

		/// <summary>
		/// Creates a key for a method in a method cache.
		/// </summary>
		private static String MakeQualifiedMethodName(String name, String[] @params)
		{
			StringBuffer sb = new StringBuffer(name);
			sb.Append('=');
			for (int i = 0; i < @params.Length; i++)
			{
				sb.Append(':');
				sb.Append(@params[i]);
			}
			return sb.ToString();
		}

		private int TargetDefaultEventIndex
		{
			get
			{
				return DefaultEventIndex;
			}
		}

		private int TargetDefaultPropertyIndex
		{
			get
			{
				return DefaultPropertyIndex;
			}
		}

		private BeanDescriptor TargetBeanDescriptor
		{
			get
			{
				// Use explicit info, if available,
				if (ExplicitBeanInfo != null)
				{
					BeanDescriptor bd = ExplicitBeanInfo.BeanDescriptor;
					if (bd != null)
					{
						return (bd);
					}
				}
				// OK, fabricate a default BeanDescriptor.
				return new BeanDescriptor(this.BeanClass, FindCustomizerClass(this.BeanClass));
			}
		}

		private static Class FindCustomizerClass(Class type)
		{
			String name = type.Name + "Customizer";
			try
			{
				type = ClassFinder.findClass(name, type.ClassLoader);
				// Each customizer should inherit java.awt.Component and implement java.beans.Customizer
				// according to the section 9.3 of JavaBeans&trade; specification
				if (type.IsSubclassOf(typeof(Component)) && type.IsSubclassOf(typeof(Customizer)))
				{
					return type;
				}
			}
			catch (Exception)
			{
				// ignore any exceptions
			}
			return null;
		}

		private bool IsEventHandler(Method m)
		{
			// We assume that a method is an event handler if it has a single
			// argument, whose type inherit from java.util.Event.
			Type[] argTypes = m.GenericParameterTypes;
			if (argTypes.Length != 1)
			{
				return false;
			}
			return IsSubclass(TypeResolver.erase(TypeResolver.resolveInClass(BeanClass, argTypes[0])), typeof(EventObject));
		}

		/*
		 * Internal method to return *public* methods within a class.
		 */
		private static Method[] GetPublicDeclaredMethods(Class clz)
		{
			// Looking up Class.getDeclaredMethods is relatively expensive,
			// so we cache the results.
			if (!ReflectUtil.isPackageAccessible(clz))
			{
				return new Method[0];
			}
			lock (DeclaredMethodCache)
			{
				Method[] result = DeclaredMethodCache.get(clz);
				if (result == null)
				{
					result = clz.Methods;
					for (int i = 0; i < result.Length; i++)
					{
						Method method = result[i];
						if (!method.DeclaringClass.Equals(clz))
						{
							result[i] = null; // ignore methods declared elsewhere
						}
						else
						{
							try
							{
								method = MethodFinder.findAccessibleMethod(method);
								Class type = method.DeclaringClass;
								result[i] = type.Equals(clz) || type.Interface ? method : null; // ignore methods from superclasses
							}
							catch (NoSuchMethodException)
							{
								// commented out because of 6976577
								// result[i] = null; // ignore inaccessible methods
							}
						}
					}
					DeclaredMethodCache.put(clz, result);
				}
				return result;
			}
		}

		//======================================================================
		// Package private support methods.
		//======================================================================

		/// <summary>
		/// Internal support for finding a target methodName with a given
		/// parameter list on a given class.
		/// </summary>
		private static Method InternalFindMethod(Class start, String methodName, int argCount, Class[] args)
		{
			// For overriden methods we need to find the most derived version.
			// So we start with the given class and walk up the superclass chain.

			Method method = null;

			for (Class cl = start; cl != null; cl = cl.BaseType)
			{
				Method[] methods = GetPublicDeclaredMethods(cl);
				for (int i = 0; i < methods.Length; i++)
				{
					method = methods[i];
					if (method == null)
					{
						continue;
					}

					// make sure method signature matches.
					if (method.Name.Equals(methodName))
					{
						Type[] @params = method.GenericParameterTypes;
						if (@params.Length == argCount)
						{
							if (args != null)
							{
								bool different = false;
								if (argCount > 0)
								{
									for (int j = 0; j < argCount; j++)
									{
										if (TypeResolver.erase(TypeResolver.resolveInClass(start, @params[j])) != args[j])
										{
											different = true;
											continue;
										}
									}
									if (different)
									{
										continue;
									}
								}
							}
							return method;
						}
					}
				}
			}
			method = null;

			// Now check any inherited interfaces.  This is necessary both when
			// the argument class is itself an interface, and when the argument
			// class is an abstract class.
			Class[] ifcs = start.Interfaces;
			for (int i = 0 ; i < ifcs.Length; i++)
			{
				// Note: The original implementation had both methods calling
				// the 3 arg method. This is preserved but perhaps it should
				// pass the args array instead of null.
				method = InternalFindMethod(ifcs[i], methodName, argCount, null);
				if (method != null)
				{
					break;
				}
			}
			return method;
		}

		/// <summary>
		/// Find a target methodName on a given class.
		/// </summary>
		internal static Method FindMethod(Class cls, String methodName, int argCount)
		{
			return FindMethod(cls, methodName, argCount, null);
		}

		/// <summary>
		/// Find a target methodName with specific parameter list on a given class.
		/// <para>
		/// Used in the contructors of the EventSetDescriptor,
		/// PropertyDescriptor and the IndexedPropertyDescriptor.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="cls"> The Class object on which to retrieve the method. </param>
		/// <param name="methodName"> Name of the method. </param>
		/// <param name="argCount"> Number of arguments for the desired method. </param>
		/// <param name="args"> Array of argument types for the method. </param>
		/// <returns> the method or null if not found </returns>
		internal static Method FindMethod(Class cls, String methodName, int argCount, Class[] args)
		{
			if (methodName == null)
			{
				return null;
			}
			return InternalFindMethod(cls, methodName, argCount, args);
		}

		/// <summary>
		/// Return true if class a is either equivalent to class b, or
		/// if class a is a subclass of class b, i.e. if a either "extends"
		/// or "implements" b.
		/// Note tht either or both "Class" objects may represent interfaces.
		/// </summary>
		internal static bool IsSubclass(Class a, Class b)
		{
			// We rely on the fact that for any given java class or
			// primtitive type there is a unqiue Class object, so
			// we can use object equivalence in the comparisons.
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			for (Class x = a; x != null; x = x.BaseType)
			{
				if (x == b)
				{
					return true;
				}
				if (b.Interface)
				{
					Class[] interfaces = x.Interfaces;
					for (int i = 0; i < interfaces.Length; i++)
					{
						if (IsSubclass(interfaces[i], b))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Return true iff the given method throws the given exception.
		/// </summary>
		private bool ThrowsException(Method method, Class exception)
		{
			Class[] exs = method.ExceptionTypes;
			for (int i = 0; i < exs.Length; i++)
			{
				if (exs[i] == exception)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Try to create an instance of a named class.
		/// First try the classloader of "sibling", then try the system
		/// classloader then the class loader of the current Thread.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object instantiate(Class sibling, String className) throws InstantiationException, IllegalAccessException, ClassNotFoundException
		internal static Object Instantiate(Class sibling, String className)
		{
			// First check with sibling's classloader (if any).
			ClassLoader cl = sibling.ClassLoader;
			Class cls = ClassFinder.findClass(className, cl);
			return cls.NewInstance();
		}

	} // end class Introspector

	//===========================================================================

	/// <summary>
	/// Package private implementation support class for Introspector's
	/// internal use.
	/// <para>
	/// Mostly this is used as a placeholder for the descriptors.
	/// </para>
	/// </summary>

	internal class GenericBeanInfo : SimpleBeanInfo
	{

		private BeanDescriptor BeanDescriptor_Renamed;
		private EventSetDescriptor[] Events;
		private int DefaultEvent;
		private PropertyDescriptor[] Properties;
		private int DefaultProperty;
		private MethodDescriptor[] Methods;
		private Reference<BeanInfo> TargetBeanInfoRef;

		public GenericBeanInfo(BeanDescriptor beanDescriptor, EventSetDescriptor[] events, int defaultEvent, PropertyDescriptor[] properties, int defaultProperty, MethodDescriptor[] methods, BeanInfo targetBeanInfo)
		{
			this.BeanDescriptor_Renamed = beanDescriptor;
			this.Events = events;
			this.DefaultEvent = defaultEvent;
			this.Properties = properties;
			this.DefaultProperty = defaultProperty;
			this.Methods = methods;
			this.TargetBeanInfoRef = (targetBeanInfo != null) ? new SoftReference<>(targetBeanInfo) : null;
		}

		/// <summary>
		/// Package-private dup constructor
		/// This must isolate the new object from any changes to the old object.
		/// </summary>
		internal GenericBeanInfo(GenericBeanInfo old)
		{

			BeanDescriptor_Renamed = new BeanDescriptor(old.BeanDescriptor_Renamed);
			if (old.Events != null)
			{
				int len = old.Events.Length;
				Events = new EventSetDescriptor[len];
				for (int i = 0; i < len; i++)
				{
					Events[i] = new EventSetDescriptor(old.Events[i]);
				}
			}
			DefaultEvent = old.DefaultEvent;
			if (old.Properties != null)
			{
				int len = old.Properties.Length;
				Properties = new PropertyDescriptor[len];
				for (int i = 0; i < len; i++)
				{
					PropertyDescriptor oldp = old.Properties[i];
					if (oldp is IndexedPropertyDescriptor)
					{
						Properties[i] = new IndexedPropertyDescriptor((IndexedPropertyDescriptor) oldp);
					}
					else
					{
						Properties[i] = new PropertyDescriptor(oldp);
					}
				}
			}
			DefaultProperty = old.DefaultProperty;
			if (old.Methods != null)
			{
				int len = old.Methods.Length;
				Methods = new MethodDescriptor[len];
				for (int i = 0; i < len; i++)
				{
					Methods[i] = new MethodDescriptor(old.Methods[i]);
				}
			}
			this.TargetBeanInfoRef = old.TargetBeanInfoRef;
		}

		public override PropertyDescriptor[] PropertyDescriptors
		{
			get
			{
				return Properties;
			}
		}

		public override int DefaultPropertyIndex
		{
			get
			{
				return DefaultProperty;
			}
		}

		public override EventSetDescriptor[] EventSetDescriptors
		{
			get
			{
				return Events;
			}
		}

		public override int DefaultEventIndex
		{
			get
			{
				return DefaultEvent;
			}
		}

		public override MethodDescriptor[] MethodDescriptors
		{
			get
			{
				return Methods;
			}
		}

		public override BeanDescriptor BeanDescriptor
		{
			get
			{
				return BeanDescriptor_Renamed;
			}
		}

		public override java.awt.Image GetIcon(int iconKind)
		{
			BeanInfo targetBeanInfo = TargetBeanInfo;
			if (targetBeanInfo != null)
			{
				return targetBeanInfo.GetIcon(iconKind);
			}
			return base.GetIcon(iconKind);
		}

		private BeanInfo TargetBeanInfo
		{
			get
			{
				if (this.TargetBeanInfoRef == null)
				{
					return null;
				}
				BeanInfo targetBeanInfo = this.TargetBeanInfoRef.get();
				if (targetBeanInfo == null)
				{
					targetBeanInfo = ThreadGroupContext.Context.BeanInfoFinder.find(this.BeanDescriptor_Renamed.BeanClass);
					if (targetBeanInfo != null)
					{
						this.TargetBeanInfoRef = new SoftReference<>(targetBeanInfo);
					}
				}
				return targetBeanInfo;
			}
		}
	}

}