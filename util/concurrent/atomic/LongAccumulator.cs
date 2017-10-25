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

	/// <summary>
	/// One or more variables that together maintain a running {@code long}
	/// value updated using a supplied function.  When updates (method
	/// <seealso cref="#accumulate"/>) are contended across threads, the set of variables
	/// may grow dynamically to reduce contention.  Method <seealso cref="#get"/>
	/// (or, equivalently, <seealso cref="#longValue"/>) returns the current value
	/// across the variables maintaining updates.
	/// 
	/// <para>This class is usually preferable to <seealso cref="AtomicLong"/> when
	/// multiple threads update a common value that is used for purposes such
	/// as collecting statistics, not for fine-grained synchronization
	/// control.  Under low update contention, the two classes have similar
	/// characteristics. But under high contention, expected throughput of
	/// this class is significantly higher, at the expense of higher space
	/// consumption.
	/// 
	/// </para>
	/// <para>The order of accumulation within or across threads is not
	/// guaranteed and cannot be depended upon, so this class is only
	/// applicable to functions for which the order of accumulation does
	/// not matter. The supplied accumulator function should be
	/// side-effect-free, since it may be re-applied when attempted updates
	/// fail due to contention among threads. The function is applied with
	/// the current value as its first argument, and the given update as
	/// the second argument.  For example, to maintain a running maximum
	/// value, you could supply {@code Long::max} along with {@code
	/// Long.MIN_VALUE} as the identity.
	/// 
	/// </para>
	/// <para>Class <seealso cref="LongAdder"/> provides analogs of the functionality of
	/// this class for the common special case of maintaining counts and
	/// sums.  The call {@code new LongAdder()} is equivalent to {@code new
	/// LongAccumulator((x, y) -> x + y, 0L}.
	/// 
	/// </para>
	/// <para>This class extends <seealso cref="Number"/>, but does <em>not</em> define
	/// methods such as {@code equals}, {@code hashCode} and {@code
	/// compareTo} because instances are expected to be mutated, and so are
	/// not useful as collection keys.
	/// 
	/// @since 1.8
	/// @author Doug Lea
	/// </para>
	/// </summary>
	[Serializable]
	public class LongAccumulator : Striped64
	{
		private const long SerialVersionUID = 7249069246863182397L;

		private readonly LongBinaryOperator Function;
		private readonly long Identity;

		/// <summary>
		/// Creates a new instance using the given accumulator function
		/// and identity element. </summary>
		/// <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		/// <param name="identity"> identity (initial value) for the accumulator function </param>
		public LongAccumulator(LongBinaryOperator accumulatorFunction, long identity)
		{
			this.Function = accumulatorFunction;
			@base = this.Identity = identity;
		}

		/// <summary>
		/// Updates with the given value.
		/// </summary>
		/// <param name="x"> the value </param>
		public virtual void Accumulate(long x)
		{
			Cell[] @as;
			long b, v, r;
			int m;
			Cell a;
			if ((@as = Cells) != null || (r = Function.ApplyAsLong(b = @base, x)) != b && !CasBase(b, r))
			{
				bool uncontended = true;
				if (@as == null || (m = @as.Length - 1) < 0 || (a = @as[Probe & m]) == null || !(uncontended = (r = Function.ApplyAsLong(v = a.Value, x)) == v || a.Cas(v, r)))
				{
					LongAccumulate(x, Function, uncontended);
				}
			}
		}

		/// <summary>
		/// Returns the current value.  The returned value is <em>NOT</em>
		/// an atomic snapshot; invocation in the absence of concurrent
		/// updates returns an accurate result, but concurrent updates that
		/// occur while the value is being calculated might not be
		/// incorporated.
		/// </summary>
		/// <returns> the current value </returns>
		public virtual long Get()
		{
			Cell[] @as = Cells;
			Cell a;
			long result = @base;
			if (@as != null)
			{
				for (int i = 0; i < @as.Length; ++i)
				{
					if ((a = @as[i]) != null)
					{
						result = Function.ApplyAsLong(result, a.Value);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Resets variables maintaining updates to the identity value.
		/// This method may be a useful alternative to creating a new
		/// updater, but is only effective if there are no concurrent
		/// updates.  Because this method is intrinsically racy, it should
		/// only be used when it is known that no threads are concurrently
		/// updating.
		/// </summary>
		public virtual void Reset()
		{
			Cell[] @as = Cells;
			Cell a;
			@base = Identity;
			if (@as != null)
			{
				for (int i = 0; i < @as.Length; ++i)
				{
					if ((a = @as[i]) != null)
					{
						a.Value = Identity;
					}
				}
			}
		}

		/// <summary>
		/// Equivalent in effect to <seealso cref="#get"/> followed by {@link
		/// #reset}. This method may apply for example during quiescent
		/// points between multithreaded computations.  If there are
		/// updates concurrent with this method, the returned value is
		/// <em>not</em> guaranteed to be the final value occurring before
		/// the reset.
		/// </summary>
		/// <returns> the value before reset </returns>
		public virtual long ThenReset
		{
			get
			{
				Cell[] @as = Cells;
				Cell a;
				long result = @base;
				@base = Identity;
				if (@as != null)
				{
					for (int i = 0; i < @as.Length; ++i)
					{
						if ((a = @as[i]) != null)
						{
							long v = a.Value;
							a.Value = Identity;
							result = Function.ApplyAsLong(result, v);
						}
					}
				}
				return result;
			}
		}

		/// <summary>
		/// Returns the String representation of the current value. </summary>
		/// <returns> the String representation of the current value </returns>
		public override String ToString()
		{
			return Convert.ToString(Get());
		}

		/// <summary>
		/// Equivalent to <seealso cref="#get"/>.
		/// </summary>
		/// <returns> the current value </returns>
		public override long LongValue()
		{
			return Get();
		}

		/// <summary>
		/// Returns the <seealso cref="#get current value"/> as an {@code int}
		/// after a narrowing primitive conversion.
		/// </summary>
		public override int IntValue()
		{
			return (int)Get();
		}

		/// <summary>
		/// Returns the <seealso cref="#get current value"/> as a {@code float}
		/// after a widening primitive conversion.
		/// </summary>
		public override float FloatValue()
		{
			return (float)Get();
		}

		/// <summary>
		/// Returns the <seealso cref="#get current value"/> as a {@code double}
		/// after a widening primitive conversion.
		/// </summary>
		public override double DoubleValue()
		{
			return (double)Get();
		}

		/// <summary>
		/// Serialization proxy, used to avoid reference to the non-public
		/// Striped64 superclass in serialized forms.
		/// @serial include
		/// </summary>
		[Serializable]
		private class SerializationProxy
		{
			internal const long SerialVersionUID = 7249069246863182397L;

			/// <summary>
			/// The current value returned by get().
			/// @serial
			/// </summary>
			internal readonly long Value;
			/// <summary>
			/// The function used for updates.
			/// @serial
			/// </summary>
			internal readonly LongBinaryOperator Function;
			/// <summary>
			/// The identity value
			/// @serial
			/// </summary>
			internal readonly long Identity;

			internal SerializationProxy(LongAccumulator a)
			{
				Function = a.Function;
				Identity = a.Identity;
				Value = a.Get();
			}

			/// <summary>
			/// Returns a {@code LongAccumulator} object with initial state
			/// held by this proxy.
			/// </summary>
			/// <returns> a {@code LongAccumulator} object with initial state
			/// held by this proxy. </returns>
			internal virtual Object ReadResolve()
			{
				LongAccumulator a = new LongAccumulator(Function, Identity);
				a.@base = Value;
				return a;
			}
		}

		/// <summary>
		/// Returns a
		/// <a href="../../../../serialized-form.html#java.util.concurrent.atomic.LongAccumulator.SerializationProxy">
		/// SerializationProxy</a>
		/// representing the state of this instance.
		/// </summary>
		/// <returns> a <seealso cref="SerializationProxy"/>
		/// representing the state of this instance </returns>
		private Object WriteReplace()
		{
			return new SerializationProxy(this);
		}

		/// <param name="s"> the stream </param>
		/// <exception cref="java.io.InvalidObjectException"> always </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.InvalidObjectException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			throw new java.io.InvalidObjectException("Proxy required");
		}

	}

}