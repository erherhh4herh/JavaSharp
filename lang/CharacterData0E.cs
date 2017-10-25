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
	/// The CharacterData class encapsulates the large tables found in
	///    Java.lang.Character. 
	/// </summary>

	internal class CharacterData0E : CharacterData
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
			int props = A[Y[X[offset >> 5] | ((offset >> 1) & 0xF)] | (offset & 0x1)];
			return props;
		}

		internal virtual int GetPropertiesEx(int ch)
		{
			char offset = (char)ch;
			int props = B[Y[X[offset >> 5] | ((offset >> 1) & 0xF)] | (offset & 0x1)];
			return props;
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

		internal override int GetType(int ch)
		{
			int props = GetProperties(ch);
			return (props & 0x1F);
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
				retval = -2;
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

		internal static readonly CharacterData Instance = new CharacterData0E();
		private CharacterData0E()
		{
		};

		// The following tables and code generated using:
	  // java GenerateCharacter -plane 14 -template d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/characterdata/CharacterData0E.java.template -spec d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/UnicodeData.txt -specialcasing d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/SpecialCasing.txt -proplist d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/PropList.txt -o d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/build/windows-amd64/jdk/gensrc/java/lang/CharacterData0E.java -string -usecharforbyte 11 4 1
	  // The X table has 2048 entries for a total of 4096 bytes.

	  internal static readonly char[] X = ("\x0000\x0010\x0010\x0010\x0020\x0020\x0020\x0020\x0030\x0030\x0030\x0030\x0030\x0030\x0030\x0040\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020" + "\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020\x0020").ToCharArray();

	  // The Y table has 80 entries for a total of 160 bytes.

	  internal static readonly char[] Y = ("\x0000\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0004\x0004\x0004" + "\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0004\x0002\x0002\x0002\x0002\x0002\x0002" + "\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0002\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006" + "\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0006\x0002\x0002\x0002\x0002" + "\x0002\x0002\x0002\x0002").ToCharArray();

	  // The A table has 8 entries for a total of 32 bytes.

	  internal static readonly int[] A = new int[8];
	  internal const String A_DATA = "\u7800\x0000\u4800\u1010\u7800\x0000\u7800\x0000\u4800\u1010\u4800\u1010\u4000\u3006" + "\u4000\u3006";

	  // The B table has 8 entries for a total of 16 bytes.

	  internal static readonly char[] B = ("\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000").ToCharArray();

	  // In all, the character property tables require 4288 bytes.

		static CharacterData0E()
		{
		{ // THIS CODE WAS AUTOMATICALLY CREATED BY GenerateCharacter:
				char[] data = A_DATA.ToCharArray();
				assert(data.Length == (8 * 2));
				int i = 0, j = 0;
				while (i < (8 * 2))
				{
					int entry = data[i++] << 16;
					A[j++] = entry | data[i++];
				}
			}

		}
	}

}