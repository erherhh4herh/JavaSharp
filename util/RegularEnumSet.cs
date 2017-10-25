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
	/// Private implementation class for EnumSet, for "regular sized" enum types
	/// (i.e., those with 64 or fewer enum constants).
	/// 
	/// @author Josh Bloch
	/// @since 1.5
	/// @serial exclude
	/// </summary>
	internal class RegularEnumSet<E> : EnumSet<E> where E : Enum<E>
	{
		private const long SerialVersionUID = 3411599620347842686L;
		/// <summary>
		/// Bit vector representation of this set.  The 2^k bit indicates the
		/// presence of universe[k] in this set.
		/// </summary>
		private long Elements = 0L;

		internal RegularEnumSet<T1>(ClasselementType, Enum<T1>[] universe) : base(elementType, universe)
		{
		}

		internal virtual void AddRange(E from, E to)
		{
			Elements = (int)((uint)(-1L >> (from.ordinal() - to.ordinal() - 1))) << from.ordinal();
		}

		internal virtual void AddAll()
		{
			if (universe.length != 0)
			{
				Elements = (int)((uint)-1L >> -universe.length);
			}
		}

		internal virtual void Complement()
		{
			if (universe.length != 0)
			{
				Elements = ~Elements;
				Elements &= (int)((uint)-1L >> -universe.length); // Mask unused bits
			}
		}

		/// <summary>
		/// Returns an iterator over the elements contained in this set.  The
		/// iterator traverses the elements in their <i>natural order</i> (which is
		/// the order in which the enum constants are declared). The returned
		/// Iterator is a "snapshot" iterator that will never throw {@link
		/// ConcurrentModificationException}; the elements are traversed as they
		/// existed when this call was invoked.
		/// </summary>
		/// <returns> an iterator over the elements contained in this set </returns>
		public virtual Iterator<E> Iterator()
		{
			return new EnumSetIterator<>();
		}

		private class EnumSetIterator<E> : Iterator<E> where E : Enum<E>
		{
			private readonly RegularEnumSet<E> OuterInstance;

			/// <summary>
			/// A bit vector representing the elements in the set not yet
			/// returned by this iterator.
			/// </summary>
			internal long Unseen;

			/// <summary>
			/// The bit representing the last element returned by this iterator
			/// but not removed, or zero if no such element exists.
			/// </summary>
			internal long LastReturned = 0;

			internal EnumSetIterator(RegularEnumSet<E> outerInstance)
			{
				this.OuterInstance = outerInstance;
				Unseen = outerInstance.Elements;
			}

			public virtual bool HasNext()
			{
				return Unseen != 0;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public E next()
			public virtual E Next()
			{
				if (Unseen == 0)
				{
					throw new NoSuchElementException();
				}
				LastReturned = Unseen & -Unseen;
				Unseen -= LastReturned;
				return (E) universe[Long.NumberOfTrailingZeros(LastReturned)];
			}

			public virtual void Remove()
			{
				if (LastReturned == 0)
				{
					throw new IllegalStateException();
				}
				outerInstance.Elements &= ~LastReturned;
				LastReturned = 0;
			}
		}

		/// <summary>
		/// Returns the number of elements in this set.
		/// </summary>
		/// <returns> the number of elements in this set </returns>
		public virtual int Size()
		{
			return Long.BitCount(Elements);
		}

		/// <summary>
		/// Returns <tt>true</tt> if this set contains no elements.
		/// </summary>
		/// <returns> <tt>true</tt> if this set contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return Elements == 0;
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
//ORIGINAL LINE: return (elements & (1L << ((Enum<?>)e).ordinal())) != 0;
			return (Elements & (1L << ((Enum<?>)e).Ordinal())) != 0;
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

			long oldElements = Elements;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: elements |= (1L << ((Enum<?>)e).ordinal());
			Elements |= (1L << ((Enum<?>)e).Ordinal());
			return Elements != oldElements;
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

			long oldElements = Elements;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: elements &= ~(1L << ((Enum<?>)e).ordinal());
			Elements &= ~(1L << ((Enum<?>)e).Ordinal());
			return Elements != oldElements;
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
			if (!(c is RegularEnumSet))
			{
				return base.ContainsAll(c);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RegularEnumSet<?> es = (RegularEnumSet<?>)c;
			RegularEnumSet<?> es = (RegularEnumSet<?>)c;
			if (es.elementType != elementType)
			{
				return es.Empty;
			}

			return (es.Elements & ~Elements) == 0;
		}

		/// <summary>
		/// Adds all of the elements in the specified collection to this set.
		/// </summary>
		/// <param name="c"> collection whose elements are to be added to this set </param>
		/// <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection or any
		///     of its elements are null </exception>
		public virtual bool addAll<T1>(Collection<T1> c) where T1 : E
		{
			if (!(c is RegularEnumSet))
			{
				return base.AddAll(c);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RegularEnumSet<?> es = (RegularEnumSet<?>)c;
			RegularEnumSet<?> es = (RegularEnumSet<?>)c;
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

			long oldElements = Elements;
			Elements |= es.Elements;
			return Elements != oldElements;
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
			if (!(c is RegularEnumSet))
			{
				return base.RemoveAll(c);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RegularEnumSet<?> es = (RegularEnumSet<?>)c;
			RegularEnumSet<?> es = (RegularEnumSet<?>)c;
			if (es.elementType != elementType)
			{
				return false;
			}

			long oldElements = Elements;
			Elements &= ~es.Elements;
			return Elements != oldElements;
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
			if (!(c is RegularEnumSet))
			{
				return base.RetainAll(c);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RegularEnumSet<?> es = (RegularEnumSet<?>)c;
			RegularEnumSet<?> es = (RegularEnumSet<?>)c;
			if (es.elementType != elementType)
			{
				bool changed = (Elements != 0);
				Elements = 0;
				return changed;
			}

			long oldElements = Elements;
			Elements &= es.Elements;
			return Elements != oldElements;
		}

		/// <summary>
		/// Removes all of the elements from this set.
		/// </summary>
		public virtual void Clear()
		{
			Elements = 0;
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
			if (!(o is RegularEnumSet))
			{
				return base.Equals(o);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RegularEnumSet<?> es = (RegularEnumSet<?>)o;
			RegularEnumSet<?> es = (RegularEnumSet<?>)o;
			if (es.elementType != elementType)
			{
				return Elements == 0 && es.Elements == 0;
			}
			return es.Elements == Elements;
		}
	}

}