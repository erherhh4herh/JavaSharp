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
namespace java.time.chrono
{


	/// <summary>
	/// The shared serialization delegate for this package.
	/// 
	/// @implNote
	/// This class wraps the object being serialized, and takes a byte representing the type of the class to
	/// be serialized.  This byte can also be used for versioning the serialization format.  In this case another
	/// byte flag would be used in order to specify an alternative version of the type format.
	/// For example {@code CHRONO_TYPE_VERSION_2 = 21}
	/// <para>
	/// In order to serialize the object it writes its byte and then calls back to the appropriate class where
	/// the serialization is performed.  In order to deserialize the object it read in the type byte, switching
	/// in order to select which class to call back into.
	/// </para>
	/// <para>
	/// The serialization format is determined on a per class basis.  In the case of field based classes each
	/// of the fields is written out with an appropriate size format in descending order of the field's size.  For
	/// example in the case of <seealso cref="LocalDate"/> year is written before month.  Composite classes, such as
	/// <seealso cref="LocalDateTime"/> are serialized as one object.  Enum classes are serialized using the index of their
	/// element.
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
		private const long SerialVersionUID = -6103370247208168577L;

		internal const sbyte CHRONO_TYPE = 1;
		internal const sbyte CHRONO_LOCAL_DATE_TIME_TYPE = 2;
		internal const sbyte CHRONO_ZONE_DATE_TIME_TYPE = 3;
		internal const sbyte JAPANESE_DATE_TYPE = 4;
		internal const sbyte JAPANESE_ERA_TYPE = 5;
		internal const sbyte HIJRAH_DATE_TYPE = 6;
		internal const sbyte MINGUO_DATE_TYPE = 7;
		internal const sbyte THAIBUDDHIST_DATE_TYPE = 8;
		internal const sbyte CHRONO_PERIOD_TYPE = 9;

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
		/// Each serializable class is mapped to a type that is the first byte
		/// in the stream.  Refer to each class {@code writeReplace}
		/// serialized form for the value of the type and sequence of values for the type.
		/// <ul>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.HijrahChronology">HijrahChronology.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.IsoChronology">IsoChronology.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseChronology">JapaneseChronology.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.MinguoChronology">MinguoChronology.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.ThaiBuddhistChronology">ThaiBuddhistChronology.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.ChronoLocalDateTimeImpl">ChronoLocalDateTime.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.ChronoZonedDateTimeImpl">ChronoZonedDateTime.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseDate">JapaneseDate.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseEra">JapaneseEra.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.HijrahDate">HijrahDate.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.MinguoDate">MinguoDate.writeReplace</a>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.ThaiBuddhistDate">ThaiBuddhistDate.writeReplace</a>
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
//ORIGINAL LINE: private static void writeInternal(byte type, Object object, java.io.ObjectOutput out) throws java.io.IOException
		private static void WriteInternal(sbyte type, Object @object, ObjectOutput @out)
		{
			@out.WriteByte(type);
			switch (type)
			{
				case CHRONO_TYPE:
					((AbstractChronology) @object).WriteExternal(@out);
					break;
				case CHRONO_LOCAL_DATE_TIME_TYPE:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ((ChronoLocalDateTimeImpl<?>) object).writeExternal(out);
					((ChronoLocalDateTimeImpl<?>) @object).WriteExternal(@out);
					break;
				case CHRONO_ZONE_DATE_TIME_TYPE:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: ((ChronoZonedDateTimeImpl<?>) object).writeExternal(out);
					((ChronoZonedDateTimeImpl<?>) @object).WriteExternal(@out);
					break;
				case JAPANESE_DATE_TYPE:
					((JapaneseDate) @object).WriteExternal(@out);
					break;
				case JAPANESE_ERA_TYPE:
					((JapaneseEra) @object).WriteExternal(@out);
					break;
				case HIJRAH_DATE_TYPE:
					((HijrahDate) @object).WriteExternal(@out);
					break;
				case MINGUO_DATE_TYPE:
					((MinguoDate) @object).WriteExternal(@out);
					break;
				case THAIBUDDHIST_DATE_TYPE:
					((ThaiBuddhistDate) @object).WriteExternal(@out);
					break;
				case CHRONO_PERIOD_TYPE:
					((ChronoPeriodImpl) @object).WriteExternal(@out);
					break;
				default:
					throw new InvalidClassException("Unknown serialized type");
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Implements the {@code Externalizable} interface to read the object.
		/// @serialData
		/// The streamed type and parameters defined by the type's {@code writeReplace}
		/// method are read and passed to the corresponding static factory for the type
		/// to create a new instance.  That instance is returned as the de-serialized
		/// {@code Ser} object.
		/// 
		/// <ul>
		/// <li><a href="../../../serialized-form.html#java.time.chrono.HijrahChronology">HijrahChronology</a> - Chronology.of(id)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.IsoChronology">IsoChronology</a> - Chronology.of(id)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseChronology">JapaneseChronology</a> - Chronology.of(id)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.MinguoChronology">MinguoChronology</a> - Chronology.of(id)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.ThaiBuddhistChronology">ThaiBuddhistChronology</a> - Chronology.of(id)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.ChronoLocalDateTimeImpl">ChronoLocalDateTime</a> - date.atTime(time)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.ChronoZonedDateTimeImpl">ChronoZonedDateTime</a> - dateTime.atZone(offset).withZoneSameLocal(zone)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseDate">JapaneseDate</a> - JapaneseChronology.INSTANCE.date(year, month, dayOfMonth)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseEra">JapaneseEra</a> - JapaneseEra.of(eraValue)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.HijrahDate">HijrahDate</a> - HijrahChronology chrono.date(year, month, dayOfMonth)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.MinguoDate">MinguoDate</a> - MinguoChronology.INSTANCE.date(year, month, dayOfMonth)
		/// <li><a href="../../../serialized-form.html#java.time.chrono.ThaiBuddhistDate">ThaiBuddhistDate</a> - ThaiBuddhistChronology.INSTANCE.date(year, month, dayOfMonth)
		/// </ul>
		/// </summary>
		/// <param name="in">  the data stream to read from, not null </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
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
				case CHRONO_TYPE:
					return AbstractChronology.ReadExternal(@in);
				case CHRONO_LOCAL_DATE_TIME_TYPE:
					return ChronoLocalDateTimeImpl.ReadExternal(@in);
				case CHRONO_ZONE_DATE_TIME_TYPE:
					return ChronoZonedDateTimeImpl.ReadExternal(@in);
				case JAPANESE_DATE_TYPE:
					return JapaneseDate.ReadExternal(@in);
				case JAPANESE_ERA_TYPE:
					return JapaneseEra.ReadExternal(@in);
				case HIJRAH_DATE_TYPE:
					return HijrahDate.ReadExternal(@in);
				case MINGUO_DATE_TYPE:
					return MinguoDate.ReadExternal(@in);
				case THAIBUDDHIST_DATE_TYPE:
					return ThaiBuddhistDate.ReadExternal(@in);
				case CHRONO_PERIOD_TYPE:
					return ChronoPeriodImpl.ReadExternal(@in);
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