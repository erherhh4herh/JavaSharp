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
 * Copyright (c) 2011-2012, Stephen Colebourne & Michael Nascimento Santos
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
	/// The shared serialization delegate for this package.
	/// 
	/// @implNote
	/// This class wraps the object being serialized, and takes a byte representing the type of the class to
	/// be serialized.  This byte can also be used for versioning the serialization format.  In this case another
	/// byte flag would be used in order to specify an alternative version of the type format.
	/// For example {@code LOCAL_DATE_TYPE_VERSION_2 = 21}.
	/// <para>
	/// In order to serialize the object it writes its byte and then calls back to the appropriate class where
	/// the serialization is performed.  In order to deserialize the object it read in the type byte, switching
	/// in order to select which class to call back into.
	/// </para>
	/// <para>
	/// The serialization format is determined on a per class basis.  In the case of field based classes each
	/// of the fields is written out with an appropriate size format in descending order of the field's size.  For
	/// example in the case of <seealso cref="LocalDate"/> year is written before month.  Composite classes, such as
	/// <seealso cref="LocalDateTime"/> are serialized as one object.
	/// </para>
	/// <para>
	/// This class is mutable and should be created once per serialization.
	/// 
	/// @serial include
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	internal sealed class Ser : Externalizable
	{

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = -7683839454370182990L;

		internal const sbyte DURATION_TYPE = 1;
		internal const sbyte INSTANT_TYPE = 2;
		internal const sbyte LOCAL_DATE_TYPE = 3;
		internal const sbyte LOCAL_TIME_TYPE = 4;
		internal const sbyte LOCAL_DATE_TIME_TYPE = 5;
		internal const sbyte ZONE_DATE_TIME_TYPE = 6;
		internal const sbyte ZONE_REGION_TYPE = 7;
		internal const sbyte ZONE_OFFSET_TYPE = 8;
		internal const sbyte OFFSET_TIME_TYPE = 9;
		internal const sbyte OFFSET_DATE_TIME_TYPE = 10;
		internal const sbyte YEAR_TYPE = 11;
		internal const sbyte YEAR_MONTH_TYPE = 12;
		internal const sbyte MONTH_DAY_TYPE = 13;
		internal const sbyte PERIOD_TYPE = 14;

		/// <summary>
		/// The type being serialized. </summary>
		private sbyte Type;
		/// <summary>
		/// The object being serialized. </summary>
		private Object @object;

		/// <summary>
		/// Constructor for deserialization.
		/// </summary>
		public Ser()
		{
		}

		/// <summary>
		/// Creates an instance for serialization.
		/// </summary>
		/// <param name="type">  the type </param>
		/// <param name="object">  the object </param>
		internal Ser(sbyte type, Object @object)
		{
			this.Type = type;
			this.@object = @object;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implements the {@code Externalizable} interface to write the object.
		/// @serialData
		/// 
		/// Each serializable class is mapped to a type that is the first byte
		/// in the stream.  Refer to each class {@code writeReplace}
		/// serialized form for the value of the type and sequence of values for the type.
		/// <ul>
		/// <li><a href="../../serialized-form.html#java.time.Duration">Duration.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.Instant">Instant.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.LocalDate">LocalDate.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.LocalDateTime">LocalDateTime.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.LocalTime">LocalTime.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.MonthDay">MonthDay.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.OffsetTime">OffsetTime.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.OffsetDateTime">OffsetDateTime.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.Period">Period.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.Year">Year.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.YearMonth">YearMonth.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.ZoneId">ZoneId.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.ZoneOffset">ZoneOffset.writeReplace</a>
		/// <li><a href="../../serialized-form.html#java.time.ZonedDateTime">ZonedDateTime.writeReplace</a>
		/// </ul>
		/// </summary>
		/// <param name="out">  the data stream to write to, not null </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void writeExternal(java.io.ObjectOutput out) throws java.io.IOException
		public void WriteExternal(ObjectOutput @out)
		{
			WriteInternal(Type, @object, @out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void writeInternal(byte type, Object object, java.io.ObjectOutput out) throws java.io.IOException
		internal static void WriteInternal(sbyte type, Object @object, ObjectOutput @out)
		{
			@out.WriteByte(type);
			switch (type)
			{
				case DURATION_TYPE:
					((Duration) @object).WriteExternal(@out);
					break;
				case INSTANT_TYPE:
					((Instant) @object).WriteExternal(@out);
					break;
				case LOCAL_DATE_TYPE:
					((LocalDate) @object).WriteExternal(@out);
					break;
				case LOCAL_DATE_TIME_TYPE:
					((LocalDateTime) @object).WriteExternal(@out);
					break;
				case LOCAL_TIME_TYPE:
					((LocalTime) @object).WriteExternal(@out);
					break;
				case ZONE_REGION_TYPE:
					((ZoneRegion) @object).WriteExternal(@out);
					break;
				case ZONE_OFFSET_TYPE:
					((ZoneOffset) @object).WriteExternal(@out);
					break;
				case ZONE_DATE_TIME_TYPE:
					((ZonedDateTime) @object).WriteExternal(@out);
					break;
				case OFFSET_TIME_TYPE:
					((OffsetTime) @object).WriteExternal(@out);
					break;
				case OFFSET_DATE_TIME_TYPE:
					((OffsetDateTime) @object).WriteExternal(@out);
					break;
				case YEAR_TYPE:
					((Year) @object).WriteExternal(@out);
					break;
				case YEAR_MONTH_TYPE:
					((YearMonth) @object).WriteExternal(@out);
					break;
				case MONTH_DAY_TYPE:
					((MonthDay) @object).WriteExternal(@out);
					break;
				case PERIOD_TYPE:
					((Period) @object).WriteExternal(@out);
					break;
				default:
					throw new InvalidClassException("Unknown serialized type");
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implements the {@code Externalizable} interface to read the object.
		/// @serialData
		/// 
		/// The streamed type and parameters defined by the type's {@code writeReplace}
		/// method are read and passed to the corresponding static factory for the type
		/// to create a new instance.  That instance is returned as the de-serialized
		/// {@code Ser} object.
		/// 
		/// <ul>
		/// <li><a href="../../serialized-form.html#java.time.Duration">Duration</a> - {@code Duration.ofSeconds(seconds, nanos);}
		/// <li><a href="../../serialized-form.html#java.time.Instant">Instant</a> - {@code Instant.ofEpochSecond(seconds, nanos);}
		/// <li><a href="../../serialized-form.html#java.time.LocalDate">LocalDate</a> - {@code LocalDate.of(year, month, day);}
		/// <li><a href="../../serialized-form.html#java.time.LocalDateTime">LocalDateTime</a> - {@code LocalDateTime.of(date, time);}
		/// <li><a href="../../serialized-form.html#java.time.LocalTime">LocalTime</a> - {@code LocalTime.of(hour, minute, second, nano);}
		/// <li><a href="../../serialized-form.html#java.time.MonthDay">MonthDay</a> - {@code MonthDay.of(month, day);}
		/// <li><a href="../../serialized-form.html#java.time.OffsetTime">OffsetTime</a> - {@code OffsetTime.of(time, offset);}
		/// <li><a href="../../serialized-form.html#java.time.OffsetDateTime">OffsetDateTime</a> - {@code OffsetDateTime.of(dateTime, offset);}
		/// <li><a href="../../serialized-form.html#java.time.Period">Period</a> - {@code Period.of(years, months, days);}
		/// <li><a href="../../serialized-form.html#java.time.Year">Year</a> - {@code Year.of(year);}
		/// <li><a href="../../serialized-form.html#java.time.YearMonth">YearMonth</a> - {@code YearMonth.of(year, month);}
		/// <li><a href="../../serialized-form.html#java.time.ZonedDateTime">ZonedDateTime</a> - {@code ZonedDateTime.ofLenient(dateTime, offset, zone);}
		/// <li><a href="../../serialized-form.html#java.time.ZoneId">ZoneId</a> - {@code ZoneId.of(id);}
		/// <li><a href="../../serialized-form.html#java.time.ZoneOffset">ZoneOffset</a> - {@code (offsetByte == 127 ? ZoneOffset.ofTotalSeconds(in.readInt()) : ZoneOffset.ofTotalSeconds(offsetByte * 900));}
		/// </ul>
		/// </summary>
		/// <param name="in">  the data to read, not null </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
		public void ReadExternal(ObjectInput @in)
		{
			Type = @in.ReadByte();
			@object = ReadInternal(Type, @in);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object read(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
		internal static Object Read(ObjectInput @in)
		{
			sbyte type = @in.ReadByte();
			return ReadInternal(type, @in);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Object readInternal(byte type, java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
		private static Object ReadInternal(sbyte type, ObjectInput @in)
		{
			switch (type)
			{
				case DURATION_TYPE:
					return Duration.ReadExternal(@in);
				case INSTANT_TYPE:
					return Instant.ReadExternal(@in);
				case LOCAL_DATE_TYPE:
					return LocalDate.ReadExternal(@in);
				case LOCAL_DATE_TIME_TYPE:
					return LocalDateTime.ReadExternal(@in);
				case LOCAL_TIME_TYPE:
					return LocalTime.ReadExternal(@in);
				case ZONE_DATE_TIME_TYPE:
					return ZonedDateTime.ReadExternal(@in);
				case ZONE_OFFSET_TYPE:
					return ZoneOffset.ReadExternal(@in);
				case ZONE_REGION_TYPE:
					return ZoneRegion.ReadExternal(@in);
				case OFFSET_TIME_TYPE:
					return OffsetTime.ReadExternal(@in);
				case OFFSET_DATE_TIME_TYPE:
					return OffsetDateTime.ReadExternal(@in);
				case YEAR_TYPE:
					return Year.ReadExternal(@in);
				case YEAR_MONTH_TYPE:
					return YearMonth.ReadExternal(@in);
				case MONTH_DAY_TYPE:
					return MonthDay.ReadExternal(@in);
				case PERIOD_TYPE:
					return Period.ReadExternal(@in);
				default:
					throw new StreamCorruptedException("Unknown serialized type");
			}
		}

		/// <summary>
		/// Returns the object that will replace this one.
		/// </summary>
		/// <returns> the read object, should never be null </returns>
		private Object ReadResolve()
		{
			 return @object;
		}

	}

}