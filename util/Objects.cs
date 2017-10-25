using System;

/*
 * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class consists of {@code static} utility methods for operating
	/// on objects.  These utilities include {@code null}-safe or {@code
	/// null}-tolerant methods for computing the hash code of an object,
	/// returning a string for an object, and comparing two objects.
	/// 
	/// @since 1.7
	/// </summary>
	public sealed class Objects
	{
		private Objects()
		{
			throw new AssertionError("No java.util.Objects instances for you!");
		}

		/// <summary>
		/// Returns {@code true} if the arguments are equal to each other
		/// and {@code false} otherwise.
		/// Consequently, if both arguments are {@code null}, {@code true}
		/// is returned and if exactly one argument is {@code null}, {@code
		/// false} is returned.  Otherwise, equality is determined by using
		/// the <seealso cref="Object#equals equals"/> method of the first
		/// argument.
		/// </summary>
		/// <param name="a"> an object </param>
		/// <param name="b"> an object to be compared with {@code a} for equality </param>
		/// <returns> {@code true} if the arguments are equal to each other
		/// and {@code false} otherwise </returns>
		/// <seealso cref= Object#equals(Object) </seealso>
		public static bool Equals(Object a, Object b)
		{
			return (a == b) || (a != null && a.Equals(b));
		}

	   /// <summary>
	   /// Returns {@code true} if the arguments are deeply equal to each other
	   /// and {@code false} otherwise.
	   /// 
	   /// Two {@code null} values are deeply equal.  If both arguments are
	   /// arrays, the algorithm in {@link Arrays#deepEquals(Object[],
	   /// Object[]) Arrays.deepEquals} is used to determine equality.
	   /// Otherwise, equality is determined by using the {@link
	   /// Object#equals equals} method of the first argument.
	   /// </summary>
	   /// <param name="a"> an object </param>
	   /// <param name="b"> an object to be compared with {@code a} for deep equality </param>
	   /// <returns> {@code true} if the arguments are deeply equal to each other
	   /// and {@code false} otherwise </returns>
	   /// <seealso cref= Arrays#deepEquals(Object[], Object[]) </seealso>
	   /// <seealso cref= Objects#equals(Object, Object) </seealso>
		public static bool DeepEquals(Object a, Object b)
		{
			if (a == b)
			{
				return true;
			}
			else if (a == null || b == null)
			{
				return false;
			}
			else
			{
				return Arrays.DeepEquals0(a, b);
			}
		}

		/// <summary>
		/// Returns the hash code of a non-{@code null} argument and 0 for
		/// a {@code null} argument.
		/// </summary>
		/// <param name="o"> an object </param>
		/// <returns> the hash code of a non-{@code null} argument and 0 for
		/// a {@code null} argument </returns>
		/// <seealso cref= Object#hashCode </seealso>
		public static int HashCode(Object o)
		{
			return o != null ? o.HashCode() : 0;
		}

	   /// <summary>
	   /// Generates a hash code for a sequence of input values. The hash
	   /// code is generated as if all the input values were placed into an
	   /// array, and that array were hashed by calling {@link
	   /// Arrays#hashCode(Object[])}.
	   /// 
	   /// <para>This method is useful for implementing {@link
	   /// Object#hashCode()} on objects containing multiple fields. For
	   /// example, if an object that has three fields, {@code x}, {@code
	   /// y}, and {@code z}, one could write:
	   /// 
	   /// <blockquote><pre>
	   /// &#064;Override public int hashCode() {
	   ///     return Objects.hash(x, y, z);
	   /// }
	   /// </pre></blockquote>
	   /// 
	   /// <b>Warning: When a single object reference is supplied, the returned
	   /// value does not equal the hash code of that object reference.</b> This
	   /// value can be computed by calling <seealso cref="#hashCode(Object)"/>.
	   /// 
	   /// </para>
	   /// </summary>
	   /// <param name="values"> the values to be hashed </param>
	   /// <returns> a hash value of the sequence of input values </returns>
	   /// <seealso cref= Arrays#hashCode(Object[]) </seealso>
	   /// <seealso cref= List#hashCode </seealso>
		public static int Hash(params Object[] values)
		{
			return Arrays.HashCode(values);
		}

		/// <summary>
		/// Returns the result of calling {@code toString} for a non-{@code
		/// null} argument and {@code "null"} for a {@code null} argument.
		/// </summary>
		/// <param name="o"> an object </param>
		/// <returns> the result of calling {@code toString} for a non-{@code
		/// null} argument and {@code "null"} for a {@code null} argument </returns>
		/// <seealso cref= Object#toString </seealso>
		/// <seealso cref= String#valueOf(Object) </seealso>
		public static String ToString(Object o)
		{
			return Convert.ToString(o);
		}

		/// <summary>
		/// Returns the result of calling {@code toString} on the first
		/// argument if the first argument is not {@code null} and returns
		/// the second argument otherwise.
		/// </summary>
		/// <param name="o"> an object </param>
		/// <param name="nullDefault"> string to return if the first argument is
		///        {@code null} </param>
		/// <returns> the result of calling {@code toString} on the first
		/// argument if it is not {@code null} and the second argument
		/// otherwise. </returns>
		/// <seealso cref= Objects#toString(Object) </seealso>
		public static String ToString(Object o, String nullDefault)
		{
			return (o != null) ? o.ToString() : nullDefault;
		}

		/// <summary>
		/// Returns 0 if the arguments are identical and {@code
		/// c.compare(a, b)} otherwise.
		/// Consequently, if both arguments are {@code null} 0
		/// is returned.
		/// 
		/// <para>Note that if one of the arguments is {@code null}, a {@code
		/// NullPointerException} may or may not be thrown depending on
		/// what ordering policy, if any, the <seealso cref="Comparator Comparator"/>
		/// chooses to have for {@code null} values.
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type of the objects being compared </param>
		/// <param name="a"> an object </param>
		/// <param name="b"> an object to be compared with {@code a} </param>
		/// <param name="c"> the {@code Comparator} to compare the first two arguments </param>
		/// <returns> 0 if the arguments are identical and {@code
		/// c.compare(a, b)} otherwise. </returns>
		/// <seealso cref= Comparable </seealso>
		/// <seealso cref= Comparator </seealso>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public static <T> int compare(T a, T b, Comparator<? base T> c)
		public static int compare<T, T1>(T a, T b, Comparator<T1> c)
		{
			return (a == b) ? 0 : c.Compare(a, b);
		}

		/// <summary>
		/// Checks that the specified object reference is not {@code null}. This
		/// method is designed primarily for doing parameter validation in methods
		/// and constructors, as demonstrated below:
		/// <blockquote><pre>
		/// public Foo(Bar bar) {
		///     this.bar = Objects.requireNonNull(bar);
		/// }
		/// </pre></blockquote>
		/// </summary>
		/// <param name="obj"> the object reference to check for nullity </param>
		/// @param <T> the type of the reference </param>
		/// <returns> {@code obj} if not {@code null} </returns>
		/// <exception cref="NullPointerException"> if {@code obj} is {@code null} </exception>
		public static T requireNonNull<T>(T obj)
		{
			if (obj == null)
			{
				throw new NullPointerException();
			}
			return obj;
		}

		/// <summary>
		/// Checks that the specified object reference is not {@code null} and
		/// throws a customized <seealso cref="NullPointerException"/> if it is. This method
		/// is designed primarily for doing parameter validation in methods and
		/// constructors with multiple parameters, as demonstrated below:
		/// <blockquote><pre>
		/// public Foo(Bar bar, Baz baz) {
		///     this.bar = Objects.requireNonNull(bar, "bar must not be null");
		///     this.baz = Objects.requireNonNull(baz, "baz must not be null");
		/// }
		/// </pre></blockquote>
		/// </summary>
		/// <param name="obj">     the object reference to check for nullity </param>
		/// <param name="message"> detail message to be used in the event that a {@code
		///                NullPointerException} is thrown </param>
		/// @param <T> the type of the reference </param>
		/// <returns> {@code obj} if not {@code null} </returns>
		/// <exception cref="NullPointerException"> if {@code obj} is {@code null} </exception>
		public static T requireNonNull<T>(T obj, String message)
		{
			if (obj == null)
			{
				throw new NullPointerException(message);
			}
			return obj;
		}

		/// <summary>
		/// Returns {@code true} if the provided reference is {@code null} otherwise
		/// returns {@code false}.
		/// 
		/// @apiNote This method exists to be used as a
		/// <seealso cref="java.util.function.Predicate"/>, {@code filter(Objects::isNull)}
		/// </summary>
		/// <param name="obj"> a reference to be checked against {@code null} </param>
		/// <returns> {@code true} if the provided reference is {@code null} otherwise
		/// {@code false}
		/// </returns>
		/// <seealso cref= java.util.function.Predicate
		/// @since 1.8 </seealso>
		public static bool IsNull(Object obj)
		{
			return obj == null;
		}

		/// <summary>
		/// Returns {@code true} if the provided reference is non-{@code null}
		/// otherwise returns {@code false}.
		/// 
		/// @apiNote This method exists to be used as a
		/// <seealso cref="java.util.function.Predicate"/>, {@code filter(Objects::nonNull)}
		/// </summary>
		/// <param name="obj"> a reference to be checked against {@code null} </param>
		/// <returns> {@code true} if the provided reference is non-{@code null}
		/// otherwise {@code false}
		/// </returns>
		/// <seealso cref= java.util.function.Predicate
		/// @since 1.8 </seealso>
		public static bool NonNull(Object obj)
		{
			return obj != null;
		}

		/// <summary>
		/// Checks that the specified object reference is not {@code null} and
		/// throws a customized <seealso cref="NullPointerException"/> if it is.
		/// 
		/// <para>Unlike the method <seealso cref="#requireNonNull(Object, String)"/>,
		/// this method allows creation of the message to be deferred until
		/// after the null check is made. While this may confer a
		/// performance advantage in the non-null case, when deciding to
		/// call this method care should be taken that the costs of
		/// creating the message supplier are less than the cost of just
		/// creating the string message directly.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">     the object reference to check for nullity </param>
		/// <param name="messageSupplier"> supplier of the detail message to be
		/// used in the event that a {@code NullPointerException} is thrown </param>
		/// @param <T> the type of the reference </param>
		/// <returns> {@code obj} if not {@code null} </returns>
		/// <exception cref="NullPointerException"> if {@code obj} is {@code null}
		/// @since 1.8 </exception>
		public static T requireNonNull<T>(T obj, Supplier<String> messageSupplier)
		{
			if (obj == null)
			{
				throw new NullPointerException(messageSupplier.Get());
			}
			return obj;
		}
	}

}