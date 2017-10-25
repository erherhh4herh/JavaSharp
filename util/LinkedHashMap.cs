using System;

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
	/// <para>Hash table and linked list implementation of the <tt>Map</tt> interface,
	/// with predictable iteration order.  This implementation differs from
	/// <tt>HashMap</tt> in that it maintains a doubly-linked list running through
	/// all of its entries.  This linked list defines the iteration ordering,
	/// which is normally the order in which keys were inserted into the map
	/// (<i>insertion-order</i>).  Note that insertion order is not affected
	/// if a key is <i>re-inserted</i> into the map.  (A key <tt>k</tt> is
	/// reinserted into a map <tt>m</tt> if <tt>m.put(k, v)</tt> is invoked when
	/// <tt>m.containsKey(k)</tt> would return <tt>true</tt> immediately prior to
	/// the invocation.)
	/// 
	/// </para>
	/// <para>This implementation spares its clients from the unspecified, generally
	/// chaotic ordering provided by <seealso cref="HashMap"/> (and <seealso cref="Hashtable"/>),
	/// without incurring the increased cost associated with <seealso cref="TreeMap"/>.  It
	/// can be used to produce a copy of a map that has the same order as the
	/// original, regardless of the original map's implementation:
	/// <pre>
	///     void foo(Map m) {
	///         Map copy = new LinkedHashMap(m);
	///         ...
	///     }
	/// </pre>
	/// This technique is particularly useful if a module takes a map on input,
	/// copies it, and later returns results whose order is determined by that of
	/// the copy.  (Clients generally appreciate having things returned in the same
	/// order they were presented.)
	/// 
	/// </para>
	/// <para>A special <seealso cref="#LinkedHashMap(int,float,boolean) constructor"/> is
	/// provided to create a linked hash map whose order of iteration is the order
	/// in which its entries were last accessed, from least-recently accessed to
	/// most-recently (<i>access-order</i>).  This kind of map is well-suited to
	/// building LRU caches.  Invoking the {@code put}, {@code putIfAbsent},
	/// {@code get}, {@code getOrDefault}, {@code compute}, {@code computeIfAbsent},
	/// {@code computeIfPresent}, or {@code merge} methods results
	/// in an access to the corresponding entry (assuming it exists after the
	/// invocation completes). The {@code replace} methods only result in an access
	/// of the entry if the value is replaced.  The {@code putAll} method generates one
	/// entry access for each mapping in the specified map, in the order that
	/// key-value mappings are provided by the specified map's entry set iterator.
	/// <i>No other methods generate entry accesses.</i>  In particular, operations
	/// on collection-views do <i>not</i> affect the order of iteration of the
	/// backing map.
	/// 
	/// </para>
	/// <para>The <seealso cref="#removeEldestEntry(Map.Entry)"/> method may be overridden to
	/// impose a policy for removing stale mappings automatically when new mappings
	/// are added to the map.
	/// 
	/// </para>
	/// <para>This class provides all of the optional <tt>Map</tt> operations, and
	/// permits null elements.  Like <tt>HashMap</tt>, it provides constant-time
	/// performance for the basic operations (<tt>add</tt>, <tt>contains</tt> and
	/// <tt>remove</tt>), assuming the hash function disperses elements
	/// properly among the buckets.  Performance is likely to be just slightly
	/// below that of <tt>HashMap</tt>, due to the added expense of maintaining the
	/// linked list, with one exception: Iteration over the collection-views
	/// of a <tt>LinkedHashMap</tt> requires time proportional to the <i>size</i>
	/// of the map, regardless of its capacity.  Iteration over a <tt>HashMap</tt>
	/// is likely to be more expensive, requiring time proportional to its
	/// <i>capacity</i>.
	/// 
	/// </para>
	/// <para>A linked hash map has two parameters that affect its performance:
	/// <i>initial capacity</i> and <i>load factor</i>.  They are defined precisely
	/// as for <tt>HashMap</tt>.  Note, however, that the penalty for choosing an
	/// excessively high value for initial capacity is less severe for this class
	/// than for <tt>HashMap</tt>, as iteration times for this class are unaffected
	/// by capacity.
	/// 
	/// </para>
	/// <para><strong>Note that this implementation is not synchronized.</strong>
	/// If multiple threads access a linked hash map concurrently, and at least
	/// one of the threads modifies the map structurally, it <em>must</em> be
	/// synchronized externally.  This is typically accomplished by
	/// synchronizing on some object that naturally encapsulates the map.
	/// 
	/// If no such object exists, the map should be "wrapped" using the
	/// <seealso cref="Collections#synchronizedMap Collections.synchronizedMap"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access to the map:<pre>
	///   Map m = Collections.synchronizedMap(new LinkedHashMap(...));</pre>
	/// 
	/// A structural modification is any operation that adds or deletes one or more
	/// mappings or, in the case of access-ordered linked hash maps, affects
	/// iteration order.  In insertion-ordered linked hash maps, merely changing
	/// the value associated with a key that is already contained in the map is not
	/// a structural modification.  <strong>In access-ordered linked hash maps,
	/// merely querying the map with <tt>get</tt> is a structural modification.
	/// </strong>)
	/// 
	/// </para>
	/// <para>The iterators returned by the <tt>iterator</tt> method of the collections
	/// returned by all of this class's collection view methods are
	/// <em>fail-fast</em>: if the map is structurally modified at any time after
	/// the iterator is created, in any way except through the iterator's own
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
	/// exception for its correctness:   <i>the fail-fast behavior of iterators
	/// should be used only to detect bugs.</i>
	/// 
	/// </para>
	/// <para>The spliterators returned by the spliterator method of the collections
	/// returned by all of this class's collection view methods are
	/// <em><a href="Spliterator.html#binding">late-binding</a></em>,
	/// <em>fail-fast</em>, and additionally report <seealso cref="Spliterator#ORDERED"/>.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @implNote
	/// The spliterators returned by the spliterator method of the collections
	/// returned by all of this class's collection view methods are created from
	/// the iterators of the corresponding collections.
	/// 
	/// </para>
	/// </summary>
	/// @param <K> the type of keys maintained by this map </param>
	/// @param <V> the type of mapped values
	/// 
	/// @author  Josh Bloch </param>
	/// <seealso cref=     Object#hashCode() </seealso>
	/// <seealso cref=     Collection </seealso>
	/// <seealso cref=     Map </seealso>
	/// <seealso cref=     HashMap </seealso>
	/// <seealso cref=     TreeMap </seealso>
	/// <seealso cref=     Hashtable
	/// @since   1.4 </seealso>
	public class LinkedHashMap<K, V> : HashMap<K, V>, Map<K, V>
	{

		/*
		 * Implementation note.  A previous version of this class was
		 * internally structured a little differently. Because superclass
		 * HashMap now uses trees for some of its nodes, class
		 * LinkedHashMap.Entry is now treated as intermediary node class
		 * that can also be converted to tree form. The name of this
		 * class, LinkedHashMap.Entry, is confusing in several ways in its
		 * current context, but cannot be changed.  Otherwise, even though
		 * it is not exported outside this package, some existing source
		 * code is known to have relied on a symbol resolution corner case
		 * rule in calls to removeEldestEntry that suppressed compilation
		 * errors due to ambiguous usages. So, we keep the name to
		 * preserve unmodified compilability.
		 *
		 * The changes in node classes also require using two fields
		 * (head, tail) rather than a pointer to a header node to maintain
		 * the doubly-linked before/after list. This class also
		 * previously used a different style of callback methods upon
		 * access, insertion, and removal.
		 */

		/// <summary>
		/// HashMap.Node subclass for normal LinkedHashMap entries.
		/// </summary>
		internal class Entry<K, V> : HashMap.Node<K, V>
		{
			internal Entry<K, V> Before, After;
			internal Entry(int hash, K key, V value, Node<K, V> next) : base(hash, key, value, next)
			{
			}
		}

		private const long SerialVersionUID = 3801124242820219131L;

		/// <summary>
		/// The head (eldest) of the doubly linked list.
		/// </summary>
		[NonSerialized]
		internal LinkedHashMap.Entry<K, V> Head;

		/// <summary>
		/// The tail (youngest) of the doubly linked list.
		/// </summary>
		[NonSerialized]
		internal LinkedHashMap.Entry<K, V> Tail;

		/// <summary>
		/// The iteration ordering method for this linked hash map: <tt>true</tt>
		/// for access-order, <tt>false</tt> for insertion-order.
		/// 
		/// @serial
		/// </summary>
		internal readonly bool AccessOrder;

		// internal utilities

		// link at the end of list
		private void LinkNodeLast(LinkedHashMap.Entry<K, V> p)
		{
			LinkedHashMap.Entry<K, V> last = Tail;
			Tail = p;
			if (last == Map_Fields.Null)
			{
				Head = p;
			}
			else
			{
				p.Before = last;
				last.After = p;
			}
		}

		// apply src's links to dst
		private void TransferLinks(LinkedHashMap.Entry<K, V> src, LinkedHashMap.Entry<K, V> dst)
		{
			LinkedHashMap.Entry<K, V> b = dst.Before = src.Before;
			LinkedHashMap.Entry<K, V> a = dst.After = src.After;
			if (b == Map_Fields.Null)
			{
				Head = dst;
			}
			else
			{
				b.After = dst;
			}
			if (a == Map_Fields.Null)
			{
				Tail = dst;
			}
			else
			{
				a.Before = dst;
			}
		}

		// overrides of HashMap hook methods

		internal virtual void Reinitialize()
		{
			base.Reinitialize();
			Head = Tail = Map_Fields.Null;
		}

		internal virtual Node<K, V> NewNode(int hash, K key, V value, Node<K, V> e)
		{
			LinkedHashMap.Entry<K, V> p = new LinkedHashMap.Entry<K, V>(hash, key, value, e);
			LinkNodeLast(p);
			return p;
		}

		internal virtual Node<K, V> ReplacementNode(Node<K, V> p, Node<K, V> next)
		{
			LinkedHashMap.Entry<K, V> q = (LinkedHashMap.Entry<K, V>)p;
			LinkedHashMap.Entry<K, V> t = new LinkedHashMap.Entry<K, V>(q.Hash, q.Key_Renamed, q.Value_Renamed, next);
			TransferLinks(q, t);
			return t;
		}

		internal virtual TreeNode<K, V> NewTreeNode(int hash, K key, V value, Node<K, V> next)
		{
			TreeNode<K, V> p = new TreeNode<K, V>(hash, key, value, next);
			LinkNodeLast(p);
			return p;
		}

		internal virtual TreeNode<K, V> ReplacementTreeNode(Node<K, V> p, Node<K, V> next)
		{
			LinkedHashMap.Entry<K, V> q = (LinkedHashMap.Entry<K, V>)p;
			TreeNode<K, V> t = new TreeNode<K, V>(q.Hash, q.Key_Renamed, q.Value_Renamed, next);
			TransferLinks(q, t);
			return t;
		}

		internal virtual void AfterNodeRemoval(Node<K, V> e) // unlink
		{
			LinkedHashMap.Entry<K, V> p = (LinkedHashMap.Entry<K, V>)e, b = p.Before, a = p.After;
			p.Before = p.After = Map_Fields.Null;
			if (b == Map_Fields.Null)
			{
				Head = a;
			}
			else
			{
				b.After = a;
			}
			if (a == Map_Fields.Null)
			{
				Tail = b;
			}
			else
			{
				a.Before = b;
			}
		}

		internal virtual void AfterNodeInsertion(bool evict) // possibly remove eldest
		{
			LinkedHashMap.Entry<K, V> first;
			if (evict && (first = Head) != Map_Fields.Null && RemoveEldestEntry(first))
			{
				K key = first.Key_Renamed;
				RemoveNode(Hash(key), key, Map_Fields.Null, Map_Fields.False, Map_Fields.True);
			}
		}

		internal virtual void AfterNodeAccess(Node<K, V> e) // move node to last
		{
			LinkedHashMap.Entry<K, V> last;
			if (AccessOrder && (last = Tail) != e)
			{
				LinkedHashMap.Entry<K, V> p = (LinkedHashMap.Entry<K, V>)e, b = p.Before, a = p.After;
				p.After = Map_Fields.Null;
				if (b == Map_Fields.Null)
				{
					Head = a;
				}
				else
				{
					b.After = a;
				}
				if (a != Map_Fields.Null)
				{
					a.Before = b;
				}
				else
				{
					last = b;
				}
				if (last == Map_Fields.Null)
				{
					Head = p;
				}
				else
				{
					p.Before = last;
					last.After = p;
				}
				Tail = p;
				++ModCount;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void internalWriteEntries(java.io.ObjectOutputStream s) throws java.io.IOException
		internal virtual void InternalWriteEntries(java.io.ObjectOutputStream s)
		{
			for (LinkedHashMap.Entry<K, V> e = Head; e != Map_Fields.Null; e = e.After)
			{
				s.WriteObject(e.Key_Renamed);
				s.WriteObject(e.Value_Renamed);
			}
		}

		/// <summary>
		/// Constructs an empty insertion-ordered <tt>LinkedHashMap</tt> instance
		/// with the specified initial capacity and load factor.
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity </param>
		/// <param name="loadFactor">      the load factor </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is negative
		///         or the load factor is nonpositive </exception>
		public LinkedHashMap(int initialCapacity, float loadFactor) : base(initialCapacity, loadFactor)
		{
			AccessOrder = Map_Fields.False;
		}

		/// <summary>
		/// Constructs an empty insertion-ordered <tt>LinkedHashMap</tt> instance
		/// with the specified initial capacity and a default load factor (0.75).
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is negative </exception>
		public LinkedHashMap(int initialCapacity) : base(initialCapacity)
		{
			AccessOrder = Map_Fields.False;
		}

		/// <summary>
		/// Constructs an empty insertion-ordered <tt>LinkedHashMap</tt> instance
		/// with the default initial capacity (16) and load factor (0.75).
		/// </summary>
		public LinkedHashMap() : base()
		{
			AccessOrder = Map_Fields.False;
		}

		/// <summary>
		/// Constructs an insertion-ordered <tt>LinkedHashMap</tt> instance with
		/// the same mappings as the specified map.  The <tt>LinkedHashMap</tt>
		/// instance is created with a default load factor (0.75) and an initial
		/// capacity sufficient to hold the mappings in the specified map.
		/// </summary>
		/// <param name="m"> the map whose mappings are to be placed in this map </param>
		/// <exception cref="NullPointerException"> if the specified map is null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public LinkedHashMap(Map<? extends K, ? extends V> m)
		public LinkedHashMap<T1>(Map<T1> m) where T1 : K where ? : V : base()
		{
			AccessOrder = Map_Fields.False;
			PutMapEntries(m, Map_Fields.False);
		}

		/// <summary>
		/// Constructs an empty <tt>LinkedHashMap</tt> instance with the
		/// specified initial capacity, load factor and ordering mode.
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity </param>
		/// <param name="loadFactor">      the load factor </param>
		/// <param name="accessOrder">     the ordering mode - <tt>true</tt> for
		///         access-order, <tt>false</tt> for insertion-order </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is negative
		///         or the load factor is nonpositive </exception>
		public LinkedHashMap(int initialCapacity, float loadFactor, bool accessOrder) : base(initialCapacity, loadFactor)
		{
			this.AccessOrder = accessOrder;
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
			for (LinkedHashMap.Entry<K, V> e = Head; e != Map_Fields.Null; e = e.After)
			{
				V Map_Fields.v = e.Value_Renamed;
				if (Map_Fields.v == value || (value != Map_Fields.Null && value.Equals(Map_Fields.v)))
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
		/// </para>
		/// </summary>
		public virtual V Get(Object key)
		{
			Node<K, V> e;
			if ((e = GetNode(Hash(key), key)) == Map_Fields.Null)
			{
				return Map_Fields.Null;
			}
			if (AccessOrder)
			{
				AfterNodeAccess(e);
			}
			return e.Value_Renamed;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual V GetOrDefault(Object key, V defaultValue)
		{
		   Node<K, V> e;
		   if ((e = GetNode(Hash(key), key)) == Map_Fields.Null)
		   {
			   return defaultValue;
		   }
		   if (AccessOrder)
		   {
			   AfterNodeAccess(e);
		   }
		   return e.Value_Renamed;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual void Clear()
		{
			base.Clear();
			Head = Tail = Map_Fields.Null;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this map should remove its eldest entry.
		/// This method is invoked by <tt>put</tt> and <tt>putAll</tt> after
		/// inserting a new entry into the map.  It provides the implementor
		/// with the opportunity to remove the eldest entry each time a new one
		/// is added.  This is useful if the map represents a cache: it allows
		/// the map to reduce memory consumption by deleting stale entries.
		/// 
		/// <para>Sample use: this override will allow the map to grow up to 100
		/// entries and then delete the eldest entry each time a new entry is
		/// added, maintaining a steady state of 100 entries.
		/// <pre>
		///     private static final int MAX_ENTRIES = 100;
		/// 
		///     protected boolean removeEldestEntry(Map.Entry eldest) {
		///        return size() &gt; MAX_ENTRIES;
		///     }
		/// </pre>
		/// 
		/// </para>
		/// <para>This method typically does not modify the map in any way,
		/// instead allowing the map to modify itself as directed by its
		/// return value.  It <i>is</i> permitted for this method to modify
		/// the map directly, but if it does so, it <i>must</i> return
		/// <tt>false</tt> (indicating that the map should not attempt any
		/// further modification).  The effects of returning <tt>true</tt>
		/// after modifying the map from within this method are unspecified.
		/// 
		/// </para>
		/// <para>This implementation merely returns <tt>false</tt> (so that this
		/// map acts like a normal map - the eldest element is never removed).
		/// 
		/// </para>
		/// </summary>
		/// <param name="eldest"> The least recently inserted entry in the map, or if
		///           this is an access-ordered map, the least recently accessed
		///           entry.  This is the entry that will be removed it this
		///           method returns <tt>true</tt>.  If the map was empty prior
		///           to the <tt>put</tt> or <tt>putAll</tt> invocation resulting
		///           in this invocation, this will be the entry that was just
		///           inserted; in other words, if the map contains a single
		///           entry, the eldest entry is also the newest. </param>
		/// <returns>   <tt>true</tt> if the eldest entry should be removed
		///           from the map; <tt>false</tt> if it should be retained. </returns>
		protected internal virtual bool RemoveEldestEntry(Map_Entry<K, V> eldest)
		{
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
		/// Its <seealso cref="Spliterator"/> typically provides faster sequential
		/// performance but much poorer parallel performance than that of
		/// {@code HashMap}.
		/// </summary>
		/// <returns> a set view of the keys contained in this map </returns>
		public virtual Set<K> KeySet()
		{
			Set<K> ks;
			return (ks = KeySet_Renamed) == Map_Fields.Null ? (KeySet_Renamed = new LinkedKeySet(this)) : ks;
		}

		internal sealed class LinkedKeySet : AbstractSet<K>
		{
			private readonly LinkedHashMap<K, V> OuterInstance;

			public LinkedKeySet(LinkedHashMap<K, V> outerInstance)
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
				return new LinkedKeyIterator(OuterInstance);
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
				return Spliterators.Spliterator(this, Spliterator_Fields.SIZED | Spliterator_Fields.ORDERED | Spliterator_Fields.DISTINCT);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final void forEach(java.util.function.Consumer<? base K> action)
			public void forEach<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				int mc = outerInstance.ModCount;
				for (LinkedHashMap.Entry<K, V> e = outerInstance.Head; e != Map_Fields.Null; e = e.After)
				{
					action.Accept(e.Key_Renamed);
				}
				if (outerInstance.ModCount != mc)
				{
					throw new ConcurrentModificationException();
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
		/// Its <seealso cref="Spliterator"/> typically provides faster sequential
		/// performance but much poorer parallel performance than that of
		/// {@code HashMap}.
		/// </summary>
		/// <returns> a view of the values contained in this map </returns>
		public virtual Collection<V> Values()
		{
			Collection<V> vs;
			return (vs = this.Values_Renamed) == Map_Fields.Null ? (this.Values_Renamed = new LinkedValues(this)) : vs;
		}

		internal sealed class LinkedValues : AbstractCollection<V>
		{
			private readonly LinkedHashMap<K, V> OuterInstance;

			public LinkedValues(LinkedHashMap<K, V> outerInstance)
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
				return new LinkedValueIterator(OuterInstance);
			}
			public bool Contains(Object o)
			{
				return outerInstance.ContainsValue(o);
			}
			public Spliterator<V> Spliterator()
			{
				return Spliterators.Spliterator(this, Spliterator_Fields.SIZED | Spliterator_Fields.ORDERED);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final void forEach(java.util.function.Consumer<? base V> action)
			public void forEach<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				int mc = outerInstance.ModCount;
				for (LinkedHashMap.Entry<K, V> e = outerInstance.Head; e != Map_Fields.Null; e = e.After)
				{
					action.Accept(e.Value_Renamed);
				}
				if (outerInstance.ModCount != mc)
				{
					throw new ConcurrentModificationException();
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
		/// Its <seealso cref="Spliterator"/> typically provides faster sequential
		/// performance but much poorer parallel performance than that of
		/// {@code HashMap}.
		/// </summary>
		/// <returns> a set view of the mappings contained in this map </returns>
		public virtual Set<Map_Entry<K, V>> EntrySet()
		{
			Set<Map_Entry<K, V>> es;
			return (es = this.EntrySet_Renamed) == Map_Fields.Null ? (this.EntrySet_Renamed = new LinkedEntrySet(this)) : es;
		}

		internal sealed class LinkedEntrySet : AbstractSet<Map_Entry<K, V>>
		{
			private readonly LinkedHashMap<K, V> OuterInstance;

			public LinkedEntrySet(LinkedHashMap<K, V> outerInstance)
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
				return new LinkedEntryIterator(OuterInstance);
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
				return Spliterators.Spliterator(this, Spliterator_Fields.SIZED | Spliterator_Fields.ORDERED | Spliterator_Fields.DISTINCT);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final void forEach(java.util.function.Consumer<? base Map_Entry<K,V>> action)
			public void forEach<T1>(Consumer<T1> action)
			{
				if (action == Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				int mc = outerInstance.ModCount;
				for (LinkedHashMap.Entry<K, V> e = outerInstance.Head; e != Map_Fields.Null; e = e.After)
				{
					action.Accept(e);
				}
				if (outerInstance.ModCount != mc)
				{
					throw new ConcurrentModificationException();
				}
			}
		}

		// Map overrides

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
		public virtual void forEach<T1>(BiConsumer<T1> action)
		{
			if (action == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int mc = ModCount;
			for (LinkedHashMap.Entry<K, V> e = Head; e != Map_Fields.Null; e = e.After)
			{
				action.Accept(e.Key_Renamed, e.Value_Renamed);
			}
			if (ModCount != mc)
			{
				throw new ConcurrentModificationException();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
		public virtual void replaceAll<T1>(BiFunction<T1> function) where T1 : V
		{
			if (function == Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int mc = ModCount;
			for (LinkedHashMap.Entry<K, V> e = Head; e != Map_Fields.Null; e = e.After)
			{
				e.Value_Renamed = function.Apply(e.Key_Renamed, e.Value_Renamed);
			}
			if (ModCount != mc)
			{
				throw new ConcurrentModificationException();
			}
		}

		// Iterators

		internal abstract class LinkedHashIterator
		{
			private readonly LinkedHashMap<K, V> OuterInstance;

			internal LinkedHashMap.Entry<K, V> Next;
			internal LinkedHashMap.Entry<K, V> Current;
			internal int ExpectedModCount;

			internal LinkedHashIterator(LinkedHashMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
				Next = outerInstance.Head;
				ExpectedModCount = outerInstance.ModCount;
				Current = Map_Fields.Null;
			}

			public bool HasNext()
			{
				return Next != Map_Fields.Null;
			}

			internal LinkedHashMap.Entry<K, V> NextNode()
			{
				LinkedHashMap.Entry<K, V> e = Next;
				if (outerInstance.ModCount != ExpectedModCount)
				{
					throw new ConcurrentModificationException();
				}
				if (e == Map_Fields.Null)
				{
					throw new NoSuchElementException();
				}
				Current = e;
				Next = e.After;
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

		internal sealed class LinkedKeyIterator : LinkedHashIterator, Iterator<K>
		{
			private readonly LinkedHashMap<K, V> OuterInstance;

			public LinkedKeyIterator(LinkedHashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public K Next()
			{
				return NextNode().Key;
			}
		}

		internal sealed class LinkedValueIterator : LinkedHashIterator, Iterator<V>
		{
			private readonly LinkedHashMap<K, V> OuterInstance;

			public LinkedValueIterator(LinkedHashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public V Next()
			{
				return NextNode().Value_Renamed;
			}
		}

		internal sealed class LinkedEntryIterator : LinkedHashIterator, Iterator<Map_Entry<K, V>>
		{
			private readonly LinkedHashMap<K, V> OuterInstance;

			public LinkedEntryIterator(LinkedHashMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public Map_Entry<K, V> Next()
			{
				return NextNode();
			}
		}


	}

}