using System;
using System.Collections;
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
	/// Hash table based implementation of the <tt>Map</tt> interface, with
	/// <em>weak keys</em>.
	/// An entry in a <tt>WeakHashMap</tt> will automatically be removed when
	/// its key is no longer in ordinary use.  More precisely, the presence of a
	/// mapping for a given key will not prevent the key from being discarded by the
	/// garbage collector, that is, made finalizable, finalized, and then reclaimed.
	/// When a key has been discarded its entry is effectively removed from the map,
	/// so this class behaves somewhat differently from other <tt>Map</tt>
	/// implementations.
	/// 
	/// <para> Both null values and the null key are supported. This class has
	/// performance characteristics similar to those of the <tt>HashMap</tt>
	/// class, and has the same efficiency parameters of <em>initial capacity</em>
	/// and <em>load factor</em>.
	/// 
	/// </para>
	/// <para> Like most collection classes, this class is not synchronized.
	/// A synchronized <tt>WeakHashMap</tt> may be constructed using the
	/// <seealso cref="Collections#synchronizedMap Collections.synchronizedMap"/>
	/// method.
	/// 
	/// </para>
	/// <para> This class is intended primarily for use with key objects whose
	/// <tt>equals</tt> methods test for object identity using the
	/// <tt>==</tt> operator.  Once such a key is discarded it can never be
	/// recreated, so it is impossible to do a lookup of that key in a
	/// <tt>WeakHashMap</tt> at some later time and be surprised that its entry
	/// has been removed.  This class will work perfectly well with key objects
	/// whose <tt>equals</tt> methods are not based upon object identity, such
	/// as <tt>String</tt> instances.  With such recreatable key objects,
	/// however, the automatic removal of <tt>WeakHashMap</tt> entries whose
	/// keys have been discarded may prove to be confusing.
	/// 
	/// </para>
	/// <para> The behavior of the <tt>WeakHashMap</tt> class depends in part upon
	/// the actions of the garbage collector, so several familiar (though not
	/// required) <tt>Map</tt> invariants do not hold for this class.  Because
	/// the garbage collector may discard keys at any time, a
	/// <tt>WeakHashMap</tt> may behave as though an unknown thread is silently
	/// removing entries.  In particular, even if you synchronize on a
	/// <tt>WeakHashMap</tt> instance and invoke none of its mutator methods, it
	/// is possible for the <tt>size</tt> method to return smaller values over
	/// time, for the <tt>isEmpty</tt> method to return <tt>false</tt> and
	/// then <tt>true</tt>, for the <tt>containsKey</tt> method to return
	/// <tt>true</tt> and later <tt>false</tt> for a given key, for the
	/// <tt>get</tt> method to return a value for a given key but later return
	/// <tt>null</tt>, for the <tt>put</tt> method to return
	/// <tt>null</tt> and the <tt>remove</tt> method to return
	/// <tt>false</tt> for a key that previously appeared to be in the map, and
	/// for successive examinations of the key set, the value collection, and
	/// the entry set to yield successively smaller numbers of elements.
	/// 
	/// </para>
	/// <para> Each key object in a <tt>WeakHashMap</tt> is stored indirectly as
	/// the referent of a weak reference.  Therefore a key will automatically be
	/// removed only after the weak references to it, both inside and outside of the
	/// map, have been cleared by the garbage collector.
	/// 
	/// </para>
	/// <para> <strong>Implementation note:</strong> The value objects in a
	/// <tt>WeakHashMap</tt> are held by ordinary strong references.  Thus care
	/// should be taken to ensure that value objects do not strongly refer to their
	/// own keys, either directly or indirectly, since that will prevent the keys
	/// from being discarded.  Note that a value object may refer indirectly to its
	/// key via the <tt>WeakHashMap</tt> itself; that is, a value object may
	/// strongly refer to some other key object whose associated value object, in
	/// turn, strongly refers to the key of the first value object.  If the values
	/// in the map do not rely on the map holding strong references to them, one way
	/// to deal with this is to wrap values themselves within
	/// <tt>WeakReferences</tt> before
	/// inserting, as in: <tt>m.put(key, new WeakReference(value))</tt>,
	/// and then unwrapping upon each <tt>get</tt>.
	/// 
	/// </para>
	/// <para>The iterators returned by the <tt>iterator</tt> method of the collections
	/// returned by all of this class's "collection view methods" are
	/// <i>fail-fast</i>: if the map is structurally modified at any time after the
	/// iterator is created, in any way except through the iterator's own
	/// <tt>remove</tt> method, the iterator will throw a {@link
	/// ConcurrentModificationException}.  Thus, in the face of concurrent
	/// modification, the iterator fails quickly and cleanly, rather than risking
	/// arbitrary, non-deterministic behavior at an undetermined time in the future.
	/// 
	/// </para>
	/// <para>Note that the fail-fast behavior of an iterator cannot be guaranteed
	/// as it is, generally speaking, impossible to make any hard guarantees in the
	/// presence of unsynchronized concurrent modification.  Fail-fast iterators
	/// throw <tt>ConcurrentModificationException</tt> on a best-effort basis.
	/// Therefore, it would be wrong to write a program that depended on this
	/// exception for its correctness:  <i>the fail-fast behavior of iterators
	/// should be used only to detect bugs.</i>
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
	/// @author      Doug Lea
	/// @author      Josh Bloch
	/// @author      Mark Reinhold
	/// @since       1.2 </param>
	/// <seealso cref=         java.util.HashMap </seealso>
	/// <seealso cref=         java.lang.ref.WeakReference </seealso>
	public class WeakHashMap<K, V> : AbstractMap<K, V>, Map<K, V>
	{

		/// <summary>
		/// The default initial capacity -- MUST be a power of two.
		/// </summary>
		private const int DEFAULT_INITIAL_CAPACITY = 16;

		/// <summary>
		/// The maximum capacity, used if a higher value is implicitly specified
		/// by either of the constructors with arguments.
		/// MUST be a power of two <= 1<<30.
		/// </summary>
		private static readonly int MAXIMUM_CAPACITY = 1 << 30;

		/// <summary>
		/// The load factor used when none specified in constructor.
		/// </summary>
		private const float DEFAULT_LOAD_FACTOR = 0.75f;

		/// <summary>
		/// The table, resized as necessary. Length MUST Always be a power of two.
		/// </summary>
		internal Entry<K, V>[] Table_Renamed;

		/// <summary>
		/// The number of key-value mappings contained in this weak hash map.
		/// </summary>
		private int Size_Renamed;

		/// <summary>
		/// The next size value at which to resize (capacity * load factor).
		/// </summary>
		private int Threshold;

		/// <summary>
		/// The load factor for the hash table.
		/// </summary>
		private readonly float LoadFactor;

		/// <summary>
		/// Reference queue for cleared WeakEntries
		/// </summary>
		private readonly ReferenceQueue<Object> Queue = new ReferenceQueue<Object>();

		/// <summary>
		/// The number of times this WeakHashMap has been structurally modified.
		/// Structural modifications are those that change the number of
		/// mappings in the map or otherwise modify its internal structure
		/// (e.g., rehash).  This field is used to make iterators on
		/// Collection-views of the map fail-fast.
		/// </summary>
		/// <seealso cref= ConcurrentModificationException </seealso>
		internal int ModCount;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private Entry<K,V>[] newTable(int n)
		private Entry<K, V>[] NewTable(int n)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (Entry<K,V>[]) new Entry<?,?>[n];
			return (Entry<K, V>[]) new Entry<?, ?>[n];
		}

		/// <summary>
		/// Constructs a new, empty <tt>WeakHashMap</tt> with the given initial
		/// capacity and the given load factor.
		/// </summary>
		/// <param name="initialCapacity"> The initial capacity of the <tt>WeakHashMap</tt> </param>
		/// <param name="loadFactor">      The load factor of the <tt>WeakHashMap</tt> </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is negative,
		///         or if the load factor is nonpositive. </exception>
		public WeakHashMap(int initialCapacity, float loadFactor)
		{
			if (initialCapacity < 0)
			{
				throw new IllegalArgumentException("Illegal Initial Capacity: " + initialCapacity);
			}
			if (initialCapacity > MAXIMUM_CAPACITY)
			{
				initialCapacity = MAXIMUM_CAPACITY;
			}

			if (loadFactor <= 0 || Float.IsNaN(loadFactor))
			{
				throw new IllegalArgumentException("Illegal Load factor: " + loadFactor);
			}
			int capacity = 1;
			while (capacity < initialCapacity)
			{
				capacity <<= 1;
			}
			Table_Renamed = NewTable(capacity);
			this.LoadFactor = loadFactor;
			Threshold = (int)(capacity * loadFactor);
		}

		/// <summary>
		/// Constructs a new, empty <tt>WeakHashMap</tt> with the given initial
		/// capacity and the default load factor (0.75).
		/// </summary>
		/// <param name="initialCapacity"> The initial capacity of the <tt>WeakHashMap</tt> </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is negative </exception>
		public WeakHashMap(int initialCapacity) : this(initialCapacity, DEFAULT_LOAD_FACTOR)
		{
		}

		/// <summary>
		/// Constructs a new, empty <tt>WeakHashMap</tt> with the default initial
		/// capacity (16) and load factor (0.75).
		/// </summary>
		public WeakHashMap() : this(DEFAULT_INITIAL_CAPACITY, DEFAULT_LOAD_FACTOR)
		{
		}

		/// <summary>
		/// Constructs a new <tt>WeakHashMap</tt> with the same mappings as the
		/// specified map.  The <tt>WeakHashMap</tt> is created with the default
		/// load factor (0.75) and an initial capacity sufficient to hold the
		/// mappings in the specified map.
		/// </summary>
		/// <param name="m"> the map whose mappings are to be placed in this map </param>
		/// <exception cref="NullPointerException"> if the specified map is null
		/// @since   1.3 </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public WeakHashMap(Map<? extends K, ? extends V> m)
		public WeakHashMap<T1>(Map<T1> m) where T1 : K where ? : V : this(System.Math.Max((int)(m.Size() / DEFAULT_LOAD_FACTOR) + 1, DEFAULT_INITIAL_CAPACITY), DEFAULT_LOAD_FACTOR)
		{
			PutAll(m);
		}

		// internal utilities

		/// <summary>
		/// Value representing null keys inside tables.
		/// </summary>
		private static readonly Object NULL_KEY = new Object();

		/// <summary>
		/// Use NULL_KEY for key if it is null.
		/// </summary>
		private static Object MaskNull(Object key)
		{
			return (key == Map_Fields.Null) ? NULL_KEY : key;
		}

		/// <summary>
		/// Returns internal representation of null key back to caller as null.
		/// </summary>
		internal static Object UnmaskNull(Object key)
		{
			return (key == NULL_KEY) ? Map_Fields.Null : key;
		}

		/// <summary>
		/// Checks for equality of non-null reference x and possibly-null y.  By
		/// default uses Object.equals.
		/// </summary>
		private static bool Eq(Object x, Object y)
		{
			return x == y || x.Equals(y);
		}

		/// <summary>
		/// Retrieve object hash code and applies a supplemental hash function to the
		/// result hash, which defends against poor quality hash functions.  This is
		/// critical because HashMap uses power-of-two length hash tables, that
		/// otherwise encounter collisions for hashCodes that do not differ
		/// in lower bits.
		/// </summary>
		internal int Hash(Object Map_Fields)
		{
			int h = Map_Fields.k.HashCode();

			// This function ensures that hashCodes that differ only by
			// constant multiples at each bit position have a bounded
			// number of collisions (approximately 8 at default load factor).
			h ^= ((int)((uint)h >> 20)) ^ ((int)((uint)h >> 12));
			return h ^ ((int)((uint)h >> 7)) ^ ((int)((uint)h >> 4));
		}

		/// <summary>
		/// Returns index for hash code h.
		/// </summary>
		private static int IndexFor(int h, int length)
		{
			return h & (length - 1);
		}

		/// <summary>
		/// Expunges stale entries from the table.
		/// </summary>
		private void ExpungeStaleEntries()
		{
			for (Object x; (x = Queue.poll()) != Map_Fields.Null;)
			{
				lock (Queue)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>) x;
					Entry<K, V> e = (Entry<K, V>) x;
					int i = IndexFor(e.Hash, Table_Renamed.Length);

					Entry<K, V> prev = Table_Renamed[i];
					Entry<K, V> p = prev;
					while (p != Map_Fields.Null)
					{
						Entry<K, V> next = p.Next;
						if (p == e)
						{
							if (prev == e)
							{
								Table_Renamed[i] = next;
							}
							else
							{
								prev.Next = next;
							}
							// Must not null out e.next;
							// stale entries may be in use by a HashIterator
							e.Value_Renamed = Map_Fields.Null; // Help GC
							Size_Renamed--;
							break;
						}
						prev = p;
						p = next;
					}
				}
			}
		}

		/// <summary>
		/// Returns the table after first expunging stale entries.
		/// </summary>
		private Entry<K, V>[] Table
		{
			get
			{
				ExpungeStaleEntries();
				return Table_Renamed;
			}
		}

		/// <summary>
		/// Returns the number of key-value mappings in this map.
		/// This result is a snapshot, and may not reflect unprocessed
		/// entries that will be removed before next attempted access
		/// because they are no longer referenced.
		/// </summary>
		public virtual int Size()
		{
			if (Size_Renamed == 0)
			{
				return 0;
			}
			ExpungeStaleEntries();
			return Size_Renamed;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this map contains no key-value mappings.
		/// This result is a snapshot, and may not reflect unprocessed
		/// entries that will be removed before next attempted access
		/// because they are no longer referenced.
		/// </summary>
		public virtual bool Empty
		{
			get
			{
				return Size() == 0;
			}
		}

		/// <summary>
		/// Returns the value to which the specified key is mapped,
		/// or {@code null} if this map contains no mapping for the key.
		/// 
		/// <para>More formally, if this map contains a mapping from a key
		/// {@code k} to a value {@code v} such that {@code (key==null ? k==null :
		/// key.equals(k))}, then this method returns {@code v}; otherwise
		/// it returns {@code null}.  (There can be at most one such mapping.)
		/// 
		/// </para>
		/// <para>A return value of {@code null} does not <i>necessarily</i>
		/// indicate that the map contains no mapping for the key; it's also
		/// possible that the map explicitly maps the key to {@code null}.
		/// The <seealso cref="#containsKey containsKey"/> operation may be used to
		/// distinguish these two cases.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #put(Object, Object) </seealso>
		public virtual V Get(Object key)
		{
			Object Map_Fields.k = MaskNull(key);
			int h = Hash(Map_Fields.k);
			Entry<K, V>[] tab = Table;
			int index = IndexFor(h, tab.Length);
			Entry<K, V> e = tab[index];
			while (e != Map_Fields.Null)
			{
				if (e.Hash == h && Eq(Map_Fields.k, e.get()))
				{
					return e.Value_Renamed;
				}
				e = e.Next;
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this map contains a mapping for the
		/// specified key.
		/// </summary>
		/// <param name="key">   The key whose presence in this map is to be tested </param>
		/// <returns> <tt>true</tt> if there is a mapping for <tt>key</tt>;
		///         <tt>false</tt> otherwise </returns>
		public virtual bool ContainsKey(Object key)
		{
			return GetEntry(key) != Map_Fields.Null;
		}

		/// <summary>
		/// Returns the entry associated with the specified key in this map.
		/// Returns null if the map contains no mapping for this key.
		/// </summary>
		internal virtual Entry<K, V> GetEntry(Object key)
		{
			Object Map_Fields.k = MaskNull(key);
			int h = Hash(Map_Fields.k);
			Entry<K, V>[] tab = Table;
			int index = IndexFor(h, tab.Length);
			Entry<K, V> e = tab[index];
			while (e != Map_Fields.Null && !(e.Hash == h && Eq(Map_Fields.k, e.get())))
			{
				e = e.Next;
			}
			return e;
		}

		/// <summary>
		/// Associates the specified value with the specified key in this map.
		/// If the map previously contained a mapping for this key, the old
		/// value is replaced.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated. </param>
		/// <param name="value"> value to be associated with the specified key. </param>
		/// <returns> the previous value associated with <tt>key</tt>, or
		///         <tt>null</tt> if there was no mapping for <tt>key</tt>.
		///         (A <tt>null</tt> return can also indicate that the map
		///         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
		public virtual V Put(K key, V value)
		{
			Object Map_Fields.k = MaskNull(key);
			int h = Hash(Map_Fields.k);
			Entry<K, V>[] tab = Table;
			int i = IndexFor(h, tab.Length);

			for (Entry<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
			{
				if (h == e.Hash && Eq(Map_Fields.k, e.get()))
				{
					V Map_Fields.OldValue = e.Value_Renamed;
					if (value != Map_Fields.OldValue)
					{
						e.Value_Renamed = value;
					}
					return Map_Fields.OldValue;
				}
			}

			ModCount++;
			Entry<K, V> e = tab[i];
			tab[i] = new Entry<>(Map_Fields.k, value, Queue, h, e);
			if (++Size_Renamed >= Threshold)
			{
				Resize(tab.Length * 2);
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Rehashes the contents of this map into a new array with a
		/// larger capacity.  This method is called automatically when the
		/// number of keys in this map reaches its threshold.
		/// 
		/// If current capacity is MAXIMUM_CAPACITY, this method does not
		/// resize the map, but sets threshold to Integer.MAX_VALUE.
		/// This has the effect of preventing future calls.
		/// </summary>
		/// <param name="newCapacity"> the new capacity, MUST be a power of two;
		///        must be greater than current capacity unless current
		///        capacity is MAXIMUM_CAPACITY (in which case value
		///        is irrelevant). </param>
		internal virtual void Resize(int newCapacity)
		{
			Entry<K, V>[] oldTable = Table;
			int oldCapacity = oldTable.Length;
			if (oldCapacity == MAXIMUM_CAPACITY)
			{
				Threshold = Integer.MaxValue;
				return;
			}

			Entry<K, V>[] newTable = NewTable(newCapacity);
			Transfer(oldTable, newTable);
			Table_Renamed = newTable;

			/*
			 * If ignoring null elements and processing ref queue caused massive
			 * shrinkage, then restore old table.  This should be rare, but avoids
			 * unbounded expansion of garbage-filled tables.
			 */
			if (Size_Renamed >= Threshold / 2)
			{
				Threshold = (int)(newCapacity * LoadFactor);
			}
			else
			{
				ExpungeStaleEntries();
				Transfer(newTable, oldTable);
				Table_Renamed = oldTable;
			}
		}

		/// <summary>
		/// Transfers all entries from src to dest tables </summary>
		private void Transfer(Entry<K, V>[] src, Entry<K, V>[] dest)
		{
			for (int j = 0; j < src.Length; ++j)
			{
				Entry<K, V> e = src[j];
				src[j] = Map_Fields.Null;
				while (e != Map_Fields.Null)
				{
					Entry<K, V> next = e.Next;
					Object key = e.get();
					if (key == Map_Fields.Null)
					{
						e.Next = Map_Fields.Null; // Help GC
						e.Value_Renamed = Map_Fields.Null; //  "   "
						Size_Renamed--;
					}
					else
					{
						int i = IndexFor(e.Hash, dest.Length);
						e.Next = dest[i];
						dest[i] = e;
					}
					e = next;
				}
			}
		}

		/// <summary>
		/// Copies all of the mappings from the specified map to this map.
		/// These mappings will replace any mappings that this map had for any
		/// of the keys currently in the specified map.
		/// </summary>
		/// <param name="m"> mappings to be stored in this map. </param>
		/// <exception cref="NullPointerException"> if the specified map is null. </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(Map<? extends K, ? extends V> m)
		public virtual void putAll<T1>(Map<T1> m) where T1 : K where ? : V
		{
			int numKeysToBeAdded = m.Size();
			if (numKeysToBeAdded == 0)
			{
				return;
			}

			/*
			 * Expand the map if the map if the number of mappings to be added
			 * is greater than or equal to threshold.  This is conservative; the
			 * obvious condition is (m.size() + size) >= threshold, but this
			 * condition could result in a map with twice the appropriate capacity,
			 * if the keys to be added overlap with the keys already in this map.
			 * By using the conservative calculation, we subject ourself
			 * to at most one extra resize.
			 */
			if (numKeysToBeAdded > Threshold)
			{
				int targetCapacity = (int)(numKeysToBeAdded / LoadFactor + 1);
				if (targetCapacity > MAXIMUM_CAPACITY)
				{
					targetCapacity = MAXIMUM_CAPACITY;
				}
				int newCapacity = Table_Renamed.Length;
				while (newCapacity < targetCapacity)
				{
					newCapacity <<= 1;
				}
				if (newCapacity > Table_Renamed.Length)
				{
					Resize(newCapacity);
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Map_Entry<? extends K, ? extends V> e : m.entrySet())
			foreach (Map_Entry<?, ?> e in m.EntrySet())
			{
				Put(e.Key, e.Value);
			}
		}

		/// <summary>
		/// Removes the mapping for a key from this weak hash map if it is present.
		/// More formally, if this map contains a mapping from key <tt>k</tt> to
		/// value <tt>v</tt> such that <code>(key==null ?  k==null :
		/// key.equals(k))</code>, that mapping is removed.  (The map can contain
		/// at most one such mapping.)
		/// 
		/// <para>Returns the value to which this map previously associated the key,
		/// or <tt>null</tt> if the map contained no mapping for the key.  A
		/// return value of <tt>null</tt> does not <i>necessarily</i> indicate
		/// that the map contained no mapping for the key; it's also possible
		/// that the map explicitly mapped the key to <tt>null</tt>.
		/// 
		/// </para>
		/// <para>The map will not contain a mapping for the specified key once the
		/// call returns.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key whose mapping is to be removed from the map </param>
		/// <returns> the previous value associated with <tt>key</tt>, or
		///         <tt>null</tt> if there was no mapping for <tt>key</tt> </returns>
		public virtual V Remove(Object key)
		{
			Object Map_Fields.k = MaskNull(key);
			int h = Hash(Map_Fields.k);
			Entry<K, V>[] tab = Table;
			int i = IndexFor(h, tab.Length);
			Entry<K, V> prev = tab[i];
			Entry<K, V> e = prev;

			while (e != Map_Fields.Null)
			{
				Entry<K, V> next = e.Next;
				if (h == e.Hash && Eq(Map_Fields.k, e.get()))
				{
					ModCount++;
					Size_Renamed--;
					if (prev == e)
					{
						tab[i] = next;
					}
					else
					{
						prev.Next = next;
					}
					return e.Value_Renamed;
				}
				prev = e;
				e = next;
			}

			return Map_Fields.Null;
		}

		/// <summary>
		/// Special version of remove needed by Entry set </summary>
		internal virtual bool RemoveMapping(Object o)
		{
			if (!(o is Map_Entry))
			{
				return Map_Fields.False;
			}
			Entry<K, V>[] tab = Table;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> entry = (Map_Entry<?,?>)o;
			Map_Entry<?, ?> entry = (Map_Entry<?, ?>)o;
			Object Map_Fields.k = MaskNull(entry.Key);
			int h = Hash(Map_Fields.k);
			int i = IndexFor(h, tab.Length);
			Entry<K, V> prev = tab[i];
			Entry<K, V> e = prev;

			while (e != Map_Fields.Null)
			{
				Entry<K, V> next = e.Next;
				if (h == e.Hash && e.Equals(entry))
				{
					ModCount++;
					Size_Renamed--;
					if (prev == e)
					{
						tab[i] = next;
					}
					else
					{
						prev.Next = next;
					}
					return Map_Fields.True;
				}
				prev = e;
				e = next;
			}

			return Map_Fields.False;
		}

		/// <summary>
		/// Removes all of the mappings from this map.
		/// The map will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			// clear out ref queue. We don't need to expunge entries
			// since table is getting cleared.
			while (Queue.poll() != Map_Fields.Null)
			{
				;
			}

			ModCount++;
			Arrays.Fill(Table_Renamed, Map_Fields.Null);
			Size_Renamed = 0;

			// Allocation of array may have caused GC, which may have caused
			// additional entries to go stale.  Removing these entries from the
			// reference queue will make them eligible for reclamation.
			while (Queue.poll() != Map_Fields.Null)
			{
				;
			}
		}

		/// <summary>
		/// Returns <tt>true</tt> if this map maps one or more keys to the
		/// specified value.
		/// </summary>
		/// <param name="value"> value whose presence in this map is to be tested </param>
		/// <returns> <tt>true</tt> if this map maps one or more keys to the
		///         specified value </returns>
		public virtual bool ContainsValue(Object value)
		{
			if (value == Map_Fields.Null)
			{
				return ContainsNullValue();
			}

			Entry<K, V>[] tab = Table;
			for (int i = tab.Length; i-- > 0;)
			{
				for (Entry<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
				{
					if (value.Equals(e.Value_Renamed))
					{
						return Map_Fields.True;
					}
				}
			}
			return Map_Fields.False;
		}

		/// <summary>
		/// Special-case code for containsValue with null argument
		/// </summary>
		private bool ContainsNullValue()
		{
			Entry<K, V>[] tab = Table;
			for (int i = tab.Length; i-- > 0;)
			{
				for (Entry<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
				{
					if (e.Value_Renamed == Map_Fields.Null)
					{
						return Map_Fields.True;
					}
				}
			}
			return Map_Fields.False;
		}

		/// <summary>
		/// The entries in this hash table extend WeakReference, using its main ref
		/// field as the key.
		/// </summary>
		private class Entry<K, V> : WeakReference<Object>, Map_Entry<K, V>
		{
			internal V Value_Renamed;
			internal readonly int Hash;
			internal Entry<K, V> Next;

			/// <summary>
			/// Creates new entry.
			/// </summary>
			internal Entry(Object key, V value, ReferenceQueue<Object> queue, int hash, Entry<K, V> next) : base(key, queue)
			{
				this.Value_Renamed = value;
				this.Hash = hash;
				this.Next = next;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public K getKey()
			public virtual K Key
			{
				get
				{
					return (K) WeakHashMap.UnmaskNull(get());
				}
			}

			public virtual V Value
			{
				get
				{
					return Value_Renamed;
				}
			}

			public virtual V SetValue(V Map_Fields)
			{
				V Map_Fields.OldValue = Value_Renamed;
				Value_Renamed = Map_Fields.NewValue;
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
				K k1 = Key;
				Object k2 = e.Key;
				if (k1 == k2 || (k1 != Map_Fields.Null && k1.Equals(k2)))
				{
					V v1 = Value;
					Object v2 = e.Value;
					if (v1 == v2 || (v1 != Map_Fields.Null && v1.Equals(v2)))
					{
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}

			public override int HashCode()
			{
				K Map_Fields.k = Key;
				V Map_Fields.v = Value;
				return Objects.HashCode(Map_Fields.k) ^ Objects.HashCode(Map_Fields.v);
			}

			public override String ToString()
			{
				return Key + "=" + Value;
			}
		}

		private abstract class HashIterator<T> : Iterator<T>
		{
			internal bool InstanceFieldsInitialized = Map_Fields.False;

			internal virtual void InitializeInstanceFields()
			{
				ExpectedModCount = outerInstance.ModCount;
			}

			private readonly WeakHashMap<K, V> OuterInstance;

			internal int Index;
			internal Entry<K, V> Entry;
			internal Entry<K, V> LastReturned;
			internal int ExpectedModCount;

			/// <summary>
			/// Strong reference needed to avoid disappearance of key
			/// between hasNext and next
			/// </summary>
			internal Object NextKey;

			/// <summary>
			/// Strong reference needed to avoid disappearance of key
			/// between nextEntry() and any use of the entry
			/// </summary>
			internal Object CurrentKey;

			internal HashIterator(WeakHashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = Map_Fields.True;
				}
				Index = outerInstance.Empty ? 0 : outerInstance.Table_Renamed.Length;
			}

			public virtual bool HasNext()
			{
				Entry<K, V>[] t = outerInstance.Table_Renamed;

				while (NextKey == Map_Fields.Null)
				{
					Entry<K, V> e = Entry;
					int i = Index;
					while (e == Map_Fields.Null && i > 0)
					{
						e = t[--i];
					}
					Entry = e;
					Index = i;
					if (e == Map_Fields.Null)
					{
						CurrentKey = Map_Fields.Null;
						return Map_Fields.False;
					}
					NextKey = e.get(); // hold on to key in strong ref
					if (NextKey == Map_Fields.Null)
					{
						Entry = Entry.Next;
					}
				}
				return Map_Fields.True;
			}

			/// <summary>
			/// The common parts of next() across different types of iterators </summary>
			protected internal virtual Entry<K, V> NextEntry()
			{
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				if (NextKey == Map_Fields.Null && !HasNext())
				{
					throw new NoSuchElementException();
				}

				LastReturned = Entry;
				Entry = Entry.Next;
				CurrentKey = NextKey;
				NextKey = Map_Fields.Null;
				return LastReturned;
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

				OuterInstance.Remove(CurrentKey);
				ExpectedModCount = outerInstance.ModCount;
				LastReturned = Map_Fields.Null;
				CurrentKey = Map_Fields.Null;
			}

		}

		private class ValueIterator : HashIterator<V>
		{
			private readonly WeakHashMap<K, V> OuterInstance;

			public ValueIterator(WeakHashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual V Next()
			{
				return NextEntry().Value_Renamed;
			}
		}

		private class KeyIterator : HashIterator<K>
		{
			private readonly WeakHashMap<K, V> OuterInstance;

			public KeyIterator(WeakHashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual K Next()
			{
				return NextEntry().Key;
			}
		}

		private class EntryIterator : HashIterator<Map_Entry<K, V>>
		{
			private readonly WeakHashMap<K, V> OuterInstance;

			public EntryIterator(WeakHashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Map_Entry<K, V> Next()
			{
				return NextEntry();
			}
		}

		// Views

		[NonSerialized]
		private Set<Map_Entry<K, V>> EntrySet_Renamed;

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the keys contained in this map.
		/// The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  If the map is modified
		/// while an iteration over the set is in progress (except through
		/// the iterator's own <tt>remove</tt> operation), the results of
		/// the iteration are undefined.  The set supports element removal,
		/// which removes the corresponding mapping from the map, via the
		/// <tt>Iterator.remove</tt>, <tt>Set.remove</tt>,
		/// <tt>removeAll</tt>, <tt>retainAll</tt>, and <tt>clear</tt>
		/// operations.  It does not support the <tt>add</tt> or <tt>addAll</tt>
		/// operations.
		/// </summary>
		public virtual Set<K> KeySet()
		{
			Set<K> ks = KeySet_Renamed;
			return (ks != Map_Fields.Null ? ks : (KeySet_Renamed = new KeySet(this)));
		}

		private class KeySet : AbstractSet<K>
		{
			private readonly WeakHashMap<K, V> OuterInstance;

			public KeySet(WeakHashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<K> Iterator()
			{
				return new KeyIterator(OuterInstance);
			}

			public virtual int Size()
			{
				return OuterInstance.Size();
			}

			public virtual bool Contains(Object o)
			{
				return outerInstance.ContainsKey(o);
			}

			public virtual bool Remove(Object o)
			{
				if (outerInstance.ContainsKey(o))
				{
					OuterInstance.Remove(o);
					return Map_Fields.True;
				}
				else
				{
					return Map_Fields.False;
				}
			}

			public virtual void Clear()
			{
				OuterInstance.Clear();
			}

			public virtual Spliterator<K> Spliterator()
			{
				return new KeySpliterator<>(OuterInstance, 0, -1, 0, 0);
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Collection"/> view of the values contained in this map.
		/// The collection is backed by the map, so changes to the map are
		/// reflected in the collection, and vice-versa.  If the map is
		/// modified while an iteration over the collection is in progress
		/// (except through the iterator's own <tt>remove</tt> operation),
		/// the results of the iteration are undefined.  The collection
		/// supports element removal, which removes the corresponding
		/// mapping from the map, via the <tt>Iterator.remove</tt>,
		/// <tt>Collection.remove</tt>, <tt>removeAll</tt>,
		/// <tt>retainAll</tt> and <tt>clear</tt> operations.  It does not
		/// support the <tt>add</tt> or <tt>addAll</tt> operations.
		/// </summary>
		public virtual Collection<V> Values()
		{
			Collection<V> vs = Values_Renamed;
			return (vs != Map_Fields.Null) ? vs : (Values_Renamed = new Values(this));
		}

		private class Values : AbstractCollection<V>
		{
			private readonly WeakHashMap<K, V> OuterInstance;

			public Values(WeakHashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<V> Iterator()
			{
				return new ValueIterator(OuterInstance);
			}

			public virtual int Size()
			{
				return OuterInstance.Size();
			}

			public virtual bool Contains(Object o)
			{
				return outerInstance.ContainsValue(o);
			}

			public virtual void Clear()
			{
				OuterInstance.Clear();
			}

			public virtual Spliterator<V> Spliterator()
			{
				return new ValueSpliterator<>(OuterInstance, 0, -1, 0, 0);
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		/// The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  If the map is modified
		/// while an iteration over the set is in progress (except through
		/// the iterator's own <tt>remove</tt> operation, or through the
		/// <tt>setValue</tt> operation on a map entry returned by the
		/// iterator) the results of the iteration are undefined.  The set
		/// supports element removal, which removes the corresponding
		/// mapping from the map, via the <tt>Iterator.remove</tt>,
		/// <tt>Set.remove</tt>, <tt>removeAll</tt>, <tt>retainAll</tt> and
		/// <tt>clear</tt> operations.  It does not support the
		/// <tt>add</tt> or <tt>addAll</tt> operations.
		/// </summary>
		public virtual Set<Map_Entry<K, V>> EntrySet()
		{
			Set<Map_Entry<K, V>> es = EntrySet_Renamed;
			return es != Map_Fields.Null ? es : (EntrySet_Renamed = new EntrySet(this));
		}

		private class EntrySet : AbstractSet<Map_Entry<K, V>>
		{
			private readonly WeakHashMap<K, V> OuterInstance;

			public EntrySet(WeakHashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<Map_Entry<K, V>> Iterator()
			{
				return new EntryIterator(OuterInstance);
			}

			public virtual bool Contains(Object o)
			{
				if (!(o is Map_Entry))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>)o;
				Map_Entry<?, ?> e = (Map_Entry<?, ?>)o;
				Entry<K, V> candidate = outerInstance.GetEntry(e.Key);
				return candidate != Map_Fields.Null && candidate.Equals(e);
			}

			public virtual bool Remove(Object o)
			{
				return outerInstance.RemoveMapping(o);
			}

			public virtual int Size()
			{
				return OuterInstance.Size();
			}

			public virtual void Clear()
			{
				OuterInstance.Clear();
			}

			internal virtual List<Map_Entry<K, V>> DeepCopy()
			{
				List<Map_Entry<K, V>> list = new List<Map_Entry<K, V>>(Size());
				foreach (Map_Entry<K, V> e in this)
				{
					list.Add(new AbstractMap.SimpleEntry<>(e));
				}
				return list;
			}

			public virtual Object[] ToArray()
			{
				return DeepCopy().ToArray();
			}

			public virtual T[] toArray<T>(T[] a)
			{
				return DeepCopy().ToArray(a);
			}

			public virtual Spliterator<Map_Entry<K, V>> Spliterator()
			{
				return new EntrySpliterator<>(OuterInstance, 0, -1, 0, 0);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public override void forEach<T1>(BiConsumer<T1> action)
		{
			Objects.RequireNonNull(action);
			int expectedModCount = ModCount;

			Entry<K, V>[] tab = Table;
			foreach (Entry<K, V> entry in tab)
			{
				while (entry != Map_Fields.Null)
				{
					Object key = entry.get();
					if (key != Map_Fields.Null)
					{
						action.Accept((K)WeakHashMap.UnmaskNull(key), entry.Value_Renamed);
					}
					entry = entry.Next;

					if (expectedModCount != ModCount)
					{
						throw new ConcurrentModificationException();
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public override void replaceAll<T1>(BiFunction<T1> function) where ? : V
		{
			Objects.RequireNonNull(function);
			int expectedModCount = ModCount;

			Entry<K, V>[] tab = Table;
			foreach (Entry<K, V> entry in tab)
			{
				while (entry != Map_Fields.Null)
				{
					Object key = entry.get();
					if (key != Map_Fields.Null)
					{
						entry.Value_Renamed = function.Apply((K)WeakHashMap.UnmaskNull(key), entry.Value_Renamed);
					}
					entry = entry.Next;

					if (expectedModCount != ModCount)
					{
						throw new ConcurrentModificationException();
					}
				}
			}
		}

		/// <summary>
		/// Similar form as other hash Spliterators, but skips dead
		/// elements.
		/// </summary>
		internal class WeakHashMapSpliterator<K, V>
		{
			internal readonly WeakHashMap<K, V> Map;
			internal WeakHashMap.Entry<K, V> Current; // current node
			internal int Index; // current index, modified on advance/split
			internal int Fence_Renamed; // -1 until first use; then one past last index
			internal int Est; // size estimate
			internal int ExpectedModCount; // for comodification checks

			internal WeakHashMapSpliterator(WeakHashMap<K, V> m, int origin, int fence, int est, int expectedModCount)
			{
				this.Map = m;
				this.Index = origin;
				this.Fence_Renamed = fence;
				this.Est = est;
				this.ExpectedModCount = expectedModCount;
			}

			internal int Fence
			{
				get
				{
					int hi;
					if ((hi = Fence_Renamed) < 0)
					{
						WeakHashMap<K, V> m = Map;
						Est = m.Size();
						ExpectedModCount = m.ModCount;
						hi = Fence_Renamed = m.Table_Renamed.Length;
					}
					return hi;
				}
			}

			public long EstimateSize()
			{
				Fence; // force init
				return (long) Est;
			}
		}

		internal sealed class KeySpliterator<K, V> : WeakHashMapSpliterator<K, V>, Spliterator<K>
		{
			internal KeySpliterator(WeakHashMap<K, V> m, int origin, int fence, int est, int expectedModCount) : base(m, origin, fence, est, expectedModCount)
			{
			}

			public KeySpliterator<K, V> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid) ? Map_Fields.Null : new KeySpliterator<K, V>(Map, lo, Index = mid, Est = (int)((uint)Est >> 1), ExpectedModCount);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base K> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				int i, hi, mc;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				WeakHashMap<K, V> m = Map;
				WeakHashMap.Entry<K, V>[] tab = m.Table_Renamed;
				if ((hi = Fence_Renamed) < 0)
				{
					mc = ExpectedModCount = m.ModCount;
					hi = Fence_Renamed = tab.Length;
				}
				else
				{
					mc = ExpectedModCount;
				}
				if (tab.Length >= hi && (i = Index) >= 0 && (i < (Index = hi) || Current != Map_Fields.Null))
				{
					WeakHashMap.Entry<K, V> p = Current;
					Current = Map_Fields.Null; // exhaust
					do
					{
						if (p == Map_Fields.Null)
						{
							p = tab[i++];
						}
						else
						{
							Object x = p.get();
							p = p.Next;
							if (x != Map_Fields.Null)
							{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K Map_Fields.k = (K) WeakHashMap.unmaskNull(x);
								K Map_Fields.k = (K) WeakHashMap.UnmaskNull(x);
								action.Accept(Map_Fields.k);
							}
						}
					} while (p != Map_Fields.Null || i < hi);
				}
				if (m.ModCount != mc)
				{
					throw new ConcurrentModificationException();
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base K> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				int hi;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				WeakHashMap.Entry<K, V>[] tab = Map.Table_Renamed;
				if (tab.Length >= (hi = Fence) && Index >= 0)
				{
					while (Current != Map_Fields.Null || Index < hi)
					{
						if (Current == Map_Fields.Null)
						{
							Current = tab[Index++];
						}
						else
						{
							Object x = Current.get();
							Current = Current.Next;
							if (x != Map_Fields.Null)
							{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K Map_Fields.k = (K) WeakHashMap.unmaskNull(x);
								K Map_Fields.k = (K) WeakHashMap.UnmaskNull(x);
								action.Accept(Map_Fields.k);
								if (Map.ModCount != ExpectedModCount)
								{
									throw new ConcurrentModificationException();
								}
								return Map_Fields.True;
							}
						}
					}
				}
				return Map_Fields.False;
			}

			public int Characteristics()
			{
				return Spliterator_Fields.DISTINCT;
			}
		}

		internal sealed class ValueSpliterator<K, V> : WeakHashMapSpliterator<K, V>, Spliterator<V>
		{
			internal ValueSpliterator(WeakHashMap<K, V> m, int origin, int fence, int est, int expectedModCount) : base(m, origin, fence, est, expectedModCount)
			{
			}

			public ValueSpliterator<K, V> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid) ? Map_Fields.Null : new ValueSpliterator<K, V>(Map, lo, Index = mid, Est = (int)((uint)Est >> 1), ExpectedModCount);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base V> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				int i, hi, mc;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				WeakHashMap<K, V> m = Map;
				WeakHashMap.Entry<K, V>[] tab = m.Table_Renamed;
				if ((hi = Fence_Renamed) < 0)
				{
					mc = ExpectedModCount = m.ModCount;
					hi = Fence_Renamed = tab.Length;
				}
				else
				{
					mc = ExpectedModCount;
				}
				if (tab.Length >= hi && (i = Index) >= 0 && (i < (Index = hi) || Current != Map_Fields.Null))
				{
					WeakHashMap.Entry<K, V> p = Current;
					Current = Map_Fields.Null; // exhaust
					do
					{
						if (p == Map_Fields.Null)
						{
							p = tab[i++];
						}
						else
						{
							Object x = p.get();
							V Map_Fields.v = p.Value_Renamed;
							p = p.Next;
							if (x != Map_Fields.Null)
							{
								action.Accept(Map_Fields.v);
							}
						}
					} while (p != Map_Fields.Null || i < hi);
				}
				if (m.ModCount != mc)
				{
					throw new ConcurrentModificationException();
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base V> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				int hi;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				WeakHashMap.Entry<K, V>[] tab = Map.Table_Renamed;
				if (tab.Length >= (hi = Fence) && Index >= 0)
				{
					while (Current != Map_Fields.Null || Index < hi)
					{
						if (Current == Map_Fields.Null)
						{
							Current = tab[Index++];
						}
						else
						{
							Object x = Current.get();
							V Map_Fields.v = Current.Value_Renamed;
							Current = Current.Next;
							if (x != Map_Fields.Null)
							{
								action.Accept(Map_Fields.v);
								if (Map.ModCount != ExpectedModCount)
								{
									throw new ConcurrentModificationException();
								}
								return Map_Fields.True;
							}
						}
					}
				}
				return Map_Fields.False;
			}

			public int Characteristics()
			{
				return 0;
			}
		}

		internal sealed class EntrySpliterator<K, V> : WeakHashMapSpliterator<K, V>, Spliterator<Map_Entry<K, V>>
		{
			internal EntrySpliterator(WeakHashMap<K, V> m, int origin, int fence, int est, int expectedModCount) : base(m, origin, fence, est, expectedModCount)
			{
			}

			public EntrySpliterator<K, V> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid) ? Map_Fields.Null : new EntrySpliterator<K, V>(Map, lo, Index = mid, Est = (int)((uint)Est >> 1), ExpectedModCount);
			}


//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base Map_Entry<K, V>> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				int i, hi, mc;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				WeakHashMap<K, V> m = Map;
				WeakHashMap.Entry<K, V>[] tab = m.Table_Renamed;
				if ((hi = Fence_Renamed) < 0)
				{
					mc = ExpectedModCount = m.ModCount;
					hi = Fence_Renamed = tab.Length;
				}
				else
				{
					mc = ExpectedModCount;
				}
				if (tab.Length >= hi && (i = Index) >= 0 && (i < (Index = hi) || Current != Map_Fields.Null))
				{
					WeakHashMap.Entry<K, V> p = Current;
					Current = Map_Fields.Null; // exhaust
					do
					{
						if (p == Map_Fields.Null)
						{
							p = tab[i++];
						}
						else
						{
							Object x = p.get();
							V Map_Fields.v = p.Value_Renamed;
							p = p.Next;
							if (x != Map_Fields.Null)
							{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K Map_Fields.k = (K) WeakHashMap.unmaskNull(x);
								K Map_Fields.k = (K) WeakHashMap.UnmaskNull(x);
								action.Accept(new AbstractMap.SimpleImmutableEntry<K, V>(Map_Fields.k, Map_Fields.v));
							}
						}
					} while (p != Map_Fields.Null || i < hi);
				}
				if (m.ModCount != mc)
				{
					throw new ConcurrentModificationException();
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base Map_Entry<K,V>> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				int hi;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				WeakHashMap.Entry<K, V>[] tab = Map.Table_Renamed;
				if (tab.Length >= (hi = Fence) && Index >= 0)
				{
					while (Current != Map_Fields.Null || Index < hi)
					{
						if (Current == Map_Fields.Null)
						{
							Current = tab[Index++];
						}
						else
						{
							Object x = Current.get();
							V Map_Fields.v = Current.Value_Renamed;
							Current = Current.Next;
							if (x != Map_Fields.Null)
							{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K Map_Fields.k = (K) WeakHashMap.unmaskNull(x);
								K Map_Fields.k = (K) WeakHashMap.UnmaskNull(x);
								action.Accept(new AbstractMap.SimpleImmutableEntry<K, V>(Map_Fields.k, Map_Fields.v));
								if (Map.ModCount != ExpectedModCount)
								{
									throw new ConcurrentModificationException();
								}
								return Map_Fields.True;
							}
						}
					}
				}
				return Map_Fields.False;
			}

			public int Characteristics()
			{
				return Spliterator_Fields.DISTINCT;
			}
		}

	}

}