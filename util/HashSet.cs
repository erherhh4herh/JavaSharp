using System;
using System.Collections.Generic;

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
	/// This class implements the <tt>Set</tt> interface, backed by a hash table
	/// (actually a <tt>HashMap</tt> instance).  It makes no guarantees as to the
	/// iteration order of the set; in particular, it does not guarantee that the
	/// order will remain constant over time.  This class permits the <tt>null</tt>
	/// element.
	/// 
	/// <para>This class offers constant time performance for the basic operations
	/// (<tt>add</tt>, <tt>remove</tt>, <tt>contains</tt> and <tt>size</tt>),
	/// assuming the hash function disperses the elements properly among the
	/// buckets.  Iterating over this set requires time proportional to the sum of
	/// the <tt>HashSet</tt> instance's size (the number of elements) plus the
	/// "capacity" of the backing <tt>HashMap</tt> instance (the number of
	/// buckets).  Thus, it's very important not to set the initial capacity too
	/// high (or the load factor too low) if iteration performance is important.
	/// 
	/// </para>
	/// <para><strong>Note that this implementation is not synchronized.</strong>
	/// If multiple threads access a hash set concurrently, and at least one of
	/// the threads modifies the set, it <i>must</i> be synchronized externally.
	/// This is typically accomplished by synchronizing on some object that
	/// naturally encapsulates the set.
	/// 
	/// If no such object exists, the set should be "wrapped" using the
	/// <seealso cref="Collections#synchronizedSet Collections.synchronizedSet"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access to the set:<pre>
	///   Set s = Collections.synchronizedSet(new HashSet(...));</pre>
	/// 
	/// </para>
	/// <para>The iterators returned by this class's <tt>iterator</tt> method are
	/// <i>fail-fast</i>: if the set is modified at any time after the iterator is
	/// created, in any way except through the iterator's own <tt>remove</tt>
	/// method, the Iterator throws a <seealso cref="ConcurrentModificationException"/>.
	/// Thus, in the face of concurrent modification, the iterator fails quickly
	/// and cleanly, rather than risking arbitrary, non-deterministic behavior at
	/// an undetermined time in the future.
	/// 
	/// </para>
	/// <para>Note that the fail-fast behavior of an iterator cannot be guaranteed
	/// as it is, generally speaking, impossible to make any hard guarantees in the
	/// presence of unsynchronized concurrent modification.  Fail-fast iterators
	/// throw <tt>ConcurrentModificationException</tt> on a best-effort basis.
	/// Therefore, it would be wrong to write a program that depended on this
	/// exception for its correctness: <i>the fail-fast behavior of iterators
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
	/// @author  Josh Bloch
	/// @author  Neal Gafter </param>
	/// <seealso cref=     Collection </seealso>
	/// <seealso cref=     Set </seealso>
	/// <seealso cref=     TreeSet </seealso>
	/// <seealso cref=     HashMap
	/// @since   1.2 </seealso>

	[Serializable]
	public class HashSet<E> : AbstractSet<E>, Set<E>, Cloneable
	{
		internal const long SerialVersionUID = -5024744406713321676L;

		[NonSerialized]
		private HashMap<E, Object> Map;

		// Dummy value to associate with an Object in the backing Map
		private static readonly Object PRESENT = new Object();

		/// <summary>
		/// Constructs a new, empty set; the backing <tt>HashMap</tt> instance has
		/// default initial capacity (16) and load factor (0.75).
		/// </summary>
		public HashSet()
		{
			Map = new HashMap<>();
		}

		/// <summary>
		/// Constructs a new set containing the elements in the specified
		/// collection.  The <tt>HashMap</tt> is created with default load factor
		/// (0.75) and an initial capacity sufficient to contain the elements in
		/// the specified collection.
		/// </summary>
		/// <param name="c"> the collection whose elements are to be placed into this set </param>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public HashSet<T1>(Collection<T1> c) where T1 : E
		{
			Map = new HashMap<>(System.Math.Max((int)(c.Size() / .75f) + 1, 16));
			AddAll(c);
		}

		/// <summary>
		/// Constructs a new, empty set; the backing <tt>HashMap</tt> instance has
		/// the specified initial capacity and the specified load factor.
		/// </summary>
		/// <param name="initialCapacity">   the initial capacity of the hash map </param>
		/// <param name="loadFactor">        the load factor of the hash map </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is less
		///             than zero, or if the load factor is nonpositive </exception>
		public HashSet(int initialCapacity, float loadFactor)
		{
			Map = new HashMap<>(initialCapacity, loadFactor);
		}

		/// <summary>
		/// Constructs a new, empty set; the backing <tt>HashMap</tt> instance has
		/// the specified initial capacity and default load factor (0.75).
		/// </summary>
		/// <param name="initialCapacity">   the initial capacity of the hash table </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is less
		///             than zero </exception>
		public HashSet(int initialCapacity)
		{
			Map = new HashMap<>(initialCapacity);
		}

		/// <summary>
		/// Constructs a new, empty linked hash set.  (This package private
		/// constructor is only used by LinkedHashSet.) The backing
		/// HashMap instance is a LinkedHashMap with the specified initial
		/// capacity and the specified load factor.
		/// </summary>
		/// <param name="initialCapacity">   the initial capacity of the hash map </param>
		/// <param name="loadFactor">        the load factor of the hash map </param>
		/// <param name="dummy">             ignored (distinguishes this
		///             constructor from other int, float constructor.) </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is less
		///             than zero, or if the load factor is nonpositive </exception>
		internal HashSet(int initialCapacity, float loadFactor, bool dummy)
		{
			Map = new LinkedHashMap<>(initialCapacity, loadFactor);
		}

		/// <summary>
		/// Returns an iterator over the elements in this set.  The elements
		/// are returned in no particular order.
		/// </summary>
		/// <returns> an Iterator over the elements in this set </returns>
		/// <seealso cref= ConcurrentModificationException </seealso>
		public virtual Iterator<E> GetEnumerator()
		{
			return Map.KeySet().Iterator();
		}

		/// <summary>
		/// Returns the number of elements in this set (its cardinality).
		/// </summary>
		/// <returns> the number of elements in this set (its cardinality) </returns>
		public virtual int Count
		{
			get
			{
				return Map.Size();
			}
		}

		/// <summary>
		/// Returns <tt>true</tt> if this set contains no elements.
		/// </summary>
		/// <returns> <tt>true</tt> if this set contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return Map.Empty;
			}
		}

		/// <summary>
		/// Returns <tt>true</tt> if this set contains the specified element.
		/// More formally, returns <tt>true</tt> if and only if this set
		/// contains an element <tt>e</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </summary>
		/// <param name="o"> element whose presence in this set is to be tested </param>
		/// <returns> <tt>true</tt> if this set contains the specified element </returns>
		public virtual bool Contains(Object o)
		{
			return Map.ContainsKey(o);
		}

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// More formally, adds the specified element <tt>e</tt> to this set if
		/// this set contains no element <tt>e2</tt> such that
		/// <tt>(e==null&nbsp;?&nbsp;e2==null&nbsp;:&nbsp;e.equals(e2))</tt>.
		/// If this set already contains the element, the call leaves the set
		/// unchanged and returns <tt>false</tt>.
		/// </summary>
		/// <param name="e"> element to be added to this set </param>
		/// <returns> <tt>true</tt> if this set did not already contain the specified
		/// element </returns>
		public virtual bool Add(E e)
		{
			return Map.Put(e, PRESENT) == null;
		}

		/// <summary>
		/// Removes the specified element from this set if it is present.
		/// More formally, removes an element <tt>e</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>,
		/// if this set contains such an element.  Returns <tt>true</tt> if
		/// this set contained the element (or equivalently, if this set
		/// changed as a result of the call).  (This set will not contain the
		/// element once the call returns.)
		/// </summary>
		/// <param name="o"> object to be removed from this set, if present </param>
		/// <returns> <tt>true</tt> if the set contained the specified element </returns>
		public virtual bool Remove(Object o)
		{
			return Map.Remove(o) == PRESENT;
		}

		/// <summary>
		/// Removes all of the elements from this set.
		/// The set will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			Map.Clear();
		}

		/// <summary>
		/// Returns a shallow copy of this <tt>HashSet</tt> instance: the elements
		/// themselves are not cloned.
		/// </summary>
		/// <returns> a shallow copy of this set </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Object clone()
		public virtual Object Clone()
		{
			try
			{
				HashSet<E> newSet = (HashSet<E>) base.Clone();
				newSet.Map = (HashMap<E, Object>) Map.Clone();
				return newSet;
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Save the state of this <tt>HashSet</tt> instance to a stream (that is,
		/// serialize it).
		/// 
		/// @serialData The capacity of the backing <tt>HashMap</tt> instance
		///             (int), and its load factor (float) are emitted, followed by
		///             the size of the set (the number of elements it contains)
		///             (int), followed by all of its elements (each an Object) in
		///             no particular order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// Write out any hidden serialization magic
			s.DefaultWriteObject();

			// Write out HashMap capacity and load factor
			s.WriteInt(Map.Capacity());
			s.WriteFloat(Map.LoadFactor());

			// Write out size
			s.WriteInt(Map.Size());

			// Write out all elements in the proper order.
			foreach (E e in Map.KeySet())
			{
				s.WriteObject(e);
			}
		}

		/// <summary>
		/// Reconstitute the <tt>HashSet</tt> instance from a stream (that is,
		/// deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in any hidden serialization magic
			s.DefaultReadObject();

			// Read capacity and verify non-negative.
			int capacity = s.ReadInt();
			if (capacity < 0)
			{
				throw new InvalidObjectException("Illegal capacity: " + capacity);
			}

			// Read load factor and verify positive and non NaN.
			float loadFactor = s.ReadFloat();
			if (loadFactor <= 0 || Float.IsNaN(loadFactor))
			{
				throw new InvalidObjectException("Illegal load factor: " + loadFactor);
			}

			// Read size and verify non-negative.
			int size = s.ReadInt();
			if (size < 0)
			{
				throw new InvalidObjectException("Illegal size: " + size);
			}

			// Set the capacity according to the size and load factor ensuring that
			// the HashMap is at least 25% full but clamping to maximum capacity.
			capacity = (int) System.Math.Min(size * System.Math.Min(1 / loadFactor, 4.0f), HashMap.MAXIMUM_CAPACITY);

			// Create backing HashMap
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: map = (((HashSet<?>)this) instanceof LinkedHashSet ? new LinkedHashMap<E,Object>(capacity, loadFactor) : new HashMap<E,Object>(capacity, loadFactor));
			Map = (((HashSet<?>)this) is LinkedHashSet ? new LinkedHashMap<E, Object>(capacity, loadFactor) : new HashMap<E, Object>(capacity, loadFactor));

			// Read in all elements in the proper order.
			for (int i = 0; i < size; i++)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") E e = (E) s.readObject();
				E e = (E) s.ReadObject();
				Map.Put(e, PRESENT);
			}
		}

		/// <summary>
		/// Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		/// and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		/// set.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/> and
		/// <seealso cref="Spliterator#DISTINCT"/>.  Overriding implementations should document
		/// the reporting of additional characteristic values.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this set
		/// @since 1.8 </returns>
		public virtual Spliterator<E> Spliterator()
		{
			return new HashMap.KeySpliterator<E, Object>(Map, 0, -1, 0, 0);
		}
	}

}