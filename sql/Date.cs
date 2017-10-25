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
	/// <P>A thin wrapper around a millisecond value that allows
	/// JDBC to identify this as an SQL <code>DATE</code> value.  A
	/// milliseconds value represents the number of milliseconds that
	/// have passed since January 1, 1970 00:00:00.000 GMT.
	/// <para>
	/// To conform with the definition of SQL <code>DATE</code>, the
	/// millisecond values wrapped by a <code>java.sql.Date</code> instance
	/// must be 'normalized' by setting the
	/// hours, minutes, seconds, and milliseconds to zero in the particular
	/// time zone with which the instance is associated.
	/// </para>
	/// </summary>
	public class Date : DateTime
	{

		/// <summary>
		/// Constructs a <code>Date</code> object initialized with the given
		/// year, month, and day.
		/// <P>
		/// The result is undefined if a given argument is out of bounds.
		/// </summary>
		/// <param name="year"> the year minus 1900; must be 0 to 8099. (Note that
		///        8099 is 9999 minus 1900.) </param>
		/// <param name="month"> 0 to 11 </param>
		/// <param name="day"> 1 to 31 </param>
		/// @deprecated instead use the constructor <code>Date(long date)</code> 
		[Obsolete("instead use the constructor <code>Date(long date)</code>")]
		public Date(int year, int month, int day) : base(year, month, day)
		{
		}

		/// <summary>
		/// Constructs a <code>Date</code> object using the given milliseconds
		/// time value.  If the given milliseconds value contains time
		/// information, the driver will set the time components to the
		/// time in the default time zone (the time zone of the Java virtual
		/// machine running the application) that corresponds to zero GMT.
		/// </summary>
		/// <param name="date"> milliseconds since January 1, 1970, 00:00:00 GMT not
		///        to exceed the milliseconds representation for the year 8099.
		///        A negative number indicates the number of milliseconds
		///        before January 1, 1970, 00:00:00 GMT. </param>
		public Date(long date) : base(date)
		{
			// If the millisecond date value contains time info, mask it out.

		}

		/// <summary>
		/// Sets an existing <code>Date</code> object
		/// using the given milliseconds time value.
		/// If the given milliseconds value contains time information,
		/// the driver will set the time components to the
		/// time in the default time zone (the time zone of the Java virtual
		/// machine running the application) that corresponds to zero GMT.
		/// </summary>
		/// <param name="date"> milliseconds since January 1, 1970, 00:00:00 GMT not
		///        to exceed the milliseconds representation for the year 8099.
		///        A negative number indicates the number of milliseconds
		///        before January 1, 1970, 00:00:00 GMT. </param>
		public override long Time
		{
			set
			{
				// If the millisecond value value contains time info, mask it out.
				base = new DateTime(value);
			}
		}

		/// <summary>
		/// Converts a string in JDBC date escape format to
		/// a <code>Date</code> value.
		/// </summary>
		/// <param name="s"> a <code>String</code> object representing a date in
		///        in the format "yyyy-[m]m-[d]d". The leading zero for <code>mm</code>
		/// and <code>dd</code> may also be omitted. </param>
		/// <returns> a <code>java.sql.Date</code> object representing the
		///         given date </returns>
		/// <exception cref="IllegalArgumentException"> if the date given is not in the
		///         JDBC date escape format (yyyy-[m]m-[d]d) </exception>
		public static Date ValueOf(String s)
		{
			const int YEAR_LENGTH = 4;
			const int MONTH_LENGTH = 2;
			const int DAY_LENGTH = 2;
			const int MAX_MONTH = 12;
			const int MAX_DAY = 31;
			int firstDash;
			int secondDash;
			Date d = null;
			if (s == null)
			{
				throw new System.ArgumentException();
			}

			firstDash = s.IndexOf('-');
			secondDash = s.IndexOf('-', firstDash + 1);

			if ((firstDash > 0) && (secondDash > 0) && (secondDash < s.Length() - 1))
			{
				String yyyy = s.Substring(0, firstDash);
				String mm = StringHelperClass.SubstringSpecial(s, firstDash + 1, secondDash);
				String dd = s.Substring(secondDash + 1);
				if (yyyy.Length() == YEAR_LENGTH && (mm.Length() >= 1 && mm.Length() <= MONTH_LENGTH) && (dd.Length() >= 1 && dd.Length() <= DAY_LENGTH))
				{
					int year = Convert.ToInt32(yyyy);
					int month = Convert.ToInt32(mm);
					int day = Convert.ToInt32(dd);

					if ((month >= 1 && month <= MAX_MONTH) && (day >= 1 && day <= MAX_DAY))
					{
						d = new Date(year - 1900, month - 1, day);
					}
				}
			}
			if (d == null)
			{
				throw new System.ArgumentException();
			}

			return d;

		}


		/// <summary>
		/// Formats a date in the date escape format yyyy-mm-dd.
		/// <P> </summary>
		/// <returns> a String in yyyy-mm-dd format </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public String toString()
		public override String ToString()
		{
			int year = base.Year + 1900;
			int month = base.Month + 1;
			int day = base.Date;

			char[] buf = "2000-00-00".ToCharArray();
			buf[0] = Character.ForDigit(year / 1000,10);
			buf[1] = Character.ForDigit((year / 100) % 10,10);
			buf[2] = Character.ForDigit((year / 10) % 10,10);
			buf[3] = Character.ForDigit(year % 10,10);
			buf[5] = Character.ForDigit(month / 10,10);
			buf[6] = Character.ForDigit(month % 10,10);
			buf[8] = Character.ForDigit(day / 10,10);
			buf[9] = Character.ForDigit(day % 10,10);

			return new String(buf);
		}

		// Override all the time operations inherited from java.util.Date;

	   /// <summary>
	   /// This method is deprecated and should not be used because SQL Date
	   /// values do not have a time component.
	   /// 
	   /// @deprecated </summary>
	   /// <exception cref="java.lang.IllegalArgumentException"> if this method is invoked </exception>
	   /// <seealso cref= #setHours </seealso>
		[Obsolete]
		public override int Hours
		{
			get
			{
				throw new System.ArgumentException();
			}
			set
			{
				throw new System.ArgumentException();
			}
		}

	   /// <summary>
	   /// This method is deprecated and should not be used because SQL Date
	   /// values do not have a time component.
	   /// 
	   /// @deprecated </summary>
	   /// <exception cref="java.lang.IllegalArgumentException"> if this method is invoked </exception>
	   /// <seealso cref= #setMinutes </seealso>
		[Obsolete]
		public override int Minutes
		{
			get
			{
				throw new System.ArgumentException();
			}
			set
			{
				throw new System.ArgumentException();
			}
		}

	   /// <summary>
	   /// This method is deprecated and should not be used because SQL Date
	   /// values do not have a time component.
	   /// 
	   /// @deprecated </summary>
	   /// <exception cref="java.lang.IllegalArgumentException"> if this method is invoked </exception>
	   /// <seealso cref= #setSeconds </seealso>
		[Obsolete]
		public override int Seconds
		{
			get
			{
				throw new System.ArgumentException();
			}
			set
			{
				throw new System.ArgumentException();
			}
		}




	   /// <summary>
	   /// Private serial version unique ID to ensure serialization
	   /// compatibility.
	   /// </summary>
		internal const long SerialVersionUID = 1511598038487230103L;

		/// <summary>
		/// Obtains an instance of {@code Date} from a <seealso cref="LocalDate"/> object
		/// with the same year, month and day of month value as the given
		/// {@code LocalDate}.
		/// <para>
		/// The provided {@code LocalDate} is interpreted as the local date
		/// in the local time zone.
		/// 
		/// </para>
		/// </summary>
		/// <param name="date"> a {@code LocalDate} to convert </param>
		/// <returns> a {@code Date} object </returns>
		/// <exception cref="NullPointerException"> if {@code date} is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public static Date valueOf(java.time.LocalDate date)
		public static Date ValueOf(LocalDate date)
		{
			return new Date(date.Year - 1900, date.MonthValue -1, date.DayOfMonth);
		}

		/// <summary>
		/// Converts this {@code Date} object to a {@code LocalDate}
		/// <para>
		/// The conversion creates a {@code LocalDate} that represents the same
		/// date value as this {@code Date} in local time zone
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code LocalDate} object representing the same date value
		/// 
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public java.time.LocalDate toLocalDate()
		public virtual LocalDate ToLocalDate()
		{
			return LocalDate.Of(Year + 1900, Month + 1, Date);
		}

	   /// <summary>
	   /// This method always throws an UnsupportedOperationException and should
	   /// not be used because SQL {@code Date} values do not have a time
	   /// component.
	   /// </summary>
	   /// <exception cref="java.lang.UnsupportedOperationException"> if this method is invoked </exception>
		public override Instant ToInstant()
		{
			throw new System.NotSupportedException();
		}
	}

}