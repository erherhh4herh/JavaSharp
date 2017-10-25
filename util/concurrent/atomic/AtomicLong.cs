using System;
using System.Runtime.InteropServices;

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
	/// A {@code long} value that may be updated atomically.  See the
	/// <seealso cref="java.util.concurrent.atomic"/> package specification for
	/// description of the properties of atomic variables. An
	/// {@code AtomicLong} is used in applications such as atomically
	/// incremented sequence numbers, and cannot be used as a replacement
	/// for a <seealso cref="java.lang.Long"/>. However, this class does extend
	/// {@code Number} to allow uniform access by tools and utilities that
	/// deal with numerically-based classes.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </summary>
	[Serializable]
	public class AtomicLong : Number
	{
		private const long SerialVersionUID = 1927816293512124184L;

		// setup to use Unsafe.compareAndSwapLong for updates
		private static readonly Unsafe @unsafe = Unsafe.Unsafe;
		private static readonly long ValueOffset;

		/// <summary>
		/// Records whether the underlying JVM supports lockless
		/// compareAndSwap for longs. While the Unsafe.compareAndSwapLong
		/// method works in either case, some constructions should be
		/// handled at Java level to avoid locking user-visible locks.
		/// </summary>
		internal static readonly bool VM_SUPPORTS_LONG_CAS = VMSupportsCS8();

		/// <summary>
		/// Returns whether underlying JVM supports lockless CompareAndSet
		/// for longs. Called only once and cached in VM_SUPPORTS_LONG_CAS.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern boolean VMSupportsCS8();

		static AtomicLong()
		{
			try
			{
				ValueOffset = @unsafe.objectFieldOffset(typeof(AtomicLong).getDeclaredField("value"));
			}
			catch (Exception ex)
			{
				throw new Error(ex);
			}
		}

		private volatile long Value;

		/// <summary>
		/// Creates a new AtomicLong with the given initial value.
		/// </summary>
		/// <param name="initialValue"> the initial value </param>
		public AtomicLong(long initialValue)
		{
			Value = initialValue;
		}

		/// <summary>
		/// Creates a new AtomicLong with initial value {@code 0}.
		/// </summary>
		public AtomicLong()
		{
		}

		/// <summary>
		/// Gets the current value.
		/// </summary>
		/// <returns> the current value </returns>
		public long Get()
		{
			return Value;
		}

		/// <summary>
		/// Sets to the given value.
		/// </summary>
		/// <param name="newValue"> the new value </param>
		public void Set(long newValue)
		{
			Value = newValue;
		}

		/// <summary>
		/// Eventually sets to the given value.
		/// </summary>
		/// <param name="newValue"> the new value
		/// @since 1.6 </param>
		public void LazySet(long newValue)
		{
			@unsafe.putOrderedLong(this, ValueOffset, newValue);
		}

		/// <summary>
		/// Atomically sets to the given value and returns the old value.
		/// </summary>
		/// <param name="newValue"> the new value </param>
		/// <returns> the previous value </returns>
		public long GetAndSet(long newValue)
		{
			return @unsafe.getAndSetLong(this, ValueOffset, newValue);
		}

		/// <summary>
		/// Atomically sets the value to the given updated value
		/// if the current value {@code ==} the expected value.
		/// </summary>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful. False return indicates that
		/// the actual value was not equal to the expected value. </returns>
		public bool CompareAndSet(long expect, long update)
		{
			return @unsafe.compareAndSwapLong(this, ValueOffset, expect, update);
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
		public bool WeakCompareAndSet(long expect, long update)
		{
			return @unsafe.compareAndSwapLong(this, ValueOffset, expect, update);
		}

		/// <summary>
		/// Atomically increments by one the current value.
		/// </summary>
		/// <returns> the previous value </returns>
		public long AndIncrement
		{
			get
			{
				return @unsafe.getAndAddLong(this, ValueOffset, 1L);
			}
		}

		/// <summary>
		/// Atomically decrements by one the current value.
		/// </summary>
		/// <returns> the previous value </returns>
		public long AndDecrement
		{
			get
			{
				return @unsafe.getAndAddLong(this, ValueOffset, -1L);
			}
		}

		/// <summary>
		/// Atomically adds the given value to the current value.
		/// </summary>
		/// <param name="delta"> the value to add </param>
		/// <returns> the previous value </returns>
		public long GetAndAdd(long delta)
		{
			return @unsafe.getAndAddLong(this, ValueOffset, delta);
		}

		/// <summary>
		/// Atomically increments by one the current value.
		/// </summary>
		/// <returns> the updated value </returns>
		public long IncrementAndGet()
		{
			return @unsafe.getAndAddLong(this, ValueOffset, 1L) + 1L;
		}

		/// <summary>
		/// Atomically decrements by one the current value.
		/// </summary>
		/// <returns> the updated value </returns>
		public long DecrementAndGet()
		{
			return @unsafe.getAndAddLong(this, ValueOffset, -1L) - 1L;
		}

		/// <summary>
		/// Atomically adds the given value to the current value.
		/// </summary>
		/// <param name="delta"> the value to add </param>
		/// <returns> the updated value </returns>
		public long AddAndGet(long delta)
		{
			return @unsafe.getAndAddLong(this, ValueOffset, delta) + delta;
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
		public long GetAndUpdate(LongUnaryOperator updateFunction)
		{
			long prev, next;
			do
			{
				prev = Get();
				next = updateFunction.ApplyAsLong(prev);
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
		public long UpdateAndGet(LongUnaryOperator updateFunction)
		{
			long prev, next;
			do
			{
				prev = Get();
				next = updateFunction.ApplyAsLong(prev);
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
		public long GetAndAccumulate(long x, LongBinaryOperator accumulatorFunction)
		{
			long prev, next;
			do
			{
				prev = Get();
				next = accumulatorFunction.ApplyAsLong(prev, x);
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
		public long AccumulateAndGet(long x, LongBinaryOperator accumulatorFunction)
		{
			long prev, next;
			do
			{
				prev = Get();
				next = accumulatorFunction.ApplyAsLong(prev, x);
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
		/// Returns the value of this {@code AtomicLong} as an {@code int}
		/// after a narrowing primitive conversion.
		/// @jls 5.1.3 Narrowing Primitive Conversions
		/// </summary>
		public override int IntValue()
		{
			return (int)Get();
		}

		/// <summary>
		/// Returns the value of this {@code AtomicLong} as a {@code long}.
		/// </summary>
		public override long LongValue()
		{
			return Get();
		}

		/// <summary>
		/// Returns the value of this {@code AtomicLong} as a {@code float}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override float FloatValue()
		{
			return (float)Get();
		}

		/// <summary>
		/// Returns the value of this {@code AtomicLong} as a {@code double}
		/// after a widening primitive conversion.
		/// @jls 5.1.2 Widening Primitive Conversions
		/// </summary>
		public override double DoubleValue()
		{
			return (double)Get();
		}

	}

}