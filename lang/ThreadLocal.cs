using System.Threading;

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

namespace java.lang
{

	/// <summary>
	/// This class provides thread-local variables.  These variables differ from
	/// their normal counterparts in that each thread that accesses one (via its
	/// {@code get} or {@code set} method) has its own, independently initialized
	/// copy of the variable.  {@code ThreadLocal} instances are typically private
	/// static fields in classes that wish to associate state with a thread (e.g.,
	/// a user ID or Transaction ID).
	/// 
	/// <para>For example, the class below generates unique identifiers local to each
	/// thread.
	/// A thread's id is assigned the first time it invokes {@code ThreadId.get()}
	/// and remains unchanged on subsequent calls.
	/// <pre>
	/// import java.util.concurrent.atomic.AtomicInteger;
	/// 
	/// public class ThreadId {
	///     // Atomic integer containing the next thread ID to be assigned
	///     private static final AtomicInteger nextId = new AtomicInteger(0);
	/// 
	///     // Thread local variable containing each thread's ID
	///     private static final ThreadLocal&lt;Integer&gt; threadId =
	///         new ThreadLocal&lt;Integer&gt;() {
	///             &#64;Override protected Integer initialValue() {
	///                 return nextId.getAndIncrement();
	///         }
	///     };
	/// 
	///     // Returns the current thread's unique ID, assigning it if necessary
	///     public static int get() {
	///         return threadId.get();
	///     }
	/// }
	/// </pre>
	/// </para>
	/// <para>Each thread holds an implicit reference to its copy of a thread-local
	/// variable as long as the thread is alive and the {@code ThreadLocal}
	/// instance is accessible; after a thread goes away, all of its copies of
	/// thread-local instances are subject to garbage collection (unless other
	/// references to these copies exist).
	/// 
	/// @author  Josh Bloch and Doug Lea
	/// @since   1.2
	/// </para>
	/// </summary>
	public class ThreadLocal<T>
	{
		/// <summary>
		/// ThreadLocals rely on per-thread linear-probe hash maps attached
		/// to each thread (Thread.threadLocals and
		/// inheritableThreadLocals).  The ThreadLocal objects act as keys,
		/// searched via threadLocalHashCode.  This is a custom hash code
		/// (useful only within ThreadLocalMaps) that eliminates collisions
		/// in the common case where consecutively constructed ThreadLocals
		/// are used by the same threads, while remaining well-behaved in
		/// less common cases.
		/// </summary>
		private readonly int ThreadLocalHashCode = NextHashCode();

		/// <summary>
		/// The next hash code to be given out. Updated atomically. Starts at
		/// zero.
		/// </summary>
		private static AtomicInteger NextHashCode_Renamed = new AtomicInteger();

		/// <summary>
		/// The difference between successively generated hash codes - turns
		/// implicit sequential thread-local IDs into near-optimally spread
		/// multiplicative hash values for power-of-two-sized tables.
		/// </summary>
		private const int HASH_INCREMENT = 0x61c88647;

		/// <summary>
		/// Returns the next hash code.
		/// </summary>
		private static int NextHashCode()
		{
			return NextHashCode_Renamed.GetAndAdd(HASH_INCREMENT);
		}

		/// <summary>
		/// Returns the current thread's "initial value" for this
		/// thread-local variable.  This method will be invoked the first
		/// time a thread accesses the variable with the <seealso cref="#get"/>
		/// method, unless the thread previously invoked the <seealso cref="#set"/>
		/// method, in which case the {@code initialValue} method will not
		/// be invoked for the thread.  Normally, this method is invoked at
		/// most once per thread, but it may be invoked again in case of
		/// subsequent invocations of <seealso cref="#remove"/> followed by <seealso cref="#get"/>.
		/// 
		/// <para>This implementation simply returns {@code null}; if the
		/// programmer desires thread-local variables to have an initial
		/// value other than {@code null}, {@code ThreadLocal} must be
		/// subclassed, and this method overridden.  Typically, an
		/// anonymous inner class will be used.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the initial value for this thread-local </returns>
		protected internal virtual T InitialValue()
		{
			return null;
		}

		/// <summary>
		/// Creates a thread local variable. The initial value of the variable is
		/// determined by invoking the {@code get} method on the {@code Supplier}.
		/// </summary>
		/// @param <S> the type of the thread local's value </param>
		/// <param name="supplier"> the supplier to be used to determine the initial value </param>
		/// <returns> a new thread local variable </returns>
		/// <exception cref="NullPointerException"> if the specified supplier is null
		/// @since 1.8 </exception>
		public static ThreadLocal<S> withInitial<S, T1>(Supplier<T1> supplier) where T1 : S
		{
			return new SuppliedThreadLocal<>(supplier);
		}

		/// <summary>
		/// Creates a thread local variable. </summary>
		/// <seealso cref= #withInitial(java.util.function.Supplier) </seealso>
		public ThreadLocal()
		{
		}

		/// <summary>
		/// Returns the value in the current thread's copy of this
		/// thread-local variable.  If the variable has no value for the
		/// current thread, it is first initialized to the value returned
		/// by an invocation of the <seealso cref="#initialValue"/> method.
		/// </summary>
		/// <returns> the current thread's value of this thread-local </returns>
		public virtual T Get()
		{
			Thread t = Thread.CurrentThread;
			ThreadLocalMap map = GetMap(t);
			if (map != null)
			{
				ThreadLocalMap.Entry e = map.GetEntry(this);
				if (e != null)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T result = (T)e.value;
					T result = (T)e.Value;
					return result;
				}
			}
			return SetInitialValue();
		}

		/// <summary>
		/// Variant of set() to establish initialValue. Used instead
		/// of set() in case user has overridden the set() method.
		/// </summary>
		/// <returns> the initial value </returns>
		private T SetInitialValue()
		{
			T value = InitialValue();
			Thread t = Thread.CurrentThread;
			ThreadLocalMap map = GetMap(t);
			if (map != null)
			{
				map.Set(this, value);
			}
			else
			{
				CreateMap(t, value);
			}
			return value;
		}

		/// <summary>
		/// Sets the current thread's copy of this thread-local variable
		/// to the specified value.  Most subclasses will have no need to
		/// override this method, relying solely on the <seealso cref="#initialValue"/>
		/// method to set the values of thread-locals.
		/// </summary>
		/// <param name="value"> the value to be stored in the current thread's copy of
		///        this thread-local. </param>
		public virtual void Set(T value)
		{
			Thread t = Thread.CurrentThread;
			ThreadLocalMap map = GetMap(t);
			if (map != null)
			{
				map.Set(this, value);
			}
			else
			{
				CreateMap(t, value);
			}
		}

		/// <summary>
		/// Removes the current thread's value for this thread-local
		/// variable.  If this thread-local variable is subsequently
		/// <seealso cref="#get read"/> by the current thread, its value will be
		/// reinitialized by invoking its <seealso cref="#initialValue"/> method,
		/// unless its value is <seealso cref="#set set"/> by the current thread
		/// in the interim.  This may result in multiple invocations of the
		/// {@code initialValue} method in the current thread.
		/// 
		/// @since 1.5
		/// </summary>
		 public virtual void Remove()
		 {
			 ThreadLocalMap m = GetMap(Thread.CurrentThread);
			 if (m != null)
			 {
				 m.Remove(this);
			 }
		 }

		/// <summary>
		/// Get the map associated with a ThreadLocal. Overridden in
		/// InheritableThreadLocal.
		/// </summary>
		/// <param name="t"> the current thread </param>
		/// <returns> the map </returns>
		internal virtual ThreadLocalMap GetMap(Thread t)
		{
			return t.ThreadLocals;
		}

		/// <summary>
		/// Create the map associated with a ThreadLocal. Overridden in
		/// InheritableThreadLocal.
		/// </summary>
		/// <param name="t"> the current thread </param>
		/// <param name="firstValue"> value for the initial entry of the map </param>
		internal virtual void CreateMap(Thread t, T firstValue)
		{
			t.ThreadLocals = new ThreadLocalMap(this, firstValue);
		}

		/// <summary>
		/// Factory method to create map of inherited thread locals.
		/// Designed to be called only from Thread constructor.
		/// </summary>
		/// <param name="parentMap"> the map associated with parent thread </param>
		/// <returns> a map containing the parent's inheritable bindings </returns>
		internal static ThreadLocalMap CreateInheritedMap(ThreadLocalMap parentMap)
		{
			return new ThreadLocalMap(parentMap);
		}

		/// <summary>
		/// Method childValue is visibly defined in subclass
		/// InheritableThreadLocal, but is internally defined here for the
		/// sake of providing createInheritedMap factory method without
		/// needing to subclass the map class in InheritableThreadLocal.
		/// This technique is preferable to the alternative of embedding
		/// instanceof tests in methods.
		/// </summary>
		internal virtual T ChildValue(T parentValue)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// An extension of ThreadLocal that obtains its initial value from
		/// the specified {@code Supplier}.
		/// </summary>
		internal sealed class SuppliedThreadLocal<T> : ThreadLocal<T>
		{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final java.util.function.Supplier<? extends T> supplier;
			internal readonly Supplier<?> Supplier;

			internal SuppliedThreadLocal<T1>(Supplier<T1> supplier) where T1 : T
			{
				this.Supplier = Objects.RequireNonNull(supplier);
			}

			protected internal override T InitialValue()
			{
				return Supplier.Get();
			}
		}

		/// <summary>
		/// ThreadLocalMap is a customized hash map suitable only for
		/// maintaining thread local values. No operations are exported
		/// outside of the ThreadLocal class. The class is package private to
		/// allow declaration of fields in class Thread.  To help deal with
		/// very large and long-lived usages, the hash table entries use
		/// WeakReferences for keys. However, since reference queues are not
		/// used, stale entries are guaranteed to be removed only when
		/// the table starts running out of space.
		/// </summary>
		internal class ThreadLocalMap
		{

			/// <summary>
			/// The entries in this hash map extend WeakReference, using
			/// its main ref field as the key (which is always a
			/// ThreadLocal object).  Note that null keys (i.e. entry.get()
			/// == null) mean that the key is no longer referenced, so the
			/// entry can be expunged from table.  Such entries are referred to
			/// as "stale entries" in the code that follows.
			/// </summary>
			internal class Entry : WeakReference<ThreadLocal<JavaToDotNetGenericWildcard>>
			{
				/// <summary>
				/// The value associated with this ThreadLocal. </summary>
				internal Object Value;

				internal Entry<T1>(ThreadLocal<T1> k, Object v) : base(k)
				{
					Value = v;
				}
			}

			/// <summary>
			/// The initial capacity -- MUST be a power of two.
			/// </summary>
			internal const int INITIAL_CAPACITY = 16;

			/// <summary>
			/// The table, resized as necessary.
			/// table.length MUST always be a power of two.
			/// </summary>
			internal Entry[] Table;

			/// <summary>
			/// The number of entries in the table.
			/// </summary>
			internal int Size = 0;

			/// <summary>
			/// The next size value at which to resize.
			/// </summary>
			internal int Threshold_Renamed; // Default to 0

			/// <summary>
			/// Set the resize threshold to maintain at worst a 2/3 load factor.
			/// </summary>
			internal virtual int Threshold
			{
				set
				{
					Threshold_Renamed = value * 2 / 3;
				}
			}

			/// <summary>
			/// Increment i modulo len.
			/// </summary>
			internal static int NextIndex(int i, int len)
			{
				return ((i + 1 < len) ? i + 1 : 0);
			}

			/// <summary>
			/// Decrement i modulo len.
			/// </summary>
			internal static int PrevIndex(int i, int len)
			{
				return ((i - 1 >= 0) ? i - 1 : len - 1);
			}

			/// <summary>
			/// Construct a new map initially containing (firstKey, firstValue).
			/// ThreadLocalMaps are constructed lazily, so we only create
			/// one when we have at least one entry to put in it.
			/// </summary>
			internal ThreadLocalMap<T1>(ThreadLocal<T1> firstKey, Object firstValue)
			{
				Table = new Entry[INITIAL_CAPACITY];
				int i = firstKey.ThreadLocalHashCode & (INITIAL_CAPACITY - 1);
				Table[i] = new Entry(firstKey, firstValue);
				Size = 1;
				Threshold = INITIAL_CAPACITY;
			}

			/// <summary>
			/// Construct a new map including all Inheritable ThreadLocals
			/// from given parent map. Called only by createInheritedMap.
			/// </summary>
			/// <param name="parentMap"> the map associated with parent thread. </param>
			internal ThreadLocalMap(ThreadLocalMap parentMap)
			{
				Entry[] parentTable = parentMap.Table;
				int len = parentTable.Length;
				Threshold = len;
				Table = new Entry[len];

				for (int j = 0; j < len; j++)
				{
					Entry e = parentTable[j];
					if (e != null)
					{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") ThreadLocal<Object> key = (ThreadLocal<Object>) e.get();
						ThreadLocal<Object> key = (ThreadLocal<Object>) e.Get();
						if (key != null)
						{
							Object value = key.ChildValue(e.Value);
							Entry c = new Entry(key, value);
							int h = key.ThreadLocalHashCode & (len - 1);
							while (Table[h] != null)
							{
								h = NextIndex(h, len);
							}
							Table[h] = c;
							Size++;
						}
					}
				}
			}

			/// <summary>
			/// Get the entry associated with key.  This method
			/// itself handles only the fast path: a direct hit of existing
			/// key. It otherwise relays to getEntryAfterMiss.  This is
			/// designed to maximize performance for direct hits, in part
			/// by making this method readily inlinable.
			/// </summary>
			/// <param name="key"> the thread local object </param>
			/// <returns> the entry associated with key, or null if no such </returns>
			internal virtual Entry getEntry<T1>(ThreadLocal<T1> key)
			{
				int i = key.ThreadLocalHashCode & (Table.Length - 1);
				Entry e = Table[i];
				if (e != null && e.Get() == key)
				{
					return e;
				}
				else
				{
					return GetEntryAfterMiss(key, i, e);
				}
			}

			/// <summary>
			/// Version of getEntry method for use when key is not found in
			/// its direct hash slot.
			/// </summary>
			/// <param name="key"> the thread local object </param>
			/// <param name="i"> the table index for key's hash code </param>
			/// <param name="e"> the entry at table[i] </param>
			/// <returns> the entry associated with key, or null if no such </returns>
			internal virtual Entry getEntryAfterMiss<T1>(ThreadLocal<T1> key, int i, Entry e)
			{
				Entry[] tab = Table;
				int len = tab.Length;

				while (e != null)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ThreadLocal<?> k = e.get();
					ThreadLocal<?> k = e.Get();
					if (k == key)
					{
						return e;
					}
					if (k == null)
					{
						ExpungeStaleEntry(i);
					}
					else
					{
						i = NextIndex(i, len);
					}
					e = tab[i];
				}
				return null;
			}

			/// <summary>
			/// Set the value associated with key.
			/// </summary>
			/// <param name="key"> the thread local object </param>
			/// <param name="value"> the value to be set </param>
			internal virtual void set<T1>(ThreadLocal<T1> key, Object value)
			{

				// We don't use a fast path as with get() because it is at
				// least as common to use set() to create new entries as
				// it is to replace existing ones, in which case, a fast
				// path would fail more often than not.

				Entry[] tab = Table;
				int len = tab.Length;
				int i = key.ThreadLocalHashCode & (len - 1);

				for (Entry e = tab[i]; e != null; e = tab[i = NextIndex(i, len)])
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ThreadLocal<?> k = e.get();
					ThreadLocal<?> k = e.Get();

					if (k == key)
					{
						e.Value = value;
						return;
					}

					if (k == null)
					{
						ReplaceStaleEntry(key, value, i);
						return;
					}
				}

				tab[i] = new Entry(key, value);
				int sz = ++Size;
				if (!CleanSomeSlots(i, sz) && sz >= Threshold_Renamed)
				{
					Rehash();
				}
			}

			/// <summary>
			/// Remove the entry for key.
			/// </summary>
			internal virtual void remove<T1>(ThreadLocal<T1> key)
			{
				Entry[] tab = Table;
				int len = tab.Length;
				int i = key.ThreadLocalHashCode & (len - 1);
				for (Entry e = tab[i]; e != null; e = tab[i = NextIndex(i, len)])
				{
					if (e.Get() == key)
					{
						e.Clear();
						ExpungeStaleEntry(i);
						return;
					}
				}
			}

			/// <summary>
			/// Replace a stale entry encountered during a set operation
			/// with an entry for the specified key.  The value passed in
			/// the value parameter is stored in the entry, whether or not
			/// an entry already exists for the specified key.
			/// 
			/// As a side effect, this method expunges all stale entries in the
			/// "run" containing the stale entry.  (A run is a sequence of entries
			/// between two null slots.)
			/// </summary>
			/// <param name="key"> the key </param>
			/// <param name="value"> the value to be associated with key </param>
			/// <param name="staleSlot"> index of the first stale entry encountered while
			///         searching for key. </param>
			internal virtual void replaceStaleEntry<T1>(ThreadLocal<T1> key, Object value, int staleSlot)
			{
				Entry[] tab = Table;
				int len = tab.Length;
				Entry e;

				// Back up to check for prior stale entry in current run.
				// We clean out whole runs at a time to avoid continual
				// incremental rehashing due to garbage collector freeing
				// up refs in bunches (i.e., whenever the collector runs).
				int slotToExpunge = staleSlot;
				for (int i = PrevIndex(staleSlot, len); (e = tab[i]) != null; i = PrevIndex(i, len))
				{
					if (e.Get() == null)
					{
						slotToExpunge = i;
					}
				}

				// Find either the key or trailing null slot of run, whichever
				// occurs first
				for (int i = NextIndex(staleSlot, len); (e = tab[i]) != null; i = NextIndex(i, len))
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ThreadLocal<?> k = e.get();
					ThreadLocal<?> k = e.Get();

					// If we find key, then we need to swap it
					// with the stale entry to maintain hash table order.
					// The newly stale slot, or any other stale slot
					// encountered above it, can then be sent to expungeStaleEntry
					// to remove or rehash all of the other entries in run.
					if (k == key)
					{
						e.Value = value;

						tab[i] = tab[staleSlot];
						tab[staleSlot] = e;

						// Start expunge at preceding stale entry if it exists
						if (slotToExpunge == staleSlot)
						{
							slotToExpunge = i;
						}
						CleanSomeSlots(ExpungeStaleEntry(slotToExpunge), len);
						return;
					}

					// If we didn't find stale entry on backward scan, the
					// first stale entry seen while scanning for key is the
					// first still present in the run.
					if (k == null && slotToExpunge == staleSlot)
					{
						slotToExpunge = i;
					}
				}

				// If key not found, put new entry in stale slot
				tab[staleSlot].Value = null;
				tab[staleSlot] = new Entry(key, value);

				// If there are any other stale entries in run, expunge them
				if (slotToExpunge != staleSlot)
				{
					CleanSomeSlots(ExpungeStaleEntry(slotToExpunge), len);
				}
			}

			/// <summary>
			/// Expunge a stale entry by rehashing any possibly colliding entries
			/// lying between staleSlot and the next null slot.  This also expunges
			/// any other stale entries encountered before the trailing null.  See
			/// Knuth, Section 6.4
			/// </summary>
			/// <param name="staleSlot"> index of slot known to have null key </param>
			/// <returns> the index of the next null slot after staleSlot
			/// (all between staleSlot and this slot will have been checked
			/// for expunging). </returns>
			internal virtual int ExpungeStaleEntry(int staleSlot)
			{
				Entry[] tab = Table;
				int len = tab.Length;

				// expunge entry at staleSlot
				tab[staleSlot].Value = null;
				tab[staleSlot] = null;
				Size--;

				// Rehash until we encounter null
				Entry e;
				int i;
				for (i = NextIndex(staleSlot, len); (e = tab[i]) != null; i = NextIndex(i, len))
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ThreadLocal<?> k = e.get();
					ThreadLocal<?> k = e.Get();
					if (k == null)
					{
						e.Value = null;
						tab[i] = null;
						Size--;
					}
					else
					{
						int h = k.ThreadLocalHashCode & (len - 1);
						if (h != i)
						{
							tab[i] = null;

							// Unlike Knuth 6.4 Algorithm R, we must scan until
							// null because multiple entries could have been stale.
							while (tab[h] != null)
							{
								h = NextIndex(h, len);
							}
							tab[h] = e;
						}
					}
				}
				return i;
			}

			/// <summary>
			/// Heuristically scan some cells looking for stale entries.
			/// This is invoked when either a new element is added, or
			/// another stale one has been expunged. It performs a
			/// logarithmic number of scans, as a balance between no
			/// scanning (fast but retains garbage) and a number of scans
			/// proportional to number of elements, that would find all
			/// garbage but would cause some insertions to take O(n) time.
			/// </summary>
			/// <param name="i"> a position known NOT to hold a stale entry. The
			/// scan starts at the element after i.
			/// </param>
			/// <param name="n"> scan control: {@code log2(n)} cells are scanned,
			/// unless a stale entry is found, in which case
			/// {@code log2(table.length)-1} additional cells are scanned.
			/// When called from insertions, this parameter is the number
			/// of elements, but when from replaceStaleEntry, it is the
			/// table length. (Note: all this could be changed to be either
			/// more or less aggressive by weighting n instead of just
			/// using straight log n. But this version is simple, fast, and
			/// seems to work well.)
			/// </param>
			/// <returns> true if any stale entries have been removed. </returns>
			internal virtual bool CleanSomeSlots(int i, int n)
			{
				bool removed = false;
				Entry[] tab = Table;
				int len = tab.Length;
				do
				{
					i = NextIndex(i, len);
					Entry e = tab[i];
					if (e != null && e.Get() == null)
					{
						n = len;
						removed = true;
						i = ExpungeStaleEntry(i);
					}
				} while ((n = (int)((uint)n >> 1)) != 0);
				return removed;
			}

			/// <summary>
			/// Re-pack and/or re-size the table. First scan the entire
			/// table removing stale entries. If this doesn't sufficiently
			/// shrink the size of the table, double the table size.
			/// </summary>
			internal virtual void Rehash()
			{
				ExpungeStaleEntries();

				// Use lower threshold for doubling to avoid hysteresis
				if (Size >= Threshold_Renamed - Threshold_Renamed / 4)
				{
					Resize();
				}
			}

			/// <summary>
			/// Double the capacity of the table.
			/// </summary>
			internal virtual void Resize()
			{
				Entry[] oldTab = Table;
				int oldLen = oldTab.Length;
				int newLen = oldLen * 2;
				Entry[] newTab = new Entry[newLen];
				int count = 0;

				for (int j = 0; j < oldLen; ++j)
				{
					Entry e = oldTab[j];
					if (e != null)
					{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ThreadLocal<?> k = e.get();
						ThreadLocal<?> k = e.Get();
						if (k == null)
						{
							e.Value = null; // Help the GC
						}
						else
						{
							int h = k.ThreadLocalHashCode & (newLen - 1);
							while (newTab[h] != null)
							{
								h = NextIndex(h, newLen);
							}
							newTab[h] = e;
							count++;
						}
					}
				}

				Threshold = newLen;
				Size = count;
				Table = newTab;
			}

			/// <summary>
			/// Expunge all stale entries in the table.
			/// </summary>
			internal virtual void ExpungeStaleEntries()
			{
				Entry[] tab = Table;
				int len = tab.Length;
				for (int j = 0; j < len; j++)
				{
					Entry e = tab[j];
					if (e != null && e.Get() == null)
					{
						ExpungeStaleEntry(j);
					}
				}
			}
		}
	}

}