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
	/// An array of object references in which elements may be updated
	/// atomically.  See the <seealso cref="java.util.concurrent.atomic"/> package
	/// specification for description of the properties of atomic
	/// variables.
	/// @since 1.5
	/// @author Doug Lea </summary>
	/// @param <E> The base class of elements held in this array </param>
	[Serializable]
	public class AtomicReferenceArray<E>
	{
		private const long SerialVersionUID = -6209656149925076980L;

		private static readonly Unsafe @unsafe;
		private static readonly int @base;
		private static readonly int Shift;
		private static readonly long ArrayFieldOffset;
		private readonly Object[] Array; // must have exact type Object[]

		static AtomicReferenceArray()
		{
			try
			{
				@unsafe = Unsafe.Unsafe;
				ArrayFieldOffset = @unsafe.objectFieldOffset(typeof(AtomicReferenceArray).getDeclaredField("array"));
				@base = @unsafe.arrayBaseOffset(typeof(Object[]));
				int scale = @unsafe.arrayIndexScale(typeof(Object[]));
				if ((scale & (scale - 1)) != 0)
				{
					throw new Error("data type scale not a power of two");
				}
				Shift = 31 - Integer.NumberOfLeadingZeros(scale);
			}
			catch (Exception e)
			{
				throw new Error(e);
			}
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
		/// Creates a new AtomicReferenceArray of the given length, with all
		/// elements initially null.
		/// </summary>
		/// <param name="length"> the length of the array </param>
		public AtomicReferenceArray(int length)
		{
			Array = new Object[length];
		}

		/// <summary>
		/// Creates a new AtomicReferenceArray with the same length as, and
		/// all elements copied from, the given array.
		/// </summary>
		/// <param name="array"> the array to copy elements from </param>
		/// <exception cref="NullPointerException"> if array is null </exception>
		public AtomicReferenceArray(E[] array)
		{
			// Visibility guaranteed by final field guarantees
			this.Array = Arrays.CopyOf(array, array.Length, typeof(Object[]));
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
		public E Get(int i)
		{
			return GetRaw(CheckedByteOffset(i));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private E getRaw(long offset)
		private E GetRaw(long offset)
		{
			return (E) @unsafe.getObjectVolatile(Array, offset);
		}

		/// <summary>
		/// Sets the element at position {@code i} to the given value.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="newValue"> the new value </param>
		public void Set(int i, E newValue)
		{
			@unsafe.putObjectVolatile(Array, CheckedByteOffset(i), newValue);
		}

		/// <summary>
		/// Eventually sets the element at position {@code i} to the given value.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="newValue"> the new value
		/// @since 1.6 </param>
		public void LazySet(int i, E newValue)
		{
			@unsafe.putOrderedObject(Array, CheckedByteOffset(i), newValue);
		}

		/// <summary>
		/// Atomically sets the element at position {@code i} to the given
		/// value and returns the old value.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="newValue"> the new value </param>
		/// <returns> the previous value </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public final E getAndSet(int i, E newValue)
		public E GetAndSet(int i, E newValue)
		{
			return (E)@unsafe.getAndSetObject(Array, CheckedByteOffset(i), newValue);
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
		public bool CompareAndSet(int i, E expect, E update)
		{
			return CompareAndSetRaw(CheckedByteOffset(i), expect, update);
		}

		private bool CompareAndSetRaw(long offset, E expect, E update)
		{
			return @unsafe.compareAndSwapObject(Array, offset, expect, update);
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
		public bool WeakCompareAndSet(int i, E expect, E update)
		{
			return CompareAndSet(i, expect, update);
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
		public E GetAndUpdate(int i, UnaryOperator<E> updateFunction)
		{
			long offset = CheckedByteOffset(i);
			E prev, next;
			do
			{
				prev = GetRaw(offset);
				next = updateFunction.Apply(prev);
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
		public E UpdateAndGet(int i, UnaryOperator<E> updateFunction)
		{
			long offset = CheckedByteOffset(i);
			E prev, next;
			do
			{
				prev = GetRaw(offset);
				next = updateFunction.Apply(prev);
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
		public E GetAndAccumulate(int i, E x, BinaryOperator<E> accumulatorFunction)
		{
			long offset = CheckedByteOffset(i);
			E prev, next;
			do
			{
				prev = GetRaw(offset);
				next = accumulatorFunction.Apply(prev, x);
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
		public E AccumulateAndGet(int i, E x, BinaryOperator<E> accumulatorFunction)
		{
			long offset = CheckedByteOffset(i);
			E prev, next;
			do
			{
				prev = GetRaw(offset);
				next = accumulatorFunction.Apply(prev, x);
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

		/// <summary>
		/// Reconstitutes the instance from a stream (that is, deserializes it).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException, java.io.InvalidObjectException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			// Note: This must be changed if any additional fields are defined
			Object a = s.ReadFields().Get("array", null);
			if (a == null || !a.GetType().IsArray)
			{
				throw new java.io.InvalidObjectException("Not array type");
			}
			if (a.GetType() != typeof(Object[]))
			{
				a = Arrays.CopyOf((Object[])a, Array.getLength(a), typeof(Object[]));
			}
			@unsafe.putObjectVolatile(this, ArrayFieldOffset, a);
		}

	}

}