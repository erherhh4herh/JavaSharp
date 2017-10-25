using System;
using System.Collections;
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
	/// A scalable concurrent <seealso cref="ConcurrentNavigableMap"/> implementation.
	/// The map is sorted according to the {@link Comparable natural
	/// ordering} of its keys, or by a <seealso cref="Comparator"/> provided at map
	/// creation time, depending on which constructor is used.
	/// 
	/// <para>This class implements a concurrent variant of <a
	/// href="http://en.wikipedia.org/wiki/Skip_list" target="_top">SkipLists</a>
	/// providing expected average <i>log(n)</i> time cost for the
	/// {@code containsKey}, {@code get}, {@code put} and
	/// {@code remove} operations and their variants.  Insertion, removal,
	/// update, and access operations safely execute concurrently by
	/// multiple threads.
	/// 
	/// </para>
	/// <para>Iterators and spliterators are
	/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
	/// 
	/// </para>
	/// <para>Ascending key ordered views and their iterators are faster than
	/// descending ones.
	/// 
	/// </para>
	/// <para>All {@code Map.Entry} pairs returned by methods in this class
	/// and its views represent snapshots of mappings at the time they were
	/// produced. They do <em>not</em> support the {@code Entry.setValue}
	/// method. (Note however that it is possible to change mappings in the
	/// associated map using {@code put}, {@code putIfAbsent}, or
	/// {@code replace}, depending on exactly which effect you need.)
	/// 
	/// </para>
	/// <para>Beware that, unlike in most collections, the {@code size}
	/// method is <em>not</em> a constant-time operation. Because of the
	/// asynchronous nature of these maps, determining the current number
	/// of elements requires a traversal of the elements, and so may report
	/// inaccurate results if this collection is modified during traversal.
	/// Additionally, the bulk operations {@code putAll}, {@code equals},
	/// {@code toArray}, {@code containsValue}, and {@code clear} are
	/// <em>not</em> guaranteed to be performed atomically. For example, an
	/// iterator operating concurrently with a {@code putAll} operation
	/// might view only some of the added elements.
	/// 
	/// </para>
	/// <para>This class and its views and iterators implement all of the
	/// <em>optional</em> methods of the <seealso cref="Map"/> and <seealso cref="Iterator"/>
	/// interfaces. Like most other concurrent collections, this class does
	/// <em>not</em> permit the use of {@code null} keys or values because some
	/// null return values cannot be reliably distinguished from the absence of
	/// elements.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <K> the type of keys maintained by this map </param>
	/// @param <V> the type of mapped values
	/// @since 1.6 </param>
	[Serializable]
	public class ConcurrentSkipListMap<K, V> : AbstractMap<K, V>, ConcurrentNavigableMap<K, V>, Cloneable
	{
		/*
		 * This class implements a tree-like two-dimensionally linked skip
		 * list in which the index levels are represented in separate
		 * nodes from the base nodes holding data.  There are two reasons
		 * for taking this approach instead of the usual array-based
		 * structure: 1) Array based implementations seem to encounter
		 * more complexity and overhead 2) We can use cheaper algorithms
		 * for the heavily-traversed index lists than can be used for the
		 * base lists.  Here's a picture of some of the basics for a
		 * possible list with 2 levels of index:
		 *
		 * Head nodes          Index nodes
		 * +-+    right        +-+                      +-+
		 * |2|---------------->| |--------------------->| |->null
		 * +-+                 +-+                      +-+
		 *  | down              |                        |
		 *  v                   v                        v
		 * +-+            +-+  +-+       +-+            +-+       +-+
		 * |1|----------->| |->| |------>| |----------->| |------>| |->null
		 * +-+            +-+  +-+       +-+            +-+       +-+
		 *  v              |    |         |              |         |
		 * Nodes  next     v    v         v              v         v
		 * +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+
		 * | |->|A|->|B|->|C|->|D|->|E|->|F|->|G|->|H|->|I|->|J|->|K|->null
		 * +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+
		 *
		 * The base lists use a variant of the HM linked ordered set
		 * algorithm. See Tim Harris, "A pragmatic implementation of
		 * non-blocking linked lists"
		 * http://www.cl.cam.ac.uk/~tlh20/publications.html and Maged
		 * Michael "High Performance Dynamic Lock-Free Hash Tables and
		 * List-Based Sets"
		 * http://www.research.ibm.com/people/m/michael/pubs.htm.  The
		 * basic idea in these lists is to mark the "next" pointers of
		 * deleted nodes when deleting to avoid conflicts with concurrent
		 * insertions, and when traversing to keep track of triples
		 * (predecessor, node, successor) in order to detect when and how
		 * to unlink these deleted nodes.
		 *
		 * Rather than using mark-bits to mark list deletions (which can
		 * be slow and space-intensive using AtomicMarkedReference), nodes
		 * use direct CAS'able next pointers.  On deletion, instead of
		 * marking a pointer, they splice in another node that can be
		 * thought of as standing for a marked pointer (indicating this by
		 * using otherwise impossible field values).  Using plain nodes
		 * acts roughly like "boxed" implementations of marked pointers,
		 * but uses new nodes only when nodes are deleted, not for every
		 * link.  This requires less space and supports faster
		 * traversal. Even if marked references were better supported by
		 * JVMs, traversal using this technique might still be faster
		 * because any search need only read ahead one more node than
		 * otherwise required (to check for trailing marker) rather than
		 * unmasking mark bits or whatever on each read.
		 *
		 * This approach maintains the essential property needed in the HM
		 * algorithm of changing the next-pointer of a deleted node so
		 * that any other CAS of it will fail, but implements the idea by
		 * changing the pointer to point to a different node, not by
		 * marking it.  While it would be possible to further squeeze
		 * space by defining marker nodes not to have key/value fields, it
		 * isn't worth the extra type-testing overhead.  The deletion
		 * markers are rarely encountered during traversal and are
		 * normally quickly garbage collected. (Note that this technique
		 * would not work well in systems without garbage collection.)
		 *
		 * In addition to using deletion markers, the lists also use
		 * nullness of value fields to indicate deletion, in a style
		 * similar to typical lazy-deletion schemes.  If a node's value is
		 * null, then it is considered logically deleted and ignored even
		 * though it is still reachable. This maintains proper control of
		 * concurrent replace vs delete operations -- an attempted replace
		 * must fail if a delete beat it by nulling field, and a delete
		 * must return the last non-null value held in the field. (Note:
		 * Null, rather than some special marker, is used for value fields
		 * here because it just so happens to mesh with the Map API
		 * requirement that method get returns null if there is no
		 * mapping, which allows nodes to remain concurrently readable
		 * even when deleted. Using any other marker value here would be
		 * messy at best.)
		 *
		 * Here's the sequence of events for a deletion of node n with
		 * predecessor b and successor f, initially:
		 *
		 *        +------+       +------+      +------+
		 *   ...  |   b  |------>|   n  |----->|   f  | ...
		 *        +------+       +------+      +------+
		 *
		 * 1. CAS n's value field from non-null to null.
		 *    From this point on, no public operations encountering
		 *    the node consider this mapping to exist. However, other
		 *    ongoing insertions and deletions might still modify
		 *    n's next pointer.
		 *
		 * 2. CAS n's next pointer to point to a new marker node.
		 *    From this point on, no other nodes can be appended to n.
		 *    which avoids deletion errors in CAS-based linked lists.
		 *
		 *        +------+       +------+      +------+       +------+
		 *   ...  |   b  |------>|   n  |----->|marker|------>|   f  | ...
		 *        +------+       +------+      +------+       +------+
		 *
		 * 3. CAS b's next pointer over both n and its marker.
		 *    From this point on, no new traversals will encounter n,
		 *    and it can eventually be GCed.
		 *        +------+                                    +------+
		 *   ...  |   b  |----------------------------------->|   f  | ...
		 *        +------+                                    +------+
		 *
		 * A failure at step 1 leads to simple retry due to a lost race
		 * with another operation. Steps 2-3 can fail because some other
		 * thread noticed during a traversal a node with null value and
		 * helped out by marking and/or unlinking.  This helping-out
		 * ensures that no thread can become stuck waiting for progress of
		 * the deleting thread.  The use of marker nodes slightly
		 * complicates helping-out code because traversals must track
		 * consistent reads of up to four nodes (b, n, marker, f), not
		 * just (b, n, f), although the next field of a marker is
		 * immutable, and once a next field is CAS'ed to point to a
		 * marker, it never again changes, so this requires less care.
		 *
		 * Skip lists add indexing to this scheme, so that the base-level
		 * traversals start close to the locations being found, inserted
		 * or deleted -- usually base level traversals only traverse a few
		 * nodes. This doesn't change the basic algorithm except for the
		 * need to make sure base traversals start at predecessors (here,
		 * b) that are not (structurally) deleted, otherwise retrying
		 * after processing the deletion.
		 *
		 * Index levels are maintained as lists with volatile next fields,
		 * using CAS to link and unlink.  Races are allowed in index-list
		 * operations that can (rarely) fail to link in a new index node
		 * or delete one. (We can't do this of course for data nodes.)
		 * However, even when this happens, the index lists remain sorted,
		 * so correctly serve as indices.  This can impact performance,
		 * but since skip lists are probabilistic anyway, the net result
		 * is that under contention, the effective "p" value may be lower
		 * than its nominal value. And race windows are kept small enough
		 * that in practice these failures are rare, even under a lot of
		 * contention.
		 *
		 * The fact that retries (for both base and index lists) are
		 * relatively cheap due to indexing allows some minor
		 * simplifications of retry logic. Traversal restarts are
		 * performed after most "helping-out" CASes. This isn't always
		 * strictly necessary, but the implicit backoffs tend to help
		 * reduce other downstream failed CAS's enough to outweigh restart
		 * cost.  This worsens the worst case, but seems to improve even
		 * highly contended cases.
		 *
		 * Unlike most skip-list implementations, index insertion and
		 * deletion here require a separate traversal pass occurring after
		 * the base-level action, to add or remove index nodes.  This adds
		 * to single-threaded overhead, but improves contended
		 * multithreaded performance by narrowing interference windows,
		 * and allows deletion to ensure that all index nodes will be made
		 * unreachable upon return from a public remove operation, thus
		 * avoiding unwanted garbage retention. This is more important
		 * here than in some other data structures because we cannot null
		 * out node fields referencing user keys since they might still be
		 * read by other ongoing traversals.
		 *
		 * Indexing uses skip list parameters that maintain good search
		 * performance while using sparser-than-usual indices: The
		 * hardwired parameters k=1, p=0.5 (see method doPut) mean
		 * that about one-quarter of the nodes have indices. Of those that
		 * do, half have one level, a quarter have two, and so on (see
		 * Pugh's Skip List Cookbook, sec 3.4).  The expected total space
		 * requirement for a map is slightly less than for the current
		 * implementation of java.util.TreeMap.
		 *
		 * Changing the level of the index (i.e, the height of the
		 * tree-like structure) also uses CAS. The head index has initial
		 * level/height of one. Creation of an index with height greater
		 * than the current level adds a level to the head index by
		 * CAS'ing on a new top-most head. To maintain good performance
		 * after a lot of removals, deletion methods heuristically try to
		 * reduce the height if the topmost levels appear to be empty.
		 * This may encounter races in which it possible (but rare) to
		 * reduce and "lose" a level just as it is about to contain an
		 * index (that will then never be encountered). This does no
		 * structural harm, and in practice appears to be a better option
		 * than allowing unrestrained growth of levels.
		 *
		 * The code for all this is more verbose than you'd like. Most
		 * operations entail locating an element (or position to insert an
		 * element). The code to do this can't be nicely factored out
		 * because subsequent uses require a snapshot of predecessor
		 * and/or successor and/or value fields which can't be returned
		 * all at once, at least not without creating yet another object
		 * to hold them -- creating such little objects is an especially
		 * bad idea for basic internal search operations because it adds
		 * to GC overhead.  (This is one of the few times I've wished Java
		 * had macros.) Instead, some traversal code is interleaved within
		 * insertion and removal operations.  The control logic to handle
		 * all the retry conditions is sometimes twisty. Most search is
		 * broken into 2 parts. findPredecessor() searches index nodes
		 * only, returning a base-level predecessor of the key. findNode()
		 * finishes out the base-level search. Even with this factoring,
		 * there is a fair amount of near-duplication of code to handle
		 * variants.
		 *
		 * To produce random values without interference across threads,
		 * we use within-JDK thread local random support (via the
		 * "secondary seed", to avoid interference with user-level
		 * ThreadLocalRandom.)
		 *
		 * A previous version of this class wrapped non-comparable keys
		 * with their comparators to emulate Comparables when using
		 * comparators vs Comparables.  However, JVMs now appear to better
		 * handle infusing comparator-vs-comparable choice into search
		 * loops. Static method cpr(comparator, x, y) is used for all
		 * comparisons, which works well as long as the comparator
		 * argument is set up outside of loops (thus sometimes passed as
		 * an argument to internal methods) to avoid field re-reads.
		 *
		 * For explanation of algorithms sharing at least a couple of
		 * features with this one, see Mikhail Fomitchev's thesis
		 * (http://www.cs.yorku.ca/~mikhail/), Keir Fraser's thesis
		 * (http://www.cl.cam.ac.uk/users/kaf24/), and Hakan Sundell's
		 * thesis (http://www.cs.chalmers.se/~phs/).
		 *
		 * Given the use of tree-like index nodes, you might wonder why
		 * this doesn't use some kind of search tree instead, which would
		 * support somewhat faster search operations. The reason is that
		 * there are no known efficient lock-free insertion and deletion
		 * algorithms for search trees. The immutability of the "down"
		 * links of index nodes (as opposed to mutable "left" fields in
		 * true trees) makes this tractable using only CAS operations.
		 *
		 * Notation guide for local variables
		 * Node:         b, n, f    for  predecessor, node, successor
		 * Index:        q, r, d    for index node, right, down.
		 *               t          for another index node
		 * Head:         h
		 * Levels:       j
		 * Keys:         k, key
		 * Values:       v, value
		 * Comparisons:  c
		 */

		private const long SerialVersionUID = -8627078645895051609L;

		/// <summary>
		/// Special value used to identify base-level header
		/// </summary>
		private static readonly Object BASE_HEADER = new Object();

		/// <summary>
		/// The topmost head index of the skiplist.
		/// </summary>
		[NonSerialized]
		private volatile HeadIndex<K, V> Head;

		/// <summary>
		/// The comparator used to maintain order in this map, or null if
		/// using natural ordering.  (Non-private to simplify access in
		/// nested classes.)
		/// @serial
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.Comparator<? base K> comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		internal readonly IComparer<?> Comparator_Renamed;

		/// <summary>
		/// Lazily initialized key set </summary>
		[NonSerialized]
		private KeySet<K> KeySet_Renamed;
		/// <summary>
		/// Lazily initialized entry set </summary>
		[NonSerialized]
		private EntrySet<K, V> EntrySet_Renamed;
		/// <summary>
		/// Lazily initialized values collection </summary>
		[NonSerialized]
		private Values<V> Values_Renamed;
		/// <summary>
		/// Lazily initialized descending key set </summary>
		[NonSerialized]
		private ConcurrentNavigableMap<K, V> DescendingMap_Renamed;

		/// <summary>
		/// Initializes or resets state. Needed by constructors, clone,
		/// clear, readObject. and ConcurrentSkipListSet.clone.
		/// (Note that comparator must be separately initialized.)
		/// </summary>
		private void Initialize()
		{
			KeySet_Renamed = java.util.Map_Fields.Null;
			EntrySet_Renamed = java.util.Map_Fields.Null;
			Values_Renamed = java.util.Map_Fields.Null;
			DescendingMap_Renamed = java.util.Map_Fields.Null;
			Head = new HeadIndex<K, V>(new Node<K, V>(java.util.Map_Fields.Null, BASE_HEADER, java.util.Map_Fields.Null), java.util.Map_Fields.Null, java.util.Map_Fields.Null, 1);
		}

		/// <summary>
		/// compareAndSet head node
		/// </summary>
		private bool CasHead(HeadIndex<K, V> cmp, HeadIndex<K, V> val)
		{
			return UNSAFE.compareAndSwapObject(this, HeadOffset, cmp, val);
		}

		/* ---------------- Nodes -------------- */

		/// <summary>
		/// Nodes hold keys and values, and are singly linked in sorted
		/// order, possibly with some intervening marker nodes. The list is
		/// headed by a dummy node accessible as head.node. The value field
		/// is declared only as Object because it takes special non-V
		/// values for marker and header nodes.
		/// </summary>
		internal sealed class Node<K, V>
		{
			internal readonly K Key;
			internal volatile Object Value;
			internal volatile Node<K, V> Next;

			/// <summary>
			/// Creates a new regular node.
			/// </summary>
			internal Node(K key, Object value, Node<K, V> next)
			{
				this.Key = key;
				this.Value = value;
				this.Next = next;
			}

			/// <summary>
			/// Creates a new marker node. A marker is distinguished by
			/// having its value field point to itself.  Marker nodes also
			/// have null keys, a fact that is exploited in a few places,
			/// but this doesn't distinguish markers from the base-level
			/// header node (head.node), which also has a null key.
			/// </summary>
			internal Node(Node<K, V> next)
			{
				this.Key = java.util.Map_Fields.Null;
				this.Value = this;
				this.Next = next;
			}

			/// <summary>
			/// compareAndSet value field
			/// </summary>
			internal bool CasValue(Object cmp, Object val)
			{
				return UNSAFE.compareAndSwapObject(this, ValueOffset, cmp, val);
			}

			/// <summary>
			/// compareAndSet next field
			/// </summary>
			internal bool CasNext(Node<K, V> cmp, Node<K, V> val)
			{
				return UNSAFE.compareAndSwapObject(this, NextOffset, cmp, val);
			}

			/// <summary>
			/// Returns true if this node is a marker. This method isn't
			/// actually called in any current code checking for markers
			/// because callers will have already read value field and need
			/// to use that read (not another done here) and so directly
			/// test if value points to node.
			/// </summary>
			/// <returns> true if this node is a marker node </returns>
			internal bool Marker
			{
				get
				{
					return Value == this;
				}
			}

			/// <summary>
			/// Returns true if this node is the header of base-level list. </summary>
			/// <returns> true if this node is header node </returns>
			internal bool BaseHeader
			{
				get
				{
					return Value == BASE_HEADER;
				}
			}

			/// <summary>
			/// Tries to append a deletion marker to this node. </summary>
			/// <param name="f"> the assumed current successor of this node </param>
			/// <returns> true if successful </returns>
			internal bool AppendMarker(Node<K, V> f)
			{
				return CasNext(f, new Node<K, V>(f));
			}

			/// <summary>
			/// Helps out a deletion by appending marker or unlinking from
			/// predecessor. This is called during traversals when value
			/// field seen to be null. </summary>
			/// <param name="b"> predecessor </param>
			/// <param name="f"> successor </param>
			internal void HelpDelete(Node<K, V> b, Node<K, V> f)
			{
				/*
				 * Rechecking links and then doing only one of the
				 * help-out stages per call tends to minimize CAS
				 * interference among helping threads.
				 */
				if (f == Next && this == b.Next)
				{
					if (f == java.util.Map_Fields.Null || f.Value != f) // not already marked
					{
						CasNext(f, new Node<K, V>(f));
					}
					else
					{
						b.CasNext(this, f.Next);
					}
				}
			}

			/// <summary>
			/// Returns value if this node contains a valid key-value pair,
			/// else null. </summary>
			/// <returns> this node's value if it isn't a marker or header or
			/// is deleted, else null </returns>
			internal V ValidValue
			{
				get
				{
					Object java.util.Map_Fields.v = Value;
					if (java.util.Map_Fields.v == this || java.util.Map_Fields.v == BASE_HEADER)
					{
						return java.util.Map_Fields.Null;
					}
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
					V vv = (V)java.util.Map_Fields.v;
					return vv;
				}
			}

			/// <summary>
			/// Creates and returns a new SimpleImmutableEntry holding current
			/// mapping if this node holds a valid value, else null. </summary>
			/// <returns> new entry or null </returns>
			internal AbstractMap.SimpleImmutableEntry<K, V> CreateSnapshot()
			{
				Object java.util.Map_Fields.v = Value;
				if (java.util.Map_Fields.v == java.util.Map_Fields.Null || java.util.Map_Fields.v == this || java.util.Map_Fields.v == BASE_HEADER)
				{
					return java.util.Map_Fields.Null;
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
				V vv = (V)java.util.Map_Fields.v;
				return new AbstractMap.SimpleImmutableEntry<K, V>(Key, vv);
			}

			// UNSAFE mechanics

			internal static readonly sun.misc.Unsafe UNSAFE;
			internal static readonly long ValueOffset;
			internal static readonly long NextOffset;

			static Node()
			{
				try
				{
					UNSAFE = sun.misc.Unsafe.Unsafe;
					Class java.util.Map_Fields.k = typeof(Node);
					ValueOffset = UNSAFE.objectFieldOffset(java.util.Map_Fields.k.getDeclaredField("value"));
					NextOffset = UNSAFE.objectFieldOffset(java.util.Map_Fields.k.getDeclaredField("next"));
				}
				catch (Exception e)
				{
					throw new Error(e);
				}
			}
		}

		/* ---------------- Indexing -------------- */

		/// <summary>
		/// Index nodes represent the levels of the skip list.  Note that
		/// even though both Nodes and Indexes have forward-pointing
		/// fields, they have different types and are handled in different
		/// ways, that can't nicely be captured by placing field in a
		/// shared abstract class.
		/// </summary>
		internal class Index<K, V>
		{
			internal readonly Node<K, V> Node;
			internal readonly Index<K, V> Down;
			internal volatile Index<K, V> Right;

			/// <summary>
			/// Creates index node with given values.
			/// </summary>
			internal Index(Node<K, V> node, Index<K, V> down, Index<K, V> right)
			{
				this.Node = node;
				this.Down = down;
				this.Right = right;
			}

			/// <summary>
			/// compareAndSet right field
			/// </summary>
			internal bool CasRight(Index<K, V> cmp, Index<K, V> val)
			{
				return UNSAFE.compareAndSwapObject(this, RightOffset, cmp, val);
			}

			/// <summary>
			/// Returns true if the node this indexes has been deleted. </summary>
			/// <returns> true if indexed node is known to be deleted </returns>
			internal bool IndexesDeletedNode()
			{
				return Node.Value == java.util.Map_Fields.Null;
			}

			/// <summary>
			/// Tries to CAS newSucc as successor.  To minimize races with
			/// unlink that may lose this index node, if the node being
			/// indexed is known to be deleted, it doesn't try to link in. </summary>
			/// <param name="succ"> the expected current successor </param>
			/// <param name="newSucc"> the new successor </param>
			/// <returns> true if successful </returns>
			internal bool Link(Index<K, V> succ, Index<K, V> newSucc)
			{
				Node<K, V> n = Node;
				newSucc.Right = succ;
				return n.Value != java.util.Map_Fields.Null && CasRight(succ, newSucc);
			}

			/// <summary>
			/// Tries to CAS right field to skip over apparent successor
			/// succ.  Fails (forcing a retraversal by caller) if this node
			/// is known to be deleted. </summary>
			/// <param name="succ"> the expected current successor </param>
			/// <returns> true if successful </returns>
			internal bool Unlink(Index<K, V> succ)
			{
				return Node.Value != java.util.Map_Fields.Null && CasRight(succ, succ.Right);
			}

			// Unsafe mechanics
			internal static readonly sun.misc.Unsafe UNSAFE;
			internal static readonly long RightOffset;
			static Index()
			{
				try
				{
					UNSAFE = sun.misc.Unsafe.Unsafe;
					Class java.util.Map_Fields.k = typeof(Index);
					RightOffset = UNSAFE.objectFieldOffset(java.util.Map_Fields.k.getDeclaredField("right"));
				}
				catch (Exception e)
				{
					throw new Error(e);
				}
			}
		}

		/* ---------------- Head nodes -------------- */

		/// <summary>
		/// Nodes heading each level keep track of their level.
		/// </summary>
		internal sealed class HeadIndex<K, V> : Index<K, V>
		{
			internal readonly int Level;
			internal HeadIndex(Node<K, V> node, Index<K, V> down, Index<K, V> right, int level) : base(node, down, right)
			{
				this.Level = level;
			}
		}

		/* ---------------- Comparison utilities -------------- */

		/// <summary>
		/// Compares using comparator or natural ordering if null.
		/// Called only by methods that have performed required type checks.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) static final int cpr(java.util.Comparator c, Object x, Object y)
		internal static int Cpr(IComparer c, Object x, Object y)
		{
			return (c != java.util.Map_Fields.Null) ? c.Compare(x, y) : ((Comparable)x).CompareTo(y);
		}

		/* ---------------- Traversal -------------- */

		/// <summary>
		/// Returns a base-level node with key strictly less than given key,
		/// or the base-level header if there is no such node.  Also
		/// unlinks indexes to deleted nodes found along the way.  Callers
		/// rely on this side-effect of clearing indices to deleted nodes. </summary>
		/// <param name="key"> the key </param>
		/// <returns> a predecessor of key </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: private Node<K,V> findPredecessor(Object key, java.util.Comparator<? base K> cmp)
		private Node<K, V> FindPredecessor(Object key, IComparer<T1> cmp)
		{
			if (key == java.util.Map_Fields.Null)
			{
				throw new NullPointerException(); // don't postpone errors
			}
			for (;;)
			{
				for (Index<K, V> q = Head, r = q.Right, d;;)
				{
					if (r != java.util.Map_Fields.Null)
					{
						Node<K, V> n = r.node;
						K java.util.Map_Fields.k = n.Key;
						if (n.Value == java.util.Map_Fields.Null)
						{
							if (!q.Unlink(r))
							{
								break; // restart
							}
							r = q.Right; // reread r
							continue;
						}
						if (Cpr(cmp, key, java.util.Map_Fields.k) > 0)
						{
							q = r;
							r = r.right;
							continue;
						}
					}
					if ((d = q.down) == java.util.Map_Fields.Null)
					{
						return q.node;
					}
					q = d;
					r = d.right;
				}
			}
		}

		/// <summary>
		/// Returns node holding key or null if no such, clearing out any
		/// deleted nodes seen along the way.  Repeatedly traverses at
		/// base-level looking for key starting at predecessor returned
		/// from findPredecessor, processing base-level deletions as
		/// encountered. Some callers rely on this side-effect of clearing
		/// deleted nodes.
		/// 
		/// Restarts occur, at traversal step centered on node n, if:
		/// 
		///   (1) After reading n's next field, n is no longer assumed
		///       predecessor b's current successor, which means that
		///       we don't have a consistent 3-node snapshot and so cannot
		///       unlink any subsequent deleted nodes encountered.
		/// 
		///   (2) n's value field is null, indicating n is deleted, in
		///       which case we help out an ongoing structural deletion
		///       before retrying.  Even though there are cases where such
		///       unlinking doesn't require restart, they aren't sorted out
		///       here because doing so would not usually outweigh cost of
		///       restarting.
		/// 
		///   (3) n is a marker or n's predecessor's value field is null,
		///       indicating (among other possibilities) that
		///       findPredecessor returned a deleted node. We can't unlink
		///       the node because we don't know its predecessor, so rely
		///       on another call to findPredecessor to notice and return
		///       some earlier predecessor, which it will do. This check is
		///       only strictly needed at beginning of loop, (and the
		///       b.value check isn't strictly needed at all) but is done
		///       each iteration to help avoid contention with other
		///       threads by callers that will fail to be able to change
		///       links, and so will retry anyway.
		/// 
		/// The traversal loops in doPut, doRemove, and findNear all
		/// include the same three kinds of checks. And specialized
		/// versions appear in findFirst, and findLast and their
		/// variants. They can't easily share code because each uses the
		/// reads of fields held in locals occurring in the orders they
		/// were performed.
		/// </summary>
		/// <param name="key"> the key </param>
		/// <returns> node holding key, or null if no such </returns>
		private Node<K, V> FindNode(Object key)
		{
			if (key == java.util.Map_Fields.Null)
			{
				throw new NullPointerException(); // don't postpone errors
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IComparer<?> cmp = Comparator_Renamed;
			for (;;)
			{
				for (Node<K, V> b = FindPredecessor(key, cmp), n = b.Next;;)
				{
					Object java.util.Map_Fields.v;
					int c;
					if (n == java.util.Map_Fields.Null)
					{
						goto outerBreak;
					}
					Node<K, V> f = n.next;
					if (n != b.next) // inconsistent read
					{
						break;
					}
					if ((java.util.Map_Fields.v = n.value) == java.util.Map_Fields.Null) // n is deleted
					{
						n.helpDelete(b, f);
						break;
					}
					if (b.value == java.util.Map_Fields.Null || java.util.Map_Fields.v == n) // b is deleted
					{
						break;
					}
					if ((c = Cpr(cmp, key, n.key)) == 0)
					{
						return n;
					}
					if (c < 0)
					{
						goto outerBreak;
					}
					b = n;
					n = f;
				}
				outerContinue:;
			}
			outerBreak:
			return java.util.Map_Fields.Null;
		}

		/// <summary>
		/// Gets value for key. Almost the same as findNode, but returns
		/// the found value (to avoid retries during re-reads)
		/// </summary>
		/// <param name="key"> the key </param>
		/// <returns> the value, or null if absent </returns>
		private V DoGet(Object key)
		{
			if (key == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IComparer<?> cmp = Comparator_Renamed;
			for (;;)
			{
				for (Node<K, V> b = FindPredecessor(key, cmp), n = b.Next;;)
				{
					Object java.util.Map_Fields.v;
					int c;
					if (n == java.util.Map_Fields.Null)
					{
						goto outerBreak;
					}
					Node<K, V> f = n.next;
					if (n != b.next) // inconsistent read
					{
						break;
					}
					if ((java.util.Map_Fields.v = n.value) == java.util.Map_Fields.Null) // n is deleted
					{
						n.helpDelete(b, f);
						break;
					}
					if (b.value == java.util.Map_Fields.Null || java.util.Map_Fields.v == n) // b is deleted
					{
						break;
					}
					if ((c = Cpr(cmp, key, n.key)) == 0)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
						V vv = (V)java.util.Map_Fields.v;
						return vv;
					}
					if (c < 0)
					{
						goto outerBreak;
					}
					b = n;
					n = f;
				}
				outerContinue:;
			}
			outerBreak:
			return java.util.Map_Fields.Null;
		}

		/* ---------------- Insertion -------------- */

		/// <summary>
		/// Main insertion method.  Adds element if not present, or
		/// replaces value if present and onlyIfAbsent is false. </summary>
		/// <param name="key"> the key </param>
		/// <param name="value"> the value that must be associated with key </param>
		/// <param name="onlyIfAbsent"> if should not insert if already present </param>
		/// <returns> the old value, or null if newly inserted </returns>
		private V DoPut(K key, V value, bool onlyIfAbsent)
		{
			Node<K, V> z; // added node
			if (key == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IComparer<?> cmp = Comparator_Renamed;
			for (;;)
			{
				for (Node<K, V> b = FindPredecessor(key, cmp), n = b.Next;;)
				{
					if (n != java.util.Map_Fields.Null)
					{
						Object java.util.Map_Fields.v;
						int c;
						Node<K, V> f = n.next;
						if (n != b.Next) // inconsistent read
						{
							break;
						}
						if ((java.util.Map_Fields.v = n.value) == java.util.Map_Fields.Null) // n is deleted
						{
							n.helpDelete(b, f);
							break;
						}
						if (b.Value == java.util.Map_Fields.Null || java.util.Map_Fields.v == n) // b is deleted
						{
							break;
						}
						if ((c = Cpr(cmp, key, n.key)) > 0)
						{
							b = n;
							n = f;
							continue;
						}
						if (c == 0)
						{
							if (onlyIfAbsent || n.casValue(java.util.Map_Fields.v, value))
							{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
								V vv = (V)java.util.Map_Fields.v;
								return vv;
							}
							break; // restart if lost race to replace value
						}
						// else c < 0; fall through
					}

					z = new Node<K, V>(key, value, n);
					if (!b.casNext(n, z))
					{
						break; // restart if lost race to append to b
					}
					goto outerBreak;
				}
				outerContinue:;
			}
			outerBreak:

			int rnd = ThreadLocalRandom.NextSecondarySeed();
			if ((rnd & 0x80000001) == 0) // test highest and lowest bits
			{
				int level = 1, max ;
				while (((rnd = (int)((uint)rnd >> 1)) & 1) != 0)
				{
					++level;
				}
				Index<K, V> idx = java.util.Map_Fields.Null;
				HeadIndex<K, V> h = Head;
				if (level <= (max = h.Level))
				{
					for (int i = 1; i <= level; ++i)
					{
						idx = new Index<K, V>(z, idx, java.util.Map_Fields.Null);
					}
				}
				else // try to grow by one level
				{
					level = max + 1; // hold in array and later pick the one to use
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked")Index<K,V>[] idxs = (Index<K,V>[])new Index<?,?>[level+1];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Index<K, V>[] idxs = (Index<K, V>[])new Index<?, ?>[level + 1];
					for (int i = 1; i <= level; ++i)
					{
						idxs[i] = idx = new Index<K, V>(z, idx, java.util.Map_Fields.Null);
					}
					for (;;)
					{
						h = Head;
						int oldLevel = h.Level;
						if (level <= oldLevel) // lost race to add level
						{
							break;
						}
						HeadIndex<K, V> newh = h;
						Node<K, V> oldbase = h.Node;
						for (int j = oldLevel + 1; j <= level; ++j)
						{
							newh = new HeadIndex<K, V>(oldbase, newh, idxs[j], j);
						}
						if (CasHead(h, newh))
						{
							h = newh;
							idx = idxs[level = oldLevel];
							break;
						}
					}
				}
				// find insertion points and splice in
				for (int insertionLevel = level;;)
				{
					int j = h.Level;
					for (Index<K, V> q = h, r = q.Right, t = idx;;)
					{
						if (q == java.util.Map_Fields.Null || t == java.util.Map_Fields.Null)
						{
							goto spliceBreak;
						}
						if (r != java.util.Map_Fields.Null)
						{
							Node<K, V> n = r.node;
							// compare before deletion check avoids needing recheck
							int c = Cpr(cmp, key, n.Key);
							if (n.Value == java.util.Map_Fields.Null)
							{
								if (!q.unlink(r))
								{
									break;
								}
								r = q.right;
								continue;
							}
							if (c > 0)
							{
								q = r;
								r = r.right;
								continue;
							}
						}

						if (j == insertionLevel)
						{
							if (!q.link(r, t))
							{
								break; // restart
							}
							if (t.node.value == java.util.Map_Fields.Null)
							{
								FindNode(key);
								goto spliceBreak;
							}
							if (--insertionLevel == 0)
							{
								goto spliceBreak;
							}
						}

						if (--j >= insertionLevel && j < level)
						{
							t = t.down;
						}
						q = q.down;
						r = q.right;
					}
					spliceContinue:;
				}
				spliceBreak:;
			}
			return java.util.Map_Fields.Null;
		}

		/* ---------------- Deletion -------------- */

		/// <summary>
		/// Main deletion method. Locates node, nulls value, appends a
		/// deletion marker, unlinks predecessor, removes associated index
		/// nodes, and possibly reduces head index level.
		/// 
		/// Index nodes are cleared out simply by calling findPredecessor.
		/// which unlinks indexes to deleted nodes found along path to key,
		/// which will include the indexes to this node.  This is done
		/// unconditionally. We can't check beforehand whether there are
		/// index nodes because it might be the case that some or all
		/// indexes hadn't been inserted yet for this node during initial
		/// search for it, and we'd like to ensure lack of garbage
		/// retention, so must call to be sure.
		/// </summary>
		/// <param name="key"> the key </param>
		/// <param name="value"> if non-null, the value that must be
		/// associated with key </param>
		/// <returns> the node, or null if not found </returns>
		internal V DoRemove(Object key, Object value)
		{
			if (key == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IComparer<?> cmp = Comparator_Renamed;
			for (;;)
			{
				for (Node<K, V> b = FindPredecessor(key, cmp), n = b.Next;;)
				{
					Object java.util.Map_Fields.v;
					int c;
					if (n == java.util.Map_Fields.Null)
					{
						goto outerBreak;
					}
					Node<K, V> f = n.next;
					if (n != b.next) // inconsistent read
					{
						break;
					}
					if ((java.util.Map_Fields.v = n.value) == java.util.Map_Fields.Null) // n is deleted
					{
						n.helpDelete(b, f);
						break;
					}
					if (b.value == java.util.Map_Fields.Null || java.util.Map_Fields.v == n) // b is deleted
					{
						break;
					}
					if ((c = Cpr(cmp, key, n.key)) < 0)
					{
						goto outerBreak;
					}
					if (c > 0)
					{
						b = n;
						n = f;
						continue;
					}
					if (value != java.util.Map_Fields.Null && !value.Equals(java.util.Map_Fields.v))
					{
						goto outerBreak;
					}
					if (!n.casValue(java.util.Map_Fields.v, java.util.Map_Fields.Null))
					{
						break;
					}
					if (!n.appendMarker(f) || !b.casNext(n, f))
					{
						FindNode(key); // retry via findNode
					}
					else
					{
						FindPredecessor(key, cmp); // clean index
						if (Head.Right == java.util.Map_Fields.Null)
						{
							TryReduceLevel();
						}
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
					V vv = (V)java.util.Map_Fields.v;
					return vv;
				}
				outerContinue:;
			}
			outerBreak:
			return java.util.Map_Fields.Null;
		}

		/// <summary>
		/// Possibly reduce head level if it has no nodes.  This method can
		/// (rarely) make mistakes, in which case levels can disappear even
		/// though they are about to contain index nodes. This impacts
		/// performance, not correctness.  To minimize mistakes as well as
		/// to reduce hysteresis, the level is reduced by one only if the
		/// topmost three levels look empty. Also, if the removed level
		/// looks non-empty after CAS, we try to change it back quick
		/// before anyone notices our mistake! (This trick works pretty
		/// well because this method will practically never make mistakes
		/// unless current thread stalls immediately before first CAS, in
		/// which case it is very unlikely to stall again immediately
		/// afterwards, so will recover.)
		/// 
		/// We put up with all this rather than just let levels grow
		/// because otherwise, even a small map that has undergone a large
		/// number of insertions and removals will have a lot of levels,
		/// slowing down access more than would an occasional unwanted
		/// reduction.
		/// </summary>
		private void TryReduceLevel()
		{
			HeadIndex<K, V> h = Head;
			HeadIndex<K, V> d;
			HeadIndex<K, V> e;
			if (h.Level > 3 && (d = (HeadIndex<K, V>)h.Down) != java.util.Map_Fields.Null && (e = (HeadIndex<K, V>)d.Down) != java.util.Map_Fields.Null && e.Right == java.util.Map_Fields.Null && d.Right == java.util.Map_Fields.Null && h.Right == java.util.Map_Fields.Null && CasHead(h, d) && h.Right != java.util.Map_Fields.Null) // recheck -  try to set
			{
				CasHead(d, h); // try to backout
			}
		}

		/* ---------------- Finding and removing first element -------------- */

		/// <summary>
		/// Specialized variant of findNode to get first valid node. </summary>
		/// <returns> first node or null if empty </returns>
		internal Node<K, V> FindFirst()
		{
			for (Node<K, V> b, n;;)
			{
				if ((n = (b = Head.Node).next) == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.Null;
				}
				if (n.value != java.util.Map_Fields.Null)
				{
					return n;
				}
				n.helpDelete(b, n.next);
			}
		}

		/// <summary>
		/// Removes first entry; returns its snapshot. </summary>
		/// <returns> null if empty, else snapshot of first entry </returns>
		private java.util.Map_Entry<K, V> DoRemoveFirstEntry()
		{
			for (Node<K, V> b, n;;)
			{
				if ((n = (b = Head.Node).next) == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.Null;
				}
				Node<K, V> f = n.next;
				if (n != b.next)
				{
					continue;
				}
				Object java.util.Map_Fields.v = n.value;
				if (java.util.Map_Fields.v == java.util.Map_Fields.Null)
				{
					n.helpDelete(b, f);
					continue;
				}
				if (!n.casValue(java.util.Map_Fields.v, java.util.Map_Fields.Null))
				{
					continue;
				}
				if (!n.appendMarker(f) || !b.casNext(n, f))
				{
					FindFirst(); // retry
				}
				ClearIndexToFirst();
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
				V vv = (V)java.util.Map_Fields.v;
				return new AbstractMap.SimpleImmutableEntry<K, V>(n.key, vv);
			}
		}

		/// <summary>
		/// Clears out index nodes associated with deleted first entry.
		/// </summary>
		private void ClearIndexToFirst()
		{
			for (;;)
			{
				for (Index<K, V> q = Head;;)
				{
					Index<K, V> r = q.Right;
					if (r != java.util.Map_Fields.Null && r.IndexesDeletedNode() && !q.Unlink(r))
					{
						break;
					}
					if ((q = q.down) == java.util.Map_Fields.Null)
					{
						if (Head.Right == java.util.Map_Fields.Null)
						{
							TryReduceLevel();
						}
						return;
					}
				}
			}
		}

		/// <summary>
		/// Removes last entry; returns its snapshot.
		/// Specialized variant of doRemove. </summary>
		/// <returns> null if empty, else snapshot of last entry </returns>
		private java.util.Map_Entry<K, V> DoRemoveLastEntry()
		{
			for (;;)
			{
				Node<K, V> b = FindPredecessorOfLast();
				Node<K, V> n = b.Next;
				if (n == java.util.Map_Fields.Null)
				{
					if (b.BaseHeader) // empty
					{
						return java.util.Map_Fields.Null;
					}
					else
					{
						continue; // all b's successors are deleted; retry
					}
				}
				for (;;)
				{
					Node<K, V> f = n.Next;
					if (n != b.Next) // inconsistent read
					{
						break;
					}
					Object java.util.Map_Fields.v = n.Value;
					if (java.util.Map_Fields.v == java.util.Map_Fields.Null) // n is deleted
					{
						n.HelpDelete(b, f);
						break;
					}
					if (b.Value == java.util.Map_Fields.Null || java.util.Map_Fields.v == n) // b is deleted
					{
						break;
					}
					if (f != java.util.Map_Fields.Null)
					{
						b = n;
						n = f;
						continue;
					}
					if (!n.CasValue(java.util.Map_Fields.v, java.util.Map_Fields.Null))
					{
						break;
					}
					K key = n.Key;
					if (!n.AppendMarker(f) || !b.CasNext(n, f))
					{
						FindNode(key); // retry via findNode
					}
					else // clean index
					{
						FindPredecessor(key, Comparator_Renamed);
						if (Head.Right == java.util.Map_Fields.Null)
						{
							TryReduceLevel();
						}
					}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
					V vv = (V)java.util.Map_Fields.v;
					return new AbstractMap.SimpleImmutableEntry<K, V>(key, vv);
				}
			}
		}

		/* ---------------- Finding and removing last element -------------- */

		/// <summary>
		/// Specialized version of find to get last valid node. </summary>
		/// <returns> last node or null if empty </returns>
		internal Node<K, V> FindLast()
		{
			/*
			 * findPredecessor can't be used to traverse index level
			 * because this doesn't use comparisons.  So traversals of
			 * both levels are folded together.
			 */
			Index<K, V> q = Head;
			for (;;)
			{
				Index<K, V> d, r;
				if ((r = q.Right) != java.util.Map_Fields.Null)
				{
					if (r.IndexesDeletedNode())
					{
						q.Unlink(r);
						q = Head; // restart
					}
					else
					{
						q = r;
					}
				}
				else if ((d = q.Down) != java.util.Map_Fields.Null)
				{
					q = d;
				}
				else
				{
					for (Node<K, V> b = q.Node, n = b.Next;;)
					{
						if (n == java.util.Map_Fields.Null)
						{
							return b.BaseHeader ? java.util.Map_Fields.Null : b;
						}
						Node<K, V> f = n.next; // inconsistent read
						if (n != b.next)
						{
							break;
						}
						Object java.util.Map_Fields.v = n.value;
						if (java.util.Map_Fields.v == java.util.Map_Fields.Null) // n is deleted
						{
							n.helpDelete(b, f);
							break;
						}
						if (b.value == java.util.Map_Fields.Null || java.util.Map_Fields.v == n) // b is deleted
						{
							break;
						}
						b = n;
						n = f;
					}
					q = Head; // restart
				}
			}
		}

		/// <summary>
		/// Specialized variant of findPredecessor to get predecessor of last
		/// valid node.  Needed when removing the last entry.  It is possible
		/// that all successors of returned node will have been deleted upon
		/// return, in which case this method can be retried. </summary>
		/// <returns> likely predecessor of last node </returns>
		private Node<K, V> FindPredecessorOfLast()
		{
			for (;;)
			{
				for (Index<K, V> q = Head;;)
				{
					Index<K, V> d, r;
					if ((r = q.Right) != java.util.Map_Fields.Null)
					{
						if (r.IndexesDeletedNode())
						{
							q.Unlink(r);
							break; // must restart
						}
						// proceed as far across as possible without overshooting
						if (r.Node.Next != java.util.Map_Fields.Null)
						{
							q = r;
							continue;
						}
					}
					if ((d = q.down) != java.util.Map_Fields.Null)
					{
						q = d;
					}
					else
					{
						return q.node;
					}
				}
			}
		}

		/* ---------------- Relational operations -------------- */

		// Control values OR'ed as arguments to findNear

		private const int EQ = 1;
		private const int LT = 2;
		private const int GT = 0; // Actually checked as !LT

		/// <summary>
		/// Utility for ceiling, floor, lower, higher methods. </summary>
		/// <param name="key"> the key </param>
		/// <param name="rel"> the relation -- OR'ed combination of EQ, LT, GT </param>
		/// <returns> nearest node fitting relation, or null if no such </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final Node<K,V> findNear(K key, int rel, java.util.Comparator<? base K> cmp)
		internal Node<K, V> FindNear(K key, int rel, IComparer<T1> cmp)
		{
			if (key == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			for (;;)
			{
				for (Node<K, V> b = FindPredecessor(key, cmp), n = b.Next;;)
				{
					Object java.util.Map_Fields.v;
					if (n == java.util.Map_Fields.Null)
					{
						return ((rel & LT) == 0 || b.BaseHeader) ? java.util.Map_Fields.Null : b;
					}
					Node<K, V> f = n.next;
					if (n != b.next) // inconsistent read
					{
						break;
					}
					if ((java.util.Map_Fields.v = n.value) == java.util.Map_Fields.Null) // n is deleted
					{
						n.helpDelete(b, f);
						break;
					}
					if (b.value == java.util.Map_Fields.Null || java.util.Map_Fields.v == n) // b is deleted
					{
						break;
					}
					int c = Cpr(cmp, key, n.key);
					if ((c == 0 && (rel & EQ) != 0) || (c < 0 && (rel & LT) == 0))
					{
						return n;
					}
					if (c <= 0 && (rel & LT) != 0)
					{
						return b.BaseHeader ? java.util.Map_Fields.Null : b;
					}
					b = n;
					n = f;
				}
			}
		}

		/// <summary>
		/// Returns SimpleImmutableEntry for results of findNear. </summary>
		/// <param name="key"> the key </param>
		/// <param name="rel"> the relation -- OR'ed combination of EQ, LT, GT </param>
		/// <returns> Entry fitting relation, or null if no such </returns>
		internal AbstractMap.SimpleImmutableEntry<K, V> GetNear(K key, int rel)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IComparer<?> cmp = Comparator_Renamed;
			for (;;)
			{
				Node<K, V> n = FindNear(key, rel, cmp);
				if (n == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.Null;
				}
				AbstractMap.SimpleImmutableEntry<K, V> e = n.CreateSnapshot();
				if (e != java.util.Map_Fields.Null)
				{
					return e;
				}
			}
		}

		/* ---------------- Constructors -------------- */

		/// <summary>
		/// Constructs a new, empty map, sorted according to the
		/// <seealso cref="Comparable natural ordering"/> of the keys.
		/// </summary>
		public ConcurrentSkipListMap()
		{
			this.Comparator_Renamed = java.util.Map_Fields.Null;
			Initialize();
		}

		/// <summary>
		/// Constructs a new, empty map, sorted according to the specified
		/// comparator.
		/// </summary>
		/// <param name="comparator"> the comparator that will be used to order this map.
		///        If {@code null}, the {@link Comparable natural
		///        ordering} of the keys will be used. </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public ConcurrentSkipListMap(java.util.Comparator<? base K> comparator)
		public ConcurrentSkipListMap<T1>(IComparer<T1> comparator)
		{
			this.Comparator_Renamed = comparator;
			Initialize();
		}

		/// <summary>
		/// Constructs a new map containing the same mappings as the given map,
		/// sorted according to the <seealso cref="Comparable natural ordering"/> of
		/// the keys.
		/// </summary>
		/// <param name="m"> the map whose mappings are to be placed in this map </param>
		/// <exception cref="ClassCastException"> if the keys in {@code m} are not
		///         <seealso cref="Comparable"/>, or are not mutually comparable </exception>
		/// <exception cref="NullPointerException"> if the specified map or any of its keys
		///         or values are null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ConcurrentSkipListMap(java.util.Map<? extends K, ? extends V> m)
		public ConcurrentSkipListMap<T1>(IDictionary<T1> m) where T1 : K where ? : V
		{
			this.Comparator_Renamed = java.util.Map_Fields.Null;
			Initialize();
			PutAll(m);
		}

		/// <summary>
		/// Constructs a new map containing the same mappings and using the
		/// same ordering as the specified sorted map.
		/// </summary>
		/// <param name="m"> the sorted map whose mappings are to be placed in this
		///        map, and whose comparator is to be used to sort this map </param>
		/// <exception cref="NullPointerException"> if the specified sorted map or any of
		///         its keys or values are null </exception>
		public ConcurrentSkipListMap<T1>(SortedMap<T1> m) where T1 : V
		{
			this.Comparator_Renamed = m.Comparator();
			Initialize();
			BuildFromSorted(m);
		}

		/// <summary>
		/// Returns a shallow copy of this {@code ConcurrentSkipListMap}
		/// instance. (The keys and values themselves are not cloned.)
		/// </summary>
		/// <returns> a shallow copy of this map </returns>
		public virtual ConcurrentSkipListMap<K, V> Clone()
		{
			try
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ConcurrentSkipListMap<K,V> clone = (ConcurrentSkipListMap<K,V>) base.clone();
				ConcurrentSkipListMap<K, V> clone = (ConcurrentSkipListMap<K, V>) base.Clone();
				clone.Initialize();
				clone.BuildFromSorted(this);
				return clone;
			}
			catch (CloneNotSupportedException)
			{
				throw new InternalError();
			}
		}

		/// <summary>
		/// Streamlined bulk insertion to initialize from elements of
		/// given sorted map.  Call only from constructor or clone
		/// method.
		/// </summary>
		private void buildFromSorted<T1>(SortedMap<T1> map) where T1 : V
		{
			if (map == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}

			HeadIndex<K, V> h = Head;
			Node<K, V> basepred = h.Node;

			// Track the current rightmost node at each level. Uses an
			// ArrayList to avoid committing to initial or maximum level.
			List<Index<K, V>> preds = new List<Index<K, V>>();

			// initialize
			for (int i = 0; i <= h.Level; ++i)
			{
				preds.Add(java.util.Map_Fields.Null);
			}
			Index<K, V> q = h;
			for (int i = h.Level; i > 0; --i)
			{
				preds.Set(i, q);
				q = q.Down;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<? extends java.util.Map_Entry<? extends K, ? extends V>> it = map.entrySet().iterator();
			IEnumerator<?> it = map.GetEnumerator();
			while (it.MoveNext())
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map_Entry<? extends K, ? extends V> e = it.Current;
				java.util.Map_Entry<?, ?> e = it.Current;
				int rnd = ThreadLocalRandom.Current().NextInt();
				int j = 0;
				if ((rnd & 0x80000001) == 0)
				{
					do
					{
						++j;
					} while (((rnd = (int)((uint)rnd >> 1)) & 1) != 0);
					if (j > h.Level)
					{
						j = h.Level + 1;
					}
				}
				K java.util.Map_Fields.k = e.Key;
				V java.util.Map_Fields.v = e.Value;
				if (java.util.Map_Fields.k == java.util.Map_Fields.Null || java.util.Map_Fields.v == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				Node<K, V> z = new Node<K, V>(java.util.Map_Fields.k, java.util.Map_Fields.v, java.util.Map_Fields.Null);
				basepred.Next = z;
				basepred = z;
				if (j > 0)
				{
					Index<K, V> idx = java.util.Map_Fields.Null;
					for (int i = 1; i <= j; ++i)
					{
						idx = new Index<K, V>(z, idx, java.util.Map_Fields.Null);
						if (i > h.Level)
						{
							h = new HeadIndex<K, V>(h.Node, h, idx, i);
						}

						if (i < preds.Size())
						{
							preds.Get(i).Right = idx;
							preds.Set(i, idx);
						}
						else
						{
							preds.Add(idx);
						}
					}
				}
			}
			Head = h;
		}

		/* ---------------- Serialization -------------- */

		/// <summary>
		/// Saves this map to a stream (that is, serializes it).
		/// </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs
		/// @serialData The key (Object) and value (Object) for each
		/// key-value mapping represented by the map, followed by
		/// {@code null}. The key-value mappings are emitted in key-order
		/// (as determined by the Comparator, or by the keys' natural
		/// ordering if no Comparator). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// Write out the Comparator and any hidden stuff
			s.DefaultWriteObject();

			// Write out keys and values (alternating)
			for (Node<K, V> n = FindFirst(); n != java.util.Map_Fields.Null; n = n.Next)
			{
				V java.util.Map_Fields.v = n.ValidValue;
				if (java.util.Map_Fields.v != java.util.Map_Fields.Null)
				{
					s.WriteObject(n.Key);
					s.WriteObject(java.util.Map_Fields.v);
				}
			}
			s.WriteObject(java.util.Map_Fields.Null);
		}

		/// <summary>
		/// Reconstitutes this map from a stream (that is, deserializes it). </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized object
		///         could not be found </exception>
		/// <exception cref="java.io.IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void readObject(final java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in the Comparator and any hidden stuff
			s.DefaultReadObject();
			// Reset transients
			Initialize();

			/*
			 * This is nearly identical to buildFromSorted, but is
			 * distinct because readObject calls can't be nicely adapted
			 * as the kind of iterator needed by buildFromSorted. (They
			 * can be, but doing so requires type cheats and/or creation
			 * of adaptor classes.) It is simpler to just adapt the code.
			 */

			HeadIndex<K, V> h = Head;
			Node<K, V> basepred = h.Node;
			List<Index<K, V>> preds = new List<Index<K, V>>();
			for (int i = 0; i <= h.Level; ++i)
			{
				preds.Add(java.util.Map_Fields.Null);
			}
			Index<K, V> q = h;
			for (int i = h.Level; i > 0; --i)
			{
				preds.Set(i, q);
				q = q.Down;
			}

			for (;;)
			{
				Object java.util.Map_Fields.k = s.ReadObject();
				if (java.util.Map_Fields.k == java.util.Map_Fields.Null)
				{
					break;
				}
				Object java.util.Map_Fields.v = s.ReadObject();
				if (java.util.Map_Fields.v == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				K key = (K) java.util.Map_Fields.k;
				V val = (V) java.util.Map_Fields.v;
				int rnd = ThreadLocalRandom.Current().NextInt();
				int j = 0;
				if ((rnd & 0x80000001) == 0)
				{
					do
					{
						++j;
					} while (((rnd = (int)((uint)rnd >> 1)) & 1) != 0);
					if (j > h.Level)
					{
						j = h.Level + 1;
					}
				}
				Node<K, V> z = new Node<K, V>(key, val, java.util.Map_Fields.Null);
				basepred.Next = z;
				basepred = z;
				if (j > 0)
				{
					Index<K, V> idx = java.util.Map_Fields.Null;
					for (int i = 1; i <= j; ++i)
					{
						idx = new Index<K, V>(z, idx, java.util.Map_Fields.Null);
						if (i > h.Level)
						{
							h = new HeadIndex<K, V>(h.Node, h, idx, i);
						}

						if (i < preds.Size())
						{
							preds.Get(i).Right = idx;
							preds.Set(i, idx);
						}
						else
						{
							preds.Add(idx);
						}
					}
				}
			}
			Head = h;
		}

		/* ------ Map API methods ------ */

		/// <summary>
		/// Returns {@code true} if this map contains a mapping for the specified
		/// key.
		/// </summary>
		/// <param name="key"> key whose presence in this map is to be tested </param>
		/// <returns> {@code true} if this map contains a mapping for the specified key </returns>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual bool ContainsKey(Object key)
		{
			return DoGet(key) != java.util.Map_Fields.Null;
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
		/// </summary>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual V Get(Object key)
		{
			return DoGet(key);
		}

		/// <summary>
		/// Returns the value to which the specified key is mapped,
		/// or the given defaultValue if this map contains no mapping for the key.
		/// </summary>
		/// <param name="key"> the key </param>
		/// <param name="defaultValue"> the value to return if this map contains
		/// no mapping for the given key </param>
		/// <returns> the mapping for the key, if present; else the defaultValue </returns>
		/// <exception cref="NullPointerException"> if the specified key is null
		/// @since 1.8 </exception>
		public virtual V GetOrDefault(Object key, V defaultValue)
		{
			V java.util.Map_Fields.v;
			return (java.util.Map_Fields.v = DoGet(key)) == java.util.Map_Fields.Null ? defaultValue : java.util.Map_Fields.v;
		}

		/// <summary>
		/// Associates the specified value with the specified key in this map.
		/// If the map previously contained a mapping for the key, the old
		/// value is replaced.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="value"> value to be associated with the specified key </param>
		/// <returns> the previous value associated with the specified key, or
		///         {@code null} if there was no mapping for the key </returns>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key or value is null </exception>
		public virtual V Put(K key, V value)
		{
			if (value == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return DoPut(key, value, java.util.Map_Fields.False);
		}

		/// <summary>
		/// Removes the mapping for the specified key from this map if present.
		/// </summary>
		/// <param name="key"> key for which mapping should be removed </param>
		/// <returns> the previous value associated with the specified key, or
		///         {@code null} if there was no mapping for the key </returns>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual V Remove(Object key)
		{
			return DoRemove(key, java.util.Map_Fields.Null);
		}

		/// <summary>
		/// Returns {@code true} if this map maps one or more keys to the
		/// specified value.  This operation requires time linear in the
		/// map size. Additionally, it is possible for the map to change
		/// during execution of this method, in which case the returned
		/// result may be inaccurate.
		/// </summary>
		/// <param name="value"> value whose presence in this map is to be tested </param>
		/// <returns> {@code true} if a mapping to {@code value} exists;
		///         {@code false} otherwise </returns>
		/// <exception cref="NullPointerException"> if the specified value is null </exception>
		public virtual bool ContainsValue(Object value)
		{
			if (value == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			for (Node<K, V> n = FindFirst(); n != java.util.Map_Fields.Null; n = n.Next)
			{
				V java.util.Map_Fields.v = n.ValidValue;
				if (java.util.Map_Fields.v != java.util.Map_Fields.Null && value.Equals(java.util.Map_Fields.v))
				{
					return java.util.Map_Fields.True;
				}
			}
			return java.util.Map_Fields.False;
		}

		/// <summary>
		/// Returns the number of key-value mappings in this map.  If this map
		/// contains more than {@code Integer.MAX_VALUE} elements, it
		/// returns {@code Integer.MAX_VALUE}.
		/// 
		/// <para>Beware that, unlike in most collections, this method is
		/// <em>NOT</em> a constant-time operation. Because of the
		/// asynchronous nature of these maps, determining the current
		/// number of elements requires traversing them all to count them.
		/// Additionally, it is possible for the size to change during
		/// execution of this method, in which case the returned result
		/// will be inaccurate. Thus, this method is typically not very
		/// useful in concurrent applications.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the number of elements in this map </returns>
		public virtual int Size()
		{
			long count = 0;
			for (Node<K, V> n = FindFirst(); n != java.util.Map_Fields.Null; n = n.Next)
			{
				if (n.ValidValue != java.util.Map_Fields.Null)
				{
					++count;
				}
			}
			return (count >= Integer.MaxValue) ? Integer.MaxValue : (int) count;
		}

		/// <summary>
		/// Returns {@code true} if this map contains no key-value mappings. </summary>
		/// <returns> {@code true} if this map contains no key-value mappings </returns>
		public virtual bool Empty
		{
			get
			{
				return FindFirst() == java.util.Map_Fields.Null;
			}
		}

		/// <summary>
		/// Removes all of the mappings from this map.
		/// </summary>
		public virtual void Clear()
		{
			Initialize();
		}

		/// <summary>
		/// If the specified key is not already associated with a value,
		/// attempts to compute its value using the given mapping function
		/// and enters it into this map unless {@code null}.  The function
		/// is <em>NOT</em> guaranteed to be applied once atomically only
		/// if the value is not present.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="mappingFunction"> the function to compute a value </param>
		/// <returns> the current (existing or computed) value associated with
		///         the specified key, or null if the computed value is null </returns>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         or the mappingFunction is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
		public virtual V computeIfAbsent<T1>(K key, Function<T1> mappingFunction) where T1 : V
		{
			if (key == java.util.Map_Fields.Null || mappingFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			V java.util.Map_Fields.v, p, r;
			if ((java.util.Map_Fields.v = DoGet(key)) == java.util.Map_Fields.Null && (r = mappingFunction.Apply(key)) != java.util.Map_Fields.Null)
			{
				java.util.Map_Fields.v = (p = DoPut(key, r, java.util.Map_Fields.True)) == java.util.Map_Fields.Null ? r : p;
			}
			return java.util.Map_Fields.v;
		}

		/// <summary>
		/// If the value for the specified key is present, attempts to
		/// compute a new mapping given the key and its current mapped
		/// value. The function is <em>NOT</em> guaranteed to be applied
		/// once atomically.
		/// </summary>
		/// <param name="key"> key with which a value may be associated </param>
		/// <param name="remappingFunction"> the function to compute a value </param>
		/// <returns> the new value associated with the specified key, or null if none </returns>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         or the remappingFunction is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
		public virtual V computeIfPresent<T1>(K key, BiFunction<T1> remappingFunction) where T1 : V
		{
			if (key == java.util.Map_Fields.Null || remappingFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			Node<K, V> n;
			Object java.util.Map_Fields.v;
			while ((n = FindNode(key)) != java.util.Map_Fields.Null)
			{
				if ((java.util.Map_Fields.v = n.Value) != java.util.Map_Fields.Null)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V) java.util.Map_Fields.v;
					V vv = (V) java.util.Map_Fields.v;
					V r = remappingFunction.Apply(key, vv);
					if (r != java.util.Map_Fields.Null)
					{
						if (n.CasValue(vv, r))
						{
							return r;
						}
					}
					else if (DoRemove(key, vv) != java.util.Map_Fields.Null)
					{
						break;
					}
				}
			}
			return java.util.Map_Fields.Null;
		}

		/// <summary>
		/// Attempts to compute a mapping for the specified key and its
		/// current mapped value (or {@code null} if there is no current
		/// mapping). The function is <em>NOT</em> guaranteed to be applied
		/// once atomically.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="remappingFunction"> the function to compute a value </param>
		/// <returns> the new value associated with the specified key, or null if none </returns>
		/// <exception cref="NullPointerException"> if the specified key is null
		///         or the remappingFunction is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
		public virtual V compute<T1>(K key, BiFunction<T1> remappingFunction) where T1 : V
		{
			if (key == java.util.Map_Fields.Null || remappingFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			for (;;)
			{
				Node<K, V> n;
				Object java.util.Map_Fields.v;
				V r;
				if ((n = FindNode(key)) == java.util.Map_Fields.Null)
				{
					if ((r = remappingFunction.Apply(key, java.util.Map_Fields.Null)) == java.util.Map_Fields.Null)
					{
						break;
					}
					if (DoPut(key, r, java.util.Map_Fields.True) == java.util.Map_Fields.Null)
					{
						return r;
					}
				}
				else if ((java.util.Map_Fields.v = n.Value) != java.util.Map_Fields.Null)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V) java.util.Map_Fields.v;
					V vv = (V) java.util.Map_Fields.v;
					if ((r = remappingFunction.Apply(key, vv)) != java.util.Map_Fields.Null)
					{
						if (n.CasValue(vv, r))
						{
							return r;
						}
					}
					else if (DoRemove(key, vv) != java.util.Map_Fields.Null)
					{
						break;
					}
				}
			}
			return java.util.Map_Fields.Null;
		}

		/// <summary>
		/// If the specified key is not already associated with a value,
		/// associates it with the given value.  Otherwise, replaces the
		/// value with the results of the given remapping function, or
		/// removes if {@code null}. The function is <em>NOT</em>
		/// guaranteed to be applied once atomically.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="value"> the value to use if absent </param>
		/// <param name="remappingFunction"> the function to recompute a value if present </param>
		/// <returns> the new value associated with the specified key, or null if none </returns>
		/// <exception cref="NullPointerException"> if the specified key or value is null
		///         or the remappingFunction is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V merge(K key, V value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
		public virtual V merge<T1>(K key, V value, BiFunction<T1> remappingFunction) where T1 : V
		{
			if (key == java.util.Map_Fields.Null || value == java.util.Map_Fields.Null || remappingFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			for (;;)
			{
				Node<K, V> n;
				Object java.util.Map_Fields.v;
				V r;
				if ((n = FindNode(key)) == java.util.Map_Fields.Null)
				{
					if (DoPut(key, value, java.util.Map_Fields.True) == java.util.Map_Fields.Null)
					{
						return value;
					}
				}
				else if ((java.util.Map_Fields.v = n.Value) != java.util.Map_Fields.Null)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V) java.util.Map_Fields.v;
					V vv = (V) java.util.Map_Fields.v;
					if ((r = remappingFunction.Apply(vv, value)) != java.util.Map_Fields.Null)
					{
						if (n.CasValue(vv, r))
						{
							return r;
						}
					}
					else if (DoRemove(key, vv) != java.util.Map_Fields.Null)
					{
						return java.util.Map_Fields.Null;
					}
				}
			}
		}

		/* ---------------- View methods -------------- */

		/*
		 * Note: Lazy initialization works for views because view classes
		 * are stateless/immutable so it doesn't matter wrt correctness if
		 * more than one is created (which will only rarely happen).  Even
		 * so, the following idiom conservatively ensures that the method
		 * returns the one it created if it does so, not one created by
		 * another racing thread.
		 */

		/// <summary>
		/// Returns a <seealso cref="NavigableSet"/> view of the keys contained in this map.
		/// 
		/// <para>The set's iterator returns the keys in ascending order.
		/// The set's spliterator additionally reports <seealso cref="Spliterator#CONCURRENT"/>,
		/// <seealso cref="Spliterator#NONNULL"/>, <seealso cref="Spliterator#SORTED"/> and
		/// <seealso cref="Spliterator#ORDERED"/>, with an encounter order that is ascending
		/// key order.  The spliterator's comparator (see
		/// <seealso cref="java.util.Spliterator#getComparator()"/>) is {@code null} if
		/// the map's comparator (see <seealso cref="#comparator()"/>) is {@code null}.
		/// Otherwise, the spliterator's comparator is the same as or imposes the
		/// same total ordering as the map's comparator.
		/// 
		/// </para>
		/// <para>The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  The set supports element
		/// removal, which removes the corresponding mapping from the map,
		/// via the {@code Iterator.remove}, {@code Set.remove},
		/// {@code removeAll}, {@code retainAll}, and {@code clear}
		/// operations.  It does not support the {@code add} or {@code addAll}
		/// operations.
		/// 
		/// </para>
		/// <para>The view's iterators and spliterators are
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// <para>This method is equivalent to method {@code navigableKeySet}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a navigable set view of the keys in this map </returns>
		public virtual NavigableSet<K> KeySet()
		{
			KeySet<K> ks = KeySet_Renamed;
			return (ks != java.util.Map_Fields.Null) ? ks : (KeySet_Renamed = new KeySet<K>(this));
		}

		public virtual NavigableSet<K> NavigableKeySet()
		{
			KeySet<K> ks = KeySet_Renamed;
			return (ks != java.util.Map_Fields.Null) ? ks : (KeySet_Renamed = new KeySet<K>(this));
		}

		/// <summary>
		/// Returns a <seealso cref="Collection"/> view of the values contained in this map.
		/// <para>The collection's iterator returns the values in ascending order
		/// of the corresponding keys. The collections's spliterator additionally
		/// reports <seealso cref="Spliterator#CONCURRENT"/>, <seealso cref="Spliterator#NONNULL"/> and
		/// <seealso cref="Spliterator#ORDERED"/>, with an encounter order that is ascending
		/// order of the corresponding keys.
		/// 
		/// </para>
		/// <para>The collection is backed by the map, so changes to the map are
		/// reflected in the collection, and vice-versa.  The collection
		/// supports element removal, which removes the corresponding
		/// mapping from the map, via the {@code Iterator.remove},
		/// {@code Collection.remove}, {@code removeAll},
		/// {@code retainAll} and {@code clear} operations.  It does not
		/// support the {@code add} or {@code addAll} operations.
		/// 
		/// </para>
		/// <para>The view's iterators and spliterators are
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// </para>
		/// </summary>
		public virtual ICollection<V> Values()
		{
			Values<V> vs = Values_Renamed;
			return (vs != java.util.Map_Fields.Null) ? vs : (Values_Renamed = new Values<V>(this));
		}

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		/// 
		/// <para>The set's iterator returns the entries in ascending key order.  The
		/// set's spliterator additionally reports <seealso cref="Spliterator#CONCURRENT"/>,
		/// <seealso cref="Spliterator#NONNULL"/>, <seealso cref="Spliterator#SORTED"/> and
		/// <seealso cref="Spliterator#ORDERED"/>, with an encounter order that is ascending
		/// key order.
		/// 
		/// </para>
		/// <para>The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  The set supports element
		/// removal, which removes the corresponding mapping from the map,
		/// via the {@code Iterator.remove}, {@code Set.remove},
		/// {@code removeAll}, {@code retainAll} and {@code clear}
		/// operations.  It does not support the {@code add} or
		/// {@code addAll} operations.
		/// 
		/// </para>
		/// <para>The view's iterators and spliterators are
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// <para>The {@code Map.Entry} elements traversed by the {@code iterator}
		/// or {@code spliterator} do <em>not</em> support the {@code setValue}
		/// operation.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a set view of the mappings contained in this map,
		///         sorted in ascending key order </returns>
		public virtual Set<java.util.Map_Entry<K, V>> EntrySet()
		{
			EntrySet<K, V> es = EntrySet_Renamed;
			return (es != java.util.Map_Fields.Null) ? es : (EntrySet_Renamed = new EntrySet<K, V>(this));
		}

		public virtual ConcurrentNavigableMap<K, V> DescendingMap()
		{
			ConcurrentNavigableMap<K, V> dm = DescendingMap_Renamed;
			return (dm != java.util.Map_Fields.Null) ? dm : (DescendingMap_Renamed = new SubMap<K, V> (this, java.util.Map_Fields.Null, java.util.Map_Fields.False, java.util.Map_Fields.Null, java.util.Map_Fields.False, java.util.Map_Fields.True));
		}

		public virtual NavigableSet<K> DescendingKeySet()
		{
			return DescendingMap().NavigableKeySet();
		}

		/* ---------------- AbstractMap Overrides -------------- */

		/// <summary>
		/// Compares the specified object with this map for equality.
		/// Returns {@code true} if the given object is also a map and the
		/// two maps represent the same mappings.  More formally, two maps
		/// {@code m1} and {@code m2} represent the same mappings if
		/// {@code m1.entrySet().equals(m2.entrySet())}.  This
		/// operation may return misleading results if either map is
		/// concurrently modified during execution of this method.
		/// </summary>
		/// <param name="o"> object to be compared for equality with this map </param>
		/// <returns> {@code true} if the specified object is equal to this map </returns>
		public override bool Equals(Object o)
		{
			if (o == this)
			{
				return java.util.Map_Fields.True;
			}
			if (!(o is IDictionary))
			{
				return java.util.Map_Fields.False;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<?,?> m = (java.util.Map<?,?>) o;
			IDictionary<?, ?> m = (IDictionary<?, ?>) o;
			try
			{
				foreach (java.util.Map_Entry<K, V> e in this.EntrySet())
				{
					if (!e.Value.Equals(m[e.Key]))
					{
						return java.util.Map_Fields.False;
					}
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.util.Map_Entry<?,?> e : m.entrySet())
				foreach (java.util.Map_Entry<?, ?> e in m)
				{
					Object java.util.Map_Fields.k = e.Key;
					Object java.util.Map_Fields.v = e.Value;
					if (java.util.Map_Fields.k == java.util.Map_Fields.Null || java.util.Map_Fields.v == java.util.Map_Fields.Null || !java.util.Map_Fields.v.Equals(Get(java.util.Map_Fields.k)))
					{
						return java.util.Map_Fields.False;
					}
				}
				return java.util.Map_Fields.True;
			}
			catch (ClassCastException)
			{
				return java.util.Map_Fields.False;
			}
			catch (NullPointerException)
			{
				return java.util.Map_Fields.False;
			}
		}

		/* ------ ConcurrentMap API methods ------ */

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <returns> the previous value associated with the specified key,
		///         or {@code null} if there was no mapping for the key </returns>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key or value is null </exception>
		public virtual V PutIfAbsent(K key, V value)
		{
			if (value == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return DoPut(key, value, java.util.Map_Fields.True);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual bool Remove(Object key, Object value)
		{
			if (key == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return value != java.util.Map_Fields.Null && DoRemove(key, value) != java.util.Map_Fields.Null;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if any of the arguments are null </exception>
		public virtual bool Replace(K key, V java, V java)
		{
			if (key == java.util.Map_Fields.Null || java.util.Map_Fields.OldValue == java.util.Map_Fields.Null || java.util.Map_Fields.NewValue == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			for (;;)
			{
				Node<K, V> n;
				Object java.util.Map_Fields.v;
				if ((n = FindNode(key)) == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.False;
				}
				if ((java.util.Map_Fields.v = n.Value) != java.util.Map_Fields.Null)
				{
					if (!java.util.Map_Fields.OldValue.Equals(java.util.Map_Fields.v))
					{
						return java.util.Map_Fields.False;
					}
					if (n.CasValue(java.util.Map_Fields.v, java.util.Map_Fields.NewValue))
					{
						return java.util.Map_Fields.True;
					}
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <returns> the previous value associated with the specified key,
		///         or {@code null} if there was no mapping for the key </returns>
		/// <exception cref="ClassCastException"> if the specified key cannot be compared
		///         with the keys currently in the map </exception>
		/// <exception cref="NullPointerException"> if the specified key or value is null </exception>
		public virtual V Replace(K key, V value)
		{
			if (key == java.util.Map_Fields.Null || value == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			for (;;)
			{
				Node<K, V> n;
				Object java.util.Map_Fields.v;
				if ((n = FindNode(key)) == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.Null;
				}
				if ((java.util.Map_Fields.v = n.Value) != java.util.Map_Fields.Null && n.CasValue(java.util.Map_Fields.v, value))
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
					V vv = (V)java.util.Map_Fields.v;
					return vv;
				}
			}
		}

		/* ------ SortedMap API methods ------ */

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public java.util.Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public virtual IComparer<?> Comparator()
		{
			return Comparator_Renamed;
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual K FirstKey()
		{
			Node<K, V> n = FindFirst();
			if (n == java.util.Map_Fields.Null)
			{
				throw new NoSuchElementException();
			}
			return n.Key;
		}

		/// <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		public virtual K LastKey()
		{
			Node<K, V> n = FindLast();
			if (n == java.util.Map_Fields.Null)
			{
				throw new NoSuchElementException();
			}
			return n.Key;
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromKey} or {@code toKey} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual ConcurrentNavigableMap<K, V> SubMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive)
		{
			if (fromKey == java.util.Map_Fields.Null || toKey == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return new SubMap<K, V> (this, fromKey, fromInclusive, toKey, toInclusive, java.util.Map_Fields.False);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code toKey} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual ConcurrentNavigableMap<K, V> HeadMap(K toKey, bool inclusive)
		{
			if (toKey == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return new SubMap<K, V> (this, java.util.Map_Fields.Null, java.util.Map_Fields.False, toKey, inclusive, java.util.Map_Fields.False);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromKey} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual ConcurrentNavigableMap<K, V> TailMap(K fromKey, bool inclusive)
		{
			if (fromKey == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return new SubMap<K, V> (this, fromKey, inclusive, java.util.Map_Fields.Null, java.util.Map_Fields.False, java.util.Map_Fields.False);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromKey} or {@code toKey} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual ConcurrentNavigableMap<K, V> SubMap(K fromKey, K toKey)
		{
			return SubMap(fromKey, java.util.Map_Fields.True, toKey, java.util.Map_Fields.False);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code toKey} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual ConcurrentNavigableMap<K, V> HeadMap(K toKey)
		{
			return HeadMap(toKey, java.util.Map_Fields.False);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if {@code fromKey} is null </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		public virtual ConcurrentNavigableMap<K, V> TailMap(K fromKey)
		{
			return TailMap(fromKey, java.util.Map_Fields.True);
		}

		/* ---------------- Relational operations -------------- */

		/// <summary>
		/// Returns a key-value mapping associated with the greatest key
		/// strictly less than the given key, or {@code null} if there is
		/// no such key. The returned entry does <em>not</em> support the
		/// {@code Entry.setValue} method.
		/// </summary>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual java.util.Map_Entry<K, V> LowerEntry(K key)
		{
			return GetNear(key, LT);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual K LowerKey(K key)
		{
			Node<K, V> n = FindNear(key, LT, Comparator_Renamed);
			return (n == java.util.Map_Fields.Null) ? java.util.Map_Fields.Null : n.Key;
		}

		/// <summary>
		/// Returns a key-value mapping associated with the greatest key
		/// less than or equal to the given key, or {@code null} if there
		/// is no such key. The returned entry does <em>not</em> support
		/// the {@code Entry.setValue} method.
		/// </summary>
		/// <param name="key"> the key </param>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual java.util.Map_Entry<K, V> FloorEntry(K key)
		{
			return GetNear(key, LT | EQ);
		}

		/// <param name="key"> the key </param>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual K FloorKey(K key)
		{
			Node<K, V> n = FindNear(key, LT | EQ, Comparator_Renamed);
			return (n == java.util.Map_Fields.Null) ? java.util.Map_Fields.Null : n.Key;
		}

		/// <summary>
		/// Returns a key-value mapping associated with the least key
		/// greater than or equal to the given key, or {@code null} if
		/// there is no such entry. The returned entry does <em>not</em>
		/// support the {@code Entry.setValue} method.
		/// </summary>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual java.util.Map_Entry<K, V> CeilingEntry(K key)
		{
			return GetNear(key, GT | EQ);
		}

		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual K CeilingKey(K key)
		{
			Node<K, V> n = FindNear(key, GT | EQ, Comparator_Renamed);
			return (n == java.util.Map_Fields.Null) ? java.util.Map_Fields.Null : n.Key;
		}

		/// <summary>
		/// Returns a key-value mapping associated with the least key
		/// strictly greater than the given key, or {@code null} if there
		/// is no such key. The returned entry does <em>not</em> support
		/// the {@code Entry.setValue} method.
		/// </summary>
		/// <param name="key"> the key </param>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual java.util.Map_Entry<K, V> HigherEntry(K key)
		{
			return GetNear(key, GT);
		}

		/// <param name="key"> the key </param>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual K HigherKey(K key)
		{
			Node<K, V> n = FindNear(key, GT, Comparator_Renamed);
			return (n == java.util.Map_Fields.Null) ? java.util.Map_Fields.Null : n.Key;
		}

		/// <summary>
		/// Returns a key-value mapping associated with the least
		/// key in this map, or {@code null} if the map is empty.
		/// The returned entry does <em>not</em> support
		/// the {@code Entry.setValue} method.
		/// </summary>
		public virtual java.util.Map_Entry<K, V> FirstEntry()
		{
			for (;;)
			{
				Node<K, V> n = FindFirst();
				if (n == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.Null;
				}
				AbstractMap.SimpleImmutableEntry<K, V> e = n.CreateSnapshot();
				if (e != java.util.Map_Fields.Null)
				{
					return e;
				}
			}
		}

		/// <summary>
		/// Returns a key-value mapping associated with the greatest
		/// key in this map, or {@code null} if the map is empty.
		/// The returned entry does <em>not</em> support
		/// the {@code Entry.setValue} method.
		/// </summary>
		public virtual java.util.Map_Entry<K, V> LastEntry()
		{
			for (;;)
			{
				Node<K, V> n = FindLast();
				if (n == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.Null;
				}
				AbstractMap.SimpleImmutableEntry<K, V> e = n.CreateSnapshot();
				if (e != java.util.Map_Fields.Null)
				{
					return e;
				}
			}
		}

		/// <summary>
		/// Removes and returns a key-value mapping associated with
		/// the least key in this map, or {@code null} if the map is empty.
		/// The returned entry does <em>not</em> support
		/// the {@code Entry.setValue} method.
		/// </summary>
		public virtual java.util.Map_Entry<K, V> PollFirstEntry()
		{
			return DoRemoveFirstEntry();
		}

		/// <summary>
		/// Removes and returns a key-value mapping associated with
		/// the greatest key in this map, or {@code null} if the map is empty.
		/// The returned entry does <em>not</em> support
		/// the {@code Entry.setValue} method.
		/// </summary>
		public virtual java.util.Map_Entry<K, V> PollLastEntry()
		{
			return DoRemoveLastEntry();
		}


		/* ---------------- Iterators -------------- */

		/// <summary>
		/// Base of iterator classes:
		/// </summary>
		internal abstract class Iter<T> : Iterator<T>
		{
			private readonly ConcurrentSkipListMap<K, V> OuterInstance;

			/// <summary>
			/// the last node returned by next() </summary>
			internal Node<K, V> LastReturned;
			/// <summary>
			/// the next node to return from next(); </summary>
			internal Node<K, V> Next;
			/// <summary>
			/// Cache of next value field to maintain weak consistency </summary>
			internal V NextValue;

			/// <summary>
			/// Initializes ascending iterator for entire range. </summary>
			internal Iter(ConcurrentSkipListMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
				while ((Next = outerInstance.FindFirst()) != java.util.Map_Fields.Null)
				{
					Object x = Next.Value;
					if (x != java.util.Map_Fields.Null && x != Next)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)x;
						V vv = (V)x;
						NextValue = vv;
						break;
					}
				}
			}

			public bool HasNext()
			{
				return Next != java.util.Map_Fields.Null;
			}

			/// <summary>
			/// Advances next to higher entry. </summary>
			internal void Advance()
			{
				if (Next == java.util.Map_Fields.Null)
				{
					throw new NoSuchElementException();
				}
				LastReturned = Next;
				while ((Next = Next.Next) != java.util.Map_Fields.Null)
				{
					Object x = Next.Value;
					if (x != java.util.Map_Fields.Null && x != Next)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)x;
						V vv = (V)x;
						NextValue = vv;
						break;
					}
				}
			}

			public virtual void Remove()
			{
				Node<K, V> l = LastReturned;
				if (l == java.util.Map_Fields.Null)
				{
					throw new IllegalStateException();
				}
				// It would not be worth all of the overhead to directly
				// unlink from here. Using remove is fast enough.
				OuterInstance.Remove(l.Key);
				LastReturned = java.util.Map_Fields.Null;
			}

		}

		internal sealed class ValueIterator : Iter<V>
		{
			private readonly ConcurrentSkipListMap<K, V> OuterInstance;

			public ValueIterator(ConcurrentSkipListMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public V Next()
			{
				V java.util.Map_Fields.v = NextValue;
				Advance();
				return java.util.Map_Fields.v;
			}
		}

		internal sealed class KeyIterator : Iter<K>
		{
			private readonly ConcurrentSkipListMap<K, V> OuterInstance;

			public KeyIterator(ConcurrentSkipListMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public K Next()
			{
				Node<K, V> n = Next;
				Advance();
				return n.Key;
			}
		}

		internal sealed class EntryIterator : Iter<java.util.Map_Entry<K, V>>
		{
			private readonly ConcurrentSkipListMap<K, V> OuterInstance;

			public EntryIterator(ConcurrentSkipListMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public java.util.Map_Entry<K, V> Next()
			{
				Node<K, V> n = Next;
				V java.util.Map_Fields.v = NextValue;
				Advance();
				return new AbstractMap.SimpleImmutableEntry<K, V>(n.Key, java.util.Map_Fields.v);
			}
		}

		// Factory methods for iterators needed by ConcurrentSkipListSet etc

		internal virtual IEnumerator<K> KeyIterator()
		{
			return new KeyIterator(this);
		}

		internal virtual IEnumerator<V> ValueIterator()
		{
			return new ValueIterator(this);
		}

		internal virtual IEnumerator<java.util.Map_Entry<K, V>> EntryIterator()
		{
			return new EntryIterator(this);
		}

		/* ---------------- View Classes -------------- */

		/*
		 * View classes are static, delegating to a ConcurrentNavigableMap
		 * to allow use by SubMaps, which outweighs the ugliness of
		 * needing type-tests for Iterator methods.
		 */

		internal static IList<E> toList<E>(ICollection<E> c)
		{
			// Using size() here would be a pessimization.
			List<E> list = new List<E>();
			foreach (E e in c)
			{
				list.Add(e);
			}
			return list;
		}

		internal sealed class KeySet<E> : AbstractSet<E>, NavigableSet<E>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final java.util.concurrent.ConcurrentNavigableMap<E,?> m;
			internal readonly ConcurrentNavigableMap<E, ?> m;
			internal KeySet<T1>(ConcurrentNavigableMap<T1> map)
			{
				m = map;
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
			public bool Remove(Object o)
			{
				return m.Remove(o) != java.util.Map_Fields.Null;
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
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public java.util.Comparator<? base E> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public IComparer<?> Comparator()
			{
				return m.Comparator();
			}
			public E First()
			{
				return m.FirstKey();
			}
			public E Last()
			{
				return m.LastKey();
			}
			public E PollFirst()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map_Entry<E,?> e = m.pollFirstEntry();
				java.util.Map_Entry<E, ?> e = m.PollFirstEntry();
				return (e == java.util.Map_Fields.Null) ? java.util.Map_Fields.Null : e.Key;
			}
			public E PollLast()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map_Entry<E,?> e = m.pollLastEntry();
				java.util.Map_Entry<E, ?> e = m.PollLastEntry();
				return (e == java.util.Map_Fields.Null) ? java.util.Map_Fields.Null : e.Key;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Iterator<E> iterator()
			public IEnumerator<E> Iterator()
			{
				if (m is ConcurrentSkipListMap)
				{
					return ((ConcurrentSkipListMap<E, Object>)m).KeyIterator();
				}
				else
				{
					return ((ConcurrentSkipListMap.SubMap<E, Object>)m).KeyIterator();
				}
			}
			public override bool Equals(Object o)
			{
				if (o == this)
				{
					return java.util.Map_Fields.True;
				}
				if (!(o is Set))
				{
					return java.util.Map_Fields.False;
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
					return java.util.Map_Fields.False;
				}
				catch (NullPointerException)
				{
					return java.util.Map_Fields.False;
				}
			}
			public Object[] ToArray()
			{
				return ToList(this).ToArray();
			}
			public T[] toArray<T>(T[] a)
			{
				return ToList(this).toArray(a);
			}
			public IEnumerator<E> DescendingIterator()
			{
				return DescendingSet().Iterator();
			}
			public NavigableSet<E> SubSet(E fromElement, bool fromInclusive, E toElement, bool toInclusive)
			{
				return new KeySet<E>(m.SubMap(fromElement, fromInclusive, toElement, toInclusive));
			}
			public NavigableSet<E> HeadSet(E toElement, bool inclusive)
			{
				return new KeySet<E>(m.HeadMap(toElement, inclusive));
			}
			public NavigableSet<E> TailSet(E fromElement, bool inclusive)
			{
				return new KeySet<E>(m.TailMap(fromElement, inclusive));
			}
			public NavigableSet<E> SubSet(E fromElement, E toElement)
			{
				return SubSet(fromElement, java.util.Map_Fields.True, toElement, java.util.Map_Fields.False);
			}
			public NavigableSet<E> HeadSet(E toElement)
			{
				return HeadSet(toElement, java.util.Map_Fields.False);
			}
			public NavigableSet<E> TailSet(E fromElement)
			{
				return TailSet(fromElement, java.util.Map_Fields.True);
			}
			public NavigableSet<E> DescendingSet()
			{
				return new KeySet<E>(m.DescendingMap());
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Spliterator<E> spliterator()
			public Spliterator<E> Spliterator()
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
//ORIGINAL LINE: return (java.util.Spliterator<E>)((SubMap<E,?>)m).keyIterator();
					return (Spliterator<E>)((SubMap<E, ?>)m).KeyIterator();
				}
			}
		}

		internal sealed class Values<E> : AbstractCollection<E>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final java.util.concurrent.ConcurrentNavigableMap<?, E> m;
			internal readonly ConcurrentNavigableMap<?, E> m;
			internal Values<T1>(ConcurrentNavigableMap<T1> map)
			{
				m = map;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Iterator<E> iterator()
			public IEnumerator<E> Iterator()
			{
				if (m is ConcurrentSkipListMap)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((ConcurrentSkipListMap<?,E>)m).valueIterator();
					return ((ConcurrentSkipListMap<?, E>)m).ValueIterator();
				}
				else
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((SubMap<?,E>)m).valueIterator();
					return ((SubMap<?, E>)m).ValueIterator();
				}
			}
			public bool Empty
			{
				get
				{
					return m.Empty;
				}
			}
			public int Size()
			{
				return m.Size();
			}
			public bool Contains(Object o)
			{
				return m.ContainsValue(o);
			}
			public void Clear()
			{
				m.Clear();
			}
			public Object[] ToArray()
			{
				return ToList(this).ToArray();
			}
			public T[] toArray<T>(T[] a)
			{
				return ToList(this).toArray(a);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Spliterator<E> spliterator()
			public Spliterator<E> Spliterator()
			{
				if (m is ConcurrentSkipListMap)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((ConcurrentSkipListMap<?,E>)m).valueSpliterator();
					return ((ConcurrentSkipListMap<?, E>)m).ValueSpliterator();
				}
				else
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (java.util.Spliterator<E>)((SubMap<?,E>)m).valueIterator();
					return (Spliterator<E>)((SubMap<?, E>)m).ValueIterator();
				}
			}
		}

		internal sealed class EntrySet<K1, V1> : AbstractSet<java.util.Map_Entry<K1, V1>>
		{
			internal readonly ConcurrentNavigableMap<K1, V1> m;
			internal EntrySet(ConcurrentNavigableMap<K1, V1> map)
			{
				m = map;
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Iterator<java.util.Map_Entry<K1,V1>> iterator()
			public IEnumerator<java.util.Map_Entry<K1, V1>> Iterator()
			{
				if (m is ConcurrentSkipListMap)
				{
					return ((ConcurrentSkipListMap<K1, V1>)m).EntryIterator();
				}
				else
				{
					return ((SubMap<K1, V1>)m).EntryIterator();
				}
			}

			public bool Contains(Object o)
			{
				if (!(o is java.util.Map_Entry))
				{
					return java.util.Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map_Entry<?,?> e = (java.util.Map_Entry<?,?>)o;
				java.util.Map_Entry<?, ?> e = (java.util.Map_Entry<?, ?>)o;
				V1 java.util.Map_Fields.v = m.Get(e.Key);
				return java.util.Map_Fields.v != java.util.Map_Fields.Null && java.util.Map_Fields.v.Equals(e.Value);
			}
			public bool Remove(Object o)
			{
				if (!(o is java.util.Map_Entry))
				{
					return java.util.Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map_Entry<?,?> e = (java.util.Map_Entry<?,?>)o;
				java.util.Map_Entry<?, ?> e = (java.util.Map_Entry<?, ?>)o;
				return m.Remove(e.Key, e.Value);
			}
			public bool Empty
			{
				get
				{
					return m.Empty;
				}
			}
			public int Size()
			{
				return m.Size();
			}
			public void Clear()
			{
				m.Clear();
			}
			public override bool Equals(Object o)
			{
				if (o == this)
				{
					return java.util.Map_Fields.True;
				}
				if (!(o is Set))
				{
					return java.util.Map_Fields.False;
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
					return java.util.Map_Fields.False;
				}
				catch (NullPointerException)
				{
					return java.util.Map_Fields.False;
				}
			}
			public Object[] ToArray()
			{
				return ToList(this).ToArray();
			}
			public T[] toArray<T>(T[] a)
			{
				return ToList(this).toArray(a);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Spliterator<java.util.Map_Entry<K1,V1>> spliterator()
			public Spliterator<java.util.Map_Entry<K1, V1>> Spliterator()
			{
				if (m is ConcurrentSkipListMap)
				{
					return ((ConcurrentSkipListMap<K1, V1>)m).EntrySpliterator();
				}
				else
				{
					return (Spliterator<java.util.Map_Entry<K1, V1>>)((SubMap<K1, V1>)m).EntryIterator();
				}
			}
		}

		/// <summary>
		/// Submaps returned by <seealso cref="ConcurrentSkipListMap"/> submap operations
		/// represent a subrange of mappings of their underlying
		/// maps. Instances of this class support all methods of their
		/// underlying maps, differing in that mappings outside their range are
		/// ignored, and attempts to add mappings outside their ranges result
		/// in <seealso cref="IllegalArgumentException"/>.  Instances of this class are
		/// constructed only using the {@code subMap}, {@code headMap}, and
		/// {@code tailMap} methods of their underlying maps.
		/// 
		/// @serial include
		/// </summary>
		[Serializable]
		internal sealed class SubMap<K, V> : AbstractMap<K, V>, ConcurrentNavigableMap<K, V>, Cloneable
		{
			internal const long SerialVersionUID = -7647078645895051609L;

			/// <summary>
			/// Underlying map </summary>
			internal readonly ConcurrentSkipListMap<K, V> m;
			/// <summary>
			/// lower bound key, or null if from start </summary>
			internal readonly K Lo;
			/// <summary>
			/// upper bound key, or null if to end </summary>
			internal readonly K Hi;
			/// <summary>
			/// inclusion flag for lo </summary>
			internal readonly bool LoInclusive;
			/// <summary>
			/// inclusion flag for hi </summary>
			internal readonly bool HiInclusive;
			/// <summary>
			/// direction </summary>
			internal readonly bool IsDescending;

			// Lazily initialized view holders
			[NonSerialized]
			internal KeySet<K> KeySetView;
			[NonSerialized]
			internal Set<java.util.Map_Entry<K, V>> EntrySetView;
			[NonSerialized]
			internal ICollection<V> ValuesView;

			/// <summary>
			/// Creates a new submap, initializing all fields.
			/// </summary>
			internal SubMap(ConcurrentSkipListMap<K, V> map, K fromKey, bool fromInclusive, K toKey, bool toInclusive, bool isDescending)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = map.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = map.Comparator_Renamed;
				if (fromKey != java.util.Map_Fields.Null && toKey != java.util.Map_Fields.Null && Cpr(cmp, fromKey, toKey) > 0)
				{
					throw new IllegalArgumentException("inconsistent range");
				}
				this.m = map;
				this.Lo = fromKey;
				this.Hi = toKey;
				this.LoInclusive = fromInclusive;
				this.HiInclusive = toInclusive;
				this.IsDescending = isDescending;
			}

			/* ----------------  Utilities -------------- */

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: boolean tooLow(Object key, java.util.Comparator<? base K> cmp)
			internal bool tooLow<T1>(Object key, IComparer<T1> cmp)
			{
				int c;
				return (Lo != java.util.Map_Fields.Null && ((c = Cpr(cmp, key, Lo)) < 0 || (c == 0 && !LoInclusive)));
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: boolean tooHigh(Object key, java.util.Comparator<? base K> cmp)
			internal bool tooHigh<T1>(Object key, IComparer<T1> cmp)
			{
				int c;
				return (Hi != java.util.Map_Fields.Null && ((c = Cpr(cmp, key, Hi)) > 0 || (c == 0 && !HiInclusive)));
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: boolean inBounds(Object key, java.util.Comparator<? base K> cmp)
			internal bool inBounds<T1>(Object key, IComparer<T1> cmp)
			{
				return !TooLow(key, cmp) && !TooHigh(key, cmp);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: void checkKeyBounds(K key, java.util.Comparator<? base K> cmp)
			internal void checkKeyBounds<T1>(K key, IComparer<T1> cmp)
			{
				if (key == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				if (!InBounds(key, cmp))
				{
					throw new IllegalArgumentException("key out of range");
				}
			}

			/// <summary>
			/// Returns true if node key is less than upper bound of range.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: boolean isBeforeEnd(ConcurrentSkipListMap.Node<K,V> n, java.util.Comparator<? base K> cmp)
			internal bool isBeforeEnd<T1>(ConcurrentSkipListMap.Node<K, V> n, IComparer<T1> cmp)
			{
				if (n == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.False;
				}
				if (Hi == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.True;
				}
				K java.util.Map_Fields.k = n.Key;
				if (java.util.Map_Fields.k == java.util.Map_Fields.Null) // pass by markers and headers
				{
					return java.util.Map_Fields.True;
				}
				int c = Cpr(cmp, java.util.Map_Fields.k, Hi);
				if (c > 0 || (c == 0 && !HiInclusive))
				{
					return java.util.Map_Fields.False;
				}
				return java.util.Map_Fields.True;
			}

			/// <summary>
			/// Returns lowest node. This node might not be in range, so
			/// most usages need to check bounds.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ConcurrentSkipListMap.Node<K,V> loNode(java.util.Comparator<? base K> cmp)
			internal ConcurrentSkipListMap.Node<K, V> LoNode(IComparer<T1> cmp)
			{
				if (Lo == java.util.Map_Fields.Null)
				{
					return m.FindFirst();
				}
				else if (LoInclusive)
				{
					return m.FindNear(Lo, GT | EQ, cmp);
				}
				else
				{
					return m.FindNear(Lo, GT, cmp);
				}
			}

			/// <summary>
			/// Returns highest node. This node might not be in range, so
			/// most usages need to check bounds.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ConcurrentSkipListMap.Node<K,V> hiNode(java.util.Comparator<? base K> cmp)
			internal ConcurrentSkipListMap.Node<K, V> HiNode(IComparer<T1> cmp)
			{
				if (Hi == java.util.Map_Fields.Null)
				{
					return m.FindLast();
				}
				else if (HiInclusive)
				{
					return m.FindNear(Hi, LT | EQ, cmp);
				}
				else
				{
					return m.FindNear(Hi, LT, cmp);
				}
			}

			/// <summary>
			/// Returns lowest absolute key (ignoring directonality).
			/// </summary>
			internal K LowestKey()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				ConcurrentSkipListMap.Node<K, V> n = LoNode(cmp);
				if (IsBeforeEnd(n, cmp))
				{
					return n.Key;
				}
				else
				{
					throw new NoSuchElementException();
				}
			}

			/// <summary>
			/// Returns highest absolute key (ignoring directonality).
			/// </summary>
			internal K HighestKey()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				ConcurrentSkipListMap.Node<K, V> n = HiNode(cmp);
				if (n != java.util.Map_Fields.Null)
				{
					K last = n.Key;
					if (InBounds(last, cmp))
					{
						return last;
					}
				}
				throw new NoSuchElementException();
			}

			internal java.util.Map_Entry<K, V> LowestEntry()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				for (;;)
				{
					ConcurrentSkipListMap.Node<K, V> n = LoNode(cmp);
					if (!IsBeforeEnd(n, cmp))
					{
						return java.util.Map_Fields.Null;
					}
					java.util.Map_Entry<K, V> e = n.CreateSnapshot();
					if (e != java.util.Map_Fields.Null)
					{
						return e;
					}
				}
			}

			internal java.util.Map_Entry<K, V> HighestEntry()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				for (;;)
				{
					ConcurrentSkipListMap.Node<K, V> n = HiNode(cmp);
					if (n == java.util.Map_Fields.Null || !InBounds(n.Key, cmp))
					{
						return java.util.Map_Fields.Null;
					}
					java.util.Map_Entry<K, V> e = n.CreateSnapshot();
					if (e != java.util.Map_Fields.Null)
					{
						return e;
					}
				}
			}

			internal java.util.Map_Entry<K, V> RemoveLowest()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				for (;;)
				{
					Node<K, V> n = LoNode(cmp);
					if (n == java.util.Map_Fields.Null)
					{
						return java.util.Map_Fields.Null;
					}
					K java.util.Map_Fields.k = n.Key;
					if (!InBounds(java.util.Map_Fields.k, cmp))
					{
						return java.util.Map_Fields.Null;
					}
					V java.util.Map_Fields.v = m.DoRemove(java.util.Map_Fields.k, java.util.Map_Fields.Null);
					if (java.util.Map_Fields.v != java.util.Map_Fields.Null)
					{
						return new AbstractMap.SimpleImmutableEntry<K, V>(java.util.Map_Fields.k, java.util.Map_Fields.v);
					}
				}
			}

			internal java.util.Map_Entry<K, V> RemoveHighest()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				for (;;)
				{
					Node<K, V> n = HiNode(cmp);
					if (n == java.util.Map_Fields.Null)
					{
						return java.util.Map_Fields.Null;
					}
					K java.util.Map_Fields.k = n.Key;
					if (!InBounds(java.util.Map_Fields.k, cmp))
					{
						return java.util.Map_Fields.Null;
					}
					V java.util.Map_Fields.v = m.DoRemove(java.util.Map_Fields.k, java.util.Map_Fields.Null);
					if (java.util.Map_Fields.v != java.util.Map_Fields.Null)
					{
						return new AbstractMap.SimpleImmutableEntry<K, V>(java.util.Map_Fields.k, java.util.Map_Fields.v);
					}
				}
			}

			/// <summary>
			/// Submap version of ConcurrentSkipListMap.getNearEntry
			/// </summary>
			internal java.util.Map_Entry<K, V> GetNearEntry(K key, int rel)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				if (IsDescending) // adjust relation for direction
				{
					if ((rel & LT) == 0)
					{
						rel |= LT;
					}
					else
					{
						rel &= ~LT;
					}
				}
				if (TooLow(key, cmp))
				{
					return ((rel & LT) != 0) ? java.util.Map_Fields.Null : LowestEntry();
				}
				if (TooHigh(key, cmp))
				{
					return ((rel & LT) != 0) ? HighestEntry() : java.util.Map_Fields.Null;
				}
				for (;;)
				{
					Node<K, V> n = m.FindNear(key, rel, cmp);
					if (n == java.util.Map_Fields.Null || !InBounds(n.Key, cmp))
					{
						return java.util.Map_Fields.Null;
					}
					K java.util.Map_Fields.k = n.Key;
					V java.util.Map_Fields.v = n.ValidValue;
					if (java.util.Map_Fields.v != java.util.Map_Fields.Null)
					{
						return new AbstractMap.SimpleImmutableEntry<K, V>(java.util.Map_Fields.k, java.util.Map_Fields.v);
					}
				}
			}

			// Almost the same as getNearEntry, except for keys
			internal K GetNearKey(K key, int rel)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				if (IsDescending) // adjust relation for direction
				{
					if ((rel & LT) == 0)
					{
						rel |= LT;
					}
					else
					{
						rel &= ~LT;
					}
				}
				if (TooLow(key, cmp))
				{
					if ((rel & LT) == 0)
					{
						ConcurrentSkipListMap.Node<K, V> n = LoNode(cmp);
						if (IsBeforeEnd(n, cmp))
						{
							return n.Key;
						}
					}
					return java.util.Map_Fields.Null;
				}
				if (TooHigh(key, cmp))
				{
					if ((rel & LT) != 0)
					{
						ConcurrentSkipListMap.Node<K, V> n = HiNode(cmp);
						if (n != java.util.Map_Fields.Null)
						{
							K last = n.Key;
							if (InBounds(last, cmp))
							{
								return last;
							}
						}
					}
					return java.util.Map_Fields.Null;
				}
				for (;;)
				{
					Node<K, V> n = m.FindNear(key, rel, cmp);
					if (n == java.util.Map_Fields.Null || !InBounds(n.Key, cmp))
					{
						return java.util.Map_Fields.Null;
					}
					K java.util.Map_Fields.k = n.Key;
					V java.util.Map_Fields.v = n.ValidValue;
					if (java.util.Map_Fields.v != java.util.Map_Fields.Null)
					{
						return java.util.Map_Fields.k;
					}
				}
			}

			/* ----------------  Map API methods -------------- */

			public bool ContainsKey(Object key)
			{
				if (key == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				return InBounds(key, m.Comparator_Renamed) && m.ContainsKey(key);
			}

			public V Get(Object key)
			{
				if (key == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				return (!InBounds(key, m.Comparator_Renamed)) ? java.util.Map_Fields.Null : m.Get(key);
			}

			public V Put(K key, V value)
			{
				CheckKeyBounds(key, m.Comparator_Renamed);
				return m.Put(key, value);
			}

			public V Remove(Object key)
			{
				return (!InBounds(key, m.Comparator_Renamed)) ? java.util.Map_Fields.Null : m.Remove(key);
			}

			public int Size()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				long count = 0;
				for (ConcurrentSkipListMap.Node<K, V> n = LoNode(cmp); IsBeforeEnd(n, cmp); n = n.Next)
				{
					if (n.ValidValue != java.util.Map_Fields.Null)
					{
						++count;
					}
				}
				return count >= Integer.MaxValue ? Integer.MaxValue : (int)count;
			}

			public bool Empty
			{
				get
				{
	//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
	//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					IComparer<?> cmp = m.Comparator_Renamed;
					return !IsBeforeEnd(LoNode(cmp), cmp);
				}
			}

			public bool ContainsValue(Object value)
			{
				if (value == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				for (ConcurrentSkipListMap.Node<K, V> n = LoNode(cmp); IsBeforeEnd(n, cmp); n = n.Next)
				{
					V java.util.Map_Fields.v = n.ValidValue;
					if (java.util.Map_Fields.v != java.util.Map_Fields.Null && value.Equals(java.util.Map_Fields.v))
					{
						return java.util.Map_Fields.True;
					}
				}
				return java.util.Map_Fields.False;
			}

			public void Clear()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				for (ConcurrentSkipListMap.Node<K, V> n = LoNode(cmp); IsBeforeEnd(n, cmp); n = n.Next)
				{
					if (n.ValidValue != java.util.Map_Fields.Null)
					{
						m.Remove(n.Key);
					}
				}
			}

			/* ----------------  ConcurrentMap API methods -------------- */

			public V PutIfAbsent(K key, V value)
			{
				CheckKeyBounds(key, m.Comparator_Renamed);
				return m.PutIfAbsent(key, value);
			}

			public bool Remove(Object key, Object value)
			{
				return InBounds(key, m.Comparator_Renamed) && m.Remove(key, value);
			}

			public bool Replace(K key, V java, V java)
			{
				CheckKeyBounds(key, m.Comparator_Renamed);
				return m.Replace(key, java.util.Map_Fields.OldValue, java.util.Map_Fields.NewValue);
			}

			public V Replace(K key, V value)
			{
				CheckKeyBounds(key, m.Comparator_Renamed);
				return m.Replace(key, value);
			}

			/* ----------------  SortedMap API methods -------------- */

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public java.util.Comparator<? base K> comparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public IComparer<?> Comparator()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator();
				if (IsDescending)
				{
					return Collections.ReverseOrder(cmp);
				}
				else
				{
					return cmp;
				}
			}

			/// <summary>
			/// Utility to create submaps, where given bounds override
			/// unbounded(null) ones and/or are checked against bounded ones.
			/// </summary>
			internal SubMap<K, V> NewSubMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = m.Comparator_Renamed;
				if (IsDescending) // flip senses
				{
					K tk = fromKey;
					fromKey = toKey;
					toKey = tk;
					bool ti = fromInclusive;
					fromInclusive = toInclusive;
					toInclusive = ti;
				}
				if (Lo != java.util.Map_Fields.Null)
				{
					if (fromKey == java.util.Map_Fields.Null)
					{
						fromKey = Lo;
						fromInclusive = LoInclusive;
					}
					else
					{
						int c = Cpr(cmp, fromKey, Lo);
						if (c < 0 || (c == 0 && !LoInclusive && fromInclusive))
						{
							throw new IllegalArgumentException("key out of range");
						}
					}
				}
				if (Hi != java.util.Map_Fields.Null)
				{
					if (toKey == java.util.Map_Fields.Null)
					{
						toKey = Hi;
						toInclusive = HiInclusive;
					}
					else
					{
						int c = Cpr(cmp, toKey, Hi);
						if (c > 0 || (c == 0 && !HiInclusive && toInclusive))
						{
							throw new IllegalArgumentException("key out of range");
						}
					}
				}
				return new SubMap<K, V>(m, fromKey, fromInclusive, toKey, toInclusive, IsDescending);
			}

			public SubMap<K, V> SubMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive)
			{
				if (fromKey == java.util.Map_Fields.Null || toKey == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				return NewSubMap(fromKey, fromInclusive, toKey, toInclusive);
			}

			public SubMap<K, V> HeadMap(K toKey, bool inclusive)
			{
				if (toKey == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				return NewSubMap(java.util.Map_Fields.Null, java.util.Map_Fields.False, toKey, inclusive);
			}

			public SubMap<K, V> TailMap(K fromKey, bool inclusive)
			{
				if (fromKey == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				return NewSubMap(fromKey, inclusive, java.util.Map_Fields.Null, java.util.Map_Fields.False);
			}

			public SubMap<K, V> SubMap(K fromKey, K toKey)
			{
				return SubMap(fromKey, java.util.Map_Fields.True, toKey, java.util.Map_Fields.False);
			}

			public SubMap<K, V> HeadMap(K toKey)
			{
				return HeadMap(toKey, java.util.Map_Fields.False);
			}

			public SubMap<K, V> TailMap(K fromKey)
			{
				return TailMap(fromKey, java.util.Map_Fields.True);
			}

			public SubMap<K, V> DescendingMap()
			{
				return new SubMap<K, V>(m, Lo, LoInclusive, Hi, HiInclusive, !IsDescending);
			}

			/* ----------------  Relational methods -------------- */

			public java.util.Map_Entry<K, V> CeilingEntry(K key)
			{
				return GetNearEntry(key, GT | EQ);
			}

			public K CeilingKey(K key)
			{
				return GetNearKey(key, GT | EQ);
			}

			public java.util.Map_Entry<K, V> LowerEntry(K key)
			{
				return GetNearEntry(key, LT);
			}

			public K LowerKey(K key)
			{
				return GetNearKey(key, LT);
			}

			public java.util.Map_Entry<K, V> FloorEntry(K key)
			{
				return GetNearEntry(key, LT | EQ);
			}

			public K FloorKey(K key)
			{
				return GetNearKey(key, LT | EQ);
			}

			public java.util.Map_Entry<K, V> HigherEntry(K key)
			{
				return GetNearEntry(key, GT);
			}

			public K HigherKey(K key)
			{
				return GetNearKey(key, GT);
			}

			public K FirstKey()
			{
				return IsDescending ? HighestKey() : LowestKey();
			}

			public K LastKey()
			{
				return IsDescending ? LowestKey() : HighestKey();
			}

			public java.util.Map_Entry<K, V> FirstEntry()
			{
				return IsDescending ? HighestEntry() : LowestEntry();
			}

			public java.util.Map_Entry<K, V> LastEntry()
			{
				return IsDescending ? LowestEntry() : HighestEntry();
			}

			public java.util.Map_Entry<K, V> PollFirstEntry()
			{
				return IsDescending ? RemoveHighest() : RemoveLowest();
			}

			public java.util.Map_Entry<K, V> PollLastEntry()
			{
				return IsDescending ? RemoveLowest() : RemoveHighest();
			}

			/* ---------------- Submap Views -------------- */

			public NavigableSet<K> KeySet()
			{
				KeySet<K> ks = KeySetView;
				return (ks != java.util.Map_Fields.Null) ? ks : (KeySetView = new KeySet<K>(this));
			}

			public NavigableSet<K> NavigableKeySet()
			{
				KeySet<K> ks = KeySetView;
				return (ks != java.util.Map_Fields.Null) ? ks : (KeySetView = new KeySet<K>(this));
			}

			public ICollection<V> Values()
			{
				ICollection<V> vs = ValuesView;
				return (vs != java.util.Map_Fields.Null) ? vs : (ValuesView = new Values<V>(this));
			}

			public Set<java.util.Map_Entry<K, V>> EntrySet()
			{
				Set<java.util.Map_Entry<K, V>> es = EntrySetView;
				return (es != java.util.Map_Fields.Null) ? es : (EntrySetView = new EntrySet<K, V>(this));
			}

			public NavigableSet<K> DescendingKeySet()
			{
				return DescendingMap().NavigableKeySet();
			}

			internal IEnumerator<K> KeyIterator()
			{
				return new SubMapKeyIterator(this);
			}

			internal IEnumerator<V> ValueIterator()
			{
				return new SubMapValueIterator(this);
			}

			internal IEnumerator<java.util.Map_Entry<K, V>> EntryIterator()
			{
				return new SubMapEntryIterator(this);
			}

			/// <summary>
			/// Variant of main Iter class to traverse through submaps.
			/// Also serves as back-up Spliterator for views
			/// </summary>
			internal abstract class SubMapIter<T> : Iterator<T>, Spliterator<T>
			{
				private readonly ConcurrentSkipListMap.SubMap<K, V> OuterInstance;

				/// <summary>
				/// the last node returned by next() </summary>
				internal Node<K, V> LastReturned;
				/// <summary>
				/// the next node to return from next(); </summary>
				internal Node<K, V> Next;
				/// <summary>
				/// Cache of next value field to maintain weak consistency </summary>
				internal V NextValue;

				internal SubMapIter(ConcurrentSkipListMap.SubMap<K, V> outerInstance)
				{
					this.OuterInstance = outerInstance;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					IComparer<?> cmp = outerInstance.m.Comparator_Renamed;
					for (;;)
					{
						Next = outerInstance.IsDescending ? outerInstance.HiNode(cmp) : outerInstance.LoNode(cmp);
						if (Next == java.util.Map_Fields.Null)
						{
							break;
						}
						Object x = Next.Value;
						if (x != java.util.Map_Fields.Null && x != Next)
						{
							if (!outerInstance.InBounds(Next.Key, cmp))
							{
								Next = java.util.Map_Fields.Null;
							}
							else
							{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)x;
								V vv = (V)x;
								NextValue = vv;
							}
							break;
						}
					}
				}

				public bool HasNext()
				{
					return Next != java.util.Map_Fields.Null;
				}

				internal void Advance()
				{
					if (Next == java.util.Map_Fields.Null)
					{
						throw new NoSuchElementException();
					}
					LastReturned = Next;
					if (outerInstance.IsDescending)
					{
						Descend();
					}
					else
					{
						Ascend();
					}
				}

				internal virtual void Ascend()
				{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					IComparer<?> cmp = outerInstance.m.Comparator_Renamed;
					for (;;)
					{
						Next = Next.Next;
						if (Next == java.util.Map_Fields.Null)
						{
							break;
						}
						Object x = Next.Value;
						if (x != java.util.Map_Fields.Null && x != Next)
						{
							if (outerInstance.TooHigh(Next.Key, cmp))
							{
								Next = java.util.Map_Fields.Null;
							}
							else
							{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)x;
								V vv = (V)x;
								NextValue = vv;
							}
							break;
						}
					}
				}

				internal virtual void Descend()
				{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = m.comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					IComparer<?> cmp = outerInstance.m.Comparator_Renamed;
					for (;;)
					{
						Next = outerInstance.m.FindNear(LastReturned.Key, LT, cmp);
						if (Next == java.util.Map_Fields.Null)
						{
							break;
						}
						Object x = Next.Value;
						if (x != java.util.Map_Fields.Null && x != Next)
						{
							if (outerInstance.TooLow(Next.Key, cmp))
							{
								Next = java.util.Map_Fields.Null;
							}
							else
							{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)x;
								V vv = (V)x;
								NextValue = vv;
							}
							break;
						}
					}
				}

				public virtual void Remove()
				{
					Node<K, V> l = LastReturned;
					if (l == java.util.Map_Fields.Null)
					{
						throw new IllegalStateException();
					}
					outerInstance.m.Remove(l.Key);
					LastReturned = java.util.Map_Fields.Null;
				}

				public virtual Spliterator<T> TrySplit()
				{
					return java.util.Map_Fields.Null;
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base T> action)
				public virtual bool tryAdvance<T1>(Consumer<T1> action)
				{
					if (HasNext())
					{
						action.Accept(next());
						return java.util.Map_Fields.True;
					}
					return java.util.Map_Fields.False;
				}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base T> action)
				public virtual void forEachRemaining<T1>(Consumer<T1> action)
				{
					while (HasNext())
					{
						action.Accept(next());
					}
				}

				public virtual long EstimateSize()
				{
					return Long.MaxValue;
				}

			}

			internal sealed class SubMapValueIterator : SubMapIter<V>
			{
				private readonly ConcurrentSkipListMap.SubMap<K, V> OuterInstance;

				public SubMapValueIterator(ConcurrentSkipListMap.SubMap<K, V> outerInstance) : base(outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public V Next()
				{
					V java.util.Map_Fields.v = NextValue;
					Advance();
					return java.util.Map_Fields.v;
				}
				public int Characteristics()
				{
					return 0;
				}
			}

			internal sealed class SubMapKeyIterator : SubMapIter<K>
			{
				private readonly ConcurrentSkipListMap.SubMap<K, V> OuterInstance;

				public SubMapKeyIterator(ConcurrentSkipListMap.SubMap<K, V> outerInstance) : base(outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public K Next()
				{
					Node<K, V> n = Next;
					Advance();
					return n.Key;
				}
				public int Characteristics()
				{
					return java.util.Spliterator_Fields.DISTINCT | java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.SORTED;
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final java.util.Comparator<? base K> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				public IComparer<?> Comparator
				{
					get
					{
						return OuterInstance.Comparator();
					}
				}
			}

			internal sealed class SubMapEntryIterator : SubMapIter<java.util.Map_Entry<K, V>>
			{
				private readonly ConcurrentSkipListMap.SubMap<K, V> OuterInstance;

				public SubMapEntryIterator(ConcurrentSkipListMap.SubMap<K, V> outerInstance) : base(outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				public java.util.Map_Entry<K, V> Next()
				{
					Node<K, V> n = Next;
					V java.util.Map_Fields.v = NextValue;
					Advance();
					return new AbstractMap.SimpleImmutableEntry<K, V>(n.Key, java.util.Map_Fields.v);
				}
				public int Characteristics()
				{
					return java.util.Spliterator_Fields.DISTINCT;
				}
			}
		}

		// default Map method overrides

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
		public virtual void forEach<T1>(BiConsumer<T1> action)
		{
			if (action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			V java.util.Map_Fields.v;
			for (Node<K, V> n = FindFirst(); n != java.util.Map_Fields.Null; n = n.Next)
			{
				if ((java.util.Map_Fields.v = n.ValidValue) != java.util.Map_Fields.Null)
				{
					action.Accept(n.Key, java.util.Map_Fields.v);
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
		public virtual void replaceAll<T1>(BiFunction<T1> function) where T1 : V
		{
			if (function == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			V java.util.Map_Fields.v;
			for (Node<K, V> n = FindFirst(); n != java.util.Map_Fields.Null; n = n.Next)
			{
				while ((java.util.Map_Fields.v = n.ValidValue) != java.util.Map_Fields.Null)
				{
					V r = function.Apply(n.Key, java.util.Map_Fields.v);
					if (r == java.util.Map_Fields.Null)
					{
						throw new NullPointerException();
					}
					if (n.CasValue(java.util.Map_Fields.v, r))
					{
						break;
					}
				}
			}
		}

		/// <summary>
		/// Base class providing common structure for Spliterators.
		/// (Although not all that much common functionality; as usual for
		/// view classes, details annoyingly vary in key, value, and entry
		/// subclasses in ways that are not worth abstracting out for
		/// internal classes.)
		/// 
		/// The basic split strategy is to recursively descend from top
		/// level, row by row, descending to next row when either split
		/// off, or the end of row is encountered. Control of the number of
		/// splits relies on some statistical estimation: The expected
		/// remaining number of elements of a skip list when advancing
		/// either across or down decreases by about 25%. To make this
		/// observation useful, we need to know initial size, which we
		/// don't. But we can just use Integer.MAX_VALUE so that we
		/// don't prematurely zero out while splitting.
		/// </summary>
		internal abstract class CSLMSpliterator<K, V>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.Comparator<? base K> comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly IComparer<?> Comparator;
			internal readonly K Fence; // exclusive upper bound for keys, or null if to end
			internal Index<K, V> Row; // the level to split out
			internal Node<K, V> Current; // current traversal node; initialize at origin
			internal int Est; // pseudo-size estimate
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: CSLMSpliterator(java.util.Comparator<? base K> comparator, Index<K,V> row, Node<K,V> origin, K fence, int est)
			internal CSLMSpliterator<T1>(IComparer<T1> comparator, Index<K, V> row, Node<K, V> origin, K fence, int est)
			{
				this.Comparator = comparator;
				this.Row = row;
				this.Current = origin;
				this.Fence = fence;
				this.Est = est;
			}

			public long EstimateSize()
			{
				return (long)Est;
			}
		}

		internal sealed class KeySpliterator<K, V> : CSLMSpliterator<K, V>, Spliterator<K>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: KeySpliterator(java.util.Comparator<? base K> comparator, Index<K,V> row, Node<K,V> origin, K fence, int est)
			internal KeySpliterator<T1>(IComparer<T1> comparator, Index<K, V> row, Node<K, V> origin, K fence, int est) : base(comparator, row, origin, fence, est)
			{
			}

			public Spliterator<K> TrySplit()
			{
				Node<K, V> e;
				K ek;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator;
				K f = Fence;
				if ((e = Current) != java.util.Map_Fields.Null && (ek = e.Key) != java.util.Map_Fields.Null)
				{
					for (Index<K, V> q = Row; q != java.util.Map_Fields.Null; q = Row = q.Down)
					{
						Index<K, V> s;
						Node<K, V> b, n;
						K sk;
						if ((s = q.Right) != java.util.Map_Fields.Null && (b = s.Node) != java.util.Map_Fields.Null && (n = b.Next) != java.util.Map_Fields.Null && n.Value != java.util.Map_Fields.Null && (sk = n.Key) != java.util.Map_Fields.Null && Cpr(cmp, sk, ek) > 0 && (f == java.util.Map_Fields.Null || Cpr(cmp, sk, f) < 0))
						{
							Current = n;
							Index<K, V> r = q.Down;
							Row = (s.Right != java.util.Map_Fields.Null) ? s : s.Down;
							Est -= (int)((uint)Est >> 2);
							return new KeySpliterator<K, V>(cmp, r, e, sk, Est);
						}
					}
				}
				return java.util.Map_Fields.Null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base K> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator;
				K f = Fence;
				Node<K, V> e = Current;
				Current = java.util.Map_Fields.Null;
				for (; e != java.util.Map_Fields.Null; e = e.Next)
				{
					K java.util.Map_Fields.k;
					Object java.util.Map_Fields.v;
					if ((java.util.Map_Fields.k = e.Key) != java.util.Map_Fields.Null && f != java.util.Map_Fields.Null && Cpr(cmp, f, java.util.Map_Fields.k) <= 0)
					{
						break;
					}
					if ((java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && java.util.Map_Fields.v != e)
					{
						action.Accept(java.util.Map_Fields.k);
					}
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base K> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator;
				K f = Fence;
				Node<K, V> e = Current;
				for (; e != java.util.Map_Fields.Null; e = e.Next)
				{
					K java.util.Map_Fields.k;
					Object java.util.Map_Fields.v;
					if ((java.util.Map_Fields.k = e.Key) != java.util.Map_Fields.Null && f != java.util.Map_Fields.Null && Cpr(cmp, f, java.util.Map_Fields.k) <= 0)
					{
						e = java.util.Map_Fields.Null;
						break;
					}
					if ((java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && java.util.Map_Fields.v != e)
					{
						Current = e.Next;
						action.Accept(java.util.Map_Fields.k);
						return java.util.Map_Fields.True;
					}
				}
				Current = e;
				return java.util.Map_Fields.False;
			}

			public int Characteristics()
			{
				return java.util.Spliterator_Fields.DISTINCT | java.util.Spliterator_Fields.SORTED | java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.CONCURRENT | java.util.Spliterator_Fields.NONNULL;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public final java.util.Comparator<? base K> getComparator()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public IComparer<?> Comparator
			{
				get
				{
					return Comparator;
				}
			}
		}
		// factory method for KeySpliterator
		internal KeySpliterator<K, V> KeySpliterator()
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IComparer<?> cmp = Comparator_Renamed;
			for (;;) // ensure h corresponds to origin p
			{
				HeadIndex<K, V> h;
				Node<K, V> p;
				Node<K, V> b = (h = Head).node;
				if ((p = b.Next) == java.util.Map_Fields.Null || p.Value != java.util.Map_Fields.Null)
				{
					return new KeySpliterator<K, V>(cmp, h, p, java.util.Map_Fields.Null, (p == java.util.Map_Fields.Null) ? 0 : Integer.MaxValue);
				}
				p.HelpDelete(b, p.Next);
			}
		}

		internal sealed class ValueSpliterator<K, V> : CSLMSpliterator<K, V>, Spliterator<V>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ValueSpliterator(java.util.Comparator<? base K> comparator, Index<K,V> row, Node<K,V> origin, K fence, int est)
			internal ValueSpliterator<T1>(IComparer<T1> comparator, Index<K, V> row, Node<K, V> origin, K fence, int est) : base(comparator, row, origin, fence, est)
			{
			}

			public Spliterator<V> TrySplit()
			{
				Node<K, V> e;
				K ek;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator;
				K f = Fence;
				if ((e = Current) != java.util.Map_Fields.Null && (ek = e.Key) != java.util.Map_Fields.Null)
				{
					for (Index<K, V> q = Row; q != java.util.Map_Fields.Null; q = Row = q.Down)
					{
						Index<K, V> s;
						Node<K, V> b, n;
						K sk;
						if ((s = q.Right) != java.util.Map_Fields.Null && (b = s.Node) != java.util.Map_Fields.Null && (n = b.Next) != java.util.Map_Fields.Null && n.Value != java.util.Map_Fields.Null && (sk = n.Key) != java.util.Map_Fields.Null && Cpr(cmp, sk, ek) > 0 && (f == java.util.Map_Fields.Null || Cpr(cmp, sk, f) < 0))
						{
							Current = n;
							Index<K, V> r = q.Down;
							Row = (s.Right != java.util.Map_Fields.Null) ? s : s.Down;
							Est -= (int)((uint)Est >> 2);
							return new ValueSpliterator<K, V>(cmp, r, e, sk, Est);
						}
					}
				}
				return java.util.Map_Fields.Null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base V> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator;
				K f = Fence;
				Node<K, V> e = Current;
				Current = java.util.Map_Fields.Null;
				for (; e != java.util.Map_Fields.Null; e = e.Next)
				{
					K java.util.Map_Fields.k;
					Object java.util.Map_Fields.v;
					if ((java.util.Map_Fields.k = e.Key) != java.util.Map_Fields.Null && f != java.util.Map_Fields.Null && Cpr(cmp, f, java.util.Map_Fields.k) <= 0)
					{
						break;
					}
					if ((java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && java.util.Map_Fields.v != e)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
						V vv = (V)java.util.Map_Fields.v;
						action.Accept(vv);
					}
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base V> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator;
				K f = Fence;
				Node<K, V> e = Current;
				for (; e != java.util.Map_Fields.Null; e = e.Next)
				{
					K java.util.Map_Fields.k;
					Object java.util.Map_Fields.v;
					if ((java.util.Map_Fields.k = e.Key) != java.util.Map_Fields.Null && f != java.util.Map_Fields.Null && Cpr(cmp, f, java.util.Map_Fields.k) <= 0)
					{
						e = java.util.Map_Fields.Null;
						break;
					}
					if ((java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && java.util.Map_Fields.v != e)
					{
						Current = e.Next;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
						V vv = (V)java.util.Map_Fields.v;
						action.Accept(vv);
						return java.util.Map_Fields.True;
					}
				}
				Current = e;
				return java.util.Map_Fields.False;
			}

			public int Characteristics()
			{
				return java.util.Spliterator_Fields.CONCURRENT | java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.NONNULL;
			}
		}

		// Almost the same as keySpliterator()
		internal ValueSpliterator<K, V> ValueSpliterator()
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IComparer<?> cmp = Comparator_Renamed;
			for (;;)
			{
				HeadIndex<K, V> h;
				Node<K, V> p;
				Node<K, V> b = (h = Head).node;
				if ((p = b.Next) == java.util.Map_Fields.Null || p.Value != java.util.Map_Fields.Null)
				{
					return new ValueSpliterator<K, V>(cmp, h, p, java.util.Map_Fields.Null, (p == java.util.Map_Fields.Null) ? 0 : Integer.MaxValue);
				}
				p.HelpDelete(b, p.Next);
			}
		}

		internal sealed class EntrySpliterator<K, V> : CSLMSpliterator<K, V>, Spliterator<java.util.Map_Entry<K, V>>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: EntrySpliterator(java.util.Comparator<? base K> comparator, Index<K,V> row, Node<K,V> origin, K fence, int est)
			internal EntrySpliterator<T1>(IComparer<T1> comparator, Index<K, V> row, Node<K, V> origin, K fence, int est) : base(comparator, row, origin, fence, est)
			{
			}

			public Spliterator<java.util.Map_Entry<K, V>> TrySplit()
			{
				Node<K, V> e;
				K ek;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator;
				K f = Fence;
				if ((e = Current) != java.util.Map_Fields.Null && (ek = e.Key) != java.util.Map_Fields.Null)
				{
					for (Index<K, V> q = Row; q != java.util.Map_Fields.Null; q = Row = q.Down)
					{
						Index<K, V> s;
						Node<K, V> b, n;
						K sk;
						if ((s = q.Right) != java.util.Map_Fields.Null && (b = s.Node) != java.util.Map_Fields.Null && (n = b.Next) != java.util.Map_Fields.Null && n.Value != java.util.Map_Fields.Null && (sk = n.Key) != java.util.Map_Fields.Null && Cpr(cmp, sk, ek) > 0 && (f == java.util.Map_Fields.Null || Cpr(cmp, sk, f) < 0))
						{
							Current = n;
							Index<K, V> r = q.Down;
							Row = (s.Right != java.util.Map_Fields.Null) ? s : s.Down;
							Est -= (int)((uint)Est >> 2);
							return new EntrySpliterator<K, V>(cmp, r, e, sk, Est);
						}
					}
				}
				return java.util.Map_Fields.Null;
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base java.util.Map_Entry<K,V>> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator;
				K f = Fence;
				Node<K, V> e = Current;
				Current = java.util.Map_Fields.Null;
				for (; e != java.util.Map_Fields.Null; e = e.Next)
				{
					K java.util.Map_Fields.k;
					Object java.util.Map_Fields.v;
					if ((java.util.Map_Fields.k = e.Key) != java.util.Map_Fields.Null && f != java.util.Map_Fields.Null && Cpr(cmp, f, java.util.Map_Fields.k) <= 0)
					{
						break;
					}
					if ((java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && java.util.Map_Fields.v != e)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
						V vv = (V)java.util.Map_Fields.v;
						action.Accept(new AbstractMap.SimpleImmutableEntry<K, V>(java.util.Map_Fields.k, vv));
					}
				}
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean tryAdvance(java.util.function.Consumer<? base java.util.Map_Entry<K,V>> action)
			public bool tryAdvance<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				IComparer<?> cmp = Comparator;
				K f = Fence;
				Node<K, V> e = Current;
				for (; e != java.util.Map_Fields.Null; e = e.Next)
				{
					K java.util.Map_Fields.k;
					Object java.util.Map_Fields.v;
					if ((java.util.Map_Fields.k = e.Key) != java.util.Map_Fields.Null && f != java.util.Map_Fields.Null && Cpr(cmp, f, java.util.Map_Fields.k) <= 0)
					{
						e = java.util.Map_Fields.Null;
						break;
					}
					if ((java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && java.util.Map_Fields.v != e)
					{
						Current = e.Next;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V vv = (V)java.util.Map_Fields.v;
						V vv = (V)java.util.Map_Fields.v;
						action.Accept(new AbstractMap.SimpleImmutableEntry<K, V>(java.util.Map_Fields.k, vv));
						return java.util.Map_Fields.True;
					}
				}
				Current = e;
				return java.util.Map_Fields.False;
			}

			public int Characteristics()
			{
				return java.util.Spliterator_Fields.DISTINCT | java.util.Spliterator_Fields.SORTED | java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.CONCURRENT | java.util.Spliterator_Fields.NONNULL;
			}

			public IComparer<java.util.Map_Entry<K, V>> Comparator
			{
				get
				{
					// Adapt or create a key-based comparator
					if (Comparator != java.util.Map_Fields.Null)
					{
						return java.util.Map_Entry.comparingByKey(Comparator);
					}
					else
					{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
						return (IComparer<java.util.Map_Entry<K, V>> & Serializable)(e1, e2) =>
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

		// Almost the same as keySpliterator()
		internal EntrySpliterator<K, V> EntrySpliterator()
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Comparator<? base K> cmp = comparator;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IComparer<?> cmp = Comparator_Renamed;
			for (;;) // almost same as key version
			{
				HeadIndex<K, V> h;
				Node<K, V> p;
				Node<K, V> b = (h = Head).node;
				if ((p = b.Next) == java.util.Map_Fields.Null || p.Value != java.util.Map_Fields.Null)
				{
					return new EntrySpliterator<K, V>(cmp, h, p, java.util.Map_Fields.Null, (p == java.util.Map_Fields.Null) ? 0 : Integer.MaxValue);
				}
				p.HelpDelete(b, p.Next);
			}
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe UNSAFE;
		private static readonly long HeadOffset;
		private static readonly long SECONDARY;
		static ConcurrentSkipListMap()
		{
			try
			{
				UNSAFE = sun.misc.Unsafe.Unsafe;
				Class java.util.Map_Fields.k = typeof(ConcurrentSkipListMap);
				HeadOffset = UNSAFE.objectFieldOffset(java.util.Map_Fields.k.getDeclaredField("head"));
				Class tk = typeof(Thread);
				SECONDARY = UNSAFE.objectFieldOffset(tk.GetDeclaredField("threadLocalRandomSecondarySeed"));

			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}