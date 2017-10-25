/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// <para> This interface represents a guard, which is an object that is used
	/// to protect access to another object.
	/// 
	/// </para>
	/// <para>This interface contains a single method, {@code checkGuard},
	/// with a single {@code object} argument. {@code checkGuard} is
	/// invoked (by the GuardedObject {@code getObject} method)
	/// to determine whether or not to allow access to the object.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= GuardedObject
	/// 
	/// @author Roland Schemers
	/// @author Li Gong </seealso>

	public interface Guard
	{

		/// <summary>
		/// Determines whether or not to allow access to the guarded object
		/// {@code object}. Returns silently if access is allowed.
		/// Otherwise, throws a SecurityException.
		/// </summary>
		/// <param name="object"> the object being protected by the guard.
		/// </param>
		/// <exception cref="SecurityException"> if access is denied.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void checkGuard(Object object) throws SecurityException;
		void CheckGuard(Object @object);
	}

}