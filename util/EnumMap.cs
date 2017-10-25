using System;
using System.Collections;

/*
 * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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

	using SharedSecrets = sun.misc.SharedSecrets;

	/// <summary>
	/// A specialized <seealso cref="Map"/> implementation for use with enum type keys.  All
	/// of the keys in an enum map must come from a single enum type that is
	/// specified, explicitly or implicitly, when the map is created.  Enum maps
	/// are represented internally as arrays.  This representation is extremely
	/// compact and efficient.
	/// 
	/// <para>Enum maps are maintained in the <i>natural order</i> of their keys
	/// (the order in which the enum constants are declared).  This is reflected
	/// in the iterators returned by the collections views (<seealso cref="#keySet()"/>,
	/// <seealso cref="#entrySet()"/>, and <seealso cref="#values()"/>).
	/// 
	/// </para>
	/// <para>Iterators returned by the collection views are <i>weakly consistent</i>:
	/// they will never throw <seealso cref="ConcurrentModificationException"/> and they may
	/// or may not show the effects of any modifications to the map that occur while
	/// the iteration is in progress.
	/// 
	/// </para>
	/// <para>Null keys are not permitted.  Attempts to insert a null key will
	/// throw <seealso cref="NullPointerException"/>.  Attempts to test for the
	/// presence of a null key or to remove one will, however, function properly.
	/// Null values are permitted.
	/// 
	/// <P>Like most collection implementations <tt>EnumMap</tt> is not
	/// synchronized. If multiple threads access an enum map concurrently, and at
	/// least one of the threads modifies the map, it should be synchronized
	/// externally.  This is typically accomplished by synchronizing on some
	/// object that naturally encapsulates the enum map.  If no such object exists,
	/// the map should be "wrapped" using the <seealso cref="Collections#synchronizedMap"/>
	/// method.  This is best done at creation time, to prevent accidental
	/// unsynchronized access:
	/// 
	/// <pre>
	///     Map&lt;EnumKey, V&gt; m
	///         = Collections.synchronizedMap(new EnumMap&lt;EnumKey, V&gt;(...));
	/// </pre>
	/// 
	/// </para>
	/// <para>Implementation note: All basic operations execute in constant time.
	/// They are likely (though not guaranteed) to be faster than their
	/// <seealso cref="HashMap"/> counterparts.
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// @author Josh Bloch
	/// </para>
	/// </summary>
	/// <seealso cref= EnumSet
	/// @since 1.5 </seealso>
	[Serializable]
	public class EnumMap<K, V> : AbstractMap<K, V>, Cloneable where K : Enum<K>
	{
		/// <summary>
		/// The <tt>Class</tt> object for the enum type of all the keys of this map.
		/// 
		/// @serial
		/// </summary>
		private readonly Class KeyType;

		/// <summary>
		/// All of the values comprising K.  (Cached for performance.)
		/// </summary>
		[NonSerialized]
		private K[] KeyUniverse;

		/// <summary>
		/// Array representation of this map.  The ith element is the value
		/// to which universe[i] is currently mapped, or null if it isn't
		/// mapped to anything, or NULL if it's mapped to null.
		/// </summary>
		[NonSerialized]
		private Object[] Vals;

		/// <summary>
		/// The number of mappings in this map.
		/// </summary>
		[NonSerialized]
		private int Size_Renamed = 0;

		/// <summary>
		/// Distinguished non-null value for representing null values.
		/// </summary>
		private static readonly Object NULL = new ObjectAnonymousInnerClassHelper();

		private class ObjectAnonymousInnerClassHelper : Object
		{
			public ObjectAnonymousInnerClassHelper()
			{
			}

			public override int HashCode()
			{
				return 0;
			}

			public override String ToString()
			{
				return "java.util.EnumMap.NULL";
			}
		}

		private Object MaskNull(Object value)
		{
			return (value == Map_Fields.Null ? NULL : value);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private V unmaskNull(Object value)
		private V UnmaskNull(Object value)
		{
			return (V)(value == NULL ? Map_Fields.Null : value);
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static final Enum<?>[] ZERO_LENGTH_ENUM_ARRAY = new Enum<?>[0];
		private static readonly Enum<?>[] ZERO_LENGTH_ENUM_ARRAY = new Enum<?>[0];

		/// <summary>
		/// Creates an empty enum map with the specified key type.
		/// </summary>
		/// <param name="keyType"> the class object of the key type for this enum map </param>
		/// <exception cref="NullPointerException"> if <tt>keyType</tt> is null </exception>
		public EnumMap(Class keyType)
		{
			this.KeyType = keyType;
			KeyUniverse = GetKeyUniverse(keyType);
			Vals = new Object[KeyUniverse.Length];
		}

		/// <summary>
		/// Creates an enum map with the same key type as the specified enum
		/// map, initially containing the same mappings (if any).
		/// </summary>
		/// <param name="m"> the enum map from which to initialize this enum map </param>
		/// <exception cref="NullPointerException"> if <tt>m</tt> is null </exception>
		public EnumMap<T1>(EnumMap<T1> m) where T1 : V
		{
			KeyType = m.KeyType;
			KeyUniverse = m.KeyUniverse;
			Vals = m.Vals.clone();
			Size_Renamed = m.Size_Renamed;
		}

		/// <summary>
		/// Creates an enum map initialized from the specified map.  If the
		/// specified map is an <tt>EnumMap</tt> instance, this constructor behaves
		/// identically to <seealso cref="#EnumMap(EnumMap)"/>.  Otherwise, the specified map
		/// must contain at least one mapping (in order to determine the new
		/// enum map's key type).
		/// </summary>
		/// <param name="m"> the map from which to initialize this enum map </param>
		/// <exception cref="IllegalArgumentException"> if <tt>m</tt> is not an
		///     <tt>EnumMap</tt> instance and contains no mappings </exception>
		/// <exception cref="NullPointerException"> if <tt>m</tt> is null </exception>
		public EnumMap<T1>(Map<T1> m) where T1 : V
		{
			if (m is EnumMap)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: EnumMap<K, ? extends V> em = (EnumMap<K, ? extends V>) m;
				EnumMap<K, ?> em = (EnumMap<K, ?>) m;
				KeyType = em.KeyType;
				KeyUniverse = em.KeyUniverse;
				Vals = em.Vals.clone();
				Size_Renamed = em.Size_Renamed;
			}
			else
			{
				if (m.Empty)
				{
					throw new IllegalArgumentException("Specified map is empty");
				}
				KeyType = m.KeySet().Iterator().Next().DeclaringClass;
				KeyUniverse = GetKeyUniverse(KeyType);
				Vals = new Object[KeyUniverse.Length];
				PutAll(m);
			}
		}

		// Query Operations

		/// <summary>
		/// Returns the number of key-value mappings in this map.
		/// </summary>
		/// <returns> the number of key-value mappings in this map </returns>
		public virtual int Size()
		{
			return Size_Renamed;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this map maps one or more keys to the
		/// specified value.
		/// </summary>
		/// <param name="value"> the value whose presence in this map is to be tested </param>
		/// <returns> <tt>true</tt> if this map maps one or more keys to this value </returns>
		public virtual bool ContainsValue(Object value)
		{
			value = MaskNull(value);

			foreach (Object val in Vals)
			{
				if (value.Equals(val))
				{
					return Map_Fields.True;
				}
			}

			return Map_Fields.False;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this map contains a mapping for the specified
		/// key.
		/// </summary>
		/// <param name="key"> the key whose presence in this map is to be tested </param>
		/// <returns> <tt>true</tt> if this map contains a mapping for the specified
		///            key </returns>
		public virtual bool ContainsKey(Object key)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return isValidKey(key) && vals[((Enum<?>)key).ordinal()] != Map_Fields.null;
			return IsValidKey(key) && Vals[((Enum<?>)key).Ordinal()] != Map_Fields.Null;
		}

		private bool ContainsMapping(Object key, Object value)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return isValidKey(key) && maskNull(value).equals(vals[((Enum<?>)key).ordinal()]);
			return IsValidKey(key) && MaskNull(value).Equals(Vals[((Enum<?>)key).Ordinal()]);
		}

		/// <summary>
		/// Returns the value to which the specified key is mapped,
		/// or {@code null} if this map contains no mapping for the key.
		/// 
		/// <para>More formally, if this map contains a mapping from a key
		/// {@code k} to a value {@code v} such that {@code (key == k)},
		/// then this method returns {@code v}; otherwise it returns
		/// {@code null}.  (There can be at most one such mapping.)
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
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return (isValidKey(key) ? unmaskNull(vals[((Enum<?>)key).ordinal()]) : Map_Fields.null);
			return (IsValidKey(key) ? UnmaskNull(Vals[((Enum<?>)key).Ordinal()]) : Map_Fields.Null);
		}

		// Modification Operations

		/// <summary>
		/// Associates the specified value with the specified key in this map.
		/// If the map previously contained a mapping for this key, the old
		/// value is replaced.
		/// </summary>
		/// <param name="key"> the key with which the specified value is to be associated </param>
		/// <param name="value"> the value to be associated with the specified key
		/// </param>
		/// <returns> the previous value associated with specified key, or
		///     <tt>null</tt> if there was no mapping for key.  (A <tt>null</tt>
		///     return can also indicate that the map previously associated
		///     <tt>null</tt> with the specified key.) </returns>
		/// <exception cref="NullPointerException"> if the specified key is null </exception>
		public virtual V Put(K key, V value)
		{
			TypeCheck(key);

			int index = key.ordinal();
			Object Map_Fields.OldValue = Vals[index];
			Vals[index] = MaskNull(value);
			if (Map_Fields.OldValue == Map_Fields.Null)
			{
				Size_Renamed++;
			}
			return UnmaskNull(Map_Fields.OldValue);
		}

		/// <summary>
		/// Removes the mapping for this key from this map if present.
		/// </summary>
		/// <param name="key"> the key whose mapping is to be removed from the map </param>
		/// <returns> the previous value associated with specified key, or
		///     <tt>null</tt> if there was no entry for key.  (A <tt>null</tt>
		///     return can also indicate that the map previously associated
		///     <tt>null</tt> with the specified key.) </returns>
		public virtual V Remove(Object key)
		{
			if (!IsValidKey(key))
			{
				return Map_Fields.Null;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: int index = ((Enum<?>)key).ordinal();
			int index = ((Enum<?>)key).Ordinal();
			Object Map_Fields.OldValue = Vals[index];
			Vals[index] = Map_Fields.Null;
			if (Map_Fields.OldValue != Map_Fields.Null)
			{
				Size_Renamed--;
			}
			return UnmaskNull(Map_Fields.OldValue);
		}

		private bool RemoveMapping(Object key, Object value)
		{
			if (!IsValidKey(key))
			{
				return Map_Fields.False;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: int index = ((Enum<?>)key).ordinal();
			int index = ((Enum<?>)key).Ordinal();
			if (MaskNull(value).Equals(Vals[index]))
			{
				Vals[index] = Map_Fields.Null;
				Size_Renamed--;
				return Map_Fields.True;
			}
			return Map_Fields.False;
		}

		/// <summary>
		/// Returns true if key is of the proper type to be a key in this
		/// enum map.
		/// </summary>
		private bool IsValidKey(Object key)
		{
			if (key == Map_Fields.Null)
			{
				return Map_Fields.False;
			}

			// Cheaper than instanceof Enum followed by getDeclaringClass
			Class keyClass = key.GetType();
			return keyClass == KeyType || keyClass.BaseType == KeyType;
		}

		// Bulk Operations

		/// <summary>
		/// Copies all of the mappings from the specified map to this map.
		/// These mappings will replace any mappings that this map had for
		/// any of the keys currently in the specified map.
		/// </summary>
		/// <param name="m"> the mappings to be stored in this map </param>
		/// <exception cref="NullPointerException"> the specified map is null, or if
		///     one or more keys in the specified map are null </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(Map<? extends K, ? extends V> m)
		public virtual void putAll<T1>(Map<T1> m) where T1 : K where ? : V
		{
			if (m is EnumMap)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: EnumMap<?, ?> em = (EnumMap<?, ?>)m;
				EnumMap<?, ?> em = (EnumMap<?, ?>)m;
				if (em.KeyType != KeyType)
				{
					if (em.Empty)
					{
						return;
					}
					throw new ClassCastException(em.KeyType + " != " + KeyType);
				}

				for (int i = 0; i < KeyUniverse.Length; i++)
				{
					Object emValue = em.Vals[i];
					if (emValue != Map_Fields.Null)
					{
						if (Vals[i] == Map_Fields.Null)
						{
							Size_Renamed++;
						}
						Vals[i] = emValue;
					}
				}
			}
			else
			{
				base.PutAll(m);
			}
		}

		/// <summary>
		/// Removes all mappings from this map.
		/// </summary>
		public virtual void Clear()
		{
			Arrays.Fill(Vals, Map_Fields.Null);
			Size_Renamed = 0;
		}

		// Views

		/// <summary>
		/// This field is initialized to contain an instance of the entry set
		/// view the first time this view is requested.  The view is stateless,
		/// so there's no reason to create more than one.
		/// </summary>
		[NonSerialized]
		private Set<Map_Entry<K, V>> EntrySet_Renamed;

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the keys contained in this map.
		/// The returned set obeys the general contract outlined in
		/// <seealso cref="Map#keySet()"/>.  The set's iterator will return the keys
		/// in their natural order (the order in which the enum constants
		/// are declared).
		/// </summary>
		/// <returns> a set view of the keys contained in this enum map </returns>
		public virtual Set<K> KeySet()
		{
			Set<K> ks = keySet;
			if (ks != Map_Fields.Null)
			{
				return ks;
			}
			else
			{
				return keySet = new KeySet(this);
			}
		}

		private class KeySet : AbstractSet<K>
		{
			private readonly EnumMap<K, V> OuterInstance;

			public KeySet(EnumMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<K> Iterator()
			{
				return new KeyIterator(OuterInstance);
			}
			public virtual int Size()
			{
				return outerInstance.Size_Renamed;
			}
			public virtual bool Contains(Object o)
			{
				return outerInstance.ContainsKey(o);
			}
			public virtual bool Remove(Object o)
			{
				int oldSize = outerInstance.Size_Renamed;
				OuterInstance.Remove(o);
				return outerInstance.Size_Renamed != oldSize;
			}
			public virtual void Clear()
			{
				OuterInstance.Clear();
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Collection"/> view of the values contained in this map.
		/// The returned collection obeys the general contract outlined in
		/// <seealso cref="Map#values()"/>.  The collection's iterator will return the
		/// values in the order their corresponding keys appear in map,
		/// which is their natural order (the order in which the enum constants
		/// are declared).
		/// </summary>
		/// <returns> a collection view of the values contained in this map </returns>
		public virtual Collection<V> Values()
		{
			Collection<V> vs = values;
			if (vs != Map_Fields.Null)
			{
				return vs;
			}
			else
			{
				return values = new Values(this);
			}
		}

		private class Values : AbstractCollection<V>
		{
			private readonly EnumMap<K, V> OuterInstance;

			public Values(EnumMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<V> Iterator()
			{
				return new ValueIterator(OuterInstance);
			}
			public virtual int Size()
			{
				return outerInstance.Size_Renamed;
			}
			public virtual bool Contains(Object o)
			{
				return outerInstance.ContainsValue(o);
			}
			public virtual bool Remove(Object o)
			{
				o = outerInstance.MaskNull(o);

				for (int i = 0; i < outerInstance.Vals.Length; i++)
				{
					if (o.Equals(outerInstance.Vals[i]))
					{
						outerInstance.Vals[i] = Map_Fields.Null;
						outerInstance.Size_Renamed--;
						return Map_Fields.True;
					}
				}
				return Map_Fields.False;
			}
			public virtual void Clear()
			{
				OuterInstance.Clear();
			}
		}

		/// <summary>
		/// Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		/// The returned set obeys the general contract outlined in
		/// <seealso cref="Map#keySet()"/>.  The set's iterator will return the
		/// mappings in the order their keys appear in map, which is their
		/// natural order (the order in which the enum constants are declared).
		/// </summary>
		/// <returns> a set view of the mappings contained in this enum map </returns>
		public virtual Set<Map_Entry<K, V>> EntrySet()
		{
			Set<Map_Entry<K, V>> es = EntrySet_Renamed;
			if (es != Map_Fields.Null)
			{
				return es;
			}
			else
			{
				return EntrySet_Renamed = new EntrySet(this);
			}
		}

		private class EntrySet : AbstractSet<Map_Entry<K, V>>
		{
			private readonly EnumMap<K, V> OuterInstance;

			public EntrySet(EnumMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Iterator<Map_Entry<K, V>> Iterator()
			{
				return new EntryIterator(OuterInstance);
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
				return outerInstance.ContainsMapping(entry.Key, entry.Value);
			}
			public virtual bool Remove(Object o)
			{
				if (!(o is Map_Entry))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> entry = (Map_Entry<?,?>)o;
				Map_Entry<?, ?> entry = (Map_Entry<?, ?>)o;
				return outerInstance.RemoveMapping(entry.Key, entry.Value);
			}
			public virtual int Size()
			{
				return outerInstance.Size_Renamed;
			}
			public virtual void Clear()
			{
				OuterInstance.Clear();
			}
			public virtual Object[] ToArray()
			{
				return FillEntryArray(new Object[outerInstance.Size_Renamed]);
			}
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T[] toArray(T[] a)
			public virtual T[] toArray<T>(T[] a)
			{
				int size = Size();
				if (a.Length < size)
				{
					a = (T[])java.lang.reflect.Array.NewInstance(a.GetType().GetElementType(), size);
				}
				if (a.Length > size)
				{
					a[size] = Map_Fields.Null;
				}
				return (T[]) FillEntryArray(a);
			}
			internal virtual Object[] FillEntryArray(Object[] a)
			{
				int j = 0;
				for (int i = 0; i < outerInstance.Vals.Length; i++)
				{
					if (outerInstance.Vals[i] != Map_Fields.Null)
					{
						a[j++] = new AbstractMap.SimpleEntry<>(outerInstance.KeyUniverse[i], outerInstance.UnmaskNull(outerInstance.Vals[i]));
					}
				}
				return a;
			}
		}

		private abstract class EnumMapIterator<T> : Iterator<T>
		{
			private readonly EnumMap<K, V> OuterInstance;

			public EnumMapIterator(EnumMap<K, V> outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			// Lower bound on index of next element to return
			internal int Index = 0;

			// Index of last returned element, or -1 if none
			internal int LastReturnedIndex = -1;

			public virtual bool HasNext()
			{
				while (Index < outerInstance.Vals.Length && outerInstance.Vals[Index] == Map_Fields.Null)
				{
					Index++;
				}
				return Index != outerInstance.Vals.Length;
			}

			public virtual void Remove()
			{
				CheckLastReturnedIndex();

				if (outerInstance.Vals[LastReturnedIndex] != Map_Fields.Null)
				{
					outerInstance.Vals[LastReturnedIndex] = Map_Fields.Null;
					outerInstance.Size_Renamed--;
				}
				LastReturnedIndex = -1;
			}

			internal virtual void CheckLastReturnedIndex()
			{
				if (LastReturnedIndex < 0)
				{
					throw new IllegalStateException();
				}
			}
		}

		private class KeyIterator : EnumMapIterator<K>
		{
			private readonly EnumMap<K, V> OuterInstance;

			public KeyIterator(EnumMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual K Next()
			{
				if (!HasNext())
				{
					throw new NoSuchElementException();
				}
				LastReturnedIndex = Index++;
				return outerInstance.KeyUniverse[LastReturnedIndex];
			}
		}

		private class ValueIterator : EnumMapIterator<V>
		{
			private readonly EnumMap<K, V> OuterInstance;

			public ValueIterator(EnumMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual V Next()
			{
				if (!HasNext())
				{
					throw new NoSuchElementException();
				}
				LastReturnedIndex = Index++;
				return outerInstance.UnmaskNull(outerInstance.Vals[LastReturnedIndex]);
			}
		}

		private class EntryIterator : EnumMapIterator<Map_Entry<K, V>>
		{
			private readonly EnumMap<K, V> OuterInstance;

			public EntryIterator(EnumMap<K, V> outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			internal Map_Entry LastReturnedEntry;

			public virtual Map_Entry<K, V> Next()
			{
				if (!HasNext())
				{
					throw new NoSuchElementException();
				}
				LastReturnedEntry = new Map_Entry(Index++);
				return LastReturnedEntry;
			}

			public virtual void Remove()
			{
				LastReturnedIndex = ((Map_Fields.Null == LastReturnedEntry) ? - 1 : LastReturnedEntry.index);
				base.Remove();
				LastReturnedEntry.index = LastReturnedIndex;
				LastReturnedEntry = Map_Fields.Null;
			}

			private class Entry : Map_Entry<K, V>
			{
				private readonly EnumMap.EntryIterator OuterInstance;

				internal int Index;

				internal IDictionary.Entry(EnumMap.EntryIterator outerInstance, int index)
				{
					this.OuterInstance = outerInstance;
					this.Index = index;
				}

				public virtual K Key
				{
					get
					{
						CheckIndexForEntryUse();
						return outerInstance.OuterInstance.KeyUniverse[Index];
					}
				}

				public virtual V Value
				{
					get
					{
						CheckIndexForEntryUse();
						return outerInstance.outerInstance.UnmaskNull(outerInstance.OuterInstance.Vals[Index]);
					}
				}

				public virtual V SetValue(V value)
				{
					CheckIndexForEntryUse();
					V Map_Fields.OldValue = outerInstance.outerInstance.UnmaskNull(outerInstance.OuterInstance.Vals[Index]);
					outerInstance.OuterInstance.Vals[Index] = outerInstance.outerInstance.MaskNull(value);
					return Map_Fields.OldValue;
				}

				public override bool Equals(Object o)
				{
					if (Index < 0)
					{
						return o == this;
					}

					if (!(o is Map_Entry))
					{
						return Map_Fields.False;
					}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>)o;
					Map_Entry<?, ?> e = (Map_Entry<?, ?>)o;
					V ourValue = outerInstance.outerInstance.UnmaskNull(outerInstance.OuterInstance.Vals[Index]);
					Object hisValue = e.Value;
					return (e.Key == outerInstance.OuterInstance.KeyUniverse[Index] && (ourValue == hisValue || (ourValue != Map_Fields.Null && ourValue.Equals(hisValue))));
				}

				public override int HashCode()
				{
					if (Index < 0)
					{
						return base.HashCode();
					}

					return outerInstance.outerInstance.EntryHashCode(Index);
				}

				public override String ToString()
				{
					if (Index < 0)
					{
						return base.ToString();
					}

					return outerInstance.OuterInstance.KeyUniverse[Index] + "=" + outerInstance.outerInstance.UnmaskNull(outerInstance.OuterInstance.Vals[Index]);
				}

				internal virtual void CheckIndexForEntryUse()
				{
					if (Index < 0)
					{
						throw new IllegalStateException("Entry was removed");
					}
				}
			}
		}

		// Comparison and hashing

		/// <summary>
		/// Compares the specified object with this map for equality.  Returns
		/// <tt>true</tt> if the given object is also a map and the two maps
		/// represent the same mappings, as specified in the {@link
		/// Map#equals(Object)} contract.
		/// </summary>
		/// <param name="o"> the object to be compared for equality with this map </param>
		/// <returns> <tt>true</tt> if the specified object is equal to this map </returns>
		public override bool Equals(Object o)
		{
			if (this == o)
			{
				return Map_Fields.True;
			}
			if (o is EnumMap)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: return equals((EnumMap<?,?>)o);
				return Equals((EnumMap<?, ?>)o);
			}
			if (!(o is Map))
			{
				return Map_Fields.False;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map<?,?> m = (Map<?,?>)o;
			Map<?, ?> m = (Map<?, ?>)o;
			if (Size_Renamed != m.Size())
			{
				return Map_Fields.False;
			}

			for (int i = 0; i < KeyUniverse.Length; i++)
			{
				if (Map_Fields.Null != Vals[i])
				{
					K key = KeyUniverse[i];
					V value = UnmaskNull(Vals[i]);
					if (Map_Fields.Null == value)
					{
						if (!((Map_Fields.Null == m.Get(key)) && m.ContainsKey(key)))
						{
						   return Map_Fields.False;
						}
					}
					else
					{
					   if (!value.Equals(m.Get(key)))
					   {
						  return Map_Fields.False;
					   }
					}
				}
			}

			return Map_Fields.True;
		}

		private bool equals<T1>(EnumMap<T1> em)
		{
			if (em.KeyType != KeyType)
			{
				return Size_Renamed == 0 && em.Size_Renamed == 0;
			}

			// Key types match, compare each value
			for (int i = 0; i < KeyUniverse.Length; i++)
			{
				Object ourValue = Vals[i];
				Object hisValue = em.Vals[i];
				if (hisValue != ourValue && (hisValue == Map_Fields.Null || !hisValue.Equals(ourValue)))
				{
					return Map_Fields.False;
				}
			}
			return Map_Fields.True;
		}

		/// <summary>
		/// Returns the hash code value for this map.  The hash code of a map is
		/// defined to be the sum of the hash codes of each entry in the map.
		/// </summary>
		public override int HashCode()
		{
			int h = 0;

			for (int i = 0; i < KeyUniverse.Length; i++)
			{
				if (Map_Fields.Null != Vals[i])
				{
					h += EntryHashCode(i);
				}
			}

			return h;
		}

		private int EntryHashCode(int index)
		{
			return (KeyUniverse[index].HashCode() ^ Vals[index].HashCode());
		}

		/// <summary>
		/// Returns a shallow copy of this enum map.  (The values themselves
		/// are not cloned.
		/// </summary>
		/// <returns> a shallow copy of this enum map </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public EnumMap<K, V> clone()
		public virtual EnumMap<K, V> Clone()
		{
			EnumMap<K, V> result = Map_Fields.Null;
			try
			{
				result = (EnumMap<K, V>) base.Clone();
			}
			catch (CloneNotSupportedException)
			{
				throw new AssertionError();
			}
			result.Vals = result.Vals.clone();
			result.EntrySet_Renamed = Map_Fields.Null;
			return result;
		}

		/// <summary>
		/// Throws an exception if e is not of the correct type for this enum set.
		/// </summary>
		private void TypeCheck(K key)
		{
			Class keyClass = key.GetType();
			if (keyClass != KeyType && keyClass.BaseType != KeyType)
			{
				throw new ClassCastException(keyClass + " != " + KeyType);
			}
		}

		/// <summary>
		/// Returns all of the values comprising K.
		/// The result is uncloned, cached, and shared by all callers.
		/// </summary>
		private static K[] getKeyUniverse<K>(Class keyType) where K : Enum<K>
		{
			return SharedSecrets.JavaLangAccess.getEnumConstantsShared(keyType);
		}

		private const long SerialVersionUID = 458661240069192865L;

		/// <summary>
		/// Save the state of the <tt>EnumMap</tt> instance to a stream (i.e.,
		/// serialize it).
		/// 
		/// @serialData The <i>size</i> of the enum map (the number of key-value
		///             mappings) is emitted (int), followed by the key (Object)
		///             and value (Object) for each key-value mapping represented
		///             by the enum map.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			// Write out the key type and any hidden stuff
			s.DefaultWriteObject();

			// Write out size (number of Mappings)
			s.WriteInt(Size_Renamed);

			// Write out keys and values (alternating)
			int entriesToBeWritten = Size_Renamed;
			for (int i = 0; entriesToBeWritten > 0; i++)
			{
				if (Map_Fields.Null != Vals[i])
				{
					s.WriteObject(KeyUniverse[i]);
					s.WriteObject(UnmaskNull(Vals[i]));
					entriesToBeWritten--;
				}
			}
		}

		/// <summary>
		/// Reconstitute the <tt>EnumMap</tt> instance from a stream (i.e.,
		/// deserialize it).
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Read in the key type and any hidden stuff
			s.DefaultReadObject();

			KeyUniverse = GetKeyUniverse(KeyType);
			Vals = new Object[KeyUniverse.Length];

			// Read in size (number of Mappings)
			int size = s.ReadInt();

			// Read the keys and values, and put the mappings in the HashMap
			for (int i = 0; i < size; i++)
			{
				K key = (K) s.ReadObject();
				V value = (V) s.ReadObject();
				Put(key, value);
			}
		}
	}

}