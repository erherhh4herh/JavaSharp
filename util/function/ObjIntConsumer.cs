/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.util.function
{

	/// <summary>
	/// Represents an operation that accepts an object-valued and a
	/// {@code int}-valued argument, and returns no result.  This is the
	/// {@code (reference, int)} specialization of <seealso cref="BiConsumer"/>.
	/// Unlike most other functional interfaces, {@code ObjIntConsumer} is
	/// expected to operate via side-effects.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#accept(Object, int)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the object argument to the operation
	/// </param>
	/// <seealso cref= BiConsumer
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface ObjIntConsumer<T>
	public interface ObjIntConsumer<T>
	{

		/// <summary>
		/// Performs this operation on the given arguments.
		/// </summary>
		/// <param name="t"> the first input argument </param>
		/// <param name="value"> the second input argument </param>
		void Accept(T t, int value);
	}

}