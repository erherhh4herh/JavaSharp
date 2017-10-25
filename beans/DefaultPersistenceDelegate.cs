using System;

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
namespace java.beans
{

	using sun.reflect.misc;


	/// <summary>
	/// The <code>DefaultPersistenceDelegate</code> is a concrete implementation of
	/// the abstract <code>PersistenceDelegate</code> class and
	/// is the delegate used by default for classes about
	/// which no information is available. The <code>DefaultPersistenceDelegate</code>
	/// provides, version resilient, public API-based persistence for
	/// classes that follow the JavaBeans&trade; conventions without any class specific
	/// configuration.
	/// <para>
	/// The key assumptions are that the class has a nullary constructor
	/// and that its state is accurately represented by matching pairs
	/// of "setter" and "getter" methods in the order they are returned
	/// by the Introspector.
	/// In addition to providing code-free persistence for JavaBeans,
	/// the <code>DefaultPersistenceDelegate</code> provides a convenient means
	/// to effect persistent storage for classes that have a constructor
	/// that, while not nullary, simply requires some property values
	/// as arguments.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= #DefaultPersistenceDelegate(String[]) </seealso>
	/// <seealso cref= java.beans.Introspector
	/// 
	/// @since 1.4
	/// 
	/// @author Philip Milne </seealso>

	public class DefaultPersistenceDelegate : PersistenceDelegate
	{
		private static readonly String[] EMPTY = new String[] {};
		private readonly String[] Constructor;
		private Boolean DefinesEquals_Renamed;

		/// <summary>
		/// Creates a persistence delegate for a class with a nullary constructor.
		/// </summary>
		/// <seealso cref= #DefaultPersistenceDelegate(java.lang.String[]) </seealso>
		public DefaultPersistenceDelegate()
		{
			this.Constructor = EMPTY;
		}

		/// <summary>
		/// Creates a default persistence delegate for a class with a
		/// constructor whose arguments are the values of the property
		/// names as specified by <code>constructorPropertyNames</code>.
		/// The constructor arguments are created by
		/// evaluating the property names in the order they are supplied.
		/// To use this class to specify a single preferred constructor for use
		/// in the serialization of a particular type, we state the
		/// names of the properties that make up the constructor's
		/// arguments. For example, the <code>Font</code> class which
		/// does not define a nullary constructor can be handled
		/// with the following persistence delegate:
		/// 
		/// <pre>
		///     new DefaultPersistenceDelegate(new String[]{"name", "style", "size"});
		/// </pre>
		/// </summary>
		/// <param name="constructorPropertyNames"> The property names for the arguments of this constructor.
		/// </param>
		/// <seealso cref= #instantiate </seealso>
		public DefaultPersistenceDelegate(String[] constructorPropertyNames)
		{
			this.Constructor = (constructorPropertyNames == null) ? EMPTY : constructorPropertyNames.clone();
		}

		private static bool DefinesEquals(Class type)
		{
			try
			{
				return type == type.GetMethod("equals", typeof(Object)).DeclaringClass;
			}
			catch (NoSuchMethodException)
			{
				return false;
			}
		}

		private bool DefinesEquals(Object instance)
		{
			if (DefinesEquals_Renamed != null)
			{
				return (DefinesEquals_Renamed == true);
			}
			else
			{
				bool result = DefinesEquals(instance.GetType());
				DefinesEquals_Renamed = result ? true : false;
				return result;
			}
		}

		/// <summary>
		/// If the number of arguments in the specified constructor is non-zero and
		/// the class of <code>oldInstance</code> explicitly declares an "equals" method
		/// this method returns the value of <code>oldInstance.equals(newInstance)</code>.
		/// Otherwise, this method uses the superclass's definition which returns true if the
		/// classes of the two instances are equal.
		/// </summary>
		/// <param name="oldInstance"> The instance to be copied. </param>
		/// <param name="newInstance"> The instance that is to be modified. </param>
		/// <returns> True if an equivalent copy of <code>newInstance</code> may be
		///         created by applying a series of mutations to <code>oldInstance</code>.
		/// </returns>
		/// <seealso cref= #DefaultPersistenceDelegate(String[]) </seealso>
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			// Assume the instance is either mutable or a singleton
			// if it has a nullary constructor.
			return (Constructor.Length == 0) || !DefinesEquals(oldInstance) ? base.MutatesTo(oldInstance, newInstance) : oldInstance.Equals(newInstance);
		}

		/// <summary>
		/// This default implementation of the <code>instantiate</code> method returns
		/// an expression containing the predefined method name "new" which denotes a
		/// call to a constructor with the arguments as specified in
		/// the <code>DefaultPersistenceDelegate</code>'s constructor.
		/// </summary>
		/// <param name="oldInstance"> The instance to be instantiated. </param>
		/// <param name="out"> The code output stream. </param>
		/// <returns> An expression whose value is <code>oldInstance</code>.
		/// </returns>
		/// <exception cref="NullPointerException"> if {@code out} is {@code null}
		///                              and this value is used in the method
		/// </exception>
		/// <seealso cref= #DefaultPersistenceDelegate(String[]) </seealso>
		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			int nArgs = Constructor.Length;
			Class type = oldInstance.GetType();
			Object[] constructorArgs = new Object[nArgs];
			for (int i = 0; i < nArgs; i++)
			{
				try
				{
					Method method = FindMethod(type, this.Constructor[i]);
					constructorArgs[i] = MethodUtil.invoke(method, oldInstance, new Object[0]);
				}
				catch (Exception e)
				{
					@out.ExceptionListener.ExceptionThrown(e);
				}
			}
			return new Expression(oldInstance, oldInstance.GetType(), "new", constructorArgs);
		}

		private Method FindMethod(Class type, String property)
		{
			if (property == null)
			{
				throw new IllegalArgumentException("Property name is null");
			}
			PropertyDescriptor pd = GetPropertyDescriptor(type, property);
			if (pd == null)
			{
				throw new IllegalStateException("Could not find property by the name " + property);
			}
			Method method = pd.ReadMethod;
			if (method == null)
			{
				throw new IllegalStateException("Could not find getter for the property " + property);
			}
			return method;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void doProperty(Class type, PropertyDescriptor pd, Object oldInstance, Object newInstance, Encoder out) throws Exception
		private void DoProperty(Class type, PropertyDescriptor pd, Object oldInstance, Object newInstance, Encoder @out)
		{
			Method getter = pd.ReadMethod;
			Method setter = pd.WriteMethod;

			if (getter != null && setter != null)
			{
				Expression oldGetExp = new Expression(oldInstance, getter.Name, new Object[]{});
				Expression newGetExp = new Expression(newInstance, getter.Name, new Object[]{});
				Object oldValue = oldGetExp.Value;
				Object newValue = newGetExp.Value;
				@out.WriteExpression(oldGetExp);
				if (!Objects.Equals(newValue, @out.Get(oldValue)))
				{
					// Search for a static constant with this value;
					Object e = (Object[])pd.GetValue("enumerationValues");
					if (e is Object[] && Array.getLength(e) % 3 == 0)
					{
						Object[] a = (Object[])e;
						for (int i = 0; i < a.Length; i = i + 3)
						{
							try
							{
							   Field f = type.GetField((String)a[i]);
							   if (f.Get(null).Equals(oldValue))
							   {
								   @out.Remove(oldValue);
								   @out.WriteExpression(new Expression(oldValue, f, "get", new Object[]{null}));
							   }
							}
							catch (Exception)
							{
							}
						}
					}
					InvokeStatement(oldInstance, setter.Name, new Object[]{oldValue}, @out);
				}
			}
		}

		internal static void InvokeStatement(Object instance, String methodName, Object[] args, Encoder @out)
		{
			@out.WriteStatement(new Statement(instance, methodName, args));
		}

		// Write out the properties of this instance.
		private void InitBean(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			foreach (Field field in type.Fields)
			{
				if (!ReflectUtil.isPackageAccessible(field.DeclaringClass))
				{
					continue;
				}
				int mod = field.Modifiers;
				if (Modifier.IsFinal(mod) || Modifier.IsStatic(mod) || Modifier.IsTransient(mod))
				{
					continue;
				}
				try
				{
					Expression oldGetExp = new Expression(field, "get", new Object[] {oldInstance});
					Expression newGetExp = new Expression(field, "get", new Object[] {newInstance});
					Object oldValue = oldGetExp.Value;
					Object newValue = newGetExp.Value;
					@out.WriteExpression(oldGetExp);
					if (!Objects.Equals(newValue, @out.Get(oldValue)))
					{
						@out.WriteStatement(new Statement(field, "set", new Object[] {oldInstance, oldValue}));
					}
				}
				catch (Exception exception)
				{
					@out.ExceptionListener.ExceptionThrown(exception);
				}
			}
			BeanInfo info;
			try
			{
				info = Introspector.GetBeanInfo(type);
			}
			catch (IntrospectionException)
			{
				return;
			}
			// Properties
			foreach (PropertyDescriptor d in info.PropertyDescriptors)
			{
				if (d.IsTransient())
				{
					continue;
				}
				try
				{
					DoProperty(type, d, oldInstance, newInstance, @out);
				}
				catch (Exception e)
				{
					@out.ExceptionListener.ExceptionThrown(e);
				}
			}

			// Listeners
			/*
			Pending(milne). There is a general problem with the archival of
			listeners which is unresolved as of 1.4. Many of the methods
			which install one object inside another (typically "add" methods
			or setters) automatically install a listener on the "child" object
			so that its "parent" may respond to changes that are made to it.
			For example the JTable:setModel() method automatically adds a
			TableModelListener (the JTable itself in this case) to the supplied
			table model.
	
			We do not need to explicitly add these listeners to the model in an
			archive as they will be added automatically by, in the above case,
			the JTable's "setModel" method. In some cases, we must specifically
			avoid trying to do this since the listener may be an inner class
			that cannot be instantiated using public API.
	
			No general mechanism currently
			exists for differentiating between these kind of listeners and
			those which were added explicitly by the user. A mechanism must
			be created to provide a general means to differentiate these
			special cases so as to provide reliable persistence of listeners
			for the general case.
			*/
			if (!type.IsSubclassOf(typeof(java.awt.Component)))
			{
				return; // Just handle the listeners of Components for now.
			}
			foreach (EventSetDescriptor d in info.EventSetDescriptors)
			{
				if (d.IsTransient())
				{
					continue;
				}
				Class listenerType = d.ListenerType;


				// The ComponentListener is added automatically, when
				// Contatiner:add is called on the parent.
				if (listenerType == typeof(java.awt.@event.ComponentListener))
				{
					continue;
				}

				// JMenuItems have a change listener added to them in
				// their "add" methods to enable accessibility support -
				// see the add method in JMenuItem for details. We cannot
				// instantiate this instance as it is a private inner class
				// and do not need to do this anyway since it will be created
				// and installed by the "add" method. Special case this for now,
				// ignoring all change listeners on JMenuItems.
				if (listenerType == typeof(javax.swing.@event.ChangeListener) && type == typeof(javax.swing.JMenuItem))
				{
					continue;
				}

				EventListener[] oldL = new EventListener[0];
				EventListener[] newL = new EventListener[0];
				try
				{
					Method m = d.GetListenerMethod;
					oldL = (EventListener[])MethodUtil.invoke(m, oldInstance, new Object[]{});
					newL = (EventListener[])MethodUtil.invoke(m, newInstance, new Object[]{});
				}
				catch (Exception)
				{
					try
					{
						Method m = type.GetMethod("getListeners", new Class[]{typeof(Class)});
						oldL = (EventListener[])MethodUtil.invoke(m, oldInstance, new Object[]{listenerType});
						newL = (EventListener[])MethodUtil.invoke(m, newInstance, new Object[]{listenerType});
					}
					catch (Exception)
					{
						return;
					}
				}

				// Asssume the listeners are in the same order and that there are no gaps.
				// Eventually, this may need to do true differencing.
				String addListenerMethodName = d.AddListenerMethod.Name;
				for (int i = newL.Length; i < oldL.Length; i++)
				{
					// System.out.println("Adding listener: " + addListenerMethodName + oldL[i]);
					InvokeStatement(oldInstance, addListenerMethodName, new Object[]{oldL[i]}, @out);
				}

				String removeListenerMethodName = d.RemoveListenerMethod.Name;
				for (int i = oldL.Length; i < newL.Length; i++)
				{
					InvokeStatement(oldInstance, removeListenerMethodName, new Object[]{newL[i]}, @out);
				}
			}
		}

		/// <summary>
		/// This default implementation of the <code>initialize</code> method assumes
		/// all state held in objects of this type is exposed via the
		/// matching pairs of "setter" and "getter" methods in the order
		/// they are returned by the Introspector. If a property descriptor
		/// defines a "transient" attribute with a value equal to
		/// <code>Boolean.TRUE</code> the property is ignored by this
		/// default implementation. Note that this use of the word
		/// "transient" is quite independent of the field modifier
		/// that is used by the <code>ObjectOutputStream</code>.
		/// <para>
		/// For each non-transient property, an expression is created
		/// in which the nullary "getter" method is applied
		/// to the <code>oldInstance</code>. The value of this
		/// expression is the value of the property in the instance that is
		/// being serialized. If the value of this expression
		/// in the cloned environment <code>mutatesTo</code> the
		/// target value, the new value is initialized to make it
		/// equivalent to the old value. In this case, because
		/// the property value has not changed there is no need to
		/// call the corresponding "setter" method and no statement
		/// is emitted. If not however, the expression for this value
		/// is replaced with another expression (normally a constructor)
		/// and the corresponding "setter" method is called to install
		/// the new property value in the object. This scheme removes
		/// default information from the output produced by streams
		/// using this delegate.
		/// </para>
		/// <para>
		/// In passing these statements to the output stream, where they
		/// will be executed, side effects are made to the <code>newInstance</code>.
		/// In most cases this allows the problem of properties
		/// whose values depend on each other to actually help the
		/// serialization process by making the number of statements
		/// that need to be written to the output smaller. In general,
		/// the problem of handling interdependent properties is reduced to
		/// that of finding an order for the properties in
		/// a class such that no property value depends on the value of
		/// a subsequent property.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the type of the instances </param>
		/// <param name="oldInstance"> The instance to be copied. </param>
		/// <param name="newInstance"> The instance that is to be modified. </param>
		/// <param name="out"> The stream to which any initialization statements should be written.
		/// </param>
		/// <exception cref="NullPointerException"> if {@code out} is {@code null}
		/// </exception>
		/// <seealso cref= java.beans.Introspector#getBeanInfo </seealso>
		/// <seealso cref= java.beans.PropertyDescriptor </seealso>
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			// System.out.println("DefulatPD:initialize" + type);
			base.Initialize(type, oldInstance, newInstance, @out);
			if (oldInstance.GetType() == type) // !type.isInterface()) {
			{
				InitBean(type, oldInstance, newInstance, @out);
			}
		}

		private static PropertyDescriptor GetPropertyDescriptor(Class type, String property)
		{
			try
			{
				foreach (PropertyDescriptor pd in Introspector.GetBeanInfo(type).PropertyDescriptors)
				{
					if (property.Equals(pd.Name))
					{
						return pd;
					}
				}
			}
			catch (IntrospectionException)
			{
			}
			return null;
		}
	}

}