using System.Diagnostics;
using System.Collections.Concurrent;

/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
namespace java.lang.reflect
{


	/// <summary>
	/// Cache mapping pairs of {@code (key, sub-key) -> value}. Keys and values are
	/// weakly but sub-keys are strongly referenced.  Keys are passed directly to
	/// <seealso cref="#get"/> method which also takes a {@code parameter}. Sub-keys are
	/// calculated from keys and parameters using the {@code subKeyFactory} function
	/// passed to the constructor. Values are calculated from keys and parameters
	/// using the {@code valueFactory} function passed to the constructor.
	/// Keys can be {@code null} and are compared by identity while sub-keys returned by
	/// {@code subKeyFactory} or values returned by {@code valueFactory}
	/// can not be null. Sub-keys are compared using their <seealso cref="#equals"/> method.
	/// Entries are expunged from cache lazily on each invocation to <seealso cref="#get"/>,
	/// <seealso cref="#containsValue"/> or <seealso cref="#size"/> methods when the WeakReferences to
	/// keys are cleared. Cleared WeakReferences to individual values don't cause
	/// expunging, but such entries are logically treated as non-existent and
	/// trigger re-evaluation of {@code valueFactory} on request for their
	/// key/subKey.
	/// 
	/// @author Peter Levart </summary>
	/// @param <K> type of keys </param>
	/// @param <P> type of parameters </param>
	/// @param <V> type of values </param>
	internal sealed class WeakCache<K, P, V>
	{

		private readonly ReferenceQueue<K> RefQueue = new ReferenceQueue<K>();
		// the key type is Object for supporting null key
		private readonly ConcurrentMap<Object, ConcurrentMap<Object, Supplier<V>>> Map = new ConcurrentDictionary<Object, ConcurrentMap<Object, Supplier<V>>>();
		private readonly ConcurrentMap<Supplier<V>, Boolean> ReverseMap = new ConcurrentDictionary<Supplier<V>, Boolean>();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final java.util.function.BiFunction<K, P, ?> subKeyFactory;
		private readonly BiFunction<K, P, ?> SubKeyFactory;
		private readonly BiFunction<K, P, V> ValueFactory;

		/// <summary>
		/// Construct an instance of {@code WeakCache}
		/// </summary>
		/// <param name="subKeyFactory"> a function mapping a pair of
		///                      {@code (key, parameter) -> sub-key} </param>
		/// <param name="valueFactory">  a function mapping a pair of
		///                      {@code (key, parameter) -> value} </param>
		/// <exception cref="NullPointerException"> if {@code subKeyFactory} or
		///                              {@code valueFactory} is null. </exception>
		public WeakCache<T1>(BiFunction<T1> subKeyFactory, BiFunction<K, P, V> valueFactory)
		{
			this.SubKeyFactory = Objects.RequireNonNull(subKeyFactory);
			this.ValueFactory = Objects.RequireNonNull(valueFactory);
		}

		/// <summary>
		/// Look-up the value through the cache. This always evaluates the
		/// {@code subKeyFactory} function and optionally evaluates
		/// {@code valueFactory} function if there is no entry in the cache for given
		/// pair of (key, subKey) or the entry has already been cleared.
		/// </summary>
		/// <param name="key">       possibly null key </param>
		/// <param name="parameter"> parameter used together with key to create sub-key and
		///                  value (should not be null) </param>
		/// <returns> the cached value (never null) </returns>
		/// <exception cref="NullPointerException"> if {@code parameter} passed in or
		///                              {@code sub-key} calculated by
		///                              {@code subKeyFactory} or {@code value}
		///                              calculated by {@code valueFactory} is null. </exception>
		public V Get(K key, P parameter)
		{
			Objects.RequireNonNull(parameter);

			ExpungeStaleEntries();

			Object cacheKey = CacheKey.ValueOf(key, RefQueue);

			// lazily install the 2nd level valuesMap for the particular cacheKey
			ConcurrentMap<Object, Supplier<V>> valuesMap = Map[cacheKey];
			if (valuesMap == null)
			{
				ConcurrentMap<Object, Supplier<V>> oldValuesMap = Map.PutIfAbsent(cacheKey, valuesMap = new ConcurrentDictionary<Object, Supplier<V>>());
				if (oldValuesMap != null)
				{
					valuesMap = oldValuesMap;
				}
			}

			// create subKey and retrieve the possible Supplier<V> stored by that
			// subKey from valuesMap
			Object subKey = Objects.RequireNonNull(SubKeyFactory.Apply(key, parameter));
			Supplier<V> supplier = valuesMap[subKey];
			Factory factory = null;

			while (true)
			{
				if (supplier != null)
				{
					// supplier might be a Factory or a CacheValue<V> instance
					V value = supplier.Get();
					if (value != null)
					{
						return value;
					}
				}
				// else no supplier in cache
				// or a supplier that returned null (could be a cleared CacheValue
				// or a Factory that wasn't successful in installing the CacheValue)

				// lazily construct a Factory
				if (factory == null)
				{
					factory = new Factory(this, key, parameter, subKey, valuesMap);
				}

				if (supplier == null)
				{
					supplier = valuesMap.PutIfAbsent(subKey, factory);
					if (supplier == null)
					{
						// successfully installed Factory
						supplier = factory;
					}
					// else retry with winning supplier
				}
				else
				{
					if (valuesMap.Replace(subKey, supplier, factory))
					{
						// successfully replaced
						// cleared CacheEntry / unsuccessful Factory
						// with our Factory
						supplier = factory;
					}
					else
					{
						// retry with current supplier
						supplier = valuesMap[subKey];
					}
				}
			}
		}

		/// <summary>
		/// Checks whether the specified non-null value is already present in this
		/// {@code WeakCache}. The check is made using identity comparison regardless
		/// of whether value's class overrides <seealso cref="Object#equals"/> or not.
		/// </summary>
		/// <param name="value"> the non-null value to check </param>
		/// <returns> true if given {@code value} is already cached </returns>
		/// <exception cref="NullPointerException"> if value is null </exception>
		public bool ContainsValue(V value)
		{
			Objects.RequireNonNull(value);

			ExpungeStaleEntries();
			return ReverseMap.ContainsKey(new LookupValue<>(value));
		}

		/// <summary>
		/// Returns the current number of cached entries that
		/// can decrease over time when keys/values are GC-ed.
		/// </summary>
		public int Size()
		{
			ExpungeStaleEntries();
			return ReverseMap.Count;
		}

		private void ExpungeStaleEntries()
		{
			CacheKey<K> cacheKey;
			while ((cacheKey = (CacheKey<K>)RefQueue.poll()) != null)
			{
				cacheKey.ExpungeFrom(Map, ReverseMap);
			}
		}

		/// <summary>
		/// A factory <seealso cref="Supplier"/> that implements the lazy synchronized
		/// construction of the value and installment of it into the cache.
		/// </summary>
		private sealed class Factory : Supplier<V>
		{
			private readonly WeakCache<K, P, V> OuterInstance;


			internal readonly K Key;
			internal readonly P Parameter;
			internal readonly Object SubKey;
			internal readonly ConcurrentMap<Object, Supplier<V>> ValuesMap;

			internal Factory(WeakCache<K, P, V> outerInstance, K key, P parameter, Object subKey, ConcurrentMap<Object, Supplier<V>> valuesMap)
			{
				this.OuterInstance = outerInstance;
				this.Key = key;
				this.Parameter = parameter;
				this.SubKey = subKey;
				this.ValuesMap = valuesMap;
			}

			public override V Get() // serialize access
			{
				lock (this)
				{
					// re-check
					Supplier<V> supplier = ValuesMap[SubKey];
					if (supplier != this)
					{
						// something changed while we were waiting:
						// might be that we were replaced by a CacheValue
						// or were removed because of failure ->
						// return null to signal WeakCache.get() to retry
						// the loop
						return null;
					}
					// else still us (supplier == this)
        
					// create new value
					V value = null;
					try
					{
						value = Objects.RequireNonNull(outerInstance.ValueFactory.Apply(Key, Parameter));
					}
					finally
					{
						if (value == null) // remove us on failure
						{
							ValuesMap.Remove(SubKey, this);
						}
					}
					// the only path to reach here is with non-null value
					Debug.Assert(value != null);
        
					// wrap value with CacheValue (WeakReference)
					CacheValue<V> cacheValue = new CacheValue<V>(value);
        
					// try replacing us with CacheValue (this should always succeed)
					if (ValuesMap.Replace(SubKey, this, cacheValue))
					{
						// put also in reverseMap
						outerInstance.ReverseMap[cacheValue] = true;
					}
					else
					{
						throw new AssertionError("Should not reach here");
					}
        
					// successfully replaced us with new CacheValue -> return the value
					// wrapped by it
					return value;
				}
			}
		}

		/// <summary>
		/// Common type of value suppliers that are holding a referent.
		/// The <seealso cref="#equals"/> and <seealso cref="#hashCode"/> of implementations is defined
		/// to compare the referent by identity.
		/// </summary>
		private interface Value<V> : Supplier<V>
		{
		}

		/// <summary>
		/// An optimized <seealso cref="Value"/> used to look-up the value in
		/// <seealso cref="WeakCache#containsValue"/> method so that we are not
		/// constructing the whole <seealso cref="CacheValue"/> just to look-up the referent.
		/// </summary>
		private sealed class LookupValue<V> : Value<V>
		{
			internal readonly V Value;

			internal LookupValue(V value)
			{
				this.Value = value;
			}

			public override V Get()
			{
				return Value;
			}

			public override int HashCode()
			{
				return System.identityHashCode(Value); // compare by identity
			}

			public override bool Equals(Object obj)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return obj == this || obj instanceof Value && this.value == ((Value<?>) obj).get();
				return obj == this || obj is Value && this.Value == ((Value<?>) obj).Get(); // compare by identity
			}
		}

		/// <summary>
		/// A <seealso cref="Value"/> that weakly references the referent.
		/// </summary>
		private sealed class CacheValue<V> : WeakReference<V>, Value<V>
		{
			internal readonly int Hash;

			internal CacheValue(V value) : base(value)
			{
				this.Hash = System.identityHashCode(value); // compare by identity
			}

			public override int HashCode()
			{
				return Hash;
			}

			public override bool Equals(Object obj)
			{
				V value;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return obj == this || obj instanceof Value && (value = get()) != null && value == ((Value<?>) obj).get();
				return obj == this || obj is Value && (value = get()) != null && value == ((Value<?>) obj).Get(); // compare by identity
					   // cleared CacheValue is only equal to itself
			}
		}

		/// <summary>
		/// CacheKey containing a weakly referenced {@code key}. It registers
		/// itself with the {@code refQueue} so that it can be used to expunge
		/// the entry when the <seealso cref="WeakReference"/> is cleared.
		/// </summary>
		private sealed class CacheKey<K> : WeakReference<K>
		{

			// a replacement for null keys
			internal static readonly Object NULL_KEY = new Object();

			internal static Object valueOf<K>(K key, ReferenceQueue<K> refQueue)
			{
				return key == null ? NULL_KEY : new CacheKey<>(key, refQueue);
					   // null key means we can't weakly reference it,
					   // so we use a NULL_KEY singleton as cache key
					   // non-null key requires wrapping with a WeakReference
			}

			internal readonly int Hash;

			internal CacheKey(K key, ReferenceQueue<K> refQueue) : base(key, refQueue)
			{
				this.Hash = System.identityHashCode(key); // compare by identity
			}

			public override int HashCode()
			{
				return Hash;
			}

			public override bool Equals(Object obj)
			{
				K key;
				return obj == this || obj != null && obj.GetType() == this.GetType() && (key = this.get()) != null && key == ((CacheKey<K>) obj).get();
					   // cleared CacheKey is only equal to itself
					   // compare key by identity
			}

			internal void expungeFrom<T1, T2>(ConcurrentMap<T1> map, ConcurrentMap<T2> reverseMap) where T1 : java.util.concurrent.ConcurrentMap<T1, T1>
			{
				// removing just by key is always safe here because after a CacheKey
				// is cleared and enqueue-ed it is only equal to itself
				// (see equals method)...
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.concurrent.ConcurrentMap<?, ?> valuesMap = map.remove(this);
				ConcurrentMap<?, ?> valuesMap = map.Remove(this);
				// remove also from reverseMap if needed
				if (valuesMap != null)
				{
					foreach (Object cacheValue in valuesMap.Values)
					{
						reverseMap.Remove(cacheValue);
					}
				}
			}
		}
	}

}