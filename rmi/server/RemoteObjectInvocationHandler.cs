using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2015, Oracle and/or its affiliates. All rights reserved.
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
namespace java.rmi.server
{

	using Util = sun.rmi.server.Util;
	using WeakClassHashMap = sun.rmi.server.WeakClassHashMap;

	/// <summary>
	/// An implementation of the <code>InvocationHandler</code> interface for
	/// use with Java Remote Method Invocation (Java RMI).  This invocation
	/// handler can be used in conjunction with a dynamic proxy instance as a
	/// replacement for a pregenerated stub class.
	/// 
	/// <para>Applications are not expected to use this class directly.  A remote
	/// object exported to use a dynamic proxy with <seealso cref="UnicastRemoteObject"/>
	/// or <seealso cref="Activatable"/> has an instance of this class as that proxy's
	/// invocation handler.
	/// 
	/// @author  Ann Wollrath
	/// @since   1.5
	/// 
	/// </para>
	/// </summary>
	public class RemoteObjectInvocationHandler : RemoteObject, InvocationHandler
	{
		private const long SerialVersionUID = 2L;

		// set to true if invocation handler allows finalize method (legacy behavior)
		private static readonly bool AllowFinalizeInvocation;

		static RemoteObjectInvocationHandler()
		{
			String propName = "sun.rmi.server.invocationhandler.allowFinalizeInvocation";
			String allowProp = java.security.AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(propName));
			if ("".Equals(allowProp))
			{
				AllowFinalizeInvocation = true;
			}
			else
			{
				AllowFinalizeInvocation = Convert.ToBoolean(allowProp);
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<String>
		{
			private string PropName;

			public PrivilegedActionAnonymousInnerClassHelper(string propName)
			{
				this.PropName = propName;
			}

			public virtual String Run()
			{
				return System.getProperty(PropName);
			}
		}

		/// <summary>
		/// A weak hash map, mapping classes to weak hash maps that map
		/// method objects to method hashes.
		/// 
		/// </summary>
		private static readonly MethodToHash_Maps MethodToHash_Maps = new MethodToHash_Maps();

		/// <summary>
		/// Creates a new <code>RemoteObjectInvocationHandler</code> constructed
		/// with the specified <code>RemoteRef</code>.
		/// </summary>
		/// <param name="ref"> the remote ref
		/// </param>
		/// <exception cref="NullPointerException"> if <code>ref</code> is <code>null</code>
		///  </exception>
		public RemoteObjectInvocationHandler(RemoteRef @ref) : base(@ref)
		{
			if (@ref == null)
			{
				throw new NullPointerException();
			}
		}

		/// <summary>
		/// Processes a method invocation made on the encapsulating
		/// proxy instance, <code>proxy</code>, and returns the result.
		/// 
		/// <para><code>RemoteObjectInvocationHandler</code> implements this method
		/// as follows:
		/// 
		/// </para>
		/// <para>If <code>method</code> is one of the following methods, it
		/// is processed as described below:
		/// 
		/// <ul>
		/// 
		/// <li><seealso cref="Object#hashCode Object.hashCode"/>: Returns the hash
		/// code value for the proxy.
		/// 
		/// <li><seealso cref="Object#equals Object.equals"/>: Returns <code>true</code>
		/// if the argument (<code>args[0]</code>) is an instance of a dynamic
		/// proxy class and this invocation handler is equal to the invocation
		/// handler of that argument, and returns <code>false</code> otherwise.
		/// 
		/// <li><seealso cref="Object#toString Object.toString"/>: Returns a string
		/// representation of the proxy.
		/// </ul>
		/// 
		/// </para>
		/// <para>Otherwise, a remote call is made as follows:
		/// 
		/// <ul>
		/// <li>If <code>proxy</code> is not an instance of the interface
		/// <seealso cref="Remote"/>, then an <seealso cref="IllegalArgumentException"/> is thrown.
		/// 
		/// <li>Otherwise, the <seealso cref="RemoteRef#invoke invoke"/> method is invoked
		/// on this invocation handler's <code>RemoteRef</code>, passing
		/// <code>proxy</code>, <code>method</code>, <code>args</code>, and the
		/// method hash (defined in section 8.3 of the "Java Remote Method
		/// Invocation (RMI) Specification") for <code>method</code>, and the
		/// result is returned.
		/// 
		/// <li>If an exception is thrown by <code>RemoteRef.invoke</code> and
		/// that exception is a checked exception that is not assignable to any
		/// exception in the <code>throws</code> clause of the method
		/// implemented by the <code>proxy</code>'s class, then that exception
		/// is wrapped in an <seealso cref="UnexpectedException"/> and the wrapped
		/// exception is thrown.  Otherwise, the exception thrown by
		/// <code>invoke</code> is thrown by this method.
		/// </ul>
		/// 
		/// </para>
		/// <para>The semantics of this method are unspecified if the
		/// arguments could not have been produced by an instance of some
		/// valid dynamic proxy class containing this invocation handler.
		/// 
		/// </para>
		/// </summary>
		/// <param name="proxy"> the proxy instance that the method was invoked on </param>
		/// <param name="method"> the <code>Method</code> instance corresponding to the
		/// interface method invoked on the proxy instance </param>
		/// <param name="args"> an array of objects containing the values of the
		/// arguments passed in the method invocation on the proxy instance, or
		/// <code>null</code> if the method takes no arguments </param>
		/// <returns> the value to return from the method invocation on the proxy
		/// instance </returns>
		/// <exception cref="Throwable"> the exception to throw from the method invocation
		/// on the proxy instance
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object invoke(Object proxy, Method method, Object[] args) throws Throwable
		public virtual Object Invoke(Object proxy, Method method, Object[] args)
		{
			if (!Proxy.isProxyClass(proxy.GetType()))
			{
				throw new IllegalArgumentException("not a proxy");
			}

			if (Proxy.getInvocationHandler(proxy) != this)
			{
				throw new IllegalArgumentException("handler mismatch");
			}

			if (method.DeclaringClass == typeof(Object))
			{
				return InvokeObjectMethod(proxy, method, args);
			}
			else if ("finalize".Equals(method.Name) && method.ParameterCount == 0 && !AllowFinalizeInvocation)
			{
				return null; // ignore
			}
			else
			{
				return InvokeRemoteMethod(proxy, method, args);
			}
		}

		/// <summary>
		/// Handles java.lang.Object methods.
		/// 
		/// </summary>
		private Object InvokeObjectMethod(Object proxy, Method method, Object[] args)
		{
			String name = method.Name;

			if (name.Equals("hashCode"))
			{
				return HashCode();

			}
			else if (name.Equals("equals"))
			{
				Object obj = args[0];
				InvocationHandler hdlr;
				return proxy == obj || (obj != null && Proxy.isProxyClass(obj.GetType()) && (hdlr = Proxy.getInvocationHandler(obj)) is RemoteObjectInvocationHandler && this.Equals(hdlr));

			}
			else if (name.Equals("toString"))
			{
				return ProxyToString(proxy);

			}
			else
			{
				throw new IllegalArgumentException("unexpected Object method: " + method);
			}
		}

		/// <summary>
		/// Handles remote methods.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object invokeRemoteMethod(Object proxy, Method method, Object[] args) throws Exception
		private Object InvokeRemoteMethod(Object proxy, Method method, Object[] args)
		{
			try
			{
				if (!(proxy is Remote))
				{
					throw new IllegalArgumentException("proxy not Remote instance");
				}
				return @ref.Invoke((Remote) proxy, method, args, GetMethodHash(method));
			}
			catch (Exception e)
			{
				if (!(e is RuntimeException))
				{
					Class cl = proxy.GetType();
					try
					{
						method = cl.GetMethod(method.Name, method.ParameterTypes);
					}
					catch (NoSuchMethodException nsme)
					{
						throw (IllegalArgumentException) (new IllegalArgumentException()).InitCause(nsme);
					}
					Class thrownType = e.GetType();
					foreach (Class declaredType in method.ExceptionTypes)
					{
						if (thrownType.IsSubclassOf(declaredType))
						{
							throw e;
						}
					}
					e = new UnexpectedException("unexpected exception", e);
				}
				throw e;
			}
		}

		/// <summary>
		/// Returns a string representation for a proxy that uses this invocation
		/// handler.
		/// 
		/// </summary>
		private String ProxyToString(Object proxy)
		{
			Class[] interfaces = proxy.GetType().GetInterfaces();
			if (interfaces.Length == 0)
			{
				return "Proxy[" + this + "]";
			}
			String iface = interfaces[0].Name;
			if (iface.Equals("java.rmi.Remote") && interfaces.Length > 1)
			{
				iface = interfaces[1].Name;
			}
			int dot = iface.LastIndexOf('.');
			if (dot >= 0)
			{
				iface = iface.Substring(dot + 1);
			}
			return "Proxy[" + iface + "," + this + "]";
		}

		/// <exception cref="InvalidObjectException"> unconditionally
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObjectNoData() throws java.io.InvalidObjectException
		private void ReadObjectNoData()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new InvalidObjectException("no data in stream; class: " + this.GetType().FullName);
		}

		/// <summary>
		/// Returns the method hash for the specified method.  Subsequent calls
		/// to "getMethodHash" passing the same method argument should be faster
		/// since this method caches internally the result of the method to
		/// method hash mapping.  The method hash is calculated using the
		/// "computeMethodHash" method.
		/// </summary>
		/// <param name="method"> the remote method </param>
		/// <returns> the method hash for the specified method </returns>
		private static long GetMethodHash(Method method)
		{
			return MethodToHash_Maps.get(method.DeclaringClass).get(method);
		}

		/// <summary>
		/// A weak hash map, mapping classes to weak hash maps that map
		/// method objects to method hashes.
		/// 
		/// </summary>
		private class MethodToHash_Maps : WeakClassHashMap<IDictionary<Method, Long>>
		{
			internal MethodToHash_Maps()
			{
			}

			protected internal virtual IDictionary<Method, Long> ComputeValue(Class remoteClass)
			{
				return new WeakHashMapAnonymousInnerClassHelper(this);
			}

			private class WeakHashMapAnonymousInnerClassHelper : WeakHashMap<Method, Long>
			{
				private readonly MethodToHash_Maps OuterInstance;

				public WeakHashMapAnonymousInnerClassHelper(MethodToHash_Maps outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public virtual Long Get(Object key)
				{
					lock (this)
					{
						Long hash = base.get(key);
						if (hash == java.util.Map_Fields.Null)
						{
							Method method = (Method) key;
							hash = Util.computeMethodHash(method);
							put(method, hash);
						}
						return hash;
					}
				}
			}
		}
	}

}