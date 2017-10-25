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
	/// An {@code AtomicStampedReference} maintains an object reference
	/// along with an integer "stamp", that can be updated atomically.
	/// 
	/// <para>Implementation note: This implementation maintains stamped
	/// references by creating internal objects representing "boxed"
	/// [reference, integer] pairs.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	/// @param <V> The type of object referred to by this reference </param>
	public class AtomicStampedReference<V>
	{

		private class Pair<T>
		{
			internal readonly T Reference;
			internal readonly int Stamp;
			internal Pair(T reference, int stamp)
			{
				this.Reference = reference;
				this.Stamp = stamp;
			}
			internal static Pair<T> of<T>(T reference, int stamp)
			{
				return new Pair<T>(reference, stamp);
			}
		}

		private volatile Pair<V> Pair;

		/// <summary>
		/// Creates a new {@code AtomicStampedReference} with the given
		/// initial values.
		/// </summary>
		/// <param name="initialRef"> the initial reference </param>
		/// <param name="initialStamp"> the initial stamp </param>
		public AtomicStampedReference(V initialRef, int initialStamp)
		{
			Pair = Pair.Of(initialRef, initialStamp);
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
		/// Returns the current value of the stamp.
		/// </summary>
		/// <returns> the current value of the stamp </returns>
		public virtual int Stamp
		{
			get
			{
				return Pair.Stamp;
			}
		}

		/// <summary>
		/// Returns the current values of both the reference and the stamp.
		/// Typical usage is {@code int[1] holder; ref = v.get(holder); }.
		/// </summary>
		/// <param name="stampHolder"> an array of size of at least one.  On return,
		/// {@code stampholder[0]} will hold the value of the stamp. </param>
		/// <returns> the current value of the reference </returns>
		public virtual V Get(int[] stampHolder)
		{
			Pair<V> pair = this.Pair;
			stampHolder[0] = pair.Stamp;
			return pair.Reference;
		}

		/// <summary>
		/// Atomically sets the value of both the reference and stamp
		/// to the given update values if the
		/// current reference is {@code ==} to the expected reference
		/// and the current stamp is equal to the expected stamp.
		/// 
		/// <para><a href="package-summary.html#weakCompareAndSet">May fail
		/// spuriously and does not provide ordering guarantees</a>, so is
		/// only rarely an appropriate alternative to {@code compareAndSet}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="expectedReference"> the expected value of the reference </param>
		/// <param name="newReference"> the new value for the reference </param>
		/// <param name="expectedStamp"> the expected value of the stamp </param>
		/// <param name="newStamp"> the new value for the stamp </param>
		/// <returns> {@code true} if successful </returns>
		public virtual bool WeakCompareAndSet(V expectedReference, V newReference, int expectedStamp, int newStamp)
		{
			return CompareAndSet(expectedReference, newReference, expectedStamp, newStamp);
		}

		/// <summary>
		/// Atomically sets the value of both the reference and stamp
		/// to the given update values if the
		/// current reference is {@code ==} to the expected reference
		/// and the current stamp is equal to the expected stamp.
		/// </summary>
		/// <param name="expectedReference"> the expected value of the reference </param>
		/// <param name="newReference"> the new value for the reference </param>
		/// <param name="expectedStamp"> the expected value of the stamp </param>
		/// <param name="newStamp"> the new value for the stamp </param>
		/// <returns> {@code true} if successful </returns>
		public virtual bool CompareAndSet(V expectedReference, V newReference, int expectedStamp, int newStamp)
		{
			Pair<V> current = Pair;
			return expectedReference == current.Reference && expectedStamp == current.Stamp && ((newReference == current.Reference && newStamp == current.Stamp) || CasPair(current, Pair.Of(newReference, newStamp)));
		}

		/// <summary>
		/// Unconditionally sets the value of both the reference and stamp.
		/// </summary>
		/// <param name="newReference"> the new value for the reference </param>
		/// <param name="newStamp"> the new value for the stamp </param>
		public virtual void Set(V newReference, int newStamp)
		{
			Pair<V> current = Pair;
			if (newReference != current.Reference || newStamp != current.Stamp)
			{
				this.Pair = Pair.Of(newReference, newStamp);
			}
		}

		/// <summary>
		/// Atomically sets the value of the stamp to the given update value
		/// if the current reference is {@code ==} to the expected
		/// reference.  Any given invocation of this operation may fail
		/// (return {@code false}) spuriously, but repeated invocation
		/// when the current value holds the expected value and no other
		/// thread is also attempting to set the value will eventually
		/// succeed.
		/// </summary>
		/// <param name="expectedReference"> the expected value of the reference </param>
		/// <param name="newStamp"> the new value for the stamp </param>
		/// <returns> {@code true} if successful </returns>
		public virtual bool AttemptStamp(V expectedReference, int newStamp)
		{
			Pair<V> current = Pair;
			return expectedReference == current.Reference && (newStamp == current.Stamp || CasPair(current, Pair.Of(expectedReference, newStamp)));
		}

		// Unsafe mechanics

		private static readonly sun.misc.Unsafe UNSAFE = sun.misc.Unsafe.Unsafe;
		private static readonly long PairOffset = ObjectFieldOffset(UNSAFE, "pair", typeof(AtomicStampedReference));

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