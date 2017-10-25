using System.Collections.Generic;
using System.Threading;

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

namespace java.util.concurrent
{

	/// <summary>
	/// A {@code TimeUnit} represents time durations at a given unit of
	/// granularity and provides utility methods to convert across units,
	/// and to perform timing and delay operations in these units.  A
	/// {@code TimeUnit} does not maintain time information, but only
	/// helps organize and use time representations that may be maintained
	/// separately across various contexts.  A nanosecond is defined as one
	/// thousandth of a microsecond, a microsecond as one thousandth of a
	/// millisecond, a millisecond as one thousandth of a second, a minute
	/// as sixty seconds, an hour as sixty minutes, and a day as twenty four
	/// hours.
	/// 
	/// <para>A {@code TimeUnit} is mainly used to inform time-based methods
	/// how a given timing parameter should be interpreted. For example,
	/// the following code will timeout in 50 milliseconds if the {@link
	/// java.util.concurrent.locks.Lock lock} is not available:
	/// 
	///  <pre> {@code
	/// Lock lock = ...;
	/// if (lock.tryLock(50L, TimeUnit.MILLISECONDS)) ...}</pre>
	/// 
	/// while this code will timeout in 50 seconds:
	///  <pre> {@code
	/// Lock lock = ...;
	/// if (lock.tryLock(50L, TimeUnit.SECONDS)) ...}</pre>
	/// 
	/// Note however, that there is no guarantee that a particular timeout
	/// implementation will be able to notice the passage of time at the
	/// same granularity as the given {@code TimeUnit}.
	/// 
	/// @since 1.5
	/// @author Doug Lea
	/// </para>
	/// </summary>
	public sealed class TimeUnit
	{
		/// <summary>
		/// Time unit representing one thousandth of a microsecond
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		NANOSECONDS
		{
			public long toNanos(long d) { return d
			public static readonly TimeUnit public long toNanos(long d) { return d = new TimeUnit("public long toNanos(long d) { return d", InnerEnum.public long toNanos(long d) { return d);

			private static readonly IList<TimeUnit> valueList = new List<TimeUnit>();

			static TimeUnit()
			{
				valueList.Add(public long toNanos(long d) { return d);
			}

			public enum InnerEnum
			{
				public long toNanos(long d) { return d
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;
		}
			public long ToMicros(long d)
			{
				return d / (C1 / C0);
			}
			public long ToMillis(long d)
			{
				return d / (C2 / C0);
			}
			public long ToSeconds(long d)
			{
				return d / (C3 / C0);
			}
			public long ToMinutes(long d)
			{
				return d / (C4 / C0);
			}
			public long ToHours(long d)
			{
				return d / (C5 / C0);
			}
			public long ToDays(long d)
			{
				return d / (C6 / C0);
			}
			public long Convert(long d, TimeUnit u)
			{
				return u.ToNanos(d);
			}
			internal int ExcessNanos(long d, long m)
			{
				return (int)(d - (m * C2));
			}
	},
		public static readonly TimeUnit } = new TimeUnit("}", InnerEnum.});

		/// <summary>
		/// Time unit representing one thousandth of a millisecond
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		MICROSECONDS
		{
			public long ToNanos(long d)
			{
				return x(d, C1 / C0, MAX / (C1 / C0));
			}
			public long ToMicros(long d)
			{
				return d;
			}
			public long ToMillis(long d)
			{
				return d / (C2 / C1);
			}
			public long ToSeconds(long d)
			{
				return d / (C3 / C1);
			}
			public long ToMinutes(long d)
			{
				return d / (C4 / C1);
			}
			public long ToHours(long d)
			{
				return d / (C5 / C1);
			}
			public long ToDays(long d)
			{
				return d / (C6 / C1);
			}
			public long Convert(long d, TimeUnit u)
			{
				return u.ToMicros(d);
			}
			internal int ExcessNanos(long d, long m)
			{
				return (int)((d * C1) - (m * C2));
			}
		},

		/// <summary>
		/// Time unit representing one thousandth of a second
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		MILLISECONDS
		{
			public long ToNanos(long d)
			{
				return x(d, C2 / C0, MAX / (C2 / C0));
			}
			public long ToMicros(long d)
			{
				return x(d, C2 / C1, MAX / (C2 / C1));
			}
			public long ToMillis(long d)
			{
				return d;
			}
			public long ToSeconds(long d)
			{
				return d / (C3 / C2);
			}
			public long ToMinutes(long d)
			{
				return d / (C4 / C2);
			}
			public long ToHours(long d)
			{
				return d / (C5 / C2);
			}
			public long ToDays(long d)
			{
				return d / (C6 / C2);
			}
			public long Convert(long d, TimeUnit u)
			{
				return u.ToMillis(d);
			}
			internal int ExcessNanos(long d, long m)
			{
				return 0;
			}
		},

		/// <summary>
		/// Time unit representing one second
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		SECONDS
		{
			public long ToNanos(long d)
			{
				return x(d, C3 / C0, MAX / (C3 / C0));
			}
			public long ToMicros(long d)
			{
				return x(d, C3 / C1, MAX / (C3 / C1));
			}
			public long ToMillis(long d)
			{
				return x(d, C3 / C2, MAX / (C3 / C2));
			}
			public long ToSeconds(long d)
			{
				return d;
			}
			public long ToMinutes(long d)
			{
				return d / (C4 / C3);
			}
			public long ToHours(long d)
			{
				return d / (C5 / C3);
			}
			public long ToDays(long d)
			{
				return d / (C6 / C3);
			}
			public long Convert(long d, TimeUnit u)
			{
				return u.ToSeconds(d);
			}
			internal int ExcessNanos(long d, long m)
			{
				return 0;
			}
		},

		/// <summary>
		/// Time unit representing sixty seconds
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		MINUTES
		{
			public long ToNanos(long d)
			{
				return x(d, C4 / C0, MAX / (C4 / C0));
			}
			public long ToMicros(long d)
			{
				return x(d, C4 / C1, MAX / (C4 / C1));
			}
			public long ToMillis(long d)
			{
				return x(d, C4 / C2, MAX / (C4 / C2));
			}
			public long ToSeconds(long d)
			{
				return x(d, C4 / C3, MAX / (C4 / C3));
			}
			public long ToMinutes(long d)
			{
				return d;
			}
			public long ToHours(long d)
			{
				return d / (C5 / C4);
			}
			public long ToDays(long d)
			{
				return d / (C6 / C4);
			}
			public long Convert(long d, TimeUnit u)
			{
				return u.ToMinutes(d);
			}
			internal int ExcessNanos(long d, long m)
			{
				return 0;
			}
		},

		/// <summary>
		/// Time unit representing sixty minutes
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		HOURS
		{
			public long ToNanos(long d)
			{
				return x(d, C5 / C0, MAX / (C5 / C0));
			}
			public long ToMicros(long d)
			{
				return x(d, C5 / C1, MAX / (C5 / C1));
			}
			public long ToMillis(long d)
			{
				return x(d, C5 / C2, MAX / (C5 / C2));
			}
			public long ToSeconds(long d)
			{
				return x(d, C5 / C3, MAX / (C5 / C3));
			}
			public long ToMinutes(long d)
			{
				return x(d, C5 / C4, MAX / (C5 / C4));
			}
			public long ToHours(long d)
			{
				return d;
			}
			public long ToDays(long d)
			{
				return d / (C6 / C5);
			}
			public long Convert(long d, TimeUnit u)
			{
				return u.ToHours(d);
			}
			internal int ExcessNanos(long d, long m)
			{
				return 0;
			}
		},

		/// <summary>
		/// Time unit representing twenty four hours
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		DAYS
		{
			public long ToNanos(long d)
			{
				return x(d, C6 / C0, MAX / (C6 / C0));
			}
			public long ToMicros(long d)
			{
				return x(d, C6 / C1, MAX / (C6 / C1));
			}
			public long ToMillis(long d)
			{
				return x(d, C6 / C2, MAX / (C6 / C2));
			}
			public long ToSeconds(long d)
			{
				return x(d, C6 / C3, MAX / (C6 / C3));
			}
			public long ToMinutes(long d)
			{
				return x(d, C6 / C4, MAX / (C6 / C4));
			}
			public long ToHours(long d)
			{
				return x(d, C6 / C5, MAX / (C6 / C5));
			}
			public long ToDays(long d)
			{
				return d;
			}
			public long Convert(long d, TimeUnit u)
			{
				return u.ToDays(d);
			}
			internal int ExcessNanos(long d, long m)
			{
				return 0;
			}
		}

		// Handy constants for conversion methods
		internal const long C0 = 1L;
		internal static readonly long C1 = C0 * 1000L;
		internal static readonly long C2 = C1 * 1000L;
		internal static readonly long C3 = C2 * 1000L;
		internal static readonly long C4 = C3 * 60L;
		internal static readonly long C5 = C4 * 60L;
		internal static readonly long C6 = C5 * 24L;

		internal const long MAX = Long.MAX_VALUE;

		/// <summary>
		/// Scale d by m, checking for overflow.
		/// This has a short name to make above code more readable.
		/// </summary>
		internal static long x(long d, long m, long over)
		{
			if (d > over)
			{
				return Long.MaxValue;
			}
			if (d < -over)
			{
				return Long.MinValue;
			}
			return d * m;
		}

		// To maintain full signature compatibility with 1.5, and to improve the
		// clarity of the generated javadoc (see 6287639: Abstract methods in
		// enum classes should not be listed as abstract), method convert
		// etc. are not declared abstract but otherwise act as abstract methods.

		/// <summary>
		/// Converts the given time duration in the given unit to this unit.
		/// Conversions from finer to coarser granularities truncate, so
		/// lose precision. For example, converting {@code 999} milliseconds
		/// to seconds results in {@code 0}. Conversions from coarser to
		/// finer granularities with arguments that would numerically
		/// overflow saturate to {@code Long.MIN_VALUE} if negative or
		/// {@code Long.MAX_VALUE} if positive.
		/// 
		/// <para>For example, to convert 10 minutes to milliseconds, use:
		/// {@code TimeUnit.MILLISECONDS.convert(10L, TimeUnit.MINUTES)}
		/// 
		/// </para>
		/// </summary>
		/// <param name="sourceDuration"> the time duration in the given {@code sourceUnit} </param>
		/// <param name="sourceUnit"> the unit of the {@code sourceDuration} argument </param>
		/// <returns> the converted duration in this unit,
		/// or {@code Long.MIN_VALUE} if conversion would negatively
		/// overflow, or {@code Long.MAX_VALUE} if it would positively overflow. </returns>
		public long Convert(long sourceDuration, TimeUnit sourceUnit)
		{
			throw new AbstractMethodError();
		}

		/// <summary>
		/// Equivalent to
		/// <seealso cref="#convert(long, TimeUnit) NANOSECONDS.convert(duration, this)"/>. </summary>
		/// <param name="duration"> the duration </param>
		/// <returns> the converted duration,
		/// or {@code Long.MIN_VALUE} if conversion would negatively
		/// overflow, or {@code Long.MAX_VALUE} if it would positively overflow. </returns>
		public long ToNanos(long duration)
		{
			throw new AbstractMethodError();
		}

		/// <summary>
		/// Equivalent to
		/// <seealso cref="#convert(long, TimeUnit) MICROSECONDS.convert(duration, this)"/>. </summary>
		/// <param name="duration"> the duration </param>
		/// <returns> the converted duration,
		/// or {@code Long.MIN_VALUE} if conversion would negatively
		/// overflow, or {@code Long.MAX_VALUE} if it would positively overflow. </returns>
		public long ToMicros(long duration)
		{
			throw new AbstractMethodError();
		}

		/// <summary>
		/// Equivalent to
		/// <seealso cref="#convert(long, TimeUnit) MILLISECONDS.convert(duration, this)"/>. </summary>
		/// <param name="duration"> the duration </param>
		/// <returns> the converted duration,
		/// or {@code Long.MIN_VALUE} if conversion would negatively
		/// overflow, or {@code Long.MAX_VALUE} if it would positively overflow. </returns>
		public long ToMillis(long duration)
		{
			throw new AbstractMethodError();
		}

		/// <summary>
		/// Equivalent to
		/// <seealso cref="#convert(long, TimeUnit) SECONDS.convert(duration, this)"/>. </summary>
		/// <param name="duration"> the duration </param>
		/// <returns> the converted duration,
		/// or {@code Long.MIN_VALUE} if conversion would negatively
		/// overflow, or {@code Long.MAX_VALUE} if it would positively overflow. </returns>
		public long ToSeconds(long duration)
		{
			throw new AbstractMethodError();
		}

		/// <summary>
		/// Equivalent to
		/// <seealso cref="#convert(long, TimeUnit) MINUTES.convert(duration, this)"/>. </summary>
		/// <param name="duration"> the duration </param>
		/// <returns> the converted duration,
		/// or {@code Long.MIN_VALUE} if conversion would negatively
		/// overflow, or {@code Long.MAX_VALUE} if it would positively overflow.
		/// @since 1.6 </returns>
		public long ToMinutes(long duration)
		{
			throw new AbstractMethodError();
		}

		/// <summary>
		/// Equivalent to
		/// <seealso cref="#convert(long, TimeUnit) HOURS.convert(duration, this)"/>. </summary>
		/// <param name="duration"> the duration </param>
		/// <returns> the converted duration,
		/// or {@code Long.MIN_VALUE} if conversion would negatively
		/// overflow, or {@code Long.MAX_VALUE} if it would positively overflow.
		/// @since 1.6 </returns>
		public long ToHours(long duration)
		{
			throw new AbstractMethodError();
		}

		/// <summary>
		/// Equivalent to
		/// <seealso cref="#convert(long, TimeUnit) DAYS.convert(duration, this)"/>. </summary>
		/// <param name="duration"> the duration </param>
		/// <returns> the converted duration
		/// @since 1.6 </returns>
		public long ToDays(long duration)
		{
			throw new AbstractMethodError();
		}

		/// <summary>
		/// Utility to compute the excess-nanosecond argument to wait,
		/// sleep, join. </summary>
		/// <param name="d"> the duration </param>
		/// <param name="m"> the number of milliseconds </param>
		/// <returns> the number of nanoseconds </returns>
		public static readonly TimeUnit abstract int excessNanos = new TimeUnit("abstract int excessNanos", InnerEnum.abstract int excessNanos, long d, long m);

		private static readonly IList<TimeUnit> valueList = new List<TimeUnit>();

		static TimeUnit()
		{
			valueList.Add(public long toNanos(long d) { return d);
			valueList.Add(});
			valueList.Add(abstract int excessNanos);
		}

		public enum InnerEnum
		{
			public long toNanos(long d) { return d,
		},
			abstract int excessNanos
}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		/// <summary>
		/// Performs a timed <seealso cref="Object#wait(long, int) Object.wait"/>
		/// using this time unit.
		/// This is a convenience method that converts timeout arguments
		/// into the form required by the {@code Object.wait} method.
		/// 
		/// <para>For example, you could implement a blocking {@code poll}
		/// method (see <seealso cref="BlockingQueue#poll BlockingQueue.poll"/>)
		/// using:
		/// 
		///  <pre> {@code
		/// public synchronized Object poll(long timeout, TimeUnit unit)
		///     throws InterruptedException {
		///   while (empty) {
		///     unit.timedWait(this, timeout);
		///     ...
		///   }
		/// }}</pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj"> the object to wait on </param>
		/// <param name="timeout"> the maximum time to wait. If less than
		/// or equal to zero, do not wait at all. </param>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void timedWait(Object obj, long timeout) throws InterruptedException
		public void TimedWait(Object obj, long timeout)
		{
			if (timeout > 0)
			{
				long ms = toMillis(timeout);
				int ns = excessNanos(timeout, ms);
				obj.Wait(ms, ns);
			}
		}

		/// <summary>
		/// Performs a timed <seealso cref="Thread#join(long, int) Thread.join"/>
		/// using this time unit.
		/// This is a convenience method that converts time arguments into the
		/// form required by the {@code Thread.join} method.
		/// </summary>
		/// <param name="thread"> the thread to wait for </param>
		/// <param name="timeout"> the maximum time to wait. If less than
		/// or equal to zero, do not wait at all. </param>
		/// <exception cref="InterruptedException"> if interrupted while waiting </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void timedJoin(Thread thread, long timeout) throws InterruptedException
		public void TimedJoin(Thread thread, long timeout)
		{
			if (timeout > 0)
			{
				long ms = toMillis(timeout);
				int ns = excessNanos(timeout, ms);
				thread.Join(ms, ns);
			}
		}

		/// <summary>
		/// Performs a <seealso cref="Thread#sleep(long, int) Thread.sleep"/> using
		/// this time unit.
		/// This is a convenience method that converts time arguments into the
		/// form required by the {@code Thread.sleep} method.
		/// </summary>
		/// <param name="timeout"> the minimum time to sleep. If less than
		/// or equal to zero, do not sleep at all. </param>
		/// <exception cref="InterruptedException"> if interrupted while sleeping </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void sleep(long timeout) throws InterruptedException
		public void Sleep(long timeout)
		{
			if (timeout > 0)
			{
				long ms = toMillis(timeout);
				int ns = excessNanos(timeout, ms);
				Thread.Sleep(ms, ns);
			}
		}


		public static IList<TimeUnit> values()
		{
			return valueList;
		}

		public InnerEnum InnerEnumValue()
		{
			return innerEnumValue;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static TimeUnit valueOf(string name)
		{
			foreach (TimeUnit enumInstance in TimeUnit.values())
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}