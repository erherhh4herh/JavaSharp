using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// This class implements a hash table, which maps keys to values. Any
	/// non-<code>null</code> object can be used as a key or as a value. <para>
	/// 
	/// To successfully store and retrieve objects from a hashtable, the
	/// objects used as keys must implement the <code>hashCode</code>
	/// </para>
	/// method and the <code>equals</code> method. <para>
	/// 
	/// An instance of <code>Hashtable</code> has two parameters that affect its
	/// performance: <i>initial capacity</i> and <i>load factor</i>.  The
	/// <i>capacity</i> is the number of <i>buckets</i> in the hash table, and the
	/// <i>initial capacity</i> is simply the capacity at the time the hash table
	/// is created.  Note that the hash table is <i>open</i>: in the case of a "hash
	/// collision", a single bucket stores multiple entries, which must be searched
	/// sequentially.  The <i>load factor</i> is a measure of how full the hash
	/// table is allowed to get before its capacity is automatically increased.
	/// The initial capacity and load factor parameters are merely hints to
	/// the implementation.  The exact details as to when and whether the rehash
	/// </para>
	/// method is invoked are implementation-dependent.<para>
	/// 
	/// Generally, the default load factor (.75) offers a good tradeoff between
	/// time and space costs.  Higher values decrease the space overhead but
	/// increase the time cost to look up an entry (which is reflected in most
	/// </para>
	/// <tt>Hashtable</tt> operations, including <tt>get</tt> and <tt>put</tt>).<para>
	/// 
	/// The initial capacity controls a tradeoff between wasted space and the
	/// need for <code>rehash</code> operations, which are time-consuming.
	/// No <code>rehash</code> operations will <i>ever</i> occur if the initial
	/// capacity is greater than the maximum number of entries the
	/// <tt>Hashtable</tt> will contain divided by its load factor.  However,
	/// </para>
	/// setting the initial capacity too high can waste space.<para>
	/// 
	/// If many entries are to be made into a <code>Hashtable</code>,
	/// creating it with a sufficiently large capacity may allow the
	/// entries to be inserted more efficiently than letting it perform
	/// </para>
	/// automatic rehashing as needed to grow the table. <para>
	/// 
	/// This example creates a hashtable of numbers. It uses the names of
	/// the numbers as keys:
	/// <pre>   {@code
	///   Hashtable<String, Integer> numbers
	///     = new Hashtable<String, Integer>();
	///   numbers.put("one", 1);
	///   numbers.put("two", 2);
	///   numbers.put("three", 3);}</pre>
	/// 
	/// </para>
	/// <para>To retrieve a number, use the following code:
	/// <pre>   {@code
	///   Integer n = numbers.get("two");
	///   if (n != null) {
	///     System.out.println("two = " + n);
	///   }}</pre>
	/// 
	/// </para>
	/// <para>The iterators returned by the <tt>iterator</tt> method of the collections
	/// returned by all of this class's "collection view methods" are
	/// <em>fail-fast</em>: if the Hashtable is structurally modified at any time
	/// after the iterator is created, in any way except through the iterator's own
	/// <tt>remove</tt> method, the iterator will throw a {@link
	/// ConcurrentModificationException}.  Thus, in the face of concurrent
	/// modification, the iterator fails quickly and cleanly, rather than risking
	/// arbitrary, non-deterministic behavior at an undetermined time in the future.
	/// The Enumerations returned by Hashtable's keys and elements methods are
	/// <em>not</em> fail-fast.
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
	/// <para>As of the Java 2 platform v1.2, this class was retrofitted to
	/// implement the <seealso cref="Map"/> interface, making it a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// 
	/// Java Collections Framework</a>.  Unlike the new collection
	/// implementations, {@code Hashtable} is synchronized.  If a
	/// thread-safe implementation is not needed, it is recommended to use
	/// <seealso cref="HashMap"/> in place of {@code Hashtable}.  If a thread-safe
	/// highly-concurrent implementation is desired, then it is recommended
	/// to use <seealso cref="java.util.concurrent.ConcurrentHashMap"/> in place of
	/// {@code Hashtable}.
	/// 
	/// @author  Arthur van Hoff
	/// @author  Josh Bloch
	/// @author  Neal Gafter
	/// </para>
	/// </summary>
	/// <seealso cref=     Object#equals(java.lang.Object) </seealso>
	/// <seealso cref=     Object#hashCode() </seealso>
	/// <seealso cref=     Hashtable#rehash() </seealso>
	/// <seealso cref=     Collection </seealso>
	/// <seealso cref=     Map </seealso>
	/// <seealso cref=     HashMap </seealso>
	/// <seealso cref=     TreeMap
	/// @since JDK1.0 </seealso>
	[Serializable]
	public class Dictionary<K, V> : Dictionary<K, V>, Map<K, V>, Cloneable
	{

		/// <summary>
		/// The hash table data.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private transient Entry<?,?>[] table;
		[NonSerialized]
		private Entry<?, ?>[] Table;

		/// <summary>
		/// The total number of entries in the hash table.
		/// </summary>
		[NonSerialized]
		private int Count;

		/// <summary>
		/// The table is rehashed when its size exceeds this threshold.  (The
		/// value of this field is (int)(capacity * loadFactor).)
		/// 
		/// @serial
		/// </summary>
		private int Threshold;

		/// <summary>
		/// The load factor for the hashtable.
		/// 
		/// @serial
		/// </summary>
		private float LoadFactor;

		/// <summary>
		/// The number of times this Hashtable has been structurally modified
		/// Structural modifications are those that change the number of entries in
		/// the Hashtable or otherwise modify its internal structure (e.g.,
		/// rehash).  This field is used to make iterators on Collection-views of
		/// the Hashtable fail-fast.  (See ConcurrentModificationException).
		/// </summary>
		[NonSerialized]
		private int ModCount = 0;

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		private const long SerialVersionUID = 1421746759512286392L;

		/// <summary>
		/// Constructs a new, empty hashtable with the specified initial
		/// capacity and the specified load factor.
		/// </summary>
		/// <param name="initialCapacity">   the initial capacity of the hashtable. </param>
		/// <param name="loadFactor">        the load factor of the hashtable. </param>
		/// <exception cref="IllegalArgumentException">  if the initial capacity is less
		///             than zero, or if the load factor is nonpositive. </exception>
		public Hashtable(int initialCapacity, float loadFactor)
		{
			if (initialCapacity < 0)
			{
				throw new IllegalArgumentException("Illegal Capacity: " + initialCapacity);
			}
			if (loadFactor <= 0 || Float.IsNaN(loadFactor))
			{
				throw new IllegalArgumentException("Illegal Load: " + loadFactor);
			}

			if (initialCapacity == 0)
			{
				initialCapacity = 1;
			}
			this.LoadFactor = loadFactor;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: table = new Entry<?,?>[initialCapacity];
			Table = new Entry<?, ?>[initialCapacity];
			Threshold = (int)System.Math.Min(initialCapacity * loadFactor, MAX_ARRAY_SIZE + 1);
		}

		/// <summary>
		/// Constructs a new, empty hashtable with the specified initial capacity
		/// and default load factor (0.75).
		/// </summary>
		/// <param name="initialCapacity">   the initial capacity of the hashtable. </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is less
		///              than zero. </exception>
		public Hashtable(int initialCapacity) : this(initialCapacity, 0.75f)
		{
		}

		/// <summary>
		/// Constructs a new, empty hashtable with a default initial capacity (11)
		/// and load factor (0.75).
		/// </summary>
		public Hashtable() : this(11, 0.75f)
		{
		}

		/// <summary>
		/// Constructs a new hashtable with the same mappings as the given
		/// Map.  The hashtable is created with an initial capacity sufficient to
		/// hold the mappings in the given Map and a default load factor (0.75).
		/// </summary>
		/// <param name="t"> the map whose mappings are to be placed in this map. </param>
		/// <exception cref="NullPointerException"> if the specified map is null.
		/// @since   1.2 </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public Hashtable(Map<? extends K, ? extends V> t)
		public Dictionary<T1>(Map<T1> t) where T1 : K where ? : V : this(System.Math.Max(2 * t.Size(), 11), 0.75f)
		{
			PutAll(t);
		}

		/// <summary>
		/// Returns the number of keys in this hashtable.
		/// </summary>
		/// <returns>  the number of keys in this hashtable. </returns>
		public virtual int Size()
		{
			lock (this)
			{
				return Count;
			}
		}

		/// <summary>
		/// Tests if this hashtable maps no keys to values.
		/// </summary>
		/// <returns>  <code>true</code> if this hashtable maps no keys to values;
		///          <code>false</code> otherwise. </returns>
		public virtual bool Empty
		{
			get
			{
				lock (this)
				{
					return Count == 0;
				}
			}
		}

		/// <summary>
		/// Returns an enumeration of the keys in this hashtable.
		/// </summary>
		/// <returns>  an enumeration of the keys in this hashtable. </returns>
		/// <seealso cref=     Enumeration </seealso>
		/// <seealso cref=     #elements() </seealso>
		/// <seealso cref=     #keySet() </seealso>
		/// <seealso cref=     Map </seealso>
		public virtual IEnumerator<K> Keys()
		{
			lock (this)
			{
				return this.GetEnumeration<K>(KEYS);
			}
		}

		/// <summary>
		/// Returns an enumeration of the values in this hashtable.
		/// Use the Enumeration methods on the returned object to fetch the elements
		/// sequentially.
		/// </summary>
		/// <returns>  an enumeration of the values in this hashtable. </returns>
		/// <seealso cref=     java.util.Enumeration </seealso>
		/// <seealso cref=     #keys() </seealso>
		/// <seealso cref=     #values() </seealso>
		/// <seealso cref=     Map </seealso>
		public virtual IEnumerator<V> Elements()
		{
			lock (this)
			{
				return this.GetEnumeration<V>(VALUES);
			}
		}

		/// <summary>
		/// Tests if some key maps into the specified value in this hashtable.
		/// This operation is more expensive than the {@link #containsKey
		/// containsKey} method.
		/// 
		/// <para>Note that this method is identical in functionality to
		/// <seealso cref="#containsValue containsValue"/>, (which is part of the
		/// <seealso cref="Map"/> interface in the collections framework).
		/// 
		/// </para>
		/// </summary>
		/// <param name="value">   a value to search for </param>
		/// <returns>     <code>true</code> if and only if some key maps to the
		///             <code>value</code> argument in this hashtable as
		///             determined by the <tt>equals</tt> method;
		///             <code>false</code> otherwise. </returns>
		/// <exception cref="NullPointerException">  if the value is <code>null</code> </exception>
		public virtual bool Contains(Object value)
		{
			lock (this)
			{
				if (value == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
        
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				for (int i = tab.Length ; i-- > 0 ;)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Entry<?,?> e = tab[i] ; e != Map_Fields.null ; e = e.next)
					for (Entry<?, ?> e = tab[i] ; e != Map_Fields.Null ; e = e.Next)
					{
						if (e.Value_Renamed.Equals(value))
						{
							return Map_Fields.True;
						}
					}
				}
				return Map_Fields.False;
			}
		}

		/// <summary>
		/// Returns true if this hashtable maps one or more keys to this value.
		/// 
		/// <para>Note that this method is identical in functionality to {@link
		/// #contains contains} (which predates the <seealso cref="Map"/> interface).
		/// 
		/// </para>
		/// </summary>
		/// <param name="value"> value whose presence in this hashtable is to be tested </param>
		/// <returns> <tt>true</tt> if this map maps one or more keys to the
		///         specified value </returns>
		/// <exception cref="NullPointerException">  if the value is <code>null</code>
		/// @since 1.2 </exception>
		public virtual bool ContainsValue(Object value)
		{
			return Contains(value);
		}

		/// <summary>
		/// Tests if the specified object is a key in this hashtable.
		/// </summary>
		/// <param name="key">   possible key </param>
		/// <returns>  <code>true</code> if and only if the specified object
		///          is a key in this hashtable, as determined by the
		///          <tt>equals</tt> method; <code>false</code> otherwise. </returns>
		/// <exception cref="NullPointerException">  if the key is <code>null</code> </exception>
		/// <seealso cref=     #contains(Object) </seealso>
		public virtual bool ContainsKey(Object key)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Entry<?,?> e = tab[index] ; e != Map_Fields.null ; e = e.next)
				for (Entry<?, ?> e = tab[index] ; e != Map_Fields.Null ; e = e.Next)
				{
					if ((e.Hash == hash) && e.Key_Renamed.Equals(key))
					{
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}
		}

		/// <summary>
		/// Returns the value to which the specified key is mapped,
		/// or {@code null} if this map contains no mapping for the key.
		/// 
		/// <para>More formally, if this map contains a mapping from a key
		/// {@code k} to a value {@code v} such that {@code (key.equals(k))},
		/// then this method returns {@code v}; otherwise it returns
		/// {@code null}.  (There can be at most one such mapping.)
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> the key whose associated value is to be returned </param>
		/// <returns> the value to which the specified key is mapped, or
		///         {@code null} if this map contains no mapping for the key </returns>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		/// <seealso cref=     #put(Object, Object) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public synchronized V get(Object key)
		public virtual V Get(Object key)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Entry<?,?> e = tab[index] ; e != Map_Fields.null ; e = e.next)
				for (Entry<?, ?> e = tab[index] ; e != Map_Fields.Null ; e = e.Next)
				{
					if ((e.Hash == hash) && e.Key_Renamed.Equals(key))
					{
						return (V)e.Value_Renamed;
					}
				}
				return Map_Fields.Null;
			}
		}

		/// <summary>
		/// The maximum size of array to allocate.
		/// Some VMs reserve some header words in an array.
		/// Attempts to allocate larger arrays may result in
		/// OutOfMemoryError: Requested array size exceeds VM limit
		/// </summary>
		private static readonly int MAX_ARRAY_SIZE = Integer.MaxValue - 8;

		/// <summary>
		/// Increases the capacity of and internally reorganizes this
		/// hashtable, in order to accommodate and access its entries more
		/// efficiently.  This method is called automatically when the
		/// number of keys in the hashtable exceeds this hashtable's capacity
		/// and load factor.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void rehash()
		protected internal virtual void Rehash()
		{
			int oldCapacity = Table.Length;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?>[] oldMap = table;
			Entry<?, ?>[] oldMap = Table;

			// overflow-conscious code
			int newCapacity = (oldCapacity << 1) + 1;
			if (newCapacity - MAX_ARRAY_SIZE > 0)
			{
				if (oldCapacity == MAX_ARRAY_SIZE)
				{
					// Keep running with MAX_ARRAY_SIZE buckets
					return;
				}
				newCapacity = MAX_ARRAY_SIZE;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?>[] newMap = new Entry<?,?>[newCapacity];
			Entry<?, ?>[] newMap = new Entry<?, ?>[newCapacity];

			ModCount++;
			Threshold = (int)System.Math.Min(newCapacity * LoadFactor, MAX_ARRAY_SIZE + 1);
			Table = newMap;

			for (int i = oldCapacity ; i-- > 0 ;)
			{
				for (Entry<K, V> old = (Entry<K, V>)oldMap[i] ; old != Map_Fields.Null ;)
				{
					Entry<K, V> e = old;
					old = old.Next;

					int index = (e.Hash & 0x7FFFFFFF) % newCapacity;
					e.Next = (Entry<K, V>)newMap[index];
					newMap[index] = e;
				}
			}
		}

		private void AddEntry(int hash, K key, V value, int index)
		{
			ModCount++;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
			Entry<?, ?>[] tab = Table;
			if (Count >= Threshold)
			{
				// Rehash the table if the threshold is exceeded
				Rehash();

				tab = Table;
				hash = key.HashCode();
				index = (hash & 0x7FFFFFFF) % tab.Length;
			}

			// Creates the new entry.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>) tab[index];
			Entry<K, V> e = (Entry<K, V>) tab[index];
			tab[index] = new Entry<>(hash, key, value, e);
			Count++;
		}

		/// <summary>
		/// Maps the specified <code>key</code> to the specified
		/// <code>value</code> in this hashtable. Neither the key nor the
		/// value can be <code>null</code>. <para>
		/// 
		/// The value can be retrieved by calling the <code>get</code> method
		/// with a key that is equal to the original key.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key">     the hashtable key </param>
		/// <param name="value">   the value </param>
		/// <returns>     the previous value of the specified key in this hashtable,
		///             or <code>null</code> if it did not have one </returns>
		/// <exception cref="NullPointerException">  if the key or value is
		///               <code>null</code> </exception>
		/// <seealso cref=     Object#equals(Object) </seealso>
		/// <seealso cref=     #get(Object) </seealso>
		public virtual V Put(K key, V value)
		{
			lock (this)
			{
				// Make sure the value is not null
				if (value == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
        
				// Makes sure the key is not already in the hashtable.
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> entry = (Entry<K,V>)tab[index];
				Entry<K, V> entry = (Entry<K, V>)tab[index];
				for (; entry != Map_Fields.Null ; entry = entry.Next)
				{
					if ((entry.Hash == hash) && entry.Key_Renamed.Equals(key))
					{
						V old = entry.Value_Renamed;
						entry.Value_Renamed = value;
						return old;
					}
				}
        
				AddEntry(hash, key, value, index);
				return Map_Fields.Null;
			}
		}

		/// <summary>
		/// Removes the key (and its corresponding value) from this
		/// hashtable. This method does nothing if the key is not in the hashtable.
		/// </summary>
		/// <param name="key">   the key that needs to be removed </param>
		/// <returns>  the value to which the key had been mapped in this hashtable,
		///          or <code>null</code> if the key did not have a mapping </returns>
		/// <exception cref="NullPointerException">  if the key is <code>null</code> </exception>
		public virtual V Remove(Object key)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
				Entry<K, V> e = (Entry<K, V>)tab[index];
				for (Entry<K, V> prev = Map_Fields.Null ; e != Map_Fields.Null ; prev = e, e = e.Next)
				{
					if ((e.Hash == hash) && e.Key_Renamed.Equals(key))
					{
						ModCount++;
						if (prev != Map_Fields.Null)
						{
							prev.Next = e.Next;
						}
						else
						{
							tab[index] = e.Next;
						}
						Count--;
						V Map_Fields.OldValue = e.Value_Renamed;
						e.Value_Renamed = Map_Fields.Null;
						return Map_Fields.OldValue;
					}
				}
				return Map_Fields.Null;
			}
		}

		/// <summary>
		/// Copies all of the mappings from the specified map to this hashtable.
		/// These mappings will replace any mappings that this hashtable had for any
		/// of the keys currently in the specified map.
		/// </summary>
		/// <param name="t"> mappings to be stored in this map </param>
		/// <exception cref="NullPointerException"> if the specified map is null
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public synchronized void putAll(Map<? extends K, ? extends V> t)
		public virtual void putAll<T1>(Map<T1> t) where T1 : K where ? : V
		{
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Map_Entry<? extends K, ? extends V> e : t.entrySet())
				foreach (Map_Entry<?, ?> e in t.EntrySet())
				{
					Put(e.Key, e.Value);
				}
			}
		}

		/// <summary>
		/// Clears this hashtable so that it contains no keys.
		/// </summary>
		public virtual void Clear()
		{
			lock (this)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				ModCount++;
				for (int index = tab.Length; --index >= 0;)
				{
					tab[index] = Map_Fields.Null;
				}
				Count = 0;
			}
		}

		/// <summary>
		/// Creates a shallow copy of this hashtable. All the structure of the
		/// hashtable itself is copied, but the keys and values are not cloned.
		/// This is a relatively expensive operation.
		/// </summary>
		/// <returns>  a clone of the hashtable </returns>
		public virtual Object Clone()
		{
			lock (this)
			{
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Hashtable<?,?> t = (Hashtable<?,?>)base.clone();
					Dictionary<?, ?> t = (Dictionary<?, ?>)base.Clone();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: t.table = new Entry<?,?>[table.length];
					t.Table = new Entry<?, ?>[Table.Length];
					for (int i = Table.Length ; i-- > 0 ;)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: t.table[i] = (table[i] != Map_Fields.null) ? (Entry<?,?>) table[i].clone() : Map_Fields.null;
						t.Table[i] = (Table[i] != Map_Fields.Null) ? (Entry<?, ?>) Table[i].Clone() : Map_Fields.Null;
					}
					t.KeySet_Renamed = Map_Fields.Null;
					t.EntrySet_Renamed = Map_Fields.Null;
					t.Values_Renamed = Map_Fields.Null;
					t.ModCount = 0;
					return t;
				}
				catch (CloneNotSupportedException e)
				{
					// this shouldn't happen, since we are Cloneable
					throw new InternalError(e);
				}
			}
		}

		/// <summary>
		/// Returns a string representation of this <tt>Hashtable</tt> object
		/// in the form of a set of entries, enclosed in braces and separated
		/// by the ASCII characters "<tt>,&nbsp;</tt>" (comma and space). Each
		/// entry is rendered as the key, an equals sign <tt>=</tt>, and the
		/// associated element, where the <tt>toString</tt> method is used to
		/// convert the key and element to strings.
		/// </summary>
		/// <returns>  a string representation of this hashtable </returns>
		public override String ToString()
		{
			lock (this)
			{
				int max = Size() - 1;
				if (max == -1)
				{
					return "{}";
				}
        
				StringBuilder sb = new StringBuilder();
				Iterator<Map_Entry<K, V>> it = EntrySet().Iterator();
        
				sb.Append('{');
				for (int i = 0; ; i++)
				{
					Map_Entry<K, V> e = it.Next();
					K key = e.Key;
					V value = e.Value;
					sb.Append(key == this ? "(this Map)" : key.ToString());
					sb.Append('=');
					sb.Append(value == this ? "(this Map)" : value.ToString());
        
					if (i == max)
					{
						return sb.Append('}').ToString();
					}
					sb.Append(", ");
				}
			}
		}


		private IEnumerator<T> getEnumeration<T>(int type)
		{
			if (Count == 0)
			{
				return Collections.EmptyEnumeration();
			}
			else
			{
				return new Enumerator<>(type, Map_Fields.False);
			}
		}

		private Iterator<T> getIterator<T>(int type)
		{
			if (Count == 0)
			{
				return Collections.EmptyIterator();
			}
			else
			{
				return new Enumerator<>(type, Map_Fields.True);
			}
		}

		// Views

		/// <summary>
		/// Each of these fields are initialized to contain an instance of the
		/// appropriate view the first time this view is requested.  The views are
		/// stateless, so there's no reason to create more than one of each.
		/// </summary>
		[NonSerialized]
		private volatile Set<K> KeySet_Renamed;
		[NonSerialized]
		private volatile Set<Map_Entry<K, V>> EntrySet_Renamed;
		[NonSerialized]
		private volatile Collection<V> Values_Renamed;

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
		/// 
		/// @since 1.2
		/// </summary>
		public virtual Set<K> KeySet()
		{
			if (KeySet_Renamed == Map_Fields.Null)
			{
				KeySet_Renamed = Collections.SynchronizedSet(new KeySet(this), this);
			}
			return KeySet_Renamed;
		}

		private class KeySet : AbstractSet<K>
		{
			private readonly Dictionary<K, V> OuterInstance;

			public KeySet(Dictionary<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<K> Iterator()
			{
				return outerInstance.GetIterator(KEYS);
			}
			public virtual int Size()
			{
				return outerInstance.Count;
			}
			public virtual bool Contains(Object o)
			{
				return outerInstance.ContainsKey(o);
			}
			public virtual bool Remove(Object o)
			{
				return OuterInstance.Remove(o) != Map_Fields.Null;
			}
			public virtual void Clear()
			{
				OuterInstance.Clear();
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
		/// 
		/// @since 1.2
		/// </summary>
		public virtual Set<Map_Entry<K, V>> EntrySet()
		{
			if (EntrySet_Renamed == Map_Fields.Null)
			{
				EntrySet_Renamed = Collections.SynchronizedSet(new EntrySet(this), this);
			}
			return EntrySet_Renamed;
		}

		private class EntrySet : AbstractSet<Map_Entry<K, V>>
		{
			private readonly Dictionary<K, V> OuterInstance;

			public EntrySet(Dictionary<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<Map_Entry<K, V>> Iterator()
			{
				return outerInstance.GetIterator(ENTRIES);
			}

			public virtual bool Add(Map_Entry<K, V> o)
			{
				return base.Add(o);
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
				Object key = entry.Key;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?>[] tab = table;
				Entry<?, ?>[] tab = outerInstance.Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Entry<?,?> e = tab[index]; e != Map_Fields.null; e = e.next)
				for (Entry<?, ?> e = tab[index]; e != Map_Fields.Null; e = e.Next)
				{
					if (e.Hash == hash && e.Equals(entry))
					{
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
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
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?>[] tab = table;
				Entry<?, ?>[] tab = outerInstance.Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
				Entry<K, V> e = (Entry<K, V>)tab[index];
				for (Entry<K, V> prev = Map_Fields.Null; e != Map_Fields.Null; prev = e, e = e.Next)
				{
					if (e.Hash == hash && e.Equals(entry))
					{
						outerInstance.ModCount++;
						if (prev != Map_Fields.Null)
						{
							prev.Next = e.Next;
						}
						else
						{
							tab[index] = e.Next;
						}

						outerInstance.Count--;
						e.Value_Renamed = Map_Fields.Null;
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}

			public virtual int Size()
			{
				return outerInstance.Count;
			}

			public virtual void Clear()
			{
				OuterInstance.Clear();
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
		/// 
		/// @since 1.2
		/// </summary>
		public virtual Collection<V> Values()
		{
			if (Values_Renamed == Map_Fields.Null)
			{
				Values_Renamed = Collections.SynchronizedCollection(new ValueCollection(this), this);
			}
			return Values_Renamed;
		}

		private class ValueCollection : AbstractCollection<V>
		{
			private readonly Dictionary<K, V> OuterInstance;

			public ValueCollection(Dictionary<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<V> Iterator()
			{
				return outerInstance.GetIterator(VALUES);
			}
			public virtual int Size()
			{
				return outerInstance.Count;
			}
			public virtual bool Contains(Object o)
			{
				return outerInstance.ContainsValue(o);
			}
			public virtual void Clear()
			{
				OuterInstance.Clear();
			}
		}

		// Comparison and hashing

		/// <summary>
		/// Compares the specified Object with this Map for equality,
		/// as per the definition in the Map interface.
		/// </summary>
		/// <param name="o"> object to be compared for equality with this hashtable </param>
		/// <returns> true if the specified Object is equal to this Map </returns>
		/// <seealso cref= Map#equals(Object)
		/// @since 1.2 </seealso>
		public override bool Equals(Object o)
		{
			lock (this)
			{
				if (o == this)
				{
					return Map_Fields.True;
				}
        
				if (!(o is Map))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map<?,?> t = (Map<?,?>) o;
				Map<?, ?> t = (Map<?, ?>) o;
				if (t.Size() != Size())
				{
					return Map_Fields.False;
				}
        
				try
				{
					Iterator<Map_Entry<K, V>> i = EntrySet().Iterator();
					while (i.HasNext())
					{
						Map_Entry<K, V> e = i.Next();
						K key = e.Key;
						V value = e.Value;
						if (value == Map_Fields.Null)
						{
							if (!(t.Get(key) == Map_Fields.Null && t.ContainsKey(key)))
							{
								return Map_Fields.False;
							}
						}
						else
						{
							if (!value.Equals(t.Get(key)))
							{
								return Map_Fields.False;
							}
						}
					}
				}
				catch (ClassCastException)
				{
					return Map_Fields.False;
				}
				catch (NullPointerException)
				{
					return Map_Fields.False;
				}
        
				return Map_Fields.True;
			}
		}

		/// <summary>
		/// Returns the hash code value for this Map as per the definition in the
		/// Map interface.
		/// </summary>
		/// <seealso cref= Map#hashCode()
		/// @since 1.2 </seealso>
		public override int HashCode()
		{
			lock (this)
			{
				/*
				 * This code detects the recursion caused by computing the hash code
				 * of a self-referential hash table and prevents the stack overflow
				 * that would otherwise result.  This allows certain 1.1-era
				 * applets with self-referential hash tables to work.  This code
				 * abuses the loadFactor field to do double-duty as a hashCode
				 * in progress flag, so as not to worsen the space performance.
				 * A negative load factor indicates that hash code computation is
				 * in progress.
				 */
				int h = 0;
				if (Count == 0 || LoadFactor < 0)
				{
					return h; // Returns zero
				}
        
				LoadFactor = -LoadFactor; // Mark hashCode computation in progress
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?>[] tab = table;
				Entry<?, ?>[] tab = Table;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Entry<?,?> entry : tab)
				foreach (Entry<?, ?> entry in tab)
				{
					while (entry != Map_Fields.Null)
					{
						h += entry.HashCode();
						entry = entry.Next;
					}
				}
        
				LoadFactor = -LoadFactor; // Mark hashCode computation complete
        
				return h;
			}
		}

		public override V GetOrDefault(Object key, V defaultValue)
		{
			lock (this)
			{
				V result = Get(key);
				return (Map_Fields.Null == result) ? defaultValue : result;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public synchronized void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		public override void forEach<T1>(BiConsumer<T1> action)
		{
			lock (this)
			{
				Objects.RequireNonNull(action); // explicit check required in case
													// table is empty.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expectedModCount = modCount;
				int expectedModCount = ModCount;
        
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?, ?>[] tab = table;
				Entry<?, ?>[] tab = Table;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Entry<?, ?> entry : tab)
				foreach (Entry<?, ?> entry in tab)
				{
					while (entry != Map_Fields.Null)
					{
						action.Accept((K)entry.Key_Renamed, (V)entry.Value_Renamed);
						entry = entry.Next;
        
						if (expectedModCount != ModCount)
						{
							throw new ConcurrentModificationException();
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public synchronized void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public synchronized void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public override void replaceAll<T1>(BiFunction<T1> function) where ? : V
		{
			lock (this)
			{
				Objects.RequireNonNull(function); // explicit check required in case
													  // table is empty.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expectedModCount = modCount;
				int expectedModCount = ModCount;
        
				Entry<K, V>[] tab = (Entry<K, V>[])Table;
				foreach (Entry<K, V> entry in tab)
				{
					while (entry != Map_Fields.Null)
					{
						entry.Value_Renamed = Objects.RequireNonNull(function.Apply(entry.Key_Renamed, entry.Value_Renamed));
						entry = entry.Next;
        
						if (expectedModCount != ModCount)
						{
							throw new ConcurrentModificationException();
						}
					}
				}
			}
		}

		public override V PutIfAbsent(K key, V value)
		{
			lock (this)
			{
				Objects.RequireNonNull(value);
        
				// Makes sure the key is not already in the hashtable.
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> entry = (Entry<K,V>)tab[index];
				Entry<K, V> entry = (Entry<K, V>)tab[index];
				for (; entry != Map_Fields.Null; entry = entry.Next)
				{
					if ((entry.Hash == hash) && entry.Key_Renamed.Equals(key))
					{
						V old = entry.Value_Renamed;
						if (old == Map_Fields.Null)
						{
							entry.Value_Renamed = value;
						}
						return old;
					}
				}
        
				AddEntry(hash, key, value, index);
				return Map_Fields.Null;
			}
		}

		public override bool Remove(Object key, Object value)
		{
			lock (this)
			{
				Objects.RequireNonNull(value);
        
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
				Entry<K, V> e = (Entry<K, V>)tab[index];
				for (Entry<K, V> prev = Map_Fields.Null; e != Map_Fields.Null; prev = e, e = e.Next)
				{
					if ((e.Hash == hash) && e.Key_Renamed.Equals(key) && e.Value_Renamed.Equals(value))
					{
						ModCount++;
						if (prev != Map_Fields.Null)
						{
							prev.Next = e.Next;
						}
						else
						{
							tab[index] = e.Next;
						}
						Count--;
						e.Value_Renamed = Map_Fields.Null;
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}
		}

		public override bool Replace(K key, V Map_Fields, V Map_Fields)
		{
			lock (this)
			{
				Objects.RequireNonNull(Map_Fields.OldValue);
				Objects.RequireNonNull(Map_Fields.NewValue);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
				Entry<K, V> e = (Entry<K, V>)tab[index];
				for (; e != Map_Fields.Null; e = e.Next)
				{
					if ((e.Hash == hash) && e.Key_Renamed.Equals(key))
					{
						if (e.Value_Renamed.Equals(Map_Fields.OldValue))
						{
							e.Value_Renamed = Map_Fields.NewValue;
							return Map_Fields.True;
						}
						else
						{
							return Map_Fields.False;
						}
					}
				}
				return Map_Fields.False;
			}
		}

		public override V Replace(K key, V value)
		{
			lock (this)
			{
				Objects.RequireNonNull(value);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
				Entry<K, V> e = (Entry<K, V>)tab[index];
				for (; e != Map_Fields.Null; e = e.Next)
				{
					if ((e.Hash == hash) && e.Key_Renamed.Equals(key))
					{
						V Map_Fields.OldValue = e.Value_Renamed;
						e.Value_Renamed = value;
						return Map_Fields.OldValue;
					}
				}
				return Map_Fields.Null;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
		public override V computeIfAbsent<T1>(K key, Function<T1> mappingFunction) where T1 : V
		{
			lock (this)
			{
				Objects.RequireNonNull(mappingFunction);
        
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
				Entry<K, V> e = (Entry<K, V>)tab[index];
				for (; e != Map_Fields.Null; e = e.Next)
				{
					if (e.Hash == hash && e.Key_Renamed.Equals(key))
					{
						// Hashtable not accept null value
						return e.Value_Renamed;
					}
				}
        
				V Map_Fields.NewValue = mappingFunction.Apply(key);
				if (Map_Fields.NewValue != Map_Fields.Null)
				{
					AddEntry(hash, key, Map_Fields.NewValue, index);
				}
        
				return Map_Fields.NewValue;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
		public override V computeIfPresent<T1>(K key, BiFunction<T1> remappingFunction) where T1 : V
		{
			lock (this)
			{
				Objects.RequireNonNull(remappingFunction);
        
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
				Entry<K, V> e = (Entry<K, V>)tab[index];
				for (Entry<K, V> prev = Map_Fields.Null; e != Map_Fields.Null; prev = e, e = e.Next)
				{
					if (e.Hash == hash && e.Key_Renamed.Equals(key))
					{
						V Map_Fields.NewValue = remappingFunction.Apply(key, e.Value_Renamed);
						if (Map_Fields.NewValue == Map_Fields.Null)
						{
							ModCount++;
							if (prev != Map_Fields.Null)
							{
								prev.Next = e.Next;
							}
							else
							{
								tab[index] = e.Next;
							}
							Count--;
						}
						else
						{
							e.Value_Renamed = Map_Fields.NewValue;
						}
						return Map_Fields.NewValue;
					}
				}
				return Map_Fields.Null;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
		public override V compute<T1>(K key, BiFunction<T1> remappingFunction) where T1 : V
		{
			lock (this)
			{
				Objects.RequireNonNull(remappingFunction);
        
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
				Entry<K, V> e = (Entry<K, V>)tab[index];
				for (Entry<K, V> prev = Map_Fields.Null; e != Map_Fields.Null; prev = e, e = e.Next)
				{
					if (e.Hash == hash && Objects.Equals(e.Key_Renamed, key))
					{
						V Map_Fields.NewValue = remappingFunction.Apply(key, e.Value_Renamed);
						if (Map_Fields.NewValue == Map_Fields.Null)
						{
							ModCount++;
							if (prev != Map_Fields.Null)
							{
								prev.Next = e.Next;
							}
							else
							{
								tab[index] = e.Next;
							}
							Count--;
						}
						else
						{
							e.Value_Renamed = Map_Fields.NewValue;
						}
						return Map_Fields.NewValue;
					}
				}
        
				V Map_Fields.NewValue = remappingFunction.Apply(key, Map_Fields.Null);
				if (Map_Fields.NewValue != Map_Fields.Null)
				{
					AddEntry(hash, key, Map_Fields.NewValue, index);
				}
        
				return Map_Fields.NewValue;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public synchronized V merge(K key, V value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
		public override V merge<T1>(K key, V value, BiFunction<T1> remappingFunction) where T1 : V
		{
			lock (this)
			{
				Objects.RequireNonNull(remappingFunction);
        
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> tab[] = table;
				Entry<?, ?>[] tab = Table;
				int hash = key.HashCode();
				int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
				Entry<K, V> e = (Entry<K, V>)tab[index];
				for (Entry<K, V> prev = Map_Fields.Null; e != Map_Fields.Null; prev = e, e = e.Next)
				{
					if (e.Hash == hash && e.Key_Renamed.Equals(key))
					{
						V Map_Fields.NewValue = remappingFunction.Apply(e.Value_Renamed, value);
						if (Map_Fields.NewValue == Map_Fields.Null)
						{
							ModCount++;
							if (prev != Map_Fields.Null)
							{
								prev.Next = e.Next;
							}
							else
							{
								tab[index] = e.Next;
							}
							Count--;
						}
						else
						{
							e.Value_Renamed = Map_Fields.NewValue;
						}
						return Map_Fields.NewValue;
					}
				}
        
				if (value != Map_Fields.Null)
				{
					AddEntry(hash, key, value, index);
				}
        
				return value;
			}
		}

		/// <summary>
		/// Save the state of the Hashtable to a stream (i.e., serialize it).
		/// 
		/// @serialData The <i>capacity</i> of the Hashtable (the length of the
		///             bucket array) is emitted (int), followed by the
		///             <i>size</i> of the Hashtable (the number of key-value
		///             mappings), followed by the key (Object) and value (Object)
		///             for each key-value mapping represented by the Hashtable
		///             The key-value mappings are emitted in no particular order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			Entry<Object, Object> entryStack = Map_Fields.Null;

			lock (this)
			{
				// Write out the length, threshold, loadfactor
				s.DefaultWriteObject();

				// Write out length, count of elements
				s.WriteInt(Table.Length);
				s.WriteInt(Count);

				// Stack copies of the entries in the table
				for (int index = 0; index < Table.Length; index++)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> entry = table[index];
					Entry<?, ?> entry = Table[index];

					while (entry != Map_Fields.Null)
					{
						entryStack = new Entry<>(0, entry.Key_Renamed, entry.Value_Renamed, entryStack);
						entry = entry.Next;
					}
				}
			}

			// Write out the key/value objects from the stacked entries
			while (entryStack != Map_Fields.Null)
			{
				s.WriteObject(entryStack.Key_Renamed);
				s.WriteObject(entryStack.Value_Renamed);
				entryStack = entryStack.Next;
			}
		}

		/// <summary>
		/// Reconstitute the Hashtable from a stream (i.e., deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in the length, threshold, and loadfactor
			s.DefaultReadObject();

			// Read the original length of the array and number of elements
			int origlength = s.ReadInt();
			int elements = s.ReadInt();

			// Compute new size with a bit of room 5% to grow but
			// no larger than the original size.  Make the length
			// odd if it's large enough, this helps distribute the entries.
			// Guard against the length ending up zero, that's not valid.
			int length = (int)(elements * LoadFactor) + (elements / 20) + 3;
			if (length > elements && (length & 1) == 0)
			{
				length--;
			}
			if (origlength > 0 && length > origlength)
			{
				length = origlength;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: table = new Entry<?,?>[length];
			Table = new Entry<?, ?>[length];
			Threshold = (int)System.Math.Min(length * LoadFactor, MAX_ARRAY_SIZE + 1);
			Count = 0;

			// Read the number of elements and then all the key/value objects
			for (; elements > 0; elements--)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K key = (K)s.readObject();
				K key = (K)s.ReadObject();
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V value = (V)s.readObject();
				V value = (V)s.ReadObject();
				// synch could be eliminated for performance
				ReconstitutionPut(Table, key, value);
			}
		}

		/// <summary>
		/// The put method used by readObject. This is provided because put
		/// is overridable and should not be called in readObject since the
		/// subclass will not yet be initialized.
		/// 
		/// <para>This differs from the regular put method in several ways. No
		/// checking for rehashing is necessary since the number of elements
		/// initially in the table is known. The modCount is not incremented
		/// because we are creating a new instance. Also, no return value
		/// is needed.
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void reconstitutionPut(Entry<?,?>[] tab, K key, V value) throws StreamCorruptedException
		private void reconstitutionPut<T1>(Entry<T1>[] tab, K key, V value)
		{
			if (value == Map_Fields.Null)
			{
				throw new java.io.StreamCorruptedException();
			}
			// Makes sure the key is not already in the hashtable.
			// This should not happen in deserialized version.
			int hash = key.HashCode();
			int index = (hash & 0x7FFFFFFF) % tab.Length;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Entry<?,?> e = tab[index] ; e != Map_Fields.null ; e = e.next)
			for (Entry<?, ?> e = tab[index] ; e != Map_Fields.Null ; e = e.Next)
			{
				if ((e.Hash == hash) && e.Key_Renamed.Equals(key))
				{
					throw new java.io.StreamCorruptedException();
				}
			}
			// Creates the new entry.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
			Entry<K, V> e = (Entry<K, V>)tab[index];
			tab[index] = new Entry<>(hash, key, value, e);
			Count++;
		}

		/// <summary>
		/// Hashtable bucket collision list entry
		/// </summary>
		private class Entry<K, V> : Map_Entry<K, V>
		{
			internal readonly int Hash;
			internal readonly K Key_Renamed;
			internal V Value_Renamed;
			internal Entry<K, V> Next;

			protected internal Entry(int hash, K key, V value, Entry<K, V> next)
			{
				this.Hash = hash;
				this.Key_Renamed = key;
				this.Value_Renamed = value;
				this.Next = next;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected Object clone()
			protected internal virtual Object Clone()
			{
				return new Entry<>(Hash, Key_Renamed, Value_Renamed, (Next == Map_Fields.Null ? Map_Fields.Null : (Entry<K, V>) Next.Clone()));
			}

			// Map.Entry Ops

			public virtual K Key
			{
				get
				{
					return Key_Renamed;
				}
			}

			public virtual V Value
			{
				get
				{
					return Value_Renamed;
				}
			}

			public virtual V SetValue(V value)
			{
				if (value == Map_Fields.Null)
				{
					throw new NullPointerException();
				}

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

				return (Key_Renamed == Map_Fields.Null ? e.Key == Map_Fields.Null : Key_Renamed.Equals(e.Key)) && (Value_Renamed == Map_Fields.Null ? e.Value == Map_Fields.Null : Value_Renamed.Equals(e.Value));
			}

			public override int HashCode()
			{
				return Hash ^ Objects.HashCode(Value_Renamed);
			}

			public override String ToString()
			{
				return Key_Renamed.ToString() + "=" + Value_Renamed.ToString();
			}
		}

		// Types of Enumerations/Iterations
		private const int KEYS = 0;
		private const int VALUES = 1;
		private const int ENTRIES = 2;

		/// <summary>
		/// A hashtable enumerator class.  This class implements both the
		/// Enumeration and Iterator interfaces, but individual instances
		/// can be created with the Iterator methods disabled.  This is necessary
		/// to avoid unintentionally increasing the capabilities granted a user
		/// by passing an Enumeration.
		/// </summary>
		private class Enumerator<T> : IEnumerator<T>, Iterator<T>
		{
			internal bool InstanceFieldsInitialized = Map_Fields.False;

			internal virtual void InitializeInstanceFields()
			{
				Table = OuterInstance.Table;
				Index = Table.Length;
				ExpectedModCount = outerInstance.ModCount;
			}

			private readonly Dictionary<K, V> OuterInstance;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?>[] table = Hashtable.this.table;
			internal Entry<?, ?>[] Table;
			internal int Index;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> entry;
			internal Entry<?, ?> Entry;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> lastReturned;
			internal Entry<?, ?> LastReturned;
			internal int Type;

			/// <summary>
			/// Indicates whether this Enumerator is serving as an Iterator
			/// or an Enumeration.  (true -> Iterator).
			/// </summary>
			internal bool Iterator;

			/// <summary>
			/// The modCount value that the iterator believes that the backing
			/// Hashtable should have.  If this expectation is violated, the iterator
			/// has detected concurrent modification.
			/// </summary>
			protected internal int ExpectedModCount;

			internal Enumerator(Dictionary<K, V> outerInstance, int type, bool iterator)
			{
				this.OuterInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = Map_Fields.True;
				}
				this.Type = type;
				this.Iterator = iterator;
			}

			public virtual bool HasMoreElements()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> e = entry;
				Entry<?, ?> e = Entry;
				int i = Index;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?>[] t = table;
				Entry<?, ?>[] t = Table;
				/* Use locals for faster loop iteration */
				while (e == Map_Fields.Null && i > 0)
				{
					e = t[--i];
				}
				Entry = e;
				Index = i;
				return e != Map_Fields.Null;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T nextElement()
			public virtual T NextElement()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> et = entry;
				Entry<?, ?> et = Entry;
				int i = Index;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?>[] t = table;
				Entry<?, ?>[] t = Table;
				/* Use locals for faster loop iteration */
				while (et == Map_Fields.Null && i > 0)
				{
					et = t[--i];
				}
				Entry = et;
				Index = i;
				if (et != Map_Fields.Null)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?> e = lastReturned = entry;
					Entry<?, ?> e = LastReturned = Entry;
					Entry = e.Next;
					return Type == KEYS ? (T)e.Key_Renamed : (Type == VALUES ? (T)e.Value_Renamed : (T)e);
				}
				throw new NoSuchElementException("Hashtable Enumerator");
			}

			// Iterator methods
			public virtual bool HasNext()
			{
				return HasMoreElements();
			}

			public virtual T Next()
			{
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				return NextElement();
			}

			public virtual void Remove()
			{
				if (!Iterator)
				{
					throw new UnsupportedOperationException();
				}
				if (LastReturned == Map_Fields.Null)
				{
					throw new IllegalStateException("Hashtable Enumerator");
				}
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}

				lock (OuterInstance)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?,?>[] tab = Hashtable.this.table;
					Entry<?, ?>[] tab = OuterInstance.Table;
					int index = (LastReturned.Hash & 0x7FFFFFFF) % tab.Length;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<K,V> e = (Entry<K,V>)tab[index];
					Entry<K, V> e = (Entry<K, V>)tab[index];
					for (Entry<K, V> prev = Map_Fields.Null; e != Map_Fields.Null; prev = e, e = e.Next)
					{
						if (e == LastReturned)
						{
							outerInstance.ModCount++;
							ExpectedModCount++;
							if (prev == Map_Fields.Null)
							{
								tab[index] = e.Next;
							}
							else
							{
								prev.Next = e.Next;
							}
							outerInstance.Count--;
							LastReturned = Map_Fields.Null;
							return;
						}
					}
					throw new ConcurrentModificationException();
				}
			}
		}
	}

}