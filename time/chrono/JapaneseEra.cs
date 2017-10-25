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
 * Copyright (c) 2012, Stephen Colebourne & Michael Nascimento Santos
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



	using CalendarDate = sun.util.calendar.CalendarDate;

	/// <summary>
	/// An era in the Japanese Imperial calendar system.
	/// <para>
	/// This class defines the valid eras for the Japanese chronology.
	/// Japan introduced the Gregorian calendar starting with Meiji 6.
	/// Only Meiji and later eras are supported;
	/// dates before Meiji 6, January 1 are not supported.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class JapaneseEra : Era
	{

		// The offset value to 0-based index from the era value.
		// i.e., getValue() + ERA_OFFSET == 0-based index
		internal const int ERA_OFFSET = 2;

		internal static readonly sun.util.calendar.Era[] ERA_CONFIG;

		/// <summary>
		/// The singleton instance for the 'Meiji' era (1868-01-01 - 1912-07-29)
		/// which has the value -1.
		/// </summary>
		public static readonly JapaneseEra MEIJI = new JapaneseEra(-1, LocalDate.Of(1868, 1, 1));
		/// <summary>
		/// The singleton instance for the 'Taisho' era (1912-07-30 - 1926-12-24)
		/// which has the value 0.
		/// </summary>
		public static readonly JapaneseEra TAISHO = new JapaneseEra(0, LocalDate.Of(1912, 7, 30));
		/// <summary>
		/// The singleton instance for the 'Showa' era (1926-12-25 - 1989-01-07)
		/// which has the value 1.
		/// </summary>
		public static readonly JapaneseEra SHOWA = new JapaneseEra(1, LocalDate.Of(1926, 12, 25));
		/// <summary>
		/// The singleton instance for the 'Heisei' era (1989-01-08 - current)
		/// which has the value 2.
		/// </summary>
		public static readonly JapaneseEra HEISEI = new JapaneseEra(2, LocalDate.Of(1989, 1, 8));

		// the number of defined JapaneseEra constants.
		// There could be an extra era defined in its configuration.
		private static readonly int N_ERA_CONSTANTS = HEISEI.Value + ERA_OFFSET;

		/// <summary>
		/// Serialization version.
		/// </summary>
		private const long SerialVersionUID = 1466499369062886794L;

		// array for the singleton JapaneseEra instances
		private static readonly JapaneseEra[] KNOWN_ERAS;

		static JapaneseEra()
		{
			ERA_CONFIG = JapaneseChronology.JCAL.Eras;

			KNOWN_ERAS = new JapaneseEra[ERA_CONFIG.Length];
			KNOWN_ERAS[0] = MEIJI;
			KNOWN_ERAS[1] = TAISHO;
			KNOWN_ERAS[2] = SHOWA;
			KNOWN_ERAS[3] = HEISEI;
			for (int i = N_ERA_CONSTANTS; i < ERA_CONFIG.Length; i++)
			{
				CalendarDate date = ERA_CONFIG[i].SinceDate;
				LocalDate isoDate = LocalDate.Of(date.Year, date.Month, date.DayOfMonth);
				KNOWN_ERAS[i] = new JapaneseEra(i - ERA_OFFSET + 1, isoDate);
			}
		};

		/// <summary>
		/// The era value.
		/// @serial
		/// </summary>
		[NonSerialized]
		private readonly int EraValue;

		// the first day of the era
		[NonSerialized]
		private readonly LocalDate Since;

		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <param name="eraValue">  the era value, validated </param>
		/// <param name="since">  the date representing the first date of the era, validated not null </param>
		private JapaneseEra(int eraValue, LocalDate since)
		{
			this.EraValue = eraValue;
			this.Since = since;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns the Sun private Era instance corresponding to this {@code JapaneseEra}.
		/// </summary>
		/// <returns> the Sun private Era instance for this {@code JapaneseEra}. </returns>
		internal sun.util.calendar.Era PrivateEra
		{
			get
			{
				return ERA_CONFIG[Ordinal(EraValue)];
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code JapaneseEra} from an {@code int} value.
		/// <para>
		/// The <seealso cref="#SHOWA"/> era that contains 1970-01-01 (ISO calendar system) has the value 1
		/// Later era is numbered 2 (<seealso cref="#HEISEI"/>). Earlier eras are numbered 0 (<seealso cref="#TAISHO"/>),
		/// -1 (<seealso cref="#MEIJI"/>), only Meiji and later eras are supported.
		/// 
		/// </para>
		/// </summary>
		/// <param name="japaneseEra">  the era to represent </param>
		/// <returns> the {@code JapaneseEra} singleton, not null </returns>
		/// <exception cref="DateTimeException"> if the value is invalid </exception>
		public static JapaneseEra Of(int japaneseEra)
		{
			if (japaneseEra < MEIJI.EraValue || japaneseEra + ERA_OFFSET > KNOWN_ERAS.Length)
			{
				throw new DateTimeException("Invalid era: " + japaneseEra);
			}
			return KNOWN_ERAS[Ordinal(japaneseEra)];
		}

		/// <summary>
		/// Returns the {@code JapaneseEra} with the name.
		/// <para>
		/// The string must match exactly the name of the era.
		/// (Extraneous whitespace characters are not permitted.)
		/// 
		/// </para>
		/// </summary>
		/// <param name="japaneseEra">  the japaneseEra name; non-null </param>
		/// <returns> the {@code JapaneseEra} singleton, never null </returns>
		/// <exception cref="IllegalArgumentException"> if there is not JapaneseEra with the specified name </exception>
		public static JapaneseEra ValueOf(String japaneseEra)
		{
			Objects.RequireNonNull(japaneseEra, "japaneseEra");
			foreach (JapaneseEra era in KNOWN_ERAS)
			{
				if (era.Name.Equals(japaneseEra))
				{
					return era;
				}
			}
			throw new IllegalArgumentException("japaneseEra is invalid");
		}

		/// <summary>
		/// Returns an array of JapaneseEras.
		/// <para>
		/// This method may be used to iterate over the JapaneseEras as follows:
		/// <pre>
		/// for (JapaneseEra c : JapaneseEra.values())
		///     System.out.println(c);
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of JapaneseEras </returns>
		public static JapaneseEra[] Values()
		{
			return Arrays.CopyOf(KNOWN_ERAS, KNOWN_ERAS.Length);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code JapaneseEra} from a date.
		/// </summary>
		/// <param name="date">  the date, not null </param>
		/// <returns> the Era singleton, never null </returns>
		internal static JapaneseEra From(LocalDate date)
		{
			if (date.IsBefore(MEIJI_6_ISODATE))
			{
				throw new DateTimeException("JapaneseDate before Meiji 6 are not supported");
			}
			for (int i = KNOWN_ERAS.Length - 1; i > 0; i--)
			{
				JapaneseEra era = KNOWN_ERAS[i];
				if (date.CompareTo(era.Since) >= 0)
				{
					return era;
				}
			}
			return java.time.temporal.TemporalAccessor_Fields.Null;
		}

		internal static JapaneseEra ToJapaneseEra(sun.util.calendar.Era privateEra)
		{
			for (int i = ERA_CONFIG.Length - 1; i >= 0; i--)
			{
				if (ERA_CONFIG[i].Equals(privateEra))
				{
					return KNOWN_ERAS[i];
				}
			}
			return java.time.temporal.TemporalAccessor_Fields.Null;
		}

		internal static sun.util.calendar.Era PrivateEraFrom(LocalDate isoDate)
		{
			for (int i = KNOWN_ERAS.Length - 1; i > 0; i--)
			{
				JapaneseEra era = KNOWN_ERAS[i];
				if (isoDate.CompareTo(era.Since) >= 0)
				{
					return ERA_CONFIG[i];
				}
			}
			return java.time.temporal.TemporalAccessor_Fields.Null;
		}

		/// <summary>
		/// Returns the index into the arrays from the Era value.
		/// the eraValue is a valid Era number, -1..2.
		/// </summary>
		/// <param name="eraValue">  the era value to convert to the index </param>
		/// <returns> the index of the current Era </returns>
		private static int Ordinal(int eraValue)
		{
			return eraValue + ERA_OFFSET - 1;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the numeric era {@code int} value.
		/// <para>
		/// The <seealso cref="#SHOWA"/> era that contains 1970-01-01 (ISO calendar system) has the value 1.
		/// Later eras are numbered from 2 (<seealso cref="#HEISEI"/>).
		/// Earlier eras are numbered 0 (<seealso cref="#TAISHO"/>), -1 (<seealso cref="#MEIJI"/>)).
		/// 
		/// </para>
		/// </summary>
		/// <returns> the era value </returns>
		public int Value
		{
			get
			{
				return EraValue;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This era is used to enhance the accuracy of the returned range.
		/// If it is not possible to return the range, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The {@code ERA} field returns the range.
		/// All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.rangeRefinedBy(TemporalAccessor)}
		/// passing {@code this} as the argument.
		/// Whether the range can be obtained is determined by the field.
		/// </para>
		/// <para>
		/// The range of valid Japanese eras can change over time due to the nature
		/// of the Japanese calendar system.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to query the range for, not null </param>
		/// <returns> the range of valid values for the field, not null </returns>
		/// <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public override ValueRange java.time.temporal.TemporalAccessor_Fields.range(TemporalField Era_Fields) // override as super would return range from 0 to 1
		{
			if (Era_Fields.Field == ERA)
			{
				return JapaneseChronology.INSTANCE.Range(ERA);
			}
			return Era.this.range(Era_Fields.Field);
		}

		//-----------------------------------------------------------------------
		internal String Abbreviation
		{
			get
			{
				int index = Ordinal(Value);
				if (index == 0)
				{
					return "";
				}
				return ERA_CONFIG[index].Abbreviation;
			}
		}

		internal String Name
		{
			get
			{
				return ERA_CONFIG[Ordinal(Value)].Name;
			}
		}

		public override String ToString()
		{
			return Name;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Defend against malicious streams.
		/// </summary>
		/// <param name="s"> the stream to read </param>
		/// <exception cref="InvalidObjectException"> always </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.InvalidObjectException
		private void ReadObject(ObjectInputStream s)
		{
			throw new InvalidObjectException("Deserialization via serialization delegate");
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Writes the object using a
		/// <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		/// @serialData
		/// <pre>
		///  out.writeByte(5);        // identifies a JapaneseEra
		///  out.writeInt(getValue());
		/// </pre>
		/// </summary>
		/// <returns> the instance of {@code Ser}, not null </returns>
		private Object WriteReplace()
		{
			return new Ser(Ser.JAPANESE_ERA_TYPE, this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeExternal(java.io.DataOutput out) throws java.io.IOException
		internal void WriteExternal(DataOutput @out)
		{
			@out.WriteByte(this.Value);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static JapaneseEra readExternal(java.io.DataInput in) throws java.io.IOException
		internal static JapaneseEra ReadExternal(DataInput @in)
		{
			sbyte eraValue = @in.ReadByte();
			return JapaneseEra.Of(eraValue);
		}

	}

}