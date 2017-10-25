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
	/// A <seealso cref="java.util.Map"/> providing thread safety and atomicity
	/// guarantees.
	/// 
	/// <para>Memory consistency effects: As with other concurrent
	/// collections, actions in a thread prior to placing an object into a
	/// {@code ConcurrentMap} as a key or value
	/// <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	/// actions subsequent to the access or removal of that object from
	/// the {@code ConcurrentMap} in another thread.
	/// 
	/// </para>
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <K> the type of keys maintained by this map </param>
	/// @param <V> the type of mapped values </param>
	public interface ConcurrentMap<K, V> : Map<K, V>
	{

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implNote This implementation assumes that the ConcurrentMap cannot
		/// contain null values and {@code get()} returning null unambiguously means
		/// the key is absent. Implementations which support null values
		/// <strong>must</strong> override this default implementation.
		/// </summary>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default V getOrDefault(Object key, V defaultValue)
	//	{
	//		return ((v = get(key)) != null) ? v : defaultValue;
	//	}

	   /// <summary>
	   /// {@inheritDoc}
	   ///  
	   /// @implSpec The default implementation is equivalent to, for this
	   /// {@code map}:
	   /// <pre> {@code
	   /// for ((Map.Entry<K, V> entry : map.entrySet())
	   ///     action.accept(entry.getKey(), entry.getValue());
	   /// }</pre>
	   ///  
	   /// @implNote The default implementation assumes that
	   /// {@code IllegalStateException} thrown by {@code getKey()} or
	   /// {@code getValue()} indicates that the entry has been removed and cannot
	   /// be processed. Operation continues for subsequent entries.
	   /// </summary>
	   /// <exception cref="NullPointerException"> {@inheritDoc}
	   /// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default void forEach(java.util.function.BiConsumer<? base K, ? base V> action)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//		default void forEach(java.util.function.BiConsumer<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> action)
	//	{
	//		Objects.requireNonNull(action);
	//		for (Map.Entry<K, V> entry : entrySet())
	//		{
	//			try
	//			{
	//				k = entry.getKey();
	//				v = entry.getValue();
	//			}
	//			catch(IllegalStateException ise)
	//			{
	//				// this usually means the entry is no longer in the map.
	//			}
	//			action.accept(k, v);
	//		}
	//	}

		/// <summary>
		/// If the specified key is not already associated
		/// with a value, associate it with the given value.
		/// This is equivalent to
		///  <pre> {@code
		/// if (!map.containsKey(key))
		///   return map.put(key, value);
		/// else
		///   return map.get(key);
		/// }</pre>
		/// 
		/// except that the action is performed atomically.
		/// 
		/// @implNote This implementation intentionally re-abstracts the
		/// inappropriate default provided in {@code Map}.
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated </param>
		/// <param name="value"> value to be associated with the specified key </param>
		/// <returns> the previous value associated with the specified key, or
		///         {@code null} if there was no mapping for the key.
		///         (A {@code null} return can also indicate that the map
		///         previously associated {@code null} with the key,
		///         if the implementation supports null values.) </returns>
		/// <exception cref="UnsupportedOperationException"> if the {@code put} operation
		///         is not supported by this map </exception>
		/// <exception cref="ClassCastException"> if the class of the specified key or value
		///         prevents it from being stored in this map </exception>
		/// <exception cref="NullPointerException"> if the specified key or value is null,
		///         and this map does not permit null keys or values </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified key
		///         or value prevents it from being stored in this map </exception>
		 V PutIfAbsent(K key, V ConcurrentMap_Fields);

		/// <summary>
		/// Removes the entry for a key only if currently mapped to a given value.
		/// This is equivalent to
		///  <pre> {@code
		/// if (map.containsKey(key) && Objects.equals(map.get(key), value)) {
		///   map.remove(key);
		///   return true;
		/// } else
		///   return false;
		/// }</pre>
		/// 
		/// except that the action is performed atomically.
		/// 
		/// @implNote This implementation intentionally re-abstracts the
		/// inappropriate default provided in {@code Map}.
		/// </summary>
		/// <param name="key"> key with which the specified value is associated </param>
		/// <param name="value"> value expected to be associated with the specified key </param>
		/// <returns> {@code true} if the value was removed </returns>
		/// <exception cref="UnsupportedOperationException"> if the {@code remove} operation
		///         is not supported by this map </exception>
		/// <exception cref="ClassCastException"> if the key or value is of an inappropriate
		///         type for this map
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if the specified key or value is null,
		///         and this map does not permit null keys or values
		///         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		bool Remove(Object key, Object ConcurrentMap_Fields);

		/// <summary>
		/// Replaces the entry for a key only if currently mapped to a given value.
		/// This is equivalent to
		///  <pre> {@code
		/// if (map.containsKey(key) && Objects.equals(map.get(key), oldValue)) {
		///   map.put(key, newValue);
		///   return true;
		/// } else
		///   return false;
		/// }</pre>
		/// 
		/// except that the action is performed atomically.
		/// 
		/// @implNote This implementation intentionally re-abstracts the
		/// inappropriate default provided in {@code Map}.
		/// </summary>
		/// <param name="key"> key with which the specified value is associated </param>
		/// <param name="oldValue"> value expected to be associated with the specified key </param>
		/// <param name="newValue"> value to be associated with the specified key </param>
		/// <returns> {@code true} if the value was replaced </returns>
		/// <exception cref="UnsupportedOperationException"> if the {@code put} operation
		///         is not supported by this map </exception>
		/// <exception cref="ClassCastException"> if the class of a specified key or value
		///         prevents it from being stored in this map </exception>
		/// <exception cref="NullPointerException"> if a specified key or value is null,
		///         and this map does not permit null keys or values </exception>
		/// <exception cref="IllegalArgumentException"> if some property of a specified key
		///         or value prevents it from being stored in this map </exception>
		bool Replace(K key, V ConcurrentMap_Fields, V ConcurrentMap_Fields);

		/// <summary>
		/// Replaces the entry for a key only if currently mapped to some value.
		/// This is equivalent to
		///  <pre> {@code
		/// if (map.containsKey(key)) {
		///   return map.put(key, value);
		/// } else
		///   return null;
		/// }</pre>
		/// 
		/// except that the action is performed atomically.
		/// 
		/// @implNote This implementation intentionally re-abstracts the
		/// inappropriate default provided in {@code Map}.
		/// </summary>
		/// <param name="key"> key with which the specified value is associated </param>
		/// <param name="value"> value to be associated with the specified key </param>
		/// <returns> the previous value associated with the specified key, or
		///         {@code null} if there was no mapping for the key.
		///         (A {@code null} return can also indicate that the map
		///         previously associated {@code null} with the key,
		///         if the implementation supports null values.) </returns>
		/// <exception cref="UnsupportedOperationException"> if the {@code put} operation
		///         is not supported by this map </exception>
		/// <exception cref="ClassCastException"> if the class of the specified key or value
		///         prevents it from being stored in this map </exception>
		/// <exception cref="NullPointerException"> if the specified key or value is null,
		///         and this map does not permit null keys or values </exception>
		/// <exception cref="IllegalArgumentException"> if some property of the specified key
		///         or value prevents it from being stored in this map </exception>
		V Replace(K key, V ConcurrentMap_Fields);

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// <para>The default implementation is equivalent to, for this {@code map}:
		/// <pre> {@code
		/// for ((Map.Entry<K, V> entry : map.entrySet())
		///     do {
		///        K k = entry.getKey();
		///        V v = entry.getValue();
		///     } while(!replace(k, v, function.apply(k, v)));
		/// }</pre>
		/// 
		/// The default implementation may retry these steps when multiple
		/// threads attempt updates including potentially calling the function
		/// repeatedly for a given key.
		/// 
		/// </para>
		/// <para>This implementation assumes that the ConcurrentMap cannot contain null
		/// values and {@code get()} returning null unambiguously means the key is
		/// absent. Implementations which support null values <strong>must</strong>
		/// override this default implementation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc}
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default void replaceAll(java.util.function.BiFunction<? base K, ? base V, ? extends V> function)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default void replaceAll(java.util.function.BiFunction<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> function) where ? : V
	//	{
	//		Objects.requireNonNull(function);
	//		forEach((k,v) ->
	//				// v changed or k is gone
	//					// k is no longer in the map.
	//		{
	//			while(!replace(k, v, function.apply(k, v)))
	//			{
	//				if ((v = get(k)) == null)
	//				{
	//				}
	//			}
	//		});
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// The default implementation is equivalent to the following steps for this
		/// {@code map}, then returning the current value or {@code null} if now
		/// absent:
		/// 
		/// <pre> {@code
		/// if (map.get(key) == null) {
		///     V newValue = mappingFunction.apply(key);
		///     if (newValue != null)
		///         return map.putIfAbsent(key, newValue);
		/// }
		/// }</pre>
		/// 
		/// The default implementation may retry these steps when multiple
		/// threads attempt updates including potentially calling the mapping
		/// function multiple times.
		/// 
		/// <para>This implementation assumes that the ConcurrentMap cannot contain null
		/// values and {@code get()} returning null unambiguously means the key is
		/// absent. Implementations which support null values <strong>must</strong>
		/// override this default implementation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default V computeIfAbsent(K key, java.util.function.Function<? base K, ? extends V> mappingFunction)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default V computeIfAbsent(K key, java.util.function.Function<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> mappingFunction) where ? : V
	//	{
	//		Objects.requireNonNull(mappingFunction);
	//		return ((v = get(key)) == null && (newValue = mappingFunction.apply(key)) != null && (v = putIfAbsent(key, newValue)) == null) ? newValue : v;
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// The default implementation is equivalent to performing the following
		/// steps for this {@code map}, then returning the current value or
		/// {@code null} if now absent. :
		/// 
		/// <pre> {@code
		/// if (map.get(key) != null) {
		///     V oldValue = map.get(key);
		///     V newValue = remappingFunction.apply(key, oldValue);
		///     if (newValue != null)
		///         map.replace(key, oldValue, newValue);
		///     else
		///         map.remove(key, oldValue);
		/// }
		/// }</pre>
		/// 
		/// The default implementation may retry these steps when multiple threads
		/// attempt updates including potentially calling the remapping function
		/// multiple times.
		/// 
		/// <para>This implementation assumes that the ConcurrentMap cannot contain null
		/// values and {@code get()} returning null unambiguously means the key is
		/// absent. Implementations which support null values <strong>must</strong>
		/// override this default implementation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default V computeIfPresent(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default V computeIfPresent(K key, java.util.function.BiFunction<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> remappingFunction) where ? : V
	//	{
	//		Objects.requireNonNull(remappingFunction);
	//		while((oldValue = get(key)) != null)
	//		{
	//			if (newValue != null)
	//			{
	//				if (replace(key, oldValue, newValue))
	//			}
	//			else if (remove(key, oldValue))
	//		}
	//	}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// The default implementation is equivalent to performing the following
		/// steps for this {@code map}, then returning the current value or
		/// {@code null} if absent:
		/// 
		/// <pre> {@code
		/// V oldValue = map.get(key);
		/// V newValue = remappingFunction.apply(key, oldValue);
		/// if (oldValue != null ) {
		///    if (newValue != null)
		///       map.replace(key, oldValue, newValue);
		///    else
		///       map.remove(key, oldValue);
		/// } else {
		///    if (newValue != null)
		///       map.putIfAbsent(key, newValue);
		///    else
		///       return null;
		/// }
		/// }</pre>
		/// 
		/// The default implementation may retry these steps when multiple
		/// threads attempt updates including potentially calling the remapping
		/// function multiple times.
		/// 
		/// <para>This implementation assumes that the ConcurrentMap cannot contain null
		/// values and {@code get()} returning null unambiguously means the key is
		/// absent. Implementations which support null values <strong>must</strong>
		/// override this default implementation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default V compute(K key, java.util.function.BiFunction<? base K, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default V compute(K key, java.util.function.BiFunction<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> remappingFunction) where ? : V
	//	{
	//		Objects.requireNonNull(remappingFunction);
	//		for(;;)
	//		{
	//			if (newValue == null)
	//			{
	//				// delete mapping
	//				if (oldValue != null || containsKey(key))
	//				{
	//					// something to remove
	//					if (remove(key, oldValue))
	//					{
	//						// removed the old value as expected
	//					}
	//
	//					// some other value replaced old value. try again.
	//					oldValue = get(key);
	//				}
	//				else
	//				{
	//					// nothing to do. Leave things as they were.
	//				}
	//			}
	//			else
	//			{
	//				// add or replace old mapping
	//				if (oldValue != null)
	//				{
	//					// replace
	//					if (replace(key, oldValue, newValue))
	//					{
	//						// replaced as expected.
	//					}
	//
	//					// some other value replaced old value. try again.
	//					oldValue = get(key);
	//				}
	//				else
	//				{
	//					// add (replace if oldValue was null)
	//					if ((oldValue = putIfAbsent(key, newValue)) == null)
	//					{
	//						// replaced
	//					}
	//
	//					// some other value replaced old value. try again.
	//				}
	//			}
	//		}
	//	}


		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// The default implementation is equivalent to performing the following
		/// steps for this {@code map}, then returning the current value or
		/// {@code null} if absent:
		/// 
		/// <pre> {@code
		/// V oldValue = map.get(key);
		/// V newValue = (oldValue == null) ? value :
		///              remappingFunction.apply(oldValue, value);
		/// if (newValue == null)
		///     map.remove(key);
		/// else
		///     map.put(key, newValue);
		/// }</pre>
		/// 
		/// <para>The default implementation may retry these steps when multiple
		/// threads attempt updates including potentially calling the remapping
		/// function multiple times.
		/// 
		/// </para>
		/// <para>This implementation assumes that the ConcurrentMap cannot contain null
		/// values and {@code get()} returning null unambiguously means the key is
		/// absent. Implementations which support null values <strong>must</strong>
		/// override this default implementation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException"> {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc}
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default V merge(K key, V ConcurrentMap_Fields.value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override default V merge(K key, V ConcurrentMap_Fields.value, java.util.function.BiFunction<? base V, ? base V, ? extends V> remappingFunction)
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//		default V merge(K key, V ConcurrentMap_Fields.value, java.util.function.BiFunction<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard> remappingFunction) where ? : V
	//	{
	//		Objects.requireNonNull(remappingFunction);
	//		Objects.requireNonNull(value);
	//		for (;;)
	//		{
	//			if (oldValue != null)
	//			{
	//				if (newValue != null)
	//				{
	//					if (replace(key, oldValue, newValue))
	//				}
	//				else if (remove(key, oldValue))
	//				{
	//				}
	//				oldValue = get(key);
	//			}
	//			else
	//			{
	//				if ((oldValue = putIfAbsent(key, value)) == null)
	//				{
	//				}
	//			}
	//		}
	//	}
	}

	public static class ConcurrentMap_Fields
	{
			public static readonly V v;
				public static readonly K k;
				public static readonly V v;
					public static readonly continue;
						public static readonly break;
			public static readonly V v, NewValue;
			public static readonly V OldValue;
				public static readonly V NewValue = remappingFunction.apply(key, OldValue);
						public static readonly return NewValue;
				   public static readonly return Null;
			public static readonly return OldValue;
			public static readonly V OldValue = get(key);
				public static readonly V NewValue = remappingFunction.apply(key, OldValue);
							public static readonly return Null;
						public static readonly return Null;
							public static readonly return NewValue;
							public static readonly return NewValue;
			public static readonly V OldValue = get(key);
					public static readonly V NewValue = remappingFunction.apply(OldValue, Value);
							public static readonly return NewValue;
						public static readonly return Null;
						public static readonly return Value;
	}

}