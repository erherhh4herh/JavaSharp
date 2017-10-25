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
	/// An {@code int} array in which elements may be updated atomically.
	/// See the <seealso cref="java.util.concurrent.atomic"/> package
	/// specification for description of the properties of atomic
	/// variables.
	/// @since 1.5
	/// @author Doug Lea
	/// </summary>
	[Serializable]
	public class AtomicIntegerArray
	{
		private const long SerialVersionUID = 2862133569453604235L;

		private static readonly Unsafe @unsafe = Unsafe.Unsafe;
		private static readonly int @base = @unsafe.arrayBaseOffset(typeof(int[]));
		private static readonly int Shift;
		private readonly int[] Array;

		static AtomicIntegerArray()
		{
			int scale = @unsafe.arrayIndexScale(typeof(int[]));
			if ((scale & (scale - 1)) != 0)
			{
				throw new Error("data type scale not a power of two");
			}
			Shift = 31 - Integer.NumberOfLeadingZeros(scale);
		}

		private long CheckedByteOffset(int i)
		{
			if (i < 0 || i >= Array.Length)
			{
				throw new IndexOutOfBoundsException("index " + i);
			}

			return ByteOffset(i);
		}

		private static long ByteOffset(int i)
		{
			return ((long) i << Shift) + @base;
		}

		/// <summary>
		/// Creates a new AtomicIntegerArray of the given length, with all
		/// elements initially zero.
		/// </summary>
		/// <param name="length"> the length of the array </param>
		public AtomicIntegerArray(int length)
		{
			Array = new int[length];
		}

		/// <summary>
		/// Creates a new AtomicIntegerArray with the same length as, and
		/// all elements copied from, the given array.
		/// </summary>
		/// <param name="array"> the array to copy elements from </param>
		/// <exception cref="NullPointerException"> if array is null </exception>
		public AtomicIntegerArray(int[] array)
		{
			// Visibility guaranteed by final field guarantees
			this.Array = array.clone();
		}

		/// <summary>
		/// Returns the length of the array.
		/// </summary>
		/// <returns> the length of the array </returns>
		public int Length()
		{
			return Array.Length;
		}

		/// <summary>
		/// Gets the current value at position {@code i}.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <returns> the current value </returns>
		public int Get(int i)
		{
			return GetRaw(CheckedByteOffset(i));
		}

		private int GetRaw(long offset)
		{
			return @unsafe.getIntVolatile(Array, offset);
		}

		/// <summary>
		/// Sets the element at position {@code i} to the given value.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="newValue"> the new value </param>
		public void Set(int i, int newValue)
		{
			@unsafe.putIntVolatile(Array, CheckedByteOffset(i), newValue);
		}

		/// <summary>
		/// Eventually sets the element at position {@code i} to the given value.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="newValue"> the new value
		/// @since 1.6 </param>
		public void LazySet(int i, int newValue)
		{
			@unsafe.putOrderedInt(Array, CheckedByteOffset(i), newValue);
		}

		/// <summary>
		/// Atomically sets the element at position {@code i} to the given
		/// value and returns the old value.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="newValue"> the new value </param>
		/// <returns> the previous value </returns>
		public int GetAndSet(int i, int newValue)
		{
			return @unsafe.getAndSetInt(Array, CheckedByteOffset(i), newValue);
		}

		/// <summary>
		/// Atomically sets the element at position {@code i} to the given
		/// updated value if the current value {@code ==} the expected value.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful. False return indicates that
		/// the actual value was not equal to the expected value. </returns>
		public bool CompareAndSet(int i, int expect, int update)
		{
			return CompareAndSetRaw(CheckedByteOffset(i), expect, update);
		}

		private bool CompareAndSetRaw(long offset, int expect, int update)
		{
			return @unsafe.compareAndSwapInt(Array, offset, expect, update);
		}

		/// <summary>
		/// Atomically sets the element at position {@code i} to the given
		/// updated value if the current value {@code ==} the expected value.
		/// 
		/// <para><a href="package-summary.html#weakCompareAndSet">May fail
		/// spuriously and does not provide ordering guarantees</a>, so is
		/// only rarely an appropriate alternative to {@code compareAndSet}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful </returns>
		public bool WeakCompareAndSet(int i, int expect, int update)
		{
			return CompareAndSet(i, expect, update);
		}

		/// <summary>
		/// Atomically increments by one the element at index {@code i}.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <returns> the previous value </returns>
		public int GetAndIncrement(int i)
		{
			return GetAndAdd(i, 1);
		}

		/// <summary>
		/// Atomically decrements by one the element at index {@code i}.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <returns> the previous value </returns>
		public int GetAndDecrement(int i)
		{
			return GetAndAdd(i, -1);
		}

		/// <summary>
		/// Atomically adds the given value to the element at index {@code i}.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="delta"> the value to add </param>
		/// <returns> the previous value </returns>
		public int GetAndAdd(int i, int delta)
		{
			return @unsafe.getAndAddInt(Array, CheckedByteOffset(i), delta);
		}

		/// <summary>
		/// Atomically increments by one the element at index {@code i}.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <returns> the updated value </returns>
		public int IncrementAndGet(int i)
		{
			return GetAndAdd(i, 1) + 1;
		}

		/// <summary>
		/// Atomically decrements by one the element at index {@code i}.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <returns> the updated value </returns>
		public int DecrementAndGet(int i)
		{
			return GetAndAdd(i, -1) - 1;
		}

		/// <summary>
		/// Atomically adds the given value to the element at index {@code i}.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="delta"> the value to add </param>
		/// <returns> the updated value </returns>
		public int AddAndGet(int i, int delta)
		{
			return GetAndAdd(i, delta) + delta;
		}


		/// <summary>
		/// Atomically updates the element at index {@code i} with the results
		/// of applying the given function, returning the previous value. The
		/// function should be side-effect-free, since it may be re-applied
		/// when attempted updates fail due to contention among threads.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="updateFunction"> a side-effect-free function </param>
		/// <returns> the previous value
		/// @since 1.8 </returns>
		public int GetAndUpdate(int i, IntUnaryOperator updateFunction)
		{
			long offset = CheckedByteOffset(i);
			int prev, next;
			do
			{
				prev = GetRaw(offset);
				next = updateFunction.ApplyAsInt(prev);
			} while (!CompareAndSetRaw(offset, prev, next));
			return prev;
		}

		/// <summary>
		/// Atomically updates the element at index {@code i} with the results
		/// of applying the given function, returning the updated value. The
		/// function should be side-effect-free, since it may be re-applied
		/// when attempted updates fail due to contention among threads.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="updateFunction"> a side-effect-free function </param>
		/// <returns> the updated value
		/// @since 1.8 </returns>
		public int UpdateAndGet(int i, IntUnaryOperator updateFunction)
		{
			long offset = CheckedByteOffset(i);
			int prev, next;
			do
			{
				prev = GetRaw(offset);
				next = updateFunction.ApplyAsInt(prev);
			} while (!CompareAndSetRaw(offset, prev, next));
			return next;
		}

		/// <summary>
		/// Atomically updates the element at index {@code i} with the
		/// results of applying the given function to the current and
		/// given values, returning the previous value. The function should
		/// be side-effect-free, since it may be re-applied when attempted
		/// updates fail due to contention among threads.  The function is
		/// applied with the current value at index {@code i} as its first
		/// argument, and the given update as the second argument.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="x"> the update value </param>
		/// <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		/// <returns> the previous value
		/// @since 1.8 </returns>
		public int GetAndAccumulate(int i, int x, IntBinaryOperator accumulatorFunction)
		{
			long offset = CheckedByteOffset(i);
			int prev, next;
			do
			{
				prev = GetRaw(offset);
				next = accumulatorFunction.ApplyAsInt(prev, x);
			} while (!CompareAndSetRaw(offset, prev, next));
			return prev;
		}

		/// <summary>
		/// Atomically updates the element at index {@code i} with the
		/// results of applying the given function to the current and
		/// given values, returning the updated value. The function should
		/// be side-effect-free, since it may be re-applied when attempted
		/// updates fail due to contention among threads.  The function is
		/// applied with the current value at index {@code i} as its first
		/// argument, and the given update as the second argument.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="x"> the update value </param>
		/// <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		/// <returns> the updated value
		/// @since 1.8 </returns>
		public int AccumulateAndGet(int i, int x, IntBinaryOperator accumulatorFunction)
		{
			long offset = CheckedByteOffset(i);
			int prev, next;
			do
			{
				prev = GetRaw(offset);
				next = accumulatorFunction.ApplyAsInt(prev, x);
			} while (!CompareAndSetRaw(offset, prev, next));
			return next;
		}

		/// <summary>
		/// Returns the String representation of the current values of array. </summary>
		/// <returns> the String representation of the current values of array </returns>
		public override String ToString()
		{
			int iMax = Array.Length - 1;
			if (iMax == -1)
			{
				return "[]";
			}

			StringBuilder b = new StringBuilder();
			b.Append('[');
			for (int i = 0; ; i++)
			{
				b.Append(GetRaw(ByteOffset(i)));
				if (i == iMax)
				{
					return b.Append(']').ToString();
				}
				b.Append(',').Append(' ');
			}
		}

	}

}