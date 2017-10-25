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
	/// One or more variables that together maintain a running {@code double}
	/// value updated using a supplied function.  When updates (method
	/// <seealso cref="#accumulate"/>) are contended across threads, the set of variables
	/// may grow dynamically to reduce contention.  Method <seealso cref="#get"/>
	/// (or, equivalently, <seealso cref="#doubleValue"/>) returns the current value
	/// across the variables maintaining updates.
	/// 
	/// <para>This class is usually preferable to alternatives when multiple
	/// threads update a common value that is used for purposes such as
	/// summary statistics that are frequently updated but less frequently
	/// read.
	/// 
	/// </para>
	/// <para>The supplied accumulator function should be side-effect-free,
	/// since it may be re-applied when attempted updates fail due to
	/// contention among threads. The function is applied with the current
	/// value as its first argument, and the given update as the second
	/// argument.  For example, to maintain a running maximum value, you
	/// could supply {@code Double::max} along with {@code
	/// Double.NEGATIVE_INFINITY} as the identity. The order of
	/// accumulation within or across threads is not guaranteed. Thus, this
	/// class may not be applicable if numerical stability is required,
	/// especially when combining values of substantially different orders
	/// of magnitude.
	/// 
	/// </para>
	/// <para>Class <seealso cref="DoubleAdder"/> provides analogs of the functionality
	/// of this class for the common special case of maintaining sums.  The
	/// call {@code new DoubleAdder()} is equivalent to {@code new
	/// DoubleAccumulator((x, y) -> x + y, 0.0)}.
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
	public class DoubleAccumulator : Striped64
	{
		private const long SerialVersionUID = 7249069246863182397L;

		private readonly DoubleBinaryOperator Function;
		private readonly long Identity; // use long representation

		/// <summary>
		/// Creates a new instance using the given accumulator function
		/// and identity element. </summary>
		/// <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		/// <param name="identity"> identity (initial value) for the accumulator function </param>
		public DoubleAccumulator(DoubleBinaryOperator accumulatorFunction, double identity)
		{
			this.Function = accumulatorFunction;
			@base = this.Identity = Double.doubleToRawLongBits(identity);
		}

		/// <summary>
		/// Updates with the given value.
		/// </summary>
		/// <param name="x"> the value </param>
		public virtual void Accumulate(double x)
		{
			Cell[] @as;
			long b, v, r;
			int m;
			Cell a;
			if ((@as = Cells) != null || (r = Double.doubleToRawLongBits(Function.ApplyAsDouble(Double.longBitsToDouble(b = @base), x))) != b && !CasBase(b, r))
			{
				bool uncontended = true;
				if (@as == null || (m = @as.Length - 1) < 0 || (a = @as[Probe & m]) == null || !(uncontended = (r = Double.doubleToRawLongBits(Function.ApplyAsDouble(Double.longBitsToDouble(v = a.Value), x))) == v || a.Cas(v, r)))
				{
					DoubleAccumulate(x, Function, uncontended);
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
		public virtual double Get()
		{
			Cell[] @as = Cells;
			Cell a;
			double result = Double.longBitsToDouble(@base);
			if (@as != null)
			{
				for (int i = 0; i < @as.Length; ++i)
				{
					if ((a = @as[i]) != null)
					{
						result = Function.ApplyAsDouble(result, Double.longBitsToDouble(a.Value));
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
		public virtual double ThenReset
		{
			get
			{
				Cell[] @as = Cells;
				Cell a;
				double result = Double.longBitsToDouble(@base);
				@base = Identity;
				if (@as != null)
				{
					for (int i = 0; i < @as.Length; ++i)
					{
						if ((a = @as[i]) != null)
						{
							double v = Double.longBitsToDouble(a.Value);
							a.Value = Identity;
							result = Function.ApplyAsDouble(result, v);
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
		public override double DoubleValue()
		{
			return Get();
		}

		/// <summary>
		/// Returns the <seealso cref="#get current value"/> as a {@code long}
		/// after a narrowing primitive conversion.
		/// </summary>
		public override long LongValue()
		{
			return (long)Get();
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
		/// after a narrowing primitive conversion.
		/// </summary>
		public override float FloatValue()
		{
			return (float)Get();
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
			internal readonly double Value;
			/// <summary>
			/// The function used for updates.
			/// @serial
			/// </summary>
			internal readonly DoubleBinaryOperator Function;
			/// <summary>
			/// The identity value
			/// @serial
			/// </summary>
			internal readonly long Identity;

			internal SerializationProxy(DoubleAccumulator a)
			{
				Function = a.Function;
				Identity = a.Identity;
				Value = a.Get();
			}

			/// <summary>
			/// Returns a {@code DoubleAccumulator} object with initial state
			/// held by this proxy.
			/// </summary>
			/// <returns> a {@code DoubleAccumulator} object with initial state
			/// held by this proxy. </returns>
			internal virtual Object ReadResolve()
			{
				double d = Double.longBitsToDouble(Identity);
				DoubleAccumulator a = new DoubleAccumulator(Function, d);
				a.@base = Double.doubleToRawLongBits(Value);
				return a;
			}
		}

		/// <summary>
		/// Returns a
		/// <a href="../../../../serialized-form.html#java.util.concurrent.atomic.DoubleAccumulator.SerializationProxy">
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