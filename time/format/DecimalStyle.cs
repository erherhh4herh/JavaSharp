using System.Collections.Generic;
using System.Collections.Concurrent;

/*
 * Copyright (c) 2012, 2015, Oracle and/or its affiliates. All rights reserved.
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
	/// Localized decimal style used in date and time formatting.
	/// <para>
	/// A significant part of dealing with dates and times is the localization.
	/// This class acts as a central point for accessing the information.
	/// 
	/// @implSpec
	/// This class is immutable and thread-safe.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class DecimalStyle
	{

		/// <summary>
		/// The standard set of non-localized decimal style symbols.
		/// <para>
		/// This uses standard ASCII characters for zero, positive, negative and a dot for the decimal point.
		/// </para>
		/// </summary>
		public static readonly DecimalStyle STANDARD = new DecimalStyle('0', '+', '-', '.');
		/// <summary>
		/// The cache of DecimalStyle instances.
		/// </summary>
		private static readonly ConcurrentMap<Locale, DecimalStyle> CACHE = new ConcurrentDictionary<Locale, DecimalStyle>(16, 0.75f, 2);

		/// <summary>
		/// The zero digit.
		/// </summary>
		private readonly char ZeroDigit_Renamed;
		/// <summary>
		/// The positive sign.
		/// </summary>
		private readonly char PositiveSign_Renamed;
		/// <summary>
		/// The negative sign.
		/// </summary>
		private readonly char NegativeSign_Renamed;
		/// <summary>
		/// The decimal separator.
		/// </summary>
		private readonly char DecimalSeparator_Renamed;

		//-----------------------------------------------------------------------
		/// <summary>
		/// Lists all the locales that are supported.
		/// <para>
		/// The locale 'en_US' will always be present.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a Set of Locales for which localization is supported </returns>
		public static Set<Locale> AvailableLocales
		{
			get
			{
				Locale[] l = DecimalFormatSymbols.AvailableLocales;
				Set<Locale> locales = new HashSet<Locale>(l.Length);
				Collections.AddAll(locales, l);
				return locales;
			}
		}

		/// <summary>
		/// Obtains the DecimalStyle for the default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		/// <para>
		/// This method provides access to locale sensitive decimal style symbols.
		/// </para>
		/// <para>
		/// This is equivalent to calling
		/// {@link #of(Locale)
		///     of(Locale.getDefault(Locale.Category.FORMAT))}.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		/// <returns> the decimal style, not null </returns>
		public static DecimalStyle OfDefaultLocale()
		{
			return Of(Locale.GetDefault(Locale.Category.FORMAT));
		}

		/// <summary>
		/// Obtains the DecimalStyle for the specified locale.
		/// <para>
		/// This method provides access to locale sensitive decimal style symbols.
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale">  the locale, not null </param>
		/// <returns> the decimal style, not null </returns>
		public static DecimalStyle Of(Locale locale)
		{
			Objects.RequireNonNull(locale, "locale");
			DecimalStyle info = CACHE[locale];
			if (info == null)
			{
				info = Create(locale);
				CACHE.PutIfAbsent(locale, info);
				info = CACHE[locale];
			}
			return info;
		}

		private static DecimalStyle Create(Locale locale)
		{
			DecimalFormatSymbols oldSymbols = DecimalFormatSymbols.GetInstance(locale);
			char zeroDigit = oldSymbols.ZeroDigit;
			char positiveSign = '+';
			char negativeSign = oldSymbols.MinusSign;
			char decimalSeparator = oldSymbols.DecimalSeparator;
			if (zeroDigit == '0' && negativeSign == '-' && decimalSeparator == '.')
			{
				return STANDARD;
			}
			return new DecimalStyle(zeroDigit, positiveSign, negativeSign, decimalSeparator);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Restricted constructor.
		/// </summary>
		/// <param name="zeroChar">  the character to use for the digit of zero </param>
		/// <param name="positiveSignChar">  the character to use for the positive sign </param>
		/// <param name="negativeSignChar">  the character to use for the negative sign </param>
		/// <param name="decimalPointChar">  the character to use for the decimal point </param>
		private DecimalStyle(char zeroChar, char positiveSignChar, char negativeSignChar, char decimalPointChar)
		{
			this.ZeroDigit_Renamed = zeroChar;
			this.PositiveSign_Renamed = positiveSignChar;
			this.NegativeSign_Renamed = negativeSignChar;
			this.DecimalSeparator_Renamed = decimalPointChar;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the character that represents zero.
		/// <para>
		/// The character used to represent digits may vary by culture.
		/// This method specifies the zero character to use, which implies the characters for one to nine.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the character for zero </returns>
		public char ZeroDigit
		{
			get
			{
				return ZeroDigit_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of the info with a new character that represents zero.
		/// <para>
		/// The character used to represent digits may vary by culture.
		/// This method specifies the zero character to use, which implies the characters for one to nine.
		/// 
		/// </para>
		/// </summary>
		/// <param name="zeroDigit">  the character for zero </param>
		/// <returns>  a copy with a new character that represents zero, not null
		///  </returns>
		public DecimalStyle WithZeroDigit(char zeroDigit)
		{
			if (zeroDigit == this.ZeroDigit_Renamed)
			{
				return this;
			}
			return new DecimalStyle(zeroDigit, PositiveSign_Renamed, NegativeSign_Renamed, DecimalSeparator_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the character that represents the positive sign.
		/// <para>
		/// The character used to represent a positive number may vary by culture.
		/// This method specifies the character to use.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the character for the positive sign </returns>
		public char PositiveSign
		{
			get
			{
				return PositiveSign_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of the info with a new character that represents the positive sign.
		/// <para>
		/// The character used to represent a positive number may vary by culture.
		/// This method specifies the character to use.
		/// 
		/// </para>
		/// </summary>
		/// <param name="positiveSign">  the character for the positive sign </param>
		/// <returns>  a copy with a new character that represents the positive sign, not null </returns>
		public DecimalStyle WithPositiveSign(char positiveSign)
		{
			if (positiveSign == this.PositiveSign_Renamed)
			{
				return this;
			}
			return new DecimalStyle(ZeroDigit_Renamed, positiveSign, NegativeSign_Renamed, DecimalSeparator_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the character that represents the negative sign.
		/// <para>
		/// The character used to represent a negative number may vary by culture.
		/// This method specifies the character to use.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the character for the negative sign </returns>
		public char NegativeSign
		{
			get
			{
				return NegativeSign_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of the info with a new character that represents the negative sign.
		/// <para>
		/// The character used to represent a negative number may vary by culture.
		/// This method specifies the character to use.
		/// 
		/// </para>
		/// </summary>
		/// <param name="negativeSign">  the character for the negative sign </param>
		/// <returns>  a copy with a new character that represents the negative sign, not null </returns>
		public DecimalStyle WithNegativeSign(char negativeSign)
		{
			if (negativeSign == this.NegativeSign_Renamed)
			{
				return this;
			}
			return new DecimalStyle(ZeroDigit_Renamed, PositiveSign_Renamed, negativeSign, DecimalSeparator_Renamed);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the character that represents the decimal point.
		/// <para>
		/// The character used to represent a decimal point may vary by culture.
		/// This method specifies the character to use.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the character for the decimal point </returns>
		public char DecimalSeparator
		{
			get
			{
				return DecimalSeparator_Renamed;
			}
		}

		/// <summary>
		/// Returns a copy of the info with a new character that represents the decimal point.
		/// <para>
		/// The character used to represent a decimal point may vary by culture.
		/// This method specifies the character to use.
		/// 
		/// </para>
		/// </summary>
		/// <param name="decimalSeparator">  the character for the decimal point </param>
		/// <returns>  a copy with a new character that represents the decimal point, not null </returns>
		public DecimalStyle WithDecimalSeparator(char decimalSeparator)
		{
			if (decimalSeparator == this.DecimalSeparator_Renamed)
			{
				return this;
			}
			return new DecimalStyle(ZeroDigit_Renamed, PositiveSign_Renamed, NegativeSign_Renamed, decimalSeparator);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks whether the character is a digit, based on the currently set zero character.
		/// </summary>
		/// <param name="ch">  the character to check </param>
		/// <returns> the value, 0 to 9, of the character, or -1 if not a digit </returns>
		internal int ConvertToDigit(char ch)
		{
			int val = ch - ZeroDigit_Renamed;
			return (val >= 0 && val <= 9) ? val : -1;
		}

		/// <summary>
		/// Converts the input numeric text to the internationalized form using the zero character.
		/// </summary>
		/// <param name="numericText">  the text, consisting of digits 0 to 9, to convert, not null </param>
		/// <returns> the internationalized text, not null </returns>
		internal String ConvertNumberToI18N(String numericText)
		{
			if (ZeroDigit_Renamed == '0')
			{
				return numericText;
			}
			int diff = ZeroDigit_Renamed - '0';
			char[] array = numericText.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (char)(array[i] + diff);
			}
			return new String(array);
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Checks if this DecimalStyle is equal to another DecimalStyle.
		/// </summary>
		/// <param name="obj">  the object to check, null returns false </param>
		/// <returns> true if this is equal to the other date </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is DecimalStyle)
			{
				DecimalStyle other = (DecimalStyle) obj;
				return (ZeroDigit_Renamed == other.ZeroDigit_Renamed && PositiveSign_Renamed == other.PositiveSign_Renamed && NegativeSign_Renamed == other.NegativeSign_Renamed && DecimalSeparator_Renamed == other.DecimalSeparator_Renamed);
			}
			return false;
		}

		/// <summary>
		/// A hash code for this DecimalStyle.
		/// </summary>
		/// <returns> a suitable hash code </returns>
		public override int HashCode()
		{
			return ZeroDigit_Renamed + PositiveSign_Renamed + NegativeSign_Renamed + DecimalSeparator_Renamed;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Returns a string describing this DecimalStyle.
		/// </summary>
		/// <returns> a string description, not null </returns>
		public override String ToString()
		{
			return "DecimalStyle[" + ZeroDigit_Renamed + PositiveSign_Renamed + NegativeSign_Renamed + DecimalSeparator_Renamed + "]";
		}

	}

}