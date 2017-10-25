using System;

/*
 * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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


	using ClassFinder = com.sun.beans.finder.ClassFinder;
	using ConstructorFinder = com.sun.beans.finder.ConstructorFinder;
	using MethodFinder = com.sun.beans.finder.MethodFinder;
	using MethodUtil = sun.reflect.misc.MethodUtil;

	/// <summary>
	/// A <code>Statement</code> object represents a primitive statement
	/// in which a single method is applied to a target and
	/// a set of arguments - as in <code>"a.setFoo(b)"</code>.
	/// Note that where this example uses names
	/// to denote the target and its argument, a statement
	/// object does not require a name space and is constructed with
	/// the values themselves.
	/// The statement object associates the named method
	/// with its environment as a simple set of values:
	/// the target and an array of argument values.
	/// 
	/// @since 1.4
	/// 
	/// @author Philip Milne
	/// </summary>
	public class Statement
	{

		private static Object[] EmptyArray = new Object[]{};

		internal static ExceptionListener defaultExceptionListener = new ExceptionListenerAnonymousInnerClassHelper();

		private class ExceptionListenerAnonymousInnerClassHelper : ExceptionListener
		{
			public ExceptionListenerAnonymousInnerClassHelper()
			{
			}

			public virtual void ExceptionThrown(Exception e)
			{
				System.Console.Error.WriteLine(e);
				// e.printStackTrace();
				System.Console.Error.WriteLine("Continuing ...");
			}
		}

		private readonly AccessControlContext Acc = AccessController.Context;
		private readonly Object Target_Renamed;
		private readonly String MethodName_Renamed;
		private readonly Object[] Arguments_Renamed;
		internal ClassLoader Loader;

		/// <summary>
		/// Creates a new <seealso cref="Statement"/> object
		/// for the specified target object to invoke the method
		/// specified by the name and by the array of arguments.
		/// <para>
		/// The {@code target} and the {@code methodName} values should not be {@code null}.
		/// Otherwise an attempt to execute this {@code Expression}
		/// will result in a {@code NullPointerException}.
		/// If the {@code arguments} value is {@code null},
		/// an empty array is used as the value of the {@code arguments} property.
		/// 
		/// </para>
		/// </summary>
		/// <param name="target">  the target object of this statement </param>
		/// <param name="methodName">  the name of the method to invoke on the specified target </param>
		/// <param name="arguments">  the array of arguments to invoke the specified method </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({"target", "methodName", "arguments"}) public Statement(Object target, String methodName, Object[] arguments)
		public Statement(Object target, String methodName, Object[] arguments)
		{
			this.Target_Renamed = target;
			this.MethodName_Renamed = methodName;
			this.Arguments_Renamed = (arguments == null) ? EmptyArray : arguments.clone();
		}

		/// <summary>
		/// Returns the target object of this statement.
		/// If this method returns {@code null},
		/// the <seealso cref="#execute"/> method
		/// throws a {@code NullPointerException}.
		/// </summary>
		/// <returns> the target object of this statement </returns>
		public virtual Object Target
		{
			get
			{
				return Target_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the method to invoke.
		/// If this method returns {@code null},
		/// the <seealso cref="#execute"/> method
		/// throws a {@code NullPointerException}.
		/// </summary>
		/// <returns> the name of the method </returns>
		public virtual String MethodName
		{
			get
			{
				return MethodName_Renamed;
			}
		}

		/// <summary>
		/// Returns the arguments for the method to invoke.
		/// The number of arguments and their types
		/// must match the method being  called.
		/// {@code null} can be used as a synonym of an empty array.
		/// </summary>
		/// <returns> the array of arguments </returns>
		public virtual Object[] Arguments
		{
			get
			{
				return this.Arguments_Renamed.clone();
			}
		}

		/// <summary>
		/// The {@code execute} method finds a method whose name is the same
		/// as the {@code methodName} property, and invokes the method on
		/// the target.
		/// 
		/// When the target's class defines many methods with the given name
		/// the implementation should choose the most specific method using
		/// the algorithm specified in the Java Language Specification
		/// (15.11). The dynamic class of the target and arguments are used
		/// in place of the compile-time type information and, like the
		/// <seealso cref="java.lang.reflect.Method"/> class itself, conversion between
		/// primitive values and their associated wrapper classes is handled
		/// internally.
		/// <para>
		/// The following method types are handled as special cases:
		/// <ul>
		/// <li>
		/// Static methods may be called by using a class object as the target.
		/// <li>
		/// The reserved method name "new" may be used to call a class's constructor
		/// as if all classes defined static "new" methods. Constructor invocations
		/// are typically considered {@code Expression}s rather than {@code Statement}s
		/// as they return a value.
		/// <li>
		/// The method names "get" and "set" defined in the <seealso cref="java.util.List"/>
		/// interface may also be applied to array instances, mapping to
		/// the static methods of the same name in the {@code Array} class.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if the value of the {@code target} or
		///                              {@code methodName} property is {@code null} </exception>
		/// <exception cref="NoSuchMethodException"> if a matching method is not found </exception>
		/// <exception cref="SecurityException"> if a security manager exists and
		///                           it denies the method invocation </exception>
		/// <exception cref="Exception"> that is thrown by the invoked method
		/// </exception>
		/// <seealso cref= java.lang.reflect.Method </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute() throws Exception
		public virtual void Execute()
		{
			Invoke();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object invoke() throws Exception
		internal virtual Object Invoke()
		{
			AccessControlContext acc = this.Acc;
			if ((acc == null) && (System.SecurityManager != null))
			{
				throw new SecurityException("AccessControlContext is not set");
			}
			try
			{
				return AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClassHelper(this),
						acc);
			}
			catch (PrivilegedActionException exception)
			{
				throw exception.Exception;
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : PrivilegedExceptionAction<Object>
		{
			private readonly Statement OuterInstance;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(Statement outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object run() throws Exception
			public virtual Object Run()
			{
				return outerInstance.InvokeInternal();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object invokeInternal() throws Exception
		private Object InvokeInternal()
		{
			Object target = Target;
			String methodName = MethodName;

			if (target == null || methodName == null)
			{
				throw new NullPointerException((target == null ? "target" : "methodName") + " should not be null");
			}

			Object[] arguments = Arguments;
			if (arguments == null)
			{
				arguments = EmptyArray;
			}
			// Class.forName() won't load classes outside
			// of core from a class inside core. Special
			// case this method.
			if (target == typeof(Class) && methodName.Equals("forName"))
			{
				return ClassFinder.resolveClass((String)arguments[0], this.Loader);
			}
			Class[] argClasses = new Class[arguments.Length];
			for (int i = 0; i < arguments.Length; i++)
			{
				argClasses[i] = (arguments[i] == null) ? null : arguments[i].GetType();
			}

			AccessibleObject m = null;
			if (target is Class)
			{
				/*
				For class methods, simluate the effect of a meta class
				by taking the union of the static methods of the
				actual class, with the instance methods of "Class.class"
				and the overloaded "newInstance" methods defined by the
				constructors.
				This way "System.class", for example, will perform both
				the static method getProperties() and the instance method
				getSuperclass() defined in "Class.class".
				*/
				if (methodName.Equals("new"))
				{
					methodName = "newInstance";
				}
				// Provide a short form for array instantiation by faking an nary-constructor.
				if (methodName.Equals("newInstance") && ((Class)target).Array)
				{
					Object result = Array.newInstance(((Class)target).ComponentType, arguments.Length);
					for (int i = 0; i < arguments.Length; i++)
					{
						Array.set(result, i, arguments[i]);
					}
					return result;
				}
				if (methodName.Equals("newInstance") && arguments.Length != 0)
				{
					// The Character class, as of 1.4, does not have a constructor
					// which takes a String. All of the other "wrapper" classes
					// for Java's primitive types have a String constructor so we
					// fake such a constructor here so that this special case can be
					// ignored elsewhere.
					if (target == typeof(Character) && arguments.Length == 1 && argClasses[0] == typeof(String))
					{
						return new Character(((String)arguments[0]).CharAt(0));
					}
					try
					{
						m = ConstructorFinder.findConstructor((Class)target, argClasses);
					}
					catch (NoSuchMethodException)
					{
						m = null;
					}
				}
				if (m == null && target != typeof(Class))
				{
					m = GetMethod((Class)target, methodName, argClasses);
				}
				if (m == null)
				{
					m = GetMethod(typeof(Class), methodName, argClasses);
				}
			}
			else
			{
				/*
				This special casing of arrays is not necessary, but makes files
				involving arrays much shorter and simplifies the archiving infrastrcure.
				The Array.set() method introduces an unusual idea - that of a static method
				changing the state of an instance. Normally statements with side
				effects on objects are instance methods of the objects themselves
				and we reinstate this rule (perhaps temporarily) by special-casing arrays.
				*/
				if (target.GetType().IsArray && (methodName.Equals("set") || methodName.Equals("get")))
				{
					int index = ((Integer)arguments[0]).IntValue();
					if (methodName.Equals("get"))
					{
						return Array.get(target, index);
					}
					else
					{
						Array.set(target, index, arguments[1]);
						return null;
					}
				}
				m = GetMethod(target.GetType(), methodName, argClasses);
			}
			if (m != null)
			{
				try
				{
					if (m is Method)
					{
						return MethodUtil.invoke((Method)m, target, arguments);
					}
					else
					{
						return ((Constructor)m).newInstance(arguments);
					}
				}
				catch (IllegalAccessException iae)
				{
					throw new Exception("Statement cannot invoke: " + methodName + " on " + target.GetType(), iae);
				}
				catch (InvocationTargetException ite)
				{
					Throwable te = ite.TargetException;
					if (te is Exception)
					{
						throw (Exception)te;
					}
					else
					{
						throw ite;
					}
				}
			}
			throw new NoSuchMethodException(ToString());
		}

		internal virtual String InstanceName(Object instance)
		{
			if (instance == null)
			{
				return "null";
			}
			else if (instance.GetType() == typeof(String))
			{
				return "\"" + (String)instance + "\"";
			}
			else
			{
				// Note: there is a minor problem with using the non-caching
				// NameGenerator method. The return value will not have
				// specific information about the inner class name. For example,
				// In 1.4.2 an inner class would be represented as JList$1 now
				// would be named Class.

				return NameGenerator.UnqualifiedClassName(instance.GetType());
			}
		}

		/// <summary>
		/// Prints the value of this statement using a Java-style syntax.
		/// </summary>
		public override String ToString()
		{
			// Respect a subclass's implementation here.
			Object target = Target;
			String methodName = MethodName;
			Object[] arguments = Arguments;
			if (arguments == null)
			{
				arguments = EmptyArray;
			}
			StringBuffer result = new StringBuffer(InstanceName(target) + "." + methodName + "(");
			int n = arguments.Length;
			for (int i = 0; i < n; i++)
			{
				result.Append(InstanceName(arguments[i]));
				if (i != n - 1)
				{
					result.Append(", ");
				}
			}
			result.Append(");");
			return result.ToString();
		}

		internal static Method GetMethod(Class type, String name, params Class[] args)
		{
			try
			{
				return MethodFinder.findMethod(type, name, args);
			}
			catch (NoSuchMethodException)
			{
				return null;
			}
		}
	}

}