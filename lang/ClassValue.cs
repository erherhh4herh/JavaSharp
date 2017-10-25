using System;

/*
 * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Lazily associate a computed value with (potentially) every type.
	/// For example, if a dynamic language needs to construct a message dispatch
	/// table for each class encountered at a message send call site,
	/// it can use a {@code ClassValue} to cache information needed to
	/// perform the message send quickly, for each class encountered.
	/// @author John Rose, JSR 292 EG
	/// @since 1.7
	/// </summary>
	public abstract class ClassValue<T>
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			Version_Renamed = new Version<T>(this);
		}

		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal ClassValue()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Computes the given class's derived value for this {@code ClassValue}.
		/// <para>
		/// This method will be invoked within the first thread that accesses
		/// the value with the <seealso cref="#get get"/> method.
		/// </para>
		/// <para>
		/// Normally, this method is invoked at most once per class,
		/// but it may be invoked again if there has been a call to
		/// <seealso cref="#remove remove"/>.
		/// </para>
		/// <para>
		/// If this method throws an exception, the corresponding call to {@code get}
		/// will terminate abnormally with that exception, and no class value will be recorded.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the type whose class value must be computed </param>
		/// <returns> the newly computed value associated with this {@code ClassValue}, for the given class or interface </returns>
		/// <seealso cref= #get </seealso>
		/// <seealso cref= #remove </seealso>
		protected internal abstract T ComputeValue(Class type);

		/// <summary>
		/// Returns the value for the given class.
		/// If no value has yet been computed, it is obtained by
		/// an invocation of the <seealso cref="#computeValue computeValue"/> method.
		/// <para>
		/// The actual installation of the value on the class
		/// is performed atomically.
		/// At that point, if several racing threads have
		/// computed values, one is chosen, and returned to
		/// all the racing threads.
		/// </para>
		/// <para>
		/// The {@code type} parameter is typically a class, but it may be any type,
		/// such as an interface, a primitive type (like {@code int.class}), or {@code void.class}.
		/// </para>
		/// <para>
		/// In the absence of {@code remove} calls, a class value has a simple
		/// state diagram:  uninitialized and initialized.
		/// When {@code remove} calls are made,
		/// the rules for value observation are more complex.
		/// See the documentation for <seealso cref="#remove remove"/> for more information.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the type whose class value must be computed or retrieved </param>
		/// <returns> the current value associated with this {@code ClassValue}, for the given class or interface </returns>
		/// <exception cref="NullPointerException"> if the argument is null </exception>
		/// <seealso cref= #remove </seealso>
		/// <seealso cref= #computeValue </seealso>
		public virtual T Get(Class type)
		{
			// non-racing this.hashCodeForCache : final int
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?>[] cache;
			Entry<?>[] cache;
			Entry<T> e = probeHomeLocation(cache = GetCacheCarefully(type), this);
			// racing e : current value <=> stale value from current cache or from stale cache
			// invariant:  e is null or an Entry with readable Entry.version and Entry.value
			if (Match(e))
				// invariant:  No false positive matches.  False negatives are OK if rare.
				// The key fact that makes this work: if this.version == e.version,
				// then this thread has a right to observe (final) e.value.
			{
				return e.Value();
			}
			// The fast path can fail for any of these reasons:
			// 1. no entry has been computed yet
			// 2. hash code collision (before or after reduction mod cache.length)
			// 3. an entry has been removed (either on this type or another)
			// 4. the GC has somehow managed to delete e.version and clear the reference
			return GetFromBackup(cache, type);
		}

		/// <summary>
		/// Removes the associated value for the given class.
		/// If this value is subsequently <seealso cref="#get read"/> for the same class,
		/// its value will be reinitialized by invoking its <seealso cref="#computeValue computeValue"/> method.
		/// This may result in an additional invocation of the
		/// {@code computeValue} method for the given class.
		/// <para>
		/// In order to explain the interaction between {@code get} and {@code remove} calls,
		/// we must model the state transitions of a class value to take into account
		/// the alternation between uninitialized and initialized states.
		/// To do this, number these states sequentially from zero, and note that
		/// uninitialized (or removed) states are numbered with even numbers,
		/// while initialized (or re-initialized) states have odd numbers.
		/// </para>
		/// <para>
		/// When a thread {@code T} removes a class value in state {@code 2N},
		/// nothing happens, since the class value is already uninitialized.
		/// Otherwise, the state is advanced atomically to {@code 2N+1}.
		/// </para>
		/// <para>
		/// When a thread {@code T} queries a class value in state {@code 2N},
		/// the thread first attempts to initialize the class value to state {@code 2N+1}
		/// by invoking {@code computeValue} and installing the resulting value.
		/// </para>
		/// <para>
		/// When {@code T} attempts to install the newly computed value,
		/// if the state is still at {@code 2N}, the class value will be initialized
		/// with the computed value, advancing it to state {@code 2N+1}.
		/// </para>
		/// <para>
		/// Otherwise, whether the new state is even or odd,
		/// {@code T} will discard the newly computed value
		/// and retry the {@code get} operation.
		/// </para>
		/// <para>
		/// Discarding and retrying is an important proviso,
		/// since otherwise {@code T} could potentially install
		/// a disastrously stale value.  For example:
		/// <ul>
		/// <li>{@code T} calls {@code CV.get(C)} and sees state {@code 2N}
		/// <li>{@code T} quickly computes a time-dependent value {@code V0} and gets ready to install it
		/// <li>{@code T} is hit by an unlucky paging or scheduling event, and goes to sleep for a long time
		/// <li>...meanwhile, {@code T2} also calls {@code CV.get(C)} and sees state {@code 2N}
		/// <li>{@code T2} quickly computes a similar time-dependent value {@code V1} and installs it on {@code CV.get(C)}
		/// <li>{@code T2} (or a third thread) then calls {@code CV.remove(C)}, undoing {@code T2}'s work
		/// <li> the previous actions of {@code T2} are repeated several times
		/// <li> also, the relevant computed values change over time: {@code V1}, {@code V2}, ...
		/// <li>...meanwhile, {@code T} wakes up and attempts to install {@code V0}; <em>this must fail</em>
		/// </ul>
		/// We can assume in the above scenario that {@code CV.computeValue} uses locks to properly
		/// observe the time-dependent states as it computes {@code V1}, etc.
		/// This does not remove the threat of a stale value, since there is a window of time
		/// between the return of {@code computeValue} in {@code T} and the installation
		/// of the the new value.  No user synchronization is possible during this time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="type"> the type whose class value must be removed </param>
		/// <exception cref="NullPointerException"> if the argument is null </exception>
		public virtual void Remove(Class type)
		{
			ClassValueMap map = GetMap(type);
			map.RemoveEntry(this);
		}

		// Possible functionality for JSR 292 MR 1
		/*public*/	 internal virtual void Put(Class type, T value)
	 {
			ClassValueMap map = GetMap(type);
			map.ChangeEntry(this, value);
	 }

		/// --------
		/// Implementation...
		/// --------

		/// <summary>
		/// Return the cache, if it exists, else a dummy empty cache. </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static Entry<?>[] getCacheCarefully(Class type)
		private static Entry<?>[] GetCacheCarefully(Class type)
		{
			// racing type.classValueMap{.cacheArray} : null => new Entry[X] <=> new Entry[Y]
			ClassValueMap map = type.ClassValueMap;
			if (map == null)
			{
				return EMPTY_CACHE;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?>[] cache = map.getCache();
			Entry<?>[] cache = map.Cache;
			return cache;
			// invariant:  returned value is safe to dereference and check for an Entry
		}

		/// <summary>
		/// Initial, one-element, empty cache used by all Class instances.  Must never be filled. </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static final Entry<?>[] EMPTY_CACHE = { null };
		private static readonly Entry<?>[] EMPTY_CACHE = new Entry[] {null};

		/// <summary>
		/// Slow tail of ClassValue.get to retry at nearby locations in the cache,
		/// or take a slow lock and check the hash table.
		/// Called only if the first probe was empty or a collision.
		/// This is a separate method, so compilers can process it independently.
		/// </summary>
		private T getFromBackup<T1>(Entry<T1>[] cache, Class type)
		{
			Entry<T> e = probeBackupLocations(cache, this);
			if (e != null)
			{
				return e.Value();
			}
			return GetFromHashMap(type);
		}

		// Hack to suppress warnings on the (T) cast, which is a no-op.
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<T> castEntry(Entry<?> e)
		internal virtual Entry<T> CastEntry(Entry<T1> e)
		{
			return (Entry<T>) e;
		}

		/// <summary>
		/// Called when the fast path of get fails, and cache reprobe also fails.
		/// </summary>
		private T GetFromHashMap(Class type)
		{
			// The fail-safe recovery is to fall back to the underlying classValueMap.
			ClassValueMap map = GetMap(type);
			for (;;)
			{
				Entry<T> e = map.StartEntry(this);
				if (!e.Promise)
				{
					return e.Value();
				}
				try
				{
					// Try to make a real entry for the promised version.
					e = MakeEntry(e.Version(), ComputeValue(type));
				}
				finally
				{
					// Whether computeValue throws or returns normally,
					// be sure to remove the empty entry.
					e = map.FinishEntry(this, e);
				}
				if (e != null)
				{
					return e.Value();
				}
				// else try again, in case a racing thread called remove (so e == null)
			}
		}

		/// <summary>
		/// Check that e is non-null, matches this ClassValue, and is live. </summary>
		internal virtual bool match<T1>(Entry<T1> e)
		{
			// racing e.version : null (blank) => unique Version token => null (GC-ed version)
			// non-racing this.version : v1 => v2 => ... (updates are read faithfully from volatile)
			return (e != null && e.get() == this.Version_Renamed);
			// invariant:  No false positives on version match.  Null is OK for false negative.
			// invariant:  If version matches, then e.value is readable (final set in Entry.<init>)
		}

		/// <summary>
		/// Internal hash code for accessing Class.classValueMap.cacheArray. </summary>
		internal readonly int HashCodeForCache = NextHashCode.GetAndAdd(HASH_INCREMENT) & HASH_MASK;

		/// <summary>
		/// Value stream for hashCodeForCache.  See similar structure in ThreadLocal. </summary>
		private static readonly AtomicInteger NextHashCode = new AtomicInteger();

		/// <summary>
		/// Good for power-of-two tables.  See similar structure in ThreadLocal. </summary>
		private const int HASH_INCREMENT = 0x61c88647;

		/// <summary>
		/// Mask a hash code to be positive but not too large, to prevent wraparound. </summary>
		internal static readonly int HASH_MASK = (-1 >> > 2);

		/// <summary>
		/// Private key for retrieval of this object from ClassValueMap.
		/// </summary>
		internal class Identity
		{
		}
		/// <summary>
		/// This ClassValue's identity, expressed as an opaque object.
		/// The main object {@code ClassValue.this} is incorrect since
		/// subclasses may override {@code ClassValue.equals}, which
		/// could confuse keys in the ClassValueMap.
		/// </summary>
		internal readonly Identity Identity = new Identity();

		/// <summary>
		/// Current version for retrieving this class value from the cache.
		/// Any number of computeValue calls can be cached in association with one version.
		/// But the version changes when a remove (on any type) is executed.
		/// A version change invalidates all cache entries for the affected ClassValue,
		/// by marking them as stale.  Stale cache entries do not force another call
		/// to computeValue, but they do require a synchronized visit to a backing map.
		/// <para>
		/// All user-visible state changes on the ClassValue take place under
		/// a lock inside the synchronized methods of ClassValueMap.
		/// Readers (of ClassValue.get) are notified of such state changes
		/// when this.version is bumped to a new token.
		/// This variable must be volatile so that an unsynchronized reader
		/// will receive the notification without delay.
		/// </para>
		/// <para>
		/// If version were not volatile, one thread T1 could persistently hold onto
		/// a stale value this.value == V1, while while another thread T2 advances
		/// (under a lock) to this.value == V2.  This will typically be harmless,
		/// but if T1 and T2 interact causally via some other channel, such that
		/// T1's further actions are constrained (in the JMM) to happen after
		/// the V2 event, then T1's observation of V1 will be an error.
		/// </para>
		/// <para>
		/// The practical effect of making this.version be volatile is that it cannot
		/// be hoisted out of a loop (by an optimizing JIT) or otherwise cached.
		/// Some machines may also require a barrier instruction to execute
		/// before this.version.
		/// </para>
		/// </summary>
		private volatile Version<T> Version_Renamed;
		internal virtual Version<T> Version()
		{
			return Version_Renamed;
		}
		internal virtual void BumpVersion()
		{
			Version_Renamed = new Version<>(this);
		}
		internal class Version<T>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				Promise_Renamed = new Entry<T>(this);
			}

			internal readonly ClassValue<T> ClassValue_Renamed;
			internal Entry<T> Promise_Renamed;
			internal Version(ClassValue<T> classValue)
			{
				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
				this.ClassValue_Renamed = classValue;
			}
			internal virtual ClassValue<T> ClassValue()
			{
				return ClassValue_Renamed;
			}
			internal virtual Entry<T> Promise()
			{
				return Promise_Renamed;
			}
			internal virtual bool Live
			{
				get
				{
					return ClassValue_Renamed.Version() == this;
				}
			}
		}

		/// <summary>
		/// One binding of a value to a class via a ClassValue.
		///  States are:<ul>
		///  <li> promise if value == Entry.this
		///  <li> else dead if version == null
		///  <li> else stale if version != classValue.version
		///  <li> else live </ul>
		///  Promises are never put into the cache; they only live in the
		///  backing map while a computeValue call is in flight.
		///  Once an entry goes stale, it can be reset at any time
		///  into the dead state.
		/// </summary>
		internal class Entry<T> : WeakReference<Version<T>>
		{
			internal readonly Object Value_Renamed; // usually of type T, but sometimes (Entry)this
			internal Entry(Version<T> version, T value) : base(version)
			{
				this.Value_Renamed = value; // for a regular entry, value is of type T
			}
			internal virtual void AssertNotPromise()
			{
				assert(!Promise);
			}
			/// <summary>
			/// For creating a promise. </summary>
			internal Entry(Version<T> version) : base(version)
			{
				this.Value_Renamed = this; // for a promise, value is not of type T, but Entry!
			}
			/// <summary>
			/// Fetch the value.  This entry must not be a promise. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T value()
			internal virtual T Value() // if !isPromise, type is T
			{
				AssertNotPromise();
				return (T) Value_Renamed;
			}
			internal virtual bool Promise
			{
				get
				{
					return Value_Renamed == this;
				}
			}
			internal virtual Version<T> Version()
			{
				return get();
			}
			internal virtual ClassValue<T> ClassValueOrNull()
			{
				Version<T> v = Version();
				return (v == null) ? null : v.ClassValue();
			}
			internal virtual bool Live
			{
				get
				{
					Version<T> v = Version();
					if (v == null)
					{
						return false;
					}
					if (v.Live)
					{
						return true;
					}
					clear();
					return false;
				}
			}
			internal virtual Entry<T> RefreshVersion(Version<T> v2)
			{
				AssertNotPromise();
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<T> e2 = new Entry<>(v2, (T) value);
				Entry<T> e2 = new Entry<T>(v2, (T) Value_Renamed); // if !isPromise, type is T
				clear();
				// value = null -- caller must drop
				return e2;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static final Entry<?> DEAD_ENTRY = new Entry<>(null, null);
			internal static readonly Entry<?> DEAD_ENTRY = new Entry<?>(null, null);
		}

		/// <summary>
		/// Return the backing map associated with this type. </summary>
		private static ClassValueMap GetMap(Class type)
		{
			// racing type.classValueMap : null (blank) => unique ClassValueMap
			// if a null is observed, a map is created (lazily, synchronously, uniquely)
			// all further access to that map is synchronized
			ClassValueMap map = type.ClassValueMap;
			if (map != null)
			{
				return map;
			}
			return InitializeMap(type);
		}

		private static readonly Object CRITICAL_SECTION = new Object();
		private static ClassValueMap InitializeMap(Class type)
		{
			ClassValueMap map;
			lock (CRITICAL_SECTION) // private object to avoid deadlocks
			{
				// happens about once per type
				if ((map = type.ClassValueMap) == null)
				{
					type.ClassValueMap = map = new ClassValueMap(type);
				}
			}
				return map;
		}

		internal static Entry<T> makeEntry<T>(Version<T> explicitVersion, T value)
		{
			// Note that explicitVersion might be different from this.version.
			return new Entry<>(explicitVersion, value);

			// As soon as the Entry is put into the cache, the value will be
			// reachable via a data race (as defined by the Java Memory Model).
			// This race is benign, assuming the value object itself can be
			// read safely by multiple threads.  This is up to the user.
			//
			// The entry and version fields themselves can be safely read via
			// a race because they are either final or have controlled states.
			// If the pointer from the entry to the version is still null,
			// or if the version goes immediately dead and is nulled out,
			// the reader will take the slow path and retry under a lock.
		}

		// The following class could also be top level and non-public:

		/// <summary>
		/// A backing map for all ClassValues, relative a single given type.
		///  Gives a fully serialized "true state" for each pair (ClassValue cv, Class type).
		///  Also manages an unserialized fast-path cache.
		/// </summary>
		internal class ClassValueMap : WeakHashMap<ClassValue.Identity, Entry<JavaToDotNetGenericWildcard>>
		{
			internal readonly Class Type;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Entry<?>[] cacheArray;
			internal Entry<?>[] CacheArray;
			internal int CacheLoad, CacheLoadLimit;

			/// <summary>
			/// Number of entries initially allocated to each type when first used with any ClassValue.
			///  It would be pointless to make this much smaller than the Class and ClassValueMap objects themselves.
			///  Must be a power of 2.
			/// </summary>
			internal const int INITIAL_ENTRIES = 32;

			/// <summary>
			/// Build a backing map for ClassValues, relative the given type.
			///  Also, create an empty cache array and install it on the class.
			/// </summary>
			internal ClassValueMap(Class type)
			{
				this.Type = type;
				SizeCache(INITIAL_ENTRIES);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?>[] getCache()
			internal virtual Entry<?>[] Cache
			{
				get
				{
					return CacheArray;
				}
			}

			/// <summary>
			/// Initiate a query.  Store a promise (placeholder) if there is no value yet. </summary>
			internal virtual Entry<T> startEntry<T>(ClassValue<T> classValue)
			{
				lock (this)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<T> e = (Entry<T>) get(classValue.identity);
					Entry<T> e = (Entry<T>) Get(classValue.Identity); // one map has entries for all value types <T>
					Version<T> java.util.Map_Fields.v = classValue.Version();
					if (e == java.util.Map_Fields.Null)
					{
						e = java.util.Map_Fields.v.promise();
						// The presence of a promise means that a value is pending for v.
						// Eventually, finishEntry will overwrite the promise.
						Put(classValue.Identity, e);
						// Note that the promise is never entered into the cache!
						return e;
					}
					else if (e.Promise)
					{
						// Somebody else has asked the same question.
						// Let the races begin!
						if (e.Version() != java.util.Map_Fields.v)
						{
							e = java.util.Map_Fields.v.promise();
							Put(classValue.Identity, e);
						}
						return e;
					}
					else
					{
						// there is already a completed entry here; report it
						if (e.Version() != java.util.Map_Fields.v)
						{
							// There is a stale but valid entry here; make it fresh again.
							// Once an entry is in the hash table, we don't care what its version is.
							e = e.RefreshVersion(java.util.Map_Fields.v);
							Put(classValue.Identity, e);
						}
						// Add to the cache, to enable the fast path, next time.
						CheckCacheLoad();
						AddToCache(classValue, e);
						return e;
					}
				}
			}

			/// <summary>
			/// Finish a query.  Overwrite a matching placeholder.  Drop stale incoming values. </summary>
			internal virtual Entry<T> finishEntry<T>(ClassValue<T> classValue, Entry<T> e)
			{
				lock (this)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<T> e0 = (Entry<T>) get(classValue.identity);
					Entry<T> e0 = (Entry<T>) Get(classValue.Identity); // one map has entries for all value types <T>
					if (e == e0)
					{
						// We can get here during exception processing, unwinding from computeValue.
						assert(e.Promise);
						Remove(classValue.Identity);
						return java.util.Map_Fields.Null;
					}
					else if (e0 != java.util.Map_Fields.Null && e0.Promise && e0.Version() == e.Version())
					{
						// If e0 matches the intended entry, there has not been a remove call
						// between the previous startEntry and now.  So now overwrite e0.
						Version<T> java.util.Map_Fields.v = classValue.Version();
						if (e.Version() != java.util.Map_Fields.v)
						{
							e = e.RefreshVersion(java.util.Map_Fields.v);
						}
						Put(classValue.Identity, e);
						// Add to the cache, to enable the fast path, next time.
						CheckCacheLoad();
						AddToCache(classValue, e);
						return e;
					}
					else
					{
						// Some sort of mismatch; caller must try again.
						return java.util.Map_Fields.Null;
					}
				}
			}

			/// <summary>
			/// Remove an entry. </summary>
			internal virtual void removeEntry<T1>(ClassValue<T1> classValue)
			{
				lock (this)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?> e = remove(classValue.identity);
					Entry<?> e = Remove(classValue.Identity);
					if (e == java.util.Map_Fields.Null)
					{
						// Uninitialized, and no pending calls to computeValue.  No change.
					}
					else if (e.Promise)
					{
						// State is uninitialized, with a pending call to finishEntry.
						// Since remove is a no-op in such a state, keep the promise
						// by putting it back into the map.
						Put(classValue.Identity, e);
					}
					else
					{
						// In an initialized state.  Bump forward, and de-initialize.
						classValue.BumpVersion();
						// Make all cache elements for this guy go stale.
						RemoveStaleEntries(classValue);
					}
				}
			}

			/// <summary>
			/// Change the value for an entry. </summary>
			internal virtual void changeEntry<T>(ClassValue<T> classValue, T value)
			{
				lock (this)
				{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") Entry<T> e0 = (Entry<T>) get(classValue.identity);
					Entry<T> e0 = (Entry<T>) Get(classValue.Identity); // one map has entries for all value types <T>
					Version<T> version = classValue.Version();
					if (e0 != java.util.Map_Fields.Null)
					{
						if (e0.Version() == version && e0.Value() == value)
						{
							// no value change => no version change needed
							return;
						}
						classValue.BumpVersion();
						RemoveStaleEntries(classValue);
					}
					Entry<T> e = MakeEntry(version, value);
					Put(classValue.Identity, e);
					// Add to the cache, to enable the fast path, next time.
					CheckCacheLoad();
					AddToCache(classValue, e);
				}
			}

			/// --------
			/// Cache management.
			/// --------

			// Statics do not need synchronization.

			/// <summary>
			/// Load the cache entry at the given (hashed) location. </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static Entry<?> loadFromCache(Entry<?>[] cache, int i)
			internal static Entry<?> LoadFromCache(Entry<T1>[] cache, int i)
			{
				// non-racing cache.length : constant
				// racing cache[i & (mask)] : null <=> Entry
				return cache[i & (cache.Length - 1)];
				// invariant:  returned value is null or well-constructed (ready to match)
			}

			/// <summary>
			/// Look in the cache, at the home location for the given ClassValue. </summary>
			internal static Entry<T> probeHomeLocation<T, T1>(Entry<T1>[] cache, ClassValue<T> classValue)
			{
				return classValue.CastEntry(LoadFromCache(cache, classValue.HashCodeForCache));
			}

			/// <summary>
			/// Given that first probe was a collision, retry at nearby locations. </summary>
			internal static Entry<T> probeBackupLocations<T, T1>(Entry<T1>[] cache, ClassValue<T> classValue)
			{
				if (PROBE_LIMIT <= 0)
				{
					return java.util.Map_Fields.Null;
				}
				// Probe the cache carefully, in a range of slots.
				int mask = (cache.Length - 1);
				int home = (classValue.HashCodeForCache & mask);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?> e2 = cache[home];
				Entry<?> e2 = cache[home]; // victim, if we find the real guy
				if (e2 == java.util.Map_Fields.Null)
				{
					return java.util.Map_Fields.Null; // if nobody is at home, no need to search nearby
				}
				// assume !classValue.match(e2), but do not assert, because of races
				int pos2 = -1;
				for (int i = home + 1; i < home + PROBE_LIMIT; i++)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?> e = cache[i & mask];
					Entry<?> e = cache[i & mask];
					if (e == java.util.Map_Fields.Null)
					{
						break; // only search within non-null runs
					}
					if (classValue.Match(e))
					{
						// relocate colliding entry e2 (from cache[home]) to first empty slot
						cache[home] = e;
						if (pos2 >= 0)
						{
							cache[i & mask] = Entry.DEAD_ENTRY;
						}
						else
						{
							pos2 = i;
						}
						cache[pos2 & mask] = ((EntryDislocation(cache, pos2, e2) < PROBE_LIMIT) ? e2 : Entry.DEAD_ENTRY); // put e2 here if it fits
						return classValue.CastEntry(e);
					}
					// Remember first empty slot, if any:
					if (!e.Live && pos2 < 0)
					{
						pos2 = i;
					}
				}
				return java.util.Map_Fields.Null;
			}

			/// <summary>
			/// How far out of place is e? </summary>
			internal static int entryDislocation<T1, T2>(Entry<T1>[] cache, int pos, Entry<T2> e)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ClassValue<?> cv = e.classValueOrNull();
				ClassValue<?> cv = e.ClassValueOrNull();
				if (cv == java.util.Map_Fields.Null) // entry is not live!
				{
					return 0;
				}
				int mask = (cache.Length - 1);
				return (pos - cv.HashCodeForCache) & mask;
			}

			/// --------
			/// Below this line all functions are private, and assume synchronized access.
			/// --------

			internal virtual void SizeCache(int length)
			{
				assert((length & (length - 1)) == 0); // must be power of 2
				CacheLoad = 0;
				CacheLoadLimit = (int)((double) length * CACHE_LOAD_LIMIT / 100);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: cacheArray = new Entry<?>[length];
				CacheArray = new Entry<?>[length];
			}

			/// <summary>
			/// Make sure the cache load stays below its limit, if possible. </summary>
			internal virtual void CheckCacheLoad()
			{
				if (CacheLoad >= CacheLoadLimit)
				{
					ReduceCacheLoad();
				}
			}
			internal virtual void ReduceCacheLoad()
			{
				RemoveStaleEntries();
				if (CacheLoad < CacheLoadLimit)
				{
					return; // win
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?>[] oldCache = getCache();
				Entry<?>[] oldCache = Cache;
				if (oldCache.Length > HASH_MASK)
				{
					return; // lose
				}
				SizeCache(oldCache.Length * 2);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Entry<?> e : oldCache)
				foreach (Entry<?> e in oldCache)
				{
					if (e != java.util.Map_Fields.Null && e.Live)
					{
						AddToCache(e);
					}
				}
			}

			/// <summary>
			/// Remove stale entries in the given range.
			///  Should be executed under a Map lock.
			/// </summary>
			internal virtual void removeStaleEntries<T1>(Entry<T1>[] cache, int begin, int count)
			{
				if (PROBE_LIMIT <= 0)
				{
					return;
				}
				int mask = (cache.Length - 1);
				int removed = 0;
				for (int i = begin; i < begin + count; i++)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?> e = cache[i & mask];
					Entry<?> e = cache[i & mask];
					if (e == java.util.Map_Fields.Null || e.Live)
					{
						continue; // skip null and live entries
					}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?> replacement = java.util.Map_Fields.null;
					Entry<?> replacement = java.util.Map_Fields.Null;
					if (PROBE_LIMIT > 1)
					{
						// avoid breaking up a non-null run
						replacement = FindReplacement(cache, i);
					}
					cache[i & mask] = replacement;
					if (replacement == java.util.Map_Fields.Null)
					{
						removed += 1;
					}
				}
				CacheLoad = System.Math.Max(0, CacheLoad - removed);
			}

			/// <summary>
			/// Clearing a cache slot risks disconnecting following entries
			///  from the head of a non-null run, which would allow them
			///  to be found via reprobes.  Find an entry after cache[begin]
			///  to plug into the hole, or return null if none is needed.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Entry<?> findReplacement(Entry<?>[] cache, int home1)
			internal virtual Entry<?> FindReplacement(Entry<T1>[] cache, int home1)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?> replacement = java.util.Map_Fields.null;
				Entry<?> replacement = java.util.Map_Fields.Null;
				int haveReplacement = -1, replacementPos = 0;
				int mask = (cache.Length - 1);
				for (int i2 = home1 + 1; i2 < home1 + PROBE_LIMIT; i2++)
				{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?> e2 = cache[i2 & mask];
					Entry<?> e2 = cache[i2 & mask];
					if (e2 == java.util.Map_Fields.Null) // End of non-null run.
					{
						break;
					}
					if (!e2.Live) // Doomed anyway.
					{
						continue;
					}
					int dis2 = EntryDislocation(cache, i2, e2);
					if (dis2 == 0) // e2 already optimally placed
					{
						continue;
					}
					int home2 = i2 - dis2;
					if (home2 <= home1)
					{
						// e2 can replace entry at cache[home1]
						if (home2 == home1)
						{
							// Put e2 exactly where he belongs.
							haveReplacement = 1;
							replacementPos = i2;
							replacement = e2;
						}
						else if (haveReplacement <= 0)
						{
							haveReplacement = 0;
							replacementPos = i2;
							replacement = e2;
						}
						// And keep going, so we can favor larger dislocations.
					}
				}
				if (haveReplacement >= 0)
				{
					if (cache[(replacementPos + 1) & mask] != java.util.Map_Fields.Null)
					{
						// Be conservative, to avoid breaking up a non-null run.
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: cache[replacementPos & mask] = (Entry<?>) Entry.DEAD_ENTRY;
						cache[replacementPos & mask] = (Entry<?>) Entry.DEAD_ENTRY;
					}
					else
					{
						cache[replacementPos & mask] = java.util.Map_Fields.Null;
						CacheLoad -= 1;
					}
				}
				return replacement;
			}

			/// <summary>
			/// Remove stale entries in the range near classValue. </summary>
			internal virtual void removeStaleEntries<T1>(ClassValue<T1> classValue)
			{
				RemoveStaleEntries(Cache, classValue.HashCodeForCache, PROBE_LIMIT);
			}

			/// <summary>
			/// Remove all stale entries, everywhere. </summary>
			internal virtual void RemoveStaleEntries()
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?>[] cache = getCache();
				Entry<?>[] cache = Cache;
				RemoveStaleEntries(cache, 0, cache.Length + PROBE_LIMIT - 1);
			}

			/// <summary>
			/// Add the given entry to the cache, in its home location, unless it is out of date. </summary>
			internal virtual void addToCache<T>(Entry<T> e)
			{
				ClassValue<T> classValue = e.ClassValueOrNull();
				if (classValue != java.util.Map_Fields.Null)
				{
					AddToCache(classValue, e);
				}
			}

			/// <summary>
			/// Add the given entry to the cache, in its home location. </summary>
			internal virtual void addToCache<T>(ClassValue<T> classValue, Entry<T> e)
			{
				if (PROBE_LIMIT <= 0) // do not fill cache
				{
					return;
				}
				// Add e to the cache.
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?>[] cache = getCache();
				Entry<?>[] cache = Cache;
				int mask = (cache.Length - 1);
				int home = classValue.HashCodeForCache & mask;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?> e2 = placeInCache(cache, home, e, java.util.Map_Fields.false);
				Entry<?> e2 = PlaceInCache(cache, home, e, java.util.Map_Fields.False);
				if (e2 == java.util.Map_Fields.Null) // done
				{
					return;
				}
				if (PROBE_LIMIT > 1)
				{
					// try to move e2 somewhere else in his probe range
					int dis2 = EntryDislocation(cache, home, e2);
					int home2 = home - dis2;
					for (int i2 = home2; i2 < home2 + PROBE_LIMIT; i2++)
					{
						if (PlaceInCache(cache, i2 & mask, e2, java.util.Map_Fields.True) == java.util.Map_Fields.Null)
						{
							return;
						}
					}
				}
				// Note:  At this point, e2 is just dropped from the cache.
			}

			/// <summary>
			/// Store the given entry.  Update cacheLoad, and return any live victim.
			///  'Gently' means return self rather than dislocating a live victim.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private Entry<?> placeInCache(Entry<?>[] cache, int pos, Entry<?> e, boolean gently)
			internal virtual Entry<?> PlaceInCache(Entry<T1>[] cache, int pos, Entry<T2> e, bool gently)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Entry<?> e2 = overwrittenEntry(cache[pos]);
				Entry<?> e2 = OverwrittenEntry(cache[pos]);
				if (gently && e2 != java.util.Map_Fields.Null)
				{
					// do not overwrite a live entry
					return e;
				}
				else
				{
					cache[pos] = e;
					return e2;
				}
			}

			/// <summary>
			/// Note an entry that is about to be overwritten.
			///  If it is not live, quietly replace it by null.
			///  If it is an actual null, increment cacheLoad,
			///  because the caller is going to store something
			///  in its place.
			/// </summary>
			internal virtual Entry<T> overwrittenEntry<T>(Entry<T> e2)
			{
				if (e2 == java.util.Map_Fields.Null)
				{
					CacheLoad += 1;
				}
				else if (e2.Live)
				{
					return e2;
				}
				return java.util.Map_Fields.Null;
			}

			/// <summary>
			/// Percent loading of cache before resize. </summary>
			internal const int CACHE_LOAD_LIMIT = 67; // 0..100
			/// <summary>
			/// Maximum number of probes to attempt. </summary>
			internal const int PROBE_LIMIT = 6; // 1..
			// N.B.  Set PROBE_LIMIT=0 to disable all fast paths.
		}
	}

}