using System;

/*
 * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file.attribute
{


	/// <summary>
	/// Represents the value of a file's time stamp attribute. For example, it may
	/// represent the time that the file was last
	/// <seealso cref="BasicFileAttributes#lastModifiedTime() modified"/>,
	/// <seealso cref="BasicFileAttributes#lastAccessTime() accessed"/>,
	/// or <seealso cref="BasicFileAttributes#creationTime() created"/>.
	/// 
	/// <para> Instances of this class are immutable.
	/// 
	/// @since 1.7
	/// </para>
	/// </summary>
	/// <seealso cref= java.nio.file.Files#setLastModifiedTime </seealso>
	/// <seealso cref= java.nio.file.Files#getLastModifiedTime </seealso>

	public sealed class FileTime : Comparable<FileTime>
	{
		/// <summary>
		/// The unit of granularity to interpret the value. Null if
		/// this {@code FileTime} is converted from an {@code Instant},
		/// the {@code value} and {@code unit} pair will not be used
		/// in this scenario.
		/// </summary>
		private readonly TimeUnit Unit;

		/// <summary>
		/// The value since the epoch; can be negative.
		/// </summary>
		private readonly long Value;

		/// <summary>
		/// The value as Instant (created lazily, if not from an instant)
		/// </summary>
		private Instant Instant;

		/// <summary>
		/// The value return by toString (created lazily)
		/// </summary>
		private String ValueAsString;

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		private FileTime(long value, TimeUnit unit, Instant instant)
		{
			this.Value = value;
			this.Unit = unit;
			this.Instant = instant;
		}

		/// <summary>
		/// Returns a {@code FileTime} representing a value at the given unit of
		/// granularity.
		/// </summary>
		/// <param name="value">
		///          the value since the epoch (1970-01-01T00:00:00Z); can be
		///          negative </param>
		/// <param name="unit">
		///          the unit of granularity to interpret the value
		/// </param>
		/// <returns>  a {@code FileTime} representing the given value </returns>
		public static FileTime From(long value, TimeUnit unit)
		{
			Objects.RequireNonNull(unit, "unit");
			return new FileTime(value, unit, null);
		}

		/// <summary>
		/// Returns a {@code FileTime} representing the given value in milliseconds.
		/// </summary>
		/// <param name="value">
		///          the value, in milliseconds, since the epoch
		///          (1970-01-01T00:00:00Z); can be negative
		/// </param>
		/// <returns>  a {@code FileTime} representing the given value </returns>
		public static FileTime FromMillis(long value)
		{
			return new FileTime(value, TimeUnit.MILLISECONDS, null);
		}

		/// <summary>
		/// Returns a {@code FileTime} representing the same point of time value
		/// on the time-line as the provided {@code Instant} object.
		/// </summary>
		/// <param name="instant">
		///          the instant to convert </param>
		/// <returns>  a {@code FileTime} representing the same point on the time-line
		///          as the provided instant
		/// @since 1.8 </returns>
		public static FileTime From(Instant instant)
		{
			Objects.RequireNonNull(instant, "instant");
			return new FileTime(0, null, instant);
		}

		/// <summary>
		/// Returns the value at the given unit of granularity.
		/// 
		/// <para> Conversion from a coarser granularity that would numerically overflow
		/// saturate to {@code Long.MIN_VALUE} if negative or {@code Long.MAX_VALUE}
		/// if positive.
		/// 
		/// </para>
		/// </summary>
		/// <param name="unit">
		///          the unit of granularity for the return value
		/// </param>
		/// <returns>  value in the given unit of granularity, since the epoch
		///          since the epoch (1970-01-01T00:00:00Z); can be negative </returns>
		public long To(TimeUnit unit)
		{
			Objects.RequireNonNull(unit, "unit");
			if (this.Unit != null)
			{
				return unit.Convert(this.Value, this.Unit);
			}
			else
			{
				long secs = unit.Convert(Instant.EpochSecond, TimeUnit.SECONDS);
				if (secs == Long.MinValue || secs == Long.MaxValue)
				{
					return secs;
				}
				long nanos = unit.Convert(Instant.Nano, TimeUnit.NANOSECONDS);
				long r = secs + nanos;
				// Math.addExact() variant
				if (((secs ^ r) & (nanos ^ r)) < 0)
				{
					return (secs < 0) ? Long.MinValue : Long.MaxValue;
				}
				return r;
			}
		}

		/// <summary>
		/// Returns the value in milliseconds.
		/// 
		/// <para> Conversion from a coarser granularity that would numerically overflow
		/// saturate to {@code Long.MIN_VALUE} if negative or {@code Long.MAX_VALUE}
		/// if positive.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the value in milliseconds, since the epoch (1970-01-01T00:00:00Z) </returns>
		public long ToMillis()
		{
			if (Unit != null)
			{
				return Unit.ToMillis(Value);
			}
			else
			{
				long secs = Instant.EpochSecond;
				int nanos = Instant.Nano;
				// Math.multiplyExact() variant
				long r = secs * 1000;
				long ax = System.Math.Abs(secs);
				if (((int)((uint)(ax | 1000) >> 31) != 0))
				{
					if ((r / 1000) != secs)
					{
						return (secs < 0) ? Long.MinValue : Long.MaxValue;
					}
				}
				return r + nanos / 1000000;
			}
		}

		/// <summary>
		/// Time unit constants for conversion.
		/// </summary>
		private const long HOURS_PER_DAY = 24L;
		private const long MINUTES_PER_HOUR = 60L;
		private const long SECONDS_PER_MINUTE = 60L;
		private static readonly long SECONDS_PER_HOUR = SECONDS_PER_MINUTE * MINUTES_PER_HOUR;
		private static readonly long SECONDS_PER_DAY = SECONDS_PER_HOUR * HOURS_PER_DAY;
		private const long MILLIS_PER_SECOND = 1000L;
		private static readonly long MICROS_PER_SECOND = 1000_000L;
		private static readonly long NANOS_PER_SECOND = 1000_000_000L;
		private const int NANOS_PER_MILLI = 1000000;
		private const int NANOS_PER_MICRO = 1000;
		// The epoch second of Instant.MIN.
		private const long MIN_SECOND = -31557014167219200L;
		// The epoch second of Instant.MAX.
		private const long MAX_SECOND = 31556889864403199L;

		/*
		 * Scale d by m, checking for overflow.
		 */
		private static long Scale(long d, long m, long over)
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

		/// <summary>
		/// Converts this {@code FileTime} object to an {@code Instant}.
		/// 
		/// <para> The conversion creates an {@code Instant} that represents the
		/// same point on the time-line as this {@code FileTime}.
		/// 
		/// </para>
		/// <para> {@code FileTime} can store points on the time-line further in the
		/// future and further in the past than {@code Instant}. Conversion
		/// from such further time points saturates to <seealso cref="Instant#MIN"/> if
		/// earlier than {@code Instant.MIN} or <seealso cref="Instant#MAX"/> if later
		/// than {@code Instant.MAX}.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  an instant representing the same point on the time-line as
		///          this {@code FileTime} object
		/// @since 1.8 </returns>
		public Instant ToInstant()
		{
			if (Instant == null)
			{
				long secs = 0L;
				int nanos = 0;
				switch (Unit.InnerEnumValue())
				{
					case TimeUnit.InnerEnum.DAYS:
						secs = Scale(Value, SECONDS_PER_DAY, Long.MaxValue / SECONDS_PER_DAY);
						break;
					case TimeUnit.InnerEnum.HOURS:
						secs = Scale(Value, SECONDS_PER_HOUR, Long.MaxValue / SECONDS_PER_HOUR);
						break;
					case TimeUnit.InnerEnum.MINUTES:
						secs = Scale(Value, SECONDS_PER_MINUTE, Long.MaxValue / SECONDS_PER_MINUTE);
						break;
					case TimeUnit.InnerEnum.SECONDS:
						secs = Value;
						break;
					case TimeUnit.InnerEnum.MILLISECONDS:
						secs = Math.FloorDiv(Value, MILLIS_PER_SECOND);
						nanos = (int)Math.FloorMod(Value, MILLIS_PER_SECOND) * NANOS_PER_MILLI;
						break;
					case TimeUnit.InnerEnum.MICROSECONDS:
						secs = Math.FloorDiv(Value, MICROS_PER_SECOND);
						nanos = (int)Math.FloorMod(Value, MICROS_PER_SECOND) * NANOS_PER_MICRO;
						break;
					case TimeUnit.InnerEnum.NANOSECONDS:
						secs = Math.FloorDiv(Value, NANOS_PER_SECOND);
						nanos = (int)Math.FloorMod(Value, NANOS_PER_SECOND);
						break;
					default :
						throw new AssertionError("Unit not handled");
				}
				if (secs <= MIN_SECOND)
				{
					Instant = Instant.MIN;
				}
				else if (secs >= MAX_SECOND)
				{
					Instant = Instant.MAX;
				}
				else
				{
					Instant = Instant.OfEpochSecond(secs, nanos);
				}
			}
			return Instant;
		}

		/// <summary>
		/// Tests this {@code FileTime} for equality with the given object.
		/// 
		/// <para> The result is {@code true} if and only if the argument is not {@code
		/// null} and is a {@code FileTime} that represents the same time. This
		/// method satisfies the general contract of the {@code Object.equals} method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">
		///          the object to compare with
		/// </param>
		/// <returns>  {@code true} if, and only if, the given object is a {@code
		///          FileTime} that represents the same time </returns>
		public override bool Equals(Object obj)
		{
			return (obj is FileTime) ? CompareTo((FileTime)obj) == 0 : false;
		}

		/// <summary>
		/// Computes a hash code for this file time.
		/// 
		/// <para> The hash code is based upon the value represented, and satisfies the
		/// general contract of the <seealso cref="Object#hashCode"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the hash-code value </returns>
		public override int HashCode()
		{
			// hashcode of instant representation to satisfy contract with equals
			return ToInstant().HashCode();
		}

		private long ToDays()
		{
			if (Unit != null)
			{
				return Unit.ToDays(Value);
			}
			else
			{
				return TimeUnit.SECONDS.ToDays(ToInstant().EpochSecond);
			}
		}

		private long ToExcessNanos(long days)
		{
			if (Unit != null)
			{
				return Unit.ToNanos(Value - Unit.Convert(days, TimeUnit.DAYS));
			}
			else
			{
				return TimeUnit.SECONDS.ToNanos(ToInstant().EpochSecond - TimeUnit.DAYS.ToSeconds(days));
			}
		}

		/// <summary>
		/// Compares the value of two {@code FileTime} objects for order.
		/// </summary>
		/// <param name="other">
		///          the other {@code FileTime} to be compared
		/// </param>
		/// <returns>  {@code 0} if this {@code FileTime} is equal to {@code other}, a
		///          value less than 0 if this {@code FileTime} represents a time
		///          that is before {@code other}, and a value greater than 0 if this
		///          {@code FileTime} represents a time that is after {@code other} </returns>
		public int CompareTo(FileTime other)
		{
			// same granularity
			if (Unit != null && Unit == other.Unit)
			{
				return Long.Compare(Value, other.Value);
			}
			else
			{
				// compare using instant representation when unit differs
				long secs = ToInstant().EpochSecond;
				long secsOther = other.ToInstant().EpochSecond;
				int cmp = Long.Compare(secs, secsOther);
				if (cmp != 0)
				{
					return cmp;
				}
				cmp = Long.Compare(ToInstant().Nano, other.ToInstant().Nano);
				if (cmp != 0)
				{
					return cmp;
				}
				if (secs != MAX_SECOND && secs != MIN_SECOND)
				{
					return 0;
				}
				// if both this and other's Instant reps are MIN/MAX,
				// use daysSinceEpoch and nanosOfDays, which will not
				// saturate during calculation.
				long days = ToDays();
				long daysOther = other.ToDays();
				if (days == daysOther)
				{
					return Long.Compare(ToExcessNanos(days), other.ToExcessNanos(daysOther));
				}
				return Long.Compare(days, daysOther);
			}
		}

		// days in a 400 year cycle = 146097
		// days in a 10,000 year cycle = 146097 * 25
		// seconds per day = 86400
		private static readonly long DAYS_PER_10000_YEARS = 146097L * 25L;
		private static readonly long SECONDS_PER_10000_YEARS = 146097L * 25L * 86400L;
		private static readonly long SECONDS_0000_TO_1970 = ((146097L * 5L) - (30L * 365L + 7L)) * 86400L;

		// append year/month/day/hour/minute/second/nano with width and 0 padding
		private StringBuilder Append(StringBuilder sb, int w, int d)
		{
			while (w > 0)
			{
				sb.Append((char)(d / w + '0'));
				d = d % w;
				w /= 10;
			}
			return sb;
		}

		/// <summary>
		/// Returns the string representation of this {@code FileTime}. The string
		/// is returned in the <a
		/// href="http://www.w3.org/TR/NOTE-datetime">ISO&nbsp;8601</a> format:
		/// <pre>
		///     YYYY-MM-DDThh:mm:ss[.s+]Z
		/// </pre>
		/// where "{@code [.s+]}" represents a dot followed by one of more digits
		/// for the decimal fraction of a second. It is only present when the decimal
		/// fraction of a second is not zero. For example, {@code
		/// FileTime.fromMillis(1234567890000L).toString()} yields {@code
		/// "2009-02-13T23:31:30Z"}, and {@code FileTime.fromMillis(1234567890123L).toString()}
		/// yields {@code "2009-02-13T23:31:30.123Z"}.
		/// 
		/// <para> A {@code FileTime} is primarily intended to represent the value of a
		/// file's time stamp. Where used to represent <i>extreme values</i>, where
		/// the year is less than "{@code 0001}" or greater than "{@code 9999}" then
		/// this method deviates from ISO 8601 in the same manner as the
		/// <a href="http://www.w3.org/TR/xmlschema-2/#deviantformats">XML Schema
		/// language</a>. That is, the year may be expanded to more than four digits
		/// and may be negative-signed. If more than four digits then leading zeros
		/// are not present. The year before "{@code 0001}" is "{@code -0001}".
		/// 
		/// </para>
		/// </summary>
		/// <returns>  the string representation of this file time </returns>
		public override String ToString()
		{
			if (ValueAsString == null)
			{
				long secs = 0L;
				int nanos = 0;
				if (Instant == null && Unit.CompareTo(TimeUnit.SECONDS) >= 0)
				{
					secs = Unit.ToSeconds(Value);
				}
				else
				{
					secs = ToInstant().EpochSecond;
					nanos = ToInstant().Nano;
				}
				LocalDateTime ldt;
				int year = 0;
				if (secs >= -SECONDS_0000_TO_1970)
				{
					// current era
					long zeroSecs = secs - SECONDS_PER_10000_YEARS + SECONDS_0000_TO_1970;
					long hi = Math.FloorDiv(zeroSecs, SECONDS_PER_10000_YEARS) + 1;
					long lo = Math.FloorMod(zeroSecs, SECONDS_PER_10000_YEARS);
					ldt = LocalDateTime.OfEpochSecond(lo - SECONDS_0000_TO_1970, nanos, ZoneOffset.UTC);
					year = ldt.Year + (int)hi * 10000;
				}
				else
				{
					// before current era
					long zeroSecs = secs + SECONDS_0000_TO_1970;
					long hi = zeroSecs / SECONDS_PER_10000_YEARS;
					long lo = zeroSecs % SECONDS_PER_10000_YEARS;
					ldt = LocalDateTime.OfEpochSecond(lo - SECONDS_0000_TO_1970, nanos, ZoneOffset.UTC);
					year = ldt.Year + (int)hi * 10000;
				}
				if (year <= 0)
				{
					year = year - 1;
				}
				int fraction = ldt.Nano;
				StringBuilder sb = new StringBuilder(64);
				sb.Append(year < 0 ? "-" : "");
				year = System.Math.Abs(year);
				if (year < 10000)
				{
					Append(sb, 1000, System.Math.Abs(year));
				}
				else
				{
					sb.Append(Convert.ToString(year));
				}
				sb.Append('-');
				Append(sb, 10, ldt.MonthValue);
				sb.Append('-');
				Append(sb, 10, ldt.DayOfMonth);
				sb.Append('T');
				Append(sb, 10, ldt.Hour);
				sb.Append(':');
				Append(sb, 10, ldt.Minute);
				sb.Append(':');
				Append(sb, 10, ldt.Second);
				if (fraction != 0)
				{
					sb.Append('.');
					// adding leading zeros and stripping any trailing zeros
					int w = 100000000;
					while (fraction % 10 == 0)
					{
						fraction /= 10;
						w /= 10;
					}
					Append(sb, w, fraction);
				}
				sb.Append('Z');
				ValueAsString = sb.ToString();
			}
			return ValueAsString;
		}
	}

}