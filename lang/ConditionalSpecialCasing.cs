using System.Collections.Generic;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{

	using Normalizer = sun.text.Normalizer;


	/// <summary>
	/// This is a utility class for <code>String.toLowerCase()</code> and
	/// <code>String.toUpperCase()</code>, that handles special casing with
	/// conditions.  In other words, it handles the mappings with conditions
	/// that are defined in
	/// <a href="http://www.unicode.org/Public/UNIDATA/SpecialCasing.txt">Special
	/// Casing Properties</a> file.
	/// <para>
	/// Note that the unconditional case mappings (including 1:M mappings)
	/// are handled in <code>Character.toLower/UpperCase()</code>.
	/// </para>
	/// </summary>
	internal sealed class ConditionalSpecialCasing
	{

		// context conditions.
		internal const int FINAL_CASED = 1;
		internal const int AFTER_SOFT_DOTTED = 2;
		internal const int MORE_ABOVE = 3;
		internal const int AFTER_I = 4;
		internal const int NOT_BEFORE_DOT = 5;

		// combining class definitions
		internal const int COMBINING_CLASS_ABOVE = 230;

		// Special case mapping entries
		internal static Entry[] Entry = new Entry[] {new Entry(0x03A3, new char[]{0x03C2}, new char[]{0x03A3}, null, FINAL_CASED), new Entry(0x0130, new char[]{0x0069, 0x0307}, new char[]{0x0130}, null, 0), new Entry(0x0307, new char[]{0x0307}, new char[]{}, "lt", AFTER_SOFT_DOTTED), new Entry(0x0049, new char[]{0x0069, 0x0307}, new char[]{0x0049}, "lt", MORE_ABOVE), new Entry(0x004A, new char[]{0x006A, 0x0307}, new char[]{0x004A}, "lt", MORE_ABOVE), new Entry(0x012E, new char[]{0x012F, 0x0307}, new char[]{0x012E}, "lt", MORE_ABOVE), new Entry(0x00CC, new char[]{0x0069, 0x0307, 0x0300}, new char[]{0x00CC}, "lt", 0), new Entry(0x00CD, new char[]{0x0069, 0x0307, 0x0301}, new char[]{0x00CD}, "lt", 0), new Entry(0x0128, new char[]{0x0069, 0x0307, 0x0303}, new char[]{0x0128}, "lt", 0), new Entry(0x0130, new char[]{0x0069}, new char[]{0x0130}, "tr", 0), new Entry(0x0130, new char[]{0x0069}, new char[]{0x0130}, "az", 0), new Entry(0x0307, new char[]{}, new char[]{0x0307}, "tr", AFTER_I), new Entry(0x0307, new char[]{}, new char[]{0x0307}, "az", AFTER_I), new Entry(0x0049, new char[]{0x0131}, new char[]{0x0049}, "tr", NOT_BEFORE_DOT), new Entry(0x0049, new char[]{0x0131}, new char[]{0x0049}, "az", NOT_BEFORE_DOT), new Entry(0x0069, new char[]{0x0069}, new char[]{0x0130}, "tr", 0), new Entry(0x0069, new char[]{0x0069}, new char[]{0x0130}, "az", 0)};

		// A hash table that contains the above entries
		internal static Dictionary<Integer, HashSet<Entry>> EntryTable = new Dictionary<Integer, HashSet<Entry>>();
		static ConditionalSpecialCasing()
		{
			// create hashtable from the entry
			for (int i = 0; i < Entry.Length; i++)
			{
				Entry cur = Entry[i];
				Integer cp = new Integer(cur.CodePoint);
				HashSet<Entry> set = EntryTable[cp];
				if (set == null)
				{
					set = new HashSet<Entry>();
				}
				set.Add(cur);
				EntryTable[cp] = set;
			}
		}

		internal static int ToLowerCaseEx(String src, int index, Locale locale)
		{
			char[] result = LookUpTable(src, index, locale, true);

			if (result != null)
			{
				if (result.Length == 1)
				{
					return result[0];
				}
				else
				{
					return Character.ERROR;
				}
			}
			else
			{
				// default to Character class' one
				return char.ToLower(src.CodePointAt(index));
			}
		}

		internal static int ToUpperCaseEx(String src, int index, Locale locale)
		{
			char[] result = LookUpTable(src, index, locale, false);

			if (result != null)
			{
				if (result.Length == 1)
				{
					return result[0];
				}
				else
				{
					return Character.ERROR;
				}
			}
			else
			{
				// default to Character class' one
				return Character.ToUpperCaseEx(src.CodePointAt(index));
			}
		}

		internal static char[] ToLowerCaseCharArray(String src, int index, Locale locale)
		{
			return LookUpTable(src, index, locale, true);
		}

		internal static char[] ToUpperCaseCharArray(String src, int index, Locale locale)
		{
			char[] result = LookUpTable(src, index, locale, false);
			if (result != null)
			{
				return result;
			}
			else
			{
				return Character.ToUpperCaseCharArray(src.CodePointAt(index));
			}
		}

		private static char[] LookUpTable(String src, int index, Locale locale, bool bLowerCasing)
		{
			HashSet<Entry> set = EntryTable[new Integer(src.CodePointAt(index))];
			char[] ret = null;

			if (set != null)
			{
				IEnumerator<Entry> iter = set.GetEnumerator();
				String currentLang = locale.Language;
				while (iter.MoveNext())
				{
					Entry entry = iter.Current;
					String conditionLang = entry.Language;
					if (((conditionLang == null) || (conditionLang.Equals(currentLang))) && IsConditionMet(src, index, locale, entry.Condition))
					{
						ret = bLowerCasing ? entry.LowerCase : entry.UpperCase;
						if (conditionLang != null)
						{
							break;
						}
					}
				}
			}

			return ret;
		}

		private static bool IsConditionMet(String src, int index, Locale locale, int condition)
		{
			switch (condition)
			{
			case FINAL_CASED:
				return IsFinalCased(src, index, locale);

			case AFTER_SOFT_DOTTED:
				return IsAfterSoftDotted(src, index);

			case MORE_ABOVE:
				return IsMoreAbove(src, index);

			case AFTER_I:
				return IsAfterI(src, index);

			case NOT_BEFORE_DOT:
				return !IsBeforeDot(src, index);

			default:
				return true;
			}
		}

		/// <summary>
		/// Implements the "Final_Cased" condition
		/// 
		/// Specification: Within the closest word boundaries containing C, there is a cased
		/// letter before C, and there is no cased letter after C.
		/// 
		/// Regular Expression:
		///   Before C: [{cased==true}][{wordBoundary!=true}]*
		///   After C: !([{wordBoundary!=true}]*[{cased}])
		/// </summary>
		private static bool IsFinalCased(String src, int index, Locale locale)
		{
			BreakIterator wordBoundary = BreakIterator.GetWordInstance(locale);
			wordBoundary.SetText(src);
			int ch;

			// Look for a preceding 'cased' letter
			for (int i = index; (i >= 0) && !wordBoundary.IsBoundary(i); i -= Character.CharCount(ch))
			{

				ch = src.CodePointBefore(i);
				if (IsCased(ch))
				{

					int len = src.Length();
					// Check that there is no 'cased' letter after the index
					for (i = index + Character.CharCount(src.CodePointAt(index)); (i < len) && !wordBoundary.IsBoundary(i); i += Character.CharCount(ch))
					{

						ch = src.CodePointAt(i);
						if (IsCased(ch))
						{
							return false;
						}
					}

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Implements the "After_I" condition
		/// 
		/// Specification: The last preceding base character was an uppercase I,
		/// and there is no intervening combining character class 230 (ABOVE).
		/// 
		/// Regular Expression:
		///   Before C: [I]([{cc!=230}&{cc!=0}])*
		/// </summary>
		private static bool IsAfterI(String src, int index)
		{
			int ch;
			int cc;

			// Look for the last preceding base character
			for (int i = index; i > 0; i -= Character.CharCount(ch))
			{

				ch = src.CodePointBefore(i);

				if (ch == 'I')
				{
					return true;
				}
				else
				{
					cc = Normalizer.getCombiningClass(ch);
					if ((cc == 0) || (cc == COMBINING_CLASS_ABOVE))
					{
						return false;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Implements the "After_Soft_Dotted" condition
		/// 
		/// Specification: The last preceding character with combining class
		/// of zero before C was Soft_Dotted, and there is no intervening
		/// combining character class 230 (ABOVE).
		/// 
		/// Regular Expression:
		///   Before C: [{Soft_Dotted==true}]([{cc!=230}&{cc!=0}])*
		/// </summary>
		private static bool IsAfterSoftDotted(String src, int index)
		{
			int ch;
			int cc;

			// Look for the last preceding character
			for (int i = index; i > 0; i -= Character.CharCount(ch))
			{

				ch = src.CodePointBefore(i);

				if (IsSoftDotted(ch))
				{
					return true;
				}
				else
				{
					cc = Normalizer.getCombiningClass(ch);
					if ((cc == 0) || (cc == COMBINING_CLASS_ABOVE))
					{
						return false;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Implements the "More_Above" condition
		/// 
		/// Specification: C is followed by one or more characters of combining
		/// class 230 (ABOVE) in the combining character sequence.
		/// 
		/// Regular Expression:
		///   After C: [{cc!=0}]*[{cc==230}]
		/// </summary>
		private static bool IsMoreAbove(String src, int index)
		{
			int ch;
			int cc;
			int len = src.Length();

			// Look for a following ABOVE combining class character
			for (int i = index + Character.CharCount(src.CodePointAt(index)); i < len; i += Character.CharCount(ch))
			{

				ch = src.CodePointAt(i);
				cc = Normalizer.getCombiningClass(ch);

				if (cc == COMBINING_CLASS_ABOVE)
				{
					return true;
				}
				else if (cc == 0)
				{
					return false;
				}
			}

			return false;
		}

		/// <summary>
		/// Implements the "Before_Dot" condition
		/// 
		/// Specification: C is followed by <code>U+0307 COMBINING DOT ABOVE</code>.
		/// Any sequence of characters with a combining class that is
		/// neither 0 nor 230 may intervene between the current character
		/// and the combining dot above.
		/// 
		/// Regular Expression:
		///   After C: ([{cc!=230}&{cc!=0}])*[\u0307]
		/// </summary>
		private static bool IsBeforeDot(String src, int index)
		{
			int ch;
			int cc;
			int len = src.Length();

			// Look for a following COMBINING DOT ABOVE
			for (int i = index + Character.CharCount(src.CodePointAt(index)); i < len; i += Character.CharCount(ch))
			{

				ch = src.CodePointAt(i);

				if (ch == '\u0307')
				{
					return true;
				}
				else
				{
					cc = Normalizer.getCombiningClass(ch);
					if ((cc == 0) || (cc == COMBINING_CLASS_ABOVE))
					{
						return false;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Examines whether a character is 'cased'.
		/// 
		/// A character C is defined to be 'cased' if and only if at least one of
		/// following are true for C: uppercase==true, or lowercase==true, or
		/// general_category==titlecase_letter.
		/// 
		/// The uppercase and lowercase property values are specified in the data
		/// file DerivedCoreProperties.txt in the Unicode Character Database.
		/// </summary>
		private static bool IsCased(int ch)
		{
			int type = Character.GetType(ch);
			if (type == Character.LOWERCASE_LETTER || type == Character.UPPERCASE_LETTER || type == Character.TITLECASE_LETTER)
			{
				return true;
			}
			else
			{
				// Check for Other_Lowercase and Other_Uppercase
				//
				if ((ch >= 0x02B0) && (ch <= 0x02B8))
				{
					// MODIFIER LETTER SMALL H..MODIFIER LETTER SMALL Y
					return true;
				}
				else if ((ch >= 0x02C0) && (ch <= 0x02C1))
				{
					// MODIFIER LETTER GLOTTAL STOP..MODIFIER LETTER REVERSED GLOTTAL STOP
					return true;
				}
				else if ((ch >= 0x02E0) && (ch <= 0x02E4))
				{
					// MODIFIER LETTER SMALL GAMMA..MODIFIER LETTER SMALL REVERSED GLOTTAL STOP
					return true;
				}
				else if (ch == 0x0345)
				{
					// COMBINING GREEK YPOGEGRAMMENI
					return true;
				}
				else if (ch == 0x037A)
				{
					// GREEK YPOGEGRAMMENI
					return true;
				}
				else if ((ch >= 0x1D2C) && (ch <= 0x1D61))
				{
					// MODIFIER LETTER CAPITAL A..MODIFIER LETTER SMALL CHI
					return true;
				}
				else if ((ch >= 0x2160) && (ch <= 0x217F))
				{
					// ROMAN NUMERAL ONE..ROMAN NUMERAL ONE THOUSAND
					// SMALL ROMAN NUMERAL ONE..SMALL ROMAN NUMERAL ONE THOUSAND
					return true;
				}
				else if ((ch >= 0x24B6) && (ch <= 0x24E9))
				{
					// CIRCLED LATIN CAPITAL LETTER A..CIRCLED LATIN CAPITAL LETTER Z
					// CIRCLED LATIN SMALL LETTER A..CIRCLED LATIN SMALL LETTER Z
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		private static bool IsSoftDotted(int ch)
		{
			switch (ch)
			{
			case 0x0069: // Soft_Dotted # L&       LATIN SMALL LETTER I
			case 0x006A: // Soft_Dotted # L&       LATIN SMALL LETTER J
			case 0x012F: // Soft_Dotted # L&       LATIN SMALL LETTER I WITH OGONEK
			case 0x0268: // Soft_Dotted # L&       LATIN SMALL LETTER I WITH STROKE
			case 0x0456: // Soft_Dotted # L&       CYRILLIC SMALL LETTER BYELORUSSIAN-UKRAINIAN I
			case 0x0458: // Soft_Dotted # L&       CYRILLIC SMALL LETTER JE
			case 0x1D62: // Soft_Dotted # L&       LATIN SUBSCRIPT SMALL LETTER I
			case 0x1E2D: // Soft_Dotted # L&       LATIN SMALL LETTER I WITH TILDE BELOW
			case 0x1ECB: // Soft_Dotted # L&       LATIN SMALL LETTER I WITH DOT BELOW
			case 0x2071: // Soft_Dotted # L&       SUPERSCRIPT LATIN SMALL LETTER I
				return true;
			default:
				return false;
			}
		}

		/// <summary>
		/// An internal class that represents an entry in the Special Casing Properties.
		/// </summary>
		internal class Entry
		{
			internal int Ch;
			internal char[] Lower;
			internal char[] Upper;
			internal String Lang;
			internal int Condition_Renamed;

			internal Entry(int ch, char[] lower, char[] upper, String lang, int condition)
			{
				this.Ch = ch;
				this.Lower = lower;
				this.Upper = upper;
				this.Lang = lang;
				this.Condition_Renamed = condition;
			}

			internal virtual int CodePoint
			{
				get
				{
					return Ch;
				}
			}

			internal virtual char[] LowerCase
			{
				get
				{
					return Lower;
				}
			}

			internal virtual char[] UpperCase
			{
				get
				{
					return Upper;
				}
			}

			internal virtual String Language
			{
				get
				{
					return Lang;
				}
			}

			internal virtual int Condition
			{
				get
				{
					return Condition_Renamed;
				}
			}
		}
	}

}