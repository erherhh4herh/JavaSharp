using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.font
{


	/// <summary>
	/// The <code>NumericShaper</code> class is used to convert Latin-1 (European)
	/// digits to other Unicode decimal digits.  Users of this class will
	/// primarily be people who wish to present data using
	/// national digit shapes, but find it more convenient to represent the
	/// data internally using Latin-1 (European) digits.  This does not
	/// interpret the deprecated numeric shape selector character (U+206E).
	/// <para>
	/// Instances of <code>NumericShaper</code> are typically applied
	/// as attributes to text with the
	/// <seealso cref="TextAttribute#NUMERIC_SHAPING NUMERIC_SHAPING"/> attribute
	/// of the <code>TextAttribute</code> class.
	/// For example, this code snippet causes a <code>TextLayout</code> to
	/// shape European digits to Arabic in an Arabic context:<br>
	/// <blockquote><pre>
	/// Map map = new HashMap();
	/// map.put(TextAttribute.NUMERIC_SHAPING,
	///     NumericShaper.getContextualShaper(NumericShaper.ARABIC));
	/// FontRenderContext frc = ...;
	/// TextLayout layout = new TextLayout(text, map, frc);
	/// layout.draw(g2d, x, y);
	/// </pre></blockquote>
	/// <br>
	/// It is also possible to perform numeric shaping explicitly using instances
	/// of <code>NumericShaper</code>, as this code snippet demonstrates:<br>
	/// <blockquote><pre>
	/// char[] text = ...;
	/// // shape all EUROPEAN digits (except zero) to ARABIC digits
	/// NumericShaper shaper = NumericShaper.getShaper(NumericShaper.ARABIC);
	/// shaper.shape(text, start, count);
	/// 
	/// // shape European digits to ARABIC digits if preceding text is Arabic, or
	/// // shape European digits to TAMIL digits if preceding text is Tamil, or
	/// // leave European digits alone if there is no preceding text, or
	/// // preceding text is neither Arabic nor Tamil
	/// NumericShaper shaper =
	///     NumericShaper.getContextualShaper(NumericShaper.ARABIC |
	///                                         NumericShaper.TAMIL,
	///                                       NumericShaper.EUROPEAN);
	/// shaper.shape(text, start, count);
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para><b>Bit mask- and enum-based Unicode ranges</b></para>
	/// 
	/// <para>This class supports two different programming interfaces to
	/// represent Unicode ranges for script-specific digits: bit
	/// mask-based ones, such as <seealso cref="#ARABIC NumericShaper.ARABIC"/>, and
	/// enum-based ones, such as <seealso cref="NumericShaper.Range#ARABIC"/>.
	/// Multiple ranges can be specified by ORing bit mask-based constants,
	/// such as:
	/// <blockquote><pre>
	/// NumericShaper.ARABIC | NumericShaper.TAMIL
	/// </pre></blockquote>
	/// or creating a {@code Set} with the <seealso cref="NumericShaper.Range"/>
	/// constants, such as:
	/// <blockquote><pre>
	/// EnumSet.of(NumericShaper.Scirpt.ARABIC, NumericShaper.Range.TAMIL)
	/// </pre></blockquote>
	/// The enum-based ranges are a super set of the bit mask-based ones.
	/// 
	/// </para>
	/// <para>If the two interfaces are mixed (including serialization),
	/// Unicode range values are mapped to their counterparts where such
	/// mapping is possible, such as {@code NumericShaper.Range.ARABIC}
	/// from/to {@code NumericShaper.ARABIC}.  If any unmappable range
	/// values are specified, such as {@code NumericShaper.Range.BALINESE},
	/// those ranges are ignored.
	/// 
	/// </para>
	/// <para><b>Decimal Digits Precedence</b></para>
	/// 
	/// <para>A Unicode range may have more than one set of decimal digits. If
	/// multiple decimal digits sets are specified for the same Unicode
	/// range, one of the sets will take precedence as follows.
	/// 
	/// <table border=1 cellspacing=3 cellpadding=0 summary="NumericShaper constants precedence.">
	///    <tr>
	///       <th class="TableHeadingColor">Unicode Range</th>
	///       <th class="TableHeadingColor"><code>NumericShaper</code> Constants</th>
	///       <th class="TableHeadingColor">Precedence</th>
	///    </tr>
	///    <tr>
	///       <td rowspan="2">Arabic</td>
	///       <td><seealso cref="NumericShaper#ARABIC NumericShaper.ARABIC"/><br>
	///           <seealso cref="NumericShaper#EASTERN_ARABIC NumericShaper.EASTERN_ARABIC"/></td>
	///       <td><seealso cref="NumericShaper#EASTERN_ARABIC NumericShaper.EASTERN_ARABIC"/></td>
	///    </tr>
	///    <tr>
	///       <td><seealso cref="NumericShaper.Range#ARABIC"/><br>
	///           <seealso cref="NumericShaper.Range#EASTERN_ARABIC"/></td>
	///       <td><seealso cref="NumericShaper.Range#EASTERN_ARABIC"/></td>
	///    </tr>
	///    <tr>
	///       <td>Tai Tham</td>
	///       <td><seealso cref="NumericShaper.Range#TAI_THAM_HORA"/><br>
	///           <seealso cref="NumericShaper.Range#TAI_THAM_THAM"/></td>
	///       <td><seealso cref="NumericShaper.Range#TAI_THAM_THAM"/></td>
	///    </tr>
	/// </table>
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	[Serializable]
	public sealed class NumericShaper
	{
		/// <summary>
		/// A {@code NumericShaper.Range} represents a Unicode range of a
		/// script having its own decimal digits. For example, the {@link
		/// NumericShaper.Range#THAI} range has the Thai digits, THAI DIGIT
		/// ZERO (U+0E50) to THAI DIGIT NINE (U+0E59).
		/// 
		/// <para>The <code>Range</code> enum replaces the traditional bit
		/// mask-based values (e.g., <seealso cref="NumericShaper#ARABIC"/>), and
		/// supports more Unicode ranges than the bit mask-based ones. For
		/// example, the following code using the bit mask:
		/// <blockquote><pre>
		/// NumericShaper.getContextualShaper(NumericShaper.ARABIC |
		///                                     NumericShaper.TAMIL,
		///                                   NumericShaper.EUROPEAN);
		/// </pre></blockquote>
		/// can be written using this enum as:
		/// <blockquote><pre>
		/// NumericShaper.getContextualShaper(EnumSet.of(
		///                                     NumericShaper.Range.ARABIC,
		///                                     NumericShaper.Range.TAMIL),
		///                                   NumericShaper.Range.EUROPEAN);
		/// </pre></blockquote>
		/// 
		/// @since 1.7
		/// </para>
		/// </summary>
		public sealed class Range
		{
			// The order of EUROPEAN to MOGOLIAN must be consistent
			// with the bitmask-based constants.
			/// <summary>
			/// The Latin (European) range with the Latin (ASCII) digits.
			/// </summary>
			public static readonly Range EUROPEAN = new Range("EUROPEAN", InnerEnum.EUROPEAN, '\u0030', '\u0000', '\u0300');
			/// <summary>
			/// The Arabic range with the Arabic-Indic digits.
			/// </summary>
			public static readonly Range ARABIC = new Range("ARABIC", InnerEnum.ARABIC, '\u0660', '\u0600', '\u0780');
			/// <summary>
			/// The Arabic range with the Eastern Arabic-Indic digits.
			/// </summary>
			public static readonly Range EASTERN_ARABIC = new Range("EASTERN_ARABIC", InnerEnum.EASTERN_ARABIC, '\u06f0', '\u0600', '\u0780');
			/// <summary>
			/// The Devanagari range with the Devanagari digits.
			/// </summary>
			public static readonly Range DEVANAGARI = new Range("DEVANAGARI", InnerEnum.DEVANAGARI, '\u0966', '\u0900', '\u0980');
			/// <summary>
			/// The Bengali range with the Bengali digits.
			/// </summary>
			public static readonly Range BENGALI = new Range("BENGALI", InnerEnum.BENGALI, '\u09e6', '\u0980', '\u0a00');
			/// <summary>
			/// The Gurmukhi range with the Gurmukhi digits.
			/// </summary>
			public static readonly Range GURMUKHI = new Range("GURMUKHI", InnerEnum.GURMUKHI, '\u0a66', '\u0a00', '\u0a80');
			/// <summary>
			/// The Gujarati range with the Gujarati digits.
			/// </summary>
			public static readonly Range GUJARATI = new Range("GUJARATI", InnerEnum.GUJARATI, '\u0ae6', '\u0b00', '\u0b80');
			/// <summary>
			/// The Oriya range with the Oriya digits.
			/// </summary>
			public static readonly Range ORIYA = new Range("ORIYA", InnerEnum.ORIYA, '\u0b66', '\u0b00', '\u0b80');
			/// <summary>
			/// The Tamil range with the Tamil digits.
			/// </summary>
			public static readonly Range TAMIL = new Range("TAMIL", InnerEnum.TAMIL, '\u0be6', '\u0b80', '\u0c00');
			/// <summary>
			/// The Telugu range with the Telugu digits.
			/// </summary>
			public static readonly Range TELUGU = new Range("TELUGU", InnerEnum.TELUGU, '\u0c66', '\u0c00', '\u0c80');
			/// <summary>
			/// The Kannada range with the Kannada digits.
			/// </summary>
			public static readonly Range KANNADA = new Range("KANNADA", InnerEnum.KANNADA, '\u0ce6', '\u0c80', '\u0d00');
			/// <summary>
			/// The Malayalam range with the Malayalam digits.
			/// </summary>
			public static readonly Range MALAYALAM = new Range("MALAYALAM", InnerEnum.MALAYALAM, '\u0d66', '\u0d00', '\u0d80');
			/// <summary>
			/// The Thai range with the Thai digits.
			/// </summary>
			public static readonly Range THAI = new Range("THAI", InnerEnum.THAI, '\u0e50', '\u0e00', '\u0e80');
			/// <summary>
			/// The Lao range with the Lao digits.
			/// </summary>
			public static readonly Range LAO = new Range("LAO", InnerEnum.LAO, '\u0ed0', '\u0e80', '\u0f00');
			/// <summary>
			/// The Tibetan range with the Tibetan digits.
			/// </summary>
			public static readonly Range TIBETAN = new Range("TIBETAN", InnerEnum.TIBETAN, '\u0f20', '\u0f00', '\u1000');
			/// <summary>
			/// The Myanmar range with the Myanmar digits.
			/// </summary>
			public static readonly Range MYANMAR = new Range("MYANMAR", InnerEnum.MYANMAR, '\u1040', '\u1000', '\u1080');
			/// <summary>
			/// The Ethiopic range with the Ethiopic digits. Ethiopic
			/// does not have a decimal digit 0 so Latin (European) 0 is
			/// used.
			/// </summary>
			internal ETHIOPIC('\u1369', '\u1200', '\u1380')
			{
				char outerInstance.NumericBase { return 1;
			}
		},
			public static readonly Range } = new Range("}", InnerEnum.});
			/// <summary>
			/// The Khmer range with the Khmer digits.
			/// </summary>
			public static readonly Range KHMER('\u17e0', '\u1780', '\u1800'), MONGOLIAN('\u1810', '\u1800', '\u1900'), NKO('\u07c0', '\u07c0', '\u0800'), MYANMAR_SHAN('\u1090', '\u1000', '\u10a0'), LIMBU('\u1946', '\u1900', '\u1950'), NEW_TAI_LUE('\u19d0', '\u1980', '\u19e0'), BALINESE('\u1b50', '\u1b00', '\u1b80'), SUNDANESE('\u1bb0', '\u1b80', '\u1bc0'), LEPCHA('\u1c40', '\u1c00', '\u1c50'), OL_CHIKI('\u1c50', '\u1c50', '\u1c80'), VAI('\ua620', '\ua500', '\ua640'), SAURASHTRA('\ua8d0', '\ua880', '\ua8e0'), KAYAH_LI('\ua900', '\ua900', '\ua930'), CHAM('\uaa50', '\uaa00', '\uaa60'), TAI_THAM_HORA('\u1a80', '\u1a20', '\u1ab0'), TAI_THAM_THAM('\u1a90', '\u1a20', '\u1ab0'), JAVANESE('\ua9d0', '\ua980', '\ua9e0'), MEETEI_MAYEK = new Range("KHMER('\u17e0', '\u1780', '\u1800'), MONGOLIAN('\u1810', '\u1800', '\u1900'), NKO('\u07c0', '\u07c0', '\u0800'), MYANMAR_SHAN('\u1090', '\u1000', '\u10a0'), LIMBU('\u1946', '\u1900', '\u1950'), NEW_TAI_LUE('\u19d0', '\u1980', '\u19e0'), BALINESE('\u1b50', '\u1b00', '\u1b80'), SUNDANESE('\u1bb0', '\u1b80', '\u1bc0'), LEPCHA('\u1c40', '\u1c00', '\u1c50'), OL_CHIKI('\u1c50', '\u1c50', '\u1c80'), VAI('\ua620', '\ua500', '\ua640'), SAURASHTRA('\ua8d0', '\ua880', '\ua8e0'), KAYAH_LI('\ua900', '\ua900', '\ua930'), CHAM('\uaa50', '\uaa00', '\uaa60'), TAI_THAM_HORA('\u1a80', '\u1a20', '\u1ab0'), TAI_THAM_THAM('\u1a90', '\u1a20', '\u1ab0'), JAVANESE('\ua9d0', '\ua980', '\ua9e0'), MEETEI_MAYEK", InnerEnum.KHMER('\u17e0', '\u1780', '\u1800'), MONGOLIAN('\u1810', '\u1800', '\u1900'), NKO('\u07c0', '\u07c0', '\u0800'), MYANMAR_SHAN('\u1090', '\u1000', '\u10a0'), LIMBU('\u1946', '\u1900', '\u1950'), NEW_TAI_LUE('\u19d0', '\u1980', '\u19e0'), BALINESE('\u1b50', '\u1b00', '\u1b80'), SUNDANESE('\u1bb0', '\u1b80', '\u1bc0'), LEPCHA('\u1c40', '\u1c00', '\u1c50'), OL_CHIKI('\u1c50', '\u1c50', '\u1c80'), VAI('\ua620', '\ua500', '\ua640'), SAURASHTRA('\ua8d0', '\ua880', '\ua8e0'), KAYAH_LI('\ua900', '\ua900', '\ua930'), CHAM('\uaa50', '\uaa00', '\uaa60'), TAI_THAM_HORA('\u1a80', '\u1a20', '\u1ab0'), TAI_THAM_THAM('\u1a90', '\u1a20', '\u1ab0'), JAVANESE('\ua9d0', '\ua980', '\ua9e0'), MEETEI_MAYEK, '\uabf0', '\uabc0', '\uac00');

			private static readonly IList<Range> valueList = new List<Range>();

			static Range()
			{
				valueList.Add(EUROPEAN);
				valueList.Add(ARABIC);
				valueList.Add(EASTERN_ARABIC);
				valueList.Add(DEVANAGARI);
				valueList.Add(BENGALI);
				valueList.Add(GURMUKHI);
				valueList.Add(GUJARATI);
				valueList.Add(ORIYA);
				valueList.Add(TAMIL);
				valueList.Add(TELUGU);
				valueList.Add(KANNADA);
				valueList.Add(MALAYALAM);
				valueList.Add(THAI);
				valueList.Add(LAO);
				valueList.Add(TIBETAN);
				valueList.Add(MYANMAR);
				valueList.Add(});
				valueList.Add(KHMER('\u17e0', '\u1780', '\u1800'), MONGOLIAN('\u1810', '\u1800', '\u1900'), NKO('\u07c0', '\u07c0', '\u0800'), MYANMAR_SHAN('\u1090', '\u1000', '\u10a0'), LIMBU('\u1946', '\u1900', '\u1950'), NEW_TAI_LUE('\u19d0', '\u1980', '\u19e0'), BALINESE('\u1b50', '\u1b00', '\u1b80'), SUNDANESE('\u1bb0', '\u1b80', '\u1bc0'), LEPCHA('\u1c40', '\u1c00', '\u1c50'), OL_CHIKI('\u1c50', '\u1c50', '\u1c80'), VAI('\ua620', '\ua500', '\ua640'), SAURASHTRA('\ua8d0', '\ua880', '\ua8e0'), KAYAH_LI('\ua900', '\ua900', '\ua930'), CHAM('\uaa50', '\uaa00', '\uaa60'), TAI_THAM_HORA('\u1a80', '\u1a20', '\u1ab0'), TAI_THAM_THAM('\u1a90', '\u1a20', '\u1ab0'), JAVANESE('\ua9d0', '\ua980', '\ua9e0'), MEETEI_MAYEK);
			}

			public enum InnerEnum
			{
				EUROPEAN,
				ARABIC,
				EASTERN_ARABIC,
				DEVANAGARI,
				BENGALI,
				GURMUKHI,
				GUJARATI,
				ORIYA,
				TAMIL,
				TELUGU,
				KANNADA,
				MALAYALAM,
				THAI,
				LAO,
				TIBETAN,
				MYANMAR,
			},
				KHMER('\u17e0', '\u1780', '\u1800'), MONGOLIAN('\u1810', '\u1800', '\u1900'), NKO('\u07c0', '\u07c0', '\u0800'), MYANMAR_SHAN('\u1090', '\u1000', '\u10a0'), LIMBU('\u1946', '\u1900', '\u1950'), NEW_TAI_LUE('\u19d0', '\u1980', '\u19e0'), BALINESE('\u1b50', '\u1b00', '\u1b80'), SUNDANESE('\u1bb0', '\u1b80', '\u1bc0'), LEPCHA('\u1c40', '\u1c00', '\u1c50'), OL_CHIKI('\u1c50', '\u1c50', '\u1c80'), VAI('\ua620', '\ua500', '\ua640'), SAURASHTRA('\ua8d0', '\ua880', '\ua8e0'), KAYAH_LI('\ua900', '\ua900', '\ua930'), CHAM('\uaa50', '\uaa00', '\uaa60'), TAI_THAM_HORA('\u1a80', '\u1a20', '\u1ab0'), TAI_THAM_THAM('\u1a90', '\u1a20', '\u1ab0'), JAVANESE('\ua9d0', '\ua980', '\ua9e0'), MEETEI_MAYEK
	}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;
			/// <summary>
			/// The Mongolian range with the Mongolian digits.
			/// </summary>
			// The order of EUROPEAN to MOGOLIAN must be consistent
			// with the bitmask-based constants.
			/// <summary>
			/// The N'Ko range with the N'Ko digits.
			/// </summary>
			/// <summary>
			/// The Myanmar range with the Myanmar Shan digits.
			/// </summary>
			/// <summary>
			/// The Limbu range with the Limbu digits.
			/// </summary>
			/// <summary>
			/// The New Tai Lue range with the New Tai Lue digits.
			/// </summary>
			/// <summary>
			/// The Balinese range with the Balinese digits.
			/// </summary>
			/// <summary>
			/// The Sundanese range with the Sundanese digits.
			/// </summary>
			/// <summary>
			/// The Lepcha range with the Lepcha digits.
			/// </summary>
			/// <summary>
			/// The Ol Chiki range with the Ol Chiki digits.
			/// </summary>
			/// <summary>
			/// The Vai range with the Vai digits.
			/// </summary>
			/// <summary>
			/// The Saurashtra range with the Saurashtra digits.
			/// </summary>
			/// <summary>
			/// The Kayah Li range with the Kayah Li digits.
			/// </summary>
			/// <summary>
			/// The Cham range with the Cham digits.
			/// </summary>
			/// <summary>
			/// The Tai Tham Hora range with the Tai Tham Hora digits.
			/// </summary>
			/// <summary>
			/// The Tai Tham Tham range with the Tai Tham Tham digits.
			/// </summary>
			/// <summary>
			/// The Javanese range with the Javanese digits.
			/// </summary>
			/// <summary>
			/// The Meetei Mayek range with the Meetei Mayek digits.
			/// </summary>

			internal static int ToRangeIndex(Range script)
			{
				int index = script.ordinal();
				return index < NUM_KEYS ? index : -1;
			}

			internal static Range IndexToRange(int index)
			{
				return index < NUM_KEYS ? Range.values()[index] : null;
			}

			internal static int ToRangeMask(java.util.Set<Range> ranges)
			{
				int m = 0;
				foreach (Range range in ranges)
				{
					int index = range.ordinal();
					if (index < NUM_KEYS)
					{
						m |= 1 << index;
					}
				}
				return m;
			}

			internal static java.util.Set<Range> MaskToRangeSet(int mask)
			{
				Set<Range> set = EnumSet.NoneOf(typeof(Range));
				Range[] a = Range.values();
				for (int i = 0; i < NUM_KEYS; i++)
				{
					if ((mask & (1 << i)) != 0)
					{
						set.Add(a[i]);
					}
				}
				return set;
			}

			// base character of range digits
			internal readonly int @base;
			// Unicode range
			internal readonly int start, end; // exclusive -  inclusive

			internal Range(string name, InnerEnum innerEnum, NumericShaper outerInstance, int @base, int start, int end)
			{
				this.outerInstance = outerInstance;
				this.@base = @base - ('0' + outerInstance.NumericBase);
				this.Start = start;
				this.End = end;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			internal int DigitBase
			{
				get
				{
					return outerInstance.@base;
				}
			}

			internal char NumericBase
			{
				get
				{
					return 0;
				}
			}

			internal boolean InRange(int c)
			{
				return outerInstance.Start <= c && c < outerInstance.End;
			}

			public static IList<Range> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static Range valueOf(string name)
			{
				foreach (Range enumInstance in Range.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
}

		/// <summary>
		/// index of context for contextual shaping - values range from 0 to 18 </summary>
		private int key;

		/// <summary>
		/// flag indicating whether to shape contextually (high bit) and which
		///  digit ranges to shape (bits 0-18)
		/// </summary>
		private int mask;

		/// <summary>
		/// The context {@code Range} for contextual shaping or the {@code
		/// Range} for non-contextual shaping. {@code null} for the bit
		/// mask-based API.
		/// 
		/// @since 1.7
		/// </summary>
		private Range shapingRange;

		/// <summary>
		/// {@code Set<Range>} indicating which Unicode ranges to
		/// shape. {@code null} for the bit mask-based API.
		/// </summary>
		[NonSerialized]
		private Set<Range> rangeSet;

		/// <summary>
		/// rangeSet.toArray() value. Sorted by Range.base when the number
		/// of elements is greater then BSEARCH_THRESHOLD.
		/// </summary>
		[NonSerialized]
		private Range[] rangeArray;

		/// <summary>
		/// If more than BSEARCH_THRESHOLD ranges are specified, binary search is used.
		/// </summary>
		private const int BSEARCH_THRESHOLD = 3;

		private const long serialVersionUID = -8022764705923730308L;

		/// <summary>
		/// Identifies the Latin-1 (European) and extended range, and
		///  Latin-1 (European) decimal base.
		/// </summary>
		public static readonly int EUROPEAN = 1 << 0;

		/// <summary>
		/// Identifies the ARABIC range and decimal base. </summary>
		public static readonly int ARABIC = 1 << 1;

		/// <summary>
		/// Identifies the ARABIC range and ARABIC_EXTENDED decimal base. </summary>
		public static readonly int EASTERN_ARABIC = 1 << 2;

		/// <summary>
		/// Identifies the DEVANAGARI range and decimal base. </summary>
		public static readonly int DEVANAGARI = 1 << 3;

		/// <summary>
		/// Identifies the BENGALI range and decimal base. </summary>
		public static readonly int BENGALI = 1 << 4;

		/// <summary>
		/// Identifies the GURMUKHI range and decimal base. </summary>
		public static readonly int GURMUKHI = 1 << 5;

		/// <summary>
		/// Identifies the GUJARATI range and decimal base. </summary>
		public static readonly int GUJARATI = 1 << 6;

		/// <summary>
		/// Identifies the ORIYA range and decimal base. </summary>
		public static readonly int ORIYA = 1 << 7;

		/// <summary>
		/// Identifies the TAMIL range and decimal base. </summary>
		// TAMIL DIGIT ZERO was added in Unicode 4.1
		public static readonly int TAMIL = 1 << 8;

		/// <summary>
		/// Identifies the TELUGU range and decimal base. </summary>
		public static readonly int TELUGU = 1 << 9;

		/// <summary>
		/// Identifies the KANNADA range and decimal base. </summary>
		public static readonly int KANNADA = 1 << 10;

		/// <summary>
		/// Identifies the MALAYALAM range and decimal base. </summary>
		public static readonly int MALAYALAM = 1 << 11;

		/// <summary>
		/// Identifies the THAI range and decimal base. </summary>
		public static readonly int THAI = 1 << 12;

		/// <summary>
		/// Identifies the LAO range and decimal base. </summary>
		public static readonly int LAO = 1 << 13;

		/// <summary>
		/// Identifies the TIBETAN range and decimal base. </summary>
		public static readonly int TIBETAN = 1 << 14;

		/// <summary>
		/// Identifies the MYANMAR range and decimal base. </summary>
		public static readonly int MYANMAR = 1 << 15;

		/// <summary>
		/// Identifies the ETHIOPIC range and decimal base. </summary>
		public static readonly int ETHIOPIC = 1 << 16;

		/// <summary>
		/// Identifies the KHMER range and decimal base. </summary>
		public static readonly int KHMER = 1 << 17;

		/// <summary>
		/// Identifies the MONGOLIAN range and decimal base. </summary>
		public static readonly int MONGOLIAN = 1 << 18;

		/// <summary>
		/// Identifies all ranges, for full contextual shaping.
		/// 
		/// <para>This constant specifies all of the bit mask-based
		/// ranges. Use {@code EmunSet.allOf(NumericShaper.Range.class)} to
		/// specify all of the enum-based ranges.
		/// </para>
		/// </summary>
		public const int ALL_RANGES = 0x0007ffff;

		private const int EUROPEAN_KEY = 0;
		private const int ARABIC_KEY = 1;
		private const int EASTERN_ARABIC_KEY = 2;
		private const int DEVANAGARI_KEY = 3;
		private const int BENGALI_KEY = 4;
		private const int GURMUKHI_KEY = 5;
		private const int GUJARATI_KEY = 6;
		private const int ORIYA_KEY = 7;
		private const int TAMIL_KEY = 8;
		private const int TELUGU_KEY = 9;
		private const int KANNADA_KEY = 10;
		private const int MALAYALAM_KEY = 11;
		private const int THAI_KEY = 12;
		private const int LAO_KEY = 13;
		private const int TIBETAN_KEY = 14;
		private const int MYANMAR_KEY = 15;
		private const int ETHIOPIC_KEY = 16;
		private const int KHMER_KEY = 17;
		private const int MONGOLIAN_KEY = 18;

		private static readonly int NUM_KEYS = MONGOLIAN_KEY + 1; // fixed

		private static readonly int CONTEXTUAL_MASK = 1 << 31;

		private static readonly char[] bases = new char[] {'\u0030' - '\u0030', '\u0660' - '\u0030', '\u06f0' - '\u0030', '\u0966' - '\u0030', '\u09e6' - '\u0030', '\u0a66' - '\u0030', '\u0ae6' - '\u0030', '\u0b66' - '\u0030', '\u0be6' - '\u0030', '\u0c66' - '\u0030', '\u0ce6' - '\u0030', '\u0d66' - '\u0030', '\u0e50' - '\u0030', '\u0ed0' - '\u0030', '\u0f20' - '\u0030', '\u1040' - '\u0030', '\u1369' - '\u0031', '\u17e0' - '\u0030', '\u1810' - '\u0030'};

		// some ranges adjoin or overlap, rethink if we want to do a binary search on this

		private static readonly char[] contexts = new char[] {'\u0000', '\u0300', '\u0600', '\u0780', '\u0600', '\u0780', '\u0900', '\u0980', '\u0980', '\u0a00', '\u0a00', '\u0a80', '\u0a80', '\u0b00', '\u0b00', '\u0b80', '\u0b80', '\u0c00', '\u0c00', '\u0c80', '\u0c80', '\u0d00', '\u0d00', '\u0d80', '\u0e00', '\u0e80', '\u0e80', '\u0f00', '\u0f00', '\u1000', '\u1000', '\u1080', '\u1200', '\u1380', '\u1780', '\u1800', '\u1800', '\u1900', '\uffff'};

		// assume most characters are near each other so probing the cache is infrequent,
		// and a linear probe is ok.

		private static int ctCache = 0;
		private static int ctCacheLimit = contexts.length - 2;

		// warning, synchronize access to this as it modifies state
		private static int GetContextKey(char c)
		{
			if (c < contexts[ctCache])
			{
				while (ctCache > 0 && c < contexts[ctCache])
				{
					--ctCache;
				}
			}
			else if (c >= contexts[ctCache + 1])
			{
				while (ctCache < ctCacheLimit && c >= contexts[ctCache + 1])
				{
					++ctCache;
				}
			}

			// if we're not in a known range, then return EUROPEAN as the range key
			return (ctCache & 0x1) == 0 ? (ctCache / 2) : EUROPEAN_KEY;
		}

		// cache for the NumericShaper.Range version
		[NonSerialized]
		private volatile Range currentRange = Range.EUROPEAN;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Range rangeForCodePoint(final int codepoint)
		private Range RangeForCodePoint(int codepoint)
		{
			if (currentRange.inRange(codepoint))
			{
				return currentRange;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Range[] ranges = rangeArray;
			Range[] ranges = rangeArray;
			if (ranges.Length > BSEARCH_THRESHOLD)
			{
				int lo = 0;
				int hi = ranges.Length - 1;
				while (lo <= hi)
				{
					int mid = (lo + hi) / 2;
					Range range = ranges[mid];
					if (codepoint < range.start)
					{
						hi = mid - 1;
					}
					else if (codepoint >= range.end)
					{
						lo = mid + 1;
					}
					else
					{
						currentRange = range;
						return range;
					}
				}
			}
			else
			{
				for (int i = 0; i < ranges.Length; i++)
				{
					if (ranges[i].inRange(codepoint))
					{
						return ranges[i];
					}
				}
			}
			return Range.EUROPEAN;
		}

		/*
		 * A range table of strong directional characters (types L, R, AL).
		 * Even (left) indexes are starts of ranges of non-strong-directional (or undefined)
		 * characters, odd (right) indexes are starts of ranges of strong directional
		 * characters.
		 */
		private static int[] strongTable = new int[] {0x0000, 0x0041, 0x005b, 0x0061, 0x007b, 0x00aa, 0x00ab, 0x00b5, 0x00b6, 0x00ba, 0x00bb, 0x00c0, 0x00d7, 0x00d8, 0x00f7, 0x00f8, 0x02b9, 0x02bb, 0x02c2, 0x02d0, 0x02d2, 0x02e0, 0x02e5, 0x02ee, 0x02ef, 0x0370, 0x0374, 0x0376, 0x037e, 0x0386, 0x0387, 0x0388, 0x03f6, 0x03f7, 0x0483, 0x048a, 0x058a, 0x05be, 0x05bf, 0x05c0, 0x05c1, 0x05c3, 0x05c4, 0x05c6, 0x05c7, 0x05d0, 0x0600, 0x0608, 0x0609, 0x060b, 0x060c, 0x060d, 0x060e, 0x061b, 0x064b, 0x066d, 0x0670, 0x0671, 0x06d6, 0x06e5, 0x06e7, 0x06ee, 0x06f0, 0x06fa, 0x0711, 0x0712, 0x0730, 0x074d, 0x07a6, 0x07b1, 0x07eb, 0x07f4, 0x07f6, 0x07fa, 0x0816, 0x081a, 0x081b, 0x0824, 0x0825, 0x0828, 0x0829, 0x0830, 0x0859, 0x085e, 0x08e4, 0x0903, 0x093a, 0x093b, 0x093c, 0x093d, 0x0941, 0x0949, 0x094d, 0x094e, 0x0951, 0x0958, 0x0962, 0x0964, 0x0981, 0x0982, 0x09bc, 0x09bd, 0x09c1, 0x09c7, 0x09cd, 0x09ce, 0x09e2, 0x09e6, 0x09f2, 0x09f4, 0x09fb, 0x0a03, 0x0a3c, 0x0a3e, 0x0a41, 0x0a59, 0x0a70, 0x0a72, 0x0a75, 0x0a83, 0x0abc, 0x0abd, 0x0ac1, 0x0ac9, 0x0acd, 0x0ad0, 0x0ae2, 0x0ae6, 0x0af1, 0x0b02, 0x0b3c, 0x0b3d, 0x0b3f, 0x0b40, 0x0b41, 0x0b47, 0x0b4d, 0x0b57, 0x0b62, 0x0b66, 0x0b82, 0x0b83, 0x0bc0, 0x0bc1, 0x0bcd, 0x0bd0, 0x0bf3, 0x0c01, 0x0c3e, 0x0c41, 0x0c46, 0x0c58, 0x0c62, 0x0c66, 0x0c78, 0x0c7f, 0x0cbc, 0x0cbd, 0x0ccc, 0x0cd5, 0x0ce2, 0x0ce6, 0x0d41, 0x0d46, 0x0d4d, 0x0d4e, 0x0d62, 0x0d66, 0x0dca, 0x0dcf, 0x0dd2, 0x0dd8, 0x0e31, 0x0e32, 0x0e34, 0x0e40, 0x0e47, 0x0e4f, 0x0eb1, 0x0eb2, 0x0eb4, 0x0ebd, 0x0ec8, 0x0ed0, 0x0f18, 0x0f1a, 0x0f35, 0x0f36, 0x0f37, 0x0f38, 0x0f39, 0x0f3e, 0x0f71, 0x0f7f, 0x0f80, 0x0f85, 0x0f86, 0x0f88, 0x0f8d, 0x0fbe, 0x0fc6, 0x0fc7, 0x102d, 0x1031, 0x1032, 0x1038, 0x1039, 0x103b, 0x103d, 0x103f, 0x1058, 0x105a, 0x105e, 0x1061, 0x1071, 0x1075, 0x1082, 0x1083, 0x1085, 0x1087, 0x108d, 0x108e, 0x109d, 0x109e, 0x135d, 0x1360, 0x1390, 0x13a0, 0x1400, 0x1401, 0x1680, 0x1681, 0x169b, 0x16a0, 0x1712, 0x1720, 0x1732, 0x1735, 0x1752, 0x1760, 0x1772, 0x1780, 0x17b4, 0x17b6, 0x17b7, 0x17be, 0x17c6, 0x17c7, 0x17c9, 0x17d4, 0x17db, 0x17dc, 0x17dd, 0x17e0, 0x17f0, 0x1810, 0x18a9, 0x18aa, 0x1920, 0x1923, 0x1927, 0x1929, 0x1932, 0x1933, 0x1939, 0x1946, 0x19de, 0x1a00, 0x1a17, 0x1a19, 0x1a56, 0x1a57, 0x1a58, 0x1a61, 0x1a62, 0x1a63, 0x1a65, 0x1a6d, 0x1a73, 0x1a80, 0x1b00, 0x1b04, 0x1b34, 0x1b35, 0x1b36, 0x1b3b, 0x1b3c, 0x1b3d, 0x1b42, 0x1b43, 0x1b6b, 0x1b74, 0x1b80, 0x1b82, 0x1ba2, 0x1ba6, 0x1ba8, 0x1baa, 0x1bab, 0x1bac, 0x1be6, 0x1be7, 0x1be8, 0x1bea, 0x1bed, 0x1bee, 0x1bef, 0x1bf2, 0x1c2c, 0x1c34, 0x1c36, 0x1c3b, 0x1cd0, 0x1cd3, 0x1cd4, 0x1ce1, 0x1ce2, 0x1ce9, 0x1ced, 0x1cee, 0x1cf4, 0x1cf5, 0x1dc0, 0x1e00, 0x1fbd, 0x1fbe, 0x1fbf, 0x1fc2, 0x1fcd, 0x1fd0, 0x1fdd, 0x1fe0, 0x1fed, 0x1ff2, 0x1ffd, 0x200e, 0x2010, 0x2071, 0x2074, 0x207f, 0x2080, 0x2090, 0x20a0, 0x2102, 0x2103, 0x2107, 0x2108, 0x210a, 0x2114, 0x2115, 0x2116, 0x2119, 0x211e, 0x2124, 0x2125, 0x2126, 0x2127, 0x2128, 0x2129, 0x212a, 0x212e, 0x212f, 0x213a, 0x213c, 0x2140, 0x2145, 0x214a, 0x214e, 0x2150, 0x2160, 0x2189, 0x2336, 0x237b, 0x2395, 0x2396, 0x249c, 0x24ea, 0x26ac, 0x26ad, 0x2800, 0x2900, 0x2c00, 0x2ce5, 0x2ceb, 0x2cef, 0x2cf2, 0x2cf9, 0x2d00, 0x2d7f, 0x2d80, 0x2de0, 0x3005, 0x3008, 0x3021, 0x302a, 0x3031, 0x3036, 0x3038, 0x303d, 0x3041, 0x3099, 0x309d, 0x30a0, 0x30a1, 0x30fb, 0x30fc, 0x31c0, 0x31f0, 0x321d, 0x3220, 0x3250, 0x3260, 0x327c, 0x327f, 0x32b1, 0x32c0, 0x32cc, 0x32d0, 0x3377, 0x337b, 0x33de, 0x33e0, 0x33ff, 0x3400, 0x4dc0, 0x4e00, 0xa490, 0xa4d0, 0xa60d, 0xa610, 0xa66f, 0xa680, 0xa69f, 0xa6a0, 0xa6f0, 0xa6f2, 0xa700, 0xa722, 0xa788, 0xa789, 0xa802, 0xa803, 0xa806, 0xa807, 0xa80b, 0xa80c, 0xa825, 0xa827, 0xa828, 0xa830, 0xa838, 0xa840, 0xa874, 0xa880, 0xa8c4, 0xa8ce, 0xa8e0, 0xa8f2, 0xa926, 0xa92e, 0xa947, 0xa952, 0xa980, 0xa983, 0xa9b3, 0xa9b4, 0xa9b6, 0xa9ba, 0xa9bc, 0xa9bd, 0xaa29, 0xaa2f, 0xaa31, 0xaa33, 0xaa35, 0xaa40, 0xaa43, 0xaa44, 0xaa4c, 0xaa4d, 0xaab0, 0xaab1, 0xaab2, 0xaab5, 0xaab7, 0xaab9, 0xaabe, 0xaac0, 0xaac1, 0xaac2, 0xaaec, 0xaaee, 0xaaf6, 0xab01, 0xabe5, 0xabe6, 0xabe8, 0xabe9, 0xabed, 0xabf0, 0xfb1e, 0xfb1f, 0xfb29, 0xfb2a, 0xfd3e, 0xfd50, 0xfdfd, 0xfe70, 0xfeff, 0xff21, 0xff3b, 0xff41, 0xff5b, 0xff66, 0xffe0, 0x10000, 0x10101, 0x10102, 0x10140, 0x101d0, 0x101fd, 0x10280, 0x1091f, 0x10920, 0x10a01, 0x10a10, 0x10a38, 0x10a40, 0x10b39, 0x10b40, 0x10e60, 0x11000, 0x11001, 0x11002, 0x11038, 0x11047, 0x11052, 0x11066, 0x11080, 0x11082, 0x110b3, 0x110b7, 0x110b9, 0x110bb, 0x11100, 0x11103, 0x11127, 0x1112c, 0x1112d, 0x11136, 0x11180, 0x11182, 0x111b6, 0x111bf, 0x116ab, 0x116ac, 0x116ad, 0x116ae, 0x116b0, 0x116b6, 0x116b7, 0x116c0, 0x16f8f, 0x16f93, 0x1d167, 0x1d16a, 0x1d173, 0x1d183, 0x1d185, 0x1d18c, 0x1d1aa, 0x1d1ae, 0x1d200, 0x1d360, 0x1d6db, 0x1d6dc, 0x1d715, 0x1d716, 0x1d74f, 0x1d750, 0x1d789, 0x1d78a, 0x1d7c3, 0x1d7c4, 0x1d7ce, 0x1ee00, 0x1eef0, 0x1f110, 0x1f16a, 0x1f170, 0x1f300, 0x1f48c, 0x1f48d, 0x1f524, 0x1f525, 0x20000, 0xe0001, 0xf0000, 0x10fffe, 0x10ffff};


		// use a binary search with a cache

		[NonSerialized]
		private volatile int stCache = 0;

		private bool IsStrongDirectional(char c)
		{
			int cachedIndex = stCache;
			if (c < strongTable[cachedIndex])
			{
				cachedIndex = search(c, strongTable, 0, cachedIndex);
			}
			else if (c >= strongTable[cachedIndex + 1])
			{
				cachedIndex = search(c, strongTable, cachedIndex + 1, strongTable.length - cachedIndex - 1);
			}
			bool val = (cachedIndex & 0x1) == 1;
			stCache = cachedIndex;
			return val;
		}

		private static int GetKeyFromMask(int mask)
		{
			int key = 0;
			while (key < NUM_KEYS && ((mask & (1 << key)) == 0))
			{
				++key;
			}
			if (key == NUM_KEYS || ((mask & ~(1 << key)) != 0))
			{
				throw new IllegalArgumentException("invalid shaper: " + mask.ToString("x"));
			}
			return key;
		}

		/// <summary>
		/// Returns a shaper for the provided unicode range.  All
		/// Latin-1 (EUROPEAN) digits are converted
		/// to the corresponding decimal unicode digits. </summary>
		/// <param name="singleRange"> the specified Unicode range </param>
		/// <returns> a non-contextual numeric shaper </returns>
		/// <exception cref="IllegalArgumentException"> if the range is not a single range </exception>
		public static NumericShaper GetShaper(int singleRange)
		{
			int key = getKeyFromMask(singleRange);
			return new NumericShaper(key, singleRange);
		}

		/// <summary>
		/// Returns a shaper for the provided Unicode
		/// range. All Latin-1 (EUROPEAN) digits are converted to the
		/// corresponding decimal digits of the specified Unicode range.
		/// </summary>
		/// <param name="singleRange"> the Unicode range given by a {@link
		///                    NumericShaper.Range} constant. </param>
		/// <returns> a non-contextual {@code NumericShaper}. </returns>
		/// <exception cref="NullPointerException"> if {@code singleRange} is {@code null}
		/// @since 1.7 </exception>
		public static NumericShaper GetShaper(Range singleRange)
		{
			return new NumericShaper(singleRange, EnumSet.Of(singleRange));
		}

		/// <summary>
		/// Returns a contextual shaper for the provided unicode range(s).
		/// Latin-1 (EUROPEAN) digits are converted to the decimal digits
		/// corresponding to the range of the preceding text, if the
		/// range is one of the provided ranges.  Multiple ranges are
		/// represented by or-ing the values together, such as,
		/// <code>NumericShaper.ARABIC | NumericShaper.THAI</code>.  The
		/// shaper assumes EUROPEAN as the starting context, that is, if
		/// EUROPEAN digits are encountered before any strong directional
		/// text in the string, the context is presumed to be EUROPEAN, and
		/// so the digits will not shape. </summary>
		/// <param name="ranges"> the specified Unicode ranges </param>
		/// <returns> a shaper for the specified ranges </returns>
		public static NumericShaper GetContextualShaper(int ranges)
		{
			ranges |= CONTEXTUAL_MASK;
			return new NumericShaper(EUROPEAN_KEY, ranges);
		}

		/// <summary>
		/// Returns a contextual shaper for the provided Unicode
		/// range(s). The Latin-1 (EUROPEAN) digits are converted to the
		/// decimal digits corresponding to the range of the preceding
		/// text, if the range is one of the provided ranges.
		/// 
		/// <para>The shaper assumes EUROPEAN as the starting context, that
		/// is, if EUROPEAN digits are encountered before any strong
		/// directional text in the string, the context is presumed to be
		/// EUROPEAN, and so the digits will not shape.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ranges"> the specified Unicode ranges </param>
		/// <returns> a contextual shaper for the specified ranges </returns>
		/// <exception cref="NullPointerException"> if {@code ranges} is {@code null}.
		/// @since 1.7 </exception>
		public static NumericShaper GetContextualShaper(Set<Range> ranges)
		{
			NumericShaper shaper = new NumericShaper(Range.EUROPEAN, ranges);
			shaper.mask = CONTEXTUAL_MASK;
			return shaper;
		}

		/// <summary>
		/// Returns a contextual shaper for the provided unicode range(s).
		/// Latin-1 (EUROPEAN) digits will be converted to the decimal digits
		/// corresponding to the range of the preceding text, if the
		/// range is one of the provided ranges.  Multiple ranges are
		/// represented by or-ing the values together, for example,
		/// <code>NumericShaper.ARABIC | NumericShaper.THAI</code>.  The
		/// shaper uses defaultContext as the starting context. </summary>
		/// <param name="ranges"> the specified Unicode ranges </param>
		/// <param name="defaultContext"> the starting context, such as
		/// <code>NumericShaper.EUROPEAN</code> </param>
		/// <returns> a shaper for the specified Unicode ranges. </returns>
		/// <exception cref="IllegalArgumentException"> if the specified
		/// <code>defaultContext</code> is not a single valid range. </exception>
		public static NumericShaper GetContextualShaper(int ranges, int defaultContext)
		{
			int key = getKeyFromMask(defaultContext);
			ranges |= CONTEXTUAL_MASK;
			return new NumericShaper(key, ranges);
		}

		/// <summary>
		/// Returns a contextual shaper for the provided Unicode range(s).
		/// The Latin-1 (EUROPEAN) digits will be converted to the decimal
		/// digits corresponding to the range of the preceding text, if the
		/// range is one of the provided ranges. The shaper uses {@code
		/// defaultContext} as the starting context.
		/// </summary>
		/// <param name="ranges"> the specified Unicode ranges </param>
		/// <param name="defaultContext"> the starting context, such as
		///                       {@code NumericShaper.Range.EUROPEAN} </param>
		/// <returns> a contextual shaper for the specified Unicode ranges. </returns>
		/// <exception cref="NullPointerException">
		///         if {@code ranges} or {@code defaultContext} is {@code null}
		/// @since 1.7 </exception>
		public static NumericShaper GetContextualShaper(Set<Range> ranges, Range defaultContext)
		{
			if (defaultContext == null)
			{
				throw new NullPointerException();
			}
			NumericShaper shaper = new NumericShaper(defaultContext, ranges);
			shaper.mask = CONTEXTUAL_MASK;
			return shaper;
		}

		/// <summary>
		/// Private constructor.
		/// </summary>
		private NumericShaper(int key, int mask)
		{
			this.key = key;
			this.mask = mask;
		}

		private NumericShaper(Range defaultContext, Set<Range> ranges)
		{
			shapingRange = defaultContext;
			rangeSet = EnumSet.CopyOf(ranges); // throws NPE if ranges is null.

			// Give precedance to EASTERN_ARABIC if both ARABIC and
			// EASTERN_ARABIC are specified.
			if (rangeSet.contains(Range.EASTERN_ARABIC) && rangeSet.contains(Range.ARABIC))
			{
				rangeSet.remove(Range.ARABIC);
			}

			// As well as the above case, give precedance to TAI_THAM_THAM if both
			// TAI_THAM_HORA and TAI_THAM_THAM are specified.
			if (rangeSet.contains(Range.TAI_THAM_THAM) && rangeSet.contains(Range.TAI_THAM_HORA))
			{
				rangeSet.remove(Range.TAI_THAM_HORA);
			}

			rangeArray = rangeSet.toArray(new Range[rangeSet.size()]);
			if (rangeArray.length > BSEARCH_THRESHOLD)
			{
				// sort rangeArray for binary search
				Arrays.Sort(rangeArray, new ComparatorAnonymousInnerClassHelper(this));
			}
		}

		private class ComparatorAnonymousInnerClassHelper : Comparator<Range>
		{
			private readonly NumericShaper OuterInstance;

			public ComparatorAnonymousInnerClassHelper(NumericShaper outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual int Compare(Range s1, Range s2)
			{
				return s1.@base > s2.@base ? 1 : s1.@base == s2.@base ? 0 : -1;
			}
		}

		/// <summary>
		/// Converts the digits in the text that occur between start and
		/// start + count. </summary>
		/// <param name="text"> an array of characters to convert </param>
		/// <param name="start"> the index into <code>text</code> to start
		///        converting </param>
		/// <param name="count"> the number of characters in <code>text</code>
		///        to convert </param>
		/// <exception cref="IndexOutOfBoundsException"> if start or start + count is
		///        out of bounds </exception>
		/// <exception cref="NullPointerException"> if text is null </exception>
		public void Shape(char[] text, int start, int count)
		{
			checkParams(text, start, count);
			if (Contextual)
			{
				if (rangeSet == null)
				{
					shapeContextually(text, start, count, key);
				}
				else
				{
					shapeContextually(text, start, count, shapingRange);
				}
			}
			else
			{
				shapeNonContextually(text, start, count);
			}
		}

		/// <summary>
		/// Converts the digits in the text that occur between start and
		/// start + count, using the provided context.
		/// Context is ignored if the shaper is not a contextual shaper. </summary>
		/// <param name="text"> an array of characters </param>
		/// <param name="start"> the index into <code>text</code> to start
		///        converting </param>
		/// <param name="count"> the number of characters in <code>text</code>
		///        to convert </param>
		/// <param name="context"> the context to which to convert the
		///        characters, such as <code>NumericShaper.EUROPEAN</code> </param>
		/// <exception cref="IndexOutOfBoundsException"> if start or start + count is
		///        out of bounds </exception>
		/// <exception cref="NullPointerException"> if text is null </exception>
		/// <exception cref="IllegalArgumentException"> if this is a contextual shaper
		/// and the specified <code>context</code> is not a single valid
		/// range. </exception>
		public void Shape(char[] text, int start, int count, int context)
		{
			checkParams(text, start, count);
			if (Contextual)
			{
				int ctxKey = getKeyFromMask(context);
				if (rangeSet == null)
				{
					shapeContextually(text, start, count, ctxKey);
				}
				else
				{
					shapeContextually(text, start, count, Range.values()[ctxKey]);
				}
			}
			else
			{
				shapeNonContextually(text, start, count);
			}
		}

		/// <summary>
		/// Converts the digits in the text that occur between {@code
		/// start} and {@code start + count}, using the provided {@code
		/// context}. {@code Context} is ignored if the shaper is not a
		/// contextual shaper.
		/// </summary>
		/// <param name="text">  a {@code char} array </param>
		/// <param name="start"> the index into {@code text} to start converting </param>
		/// <param name="count"> the number of {@code char}s in {@code text}
		///              to convert </param>
		/// <param name="context"> the context to which to convert the characters,
		///                such as {@code NumericShaper.Range.EUROPEAN} </param>
		/// <exception cref="IndexOutOfBoundsException">
		///         if {@code start} or {@code start + count} is out of bounds </exception>
		/// <exception cref="NullPointerException">
		///         if {@code text} or {@code context} is null
		/// @since 1.7 </exception>
		public void Shape(char[] text, int start, int count, Range context)
		{
			checkParams(text, start, count);
			if (context == null)
			{
				throw new NullPointerException("context is null");
			}

			if (Contextual)
			{
				if (rangeSet != null)
				{
					shapeContextually(text, start, count, context);
				}
				else
				{
					int key = Range.toRangeIndex(context);
					if (key >= 0)
					{
						shapeContextually(text, start, count, key);
					}
					else
					{
						shapeContextually(text, start, count, shapingRange);
					}
				}
			}
			else
			{
				shapeNonContextually(text, start, count);
			}
		}

		private void CheckParams(char[] text, int start, int count)
		{
			if (text == null)
			{
				throw new NullPointerException("text is null");
			}
			if ((start < 0) || (start > text.Length) || ((start + count) < 0) || ((start + count) > text.Length))
			{
				throw new IndexOutOfBoundsException("bad start or count for text of length " + text.Length);
			}
		}

		/// <summary>
		/// Returns a <code>boolean</code> indicating whether or not
		/// this shaper shapes contextually. </summary>
		/// <returns> <code>true</code> if this shaper is contextual;
		///         <code>false</code> otherwise. </returns>
		public bool Contextual
		{
			get
			{
				return (mask & CONTEXTUAL_MASK) != 0;
			}
		}

		/// <summary>
		/// Returns an <code>int</code> that ORs together the values for
		/// all the ranges that will be shaped.
		/// <para>
		/// For example, to check if a shaper shapes to Arabic, you would use the
		/// following:
		/// <blockquote>
		///   {@code if ((shaper.getRanges() & shaper.ARABIC) != 0) &#123; ... }
		/// </blockquote>
		/// 
		/// </para>
		/// <para>Note that this method supports only the bit mask-based
		/// ranges. Call <seealso cref="#getRangeSet()"/> for the enum-based ranges.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the values for all the ranges to be shaped. </returns>
		public int Ranges
		{
			get
			{
				return mask & ~CONTEXTUAL_MASK;
			}
		}

		/// <summary>
		/// Returns a {@code Set} representing all the Unicode ranges in
		/// this {@code NumericShaper} that will be shaped.
		/// </summary>
		/// <returns> all the Unicode ranges to be shaped.
		/// @since 1.7 </returns>
		public Set<Range> RangeSet
		{
			get
			{
				if (rangeSet != null)
				{
					return EnumSet.CopyOf(rangeSet);
				}
				return Range.maskToRangeSet(mask);
			}
		}

		/// <summary>
		/// Perform non-contextual shaping.
		/// </summary>
		private void ShapeNonContextually(char[] text, int start, int count)
		{
			int @base;
			char minDigit = '0';
			if (shapingRange != null)
			{
				@base = shapingRange.DigitBase;
				minDigit += shapingRange.NumericBase;
			}
			else
			{
				@base = bases[key];
				if (key == ETHIOPIC_KEY)
				{
					minDigit++; // Ethiopic doesn't use decimal zero
				}
			}
			for (int i = start, e = start + count; i < e; ++i)
			{
				char c = text[i];
				if (c >= minDigit && c <= '\u0039')
				{
					text[i] = (char)(c + @base);
				}
			}
		}

		/// <summary>
		/// Perform contextual shaping.
		/// Synchronized to protect caches used in getContextKey.
		/// </summary>
		private void ShapeContextually(char[] text, int start, int count, int ctxKey)
		{
			lock (this)
			{
        
				// if we don't support this context, then don't shape
				if ((mask & (1 << ctxKey)) == 0)
				{
					ctxKey = EUROPEAN_KEY;
				}
				int lastkey = ctxKey;
        
				int @base = bases[ctxKey];
				char minDigit = ctxKey == ETHIOPIC_KEY ? '1' : '0'; // Ethiopic doesn't use decimal zero
        
				lock (typeof(NumericShaper))
				{
					for (int i = start, e = start + count; i < e; ++i)
					{
						char c = text[i];
						if (c >= minDigit && c <= '\u0039')
						{
							text[i] = (char)(c + @base);
						}
        
						if (isStrongDirectional(c))
						{
							int newkey = getContextKey(c);
							if (newkey != lastkey)
							{
								lastkey = newkey;
        
								ctxKey = newkey;
								if (((mask & EASTERN_ARABIC) != 0) && (ctxKey == ARABIC_KEY || ctxKey == EASTERN_ARABIC_KEY))
								{
									ctxKey = EASTERN_ARABIC_KEY;
								}
								else if (((mask & ARABIC) != 0) && (ctxKey == ARABIC_KEY || ctxKey == EASTERN_ARABIC_KEY))
								{
									ctxKey = ARABIC_KEY;
								}
								else if ((mask & (1 << ctxKey)) == 0)
								{
									ctxKey = EUROPEAN_KEY;
								}
        
								@base = bases[ctxKey];
        
								minDigit = ctxKey == ETHIOPIC_KEY ? '1' : '0'; // Ethiopic doesn't use decimal zero
							}
						}
					}
				}
			}
		}

		private void ShapeContextually(char[] text, int start, int count, Range ctxKey)
		{
			// if we don't support the specified context, then don't shape.
			if (ctxKey == null || !rangeSet.contains(ctxKey))
			{
				ctxKey = Range.EUROPEAN;
			}

			Range lastKey = ctxKey;
			int @base = ctxKey.DigitBase;
			char minDigit = (char)('0' + ctxKey.NumericBase);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int end = start + count;
			int end = start + count;
			for (int i = start; i < end; ++i)
			{
				char c = text[i];
				if (c >= minDigit && c <= '9')
				{
					text[i] = (char)(c + @base);
					continue;
				}
				if (isStrongDirectional(c))
				{
					ctxKey = rangeForCodePoint(c);
					if (ctxKey != lastKey)
					{
						lastKey = ctxKey;
						@base = ctxKey.DigitBase;
						minDigit = (char)('0' + ctxKey.NumericBase);
					}
				}
			}
		}

		/// <summary>
		/// Returns a hash code for this shaper. </summary>
		/// <returns> this shaper's hash code. </returns>
		/// <seealso cref= java.lang.Object#hashCode </seealso>
		public override int HashCode()
		{
			int hash = mask;
			if (rangeSet != null)
			{
				// Use the CONTEXTUAL_MASK bit only for the enum-based
				// NumericShaper. A deserialized NumericShaper might have
				// bit masks.
				hash &= CONTEXTUAL_MASK;
				hash ^= rangeSet.HashCode();
			}
			return hash;
		}

		/// <summary>
		/// Returns {@code true} if the specified object is an instance of
		/// <code>NumericShaper</code> and shapes identically to this one,
		/// regardless of the range representations, the bit mask or the
		/// enum. For example, the following code produces {@code "true"}.
		/// <blockquote><pre>
		/// NumericShaper ns1 = NumericShaper.getShaper(NumericShaper.ARABIC);
		/// NumericShaper ns2 = NumericShaper.getShaper(NumericShaper.Range.ARABIC);
		/// System.out.println(ns1.equals(ns2));
		/// </pre></blockquote>
		/// </summary>
		/// <param name="o"> the specified object to compare to this
		///          <code>NumericShaper</code> </param>
		/// <returns> <code>true</code> if <code>o</code> is an instance
		///         of <code>NumericShaper</code> and shapes in the same way;
		///         <code>false</code> otherwise. </returns>
		/// <seealso cref= java.lang.Object#equals(java.lang.Object) </seealso>
		public override bool Equals(Object o)
		{
			if (o != null)
			{
				try
				{
					NumericShaper rhs = (NumericShaper)o;
					if (rangeSet != null)
					{
						if (rhs.rangeSet != null)
						{
							return Contextual == rhs.Contextual && rangeSet.Equals(rhs.rangeSet) && shapingRange == rhs.shapingRange;
						}
						return Contextual == rhs.Contextual && rangeSet.Equals(Range.maskToRangeSet(rhs.mask)) && shapingRange == Range.indexToRange(rhs.key);
					}
					else if (rhs.rangeSet != null)
					{
						Set<Range> rset = Range.maskToRangeSet(mask);
						Range srange = Range.indexToRange(key);
						return Contextual == rhs.Contextual && rset.Equals(rhs.rangeSet) && srange == rhs.shapingRange;
					}
					return rhs.mask == mask && rhs.key == key;
				}
				catch (ClassCastException)
				{
				}
			}
			return false;
		}

		/// <summary>
		/// Returns a <code>String</code> that describes this shaper. This method
		/// is used for debugging purposes only. </summary>
		/// <returns> a <code>String</code> describing this shaper. </returns>
		public override String ToString()
		{
			StringBuilder buf = new StringBuilder(base.ToString());

			buf.Append("[contextual:").Append(Contextual);

			String[] keyNames = null;
			if (Contextual)
			{
				buf.Append(", context:");
				buf.Append(shapingRange == null ? Range.values()[key] : shapingRange);
			}

			if (rangeSet == null)
			{
				buf.Append(", range(s): ");
				bool first = true;
				for (int i = 0; i < NUM_KEYS; ++i)
				{
					if ((mask & (1 << i)) != 0)
					{
						if (first)
						{
							first = false;
						}
						else
						{
							buf.Append(", ");
						}
						buf.Append(Range.values()[i]);
					}
				}
			}
			else
			{
				buf.Append(", range set: ").Append(rangeSet);
			}
			buf.Append(']');

			return buf.ToString();
		}

		/// <summary>
		/// Returns the index of the high bit in value (assuming le, actually
		/// power of 2 >= value). value must be positive.
		/// </summary>
		private static int GetHighBit(int value)
		{
			if (value <= 0)
			{
				return -32;
			}

			int bit = 0;

			if (value >= 1 << 16)
			{
				value >>= 16;
				bit += 16;
			}

			if (value >= 1 << 8)
			{
				value >>= 8;
				bit += 8;
			}

			if (value >= 1 << 4)
			{
				value >>= 4;
				bit += 4;
			}

			if (value >= 1 << 2)
			{
				value >>= 2;
				bit += 2;
			}

			if (value >= 1 << 1)
			{
				bit += 1;
			}

			return bit;
		}

		/// <summary>
		/// fast binary search over subrange of array.
		/// </summary>
		private static int Search(int value, int[] array, int start, int length)
		{
			int power = 1 << getHighBit(length);
			int extra = length - power;
			int probe = power;
			int index = start;

			if (value >= array[index + extra])
			{
				index += extra;
			}

			while (probe > 1)
			{
				probe >>= 1;

				if (value >= array[index + probe])
				{
					index += probe;
				}
			}

			return index;
		}

		/// <summary>
		/// Converts the {@code NumericShaper.Range} enum-based parameters,
		/// if any, to the bit mask-based counterparts and writes this
		/// object to the {@code stream}. Any enum constants that have no
		/// bit mask-based counterparts are ignored in the conversion.
		/// </summary>
		/// <param name="stream"> the output stream to write to </param>
		/// <exception cref="IOException"> if an I/O error occurs while writing to {@code stream}
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream stream) throws java.io.IOException
		private void WriteObject(ObjectOutputStream stream)
		{
			if (shapingRange != null)
			{
				int index = Range.toRangeIndex(shapingRange);
				if (index >= 0)
				{
					key = index;
				}
			}
			if (rangeSet != null)
			{
				mask |= Range.toRangeMask(rangeSet);
			}
			stream.DefaultWriteObject();
		}
	}

}