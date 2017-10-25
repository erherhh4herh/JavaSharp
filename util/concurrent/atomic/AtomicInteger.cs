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
	/// An {@code int} value that may be updated atomically.  See the
	/// <seealso cref="java.util.concurrent.atomic"/> package specification for
	/// description of the properties of atomic variables. An
	/// {@code AtomicInteger} is used in applications such as atomically
	/// incremented counters, and cannot be used as a replacement for an
	/// <seealso cref="java.lang.Integer"/>. However, this class does extend
	/// {@code Number} to allow uniform access by tools and utilities that
	/// deal with numerically-based classes.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </summary>
	[Serializable]
	public class AtomicInteger : Number
	{
		private const long SerialVersionUID = 6214790243416807050L;

		// setup to use Unsafe.compareAndSwapInt for updates
		private static readonly Unsafe @unsafe = Unsafe.Unsafe;
		private static readonly long ValueOffset;

		static AtomicInteger()
		{
			try
			{
				ValueOffset = @unsafe.objectFieldOffset(typeof(AtomicInteger).getDeclaredField("value"));
			}
			catch (Exception ex)
			{
				throw new Error(ex);
			}
		}

		private volatile int Value;

		/// <summary>
		/// Creates a new AtomicInteger with the given initial value.
		/// </summary>
		/// <param name="initialValue"> the initial value </param>
		public AtomicInteger(int initialValue)
		{
			Value = initialValue;
		}

		/// <summary>
		/// Creates a new AtomicInteger with initial value {@code 0}.
		/// </summary>
		public AtomicInteger()
		{
		}

		/// <summary>
		/// Gets the current value.
		/// </summary>
		/// <returns> the current value </returns>
		public int Get()
		{
			return Value;
		}

		/// <summary>
		/// Sets to the given value.
		/// </summary>
		/// <param name="newValue"> the new value </param>
		public void Set(int newValue)
		{
			Value = newValue;
		}

		/// <summary>
		/// Eventually sets to the given value.
		/// </summary>
		/// <param name="newValue"> the new value
		/// @since 1.6 </param>
		public void LazySet(int newValue)
		{
			@unsafe.putOrderedInt(this, ValueOffset, newValue);
		}

		/// <summary>
		/// Atomically sets to the given value and returns the old value.
		/// </summary>
		/// <param name="newValue"> the new value </param>
		/// <returns> the previous value </returns>
		public int GetAndSet(int newValue)
		{
			return @unsafe.getAndSetInt(this, ValueOffset, newValue);
		}

		/// <summary>
		/// Atomically sets the value to the given updated value
		/// if the current value {@code ==} the expected value.
		/// </summary>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful. False return indicates that
		/// the actual value was not equal to the expected value. </returns>
		public bool CompareAndSet(int expect, int update)
		{
			return @unsafe.compareAndSwapInt(this, ValueOffset, expect, update);
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
		public bool WeakCompareAndSet(int expect, int update)
		{
			return @unsafe.compareAndSwapInt(this, ValueOffset, expect, update);
		}

		/// <summary>
		/// Atomically increments by one the current value.
		/// </summary>
		/// <returns> the previous value </returns>
		public int AndIncrement
		{
			get
			{
				return @unsafe.getAndAddInt(this, ValueOffset, 1);
			}
		}

		/// <summary>
		/// Atomically decrements by one the current value.
		/// </summary>
		/// <returns> the previous value </returns>
		public int AndDecrement
		{
			get
			{
				return @unsafe.getAndAddInt(this, ValueOffset, -1);
			}
		}

		/// <summary>
		/// Atomically adds the given value to the current value.
		/// </summary>
		/// <param name="delta"> the value to add </param>
		/// <returns> the previous value </returns>
		public int GetAndAdd(int delta)
		{
			return @unsafe.getAndAddInt(this, ValueOffset, delta);
		}

		/// <summary>
		/// Atomically increments by one the current value.
		/// </summary>
		/// <returns> the updated value </returns>
		public int IncrementAndGet()
		{
			return @unsafe.getAndAddInt(this, ValueOffset, 1) + 1;
		}

		/// <summary>
		/// Atomically decrements by one the current value.
		/// </summary>
		/// <returns> the updated value </returns>
		public int DecrementAndGet()
		{
			return @unsafe.getAndAddInt(this, ValueOffset, -1) - 1;
		}

		/// <summary>
		/// Atomically adds the given value to the current value.
		/// </summary>
		/// <param name="delta"> the value to add </param>
		/// <returns> the updated value </returns>
		public int AddAndGet(int delta)
		{
			return @unsafe.getAndAddInt(this, ValueOffset, delta) + delta;
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
		public int GetAndUpdate(IntUnaryOperator updateFunction)
		{
			int prev, next;
			do
			{
				prev = Get();
				next = updateFunction.ApplyAsInt(prev);
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
		public int UpdateAndGet(IntUnaryOperator updateFunction)
		{
			int prev, next;
			do
			{
				prev = Get();
				next = updateFunction.ApplyAsInt(prev);
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
		public int GetAndAccumulate(int x, IntBinaryOperator accumulatorFunction)
		{
			int prev, next;
			do
			{
				prev = Get();
				next = accumulatorFunction.ApplyAsInt(prev, x);
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
		public int AccumulateAndGet(int x, IntBinaryOperator accumulatorFunction)
		{
			int prev, next;
			do
			{
				prev = Get();
				next = accumulatorFunction.ApplyAsInt(prev, x);
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

		/// <summary>
		/// Returns the value of this {@code AtomicInteger} as an {@code int}.
		/// </summary>
		public override int IntValue()
		{
			return Get();
		}

		/// <summary>
		/// Returns the value of this {@code AtomicInteger} as a {@code long}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override long LongValue()
		{
			return (long)Get();
		}

		/// <summary>
		/// Returns the value of this {@code AtomicInteger} as a {@code float}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override float FloatValue()
		{
			return (float)Get();
		}

		/// <summary>
		/// Returns the value of this {@code AtomicInteger} as a {@code double}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override double DoubleValue()
		{
			return (double)Get();
		}

	}

}