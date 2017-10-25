/*
 * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// A comparison function, which imposes a <i>total ordering</i> on some
	/// collection of objects.  Comparators can be passed to a sort method (such
	/// as <seealso cref="Collections#sort(List,Comparator) Collections.sort"/> or {@link
	/// Arrays#sort(Object[],Comparator) Arrays.sort}) to allow precise control
	/// over the sort order.  Comparators can also be used to control the order of
	/// certain data structures (such as <seealso cref="SortedSet sorted sets"/> or {@link
	/// SortedMap sorted maps}), or to provide an ordering for collections of
	/// objects that don't have a <seealso cref="Comparable natural ordering"/>.<para>
	/// 
	/// The ordering imposed by a comparator <tt>c</tt> on a set of elements
	/// <tt>S</tt> is said to be <i>consistent with equals</i> if and only if
	/// <tt>c.compare(e1, e2)==0</tt> has the same boolean value as
	/// <tt>e1.equals(e2)</tt> for every <tt>e1</tt> and <tt>e2</tt> in
	/// </para>
	/// <tt>S</tt>.<para>
	/// 
	/// Caution should be exercised when using a comparator capable of imposing an
	/// ordering inconsistent with equals to order a sorted set (or sorted map).
	/// Suppose a sorted set (or sorted map) with an explicit comparator <tt>c</tt>
	/// is used with elements (or keys) drawn from a set <tt>S</tt>.  If the
	/// ordering imposed by <tt>c</tt> on <tt>S</tt> is inconsistent with equals,
	/// the sorted set (or sorted map) will behave "strangely."  In particular the
	/// sorted set (or sorted map) will violate the general contract for set (or
	/// </para>
	/// map), which is defined in terms of <tt>equals</tt>.<para>
	/// 
	/// For example, suppose one adds two elements {@code a} and {@code b} such that
	/// {@code (a.equals(b) && c.compare(a, b) != 0)}
	/// to an empty {@code TreeSet} with comparator {@code c}.
	/// The second {@code add} operation will return
	/// true (and the size of the tree set will increase) because {@code a} and
	/// {@code b} are not equivalent from the tree set's perspective, even though
	/// this is contrary to the specification of the
	/// </para>
	/// <seealso cref="Set#add Set.add"/> method.<para>
	/// 
	/// Note: It is generally a good idea for comparators to also implement
	/// <tt>java.io.Serializable</tt>, as they may be used as ordering methods in
	/// serializable data structures (like <seealso cref="TreeSet"/>, <seealso cref="TreeMap"/>).  In
	/// order for the data structure to serialize successfully, the comparator (if
	/// </para>
	/// provided) must implement <tt>Serializable</tt>.<para>
	/// 
	/// For the mathematically inclined, the <i>relation</i> that defines the
	/// <i>imposed ordering</i> that a given comparator <tt>c</tt> imposes on a
	/// given set of objects <tt>S</tt> is:<pre>
	///       {(x, y) such that c.compare(x, y) &lt;= 0}.
	/// </pre> The <i>quotient</i> for this total order is:<pre>
	///       {(x, y) such that c.compare(x, y) == 0}.
	/// </pre>
	/// 
	/// It follows immediately from the contract for <tt>compare</tt> that the
	/// quotient is an <i>equivalence relation</i> on <tt>S</tt>, and that the
	/// imposed ordering is a <i>total order</i> on <tt>S</tt>.  When we say that
	/// the ordering imposed by <tt>c</tt> on <tt>S</tt> is <i>consistent with
	/// equals</i>, we mean that the quotient for the ordering is the equivalence
	/// relation defined by the objects' {@link Object#equals(Object)
	/// equals(Object)} method(s):<pre>
	///     {(x, y) such that x.equals(y)}. </pre>
	/// 
	/// </para>
	/// <para>Unlike {@code Comparable}, a comparator may optionally permit
	/// comparison of null arguments, while maintaining the requirements for
	/// an equivalence relation.
	/// 
	/// </para>
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of objects that may be compared by this comparator
	/// 
	/// @author  Josh Bloch
	/// @author  Neal Gafter </param>
	/// <seealso cref= Comparable </seealso>
	/// <seealso cref= java.io.Serializable
	/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @FunctionalInterface public interface Comparator<T>
	public interface Comparator<T>
	{
		/// <summary>
		/// Compares its two arguments for order.  Returns a negative integer,
		/// zero, or a positive integer as the first argument is less than, equal
		/// to, or greater than the second.<para>
		/// 
		/// In the foregoing description, the notation
		/// <tt>sgn(</tt><i>expression</i><tt>)</tt> designates the mathematical
		/// <i>signum</i> function, which is defined to return one of <tt>-1</tt>,
		/// <tt>0</tt>, or <tt>1</tt> according to whether the value of
		/// </para>
		/// <i>expression</i> is negative, zero or positive.<para>
		/// 
		/// The implementor must ensure that <tt>sgn(compare(x, y)) ==
		/// -sgn(compare(y, x))</tt> for all <tt>x</tt> and <tt>y</tt>.  (This
		/// implies that <tt>compare(x, y)</tt> must throw an exception if and only
		/// </para>
		/// if <tt>compare(y, x)</tt> throws an exception.)<para>
		/// 
		/// The implementor must also ensure that the relation is transitive:
		/// <tt>((compare(x, y)&gt;0) &amp;&amp; (compare(y, z)&gt;0))</tt> implies
		/// </para>
		/// <tt>compare(x, z)&gt;0</tt>.<para>
		/// 
		/// Finally, the implementor must ensure that <tt>compare(x, y)==0</tt>
		/// implies that <tt>sgn(compare(x, z))==sgn(compare(y, z))</tt> for all
		/// </para>
		/// <tt>z</tt>.<para>
		/// 
		/// It is generally the case, but <i>not</i> strictly required that
		/// <tt>(compare(x, y)==0) == (x.equals(y))</tt>.  Generally speaking,
		/// any comparator that violates this condition should clearly indicate
		/// this fact.  The recommended language is "Note: this comparator
		/// imposes orderings that are inconsistent with equals."
		/// 
		/// </para>
		/// </summary>
		/// <param name="o1"> the first object to be compared. </param>
		/// <param name="o2"> the second object to be compared. </param>
		/// <returns> a negative integer, zero, or a positive integer as the
		///         first argument is less than, equal to, or greater than the
		///         second. </returns>
		/// <exception cref="NullPointerException"> if an argument is null and this
		///         comparator does not permit null arguments </exception>
		/// <exception cref="ClassCastException"> if the arguments' types prevent them from
		///         being compared by this comparator. </exception>
		int Compare(T o1, T o2);

		/// <summary>
		/// Indicates whether some other object is &quot;equal to&quot; this
		/// comparator.  This method must obey the general contract of
		/// <seealso cref="Object#equals(Object)"/>.  Additionally, this method can return
		/// <tt>true</tt> <i>only</i> if the specified object is also a comparator
		/// and it imposes the same ordering as this comparator.  Thus,
		/// <code>comp1.equals(comp2)</code> implies that <tt>sgn(comp1.compare(o1,
		/// o2))==sgn(comp2.compare(o1, o2))</tt> for every object reference
		/// <tt>o1</tt> and <tt>o2</tt>.<para>
		/// 
		/// Note that it is <i>always</i> safe <i>not</i> to override
		/// <tt>Object.equals(Object)</tt>.  However, overriding this method may,
		/// in some cases, improve performance by allowing programs to determine
		/// that two distinct comparators impose the same order.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   the reference object with which to compare. </param>
		/// <returns>  <code>true</code> only if the specified object is also
		///          a comparator and it imposes the same ordering as this
		///          comparator. </returns>
		/// <seealso cref= Object#equals(Object) </seealso>
		/// <seealso cref= Object#hashCode() </seealso>
		bool Equals(Object obj);

		/// <summary>
		/// Returns a comparator that imposes the reverse ordering of this
		/// comparator.
		/// </summary>
		/// <returns> a comparator that imposes the reverse ordering of this
		///         comparator.
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Comparator<T> reversed()
	//	{
	//		return Collections.reverseOrder(this);
	//	}

		/// <summary>
		/// Returns a lexicographic-order comparator with another comparator.
		/// If this {@code Comparator} considers two elements equal, i.e.
		/// {@code compare(a, b) == 0}, {@code other} is used to determine the order.
		/// 
		/// <para>The returned comparator is serializable if the specified comparator
		/// is also serializable.
		/// 
		/// @apiNote
		/// For example, to sort a collection of {@code String} based on the length
		/// and then case-insensitive natural ordering, the comparator can be
		/// composed using following code,
		/// 
		/// <pre>{@code
		///     Comparator<String> cmp = Comparator.comparingInt(String::length)
		///             .thenComparing(String.CASE_INSENSITIVE_ORDER);
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="other"> the other comparator to be used when this comparator
		///         compares two objects that are equal. </param>
		/// <returns> a lexicographic-order comparator composed of this and then the
		///         other comparator </returns>
		/// <exception cref="NullPointerException"> if the argument is null.
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default Comparator<T> thenComparing(Comparator<? base T> other)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Comparator<T> thenComparing(Comparator<JavaToDotNetGenericWildcard> other)
	//	{
	//		Objects.requireNonNull(other);
	//		return (Comparator<T> & Serializable)(c1, c2) ->
	//		{
	//			return (res != 0) ? res : other.compare(c1, c2);
	//		};
	//	}

		/// <summary>
		/// Returns a lexicographic-order comparator with a function that
		/// extracts a key to be compared with the given {@code Comparator}.
		/// 
		/// @implSpec This default implementation behaves as if {@code
		///           thenComparing(comparing(keyExtractor, cmp))}.
		/// </summary>
		/// @param  <U>  the type of the sort key </param>
		/// <param name="keyExtractor"> the function used to extract the sort key </param>
		/// <param name="keyComparator"> the {@code Comparator} used to compare the sort key </param>
		/// <returns> a lexicographic-order comparator composed of this comparator
		///         and then comparing on the key extracted by the keyExtractor function </returns>
		/// <exception cref="NullPointerException"> if either argument is null. </exception>
		/// <seealso cref= #comparing(Function, Comparator) </seealso>
		/// <seealso cref= #thenComparing(Comparator)
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <U> Comparator<T> thenComparing(java.util.function.Function<? base T, ? extends U> keyExtractor, Comparator<? base U> keyComparator)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <U> Comparator<T> thenComparing(java.util.function.Function<? base T, ? extends U> keyExtractor, Comparator<? base U> keyComparator)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default <U> Comparator<T> thenComparing(java.util.function.Function<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> keyExtractor, Comparator<JavaToDotNetGenericWildcard> keyComparator) where ? : U
	//	{
	//		return thenComparing(comparing(keyExtractor, keyComparator));
	//	}

		/// <summary>
		/// Returns a lexicographic-order comparator with a function that
		/// extracts a {@code Comparable} sort key.
		/// 
		/// @implSpec This default implementation behaves as if {@code
		///           thenComparing(comparing(keyExtractor))}.
		/// </summary>
		/// @param  <U>  the type of the <seealso cref="Comparable"/> sort key </param>
		/// <param name="keyExtractor"> the function used to extract the {@link
		///         Comparable} sort key </param>
		/// <returns> a lexicographic-order comparator composed of this and then the
		///         <seealso cref="Comparable"/> sort key. </returns>
		/// <exception cref="NullPointerException"> if the argument is null. </exception>
		/// <seealso cref= #comparing(Function) </seealso>
		/// <seealso cref= #thenComparing(Comparator)
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <U extends Comparable<? base U>> Comparator<T> thenComparing(java.util.function.Function<? base T, ? extends U> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default <U extends Comparable<? base U>> Comparator<T> thenComparing(java.util.function.Function<? base T, ? extends U> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default <U> Comparator<T> thenComparing(java.util.function.Function<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> keyExtractor) where U : Comparable<? base U> where ? : U
	//	{
	//		return thenComparing(comparing(keyExtractor));
	//	}

		/// <summary>
		/// Returns a lexicographic-order comparator with a function that
		/// extracts a {@code int} sort key.
		/// 
		/// @implSpec This default implementation behaves as if {@code
		///           thenComparing(comparingInt(keyExtractor))}.
		/// </summary>
		/// <param name="keyExtractor"> the function used to extract the integer sort key </param>
		/// <returns> a lexicographic-order comparator composed of this and then the
		///         {@code int} sort key </returns>
		/// <exception cref="NullPointerException"> if the argument is null. </exception>
		/// <seealso cref= #comparingInt(ToIntFunction) </seealso>
		/// <seealso cref= #thenComparing(Comparator)
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default Comparator<T> thenComparingInt(java.util.function.ToIntFunction<? base T> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Comparator<T> thenComparingInt(java.util.function.ToIntFunction<JavaToDotNetGenericWildcard> keyExtractor)
	//	{
	//		return thenComparing(comparingInt(keyExtractor));
	//	}

		/// <summary>
		/// Returns a lexicographic-order comparator with a function that
		/// extracts a {@code long} sort key.
		/// 
		/// @implSpec This default implementation behaves as if {@code
		///           thenComparing(comparingLong(keyExtractor))}.
		/// </summary>
		/// <param name="keyExtractor"> the function used to extract the long sort key </param>
		/// <returns> a lexicographic-order comparator composed of this and then the
		///         {@code long} sort key </returns>
		/// <exception cref="NullPointerException"> if the argument is null. </exception>
		/// <seealso cref= #comparingLong(ToLongFunction) </seealso>
		/// <seealso cref= #thenComparing(Comparator)
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default Comparator<T> thenComparingLong(java.util.function.ToLongFunction<? base T> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Comparator<T> thenComparingLong(java.util.function.ToLongFunction<JavaToDotNetGenericWildcard> keyExtractor)
	//	{
	//		return thenComparing(comparingLong(keyExtractor));
	//	}

		/// <summary>
		/// Returns a lexicographic-order comparator with a function that
		/// extracts a {@code double} sort key.
		/// 
		/// @implSpec This default implementation behaves as if {@code
		///           thenComparing(comparingDouble(keyExtractor))}.
		/// </summary>
		/// <param name="keyExtractor"> the function used to extract the double sort key </param>
		/// <returns> a lexicographic-order comparator composed of this and then the
		///         {@code double} sort key </returns>
		/// <exception cref="NullPointerException"> if the argument is null. </exception>
		/// <seealso cref= #comparingDouble(ToDoubleFunction) </seealso>
		/// <seealso cref= #thenComparing(Comparator)
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: default Comparator<T> thenComparingDouble(java.util.function.ToDoubleFunction<? base T> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default Comparator<T> thenComparingDouble(java.util.function.ToDoubleFunction<JavaToDotNetGenericWildcard> keyExtractor)
	//	{
	//		return thenComparing(comparingDouble(keyExtractor));
	//	}

		/// <summary>
		/// Returns a comparator that imposes the reverse of the <em>natural
		/// ordering</em>.
		/// 
		/// <para>The returned comparator is serializable and throws {@link
		/// NullPointerException} when comparing {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the <seealso cref="Comparable"/> type of element to be compared </param>
		/// <returns> a comparator that imposes the reverse of the <i>natural
		///         ordering</i> on {@code Comparable} objects. </returns>
		/// <seealso cref= Comparable
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T extends Comparable<? base T>> Comparator<T> reverseOrder()
//		public static <T> Comparator<T> reverseOrder() where T : Comparable<? base T>
	//	{
	//		return Collections.reverseOrder();
	//	}

		/// <summary>
		/// Returns a comparator that compares <seealso cref="Comparable"/> objects in natural
		/// order.
		/// 
		/// <para>The returned comparator is serializable and throws {@link
		/// NullPointerException} when comparing {@code null}.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the <seealso cref="Comparable"/> type of element to be compared </param>
		/// <returns> a comparator that imposes the <i>natural ordering</i> on {@code
		///         Comparable} objects. </returns>
		/// <seealso cref= Comparable
		/// @since 1.8 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends Comparable<? base T>> Comparator<T> naturalOrder()
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends Comparable<? base T>> Comparator<T> naturalOrder()
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		public static <T> Comparator<T> naturalOrder() where T : Comparable<? base T>
	//	{
	//		return (Comparator<T>) Comparators.NaturalOrderComparator.INSTANCE;
	//	}

		/// <summary>
		/// Returns a null-friendly comparator that considers {@code null} to be
		/// less than non-null. When both are {@code null}, they are considered
		/// equal. If both are non-null, the specified {@code Comparator} is used
		/// to determine the order. If the specified comparator is {@code null},
		/// then the returned comparator considers all non-null values to be equal.
		/// 
		/// <para>The returned comparator is serializable if the specified comparator
		/// is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the type of the elements to be compared </param>
		/// <param name="comparator"> a {@code Comparator} for comparing non-null values </param>
		/// <returns> a comparator that considers {@code null} to be less than
		///         non-null, and compares non-null objects with the supplied
		///         {@code Comparator}.
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Comparator<T> nullsFirst(Comparator<? base T> comparator)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static <T> Comparator<T> nullsFirst(Comparator<JavaToDotNetGenericWildcard> comparator)
	//	{
	//		return new Comparators.NullComparator<>(true, comparator);
	//	}

		/// <summary>
		/// Returns a null-friendly comparator that considers {@code null} to be
		/// greater than non-null. When both are {@code null}, they are considered
		/// equal. If both are non-null, the specified {@code Comparator} is used
		/// to determine the order. If the specified comparator is {@code null},
		/// then the returned comparator considers all non-null values to be equal.
		/// 
		/// <para>The returned comparator is serializable if the specified comparator
		/// is serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the type of the elements to be compared </param>
		/// <param name="comparator"> a {@code Comparator} for comparing non-null values </param>
		/// <returns> a comparator that considers {@code null} to be greater than
		///         non-null, and compares non-null objects with the supplied
		///         {@code Comparator}.
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Comparator<T> nullsLast(Comparator<? base T> comparator)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static <T> Comparator<T> nullsLast(Comparator<JavaToDotNetGenericWildcard> comparator)
	//	{
	//		return new Comparators.NullComparator<>(false, comparator);
	//	}

		/// <summary>
		/// Accepts a function that extracts a sort key from a type {@code T}, and
		/// returns a {@code Comparator<T>} that compares by that sort key using
		/// the specified <seealso cref="Comparator"/>.
		///  
		/// <para>The returned comparator is serializable if the specified function
		/// and comparator are both serializable.
		/// 
		/// @apiNote
		/// For example, to obtain a {@code Comparator} that compares {@code
		/// Person} objects by their last name ignoring case differences,
		/// 
		/// <pre>{@code
		///     Comparator<Person> cmp = Comparator.comparing(
		///             Person::getLastName,
		///             String.CASE_INSENSITIVE_ORDER);
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the type of element to be compared </param>
		/// @param  <U> the type of the sort key </param>
		/// <param name="keyExtractor"> the function used to extract the sort key </param>
		/// <param name="keyComparator"> the {@code Comparator} used to compare the sort key </param>
		/// <returns> a comparator that compares by an extracted key using the
		///         specified {@code Comparator} </returns>
		/// <exception cref="NullPointerException"> if either argument is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, U> Comparator<T> comparing(java.util.function.Function<? base T, ? extends U> keyExtractor, Comparator<? base U> keyComparator)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, U> Comparator<T> comparing(java.util.function.Function<? base T, ? extends U> keyExtractor, Comparator<? base U> keyComparator)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		public static <T, U> Comparator<T> comparing(java.util.function.Function<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> keyExtractor, Comparator<JavaToDotNetGenericWildcard> keyComparator) where ? : U
	//	{
	//		Objects.requireNonNull(keyExtractor);
	//		Objects.requireNonNull(keyComparator);
	//		return (Comparator<T> & Serializable)(c1, c2) -> keyComparator.compare(keyExtractor.apply(c1), keyExtractor.apply(c2));
	//	}

		/// <summary>
		/// Accepts a function that extracts a {@link java.lang.Comparable
		/// Comparable} sort key from a type {@code T}, and returns a {@code
		/// Comparator<T>} that compares by that sort key.
		/// 
		/// <para>The returned comparator is serializable if the specified function
		/// is also serializable.
		/// 
		/// @apiNote
		/// For example, to obtain a {@code Comparator} that compares {@code
		/// Person} objects by their last name,
		/// 
		/// <pre>{@code
		///     Comparator<Person> byLastName = Comparator.comparing(Person::getLastName);
		/// }</pre>
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the type of element to be compared </param>
		/// @param  <U> the type of the {@code Comparable} sort key </param>
		/// <param name="keyExtractor"> the function used to extract the {@link
		///         Comparable} sort key </param>
		/// <returns> a comparator that compares by an extracted key </returns>
		/// <exception cref="NullPointerException"> if the argument is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, U extends Comparable<? base U>> Comparator<T> comparing(java.util.function.Function<? base T, ? extends U> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T, U extends Comparable<? base U>> Comparator<T> comparing(java.util.function.Function<? base T, ? extends U> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		public static <T, U> Comparator<T> comparing(java.util.function.Function<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> keyExtractor) where U : Comparable<? base U> where ? : U
	//	{
	//		Objects.requireNonNull(keyExtractor);
	//		return (Comparator<T> & Serializable)(c1, c2) -> keyExtractor.apply(c1).compareTo(keyExtractor.apply(c2));
	//	}

		/// <summary>
		/// Accepts a function that extracts an {@code int} sort key from a type
		/// {@code T}, and returns a {@code Comparator<T>} that compares by that
		/// sort key.
		/// 
		/// <para>The returned comparator is serializable if the specified function
		/// is also serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the type of element to be compared </param>
		/// <param name="keyExtractor"> the function used to extract the integer sort key </param>
		/// <returns> a comparator that compares by an extracted key </returns>
		/// <seealso cref= #comparing(Function) </seealso>
		/// <exception cref="NullPointerException"> if the argument is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Comparator<T> comparingInt(java.util.function.ToIntFunction<? base T> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static <T> Comparator<T> comparingInt(java.util.function.ToIntFunction<JavaToDotNetGenericWildcard> keyExtractor)
	//	{
	//		Objects.requireNonNull(keyExtractor);
	//		return (Comparator<T> & Serializable)(c1, c2) -> Integer.compare(keyExtractor.applyAsInt(c1), keyExtractor.applyAsInt(c2));
	//	}

		/// <summary>
		/// Accepts a function that extracts a {@code long} sort key from a type
		/// {@code T}, and returns a {@code Comparator<T>} that compares by that
		/// sort key.
		/// 
		/// <para>The returned comparator is serializable if the specified function is
		/// also serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the type of element to be compared </param>
		/// <param name="keyExtractor"> the function used to extract the long sort key </param>
		/// <returns> a comparator that compares by an extracted key </returns>
		/// <seealso cref= #comparing(Function) </seealso>
		/// <exception cref="NullPointerException"> if the argument is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> Comparator<T> comparingLong(java.util.function.ToLongFunction<? base T> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static <T> Comparator<T> comparingLong(java.util.function.ToLongFunction<JavaToDotNetGenericWildcard> keyExtractor)
	//	{
	//		Objects.requireNonNull(keyExtractor);
	//		return (Comparator<T> & Serializable)(c1, c2) -> Long.compare(keyExtractor.applyAsLong(c1), keyExtractor.applyAsLong(c2));
	//	}

		/// <summary>
		/// Accepts a function that extracts a {@code double} sort key from a type
		/// {@code T}, and returns a {@code Comparator<T>} that compares by that
		/// sort key.
		/// 
		/// <para>The returned comparator is serializable if the specified function
		/// is also serializable.
		/// 
		/// </para>
		/// </summary>
		/// @param  <T> the type of element to be compared </param>
		/// <param name="keyExtractor"> the function used to extract the double sort key </param>
		/// <returns> a comparator that compares by an extracted key </returns>
		/// <seealso cref= #comparing(Function) </seealso>
		/// <exception cref="NullPointerException"> if the argument is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static<T> Comparator<T> comparingDouble(java.util.function.ToDoubleFunction<? base T> keyExtractor)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
//		public static<T> Comparator<T> comparingDouble(java.util.function.ToDoubleFunction<JavaToDotNetGenericWildcard> keyExtractor)
	//	{
	//		Objects.requireNonNull(keyExtractor);
	//		return (Comparator<T> & Serializable)(c1, c2) -> Double.compare(keyExtractor.applyAsDouble(c1), keyExtractor.applyAsDouble(c2));
	//	}
	}

	public static class Comparator_Fields
	{
				public static readonly int Res = compare(c1, c2);
	}

}