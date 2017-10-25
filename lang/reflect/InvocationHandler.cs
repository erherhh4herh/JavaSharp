/*
 * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.reflect
{

	/// <summary>
	/// {@code InvocationHandler} is the interface implemented by
	/// the <i>invocation handler</i> of a proxy instance.
	/// 
	/// <para>Each proxy instance has an associated invocation handler.
	/// When a method is invoked on a proxy instance, the method
	/// invocation is encoded and dispatched to the {@code invoke}
	/// method of its invocation handler.
	/// 
	/// @author      Peter Jones
	/// </para>
	/// </summary>
	/// <seealso cref=         Proxy
	/// @since       1.3 </seealso>
	public interface InvocationHandler
	{

		/// <summary>
		/// Processes a method invocation on a proxy instance and returns
		/// the result.  This method will be invoked on an invocation handler
		/// when a method is invoked on a proxy instance that it is
		/// associated with.
		/// </summary>
		/// <param name="proxy"> the proxy instance that the method was invoked on
		/// </param>
		/// <param name="method"> the {@code Method} instance corresponding to
		/// the interface method invoked on the proxy instance.  The declaring
		/// class of the {@code Method} object will be the interface that
		/// the method was declared in, which may be a superinterface of the
		/// proxy interface that the proxy class inherits the method through.
		/// </param>
		/// <param name="args"> an array of objects containing the values of the
		/// arguments passed in the method invocation on the proxy instance,
		/// or {@code null} if interface method takes no arguments.
		/// Arguments of primitive types are wrapped in instances of the
		/// appropriate primitive wrapper class, such as
		/// {@code java.lang.Integer} or {@code java.lang.Boolean}.
		/// </param>
		/// <returns>  the value to return from the method invocation on the
		/// proxy instance.  If the declared return type of the interface
		/// method is a primitive type, then the value returned by
		/// this method must be an instance of the corresponding primitive
		/// wrapper class; otherwise, it must be a type assignable to the
		/// declared return type.  If the value returned by this method is
		/// {@code null} and the interface method's return type is
		/// primitive, then a {@code NullPointerException} will be
		/// thrown by the method invocation on the proxy instance.  If the
		/// value returned by this method is otherwise not compatible with
		/// the interface method's declared return type as described above,
		/// a {@code ClassCastException} will be thrown by the method
		/// invocation on the proxy instance.
		/// </returns>
		/// <exception cref="Throwable"> the exception to throw from the method
		/// invocation on the proxy instance.  The exception's type must be
		/// assignable either to any of the exception types declared in the
		/// {@code throws} clause of the interface method or to the
		/// unchecked exception types {@code java.lang.RuntimeException}
		/// or {@code java.lang.Error}.  If a checked exception is
		/// thrown by this method that is not assignable to any of the
		/// exception types declared in the {@code throws} clause of
		/// the interface method, then an
		/// <seealso cref="UndeclaredThrowableException"/> containing the
		/// exception that was thrown by this method will be thrown by the
		/// method invocation on the proxy instance.
		/// </exception>
		/// <seealso cref=     UndeclaredThrowableException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object invoke(Object proxy, Method method, Object[] args) throws Throwable;
		Object Invoke(Object proxy, Method method, Object[] args);
	}

}