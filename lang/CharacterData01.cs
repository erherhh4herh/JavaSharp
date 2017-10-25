// This file was generated AUTOMATICALLY from a template file Fri Jan 29 17:43:04 PST 2016
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

	/// <summary>
	/// The CharacterData class encapsulates the large tables once found in
	///  java.lang.Character. 
	/// </summary>

	internal class CharacterData01 : CharacterData
	{
		/* The character properties are currently encoded into 32 bits in the following manner:
		    1 bit   mirrored property
		    4 bits  directionality property
		    9 bits  signed offset used for converting case
		    1 bit   if 1, adding the signed offset converts the character to lowercase
		    1 bit   if 1, subtracting the signed offset converts the character to uppercase
		    1 bit   if 1, this character has a titlecase equivalent (possibly itself)
		    3 bits  0  may not be part of an identifier
		            1  ignorable control; may continue a Unicode identifier or Java identifier
		            2  may continue a Java identifier but not a Unicode identifier (unused)
		            3  may continue a Unicode identifier or Java identifier
		            4  is a Java whitespace character
		            5  may start or continue a Java identifier;
		               may continue but not start a Unicode identifier (underscores)
		            6  may start or continue a Java identifier but not a Unicode identifier ($)
		            7  may start or continue a Unicode identifier or Java identifier
		            Thus:
		               5, 6, 7 may start a Java identifier
		               1, 2, 3, 5, 6, 7 may continue a Java identifier
		               7 may start a Unicode identifier
		               1, 3, 5, 7 may continue a Unicode identifier
		               1 is ignorable within an identifier
		               4 is Java whitespace
		    2 bits  0  this character has no numeric property
		            1  adding the digit offset to the character code and then
		               masking with 0x1F will produce the desired numeric value
		            2  this character has a "strange" numeric value
		            3  a Java supradecimal digit: adding the digit offset to the
		               character code, then masking with 0x1F, then adding 10
		               will produce the desired numeric value
		    5 bits  digit offset
		    5 bits  character type
	
		    The encoding of character properties is subject to change at any time.
		 */

		internal override int GetProperties(int ch)
		{
			char offset = (char)ch;
			int props = A[(Y[(X[offset >> 5] << 4) | ((offset>>1) & 0xF)] << 1) | (offset & 0x1)];
			return props;
		}

		internal virtual int GetPropertiesEx(int ch)
		{
			char offset = (char)ch;
			int props = B[(Y[(X[offset >> 5] << 4) | ((offset>>1) & 0xF)] << 1) | (offset & 0x1)];
			return props;
		}

		internal override int GetType(int ch)
		{
			int props = GetProperties(ch);
			return (props & 0x1F);
		}

		internal override bool IsOtherLowercase(int ch)
		{
			int props = GetPropertiesEx(ch);
			return (props & 0x0001) != 0;
		}

		internal override bool IsOtherUppercase(int ch)
		{
			int props = GetPropertiesEx(ch);
			return (props & 0x0002) != 0;
		}

		internal override bool IsOtherAlphabetic(int ch)
		{
			int props = GetPropertiesEx(ch);
			return (props & 0x0004) != 0;
		}

		internal override bool IsIdeographic(int ch)
		{
			int props = GetPropertiesEx(ch);
			return (props & 0x0010) != 0;
		}

		internal override bool IsJavaIdentifierStart(int ch)
		{
			int props = GetProperties(ch);
			return ((props & 0x00007000) >= 0x00005000);
		}

		internal override bool IsJavaIdentifierPart(int ch)
		{
			int props = GetProperties(ch);
			return ((props & 0x00003000) != 0);
		}

		internal override bool IsUnicodeIdentifierStart(int ch)
		{
			int props = GetProperties(ch);
			return ((props & 0x00007000) == 0x00007000);
		}

		internal override bool IsUnicodeIdentifierPart(int ch)
		{
			int props = GetProperties(ch);
			return ((props & 0x00001000) != 0);
		}

		internal override bool IsIdentifierIgnorable(int ch)
		{
			int props = GetProperties(ch);
			return ((props & 0x00007000) == 0x00001000);
		}

		internal override int ToLowerCase(int ch)
		{
			int mapChar = ch;
			int val = GetProperties(ch);

			if ((val & 0x00020000) != 0)
			{
				int offset = val << 5 >> (5 + 18);
				mapChar = ch + offset;
			}
			return mapChar;
		}

		internal override int ToUpperCase(int ch)
		{
			int mapChar = ch;
			int val = GetProperties(ch);

			if ((val & 0x00010000) != 0)
			{
				int offset = val << 5 >> (5 + 18);
				mapChar = ch - offset;
			}
			return mapChar;
		}

		internal override int ToTitleCase(int ch)
		{
			int mapChar = ch;
			int val = GetProperties(ch);

			if ((val & 0x00008000) != 0)
			{
				// There is a titlecase equivalent.  Perform further checks:
				if ((val & 0x00010000) == 0)
				{
					// The character does not have an uppercase equivalent, so it must
					// already be uppercase; so add 1 to get the titlecase form.
					mapChar = ch + 1;
				}
				else if ((val & 0x00020000) == 0)
				{
					// The character does not have a lowercase equivalent, so it must
					// already be lowercase; so subtract 1 to get the titlecase form.
					mapChar = ch - 1;
				}
				// else {
				// The character has both an uppercase equivalent and a lowercase
				// equivalent, so it must itself be a titlecase form; return it.
				// return ch;
				//}
			}
			else if ((val & 0x00010000) != 0)
			{
				// This character has no titlecase equivalent but it does have an
				// uppercase equivalent, so use that (subtract the signed case offset).
				mapChar = ToUpperCase(ch);
			}
			return mapChar;
		}

		internal override int Digit(int ch, int radix)
		{
			int value = -1;
			if (radix >= Character.MIN_RADIX && radix <= Character.MAX_RADIX)
			{
				int val = GetProperties(ch);
				int kind = val & 0x1F;
				if (kind == Character.DECIMAL_DIGIT_NUMBER)
				{
					value = ch + ((val & 0x3E0) >> 5) & 0x1F;
				}
				else if ((val & 0xC00) == 0x00000C00)
				{
					// Java supradecimal digit
					value = (ch + ((val & 0x3E0) >> 5) & 0x1F) + 10;
				}
			}
			return (value < radix) ? value : -1;
		}

		internal override int GetNumericValue(int ch)
		{
			int val = GetProperties(ch);
			int retval = -1;

			switch (val & 0xC00)
			{
			default: // cannot occur
				goto case (0x00000000);
			case (0x00000000): // not numeric
				retval = -1;
				break;
			case (0x00000400): // simple numeric
				retval = ch + ((val & 0x3E0) >> 5) & 0x1F;
				break;
			case (0x00000800) : // "strange" numeric
				switch (ch)
				{
				case 0x10113: // AEGEAN NUMBER FORTY
					retval = 40;
					break;
				case 0x10114: // AEGEAN NUMBER FIFTY
					retval = 50;
					break;
				case 0x10115: // AEGEAN NUMBER SIXTY
					retval = 60;
					break;
				case 0x10116: // AEGEAN NUMBER SEVENTY
					retval = 70;
					break;
				case 0x10117: // AEGEAN NUMBER EIGHTY
					retval = 80;
					break;
				case 0x10118: // AEGEAN NUMBER NINETY
					retval = 90;
					break;
				case 0x10119: // AEGEAN NUMBER ONE HUNDRED
					retval = 100;
					break;
				case 0x1011A: // AEGEAN NUMBER TWO HUNDRED
					retval = 200;
					break;
				case 0x1011B: // AEGEAN NUMBER THREE HUNDRED
					retval = 300;
					break;
				case 0x1011C: // AEGEAN NUMBER FOUR HUNDRED
					retval = 400;
					break;
				case 0x1011D: // AEGEAN NUMBER FIVE HUNDRED
					retval = 500;
					break;
				case 0x1011E: // AEGEAN NUMBER SIX HUNDRED
					retval = 600;
					break;
				case 0x1011F: // AEGEAN NUMBER SEVEN HUNDRED
					retval = 700;
					break;
				case 0x10120: // AEGEAN NUMBER EIGHT HUNDRED
					retval = 800;
					break;
				case 0x10121: // AEGEAN NUMBER NINE HUNDRED
					retval = 900;
					break;
				case 0x10122: // AEGEAN NUMBER ONE THOUSAND
					retval = 1000;
					break;
				case 0x10123: // AEGEAN NUMBER TWO THOUSAND
					retval = 2000;
					break;
				case 0x10124: // AEGEAN NUMBER THREE THOUSAND
					retval = 3000;
					break;
				case 0x10125: // AEGEAN NUMBER FOUR THOUSAND
					retval = 4000;
					break;
				case 0x10126: // AEGEAN NUMBER FIVE THOUSAND
					retval = 5000;
					break;
				case 0x10127: // AEGEAN NUMBER SIX THOUSAND
					retval = 6000;
					break;
				case 0x10128: // AEGEAN NUMBER SEVEN THOUSAND
					retval = 7000;
					break;
				case 0x10129: // AEGEAN NUMBER EIGHT THOUSAND
					retval = 8000;
					break;
				case 0x1012A: // AEGEAN NUMBER NINE THOUSAND
					retval = 9000;
					break;
				case 0x1012B: // AEGEAN NUMBER TEN THOUSAND
					retval = 10000;
					break;
				case 0x1012C: // AEGEAN NUMBER TWENTY THOUSAND
					retval = 20000;
					break;
				case 0x1012D: // AEGEAN NUMBER THIRTY THOUSAND
					retval = 30000;
					break;
				case 0x1012E: // AEGEAN NUMBER FORTY THOUSAND
					retval = 40000;
					break;
				case 0x1012F: // AEGEAN NUMBER FIFTY THOUSAND
					retval = 50000;
					break;
				case 0x10130: // AEGEAN NUMBER SIXTY THOUSAND
					retval = 60000;
					break;
				case 0x10131: // AEGEAN NUMBER SEVENTY THOUSAND
					retval = 70000;
					break;
				case 0x10132: // AEGEAN NUMBER EIGHTY THOUSAND
					retval = 80000;
					break;
				case 0x10133: // AEGEAN NUMBER NINETY THOUSAND
					retval = 90000;
					break;
				case 0x10323: // OLD ITALIC NUMERAL FIFTY
					retval = 50;
					break;

				case 0x010144: // ACROPHONIC ATTIC FIFTY
					retval = 50;
					break;
				case 0x010145: // ACROPHONIC ATTIC FIVE HUNDRED
					retval = 500;
					break;
				case 0x010146: // ACROPHONIC ATTIC FIVE THOUSAND
					retval = 5000;
					break;
				case 0x010147: // ACROPHONIC ATTIC FIFTY THOUSAND
					retval = 50000;
					break;
				case 0x01014A: // ACROPHONIC ATTIC FIFTY TALENTS
					retval = 50;
					break;
				case 0x01014B: // ACROPHONIC ATTIC ONE HUNDRED TALENTS
					retval = 100;
					break;
				case 0x01014C: // ACROPHONIC ATTIC FIVE HUNDRED TALENTS
					retval = 500;
					break;
				case 0x01014D: // ACROPHONIC ATTIC ONE THOUSAND TALENTS
					retval = 1000;
					break;
				case 0x01014E: // ACROPHONIC ATTIC FIVE THOUSAND TALENTS
					retval = 5000;
					break;
				case 0x010151: // ACROPHONIC ATTIC FIFTY STATERS
					retval = 50;
					break;
				case 0x010152: // ACROPHONIC ATTIC ONE HUNDRED STATERS
					retval = 100;
					break;
				case 0x010153: // ACROPHONIC ATTIC FIVE HUNDRED STATERS
					retval = 500;
					break;
				case 0x010154: // ACROPHONIC ATTIC ONE THOUSAND STATERS
					retval = 1000;
					break;
				case 0x010155: // ACROPHONIC ATTIC TEN THOUSAND STATERS
					retval = 10000;
					break;
				case 0x010156: // ACROPHONIC ATTIC FIFTY THOUSAND STATERS
					retval = 50000;
					break;
				case 0x010166: // ACROPHONIC TROEZENIAN FIFTY
					retval = 50;
					break;
				case 0x010167: // ACROPHONIC TROEZENIAN FIFTY ALTERNATE FORM
					retval = 50;
					break;
				case 0x010168: // ACROPHONIC HERMIONIAN FIFTY
					retval = 50;
					break;
				case 0x010169: // ACROPHONIC THESPIAN FIFTY
					retval = 50;
					break;
				case 0x01016A: // ACROPHONIC THESPIAN ONE HUNDRED
					retval = 100;
					break;
				case 0x01016B: // ACROPHONIC THESPIAN THREE HUNDRED
					retval = 300;
					break;
				case 0x01016C: // ACROPHONIC EPIDAUREAN FIVE HUNDRED
					retval = 500;
					break;
				case 0x01016D: // ACROPHONIC TROEZENIAN FIVE HUNDRED
					retval = 500;
					break;
				case 0x01016E: // ACROPHONIC THESPIAN FIVE HUNDRED
					retval = 500;
					break;
				case 0x01016F: // ACROPHONIC CARYSTIAN FIVE HUNDRED
					retval = 500;
					break;
				case 0x010170: // ACROPHONIC NAXIAN FIVE HUNDRED
					retval = 500;
					break;
				case 0x010171: // ACROPHONIC THESPIAN ONE THOUSAND
					retval = 1000;
					break;
				case 0x010172: // ACROPHONIC THESPIAN FIVE THOUSAND
					retval = 5000;
					break;
				case 0x010174: // ACROPHONIC STRATIAN FIFTY MNAS
					retval = 50;
					break;
				case 0x010341: // GOTHIC LETTER NINETY
					retval = 90;
					break;
				case 0x01034A: // GOTHIC LETTER NINE HUNDRED
					retval = 900;
					break;
				case 0x0103D5: // OLD PERSIAN NUMBER HUNDRED
					retval = 100;
					break;
				case 0x01085D: // IMPERIAL ARAMAIC NUMBER ONE HUNDRED
					retval = 100;
					break;
				case 0x01085E: // IMPERIAL ARAMAIC NUMBER ONE THOUSAND
					retval = 1000;
					break;
				case 0x01085F: // IMPERIAL ARAMAIC NUMBER TEN THOUSAND
					retval = 10000;
					break;
				case 0x010919: // PHOENICIAN NUMBER ONE HUNDRED
					retval = 100;
					break;
				case 0x010A46: // KHAROSHTHI NUMBER ONE HUNDRED
					retval = 100;
					break;
				case 0x010A47: // KHAROSHTHI NUMBER ONE THOUSAND
					retval = 1000;
					break;
				case 0x010A7E: // OLD SOUTH ARABIAN NUMBER FIFTY
					retval = 50;
					break;
				case 0x010B5E: // INSCRIPTIONAL PARTHIAN NUMBER ONE HUNDRED
					retval = 100;
					break;
				case 0x010B5F: // INSCRIPTIONAL PARTHIAN NUMBER ONE THOUSAND
					retval = 1000;
					break;
				case 0x010B7E: // INSCRIPTIONAL PAHLAVI NUMBER ONE HUNDRED
					retval = 100;
					break;
				case 0x010B7F: // INSCRIPTIONAL PAHLAVI NUMBER ONE THOUSAND
					retval = 1000;
					break;
				case 0x010E6C: // RUMI NUMBER FORTY
					retval = 40;
					break;
				case 0x010E6D: // RUMI NUMBER FIFTY
					retval = 50;
					break;
				case 0x010E6E: // RUMI NUMBER SIXTY
					retval = 60;
					break;
				case 0x010E6F: // RUMI NUMBER SEVENTY
					retval = 70;
					break;
				case 0x010E70: // RUMI NUMBER EIGHTY
					retval = 80;
					break;
				case 0x010E71: // RUMI NUMBER NINETY
					retval = 90;
					break;
				case 0x010E72: // RUMI NUMBER ONE HUNDRED
					retval = 100;
					break;
				case 0x010E73: // RUMI NUMBER TWO HUNDRED
					retval = 200;
					break;
				case 0x010E74: // RUMI NUMBER THREE HUNDRED
					retval = 300;
					break;
				case 0x010E75: // RUMI NUMBER FOUR HUNDRED
					retval = 400;
					break;
				case 0x010E76: // RUMI NUMBER FIVE HUNDRED
					retval = 500;
					break;
				case 0x010E77: // RUMI NUMBER SIX HUNDRED
					retval = 600;
					break;
				case 0x010E78: // RUMI NUMBER SEVEN HUNDRED
					retval = 700;
					break;
				case 0x010E79: // RUMI NUMBER EIGHT HUNDRED
					retval = 800;
					break;
				case 0x010E7A: // RUMI NUMBER NINE HUNDRED
					retval = 900;
					break;
				case 0x01105E: // BRAHMI NUMBER FORTY
					retval = 40;
					break;
				case 0x01105F: // BRAHMI NUMBER FIFTY
					retval = 50;
					break;
				case 0x011060: // BRAHMI NUMBER SIXTY
					retval = 60;
					break;
				case 0x011061: // BRAHMI NUMBER SEVENTY
					retval = 70;
					break;
				case 0x011062: // BRAHMI NUMBER EIGHTY
					retval = 80;
					break;
				case 0x011063: // BRAHMI NUMBER NINETY
					retval = 90;
					break;
				case 0x011064: // BRAHMI NUMBER ONE HUNDRED
					retval = 100;
					break;
				case 0x011065: // BRAHMI NUMBER ONE THOUSAND
					retval = 1000;
					break;
				case 0x012432: // CUNEIFORM NUMERIC SIGN SHAR2 TIMES GAL PLUS DISH
					retval = 216000;
					break;
				case 0x012433: // CUNEIFORM NUMERIC SIGN SHAR2 TIMES GAL PLUS MIN
					retval = 432000;
					break;
				case 0x01D36C: // COUNTING ROD TENS DIGIT FOUR
					retval = 40;
					break;
				case 0x01D36D: // COUNTING ROD TENS DIGIT FIVE
					retval = 50;
					break;
				case 0x01D36E: // COUNTING ROD TENS DIGIT SIX
					retval = 60;
					break;
				case 0x01D36F: // COUNTING ROD TENS DIGIT SEVEN
					retval = 70;
					break;
				case 0x01D370: // COUNTING ROD TENS DIGIT EIGHT
					retval = 80;
					break;
				case 0x01D371: // COUNTING ROD TENS DIGIT NINE
					retval = 90;
					break;
				default:
					retval = -2;
					break;
				}

				break;
			case (0x00000C00): // Java supradecimal
				retval = (ch + ((val & 0x3E0) >> 5) & 0x1F) + 10;
				break;
			}
			return retval;
		}

		internal override bool IsWhitespace(int ch)
		{
			int props = GetProperties(ch);
			return ((props & 0x00007000) == 0x00004000);
		}

		internal override sbyte GetDirectionality(int ch)
		{
			int val = GetProperties(ch);
			sbyte directionality = (sbyte)((val & 0x78000000) >> 27);
			if (directionality == 0xF)
			{
				directionality = Character.DIRECTIONALITY_UNDEFINED;
			}
			return directionality;
		}

		internal override bool IsMirrored(int ch)
		{
			int props = GetProperties(ch);
			return ((props & 0x80000000) != 0);
		}

		internal static readonly CharacterData Instance = new CharacterData01();
		private CharacterData01()
		{
		};

		// The following tables and code generated using:
	  // java GenerateCharacter -plane 1 -template d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/characterdata/CharacterData01.java.template -spec d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/UnicodeData.txt -specialcasing d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/SpecialCasing.txt -proplist d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/PropList.txt -o d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/build/windows-amd64/jdk/gensrc/java/lang/CharacterData01.java -string -usecharforbyte 11 4 1
	  // The X table has 2048 entries for a total of 4096 bytes.

	  internal static readonly char[] X = ("\x0000\x0001\x0002\x0003\x0004\x0004\x0004\x0005\x0006\x0007\x0008\x0009\x000A\x0003\x000B\x000C\x0003\x0003\x0003" + "\x0003\x000D\x0004\x000E\x0003\x000F\x0010\x0011\x0003\x0012\x0004\x0013\x0003\x0014\x0015\x0016\x0004\x0017\x0018" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0019\x001A\x001B\x0003\x0003\x0003\x0003\x0003\x001C\x001D\x0003\x0003" + "\x001E\x001F\x0003\x0003\x0020\x0021\x0022\x0023\x0003\x0003\x0003\x0003\x001E\x0024\x0025\x0026\x0003\x0003\x0003" + "\x0003\x001E\x001E\x0027\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0028\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0029\x002A\x002B\x002C\x002D" + "\x002E\x002F\x0030\x0031\x0032\x0033\x0003\x0034\x0035\x0036\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0004\x0037\x0018\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004" + "\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0038\x0003" + "\x0003\x0003\x0003\x0039\x003A\x003B\x003C\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004" + "\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0038" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0004\x0004\x0004\x0004" + "\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x003D\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0004\x0004\x003E\x003F\x0040" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0041\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0042\x0042\x0042\x0042\x0042\x0042\x0042\x0043" + "\x0042\x0044\x0042\x0045\x0046\x0047\x0048\x0003\x0049\x0049\x004A\x0003\x0003\x0003\x0003\x0003\x0049\x0049\x004B" + "\x004C\x0003\x0003\x0003\x0003\x004D\x004E\x004F\x0050\x0051\x0052\x0053\x0054\x0055\x0056\x0057\x0058\x0059\x004D" + "\x004E\x005A\x0050\x005B\x005C\x005D\x0054\x005E\x005F\x0060\x0061\x0062\x0063\x0064\x0065\x0066\x0067\x0068\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0069\x006A\x006B\x006C\x006D\x006E\x0003\x006F\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0049\x0070\x0049\x0049\x0071\x0072\x0073\x0003\x0074\x0075\x0042\x0076\x0077\x0003\x0003\x0078\x0079\x0077" + "\x007A\x0003\x0003\x0003\x0003\x0003\x0049\x007B\x0049\x007C\x0071\x0049\x007D\x007E\x0049\x007F\x0080\x0049\x0049" + "\x0049\x0049\x0081\x0049\x0082\x0083\x0084\x0003\x0003\x0003\x0085\x0049\x0049\x0086\x0003\x0049\x0049\x0087\x0003" + "\x0049\x0049\x0049\x0071\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003").ToCharArray();

	  // The Y table has 2176 entries for a total of 4352 bytes.

	  internal static readonly char[] Y = ("\x0000\x0000\x0000\x0000\x0000\x0000\x0001\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0002\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0002\x0000\x0001\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0003\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0002\x0003" + "\x0003\x0004\x0005\x0003\x0006\x0007\x0007\x0007\x0007\x0008\x0009\x000A\x000A\x000A\x000A\x000A\x000A\x000A\x000A" + "\x000A\x000A\x000A\x000A\x000A\x000A\x000A\x000A\x0003\x000B\x000C\x000C\x000C\x000C\x000D\x000E\x000D\x000D\x000F" + "\x000D\x000D\x0010\x0011\x000D\x000D\x0012\x0013\x0014\x0015\x0016\x0017\x0018\x0019\x000D\x000D\x000D\x000D\x000D" + "\x000D\x001A\x001B\x001C\x001D\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001F\x0003\x0003\x001E\x001E\x001E" + "\x001E\x001E\x001E\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x000C\x000C\x000C\x000C\x000C\x000C" + "\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x0020\x0003\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0002\x0003\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0002\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0002\x0021\x0022\x0003\x0003\x0003\x0003\x0003\x0003\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0023\x0000\x0000\x0000\x0000\x0024\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0025" + "\x0000\x0000\x0003\x0003\x0000\x0000\x0000\x0000\x0026\x0027\x0028\x0003\x0003\x0003\x0003\x0003\x0029\x0029\x0029" + "\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x0029\x002A\x002A" + "\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x002A\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0003\x002B\x002B\x002B\x002B\x002B\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x002C\x002C\x002C\x0003\x002D\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C" + "\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002E\x002D\x0003\x002D\x002E\x002C\x002C\x002C\x002C\x002C" + "\x002C\x002C\x002C\x002C\x002C\x002C\x002F\x0030\x0031\x0032\x0033\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C" + "\x002C\x002C\x002C\x0034\x0035\x0036\x0003\x0037\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C" + "\x002C\x002C\x0003\x0003\x002F\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C" + "\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x0003\x0003\x0003\x002C\x0038" + "\x0039\x003A\x003B\x0003\x0003\x0039\x0039\x002C\x002C\x002E\x002C\x002E\x002C\x002C\x002C\x002C\x002C\x002C\x002C" + "\x002C\x002C\x002C\x002C\x002C\x002C\x0003\x0003\x003C\x003D\x0003\x003E\x003F\x003F\x0040\x0033\x0003\x0003\x0003" + "\x0003\x0041\x0041\x0041\x0041\x0042\x0003\x0003\x0003\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C" + "\x002C\x002C\x002C\x002C\x0043\x0044\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x0003\x0037" + "\x0045\x0045\x0045\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x0003\x0030\x0030\x0046\x0033" + "\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002C\x002D\x0003\x0003\x0030\x0030\x0046\x0033\x002C\x002C\x002C" + "\x002C\x002D\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0047\x0047\x0047\x0047\x0047\x0048" + "\x0049\x0049\x0049\x0049\x0049\x0049\x0049\x0049\x0049\x004A\x004B\x004C\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0039\x0039\x0039\x0039\x0039\x0039\x0039\x004D\x004E\x004E\x004E\x0003\x0003\x004F\x004F\x004F\x004F\x004F\x0050" + "\x001C\x001C\x001C\x001C\x0051\x0051\x0051\x0051\x0051\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x003C\x004C" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0052\x004B\x0039\x0053\x0054\x004D\x0055\x004E\x004E\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0002\x0003\x0003\x0003\x0056\x0056\x0056" + "\x0056\x0056\x0003\x0003\x0003\x0039\x0057\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0058\x0039\x0039\x004B\x0039\x0039\x0059\x003D\x005A\x005A\x005A\x005A\x005A\x004E" + "\x004E\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0039\x004C\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x005B\x0052\x0039\x0039\x0039\x0039\x0053\x005C\x0000\x005D\x004E\x0005\x0003\x0003\x0003\x0056\x0056" + "\x0056\x0056\x0056\x0003\x0003\x0003\x0000\x0000\x0000\x0000\x0000\x0058\x004B\x0052\x0039\x0039\x0039\x005E\x0003" + "\x0003\x0003\x0003\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0002\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x005F\x005F\x005F\x005F\x0060\x0060\x0060\x0061\x0062\x0062\x0063\x0064\x0064\x0064\x0064\x0065\x0065\x0066\x0067" + "\x0068\x0068\x0068\x0062\x0069\x006A\x006B\x006C\x006D\x0064\x006E\x006F\x0070\x0071\x0072\x0073\x0074\x0075\x0076" + "\x0076\x0077\x0078\x0079\x007A\x006B\x007B\x006B\x006B\x006B\x006B\x0024\x0003\x0003\x0003\x0003\x0003\x0003\x004E" + "\x004E\x0003\x0003\x0003\x0003\x0003\x0003\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0002\x0003\x0003\x0003\x0000\x0000\x0002\x0003\x0003\x0003\x0003\x0003\x005B\x0052\x0052\x0052\x0052\x0052\x0052" + "\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x0052\x007C\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x003E\x003C\x007D\x007E\x007E\x007E\x007E\x007E\x007E\x0000\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C" + "\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C" + "\x0003\x0003\x0003\x0003\x0003\x000C\x000C\x000C\x007F\x000B\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C" + "\x000C\x000C\x000C\x000C\x0080\x005E\x003C\x000C\x0080\x0081\x0081\x0082\x0083\x0083\x0083\x0084\x003C\x003C\x003C" + "\x0085\x0020\x003C\x003C\x003C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C" + "\x000C\x003C\x003C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C" + "\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x0003\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E" + "\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x003C\x0086\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x0087\x0003\x0003\x0003\x0003" + "\x0088\x0088\x0088\x0088\x0088\x0089\x000A\x000A\x000A\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x008A\x008A\x008A" + "\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B" + "\x008B\x008B\x008B\x008B\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008B\x008B" + "\x008B\x008C\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A" + "\x008A\x008A\x008A\x008A\x008A\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008D" + "\x008A\x0003\x008D\x008E\x008D\x008E\x008A\x008D\x008A\x008A\x008A\x008A\x008B\x008B\x008F\x008F\x008B\x008B\x008B" + "\x008F\x008B\x008B\x008B\x008B\x008B\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A" + "\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008A\x008E\x008A\x008D\x008E\x008A" + "\x008A\x008A\x008D\x008A\x008A\x008A\x008D\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B" + "\x008B\x008A\x008E\x008A\x008D\x008A\x008A\x008D\x008D\x0003\x008A\x008A\x008A\x008D\x008B\x008B\x008B\x008B\x008B" + "\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A" + "\x008A\x008A\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008A\x008A\x008A\x008A" + "\x008A\x008A\x008A\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008A\x008B\x008B\x008B\x008B\x008B\x008B" + "\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A" + "\x008A\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008A\x008A\x008A\x008A\x008A" + "\x008A\x008A\x008A\x008B\x008B\x008B\x0003\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A" + "\x0090\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x0091\x008B\x008B\x008B\x008A\x008A" + "\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x0090\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B" + "\x008B\x008B\x008B\x008B\x0091\x008B\x008B\x008B\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A" + "\x008A\x0090\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x0091\x008B\x008B\x008B\x008A" + "\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x0090\x008B\x008B\x008B\x008B\x008B\x008B\x008B" + "\x008B\x008B\x008B\x008B\x008B\x0091\x008B\x008B\x008B\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A\x008A" + "\x008A\x008A\x0090\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x008B\x0091\x008B\x008B\x008B" + "\x0092\x0003\x0093\x0093\x0093\x0093\x0093\x0094\x0094\x0094\x0094\x0094\x0095\x0095\x0095\x0095\x0095\x0096\x0096" + "\x0096\x0096\x0096\x0097\x0097\x0097\x0097\x0097\x0098\x0098\x0099\x0098\x0098\x0098\x0098\x0098\x0098\x0098\x0098" + "\x0098\x0098\x0098\x0098\x0098\x0099\x009A\x009A\x0099\x0099\x0098\x0098\x0098\x0098\x009A\x0098\x0098\x0099\x0099" + "\x0003\x0003\x0003\x009A\x0003\x0099\x0099\x0099\x0099\x0098\x0099\x009A\x009A\x0099\x0099\x0099\x0099\x0099\x0099" + "\x009A\x009A\x0099\x0098\x009A\x0098\x0098\x0098\x009A\x0098\x0098\x0099\x0098\x009A\x009A\x0098\x0098\x0098\x0098" + "\x0098\x0099\x0098\x0098\x0098\x0098\x0098\x0098\x0098\x0098\x0003\x0003\x0099\x0098\x0099\x0098\x0098\x0099\x0098" + "\x0098\x0098\x0098\x0098\x0098\x0098\x0098\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x009B\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x001E\x001E\x001E\x001E\x001E\x001E\x0003\x0003\x001E\x001E\x001E\x001E\x001E" + "\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x0003\x0003\x0003\x0003\x0003\x0003" + "\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x0087\x009C\x001E\x001E\x001E\x001E\x001E\x001E\x0087\x009C\x001E\x001E" + "\x001E\x001E\x001E\x001E\x001E\x009C\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x009D\x009E\x009E\x009E\x009E\x009F" + "\x0003\x0003\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x007F\x000C" + "\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x001E\x0003\x0003\x000C\x000C\x000C\x000C" + "\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x007F\x0003" + "\x0003\x0003\x0003\x0003\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x007F" + "\x0003\x0003\x0003\x0003\x0003\x0003\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x000C\x007F" + "\x0003\x0003\x0003\x000C\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0087\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x001E\x001E\x001E\x009C\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E" + "\x001E\x001E\x001E\x0087\x0003\x001E\x001E\x0087\x001E\x001E\x0087\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x0087\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x001E" + "\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x0087\x0087\x001E\x001E\x001E" + "\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E" + "\x001E\x001E\x001E\x001E\x001E\x009C\x001E\x0087\x0003\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x001E" + "\x001E\x001E\x001E\x001E\x001E\x0003\x001E\x001E\x0003\x0003\x0003\x0003\x0003\x0003\x001E\x001E\x001E\x001E\x001E" + "\x001E\x001E\x001E\x001E\x001E\x001E\x001E\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x009C\x001E\x001E\x0087\x0003\x009C" + "\x001E\x001E\x001E\x001E\x001E\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x001E\x001E\x001E\x0003\x0003\x0003" + "\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003\x0003").ToCharArray();

	  // The A table has 320 entries for a total of 1280 bytes.

	  internal static readonly int[] A = new int[320];
	  internal const String A_DATA = "\x0000\u7005\x0000\u7005\u7800\x0000\x0000\u7005\x0000\u7005\u7800\x0000\u7800\x0000\u7800" + "\x0000\x0000\x0018\u6800\x0018\x0000\x0018\u7800\x0000\u7800\x0000\x0000\u074B\x0000\u074B\x0000" + "\u074B\x0000\u074B\x0000\u046B\x0000\u058B\x0000\u080B\x0000\u080B\x0000\u080B\u7800\x0000" + "\x0000\x001C\x0000\x001C\x0000\x001C\u6800\u780A\u6800\u780A\u6800\u77EA\u6800\u744A\u6800" + "\u77AA\u6800\u742A\u6800\u780A\u6800\u76CA\u6800\u774A\u6800\u780A\u6800\u780A" + "\u6800\u766A\u6800\u752A\u6800\u750A\u6800\u74EA\u6800\u74EA\u6800\u74CA\u6800" + "\u74AA\u6800\u748A\u6800\u74CA\u6800\u754A\u6800\u752A\u6800\u750A\u6800\u74EA" + "\u6800\u74CA\u6800\u772A\u6800\u780A\u6800\u764A\u6800\u780A\u6800\u080B\u6800" + "\u080B\u6800\u080B\u6800\u080B\u6800\x001C\u6800\x001C\u6800\x001C\u6800\u06CB\u7800" + "\x0000\x0000\x001C\u4000\u3006\x0000\u042B\x0000\u048B\x0000\u050B\x0000\u080B\x0000\u7005" + "\x0000\u780A\x0000\u780A\u7800\x0000\u7800\x0000\x0000\x0018\x0000\x0018\x0000\u760A\x0000\u760A" + "\x0000\u76EA\x0000\u740A\x0000\u780A\x00A2\u7001\x00A2\u7001\x00A1\u7002\x00A1\u7002\x0000" + "\u3409\x0000\u3409\u0800\u7005\u0800\u7005\u0800\u7005\u7800\x0000\u7800\x0000\u0800" + "\u7005\u7800\x0000\u0800\x0018\u0800\u052B\u0800\u052B\u0800\u052B\u0800\u05EB" + "\u0800\u070B\u0800\u080B\u0800\u080B\u0800\u080B\u0800\u056B\u0800\u066B\u0800" + "\u078B\u0800\u080B\u0800\u050B\u0800\u050B\u7800\x0000\u6800\x0018\u0800\u7005" + "\u4000\u3006\u4000\u3006\u4000\u3006\u7800\x0000\u4000\u3006\u4000\u3006\u7800" + "\x0000\u4000\u3006\u4000\u3006\u4000\u3006\u7800\x0000\u7800\x0000\u4000\u3006\u0800" + "\u042B\u0800\u042B\u0800\u04CB\u0800\u05EB\u0800\x0018\u0800\x0018\u0800\x0018\u7800" + "\x0000\u0800\u7005\u0800\u048B\u0800\u080B\u0800\x0018\u6800\x0018\u6800\x0018\u0800" + "\u05CB\u0800\u06EB\u3000\u042B\u3000\u042B\u3000\u054B\u3000\u066B\u3000\u080B" + "\u3000\u080B\u3000\u080B\u7800\x0000\x0000\u3008\u4000\u3006\x0000\u3008\x0000\u7005" + "\u4000\u3006\x0000\x0018\x0000\x0018\x0000\x0018\u6800\u05EB\u6800\u05EB\u6800\u070B\u6800" + "\u042B\x0000\u3749\x0000\u3749\x0000\u3008\x0000\u3008\u4000\u3006\x0000\u3008\x0000\u3008" + "\u4000\u3006\x0000\x0018\x0000\u1010\x0000\u3609\x0000\u3609\u4000\u3006\x0000\u7005\x0000" + "\u7005\u4000\u3006\u4000\u3006\u4000\u3006\x0000\u3549\x0000\u3549\x0000\u7005\x0000" + "\u3008\x0000\u3008\x0000\u7005\x0000\u7005\x0000\x0018\x0000\u3008\u4000\u3006\x0000\u744A" + "\x0000\u744A\x0000\u776A\x0000\u776A\x0000\u776A\x0000\u76AA\x0000\u76AA\x0000\u76AA\x0000" + "\u76AA\x0000\u758A\x0000\u758A\x0000\u758A\x0000\u746A\x0000\u746A\x0000\u746A\x0000\u77EA" + "\x0000\u77EA\x0000\u77CA\x0000\u77CA\x0000\u77CA\x0000\u76AA\x0000\u768A\x0000\u768A\x0000" + "\u768A\x0000\u780A\x0000\u780A\x0000\u75AA\x0000\u75AA\x0000\u75AA\x0000\u758A\x0000\u752A" + "\x0000\u750A\x0000\u750A\x0000\u74EA\x0000\u74CA\x0000\u74AA\x0000\u74CA\x0000\u74CA\x0000" + "\u74AA\x0000\u748A\x0000\u748A\x0000\u746A\x0000\u746A\x0000\u744A\x0000\u742A\x0000\u740A" + "\x0000\u770A\x0000\u770A\x0000\u770A\x0000\u764A\x0000\u764A\x0000\u764A\x0000\u764A\x0000" + "\u762A\x0000\u762A\x0000\u760A\x0000\u752A\x0000\u752A\x0000\u3008\u7800\x0000\u4000\u3006" + "\x0000\u7004\x0000\u7004\x0000\u7004\x0000\x001C\u7800\x0000\x0000\x001C\x0000\u3008\x0000\u3008" + "\x0000\u3008\x0000\u3008\u4800\u1010\u4800\u1010\u4800\u1010\u4800\u1010\u4000" + "\u3006\u4000\u3006\x0000\x001C\u4000\u3006\u6800\x001C\u6800\x001C\u7800\x0000\x0000\u042B" + "\x0000\u042B\x0000\u054B\x0000\u066B\x0000\u7001\x0000\u7001\x0000\u7002\x0000\u7002\x0000" + "\u7002\u7800\x0000\x0000\u7001\u7800\x0000\u7800\x0000\x0000\u7001\u7800\x0000\x0000\u7002" + "\x0000\u7001\x0000\x0019\x0000\u7002\uE800\x0019\x0000\u7001\x0000\u7002\u1800\u3649\u1800" + "\u3649\u1800\u3509\u1800\u3509\u1800\u37C9\u1800\u37C9\u1800\u3689\u1800\u3689" + "\u1800\u3549\u1800\u3549\u1000\u7005\u1000\u7005\u7800\x0000\u1000\u7005\u1000" + "\u7005\u7800\x0000\u6800\x0019\u6800\x0019\u7800\x0000\u6800\x001C\u1800\u040B\u1800" + "\u07EB\u1800\u07EB\u1800\u07EB\u1800\u07EB\u7800\x0000";

	  // The B table has 320 entries for a total of 640 bytes.

	  internal static readonly char[] B = ("\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0004" + "\x0004\x0004\x0000\x0004\x0004\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0004\x0004" + "\x0004\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0004\x0004\x0004\x0004\x0004\x0000\x0000" + "\x0000\x0000\x0000\x0004\x0000\x0000\x0004\x0004\x0000\x0000\x0000\x0000\x0004\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0004\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000").ToCharArray();

	  // In all, the character property tables require 9728 bytes.

		static CharacterData01()
		{
		{ // THIS CODE WAS AUTOMATICALLY CREATED BY GenerateCharacter:
				char[] data = A_DATA.ToCharArray();
				assert(data.Length == (320 * 2));
				int i = 0, j = 0;
				while (i < (320 * 2))
				{
					int entry = data[i++] << 16;
					A[j++] = entry | data[i++];
				}
			}

		}
	}

}