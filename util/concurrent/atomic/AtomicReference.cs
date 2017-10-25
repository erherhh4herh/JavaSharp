using System;

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
	using Unsafe = sun.misc.Unsafe;

	/// <summary>
	/// An object reference that may be updated atomically. See the {@link
	/// java.util.concurrent.atomic} package specification for description
	/// of the properties of atomic variables.
	/// @since 1.5
	/// @author Doug Lea </summary>
	/// @param <V> The type of object referred to by this reference </param>
	[Serializable]
	public class AtomicReference<V>
	{
		private const long SerialVersionUID = -1848883965231344442L;

		private static readonly Unsafe @unsafe = Unsafe.Unsafe;
		private static readonly long ValueOffset;

		static AtomicReference()
		{
			try
			{
				ValueOffset = @unsafe.objectFieldOffset(typeof(AtomicReference).getDeclaredField("value"));
			}
			catch (Exception ex)
			{
				throw new Error(ex);
			}
		}

		private volatile V Value;

		/// <summary>
		/// Creates a new AtomicReference with the given initial value.
		/// </summary>
		/// <param name="initialValue"> the initial value </param>
		public AtomicReference(V initialValue)
		{
			Value = initialValue;
		}

		/// <summary>
		/// Creates a new AtomicReference with null initial value.
		/// </summary>
		public AtomicReference()
		{
		}

		/// <summary>
		/// Gets the current value.
		/// </summary>
		/// <returns> the current value </returns>
		public V Get()
		{
			return Value;
		}

		/// <summary>
		/// Sets to the given value.
		/// </summary>
		/// <param name="newValue"> the new value </param>
		public void Set(V newValue)
		{
			Value = newValue;
		}

		/// <summary>
		/// Eventually sets to the given value.
		/// </summary>
		/// <param name="newValue"> the new value
		/// @since 1.6 </param>
		public void LazySet(V newValue)
		{
			@unsafe.putOrderedObject(this, ValueOffset, newValue);
		}

		/// <summary>
		/// Atomically sets the value to the given updated value
		/// if the current value {@code ==} the expected value. </summary>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful. False return indicates that
		/// the actual value was not equal to the expected value. </returns>
		public bool CompareAndSet(V expect, V update)
		{
			return @unsafe.compareAndSwapObject(this, ValueOffset, expect, update);
		}

		/// <summary>
		/// Atomically sets the value to the given updated value
		/// if the current value {@code ==} the expected value.
		/// 
		/// <para><a href="package-summary.html#weakCompareAndSet">May fail
		/// spuriously and does not provide ordering guarantees</a>, so is
		/// only rarely an appropriate alternative to {@code compareAndSet}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful </returns>
		public bool WeakCompareAndSet(V expect, V update)
		{
			return @unsafe.compareAndSwapObject(this, ValueOffset, expect, update);
		}

		/// <summary>
		/// Atomically sets to the given value and returns the old value.
		/// </summary>
		/// <param name="newValue"> the new value </param>
		/// <returns> the previous value </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public final V getAndSet(V newValue)
		public V GetAndSet(V newValue)
		{
			return (V)@unsafe.getAndSetObject(this, ValueOffset, newValue);
		}

		/// <summary>
		/// Atomically updates the current value with the results of
		/// applying the given function, returning the previous value. The
		/// function should be side-effect-free, since it may be re-applied
		/// when attempted updates fail due to contention among threads.
		/// </summary>
		/// <param name="updateFunction"> a side-effect-free function </param>
		/// <returns> the previous value
		/// @since 1.8 </returns>
		public V GetAndUpdate(UnaryOperator<V> updateFunction)
		{
			V prev, next;
			do
			{
				prev = Get();
				next = updateFunction.Apply(prev);
			} while (!CompareAndSet(prev, next));
			return prev;
		}

		/// <summary>
		/// Atomically updates the current value with the results of
		/// applying the given function, returning the updated value. The
		/// function should be side-effect-free, since it may be re-applied
		/// when attempted updates fail due to contention among threads.
		/// </summary>
		/// <param name="updateFunction"> a side-effect-free function </param>
		/// <returns> the updated value
		/// @since 1.8 </returns>
		public V UpdateAndGet(UnaryOperator<V> updateFunction)
		{
			V prev, next;
			do
			{
				prev = Get();
				next = updateFunction.Apply(prev);
			} while (!CompareAndSet(prev, next));
			return next;
		}

		/// <summary>
		/// Atomically updates the current value with the results of
		/// applying the given function to the current and given values,
		/// returning the previous value. The function should be
		/// side-effect-free, since it may be re-applied when attempted
		/// updates fail due to contention among threads.  The function
		/// is applied with the current value as its first argument,
		/// and the given update as the second argument.
		/// </summary>
		/// <param name="x"> the update value </param>
		/// <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		/// <returns> the previous value
		/// @since 1.8 </returns>
		public V GetAndAccumulate(V x, BinaryOperator<V> accumulatorFunction)
		{
			V prev, next;
			do
			{
				prev = Get();
				next = accumulatorFunction.Apply(prev, x);
			} while (!CompareAndSet(prev, next));
			return prev;
		}

		/// <summary>
		/// Atomically updates the current value with the results of
		/// applying the given function to the current and given values,
		/// returning the updated value. The function should be
		/// side-effect-free, since it may be re-applied when attempted
		/// updates fail due to contention among threads.  The function
		/// is applied with the current value as its first argument,
		/// and the given update as the second argument.
		/// </summary>
		/// <param name="x"> the update value </param>
		/// <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		/// <returns> the updated value
		/// @since 1.8 </returns>
		public V AccumulateAndGet(V x, BinaryOperator<V> accumulatorFunction)
		{
			V prev, next;
			do
			{
				prev = Get();
				next = accumulatorFunction.Apply(prev, x);
			} while (!CompareAndSet(prev, next));
			return next;
		}

		/// <summary>
		/// Returns the String representation of the current value. </summary>
		/// <returns> the String representation of the current value </returns>
		public override String ToString()
		{
			return Convert.ToString(Get());
		}

	}

}