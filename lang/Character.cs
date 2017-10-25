using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// The {@code Character} class wraps a value of the primitive
	/// type {@code char} in an object. An object of type
	/// {@code Character} contains a single field whose type is
	/// {@code char}.
	/// <para>
	/// In addition, this class provides several methods for determining
	/// a character's category (lowercase letter, digit, etc.) and for converting
	/// characters from uppercase to lowercase and vice versa.
	/// </para>
	/// <para>
	/// Character information is based on the Unicode Standard, version 6.2.0.
	/// </para>
	/// <para>
	/// The methods and data of class {@code Character} are defined by
	/// the information in the <i>UnicodeData</i> file that is part of the
	/// Unicode Character Database maintained by the Unicode
	/// Consortium. This file specifies various properties including name
	/// and general category for every defined Unicode code point or
	/// character range.
	/// </para>
	/// <para>
	/// The file and its description are available from the Unicode Consortium at:
	/// <ul>
	/// <li><a href="http://www.unicode.org">http://www.unicode.org</a>
	/// </ul>
	/// 
	/// <h3><a name="unicode">Unicode Character Representations</a></h3>
	/// 
	/// </para>
	/// <para>The {@code char} data type (and therefore the value that a
	/// {@code Character} object encapsulates) are based on the
	/// original Unicode specification, which defined characters as
	/// fixed-width 16-bit entities. The Unicode Standard has since been
	/// changed to allow for characters whose representation requires more
	/// than 16 bits.  The range of legal <em>code point</em>s is now
	/// U+0000 to U+10FFFF, known as <em>Unicode scalar value</em>.
	/// (Refer to the <a
	/// href="http://www.unicode.org/reports/tr27/#notation"><i>
	/// definition</i></a> of the U+<i>n</i> notation in the Unicode
	/// Standard.)
	/// 
	/// </para>
	/// <para><a name="BMP">The set of characters from U+0000 to U+FFFF</a> is
	/// sometimes referred to as the <em>Basic Multilingual Plane (BMP)</em>.
	/// <a name="supplementary">Characters</a> whose code points are greater
	/// than U+FFFF are called <em>supplementary character</em>s.  The Java
	/// platform uses the UTF-16 representation in {@code char} arrays and
	/// in the {@code String} and {@code StringBuffer} classes. In
	/// this representation, supplementary characters are represented as a pair
	/// of {@code char} values, the first from the <em>high-surrogates</em>
	/// range, (&#92;uD800-&#92;uDBFF), the second from the
	/// <em>low-surrogates</em> range (&#92;uDC00-&#92;uDFFF).
	/// 
	/// </para>
	/// <para>A {@code char} value, therefore, represents Basic
	/// Multilingual Plane (BMP) code points, including the surrogate
	/// code points, or code units of the UTF-16 encoding. An
	/// {@code int} value represents all Unicode code points,
	/// including supplementary code points. The lower (least significant)
	/// 21 bits of {@code int} are used to represent Unicode code
	/// points and the upper (most significant) 11 bits must be zero.
	/// Unless otherwise specified, the behavior with respect to
	/// supplementary characters and surrogate {@code char} values is
	/// as follows:
	/// 
	/// <ul>
	/// <li>The methods that only accept a {@code char} value cannot support
	/// supplementary characters. They treat {@code char} values from the
	/// surrogate ranges as undefined characters. For example,
	/// {@code Character.isLetter('\u005CuD840')} returns {@code false}, even though
	/// this specific value if followed by any low-surrogate value in a string
	/// would represent a letter.
	/// 
	/// <li>The methods that accept an {@code int} value support all
	/// Unicode characters, including supplementary characters. For
	/// example, {@code Character.isLetter(0x2F81A)} returns
	/// {@code true} because the code point value represents a letter
	/// (a CJK ideograph).
	/// </ul>
	/// 
	/// </para>
	/// <para>In the Java SE API documentation, <em>Unicode code point</em> is
	/// used for character values in the range between U+0000 and U+10FFFF,
	/// and <em>Unicode code unit</em> is used for 16-bit
	/// {@code char} values that are code units of the <em>UTF-16</em>
	/// encoding. For more information on Unicode terminology, refer to the
	/// <a href="http://www.unicode.org/glossary/">Unicode Glossary</a>.
	/// 
	/// @author  Lee Boynton
	/// @author  Guy Steele
	/// @author  Akira Tanaka
	/// @author  Martin Buchholz
	/// @author  Ulf Zibis
	/// @since   1.0
	/// </para>
	/// </summary>
	[Serializable]
	public sealed class Character : Comparable<Character>
	{
		/// <summary>
		/// The minimum radix available for conversion to and from strings.
		/// The constant value of this field is the smallest value permitted
		/// for the radix argument in radix-conversion methods such as the
		/// {@code digit} method, the {@code forDigit} method, and the
		/// {@code toString} method of class {@code Integer}.
		/// </summary>
		/// <seealso cref=     Character#digit(char, int) </seealso>
		/// <seealso cref=     Character#forDigit(int, int) </seealso>
		/// <seealso cref=     Integer#toString(int, int) </seealso>
		/// <seealso cref=     Integer#valueOf(String) </seealso>
		public const int MIN_RADIX = 2;

		/// <summary>
		/// The maximum radix available for conversion to and from strings.
		/// The constant value of this field is the largest value permitted
		/// for the radix argument in radix-conversion methods such as the
		/// {@code digit} method, the {@code forDigit} method, and the
		/// {@code toString} method of class {@code Integer}.
		/// </summary>
		/// <seealso cref=     Character#digit(char, int) </seealso>
		/// <seealso cref=     Character#forDigit(int, int) </seealso>
		/// <seealso cref=     Integer#toString(int, int) </seealso>
		/// <seealso cref=     Integer#valueOf(String) </seealso>
		public const int MAX_RADIX = 36;

		/// <summary>
		/// The constant value of this field is the smallest value of type
		/// {@code char}, {@code '\u005Cu0000'}.
		/// 
		/// @since   1.0.2
		/// </summary>
		public const char MIN_VALUE = '\u0000';

		/// <summary>
		/// The constant value of this field is the largest value of type
		/// {@code char}, {@code '\u005CuFFFF'}.
		/// 
		/// @since   1.0.2
		/// </summary>
		public const char MAX_VALUE = '\uFFFF';

		/// <summary>
		/// The {@code Class} instance representing the primitive type
		/// {@code char}.
		/// 
		/// @since   1.1
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static final Class TYPE = (Class) Class.getPrimitiveClass("char");
		public static readonly Class TYPE = (Class) Class.getPrimitiveClass("char");

		/*
		 * Normative general types
		 */

		/*
		 * General character types
		 */

		/// <summary>
		/// General category "Cn" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte UNASSIGNED = 0;

		/// <summary>
		/// General category "Lu" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte UPPERCASE_LETTER = 1;

		/// <summary>
		/// General category "Ll" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte LOWERCASE_LETTER = 2;

		/// <summary>
		/// General category "Lt" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte TITLECASE_LETTER = 3;

		/// <summary>
		/// General category "Lm" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte MODIFIER_LETTER = 4;

		/// <summary>
		/// General category "Lo" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte OTHER_LETTER = 5;

		/// <summary>
		/// General category "Mn" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte NON_SPACING_MARK = 6;

		/// <summary>
		/// General category "Me" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte ENCLOSING_MARK = 7;

		/// <summary>
		/// General category "Mc" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte COMBINING_SPACING_MARK = 8;

		/// <summary>
		/// General category "Nd" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte DECIMAL_DIGIT_NUMBER = 9;

		/// <summary>
		/// General category "Nl" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte LETTER_NUMBER = 10;

		/// <summary>
		/// General category "No" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte OTHER_NUMBER = 11;

		/// <summary>
		/// General category "Zs" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte SPACE_SEPARATOR = 12;

		/// <summary>
		/// General category "Zl" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte LINE_SEPARATOR = 13;

		/// <summary>
		/// General category "Zp" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte PARAGRAPH_SEPARATOR = 14;

		/// <summary>
		/// General category "Cc" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte CONTROL = 15;

		/// <summary>
		/// General category "Cf" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte FORMAT = 16;

		/// <summary>
		/// General category "Co" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte PRIVATE_USE = 18;

		/// <summary>
		/// General category "Cs" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte SURROGATE = 19;

		/// <summary>
		/// General category "Pd" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte DASH_PUNCTUATION = 20;

		/// <summary>
		/// General category "Ps" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte START_PUNCTUATION = 21;

		/// <summary>
		/// General category "Pe" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte END_PUNCTUATION = 22;

		/// <summary>
		/// General category "Pc" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte CONNECTOR_PUNCTUATION = 23;

		/// <summary>
		/// General category "Po" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte OTHER_PUNCTUATION = 24;

		/// <summary>
		/// General category "Sm" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte MATH_SYMBOL = 25;

		/// <summary>
		/// General category "Sc" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte CURRENCY_SYMBOL = 26;

		/// <summary>
		/// General category "Sk" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte MODIFIER_SYMBOL = 27;

		/// <summary>
		/// General category "So" in the Unicode specification.
		/// @since   1.1
		/// </summary>
		public const sbyte OTHER_SYMBOL = 28;

		/// <summary>
		/// General category "Pi" in the Unicode specification.
		/// @since   1.4
		/// </summary>
		public const sbyte INITIAL_QUOTE_PUNCTUATION = 29;

		/// <summary>
		/// General category "Pf" in the Unicode specification.
		/// @since   1.4
		/// </summary>
		public const sbyte FINAL_QUOTE_PUNCTUATION = 30;

		/// <summary>
		/// Error flag. Use int (code point) to avoid confusion with U+FFFF.
		/// </summary>
		internal const int ERROR = unchecked((int)0xFFFFFFFF);


		/// <summary>
		/// Undefined bidirectional character type. Undefined {@code char}
		/// values have undefined directionality in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_UNDEFINED = -1;

		/// <summary>
		/// Strong bidirectional character type "L" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_LEFT_TO_RIGHT = 0;

		/// <summary>
		/// Strong bidirectional character type "R" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_RIGHT_TO_LEFT = 1;

		/// <summary>
		/// Strong bidirectional character type "AL" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC = 2;

		/// <summary>
		/// Weak bidirectional character type "EN" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_EUROPEAN_NUMBER = 3;

		/// <summary>
		/// Weak bidirectional character type "ES" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR = 4;

		/// <summary>
		/// Weak bidirectional character type "ET" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR = 5;

		/// <summary>
		/// Weak bidirectional character type "AN" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_ARABIC_NUMBER = 6;

		/// <summary>
		/// Weak bidirectional character type "CS" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_COMMON_NUMBER_SEPARATOR = 7;

		/// <summary>
		/// Weak bidirectional character type "NSM" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_NONSPACING_MARK = 8;

		/// <summary>
		/// Weak bidirectional character type "BN" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_BOUNDARY_NEUTRAL = 9;

		/// <summary>
		/// Neutral bidirectional character type "B" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_PARAGRAPH_SEPARATOR = 10;

		/// <summary>
		/// Neutral bidirectional character type "S" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_SEGMENT_SEPARATOR = 11;

		/// <summary>
		/// Neutral bidirectional character type "WS" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_WHITESPACE = 12;

		/// <summary>
		/// Neutral bidirectional character type "ON" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_OTHER_NEUTRALS = 13;

		/// <summary>
		/// Strong bidirectional character type "LRE" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING = 14;

		/// <summary>
		/// Strong bidirectional character type "LRO" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE = 15;

		/// <summary>
		/// Strong bidirectional character type "RLE" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING = 16;

		/// <summary>
		/// Strong bidirectional character type "RLO" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE = 17;

		/// <summary>
		/// Weak bidirectional character type "PDF" in the Unicode specification.
		/// @since 1.4
		/// </summary>
		public const sbyte DIRECTIONALITY_POP_DIRECTIONAL_FORMAT = 18;

		/// <summary>
		/// The minimum value of a
		/// <a href="http://www.unicode.org/glossary/#high_surrogate_code_unit">
		/// Unicode high-surrogate code unit</a>
		/// in the UTF-16 encoding, constant {@code '\u005CuD800'}.
		/// A high-surrogate is also known as a <i>leading-surrogate</i>.
		/// 
		/// @since 1.5
		/// </summary>
		public const char MIN_HIGH_SURROGATE = '\uD800';

		/// <summary>
		/// The maximum value of a
		/// <a href="http://www.unicode.org/glossary/#high_surrogate_code_unit">
		/// Unicode high-surrogate code unit</a>
		/// in the UTF-16 encoding, constant {@code '\u005CuDBFF'}.
		/// A high-surrogate is also known as a <i>leading-surrogate</i>.
		/// 
		/// @since 1.5
		/// </summary>
		public const char MAX_HIGH_SURROGATE = '\uDBFF';

		/// <summary>
		/// The minimum value of a
		/// <a href="http://www.unicode.org/glossary/#low_surrogate_code_unit">
		/// Unicode low-surrogate code unit</a>
		/// in the UTF-16 encoding, constant {@code '\u005CuDC00'}.
		/// A low-surrogate is also known as a <i>trailing-surrogate</i>.
		/// 
		/// @since 1.5
		/// </summary>
		public const char MIN_LOW_SURROGATE = '\uDC00';

		/// <summary>
		/// The maximum value of a
		/// <a href="http://www.unicode.org/glossary/#low_surrogate_code_unit">
		/// Unicode low-surrogate code unit</a>
		/// in the UTF-16 encoding, constant {@code '\u005CuDFFF'}.
		/// A low-surrogate is also known as a <i>trailing-surrogate</i>.
		/// 
		/// @since 1.5
		/// </summary>
		public const char MAX_LOW_SURROGATE = '\uDFFF';

		/// <summary>
		/// The minimum value of a Unicode surrogate code unit in the
		/// UTF-16 encoding, constant {@code '\u005CuD800'}.
		/// 
		/// @since 1.5
		/// </summary>
		public const char MIN_SURROGATE = MIN_HIGH_SURROGATE;

		/// <summary>
		/// The maximum value of a Unicode surrogate code unit in the
		/// UTF-16 encoding, constant {@code '\u005CuDFFF'}.
		/// 
		/// @since 1.5
		/// </summary>
		public const char MAX_SURROGATE = MAX_LOW_SURROGATE;

		/// <summary>
		/// The minimum value of a
		/// <a href="http://www.unicode.org/glossary/#supplementary_code_point">
		/// Unicode supplementary code point</a>, constant {@code U+10000}.
		/// 
		/// @since 1.5
		/// </summary>
		public const int MIN_SUPPLEMENTARY_CODE_POINT = 0x010000;

		/// <summary>
		/// The minimum value of a
		/// <a href="http://www.unicode.org/glossary/#code_point">
		/// Unicode code point</a>, constant {@code U+0000}.
		/// 
		/// @since 1.5
		/// </summary>
		public const int MIN_CODE_POINT = 0x000000;

		/// <summary>
		/// The maximum value of a
		/// <a href="http://www.unicode.org/glossary/#code_point">
		/// Unicode code point</a>, constant {@code U+10FFFF}.
		/// 
		/// @since 1.5
		/// </summary>
		public const int MAX_CODE_POINT = 0X10FFFF;


		/// <summary>
		/// Instances of this class represent particular subsets of the Unicode
		/// character set.  The only family of subsets defined in the
		/// {@code Character} class is <seealso cref="Character.UnicodeBlock"/>.
		/// Other portions of the Java API may define other subsets for their
		/// own purposes.
		/// 
		/// @since 1.2
		/// </summary>
		public class Subset
		{

			internal String Name;

			/// <summary>
			/// Constructs a new {@code Subset} instance.
			/// </summary>
			/// <param name="name">  The name of this subset </param>
			/// <exception cref="NullPointerException"> if name is {@code null} </exception>
			protected internal Subset(String name)
			{
				if (name == null)
				{
					throw new NullPointerException("name");
				}
				this.Name = name;
			}

			/// <summary>
			/// Compares two {@code Subset} objects for equality.
			/// This method returns {@code true} if and only if
			/// {@code this} and the argument refer to the same
			/// object; since this method is {@code final}, this
			/// guarantee holds for all subclasses.
			/// </summary>
			public sealed override bool Equals(Object obj)
			{
				return (this == obj);
			}

			/// <summary>
			/// Returns the standard hash code as defined by the
			/// <seealso cref="Object#hashCode"/> method.  This method
			/// is {@code final} in order to ensure that the
			/// {@code equals} and {@code hashCode} methods will
			/// be consistent in all subclasses.
			/// </summary>
			public sealed override int HashCode()
			{
				return base.HashCode();
			}

			/// <summary>
			/// Returns the name of this subset.
			/// </summary>
			public sealed override String ToString()
			{
				return Name;
			}
		}

		// See http://www.unicode.org/Public/UNIDATA/Blocks.txt
		// for the latest specification of Unicode Blocks.

		/// <summary>
		/// A family of character subsets representing the character blocks in the
		/// Unicode specification. Character blocks generally define characters
		/// used for a specific script or purpose. A character is contained by
		/// at most one Unicode block.
		/// 
		/// @since 1.2
		/// </summary>
		public sealed class UnicodeBlock : Subset
		{

			internal static IDictionary<String, UnicodeBlock> Map = new Dictionary<String, UnicodeBlock>(256);

			/// <summary>
			/// Creates a UnicodeBlock with the given identifier name.
			/// This name must be the same as the block identifier.
			/// </summary>
			internal UnicodeBlock(String idName) : base(idName)
			{
				Map[idName] = this;
			}

			/// <summary>
			/// Creates a UnicodeBlock with the given identifier name and
			/// alias name.
			/// </summary>
			internal UnicodeBlock(String idName, String alias) : this(idName)
			{
				Map[alias] = this;
			}

			/// <summary>
			/// Creates a UnicodeBlock with the given identifier name and
			/// alias names.
			/// </summary>
			internal UnicodeBlock(String idName, params String[] aliases) : this(idName)
			{
				foreach (String alias in aliases)
				{
					Map[alias] = this;
				}
			}

			/// <summary>
			/// Constant for the "Basic Latin" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock BASIC_LATIN = new UnicodeBlock("BASIC_LATIN", "BASIC LATIN", "BASICLATIN");

			/// <summary>
			/// Constant for the "Latin-1 Supplement" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock LATIN_1_SUPPLEMENT = new UnicodeBlock("LATIN_1_SUPPLEMENT", "LATIN-1 SUPPLEMENT", "LATIN-1SUPPLEMENT");

			/// <summary>
			/// Constant for the "Latin Extended-A" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock LATIN_EXTENDED_A = new UnicodeBlock("LATIN_EXTENDED_A", "LATIN EXTENDED-A", "LATINEXTENDED-A");

			/// <summary>
			/// Constant for the "Latin Extended-B" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock LATIN_EXTENDED_B = new UnicodeBlock("LATIN_EXTENDED_B", "LATIN EXTENDED-B", "LATINEXTENDED-B");

			/// <summary>
			/// Constant for the "IPA Extensions" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock IPA_EXTENSIONS = new UnicodeBlock("IPA_EXTENSIONS", "IPA EXTENSIONS", "IPAEXTENSIONS");

			/// <summary>
			/// Constant for the "Spacing Modifier Letters" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock SPACING_MODIFIER_LETTERS = new UnicodeBlock("SPACING_MODIFIER_LETTERS", "SPACING MODIFIER LETTERS", "SPACINGMODIFIERLETTERS");

			/// <summary>
			/// Constant for the "Combining Diacritical Marks" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock COMBINING_DIACRITICAL_MARKS = new UnicodeBlock("COMBINING_DIACRITICAL_MARKS", "COMBINING DIACRITICAL MARKS", "COMBININGDIACRITICALMARKS");

			/// <summary>
			/// Constant for the "Greek and Coptic" Unicode character block.
			/// <para>
			/// This block was previously known as the "Greek" block.
			/// 
			/// @since 1.2
			/// </para>
			/// </summary>
			public static readonly UnicodeBlock GREEK = new UnicodeBlock("GREEK", "GREEK AND COPTIC", "GREEKANDCOPTIC");

			/// <summary>
			/// Constant for the "Cyrillic" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock CYRILLIC = new UnicodeBlock("CYRILLIC");

			/// <summary>
			/// Constant for the "Armenian" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock ARMENIAN = new UnicodeBlock("ARMENIAN");

			/// <summary>
			/// Constant for the "Hebrew" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock HEBREW = new UnicodeBlock("HEBREW");

			/// <summary>
			/// Constant for the "Arabic" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock ARABIC = new UnicodeBlock("ARABIC");

			/// <summary>
			/// Constant for the "Devanagari" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock DEVANAGARI = new UnicodeBlock("DEVANAGARI");

			/// <summary>
			/// Constant for the "Bengali" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock BENGALI = new UnicodeBlock("BENGALI");

			/// <summary>
			/// Constant for the "Gurmukhi" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock GURMUKHI = new UnicodeBlock("GURMUKHI");

			/// <summary>
			/// Constant for the "Gujarati" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock GUJARATI = new UnicodeBlock("GUJARATI");

			/// <summary>
			/// Constant for the "Oriya" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock ORIYA = new UnicodeBlock("ORIYA");

			/// <summary>
			/// Constant for the "Tamil" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock TAMIL = new UnicodeBlock("TAMIL");

			/// <summary>
			/// Constant for the "Telugu" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock TELUGU = new UnicodeBlock("TELUGU");

			/// <summary>
			/// Constant for the "Kannada" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock KANNADA = new UnicodeBlock("KANNADA");

			/// <summary>
			/// Constant for the "Malayalam" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock MALAYALAM = new UnicodeBlock("MALAYALAM");

			/// <summary>
			/// Constant for the "Thai" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock THAI = new UnicodeBlock("THAI");

			/// <summary>
			/// Constant for the "Lao" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock LAO = new UnicodeBlock("LAO");

			/// <summary>
			/// Constant for the "Tibetan" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock TIBETAN = new UnicodeBlock("TIBETAN");

			/// <summary>
			/// Constant for the "Georgian" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock GEORGIAN = new UnicodeBlock("GEORGIAN");

			/// <summary>
			/// Constant for the "Hangul Jamo" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock HANGUL_JAMO = new UnicodeBlock("HANGUL_JAMO", "HANGUL JAMO", "HANGULJAMO");

			/// <summary>
			/// Constant for the "Latin Extended Additional" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock LATIN_EXTENDED_ADDITIONAL = new UnicodeBlock("LATIN_EXTENDED_ADDITIONAL", "LATIN EXTENDED ADDITIONAL", "LATINEXTENDEDADDITIONAL");

			/// <summary>
			/// Constant for the "Greek Extended" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock GREEK_EXTENDED = new UnicodeBlock("GREEK_EXTENDED", "GREEK EXTENDED", "GREEKEXTENDED");

			/// <summary>
			/// Constant for the "General Punctuation" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock GENERAL_PUNCTUATION = new UnicodeBlock("GENERAL_PUNCTUATION", "GENERAL PUNCTUATION", "GENERALPUNCTUATION");

			/// <summary>
			/// Constant for the "Superscripts and Subscripts" Unicode character
			/// block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock SUPERSCRIPTS_AND_SUBSCRIPTS = new UnicodeBlock("SUPERSCRIPTS_AND_SUBSCRIPTS", "SUPERSCRIPTS AND SUBSCRIPTS", "SUPERSCRIPTSANDSUBSCRIPTS");

			/// <summary>
			/// Constant for the "Currency Symbols" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock CURRENCY_SYMBOLS = new UnicodeBlock("CURRENCY_SYMBOLS", "CURRENCY SYMBOLS", "CURRENCYSYMBOLS");

			/// <summary>
			/// Constant for the "Combining Diacritical Marks for Symbols" Unicode
			/// character block.
			/// <para>
			/// This block was previously known as "Combining Marks for Symbols".
			/// @since 1.2
			/// </para>
			/// </summary>
			public static readonly UnicodeBlock COMBINING_MARKS_FOR_SYMBOLS = new UnicodeBlock("COMBINING_MARKS_FOR_SYMBOLS", "COMBINING DIACRITICAL MARKS FOR SYMBOLS", "COMBININGDIACRITICALMARKSFORSYMBOLS", "COMBINING MARKS FOR SYMBOLS", "COMBININGMARKSFORSYMBOLS");

			/// <summary>
			/// Constant for the "Letterlike Symbols" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock LETTERLIKE_SYMBOLS = new UnicodeBlock("LETTERLIKE_SYMBOLS", "LETTERLIKE SYMBOLS", "LETTERLIKESYMBOLS");

			/// <summary>
			/// Constant for the "Number Forms" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock NUMBER_FORMS = new UnicodeBlock("NUMBER_FORMS", "NUMBER FORMS", "NUMBERFORMS");

			/// <summary>
			/// Constant for the "Arrows" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock ARROWS = new UnicodeBlock("ARROWS");

			/// <summary>
			/// Constant for the "Mathematical Operators" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock MATHEMATICAL_OPERATORS = new UnicodeBlock("MATHEMATICAL_OPERATORS", "MATHEMATICAL OPERATORS", "MATHEMATICALOPERATORS");

			/// <summary>
			/// Constant for the "Miscellaneous Technical" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock MISCELLANEOUS_TECHNICAL = new UnicodeBlock("MISCELLANEOUS_TECHNICAL", "MISCELLANEOUS TECHNICAL", "MISCELLANEOUSTECHNICAL");

			/// <summary>
			/// Constant for the "Control Pictures" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock CONTROL_PICTURES = new UnicodeBlock("CONTROL_PICTURES", "CONTROL PICTURES", "CONTROLPICTURES");

			/// <summary>
			/// Constant for the "Optical Character Recognition" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock OPTICAL_CHARACTER_RECOGNITION = new UnicodeBlock("OPTICAL_CHARACTER_RECOGNITION", "OPTICAL CHARACTER RECOGNITION", "OPTICALCHARACTERRECOGNITION");

			/// <summary>
			/// Constant for the "Enclosed Alphanumerics" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock ENCLOSED_ALPHANUMERICS = new UnicodeBlock("ENCLOSED_ALPHANUMERICS", "ENCLOSED ALPHANUMERICS", "ENCLOSEDALPHANUMERICS");

			/// <summary>
			/// Constant for the "Box Drawing" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock BOX_DRAWING = new UnicodeBlock("BOX_DRAWING", "BOX DRAWING", "BOXDRAWING");

			/// <summary>
			/// Constant for the "Block Elements" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock BLOCK_ELEMENTS = new UnicodeBlock("BLOCK_ELEMENTS", "BLOCK ELEMENTS", "BLOCKELEMENTS");

			/// <summary>
			/// Constant for the "Geometric Shapes" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock GEOMETRIC_SHAPES = new UnicodeBlock("GEOMETRIC_SHAPES", "GEOMETRIC SHAPES", "GEOMETRICSHAPES");

			/// <summary>
			/// Constant for the "Miscellaneous Symbols" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock MISCELLANEOUS_SYMBOLS = new UnicodeBlock("MISCELLANEOUS_SYMBOLS", "MISCELLANEOUS SYMBOLS", "MISCELLANEOUSSYMBOLS");

			/// <summary>
			/// Constant for the "Dingbats" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock DINGBATS = new UnicodeBlock("DINGBATS");

			/// <summary>
			/// Constant for the "CJK Symbols and Punctuation" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock CJK_SYMBOLS_AND_PUNCTUATION = new UnicodeBlock("CJK_SYMBOLS_AND_PUNCTUATION", "CJK SYMBOLS AND PUNCTUATION", "CJKSYMBOLSANDPUNCTUATION");

			/// <summary>
			/// Constant for the "Hiragana" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock HIRAGANA = new UnicodeBlock("HIRAGANA");

			/// <summary>
			/// Constant for the "Katakana" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock KATAKANA = new UnicodeBlock("KATAKANA");

			/// <summary>
			/// Constant for the "Bopomofo" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock BOPOMOFO = new UnicodeBlock("BOPOMOFO");

			/// <summary>
			/// Constant for the "Hangul Compatibility Jamo" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock HANGUL_COMPATIBILITY_JAMO = new UnicodeBlock("HANGUL_COMPATIBILITY_JAMO", "HANGUL COMPATIBILITY JAMO", "HANGULCOMPATIBILITYJAMO");

			/// <summary>
			/// Constant for the "Kanbun" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock KANBUN = new UnicodeBlock("KANBUN");

			/// <summary>
			/// Constant for the "Enclosed CJK Letters and Months" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock ENCLOSED_CJK_LETTERS_AND_MONTHS = new UnicodeBlock("ENCLOSED_CJK_LETTERS_AND_MONTHS", "ENCLOSED CJK LETTERS AND MONTHS", "ENCLOSEDCJKLETTERSANDMONTHS");

			/// <summary>
			/// Constant for the "CJK Compatibility" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock CJK_COMPATIBILITY = new UnicodeBlock("CJK_COMPATIBILITY", "CJK COMPATIBILITY", "CJKCOMPATIBILITY");

			/// <summary>
			/// Constant for the "CJK Unified Ideographs" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock CJK_UNIFIED_IDEOGRAPHS = new UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS", "CJK UNIFIED IDEOGRAPHS", "CJKUNIFIEDIDEOGRAPHS");

			/// <summary>
			/// Constant for the "Hangul Syllables" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock HANGUL_SYLLABLES = new UnicodeBlock("HANGUL_SYLLABLES", "HANGUL SYLLABLES", "HANGULSYLLABLES");

			/// <summary>
			/// Constant for the "Private Use Area" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock PRIVATE_USE_AREA = new UnicodeBlock("PRIVATE_USE_AREA", "PRIVATE USE AREA", "PRIVATEUSEAREA");

			/// <summary>
			/// Constant for the "CJK Compatibility Ideographs" Unicode character
			/// block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock CJK_COMPATIBILITY_IDEOGRAPHS = new UnicodeBlock("CJK_COMPATIBILITY_IDEOGRAPHS", "CJK COMPATIBILITY IDEOGRAPHS", "CJKCOMPATIBILITYIDEOGRAPHS");

			/// <summary>
			/// Constant for the "Alphabetic Presentation Forms" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock ALPHABETIC_PRESENTATION_FORMS = new UnicodeBlock("ALPHABETIC_PRESENTATION_FORMS", "ALPHABETIC PRESENTATION FORMS", "ALPHABETICPRESENTATIONFORMS");

			/// <summary>
			/// Constant for the "Arabic Presentation Forms-A" Unicode character
			/// block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock ARABIC_PRESENTATION_FORMS_A = new UnicodeBlock("ARABIC_PRESENTATION_FORMS_A", "ARABIC PRESENTATION FORMS-A", "ARABICPRESENTATIONFORMS-A");

			/// <summary>
			/// Constant for the "Combining Half Marks" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock COMBINING_HALF_MARKS = new UnicodeBlock("COMBINING_HALF_MARKS", "COMBINING HALF MARKS", "COMBININGHALFMARKS");

			/// <summary>
			/// Constant for the "CJK Compatibility Forms" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock CJK_COMPATIBILITY_FORMS = new UnicodeBlock("CJK_COMPATIBILITY_FORMS", "CJK COMPATIBILITY FORMS", "CJKCOMPATIBILITYFORMS");

			/// <summary>
			/// Constant for the "Small Form Variants" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock SMALL_FORM_VARIANTS = new UnicodeBlock("SMALL_FORM_VARIANTS", "SMALL FORM VARIANTS", "SMALLFORMVARIANTS");

			/// <summary>
			/// Constant for the "Arabic Presentation Forms-B" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock ARABIC_PRESENTATION_FORMS_B = new UnicodeBlock("ARABIC_PRESENTATION_FORMS_B", "ARABIC PRESENTATION FORMS-B", "ARABICPRESENTATIONFORMS-B");

			/// <summary>
			/// Constant for the "Halfwidth and Fullwidth Forms" Unicode character
			/// block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock HALFWIDTH_AND_FULLWIDTH_FORMS = new UnicodeBlock("HALFWIDTH_AND_FULLWIDTH_FORMS", "HALFWIDTH AND FULLWIDTH FORMS", "HALFWIDTHANDFULLWIDTHFORMS");

			/// <summary>
			/// Constant for the "Specials" Unicode character block.
			/// @since 1.2
			/// </summary>
			public static readonly UnicodeBlock SPECIALS = new UnicodeBlock("SPECIALS");

			/// @deprecated As of J2SE 5, use <seealso cref="#HIGH_SURROGATES"/>,
			///             <seealso cref="#HIGH_PRIVATE_USE_SURROGATES"/>, and
			///             <seealso cref="#LOW_SURROGATES"/>. These new constants match
			///             the block definitions of the Unicode Standard.
			///             The <seealso cref="#of(char)"/> and <seealso cref="#of(int)"/> methods
			///             return the new constants, not SURROGATES_AREA. 
			[Obsolete("As of J2SE 5, use <seealso cref="#HIGH_SURROGATES"/>,")]
			public static readonly UnicodeBlock SURROGATES_AREA = new UnicodeBlock("SURROGATES_AREA");

			/// <summary>
			/// Constant for the "Syriac" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock SYRIAC = new UnicodeBlock("SYRIAC");

			/// <summary>
			/// Constant for the "Thaana" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock THAANA = new UnicodeBlock("THAANA");

			/// <summary>
			/// Constant for the "Sinhala" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock SINHALA = new UnicodeBlock("SINHALA");

			/// <summary>
			/// Constant for the "Myanmar" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock MYANMAR = new UnicodeBlock("MYANMAR");

			/// <summary>
			/// Constant for the "Ethiopic" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock ETHIOPIC = new UnicodeBlock("ETHIOPIC");

			/// <summary>
			/// Constant for the "Cherokee" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock CHEROKEE = new UnicodeBlock("CHEROKEE");

			/// <summary>
			/// Constant for the "Unified Canadian Aboriginal Syllabics" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS = new UnicodeBlock("UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS", "UNIFIED CANADIAN ABORIGINAL SYLLABICS", "UNIFIEDCANADIANABORIGINALSYLLABICS");

			/// <summary>
			/// Constant for the "Ogham" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock OGHAM = new UnicodeBlock("OGHAM");

			/// <summary>
			/// Constant for the "Runic" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock RUNIC = new UnicodeBlock("RUNIC");

			/// <summary>
			/// Constant for the "Khmer" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock KHMER = new UnicodeBlock("KHMER");

			/// <summary>
			/// Constant for the "Mongolian" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock MONGOLIAN = new UnicodeBlock("MONGOLIAN");

			/// <summary>
			/// Constant for the "Braille Patterns" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock BRAILLE_PATTERNS = new UnicodeBlock("BRAILLE_PATTERNS", "BRAILLE PATTERNS", "BRAILLEPATTERNS");

			/// <summary>
			/// Constant for the "CJK Radicals Supplement" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock CJK_RADICALS_SUPPLEMENT = new UnicodeBlock("CJK_RADICALS_SUPPLEMENT", "CJK RADICALS SUPPLEMENT", "CJKRADICALSSUPPLEMENT");

			/// <summary>
			/// Constant for the "Kangxi Radicals" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock KANGXI_RADICALS = new UnicodeBlock("KANGXI_RADICALS", "KANGXI RADICALS", "KANGXIRADICALS");

			/// <summary>
			/// Constant for the "Ideographic Description Characters" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock IDEOGRAPHIC_DESCRIPTION_CHARACTERS = new UnicodeBlock("IDEOGRAPHIC_DESCRIPTION_CHARACTERS", "IDEOGRAPHIC DESCRIPTION CHARACTERS", "IDEOGRAPHICDESCRIPTIONCHARACTERS");

			/// <summary>
			/// Constant for the "Bopomofo Extended" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock BOPOMOFO_EXTENDED = new UnicodeBlock("BOPOMOFO_EXTENDED", "BOPOMOFO EXTENDED", "BOPOMOFOEXTENDED");

			/// <summary>
			/// Constant for the "CJK Unified Ideographs Extension A" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A = new UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A", "CJK UNIFIED IDEOGRAPHS EXTENSION A", "CJKUNIFIEDIDEOGRAPHSEXTENSIONA");

			/// <summary>
			/// Constant for the "Yi Syllables" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock YI_SYLLABLES = new UnicodeBlock("YI_SYLLABLES", "YI SYLLABLES", "YISYLLABLES");

			/// <summary>
			/// Constant for the "Yi Radicals" Unicode character block.
			/// @since 1.4
			/// </summary>
			public static readonly UnicodeBlock YI_RADICALS = new UnicodeBlock("YI_RADICALS", "YI RADICALS", "YIRADICALS");

			/// <summary>
			/// Constant for the "Cyrillic Supplementary" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock CYRILLIC_SUPPLEMENTARY = new UnicodeBlock("CYRILLIC_SUPPLEMENTARY", "CYRILLIC SUPPLEMENTARY", "CYRILLICSUPPLEMENTARY", "CYRILLIC SUPPLEMENT", "CYRILLICSUPPLEMENT");

			/// <summary>
			/// Constant for the "Tagalog" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock TAGALOG = new UnicodeBlock("TAGALOG");

			/// <summary>
			/// Constant for the "Hanunoo" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock HANUNOO = new UnicodeBlock("HANUNOO");

			/// <summary>
			/// Constant for the "Buhid" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock BUHID = new UnicodeBlock("BUHID");

			/// <summary>
			/// Constant for the "Tagbanwa" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock TAGBANWA = new UnicodeBlock("TAGBANWA");

			/// <summary>
			/// Constant for the "Limbu" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock LIMBU = new UnicodeBlock("LIMBU");

			/// <summary>
			/// Constant for the "Tai Le" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock TAI_LE = new UnicodeBlock("TAI_LE", "TAI LE", "TAILE");

			/// <summary>
			/// Constant for the "Khmer Symbols" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock KHMER_SYMBOLS = new UnicodeBlock("KHMER_SYMBOLS", "KHMER SYMBOLS", "KHMERSYMBOLS");

			/// <summary>
			/// Constant for the "Phonetic Extensions" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock PHONETIC_EXTENSIONS = new UnicodeBlock("PHONETIC_EXTENSIONS", "PHONETIC EXTENSIONS", "PHONETICEXTENSIONS");

			/// <summary>
			/// Constant for the "Miscellaneous Mathematical Symbols-A" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A = new UnicodeBlock("MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A", "MISCELLANEOUS MATHEMATICAL SYMBOLS-A", "MISCELLANEOUSMATHEMATICALSYMBOLS-A");

			/// <summary>
			/// Constant for the "Supplemental Arrows-A" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock SUPPLEMENTAL_ARROWS_A = new UnicodeBlock("SUPPLEMENTAL_ARROWS_A", "SUPPLEMENTAL ARROWS-A", "SUPPLEMENTALARROWS-A");

			/// <summary>
			/// Constant for the "Supplemental Arrows-B" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock SUPPLEMENTAL_ARROWS_B = new UnicodeBlock("SUPPLEMENTAL_ARROWS_B", "SUPPLEMENTAL ARROWS-B", "SUPPLEMENTALARROWS-B");

			/// <summary>
			/// Constant for the "Miscellaneous Mathematical Symbols-B" Unicode
			/// character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B = new UnicodeBlock("MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B", "MISCELLANEOUS MATHEMATICAL SYMBOLS-B", "MISCELLANEOUSMATHEMATICALSYMBOLS-B");

			/// <summary>
			/// Constant for the "Supplemental Mathematical Operators" Unicode
			/// character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock SUPPLEMENTAL_MATHEMATICAL_OPERATORS = new UnicodeBlock("SUPPLEMENTAL_MATHEMATICAL_OPERATORS", "SUPPLEMENTAL MATHEMATICAL OPERATORS", "SUPPLEMENTALMATHEMATICALOPERATORS");

			/// <summary>
			/// Constant for the "Miscellaneous Symbols and Arrows" Unicode character
			/// block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock MISCELLANEOUS_SYMBOLS_AND_ARROWS = new UnicodeBlock("MISCELLANEOUS_SYMBOLS_AND_ARROWS", "MISCELLANEOUS SYMBOLS AND ARROWS", "MISCELLANEOUSSYMBOLSANDARROWS");

			/// <summary>
			/// Constant for the "Katakana Phonetic Extensions" Unicode character
			/// block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock KATAKANA_PHONETIC_EXTENSIONS = new UnicodeBlock("KATAKANA_PHONETIC_EXTENSIONS", "KATAKANA PHONETIC EXTENSIONS", "KATAKANAPHONETICEXTENSIONS");

			/// <summary>
			/// Constant for the "Yijing Hexagram Symbols" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock YIJING_HEXAGRAM_SYMBOLS = new UnicodeBlock("YIJING_HEXAGRAM_SYMBOLS", "YIJING HEXAGRAM SYMBOLS", "YIJINGHEXAGRAMSYMBOLS");

			/// <summary>
			/// Constant for the "Variation Selectors" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock VARIATION_SELECTORS = new UnicodeBlock("VARIATION_SELECTORS", "VARIATION SELECTORS", "VARIATIONSELECTORS");

			/// <summary>
			/// Constant for the "Linear B Syllabary" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock LINEAR_B_SYLLABARY = new UnicodeBlock("LINEAR_B_SYLLABARY", "LINEAR B SYLLABARY", "LINEARBSYLLABARY");

			/// <summary>
			/// Constant for the "Linear B Ideograms" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock LINEAR_B_IDEOGRAMS = new UnicodeBlock("LINEAR_B_IDEOGRAMS", "LINEAR B IDEOGRAMS", "LINEARBIDEOGRAMS");

			/// <summary>
			/// Constant for the "Aegean Numbers" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock AEGEAN_NUMBERS = new UnicodeBlock("AEGEAN_NUMBERS", "AEGEAN NUMBERS", "AEGEANNUMBERS");

			/// <summary>
			/// Constant for the "Old Italic" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock OLD_ITALIC = new UnicodeBlock("OLD_ITALIC", "OLD ITALIC", "OLDITALIC");

			/// <summary>
			/// Constant for the "Gothic" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock GOTHIC = new UnicodeBlock("GOTHIC");

			/// <summary>
			/// Constant for the "Ugaritic" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock UGARITIC = new UnicodeBlock("UGARITIC");

			/// <summary>
			/// Constant for the "Deseret" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock DESERET = new UnicodeBlock("DESERET");

			/// <summary>
			/// Constant for the "Shavian" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock SHAVIAN = new UnicodeBlock("SHAVIAN");

			/// <summary>
			/// Constant for the "Osmanya" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock OSMANYA = new UnicodeBlock("OSMANYA");

			/// <summary>
			/// Constant for the "Cypriot Syllabary" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock CYPRIOT_SYLLABARY = new UnicodeBlock("CYPRIOT_SYLLABARY", "CYPRIOT SYLLABARY", "CYPRIOTSYLLABARY");

			/// <summary>
			/// Constant for the "Byzantine Musical Symbols" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock BYZANTINE_MUSICAL_SYMBOLS = new UnicodeBlock("BYZANTINE_MUSICAL_SYMBOLS", "BYZANTINE MUSICAL SYMBOLS", "BYZANTINEMUSICALSYMBOLS");

			/// <summary>
			/// Constant for the "Musical Symbols" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock MUSICAL_SYMBOLS = new UnicodeBlock("MUSICAL_SYMBOLS", "MUSICAL SYMBOLS", "MUSICALSYMBOLS");

			/// <summary>
			/// Constant for the "Tai Xuan Jing Symbols" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock TAI_XUAN_JING_SYMBOLS = new UnicodeBlock("TAI_XUAN_JING_SYMBOLS", "TAI XUAN JING SYMBOLS", "TAIXUANJINGSYMBOLS");

			/// <summary>
			/// Constant for the "Mathematical Alphanumeric Symbols" Unicode
			/// character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock MATHEMATICAL_ALPHANUMERIC_SYMBOLS = new UnicodeBlock("MATHEMATICAL_ALPHANUMERIC_SYMBOLS", "MATHEMATICAL ALPHANUMERIC SYMBOLS", "MATHEMATICALALPHANUMERICSYMBOLS");

			/// <summary>
			/// Constant for the "CJK Unified Ideographs Extension B" Unicode
			/// character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B = new UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B", "CJK UNIFIED IDEOGRAPHS EXTENSION B", "CJKUNIFIEDIDEOGRAPHSEXTENSIONB");

			/// <summary>
			/// Constant for the "CJK Compatibility Ideographs Supplement" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT = new UnicodeBlock("CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT", "CJK COMPATIBILITY IDEOGRAPHS SUPPLEMENT", "CJKCOMPATIBILITYIDEOGRAPHSSUPPLEMENT");

			/// <summary>
			/// Constant for the "Tags" Unicode character block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock TAGS = new UnicodeBlock("TAGS");

			/// <summary>
			/// Constant for the "Variation Selectors Supplement" Unicode character
			/// block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock VARIATION_SELECTORS_SUPPLEMENT = new UnicodeBlock("VARIATION_SELECTORS_SUPPLEMENT", "VARIATION SELECTORS SUPPLEMENT", "VARIATIONSELECTORSSUPPLEMENT");

			/// <summary>
			/// Constant for the "Supplementary Private Use Area-A" Unicode character
			/// block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock SUPPLEMENTARY_PRIVATE_USE_AREA_A = new UnicodeBlock("SUPPLEMENTARY_PRIVATE_USE_AREA_A", "SUPPLEMENTARY PRIVATE USE AREA-A", "SUPPLEMENTARYPRIVATEUSEAREA-A");

			/// <summary>
			/// Constant for the "Supplementary Private Use Area-B" Unicode character
			/// block.
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock SUPPLEMENTARY_PRIVATE_USE_AREA_B = new UnicodeBlock("SUPPLEMENTARY_PRIVATE_USE_AREA_B", "SUPPLEMENTARY PRIVATE USE AREA-B", "SUPPLEMENTARYPRIVATEUSEAREA-B");

			/// <summary>
			/// Constant for the "High Surrogates" Unicode character block.
			/// This block represents codepoint values in the high surrogate
			/// range: U+D800 through U+DB7F
			/// 
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock HIGH_SURROGATES = new UnicodeBlock("HIGH_SURROGATES", "HIGH SURROGATES", "HIGHSURROGATES");

			/// <summary>
			/// Constant for the "High Private Use Surrogates" Unicode character
			/// block.
			/// This block represents codepoint values in the private use high
			/// surrogate range: U+DB80 through U+DBFF
			/// 
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock HIGH_PRIVATE_USE_SURROGATES = new UnicodeBlock("HIGH_PRIVATE_USE_SURROGATES", "HIGH PRIVATE USE SURROGATES", "HIGHPRIVATEUSESURROGATES");

			/// <summary>
			/// Constant for the "Low Surrogates" Unicode character block.
			/// This block represents codepoint values in the low surrogate
			/// range: U+DC00 through U+DFFF
			/// 
			/// @since 1.5
			/// </summary>
			public static readonly UnicodeBlock LOW_SURROGATES = new UnicodeBlock("LOW_SURROGATES", "LOW SURROGATES", "LOWSURROGATES");

			/// <summary>
			/// Constant for the "Arabic Supplement" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ARABIC_SUPPLEMENT = new UnicodeBlock("ARABIC_SUPPLEMENT", "ARABIC SUPPLEMENT", "ARABICSUPPLEMENT");

			/// <summary>
			/// Constant for the "NKo" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock NKO = new UnicodeBlock("NKO");

			/// <summary>
			/// Constant for the "Samaritan" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock SAMARITAN = new UnicodeBlock("SAMARITAN");

			/// <summary>
			/// Constant for the "Mandaic" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock MANDAIC = new UnicodeBlock("MANDAIC");

			/// <summary>
			/// Constant for the "Ethiopic Supplement" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ETHIOPIC_SUPPLEMENT = new UnicodeBlock("ETHIOPIC_SUPPLEMENT", "ETHIOPIC SUPPLEMENT", "ETHIOPICSUPPLEMENT");

			/// <summary>
			/// Constant for the "Unified Canadian Aboriginal Syllabics Extended"
			/// Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS_EXTENDED = new UnicodeBlock("UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS_EXTENDED", "UNIFIED CANADIAN ABORIGINAL SYLLABICS EXTENDED", "UNIFIEDCANADIANABORIGINALSYLLABICSEXTENDED");

			/// <summary>
			/// Constant for the "New Tai Lue" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock NEW_TAI_LUE = new UnicodeBlock("NEW_TAI_LUE", "NEW TAI LUE", "NEWTAILUE");

			/// <summary>
			/// Constant for the "Buginese" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock BUGINESE = new UnicodeBlock("BUGINESE");

			/// <summary>
			/// Constant for the "Tai Tham" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock TAI_THAM = new UnicodeBlock("TAI_THAM", "TAI THAM", "TAITHAM");

			/// <summary>
			/// Constant for the "Balinese" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock BALINESE = new UnicodeBlock("BALINESE");

			/// <summary>
			/// Constant for the "Sundanese" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock SUNDANESE = new UnicodeBlock("SUNDANESE");

			/// <summary>
			/// Constant for the "Batak" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock BATAK = new UnicodeBlock("BATAK");

			/// <summary>
			/// Constant for the "Lepcha" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock LEPCHA = new UnicodeBlock("LEPCHA");

			/// <summary>
			/// Constant for the "Ol Chiki" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock OL_CHIKI = new UnicodeBlock("OL_CHIKI", "OL CHIKI", "OLCHIKI");

			/// <summary>
			/// Constant for the "Vedic Extensions" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock VEDIC_EXTENSIONS = new UnicodeBlock("VEDIC_EXTENSIONS", "VEDIC EXTENSIONS", "VEDICEXTENSIONS");

			/// <summary>
			/// Constant for the "Phonetic Extensions Supplement" Unicode character
			/// block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock PHONETIC_EXTENSIONS_SUPPLEMENT = new UnicodeBlock("PHONETIC_EXTENSIONS_SUPPLEMENT", "PHONETIC EXTENSIONS SUPPLEMENT", "PHONETICEXTENSIONSSUPPLEMENT");

			/// <summary>
			/// Constant for the "Combining Diacritical Marks Supplement" Unicode
			/// character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock COMBINING_DIACRITICAL_MARKS_SUPPLEMENT = new UnicodeBlock("COMBINING_DIACRITICAL_MARKS_SUPPLEMENT", "COMBINING DIACRITICAL MARKS SUPPLEMENT", "COMBININGDIACRITICALMARKSSUPPLEMENT");

			/// <summary>
			/// Constant for the "Glagolitic" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock GLAGOLITIC = new UnicodeBlock("GLAGOLITIC");

			/// <summary>
			/// Constant for the "Latin Extended-C" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock LATIN_EXTENDED_C = new UnicodeBlock("LATIN_EXTENDED_C", "LATIN EXTENDED-C", "LATINEXTENDED-C");

			/// <summary>
			/// Constant for the "Coptic" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock COPTIC = new UnicodeBlock("COPTIC");

			/// <summary>
			/// Constant for the "Georgian Supplement" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock GEORGIAN_SUPPLEMENT = new UnicodeBlock("GEORGIAN_SUPPLEMENT", "GEORGIAN SUPPLEMENT", "GEORGIANSUPPLEMENT");

			/// <summary>
			/// Constant for the "Tifinagh" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock TIFINAGH = new UnicodeBlock("TIFINAGH");

			/// <summary>
			/// Constant for the "Ethiopic Extended" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ETHIOPIC_EXTENDED = new UnicodeBlock("ETHIOPIC_EXTENDED", "ETHIOPIC EXTENDED", "ETHIOPICEXTENDED");

			/// <summary>
			/// Constant for the "Cyrillic Extended-A" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock CYRILLIC_EXTENDED_A = new UnicodeBlock("CYRILLIC_EXTENDED_A", "CYRILLIC EXTENDED-A", "CYRILLICEXTENDED-A");

			/// <summary>
			/// Constant for the "Supplemental Punctuation" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock SUPPLEMENTAL_PUNCTUATION = new UnicodeBlock("SUPPLEMENTAL_PUNCTUATION", "SUPPLEMENTAL PUNCTUATION", "SUPPLEMENTALPUNCTUATION");

			/// <summary>
			/// Constant for the "CJK Strokes" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock CJK_STROKES = new UnicodeBlock("CJK_STROKES", "CJK STROKES", "CJKSTROKES");

			/// <summary>
			/// Constant for the "Lisu" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock LISU = new UnicodeBlock("LISU");

			/// <summary>
			/// Constant for the "Vai" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock VAI = new UnicodeBlock("VAI");

			/// <summary>
			/// Constant for the "Cyrillic Extended-B" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock CYRILLIC_EXTENDED_B = new UnicodeBlock("CYRILLIC_EXTENDED_B", "CYRILLIC EXTENDED-B", "CYRILLICEXTENDED-B");

			/// <summary>
			/// Constant for the "Bamum" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock BAMUM = new UnicodeBlock("BAMUM");

			/// <summary>
			/// Constant for the "Modifier Tone Letters" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock MODIFIER_TONE_LETTERS = new UnicodeBlock("MODIFIER_TONE_LETTERS", "MODIFIER TONE LETTERS", "MODIFIERTONELETTERS");

			/// <summary>
			/// Constant for the "Latin Extended-D" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock LATIN_EXTENDED_D = new UnicodeBlock("LATIN_EXTENDED_D", "LATIN EXTENDED-D", "LATINEXTENDED-D");

			/// <summary>
			/// Constant for the "Syloti Nagri" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock SYLOTI_NAGRI = new UnicodeBlock("SYLOTI_NAGRI", "SYLOTI NAGRI", "SYLOTINAGRI");

			/// <summary>
			/// Constant for the "Common Indic Number Forms" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock COMMON_INDIC_NUMBER_FORMS = new UnicodeBlock("COMMON_INDIC_NUMBER_FORMS", "COMMON INDIC NUMBER FORMS", "COMMONINDICNUMBERFORMS");

			/// <summary>
			/// Constant for the "Phags-pa" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock PHAGS_PA = new UnicodeBlock("PHAGS_PA", "PHAGS-PA");

			/// <summary>
			/// Constant for the "Saurashtra" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock SAURASHTRA = new UnicodeBlock("SAURASHTRA");

			/// <summary>
			/// Constant for the "Devanagari Extended" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock DEVANAGARI_EXTENDED = new UnicodeBlock("DEVANAGARI_EXTENDED", "DEVANAGARI EXTENDED", "DEVANAGARIEXTENDED");

			/// <summary>
			/// Constant for the "Kayah Li" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock KAYAH_LI = new UnicodeBlock("KAYAH_LI", "KAYAH LI", "KAYAHLI");

			/// <summary>
			/// Constant for the "Rejang" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock REJANG = new UnicodeBlock("REJANG");

			/// <summary>
			/// Constant for the "Hangul Jamo Extended-A" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock HANGUL_JAMO_EXTENDED_A = new UnicodeBlock("HANGUL_JAMO_EXTENDED_A", "HANGUL JAMO EXTENDED-A", "HANGULJAMOEXTENDED-A");

			/// <summary>
			/// Constant for the "Javanese" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock JAVANESE = new UnicodeBlock("JAVANESE");

			/// <summary>
			/// Constant for the "Cham" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock CHAM = new UnicodeBlock("CHAM");

			/// <summary>
			/// Constant for the "Myanmar Extended-A" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock MYANMAR_EXTENDED_A = new UnicodeBlock("MYANMAR_EXTENDED_A", "MYANMAR EXTENDED-A", "MYANMAREXTENDED-A");

			/// <summary>
			/// Constant for the "Tai Viet" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock TAI_VIET = new UnicodeBlock("TAI_VIET", "TAI VIET", "TAIVIET");

			/// <summary>
			/// Constant for the "Ethiopic Extended-A" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ETHIOPIC_EXTENDED_A = new UnicodeBlock("ETHIOPIC_EXTENDED_A", "ETHIOPIC EXTENDED-A", "ETHIOPICEXTENDED-A");

			/// <summary>
			/// Constant for the "Meetei Mayek" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock MEETEI_MAYEK = new UnicodeBlock("MEETEI_MAYEK", "MEETEI MAYEK", "MEETEIMAYEK");

			/// <summary>
			/// Constant for the "Hangul Jamo Extended-B" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock HANGUL_JAMO_EXTENDED_B = new UnicodeBlock("HANGUL_JAMO_EXTENDED_B", "HANGUL JAMO EXTENDED-B", "HANGULJAMOEXTENDED-B");

			/// <summary>
			/// Constant for the "Vertical Forms" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock VERTICAL_FORMS = new UnicodeBlock("VERTICAL_FORMS", "VERTICAL FORMS", "VERTICALFORMS");

			/// <summary>
			/// Constant for the "Ancient Greek Numbers" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ANCIENT_GREEK_NUMBERS = new UnicodeBlock("ANCIENT_GREEK_NUMBERS", "ANCIENT GREEK NUMBERS", "ANCIENTGREEKNUMBERS");

			/// <summary>
			/// Constant for the "Ancient Symbols" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ANCIENT_SYMBOLS = new UnicodeBlock("ANCIENT_SYMBOLS", "ANCIENT SYMBOLS", "ANCIENTSYMBOLS");

			/// <summary>
			/// Constant for the "Phaistos Disc" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock PHAISTOS_DISC = new UnicodeBlock("PHAISTOS_DISC", "PHAISTOS DISC", "PHAISTOSDISC");

			/// <summary>
			/// Constant for the "Lycian" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock LYCIAN = new UnicodeBlock("LYCIAN");

			/// <summary>
			/// Constant for the "Carian" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock CARIAN = new UnicodeBlock("CARIAN");

			/// <summary>
			/// Constant for the "Old Persian" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock OLD_PERSIAN = new UnicodeBlock("OLD_PERSIAN", "OLD PERSIAN", "OLDPERSIAN");

			/// <summary>
			/// Constant for the "Imperial Aramaic" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock IMPERIAL_ARAMAIC = new UnicodeBlock("IMPERIAL_ARAMAIC", "IMPERIAL ARAMAIC", "IMPERIALARAMAIC");

			/// <summary>
			/// Constant for the "Phoenician" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock PHOENICIAN = new UnicodeBlock("PHOENICIAN");

			/// <summary>
			/// Constant for the "Lydian" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock LYDIAN = new UnicodeBlock("LYDIAN");

			/// <summary>
			/// Constant for the "Kharoshthi" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock KHAROSHTHI = new UnicodeBlock("KHAROSHTHI");

			/// <summary>
			/// Constant for the "Old South Arabian" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock OLD_SOUTH_ARABIAN = new UnicodeBlock("OLD_SOUTH_ARABIAN", "OLD SOUTH ARABIAN", "OLDSOUTHARABIAN");

			/// <summary>
			/// Constant for the "Avestan" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock AVESTAN = new UnicodeBlock("AVESTAN");

			/// <summary>
			/// Constant for the "Inscriptional Parthian" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock INSCRIPTIONAL_PARTHIAN = new UnicodeBlock("INSCRIPTIONAL_PARTHIAN", "INSCRIPTIONAL PARTHIAN", "INSCRIPTIONALPARTHIAN");

			/// <summary>
			/// Constant for the "Inscriptional Pahlavi" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock INSCRIPTIONAL_PAHLAVI = new UnicodeBlock("INSCRIPTIONAL_PAHLAVI", "INSCRIPTIONAL PAHLAVI", "INSCRIPTIONALPAHLAVI");

			/// <summary>
			/// Constant for the "Old Turkic" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock OLD_TURKIC = new UnicodeBlock("OLD_TURKIC", "OLD TURKIC", "OLDTURKIC");

			/// <summary>
			/// Constant for the "Rumi Numeral Symbols" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock RUMI_NUMERAL_SYMBOLS = new UnicodeBlock("RUMI_NUMERAL_SYMBOLS", "RUMI NUMERAL SYMBOLS", "RUMINUMERALSYMBOLS");

			/// <summary>
			/// Constant for the "Brahmi" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock BRAHMI = new UnicodeBlock("BRAHMI");

			/// <summary>
			/// Constant for the "Kaithi" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock KAITHI = new UnicodeBlock("KAITHI");

			/// <summary>
			/// Constant for the "Cuneiform" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock CUNEIFORM = new UnicodeBlock("CUNEIFORM");

			/// <summary>
			/// Constant for the "Cuneiform Numbers and Punctuation" Unicode
			/// character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock CUNEIFORM_NUMBERS_AND_PUNCTUATION = new UnicodeBlock("CUNEIFORM_NUMBERS_AND_PUNCTUATION", "CUNEIFORM NUMBERS AND PUNCTUATION", "CUNEIFORMNUMBERSANDPUNCTUATION");

			/// <summary>
			/// Constant for the "Egyptian Hieroglyphs" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock EGYPTIAN_HIEROGLYPHS = new UnicodeBlock("EGYPTIAN_HIEROGLYPHS", "EGYPTIAN HIEROGLYPHS", "EGYPTIANHIEROGLYPHS");

			/// <summary>
			/// Constant for the "Bamum Supplement" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock BAMUM_SUPPLEMENT = new UnicodeBlock("BAMUM_SUPPLEMENT", "BAMUM SUPPLEMENT", "BAMUMSUPPLEMENT");

			/// <summary>
			/// Constant for the "Kana Supplement" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock KANA_SUPPLEMENT = new UnicodeBlock("KANA_SUPPLEMENT", "KANA SUPPLEMENT", "KANASUPPLEMENT");

			/// <summary>
			/// Constant for the "Ancient Greek Musical Notation" Unicode character
			/// block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ANCIENT_GREEK_MUSICAL_NOTATION = new UnicodeBlock("ANCIENT_GREEK_MUSICAL_NOTATION", "ANCIENT GREEK MUSICAL NOTATION", "ANCIENTGREEKMUSICALNOTATION");

			/// <summary>
			/// Constant for the "Counting Rod Numerals" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock COUNTING_ROD_NUMERALS = new UnicodeBlock("COUNTING_ROD_NUMERALS", "COUNTING ROD NUMERALS", "COUNTINGRODNUMERALS");

			/// <summary>
			/// Constant for the "Mahjong Tiles" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock MAHJONG_TILES = new UnicodeBlock("MAHJONG_TILES", "MAHJONG TILES", "MAHJONGTILES");

			/// <summary>
			/// Constant for the "Domino Tiles" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock DOMINO_TILES = new UnicodeBlock("DOMINO_TILES", "DOMINO TILES", "DOMINOTILES");

			/// <summary>
			/// Constant for the "Playing Cards" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock PLAYING_CARDS = new UnicodeBlock("PLAYING_CARDS", "PLAYING CARDS", "PLAYINGCARDS");

			/// <summary>
			/// Constant for the "Enclosed Alphanumeric Supplement" Unicode character
			/// block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ENCLOSED_ALPHANUMERIC_SUPPLEMENT = new UnicodeBlock("ENCLOSED_ALPHANUMERIC_SUPPLEMENT", "ENCLOSED ALPHANUMERIC SUPPLEMENT", "ENCLOSEDALPHANUMERICSUPPLEMENT");

			/// <summary>
			/// Constant for the "Enclosed Ideographic Supplement" Unicode character
			/// block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ENCLOSED_IDEOGRAPHIC_SUPPLEMENT = new UnicodeBlock("ENCLOSED_IDEOGRAPHIC_SUPPLEMENT", "ENCLOSED IDEOGRAPHIC SUPPLEMENT", "ENCLOSEDIDEOGRAPHICSUPPLEMENT");

			/// <summary>
			/// Constant for the "Miscellaneous Symbols And Pictographs" Unicode
			/// character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock MISCELLANEOUS_SYMBOLS_AND_PICTOGRAPHS = new UnicodeBlock("MISCELLANEOUS_SYMBOLS_AND_PICTOGRAPHS", "MISCELLANEOUS SYMBOLS AND PICTOGRAPHS", "MISCELLANEOUSSYMBOLSANDPICTOGRAPHS");

			/// <summary>
			/// Constant for the "Emoticons" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock EMOTICONS = new UnicodeBlock("EMOTICONS");

			/// <summary>
			/// Constant for the "Transport And Map Symbols" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock TRANSPORT_AND_MAP_SYMBOLS = new UnicodeBlock("TRANSPORT_AND_MAP_SYMBOLS", "TRANSPORT AND MAP SYMBOLS", "TRANSPORTANDMAPSYMBOLS");

			/// <summary>
			/// Constant for the "Alchemical Symbols" Unicode character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock ALCHEMICAL_SYMBOLS = new UnicodeBlock("ALCHEMICAL_SYMBOLS", "ALCHEMICAL SYMBOLS", "ALCHEMICALSYMBOLS");

			/// <summary>
			/// Constant for the "CJK Unified Ideographs Extension C" Unicode
			/// character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock CJK_UNIFIED_IDEOGRAPHS_EXTENSION_C = new UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_C", "CJK UNIFIED IDEOGRAPHS EXTENSION C", "CJKUNIFIEDIDEOGRAPHSEXTENSIONC");

			/// <summary>
			/// Constant for the "CJK Unified Ideographs Extension D" Unicode
			/// character block.
			/// @since 1.7
			/// </summary>
			public static readonly UnicodeBlock CJK_UNIFIED_IDEOGRAPHS_EXTENSION_D = new UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_D", "CJK UNIFIED IDEOGRAPHS EXTENSION D", "CJKUNIFIEDIDEOGRAPHSEXTENSIOND");

			/// <summary>
			/// Constant for the "Arabic Extended-A" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock ARABIC_EXTENDED_A = new UnicodeBlock("ARABIC_EXTENDED_A", "ARABIC EXTENDED-A", "ARABICEXTENDED-A");

			/// <summary>
			/// Constant for the "Sundanese Supplement" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock SUNDANESE_SUPPLEMENT = new UnicodeBlock("SUNDANESE_SUPPLEMENT", "SUNDANESE SUPPLEMENT", "SUNDANESESUPPLEMENT");

			/// <summary>
			/// Constant for the "Meetei Mayek Extensions" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock MEETEI_MAYEK_EXTENSIONS = new UnicodeBlock("MEETEI_MAYEK_EXTENSIONS", "MEETEI MAYEK EXTENSIONS", "MEETEIMAYEKEXTENSIONS");

			/// <summary>
			/// Constant for the "Meroitic Hieroglyphs" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock MEROITIC_HIEROGLYPHS = new UnicodeBlock("MEROITIC_HIEROGLYPHS", "MEROITIC HIEROGLYPHS", "MEROITICHIEROGLYPHS");

			/// <summary>
			/// Constant for the "Meroitic Cursive" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock MEROITIC_CURSIVE = new UnicodeBlock("MEROITIC_CURSIVE", "MEROITIC CURSIVE", "MEROITICCURSIVE");

			/// <summary>
			/// Constant for the "Sora Sompeng" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock SORA_SOMPENG = new UnicodeBlock("SORA_SOMPENG", "SORA SOMPENG", "SORASOMPENG");

			/// <summary>
			/// Constant for the "Chakma" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock CHAKMA = new UnicodeBlock("CHAKMA");

			/// <summary>
			/// Constant for the "Sharada" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock SHARADA = new UnicodeBlock("SHARADA");

			/// <summary>
			/// Constant for the "Takri" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock TAKRI = new UnicodeBlock("TAKRI");

			/// <summary>
			/// Constant for the "Miao" Unicode character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock MIAO = new UnicodeBlock("MIAO");

			/// <summary>
			/// Constant for the "Arabic Mathematical Alphabetic Symbols" Unicode
			/// character block.
			/// @since 1.8
			/// </summary>
			public static readonly UnicodeBlock ARABIC_MATHEMATICAL_ALPHABETIC_SYMBOLS = new UnicodeBlock("ARABIC_MATHEMATICAL_ALPHABETIC_SYMBOLS", "ARABIC MATHEMATICAL ALPHABETIC SYMBOLS", "ARABICMATHEMATICALALPHABETICSYMBOLS");

			internal static readonly int[] BlockStarts = new int[] {0x0000, 0x0080, 0x0100, 0x0180, 0x0250, 0x02B0, 0x0300, 0x0370, 0x0400, 0x0500, 0x0530, 0x0590, 0x0600, 0x0700, 0x0750, 0x0780, 0x07C0, 0x0800, 0x0840, 0x0860, 0x08A0, 0x0900, 0x0980, 0x0A00, 0x0A80, 0x0B00, 0x0B80, 0x0C00, 0x0C80, 0x0D00, 0x0D80, 0x0E00, 0x0E80, 0x0F00, 0x1000, 0x10A0, 0x1100, 0x1200, 0x1380, 0x13A0, 0x1400, 0x1680, 0x16A0, 0x1700, 0x1720, 0x1740, 0x1760, 0x1780, 0x1800, 0x18B0, 0x1900, 0x1950, 0x1980, 0x19E0, 0x1A00, 0x1A20, 0x1AB0, 0x1B00, 0x1B80, 0x1BC0, 0x1C00, 0x1C50, 0x1C80, 0x1CC0, 0x1CD0, 0x1D00, 0x1D80, 0x1DC0, 0x1E00, 0x1F00, 0x2000, 0x2070, 0x20A0, 0x20D0, 0x2100, 0x2150, 0x2190, 0x2200, 0x2300, 0x2400, 0x2440, 0x2460, 0x2500, 0x2580, 0x25A0, 0x2600, 0x2700, 0x27C0, 0x27F0, 0x2800, 0x2900, 0x2980, 0x2A00, 0x2B00, 0x2C00, 0x2C60, 0x2C80, 0x2D00, 0x2D30, 0x2D80, 0x2DE0, 0x2E00, 0x2E80, 0x2F00, 0x2FE0, 0x2FF0, 0x3000, 0x3040, 0x30A0, 0x3100, 0x3130, 0x3190, 0x31A0, 0x31C0, 0x31F0, 0x3200, 0x3300, 0x3400, 0x4DC0, 0x4E00, 0xA000, 0xA490, 0xA4D0, 0xA500, 0xA640, 0xA6A0, 0xA700, 0xA720, 0xA800, 0xA830, 0xA840, 0xA880, 0xA8E0, 0xA900, 0xA930, 0xA960, 0xA980, 0xA9E0, 0xAA00, 0xAA60, 0xAA80, 0xAAE0, 0xAB00, 0xAB30, 0xABC0, 0xAC00, 0xD7B0, 0xD800, 0xDB80, 0xDC00, 0xE000, 0xF900, 0xFB00, 0xFB50, 0xFE00, 0xFE10, 0xFE20, 0xFE30, 0xFE50, 0xFE70, 0xFF00, 0xFFF0, 0x10000, 0x10080, 0x10100, 0x10140, 0x10190, 0x101D0, 0x10200, 0x10280, 0x102A0, 0x102E0, 0x10300, 0x10330, 0x10350, 0x10380, 0x103A0, 0x103E0, 0x10400, 0x10450, 0x10480, 0x104B0, 0x10800, 0x10840, 0x10860, 0x10900, 0x10920, 0x10940, 0x10980, 0x109A0, 0x10A00, 0x10A60, 0x10A80, 0x10B00, 0x10B40, 0x10B60, 0x10B80, 0x10C00, 0x10C50, 0x10E60, 0x10E80, 0x11000, 0x11080, 0x110D0, 0x11100, 0x11150, 0x11180, 0x111E0, 0x11680, 0x116D0, 0x12000, 0x12400, 0x12480, 0x13000, 0x13430, 0x16800, 0x16A40, 0x16F00, 0x16FA0, 0x1B000, 0x1B100, 0x1D000, 0x1D100, 0x1D200, 0x1D250, 0x1D300, 0x1D360, 0x1D380, 0x1D400, 0x1D800, 0x1EE00, 0x1EF00, 0x1F000, 0x1F030, 0x1F0A0, 0x1F100, 0x1F200, 0x1F300, 0x1F600, 0x1F650, 0x1F680, 0x1F700, 0x1F780, 0x20000, 0x2A6E0, 0x2A700, 0x2B740, 0x2B820, 0x2F800, 0x2FA20, 0xE0000, 0xE0080, 0xE0100, 0xE01F0, 0xF0000, 0x100000};

			internal static readonly UnicodeBlock[] Blocks = new UnicodeBlock[] {BASIC_LATIN, LATIN_1_SUPPLEMENT, LATIN_EXTENDED_A, LATIN_EXTENDED_B, IPA_EXTENSIONS, SPACING_MODIFIER_LETTERS, COMBINING_DIACRITICAL_MARKS, GREEK, CYRILLIC, CYRILLIC_SUPPLEMENTARY, ARMENIAN, HEBREW, ARABIC, SYRIAC, ARABIC_SUPPLEMENT, THAANA, NKO, SAMARITAN, MANDAIC, null, ARABIC_EXTENDED_A, DEVANAGARI, BENGALI, GURMUKHI, GUJARATI, ORIYA, TAMIL, TELUGU, KANNADA, MALAYALAM, SINHALA, THAI, LAO, TIBETAN, MYANMAR, GEORGIAN, HANGUL_JAMO, ETHIOPIC, ETHIOPIC_SUPPLEMENT, CHEROKEE, UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS, OGHAM, RUNIC, TAGALOG, HANUNOO, BUHID, TAGBANWA, KHMER, MONGOLIAN, UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS_EXTENDED, LIMBU, TAI_LE, NEW_TAI_LUE, KHMER_SYMBOLS, BUGINESE, TAI_THAM, null, BALINESE, SUNDANESE, BATAK, LEPCHA, OL_CHIKI, null, SUNDANESE_SUPPLEMENT, VEDIC_EXTENSIONS, PHONETIC_EXTENSIONS, PHONETIC_EXTENSIONS_SUPPLEMENT, COMBINING_DIACRITICAL_MARKS_SUPPLEMENT, LATIN_EXTENDED_ADDITIONAL, GREEK_EXTENDED, GENERAL_PUNCTUATION, SUPERSCRIPTS_AND_SUBSCRIPTS, CURRENCY_SYMBOLS, COMBINING_MARKS_FOR_SYMBOLS, LETTERLIKE_SYMBOLS, NUMBER_FORMS, ARROWS, MATHEMATICAL_OPERATORS, MISCELLANEOUS_TECHNICAL, CONTROL_PICTURES, OPTICAL_CHARACTER_RECOGNITION, ENCLOSED_ALPHANUMERICS, BOX_DRAWING, BLOCK_ELEMENTS, GEOMETRIC_SHAPES, MISCELLANEOUS_SYMBOLS, DINGBATS, MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A, SUPPLEMENTAL_ARROWS_A, BRAILLE_PATTERNS, SUPPLEMENTAL_ARROWS_B, MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B, SUPPLEMENTAL_MATHEMATICAL_OPERATORS, MISCELLANEOUS_SYMBOLS_AND_ARROWS, GLAGOLITIC, LATIN_EXTENDED_C, COPTIC, GEORGIAN_SUPPLEMENT, TIFINAGH, ETHIOPIC_EXTENDED, CYRILLIC_EXTENDED_A, SUPPLEMENTAL_PUNCTUATION, CJK_RADICALS_SUPPLEMENT, KANGXI_RADICALS, null, IDEOGRAPHIC_DESCRIPTION_CHARACTERS, CJK_SYMBOLS_AND_PUNCTUATION, HIRAGANA, KATAKANA, BOPOMOFO, HANGUL_COMPATIBILITY_JAMO, KANBUN, BOPOMOFO_EXTENDED, CJK_STROKES, KATAKANA_PHONETIC_EXTENSIONS, ENCLOSED_CJK_LETTERS_AND_MONTHS, CJK_COMPATIBILITY, CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A, YIJING_HEXAGRAM_SYMBOLS, CJK_UNIFIED_IDEOGRAPHS, YI_SYLLABLES, YI_RADICALS, LISU, VAI, CYRILLIC_EXTENDED_B, BAMUM, MODIFIER_TONE_LETTERS, LATIN_EXTENDED_D, SYLOTI_NAGRI, COMMON_INDIC_NUMBER_FORMS, PHAGS_PA, SAURASHTRA, DEVANAGARI_EXTENDED, KAYAH_LI, REJANG, HANGUL_JAMO_EXTENDED_A, JAVANESE, null, CHAM, MYANMAR_EXTENDED_A, TAI_VIET, MEETEI_MAYEK_EXTENSIONS, ETHIOPIC_EXTENDED_A, null, MEETEI_MAYEK, HANGUL_SYLLABLES, HANGUL_JAMO_EXTENDED_B, HIGH_SURROGATES, HIGH_PRIVATE_USE_SURROGATES, LOW_SURROGATES, PRIVATE_USE_AREA, CJK_COMPATIBILITY_IDEOGRAPHS, ALPHABETIC_PRESENTATION_FORMS, ARABIC_PRESENTATION_FORMS_A, VARIATION_SELECTORS, VERTICAL_FORMS, COMBINING_HALF_MARKS, CJK_COMPATIBILITY_FORMS, SMALL_FORM_VARIANTS, ARABIC_PRESENTATION_FORMS_B, HALFWIDTH_AND_FULLWIDTH_FORMS, SPECIALS, LINEAR_B_SYLLABARY, LINEAR_B_IDEOGRAMS, AEGEAN_NUMBERS, ANCIENT_GREEK_NUMBERS, ANCIENT_SYMBOLS, PHAISTOS_DISC, null, LYCIAN, CARIAN, null, OLD_ITALIC, GOTHIC, null, UGARITIC, OLD_PERSIAN, null, DESERET, SHAVIAN, OSMANYA, null, CYPRIOT_SYLLABARY, IMPERIAL_ARAMAIC, null, PHOENICIAN, LYDIAN, null, MEROITIC_HIEROGLYPHS, MEROITIC_CURSIVE, KHAROSHTHI, OLD_SOUTH_ARABIAN, null, AVESTAN, INSCRIPTIONAL_PARTHIAN, INSCRIPTIONAL_PAHLAVI, null, OLD_TURKIC, null, RUMI_NUMERAL_SYMBOLS, null, BRAHMI, KAITHI, SORA_SOMPENG, CHAKMA, null, SHARADA, null, TAKRI, null, CUNEIFORM, CUNEIFORM_NUMBERS_AND_PUNCTUATION, null, EGYPTIAN_HIEROGLYPHS, null, BAMUM_SUPPLEMENT, null, MIAO, null, KANA_SUPPLEMENT, null, BYZANTINE_MUSICAL_SYMBOLS, MUSICAL_SYMBOLS, ANCIENT_GREEK_MUSICAL_NOTATION, null, TAI_XUAN_JING_SYMBOLS, COUNTING_ROD_NUMERALS, null, MATHEMATICAL_ALPHANUMERIC_SYMBOLS, null, ARABIC_MATHEMATICAL_ALPHABETIC_SYMBOLS, null, MAHJONG_TILES, DOMINO_TILES, PLAYING_CARDS, ENCLOSED_ALPHANUMERIC_SUPPLEMENT, ENCLOSED_IDEOGRAPHIC_SUPPLEMENT, MISCELLANEOUS_SYMBOLS_AND_PICTOGRAPHS, EMOTICONS, null, TRANSPORT_AND_MAP_SYMBOLS, ALCHEMICAL_SYMBOLS, null, CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B, null, CJK_UNIFIED_IDEOGRAPHS_EXTENSION_C, CJK_UNIFIED_IDEOGRAPHS_EXTENSION_D, null, CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT, null, TAGS, null, VARIATION_SELECTORS_SUPPLEMENT, null, SUPPLEMENTARY_PRIVATE_USE_AREA_A, SUPPLEMENTARY_PRIVATE_USE_AREA_B};


			/// <summary>
			/// Returns the object representing the Unicode block containing the
			/// given character, or {@code null} if the character is not a
			/// member of a defined block.
			/// 
			/// <para><b>Note:</b> This method cannot handle
			/// <a href="Character.html#supplementary"> supplementary
			/// characters</a>.  To support all Unicode characters, including
			/// supplementary characters, use the <seealso cref="#of(int)"/> method.
			/// 
			/// </para>
			/// </summary>
			/// <param name="c">  The character in question </param>
			/// <returns>  The {@code UnicodeBlock} instance representing the
			///          Unicode block of which this character is a member, or
			///          {@code null} if the character is not a member of any
			///          Unicode block </returns>
			public static UnicodeBlock Of(char c)
			{
				return Of((int)c);
			}

			/// <summary>
			/// Returns the object representing the Unicode block
			/// containing the given character (Unicode code point), or
			/// {@code null} if the character is not a member of a
			/// defined block.
			/// </summary>
			/// <param name="codePoint"> the character (Unicode code point) in question. </param>
			/// <returns>  The {@code UnicodeBlock} instance representing the
			///          Unicode block of which this character is a member, or
			///          {@code null} if the character is not a member of any
			///          Unicode block </returns>
			/// <exception cref="IllegalArgumentException"> if the specified
			/// {@code codePoint} is an invalid Unicode code point. </exception>
			/// <seealso cref= Character#isValidCodePoint(int)
			/// @since   1.5 </seealso>
			public static UnicodeBlock Of(int codePoint)
			{
				if (!isValidCodePoint(codePoint))
				{
					throw new IllegalArgumentException();
				}

				int top, bottom, current;
				bottom = 0;
				top = BlockStarts.Length;
				current = top / 2;

				// invariant: top > current >= bottom && codePoint >= unicodeBlockStarts[bottom]
				while (top - bottom > 1)
				{
					if (codePoint >= BlockStarts[current])
					{
						bottom = current;
					}
					else
					{
						top = current;
					}
					current = (top + bottom) / 2;
				}
				return Blocks[current];
			}

			/// <summary>
			/// Returns the UnicodeBlock with the given name. Block
			/// names are determined by The Unicode Standard. The file
			/// Blocks-&lt;version&gt;.txt defines blocks for a particular
			/// version of the standard. The <seealso cref="Character"/> class specifies
			/// the version of the standard that it supports.
			/// <para>
			/// This method accepts block names in the following forms:
			/// <ol>
			/// <li> Canonical block names as defined by the Unicode Standard.
			/// For example, the standard defines a "Basic Latin" block. Therefore, this
			/// method accepts "Basic Latin" as a valid block name. The documentation of
			/// each UnicodeBlock provides the canonical name.
			/// <li>Canonical block names with all spaces removed. For example, "BasicLatin"
			/// is a valid block name for the "Basic Latin" block.
			/// <li>The text representation of each constant UnicodeBlock identifier.
			/// For example, this method will return the <seealso cref="#BASIC_LATIN"/> block if
			/// provided with the "BASIC_LATIN" name. This form replaces all spaces and
			/// hyphens in the canonical name with underscores.
			/// </ol>
			/// Finally, character case is ignored for all of the valid block name forms.
			/// For example, "BASIC_LATIN" and "basic_latin" are both valid block names.
			/// The en_US locale's case mapping rules are used to provide case-insensitive
			/// string comparisons for block name validation.
			/// </para>
			/// <para>
			/// If the Unicode Standard changes block names, both the previous and
			/// current names will be accepted.
			/// 
			/// </para>
			/// </summary>
			/// <param name="blockName"> A {@code UnicodeBlock} name. </param>
			/// <returns> The {@code UnicodeBlock} instance identified
			///         by {@code blockName} </returns>
			/// <exception cref="IllegalArgumentException"> if {@code blockName} is an
			///         invalid name </exception>
			/// <exception cref="NullPointerException"> if {@code blockName} is null
			/// @since 1.5 </exception>
			public static UnicodeBlock ForName(String blockName)
			{
				UnicodeBlock block = Map[blockName.ToUpperCase(Locale.US)];
				if (block == null)
				{
					throw new IllegalArgumentException();
				}
				return block;
			}
		}


		/// <summary>
		/// A family of character subsets representing the character scripts
		/// defined in the <a href="http://www.unicode.org/reports/tr24/">
		/// <i>Unicode Standard Annex #24: Script Names</i></a>. Every Unicode
		/// character is assigned to a single Unicode script, either a specific
		/// script, such as <seealso cref="Character.UnicodeScript#LATIN Latin"/>, or
		/// one of the following three special values,
		/// <seealso cref="Character.UnicodeScript#INHERITED Inherited"/>,
		/// <seealso cref="Character.UnicodeScript#COMMON Common"/> or
		/// <seealso cref="Character.UnicodeScript#UNKNOWN Unknown"/>.
		/// 
		/// @since 1.7
		/// </summary>
		public sealed class UnicodeScript
		{
			/// <summary>
			/// Unicode script "Common".
			/// </summary>
			COMMON,
			public static readonly UnicodeScript COMMON = new UnicodeScript("COMMON", InnerEnum.COMMON);

			/// <summary>
			/// Unicode script "Latin".
			/// </summary>
			LATIN,
			public static readonly UnicodeScript LATIN = new UnicodeScript("LATIN", InnerEnum.LATIN);

			/// <summary>
			/// Unicode script "Greek".
			/// </summary>
			GREEK,
			public static readonly UnicodeScript GREEK = new UnicodeScript("GREEK", InnerEnum.GREEK);

			/// <summary>
			/// Unicode script "Cyrillic".
			/// </summary>
			CYRILLIC,
			public static readonly UnicodeScript CYRILLIC = new UnicodeScript("CYRILLIC", InnerEnum.CYRILLIC);

			/// <summary>
			/// Unicode script "Armenian".
			/// </summary>
			ARMENIAN,
			public static readonly UnicodeScript ARMENIAN = new UnicodeScript("ARMENIAN", InnerEnum.ARMENIAN);

			/// <summary>
			/// Unicode script "Hebrew".
			/// </summary>
			HEBREW,
			public static readonly UnicodeScript HEBREW = new UnicodeScript("HEBREW", InnerEnum.HEBREW);

			/// <summary>
			/// Unicode script "Arabic".
			/// </summary>
			ARABIC,
			public static readonly UnicodeScript ARABIC = new UnicodeScript("ARABIC", InnerEnum.ARABIC);

			/// <summary>
			/// Unicode script "Syriac".
			/// </summary>
			SYRIAC,
			public static readonly UnicodeScript SYRIAC = new UnicodeScript("SYRIAC", InnerEnum.SYRIAC);

			/// <summary>
			/// Unicode script "Thaana".
			/// </summary>
			THAANA,
			public static readonly UnicodeScript THAANA = new UnicodeScript("THAANA", InnerEnum.THAANA);

			/// <summary>
			/// Unicode script "Devanagari".
			/// </summary>
			DEVANAGARI,
			public static readonly UnicodeScript DEVANAGARI = new UnicodeScript("DEVANAGARI", InnerEnum.DEVANAGARI);

			/// <summary>
			/// Unicode script "Bengali".
			/// </summary>
			BENGALI,
			public static readonly UnicodeScript BENGALI = new UnicodeScript("BENGALI", InnerEnum.BENGALI);

			/// <summary>
			/// Unicode script "Gurmukhi".
			/// </summary>
			GURMUKHI,
			public static readonly UnicodeScript GURMUKHI = new UnicodeScript("GURMUKHI", InnerEnum.GURMUKHI);

			/// <summary>
			/// Unicode script "Gujarati".
			/// </summary>
			GUJARATI,
			public static readonly UnicodeScript GUJARATI = new UnicodeScript("GUJARATI", InnerEnum.GUJARATI);

			/// <summary>
			/// Unicode script "Oriya".
			/// </summary>
			ORIYA,
			public static readonly UnicodeScript ORIYA = new UnicodeScript("ORIYA", InnerEnum.ORIYA);

			/// <summary>
			/// Unicode script "Tamil".
			/// </summary>
			TAMIL,
			public static readonly UnicodeScript TAMIL = new UnicodeScript("TAMIL", InnerEnum.TAMIL);

			/// <summary>
			/// Unicode script "Telugu".
			/// </summary>
			TELUGU,
			public static readonly UnicodeScript TELUGU = new UnicodeScript("TELUGU", InnerEnum.TELUGU);

			/// <summary>
			/// Unicode script "Kannada".
			/// </summary>
			KANNADA,
			public static readonly UnicodeScript KANNADA = new UnicodeScript("KANNADA", InnerEnum.KANNADA);

			/// <summary>
			/// Unicode script "Malayalam".
			/// </summary>
			MALAYALAM,
			public static readonly UnicodeScript MALAYALAM = new UnicodeScript("MALAYALAM", InnerEnum.MALAYALAM);

			/// <summary>
			/// Unicode script "Sinhala".
			/// </summary>
			SINHALA,
			public static readonly UnicodeScript SINHALA = new UnicodeScript("SINHALA", InnerEnum.SINHALA);

			/// <summary>
			/// Unicode script "Thai".
			/// </summary>
			THAI,
			public static readonly UnicodeScript THAI = new UnicodeScript("THAI", InnerEnum.THAI);

			/// <summary>
			/// Unicode script "Lao".
			/// </summary>
			LAO,
			public static readonly UnicodeScript LAO = new UnicodeScript("LAO", InnerEnum.LAO);

			/// <summary>
			/// Unicode script "Tibetan".
			/// </summary>
			TIBETAN,
			public static readonly UnicodeScript TIBETAN = new UnicodeScript("TIBETAN", InnerEnum.TIBETAN);

			/// <summary>
			/// Unicode script "Myanmar".
			/// </summary>
			MYANMAR,
			public static readonly UnicodeScript MYANMAR = new UnicodeScript("MYANMAR", InnerEnum.MYANMAR);

			/// <summary>
			/// Unicode script "Georgian".
			/// </summary>
			GEORGIAN,
			public static readonly UnicodeScript GEORGIAN = new UnicodeScript("GEORGIAN", InnerEnum.GEORGIAN);

			/// <summary>
			/// Unicode script "Hangul".
			/// </summary>
			HANGUL,
			public static readonly UnicodeScript HANGUL = new UnicodeScript("HANGUL", InnerEnum.HANGUL);

			/// <summary>
			/// Unicode script "Ethiopic".
			/// </summary>
			ETHIOPIC,
			public static readonly UnicodeScript ETHIOPIC = new UnicodeScript("ETHIOPIC", InnerEnum.ETHIOPIC);

			/// <summary>
			/// Unicode script "Cherokee".
			/// </summary>
			CHEROKEE,
			public static readonly UnicodeScript CHEROKEE = new UnicodeScript("CHEROKEE", InnerEnum.CHEROKEE);

			/// <summary>
			/// Unicode script "Canadian_Aboriginal".
			/// </summary>
			CANADIAN_ABORIGINAL,
			public static readonly UnicodeScript CANADIAN_ABORIGINAL = new UnicodeScript("CANADIAN_ABORIGINAL", InnerEnum.CANADIAN_ABORIGINAL);

			/// <summary>
			/// Unicode script "Ogham".
			/// </summary>
			OGHAM,
			public static readonly UnicodeScript OGHAM = new UnicodeScript("OGHAM", InnerEnum.OGHAM);

			/// <summary>
			/// Unicode script "Runic".
			/// </summary>
			RUNIC,
			public static readonly UnicodeScript RUNIC = new UnicodeScript("RUNIC", InnerEnum.RUNIC);

			/// <summary>
			/// Unicode script "Khmer".
			/// </summary>
			KHMER,
			public static readonly UnicodeScript KHMER = new UnicodeScript("KHMER", InnerEnum.KHMER);

			/// <summary>
			/// Unicode script "Mongolian".
			/// </summary>
			MONGOLIAN,
			public static readonly UnicodeScript MONGOLIAN = new UnicodeScript("MONGOLIAN", InnerEnum.MONGOLIAN);

			/// <summary>
			/// Unicode script "Hiragana".
			/// </summary>
			HIRAGANA,
			public static readonly UnicodeScript HIRAGANA = new UnicodeScript("HIRAGANA", InnerEnum.HIRAGANA);

			/// <summary>
			/// Unicode script "Katakana".
			/// </summary>
			KATAKANA,
			public static readonly UnicodeScript KATAKANA = new UnicodeScript("KATAKANA", InnerEnum.KATAKANA);

			/// <summary>
			/// Unicode script "Bopomofo".
			/// </summary>
			BOPOMOFO,
			public static readonly UnicodeScript BOPOMOFO = new UnicodeScript("BOPOMOFO", InnerEnum.BOPOMOFO);

			/// <summary>
			/// Unicode script "Han".
			/// </summary>
			HAN,
			public static readonly UnicodeScript HAN = new UnicodeScript("HAN", InnerEnum.HAN);

			/// <summary>
			/// Unicode script "Yi".
			/// </summary>
			YI,
			public static readonly UnicodeScript YI = new UnicodeScript("YI", InnerEnum.YI);

			/// <summary>
			/// Unicode script "Old_Italic".
			/// </summary>
			OLD_ITALIC,
			public static readonly UnicodeScript OLD_ITALIC = new UnicodeScript("OLD_ITALIC", InnerEnum.OLD_ITALIC);

			/// <summary>
			/// Unicode script "Gothic".
			/// </summary>
			GOTHIC,
			public static readonly UnicodeScript GOTHIC = new UnicodeScript("GOTHIC", InnerEnum.GOTHIC);

			/// <summary>
			/// Unicode script "Deseret".
			/// </summary>
			DESERET,
			public static readonly UnicodeScript DESERET = new UnicodeScript("DESERET", InnerEnum.DESERET);

			/// <summary>
			/// Unicode script "Inherited".
			/// </summary>
			INHERITED,
			public static readonly UnicodeScript INHERITED = new UnicodeScript("INHERITED", InnerEnum.INHERITED);

			/// <summary>
			/// Unicode script "Tagalog".
			/// </summary>
			TAGALOG,
			public static readonly UnicodeScript TAGALOG = new UnicodeScript("TAGALOG", InnerEnum.TAGALOG);

			/// <summary>
			/// Unicode script "Hanunoo".
			/// </summary>
			HANUNOO,
			public static readonly UnicodeScript HANUNOO = new UnicodeScript("HANUNOO", InnerEnum.HANUNOO);

			/// <summary>
			/// Unicode script "Buhid".
			/// </summary>
			BUHID,
			public static readonly UnicodeScript BUHID = new UnicodeScript("BUHID", InnerEnum.BUHID);

			/// <summary>
			/// Unicode script "Tagbanwa".
			/// </summary>
			TAGBANWA,
			public static readonly UnicodeScript TAGBANWA = new UnicodeScript("TAGBANWA", InnerEnum.TAGBANWA);

			/// <summary>
			/// Unicode script "Limbu".
			/// </summary>
			LIMBU,
			public static readonly UnicodeScript LIMBU = new UnicodeScript("LIMBU", InnerEnum.LIMBU);

			/// <summary>
			/// Unicode script "Tai_Le".
			/// </summary>
			TAI_LE,
			public static readonly UnicodeScript TAI_LE = new UnicodeScript("TAI_LE", InnerEnum.TAI_LE);

			/// <summary>
			/// Unicode script "Linear_B".
			/// </summary>
			LINEAR_B,
			public static readonly UnicodeScript LINEAR_B = new UnicodeScript("LINEAR_B", InnerEnum.LINEAR_B);

			/// <summary>
			/// Unicode script "Ugaritic".
			/// </summary>
			UGARITIC,
			public static readonly UnicodeScript UGARITIC = new UnicodeScript("UGARITIC", InnerEnum.UGARITIC);

			/// <summary>
			/// Unicode script "Shavian".
			/// </summary>
			SHAVIAN,
			public static readonly UnicodeScript SHAVIAN = new UnicodeScript("SHAVIAN", InnerEnum.SHAVIAN);

			/// <summary>
			/// Unicode script "Osmanya".
			/// </summary>
			OSMANYA,
			public static readonly UnicodeScript OSMANYA = new UnicodeScript("OSMANYA", InnerEnum.OSMANYA);

			/// <summary>
			/// Unicode script "Cypriot".
			/// </summary>
			CYPRIOT,
			public static readonly UnicodeScript CYPRIOT = new UnicodeScript("CYPRIOT", InnerEnum.CYPRIOT);

			/// <summary>
			/// Unicode script "Braille".
			/// </summary>
			BRAILLE,
			public static readonly UnicodeScript BRAILLE = new UnicodeScript("BRAILLE", InnerEnum.BRAILLE);

			/// <summary>
			/// Unicode script "Buginese".
			/// </summary>
			BUGINESE,
			public static readonly UnicodeScript BUGINESE = new UnicodeScript("BUGINESE", InnerEnum.BUGINESE);

			/// <summary>
			/// Unicode script "Coptic".
			/// </summary>
			COPTIC,
			public static readonly UnicodeScript COPTIC = new UnicodeScript("COPTIC", InnerEnum.COPTIC);

			/// <summary>
			/// Unicode script "New_Tai_Lue".
			/// </summary>
			NEW_TAI_LUE,
			public static readonly UnicodeScript NEW_TAI_LUE = new UnicodeScript("NEW_TAI_LUE", InnerEnum.NEW_TAI_LUE);

			/// <summary>
			/// Unicode script "Glagolitic".
			/// </summary>
			GLAGOLITIC,
			public static readonly UnicodeScript GLAGOLITIC = new UnicodeScript("GLAGOLITIC", InnerEnum.GLAGOLITIC);

			/// <summary>
			/// Unicode script "Tifinagh".
			/// </summary>
			TIFINAGH,
			public static readonly UnicodeScript TIFINAGH = new UnicodeScript("TIFINAGH", InnerEnum.TIFINAGH);

			/// <summary>
			/// Unicode script "Syloti_Nagri".
			/// </summary>
			SYLOTI_NAGRI,
			public static readonly UnicodeScript SYLOTI_NAGRI = new UnicodeScript("SYLOTI_NAGRI", InnerEnum.SYLOTI_NAGRI);

			/// <summary>
			/// Unicode script "Old_Persian".
			/// </summary>
			OLD_PERSIAN,
			public static readonly UnicodeScript OLD_PERSIAN = new UnicodeScript("OLD_PERSIAN", InnerEnum.OLD_PERSIAN);

			/// <summary>
			/// Unicode script "Kharoshthi".
			/// </summary>
			KHAROSHTHI,
			public static readonly UnicodeScript KHAROSHTHI = new UnicodeScript("KHAROSHTHI", InnerEnum.KHAROSHTHI);

			/// <summary>
			/// Unicode script "Balinese".
			/// </summary>
			BALINESE,
			public static readonly UnicodeScript BALINESE = new UnicodeScript("BALINESE", InnerEnum.BALINESE);

			/// <summary>
			/// Unicode script "Cuneiform".
			/// </summary>
			CUNEIFORM,
			public static readonly UnicodeScript CUNEIFORM = new UnicodeScript("CUNEIFORM", InnerEnum.CUNEIFORM);

			/// <summary>
			/// Unicode script "Phoenician".
			/// </summary>
			PHOENICIAN,
			public static readonly UnicodeScript PHOENICIAN = new UnicodeScript("PHOENICIAN", InnerEnum.PHOENICIAN);

			/// <summary>
			/// Unicode script "Phags_Pa".
			/// </summary>
			PHAGS_PA,
			public static readonly UnicodeScript PHAGS_PA = new UnicodeScript("PHAGS_PA", InnerEnum.PHAGS_PA);

			/// <summary>
			/// Unicode script "Nko".
			/// </summary>
			NKO,
			public static readonly UnicodeScript NKO = new UnicodeScript("NKO", InnerEnum.NKO);

			/// <summary>
			/// Unicode script "Sundanese".
			/// </summary>
			SUNDANESE,
			public static readonly UnicodeScript SUNDANESE = new UnicodeScript("SUNDANESE", InnerEnum.SUNDANESE);

			/// <summary>
			/// Unicode script "Batak".
			/// </summary>
			BATAK,
			public static readonly UnicodeScript BATAK = new UnicodeScript("BATAK", InnerEnum.BATAK);

			/// <summary>
			/// Unicode script "Lepcha".
			/// </summary>
			LEPCHA,
			public static readonly UnicodeScript LEPCHA = new UnicodeScript("LEPCHA", InnerEnum.LEPCHA);

			/// <summary>
			/// Unicode script "Ol_Chiki".
			/// </summary>
			OL_CHIKI,
			public static readonly UnicodeScript OL_CHIKI = new UnicodeScript("OL_CHIKI", InnerEnum.OL_CHIKI);

			/// <summary>
			/// Unicode script "Vai".
			/// </summary>
			VAI,
			public static readonly UnicodeScript VAI = new UnicodeScript("VAI", InnerEnum.VAI);

			/// <summary>
			/// Unicode script "Saurashtra".
			/// </summary>
			SAURASHTRA,
			public static readonly UnicodeScript SAURASHTRA = new UnicodeScript("SAURASHTRA", InnerEnum.SAURASHTRA);

			/// <summary>
			/// Unicode script "Kayah_Li".
			/// </summary>
			KAYAH_LI,
			public static readonly UnicodeScript KAYAH_LI = new UnicodeScript("KAYAH_LI", InnerEnum.KAYAH_LI);

			/// <summary>
			/// Unicode script "Rejang".
			/// </summary>
			REJANG,
			public static readonly UnicodeScript REJANG = new UnicodeScript("REJANG", InnerEnum.REJANG);

			/// <summary>
			/// Unicode script "Lycian".
			/// </summary>
			LYCIAN,
			public static readonly UnicodeScript LYCIAN = new UnicodeScript("LYCIAN", InnerEnum.LYCIAN);

			/// <summary>
			/// Unicode script "Carian".
			/// </summary>
			CARIAN,
			public static readonly UnicodeScript CARIAN = new UnicodeScript("CARIAN", InnerEnum.CARIAN);

			/// <summary>
			/// Unicode script "Lydian".
			/// </summary>
			LYDIAN,
			public static readonly UnicodeScript LYDIAN = new UnicodeScript("LYDIAN", InnerEnum.LYDIAN);

			/// <summary>
			/// Unicode script "Cham".
			/// </summary>
			CHAM,
			public static readonly UnicodeScript CHAM = new UnicodeScript("CHAM", InnerEnum.CHAM);

			/// <summary>
			/// Unicode script "Tai_Tham".
			/// </summary>
			TAI_THAM,
			public static readonly UnicodeScript TAI_THAM = new UnicodeScript("TAI_THAM", InnerEnum.TAI_THAM);

			/// <summary>
			/// Unicode script "Tai_Viet".
			/// </summary>
			TAI_VIET,
			public static readonly UnicodeScript TAI_VIET = new UnicodeScript("TAI_VIET", InnerEnum.TAI_VIET);

			/// <summary>
			/// Unicode script "Avestan".
			/// </summary>
			AVESTAN,
			public static readonly UnicodeScript AVESTAN = new UnicodeScript("AVESTAN", InnerEnum.AVESTAN);

			/// <summary>
			/// Unicode script "Egyptian_Hieroglyphs".
			/// </summary>
			EGYPTIAN_HIEROGLYPHS,
			public static readonly UnicodeScript EGYPTIAN_HIEROGLYPHS = new UnicodeScript("EGYPTIAN_HIEROGLYPHS", InnerEnum.EGYPTIAN_HIEROGLYPHS);

			/// <summary>
			/// Unicode script "Samaritan".
			/// </summary>
			SAMARITAN,
			public static readonly UnicodeScript SAMARITAN = new UnicodeScript("SAMARITAN", InnerEnum.SAMARITAN);

			/// <summary>
			/// Unicode script "Mandaic".
			/// </summary>
			MANDAIC,
			public static readonly UnicodeScript MANDAIC = new UnicodeScript("MANDAIC", InnerEnum.MANDAIC);

			/// <summary>
			/// Unicode script "Lisu".
			/// </summary>
			LISU,
			public static readonly UnicodeScript LISU = new UnicodeScript("LISU", InnerEnum.LISU);

			/// <summary>
			/// Unicode script "Bamum".
			/// </summary>
			BAMUM,
			public static readonly UnicodeScript BAMUM = new UnicodeScript("BAMUM", InnerEnum.BAMUM);

			/// <summary>
			/// Unicode script "Javanese".
			/// </summary>
			JAVANESE,
			public static readonly UnicodeScript JAVANESE = new UnicodeScript("JAVANESE", InnerEnum.JAVANESE);

			/// <summary>
			/// Unicode script "Meetei_Mayek".
			/// </summary>
			MEETEI_MAYEK,
			public static readonly UnicodeScript MEETEI_MAYEK = new UnicodeScript("MEETEI_MAYEK", InnerEnum.MEETEI_MAYEK);

			/// <summary>
			/// Unicode script "Imperial_Aramaic".
			/// </summary>
			IMPERIAL_ARAMAIC,
			public static readonly UnicodeScript IMPERIAL_ARAMAIC = new UnicodeScript("IMPERIAL_ARAMAIC", InnerEnum.IMPERIAL_ARAMAIC);

			/// <summary>
			/// Unicode script "Old_South_Arabian".
			/// </summary>
			OLD_SOUTH_ARABIAN,
			public static readonly UnicodeScript OLD_SOUTH_ARABIAN = new UnicodeScript("OLD_SOUTH_ARABIAN", InnerEnum.OLD_SOUTH_ARABIAN);

			/// <summary>
			/// Unicode script "Inscriptional_Parthian".
			/// </summary>
			INSCRIPTIONAL_PARTHIAN,
			public static readonly UnicodeScript INSCRIPTIONAL_PARTHIAN = new UnicodeScript("INSCRIPTIONAL_PARTHIAN", InnerEnum.INSCRIPTIONAL_PARTHIAN);

			/// <summary>
			/// Unicode script "Inscriptional_Pahlavi".
			/// </summary>
			INSCRIPTIONAL_PAHLAVI,
			public static readonly UnicodeScript INSCRIPTIONAL_PAHLAVI = new UnicodeScript("INSCRIPTIONAL_PAHLAVI", InnerEnum.INSCRIPTIONAL_PAHLAVI);

			/// <summary>
			/// Unicode script "Old_Turkic".
			/// </summary>
			OLD_TURKIC,
			public static readonly UnicodeScript OLD_TURKIC = new UnicodeScript("OLD_TURKIC", InnerEnum.OLD_TURKIC);

			/// <summary>
			/// Unicode script "Brahmi".
			/// </summary>
			BRAHMI,
			public static readonly UnicodeScript BRAHMI = new UnicodeScript("BRAHMI", InnerEnum.BRAHMI);

			/// <summary>
			/// Unicode script "Kaithi".
			/// </summary>
			KAITHI,
			public static readonly UnicodeScript KAITHI = new UnicodeScript("KAITHI", InnerEnum.KAITHI);

			/// <summary>
			/// Unicode script "Meroitic Hieroglyphs".
			/// </summary>
			MEROITIC_HIEROGLYPHS,
			public static readonly UnicodeScript MEROITIC_HIEROGLYPHS = new UnicodeScript("MEROITIC_HIEROGLYPHS", InnerEnum.MEROITIC_HIEROGLYPHS);

			/// <summary>
			/// Unicode script "Meroitic Cursive".
			/// </summary>
			MEROITIC_CURSIVE,
			public static readonly UnicodeScript MEROITIC_CURSIVE = new UnicodeScript("MEROITIC_CURSIVE", InnerEnum.MEROITIC_CURSIVE);

			/// <summary>
			/// Unicode script "Sora Sompeng".
			/// </summary>
			SORA_SOMPENG,
			public static readonly UnicodeScript SORA_SOMPENG = new UnicodeScript("SORA_SOMPENG", InnerEnum.SORA_SOMPENG);

			/// <summary>
			/// Unicode script "Chakma".
			/// </summary>
			CHAKMA,
			public static readonly UnicodeScript CHAKMA = new UnicodeScript("CHAKMA", InnerEnum.CHAKMA);

			/// <summary>
			/// Unicode script "Sharada".
			/// </summary>
			SHARADA,
			public static readonly UnicodeScript SHARADA = new UnicodeScript("SHARADA", InnerEnum.SHARADA);

			/// <summary>
			/// Unicode script "Takri".
			/// </summary>
			TAKRI,
			public static readonly UnicodeScript TAKRI = new UnicodeScript("TAKRI", InnerEnum.TAKRI);

			/// <summary>
			/// Unicode script "Miao".
			/// </summary>
			MIAO,
			public static readonly UnicodeScript MIAO = new UnicodeScript("MIAO", InnerEnum.MIAO);

			/// <summary>
			/// Unicode script "Unknown".
			/// </summary>
			UNKNOWN
			public static readonly UnicodeScript UNKNOWN = new UnicodeScript("UNKNOWN", InnerEnum.UNKNOWN);

			private static readonly IList<UnicodeScript> valueList = new List<UnicodeScript>();

			static UnicodeScript()
			{
				valueList.Add(COMMON);
				valueList.Add(LATIN);
				valueList.Add(GREEK);
				valueList.Add(CYRILLIC);
				valueList.Add(ARMENIAN);
				valueList.Add(HEBREW);
				valueList.Add(ARABIC);
				valueList.Add(SYRIAC);
				valueList.Add(THAANA);
				valueList.Add(DEVANAGARI);
				valueList.Add(BENGALI);
				valueList.Add(GURMUKHI);
				valueList.Add(GUJARATI);
				valueList.Add(ORIYA);
				valueList.Add(TAMIL);
				valueList.Add(TELUGU);
				valueList.Add(KANNADA);
				valueList.Add(MALAYALAM);
				valueList.Add(SINHALA);
				valueList.Add(THAI);
				valueList.Add(LAO);
				valueList.Add(TIBETAN);
				valueList.Add(MYANMAR);
				valueList.Add(GEORGIAN);
				valueList.Add(HANGUL);
				valueList.Add(ETHIOPIC);
				valueList.Add(CHEROKEE);
				valueList.Add(CANADIAN_ABORIGINAL);
				valueList.Add(OGHAM);
				valueList.Add(RUNIC);
				valueList.Add(KHMER);
				valueList.Add(MONGOLIAN);
				valueList.Add(HIRAGANA);
				valueList.Add(KATAKANA);
				valueList.Add(BOPOMOFO);
				valueList.Add(HAN);
				valueList.Add(YI);
				valueList.Add(OLD_ITALIC);
				valueList.Add(GOTHIC);
				valueList.Add(DESERET);
				valueList.Add(INHERITED);
				valueList.Add(TAGALOG);
				valueList.Add(HANUNOO);
				valueList.Add(BUHID);
				valueList.Add(TAGBANWA);
				valueList.Add(LIMBU);
				valueList.Add(TAI_LE);
				valueList.Add(LINEAR_B);
				valueList.Add(UGARITIC);
				valueList.Add(SHAVIAN);
				valueList.Add(OSMANYA);
				valueList.Add(CYPRIOT);
				valueList.Add(BRAILLE);
				valueList.Add(BUGINESE);
				valueList.Add(COPTIC);
				valueList.Add(NEW_TAI_LUE);
				valueList.Add(GLAGOLITIC);
				valueList.Add(TIFINAGH);
				valueList.Add(SYLOTI_NAGRI);
				valueList.Add(OLD_PERSIAN);
				valueList.Add(KHAROSHTHI);
				valueList.Add(BALINESE);
				valueList.Add(CUNEIFORM);
				valueList.Add(PHOENICIAN);
				valueList.Add(PHAGS_PA);
				valueList.Add(NKO);
				valueList.Add(SUNDANESE);
				valueList.Add(BATAK);
				valueList.Add(LEPCHA);
				valueList.Add(OL_CHIKI);
				valueList.Add(VAI);
				valueList.Add(SAURASHTRA);
				valueList.Add(KAYAH_LI);
				valueList.Add(REJANG);
				valueList.Add(LYCIAN);
				valueList.Add(CARIAN);
				valueList.Add(LYDIAN);
				valueList.Add(CHAM);
				valueList.Add(TAI_THAM);
				valueList.Add(TAI_VIET);
				valueList.Add(AVESTAN);
				valueList.Add(EGYPTIAN_HIEROGLYPHS);
				valueList.Add(SAMARITAN);
				valueList.Add(MANDAIC);
				valueList.Add(LISU);
				valueList.Add(BAMUM);
				valueList.Add(JAVANESE);
				valueList.Add(MEETEI_MAYEK);
				valueList.Add(IMPERIAL_ARAMAIC);
				valueList.Add(OLD_SOUTH_ARABIAN);
				valueList.Add(INSCRIPTIONAL_PARTHIAN);
				valueList.Add(INSCRIPTIONAL_PAHLAVI);
				valueList.Add(OLD_TURKIC);
				valueList.Add(BRAHMI);
				valueList.Add(KAITHI);
				valueList.Add(MEROITIC_HIEROGLYPHS);
				valueList.Add(MEROITIC_CURSIVE);
				valueList.Add(SORA_SOMPENG);
				valueList.Add(CHAKMA);
				valueList.Add(SHARADA);
				valueList.Add(TAKRI);
				valueList.Add(MIAO);
				valueList.Add(UNKNOWN);
			}

			public enum InnerEnum
			{
				COMMON,
				LATIN,
				GREEK,
				CYRILLIC,
				ARMENIAN,
				HEBREW,
				ARABIC,
				SYRIAC,
				THAANA,
				DEVANAGARI,
				BENGALI,
				GURMUKHI,
				GUJARATI,
				ORIYA,
				TAMIL,
				TELUGU,
				KANNADA,
				MALAYALAM,
				SINHALA,
				THAI,
				LAO,
				TIBETAN,
				MYANMAR,
				GEORGIAN,
				HANGUL,
				ETHIOPIC,
				CHEROKEE,
				CANADIAN_ABORIGINAL,
				OGHAM,
				RUNIC,
				KHMER,
				MONGOLIAN,
				HIRAGANA,
				KATAKANA,
				BOPOMOFO,
				HAN,
				YI,
				OLD_ITALIC,
				GOTHIC,
				DESERET,
				INHERITED,
				TAGALOG,
				HANUNOO,
				BUHID,
				TAGBANWA,
				LIMBU,
				TAI_LE,
				LINEAR_B,
				UGARITIC,
				SHAVIAN,
				OSMANYA,
				CYPRIOT,
				BRAILLE,
				BUGINESE,
				COPTIC,
				NEW_TAI_LUE,
				GLAGOLITIC,
				TIFINAGH,
				SYLOTI_NAGRI,
				OLD_PERSIAN,
				KHAROSHTHI,
				BALINESE,
				CUNEIFORM,
				PHOENICIAN,
				PHAGS_PA,
				NKO,
				SUNDANESE,
				BATAK,
				LEPCHA,
				OL_CHIKI,
				VAI,
				SAURASHTRA,
				KAYAH_LI,
				REJANG,
				LYCIAN,
				CARIAN,
				LYDIAN,
				CHAM,
				TAI_THAM,
				TAI_VIET,
				AVESTAN,
				EGYPTIAN_HIEROGLYPHS,
				SAMARITAN,
				MANDAIC,
				LISU,
				BAMUM,
				JAVANESE,
				MEETEI_MAYEK,
				IMPERIAL_ARAMAIC,
				OLD_SOUTH_ARABIAN,
				INSCRIPTIONAL_PARTHIAN,
				INSCRIPTIONAL_PAHLAVI,
				OLD_TURKIC,
				BRAHMI,
				KAITHI,
				MEROITIC_HIEROGLYPHS,
				MEROITIC_CURSIVE,
				SORA_SOMPENG,
				CHAKMA,
				SHARADA,
				TAKRI,
				MIAO,
				UNKNOWN
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			private static final int[] scriptStarts = { 0x0000, 0x0041, 0x005B, 0x0061, 0x007B, 0x00AA, 0x00AB, 0x00BA, 0x00BB, 0x00C0, 0x00D7, 0x00D8, 0x00F7, 0x00F8, 0x02B9, 0x02E0, 0x02E5, 0x02EA, 0x02EC, 0x0300, 0x0370, 0x0374, 0x0375, 0x037E, 0x0384, 0x0385, 0x0386, 0x0387, 0x0388, 0x03E2, 0x03F0, 0x0400, 0x0485, 0x0487, 0x0531, 0x0589, 0x058A, 0x0591, 0x0600, 0x060C, 0x060D, 0x061B, 0x061E, 0x061F, 0x0620, 0x0640, 0x0641, 0x064B, 0x0656, 0x0660, 0x066A, 0x0670, 0x0671, 0x06DD, 0x06DE, 0x0700, 0x0750, 0x0780, 0x07C0, 0x0800, 0x0840, 0x08A0, 0x0900, 0x0951, 0x0953, 0x0964, 0x0966, 0x0981, 0x0A01, 0x0A81, 0x0B01, 0x0B82, 0x0C01, 0x0C82, 0x0D02, 0x0D82, 0x0E01, 0x0E3F, 0x0E40, 0x0E81, 0x0F00, 0x0FD5, 0x0FD9, 0x1000, 0x10A0, 0x10FB, 0x10FC, 0x1100, 0x1200, 0x13A0, 0x1400, 0x1680, 0x16A0, 0x16EB, 0x16EE, 0x1700, 0x1720, 0x1735, 0x1740, 0x1760, 0x1780, 0x1800, 0x1802, 0x1804, 0x1805, 0x1806, 0x18B0, 0x1900, 0x1950, 0x1980, 0x19E0, 0x1A00, 0x1A20, 0x1B00, 0x1B80, 0x1BC0, 0x1C00, 0x1C50, 0x1CC0, 0x1CD0, 0x1CD3, 0x1CD4, 0x1CE1, 0x1CE2, 0x1CE9, 0x1CED, 0x1CEE, 0x1CF4, 0x1CF5, 0x1D00, 0x1D26, 0x1D2B, 0x1D2C, 0x1D5D, 0x1D62, 0x1D66, 0x1D6B, 0x1D78, 0x1D79, 0x1DBF, 0x1DC0, 0x1E00, 0x1F00, 0x2000, 0x200C, 0x200E, 0x2071, 0x2074, 0x207F, 0x2080, 0x2090, 0x20A0, 0x20D0, 0x2100, 0x2126, 0x2127, 0x212A, 0x212C, 0x2132, 0x2133, 0x214E, 0x214F, 0x2160, 0x2189, 0x2800, 0x2900, 0x2C00, 0x2C60, 0x2C80, 0x2D00, 0x2D30, 0x2D80, 0x2DE0, 0x2E00, 0x2E80, 0x2FF0, 0x3005, 0x3006, 0x3007, 0x3008, 0x3021, 0x302A, 0x302E, 0x3030, 0x3038, 0x303C, 0x3041, 0x3099, 0x309B, 0x309D, 0x30A0, 0x30A1, 0x30FB, 0x30FD, 0x3105, 0x3131, 0x3190, 0x31A0, 0x31C0, 0x31F0, 0x3200, 0x3220, 0x3260, 0x327F, 0x32D0, 0x3358, 0x3400, 0x4DC0, 0x4E00, 0xA000, 0xA4D0, 0xA500, 0xA640, 0xA6A0, 0xA700, 0xA722, 0xA788, 0xA78B, 0xA800, 0xA830, 0xA840, 0xA880, 0xA8E0, 0xA900, 0xA930, 0xA960, 0xA980, 0xAA00, 0xAA60, 0xAA80, 0xAAE0, 0xAB01, 0xABC0, 0xAC00, 0xD7FC, 0xF900, 0xFB00, 0xFB13, 0xFB1D, 0xFB50, 0xFD3E, 0xFD50, 0xFDFD, 0xFE00, 0xFE10, 0xFE20, 0xFE30, 0xFE70, 0xFEFF, 0xFF21, 0xFF3B, 0xFF41, 0xFF5B, 0xFF66, 0xFF70, 0xFF71, 0xFF9E, 0xFFA0, 0xFFE0, 0x10000, 0x10100, 0x10140, 0x10190, 0x101FD, 0x10280, 0x102A0, 0x10300, 0x10330, 0x10380, 0x103A0, 0x10400, 0x10450, 0x10480, 0x10800, 0x10840, 0x10900, 0x10920, 0x10980, 0x109A0, 0x10A00, 0x10A60, 0x10B00, 0x10B40, 0x10B60, 0x10C00, 0x10E60, 0x11000, 0x11080, 0x110D0, 0x11100, 0x11180, 0x11680, 0x12000, 0x13000, 0x16800, 0x16F00, 0x1B000, 0x1B001, 0x1D000, 0x1D167, 0x1D16A, 0x1D17B, 0x1D183, 0x1D185, 0x1D18C, 0x1D1AA, 0x1D1AE, 0x1D200, 0x1D300, 0x1EE00, 0x1F000, 0x1F200, 0x1F201, 0x20000, 0xE0001, 0xE0100, 0xE01F0
			public static readonly UnicodeScript private static final int[] scriptStarts = { 0x0000, 0x0041, 0x005B, 0x0061, 0x007B, 0x00AA, 0x00AB, 0x00BA, 0x00BB, 0x00C0, 0x00D7, 0x00D8, 0x00F7, 0x00F8, 0x02B9, 0x02E0, 0x02E5, 0x02EA, 0x02EC, 0x0300, 0x0370, 0x0374, 0x0375, 0x037E, 0x0384, 0x0385, 0x0386, 0x0387, 0x0388, 0x03E2, 0x03F0, 0x0400, 0x0485, 0x0487, 0x0531, 0x0589, 0x058A, 0x0591, 0x0600, 0x060C, 0x060D, 0x061B, 0x061E, 0x061F, 0x0620, 0x0640, 0x0641, 0x064B, 0x0656, 0x0660, 0x066A, 0x0670, 0x0671, 0x06DD, 0x06DE, 0x0700, 0x0750, 0x0780, 0x07C0, 0x0800, 0x0840, 0x08A0, 0x0900, 0x0951, 0x0953, 0x0964, 0x0966, 0x0981, 0x0A01, 0x0A81, 0x0B01, 0x0B82, 0x0C01, 0x0C82, 0x0D02, 0x0D82, 0x0E01, 0x0E3F, 0x0E40, 0x0E81, 0x0F00, 0x0FD5, 0x0FD9, 0x1000, 0x10A0, 0x10FB, 0x10FC, 0x1100, 0x1200, 0x13A0, 0x1400, 0x1680, 0x16A0, 0x16EB, 0x16EE, 0x1700, 0x1720, 0x1735, 0x1740, 0x1760, 0x1780, 0x1800, 0x1802, 0x1804, 0x1805, 0x1806, 0x18B0, 0x1900, 0x1950, 0x1980, 0x19E0, 0x1A00, 0x1A20, 0x1B00, 0x1B80, 0x1BC0, 0x1C00, 0x1C50, 0x1CC0, 0x1CD0, 0x1CD3, 0x1CD4, 0x1CE1, 0x1CE2, 0x1CE9, 0x1CED, 0x1CEE, 0x1CF4, 0x1CF5, 0x1D00, 0x1D26, 0x1D2B, 0x1D2C, 0x1D5D, 0x1D62, 0x1D66, 0x1D6B, 0x1D78, 0x1D79, 0x1DBF, 0x1DC0, 0x1E00, 0x1F00, 0x2000, 0x200C, 0x200E, 0x2071, 0x2074, 0x207F, 0x2080, 0x2090, 0x20A0, 0x20D0, 0x2100, 0x2126, 0x2127, 0x212A, 0x212C, 0x2132, 0x2133, 0x214E, 0x214F, 0x2160, 0x2189, 0x2800, 0x2900, 0x2C00, 0x2C60, 0x2C80, 0x2D00, 0x2D30, 0x2D80, 0x2DE0, 0x2E00, 0x2E80, 0x2FF0, 0x3005, 0x3006, 0x3007, 0x3008, 0x3021, 0x302A, 0x302E, 0x3030, 0x3038, 0x303C, 0x3041, 0x3099, 0x309B, 0x309D, 0x30A0, 0x30A1, 0x30FB, 0x30FD, 0x3105, 0x3131, 0x3190, 0x31A0, 0x31C0, 0x31F0, 0x3200, 0x3220, 0x3260, 0x327F, 0x32D0, 0x3358, 0x3400, 0x4DC0, 0x4E00, 0xA000, 0xA4D0, 0xA500, 0xA640, 0xA6A0, 0xA700, 0xA722, 0xA788, 0xA78B, 0xA800, 0xA830, 0xA840, 0xA880, 0xA8E0, 0xA900, 0xA930, 0xA960, 0xA980, 0xAA00, 0xAA60, 0xAA80, 0xAAE0, 0xAB01, 0xABC0, 0xAC00, 0xD7FC, 0xF900, 0xFB00, 0xFB13, 0xFB1D, 0xFB50, 0xFD3E, 0xFD50, 0xFDFD, 0xFE00, 0xFE10, 0xFE20, 0xFE30, 0xFE70, 0xFEFF, 0xFF21, 0xFF3B, 0xFF41, 0xFF5B, 0xFF66, 0xFF70, 0xFF71, 0xFF9E, 0xFFA0, 0xFFE0, 0x10000, 0x10100, 0x10140, 0x10190, 0x101FD, 0x10280, 0x102A0, 0x10300, 0x10330, 0x10380, 0x103A0, 0x10400, 0x10450, 0x10480, 0x10800, 0x10840, 0x10900, 0x10920, 0x10980, 0x109A0, 0x10A00, 0x10A60, 0x10B00, 0x10B40, 0x10B60, 0x10C00, 0x10E60, 0x11000, 0x11080, 0x110D0, 0x11100, 0x11180, 0x11680, 0x12000, 0x13000, 0x16800, 0x16F00, 0x1B000, 0x1B001, 0x1D000, 0x1D167, 0x1D16A, 0x1D17B, 0x1D183, 0x1D185, 0x1D18C, 0x1D1AA, 0x1D1AE, 0x1D200, 0x1D300, 0x1EE00, 0x1F000, 0x1F200, 0x1F201, 0x20000, 0xE0001, 0xE0100, 0xE01F0 = new UnicodeScript("private static final int[] scriptStarts = { 0x0000, 0x0041, 0x005B, 0x0061, 0x007B, 0x00AA, 0x00AB, 0x00BA, 0x00BB, 0x00C0, 0x00D7, 0x00D8, 0x00F7, 0x00F8, 0x02B9, 0x02E0, 0x02E5, 0x02EA, 0x02EC, 0x0300, 0x0370, 0x0374, 0x0375, 0x037E, 0x0384, 0x0385, 0x0386, 0x0387, 0x0388, 0x03E2, 0x03F0, 0x0400, 0x0485, 0x0487, 0x0531, 0x0589, 0x058A, 0x0591, 0x0600, 0x060C, 0x060D, 0x061B, 0x061E, 0x061F, 0x0620, 0x0640, 0x0641, 0x064B, 0x0656, 0x0660, 0x066A, 0x0670, 0x0671, 0x06DD, 0x06DE, 0x0700, 0x0750, 0x0780, 0x07C0, 0x0800, 0x0840, 0x08A0, 0x0900, 0x0951, 0x0953, 0x0964, 0x0966, 0x0981, 0x0A01, 0x0A81, 0x0B01, 0x0B82, 0x0C01, 0x0C82, 0x0D02, 0x0D82, 0x0E01, 0x0E3F, 0x0E40, 0x0E81, 0x0F00, 0x0FD5, 0x0FD9, 0x1000, 0x10A0, 0x10FB, 0x10FC, 0x1100, 0x1200, 0x13A0, 0x1400, 0x1680, 0x16A0, 0x16EB, 0x16EE, 0x1700, 0x1720, 0x1735, 0x1740, 0x1760, 0x1780, 0x1800, 0x1802, 0x1804, 0x1805, 0x1806, 0x18B0, 0x1900, 0x1950, 0x1980, 0x19E0, 0x1A00, 0x1A20, 0x1B00, 0x1B80, 0x1BC0, 0x1C00, 0x1C50, 0x1CC0, 0x1CD0, 0x1CD3, 0x1CD4, 0x1CE1, 0x1CE2, 0x1CE9, 0x1CED, 0x1CEE, 0x1CF4, 0x1CF5, 0x1D00, 0x1D26, 0x1D2B, 0x1D2C, 0x1D5D, 0x1D62, 0x1D66, 0x1D6B, 0x1D78, 0x1D79, 0x1DBF, 0x1DC0, 0x1E00, 0x1F00, 0x2000, 0x200C, 0x200E, 0x2071, 0x2074, 0x207F, 0x2080, 0x2090, 0x20A0, 0x20D0, 0x2100, 0x2126, 0x2127, 0x212A, 0x212C, 0x2132, 0x2133, 0x214E, 0x214F, 0x2160, 0x2189, 0x2800, 0x2900, 0x2C00, 0x2C60, 0x2C80, 0x2D00, 0x2D30, 0x2D80, 0x2DE0, 0x2E00, 0x2E80, 0x2FF0, 0x3005, 0x3006, 0x3007, 0x3008, 0x3021, 0x302A, 0x302E, 0x3030, 0x3038, 0x303C, 0x3041, 0x3099, 0x309B, 0x309D, 0x30A0, 0x30A1, 0x30FB, 0x30FD, 0x3105, 0x3131, 0x3190, 0x31A0, 0x31C0, 0x31F0, 0x3200, 0x3220, 0x3260, 0x327F, 0x32D0, 0x3358, 0x3400, 0x4DC0, 0x4E00, 0xA000, 0xA4D0, 0xA500, 0xA640, 0xA6A0, 0xA700, 0xA722, 0xA788, 0xA78B, 0xA800, 0xA830, 0xA840, 0xA880, 0xA8E0, 0xA900, 0xA930, 0xA960, 0xA980, 0xAA00, 0xAA60, 0xAA80, 0xAAE0, 0xAB01, 0xABC0, 0xAC00, 0xD7FC, 0xF900, 0xFB00, 0xFB13, 0xFB1D, 0xFB50, 0xFD3E, 0xFD50, 0xFDFD, 0xFE00, 0xFE10, 0xFE20, 0xFE30, 0xFE70, 0xFEFF, 0xFF21, 0xFF3B, 0xFF41, 0xFF5B, 0xFF66, 0xFF70, 0xFF71, 0xFF9E, 0xFFA0, 0xFFE0, 0x10000, 0x10100, 0x10140, 0x10190, 0x101FD, 0x10280, 0x102A0, 0x10300, 0x10330, 0x10380, 0x103A0, 0x10400, 0x10450, 0x10480, 0x10800, 0x10840, 0x10900, 0x10920, 0x10980, 0x109A0, 0x10A00, 0x10A60, 0x10B00, 0x10B40, 0x10B60, 0x10C00, 0x10E60, 0x11000, 0x11080, 0x110D0, 0x11100, 0x11180, 0x11680, 0x12000, 0x13000, 0x16800, 0x16F00, 0x1B000, 0x1B001, 0x1D000, 0x1D167, 0x1D16A, 0x1D17B, 0x1D183, 0x1D185, 0x1D18C, 0x1D1AA, 0x1D1AE, 0x1D200, 0x1D300, 0x1EE00, 0x1F000, 0x1F200, 0x1F201, 0x20000, 0xE0001, 0xE0100, 0xE01F0", InnerEnum.private static final int[] scriptStarts = { 0x0000, 0x0041, 0x005B, 0x0061, 0x007B, 0x00AA, 0x00AB, 0x00BA, 0x00BB, 0x00C0, 0x00D7, 0x00D8, 0x00F7, 0x00F8, 0x02B9, 0x02E0, 0x02E5, 0x02EA, 0x02EC, 0x0300, 0x0370, 0x0374, 0x0375, 0x037E, 0x0384, 0x0385, 0x0386, 0x0387, 0x0388, 0x03E2, 0x03F0, 0x0400, 0x0485, 0x0487, 0x0531, 0x0589, 0x058A, 0x0591, 0x0600, 0x060C, 0x060D, 0x061B, 0x061E, 0x061F, 0x0620, 0x0640, 0x0641, 0x064B, 0x0656, 0x0660, 0x066A, 0x0670, 0x0671, 0x06DD, 0x06DE, 0x0700, 0x0750, 0x0780, 0x07C0, 0x0800, 0x0840, 0x08A0, 0x0900, 0x0951, 0x0953, 0x0964, 0x0966, 0x0981, 0x0A01, 0x0A81, 0x0B01, 0x0B82, 0x0C01, 0x0C82, 0x0D02, 0x0D82, 0x0E01, 0x0E3F, 0x0E40, 0x0E81, 0x0F00, 0x0FD5, 0x0FD9, 0x1000, 0x10A0, 0x10FB, 0x10FC, 0x1100, 0x1200, 0x13A0, 0x1400, 0x1680, 0x16A0, 0x16EB, 0x16EE, 0x1700, 0x1720, 0x1735, 0x1740, 0x1760, 0x1780, 0x1800, 0x1802, 0x1804, 0x1805, 0x1806, 0x18B0, 0x1900, 0x1950, 0x1980, 0x19E0, 0x1A00, 0x1A20, 0x1B00, 0x1B80, 0x1BC0, 0x1C00, 0x1C50, 0x1CC0, 0x1CD0, 0x1CD3, 0x1CD4, 0x1CE1, 0x1CE2, 0x1CE9, 0x1CED, 0x1CEE, 0x1CF4, 0x1CF5, 0x1D00, 0x1D26, 0x1D2B, 0x1D2C, 0x1D5D, 0x1D62, 0x1D66, 0x1D6B, 0x1D78, 0x1D79, 0x1DBF, 0x1DC0, 0x1E00, 0x1F00, 0x2000, 0x200C, 0x200E, 0x2071, 0x2074, 0x207F, 0x2080, 0x2090, 0x20A0, 0x20D0, 0x2100, 0x2126, 0x2127, 0x212A, 0x212C, 0x2132, 0x2133, 0x214E, 0x214F, 0x2160, 0x2189, 0x2800, 0x2900, 0x2C00, 0x2C60, 0x2C80, 0x2D00, 0x2D30, 0x2D80, 0x2DE0, 0x2E00, 0x2E80, 0x2FF0, 0x3005, 0x3006, 0x3007, 0x3008, 0x3021, 0x302A, 0x302E, 0x3030, 0x3038, 0x303C, 0x3041, 0x3099, 0x309B, 0x309D, 0x30A0, 0x30A1, 0x30FB, 0x30FD, 0x3105, 0x3131, 0x3190, 0x31A0, 0x31C0, 0x31F0, 0x3200, 0x3220, 0x3260, 0x327F, 0x32D0, 0x3358, 0x3400, 0x4DC0, 0x4E00, 0xA000, 0xA4D0, 0xA500, 0xA640, 0xA6A0, 0xA700, 0xA722, 0xA788, 0xA78B, 0xA800, 0xA830, 0xA840, 0xA880, 0xA8E0, 0xA900, 0xA930, 0xA960, 0xA980, 0xAA00, 0xAA60, 0xAA80, 0xAAE0, 0xAB01, 0xABC0, 0xAC00, 0xD7FC, 0xF900, 0xFB00, 0xFB13, 0xFB1D, 0xFB50, 0xFD3E, 0xFD50, 0xFDFD, 0xFE00, 0xFE10, 0xFE20, 0xFE30, 0xFE70, 0xFEFF, 0xFF21, 0xFF3B, 0xFF41, 0xFF5B, 0xFF66, 0xFF70, 0xFF71, 0xFF9E, 0xFFA0, 0xFFE0, 0x10000, 0x10100, 0x10140, 0x10190, 0x101FD, 0x10280, 0x102A0, 0x10300, 0x10330, 0x10380, 0x103A0, 0x10400, 0x10450, 0x10480, 0x10800, 0x10840, 0x10900, 0x10920, 0x10980, 0x109A0, 0x10A00, 0x10A60, 0x10B00, 0x10B40, 0x10B60, 0x10C00, 0x10E60, 0x11000, 0x11080, 0x110D0, 0x11100, 0x11180, 0x11680, 0x12000, 0x13000, 0x16800, 0x16F00, 0x1B000, 0x1B001, 0x1D000, 0x1D167, 0x1D16A, 0x1D17B, 0x1D183, 0x1D185, 0x1D18C, 0x1D1AA, 0x1D1AE, 0x1D200, 0x1D300, 0x1EE00, 0x1F000, 0x1F200, 0x1F201, 0x20000, 0xE0001, 0xE0100, 0xE01F0);

			public static IList<UnicodeScript> values()
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

			public static UnicodeScript valueOf(string name)
			{
				foreach (UnicodeScript enumInstance in UnicodeScript.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

			private static readonly UnicodeScript[] Scripts = new UnicodeScript[] {COMMON, LATIN, COMMON, LATIN, COMMON, LATIN, COMMON, LATIN, COMMON, LATIN, COMMON, LATIN, COMMON, LATIN, COMMON, LATIN, COMMON, BOPOMOFO, COMMON, INHERITED, GREEK, COMMON, GREEK, COMMON, GREEK, COMMON, GREEK, COMMON, GREEK, COPTIC, GREEK, CYRILLIC, INHERITED, CYRILLIC, ARMENIAN, COMMON, ARMENIAN, HEBREW, ARABIC, COMMON, ARABIC, COMMON, ARABIC, COMMON, ARABIC, COMMON, ARABIC, INHERITED, ARABIC, COMMON, ARABIC, INHERITED, ARABIC, COMMON, ARABIC, SYRIAC, ARABIC, THAANA, NKO, SAMARITAN, MANDAIC, ARABIC, DEVANAGARI, INHERITED, DEVANAGARI, COMMON, DEVANAGARI, BENGALI, GURMUKHI, GUJARATI, ORIYA, TAMIL, TELUGU, KANNADA, MALAYALAM, SINHALA, THAI, COMMON, THAI, LAO, TIBETAN, COMMON, TIBETAN, MYANMAR, GEORGIAN, COMMON, GEORGIAN, HANGUL, ETHIOPIC, CHEROKEE, CANADIAN_ABORIGINAL, OGHAM, RUNIC, COMMON, RUNIC, TAGALOG, HANUNOO, COMMON, BUHID, TAGBANWA, KHMER, MONGOLIAN, COMMON, MONGOLIAN, COMMON, MONGOLIAN, CANADIAN_ABORIGINAL, LIMBU, TAI_LE, NEW_TAI_LUE, KHMER, BUGINESE, TAI_THAM, BALINESE, SUNDANESE, BATAK, LEPCHA, OL_CHIKI, SUNDANESE, INHERITED, COMMON, INHERITED, COMMON, INHERITED, COMMON, INHERITED, COMMON, INHERITED, COMMON, LATIN, GREEK, CYRILLIC, LATIN, GREEK, LATIN, GREEK, LATIN, CYRILLIC, LATIN, GREEK, INHERITED, LATIN, GREEK, COMMON, INHERITED, COMMON, LATIN, COMMON, LATIN, COMMON, LATIN, COMMON, INHERITED, COMMON, GREEK, COMMON, LATIN, COMMON, LATIN, COMMON, LATIN, COMMON, LATIN, COMMON, BRAILLE, COMMON, GLAGOLITIC, LATIN, COPTIC, GEORGIAN, TIFINAGH, ETHIOPIC, CYRILLIC, COMMON, HAN, COMMON, HAN, COMMON, HAN, COMMON, HAN, INHERITED, HANGUL, COMMON, HAN, COMMON, HIRAGANA, INHERITED, COMMON, HIRAGANA, COMMON, KATAKANA, COMMON, KATAKANA, BOPOMOFO, HANGUL, COMMON, BOPOMOFO, COMMON, KATAKANA, HANGUL, COMMON, HANGUL, COMMON, KATAKANA, COMMON, HAN, COMMON, HAN, YI, LISU, VAI, CYRILLIC, BAMUM, COMMON, LATIN, COMMON, LATIN, SYLOTI_NAGRI, COMMON, PHAGS_PA, SAURASHTRA, DEVANAGARI, KAYAH_LI, REJANG, HANGUL, JAVANESE, CHAM, MYANMAR, TAI_VIET, MEETEI_MAYEK, ETHIOPIC, MEETEI_MAYEK, HANGUL, UNKNOWN, HAN, LATIN, ARMENIAN, HEBREW, ARABIC, COMMON, ARABIC, COMMON, INHERITED, COMMON, INHERITED, COMMON, ARABIC, COMMON, LATIN, COMMON, LATIN, COMMON, KATAKANA, COMMON, KATAKANA, COMMON, HANGUL, COMMON, LINEAR_B, COMMON, GREEK, COMMON, INHERITED, LYCIAN, CARIAN, OLD_ITALIC, GOTHIC, UGARITIC, OLD_PERSIAN, DESERET, SHAVIAN, OSMANYA, CYPRIOT, IMPERIAL_ARAMAIC, PHOENICIAN, LYDIAN, MEROITIC_HIEROGLYPHS, MEROITIC_CURSIVE, KHAROSHTHI, OLD_SOUTH_ARABIAN, AVESTAN, INSCRIPTIONAL_PARTHIAN, INSCRIPTIONAL_PAHLAVI, OLD_TURKIC, ARABIC, BRAHMI, KAITHI, SORA_SOMPENG, CHAKMA, SHARADA, TAKRI, CUNEIFORM, EGYPTIAN_HIEROGLYPHS, BAMUM, MIAO, KATAKANA, HIRAGANA, COMMON, INHERITED, COMMON, INHERITED, COMMON, INHERITED, COMMON, INHERITED, COMMON, GREEK, COMMON, ARABIC, COMMON, HIRAGANA, COMMON, HAN, COMMON, INHERITED, UNKNOWN};

			private static Dictionary<String, Character.UnicodeScript> Aliases;
			static Character()
			{
				Aliases = new Dictionary<>(128);
				Aliases["ARAB"] = ARABIC;
				Aliases["ARMI"] = IMPERIAL_ARAMAIC;
				Aliases["ARMN"] = ARMENIAN;
				Aliases["AVST"] = AVESTAN;
				Aliases["BALI"] = BALINESE;
				Aliases["BAMU"] = BAMUM;
				Aliases["BATK"] = BATAK;
				Aliases["BENG"] = BENGALI;
				Aliases["BOPO"] = BOPOMOFO;
				Aliases["BRAI"] = BRAILLE;
				Aliases["BRAH"] = BRAHMI;
				Aliases["BUGI"] = BUGINESE;
				Aliases["BUHD"] = BUHID;
				Aliases["CAKM"] = CHAKMA;
				Aliases["CANS"] = CANADIAN_ABORIGINAL;
				Aliases["CARI"] = CARIAN;
				Aliases["CHAM"] = CHAM;
				Aliases["CHER"] = CHEROKEE;
				Aliases["COPT"] = COPTIC;
				Aliases["CPRT"] = CYPRIOT;
				Aliases["CYRL"] = CYRILLIC;
				Aliases["DEVA"] = DEVANAGARI;
				Aliases["DSRT"] = DESERET;
				Aliases["EGYP"] = EGYPTIAN_HIEROGLYPHS;
				Aliases["ETHI"] = ETHIOPIC;
				Aliases["GEOR"] = GEORGIAN;
				Aliases["GLAG"] = GLAGOLITIC;
				Aliases["GOTH"] = GOTHIC;
				Aliases["GREK"] = GREEK;
				Aliases["GUJR"] = GUJARATI;
				Aliases["GURU"] = GURMUKHI;
				Aliases["HANG"] = HANGUL;
				Aliases["HANI"] = HAN;
				Aliases["HANO"] = HANUNOO;
				Aliases["HEBR"] = HEBREW;
				Aliases["HIRA"] = HIRAGANA;
				// it appears we don't have the KATAKANA_OR_HIRAGANA
				//aliases.put("HRKT", KATAKANA_OR_HIRAGANA);
				Aliases["ITAL"] = OLD_ITALIC;
				Aliases["JAVA"] = JAVANESE;
				Aliases["KALI"] = KAYAH_LI;
				Aliases["KANA"] = KATAKANA;
				Aliases["KHAR"] = KHAROSHTHI;
				Aliases["KHMR"] = KHMER;
				Aliases["KNDA"] = KANNADA;
				Aliases["KTHI"] = KAITHI;
				Aliases["LANA"] = TAI_THAM;
				Aliases["LAOO"] = LAO;
				Aliases["LATN"] = LATIN;
				Aliases["LEPC"] = LEPCHA;
				Aliases["LIMB"] = LIMBU;
				Aliases["LINB"] = LINEAR_B;
				Aliases["LISU"] = LISU;
				Aliases["LYCI"] = LYCIAN;
				Aliases["LYDI"] = LYDIAN;
				Aliases["MAND"] = MANDAIC;
				Aliases["MERC"] = MEROITIC_CURSIVE;
				Aliases["MERO"] = MEROITIC_HIEROGLYPHS;
				Aliases["MLYM"] = MALAYALAM;
				Aliases["MONG"] = MONGOLIAN;
				Aliases["MTEI"] = MEETEI_MAYEK;
				Aliases["MYMR"] = MYANMAR;
				Aliases["NKOO"] = NKO;
				Aliases["OGAM"] = OGHAM;
				Aliases["OLCK"] = OL_CHIKI;
				Aliases["ORKH"] = OLD_TURKIC;
				Aliases["ORYA"] = ORIYA;
				Aliases["OSMA"] = OSMANYA;
				Aliases["PHAG"] = PHAGS_PA;
				Aliases["PLRD"] = MIAO;
				Aliases["PHLI"] = INSCRIPTIONAL_PAHLAVI;
				Aliases["PHNX"] = PHOENICIAN;
				Aliases["PRTI"] = INSCRIPTIONAL_PARTHIAN;
				Aliases["RJNG"] = REJANG;
				Aliases["RUNR"] = RUNIC;
				Aliases["SAMR"] = SAMARITAN;
				Aliases["SARB"] = OLD_SOUTH_ARABIAN;
				Aliases["SAUR"] = SAURASHTRA;
				Aliases["SHAW"] = SHAVIAN;
				Aliases["SHRD"] = SHARADA;
				Aliases["SINH"] = SINHALA;
				Aliases["SORA"] = SORA_SOMPENG;
				Aliases["SUND"] = SUNDANESE;
				Aliases["SYLO"] = SYLOTI_NAGRI;
				Aliases["SYRC"] = SYRIAC;
				Aliases["TAGB"] = TAGBANWA;
				Aliases["TALE"] = TAI_LE;
				Aliases["TAKR"] = TAKRI;
				Aliases["TALU"] = NEW_TAI_LUE;
				Aliases["TAML"] = TAMIL;
				Aliases["TAVT"] = TAI_VIET;
				Aliases["TELU"] = TELUGU;
				Aliases["TFNG"] = TIFINAGH;
				Aliases["TGLG"] = TAGALOG;
				Aliases["THAA"] = THAANA;
				Aliases["THAI"] = THAI;
				Aliases["TIBT"] = TIBETAN;
				Aliases["UGAR"] = UGARITIC;
				Aliases["VAII"] = VAI;
				Aliases["XPEO"] = OLD_PERSIAN;
				Aliases["XSUX"] = CUNEIFORM;
				Aliases["YIII"] = YI;
				Aliases["ZINH"] = INHERITED;
				Aliases["ZYYY"] = COMMON;
				Aliases["ZZZZ"] = UNKNOWN;
			}

			/// <summary>
			/// Returns the enum constant representing the Unicode script of which
			/// the given character (Unicode code point) is assigned to.
			/// </summary>
			/// <param name="codePoint"> the character (Unicode code point) in question. </param>
			/// <returns>  The {@code UnicodeScript} constant representing the
			///          Unicode script of which this character is assigned to.
			/// </returns>
			/// <exception cref="IllegalArgumentException"> if the specified
			/// {@code codePoint} is an invalid Unicode code point. </exception>
			/// <seealso cref= Character#isValidCodePoint(int)
			///  </seealso>
			public static UnicodeScript Of(int codePoint)
			{
				if (!isValidCodePoint(codePoint))
				{
					throw new IllegalArgumentException();
				}
				int type = getType(codePoint);
				// leave SURROGATE and PRIVATE_USE for table lookup
				if (type == UNASSIGNED)
				{
					return UNKNOWN;
				}
				int index = Arrays.BinarySearch(scriptStarts, codePoint);
				if (index < 0)
				{
					index = -index - 2;
				}
				return Scripts[index];
			}

			/// <summary>
			/// Returns the UnicodeScript constant with the given Unicode script
			/// name or the script name alias. Script names and their aliases are
			/// determined by The Unicode Standard. The files Scripts&lt;version&gt;.txt
			/// and PropertyValueAliases&lt;version&gt;.txt define script names
			/// and the script name aliases for a particular version of the
			/// standard. The <seealso cref="Character"/> class specifies the version of
			/// the standard that it supports.
			/// <para>
			/// Character case is ignored for all of the valid script names.
			/// The en_US locale's case mapping rules are used to provide
			/// case-insensitive string comparisons for script name validation.
			/// </para>
			/// <para>
			/// 
			/// </para>
			/// </summary>
			/// <param name="scriptName"> A {@code UnicodeScript} name. </param>
			/// <returns> The {@code UnicodeScript} constant identified
			///         by {@code scriptName} </returns>
			/// <exception cref="IllegalArgumentException"> if {@code scriptName} is an
			///         invalid name </exception>
			/// <exception cref="NullPointerException"> if {@code scriptName} is null </exception>
			public static UnicodeScript ForName(String scriptName)
			{
				scriptName = scriptName.ToUpperCase(Locale.ENGLISH);
									 //.replace(' ', '_'));
				UnicodeScript sc = Aliases[scriptName];
				if (sc != null)
				{
					return sc;
				}
				return valueOf(scriptName);
			}
	}

		/// <summary>
		/// The value of the {@code Character}.
		/// 
		/// @serial
		/// </summary>
		private readonly char Value;

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		private const long SerialVersionUID = 3786198910865385080L;

		/// <summary>
		/// Constructs a newly allocated {@code Character} object that
		/// represents the specified {@code char} value.
		/// </summary>
		/// <param name="value">   the value to be represented by the
		///                  {@code Character} object. </param>
//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
//ORIGINAL LINE: public Character(char value)
		public Character(char value)
		{
			this.Value = value;
		}

		private class CharacterCache
		{
			private CharacterCache()
			{
			}

			internal static readonly Character[] Cache = new Character[127 + 1];

			static CharacterCache()
			{
				for (int i = 0; i < Cache.Length; i++)
				{
					Cache[i] = new Character((char)i);
				}
			}
		}

		/// <summary>
		/// Returns a <tt>Character</tt> instance representing the specified
		/// <tt>char</tt> value.
		/// If a new <tt>Character</tt> instance is not required, this method
		/// should generally be used in preference to the constructor
		/// <seealso cref="#Character(char)"/>, as this method is likely to yield
		/// significantly better space and time performance by caching
		/// frequently requested values.
		/// 
		/// This method will always cache values in the range {@code
		/// '\u005Cu0000'} to {@code '\u005Cu007F'}, inclusive, and may
		/// cache other values outside of this range.
		/// </summary>
		/// <param name="c"> a char value. </param>
		/// <returns> a <tt>Character</tt> instance representing <tt>c</tt>.
		/// @since  1.5 </returns>
		public static Character ValueOf(char c)
		{
			if (c <= 127) // must cache
			{
				return CharacterCache.Cache[(int)c];
			}
			return new Character(c);
		}

		/// <summary>
		/// Returns the value of this {@code Character} object. </summary>
		/// <returns>  the primitive {@code char} value represented by
		///          this object. </returns>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		public char charValue()
		{
			return Value;
		}

		/// <summary>
		/// Returns a hash code for this {@code Character}; equal to the result
		/// of invoking {@code charValue()}.
		/// </summary>
		/// <returns> a hash code value for this {@code Character} </returns>
		public int HashCode()
		{
			return Character.HashCode(Value);
		}

		/// <summary>
		/// Returns a hash code for a {@code char} value; compatible with
		/// {@code Character.hashCode()}.
		/// 
		/// @since 1.8
		/// </summary>
		/// <param name="value"> The {@code char} for which to return a hash code. </param>
		/// <returns> a hash code value for a {@code char} value. </returns>
		public static int HashCode(char Value)
		{
			return (int)Value;
		}

		/// <summary>
		/// Compares this object against the specified object.
		/// The result is {@code true} if and only if the argument is not
		/// {@code null} and is a {@code Character} object that
		/// represents the same {@code char} value as this object.
		/// </summary>
		/// <param name="obj">   the object to compare with. </param>
		/// <returns>  {@code true} if the objects are the same;
		///          {@code false} otherwise. </returns>
		public bool Equals(Object obj)
		{
			if (obj is Character)
			{
				return Value == ((Character)obj).CharValue();
			}
			return false;
		}

		/// <summary>
		/// Returns a {@code String} object representing this
		/// {@code Character}'s value.  The result is a string of
		/// length 1 whose sole component is the primitive
		/// {@code char} value represented by this
		/// {@code Character} object.
		/// </summary>
		/// <returns>  a string representation of this object. </returns>
		public String ToString()
		{
			char[] buf = new char[] {Value};
			return Convert.ToString(buf);
		}

		/// <summary>
		/// Returns a {@code String} object representing the
		/// specified {@code char}.  The result is a string of length
		/// 1 consisting solely of the specified {@code char}.
		/// </summary>
		/// <param name="c"> the {@code char} to be converted </param>
		/// <returns> the string representation of the specified {@code char}
		/// @since 1.4 </returns>
		public static String ToString(char c)
		{
			return Convert.ToString(c);
		}

		/// <summary>
		/// Determines whether the specified code point is a valid
		/// <a href="http://www.unicode.org/glossary/#code_point">
		/// Unicode code point value</a>.
		/// </summary>
		/// <param name="codePoint"> the Unicode code point to be tested </param>
		/// <returns> {@code true} if the specified code point value is between
		///         <seealso cref="#MIN_CODE_POINT"/> and
		///         <seealso cref="#MAX_CODE_POINT"/> inclusive;
		///         {@code false} otherwise.
		/// @since  1.5 </returns>
		public static bool IsValidCodePoint(int codePoint)
		{
			// Optimized form of:
			//     codePoint >= MIN_CODE_POINT && codePoint <= MAX_CODE_POINT
			int plane = (int)((uint)codePoint >> 16);
			return plane < ((int)((uint)(MAX_CODE_POINT + 1) >> 16));
		}

		/// <summary>
		/// Determines whether the specified character (Unicode code point)
		/// is in the <a href="#BMP">Basic Multilingual Plane (BMP)</a>.
		/// Such code points can be represented using a single {@code char}.
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested </param>
		/// <returns> {@code true} if the specified code point is between
		///         <seealso cref="#MIN_VALUE"/> and <seealso cref="#MAX_VALUE"/> inclusive;
		///         {@code false} otherwise.
		/// @since  1.7 </returns>
		public static bool IsBmpCodePoint(int codePoint)
		{
			return (int)((uint)codePoint >> 16) == 0;
			// Optimized form of:
			//     codePoint >= MIN_VALUE && codePoint <= MAX_VALUE
			// We consistently use logical shift (>>>) to facilitate
			// additional runtime optimizations.
		}

		/// <summary>
		/// Determines whether the specified character (Unicode code point)
		/// is in the <a href="#supplementary">supplementary character</a> range.
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested </param>
		/// <returns> {@code true} if the specified code point is between
		///         <seealso cref="#MIN_SUPPLEMENTARY_CODE_POINT"/> and
		///         <seealso cref="#MAX_CODE_POINT"/> inclusive;
		///         {@code false} otherwise.
		/// @since  1.5 </returns>
		public static bool IsSupplementaryCodePoint(int codePoint)
		{
			return codePoint >= MIN_SUPPLEMENTARY_CODE_POINT && codePoint < MAX_CODE_POINT + 1;
		}

		/// <summary>
		/// Determines if the given {@code char} value is a
		/// <a href="http://www.unicode.org/glossary/#high_surrogate_code_unit">
		/// Unicode high-surrogate code unit</a>
		/// (also known as <i>leading-surrogate code unit</i>).
		/// 
		/// <para>Such values do not represent characters by themselves,
		/// but are used in the representation of
		/// <a href="#supplementary">supplementary characters</a>
		/// in the UTF-16 encoding.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> the {@code char} value to be tested. </param>
		/// <returns> {@code true} if the {@code char} value is between
		///         <seealso cref="#MIN_HIGH_SURROGATE"/> and
		///         <seealso cref="#MAX_HIGH_SURROGATE"/> inclusive;
		///         {@code false} otherwise. </returns>
		/// <seealso cref=    Character#isLowSurrogate(char) </seealso>
		/// <seealso cref=    Character.UnicodeBlock#of(int)
		/// @since  1.5 </seealso>
		public static bool IsHighSurrogate(char ch)
		{
			// Help VM constant-fold; MAX_HIGH_SURROGATE + 1 == MIN_LOW_SURROGATE
			return ch >= MIN_HIGH_SURROGATE && ch < (MAX_HIGH_SURROGATE + 1);
		}

		/// <summary>
		/// Determines if the given {@code char} value is a
		/// <a href="http://www.unicode.org/glossary/#low_surrogate_code_unit">
		/// Unicode low-surrogate code unit</a>
		/// (also known as <i>trailing-surrogate code unit</i>).
		/// 
		/// <para>Such values do not represent characters by themselves,
		/// but are used in the representation of
		/// <a href="#supplementary">supplementary characters</a>
		/// in the UTF-16 encoding.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> the {@code char} value to be tested. </param>
		/// <returns> {@code true} if the {@code char} value is between
		///         <seealso cref="#MIN_LOW_SURROGATE"/> and
		///         <seealso cref="#MAX_LOW_SURROGATE"/> inclusive;
		///         {@code false} otherwise. </returns>
		/// <seealso cref=    Character#isHighSurrogate(char)
		/// @since  1.5 </seealso>
		public static bool IsLowSurrogate(char ch)
		{
			return ch >= MIN_LOW_SURROGATE && ch < (MAX_LOW_SURROGATE + 1);
		}

		/// <summary>
		/// Determines if the given {@code char} value is a Unicode
		/// <i>surrogate code unit</i>.
		/// 
		/// <para>Such values do not represent characters by themselves,
		/// but are used in the representation of
		/// <a href="#supplementary">supplementary characters</a>
		/// in the UTF-16 encoding.
		/// 
		/// </para>
		/// <para>A char value is a surrogate code unit if and only if it is either
		/// a <seealso cref="#isLowSurrogate(char) low-surrogate code unit"/> or
		/// a <seealso cref="#isHighSurrogate(char) high-surrogate code unit"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> the {@code char} value to be tested. </param>
		/// <returns> {@code true} if the {@code char} value is between
		///         <seealso cref="#MIN_SURROGATE"/> and
		///         <seealso cref="#MAX_SURROGATE"/> inclusive;
		///         {@code false} otherwise.
		/// @since  1.7 </returns>
		public static bool IsSurrogate(char ch)
		{
			return ch >= MIN_SURROGATE && ch < (MAX_SURROGATE + 1);
		}

		/// <summary>
		/// Determines whether the specified pair of {@code char}
		/// values is a valid
		/// <a href="http://www.unicode.org/glossary/#surrogate_pair">
		/// Unicode surrogate pair</a>.
		/// 
		/// <para>This method is equivalent to the expression:
		/// <blockquote><pre>{@code
		/// isHighSurrogate(high) && isLowSurrogate(low)
		/// }</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="high"> the high-surrogate code value to be tested </param>
		/// <param name="low"> the low-surrogate code value to be tested </param>
		/// <returns> {@code true} if the specified high and
		/// low-surrogate code values represent a valid surrogate pair;
		/// {@code false} otherwise.
		/// @since  1.5 </returns>
		public static bool IsSurrogatePair(char high, char low)
		{
			return IsHighSurrogate(high) && IsLowSurrogate(low);
		}

		/// <summary>
		/// Determines the number of {@code char} values needed to
		/// represent the specified character (Unicode code point). If the
		/// specified character is equal to or greater than 0x10000, then
		/// the method returns 2. Otherwise, the method returns 1.
		/// 
		/// <para>This method doesn't validate the specified character to be a
		/// valid Unicode code point. The caller must validate the
		/// character value using <seealso cref="#isValidCodePoint(int) isValidCodePoint"/>
		/// if necessary.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  2 if the character is a valid supplementary character; 1 otherwise. </returns>
		/// <seealso cref=     Character#isSupplementaryCodePoint(int)
		/// @since   1.5 </seealso>
		public static int CharCount(int codePoint)
		{
			return codePoint >= MIN_SUPPLEMENTARY_CODE_POINT ? 2 : 1;
		}

		/// <summary>
		/// Converts the specified surrogate pair to its supplementary code
		/// point value. This method does not validate the specified
		/// surrogate pair. The caller must validate it using {@link
		/// #isSurrogatePair(char, char) isSurrogatePair} if necessary.
		/// </summary>
		/// <param name="high"> the high-surrogate code unit </param>
		/// <param name="low"> the low-surrogate code unit </param>
		/// <returns> the supplementary code point composed from the
		///         specified surrogate pair.
		/// @since  1.5 </returns>
		public static int ToCodePoint(char high, char low)
		{
			// Optimized form of:
			// return ((high - MIN_HIGH_SURROGATE) << 10)
			//         + (low - MIN_LOW_SURROGATE)
			//         + MIN_SUPPLEMENTARY_CODE_POINT;
			return ((high << 10) + low) + (MIN_SUPPLEMENTARY_CODE_POINT - (MIN_HIGH_SURROGATE << 10) - MIN_LOW_SURROGATE);
		}

		/// <summary>
		/// Returns the code point at the given index of the
		/// {@code CharSequence}. If the {@code char} value at
		/// the given index in the {@code CharSequence} is in the
		/// high-surrogate range, the following index is less than the
		/// length of the {@code CharSequence}, and the
		/// {@code char} value at the following index is in the
		/// low-surrogate range, then the supplementary code point
		/// corresponding to this surrogate pair is returned. Otherwise,
		/// the {@code char} value at the given index is returned.
		/// </summary>
		/// <param name="seq"> a sequence of {@code char} values (Unicode code
		/// units) </param>
		/// <param name="index"> the index to the {@code char} values (Unicode
		/// code units) in {@code seq} to be converted </param>
		/// <returns> the Unicode code point at the given index </returns>
		/// <exception cref="NullPointerException"> if {@code seq} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the value
		/// {@code index} is negative or not less than
		/// <seealso cref="CharSequence#length() seq.length()"/>.
		/// @since  1.5 </exception>
		public static int CodePointAt(CharSequence seq, int index)
		{
			char c1 = seq.charAt(index);
			if (IsHighSurrogate(c1) && ++index < seq.length())
			{
				char c2 = seq.charAt(index);
				if (IsLowSurrogate(c2))
				{
					return ToCodePoint(c1, c2);
				}
			}
			return c1;
		}

		/// <summary>
		/// Returns the code point at the given index of the
		/// {@code char} array. If the {@code char} value at
		/// the given index in the {@code char} array is in the
		/// high-surrogate range, the following index is less than the
		/// length of the {@code char} array, and the
		/// {@code char} value at the following index is in the
		/// low-surrogate range, then the supplementary code point
		/// corresponding to this surrogate pair is returned. Otherwise,
		/// the {@code char} value at the given index is returned.
		/// </summary>
		/// <param name="a"> the {@code char} array </param>
		/// <param name="index"> the index to the {@code char} values (Unicode
		/// code units) in the {@code char} array to be converted </param>
		/// <returns> the Unicode code point at the given index </returns>
		/// <exception cref="NullPointerException"> if {@code a} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the value
		/// {@code index} is negative or not less than
		/// the length of the {@code char} array.
		/// @since  1.5 </exception>
		public static int CodePointAt(char[] a, int index)
		{
			return CodePointAtImpl(a, index, a.length);
		}

		/// <summary>
		/// Returns the code point at the given index of the
		/// {@code char} array, where only array elements with
		/// {@code index} less than {@code limit} can be used. If
		/// the {@code char} value at the given index in the
		/// {@code char} array is in the high-surrogate range, the
		/// following index is less than the {@code limit}, and the
		/// {@code char} value at the following index is in the
		/// low-surrogate range, then the supplementary code point
		/// corresponding to this surrogate pair is returned. Otherwise,
		/// the {@code char} value at the given index is returned.
		/// </summary>
		/// <param name="a"> the {@code char} array </param>
		/// <param name="index"> the index to the {@code char} values (Unicode
		/// code units) in the {@code char} array to be converted </param>
		/// <param name="limit"> the index after the last array element that
		/// can be used in the {@code char} array </param>
		/// <returns> the Unicode code point at the given index </returns>
		/// <exception cref="NullPointerException"> if {@code a} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the {@code index}
		/// argument is negative or not less than the {@code limit}
		/// argument, or if the {@code limit} argument is negative or
		/// greater than the length of the {@code char} array.
		/// @since  1.5 </exception>
		public static int CodePointAt(char[] a, int index, int limit)
		{
			if (index >= limit || limit < 0 || limit > a.length)
			{
				throw new IndexOutOfBoundsException();
			}
			return CodePointAtImpl(a, index, limit);
		}

		// throws ArrayIndexOutOfBoundsException if index out of bounds
		static int CodePointAtImpl(char[] a, int index, int limit)
		{
			char c1 = a[index];
			if (IsHighSurrogate(c1) && ++index < limit)
			{
				char c2 = a[index];
				if (IsLowSurrogate(c2))
				{
					return ToCodePoint(c1, c2);
				}
			}
			return c1;
		}

		/// <summary>
		/// Returns the code point preceding the given index of the
		/// {@code CharSequence}. If the {@code char} value at
		/// {@code (index - 1)} in the {@code CharSequence} is in
		/// the low-surrogate range, {@code (index - 2)} is not
		/// negative, and the {@code char} value at {@code (index - 2)}
		/// in the {@code CharSequence} is in the
		/// high-surrogate range, then the supplementary code point
		/// corresponding to this surrogate pair is returned. Otherwise,
		/// the {@code char} value at {@code (index - 1)} is
		/// returned.
		/// </summary>
		/// <param name="seq"> the {@code CharSequence} instance </param>
		/// <param name="index"> the index following the code point that should be returned </param>
		/// <returns> the Unicode code point value before the given index. </returns>
		/// <exception cref="NullPointerException"> if {@code seq} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the {@code index}
		/// argument is less than 1 or greater than {@link
		/// CharSequence#length() seq.length()}.
		/// @since  1.5 </exception>
		public static int CodePointBefore(CharSequence seq, int index)
		{
			char c2 = seq.charAt(--index);
			if (IsLowSurrogate(c2) && index > 0)
			{
				char c1 = seq.charAt(--index);
				if (IsHighSurrogate(c1))
				{
					return ToCodePoint(c1, c2);
				}
			}
			return c2;
		}

		/// <summary>
		/// Returns the code point preceding the given index of the
		/// {@code char} array. If the {@code char} value at
		/// {@code (index - 1)} in the {@code char} array is in
		/// the low-surrogate range, {@code (index - 2)} is not
		/// negative, and the {@code char} value at {@code (index - 2)}
		/// in the {@code char} array is in the
		/// high-surrogate range, then the supplementary code point
		/// corresponding to this surrogate pair is returned. Otherwise,
		/// the {@code char} value at {@code (index - 1)} is
		/// returned.
		/// </summary>
		/// <param name="a"> the {@code char} array </param>
		/// <param name="index"> the index following the code point that should be returned </param>
		/// <returns> the Unicode code point value before the given index. </returns>
		/// <exception cref="NullPointerException"> if {@code a} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the {@code index}
		/// argument is less than 1 or greater than the length of the
		/// {@code char} array
		/// @since  1.5 </exception>
		public static int CodePointBefore(char[] a, int index)
		{
			return CodePointBeforeImpl(a, index, 0);
		}

		/// <summary>
		/// Returns the code point preceding the given index of the
		/// {@code char} array, where only array elements with
		/// {@code index} greater than or equal to {@code start}
		/// can be used. If the {@code char} value at {@code (index - 1)}
		/// in the {@code char} array is in the
		/// low-surrogate range, {@code (index - 2)} is not less than
		/// {@code start}, and the {@code char} value at
		/// {@code (index - 2)} in the {@code char} array is in
		/// the high-surrogate range, then the supplementary code point
		/// corresponding to this surrogate pair is returned. Otherwise,
		/// the {@code char} value at {@code (index - 1)} is
		/// returned.
		/// </summary>
		/// <param name="a"> the {@code char} array </param>
		/// <param name="index"> the index following the code point that should be returned </param>
		/// <param name="start"> the index of the first array element in the
		/// {@code char} array </param>
		/// <returns> the Unicode code point value before the given index. </returns>
		/// <exception cref="NullPointerException"> if {@code a} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the {@code index}
		/// argument is not greater than the {@code start} argument or
		/// is greater than the length of the {@code char} array, or
		/// if the {@code start} argument is negative or not less than
		/// the length of the {@code char} array.
		/// @since  1.5 </exception>
		public static int CodePointBefore(char[] a, int index, int start)
		{
			if (index <= start || start < 0 || start >= a.length)
			{
				throw new IndexOutOfBoundsException();
			}
			return CodePointBeforeImpl(a, index, start);
		}

		// throws ArrayIndexOutOfBoundsException if index-1 out of bounds
		static int CodePointBeforeImpl(char[] a, int index, int start)
		{
			char c2 = a[--index];
			if (IsLowSurrogate(c2) && index > start)
			{
				char c1 = a[--index];
				if (IsHighSurrogate(c1))
				{
					return ToCodePoint(c1, c2);
				}
			}
			return c2;
		}

		/// <summary>
		/// Returns the leading surrogate (a
		/// <a href="http://www.unicode.org/glossary/#high_surrogate_code_unit">
		/// high surrogate code unit</a>) of the
		/// <a href="http://www.unicode.org/glossary/#surrogate_pair">
		/// surrogate pair</a>
		/// representing the specified supplementary character (Unicode
		/// code point) in the UTF-16 encoding.  If the specified character
		/// is not a
		/// <a href="Character.html#supplementary">supplementary character</a>,
		/// an unspecified {@code char} is returned.
		/// 
		/// <para>If
		/// <seealso cref="#isSupplementaryCodePoint isSupplementaryCodePoint(x)"/>
		/// is {@code true}, then
		/// <seealso cref="#isHighSurrogate isHighSurrogate"/>{@code (highSurrogate(x))} and
		/// <seealso cref="#toCodePoint toCodePoint"/>{@code (highSurrogate(x), }<seealso cref="#lowSurrogate lowSurrogate"/>{@code (x)) == x}
		/// are also always {@code true}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> a supplementary character (Unicode code point) </param>
		/// <returns>  the leading surrogate code unit used to represent the
		///          character in the UTF-16 encoding
		/// @since   1.7 </returns>
		public static char HighSurrogate(int codePoint)
		{
			return (char)(((int)((uint)codePoint >> 10)) + (MIN_HIGH_SURROGATE - ((int)((uint)MIN_SUPPLEMENTARY_CODE_POINT >> 10))));
		}

		/// <summary>
		/// Returns the trailing surrogate (a
		/// <a href="http://www.unicode.org/glossary/#low_surrogate_code_unit">
		/// low surrogate code unit</a>) of the
		/// <a href="http://www.unicode.org/glossary/#surrogate_pair">
		/// surrogate pair</a>
		/// representing the specified supplementary character (Unicode
		/// code point) in the UTF-16 encoding.  If the specified character
		/// is not a
		/// <a href="Character.html#supplementary">supplementary character</a>,
		/// an unspecified {@code char} is returned.
		/// 
		/// <para>If
		/// <seealso cref="#isSupplementaryCodePoint isSupplementaryCodePoint(x)"/>
		/// is {@code true}, then
		/// <seealso cref="#isLowSurrogate isLowSurrogate"/>{@code (lowSurrogate(x))} and
		/// <seealso cref="#toCodePoint toCodePoint"/>{@code (}<seealso cref="#highSurrogate highSurrogate"/>{@code (x), lowSurrogate(x)) == x}
		/// are also always {@code true}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> a supplementary character (Unicode code point) </param>
		/// <returns>  the trailing surrogate code unit used to represent the
		///          character in the UTF-16 encoding
		/// @since   1.7 </returns>
		public static char LowSurrogate(int codePoint)
		{
			return (char)((codePoint & 0x3ff) + MIN_LOW_SURROGATE);
		}

		/// <summary>
		/// Converts the specified character (Unicode code point) to its
		/// UTF-16 representation. If the specified code point is a BMP
		/// (Basic Multilingual Plane or Plane 0) value, the same value is
		/// stored in {@code dst[dstIndex]}, and 1 is returned. If the
		/// specified code point is a supplementary character, its
		/// surrogate values are stored in {@code dst[dstIndex]}
		/// (high-surrogate) and {@code dst[dstIndex+1]}
		/// (low-surrogate), and 2 is returned.
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be converted. </param>
		/// <param name="dst"> an array of {@code char} in which the
		/// {@code codePoint}'s UTF-16 value is stored. </param>
		/// <param name="dstIndex"> the start index into the {@code dst}
		/// array where the converted value is stored. </param>
		/// <returns> 1 if the code point is a BMP code point, 2 if the
		/// code point is a supplementary code point. </returns>
		/// <exception cref="IllegalArgumentException"> if the specified
		/// {@code codePoint} is not a valid Unicode code point. </exception>
		/// <exception cref="NullPointerException"> if the specified {@code dst} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if {@code dstIndex}
		/// is negative or not less than {@code dst.length}, or if
		/// {@code dst} at {@code dstIndex} doesn't have enough
		/// array element(s) to store the resulting {@code char}
		/// value(s). (If {@code dstIndex} is equal to
		/// {@code dst.length-1} and the specified
		/// {@code codePoint} is a supplementary character, the
		/// high-surrogate value is not stored in
		/// {@code dst[dstIndex]}.)
		/// @since  1.5 </exception>
		public static int ToChars(int codePoint, char[] dst, int dstIndex)
		{
			if (IsBmpCodePoint(codePoint))
			{
				dst[dstIndex] = (char) codePoint;
				return 1;
			}
			else if (IsValidCodePoint(codePoint))
			{
				ToSurrogates(codePoint, dst, dstIndex);
				return 2;
			}
			else
			{
				throw new IllegalArgumentException();
			}
		}

		/// <summary>
		/// Converts the specified character (Unicode code point) to its
		/// UTF-16 representation stored in a {@code char} array. If
		/// the specified code point is a BMP (Basic Multilingual Plane or
		/// Plane 0) value, the resulting {@code char} array has
		/// the same value as {@code codePoint}. If the specified code
		/// point is a supplementary code point, the resulting
		/// {@code char} array has the corresponding surrogate pair.
		/// </summary>
		/// <param name="codePoint"> a Unicode code point </param>
		/// <returns> a {@code char} array having
		///         {@code codePoint}'s UTF-16 representation. </returns>
		/// <exception cref="IllegalArgumentException"> if the specified
		/// {@code codePoint} is not a valid Unicode code point.
		/// @since  1.5 </exception>
		public static char[] ToChars(int codePoint)
		{
			if (IsBmpCodePoint(codePoint))
			{
				return new char[] {(char) codePoint};
			}
			else if (IsValidCodePoint(codePoint))
			{
				char[] result = new char[2];
				ToSurrogates(codePoint, result, 0);
				return result;
			}
			else
			{
				throw new IllegalArgumentException();
			}
		}

		static void toSurrogates(int codePoint, char[] dst, int index)
		{
			// We write elements "backwards" to guarantee all-or-nothing
			dst[index + 1] = LowSurrogate(codePoint);
			dst[index] = HighSurrogate(codePoint);
		}

		/// <summary>
		/// Returns the number of Unicode code points in the text range of
		/// the specified char sequence. The text range begins at the
		/// specified {@code beginIndex} and extends to the
		/// {@code char} at index {@code endIndex - 1}. Thus the
		/// length (in {@code char}s) of the text range is
		/// {@code endIndex-beginIndex}. Unpaired surrogates within
		/// the text range count as one code point each.
		/// </summary>
		/// <param name="seq"> the char sequence </param>
		/// <param name="beginIndex"> the index to the first {@code char} of
		/// the text range. </param>
		/// <param name="endIndex"> the index after the last {@code char} of
		/// the text range. </param>
		/// <returns> the number of Unicode code points in the specified text
		/// range </returns>
		/// <exception cref="NullPointerException"> if {@code seq} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if the
		/// {@code beginIndex} is negative, or {@code endIndex}
		/// is larger than the length of the given sequence, or
		/// {@code beginIndex} is larger than {@code endIndex}.
		/// @since  1.5 </exception>
		public static int CodePointCount(CharSequence seq, int beginIndex, int endIndex)
		{
			int length = seq.length();
			if (beginIndex < 0 || endIndex > length || beginIndex > endIndex)
			{
				throw new IndexOutOfBoundsException();
			}
			int n = endIndex - beginIndex;
			for (int i = beginIndex; i < endIndex;)
			{
				if (IsHighSurrogate(seq.charAt(i++)) && i < endIndex && IsLowSurrogate(seq.charAt(i)))
				{
					n--;
					i++;
				}
			}
			return n;
		}

		/// <summary>
		/// Returns the number of Unicode code points in a subarray of the
		/// {@code char} array argument. The {@code offset}
		/// argument is the index of the first {@code char} of the
		/// subarray and the {@code count} argument specifies the
		/// length of the subarray in {@code char}s. Unpaired
		/// surrogates within the subarray count as one code point each.
		/// </summary>
		/// <param name="a"> the {@code char} array </param>
		/// <param name="offset"> the index of the first {@code char} in the
		/// given {@code char} array </param>
		/// <param name="count"> the length of the subarray in {@code char}s </param>
		/// <returns> the number of Unicode code points in the specified subarray </returns>
		/// <exception cref="NullPointerException"> if {@code a} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if {@code offset} or
		/// {@code count} is negative, or if {@code offset +
		/// count} is larger than the length of the given array.
		/// @since  1.5 </exception>
		public static int CodePointCount(char[] a, int offset, int count)
		{
			if (count > a.length - offset || offset < 0 || count < 0)
			{
				throw new IndexOutOfBoundsException();
			}
			return CodePointCountImpl(a, offset, count);
		}

		static int CodePointCountImpl(char[] a, int offset, int count)
		{
			int endIndex = offset + count;
			int n = count;
			for (int i = offset; i < endIndex;)
			{
				if (IsHighSurrogate(a[i++]) && i < endIndex && IsLowSurrogate(a[i]))
				{
					n--;
					i++;
				}
			}
			return n;
		}

		/// <summary>
		/// Returns the index within the given char sequence that is offset
		/// from the given {@code index} by {@code codePointOffset}
		/// code points. Unpaired surrogates within the text range given by
		/// {@code index} and {@code codePointOffset} count as
		/// one code point each.
		/// </summary>
		/// <param name="seq"> the char sequence </param>
		/// <param name="index"> the index to be offset </param>
		/// <param name="codePointOffset"> the offset in code points </param>
		/// <returns> the index within the char sequence </returns>
		/// <exception cref="NullPointerException"> if {@code seq} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if {@code index}
		///   is negative or larger then the length of the char sequence,
		///   or if {@code codePointOffset} is positive and the
		///   subsequence starting with {@code index} has fewer than
		///   {@code codePointOffset} code points, or if
		///   {@code codePointOffset} is negative and the subsequence
		///   before {@code index} has fewer than the absolute value
		///   of {@code codePointOffset} code points.
		/// @since 1.5 </exception>
		public static int OffsetByCodePoints(CharSequence seq, int index, int codePointOffset)
		{
			int length = seq.length();
			if (index < 0 || index > length)
			{
				throw new IndexOutOfBoundsException();
			}

			int x = index;
			if (codePointOffset >= 0)
			{
				int i;
				for (i = 0; x < length && i < codePointOffset; i++)
				{
					if (IsHighSurrogate(seq.charAt(x++)) && x < length && IsLowSurrogate(seq.charAt(x)))
					{
						x++;
					}
				}
				if (i < codePointOffset)
				{
					throw new IndexOutOfBoundsException();
				}
			}
			else
			{
				int i;
				for (i = codePointOffset; x > 0 && i < 0; i++)
				{
					if (IsLowSurrogate(seq.charAt(--x)) && x > 0 && IsHighSurrogate(seq.charAt(x - 1)))
					{
						x--;
					}
				}
				if (i < 0)
				{
					throw new IndexOutOfBoundsException();
				}
			}
			return x;
		}

		/// <summary>
		/// Returns the index within the given {@code char} subarray
		/// that is offset from the given {@code index} by
		/// {@code codePointOffset} code points. The
		/// {@code start} and {@code count} arguments specify a
		/// subarray of the {@code char} array. Unpaired surrogates
		/// within the text range given by {@code index} and
		/// {@code codePointOffset} count as one code point each.
		/// </summary>
		/// <param name="a"> the {@code char} array </param>
		/// <param name="start"> the index of the first {@code char} of the
		/// subarray </param>
		/// <param name="count"> the length of the subarray in {@code char}s </param>
		/// <param name="index"> the index to be offset </param>
		/// <param name="codePointOffset"> the offset in code points </param>
		/// <returns> the index within the subarray </returns>
		/// <exception cref="NullPointerException"> if {@code a} is null. </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///   if {@code start} or {@code count} is negative,
		///   or if {@code start + count} is larger than the length of
		///   the given array,
		///   or if {@code index} is less than {@code start} or
		///   larger then {@code start + count},
		///   or if {@code codePointOffset} is positive and the text range
		///   starting with {@code index} and ending with {@code start + count - 1}
		///   has fewer than {@code codePointOffset} code
		///   points,
		///   or if {@code codePointOffset} is negative and the text range
		///   starting with {@code start} and ending with {@code index - 1}
		///   has fewer than the absolute value of
		///   {@code codePointOffset} code points.
		/// @since 1.5 </exception>
		public static int OffsetByCodePoints(char[] a, int start, int count, int index, int codePointOffset)
		{
			if (count > a.length - start || start < 0 || count < 0 || index < start || index > start + count)
			{
				throw new IndexOutOfBoundsException();
			}
			return OffsetByCodePointsImpl(a, start, count, index, codePointOffset);
		}

		static int OffsetByCodePointsImpl(char[] a, int start, int count, int index, int codePointOffset)
		{
			int x = index;
			if (codePointOffset >= 0)
			{
				int limit = start + count;
				int i;
				for (i = 0; x < limit && i < codePointOffset; i++)
				{
					if (IsHighSurrogate(a[x++]) && x < limit && IsLowSurrogate(a[x]))
					{
						x++;
					}
				}
				if (i < codePointOffset)
				{
					throw new IndexOutOfBoundsException();
				}
			}
			else
			{
				int i;
				for (i = codePointOffset; x > start && i < 0; i++)
				{
					if (IsLowSurrogate(a[--x]) && x > start && IsHighSurrogate(a[x - 1]))
					{
						x--;
					}
				}
				if (i < 0)
				{
					throw new IndexOutOfBoundsException();
				}
			}
			return x;
		}

		/// <summary>
		/// Determines if the specified character is a lowercase character.
		/// <para>
		/// A character is lowercase if its general category type, provided
		/// by {@code Character.getType(ch)}, is
		/// {@code LOWERCASE_LETTER}, or it has contributory property
		/// Other_Lowercase as defined by the Unicode Standard.
		/// </para>
		/// <para>
		/// The following are examples of lowercase characters:
		/// <blockquote><pre>
		/// a b c d e f g h i j k l m n o p q r s t u v w x y z
		/// '&#92;u00DF' '&#92;u00E0' '&#92;u00E1' '&#92;u00E2' '&#92;u00E3' '&#92;u00E4' '&#92;u00E5' '&#92;u00E6'
		/// '&#92;u00E7' '&#92;u00E8' '&#92;u00E9' '&#92;u00EA' '&#92;u00EB' '&#92;u00EC' '&#92;u00ED' '&#92;u00EE'
		/// '&#92;u00EF' '&#92;u00F0' '&#92;u00F1' '&#92;u00F2' '&#92;u00F3' '&#92;u00F4' '&#92;u00F5' '&#92;u00F6'
		/// '&#92;u00F8' '&#92;u00F9' '&#92;u00FA' '&#92;u00FB' '&#92;u00FC' '&#92;u00FD' '&#92;u00FE' '&#92;u00FF'
		/// </pre></blockquote>
		/// </para>
		/// <para> Many other Unicode characters are lowercase too.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isLowerCase(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be tested. </param>
		/// <returns>  {@code true} if the character is lowercase;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isLowerCase(char) </seealso>
		/// <seealso cref=     Character#isTitleCase(char) </seealso>
		/// <seealso cref=     Character#toLowerCase(char) </seealso>
		/// <seealso cref=     Character#getType(char) </seealso>
		public static bool IsLowerCase(char ch)
		{
			return IsLowerCase((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is a
		/// lowercase character.
		/// <para>
		/// A character is lowercase if its general category type, provided
		/// by <seealso cref="Character#getType getType(codePoint)"/>, is
		/// {@code LOWERCASE_LETTER}, or it has contributory property
		/// Other_Lowercase as defined by the Unicode Standard.
		/// </para>
		/// <para>
		/// The following are examples of lowercase characters:
		/// <blockquote><pre>
		/// a b c d e f g h i j k l m n o p q r s t u v w x y z
		/// '&#92;u00DF' '&#92;u00E0' '&#92;u00E1' '&#92;u00E2' '&#92;u00E3' '&#92;u00E4' '&#92;u00E5' '&#92;u00E6'
		/// '&#92;u00E7' '&#92;u00E8' '&#92;u00E9' '&#92;u00EA' '&#92;u00EB' '&#92;u00EC' '&#92;u00ED' '&#92;u00EE'
		/// '&#92;u00EF' '&#92;u00F0' '&#92;u00F1' '&#92;u00F2' '&#92;u00F3' '&#92;u00F4' '&#92;u00F5' '&#92;u00F6'
		/// '&#92;u00F8' '&#92;u00F9' '&#92;u00FA' '&#92;u00FB' '&#92;u00FC' '&#92;u00FD' '&#92;u00FE' '&#92;u00FF'
		/// </pre></blockquote>
		/// </para>
		/// <para> Many other Unicode characters are lowercase too.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is lowercase;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isLowerCase(int) </seealso>
		/// <seealso cref=     Character#isTitleCase(int) </seealso>
		/// <seealso cref=     Character#toLowerCase(int) </seealso>
		/// <seealso cref=     Character#getType(int)
		/// @since   1.5 </seealso>
		public static bool IsLowerCase(int codePoint)
		{
			return GetType(codePoint) == Character.LOWERCASE_LETTER || CharacterData.Of(codePoint).IsOtherLowercase(codePoint);
		}

		/// <summary>
		/// Determines if the specified character is an uppercase character.
		/// <para>
		/// A character is uppercase if its general category type, provided by
		/// {@code Character.getType(ch)}, is {@code UPPERCASE_LETTER}.
		/// or it has contributory property Other_Uppercase as defined by the Unicode Standard.
		/// </para>
		/// <para>
		/// The following are examples of uppercase characters:
		/// <blockquote><pre>
		/// A B C D E F G H I J K L M N O P Q R S T U V W X Y Z
		/// '&#92;u00C0' '&#92;u00C1' '&#92;u00C2' '&#92;u00C3' '&#92;u00C4' '&#92;u00C5' '&#92;u00C6' '&#92;u00C7'
		/// '&#92;u00C8' '&#92;u00C9' '&#92;u00CA' '&#92;u00CB' '&#92;u00CC' '&#92;u00CD' '&#92;u00CE' '&#92;u00CF'
		/// '&#92;u00D0' '&#92;u00D1' '&#92;u00D2' '&#92;u00D3' '&#92;u00D4' '&#92;u00D5' '&#92;u00D6' '&#92;u00D8'
		/// '&#92;u00D9' '&#92;u00DA' '&#92;u00DB' '&#92;u00DC' '&#92;u00DD' '&#92;u00DE'
		/// </pre></blockquote>
		/// </para>
		/// <para> Many other Unicode characters are uppercase too.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isUpperCase(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be tested. </param>
		/// <returns>  {@code true} if the character is uppercase;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isLowerCase(char) </seealso>
		/// <seealso cref=     Character#isTitleCase(char) </seealso>
		/// <seealso cref=     Character#toUpperCase(char) </seealso>
		/// <seealso cref=     Character#getType(char)
		/// @since   1.0 </seealso>
		public static bool IsUpperCase(char ch)
		{
			return IsUpperCase((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is an uppercase character.
		/// <para>
		/// A character is uppercase if its general category type, provided by
		/// <seealso cref="Character#getType(int) getType(codePoint)"/>, is {@code UPPERCASE_LETTER},
		/// or it has contributory property Other_Uppercase as defined by the Unicode Standard.
		/// </para>
		/// <para>
		/// The following are examples of uppercase characters:
		/// <blockquote><pre>
		/// A B C D E F G H I J K L M N O P Q R S T U V W X Y Z
		/// '&#92;u00C0' '&#92;u00C1' '&#92;u00C2' '&#92;u00C3' '&#92;u00C4' '&#92;u00C5' '&#92;u00C6' '&#92;u00C7'
		/// '&#92;u00C8' '&#92;u00C9' '&#92;u00CA' '&#92;u00CB' '&#92;u00CC' '&#92;u00CD' '&#92;u00CE' '&#92;u00CF'
		/// '&#92;u00D0' '&#92;u00D1' '&#92;u00D2' '&#92;u00D3' '&#92;u00D4' '&#92;u00D5' '&#92;u00D6' '&#92;u00D8'
		/// '&#92;u00D9' '&#92;u00DA' '&#92;u00DB' '&#92;u00DC' '&#92;u00DD' '&#92;u00DE'
		/// </pre></blockquote>
		/// </para>
		/// <para> Many other Unicode characters are uppercase too.<p>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is uppercase;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isLowerCase(int) </seealso>
		/// <seealso cref=     Character#isTitleCase(int) </seealso>
		/// <seealso cref=     Character#toUpperCase(int) </seealso>
		/// <seealso cref=     Character#getType(int)
		/// @since   1.5 </seealso>
		public static bool IsUpperCase(int codePoint)
		{
			return GetType(codePoint) == Character.UPPERCASE_LETTER || CharacterData.Of(codePoint).IsOtherUppercase(codePoint);
		}

		/// <summary>
		/// Determines if the specified character is a titlecase character.
		/// <para>
		/// A character is a titlecase character if its general
		/// category type, provided by {@code Character.getType(ch)},
		/// is {@code TITLECASE_LETTER}.
		/// </para>
		/// <para>
		/// Some characters look like pairs of Latin letters. For example, there
		/// is an uppercase letter that looks like "LJ" and has a corresponding
		/// lowercase letter that looks like "lj". A third form, which looks like "Lj",
		/// is the appropriate form to use when rendering a word in lowercase
		/// with initial capitals, as for a book title.
		/// </para>
		/// <para>
		/// These are some of the Unicode characters for which this method returns
		/// {@code true}:
		/// <ul>
		/// <li>{@code LATIN CAPITAL LETTER D WITH SMALL LETTER Z WITH CARON}
		/// <li>{@code LATIN CAPITAL LETTER L WITH SMALL LETTER J}
		/// <li>{@code LATIN CAPITAL LETTER N WITH SMALL LETTER J}
		/// <li>{@code LATIN CAPITAL LETTER D WITH SMALL LETTER Z}
		/// </ul>
		/// </para>
		/// <para> Many other Unicode characters are titlecase too.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isTitleCase(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be tested. </param>
		/// <returns>  {@code true} if the character is titlecase;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isLowerCase(char) </seealso>
		/// <seealso cref=     Character#isUpperCase(char) </seealso>
		/// <seealso cref=     Character#toTitleCase(char) </seealso>
		/// <seealso cref=     Character#getType(char)
		/// @since   1.0.2 </seealso>
		public static bool IsTitleCase(char ch)
		{
			return IsTitleCase((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is a titlecase character.
		/// <para>
		/// A character is a titlecase character if its general
		/// category type, provided by <seealso cref="Character#getType(int) getType(codePoint)"/>,
		/// is {@code TITLECASE_LETTER}.
		/// </para>
		/// <para>
		/// Some characters look like pairs of Latin letters. For example, there
		/// is an uppercase letter that looks like "LJ" and has a corresponding
		/// lowercase letter that looks like "lj". A third form, which looks like "Lj",
		/// is the appropriate form to use when rendering a word in lowercase
		/// with initial capitals, as for a book title.
		/// </para>
		/// <para>
		/// These are some of the Unicode characters for which this method returns
		/// {@code true}:
		/// <ul>
		/// <li>{@code LATIN CAPITAL LETTER D WITH SMALL LETTER Z WITH CARON}
		/// <li>{@code LATIN CAPITAL LETTER L WITH SMALL LETTER J}
		/// <li>{@code LATIN CAPITAL LETTER N WITH SMALL LETTER J}
		/// <li>{@code LATIN CAPITAL LETTER D WITH SMALL LETTER Z}
		/// </ul>
		/// </para>
		/// <para> Many other Unicode characters are titlecase too.<p>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is titlecase;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isLowerCase(int) </seealso>
		/// <seealso cref=     Character#isUpperCase(int) </seealso>
		/// <seealso cref=     Character#toTitleCase(int) </seealso>
		/// <seealso cref=     Character#getType(int)
		/// @since   1.5 </seealso>
		public static bool IsTitleCase(int codePoint)
		{
			return GetType(codePoint) == Character.TITLECASE_LETTER;
		}

		/// <summary>
		/// Determines if the specified character is a digit.
		/// <para>
		/// A character is a digit if its general category type, provided
		/// by {@code Character.getType(ch)}, is
		/// {@code DECIMAL_DIGIT_NUMBER}.
		/// </para>
		/// <para>
		/// Some Unicode character ranges that contain digits:
		/// <ul>
		/// <li>{@code '\u005Cu0030'} through {@code '\u005Cu0039'},
		///     ISO-LATIN-1 digits ({@code '0'} through {@code '9'})
		/// <li>{@code '\u005Cu0660'} through {@code '\u005Cu0669'},
		///     Arabic-Indic digits
		/// <li>{@code '\u005Cu06F0'} through {@code '\u005Cu06F9'},
		///     Extended Arabic-Indic digits
		/// <li>{@code '\u005Cu0966'} through {@code '\u005Cu096F'},
		///     Devanagari digits
		/// <li>{@code '\u005CuFF10'} through {@code '\u005CuFF19'},
		///     Fullwidth digits
		/// </ul>
		/// 
		/// Many other character ranges contain digits as well.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isDigit(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be tested. </param>
		/// <returns>  {@code true} if the character is a digit;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#digit(char, int) </seealso>
		/// <seealso cref=     Character#forDigit(int, int) </seealso>
		/// <seealso cref=     Character#getType(char) </seealso>
		public static bool IsDigit(char ch)
		{
			return IsDigit((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is a digit.
		/// <para>
		/// A character is a digit if its general category type, provided
		/// by <seealso cref="Character#getType(int) getType(codePoint)"/>, is
		/// {@code DECIMAL_DIGIT_NUMBER}.
		/// </para>
		/// <para>
		/// Some Unicode character ranges that contain digits:
		/// <ul>
		/// <li>{@code '\u005Cu0030'} through {@code '\u005Cu0039'},
		///     ISO-LATIN-1 digits ({@code '0'} through {@code '9'})
		/// <li>{@code '\u005Cu0660'} through {@code '\u005Cu0669'},
		///     Arabic-Indic digits
		/// <li>{@code '\u005Cu06F0'} through {@code '\u005Cu06F9'},
		///     Extended Arabic-Indic digits
		/// <li>{@code '\u005Cu0966'} through {@code '\u005Cu096F'},
		///     Devanagari digits
		/// <li>{@code '\u005CuFF10'} through {@code '\u005CuFF19'},
		///     Fullwidth digits
		/// </ul>
		/// 
		/// Many other character ranges contain digits as well.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is a digit;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#forDigit(int, int) </seealso>
		/// <seealso cref=     Character#getType(int)
		/// @since   1.5 </seealso>
		public static bool IsDigit(int codePoint)
		{
			return GetType(codePoint) == Character.DECIMAL_DIGIT_NUMBER;
		}

		/// <summary>
		/// Determines if a character is defined in Unicode.
		/// <para>
		/// A character is defined if at least one of the following is true:
		/// <ul>
		/// <li>It has an entry in the UnicodeData file.
		/// <li>It has a value in a range defined by the UnicodeData file.
		/// </ul>
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isDefined(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be tested </param>
		/// <returns>  {@code true} if the character has a defined meaning
		///          in Unicode; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isDigit(char) </seealso>
		/// <seealso cref=     Character#isLetter(char) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(char) </seealso>
		/// <seealso cref=     Character#isLowerCase(char) </seealso>
		/// <seealso cref=     Character#isTitleCase(char) </seealso>
		/// <seealso cref=     Character#isUpperCase(char)
		/// @since   1.0.2 </seealso>
		public static bool IsDefined(char ch)
		{
			return IsDefined((int)ch);
		}

		/// <summary>
		/// Determines if a character (Unicode code point) is defined in Unicode.
		/// <para>
		/// A character is defined if at least one of the following is true:
		/// <ul>
		/// <li>It has an entry in the UnicodeData file.
		/// <li>It has a value in a range defined by the UnicodeData file.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character has a defined meaning
		///          in Unicode; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isDigit(int) </seealso>
		/// <seealso cref=     Character#isLetter(int) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(int) </seealso>
		/// <seealso cref=     Character#isLowerCase(int) </seealso>
		/// <seealso cref=     Character#isTitleCase(int) </seealso>
		/// <seealso cref=     Character#isUpperCase(int)
		/// @since   1.5 </seealso>
		public static bool IsDefined(int codePoint)
		{
			return GetType(codePoint) != Character.UNASSIGNED;
		}

		/// <summary>
		/// Determines if the specified character is a letter.
		/// <para>
		/// A character is considered to be a letter if its general
		/// category type, provided by {@code Character.getType(ch)},
		/// is any of the following:
		/// <ul>
		/// <li> {@code UPPERCASE_LETTER}
		/// <li> {@code LOWERCASE_LETTER}
		/// <li> {@code TITLECASE_LETTER}
		/// <li> {@code MODIFIER_LETTER}
		/// <li> {@code OTHER_LETTER}
		/// </ul>
		/// 
		/// Not all letters have case. Many characters are
		/// letters but are neither uppercase nor lowercase nor titlecase.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isLetter(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be tested. </param>
		/// <returns>  {@code true} if the character is a letter;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isDigit(char) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
		/// <seealso cref=     Character#isJavaLetter(char) </seealso>
		/// <seealso cref=     Character#isJavaLetterOrDigit(char) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(char) </seealso>
		/// <seealso cref=     Character#isLowerCase(char) </seealso>
		/// <seealso cref=     Character#isTitleCase(char) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierStart(char) </seealso>
		/// <seealso cref=     Character#isUpperCase(char) </seealso>
		public static bool IsLetter(char ch)
		{
			return IsLetter((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is a letter.
		/// <para>
		/// A character is considered to be a letter if its general
		/// category type, provided by <seealso cref="Character#getType(int) getType(codePoint)"/>,
		/// is any of the following:
		/// <ul>
		/// <li> {@code UPPERCASE_LETTER}
		/// <li> {@code LOWERCASE_LETTER}
		/// <li> {@code TITLECASE_LETTER}
		/// <li> {@code MODIFIER_LETTER}
		/// <li> {@code OTHER_LETTER}
		/// </ul>
		/// 
		/// Not all letters have case. Many characters are
		/// letters but are neither uppercase nor lowercase nor titlecase.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is a letter;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isDigit(int) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierStart(int) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(int) </seealso>
		/// <seealso cref=     Character#isLowerCase(int) </seealso>
		/// <seealso cref=     Character#isTitleCase(int) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierStart(int) </seealso>
		/// <seealso cref=     Character#isUpperCase(int)
		/// @since   1.5 </seealso>
		public static bool IsLetter(int codePoint)
		{
			return ((((1 << Character.UPPERCASE_LETTER) | (1 << Character.LOWERCASE_LETTER) | (1 << Character.TITLECASE_LETTER) | (1 << Character.MODIFIER_LETTER) | (1 << Character.OTHER_LETTER)) >> GetType(codePoint)) & 1) != 0;
		}

		/// <summary>
		/// Determines if the specified character is a letter or digit.
		/// <para>
		/// A character is considered to be a letter or digit if either
		/// {@code Character.isLetter(char ch)} or
		/// {@code Character.isDigit(char ch)} returns
		/// {@code true} for the character.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isLetterOrDigit(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be tested. </param>
		/// <returns>  {@code true} if the character is a letter or digit;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isDigit(char) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
		/// <seealso cref=     Character#isJavaLetter(char) </seealso>
		/// <seealso cref=     Character#isJavaLetterOrDigit(char) </seealso>
		/// <seealso cref=     Character#isLetter(char) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierPart(char)
		/// @since   1.0.2 </seealso>
		public static bool IsLetterOrDigit(char ch)
		{
			return IsLetterOrDigit((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is a letter or digit.
		/// <para>
		/// A character is considered to be a letter or digit if either
		/// <seealso cref="#isLetter(int) isLetter(codePoint)"/> or
		/// <seealso cref="#isDigit(int) isDigit(codePoint)"/> returns
		/// {@code true} for the character.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is a letter or digit;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isDigit(int) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierPart(int) </seealso>
		/// <seealso cref=     Character#isLetter(int) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierPart(int)
		/// @since   1.5 </seealso>
		public static bool IsLetterOrDigit(int codePoint)
		{
			return ((((1 << Character.UPPERCASE_LETTER) | (1 << Character.LOWERCASE_LETTER) | (1 << Character.TITLECASE_LETTER) | (1 << Character.MODIFIER_LETTER) | (1 << Character.OTHER_LETTER) | (1 << Character.DECIMAL_DIGIT_NUMBER)) >> GetType(codePoint)) & 1) != 0;
		}

		/// <summary>
		/// Determines if the specified character is permissible as the first
		/// character in a Java identifier.
		/// <para>
		/// A character may start a Java identifier if and only if
		/// one of the following is true:
		/// <ul>
		/// <li> <seealso cref="#isLetter(char) isLetter(ch)"/> returns {@code true}
		/// <li> <seealso cref="#getType(char) getType(ch)"/> returns {@code LETTER_NUMBER}
		/// <li> {@code ch} is a currency symbol (such as {@code '$'})
		/// <li> {@code ch} is a connecting punctuation character (such as {@code '_'}).
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> the character to be tested. </param>
		/// <returns>  {@code true} if the character may start a Java
		///          identifier; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isJavaLetterOrDigit(char) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
		/// <seealso cref=     Character#isLetter(char) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(char) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierStart(char)
		/// @since   1.02 </seealso>
		/// @deprecated Replaced by isJavaIdentifierStart(char). 
		[Obsolete("Replaced by isJavaIdentifierStart(char).")]
		public static bool IsJavaLetter(char ch)
		{
			return IsJavaIdentifierStart(ch);
		}

		/// <summary>
		/// Determines if the specified character may be part of a Java
		/// identifier as other than the first character.
		/// <para>
		/// A character may be part of a Java identifier if and only if any
		/// of the following are true:
		/// <ul>
		/// <li>  it is a letter
		/// <li>  it is a currency symbol (such as {@code '$'})
		/// <li>  it is a connecting punctuation character (such as {@code '_'})
		/// <li>  it is a digit
		/// <li>  it is a numeric letter (such as a Roman numeral character)
		/// <li>  it is a combining mark
		/// <li>  it is a non-spacing mark
		/// <li> {@code isIdentifierIgnorable} returns
		/// {@code true} for the character.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> the character to be tested. </param>
		/// <returns>  {@code true} if the character may be part of a
		///          Java identifier; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isJavaLetter(char) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
		/// <seealso cref=     Character#isLetter(char) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(char) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierPart(char) </seealso>
		/// <seealso cref=     Character#isIdentifierIgnorable(char)
		/// @since   1.02 </seealso>
		/// @deprecated Replaced by isJavaIdentifierPart(char). 
		[Obsolete("Replaced by isJavaIdentifierPart(char).")]
		public static bool IsJavaLetterOrDigit(char ch)
		{
			return IsJavaIdentifierPart(ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is an alphabet.
		/// <para>
		/// A character is considered to be alphabetic if its general category type,
		/// provided by <seealso cref="Character#getType(int) getType(codePoint)"/>, is any of
		/// the following:
		/// <ul>
		/// <li> <code>UPPERCASE_LETTER</code>
		/// <li> <code>LOWERCASE_LETTER</code>
		/// <li> <code>TITLECASE_LETTER</code>
		/// <li> <code>MODIFIER_LETTER</code>
		/// <li> <code>OTHER_LETTER</code>
		/// <li> <code>LETTER_NUMBER</code>
		/// </ul>
		/// or it has contributory property Other_Alphabetic as defined by the
		/// Unicode Standard.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  <code>true</code> if the character is a Unicode alphabet
		///          character, <code>false</code> otherwise.
		/// @since   1.7 </returns>
		public static bool IsAlphabetic(int codePoint)
		{
			return (((((1 << Character.UPPERCASE_LETTER) | (1 << Character.LOWERCASE_LETTER) | (1 << Character.TITLECASE_LETTER) | (1 << Character.MODIFIER_LETTER) | (1 << Character.OTHER_LETTER) | (1 << Character.LETTER_NUMBER)) >> GetType(codePoint)) & 1) != 0) || CharacterData.Of(codePoint).IsOtherAlphabetic(codePoint);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is a CJKV
		/// (Chinese, Japanese, Korean and Vietnamese) ideograph, as defined by
		/// the Unicode Standard.
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  <code>true</code> if the character is a Unicode ideograph
		///          character, <code>false</code> otherwise.
		/// @since   1.7 </returns>
		public static bool IsIdeographic(int codePoint)
		{
			return CharacterData.Of(codePoint).IsIdeographic(codePoint);
		}

		/// <summary>
		/// Determines if the specified character is
		/// permissible as the first character in a Java identifier.
		/// <para>
		/// A character may start a Java identifier if and only if
		/// one of the following conditions is true:
		/// <ul>
		/// <li> <seealso cref="#isLetter(char) isLetter(ch)"/> returns {@code true}
		/// <li> <seealso cref="#getType(char) getType(ch)"/> returns {@code LETTER_NUMBER}
		/// <li> {@code ch} is a currency symbol (such as {@code '$'})
		/// <li> {@code ch} is a connecting punctuation character (such as {@code '_'}).
		/// </ul>
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isJavaIdentifierStart(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> the character to be tested. </param>
		/// <returns>  {@code true} if the character may start a Java identifier;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
		/// <seealso cref=     Character#isLetter(char) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierStart(char) </seealso>
		/// <seealso cref=     javax.lang.model.SourceVersion#isIdentifier(CharSequence)
		/// @since   1.1 </seealso>
		public static bool IsJavaIdentifierStart(char ch)
		{
			return IsJavaIdentifierStart((int)ch);
		}

		/// <summary>
		/// Determines if the character (Unicode code point) is
		/// permissible as the first character in a Java identifier.
		/// <para>
		/// A character may start a Java identifier if and only if
		/// one of the following conditions is true:
		/// <ul>
		/// <li> <seealso cref="#isLetter(int) isLetter(codePoint)"/>
		///      returns {@code true}
		/// <li> <seealso cref="#getType(int) getType(codePoint)"/>
		///      returns {@code LETTER_NUMBER}
		/// <li> the referenced character is a currency symbol (such as {@code '$'})
		/// <li> the referenced character is a connecting punctuation character
		///      (such as {@code '_'}).
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character may start a Java identifier;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isJavaIdentifierPart(int) </seealso>
		/// <seealso cref=     Character#isLetter(int) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierStart(int) </seealso>
		/// <seealso cref=     javax.lang.model.SourceVersion#isIdentifier(CharSequence)
		/// @since   1.5 </seealso>
		public static bool IsJavaIdentifierStart(int codePoint)
		{
			return CharacterData.Of(codePoint).IsJavaIdentifierStart(codePoint);
		}

		/// <summary>
		/// Determines if the specified character may be part of a Java
		/// identifier as other than the first character.
		/// <para>
		/// A character may be part of a Java identifier if any of the following
		/// are true:
		/// <ul>
		/// <li>  it is a letter
		/// <li>  it is a currency symbol (such as {@code '$'})
		/// <li>  it is a connecting punctuation character (such as {@code '_'})
		/// <li>  it is a digit
		/// <li>  it is a numeric letter (such as a Roman numeral character)
		/// <li>  it is a combining mark
		/// <li>  it is a non-spacing mark
		/// <li> {@code isIdentifierIgnorable} returns
		/// {@code true} for the character
		/// </ul>
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isJavaIdentifierPart(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">      the character to be tested. </param>
		/// <returns> {@code true} if the character may be part of a
		///          Java identifier; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isIdentifierIgnorable(char) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(char) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierPart(char) </seealso>
		/// <seealso cref=     javax.lang.model.SourceVersion#isIdentifier(CharSequence)
		/// @since   1.1 </seealso>
		public static bool IsJavaIdentifierPart(char ch)
		{
			return IsJavaIdentifierPart((int)ch);
		}

		/// <summary>
		/// Determines if the character (Unicode code point) may be part of a Java
		/// identifier as other than the first character.
		/// <para>
		/// A character may be part of a Java identifier if any of the following
		/// are true:
		/// <ul>
		/// <li>  it is a letter
		/// <li>  it is a currency symbol (such as {@code '$'})
		/// <li>  it is a connecting punctuation character (such as {@code '_'})
		/// <li>  it is a digit
		/// <li>  it is a numeric letter (such as a Roman numeral character)
		/// <li>  it is a combining mark
		/// <li>  it is a non-spacing mark
		/// <li> {@link #isIdentifierIgnorable(int)
		/// isIdentifierIgnorable(codePoint)} returns {@code true} for
		/// the character
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns> {@code true} if the character may be part of a
		///          Java identifier; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isIdentifierIgnorable(int) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierStart(int) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(int) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierPart(int) </seealso>
		/// <seealso cref=     javax.lang.model.SourceVersion#isIdentifier(CharSequence)
		/// @since   1.5 </seealso>
		public static bool IsJavaIdentifierPart(int codePoint)
		{
			return CharacterData.Of(codePoint).IsJavaIdentifierPart(codePoint);
		}

		/// <summary>
		/// Determines if the specified character is permissible as the
		/// first character in a Unicode identifier.
		/// <para>
		/// A character may start a Unicode identifier if and only if
		/// one of the following conditions is true:
		/// <ul>
		/// <li> <seealso cref="#isLetter(char) isLetter(ch)"/> returns {@code true}
		/// <li> <seealso cref="#getType(char) getType(ch)"/> returns
		///      {@code LETTER_NUMBER}.
		/// </ul>
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isUnicodeIdentifierStart(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">      the character to be tested. </param>
		/// <returns>  {@code true} if the character may start a Unicode
		///          identifier; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
		/// <seealso cref=     Character#isLetter(char) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierPart(char)
		/// @since   1.1 </seealso>
		public static bool IsUnicodeIdentifierStart(char ch)
		{
			return IsUnicodeIdentifierStart((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is permissible as the
		/// first character in a Unicode identifier.
		/// <para>
		/// A character may start a Unicode identifier if and only if
		/// one of the following conditions is true:
		/// <ul>
		/// <li> <seealso cref="#isLetter(int) isLetter(codePoint)"/>
		///      returns {@code true}
		/// <li> <seealso cref="#getType(int) getType(codePoint)"/>
		///      returns {@code LETTER_NUMBER}.
		/// </ul>
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character may start a Unicode
		///          identifier; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isJavaIdentifierStart(int) </seealso>
		/// <seealso cref=     Character#isLetter(int) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierPart(int)
		/// @since   1.5 </seealso>
		public static bool IsUnicodeIdentifierStart(int codePoint)
		{
			return CharacterData.Of(codePoint).IsUnicodeIdentifierStart(codePoint);
		}

		/// <summary>
		/// Determines if the specified character may be part of a Unicode
		/// identifier as other than the first character.
		/// <para>
		/// A character may be part of a Unicode identifier if and only if
		/// one of the following statements is true:
		/// <ul>
		/// <li>  it is a letter
		/// <li>  it is a connecting punctuation character (such as {@code '_'})
		/// <li>  it is a digit
		/// <li>  it is a numeric letter (such as a Roman numeral character)
		/// <li>  it is a combining mark
		/// <li>  it is a non-spacing mark
		/// <li> {@code isIdentifierIgnorable} returns
		/// {@code true} for this character.
		/// </ul>
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isUnicodeIdentifierPart(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">      the character to be tested. </param>
		/// <returns>  {@code true} if the character may be part of a
		///          Unicode identifier; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isIdentifierIgnorable(char) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(char) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierStart(char)
		/// @since   1.1 </seealso>
		public static bool IsUnicodeIdentifierPart(char ch)
		{
			return IsUnicodeIdentifierPart((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) may be part of a Unicode
		/// identifier as other than the first character.
		/// <para>
		/// A character may be part of a Unicode identifier if and only if
		/// one of the following statements is true:
		/// <ul>
		/// <li>  it is a letter
		/// <li>  it is a connecting punctuation character (such as {@code '_'})
		/// <li>  it is a digit
		/// <li>  it is a numeric letter (such as a Roman numeral character)
		/// <li>  it is a combining mark
		/// <li>  it is a non-spacing mark
		/// <li> {@code isIdentifierIgnorable} returns
		/// {@code true} for this character.
		/// </ul>
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character may be part of a
		///          Unicode identifier; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isIdentifierIgnorable(int) </seealso>
		/// <seealso cref=     Character#isJavaIdentifierPart(int) </seealso>
		/// <seealso cref=     Character#isLetterOrDigit(int) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierStart(int)
		/// @since   1.5 </seealso>
		public static bool IsUnicodeIdentifierPart(int codePoint)
		{
			return CharacterData.Of(codePoint).IsUnicodeIdentifierPart(codePoint);
		}

		/// <summary>
		/// Determines if the specified character should be regarded as
		/// an ignorable character in a Java identifier or a Unicode identifier.
		/// <para>
		/// The following Unicode characters are ignorable in a Java identifier
		/// or a Unicode identifier:
		/// <ul>
		/// <li>ISO control characters that are not whitespace
		/// <ul>
		/// <li>{@code '\u005Cu0000'} through {@code '\u005Cu0008'}
		/// <li>{@code '\u005Cu000E'} through {@code '\u005Cu001B'}
		/// <li>{@code '\u005Cu007F'} through {@code '\u005Cu009F'}
		/// </ul>
		/// 
		/// <li>all characters that have the {@code FORMAT} general
		/// category value
		/// </ul>
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isIdentifierIgnorable(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">      the character to be tested. </param>
		/// <returns>  {@code true} if the character is an ignorable control
		///          character that may be part of a Java or Unicode identifier;
		///           {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierPart(char)
		/// @since   1.1 </seealso>
		public static bool IsIdentifierIgnorable(char ch)
		{
			return IsIdentifierIgnorable((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) should be regarded as
		/// an ignorable character in a Java identifier or a Unicode identifier.
		/// <para>
		/// The following Unicode characters are ignorable in a Java identifier
		/// or a Unicode identifier:
		/// <ul>
		/// <li>ISO control characters that are not whitespace
		/// <ul>
		/// <li>{@code '\u005Cu0000'} through {@code '\u005Cu0008'}
		/// <li>{@code '\u005Cu000E'} through {@code '\u005Cu001B'}
		/// <li>{@code '\u005Cu007F'} through {@code '\u005Cu009F'}
		/// </ul>
		/// 
		/// <li>all characters that have the {@code FORMAT} general
		/// category value
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is an ignorable control
		///          character that may be part of a Java or Unicode identifier;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isJavaIdentifierPart(int) </seealso>
		/// <seealso cref=     Character#isUnicodeIdentifierPart(int)
		/// @since   1.5 </seealso>
		public static bool IsIdentifierIgnorable(int codePoint)
		{
			return CharacterData.Of(codePoint).IsIdentifierIgnorable(codePoint);
		}

		/// <summary>
		/// Converts the character argument to lowercase using case
		/// mapping information from the UnicodeData file.
		/// <para>
		/// Note that
		/// {@code Character.isLowerCase(Character.toLowerCase(ch))}
		/// does not always return {@code true} for some ranges of
		/// characters, particularly those that are symbols or ideographs.
		/// 
		/// </para>
		/// <para>In general, <seealso cref="String#toLowerCase()"/> should be used to map
		/// characters to lowercase. {@code String} case mapping methods
		/// have several benefits over {@code Character} case mapping methods.
		/// {@code String} case mapping methods can perform locale-sensitive
		/// mappings, context-sensitive mappings, and 1:M character mappings, whereas
		/// the {@code Character} case mapping methods cannot.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#toLowerCase(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be converted. </param>
		/// <returns>  the lowercase equivalent of the character, if any;
		///          otherwise, the character itself. </returns>
		/// <seealso cref=     Character#isLowerCase(char) </seealso>
		/// <seealso cref=     String#toLowerCase() </seealso>
		public static char ToLowerCase(char ch)
		{
			return (char)ToLowerCase((int)ch);
		}

		/// <summary>
		/// Converts the character (Unicode code point) argument to
		/// lowercase using case mapping information from the UnicodeData
		/// file.
		/// 
		/// <para> Note that
		/// {@code Character.isLowerCase(Character.toLowerCase(codePoint))}
		/// does not always return {@code true} for some ranges of
		/// characters, particularly those that are symbols or ideographs.
		/// 
		/// </para>
		/// <para>In general, <seealso cref="String#toLowerCase()"/> should be used to map
		/// characters to lowercase. {@code String} case mapping methods
		/// have several benefits over {@code Character} case mapping methods.
		/// {@code String} case mapping methods can perform locale-sensitive
		/// mappings, context-sensitive mappings, and 1:M character mappings, whereas
		/// the {@code Character} case mapping methods cannot.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint">   the character (Unicode code point) to be converted. </param>
		/// <returns>  the lowercase equivalent of the character (Unicode code
		///          point), if any; otherwise, the character itself. </returns>
		/// <seealso cref=     Character#isLowerCase(int) </seealso>
		/// <seealso cref=     String#toLowerCase()
		/// 
		/// @since   1.5 </seealso>
		public static int ToLowerCase(int codePoint)
		{
			return CharacterData.Of(codePoint).ToLowerCase(codePoint);
		}

		/// <summary>
		/// Converts the character argument to uppercase using case mapping
		/// information from the UnicodeData file.
		/// <para>
		/// Note that
		/// {@code Character.isUpperCase(Character.toUpperCase(ch))}
		/// does not always return {@code true} for some ranges of
		/// characters, particularly those that are symbols or ideographs.
		/// 
		/// </para>
		/// <para>In general, <seealso cref="String#toUpperCase()"/> should be used to map
		/// characters to uppercase. {@code String} case mapping methods
		/// have several benefits over {@code Character} case mapping methods.
		/// {@code String} case mapping methods can perform locale-sensitive
		/// mappings, context-sensitive mappings, and 1:M character mappings, whereas
		/// the {@code Character} case mapping methods cannot.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#toUpperCase(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be converted. </param>
		/// <returns>  the uppercase equivalent of the character, if any;
		///          otherwise, the character itself. </returns>
		/// <seealso cref=     Character#isUpperCase(char) </seealso>
		/// <seealso cref=     String#toUpperCase() </seealso>
		public static char ToUpperCase(char ch)
		{
			return (char)ToUpperCase((int)ch);
		}

		/// <summary>
		/// Converts the character (Unicode code point) argument to
		/// uppercase using case mapping information from the UnicodeData
		/// file.
		/// 
		/// <para>Note that
		/// {@code Character.isUpperCase(Character.toUpperCase(codePoint))}
		/// does not always return {@code true} for some ranges of
		/// characters, particularly those that are symbols or ideographs.
		/// 
		/// </para>
		/// <para>In general, <seealso cref="String#toUpperCase()"/> should be used to map
		/// characters to uppercase. {@code String} case mapping methods
		/// have several benefits over {@code Character} case mapping methods.
		/// {@code String} case mapping methods can perform locale-sensitive
		/// mappings, context-sensitive mappings, and 1:M character mappings, whereas
		/// the {@code Character} case mapping methods cannot.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint">   the character (Unicode code point) to be converted. </param>
		/// <returns>  the uppercase equivalent of the character, if any;
		///          otherwise, the character itself. </returns>
		/// <seealso cref=     Character#isUpperCase(int) </seealso>
		/// <seealso cref=     String#toUpperCase()
		/// 
		/// @since   1.5 </seealso>
		public static int ToUpperCase(int codePoint)
		{
			return CharacterData.Of(codePoint).ToUpperCase(codePoint);
		}

		/// <summary>
		/// Converts the character argument to titlecase using case mapping
		/// information from the UnicodeData file. If a character has no
		/// explicit titlecase mapping and is not itself a titlecase char
		/// according to UnicodeData, then the uppercase mapping is
		/// returned as an equivalent titlecase mapping. If the
		/// {@code char} argument is already a titlecase
		/// {@code char}, the same {@code char} value will be
		/// returned.
		/// <para>
		/// Note that
		/// {@code Character.isTitleCase(Character.toTitleCase(ch))}
		/// does not always return {@code true} for some ranges of
		/// characters.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#toTitleCase(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">   the character to be converted. </param>
		/// <returns>  the titlecase equivalent of the character, if any;
		///          otherwise, the character itself. </returns>
		/// <seealso cref=     Character#isTitleCase(char) </seealso>
		/// <seealso cref=     Character#toLowerCase(char) </seealso>
		/// <seealso cref=     Character#toUpperCase(char)
		/// @since   1.0.2 </seealso>
		public static char ToTitleCase(char ch)
		{
			return (char)ToTitleCase((int)ch);
		}

		/// <summary>
		/// Converts the character (Unicode code point) argument to titlecase using case mapping
		/// information from the UnicodeData file. If a character has no
		/// explicit titlecase mapping and is not itself a titlecase char
		/// according to UnicodeData, then the uppercase mapping is
		/// returned as an equivalent titlecase mapping. If the
		/// character argument is already a titlecase
		/// character, the same character value will be
		/// returned.
		/// 
		/// <para>Note that
		/// {@code Character.isTitleCase(Character.toTitleCase(codePoint))}
		/// does not always return {@code true} for some ranges of
		/// characters.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint">   the character (Unicode code point) to be converted. </param>
		/// <returns>  the titlecase equivalent of the character, if any;
		///          otherwise, the character itself. </returns>
		/// <seealso cref=     Character#isTitleCase(int) </seealso>
		/// <seealso cref=     Character#toLowerCase(int) </seealso>
		/// <seealso cref=     Character#toUpperCase(int)
		/// @since   1.5 </seealso>
		public static int ToTitleCase(int codePoint)
		{
			return CharacterData.Of(codePoint).ToTitleCase(codePoint);
		}

		/// <summary>
		/// Returns the numeric value of the character {@code ch} in the
		/// specified radix.
		/// <para>
		/// If the radix is not in the range {@code MIN_RADIX} &le;
		/// {@code radix} &le; {@code MAX_RADIX} or if the
		/// value of {@code ch} is not a valid digit in the specified
		/// radix, {@code -1} is returned. A character is a valid digit
		/// if at least one of the following is true:
		/// <ul>
		/// <li>The method {@code isDigit} is {@code true} of the character
		///     and the Unicode decimal digit value of the character (or its
		///     single-character decomposition) is less than the specified radix.
		///     In this case the decimal digit value is returned.
		/// <li>The character is one of the uppercase Latin letters
		///     {@code 'A'} through {@code 'Z'} and its code is less than
		///     {@code radix + 'A' - 10}.
		///     In this case, {@code ch - 'A' + 10}
		///     is returned.
		/// <li>The character is one of the lowercase Latin letters
		///     {@code 'a'} through {@code 'z'} and its code is less than
		///     {@code radix + 'a' - 10}.
		///     In this case, {@code ch - 'a' + 10}
		///     is returned.
		/// <li>The character is one of the fullwidth uppercase Latin letters A
		///     ({@code '\u005CuFF21'}) through Z ({@code '\u005CuFF3A'})
		///     and its code is less than
		///     {@code radix + '\u005CuFF21' - 10}.
		///     In this case, {@code ch - '\u005CuFF21' + 10}
		///     is returned.
		/// <li>The character is one of the fullwidth lowercase Latin letters a
		///     ({@code '\u005CuFF41'}) through z ({@code '\u005CuFF5A'})
		///     and its code is less than
		///     {@code radix + '\u005CuFF41' - 10}.
		///     In this case, {@code ch - '\u005CuFF41' + 10}
		///     is returned.
		/// </ul>
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#digit(int, int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">      the character to be converted. </param>
		/// <param name="radix">   the radix. </param>
		/// <returns>  the numeric value represented by the character in the
		///          specified radix. </returns>
		/// <seealso cref=     Character#forDigit(int, int) </seealso>
		/// <seealso cref=     Character#isDigit(char) </seealso>
		public static int Digit(char ch, int radix)
		{
			return Digit((int)ch, radix);
		}

		/// <summary>
		/// Returns the numeric value of the specified character (Unicode
		/// code point) in the specified radix.
		/// 
		/// <para>If the radix is not in the range {@code MIN_RADIX} &le;
		/// {@code radix} &le; {@code MAX_RADIX} or if the
		/// character is not a valid digit in the specified
		/// radix, {@code -1} is returned. A character is a valid digit
		/// if at least one of the following is true:
		/// <ul>
		/// <li>The method <seealso cref="#isDigit(int) isDigit(codePoint)"/> is {@code true} of the character
		///     and the Unicode decimal digit value of the character (or its
		///     single-character decomposition) is less than the specified radix.
		///     In this case the decimal digit value is returned.
		/// <li>The character is one of the uppercase Latin letters
		///     {@code 'A'} through {@code 'Z'} and its code is less than
		///     {@code radix + 'A' - 10}.
		///     In this case, {@code codePoint - 'A' + 10}
		///     is returned.
		/// <li>The character is one of the lowercase Latin letters
		///     {@code 'a'} through {@code 'z'} and its code is less than
		///     {@code radix + 'a' - 10}.
		///     In this case, {@code codePoint - 'a' + 10}
		///     is returned.
		/// <li>The character is one of the fullwidth uppercase Latin letters A
		///     ({@code '\u005CuFF21'}) through Z ({@code '\u005CuFF3A'})
		///     and its code is less than
		///     {@code radix + '\u005CuFF21' - 10}.
		///     In this case,
		///     {@code codePoint - '\u005CuFF21' + 10}
		///     is returned.
		/// <li>The character is one of the fullwidth lowercase Latin letters a
		///     ({@code '\u005CuFF41'}) through z ({@code '\u005CuFF5A'})
		///     and its code is less than
		///     {@code radix + '\u005CuFF41'- 10}.
		///     In this case,
		///     {@code codePoint - '\u005CuFF41' + 10}
		///     is returned.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be converted. </param>
		/// <param name="radix">   the radix. </param>
		/// <returns>  the numeric value represented by the character in the
		///          specified radix. </returns>
		/// <seealso cref=     Character#forDigit(int, int) </seealso>
		/// <seealso cref=     Character#isDigit(int)
		/// @since   1.5 </seealso>
		public static int Digit(int codePoint, int radix)
		{
			return CharacterData.Of(codePoint).Digit(codePoint, radix);
		}

		/// <summary>
		/// Returns the {@code int} value that the specified Unicode
		/// character represents. For example, the character
		/// {@code '\u005Cu216C'} (the roman numeral fifty) will return
		/// an int with a value of 50.
		/// <para>
		/// The letters A-Z in their uppercase ({@code '\u005Cu0041'} through
		/// {@code '\u005Cu005A'}), lowercase
		/// ({@code '\u005Cu0061'} through {@code '\u005Cu007A'}), and
		/// full width variant ({@code '\u005CuFF21'} through
		/// {@code '\u005CuFF3A'} and {@code '\u005CuFF41'} through
		/// {@code '\u005CuFF5A'}) forms have numeric values from 10
		/// through 35. This is independent of the Unicode specification,
		/// which does not assign numeric values to these {@code char}
		/// values.
		/// </para>
		/// <para>
		/// If the character does not have a numeric value, then -1 is returned.
		/// If the character has a numeric value that cannot be represented as a
		/// nonnegative integer (for example, a fractional value), then -2
		/// is returned.
		/// 
		/// </para>
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#getNumericValue(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">      the character to be converted. </param>
		/// <returns>  the numeric value of the character, as a nonnegative {@code int}
		///           value; -2 if the character has a numeric value that is not a
		///          nonnegative integer; -1 if the character has no numeric value. </returns>
		/// <seealso cref=     Character#forDigit(int, int) </seealso>
		/// <seealso cref=     Character#isDigit(char)
		/// @since   1.1 </seealso>
		public static int GetNumericValue(char ch)
		{
			return GetNumericValue((int)ch);
		}

		/// <summary>
		/// Returns the {@code int} value that the specified
		/// character (Unicode code point) represents. For example, the character
		/// {@code '\u005Cu216C'} (the Roman numeral fifty) will return
		/// an {@code int} with a value of 50.
		/// <para>
		/// The letters A-Z in their uppercase ({@code '\u005Cu0041'} through
		/// {@code '\u005Cu005A'}), lowercase
		/// ({@code '\u005Cu0061'} through {@code '\u005Cu007A'}), and
		/// full width variant ({@code '\u005CuFF21'} through
		/// {@code '\u005CuFF3A'} and {@code '\u005CuFF41'} through
		/// {@code '\u005CuFF5A'}) forms have numeric values from 10
		/// through 35. This is independent of the Unicode specification,
		/// which does not assign numeric values to these {@code char}
		/// values.
		/// </para>
		/// <para>
		/// If the character does not have a numeric value, then -1 is returned.
		/// If the character has a numeric value that cannot be represented as a
		/// nonnegative integer (for example, a fractional value), then -2
		/// is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be converted. </param>
		/// <returns>  the numeric value of the character, as a nonnegative {@code int}
		///          value; -2 if the character has a numeric value that is not a
		///          nonnegative integer; -1 if the character has no numeric value. </returns>
		/// <seealso cref=     Character#forDigit(int, int) </seealso>
		/// <seealso cref=     Character#isDigit(int)
		/// @since   1.5 </seealso>
		public static int GetNumericValue(int codePoint)
		{
			return CharacterData.Of(codePoint).GetNumericValue(codePoint);
		}

		/// <summary>
		/// Determines if the specified character is ISO-LATIN-1 white space.
		/// This method returns {@code true} for the following five
		/// characters only:
		/// <table summary="truechars">
		/// <tr><td>{@code '\t'}</td>            <td>{@code U+0009}</td>
		///     <td>{@code HORIZONTAL TABULATION}</td></tr>
		/// <tr><td>{@code '\n'}</td>            <td>{@code U+000A}</td>
		///     <td>{@code NEW LINE}</td></tr>
		/// <tr><td>{@code '\f'}</td>            <td>{@code U+000C}</td>
		///     <td>{@code FORM FEED}</td></tr>
		/// <tr><td>{@code '\r'}</td>            <td>{@code U+000D}</td>
		///     <td>{@code CARRIAGE RETURN}</td></tr>
		/// <tr><td>{@code '&nbsp;'}</td>  <td>{@code U+0020}</td>
		///     <td>{@code SPACE}</td></tr>
		/// </table>
		/// </summary>
		/// <param name="ch">   the character to be tested. </param>
		/// <returns>     {@code true} if the character is ISO-LATIN-1 white
		///             space; {@code false} otherwise. </returns>
		/// <seealso cref=        Character#isSpaceChar(char) </seealso>
		/// <seealso cref=        Character#isWhitespace(char) </seealso>
		/// @deprecated Replaced by isWhitespace(char). 
		[Obsolete("Replaced by isWhitespace(char).")]
		public static bool IsSpace(char ch)
		{
			return (ch <= 0x0020) && (((((1L << 0x0009) | (1L << 0x000A) | (1L << 0x000C) | (1L << 0x000D) | (1L << 0x0020)) >> ch) & 1L) != 0);
		}


		/// <summary>
		/// Determines if the specified character is a Unicode space character.
		/// A character is considered to be a space character if and only if
		/// it is specified to be a space character by the Unicode Standard. This
		/// method returns true if the character's general category type is any of
		/// the following:
		/// <ul>
		/// <li> {@code SPACE_SEPARATOR}
		/// <li> {@code LINE_SEPARATOR}
		/// <li> {@code PARAGRAPH_SEPARATOR}
		/// </ul>
		/// 
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isSpaceChar(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">      the character to be tested. </param>
		/// <returns>  {@code true} if the character is a space character;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isWhitespace(char)
		/// @since   1.1 </seealso>
		public static bool IsSpaceChar(char ch)
		{
			return IsSpaceChar((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is a
		/// Unicode space character.  A character is considered to be a
		/// space character if and only if it is specified to be a space
		/// character by the Unicode Standard. This method returns true if
		/// the character's general category type is any of the following:
		/// 
		/// <ul>
		/// <li> <seealso cref="#SPACE_SEPARATOR"/>
		/// <li> <seealso cref="#LINE_SEPARATOR"/>
		/// <li> <seealso cref="#PARAGRAPH_SEPARATOR"/>
		/// </ul>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is a space character;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isWhitespace(int)
		/// @since   1.5 </seealso>
		public static bool IsSpaceChar(int codePoint)
		{
			return ((((1 << Character.SPACE_SEPARATOR) | (1 << Character.LINE_SEPARATOR) | (1 << Character.PARAGRAPH_SEPARATOR)) >> GetType(codePoint)) & 1) != 0;
		}

		/// <summary>
		/// Determines if the specified character is white space according to Java.
		/// A character is a Java whitespace character if and only if it satisfies
		/// one of the following criteria:
		/// <ul>
		/// <li> It is a Unicode space character ({@code SPACE_SEPARATOR},
		///      {@code LINE_SEPARATOR}, or {@code PARAGRAPH_SEPARATOR})
		///      but is not also a non-breaking space ({@code '\u005Cu00A0'},
		///      {@code '\u005Cu2007'}, {@code '\u005Cu202F'}).
		/// <li> It is {@code '\u005Ct'}, U+0009 HORIZONTAL TABULATION.
		/// <li> It is {@code '\u005Cn'}, U+000A LINE FEED.
		/// <li> It is {@code '\u005Cu000B'}, U+000B VERTICAL TABULATION.
		/// <li> It is {@code '\u005Cf'}, U+000C FORM FEED.
		/// <li> It is {@code '\u005Cr'}, U+000D CARRIAGE RETURN.
		/// <li> It is {@code '\u005Cu001C'}, U+001C FILE SEPARATOR.
		/// <li> It is {@code '\u005Cu001D'}, U+001D GROUP SEPARATOR.
		/// <li> It is {@code '\u005Cu001E'}, U+001E RECORD SEPARATOR.
		/// <li> It is {@code '\u005Cu001F'}, U+001F UNIT SEPARATOR.
		/// </ul>
		/// 
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isWhitespace(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> the character to be tested. </param>
		/// <returns>  {@code true} if the character is a Java whitespace
		///          character; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isSpaceChar(char)
		/// @since   1.1 </seealso>
		public static bool IsWhitespace(char ch)
		{
			return IsWhitespace((int)ch);
		}

		/// <summary>
		/// Determines if the specified character (Unicode code point) is
		/// white space according to Java.  A character is a Java
		/// whitespace character if and only if it satisfies one of the
		/// following criteria:
		/// <ul>
		/// <li> It is a Unicode space character (<seealso cref="#SPACE_SEPARATOR"/>,
		///      <seealso cref="#LINE_SEPARATOR"/>, or <seealso cref="#PARAGRAPH_SEPARATOR"/>)
		///      but is not also a non-breaking space ({@code '\u005Cu00A0'},
		///      {@code '\u005Cu2007'}, {@code '\u005Cu202F'}).
		/// <li> It is {@code '\u005Ct'}, U+0009 HORIZONTAL TABULATION.
		/// <li> It is {@code '\u005Cn'}, U+000A LINE FEED.
		/// <li> It is {@code '\u005Cu000B'}, U+000B VERTICAL TABULATION.
		/// <li> It is {@code '\u005Cf'}, U+000C FORM FEED.
		/// <li> It is {@code '\u005Cr'}, U+000D CARRIAGE RETURN.
		/// <li> It is {@code '\u005Cu001C'}, U+001C FILE SEPARATOR.
		/// <li> It is {@code '\u005Cu001D'}, U+001D GROUP SEPARATOR.
		/// <li> It is {@code '\u005Cu001E'}, U+001E RECORD SEPARATOR.
		/// <li> It is {@code '\u005Cu001F'}, U+001F UNIT SEPARATOR.
		/// </ul>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is a Java whitespace
		///          character; {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isSpaceChar(int)
		/// @since   1.5 </seealso>
		public static bool IsWhitespace(int codePoint)
		{
			return CharacterData.Of(codePoint).IsWhitespace(codePoint);
		}

		/// <summary>
		/// Determines if the specified character is an ISO control
		/// character.  A character is considered to be an ISO control
		/// character if its code is in the range {@code '\u005Cu0000'}
		/// through {@code '\u005Cu001F'} or in the range
		/// {@code '\u005Cu007F'} through {@code '\u005Cu009F'}.
		/// 
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isISOControl(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">      the character to be tested. </param>
		/// <returns>  {@code true} if the character is an ISO control character;
		///          {@code false} otherwise.
		/// </returns>
		/// <seealso cref=     Character#isSpaceChar(char) </seealso>
		/// <seealso cref=     Character#isWhitespace(char)
		/// @since   1.1 </seealso>
		public static bool IsISOControl(char ch)
		{
			return IsISOControl((int)ch);
		}

		/// <summary>
		/// Determines if the referenced character (Unicode code point) is an ISO control
		/// character.  A character is considered to be an ISO control
		/// character if its code is in the range {@code '\u005Cu0000'}
		/// through {@code '\u005Cu001F'} or in the range
		/// {@code '\u005Cu007F'} through {@code '\u005Cu009F'}.
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is an ISO control character;
		///          {@code false} otherwise. </returns>
		/// <seealso cref=     Character#isSpaceChar(int) </seealso>
		/// <seealso cref=     Character#isWhitespace(int)
		/// @since   1.5 </seealso>
		public static bool IsISOControl(int codePoint)
		{
			// Optimized form of:
			//     (codePoint >= 0x00 && codePoint <= 0x1F) ||
			//     (codePoint >= 0x7F && codePoint <= 0x9F);
			return codePoint <= 0x9F && (codePoint >= 0x7F || ((int)((uint)codePoint >> 5) == 0));
		}

		/// <summary>
		/// Returns a value indicating a character's general category.
		/// 
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#getType(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">      the character to be tested. </param>
		/// <returns>  a value of type {@code int} representing the
		///          character's general category. </returns>
		/// <seealso cref=     Character#COMBINING_SPACING_MARK </seealso>
		/// <seealso cref=     Character#CONNECTOR_PUNCTUATION </seealso>
		/// <seealso cref=     Character#CONTROL </seealso>
		/// <seealso cref=     Character#CURRENCY_SYMBOL </seealso>
		/// <seealso cref=     Character#DASH_PUNCTUATION </seealso>
		/// <seealso cref=     Character#DECIMAL_DIGIT_NUMBER </seealso>
		/// <seealso cref=     Character#ENCLOSING_MARK </seealso>
		/// <seealso cref=     Character#END_PUNCTUATION </seealso>
		/// <seealso cref=     Character#FINAL_QUOTE_PUNCTUATION </seealso>
		/// <seealso cref=     Character#FORMAT </seealso>
		/// <seealso cref=     Character#INITIAL_QUOTE_PUNCTUATION </seealso>
		/// <seealso cref=     Character#LETTER_NUMBER </seealso>
		/// <seealso cref=     Character#LINE_SEPARATOR </seealso>
		/// <seealso cref=     Character#LOWERCASE_LETTER </seealso>
		/// <seealso cref=     Character#MATH_SYMBOL </seealso>
		/// <seealso cref=     Character#MODIFIER_LETTER </seealso>
		/// <seealso cref=     Character#MODIFIER_SYMBOL </seealso>
		/// <seealso cref=     Character#NON_SPACING_MARK </seealso>
		/// <seealso cref=     Character#OTHER_LETTER </seealso>
		/// <seealso cref=     Character#OTHER_NUMBER </seealso>
		/// <seealso cref=     Character#OTHER_PUNCTUATION </seealso>
		/// <seealso cref=     Character#OTHER_SYMBOL </seealso>
		/// <seealso cref=     Character#PARAGRAPH_SEPARATOR </seealso>
		/// <seealso cref=     Character#PRIVATE_USE </seealso>
		/// <seealso cref=     Character#SPACE_SEPARATOR </seealso>
		/// <seealso cref=     Character#START_PUNCTUATION </seealso>
		/// <seealso cref=     Character#SURROGATE </seealso>
		/// <seealso cref=     Character#TITLECASE_LETTER </seealso>
		/// <seealso cref=     Character#UNASSIGNED </seealso>
		/// <seealso cref=     Character#UPPERCASE_LETTER
		/// @since   1.1 </seealso>
		public static int GetType(char ch)
		{
			return GetType((int)ch);
		}

		/// <summary>
		/// Returns a value indicating a character's general category.
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  a value of type {@code int} representing the
		///          character's general category. </returns>
		/// <seealso cref=     Character#COMBINING_SPACING_MARK COMBINING_SPACING_MARK </seealso>
		/// <seealso cref=     Character#CONNECTOR_PUNCTUATION CONNECTOR_PUNCTUATION </seealso>
		/// <seealso cref=     Character#CONTROL CONTROL </seealso>
		/// <seealso cref=     Character#CURRENCY_SYMBOL CURRENCY_SYMBOL </seealso>
		/// <seealso cref=     Character#DASH_PUNCTUATION DASH_PUNCTUATION </seealso>
		/// <seealso cref=     Character#DECIMAL_DIGIT_NUMBER DECIMAL_DIGIT_NUMBER </seealso>
		/// <seealso cref=     Character#ENCLOSING_MARK ENCLOSING_MARK </seealso>
		/// <seealso cref=     Character#END_PUNCTUATION END_PUNCTUATION </seealso>
		/// <seealso cref=     Character#FINAL_QUOTE_PUNCTUATION FINAL_QUOTE_PUNCTUATION </seealso>
		/// <seealso cref=     Character#FORMAT FORMAT </seealso>
		/// <seealso cref=     Character#INITIAL_QUOTE_PUNCTUATION INITIAL_QUOTE_PUNCTUATION </seealso>
		/// <seealso cref=     Character#LETTER_NUMBER LETTER_NUMBER </seealso>
		/// <seealso cref=     Character#LINE_SEPARATOR LINE_SEPARATOR </seealso>
		/// <seealso cref=     Character#LOWERCASE_LETTER LOWERCASE_LETTER </seealso>
		/// <seealso cref=     Character#MATH_SYMBOL MATH_SYMBOL </seealso>
		/// <seealso cref=     Character#MODIFIER_LETTER MODIFIER_LETTER </seealso>
		/// <seealso cref=     Character#MODIFIER_SYMBOL MODIFIER_SYMBOL </seealso>
		/// <seealso cref=     Character#NON_SPACING_MARK NON_SPACING_MARK </seealso>
		/// <seealso cref=     Character#OTHER_LETTER OTHER_LETTER </seealso>
		/// <seealso cref=     Character#OTHER_NUMBER OTHER_NUMBER </seealso>
		/// <seealso cref=     Character#OTHER_PUNCTUATION OTHER_PUNCTUATION </seealso>
		/// <seealso cref=     Character#OTHER_SYMBOL OTHER_SYMBOL </seealso>
		/// <seealso cref=     Character#PARAGRAPH_SEPARATOR PARAGRAPH_SEPARATOR </seealso>
		/// <seealso cref=     Character#PRIVATE_USE PRIVATE_USE </seealso>
		/// <seealso cref=     Character#SPACE_SEPARATOR SPACE_SEPARATOR </seealso>
		/// <seealso cref=     Character#START_PUNCTUATION START_PUNCTUATION </seealso>
		/// <seealso cref=     Character#SURROGATE SURROGATE </seealso>
		/// <seealso cref=     Character#TITLECASE_LETTER TITLECASE_LETTER </seealso>
		/// <seealso cref=     Character#UNASSIGNED UNASSIGNED </seealso>
		/// <seealso cref=     Character#UPPERCASE_LETTER UPPERCASE_LETTER
		/// @since   1.5 </seealso>
		public static int GetType(int codePoint)
		{
			return CharacterData.Of(codePoint).GetType(codePoint);
		}

		/// <summary>
		/// Determines the character representation for a specific digit in
		/// the specified radix. If the value of {@code radix} is not a
		/// valid radix, or the value of {@code digit} is not a valid
		/// digit in the specified radix, the null character
		/// ({@code '\u005Cu0000'}) is returned.
		/// <para>
		/// The {@code radix} argument is valid if it is greater than or
		/// equal to {@code MIN_RADIX} and less than or equal to
		/// {@code MAX_RADIX}. The {@code digit} argument is valid if
		/// {@code 0 <= digit < radix}.
		/// </para>
		/// <para>
		/// If the digit is less than 10, then
		/// {@code '0' + digit} is returned. Otherwise, the value
		/// {@code 'a' + digit - 10} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="digit">   the number to convert to a character. </param>
		/// <param name="radix">   the radix. </param>
		/// <returns>  the {@code char} representation of the specified digit
		///          in the specified radix. </returns>
		/// <seealso cref=     Character#MIN_RADIX </seealso>
		/// <seealso cref=     Character#MAX_RADIX </seealso>
		/// <seealso cref=     Character#digit(char, int) </seealso>
		public static char ForDigit(int digit, int radix)
		{
			if ((digit >= radix) || (digit < 0))
			{
				return '\0';
			}
			if ((radix < Character.MIN_RADIX) || (radix > Character.MAX_RADIX))
			{
				return '\0';
			}
			if (digit < 10)
			{
				return (char)('0' + digit);
			}
			return (char)('a' - 10 + digit);
		}

		/// <summary>
		/// Returns the Unicode directionality property for the given
		/// character.  Character directionality is used to calculate the
		/// visual ordering of text. The directionality value of undefined
		/// {@code char} values is {@code DIRECTIONALITY_UNDEFINED}.
		/// 
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#getDirectionality(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> {@code char} for which the directionality property
		///            is requested. </param>
		/// <returns> the directionality property of the {@code char} value.
		/// </returns>
		/// <seealso cref= Character#DIRECTIONALITY_UNDEFINED </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_ARABIC_NUMBER </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_COMMON_NUMBER_SEPARATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_NONSPACING_MARK </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_BOUNDARY_NEUTRAL </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_PARAGRAPH_SEPARATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_SEGMENT_SEPARATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_WHITESPACE </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_OTHER_NEUTRALS </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_POP_DIRECTIONAL_FORMAT
		/// @since 1.4 </seealso>
		public static sbyte GetDirectionality(char ch)
		{
			return GetDirectionality((int)ch);
		}

		/// <summary>
		/// Returns the Unicode directionality property for the given
		/// character (Unicode code point).  Character directionality is
		/// used to calculate the visual ordering of text. The
		/// directionality value of undefined character is {@link
		/// #DIRECTIONALITY_UNDEFINED}.
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) for which
		///          the directionality property is requested. </param>
		/// <returns> the directionality property of the character.
		/// </returns>
		/// <seealso cref= Character#DIRECTIONALITY_UNDEFINED DIRECTIONALITY_UNDEFINED </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT DIRECTIONALITY_LEFT_TO_RIGHT </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT DIRECTIONALITY_RIGHT_TO_LEFT </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER DIRECTIONALITY_EUROPEAN_NUMBER </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_ARABIC_NUMBER DIRECTIONALITY_ARABIC_NUMBER </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_COMMON_NUMBER_SEPARATOR DIRECTIONALITY_COMMON_NUMBER_SEPARATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_NONSPACING_MARK DIRECTIONALITY_NONSPACING_MARK </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_BOUNDARY_NEUTRAL DIRECTIONALITY_BOUNDARY_NEUTRAL </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_PARAGRAPH_SEPARATOR DIRECTIONALITY_PARAGRAPH_SEPARATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_SEGMENT_SEPARATOR DIRECTIONALITY_SEGMENT_SEPARATOR </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_WHITESPACE DIRECTIONALITY_WHITESPACE </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_OTHER_NEUTRALS DIRECTIONALITY_OTHER_NEUTRALS </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE </seealso>
		/// <seealso cref= Character#DIRECTIONALITY_POP_DIRECTIONAL_FORMAT DIRECTIONALITY_POP_DIRECTIONAL_FORMAT
		/// @since    1.5 </seealso>
		public static sbyte GetDirectionality(int codePoint)
		{
			return CharacterData.Of(codePoint).GetDirectionality(codePoint);
		}

		/// <summary>
		/// Determines whether the character is mirrored according to the
		/// Unicode specification.  Mirrored characters should have their
		/// glyphs horizontally mirrored when displayed in text that is
		/// right-to-left.  For example, {@code '\u005Cu0028'} LEFT
		/// PARENTHESIS is semantically defined to be an <i>opening
		/// parenthesis</i>.  This will appear as a "(" in text that is
		/// left-to-right but as a ")" in text that is right-to-left.
		/// 
		/// <para><b>Note:</b> This method cannot handle <a
		/// href="#supplementary"> supplementary characters</a>. To support
		/// all Unicode characters, including supplementary characters, use
		/// the <seealso cref="#isMirrored(int)"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch"> {@code char} for which the mirrored property is requested </param>
		/// <returns> {@code true} if the char is mirrored, {@code false}
		///         if the {@code char} is not mirrored or is not defined.
		/// @since 1.4 </returns>
		public static bool IsMirrored(char ch)
		{
			return IsMirrored((int)ch);
		}

		/// <summary>
		/// Determines whether the specified character (Unicode code point)
		/// is mirrored according to the Unicode specification.  Mirrored
		/// characters should have their glyphs horizontally mirrored when
		/// displayed in text that is right-to-left.  For example,
		/// {@code '\u005Cu0028'} LEFT PARENTHESIS is semantically
		/// defined to be an <i>opening parenthesis</i>.  This will appear
		/// as a "(" in text that is left-to-right but as a ")" in text
		/// that is right-to-left.
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point) to be tested. </param>
		/// <returns>  {@code true} if the character is mirrored, {@code false}
		///          if the character is not mirrored or is not defined.
		/// @since   1.5 </returns>
		public static bool IsMirrored(int codePoint)
		{
			return CharacterData.Of(codePoint).IsMirrored(codePoint);
		}

		/// <summary>
		/// Compares two {@code Character} objects numerically.
		/// </summary>
		/// <param name="anotherCharacter">   the {@code Character} to be compared.
		/// </param>
		/// <returns>  the value {@code 0} if the argument {@code Character}
		///          is equal to this {@code Character}; a value less than
		///          {@code 0} if this {@code Character} is numerically less
		///          than the {@code Character} argument; and a value greater than
		///          {@code 0} if this {@code Character} is numerically greater
		///          than the {@code Character} argument (unsigned comparison).
		///          Note that this is strictly a numerical comparison; it is not
		///          locale-dependent.
		/// @since   1.2 </returns>
		public int CompareTo(Character anotherCharacter)
		{
			return Compare(this.Value, anotherCharacter.Value);
		}

		/// <summary>
		/// Compares two {@code char} values numerically.
		/// The value returned is identical to what would be returned by:
		/// <pre>
		///    Character.valueOf(x).compareTo(Character.valueOf(y))
		/// </pre>
		/// </summary>
		/// <param name="x"> the first {@code char} to compare </param>
		/// <param name="y"> the second {@code char} to compare </param>
		/// <returns> the value {@code 0} if {@code x == y};
		///         a value less than {@code 0} if {@code x < y}; and
		///         a value greater than {@code 0} if {@code x > y}
		/// @since 1.7 </returns>
		public static int Compare(char x, char y)
		{
			return x - y;
		}

		/// <summary>
		/// Converts the character (Unicode code point) argument to uppercase using
		/// information from the UnicodeData file.
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint">   the character (Unicode code point) to be converted. </param>
		/// <returns>  either the uppercase equivalent of the character, if
		///          any, or an error flag ({@code Character.ERROR})
		///          that indicates that a 1:M {@code char} mapping exists. </returns>
		/// <seealso cref=     Character#isLowerCase(char) </seealso>
		/// <seealso cref=     Character#isUpperCase(char) </seealso>
		/// <seealso cref=     Character#toLowerCase(char) </seealso>
		/// <seealso cref=     Character#toTitleCase(char)
		/// @since 1.4 </seealso>
		static int ToUpperCaseEx(int codePoint)
		{
			Debug.Assert(IsValidCodePoint(codePoint));
			return CharacterData.Of(codePoint).ToUpperCaseEx(codePoint);
		}

		/// <summary>
		/// Converts the character (Unicode code point) argument to uppercase using case
		/// mapping information from the SpecialCasing file in the Unicode
		/// specification. If a character has no explicit uppercase
		/// mapping, then the {@code char} itself is returned in the
		/// {@code char[]}.
		/// </summary>
		/// <param name="codePoint">   the character (Unicode code point) to be converted. </param>
		/// <returns> a {@code char[]} with the uppercased character.
		/// @since 1.4 </returns>
		static char[] ToUpperCaseCharArray(int codePoint)
		{
			// As of Unicode 6.0, 1:M uppercasings only happen in the BMP.
			Debug.Assert(IsBmpCodePoint(codePoint));
			return CharacterData.Of(codePoint).ToUpperCaseCharArray(codePoint);
		}

		/// <summary>
		/// The number of bits used to represent a <tt>char</tt> value in unsigned
		/// binary form, constant {@code 16}.
		/// 
		/// @since 1.5
		/// </summary>
		public static final int SIZE = 16;

		/// <summary>
		/// The number of bytes used to represent a {@code char} value in unsigned
		/// binary form.
		/// 
		/// @since 1.8
		/// </summary>
		public static final int BYTES = SIZE / sizeof(sbyte);

		/// <summary>
		/// Returns the value obtained by reversing the order of the bytes in the
		/// specified <tt>char</tt> value.
		/// </summary>
		/// <param name="ch"> The {@code char} of which to reverse the byte order. </param>
		/// <returns> the value obtained by reversing (or, equivalently, swapping)
		///     the bytes in the specified <tt>char</tt> value.
		/// @since 1.5 </returns>
		public static char ReverseBytes(char ch)
		{
			return (char)(((ch & 0xFF00) >> 8) | (ch << 8));
		}

		/// <summary>
		/// Returns the Unicode name of the specified character
		/// {@code codePoint}, or null if the code point is
		/// <seealso cref="#UNASSIGNED unassigned"/>.
		/// <para>
		/// Note: if the specified character is not assigned a name by
		/// the <i>UnicodeData</i> file (part of the Unicode Character
		/// Database maintained by the Unicode Consortium), the returned
		/// name is the same as the result of expression.
		/// 
		/// <blockquote>{@code
		///     Character.UnicodeBlock.of(codePoint).toString().replace('_', ' ')
		///     + " "
		///     + Integer.toHexString(codePoint).toUpperCase(Locale.ENGLISH);
		/// 
		/// }</blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint"> the character (Unicode code point)
		/// </param>
		/// <returns> the Unicode name of the specified character, or null if
		///         the code point is unassigned.
		/// </returns>
		/// <exception cref="IllegalArgumentException"> if the specified
		///            {@code codePoint} is not a valid Unicode
		///            code point.
		/// 
		/// @since 1.7 </exception>
		public static String GetName(int codePoint)
		{
			if (!IsValidCodePoint(codePoint))
			{
				throw new IllegalArgumentException();
			}
			String name = CharacterName.Get(codePoint);
			if (name != null)
			{
				return name;
			}
			if (GetType(codePoint) == UNASSIGNED)
			{
				return null;
			}
			UnicodeBlock block = UnicodeBlock.of(codePoint);
			if (block != null)
			{
				return block.ToString().Replace('_', ' ') + " " + codePoint.ToString("x").ToUpperCase(Locale.ENGLISH);
			}
			// should never come here
			return codePoint.ToString("x").ToUpperCase(Locale.ENGLISH);
		}
}

}