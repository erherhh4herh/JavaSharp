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
	/// Represents an operation that accepts two input arguments and returns no
	/// result.  This is the two-arity specialization of <seealso cref="Consumer"/>.
	/// Unlike most other functional interfaces, {@code BiConsumer} is expected
	/// to operate via side-effects.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#accept(Object, Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the first argument to the operation </param>
	/// @param <U> the type of the second argument to the operation
	/// </param>
	/// <seealso cref= Consumer
	/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface BiConsumer<T, U>
	public interface BiConsumer<T, U>
	{

		/// <summary>
		/// Performs this operation on the given arguments.
		/// </summary>
		/// <param name="t"> the first input argument </param>
		/// <param name="u"> the second input argument </param>
		void Accept(T t, U u);

		/// <summary>
		/// Returns a composed {@code BiConsumer} that performs, in sequence, this
		/// operation followed by the {@code after} operation. If performing either
		/// operation throws an exception, it is relayed to the caller of the
		/// composed operation.  If performing this operation throws an exception,
		/// the {@code after} operation will not be performed.
		/// </summary>
		/// <param name="after"> the operation to perform after this operation </param>
		/// <returns> a composed {@code BiConsumer} that performs in sequence this
		/// operation followed by the {@code after} operation </returns>
		/// <exception cref="NullPointerException"> if {@code after} is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default BiConsumer<T, U> andThen(BiConsumer<? base T, ? base U> after)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default BiConsumer<T, U> andThen(BiConsumer<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> after)
	//	{
	//		Objects.requireNonNull(after);
	//
	//		return (l, r) ->
	//		{
	//			accept(l, r);
	//			after.accept(l, r);
	//		};
	//	}
	}

}