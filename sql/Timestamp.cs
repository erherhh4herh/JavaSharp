using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.sql
{


	/// <summary>
	/// <P>A thin wrapper around <code>java.util.Date</code> that allows
	/// the JDBC API to identify this as an SQL <code>TIMESTAMP</code> value.
	/// It adds the ability
	/// to hold the SQL <code>TIMESTAMP</code> fractional seconds value, by allowing
	/// the specification of fractional seconds to a precision of nanoseconds.
	/// A Timestamp also provides formatting and
	/// parsing operations to support the JDBC escape syntax for timestamp values.
	/// 
	/// <para>The precision of a Timestamp object is calculated to be either:
	/// <ul>
	/// <li><code>19 </code>, which is the number of characters in yyyy-mm-dd hh:mm:ss
	/// <li> <code> 20 + s </code>, which is the number
	/// of characters in the yyyy-mm-dd hh:mm:ss.[fff...] and <code>s</code> represents  the scale of the given Timestamp,
	/// its fractional seconds precision.
	/// </ul>
	/// 
	/// <P><B>Note:</B> This type is a composite of a <code>java.util.Date</code> and a
	/// separate nanoseconds value. Only integral seconds are stored in the
	/// <code>java.util.Date</code> component. The fractional seconds - the nanos - are
	/// separate.  The <code>Timestamp.equals(Object)</code> method never returns
	/// <code>true</code> when passed an object
	/// that isn't an instance of <code>java.sql.Timestamp</code>,
	/// because the nanos component of a date is unknown.
	/// As a result, the <code>Timestamp.equals(Object)</code>
	/// method is not symmetric with respect to the
	/// <code>java.util.Date.equals(Object)</code>
	/// method.  Also, the <code>hashCode</code> method uses the underlying
	/// <code>java.util.Date</code>
	/// implementation and therefore does not include nanos in its computation.
	/// <P>
	/// Due to the differences between the <code>Timestamp</code> class
	/// and the <code>java.util.Date</code>
	/// class mentioned above, it is recommended that code not view
	/// <code>Timestamp</code> values generically as an instance of
	/// <code>java.util.Date</code>.  The
	/// inheritance relationship between <code>Timestamp</code>
	/// and <code>java.util.Date</code> really
	/// denotes implementation inheritance, and not type inheritance.
	/// </para>
	/// </summary>
	public class Timestamp : DateTime
	{

		/// <summary>
		/// Constructs a <code>Timestamp</code> object initialized
		/// with the given values.
		/// </summary>
		/// <param name="year"> the year minus 1900 </param>
		/// <param name="month"> 0 to 11 </param>
		/// <param name="date"> 1 to 31 </param>
		/// <param name="hour"> 0 to 23 </param>
		/// <param name="minute"> 0 to 59 </param>
		/// <param name="second"> 0 to 59 </param>
		/// <param name="nano"> 0 to 999,999,999 </param>
		/// @deprecated instead use the constructor <code>Timestamp(long millis)</code> 
		/// <exception cref="IllegalArgumentException"> if the nano argument is out of bounds </exception>
		[Obsolete("instead use the constructor <code>Timestamp(long millis)</code>")]
		public Timestamp(int year, int month, int date, int hour, int minute, int second, int nano) : base(year, month, date, hour, minute, second)
		{
			if (nano > 999999999 || nano < 0)
			{
				throw new IllegalArgumentException("nanos > 999999999 or < 0");
			}
			Nanos_Renamed = nano;
		}

		/// <summary>
		/// Constructs a <code>Timestamp</code> object
		/// using a milliseconds time value. The
		/// integral seconds are stored in the underlying date value; the
		/// fractional seconds are stored in the <code>nanos</code> field of
		/// the <code>Timestamp</code> object.
		/// </summary>
		/// <param name="time"> milliseconds since January 1, 1970, 00:00:00 GMT.
		///        A negative number is the number of milliseconds before
		///         January 1, 1970, 00:00:00 GMT. </param>
		/// <seealso cref= java.util.Calendar </seealso>
		public Timestamp(long time) : base((time / 1000) * 1000)
		{
			Nanos_Renamed = (int)((time % 1000) * 1000000);
			if (Nanos_Renamed < 0)
			{
				Nanos_Renamed = 1000000000 + Nanos_Renamed;
				base = new DateTime(((time / 1000) - 1) * 1000);
			}
		}

		/// <summary>
		/// Sets this <code>Timestamp</code> object to represent a point in time that is
		/// <tt>time</tt> milliseconds after January 1, 1970 00:00:00 GMT.
		/// </summary>
		/// <param name="time">   the number of milliseconds. </param>
		/// <seealso cref= #getTime </seealso>
		/// <seealso cref= #Timestamp(long time) </seealso>
		/// <seealso cref= java.util.Calendar </seealso>
		public override long Time
		{
			set
			{
				base = new DateTime((value / 1000) * 1000);
				Nanos_Renamed = (int)((value % 1000) * 1000000);
				if (Nanos_Renamed < 0)
				{
					Nanos_Renamed = 1000000000 + Nanos_Renamed;
					base = new DateTime(((value / 1000) - 1) * 1000);
				}
			}
			get
			{
				long time = base.Ticks;
				return (time + (Nanos_Renamed / 1000000));
			}
		}



		/// <summary>
		/// @serial
		/// </summary>
		private int Nanos_Renamed;

		/// <summary>
		/// Converts a <code>String</code> object in JDBC timestamp escape format to a
		/// <code>Timestamp</code> value.
		/// </summary>
		/// <param name="s"> timestamp in format <code>yyyy-[m]m-[d]d hh:mm:ss[.f...]</code>.  The
		/// fractional seconds may be omitted. The leading zero for <code>mm</code>
		/// and <code>dd</code> may also be omitted.
		/// </param>
		/// <returns> corresponding <code>Timestamp</code> value </returns>
		/// <exception cref="java.lang.IllegalArgumentException"> if the given argument
		/// does not have the format <code>yyyy-[m]m-[d]d hh:mm:ss[.f...]</code> </exception>
		public static Timestamp ValueOf(String s)
		{
			const int YEAR_LENGTH = 4;
			const int MONTH_LENGTH = 2;
			const int DAY_LENGTH = 2;
			const int MAX_MONTH = 12;
			const int MAX_DAY = 31;
			String date_s;
			String time_s;
			String nanos_s;
			int year = 0;
			int month = 0;
			int day = 0;
			int hour;
			int minute;
			int second;
			int a_nanos = 0;
			int firstDash;
			int secondDash;
			int dividingSpace;
			int firstColon = 0;
			int secondColon = 0;
			int period = 0;
			String formatError = "Timestamp format must be yyyy-mm-dd hh:mm:ss[.fffffffff]";
			String zeros = "000000000";
			String delimiterDate = "-";
			String delimiterTime = ":";

			if (s == null)
			{
				throw new System.ArgumentException("null string");
			}

			// Split the string into date and time components
			s = s.Trim();
			dividingSpace = s.IndexOf(' ');
			if (dividingSpace > 0)
			{
				date_s = s.Substring(0,dividingSpace);
				time_s = s.Substring(dividingSpace+1);
			}
			else
			{
				throw new System.ArgumentException(formatError);
			}

			// Parse the date
			firstDash = date_s.IndexOf('-');
			secondDash = date_s.IndexOf('-', firstDash + 1);

			// Parse the time
			if (time_s == null)
			{
				throw new System.ArgumentException(formatError);
			}
			firstColon = time_s.IndexOf(':');
			secondColon = time_s.IndexOf(':', firstColon + 1);
			period = time_s.IndexOf('.', secondColon + 1);

			// Convert the date
			bool parsedDate = false;
			if ((firstDash > 0) && (secondDash > 0) && (secondDash < date_s.Length() - 1))
			{
				String yyyy = date_s.Substring(0, firstDash);
				String mm = StringHelperClass.SubstringSpecial(date_s, firstDash + 1, secondDash);
				String dd = date_s.Substring(secondDash + 1);
				if (yyyy.Length() == YEAR_LENGTH && (mm.Length() >= 1 && mm.Length() <= MONTH_LENGTH) && (dd.Length() >= 1 && dd.Length() <= DAY_LENGTH))
				{
					 year = Convert.ToInt32(yyyy);
					 month = Convert.ToInt32(mm);
					 day = Convert.ToInt32(dd);

					if ((month >= 1 && month <= MAX_MONTH) && (day >= 1 && day <= MAX_DAY))
					{
						parsedDate = true;
					}
				}
			}
			if (!parsedDate)
			{
				throw new System.ArgumentException(formatError);
			}

			// Convert the time; default missing nanos
			if ((firstColon > 0) & (secondColon > 0) & (secondColon < time_s.Length() - 1))
			{
				hour = Convert.ToInt32(time_s.Substring(0, firstColon));
				minute = Convert.ToInt32(StringHelperClass.SubstringSpecial(time_s, firstColon + 1, secondColon));
				if ((period > 0) & (period < time_s.Length() - 1))
				{
					second = Convert.ToInt32(StringHelperClass.SubstringSpecial(time_s, secondColon + 1, period));
					nanos_s = time_s.Substring(period + 1);
					if (nanos_s.Length() > 9)
					{
						throw new System.ArgumentException(formatError);
					}
					if (!char.IsDigit(nanos_s.CharAt(0)))
					{
						throw new System.ArgumentException(formatError);
					}
					nanos_s = nanos_s + zeros.Substring(0,9 - nanos_s.Length());
					a_nanos = Convert.ToInt32(nanos_s);
				}
				else if (period > 0)
				{
					throw new System.ArgumentException(formatError);
				}
				else
				{
					second = Convert.ToInt32(time_s.Substring(secondColon + 1));
				}
			}
			else
			{
				throw new System.ArgumentException(formatError);
			}

			return new Timestamp(year - 1900, month - 1, day, hour, minute, second, a_nanos);
		}

		/// <summary>
		/// Formats a timestamp in JDBC timestamp escape format.
		///         <code>yyyy-mm-dd hh:mm:ss.fffffffff</code>,
		/// where <code>ffffffffff</code> indicates nanoseconds.
		/// <P> </summary>
		/// <returns> a <code>String</code> object in
		///           <code>yyyy-mm-dd hh:mm:ss.fffffffff</code> format </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public String toString()
		public override String ToString()
		{

			int year = base.Year + 1900;
			int month = base.Month + 1;
			int day = base.Date;
			int hour = base.Hour;
			int minute = base.Minute;
			int second = base.Second;
			String yearString;
			String monthString;
			String dayString;
			String hourString;
			String minuteString;
			String secondString;
			String nanosString;
			String zeros = "000000000";
			String yearZeros = "0000";
			StringBuffer timestampBuf;

			if (year < 1000)
			{
				// Add leading zeros
				yearString = "" + year;
				yearString = yearZeros.Substring(0, (4 - yearString.Length())) + yearString;
			}
			else
			{
				yearString = "" + year;
			}
			if (month < 10)
			{
				monthString = "0" + month;
			}
			else
			{
				monthString = Convert.ToString(month);
			}
			if (day < 10)
			{
				dayString = "0" + day;
			}
			else
			{
				dayString = Convert.ToString(day);
			}
			if (hour < 10)
			{
				hourString = "0" + hour;
			}
			else
			{
				hourString = Convert.ToString(hour);
			}
			if (minute < 10)
			{
				minuteString = "0" + minute;
			}
			else
			{
				minuteString = Convert.ToString(minute);
			}
			if (second < 10)
			{
				secondString = "0" + second;
			}
			else
			{
				secondString = Convert.ToString(second);
			}
			if (Nanos_Renamed == 0)
			{
				nanosString = "0";
			}
			else
			{
				nanosString = Convert.ToString(Nanos_Renamed);

				// Add leading zeros
				nanosString = zeros.Substring(0, (9 - nanosString.Length())) + nanosString;

				// Truncate trailing zeros
				char[] nanosChar = new char[nanosString.Length()];
				nanosString.GetChars(0, nanosString.Length(), nanosChar, 0);
				int truncIndex = 8;
				while (nanosChar[truncIndex] == '0')
				{
					truncIndex--;
				}

				nanosString = new String(nanosChar, 0, truncIndex + 1);
			}

			// do a string buffer here instead.
			timestampBuf = new StringBuffer(20 + nanosString.Length());
			timestampBuf.Append(yearString);
			timestampBuf.Append("-");
			timestampBuf.Append(monthString);
			timestampBuf.Append("-");
			timestampBuf.Append(dayString);
			timestampBuf.Append(" ");
			timestampBuf.Append(hourString);
			timestampBuf.Append(":");
			timestampBuf.Append(minuteString);
			timestampBuf.Append(":");
			timestampBuf.Append(secondString);
			timestampBuf.Append(".");
			timestampBuf.Append(nanosString);

			return (timestampBuf.ToString());
		}

		/// <summary>
		/// Gets this <code>Timestamp</code> object's <code>nanos</code> value.
		/// </summary>
		/// <returns> this <code>Timestamp</code> object's fractional seconds component </returns>
		/// <seealso cref= #setNanos </seealso>
		public virtual int Nanos
		{
			get
			{
				return Nanos_Renamed;
			}
			set
			{
				if (value > 999999999 || value < 0)
				{
					throw new IllegalArgumentException("nanos > 999999999 or < 0");
				}
				Nanos_Renamed = value;
			}
		}


		/// <summary>
		/// Tests to see if this <code>Timestamp</code> object is
		/// equal to the given <code>Timestamp</code> object.
		/// </summary>
		/// <param name="ts"> the <code>Timestamp</code> value to compare with </param>
		/// <returns> <code>true</code> if the given <code>Timestamp</code>
		///         object is equal to this <code>Timestamp</code> object;
		///         <code>false</code> otherwise </returns>
		public virtual bool Equals(Timestamp ts)
		{
			if (base.Equals(ts))
			{
				if (Nanos_Renamed == ts.Nanos_Renamed)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Tests to see if this <code>Timestamp</code> object is
		/// equal to the given object.
		/// 
		/// This version of the method <code>equals</code> has been added
		/// to fix the incorrect
		/// signature of <code>Timestamp.equals(Timestamp)</code> and to preserve backward
		/// compatibility with existing class files.
		/// 
		/// Note: This method is not symmetric with respect to the
		/// <code>equals(Object)</code> method in the base class.
		/// </summary>
		/// <param name="ts"> the <code>Object</code> value to compare with </param>
		/// <returns> <code>true</code> if the given <code>Object</code> is an instance
		///         of a <code>Timestamp</code> that
		///         is equal to this <code>Timestamp</code> object;
		///         <code>false</code> otherwise </returns>
		public override bool Equals(object ts)
		{
		  if (ts is Timestamp)
		  {
			return this.Equals((Timestamp)ts);
		  }
		  else
		  {
			return false;
		  }
		}

		/// <summary>
		/// Indicates whether this <code>Timestamp</code> object is
		/// earlier than the given <code>Timestamp</code> object.
		/// </summary>
		/// <param name="ts"> the <code>Timestamp</code> value to compare with </param>
		/// <returns> <code>true</code> if this <code>Timestamp</code> object is earlier;
		///        <code>false</code> otherwise </returns>
		public virtual bool Before(Timestamp ts)
		{
			return CompareTo(ts) < 0;
		}

		/// <summary>
		/// Indicates whether this <code>Timestamp</code> object is
		/// later than the given <code>Timestamp</code> object.
		/// </summary>
		/// <param name="ts"> the <code>Timestamp</code> value to compare with </param>
		/// <returns> <code>true</code> if this <code>Timestamp</code> object is later;
		///        <code>false</code> otherwise </returns>
		public virtual bool After(Timestamp ts)
		{
			return CompareTo(ts) > 0;
		}

		/// <summary>
		/// Compares this <code>Timestamp</code> object to the given
		/// <code>Timestamp</code> object.
		/// </summary>
		/// <param name="ts">   the <code>Timestamp</code> object to be compared to
		///                this <code>Timestamp</code> object </param>
		/// <returns>  the value <code>0</code> if the two <code>Timestamp</code>
		///          objects are equal; a value less than <code>0</code> if this
		///          <code>Timestamp</code> object is before the given argument;
		///          and a value greater than <code>0</code> if this
		///          <code>Timestamp</code> object is after the given argument.
		/// @since   1.4 </returns>
		public virtual int CompareTo(Timestamp ts)
		{
			long thisTime = this.Ticks;
			long anotherTime = ts.Ticks;
			int i = (thisTime < anotherTime ? - 1 :(thisTime == anotherTime?0 :1));
			if (i == 0)
			{
				if (Nanos_Renamed > ts.Nanos_Renamed)
				{
						return 1;
				}
				else if (Nanos_Renamed < ts.Nanos_Renamed)
				{
					return -1;
				}
			}
			return i;
		}

		/// <summary>
		/// Compares this <code>Timestamp</code> object to the given
		/// <code>Date</code> object.
		/// </summary>
		/// <param name="o"> the <code>Date</code> to be compared to
		///          this <code>Timestamp</code> object </param>
		/// <returns>  the value <code>0</code> if this <code>Timestamp</code> object
		///          and the given object are equal; a value less than <code>0</code>
		///          if this  <code>Timestamp</code> object is before the given argument;
		///          and a value greater than <code>0</code> if this
		///          <code>Timestamp</code> object is after the given argument.
		/// 
		/// @since   1.5 </returns>
		public override int CompareTo(DateTime o)
		{
		   if (o is Timestamp)
		   {
				// When Timestamp instance compare it with a Timestamp
				// Hence it is basically calling this.compareTo((Timestamp))o);
				// Note typecasting is safe because o is instance of Timestamp
			   return CompareTo((Timestamp)o);
		   }
		  else
		  {
				// When Date doing a o.compareTo(this)
				// will give wrong results.
			  Timestamp ts = new Timestamp(o.Ticks);
			  return this.CompareTo(ts);
		  }
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The {@code hashCode} method uses the underlying {@code java.util.Date}
		/// implementation and therefore does not include nanos in its computation.
		/// 
		/// </summary>
		public override int HashCode()
		{
			return base.HashCode();
		}

		internal const long SerialVersionUID = 2745179027874758501L;

		private const int MILLIS_PER_SECOND = 1000;

		/// <summary>
		/// Obtains an instance of {@code Timestamp} from a {@code LocalDateTime}
		/// object, with the same year, month, day of month, hours, minutes,
		/// seconds and nanos date-time value as the provided {@code LocalDateTime}.
		/// <para>
		/// The provided {@code LocalDateTime} is interpreted as the local
		/// date-time in the local time zone.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dateTime"> a {@code LocalDateTime} to convert </param>
		/// <returns> a {@code Timestamp} object </returns>
		/// <exception cref="NullPointerException"> if {@code dateTime} is null.
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public static Timestamp valueOf(java.time.LocalDateTime dateTime)
		public static Timestamp ValueOf(LocalDateTime dateTime)
		{
			return new Timestamp(dateTime.Year - 1900, dateTime.MonthValue - 1, dateTime.DayOfMonth, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Nano);
		}

		/// <summary>
		/// Converts this {@code Timestamp} object to a {@code LocalDateTime}.
		/// <para>
		/// The conversion creates a {@code LocalDateTime} that represents the
		/// same year, month, day of month, hours, minutes, seconds and nanos
		/// date-time value as this {@code Timestamp} in the local time zone.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code LocalDateTime} object representing the same date-time value
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public java.time.LocalDateTime toLocalDateTime()
		public virtual LocalDateTime ToLocalDateTime()
		{
			return LocalDateTime.Of(Year + 1900, Month + 1, Date, Hours, Minutes, Seconds, Nanos);
		}

		/// <summary>
		/// Obtains an instance of {@code Timestamp} from an <seealso cref="Instant"/> object.
		/// <para>
		/// {@code Instant} can store points on the time-line further in the future
		/// and further in the past than {@code Date}. In this scenario, this method
		/// will throw an exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="instant">  the instant to convert </param>
		/// <returns> an {@code Timestamp} representing the same point on the time-line as
		///  the provided instant </returns>
		/// <exception cref="NullPointerException"> if {@code instant} is null. </exception>
		/// <exception cref="IllegalArgumentException"> if the instant is too large to
		///  represent as a {@code Timesamp}
		/// @since 1.8 </exception>
		public static Timestamp From(Instant instant)
		{
			try
			{
				Timestamp stamp = new Timestamp(instant.EpochSecond * MILLIS_PER_SECOND);
				stamp.Nanos_Renamed = instant.Nano;
				return stamp;
			}
			catch (ArithmeticException ex)
			{
				throw new IllegalArgumentException(ex);
			}
		}

		/// <summary>
		/// Converts this {@code Timestamp} object to an {@code Instant}.
		/// <para>
		/// The conversion creates an {@code Instant} that represents the same
		/// point on the time-line as this {@code Timestamp}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an instant representing the same point on the time-line
		/// @since 1.8 </returns>
		public override Instant ToInstant()
		{
			return Instant.OfEpochSecond(base.Ticks / MILLIS_PER_SECOND, Nanos_Renamed);
		}
	}

}