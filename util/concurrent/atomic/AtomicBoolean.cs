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
	/// A {@code boolean} value that may be updated atomically. See the
	/// <seealso cref="java.util.concurrent.atomic"/> package specification for
	/// description of the properties of atomic variables. An
	/// {@code AtomicBoolean} is used in applications such as atomically
	/// updated flags, and cannot be used as a replacement for a
	/// <seealso cref="java.lang.Boolean"/>.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </summary>
	[Serializable]
	public class AtomicBoolean
	{
		private const long SerialVersionUID = 4654671469794556979L;
		// setup to use Unsafe.compareAndSwapInt for updates
		private static readonly Unsafe @unsafe = Unsafe.Unsafe;
		private static readonly long ValueOffset;

		static AtomicBoolean()
		{
			try
			{
				ValueOffset = @unsafe.objectFieldOffset(typeof(AtomicBoolean).getDeclaredField("value"));
			}
			catch (Exception ex)
			{
				throw new Error(ex);
			}
		}

		private volatile int Value;

		/// <summary>
		/// Creates a new {@code AtomicBoolean} with the given initial value.
		/// </summary>
		/// <param name="initialValue"> the initial value </param>
		public AtomicBoolean(bool initialValue)
		{
			Value = initialValue ? 1 : 0;
		}

		/// <summary>
		/// Creates a new {@code AtomicBoolean} with initial value {@code false}.
		/// </summary>
		public AtomicBoolean()
		{
		}

		/// <summary>
		/// Returns the current value.
		/// </summary>
		/// <returns> the current value </returns>
		public bool Get()
		{
			return Value != 0;
		}

		/// <summary>
		/// Atomically sets the value to the given updated value
		/// if the current value {@code ==} the expected value.
		/// </summary>
		/// <param name="expect"> the expected value </param>
		/// <param name="update"> the new value </param>
		/// <returns> {@code true} if successful. False return indicates that
		/// the actual value was not equal to the expected value. </returns>
		public bool CompareAndSet(bool expect, bool update)
		{
			int e = expect ? 1 : 0;
			int u = update ? 1 : 0;
			return @unsafe.compareAndSwapInt(this, ValueOffset, e, u);
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
		public virtual bool WeakCompareAndSet(bool expect, bool update)
		{
			int e = expect ? 1 : 0;
			int u = update ? 1 : 0;
			return @unsafe.compareAndSwapInt(this, ValueOffset, e, u);
		}

		/// <summary>
		/// Unconditionally sets to the given value.
		/// </summary>
		/// <param name="newValue"> the new value </param>
		public void Set(bool newValue)
		{
			Value = newValue ? 1 : 0;
		}

		/// <summary>
		/// Eventually sets to the given value.
		/// </summary>
		/// <param name="newValue"> the new value
		/// @since 1.6 </param>
		public void LazySet(bool newValue)
		{
			int v = newValue ? 1 : 0;
			@unsafe.putOrderedInt(this, ValueOffset, v);
		}

		/// <summary>
		/// Atomically sets to the given value and returns the previous value.
		/// </summary>
		/// <param name="newValue"> the new value </param>
		/// <returns> the previous value </returns>
		public bool GetAndSet(bool newValue)
		{
			bool prev;
			do
			{
				prev = Get();
			} while (!CompareAndSet(prev, newValue));
			return prev;
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