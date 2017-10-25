using System;

/*
 * Copyright (c) 2000, 2014, Oracle and/or its affiliates. All rights reserved.
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
	/// This class implements the <tt>Map</tt> interface with a hash table, using
	/// reference-equality in place of object-equality when comparing keys (and
	/// values).  In other words, in an <tt>IdentityHashMap</tt>, two keys
	/// <tt>k1</tt> and <tt>k2</tt> are considered equal if and only if
	/// <tt>(k1==k2)</tt>.  (In normal <tt>Map</tt> implementations (like
	/// <tt>HashMap</tt>) two keys <tt>k1</tt> and <tt>k2</tt> are considered equal
	/// if and only if <tt>(k1==null ? k2==null : k1.equals(k2))</tt>.)
	/// 
	/// <para><b>This class is <i>not</i> a general-purpose <tt>Map</tt>
	/// implementation!  While this class implements the <tt>Map</tt> interface, it
	/// intentionally violates <tt>Map's</tt> general contract, which mandates the
	/// use of the <tt>equals</tt> method when comparing objects.  This class is
	/// designed for use only in the rare cases wherein reference-equality
	/// semantics are required.</b>
	/// 
	/// </para>
	/// <para>A typical use of this class is <i>topology-preserving object graph
	/// transformations</i>, such as serialization or deep-copying.  To perform such
	/// a transformation, a program must maintain a "node table" that keeps track
	/// of all the object references that have already been processed.  The node
	/// table must not equate distinct objects even if they happen to be equal.
	/// Another typical use of this class is to maintain <i>proxy objects</i>.  For
	/// example, a debugging facility might wish to maintain a proxy object for
	/// each object in the program being debugged.
	/// 
	/// </para>
	/// <para>This class provides all of the optional map operations, and permits
	/// <tt>null</tt> values and the <tt>null</tt> key.  This class makes no
	/// guarantees as to the order of the map; in particular, it does not guarantee
	/// that the order will remain constant over time.
	/// 
	/// </para>
	/// <para>This class provides constant-time performance for the basic
	/// operations (<tt>get</tt> and <tt>put</tt>), assuming the system
	/// identity hash function (<seealso cref="System#identityHashCode(Object)"/>)
	/// disperses elements properly among the buckets.
	/// 
	/// </para>
	/// <para>This class has one tuning parameter (which affects performance but not
	/// semantics): <i>expected maximum size</i>.  This parameter is the maximum
	/// number of key-value mappings that the map is expected to hold.  Internally,
	/// this parameter is used to determine the number of buckets initially
	/// comprising the hash table.  The precise relationship between the expected
	/// maximum size and the number of buckets is unspecified.
	/// 
	/// </para>
	/// <para>If the size of the map (the number of key-value mappings) sufficiently
	/// exceeds the expected maximum size, the number of buckets is increased.
	/// Increasing the number of buckets ("rehashing") may be fairly expensive, so
	/// it pays to create identity hash maps with a sufficiently large expected
	/// maximum size.  On the other hand, iteration over collection views requires
	/// time proportional to the number of buckets in the hash table, so it
	/// pays not to set the expected maximum size too high if you are especially
	/// concerned with iteration performance or memory usage.
	/// 
	/// </para>
	/// <para><strong>Note that this implementation is not synchronized.</strong>
	/// If multiple threads access an identity hash map concurrently, and at
	/// least one of the threads modifies the map structurally, it <i>must</i>
	/// be synchronized externally.  (A structural modification is any operation
	/// that adds or deletes one or more mappings; merely changing the value
	/// associated with a key that an instance already contains is not a
	/// structural modification.)  This is typically accomplished by
	/// synchronizing on some object that naturally encapsulates the map.
	/// 
	/// If no such object exists, the map should be "wrapped" using the
	/// <seealso cref="Collections#synchronizedMap Collections.synchronizedMap"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access to the map:<pre>
	///   Map m = Collections.synchronizedMap(new IdentityHashMap(...));</pre>
	/// 
	/// </para>
	/// <para>The iterators returned by the <tt>iterator</tt> method of the
	/// collections returned by all of this class's "collection view
	/// methods" are <i>fail-fast</i>: if the map is structurally modified
	/// at any time after the iterator is created, in any way except
	/// through the iterator's own <tt>remove</tt> method, the iterator
	/// will throw a <seealso cref="ConcurrentModificationException"/>.  Thus, in the
	/// face of concurrent modification, the iterator fails quickly and
	/// cleanly, rather than risking arbitrary, non-deterministic behavior
	/// at an undetermined time in the future.
	/// 
	/// </para>
	/// <para>Note that the fail-fast behavior of an iterator cannot be guaranteed
	/// as it is, generally speaking, impossible to make any hard guarantees in the
	/// presence of unsynchronized concurrent modification.  Fail-fast iterators
	/// throw <tt>ConcurrentModificationException</tt> on a best-effort basis.
	/// Therefore, it would be wrong to write a program that depended on this
	/// exception for its correctness: <i>fail-fast iterators should be used only
	/// to detect bugs.</i>
	/// 
	/// </para>
	/// <para>Implementation note: This is a simple <i>linear-probe</i> hash table,
	/// as described for example in texts by Sedgewick and Knuth.  The array
	/// alternates holding keys and values.  (This has better locality for large
	/// tables than does using separate arrays.)  For many JRE implementations
	/// and operation mixes, this class will yield better performance than
	/// <seealso cref="HashMap"/> (which uses <i>chaining</i> rather than linear-probing).
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=     System#identityHashCode(Object) </seealso>
	/// <seealso cref=     Object#hashCode() </seealso>
	/// <seealso cref=     Collection </seealso>
	/// <seealso cref=     Map </seealso>
	/// <seealso cref=     HashMap </seealso>
	/// <seealso cref=     TreeMap
	/// @author  Doug Lea and Josh Bloch
	/// @since   1.4 </seealso>

	[Serializable]
	public class IdentityHashMap<K, V> : AbstractMap<K, V>, Map<K, V>, Cloneable
	{
		/// <summary>
		/// The initial capacity used by the no-args constructor.
		/// MUST be a power of two.  The value 32 corresponds to the
		/// (specified) expected maximum size of 21, given a load factor
		/// of 2/3.
		/// </summary>
		private const int DEFAULT_CAPACITY = 32;

		/// <summary>
		/// The minimum capacity, used if a lower value is implicitly specified
		/// by either of the constructors with arguments.  The value 4 corresponds
		/// to an expected maximum size of 2, given a load factor of 2/3.
		/// MUST be a power of two.
		/// </summary>
		private const int MINIMUM_CAPACITY = 4;

		/// <summary>
		/// The maximum capacity, used if a higher value is implicitly specified
		/// by either of the constructors with arguments.
		/// MUST be a power of two <= 1<<29.
		/// 
		/// In fact, the map can hold no more than MAXIMUM_CAPACITY-1 items
		/// because it has to have at least one slot with the key == null
		/// in order to avoid infinite loops in get(), put(), remove()
		/// </summary>
		private static readonly int MAXIMUM_CAPACITY = 1 << 29;

		/// <summary>
		/// The table, resized as necessary. Length MUST always be a power of two.
		/// </summary>
		[NonSerialized]
		internal Object[] Table; // non-private to simplify nested class access

		/// <summary>
		/// The number of key-value mappings contained in this identity hash map.
		/// 
		/// @serial
		/// </summary>
		internal int Size_Renamed;

		/// <summary>
		/// The number of modifications, to support fast-fail iterators
		/// </summary>
		[NonSerialized]
		internal int ModCount;

		/// <summary>
		/// Value representing null keys inside tables.
		/// </summary>
		internal static readonly Object NULL_KEY = new Object();

		/// <summary>
		/// Use NULL_KEY for key if it is null.
		/// </summary>
		private static Object MaskNull(Object key)
		{
			return (key == Map_Fields.Null ? NULL_KEY : key);
		}

		/// <summary>
		/// Returns internal representation of null key back to caller as null.
		/// </summary>
		internal static Object UnmaskNull(Object key)
		{
			return (key == NULL_KEY ? Map_Fields.Null : key);
		}

		/// <summary>
		/// Constructs a new, empty identity hash map with a default expected
		/// maximum size (21).
		/// </summary>
		public IdentityHashMap()
		{
			Init(DEFAULT_CAPACITY);
		}

		/// <summary>
		/// Constructs a new, empty map with the specified expected maximum size.
		/// Putting more than the expected number of key-value mappings into
		/// the map may cause the internal data structure to grow, which may be
		/// somewhat time-consuming.
		/// </summary>
		/// <param name="expectedMaxSize"> the expected maximum size of the map </param>
		/// <exception cref="IllegalArgumentException"> if <tt>expectedMaxSize</tt> is negative </exception>
		public IdentityHashMap(int expectedMaxSize)
		{
			if (expectedMaxSize < 0)
			{
				throw new IllegalArgumentException("expectedMaxSize is negative: " + expectedMaxSize);
			}
			Init(Capacity(expectedMaxSize));
		}

		/// <summary>
		/// Returns the appropriate capacity for the given expected maximum size.
		/// Returns the smallest power of two between MINIMUM_CAPACITY and
		/// MAXIMUM_CAPACITY, inclusive, that is greater than (3 *
		/// expectedMaxSize)/2, if such a number exists.  Otherwise returns
		/// MAXIMUM_CAPACITY.
		/// </summary>
		private static int Capacity(int expectedMaxSize)
		{
			// assert expectedMaxSize >= 0;
			return (expectedMaxSize > MAXIMUM_CAPACITY / 3) ? MAXIMUM_CAPACITY : (expectedMaxSize <= 2 * MINIMUM_CAPACITY / 3) ? MINIMUM_CAPACITY : Integer.HighestOneBit(expectedMaxSize + (expectedMaxSize << 1));
		}

		/// <summary>
		/// Initializes object to be an empty map with the specified initial
		/// capacity, which is assumed to be a power of two between
		/// MINIMUM_CAPACITY and MAXIMUM_CAPACITY inclusive.
		/// </summary>
		private void Init(int initCapacity)
		{
			// assert (initCapacity & -initCapacity) == initCapacity; // power of 2
			// assert initCapacity >= MINIMUM_CAPACITY;
			// assert initCapacity <= MAXIMUM_CAPACITY;

			Table = new Object[2 * initCapacity];
		}

		/// <summary>
		/// Constructs a new identity hash map containing the keys-value mappings
		/// in the specified map.
		/// </summary>
		/// <param name="m"> the map whose mappings are to be placed into this map </param>
		/// <exception cref="NullPointerException"> if the specified map is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public IdentityHashMap(Map<? extends K, ? extends V> m)
		public IdentityHashMap<T1>(Map<T1> m) where T1 : K where ? : V : this((int)((1 + m.Size()) * 1.1))
		{
			// Allow for a bit of growth
			PutAll(m);
		}

		/// <summary>
		/// Returns the number of key-value mappings in this identity hash map.
		/// </summary>
		/// <returns> the number of key-value mappings in this map </returns>
		public virtual int Size()
		{
			return Size_Renamed;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this identity hash map contains no key-value
		/// mappings.
		/// </summary>
		/// <returns> <tt>true</tt> if this identity hash map contains no key-value
		///         mappings </returns>
		public virtual bool Empty
		{
			get
			{
				return Size_Renamed == 0;
			}
		}

		/// <summary>
		/// Returns index for Object x.
		/// </summary>
		private static int Hash(Object x, int length)
		{
			int h = System.identityHashCode(x);
			// Multiply by -127, and left-shift to use least bit as part of hash
			return ((h << 1) - (h << 8)) & (length - 1);
		}

		/// <summary>
		/// Circularly traverses table of size len.
		/// </summary>
		private static int NextKeyIndex(int i, int len)
		{
			return (i + 2 < len ? i + 2 : 0);
		}

		/// <summary>
		/// Returns the value to which the specified key is mapped,
		/// or {@code null} if this map contains no mapping for the key.
		/// 
		/// <para>More formally, if this map contains a mapping from a key
		/// {@code k} to a value {@code v} such that {@code (key == k)},
		/// then this method returns {@code v}; otherwise it returns
		/// {@code null}.  (There can be at most one such mapping.)
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
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V get(Object key)
		public virtual V Get(Object key)
		{
			Object Map_Fields.k = MaskNull(key);
			Object[] tab = Table;
			int len = tab.Length;
			int i = Hash(Map_Fields.k, len);
			while (Map_Fields.True)
			{
				Object item = tab[i];
				if (item == Map_Fields.k)
				{
					return (V) tab[i + 1];
				}
				if (item == Map_Fields.Null)
				{
					return Map_Fields.Null;
				}
				i = NextKeyIndex(i, len);
			}
		}

		/// <summary>
		/// Tests whether the specified object reference is a key in this identity
		/// hash map.
		/// </summary>
		/// <param name="key">   possible key </param>
		/// <returns>  <code>true</code> if the specified object reference is a key
		///          in this map </returns>
		/// <seealso cref=     #containsValue(Object) </seealso>
		public virtual bool ContainsKey(Object key)
		{
			Object Map_Fields.k = MaskNull(key);
			Object[] tab = Table;
			int len = tab.Length;
			int i = Hash(Map_Fields.k, len);
			while (Map_Fields.True)
			{
				Object item = tab[i];
				if (item == Map_Fields.k)
				{
					return Map_Fields.True;
				}
				if (item == Map_Fields.Null)
				{
					return Map_Fields.False;
				}
				i = NextKeyIndex(i, len);
			}
		}

		/// <summary>
		/// Tests whether the specified object reference is a value in this identity
		/// hash map.
		/// </summary>
		/// <param name="value"> value whose presence in this map is to be tested </param>
		/// <returns> <tt>true</tt> if this map maps one or more keys to the
		///         specified object reference </returns>
		/// <seealso cref=     #containsKey(Object) </seealso>
		public virtual bool ContainsValue(Object value)
		{
			Object[] tab = Table;
			for (int i = 1; i < tab.Length; i += 2)
			{
				if (tab[i] == value && tab[i - 1] != Map_Fields.Null)
				{
					return Map_Fields.True;
				}
			}

			return Map_Fields.False;
		}

		/// <summary>
		/// Tests if the specified key-value mapping is in the map.
		/// </summary>
		/// <param name="key">   possible key </param>
		/// <param name="value"> possible value </param>
		/// <returns>  <code>true</code> if and only if the specified key-value
		///          mapping is in the map </returns>
		private bool ContainsMapping(Object key, Object value)
		{
			Object Map_Fields.k = MaskNull(key);
			Object[] tab = Table;
			int len = tab.Length;
			int i = Hash(Map_Fields.k, len);
			while (Map_Fields.True)
			{
				Object item = tab[i];
				if (item == Map_Fields.k)
				{
					return tab[i + 1] == value;
				}
				if (item == Map_Fields.Null)
				{
					return Map_Fields.False;
				}
				i = NextKeyIndex(i, len);
			}
		}

		/// <summary>
		/// Associates the specified value with the specified key in this identity
		/// hash map.  If the map previously contained a mapping for the key, the
		/// old value is replaced.
		/// </summary>
		/// <param name="key"> the key with which the specified value is to be associated </param>
		/// <param name="value"> the value to be associated with the specified key </param>
		/// <returns> the previous value associated with <tt>key</tt>, or
		///         <tt>null</tt> if there was no mapping for <tt>key</tt>.
		///         (A <tt>null</tt> return can also indicate that the map
		///         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
		/// <seealso cref=     Object#equals(Object) </seealso>
		/// <seealso cref=     #get(Object) </seealso>
		/// <seealso cref=     #containsKey(Object) </seealso>
		public virtual V Put(K key, V value)
		{
			final Object Map_Fields.k = MaskNull(key);

			for (;;)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] tab = table;
				Object[] tab = Table;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = tab.length;
				int len = tab.Length;
				int i = Hash(Map_Fields.k, len);

				for (Object item; (item = tab[i]) != Map_Fields.Null; i = NextKeyIndex(i, len))
				{
					if (item == Map_Fields.k)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V Map_Fields.oldValue = (V) tab[i + 1];
						V Map_Fields.OldValue = (V) tab[i + 1];
						tab[i + 1] = value;
						return Map_Fields.OldValue;
					}
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int s = size + 1;
				int s = Size_Renamed + 1;
				// Use optimized form of 3 * s.
				// Next capacity is len, 2 * current capacity.
				if (s + (s << 1) > len && Resize(len))
				{
					goto retryAfterResizeContinue;
				}

				ModCount++;
				tab[i] = Map_Fields.k;
				tab[i + 1] = value;
				Size_Renamed = s;
				return Map_Fields.Null;
				retryAfterResizeContinue:;
			}
			retryAfterResizeBreak:;
		}

		/// <summary>
		/// Resizes the table if necessary to hold given capacity.
		/// </summary>
		/// <param name="newCapacity"> the new capacity, must be a power of two. </param>
		/// <returns> whether a resize did in fact take place </returns>
		private bool Resize(int newCapacity)
		{
			// assert (newCapacity & -newCapacity) == newCapacity; // power of 2
			int newLength = newCapacity * 2;

			Object[] oldTable = Table;
			int oldLength = oldTable.Length;
			if (oldLength == 2 * MAXIMUM_CAPACITY) // can't expand any further
			{
				if (Size_Renamed == MAXIMUM_CAPACITY - 1)
				{
					throw new IllegalStateException("Capacity exhausted.");
				}
				return Map_Fields.False;
			}
			if (oldLength >= newLength)
			{
				return Map_Fields.False;
			}

			Object[] newTable = new Object[newLength];

			for (int j = 0; j < oldLength; j += 2)
			{
				Object key = oldTable[j];
				if (key != Map_Fields.Null)
				{
					Object value = oldTable[j + 1];
					oldTable[j] = Map_Fields.Null;
					oldTable[j + 1] = Map_Fields.Null;
					int i = Hash(key, newLength);
					while (newTable[i] != Map_Fields.Null)
					{
						i = NextKeyIndex(i, newLength);
					}
					newTable[i] = key;
					newTable[i + 1] = value;
				}
			}
			Table = newTable;
			return Map_Fields.True;
		}

		/// <summary>
		/// Copies all of the mappings from the specified map to this map.
		/// These mappings will replace any mappings that this map had for
		/// any of the keys currently in the specified map.
		/// </summary>
		/// <param name="m"> mappings to be stored in this map </param>
		/// <exception cref="NullPointerException"> if the specified map is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(Map<? extends K, ? extends V> m)
		public virtual void putAll<T1>(Map<T1> m) where T1 : K where ? : V
		{
			int n = m.Size();
			if (n == 0)
			{
				return;
			}
			if (n > Size_Renamed)
			{
				Resize(Capacity(n)); // conservatively pre-expand
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Map_Entry<? extends K, ? extends V> e : m.entrySet())
			foreach (Map_Entry<?, ?> e in m.EntrySet())
			{
				Put(e.Key, e.Value);
			}
		}

		/// <summary>
		/// Removes the mapping for this key from this map if present.
		/// </summary>
		/// <param name="key"> key whose mapping is to be removed from the map </param>
		/// <returns> the previous value associated with <tt>key</tt>, or
		///         <tt>null</tt> if there was no mapping for <tt>key</tt>.
		///         (A <tt>null</tt> return can also indicate that the map
		///         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
		public virtual V Remove(Object key)
		{
			Object Map_Fields.k = MaskNull(key);
			Object[] tab = Table;
			int len = tab.Length;
			int i = Hash(Map_Fields.k, len);

			while (Map_Fields.True)
			{
				Object item = tab[i];
				if (item == Map_Fields.k)
				{
					ModCount++;
					Size_Renamed--;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V Map_Fields.oldValue = (V) tab[i + 1];
					V Map_Fields.OldValue = (V) tab[i + 1];
					tab[i + 1] = Map_Fields.Null;
					tab[i] = Map_Fields.Null;
					CloseDeletion(i);
					return Map_Fields.OldValue;
				}
				if (item == Map_Fields.Null)
				{
					return Map_Fields.Null;
				}
				i = NextKeyIndex(i, len);
			}
		}

		/// <summary>
		/// Removes the specified key-value mapping from the map if it is present.
		/// </summary>
		/// <param name="key">   possible key </param>
		/// <param name="value"> possible value </param>
		/// <returns>  <code>true</code> if and only if the specified key-value
		///          mapping was in the map </returns>
		private bool RemoveMapping(Object key, Object value)
		{
			Object Map_Fields.k = MaskNull(key);
			Object[] tab = Table;
			int len = tab.Length;
			int i = Hash(Map_Fields.k, len);

			while (Map_Fields.True)
			{
				Object item = tab[i];
				if (item == Map_Fields.k)
				{
					if (tab[i + 1] != value)
					{
						return Map_Fields.False;
					}
					ModCount++;
					Size_Renamed--;
					tab[i] = Map_Fields.Null;
					tab[i + 1] = Map_Fields.Null;
					CloseDeletion(i);
					return Map_Fields.True;
				}
				if (item == Map_Fields.Null)
				{
					return Map_Fields.False;
				}
				i = NextKeyIndex(i, len);
			}
		}

		/// <summary>
		/// Rehash all possibly-colliding entries following a
		/// deletion. This preserves the linear-probe
		/// collision properties required by get, put, etc.
		/// </summary>
		/// <param name="d"> the index of a newly empty deleted slot </param>
		private void CloseDeletion(int d)
		{
			// Adapted from Knuth Section 6.4 Algorithm R
			Object[] tab = Table;
			int len = tab.Length;

			// Look for items to swap into newly vacated slot
			// starting at index immediately following deletion,
			// and continuing until a null slot is seen, indicating
			// the end of a run of possibly-colliding keys.
			Object item;
			for (int i = NextKeyIndex(d, len); (item = tab[i]) != Map_Fields.Null; i = NextKeyIndex(i, len))
			{
				// The following test triggers if the item at slot i (which
				// hashes to be at slot r) should take the spot vacated by d.
				// If so, we swap it in, and then continue with d now at the
				// newly vacated i.  This process will terminate when we hit
				// the null slot at the end of this run.
				// The test is messy because we are using a circular table.
				int r = Hash(item, len);
				if ((i < r && (r <= d || d <= i)) || (r <= d && d <= i))
				{
					tab[d] = item;
					tab[d + 1] = tab[i + 1];
					tab[i] = Map_Fields.Null;
					tab[i + 1] = Map_Fields.Null;
					d = i;
				}
			}
		}

		/// <summary>
		/// Removes all of the mappings from this map.
		/// The map will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			ModCount++;
			Object[] tab = Table;
			for (int i = 0; i < tab.Length; i++)
			{
				tab[i] = Map_Fields.Null;
			}
			Size_Renamed = 0;
		}

		/// <summary>
		/// Compares the specified object with this map for equality.  Returns
		/// <tt>true</tt> if the given object is also a map and the two maps
		/// represent identical object-reference mappings.  More formally, this
		/// map is equal to another map <tt>m</tt> if and only if
		/// <tt>this.entrySet().equals(m.entrySet())</tt>.
		/// 
		/// <para><b>Owing to the reference-equality-based semantics of this map it is
		/// possible that the symmetry and transitivity requirements of the
		/// <tt>Object.equals</tt> contract may be violated if this map is compared
		/// to a normal map.  However, the <tt>Object.equals</tt> contract is
		/// guaranteed to hold among <tt>IdentityHashMap</tt> instances.</b>
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> object to be compared for equality with this map </param>
		/// <returns> <tt>true</tt> if the specified object is equal to this map </returns>
		/// <seealso cref= Object#equals(Object) </seealso>
		public override bool Equals(Object o)
		{
			if (o == this)
			{
				return Map_Fields.True;
			}
			else if (o is IdentityHashMap)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: IdentityHashMap<?,?> m = (IdentityHashMap<?,?>) o;
				IdentityHashMap<?, ?> m = (IdentityHashMap<?, ?>) o;
				if (m.Size() != Size_Renamed)
				{
					return Map_Fields.False;
				}

				Object[] tab = m.Table;
				for (int i = 0; i < tab.Length; i += 2)
				{
					Object Map_Fields.k = tab[i];
					if (Map_Fields.k != Map_Fields.Null && !ContainsMapping(Map_Fields.k, tab[i + 1]))
					{
						return Map_Fields.False;
					}
				}
				return Map_Fields.True;
			}
			else if (o is Map)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map<?,?> m = (Map<?,?>)o;
				Map<?, ?> m = (Map<?, ?>)o;
				return EntrySet().Equals(m.EntrySet());
			}
			else
			{
				return Map_Fields.False; // o is not a Map
			}
		}

		/// <summary>
		/// Returns the hash code value for this map.  The hash code of a map is
		/// defined to be the sum of the hash codes of each entry in the map's
		/// <tt>entrySet()</tt> view.  This ensures that <tt>m1.equals(m2)</tt>
		/// implies that <tt>m1.hashCode()==m2.hashCode()</tt> for any two
		/// <tt>IdentityHashMap</tt> instances <tt>m1</tt> and <tt>m2</tt>, as
		/// required by the general contract of <seealso cref="Object#hashCode"/>.
		/// 
		/// <para><b>Owing to the reference-equality-based semantics of the
		/// <tt>Map.Entry</tt> instances in the set returned by this map's
		/// <tt>entrySet</tt> method, it is possible that the contractual
		/// requirement of <tt>Object.hashCode</tt> mentioned in the previous
		/// paragraph will be violated if one of the two objects being compared is
		/// an <tt>IdentityHashMap</tt> instance and the other is a normal map.</b>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the hash code value for this map </returns>
		/// <seealso cref= Object#equals(Object) </seealso>
		/// <seealso cref= #equals(Object) </seealso>
		public override int HashCode()
		{
			int result = 0;
			Object[] tab = Table;
			for (int i = 0; i < tab.Length; i += 2)
			{
				Object key = tab[i];
				if (key != Map_Fields.Null)
				{
					Object Map_Fields.k = UnmaskNull(key);
					result += System.identityHashCode(Map_Fields.k) ^ System.identityHashCode(tab[i + 1]);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns a shallow copy of this identity hash map: the keys and values
		/// themselves are not cloned.
		/// </summary>
		/// <returns> a shallow copy of this map </returns>
		public virtual Object Clone()
		{
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: IdentityHashMap<?,?> m = (IdentityHashMap<?,?>) base.clone();
				IdentityHashMap<?, ?> m = (IdentityHashMap<?, ?>) base.Clone();
				m.EntrySet_Renamed = Map_Fields.Null;
				m.Table = Table.clone();
				return m;
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		private abstract class IdentityHashMapIterator<T> : Iterator<T>
		{
			internal bool InstanceFieldsInitialized = Map_Fields.False;

			internal virtual void InitializeInstanceFields()
			{
				Index = (outerInstance.Size_Renamed != 0 ? 0 : outerInstance.Table.Length);
				ExpectedModCount = outerInstance.ModCount;
				TraversalTable = outerInstance.Table;
			}

			private readonly IdentityHashMap<K, V> OuterInstance;

			public IdentityHashMapIterator(IdentityHashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = Map_Fields.True;
				}
			}

			internal int Index; // current slot.
			internal int ExpectedModCount; // to support fast-fail
			internal int LastReturnedIndex = -1; // to allow remove()
			internal bool IndexValid; // To avoid unnecessary next computation
			internal Object[] TraversalTable; // reference to main table or copy

			public virtual bool HasNext()
			{
				Object[] tab = TraversalTable;
				for (int i = Index; i < tab.Length; i += 2)
				{
					Object key = tab[i];
					if (key != Map_Fields.Null)
					{
						Index = i;
						return IndexValid = Map_Fields.True;
					}
				}
				Index = tab.Length;
				return Map_Fields.False;
			}

			protected internal virtual int NextIndex()
			{
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				if (!IndexValid && !HasNext())
				{
					throw new NoSuchElementException();
				}

				IndexValid = Map_Fields.False;
				LastReturnedIndex = Index;
				Index += 2;
				return LastReturnedIndex;
			}

			public virtual void Remove()
			{
				if (LastReturnedIndex == -1)
				{
					throw new IllegalStateException();
				}
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}

				ExpectedModCount = ++outerInstance.ModCount;
				int deletedSlot = LastReturnedIndex;
				LastReturnedIndex = -1;
				// back up index to revisit new contents after deletion
				Index = deletedSlot;
				IndexValid = Map_Fields.False;

				// Removal code proceeds as in closeDeletion except that
				// it must catch the rare case where an element already
				// seen is swapped into a vacant slot that will be later
				// traversed by this iterator. We cannot allow future
				// next() calls to return it again.  The likelihood of
				// this occurring under 2/3 load factor is very slim, but
				// when it does happen, we must make a copy of the rest of
				// the table to use for the rest of the traversal. Since
				// this can only happen when we are near the end of the table,
				// even in these rare cases, this is not very expensive in
				// time or space.

				Object[] tab = TraversalTable;
				int len = tab.Length;

				int d = deletedSlot;
				Object key = tab[d];
				tab[d] = Map_Fields.Null; // vacate the slot
				tab[d + 1] = Map_Fields.Null;

				// If traversing a copy, remove in real table.
				// We can skip gap-closure on copy.
				if (tab != OuterInstance.Table)
				{
					OuterInstance.Remove(key);
					ExpectedModCount = outerInstance.ModCount;
					return;
				}

				outerInstance.Size_Renamed--;

				Object item;
				for (int i = NextKeyIndex(d, len); (item = tab[i]) != Map_Fields.Null; i = NextKeyIndex(i, len))
				{
					int r = Hash(item, len);
					// See closeDeletion for explanation of this conditional
					if ((i < r && (r <= d || d <= i)) || (r <= d && d <= i))
					{

						// If we are about to swap an already-seen element
						// into a slot that may later be returned by next(),
						// then clone the rest of table for use in future
						// next() calls. It is OK that our copy will have
						// a gap in the "wrong" place, since it will never
						// be used for searching anyway.

						if (i < deletedSlot && d >= deletedSlot && TraversalTable == OuterInstance.Table)
						{
							int remaining = len - deletedSlot;
							Object[] newTable = new Object[remaining];
							System.Array.Copy(tab, deletedSlot, newTable, 0, remaining);
							TraversalTable = newTable;
							Index = 0;
						}

						tab[d] = item;
						tab[d + 1] = tab[i + 1];
						tab[i] = Map_Fields.Null;
						tab[i + 1] = Map_Fields.Null;
						d = i;
					}
				}
			}
		}

		private class KeyIterator : IdentityHashMapIterator<K>
		{
			private readonly IdentityHashMap<K, V> OuterInstance;

			public KeyIterator(IdentityHashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public K next()
			public virtual K Next()
			{
				return (K) UnmaskNull(TraversalTable[NextIndex()]);
			}
		}

		private class ValueIterator : IdentityHashMapIterator<V>
		{
			private readonly IdentityHashMap<K, V> OuterInstance;

			public ValueIterator(IdentityHashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V next()
			public virtual V Next()
			{
				return (V) TraversalTable[NextIndex() + 1];
			}
		}

		private class EntryIterator : IdentityHashMapIterator<Map_Entry<K, V>>
		{
			private readonly IdentityHashMap<K, V> OuterInstance;

			public EntryIterator(IdentityHashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			internal Entry LastReturnedEntry;

			public virtual Map_Entry<K, V> Next()
			{
				LastReturnedEntry = new Entry(this, NextIndex());
				return LastReturnedEntry;
			}

			public virtual void Remove()
			{
				LastReturnedIndex = ((Map_Fields.Null == LastReturnedEntry) ? - 1 : LastReturnedEntry.Index);
				base.Remove();
				LastReturnedEntry.Index = LastReturnedIndex;
				LastReturnedEntry = Map_Fields.Null;
			}

			private class Entry : Map_Entry<K, V>
			{
				private readonly IdentityHashMap.EntryIterator OuterInstance;

				internal int Index;

				internal Entry(IdentityHashMap.EntryIterator outerInstance, int index)
				{
					this.OuterInstance = outerInstance;
					this.Index = index;
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public K getKey()
				public virtual K Key
				{
					get
					{
						CheckIndexForEntryUse();
						return (K) UnmaskNull(outerInstance.TraversalTable[Index]);
					}
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V getValue()
				public virtual V Value
				{
					get
					{
						CheckIndexForEntryUse();
						return (V) outerInstance.TraversalTable[Index + 1];
					}
				}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public V setValue(V value)
				public virtual V SetValue(V value)
				{
					CheckIndexForEntryUse();
					V Map_Fields.OldValue = (V) outerInstance.TraversalTable[Index + 1];
					outerInstance.TraversalTable[Index + 1] = value;
					// if shadowing, force into main table
					if (outerInstance.TraversalTable != OuterInstance.OuterInstance.table)
					{
						outerInstance.outerInstance.Put((K) outerInstance.TraversalTable[Index], value);
					}
					return Map_Fields.OldValue;
				}

				public override bool Equals(Object o)
				{
					if (Index < 0)
					{
						return base.Equals(o);
					}

					if (!(o is Map_Entry))
					{
						return Map_Fields.False;
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>)o;
					Map_Entry<?, ?> e = (Map_Entry<?, ?>)o;
					return (e.Key == UnmaskNull(outerInstance.TraversalTable[Index]) && e.Value == outerInstance.TraversalTable[Index + 1]);
				}

				public override int HashCode()
				{
					if (outerInstance.LastReturnedIndex < 0)
					{
						return base.HashCode();
					}

					return (System.identityHashCode(UnmaskNull(outerInstance.TraversalTable[Index])) ^ System.identityHashCode(outerInstance.TraversalTable[Index + 1]));
				}

				public override String ToString()
				{
					if (Index < 0)
					{
						return base.ToString();
					}

					return (UnmaskNull(outerInstance.TraversalTable[Index]) + "=" + outerInstance.TraversalTable[Index + 1]);
				}

				internal virtual void CheckIndexForEntryUse()
				{
					if (Index < 0)
					{
						throw new IllegalStateException("Entry was removed");
					}
				}
			}
		}

		// Views

		/// <summary>
		/// This field is initialized to contain an instance of the entry set
		/// view the first time this view is requested.  The view is stateless,
		/// so there's no reason to create more than one.
		/// </summary>
		[NonSerialized]
		private Set<Map_Entry<K, V>> EntrySet_Renamed;

		/// <summary>
		/// Returns an identity-based set view of the keys contained in this map.
		/// The set is backed by the map, so changes to the map are reflected in
		/// the set, and vice-versa.  If the map is modified while an iteration
		/// over the set is in progress, the results of the iteration are
		/// undefined.  The set supports element removal, which removes the
		/// corresponding mapping from the map, via the <tt>Iterator.remove</tt>,
		/// <tt>Set.remove</tt>, <tt>removeAll</tt>, <tt>retainAll</tt>, and
		/// <tt>clear</tt> methods.  It does not support the <tt>add</tt> or
		/// <tt>addAll</tt> methods.
		/// 
		/// <para><b>While the object returned by this method implements the
		/// <tt>Set</tt> interface, it does <i>not</i> obey <tt>Set's</tt> general
		/// contract.  Like its backing map, the set returned by this method
		/// defines element equality as reference-equality rather than
		/// object-equality.  This affects the behavior of its <tt>contains</tt>,
		/// <tt>remove</tt>, <tt>containsAll</tt>, <tt>equals</tt>, and
		/// <tt>hashCode</tt> methods.</b>
		/// 
		/// </para>
		/// <para><b>The <tt>equals</tt> method of the returned set returns <tt>true</tt>
		/// only if the specified object is a set containing exactly the same
		/// object references as the returned set.  The symmetry and transitivity
		/// requirements of the <tt>Object.equals</tt> contract may be violated if
		/// the set returned by this method is compared to a normal set.  However,
		/// the <tt>Object.equals</tt> contract is guaranteed to hold among sets
		/// returned by this method.</b>
		/// 
		/// </para>
		/// <para>The <tt>hashCode</tt> method of the returned set returns the sum of
		/// the <i>identity hashcodes</i> of the elements in the set, rather than
		/// the sum of their hashcodes.  This is mandated by the change in the
		/// semantics of the <tt>equals</tt> method, in order to enforce the
		/// general contract of the <tt>Object.hashCode</tt> method among sets
		/// returned by this method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an identity-based set view of the keys contained in this map </returns>
		/// <seealso cref= Object#equals(Object) </seealso>
		/// <seealso cref= System#identityHashCode(Object) </seealso>
		public virtual Set<K> KeySet()
		{
			Set<K> ks = KeySet_Renamed;
			if (ks != Map_Fields.Null)
			{
				return ks;
			}
			else
			{
				return KeySet_Renamed = new KeySet(this);
			}
		}

		private class KeySet : AbstractSet<K>
		{
			private readonly IdentityHashMap<K, V> OuterInstance;

			public KeySet(IdentityHashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<K> Iterator()
			{
				return new KeyIterator(OuterInstance);
			}
			public virtual int Size()
			{
				return outerInstance.Size_Renamed;
			}
			public virtual bool Contains(Object o)
			{
				return outerInstance.ContainsKey(o);
			}
			public virtual bool Remove(Object o)
			{
				int oldSize = outerInstance.Size_Renamed;
				OuterInstance.Remove(o);
				return outerInstance.Size_Renamed != oldSize;
			}
			/*
			 * Must revert from AbstractSet's impl to AbstractCollection's, as
			 * the former contains an optimization that results in incorrect
			 * behavior when c is a smaller "normal" (non-identity-based) Set.
			 */
			public virtual bool removeAll<T1>(Collection<T1> c)
			{
				Objects.RequireNonNull(c);
				bool modified = Map_Fields.False;
				for (Iterator<K> i = Iterator(); i.HasNext();)
				{
					if (c.Contains(i.Next()))
					{
						i.remove();
						modified = Map_Fields.True;
					}
				}
				return modified;
			}
			public virtual void Clear()
			{
				OuterInstance.Clear();
			}
			public override int HashCode()
			{
				int result = 0;
				foreach (K key in this)
				{
					result += System.identityHashCode(key);
				}
				return result;
			}
			public virtual Object[] ToArray()
			{
				return ToArray(new Object[0]);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
			public virtual T[] toArray<T>(T[] a)
			{
				int expectedModCount = outerInstance.ModCount;
				int size = Size();
				if (a.Length < size)
				{
					a = (T[]) Array.newInstance(a.GetType().GetElementType(), size);
				}
				Object[] tab = outerInstance.Table;
				int ti = 0;
				for (int si = 0; si < tab.Length; si += 2)
				{
					Object key;
					if ((key = tab[si]) != Map_Fields.Null) // key present ?
					{
						// more elements than expected -> concurrent modification from other thread
						if (ti >= size)
						{
							throw new ConcurrentModificationException();
						}
						a[ti++] = (T) UnmaskNull(key); // unmask key
					}
				}
				// fewer elements than expected or concurrent modification from other thread detected
				if (ti < size || expectedModCount != outerInstance.ModCount)
				{
					throw new ConcurrentModificationException();
				}
				// final null marker as per spec
				if (ti < a.Length)
				{
					a[ti] = Map_Fields.Null;
				}
				return a;
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
		/// modified while an iteration over the collection is in progress,
		/// the results of the iteration are undefined.  The collection
		/// supports element removal, which removes the corresponding
		/// mapping from the map, via the <tt>Iterator.remove</tt>,
		/// <tt>Collection.remove</tt>, <tt>removeAll</tt>,
		/// <tt>retainAll</tt> and <tt>clear</tt> methods.  It does not
		/// support the <tt>add</tt> or <tt>addAll</tt> methods.
		/// 
		/// <para><b>While the object returned by this method implements the
		/// <tt>Collection</tt> interface, it does <i>not</i> obey
		/// <tt>Collection's</tt> general contract.  Like its backing map,
		/// the collection returned by this method defines element equality as
		/// reference-equality rather than object-equality.  This affects the
		/// behavior of its <tt>contains</tt>, <tt>remove</tt> and
		/// <tt>containsAll</tt> methods.</b>
		/// </para>
		/// </summary>
		public virtual Collection<V> Values()
		{
			Collection<V> vs = Values_Renamed;
			if (vs != Map_Fields.Null)
			{
				return vs;
			}
			else
			{
				return Values_Renamed = new Values(this);
			}
		}

		private class Values : AbstractCollection<V>
		{
			private readonly IdentityHashMap<K, V> OuterInstance;

			public Values(IdentityHashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<V> Iterator()
			{
				return new ValueIterator(OuterInstance);
			}
			public virtual int Size()
			{
				return outerInstance.Size_Renamed;
			}
			public virtual bool Contains(Object o)
			{
				return outerInstance.ContainsValue(o);
			}
			public virtual bool Remove(Object o)
			{
				for (Iterator<V> i = Iterator(); i.HasNext();)
				{
					if (i.Next() == o)
					{
						i.remove();
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}
			public virtual void Clear()
			{
				OuterInstance.Clear();
			}
			public virtual Object[] ToArray()
			{
				return ToArray(new Object[0]);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
			public virtual T[] toArray<T>(T[] a)
			{
				int expectedModCount = outerInstance.ModCount;
				int size = Size();
				if (a.Length < size)
				{
					a = (T[]) Array.newInstance(a.GetType().GetElementType(), size);
				}
				Object[] tab = outerInstance.Table;
				int ti = 0;
				for (int si = 0; si < tab.Length; si += 2)
				{
					if (tab[si] != Map_Fields.Null) // key present ?
					{
						// more elements than expected -> concurrent modification from other thread
						if (ti >= size)
						{
							throw new ConcurrentModificationException();
						}
						a[ti++] = (T) tab[si + 1]; // copy value
					}
				}
				// fewer elements than expected or concurrent modification from other thread detected
				if (ti < size || expectedModCount != outerInstance.ModCount)
				{
					throw new ConcurrentModificationException();
				}
				// final null marker as per spec
				if (ti < a.Length)
				{
					a[ti] = Map_Fields.Null;
				}
				return a;
			}

			public virtual Spliterator<V> Spliterator()
			{
				return new ValueSpliterator<>(OuterInstance, 0, -1, 0, 0);
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		/// Each element in the returned set is a reference-equality-based
		/// <tt>Map.Entry</tt>.  The set is backed by the map, so changes
		/// to the map are reflected in the set, and vice-versa.  If the
		/// map is modified while an iteration over the set is in progress,
		/// the results of the iteration are undefined.  The set supports
		/// element removal, which removes the corresponding mapping from
		/// the map, via the <tt>Iterator.remove</tt>, <tt>Set.remove</tt>,
		/// <tt>removeAll</tt>, <tt>retainAll</tt> and <tt>clear</tt>
		/// methods.  It does not support the <tt>add</tt> or
		/// <tt>addAll</tt> methods.
		/// 
		/// <para>Like the backing map, the <tt>Map.Entry</tt> objects in the set
		/// returned by this method define key and value equality as
		/// reference-equality rather than object-equality.  This affects the
		/// behavior of the <tt>equals</tt> and <tt>hashCode</tt> methods of these
		/// <tt>Map.Entry</tt> objects.  A reference-equality based <tt>Map.Entry
		/// e</tt> is equal to an object <tt>o</tt> if and only if <tt>o</tt> is a
		/// <tt>Map.Entry</tt> and <tt>e.getKey()==o.getKey() &amp;&amp;
		/// e.getValue()==o.getValue()</tt>.  To accommodate these equals
		/// semantics, the <tt>hashCode</tt> method returns
		/// <tt>System.identityHashCode(e.getKey()) ^
		/// System.identityHashCode(e.getValue())</tt>.
		/// 
		/// </para>
		/// <para><b>Owing to the reference-equality-based semantics of the
		/// <tt>Map.Entry</tt> instances in the set returned by this method,
		/// it is possible that the symmetry and transitivity requirements of
		/// the <seealso cref="Object#equals(Object)"/> contract may be violated if any of
		/// the entries in the set is compared to a normal map entry, or if
		/// the set returned by this method is compared to a set of normal map
		/// entries (such as would be returned by a call to this method on a normal
		/// map).  However, the <tt>Object.equals</tt> contract is guaranteed to
		/// hold among identity-based map entries, and among sets of such entries.
		/// </b>
		/// 
		/// </para>
		/// </summary>
		/// <returns> a set view of the identity-mappings contained in this map </returns>
		public virtual Set<Map_Entry<K, V>> EntrySet()
		{
			Set<Map_Entry<K, V>> es = EntrySet_Renamed;
			if (es != Map_Fields.Null)
			{
				return es;
			}
			else
			{
				return EntrySet_Renamed = new EntrySet(this);
			}
		}

		private class EntrySet : AbstractSet<Map_Entry<K, V>>
		{
			private readonly IdentityHashMap<K, V> OuterInstance;

			public EntrySet(IdentityHashMap<K, V> outerInstance)
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
//ORIGINAL LINE: Map_Entry<?,?> entry = (Map_Entry<?,?>)o;
				Map_Entry<?, ?> entry = (Map_Entry<?, ?>)o;
				return outerInstance.ContainsMapping(entry.Key, entry.Value);
			}
			public virtual bool Remove(Object o)
			{
				if (!(o is Map_Entry))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> entry = (Map_Entry<?,?>)o;
				Map_Entry<?, ?> entry = (Map_Entry<?, ?>)o;
				return outerInstance.RemoveMapping(entry.Key, entry.Value);
			}
			public virtual int Size()
			{
				return outerInstance.Size_Renamed;
			}
			public virtual void Clear()
			{
				OuterInstance.Clear();
			}
			/*
			 * Must revert from AbstractSet's impl to AbstractCollection's, as
			 * the former contains an optimization that results in incorrect
			 * behavior when c is a smaller "normal" (non-identity-based) Set.
			 */
			public virtual bool removeAll<T1>(Collection<T1> c)
			{
				Objects.RequireNonNull(c);
				bool modified = Map_Fields.False;
				for (Iterator<Map_Entry<K, V>> i = Iterator(); i.HasNext();)
				{
					if (c.Contains(i.Next()))
					{
						i.remove();
						modified = Map_Fields.True;
					}
				}
				return modified;
			}

			public virtual Object[] ToArray()
			{
				return ToArray(new Object[0]);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
			public virtual T[] toArray<T>(T[] a)
			{
				int expectedModCount = outerInstance.ModCount;
				int size = Size();
				if (a.Length < size)
				{
					a = (T[]) Array.newInstance(a.GetType().GetElementType(), size);
				}
				Object[] tab = outerInstance.Table;
				int ti = 0;
				for (int si = 0; si < tab.Length; si += 2)
				{
					Object key;
					if ((key = tab[si]) != Map_Fields.Null) // key present ?
					{
						// more elements than expected -> concurrent modification from other thread
						if (ti >= size)
						{
							throw new ConcurrentModificationException();
						}
						a[ti++] = (T) new AbstractMap.SimpleEntry<>(UnmaskNull(key), tab[si + 1]);
					}
				}
				// fewer elements than expected or concurrent modification from other thread detected
				if (ti < size || expectedModCount != outerInstance.ModCount)
				{
					throw new ConcurrentModificationException();
				}
				// final null marker as per spec
				if (ti < a.Length)
				{
					a[ti] = Map_Fields.Null;
				}
				return a;
			}

			public virtual Spliterator<Map_Entry<K, V>> Spliterator()
			{
				return new EntrySpliterator<>(OuterInstance, 0, -1, 0, 0);
			}
		}


		private const long SerialVersionUID = 8188218128353913216L;

		/// <summary>
		/// Saves the state of the <tt>IdentityHashMap</tt> instance to a stream
		/// (i.e., serializes it).
		/// 
		/// @serialData The <i>size</i> of the HashMap (the number of key-value
		///          mappings) (<tt>int</tt>), followed by the key (Object) and
		///          value (Object) for each key-value mapping represented by the
		///          IdentityHashMap.  The key-value mappings are emitted in no
		///          particular order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// Write out and any hidden stuff
			s.DefaultWriteObject();

			// Write out size (number of Mappings)
			s.WriteInt(Size_Renamed);

			// Write out keys and values (alternating)
			Object[] tab = Table;
			for (int i = 0; i < tab.Length; i += 2)
			{
				Object key = tab[i];
				if (key != Map_Fields.Null)
				{
					s.WriteObject(UnmaskNull(key));
					s.WriteObject(tab[i + 1]);
				}
			}
		}

		/// <summary>
		/// Reconstitutes the <tt>IdentityHashMap</tt> instance from a stream (i.e.,
		/// deserializes it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in any hidden stuff
			s.DefaultReadObject();

			// Read in size (number of Mappings)
			int size = s.ReadInt();
			if (size < 0)
			{
				throw new java.io.StreamCorruptedException("Illegal mappings count: " + size);
			}
			Init(Capacity(size));

			// Read the keys and values, and put the mappings in the table
			for (int i = 0; i < size; i++)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K key = (K) s.readObject();
				K key = (K) s.ReadObject();
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V value = (V) s.readObject();
				V value = (V) s.ReadObject();
				PutForCreate(key, value);
			}
		}

		/// <summary>
		/// The put method for readObject.  It does not resize the table,
		/// update modCount, etc.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void putForCreate(K key, V value) throws java.io.StreamCorruptedException
		private void PutForCreate(K key, V value)
		{
			Object Map_Fields.k = MaskNull(key);
			Object[] tab = Table;
			int len = tab.Length;
			int i = Hash(Map_Fields.k, len);

			Object item;
			while ((item = tab[i]) != Map_Fields.Null)
			{
				if (item == Map_Fields.k)
				{
					throw new java.io.StreamCorruptedException();
				}
				i = NextKeyIndex(i, len);
			}
			tab[i] = Map_Fields.k;
			tab[i + 1] = value;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public override void forEach<T1>(BiConsumer<T1> action)
		{
			Objects.RequireNonNull(action);
			int expectedModCount = ModCount;

			Object[] t = Table;
			for (int index = 0; index < t.Length; index += 2)
			{
				Object Map_Fields.k = t[index];
				if (Map_Fields.k != Map_Fields.Null)
				{
					action.Accept((K) UnmaskNull(Map_Fields.k), (V) t[index + 1]);
				}

				if (ModCount != expectedModCount)
				{
					throw new ConcurrentModificationException();
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

			Object[] t = Table;
			for (int index = 0; index < t.Length; index += 2)
			{
				Object Map_Fields.k = t[index];
				if (Map_Fields.k != Map_Fields.Null)
				{
					t[index + 1] = function.Apply((K) UnmaskNull(Map_Fields.k), (V) t[index + 1]);
				}

				if (ModCount != expectedModCount)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

		/// <summary>
		/// Similar form as array-based Spliterators, but skips blank elements,
		/// and guestimates size as decreasing by half per split.
		/// </summary>
		internal class IdentityHashMapSpliterator<K, V>
		{
			internal readonly IdentityHashMap<K, V> Map;
			internal int Index; // current index, modified on advance/split
			internal int Fence_Renamed; // -1 until first use; then one past last index
			internal int Est; // size estimate
			internal int ExpectedModCount; // initialized when fence set

			internal IdentityHashMapSpliterator(IdentityHashMap<K, V> map, int origin, int fence, int est, int expectedModCount)
			{
				this.Map = map;
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
						Est = Map.Size_Renamed;
						ExpectedModCount = Map.ModCount;
						hi = Fence_Renamed = Map.Table.Length;
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

		internal sealed class KeySpliterator<K, V> : IdentityHashMapSpliterator<K, V>, Spliterator<K>
		{
			internal KeySpliterator(IdentityHashMap<K, V> map, int origin, int fence, int est, int expectedModCount) : base(map, origin, fence, est, expectedModCount)
			{
			}

			public KeySpliterator<K, V> TrySplit()
			{
				int hi = Fence, lo = Index, mid = ((int)((uint)(lo + hi) >> 1)) & ~1;
				return (lo >= mid) ? Map_Fields.Null : new KeySpliterator<K, V>(Map, lo, Index = mid, Est = (int)((uint)Est >> 1), ExpectedModCount);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void forEachRemaining(java.util.function.Consumer<? base K> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				int i, hi, mc;
				Object key;
				IdentityHashMap<K, V> m;
				Object[] a;
				if ((m = Map) != Map_Fields.Null && (a = m.Table) != Map_Fields.Null && (i = Index) >= 0 && (Index = hi = Fence) <= a.Length)
				{
					for (; i < hi; i += 2)
					{
						if ((key = a[i]) != Map_Fields.Null)
						{
							action.Accept((K)UnmaskNull(key));
						}
					}
					if (m.ModCount == ExpectedModCount)
					{
						return;
					}
				}
				throw new ConcurrentModificationException();
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public boolean tryAdvance(java.util.function.Consumer<? base K> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				Object[] a = Map.Table;
				int hi = Fence;
				while (Index < hi)
				{
					Object key = a[Index];
					Index += 2;
					if (key != Map_Fields.Null)
					{
						action.Accept((K)UnmaskNull(key));
						if (Map.ModCount != ExpectedModCount)
						{
							throw new ConcurrentModificationException();
						}
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}

			public int Characteristics()
			{
				return (Fence_Renamed < 0 || Est == Map.Size_Renamed ? Spliterator_Fields.SIZED : 0) | Spliterator_Fields.DISTINCT;
			}
		}

		internal sealed class ValueSpliterator<K, V> : IdentityHashMapSpliterator<K, V>, Spliterator<V>
		{
			internal ValueSpliterator(IdentityHashMap<K, V> m, int origin, int fence, int est, int expectedModCount) : base(m, origin, fence, est, expectedModCount)
			{
			}

			public ValueSpliterator<K, V> TrySplit()
			{
				int hi = Fence, lo = Index, mid = ((int)((uint)(lo + hi) >> 1)) & ~1;
				return (lo >= mid) ? Map_Fields.Null : new ValueSpliterator<K, V>(Map, lo, Index = mid, Est = (int)((uint)Est >> 1), ExpectedModCount);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base V> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				int i, hi, mc;
				IdentityHashMap<K, V> m;
				Object[] a;
				if ((m = Map) != Map_Fields.Null && (a = m.Table) != Map_Fields.Null && (i = Index) >= 0 && (Index = hi = Fence) <= a.Length)
				{
					for (; i < hi; i += 2)
					{
						if (a[i] != Map_Fields.Null)
						{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V Map_Fields.v = (V)a[i+1];
							V Map_Fields.v = (V)a[i + 1];
							action.Accept(Map_Fields.v);
						}
					}
					if (m.ModCount == ExpectedModCount)
					{
						return;
					}
				}
				throw new ConcurrentModificationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base V> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				Object[] a = Map.Table;
				int hi = Fence;
				while (Index < hi)
				{
					Object key = a[Index];
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V Map_Fields.v = (V)a[index+1];
					V Map_Fields.v = (V)a[Index + 1];
					Index += 2;
					if (key != Map_Fields.Null)
					{
						action.Accept(Map_Fields.v);
						if (Map.ModCount != ExpectedModCount)
						{
							throw new ConcurrentModificationException();
						}
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}

			public int Characteristics()
			{
				return (Fence_Renamed < 0 || Est == Map.Size_Renamed ? Spliterator_Fields.SIZED : 0);
			}

		}

		internal sealed class EntrySpliterator<K, V> : IdentityHashMapSpliterator<K, V>, Spliterator<Map_Entry<K, V>>
		{
			internal EntrySpliterator(IdentityHashMap<K, V> m, int origin, int fence, int est, int expectedModCount) : base(m, origin, fence, est, expectedModCount)
			{
			}

			public EntrySpliterator<K, V> TrySplit()
			{
				int hi = Fence, lo = Index, mid = ((int)((uint)(lo + hi) >> 1)) & ~1;
				return (lo >= mid) ? Map_Fields.Null : new EntrySpliterator<K, V>(Map, lo, Index = mid, Est = (int)((uint)Est >> 1), ExpectedModCount);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base Map_Entry<K, V>> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				int i, hi, mc;
				IdentityHashMap<K, V> m;
				Object[] a;
				if ((m = Map) != Map_Fields.Null && (a = m.Table) != Map_Fields.Null && (i = Index) >= 0 && (Index = hi = Fence) <= a.Length)
				{
					for (; i < hi; i += 2)
					{
						Object key = a[i];
						if (key != Map_Fields.Null)
						{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K Map_Fields.k = (K)unmaskNull(key);
							K Map_Fields.k = (K)UnmaskNull(key);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V Map_Fields.v = (V)a[i+1];
							V Map_Fields.v = (V)a[i + 1];
							action.Accept(new AbstractMap.SimpleImmutableEntry<K, V>(Map_Fields.k, Map_Fields.v));

						}
					}
					if (m.ModCount == ExpectedModCount)
					{
						return;
					}
				}
				throw new ConcurrentModificationException();
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base Map_Entry<K,V>> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				Object[] a = Map.Table;
				int hi = Fence;
				while (Index < hi)
				{
					Object key = a[Index];
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V Map_Fields.v = (V)a[index+1];
					V Map_Fields.v = (V)a[Index + 1];
					Index += 2;
					if (key != Map_Fields.Null)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K Map_Fields.k = (K)unmaskNull(key);
						K Map_Fields.k = (K)UnmaskNull(key);
						action.Accept(new AbstractMap.SimpleImmutableEntry<K, V>(Map_Fields.k, Map_Fields.v));
						if (Map.ModCount != ExpectedModCount)
						{
							throw new ConcurrentModificationException();
						}
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}

			public int Characteristics()
			{
				return (Fence_Renamed < 0 || Est == Map.Size_Renamed ? Spliterator_Fields.SIZED : 0) | Spliterator_Fields.DISTINCT;
			}
		}

	}

}