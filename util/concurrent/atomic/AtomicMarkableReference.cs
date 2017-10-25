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

namespace java.util.concurrent.atomic
{

	/// <summary>
	/// An {@code AtomicMarkableReference} maintains an object reference
	/// along with a mark bit, that can be updated atomically.
	/// 
	/// <para>Implementation note: This implementation maintains markable
	/// references by creating internal objects representing "boxed"
	/// [reference, boolean] pairs.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <V> The type of object referred to by this reference </param>
	public class AtomicMarkableReference<V>
	{

		private class Pair<T>
		{
			internal readonly T Reference;
			internal readonly bool Mark;
			internal Pair(T reference, bool mark)
			{
				this.Reference = reference;
				this.Mark = mark;
			}
			internal static Pair<T> of<T>(T reference, bool mark)
			{
				return new Pair<T>(reference, mark);
			}
		}

		private volatile Pair<V> Pair;

		/// <summary>
		/// Creates a new {@code AtomicMarkableReference} with the given
		/// initial values.
		/// </summary>
		/// <param name="initialRef"> the initial reference </param>
		/// <param name="initialMark"> the initial mark </param>
		public AtomicMarkableReference(V initialRef, bool initialMark)
		{
			Pair = Pair.Of(initialRef, initialMark);
		}

		/// <summary>
		/// Returns the current value of the reference.
		/// </summary>
		/// <returns> the current value of the reference </returns>
		public virtual V Reference
		{
			get
			{
				return Pair.Reference;
			}
		}

		/// <summary>
		/// Returns the current value of the mark.
		/// </summary>
		/// <returns> the current value of the mark </returns>
		public virtual bool Marked
		{
			get
			{
				return Pair.Mark;
			}
		}

		/// <summary>
		/// Returns the current values of both the reference and the mark.
		/// Typical usage is {@code boolean[1] holder; ref = v.get(holder); }.
		/// </summary>
		/// <param name="markHolder"> an array of size of at least one. On return,
		/// {@code markholder[0]} will hold the value of the mark. </param>
		/// <returns> the current value of the reference </returns>
		public virtual V Get(bool[] markHolder)
		{
			Pair<V> pair = this.Pair;
			markHolder[0] = pair.Mark;
			return pair.Reference;
		}

		/// <summary>
		/// Atomically sets the value of both the reference and mark
		/// to the given update values if the
		/// current reference is {@code ==} to the expected reference
		/// and the current mark is equal to the expected mark.
		/// 
		/// <para><a href="package-summary.html#weakCompareAndSet">May fail
		/// spuriously and does not provide ordering guarantees</a>, so is
		/// only rarely an appropriate alternative to {@code compareAndSet}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="expectedReference"> the expected value of the reference </param>
		/// <param name="newReference"> the new value for the reference </param>
		/// <param name="expectedMark"> the expected value of the mark </param>
		/// <param name="newMark"> the new value for the mark </param>
		/// <returns> {@code true} if successful </returns>
		public virtual bool WeakCompareAndSet(V expectedReference, V newReference, bool expectedMark, bool newMark)
		{
			return CompareAndSet(expectedReference, newReference, expectedMark, newMark);
		}

		/// <summary>
		/// Atomically sets the value of both the reference and mark
		/// to the given update values if the
		/// current reference is {@code ==} to the expected reference
		/// and the current mark is equal to the expected mark.
		/// </summary>
		/// <param name="expectedReference"> the expected value of the reference </param>
		/// <param name="newReference"> the new value for the reference </param>
		/// <param name="expectedMark"> the expected value of the mark </param>
		/// <param name="newMark"> the new value for the mark </param>
		/// <returns> {@code true} if successful </returns>
		public virtual bool CompareAndSet(V expectedReference, V newReference, bool expectedMark, bool newMark)
		{
			Pair<V> current = Pair;
			return expectedReference == current.Reference && expectedMark == current.Mark && ((newReference == current.Reference && newMark == current.Mark) || CasPair(current, Pair.Of(newReference, newMark)));
		}

		/// <summary>
		/// Unconditionally sets the value of both the reference and mark.
		/// </summary>
		/// <param name="newReference"> the new value for the reference </param>
		/// <param name="newMark"> the new value for the mark </param>
		public virtual void Set(V newReference, bool newMark)
		{
			Pair<V> current = Pair;
			if (newReference != current.Reference || newMark != current.Mark)
			{
				this.Pair = Pair.Of(newReference, newMark);
			}
		}

		/// <summary>
		/// Atomically sets the value of the mark to the given update value
		/// if the current reference is {@code ==} to the expected
		/// reference.  Any given invocation of this operation may fail
		/// (return {@code false}) spuriously, but repeated invocation
		/// when the current value holds the expected value and no other
		/// thread is also attempting to set the value will eventually
		/// succeed.
		/// </summary>
		/// <param name="expectedReference"> the expected value of the reference </param>
		/// <param name="newMark"> the new value for the mark </param>
		/// <returns> {@code true} if successful </returns>
		public virtual bool AttemptMark(V expectedReference, bool newMark)
		{
			Pair<V> current = Pair;
			return expectedReference == current.Reference && (newMark == current.Mark || CasPair(current, Pair.Of(expectedReference, newMark)));
		}

		// Unsafe mechanics

		private static readonly sun.misc.Unsafe UNSAFE = sun.misc.Unsafe.Unsafe;
		private static readonly long PairOffset = ObjectFieldOffset(UNSAFE, "pair", typeof(AtomicMarkableReference));

		private bool CasPair(Pair<V> cmp, Pair<V> val)
		{
			return UNSAFE.compareAndSwapObject(this, PairOffset, cmp, val);
		}

		internal static long ObjectFieldOffset(sun.misc.Unsafe UNSAFE, String field, Class klazz)
		{
			try
			{
				return UNSAFE.objectFieldOffset(klazz.GetDeclaredField(field));
			}
			catch (NoSuchFieldException e)
			{
				// Convert Exception to corresponding Error
				NoSuchFieldError error = new NoSuchFieldError(field);
				error.InitCause(e);
				throw error;
			}
		}
	}

}