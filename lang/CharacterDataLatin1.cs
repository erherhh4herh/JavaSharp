// This file was generated AUTOMATICALLY from a template file Fri Jan 29 17:43:04 PST 2016
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
	/// The CharacterData class encapsulates the large tables found in
	///    Java.lang.Character. 
	/// </summary>

	internal class CharacterDataLatin1 : CharacterData
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
			int props = A[offset];
			return props;
		}

		internal virtual int GetPropertiesEx(int ch)
		{
			char offset = (char)ch;
			int props = B[offset];
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

			if (((val & 0x00020000) != 0) && ((val & 0x07FC0000) != 0x07FC0000))
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
				if ((val & 0x07FC0000) != 0x07FC0000)
				{
					int offset = val << 5 >> (5 + 18);
					mapChar = ch - offset;
				}
				else if (ch == 0x00B5)
				{
					mapChar = 0x039C;
				}
			}
			return mapChar;
		}

		internal override int ToTitleCase(int ch)
		{
			return ToUpperCase(ch);
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
				directionality = -1;
			}
			return directionality;
		}

		internal override bool IsMirrored(int ch)
		{
			int props = GetProperties(ch);
			return ((props & 0x80000000) != 0);
		}

		internal override int ToUpperCaseEx(int ch)
		{
			int mapChar = ch;
			int val = GetProperties(ch);

			if ((val & 0x00010000) != 0)
			{
				if ((val & 0x07FC0000) != 0x07FC0000)
				{
					int offset = val << 5 >> (5 + 18);
					mapChar = ch - offset;
				}
				else
				{
					switch (ch)
					{
						// map overflow characters
						case 0x00B5 :
							mapChar = 0x039C;
							break;
						default :
							mapChar = Character.ERROR;
							break;
					}
				}
			}
			return mapChar;
		}

		internal static char[] SharpsMap = new char[] {'S', 'S'};

		internal override char[] ToUpperCaseCharArray(int ch)
		{
			char[] upperMap = new char[] {(char)ch};
			if (ch == 0x00DF)
			{
				upperMap = SharpsMap;
			}
			return upperMap;
		}

		internal static readonly CharacterDataLatin1 Instance = new CharacterDataLatin1();
		private CharacterDataLatin1()
		{
		};

		// The following tables and code generated using:
	  // java GenerateCharacter -template d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/characterdata/CharacterDataLatin1.java.template -spec d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/UnicodeData.txt -specialcasing d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/SpecialCasing.txt -proplist d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/PropList.txt -o d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/build/windows-amd64/jdk/gensrc/java/lang/CharacterDataLatin1.java -string -usecharforbyte -latin1 8
	  // The A table has 256 entries for a total of 1024 bytes.

	  internal static readonly int[] A = new int[256];
	  internal const String A_DATA = "\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800" + "\u100F\u4800\u100F\u4800\u100F\u5800\u400F\u5000\u400F\u5800\u400F\u6000\u400F" + "\u5000\u400F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800" + "\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F" + "\u4800\u100F\u4800\u100F\u5000\u400F\u5000\u400F\u5000\u400F\u5800\u400F\u6000" + "\u400C\u6800\x0018\u6800\x0018\u2800\x0018\u2800\u601A\u2800\x0018\u6800\x0018\u6800" + "\x0018\uE800\x0015\uE800\x0016\u6800\x0018\u2000\x0019\u3800\x0018\u2000\x0014\u3800\x0018" + "\u3800\x0018\u1800\u3609\u1800\u3609\u1800\u3609\u1800\u3609\u1800\u3609\u1800" + "\u3609\u1800\u3609\u1800\u3609\u1800\u3609\u1800\u3609\u3800\x0018\u6800\x0018" + "\uE800\x0019\u6800\x0019\uE800\x0019\u6800\x0018\u6800\x0018\x0082\u7FE1\x0082\u7FE1\x0082" + "\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1" + "\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082" + "\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1" + "\x0082\u7FE1\uE800\x0015\u6800\x0018\uE800\x0016\u6800\x001B\u6800\u5017\u6800\x001B\x0081" + "\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2" + "\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081" + "\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2" + "\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\uE800\x0015\u6800\x0019\uE800\x0016\u6800\x0019\u4800" + "\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u5000\u100F" + "\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800" + "\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F" + "\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800" + "\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F\u4800\u100F" + "\u3800\x000C\u6800\x0018\u2800\u601A\u2800\u601A\u2800\u601A\u2800\u601A\u6800" + "\x001C\u6800\x0018\u6800\x001B\u6800\x001C\x0000\u7005\uE800\x001D\u6800\x0019\u4800\u1010" + "\u6800\x001C\u6800\x001B\u2800\x001C\u2800\x0019\u1800\u060B\u1800\u060B\u6800\x001B" + "\u07FD\u7002\u6800\x0018\u6800\x0018\u6800\x001B\u1800\u050B\x0000\u7005\uE800\x001E" + "\u6800\u080B\u6800\u080B\u6800\u080B\u6800\x0018\x0082\u7001\x0082\u7001\x0082\u7001" + "\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082" + "\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001" + "\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\u6800\x0019\x0082\u7001\x0082" + "\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\x0082\u7001\u07FD\u7002\x0081\u7002" + "\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081" + "\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002" + "\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\u6800" + "\x0019\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002\x0081\u7002" + "\u061D\u7002";

	  // The B table has 256 entries for a total of 512 bytes.

	  internal static readonly char[] B = ("\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0001" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0001\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000").ToCharArray();

	  // In all, the character property tables require 1024 bytes.

		static CharacterDataLatin1()
		{
		{ // THIS CODE WAS AUTOMATICALLY CREATED BY GenerateCharacter:
				char[] data = A_DATA.ToCharArray();
				assert(data.Length == (256 * 2));
				int i = 0, j = 0;
				while (i < (256 * 2))
				{
					int entry = data[i++] << 16;
					A[j++] = entry | data[i++];
				}
			}

		}
	}


}