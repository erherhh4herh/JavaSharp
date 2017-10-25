/*
 * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.im
{


	/// <summary>
	/// Defines additional Unicode subsets for use by input methods.  Unlike the
	/// UnicodeBlock subsets defined in the <code>{@link
	/// java.lang.Character.UnicodeBlock}</code> class, these constants do not
	/// directly correspond to Unicode code blocks.
	/// 
	/// @since   1.2
	/// </summary>

	public sealed class InputSubset : Character.Subset
	{

		private InputSubset(String name) : base(name)
		{
		}

		/// <summary>
		/// Constant for all Latin characters, including the characters
		/// in the BASIC_LATIN, LATIN_1_SUPPLEMENT, LATIN_EXTENDED_A,
		/// LATIN_EXTENDED_B Unicode character blocks.
		/// </summary>
		public static readonly InputSubset LATIN = new InputSubset("LATIN");

		/// <summary>
		/// Constant for the digits included in the BASIC_LATIN Unicode character
		/// block.
		/// </summary>
		public static readonly InputSubset LATIN_DIGITS = new InputSubset("LATIN_DIGITS");

		/// <summary>
		/// Constant for all Han characters used in writing Traditional Chinese,
		/// including a subset of the CJK unified ideographs as well as Traditional
		/// Chinese Han characters that may be defined as surrogate characters.
		/// </summary>
		public static readonly InputSubset TRADITIONAL_HANZI = new InputSubset("TRADITIONAL_HANZI");

		/// <summary>
		/// Constant for all Han characters used in writing Simplified Chinese,
		/// including a subset of the CJK unified ideographs as well as Simplified
		/// Chinese Han characters that may be defined as surrogate characters.
		/// </summary>
		public static readonly InputSubset SIMPLIFIED_HANZI = new InputSubset("SIMPLIFIED_HANZI");

		/// <summary>
		/// Constant for all Han characters used in writing Japanese, including a
		/// subset of the CJK unified ideographs as well as Japanese Han characters
		/// that may be defined as surrogate characters.
		/// </summary>
		public static readonly InputSubset KANJI = new InputSubset("KANJI");

		/// <summary>
		/// Constant for all Han characters used in writing Korean, including a
		/// subset of the CJK unified ideographs as well as Korean Han characters
		/// that may be defined as surrogate characters.
		/// </summary>
		public static readonly InputSubset HANJA = new InputSubset("HANJA");

		/// <summary>
		/// Constant for the halfwidth katakana subset of the Unicode halfwidth and
		/// fullwidth forms character block.
		/// </summary>
		public static readonly InputSubset HALFWIDTH_KATAKANA = new InputSubset("HALFWIDTH_KATAKANA");

		/// <summary>
		/// Constant for the fullwidth ASCII variants subset of the Unicode halfwidth and
		/// fullwidth forms character block.
		/// @since 1.3
		/// </summary>
		public static readonly InputSubset FULLWIDTH_LATIN = new InputSubset("FULLWIDTH_LATIN");

		/// <summary>
		/// Constant for the fullwidth digits included in the Unicode halfwidth and
		/// fullwidth forms character block.
		/// @since 1.3
		/// </summary>
		public static readonly InputSubset FULLWIDTH_DIGITS = new InputSubset("FULLWIDTH_DIGITS");

	}

}