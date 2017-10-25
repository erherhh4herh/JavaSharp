using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

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
	/// A hash table supporting full concurrency of retrievals and
	/// high expected concurrency for updates. This class obeys the
	/// same functional specification as <seealso cref="java.util.Hashtable"/>, and
	/// includes versions of methods corresponding to each method of
	/// {@code Hashtable}. However, even though all operations are
	/// thread-safe, retrieval operations do <em>not</em> entail locking,
	/// and there is <em>not</em> any support for locking the entire table
	/// in a way that prevents all access.  This class is fully
	/// interoperable with {@code Hashtable} in programs that rely on its
	/// thread safety but not on its synchronization details.
	/// 
	/// <para>Retrieval operations (including {@code get}) generally do not
	/// block, so may overlap with update operations (including {@code put}
	/// and {@code remove}). Retrievals reflect the results of the most
	/// recently <em>completed</em> update operations holding upon their
	/// onset. (More formally, an update operation for a given key bears a
	/// <em>happens-before</em> relation with any (non-null) retrieval for
	/// that key reporting the updated value.)  For aggregate operations
	/// such as {@code putAll} and {@code clear}, concurrent retrievals may
	/// reflect insertion or removal of only some entries.  Similarly,
	/// Iterators, Spliterators and Enumerations return elements reflecting the
	/// state of the hash table at some point at or since the creation of the
	/// iterator/enumeration.  They do <em>not</em> throw {@link
	/// java.util.ConcurrentModificationException ConcurrentModificationException}.
	/// However, iterators are designed to be used by only one thread at a time.
	/// Bear in mind that the results of aggregate status methods including
	/// {@code size}, {@code isEmpty}, and {@code containsValue} are typically
	/// useful only when a map is not undergoing concurrent updates in other threads.
	/// Otherwise the results of these methods reflect transient states
	/// that may be adequate for monitoring or estimation purposes, but not
	/// for program control.
	/// 
	/// </para>
	/// <para>The table is dynamically expanded when there are too many
	/// collisions (i.e., keys that have distinct hash codes but fall into
	/// the same slot modulo the table size), with the expected average
	/// effect of maintaining roughly two bins per mapping (corresponding
	/// to a 0.75 load factor threshold for resizing). There may be much
	/// variance around this average as mappings are added and removed, but
	/// overall, this maintains a commonly accepted time/space tradeoff for
	/// hash tables.  However, resizing this or any other kind of hash
	/// table may be a relatively slow operation. When possible, it is a
	/// good idea to provide a size estimate as an optional {@code
	/// initialCapacity} constructor argument. An additional optional
	/// {@code loadFactor} constructor argument provides a further means of
	/// customizing initial table capacity by specifying the table density
	/// to be used in calculating the amount of space to allocate for the
	/// given number of elements.  Also, for compatibility with previous
	/// versions of this class, constructors may optionally specify an
	/// expected {@code concurrencyLevel} as an additional hint for
	/// internal sizing.  Note that using many keys with exactly the same
	/// {@code hashCode()} is a sure way to slow down performance of any
	/// hash table. To ameliorate impact, when keys are <seealso cref="Comparable"/>,
	/// this class may use comparison order among keys to help break ties.
	/// 
	/// </para>
	/// <para>A <seealso cref="Set"/> projection of a ConcurrentHashMap may be created
	/// (using <seealso cref="#newKeySet()"/> or <seealso cref="#newKeySet(int)"/>), or viewed
	/// (using <seealso cref="#keySet(Object)"/> when only keys are of interest, and the
	/// mapped values are (perhaps transiently) not used or all take the
	/// same mapping value.
	/// 
	/// </para>
	/// <para>A ConcurrentHashMap can be used as scalable frequency map (a
	/// form of histogram or multiset) by using {@link
	/// java.util.concurrent.atomic.LongAdder} values and initializing via
	/// <seealso cref="#computeIfAbsent computeIfAbsent"/>. For example, to add a count
	/// to a {@code ConcurrentHashMap<String,LongAdder> freqs}, you can use
	/// {@code freqs.computeIfAbsent(k -> new LongAdder()).increment();}
	/// 
	/// </para>
	/// <para>This class and its views and iterators implement all of the
	/// <em>optional</em> methods of the <seealso cref="Map"/> and <seealso cref="Iterator"/>
	/// interfaces.
	/// 
	/// </para>
	/// <para>Like <seealso cref="Hashtable"/> but unlike <seealso cref="HashMap"/>, this class
	/// does <em>not</em> allow {@code null} to be used as a key or value.
	/// 
	/// </para>
	/// <para>ConcurrentHashMaps support a set of sequential and parallel bulk
	/// operations that, unlike most <seealso cref="Stream"/> methods, are designed
	/// to be safely, and often sensibly, applied even with maps that are
	/// being concurrently updated by other threads; for example, when
	/// computing a snapshot summary of the values in a shared registry.
	/// There are three kinds of operation, each with four forms, accepting
	/// functions with Keys, Values, Entries, and (Key, Value) arguments
	/// and/or return values. Because the elements of a ConcurrentHashMap
	/// are not ordered in any particular way, and may be processed in
	/// different orders in different parallel executions, the correctness
	/// of supplied functions should not depend on any ordering, or on any
	/// other objects or values that may transiently change while
	/// computation is in progress; and except for forEach actions, should
	/// ideally be side-effect-free. Bulk operations on <seealso cref="java.util.Map.Entry"/>
	/// objects do not support method {@code setValue}.
	/// 
	/// <ul>
	/// <li> forEach: Perform a given action on each element.
	/// A variant form applies a given transformation on each element
	/// before performing the action.</li>
	/// 
	/// <li> search: Return the first available non-null result of
	/// applying a given function on each element; skipping further
	/// search when a result is found.</li>
	/// 
	/// <li> reduce: Accumulate each element.  The supplied reduction
	/// function cannot rely on ordering (more formally, it should be
	/// both associative and commutative).  There are five variants:
	/// 
	/// <ul>
	/// 
	/// <li> Plain reductions. (There is not a form of this method for
	/// (key, value) function arguments since there is no corresponding
	/// return type.)</li>
	/// 
	/// <li> Mapped reductions that accumulate the results of a given
	/// function applied to each element.</li>
	/// 
	/// <li> Reductions to scalar doubles, longs, and ints, using a
	/// given basis value.</li>
	/// 
	/// </ul>
	/// </li>
	/// </ul>
	/// 
	/// </para>
	/// <para>These bulk operations accept a {@code parallelismThreshold}
	/// argument. Methods proceed sequentially if the current map size is
	/// estimated to be less than the given threshold. Using a value of
	/// {@code Long.MAX_VALUE} suppresses all parallelism.  Using a value
	/// of {@code 1} results in maximal parallelism by partitioning into
	/// enough subtasks to fully utilize the {@link
	/// ForkJoinPool#commonPool()} that is used for all parallel
	/// computations. Normally, you would initially choose one of these
	/// extreme values, and then measure performance of using in-between
	/// values that trade off overhead versus throughput.
	/// 
	/// </para>
	/// <para>The concurrency properties of bulk operations follow
	/// from those of ConcurrentHashMap: Any non-null result returned
	/// from {@code get(key)} and related access methods bears a
	/// happens-before relation with the associated insertion or
	/// update.  The result of any bulk operation reflects the
	/// composition of these per-element relations (but is not
	/// necessarily atomic with respect to the map as a whole unless it
	/// is somehow known to be quiescent).  Conversely, because keys
	/// and values in the map are never null, null serves as a reliable
	/// atomic indicator of the current lack of any result.  To
	/// maintain this property, null serves as an implicit basis for
	/// all non-scalar reduction operations. For the double, long, and
	/// int versions, the basis should be one that, when combined with
	/// any other value, returns that other value (more formally, it
	/// should be the identity element for the reduction). Most common
	/// reductions have these properties; for example, computing a sum
	/// with basis 0 or a minimum with basis MAX_VALUE.
	/// 
	/// </para>
	/// <para>Search and transformation functions provided as arguments
	/// should similarly return null to indicate the lack of any result
	/// (in which case it is not used). In the case of mapped
	/// reductions, this also enables transformations to serve as
	/// filters, returning null (or, in the case of primitive
	/// specializations, the identity basis) if the element should not
	/// be combined. You can create compound transformations and
	/// filterings by composing them yourself under this "null means
	/// there is nothing there now" rule before using them in search or
	/// reduce operations.
	/// 
	/// </para>
	/// <para>Methods accepting and/or returning Entry arguments maintain
	/// key-value associations. They may be useful for example when
	/// finding the key for the greatest value. Note that "plain" Entry
	/// arguments can be supplied using {@code new
	/// AbstractMap.SimpleEntry(k,v)}.
	/// 
	/// </para>
	/// <para>Bulk operations may complete abruptly, throwing an
	/// exception encountered in the application of a supplied
	/// function. Bear in mind when handling such exceptions that other
	/// concurrently executing functions could also have thrown
	/// exceptions, or would have done so if the first exception had
	/// not occurred.
	/// 
	/// </para>
	/// <para>Speedups for parallel compared to sequential forms are common
	/// but not guaranteed.  Parallel operations involving brief functions
	/// on small maps may execute more slowly than sequential forms if the
	/// underlying work to parallelize the computation is more expensive
	/// than the computation itself.  Similarly, parallelization may not
	/// lead to much actual parallelism if all processors are busy
	/// performing unrelated tasks.
	/// 
	/// </para>
	/// <para>All arguments to all task methods must be non-null.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <K> the type of keys maintained by this map </param>
	/// @param <V> the type of mapped values </param>
	[Serializable]
	public class ConcurrentHashMap<K, V> : AbstractMap<K, V>, ConcurrentMap<K, V>
	{
		private const long SerialVersionUID = 7249069246763182397L;

		/*
		 * Overview:
		 *
		 * The primary design goal of this hash table is to maintain
		 * concurrent readability (typically method get(), but also
		 * iterators and related methods) while minimizing update
		 * contention. Secondary goals are to keep space consumption about
		 * the same or better than java.util.HashMap, and to support high
		 * initial insertion rates on an empty table by many threads.
		 *
		 * This map usually acts as a binned (bucketed) hash table.  Each
		 * key-value mapping is held in a Node.  Most nodes are instances
		 * of the basic Node class with hash, key, value, and next
		 * fields. However, various subclasses exist: TreeNodes are
		 * arranged in balanced trees, not lists.  TreeBins hold the roots
		 * of sets of TreeNodes. ForwardingNodes are placed at the heads
		 * of bins during resizing. ReservationNodes are used as
		 * placeholders while establishing values in computeIfAbsent and
		 * related methods.  The types TreeBin, ForwardingNode, and
		 * ReservationNode do not hold normal user keys, values, or
		 * hashes, and are readily distinguishable during search etc
		 * because they have negative hash fields and null key and value
		 * fields. (These special nodes are either uncommon or transient,
		 * so the impact of carrying around some unused fields is
		 * insignificant.)
		 *
		 * The table is lazily initialized to a power-of-two size upon the
		 * first insertion.  Each bin in the table normally contains a
		 * list of Nodes (most often, the list has only zero or one Node).
		 * Table accesses require volatile/atomic reads, writes, and
		 * CASes.  Because there is no other way to arrange this without
		 * adding further indirections, we use intrinsics
		 * (sun.misc.Unsafe) operations.
		 *
		 * We use the top (sign) bit of Node hash fields for control
		 * purposes -- it is available anyway because of addressing
		 * constraints.  Nodes with negative hash fields are specially
		 * handled or ignored in map methods.
		 *
		 * Insertion (via put or its variants) of the first node in an
		 * empty bin is performed by just CASing it to the bin.  This is
		 * by far the most common case for put operations under most
		 * key/hash distributions.  Other update operations (insert,
		 * delete, and replace) require locks.  We do not want to waste
		 * the space required to associate a distinct lock object with
		 * each bin, so instead use the first node of a bin list itself as
		 * a lock. Locking support for these locks relies on builtin
		 * "synchronized" monitors.
		 *
		 * Using the first node of a list as a lock does not by itself
		 * suffice though: When a node is locked, any update must first
		 * validate that it is still the first node after locking it, and
		 * retry if not. Because new nodes are always appended to lists,
		 * once a node is first in a bin, it remains first until deleted
		 * or the bin becomes invalidated (upon resizing).
		 *
		 * The main disadvantage of per-bin locks is that other update
		 * operations on other nodes in a bin list protected by the same
		 * lock can stall, for example when user equals() or mapping
		 * functions take a long time.  However, statistically, under
		 * random hash codes, this is not a common problem.  Ideally, the
		 * frequency of nodes in bins follows a Poisson distribution
		 * (http://en.wikipedia.org/wiki/Poisson_distribution) with a
		 * parameter of about 0.5 on average, given the resizing threshold
		 * of 0.75, although with a large variance because of resizing
		 * granularity. Ignoring variance, the expected occurrences of
		 * list size k are (exp(-0.5) * pow(0.5, k) / factorial(k)). The
		 * first values are:
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
		 * Lock contention probability for two threads accessing distinct
		 * elements is roughly 1 / (8 * #elements) under random hashes.
		 *
		 * Actual hash code distributions encountered in practice
		 * sometimes deviate significantly from uniform randomness.  This
		 * includes the case when N > (1<<30), so some keys MUST collide.
		 * Similarly for dumb or hostile usages in which multiple keys are
		 * designed to have identical hash codes or ones that differs only
		 * in masked-out high bits. So we use a secondary strategy that
		 * applies when the number of nodes in a bin exceeds a
		 * threshold. These TreeBins use a balanced tree to hold nodes (a
		 * specialized form of red-black trees), bounding search time to
		 * O(log N).  Each search step in a TreeBin is at least twice as
		 * slow as in a regular list, but given that N cannot exceed
		 * (1<<64) (before running out of addresses) this bounds search
		 * steps, lock hold times, etc, to reasonable constants (roughly
		 * 100 nodes inspected per operation worst case) so long as keys
		 * are Comparable (which is very common -- String, Long, etc).
		 * TreeBin nodes (TreeNodes) also maintain the same "next"
		 * traversal pointers as regular nodes, so can be traversed in
		 * iterators in the same way.
		 *
		 * The table is resized when occupancy exceeds a percentage
		 * threshold (nominally, 0.75, but see below).  Any thread
		 * noticing an overfull bin may assist in resizing after the
		 * initiating thread allocates and sets up the replacement array.
		 * However, rather than stalling, these other threads may proceed
		 * with insertions etc.  The use of TreeBins shields us from the
		 * worst case effects of overfilling while resizes are in
		 * progress.  Resizing proceeds by transferring bins, one by one,
		 * from the table to the next table. However, threads claim small
		 * blocks of indices to transfer (via field transferIndex) before
		 * doing so, reducing contention.  A generation stamp in field
		 * sizeCtl ensures that resizings do not overlap. Because we are
		 * using power-of-two expansion, the elements from each bin must
		 * either stay at same index, or move with a power of two
		 * offset. We eliminate unnecessary node creation by catching
		 * cases where old nodes can be reused because their next fields
		 * won't change.  On average, only about one-sixth of them need
		 * cloning when a table doubles. The nodes they replace will be
		 * garbage collectable as soon as they are no longer referenced by
		 * any reader thread that may be in the midst of concurrently
		 * traversing table.  Upon transfer, the old table bin contains
		 * only a special forwarding node (with hash field "MOVED") that
		 * contains the next table as its key. On encountering a
		 * forwarding node, access and update operations restart, using
		 * the new table.
		 *
		 * Each bin transfer requires its bin lock, which can stall
		 * waiting for locks while resizing. However, because other
		 * threads can join in and help resize rather than contend for
		 * locks, average aggregate waits become shorter as resizing
		 * progresses.  The transfer operation must also ensure that all
		 * accessible bins in both the old and new table are usable by any
		 * traversal.  This is arranged in part by proceeding from the
		 * last bin (table.length - 1) up towards the first.  Upon seeing
		 * a forwarding node, traversals (see class Traverser) arrange to
		 * move to the new table without revisiting nodes.  To ensure that
		 * no intervening nodes are skipped even when moved out of order,
		 * a stack (see class TableStack) is created on first encounter of
		 * a forwarding node during a traversal, to maintain its place if
		 * later processing the current table. The need for these
		 * save/restore mechanics is relatively rare, but when one
		 * forwarding node is encountered, typically many more will be.
		 * So Traversers use a simple caching scheme to avoid creating so
		 * many new TableStack nodes. (Thanks to Peter Levart for
		 * suggesting use of a stack here.)
		 *
		 * The traversal scheme also applies to partial traversals of
		 * ranges of bins (via an alternate Traverser constructor)
		 * to support partitioned aggregate operations.  Also, read-only
		 * operations give up if ever forwarded to a null table, which
		 * provides support for shutdown-style clearing, which is also not
		 * currently implemented.
		 *
		 * Lazy table initialization minimizes footprint until first use,
		 * and also avoids resizings when the first operation is from a
		 * putAll, constructor with map argument, or deserialization.
		 * These cases attempt to override the initial capacity settings,
		 * but harmlessly fail to take effect in cases of races.
		 *
		 * The element count is maintained using a specialization of
		 * LongAdder. We need to incorporate a specialization rather than
		 * just use a LongAdder in order to access implicit
		 * contention-sensing that leads to creation of multiple
		 * CounterCells.  The counter mechanics avoid contention on
		 * updates but can encounter cache thrashing if read too
		 * frequently during concurrent access. To avoid reading so often,
		 * resizing under contention is attempted only upon adding to a
		 * bin already holding two or more nodes. Under uniform hash
		 * distributions, the probability of this occurring at threshold
		 * is around 13%, meaning that only about 1 in 8 puts check
		 * threshold (and after resizing, many fewer do so).
		 *
		 * TreeBins use a special form of comparison for search and
		 * related operations (which is the main reason we cannot use
		 * existing collections such as TreeMaps). TreeBins contain
		 * Comparable elements, but may contain others, as well as
		 * elements that are Comparable but not necessarily Comparable for
		 * the same T, so we cannot invoke compareTo among them. To handle
		 * this, the tree is ordered primarily by hash value, then by
		 * Comparable.compareTo order if applicable.  On lookup at a node,
		 * if elements are not comparable or compare as 0 then both left
		 * and right children may need to be searched in the case of tied
		 * hash values. (This corresponds to the full list search that
		 * would be necessary if all elements were non-Comparable and had
		 * tied hashes.) On insertion, to keep a total ordering (or as
		 * close as is required here) across rebalancings, we compare
		 * classes and identityHashCodes as tie-breakers. The red-black
		 * balancing code is updated from pre-jdk-collections
		 * (http://gee.cs.oswego.edu/dl/classes/collections/RBCell.java)
		 * based in turn on Cormen, Leiserson, and Rivest "Introduction to
		 * Algorithms" (CLR).
		 *
		 * TreeBins also require an additional locking mechanism.  While
		 * list traversal is always possible by readers even during
		 * updates, tree traversal is not, mainly because of tree-rotations
		 * that may change the root node and/or its linkages.  TreeBins
		 * include a simple read-write lock mechanism parasitic on the
		 * main bin-synchronization strategy: Structural adjustments
		 * associated with an insertion or removal are already bin-locked
		 * (and so cannot conflict with other writers) but must wait for
		 * ongoing readers to finish. Since there can be only one such
		 * waiter, we use a simple scheme using a single "waiter" field to
		 * block writers.  However, readers need never block.  If the root
		 * lock is held, they proceed along the slow traversal path (via
		 * next-pointers) until the lock becomes available or the list is
		 * exhausted, whichever comes first. These cases are not fast, but
		 * maximize aggregate expected throughput.
		 *
		 * Maintaining API and serialization compatibility with previous
		 * versions of this class introduces several oddities. Mainly: We
		 * leave untouched but unused constructor arguments refering to
		 * concurrencyLevel. We accept a loadFactor constructor argument,
		 * but apply it only to initial table capacity (which is the only
		 * time that we can guarantee to honor it.) We also declare an
		 * unused "Segment" class that is instantiated in minimal form
		 * only when serializing.
		 *
		 * Also, solely for compatibility with previous versions of this
		 * class, it extends AbstractMap, even though all of its methods
		 * are overridden, so it is just useless baggage.
		 *
		 * This file is organized to make things a little easier to follow
		 * while reading than they might otherwise: First the main static
		 * declarations and utilities, then fields, then main public
		 * methods (with a few factorings of multiple public methods into
		 * internal ones), then sizing methods, trees, traversers, and
		 * bulk operations.
		 */

		/* ---------------- Constants -------------- */

		/// <summary>
		/// The largest possible table capacity.  This value must be
		/// exactly 1<<30 to stay within Java array allocation and indexing
		/// bounds for power of two table sizes, and is further required
		/// because the top two bits of 32bit hash fields are used for
		/// control purposes.
		/// </summary>
		private static readonly int MAXIMUM_CAPACITY = 1 << 30;

		/// <summary>
		/// The default initial table capacity.  Must be a power of 2
		/// (i.e., at least 1) and at most MAXIMUM_CAPACITY.
		/// </summary>
		private const int DEFAULT_CAPACITY = 16;

		/// <summary>
		/// The largest possible (non-power of two) array size.
		/// Needed by toArray and related methods.
		/// </summary>
		internal static readonly int MAX_ARRAY_SIZE = Integer.MaxValue - 8;

		/// <summary>
		/// The default concurrency level for this table. Unused but
		/// defined for compatibility with previous versions of this class.
		/// </summary>
		private const int DEFAULT_CONCURRENCY_LEVEL = 16;

		/// <summary>
		/// The load factor for this table. Overrides of this value in
		/// constructors affect only the initial table capacity.  The
		/// actual floating point value isn't normally used -- it is
		/// simpler to use expressions such as {@code n - (n >>> 2)} for
		/// the associated resizing threshold.
		/// </summary>
		private const float LOAD_FACTOR = 0.75f;

		/// <summary>
		/// The bin count threshold for using a tree rather than list for a
		/// bin.  Bins are converted to trees when adding an element to a
		/// bin with at least this many nodes. The value must be greater
		/// than 2, and should be at least 8 to mesh with assumptions in
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
		/// The value should be at least 4 * TREEIFY_THRESHOLD to avoid
		/// conflicts between resizing and treeification thresholds.
		/// </summary>
		internal const int MIN_TREEIFY_CAPACITY = 64;

		/// <summary>
		/// Minimum number of rebinnings per transfer step. Ranges are
		/// subdivided to allow multiple resizer threads.  This value
		/// serves as a lower bound to avoid resizers encountering
		/// excessive memory contention.  The value should be at least
		/// DEFAULT_CAPACITY.
		/// </summary>
		private const int MIN_TRANSFER_STRIDE = 16;

		/// <summary>
		/// The number of bits used for generation stamp in sizeCtl.
		/// Must be at least 6 for 32bit arrays.
		/// </summary>
		private static int RESIZE_STAMP_BITS = 16;

		/// <summary>
		/// The maximum number of threads that can help resize.
		/// Must fit in 32 - RESIZE_STAMP_BITS bits.
		/// </summary>
		private static readonly int MAX_RESIZERS = (1 << (32 - RESIZE_STAMP_BITS)) - 1;

		/// <summary>
		/// The bit shift for recording size stamp in sizeCtl.
		/// </summary>
		private static readonly int RESIZE_STAMP_SHIFT = 32 - RESIZE_STAMP_BITS;

		/*
		 * Encodings for Node hash fields. See above for explanation.
		 */
		internal const int MOVED = -1; // hash for forwarding nodes
		internal const int TREEBIN = -2; // hash for roots of trees
		internal const int RESERVED = -3; // hash for transient reservations
		internal const int HASH_BITS = 0x7fffffff; // usable bits of normal node hash

		/// <summary>
		/// Number of CPUS, to place bounds on some sizings </summary>
		internal static readonly int NCPU = Runtime.Runtime.availableProcessors();

		/// <summary>
		/// For serialization compatibility. </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("segments", typeof(Segment[])), new ObjectStreamField("segmentMask", Integer.TYPE), new ObjectStreamField("segmentShift", Integer.TYPE)};

		/* ---------------- Nodes -------------- */

		/// <summary>
		/// Key-value entry.  This class is never exported out as a
		/// user-mutable Map.Entry (i.e., one supporting setValue; see
		/// MapEntry below), but can be used for read-only traversals used
		/// in bulk tasks.  Subclasses of Node with a negative hash field
		/// are special, and contain null keys and values (but are never
		/// exported).  Otherwise, keys and vals are never null.
		/// </summary>
		internal class Node<K, V> : java.util.Map_Entry<K, V>
		{
			internal readonly int Hash;
			internal readonly K Key_Renamed;
			internal volatile V Val;
			internal volatile Node<K, V> Next;

			internal Node(int hash, K key, V val, Node<K, V> next)
			{
				this.Hash = hash;
				this.Key_Renamed = key;
				this.Val = val;
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
					return Val;
				}
			}
			public sealed override int HashCode()
			{
				return Key_Renamed.HashCode() ^ Val.HashCode();
			}
			public sealed override String ToString()
			{
				return Key_Renamed + "=" + Val;
			}
			public V SetValue(V ConcurrentMap_Fields)
			{
				throw new UnsupportedOperationException();
			}

			public sealed override bool Equals(Object o)
			{
				Object java.util.Map_Fields.k, java.util.Map_Fields.v, u;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map_Entry<?,?> e;
				java.util.Map_Entry<?, ?> e;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((o instanceof java.util.Map_Entry) && (java.util.Map_Fields.k = (e = (java.util.Map_Entry<?,?>)o).getKey()) != java.util.Map_Fields.null && (java.util.Map_Fields.v = e.getValue()) != java.util.Map_Fields.null && (java.util.Map_Fields.k == key || java.util.Map_Fields.k.equals(key)) && (java.util.Map_Fields.v == (u = val) || java.util.Map_Fields.v.equals(u)));
				return ((o is java.util.Map_Entry) && (java.util.Map_Fields.k = (e = (java.util.Map_Entry<?, ?>)o).Key) != java.util.Map_Fields.Null && (java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && (java.util.Map_Fields.k == Key_Renamed || java.util.Map_Fields.k.Equals(Key_Renamed)) && (java.util.Map_Fields.v == (u = Val) || java.util.Map_Fields.v.Equals(u)));
			}

			/// <summary>
			/// Virtualized support for map.get(); overridden in subclasses.
			/// </summary>
			internal virtual Node<K, V> Find(int h, Object java)
			{
				Node<K, V> e = this;
				if (java.util.Map_Fields.k != java.util.Map_Fields.Null)
				{
					do
					{
						K ek;
						if (e.Hash == h && ((ek = e.Key_Renamed) == java.util.Map_Fields.k || (ek != java.util.Map_Fields.Null && java.util.Map_Fields.k.Equals(ek))))
						{
							return e;
						}
					} while ((e = e.Next) != java.util.Map_Fields.Null);
				}
				return java.util.Map_Fields.Null;
			}
		}

		/* ---------------- Static utilities -------------- */

		/// <summary>
		/// Spreads (XORs) higher bits of hash to lower and also forces top
		/// bit to 0. Because the table uses power-of-two masking, sets of
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
		internal static int Spread(int h)
		{
			return (h ^ ((int)((uint)h >> 16))) & HASH_BITS;
		}

		/// <summary>
		/// Returns a power of two table size for the given desired capacity.
		/// See Hackers Delight, sec 3.2
		/// </summary>
		private static int TableSizeFor(int c)
		{
			int n = c - 1;
			n |= (int)((uint)n >> 1);
			n |= (int)((uint)n >> 2);
			n |= (int)((uint)n >> 4);
			n |= (int)((uint)n >> 8);
			n |= (int)((uint)n >> 16);
			return (n < 0) ? 1 : (n >= MAXIMUM_CAPACITY) ? MAXIMUM_CAPACITY : n + 1;
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
				if ((ts = c.GenericInterfaces) != java.util.Map_Fields.Null)
				{
					for (int i = 0; i < ts.Length; ++i)
					{
						if (((t = ts[i]) is ParameterizedType) && ((p = (ParameterizedType)t).RawType == typeof(Comparable)) && (@as = p.ActualTypeArguments) != java.util.Map_Fields.Null && @as.Length == 1 && @as[0] == c) // type arg is c
						{
							return c;
						}
					}
				}
			}
			return java.util.Map_Fields.Null;
		}

		/// <summary>
		/// Returns k.compareTo(x) if x matches kc (k's screened comparable
		/// class), else 0.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"rawtypes","unchecked"}) static int compareComparables(Class kc, Object java.util.Map_Fields.k, Object x)
		internal static int CompareComparables(Class kc, Object java, Object x) // for cast to Comparable
		{
			return (x == java.util.Map_Fields.Null || x.GetType() != kc ? 0 : ((Comparable)java.util.Map_Fields.k).CompareTo(x));
		}

		/* ---------------- Table element access -------------- */

		/*
		 * Volatile access methods are used for table elements as well as
		 * elements of in-progress next table while resizing.  All uses of
		 * the tab arguments must be null checked by callers.  All callers
		 * also paranoically precheck that tab's length is not zero (or an
		 * equivalent check), thus ensuring that any index argument taking
		 * the form of a hash value anded with (length - 1) is a valid
		 * index.  Note that, to be correct wrt arbitrary concurrency
		 * errors by users, these checks must operate on local variables,
		 * which accounts for some odd-looking inline assignments below.
		 * Note that calls to setTabAt always occur within locked regions,
		 * and so in principle require only release ordering, not
		 * full volatile semantics, but are currently coded as volatile
		 * writes to be conservative.
		 */

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") static final <K,V> Node<K,V> tabAt(Node<K,V>[] tab, int i)
		internal static Node<K, V> tabAt<K, V>(Node<K, V>[] tab, int i)
		{
			return (Node<K, V>)U.getObjectVolatile(tab, ((long)i << ASHIFT) + ABASE);
		}

		internal static bool casTabAt<K, V>(Node<K, V>[] tab, int i, Node<K, V> c, Node<K, V> java)
		{
			return U.compareAndSwapObject(tab, ((long)i << ASHIFT) + ABASE, c, java.util.Map_Fields.v);
		}

		internal static void setTabAt<K, V>(Node<K, V>[] tab, int i, Node<K, V> java)
		{
			U.putObjectVolatile(tab, ((long)i << ASHIFT) + ABASE, java.util.Map_Fields.v);
		}

		/* ---------------- Fields -------------- */

		/// <summary>
		/// The array of bins. Lazily initialized upon first insertion.
		/// Size is always a power of two. Accessed directly by iterators.
		/// </summary>
		[NonSerialized]
		internal volatile Node<K, V>[] Table;

		/// <summary>
		/// The next table to use; non-null only while resizing.
		/// </summary>
		[NonSerialized]
		private volatile Node<K, V>[] NextTable;

		/// <summary>
		/// Base counter value, used mainly when there is no contention,
		/// but also as a fallback during table initialization
		/// races. Updated via CAS.
		/// </summary>
		[NonSerialized]
		private volatile long BaseCount;

		/// <summary>
		/// Table initialization and resizing control.  When negative, the
		/// table is being initialized or resized: -1 for initialization,
		/// else -(1 + the number of active resizing threads).  Otherwise,
		/// when table is null, holds the initial table size to use upon
		/// creation, or 0 for default. After initialization, holds the
		/// next element count value upon which to resize the table.
		/// </summary>
		[NonSerialized]
		private volatile int SizeCtl;

		/// <summary>
		/// The next table index (plus one) to split while resizing.
		/// </summary>
		[NonSerialized]
		private volatile int TransferIndex;

		/// <summary>
		/// Spinlock (locked via CAS) used when resizing and/or creating CounterCells.
		/// </summary>
		[NonSerialized]
		private volatile int CellsBusy;

		/// <summary>
		/// Table of counter cells. When non-null, size is a power of 2.
		/// </summary>
		[NonSerialized]
		private volatile CounterCell[] CounterCells;

		// views
		[NonSerialized]
		private KeySetView<K, V> KeySet_Renamed;
		[NonSerialized]
		private ValuesView<K, V> Values_Renamed;
		[NonSerialized]
		private EntrySetView<K, V> EntrySet_Renamed;


		/* ---------------- Public operations -------------- */

		/// <summary>
		/// Creates a new, empty map with the default initial table size (16).
		/// </summary>
		public ConcurrentHashMap()
		{
		}

		/// <summary>
		/// Creates a new, empty map with an initial table size
		/// accommodating the specified number of elements without the need
		/// to dynamically resize.
		/// </summary>
		/// <param name="initialCapacity"> The implementation performs internal
		/// sizing to accommodate this many elements. </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity of
		/// elements is negative </exception>
		public ConcurrentHashMap(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new IllegalArgumentException();
			}
			int cap = ((initialCapacity >= ((int)((uint)MAXIMUM_CAPACITY >> 1))) ? MAXIMUM_CAPACITY : TableSizeFor(initialCapacity + ((int)((uint)initialCapacity >> 1)) + 1));
			this.SizeCtl = cap;
		}

		/// <summary>
		/// Creates a new map with the same mappings as the given map.
		/// </summary>
		/// <param name="m"> the map </param>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public ConcurrentHashMap(java.util.Map<? extends K, ? extends V> m)
		public ConcurrentHashMap<T1>(IDictionary<T1> m) where T1 : K where ? : V
		{
			this.SizeCtl = DEFAULT_CAPACITY;
			PutAll(m);
		}

		/// <summary>
		/// Creates a new, empty map with an initial table size based on
		/// the given number of elements ({@code initialCapacity}) and
		/// initial table density ({@code loadFactor}).
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity. The implementation
		/// performs internal sizing to accommodate this many elements,
		/// given the specified load factor. </param>
		/// <param name="loadFactor"> the load factor (table density) for
		/// establishing the initial table size </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity of
		/// elements is negative or the load factor is nonpositive
		/// 
		/// @since 1.6 </exception>
		public ConcurrentHashMap(int initialCapacity, float loadFactor) : this(initialCapacity, loadFactor, 1)
		{
		}

		/// <summary>
		/// Creates a new, empty map with an initial table size based on
		/// the given number of elements ({@code initialCapacity}), table
		/// density ({@code loadFactor}), and number of concurrently
		/// updating threads ({@code concurrencyLevel}).
		/// </summary>
		/// <param name="initialCapacity"> the initial capacity. The implementation
		/// performs internal sizing to accommodate this many elements,
		/// given the specified load factor. </param>
		/// <param name="loadFactor"> the load factor (table density) for
		/// establishing the initial table size </param>
		/// <param name="concurrencyLevel"> the estimated number of concurrently
		/// updating threads. The implementation may use this value as
		/// a sizing hint. </param>
		/// <exception cref="IllegalArgumentException"> if the initial capacity is
		/// negative or the load factor or concurrencyLevel are
		/// nonpositive </exception>
		public ConcurrentHashMap(int initialCapacity, float loadFactor, int concurrencyLevel)
		{
			if (!(loadFactor > 0.0f) || initialCapacity < 0 || concurrencyLevel <= 0)
			{
				throw new IllegalArgumentException();
			}
			if (initialCapacity < concurrencyLevel) // Use at least as many bins
			{
				initialCapacity = concurrencyLevel; // as estimated threads
			}
			long size = (long)(1.0 + (long)initialCapacity / loadFactor);
			int cap = (size >= (long)MAXIMUM_CAPACITY) ? MAXIMUM_CAPACITY : TableSizeFor((int)size);
			this.SizeCtl = cap;
		}

		// Original (since JDK1.2) Map methods

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual int Count
		{
			get
			{
				long n = SumCount();
				return ((n < 0L) ? 0 : (n > (long)Integer.MaxValue) ? Integer.MaxValue : (int)n);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual bool Empty
		{
			get
			{
				return SumCount() <= 0L; // ignore transient negative values
			}
		}

		/// <summary>
		/// Returns the value to which the specified key is mapped,
		/// or {@code null} if this map contains no mapping for the key.
		/// 
		/// <para>More formally, if this map contains a mapping from a key
		/// {@code k} to a value {@code v} such that {@code key.equals(k)},
		/// then this method returns {@code v}; otherwise it returns
		/// {@code null}.  (There can be at most one such mapping.)
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual V Get(Object key)
		{
			Node<K, V>[] tab;
			Node<K, V> e, p;
			int n, eh;
			K ek;
			int h = Spread(key.HashCode());
			if ((tab = Table) != java.util.Map_Fields.Null && (n = tab.Length) > 0 && (e = TabAt(tab, (n - 1) & h)) != java.util.Map_Fields.Null)
			{
				if ((eh = e.Hash) == h)
				{
					if ((ek = e.Key_Renamed) == key || (ek != java.util.Map_Fields.Null && key.Equals(ek)))
					{
						return e.Val;
					}
				}
				else if (eh < 0)
				{
					return (p = e.Find(h, key)) != java.util.Map_Fields.Null ? p.Val : java.util.Map_Fields.Null;
				}
				while ((e = e.Next) != java.util.Map_Fields.Null)
				{
					if (e.Hash == h && ((ek = e.Key_Renamed) == key || (ek != java.util.Map_Fields.Null && key.Equals(ek))))
					{
						return e.Val;
					}
				}
			}
			return java.util.Map_Fields.Null;
		}

		/// <summary>
		/// Tests if the specified object is a key in this table.
		/// </summary>
		/// <param name="key"> possible key </param>
		/// <returns> {@code true} if and only if the specified object
		///         is a key in this table, as determined by the
		///         {@code equals} method; {@code false} otherwise </returns>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual bool ContainsKey(Object key)
		{
			return Get(key) != java.util.Map_Fields.Null;
		}

		/// <summary>
		/// Returns {@code true} if this map maps one or more keys to the
		/// specified value. Note: This method may require a full traversal
		/// of the map, and is much slower than method {@code containsKey}.
		/// </summary>
		/// <param name="value"> value whose presence in this map is to be tested </param>
		/// <returns> {@code true} if this map maps one or more keys to the
		///         specified value </returns>
		/// <exception cref="NullPointerException"> if the specified value is null </exception>
		public virtual bool ContainsValue(Object ConcurrentMap_Fields)
		{
			if (ConcurrentMap_Fields.Value == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			Node<K, V>[] t;
			if ((t = Table) != java.util.Map_Fields.Null)
			{
				Traverser<K, V> it = new Traverser<K, V>(t, t.Length, 0, t.Length);
				for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
				{
					V java.util.Map_Fields.v;
					if ((java.util.Map_Fields.v = p.val) == ConcurrentMap_Fields.Value || (java.util.Map_Fields.v != java.util.Map_Fields.Null && ConcurrentMap_Fields.Value.Equals(java.util.Map_Fields.v)))
					{
						return java.util.Map_Fields.True;
					}
				}
			}
			return java.util.Map_Fields.False;
		}

		/// <summary>
		/// Maps the specified key to the specified value in this table.
		/// Neither the key nor the value can be null.
		/// 
		/// <para>The value can be retrieved by calling the {@code get} method
		/// with a key that is equal to the original key.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="value"> value to be associated with the specified key </param>
		/// <returns> the previous value associated with {@code key}, or
		///         {@code null} if there was no mapping for {@code key} </returns>
		/// <exception cref="NullPointerException"> if the specified key or value is null </exception>
		public virtual V Put(K key, V ConcurrentMap_Fields)
		{
			return PutVal(key, ConcurrentMap_Fields.Value, java.util.Map_Fields.False);
		}

		/// <summary>
		/// Implementation for put and putIfAbsent </summary>
		internal V PutVal(K key, V ConcurrentMap_Fields, bool onlyIfAbsent)
		{
			if (key == java.util.Map_Fields.Null || ConcurrentMap_Fields.Value == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int hash = Spread(key.HashCode());
			int binCount = 0;
			for (Node<K, V>[] tab = Table;;)
			{
				Node<K, V> f;
				int n, i, fh;
				if (tab == java.util.Map_Fields.Null || (n = tab.Length) == 0)
				{
					tab = InitTable();
				}
				else if ((f = TabAt(tab, i = (n - 1) & hash)) == java.util.Map_Fields.Null)
				{
					if (CasTabAt(tab, i, java.util.Map_Fields.Null, new Node<K, V>(hash, key, ConcurrentMap_Fields.Value, java.util.Map_Fields.Null)))
					{
						ConcurrentMap_Fields.break; // no lock when adding to empty bin
					}
				}
				else if ((fh = f.Hash) == MOVED)
				{
					tab = HelpTransfer(tab, f);
				}
				else
				{
					V oldVal = java.util.Map_Fields.Null;
					lock (f)
					{
						if (TabAt(tab, i) == f)
						{
							if (fh >= 0)
							{
								binCount = 1;
								for (Node<K, V> e = f;; ++binCount)
								{
									K ek;
									if (e.Hash == hash && ((ek = e.Key_Renamed) == key || (ek != java.util.Map_Fields.Null && key.Equals(ek))))
									{
										oldVal = e.Val;
										if (!onlyIfAbsent)
										{
											e.Val = ConcurrentMap_Fields.Value;
										}
										ConcurrentMap_Fields.break;
									}
									Node<K, V> pred = e;
									if ((e = e.next) == java.util.Map_Fields.Null)
									{
										pred.Next = new Node<K, V>(hash, key, ConcurrentMap_Fields.Value, java.util.Map_Fields.Null);
										ConcurrentMap_Fields.break;
									}
								}
							}
							else if (f is TreeBin)
							{
								Node<K, V> p;
								binCount = 2;
								if ((p = ((TreeBin<K, V>)f).PutTreeVal(hash, key, ConcurrentMap_Fields.Value)) != java.util.Map_Fields.Null)
								{
									oldVal = p.Val;
									if (!onlyIfAbsent)
									{
										p.Val = ConcurrentMap_Fields.Value;
									}
								}
							}
						}
					}
					if (binCount != 0)
					{
						if (binCount >= TREEIFY_THRESHOLD)
						{
							TreeifyBin(tab, i);
						}
						if (oldVal != java.util.Map_Fields.Null)
						{
							return oldVal;
						}
						ConcurrentMap_Fields.break;
					}
				}
			}
			AddCount(1L, binCount);
			return java.util.Map_Fields.Null;
		}

		/// <summary>
		/// Copies all of the mappings from the specified map to this one.
		/// These mappings replace any mappings that this map had for any of the
		/// keys currently in the specified map.
		/// </summary>
		/// <param name="m"> mappings to be stored in this map </param>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(java.util.Map<? extends K, ? extends V> m)
		public virtual void putAll<T1>(IDictionary<T1> m) where T1 : K where ? : V
		{
			TryPresize(m.Count);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.util.Map_Entry<? extends K, ? extends V> e : m.entrySet())
			foreach (java.util.Map_Entry<?, ?> e in m)
			{
				PutVal(e.Key, e.Value, java.util.Map_Fields.False);
			}
		}

		/// <summary>
		/// Removes the key (and its corresponding value) from this map.
		/// This method does nothing if the key is not in the map.
		/// </summary>
		/// <param name="key"> the key that needs to be removed </param>
		/// <returns> the previous value associated with {@code key}, or
		///         {@code null} if there was no mapping for {@code key} </returns>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual V Remove(Object key)
		{
			return ReplaceNode(key, java.util.Map_Fields.Null, java.util.Map_Fields.Null);
		}

		/// <summary>
		/// Implementation for the four public remove/replace methods:
		/// Replaces node value with v, conditional upon match of cv if
		/// non-null.  If resulting value is null, delete.
		/// </summary>
		internal V ReplaceNode(Object key, V ConcurrentMap_Fields, Object cv)
		{
			int hash = Spread(key.HashCode());
			for (Node<K, V>[] tab = Table;;)
			{
				Node<K, V> f;
				int n, i, fh;
				if (tab == java.util.Map_Fields.Null || (n = tab.Length) == 0 || (f = TabAt(tab, i = (n - 1) & hash)) == java.util.Map_Fields.Null)
				{
					ConcurrentMap_Fields.break;
				}
				else if ((fh = f.Hash) == MOVED)
				{
					tab = HelpTransfer(tab, f);
				}
				else
				{
					V oldVal = java.util.Map_Fields.Null;
					bool validated = java.util.Map_Fields.False;
					lock (f)
					{
						if (TabAt(tab, i) == f)
						{
							if (fh >= 0)
							{
								validated = java.util.Map_Fields.True;
								for (Node<K, V> e = f, pred = java.util.Map_Fields.Null;;)
								{
									K ek;
									if (e.Hash == hash && ((ek = e.Key_Renamed) == key || (ek != java.util.Map_Fields.Null && key.Equals(ek))))
									{
										V ev = e.Val;
										if (cv == java.util.Map_Fields.Null || cv == ev || (ev != java.util.Map_Fields.Null && cv.Equals(ev)))
										{
											oldVal = ev;
											if (ConcurrentMap_Fields.Value != java.util.Map_Fields.Null)
											{
												e.Val = ConcurrentMap_Fields.Value;
											}
											else if (pred != java.util.Map_Fields.Null)
											{
												pred.next = e.Next;
											}
											else
											{
												SetTabAt(tab, i, e.Next);
											}
										}
										ConcurrentMap_Fields.break;
									}
									pred = e;
									if ((e = e.next) == java.util.Map_Fields.Null)
									{
										ConcurrentMap_Fields.break;
									}
								}
							}
							else if (f is TreeBin)
							{
								validated = java.util.Map_Fields.True;
								TreeBin<K, V> t = (TreeBin<K, V>)f;
								TreeNode<K, V> r, p;
								if ((r = t.Root) != java.util.Map_Fields.Null && (p = r.FindTreeNode(hash, key, java.util.Map_Fields.Null)) != java.util.Map_Fields.Null)
								{
									V pv = p.Val;
									if (cv == java.util.Map_Fields.Null || cv == pv || (pv != java.util.Map_Fields.Null && cv.Equals(pv)))
									{
										oldVal = pv;
										if (ConcurrentMap_Fields.Value != java.util.Map_Fields.Null)
										{
											p.Val = ConcurrentMap_Fields.Value;
										}
										else if (t.RemoveTreeNode(p))
										{
											SetTabAt(tab, i, Untreeify(t.First));
										}
									}
								}
							}
						}
					}
					if (validated)
					{
						if (oldVal != java.util.Map_Fields.Null)
						{
							if (ConcurrentMap_Fields.Value == java.util.Map_Fields.Null)
							{
								AddCount(-1L, -1);
							}
							return oldVal;
						}
						ConcurrentMap_Fields.break;
					}
				}
			}
			return java.util.Map_Fields.Null;
		}

		/// <summary>
		/// Removes all of the mappings from this map.
		/// </summary>
		public virtual void Clear()
		{
			long delta = 0L; // negative number of deletions
			int i = 0;
			Node<K, V>[] tab = Table;
			while (tab != java.util.Map_Fields.Null && i < tab.Length)
			{
				int fh;
				Node<K, V> f = TabAt(tab, i);
				if (f == java.util.Map_Fields.Null)
				{
					++i;
				}
				else if ((fh = f.Hash) == MOVED)
				{
					tab = HelpTransfer(tab, f);
					i = 0; // restart
				}
				else
				{
					lock (f)
					{
						if (TabAt(tab, i) == f)
						{
							Node<K, V> p = (fh >= 0 ? f : (f is TreeBin) ? ((TreeBin<K, V>)f).First : java.util.Map_Fields.Null);
							while (p != java.util.Map_Fields.Null)
							{
								--delta;
								p = p.Next;
							}
							SetTabAt(tab, i++, java.util.Map_Fields.Null);
						}
					}
				}
			}
			if (delta != 0L)
			{
				AddCount(delta, -1);
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the keys contained in this map.
		/// The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa. The set supports element
		/// removal, which removes the corresponding mapping from this map,
		/// via the {@code Iterator.remove}, {@code Set.remove},
		/// {@code removeAll}, {@code retainAll}, and {@code clear}
		/// operations.  It does not support the {@code add} or
		/// {@code addAll} operations.
		/// 
		/// <para>The view's iterators and spliterators are
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// <para>The view's {@code spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>,
		/// <seealso cref="Spliterator#DISTINCT"/>, and <seealso cref="Spliterator#NONNULL"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the set view </returns>
		public virtual KeySetView<K, V> KeySet()
		{
			KeySetView<K, V> ks;
			return (ks = KeySet_Renamed) != java.util.Map_Fields.Null ? ks : (KeySet_Renamed = new KeySetView<K, V>(this, java.util.Map_Fields.Null));
		}

		/// <summary>
		/// Returns a <seealso cref="Collection"/> view of the values contained in this map.
		/// The collection is backed by the map, so changes to the map are
		/// reflected in the collection, and vice-versa.  The collection
		/// supports element removal, which removes the corresponding
		/// mapping from this map, via the {@code Iterator.remove},
		/// {@code Collection.remove}, {@code removeAll},
		/// {@code retainAll}, and {@code clear} operations.  It does not
		/// support the {@code add} or {@code addAll} operations.
		/// 
		/// <para>The view's iterators and spliterators are
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// <para>The view's {@code spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>
		/// and <seealso cref="Spliterator#NONNULL"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the collection view </returns>
		public virtual ICollection<V> Values
		{
			get
			{
				ValuesView<K, V> vs;
				return (vs = Values_Renamed) != java.util.Map_Fields.Null ? vs : (Values_Renamed = new ValuesView<K, V>(this));
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		/// The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  The set supports element
		/// removal, which removes the corresponding mapping from the map,
		/// via the {@code Iterator.remove}, {@code Set.remove},
		/// {@code removeAll}, {@code retainAll}, and {@code clear}
		/// operations.
		/// 
		/// <para>The view's iterators and spliterators are
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// <para>The view's {@code spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>,
		/// <seealso cref="Spliterator#DISTINCT"/>, and <seealso cref="Spliterator#NONNULL"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the set view </returns>
		public virtual Set<java.util.Map_Entry<K, V>> EntrySet()
		{
			EntrySetView<K, V> es;
			return (es = EntrySet_Renamed) != java.util.Map_Fields.Null ? es : (EntrySet_Renamed = new EntrySetView<K, V>(this));
		}

		/// <summary>
		/// Returns the hash code value for this <seealso cref="Map"/>, i.e.,
		/// the sum of, for each key-value pair in the map,
		/// {@code key.hashCode() ^ value.hashCode()}.
		/// </summary>
		/// <returns> the hash code value for this map </returns>
		public override int HashCode()
		{
			int h = 0;
			Node<K, V>[] t;
			if ((t = Table) != java.util.Map_Fields.Null)
			{
				Traverser<K, V> it = new Traverser<K, V>(t, t.Length, 0, t.Length);
				for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
				{
					h += p.key.HashCode() ^ p.val.HashCode();
				}
			}
			return h;
		}

		/// <summary>
		/// Returns a string representation of this map.  The string
		/// representation consists of a list of key-value mappings (in no
		/// particular order) enclosed in braces ("{@code {}}").  Adjacent
		/// mappings are separated by the characters {@code ", "} (comma
		/// and space).  Each key-value mapping is rendered as the key
		/// followed by an equals sign ("{@code =}") followed by the
		/// associated value.
		/// </summary>
		/// <returns> a string representation of this map </returns>
		public override String ToString()
		{
			Node<K, V>[] t;
			int f = (t = Table) == java.util.Map_Fields.Null ? 0 : t.Length;
			Traverser<K, V> it = new Traverser<K, V>(t, f, 0, f);
			StringBuilder sb = new StringBuilder();
			sb.Append('{');
			Node<K, V> p;
			if ((p = it.Advance()) != java.util.Map_Fields.Null)
			{
				for (;;)
				{
					K java.util.Map_Fields.k = p.Key_Renamed;
					V java.util.Map_Fields.v = p.Val;
					sb.Append(java.util.Map_Fields.k == this ? "(this Map)" : java.util.Map_Fields.k);
					sb.Append('=');
					sb.Append(java.util.Map_Fields.v == this ? "(this Map)" : java.util.Map_Fields.v);
					if ((p = it.Advance()) == java.util.Map_Fields.Null)
					{
						ConcurrentMap_Fields.break;
					}
					sb.Append(',').Append(' ');
				}
			}
			return sb.Append('}').ToString();
		}

		/// <summary>
		/// Compares the specified object with this map for equality.
		/// Returns {@code true} if the given object is a map with the same
		/// mappings as this map.  This operation may return misleading
		/// results if either map is concurrently modified during execution
		/// of this method.
		/// </summary>
		/// <param name="o"> object to be compared for equality with this map </param>
		/// <returns> {@code true} if the specified object is equal to this map </returns>
		public override bool Equals(Object o)
		{
			if (o != this)
			{
				if (!(o is IDictionary))
				{
					return java.util.Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<?,?> m = (java.util.Map<?,?>) o;
				IDictionary<?, ?> m = (IDictionary<?, ?>) o;
				Node<K, V>[] t;
				int f = (t = Table) == java.util.Map_Fields.Null ? 0 : t.Length;
				Traverser<K, V> it = new Traverser<K, V>(t, f, 0, f);
				for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
				{
					V val = p.val;
					Object java.util.Map_Fields.v = m[p.key];
					if (java.util.Map_Fields.v == java.util.Map_Fields.Null || (java.util.Map_Fields.v != val && !java.util.Map_Fields.v.Equals(val)))
					{
						return java.util.Map_Fields.False;
					}
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.util.Map_Entry<?,?> e : m.entrySet())
				foreach (java.util.Map_Entry<?, ?> e in m)
				{
					Object mk, mv, java;
					if ((mk = e.Key) == java.util.Map_Fields.Null || (mv = e.Value) == java.util.Map_Fields.Null || (java.util.Map_Fields.v = Get(mk)) == java.util.Map_Fields.Null || (mv != java.util.Map_Fields.v && !mv.Equals(java.util.Map_Fields.v)))
					{
						return java.util.Map_Fields.False;
					}
				}
			}
			return java.util.Map_Fields.True;
		}

		/// <summary>
		/// Stripped-down version of helper class used in previous version,
		/// declared for the sake of serialization compatibility
		/// </summary>
		[Serializable]
		internal class Segment<K, V> : ReentrantLock
		{
			internal const long SerialVersionUID = 2249069246763182397L;
			internal readonly float LoadFactor;
			internal Segment(float lf)
			{
				this.LoadFactor = lf;
			}
		}

		/// <summary>
		/// Saves the state of the {@code ConcurrentHashMap} instance to a
		/// stream (i.e., serializes it). </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.IOException"> if an I/O error occurs
		/// @serialData
		/// the key (Object) and value (Object)
		/// for each key-value mapping, followed by a null pair.
		/// The key-value mappings are emitted in no particular order. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// For serialization compatibility
			// Emulate segment calculation from previous version of this class
			int sshift = 0;
			int ssize = 1;
			while (ssize < DEFAULT_CONCURRENCY_LEVEL)
			{
				++sshift;
				ssize <<= 1;
			}
			int segmentShift = 32 - sshift;
			int segmentMask = ssize - 1;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Segment<K,V>[] segments = (Segment<K,V>[]) new Segment<?,?>[DEFAULT_CONCURRENCY_LEVEL];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Segment<K, V>[] segments = (Segment<K, V>[]) new Segment<?, ?>[DEFAULT_CONCURRENCY_LEVEL];
			for (int i = 0; i < segments.Length; ++i)
			{
				segments[i] = new Segment<K, V>(LOAD_FACTOR);
			}
			s.PutFields().Put("segments", segments);
			s.PutFields().Put("segmentShift", segmentShift);
			s.PutFields().Put("segmentMask", segmentMask);
			s.WriteFields();

			Node<K, V>[] t;
			if ((t = Table) != java.util.Map_Fields.Null)
			{
				Traverser<K, V> it = new Traverser<K, V>(t, t.Length, 0, t.Length);
				for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
				{
					s.WriteObject(p.key);
					s.WriteObject(p.val);
				}
			}
			s.WriteObject(java.util.Map_Fields.Null);
			s.WriteObject(java.util.Map_Fields.Null);
			segments = java.util.Map_Fields.Null; // throw away
		}

		/// <summary>
		/// Reconstitutes the instance from a stream (that is, deserializes it). </summary>
		/// <param name="s"> the stream </param>
		/// <exception cref="ClassNotFoundException"> if the class of a serialized object
		///         could not be found </exception>
		/// <exception cref="java.io.IOException"> if an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			/*
			 * To improve performance in typical cases, we create nodes
			 * while reading, then place in table once size is known.
			 * However, we must also validate uniqueness and deal with
			 * overpopulated bins while doing so, which requires
			 * specialized versions of putVal mechanics.
			 */
			SizeCtl = -1; // force exclusion for table construction
			s.DefaultReadObject();
			long size = 0L;
			Node<K, V> p = java.util.Map_Fields.Null;
			for (;;)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") K java.util.Map_Fields.k = (K) s.readObject();
				K java.util.Map_Fields.k = (K) s.ReadObject();
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") V java.util.Map_Fields.v = (V) s.readObject();
				V java.util.Map_Fields.v = (V) s.ReadObject();
				if (java.util.Map_Fields.k != java.util.Map_Fields.Null && java.util.Map_Fields.v != java.util.Map_Fields.Null)
				{
					p = new Node<K, V>(Spread(java.util.Map_Fields.k.HashCode()), java.util.Map_Fields.k, java.util.Map_Fields.v, p);
					++size;
				}
				else
				{
					ConcurrentMap_Fields.break;
				}
			}
			if (size == 0L)
			{
				SizeCtl = 0;
			}
			else
			{
				int n;
				if (size >= (long)((int)((uint)MAXIMUM_CAPACITY >> 1)))
				{
					n = MAXIMUM_CAPACITY;
				}
				else
				{
					int sz = (int)size;
					n = TableSizeFor(sz + ((int)((uint)sz >> 1)) + 1);
				}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Node<K,V>[] tab = (Node<K,V>[])new Node<?,?>[n];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Node<K, V>[] tab = (Node<K, V>[])new Node<?, ?>[n];
				int mask = n - 1;
				long added = 0L;
				while (p != java.util.Map_Fields.Null)
				{
					bool insertAtFront;
					Node<K, V> next = p.Next, first ;
					int h = p.Hash, j = h & mask;
					if ((first = TabAt(tab, j)) == java.util.Map_Fields.Null)
					{
						insertAtFront = java.util.Map_Fields.True;
					}
					else
					{
						K java.util.Map_Fields.k = p.Key_Renamed;
						if (first.Hash < 0)
						{
							TreeBin<K, V> t = (TreeBin<K, V>)first;
							if (t.PutTreeVal(h, java.util.Map_Fields.k, p.Val) == java.util.Map_Fields.Null)
							{
								++added;
							}
							insertAtFront = java.util.Map_Fields.False;
						}
						else
						{
							int binCount = 0;
							insertAtFront = java.util.Map_Fields.True;
							Node<K, V> q;
							K qk;
							for (q = first; q != java.util.Map_Fields.Null; q = q.Next)
							{
								if (q.Hash == h && ((qk = q.Key_Renamed) == java.util.Map_Fields.k || (qk != java.util.Map_Fields.Null && java.util.Map_Fields.k.Equals(qk))))
								{
									insertAtFront = java.util.Map_Fields.False;
									ConcurrentMap_Fields.break;
								}
								++binCount;
							}
							if (insertAtFront && binCount >= TREEIFY_THRESHOLD)
							{
								insertAtFront = java.util.Map_Fields.False;
								++added;
								p.Next = first;
								TreeNode<K, V> hd = java.util.Map_Fields.Null, tl = java.util.Map_Fields.Null;
								for (q = p; q != java.util.Map_Fields.Null; q = q.Next)
								{
									TreeNode<K, V> t = new TreeNode<K, V> (q.Hash, q.Key_Renamed, q.Val, java.util.Map_Fields.Null, java.util.Map_Fields.Null);
									if ((t.Prev = tl) == java.util.Map_Fields.Null)
									{
										hd = t;
									}
									else
									{
										tl.Next = t;
									}
									tl = t;
								}
								SetTabAt(tab, j, new TreeBin<K, V>(hd));
							}
						}
					}
					if (insertAtFront)
					{
						++added;
						p.Next = first;
						SetTabAt(tab, j, p);
					}
					p = next;
				}
				Table = tab;
				SizeCtl = n - ((int)((uint)n >> 2));
				BaseCount = added;
			}
		}

		// ConcurrentMap methods

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <returns> the previous value associated with the specified key,
		///         or {@code null} if there was no mapping for the key </returns>
		/// <exception cref="NullPointerException"> if the specified key or value is null </exception>
		public virtual V PutIfAbsent(K key, V ConcurrentMap_Fields)
		{
			return PutVal(key, ConcurrentMap_Fields.Value, java.util.Map_Fields.True);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual bool Remove(Object key, Object ConcurrentMap_Fields)
		{
			if (key == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return ConcurrentMap_Fields.Value != java.util.Map_Fields.Null && ReplaceNode(key, java.util.Map_Fields.Null, ConcurrentMap_Fields.Value) != java.util.Map_Fields.Null;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="NullPointerException"> if any of the arguments are null </exception>
		public virtual bool Replace(K key, V java, V java)
		{
			if (key == java.util.Map_Fields.Null || java.util.Map_Fields.OldValue == java.util.Map_Fields.Null || java.util.Map_Fields.NewValue == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return ReplaceNode(key, java.util.Map_Fields.NewValue, java.util.Map_Fields.OldValue) != java.util.Map_Fields.Null;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <returns> the previous value associated with the specified key,
		///         or {@code null} if there was no mapping for the key </returns>
		/// <exception cref="NullPointerException"> if the specified key or value is null </exception>
		public virtual V Replace(K key, V ConcurrentMap_Fields)
		{
			if (key == java.util.Map_Fields.Null || ConcurrentMap_Fields.Value == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return ReplaceNode(key, ConcurrentMap_Fields.Value, java.util.Map_Fields.Null);
		}

		// Overrides of JDK8+ Map extension method defaults

		/// <summary>
		/// Returns the value to which the specified key is mapped, or the
		/// given default value if this map contains no mapping for the
		/// key.
		/// </summary>
		/// <param name="key"> the key whose associated value is to be returned </param>
		/// <param name="defaultValue"> the value to return if this map contains
		/// no mapping for the given key </param>
		/// <returns> the mapping for the key, if present; else the default value </returns>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual V GetOrDefault(Object key, V defaultValue)
		{
			V java.util.Map_Fields.v;
			return (java.util.Map_Fields.v = Get(key)) == java.util.Map_Fields.Null ? defaultValue : java.util.Map_Fields.v;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
		public virtual void forEach<T1>(BiConsumer<T1> action)
		{
			if (action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			Node<K, V>[] t;
			if ((t = Table) != java.util.Map_Fields.Null)
			{
				Traverser<K, V> it = new Traverser<K, V>(t, t.Length, 0, t.Length);
				for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
				{
					action.Accept(p.key, p.val);
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
			Node<K, V>[] t;
			if ((t = Table) != java.util.Map_Fields.Null)
			{
				Traverser<K, V> it = new Traverser<K, V>(t, t.Length, 0, t.Length);
				for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
				{
					V java.util.Map_Fields.OldValue = p.val;
					for (K key = p.key;;)
					{
						V java.util.Map_Fields.NewValue = function.Apply(key, java.util.Map_Fields.OldValue);
						if (java.util.Map_Fields.NewValue == java.util.Map_Fields.Null)
						{
							throw new NullPointerException();
						}
						if (ReplaceNode(key, java.util.Map_Fields.NewValue, java.util.Map_Fields.OldValue) != java.util.Map_Fields.Null || (java.util.Map_Fields.OldValue = Get(key)) == java.util.Map_Fields.Null)
						{
							ConcurrentMap_Fields.break;
						}
					}
				}
			}
		}

		/// <summary>
		/// If the specified key is not already associated with a value,
		/// attempts to compute its value using the given mapping function
		/// and enters it into this map unless {@code null}.  The entire
		/// method invocation is performed atomically, so the function is
		/// applied at most once per key.  Some attempted update operations
		/// on this map by other threads may be blocked while computation
		/// is in progress, so the computation should be short and simple,
		/// and must not attempt to update any other mappings of this map.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="mappingFunction"> the function to compute a value </param>
		/// <returns> the current (existing or computed) value associated with
		///         the specified key, or null if the computed value is null </returns>
		/// <exception cref="NullPointerException"> if the specified key or mappingFunction
		///         is null </exception>
		/// <exception cref="IllegalStateException"> if the computation detectably
		///         attempts a recursive update to this map that would
		///         otherwise never complete </exception>
		/// <exception cref="RuntimeException"> or Error if the mappingFunction does so,
		///         in which case the mapping is left unestablished </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
		public virtual V computeIfAbsent<T1>(K key, Function<T1> mappingFunction) where T1 : V
		{
			if (key == java.util.Map_Fields.Null || mappingFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int h = Spread(key.HashCode());
			V val = java.util.Map_Fields.Null;
			int binCount = 0;
			for (Node<K, V>[] tab = Table;;)
			{
				Node<K, V> f;
				int n, i, fh;
				if (tab == java.util.Map_Fields.Null || (n = tab.Length) == 0)
				{
					tab = InitTable();
				}
				else if ((f = TabAt(tab, i = (n - 1) & h)) == java.util.Map_Fields.Null)
				{
					Node<K, V> r = new ReservationNode<K, V>();
					lock (r)
					{
						if (CasTabAt(tab, i, java.util.Map_Fields.Null, r))
						{
							binCount = 1;
							Node<K, V> node = java.util.Map_Fields.Null;
							try
							{
								if ((val = mappingFunction.Apply(key)) != java.util.Map_Fields.Null)
								{
									node = new Node<K, V>(h, key, val, java.util.Map_Fields.Null);
								}
							}
							finally
							{
								SetTabAt(tab, i, node);
							}
						}
					}
					if (binCount != 0)
					{
						ConcurrentMap_Fields.break;
					}
				}
				else if ((fh = f.Hash) == MOVED)
				{
					tab = HelpTransfer(tab, f);
				}
				else
				{
					bool added = java.util.Map_Fields.False;
					lock (f)
					{
						if (TabAt(tab, i) == f)
						{
							if (fh >= 0)
							{
								binCount = 1;
								for (Node<K, V> e = f;; ++binCount)
								{
									K ek;
									V ev;
									if (e.Hash == h && ((ek = e.Key_Renamed) == key || (ek != java.util.Map_Fields.Null && key.Equals(ek))))
									{
										val = e.Val;
										ConcurrentMap_Fields.break;
									}
									Node<K, V> pred = e;
									if ((e = e.next) == java.util.Map_Fields.Null)
									{
										if ((val = mappingFunction.Apply(key)) != java.util.Map_Fields.Null)
										{
											added = java.util.Map_Fields.True;
											pred.Next = new Node<K, V>(h, key, val, java.util.Map_Fields.Null);
										}
										ConcurrentMap_Fields.break;
									}
								}
							}
							else if (f is TreeBin)
							{
								binCount = 2;
								TreeBin<K, V> t = (TreeBin<K, V>)f;
								TreeNode<K, V> r, p;
								if ((r = t.Root) != java.util.Map_Fields.Null && (p = r.FindTreeNode(h, key, java.util.Map_Fields.Null)) != java.util.Map_Fields.Null)
								{
									val = p.Val;
								}
								else if ((val = mappingFunction.Apply(key)) != java.util.Map_Fields.Null)
								{
									added = java.util.Map_Fields.True;
									t.PutTreeVal(h, key, val);
								}
							}
						}
					}
					if (binCount != 0)
					{
						if (binCount >= TREEIFY_THRESHOLD)
						{
							TreeifyBin(tab, i);
						}
						if (!added)
						{
							return val;
						}
						ConcurrentMap_Fields.break;
					}
				}
			}
			if (val != java.util.Map_Fields.Null)
			{
				AddCount(1L, binCount);
			}
			return val;
		}

		/// <summary>
		/// If the value for the specified key is present, attempts to
		/// compute a new mapping given the key and its current mapped
		/// value.  The entire method invocation is performed atomically.
		/// Some attempted update operations on this map by other threads
		/// may be blocked while computation is in progress, so the
		/// computation should be short and simple, and must not attempt to
		/// update any other mappings of this map.
		/// </summary>
		/// <param name="key"> key with which a value may be associated </param>
		/// <param name="remappingFunction"> the function to compute a value </param>
		/// <returns> the new value associated with the specified key, or null if none </returns>
		/// <exception cref="NullPointerException"> if the specified key or remappingFunction
		///         is null </exception>
		/// <exception cref="IllegalStateException"> if the computation detectably
		///         attempts a recursive update to this map that would
		///         otherwise never complete </exception>
		/// <exception cref="RuntimeException"> or Error if the remappingFunction does so,
		///         in which case the mapping is unchanged </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
		public virtual V computeIfPresent<T1>(K key, BiFunction<T1> remappingFunction) where T1 : V
		{
			if (key == java.util.Map_Fields.Null || remappingFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int h = Spread(key.HashCode());
			V val = java.util.Map_Fields.Null;
			int delta = 0;
			int binCount = 0;
			for (Node<K, V>[] tab = Table;;)
			{
				Node<K, V> f;
				int n, i, fh;
				if (tab == java.util.Map_Fields.Null || (n = tab.Length) == 0)
				{
					tab = InitTable();
				}
				else if ((f = TabAt(tab, i = (n - 1) & h)) == java.util.Map_Fields.Null)
				{
					ConcurrentMap_Fields.break;
				}
				else if ((fh = f.Hash) == MOVED)
				{
					tab = HelpTransfer(tab, f);
				}
				else
				{
					lock (f)
					{
						if (TabAt(tab, i) == f)
						{
							if (fh >= 0)
							{
								binCount = 1;
								for (Node<K, V> e = f, pred = java.util.Map_Fields.Null;; ++binCount)
								{
									K ek;
									if (e.Hash == h && ((ek = e.Key_Renamed) == key || (ek != java.util.Map_Fields.Null && key.Equals(ek))))
									{
										val = remappingFunction.Apply(key, e.Val);
										if (val != java.util.Map_Fields.Null)
										{
											e.Val = val;
										}
										else
										{
											delta = -1;
											Node<K, V> en = e.Next;
											if (pred != java.util.Map_Fields.Null)
											{
												pred.next = en;
											}
											else
											{
												SetTabAt(tab, i, en);
											}
										}
										ConcurrentMap_Fields.break;
									}
									pred = e;
									if ((e = e.next) == java.util.Map_Fields.Null)
									{
										ConcurrentMap_Fields.break;
									}
								}
							}
							else if (f is TreeBin)
							{
								binCount = 2;
								TreeBin<K, V> t = (TreeBin<K, V>)f;
								TreeNode<K, V> r, p;
								if ((r = t.Root) != java.util.Map_Fields.Null && (p = r.FindTreeNode(h, key, java.util.Map_Fields.Null)) != java.util.Map_Fields.Null)
								{
									val = remappingFunction.Apply(key, p.Val);
									if (val != java.util.Map_Fields.Null)
									{
										p.Val = val;
									}
									else
									{
										delta = -1;
										if (t.RemoveTreeNode(p))
										{
											SetTabAt(tab, i, Untreeify(t.First));
										}
									}
								}
							}
						}
					}
					if (binCount != 0)
					{
						ConcurrentMap_Fields.break;
					}
				}
			}
			if (delta != 0)
			{
				AddCount((long)delta, binCount);
			}
			return val;
		}

		/// <summary>
		/// Attempts to compute a mapping for the specified key and its
		/// current mapped value (or {@code null} if there is no current
		/// mapping). The entire method invocation is performed atomically.
		/// Some attempted update operations on this map by other threads
		/// may be blocked while computation is in progress, so the
		/// computation should be short and simple, and must not attempt to
		/// update any other mappings of this Map.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="remappingFunction"> the function to compute a value </param>
		/// <returns> the new value associated with the specified key, or null if none </returns>
		/// <exception cref="NullPointerException"> if the specified key or remappingFunction
		///         is null </exception>
		/// <exception cref="IllegalStateException"> if the computation detectably
		///         attempts a recursive update to this map that would
		///         otherwise never complete </exception>
		/// <exception cref="RuntimeException"> or Error if the remappingFunction does so,
		///         in which case the mapping is unchanged </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
		public virtual V compute<T1>(K key, BiFunction<T1> remappingFunction) where T1 : V
		{
			if (key == java.util.Map_Fields.Null || remappingFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int h = Spread(key.HashCode());
			V val = java.util.Map_Fields.Null;
			int delta = 0;
			int binCount = 0;
			for (Node<K, V>[] tab = Table;;)
			{
				Node<K, V> f;
				int n, i, fh;
				if (tab == java.util.Map_Fields.Null || (n = tab.Length) == 0)
				{
					tab = InitTable();
				}
				else if ((f = TabAt(tab, i = (n - 1) & h)) == java.util.Map_Fields.Null)
				{
					Node<K, V> r = new ReservationNode<K, V>();
					lock (r)
					{
						if (CasTabAt(tab, i, java.util.Map_Fields.Null, r))
						{
							binCount = 1;
							Node<K, V> node = java.util.Map_Fields.Null;
							try
							{
								if ((val = remappingFunction.Apply(key, java.util.Map_Fields.Null)) != java.util.Map_Fields.Null)
								{
									delta = 1;
									node = new Node<K, V>(h, key, val, java.util.Map_Fields.Null);
								}
							}
							finally
							{
								SetTabAt(tab, i, node);
							}
						}
					}
					if (binCount != 0)
					{
						ConcurrentMap_Fields.break;
					}
				}
				else if ((fh = f.Hash) == MOVED)
				{
					tab = HelpTransfer(tab, f);
				}
				else
				{
					lock (f)
					{
						if (TabAt(tab, i) == f)
						{
							if (fh >= 0)
							{
								binCount = 1;
								for (Node<K, V> e = f, pred = java.util.Map_Fields.Null;; ++binCount)
								{
									K ek;
									if (e.Hash == h && ((ek = e.Key_Renamed) == key || (ek != java.util.Map_Fields.Null && key.Equals(ek))))
									{
										val = remappingFunction.Apply(key, e.Val);
										if (val != java.util.Map_Fields.Null)
										{
											e.Val = val;
										}
										else
										{
											delta = -1;
											Node<K, V> en = e.Next;
											if (pred != java.util.Map_Fields.Null)
											{
												pred.next = en;
											}
											else
											{
												SetTabAt(tab, i, en);
											}
										}
										ConcurrentMap_Fields.break;
									}
									pred = e;
									if ((e = e.next) == java.util.Map_Fields.Null)
									{
										val = remappingFunction.Apply(key, java.util.Map_Fields.Null);
										if (val != java.util.Map_Fields.Null)
										{
											delta = 1;
											pred.next = new Node<K, V>(h, key, val, java.util.Map_Fields.Null);
										}
										ConcurrentMap_Fields.break;
									}
								}
							}
							else if (f is TreeBin)
							{
								binCount = 1;
								TreeBin<K, V> t = (TreeBin<K, V>)f;
								TreeNode<K, V> r, p;
								if ((r = t.Root) != java.util.Map_Fields.Null)
								{
									p = r.FindTreeNode(h, key, java.util.Map_Fields.Null);
								}
								else
								{
									p = java.util.Map_Fields.Null;
								}
								V pv = (p == java.util.Map_Fields.Null) ? java.util.Map_Fields.Null : p.Val;
								val = remappingFunction.Apply(key, pv);
								if (val != java.util.Map_Fields.Null)
								{
									if (p != java.util.Map_Fields.Null)
									{
										p.Val = val;
									}
									else
									{
										delta = 1;
										t.PutTreeVal(h, key, val);
									}
								}
								else if (p != java.util.Map_Fields.Null)
								{
									delta = -1;
									if (t.RemoveTreeNode(p))
									{
										SetTabAt(tab, i, Untreeify(t.First));
									}
								}
							}
						}
					}
					if (binCount != 0)
					{
						if (binCount >= TREEIFY_THRESHOLD)
						{
							TreeifyBin(tab, i);
						}
						ConcurrentMap_Fields.break;
					}
				}
			}
			if (delta != 0)
			{
				AddCount((long)delta, binCount);
			}
			return val;
		}

		/// <summary>
		/// If the specified key is not already associated with a
		/// (non-null) value, associates it with the given value.
		/// Otherwise, replaces the value with the results of the given
		/// remapping function, or removes if {@code null}. The entire
		/// method invocation is performed atomically.  Some attempted
		/// update operations on this map by other threads may be blocked
		/// while computation is in progress, so the computation should be
		/// short and simple, and must not attempt to update any other
		/// mappings of this Map.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="value"> the value to use if absent </param>
		/// <param name="remappingFunction"> the function to recompute a value if present </param>
		/// <returns> the new value associated with the specified key, or null if none </returns>
		/// <exception cref="NullPointerException"> if the specified key or the
		///         remappingFunction is null </exception>
		/// <exception cref="RuntimeException"> or Error if the remappingFunction does so,
		///         in which case the mapping is unchanged </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V merge(K key, V ConcurrentMap_Fields.value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
		public virtual V merge<T1>(K key, V ConcurrentMap_Fields, BiFunction<T1> remappingFunction) where T1 : V
		{
			if (key == java.util.Map_Fields.Null || ConcurrentMap_Fields.Value == java.util.Map_Fields.Null || remappingFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			int h = Spread(key.HashCode());
			V val = java.util.Map_Fields.Null;
			int delta = 0;
			int binCount = 0;
			for (Node<K, V>[] tab = Table;;)
			{
				Node<K, V> f;
				int n, i, fh;
				if (tab == java.util.Map_Fields.Null || (n = tab.Length) == 0)
				{
					tab = InitTable();
				}
				else if ((f = TabAt(tab, i = (n - 1) & h)) == java.util.Map_Fields.Null)
				{
					if (CasTabAt(tab, i, java.util.Map_Fields.Null, new Node<K, V>(h, key, ConcurrentMap_Fields.Value, java.util.Map_Fields.Null)))
					{
						delta = 1;
						val = ConcurrentMap_Fields.Value;
						ConcurrentMap_Fields.break;
					}
				}
				else if ((fh = f.Hash) == MOVED)
				{
					tab = HelpTransfer(tab, f);
				}
				else
				{
					lock (f)
					{
						if (TabAt(tab, i) == f)
						{
							if (fh >= 0)
							{
								binCount = 1;
								for (Node<K, V> e = f, pred = java.util.Map_Fields.Null;; ++binCount)
								{
									K ek;
									if (e.Hash == h && ((ek = e.Key_Renamed) == key || (ek != java.util.Map_Fields.Null && key.Equals(ek))))
									{
										val = remappingFunction.Apply(e.Val, ConcurrentMap_Fields.Value);
										if (val != java.util.Map_Fields.Null)
										{
											e.Val = val;
										}
										else
										{
											delta = -1;
											Node<K, V> en = e.Next;
											if (pred != java.util.Map_Fields.Null)
											{
												pred.next = en;
											}
											else
											{
												SetTabAt(tab, i, en);
											}
										}
										ConcurrentMap_Fields.break;
									}
									pred = e;
									if ((e = e.next) == java.util.Map_Fields.Null)
									{
										delta = 1;
										val = ConcurrentMap_Fields.Value;
										pred.next = new Node<K, V>(h, key, val, java.util.Map_Fields.Null);
										ConcurrentMap_Fields.break;
									}
								}
							}
							else if (f is TreeBin)
							{
								binCount = 2;
								TreeBin<K, V> t = (TreeBin<K, V>)f;
								TreeNode<K, V> r = t.Root;
								TreeNode<K, V> p = (r == java.util.Map_Fields.Null) ? java.util.Map_Fields.Null : r.FindTreeNode(h, key, java.util.Map_Fields.Null);
								val = (p == java.util.Map_Fields.Null) ? ConcurrentMap_Fields.Value : remappingFunction.Apply(p.Val, ConcurrentMap_Fields.Value);
								if (val != java.util.Map_Fields.Null)
								{
									if (p != java.util.Map_Fields.Null)
									{
										p.Val = val;
									}
									else
									{
										delta = 1;
										t.PutTreeVal(h, key, val);
									}
								}
								else if (p != java.util.Map_Fields.Null)
								{
									delta = -1;
									if (t.RemoveTreeNode(p))
									{
										SetTabAt(tab, i, Untreeify(t.First));
									}
								}
							}
						}
					}
					if (binCount != 0)
					{
						if (binCount >= TREEIFY_THRESHOLD)
						{
							TreeifyBin(tab, i);
						}
						ConcurrentMap_Fields.break;
					}
				}
			}
			if (delta != 0)
			{
				AddCount((long)delta, binCount);
			}
			return val;
		}

		// Hashtable legacy methods

		/// <summary>
		/// Legacy method testing if some key maps into the specified value
		/// in this table.  This method is identical in functionality to
		/// <seealso cref="#containsValue(Object)"/>, and exists solely to ensure
		/// full compatibility with class <seealso cref="java.util.Hashtable"/>,
		/// which supported this method prior to introduction of the
		/// Java Collections framework.
		/// </summary>
		/// <param name="value"> a value to search for </param>
		/// <returns> {@code true} if and only if some key maps to the
		///         {@code value} argument in this table as
		///         determined by the {@code equals} method;
		///         {@code false} otherwise </returns>
		/// <exception cref="NullPointerException"> if the specified value is null </exception>
		public virtual bool Contains(Object ConcurrentMap_Fields)
		{
			return ContainsValue(ConcurrentMap_Fields.Value);
		}

		/// <summary>
		/// Returns an enumeration of the keys in this table.
		/// </summary>
		/// <returns> an enumeration of the keys in this table </returns>
		/// <seealso cref= #keySet() </seealso>
		public virtual IEnumerator<K> Keys()
		{
			Node<K, V>[] t;
			int f = (t = Table) == java.util.Map_Fields.Null ? 0 : t.Length;
			return new KeyIterator<K, V>(t, f, 0, f, this);
		}

		/// <summary>
		/// Returns an enumeration of the values in this table.
		/// </summary>
		/// <returns> an enumeration of the values in this table </returns>
		/// <seealso cref= #values() </seealso>
		public virtual IEnumerator<V> Elements()
		{
			Node<K, V>[] t;
			int f = (t = Table) == java.util.Map_Fields.Null ? 0 : t.Length;
			return new ValueIterator<K, V>(t, f, 0, f, this);
		}

		// ConcurrentHashMap-only methods

		/// <summary>
		/// Returns the number of mappings. This method should be used
		/// instead of <seealso cref="#size"/> because a ConcurrentHashMap may
		/// contain more mappings than can be represented as an int. The
		/// value returned is an estimate; the actual count may differ if
		/// there are concurrent insertions or removals.
		/// </summary>
		/// <returns> the number of mappings
		/// @since 1.8 </returns>
		public virtual long MappingCount()
		{
			long n = SumCount();
			return (n < 0L) ? 0L : n; // ignore transient negative values
		}

		/// <summary>
		/// Creates a new <seealso cref="Set"/> backed by a ConcurrentHashMap
		/// from the given type to {@code Boolean.TRUE}.
		/// </summary>
		/// @param <K> the element type of the returned set </param>
		/// <returns> the new set
		/// @since 1.8 </returns>
		public static KeySetView<K, Boolean> newKeySet<K>()
		{
			return new KeySetView<K, Boolean> (new ConcurrentHashMap<K, Boolean>(), true);
		}

		/// <summary>
		/// Creates a new <seealso cref="Set"/> backed by a ConcurrentHashMap
		/// from the given type to {@code Boolean.TRUE}.
		/// </summary>
		/// <param name="initialCapacity"> The implementation performs internal
		/// sizing to accommodate this many elements. </param>
		/// @param <K> the element type of the returned set </param>
		/// <returns> the new set </returns>
		/// <exception cref="IllegalArgumentException"> if the initial capacity of
		/// elements is negative
		/// @since 1.8 </exception>
		public static KeySetView<K, Boolean> newKeySet<K>(int initialCapacity)
		{
			return new KeySetView<K, Boolean> (new ConcurrentHashMap<K, Boolean>(initialCapacity), true);
		}

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the keys in this map, using the
		/// given common mapped value for any additions (i.e., {@link
		/// Collection#add} and <seealso cref="Collection#addAll(Collection)"/>).
		/// This is of course only appropriate if it is acceptable to use
		/// the same value for all additions from this view.
		/// </summary>
		/// <param name="mappedValue"> the mapped value to use for any additions </param>
		/// <returns> the set view </returns>
		/// <exception cref="NullPointerException"> if the mappedValue is null </exception>
		public virtual KeySetView<K, V> KeySet(V mappedValue)
		{
			if (mappedValue == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return new KeySetView<K, V>(this, mappedValue);
		}

		/* ---------------- Special Nodes -------------- */

		/// <summary>
		/// A node inserted at head of bins during transfer operations.
		/// </summary>
		internal sealed class ForwardingNode<K, V> : Node<K, V>
		{
			internal readonly Node<K, V>[] NextTable;
			internal ForwardingNode(Node<K, V>[] tab) : base(MOVED, java.util.Map_Fields.Null, java.util.Map_Fields.Null, java.util.Map_Fields.Null)
			{
				this.NextTable = tab;
			}

			internal Node<K, V> Find(int h, Object java)
			{
				// loop to avoid arbitrarily deep recursion on forwarding nodes
				for (Node<K, V>[] tab = NextTable;;)
				{
					Node<K, V> e;
					int n;
					if (java.util.Map_Fields.k == java.util.Map_Fields.Null || tab == java.util.Map_Fields.Null || (n = tab.Length) == 0 || (e = TabAt(tab, (n - 1) & h)) == java.util.Map_Fields.Null)
					{
						return java.util.Map_Fields.Null;
					}
					for (;;)
					{
						int eh;
						K ek;
						if ((eh = e.Hash) == h && ((ek = e.Key_Renamed) == java.util.Map_Fields.k || (ek != java.util.Map_Fields.Null && java.util.Map_Fields.k.Equals(ek))))
						{
							return e;
						}
						if (eh < 0)
						{
							if (e is ForwardingNode)
							{
								tab = ((ForwardingNode<K, V>)e).NextTable;
								ConcurrentMap_Fields.continue outer;
							}
							else
							{
								return e.Find(h, java.util.Map_Fields.k);
							}
						}
						if ((e = e.Next) == java.util.Map_Fields.Null)
						{
							return java.util.Map_Fields.Null;
						}
					}
					outerContinue:;
				}
				outerBreak:;
			}
		}

		/// <summary>
		/// A place-holder node used in computeIfAbsent and compute
		/// </summary>
		internal sealed class ReservationNode<K, V> : Node<K, V>
		{
			internal ReservationNode() : base(RESERVED, java.util.Map_Fields.Null, java.util.Map_Fields.Null, java.util.Map_Fields.Null)
			{
			}

			internal Node<K, V> Find(int h, Object java)
			{
				return java.util.Map_Fields.Null;
			}
		}

		/* ---------------- Table Initialization and Resizing -------------- */

		/// <summary>
		/// Returns the stamp bits for resizing a table of size n.
		/// Must be negative when shifted left by RESIZE_STAMP_SHIFT.
		/// </summary>
		internal static int ResizeStamp(int n)
		{
			return Integer.NumberOfLeadingZeros(n) | (1 << (RESIZE_STAMP_BITS - 1));
		}

		/// <summary>
		/// Initializes table, using the size recorded in sizeCtl.
		/// </summary>
		private Node<K, V>[] InitTable()
		{
			Node<K, V>[] tab;
			int sc;
			while ((tab = Table) == java.util.Map_Fields.Null || tab.Length == 0)
			{
				if ((sc = SizeCtl) < 0)
				{
					Thread.@yield(); // lost initialization race; just spin
				}
				else if (U.compareAndSwapInt(this, SIZECTL, sc, -1))
				{
					try
					{
						if ((tab = Table) == java.util.Map_Fields.Null || tab.Length == 0)
						{
							int n = (sc > 0) ? sc : DEFAULT_CAPACITY;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Node<K,V>[] nt = (Node<K,V>[])new Node<?,?>[n];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Node<K, V>[] nt = (Node<K, V>[])new Node<?, ?>[n];
							Table = tab = nt;
							sc = n - ((int)((uint)n >> 2));
						}
					}
					finally
					{
						SizeCtl = sc;
					}
					ConcurrentMap_Fields.break;
				}
			}
			return tab;
		}

		/// <summary>
		/// Adds to count, and if table is too small and not already
		/// resizing, initiates transfer. If already resizing, helps
		/// perform transfer if work is available.  Rechecks occupancy
		/// after a transfer to see if another resize is already needed
		/// because resizings are lagging additions.
		/// </summary>
		/// <param name="x"> the count to add </param>
		/// <param name="check"> if <0, don't check resize, if <= 1 only check if uncontended </param>
		private void AddCount(long x, int check)
		{
			CounterCell[] @as;
			long b, s;
			if ((@as = CounterCells) != java.util.Map_Fields.Null || !U.compareAndSwapLong(this, BASECOUNT, b = BaseCount, s = b + x))
			{
				CounterCell a;
				long java.util.Map_Fields.v;
				int m;
				bool uncontended = java.util.Map_Fields.True;
				if (@as == java.util.Map_Fields.Null || (m = @as.Length - 1) < 0 || (a = @as[ThreadLocalRandom.Probe & m]) == java.util.Map_Fields.Null || !(uncontended = U.compareAndSwapLong(a, CELLVALUE, java.util.Map_Fields.v = a.Value, java.util.Map_Fields.v + x)))
				{
					FullAddCount(x, uncontended);
					return;
				}
				if (check <= 1)
				{
					return;
				}
				s = SumCount();
			}
			if (check >= 0)
			{
				Node<K, V>[] tab, nt;
				int n, sc;
				while (s >= (long)(sc = SizeCtl) && (tab = Table) != java.util.Map_Fields.Null && (n = tab.Length) < MAXIMUM_CAPACITY)
				{
					int rs = ResizeStamp(n);
					if (sc < 0)
					{
						if (((int)((uint)sc >> RESIZE_STAMP_SHIFT)) != rs || sc == rs + 1 || sc == rs + MAX_RESIZERS || (nt = NextTable) == java.util.Map_Fields.Null || TransferIndex <= 0)
						{
							ConcurrentMap_Fields.break;
						}
						if (U.compareAndSwapInt(this, SIZECTL, sc, sc + 1))
						{
							Transfer(tab, nt);
						}
					}
					else if (U.compareAndSwapInt(this, SIZECTL, sc, (rs << RESIZE_STAMP_SHIFT) + 2))
					{
						Transfer(tab, java.util.Map_Fields.Null);
					}
					s = SumCount();
				}
			}
		}

		/// <summary>
		/// Helps transfer if a resize is in progress.
		/// </summary>
		internal Node<K, V>[] HelpTransfer(Node<K, V>[] tab, Node<K, V> f)
		{
			Node<K, V>[] nextTab;
			int sc;
			if (tab != java.util.Map_Fields.Null && (f is ForwardingNode) && (nextTab = ((ForwardingNode<K, V>)f).NextTable) != java.util.Map_Fields.Null)
			{
				int rs = ResizeStamp(tab.Length);
				while (nextTab == NextTable && Table == tab && (sc = SizeCtl) < 0)
				{
					if (((int)((uint)sc >> RESIZE_STAMP_SHIFT)) != rs || sc == rs + 1 || sc == rs + MAX_RESIZERS || TransferIndex <= 0)
					{
						ConcurrentMap_Fields.break;
					}
					if (U.compareAndSwapInt(this, SIZECTL, sc, sc + 1))
					{
						Transfer(tab, nextTab);
						ConcurrentMap_Fields.break;
					}
				}
				return nextTab;
			}
			return Table;
		}

		/// <summary>
		/// Tries to presize table to accommodate the given number of elements.
		/// </summary>
		/// <param name="size"> number of elements (doesn't need to be perfectly accurate) </param>
		private void TryPresize(int size)
		{
			int c = (size >= ((int)((uint)MAXIMUM_CAPACITY >> 1))) ? MAXIMUM_CAPACITY : TableSizeFor(size + ((int)((uint)size >> 1)) + 1);
			int sc;
			while ((sc = SizeCtl) >= 0)
			{
				Node<K, V>[] tab = Table;
				int n;
				if (tab == java.util.Map_Fields.Null || (n = tab.Length) == 0)
				{
					n = (sc > c) ? sc : c;
					if (U.compareAndSwapInt(this, SIZECTL, sc, -1))
					{
						try
						{
							if (Table == tab)
							{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Node<K,V>[] nt = (Node<K,V>[])new Node<?,?>[n];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
								Node<K, V>[] nt = (Node<K, V>[])new Node<?, ?>[n];
								Table = nt;
								sc = n - ((int)((uint)n >> 2));
							}
						}
						finally
						{
							SizeCtl = sc;
						}
					}
				}
				else if (c <= sc || n >= MAXIMUM_CAPACITY)
				{
					ConcurrentMap_Fields.break;
				}
				else if (tab == Table)
				{
					int rs = ResizeStamp(n);
					if (sc < 0)
					{
						Node<K, V>[] nt;
						if (((int)((uint)sc >> RESIZE_STAMP_SHIFT)) != rs || sc == rs + 1 || sc == rs + MAX_RESIZERS || (nt = NextTable) == java.util.Map_Fields.Null || TransferIndex <= 0)
						{
							ConcurrentMap_Fields.break;
						}
						if (U.compareAndSwapInt(this, SIZECTL, sc, sc + 1))
						{
							Transfer(tab, nt);
						}
					}
					else if (U.compareAndSwapInt(this, SIZECTL, sc, (rs << RESIZE_STAMP_SHIFT) + 2))
					{
						Transfer(tab, java.util.Map_Fields.Null);
					}
				}
			}
		}

		/// <summary>
		/// Moves and/or copies the nodes in each bin to new table. See
		/// above for explanation.
		/// </summary>
		private void Transfer(Node<K, V>[] tab, Node<K, V>[] nextTab)
		{
			int n = tab.Length, stride ;
			if ((stride = (NCPU > 1) ? ((int)((uint)n >> 3)) / NCPU : n) < MIN_TRANSFER_STRIDE)
			{
				stride = MIN_TRANSFER_STRIDE; // subdivide range
			}
			if (nextTab == java.util.Map_Fields.Null) // initiating
			{
				try
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Node<K,V>[] nt = (Node<K,V>[])new Node<?,?>[n << 1];
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Node<K, V>[] nt = (Node<K, V>[])new Node<?, ?>[n << 1];
					nextTab = nt;
				} // try to cope with OOME
				catch (Throwable)
				{
					SizeCtl = Integer.MaxValue;
					return;
				}
				NextTable = nextTab;
				TransferIndex = n;
			}
			int nextn = nextTab.Length;
			ForwardingNode<K, V> fwd = new ForwardingNode<K, V>(nextTab);
			bool advance = java.util.Map_Fields.True;
			bool finishing = java.util.Map_Fields.False; // to ensure sweep before committing nextTab
			for (int i = 0, bound = 0;;)
			{
				Node<K, V> f;
				int fh;
				while (advance)
				{
					int nextIndex, nextBound;
					if (--i >= bound || finishing)
					{
						advance = java.util.Map_Fields.False;
					}
					else if ((nextIndex = TransferIndex) <= 0)
					{
						i = -1;
						advance = java.util.Map_Fields.False;
					}
					else if (U.compareAndSwapInt(this, TRANSFERINDEX, nextIndex, nextBound = (nextIndex > stride ? nextIndex - stride : 0)))
					{
						bound = nextBound;
						i = nextIndex - 1;
						advance = java.util.Map_Fields.False;
					}
				}
				if (i < 0 || i >= n || i + n >= nextn)
				{
					int sc;
					if (finishing)
					{
						NextTable = java.util.Map_Fields.Null;
						Table = nextTab;
						SizeCtl = (n << 1) - ((int)((uint)n >> 1));
						return;
					}
					if (U.compareAndSwapInt(this, SIZECTL, sc = SizeCtl, sc - 1))
					{
						if ((sc - 2) != ResizeStamp(n) << RESIZE_STAMP_SHIFT)
						{
							return;
						}
						finishing = advance = java.util.Map_Fields.True;
						i = n; // recheck before commit
					}
				}
				else if ((f = TabAt(tab, i)) == java.util.Map_Fields.Null)
				{
					advance = CasTabAt(tab, i, java.util.Map_Fields.Null, fwd);
				}
				else if ((fh = f.Hash) == MOVED)
				{
					advance = java.util.Map_Fields.True; // already processed
				}
				else
				{
					lock (f)
					{
						if (TabAt(tab, i) == f)
						{
							Node<K, V> ln, hn;
							if (fh >= 0)
							{
								int runBit = fh & n;
								Node<K, V> lastRun = f;
								for (Node<K, V> p = f.Next; p != java.util.Map_Fields.Null; p = p.Next)
								{
									int b = p.Hash & n;
									if (b != runBit)
									{
										runBit = b;
										lastRun = p;
									}
								}
								if (runBit == 0)
								{
									ln = lastRun;
									hn = java.util.Map_Fields.Null;
								}
								else
								{
									hn = lastRun;
									ln = java.util.Map_Fields.Null;
								}
								for (Node<K, V> p = f; p != lastRun; p = p.Next)
								{
									int ph = p.Hash;
									K pk = p.Key_Renamed;
									V pv = p.Val;
									if ((ph & n) == 0)
									{
										ln = new Node<K, V>(ph, pk, pv, ln);
									}
									else
									{
										hn = new Node<K, V>(ph, pk, pv, hn);
									}
								}
								SetTabAt(nextTab, i, ln);
								SetTabAt(nextTab, i + n, hn);
								SetTabAt(tab, i, fwd);
								advance = java.util.Map_Fields.True;
							}
							else if (f is TreeBin)
							{
								TreeBin<K, V> t = (TreeBin<K, V>)f;
								TreeNode<K, V> lo = java.util.Map_Fields.Null, loTail = java.util.Map_Fields.Null;
								TreeNode<K, V> hi = java.util.Map_Fields.Null, hiTail = java.util.Map_Fields.Null;
								int lc = 0, hc = 0;
								for (Node<K, V> e = t.First; e != java.util.Map_Fields.Null; e = e.Next)
								{
									int h = e.Hash;
									TreeNode<K, V> p = new TreeNode<K, V> (h, e.Key_Renamed, e.Val, java.util.Map_Fields.Null, java.util.Map_Fields.Null);
									if ((h & n) == 0)
									{
										if ((p.Prev = loTail) == java.util.Map_Fields.Null)
										{
											lo = p;
										}
										else
										{
											loTail.Next = p;
										}
										loTail = p;
										++lc;
									}
									else
									{
										if ((p.Prev = hiTail) == java.util.Map_Fields.Null)
										{
											hi = p;
										}
										else
										{
											hiTail.Next = p;
										}
										hiTail = p;
										++hc;
									}
								}
								ln = (lc <= UNTREEIFY_THRESHOLD) ? Untreeify(lo) : (hc != 0) ? new TreeBin<K, V>(lo) : t;
								hn = (hc <= UNTREEIFY_THRESHOLD) ? Untreeify(hi) : (lc != 0) ? new TreeBin<K, V>(hi) : t;
								SetTabAt(nextTab, i, ln);
								SetTabAt(nextTab, i + n, hn);
								SetTabAt(tab, i, fwd);
								advance = java.util.Map_Fields.True;
							}
						}
					}
				}
			}
		}

		/* ---------------- Counter support -------------- */

		/// <summary>
		/// A padded cell for distributing counts.  Adapted from LongAdder
		/// and Striped64.  See their internal docs for explanation.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @sun.misc.Contended static final class CounterCell
		internal sealed class CounterCell
		{
			internal volatile long ConcurrentMap_Fields;
			internal CounterCell(long x)
			{
				ConcurrentMap_Fields.Value = x;
			}
		}

		internal long SumCount()
		{
			CounterCell[] @as = CounterCells;
			CounterCell a;
			long sum = BaseCount;
			if (@as != java.util.Map_Fields.Null)
			{
				for (int i = 0; i < @as.Length; ++i)
				{
					if ((a = @as[i]) != java.util.Map_Fields.Null)
					{
						sum += a.Value;
					}
				}
			}
			return sum;
		}

		// See LongAdder version for explanation
		private void FullAddCount(long x, bool wasUncontended)
		{
			int h;
			if ((h = ThreadLocalRandom.Probe) == 0)
			{
				ThreadLocalRandom.LocalInit(); // force initialization
				h = ThreadLocalRandom.Probe;
				wasUncontended = java.util.Map_Fields.True;
			}
			bool collide = java.util.Map_Fields.False; // True if last slot nonempty
			for (;;)
			{
				CounterCell[] @as;
				CounterCell a;
				int n;
				long java.util.Map_Fields.v;
				if ((@as = CounterCells) != java.util.Map_Fields.Null && (n = @as.Length) > 0)
				{
					if ((a = @as[(n - 1) & h]) == java.util.Map_Fields.Null)
					{
						if (CellsBusy == 0) // Try to attach new Cell
						{
							CounterCell r = new CounterCell(x); // Optimistic create
							if (CellsBusy == 0 && U.compareAndSwapInt(this, CELLSBUSY, 0, 1))
							{
								bool created = java.util.Map_Fields.False;
								try // Recheck under lock
								{
									CounterCell[] rs;
									int m, j;
									if ((rs = CounterCells) != java.util.Map_Fields.Null && (m = rs.Length) > 0 && rs[j = (m - 1) & h] == java.util.Map_Fields.Null)
									{
										rs[j] = r;
										created = java.util.Map_Fields.True;
									}
								}
								finally
								{
									CellsBusy = 0;
								}
								if (created)
								{
									ConcurrentMap_Fields.break;
								}
								ConcurrentMap_Fields.continue; // Slot is now non-empty
							}
						}
						collide = java.util.Map_Fields.False;
					}
					else if (!wasUncontended) // CAS already known to fail
					{
						wasUncontended = java.util.Map_Fields.True; // Continue after rehash
					}
					else if (U.compareAndSwapLong(a, CELLVALUE, java.util.Map_Fields.v = a.Value, java.util.Map_Fields.v + x))
					{
						ConcurrentMap_Fields.break;
					}
					else if (CounterCells != @as || n >= NCPU)
					{
						collide = java.util.Map_Fields.False; // At max size or stale
					}
					else if (!collide)
					{
						collide = java.util.Map_Fields.True;
					}
					else if (CellsBusy == 0 && U.compareAndSwapInt(this, CELLSBUSY, 0, 1))
					{
						try
						{
							if (CounterCells == @as) // Expand table unless stale
							{
								CounterCell[] rs = new CounterCell[n << 1];
								for (int i = 0; i < n; ++i)
								{
									rs[i] = @as[i];
								}
								CounterCells = rs;
							}
						}
						finally
						{
							CellsBusy = 0;
						}
						collide = java.util.Map_Fields.False;
						ConcurrentMap_Fields.continue; // Retry with expanded table
					}
					h = ThreadLocalRandom.AdvanceProbe(h);
				}
				else if (CellsBusy == 0 && CounterCells == @as && U.compareAndSwapInt(this, CELLSBUSY, 0, 1))
				{
					bool init = java.util.Map_Fields.False;
					try // Initialize table
					{
						if (CounterCells == @as)
						{
							CounterCell[] rs = new CounterCell[2];
							rs[h & 1] = new CounterCell(x);
							CounterCells = rs;
							init = java.util.Map_Fields.True;
						}
					}
					finally
					{
						CellsBusy = 0;
					}
					if (init)
					{
						ConcurrentMap_Fields.break;
					}
				}
				else if (U.compareAndSwapLong(this, BASECOUNT, java.util.Map_Fields.v = BaseCount, java.util.Map_Fields.v + x))
				{
					ConcurrentMap_Fields.break; // Fall back on using base
				}
			}
		}

		/* ---------------- Conversion from/to TreeBins -------------- */

		/// <summary>
		/// Replaces all linked nodes in bin at given index unless table is
		/// too small, in which case resizes instead.
		/// </summary>
		private void TreeifyBin(Node<K, V>[] tab, int index)
		{
			Node<K, V> b;
			int n, sc;
			if (tab != java.util.Map_Fields.Null)
			{
				if ((n = tab.Length) < MIN_TREEIFY_CAPACITY)
				{
					TryPresize(n << 1);
				}
				else if ((b = TabAt(tab, index)) != java.util.Map_Fields.Null && b.Hash >= 0)
				{
					lock (b)
					{
						if (TabAt(tab, index) == b)
						{
							TreeNode<K, V> hd = java.util.Map_Fields.Null, tl = java.util.Map_Fields.Null;
							for (Node<K, V> e = b; e != java.util.Map_Fields.Null; e = e.Next)
							{
								TreeNode<K, V> p = new TreeNode<K, V>(e.Hash, e.Key_Renamed, e.Val, java.util.Map_Fields.Null, java.util.Map_Fields.Null);
								if ((p.Prev = tl) == java.util.Map_Fields.Null)
								{
									hd = p;
								}
								else
								{
									tl.Next = p;
								}
								tl = p;
							}
							SetTabAt(tab, index, new TreeBin<K, V>(hd));
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns a list on non-TreeNodes replacing those in given list.
		/// </summary>
		internal static Node<K, V> untreeify<K, V>(Node<K, V> b)
		{
			Node<K, V> hd = java.util.Map_Fields.Null, tl = java.util.Map_Fields.Null;
			for (Node<K, V> q = b; q != java.util.Map_Fields.Null; q = q.Next)
			{
				Node<K, V> p = new Node<K, V>(q.Hash, q.Key_Renamed, q.Val, java.util.Map_Fields.Null);
				if (tl == java.util.Map_Fields.Null)
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

		/* ---------------- TreeNodes -------------- */

		/// <summary>
		/// Nodes for use in TreeBins
		/// </summary>
		internal sealed class TreeNode<K, V> : Node<K, V>
		{
			internal TreeNode<K, V> Parent; // red-black tree links
			internal TreeNode<K, V> Left;
			internal TreeNode<K, V> Right;
			internal TreeNode<K, V> Prev; // needed to unlink next upon deletion
			internal bool Red;

			internal TreeNode(int hash, K key, V val, Node<K, V> next, TreeNode<K, V> parent) : base(hash, key, val, next)
			{
				this.Parent = parent;
			}

			internal Node<K, V> Find(int h, Object java)
			{
				return FindTreeNode(h, java.util.Map_Fields.k, java.util.Map_Fields.Null);
			}

			/// <summary>
			/// Returns the TreeNode (or null if not found) for the given key
			/// starting at given root.
			/// </summary>
			internal TreeNode<K, V> FindTreeNode(int h, Object java, Class kc)
			{
				if (java.util.Map_Fields.k != java.util.Map_Fields.Null)
				{
					TreeNode<K, V> p = this;
					do
					{
						int ph, dir;
						K pk;
						TreeNode<K, V> q;
						TreeNode<K, V> pl = p.Left, pr = p.Right;
						if ((ph = p.Hash) > h)
						{
							p = pl;
						}
						else if (ph < h)
						{
							p = pr;
						}
						else if ((pk = p.Key_Renamed) == java.util.Map_Fields.k || (pk != java.util.Map_Fields.Null && java.util.Map_Fields.k.Equals(pk)))
						{
							return p;
						}
						else if (pl == java.util.Map_Fields.Null)
						{
							p = pr;
						}
						else if (pr == java.util.Map_Fields.Null)
						{
							p = pl;
						}
						else if ((kc != java.util.Map_Fields.Null || (kc = ComparableClassFor(java.util.Map_Fields.k)) != java.util.Map_Fields.Null) && (dir = CompareComparables(kc, java.util.Map_Fields.k, pk)) != 0)
						{
							p = (dir < 0) ? pl : pr;
						}
						else if ((q = pr.FindTreeNode(h, java.util.Map_Fields.k, kc)) != java.util.Map_Fields.Null)
						{
							return q;
						}
						else
						{
							p = pl;
						}
					} while (p != java.util.Map_Fields.Null);
				}
				return java.util.Map_Fields.Null;
			}
		}

		/* ---------------- TreeBins -------------- */

		/// <summary>
		/// TreeNodes used at the heads of bins. TreeBins do not hold user
		/// keys or values, but instead point to list of TreeNodes and
		/// their root. They also maintain a parasitic read-write lock
		/// forcing writers (who hold bin lock) to wait for readers (who do
		/// not) to complete before tree restructuring operations.
		/// </summary>
		internal sealed class TreeBin<K, V> : Node<K, V>
		{
			internal TreeNode<K, V> Root;
			internal volatile TreeNode<K, V> First;
			internal volatile Thread Waiter;
			internal volatile int LockState;
			// values for lockState
			internal const int WRITER = 1; // set while holding write lock
			internal const int WAITER = 2; // set when waiting for write lock
			internal const int READER = 4; // increment value for setting read lock

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
				if (a == java.util.Map_Fields.Null || b == java.util.Map_Fields.Null || (d = a.GetType().FullName.CompareTo(b.GetType().FullName)) == 0)
				{
					d = (System.identityHashCode(a) <= System.identityHashCode(b) ? - 1 : 1);
				}
				return d;
			}

			/// <summary>
			/// Creates bin with initial set of nodes headed by b.
			/// </summary>
			internal TreeBin(TreeNode<K, V> b) : base(TREEBIN, java.util.Map_Fields.Null, java.util.Map_Fields.Null, java.util.Map_Fields.Null)
			{
				this.First = b;
				TreeNode<K, V> r = java.util.Map_Fields.Null;
				for (TreeNode<K, V> x = b, Next; x != java.util.Map_Fields.Null; x = Next)
				{
					Next = (TreeNode<K, V>)x.Next;
					x.Left = x.Right = java.util.Map_Fields.Null;
					if (r == java.util.Map_Fields.Null)
					{
						x.Parent = java.util.Map_Fields.Null;
						x.Red = java.util.Map_Fields.False;
						r = x;
					}
					else
					{
						K java.util.Map_Fields.k = x.key;
						int h = x.hash;
						Class kc = java.util.Map_Fields.Null;
						for (TreeNode<K, V> p = r;;)
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
							else if ((kc == java.util.Map_Fields.Null && (kc = ComparableClassFor(java.util.Map_Fields.k)) == java.util.Map_Fields.Null) || (dir = CompareComparables(kc, java.util.Map_Fields.k, pk)) == 0)
							{
								dir = TieBreakOrder(java.util.Map_Fields.k, pk);
							}
								TreeNode<K, V> xp = p;
							if ((p = (dir <= 0) ? p.left : p.right) == java.util.Map_Fields.Null)
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
								r = BalanceInsertion(r, x);
								ConcurrentMap_Fields.break;
							}
						}
					}
				}
				this.Root = r;
				Debug.Assert(CheckInvariants(Root));
			}

			/// <summary>
			/// Acquires write lock for tree restructuring.
			/// </summary>
			internal void LockRoot()
			{
				if (!U.compareAndSwapInt(this, LOCKSTATE, 0, WRITER))
				{
					ContendedLock(); // offload to separate method
				}
			}

			/// <summary>
			/// Releases write lock for tree restructuring.
			/// </summary>
			internal void UnlockRoot()
			{
				LockState = 0;
			}

			/// <summary>
			/// Possibly blocks awaiting root lock.
			/// </summary>
			internal void ContendedLock()
			{
				bool waiting = java.util.Map_Fields.False;
				for (int s;;)
				{
					if (((s = LockState) & ~WAITER) == 0)
					{
						if (U.compareAndSwapInt(this, LOCKSTATE, s, WRITER))
						{
							if (waiting)
							{
								Waiter = java.util.Map_Fields.Null;
							}
							return;
						}
					}
					else if ((s & WAITER) == 0)
					{
						if (U.compareAndSwapInt(this, LOCKSTATE, s, s | WAITER))
						{
							waiting = java.util.Map_Fields.True;
							Waiter = Thread.CurrentThread;
						}
					}
					else if (waiting)
					{
						LockSupport.Park(this);
					}
				}
			}

			/// <summary>
			/// Returns matching node or null if none. Tries to search
			/// using tree comparisons from root, but continues linear
			/// search when lock not available.
			/// </summary>
			internal Node<K, V> Find(int h, Object java)
			{
				if (java.util.Map_Fields.k != java.util.Map_Fields.Null)
				{
					for (Node<K, V> e = First; e != java.util.Map_Fields.Null;)
					{
						int s;
						K ek;
						if (((s = LockState) & (WAITER | WRITER)) != 0)
						{
							if (e.Hash == h && ((ek = e.Key_Renamed) == java.util.Map_Fields.k || (ek != java.util.Map_Fields.Null && java.util.Map_Fields.k.Equals(ek))))
							{
								return e;
							}
							e = e.Next;
						}
						else if (U.compareAndSwapInt(this, LOCKSTATE, s, s + READER))
						{
							TreeNode<K, V> r, p;
							try
							{
								p = ((r = Root) == java.util.Map_Fields.Null ? java.util.Map_Fields.Null : r.FindTreeNode(h, java.util.Map_Fields.k, java.util.Map_Fields.Null));
							}
							finally
							{
								Thread w;
								if (U.getAndAddInt(this, LOCKSTATE, -READER) == (READER | WAITER) && (w = Waiter) != java.util.Map_Fields.Null)
								{
									LockSupport.Unpark(w);
								}
							}
							return p;
						}
					}
				}
				return java.util.Map_Fields.Null;
			}

			/// <summary>
			/// Finds or adds a node. </summary>
			/// <returns> null if added </returns>
			internal TreeNode<K, V> PutTreeVal(int h, K java, V java)
			{
				Class kc = java.util.Map_Fields.Null;
				bool searched = java.util.Map_Fields.False;
				for (TreeNode<K, V> p = Root;;)
				{
					int dir, ph;
					K pk;
					if (p == java.util.Map_Fields.Null)
					{
						First = Root = new TreeNode<K, V>(h, java.util.Map_Fields.k, java.util.Map_Fields.v, java.util.Map_Fields.Null, java.util.Map_Fields.Null);
						ConcurrentMap_Fields.break;
					}
					else if ((ph = p.hash) > h)
					{
						dir = -1;
					}
					else if (ph < h)
					{
						dir = 1;
					}
					else if ((pk = p.key) == java.util.Map_Fields.k || (pk != java.util.Map_Fields.Null && java.util.Map_Fields.k.Equals(pk)))
					{
						return p;
					}
					else if ((kc == java.util.Map_Fields.Null && (kc = ComparableClassFor(java.util.Map_Fields.k)) == java.util.Map_Fields.Null) || (dir = CompareComparables(kc, java.util.Map_Fields.k, pk)) == 0)
					{
						if (!searched)
						{
							TreeNode<K, V> q, ch;
							searched = java.util.Map_Fields.True;
							if (((ch = p.left) != java.util.Map_Fields.Null && (q = ch.FindTreeNode(h, java.util.Map_Fields.k, kc)) != java.util.Map_Fields.Null) || ((ch = p.right) != java.util.Map_Fields.Null && (q = ch.FindTreeNode(h, java.util.Map_Fields.k, kc)) != java.util.Map_Fields.Null))
							{
								return q;
							}
						}
						dir = TieBreakOrder(java.util.Map_Fields.k, pk);
					}

					TreeNode<K, V> xp = p;
					if ((p = (dir <= 0) ? p.left : p.right) == java.util.Map_Fields.Null)
					{
						TreeNode<K, V> x , f = First;
						First = x = new TreeNode<K, V>(h, java.util.Map_Fields.k, java.util.Map_Fields.v, f, xp);
						if (f != java.util.Map_Fields.Null)
						{
							f.Prev = x;
						}
						if (dir <= 0)
						{
							xp.Left = x;
						}
						else
						{
							xp.Right = x;
						}
						if (!xp.Red)
						{
							x.Red = java.util.Map_Fields.True;
						}
						else
						{
							LockRoot();
							try
							{
								Root = BalanceInsertion(Root, x);
							}
							finally
							{
								UnlockRoot();
							}
						}
						ConcurrentMap_Fields.break;
					}
				}
				Debug.Assert(CheckInvariants(Root));
				return java.util.Map_Fields.Null;
			}

			/// <summary>
			/// Removes the given node, that must be present before this
			/// call.  This is messier than typical red-black deletion code
			/// because we cannot swap the contents of an interior node
			/// with a leaf successor that is pinned by "next" pointers
			/// that are accessible independently of lock. So instead we
			/// swap the tree linkages.
			/// </summary>
			/// <returns> true if now too small, so should be untreeified </returns>
			internal bool RemoveTreeNode(TreeNode<K, V> p)
			{
				TreeNode<K, V> next = (TreeNode<K, V>)p.Next;
				TreeNode<K, V> pred = p.Prev; // unlink traversal pointers
				TreeNode<K, V> r, rl;
				if (pred == java.util.Map_Fields.Null)
				{
					First = next;
				}
				else
				{
					pred.Next = next;
				}
				if (next != java.util.Map_Fields.Null)
				{
					next.Prev = pred;
				}
				if (First == java.util.Map_Fields.Null)
				{
					Root = java.util.Map_Fields.Null;
					return java.util.Map_Fields.True;
				}
				if ((r = Root) == java.util.Map_Fields.Null || r.Right == java.util.Map_Fields.Null || (rl = r.Left) == java.util.Map_Fields.Null || rl.Left == java.util.Map_Fields.Null) // too small
				{
					return java.util.Map_Fields.True;
				}
				LockRoot();
				try
				{
					TreeNode<K, V> replacement;
					TreeNode<K, V> pl = p.Left;
					TreeNode<K, V> pr = p.Right;
					if (pl != java.util.Map_Fields.Null && pr != java.util.Map_Fields.Null)
					{
						TreeNode<K, V> s = pr, sl ;
						while ((sl = s.Left) != java.util.Map_Fields.Null) // find successor
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
							if ((p.Parent = sp) != java.util.Map_Fields.Null)
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
							if ((s.Right = pr) != java.util.Map_Fields.Null)
							{
								pr.Parent = s;
							}
						}
						p.Left = java.util.Map_Fields.Null;
						if ((p.Right = sr) != java.util.Map_Fields.Null)
						{
							sr.Parent = p;
						}
						if ((s.Left = pl) != java.util.Map_Fields.Null)
						{
							pl.Parent = s;
						}
						if ((s.Parent = pp) == java.util.Map_Fields.Null)
						{
							r = s;
						}
						else if (p == pp.Left)
						{
							pp.Left = s;
						}
						else
						{
							pp.Right = s;
						}
						if (sr != java.util.Map_Fields.Null)
						{
							replacement = sr;
						}
						else
						{
							replacement = p;
						}
					}
					else if (pl != java.util.Map_Fields.Null)
					{
						replacement = pl;
					}
					else if (pr != java.util.Map_Fields.Null)
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
						if (pp == java.util.Map_Fields.Null)
						{
							r = replacement;
						}
						else if (p == pp.Left)
						{
							pp.Left = replacement;
						}
						else
						{
							pp.Right = replacement;
						}
						p.Left = p.Right = p.Parent = java.util.Map_Fields.Null;
					}

					Root = (p.Red) ? r : BalanceDeletion(r, replacement);

					if (p == replacement) // detach pointers
					{
						TreeNode<K, V> pp;
						if ((pp = p.Parent) != java.util.Map_Fields.Null)
						{
							if (p == pp.Left)
							{
								pp.Left = java.util.Map_Fields.Null;
							}
							else if (p == pp.Right)
							{
								pp.Right = java.util.Map_Fields.Null;
							}
							p.Parent = java.util.Map_Fields.Null;
						}
					}
				}
				finally
				{
					UnlockRoot();
				}
				Debug.Assert(CheckInvariants(Root));
				return java.util.Map_Fields.False;
			}

			/* ------------------------------------------------------------ */
			// Red-black tree methods, all adapted from CLR

			internal static TreeNode<K, V> rotateLeft<K, V>(TreeNode<K, V> root, TreeNode<K, V> p)
			{
				TreeNode<K, V> r, pp, rl;
				if (p != java.util.Map_Fields.Null && (r = p.Right) != java.util.Map_Fields.Null)
				{
					if ((rl = p.Right = r.Left) != java.util.Map_Fields.Null)
					{
						rl.Parent = p;
					}
					if ((pp = r.Parent = p.Parent) == java.util.Map_Fields.Null)
					{
						(root = r).red = java.util.Map_Fields.False;
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
				if (p != java.util.Map_Fields.Null && (l = p.Left) != java.util.Map_Fields.Null)
				{
					if ((lr = p.Left = l.Right) != java.util.Map_Fields.Null)
					{
						lr.Parent = p;
					}
					if ((pp = l.Parent = p.Parent) == java.util.Map_Fields.Null)
					{
						(root = l).red = java.util.Map_Fields.False;
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
				x.Red = java.util.Map_Fields.True;
				for (TreeNode<K, V> xp, xpp, xppl, xppr;;)
				{
					if ((xp = x.Parent) == java.util.Map_Fields.Null)
					{
						x.Red = java.util.Map_Fields.False;
						return x;
					}
					else if (!xp.red || (xpp = xp.parent) == java.util.Map_Fields.Null)
					{
						return root;
					}
					if (xp == (xppl = xpp.left))
					{
						if ((xppr = xpp.right) != java.util.Map_Fields.Null && xppr.red)
						{
							xppr.red = java.util.Map_Fields.False;
							xp.red = java.util.Map_Fields.False;
							xpp.red = java.util.Map_Fields.True;
							x = xpp;
						}
						else
						{
							if (x == xp.right)
							{
								root = RotateLeft(root, x = xp);
								xpp = (xp = x.Parent) == java.util.Map_Fields.Null ? java.util.Map_Fields.Null : xp.parent;
							}
							if (xp != java.util.Map_Fields.Null)
							{
								xp.red = java.util.Map_Fields.False;
								if (xpp != java.util.Map_Fields.Null)
								{
									xpp.red = java.util.Map_Fields.True;
									root = RotateRight(root, xpp);
								}
							}
						}
					}
					else
					{
						if (xppl != java.util.Map_Fields.Null && xppl.red)
						{
							xppl.red = java.util.Map_Fields.False;
							xp.red = java.util.Map_Fields.False;
							xpp.red = java.util.Map_Fields.True;
							x = xpp;
						}
						else
						{
							if (x == xp.left)
							{
								root = RotateRight(root, x = xp);
								xpp = (xp = x.Parent) == java.util.Map_Fields.Null ? java.util.Map_Fields.Null : xp.parent;
							}
							if (xp != java.util.Map_Fields.Null)
							{
								xp.red = java.util.Map_Fields.False;
								if (xpp != java.util.Map_Fields.Null)
								{
									xpp.red = java.util.Map_Fields.True;
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
					if (x == java.util.Map_Fields.Null || x == root)
					{
						return root;
					}
					else if ((xp = x.Parent) == java.util.Map_Fields.Null)
					{
						x.Red = java.util.Map_Fields.False;
						return x;
					}
					else if (x.Red)
					{
						x.Red = java.util.Map_Fields.False;
						return root;
					}
					else if ((xpl = xp.left) == x)
					{
						if ((xpr = xp.right) != java.util.Map_Fields.Null && xpr.red)
						{
							xpr.red = java.util.Map_Fields.False;
							xp.red = java.util.Map_Fields.True;
							root = RotateLeft(root, xp);
							xpr = (xp = x.Parent) == java.util.Map_Fields.Null ? java.util.Map_Fields.Null : xp.right;
						}
						if (xpr == java.util.Map_Fields.Null)
						{
							x = xp;
						}
						else
						{
							TreeNode<K, V> sl = xpr.left, sr = xpr.right;
							if ((sr == java.util.Map_Fields.Null || !sr.Red) && (sl == java.util.Map_Fields.Null || !sl.Red))
							{
								xpr.red = java.util.Map_Fields.True;
								x = xp;
							}
							else
							{
								if (sr == java.util.Map_Fields.Null || !sr.Red)
								{
									if (sl != java.util.Map_Fields.Null)
									{
										sl.Red = java.util.Map_Fields.False;
									}
									xpr.red = java.util.Map_Fields.True;
									root = RotateRight(root, xpr);
									xpr = (xp = x.Parent) == java.util.Map_Fields.Null ? java.util.Map_Fields.Null : xp.right;
								}
								if (xpr != java.util.Map_Fields.Null)
								{
									xpr.red = (xp == java.util.Map_Fields.Null) ? java.util.Map_Fields.False : xp.red;
									if ((sr = xpr.right) != java.util.Map_Fields.Null)
									{
										sr.Red = java.util.Map_Fields.False;
									}
								}
								if (xp != java.util.Map_Fields.Null)
								{
									xp.red = java.util.Map_Fields.False;
									root = RotateLeft(root, xp);
								}
								x = root;
							}
						}
					}
					else // symmetric
					{
						if (xpl != java.util.Map_Fields.Null && xpl.red)
						{
							xpl.red = java.util.Map_Fields.False;
							xp.red = java.util.Map_Fields.True;
							root = RotateRight(root, xp);
							xpl = (xp = x.Parent) == java.util.Map_Fields.Null ? java.util.Map_Fields.Null : xp.left;
						}
						if (xpl == java.util.Map_Fields.Null)
						{
							x = xp;
						}
						else
						{
							TreeNode<K, V> sl = xpl.left, sr = xpl.right;
							if ((sl == java.util.Map_Fields.Null || !sl.Red) && (sr == java.util.Map_Fields.Null || !sr.Red))
							{
								xpl.red = java.util.Map_Fields.True;
								x = xp;
							}
							else
							{
								if (sl == java.util.Map_Fields.Null || !sl.Red)
								{
									if (sr != java.util.Map_Fields.Null)
									{
										sr.Red = java.util.Map_Fields.False;
									}
									xpl.red = java.util.Map_Fields.True;
									root = RotateLeft(root, xpl);
									xpl = (xp = x.Parent) == java.util.Map_Fields.Null ? java.util.Map_Fields.Null : xp.left;
								}
								if (xpl != java.util.Map_Fields.Null)
								{
									xpl.red = (xp == java.util.Map_Fields.Null) ? java.util.Map_Fields.False : xp.red;
									if ((sl = xpl.left) != java.util.Map_Fields.Null)
									{
										sl.Red = java.util.Map_Fields.False;
									}
								}
								if (xp != java.util.Map_Fields.Null)
								{
									xp.red = java.util.Map_Fields.False;
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
				if (tb != java.util.Map_Fields.Null && tb.Next != t)
				{
					return java.util.Map_Fields.False;
				}
				if (tn != java.util.Map_Fields.Null && tn.Prev != t)
				{
					return java.util.Map_Fields.False;
				}
				if (tp != java.util.Map_Fields.Null && t != tp.Left && t != tp.Right)
				{
					return java.util.Map_Fields.False;
				}
				if (tl != java.util.Map_Fields.Null && (tl.Parent != t || tl.Hash > t.Hash))
				{
					return java.util.Map_Fields.False;
				}
				if (tr != java.util.Map_Fields.Null && (tr.Parent != t || tr.Hash < t.Hash))
				{
					return java.util.Map_Fields.False;
				}
				if (t.Red && tl != java.util.Map_Fields.Null && tl.Red && tr != java.util.Map_Fields.Null && tr.Red)
				{
					return java.util.Map_Fields.False;
				}
				if (tl != java.util.Map_Fields.Null && !CheckInvariants(tl))
				{
					return java.util.Map_Fields.False;
				}
				if (tr != java.util.Map_Fields.Null && !CheckInvariants(tr))
				{
					return java.util.Map_Fields.False;
				}
				return java.util.Map_Fields.True;
			}

			internal static readonly sun.misc.Unsafe U;
			internal static readonly long LOCKSTATE;
			static TreeBin()
			{
				try
				{
					U = sun.misc.Unsafe.Unsafe;
					Class java.util.Map_Fields.k = typeof(TreeBin);
					LOCKSTATE = U.objectFieldOffset(java.util.Map_Fields.k.getDeclaredField("lockState"));
				}
				catch (Exception e)
				{
					throw new Error(e);
				}
			}
		}

		/* ----------------Table Traversal -------------- */

		/// <summary>
		/// Records the table, its length, and current traversal index for a
		/// traverser that must process a region of a forwarded table before
		/// proceeding with current table.
		/// </summary>
		internal sealed class TableStack<K, V>
		{
			internal int Length;
			internal int Index;
			internal Node<K, V>[] Tab;
			internal TableStack<K, V> Next;
		}

		/// <summary>
		/// Encapsulates traversal for methods such as containsValue; also
		/// serves as a base class for other iterators and spliterators.
		/// 
		/// Method advance visits once each still-valid node that was
		/// reachable upon iterator construction. It might miss some that
		/// were added to a bin after the bin was visited, which is OK wrt
		/// consistency guarantees. Maintaining this property in the face
		/// of possible ongoing resizes requires a fair amount of
		/// bookkeeping state that is difficult to optimize away amidst
		/// volatile accesses.  Even so, traversal maintains reasonable
		/// throughput.
		/// 
		/// Normally, iteration proceeds bin-by-bin traversing lists.
		/// However, if the table has been resized, then all future steps
		/// must traverse both the bin at the current index as well as at
		/// (index + baseSize); and so on for further resizings. To
		/// paranoically cope with potential sharing by users of iterators
		/// across threads, iteration terminates if a bounds checks fails
		/// for a table read.
		/// </summary>
		internal class Traverser<K, V>
		{
			internal Node<K, V>[] Tab; // current table; updated if resized
			internal Node<K, V> Next; // the next entry to use
			internal TableStack<K, V> Stack, Spare; // to save/restore on ForwardingNodes
			internal int Index; // index of bin to use next
			internal int BaseIndex; // current index of initial table
			internal int BaseLimit; // index bound for initial table
			internal readonly int BaseSize; // initial table size

			internal Traverser(Node<K, V>[] tab, int size, int index, int limit)
			{
				this.Tab = tab;
				this.BaseSize = size;
				this.BaseIndex = this.Index = index;
				this.BaseLimit = limit;
				this.Next = java.util.Map_Fields.Null;
			}

			/// <summary>
			/// Advances if possible, returning next valid node, or null if none.
			/// </summary>
			internal Node<K, V> Advance()
			{
				Node<K, V> e;
				if ((e = Next) != java.util.Map_Fields.Null)
				{
					e = e.Next;
				}
				for (;;)
				{
					Node<K, V>[] t; // must use locals in checks
					int i, n;
					if (e != java.util.Map_Fields.Null)
					{
						return Next = e;
					}
					if (BaseIndex >= BaseLimit || (t = Tab) == java.util.Map_Fields.Null || (n = t.Length) <= (i = Index) || i < 0)
					{
						return Next = java.util.Map_Fields.Null;
					}
					if ((e = TabAt(t, i)) != java.util.Map_Fields.Null && e.Hash < 0)
					{
						if (e is ForwardingNode)
						{
							Tab = ((ForwardingNode<K, V>)e).NextTable;
							e = java.util.Map_Fields.Null;
							PushState(t, i, n);
							ConcurrentMap_Fields.continue;
						}
						else if (e is TreeBin)
						{
							e = ((TreeBin<K, V>)e).First;
						}
						else
						{
							e = java.util.Map_Fields.Null;
						}
					}
					if (Stack != java.util.Map_Fields.Null)
					{
						RecoverState(n);
					}
					else if ((Index = i + BaseSize) >= n)
					{
						Index = ++BaseIndex; // visit upper slots if present
					}
				}
			}

			/// <summary>
			/// Saves traversal state upon encountering a forwarding node.
			/// </summary>
			internal virtual void PushState(Node<K, V>[] t, int i, int n)
			{
				TableStack<K, V> s = Spare; // reuse if possible
				if (s != java.util.Map_Fields.Null)
				{
					Spare = s.Next;
				}
				else
				{
					s = new TableStack<K, V>();
				}
				s.Tab = t;
				s.Length = n;
				s.Index = i;
				s.Next = Stack;
				Stack = s;
			}

			/// <summary>
			/// Possibly pops traversal state.
			/// </summary>
			/// <param name="n"> length of current table </param>
			internal virtual void RecoverState(int n)
			{
				TableStack<K, V> s;
				int len;
				while ((s = Stack) != java.util.Map_Fields.Null && (Index += (len = s.Length)) >= n)
				{
					n = len;
					Index = s.Index;
					Tab = s.Tab;
					s.Tab = java.util.Map_Fields.Null;
					TableStack<K, V> next = s.Next;
					s.Next = Spare; // save for reuse
					Stack = next;
					Spare = s;
				}
				if (s == java.util.Map_Fields.Null && (Index += BaseSize) >= n)
				{
					Index = ++BaseIndex;
				}
			}
		}

		/// <summary>
		/// Base of key, value, and entry Iterators. Adds fields to
		/// Traverser to support iterator.remove.
		/// </summary>
		internal class BaseIterator<K, V> : Traverser<K, V>
		{
			internal readonly ConcurrentHashMap<K, V> Map;
			internal Node<K, V> LastReturned;
			internal BaseIterator(Node<K, V>[] tab, int size, int index, int limit, ConcurrentHashMap<K, V> map) : base(tab, size, index, limit)
			{
				this.Map = map;
				Advance();
			}

			public bool HasNext()
			{
				return Next != java.util.Map_Fields.Null;
			}
			public bool HasMoreElements()
			{
				return Next != java.util.Map_Fields.Null;
			}

			public void Remove()
			{
				Node<K, V> p;
				if ((p = LastReturned) == java.util.Map_Fields.Null)
				{
					throw new IllegalStateException();
				}
				LastReturned = java.util.Map_Fields.Null;
				Map.ReplaceNode(p.Key_Renamed, java.util.Map_Fields.Null, java.util.Map_Fields.Null);
			}
		}

		internal sealed class KeyIterator<K, V> : BaseIterator<K, V>, Iterator<K>, Iterator<K>
		{
			internal KeyIterator(Node<K, V>[] tab, int index, int size, int limit, ConcurrentHashMap<K, V> map) : base(tab, index, size, limit, map)
			{
			}

			public K Next()
			{
				Node<K, V> p;
				if ((p = Next) == java.util.Map_Fields.Null)
				{
					throw new NoSuchElementException();
				}
				K java.util.Map_Fields.k = p.Key_Renamed;
				LastReturned = p;
				Advance();
				return java.util.Map_Fields.k;
			}

			public K NextElement()
			{
				return Next();
			}
		}

		internal sealed class ValueIterator<K, V> : BaseIterator<K, V>, Iterator<V>, Iterator<V>
		{
			internal ValueIterator(Node<K, V>[] tab, int index, int size, int limit, ConcurrentHashMap<K, V> map) : base(tab, index, size, limit, map)
			{
			}

			public V Next()
			{
				Node<K, V> p;
				if ((p = Next) == java.util.Map_Fields.Null)
				{
					throw new NoSuchElementException();
				}
				V java.util.Map_Fields.v = p.Val;
				LastReturned = p;
				Advance();
				return java.util.Map_Fields.v;
			}

			public V NextElement()
			{
				return Next();
			}
		}

		internal sealed class EntryIterator<K, V> : BaseIterator<K, V>, Iterator<java.util.Map_Entry<K, V>>
		{
			internal EntryIterator(Node<K, V>[] tab, int index, int size, int limit, ConcurrentHashMap<K, V> map) : base(tab, index, size, limit, map)
			{
			}

			public java.util.Map_Entry<K, V> Next()
			{
				Node<K, V> p;
				if ((p = Next) == java.util.Map_Fields.Null)
				{
					throw new NoSuchElementException();
				}
				K java.util.Map_Fields.k = p.Key_Renamed;
				V java.util.Map_Fields.v = p.Val;
				LastReturned = p;
				Advance();
				return new MapEntry<K, V>(java.util.Map_Fields.k, java.util.Map_Fields.v, Map);
			}
		}

		/// <summary>
		/// Exported Entry for EntryIterator
		/// </summary>
		internal sealed class MapEntry<K, V> : java.util.Map_Entry<K, V>
		{
			internal readonly K Key_Renamed; // non-null
			internal V Val; // non-null
			internal readonly ConcurrentHashMap<K, V> Map;
			internal MapEntry(K key, V val, ConcurrentHashMap<K, V> map)
			{
				this.Key_Renamed = key;
				this.Val = val;
				this.Map = map;
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
					return Val;
				}
			}
			public override int HashCode()
			{
				return Key_Renamed.HashCode() ^ Val.HashCode();
			}
			public override String ToString()
			{
				return Key_Renamed + "=" + Val;
			}

			public override bool Equals(Object o)
			{
				Object java.util.Map_Fields.k, java.util.Map_Fields.v;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map_Entry<?,?> e;
				java.util.Map_Entry<?, ?> e;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((o instanceof java.util.Map_Entry) && (java.util.Map_Fields.k = (e = (java.util.Map_Entry<?,?>)o).getKey()) != java.util.Map_Fields.null && (java.util.Map_Fields.v = e.getValue()) != java.util.Map_Fields.null && (java.util.Map_Fields.k == key || java.util.Map_Fields.k.equals(key)) && (java.util.Map_Fields.v == val || java.util.Map_Fields.v.equals(val)));
				return ((o is java.util.Map_Entry) && (java.util.Map_Fields.k = (e = (java.util.Map_Entry<?, ?>)o).Key) != java.util.Map_Fields.Null && (java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && (java.util.Map_Fields.k == Key_Renamed || java.util.Map_Fields.k.Equals(Key_Renamed)) && (java.util.Map_Fields.v == Val || java.util.Map_Fields.v.Equals(Val)));
			}

			/// <summary>
			/// Sets our entry's value and writes through to the map. The
			/// value to return is somewhat arbitrary here. Since we do not
			/// necessarily track asynchronous changes, the most recent
			/// "previous" value could be different from what we return (or
			/// could even have been removed, in which case the put will
			/// re-establish). We do not and cannot guarantee more.
			/// </summary>
			public V SetValue(V ConcurrentMap_Fields)
			{
				if (ConcurrentMap_Fields.Value == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				V java.util.Map_Fields.v = Val;
				Val = ConcurrentMap_Fields.Value;
				Map.Put(Key_Renamed, ConcurrentMap_Fields.Value);
				return java.util.Map_Fields.v;
			}
		}

		internal sealed class KeySpliterator<K, V> : Traverser<K, V>, Spliterator<K>
		{
			internal long Est; // size estimate
			internal KeySpliterator(Node<K, V>[] tab, int size, int index, int limit, long est) : base(tab, size, index, limit)
			{
				this.Est = est;
			}

			public Spliterator<K> TrySplit()
			{
				int i, f, h;
				return (h = (int)((uint)((i = BaseIndex) + (f = BaseLimit)) >> 1)) <= i ? java.util.Map_Fields.Null : new KeySpliterator<K, V>(Tab, BaseSize, BaseLimit = h, f, Est = (long)((ulong)Est >> 1));
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base K> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
				{
					action.Accept(p.key);
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
				Node<K, V> p;
				if ((p = Advance()) == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.False;
				}
				action.Accept(p.Key_Renamed);
				return java.util.Map_Fields.True;
			}

			public long EstimateSize()
			{
				return Est;
			}

			public int Characteristics()
			{
				return java.util.Spliterator_Fields.DISTINCT | java.util.Spliterator_Fields.CONCURRENT | java.util.Spliterator_Fields.NONNULL;
			}
		}

		internal sealed class ValueSpliterator<K, V> : Traverser<K, V>, Spliterator<V>
		{
			internal long Est; // size estimate
			internal ValueSpliterator(Node<K, V>[] tab, int size, int index, int limit, long est) : base(tab, size, index, limit)
			{
				this.Est = est;
			}

			public Spliterator<V> TrySplit()
			{
				int i, f, h;
				return (h = (int)((uint)((i = BaseIndex) + (f = BaseLimit)) >> 1)) <= i ? java.util.Map_Fields.Null : new ValueSpliterator<K, V>(Tab, BaseSize, BaseLimit = h, f, Est = (long)((ulong)Est >> 1));
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base V> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
				{
					action.Accept(p.val);
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
				Node<K, V> p;
				if ((p = Advance()) == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.False;
				}
				action.Accept(p.Val);
				return java.util.Map_Fields.True;
			}

			public long EstimateSize()
			{
				return Est;
			}

			public int Characteristics()
			{
				return java.util.Spliterator_Fields.CONCURRENT | java.util.Spliterator_Fields.NONNULL;
			}
		}

		internal sealed class EntrySpliterator<K, V> : Traverser<K, V>, Spliterator<java.util.Map_Entry<K, V>>
		{
			internal readonly ConcurrentHashMap<K, V> Map; // To export MapEntry
			internal long Est; // size estimate
			internal EntrySpliterator(Node<K, V>[] tab, int size, int index, int limit, long est, ConcurrentHashMap<K, V> map) : base(tab, size, index, limit)
			{
				this.Map = map;
				this.Est = est;
			}

			public Spliterator<java.util.Map_Entry<K, V>> TrySplit()
			{
				int i, f, h;
				return (h = (int)((uint)((i = BaseIndex) + (f = BaseLimit)) >> 1)) <= i ? java.util.Map_Fields.Null : new EntrySpliterator<K, V>(Tab, BaseSize, BaseLimit = h, f, Est = (long)((ulong)Est >> 1), Map);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachRemaining(java.util.function.Consumer<? base java.util.Map_Entry<K,V>> action)
			public void forEachRemaining<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
				{
					action.Accept(new MapEntry<K, V>(p.key, p.val, Map));
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
				Node<K, V> p;
				if ((p = Advance()) == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.False;
				}
				action.Accept(new MapEntry<K, V>(p.Key_Renamed, p.Val, Map));
				return java.util.Map_Fields.True;
			}

			public long EstimateSize()
			{
				return Est;
			}

			public int Characteristics()
			{
				return java.util.Spliterator_Fields.DISTINCT | java.util.Spliterator_Fields.CONCURRENT | java.util.Spliterator_Fields.NONNULL;
			}
		}

		// Parallel bulk operations

		/// <summary>
		/// Computes initial batch value for bulk tasks. The returned value
		/// is approximately exp2 of the number of times (minus one) to
		/// split task by two before executing leaf action. This value is
		/// faster to compute and more convenient to use as a guide to
		/// splitting than is the depth, since it is used while dividing by
		/// two anyway.
		/// </summary>
		internal int BatchFor(long b)
		{
			long n;
			if (b == Long.MaxValue || (n = SumCount()) <= 1L || n < b)
			{
				return 0;
			}
			int sp = ForkJoinPool.CommonPoolParallelism << 2; // slack of 4
			return (b <= 0L || (n /= b) >= sp) ? sp : (int)n;
		}

		/// <summary>
		/// Performs the given action for each (key, value).
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="action"> the action
		/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(long parallelismThreshold, java.util.function.BiConsumer<? base K,? base V> action)
		public virtual void forEach<T1>(long parallelismThreshold, BiConsumer<T1> action)
		{
			if (action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			(new ForEachMappingTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, action)).Invoke();
		}

		/// <summary>
		/// Performs the given action for each non-null transformation
		/// of each (key, value).
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element, or null if there is no transformation (in
		/// which case the action is not applied) </param>
		/// <param name="action"> the action </param>
		/// @param <U> the return type of the transformer
		/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> void forEach(long parallelismThreshold, java.util.function.BiFunction<? base K, ? base V, ? extends U> transformer, java.util.function.Consumer<? base U> action)
		public virtual void forEach<U, T1, T2>(long parallelismThreshold, BiFunction<T1> transformer, Consumer<T2> action) where T1 : U
		{
			if (transformer == java.util.Map_Fields.Null || action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			(new ForEachTransformedMappingTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, transformer, action)).Invoke();
		}

		/// <summary>
		/// Returns a non-null result from applying the given search
		/// function on each (key, value), or null if none.  Upon
		/// success, further element processing is suppressed and the
		/// results of any other parallel invocations of the search
		/// function are ignored.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="searchFunction"> a function returning a non-null
		/// result on success, else null </param>
		/// @param <U> the return type of the search function </param>
		/// <returns> a non-null result from applying the given search
		/// function on each (key, value), or null if none
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> U search(long parallelismThreshold, java.util.function.BiFunction<? base K, ? base V, ? extends U> searchFunction)
		public virtual U search<U, T1>(long parallelismThreshold, BiFunction<T1> searchFunction) where T1 : U
		{
			if (searchFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new SearchMappingsTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, searchFunction, new AtomicReference<U>())).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all (key, value) pairs using the given reducer to
		/// combine values, or null if none.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element, or null if there is no transformation (in
		/// which case it is not combined) </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// @param <U> the return type of the transformer </param>
		/// <returns> the result of accumulating the given transformation
		/// of all (key, value) pairs
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> U reduce(long parallelismThreshold, java.util.function.BiFunction<? base K, ? base V, ? extends U> transformer, java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer)
		public virtual U reduce<U, T1, T2>(long parallelismThreshold, BiFunction<T1> transformer, BiFunction<T2> reducer) where T1 : U where T2 : U
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceMappingsTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all (key, value) pairs using the given reducer to
		/// combine values, and the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all (key, value) pairs
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public double reduceToDouble(long parallelismThreshold, java.util.function.ToDoubleBiFunction<? base K, ? base V> transformer, double basis, java.util.function.DoubleBinaryOperator reducer)
		public virtual double reduceToDouble<T1>(long parallelismThreshold, ToDoubleBiFunction<T1> transformer, double basis, DoubleBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceMappingsToDoubleTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all (key, value) pairs using the given reducer to
		/// combine values, and the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all (key, value) pairs
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public long reduceToLong(long parallelismThreshold, java.util.function.ToLongBiFunction<? base K, ? base V> transformer, long basis, java.util.function.LongBinaryOperator reducer)
		public virtual long reduceToLong<T1>(long parallelismThreshold, ToLongBiFunction<T1> transformer, long basis, LongBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceMappingsToLongTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all (key, value) pairs using the given reducer to
		/// combine values, and the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all (key, value) pairs
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int reduceToInt(long parallelismThreshold, java.util.function.ToIntBiFunction<? base K, ? base V> transformer, int basis, java.util.function.IntBinaryOperator reducer)
		public virtual int reduceToInt<T1>(long parallelismThreshold, ToIntBiFunction<T1> transformer, int basis, IntBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceMappingsToIntTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Performs the given action for each key.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="action"> the action
		/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachKey(long parallelismThreshold, java.util.function.Consumer<? base K> action)
		public virtual void forEachKey<T1>(long parallelismThreshold, Consumer<T1> action)
		{
			if (action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			(new ForEachKeyTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, action)).Invoke();
		}

		/// <summary>
		/// Performs the given action for each non-null transformation
		/// of each key.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element, or null if there is no transformation (in
		/// which case the action is not applied) </param>
		/// <param name="action"> the action </param>
		/// @param <U> the return type of the transformer
		/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> void forEachKey(long parallelismThreshold, java.util.function.Function<? base K, ? extends U> transformer, java.util.function.Consumer<? base U> action)
		public virtual void forEachKey<U, T1, T2>(long parallelismThreshold, Function<T1> transformer, Consumer<T2> action) where T1 : U
		{
			if (transformer == java.util.Map_Fields.Null || action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			(new ForEachTransformedKeyTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, transformer, action)).Invoke();
		}

		/// <summary>
		/// Returns a non-null result from applying the given search
		/// function on each key, or null if none. Upon success,
		/// further element processing is suppressed and the results of
		/// any other parallel invocations of the search function are
		/// ignored.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="searchFunction"> a function returning a non-null
		/// result on success, else null </param>
		/// @param <U> the return type of the search function </param>
		/// <returns> a non-null result from applying the given search
		/// function on each key, or null if none
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> U searchKeys(long parallelismThreshold, java.util.function.Function<? base K, ? extends U> searchFunction)
		public virtual U searchKeys<U, T1>(long parallelismThreshold, Function<T1> searchFunction) where T1 : U
		{
			if (searchFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new SearchKeysTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, searchFunction, new AtomicReference<U>())).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating all keys using the given
		/// reducer to combine values, or null if none.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating all keys using the given
		/// reducer to combine values, or null if none
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public K reduceKeys(long parallelismThreshold, java.util.function.BiFunction<? base K, ? base K, ? extends K> reducer)
		public virtual K reduceKeys<T1>(long parallelismThreshold, BiFunction<T1> reducer) where T1 : K
		{
			if (reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new ReduceKeysTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all keys using the given reducer to combine values, or
		/// null if none.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element, or null if there is no transformation (in
		/// which case it is not combined) </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// @param <U> the return type of the transformer </param>
		/// <returns> the result of accumulating the given transformation
		/// of all keys
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> U reduceKeys(long parallelismThreshold, java.util.function.Function<? base K, ? extends U> transformer, java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer)
		public virtual U reduceKeys<U, T1, T2>(long parallelismThreshold, Function<T1> transformer, BiFunction<T2> reducer) where T1 : U where T2 : U
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceKeysTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all keys using the given reducer to combine values, and
		/// the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all keys
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public double reduceKeysToDouble(long parallelismThreshold, java.util.function.ToDoubleFunction<? base K> transformer, double basis, java.util.function.DoubleBinaryOperator reducer)
		public virtual double reduceKeysToDouble<T1>(long parallelismThreshold, ToDoubleFunction<T1> transformer, double basis, DoubleBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceKeysToDoubleTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all keys using the given reducer to combine values, and
		/// the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all keys
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public long reduceKeysToLong(long parallelismThreshold, java.util.function.ToLongFunction<? base K> transformer, long basis, java.util.function.LongBinaryOperator reducer)
		public virtual long reduceKeysToLong<T1>(long parallelismThreshold, ToLongFunction<T1> transformer, long basis, LongBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceKeysToLongTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all keys using the given reducer to combine values, and
		/// the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all keys
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int reduceKeysToInt(long parallelismThreshold, java.util.function.ToIntFunction<? base K> transformer, int basis, java.util.function.IntBinaryOperator reducer)
		public virtual int reduceKeysToInt<T1>(long parallelismThreshold, ToIntFunction<T1> transformer, int basis, IntBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceKeysToIntTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Performs the given action for each value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="action"> the action
		/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachValue(long parallelismThreshold, java.util.function.Consumer<? base V> action)
		public virtual void forEachValue<T1>(long parallelismThreshold, Consumer<T1> action)
		{
			if (action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			(new ForEachValueTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, action)).Invoke();
		}

		/// <summary>
		/// Performs the given action for each non-null transformation
		/// of each value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element, or null if there is no transformation (in
		/// which case the action is not applied) </param>
		/// <param name="action"> the action </param>
		/// @param <U> the return type of the transformer
		/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> void forEachValue(long parallelismThreshold, java.util.function.Function<? base V, ? extends U> transformer, java.util.function.Consumer<? base U> action)
		public virtual void forEachValue<U, T1, T2>(long parallelismThreshold, Function<T1> transformer, Consumer<T2> action) where T1 : U
		{
			if (transformer == java.util.Map_Fields.Null || action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			(new ForEachTransformedValueTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, transformer, action)).Invoke();
		}

		/// <summary>
		/// Returns a non-null result from applying the given search
		/// function on each value, or null if none.  Upon success,
		/// further element processing is suppressed and the results of
		/// any other parallel invocations of the search function are
		/// ignored.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="searchFunction"> a function returning a non-null
		/// result on success, else null </param>
		/// @param <U> the return type of the search function </param>
		/// <returns> a non-null result from applying the given search
		/// function on each value, or null if none
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> U searchValues(long parallelismThreshold, java.util.function.Function<? base V, ? extends U> searchFunction)
		public virtual U searchValues<U, T1>(long parallelismThreshold, Function<T1> searchFunction) where T1 : U
		{
			if (searchFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new SearchValuesTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, searchFunction, new AtomicReference<U>())).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating all values using the
		/// given reducer to combine values, or null if none.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating all values
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public V reduceValues(long parallelismThreshold, java.util.function.BiFunction<? base V, ? base V, ? extends V> reducer)
		public virtual V reduceValues<T1>(long parallelismThreshold, BiFunction<T1> reducer) where T1 : V
		{
			if (reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new ReduceValuesTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all values using the given reducer to combine values, or
		/// null if none.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element, or null if there is no transformation (in
		/// which case it is not combined) </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// @param <U> the return type of the transformer </param>
		/// <returns> the result of accumulating the given transformation
		/// of all values
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> U reduceValues(long parallelismThreshold, java.util.function.Function<? base V, ? extends U> transformer, java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer)
		public virtual U reduceValues<U, T1, T2>(long parallelismThreshold, Function<T1> transformer, BiFunction<T2> reducer) where T1 : U where T2 : U
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceValuesTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all values using the given reducer to combine values,
		/// and the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all values
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public double reduceValuesToDouble(long parallelismThreshold, java.util.function.ToDoubleFunction<? base V> transformer, double basis, java.util.function.DoubleBinaryOperator reducer)
		public virtual double reduceValuesToDouble<T1>(long parallelismThreshold, ToDoubleFunction<T1> transformer, double basis, DoubleBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceValuesToDoubleTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all values using the given reducer to combine values,
		/// and the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all values
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public long reduceValuesToLong(long parallelismThreshold, java.util.function.ToLongFunction<? base V> transformer, long basis, java.util.function.LongBinaryOperator reducer)
		public virtual long reduceValuesToLong<T1>(long parallelismThreshold, ToLongFunction<T1> transformer, long basis, LongBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceValuesToLongTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all values using the given reducer to combine values,
		/// and the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all values
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public int reduceValuesToInt(long parallelismThreshold, java.util.function.ToIntFunction<? base V> transformer, int basis, java.util.function.IntBinaryOperator reducer)
		public virtual int reduceValuesToInt<T1>(long parallelismThreshold, ToIntFunction<T1> transformer, int basis, IntBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceValuesToIntTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Performs the given action for each entry.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="action"> the action
		/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEachEntry(long parallelismThreshold, java.util.function.Consumer<? base java.util.Map_Entry<K,V>> action)
		public virtual void forEachEntry<T1>(long parallelismThreshold, Consumer<T1> action)
		{
			if (action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			(new ForEachEntryTask<K, V>(java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, action)).Invoke();
		}

		/// <summary>
		/// Performs the given action for each non-null transformation
		/// of each entry.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element, or null if there is no transformation (in
		/// which case the action is not applied) </param>
		/// <param name="action"> the action </param>
		/// @param <U> the return type of the transformer
		/// @since 1.8 </param>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> void forEachEntry(long parallelismThreshold, java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> transformer, java.util.function.Consumer<? base U> action)
		public virtual void forEachEntry<U, T1, T2>(long parallelismThreshold, Function<T1> transformer, Consumer<T2> action) where T1 : U
		{
			if (transformer == java.util.Map_Fields.Null || action == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			(new ForEachTransformedEntryTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, transformer, action)).Invoke();
		}

		/// <summary>
		/// Returns a non-null result from applying the given search
		/// function on each entry, or null if none.  Upon success,
		/// further element processing is suppressed and the results of
		/// any other parallel invocations of the search function are
		/// ignored.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="searchFunction"> a function returning a non-null
		/// result on success, else null </param>
		/// @param <U> the return type of the search function </param>
		/// <returns> a non-null result from applying the given search
		/// function on each entry, or null if none
		/// @since 1.8 </returns>
		public virtual U searchEntries<U, T1>(long parallelismThreshold, Function<T1> searchFunction) where T1 : U
		{
			if (searchFunction == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new SearchEntriesTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, searchFunction, new AtomicReference<U>())).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating all entries using the
		/// given reducer to combine values, or null if none.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating all entries
		/// @since 1.8 </returns>
		public virtual java.util.Map_Entry<K, V> ReduceEntries(long parallelismThreshold, BiFunction<T1> reducer) where T1 : java.util.Map_Entry<K,V>
		{
			if (reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new ReduceEntriesTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all entries using the given reducer to combine values,
		/// or null if none.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element, or null if there is no transformation (in
		/// which case it is not combined) </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// @param <U> the return type of the transformer </param>
		/// <returns> the result of accumulating the given transformation
		/// of all entries
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public <U> U reduceEntries(long parallelismThreshold, java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> transformer, java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer)
		public virtual U reduceEntries<U, T1, T2>(long parallelismThreshold, Function<T1> transformer, BiFunction<T2> reducer) where T1 : U where T2 : U
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceEntriesTask<K, V, U> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all entries using the given reducer to combine values,
		/// and the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all entries
		/// @since 1.8 </returns>
		public virtual double ReduceEntriesToDouble(long parallelismThreshold, ToDoubleFunction<java.util.Map_Entry<K, V>> transformer, double basis, DoubleBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceEntriesToDoubleTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all entries using the given reducer to combine values,
		/// and the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all entries
		/// @since 1.8 </returns>
		public virtual long ReduceEntriesToLong(long parallelismThreshold, ToLongFunction<java.util.Map_Entry<K, V>> transformer, long basis, LongBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceEntriesToLongTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}

		/// <summary>
		/// Returns the result of accumulating the given transformation
		/// of all entries using the given reducer to combine values,
		/// and the given basis as an identity value.
		/// </summary>
		/// <param name="parallelismThreshold"> the (estimated) number of elements
		/// needed for this operation to be executed in parallel </param>
		/// <param name="transformer"> a function returning the transformation
		/// for an element </param>
		/// <param name="basis"> the identity (initial default value) for the reduction </param>
		/// <param name="reducer"> a commutative associative combining function </param>
		/// <returns> the result of accumulating the given transformation
		/// of all entries
		/// @since 1.8 </returns>
		public virtual int ReduceEntriesToInt(long parallelismThreshold, ToIntFunction<java.util.Map_Entry<K, V>> transformer, int basis, IntBinaryOperator reducer)
		{
			if (transformer == java.util.Map_Fields.Null || reducer == java.util.Map_Fields.Null)
			{
				throw new NullPointerException();
			}
			return (new MapReduceEntriesToIntTask<K, V> (java.util.Map_Fields.Null, BatchFor(parallelismThreshold), 0, 0, Table, java.util.Map_Fields.Null, transformer, basis, reducer)).Invoke();
		}


		/* ----------------Views -------------- */

		/// <summary>
		/// Base class for views.
		/// </summary>
		[Serializable]
		internal abstract class CollectionView<K, V, E> : Collection<E>
		{
			internal const long SerialVersionUID = 7249069246763182397L;
			internal readonly ConcurrentHashMap<K, V> Map_Renamed;
			internal CollectionView(ConcurrentHashMap<K, V> map)
			{
				this.Map_Renamed = map;
			}

			/// <summary>
			/// Returns the map backing this view.
			/// </summary>
			/// <returns> the map backing this view </returns>
			public virtual ConcurrentHashMap<K, V> Map
			{
				get
				{
					return Map_Renamed;
				}
			}

			/// <summary>
			/// Removes all of the elements from this view, by removing all
			/// the mappings from the map backing this view.
			/// </summary>
			public void Clear()
			{
				Map_Renamed.Clear();
			}
			public int Count
			{
				get
				{
					return Map_Renamed.Size();
				}
			}
			public bool Empty
			{
				get
				{
					return Map_Renamed.Empty;
				}
			}

			// implementations below rely on concrete classes supplying these
			// abstract methods
			/// <summary>
			/// Returns an iterator over the elements in this collection.
			/// 
			/// <para>The returned iterator is
			/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
			/// 
			/// </para>
			/// </summary>
			/// <returns> an iterator over the elements in this collection </returns>
			public abstract IEnumerator<E> Iterator();
			public abstract bool Contains(Object o);
			public abstract bool Remove(Object o);

			internal const String OomeMsg = "Required array size too large";

			public Object[] ToArray()
			{
				long sz = Map_Renamed.MappingCount();
				if (sz > MAX_ARRAY_SIZE)
				{
					throw new OutOfMemoryError(OomeMsg);
				}
				int n = (int)sz;
				Object[] r = new Object[n];
				int i = 0;
				foreach (E e in this)
				{
					if (i == n)
					{
						if (n >= MAX_ARRAY_SIZE)
						{
							throw new OutOfMemoryError(OomeMsg);
						}
						if (n >= MAX_ARRAY_SIZE - ((int)((uint)MAX_ARRAY_SIZE >> 1)) - 1)
						{
							n = MAX_ARRAY_SIZE;
						}
						else
						{
							n += ((int)((uint)n >> 1)) + 1;
						}
						r = Arrays.CopyOf(r, n);
					}
					r[i++] = e;
				}
				return (i == n) ? r : Arrays.CopyOf(r, i);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public final <T> T[] toArray(T[] a)
			public T[] toArray<T>(T[] a)
			{
				long sz = Map_Renamed.MappingCount();
				if (sz > MAX_ARRAY_SIZE)
				{
					throw new OutOfMemoryError(OomeMsg);
				}
				int m = (int)sz;
				T[] r = (a.Length >= m) ? a : (T[])java.lang.reflect.Array.NewInstance(a.GetType().GetElementType(), m);
				int n = r.Length;
				int i = 0;
				foreach (E e in this)
				{
					if (i == n)
					{
						if (n >= MAX_ARRAY_SIZE)
						{
							throw new OutOfMemoryError(OomeMsg);
						}
						if (n >= MAX_ARRAY_SIZE - ((int)((uint)MAX_ARRAY_SIZE >> 1)) - 1)
						{
							n = MAX_ARRAY_SIZE;
						}
						else
						{
							n += ((int)((uint)n >> 1)) + 1;
						}
						r = Arrays.CopyOf(r, n);
					}
					r[i++] = (T)e;
				}
				if (a == r && i < n)
				{
					r[i] = java.util.Map_Fields.Null; // null-terminate
					return r;
				}
				return (i == n) ? r : Arrays.CopyOf(r, i);
			}

			/// <summary>
			/// Returns a string representation of this collection.
			/// The string representation consists of the string representations
			/// of the collection's elements in the order they are returned by
			/// its iterator, enclosed in square brackets ({@code "[]"}).
			/// Adjacent elements are separated by the characters {@code ", "}
			/// (comma and space).  Elements are converted to strings as by
			/// <seealso cref="String#valueOf(Object)"/>.
			/// </summary>
			/// <returns> a string representation of this collection </returns>
			public sealed override String ToString()
			{
				StringBuilder sb = new StringBuilder();
				sb.Append('[');
				IEnumerator<E> it = Iterator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (it.hasNext())
				{
					for (;;)
					{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Object e = it.next();
						sb.Append(e == this ? "(this Collection)" : e);
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						if (!it.hasNext())
						{
							ConcurrentMap_Fields.break;
						}
						sb.Append(',').Append(' ');
					}
				}
				return sb.Append(']').ToString();
			}

			public bool containsAll<T1>(ICollection<T1> c)
			{
				if (c != this)
				{
					foreach (Object e in c)
					{
						if (e == java.util.Map_Fields.Null || !Contains(e))
						{
							return java.util.Map_Fields.False;
						}
					}
				}
				return java.util.Map_Fields.True;
			}

			public bool removeAll<T1>(ICollection<T1> c)
			{
				if (c == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				bool modified = java.util.Map_Fields.False;
				for (IEnumerator<E> it = Iterator(); it.MoveNext();)
				{
					if (c.Contains(it.Current))
					{
						it.remove();
						modified = java.util.Map_Fields.True;
					}
				}
				return modified;
			}

			public bool retainAll<T1>(ICollection<T1> c)
			{
				if (c == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				bool modified = java.util.Map_Fields.False;
				for (IEnumerator<E> it = Iterator(); it.MoveNext();)
				{
					if (!c.Contains(it.Current))
					{
						it.remove();
						modified = java.util.Map_Fields.True;
					}
				}
				return modified;
			}

		}

		/// <summary>
		/// A view of a ConcurrentHashMap as a <seealso cref="Set"/> of keys, in
		/// which additions may optionally be enabled by mapping to a
		/// common value.  This class cannot be directly instantiated.
		/// See <seealso cref="#keySet() keySet()"/>,
		/// <seealso cref="#keySet(Object) keySet(V)"/>,
		/// <seealso cref="#newKeySet() newKeySet()"/>,
		/// <seealso cref="#newKeySet(int) newKeySet(int)"/>.
		/// 
		/// @since 1.8
		/// </summary>
		[Serializable]
		public class KeySetView<K, V> : CollectionView<K, V, K>, Set<K>
		{
			internal const long SerialVersionUID = 7249069246763182397L;
			internal readonly V ConcurrentMap_Fields;
			internal KeySetView(ConcurrentHashMap<K, V> map, V ConcurrentMap_Fields) : base(map) // non-public
			{
				this.Value = ConcurrentMap_Fields.Value;
			}

			/// <summary>
			/// Returns the default mapped value for additions,
			/// or {@code null} if additions are not supported.
			/// </summary>
			/// <returns> the default mapped value for additions, or {@code null}
			/// if not supported </returns>
			public virtual V MappedValue
			{
				get
				{
					return ConcurrentMap_Fields.Value;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			/// <exception cref="NullPointerException"> if the specified key is null </exception>
			public virtual bool Contains(Object o)
			{
				return Map_Renamed.ContainsKey(o);
			}

			/// <summary>
			/// Removes the key from this map view, by removing the key (and its
			/// corresponding value) from the backing map.  This method does
			/// nothing if the key is not in the map.
			/// </summary>
			/// <param name="o"> the key to be removed from the backing map </param>
			/// <returns> {@code true} if the backing map contained the specified key </returns>
			/// <exception cref="NullPointerException"> if the specified key is null </exception>
			public virtual bool Remove(Object o)
			{
				return Map_Renamed.Remove(o) != java.util.Map_Fields.Null;
			}

			/// <returns> an iterator over the keys of the backing map </returns>
			public virtual IEnumerator<K> GetEnumerator()
			{
				Node<K, V>[] t;
				ConcurrentHashMap<K, V> m = Map_Renamed;
				int f = (t = m.Table) == java.util.Map_Fields.Null ? 0 : t.Length;
				return new KeyIterator<K, V>(t, f, 0, f, m);
			}

			/// <summary>
			/// Adds the specified key to this set view by mapping the key to
			/// the default mapped value in the backing map, if defined.
			/// </summary>
			/// <param name="e"> key to be added </param>
			/// <returns> {@code true} if this set changed as a result of the call </returns>
			/// <exception cref="NullPointerException"> if the specified key is null </exception>
			/// <exception cref="UnsupportedOperationException"> if no default mapped value
			/// for additions was provided </exception>
			public virtual bool Add(K e)
			{
				V java.util.Map_Fields.v;
				if ((java.util.Map_Fields.v = ConcurrentMap_Fields.Value) == java.util.Map_Fields.Null)
				{
					throw new UnsupportedOperationException();
				}
				return Map_Renamed.PutVal(e, java.util.Map_Fields.v, java.util.Map_Fields.True) == java.util.Map_Fields.Null;
			}

			/// <summary>
			/// Adds all of the elements in the specified collection to this set,
			/// as if by calling <seealso cref="#add"/> on each one.
			/// </summary>
			/// <param name="c"> the elements to be inserted into this set </param>
			/// <returns> {@code true} if this set changed as a result of the call </returns>
			/// <exception cref="NullPointerException"> if the collection or any of its
			/// elements are {@code null} </exception>
			/// <exception cref="UnsupportedOperationException"> if no default mapped value
			/// for additions was provided </exception>
			public virtual bool addAll<T1>(ICollection<T1> c) where T1 : K
			{
				bool added = java.util.Map_Fields.False;
				V java.util.Map_Fields.v;
				if ((java.util.Map_Fields.v = ConcurrentMap_Fields.Value) == java.util.Map_Fields.Null)
				{
					throw new UnsupportedOperationException();
				}
				foreach (K e in c)
				{
					if (Map_Renamed.PutVal(e, java.util.Map_Fields.v, java.util.Map_Fields.True) == java.util.Map_Fields.Null)
					{
						added = java.util.Map_Fields.True;
					}
				}
				return added;
			}

			public override int HashCode()
			{
				int h = 0;
				foreach (K e in this)
				{
					h += e.HashCode();
				}
				return h;
			}

			public override bool Equals(Object o)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Set<?> c;
				Set<?> c;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((o instanceof java.util.Set) && ((c = (java.util.Set<?>)o) == this || (containsAll(c) && c.containsAll(this))));
				return ((o is Set) && ((c = (Set<?>)o) == this || (ContainsAll(c) && c.ContainsAll(this))));
			}

			public virtual Spliterator<K> Spliterator()
			{
				Node<K, V>[] t;
				ConcurrentHashMap<K, V> m = Map_Renamed;
				long n = m.SumCount();
				int f = (t = m.Table) == java.util.Map_Fields.Null ? 0 : t.Length;
				return new KeySpliterator<K, V>(t, f, 0, f, n < 0L ? 0L : n);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.Consumer<? base K> action)
			public virtual void forEach<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				Node<K, V>[] t;
				if ((t = Map_Renamed.Table) != java.util.Map_Fields.Null)
				{
					Traverser<K, V> it = new Traverser<K, V>(t, t.Length, 0, t.Length);
					for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
					{
						action.Accept(p.key);
					}
				}
			}
		}

		/// <summary>
		/// A view of a ConcurrentHashMap as a <seealso cref="Collection"/> of
		/// values, in which additions are disabled. This class cannot be
		/// directly instantiated. See <seealso cref="#values()"/>.
		/// </summary>
		[Serializable]
		internal sealed class ValuesView<K, V> : CollectionView<K, V, V>, Collection<V>
		{
			internal const long SerialVersionUID = 2249069246763182397L;
			internal ValuesView(ConcurrentHashMap<K, V> map) : base(map)
			{
			}
			public bool Contains(Object o)
			{
				return Map_Renamed.ContainsValue(o);
			}

			public bool Remove(Object o)
			{
				if (o != java.util.Map_Fields.Null)
				{
					for (IEnumerator<V> it = Iterator(); it.MoveNext();)
					{
						if (o.Equals(it.Current))
						{
							it.remove();
							return java.util.Map_Fields.True;
						}
					}
				}
				return java.util.Map_Fields.False;
			}

			public IEnumerator<V> GetEnumerator()
			{
				ConcurrentHashMap<K, V> m = Map_Renamed;
				Node<K, V>[] t;
				int f = (t = m.Table) == java.util.Map_Fields.Null ? 0 : t.Length;
				return new ValueIterator<K, V>(t, f, 0, f, m);
			}

			public bool Add(V e)
			{
				throw new UnsupportedOperationException();
			}
			public bool addAll<T1>(ICollection<T1> c) where T1 : V
			{
				throw new UnsupportedOperationException();
			}

			public Spliterator<V> Spliterator()
			{
				Node<K, V>[] t;
				ConcurrentHashMap<K, V> m = Map_Renamed;
				long n = m.SumCount();
				int f = (t = m.Table) == java.util.Map_Fields.Null ? 0 : t.Length;
				return new ValueSpliterator<K, V>(t, f, 0, f, n < 0L ? 0L : n);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.Consumer<? base V> action)
			public void forEach<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				Node<K, V>[] t;
				if ((t = Map_Renamed.Table) != java.util.Map_Fields.Null)
				{
					Traverser<K, V> it = new Traverser<K, V>(t, t.Length, 0, t.Length);
					for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
					{
						action.Accept(p.val);
					}
				}
			}
		}

		/// <summary>
		/// A view of a ConcurrentHashMap as a <seealso cref="Set"/> of (key, value)
		/// entries.  This class cannot be directly instantiated. See
		/// <seealso cref="#entrySet()"/>.
		/// </summary>
		[Serializable]
		internal sealed class EntrySetView<K, V> : CollectionView<K, V, java.util.Map_Entry<K, V>>, Set<java.util.Map_Entry<K, V>>
		{
			internal const long SerialVersionUID = 2249069246763182397L;
			internal EntrySetView(ConcurrentHashMap<K, V> map) : base(map)
			{
			}

			public bool Contains(Object o)
			{
				Object java.util.Map_Fields.k, java.util.Map_Fields.v, r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map_Entry<?,?> e;
				java.util.Map_Entry<?, ?> e;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((o instanceof java.util.Map_Entry) && (java.util.Map_Fields.k = (e = (java.util.Map_Entry<?,?>)o).getKey()) != java.util.Map_Fields.null && (r = map.get(java.util.Map_Fields.k)) != java.util.Map_Fields.null && (java.util.Map_Fields.v = e.getValue()) != java.util.Map_Fields.null && (java.util.Map_Fields.v == r || java.util.Map_Fields.v.equals(r)));
				return ((o is java.util.Map_Entry) && (java.util.Map_Fields.k = (e = (java.util.Map_Entry<?, ?>)o).Key) != java.util.Map_Fields.Null && (r = Map_Renamed.Get(java.util.Map_Fields.k)) != java.util.Map_Fields.Null && (java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && (java.util.Map_Fields.v == r || java.util.Map_Fields.v.Equals(r)));
			}

			public bool Remove(Object o)
			{
				Object java.util.Map_Fields.k, java.util.Map_Fields.v;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map_Entry<?,?> e;
				java.util.Map_Entry<?, ?> e;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((o instanceof java.util.Map_Entry) && (java.util.Map_Fields.k = (e = (java.util.Map_Entry<?,?>)o).getKey()) != java.util.Map_Fields.null && (java.util.Map_Fields.v = e.getValue()) != java.util.Map_Fields.null && map.remove(java.util.Map_Fields.k, java.util.Map_Fields.v));
				return ((o is java.util.Map_Entry) && (java.util.Map_Fields.k = (e = (java.util.Map_Entry<?, ?>)o).Key) != java.util.Map_Fields.Null && (java.util.Map_Fields.v = e.Value) != java.util.Map_Fields.Null && Map_Renamed.Remove(java.util.Map_Fields.k, java.util.Map_Fields.v));
			}

			/// <returns> an iterator over the entries of the backing map </returns>
			public IEnumerator<java.util.Map_Entry<K, V>> GetEnumerator()
			{
				ConcurrentHashMap<K, V> m = Map_Renamed;
				Node<K, V>[] t;
				int f = (t = m.Table) == java.util.Map_Fields.Null ? 0 : t.Length;
				return new EntryIterator<K, V>(t, f, 0, f, m);
			}

			public bool Add(java.util.Map_Entry<K, V> e)
			{
				return Map_Renamed.PutVal(e.Key, e.Value, java.util.Map_Fields.False) == java.util.Map_Fields.Null;
			}

			public bool addAll<T1>(ICollection<T1> c) where T1 : java.util.Map_Entry<K,V>
			{
				bool added = java.util.Map_Fields.False;
				foreach (java.util.Map_Entry<K, V> e in c)
				{
					if (Add(e))
					{
						added = java.util.Map_Fields.True;
					}
				}
				return added;
			}

			public override int HashCode()
			{
				int h = 0;
				Node<K, V>[] t;
				if ((t = Map_Renamed.Table) != java.util.Map_Fields.Null)
				{
					Traverser<K, V> it = new Traverser<K, V>(t, t.Length, 0, t.Length);
					for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
					{
						h += p.HashCode();
					}
				}
				return h;
			}

			public override bool Equals(Object o)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Set<?> c;
				Set<?> c;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return ((o instanceof java.util.Set) && ((c = (java.util.Set<?>)o) == this || (containsAll(c) && c.containsAll(this))));
				return ((o is Set) && ((c = (Set<?>)o) == this || (ContainsAll(c) && c.ContainsAll(this))));
			}

			public Spliterator<java.util.Map_Entry<K, V>> Spliterator()
			{
				Node<K, V>[] t;
				ConcurrentHashMap<K, V> m = Map_Renamed;
				long n = m.SumCount();
				int f = (t = m.Table) == java.util.Map_Fields.Null ? 0 : t.Length;
				return new EntrySpliterator<K, V>(t, f, 0, f, n < 0L ? 0L : n, m);
			}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.Consumer<? base java.util.Map_Entry<K,V>> action)
			public void forEach<T1>(Consumer<T1> action)
			{
				if (action == java.util.Map_Fields.Null)
				{
					throw new NullPointerException();
				}
				Node<K, V>[] t;
				if ((t = Map_Renamed.Table) != java.util.Map_Fields.Null)
				{
					Traverser<K, V> it = new Traverser<K, V>(t, t.Length, 0, t.Length);
					for (Node<K, V> p; (p = it.Advance()) != java.util.Map_Fields.Null;)
					{
						action.Accept(new MapEntry<K, V>(p.key, p.val, Map_Renamed));
					}
				}
			}

		}

		// -------------------------------------------------------

		/// <summary>
		/// Base class for bulk tasks. Repeats some fields and code from
		/// class Traverser, because we need to subclass CountedCompleter.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") abstract static class BulkTask<K,V,R> extends CountedCompleter<R>
		internal abstract class BulkTask<K, V, R> : CountedCompleter<R>
		{
			internal Node<K, V>[] Tab; // same as Traverser
			internal Node<K, V> Next;
			internal TableStack<K, V> Stack, Spare;
			internal int Index;
			internal int BaseIndex;
			internal int BaseLimit;
			internal readonly int BaseSize;
			internal int Batch; // split control

			internal BulkTask<T1>(BulkTask<T1> par, int b, int i, int f, Node<K, V>[] t) : base(par)
			{
				this.Batch = b;
				this.Index = this.BaseIndex = i;
				if ((this.Tab = t) == java.util.Map_Fields.Null)
				{
					this.BaseSize = this.BaseLimit = 0;
				}
				else if (par == java.util.Map_Fields.Null)
				{
					this.BaseSize = this.BaseLimit = t.Length;
				}
				else
				{
					this.BaseLimit = f;
					this.BaseSize = par.BaseSize;
				}
			}

			/// <summary>
			/// Same as Traverser version
			/// </summary>
			internal Node<K, V> Advance()
			{
				Node<K, V> e;
				if ((e = Next) != java.util.Map_Fields.Null)
				{
					e = e.Next;
				}
				for (;;)
				{
					Node<K, V>[] t;
					int i, n;
					if (e != java.util.Map_Fields.Null)
					{
						return Next = e;
					}
					if (BaseIndex >= BaseLimit || (t = Tab) == java.util.Map_Fields.Null || (n = t.Length) <= (i = Index) || i < 0)
					{
						return Next = java.util.Map_Fields.Null;
					}
					if ((e = TabAt(t, i)) != java.util.Map_Fields.Null && e.Hash < 0)
					{
						if (e is ForwardingNode)
						{
							Tab = ((ForwardingNode<K, V>)e).NextTable;
							e = java.util.Map_Fields.Null;
							PushState(t, i, n);
							ConcurrentMap_Fields.continue;
						}
						else if (e is TreeBin)
						{
							e = ((TreeBin<K, V>)e).First;
						}
						else
						{
							e = java.util.Map_Fields.Null;
						}
					}
					if (Stack != java.util.Map_Fields.Null)
					{
						RecoverState(n);
					}
					else if ((Index = i + BaseSize) >= n)
					{
						Index = ++BaseIndex;
					}
				}
			}

			internal virtual void PushState(Node<K, V>[] t, int i, int n)
			{
				TableStack<K, V> s = Spare;
				if (s != java.util.Map_Fields.Null)
				{
					Spare = s.Next;
				}
				else
				{
					s = new TableStack<K, V>();
				}
				s.Tab = t;
				s.Length = n;
				s.Index = i;
				s.Next = Stack;
				Stack = s;
			}

			internal virtual void RecoverState(int n)
			{
				TableStack<K, V> s;
				int len;
				while ((s = Stack) != java.util.Map_Fields.Null && (Index += (len = s.Length)) >= n)
				{
					n = len;
					Index = s.Index;
					Tab = s.Tab;
					s.Tab = java.util.Map_Fields.Null;
					TableStack<K, V> next = s.Next;
					s.Next = Spare; // save for reuse
					Stack = next;
					Spare = s;
				}
				if (s == java.util.Map_Fields.Null && (Index += BaseSize) >= n)
				{
					Index = ++BaseIndex;
				}
			}
		}

		/*
		 * Task classes. Coded in a regular but ugly format/style to
		 * simplify checks that each variant differs in the right way from
		 * others. The null screenings exist because compilers cannot tell
		 * that we've already null-checked task arguments, so we force
		 * simplest hoisted bypass to help avoid convoluted traps.
		 */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachKeyTask<K,V> extends BulkTask<K,V,Void>
		internal sealed class ForEachKeyTask<K, V> : BulkTask<K, V, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base K> action;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Consumer<?> Action;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ForEachKeyTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.Consumer<? base K> action)
			internal ForEachKeyTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, Consumer<T2> action) : base(p, b, i, f, t)
			{
				this.Action = action;
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base K> action;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Consumer<?> action;
				if ((action = this.Action) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(new ForEachKeyTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, action)).Fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						action.Accept(p.key);
					}
					PropagateCompletion();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachValueTask<K,V> extends BulkTask<K,V,Void>
		internal sealed class ForEachValueTask<K, V> : BulkTask<K, V, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base V> action;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Consumer<?> Action;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ForEachValueTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.Consumer<? base V> action)
			internal ForEachValueTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, Consumer<T2> action) : base(p, b, i, f, t)
			{
				this.Action = action;
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base V> action;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Consumer<?> action;
				if ((action = this.Action) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(new ForEachValueTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, action)).Fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						action.Accept(p.val);
					}
					PropagateCompletion();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachEntryTask<K,V> extends BulkTask<K,V,Void>
		internal sealed class ForEachEntryTask<K, V> : BulkTask<K, V, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base java.util.Map_Entry<K,V>> action;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Consumer<?> Action;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ForEachEntryTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.Consumer<? base java.util.Map_Entry<K,V>> action)
			internal ForEachEntryTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, Consumer<T2> action) : base(p, b, i, f, t)
			{
				this.Action = action;
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base java.util.Map_Entry<K,V>> action;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Consumer<?> action;
				if ((action = this.Action) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(new ForEachEntryTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, action)).Fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						action.Accept(p);
					}
					PropagateCompletion();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachMappingTask<K,V> extends BulkTask<K,V,Void>
		internal sealed class ForEachMappingTask<K, V> : BulkTask<K, V, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiConsumer<? base K, ? base V> action;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiConsumer<?, ?> Action;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ForEachMappingTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.BiConsumer<? base K,? base V> action)
			internal ForEachMappingTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, BiConsumer<T2> action) : base(p, b, i, f, t)
			{
				this.Action = action;
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiConsumer<? base K, ? base V> action;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiConsumer<?, ?> action;
				if ((action = this.Action) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(new ForEachMappingTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, action)).Fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						action.Accept(p.key, p.val);
					}
					PropagateCompletion();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachTransformedKeyTask<K,V,U> extends BulkTask<K,V,Void>
		internal sealed class ForEachTransformedKeyTask<K, V, U> : BulkTask<K, V, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base K, ? extends U> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Function<?, ?> Transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base U> action;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Consumer<?> Action;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ForEachTransformedKeyTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.Function<? base K, ? extends U> transformer, java.util.function.Consumer<? base U> action)
			internal ForEachTransformedKeyTask<T1, T2, T3>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, Function<T2> transformer, Consumer<T3> action) where T2 : U : base(p, b, i, f, t)
			{
				this.Transformer = transformer;
				this.Action = action;
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base K, ? extends U> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Function<?, ?> transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base U> action;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Consumer<?> action;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (action = this.Action) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(new ForEachTransformedKeyTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, transformer, action)).Fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						U u;
						if ((u = transformer.Apply(p.key)) != java.util.Map_Fields.Null)
						{
							action.Accept(u);
						}
					}
					PropagateCompletion();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachTransformedValueTask<K,V,U> extends BulkTask<K,V,Void>
		internal sealed class ForEachTransformedValueTask<K, V, U> : BulkTask<K, V, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base V, ? extends U> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Function<?, ?> Transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base U> action;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Consumer<?> Action;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ForEachTransformedValueTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.Function<? base V, ? extends U> transformer, java.util.function.Consumer<? base U> action)
			internal ForEachTransformedValueTask<T1, T2, T3>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, Function<T2> transformer, Consumer<T3> action) where T2 : U : base(p, b, i, f, t)
			{
				this.Transformer = transformer;
				this.Action = action;
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base V, ? extends U> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Function<?, ?> transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base U> action;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Consumer<?> action;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (action = this.Action) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(new ForEachTransformedValueTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, transformer, action)).Fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						U u;
						if ((u = transformer.Apply(p.val)) != java.util.Map_Fields.Null)
						{
							action.Accept(u);
						}
					}
					PropagateCompletion();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachTransformedEntryTask<K,V,U> extends BulkTask<K,V,Void>
		internal sealed class ForEachTransformedEntryTask<K, V, U> : BulkTask<K, V, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> transformer;
			internal readonly Function<java.util.Map_Entry<K, V>, ?> Transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base U> action;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Consumer<?> Action;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ForEachTransformedEntryTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> transformer, java.util.function.Consumer<? base U> action)
			internal ForEachTransformedEntryTask<T1, T2, T3>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, Function<T2> transformer, Consumer<T3> action) where T2 : U : base(p, b, i, f, t)
			{
				this.Transformer = transformer;
				this.Action = action;
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Function<java.util.Map_Entry<K, V>, ?> transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base U> action;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Consumer<?> action;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (action = this.Action) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(new ForEachTransformedEntryTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, transformer, action)).Fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						U u;
						if ((u = transformer.Apply(p)) != java.util.Map_Fields.Null)
						{
							action.Accept(u);
						}
					}
					PropagateCompletion();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ForEachTransformedMappingTask<K,V,U> extends BulkTask<K,V,Void>
		internal sealed class ForEachTransformedMappingTask<K, V, U> : BulkTask<K, V, Void>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base K, ? base V, ? extends U> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiFunction<?, ?, ?> Transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base U> action;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Consumer<?> Action;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ForEachTransformedMappingTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.BiFunction<? base K, ? base V, ? extends U> transformer, java.util.function.Consumer<? base U> action)
			internal ForEachTransformedMappingTask<T1, T2, T3>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, BiFunction<T2> transformer, Consumer<T3> action) where T2 : U : base(p, b, i, f, t)
			{
				this.Transformer = transformer;
				this.Action = action;
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base K, ? base V, ? extends U> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<?, ?, ?> transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Consumer<? base U> action;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Consumer<?> action;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (action = this.Action) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(new ForEachTransformedMappingTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, transformer, action)).Fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						U u;
						if ((u = transformer.Apply(p.key, p.val)) != java.util.Map_Fields.Null)
						{
							action.Accept(u);
						}
					}
					PropagateCompletion();
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class SearchKeysTask<K,V,U> extends BulkTask<K,V,U>
		internal sealed class SearchKeysTask<K, V, U> : BulkTask<K, V, U>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base K, ? extends U> searchFunction;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Function<?, ?> SearchFunction;
			internal readonly AtomicReference<U> Result;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: SearchKeysTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.Function<? base K, ? extends U> searchFunction, java.util.concurrent.atomic.AtomicReference<U> result)
			internal SearchKeysTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, Function<T2> searchFunction, AtomicReference<U> result) where T2 : U : base(p, b, i, f, t)
			{
				this.SearchFunction = searchFunction;
				this.Result = result;
			}
			public U RawResult
			{
				get
				{
					return Result.Get();
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base K, ? extends U> searchFunction;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Function<?, ?> searchFunction;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicReference<U> result;
				AtomicReference<U> result;
				if ((searchFunction = this.SearchFunction) != java.util.Map_Fields.Null && (result = this.Result) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						if (result.Get() != java.util.Map_Fields.Null)
						{
							return;
						}
						AddToPendingCount(1);
						(new SearchKeysTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, searchFunction, result)).Fork();
					}
					while (result.Get() == java.util.Map_Fields.Null)
					{
						U u;
						Node<K, V> p;
						if ((p = Advance()) == java.util.Map_Fields.Null)
						{
							PropagateCompletion();
							ConcurrentMap_Fields.break;
						}
						if ((u = searchFunction.Apply(p.Key_Renamed)) != java.util.Map_Fields.Null)
						{
							if (result.CompareAndSet(java.util.Map_Fields.Null, u))
							{
								QuietlyCompleteRoot();
							}
							ConcurrentMap_Fields.break;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class SearchValuesTask<K,V,U> extends BulkTask<K,V,U>
		internal sealed class SearchValuesTask<K, V, U> : BulkTask<K, V, U>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base V, ? extends U> searchFunction;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Function<?, ?> SearchFunction;
			internal readonly AtomicReference<U> Result;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: SearchValuesTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.Function<? base V, ? extends U> searchFunction, java.util.concurrent.atomic.AtomicReference<U> result)
			internal SearchValuesTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, Function<T2> searchFunction, AtomicReference<U> result) where T2 : U : base(p, b, i, f, t)
			{
				this.SearchFunction = searchFunction;
				this.Result = result;
			}
			public U RawResult
			{
				get
				{
					return Result.Get();
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base V, ? extends U> searchFunction;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Function<?, ?> searchFunction;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicReference<U> result;
				AtomicReference<U> result;
				if ((searchFunction = this.SearchFunction) != java.util.Map_Fields.Null && (result = this.Result) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						if (result.Get() != java.util.Map_Fields.Null)
						{
							return;
						}
						AddToPendingCount(1);
						(new SearchValuesTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, searchFunction, result)).Fork();
					}
					while (result.Get() == java.util.Map_Fields.Null)
					{
						U u;
						Node<K, V> p;
						if ((p = Advance()) == java.util.Map_Fields.Null)
						{
							PropagateCompletion();
							ConcurrentMap_Fields.break;
						}
						if ((u = searchFunction.Apply(p.Val)) != java.util.Map_Fields.Null)
						{
							if (result.CompareAndSet(java.util.Map_Fields.Null, u))
							{
								QuietlyCompleteRoot();
							}
							ConcurrentMap_Fields.break;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class SearchEntriesTask<K,V,U> extends BulkTask<K,V,U>
		internal sealed class SearchEntriesTask<K, V, U> : BulkTask<K, V, U>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> searchFunction;
			internal readonly Function<java.util.Map_Entry<K, V>, ?> SearchFunction;
			internal readonly AtomicReference<U> Result;
			internal SearchEntriesTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, Function<T2> searchFunction, AtomicReference<U> result) where T2 : U : base(p, b, i, f, t)
			{
				this.SearchFunction = searchFunction;
				this.Result = result;
			}
			public U RawResult
			{
				get
				{
					return Result.Get();
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> searchFunction;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Function<java.util.Map_Entry<K, V>, ?> searchFunction;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicReference<U> result;
				AtomicReference<U> result;
				if ((searchFunction = this.SearchFunction) != java.util.Map_Fields.Null && (result = this.Result) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						if (result.Get() != java.util.Map_Fields.Null)
						{
							return;
						}
						AddToPendingCount(1);
						(new SearchEntriesTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, searchFunction, result)).Fork();
					}
					while (result.Get() == java.util.Map_Fields.Null)
					{
						U u;
						Node<K, V> p;
						if ((p = Advance()) == java.util.Map_Fields.Null)
						{
							PropagateCompletion();
							ConcurrentMap_Fields.break;
						}
						if ((u = searchFunction.Apply(p)) != java.util.Map_Fields.Null)
						{
							if (result.CompareAndSet(java.util.Map_Fields.Null, u))
							{
								QuietlyCompleteRoot();
							}
							return;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class SearchMappingsTask<K,V,U> extends BulkTask<K,V,U>
		internal sealed class SearchMappingsTask<K, V, U> : BulkTask<K, V, U>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base K, ? base V, ? extends U> searchFunction;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiFunction<?, ?, ?> SearchFunction;
			internal readonly AtomicReference<U> Result;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: SearchMappingsTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, java.util.function.BiFunction<? base K, ? base V, ? extends U> searchFunction, java.util.concurrent.atomic.AtomicReference<U> result)
			internal SearchMappingsTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, BiFunction<T2> searchFunction, AtomicReference<U> result) where T2 : U : base(p, b, i, f, t)
			{
				this.SearchFunction = searchFunction;
				this.Result = result;
			}
			public U RawResult
			{
				get
				{
					return Result.Get();
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base K, ? base V, ? extends U> searchFunction;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<?, ?, ?> searchFunction;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.atomic.AtomicReference<U> result;
				AtomicReference<U> result;
				if ((searchFunction = this.SearchFunction) != java.util.Map_Fields.Null && (result = this.Result) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						if (result.Get() != java.util.Map_Fields.Null)
						{
							return;
						}
						AddToPendingCount(1);
						(new SearchMappingsTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, searchFunction, result)).Fork();
					}
					while (result.Get() == java.util.Map_Fields.Null)
					{
						U u;
						Node<K, V> p;
						if ((p = Advance()) == java.util.Map_Fields.Null)
						{
							PropagateCompletion();
							ConcurrentMap_Fields.break;
						}
						if ((u = searchFunction.Apply(p.Key_Renamed, p.Val)) != java.util.Map_Fields.Null)
						{
							if (result.CompareAndSet(java.util.Map_Fields.Null, u))
							{
								QuietlyCompleteRoot();
							}
							ConcurrentMap_Fields.break;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ReduceKeysTask<K,V> extends BulkTask<K,V,K>
		internal sealed class ReduceKeysTask<K, V> : BulkTask<K, V, K>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base K, ? base K, ? extends K> reducer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiFunction<?, ?, ?> Reducer;
			internal K Result;
			internal ReduceKeysTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ReduceKeysTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, ReduceKeysTask<K,V> nextRight, java.util.function.BiFunction<? base K, ? base K, ? extends K> reducer)
			internal ReduceKeysTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, ReduceKeysTask<K, V> nextRight, BiFunction<T2> reducer) where T2 : K : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Reducer = reducer;
			}
			public K RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base K, ? base K, ? extends K> reducer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<?, ?, ?> reducer;
				if ((reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new ReduceKeysTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, reducer)).fork();
					}
					K r = java.util.Map_Fields.Null;
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						K u = p.key;
						r = (r == java.util.Map_Fields.Null) ? u : u == java.util.Map_Fields.Null ? r : reducer.Apply(r, u);
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ReduceKeysTask<K,V> t = (ReduceKeysTask<K,V>)c, s = t.rights;
						ReduceKeysTask<K, V> t = (ReduceKeysTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							K tr, sr;
							if ((sr = s.Result) != java.util.Map_Fields.Null)
							{
								t.Result = (((tr = t.Result) == java.util.Map_Fields.Null) ? sr : reducer.Apply(tr, sr));
							}
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ReduceValuesTask<K,V> extends BulkTask<K,V,V>
		internal sealed class ReduceValuesTask<K, V> : BulkTask<K, V, V>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base V, ? base V, ? extends V> reducer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiFunction<?, ?, ?> Reducer;
			internal V Result;
			internal ReduceValuesTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: ReduceValuesTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, ReduceValuesTask<K,V> nextRight, java.util.function.BiFunction<? base V, ? base V, ? extends V> reducer)
			internal ReduceValuesTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, ReduceValuesTask<K, V> nextRight, BiFunction<T2> reducer) where T2 : V : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Reducer = reducer;
			}
			public V RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base V, ? base V, ? extends V> reducer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<?, ?, ?> reducer;
				if ((reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new ReduceValuesTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, reducer)).fork();
					}
					V r = java.util.Map_Fields.Null;
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						V java.util.Map_Fields.v = p.val;
						r = (r == java.util.Map_Fields.Null) ? java.util.Map_Fields.v : reducer.Apply(r, java.util.Map_Fields.v);
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ReduceValuesTask<K,V> t = (ReduceValuesTask<K,V>)c, s = t.rights;
						ReduceValuesTask<K, V> t = (ReduceValuesTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							V tr, sr;
							if ((sr = s.Result) != java.util.Map_Fields.Null)
							{
								t.Result = (((tr = t.Result) == java.util.Map_Fields.Null) ? sr : reducer.Apply(tr, sr));
							}
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class ReduceEntriesTask<K,V> extends BulkTask<K,V,java.util.Map_Entry<K,V>>
		internal sealed class ReduceEntriesTask<K, V> : BulkTask<K, V, java.util.Map_Entry<K, V>>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final java.util.function.BiFunction<java.util.Map_Entry<K,V>, java.util.Map_Entry<K,V>, ? extends java.util.Map_Entry<K,V>> reducer;
			internal readonly BiFunction<java.util.Map_Entry<K, V>, java.util.Map_Entry<K, V>, ?> Reducer;
			internal java.util.Map_Entry<K, V> Result;
			internal ReduceEntriesTask<K, V> Rights, NextRight;
			internal ReduceEntriesTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, ReduceEntriesTask<K, V> nextRight, BiFunction<T2> reducer) where T2 : java.util.Map_Entry<K,V> : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Reducer = reducer;
			}
			public java.util.Map_Entry<K, V> RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.BiFunction<java.util.Map_Entry<K,V>, java.util.Map_Entry<K,V>, ? extends java.util.Map_Entry<K,V>> reducer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<java.util.Map_Entry<K, V>, java.util.Map_Entry<K, V>, ?> reducer;
				if ((reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new ReduceEntriesTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, reducer)).fork();
					}
					java.util.Map_Entry<K, V> r = java.util.Map_Fields.Null;
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = (r == java.util.Map_Fields.Null) ? p : reducer.Apply(r, p);
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ReduceEntriesTask<K,V> t = (ReduceEntriesTask<K,V>)c, s = t.rights;
						ReduceEntriesTask<K, V> t = (ReduceEntriesTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							java.util.Map_Entry<K, V> tr, sr;
							if ((sr = s.Result) != java.util.Map_Fields.Null)
							{
								t.Result = (((tr = t.Result) == java.util.Map_Fields.Null) ? sr : reducer.Apply(tr, sr));
							}
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceKeysTask<K,V,U> extends BulkTask<K,V,U>
		internal sealed class MapReduceKeysTask<K, V, U> : BulkTask<K, V, U>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base K, ? extends U> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Function<?, ?> Transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiFunction<?, ?, ?> Reducer;
			internal U Result;
			internal MapReduceKeysTask<K, V, U> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceKeysTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceKeysTask<K,V,U> nextRight, java.util.function.Function<? base K, ? extends U> transformer, java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer)
			internal MapReduceKeysTask<T1, T2, T3>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceKeysTask<K, V, U> nextRight, Function<T2> transformer, BiFunction<T3> reducer) where T2 : U where T3 : U : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Reducer = reducer;
			}
			public U RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base K, ? extends U> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Function<?, ?> transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<?, ?, ?> reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceKeysTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, reducer)).fork();
					}
					U r = java.util.Map_Fields.Null;
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						U u;
						if ((u = transformer.Apply(p.key)) != java.util.Map_Fields.Null)
						{
							r = (r == java.util.Map_Fields.Null) ? u : reducer.Apply(r, u);
						}
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceKeysTask<K,V,U> t = (MapReduceKeysTask<K,V,U>)c, s = t.rights;
						MapReduceKeysTask<K, V, U> t = (MapReduceKeysTask<K, V, U>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							U tr, sr;
							if ((sr = s.Result) != java.util.Map_Fields.Null)
							{
								t.Result = (((tr = t.Result) == java.util.Map_Fields.Null) ? sr : reducer.Apply(tr, sr));
							}
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceValuesTask<K,V,U> extends BulkTask<K,V,U>
		internal sealed class MapReduceValuesTask<K, V, U> : BulkTask<K, V, U>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base V, ? extends U> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly Function<?, ?> Transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiFunction<?, ?, ?> Reducer;
			internal U Result;
			internal MapReduceValuesTask<K, V, U> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceValuesTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceValuesTask<K,V,U> nextRight, java.util.function.Function<? base V, ? extends U> transformer, java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer)
			internal MapReduceValuesTask<T1, T2, T3>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceValuesTask<K, V, U> nextRight, Function<T2> transformer, BiFunction<T3> reducer) where T2 : U where T3 : U : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Reducer = reducer;
			}
			public U RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.Function<? base V, ? extends U> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Function<?, ?> transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<?, ?, ?> reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceValuesTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, reducer)).fork();
					}
					U r = java.util.Map_Fields.Null;
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						U u;
						if ((u = transformer.Apply(p.val)) != java.util.Map_Fields.Null)
						{
							r = (r == java.util.Map_Fields.Null) ? u : reducer.Apply(r, u);
						}
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceValuesTask<K,V,U> t = (MapReduceValuesTask<K,V,U>)c, s = t.rights;
						MapReduceValuesTask<K, V, U> t = (MapReduceValuesTask<K, V, U>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							U tr, sr;
							if ((sr = s.Result) != java.util.Map_Fields.Null)
							{
								t.Result = (((tr = t.Result) == java.util.Map_Fields.Null) ? sr : reducer.Apply(tr, sr));
							}
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceEntriesTask<K,V,U> extends BulkTask<K,V,U>
		internal sealed class MapReduceEntriesTask<K, V, U> : BulkTask<K, V, U>
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: final java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> transformer;
			internal readonly Function<java.util.Map_Entry<K, V>, ?> Transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiFunction<?, ?, ?> Reducer;
			internal U Result;
			internal MapReduceEntriesTask<K, V, U> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceEntriesTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceEntriesTask<K,V,U> nextRight, java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> transformer, java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer)
			internal MapReduceEntriesTask<T1, T2, T3>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceEntriesTask<K, V, U> nextRight, Function<T2> transformer, BiFunction<T3> reducer) where T2 : U where T3 : U : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Reducer = reducer;
			}
			public U RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.Function<java.util.Map_Entry<K,V>, ? extends U> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Function<java.util.Map_Entry<K, V>, ?> transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<?, ?, ?> reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceEntriesTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, reducer)).fork();
					}
					U r = java.util.Map_Fields.Null;
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						U u;
						if ((u = transformer.Apply(p)) != java.util.Map_Fields.Null)
						{
							r = (r == java.util.Map_Fields.Null) ? u : reducer.Apply(r, u);
						}
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceEntriesTask<K,V,U> t = (MapReduceEntriesTask<K,V,U>)c, s = t.rights;
						MapReduceEntriesTask<K, V, U> t = (MapReduceEntriesTask<K, V, U>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							U tr, sr;
							if ((sr = s.Result) != java.util.Map_Fields.Null)
							{
								t.Result = (((tr = t.Result) == java.util.Map_Fields.Null) ? sr : reducer.Apply(tr, sr));
							}
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceMappingsTask<K,V,U> extends BulkTask<K,V,U>
		internal sealed class MapReduceMappingsTask<K, V, U> : BulkTask<K, V, U>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base K, ? base V, ? extends U> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiFunction<?, ?, ?> Transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly BiFunction<?, ?, ?> Reducer;
			internal U Result;
			internal MapReduceMappingsTask<K, V, U> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceMappingsTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceMappingsTask<K,V,U> nextRight, java.util.function.BiFunction<? base K, ? base V, ? extends U> transformer, java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer)
			internal MapReduceMappingsTask<T1, T2, T3>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceMappingsTask<K, V, U> nextRight, BiFunction<T2> transformer, BiFunction<T3> reducer) where T2 : U where T3 : U : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Reducer = reducer;
			}
			public U RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base K, ? base V, ? extends U> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<?, ?, ?> transformer;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.BiFunction<? base U, ? base U, ? extends U> reducer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				BiFunction<?, ?, ?> reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceMappingsTask<K, V, U> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, reducer)).fork();
					}
					U r = java.util.Map_Fields.Null;
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						U u;
						if ((u = transformer.Apply(p.key, p.val)) != java.util.Map_Fields.Null)
						{
							r = (r == java.util.Map_Fields.Null) ? u : reducer.Apply(r, u);
						}
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceMappingsTask<K,V,U> t = (MapReduceMappingsTask<K,V,U>)c, s = t.rights;
						MapReduceMappingsTask<K, V, U> t = (MapReduceMappingsTask<K, V, U>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							U tr, sr;
							if ((sr = s.Result) != java.util.Map_Fields.Null)
							{
								t.Result = (((tr = t.Result) == java.util.Map_Fields.Null) ? sr : reducer.Apply(tr, sr));
							}
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceKeysToDoubleTask<K,V> extends BulkTask<K,V,Double>
		internal sealed class MapReduceKeysToDoubleTask<K, V> : BulkTask<K, V, Double>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToDoubleFunction<? base K> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly ToDoubleFunction<?> Transformer;
			internal readonly DoubleBinaryOperator Reducer;
			internal readonly double Basis;
			internal double Result;
			internal MapReduceKeysToDoubleTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceKeysToDoubleTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceKeysToDoubleTask<K,V> nextRight, java.util.function.ToDoubleFunction<? base K> transformer, double basis, java.util.function.DoubleBinaryOperator reducer)
			internal MapReduceKeysToDoubleTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceKeysToDoubleTask<K, V> nextRight, ToDoubleFunction<T2> transformer, double basis, DoubleBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Double RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToDoubleFunction<? base K> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ToDoubleFunction<?> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.DoubleBinaryOperator reducer;
				DoubleBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					double r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceKeysToDoubleTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsDouble(r, transformer.ApplyAsDouble(p.key));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceKeysToDoubleTask<K,V> t = (MapReduceKeysToDoubleTask<K,V>)c, s = t.rights;
						MapReduceKeysToDoubleTask<K, V> t = (MapReduceKeysToDoubleTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsDouble(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceValuesToDoubleTask<K,V> extends BulkTask<K,V,Double>
		internal sealed class MapReduceValuesToDoubleTask<K, V> : BulkTask<K, V, Double>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToDoubleFunction<? base V> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly ToDoubleFunction<?> Transformer;
			internal readonly DoubleBinaryOperator Reducer;
			internal readonly double Basis;
			internal double Result;
			internal MapReduceValuesToDoubleTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceValuesToDoubleTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceValuesToDoubleTask<K,V> nextRight, java.util.function.ToDoubleFunction<? base V> transformer, double basis, java.util.function.DoubleBinaryOperator reducer)
			internal MapReduceValuesToDoubleTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceValuesToDoubleTask<K, V> nextRight, ToDoubleFunction<T2> transformer, double basis, DoubleBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Double RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToDoubleFunction<? base V> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ToDoubleFunction<?> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.DoubleBinaryOperator reducer;
				DoubleBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					double r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceValuesToDoubleTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsDouble(r, transformer.ApplyAsDouble(p.val));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceValuesToDoubleTask<K,V> t = (MapReduceValuesToDoubleTask<K,V>)c, s = t.rights;
						MapReduceValuesToDoubleTask<K, V> t = (MapReduceValuesToDoubleTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsDouble(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceEntriesToDoubleTask<K,V> extends BulkTask<K,V,Double>
		internal sealed class MapReduceEntriesToDoubleTask<K, V> : BulkTask<K, V, Double>
		{
			internal readonly ToDoubleFunction<java.util.Map_Entry<K, V>> Transformer;
			internal readonly DoubleBinaryOperator Reducer;
			internal readonly double Basis;
			internal double Result;
			internal MapReduceEntriesToDoubleTask<K, V> Rights, NextRight;
			internal MapReduceEntriesToDoubleTask<T1>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceEntriesToDoubleTask<K, V> nextRight, ToDoubleFunction<java.util.Map_Entry<K, V>> transformer, double basis, DoubleBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Double RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.ToDoubleFunction<java.util.Map_Entry<K,V>> transformer;
				ToDoubleFunction<java.util.Map_Entry<K, V>> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.DoubleBinaryOperator reducer;
				DoubleBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					double r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceEntriesToDoubleTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsDouble(r, transformer.ApplyAsDouble(p));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceEntriesToDoubleTask<K,V> t = (MapReduceEntriesToDoubleTask<K,V>)c, s = t.rights;
						MapReduceEntriesToDoubleTask<K, V> t = (MapReduceEntriesToDoubleTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsDouble(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceMappingsToDoubleTask<K,V> extends BulkTask<K,V,Double>
		internal sealed class MapReduceMappingsToDoubleTask<K, V> : BulkTask<K, V, Double>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToDoubleBiFunction<? base K, ? base V> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly ToDoubleBiFunction<?, ?> Transformer;
			internal readonly DoubleBinaryOperator Reducer;
			internal readonly double Basis;
			internal double Result;
			internal MapReduceMappingsToDoubleTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceMappingsToDoubleTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceMappingsToDoubleTask<K,V> nextRight, java.util.function.ToDoubleBiFunction<? base K, ? base V> transformer, double basis, java.util.function.DoubleBinaryOperator reducer)
			internal MapReduceMappingsToDoubleTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceMappingsToDoubleTask<K, V> nextRight, ToDoubleBiFunction<T2> transformer, double basis, DoubleBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Double RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToDoubleBiFunction<? base K, ? base V> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ToDoubleBiFunction<?, ?> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.DoubleBinaryOperator reducer;
				DoubleBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					double r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceMappingsToDoubleTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsDouble(r, transformer.ApplyAsDouble(p.key, p.val));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceMappingsToDoubleTask<K,V> t = (MapReduceMappingsToDoubleTask<K,V>)c, s = t.rights;
						MapReduceMappingsToDoubleTask<K, V> t = (MapReduceMappingsToDoubleTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsDouble(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceKeysToLongTask<K,V> extends BulkTask<K,V,Long>
		internal sealed class MapReduceKeysToLongTask<K, V> : BulkTask<K, V, Long>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToLongFunction<? base K> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly ToLongFunction<?> Transformer;
			internal readonly LongBinaryOperator Reducer;
			internal readonly long Basis;
			internal long Result;
			internal MapReduceKeysToLongTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceKeysToLongTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceKeysToLongTask<K,V> nextRight, java.util.function.ToLongFunction<? base K> transformer, long basis, java.util.function.LongBinaryOperator reducer)
			internal MapReduceKeysToLongTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceKeysToLongTask<K, V> nextRight, ToLongFunction<T2> transformer, long basis, LongBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Long RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToLongFunction<? base K> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ToLongFunction<?> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.LongBinaryOperator reducer;
				LongBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					long r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceKeysToLongTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsLong(r, transformer.ApplyAsLong(p.key));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceKeysToLongTask<K,V> t = (MapReduceKeysToLongTask<K,V>)c, s = t.rights;
						MapReduceKeysToLongTask<K, V> t = (MapReduceKeysToLongTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsLong(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceValuesToLongTask<K,V> extends BulkTask<K,V,Long>
		internal sealed class MapReduceValuesToLongTask<K, V> : BulkTask<K, V, Long>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToLongFunction<? base V> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly ToLongFunction<?> Transformer;
			internal readonly LongBinaryOperator Reducer;
			internal readonly long Basis;
			internal long Result;
			internal MapReduceValuesToLongTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceValuesToLongTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceValuesToLongTask<K,V> nextRight, java.util.function.ToLongFunction<? base V> transformer, long basis, java.util.function.LongBinaryOperator reducer)
			internal MapReduceValuesToLongTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceValuesToLongTask<K, V> nextRight, ToLongFunction<T2> transformer, long basis, LongBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Long RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToLongFunction<? base V> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ToLongFunction<?> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.LongBinaryOperator reducer;
				LongBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					long r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceValuesToLongTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsLong(r, transformer.ApplyAsLong(p.val));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceValuesToLongTask<K,V> t = (MapReduceValuesToLongTask<K,V>)c, s = t.rights;
						MapReduceValuesToLongTask<K, V> t = (MapReduceValuesToLongTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsLong(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceEntriesToLongTask<K,V> extends BulkTask<K,V,Long>
		internal sealed class MapReduceEntriesToLongTask<K, V> : BulkTask<K, V, Long>
		{
			internal readonly ToLongFunction<java.util.Map_Entry<K, V>> Transformer;
			internal readonly LongBinaryOperator Reducer;
			internal readonly long Basis;
			internal long Result;
			internal MapReduceEntriesToLongTask<K, V> Rights, NextRight;
			internal MapReduceEntriesToLongTask<T1>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceEntriesToLongTask<K, V> nextRight, ToLongFunction<java.util.Map_Entry<K, V>> transformer, long basis, LongBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Long RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.ToLongFunction<java.util.Map_Entry<K,V>> transformer;
				ToLongFunction<java.util.Map_Entry<K, V>> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.LongBinaryOperator reducer;
				LongBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					long r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceEntriesToLongTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsLong(r, transformer.ApplyAsLong(p));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceEntriesToLongTask<K,V> t = (MapReduceEntriesToLongTask<K,V>)c, s = t.rights;
						MapReduceEntriesToLongTask<K, V> t = (MapReduceEntriesToLongTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsLong(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceMappingsToLongTask<K,V> extends BulkTask<K,V,Long>
		internal sealed class MapReduceMappingsToLongTask<K, V> : BulkTask<K, V, Long>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToLongBiFunction<? base K, ? base V> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly ToLongBiFunction<?, ?> Transformer;
			internal readonly LongBinaryOperator Reducer;
			internal readonly long Basis;
			internal long Result;
			internal MapReduceMappingsToLongTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceMappingsToLongTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceMappingsToLongTask<K,V> nextRight, java.util.function.ToLongBiFunction<? base K, ? base V> transformer, long basis, java.util.function.LongBinaryOperator reducer)
			internal MapReduceMappingsToLongTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceMappingsToLongTask<K, V> nextRight, ToLongBiFunction<T2> transformer, long basis, LongBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Long RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToLongBiFunction<? base K, ? base V> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ToLongBiFunction<?, ?> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.LongBinaryOperator reducer;
				LongBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					long r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceMappingsToLongTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsLong(r, transformer.ApplyAsLong(p.key, p.val));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceMappingsToLongTask<K,V> t = (MapReduceMappingsToLongTask<K,V>)c, s = t.rights;
						MapReduceMappingsToLongTask<K, V> t = (MapReduceMappingsToLongTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsLong(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceKeysToIntTask<K,V> extends BulkTask<K,V,Integer>
		internal sealed class MapReduceKeysToIntTask<K, V> : BulkTask<K, V, Integer>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToIntFunction<? base K> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly ToIntFunction<?> Transformer;
			internal readonly IntBinaryOperator Reducer;
			internal readonly int Basis;
			internal int Result;
			internal MapReduceKeysToIntTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceKeysToIntTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceKeysToIntTask<K,V> nextRight, java.util.function.ToIntFunction<? base K> transformer, int basis, java.util.function.IntBinaryOperator reducer)
			internal MapReduceKeysToIntTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceKeysToIntTask<K, V> nextRight, ToIntFunction<T2> transformer, int basis, IntBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Integer RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToIntFunction<? base K> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ToIntFunction<?> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.IntBinaryOperator reducer;
				IntBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					int r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceKeysToIntTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsInt(r, transformer.ApplyAsInt(p.key));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceKeysToIntTask<K,V> t = (MapReduceKeysToIntTask<K,V>)c, s = t.rights;
						MapReduceKeysToIntTask<K, V> t = (MapReduceKeysToIntTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsInt(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceValuesToIntTask<K,V> extends BulkTask<K,V,Integer>
		internal sealed class MapReduceValuesToIntTask<K, V> : BulkTask<K, V, Integer>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToIntFunction<? base V> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly ToIntFunction<?> Transformer;
			internal readonly IntBinaryOperator Reducer;
			internal readonly int Basis;
			internal int Result;
			internal MapReduceValuesToIntTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceValuesToIntTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceValuesToIntTask<K,V> nextRight, java.util.function.ToIntFunction<? base V> transformer, int basis, java.util.function.IntBinaryOperator reducer)
			internal MapReduceValuesToIntTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceValuesToIntTask<K, V> nextRight, ToIntFunction<T2> transformer, int basis, IntBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Integer RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToIntFunction<? base V> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ToIntFunction<?> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.IntBinaryOperator reducer;
				IntBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					int r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceValuesToIntTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsInt(r, transformer.ApplyAsInt(p.val));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceValuesToIntTask<K,V> t = (MapReduceValuesToIntTask<K,V>)c, s = t.rights;
						MapReduceValuesToIntTask<K, V> t = (MapReduceValuesToIntTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsInt(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceEntriesToIntTask<K,V> extends BulkTask<K,V,Integer>
		internal sealed class MapReduceEntriesToIntTask<K, V> : BulkTask<K, V, Integer>
		{
			internal readonly ToIntFunction<java.util.Map_Entry<K, V>> Transformer;
			internal readonly IntBinaryOperator Reducer;
			internal readonly int Basis;
			internal int Result;
			internal MapReduceEntriesToIntTask<K, V> Rights, NextRight;
			internal MapReduceEntriesToIntTask<T1>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceEntriesToIntTask<K, V> nextRight, ToIntFunction<java.util.Map_Entry<K, V>> transformer, int basis, IntBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Integer RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.ToIntFunction<java.util.Map_Entry<K,V>> transformer;
				ToIntFunction<java.util.Map_Entry<K, V>> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.IntBinaryOperator reducer;
				IntBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					int r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceEntriesToIntTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsInt(r, transformer.ApplyAsInt(p));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceEntriesToIntTask<K,V> t = (MapReduceEntriesToIntTask<K,V>)c, s = t.rights;
						MapReduceEntriesToIntTask<K, V> t = (MapReduceEntriesToIntTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsInt(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") static final class MapReduceMappingsToIntTask<K,V> extends BulkTask<K,V,Integer>
		internal sealed class MapReduceMappingsToIntTask<K, V> : BulkTask<K, V, Integer>
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToIntBiFunction<? base K, ? base V> transformer;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			internal readonly ToIntBiFunction<?, ?> Transformer;
			internal readonly IntBinaryOperator Reducer;
			internal readonly int Basis;
			internal int Result;
			internal MapReduceMappingsToIntTask<K, V> Rights, NextRight;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: MapReduceMappingsToIntTask(BulkTask<K,V,?> p, int b, int i, int f, Node<K,V>[] t, MapReduceMappingsToIntTask<K,V> nextRight, java.util.function.ToIntBiFunction<? base K, ? base V> transformer, int basis, java.util.function.IntBinaryOperator reducer)
			internal MapReduceMappingsToIntTask<T1, T2>(BulkTask<T1> p, int b, int i, int f, Node<K, V>[] t, MapReduceMappingsToIntTask<K, V> nextRight, ToIntBiFunction<T2> transformer, int basis, IntBinaryOperator reducer) : base(p, b, i, f, t)
			{
				this.NextRight = nextRight;
				this.Transformer = transformer;
				this.Basis = basis;
				this.Reducer = reducer;
			}
			public Integer RawResult
			{
				get
				{
					return Result;
				}
			}
			public void Compute()
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: final java.util.function.ToIntBiFunction<? base K, ? base V> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				ToIntBiFunction<?, ?> transformer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.function.IntBinaryOperator reducer;
				IntBinaryOperator reducer;
				if ((transformer = this.Transformer) != java.util.Map_Fields.Null && (reducer = this.Reducer) != java.util.Map_Fields.Null)
				{
					int r = this.Basis;
					for (int i = BaseIndex, f, h; Batch > 0 && (h = (int)((uint)((f = BaseLimit) + i) >> 1)) > i;)
					{
						AddToPendingCount(1);
						(Rights = new MapReduceMappingsToIntTask<K, V> (this, Batch = (int)((uint)Batch >> 1), BaseLimit = h, f, Tab, Rights, transformer, r, reducer)).fork();
					}
					for (Node<K, V> p; (p = Advance()) != java.util.Map_Fields.Null;)
					{
						r = reducer.ApplyAsInt(r, transformer.ApplyAsInt(p.key, p.val));
					}
					Result = r;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: CountedCompleter<?> c;
					CountedCompleter<?> c;
					for (c = FirstComplete(); c != java.util.Map_Fields.Null; c = c.NextComplete())
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") MapReduceMappingsToIntTask<K,V> t = (MapReduceMappingsToIntTask<K,V>)c, s = t.rights;
						MapReduceMappingsToIntTask<K, V> t = (MapReduceMappingsToIntTask<K, V>)c, s = t.Rights;
						while (s != java.util.Map_Fields.Null)
						{
							t.Result = reducer.ApplyAsInt(t.Result, s.Result);
							s = t.Rights = s.NextRight;
						}
					}
				}
			}
		}

		// Unsafe mechanics
		private static readonly sun.misc.Unsafe U;
		private static readonly long SIZECTL;
		private static readonly long TRANSFERINDEX;
		private static readonly long BASECOUNT;
		private static readonly long CELLSBUSY;
		private static readonly long CELLVALUE;
		private static readonly long ABASE;
		private static readonly int ASHIFT;

		static ConcurrentHashMap()
		{
			try
			{
				U = sun.misc.Unsafe.Unsafe;
				Class java.util.Map_Fields.k = typeof(ConcurrentHashMap);
				SIZECTL = U.objectFieldOffset(java.util.Map_Fields.k.getDeclaredField("sizeCtl"));
				TRANSFERINDEX = U.objectFieldOffset(java.util.Map_Fields.k.getDeclaredField("transferIndex"));
				BASECOUNT = U.objectFieldOffset(java.util.Map_Fields.k.getDeclaredField("baseCount"));
				CELLSBUSY = U.objectFieldOffset(java.util.Map_Fields.k.getDeclaredField("cellsBusy"));
				Class ck = typeof(CounterCell);
				CELLVALUE = U.objectFieldOffset(ck.GetDeclaredField("value"));
				Class ak = typeof(Node[]);
				ABASE = U.arrayBaseOffset(ak);
				int scale = U.arrayIndexScale(ak);
				if ((scale & (scale - 1)) != 0)
				{
					throw new Error("data type scale not a power of two");
				}
				ASHIFT = 31 - Integer.NumberOfLeadingZeros(scale);
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
		}
	}

}