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
	/// A <seealso cref="ConcurrentMap"/> supporting <seealso cref="NavigableMap"/> operations,
	/// and recursively so for its navigable sub-maps.
	/// 
	/// <para>This interface is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <K> the type of keys maintained by this map </param>
	/// @param <V> the type of mapped values
	/// @since 1.6 </param>
	public interface ConcurrentNavigableMap<K, V> : ConcurrentMap<K, V>, NavigableMap<K, V>
	{
		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		ConcurrentNavigableMap<K, V> SubMap(K fromKey, bool fromInclusive, K toKey, bool toInclusive);

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		ConcurrentNavigableMap<K, V> HeadMap(K toKey, bool inclusive);

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		ConcurrentNavigableMap<K, V> TailMap(K fromKey, bool inclusive);

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		ConcurrentNavigableMap<K, V> SubMap(K fromKey, K toKey);

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		ConcurrentNavigableMap<K, V> HeadMap(K toKey);

		/// <exception cref="ClassCastException">       {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">     {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		ConcurrentNavigableMap<K, V> TailMap(K fromKey);

		/// <summary>
		/// Returns a reverse order view of the mappings contained in this map.
		/// The descending map is backed by this map, so changes to the map are
		/// reflected in the descending map, and vice-versa.
		/// 
		/// <para>The returned map has an ordering equivalent to
		/// <seealso cref="Collections#reverseOrder(Comparator) Collections.reverseOrder"/>{@code (comparator())}.
		/// The expression {@code m.descendingMap().descendingMap()} returns a
		/// view of {@code m} essentially equivalent to {@code m}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a reverse order view of this map </returns>
		ConcurrentNavigableMap<K, V> DescendingMap();

		/// <summary>
		/// Returns a <seealso cref="NavigableSet"/> view of the keys contained in this map.
		/// The set's iterator returns the keys in ascending order.
		/// The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  The set supports element
		/// removal, which removes the corresponding mapping from the map,
		/// via the {@code Iterator.remove}, {@code Set.remove},
		/// {@code removeAll}, {@code retainAll}, and {@code clear}
		/// operations.  It does not support the {@code add} or {@code addAll}
		/// operations.
		/// 
		/// <para>The view's iterators and spliterators are
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a navigable set view of the keys in this map </returns>
		NavigableSet<K> NavigableKeySet();

		/// <summary>
		/// Returns a <seealso cref="NavigableSet"/> view of the keys contained in this map.
		/// The set's iterator returns the keys in ascending order.
		/// The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  The set supports element
		/// removal, which removes the corresponding mapping from the map,
		/// via the {@code Iterator.remove}, {@code Set.remove},
		/// {@code removeAll}, {@code retainAll}, and {@code clear}
		/// operations.  It does not support the {@code add} or {@code addAll}
		/// operations.
		/// 
		/// <para>The view's iterators and spliterators are
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// <para>This method is equivalent to method {@code navigableKeySet}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a navigable set view of the keys in this map </returns>
		NavigableSet<K> KeySet();

		/// <summary>
		/// Returns a reverse order <seealso cref="NavigableSet"/> view of the keys contained in this map.
		/// The set's iterator returns the keys in descending order.
		/// The set is backed by the map, so changes to the map are
		/// reflected in the set, and vice-versa.  The set supports element
		/// removal, which removes the corresponding mapping from the map,
		/// via the {@code Iterator.remove}, {@code Set.remove},
		/// {@code removeAll}, {@code retainAll}, and {@code clear}
		/// operations.  It does not support the {@code add} or {@code addAll}
		/// operations.
		/// 
		/// <para>The view's iterators and spliterators are
		/// <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a reverse order navigable set view of the keys in this map </returns>
		NavigableSet<K> DescendingKeySet();
	}

}