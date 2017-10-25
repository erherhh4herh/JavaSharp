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
	/// This class provides a skeletal implementation of the <tt>Set</tt>
	/// interface to minimize the effort required to implement this
	/// interface. <para>
	/// 
	/// The process of implementing a set by extending this class is identical
	/// to that of implementing a Collection by extending AbstractCollection,
	/// except that all of the methods and constructors in subclasses of this
	/// class must obey the additional constraints imposed by the <tt>Set</tt>
	/// interface (for instance, the add method must not permit addition of
	/// </para>
	/// multiple instances of an object to a set).<para>
	/// 
	/// Note that this class does not override any of the implementations from
	/// the <tt>AbstractCollection</tt> class.  It merely adds implementations
	/// </para>
	/// for <tt>equals</tt> and <tt>hashCode</tt>.<para>
	/// 
	/// This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// </para>
	/// </summary>
	/// @param <E> the type of elements maintained by this set
	/// 
	/// @author  Josh Bloch
	/// @author  Neal Gafter </param>
	/// <seealso cref= Collection </seealso>
	/// <seealso cref= AbstractCollection </seealso>
	/// <seealso cref= Set
	/// @since 1.2 </seealso>

	public abstract class AbstractSet<E> : AbstractCollection<E>, Set<E>
	{
		/// <summary>
		/// Sole constructor.  (For invocation by subclass constructors, typically
		/// implicit.)
		/// </summary>
		protected internal AbstractSet()
		{
		}

		// Comparison and hashing

		/// <summary>
		/// Compares the specified object with this set for equality.  Returns
		/// <tt>true</tt> if the given object is also a set, the two sets have
		/// the same size, and every member of the given set is contained in
		/// this set.  This ensures that the <tt>equals</tt> method works
		/// properly across different implementations of the <tt>Set</tt>
		/// interface.<para>
		/// 
		/// This implementation first checks if the specified object is this
		/// set; if so it returns <tt>true</tt>.  Then, it checks if the
		/// specified object is a set whose size is identical to the size of
		/// this set; if not, it returns false.  If so, it returns
		/// <tt>containsAll((Collection) o)</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="o"> object to be compared for equality with this set </param>
		/// <returns> <tt>true</tt> if the specified object is equal to this set </returns>
		public override bool Equals(Object o)
		{
			if (o == this)
			{
				return true;
			}

			if (!(o is Set))
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Collection<?> c = (Collection<?>) o;
			Collection<?> c = (Collection<?>) o;
			if (c.Size() != Size())
			{
				return false;
			}
			try
			{
				return ContainsAll(c);
			}
			catch (ClassCastException)
			{
				return false;
			}
			catch (NullPointerException)
			{
				return false;
			}
		}

		/// <summary>
		/// Returns the hash code value for this set.  The hash code of a set is
		/// defined to be the sum of the hash codes of the elements in the set,
		/// where the hash code of a <tt>null</tt> element is defined to be zero.
		/// This ensures that <tt>s1.equals(s2)</tt> implies that
		/// <tt>s1.hashCode()==s2.hashCode()</tt> for any two sets <tt>s1</tt>
		/// and <tt>s2</tt>, as required by the general contract of
		/// <seealso cref="Object#hashCode"/>.
		/// 
		/// <para>This implementation iterates over the set, calling the
		/// <tt>hashCode</tt> method on each element in the set, and adding up
		/// the results.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the hash code value for this set </returns>
		/// <seealso cref= Object#equals(Object) </seealso>
		/// <seealso cref= Set#equals(Object) </seealso>
		public override int HashCode()
		{
			int h = 0;
			Iterator<E> i = Iterator();
			while (i.HasNext())
			{
				E obj = i.Next();
				if (obj != null)
				{
					h += obj.HashCode();
				}
			}
			return h;
		}

		/// <summary>
		/// Removes from this set all of its elements that are contained in the
		/// specified collection (optional operation).  If the specified
		/// collection is also a set, this operation effectively modifies this
		/// set so that its value is the <i>asymmetric set difference</i> of
		/// the two sets.
		/// 
		/// <para>This implementation determines which is the smaller of this set
		/// and the specified collection, by invoking the <tt>size</tt>
		/// method on each.  If this set has fewer elements, then the
		/// implementation iterates over this set, checking each element
		/// returned by the iterator in turn to see if it is contained in
		/// the specified collection.  If it is so contained, it is removed
		/// from this set with the iterator's <tt>remove</tt> method.  If
		/// the specified collection has fewer elements, then the
		/// implementation iterates over the specified collection, removing
		/// from this set each element returned by the iterator, using this
		/// set's <tt>remove</tt> method.
		/// 
		/// </para>
		/// <para>Note that this implementation will throw an
		/// <tt>UnsupportedOperationException</tt> if the iterator returned by the
		/// <tt>iterator</tt> method does not implement the <tt>remove</tt> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c"> collection containing elements to be removed from this set </param>
		/// <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		/// <exception cref="UnsupportedOperationException"> if the <tt>removeAll</tt> operation
		///         is not supported by this set </exception>
		/// <exception cref="ClassCastException"> if the class of an element of this set
		///         is incompatible with the specified collection
		/// (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		/// <exception cref="NullPointerException"> if this set contains a null element and the
		///         specified collection does not permit null elements
		/// (<a href="Collection.html#optional-restrictions">optional</a>),
		///         or if the specified collection is null </exception>
		/// <seealso cref= #remove(Object) </seealso>
		/// <seealso cref= #contains(Object) </seealso>
		public virtual bool removeAll<T1>(Collection<T1> c)
		{
			Objects.RequireNonNull(c);
			bool modified = false;

			if (Size() > c.Size())
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Iterator<?> i = c.iterator(); i.hasNext();)
				for (Iterator<?> i = c.Iterator(); i.HasNext();)
				{
					modified |= Remove(i.Next());
				}
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Iterator<?> i = iterator(); i.hasNext();)
				for (Iterator<?> i = Iterator(); i.HasNext();)
				{
					if (c.Contains(i.Next()))
					{
						i.remove();
						modified = true;
					}
				}
			}
			return modified;
		}

	}

}