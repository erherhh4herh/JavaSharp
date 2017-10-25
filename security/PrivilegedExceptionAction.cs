/*
 * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.security
{


	/// <summary>
	/// A computation to be performed with privileges enabled, that throws one or
	/// more checked exceptions.  The computation is performed by invoking
	/// {@code AccessController.doPrivileged} on the
	/// {@code PrivilegedExceptionAction} object.  This interface is
	/// used only for computations that throw checked exceptions;
	/// computations that do not throw
	/// checked exceptions should use {@code PrivilegedAction} instead.
	/// </summary>
	/// <seealso cref= AccessController </seealso>
	/// <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction) </seealso>
	/// <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction,
	///                                              AccessControlContext) </seealso>
	/// <seealso cref= PrivilegedAction </seealso>

	public interface PrivilegedExceptionAction<T>
	{
		/// <summary>
		/// Performs the computation.  This method will be called by
		/// {@code AccessController.doPrivileged} after enabling privileges.
		/// </summary>
		/// <returns> a class-dependent value that may represent the results of the
		///         computation.  Each class that implements
		///         {@code PrivilegedExceptionAction} should document what
		///         (if anything) this value represents. </returns>
		/// <exception cref="Exception"> an exceptional condition has occurred.  Each class
		///         that implements {@code PrivilegedExceptionAction} should
		///         document the exceptions that its run method can throw. </exception>
		/// <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction) </seealso>
		/// <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction,AccessControlContext) </seealso>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T run() throws Exception;
		T Run();
	}

}