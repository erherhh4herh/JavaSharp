using System;
using System.Collections.Generic;

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

	using PrimitiveWrapperMap = com.sun.beans.finder.PrimitiveWrapperMap;





	using PrintColorUIResource = sun.swing.PrintColorUIResource;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to .NET:
	import static sun.reflect.misc.ReflectUtil.isPackageAccessible;

	/*
	 * Like the <code>Intropector</code>, the <code>MetaData</code> class
	 * contains <em>meta</em> objects that describe the way
	 * classes should express their state in terms of their
	 * own public APIs.
	 *
	 * @see java.beans.Intropector
	 *
	 * @author Philip Milne
	 * @author Steve Langley
	 */
	internal class MetaData
	{

	internal sealed class NullPersistenceDelegate : PersistenceDelegate
	{
		// Note this will be called by all classes when they reach the
		// top of their superclass chain.
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
		}
		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			return null;
		}

		public override void WriteObject(Object oldInstance, Encoder @out)
		{
		// System.out.println("NullPersistenceDelegate:writeObject " + oldInstance);
		}
	}

	/// <summary>
	/// The persistence delegate for <CODE>enum</CODE> classes.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	internal sealed class EnumPersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance == newInstance;
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Enum<?> e = (Enum<?>) oldInstance;
			Enum<?> e = (Enum<?>) oldInstance;
			return new Expression(e, typeof(Enum), "valueOf", new Object[]{e.DeclaringClass, e.Name()});
		}
	}

	internal sealed class PrimitivePersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance.Equals(newInstance);
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			return new Expression(oldInstance, oldInstance.GetType(), "new", new Object[]{oldInstance.ToString()});
		}
	}

	internal sealed class ArrayPersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return (newInstance != null && oldInstance.GetType() == newInstance.GetType() && Array.getLength(oldInstance) == Array.getLength(newInstance)); // Also ensures the subtype is correct.
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			// System.out.println("instantiate: " + type + " " + oldInstance);
			Class oldClass = oldInstance.GetType();
			return new Expression(oldInstance, typeof(Array), "newInstance", new Object[]{oldClass.ComponentType, new Integer(Array.getLength(oldInstance))});
		}

		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			int n = Array.getLength(oldInstance);
			for (int i = 0; i < n; i++)
			{
				Object index = new Integer(i);
				// Expression oldGetExp = new Expression(Array.class, "get", new Object[]{oldInstance, index});
				// Expression newGetExp = new Expression(Array.class, "get", new Object[]{newInstance, index});
				Expression oldGetExp = new Expression(oldInstance, "get", new Object[]{index});
				Expression newGetExp = new Expression(newInstance, "get", new Object[]{index});
				try
				{
					Object oldValue = oldGetExp.Value;
					Object newValue = newGetExp.Value;
					@out.WriteExpression(oldGetExp);
					if (!Objects.Equals(newValue, @out.Get(oldValue)))
					{
						// System.out.println("Not equal: " + newGetExp + " != " + actualGetExp);
						// invokeStatement(Array.class, "set", new Object[]{oldInstance, index, oldValue}, out);
						DefaultPersistenceDelegate.InvokeStatement(oldInstance, "set", new Object[]{index, oldValue}, @out);
					}
				}
				catch (Exception e)
				{
					// System.err.println("Warning:: failed to write: " + oldGetExp);
					@out.ExceptionListener.ExceptionThrown(e);
				}
			}
		}
	}

	internal sealed class ProxyPersistenceDelegate : PersistenceDelegate
	{
		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			Class type = oldInstance.GetType();
			java.lang.reflect.Proxy p = (java.lang.reflect.Proxy)oldInstance;
			// This unappealing hack is not required but makes the
			// representation of EventHandlers much more concise.
			java.lang.reflect.InvocationHandler ih = java.lang.reflect.Proxy.GetInvocationHandler(p);
			if (ih is EventHandler)
			{
				EventHandler eh = (EventHandler)ih;
				Vector<Object> args = new Vector<Object>();
				args.Add(type.Interfaces[0]);
				args.Add(eh.Target);
				args.Add(eh.Action);
				if (eh.EventPropertyName != null)
				{
					args.Add(eh.EventPropertyName);
				}
				if (eh.ListenerMethodName != null)
				{
					args.Size = 4;
					args.Add(eh.ListenerMethodName);
				}
				return new Expression(oldInstance, typeof(EventHandler), "create", args.ToArray());
			}
			return new Expression(oldInstance, typeof(java.lang.reflect.Proxy), "newProxyInstance", new Object[]{type.ClassLoader, type.Interfaces, ih});
		}
	}

	// Strings
	internal sealed class java_lang_String_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			return null;
		}

		public override void WriteObject(Object oldInstance, Encoder @out)
		{
			// System.out.println("NullPersistenceDelegate:writeObject " + oldInstance);
		}
	}

	// Classes
	internal sealed class java_lang_Class_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance.Equals(newInstance);
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			Class c = (Class)oldInstance;
			// As of 1.3 it is not possible to call Class.forName("int"),
			// so we have to generate different code for primitive types.
			// This is needed for arrays whose subtype may be primitive.
			if (c.Primitive)
			{
				Field field = null;
				try
				{
					field = PrimitiveWrapperMap.GetType(c.Name).getDeclaredField("TYPE");
				}
				catch (NoSuchFieldException)
				{
					System.Console.Error.WriteLine("Unknown primitive type: " + c);
				}
				return new Expression(oldInstance, field, "get", new Object[]{null});
			}
			else if (oldInstance == typeof(String))
			{
				return new Expression(oldInstance, "", "getClass", new Object[]{});
			}
			else if (oldInstance == typeof(Class))
			{
				return new Expression(oldInstance, typeof(String), "getClass", new Object[]{});
			}
			else
			{
				Expression newInstance = new Expression(oldInstance, typeof(Class), "forName", new Object[] {c.Name});
				newInstance.Loader = c.ClassLoader;
				return newInstance;
			}
		}
	}

	// Fields
	internal sealed class java_lang_reflect_Field_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance.Equals(newInstance);
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			Field f = (Field)oldInstance;
			return new Expression(oldInstance, f.DeclaringClass, "getField", new Object[]{f.Name});
		}
	}

	// Methods
	internal sealed class java_lang_reflect_Method_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance.Equals(newInstance);
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			Method m = (Method)oldInstance;
			return new Expression(oldInstance, m.DeclaringClass, "getMethod", new Object[]{m.Name, m.ParameterTypes});
		}
	}

	// Dates

	/// <summary>
	/// The persistence delegate for <CODE>java.util.Date</CODE> classes.
	/// Do not extend DefaultPersistenceDelegate to improve performance and
	/// to avoid problems with <CODE>java.sql.Date</CODE>,
	/// <CODE>java.sql.Time</CODE> and <CODE>java.sql.Timestamp</CODE>.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	internal class java_util_Date_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			if (!base.MutatesTo(oldInstance, newInstance))
			{
				return false;
			}
			Date oldDate = (Date)oldInstance;
			Date newDate = (Date)newInstance;

			return oldDate.Time == newDate.Time;
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			Date date = (Date)oldInstance;
			return new Expression(date, date.GetType(), "new", new Object[] {date.Time});
		}
	}

	/// <summary>
	/// The persistence delegate for <CODE>java.sql.Timestamp</CODE> classes.
	/// It supports nanoseconds.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	internal sealed class java_sql_Timestamp_PersistenceDelegate : java_util_Date_PersistenceDelegate
	{
		internal static readonly Method GetNanosMethod = NanosMethod;

		internal static Method NanosMethod
		{
			get
			{
				try
				{
					Class c = Class.ForName("java.sql.Timestamp", true, null);
					return c.getMethod("getNanos");
				}
				catch (ClassNotFoundException)
				{
					return null;
				}
				catch (NoSuchMethodException e)
				{
					throw new AssertionError(e);
				}
			}
		}

		/// <summary>
		/// Invoke Timstamp getNanos.
		/// </summary>
		internal static int GetNanos(Object obj)
		{
			if (GetNanosMethod == null)
			{
				throw new AssertionError("Should not get here");
			}
			try
			{
				return (Integer)GetNanosMethod.invoke(obj);
			}
			catch (InvocationTargetException e)
			{
				Throwable cause = e.InnerException;
				if (cause is RuntimeException)
				{
					throw (RuntimeException)cause;
				}
				if (cause is Error)
				{
					throw (Error)cause;
				}
				throw new AssertionError(e);
			}
			catch (IllegalAccessException iae)
			{
				throw new AssertionError(iae);
			}
		}

		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			// assumes oldInstance and newInstance are Timestamps
			int nanos = GetNanos(oldInstance);
			if (nanos != GetNanos(newInstance))
			{
				@out.WriteStatement(new Statement(oldInstance, "setNanos", new Object[] {nanos}));
			}
		}
	}

	// Collections

	/*
	The Hashtable and AbstractMap classes have no common ancestor yet may
	be handled with a single persistence delegate: one which uses the methods
	of the Map insterface exclusively. Attatching the persistence delegates
	to the interfaces themselves is fraught however since, in the case of
	the Map, both the AbstractMap and HashMap classes are declared to
	implement the Map interface, leaving the obvious implementation prone
	to repeating their initialization. These issues and questions around
	the ordering of delegates attached to interfaces have lead us to
	ignore any delegates attached to interfaces and force all persistence
	delegates to be registered with concrete classes.
	*/

	/// <summary>
	/// The base class for persistence delegates for inner classes
	/// that can be created using <seealso cref="Collections"/>.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	private abstract class java_util_Collections : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			if (!base.MutatesTo(oldInstance, newInstance))
			{
				return false;
			}
			if ((oldInstance is List) || (oldInstance is Set) || (oldInstance is Map))
			{
				return oldInstance.Equals(newInstance);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Collection<?> oldC = (Collection<?>) oldInstance;
			Collection<?> oldC = (Collection<?>) oldInstance;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Collection<?> newC = (Collection<?>) newInstance;
			Collection<?> newC = (Collection<?>) newInstance;
			return (oldC.Size() == newC.Size()) && oldC.ContainsAll(newC);
		}

		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			// do not initialize these custom collections in default way
		}

		internal sealed class EmptyList_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				return new Expression(oldInstance, typeof(Collections), "emptyList", null);
			}
		}

		internal sealed class EmptySet_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				return new Expression(oldInstance, typeof(Collections), "emptySet", null);
			}
		}

		internal sealed class EmptyMap_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				return new Expression(oldInstance, typeof(Collections), "emptyMap", null);
			}
		}

		internal sealed class SingletonList_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = (List<?>) oldInstance;
				List<?> list = (List<?>) oldInstance;
				return new Expression(oldInstance, typeof(Collections), "singletonList", new Object[]{list.Get(0)});
			}
		}

		internal sealed class SingletonSet_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Set<?> set = (Set<?>) oldInstance;
				Set<?> set = (Set<?>) oldInstance;
				return new Expression(oldInstance, typeof(Collections), "singleton", new Object[]{set.Iterator().Next()});
			}
		}

		internal sealed class SingletonMap_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map<?,?> map = (Map<?,?>) oldInstance;
				Map<?, ?> map = (Map<?, ?>) oldInstance;
				Object key = map.KeySet().Iterator().Next();
				return new Expression(oldInstance, typeof(Collections), "singletonMap", new Object[]{key, map.Get(key)});
			}
		}

		internal sealed class UnmodifiableCollection_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = new ArrayList<>((Collection<?>) oldInstance);
				List<?> list = new List<?>((Collection<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "unmodifiableCollection", new Object[]{list});
			}
		}

		internal sealed class UnmodifiableList_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = new LinkedList<>((Collection<?>) oldInstance);
				List<?> list = new LinkedList<?>((Collection<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "unmodifiableList", new Object[]{list});
			}
		}

		internal sealed class UnmodifiableRandomAccessList_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = new ArrayList<>((Collection<?>) oldInstance);
				List<?> list = new List<?>((Collection<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "unmodifiableList", new Object[]{list});
			}
		}

		internal sealed class UnmodifiableSet_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Set<?> set = new HashSet<>((Set<?>) oldInstance);
				Set<?> set = new HashSet<?>((Set<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "unmodifiableSet", new Object[]{set});
			}
		}

		internal sealed class UnmodifiableSortedSet_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: SortedSet<?> set = new TreeSet<>((SortedSet<?>) oldInstance);
				SortedSet<?> set = new TreeSet<?>((SortedSet<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "unmodifiableSortedSet", new Object[]{set});
			}
		}

		internal sealed class UnmodifiableMap_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map<?,?> map = new HashMap<>((Map<?,?>) oldInstance);
				Map<?, ?> map = new HashMap<?, ?>((Map<?, ?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "unmodifiableMap", new Object[]{map});
			}
		}

		internal sealed class UnmodifiableSortedMap_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: SortedMap<?,?> map = new TreeMap<>((SortedMap<?,?>) oldInstance);
				SortedMap<?, ?> map = new TreeMap<?, ?>((SortedMap<?, ?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "unmodifiableSortedMap", new Object[]{map});
			}
		}

		internal sealed class SynchronizedCollection_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = new ArrayList<>((Collection<?>) oldInstance);
				List<?> list = new List<?>((Collection<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "synchronizedCollection", new Object[]{list});
			}
		}

		internal sealed class SynchronizedList_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = new LinkedList<>((Collection<?>) oldInstance);
				List<?> list = new LinkedList<?>((Collection<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "synchronizedList", new Object[]{list});
			}
		}

		internal sealed class SynchronizedRandomAccessList_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = new ArrayList<>((Collection<?>) oldInstance);
				List<?> list = new List<?>((Collection<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "synchronizedList", new Object[]{list});
			}
		}

		internal sealed class SynchronizedSet_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Set<?> set = new HashSet<>((Set<?>) oldInstance);
				Set<?> set = new HashSet<?>((Set<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "synchronizedSet", new Object[]{set});
			}
		}

		internal sealed class SynchronizedSortedSet_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: SortedSet<?> set = new TreeSet<>((SortedSet<?>) oldInstance);
				SortedSet<?> set = new TreeSet<?>((SortedSet<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "synchronizedSortedSet", new Object[]{set});
			}
		}

		internal sealed class SynchronizedMap_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map<?,?> map = new HashMap<>((Map<?,?>) oldInstance);
				Map<?, ?> map = new HashMap<?, ?>((Map<?, ?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "synchronizedMap", new Object[]{map});
			}
		}

		internal sealed class SynchronizedSortedMap_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: SortedMap<?,?> map = new TreeMap<>((SortedMap<?,?>) oldInstance);
				SortedMap<?, ?> map = new TreeMap<?, ?>((SortedMap<?, ?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "synchronizedSortedMap", new Object[]{map});
			}
		}

		internal sealed class CheckedCollection_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				Object type = MetaData.GetPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type");
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = new ArrayList<>((Collection<?>) oldInstance);
				List<?> list = new List<?>((Collection<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "checkedCollection", new Object[]{list, type});
			}
		}

		internal sealed class CheckedList_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				Object type = MetaData.GetPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type");
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = new LinkedList<>((Collection<?>) oldInstance);
				List<?> list = new LinkedList<?>((Collection<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "checkedList", new Object[]{list, type});
			}
		}

		internal sealed class CheckedRandomAccessList_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				Object type = MetaData.GetPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type");
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: List<?> list = new ArrayList<>((Collection<?>) oldInstance);
				List<?> list = new List<?>((Collection<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "checkedList", new Object[]{list, type});
			}
		}

		internal sealed class CheckedSet_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				Object type = MetaData.GetPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type");
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Set<?> set = new HashSet<>((Set<?>) oldInstance);
				Set<?> set = new HashSet<?>((Set<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "checkedSet", new Object[]{set, type});
			}
		}

		internal sealed class CheckedSortedSet_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				Object type = MetaData.GetPrivateFieldValue(oldInstance, "java.util.Collections$CheckedCollection.type");
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: SortedSet<?> set = new TreeSet<>((SortedSet<?>) oldInstance);
				SortedSet<?> set = new TreeSet<?>((SortedSet<?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "checkedSortedSet", new Object[]{set, type});
			}
		}

		internal sealed class CheckedMap_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				Object keyType = MetaData.GetPrivateFieldValue(oldInstance, "java.util.Collections$CheckedMap.keyType");
				Object valueType = MetaData.GetPrivateFieldValue(oldInstance, "java.util.Collections$CheckedMap.valueType");
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map<?,?> map = new HashMap<>((Map<?,?>) oldInstance);
				Map<?, ?> map = new HashMap<?, ?>((Map<?, ?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "checkedMap", new Object[]{map, keyType, valueType});
			}
		}

		internal sealed class CheckedSortedMap_PersistenceDelegate : java_util_Collections
		{
			protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
			{
				Object keyType = MetaData.GetPrivateFieldValue(oldInstance, "java.util.Collections$CheckedMap.keyType");
				Object valueType = MetaData.GetPrivateFieldValue(oldInstance, "java.util.Collections$CheckedMap.valueType");
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: SortedMap<?,?> map = new TreeMap<>((SortedMap<?,?>) oldInstance);
				SortedMap<?, ?> map = new TreeMap<?, ?>((SortedMap<?, ?>) oldInstance);
				return new Expression(oldInstance, typeof(Collections), "checkedSortedMap", new Object[]{map, keyType, valueType});
			}
		}
	}

	/// <summary>
	/// The persistence delegate for <CODE>java.util.EnumMap</CODE> classes.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	internal sealed class java_util_EnumMap_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return base.MutatesTo(oldInstance, newInstance) && (GetType(oldInstance) == GetType(newInstance));
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			return new Expression(oldInstance, typeof(EnumMap), "new", new Object[] {GetType(oldInstance)});
		}

		internal static Object GetType(Object instance)
		{
			return MetaData.GetPrivateFieldValue(instance, "java.util.EnumMap.keyType");
		}
	}

	/// <summary>
	/// The persistence delegate for <CODE>java.util.EnumSet</CODE> classes.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	internal sealed class java_util_EnumSet_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return base.MutatesTo(oldInstance, newInstance) && (GetType(oldInstance) == GetType(newInstance));
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			return new Expression(oldInstance, typeof(EnumSet), "noneOf", new Object[] {GetType(oldInstance)});
		}

		internal static Object GetType(Object instance)
		{
			return MetaData.GetPrivateFieldValue(instance, "java.util.EnumSet.elementType");
		}
	}

	// Collection
	internal class java_util_Collection_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Collection<?> oldO = (java.util.Collection)oldInstance;
			ICollection<?> oldO = (ICollection)oldInstance;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Collection<?> newO = (java.util.Collection)newInstance;
			ICollection<?> newO = (ICollection)newInstance;

			if (newO.Count != 0)
			{
				InvokeStatement(oldInstance, "clear", new Object[]{}, @out);
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Iterator<?> i = oldO.iterator(); i.hasNext();)
			for (Iterator<?> i = oldO.GetEnumerator(); i.HasNext();)
			{
				InvokeStatement(oldInstance, "add", new Object[]{i.Next()}, @out);
			}
		}
	}

	// List
	internal class java_util_List_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<?> oldO = (java.util.List<?>)oldInstance;
			IList<?> oldO = (IList<?>)oldInstance;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<?> newO = (java.util.List<?>)newInstance;
			IList<?> newO = (IList<?>)newInstance;
			int oldSize = oldO.Count;
			int newSize = (newO == null) ? 0 : newO.Count;
			if (oldSize < newSize)
			{
				InvokeStatement(oldInstance, "clear", new Object[]{}, @out);
				newSize = 0;
			}
			for (int i = 0; i < newSize; i++)
			{
				Object index = new Integer(i);

				Expression oldGetExp = new Expression(oldInstance, "get", new Object[]{index});
				Expression newGetExp = new Expression(newInstance, "get", new Object[]{index});
				try
				{
					Object oldValue = oldGetExp.Value;
					Object newValue = newGetExp.Value;
					@out.WriteExpression(oldGetExp);
					if (!Objects.Equals(newValue, @out.Get(oldValue)))
					{
						InvokeStatement(oldInstance, "set", new Object[]{index, oldValue}, @out);
					}
				}
				catch (Exception e)
				{
					@out.ExceptionListener.ExceptionThrown(e);
				}
			}
			for (int i = newSize; i < oldSize; i++)
			{
				InvokeStatement(oldInstance, "add", new Object[]{oldO[i]}, @out);
			}
		}
	}


	// Map
	internal class java_util_Map_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			// System.out.println("Initializing: " + newInstance);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<?,?> oldMap = (java.util.Map)oldInstance;
			IDictionary<?, ?> oldMap = (IDictionary)oldInstance;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<?,?> newMap = (java.util.Map)newInstance;
			IDictionary<?, ?> newMap = (IDictionary)newInstance;
			// Remove the new elements.
			// Do this first otherwise we undo the adding work.
			if (newMap != null)
			{
				foreach (Object newKey in newMap.Keys.toArray())
				{
				   // PENDING: This "key" is not in the right environment.
					if (!oldMap.ContainsKey(newKey))
					{
						InvokeStatement(oldInstance, "remove", new Object[]{newKey}, @out);
					}
				}
			}
			// Add the new elements.
			foreach (Object oldKey in oldMap.Keys)
			{
				Expression oldGetExp = new Expression(oldInstance, "get", new Object[]{oldKey});
				// Pending: should use newKey.
				Expression newGetExp = new Expression(newInstance, "get", new Object[]{oldKey});
				try
				{
					Object oldValue = oldGetExp.Value;
					Object newValue = newGetExp.Value;
					@out.WriteExpression(oldGetExp);
					if (!Objects.Equals(newValue, @out.Get(oldValue)))
					{
						InvokeStatement(oldInstance, "put", new Object[]{oldKey, oldValue}, @out);
					}
					else if ((newValue == null) && !newMap.ContainsKey(oldKey))
					{
						// put oldValue(=null?) if oldKey is absent in newMap
						InvokeStatement(oldInstance, "put", new Object[]{oldKey, oldValue}, @out);
					}
				}
				catch (Exception e)
				{
					@out.ExceptionListener.ExceptionThrown(e);
				}
			}
		}
	}

	internal sealed class java_util_AbstractCollection_PersistenceDelegate : java_util_Collection_PersistenceDelegate
	{
	}
	internal sealed class java_util_AbstractList_PersistenceDelegate : java_util_List_PersistenceDelegate
	{
	}
	internal sealed class java_util_AbstractMap_PersistenceDelegate : java_util_Map_PersistenceDelegate
	{
	}
	internal sealed class java_util_Hashtable_PersistenceDelegate : java_util_Map_PersistenceDelegate
	{
	}


	// Beans
	internal sealed class java_beans_beancontext_BeanContextSupport_PersistenceDelegate : java_util_Collection_PersistenceDelegate
	{
	}

	// AWT

	/// <summary>
	/// The persistence delegate for <seealso cref="Insets"/>.
	/// It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	/// because this class does not have any properties.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	internal sealed class java_awt_Insets_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance.Equals(newInstance);
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			Insets insets = (Insets) oldInstance;
			Object[] args = new Object[] {insets.Top, insets.Left, insets.Bottom, insets.Right};
			return new Expression(insets, insets.GetType(), "new", args);
		}
	}

	/// <summary>
	/// The persistence delegate for <seealso cref="Font"/>.
	/// It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	/// because size of the font can be float value.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	internal sealed class java_awt_Font_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance.Equals(newInstance);
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			Font font = (Font) oldInstance;

			int count = 0;
			String family = null;
			int style = Font.PLAIN;
			int size = 12;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map<java.awt.font.TextAttribute, ?> basic = font.getAttributes();
			Map<TextAttribute, ?> basic = font.Attributes;
			Map<TextAttribute, Object> clone = new HashMap<TextAttribute, Object>(basic.Size());
			foreach (TextAttribute key in basic.KeySet())
			{
				Object value = basic.Get(key);
				if (value != null)
				{
					clone.Put(key, value);
				}
				if (key == TextAttribute.FAMILY)
				{
					if (value is String)
					{
						count++;
						family = (String) value;
					}
				}
				else if (key == TextAttribute.WEIGHT)
				{
					if (TextAttribute.WEIGHT_REGULAR.Equals(value))
					{
						count++;
					}
					else if (TextAttribute.WEIGHT_BOLD.Equals(value))
					{
						count++;
						style |= Font.BOLD;
					}
				}
				else if (key == TextAttribute.POSTURE)
				{
					if (TextAttribute.POSTURE_REGULAR.Equals(value))
					{
						count++;
					}
					else if (TextAttribute.POSTURE_OBLIQUE.Equals(value))
					{
						count++;
						style |= Font.ITALIC;
					}
				}
				else if (key == TextAttribute.SIZE)
				{
					if (value is Number)
					{
						Number number = (Number) value;
						size = number.IntValue();
						if (size == number.FloatValue())
						{
							count++;
						}
					}
				}
			}
			Class type = font.GetType();
			if (count == clone.Size())
			{
				return new Expression(font, type, "new", new Object[]{family, style, size});
			}
			if (type == typeof(Font))
			{
				return new Expression(font, type, "getFont", new Object[]{clone});
			}
			return new Expression(font, type, "new", new Object[]{Font.GetFont(clone)});
		}
	}

	/// <summary>
	/// The persistence delegate for <seealso cref="AWTKeyStroke"/>.
	/// It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	/// because this class have no public constructor.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	internal sealed class java_awt_AWTKeyStroke_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance.Equals(newInstance);
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			AWTKeyStroke key = (AWTKeyStroke) oldInstance;

			char ch = key.KeyChar;
			int code = key.KeyCode;
			int mask = key.Modifiers;
			bool onKeyRelease = key.OnKeyRelease;

			Object[] args = null;
			if (ch == KeyEvent.CHAR_UNDEFINED)
			{
				args = !onKeyRelease ? new Object[]{code, mask} : new Object[]{code, mask, onKeyRelease};
			}
			else if (code == KeyEvent.VK_UNDEFINED)
			{
				if (!onKeyRelease)
				{
					args = (mask == 0) ? new Object[]{ch} : new Object[]{ch, mask};
				}
				else if (mask == 0)
				{
					args = new Object[]{ch, onKeyRelease};
				}
			}
			if (args == null)
			{
				throw new IllegalStateException("Unsupported KeyStroke: " + key);
			}
			Class type = key.GetType();
			String name = type.Name;
			// get short name of the class
			int index = name.LastIndexOf('.') + 1;
			if (index > 0)
			{
				name = name.Substring(index);
			}
			return new Expression(key, type, "get" + name, args);
		}
	}

	internal class StaticFieldsPersistenceDelegate : PersistenceDelegate
	{
		protected internal virtual void InstallFields(Encoder @out, Class cls)
		{
			if (Modifier.isPublic(cls.Modifiers) && isPackageAccessible(cls))
			{
				Field[] fields = cls.Fields;
				for (int i = 0; i < fields.Length; i++)
				{
					Field field = fields[i];
					// Don't install primitives, their identity will not be preserved
					// by wrapping.
					if (field.Type.IsSubclassOf(typeof(Object)))
					{
						@out.WriteExpression(new Expression(field, "get", new Object[]{null}));
					}
				}
			}
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			throw new RuntimeException("Unrecognized instance: " + oldInstance);
		}

		public override void WriteObject(Object oldInstance, Encoder @out)
		{
			if (@out.GetAttribute(this) == null)
			{
				@out.SetAttribute(this, true);
				InstallFields(@out, oldInstance.GetType());
			}
			base.WriteObject(oldInstance, @out);
		}
	}

	// SystemColor
	internal sealed class java_awt_SystemColor_PersistenceDelegate : StaticFieldsPersistenceDelegate
	{
	}

	// TextAttribute
	internal sealed class java_awt_font_TextAttribute_PersistenceDelegate : StaticFieldsPersistenceDelegate
	{
	}

	// MenuShortcut
	internal sealed class java_awt_MenuShortcut_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance.Equals(newInstance);
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			java.awt.MenuShortcut m = (java.awt.MenuShortcut)oldInstance;
			return new Expression(oldInstance, m.GetType(), "new", new Object[]{new Integer(m.Key), Convert.ToBoolean(m.UsesShiftModifier())});
		}
	}

	// Component
	internal sealed class java_awt_Component_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			java.awt.Component c = (java.awt.Component)oldInstance;
			java.awt.Component c2 = (java.awt.Component)newInstance;
			// The "background", "foreground" and "font" properties.
			// The foreground and font properties of Windows change from
			// null to defined values after the Windows are made visible -
			// special case them for now.
			if (!(oldInstance is java.awt.Window))
			{
				Object oldBackground = c.BackgroundSet ? c.Background : null;
				Object newBackground = c2.BackgroundSet ? c2.Background : null;
				if (!Objects.Equals(oldBackground, newBackground))
				{
					InvokeStatement(oldInstance, "setBackground", new Object[] {oldBackground}, @out);
				}
				Object oldForeground = c.ForegroundSet ? c.Foreground : null;
				Object newForeground = c2.ForegroundSet ? c2.Foreground : null;
				if (!Objects.Equals(oldForeground, newForeground))
				{
					InvokeStatement(oldInstance, "setForeground", new Object[] {oldForeground}, @out);
				}
				Object oldFont = c.FontSet ? c.Font : null;
				Object newFont = c2.FontSet ? c2.Font : null;
				if (!Objects.Equals(oldFont, newFont))
				{
					InvokeStatement(oldInstance, "setFont", new Object[] {oldFont}, @out);
				}
			}

			// Bounds
			java.awt.Container p = c.Parent;
			if (p == null || p.Layout == null)
			{
				// Use the most concise construct.
				bool locationCorrect = c.Location.Equals(c2.Location);
				bool sizeCorrect = c.Size.Equals(c2.Size);
				if (!locationCorrect && !sizeCorrect)
				{
					InvokeStatement(oldInstance, "setBounds", new Object[]{c.Bounds}, @out);
				}
				else if (!locationCorrect)
				{
					InvokeStatement(oldInstance, "setLocation", new Object[]{c.Location}, @out);
				}
				else if (!sizeCorrect)
				{
					InvokeStatement(oldInstance, "setSize", new Object[]{c.Size}, @out);
				}
			}
		}
	}

	// Container
	internal sealed class java_awt_Container_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			// Ignore the children of a JScrollPane.
			// Pending(milne) find a better way to do this.
			if (oldInstance is javax.swing.JScrollPane)
			{
				return;
			}
			java.awt.Container oldC = (java.awt.Container)oldInstance;
			java.awt.Component[] oldChildren = oldC.Components;
			java.awt.Container newC = (java.awt.Container)newInstance;
			java.awt.Component[] newChildren = (newC == null) ? new java.awt.Component[0] : newC.Components;

			BorderLayout layout = (oldC.Layout is BorderLayout) ? (BorderLayout)oldC.Layout : null;

			JLayeredPane oldLayeredPane = (oldInstance is JLayeredPane) ? (JLayeredPane) oldInstance : null;

			// Pending. Assume all the new children are unaltered.
			for (int i = newChildren.Length; i < oldChildren.Length; i++)
			{
				Object[] args = (layout != null) ? new Object[] {oldChildren[i], layout.GetConstraints(oldChildren[i])} : (oldLayeredPane != null) ? new Object[] {oldChildren[i], oldLayeredPane.getLayer(oldChildren[i]), Convert.ToInt32(-1)} : new Object[] {oldChildren[i]};

				InvokeStatement(oldInstance, "add", args, @out);
			}
		}
	}

	// Choice
	internal sealed class java_awt_Choice_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			java.awt.Choice m = (java.awt.Choice)oldInstance;
			java.awt.Choice n = (java.awt.Choice)newInstance;
			for (int i = n.ItemCount; i < m.ItemCount; i++)
			{
				InvokeStatement(oldInstance, "add", new Object[]{m.GetItem(i)}, @out);
			}
		}
	}

	// Menu
	internal sealed class java_awt_Menu_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			java.awt.Menu m = (java.awt.Menu)oldInstance;
			java.awt.Menu n = (java.awt.Menu)newInstance;
			for (int i = n.ItemCount; i < m.ItemCount; i++)
			{
				InvokeStatement(oldInstance, "add", new Object[]{m.GetItem(i)}, @out);
			}
		}
	}

	// MenuBar
	internal sealed class java_awt_MenuBar_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			java.awt.MenuBar m = (java.awt.MenuBar)oldInstance;
			java.awt.MenuBar n = (java.awt.MenuBar)newInstance;
			for (int i = n.MenuCount; i < m.MenuCount; i++)
			{
				InvokeStatement(oldInstance, "add", new Object[]{m.GetMenu(i)}, @out);
			}
		}
	}

	// List
	internal sealed class java_awt_List_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			java.awt.List m = (java.awt.List)oldInstance;
			java.awt.List n = (java.awt.List)newInstance;
			for (int i = n.ItemCount; i < m.ItemCount; i++)
			{
				InvokeStatement(oldInstance, "add", new Object[]{m.GetItem(i)}, @out);
			}
		}
	}


	// LayoutManagers

	// BorderLayout
	internal sealed class java_awt_BorderLayout_PersistenceDelegate : DefaultPersistenceDelegate
	{
		internal static readonly String[] CONSTRAINTS = new String[] {BorderLayout.NORTH, BorderLayout.SOUTH, BorderLayout.EAST, BorderLayout.WEST, BorderLayout.CENTER, BorderLayout.PAGE_START, BorderLayout.PAGE_END, BorderLayout.LINE_START, BorderLayout.LINE_END};
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			BorderLayout oldLayout = (BorderLayout) oldInstance;
			BorderLayout newLayout = (BorderLayout) newInstance;
			foreach (String constraints in CONSTRAINTS)
			{
				Object oldC = oldLayout.GetLayoutComponent(constraints);
				Object newC = newLayout.GetLayoutComponent(constraints);
				// Pending, assume any existing elements are OK.
				if (oldC != null && newC == null)
				{
					InvokeStatement(oldInstance, "addLayoutComponent", new Object[] {oldC, constraints}, @out);
				}
			}
		}
	}

	// CardLayout
	internal sealed class java_awt_CardLayout_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			if (GetVector(newInstance).Empty)
			{
				foreach (Object card in GetVector(oldInstance))
				{
					Object[] args = new Object[] {MetaData.GetPrivateFieldValue(card, "java.awt.CardLayout$Card.name"), MetaData.GetPrivateFieldValue(card, "java.awt.CardLayout$Card.comp")};
					InvokeStatement(oldInstance, "addLayoutComponent", args, @out);
				}
			}
		}
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return base.MutatesTo(oldInstance, newInstance) && GetVector(newInstance).Empty;
		}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static Vector<?> getVector(Object instance)
		internal static Vector<?> GetVector(Object instance)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (Vector<?>) MetaData.getPrivateFieldValue(instance, "java.awt.CardLayout.vector");
			return (Vector<?>) MetaData.GetPrivateFieldValue(instance, "java.awt.CardLayout.vector");
		}
	}

	// GridBagLayout
	internal sealed class java_awt_GridBagLayout_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			if (GetHashtable(newInstance).Empty)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Map_Entry<?,?> entry : getHashtable(oldInstance).entrySet())
				foreach (Map_Entry<?, ?> entry in GetHashtable(oldInstance).EntrySet())
				{
					Object[] args = new Object[] {entry.Key, entry.Value};
					InvokeStatement(oldInstance, "addLayoutComponent", args, @out);
				}
			}
		}
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return base.MutatesTo(oldInstance, newInstance) && GetHashtable(newInstance).Empty;
		}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static Hashtable<?,?> getHashtable(Object instance)
		internal static Dictionary<?, ?> GetHashtable(Object instance)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (Hashtable<?,?>) MetaData.getPrivateFieldValue(instance, "java.awt.GridBagLayout.comptable");
			return (Dictionary<?, ?>) MetaData.GetPrivateFieldValue(instance, "java.awt.GridBagLayout.comptable");
		}
	}

	// Swing

	// JFrame (If we do this for Window instead of JFrame, the setVisible call
	// will be issued before we have added all the children to the JFrame and
	// will appear blank).
	internal sealed class javax_swing_JFrame_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			java.awt.Window oldC = (java.awt.Window)oldInstance;
			java.awt.Window newC = (java.awt.Window)newInstance;
			bool oldV = oldC.Visible;
			bool newV = newC.Visible;
			if (newV != oldV)
			{
				// false means: don't execute this statement at write time.
				bool executeStatements = @out.ExecuteStatements;
				@out.ExecuteStatements = false;
				InvokeStatement(oldInstance, "setVisible", new Object[]{Convert.ToBoolean(oldV)}, @out);
				@out.ExecuteStatements = executeStatements;
			}
		}
	}

	// Models

	// DefaultListModel
	internal sealed class javax_swing_DefaultListModel_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			// Note, the "size" property will be set here.
			base.Initialize(type, oldInstance, newInstance, @out);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: javax.swing.DefaultListModel<?> m = (javax.swing.DefaultListModel<?>)oldInstance;
			javax.swing.DefaultListModel<?> m = (javax.swing.DefaultListModel<?>)oldInstance;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: javax.swing.DefaultListModel<?> n = (javax.swing.DefaultListModel<?>)newInstance;
			javax.swing.DefaultListModel<?> n = (javax.swing.DefaultListModel<?>)newInstance;
			for (int i = n.Size; i < m.Size; i++)
			{
				InvokeStatement(oldInstance, "add", new Object[]{m.getElementAt(i)}, @out); // Can also use "addElement".
			}
		}
	}

	// DefaultComboBoxModel
	internal sealed class javax_swing_DefaultComboBoxModel_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: javax.swing.DefaultComboBoxModel<?> m = (javax.swing.DefaultComboBoxModel<?>)oldInstance;
			javax.swing.DefaultComboBoxModel<?> m = (javax.swing.DefaultComboBoxModel<?>)oldInstance;
			for (int i = 0; i < m.Size; i++)
			{
				InvokeStatement(oldInstance, "addElement", new Object[]{m.getElementAt(i)}, @out);
			}
		}
	}


	// DefaultMutableTreeNode
	internal sealed class javax_swing_tree_DefaultMutableTreeNode_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			javax.swing.tree.DefaultMutableTreeNode m = (javax.swing.tree.DefaultMutableTreeNode)oldInstance;
			javax.swing.tree.DefaultMutableTreeNode n = (javax.swing.tree.DefaultMutableTreeNode)newInstance;
			for (int i = n.ChildCount; i < m.ChildCount; i++)
			{
				InvokeStatement(oldInstance, "add", new Object[]{m.getChildAt(i)}, @out);
			}
		}
	}

	// ToolTipManager
	internal sealed class javax_swing_ToolTipManager_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			return new Expression(oldInstance, typeof(javax.swing.ToolTipManager), "sharedInstance", new Object[]{});
		}
	}

	// JTabbedPane
	internal sealed class javax_swing_JTabbedPane_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			javax.swing.JTabbedPane p = (javax.swing.JTabbedPane)oldInstance;
			for (int i = 0; i < p.TabCount; i++)
			{
				InvokeStatement(oldInstance, "addTab", new Object[]{p.getTitleAt(i), p.getIconAt(i), p.getComponentAt(i)}, @out);
			}
		}
	}

	// Box
	internal sealed class javax_swing_Box_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return base.MutatesTo(oldInstance, newInstance) && GetAxis(oldInstance).Equals(GetAxis(newInstance));
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			return new Expression(oldInstance, oldInstance.GetType(), "new", new Object[] {GetAxis(oldInstance)});
		}

		internal Integer GetAxis(Object @object)
		{
			Box box = (Box) @object;
			return (Integer) MetaData.GetPrivateFieldValue(box.Layout, "javax.swing.BoxLayout.axis");
		}
	}

	// JMenu
	// Note that we do not need to state the initialiser for
	// JMenuItems since the getComponents() method defined in
	// Container will return all of the sub menu items that
	// need to be added to the menu item.
	// Not so for JMenu apparently.
	internal sealed class javax_swing_JMenu_PersistenceDelegate : DefaultPersistenceDelegate
	{
		protected internal override void Initialize(Class type, Object oldInstance, Object newInstance, Encoder @out)
		{
			base.Initialize(type, oldInstance, newInstance, @out);
			javax.swing.JMenu m = (javax.swing.JMenu)oldInstance;
			java.awt.Component[] c = m.MenuComponents;
			for (int i = 0; i < c.Length; i++)
			{
				InvokeStatement(oldInstance, "add", new Object[]{c[i]}, @out);
			}
		}
	}

	/// <summary>
	/// The persistence delegate for <seealso cref="MatteBorder"/>.
	/// It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	/// because this class does not have writable properties.
	/// 
	/// @author Sergey A. Malenkov
	/// </summary>
	internal sealed class javax_swing_border_MatteBorder_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			MatteBorder border = (MatteBorder) oldInstance;
			Insets insets = border.BorderInsets;
			Object @object = border.TileIcon;
			if (@object == null)
			{
				@object = border.MatteColor;
			}
			Object[] args = new Object[] {insets.Top, insets.Left, insets.Bottom, insets.Right, @object};
			return new Expression(border, border.GetType(), "new", args);
		}
	}

	/* XXX - doens't seem to work. Debug later.
	static final class javax_swing_JMenu_PersistenceDelegate extends DefaultPersistenceDelegate {
	    protected void initialize(Class<?> type, Object oldInstance, Object newInstance, Encoder out) {
	        super.initialize(type, oldInstance, newInstance, out);
	        javax.swing.JMenu m = (javax.swing.JMenu)oldInstance;
	        javax.swing.JMenu n = (javax.swing.JMenu)newInstance;
	        for (int i = n.getItemCount(); i < m.getItemCount(); i++) {
	            invokeStatement(oldInstance, "add", new Object[]{m.getItem(i)}, out);
	        }
	    }
	}
	*/

	/// <summary>
	/// The persistence delegate for <seealso cref="PrintColorUIResource"/>.
	/// It is impossible to use <seealso cref="DefaultPersistenceDelegate"/>
	/// because this class has special rule for serialization:
	/// it should be converted to <seealso cref="ColorUIResource"/>.
	/// </summary>
	/// <seealso cref= PrintColorUIResource#writeReplace
	/// 
	/// @author Sergey A. Malenkov </seealso>
	internal sealed class sun_swing_PrintColorUIResource_PersistenceDelegate : PersistenceDelegate
	{
		protected internal override bool MutatesTo(Object oldInstance, Object newInstance)
		{
			return oldInstance.Equals(newInstance);
		}

		protected internal override Expression Instantiate(Object oldInstance, Encoder @out)
		{
			Color color = (Color) oldInstance;
			Object[] args = new Object[] {color.RGB};
			return new Expression(color, typeof(ColorUIResource), "new", args);
		}
	}

		private static readonly Map<String, Field> Fields = Collections.SynchronizedMap(new WeakHashMap<String, Field>());
		private static Dictionary<String, PersistenceDelegate> InternalPersistenceDelegates = new Dictionary<String, PersistenceDelegate>();

		private static PersistenceDelegate NullPersistenceDelegate = new NullPersistenceDelegate();
		private static PersistenceDelegate EnumPersistenceDelegate = new EnumPersistenceDelegate();
		private static PersistenceDelegate PrimitivePersistenceDelegate = new PrimitivePersistenceDelegate();
		private static PersistenceDelegate DefaultPersistenceDelegate = new DefaultPersistenceDelegate();
		private static PersistenceDelegate ArrayPersistenceDelegate;
		private static PersistenceDelegate ProxyPersistenceDelegate;

		static MetaData()
		{

			InternalPersistenceDelegates.Put("java.net.URI", new PrimitivePersistenceDelegate());

			// it is possible because MatteBorder is assignable from MatteBorderUIResource
			InternalPersistenceDelegates.Put("javax.swing.plaf.BorderUIResource$MatteBorderUIResource", new javax_swing_border_MatteBorder_PersistenceDelegate());

			// it is possible because FontUIResource is supported by java_awt_Font_PersistenceDelegate
			InternalPersistenceDelegates.Put("javax.swing.plaf.FontUIResource", new java_awt_Font_PersistenceDelegate());

			// it is possible because KeyStroke is supported by java_awt_AWTKeyStroke_PersistenceDelegate
			InternalPersistenceDelegates.Put("javax.swing.KeyStroke", new java_awt_AWTKeyStroke_PersistenceDelegate());

			InternalPersistenceDelegates.Put("java.sql.Date", new java_util_Date_PersistenceDelegate());
			InternalPersistenceDelegates.Put("java.sql.Time", new java_util_Date_PersistenceDelegate());

			InternalPersistenceDelegates.Put("java.util.JumboEnumSet", new java_util_EnumSet_PersistenceDelegate());
			InternalPersistenceDelegates.Put("java.util.RegularEnumSet", new java_util_EnumSet_PersistenceDelegate());
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public synchronized static PersistenceDelegate getPersistenceDelegate(Class type)
		public static PersistenceDelegate GetPersistenceDelegate(Class type)
		{
			lock (typeof(MetaData))
			{
				if (type == null)
				{
					return NullPersistenceDelegate;
				}
				if (type.IsSubclassOf(typeof(Enum)))
				{
					return EnumPersistenceDelegate;
				}
				if (null != XMLEncoder.PrimitiveTypeFor(type))
				{
					return PrimitivePersistenceDelegate;
				}
				// The persistence delegate for arrays is non-trivial; instantiate it lazily.
				if (type.Array)
				{
					if (ArrayPersistenceDelegate == null)
					{
						ArrayPersistenceDelegate = new ArrayPersistenceDelegate();
					}
					return ArrayPersistenceDelegate;
				}
				// Handle proxies lazily for backward compatibility with 1.2.
				try
				{
					if (java.lang.reflect.Proxy.IsProxyClass(type))
					{
						if (ProxyPersistenceDelegate == null)
						{
							ProxyPersistenceDelegate = new ProxyPersistenceDelegate();
						}
						return ProxyPersistenceDelegate;
					}
				}
				catch (Exception)
				{
				}
				// else if (type.getDeclaringClass() != null) {
				//     return new DefaultPersistenceDelegate(new String[]{"this$0"});
				// }
        
				String typeName = type.Name;
				PersistenceDelegate pd = (PersistenceDelegate)GetBeanAttribute(type, "persistenceDelegate");
				if (pd == null)
				{
					pd = InternalPersistenceDelegates.Get(typeName);
					if (pd != null)
					{
						return pd;
					}
					InternalPersistenceDelegates.Put(typeName, DefaultPersistenceDelegate);
					try
					{
						String name = type.Name;
						Class c = Class.ForName("java.beans.MetaData$" + name.Replace('.', '_') + "_PersistenceDelegate");
						pd = (PersistenceDelegate)c.NewInstance();
						InternalPersistenceDelegates.Put(typeName, pd);
					}
					catch (ClassNotFoundException)
					{
						String[] properties = GetConstructorProperties(type);
						if (properties != null)
						{
							pd = new DefaultPersistenceDelegate(properties);
							InternalPersistenceDelegates.Put(typeName, pd);
						}
					}
					catch (Exception e)
					{
						System.Console.Error.WriteLine("Internal error: " + e);
					}
				}
        
				return (pd != null) ? pd : DefaultPersistenceDelegate;
			}
		}

		private static String[] GetConstructorProperties(Class type)
		{
			String[] names = null;
			int length = 0;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Constructor<?> constructor : type.getConstructors())
			foreach (Constructor<?> constructor in type.Constructors)
			{
				String[] value = GetAnnotationValue(constructor);
				if ((value != null) && (length < value.Length) && IsValid(constructor, value))
				{
					names = value;
					length = value.Length;
				}
			}
			return names;
		}

		private static String[] getAnnotationValue<T1>(Constructor<T1> constructor)
		{
			ConstructorProperties annotation = constructor.getAnnotation(typeof(ConstructorProperties));
			return (annotation != null) ? annotation.value() : null;
		}

		private static bool isValid<T1>(Constructor<T1> constructor, String[] names)
		{
			Class[] parameters = constructor.ParameterTypes;
			if (names.Length != parameters.Length)
			{
				return false;
			}
			foreach (String name in names)
			{
				if (name == null)
				{
					return false;
				}
			}
			return true;
		}

		private static Object GetBeanAttribute(Class type, String attribute)
		{
			try
			{
				return Introspector.GetBeanInfo(type).BeanDescriptor.GetValue(attribute);
			}
			catch (IntrospectionException)
			{
				return null;
			}
		}

		internal static Object GetPrivateFieldValue(Object instance, String name)
		{
			Field field = Fields.Get(name);
			if (field == null)
			{
				int index = name.LastIndexOf('.');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = name.substring(0, index);
				String className = name.Substring(0, index);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String fieldName = name.substring(1 + index);
				String fieldName = name.Substring(1 + index);
				field = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(field, className, fieldName));
				Fields.Put(name, field);
			}
			try
			{
				return field.get(instance);
			}
			catch (IllegalAccessException exception)
			{
				throw new IllegalStateException("Could not get value of the field", exception);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Field>
		{
			private Field Field;
			private string ClassName;
			private string FieldName;

			public PrivilegedActionAnonymousInnerClassHelper(Field field, string className, string fieldName)
			{
				this.Field = field;
				this.ClassName = className;
				this.FieldName = fieldName;
			}

			public virtual Field Run()
			{
				try
				{
					Field field = Class.ForName(ClassName).GetDeclaredField(FieldName);
					field.Accessible = true;
					return field;
				}
				catch (ClassNotFoundException exception)
				{
					throw new IllegalStateException("Could not find class", exception);
				}
				catch (NoSuchFieldException exception)
				{
					throw new IllegalStateException("Could not find field", exception);
				}
			}
		}
	}

}