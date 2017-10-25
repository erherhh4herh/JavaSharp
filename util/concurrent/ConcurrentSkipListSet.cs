using System;
using System.Collections.Generic;

/*
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

/*
 *
 *
 *
 *
 *
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/publicdomain/zero/1.0/
 */

namespace java.util.concurrent
{

	/// <summary>
	/// A scalable concurrent <seealso cref="NavigableSet"/> implementation based on
	/// a <seealso cref="ConcurrentSkipListMap"/>.  The elements of the set are kept
	/// sorted according to their <seealso cref="Comparable natural ordering"/>,
	/// or by a <seealso cref="Comparator"/> provided at set creation time, depending
	/// on which constructor is used.
	/// 
	/// <para>This implementation provides expected average <i>log(n)</i> time
	/// cost for the {@code contains}, {@code add}, and {@code remove}
	/// operations and their variants.  Insertion, removal, and access
	/// operations safely execute concurrently by multiple threads.
	/// 
	/// </para>
	/// <para>Iterators and spliterators are
	/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
	/// 
	/// </para>
	/// <para>Ascending ordered views and their iterators are faster than
	/// descending ones.
	/// 
	/// </para>
	/// <para>Beware that, unlike in most collections, the {@code size}
	/// method is <em>not</em> a constant-time operation. Because of the
	/// asynchronous nature of these sets, determining the current number
	/// of elements requires a traversal of the elements, and so may report
	/// inaccurate results if this collection is modified during traversal.
	/// Additionally, the bulk operations {@code addAll},
	/// {@code removeAll}, {@code retainAll}, {@code containsAll},
	/// {@code equals}, and {@code toArray} are <em>not</em> guaranteed
	/// to be performed atomically. For example, an iterator operating
	/// concurrently with an {@code addAll} operation might view only some
	/// of the added elements.
	/// 
	/// </para>
	/// <para>This class and its iterators implement all of the
	/// <em>optional</em> methods of the <seealso cref="Set"/> and <seealso cref="Iterator"/>
	/// interfaces. Like most other concurrent collection implementations,
	/// this class does not permit the use of {@code null} elements,
	/// because {@code null} arguments and return values cannot be reliably
	/// distinguished from the absence of elements.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <E> the type of elements maintained by this set
	/// @since 1.6 </param>
	[Serializable]
	public class ConcurrentSkipListSet<E> : AbstractSet<E>, NavigableSet<E>, Cloneable
	{

		private const long SerialVersionUID = -2479143111061671589L;

		/// <summary>
		/// The underlying map. Uses Boolean.TRUE as value for each
		/// element.  This field is declared final for the sake of thread
		/// safety, which entails some ugliness in clone().
		/// </summary>
		private readonly ConcurrentNavigableMap<E, Object> m;

		/// <summary>
		/// Constructs a new, empty set that orders its elements according to
		/// their <seealso cref="Comparable natural ordering"/>.
		/// </summary>
		public ConcurrentSkipListSet()
		{
			m = new ConcurrentSkipListMap<E, Object>();
		}

		/// <summary>
		/// Constructs a new, empty set that orders its elements according to
		/// the specified comparator.
		/// </summary>
		/// <param name="comparator"> the comparator that will be used to order this set.
		///        If {@code null}, the {@link Comparable natural
		///        ordering} of the elements will be used. </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public ConcurrentSkipListSet(java.util.Comparator<? base E> comparator)
		public ConcurrentSkipListSet<T1>(IComparer<T1> comparator)
		{
			m = new ConcurrentSkipListMap<E, Object>(comparator);
		}

		/// <summary>
		/// Constructs a new set containing the elements in the specified
		/// collection, that orders its elements according to their
		/// <seealso cref="Comparable natural ordering"/>.
		/// </summary>
		/// <param name="c"> The elements that will comprise the new set </param>
		/// <exception cref="ClassCastException"> if the elements in {@code c} are
		///         not <seealso cref="Comparable"/>, or are not mutually comparable </exception>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		public ConcurrentSkipListSet<T1>(ICollection<T1> c) where T1 : E
		{
			m = new ConcurrentSkipListMap<E, Object>();
			AddAll(c);
		}

		/// <summary>
		/// Constructs a new set containing the same elements and using the
		/// same ordering as the specified sorted set.
		/// </summary>
		/// <param name="s"> sorted set whose elements will comprise the new set </param>
		/// <exception cref="NullPointerException"> if the specified sorted set or any
		///         of its elements are null </exception>
		public ConcurrentSkipListSet(SortedSet<E> s)
		{
			m = new ConcurrentSkipListMap<E, Object>(s.Comparator());
			AddAll(s);
		}

		/// <summary>
		/// For use by submaps
		/// </summary>
		internal ConcurrentSkipListSet(ConcurrentNavigableMap<E, Object> m)
		{
			this.m = m;
		}

		/// <summary>
		/// Returns a shallow copy of this {@code ConcurrentSkipListSet}
		/// instance. (The elements themselves are not cloned.)
		/// </summary>
		/// <returns> a shallow copy of this set </returns>
		public virtual ConcurrentSkipListSet<E> Clone()
		{
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ConcurrentSkipListSet<E> clone = (ConcurrentSkipListSet<E>) base.clone();
				ConcurrentSkipListSet<E> clone = (ConcurrentSkipListSet<E>) base.Clone();
				clone.Map = new ConcurrentSkipListMap<E, Object>(m);
				return clone;
			}
			catch (CloneNotSupportedException)
			{
				throw new InternalError();
			}
		}

		/* ---------------- Set operations -------------- */

		/// <summary>
		/// Returns the number of elements in this set.  If this set
		/// contains more than {@code Integer.MAX_VALUE} elements, it
		/// returns {@code Integer.MAX_VALUE}.
		/// 
		/// <para>Beware that, unlike in most collections, this method is
		/// <em>NOT</em> a constant-time operation. Because of the
		/// asynchronous nature of these sets, determining the current
		/// number of elements requires traversing them all to count them.
		/// Additionally, it is possible for the size to change during
		/// execution of this method, in which case the returned result
		/// will be inaccurate. Thus, this method is typically not very
		/// useful in concurrent applications.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of elements in this set </returns>
		public virtual int Size()
		{
			return m.Size();
		}

		/// <summary>
		/// Returns {@code true} if this set contains no elements. </summary>
		/// <returns> {@code true} if this set contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return m.Empty;
			}
		}

		/// <summary>
		/// Returns {@code true} if this set contains the specified element.
		/// More formally, returns {@code true} if and only if this set
		/// contains an element {@code e} such that {@code o.equals(e)}.
		/// </summary>
		/// <param name="o"> object to be checked for containment in this set </param>
		/// <returns> {@code true} if this set contains the specified element </returns>
		/// <exception cref="ClassCastException"> if the specified element cannot be
		///         compared with the elements currently in this set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Contains(Object o)
		{
			return m.ContainsKey(o);
		}

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// More formally, adds the specified element {@code e} to this set if
		/// the set contains no element {@code e2} such that {@code e.equals(e2)}.
		/// If this set already contains the element, the call leaves the set
		/// unchanged and returns {@code false}.
		/// </summary>
		/// <param name="e"> element to be added to this set </param>
		/// <returns> {@code true} if this set did not already contain the
		///         specified element </returns>
		/// <exception cref="ClassCastException"> if {@code e} cannot be compared
		///         with the elements currently in this set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Add(E e)
		{
			return m.PutIfAbsent(e, true) == null;
		}

		/// <summary>
		/// Removes the specified element from this set if it is present.
		/// More formally, removes an element {@code e} such that
		/// {@code o.equals(e)}, if this set contains such an element.
		/// Returns {@code true} if this set contained the element (or
		/// equivalently, if this set changed as a result of the call).
		/// (This set will not contain the element once the call returns.)
		/// </summary>
		/// <param name="o"> object to be removed from this set, if present </param>
		/// <returns> {@code true} if this set contained the specified element </returns>
		/// <exception cref="ClassCastException"> if {@code o} cannot be compared
		///         with the elements currently in this set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual bool Remove(Object o)
		{
			return m.Remove(o, true);
		}

		/// <summary>
		/// Removes all of the elements from this set.
		/// </summary>
		public virtual void Clear()
		{
			m.Clear();
		}

		/// <summary>
		/// Returns an iterator over the elements in this set in ascending order.
		/// </summary>
		/// <returns> an iterator over the elements in this set in ascending order </returns>
		public virtual IEnumerator<E> Iterator()
		{
			return m.NavigableKeySet().Iterator();
		}

		/// <summary>
		/// Returns an iterator over the elements in this set in descending order.
		/// </summary>
		/// <returns> an iterator over the elements in this set in descending order </returns>
		public virtual IEnumerator<E> DescendingIterator()
		{
			return m.DescendingKeySet().Iterator();
		}


		/* ---------------- AbstractSet Overrides -------------- */

		/// <summary>
		/// Compares the specified object with this set for equality.  Returns
		/// {@code true} if the specified object is also a set, the two sets
		/// have the same size, and every member of the specified set is
		/// contained in this set (or equivalently, every member of this set is
		/// contained in the specified set).  This definition ensures that the
		/// equals method works properly across different implementations of the
		/// set interface.
		/// </summary>
		/// <param name="o"> the object to be compared for equality with this set </param>
		/// <returns> {@code true} if the specified object is equal to this set </returns>
		public override bool Equals(Object o)
		{
			// Override AbstractSet version to avoid calling size()
			if (o == this)
			{
				return true;
			}
			if (!(o is Set))
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Collection<?> c = (java.util.Collection<?>) o;
			ICollection<?> c = (ICollection<?>) o;
			try
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
				return ContainsAll(c) && c.containsAll(this);
			}
			catch (ClassCastException)
			{
				return false;
			}
			catch (NullPointerException)
			{
				return false;
			}
		}

		/// <summary>
		/// Removes from this set all of its elements that are contained in
		/// the specified collection.  If the specified collection is also
		/// a set, this operation effectively modifies this set so that its
		/// value is the <i>asymmetric set difference</i> of the two sets.
		/// </summary>
		/// <param name="c"> collection containing elements to be removed from this set </param>
		/// <returns> {@code true} if this set changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the types of one or more elements in this
		///         set are incompatible with the specified collection </exception>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///         of its elements are null </exception>
		public virtual bool removeAll<T1>(ICollection<T1> c)
		{
			// Override AbstractSet version to avoid unnecessary call to size()
			bool modified = false;
			foreach (Object e in c)
			{
				if (Remove(e))
				{
					modified = true;
				}
			}
			return modified;
		}

		/* ---------------- Relational operations -------------- */

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual E Lower(E e)
		{
			return m.LowerKey(e);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual E Floor(E e)
		{
			return m.FloorKey(e);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual E Ceiling(E e)
		{
			return m.CeilingKey(e);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null </exception>
		public virtual E Higher(E e)
		{
			return m.HigherKey(e);
		}

		public virtual E PollFirst()
		{
			java.util.Map_Entry<E, Object> e = m.PollFirstEntry();
			return (e == null) ? null : e.Key;
		}

		public virtual E PollLast()
		{
			java.util.Map_Entry<E, Object> e = m.PollLastEntry();
			return (e == null) ? null : e.Key;
		}


		/* ---------------- SortedSet operations -------------- */


//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public java.util.Comparator<? base E> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual IComparer<?> Comparator()
		{
			return m.Comparator();
		}

		/// <exception cref="java.util.NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E First()
		{
			return m.FirstKey();
		}

		/// <exception cref="java.util.NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Last()
		{
			return m.LastKey();
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} or
		///         {@code toElement} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual NavigableSet<E> SubSet(E fromElement, bool fromInclusive, E toElement, bool toInclusive)
		{
			return new ConcurrentSkipListSet<E> (m.SubMap(fromElement, fromInclusive, toElement, toInclusive));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code toElement} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual NavigableSet<E> HeadSet(E toElement, bool inclusive)
		{
			return new ConcurrentSkipListSet<E>(m.HeadMap(toElement, inclusive));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual NavigableSet<E> TailSet(E fromElement, bool inclusive)
		{
			return new ConcurrentSkipListSet<E>(m.TailMap(fromElement, inclusive));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} or
		///         {@code toElement} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual NavigableSet<E> SubSet(E fromElement, E toElement)
		{
			return SubSet(fromElement, true, toElement, false);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code toElement} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual NavigableSet<E> HeadSet(E toElement)
		{
			return HeadSet(toElement, false);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual NavigableSet<E> TailSet(E fromElement)
		{
			return TailSet(fromElement, true);
		}

		/// <summary>
		/// Returns a reverse order view of the elements contained in this set.
		/// The descending set is backed by this set, so changes to the set are
		/// reflected in the descending set, and vice-versa.
		/// 
		/// <para>The returned set has an ordering equivalent to
		/// <seealso cref="Collections#reverseOrder(Comparator) Collections.reverseOrder"/>{@code (comparator())}.
		/// The expression {@code s.descendingSet().descendingSet()} returns a
		/// view of {@code s} essentially equivalent to {@code s}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a reverse order view of this set </returns>
		public virtual NavigableSet<E> DescendingSet()
		{
			return new ConcurrentSkipListSet<E>(m.DescendingMap());
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator"/> over the elements in this set.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>,
		/// <seealso cref="Spliterator#NONNULL"/>, <seealso cref="Spliterator#DISTINCT"/>,
		/// <seealso cref="Spliterator#SORTED"/> and <seealso cref="Spliterator#ORDERED"/>, with an
		/// encounter order that is ascending order.  Overriding implementations
		/// should document the reporting of additional characteristic values.
		/// 
		/// </para>
		/// <para>The spliterator's comparator (see
		/// <seealso cref="java.util.Spliterator#getComparator()"/>) is {@code null} if
		/// the set's comparator (see <seealso cref="#comparator()"/>) is {@code null}.
		/// Otherwise, the spliterator's comparator is the same as or imposes the
		/// same total ordering as the set's comparator.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this set
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Spliterator<E> spliterator()
		public virtual Spliterator<E> Spliterator()
		{
			if (m is ConcurrentSkipListMap)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((ConcurrentSkipListMap<E,?>)m).keySpliterator();
				return ((ConcurrentSkipListMap<E, ?>)m).KeySpliterator();
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (java.util.Spliterator<E>)((ConcurrentSkipListMap.SubMap<E,?>)m).keyIterator();
				return (Spliterator<E>)((ConcurrentSkipListMap.SubMap<E, ?>)m).KeyIterator();
			}
		}

		// Support for resetting map in clone
		private ConcurrentNavigableMap<E, Object> Map
		{
			set
			{
				UNSAFE.putObjectVolatile(this, MapOffset, value);
			}
		}

		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long MapOffset;
		static ConcurrentSkipListSet()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class k = typeof(ConcurrentSkipListSet);
				MapOffset = UNSAFE.objectFieldOffset(k.GetDeclaredField("m"));
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}