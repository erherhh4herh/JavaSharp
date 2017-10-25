/*
 * Copyright (c) 1996, 2012, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// An EventSetDescriptor describes a group of events that a given Java
	/// bean fires.
	/// <P>
	/// The given group of events are all delivered as method calls on a single
	/// event listener interface, and an event listener object can be registered
	/// via a call on a registration method supplied by the event source.
	/// </summary>
	public class EventSetDescriptor : FeatureDescriptor
	{

		private MethodDescriptor[] ListenerMethodDescriptors_Renamed;
		private MethodDescriptor AddMethodDescriptor;
		private MethodDescriptor RemoveMethodDescriptor;
		private MethodDescriptor GetMethodDescriptor;

		private Reference<Method[]> ListenerMethodsRef;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Reference<? extends Class> listenerTypeRef;
		private Reference<?> ListenerTypeRef;

		private bool Unicast_Renamed;
		private bool InDefaultEventSet_Renamed = true;

		/// <summary>
		/// Creates an <TT>EventSetDescriptor</TT> assuming that you are
		/// following the most simple standard design pattern where a named
		/// event &quot;fred&quot; is (1) delivered as a call on the single method of
		/// interface FredListener, (2) has a single argument of type FredEvent,
		/// and (3) where the FredListener may be registered with a call on an
		/// addFredListener method of the source component and removed with a
		/// call on a removeFredListener method.
		/// </summary>
		/// <param name="sourceClass">  The class firing the event. </param>
		/// <param name="eventSetName">  The programmatic name of the event.  E.g. &quot;fred&quot;.
		///          Note that this should normally start with a lower-case character. </param>
		/// <param name="listenerType">  The target interface that events
		///          will get delivered to. </param>
		/// <param name="listenerMethodName">  The method that will get called when the event gets
		///          delivered to its target listener interface. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EventSetDescriptor(Class sourceClass, String eventSetName, Class listenerType, String listenerMethodName) throws IntrospectionException
		public EventSetDescriptor(Class sourceClass, String eventSetName, Class listenerType, String listenerMethodName) : this(sourceClass, eventSetName, listenerType, new String[] {listenerMethodName}, Introspector.ADD_PREFIX + GetListenerClassName(listenerType), Introspector.REMOVE_PREFIX + GetListenerClassName(listenerType), Introspector.GET_PREFIX + GetListenerClassName(listenerType) + "s")
		{

			String eventName = NameGenerator.Capitalize(eventSetName) + "Event";
			Method[] listenerMethods = ListenerMethods;
			if (listenerMethods.Length > 0)
			{
				Class[] args = GetParameterTypes(Class0, listenerMethods[0]);
				// Check for EventSet compliance. Special case for vetoableChange. See 4529996
				if (!"vetoableChange".Equals(eventSetName) && !args[0].Name.EndsWith(eventName))
				{
					throw new IntrospectionException("Method \"" + listenerMethodName + "\" should have argument \"" + eventName + "\"");
				}
			}
		}

		private static String GetListenerClassName(Class cls)
		{
			String className = cls.Name;
			return className.Substring(className.LastIndexOf('.') + 1);
		}

		/// <summary>
		/// Creates an <TT>EventSetDescriptor</TT> from scratch using
		/// string names.
		/// </summary>
		/// <param name="sourceClass">  The class firing the event. </param>
		/// <param name="eventSetName"> The programmatic name of the event set.
		///          Note that this should normally start with a lower-case character. </param>
		/// <param name="listenerType">  The Class of the target interface that events
		///          will get delivered to. </param>
		/// <param name="listenerMethodNames"> The names of the methods that will get called
		///          when the event gets delivered to its target listener interface. </param>
		/// <param name="addListenerMethodName">  The name of the method on the event source
		///          that can be used to register an event listener object. </param>
		/// <param name="removeListenerMethodName">  The name of the method on the event source
		///          that can be used to de-register an event listener object. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EventSetDescriptor(Class sourceClass, String eventSetName, Class listenerType, String listenerMethodNames[] , String addListenerMethodName, String removeListenerMethodName) throws IntrospectionException
		public EventSetDescriptor(Class sourceClass, String eventSetName, Class listenerType, String[] listenerMethodNames, String addListenerMethodName, String removeListenerMethodName) : this(sourceClass, eventSetName, listenerType, listenerMethodNames, addListenerMethodName, removeListenerMethodName, null)
		{
		}

		/// <summary>
		/// This constructor creates an EventSetDescriptor from scratch using
		/// string names.
		/// </summary>
		/// <param name="sourceClass">  The class firing the event. </param>
		/// <param name="eventSetName"> The programmatic name of the event set.
		///          Note that this should normally start with a lower-case character. </param>
		/// <param name="listenerType">  The Class of the target interface that events
		///          will get delivered to. </param>
		/// <param name="listenerMethodNames"> The names of the methods that will get called
		///          when the event gets delivered to its target listener interface. </param>
		/// <param name="addListenerMethodName">  The name of the method on the event source
		///          that can be used to register an event listener object. </param>
		/// <param name="removeListenerMethodName">  The name of the method on the event source
		///          that can be used to de-register an event listener object. </param>
		/// <param name="getListenerMethodName"> The method on the event source that
		///          can be used to access the array of event listener objects. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EventSetDescriptor(Class sourceClass, String eventSetName, Class listenerType, String listenerMethodNames[] , String addListenerMethodName, String removeListenerMethodName, String getListenerMethodName) throws IntrospectionException
		public EventSetDescriptor(Class sourceClass, String eventSetName, Class listenerType, String[] listenerMethodNames, String addListenerMethodName, String removeListenerMethodName, String getListenerMethodName)
		{
			if (sourceClass == null || eventSetName == null || listenerType == null)
			{
				throw new NullPointerException();
			}
			Name = eventSetName;
			Class0 = sourceClass;
			ListenerType = listenerType;

			Method[] listenerMethods = new Method[listenerMethodNames.Length];
			for (int i = 0; i < listenerMethodNames.Length; i++)
			{
				// Check for null names
				if (listenerMethodNames[i] == null)
				{
					throw new NullPointerException();
				}
				listenerMethods[i] = GetMethod(listenerType, listenerMethodNames[i], 1);
			}
			ListenerMethods = listenerMethods;

			AddListenerMethod = GetMethod(sourceClass, addListenerMethodName, 1);
			RemoveListenerMethod = GetMethod(sourceClass, removeListenerMethodName, 1);

			// Be more forgiving of not finding the getListener method.
			Method method = Introspector.FindMethod(sourceClass, getListenerMethodName, 0);
			if (method != null)
			{
				GetListenerMethod = method;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Method getMethod(Class cls, String name, int args) throws IntrospectionException
		private static Method GetMethod(Class cls, String name, int args)
		{
			if (name == null)
			{
				return null;
			}
			Method method = Introspector.FindMethod(cls, name, args);
			if ((method == null) || Modifier.isStatic(method.Modifiers))
			{
				throw new IntrospectionException("Method not found: " + name + " on class " + cls.Name);
			}
			return method;
		}

		/// <summary>
		/// Creates an <TT>EventSetDescriptor</TT> from scratch using
		/// <TT>java.lang.reflect.Method</TT> and <TT>java.lang.Class</TT> objects.
		/// </summary>
		/// <param name="eventSetName"> The programmatic name of the event set. </param>
		/// <param name="listenerType"> The Class for the listener interface. </param>
		/// <param name="listenerMethods">  An array of Method objects describing each
		///          of the event handling methods in the target listener. </param>
		/// <param name="addListenerMethod">  The method on the event source
		///          that can be used to register an event listener object. </param>
		/// <param name="removeListenerMethod">  The method on the event source
		///          that can be used to de-register an event listener object. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EventSetDescriptor(String eventSetName, Class listenerType, Method listenerMethods[] , Method addListenerMethod, Method removeListenerMethod) throws IntrospectionException
		public EventSetDescriptor(String eventSetName, Class listenerType, Method[] listenerMethods, Method addListenerMethod, Method removeListenerMethod) : this(eventSetName, listenerType, listenerMethods, addListenerMethod, removeListenerMethod, null)
		{
		}

		/// <summary>
		/// This constructor creates an EventSetDescriptor from scratch using
		/// java.lang.reflect.Method and java.lang.Class objects.
		/// </summary>
		/// <param name="eventSetName"> The programmatic name of the event set. </param>
		/// <param name="listenerType"> The Class for the listener interface. </param>
		/// <param name="listenerMethods">  An array of Method objects describing each
		///          of the event handling methods in the target listener. </param>
		/// <param name="addListenerMethod">  The method on the event source
		///          that can be used to register an event listener object. </param>
		/// <param name="removeListenerMethod">  The method on the event source
		///          that can be used to de-register an event listener object. </param>
		/// <param name="getListenerMethod"> The method on the event source
		///          that can be used to access the array of event listener objects. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection.
		/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EventSetDescriptor(String eventSetName, Class listenerType, Method listenerMethods[] , Method addListenerMethod, Method removeListenerMethod, Method getListenerMethod) throws IntrospectionException
		public EventSetDescriptor(String eventSetName, Class listenerType, Method[] listenerMethods, Method addListenerMethod, Method removeListenerMethod, Method getListenerMethod)
		{
			Name = eventSetName;
			ListenerMethods = listenerMethods;
			AddListenerMethod = addListenerMethod;
			RemoveListenerMethod = removeListenerMethod;
			GetListenerMethod = getListenerMethod;
			ListenerType = listenerType;
		}

		/// <summary>
		/// Creates an <TT>EventSetDescriptor</TT> from scratch using
		/// <TT>java.lang.reflect.MethodDescriptor</TT> and <TT>java.lang.Class</TT>
		///  objects.
		/// </summary>
		/// <param name="eventSetName"> The programmatic name of the event set. </param>
		/// <param name="listenerType"> The Class for the listener interface. </param>
		/// <param name="listenerMethodDescriptors">  An array of MethodDescriptor objects
		///           describing each of the event handling methods in the
		///           target listener. </param>
		/// <param name="addListenerMethod">  The method on the event source
		///          that can be used to register an event listener object. </param>
		/// <param name="removeListenerMethod">  The method on the event source
		///          that can be used to de-register an event listener object. </param>
		/// <exception cref="IntrospectionException"> if an exception occurs during
		///              introspection. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EventSetDescriptor(String eventSetName, Class listenerType, MethodDescriptor listenerMethodDescriptors[] , Method addListenerMethod, Method removeListenerMethod) throws IntrospectionException
		public EventSetDescriptor(String eventSetName, Class listenerType, MethodDescriptor[] listenerMethodDescriptors, Method addListenerMethod, Method removeListenerMethod)
		{
			Name = eventSetName;
			this.ListenerMethodDescriptors_Renamed = (listenerMethodDescriptors != null) ? listenerMethodDescriptors.clone() : null;
			AddListenerMethod = addListenerMethod;
			RemoveListenerMethod = removeListenerMethod;
			ListenerType = listenerType;
		}

		/// <summary>
		/// Gets the <TT>Class</TT> object for the target interface.
		/// </summary>
		/// <returns> The Class object for the target interface that will
		/// get invoked when the event is fired. </returns>
		public virtual Class ListenerType
		{
			get
			{
				return (this.ListenerTypeRef != null) ? this.ListenerTypeRef.get() : null;
			}
			set
			{
				this.ListenerTypeRef = GetWeakReference(value);
			}
		}


		/// <summary>
		/// Gets the methods of the target listener interface.
		/// </summary>
		/// <returns> An array of <TT>Method</TT> objects for the target methods
		/// within the target listener interface that will get called when
		/// events are fired. </returns>
		public virtual Method[] ListenerMethods
		{
			get
			{
				lock (this)
				{
					Method[] methods = ListenerMethods0;
					if (methods == null)
					{
						if (ListenerMethodDescriptors_Renamed != null)
						{
							methods = new Method[ListenerMethodDescriptors_Renamed.Length];
							for (int i = 0; i < methods.Length; i++)
							{
								methods[i] = ListenerMethodDescriptors_Renamed[i].Method;
							}
						}
						ListenerMethods = methods;
					}
					return methods;
				}
			}
			set
			{
				if (value == null)
				{
					return;
				}
				if (ListenerMethodDescriptors_Renamed == null)
				{
					ListenerMethodDescriptors_Renamed = new MethodDescriptor[value.Length];
					for (int i = 0; i < value.Length; i++)
					{
						ListenerMethodDescriptors_Renamed[i] = new MethodDescriptor(value[i]);
					}
				}
				this.ListenerMethodsRef = GetSoftReference(value);
			}
		}


		private Method[] ListenerMethods0
		{
			get
			{
				return (this.ListenerMethodsRef != null) ? this.ListenerMethodsRef.get() : null;
			}
		}

		/// <summary>
		/// Gets the <code>MethodDescriptor</code>s of the target listener interface.
		/// </summary>
		/// <returns> An array of <code>MethodDescriptor</code> objects for the target methods
		/// within the target listener interface that will get called when
		/// events are fired. </returns>
		public virtual MethodDescriptor[] ListenerMethodDescriptors
		{
			get
			{
				lock (this)
				{
					return (this.ListenerMethodDescriptors_Renamed != null) ? this.ListenerMethodDescriptors_Renamed.clone() : null;
				}
			}
		}

		/// <summary>
		/// Gets the method used to add event listeners.
		/// </summary>
		/// <returns> The method used to register a listener at the event source. </returns>
		public virtual Method AddListenerMethod
		{
			get
			{
				lock (this)
				{
					return GetMethod(this.AddMethodDescriptor);
				}
			}
			set
			{
				lock (this)
				{
					if (value == null)
					{
						return;
					}
					if (Class0 == null)
					{
						Class0 = value.DeclaringClass;
					}
					AddMethodDescriptor = new MethodDescriptor(value);
					SetTransient(value.getAnnotation(typeof(Transient)));
				}
			}
		}


		/// <summary>
		/// Gets the method used to remove event listeners.
		/// </summary>
		/// <returns> The method used to remove a listener at the event source. </returns>
		public virtual Method RemoveListenerMethod
		{
			get
			{
				lock (this)
				{
					return GetMethod(this.RemoveMethodDescriptor);
				}
			}
			set
			{
				lock (this)
				{
					if (value == null)
					{
						return;
					}
					if (Class0 == null)
					{
						Class0 = value.DeclaringClass;
					}
					RemoveMethodDescriptor = new MethodDescriptor(value);
					SetTransient(value.getAnnotation(typeof(Transient)));
				}
			}
		}


		/// <summary>
		/// Gets the method used to access the registered event listeners.
		/// </summary>
		/// <returns> The method used to access the array of listeners at the event
		///         source or null if it doesn't exist.
		/// @since 1.4 </returns>
		public virtual Method GetListenerMethod
		{
			get
			{
				lock (this)
				{
					return GetMethod(this.GetMethodDescriptor);
				}
			}
			set
			{
				lock (this)
				{
					if (value == null)
					{
						return;
					}
					if (Class0 == null)
					{
						Class0 = value.DeclaringClass;
					}
					GetMethodDescriptor = new MethodDescriptor(value);
					SetTransient(value.getAnnotation(typeof(Transient)));
				}
			}
		}


		/// <summary>
		/// Mark an event set as unicast (or not).
		/// </summary>
		/// <param name="unicast">  True if the event set is unicast. </param>
		public virtual bool Unicast
		{
			set
			{
				this.Unicast_Renamed = value;
			}
			get
			{
				return Unicast_Renamed;
			}
		}


		/// <summary>
		/// Marks an event set as being in the &quot;default&quot; set (or not).
		/// By default this is <TT>true</TT>.
		/// </summary>
		/// <param name="inDefaultEventSet"> <code>true</code> if the event set is in
		///                          the &quot;default&quot; set,
		///                          <code>false</code> if not </param>
		public virtual bool InDefaultEventSet
		{
			set
			{
				this.InDefaultEventSet_Renamed = value;
			}
			get
			{
				return InDefaultEventSet_Renamed;
			}
		}


		/*
		 * Package-private constructor
		 * Merge two event set descriptors.  Where they conflict, give the
		 * second argument (y) priority over the first argument (x).
		 *
		 * @param x  The first (lower priority) EventSetDescriptor
		 * @param y  The second (higher priority) EventSetDescriptor
		 */
		internal EventSetDescriptor(EventSetDescriptor x, EventSetDescriptor y) : base(x,y)
		{
			ListenerMethodDescriptors_Renamed = x.ListenerMethodDescriptors_Renamed;
			if (y.ListenerMethodDescriptors_Renamed != null)
			{
				ListenerMethodDescriptors_Renamed = y.ListenerMethodDescriptors_Renamed;
			}

			ListenerTypeRef = x.ListenerTypeRef;
			if (y.ListenerTypeRef != null)
			{
				ListenerTypeRef = y.ListenerTypeRef;
			}

			AddMethodDescriptor = x.AddMethodDescriptor;
			if (y.AddMethodDescriptor != null)
			{
				AddMethodDescriptor = y.AddMethodDescriptor;
			}

			RemoveMethodDescriptor = x.RemoveMethodDescriptor;
			if (y.RemoveMethodDescriptor != null)
			{
				RemoveMethodDescriptor = y.RemoveMethodDescriptor;
			}

			GetMethodDescriptor = x.GetMethodDescriptor;
			if (y.GetMethodDescriptor != null)
			{
				GetMethodDescriptor = y.GetMethodDescriptor;
			}

			Unicast_Renamed = y.Unicast_Renamed;
			if (!x.InDefaultEventSet_Renamed || !y.InDefaultEventSet_Renamed)
			{
				InDefaultEventSet_Renamed = false;
			}
		}

		/*
		 * Package-private dup constructor
		 * This must isolate the new object from any changes to the old object.
		 */
		internal EventSetDescriptor(EventSetDescriptor old) : base(old)
		{
			if (old.ListenerMethodDescriptors_Renamed != null)
			{
				int len = old.ListenerMethodDescriptors_Renamed.Length;
				ListenerMethodDescriptors_Renamed = new MethodDescriptor[len];
				for (int i = 0; i < len; i++)
				{
					ListenerMethodDescriptors_Renamed[i] = new MethodDescriptor(old.ListenerMethodDescriptors_Renamed[i]);
				}
			}
			ListenerTypeRef = old.ListenerTypeRef;

			AddMethodDescriptor = old.AddMethodDescriptor;
			RemoveMethodDescriptor = old.RemoveMethodDescriptor;
			GetMethodDescriptor = old.GetMethodDescriptor;

			Unicast_Renamed = old.Unicast_Renamed;
			InDefaultEventSet_Renamed = old.InDefaultEventSet_Renamed;
		}

		internal override void AppendTo(StringBuilder sb)
		{
			AppendTo(sb, "unicast", this.Unicast_Renamed);
			AppendTo(sb, "inDefaultEventSet", this.InDefaultEventSet_Renamed);
			AppendTo(sb, "listenerType", this.ListenerTypeRef);
			AppendTo(sb, "getListenerMethod", GetMethod(this.GetMethodDescriptor));
			AppendTo(sb, "addListenerMethod", GetMethod(this.AddMethodDescriptor));
			AppendTo(sb, "removeListenerMethod", GetMethod(this.RemoveMethodDescriptor));
		}

		private static Method GetMethod(MethodDescriptor descriptor)
		{
			return (descriptor != null) ? descriptor.Method : null;
		}
	}

}