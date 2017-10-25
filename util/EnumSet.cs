using System;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using SharedSecrets = sun.misc.SharedSecrets;

	/// <summary>
	/// A specialized <seealso cref="Set"/> implementation for use with enum types.  All of
	/// the elements in an enum set must come from a single enum type that is
	/// specified, explicitly or implicitly, when the set is created.  Enum sets
	/// are represented internally as bit vectors.  This representation is
	/// extremely compact and efficient. The space and time performance of this
	/// class should be good enough to allow its use as a high-quality, typesafe
	/// alternative to traditional <tt>int</tt>-based "bit flags."  Even bulk
	/// operations (such as <tt>containsAll</tt> and <tt>retainAll</tt>) should
	/// run very quickly if their argument is also an enum set.
	/// 
	/// <para>The iterator returned by the <tt>iterator</tt> method traverses the
	/// elements in their <i>natural order</i> (the order in which the enum
	/// constants are declared).  The returned iterator is <i>weakly
	/// consistent</i>: it will never throw <seealso cref="ConcurrentModificationException"/>
	/// and it may or may not show the effects of any modifications to the set that
	/// occur while the iteration is in progress.
	/// 
	/// </para>
	/// <para>Null elements are not permitted.  Attempts to insert a null element
	/// will throw <seealso cref="NullPointerException"/>.  Attempts to test for the
	/// presence of a null element or to remove one will, however, function
	/// properly.
	/// 
	/// <P>Like most collection implementations, <tt>EnumSet</tt> is not
	/// synchronized.  If multiple threads access an enum set concurrently, and at
	/// least one of the threads modifies the set, it should be synchronized
	/// externally.  This is typically accomplished by synchronizing on some
	/// object that naturally encapsulates the enum set.  If no such object exists,
	/// the set should be "wrapped" using the <seealso cref="Collections#synchronizedSet"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access:
	/// 
	/// <pre>
	/// Set&lt;MyEnum&gt; s = Collections.synchronizedSet(EnumSet.noneOf(MyEnum.class));
	/// </pre>
	/// 
	/// </para>
	/// <para>Implementation note: All basic operations execute in constant time.
	/// They are likely (though not guaranteed) to be much faster than their
	/// <seealso cref="HashSet"/> counterparts.  Even bulk operations execute in
	/// constant time if their argument is also an enum set.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author Josh Bloch
	/// @since 1.5
	/// </para>
	/// </summary>
	/// <seealso cref= EnumMap
	/// @serial exclude </seealso>
	[Serializable]
	public abstract class EnumSet<E> : AbstractSet<E>, Cloneable where E : Enum<E>
	{
		/// <summary>
		/// The class of all the elements of this set.
		/// </summary>
		internal readonly Class ElementType;

		/// <summary>
		/// All of the values comprising T.  (Cached for performance.)
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final Enum<?>[] universe;
		internal readonly Enum<?>[] Universe;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static Enum<?>[] ZERO_LENGTH_ENUM_ARRAY = new Enum<?>[0];
		private static Enum<?>[] ZERO_LENGTH_ENUM_ARRAY = new Enum<?>[0];

		internal EnumSet<T1>(ClasselementType, Enum<T1>[] universe)
		{
			this.ElementType = ElementType;
			this.Universe = universe;
		}

		/// <summary>
		/// Creates an empty enum set with the specified element type.
		/// </summary>
		/// @param <E> The class of the elements in the set </param>
		/// <param name="elementType"> the class object of the element type for this enum
		///     set </param>
		/// <returns> An empty enum set of the specified type. </returns>
		/// <exception cref="NullPointerException"> if <tt>elementType</tt> is null </exception>
		public static EnumSet<E> noneOf<E>(Class elementType) where E : Enum<E>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Enum<?>[] universe = getUniverse(elementType);
			Enum<?>[] universe = GetUniverse(elementType);
			if (universe == null)
			{
				throw new ClassCastException(elementType + " not an enum");
			}

			if (universe.Length <= 64)
			{
				return new RegularEnumSet<>(elementType, universe);
			}
			else
			{
				return new JumboEnumSet<>(elementType, universe);
			}
		}

		/// <summary>
		/// Creates an enum set containing all of the elements in the specified
		/// element type.
		/// </summary>
		/// @param <E> The class of the elements in the set </param>
		/// <param name="elementType"> the class object of the element type for this enum
		///     set </param>
		/// <returns> An enum set containing all the elements in the specified type. </returns>
		/// <exception cref="NullPointerException"> if <tt>elementType</tt> is null </exception>
		public static EnumSet<E> allOf<E>(Class elementType) where E : Enum<E>
		{
			EnumSet<E> result = NoneOf(elementType);
			result.AddAll();
			return result;
		}

		/// <summary>
		/// Adds all of the elements from the appropriate enum type to this enum
		/// set, which is empty prior to the call.
		/// </summary>
		internal abstract void AddAll();

		/// <summary>
		/// Creates an enum set with the same element type as the specified enum
		/// set, initially containing the same elements (if any).
		/// </summary>
		/// @param <E> The class of the elements in the set </param>
		/// <param name="s"> the enum set from which to initialize this enum set </param>
		/// <returns> A copy of the specified enum set. </returns>
		/// <exception cref="NullPointerException"> if <tt>s</tt> is null </exception>
		public static EnumSet<E> copyOf<E>(EnumSet<E> s) where E : Enum<E>
		{
			return s.Clone();
		}

		/// <summary>
		/// Creates an enum set initialized from the specified collection.  If
		/// the specified collection is an <tt>EnumSet</tt> instance, this static
		/// factory method behaves identically to <seealso cref="#copyOf(EnumSet)"/>.
		/// Otherwise, the specified collection must contain at least one element
		/// (in order to determine the new enum set's element type).
		/// </summary>
		/// @param <E> The class of the elements in the collection </param>
		/// <param name="c"> the collection from which to initialize this enum set </param>
		/// <returns> An enum set initialized from the given collection. </returns>
		/// <exception cref="IllegalArgumentException"> if <tt>c</tt> is not an
		///     <tt>EnumSet</tt> instance and contains no elements </exception>
		/// <exception cref="NullPointerException"> if <tt>c</tt> is null </exception>
		public static EnumSet<E> copyOf<E>(Collection<E> c) where E : Enum<E>
		{
			if (c is EnumSet)
			{
				return ((EnumSet<E>)c).Clone();
			}
			else
			{
				if (c.Empty)
				{
					throw new IllegalArgumentException("Collection is empty");
				}
				Iterator<E> i = c.Iterator();
				E first = i.Next();
				EnumSet<E> result = EnumSet.Of(first);
				while (i.HasNext())
				{
					result.add(i.Next());
				}
				return result;
			}
		}

		/// <summary>
		/// Creates an enum set with the same element type as the specified enum
		/// set, initially containing all the elements of this type that are
		/// <i>not</i> contained in the specified set.
		/// </summary>
		/// @param <E> The class of the elements in the enum set </param>
		/// <param name="s"> the enum set from whose complement to initialize this enum set </param>
		/// <returns> The complement of the specified set in this set </returns>
		/// <exception cref="NullPointerException"> if <tt>s</tt> is null </exception>
		public static EnumSet<E> complementOf<E>(EnumSet<E> s) where E : Enum<E>
		{
			EnumSet<E> result = CopyOf(s);
			result.Complement();
			return result;
		}

		/// <summary>
		/// Creates an enum set initially containing the specified element.
		/// 
		/// Overloadings of this method exist to initialize an enum set with
		/// one through five elements.  A sixth overloading is provided that
		/// uses the varargs feature.  This overloading may be used to create
		/// an enum set initially containing an arbitrary number of elements, but
		/// is likely to run slower than the overloadings that do not use varargs.
		/// </summary>
		/// @param <E> The class of the specified element and of the set </param>
		/// <param name="e"> the element that this set is to contain initially </param>
		/// <exception cref="NullPointerException"> if <tt>e</tt> is null </exception>
		/// <returns> an enum set initially containing the specified element </returns>
		public static EnumSet<E> of<E>(E e) where E : Enum<E>
		{
			EnumSet<E> result = NoneOf(e.DeclaringClass);
			result.add(e);
			return result;
		}

		/// <summary>
		/// Creates an enum set initially containing the specified elements.
		/// 
		/// Overloadings of this method exist to initialize an enum set with
		/// one through five elements.  A sixth overloading is provided that
		/// uses the varargs feature.  This overloading may be used to create
		/// an enum set initially containing an arbitrary number of elements, but
		/// is likely to run slower than the overloadings that do not use varargs.
		/// </summary>
		/// @param <E> The class of the parameter elements and of the set </param>
		/// <param name="e1"> an element that this set is to contain initially </param>
		/// <param name="e2"> another element that this set is to contain initially </param>
		/// <exception cref="NullPointerException"> if any parameters are null </exception>
		/// <returns> an enum set initially containing the specified elements </returns>
		public static EnumSet<E> of<E>(E e1, E e2) where E : Enum<E>
		{
			EnumSet<E> result = NoneOf(e1.DeclaringClass);
			result.add(e1);
			result.add(e2);
			return result;
		}

		/// <summary>
		/// Creates an enum set initially containing the specified elements.
		/// 
		/// Overloadings of this method exist to initialize an enum set with
		/// one through five elements.  A sixth overloading is provided that
		/// uses the varargs feature.  This overloading may be used to create
		/// an enum set initially containing an arbitrary number of elements, but
		/// is likely to run slower than the overloadings that do not use varargs.
		/// </summary>
		/// @param <E> The class of the parameter elements and of the set </param>
		/// <param name="e1"> an element that this set is to contain initially </param>
		/// <param name="e2"> another element that this set is to contain initially </param>
		/// <param name="e3"> another element that this set is to contain initially </param>
		/// <exception cref="NullPointerException"> if any parameters are null </exception>
		/// <returns> an enum set initially containing the specified elements </returns>
		public static EnumSet<E> of<E>(E e1, E e2, E e3) where E : Enum<E>
		{
			EnumSet<E> result = NoneOf(e1.DeclaringClass);
			result.add(e1);
			result.add(e2);
			result.add(e3);
			return result;
		}

		/// <summary>
		/// Creates an enum set initially containing the specified elements.
		/// 
		/// Overloadings of this method exist to initialize an enum set with
		/// one through five elements.  A sixth overloading is provided that
		/// uses the varargs feature.  This overloading may be used to create
		/// an enum set initially containing an arbitrary number of elements, but
		/// is likely to run slower than the overloadings that do not use varargs.
		/// </summary>
		/// @param <E> The class of the parameter elements and of the set </param>
		/// <param name="e1"> an element that this set is to contain initially </param>
		/// <param name="e2"> another element that this set is to contain initially </param>
		/// <param name="e3"> another element that this set is to contain initially </param>
		/// <param name="e4"> another element that this set is to contain initially </param>
		/// <exception cref="NullPointerException"> if any parameters are null </exception>
		/// <returns> an enum set initially containing the specified elements </returns>
		public static EnumSet<E> of<E>(E e1, E e2, E e3, E e4) where E : Enum<E>
		{
			EnumSet<E> result = NoneOf(e1.DeclaringClass);
			result.add(e1);
			result.add(e2);
			result.add(e3);
			result.add(e4);
			return result;
		}

		/// <summary>
		/// Creates an enum set initially containing the specified elements.
		/// 
		/// Overloadings of this method exist to initialize an enum set with
		/// one through five elements.  A sixth overloading is provided that
		/// uses the varargs feature.  This overloading may be used to create
		/// an enum set initially containing an arbitrary number of elements, but
		/// is likely to run slower than the overloadings that do not use varargs.
		/// </summary>
		/// @param <E> The class of the parameter elements and of the set </param>
		/// <param name="e1"> an element that this set is to contain initially </param>
		/// <param name="e2"> another element that this set is to contain initially </param>
		/// <param name="e3"> another element that this set is to contain initially </param>
		/// <param name="e4"> another element that this set is to contain initially </param>
		/// <param name="e5"> another element that this set is to contain initially </param>
		/// <exception cref="NullPointerException"> if any parameters are null </exception>
		/// <returns> an enum set initially containing the specified elements </returns>
		public static EnumSet<E> of<E>(E e1, E e2, E e3, E e4, E e5) where E : Enum<E>
		{
			EnumSet<E> result = NoneOf(e1.DeclaringClass);
			result.add(e1);
			result.add(e2);
			result.add(e3);
			result.add(e4);
			result.add(e5);
			return result;
		}

		/// <summary>
		/// Creates an enum set initially containing the specified elements.
		/// This factory, whose parameter list uses the varargs feature, may
		/// be used to create an enum set initially containing an arbitrary
		/// number of elements, but it is likely to run slower than the overloadings
		/// that do not use varargs.
		/// </summary>
		/// @param <E> The class of the parameter elements and of the set </param>
		/// <param name="first"> an element that the set is to contain initially </param>
		/// <param name="rest"> the remaining elements the set is to contain initially </param>
		/// <exception cref="NullPointerException"> if any of the specified elements are null,
		///     or if <tt>rest</tt> is null </exception>
		/// <returns> an enum set initially containing the specified elements </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SafeVarargs public static <E extends Enum<E>> EnumSet<E> of(E first, E... rest)
		public static EnumSet<E> of<E>(E first, params E[] rest) where E : Enum<E>
		{
			EnumSet<E> result = NoneOf(first.DeclaringClass);
			result.add(first);
			foreach (E e in rest)
			{
				result.add(e);
			}
			return result;
		}

		/// <summary>
		/// Creates an enum set initially containing all of the elements in the
		/// range defined by the two specified endpoints.  The returned set will
		/// contain the endpoints themselves, which may be identical but must not
		/// be out of order.
		/// </summary>
		/// @param <E> The class of the parameter elements and of the set </param>
		/// <param name="from"> the first element in the range </param>
		/// <param name="to"> the last element in the range </param>
		/// <exception cref="NullPointerException"> if {@code from} or {@code to} are null </exception>
		/// <exception cref="IllegalArgumentException"> if {@code from.compareTo(to) > 0} </exception>
		/// <returns> an enum set initially containing all of the elements in the
		///         range defined by the two specified endpoints </returns>
		public static EnumSet<E> range<E>(E from, E to) where E : Enum<E>
		{
			if (from.CompareTo(to) > 0)
			{
				throw new IllegalArgumentException(from + " > " + to);
			}
			EnumSet<E> result = NoneOf(from.DeclaringClass);
			result.AddRange(from, to);
			return result;
		}

		/// <summary>
		/// Adds the specified range to this enum set, which is empty prior
		/// to the call.
		/// </summary>
		internal abstract void AddRange(E from, E to);

		/// <summary>
		/// Returns a copy of this set.
		/// </summary>
		/// <returns> a copy of this set </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public EnumSet<E> clone()
		public virtual EnumSet<E> Clone()
		{
			try
			{
				return (EnumSet<E>) base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				throw new AssertionError(e);
			}
		}

		/// <summary>
		/// Complements the contents of this enum set.
		/// </summary>
		internal abstract void Complement();

		/// <summary>
		/// Throws an exception if e is not of the correct type for this enum set.
		/// </summary>
		internal void TypeCheck(E e)
		{
			Class eClass = e.GetType();
			if (eClass != ElementType && eClass.BaseType != ElementType)
			{
				throw new ClassCastException(eClass + " != " + ElementType);
			}
		}

		/// <summary>
		/// Returns all of the values comprising E.
		/// The result is uncloned, cached, and shared by all callers.
		/// </summary>
		private static E[] getUniverse<E>(Class elementType) where E : Enum<E>
		{
			return SharedSecrets.JavaLangAccess.getEnumConstantsShared(elementType);
		}

		/// <summary>
		/// This class is used to serialize all EnumSet instances, regardless of
		/// implementation type.  It captures their "logical contents" and they
		/// are reconstructed using public static factories.  This is necessary
		/// to ensure that the existence of a particular implementation type is
		/// an implementation detail.
		/// 
		/// @serial include
		/// </summary>
		[Serializable]
		private class SerializationProxy <E> where E : Enum<E>
		{
			/// <summary>
			/// The element type of this enum set.
			/// 
			/// @serial
			/// </summary>
			internal readonly Class ElementType;

			/// <summary>
			/// The elements contained in this enum set.
			/// 
			/// @serial
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final Enum<?>[] elements;
			internal readonly Enum<?>[] Elements;

			internal SerializationProxy(EnumSet<E> set)
			{
				ElementType = set.ElementType;
				Elements = set.toArray(ZERO_LENGTH_ENUM_ARRAY);
			}

			// instead of cast to E, we should perhaps use elementType.cast()
			// to avoid injection of forged stream, but it will slow the implementation
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private Object readResolve()
			internal virtual Object ReadResolve()
			{
				EnumSet<E> result = EnumSet.NoneOf(ElementType);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Enum<?> e : elements)
				foreach (Enum<?> e in Elements)
				{
					result.add((E)e);
				}
				return result;
			}

			internal const long SerialVersionUID = 362491234563181265L;
		}

		internal virtual Object WriteReplace()
		{
			return new SerializationProxy<>(this);
		}

		// readObject method for the serialization proxy pattern
		// See Effective Java, Second Ed., Item 78.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.InvalidObjectException
		private void ReadObject(java.io.ObjectInputStream stream)
		{
			throw new java.io.InvalidObjectException("Proxy required");
		}
	}

}