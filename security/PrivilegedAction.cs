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
	/// A computation to be performed with privileges enabled.  The computation is
	/// performed by invoking {@code AccessController.doPrivileged} on the
	/// {@code PrivilegedAction} object.  This interface is used only for
	/// computations that do not throw checked exceptions; computations that
	/// throw checked exceptions must use {@code PrivilegedExceptionAction}
	/// instead.
	/// </summary>
	/// <seealso cref= AccessController </seealso>
	/// <seealso cref= AccessController#doPrivileged(PrivilegedAction) </seealso>
	/// <seealso cref= PrivilegedExceptionAction </seealso>

	public interface PrivilegedAction<T>
	{
		/// <summary>
		/// Performs the computation.  This method will be called by
		/// {@code AccessController.doPrivileged} after enabling privileges.
		/// </summary>
		/// <returns> a class-dependent value that may represent the results of the
		///         computation. Each class that implements
		///         {@code PrivilegedAction}
		///         should document what (if anything) this value represents. </returns>
		/// <seealso cref= AccessController#doPrivileged(PrivilegedAction) </seealso>
		/// <seealso cref= AccessController#doPrivileged(PrivilegedAction,
		///                                     AccessControlContext) </seealso>
		T Run();
	}

}