using System;
using System.Collections.Generic;

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

namespace java.util
{

	/// <summary>
	/// A <seealso cref="NavigableSet"/> implementation based on a <seealso cref="TreeMap"/>.
	/// The elements are ordered using their {@link Comparable natural
	/// ordering}, or by a <seealso cref="Comparator"/> provided at set creation
	/// time, depending on which constructor is used.
	/// 
	/// <para>This implementation provides guaranteed log(n) time cost for the basic
	/// operations ({@code add}, {@code remove} and {@code contains}).
	/// 
	/// </para>
	/// <para>Note that the ordering maintained by a set (whether or not an explicit
	/// comparator is provided) must be <i>consistent with equals</i> if it is to
	/// correctly implement the {@code Set} interface.  (See {@code Comparable}
	/// or {@code Comparator} for a precise definition of <i>consistent with
	/// equals</i>.)  This is so because the {@code Set} interface is defined in
	/// terms of the {@code equals} operation, but a {@code TreeSet} instance
	/// performs all element comparisons using its {@code compareTo} (or
	/// {@code compare}) method, so two elements that are deemed equal by this method
	/// are, from the standpoint of the set, equal.  The behavior of a set
	/// <i>is</i> well-defined even if its ordering is inconsistent with equals; it
	/// just fails to obey the general contract of the {@code Set} interface.
	/// 
	/// </para>
	/// <para><strong>Note that this implementation is not synchronized.</strong>
	/// If multiple threads access a tree set concurrently, and at least one
	/// of the threads modifies the set, it <i>must</i> be synchronized
	/// externally.  This is typically accomplished by synchronizing on some
	/// object that naturally encapsulates the set.
	/// If no such object exists, the set should be "wrapped" using the
	/// <seealso cref="Collections#synchronizedSortedSet Collections.synchronizedSortedSet"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access to the set: <pre>
	///   SortedSet s = Collections.synchronizedSortedSet(new TreeSet(...));</pre>
	/// 
	/// </para>
	/// <para>The iterators returned by this class's {@code iterator} method are
	/// <i>fail-fast</i>: if the set is modified at any time after the iterator is
	/// created, in any way except through the iterator's own {@code remove}
	/// method, the iterator will throw a <seealso cref="ConcurrentModificationException"/>.
	/// Thus, in the face of concurrent modification, the iterator fails quickly
	/// and cleanly, rather than risking arbitrary, non-deterministic behavior at
	/// an undetermined time in the future.
	/// 
	/// </para>
	/// <para>Note that the fail-fast behavior of an iterator cannot be guaranteed
	/// as it is, generally speaking, impossible to make any hard guarantees in the
	/// presence of unsynchronized concurrent modification.  Fail-fast iterators
	/// throw {@code ConcurrentModificationException} on a best-effort basis.
	/// Therefore, it would be wrong to write a program that depended on this
	/// exception for its correctness:   <i>the fail-fast behavior of iterators
	/// should be used only to detect bugs.</i>
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// </para>
	/// </summary>
	/// @param <E> the type of elements maintained by this set
	/// 
	/// @author  Josh Bloch </param>
	/// <seealso cref=     Collection </seealso>
	/// <seealso cref=     Set </seealso>
	/// <seealso cref=     HashSet </seealso>
	/// <seealso cref=     Comparable </seealso>
	/// <seealso cref=     Comparator </seealso>
	/// <seealso cref=     TreeMap
	/// @since   1.2 </seealso>

	[Serializable]
	public class TreeSet<E> : AbstractSet<E>, NavigableSet<E>, Cloneable
	{
		/// <summary>
		/// The backing map.
		/// </summary>
		[NonSerialized]
		private NavigableMap<E, Object> m;

		// Dummy value to associate with an Object in the backing Map
		private static readonly Object PRESENT = new Object();

		/// <summary>
		/// Constructs a set backed by the specified navigable map.
		/// </summary>
		internal TreeSet(NavigableMap<E, Object> m)
		{
			this.m = m;
		}

		/// <summary>
		/// Constructs a new, empty tree set, sorted according to the
		/// natural ordering of its elements.  All elements inserted into
		/// the set must implement the <seealso cref="Comparable"/> interface.
		/// Furthermore, all such elements must be <i>mutually
		/// comparable</i>: {@code e1.compareTo(e2)} must not throw a
		/// {@code ClassCastException} for any elements {@code e1} and
		/// {@code e2} in the set.  If the user attempts to add an element
		/// to the set that violates this constraint (for example, the user
		/// attempts to add a string element to a set whose elements are
		/// integers), the {@code add} call will throw a
		/// {@code ClassCastException}.
		/// </summary>
		public TreeSet() : this(new TreeMap<E, Object>())
		{
		}

		/// <summary>
		/// Constructs a new, empty tree set, sorted according to the specified
		/// comparator.  All elements inserted into the set must be <i>mutually
		/// comparable</i> by the specified comparator: {@code comparator.compare(e1,
		/// e2)} must not throw a {@code ClassCastException} for any elements
		/// {@code e1} and {@code e2} in the set.  If the user attempts to add
		/// an element to the set that violates this constraint, the
		/// {@code add} call will throw a {@code ClassCastException}.
		/// </summary>
		/// <param name="comparator"> the comparator that will be used to order this set.
		///        If {@code null}, the {@link Comparable natural
		///        ordering} of the elements will be used. </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public TreeSet(Comparator<? base E> comparator)
		public TreeSet<T1>(Comparator<T1> comparator) : this(new TreeMap<>(comparator))
		{
		}

		/// <summary>
		/// Constructs a new tree set containing the elements in the specified
		/// collection, sorted according to the <i>natural ordering</i> of its
		/// elements.  All elements inserted into the set must implement the
		/// <seealso cref="Comparable"/> interface.  Furthermore, all such elements must be
		/// <i>mutually comparable</i>: {@code e1.compareTo(e2)} must not throw a
		/// {@code ClassCastException} for any elements {@code e1} and
		/// {@code e2} in the set.
		/// </summary>
		/// <param name="c"> collection whose elements will comprise the new set </param>
		/// <exception cref="ClassCastException"> if the elements in {@code c} are
		///         not <seealso cref="Comparable"/>, or are not mutually comparable </exception>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public TreeSet<T1>(Collection<T1> c) where T1 : E : this()
		{
			AddAll(c);
		}

		/// <summary>
		/// Constructs a new tree set containing the same elements and
		/// using the same ordering as the specified sorted set.
		/// </summary>
		/// <param name="s"> sorted set whose elements will comprise the new set </param>
		/// <exception cref="NullPointerException"> if the specified sorted set is null </exception>
		public TreeSet(SortedSet<E> s) : this(s.Comparator())
		{
			AddAll(s);
		}

		/// <summary>
		/// Returns an iterator over the elements in this set in ascending order.
		/// </summary>
		/// <returns> an iterator over the elements in this set in ascending order </returns>
		public virtual Iterator<E> Iterator()
		{
			return m.NavigableKeySet().Iterator();
		}

		/// <summary>
		/// Returns an iterator over the elements in this set in descending order.
		/// </summary>
		/// <returns> an iterator over the elements in this set in descending order
		/// @since 1.6 </returns>
		public virtual Iterator<E> DescendingIterator()
		{
			return m.DescendingKeySet().Iterator();
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual NavigableSet<E> DescendingSet()
		{
			return new TreeSet<>(m.DescendingMap());
		}

		/// <summary>
		/// Returns the number of elements in this set (its cardinality).
		/// </summary>
		/// <returns> the number of elements in this set (its cardinality) </returns>
		public virtual int Size()
		{
			return m.Size();
		}

		/// <summary>
		/// Returns {@code true} if this set contains no elements.
		/// </summary>
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
		/// contains an element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </summary>
		/// <param name="o"> object to be checked for containment in this set </param>
		/// <returns> {@code true} if this set contains the specified element </returns>
		/// <exception cref="ClassCastException"> if the specified object cannot be compared
		///         with the elements currently in the set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set uses natural ordering, or its comparator
		///         does not permit null elements </exception>
		public virtual bool Contains(Object o)
		{
			return m.ContainsKey(o);
		}

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// More formally, adds the specified element {@code e} to this set if
		/// the set contains no element {@code e2} such that
		/// <tt>(e==null&nbsp;?&nbsp;e2==null&nbsp;:&nbsp;e.equals(e2))</tt>.
		/// If this set already contains the element, the call leaves the set
		/// unchanged and returns {@code false}.
		/// </summary>
		/// <param name="e"> element to be added to this set </param>
		/// <returns> {@code true} if this set did not already contain the specified
		///         element </returns>
		/// <exception cref="ClassCastException"> if the specified object cannot be compared
		///         with the elements currently in this set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set uses natural ordering, or its comparator
		///         does not permit null elements </exception>
		public virtual bool Add(E e)
		{
			return m.Put(e, PRESENT) == null;
		}

		/// <summary>
		/// Removes the specified element from this set if it is present.
		/// More formally, removes an element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>,
		/// if this set contains such an element.  Returns {@code true} if
		/// this set contained the element (or equivalently, if this set
		/// changed as a result of the call).  (This set will not contain the
		/// element once the call returns.)
		/// </summary>
		/// <param name="o"> object to be removed from this set, if present </param>
		/// <returns> {@code true} if this set contained the specified element </returns>
		/// <exception cref="ClassCastException"> if the specified object cannot be compared
		///         with the elements currently in this set </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set uses natural ordering, or its comparator
		///         does not permit null elements </exception>
		public virtual bool Remove(Object o)
		{
			return m.Remove(o) == PRESENT;
		}

		/// <summary>
		/// Removes all of the elements from this set.
		/// The set will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			m.Clear();
		}

		/// <summary>
		/// Adds all of the elements in the specified collection to this set.
		/// </summary>
		/// <param name="c"> collection containing elements to be added to this set </param>
		/// <returns> {@code true} if this set changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the elements provided cannot be compared
		///         with the elements currently in the set </exception>
		/// <exception cref="NullPointerException"> if the specified collection is null or
		///         if any element is null and this set uses natural ordering, or
		///         its comparator does not permit null elements </exception>
		public virtual bool addAll<T1>(Collection<T1> c) where T1 : E
		{
			// Use linear-time version if applicable
			if (m.Size() == 0 && c.Size() > 0 && c is SortedSet && m is TreeMap)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: SortedSet<? extends E> set = (SortedSet<? extends E>) c;
				SortedSet<?> set = (SortedSet<?>) c;
				TreeMap<E, Object> map = (TreeMap<E, Object>) m;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Comparator<?> cc = set.comparator();
				Comparator<?> cc = set.Comparator();
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparator<? base E> mc = map.comparator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Comparator<?> mc = map.Comparator();
				if (cc == mc || (cc != null && cc.Equals(mc)))
				{
					map.AddAllForTreeSet(set, PRESENT);
					return true;
				}
			}
			return base.AddAll(c);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} or {@code toElement}
		///         is null and this set uses natural ordering, or its comparator
		///         does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc}
		/// @since 1.6 </exception>
		public virtual NavigableSet<E> SubSet(E fromElement, bool fromInclusive, E toElement, bool toInclusive)
		{
			return new TreeSet<>(m.SubMap(fromElement, fromInclusive, toElement, toInclusive));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code toElement} is null and
		///         this set uses natural ordering, or its comparator does
		///         not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc}
		/// @since 1.6 </exception>
		public virtual NavigableSet<E> HeadSet(E toElement, bool inclusive)
		{
			return new TreeSet<>(m.HeadMap(toElement, inclusive));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} is null and
		///         this set uses natural ordering, or its comparator does
		///         not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc}
		/// @since 1.6 </exception>
		public virtual NavigableSet<E> TailSet(E fromElement, bool inclusive)
		{
			return new TreeSet<>(m.TailMap(fromElement, inclusive));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} or
		///         {@code toElement} is null and this set uses natural ordering,
		///         or its comparator does not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual SortedSet<E> SubSet(E fromElement, E toElement)
		{
			return SubSet(fromElement, true, toElement, false);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code toElement} is null
		///         and this set uses natural ordering, or its comparator does
		///         not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual SortedSet<E> HeadSet(E toElement)
		{
			return HeadSet(toElement, false);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromElement} is null
		///         and this set uses natural ordering, or its comparator does
		///         not permit null elements </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual SortedSet<E> TailSet(E fromElement)
		{
			return TailSet(fromElement, true);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base E> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual Comparator<?> Comparator()
		{
			return m.Comparator();
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E First()
		{
			return m.FirstKey();
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual E Last()
		{
			return m.LastKey();
		}

		// NavigableSet API methods

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set uses natural ordering, or its comparator
		///         does not permit null elements
		/// @since 1.6 </exception>
		public virtual E Lower(E e)
		{
			return m.LowerKey(e);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set uses natural ordering, or its comparator
		///         does not permit null elements
		/// @since 1.6 </exception>
		public virtual E Floor(E e)
		{
			return m.FloorKey(e);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set uses natural ordering, or its comparator
		///         does not permit null elements
		/// @since 1.6 </exception>
		public virtual E Ceiling(E e)
		{
			return m.CeilingKey(e);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified element is null
		///         and this set uses natural ordering, or its comparator
		///         does not permit null elements
		/// @since 1.6 </exception>
		public virtual E Higher(E e)
		{
			return m.HigherKey(e);
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual E PollFirst()
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<E,?> e = m.pollFirstEntry();
			Map_Entry<E, ?> e = m.PollFirstEntry();
			return (e == null) ? null : e.Key;
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual E PollLast()
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<E,?> e = m.pollLastEntry();
			Map_Entry<E, ?> e = m.PollLastEntry();
			return (e == null) ? null : e.Key;
		}

		/// <summary>
		/// Returns a shallow copy of this {@code TreeSet} instance. (The elements
		/// themselves are not cloned.)
		/// </summary>
		/// <returns> a shallow copy of this set </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Object clone()
		public virtual Object Clone()
		{
			TreeSet<E> clone;
			try
			{
				clone = (TreeSet<E>) base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}

			clone.m = new TreeMap<>(m);
			return clone;
		}

		/// <summary>
		/// Save the state of the {@code TreeSet} instance to a stream (that is,
		/// serialize it).
		/// 
		/// @serialData Emits the comparator used to order this set, or
		///             {@code null} if it obeys its elements' natural ordering
		///             (Object), followed by the size of the set (the number of
		///             elements it contains) (int), followed by all of its
		///             elements (each an Object) in order (as determined by the
		///             set's Comparator, or by the elements' natural ordering if
		///             the set has no Comparator).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// Write out any hidden stuff
			s.DefaultWriteObject();

			// Write out Comparator
			s.WriteObject(m.Comparator());

			// Write out size
			s.WriteInt(m.Size());

			// Write out all elements in the proper order.
			foreach (E e in m.KeySet())
			{
				s.WriteObject(e);
			}
		}

		/// <summary>
		/// Reconstitute the {@code TreeSet} instance from a stream (that is,
		/// deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in any hidden stuff
			s.DefaultReadObject();

			// Read in Comparator
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Comparator<? base E> c = (Comparator<? base E>) s.readObject();
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Comparator<?> c = (Comparator<?>) s.ReadObject();

			// Create backing TreeMap
			TreeMap<E, Object> tm = new TreeMap<E, Object>(c);
			m = tm;

			// Read in size
			int size = s.ReadInt();

			tm.ReadTreeSet(size, s, PRESENT);
		}

		/// <summary>
		/// Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		/// and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		/// set.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/>,
		/// <seealso cref="Spliterator#DISTINCT"/>, <seealso cref="Spliterator#SORTED"/>, and
		/// <seealso cref="Spliterator#ORDERED"/>.  Overriding implementations should document
		/// the reporting of additional characteristic values.
		/// 
		/// </para>
		/// <para>The spliterator's comparator (see
		/// <seealso cref="java.util.Spliterator#getComparator()"/>) is {@code null} if
		/// the tree set's comparator (see <seealso cref="#comparator()"/>) is {@code null}.
		/// Otherwise, the spliterator's comparator is the same as or imposes the
		/// same total ordering as the tree set's comparator.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this set
		/// @since 1.8 </returns>
		public virtual Spliterator<E> Spliterator()
		{
			return TreeMap.KeySpliteratorFor(m);
		}

		private const long SerialVersionUID = -2479143000061671589L;
	}

}