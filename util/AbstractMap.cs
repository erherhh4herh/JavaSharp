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
	/// This class provides a skeletal implementation of the <tt>Map</tt>
	/// interface, to minimize the effort required to implement this interface.
	/// 
	/// <para>To implement an unmodifiable map, the programmer needs only to extend this
	/// class and provide an implementation for the <tt>entrySet</tt> method, which
	/// returns a set-view of the map's mappings.  Typically, the returned set
	/// will, in turn, be implemented atop <tt>AbstractSet</tt>.  This set should
	/// not support the <tt>add</tt> or <tt>remove</tt> methods, and its iterator
	/// should not support the <tt>remove</tt> method.
	/// 
	/// </para>
	/// <para>To implement a modifiable map, the programmer must additionally override
	/// this class's <tt>put</tt> method (which otherwise throws an
	/// <tt>UnsupportedOperationException</tt>), and the iterator returned by
	/// <tt>entrySet().iterator()</tt> must additionally implement its
	/// <tt>remove</tt> method.
	/// 
	/// </para>
	/// <para>The programmer should generally provide a void (no argument) and map
	/// constructor, as per the recommendation in the <tt>Map</tt> interface
	/// specification.
	/// 
	/// </para>
	/// <para>The documentation for each non-abstract method in this class describes its
	/// implementation in detail.  Each of these methods may be overridden if the
	/// map being implemented admits a more efficient implementation.
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
	/// @author  Josh Bloch
	/// @author  Neal Gafter </param>
	/// <seealso cref= Map </seealso>
	/// <seealso cref= Collection
	/// @since 1.2 </seealso>

	public abstract class AbstractMap<K, V> : Map<K, V>
	{
		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal AbstractMap()
		{
		}

		// Query Operations

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation returns <tt>entrySet().size()</tt>.
		/// </summary>
		public virtual int Size()
		{
			return EntrySet().Count;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation returns <tt>size() == 0</tt>.
		/// </summary>
		public virtual bool Empty
		{
			get
			{
				return Size() == 0;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation iterates over <tt>entrySet()</tt> searching
		/// for an entry with the specified value.  If such an entry is found,
		/// <tt>true</tt> is returned.  If the iteration terminates without
		/// finding such an entry, <tt>false</tt> is returned.  Note that this
		/// implementation requires linear time in the size of the map.
		/// </summary>
		/// <exception cref="ClassCastException">   {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual bool ContainsValue(Object value)
		{
			Iterator<Map_Entry<K, V>> i = EntrySet().Iterator();
			if (value == Map_Fields.Null)
			{
				while (i.HasNext())
				{
					Map_Entry<K, V> e = i.Next();
					if (e.Value == Map_Fields.Null)
					{
						return Map_Fields.True;
					}
				}
			}
			else
			{
				while (i.HasNext())
				{
					Map_Entry<K, V> e = i.Next();
					if (value.Equals(e.Value))
					{
						return Map_Fields.True;
					}
				}
			}
			return Map_Fields.False;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation iterates over <tt>entrySet()</tt> searching
		/// for an entry with the specified key.  If such an entry is found,
		/// <tt>true</tt> is returned.  If the iteration terminates without
		/// finding such an entry, <tt>false</tt> is returned.  Note that this
		/// implementation requires linear time in the size of the map; many
		/// implementations will override this method.
		/// </summary>
		/// <exception cref="ClassCastException">   {@inheritDoc} </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc} </exception>
		public virtual bool ContainsKey(Object key)
		{
			Iterator<Map_Entry<K, V>> i = EntrySet().Iterator();
			if (key == Map_Fields.Null)
			{
				while (i.HasNext())
				{
					Map_Entry<K, V> e = i.Next();
					if (e.Key == Map_Fields.Null)
					{
						return Map_Fields.True;
					}
				}
			}
			else
			{
				while (i.HasNext())
				{
					Map_Entry<K, V> e = i.Next();
					if (key.Equals(e.Key))
					{
						return Map_Fields.True;
					}
				}
			}
			return Map_Fields.False;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation iterates over <tt>entrySet()</tt> searching
		/// for an entry with the specified key.  If such an entry is found,
		/// the entry's value is returned.  If the iteration terminates without
		/// finding such an entry, <tt>null</tt> is returned.  Note that this
		/// implementation requires linear time in the size of the map; many
		/// implementations will override this method.
		/// </summary>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		public virtual V Get(Object key)
		{
			Iterator<Map_Entry<K, V>> i = EntrySet().Iterator();
			if (key == Map_Fields.Null)
			{
				while (i.HasNext())
				{
					Map_Entry<K, V> e = i.Next();
					if (e.Key == Map_Fields.Null)
					{
						return e.Value;
					}
				}
			}
			else
			{
				while (i.HasNext())
				{
					Map_Entry<K, V> e = i.Next();
					if (key.Equals(e.Key))
					{
						return e.Value;
					}
				}
			}
			return Map_Fields.Null;
		}


		// Modification Operations

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation always throws an
		/// <tt>UnsupportedOperationException</tt>.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		public virtual V Put(K key, V value)
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation iterates over <tt>entrySet()</tt> searching for an
		/// entry with the specified key.  If such an entry is found, its value is
		/// obtained with its <tt>getValue</tt> operation, the entry is removed
		/// from the collection (and the backing map) with the iterator's
		/// <tt>remove</tt> operation, and the saved value is returned.  If the
		/// iteration terminates without finding such an entry, <tt>null</tt> is
		/// returned.  Note that this implementation requires linear time in the
		/// size of the map; many implementations will override this method.
		/// 
		/// <para>Note that this implementation throws an
		/// <tt>UnsupportedOperationException</tt> if the <tt>entrySet</tt>
		/// iterator does not support the <tt>remove</tt> method and this map
		/// contains a mapping for the specified key.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		public virtual V Remove(Object key)
		{
			Iterator<Map_Entry<K, V>> i = EntrySet().Iterator();
			Map_Entry<K, V> correctEntry = Map_Fields.Null;
			if (key == Map_Fields.Null)
			{
				while (correctEntry == Map_Fields.Null && i.HasNext())
				{
					Map_Entry<K, V> e = i.Next();
					if (e.Key == Map_Fields.Null)
					{
						correctEntry = e;
					}
				}
			}
			else
			{
				while (correctEntry == Map_Fields.Null && i.HasNext())
				{
					Map_Entry<K, V> e = i.Next();
					if (key.Equals(e.Key))
					{
						correctEntry = e;
					}
				}
			}

			V Map_Fields.OldValue = Map_Fields.Null;
			if (correctEntry != Map_Fields.Null)
			{
				Map_Fields.OldValue = correctEntry.Value;
				i.remove();
			}
			return Map_Fields.OldValue;
		}


		// Bulk Operations

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation iterates over the specified map's
		/// <tt>entrySet()</tt> collection, and calls this map's <tt>put</tt>
		/// operation once for each entry returned by the iteration.
		/// 
		/// <para>Note that this implementation throws an
		/// <tt>UnsupportedOperationException</tt> if this map does not support
		/// the <tt>put</tt> operation and the specified map is nonempty.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		/// <exception cref="ClassCastException">            {@inheritDoc} </exception>
		/// <exception cref="NullPointerException">          {@inheritDoc} </exception>
		/// <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(Map<? extends K, ? extends V> m)
		public virtual void putAll<T1>(Map<T1> m) where T1 : K where ? : V
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Map_Entry<? extends K, ? extends V> e : m.entrySet())
			foreach (Map_Entry<?, ?> e in m.EntrySet())
			{
				Put(e.Key, e.Value);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation calls <tt>entrySet().clear()</tt>.
		/// 
		/// <para>Note that this implementation throws an
		/// <tt>UnsupportedOperationException</tt> if the <tt>entrySet</tt>
		/// does not support the <tt>clear</tt> operation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		public virtual void Clear()
		{
			EntrySet().Clear();
		}


		// Views

		/// <summary>
		/// Each of these fields are initialized to contain an instance of the
		/// appropriate view the first time this view is requested.  The views are
		/// stateless, so there's no reason to create more than one of each.
		/// </summary>
		[NonSerialized]
		internal volatile Set<K> KeySet_Renamed;
		[NonSerialized]
		internal volatile Collection<V> Values_Renamed;

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation returns a set that subclasses <seealso cref="AbstractSet"/>.
		/// The subclass's iterator method returns a "wrapper object" over this
		/// map's <tt>entrySet()</tt> iterator.  The <tt>size</tt> method
		/// delegates to this map's <tt>size</tt> method and the
		/// <tt>contains</tt> method delegates to this map's
		/// <tt>containsKey</tt> method.
		/// 
		/// <para>The set is created the first time this method is called,
		/// and returned in response to all subsequent calls.  No synchronization
		/// is performed, so there is a slight chance that multiple calls to this
		/// method will not all return the same set.
		/// </para>
		/// </summary>
		public virtual Set<K> KeySet()
		{
			if (KeySet_Renamed == Map_Fields.Null)
			{
				KeySet_Renamed = new AbstractSetAnonymousInnerClassHelper(this);
			}
			return KeySet_Renamed;
		}

		private class AbstractSetAnonymousInnerClassHelper : AbstractSet<K>
		{
			private readonly AbstractMap<K, V> OuterInstance;

			public AbstractSetAnonymousInnerClassHelper(AbstractMap<K, V> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual Iterator<K> Iterator()
			{
				return new IteratorAnonymousInnerClassHelper(this);
			}

			private class IteratorAnonymousInnerClassHelper : Iterator<K>
			{
				private readonly AbstractSetAnonymousInnerClassHelper OuterInstance;

				public IteratorAnonymousInnerClassHelper(AbstractSetAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
					i = outerInstance.outerInstance.EntrySet().GetEnumerator();
				}

				private Iterator<Map_Entry<K, V>> i;

				public virtual bool HasNext()
				{
					return i.hasNext();
				}

				public virtual K Next()
				{
					return i.next().Key;
				}

				public virtual void Remove()
				{
					i.remove();
				}
			}

			public virtual int Size()
			{
				return OuterInstance.size();
			}

			public virtual bool Empty
			{
				get
				{
					return OuterInstance.Empty;
				}
			}

			public virtual void Clear()
			{
				OuterInstance.clear();
			}

			public virtual bool Contains(Object Map_Fields)
			{
				return OuterInstance.containsKey(Map_Fields.k);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @implSpec
		/// This implementation returns a collection that subclasses {@link
		/// AbstractCollection}.  The subclass's iterator method returns a
		/// "wrapper object" over this map's <tt>entrySet()</tt> iterator.
		/// The <tt>size</tt> method delegates to this map's <tt>size</tt>
		/// method and the <tt>contains</tt> method delegates to this map's
		/// <tt>containsValue</tt> method.
		/// 
		/// <para>The collection is created the first time this method is called, and
		/// returned in response to all subsequent calls.  No synchronization is
		/// performed, so there is a slight chance that multiple calls to this
		/// method will not all return the same collection.
		/// </para>
		/// </summary>
		public virtual Collection<V> Values()
		{
			if (Values_Renamed == Map_Fields.Null)
			{
				Values_Renamed = new AbstractCollectionAnonymousInnerClassHelper(this);
			}
			return Values_Renamed;
		}

		private class AbstractCollectionAnonymousInnerClassHelper : AbstractCollection<V>
		{
			private readonly AbstractMap<K, V> OuterInstance;

			public AbstractCollectionAnonymousInnerClassHelper(AbstractMap<K, V> outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual Iterator<V> Iterator()
			{
				return new IteratorAnonymousInnerClassHelper2(this);
			}

			private class IteratorAnonymousInnerClassHelper2 : Iterator<V>
			{
				private readonly AbstractCollectionAnonymousInnerClassHelper OuterInstance;

				public IteratorAnonymousInnerClassHelper2(AbstractCollectionAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
					i = outerInstance.outerInstance.EntrySet().GetEnumerator();
				}

				private Iterator<Map_Entry<K, V>> i;

				public virtual bool HasNext()
				{
					return i.hasNext();
				}

				public virtual V Next()
				{
					return i.next().Value;
				}

				public virtual void Remove()
				{
					i.remove();
				}
			}

			public virtual int Size()
			{
				return OuterInstance.size();
			}

			public virtual bool Empty
			{
				get
				{
					return OuterInstance.Empty;
				}
			}

			public virtual void Clear()
			{
				OuterInstance.clear();
			}

			public virtual bool Contains(Object Map_Fields)
			{
				return OuterInstance.containsValue(Map_Fields.v);
			}
		}

		public abstract Set<Map_Entry<K, V>> EntrySet();


		// Comparison and hashing

		/// <summary>
		/// Compares the specified object with this map for equality.  Returns
		/// <tt>true</tt> if the given object is also a map and the two maps
		/// represent the same mappings.  More formally, two maps <tt>m1</tt> and
		/// <tt>m2</tt> represent the same mappings if
		/// <tt>m1.entrySet().equals(m2.entrySet())</tt>.  This ensures that the
		/// <tt>equals</tt> method works properly across different implementations
		/// of the <tt>Map</tt> interface.
		/// 
		/// @implSpec
		/// This implementation first checks if the specified object is this map;
		/// if so it returns <tt>true</tt>.  Then, it checks if the specified
		/// object is a map whose size is identical to the size of this map; if
		/// not, it returns <tt>false</tt>.  If so, it iterates over this map's
		/// <tt>entrySet</tt> collection, and checks that the specified map
		/// contains each mapping that this map contains.  If the specified map
		/// fails to contain such a mapping, <tt>false</tt> is returned.  If the
		/// iteration completes, <tt>true</tt> is returned.
		/// </summary>
		/// <param name="o"> object to be compared for equality with this map </param>
		/// <returns> <tt>true</tt> if the specified object is equal to this map </returns>
		public override bool Equals(Object o)
		{
			if (o == this)
			{
				return Map_Fields.True;
			}

			if (!(o is Map))
			{
				return Map_Fields.False;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map<?,?> m = (Map<?,?>) o;
			Map<?, ?> m = (Map<?, ?>) o;
			if (m.Size() != Size())
			{
				return Map_Fields.False;
			}

			try
			{
				Iterator<Map_Entry<K, V>> i = EntrySet().Iterator();
				while (i.HasNext())
				{
					Map_Entry<K, V> e = i.Next();
					K key = e.Key;
					V value = e.Value;
					if (value == Map_Fields.Null)
					{
						if (!(m.Get(key) == Map_Fields.Null && m.ContainsKey(key)))
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
			catch (ClassCastException)
			{
				return Map_Fields.False;
			}
			catch (NullPointerException)
			{
				return Map_Fields.False;
			}

			return Map_Fields.True;
		}

		/// <summary>
		/// Returns the hash code value for this map.  The hash code of a map is
		/// defined to be the sum of the hash codes of each entry in the map's
		/// <tt>entrySet()</tt> view.  This ensures that <tt>m1.equals(m2)</tt>
		/// implies that <tt>m1.hashCode()==m2.hashCode()</tt> for any two maps
		/// <tt>m1</tt> and <tt>m2</tt>, as required by the general contract of
		/// <seealso cref="Object#hashCode"/>.
		/// 
		/// @implSpec
		/// This implementation iterates over <tt>entrySet()</tt>, calling
		/// <seealso cref="Map.Entry#hashCode hashCode()"/> on each element (entry) in the
		/// set, and adding up the results.
		/// </summary>
		/// <returns> the hash code value for this map </returns>
		/// <seealso cref= Map.Entry#hashCode() </seealso>
		/// <seealso cref= Object#equals(Object) </seealso>
		/// <seealso cref= Set#equals(Object) </seealso>
		public override int HashCode()
		{
			int h = 0;
			Iterator<Map_Entry<K, V>> i = EntrySet().Iterator();
			while (i.HasNext())
			{
				h += i.Next().HashCode();
			}
			return h;
		}

		/// <summary>
		/// Returns a string representation of this map.  The string representation
		/// consists of a list of key-value mappings in the order returned by the
		/// map's <tt>entrySet</tt> view's iterator, enclosed in braces
		/// (<tt>"{}"</tt>).  Adjacent mappings are separated by the characters
		/// <tt>", "</tt> (comma and space).  Each key-value mapping is rendered as
		/// the key followed by an equals sign (<tt>"="</tt>) followed by the
		/// associated value.  Keys and values are converted to strings as by
		/// <seealso cref="String#valueOf(Object)"/>.
		/// </summary>
		/// <returns> a string representation of this map </returns>
		public override String ToString()
		{
			Iterator<Map_Entry<K, V>> i = EntrySet().Iterator();
			if (!i.HasNext())
			{
				return "{}";
			}

			StringBuilder sb = new StringBuilder();
			sb.Append('{');
			for (;;)
			{
				Map_Entry<K, V> e = i.Next();
				K key = e.Key;
				V value = e.Value;
				sb.Append(key == this ? "(this Map)" : key);
				sb.Append('=');
				sb.Append(value == this ? "(this Map)" : value);
				if (!i.HasNext())
				{
					return sb.Append('}').ToString();
				}
				sb.Append(',').Append(' ');
			}
		}

		/// <summary>
		/// Returns a shallow copy of this <tt>AbstractMap</tt> instance: the keys
		/// and values themselves are not cloned.
		/// </summary>
		/// <returns> a shallow copy of this map </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object clone() throws CloneNotSupportedException
		protected internal virtual Object Clone()
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: AbstractMap<?,?> result = (AbstractMap<?,?>)base.clone();
			AbstractMap<?, ?> result = (AbstractMap<?, ?>)base.Clone();
			result.KeySet_Renamed = Map_Fields.Null;
			result.Values_Renamed = Map_Fields.Null;
			return result;
		}

		/// <summary>
		/// Utility method for SimpleEntry and SimpleImmutableEntry.
		/// Test for equality, checking for nulls.
		/// 
		/// NB: Do not replace with Object.equals until JDK-8015417 is resolved.
		/// </summary>
		private static bool Eq(Object o1, Object o2)
		{
			return o1 == Map_Fields.Null ? o2 == Map_Fields.Null : o1.Equals(o2);
		}

		// Implementation Note: SimpleEntry and SimpleImmutableEntry
		// are distinct unrelated classes, even though they share
		// some code. Since you can't add or subtract final-ness
		// of a field in a subclass, they can't share representations,
		// and the amount of duplicated code is too small to warrant
		// exposing a common abstract class.


		/// <summary>
		/// An Entry maintaining a key and a value.  The value may be
		/// changed using the <tt>setValue</tt> method.  This class
		/// facilitates the process of building custom map
		/// implementations. For example, it may be convenient to return
		/// arrays of <tt>SimpleEntry</tt> instances in method
		/// <tt>Map.entrySet().toArray</tt>.
		/// 
		/// @since 1.6
		/// </summary>
		[Serializable]
		public class SimpleEntry<K, V> : Map_Entry<K, V>
		{
			internal const long SerialVersionUID = -8499721149061103585L;

			internal readonly K Key_Renamed;
			internal V Value_Renamed;

			/// <summary>
			/// Creates an entry representing a mapping from the specified
			/// key to the specified value.
			/// </summary>
			/// <param name="key"> the key represented by this entry </param>
			/// <param name="value"> the value represented by this entry </param>
			public SimpleEntry(K key, V value)
			{
				this.Key_Renamed = key;
				this.Value_Renamed = value;
			}

			/// <summary>
			/// Creates an entry representing the same mapping as the
			/// specified entry.
			/// </summary>
			/// <param name="entry"> the entry to copy </param>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public SimpleEntry(java.util.Map_Entry<? extends K, ? extends V> entry)
			public SimpleEntry<T1>(Map_Entry<T1> entry) where T1 : K where ? : V
			{
				this.Key_Renamed = entry.Key;
				this.Value_Renamed = entry.Value;
			}

			/// <summary>
			/// Returns the key corresponding to this entry.
			/// </summary>
			/// <returns> the key corresponding to this entry </returns>
			public virtual K Key
			{
				get
				{
					return Key_Renamed;
				}
			}

			/// <summary>
			/// Returns the value corresponding to this entry.
			/// </summary>
			/// <returns> the value corresponding to this entry </returns>
			public virtual V Value
			{
				get
				{
					return Value_Renamed;
				}
			}

			/// <summary>
			/// Replaces the value corresponding to this entry with the specified
			/// value.
			/// </summary>
			/// <param name="value"> new value to be stored in this entry </param>
			/// <returns> the old value corresponding to the entry </returns>
			public virtual V SetValue(V value)
			{
				V Map_Fields.OldValue = this.Value_Renamed;
				this.Value_Renamed = value;
				return Map_Fields.OldValue;
			}

			/// <summary>
			/// Compares the specified object with this entry for equality.
			/// Returns {@code true} if the given object is also a map entry and
			/// the two entries represent the same mapping.  More formally, two
			/// entries {@code e1} and {@code e2} represent the same mapping
			/// if<pre>
			///   (e1.getKey()==null ?
			///    e2.getKey()==null :
			///    e1.getKey().equals(e2.getKey()))
			///   &amp;&amp;
			///   (e1.getValue()==null ?
			///    e2.getValue()==null :
			///    e1.getValue().equals(e2.getValue()))</pre>
			/// This ensures that the {@code equals} method works properly across
			/// different implementations of the {@code Map.Entry} interface.
			/// </summary>
			/// <param name="o"> object to be compared for equality with this map entry </param>
			/// <returns> {@code true} if the specified object is equal to this map
			///         entry </returns>
			/// <seealso cref=    #hashCode </seealso>
			public override bool Equals(Object o)
			{
				if (!(o is Map_Entry))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>)o;
				Map_Entry<?, ?> e = (Map_Entry<?, ?>)o;
				return Eq(Key_Renamed, e.Key) && Eq(Value_Renamed, e.Value);
			}

			/// <summary>
			/// Returns the hash code value for this map entry.  The hash code
			/// of a map entry {@code e} is defined to be: <pre>
			///   (e.getKey()==null   ? 0 : e.getKey().hashCode()) ^
			///   (e.getValue()==null ? 0 : e.getValue().hashCode())</pre>
			/// This ensures that {@code e1.equals(e2)} implies that
			/// {@code e1.hashCode()==e2.hashCode()} for any two Entries
			/// {@code e1} and {@code e2}, as required by the general
			/// contract of <seealso cref="Object#hashCode"/>.
			/// </summary>
			/// <returns> the hash code value for this map entry </returns>
			/// <seealso cref=    #equals </seealso>
			public override int HashCode()
			{
				return (Key_Renamed == Map_Fields.Null ? 0 : Key_Renamed.HashCode()) ^ (Value_Renamed == Map_Fields.Null ? 0 : Value_Renamed.HashCode());
			}

			/// <summary>
			/// Returns a String representation of this map entry.  This
			/// implementation returns the string representation of this
			/// entry's key followed by the equals character ("<tt>=</tt>")
			/// followed by the string representation of this entry's value.
			/// </summary>
			/// <returns> a String representation of this map entry </returns>
			public override String ToString()
			{
				return Key_Renamed + "=" + Value_Renamed;
			}

		}

		/// <summary>
		/// An Entry maintaining an immutable key and value.  This class
		/// does not support method <tt>setValue</tt>.  This class may be
		/// convenient in methods that return thread-safe snapshots of
		/// key-value mappings.
		/// 
		/// @since 1.6
		/// </summary>
		[Serializable]
		public class SimpleImmutableEntry<K, V> : Map_Entry<K, V>
		{
			internal const long SerialVersionUID = 7138329143949025153L;

			internal readonly K Key_Renamed;
			internal readonly V Value_Renamed;

			/// <summary>
			/// Creates an entry representing a mapping from the specified
			/// key to the specified value.
			/// </summary>
			/// <param name="key"> the key represented by this entry </param>
			/// <param name="value"> the value represented by this entry </param>
			public SimpleImmutableEntry(K key, V value)
			{
				this.Key_Renamed = key;
				this.Value_Renamed = value;
			}

			/// <summary>
			/// Creates an entry representing the same mapping as the
			/// specified entry.
			/// </summary>
			/// <param name="entry"> the entry to copy </param>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public SimpleImmutableEntry(java.util.Map_Entry<? extends K, ? extends V> entry)
			public SimpleImmutableEntry<T1>(Map_Entry<T1> entry) where T1 : K where ? : V
			{
				this.Key_Renamed = entry.Key;
				this.Value_Renamed = entry.Value;
			}

			/// <summary>
			/// Returns the key corresponding to this entry.
			/// </summary>
			/// <returns> the key corresponding to this entry </returns>
			public virtual K Key
			{
				get
				{
					return Key_Renamed;
				}
			}

			/// <summary>
			/// Returns the value corresponding to this entry.
			/// </summary>
			/// <returns> the value corresponding to this entry </returns>
			public virtual V Value
			{
				get
				{
					return Value_Renamed;
				}
			}

			/// <summary>
			/// Replaces the value corresponding to this entry with the specified
			/// value (optional operation).  This implementation simply throws
			/// <tt>UnsupportedOperationException</tt>, as this class implements
			/// an <i>immutable</i> map entry.
			/// </summary>
			/// <param name="value"> new value to be stored in this entry </param>
			/// <returns> (Does not return) </returns>
			/// <exception cref="UnsupportedOperationException"> always </exception>
			public virtual V SetValue(V value)
			{
				throw new UnsupportedOperationException();
			}

			/// <summary>
			/// Compares the specified object with this entry for equality.
			/// Returns {@code true} if the given object is also a map entry and
			/// the two entries represent the same mapping.  More formally, two
			/// entries {@code e1} and {@code e2} represent the same mapping
			/// if<pre>
			///   (e1.getKey()==null ?
			///    e2.getKey()==null :
			///    e1.getKey().equals(e2.getKey()))
			///   &amp;&amp;
			///   (e1.getValue()==null ?
			///    e2.getValue()==null :
			///    e1.getValue().equals(e2.getValue()))</pre>
			/// This ensures that the {@code equals} method works properly across
			/// different implementations of the {@code Map.Entry} interface.
			/// </summary>
			/// <param name="o"> object to be compared for equality with this map entry </param>
			/// <returns> {@code true} if the specified object is equal to this map
			///         entry </returns>
			/// <seealso cref=    #hashCode </seealso>
			public override bool Equals(Object o)
			{
				if (!(o is Map_Entry))
				{
					return Map_Fields.False;
				}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Map_Entry<?,?> e = (Map_Entry<?,?>)o;
				Map_Entry<?, ?> e = (Map_Entry<?, ?>)o;
				return Eq(Key_Renamed, e.Key) && Eq(Value_Renamed, e.Value);
			}

			/// <summary>
			/// Returns the hash code value for this map entry.  The hash code
			/// of a map entry {@code e} is defined to be: <pre>
			///   (e.getKey()==null   ? 0 : e.getKey().hashCode()) ^
			///   (e.getValue()==null ? 0 : e.getValue().hashCode())</pre>
			/// This ensures that {@code e1.equals(e2)} implies that
			/// {@code e1.hashCode()==e2.hashCode()} for any two Entries
			/// {@code e1} and {@code e2}, as required by the general
			/// contract of <seealso cref="Object#hashCode"/>.
			/// </summary>
			/// <returns> the hash code value for this map entry </returns>
			/// <seealso cref=    #equals </seealso>
			public override int HashCode()
			{
				return (Key_Renamed == Map_Fields.Null ? 0 : Key_Renamed.HashCode()) ^ (Value_Renamed == Map_Fields.Null ? 0 : Value_Renamed.HashCode());
			}

			/// <summary>
			/// Returns a String representation of this map entry.  This
			/// implementation returns the string representation of this
			/// entry's key followed by the equals character ("<tt>=</tt>")
			/// followed by the string representation of this entry's value.
			/// </summary>
			/// <returns> a String representation of this map entry </returns>
			public override String ToString()
			{
				return Key_Renamed + "=" + Value_Renamed;
			}

		}

	}

}