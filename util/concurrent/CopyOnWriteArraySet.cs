using System;
using System.Collections.Generic;

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
	/// A <seealso cref="java.util.Set"/> that uses an internal <seealso cref="CopyOnWriteArrayList"/>
	/// for all of its operations.  Thus, it shares the same basic properties:
	/// <ul>
	///  <li>It is best suited for applications in which set sizes generally
	///       stay small, read-only operations
	///       vastly outnumber mutative operations, and you need
	///       to prevent interference among threads during traversal.
	///  <li>It is thread-safe.
	///  <li>Mutative operations ({@code add}, {@code set}, {@code remove}, etc.)
	///      are expensive since they usually entail copying the entire underlying
	///      array.
	///  <li>Iterators do not support the mutative {@code remove} operation.
	///  <li>Traversal via iterators is fast and cannot encounter
	///      interference from other threads. Iterators rely on
	///      unchanging snapshots of the array at the time the iterators were
	///      constructed.
	/// </ul>
	/// 
	/// <para><b>Sample Usage.</b> The following code sketch uses a
	/// copy-on-write set to maintain a set of Handler objects that
	/// perform some action upon state updates.
	/// 
	///  <pre> {@code
	/// class Handler { void handle(); ... }
	/// 
	/// class X {
	///   private final CopyOnWriteArraySet<Handler> handlers
	///     = new CopyOnWriteArraySet<Handler>();
	///   public void addHandler(Handler h) { handlers.add(h); }
	/// 
	///   private long internalState;
	///   private synchronized void changeState() { internalState = ...; }
	/// 
	///   public void update() {
	///     changeState();
	///     for (Handler handler : handlers)
	///       handler.handle();
	///   }
	/// }}</pre>
	/// 
	/// </para>
	/// <para>This class is a member of the
	/// <a href="{@docRoot}/../technotes/guides/collections/index.html">
	/// Java Collections Framework</a>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= CopyOnWriteArrayList
	/// @since 1.5
	/// @author Doug Lea </seealso>
	/// @param <E> the type of elements held in this collection </param>
	[Serializable]
	public class CopyOnWriteArraySet<E> : AbstractSet<E>
	{
		private const long SerialVersionUID = 5457747651344034263L;

		private readonly CopyOnWriteArrayList<E> Al;

		/// <summary>
		/// Creates an empty set.
		/// </summary>
		public CopyOnWriteArraySet()
		{
			Al = new CopyOnWriteArrayList<E>();
		}

		/// <summary>
		/// Creates a set containing all of the elements of the specified
		/// collection.
		/// </summary>
		/// <param name="c"> the collection of elements to initially contain </param>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		public CopyOnWriteArraySet<T1>(ICollection<T1> c) where T1 : E
		{
			if (c.GetType() == typeof(CopyOnWriteArraySet))
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") CopyOnWriteArraySet<E> cc = (CopyOnWriteArraySet<E>)c;
				CopyOnWriteArraySet<E> cc = (CopyOnWriteArraySet<E>)c;
				Al = new CopyOnWriteArrayList<E>(cc.Al);
			}
			else
			{
				Al = new CopyOnWriteArrayList<E>();
				Al.AddAllAbsent(c);
			}
		}

		/// <summary>
		/// Returns the number of elements in this set.
		/// </summary>
		/// <returns> the number of elements in this set </returns>
		public virtual int Size()
		{
			return Al.Count;
		}

		/// <summary>
		/// Returns {@code true} if this set contains no elements.
		/// </summary>
		/// <returns> {@code true} if this set contains no elements </returns>
		public virtual bool Empty
		{
			get
			{
				return Al.Count == 0;
			}
		}

		/// <summary>
		/// Returns {@code true} if this set contains the specified element.
		/// More formally, returns {@code true} if and only if this set
		/// contains an element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </summary>
		/// <param name="o"> element whose presence in this set is to be tested </param>
		/// <returns> {@code true} if this set contains the specified element </returns>
		public virtual bool Contains(Object o)
		{
			return Al.Contains(o);
		}

		/// <summary>
		/// Returns an array containing all of the elements in this set.
		/// If this set makes any guarantees as to what order its elements
		/// are returned by its iterator, this method must return the
		/// elements in the same order.
		/// 
		/// <para>The returned array will be "safe" in that no references to it
		/// are maintained by this set.  (In other words, this method must
		/// allocate a new array even if this set is backed by an array).
		/// The caller is thus free to modify the returned array.
		/// 
		/// </para>
		/// <para>This method acts as bridge between array-based and collection-based
		/// APIs.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array containing all the elements in this set </returns>
		public virtual Object[] ToArray()
		{
			return Al.ToArray();
		}

		/// <summary>
		/// Returns an array containing all of the elements in this set; the
		/// runtime type of the returned array is that of the specified array.
		/// If the set fits in the specified array, it is returned therein.
		/// Otherwise, a new array is allocated with the runtime type of the
		/// specified array and the size of this set.
		/// 
		/// <para>If this set fits in the specified array with room to spare
		/// (i.e., the array has more elements than this set), the element in
		/// the array immediately following the end of the set is set to
		/// {@code null}.  (This is useful in determining the length of this
		/// set <i>only</i> if the caller knows that this set does not contain
		/// any null elements.)
		/// 
		/// </para>
		/// <para>If this set makes any guarantees as to what order its elements
		/// are returned by its iterator, this method must return the elements
		/// in the same order.
		/// 
		/// </para>
		/// <para>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		/// array-based and collection-based APIs.  Further, this method allows
		/// precise control over the runtime type of the output array, and may,
		/// under certain circumstances, be used to save allocation costs.
		/// 
		/// </para>
		/// <para>Suppose {@code x} is a set known to contain only strings.
		/// The following code can be used to dump the set into a newly allocated
		/// array of {@code String}:
		/// 
		///  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
		/// 
		/// Note that {@code toArray(new Object[0])} is identical in function to
		/// {@code toArray()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="a"> the array into which the elements of this set are to be
		///        stored, if it is big enough; otherwise, a new array of the same
		///        runtime type is allocated for this purpose. </param>
		/// <returns> an array containing all the elements in this set </returns>
		/// <exception cref="ArrayStoreException"> if the runtime type of the specified array
		///         is not a supertype of the runtime type of every element in this
		///         set </exception>
		/// <exception cref="NullPointerException"> if the specified array is null </exception>
		public virtual T[] toArray<T>(T[] a)
		{
			return Al.ToArray(a);
		}

		/// <summary>
		/// Removes all of the elements from this set.
		/// The set will be empty after this call returns.
		/// </summary>
		public virtual void Clear()
		{
			Al.Clear();
		}

		/// <summary>
		/// Removes the specified element from this set if it is present.
		/// More formally, removes an element {@code e} such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>,
		/// if this set contains such an element.  Returns {@code true} if
		/// this set contained the element (or equivalently, if this set
		/// changed as a result of the call).  (This set will not contain the
		/// element once the call returns.)
		/// </summary>
		/// <param name="o"> object to be removed from this set, if present </param>
		/// <returns> {@code true} if this set contained the specified element </returns>
		public virtual bool Remove(Object o)
		{
			return Al.Remove(o);
		}

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// More formally, adds the specified element {@code e} to this set if
		/// the set contains no element {@code e2} such that
		/// <tt>(e==null&nbsp;?&nbsp;e2==null&nbsp;:&nbsp;e.equals(e2))</tt>.
		/// If this set already contains the element, the call leaves the set
		/// unchanged and returns {@code false}.
		/// </summary>
		/// <param name="e"> element to be added to this set </param>
		/// <returns> {@code true} if this set did not already contain the specified
		///         element </returns>
		public virtual bool Add(E e)
		{
			return Al.AddIfAbsent(e);
		}

		/// <summary>
		/// Returns {@code true} if this set contains all of the elements of the
		/// specified collection.  If the specified collection is also a set, this
		/// method returns {@code true} if it is a <i>subset</i> of this set.
		/// </summary>
		/// <param name="c"> collection to be checked for containment in this set </param>
		/// <returns> {@code true} if this set contains all of the elements of the
		///         specified collection </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		/// <seealso cref= #contains(Object) </seealso>
		public virtual bool containsAll<T1>(ICollection<T1> c)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
			return Al.ContainsAll(c);
		}

		/// <summary>
		/// Adds all of the elements in the specified collection to this set if
		/// they're not already present.  If the specified collection is also a
		/// set, the {@code addAll} operation effectively modifies this set so
		/// that its value is the <i>union</i> of the two sets.  The behavior of
		/// this operation is undefined if the specified collection is modified
		/// while the operation is in progress.
		/// </summary>
		/// <param name="c"> collection containing elements to be added to this set </param>
		/// <returns> {@code true} if this set changed as a result of the call </returns>
		/// <exception cref="NullPointerException"> if the specified collection is null </exception>
		/// <seealso cref= #add(Object) </seealso>
		public virtual bool addAll<T1>(ICollection<T1> c) where T1 : E
		{
			return Al.AddAllAbsent(c) > 0;
		}

		/// <summary>
		/// Removes from this set all of its elements that are contained in the
		/// specified collection.  If the specified collection is also a set,
		/// this operation effectively modifies this set so that its value is the
		/// <i>asymmetric set difference</i> of the two sets.
		/// </summary>
		/// <param name="c"> collection containing elements to be removed from this set </param>
		/// <returns> {@code true} if this set changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the class of an element of this set
		///         is incompatible with the specified collection (optional) </exception>
		/// <exception cref="NullPointerException"> if this set contains a null element and the
		///         specified collection does not permit null elements (optional),
		///         or if the specified collection is null </exception>
		/// <seealso cref= #remove(Object) </seealso>
		public virtual bool removeAll<T1>(ICollection<T1> c)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
			return Al.RemoveAll(c);
		}

		/// <summary>
		/// Retains only the elements in this set that are contained in the
		/// specified collection.  In other words, removes from this set all of
		/// its elements that are not contained in the specified collection.  If
		/// the specified collection is also a set, this operation effectively
		/// modifies this set so that its value is the <i>intersection</i> of the
		/// two sets.
		/// </summary>
		/// <param name="c"> collection containing elements to be retained in this set </param>
		/// <returns> {@code true} if this set changed as a result of the call </returns>
		/// <exception cref="ClassCastException"> if the class of an element of this set
		///         is incompatible with the specified collection (optional) </exception>
		/// <exception cref="NullPointerException"> if this set contains a null element and the
		///         specified collection does not permit null elements (optional),
		///         or if the specified collection is null </exception>
		/// <seealso cref= #remove(Object) </seealso>
		public virtual bool retainAll<T1>(ICollection<T1> c)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'retainAll' method:
			return Al.RetainAll(c);
		}

		/// <summary>
		/// Returns an iterator over the elements contained in this set
		/// in the order in which these elements were added.
		/// 
		/// <para>The returned iterator provides a snapshot of the state of the set
		/// when the iterator was constructed. No synchronization is needed while
		/// traversing the iterator. The iterator does <em>NOT</em> support the
		/// {@code remove} method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an iterator over the elements in this set </returns>
		public virtual IEnumerator<E> Iterator()
		{
			return Al.Iterator();
		}

		/// <summary>
		/// Compares the specified object with this set for equality.
		/// Returns {@code true} if the specified object is the same object
		/// as this object, or if it is also a <seealso cref="Set"/> and the elements
		/// returned by an <seealso cref="Set#iterator() iterator"/> over the
		/// specified set are the same as the elements returned by an
		/// iterator over this set.  More formally, the two iterators are
		/// considered to return the same elements if they return the same
		/// number of elements and for every element {@code e1} returned by
		/// the iterator over the specified set, there is an element
		/// {@code e2} returned by the iterator over this set such that
		/// {@code (e1==null ? e2==null : e1.equals(e2))}.
		/// </summary>
		/// <param name="o"> object to be compared for equality with this set </param>
		/// <returns> {@code true} if the specified object is equal to this set </returns>
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
//ORIGINAL LINE: java.util.Set<?> set = (java.util.Set<?>)(o);
			Set<?> set = (Set<?>)(o);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<?> it = set.iterator();
			IEnumerator<?> it = set.Iterator();

			// Uses O(n^2) algorithm that is only appropriate
			// for small sets, which CopyOnWriteArraySets should be.

			//  Use a single snapshot of underlying array
			Object[] elements = Al.Array;
			int len = elements.Length;
			// Mark matched elements to avoid re-checking
			bool[] matched = new bool[len];
			int k = 0;
			while (it.MoveNext())
			{
				if (++k > len)
				{
					return false;
				}
				Object x = it.Current;
				for (int i = 0; i < len; ++i)
				{
					if (!matched[i] && Eq(x, elements[i]))
					{
						matched[i] = true;
						goto outerContinue;
					}
				}
				return false;
				outerContinue:;
			}
			outerBreak:
			return k == len;
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public boolean removeIf(java.util.function.Predicate<? base E> filter)
		public virtual bool removeIf<T1>(Predicate<T1> filter)
		{
			return Al.RemoveIf(filter);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: public void forEach(java.util.function.Consumer<? base E> action)
		public virtual void forEach<T1>(Consumer<T1> action)
		{
			Al.ForEach(action);
		}

		/// <summary>
		/// Returns a <seealso cref="Spliterator"/> over the elements in this set in the order
		/// in which these elements were added.
		/// 
		/// <para>The {@code Spliterator} reports <seealso cref="Spliterator#IMMUTABLE"/>,
		/// <seealso cref="Spliterator#DISTINCT"/>, <seealso cref="Spliterator#SIZED"/>, and
		/// <seealso cref="Spliterator#SUBSIZED"/>.
		/// 
		/// </para>
		/// <para>The spliterator provides a snapshot of the state of the set
		/// when the spliterator was constructed. No synchronization is needed while
		/// operating on the spliterator.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code Spliterator} over the elements in this set
		/// @since 1.8 </returns>
		public virtual Spliterator<E> Spliterator()
		{
			return Spliterators.Spliterator(Al.Array, java.util.Spliterator_Fields.IMMUTABLE | java.util.Spliterator_Fields.DISTINCT);
		}

		/// <summary>
		/// Tests for equality, coping with nulls.
		/// </summary>
		private static bool Eq(Object o1, Object o2)
		{
			return (o1 == null) ? o2 == null : o1.Equals(o2);
		}
	}

}