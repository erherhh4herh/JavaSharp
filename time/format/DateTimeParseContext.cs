using System.Collections.Generic;

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
 * Copyright (c) 2008-2012, Stephen Colebourne & Michael Nascimento Santos
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
namespace java.time.format
{


	/// <summary>
	/// Context object used during date and time parsing.
	/// <para>
	/// This class represents the current state of the parse.
	/// It has the ability to store and retrieve the parsed values and manage optional segments.
	/// It also provides key information to the parsing methods.
	/// </para>
	/// <para>
	/// Once parsing is complete, the <seealso cref="#toUnresolved()"/> is used to obtain the unresolved
	/// result data. The <seealso cref="#toResolved()"/> is used to obtain the resolved result.
	/// 
	/// @implSpec
	/// This class is a mutable context intended for use from a single thread.
	/// Usage of the class is thread-safe within standard parsing as a new instance of this class
	/// is automatically created for each parse and parsing is single-threaded
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	internal sealed class DateTimeParseContext
	{

		/// <summary>
		/// The formatter, not null.
		/// </summary>
		private DateTimeFormatter Formatter;
		/// <summary>
		/// Whether to parse using case sensitively.
		/// </summary>
		private bool CaseSensitive_Renamed = true;
		/// <summary>
		/// Whether to parse using strict rules.
		/// </summary>
		private bool Strict_Renamed = true;
		/// <summary>
		/// The list of parsed data.
		/// </summary>
		private readonly List<Parsed> Parsed_Renamed = new List<Parsed>();
		/// <summary>
		/// List of Consumers<Chronology> to be notified if the Chronology changes.
		/// </summary>
		private List<Consumer<Chronology>> ChronoListeners = null;

		/// <summary>
		/// Creates a new instance of the context.
		/// </summary>
		/// <param name="formatter">  the formatter controlling the parse, not null </param>
		internal DateTimeParseContext(DateTimeFormatter formatter) : base()
		{
			this.Formatter = formatter;
			Parsed_Renamed.Add(new Parsed());
		}

		/// <summary>
		/// Creates a copy of this context.
		/// This retains the case sensitive and strict flags.
		/// </summary>
		internal DateTimeParseContext Copy()
		{
			DateTimeParseContext newContext = new DateTimeParseContext(Formatter);
			newContext.CaseSensitive_Renamed = CaseSensitive_Renamed;
			newContext.Strict_Renamed = Strict_Renamed;
			return newContext;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the locale.
		/// <para>
		/// This locale is used to control localization in the parse except
		/// where localization is controlled by the DecimalStyle.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the locale, not null </returns>
		internal Locale Locale
		{
			get
			{
				return Formatter.Locale;
			}
		}

		/// <summary>
		/// Gets the DecimalStyle.
		/// <para>
		/// The DecimalStyle controls the numeric parsing.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the DecimalStyle, not null </returns>
		internal DecimalStyle DecimalStyle
		{
			get
			{
				return Formatter.DecimalStyle;
			}
		}

		/// <summary>
		/// Gets the effective chronology during parsing.
		/// </summary>
		/// <returns> the effective parsing chronology, not null </returns>
		internal Chronology EffectiveChronology
		{
			get
			{
				Chronology chrono = CurrentParsed().Chrono;
				if (chrono == null)
				{
					chrono = Formatter.Chronology;
					if (chrono == null)
					{
						chrono = IsoChronology.INSTANCE;
					}
				}
				return chrono;
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if parsing is case sensitive.
		/// </summary>
		/// <returns> true if parsing is case sensitive, false if case insensitive </returns>
		internal bool CaseSensitive
		{
			get
			{
				return CaseSensitive_Renamed;
			}
			set
			{
				this.CaseSensitive_Renamed = value;
			}
		}


		//-----------------------------------------------------------------------
		/// <summary>
		/// Helper to compare two {@code CharSequence} instances.
		/// This uses <seealso cref="#isCaseSensitive()"/>.
		/// </summary>
		/// <param name="cs1">  the first character sequence, not null </param>
		/// <param name="offset1">  the offset into the first sequence, valid </param>
		/// <param name="cs2">  the second character sequence, not null </param>
		/// <param name="offset2">  the offset into the second sequence, valid </param>
		/// <param name="length">  the length to check, valid </param>
		/// <returns> true if equal </returns>
		internal bool SubSequenceEquals(CharSequence cs1, int offset1, CharSequence cs2, int offset2, int length)
		{
			if (offset1 + length > cs1.Length() || offset2 + length > cs2.Length())
			{
				return false;
			}
			if (CaseSensitive)
			{
				for (int i = 0; i < length; i++)
				{
					char ch1 = cs1.CharAt(offset1 + i);
					char ch2 = cs2.CharAt(offset2 + i);
					if (ch1 != ch2)
					{
						return false;
					}
				}
			}
			else
			{
				for (int i = 0; i < length; i++)
				{
					char ch1 = cs1.CharAt(offset1 + i);
					char ch2 = cs2.CharAt(offset2 + i);
					if (ch1 != ch2 && char.ToUpper(ch1) != char.ToUpper(ch2) && char.ToLower(ch1) != char.ToLower(ch2))
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Helper to compare two {@code char}.
		/// This uses <seealso cref="#isCaseSensitive()"/>.
		/// </summary>
		/// <param name="ch1">  the first character </param>
		/// <param name="ch2">  the second character </param>
		/// <returns> true if equal </returns>
		internal bool CharEquals(char ch1, char ch2)
		{
			if (CaseSensitive)
			{
				return ch1 == ch2;
			}
			return CharEqualsIgnoreCase(ch1, ch2);
		}

		/// <summary>
		/// Compares two characters ignoring case.
		/// </summary>
		/// <param name="c1">  the first </param>
		/// <param name="c2">  the second </param>
		/// <returns> true if equal </returns>
		internal static bool CharEqualsIgnoreCase(char c1, char c2)
		{
			return c1 == c2 || char.ToUpper(c1) == char.ToUpper(c2) || char.ToLower(c1) == char.ToLower(c2);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if parsing is strict.
		/// <para>
		/// Strict parsing requires exact matching of the text and sign styles.
		/// 
		/// </para>
		/// </summary>
		/// <returns> true if parsing is strict, false if lenient </returns>
		internal bool Strict
		{
			get
			{
				return Strict_Renamed;
			}
			set
			{
				this.Strict_Renamed = value;
			}
		}


		//-----------------------------------------------------------------------
		/// <summary>
		/// Starts the parsing of an optional segment of the input.
		/// </summary>
		internal void StartOptional()
		{
			Parsed_Renamed.Add(CurrentParsed().Copy());
		}

		/// <summary>
		/// Ends the parsing of an optional segment of the input.
		/// </summary>
		/// <param name="successful">  whether the optional segment was successfully parsed </param>
		internal void EndOptional(bool successful)
		{
			if (successful)
			{
				Parsed_Renamed.Remove(Parsed_Renamed.Count - 2);
			}
			else
			{
				Parsed_Renamed.Remove(Parsed_Renamed.Count - 1);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the currently active temporal objects.
		/// </summary>
		/// <returns> the current temporal objects, not null </returns>
		private Parsed CurrentParsed()
		{
			return Parsed_Renamed[Parsed_Renamed.Count - 1];
		}

		/// <summary>
		/// Gets the unresolved result of the parse.
		/// </summary>
		/// <returns> the result of the parse, not null </returns>
		internal Parsed ToUnresolved()
		{
			return CurrentParsed();
		}

		/// <summary>
		/// Gets the resolved result of the parse.
		/// </summary>
		/// <returns> the result of the parse, not null </returns>
		internal TemporalAccessor ToResolved(ResolverStyle resolverStyle, Set<TemporalField> resolverFields)
		{
			Parsed parsed = CurrentParsed();
			parsed.Chrono = EffectiveChronology;
			parsed.Zone = (parsed.Zone != null ? parsed.Zone : Formatter.Zone);
			return parsed.Resolve(resolverStyle, resolverFields);
		}


		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the first value that was parsed for the specified field.
		/// <para>
		/// This searches the results of the parse, returning the first value found
		/// for the specified field. No attempt is made to derive a value.
		/// The field may have an out of range value.
		/// For example, the day-of-month might be set to 50, or the hour to 1000.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to query from the map, null returns null </param>
		/// <returns> the value mapped to the specified field, null if field was not parsed </returns>
		internal Long GetParsed(TemporalField field)
		{
			return CurrentParsed().FieldValues[field];
		}

		/// <summary>
		/// Stores the parsed field.
		/// <para>
		/// This stores a field-value pair that has been parsed.
		/// The value stored may be out of range for the field - no checks are performed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to set in the field-value map, not null </param>
		/// <param name="value">  the value to set in the field-value map </param>
		/// <param name="errorPos">  the position of the field being parsed </param>
		/// <param name="successPos">  the position after the field being parsed </param>
		/// <returns> the new position </returns>
		internal int SetParsedField(TemporalField field, long value, int errorPos, int successPos)
		{
			Objects.RequireNonNull(field, "field");
			Long old = CurrentParsed().FieldValues[field] = value;
			return (old != null && old.LongValue() != value) ?~errorPos : successPos;
		}

		/// <summary>
		/// Stores the parsed chronology.
		/// <para>
		/// This stores the chronology that has been parsed.
		/// No validation is performed other than ensuring it is not null.
		/// </para>
		/// <para>
		/// The list of listeners is copied and cleared so that each
		/// listener is called only once.  A listener can add itself again
		/// if it needs to be notified of future changes.
		/// 
		/// </para>
		/// </summary>
		/// <param name="chrono">  the parsed chronology, not null </param>
		internal Chronology Parsed
		{
			set
			{
				Objects.RequireNonNull(value, "chrono");
				CurrentParsed().Chrono = value;
				if (ChronoListeners != null && ChronoListeners.Count > 0)
				{
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings({"rawtypes", "unchecked"}) java.util.function.Consumer<java.time.chrono.Chronology>[] tmp = new java.util.function.Consumer[1];
					Consumer<Chronology>[] tmp = new Consumer[1];
					Consumer<Chronology>[] listeners = ChronoListeners.toArray(tmp);
					ChronoListeners.Clear();
					foreach (Consumer<Chronology> l in listeners)
					{
						l.Accept(value);
					}
				}
			}
		}

		/// <summary>
		/// Adds a Consumer<Chronology> to the list of listeners to be notified
		/// if the Chronology changes. </summary>
		/// <param name="listener"> a Consumer<Chronology> to be called when Chronology changes </param>
		internal void AddChronoChangedListener(Consumer<Chronology> listener)
		{
			if (ChronoListeners == null)
			{
				ChronoListeners = new List<Consumer<Chronology>>();
			}
			ChronoListeners.Add(listener);
		}

		/// <summary>
		/// Stores the parsed zone.
		/// <para>
		/// This stores the zone that has been parsed.
		/// No validation is performed other than ensuring it is not null.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zone">  the parsed zone, not null </param>
		internal ZoneId Parsed
		{
			set
			{
				Objects.RequireNonNull(value, "zone");
				CurrentParsed().Zone = value;
			}
		}

		/// <summary>
		/// Stores the parsed leap second.
		/// </summary>
		internal void SetParsedLeapSecond()
		{
			CurrentParsed().LeapSecond = true;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a string version of the context for debugging.
		/// </summary>
		/// <returns> a string representation of the context data, not null </returns>
		public override String ToString()
		{
			return CurrentParsed().ToString();
		}

	}

}