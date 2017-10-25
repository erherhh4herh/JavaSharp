/*
 * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Represents an operation that accepts a single input argument and returns no
	/// result. Unlike most other functional interfaces, {@code Consumer} is expected
	/// to operate via side-effects.
	/// 
	/// <para>This is a <a href="package-summary.html">functional interface</a>
	/// whose functional method is <seealso cref="#accept(Object)"/>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of the input to the operation
	/// 
	/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface Consumer<T>
	public interface Consumer<T>
	{

		/// <summary>
		/// Performs this operation on the given argument.
		/// </summary>
		/// <param name="t"> the input argument </param>
		void Accept(T t);

		/// <summary>
		/// Returns a composed {@code Consumer} that performs, in sequence, this
		/// operation followed by the {@code after} operation. If performing either
		/// operation throws an exception, it is relayed to the caller of the
		/// composed operation.  If performing this operation throws an exception,
		/// the {@code after} operation will not be performed.
		/// </summary>
		/// <param name="after"> the operation to perform after this operation </param>
		/// <returns> a composed {@code Consumer} that performs in sequence this
		/// operation followed by the {@code after} operation </returns>
		/// <exception cref="NullPointerException"> if {@code after} is null </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default Consumer<T> andThen(Consumer<? base T> after)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Consumer<T> andThen(Consumer<JavaToDotNetGenericWildcard> after)
	//	{
	//		Objects.requireNonNull(after);
	//		return (T t) ->
	//		{
	//			accept(t);
	//			after.accept(t);
	//		};
	//	}
	}

}