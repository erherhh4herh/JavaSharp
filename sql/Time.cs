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
	/// <P>A thin wrapper around the <code>java.util.Date</code> class that allows the JDBC
	/// API to identify this as an SQL <code>TIME</code> value. The <code>Time</code>
	/// class adds formatting and
	/// parsing operations to support the JDBC escape syntax for time
	/// values.
	/// <para>The date components should be set to the "zero epoch"
	/// value of January 1, 1970 and should not be accessed.
	/// </para>
	/// </summary>
	public class Time : DateTime
	{

		/// <summary>
		/// Constructs a <code>Time</code> object initialized with the
		/// given values for the hour, minute, and second.
		/// The driver sets the date components to January 1, 1970.
		/// Any method that attempts to access the date components of a
		/// <code>Time</code> object will throw a
		/// <code>java.lang.IllegalArgumentException</code>.
		/// <P>
		/// The result is undefined if a given argument is out of bounds.
		/// </summary>
		/// <param name="hour"> 0 to 23 </param>
		/// <param name="minute"> 0 to 59 </param>
		/// <param name="second"> 0 to 59
		/// </param>
		/// @deprecated Use the constructor that takes a milliseconds value
		///             in place of this constructor 
		[Obsolete("Use the constructor that takes a milliseconds value")]
		public Time(int hour, int minute, int second) : base(70, 0, 1, hour, minute, second)
		{
		}

		/// <summary>
		/// Constructs a <code>Time</code> object using a milliseconds time value.
		/// </summary>
		/// <param name="time"> milliseconds since January 1, 1970, 00:00:00 GMT;
		///             a negative number is milliseconds before
		///               January 1, 1970, 00:00:00 GMT </param>
		public Time(long time) : base(time)
		{
		}

		/// <summary>
		/// Sets a <code>Time</code> object using a milliseconds time value.
		/// </summary>
		/// <param name="time"> milliseconds since January 1, 1970, 00:00:00 GMT;
		///             a negative number is milliseconds before
		///               January 1, 1970, 00:00:00 GMT </param>
		public override long Time
		{
			set
			{
				base = new DateTime(value);
			}
		}

		/// <summary>
		/// Converts a string in JDBC time escape format to a <code>Time</code> value.
		/// </summary>
		/// <param name="s"> time in format "hh:mm:ss" </param>
		/// <returns> a corresponding <code>Time</code> object </returns>
		public static Time ValueOf(String s)
		{
			int hour;
			int minute;
			int second;
			int firstColon;
			int secondColon;

			if (s == null)
			{
				throw new System.ArgumentException();
			}

			firstColon = s.IndexOf(':');
			secondColon = s.IndexOf(':', firstColon + 1);
			if ((firstColon > 0) & (secondColon > 0) & (secondColon < s.Length() - 1))
			{
				hour = Convert.ToInt32(s.Substring(0, firstColon));
				minute = Convert.ToInt32(StringHelperClass.SubstringSpecial(s, firstColon + 1, secondColon));
				second = Convert.ToInt32(s.Substring(secondColon + 1));
			}
			else
			{
				throw new System.ArgumentException();
			}

			return new Time(hour, minute, second);
		}

		/// <summary>
		/// Formats a time in JDBC time escape format.
		/// </summary>
		/// <returns> a <code>String</code> in hh:mm:ss format </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public String toString()
		public override String ToString()
		{
			int hour = base.Hour;
			int minute = base.Minute;
			int second = base.Second;
			String hourString;
			String minuteString;
			String secondString;

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
			return (hourString + ":" + minuteString + ":" + secondString);
		}

		// Override all the date operations inherited from java.util.Date;

	   /// <summary>
	   /// This method is deprecated and should not be used because SQL <code>TIME</code>
	   /// values do not have a year component.
	   /// 
	   /// @deprecated </summary>
	   /// <exception cref="java.lang.IllegalArgumentException"> if this
	   ///           method is invoked </exception>
	   /// <seealso cref= #setYear </seealso>
		[Obsolete]
		public override int Year
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
	   /// This method is deprecated and should not be used because SQL <code>TIME</code>
	   /// values do not have a month component.
	   /// 
	   /// @deprecated </summary>
	   /// <exception cref="java.lang.IllegalArgumentException"> if this
	   ///           method is invoked </exception>
	   /// <seealso cref= #setMonth </seealso>
		[Obsolete]
		public override int Month
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
	   /// This method is deprecated and should not be used because SQL <code>TIME</code>
	   /// values do not have a day component.
	   /// 
	   /// @deprecated </summary>
	   /// <exception cref="java.lang.IllegalArgumentException"> if this
	   ///           method is invoked </exception>
		[Obsolete]
		public override int Day
		{
			get
			{
				throw new System.ArgumentException();
			}
		}

	   /// <summary>
	   /// This method is deprecated and should not be used because SQL <code>TIME</code>
	   /// values do not have a date component.
	   /// 
	   /// @deprecated </summary>
	   /// <exception cref="java.lang.IllegalArgumentException"> if this
	   ///           method is invoked </exception>
	   /// <seealso cref= #setDate </seealso>
		[Obsolete]
		public override int Date
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
		internal const long SerialVersionUID = 8397324403548013681L;

		/// <summary>
		/// Obtains an instance of {@code Time} from a <seealso cref="LocalTime"/> object
		/// with the same hour, minute and second time value as the given
		/// {@code LocalTime}.
		/// </summary>
		/// <param name="time"> a {@code LocalTime} to convert </param>
		/// <returns> a {@code Time} object </returns>
		/// <exception cref="NullPointerException"> if {@code time} is null
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public static Time valueOf(java.time.LocalTime time)
		public static Time ValueOf(LocalTime time)
		{
			return new Time(time.Hour, time.Minute, time.Second);
		}

		/// <summary>
		/// Converts this {@code Time} object to a {@code LocalTime}.
		/// <para>
		/// The conversion creates a {@code LocalTime} that represents the same
		/// hour, minute, and second time value as this {@code Time}.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a {@code LocalTime} object representing the same time value
		/// @since 1.8 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public java.time.LocalTime toLocalTime()
		public virtual LocalTime ToLocalTime()
		{
			return LocalTime.Of(Hours, Minutes, Seconds);
		}

	   /// <summary>
	   /// This method always throws an UnsupportedOperationException and should
	   /// not be used because SQL {@code Time} values do not have a date
	   /// component.
	   /// </summary>
	   /// <exception cref="java.lang.UnsupportedOperationException"> if this method is invoked </exception>
		public override Instant ToInstant()
		{
			throw new System.NotSupportedException();
		}
	}

}