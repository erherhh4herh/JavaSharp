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

namespace java.util
{

	/// <summary>
	/// An iterator over a collection.  {@code Iterator} takes the place of
	/// <seealso cref="Enumeration"/> in the Java Collections Framework.  Iterators
	/// differ from enumerations in two ways:
	/// 
	/// <ul>
	///      <li> Iterators allow the caller to remove elements from the
	///           underlying collection during the iteration with well-defined
	///           semantics.
	///      <li> Method names have been improved.
	/// </ul>
	/// 
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// </para>
	/// </summary>
	/// @param <E> the type of elements returned by this iterator
	/// 
	/// @author  Josh Bloch </param>
	/// <seealso cref= Collection </seealso>
	/// <seealso cref= ListIterator </seealso>
	/// <seealso cref= Iterable
	/// @since 1.2 </seealso>
	public interface Iterator<E>
	{
		/// <summary>
		/// Returns {@code true} if the iteration has more elements.
		/// (In other words, returns {@code true} if <seealso cref="#next"/> would
		/// return an element rather than throwing an exception.)
		/// </summary>
		/// <returns> {@code true} if the iteration has more elements </returns>
		bool HasNext();

		/// <summary>
		/// Returns the next element in the iteration.
		/// </summary>
		/// <returns> the next element in the iteration </returns>
		/// <exception cref="NoSuchElementException"> if the iteration has no more elements </exception>
		E Next();

		/// <summary>
		/// Removes from the underlying collection the last element returned
		/// by this iterator (optional operation).  This method can be called
		/// only once per call to <seealso cref="#next"/>.  The behavior of an iterator
		/// is unspecified if the underlying collection is modified while the
		/// iteration is in progress in any way other than by calling this
		/// method.
		/// 
		/// @implSpec
		/// The default implementation throws an instance of
		/// <seealso cref="UnsupportedOperationException"/> and performs no other action.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> if the {@code remove}
		///         operation is not supported by this iterator
		/// </exception>
		/// <exception cref="IllegalStateException"> if the {@code next} method has not
		///         yet been called, or the {@code remove} method has already
		///         been called after the last call to the {@code next}
		///         method </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void remove()
	//	{
	//		throw new UnsupportedOperationException("remove");
	//	}

		/// <summary>
		/// Performs the given action for each remaining element until all elements
		/// have been processed or the action throws an exception.  Actions are
		/// performed in the order of iteration, if that order is specified.
		/// Exceptions thrown by the action are relayed to the caller.
		/// 
		/// @implSpec
		/// <para>The default implementation behaves as if:
		/// <pre>{@code
		///     while (hasNext())
		///         action.accept(next());
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="action"> The action to be performed for each element </param>
		/// <exception cref="NullPointerException"> if the specified action is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default void forEachRemaining(java.util.function.Consumer<? base E> action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEachRemaining(java.util.function.Consumer<JavaToDotNetGenericWildcard> action)
	//	{
	//		Objects.requireNonNull(action);
	//		while (hasNext())
	//			action.accept(next());
	//	}
	}

}