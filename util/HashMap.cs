using System;
using System.Diagnostics;

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
	/// Hash table based implementation of the <tt>Map</tt> interface.  This
	/// implementation provides all of the optional map operations, and permits
	/// <tt>null</tt> values and the <tt>null</tt> key.  (The <tt>HashMap</tt>
	/// class is roughly equivalent to <tt>Hashtable</tt>, except that it is
	/// unsynchronized and permits nulls.)  This class makes no guarantees as to
	/// the order of the map; in particular, it does not guarantee that the order
	/// will remain constant over time.
	/// 
	/// <para>This implementation provides constant-time performance for the basic
	/// operations (<tt>get</tt> and <tt>put</tt>), assuming the hash function
	/// disperses the elements properly among the buckets.  Iteration over
	/// collection views requires time proportional to the "capacity" of the
	/// <tt>HashMap</tt> instance (the number of buckets) plus its size (the number
	/// of key-value mappings).  Thus, it's very important not to set the initial
	/// capacity too high (or the load factor too low) if iteration performance is
	/// important.
	/// 
	/// </para>
	/// <para>An instance of <tt>HashMap</tt> has two parameters that affect its
	/// performance: <i>initial capacity</i> and <i>load factor</i>.  The
	/// <i>capacity</i> is the number of buckets in the hash table, and the initial
	/// capacity is simply the capacity at the time the hash table is created.  The
	/// <i>load factor</i> is a measure of how full the hash table is allowed to
	/// get before its capacity is automatically increased.  When the number of
	/// entries in the hash table exceeds the product of the load factor and the
	/// current capacity, the hash table is <i>rehashed</i> (that is, internal data
	/// structures are rebuilt) so that the hash table has approximately twice the
	/// number of buckets.
	/// 
	/// </para>
	/// <para>As a general rule, the default load factor (.75) offers a good
	/// tradeoff between time and space costs.  Higher values decrease the
	/// space overhead but increase the lookup cost (reflected in most of
	/// the operations of the <tt>HashMap</tt> class, including
	/// <tt>get</tt> and <tt>put</tt>).  The expected number of entries in
	/// the map and its load factor should be taken into account when
	/// setting its initial capacity, so as to minimize the number of
	/// rehash operations.  If the initial capacity is greater than the
	/// maximum number of entries divided by the load factor, no rehash
	/// operations will ever occur.
	/// 
	/// </para>
	/// <para>If many mappings are to be stored in a <tt>HashMap</tt>
	/// instance, creating it with a sufficiently large capacity will allow
	/// the mappings to be stored more efficiently than letting it perform
	/// automatic rehashing as needed to grow the table.  Note that using
	/// many keys with the same {@code hashCode()} is a sure way to slow
	/// down performance of any hash table. To ameliorate impact, when keys
	/// are <seealso cref="Comparable"/>, this class may use comparison order among
	/// keys to help break ties.
	/// 
	/// </para>
	/// <para><strong>Note that this implementation is not synchronized.</strong>
	/// If multiple threads access a hash map concurrently, and at least one of
	/// the threads modifies the map structurally, it <i>must</i> be
	/// synchronized externally.  (A structural modification is any operation
	/// that adds or deletes one or more mappings; merely changing the value
	/// associated with a key that an instance already contains is not a
	/// structural modification.)  This is typically accomplished by
	/// synchronizing on some object that naturally encapsulates the map.
	/// 
	/// If no such object exists, the map should be "wrapped" using the
	/// <seealso cref="Collections#synchronizedMap Collections.synchronizedMap"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access to the map:<pre>
	///   Map m = Collections.synchronizedMap(new HashMap(...));</pre>
	/// 
	/// </para>
	/// <para>The iterators returned by all of this class's "collection view methods"
	/// are <i>fail-fast</i>: if the map is structurally modified at any time after
	/// the iterator is created, in any way except through the iterator's own
	/// <tt>remove</tt> method, the iterator will throw a
	/// <seealso cref="ConcurrentModificationException"/>.  Thus, in the face of concurrent
	/// modification, the iterator fails quickly and cleanly, rather than risking
	/// arbitrary, non-deterministic behavior at an undetermined time in the
	/// future.
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
	/// @param <K> the type of keys maintained by this map </param>
	/// @param <V> the type of mapped values
	/// 
	/// @author  Doug Lea
	/// @author  Josh Bloch
	/// @author  Arthur van Hoff
	/// @author  Neal Gafter </param>
	/// <seealso cref=     Object#hashCode() </seealso>
	/// <seealso cref=     Collection </seealso>
	/// <seealso cref=     Map </seealso>
	/// <seealso cref=     TreeMap </seealso>
	/// <seealso cref=     Hashtable
	/// @since   1.2 </seealso>
	[Serializable]
	public class HashMap<K, V> : AbstractMap<K, V>, Map<K, V>, Cloneable
	{

		private const long SerialVersionUID = 362498820763181265L;

		/*
		 * Implementation notes.
		 *
		 * This map usually acts as a binned (bucketed) hash table, but
		 * when bins get too large, they are transformed into bins of
		 * TreeNodes, each structured similarly to those in
		 * java.util.TreeMap. Most methods try to use normal bins, but
		 * relay to TreeNode methods when applicable (simply by checking
		 * instanceof a node).  Bins of TreeNodes may be traversed and
		 * used like any others, but additionally support faster lookup
		 * when overpopulated. However, since the vast majority of bins in
		 * normal use are not overpopulated, checking for existence of
		 * tree bins may be delayed in the course of table methods.
		 *
		 * Tree bins (i.e., bins whose elements are all TreeNodes) are
		 * ordered primarily by hashCode, but in the case of ties, if two
		 * elements are of the same "class C implements Comparable<C>",
		 * type then their compareTo method is used for ordering. (We
		 * conservatively check generic types via reflection to validate
		 * this -- see method comparableClassFor).  The added complexity
		 * of tree bins is worthwhile in providing worst-case O(log n)
		 * operations when keys either have distinct hashes or are
		 * orderable, Thus, performance degrades gracefully under
		 * accidental or malicious usages in which hashCode() methods
		 * return values that are poorly distributed, as well as those in
		 * which many keys share a hashCode, so long as they are also
		 * Comparable. (If neither of these apply, we may waste about a
		 * factor of two in time and space compared to taking no
		 * precautions. But the only known cases stem from poor user
		 * programming practices that are already so slow that this makes
		 * little difference.)
		 *
		 * Because TreeNodes are about twice the size of regular nodes, we
		 * use them only when bins contain enough nodes to warrant use
		 * (see TREEIFY_THRESHOLD). And when they become too small (due to
		 * removal or resizing) they are converted back to plain bins.  In
		 * usages with well-distributed user hashCodes, tree bins are
		 * rarely used.  Ideally, under random hashCodes, the frequency of
		 * nodes in bins follows a Poisson distribution
		 * (http://en.wikipedia.org/wiki/Poisson_distribution) with a
		 * parameter of about 0.5 on average for the default resizing
		 * threshold of 0.75, although with a large variance because of
		 * resizing granularity. Ignoring variance, the expected
		 * occurrences of list size k are (exp(-0.5) * pow(0.5, k) /
		 * factorial(k)). The first values are:
		 *
		 * 0:    0.60653066
		 * 1:    0.30326533
		 * 2:    0.07581633
		 * 3:    0.01263606
		 * 4:    0.00157952
		 * 5:    0.00015795
		 * 6:    0.00001316
		 * 7:    0.00000094
		 * 8:    0.00000006
		 * more: less than 1 in ten million
		 *
		 * The root of a tree bin is normally its first node.  However,
		 * sometimes (currently only upon Iterator.remove), the root might
		 * be elsewhere, but can be recovered following parent links
		 * (method TreeNode.root()).
		 *
		 * All applicable internal methods accept a hash code as an
		 * argument (as normally supplied from a public method), allowing
		 * them to call each other without recomputing user hashCodes.
		 * Most internal methods also accept a "tab" argument, that is
		 * normally the current table, but may be a new or old one when
		 * resizing or converting.
		 *
		 * When bin lists are treeified, split, or untreeified, we keep
		 * them in the same relative access/traversal order (i.e., field
		 * Node.next) to better preserve locality, and to slightly
		 * simplify handling of splits and traversals that invoke
		 * iterator.remove. When using comparators on insertion, to keep a
		 * total ordering (or as close as is required here) across
		 * rebalancings, we compare classes and identityHashCodes as
		 * tie-breakers.
		 *
		 * The use and transitions among plain vs tree modes is
		 * complicated by the existence of subclass LinkedHashMap. See
		 * below for hook methods defined to be invoked upon insertion,
		 * removal and access that allow LinkedHashMap internals to
		 * otherwise remain independent of these mechanics. (This also
		 * requires that a map instance be passed to some utility methods
		 * that may create new nodes.)
		 *
		 * The concurrent-programming-like SSA-based coding style helps
		 * avoid aliasing errors amid all of the twisty pointer operations.
		 */

		/// <summary>
		/// The default initial capacity - MUST be a power of two.
		/// </summary>
		internal static readonly int DEFAULT_INITIAL_CAPACITY = 1 << 4; // aka 16

		/// <summary>
		/// The maximum capacity, used if a higher value is implicitly specified
		/// by either of the constructors with arguments.
		/// MUST be a power of two <= 1<<30.
		/// </summary>
		internal static readonly int MAXIMUM_CAPACITY = 1 << 30;

		/// <summary>
		/// The load factor used when none specified in constructor.
		/// </summary>
		internal const float DEFAULT_LOAD_FACTOR = 0.75f;

		/// <summary>
		/// The bin count threshold for using a tree rather than list for a
		/// bin.  Bins are converted to trees when adding an element to a
		/// bin with at least this many nodes. The value must be greater
		/// than 2 and should be at least 8 to mesh with assumptions in
		/// tree removal about conversion back to plain bins upon
		/// shrinkage.
		/// </summary>
		internal const int TREEIFY_THRESHOLD = 8;

		/// <summary>
		/// The bin count threshold for untreeifying a (split) bin during a
		/// resize operation. Should be less than TREEIFY_THRESHOLD, and at
		/// most 6 to mesh with shrinkage detection under removal.
		/// </summary>
		internal const int UNTREEIFY_THRESHOLD = 6;

		/// <summary>
		/// The smallest table capacity for which bins may be treeified.
		/// (Otherwise the table is resized if too many nodes in a bin.)
		/// Should be at least 4 * TREEIFY_THRESHOLD to avoid conflicts
		/// between resizing and treeification thresholds.
		/// </summary>
		internal const int MIN_TREEIFY_CAPACITY = 64;

		/// <summary>
		/// Basic hash bin node, used for most entries.  (See below for
		/// TreeNode subclass, and in LinkedHashMap for its Entry subclass.)
		/// </summary>
		internal class Node<K, V> : Map_Entry<K, V>
		{
			internal readonly int Hash;
			internal readonly K Key_Renamed;
			internal V Value_Renamed;
			internal Node<K, V> Next;

			internal Node(int hash, K key, V value, Node<K, V> next)
			{
				this.Hash = hash;
				this.Key_Renamed = key;
				this.Value_Renamed = value;
				this.Next = next;
			}

			public K Key
			{
				get
				{
					return Key_Renamed;
				}
			}
			public V Value
			{
				get
				{
					return Value_Renamed;
				}
			}
			public sealed override String ToString()
			{
				return Key_Renamed + "=" + Value_Renamed;
			}

			public sealed override int HashCode()
			{
				return Objects.HashCode(Key_Renamed) ^ Objects.HashCode(Value_Renamed);
			}

			public V SetValue(V Map_Fields)
			{
				V Map_Fields.OldValue = Value_Renamed;
				Value_Renamed = Map_Fields.NewValue;
				return Map_Fields.OldValue;
			}

			public sealed override bool Equals(Object o)
			{
				if (o == this)
				{
					return Map_Fields.True;
				}
				if (o is Map_Entry)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>)o;
					Map_Entry<?, ?> e = (Map_Entry<?, ?>)o;
					if (Objects.Equals(Key_Renamed, e.Key) && Objects.Equals(Value_Renamed, e.Value))
					{
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}
		}

		/* ---------------- Static utilities -------------- */

		/// <summary>
		/// Computes key.hashCode() and spreads (XORs) higher bits of hash
		/// to lower.  Because the table uses power-of-two masking, sets of
		/// hashes that vary only in bits above the current mask will
		/// always collide. (Among known examples are sets of Float keys
		/// holding consecutive whole numbers in small tables.)  So we
		/// apply a transform that spreads the impact of higher bits
		/// downward. There is a tradeoff between speed, utility, and
		/// quality of bit-spreading. Because many common sets of hashes
		/// are already reasonably distributed (so don't benefit from
		/// spreading), and because we use trees to handle large sets of
		/// collisions in bins, we just XOR some shifted bits in the
		/// cheapest possible way to reduce systematic lossage, as well as
		/// to incorporate impact of the highest bits that would otherwise
		/// never be used in index calculations because of table bounds.
		/// </summary>
		internal static int Hash(Object key)
		{
			int h;
			return (key == Map_Fields.Null) ? 0 : (h = key.HashCode()) ^ ((int)((uint)h >> 16));
		}

		/// <summary>
		/// Returns x's Class if it is of the form "class C implements
		/// Comparable<C>", else null.
		/// </summary>
		internal static Class ComparableClassFor(Object x)
		{
			if (x is Comparable)
			{
				Class c;
				Type[] ts, @as;
				Type t;
				ParameterizedType p;
				if ((c = x.GetType()) == typeof(String)) // bypass checks
				{
					return c;
				}
				if ((ts = c.GenericInterfaces) != Map_Fields.Null)
				{
					for (int i = 0; i < ts.Length; ++i)
					{
						if (((t = ts[i]) is ParameterizedType) && ((p = (ParameterizedType)t).RawType == typeof(Comparable)) && (@as = p.ActualTypeArguments) != Map_Fields.Null && @as.Length == 1 && @as[0] == c) // type arg is c
						{
							return c;
						}
					}
				}
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Returns k.compareTo(x) if x matches kc (k's screened comparable
		/// class), else 0.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes","unchecked"}) static int compareComparables(Class kc, Object Map_Fields.k, Object x)
		internal static int CompareComparables(Class kc, Object Map_Fields, Object x) // for cast to Comparable
		{
			return (x == Map_Fields.Null || x.GetType() != kc ? 0 : ((Comparable)Map_Fields.k).CompareTo(x));
		}

		/// <summary>
		/// Returns a power of two size for the given target capacity.
		/// </summary>
		internal static int TableSizeFor(int cap)
		{
			int n = cap - 1;
			n |= (int)((uint)n >> 1);
			n |= (int)((uint)n >> 2);
			n |= (int)((uint)n >> 4);
			n |= (int)((uint)n >> 8);
			n |= (int)((uint)n >> 16);
			return (n < 0) ? 1 : (n >= MAXIMUM_CAPACITY) ? MAXIMUM_CAPACITY : n + 1;
		}

		/* ---------------- Fields -------------- */

		/// <summary>
		/// The table, initialized on first use, and resized as
		/// necessary. When allocated, length is always a power of two.
		/// (We also tolerate length zero in some operations to allow
		/// bootstrapping mechanics that are currently not needed.)
		/// </summary>
		[NonSerialized]
		internal Node<K, V>[] Table;

		/// <summary>
		/// Holds cached entrySet(). Note that AbstractMap fields are used
		/// for keySet() and values().
		/// </summary>
		[NonSerialized]
		internal Set<Map_Entry<K, V>> EntrySet_Renamed;

		/// <summary>
		/// The number of key-value mappings contained in this map.
		/// </summary>
		[NonSerialized]
		internal int Size_Renamed;

		/// <summary>
		/// The number of times this HashMap has been structurally modified
		/// Structural modifications are those that change the number of mappings in
		/// the HashMap or otherwise modify its internal structure (e.g.,
		/// rehash).  This field is used to make iterators on Collection-views of
		/// the HashMap fail-fast.  (See ConcurrentModificationException).
		/// </summary>
		[NonSerialized]
		internal int ModCount;

		/// <summary>
		/// The next size value at which to resize (capacity * load factor).
		/// 
		/// @serial
		/// </summary>
		// (The javadoc description is true upon serialization.
		// Additionally, if the table array has not been allocated, this
		// field holds the initial array capacity, or zero signifying
		// DEFAULT_INITIAL_CAPACITY.)
		internal int Threshold;

		/// <summary>
		/// The load factor for the hash table.
		/// 
		/// @serial
		/// </summary>
		internal readonly float LoadFactor_Renamed;

		/* ---------------- Public operations -------------- */

		/// <summary>
		/// Constructs an empty <tt>HashMap</tt> with the specified initial
		/// capacity and load factor.
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity </param>
		/// <param name="loadFactor">      the load factor </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is negative
		///         or the load factor is nonpositive </exception>
		public HashMap(int initialCapacity, float loadFactor)
		{
			if (initialCapacity < 0)
			{
				throw new IllegalArgumentException("Illegal initial capacity: " + initialCapacity);
			}
			if (initialCapacity > MAXIMUM_CAPACITY)
			{
				initialCapacity = MAXIMUM_CAPACITY;
			}
			if (loadFactor <= 0 || Float.IsNaN(loadFactor))
			{
				throw new IllegalArgumentException("Illegal load factor: " + loadFactor);
			}
			this.LoadFactor_Renamed = loadFactor;
			this.Threshold = TableSizeFor(initialCapacity);
		}

		/// <summary>
		/// Constructs an empty <tt>HashMap</tt> with the specified initial
		/// capacity and the default load factor (0.75).
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity. </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is negative. </exception>
		public HashMap(int initialCapacity) : this(initialCapacity, DEFAULT_LOAD_FACTOR)
		{
		}

		/// <summary>
		/// Constructs an empty <tt>HashMap</tt> with the default initial capacity
		/// (16) and the default load factor (0.75).
		/// </summary>
		public HashMap()
		{
			this.LoadFactor_Renamed = DEFAULT_LOAD_FACTOR; // all other fields defaulted
		}

		/// <summary>
		/// Constructs a new <tt>HashMap</tt> with the same mappings as the
		/// specified <tt>Map</tt>.  The <tt>HashMap</tt> is created with
		/// default load factor (0.75) and an initial capacity sufficient to
		/// hold the mappings in the specified <tt>Map</tt>.
		/// </summary>
		/// <param name="m"> the map whose mappings are to be placed in this map </param>
		/// <exception cref="NullPointerException"> if the specified map is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public HashMap(Map<? extends K, ? extends V> m)
		public HashMap<T1>(Map<T1> m) where T1 : K where ? : V
		{
			this.LoadFactor_Renamed = DEFAULT_LOAD_FACTOR;
			PutMapEntries(m, Map_Fields.False);
		}

		/// <summary>
		/// Implements Map.putAll and Map constructor
		/// </summary>
		/// <param name="m"> the map </param>
		/// <param name="evict"> false when initially constructing this map, else
		/// true (relayed to method afterNodeInsertion). </param>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final void putMapEntries(Map<? extends K, ? extends V> m, boolean evict)
		internal void putMapEntries<T1>(Map<T1> m, bool evict) where T1 : K where ? : V
		{
			int s = m.Size();
			if (s > 0)
			{
				if (Table == Map_Fields.Null) // pre-size
				{
					float ft = ((float)s / LoadFactor_Renamed) + 1.0F;
					int t = ((ft < (float)MAXIMUM_CAPACITY) ? (int)ft : MAXIMUM_CAPACITY);
					if (t > Threshold)
					{
						Threshold = TableSizeFor(t);
					}
				}
				else if (s > Threshold)
				{
					Resize();
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Map_Entry<? extends K, ? extends V> e : m.entrySet())
				foreach (Map_Entry<?, ?> e in m.EntrySet())
				{
					K key = e.Key;
					V value = e.Value;
					PutVal(Hash(key), key, value, Map_Fields.False, evict);
				}
			}
		}

		/// <summary>
		/// Returns the number of key-value mappings in this map.
		/// </summary>
		/// <returns> the number of key-value mappings in this map </returns>
		public virtual int Size()
		{
			return Size_Renamed;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this map contains no key-value mappings.
		/// </summary>
		/// <returns> <tt>true</tt> if this map contains no key-value mappings </returns>
		public virtual bool Empty
		{
			get
			{
				return Size_Renamed == 0;
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
			Node<K, V> e;
			return (e = GetNode(Hash(key), key)) == Map_Fields.Null ? Map_Fields.Null : e.Value_Renamed;
		}

		/// <summary>
		/// Implements Map.get and related methods
		/// </summary>
		/// <param name="hash"> hash for key </param>
		/// <param name="key"> the key </param>
		/// <returns> the node, or null if none </returns>
		internal Node<K, V> GetNode(int hash, Object key)
		{
			Node<K, V>[] tab;
			Node<K, V> first, e;
			int n;
			K Map_Fields.k;
			if ((tab = Table) != Map_Fields.Null && (n = tab.Length) > 0 && (first = tab[(n - 1) & hash]) != Map_Fields.Null)
			{
				if (first.Hash == hash && ((Map_Fields.k = first.Key_Renamed) == key || (key != Map_Fields.Null && key.Equals(Map_Fields.k)))) // always check first node
				{
					return first;
				}
				if ((e = first.Next) != Map_Fields.Null)
				{
					if (first is TreeNode)
					{
						return ((TreeNode<K, V>)first).GetTreeNode(hash, key);
					}
					do
					{
						if (e.Hash == hash && ((Map_Fields.k = e.Key_Renamed) == key || (key != Map_Fields.Null && key.Equals(Map_Fields.k))))
						{
							return e;
						}
					} while ((e = e.Next) != Map_Fields.Null);
				}
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this map contains a mapping for the
		/// specified key.
		/// </summary>
		/// <param name="key">   The key whose presence in this map is to be tested </param>
		/// <returns> <tt>true</tt> if this map contains a mapping for the specified
		/// key. </returns>
		public virtual bool ContainsKey(Object key)
		{
			return GetNode(Hash(key), key) != Map_Fields.Null;
		}

		/// <summary>
		/// Associates the specified value with the specified key in this map.
		/// If the map previously contained a mapping for the key, the old
		/// value is replaced.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="value"> value to be associated with the specified key </param>
		/// <returns> the previous value associated with <tt>key</tt>, or
		///         <tt>null</tt> if there was no mapping for <tt>key</tt>.
		///         (A <tt>null</tt> return can also indicate that the map
		///         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
		public virtual V Put(K key, V value)
		{
			return PutVal(Hash(key), key, value, Map_Fields.False, Map_Fields.True);
		}

		/// <summary>
		/// Implements Map.put and related methods
		/// </summary>
		/// <param name="hash"> hash for key </param>
		/// <param name="key"> the key </param>
		/// <param name="value"> the value to put </param>
		/// <param name="onlyIfAbsent"> if true, don't change existing value </param>
		/// <param name="evict"> if false, the table is in creation mode. </param>
		/// <returns> previous value, or null if none </returns>
		internal V PutVal(int hash, K key, V value, bool onlyIfAbsent, bool evict)
		{
			Node<K, V>[] tab;
			Node<K, V> p;
			int n, i;
			if ((tab = Table) == Map_Fields.Null || (n = tab.Length) == 0)
			{
				n = (tab = Resize()).Length;
			}
			if ((p = tab[i = (n - 1) & hash]) == Map_Fields.Null)
			{
				tab[i] = NewNode(hash, key, value, Map_Fields.Null);
			}
			else
			{
				Node<K, V> e;
				K Map_Fields.k;
				if (p.Hash == hash && ((Map_Fields.k = p.Key_Renamed) == key || (key != Map_Fields.Null && key.Equals(Map_Fields.k))))
				{
					e = p;
				}
				else if (p is TreeNode)
				{
					e = ((TreeNode<K, V>)p).PutTreeVal(this, tab, hash, key, value);
				}
				else
				{
					for (int binCount = 0; ; ++binCount)
					{
						if ((e = p.Next) == Map_Fields.Null)
						{
							p.Next = NewNode(hash, key, value, Map_Fields.Null);
							if (binCount >= TREEIFY_THRESHOLD - 1) // -1 for 1st
							{
								TreeifyBin(tab, hash);
							}
							break;
						}
						if (e.Hash == hash && ((Map_Fields.k = e.Key_Renamed) == key || (key != Map_Fields.Null && key.Equals(Map_Fields.k))))
						{
							break;
						}
						p = e;
					}
				}
				if (e != Map_Fields.Null) // existing mapping for key
				{
					V Map_Fields.OldValue = e.Value_Renamed;
					if (!onlyIfAbsent || Map_Fields.OldValue == Map_Fields.Null)
					{
						e.Value_Renamed = value;
					}
					AfterNodeAccess(e);
					return Map_Fields.OldValue;
				}
			}
			++ModCount;
			if (++Size_Renamed > Threshold)
			{
				Resize();
			}
			AfterNodeInsertion(evict);
			return Map_Fields.Null;
		}

		/// <summary>
		/// Initializes or doubles table size.  If null, allocates in
		/// accord with initial capacity target held in field threshold.
		/// Otherwise, because we are using power-of-two expansion, the
		/// elements from each bin must either stay at same index, or move
		/// with a power of two offset in the new table.
		/// </summary>
		/// <returns> the table </returns>
		internal Node<K, V>[] Resize()
		{
			Node<K, V>[] oldTab = Table;
			int oldCap = (oldTab == Map_Fields.Null) ? 0 : oldTab.Length;
			int oldThr = Threshold;
			int newCap , newThr = 0;
			if (oldCap > 0)
			{
				if (oldCap >= MAXIMUM_CAPACITY)
				{
					Threshold = Integer.MaxValue;
					return oldTab;
				}
				else if ((newCap = oldCap << 1) < MAXIMUM_CAPACITY && oldCap >= DEFAULT_INITIAL_CAPACITY)
				{
					newThr = oldThr << 1; // double threshold
				}
			}
			else if (oldThr > 0) // initial capacity was placed in threshold
			{
				newCap = oldThr;
			}
			else // zero initial threshold signifies using defaults
			{
				newCap = DEFAULT_INITIAL_CAPACITY;
				newThr = (int)(DEFAULT_LOAD_FACTOR * DEFAULT_INITIAL_CAPACITY);
			}
			if (newThr == 0)
			{
				float ft = (float)newCap * LoadFactor_Renamed;
				newThr = (newCap < MAXIMUM_CAPACITY && ft < (float)MAXIMUM_CAPACITY ? (int)ft : Integer.MaxValue);
			}
			Threshold = newThr;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes","unchecked"}) Node<K,V>[] newTab = (Node<K,V>[])new Node[newCap];
			Node<K, V>[] newTab = (Node<K, V>[])new Node[newCap];
			Table = newTab;
			if (oldTab != Map_Fields.Null)
			{
				for (int j = 0; j < oldCap; ++j)
				{
					Node<K, V> e;
					if ((e = oldTab[j]) != Map_Fields.Null)
					{
						oldTab[j] = Map_Fields.Null;
						if (e.Next == Map_Fields.Null)
						{
							newTab[e.Hash & (newCap - 1)] = e;
						}
						else if (e is TreeNode)
						{
							((TreeNode<K, V>)e).Split(this, newTab, j, oldCap);
						}
						else // preserve order
						{
							Node<K, V> loHead = Map_Fields.Null, loTail = Map_Fields.Null;
							Node<K, V> hiHead = Map_Fields.Null, hiTail = Map_Fields.Null;
							Node<K, V> next;
							do
							{
								next = e.Next;
								if ((e.Hash & oldCap) == 0)
								{
									if (loTail == Map_Fields.Null)
									{
										loHead = e;
									}
									else
									{
										loTail.Next = e;
									}
									loTail = e;
								}
								else
								{
									if (hiTail == Map_Fields.Null)
									{
										hiHead = e;
									}
									else
									{
										hiTail.Next = e;
									}
									hiTail = e;
								}
							} while ((e = next) != Map_Fields.Null);
							if (loTail != Map_Fields.Null)
							{
								loTail.Next = Map_Fields.Null;
								newTab[j] = loHead;
							}
							if (hiTail != Map_Fields.Null)
							{
								hiTail.Next = Map_Fields.Null;
								newTab[j + oldCap] = hiHead;
							}
						}
					}
				}
			}
			return newTab;
		}

		/// <summary>
		/// Replaces all linked nodes in bin at index for given hash unless
		/// table is too small, in which case resizes instead.
		/// </summary>
		internal void TreeifyBin(Node<K, V>[] tab, int hash)
		{
			int n, index;
			Node<K, V> e;
			if (tab == Map_Fields.Null || (n = tab.Length) < MIN_TREEIFY_CAPACITY)
			{
				Resize();
			}
			else if ((e = tab[index = (n - 1) & hash]) != Map_Fields.Null)
			{
				TreeNode<K, V> hd = Map_Fields.Null, tl = Map_Fields.Null;
				do
				{
					TreeNode<K, V> p = ReplacementTreeNode(e, Map_Fields.Null);
					if (tl == Map_Fields.Null)
					{
						hd = p;
					}
					else
					{
						p.Prev = tl;
						tl.Next = p;
					}
					tl = p;
				} while ((e = e.Next) != Map_Fields.Null);
				if ((tab[index] = hd) != Map_Fields.Null)
				{
					hd.Treeify(tab);
				}
			}
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
			PutMapEntries(m, Map_Fields.True);
		}

		/// <summary>
		/// Removes the mapping for the specified key from this map if present.
		/// </summary>
		/// <param name="key"> key whose mapping is to be removed from the map </param>
		/// <returns> the previous value associated with <tt>key</tt>, or
		///         <tt>null</tt> if there was no mapping for <tt>key</tt>.
		///         (A <tt>null</tt> return can also indicate that the map
		///         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
		public virtual V Remove(Object key)
		{
			Node<K, V> e;
			return (e = RemoveNode(Hash(key), key, Map_Fields.Null, Map_Fields.False, Map_Fields.True)) == Map_Fields.Null ? Map_Fields.Null : e.Value_Renamed;
		}

		/// <summary>
		/// Implements Map.remove and related methods
		/// </summary>
		/// <param name="hash"> hash for key </param>
		/// <param name="key"> the key </param>
		/// <param name="value"> the value to match if matchValue, else ignored </param>
		/// <param name="matchValue"> if true only remove if value is equal </param>
		/// <param name="movable"> if false do not move other nodes while removing </param>
		/// <returns> the node, or null if none </returns>
		internal Node<K, V> RemoveNode(int hash, Object key, Object value, bool matchValue, bool movable)
		{
			Node<K, V>[] tab;
			Node<K, V> p;
			int n, index;
			if ((tab = Table) != Map_Fields.Null && (n = tab.Length) > 0 && (p = tab[index = (n - 1) & hash]) != Map_Fields.Null)
			{
				Node<K, V> node = Map_Fields.Null, e ;
				K Map_Fields.k;
				V Map_Fields.v;
				if (p.Hash == hash && ((Map_Fields.k = p.Key_Renamed) == key || (key != Map_Fields.Null && key.Equals(Map_Fields.k))))
				{
					node = p;
				}
				else if ((e = p.Next) != Map_Fields.Null)
				{
					if (p is TreeNode)
					{
						node = ((TreeNode<K, V>)p).GetTreeNode(hash, key);
					}
					else
					{
						do
						{
							if (e.Hash == hash && ((Map_Fields.k = e.Key_Renamed) == key || (key != Map_Fields.Null && key.Equals(Map_Fields.k))))
							{
								node = e;
								break;
							}
							p = e;
						} while ((e = e.Next) != Map_Fields.Null);
					}
				}
				if (node != Map_Fields.Null && (!matchValue || (Map_Fields.v = node.Value_Renamed) == value || (value != Map_Fields.Null && value.Equals(Map_Fields.v))))
				{
					if (node is TreeNode)
					{
						((TreeNode<K, V>)node).RemoveTreeNode(this, tab, movable);
					}
					else if (node == p)
					{
						tab[index] = node.Next;
					}
					else
					{
						p.Next = node.Next;
					}
					++ModCount;
					--Size_Renamed;
					AfterNodeRemoval(node);
					return node;
				}
			}
			return Map_Fields.Null;
		}

		/// <summary>
		/// Removes all of the mappings from this map.
		/// The map will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			Node<K, V>[] tab;
			ModCount++;
			if ((tab = Table) != Map_Fields.Null && Size_Renamed > 0)
			{
				Size_Renamed = 0;
				for (int i = 0; i < tab.Length; ++i)
				{
					tab[i] = Map_Fields.Null;
				}
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
			Node<K, V>[] tab;
			V Map_Fields.v;
			if ((tab = Table) != Map_Fields.Null && Size_Renamed > 0)
			{
				for (int i = 0; i < tab.Length; ++i)
				{
					for (Node<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
					{
						if ((Map_Fields.v = e.Value_Renamed) == value || (value != Map_Fields.Null && value.Equals(Map_Fields.v)))
						{
							return Map_Fields.True;
						}
					}
				}
			}
			return Map_Fields.False;
		}

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
		/// <returns> a set view of the keys contained in this map </returns>
		public virtual Set<K> KeySet()
		{
			Set<K> ks;
			return (ks = KeySet_Renamed) == Map_Fields.Null ? (KeySet_Renamed = new KeySet(this)) : ks;
		}

		internal sealed class KeySet : AbstractSet<K>
		{
			private readonly HashMap<K, V> OuterInstance;

			public KeySet(HashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public int Size()
			{
				return outerInstance.Size_Renamed;
			}
			public void Clear()
			{
				OuterInstance.Clear();
			}
			public Iterator<K> Iterator()
			{
				return new KeyIterator(OuterInstance);
			}
			public bool Contains(Object o)
			{
				return outerInstance.ContainsKey(o);
			}
			public bool Remove(Object key)
			{
				return outerInstance.RemoveNode(Hash(key), key, Map_Fields.Null, Map_Fields.False, Map_Fields.True) != Map_Fields.Null;
			}
			public Spliterator<K> Spliterator()
			{
				return new KeySpliterator<>(OuterInstance, 0, -1, 0, 0);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final void forEach(java.util.function.Consumer<? base K> action)
			public void forEach<T1>(Consumer<T1> action)
			{
				Node<K, V>[] tab;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (outerInstance.Size_Renamed > 0 && (tab = outerInstance.Table) != Map_Fields.Null)
				{
					int mc = outerInstance.ModCount;
					for (int i = 0; i < tab.Length; ++i)
					{
						for (Node<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
						{
							action.Accept(e.Key_Renamed);
						}
					}
					if (outerInstance.ModCount != mc)
					{
						throw new ConcurrentModificationException();
					}
				}
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
		/// <returns> a view of the values contained in this map </returns>
		public virtual Collection<V> Values()
		{
			Collection<V> vs;
			return (vs = Values_Renamed) == Map_Fields.Null ? (Values_Renamed = new Values(this)) : vs;
		}

		internal sealed class Values : AbstractCollection<V>
		{
			private readonly HashMap<K, V> OuterInstance;

			public Values(HashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public int Size()
			{
				return outerInstance.Size_Renamed;
			}
			public void Clear()
			{
				OuterInstance.Clear();
			}
			public Iterator<V> Iterator()
			{
				return new ValueIterator(OuterInstance);
			}
			public bool Contains(Object o)
			{
				return outerInstance.ContainsValue(o);
			}
			public Spliterator<V> Spliterator()
			{
				return new ValueSpliterator<>(OuterInstance, 0, -1, 0, 0);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final void forEach(java.util.function.Consumer<? base V> action)
			public void forEach<T1>(Consumer<T1> action)
			{
				Node<K, V>[] tab;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (outerInstance.Size_Renamed > 0 && (tab = outerInstance.Table) != Map_Fields.Null)
				{
					int mc = outerInstance.ModCount;
					for (int i = 0; i < tab.Length; ++i)
					{
						for (Node<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
						{
							action.Accept(e.Value_Renamed);
						}
					}
					if (outerInstance.ModCount != mc)
					{
						throw new ConcurrentModificationException();
					}
				}
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
		/// <returns> a set view of the mappings contained in this map </returns>
		public virtual Set<Map_Entry<K, V>> EntrySet()
		{
			Set<Map_Entry<K, V>> es;
			return (es = EntrySet_Renamed) == Map_Fields.Null ? (EntrySet_Renamed = new EntrySet(this)) : es;
		}

		internal sealed class EntrySet : AbstractSet<Map_Entry<K, V>>
		{
			private readonly HashMap<K, V> OuterInstance;

			public EntrySet(HashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public int Size()
			{
				return outerInstance.Size_Renamed;
			}
			public void Clear()
			{
				OuterInstance.Clear();
			}
			public Iterator<Map_Entry<K, V>> Iterator()
			{
				return new EntryIterator(OuterInstance);
			}
			public bool Contains(Object o)
			{
				if (!(o is Map_Entry))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>) o;
				Map_Entry<?, ?> e = (Map_Entry<?, ?>) o;
				Object key = e.Key;
				Node<K, V> candidate = outerInstance.GetNode(Hash(key), key);
				return candidate != Map_Fields.Null && candidate.Equals(e);
			}
			public bool Remove(Object o)
			{
				if (o is Map_Entry)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>) o;
					Map_Entry<?, ?> e = (Map_Entry<?, ?>) o;
					Object key = e.Key;
					Object value = e.Value;
					return outerInstance.RemoveNode(Hash(key), key, value, Map_Fields.True, Map_Fields.True) != Map_Fields.Null;
				}
				return Map_Fields.False;
			}
			public Spliterator<Map_Entry<K, V>> Spliterator()
			{
				return new EntrySpliterator<>(OuterInstance, 0, -1, 0, 0);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final void forEach(java.util.function.Consumer<? base Map_Entry<K,V>> action)
			public void forEach<T1>(Consumer<T1> action)
			{
				Node<K, V>[] tab;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (outerInstance.Size_Renamed > 0 && (tab = outerInstance.Table) != Map_Fields.Null)
				{
					int mc = outerInstance.ModCount;
					for (int i = 0; i < tab.Length; ++i)
					{
						for (Node<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
						{
							action.Accept(e);
						}
					}
					if (outerInstance.ModCount != mc)
					{
						throw new ConcurrentModificationException();
					}
				}
			}
		}

		// Overrides of JDK8 Map extension methods

		public override V GetOrDefault(Object key, V defaultValue)
		{
			Node<K, V> e;
			return (e = GetNode(Hash(key), key)) == Map_Fields.Null ? defaultValue : e.Value_Renamed;
		}

		public override V PutIfAbsent(K key, V value)
		{
			return PutVal(Hash(key), key, value, Map_Fields.True, Map_Fields.True);
		}

		public override bool Remove(Object key, Object value)
		{
			return RemoveNode(Hash(key), key, value, Map_Fields.True, Map_Fields.True) != Map_Fields.Null;
		}

		public override bool Replace(K key, V Map_Fields, V Map_Fields)
		{
			Node<K, V> e;
			V Map_Fields.v;
			if ((e = GetNode(Hash(key), key)) != Map_Fields.Null && ((Map_Fields.v = e.Value_Renamed) == Map_Fields.OldValue || (Map_Fields.v != Map_Fields.Null && Map_Fields.v.Equals(Map_Fields.OldValue))))
			{
				e.Value_Renamed = Map_Fields.NewValue;
				AfterNodeAccess(e);
				return Map_Fields.True;
			}
			return Map_Fields.False;
		}

		public override V Replace(K key, V value)
		{
			Node<K, V> e;
			if ((e = GetNode(Hash(key), key)) != Map_Fields.Null)
			{
				V Map_Fields.OldValue = e.Value_Renamed;
				e.Value_Renamed = value;
				AfterNodeAccess(e);
				return Map_Fields.OldValue;
			}
			return Map_Fields.Null;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
		public override V computeIfAbsent<T1>(K key, Function<T1> mappingFunction) where T1 : V
		{
			if (mappingFunction == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int hash = Hash(key);
			Node<K, V>[] tab;
			Node<K, V> first;
			int n, i;
			int binCount = 0;
			TreeNode<K, V> t = Map_Fields.Null;
			Node<K, V> old = Map_Fields.Null;
			if (Size_Renamed > Threshold || (tab = Table) == Map_Fields.Null || (n = tab.Length) == 0)
			{
				n = (tab = Resize()).Length;
			}
			if ((first = tab[i = (n - 1) & hash]) != Map_Fields.Null)
			{
				if (first is TreeNode)
				{
					old = (t = (TreeNode<K, V>)first).getTreeNode(hash, key);
				}
				else
				{
					Node<K, V> e = first;
					K Map_Fields.k;
					do
					{
						if (e.Hash == hash && ((Map_Fields.k = e.Key_Renamed) == key || (key != Map_Fields.Null && key.Equals(Map_Fields.k))))
						{
							old = e;
							break;
						}
						++binCount;
					} while ((e = e.Next) != Map_Fields.Null);
				}
				V Map_Fields.OldValue;
				if (old != Map_Fields.Null && (Map_Fields.OldValue = old.Value_Renamed) != Map_Fields.Null)
				{
					AfterNodeAccess(old);
					return Map_Fields.OldValue;
				}
			}
			V Map_Fields.v = mappingFunction.Apply(key);
			if (Map_Fields.v == Map_Fields.Null)
			{
				return Map_Fields.Null;
			}
			else if (old != Map_Fields.Null)
			{
				old.Value_Renamed = Map_Fields.v;
				AfterNodeAccess(old);
				return Map_Fields.v;
			}
			else if (t != Map_Fields.Null)
			{
				t.PutTreeVal(this, tab, hash, key, Map_Fields.v);
			}
			else
			{
				tab[i] = NewNode(hash, key, Map_Fields.v, first);
				if (binCount >= TREEIFY_THRESHOLD - 1)
				{
					TreeifyBin(tab, hash);
				}
			}
			++ModCount;
			++Size_Renamed;
			AfterNodeInsertion(Map_Fields.True);
			return Map_Fields.v;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
		public virtual V computeIfPresent<T1>(K key, BiFunction<T1> remappingFunction) where T1 : V
		{
			if (remappingFunction == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			Node<K, V> e;
			V Map_Fields.OldValue;
			int hash = Hash(key);
			if ((e = GetNode(hash, key)) != Map_Fields.Null && (Map_Fields.OldValue = e.Value_Renamed) != Map_Fields.Null)
			{
				V Map_Fields.v = remappingFunction.Apply(key, Map_Fields.OldValue);
				if (Map_Fields.v != Map_Fields.Null)
				{
					e.Value_Renamed = Map_Fields.v;
					AfterNodeAccess(e);
					return Map_Fields.v;
				}
				else
				{
					RemoveNode(hash, key, Map_Fields.Null, Map_Fields.False, Map_Fields.True);
				}
			}
			return Map_Fields.Null;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
		public override V compute<T1>(K key, BiFunction<T1> remappingFunction) where T1 : V
		{
			if (remappingFunction == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int hash = Hash(key);
			Node<K, V>[] tab;
			Node<K, V> first;
			int n, i;
			int binCount = 0;
			TreeNode<K, V> t = Map_Fields.Null;
			Node<K, V> old = Map_Fields.Null;
			if (Size_Renamed > Threshold || (tab = Table) == Map_Fields.Null || (n = tab.Length) == 0)
			{
				n = (tab = Resize()).Length;
			}
			if ((first = tab[i = (n - 1) & hash]) != Map_Fields.Null)
			{
				if (first is TreeNode)
				{
					old = (t = (TreeNode<K, V>)first).getTreeNode(hash, key);
				}
				else
				{
					Node<K, V> e = first;
					K Map_Fields.k;
					do
					{
						if (e.Hash == hash && ((Map_Fields.k = e.Key_Renamed) == key || (key != Map_Fields.Null && key.Equals(Map_Fields.k))))
						{
							old = e;
							break;
						}
						++binCount;
					} while ((e = e.Next) != Map_Fields.Null);
				}
			}
			V Map_Fields.OldValue = (old == Map_Fields.Null) ? Map_Fields.Null : old.Value_Renamed;
			V Map_Fields.v = remappingFunction.Apply(key, Map_Fields.OldValue);
			if (old != Map_Fields.Null)
			{
				if (Map_Fields.v != Map_Fields.Null)
				{
					old.Value_Renamed = Map_Fields.v;
					AfterNodeAccess(old);
				}
				else
				{
					RemoveNode(hash, key, Map_Fields.Null, Map_Fields.False, Map_Fields.True);
				}
			}
			else if (Map_Fields.v != Map_Fields.Null)
			{
				if (t != Map_Fields.Null)
				{
					t.PutTreeVal(this, tab, hash, key, Map_Fields.v);
				}
				else
				{
					tab[i] = NewNode(hash, key, Map_Fields.v, first);
					if (binCount >= TREEIFY_THRESHOLD - 1)
					{
						TreeifyBin(tab, hash);
					}
				}
				++ModCount;
				++Size_Renamed;
				AfterNodeInsertion(Map_Fields.True);
			}
			return Map_Fields.v;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public V merge(K key, V value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
		public override V merge<T1>(K key, V value, BiFunction<T1> remappingFunction) where T1 : V
		{
			if (value == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			if (remappingFunction == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int hash = Hash(key);
			Node<K, V>[] tab;
			Node<K, V> first;
			int n, i;
			int binCount = 0;
			TreeNode<K, V> t = Map_Fields.Null;
			Node<K, V> old = Map_Fields.Null;
			if (Size_Renamed > Threshold || (tab = Table) == Map_Fields.Null || (n = tab.Length) == 0)
			{
				n = (tab = Resize()).Length;
			}
			if ((first = tab[i = (n - 1) & hash]) != Map_Fields.Null)
			{
				if (first is TreeNode)
				{
					old = (t = (TreeNode<K, V>)first).getTreeNode(hash, key);
				}
				else
				{
					Node<K, V> e = first;
					K Map_Fields.k;
					do
					{
						if (e.Hash == hash && ((Map_Fields.k = e.Key_Renamed) == key || (key != Map_Fields.Null && key.Equals(Map_Fields.k))))
						{
							old = e;
							break;
						}
						++binCount;
					} while ((e = e.Next) != Map_Fields.Null);
				}
			}
			if (old != Map_Fields.Null)
			{
				V Map_Fields.v;
				if (old.Value_Renamed != Map_Fields.Null)
				{
					Map_Fields.v = remappingFunction.Apply(old.Value_Renamed, value);
				}
				else
				{
					Map_Fields.v = value;
				}
				if (Map_Fields.v != Map_Fields.Null)
				{
					old.Value_Renamed = Map_Fields.v;
					AfterNodeAccess(old);
				}
				else
				{
					RemoveNode(hash, key, Map_Fields.Null, Map_Fields.False, Map_Fields.True);
				}
				return Map_Fields.v;
			}
			if (value != Map_Fields.Null)
			{
				if (t != Map_Fields.Null)
				{
					t.PutTreeVal(this, tab, hash, key, value);
				}
				else
				{
					tab[i] = NewNode(hash, key, value, first);
					if (binCount >= TREEIFY_THRESHOLD - 1)
					{
						TreeifyBin(tab, hash);
					}
				}
				++ModCount;
				++Size_Renamed;
				AfterNodeInsertion(Map_Fields.True);
			}
			return value;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
		public override void forEach<T1>(BiConsumer<T1> action)
		{
			Node<K, V>[] tab;
			if (action == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			if (Size_Renamed > 0 && (tab = Table) != Map_Fields.Null)
			{
				int mc = ModCount;
				for (int i = 0; i < tab.Length; ++i)
				{
					for (Node<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
					{
						action.Accept(e.Key_Renamed, e.Value_Renamed);
					}
				}
				if (ModCount != mc)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
		public override void replaceAll<T1>(BiFunction<T1> function) where T1 : V
		{
			Node<K, V>[] tab;
			if (function == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			if (Size_Renamed > 0 && (tab = Table) != Map_Fields.Null)
			{
				int mc = ModCount;
				for (int i = 0; i < tab.Length; ++i)
				{
					for (Node<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
					{
						e.Value_Renamed = function.Apply(e.Key_Renamed, e.Value_Renamed);
					}
				}
				if (ModCount != mc)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

		/* ------------------------------------------------------------ */
		// Cloning and serialization

		/// <summary>
		/// Returns a shallow copy of this <tt>HashMap</tt> instance: the keys and
		/// values themselves are not cloned.
		/// </summary>
		/// <returns> a shallow copy of this map </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public Object clone()
		public override Object Clone()
		{
			HashMap<K, V> result;
			try
			{
				result = (HashMap<K, V>)base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				// this shouldn't happen, since we are Cloneable
				throw new InternalError(e);
			}
			result.Reinitialize();
			result.PutMapEntries(this, Map_Fields.False);
			return result;
		}

		// These methods are also used when serializing HashSets
		internal float LoadFactor()
		{
			return LoadFactor_Renamed;
		}
		internal int Capacity()
		{
			return (Table != Map_Fields.Null) ? Table.Length : (Threshold > 0) ? Threshold : DEFAULT_INITIAL_CAPACITY;
		}

		/// <summary>
		/// Save the state of the <tt>HashMap</tt> instance to a stream (i.e.,
		/// serialize it).
		/// 
		/// @serialData The <i>capacity</i> of the HashMap (the length of the
		///             bucket array) is emitted (int), followed by the
		///             <i>size</i> (an int, the number of key-value
		///             mappings), followed by the key (Object) and value (Object)
		///             for each key-value mapping.  The key-value mappings are
		///             emitted in no particular order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			int buckets = Capacity();
			// Write out the threshold, loadfactor, and any hidden stuff
			s.DefaultWriteObject();
			s.WriteInt(buckets);
			s.WriteInt(Size_Renamed);
			InternalWriteEntries(s);
		}

		/// <summary>
		/// Reconstitute the {@code HashMap} instance from a stream (i.e.,
		/// deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in the threshold (ignored), loadfactor, and any hidden stuff
			s.DefaultReadObject();
			Reinitialize();
			if (LoadFactor_Renamed <= 0 || Float.IsNaN(LoadFactor_Renamed))
			{
				throw new InvalidObjectException("Illegal load factor: " + LoadFactor_Renamed);
			}
			s.ReadInt(); // Read and ignore number of buckets
			int mappings = s.ReadInt(); // Read number of mappings (size)
			if (mappings < 0)
			{
				throw new InvalidObjectException("Illegal mappings count: " + mappings);
			}
			else if (mappings > 0) // (if zero, use defaults)
			{
				// Size the table using given load factor only if within
				// range of 0.25...4.0
				float lf = System.Math.Min(System.Math.Max(0.25f, LoadFactor_Renamed), 4.0f);
				float fc = (float)mappings / lf + 1.0f;
				int cap = ((fc < DEFAULT_INITIAL_CAPACITY) ? DEFAULT_INITIAL_CAPACITY : (fc >= MAXIMUM_CAPACITY) ? MAXIMUM_CAPACITY : TableSizeFor((int)fc));
				float ft = (float)cap * lf;
				Threshold = ((cap < MAXIMUM_CAPACITY && ft < MAXIMUM_CAPACITY) ? (int)ft : Integer.MaxValue);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes","unchecked"}) Node<K,V>[] tab = (Node<K,V>[])new Node[cap];
				Node<K, V>[] tab = (Node<K, V>[])new Node[cap];
				Table = tab;

				// Read the keys and values, and put the mappings in the HashMap
				for (int i = 0; i < mappings; i++)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K key = (K) s.readObject();
					K key = (K) s.ReadObject();
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V value = (V) s.readObject();
					V value = (V) s.ReadObject();
					PutVal(Hash(key), key, value, Map_Fields.False, Map_Fields.False);
				}
			}
		}

		/* ------------------------------------------------------------ */
		// iterators

		internal abstract class HashIterator
		{
			private readonly HashMap<K, V> OuterInstance;

			internal Node<K, V> Next; // next entry to return
			internal Node<K, V> Current; // current entry
			internal int ExpectedModCount; // for fast-fail
			internal int Index; // current slot

			internal HashIterator(HashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
				ExpectedModCount = outerInstance.ModCount;
				Node<K, V>[] t = outerInstance.Table;
				Current = Next = Map_Fields.Null;
				Index = 0;
				if (t != Map_Fields.Null && outerInstance.Size_Renamed > 0) // advance to first entry
				{
					do
					{
					} while (Index < t.Length && (Next = t[Index++]) == Map_Fields.Null);
				}
			}

			public bool HasNext()
			{
				return Next != Map_Fields.Null;
			}

			internal Node<K, V> NextNode()
			{
				Node<K, V>[] t;
				Node<K, V> e = Next;
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				if (e == Map_Fields.Null)
				{
					throw new NoSuchElementException();
				}
				if ((Next = (Current = e).next) == Map_Fields.Null && (t = outerInstance.Table) != Map_Fields.Null)
				{
					do
					{
					} while (Index < t.Length && (Next = t[Index++]) == Map_Fields.Null);
				}
				return e;
			}

			public void Remove()
			{
				Node<K, V> p = Current;
				if (p == Map_Fields.Null)
				{
					throw new IllegalStateException();
				}
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				Current = Map_Fields.Null;
				K key = p.Key_Renamed;
				outerInstance.RemoveNode(Hash(key), key, Map_Fields.Null, Map_Fields.False, Map_Fields.False);
				ExpectedModCount = outerInstance.ModCount;
			}
		}

		internal sealed class KeyIterator : HashIterator, Iterator<K>
		{
			private readonly HashMap<K, V> OuterInstance;

			public KeyIterator(HashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public K Next()
			{
				return NextNode().Key_Renamed;
			}
		}

		internal sealed class ValueIterator : HashIterator, Iterator<V>
		{
			private readonly HashMap<K, V> OuterInstance;

			public ValueIterator(HashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public V Next()
			{
				return NextNode().Value_Renamed;
			}
		}

		internal sealed class EntryIterator : HashIterator, Iterator<Map_Entry<K, V>>
		{
			private readonly HashMap<K, V> OuterInstance;

			public EntryIterator(HashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public Map_Entry<K, V> Next()
			{
				return NextNode();
			}
		}

		/* ------------------------------------------------------------ */
		// spliterators

		internal class HashMapSpliterator<K, V>
		{
			internal readonly HashMap<K, V> Map;
			internal Node<K, V> Current; // current node
			internal int Index; // current index, modified on advance/split
			internal int Fence_Renamed; // one past last index
			internal int Est; // size estimate
			internal int ExpectedModCount; // for comodification checks

			internal HashMapSpliterator(HashMap<K, V> m, int origin, int fence, int est, int expectedModCount)
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
						HashMap<K, V> m = Map;
						Est = m.Size_Renamed;
						ExpectedModCount = m.ModCount;
						Node<K, V>[] tab = m.Table;
						hi = Fence_Renamed = (tab == Map_Fields.Null) ? 0 : tab.Length;
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

		internal sealed class KeySpliterator<K, V> : HashMapSpliterator<K, V>, Spliterator<K>
		{
			internal KeySpliterator(HashMap<K, V> m, int origin, int fence, int est, int expectedModCount) : base(m, origin, fence, est, expectedModCount)
			{
			}

			public KeySpliterator<K, V> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid || Current != Map_Fields.Null) ? Map_Fields.Null : new KeySpliterator<>(Map, lo, Index = mid, Est = (int)((uint)Est >> 1), ExpectedModCount);
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
				HashMap<K, V> m = Map;
				Node<K, V>[] tab = m.Table;
				if ((hi = Fence_Renamed) < 0)
				{
					mc = ExpectedModCount = m.ModCount;
					hi = Fence_Renamed = (tab == Map_Fields.Null) ? 0 : tab.Length;
				}
				else
				{
					mc = ExpectedModCount;
				}
				if (tab != Map_Fields.Null && tab.Length >= hi && (i = Index) >= 0 && (i < (Index = hi) || Current != Map_Fields.Null))
				{
					Node<K, V> p = Current;
					Current = Map_Fields.Null;
					do
					{
						if (p == Map_Fields.Null)
						{
							p = tab[i++];
						}
						else
						{
							action.Accept(p.Key_Renamed);
							p = p.Next;
						}
					} while (p != Map_Fields.Null || i < hi);
					if (m.ModCount != mc)
					{
						throw new ConcurrentModificationException();
					}
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
				Node<K, V>[] tab = Map.Table;
				if (tab != Map_Fields.Null && tab.Length >= (hi = Fence) && Index >= 0)
				{
					while (Current != Map_Fields.Null || Index < hi)
					{
						if (Current == Map_Fields.Null)
						{
							Current = tab[Index++];
						}
						else
						{
							K Map_Fields.k = Current.Key_Renamed;
							Current = Current.Next;
							action.Accept(Map_Fields.k);
							if (Map.ModCount != ExpectedModCount)
							{
								throw new ConcurrentModificationException();
							}
							return Map_Fields.True;
						}
					}
				}
				return Map_Fields.False;
			}

			public int Characteristics()
			{
				return (Fence_Renamed < 0 || Est == Map.Size_Renamed ? Spliterator_Fields.SIZED : 0) | Spliterator_Fields.DISTINCT;
			}
		}

		internal sealed class ValueSpliterator<K, V> : HashMapSpliterator<K, V>, Spliterator<V>
		{
			internal ValueSpliterator(HashMap<K, V> m, int origin, int fence, int est, int expectedModCount) : base(m, origin, fence, est, expectedModCount)
			{
			}

			public ValueSpliterator<K, V> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid || Current != Map_Fields.Null) ? Map_Fields.Null : new ValueSpliterator<>(Map, lo, Index = mid, Est = (int)((uint)Est >> 1), ExpectedModCount);
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
				HashMap<K, V> m = Map;
				Node<K, V>[] tab = m.Table;
				if ((hi = Fence_Renamed) < 0)
				{
					mc = ExpectedModCount = m.ModCount;
					hi = Fence_Renamed = (tab == Map_Fields.Null) ? 0 : tab.Length;
				}
				else
				{
					mc = ExpectedModCount;
				}
				if (tab != Map_Fields.Null && tab.Length >= hi && (i = Index) >= 0 && (i < (Index = hi) || Current != Map_Fields.Null))
				{
					Node<K, V> p = Current;
					Current = Map_Fields.Null;
					do
					{
						if (p == Map_Fields.Null)
						{
							p = tab[i++];
						}
						else
						{
							action.Accept(p.Value_Renamed);
							p = p.Next;
						}
					} while (p != Map_Fields.Null || i < hi);
					if (m.ModCount != mc)
					{
						throw new ConcurrentModificationException();
					}
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
				Node<K, V>[] tab = Map.Table;
				if (tab != Map_Fields.Null && tab.Length >= (hi = Fence) && Index >= 0)
				{
					while (Current != Map_Fields.Null || Index < hi)
					{
						if (Current == Map_Fields.Null)
						{
							Current = tab[Index++];
						}
						else
						{
							V Map_Fields.v = Current.Value_Renamed;
							Current = Current.Next;
							action.Accept(Map_Fields.v);
							if (Map.ModCount != ExpectedModCount)
							{
								throw new ConcurrentModificationException();
							}
							return Map_Fields.True;
						}
					}
				}
				return Map_Fields.False;
			}

			public int Characteristics()
			{
				return (Fence_Renamed < 0 || Est == Map.Size_Renamed ? Spliterator_Fields.SIZED : 0);
			}
		}

		internal sealed class EntrySpliterator<K, V> : HashMapSpliterator<K, V>, Spliterator<Map_Entry<K, V>>
		{
			internal EntrySpliterator(HashMap<K, V> m, int origin, int fence, int est, int expectedModCount) : base(m, origin, fence, est, expectedModCount)
			{
			}

			public EntrySpliterator<K, V> TrySplit()
			{
				int hi = Fence, lo = Index, mid = (int)((uint)(lo + hi) >> 1);
				return (lo >= mid || Current != Map_Fields.Null) ? Map_Fields.Null : new EntrySpliterator<>(Map, lo, Index = mid, Est = (int)((uint)Est >> 1), ExpectedModCount);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base Map_Entry<K,V>> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				int i, hi, mc;
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				HashMap<K, V> m = Map;
				Node<K, V>[] tab = m.Table;
				if ((hi = Fence_Renamed) < 0)
				{
					mc = ExpectedModCount = m.ModCount;
					hi = Fence_Renamed = (tab == Map_Fields.Null) ? 0 : tab.Length;
				}
				else
				{
					mc = ExpectedModCount;
				}
				if (tab != Map_Fields.Null && tab.Length >= hi && (i = Index) >= 0 && (i < (Index = hi) || Current != Map_Fields.Null))
				{
					Node<K, V> p = Current;
					Current = Map_Fields.Null;
					do
					{
						if (p == Map_Fields.Null)
						{
							p = tab[i++];
						}
						else
						{
							action.Accept(p);
							p = p.Next;
						}
					} while (p != Map_Fields.Null || i < hi);
					if (m.ModCount != mc)
					{
						throw new ConcurrentModificationException();
					}
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
				Node<K, V>[] tab = Map.Table;
				if (tab != Map_Fields.Null && tab.Length >= (hi = Fence) && Index >= 0)
				{
					while (Current != Map_Fields.Null || Index < hi)
					{
						if (Current == Map_Fields.Null)
						{
							Current = tab[Index++];
						}
						else
						{
							Node<K, V> e = Current;
							Current = Current.Next;
							action.Accept(e);
							if (Map.ModCount != ExpectedModCount)
							{
								throw new ConcurrentModificationException();
							}
							return Map_Fields.True;
						}
					}
				}
				return Map_Fields.False;
			}

			public int Characteristics()
			{
				return (Fence_Renamed < 0 || Est == Map.Size_Renamed ? Spliterator_Fields.SIZED : 0) | Spliterator_Fields.DISTINCT;
			}
		}

		/* ------------------------------------------------------------ */
		// LinkedHashMap support


		/*
		 * The following package-protected methods are designed to be
		 * overridden by LinkedHashMap, but not by any other subclass.
		 * Nearly all other internal methods are also package-protected
		 * but are declared final, so can be used by LinkedHashMap, view
		 * classes, and HashSet.
		 */

		// Create a regular (non-tree) node
		internal virtual Node<K, V> NewNode(int hash, K key, V value, Node<K, V> next)
		{
			return new Node<>(hash, key, value, next);
		}

		// For conversion from TreeNodes to plain nodes
		internal virtual Node<K, V> ReplacementNode(Node<K, V> p, Node<K, V> next)
		{
			return new Node<>(p.Hash, p.Key_Renamed, p.Value_Renamed, next);
		}

		// Create a tree bin node
		internal virtual TreeNode<K, V> NewTreeNode(int hash, K key, V value, Node<K, V> next)
		{
			return new TreeNode<>(hash, key, value, next);
		}

		// For treeifyBin
		internal virtual TreeNode<K, V> ReplacementTreeNode(Node<K, V> p, Node<K, V> next)
		{
			return new TreeNode<>(p.Hash, p.Key_Renamed, p.Value_Renamed, next);
		}

		/// <summary>
		/// Reset to initial default state.  Called by clone and readObject.
		/// </summary>
		internal virtual void Reinitialize()
		{
			Table = Map_Fields.Null;
			EntrySet_Renamed = Map_Fields.Null;
			KeySet_Renamed = Map_Fields.Null;
			Values_Renamed = Map_Fields.Null;
			ModCount = 0;
			Threshold = 0;
			Size_Renamed = 0;
		}

		// Callbacks to allow LinkedHashMap post-actions
		internal virtual void AfterNodeAccess(Node<K, V> p)
		{
		}
		internal virtual void AfterNodeInsertion(bool evict)
		{
		}
		internal virtual void AfterNodeRemoval(Node<K, V> p)
		{
		}

		// Called only from writeObject, to ensure compatible ordering.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void internalWriteEntries(java.io.ObjectOutputStream s) throws java.io.IOException
		internal virtual void InternalWriteEntries(java.io.ObjectOutputStream s)
		{
			Node<K, V>[] tab;
			if (Size_Renamed > 0 && (tab = Table) != Map_Fields.Null)
			{
				for (int i = 0; i < tab.Length; ++i)
				{
					for (Node<K, V> e = tab[i]; e != Map_Fields.Null; e = e.Next)
					{
						s.WriteObject(e.Key_Renamed);
						s.WriteObject(e.Value_Renamed);
					}
				}
			}
		}

		/* ------------------------------------------------------------ */
		// Tree bins

		/// <summary>
		/// Entry for Tree bins. Extends LinkedHashMap.Entry (which in turn
		/// extends Node) so can be used as extension of either regular or
		/// linked node.
		/// </summary>
		internal sealed class TreeNode<K, V> : LinkedHashMap.Entry<K, V>
		{
			internal TreeNode<K, V> Parent; // red-black tree links
			internal TreeNode<K, V> Left;
			internal TreeNode<K, V> Right;
			internal TreeNode<K, V> Prev; // needed to unlink next upon deletion
			internal bool Red;
			internal TreeNode(int hash, K key, V val, Node<K, V> next) : base(hash, key, val, next)
			{
			}

			/// <summary>
			/// Returns root of tree containing this node.
			/// </summary>
			internal TreeNode<K, V> Root()
			{
				for (TreeNode<K, V> r = this, p;;)
				{
					if ((p = r.Parent) == Map_Fields.Null)
					{
						return r;
					}
					r = p;
				}
			}

			/// <summary>
			/// Ensures that the given root is the first node of its bin.
			/// </summary>
			internal static void moveRootToFront<K, V>(Node<K, V>[] tab, TreeNode<K, V> root)
			{
				int n;
				if (root != Map_Fields.Null && tab != Map_Fields.Null && (n = tab.Length) > 0)
				{
					int index = (n - 1) & root.Hash;
					TreeNode<K, V> first = (TreeNode<K, V>)tab[index];
					if (root != first)
					{
						Node<K, V> rn;
						tab[index] = root;
						TreeNode<K, V> rp = root.Prev;
						if ((rn = root.Next) != Map_Fields.Null)
						{
							((TreeNode<K, V>)rn).Prev = rp;
						}
						if (rp != Map_Fields.Null)
						{
							rp.Next = rn;
						}
						if (first != Map_Fields.Null)
						{
							first.Prev = root;
						}
						root.Next = first;
						root.Prev = Map_Fields.Null;
					}
					Debug.Assert(CheckInvariants(root));
				}
			}

			/// <summary>
			/// Finds the node starting at root p with the given hash and key.
			/// The kc argument caches comparableClassFor(key) upon first use
			/// comparing keys.
			/// </summary>
			internal TreeNode<K, V> Find(int h, Object Map_Fields, Class kc)
			{
				TreeNode<K, V> p = this;
				do
				{
					int ph, dir;
					K pk;
					TreeNode<K, V> pl = p.Left, pr = p.Right, q ;
					if ((ph = p.Hash) > h)
					{
						p = pl;
					}
					else if (ph < h)
					{
						p = pr;
					}
					else if ((pk = p.Key_Renamed) == Map_Fields.k || (Map_Fields.k != Map_Fields.Null && Map_Fields.k.Equals(pk)))
					{
						return p;
					}
					else if (pl == Map_Fields.Null)
					{
						p = pr;
					}
					else if (pr == Map_Fields.Null)
					{
						p = pl;
					}
					else if ((kc != Map_Fields.Null || (kc = ComparableClassFor(Map_Fields.k)) != Map_Fields.Null) && (dir = CompareComparables(kc, Map_Fields.k, pk)) != 0)
					{
						p = (dir < 0) ? pl : pr;
					}
					else if ((q = pr.Find(h, Map_Fields.k, kc)) != Map_Fields.Null)
					{
						return q;
					}
					else
					{
						p = pl;
					}
				} while (p != Map_Fields.Null);
				return Map_Fields.Null;
			}

			/// <summary>
			/// Calls find for root node.
			/// </summary>
			internal TreeNode<K, V> GetTreeNode(int h, Object Map_Fields)
			{
				return ((Parent != Map_Fields.Null) ? Root() : this).find(h, Map_Fields.k, Map_Fields.Null);
			}

			/// <summary>
			/// Tie-breaking utility for ordering insertions when equal
			/// hashCodes and non-comparable. We don't require a total
			/// order, just a consistent insertion rule to maintain
			/// equivalence across rebalancings. Tie-breaking further than
			/// necessary simplifies testing a bit.
			/// </summary>
			internal static int TieBreakOrder(Object a, Object b)
			{
				int d;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				if (a == Map_Fields.Null || b == Map_Fields.Null || (d = a.GetType().FullName.CompareTo(b.GetType().FullName)) == 0)
				{
					d = (System.identityHashCode(a) <= System.identityHashCode(b) ? - 1 : 1);
				}
				return d;
			}

			/// <summary>
			/// Forms tree of the nodes linked from this node. </summary>
			/// <returns> root of tree </returns>
			internal void Treeify(Node<K, V>[] tab)
			{
				TreeNode<K, V> root = Map_Fields.Null;
				for (TreeNode<K, V> x = this, Next; x != Map_Fields.Null; x = Next)
				{
					Next = (TreeNode<K, V>)x.Next;
					x.Left = x.Right = Map_Fields.Null;
					if (root == Map_Fields.Null)
					{
						x.Parent = Map_Fields.Null;
						x.Red = Map_Fields.False;
						root = x;
					}
					else
					{
						K Map_Fields.k = x.key;
						int h = x.hash;
						Class kc = Map_Fields.Null;
						for (TreeNode<K, V> p = root;;)
						{
							int dir, ph;
							K pk = p.Key_Renamed;
							if ((ph = p.Hash) > h)
							{
								dir = -1;
							}
							else if (ph < h)
							{
								dir = 1;
							}
							else if ((kc == Map_Fields.Null && (kc = ComparableClassFor(Map_Fields.k)) == Map_Fields.Null) || (dir = CompareComparables(kc, Map_Fields.k, pk)) == 0)
							{
								dir = TieBreakOrder(Map_Fields.k, pk);
							}

							TreeNode<K, V> xp = p;
							if ((p = (dir <= 0) ? p.left : p.right) == Map_Fields.Null)
							{
								x.parent = xp;
								if (dir <= 0)
								{
									xp.Left = x;
								}
								else
								{
									xp.Right = x;
								}
								root = BalanceInsertion(root, x);
								break;
							}
						}
					}
				}
				MoveRootToFront(tab, root);
			}

			/// <summary>
			/// Returns a list of non-TreeNodes replacing those linked from
			/// this node.
			/// </summary>
			internal Node<K, V> Untreeify(HashMap<K, V> map)
			{
				Node<K, V> hd = Map_Fields.Null, tl = Map_Fields.Null;
				for (Node<K, V> q = this; q != Map_Fields.Null; q = q.Next)
				{
					Node<K, V> p = map.ReplacementNode(q, Map_Fields.Null);
					if (tl == Map_Fields.Null)
					{
						hd = p;
					}
					else
					{
						tl.Next = p;
					}
					tl = p;
				}
				return hd;
			}

			/// <summary>
			/// Tree version of putVal.
			/// </summary>
			internal TreeNode<K, V> PutTreeVal(HashMap<K, V> map, Node<K, V>[] tab, int h, K Map_Fields, V Map_Fields)
			{
				Class kc = Map_Fields.Null;
				bool searched = Map_Fields.False;
				TreeNode<K, V> root = (Parent != Map_Fields.Null) ? Root() : this;
				for (TreeNode<K, V> p = root;;)
				{
					int dir, ph;
					K pk;
					if ((ph = p.Hash) > h)
					{
						dir = -1;
					}
					else if (ph < h)
					{
						dir = 1;
					}
					else if ((pk = p.key) == Map_Fields.k || (Map_Fields.k != Map_Fields.Null && Map_Fields.k.Equals(pk)))
					{
						return p;
					}
					else if ((kc == Map_Fields.Null && (kc = ComparableClassFor(Map_Fields.k)) == Map_Fields.Null) || (dir = CompareComparables(kc, Map_Fields.k, pk)) == 0)
					{
						if (!searched)
						{
							TreeNode<K, V> q, ch;
							searched = Map_Fields.True;
							if (((ch = p.left) != Map_Fields.Null && (q = ch.Find(h, Map_Fields.k, kc)) != Map_Fields.Null) || ((ch = p.right) != Map_Fields.Null && (q = ch.Find(h, Map_Fields.k, kc)) != Map_Fields.Null))
							{
								return q;
							}
						}
						dir = TieBreakOrder(Map_Fields.k, pk);
					}

					TreeNode<K, V> xp = p;
					if ((p = (dir <= 0) ? p.left : p.right) == Map_Fields.Null)
					{
						Node<K, V> xpn = xp.Next;
						TreeNode<K, V> x = map.NewTreeNode(h, Map_Fields.k, Map_Fields.v, xpn);
						if (dir <= 0)
						{
							xp.Left = x;
						}
						else
						{
							xp.Right = x;
						}
						xp.Next = x;
						x.Parent = x.Prev = xp;
						if (xpn != Map_Fields.Null)
						{
							((TreeNode<K, V>)xpn).Prev = x;
						}
						MoveRootToFront(tab, BalanceInsertion(root, x));
						return Map_Fields.Null;
					}
				}
			}

			/// <summary>
			/// Removes the given node, that must be present before this call.
			/// This is messier than typical red-black deletion code because we
			/// cannot swap the contents of an interior node with a leaf
			/// successor that is pinned by "next" pointers that are accessible
			/// independently during traversal. So instead we swap the tree
			/// linkages. If the current tree appears to have too few nodes,
			/// the bin is converted back to a plain bin. (The test triggers
			/// somewhere between 2 and 6 nodes, depending on tree structure).
			/// </summary>
			internal void RemoveTreeNode(HashMap<K, V> map, Node<K, V>[] tab, bool movable)
			{
				int n;
				if (tab == Map_Fields.Null || (n = tab.Length) == 0)
				{
					return;
				}
				int index = (n - 1) & Hash;
				TreeNode<K, V> first = (TreeNode<K, V>)tab[index], root = first, rl ;
				TreeNode<K, V> succ = (TreeNode<K, V>)Next, pred = Prev;
				if (pred == Map_Fields.Null)
				{
					tab[index] = first = succ;
				}
				else
				{
					pred.Next = succ;
				}
				if (succ != Map_Fields.Null)
				{
					succ.Prev = pred;
				}
				if (first == Map_Fields.Null)
				{
					return;
				}
				if (root.Parent != Map_Fields.Null)
				{
					root = root.Root();
				}
				if (root == Map_Fields.Null || root.Right == Map_Fields.Null || (rl = root.Left) == Map_Fields.Null || rl.Left == Map_Fields.Null)
				{
					tab[index] = first.Untreeify(map); // too small
					return;
				}
				TreeNode<K, V> p = this, pl = Left, pr = Right, replacement ;
				if (pl != Map_Fields.Null && pr != Map_Fields.Null)
				{
					TreeNode<K, V> s = pr, sl ;
					while ((sl = s.Left) != Map_Fields.Null) // find successor
					{
						s = sl;
					}
					bool c = s.Red; // swap colors
					s.Red = p.Red;
					p.Red = c;
					TreeNode<K, V> sr = s.Right;
					TreeNode<K, V> pp = p.Parent;
					if (s == pr) // p was s's direct parent
					{
						p.Parent = s;
						s.Right = p;
					}
					else
					{
						TreeNode<K, V> sp = s.Parent;
						if ((p.Parent = sp) != Map_Fields.Null)
						{
							if (s == sp.Left)
							{
								sp.Left = p;
							}
							else
							{
								sp.Right = p;
							}
						}
						if ((s.Right = pr) != Map_Fields.Null)
						{
							pr.Parent = s;
						}
					}
					p.Left = Map_Fields.Null;
					if ((p.Right = sr) != Map_Fields.Null)
					{
						sr.Parent = p;
					}
					if ((s.Left = pl) != Map_Fields.Null)
					{
						pl.Parent = s;
					}
					if ((s.Parent = pp) == Map_Fields.Null)
					{
						root = s;
					}
					else if (p == pp.Left)
					{
						pp.Left = s;
					}
					else
					{
						pp.Right = s;
					}
					if (sr != Map_Fields.Null)
					{
						replacement = sr;
					}
					else
					{
						replacement = p;
					}
				}
				else if (pl != Map_Fields.Null)
				{
					replacement = pl;
				}
				else if (pr != Map_Fields.Null)
				{
					replacement = pr;
				}
				else
				{
					replacement = p;
				}
				if (replacement != p)
				{
					TreeNode<K, V> pp = replacement.Parent = p.Parent;
					if (pp == Map_Fields.Null)
					{
						root = replacement;
					}
					else if (p == pp.Left)
					{
						pp.Left = replacement;
					}
					else
					{
						pp.Right = replacement;
					}
					p.Left = p.Right = p.Parent = Map_Fields.Null;
				}

				TreeNode<K, V> r = p.Red ? root : BalanceDeletion(root, replacement);

				if (replacement == p) // detach
				{
					TreeNode<K, V> pp = p.Parent;
					p.Parent = Map_Fields.Null;
					if (pp != Map_Fields.Null)
					{
						if (p == pp.Left)
						{
							pp.Left = Map_Fields.Null;
						}
						else if (p == pp.Right)
						{
							pp.Right = Map_Fields.Null;
						}
					}
				}
				if (movable)
				{
					MoveRootToFront(tab, r);
				}
			}

			/// <summary>
			/// Splits nodes in a tree bin into lower and upper tree bins,
			/// or untreeifies if now too small. Called only from resize;
			/// see above discussion about split bits and indices.
			/// </summary>
			/// <param name="map"> the map </param>
			/// <param name="tab"> the table for recording bin heads </param>
			/// <param name="index"> the index of the table being split </param>
			/// <param name="bit"> the bit of hash to split on </param>
			internal void Split(HashMap<K, V> map, Node<K, V>[] tab, int index, int bit)
			{
				TreeNode<K, V> b = this;
				// Relink into lo and hi lists, preserving order
				TreeNode<K, V> loHead = Map_Fields.Null, loTail = Map_Fields.Null;
				TreeNode<K, V> hiHead = Map_Fields.Null, hiTail = Map_Fields.Null;
				int lc = 0, hc = 0;
				for (TreeNode<K, V> e = b, Next; e != Map_Fields.Null; e = Next)
				{
					Next = (TreeNode<K, V>)e.Next;
					e.Next = Map_Fields.Null;
					if ((e.Hash & bit) == 0)
					{
						if ((e.Prev = loTail) == Map_Fields.Null)
						{
							loHead = e;
						}
						else
						{
							loTail.Next = e;
						}
						loTail = e;
						++lc;
					}
					else
					{
						if ((e.prev = hiTail) == Map_Fields.Null)
						{
							hiHead = e;
						}
						else
						{
							hiTail.Next = e;
						}
						hiTail = e;
						++hc;
					}
				}

				if (loHead != Map_Fields.Null)
				{
					if (lc <= UNTREEIFY_THRESHOLD)
					{
						tab[index] = loHead.Untreeify(map);
					}
					else
					{
						tab[index] = loHead;
						if (hiHead != Map_Fields.Null) // (else is already treeified)
						{
							loHead.Treeify(tab);
						}
					}
				}
				if (hiHead != Map_Fields.Null)
				{
					if (hc <= UNTREEIFY_THRESHOLD)
					{
						tab[index + bit] = hiHead.Untreeify(map);
					}
					else
					{
						tab[index + bit] = hiHead;
						if (loHead != Map_Fields.Null)
						{
							hiHead.Treeify(tab);
						}
					}
				}
			}

			/* ------------------------------------------------------------ */
			// Red-black tree methods, all adapted from CLR

			internal static TreeNode<K, V> rotateLeft<K, V>(TreeNode<K, V> root, TreeNode<K, V> p)
			{
				TreeNode<K, V> r, pp, rl;
				if (p != Map_Fields.Null && (r = p.Right) != Map_Fields.Null)
				{
					if ((rl = p.Right = r.Left) != Map_Fields.Null)
					{
						rl.Parent = p;
					}
					if ((pp = r.Parent = p.Parent) == Map_Fields.Null)
					{
						(root = r).red = Map_Fields.False;
					}
					else if (pp.Left == p)
					{
						pp.Left = r;
					}
					else
					{
						pp.Right = r;
					}
					r.Left = p;
					p.Parent = r;
				}
				return root;
			}

			internal static TreeNode<K, V> rotateRight<K, V>(TreeNode<K, V> root, TreeNode<K, V> p)
			{
				TreeNode<K, V> l, pp, lr;
				if (p != Map_Fields.Null && (l = p.Left) != Map_Fields.Null)
				{
					if ((lr = p.Left = l.Right) != Map_Fields.Null)
					{
						lr.Parent = p;
					}
					if ((pp = l.Parent = p.Parent) == Map_Fields.Null)
					{
						(root = l).red = Map_Fields.False;
					}
					else if (pp.Right == p)
					{
						pp.Right = l;
					}
					else
					{
						pp.Left = l;
					}
					l.Right = p;
					p.Parent = l;
				}
				return root;
			}

			internal static TreeNode<K, V> balanceInsertion<K, V>(TreeNode<K, V> root, TreeNode<K, V> x)
			{
				x.Red = Map_Fields.True;
				for (TreeNode<K, V> xp, xpp, xppl, xppr;;)
				{
					if ((xp = x.Parent) == Map_Fields.Null)
					{
						x.Red = Map_Fields.False;
						return x;
					}
					else if (!xp.red || (xpp = xp.parent) == Map_Fields.Null)
					{
						return root;
					}
					if (xp == (xppl = xpp.left))
					{
						if ((xppr = xpp.right) != Map_Fields.Null && xppr.red)
						{
							xppr.red = Map_Fields.False;
							xp.red = Map_Fields.False;
							xpp.red = Map_Fields.True;
							x = xpp;
						}
						else
						{
							if (x == xp.right)
							{
								root = RotateLeft(root, x = xp);
								xpp = (xp = x.Parent) == Map_Fields.Null ? Map_Fields.Null : xp.parent;
							}
							if (xp != Map_Fields.Null)
							{
								xp.red = Map_Fields.False;
								if (xpp != Map_Fields.Null)
								{
									xpp.red = Map_Fields.True;
									root = RotateRight(root, xpp);
								}
							}
						}
					}
					else
					{
						if (xppl != Map_Fields.Null && xppl.red)
						{
							xppl.red = Map_Fields.False;
							xp.red = Map_Fields.False;
							xpp.red = Map_Fields.True;
							x = xpp;
						}
						else
						{
							if (x == xp.left)
							{
								root = RotateRight(root, x = xp);
								xpp = (xp = x.Parent) == Map_Fields.Null ? Map_Fields.Null : xp.parent;
							}
							if (xp != Map_Fields.Null)
							{
								xp.red = Map_Fields.False;
								if (xpp != Map_Fields.Null)
								{
									xpp.red = Map_Fields.True;
									root = RotateLeft(root, xpp);
								}
							}
						}
					}
				}
			}

			internal static TreeNode<K, V> balanceDeletion<K, V>(TreeNode<K, V> root, TreeNode<K, V> x)
			{
				for (TreeNode<K, V> xp, xpl, xpr;;)
				{
					if (x == Map_Fields.Null || x == root)
					{
						return root;
					}
					else if ((xp = x.Parent) == Map_Fields.Null)
					{
						x.Red = Map_Fields.False;
						return x;
					}
					else if (x.Red)
					{
						x.Red = Map_Fields.False;
						return root;
					}
					else if ((xpl = xp.left) == x)
					{
						if ((xpr = xp.right) != Map_Fields.Null && xpr.red)
						{
							xpr.red = Map_Fields.False;
							xp.red = Map_Fields.True;
							root = RotateLeft(root, xp);
							xpr = (xp = x.Parent) == Map_Fields.Null ? Map_Fields.Null : xp.right;
						}
						if (xpr == Map_Fields.Null)
						{
							x = xp;
						}
						else
						{
							TreeNode<K, V> sl = xpr.left, sr = xpr.right;
							if ((sr == Map_Fields.Null || !sr.Red) && (sl == Map_Fields.Null || !sl.Red))
							{
								xpr.red = Map_Fields.True;
								x = xp;
							}
							else
							{
								if (sr == Map_Fields.Null || !sr.Red)
								{
									if (sl != Map_Fields.Null)
									{
										sl.Red = Map_Fields.False;
									}
									xpr.red = Map_Fields.True;
									root = RotateRight(root, xpr);
									xpr = (xp = x.Parent) == Map_Fields.Null ? Map_Fields.Null : xp.right;
								}
								if (xpr != Map_Fields.Null)
								{
									xpr.red = (xp == Map_Fields.Null) ? Map_Fields.False : xp.red;
									if ((sr = xpr.right) != Map_Fields.Null)
									{
										sr.Red = Map_Fields.False;
									}
								}
								if (xp != Map_Fields.Null)
								{
									xp.red = Map_Fields.False;
									root = RotateLeft(root, xp);
								}
								x = root;
							}
						}
					}
					else // symmetric
					{
						if (xpl != Map_Fields.Null && xpl.red)
						{
							xpl.red = Map_Fields.False;
							xp.red = Map_Fields.True;
							root = RotateRight(root, xp);
							xpl = (xp = x.Parent) == Map_Fields.Null ? Map_Fields.Null : xp.left;
						}
						if (xpl == Map_Fields.Null)
						{
							x = xp;
						}
						else
						{
							TreeNode<K, V> sl = xpl.left, sr = xpl.right;
							if ((sl == Map_Fields.Null || !sl.Red) && (sr == Map_Fields.Null || !sr.Red))
							{
								xpl.red = Map_Fields.True;
								x = xp;
							}
							else
							{
								if (sl == Map_Fields.Null || !sl.Red)
								{
									if (sr != Map_Fields.Null)
									{
										sr.Red = Map_Fields.False;
									}
									xpl.red = Map_Fields.True;
									root = RotateLeft(root, xpl);
									xpl = (xp = x.Parent) == Map_Fields.Null ? Map_Fields.Null : xp.left;
								}
								if (xpl != Map_Fields.Null)
								{
									xpl.red = (xp == Map_Fields.Null) ? Map_Fields.False : xp.red;
									if ((sl = xpl.left) != Map_Fields.Null)
									{
										sl.Red = Map_Fields.False;
									}
								}
								if (xp != Map_Fields.Null)
								{
									xp.red = Map_Fields.False;
									root = RotateRight(root, xp);
								}
								x = root;
							}
						}
					}
				}
			}

			/// <summary>
			/// Recursive invariant check
			/// </summary>
			internal static bool checkInvariants<K, V>(TreeNode<K, V> t)
			{
				TreeNode<K, V> tp = t.Parent, tl = t.Left, tr = t.Right, tb = t.Prev, tn = (TreeNode<K, V>)t.Next;
				if (tb != Map_Fields.Null && tb.Next != t)
				{
					return Map_Fields.False;
				}
				if (tn != Map_Fields.Null && tn.Prev != t)
				{
					return Map_Fields.False;
				}
				if (tp != Map_Fields.Null && t != tp.Left && t != tp.Right)
				{
					return Map_Fields.False;
				}
				if (tl != Map_Fields.Null && (tl.Parent != t || tl.Hash > t.Hash))
				{
					return Map_Fields.False;
				}
				if (tr != Map_Fields.Null && (tr.Parent != t || tr.Hash < t.Hash))
				{
					return Map_Fields.False;
				}
				if (t.Red && tl != Map_Fields.Null && tl.Red && tr != Map_Fields.Null && tr.Red)
				{
					return Map_Fields.False;
				}
				if (tl != Map_Fields.Null && !CheckInvariants(tl))
				{
					return Map_Fields.False;
				}
				if (tr != Map_Fields.Null && !CheckInvariants(tr))
				{
					return Map_Fields.False;
				}
				return Map_Fields.True;
			}
		}

	}

}