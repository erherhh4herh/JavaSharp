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
	/// {@code double} sum.  When updates (method <seealso cref="#add"/>) are
	/// contended across threads, the set of variables may grow dynamically
	/// to reduce contention.  Method <seealso cref="#sum"/> (or, equivalently {@link
	/// #doubleValue}) returns the current total combined across the
	/// variables maintaining the sum. The order of accumulation within or
	/// across threads is not guaranteed. Thus, this class may not be
	/// applicable if numerical stability is required, especially when
	/// combining values of substantially different orders of magnitude.
	/// 
	/// <para>This class is usually preferable to alternatives when multiple
	/// threads update a common value that is used for purposes such as
	/// summary statistics that are frequently updated but less frequently
	/// read.
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
	public class DoubleAdder : Striped64
	{
		private const long SerialVersionUID = 7249069246863182397L;

		/*
		 * Note that we must use "long" for underlying representations,
		 * because there is no compareAndSet for double, due to the fact
		 * that the bitwise equals used in any CAS implementation is not
		 * the same as double-precision equals.  However, we use CAS only
		 * to detect and alleviate contention, for which bitwise equals
		 * works best anyway. In principle, the long/double conversions
		 * used here should be essentially free on most platforms since
		 * they just re-interpret bits.
		 */

		/// <summary>
		/// Creates a new adder with initial sum of zero.
		/// </summary>
		public DoubleAdder()
		{
		}

		/// <summary>
		/// Adds the given value.
		/// </summary>
		/// <param name="x"> the value to add </param>
		public virtual void Add(double x)
		{
			Cell[] @as;
			long b, v;
			int m;
			Cell a;
			if ((@as = Cells) != null || !CasBase(b = @base, Double.doubleToRawLongBits(Double.longBitsToDouble(b) + x)))
			{
				bool uncontended = true;
				if (@as == null || (m = @as.Length - 1) < 0 || (a = @as[Probe & m]) == null || !(uncontended = a.Cas(v = a.Value, Double.doubleToRawLongBits(Double.longBitsToDouble(v) + x))))
				{
					DoubleAccumulate(x, null, uncontended);
				}
			}
		}

		/// <summary>
		/// Returns the current sum.  The returned value is <em>NOT</em> an
		/// atomic snapshot; invocation in the absence of concurrent
		/// updates returns an accurate result, but concurrent updates that
		/// occur while the sum is being calculated might not be
		/// incorporated.  Also, because floating-point arithmetic is not
		/// strictly associative, the returned result need not be identical
		/// to the value that would be obtained in a sequential series of
		/// updates to a single variable.
		/// </summary>
		/// <returns> the sum </returns>
		public virtual double Sum()
		{
			Cell[] @as = Cells;
			Cell a;
			double sum = Double.longBitsToDouble(@base);
			if (@as != null)
			{
				for (int i = 0; i < @as.Length; ++i)
				{
					if ((a = @as[i]) != null)
					{
						sum += Double.longBitsToDouble(a.Value);
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
			@base = 0L; // relies on fact that double 0 must have same rep as long
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
		public virtual double SumThenReset()
		{
			Cell[] @as = Cells;
			Cell a;
			double sum = Double.longBitsToDouble(@base);
			@base = 0L;
			if (@as != null)
			{
				for (int i = 0; i < @as.Length; ++i)
				{
					if ((a = @as[i]) != null)
					{
						long v = a.Value;
						a.Value = 0L;
						sum += Double.longBitsToDouble(v);
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
		public override double DoubleValue()
		{
			return Sum();
		}

		/// <summary>
		/// Returns the <seealso cref="#sum"/> as a {@code long} after a
		/// narrowing primitive conversion.
		/// </summary>
		public override long LongValue()
		{
			return (long)Sum();
		}

		/// <summary>
		/// Returns the <seealso cref="#sum"/> as an {@code int} after a
		/// narrowing primitive conversion.
		/// </summary>
		public override int IntValue()
		{
			return (int)Sum();
		}

		/// <summary>
		/// Returns the <seealso cref="#sum"/> as a {@code float}
		/// after a narrowing primitive conversion.
		/// </summary>
		public override float FloatValue()
		{
			return (float)Sum();
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
			internal readonly double Value;

			internal SerializationProxy(DoubleAdder a)
			{
				Value = a.Sum();
			}

			/// <summary>
			/// Returns a {@code DoubleAdder} object with initial state
			/// held by this proxy.
			/// </summary>
			/// <returns> a {@code DoubleAdder} object with initial state
			/// held by this proxy. </returns>
			internal virtual Object ReadResolve()
			{
				DoubleAdder a = new DoubleAdder();
				a.@base = Double.doubleToRawLongBits(Value);
				return a;
			}
		}

		/// <summary>
		/// Returns a
		/// <a href="../../../../serialized-form.html#java.util.concurrent.atomic.DoubleAdder.SerializationProxy">
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