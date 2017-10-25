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

	/// <summary>
	/// Private implementation class for EnumSet, for "jumbo" enum types
	/// (i.e., those with more than 64 elements).
	/// 
	/// @author Josh Bloch
	/// @since 1.5
	/// @serial exclude
	/// </summary>
	internal class JumboEnumSet<E> : EnumSet<E> where E : Enum<E>
	{
		private const long SerialVersionUID = 334349849919042784L;

		/// <summary>
		/// Bit vector representation of this set.  The ith bit of the jth
		/// element of this array represents the  presence of universe[64*j +i]
		/// in this set.
		/// </summary>
		private long[] Elements;

		// Redundant - maintained for performance
		private int Size_Renamed = 0;

		internal JumboEnumSet<T1>(ClasselementType, Enum<T1>[] universe) : base(elementType, universe)
		{
			Elements = new long[(int)((uint)(universe.Length + 63) >> 6)];
		}

		internal virtual void AddRange(E from, E to)
		{
			int fromIndex = (int)((uint)from.ordinal() >> 6);
			int toIndex = (int)((uint)to.ordinal() >> 6);

			if (fromIndex == toIndex)
			{
				Elements[fromIndex] = (int)((uint)(-1L >> (from.ordinal() - to.ordinal() - 1))) << from.ordinal();
			}
			else
			{
				Elements[fromIndex] = (-1L << from.ordinal());
				for (int i = fromIndex + 1; i < toIndex; i++)
				{
					Elements[i] = -1;
				}
				Elements[toIndex] = (int)((uint)-1L >> (63 - to.ordinal()));
			}
			Size_Renamed = to.ordinal() - from.ordinal() + 1;
		}

		internal virtual void AddAll()
		{
			for (int i = 0; i < Elements.Length; i++)
			{
				Elements[i] = -1;
			}
			Elements[Elements.Length - 1] = (long)((ulong)Elements[Elements.Length - 1] >> -universe.length);
			Size_Renamed = universe.length;
		}

		internal virtual void Complement()
		{
			for (int i = 0; i < Elements.Length; i++)
			{
				Elements[i] = ~Elements[i];
			}
			Elements[Elements.Length - 1] &= (int)((uint)(-1L >> -universe.length));
			Size_Renamed = universe.length - Size_Renamed;
		}

		/// <summary>
		/// Returns an iterator over the elements contained in this set.  The
		/// iterator traverses the elements in their <i>natural order</i> (which is
		/// the order in which the enum constants are declared). The returned
		/// Iterator is a "weakly consistent" iterator that will never throw {@link
		/// ConcurrentModificationException}.
		/// </summary>
		/// <returns> an iterator over the elements contained in this set </returns>
		public virtual Iterator<E> Iterator()
		{
			return new EnumSetIterator<>();
		}

		private class EnumSetIterator<E> : Iterator<E> where E : Enum<E>
		{
			private readonly JumboEnumSet<E> OuterInstance;

			/// <summary>
			/// A bit vector representing the elements in the current "word"
			/// of the set not yet returned by this iterator.
			/// </summary>
			internal long Unseen;

			/// <summary>
			/// The index corresponding to unseen in the elements array.
			/// </summary>
			internal int UnseenIndex = 0;

			/// <summary>
			/// The bit representing the last element returned by this iterator
			/// but not removed, or zero if no such element exists.
			/// </summary>
			internal long LastReturned = 0;

			/// <summary>
			/// The index corresponding to lastReturned in the elements array.
			/// </summary>
			internal int LastReturnedIndex = 0;

			internal EnumSetIterator(JumboEnumSet<E> outerInstance)
			{
				this.OuterInstance = outerInstance;
				Unseen = outerInstance.Elements[0];
			}

			public override bool HasNext()
			{
				while (Unseen == 0 && UnseenIndex < outerInstance.Elements.Length - 1)
				{
					Unseen = outerInstance.Elements[++UnseenIndex];
				}
				return Unseen != 0;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public E next()
			public override E Next()
			{
				if (!HasNext())
				{
					throw new NoSuchElementException();
				}
				LastReturned = Unseen & -Unseen;
				LastReturnedIndex = UnseenIndex;
				Unseen -= LastReturned;
				return (E) universe[(LastReturnedIndex << 6) + Long.NumberOfTrailingZeros(LastReturned)];
			}

			public override void Remove()
			{
				if (LastReturned == 0)
				{
					throw new IllegalStateException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long oldElements = elements[lastReturnedIndex];
				long oldElements = outerInstance.Elements[LastReturnedIndex];
				outerInstance.Elements[LastReturnedIndex] &= ~LastReturned;
				if (oldElements != outerInstance.Elements[LastReturnedIndex])
				{
					outerInstance.Size_Renamed--;
				}
				LastReturned = 0;
			}
		}

		/// <summary>
		/// Returns the number of elements in this set.
		/// </summary>
		/// <returns> the number of elements in this set </returns>
		public virtual int Size()
		{
			return Size_Renamed;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this set contains no elements.
		/// </summary>
		/// <returns> <tt>true</tt> if this set contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return Size_Renamed == 0;
			}
		}

		/// <summary>
		/// Returns <tt>true</tt> if this set contains the specified element.
		/// </summary>
		/// <param name="e"> element to be checked for containment in this collection </param>
		/// <returns> <tt>true</tt> if this set contains the specified element </returns>
		public virtual bool Contains(Object e)
		{
			if (e == null)
			{
				return false;
			}
			Class eClass = e.GetType();
			if (eClass != elementType && eClass.BaseType != elementType)
			{
				return false;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: int eOrdinal = ((Enum<?>)e).ordinal();
			int eOrdinal = ((Enum<?>)e).Ordinal();
			return (Elements[(int)((uint)eOrdinal >> 6)] & (1L << eOrdinal)) != 0;
		}

		// Modification Operations

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// </summary>
		/// <param name="e"> element to be added to this set </param>
		/// <returns> <tt>true</tt> if the set changed as a result of the call
		/// </returns>
		/// <exception cref="NullPointerException"> if <tt>e</tt> is null </exception>
		public virtual bool Add(E e)
		{
			typeCheck(e);

			int eOrdinal = e.ordinal();
			int eWordNum = (int)((uint)eOrdinal >> 6);

			long oldElements = Elements[eWordNum];
			Elements[eWordNum] |= (1L << eOrdinal);
			bool result = (Elements[eWordNum] != oldElements);
			if (result)
			{
				Size_Renamed++;
			}
			return result;
		}

		/// <summary>
		/// Removes the specified element from this set if it is present.
		/// </summary>
		/// <param name="e"> element to be removed from this set, if present </param>
		/// <returns> <tt>true</tt> if the set contained the specified element </returns>
		public virtual bool Remove(Object e)
		{
			if (e == null)
			{
				return false;
			}
			Class eClass = e.GetType();
			if (eClass != elementType && eClass.BaseType != elementType)
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: int eOrdinal = ((Enum<?>)e).ordinal();
			int eOrdinal = ((Enum<?>)e).Ordinal();
			int eWordNum = (int)((uint)eOrdinal >> 6);

			long oldElements = Elements[eWordNum];
			Elements[eWordNum] &= ~(1L << eOrdinal);
			bool result = (Elements[eWordNum] != oldElements);
			if (result)
			{
				Size_Renamed--;
			}
			return result;
		}

		// Bulk Operations

		/// <summary>
		/// Returns <tt>true</tt> if this set contains all of the elements
		/// in the specified collection.
		/// </summary>
		/// <param name="c"> collection to be checked for containment in this set </param>
		/// <returns> <tt>true</tt> if this set contains all of the elements
		///        in the specified collection </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public virtual bool containsAll<T1>(Collection<T1> c)
		{
			if (!(c is JumboEnumSet))
			{
				return base.ContainsAll(c);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: JumboEnumSet<?> es = (JumboEnumSet<?>)c;
			JumboEnumSet<?> es = (JumboEnumSet<?>)c;
			if (es.elementType != elementType)
			{
				return es.Empty;
			}

			for (int i = 0; i < Elements.Length; i++)
			{
				if ((es.Elements[i] & ~Elements[i]) != 0)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Adds all of the elements in the specified collection to this set.
		/// </summary>
		/// <param name="c"> collection whose elements are to be added to this set </param>
		/// <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection or any of
		///     its elements are null </exception>
		public virtual bool addAll<T1>(Collection<T1> c) where T1 : E
		{
			if (!(c is JumboEnumSet))
			{
				return base.AddAll(c);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: JumboEnumSet<?> es = (JumboEnumSet<?>)c;
			JumboEnumSet<?> es = (JumboEnumSet<?>)c;
			if (es.elementType != elementType)
			{
				if (es.Empty)
				{
					return false;
				}
				else
				{
					throw new ClassCastException(es.elementType + " != " + elementType);
				}
			}

			for (int i = 0; i < Elements.Length; i++)
			{
				Elements[i] |= es.Elements[i];
			}
			return RecalculateSize();
		}

		/// <summary>
		/// Removes from this set all of its elements that are contained in
		/// the specified collection.
		/// </summary>
		/// <param name="c"> elements to be removed from this set </param>
		/// <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public virtual bool removeAll<T1>(Collection<T1> c)
		{
			if (!(c is JumboEnumSet))
			{
				return base.RemoveAll(c);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: JumboEnumSet<?> es = (JumboEnumSet<?>)c;
			JumboEnumSet<?> es = (JumboEnumSet<?>)c;
			if (es.elementType != elementType)
			{
				return false;
			}

			for (int i = 0; i < Elements.Length; i++)
			{
				Elements[i] &= ~es.Elements[i];
			}
			return RecalculateSize();
		}

		/// <summary>
		/// Retains only the elements in this set that are contained in the
		/// specified collection.
		/// </summary>
		/// <param name="c"> elements to be retained in this set </param>
		/// <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public virtual bool retainAll<T1>(Collection<T1> c)
		{
			if (!(c is JumboEnumSet))
			{
				return base.RetainAll(c);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: JumboEnumSet<?> es = (JumboEnumSet<?>)c;
			JumboEnumSet<?> es = (JumboEnumSet<?>)c;
			if (es.elementType != elementType)
			{
				bool changed = (Size_Renamed != 0);
				Clear();
				return changed;
			}

			for (int i = 0; i < Elements.Length; i++)
			{
				Elements[i] &= es.Elements[i];
			}
			return RecalculateSize();
		}

		/// <summary>
		/// Removes all of the elements from this set.
		/// </summary>
		public virtual void Clear()
		{
			Arrays.Fill(Elements, 0);
			Size_Renamed = 0;
		}

		/// <summary>
		/// Compares the specified object with this set for equality.  Returns
		/// <tt>true</tt> if the given object is also a set, the two sets have
		/// the same size, and every member of the given set is contained in
		/// this set.
		/// </summary>
		/// <param name="o"> object to be compared for equality with this set </param>
		/// <returns> <tt>true</tt> if the specified object is equal to this set </returns>
		public override bool Equals(Object o)
		{
			if (!(o is JumboEnumSet))
			{
				return base.Equals(o);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: JumboEnumSet<?> es = (JumboEnumSet<?>)o;
			JumboEnumSet<?> es = (JumboEnumSet<?>)o;
			if (es.elementType != elementType)
			{
				return Size_Renamed == 0 && es.Size_Renamed == 0;
			}

			return System.Array.Equals(es.Elements, Elements);
		}

		/// <summary>
		/// Recalculates the size of the set.  Returns true if it's changed.
		/// </summary>
		private bool RecalculateSize()
		{
			int oldSize = Size_Renamed;
			Size_Renamed = 0;
			foreach (long elt in Elements)
			{
				Size_Renamed += Long.BitCount(elt);
			}

			return Size_Renamed != oldSize;
		}

		public virtual EnumSet<E> Clone()
		{
			JumboEnumSet<E> result = (JumboEnumSet<E>) base.Clone();
			result.Elements = result.Elements.clone();
			return result;
		}
	}

}