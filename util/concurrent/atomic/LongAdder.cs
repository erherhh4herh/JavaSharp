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
	/// One or more variables that together maintain an initially zero
	/// {@code long} sum.  When updates (method <seealso cref="#add"/>) are contended
	/// across threads, the set of variables may grow dynamically to reduce
	/// contention. Method <seealso cref="#sum"/> (or, equivalently, {@link
	/// #longValue}) returns the current total combined across the
	/// variables maintaining the sum.
	/// 
	/// <para>This class is usually preferable to <seealso cref="AtomicLong"/> when
	/// multiple threads update a common sum that is used for purposes such
	/// as collecting statistics, not for fine-grained synchronization
	/// control.  Under low update contention, the two classes have similar
	/// characteristics. But under high contention, expected throughput of
	/// this class is significantly higher, at the expense of higher space
	/// consumption.
	/// 
	/// </para>
	/// <para>LongAdders can be used with a {@link
	/// java.util.concurrent.ConcurrentHashMap} to maintain a scalable
	/// frequency map (a form of histogram or multiset). For example, to
	/// add a count to a {@code ConcurrentHashMap<String,LongAdder> freqs},
	/// initializing if not already present, you can use {@code
	/// freqs.computeIfAbsent(k -> new LongAdder()).increment();}
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
	public class LongAdder : Striped64
	{
		private const long SerialVersionUID = 7249069246863182397L;

		/// <summary>
		/// Creates a new adder with initial sum of zero.
		/// </summary>
		public LongAdder()
		{
		}

		/// <summary>
		/// Adds the given value.
		/// </summary>
		/// <param name="x"> the value to add </param>
		public virtual void Add(long x)
		{
			Cell[] @as;
			long b, v;
			int m;
			Cell a;
			if ((@as = Cells) != null || !CasBase(b = @base, b + x))
			{
				bool uncontended = true;
				if (@as == null || (m = @as.Length - 1) < 0 || (a = @as[Probe & m]) == null || !(uncontended = a.Cas(v = a.Value, v + x)))
				{
					LongAccumulate(x, null, uncontended);
				}
			}
		}

		/// <summary>
		/// Equivalent to {@code add(1)}.
		/// </summary>
		public virtual void Increment()
		{
			Add(1L);
		}

		/// <summary>
		/// Equivalent to {@code add(-1)}.
		/// </summary>
		public virtual void Decrement()
		{
			Add(-1L);
		}

		/// <summary>
		/// Returns the current sum.  The returned value is <em>NOT</em> an
		/// atomic snapshot; invocation in the absence of concurrent
		/// updates returns an accurate result, but concurrent updates that
		/// occur while the sum is being calculated might not be
		/// incorporated.
		/// </summary>
		/// <returns> the sum </returns>
		public virtual long Sum()
		{
			Cell[] @as = Cells;
			Cell a;
			long sum = @base;
			if (@as != null)
			{
				for (int i = 0; i < @as.Length; ++i)
				{
					if ((a = @as[i]) != null)
					{
						sum += a.Value;
					}
				}
			}
			return sum;
		}

		/// <summary>
		/// Resets variables maintaining the sum to zero.  This method may
		/// be a useful alternative to creating a new adder, but is only
		/// effective if there are no concurrent updates.  Because this
		/// method is intrinsically racy, it should only be used when it is
		/// known that no threads are concurrently updating.
		/// </summary>
		public virtual void Reset()
		{
			Cell[] @as = Cells;
			Cell a;
			@base = 0L;
			if (@as != null)
			{
				for (int i = 0; i < @as.Length; ++i)
				{
					if ((a = @as[i]) != null)
					{
						a.Value = 0L;
					}
				}
			}
		}

		/// <summary>
		/// Equivalent in effect to <seealso cref="#sum"/> followed by {@link
		/// #reset}. This method may apply for example during quiescent
		/// points between multithreaded computations.  If there are
		/// updates concurrent with this method, the returned value is
		/// <em>not</em> guaranteed to be the final value occurring before
		/// the reset.
		/// </summary>
		/// <returns> the sum </returns>
		public virtual long SumThenReset()
		{
			Cell[] @as = Cells;
			Cell a;
			long sum = @base;
			@base = 0L;
			if (@as != null)
			{
				for (int i = 0; i < @as.Length; ++i)
				{
					if ((a = @as[i]) != null)
					{
						sum += a.Value;
						a.Value = 0L;
					}
				}
			}
			return sum;
		}

		/// <summary>
		/// Returns the String representation of the <seealso cref="#sum"/>. </summary>
		/// <returns> the String representation of the <seealso cref="#sum"/> </returns>
		public override String ToString()
		{
			return Convert.ToString(Sum());
		}

		/// <summary>
		/// Equivalent to <seealso cref="#sum"/>.
		/// </summary>
		/// <returns> the sum </returns>
		public override long LongValue()
		{
			return Sum();
		}

		/// <summary>
		/// Returns the <seealso cref="#sum"/> as an {@code int} after a narrowing
		/// primitive conversion.
		/// </summary>
		public override int IntValue()
		{
			return (int)Sum();
		}

		/// <summary>
		/// Returns the <seealso cref="#sum"/> as a {@code float}
		/// after a widening primitive conversion.
		/// </summary>
		public override float FloatValue()
		{
			return (float)Sum();
		}

		/// <summary>
		/// Returns the <seealso cref="#sum"/> as a {@code double} after a widening
		/// primitive conversion.
		/// </summary>
		public override double DoubleValue()
		{
			return (double)Sum();
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
			/// The current value returned by sum().
			/// @serial
			/// </summary>
			internal readonly long Value;

			internal SerializationProxy(LongAdder a)
			{
				Value = a.Sum();
			}

			/// <summary>
			/// Return a {@code LongAdder} object with initial state
			/// held by this proxy.
			/// </summary>
			/// <returns> a {@code LongAdder} object with initial state
			/// held by this proxy. </returns>
			internal virtual Object ReadResolve()
			{
				LongAdder a = new LongAdder();
				a.@base = Value;
				return a;
			}
		}

		/// <summary>
		/// Returns a
		/// <a href="../../../../serialized-form.html#java.util.concurrent.atomic.LongAdder.SerializationProxy">
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