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

namespace java.lang
{

	/// <summary>
	/// This interface imposes a total ordering on the objects of each class that
	/// implements it.  This ordering is referred to as the class's <i>natural
	/// ordering</i>, and the class's <tt>compareTo</tt> method is referred to as
	/// its <i>natural comparison method</i>.<para>
	/// 
	/// Lists (and arrays) of objects that implement this interface can be sorted
	/// automatically by <seealso cref="Collections#sort(List) Collections.sort"/> (and
	/// <seealso cref="Arrays#sort(Object[]) Arrays.sort"/>).  Objects that implement this
	/// interface can be used as keys in a <seealso cref="SortedMap sorted map"/> or as
	/// elements in a <seealso cref="SortedSet sorted set"/>, without the need to
	/// </para>
	/// specify a <seealso cref="Comparator comparator"/>.<para>
	/// 
	/// The natural ordering for a class <tt>C</tt> is said to be <i>consistent
	/// with equals</i> if and only if <tt>e1.compareTo(e2) == 0</tt> has
	/// the same boolean value as <tt>e1.equals(e2)</tt> for every
	/// <tt>e1</tt> and <tt>e2</tt> of class <tt>C</tt>.  Note that <tt>null</tt>
	/// is not an instance of any class, and <tt>e.compareTo(null)</tt> should
	/// throw a <tt>NullPointerException</tt> even though <tt>e.equals(null)</tt>
	/// </para>
	/// returns <tt>false</tt>.<para>
	/// 
	/// It is strongly recommended (though not required) that natural orderings be
	/// consistent with equals.  This is so because sorted sets (and sorted maps)
	/// without explicit comparators behave "strangely" when they are used with
	/// elements (or keys) whose natural ordering is inconsistent with equals.  In
	/// particular, such a sorted set (or sorted map) violates the general contract
	/// for set (or map), which is defined in terms of the <tt>equals</tt>
	/// </para>
	/// method.<para>
	/// 
	/// For example, if one adds two keys <tt>a</tt> and <tt>b</tt> such that
	/// {@code (!a.equals(b) && a.compareTo(b) == 0)} to a sorted
	/// set that does not use an explicit comparator, the second <tt>add</tt>
	/// operation returns false (and the size of the sorted set does not increase)
	/// because <tt>a</tt> and <tt>b</tt> are equivalent from the sorted set's
	/// </para>
	/// perspective.<para>
	/// 
	/// Virtually all Java core classes that implement <tt>Comparable</tt> have natural
	/// orderings that are consistent with equals.  One exception is
	/// <tt>java.math.BigDecimal</tt>, whose natural ordering equates
	/// <tt>BigDecimal</tt> objects with equal values and different precisions
	/// </para>
	/// (such as 4.0 and 4.00).<para>
	/// 
	/// For the mathematically inclined, the <i>relation</i> that defines
	/// the natural ordering on a given class C is:<pre>
	///       {(x, y) such that x.compareTo(y) &lt;= 0}.
	/// </pre> The <i>quotient</i> for this total order is: <pre>
	///       {(x, y) such that x.compareTo(y) == 0}.
	/// </pre>
	/// 
	/// It follows immediately from the contract for <tt>compareTo</tt> that the
	/// quotient is an <i>equivalence relation</i> on <tt>C</tt>, and that the
	/// natural ordering is a <i>total order</i> on <tt>C</tt>.  When we say that a
	/// class's natural ordering is <i>consistent with equals</i>, we mean that the
	/// quotient for the natural ordering is the equivalence relation defined by
	/// the class's <seealso cref="Object#equals(Object) equals(Object)"/> method:<pre>
	/// </para>
	///     {(x, y) such that x.equals(y)}. </pre><para>
	/// 
	/// This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// </para>
	/// </summary>
	/// @param <T> the type of objects that this object may be compared to
	/// 
	/// @author  Josh Bloch </param>
	/// <seealso cref= java.util.Comparator
	/// @since 1.2 </seealso>
	public interface Comparable<T>
	{
		/// <summary>
		/// Compares this object with the specified object for order.  Returns a
		/// negative integer, zero, or a positive integer as this object is less
		/// than, equal to, or greater than the specified object.
		/// 
		/// <para>The implementor must ensure <tt>sgn(x.compareTo(y)) ==
		/// -sgn(y.compareTo(x))</tt> for all <tt>x</tt> and <tt>y</tt>.  (This
		/// implies that <tt>x.compareTo(y)</tt> must throw an exception iff
		/// <tt>y.compareTo(x)</tt> throws an exception.)
		/// 
		/// </para>
		/// <para>The implementor must also ensure that the relation is transitive:
		/// <tt>(x.compareTo(y)&gt;0 &amp;&amp; y.compareTo(z)&gt;0)</tt> implies
		/// <tt>x.compareTo(z)&gt;0</tt>.
		/// 
		/// </para>
		/// <para>Finally, the implementor must ensure that <tt>x.compareTo(y)==0</tt>
		/// implies that <tt>sgn(x.compareTo(z)) == sgn(y.compareTo(z))</tt>, for
		/// all <tt>z</tt>.
		/// 
		/// </para>
		/// <para>It is strongly recommended, but <i>not</i> strictly required that
		/// <tt>(x.compareTo(y)==0) == (x.equals(y))</tt>.  Generally speaking, any
		/// class that implements the <tt>Comparable</tt> interface and violates
		/// this condition should clearly indicate this fact.  The recommended
		/// language is "Note: this class has a natural ordering that is
		/// inconsistent with equals."
		/// 
		/// </para>
		/// <para>In the foregoing description, the notation
		/// <tt>sgn(</tt><i>expression</i><tt>)</tt> designates the mathematical
		/// <i>signum</i> function, which is defined to return one of <tt>-1</tt>,
		/// <tt>0</tt>, or <tt>1</tt> according to whether the value of
		/// <i>expression</i> is negative, zero or positive.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> the object to be compared. </param>
		/// <returns>  a negative integer, zero, or a positive integer as this object
		///          is less than, equal to, or greater than the specified object.
		/// </returns>
		/// <exception cref="NullPointerException"> if the specified object is null </exception>
		/// <exception cref="ClassCastException"> if the specified object's type prevents it
		///         from being compared to this object. </exception>
		int CompareTo(T o);
	}

}