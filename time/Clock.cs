using System;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
 * Copyright (c) 2007-2012, Stephen Colebourne & Michael Nascimento Santos
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *
 *  * Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 *  * Neither the name of JSR-310 nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
namespace java.time
{



	/// <summary>
	/// A clock providing access to the current instant, date and time using a time-zone.
	/// <para>
	/// Instances of this class are used to find the current instant, which can be
	/// interpreted using the stored time-zone to find the current date and time.
	/// As such, a clock can be used instead of <seealso cref="System#currentTimeMillis()"/>
	/// and <seealso cref="TimeZone#getDefault()"/>.
	/// </para>
	/// <para>
	/// Use of a {@code Clock} is optional. All key date-time classes also have a
	/// {@code now()} factory method that uses the system clock in the default time zone.
	/// The primary purpose of this abstraction is to allow alternate clocks to be
	/// plugged in as and when required. Applications use an object to obtain the
	/// current time rather than a static method. This can simplify testing.
	/// </para>
	/// <para>
	/// Best practice for applications is to pass a {@code Clock} into any method
	/// that requires the current instant. A dependency injection framework is one
	/// way to achieve this:
	/// <pre>
	///  public class MyBean {
	///    private Clock clock;  // dependency inject
	///    ...
	///    public void process(LocalDate eventDate) {
	///      if (eventDate.isBefore(LocalDate.now(clock)) {
	///        ...
	///      }
	///    }
	///  }
	/// </pre>
	/// This approach allows an alternate clock, such as <seealso cref="#fixed(Instant, ZoneId) fixed"/>
	/// or <seealso cref="#offset(Clock, Duration) offset"/> to be used during testing.
	/// </para>
	/// <para>
	/// The {@code system} factory methods provide clocks based on the best available
	/// system clock This may use <seealso cref="System#currentTimeMillis()"/>, or a higher
	/// resolution clock if one is available.
	/// 
	/// @implSpec
	/// This abstract class must be implemented with care to ensure other classes operate correctly.
	/// All implementations that can be instantiated must be final, immutable and thread-safe.
	/// </para>
	/// <para>
	/// The principal methods are defined to allow the throwing of an exception.
	/// In normal use, no exceptions will be thrown, however one possible implementation would be to
	/// obtain the time from a central time server across the network. Obviously, in this case the
	/// lookup could fail, and so the method is permitted to throw an exception.
	/// </para>
	/// <para>
	/// The returned instants from {@code Clock} work on a time-scale that ignores leap seconds,
	/// as described in <seealso cref="Instant"/>. If the implementation wraps a source that provides leap
	/// second information, then a mechanism should be used to "smooth" the leap second.
	/// The Java Time-Scale mandates the use of UTC-SLS, however clock implementations may choose
	/// how accurate they are with the time-scale so long as they document how they work.
	/// Implementations are therefore not required to actually perform the UTC-SLS slew or to
	/// otherwise be aware of leap seconds.
	/// </para>
	/// <para>
	/// Implementations should implement {@code Serializable} wherever possible and must
	/// document whether or not they do support serialization.
	/// 
	/// @implNote
	/// The clock implementation provided here is based on <seealso cref="System#currentTimeMillis()"/>.
	/// That method provides little to no guarantee about the accuracy of the clock.
	/// Applications requiring a more accurate clock must implement this abstract class
	/// themselves using a different external clock, such as an NTP server.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public abstract class Clock
	{

		/// <summary>
		/// Obtains a clock that returns the current instant using the best available
		/// system clock, converting to date and time using the UTC time-zone.
		/// <para>
		/// This clock, rather than <seealso cref="#systemDefaultZone()"/>, should be used when
		/// you need the current instant without the date or time.
		/// </para>
		/// <para>
		/// This clock is based on the best available system clock.
		/// This may use <seealso cref="System#currentTimeMillis()"/>, or a higher resolution
		/// clock if one is available.
		/// </para>
		/// <para>
		/// Conversion from instant to date or time uses the <seealso cref="ZoneOffset#UTC UTC time-zone"/>.
		/// </para>
		/// <para>
		/// The returned implementation is immutable, thread-safe and {@code Serializable}.
		/// It is equivalent to {@code system(ZoneOffset.UTC)}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a clock that uses the best available system clock in the UTC zone, not null </returns>
		public static Clock SystemUTC()
		{
			return new SystemClock(ZoneOffset.UTC);
		}

		/// <summary>
		/// Obtains a clock that returns the current instant using the best available
		/// system clock, converting to date and time using the default time-zone.
		/// <para>
		/// This clock is based on the best available system clock.
		/// This may use <seealso cref="System#currentTimeMillis()"/>, or a higher resolution
		/// clock if one is available.
		/// </para>
		/// <para>
		/// Using this method hard codes a dependency to the default time-zone into your application.
		/// It is recommended to avoid this and use a specific time-zone whenever possible.
		/// The <seealso cref="#systemUTC() UTC clock"/> should be used when you need the current instant
		/// without the date or time.
		/// </para>
		/// <para>
		/// The returned implementation is immutable, thread-safe and {@code Serializable}.
		/// It is equivalent to {@code system(ZoneId.systemDefault())}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a clock that uses the best available system clock in the default zone, not null </returns>
		/// <seealso cref= ZoneId#systemDefault() </seealso>
		public static Clock SystemDefaultZone()
		{
			return new SystemClock(ZoneId.SystemDefault());
		}

		/// <summary>
		/// Obtains a clock that returns the current instant using best available
		/// system clock.
		/// <para>
		/// This clock is based on the best available system clock.
		/// This may use <seealso cref="System#currentTimeMillis()"/>, or a higher resolution
		/// clock if one is available.
		/// </para>
		/// <para>
		/// Conversion from instant to date or time uses the specified time-zone.
		/// </para>
		/// <para>
		/// The returned implementation is immutable, thread-safe and {@code Serializable}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to use to convert the instant to date-time, not null </param>
		/// <returns> a clock that uses the best available system clock in the specified zone, not null </returns>
		public static Clock System(ZoneId zone)
		{
			Objects.RequireNonNull(zone, "zone");
			return new SystemClock(zone);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Obtains a clock that returns the current instant ticking in whole seconds
		/// using best available system clock.
		/// <para>
		/// This clock will always have the nano-of-second field set to zero.
		/// This ensures that the visible time ticks in whole seconds.
		/// The underlying clock is the best available system clock, equivalent to
		/// using <seealso cref="#system(ZoneId)"/>.
		/// </para>
		/// <para>
		/// Implementations may use a caching strategy for performance reasons.
		/// As such, it is possible that the start of the second observed via this
		/// clock will be later than that observed directly via the underlying clock.
		/// </para>
		/// <para>
		/// The returned implementation is immutable, thread-safe and {@code Serializable}.
		/// It is equivalent to {@code tick(system(zone), Duration.ofSeconds(1))}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to use to convert the instant to date-time, not null </param>
		/// <returns> a clock that ticks in whole seconds using the specified zone, not null </returns>
		public static Clock TickSeconds(ZoneId zone)
		{
			return new TickClock(System(zone), NANOS_PER_SECOND);
		}

		/// <summary>
		/// Obtains a clock that returns the current instant ticking in whole minutes
		/// using best available system clock.
		/// <para>
		/// This clock will always have the nano-of-second and second-of-minute fields set to zero.
		/// This ensures that the visible time ticks in whole minutes.
		/// The underlying clock is the best available system clock, equivalent to
		/// using <seealso cref="#system(ZoneId)"/>.
		/// </para>
		/// <para>
		/// Implementations may use a caching strategy for performance reasons.
		/// As such, it is possible that the start of the minute observed via this
		/// clock will be later than that observed directly via the underlying clock.
		/// </para>
		/// <para>
		/// The returned implementation is immutable, thread-safe and {@code Serializable}.
		/// It is equivalent to {@code tick(system(zone), Duration.ofMinutes(1))}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to use to convert the instant to date-time, not null </param>
		/// <returns> a clock that ticks in whole minutes using the specified zone, not null </returns>
		public static Clock TickMinutes(ZoneId zone)
		{
			return new TickClock(System(zone), NANOS_PER_MINUTE);
		}

		/// <summary>
		/// Obtains a clock that returns instants from the specified clock truncated
		/// to the nearest occurrence of the specified duration.
		/// <para>
		/// This clock will only tick as per the specified duration. Thus, if the duration
		/// is half a second, the clock will return instants truncated to the half second.
		/// </para>
		/// <para>
		/// The tick duration must be positive. If it has a part smaller than a whole
		/// millisecond, then the whole duration must divide into one second without
		/// leaving a remainder. All normal tick durations will match these criteria,
		/// including any multiple of hours, minutes, seconds and milliseconds, and
		/// sensible nanosecond durations, such as 20ns, 250,000ns and 500,000ns.
		/// </para>
		/// <para>
		/// A duration of zero or one nanosecond would have no truncation effect.
		/// Passing one of these will return the underlying clock.
		/// </para>
		/// <para>
		/// Implementations may use a caching strategy for performance reasons.
		/// As such, it is possible that the start of the requested duration observed
		/// via this clock will be later than that observed directly via the underlying clock.
		/// </para>
		/// <para>
		/// The returned implementation is immutable, thread-safe and {@code Serializable}
		/// providing that the base clock is.
		/// 
		/// </para>
		/// </summary>
		/// <param name="baseClock">  the base clock to base the ticking clock on, not null </param>
		/// <param name="tickDuration">  the duration of each visible tick, not negative, not null </param>
		/// <returns> a clock that ticks in whole units of the duration, not null </returns>
		/// <exception cref="IllegalArgumentException"> if the duration is negative, or has a
		///  part smaller than a whole millisecond such that the whole duration is not
		///  divisible into one second </exception>
		/// <exception cref="ArithmeticException"> if the duration is too large to be represented as nanos </exception>
		public static Clock Tick(Clock baseClock, Duration tickDuration)
		{
			Objects.RequireNonNull(baseClock, "baseClock");
			Objects.RequireNonNull(tickDuration, "tickDuration");
			if (tickDuration.Negative)
			{
				throw new IllegalArgumentException("Tick duration must not be negative");
			}
			long tickNanos = tickDuration.ToNanos();
			if (tickNanos % 1000000 == 0)
			{
				// ok, no fraction of millisecond
			}
			else if (1000000000 % tickNanos == 0)
			{
				// ok, divides into one second without remainder
			}
			else
			{
				throw new IllegalArgumentException("Invalid tick duration");
			}
			if (tickNanos <= 1)
			{
				return baseClock;
			}
			return new TickClock(baseClock, tickNanos);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains a clock that always returns the same instant.
		/// <para>
		/// This clock simply returns the specified instant.
		/// As such, it is not a clock in the conventional sense.
		/// The main use case for this is in testing, where the fixed clock ensures
		/// tests are not dependent on the current clock.
		/// </para>
		/// <para>
		/// The returned implementation is immutable, thread-safe and {@code Serializable}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="fixedInstant">  the instant to use as the clock, not null </param>
		/// <param name="zone">  the time-zone to use to convert the instant to date-time, not null </param>
		/// <returns> a clock that always returns the same instant, not null </returns>
		public static Clock @fixed(Instant fixedInstant, ZoneId zone)
		{
			Objects.RequireNonNull(fixedInstant, "fixedInstant");
			Objects.RequireNonNull(zone, "zone");
			return new FixedClock(fixedInstant, zone);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Obtains a clock that returns instants from the specified clock with the
		/// specified duration added
		/// <para>
		/// This clock wraps another clock, returning instants that are later by the
		/// specified duration. If the duration is negative, the instants will be
		/// earlier than the current date and time.
		/// The main use case for this is to simulate running in the future or in the past.
		/// </para>
		/// <para>
		/// A duration of zero would have no offsetting effect.
		/// Passing zero will return the underlying clock.
		/// </para>
		/// <para>
		/// The returned implementation is immutable, thread-safe and {@code Serializable}
		/// providing that the base clock is.
		/// 
		/// </para>
		/// </summary>
		/// <param name="baseClock">  the base clock to add the duration to, not null </param>
		/// <param name="offsetDuration">  the duration to add, not null </param>
		/// <returns> a clock based on the base clock with the duration added, not null </returns>
		public static Clock Offset(Clock baseClock, Duration offsetDuration)
		{
			Objects.RequireNonNull(baseClock, "baseClock");
			Objects.RequireNonNull(offsetDuration, "offsetDuration");
			if (offsetDuration.Equals(Duration.ZERO))
			{
				return baseClock;
			}
			return new OffsetClock(baseClock, offsetDuration);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Constructor accessible by subclasses.
		/// </summary>
		protected internal Clock()
		{
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the time-zone being used to create dates and times.
		/// <para>
		/// A clock will typically obtain the current instant and then convert that
		/// to a date or time using a time-zone. This method returns the time-zone used.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the time-zone being used to interpret instants, not null </returns>
		public abstract ZoneId Zone {get;}

		/// <summary>
		/// Returns a copy of this clock with a different time-zone.
		/// <para>
		/// A clock will typically obtain the current instant and then convert that
		/// to a date or time using a time-zone. This method returns a clock with
		/// similar properties but using a different time-zone.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the time-zone to change to, not null </param>
		/// <returns> a clock based on this clock with the specified time-zone, not null </returns>
		public abstract Clock WithZone(ZoneId zone);

		//-------------------------------------------------------------------------
		/// <summary>
		/// Gets the current millisecond instant of the clock.
		/// <para>
		/// This returns the millisecond-based instant, measured from 1970-01-01T00:00Z (UTC).
		/// This is equivalent to the definition of <seealso cref="System#currentTimeMillis()"/>.
		/// </para>
		/// <para>
		/// Most applications should avoid this method and use <seealso cref="Instant"/> to represent
		/// an instant on the time-line rather than a raw millisecond value.
		/// This method is provided to allow the use of the clock in high performance use cases
		/// where the creation of an object would be unacceptable.
		/// </para>
		/// <para>
		/// The default implementation currently calls <seealso cref="#instant"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current millisecond instant from this clock, measured from
		///  the Java epoch of 1970-01-01T00:00Z (UTC), not null </returns>
		/// <exception cref="DateTimeException"> if the instant cannot be obtained, not thrown by most implementations </exception>
		public virtual long Millis()
		{
			return Instant().ToEpochMilli();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the current instant of the clock.
		/// <para>
		/// This returns an instant representing the current instant as defined by the clock.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the current instant from this clock, not null </returns>
		/// <exception cref="DateTimeException"> if the instant cannot be obtained, not thrown by most implementations </exception>
		public abstract Instant Instant();

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this clock is equal to another clock.
		/// <para>
		/// Clocks should override this method to compare equals based on
		/// their state and to meet the contract of <seealso cref="Object#equals"/>.
		/// If not overridden, the behavior is defined by <seealso cref="Object#equals"/>
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other clock </returns>
		public override bool Equals(Object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// A hash code for this clock.
		/// <para>
		/// Clocks should override this method based on
		/// their state and to meet the contract of <seealso cref="Object#hashCode"/>.
		/// If not overridden, the behavior is defined by <seealso cref="Object#hashCode"/>
		/// 
		/// </para>
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return base.HashCode();
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implementation of a clock that always returns the latest time from
		/// <seealso cref="System#currentTimeMillis()"/>.
		/// </summary>
		[Serializable]
		internal sealed class SystemClock : Clock
		{
			internal const long SerialVersionUID = 6740630888130243051L;
			internal readonly ZoneId Zone_Renamed;

			internal SystemClock(ZoneId zone)
			{
				this.Zone_Renamed = zone;
			}
			public override ZoneId Zone
			{
				get
				{
					return Zone_Renamed;
				}
			}
			public override Clock WithZone(ZoneId zone)
			{
				if (zone.Equals(this.Zone_Renamed)) // intentional NPE
				{
					return this;
				}
				return new SystemClock(zone);
			}
			public override long Millis()
			{
				return DateTimeHelperClass.CurrentUnixTimeMillis();
			}
			public override Instant Instant()
			{
				return Instant.OfEpochMilli(Millis());
			}
			public override bool Equals(Object obj)
			{
				if (obj is SystemClock)
				{
					return Zone_Renamed.Equals(((SystemClock) obj).Zone_Renamed);
				}
				return false;
			}
			public override int HashCode()
			{
				return Zone_Renamed.HashCode() + 1;
			}
			public override String ToString()
			{
				return "SystemClock[" + Zone_Renamed + "]";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implementation of a clock that always returns the same instant.
		/// This is typically used for testing.
		/// </summary>
		[Serializable]
		internal sealed class FixedClock : Clock
		{
		   internal const long SerialVersionUID = 7430389292664866958L;
			internal readonly Instant Instant_Renamed;
			internal readonly ZoneId Zone_Renamed;

			internal FixedClock(Instant fixedInstant, ZoneId zone)
			{
				this.Instant_Renamed = fixedInstant;
				this.Zone_Renamed = zone;
			}
			public override ZoneId Zone
			{
				get
				{
					return Zone_Renamed;
				}
			}
			public override Clock WithZone(ZoneId zone)
			{
				if (zone.Equals(this.Zone_Renamed)) // intentional NPE
				{
					return this;
				}
				return new FixedClock(Instant_Renamed, zone);
			}
			public override long Millis()
			{
				return Instant_Renamed.ToEpochMilli();
			}
			public override Instant Instant()
			{
				return Instant_Renamed;
			}
			public override bool Equals(Object obj)
			{
				if (obj is FixedClock)
				{
					FixedClock other = (FixedClock) obj;
					return Instant_Renamed.Equals(other.Instant_Renamed) && Zone_Renamed.Equals(other.Zone_Renamed);
				}
				return false;
			}
			public override int HashCode()
			{
				return Instant_Renamed.HashCode() ^ Zone_Renamed.HashCode();
			}
			public override String ToString()
			{
				return "FixedClock[" + Instant_Renamed + "," + Zone_Renamed + "]";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implementation of a clock that adds an offset to an underlying clock.
		/// </summary>
		[Serializable]
		internal sealed class OffsetClock : Clock
		{
		   internal const long SerialVersionUID = 2007484719125426256L;
			internal readonly Clock BaseClock;
			internal readonly Duration Offset;

			internal OffsetClock(Clock baseClock, Duration offset)
			{
				this.BaseClock = baseClock;
				this.Offset = offset;
			}
			public override ZoneId Zone
			{
				get
				{
					return BaseClock.Zone;
				}
			}
			public override Clock WithZone(ZoneId zone)
			{
				if (zone.Equals(BaseClock.Zone)) // intentional NPE
				{
					return this;
				}
				return new OffsetClock(BaseClock.WithZone(zone), Offset);
			}
			public override long Millis()
			{
				return Math.AddExact(BaseClock.Millis(), Offset.ToMillis());
			}
			public override Instant Instant()
			{
				return BaseClock.Instant().Plus(Offset);
			}
			public override bool Equals(Object obj)
			{
				if (obj is OffsetClock)
				{
					OffsetClock other = (OffsetClock) obj;
					return BaseClock.Equals(other.BaseClock) && Offset.Equals(other.Offset);
				}
				return false;
			}
			public override int HashCode()
			{
				return BaseClock.HashCode() ^ Offset.HashCode();
			}
			public override String ToString()
			{
				return "OffsetClock[" + BaseClock + "," + Offset + "]";
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implementation of a clock that adds an offset to an underlying clock.
		/// </summary>
		[Serializable]
		internal sealed class TickClock : Clock
		{
			internal const long SerialVersionUID = 6504659149906368850L;
			internal readonly Clock BaseClock;
			internal readonly long TickNanos;

			internal TickClock(Clock baseClock, long tickNanos)
			{
				this.BaseClock = baseClock;
				this.TickNanos = tickNanos;
			}
			public override ZoneId Zone
			{
				get
				{
					return BaseClock.Zone;
				}
			}
			public override Clock WithZone(ZoneId zone)
			{
				if (zone.Equals(BaseClock.Zone)) // intentional NPE
				{
					return this;
				}
				return new TickClock(BaseClock.WithZone(zone), TickNanos);
			}
			public override long Millis()
			{
				long millis = BaseClock.Millis();
				return millis - Math.FloorMod(millis, TickNanos / 1000_000L);
			}
			public override Instant Instant()
			{
				if ((TickNanos % 1000000) == 0)
				{
					long millis = BaseClock.Millis();
					return Instant.OfEpochMilli(millis - Math.FloorMod(millis, TickNanos / 1000_000L));
				}
				Instant instant = BaseClock.Instant();
				long nanos = instant.Nano;
				long adjust = Math.FloorMod(nanos, TickNanos);
				return instant.MinusNanos(adjust);
			}
			public override bool Equals(Object obj)
			{
				if (obj is TickClock)
				{
					TickClock other = (TickClock) obj;
					return BaseClock.Equals(other.BaseClock) && TickNanos == other.TickNanos;
				}
				return false;
			}
			public override int HashCode()
			{
				return BaseClock.HashCode() ^ ((int)(TickNanos ^ ((long)((ulong)TickNanos >> 32))));
			}
			public override String ToString()
			{
				return "TickClock[" + BaseClock + "," + Duration.OfNanos(TickNanos) + "]";
			}
		}

	}

}