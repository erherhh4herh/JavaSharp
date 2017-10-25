using System;
using System.Collections.Generic;

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
	/// A Red-Black tree based <seealso cref="NavigableMap"/> implementation.
	/// The map is sorted according to the {@link Comparable natural
	/// ordering} of its keys, or by a <seealso cref="Comparator"/> provided at map
	/// creation time, depending on which constructor is used.
	/// 
	/// <para>This implementation provides guaranteed log(n) time cost for the
	/// {@code containsKey}, {@code get}, {@code put} and {@code remove}
	/// operations.  Algorithms are adaptations of those in Cormen, Leiserson, and
	/// Rivest's <em>Introduction to Algorithms</em>.
	/// 
	/// </para>
	/// <para>Note that the ordering maintained by a tree map, like any sorted map, and
	/// whether or not an explicit comparator is provided, must be <em>consistent
	/// with {@code equals}</em> if this sorted map is to correctly implement the
	/// {@code Map} interface.  (See {@code Comparable} or {@code Comparator} for a
	/// precise definition of <em>consistent with equals</em>.)  This is so because
	/// the {@code Map} interface is defined in terms of the {@code equals}
	/// operation, but a sorted map performs all key comparisons using its {@code
	/// compareTo} (or {@code compare}) method, so two keys that are deemed equal by
	/// this method are, from the standpoint of the sorted map, equal.  The behavior
	/// of a sorted map <em>is</em> well-defined even if its ordering is
	/// inconsistent with {@code equals}; it just fails to obey the general contract
	/// of the {@code Map} interface.
	/// 
	/// </para>
	/// <para><strong>Note that this implementation is not synchronized.</strong>
	/// If multiple threads access a map concurrently, and at least one of the
	/// threads modifies the map structurally, it <em>must</em> be synchronized
	/// externally.  (A structural modification is any operation that adds or
	/// deletes one or more mappings; merely changing the value associated
	/// with an existing key is not a structural modification.)  This is
	/// typically accomplished by synchronizing on some object that naturally
	/// encapsulates the map.
	/// If no such object exists, the map should be "wrapped" using the
	/// <seealso cref="Collections#synchronizedSortedMap Collections.synchronizedSortedMap"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access to the map: <pre>
	///   SortedMap m = Collections.synchronizedSortedMap(new TreeMap(...));</pre>
	/// 
	/// </para>
	/// <para>The iterators returned by the {@code iterator} method of the collections
	/// returned by all of this class's "collection view methods" are
	/// <em>fail-fast</em>: if the map is structurally modified at any time after
	/// the iterator is created, in any way except through the iterator's own
	/// {@code remove} method, the iterator will throw a {@link
	/// ConcurrentModificationException}.  Thus, in the face of concurrent
	/// modification, the iterator fails quickly and cleanly, rather than risking
	/// arbitrary, non-deterministic behavior at an undetermined time in the future.
	/// 
	/// </para>
	/// <para>Note that the fail-fast behavior of an iterator cannot be guaranteed
	/// as it is, generally speaking, impossible to make any hard guarantees in the
	/// presence of unsynchronized concurrent modification.  Fail-fast iterators
	/// throw {@code ConcurrentModificationException} on a best-effort basis.
	/// Therefore, it would be wrong to write a program that depended on this
	/// exception for its correctness:   <em>the fail-fast behavior of iterators
	/// should be used only to detect bugs.</em>
	/// 
	/// </para>
	/// <para>All {@code Map.Entry} pairs returned by methods in this class
	/// and its views represent snapshots of mappings at the time they were
	/// produced. They do <strong>not</strong> support the {@code Entry.setValue}
	/// method. (Note however that it is possible to change mappings in the
	/// associated map using {@code put}.)
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// </para>
	/// </summary>
	/// @param <K> the type of keys maintained by this map </param>
	/// @param <V> the type of mapped values
	/// 
	/// @author  Josh Bloch and Doug Lea </param>
	/// <seealso cref= Map </seealso>
	/// <seealso cref= HashMap </seealso>
	/// <seealso cref= Hashtable </seealso>
	/// <seealso cref= Comparable </seealso>
	/// <seealso cref= Comparator </seealso>
	/// <seealso cref= Collection
	/// @since 1.2 </seealso>

	[Serializable]
	public class TreeMap<K, V> : AbstractMap<K, V>, NavigableMap<K, V>, Cloneable
	{
		/// <summary>
		/// The comparator used to maintain order in this tree map, or
		/// null if it uses the natural ordering of its keys.
		/// 
		/// @serial
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private final Comparator<? base K> comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private readonly Comparator<?> Comparator_Renamed;

		[NonSerialized]
		private Entry<K, V> Root;

		/// <summary>
		/// The number of entries in the tree
		/// </summary>
		[NonSerialized]
		private int Size_Renamed = 0;

		/// <summary>
		/// The number of structural modifications to the tree.
		/// </summary>
		[NonSerialized]
		private int ModCount = 0;

		/// <summary>
		/// Constructs a new, empty tree map, using the natural ordering of its
		/// keys.  All keys inserted into the map must implement the {@link
		/// Comparable} interface.  Furthermore, all such keys must be
		/// <em>mutually comparable</em>: {@code k1.compareTo(k2)} must not throw
		/// a {@code ClassCastException} for any keys {@code k1} and
		/// {@code k2} in the map.  If the user attempts to put a key into the
		/// map that violates this constraint (for example, the user attempts to
		/// put a string key into a map whose keys are integers), the
		/// {@code put(Object key, Object value)} call will throw a
		/// {@code ClassCastException}.
		/// </summary>
		public TreeMap()
		{
			Comparator_Renamed = Map_Fields.Null;
		}

		/// <summary>
		/// Constructs a new, empty tree map, ordered according to the given
		/// comparator.  All keys inserted into the map must be <em>mutually
		/// comparable</em> by the given comparator: {@code comparator.compare(k1,
		/// k2)} must not throw a {@code ClassCastException} for any keys
		/// {@code k1} and {@code k2} in the map.  If the user attempts to put
		/// a key into the map that violates this constraint, the {@code put(Object
		/// key, Object value)} call will throw a
		/// {@code ClassCastException}.
		/// </summary>
		/// <param name="comparator"> the comparator that will be used to order this map.
		///        If {@code null}, the {@link Comparable natural
		///        ordering} of the keys will be used. </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public TreeMap(Comparator<? base K> comparator)
		public TreeMap<T1>(Comparator<T1> comparator)
		{
			this.Comparator_Renamed = comparator;
		}

		/// <summary>
		/// Constructs a new tree map containing the same mappings as the given
		/// map, ordered according to the <em>natural ordering</em> of its keys.
		/// All keys inserted into the new map must implement the {@link
		/// Comparable} interface.  Furthermore, all such keys must be
		/// <em>mutually comparable</em>: {@code k1.compareTo(k2)} must not throw
		/// a {@code ClassCastException} for any keys {@code k1} and
		/// {@code k2} in the map.  This method runs in n*log(n) time.
		/// </summary>
		/// <param name="m"> the map whose mappings are to be placed in this map </param>
		/// <exception cref="ClassCastException"> if the keys in m are not <seealso cref="Comparable"/>,
		///         or are not mutually comparable </exception>
		/// <exception cref="NullPointerException"> if the specified map is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public TreeMap(Map<? extends K, ? extends V> m)
		public TreeMap<T1>(Map<T1> m) where T1 : K where ? : V
		{
			Comparator_Renamed = Map_Fields.Null;
			PutAll(m);
		}

		/// <summary>
		/// Constructs a new tree map containing the same mappings and
		/// using the same ordering as the specified sorted map.  This
		/// method runs in linear time.
		/// </summary>
		/// <param name="m"> the sorted map whose mappings are to be placed in this map,
		///         and whose comparator is to be used to sort this map </param>
		/// <exception cref="NullPointerException"> if the specified map is null </exception>
		public TreeMap<T1>(SortedMap<T1> m) where T1 : V
		{
			Comparator_Renamed = m.Comparator();
			try
			{
				BuildFromSorted(m.Count, m.GetEnumerator(), Map_Fields.Null, Map_Fields.Null);
			}
			catch (java.io.IOException)
			{
			}
			catch (ClassNotFoundException)
			{
			}
		}


		// Query Operations

		/// <summary>
		/// Returns the number of key-value mappings in this map.
		/// </summary>
		/// <returns> the number of key-value mappings in this map </returns>
		public virtual int Size()
		{
			return Size_Renamed;
		}

		/// <summary>
		/// Returns {@code true} if this map contains a mapping for the specified
		/// key.
		/// </summary>
		/// <param name="key"> key whose presence in this map is to be tested </param>
		/// <returns> {@code true} if this map contains a mapping for the
		///         specified key </returns>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		public virtual bool ContainsKey(Object key)
		{
			return GetEntry(key) != Map_Fields.Null;
		}

		/// <summary>
		/// Returns {@code true} if this map maps one or more keys to the
		/// specified value.  More formally, returns {@code true} if and only if
		/// this map contains at least one mapping to a value {@code v} such
		/// that {@code (value==null ? v==null : value.equals(v))}.  This
		/// operation will probably require time linear in the map size for
		/// most implementations.
		/// </summary>
		/// <param name="value"> value whose presence in this map is to be tested </param>
		/// <returns> {@code true} if a mapping to {@code value} exists;
		///         {@code false} otherwise
		/// @since 1.2 </returns>
		public virtual bool ContainsValue(Object value)
		{
			for (Entry<K, V> e = FirstEntry; e != Map_Fields.Null; e = Successor(e))
			{
				if (ValEquals(value, e.Value_Renamed))
				{
					return Map_Fields.True;
				}
			}
			return Map_Fields.False;
		}

		/// <summary>
		/// Returns the value to which the specified key is mapped,
		/// or {@code null} if this map contains no mapping for the key.
		/// 
		/// <para>More formally, if this map contains a mapping from a key
		/// {@code k} to a value {@code v} such that {@code key} compares
		/// equal to {@code k} according to the map's ordering, then this
		/// method returns {@code v}; otherwise it returns {@code null}.
		/// (There can be at most one such mapping.)
		/// 
		/// </para>
		/// <para>A return value of {@code null} does not <em>necessarily</em>
		/// indicate that the map contains no mapping for the key; it's also
		/// possible that the map explicitly maps the key to {@code null}.
		/// The <seealso cref="#containsKey containsKey"/> operation may be used to
		/// distinguish these two cases.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		public virtual V Get(Object key)
		{
			Entry<K, V> p = GetEntry(key);
			return (p == Map_Fields.Null ? Map_Fields.Null : p.Value_Renamed);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual Comparator<?> Comparator()
		{
			return Comparator_Renamed;
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual K FirstKey()
		{
			return Key(FirstEntry);
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual K LastKey()
		{
			return Key(LastEntry);
		}

		/// <summary>
		/// Copies all of the mappings from the specified map to this map.
		/// These mappings replace any mappings that this map had for any
		/// of the keys currently in the specified map.
		/// </summary>
		/// <param name="map"> mappings to be stored in this map </param>
		/// <exception cref="ClassCastException"> if the class of a key or value in
		///         the specified map prevents it from being stored in this map </exception>
		/// <exception cref="NullPointerException"> if the specified map is null or
		///         the specified map contains a null key and this map does not
		///         permit null keys </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(Map<? extends K, ? extends V> map)
		public virtual void putAll<T1>(Map<T1> map) where T1 : K where ? : V
		{
			int mapSize = map.Size();
			if (Size_Renamed == 0 && mapSize != 0 && map is SortedMap)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Comparator<?> c = ((SortedMap<?,?>)map).comparator();
				Comparator<?> c = ((SortedMap<?, ?>)map).Comparator();
				if (c == Comparator_Renamed || (c != Map_Fields.Null && c.Equals(Comparator_Renamed)))
				{
					++ModCount;
					try
					{
						BuildFromSorted(mapSize, map.EntrySet().Iterator(), Map_Fields.Null, Map_Fields.Null);
					}
					catch (java.io.IOException)
					{
					}
					catch (ClassNotFoundException)
					{
					}
					return;
				}
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			base.PutAll(map);
		}

		/// <summary>
		/// Returns this map's entry for the given key, or {@code null} if the map
		/// does not contain an entry for the key.
		/// </summary>
		/// <returns> this map's entry for the given key, or {@code null} if the map
		///         does not contain an entry for the key </returns>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		internal Entry<K, V> GetEntry(Object key)
		{
			// Offload comparator-based version for sake of performance
			if (Comparator_Renamed != Map_Fields.Null)
			{
				return GetEntryUsingComparator(key);
			}
			if (key == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Comparable<? base K> Map_Fields.k = (Comparable<? base K>) key;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Comparable<?> Map_Fields.k = (Comparable<?>) key;
			Entry<K, V> p = Root;
			while (p != Map_Fields.Null)
			{
				int cmp = Map_Fields.k.CompareTo(p.Key_Renamed);
				if (cmp < 0)
				{
					p = p.Left;
				}
				else if (cmp > 0)
				{
					p = p.Right;
				}
				else
				{
					return p;
				}
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Version of getEntry using comparator. Split off from getEntry
		/// for performance. (This is not worth doing for most methods,
		/// that are less dependent on comparator performance, but is
		/// worthwhile here.)
		/// </summary>
		internal Entry<K, V> GetEntryUsingComparator(Object key)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K Map_Fields.k = (K) key;
			K Map_Fields.k = (K) key;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparator<? base K> cpr = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Comparator<?> cpr = Comparator_Renamed;
			if (cpr != Map_Fields.Null)
			{
				Entry<K, V> p = Root;
				while (p != Map_Fields.Null)
				{
					int cmp = cpr.Compare(Map_Fields.k, p.Key_Renamed);
					if (cmp < 0)
					{
						p = p.Left;
					}
					else if (cmp > 0)
					{
						p = p.Right;
					}
					else
					{
						return p;
					}
				}
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Gets the entry corresponding to the specified key; if no such entry
		/// exists, returns the entry for the least key greater than the specified
		/// key; if no such entry exists (i.e., the greatest key in the Tree is less
		/// than the specified key), returns {@code null}.
		/// </summary>
		internal Entry<K, V> GetCeilingEntry(K key)
		{
			Entry<K, V> p = Root;
			while (p != Map_Fields.Null)
			{
				int cmp = Compare(key, p.Key_Renamed);
				if (cmp < 0)
				{
					if (p.Left != Map_Fields.Null)
					{
						p = p.Left;
					}
					else
					{
						return p;
					}
				}
				else if (cmp > 0)
				{
					if (p.Right != Map_Fields.Null)
					{
						p = p.Right;
					}
					else
					{
						Entry<K, V> parent = p.Parent;
						Entry<K, V> ch = p;
						while (parent != Map_Fields.Null && ch == parent.Right)
						{
							ch = parent;
							parent = parent.Parent;
						}
						return parent;
					}
				}
				else
				{
					return p;
				}
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Gets the entry corresponding to the specified key; if no such entry
		/// exists, returns the entry for the greatest key less than the specified
		/// key; if no such entry exists, returns {@code null}.
		/// </summary>
		internal Entry<K, V> GetFloorEntry(K key)
		{
			Entry<K, V> p = Root;
			while (p != Map_Fields.Null)
			{
				int cmp = Compare(key, p.Key_Renamed);
				if (cmp > 0)
				{
					if (p.Right != Map_Fields.Null)
					{
						p = p.Right;
					}
					else
					{
						return p;
					}
				}
				else if (cmp < 0)
				{
					if (p.Left != Map_Fields.Null)
					{
						p = p.Left;
					}
					else
					{
						Entry<K, V> parent = p.Parent;
						Entry<K, V> ch = p;
						while (parent != Map_Fields.Null && ch == parent.Left)
						{
							ch = parent;
							parent = parent.Parent;
						}
						return parent;
					}
				}
				else
				{
					return p;
				}

			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Gets the entry for the least key greater than the specified
		/// key; if no such entry exists, returns the entry for the least
		/// key greater than the specified key; if no such entry exists
		/// returns {@code null}.
		/// </summary>
		internal Entry<K, V> GetHigherEntry(K key)
		{
			Entry<K, V> p = Root;
			while (p != Map_Fields.Null)
			{
				int cmp = Compare(key, p.Key_Renamed);
				if (cmp < 0)
				{
					if (p.Left != Map_Fields.Null)
					{
						p = p.Left;
					}
					else
					{
						return p;
					}
				}
				else
				{
					if (p.Right != Map_Fields.Null)
					{
						p = p.Right;
					}
					else
					{
						Entry<K, V> parent = p.Parent;
						Entry<K, V> ch = p;
						while (parent != Map_Fields.Null && ch == parent.Right)
						{
							ch = parent;
							parent = parent.Parent;
						}
						return parent;
					}
				}
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Returns the entry for the greatest key less than the specified key; if
		/// no such entry exists (i.e., the least key in the Tree is greater than
		/// the specified key), returns {@code null}.
		/// </summary>
		internal Entry<K, V> GetLowerEntry(K key)
		{
			Entry<K, V> p = Root;
			while (p != Map_Fields.Null)
			{
				int cmp = Compare(key, p.Key_Renamed);
				if (cmp > 0)
				{
					if (p.Right != Map_Fields.Null)
					{
						p = p.Right;
					}
					else
					{
						return p;
					}
				}
				else
				{
					if (p.Left != Map_Fields.Null)
					{
						p = p.Left;
					}
					else
					{
						Entry<K, V> parent = p.Parent;
						Entry<K, V> ch = p;
						while (parent != Map_Fields.Null && ch == parent.Left)
						{
							ch = parent;
							parent = parent.Parent;
						}
						return parent;
					}
				}
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Associates the specified value with the specified key in this map.
		/// If the map previously contained a mapping for the key, the old
		/// value is replaced.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="value"> value to be associated with the specified key
		/// </param>
		/// <returns> the previous value associated with {@code key}, or
		///         {@code null} if there was no mapping for {@code key}.
		///         (A {@code null} return can also indicate that the map
		///         previously associated {@code null} with {@code key}.) </returns>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		public virtual V Put(K key, V value)
		{
			Entry<K, V> t = Root;
			if (t == Map_Fields.Null)
			{
				Compare(key, key); // type (and possibly null) check

				Root = new Entry<>(key, value, Map_Fields.Null);
				Size_Renamed = 1;
				ModCount++;
				return Map_Fields.Null;
			}
			int cmp;
			Entry<K, V> parent;
			// split comparator and comparable paths
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: Comparator<? base K> cpr = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Comparator<?> cpr = Comparator_Renamed;
			if (cpr != Map_Fields.Null)
			{
				do
				{
					parent = t;
					cmp = cpr.Compare(key, t.Key_Renamed);
					if (cmp < 0)
					{
						t = t.Left;
					}
					else if (cmp > 0)
					{
						t = t.Right;
					}
					else
					{
						return t.setValue(value);
					}
				} while (t != Map_Fields.Null);
			}
			else
			{
				if (key == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Comparable<? base K> Map_Fields.k = (Comparable<? base K>) key;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Comparable<?> Map_Fields.k = (Comparable<?>) key;
				do
				{
					parent = t;
					cmp = Map_Fields.k.CompareTo(t.Key_Renamed);
					if (cmp < 0)
					{
						t = t.Left;
					}
					else if (cmp > 0)
					{
						t = t.Right;
					}
					else
					{
						return t.setValue(value);
					}
				} while (t != Map_Fields.Null);
			}
			Entry<K, V> e = new Entry<K, V>(key, value, parent);
			if (cmp < 0)
			{
				parent.Left = e;
			}
			else
			{
				parent.Right = e;
			}
			FixAfterInsertion(e);
			Size_Renamed++;
			ModCount++;
			return Map_Fields.Null;
		}

		/// <summary>
		/// Removes the mapping for this key from this TreeMap if present.
		/// </summary>
		/// <param name="key"> key for which mapping should be removed </param>
		/// <returns> the previous value associated with {@code key}, or
		///         {@code null} if there was no mapping for {@code key}.
		///         (A {@code null} return can also indicate that the map
		///         previously associated {@code null} with {@code key}.) </returns>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		public virtual V Remove(Object key)
		{
			Entry<K, V> p = GetEntry(key);
			if (p == Map_Fields.Null)
			{
				return Map_Fields.Null;
			}

			V Map_Fields.OldValue = p.Value_Renamed;
			DeleteEntry(p);
			return Map_Fields.OldValue;
		}

		/// <summary>
		/// Removes all of the mappings from this map.
		/// The map will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			ModCount++;
			Size_Renamed = 0;
			Root = Map_Fields.Null;
		}

		/// <summary>
		/// Returns a shallow copy of this {@code TreeMap} instance. (The keys and
		/// values themselves are not cloned.)
		/// </summary>
		/// <returns> a shallow copy of this map </returns>
		public virtual Object Clone()
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: TreeMap<?,?> clone;
			TreeMap<?, ?> clone;
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: clone = (TreeMap<?,?>) base.clone();
				clone = (TreeMap<?, ?>) base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}

			// Put clone into "virgin" state (except for comparator)
			clone.Root = Map_Fields.Null;
			clone.Size_Renamed = 0;
			clone.ModCount = 0;
			clone.EntrySet_Renamed = Map_Fields.Null;
			clone.NavigableKeySet_Renamed = Map_Fields.Null;
			clone.DescendingMap_Renamed = Map_Fields.Null;

			// Initialize clone with our mappings
			try
			{
				clone.BuildFromSorted(Size_Renamed, EntrySet().Iterator(), Map_Fields.Null, Map_Fields.Null);
			}
			catch (java.io.IOException)
			{
			}
			catch (ClassNotFoundException)
			{
			}

			return clone;
		}

		// NavigableMap API methods

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual Map_Entry<K, V> FirstEntry()
		{
			return ExportEntry(FirstEntry);
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual Map_Entry<K, V> LastEntry()
		{
			return ExportEntry(LastEntry);
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual Map_Entry<K, V> PollFirstEntry()
		{
			Entry<K, V> p = FirstEntry;
			Map_Entry<K, V> result = ExportEntry(p);
			if (p != Map_Fields.Null)
			{
				DeleteEntry(p);
			}
			return result;
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual Map_Entry<K, V> PollLastEntry()
		{
			Entry<K, V> p = LastEntry;
			Map_Entry<K, V> result = ExportEntry(p);
			if (p != Map_Fields.Null)
			{
				DeleteEntry(p);
			}
			return result;
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys
		/// @since 1.6 </exception>
		public virtual Map_Entry<K, V> LowerEntry(K key)
		{
			return ExportEntry(GetLowerEntry(key));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys
		/// @since 1.6 </exception>
		public virtual K LowerKey(K key)
		{
			return KeyOrNull(GetLowerEntry(key));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys
		/// @since 1.6 </exception>
		public virtual Map_Entry<K, V> FloorEntry(K key)
		{
			return ExportEntry(GetFloorEntry(key));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys
		/// @since 1.6 </exception>
		public virtual K FloorKey(K key)
		{
			return KeyOrNull(GetFloorEntry(key));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys
		/// @since 1.6 </exception>
		public virtual Map_Entry<K, V> CeilingEntry(K key)
		{
			return ExportEntry(GetCeilingEntry(key));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys
		/// @since 1.6 </exception>
		public virtual K CeilingKey(K key)
		{
			return KeyOrNull(GetCeilingEntry(key));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys
		/// @since 1.6 </exception>
		public virtual Map_Entry<K, V> HigherEntry(K key)
		{
			return ExportEntry(GetHigherEntry(key));
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys
		/// @since 1.6 </exception>
		public virtual K HigherKey(K key)
		{
			return KeyOrNull(GetHigherEntry(key));
		}

		// Views

		/// <summary>
		/// Fields initialized to contain an instance of the entry set view
		/// the first time this view is requested.  Views are stateless, so
		/// there's no reason to create more than one.
		/// </summary>
		[NonSerialized]
		private EntrySet EntrySet_Renamed;
		[NonSerialized]
		private KeySet<K> NavigableKeySet_Renamed;
		[NonSerialized]
		private NavigableMap<K, V> DescendingMap_Renamed;

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the keys contained in this map.
		/// 
		/// <para>The set's iterator returns the keys in ascending order.
		/// The set's spliterator is
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>,
		/// <em>fail-fast</em>, and additionally reports <seealso cref="Spliterator#SORTED"/>
		/// and <seealso cref="Spliterator#ORDERED"/> with an encounter order that is ascending
		/// key order.  The spliterator's comparator (see
		/// <seealso cref="java.util.Spliterator#getComparator()"/>) is {@code null} if
		/// the tree map's comparator (see <seealso cref="#comparator()"/>) is {@code null}.
		/// Otherwise, the spliterator's comparator is the same as or imposes the
		/// same total ordering as the tree map's comparator.
		/// 
		/// </para>
		/// <para>The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  If the map is modified
		/// while an iteration over the set is in progress (except through
		/// the iterator's own {@code remove} operation), the results of
		/// the iteration are undefined.  The set supports element removal,
		/// which removes the corresponding mapping from the map, via the
		/// {@code Iterator.remove}, {@code Set.remove},
		/// {@code removeAll}, {@code retainAll}, and {@code clear}
		/// operations.  It does not support the {@code add} or {@code addAll}
		/// operations.
		/// </para>
		/// </summary>
		public virtual Set<K> KeySet()
		{
			return NavigableKeySet();
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual NavigableSet<K> NavigableKeySet()
		{
			KeySet<K> nks = NavigableKeySet_Renamed;
			return (nks != Map_Fields.Null) ? nks : (NavigableKeySet_Renamed = new KeySet<>(this));
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual NavigableSet<K> DescendingKeySet()
		{
			return DescendingMap().NavigableKeySet();
		}

		/// <summary>
		/// Returns a <seealso cref="Collection"/> view of the values contained in this map.
		/// 
		/// <para>The collection's iterator returns the values in ascending order
		/// of the corresponding keys. The collection's spliterator is
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>,
		/// <em>fail-fast</em>, and additionally reports <seealso cref="Spliterator#ORDERED"/>
		/// with an encounter order that is ascending order of the corresponding
		/// keys.
		/// 
		/// </para>
		/// <para>The collection is backed by the map, so changes to the map are
		/// reflected in the collection, and vice-versa.  If the map is
		/// modified while an iteration over the collection is in progress
		/// (except through the iterator's own {@code remove} operation),
		/// the results of the iteration are undefined.  The collection
		/// supports element removal, which removes the corresponding
		/// mapping from the map, via the {@code Iterator.remove},
		/// {@code Collection.remove}, {@code removeAll},
		/// {@code retainAll} and {@code clear} operations.  It does not
		/// support the {@code add} or {@code addAll} operations.
		/// </para>
		/// </summary>
		public virtual Collection<V> Values()
		{
			Collection<V> vs = Values_Renamed;
			return (vs != Map_Fields.Null) ? vs : (Values_Renamed = new Values(this));
		}

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		/// 
		/// <para>The set's iterator returns the entries in ascending key order. The
		/// sets's spliterator is
		/// <em><a href="Spliterator.html#binding">late-binding</a></em>,
		/// <em>fail-fast</em>, and additionally reports <seealso cref="Spliterator#SORTED"/> and
		/// <seealso cref="Spliterator#ORDERED"/> with an encounter order that is ascending key
		/// order.
		/// 
		/// </para>
		/// <para>The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  If the map is modified
		/// while an iteration over the set is in progress (except through
		/// the iterator's own {@code remove} operation, or through the
		/// {@code setValue} operation on a map entry returned by the
		/// iterator) the results of the iteration are undefined.  The set
		/// supports element removal, which removes the corresponding
		/// mapping from the map, via the {@code Iterator.remove},
		/// {@code Set.remove}, {@code removeAll}, {@code retainAll} and
		/// {@code clear} operations.  It does not support the
		/// {@code add} or {@code addAll} operations.
		/// </para>
		/// </summary>
		public virtual Set<Map_Entry<K, V>> EntrySet()
		{
			EntrySet es = EntrySet_Renamed;
			return (es != Map_Fields.Null) ? es : (EntrySet_Renamed = new EntrySet(this));
		}

		/// <summary>
		/// @since 1.6
		/// </summary>
		public virtual NavigableMap<K, V> DescendingMap()
		{
			NavigableMap<K, V> km = DescendingMap_Renamed;
			return (km != Map_Fields.Null) ? km : (DescendingMap_Renamed = new DescendingSubMap<>(this, Map_Fields.True, Map_Fields.Null, Map_Fields.True, Map_Fields.True, Map_Fields.Null, Map_Fields.True));
		}

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromKey} or {@code toKey} is
		///         null and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc}
		/// @since 1.6 </exception>
		public virtual NavigableMap<K, V> SubMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive)
		{
			return new AscendingSubMap<>(this, Map_Fields.False, fromKey, fromInclusive, Map_Fields.False, toKey, toInclusive);
		}

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code toKey} is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc}
		/// @since 1.6 </exception>
		public virtual NavigableMap<K, V> HeadMap(K toKey, bool inclusive)
		{
			return new AscendingSubMap<>(this, Map_Fields.True, Map_Fields.Null, Map_Fields.True, Map_Fields.False, toKey, inclusive);
		}

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromKey} is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc}
		/// @since 1.6 </exception>
		public virtual NavigableMap<K, V> TailMap(K fromKey, bool inclusive)
		{
			return new AscendingSubMap<>(this, Map_Fields.False, fromKey, inclusive, Map_Fields.True, Map_Fields.Null, Map_Fields.True);
		}

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromKey} or {@code toKey} is
		///         null and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual SortedMap<K, V> SubMap(K fromKey, K toKey)
		{
			return SubMap(fromKey, Map_Fields.True, toKey, Map_Fields.False);
		}

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code toKey} is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual SortedMap<K, V> HeadMap(K toKey)
		{
			return HeadMap(toKey, Map_Fields.False);
		}

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromKey} is null
		///         and this map uses natural ordering, or its comparator
		///         does not permit null keys </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual SortedMap<K, V> TailMap(K fromKey)
		{
			return TailMap(fromKey, Map_Fields.True);
		}

		public override bool Replace(K key, V Map_Fields, V Map_Fields)
		{
			Entry<K, V> p = GetEntry(key);
			if (p != Map_Fields.Null && Objects.Equals(Map_Fields.OldValue, p.Value_Renamed))
			{
				p.Value_Renamed = Map_Fields.NewValue;
				return Map_Fields.True;
			}
			return Map_Fields.False;
		}

		public override V Replace(K key, V value)
		{
			Entry<K, V> p = GetEntry(key);
			if (p != Map_Fields.Null)
			{
				V Map_Fields.OldValue = p.Value_Renamed;
				p.Value_Renamed = value;
				return Map_Fields.OldValue;
			}
			return Map_Fields.Null;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
		public override void forEach<T1>(BiConsumer<T1> action)
		{
			Objects.RequireNonNull(action);
			int expectedModCount = ModCount;
			for (Entry<K, V> e = FirstEntry; e != Map_Fields.Null; e = Successor(e))
			{
				action.Accept(e.Key_Renamed, e.Value_Renamed);

				if (expectedModCount != ModCount)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
		public override void replaceAll<T1>(BiFunction<T1> function) where T1 : V
		{
			Objects.RequireNonNull(function);
			int expectedModCount = ModCount;

			for (Entry<K, V> e = FirstEntry; e != Map_Fields.Null; e = Successor(e))
			{
				e.Value_Renamed = function.Apply(e.Key_Renamed, e.Value_Renamed);

				if (expectedModCount != ModCount)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

		// View class support

		internal class Values : AbstractCollection<V>
		{
			private readonly TreeMap<K, V> OuterInstance;

			public Values(TreeMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<V> Iterator()
			{
				return new ValueIterator(OuterInstance, outerInstance.FirstEntry);
			}

			public virtual int Size()
			{
				return OuterInstance.Size();
			}

			public virtual bool Contains(Object o)
			{
				return OuterInstance.ContainsValue(o);
			}

			public virtual bool Remove(Object o)
			{
				for (Entry<K, V> e = outerInstance.FirstEntry; e != Map_Fields.Null; e = Successor(e))
				{
					if (ValEquals(e.Value, o))
					{
						outerInstance.DeleteEntry(e);
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}

			public virtual void Clear()
			{
				OuterInstance.Clear();
			}

			public virtual Spliterator<V> Spliterator()
			{
				return new ValueSpliterator<K, V>(OuterInstance, Map_Fields.Null, Map_Fields.Null, 0, -1, 0);
			}
		}

		internal class EntrySet : AbstractSet<Map_Entry<K, V>>
		{
			private readonly TreeMap<K, V> OuterInstance;

			public EntrySet(TreeMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<Map_Entry<K, V>> Iterator()
			{
				return new EntryIterator(OuterInstance, outerInstance.FirstEntry);
			}

			public virtual bool Contains(Object o)
			{
				if (!(o is Map_Entry))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> entry = (Map_Entry<?,?>) o;
				Map_Entry<?, ?> entry = (Map_Entry<?, ?>) o;
				Object value = entry.Value;
				Entry<K, V> p = outerInstance.GetEntry(entry.Key);
				return p != Map_Fields.Null && ValEquals(p.Value, value);
			}

			public virtual bool Remove(Object o)
			{
				if (!(o is Map_Entry))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> entry = (Map_Entry<?,?>) o;
				Map_Entry<?, ?> entry = (Map_Entry<?, ?>) o;
				Object value = entry.Value;
				Entry<K, V> p = outerInstance.GetEntry(entry.Key);
				if (p != Map_Fields.Null && ValEquals(p.Value, value))
				{
					outerInstance.DeleteEntry(p);
					return Map_Fields.True;
				}
				return Map_Fields.False;
			}

			public virtual int Size()
			{
				return OuterInstance.Size();
			}

			public virtual void Clear()
			{
				OuterInstance.Clear();
			}

			public virtual Spliterator<Map_Entry<K, V>> Spliterator()
			{
				return new EntrySpliterator<K, V>(OuterInstance, Map_Fields.Null, Map_Fields.Null, 0, -1, 0);
			}
		}

		/*
		 * Unlike Values and EntrySet, the KeySet class is static,
		 * delegating to a NavigableMap to allow use by SubMaps, which
		 * outweighs the ugliness of needing type-tests for the following
		 * Iterator methods that are defined appropriately in main versus
		 * submap classes.
		 */

		internal virtual Iterator<K> KeyIterator()
		{
			return new KeyIterator(this, FirstEntry);
		}

		internal virtual Iterator<K> DescendingKeyIterator()
		{
			return new DescendingKeyIterator(this, LastEntry);
		}

		internal sealed class KeySet<E> : AbstractSet<E>, NavigableSet<E>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final NavigableMap<E, ?> m;
			internal readonly NavigableMap<E, ?> m;
			internal KeySet<T1>(NavigableMap<T1> map)
			{
				m = map;
			}

			public Iterator<E> Iterator()
			{
				if (m is TreeMap)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((TreeMap<E,?>)m).keyIterator();
					return ((TreeMap<E, ?>)m).KeyIterator();
				}
				else
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((TreeMap.NavigableSubMap<E,?>)m).keyIterator();
					return ((TreeMap.NavigableSubMap<E, ?>)m).KeyIterator();
				}
			}

			public Iterator<E> DescendingIterator()
			{
				if (m is TreeMap)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((TreeMap<E,?>)m).descendingKeyIterator();
					return ((TreeMap<E, ?>)m).DescendingKeyIterator();
				}
				else
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((TreeMap.NavigableSubMap<E,?>)m).descendingKeyIterator();
					return ((TreeMap.NavigableSubMap<E, ?>)m).DescendingKeyIterator();
				}
			}

			public int Size()
			{
				return m.Size();
			}
			public bool Empty
			{
				get
				{
					return m.Empty;
				}
			}
			public bool Contains(Object o)
			{
				return m.ContainsKey(o);
			}
			public void Clear()
			{
				m.Clear();
			}
			public E Lower(E e)
			{
				return m.LowerKey(e);
			}
			public E Floor(E e)
			{
				return m.FloorKey(e);
			}
			public E Ceiling(E e)
			{
				return m.CeilingKey(e);
			}
			public E Higher(E e)
			{
				return m.HigherKey(e);
			}
			public E First()
			{
				return m.FirstKey();
			}
			public E Last()
			{
				return m.LastKey();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base E> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> Comparator()
			{
				return m.Comparator();
			}
			public E PollFirst()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<E,?> e = m.pollFirstEntry();
				Map_Entry<E, ?> e = m.PollFirstEntry();
				return (e == Map_Fields.Null) ? Map_Fields.Null : e.Key;
			}
			public E PollLast()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<E,?> e = m.pollLastEntry();
				Map_Entry<E, ?> e = m.PollLastEntry();
				return (e == Map_Fields.Null) ? Map_Fields.Null : e.Key;
			}
			public bool Remove(Object o)
			{
				int oldSize = Size();
				m.Remove(o);
				return Size() != oldSize;
			}
			public NavigableSet<E> SubSet(E fromElement, bool fromInclusive, E toElement, bool toInclusive)
			{
				return new KeySet<>(m.SubMap(fromElement, fromInclusive, toElement, toInclusive));
			}
			public NavigableSet<E> HeadSet(E toElement, bool inclusive)
			{
				return new KeySet<>(m.HeadMap(toElement, inclusive));
			}
			public NavigableSet<E> TailSet(E fromElement, bool inclusive)
			{
				return new KeySet<>(m.TailMap(fromElement, inclusive));
			}
			public SortedSet<E> SubSet(E fromElement, E toElement)
			{
				return SubSet(fromElement, Map_Fields.True, toElement, Map_Fields.False);
			}
			public SortedSet<E> HeadSet(E toElement)
			{
				return HeadSet(toElement, Map_Fields.False);
			}
			public SortedSet<E> TailSet(E fromElement)
			{
				return TailSet(fromElement, Map_Fields.True);
			}
			public NavigableSet<E> DescendingSet()
			{
				return new KeySet<>(m.DescendingMap());
			}

			public Spliterator<E> Spliterator()
			{
				return KeySpliteratorFor(m);
			}
		}

		/// <summary>
		/// Base class for TreeMap Iterators
		/// </summary>
		internal abstract class PrivateEntryIterator<T> : Iterator<T>
		{
			private readonly TreeMap<K, V> OuterInstance;

			internal Entry<K, V> Next;
			internal Entry<K, V> LastReturned;
			internal int ExpectedModCount;

			internal PrivateEntryIterator(TreeMap<K, V> outerInstance, Entry<K, V> first)
			{
				this.OuterInstance = outerInstance;
				ExpectedModCount = outerInstance.ModCount;
				LastReturned = Map_Fields.Null;
				Next = first;
			}

			public bool HasNext()
			{
				return Next != Map_Fields.Null;
			}

			internal Entry<K, V> NextEntry()
			{
				Entry<K, V> e = Next;
				if (e == Map_Fields.Null)
				{
					throw new NoSuchElementException();
				}
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				Next = Successor(e);
				LastReturned = e;
				return e;
			}

			internal Entry<K, V> PrevEntry()
			{
				Entry<K, V> e = Next;
				if (e == Map_Fields.Null)
				{
					throw new NoSuchElementException();
				}
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				Next = Predecessor(e);
				LastReturned = e;
				return e;
			}

			public virtual void Remove()
			{
				if (LastReturned == Map_Fields.Null)
				{
					throw new IllegalStateException();
				}
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				// deleted entries are replaced by their successors
				if (LastReturned.Left != Map_Fields.Null && LastReturned.Right != Map_Fields.Null)
				{
					Next = LastReturned;
				}
				outerInstance.DeleteEntry(LastReturned);
				ExpectedModCount = outerInstance.ModCount;
				LastReturned = Map_Fields.Null;
			}
		}

		internal sealed class EntryIterator : PrivateEntryIterator<Map_Entry<K, V>>
		{
			private readonly TreeMap<K, V> OuterInstance;

			internal EntryIterator(TreeMap<K, V> outerInstance, Entry<K, V> first) : base(outerInstance, first)
			{
				this.OuterInstance = outerInstance;
			}
			public Map_Entry<K, V> Next()
			{
				return NextEntry();
			}
		}

		internal sealed class ValueIterator : PrivateEntryIterator<V>
		{
			private readonly TreeMap<K, V> OuterInstance;

			internal ValueIterator(TreeMap<K, V> outerInstance, Entry<K, V> first) : base(outerInstance, first)
			{
				this.OuterInstance = outerInstance;
			}
			public V Next()
			{
				return NextEntry().Value_Renamed;
			}
		}

		internal sealed class KeyIterator : PrivateEntryIterator<K>
		{
			private readonly TreeMap<K, V> OuterInstance;

			internal KeyIterator(TreeMap<K, V> outerInstance, Entry<K, V> first) : base(outerInstance, first)
			{
				this.OuterInstance = outerInstance;
			}
			public K Next()
			{
				return NextEntry().Key_Renamed;
			}
		}

		internal sealed class DescendingKeyIterator : PrivateEntryIterator<K>
		{
			private readonly TreeMap<K, V> OuterInstance;

			internal DescendingKeyIterator(TreeMap<K, V> outerInstance, Entry<K, V> first) : base(outerInstance, first)
			{
				this.OuterInstance = outerInstance;
			}
			public K Next()
			{
				return PrevEntry().Key_Renamed;
			}
			public void Remove()
			{
				if (LastReturned == Map_Fields.Null)
				{
					throw new IllegalStateException();
				}
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				outerInstance.DeleteEntry(LastReturned);
				LastReturned = Map_Fields.Null;
				ExpectedModCount = outerInstance.ModCount;
			}
		}

		// Little utilities

		/// <summary>
		/// Compares two keys using the correct comparison method for this TreeMap.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final int compare(Object k1, Object k2)
		internal int Compare(Object k1, Object k2)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: return comparator==Map_Fields.null ? ((Comparable<? base K>)k1).compareTo((K)k2) : comparator.compare((K)k1, (K)k2);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			return Comparator_Renamed == Map_Fields.Null ? ((Comparable<?>)k1).CompareTo((K)k2) : Comparator_Renamed.Compare((K)k1, (K)k2);
		}

		/// <summary>
		/// Test two values for equality.  Differs from o1.equals(o2) only in
		/// that it copes with {@code null} o1 properly.
		/// </summary>
		internal static bool ValEquals(Object o1, Object o2)
		{
			return (o1 == Map_Fields.Null ? o2 == Map_Fields.Null : o1.Equals(o2));
		}

		/// <summary>
		/// Return SimpleImmutableEntry for entry, or null if null
		/// </summary>
		internal static Map_Entry<K, V> exportEntry<K, V>(TreeMap.Entry<K, V> e)
		{
			return (e == Map_Fields.Null) ? Map_Fields.Null : new AbstractMap.SimpleImmutableEntry<>(e);
		}

		/// <summary>
		/// Return key for entry, or null if null
		/// </summary>
		internal static K keyOrNull<K, V>(TreeMap.Entry<K, V> e)
		{
			return (e == Map_Fields.Null) ? Map_Fields.Null : e.Key_Renamed;
		}

		/// <summary>
		/// Returns the key corresponding to the specified Entry. </summary>
		/// <exception cref="NoSuchElementException"> if the Entry is null </exception>
		internal static K key<K, T1>(Entry<T1> e)
		{
			if (e == Map_Fields.Null)
			{
				throw new NoSuchElementException();
			}
			return e.Key_Renamed;
		}


		// SubMaps

		/// <summary>
		/// Dummy value serving as unmatchable fence key for unbounded
		/// SubMapIterators
		/// </summary>
		private static readonly Object UNBOUNDED = new Object();

		/// <summary>
		/// @serial include
		/// </summary>
		[Serializable]
		internal abstract class NavigableSubMap<K, V> : AbstractMap<K, V>, NavigableMap<K, V>
		{
			internal const long SerialVersionUID = -2102997345730753016L;
			/// <summary>
			/// The backing map.
			/// </summary>
			internal readonly TreeMap<K, V> m;

			/// <summary>
			/// Endpoints are represented as triples (fromStart, lo,
			/// loInclusive) and (toEnd, hi, hiInclusive). If fromStart is
			/// true, then the low (absolute) bound is the start of the
			/// backing map, and the other values are ignored. Otherwise,
			/// if loInclusive is true, lo is the inclusive bound, else lo
			/// is the exclusive bound. Similarly for the upper bound.
			/// </summary>
			internal readonly K Lo, Hi;
			internal readonly bool FromStart, ToEnd;
			internal readonly bool LoInclusive, HiInclusive;

			internal NavigableSubMap(TreeMap<K, V> m, bool fromStart, K lo, bool loInclusive, bool toEnd, K hi, bool hiInclusive)
			{
				if (!fromStart && !toEnd)
				{
					if (m.Compare(lo, hi) > 0)
					{
						throw new IllegalArgumentException("fromKey > toKey");
					}
				}
				else
				{
					if (!fromStart) // type check
					{
						m.Compare(lo, lo);
					}
					if (!toEnd)
					{
						m.Compare(hi, hi);
					}
				}

				this.m = m;
				this.FromStart = fromStart;
				this.Lo = lo;
				this.LoInclusive = loInclusive;
				this.ToEnd = toEnd;
				this.Hi = hi;
				this.HiInclusive = hiInclusive;
			}

			// internal utilities

			internal bool TooLow(Object key)
			{
				if (!FromStart)
				{
					int c = m.Compare(key, Lo);
					if (c < 0 || (c == 0 && !LoInclusive))
					{
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}

			internal bool TooHigh(Object key)
			{
				if (!ToEnd)
				{
					int c = m.Compare(key, Hi);
					if (c > 0 || (c == 0 && !HiInclusive))
					{
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}

			internal bool InRange(Object key)
			{
				return !TooLow(key) && !TooHigh(key);
			}

			internal bool InClosedRange(Object key)
			{
				return (FromStart || m.Compare(key, Lo) >= 0) && (ToEnd || m.Compare(Hi, key) >= 0);
			}

			internal bool InRange(Object key, bool inclusive)
			{
				return inclusive ? InRange(key) : InClosedRange(key);
			}

			/*
			 * Absolute versions of relation operations.
			 * Subclasses map to these using like-named "sub"
			 * versions that invert senses for descending maps
			 */

			internal TreeMap.Entry<K, V> AbsLowest()
			{
				TreeMap.Entry<K, V> e = (FromStart ? m.FirstEntry : (LoInclusive ? m.GetCeilingEntry(Lo) : m.GetHigherEntry(Lo)));
				return (e == Map_Fields.Null || TooHigh(e.Key_Renamed)) ? Map_Fields.Null : e;
			}

			internal TreeMap.Entry<K, V> AbsHighest()
			{
				TreeMap.Entry<K, V> e = (ToEnd ? m.LastEntry : (HiInclusive ? m.GetFloorEntry(Hi) : m.GetLowerEntry(Hi)));
				return (e == Map_Fields.Null || TooLow(e.Key_Renamed)) ? Map_Fields.Null : e;
			}

			internal TreeMap.Entry<K, V> AbsCeiling(K key)
			{
				if (TooLow(key))
				{
					return AbsLowest();
				}
				TreeMap.Entry<K, V> e = m.GetCeilingEntry(key);
				return (e == Map_Fields.Null || TooHigh(e.Key_Renamed)) ? Map_Fields.Null : e;
			}

			internal TreeMap.Entry<K, V> AbsHigher(K key)
			{
				if (TooLow(key))
				{
					return AbsLowest();
				}
				TreeMap.Entry<K, V> e = m.GetHigherEntry(key);
				return (e == Map_Fields.Null || TooHigh(e.Key_Renamed)) ? Map_Fields.Null : e;
			}

			internal TreeMap.Entry<K, V> AbsFloor(K key)
			{
				if (TooHigh(key))
				{
					return AbsHighest();
				}
				TreeMap.Entry<K, V> e = m.GetFloorEntry(key);
				return (e == Map_Fields.Null || TooLow(e.Key_Renamed)) ? Map_Fields.Null : e;
			}

			internal TreeMap.Entry<K, V> AbsLower(K key)
			{
				if (TooHigh(key))
				{
					return AbsHighest();
				}
				TreeMap.Entry<K, V> e = m.GetLowerEntry(key);
				return (e == Map_Fields.Null || TooLow(e.Key_Renamed)) ? Map_Fields.Null : e;
			}

			/// <summary>
			/// Returns the absolute high fence for ascending traversal </summary>
			internal TreeMap.Entry<K, V> AbsHighFence()
			{
				return (ToEnd ? Map_Fields.Null : (HiInclusive ? m.GetHigherEntry(Hi) : m.GetCeilingEntry(Hi)));
			}

			/// <summary>
			/// Return the absolute low fence for descending traversal </summary>
			internal TreeMap.Entry<K, V> AbsLowFence()
			{
				return (FromStart ? Map_Fields.Null : (LoInclusive ? m.GetLowerEntry(Lo) : m.GetFloorEntry(Lo)));
			}

			// Abstract methods defined in ascending vs descending classes
			// These relay to the appropriate absolute versions

			internal abstract TreeMap.Entry<K, V> SubLowest();
			internal abstract TreeMap.Entry<K, V> SubHighest();
			internal abstract TreeMap.Entry<K, V> SubCeiling(K key);
			internal abstract TreeMap.Entry<K, V> SubHigher(K key);
			internal abstract TreeMap.Entry<K, V> SubFloor(K key);
			internal abstract TreeMap.Entry<K, V> SubLower(K key);

			/// <summary>
			/// Returns ascending iterator from the perspective of this submap </summary>
			internal abstract Iterator<K> KeyIterator();

			internal abstract Spliterator<K> KeySpliterator();

			/// <summary>
			/// Returns descending iterator from the perspective of this submap </summary>
			internal abstract Iterator<K> DescendingKeyIterator();

			// public methods

			public virtual bool Empty
			{
				get
				{
					return (FromStart && ToEnd) ? m.Empty : EntrySet().Count == 0;
				}
			}

			public virtual int Size()
			{
				return (FromStart && ToEnd) ? m.Size() : EntrySet().Count;
			}

			public bool ContainsKey(Object key)
			{
				return InRange(key) && m.ContainsKey(key);
			}

			public V Put(K key, V value)
			{
				if (!InRange(key))
				{
					throw new IllegalArgumentException("key out of range");
				}
				return m.Put(key, value);
			}

			public V Get(Object key)
			{
				return !InRange(key) ? Map_Fields.Null : m.Get(key);
			}

			public V Remove(Object key)
			{
				return !InRange(key) ? Map_Fields.Null : m.Remove(key);
			}

			public Map_Entry<K, V> CeilingEntry(K key)
			{
				return ExportEntry(SubCeiling(key));
			}

			public K CeilingKey(K key)
			{
				return KeyOrNull(SubCeiling(key));
			}

			public Map_Entry<K, V> HigherEntry(K key)
			{
				return ExportEntry(SubHigher(key));
			}

			public K HigherKey(K key)
			{
				return KeyOrNull(SubHigher(key));
			}

			public Map_Entry<K, V> FloorEntry(K key)
			{
				return ExportEntry(SubFloor(key));
			}

			public K FloorKey(K key)
			{
				return KeyOrNull(SubFloor(key));
			}

			public Map_Entry<K, V> LowerEntry(K key)
			{
				return ExportEntry(SubLower(key));
			}

			public K LowerKey(K key)
			{
				return KeyOrNull(SubLower(key));
			}

			public K FirstKey()
			{
				return Key(SubLowest());
			}

			public K LastKey()
			{
				return Key(SubHighest());
			}

			public Map_Entry<K, V> FirstEntry()
			{
				return ExportEntry(SubLowest());
			}

			public Map_Entry<K, V> LastEntry()
			{
				return ExportEntry(SubHighest());
			}

			public Map_Entry<K, V> PollFirstEntry()
			{
				TreeMap.Entry<K, V> e = SubLowest();
				Map_Entry<K, V> result = ExportEntry(e);
				if (e != Map_Fields.Null)
				{
					m.DeleteEntry(e);
				}
				return result;
			}

			public Map_Entry<K, V> PollLastEntry()
			{
				TreeMap.Entry<K, V> e = SubHighest();
				Map_Entry<K, V> result = ExportEntry(e);
				if (e != Map_Fields.Null)
				{
					m.DeleteEntry(e);
				}
				return result;
			}

			// Views
			[NonSerialized]
			internal NavigableMap<K, V> DescendingMapView;
			[NonSerialized]
			internal EntrySetView EntrySetView;
			[NonSerialized]
			internal KeySet<K> NavigableKeySetView;

			public NavigableSet<K> NavigableKeySet()
			{
				KeySet<K> nksv = NavigableKeySetView;
				return (nksv != Map_Fields.Null) ? nksv : (NavigableKeySetView = new TreeMap.KeySet<>(this));
			}

			public Set<K> KeySet()
			{
				return NavigableKeySet();
			}

			public virtual NavigableSet<K> DescendingKeySet()
			{
				return outerInstance.DescendingMap().navigableKeySet();
			}

			public SortedMap<K, V> SubMap(K fromKey, K toKey)
			{
				return subMap(fromKey, Map_Fields.True, toKey, Map_Fields.False);
			}

			public SortedMap<K, V> HeadMap(K toKey)
			{
				return headMap(toKey, Map_Fields.False);
			}

			public SortedMap<K, V> TailMap(K fromKey)
			{
				return tailMap(fromKey, Map_Fields.True);
			}

			// View classes

			internal abstract class EntrySetView : AbstractSet<Map_Entry<K, V>>
			{
				private readonly TreeMap.NavigableSubMap<K, V> OuterInstance;

				public EntrySetView(TreeMap.NavigableSubMap<K, V> outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				[NonSerialized]
				internal int Size_Renamed = -1, SizeModCount;

				public virtual int Size()
				{
					if (outerInstance.FromStart && outerInstance.ToEnd)
					{
						return outerInstance.m.Size();
					}
					if (Size_Renamed == -1 || SizeModCount != outerInstance.m.modCount)
					{
						SizeModCount = outerInstance.m.modCount;
						Size_Renamed = 0;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Iterator<?> i = iterator();
						Iterator<?> i = Iterator();
						while (i.HasNext())
						{
							Size_Renamed++;
							i.Next();
						}
					}
					return Size_Renamed;
				}

				public virtual bool Empty
				{
					get
					{
						TreeMap.Entry<K, V> n = outerInstance.AbsLowest();
						return n == Map_Fields.Null || outerInstance.TooHigh(n.Key_Renamed);
					}
				}

				public virtual bool Contains(Object o)
				{
					if (!(o is Map_Entry))
					{
						return Map_Fields.False;
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> entry = (Map_Entry<?,?>) o;
					Map_Entry<?, ?> entry = (Map_Entry<?, ?>) o;
					Object key = entry.Key;
					if (!outerInstance.InRange(key))
					{
						return Map_Fields.False;
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: TreeMap.Entry<?,?> node = m.getEntry(key);
					TreeMap.Entry<?, ?> node = outerInstance.m.GetEntry(key);
					return node != Map_Fields.Null && ValEquals(node.Value, entry.Value);
				}

				public virtual bool Remove(Object o)
				{
					if (!(o is Map_Entry))
					{
						return Map_Fields.False;
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> entry = (Map_Entry<?,?>) o;
					Map_Entry<?, ?> entry = (Map_Entry<?, ?>) o;
					Object key = entry.Key;
					if (!outerInstance.InRange(key))
					{
						return Map_Fields.False;
					}
					TreeMap.Entry<K, V> node = outerInstance.m.GetEntry(key);
					if (node != Map_Fields.Null && ValEquals(node.Value, entry.Value))
					{
						outerInstance.m.DeleteEntry(node);
						return Map_Fields.True;
					}
					return Map_Fields.False;
				}
			}

			/// <summary>
			/// Iterators for SubMaps
			/// </summary>
			internal abstract class SubMapIterator<T> : Iterator<T>
			{
				private readonly TreeMap.NavigableSubMap<K, V> OuterInstance;

				internal TreeMap.Entry<K, V> LastReturned;
				internal TreeMap.Entry<K, V> Next;
				internal readonly Object FenceKey;
				internal int ExpectedModCount;

				internal SubMapIterator(TreeMap.NavigableSubMap<K, V> outerInstance, TreeMap.Entry<K, V> first, TreeMap.Entry<K, V> fence)
				{
					this.OuterInstance = outerInstance;
					ExpectedModCount = outerInstance.m.modCount;
					LastReturned = Map_Fields.Null;
					Next = first;
					FenceKey = fence == Map_Fields.Null ? UNBOUNDED : fence.Key_Renamed;
				}

				public bool HasNext()
				{
					return Next != Map_Fields.Null && Next.Key_Renamed != FenceKey;
				}

				internal TreeMap.Entry<K, V> NextEntry()
				{
					TreeMap.Entry<K, V> e = Next;
					if (e == Map_Fields.Null || e.Key_Renamed == FenceKey)
					{
						throw new NoSuchElementException();
					}
					if (outerInstance.m.modCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
					Next = Successor(e);
					LastReturned = e;
					return e;
				}

				internal TreeMap.Entry<K, V> PrevEntry()
				{
					TreeMap.Entry<K, V> e = Next;
					if (e == Map_Fields.Null || e.Key_Renamed == FenceKey)
					{
						throw new NoSuchElementException();
					}
					if (outerInstance.m.modCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
					Next = Predecessor(e);
					LastReturned = e;
					return e;
				}

				internal void RemoveAscending()
				{
					if (LastReturned == Map_Fields.Null)
					{
						throw new IllegalStateException();
					}
					if (outerInstance.m.modCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
					// deleted entries are replaced by their successors
					if (LastReturned.Left != Map_Fields.Null && LastReturned.Right != Map_Fields.Null)
					{
						Next = LastReturned;
					}
					outerInstance.m.DeleteEntry(LastReturned);
					LastReturned = Map_Fields.Null;
					ExpectedModCount = outerInstance.m.modCount;
				}

				internal void RemoveDescending()
				{
					if (LastReturned == Map_Fields.Null)
					{
						throw new IllegalStateException();
					}
					if (outerInstance.m.modCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
					outerInstance.m.DeleteEntry(LastReturned);
					LastReturned = Map_Fields.Null;
					ExpectedModCount = outerInstance.m.modCount;
				}

			}

			internal sealed class SubMapEntryIterator : SubMapIterator<Map_Entry<K, V>>
			{
				private readonly TreeMap.NavigableSubMap<K, V> OuterInstance;

				internal SubMapEntryIterator(TreeMap.NavigableSubMap<K, V> outerInstance, TreeMap.Entry<K, V> first, TreeMap.Entry<K, V> fence) : base(outerInstance, first, fence)
				{
					this.OuterInstance = outerInstance;
				}
				public Map_Entry<K, V> Next()
				{
					return NextEntry();
				}
				public void Remove()
				{
					RemoveAscending();
				}
			}

			internal sealed class DescendingSubMapEntryIterator : SubMapIterator<Map_Entry<K, V>>
			{
				private readonly TreeMap.NavigableSubMap<K, V> OuterInstance;

				internal DescendingSubMapEntryIterator(TreeMap.NavigableSubMap<K, V> outerInstance, TreeMap.Entry<K, V> last, TreeMap.Entry<K, V> fence) : base(outerInstance, last, fence)
				{
					this.OuterInstance = outerInstance;
				}

				public Map_Entry<K, V> Next()
				{
					return PrevEntry();
				}
				public void Remove()
				{
					RemoveDescending();
				}
			}

			// Implement minimal Spliterator as KeySpliterator backup
			internal sealed class SubMapKeyIterator : SubMapIterator<K>, Spliterator<K>
			{
				private readonly TreeMap.NavigableSubMap<K, V> OuterInstance;

				internal SubMapKeyIterator(TreeMap.NavigableSubMap<K, V> outerInstance, TreeMap.Entry<K, V> first, TreeMap.Entry<K, V> fence) : base(outerInstance, first, fence)
				{
					this.OuterInstance = outerInstance;
				}
				public K Next()
				{
					return NextEntry().Key_Renamed;
				}
				public void Remove()
				{
					RemoveAscending();
				}
				public Spliterator<K> TrySplit()
				{
					return Map_Fields.Null;
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base K> action)
				public void forEachRemaining<T1>(Consumer<T1> action)
				{
					while (HasNext())
					{
						action.Accept(Next());
					}
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base K> action)
				public bool tryAdvance<T1>(Consumer<T1> action)
				{
					if (HasNext())
					{
						action.Accept(Next());
						return Map_Fields.True;
					}
					return Map_Fields.False;
				}
				public long EstimateSize()
				{
					return Long.MaxValue;
				}
				public int Characteristics()
				{
					return Spliterator_Fields.DISTINCT | Spliterator_Fields.ORDERED | Spliterator_Fields.SORTED;
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final Comparator<? base K> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				public Comparator<?> Comparator
				{
					get
					{
						return OuterInstance.comparator();
					}
				}
			}

			internal sealed class DescendingSubMapKeyIterator : SubMapIterator<K>, Spliterator<K>
			{
				private readonly TreeMap.NavigableSubMap<K, V> OuterInstance;

				internal DescendingSubMapKeyIterator(TreeMap.NavigableSubMap<K, V> outerInstance, TreeMap.Entry<K, V> last, TreeMap.Entry<K, V> fence) : base(outerInstance, last, fence)
				{
					this.OuterInstance = outerInstance;
				}
				public K Next()
				{
					return PrevEntry().Key_Renamed;
				}
				public void Remove()
				{
					RemoveDescending();
				}
				public Spliterator<K> TrySplit()
				{
					return Map_Fields.Null;
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base K> action)
				public void forEachRemaining<T1>(Consumer<T1> action)
				{
					while (HasNext())
					{
						action.Accept(Next());
					}
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base K> action)
				public bool tryAdvance<T1>(Consumer<T1> action)
				{
					if (HasNext())
					{
						action.Accept(Next());
						return Map_Fields.True;
					}
					return Map_Fields.False;
				}
				public long EstimateSize()
				{
					return Long.MaxValue;
				}
				public int Characteristics()
				{
					return Spliterator_Fields.DISTINCT | Spliterator_Fields.ORDERED;
				}
			}
		}

		/// <summary>
		/// @serial include
		/// </summary>
		internal sealed class AscendingSubMap<K, V> : NavigableSubMap<K, V>
		{
			internal const long SerialVersionUID = 912986545866124060L;

			internal AscendingSubMap(TreeMap<K, V> m, bool fromStart, K lo, bool loInclusive, bool toEnd, K hi, bool hiInclusive) : base(m, fromStart, lo, loInclusive, toEnd, hi, hiInclusive)
			{
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> Comparator()
			{
				return m.Comparator();
			}

			public NavigableMap<K, V> SubMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive)
			{
				if (!InRange(fromKey, fromInclusive))
				{
					throw new IllegalArgumentException("fromKey out of range");
				}
				if (!InRange(toKey, toInclusive))
				{
					throw new IllegalArgumentException("toKey out of range");
				}
				return new AscendingSubMap<>(m, Map_Fields.False, fromKey, fromInclusive, Map_Fields.False, toKey, toInclusive);
			}

			public NavigableMap<K, V> HeadMap(K toKey, bool inclusive)
			{
				if (!InRange(toKey, inclusive))
				{
					throw new IllegalArgumentException("toKey out of range");
				}
				return new AscendingSubMap<>(m, FromStart, Lo, LoInclusive, Map_Fields.False, toKey, inclusive);
			}

			public NavigableMap<K, V> TailMap(K fromKey, bool inclusive)
			{
				if (!InRange(fromKey, inclusive))
				{
					throw new IllegalArgumentException("fromKey out of range");
				}
				return new AscendingSubMap<>(m, Map_Fields.False, fromKey, inclusive, ToEnd, Hi, HiInclusive);
			}

			public NavigableMap<K, V> DescendingMap()
			{
				NavigableMap<K, V> mv = DescendingMapView;
				return (mv != Map_Fields.Null) ? mv : (DescendingMapView = new DescendingSubMap<>(m, FromStart, Lo, LoInclusive, ToEnd, Hi, HiInclusive));
			}

			internal Iterator<K> KeyIterator()
			{
				return new SubMapKeyIterator(this, AbsLowest(), AbsHighFence());
			}

			internal Spliterator<K> KeySpliterator()
			{
				return new SubMapKeyIterator(this, AbsLowest(), AbsHighFence());
			}

			internal Iterator<K> DescendingKeyIterator()
			{
				return new DescendingSubMapKeyIterator(this, AbsHighest(), AbsLowFence());
			}

			internal sealed class AscendingEntrySetView : EntrySetView
			{
				private readonly TreeMap.AscendingSubMap<K, V> OuterInstance;

				public AscendingEntrySetView(TreeMap.AscendingSubMap<K, V> outerInstance) : base(outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public override Iterator<Map_Entry<K, V>> Iterator()
				{
					return new SubMapEntryIterator(OuterInstance, outerInstance.AbsLowest(), outerInstance.AbsHighFence());
				}
			}

			public Set<Map_Entry<K, V>> EntrySet()
			{
				EntrySetView es = EntrySetView;
				return (es != Map_Fields.Null) ? es : (EntrySetView = new AscendingEntrySetView(this));
			}

			internal TreeMap.Entry<K, V> SubLowest()
			{
				return AbsLowest();
			}
			internal TreeMap.Entry<K, V> SubHighest()
			{
				return AbsHighest();
			}
			internal TreeMap.Entry<K, V> SubCeiling(K key)
			{
				return AbsCeiling(key);
			}
			internal TreeMap.Entry<K, V> SubHigher(K key)
			{
				return AbsHigher(key);
			}
			internal TreeMap.Entry<K, V> SubFloor(K key)
			{
				return AbsFloor(key);
			}
			internal TreeMap.Entry<K, V> SubLower(K key)
			{
				return AbsLower(key);
			}
		}

		/// <summary>
		/// @serial include
		/// </summary>
		internal sealed class DescendingSubMap<K, V> : NavigableSubMap<K, V>
		{
			internal bool InstanceFieldsInitialized = Map_Fields.False;

			internal void InitializeInstanceFields()
			{
				ReverseComparator = Collections.ReverseOrder(m.Comparator_Renamed);
			}

			internal const long SerialVersionUID = 912986545866120460L;
			internal DescendingSubMap(TreeMap<K, V> m, bool fromStart, K lo, bool loInclusive, bool toEnd, K hi, bool hiInclusive) : base(m, fromStart, lo, loInclusive, toEnd, hi, hiInclusive)
			{
				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = Map_Fields.True;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private final Comparator<? base K> reverseComparator = Collections.reverseOrder(m.comparator);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal Comparator<?> ReverseComparator;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> Comparator()
			{
				return ReverseComparator;
			}

			public NavigableMap<K, V> SubMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive)
			{
				if (!InRange(fromKey, fromInclusive))
				{
					throw new IllegalArgumentException("fromKey out of range");
				}
				if (!InRange(toKey, toInclusive))
				{
					throw new IllegalArgumentException("toKey out of range");
				}
				return new DescendingSubMap<>(m, Map_Fields.False, toKey, toInclusive, Map_Fields.False, fromKey, fromInclusive);
			}

			public NavigableMap<K, V> HeadMap(K toKey, bool inclusive)
			{
				if (!InRange(toKey, inclusive))
				{
					throw new IllegalArgumentException("toKey out of range");
				}
				return new DescendingSubMap<>(m, Map_Fields.False, toKey, inclusive, ToEnd, Hi, HiInclusive);
			}

			public NavigableMap<K, V> TailMap(K fromKey, bool inclusive)
			{
				if (!InRange(fromKey, inclusive))
				{
					throw new IllegalArgumentException("fromKey out of range");
				}
				return new DescendingSubMap<>(m, FromStart, Lo, LoInclusive, Map_Fields.False, fromKey, inclusive);
			}

			public NavigableMap<K, V> DescendingMap()
			{
				NavigableMap<K, V> mv = DescendingMapView;
				return (mv != Map_Fields.Null) ? mv : (DescendingMapView = new AscendingSubMap<>(m, FromStart, Lo, LoInclusive, ToEnd, Hi, HiInclusive));
			}

			internal Iterator<K> KeyIterator()
			{
				return new DescendingSubMapKeyIterator(this, AbsHighest(), AbsLowFence());
			}

			internal Spliterator<K> KeySpliterator()
			{
				return new DescendingSubMapKeyIterator(this, AbsHighest(), AbsLowFence());
			}

			internal Iterator<K> DescendingKeyIterator()
			{
				return new SubMapKeyIterator(this, AbsLowest(), AbsHighFence());
			}

			internal sealed class DescendingEntrySetView : EntrySetView
			{
				private readonly TreeMap.DescendingSubMap<K, V> OuterInstance;

				public DescendingEntrySetView(TreeMap.DescendingSubMap<K, V> outerInstance) : base(outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public override Iterator<Map_Entry<K, V>> Iterator()
				{
					return new DescendingSubMapEntryIterator(OuterInstance, outerInstance.AbsHighest(), outerInstance.AbsLowFence());
				}
			}

			public Set<Map_Entry<K, V>> EntrySet()
			{
				EntrySetView es = EntrySetView;
				return (es != Map_Fields.Null) ? es : (EntrySetView = new DescendingEntrySetView(this));
			}

			internal TreeMap.Entry<K, V> SubLowest()
			{
				return AbsHighest();
			}
			internal TreeMap.Entry<K, V> SubHighest()
			{
				return AbsLowest();
			}
			internal TreeMap.Entry<K, V> SubCeiling(K key)
			{
				return AbsFloor(key);
			}
			internal TreeMap.Entry<K, V> SubHigher(K key)
			{
				return AbsLower(key);
			}
			internal TreeMap.Entry<K, V> SubFloor(K key)
			{
				return AbsCeiling(key);
			}
			internal TreeMap.Entry<K, V> SubLower(K key)
			{
				return AbsHigher(key);
			}
		}

		/// <summary>
		/// This class exists solely for the sake of serialization
		/// compatibility with previous releases of TreeMap that did not
		/// support NavigableMap.  It translates an old-version SubMap into
		/// a new-version AscendingSubMap. This class is never otherwise
		/// used.
		/// 
		/// @serial include
		/// </summary>
		[Serializable]
		private class SubMap : AbstractMap<K, V>, SortedMap<K, V>
		{
			private readonly TreeMap<K, V> OuterInstance;

			public SubMap(TreeMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			internal const long SerialVersionUID = -6520786458950516097L;
			internal bool FromStart = Map_Fields.False, ToEnd = Map_Fields.False;
			internal K FromKey, ToKey;
			internal virtual Object ReadResolve()
			{
				return new AscendingSubMap<>(OuterInstance, FromStart, FromKey, Map_Fields.True, ToEnd, ToKey, Map_Fields.False);
			}
			public virtual Set<Map_Entry<K, V>> EntrySet()
			{
				throw new InternalError();
			}
			public virtual K LastKey()
			{
				throw new InternalError();
			}
			public virtual K FirstKey()
			{
				throw new InternalError();
			}
			public virtual SortedMap<K, V> SubMap(K fromKey, K toKey)
			{
				throw new InternalError();
			}
			public virtual SortedMap<K, V> HeadMap(K toKey)
			{
				throw new InternalError();
			}
			public virtual SortedMap<K, V> TailMap(K fromKey)
			{
				throw new InternalError();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public virtual Comparator<?> Comparator()
			{
				throw new InternalError();
			}
		}


		// Red-black mechanics

		private static readonly bool RED = Map_Fields.False;
		private static readonly bool BLACK = Map_Fields.True;

		/// <summary>
		/// Node in the Tree.  Doubles as a means to pass key-value pairs back to
		/// user (see Map.Entry).
		/// </summary>

		internal sealed class Entry<K, V> : Map_Entry<K, V>
		{
			internal K Key_Renamed;
			internal V Value_Renamed;
			internal Entry<K, V> Left;
			internal Entry<K, V> Right;
			internal Entry<K, V> Parent;
			internal bool Color = BLACK;

			/// <summary>
			/// Make a new cell with given key, value, and parent, and with
			/// {@code null} child links, and BLACK color.
			/// </summary>
			internal Entry(K key, V value, Entry<K, V> parent)
			{
				this.Key_Renamed = key;
				this.Value_Renamed = value;
				this.Parent = parent;
			}

			/// <summary>
			/// Returns the key.
			/// </summary>
			/// <returns> the key </returns>
			public K Key
			{
				get
				{
					return Key_Renamed;
				}
			}

			/// <summary>
			/// Returns the value associated with the key.
			/// </summary>
			/// <returns> the value associated with the key </returns>
			public V Value
			{
				get
				{
					return Value_Renamed;
				}
			}

			/// <summary>
			/// Replaces the value currently associated with the key with the given
			/// value.
			/// </summary>
			/// <returns> the value associated with the key before this method was
			///         called </returns>
			public V SetValue(V value)
			{
				V Map_Fields.OldValue = this.Value_Renamed;
				this.Value_Renamed = value;
				return Map_Fields.OldValue;
			}

			public override bool Equals(Object o)
			{
				if (!(o is Map_Entry))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>)o;
				Map_Entry<?, ?> e = (Map_Entry<?, ?>)o;

				return ValEquals(Key_Renamed,e.Key) && ValEquals(Value_Renamed,e.Value);
			}

			public override int HashCode()
			{
				int keyHash = (Key_Renamed == Map_Fields.Null ? 0 : Key_Renamed.HashCode());
				int valueHash = (Value_Renamed == Map_Fields.Null ? 0 : Value_Renamed.HashCode());
				return keyHash ^ valueHash;
			}

			public override String ToString()
			{
				return Key_Renamed + "=" + Value_Renamed;
			}
		}

		/// <summary>
		/// Returns the first Entry in the TreeMap (according to the TreeMap's
		/// key-sort function).  Returns null if the TreeMap is empty.
		/// </summary>
		internal Entry<K, V> FirstEntry
		{
			get
			{
				Entry<K, V> p = Root;
				if (p != Map_Fields.Null)
				{
					while (p.Left != Map_Fields.Null)
					{
						p = p.Left;
					}
				}
				return p;
			}
		}

		/// <summary>
		/// Returns the last Entry in the TreeMap (according to the TreeMap's
		/// key-sort function).  Returns null if the TreeMap is empty.
		/// </summary>
		internal Entry<K, V> LastEntry
		{
			get
			{
				Entry<K, V> p = Root;
				if (p != Map_Fields.Null)
				{
					while (p.Right != Map_Fields.Null)
					{
						p = p.Right;
					}
				}
				return p;
			}
		}

		/// <summary>
		/// Returns the successor of the specified Entry, or null if no such.
		/// </summary>
		internal static TreeMap.Entry<K, V> successor<K, V>(Entry<K, V> t)
		{
			if (t == Map_Fields.Null)
			{
				return Map_Fields.Null;
			}
			else if (t.Right != Map_Fields.Null)
			{
				Entry<K, V> p = t.Right;
				while (p.Left != Map_Fields.Null)
				{
					p = p.Left;
				}
				return p;
			}
			else
			{
				Entry<K, V> p = t.Parent;
				Entry<K, V> ch = t;
				while (p != Map_Fields.Null && ch == p.Right)
				{
					ch = p;
					p = p.Parent;
				}
				return p;
			}
		}

		/// <summary>
		/// Returns the predecessor of the specified Entry, or null if no such.
		/// </summary>
		internal static Entry<K, V> predecessor<K, V>(Entry<K, V> t)
		{
			if (t == Map_Fields.Null)
			{
				return Map_Fields.Null;
			}
			else if (t.Left != Map_Fields.Null)
			{
				Entry<K, V> p = t.Left;
				while (p.Right != Map_Fields.Null)
				{
					p = p.Right;
				}
				return p;
			}
			else
			{
				Entry<K, V> p = t.Parent;
				Entry<K, V> ch = t;
				while (p != Map_Fields.Null && ch == p.Left)
				{
					ch = p;
					p = p.Parent;
				}
				return p;
			}
		}

		/// <summary>
		/// Balancing operations.
		/// 
		/// Implementations of rebalancings during insertion and deletion are
		/// slightly different than the CLR version.  Rather than using dummy
		/// nilnodes, we use a set of accessors that deal properly with null.  They
		/// are used to avoid messiness surrounding nullness checks in the main
		/// algorithms.
		/// </summary>

		private static bool colorOf<K, V>(Entry<K, V> p)
		{
			return (p == Map_Fields.Null ? BLACK : p.Color);
		}

		private static Entry<K, V> parentOf<K, V>(Entry<K, V> p)
		{
			return (p == Map_Fields.Null ? Map_Fields.Null: p.Parent);
		}

		private static void setColor<K, V>(Entry<K, V> p, bool c)
		{
			if (p != Map_Fields.Null)
			{
				p.Color = c;
			}
		}

		private static Entry<K, V> leftOf<K, V>(Entry<K, V> p)
		{
			return (p == Map_Fields.Null) ? Map_Fields.Null: p.Left;
		}

		private static Entry<K, V> rightOf<K, V>(Entry<K, V> p)
		{
			return (p == Map_Fields.Null) ? Map_Fields.Null: p.Right;
		}

		/// <summary>
		/// From CLR </summary>
		private void RotateLeft(Entry<K, V> p)
		{
			if (p != Map_Fields.Null)
			{
				Entry<K, V> r = p.Right;
				p.Right = r.Left;
				if (r.Left != Map_Fields.Null)
				{
					r.Left.Parent = p;
				}
				r.Parent = p.Parent;
				if (p.Parent == Map_Fields.Null)
				{
					Root = r;
				}
				else if (p.Parent.Left == p)
				{
					p.Parent.Left = r;
				}
				else
				{
					p.Parent.Right = r;
				}
				r.Left = p;
				p.Parent = r;
			}
		}

		/// <summary>
		/// From CLR </summary>
		private void RotateRight(Entry<K, V> p)
		{
			if (p != Map_Fields.Null)
			{
				Entry<K, V> l = p.Left;
				p.Left = l.Right;
				if (l.Right != Map_Fields.Null)
				{
					l.Right.Parent = p;
				}
				l.Parent = p.Parent;
				if (p.Parent == Map_Fields.Null)
				{
					Root = l;
				}
				else if (p.Parent.Right == p)
				{
					p.Parent.Right = l;
				}
				else
				{
					p.Parent.Left = l;
				}
				l.Right = p;
				p.Parent = l;
			}
		}

		/// <summary>
		/// From CLR </summary>
		private void FixAfterInsertion(Entry<K, V> x)
		{
			x.Color = RED;

			while (x != Map_Fields.Null && x != Root && x.Parent.Color == RED)
			{
				if (ParentOf(x) == LeftOf(ParentOf(ParentOf(x))))
				{
					Entry<K, V> y = RightOf(ParentOf(ParentOf(x)));
					if (ColorOf(y) == RED)
					{
						SetColor(ParentOf(x), BLACK);
						SetColor(y, BLACK);
						SetColor(ParentOf(ParentOf(x)), RED);
						x = ParentOf(ParentOf(x));
					}
					else
					{
						if (x == RightOf(ParentOf(x)))
						{
							x = ParentOf(x);
							RotateLeft(x);
						}
						SetColor(ParentOf(x), BLACK);
						SetColor(ParentOf(ParentOf(x)), RED);
						RotateRight(ParentOf(ParentOf(x)));
					}
				}
				else
				{
					Entry<K, V> y = LeftOf(ParentOf(ParentOf(x)));
					if (ColorOf(y) == RED)
					{
						SetColor(ParentOf(x), BLACK);
						SetColor(y, BLACK);
						SetColor(ParentOf(ParentOf(x)), RED);
						x = ParentOf(ParentOf(x));
					}
					else
					{
						if (x == LeftOf(ParentOf(x)))
						{
							x = ParentOf(x);
							RotateRight(x);
						}
						SetColor(ParentOf(x), BLACK);
						SetColor(ParentOf(ParentOf(x)), RED);
						RotateLeft(ParentOf(ParentOf(x)));
					}
				}
			}
			Root.Color = BLACK;
		}

		/// <summary>
		/// Delete node p, and then rebalance the tree.
		/// </summary>
		private void DeleteEntry(Entry<K, V> p)
		{
			ModCount++;
			Size_Renamed--;

			// If strictly internal, copy successor's element to p and then make p
			// point to successor.
			if (p.Left != Map_Fields.Null && p.Right != Map_Fields.Null)
			{
				Entry<K, V> s = Successor(p);
				p.Key_Renamed = s.Key_Renamed;
				p.Value_Renamed = s.Value_Renamed;
				p = s;
			} // p has 2 children

			// Start fixup at replacement node, if it exists.
			Entry<K, V> replacement = (p.Left != Map_Fields.Null ? p.Left : p.Right);

			if (replacement != Map_Fields.Null)
			{
				// Link replacement to parent
				replacement.Parent = p.Parent;
				if (p.Parent == Map_Fields.Null)
				{
					Root = replacement;
				}
				else if (p == p.Parent.Left)
				{
					p.Parent.Left = replacement;
				}
				else
				{
					p.Parent.Right = replacement;
				}

				// Null out links so they are OK to use by fixAfterDeletion.
				p.Left = p.Right = p.Parent = Map_Fields.Null;

				// Fix replacement
				if (p.Color == BLACK)
				{
					FixAfterDeletion(replacement);
				}
			} // return if we are the only node.
			else if (p.Parent == Map_Fields.Null)
			{
				Root = Map_Fields.Null;
			} //  No children. Use self as phantom replacement and unlink.
			else
			{
				if (p.Color == BLACK)
				{
					FixAfterDeletion(p);
				}

				if (p.Parent != Map_Fields.Null)
				{
					if (p == p.Parent.Left)
					{
						p.Parent.Left = Map_Fields.Null;
					}
					else if (p == p.Parent.Right)
					{
						p.Parent.Right = Map_Fields.Null;
					}
					p.Parent = Map_Fields.Null;
				}
			}
		}

		/// <summary>
		/// From CLR </summary>
		private void FixAfterDeletion(Entry<K, V> x)
		{
			while (x != Root && ColorOf(x) == BLACK)
			{
				if (x == LeftOf(ParentOf(x)))
				{
					Entry<K, V> sib = RightOf(ParentOf(x));

					if (ColorOf(sib) == RED)
					{
						SetColor(sib, BLACK);
						SetColor(ParentOf(x), RED);
						RotateLeft(ParentOf(x));
						sib = RightOf(ParentOf(x));
					}

					if (ColorOf(LeftOf(sib)) == BLACK && ColorOf(RightOf(sib)) == BLACK)
					{
						SetColor(sib, RED);
						x = ParentOf(x);
					}
					else
					{
						if (ColorOf(RightOf(sib)) == BLACK)
						{
							SetColor(LeftOf(sib), BLACK);
							SetColor(sib, RED);
							RotateRight(sib);
							sib = RightOf(ParentOf(x));
						}
						SetColor(sib, ColorOf(ParentOf(x)));
						SetColor(ParentOf(x), BLACK);
						SetColor(RightOf(sib), BLACK);
						RotateLeft(ParentOf(x));
						x = Root;
					}
				} // symmetric
				else
				{
					Entry<K, V> sib = LeftOf(ParentOf(x));

					if (ColorOf(sib) == RED)
					{
						SetColor(sib, BLACK);
						SetColor(ParentOf(x), RED);
						RotateRight(ParentOf(x));
						sib = LeftOf(ParentOf(x));
					}

					if (ColorOf(RightOf(sib)) == BLACK && ColorOf(LeftOf(sib)) == BLACK)
					{
						SetColor(sib, RED);
						x = ParentOf(x);
					}
					else
					{
						if (ColorOf(LeftOf(sib)) == BLACK)
						{
							SetColor(RightOf(sib), BLACK);
							SetColor(sib, RED);
							RotateLeft(sib);
							sib = LeftOf(ParentOf(x));
						}
						SetColor(sib, ColorOf(ParentOf(x)));
						SetColor(ParentOf(x), BLACK);
						SetColor(LeftOf(sib), BLACK);
						RotateRight(ParentOf(x));
						x = Root;
					}
				}
			}

			SetColor(x, BLACK);
		}

		private const long SerialVersionUID = 919286545866124006L;

		/// <summary>
		/// Save the state of the {@code TreeMap} instance to a stream (i.e.,
		/// serialize it).
		/// 
		/// @serialData The <em>size</em> of the TreeMap (the number of key-value
		///             mappings) is emitted (int), followed by the key (Object)
		///             and value (Object) for each key-value mapping represented
		///             by the TreeMap. The key-value mappings are emitted in
		///             key-order (as determined by the TreeMap's Comparator,
		///             or by the keys' natural ordering if the TreeMap has no
		///             Comparator).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// Write out the Comparator and any hidden stuff
			s.DefaultWriteObject();

			// Write out size (number of Mappings)
			s.WriteInt(Size_Renamed);

			// Write out keys and values (alternating)
			for (Iterator<Map_Entry<K, V>> i = EntrySet().Iterator(); i.HasNext();)
			{
				Map_Entry<K, V> e = i.Next();
				s.WriteObject(e.Key);
				s.WriteObject(e.Value);
			}
		}

		/// <summary>
		/// Reconstitute the {@code TreeMap} instance from a stream (i.e.,
		/// deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(final java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in the Comparator and any hidden stuff
			s.DefaultReadObject();

			// Read in size
			int size = s.ReadInt();

			BuildFromSorted(size, Map_Fields.Null, s, Map_Fields.Null);
		}

		/// <summary>
		/// Intended to be called only from TreeSet.readObject </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readTreeSet(int size, java.io.ObjectInputStream s, V defaultVal) throws java.io.IOException, ClassNotFoundException
		internal virtual void ReadTreeSet(int size, java.io.ObjectInputStream s, V defaultVal)
		{
			BuildFromSorted(size, Map_Fields.Null, s, defaultVal);
		}

		/// <summary>
		/// Intended to be called only from TreeSet.addAll </summary>
		internal virtual void addAllForTreeSet<T1>(SortedSet<T1> set, V defaultVal) where T1 : K
		{
			try
			{
				BuildFromSorted(set.Size(), set.Iterator(), Map_Fields.Null, defaultVal);
			}
			catch (java.io.IOException)
			{
			}
			catch (ClassNotFoundException)
			{
			}
		}


		/// <summary>
		/// Linear time tree building algorithm from sorted data.  Can accept keys
		/// and/or values from iterator or stream. This leads to too many
		/// parameters, but seems better than alternatives.  The four formats
		/// that this method accepts are:
		/// 
		///    1) An iterator of Map.Entries.  (it != null, defaultVal == null).
		///    2) An iterator of keys.         (it != null, defaultVal != null).
		///    3) A stream of alternating serialized keys and values.
		///                                   (it == null, defaultVal == null).
		///    4) A stream of serialized keys. (it == null, defaultVal != null).
		/// 
		/// It is assumed that the comparator of the TreeMap is already set prior
		/// to calling this method.
		/// </summary>
		/// <param name="size"> the number of keys (or key-value pairs) to be read from
		///        the iterator or stream </param>
		/// <param name="it"> If non-null, new entries are created from entries
		///        or keys read from this iterator. </param>
		/// <param name="str"> If non-null, new entries are created from keys and
		///        possibly values read from this stream in serialized form.
		///        Exactly one of it and str should be non-null. </param>
		/// <param name="defaultVal"> if non-null, this default value is used for
		///        each value in the map.  If null, each value is read from
		///        iterator or stream, as described above. </param>
		/// <exception cref="java.io.IOException"> propagated from stream reads. This cannot
		///         occur if str is null. </exception>
		/// <exception cref="ClassNotFoundException"> propagated from readObject.
		///         This cannot occur if str is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void buildFromSorted(int size, Iterator<?> it, java.io.ObjectInputStream str, V defaultVal) throws java.io.IOException, ClassNotFoundException
		private void buildFromSorted<T1>(int size, Iterator<T1> it, java.io.ObjectInputStream str, V defaultVal)
		{
			this.Size_Renamed = size;
			Root = BuildFromSorted(0, 0, size-1, ComputeRedLevel(size), it, str, defaultVal);
		}

		/// <summary>
		/// Recursive "helper method" that does the real work of the
		/// previous method.  Identically named parameters have
		/// identical definitions.  Additional parameters are documented below.
		/// It is assumed that the comparator and size fields of the TreeMap are
		/// already set prior to calling this method.  (It ignores both fields.)
		/// </summary>
		/// <param name="level"> the current level of tree. Initial call should be 0. </param>
		/// <param name="lo"> the first element index of this subtree. Initial should be 0. </param>
		/// <param name="hi"> the last element index of this subtree.  Initial should be
		///        size-1. </param>
		/// <param name="redLevel"> the level at which nodes should be red.
		///        Must be equal to computeRedLevel for tree of this size. </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private final Entry<K,V> buildFromSorted(int level, int lo, int hi, int redLevel, Iterator<?> it, java.io.ObjectInputStream str, V defaultVal) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		private Entry<K, V> BuildFromSorted(int level, int lo, int hi, int redLevel, Iterator<T1> it, java.io.ObjectInputStream str, V defaultVal)
		{
			/*
			 * Strategy: The root is the middlemost element. To get to it, we
			 * have to first recursively construct the entire left subtree,
			 * so as to grab all of its elements. We can then proceed with right
			 * subtree.
			 *
			 * The lo and hi arguments are the minimum and maximum
			 * indices to pull out of the iterator or stream for current subtree.
			 * They are not actually indexed, we just proceed sequentially,
			 * ensuring that items are extracted in corresponding order.
			 */

			if (hi < lo)
			{
				return Map_Fields.Null;
			}

			int mid = (int)((uint)(lo + hi) >> 1);

			Entry<K, V> left = Map_Fields.Null;
			if (lo < mid)
			{
				left = BuildFromSorted(level + 1, lo, mid - 1, redLevel, it, str, defaultVal);
			}

			// extract key and/or value from iterator or stream
			K key;
			V value;
			if (it != Map_Fields.Null)
			{
				if (defaultVal == Map_Fields.Null)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> entry = (Map_Entry<?,?>)it.next();
					Map_Entry<?, ?> entry = (Map_Entry<?, ?>)it.Next();
					key = (K)entry.Key;
					value = (V)entry.Value;
				}
				else
				{
					key = (K)it.Next();
					value = defaultVal;
				}
			} // use stream
			else
			{
				key = (K) str.ReadObject();
				value = (defaultVal != Map_Fields.Null ? defaultVal : (V) str.ReadObject());
			}

			Entry<K, V> middle = new Entry<K, V>(key, value, Map_Fields.Null);

			// color nodes in non-full bottommost level red
			if (level == redLevel)
			{
				middle.Color = RED;
			}

			if (left != Map_Fields.Null)
			{
				middle.Left = left;
				left.Parent = middle;
			}

			if (mid < hi)
			{
				Entry<K, V> right = BuildFromSorted(level + 1, mid + 1, hi, redLevel, it, str, defaultVal);
				middle.Right = right;
				right.Parent = middle;
			}

			return middle;
		}

		/// <summary>
		/// Find the level down to which to assign all nodes BLACK.  This is the
		/// last `full' level of the complete binary tree produced by
		/// buildTree. The remaining nodes are colored RED. (This makes a `nice'
		/// set of color assignments wrt future insertions.) This level number is
		/// computed by finding the number of splits needed to reach the zeroeth
		/// node.  (The answer is ~lg(N), but in any case must be computed by same
		/// quick O(lg(N)) loop.)
		/// </summary>
		private static int ComputeRedLevel(int sz)
		{
			int level = 0;
			for (int m = sz - 1; m >= 0; m = m / 2 - 1)
			{
				level++;
			}
			return level;
		}

		/// <summary>
		/// Currently, we support Spliterator-based versions only for the
		/// full map, in either plain of descending form, otherwise relying
		/// on defaults because size estimation for submaps would dominate
		/// costs. The type tests needed to check these for key views are
		/// not very nice but avoid disrupting existing class
		/// structures. Callers must use plain default spliterators if this
		/// returns null.
		/// </summary>
		internal static Spliterator<K> keySpliteratorFor<K, T1>(NavigableMap<T1> m)
		{
			if (m is TreeMap)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") TreeMap<K,Object> t = (TreeMap<K,Object>) m;
				TreeMap<K, Object> t = (TreeMap<K, Object>) m;
				return t.KeySpliterator();
			}
			if (m is DescendingSubMap)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") DescendingSubMap<K,?> dm = (DescendingSubMap<K,?>) m;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				DescendingSubMap<K, ?> dm = (DescendingSubMap<K, ?>) m;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: TreeMap<K,?> tm = dm.m;
				TreeMap<K, ?> tm = dm.m;
				if (dm == tm.DescendingMap_Renamed)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") TreeMap<K,Object> t = (TreeMap<K,Object>) tm;
					TreeMap<K, Object> t = (TreeMap<K, Object>) tm;
					return t.DescendingKeySpliterator();
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") NavigableSubMap<K,?> sm = (NavigableSubMap<K,?>) m;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			NavigableSubMap<K, ?> sm = (NavigableSubMap<K, ?>) m;
			return sm.KeySpliterator();
		}

		internal Spliterator<K> KeySpliterator()
		{
			return new KeySpliterator<K, V>(this, Map_Fields.Null, Map_Fields.Null, 0, -1, 0);
		}

		internal Spliterator<K> DescendingKeySpliterator()
		{
			return new DescendingKeySpliterator<K, V>(this, Map_Fields.Null, Map_Fields.Null, 0, -2, 0);
		}

		/// <summary>
		/// Base class for spliterators.  Iteration starts at a given
		/// origin and continues up to but not including a given fence (or
		/// null for end).  At top-level, for ascending cases, the first
		/// split uses the root as left-fence/right-origin. From there,
		/// right-hand splits replace the current fence with its left
		/// child, also serving as origin for the split-off spliterator.
		/// Left-hands are symmetric. Descending versions place the origin
		/// at the end and invert ascending split rules.  This base class
		/// is non-commital about directionality, or whether the top-level
		/// spliterator covers the whole tree. This means that the actual
		/// split mechanics are located in subclasses. Some of the subclass
		/// trySplit methods are identical (except for return types), but
		/// not nicely factorable.
		/// 
		/// Currently, subclass versions exist only for the full map
		/// (including descending keys via its descendingMap).  Others are
		/// possible but currently not worthwhile because submaps require
		/// O(n) computations to determine size, which substantially limits
		/// potential speed-ups of using custom Spliterators versus default
		/// mechanics.
		/// 
		/// To boostrap initialization, external constructors use
		/// negative size estimates: -1 for ascend, -2 for descend.
		/// </summary>
		internal class TreeMapSpliterator<K, V>
		{
			internal readonly TreeMap<K, V> Tree;
			internal TreeMap.Entry<K, V> Current; // traverser; initially first node in range
			internal TreeMap.Entry<K, V> Fence; // one past last, or null
			internal int Side; // 0: top, -1: is a left split, +1: right
			internal int Est; // size estimate (exact only for top-level)
			internal int ExpectedModCount; // for CME checks

			internal TreeMapSpliterator(TreeMap<K, V> tree, TreeMap.Entry<K, V> origin, TreeMap.Entry<K, V> fence, int side, int est, int expectedModCount)
			{
				this.Tree = tree;
				this.Current = origin;
				this.Fence = fence;
				this.Side = side;
				this.Est = est;
				this.ExpectedModCount = expectedModCount;
			}

			internal int Estimate
			{
				get
				{
					int s;
					TreeMap<K, V> t;
					if ((s = Est) < 0)
					{
						if ((t = Tree) != Map_Fields.Null)
						{
							Current = (s == -1) ? t.FirstEntry : t.LastEntry;
							s = Est = t.Size_Renamed;
							ExpectedModCount = t.ModCount;
						}
						else
						{
							s = Est = 0;
						}
					}
					return s;
				}
			}

			public long EstimateSize()
			{
				return (long)Estimate;
			}
		}

		internal sealed class KeySpliterator<K, V> : TreeMapSpliterator<K, V>, Spliterator<K>
		{
			internal KeySpliterator(TreeMap<K, V> tree, TreeMap.Entry<K, V> origin, TreeMap.Entry<K, V> fence, int side, int est, int expectedModCount) : base(tree, origin, fence, side, est, expectedModCount)
			{
			}

			public KeySpliterator<K, V> TrySplit()
			{
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				int d = Side;
				TreeMap.Entry<K, V> e = Current, f = Fence, s = ((e == Map_Fields.Null || e == f) ? Map_Fields.Null : (d == 0) ? Tree.Root : (d > 0) ? e.Right : (d < 0 && f != Map_Fields.Null) ? f.Left : Map_Fields.Null); // was left -  was right -  was top -  empty
				if (s != Map_Fields.Null && s != e && s != f && Tree.Compare(e.Key_Renamed, s.Key_Renamed) < 0) // e not already past s
				{
					Side = 1;
					return new KeySpliterator<> (Tree, e, Current = s, -1, Est = (int)((uint)Est >> 1), ExpectedModCount);
				}
				return Map_Fields.Null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base K> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				TreeMap.Entry<K, V> f = Fence, e , p , pl ;
				if ((e = Current) != Map_Fields.Null && e != f)
				{
					Current = f; // exhaust
					do
					{
						action.Accept(e.Key_Renamed);
						if ((p = e.Right) != Map_Fields.Null)
						{
							while ((pl = p.Left) != Map_Fields.Null)
							{
								p = pl;
							}
						}
						else
						{
							while ((p = e.Parent) != Map_Fields.Null && e == p.Right)
							{
								e = p;
							}
						}
					} while ((e = p) != Map_Fields.Null && e != f);
					if (Tree.ModCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base K> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				TreeMap.Entry<K, V> e;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				if ((e = Current) == Map_Fields.Null || e == Fence)
				{
					return Map_Fields.False;
				}
				Current = Successor(e);
				action.Accept(e.Key_Renamed);
				if (Tree.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				return Map_Fields.True;
			}

			public int Characteristics()
			{
				return (Side == 0 ? Spliterator_Fields.SIZED : 0) | Spliterator_Fields.DISTINCT | Spliterator_Fields.SORTED | Spliterator_Fields.ORDERED;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final Comparator<? base K> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public Comparator<?> Comparator
			{
				get
				{
					return Tree.Comparator_Renamed;
				}
			}

		}

		internal sealed class DescendingKeySpliterator<K, V> : TreeMapSpliterator<K, V>, Spliterator<K>
		{
			internal DescendingKeySpliterator(TreeMap<K, V> tree, TreeMap.Entry<K, V> origin, TreeMap.Entry<K, V> fence, int side, int est, int expectedModCount) : base(tree, origin, fence, side, est, expectedModCount)
			{
			}

			public DescendingKeySpliterator<K, V> TrySplit()
			{
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				int d = Side;
				TreeMap.Entry<K, V> e = Current, f = Fence, s = ((e == Map_Fields.Null || e == f) ? Map_Fields.Null : (d == 0) ? Tree.Root : (d < 0) ? e.Left : (d > 0 && f != Map_Fields.Null) ? f.Right : Map_Fields.Null); // was right -  was left -  was top -  empty
				if (s != Map_Fields.Null && s != e && s != f && Tree.Compare(e.Key_Renamed, s.Key_Renamed) > 0) // e not already past s
				{
					Side = 1;
					return new DescendingKeySpliterator<> (Tree, e, Current = s, -1, Est = (int)((uint)Est >> 1), ExpectedModCount);
				}
				return Map_Fields.Null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base K> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				TreeMap.Entry<K, V> f = Fence, e , p , pr ;
				if ((e = Current) != Map_Fields.Null && e != f)
				{
					Current = f; // exhaust
					do
					{
						action.Accept(e.Key_Renamed);
						if ((p = e.Left) != Map_Fields.Null)
						{
							while ((pr = p.Right) != Map_Fields.Null)
							{
								p = pr;
							}
						}
						else
						{
							while ((p = e.Parent) != Map_Fields.Null && e == p.Left)
							{
								e = p;
							}
						}
					} while ((e = p) != Map_Fields.Null && e != f);
					if (Tree.ModCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base K> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				TreeMap.Entry<K, V> e;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				if ((e = Current) == Map_Fields.Null || e == Fence)
				{
					return Map_Fields.False;
				}
				Current = Predecessor(e);
				action.Accept(e.Key_Renamed);
				if (Tree.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				return Map_Fields.True;
			}

			public int Characteristics()
			{
				return (Side == 0 ? Spliterator_Fields.SIZED : 0) | Spliterator_Fields.DISTINCT | Spliterator_Fields.ORDERED;
			}
		}

		internal sealed class ValueSpliterator<K, V> : TreeMapSpliterator<K, V>, Spliterator<V>
		{
			internal ValueSpliterator(TreeMap<K, V> tree, TreeMap.Entry<K, V> origin, TreeMap.Entry<K, V> fence, int side, int est, int expectedModCount) : base(tree, origin, fence, side, est, expectedModCount)
			{
			}

			public ValueSpliterator<K, V> TrySplit()
			{
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				int d = Side;
				TreeMap.Entry<K, V> e = Current, f = Fence, s = ((e == Map_Fields.Null || e == f) ? Map_Fields.Null : (d == 0) ? Tree.Root : (d > 0) ? e.Right : (d < 0 && f != Map_Fields.Null) ? f.Left : Map_Fields.Null); // was left -  was right -  was top -  empty
				if (s != Map_Fields.Null && s != e && s != f && Tree.Compare(e.Key_Renamed, s.Key_Renamed) < 0) // e not already past s
				{
					Side = 1;
					return new ValueSpliterator<> (Tree, e, Current = s, -1, Est = (int)((uint)Est >> 1), ExpectedModCount);
				}
				return Map_Fields.Null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base V> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				TreeMap.Entry<K, V> f = Fence, e , p , pl ;
				if ((e = Current) != Map_Fields.Null && e != f)
				{
					Current = f; // exhaust
					do
					{
						action.Accept(e.Value_Renamed);
						if ((p = e.Right) != Map_Fields.Null)
						{
							while ((pl = p.Left) != Map_Fields.Null)
							{
								p = pl;
							}
						}
						else
						{
							while ((p = e.Parent) != Map_Fields.Null && e == p.Right)
							{
								e = p;
							}
						}
					} while ((e = p) != Map_Fields.Null && e != f);
					if (Tree.ModCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base V> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				TreeMap.Entry<K, V> e;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				if ((e = Current) == Map_Fields.Null || e == Fence)
				{
					return Map_Fields.False;
				}
				Current = Successor(e);
				action.Accept(e.Value_Renamed);
				if (Tree.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				return Map_Fields.True;
			}

			public int Characteristics()
			{
				return (Side == 0 ? Spliterator_Fields.SIZED : 0) | Spliterator_Fields.ORDERED;
			}
		}

		internal sealed class EntrySpliterator<K, V> : TreeMapSpliterator<K, V>, Spliterator<Map_Entry<K, V>>
		{
			internal EntrySpliterator(TreeMap<K, V> tree, TreeMap.Entry<K, V> origin, TreeMap.Entry<K, V> fence, int side, int est, int expectedModCount) : base(tree, origin, fence, side, est, expectedModCount)
			{
			}

			public EntrySpliterator<K, V> TrySplit()
			{
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				int d = Side;
				TreeMap.Entry<K, V> e = Current, f = Fence, s = ((e == Map_Fields.Null || e == f) ? Map_Fields.Null : (d == 0) ? Tree.Root : (d > 0) ? e.Right : (d < 0 && f != Map_Fields.Null) ? f.Left : Map_Fields.Null); // was left -  was right -  was top -  empty
				if (s != Map_Fields.Null && s != e && s != f && Tree.Compare(e.Key_Renamed, s.Key_Renamed) < 0) // e not already past s
				{
					Side = 1;
					return new EntrySpliterator<> (Tree, e, Current = s, -1, Est = (int)((uint)Est >> 1), ExpectedModCount);
				}
				return Map_Fields.Null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base Map_Entry<K, V>> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				TreeMap.Entry<K, V> f = Fence, e , p , pl ;
				if ((e = Current) != Map_Fields.Null && e != f)
				{
					Current = f; // exhaust
					do
					{
						action.Accept(e);
						if ((p = e.Right) != Map_Fields.Null)
						{
							while ((pl = p.Left) != Map_Fields.Null)
							{
								p = pl;
							}
						}
						else
						{
							while ((p = e.Parent) != Map_Fields.Null && e == p.Right)
							{
								e = p;
							}
						}
					} while ((e = p) != Map_Fields.Null && e != f);
					if (Tree.ModCount != ExpectedModCount)
					{
						throw new ConcurrentModificationException();
					}
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base Map_Entry<K,V>> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				TreeMap.Entry<K, V> e;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (Est < 0)
				{
					Estimate; // force initialization
				}
				if ((e = Current) == Map_Fields.Null || e == Fence)
				{
					return Map_Fields.False;
				}
				Current = Successor(e);
				action.Accept(e);
				if (Tree.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				return Map_Fields.True;
			}

			public int Characteristics()
			{
				return (Side == 0 ? Spliterator_Fields.SIZED : 0) | Spliterator_Fields.DISTINCT | Spliterator_Fields.SORTED | Spliterator_Fields.ORDERED;
			}

			public override Comparator<Map_Entry<K, V>> Comparator
			{
				get
				{
					// Adapt or create a key-based comparator
					if (Tree.Comparator_Renamed != Map_Fields.Null)
					{
						return Map_Entry.comparingByKey(Tree.Comparator_Renamed);
					}
					else
					{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
						return (Comparator<Map_Entry<K, V>> & Serializable)(e1, e2) =>
						{
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("unchecked") Comparable<? base K> k1 = (Comparable<? base K>) e1.getKey();
	//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Comparable<?> k1 = (Comparable<?>) e1.Key;
							return k1.CompareTo(e2.Key);
						};
					}
				}
			}
		}
	}

}