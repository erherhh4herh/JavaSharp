using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.invoke
{

	using WrapperInstance = sun.invoke.WrapperInstance;
	using CallerSensitive = sun.reflect.CallerSensitive;
	using Reflection = sun.reflect.Reflection;
	using ReflectUtil = sun.reflect.misc.ReflectUtil;

	/// <summary>
	/// This class consists exclusively of static methods that help adapt
	/// method handles to other JVM types, such as interfaces.
	/// </summary>
	public class MethodHandleProxies
	{

		private MethodHandleProxies() // do not instantiate
		{
		}

		/// <summary>
		/// Produces an instance of the given single-method interface which redirects
		/// its calls to the given method handle.
		/// <para>
		/// A single-method interface is an interface which declares a uniquely named method.
		/// When determining the uniquely named method of a single-method interface,
		/// the public {@code Object} methods ({@code toString}, {@code equals}, {@code hashCode})
		/// are disregarded.  For example, <seealso cref="java.util.Comparator"/> is a single-method interface,
		/// even though it re-declares the {@code Object.equals} method.
		/// </para>
		/// <para>
		/// The interface must be public.  No additional access checks are performed.
		/// </para>
		/// <para>
		/// The resulting instance of the required type will respond to
		/// invocation of the type's uniquely named method by calling
		/// the given target on the incoming arguments,
		/// and returning or throwing whatever the target
		/// returns or throws.  The invocation will be as if by
		/// {@code target.invoke}.
		/// The target's type will be checked before the
		/// instance is created, as if by a call to {@code asType},
		/// which may result in a {@code WrongMethodTypeException}.
		/// </para>
		/// <para>
		/// The uniquely named method is allowed to be multiply declared,
		/// with distinct type descriptors.  (E.g., it can be overloaded,
		/// or can possess bridge methods.)  All such declarations are
		/// connected directly to the target method handle.
		/// Argument and return types are adjusted by {@code asType}
		/// for each individual declaration.
		/// </para>
		/// <para>
		/// The wrapper instance will implement the requested interface
		/// and its super-types, but no other single-method interfaces.
		/// This means that the instance will not unexpectedly
		/// pass an {@code instanceof} test for any unrequested type.
		/// <p style="font-size:smaller;">
		/// <em>Implementation Note:</em>
		/// Therefore, each instance must implement a unique single-method interface.
		/// Implementations may not bundle together
		/// multiple single-method interfaces onto single implementation classes
		/// in the style of <seealso cref="java.awt.AWTEventMulticaster"/>.
		/// </para>
		/// <para>
		/// The method handle may throw an <em>undeclared exception</em>,
		/// which means any checked exception (or other checked throwable)
		/// not declared by the requested type's single abstract method.
		/// If this happens, the throwable will be wrapped in an instance of
		/// <seealso cref="java.lang.reflect.UndeclaredThrowableException UndeclaredThrowableException"/>
		/// and thrown in that wrapped form.
		/// </para>
		/// <para>
		/// Like <seealso cref="java.lang.Integer#valueOf Integer.valueOf"/>,
		/// {@code asInterfaceInstance} is a factory method whose results are defined
		/// by their behavior.
		/// It is not guaranteed to return a new instance for every call.
		/// </para>
		/// <para>
		/// Because of the possibility of <seealso cref="java.lang.reflect.Method#isBridge bridge methods"/>
		/// and other corner cases, the interface may also have several abstract methods
		/// with the same name but having distinct descriptors (types of returns and parameters).
		/// In this case, all the methods are bound in common to the one given target.
		/// The type check and effective {@code asType} conversion is applied to each
		/// method type descriptor, and all abstract methods are bound to the target in common.
		/// Beyond this type check, no further checks are made to determine that the
		/// abstract methods are related in any way.
		/// </para>
		/// <para>
		/// Future versions of this API may accept additional types,
		/// such as abstract classes with single abstract methods.
		/// Future versions of this API may also equip wrapper instances
		/// with one or more additional public "marker" interfaces.
		/// </para>
		/// <para>
		/// If a security manager is installed, this method is caller sensitive.
		/// During any invocation of the target method handle via the returned wrapper,
		/// the original creator of the wrapper (the caller) will be visible
		/// to context checks requested by the security manager.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the desired type of the wrapper, a single-method interface </param>
		/// <param name="intfc"> a class object representing {@code T} </param>
		/// <param name="target"> the method handle to invoke from the wrapper </param>
		/// <returns> a correctly-typed wrapper for the given target </returns>
		/// <exception cref="NullPointerException"> if either argument is null </exception>
		/// <exception cref="IllegalArgumentException"> if the {@code intfc} is not a
		///         valid argument to this method </exception>
		/// <exception cref="WrongMethodTypeException"> if the target cannot
		///         be converted to the type required by the requested interface </exception>
		// Other notes to implementors:
		// <p>
		// No stable mapping is promised between the single-method interface and
		// the implementation class C.  Over time, several implementation
		// classes might be used for the same type.
		// <p>
		// If the implementation is able
		// to prove that a wrapper of the required type
		// has already been created for a given
		// method handle, or for another method handle with the
		// same behavior, the implementation may return that wrapper in place of
		// a new wrapper.
		// <p>
		// This method is designed to apply to common use cases
		// where a single method handle must interoperate with
		// an interface that implements a function-like
		// API.  Additional variations, such as single-abstract-method classes with
		// private constructors, or interfaces with multiple but related
		// entry points, must be covered by hand-written or automatically
		// generated adapter classes.
		//
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @CallerSensitive public static <T> T asInterfaceInstance(final Class intfc, final MethodHandle target)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static T asInterfaceInstance<T>(Class intfc, MethodHandle target)
		{
			if (!intfc.Interface || !Modifier.IsPublic(intfc.Modifiers))
			{
				throw newIllegalArgumentException("not a public interface", intfc.Name);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MethodHandle mh;
			MethodHandle mh;
			if (System.SecurityManager != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class caller = sun.reflect.Reflection.getCallerClass();
				Class caller = Reflection.CallerClass;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader ccl = caller != null ? caller.getClassLoader() : null;
				ClassLoader ccl = caller != null ? caller.ClassLoader : null;
				ReflectUtil.checkProxyPackageAccess(ccl, intfc);
				mh = ccl != null ? BindCaller(target, caller) : target;
			}
			else
			{
				mh = target;
			}
			ClassLoader proxyLoader = intfc.ClassLoader;
			if (proxyLoader == null)
			{
				ClassLoader cl = Thread.CurrentThread.ContextClassLoader; // avoid use of BCP
				proxyLoader = cl != null ? cl : ClassLoader.SystemClassLoader;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Method[] methods = getSingleNameMethods(intfc);
			Method[] methods = GetSingleNameMethods(intfc);
			if (methods == null)
			{
				throw newIllegalArgumentException("not a single-method interface", intfc.Name);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MethodHandle[] vaTargets = new MethodHandle[methods.length];
			MethodHandle[] vaTargets = new MethodHandle[methods.Length];
			for (int i = 0; i < methods.Length; i++)
			{
				Method sm = methods[i];
				MethodType smMT = MethodType.MethodType(sm.ReturnType, sm.ParameterTypes);
				MethodHandle checkTarget = mh.AsType(smMT); // make throw WMT
				checkTarget = checkTarget.AsType(checkTarget.Type().ChangeReturnType(typeof(Object)));
				vaTargets[i] = checkTarget.AsSpreader(typeof(Object[]), smMT.ParameterCount());
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final InvocationHandler ih = new InvocationHandler()
			InvocationHandler ih = new InvocationHandlerAnonymousInnerClassHelper(intfc, target, methods, vaTargets);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object proxy;
			Object proxy;
			if (System.SecurityManager != null)
			{
				// sun.invoke.WrapperInstance is a restricted interface not accessible
				// by any non-null class loader.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader loader = proxyLoader;
				ClassLoader loader = proxyLoader;
				proxy = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(intfc, ih, loader));
			}
			else
			{
				proxy = Proxy.NewProxyInstance(proxyLoader, new Class[]{intfc, typeof(WrapperInstance)}, ih);
			}
			return intfc.Cast(proxy);
		}

		private class InvocationHandlerAnonymousInnerClassHelper : InvocationHandler
		{
			private Type Intfc;
			private java.lang.invoke.MethodHandle Target;
			private System.Reflection.MethodInfo[] Methods;
			private java.lang.invoke.MethodHandle[] VaTargets;

			public InvocationHandlerAnonymousInnerClassHelper(Type intfc, java.lang.invoke.MethodHandle target, System.Reflection.MethodInfo[] methods, java.lang.invoke.MethodHandle[] vaTargets)
			{
				this.Intfc = intfc;
				this.Target = target;
				this.Methods = methods;
				this.VaTargets = vaTargets;
			}

			private Object GetArg(String name)
			{
				if ((Object)name == "getWrapperInstanceTarget")
				{
					return Target;
				}
				if ((Object)name == "getWrapperInstanceType")
				{
					return Intfc;
				}
				throw new AssertionError();
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object invoke(Object proxy, Method method, Object[] args) throws Throwable
			public virtual Object Invoke(Object proxy, Method method, Object[] args)
			{
				for (int i = 0; i < Methods.Length; i++)
				{
					if (method.Equals(Methods[i]))
					{
						return VaTargets[i].invokeExact(args);
					}
				}
				if (method.DeclaringClass == typeof(WrapperInstance))
				{
					return getArg(method.Name);
				}
				if (IsObjectMethod(method))
				{
					return CallObjectMethod(proxy, method, args);
				}
				throw newInternalError("bad proxy method: " + method);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			private Type Intfc;
			private java.lang.reflect.InvocationHandler Ih;
			private java.lang.ClassLoader Loader;

			public PrivilegedActionAnonymousInnerClassHelper(Type intfc, java.lang.reflect.InvocationHandler ih, java.lang.ClassLoader loader)
			{
				this.Intfc = intfc;
				this.Ih = ih;
				this.Loader = loader;
			}

			public virtual Object Run()
			{
				return Proxy.NewProxyInstance(Loader, new Class[]{Intfc, typeof(WrapperInstance)}, Ih);
			}
		}

		private static MethodHandle BindCaller(MethodHandle target, Class hostClass)
		{
			MethodHandle cbmh = MethodHandleImpl.BindCaller(target, hostClass);
			if (target.VarargsCollector)
			{
				MethodType type = cbmh.Type();
				int arity = type.ParameterCount();
				return cbmh.AsVarargsCollector(type.ParameterType(arity - 1));
			}
			return cbmh;
		}

		/// <summary>
		/// Determines if the given object was produced by a call to <seealso cref="#asInterfaceInstance asInterfaceInstance"/>. </summary>
		/// <param name="x"> any reference </param>
		/// <returns> true if the reference is not null and points to an object produced by {@code asInterfaceInstance} </returns>
		public static bool IsWrapperInstance(Object x)
		{
			return x is WrapperInstance;
		}

		private static WrapperInstance AsWrapperInstance(Object x)
		{
			try
			{
				if (x != null)
				{
					return (WrapperInstance) x;
				}
			}
			catch (ClassCastException)
			{
			}
			throw newIllegalArgumentException("not a wrapper instance");
		}

		/// <summary>
		/// Produces or recovers a target method handle which is behaviorally
		/// equivalent to the unique method of this wrapper instance.
		/// The object {@code x} must have been produced by a call to <seealso cref="#asInterfaceInstance asInterfaceInstance"/>.
		/// This requirement may be tested via <seealso cref="#isWrapperInstance isWrapperInstance"/>. </summary>
		/// <param name="x"> any reference </param>
		/// <returns> a method handle implementing the unique method </returns>
		/// <exception cref="IllegalArgumentException"> if the reference x is not to a wrapper instance </exception>
		public static MethodHandle WrapperInstanceTarget(Object x)
		{
			return AsWrapperInstance(x).WrapperInstanceTarget;
		}

		/// <summary>
		/// Recovers the unique single-method interface type for which this wrapper instance was created.
		/// The object {@code x} must have been produced by a call to <seealso cref="#asInterfaceInstance asInterfaceInstance"/>.
		/// This requirement may be tested via <seealso cref="#isWrapperInstance isWrapperInstance"/>. </summary>
		/// <param name="x"> any reference </param>
		/// <returns> the single-method interface type for which the wrapper was created </returns>
		/// <exception cref="IllegalArgumentException"> if the reference x is not to a wrapper instance </exception>
		public static Class WrapperInstanceType(Object x)
		{
			return AsWrapperInstance(x).WrapperInstanceType;
		}

		private static bool IsObjectMethod(Method m)
		{
			switch (m.Name)
			{
			case "toString":
				return (m.ReturnType == typeof(String) && m.ParameterTypes.Length == 0);
			case "hashCode":
				return (m.ReturnType == typeof(int) && m.ParameterTypes.Length == 0);
			case "equals":
				return (m.ReturnType == typeof(bool) && m.ParameterTypes.Length == 1 && m.ParameterTypes[0] == typeof(Object));
			}
			return false;
		}

		private static Object CallObjectMethod(Object self, Method m, Object[] args)
		{
			assert(IsObjectMethod(m)) : m;
			switch (m.Name)
			{
			case "toString":
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return self.GetType().FullName + "@" + self.HashCode().ToString("x");
			case "hashCode":
				return System.identityHashCode(self);
			case "equals":
				return (self == args[0]);
			}
			return null;
		}

		private static Method[] GetSingleNameMethods(Class intfc)
		{
			List<Method> methods = new List<Method>();
			String uniqueName = null;
			foreach (Method m in intfc.Methods)
			{
				if (IsObjectMethod(m))
				{
					continue;
				}
				if (!Modifier.IsAbstract(m.Modifiers))
				{
					continue;
				}
				String mname = m.Name;
				if (uniqueName == null)
				{
					uniqueName = mname;
				}
				else if (!uniqueName.Equals(mname))
				{
					return null; // too many abstract methods
				}
				methods.Add(m);
			}
			if (uniqueName == null)
			{
				return null;
			}
			return methods.ToArray();
		}
	}

}