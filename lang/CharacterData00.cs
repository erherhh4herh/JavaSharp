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
	/// The CharacterData00 class encapsulates the large tables once found in
	/// java.lang.Character
	/// </summary>

	internal class CharacterData00 : CharacterData
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
			  if ((val & 0x07FC0000) == 0x07FC0000)
			  {
				switch (ch)
				{
				  // map the offset overflow chars
				case 0x0130 :
					mapChar = 0x0069;
					break;
				case 0x2126 :
					mapChar = 0x03C9;
					break;
				case 0x212A :
					mapChar = 0x006B;
					break;
				case 0x212B :
					mapChar = 0x00E5;
					break;
				  // map the titlecase chars with both a 1:M uppercase map
				  // and a lowercase map
				case 0x1F88 :
					mapChar = 0x1F80;
					break;
				case 0x1F89 :
					mapChar = 0x1F81;
					break;
				case 0x1F8A :
					mapChar = 0x1F82;
					break;
				case 0x1F8B :
					mapChar = 0x1F83;
					break;
				case 0x1F8C :
					mapChar = 0x1F84;
					break;
				case 0x1F8D :
					mapChar = 0x1F85;
					break;
				case 0x1F8E :
					mapChar = 0x1F86;
					break;
				case 0x1F8F :
					mapChar = 0x1F87;
					break;
				case 0x1F98 :
					mapChar = 0x1F90;
					break;
				case 0x1F99 :
					mapChar = 0x1F91;
					break;
				case 0x1F9A :
					mapChar = 0x1F92;
					break;
				case 0x1F9B :
					mapChar = 0x1F93;
					break;
				case 0x1F9C :
					mapChar = 0x1F94;
					break;
				case 0x1F9D :
					mapChar = 0x1F95;
					break;
				case 0x1F9E :
					mapChar = 0x1F96;
					break;
				case 0x1F9F :
					mapChar = 0x1F97;
					break;
				case 0x1FA8 :
					mapChar = 0x1FA0;
					break;
				case 0x1FA9 :
					mapChar = 0x1FA1;
					break;
				case 0x1FAA :
					mapChar = 0x1FA2;
					break;
				case 0x1FAB :
					mapChar = 0x1FA3;
					break;
				case 0x1FAC :
					mapChar = 0x1FA4;
					break;
				case 0x1FAD :
					mapChar = 0x1FA5;
					break;
				case 0x1FAE :
					mapChar = 0x1FA6;
					break;
				case 0x1FAF :
					mapChar = 0x1FA7;
					break;
				case 0x1FBC :
					mapChar = 0x1FB3;
					break;
				case 0x1FCC :
					mapChar = 0x1FC3;
					break;
				case 0x1FFC :
					mapChar = 0x1FF3;
					break;

				case 0x023A :
					mapChar = 0x2C65;
					break;
				case 0x023E :
					mapChar = 0x2C66;
					break;
				case 0x10A0 :
					mapChar = 0x2D00;
					break;
				case 0x10A1 :
					mapChar = 0x2D01;
					break;
				case 0x10A2 :
					mapChar = 0x2D02;
					break;
				case 0x10A3 :
					mapChar = 0x2D03;
					break;
				case 0x10A4 :
					mapChar = 0x2D04;
					break;
				case 0x10A5 :
					mapChar = 0x2D05;
					break;
				case 0x10A6 :
					mapChar = 0x2D06;
					break;
				case 0x10A7 :
					mapChar = 0x2D07;
					break;
				case 0x10A8 :
					mapChar = 0x2D08;
					break;
				case 0x10A9 :
					mapChar = 0x2D09;
					break;
				case 0x10AA :
					mapChar = 0x2D0A;
					break;
				case 0x10AB :
					mapChar = 0x2D0B;
					break;
				case 0x10AC :
					mapChar = 0x2D0C;
					break;
				case 0x10AD :
					mapChar = 0x2D0D;
					break;
				case 0x10AE :
					mapChar = 0x2D0E;
					break;
				case 0x10AF :
					mapChar = 0x2D0F;
					break;
				case 0x10B0 :
					mapChar = 0x2D10;
					break;
				case 0x10B1 :
					mapChar = 0x2D11;
					break;
				case 0x10B2 :
					mapChar = 0x2D12;
					break;
				case 0x10B3 :
					mapChar = 0x2D13;
					break;
				case 0x10B4 :
					mapChar = 0x2D14;
					break;
				case 0x10B5 :
					mapChar = 0x2D15;
					break;
				case 0x10B6 :
					mapChar = 0x2D16;
					break;
				case 0x10B7 :
					mapChar = 0x2D17;
					break;
				case 0x10B8 :
					mapChar = 0x2D18;
					break;
				case 0x10B9 :
					mapChar = 0x2D19;
					break;
				case 0x10BA :
					mapChar = 0x2D1A;
					break;
				case 0x10BB :
					mapChar = 0x2D1B;
					break;
				case 0x10BC :
					mapChar = 0x2D1C;
					break;
				case 0x10BD :
					mapChar = 0x2D1D;
					break;
				case 0x10BE :
					mapChar = 0x2D1E;
					break;
				case 0x10BF :
					mapChar = 0x2D1F;
					break;
				case 0x10C0 :
					mapChar = 0x2D20;
					break;
				case 0x10C1 :
					mapChar = 0x2D21;
					break;
				case 0x10C2 :
					mapChar = 0x2D22;
					break;
				case 0x10C3 :
					mapChar = 0x2D23;
					break;
				case 0x10C4 :
					mapChar = 0x2D24;
					break;
				case 0x10C5 :
					mapChar = 0x2D25;
					break;
				case 0x10C7 :
					mapChar = 0x2D27;
					break;
				case 0x10CD :
					mapChar = 0x2D2D;
					break;
				case 0x1E9E :
					mapChar = 0x00DF;
					break;
				case 0x2C62 :
					mapChar = 0x026B;
					break;
				case 0x2C63 :
					mapChar = 0x1D7D;
					break;
				case 0x2C64 :
					mapChar = 0x027D;
					break;
				case 0x2C6D :
					mapChar = 0x0251;
					break;
				case 0x2C6E :
					mapChar = 0x0271;
					break;
				case 0x2C6F :
					mapChar = 0x0250;
					break;
				case 0x2C70 :
					mapChar = 0x0252;
					break;
				case 0x2C7E :
					mapChar = 0x023F;
					break;
				case 0x2C7F :
					mapChar = 0x0240;
					break;
				case 0xA77D :
					mapChar = 0x1D79;
					break;
				case 0xA78D :
					mapChar = 0x0265;
					break;
				case 0xA7AA :
					mapChar = 0x0266;
					break;
				  // default mapChar is already set, so no
				  // need to redo it here.
				  // default       : mapChar = ch;
				}
			  }
			  else
			  {
				int offset = val << 5 >> (5 + 18);
				mapChar = ch + offset;
			  }
			}
			return mapChar;
		}

		internal override int ToUpperCase(int ch)
		{
			int mapChar = ch;
			int val = GetProperties(ch);

			if ((val & 0x00010000) != 0)
			{
			  if ((val & 0x07FC0000) == 0x07FC0000)
			  {
				switch (ch)
				{
				  // map chars with overflow offsets
				case 0x00B5 :
					mapChar = 0x039C;
					break;
				case 0x017F :
					mapChar = 0x0053;
					break;
				case 0x1FBE :
					mapChar = 0x0399;
					break;
				  // map char that have both a 1:1 and 1:M map
				case 0x1F80 :
					mapChar = 0x1F88;
					break;
				case 0x1F81 :
					mapChar = 0x1F89;
					break;
				case 0x1F82 :
					mapChar = 0x1F8A;
					break;
				case 0x1F83 :
					mapChar = 0x1F8B;
					break;
				case 0x1F84 :
					mapChar = 0x1F8C;
					break;
				case 0x1F85 :
					mapChar = 0x1F8D;
					break;
				case 0x1F86 :
					mapChar = 0x1F8E;
					break;
				case 0x1F87 :
					mapChar = 0x1F8F;
					break;
				case 0x1F90 :
					mapChar = 0x1F98;
					break;
				case 0x1F91 :
					mapChar = 0x1F99;
					break;
				case 0x1F92 :
					mapChar = 0x1F9A;
					break;
				case 0x1F93 :
					mapChar = 0x1F9B;
					break;
				case 0x1F94 :
					mapChar = 0x1F9C;
					break;
				case 0x1F95 :
					mapChar = 0x1F9D;
					break;
				case 0x1F96 :
					mapChar = 0x1F9E;
					break;
				case 0x1F97 :
					mapChar = 0x1F9F;
					break;
				case 0x1FA0 :
					mapChar = 0x1FA8;
					break;
				case 0x1FA1 :
					mapChar = 0x1FA9;
					break;
				case 0x1FA2 :
					mapChar = 0x1FAA;
					break;
				case 0x1FA3 :
					mapChar = 0x1FAB;
					break;
				case 0x1FA4 :
					mapChar = 0x1FAC;
					break;
				case 0x1FA5 :
					mapChar = 0x1FAD;
					break;
				case 0x1FA6 :
					mapChar = 0x1FAE;
					break;
				case 0x1FA7 :
					mapChar = 0x1FAF;
					break;
				case 0x1FB3 :
					mapChar = 0x1FBC;
					break;
				case 0x1FC3 :
					mapChar = 0x1FCC;
					break;
				case 0x1FF3 :
					mapChar = 0x1FFC;
					break;

				case 0x023F :
					mapChar = 0x2C7E;
					break;
				case 0x0240 :
					mapChar = 0x2C7F;
					break;
				case 0x0250 :
					mapChar = 0x2C6F;
					break;
				case 0x0251 :
					mapChar = 0x2C6D;
					break;
				case 0x0252 :
					mapChar = 0x2C70;
					break;
				case 0x0265 :
					mapChar = 0xA78D;
					break;
				case 0x0266 :
					mapChar = 0xA7AA;
					break;
				case 0x026B :
					mapChar = 0x2C62;
					break;
				case 0x0271 :
					mapChar = 0x2C6E;
					break;
				case 0x027D :
					mapChar = 0x2C64;
					break;
				case 0x1D79 :
					mapChar = 0xA77D;
					break;
				case 0x1D7D :
					mapChar = 0x2C63;
					break;
				case 0x2C65 :
					mapChar = 0x023A;
					break;
				case 0x2C66 :
					mapChar = 0x023E;
					break;
				case 0x2D00 :
					mapChar = 0x10A0;
					break;
				case 0x2D01 :
					mapChar = 0x10A1;
					break;
				case 0x2D02 :
					mapChar = 0x10A2;
					break;
				case 0x2D03 :
					mapChar = 0x10A3;
					break;
				case 0x2D04 :
					mapChar = 0x10A4;
					break;
				case 0x2D05 :
					mapChar = 0x10A5;
					break;
				case 0x2D06 :
					mapChar = 0x10A6;
					break;
				case 0x2D07 :
					mapChar = 0x10A7;
					break;
				case 0x2D08 :
					mapChar = 0x10A8;
					break;
				case 0x2D09 :
					mapChar = 0x10A9;
					break;
				case 0x2D0A :
					mapChar = 0x10AA;
					break;
				case 0x2D0B :
					mapChar = 0x10AB;
					break;
				case 0x2D0C :
					mapChar = 0x10AC;
					break;
				case 0x2D0D :
					mapChar = 0x10AD;
					break;
				case 0x2D0E :
					mapChar = 0x10AE;
					break;
				case 0x2D0F :
					mapChar = 0x10AF;
					break;
				case 0x2D10 :
					mapChar = 0x10B0;
					break;
				case 0x2D11 :
					mapChar = 0x10B1;
					break;
				case 0x2D12 :
					mapChar = 0x10B2;
					break;
				case 0x2D13 :
					mapChar = 0x10B3;
					break;
				case 0x2D14 :
					mapChar = 0x10B4;
					break;
				case 0x2D15 :
					mapChar = 0x10B5;
					break;
				case 0x2D16 :
					mapChar = 0x10B6;
					break;
				case 0x2D17 :
					mapChar = 0x10B7;
					break;
				case 0x2D18 :
					mapChar = 0x10B8;
					break;
				case 0x2D19 :
					mapChar = 0x10B9;
					break;
				case 0x2D1A :
					mapChar = 0x10BA;
					break;
				case 0x2D1B :
					mapChar = 0x10BB;
					break;
				case 0x2D1C :
					mapChar = 0x10BC;
					break;
				case 0x2D1D :
					mapChar = 0x10BD;
					break;
				case 0x2D1E :
					mapChar = 0x10BE;
					break;
				case 0x2D1F :
					mapChar = 0x10BF;
					break;
				case 0x2D20 :
					mapChar = 0x10C0;
					break;
				case 0x2D21 :
					mapChar = 0x10C1;
					break;
				case 0x2D22 :
					mapChar = 0x10C2;
					break;
				case 0x2D23 :
					mapChar = 0x10C3;
					break;
				case 0x2D24 :
					mapChar = 0x10C4;
					break;
				case 0x2D25 :
					mapChar = 0x10C5;
					break;
				case 0x2D27 :
					mapChar = 0x10C7;
					break;
				case 0x2D2D :
					mapChar = 0x10CD;
					break;
				  // ch must have a 1:M case mapping, but we
				  // can't handle it here. Return ch.
				  // since mapChar is already set, no need
				  // to redo it here.
				  //default       : mapChar = ch;
				}
			  }
			  else
			  {
				int offset = val << 5 >> (5 + 18);
				mapChar = ch - offset;
			  }
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
					case 0x0BF1: // TAMIL NUMBER ONE HUNDRED
						retval = 100;
						break;
					case 0x0BF2: // TAMIL NUMBER ONE THOUSAND
						retval = 1000;
						break;
					case 0x1375: // ETHIOPIC NUMBER FORTY
						retval = 40;
						break;
					case 0x1376: // ETHIOPIC NUMBER FIFTY
						retval = 50;
						break;
					case 0x1377: // ETHIOPIC NUMBER SIXTY
						retval = 60;
						break;
					case 0x1378: // ETHIOPIC NUMBER SEVENTY
						retval = 70;
						break;
					case 0x1379: // ETHIOPIC NUMBER EIGHTY
						retval = 80;
						break;
					case 0x137A: // ETHIOPIC NUMBER NINETY
						retval = 90;
						break;
					case 0x137B: // ETHIOPIC NUMBER HUNDRED
						retval = 100;
						break;
					case 0x137C: // ETHIOPIC NUMBER TEN THOUSAND
						retval = 10000;
						break;
					case 0x215F: // FRACTION NUMERATOR ONE
						retval = 1;
						break;
					case 0x216C: // ROMAN NUMERAL FIFTY
						retval = 50;
						break;
					case 0x216D: // ROMAN NUMERAL ONE HUNDRED
						retval = 100;
						break;
					case 0x216E: // ROMAN NUMERAL FIVE HUNDRED
						retval = 500;
						break;
					case 0x216F: // ROMAN NUMERAL ONE THOUSAND
						retval = 1000;
						break;
					case 0x217C: // SMALL ROMAN NUMERAL FIFTY
						retval = 50;
						break;
					case 0x217D: // SMALL ROMAN NUMERAL ONE HUNDRED
						retval = 100;
						break;
					case 0x217E: // SMALL ROMAN NUMERAL FIVE HUNDRED
						retval = 500;
						break;
					case 0x217F: // SMALL ROMAN NUMERAL ONE THOUSAND
						retval = 1000;
						break;
					case 0x2180: // ROMAN NUMERAL ONE THOUSAND C D
						retval = 1000;
						break;
					case 0x2181: // ROMAN NUMERAL FIVE THOUSAND
						retval = 5000;
						break;
					case 0x2182: // ROMAN NUMERAL TEN THOUSAND
						retval = 10000;
						break;

					case 0x324B:
						retval = 40;
						break;
					case 0x324C:
						retval = 50;
						break;
					case 0x324D:
						retval = 60;
						break;
					case 0x324E:
						retval = 70;
						break;
					case 0x324F:
						retval = 80;
						break;
					case 0x325C:
						retval = 32;
						break;

					case 0x325D: // CIRCLED NUMBER THIRTY THREE
						retval = 33;
						break;
					case 0x325E: // CIRCLED NUMBER THIRTY FOUR
						retval = 34;
						break;
					case 0x325F: // CIRCLED NUMBER THIRTY FIVE
						retval = 35;
						break;
					case 0x32B1: // CIRCLED NUMBER THIRTY SIX
						retval = 36;
						break;
					case 0x32B2: // CIRCLED NUMBER THIRTY SEVEN
						retval = 37;
						break;
					case 0x32B3: // CIRCLED NUMBER THIRTY EIGHT
						retval = 38;
						break;
					case 0x32B4: // CIRCLED NUMBER THIRTY NINE
						retval = 39;
						break;
					case 0x32B5: // CIRCLED NUMBER FORTY
						retval = 40;
						break;
					case 0x32B6: // CIRCLED NUMBER FORTY ONE
						retval = 41;
						break;
					case 0x32B7: // CIRCLED NUMBER FORTY TWO
						retval = 42;
						break;
					case 0x32B8: // CIRCLED NUMBER FORTY THREE
						retval = 43;
						break;
					case 0x32B9: // CIRCLED NUMBER FORTY FOUR
						retval = 44;
						break;
					case 0x32BA: // CIRCLED NUMBER FORTY FIVE
						retval = 45;
						break;
					case 0x32BB: // CIRCLED NUMBER FORTY SIX
						retval = 46;
						break;
					case 0x32BC: // CIRCLED NUMBER FORTY SEVEN
						retval = 47;
						break;
					case 0x32BD: // CIRCLED NUMBER FORTY EIGHT
						retval = 48;
						break;
					case 0x32BE: // CIRCLED NUMBER FORTY NINE
						retval = 49;
						break;
					case 0x32BF: // CIRCLED NUMBER FIFTY
						retval = 50;
						break;

					case 0x0D71: // MALAYALAM NUMBER ONE HUNDRED
						retval = 100;
						break;
					case 0x0D72: // MALAYALAM NUMBER ONE THOUSAND
						retval = 1000;
						break;
					case 0x2186: // ROMAN NUMERAL FIFTY EARLY FORM
						retval = 50;
						break;
					case 0x2187: // ROMAN NUMERAL FIFTY THOUSAND
						retval = 50000;
						break;
					case 0x2188: // ROMAN NUMERAL ONE HUNDRED THOUSAND
						retval = 100000;
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
				switch (ch)
				{
					case 0x202A :
						// This is the only char with LRE
						directionality = Character.DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING;
						break;
					case 0x202B :
						// This is the only char with RLE
						directionality = Character.DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING;
						break;
					case 0x202C :
						// This is the only char with PDF
						directionality = Character.DIRECTIONALITY_POP_DIRECTIONAL_FORMAT;
						break;
					case 0x202D :
						// This is the only char with LRO
						directionality = Character.DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE;
						break;
					case 0x202E :
						// This is the only char with RLO
						directionality = Character.DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE;
						break;
					default :
						directionality = Character.DIRECTIONALITY_UNDEFINED;
						break;
				}
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
						case 0x017F :
							mapChar = 0x0053;
							break;
						case 0x1FBE :
							mapChar = 0x0399;
							break;

						case 0x023F :
							mapChar = 0x2C7E;
							break;
						case 0x0240 :
							mapChar = 0x2C7F;
							break;
						case 0x0250 :
							mapChar = 0x2C6F;
							break;
						case 0x0251 :
							mapChar = 0x2C6D;
							break;
						case 0x0252 :
							mapChar = 0x2C70;
							break;
						case 0x0265 :
							mapChar = 0xA78D;
							break;
						case 0x0266 :
							mapChar = 0xA7AA;
							break;
						case 0x026B :
							mapChar = 0x2C62;
							break;
						case 0x0271 :
							mapChar = 0x2C6E;
							break;
						case 0x027D :
							mapChar = 0x2C64;
							break;
						case 0x1D79 :
							mapChar = 0xA77D;
							break;
						case 0x1D7D :
							mapChar = 0x2C63;
							break;
						case 0x2C65 :
							mapChar = 0x023A;
							break;
						case 0x2C66 :
							mapChar = 0x023E;
							break;
						case 0x2D00 :
							mapChar = 0x10A0;
							break;
						case 0x2D01 :
							mapChar = 0x10A1;
							break;
						case 0x2D02 :
							mapChar = 0x10A2;
							break;
						case 0x2D03 :
							mapChar = 0x10A3;
							break;
						case 0x2D04 :
							mapChar = 0x10A4;
							break;
						case 0x2D05 :
							mapChar = 0x10A5;
							break;
						case 0x2D06 :
							mapChar = 0x10A6;
							break;
						case 0x2D07 :
							mapChar = 0x10A7;
							break;
						case 0x2D08 :
							mapChar = 0x10A8;
							break;
						case 0x2D09 :
							mapChar = 0x10A9;
							break;
						case 0x2D0A :
							mapChar = 0x10AA;
							break;
						case 0x2D0B :
							mapChar = 0x10AB;
							break;
						case 0x2D0C :
							mapChar = 0x10AC;
							break;
						case 0x2D0D :
							mapChar = 0x10AD;
							break;
						case 0x2D0E :
							mapChar = 0x10AE;
							break;
						case 0x2D0F :
							mapChar = 0x10AF;
							break;
						case 0x2D10 :
							mapChar = 0x10B0;
							break;
						case 0x2D11 :
							mapChar = 0x10B1;
							break;
						case 0x2D12 :
							mapChar = 0x10B2;
							break;
						case 0x2D13 :
							mapChar = 0x10B3;
							break;
						case 0x2D14 :
							mapChar = 0x10B4;
							break;
						case 0x2D15 :
							mapChar = 0x10B5;
							break;
						case 0x2D16 :
							mapChar = 0x10B6;
							break;
						case 0x2D17 :
							mapChar = 0x10B7;
							break;
						case 0x2D18 :
							mapChar = 0x10B8;
							break;
						case 0x2D19 :
							mapChar = 0x10B9;
							break;
						case 0x2D1A :
							mapChar = 0x10BA;
							break;
						case 0x2D1B :
							mapChar = 0x10BB;
							break;
						case 0x2D1C :
							mapChar = 0x10BC;
							break;
						case 0x2D1D :
							mapChar = 0x10BD;
							break;
						case 0x2D1E :
							mapChar = 0x10BE;
							break;
						case 0x2D1F :
							mapChar = 0x10BF;
							break;
						case 0x2D20 :
							mapChar = 0x10C0;
							break;
						case 0x2D21 :
							mapChar = 0x10C1;
							break;
						case 0x2D22 :
							mapChar = 0x10C2;
							break;
						case 0x2D23 :
							mapChar = 0x10C3;
							break;
						case 0x2D24 :
							mapChar = 0x10C4;
							break;
						case 0x2D25 :
							mapChar = 0x10C5;
							break;
						case 0x2D27 :
							mapChar = 0x10C7;
							break;
						case 0x2D2D :
							mapChar = 0x10CD;
							break;
						default :
							mapChar = Character.ERROR;
							break;
					}
				}
			}
			return mapChar;
		}

		internal override char[] ToUpperCaseCharArray(int ch)
		{
			char[] upperMap = new char[] {(char)ch};
			int location = FindInCharMap(ch);
			if (location != -1)
			{
				upperMap = CharMap[location][1];
			}
			return upperMap;
		}


		/// <summary>
		/// Finds the character in the uppercase mapping table.
		/// </summary>
		/// <param name="ch"> the <code>char</code> to search </param>
		/// <returns> the index location ch in the table or -1 if not found
		/// @since 1.4 </returns>
		 internal virtual int FindInCharMap(int ch)
		 {
			if (CharMap == null || CharMap.Length == 0)
			{
				return -1;
			}
			int top, bottom, current;
			bottom = 0;
			top = CharMap.Length;
			current = top / 2;
			// invariant: top > current >= bottom && ch >= CharacterData.charMap[bottom][0]
			while (top - bottom > 1)
			{
				if (ch >= CharMap[current][0][0])
				{
					bottom = current;
				}
				else
				{
					top = current;
				}
				current = (top + bottom) / 2;
			}
			if (ch == CharMap[current][0][0])
			{
				return current;
			}
			else
			{
				return -1;
			}
		 }

		internal static readonly CharacterData00 Instance = new CharacterData00();
		private CharacterData00()
		{
		};

		// The following tables and code generated using:
	  // java GenerateCharacter -plane 0 -template d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/characterdata/CharacterData00.java.template -spec d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/UnicodeData.txt -specialcasing d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/SpecialCasing.txt -proplist d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/PropList.txt -o d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/build/windows-amd64/jdk/gensrc/java/lang/CharacterData00.java -string -usecharforbyte 11 4 1
		  internal static readonly char[][][] CharMap;
	// The X table has 2048 entries for a total of 4096 bytes.

	  internal static readonly char[] X = ("\x0000\x0010\x0020\x0030\x0040\x0050\x0060\x0070\x0080\x0090\x00A0\x00B0\x00C0\x00D0\x00E0\x00F0\x0080\u0100" + "\u0110\u0120\u0130\u0140\u0150\u0160\u0170\u0170\u0180\u0190\u01A0\u01B0\u01C0" + "\u01D0\u01E0\u01F0\u0200\x0080\u0210\x0080\u0220\x0080\x0080\u0230\u0240\u0250\u0260" + "\u0270\u0280\u0290\u02A0\u02B0\u02C0\u02D0\u02B0\u02B0\u02E0\u02F0\u0300\u0310" + "\u0320\u02B0\u02B0\u0330\u0340\u0350\u0360\u0370\u0380\u0390\u0390\u03A0\u0390" + "\u03B0\u03C0\u03D0\u03E0\u03F0\u0400\u0410\u0420\u0430\u0440\u0450\u0460\u0470" + "\u0480\u0490\u04A0\u04B0\u0400\u04C0\u04D0\u04E0\u04F0\u0500\u0510\u0520\u0530" + "\u0540\u0550\u0560\u0570\u0580\u0590\u05A0\u0570\u05B0\u05C0\u05D0\u05E0\u05F0" + "\u0600\u0610\u0620\u0630\u0640\u0390\u0650\u0660\u0670\u0390\u0680\u0690\u06A0" + "\u06B0\u06C0\u06D0\u06E0\u0390\u06F0\u0700\u0710\u0720\u0730\u0740\u0750\u0760" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u0770\u06F0\u0780" + "\u0790\u07A0\u06F0\u07B0\u06F0\u07C0\u07D0\u07E0\u06F0\u06F0\u07F0\u0800\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u0810\u0820\u06F0\u06F0\u0830\u0840\u0850\u0860\u0870" + "\u06F0\u0880\u0890\u08A0\u08B0\u06F0\u08C0\u08D0\u06F0\u08E0\u06F0\u08F0\u0900" + "\u0910\u0920\u0930\u06F0\u0940\u0950\u0960\u0970\u06F0\u0980\u0990\u09A0\u09B0" + "\u0390\u0390\u09C0\u09D0\u09E0\u09F0\u0A00\u0A10\u06F0\u0A20\u06F0\u0A30\u0A40" + "\u0A50\u0390\u0390\u0A60\u0A70\u0A80\u0A90\u0AA0\u0AB0\u0AC0\u0AA0\u0170\u0AD0" + "\x0080\x0080\x0080\x0080\u0AE0\x0080\x0080\x0080\u0AF0\u0B00\u0B10\u0B20\u0B30\u0B40\u0B50" + "\u0B60\u0B70\u0B80\u0B90\u0BA0\u0BB0\u0BC0\u0BD0\u0BE0\u0BF0\u0C00\u0C10\u0C20" + "\u0C30\u0C40\u0C50\u0C60\u0C70\u0C80\u0C90\u0CA0\u0CB0\u0CC0\u0CD0\u0CE0\u0CF0" + "\u0D00\u0D10\u0D20\u0D30\u0D40\u0D50\u0D60\u0960\u0D70\u0D80\u0D90\u0DA0\u0DB0" + "\u0DC0\u0DD0\u0960\u0960\u0960\u0960\u0960\u0DE0\u0DF0\u0E00\u0960\u0960\u0960" + "\u0E10\u0960\u0E20\u0960\u0960\u0E30\u0960\u0960\u0E40\u0E50\u0960\u0E60\u0E70" + "\u0D10\u0D10\u0D10\u0D10\u0D10\u0D10\u0D10\u0D10\u0E80\u0E80\u0E80\u0E80\u0E90" + "\u0EA0\u0EB0\u0EC0\u0ED0\u0EE0\u0EF0\u0F00\u0F10\u0F20\u0F30\u0F40\u0960\u0F50" + "\u0F60\u0390\u0390\u0390\u0390\u0390\u0F70\u0F80\u0F90\u0FA0\x0080\x0080\x0080\u0FB0" + "\u0FC0\u0FD0\u06F0\u0FE0\u0FF0\u1000\u1000\u1010\u1020\u1030\u0390\u0390\u1040" + "\u0960\u0960\u1050\u0960\u0960\u0960\u0960\u0960\u0960\u1060\u1070\u1080\u1090" + "\u0620\u06F0\u10A0\u0800\u06F0\u10B0\u10C0\u10D0\u06F0\u06F0\u10E0\u10F0\u0960" + "\u1100\u1110\u1120\u1130\u1140\u1120\u1150\u1160\u1170\u0D10\u0D10\u0D10\u1180" + "\u0D10\u0D10\u1190\u11A0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11C0\u0960\u0960\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0" + "\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11B0\u11D0\u0390\u11E0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u11F0\u0960\u1200\u0A50\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u1210\u1220\x0080\u1230\u1240\u06F0\u06F0" + "\u1250\u1260\u1270\x0080\u1280\u1290\u12A0\u0390\u12B0\u12C0\u12D0\u06F0\u12E0" + "\u12F0\u1300\u1310\u1320\u1330\u1340\u1350\u0900\u03C0\u1360\u1370\u0390\u06F0" + "\u1380\u1390\u13A0\u06F0\u13B0\u13C0\u13D0\u13E0\u13F0\u0390\u0390\u0390\u0390" + "\u06F0\u1400\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0\u06F0" + "\u1410\u1420\u1430\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440" + "\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440" + "\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440" + "\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440" + "\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440\u1440" + "\u1440\u1440\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u1450" + "\u1450\u1450\u1450\u1450\u1450\u1450\u1450\u11B0\u11B0\u11B0\u1460\u11B0\u1470" + "\u1480\u1490\u11B0\u11B0\u11B0\u14A0\u11B0\u11B0\u14B0\u0390\u14C0\u14D0\u14E0" + "\u02B0\u02B0\u14F0\u1500\u02B0\u02B0\u02B0\u02B0\u02B0\u02B0\u02B0\u02B0\u02B0" + "\u02B0\u1510\u1520\u02B0\u1530\u02B0\u1540\u1550\u1560\u1570\u1580\u1590\u02B0" + "\u02B0\u02B0\u15A0\u15B0\x0020\u15C0\u15D0\u15E0\u15F0\u1600\u1610").ToCharArray();

	  // The Y table has 5664 entries for a total of 11328 bytes.

	  internal static readonly char[] Y = ("\x0000\x0000\x0000\x0000\x0002\x0004\x0006\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0008\x0004\x000A\x000C\x000E" + "\x0010\x0012\x0014\x0016\x0018\x001A\x001A\x001A\x001A\x001A\x001C\x001E\x0020\x0022\x0024\x0024\x0024\x0024\x0024" + "\x0024\x0024\x0024\x0024\x0024\x0024\x0024\x0026\x0028\x002A\x002C\x002E\x002E\x002E\x002E\x002E\x002E\x002E\x002E" + "\x002E\x002E\x002E\x002E\x0030\x0032\x0034\x0000\x0000\x0036\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0038\x003A\x003A\x003C\x003E\x0040\x0042\x0044\x0046\x0048\x004A\x0010\x004C\x004E\x0050" + "\x0052\x0054\x0054\x0054\x0054\x0054\x0054\x0054\x0054\x0054\x0054\x0054\x0056\x0054\x0054\x0054\x0058\x005A\x005A" + "\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005C\x005A\x005A\x005A\x005E\x0060\x0060\x0060\x0060\x0060" + "\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060" + "\x0062\x0060\x0060\x0060\x0064\x0066\x0066\x0066\x0066\x0066\x0066\x0066\x0068\x0060\x0060\x0060\x0060\x0060\x0060" + "\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x006A\x0066" + "\x0066\x0068\x006C\x0060\x0060\x006E\x0070\x0072\x0074\x0076\x0078\x0070\x007A\x007C\x0060\x007E\x0080\x0082\x0060" + "\x0060\x0060\x0084\x0086\x0088\x0060\x0084\x008A\x008C\x0066\x008E\x0060\x0090\x0060\x0092\x0094\x0094\x0096\x0098" + "\x009A\x0096\x009C\x0066\x0066\x0066\x0066\x0066\x0066\x0066\x009E\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060" + "\x0060\x00A0\x009A\x0060\x00A2\x0060\x0060\x0060\x0060\x00A4\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060" + "\x0088\x0088\x0088\x00A6\x00A8\x00AA\x00AC\x00AE\x00B0\x0060\x0060\x0060\x0060\x0060\x00B2\x00B4\x00B6\x00B8\x00BA" + "\x00BC\x0088\x0088\x00BE\x00C0\x00C2\x00C4\x00C6\x00C2\x0088\x00C8\x00C2\x00CA\x00CC\x0088\x0088\x0088\x00C2\x0088" + "\x00CE\x00D0\x0088\x0088\x00D2\x00D4\x00D6\x0088\x0088\x00D8\x00DA\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088" + "\x0088\x0088\x0088\x0088\x0088\x00DC\x00DC\x00DC\x00DC\x00DE\x00E0\x00E2\x00E2\x00DC\x00E4\x00E4\x00E6\x00E6\x00E6" + "\x00E6\x00E6\x00E2\x00E4\x00E4\x00E4\x00E4\x00E4\x00E4\x00E4\x00DC\x00DC\x00E8\x00E4\x00E4\x00E4\x00EA\x00EC\x00E4" + "\x00E4\x00E4\x00E4\x00E4\x00E4\x00E4\x00E4\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE" + "\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00F0\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE" + "\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x0060\x0060\x00EA\x0060\x00F2\x00F4\x00F6\x00F8\x00F2\x00F2" + "\x00E4\x00FA\x00FC\x00FE\u0100\u0102\u0104\x0054\x0054\x0054\x0054\x0054\x0054\x0054\x0054\u0106\x0054" + "\x0054\x0054\x0054\u0108\u010A\u010C\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005A\u010E\x005A\x005A" + "\x005A\x005A\u0110\u0112\u0114\u0116\u0118\u011A\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060" + "\x0060\x0060\x0060\x0060\u011C\u011E\u0120\u0122\u0124\x0060\u0126\u0128\u012A\u012A" + "\u012A\u012A\u012A\u012A\u012A\u012A\x0054\x0054\x0054\x0054\x0054\x0054\x0054\x0054\x0054\x0054" + "\x0054\x0054\x0054\x0054\x0054\x0054\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005A\x005A" + "\x005A\x005A\x005A\u012C\u012C\u012C\u012C\u012C\u012C\u012C\u012C\x0060\u012E\x00EE" + "\x00EE\u0130\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\u0132\x0066\x0066\x0066\x0066" + "\x0066\x0066\u0134\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x00F2\x00F2\x00F2" + "\x00F2\u0136\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138" + "\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u013A\u013C\u013E\u013E\u013E\u0140" + "\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142" + "\u0142\u0142\u0142\u0142\u0142\u0144\u0146\u0148\x00F2\u014A\u014C\x00EE\x00EE\x00EE" + "\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\u014E\u014E\u014E\u014E\u014E" + "\u014E\u014E\u0150\u0152\u0154\u014E\u0152\x00F2\x00F2\x00F2\x00F2\u0156\u0156\u0156" + "\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0158\x00F2\x00F2" + "\u0156\u015A\u015C\x00F2\x00F2\x00F2\x00F2\x00F2\u015E\u015E\u0160\u0162\u0164\u0166" + "\u0168\u016A\u014E\u014E\u014E\u014E\u014E\u016C\x00F2\u016E\u0170\u0170\u0170" + "\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170" + "\u0172\u0170\u0170\u0170\u0170\u0174\u014E\u014E\u014E\u014E\u014E\u014E\u0176" + "\u014E\u014E\u014E\u0178\u0178\u0178\u0178\u0178\u017A\u017C\u0170\u017E\u0170" + "\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170" + "\u0170\u0170\u0170\u0180\u014E\u014E\u014E\u0182\u0184\u0176\u014E\u0186\u0188" + "\u018A\x00EE\u0176\u0170\x001A\x001A\x001A\x001A\x001A\u0170\u018C\u018E\u016E\u016E\u016E" + "\u016E\u016E\u016E\u016E\u0190\u0174\u0170\u0170\u0170\u0170\u0170\u0170\u0170" + "\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u014E\u014E\u014E\u014E\u014E" + "\u014E\u014E\u014E\x00EE\x00EE\x00EE\x00EE\x00EE\u0192\u0194\u0170\u0170\u0170\u0170" + "\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u014E\u014E\u014E\u014E\u014E" + "\u017E\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\u0196\u0196\u0196\u0196\u0196\u0156\u0156" + "\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156" + "\u0156\u0198\x00EE\x00EE\x00EE\x00EE\u019A\x003C\x0010\u019C\x00F2\x00F2\u0156\u0156\u0156" + "\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u014E\x00EE\u019E\u014E\u014E" + "\u014E\u014E\u019E\u014E\u019E\u014E\u01A0\x00F2\u01A2\u01A2\u01A2\u01A2\u01A2" + "\u01A2\u01A2\u015C\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156\u0156" + "\u0156\u0156\u0198\x00EE\x00F2\u015C\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2" + "\x00F2\x00F2\x00F2\x00F2\x00F2\u01A4\u0170\u0170\u0170\u0170\u0170\u01A4\x00F2\x00F2\x00F2" + "\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\u014E\u014E\u014E\x00EE\x00EE\x00EE\u014E\u014E" + "\u014E\u014E\u014E\u014E\u014E\u01A6\u014E\u01A8\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\u01A8\u01AA\u01AC\u01AE\u014E\u014E\u014E\u01A8\u01AC\u01B0\u01AC\u01B2" + "\x00EE\u0176\u014E\x0094\x0094\x0094\x0094\x0094\u014E\u013E\u01B4\u01B4\u01B4\u01B4\u01B4" + "\u01B6\x0094\x0094\x0094\u01B8\x0094\x0094\x0094\u01BA\u01AC\u01B8\x0094\x0094\x0094\u01BC\u01B8" + "\u01BC\u01B8\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01BC\x0094\x0094\x0094\u01BC" + "\u01BC\x00F2\x0094\x0094\x00F2\u01AA\u01AC\u01AE\u014E\u01A6\u01BE\u01C0\u01BE\u01B0" + "\u01BC\x00F2\x00F2\x00F2\u01BE\x00F2\x00F2\x0094\u01B8\x0094\u014E\x00F2\u01B4\u01B4\u01B4" + "\u01B4\u01B4\x0094\x003A\u01C2\u01C2\u01C4\u01C6\x00F2\x00F2\u01BA\u01A8\u01B8\x0094" + "\x0094\u01BC\x00F2\u01B8\u01BC\u01B8\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01BC" + "\x0094\x0094\x0094\u01BC\x0094\u01B8\u01BC\x0094\x00F2\u0192\u01AC\u01AE\u01A6\x00F2\u01BA" + "\u01A6\u01BA\u01A0\x00F2\u01BA\x00F2\x00F2\x00F2\u01B8\x0094\u01BC\u01BC\x00F2\x00F2\x00F2" + "\u01B4\u01B4\u01B4\u01B4\u01B4\u014E\x0094\u01C8\x00F2\x00F2\x00F2\x00F2\x00F2\u01BA\u01A8" + "\u01B8\x0094\x0094\x0094\x0094\u01B8\x0094\u01B8\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\u01BC\x0094\x0094\x0094\u01BC\x0094\u01B8\x0094\x0094\x00F2\u01AA\u01AC\u01AE\u014E" + "\u014E\u01BA\u01A8\u01BE\u01B0\x00F2\u01BC\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\u014E" + "\x00F2\u01B4\u01B4\u01B4\u01B4\u01B4\u01CA\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\x0094" + "\x0094\x0094\u01BC\x0094\x0094\x0094\u01BC\x0094\u01B8\x0094\x0094\x00F2\u01AA\u01AE\u01AE\u014E" + "\u01A6\u01BE\u01C0\u01BE\u01B0\x00F2\x00F2\x00F2\x00F2\u01A8\x00F2\x00F2\x0094\u01B8\x0094" + "\u014E\x00F2\u01B4\u01B4\u01B4\u01B4\u01B4\u01CC\u01C2\u01C2\u01C2\x00F2\x00F2\x00F2" + "\x00F2\x00F2\u01CE\u01B8\x0094\x0094\u01BC\x00F2\x0094\u01BC\x0094\x0094\x00F2\u01B8\u01BC\u01BC" + "\x0094\x00F2\u01B8\u01BC\x00F2\x0094\u01BC\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x00F2\x00F2\u01AC" + "\u01A8\u01C0\x00F2\u01AC\u01C0\u01AC\u01B0\x00F2\u01BC\x00F2\x00F2\u01BE\x00F2\x00F2\x00F2" + "\x00F2\x00F2\x00F2\x00F2\u01B4\u01B4\u01B4\u01B4\u01B4\u01D0\u01D2\u016A\u016A\u01D4" + "\u01D6\x00F2\x00F2\u01BE\u01AC\u01B8\x0094\x0094\x0094\u01BC\x0094\u01BC\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01BC\x0094\x0094\x0094\x0094\x0094\u01B8\x0094\x0094\x00F2\u01B8" + "\u014E\u01A8\u01AC\u01C0\u014E\u01A6\u014E\u01A0\x00F2\x00F2\x00F2\u01BA\u01A6\x0094" + "\x00F2\x00F2\x00F2\x0094\u014E\x00F2\u01B4\u01B4\u01B4\u01B4\u01B4\x00F2\x00F2\x00F2\x00F2\u01D8" + "\u01D8\u01DA\u01DC\x00F2\u01AC\u01B8\x0094\x0094\x0094\u01BC\x0094\u01BC\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01BC\x0094\x0094\x0094\x0094\x0094\u01B8\x0094\x0094\x00F2" + "\u01AA\u01DE\u01AC\u01AC\u01C0\u01E0\u01C0\u01AC\u01A0\x00F2\x00F2\x00F2\u01BE\u01C0" + "\x00F2\x00F2\x00F2\u01BC\x0094\u014E\x00F2\u01B4\u01B4\u01B4\u01B4\u01B4\u01B8\u01BC" + "\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\u01BC\u01B8\u01AC\u01AE\u014E\u01A6\u01AC\u01C0\u01AC\u01B0\u01BC\x00F2\x00F2" + "\x00F2\u01BE\x00F2\x00F2\x00F2\x00F2\x0094\u014E\x00F2\u01B4\u01B4\u01B4\u01B4\u01B4\u01D0" + "\u01C2\u01C2\x00F2\u01E2\x0094\x0094\x0094\x00F2\u01AC\u01B8\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\u01BC\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01B8\x0094" + "\x0094\x0094\x0094\u01B8\x00F2\x0094\x0094\x0094\u01BC\x00F2\u0192\x00F2\u01BE\u01AC\u014E\u01A6" + "\u01A6\u01AC\u01AC\u01AC\u01AC\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\u01AC\u01E4" + "\x00F2\x00F2\x00F2\x00F2\x00F2\u01B8\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01C8\x0094\u014E\u014E\u014E\u01A6" + "\x00F2\u014A\x0094\x0094\x0094\u01E6\x00EE\x00EE\u0176\u01E8\u01EA\u01EA\u01EA\u01EA\u01EA" + "\u013E\x00F2\x00F2\u01B8\u01BC\u01BC\u01B8\u01BC\u01BC\u01B8\x00F2\x00F2\x00F2\x0094\x0094" + "\u01B8\x0094\x0094\x0094\u01B8\x0094\u01B8\u01B8\x00F2\x0094\u01B8\x0094\u01C8\x0094\u014E" + "\u014E\u014E\u01BA\u01CE\x00F2\x0094\x0094\u01BC\u01EC\x00EE\x00EE\u0176\x00F2\u01EA\u01EA" + "\u01EA\u01EA\u01EA\x00F2\x0094\x0094\u01EE\u01F0\u013E\u013E\u013E\u013E\u013E\u013E" + "\u013E\u01F2\u01F2\u01F0\x00EE\u01F0\u01F0\u01F0\u01F4\u01F4\u01F4\u01F4\u01F4" + "\u01C2\u01C2\u01C2\u01C2\u01C2\u012E\u012E\u012E\x0012\x0012\u01F6\x0094\x0094\x0094" + "\x0094\u01B8\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\u01BC\x00F2\u01BA\u014E\u014E\u014E\u014E\u014E\u014E\u01A8\u014E\x00EE\u01E8" + "\x00EE\x0094\x0094\u01C8\u014E\u014E\u014E\u014E\u014E\u01BA\u014E\u014E\u014E\u014E" + "\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E" + "\u01A6\u01F0\u01F0\u01F0\u01F0\u01F8\u01F0\u01F0\u01FA\u01F0\u013E\u013E\u01F2" + "\u01F0\u01FC\u01E4\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01FE\u01AE\u014E\u01A8\u014E\u014E\u01A0" + "\u01B0\u0200\u01AE\u01CE\u01F4\u01F4\u01F4\u01F4\u01F4\u013E\u013E\u013E\x0094" + "\x0094\x0094\u01AC\u014E\x0094\x0094\u014E\u01CE\u0202\u0204\u01FE\u0202\u01F6\u01F6" + "\x0094\u01C8\u014E\u01CE\x0094\x0094\x0094\x0094\x0094\x0094\u01A8\u01AE\u0206\u01F6\u01F6" + "\u0208\u020A\u01EA\u01EA\u01EA\u01EA\u01EA\u01F6\u01AE\u01F0\u020C\u020C\u020C" + "\u020C\u020C\u020C\u020C\u020C\u020C\u020C\u020C\u020C\u020C\u020C\u020C\u020C" + "\u020C\u020C\u020C\u020E\x00F2\x00F2\u020E\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u0210\u0212\x0094\x0094\x0094" + "\x0094\x0094\u01BC\x0094\x0094\x00F2\x0094\x0094\x0094\u01BC\u01BC\x0094\x0094\x00F2\x0094\x0094\x0094" + "\x0094\u01BC\x0094\x0094\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\u01BC\x0094\x0094\x00F2\x0094\x0094\x0094\u01BC\u01BC\x0094\x0094\x00F2\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\u01BC\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\u01BC\x0094\x0094\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\u01BC\u014C\u0176\u013E\u013E\u013E\u013E\u0214\u0216\u0216\u0216" + "\u0216\u0218\u021A\u01C2\u01C2\u01C2\u021C\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\u016A\u016A\u016A\u016A\u016A\x00F2\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\u01BC\x00F2\x00F2\x00F2\x00F2\x00F2\u021E\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u0210\u0220\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\u0222\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\u0224\u0226\x00F2\x0094\x0094\x0094\x0094\x0094\u0210\u013E\u0228\u022A\x00F2\x00F2\x00F2\x00F2" + "\x00F2\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\u01BC\x0094\x0094\u014E\u0192\x00F2\x00F2\x00F2" + "\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u014E\u01E8\u01E4\x00F2\x00F2\x00F2" + "\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u014E\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0094" + "\x0094\x0094\x0094\x0094\x0094\u01BC\x0094\u01BC\u014E\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x00EE\u01AE\u014E\u014E\u014E\u01AC\u01AC\u01AC" + "\u01AC\u01A8\u01B0\x00EE\x00EE\x00EE\x00EE\x00EE\u013E\u01B6\u013E\u01CA\u01B2\x00F2\u01F4" + "\u01F4\u01F4\u01F4\u01F4\x00F2\x00F2\x00F2\u022C\u022C\u022C\u022C\u022C\x00F2\x00F2" + "\x00F2\x0010\x0010\x0010\u022E\x0010\u0230\x00EE\u0232\u01EA\u01EA\u01EA\u01EA\u01EA\x00F2" + "\x00F2\x00F2\x0094\u0234\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x00F2\x00F2\x00F2\x00F2\x0094\x0094\x0094" + "\x0094\u01C8\u01BC\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\u01BC\x00F2\u014E\u01A8\u01AC\u01AE\u01A8\u01AC\x00F2" + "\x00F2\u01AC\u01A8\u01AC\u01AC\u01B0\x00EE\x00F2\x00F2\u01D6\x00F2\x0010\u01B4\u01B4\u01B4" + "\u01B4\u01B4\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x00F2" + "\x0094\x0094\u01BC\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x00F2\x00F2\u01AC\u01AC" + "\u01AC\u01AC\u01AC\u01AC\u01AC\u01AC\u0236\x0094\x0094\x0094\u01AC\x00F2\x00F2\x00F2\u01EA" + "\u01EA\u01EA\u01EA\u01EA\u0238\x00F2\u016A\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01C8\u01A8\u01AC\x00F2\u013E\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\u01FE\u01A8\u014E\u014E\u014E\u01A6\u0200\u01A8\u01AE" + "\u014E\u014E\u014E\u01A8\u01AC\u01AC\u01AE\u01A0\x00EE\x00EE\x00EE\u0192\u014C\u01F4" + "\u01F4\u01F4\u01F4\u01F4\x00F2\x00F2\x00F2\u01EA\u01EA\u01EA\u01EA\u01EA\x00F2\x00F2" + "\x00F2\u013E\u013E\u013E\u01B6\u013E\u013E\u013E\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2" + "\x00F2\x00F2\u014E\u014E\u0236\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u0200\u014E\u014E\u01A8\u01A8" + "\u01AC\u01AC\u01A8\u0204\x0094\x0094\x0094\x00F2\x00F2\u01EA\u01EA\u01EA\u01EA\u01EA" + "\u013E\u013E\u013E\u01F2\u01F0\u01F0\u01F0\u01F0\u012E\x00EE\x00EE\x00EE\x00EE\u01F0" + "\u01F0\u01F0\u01F0\u01FA\x00F2\u014E\u0236\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\u01FE\u014E\u014E\u01AC\u014E\u0208\u01AC\x0094\u01EA\u01EA" + "\u01EA\u01EA\u01EA\x0094\x0094\x0094\x0094\x0094\x0094\u0200\u014E\u01AC\u01AE\u01AE\u014E" + "\u01F6\x00F2\x00F2\x00F2\x00F2\u013E\u013E\x0094\x0094\u01AC\u01AC\u01AC\u01AC\u014E\u014E" + "\u014E\u014E\u01AC\x00EE\x00F2\u0146\u013E\u013E\u01F4\u01F4\u01F4\u01F4\u01F4" + "\x00F2\u01B8\x0094\u01EA\u01EA\u01EA\u01EA\u01EA\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x00E2\x00E2\x00E2\u013E\u013E\u013E\u013E\u013E\x00F2" + "\x00F2\x00F2\x00F2\x00EE\u01E8\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE\u023A\x00EE\x00EE\x00EE\u01AA\x0094" + "\u01B2\x0094\x0094\u01AC\u01AA\u01BC\x00F2\x00F2\x00F2\x00F2\x0088\x0088\x0088\x0088\x0088\x0088\x0088" + "\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x00DC\x00DC\x00DC\x00DC" + "\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC" + "\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\u023C\x0088\x0088\x0088\x0088\x0088\x0088\u023E\x0088\x00C2" + "\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\x0088\u0240\x00DC\x00DC\x00EE" + "\x00EE\x00EE\u0192\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00EE\x00EE\x0060\x0060\x0060" + "\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x00B2\x00B2\u0242\x0088\u0244\u0246\u0246\u0246" + "\u0246\u0248\u0248\u0248\u0248\u0246\u0246\u0246\x00F2\u0248\u0248\u0248\x00F2" + "\u0246\u0246\u0246\u0246\u0248\u0248\u0248\u0248\u0246\u0246\u0246\u0246\u0248" + "\u0248\u0248\u0248\u0246\u0246\u0246\x00F2\u0248\u0248\u0248\x00F2\u024A\u024A" + "\u024A\u024A\u024C\u024C\u024C\u024C\u0246\u0246\u0246\u0246\u0248\u0248\u0248" + "\u0248\u024E\u0250\u0250\u0252\u0254\u0256\u0258\x00F2\x00B2\x00B2\x00B2\x00B2\u025A" + "\u025A\u025A\u025A\x00B2\x00B2\x00B2\x00B2\u025A\u025A\u025A\u025A\x00B2\x00B2\x00B2\x00B2" + "\u025A\u025A\u025A\u025A\u0246\x00B2\u025C\x00B2\u0248\u025E\u0260\u0262\x00E4\x00B2" + "\u025C\x00B2\u0264\u0264\u0260\x00E4\u0246\x00B2\x00F2\x00B2\u0248\u0266\u0268\x00E4\u0246" + "\x00B2\u026A\x00B2\u0248\u026C\u026E\x00E4\x00F2\x00B2\u025C\x00B2\u0270\u0272\u0260\u0274" + "\u0276\u0276\u0276\u0278\u0276\u027A\u027C\u027E\u0280\u0280\u0280\x0010\u0282" + "\u0284\u0282\u0284\x0010\x0010\x0010\x0010\u0286\u0288\u0288\u028A\u028C\u028C\u028E" + "\x0010\u0290\u0292\x0010\u0294\u0296\x0010\u0298\u029A\x0010\x0010\x0010\x0010\x0010\u029C" + "\u0296\x0010\x0010\x0010\x0010\u029E\u027C\u027C\u02A0\x00F2\x00F2\u027C\u027C\u027C\u02A2" + "\x00F2\x0048\x0048\x0048\u02A4\u02A6\u02A8\u02AA\u02AA\u02AA\u02AA\u02AA\u02A4\u02A6" + "\u0226\x00DC\x00DC\x00DC\x00DC\x00DC\x00DC\u02AC\x00F2\x003A\x003A\x003A\x003A\x003A\x003A\x003A\x003A\x003A" + "\x003A\x003A\x003A\x003A\u02AE\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00EE\x00EE\x00EE" + "\x00EE\x00EE\x00EE\u02B0\u0130\u02B2\u0130\u02B2\x00EE\x00EE\x00EE\x00EE\x00EE\u0192\x00F2\x00F2" + "\x00F2\x00F2\x00F2\x00F2\x00F2\u016A\u02B4\u016A\u02B6\u016A\u02B8\u0116\x0088\u0116\u02BA" + "\u02B6\u016A\u02BC\u0116\u0116\u016A\u016A\u016A\u02B4\u02BE\u02B4\u020C\u0116" + "\u02C0\u0116\u02C2\x0090\x0094\x00DA\u016A\x0088\u0116\x001E\u0162\u02BC\x0088\x0088\u02C4" + "\u016A\u02C6\x0050\x0050\x0050\x0050\x0050\x0050\x0050\x0050\u02C8\u02C8\u02C8\u02C8\u02C8" + "\u02C8\u02CA\u02CA\u02CC\u02CC\u02CC\u02CC\u02CC\u02CC\u02CE\u02CE\u02D0\u02D2" + "\u02D4\u02D0\u02D6\x00F2\x00F2\x00F2\u0162\u0162\u02D8\u016A\u016A\u0162\u016A\u016A" + "\u02D8\u02C4\u016A\u02D8\u016A\u016A\u016A\u02D8\u016A\u016A\u016A\u016A\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u0162\u016A\u02D8" + "\u02D8\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u016A\u016A\u0162\u0162\u0162\u0162\u0162\u0162\u02DA\u02DC\x001E\u0162" + "\u02DC\u02DC\u02DC\u0162\u02DA\u02DE\u02DA\x001E\u0162\u02DC\u02DC\u02DA\u02DC" + "\x001E\x001E\x001E\u0162\u02DA\u02DC\u02DC\u02DC\u02DC\u0162\u0162\u02DA\u02DA\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\x001E\u0162\u0162\u02DC\u02DC\u0162" + "\u0162\u0162\u0162\u02DA\x001E\x001E\u02DC\u02DC\u02DC\u02DC\u0162\u02DC\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC" + "\x001E\u02DA\u02DC\x001E\u0162\u0162\x001E\u0162\u0162\u0162\u0162\u02DC\u0162\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\x001E\u0162\u0162\u02DC\u0162" + "\u0162\u0162\u0162\u02DA\u02DC\u02DC\u0162\u02DC\u0162\u0162\u02DC\u02DC\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u0162\u02DC\u02DC\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u02DC\u016A\u016A\u016A\u016A\u02DC\u02DC\u016A\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u02DC\u016A\u016A\u016A\u02E0" + "\u02E2\u016A\u016A\u016A\u016A\u016A\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0" + "\u01F0\u02E4\u02D8\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u02E6\u016A\u016A\u02C4\u0162\u0162\u0162\u0162\u0162\u0162\u0162\u0162" + "\u0162\u0162\u0162\u0162\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u0162\u0162" + "\u0162\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\x00F2\x00F2\x00F2\x00F2" + "\x00F2\x00F2\u016A\u016A\u016A\u01D6\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2" + "\x00F2\u016A\u016A\u016A\u016A\u016A\u01D6\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2" + "\x00F2\u02E8\u02E8\u02E8\u02E8\u02E8\u02E8\u02E8\u02E8\u02E8\u02E8\u02EA\u02EA" + "\u02EA\u02EA\u02EA\u02EA\u02EA\u02EA\u02EA\u02EA\u02EC\u02EC\u02EC\u02EC\u02EC" + "\u02EC\u02EC\u02EC\u02EC\u02EC\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u01F0\u01F0\u02EE\u02EE\u02EE\u02EE\u02EE\u02EE\u02EE\u02EE" + "\u02EE\u02EE\u02EE\u02EE\u02EE\u02F0\u02F0\u02F0\u02F0\u02F0\u02F0\u02F0\u02F0" + "\u02F0\u02F0\u02F0\u02F0\u02F0\u02F2\u02F4\u02F4\u02F4\u02F4\u02F6\u02F8\u02F8" + "\u02F8\u02F8\u02FA\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u02C4\u016A\u016A\u016A\u016A\u02C4\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u0162\u0162\u0162\u0162\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u02C4\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u02E4\u016A\u016A\u016A\u016A" + "\u016A\u016A\u016A\u016A\u016A\u02FC\u016A\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\x0012" + "\x0012\x0012\x0012\x0012\x0012\x0012\u02FE\u02FE\u02FE\u02FE\u02FE\u02E8\u02E8\u02E8\u02E8" + "\u02E8\u0300\u0300\u0300\u0300\u0300\u016A\u016A\u016A\u016A\u016A\u016A\x001E" + "\u02DA\u0302\u0304\u02DC\u02DA\u02DC\u0162\u0162\u02DA\u02DC\x001E\u0162\u0162" + "\u02DC\x001E\u0162\u02DC\u02DC\x0012\x0012\x0012\x0012\x0012\u0162\u0162\u0162\u0162\u0162" + "\u0162\u0162\u0162\u0162\u0162\u0162\u0162\u0162\u0162\u0162\u0162\u0162\u0162" + "\u0162\u0162\u0162\u0162\u0162\u0162\u0162\u02A6\u0306\u0306\u0306\u0306\u0306" + "\u0306\u0306\u0306\u0306\u0306\u0304\u02DA\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u0162\u0162\u0162\u0162\x001E\u0162\u0162\u0162\u02DC" + "\u02DC\u02DC\u0162\u02DA\u0162\u0162\u02DC\u02DC\x001E\u02DC\u0162\x0012\x0012\x001E" + "\u0162\u02DA\u02DA\u02DC\u0162\u02DC\u0162\u0162\u0162\u0162\u0162\u02DC\u02DC" + "\u02DC\u0162\x0012\u0162\u0162\u0162\u0162\u0162\u0162\u02DC\u02DC\u02DC\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u02DC\x001E\u02DC\u02DC\u0162\x001E\x001E\u02DA\u02DA\u02DC" + "\x001E\u0162\u0162\u02DC\u0162\u0162\u0162\u02DC\x001E\u0162\u0162\u0162\u0162" + "\u0162\u0162\u0162\u0162\u0162\u0162\u0162\u02DA\x001E\u0162\u0162\u0162\u0162" + "\u0162\u02DC\u0162\u0162\u02DC\u02DC\u02DA\x001E\u02DA\x001E\u0162\u02DA\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u0162\u02DC\u02DC\u02DC\u02DC\u02DA" + "\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC" + "\u02DC\u02DC\u02DC\u02DC\u02DC\u02DC\x001E\u0162\u0162\x001E\x001E\u0162\u02DC\u02DC" + "\x001E\u0162\u0162\u02DC\x001E\u0162\u02DA\u0162\u02DA\u02DC\u02DC\u02DA\u0162" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u0162\u0162\u0162\u0162\u0162" + "\u0162\u0162\u0162\u0162\u0162\u02D8\u02C4\u0162\u0162\u0308\x00F2\u016A\u016A" + "\u016A\u016A\u016A\x00F2\x00F2\x00F2\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138" + "\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138\u0138" + "\u0138\u0138\u013A\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142" + "\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142\u0142" + "\u030A\x0060\u020C\x00AA\x00AC\x0066\x0066\u030C\u020C\u0244\x0060\x0064\x0074\x0088\x0088\x00DC" + "\u020C\x0060\x0060\u030E\u016A\u016A\u0310\x0066\u0312\x00EE\x0060\x00F2\x00F2\u0314\x0010" + "\u0316\x0010\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2\x00B2" + "\x00B2\x00B2\x00B2\u0318\x00F2\x00F2\u0318\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x00F2\x00F2\x00F2\u013C\u01E4\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\u014C\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01BC\x00F2\x00F2\x00F2\x00F2\x0094\x0094\x0094\u01BC\x0094" + "\x0094\x0094\u01BC\x0094\x0094\x0094\u01BC\x0094\x0094\x0094\u01BC\u014E\u014E\u014E\u014E" + "\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\u014E\x0010" + "\u031A\u031A\x0010\u0290\u0292\u031A\x0010\x0010\x0010\x0010\u031C\x0010\u022E\u031A\x0010" + "\u031A\x0012\x0012\x0012\x0012\x0010\x0010\u031E\x0010\x0010\x0010\x0010\x0010\u0280\x00F2\x00F2\u016A" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u02FC" + "\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\x00F2" + "\x00F2\x00F2\x00F2\x00F2\x00F2\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A" + "\u016A\u016A\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\u016A\u016A" + "\u016A\u016A\u016A\u016A\x00F2\x00F2\x000A\x0010\u0320\u0322\x0012\x0012\x0012\x0012\x0012\u016A" + "\x0012\x0012\x0012\x0012\u0324\u0326\u0328\u032A\u032A\u032A\u032A\x00EE\x00EE\u01F6\u032C" + "\x00E2\x00E2\u016A\u032E\u0330\u0332\u016A\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\u01BC\u014C\u0334\u0336\u0212\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\u0332\x00E2\u0212\x00F2\x00F2\u01B8\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x00F2\u01B8\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01BC\u01F0\u0338\u0338\u01F0" + "\u01F0\u01F0\u01F0\u01F0\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\u01BC\x00F2\x00F2\u016A\u016A\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u02E4\u01D6\u033A\u033A\u033A\u033A\u033A\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u033C" + "\u033E\u01C2\u01C2\u0340\u0342\u0342\u0342\u0342\u0342\x0050\x0050\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u016A" + "\u02E6\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u0344\x0050\x0050\x0050\x0050" + "\x0050\x0050\x0050\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u016A\u016A\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01FA\u01F0\u01F0\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u02E4\u016A\u02E6\u01F0\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0" + "\u016A\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0\u01F0" + "\u01F0\u01F0\u01F0\u02E4\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346" + "\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346" + "\u0346\u0346\u0346\u0346\u0346\x00F2\x00F2\x00F2\x00F2\x00F2\u0346\u0346\u0346\u0346" + "\u0346\u0346\u0348\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\u0234\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01BC" + "\x00F2\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u016A\u01D6" + "\x00F2\x00F2\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u034A" + "\x0010\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01F4\u01F4\u01F4\u01F4\u01F4\x0094\x00F2" + "\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0060\x0060\x0060\x0060\x0060\x0060\x0060\u01B2\u0130" + "\u034C\u014E\u014E\u014E\u014E\x00EE\u031E\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060" + "\x0060\x0060\x0060\x00F2\x00F2\x00F2\u01BA\x0094\x0094\x0094\u034E\u034E\u034E\u034E\u0350\x00EE" + "\u013E\u013E\u013E\x00F2\x00F2\x00F2\x00F2\x00E4\x00E4\x00E4\x00E4\x00E4\x00E4\x00E4\x00E4\x00E4\x00E4" + "\x00E4\u0352\x00E6\x00E6\x00E6\x00E6\x00E4\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0088\x0060\x0060\x0060" + "\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\x0060\u023C\x0088\x0088\x0088\x0064\x0066" + "\u030C\x0060\x0060\x0060\x0060\x0060\u0354\u0356\u030C\u0358\x0060\x0060\x00F2\x00F2\x00F2\x00F2" + "\x00F2\x00F2\x0060\x0060\x0060\x0060\x0060\u035A\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2" + "\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00DC\x0090\x0094\x0094\x0094\u01AA" + "\x0094\u01AA\x0094\u01B2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01FE\u01AE" + "\u01A8\u016A\u016A\x00F2\x00F2\u01C2\u01C2\u01C2\u01F0\u035C\x00F2\x00F2\x00F2\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0010\x0010\x00F2\x00F2\x00F2\x00F2\u01AC\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\u01AC\u01AC\u01AC\u01AC\u01AC\u01AC\u01AC\u01AC\u0192\x00F2\x00F2" + "\x00F2\x00F2\u013E\u01EA\u01EA\u01EA\u01EA\u01EA\x00F2\x00F2\x00F2\x00EE\x00EE\x00EE\x00EE\x00EE" + "\x00EE\x00EE\x00EE\x00EE\x0094\x0094\x0094\u013E\u0220\x00F2\x00F2\u01F4\u01F4\u01F4\u01F4\u01F4" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u014E\u014E\u01A0" + "\x00EE\u013E\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01C8\u014E\u014E\u014E" + "\u014E\u014E\u0202\x00F2\x00F2\x00F2\x00F2\x00F2\u0146\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\u01B2\u01AC\u014E\u014E\u01AC\u01A8\u01AC\u035E\u013E\u013E\u013E\u013E" + "\u013E\u013E\u013C\u01EA\u01EA\u01EA\u01EA\u01EA\x00F2\x00F2\u013E\x0094\x0094\x0094" + "\x0094\u01C8\u014E\u014E\u01A8\u01AE\u01A8\u01AE\u01A6\x00F2\x00F2\x00F2\x00F2\x0094\u01C8" + "\x0094\x0094\x0094\x0094\u01A8\x00F2\u01EA\u01EA\u01EA\u01EA\u01EA\x00F2\u013E\u013E\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u0212\x0094\x0094\u01EE\u01F0\u020A\x00F2\x00F2\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\u01CE\u014E\u01CE\u01C8\u01CE\x0094\x0094\u01A0\u01B2" + "\u01BC\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\u01B8\u0234\u013E\x0094\x0094" + "\x0094\x0094\x0094\u01FE\u014E\u01AC\u013E\u0234\u0360\u0192\x00F2\x00F2\x00F2\x00F2\u01B8" + "\x0094\x0094\u01BC\u01B8\x0094\x0094\u01BC\u01B8\x0094\x0094\u01BC\x00F2\x00F2\x00F2\x00F2\x0094" + "\x0094\x0094\u01BC\x0094\x0094\x0094\u01BC\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\u01FE" + "\u01AE\u01AC\u01A8\u0362\u0208\x00F2\u01EA\u01EA\u01EA\u01EA\u01EA\x00F2\x00F2\x00F2" + "\x0094\x0094\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\u01BC\x00F2\u01B8\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x00F2\x00F2\u0364\u0364\u0364\u0364\u0364" + "\u0364\u0364\u0364\u0364\u0364\u0364\u0364\u0364\u0364\u0364\u0364\u0366\u0366" + "\u0366\u0366\u0366\u0366\u0366\u0366\u0366\u0366\u0366\u0366\u0366\u0366\u0366" + "\u0366\u0346\u0346\u0346\u0346\u0346\u0368\u0346\u0346\u0346\u036A\u0346\u0346" + "\u036C\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346" + "\u036E\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346" + "\u0346\u0346\u0370\u0372\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346" + "\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0374\u0346" + "\u0346\u0346\u0346\u0346\u0346\u0346\u0346\x00F2\u0346\u0346\u0346\u0346\u0346" + "\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346\u0346" + "\u0346\u0346\u0346\x00F2\x00F2\x00F2\x00B2\x00B2\x00B2\u025C\x00F2\x00F2\x00F2\x00F2\x00F2\u0318" + "\x00B2\x00B2\x00F2\x00F2\u0376\u0378\u0156\u0156\u0156\u0156\u037A\u0156\u0156\u0156" + "\u0156\u0156\u0156\u0158\u0156\u0156\u0158\u0158\u0156\u0376\u0158\u0156\u0156" + "\u0156\u0156\u0156\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170" + "\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u037C\u037C\u037C\u037C\u037C\u037C" + "\u037C\u037C\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\u0194\u0170\u0170\u0170\u0170" + "\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170" + "\u0170\u0170\u0170\u0170\u037E\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\u0170\u0170" + "\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170" + "\u0170\x00F2\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170" + "\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2\x00F2" + "\x00F2\u0170\u0170\u0170\u0170\u0170\u0170\u0380\x00F2\x00EE\x00EE\x00EE\x00EE\x00EE\x00EE" + "\x00EE\x00EE\x0010\x0010\x0010\u0382\u0384\x00F2\x00F2\x00F2\x00EE\x00EE\x00EE\u0192\x00F2\x00F2\x00F2" + "\x00F2\u031C\u0386\u0388\u038A\u038A\u038A\u038A\u038A\u038A\u038A\u0384\u0382" + "\u0384\x0010\u0294\u038C\x001C\u038E\u0390\x0010\u0392\u0306\u0306\u0394\x0010\u0396" + "\u02DC\u0308\u0398\u028E\x00F2\x00F2\u0170\u0170\u01A4\u0170\u0170\u0170\u0170" + "\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170\u0170" + "\u0170\u0170\u01A4\u039A\u0314\x000C\x000E\x0010\x0012\x0014\x0016\x0018\x001A\x001A\x001A\x001A" + "\x001A\x001C\x001E\x0020\x002C\x002E\x002E\x002E\x002E\x002E\x002E\x002E\x002E\x002E\x002E\x002E\x002E\x0030\x0032" + "\u02A6\u029A\x0012\x0010\x0094\x0094\x0094\x0094\x0094\u0212\x0094\x0094\x0094\x0094\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x00E2\x0094\x0094\x0094" + "\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\x0094\u01BC\x00F2\x0094\x0094\x0094\x00F2" + "\x0094\x0094\x0094\x00F2\x0094\x0094\x0094\x00F2\x0094\u01BC\x00F2\x003A\u039C\u01D4\u02AE\u02C4" + "\u0162\u02D8\u01D6\x00F2\x00F2\x00F2\x00F2\u039E\u03A0\u016A\x00F2").ToCharArray();

	  // The A table has 930 entries for a total of 3720 bytes.

	  internal static readonly int[] A = new int[930];
	  internal const String A_DATA = "\u4800\u100F\u4800\u100F\u4800\u100F\u5800\u400F\u5000\u400F\u5800\u400F\u6000" + "\u400F\u5000\u400F\u5000\u400F\u5000\u400F\u6000\u400C\u6800\x0018\u6800\x0018" + "\u2800\x0018\u2800\u601A\u2800\x0018\u6800\x0018\u6800\x0018\uE800\x0015\uE800\x0016\u6800" + "\x0018\u2000\x0019\u3800\x0018\u2000\x0014\u3800\x0018\u3800\x0018\u1800\u3609\u1800\u3609" + "\u3800\x0018\u6800\x0018\uE800\x0019\u6800\x0019\uE800\x0019\u6800\x0018\u6800\x0018\x0082" + "\u7FE1\x0082\u7FE1\x0082\u7FE1\x0082\u7FE1\uE800\x0015\u6800\x0018\uE800\x0016\u6800\x001B" + "\u6800\u5017\u6800\x001B\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\x0081\u7FE2\uE800\x0015\u6800" + "\x0019\uE800\x0016\u6800\x0019\u4800\u100F\u4800\u100F\u5000\u100F\u3800\x000C\u6800" + "\x0018\u2800\u601A\u2800\u601A\u6800\x001C\u6800\x0018\u6800\x001B\u6800\x001C\x0000\u7005" + "\uE800\x001D\u6800\x0019\u4800\u1010\u6800\x001C\u6800\x001B\u2800\x001C\u2800\x0019\u1800" + "\u060B\u1800\u060B\u6800\x001B\u07FD\u7002\u6800\x001B\u1800\u050B\x0000\u7005\uE800" + "\x001E\u6800\u080B\u6800\u080B\u6800\u080B\u6800\x0018\x0082\u7001\x0082\u7001\x0082" + "\u7001\u6800\x0019\x0082\u7001\u07FD\u7002\x0081\u7002\x0081\u7002\x0081\u7002\u6800" + "\x0019\x0081\u7002\u061D\u7002\x0006\u7001\x0005\u7002\u07FF\uF001\u03A1\u7002\x0000" + "\u7002\x0006\u7001\x0005\u7002\x0006\u7001\x0005\u7002\u07FD\u7002\u061E\u7001\x0006" + "\u7001\u04F5\u7002\u034A\u7001\u033A\u7001\x0006\u7001\x0005\u7002\u0336\u7001" + "\u0336\u7001\x0006\u7001\x0005\u7002\x0000\u7002\u013E\u7001\u032A\u7001\u032E\u7001" + "\x0006\u7001\u033E\u7001\u067D\u7002\u034E\u7001\u0346\u7001\u0575\u7002\x0000" + "\u7002\u034E\u7001\u0356\u7001\u05F9\u7002\u035A\u7001\u036A\u7001\x0006\u7001" + "\x0005\u7002\u036A\u7001\x0000\u7002\x0000\u7002\x0005\u7002\u0366\u7001\u0366\u7001" + "\x0006\u7001\x0005\u7002\u036E\u7001\x0000\u7002\x0000\u7005\x0000\u7002\u0721\u7002" + "\x0000\u7005\x0000\u7005\x000A\uF001\x0007\uF003\x0009\uF002\x000A\uF001\x0007\uF003\x0009" + "\uF002\x0009\uF002\x0006\u7001\x0005\u7002\u013D\u7002\u07FD\u7002\x000A\uF001\u067E" + "\u7001\u0722\u7001\u05FA\u7001\x0000\u7002\u07FE\u7001\x0006\u7001\x0005\u7002\u0576" + "\u7001\u07FE\u7001\u07FD\u7002\u07FD\u7002\x0006\u7001\x0005\u7002\u04F6\u7001" + "\u0116\u7001\u011E\u7001\u07FD\u7002\u07FD\u7002\u07FD\u7002\u0349\u7002\u0339" + "\u7002\x0000\u7002\u0335\u7002\u0335\u7002\x0000\u7002\u0329\u7002\x0000\u7002\u032D" + "\u7002\u0335\u7002\x0000\u7002\x0000\u7002\u033D\u7002\x0000\u7002\u07FD\u7002\u07FD" + "\u7002\x0000\u7002\u0345\u7002\u034D\u7002\x0000\u7002\u034D\u7002\u0355\u7002" + "\x0000\u7002\x0000\u7002\u0359\u7002\u0369\u7002\x0000\u7002\x0000\u7002\u0369\u7002" + "\u0369\u7002\u0115\u7002\u0365\u7002\u0365\u7002\u011D\u7002\x0000\u7002\u036D" + "\u7002\x0000\u7002\x0000\u7005\x0000\u7002\x0000\u7004\x0000\u7004\x0000\u7004\u6800\u7004" + "\u6800\u7004\x0000\u7004\x0000\u7004\x0000\u7004\u6800\x001B\u6800\x001B\u6800\u7004" + "\u6800\u7004\x0000\u7004\u6800\x001B\u6800\u7004\u6800\x001B\x0000\u7004\u6800\x001B" + "\u4000\u3006\u4000\u3006\u4000\u3006\u46B1\u3006\u7800\x0000\u7800\x0000\x0000\u7004" + "\u05F9\u7002\u05F9\u7002\u05F9\u7002\u6800\x0018\u7800\x0000\x009A\u7001\u6800\x0018" + "\x0096\u7001\x0096\u7001\x0096\u7001\u7800\x0000\u0102\u7001\u7800\x0000\x00FE\u7001\x00FE" + "\u7001\u07FD\u7002\x0082\u7001\u7800\x0000\x0082\u7001\x0099\u7002\x0095\u7002\x0095\u7002" + "\x0095\u7002\u07FD\u7002\x0081\u7002\x007D\u7002\x0081\u7002\u0101\u7002\x00FD\u7002" + "\x00FD\u7002\x0022\u7001\x00F9\u7002\x00E5\u7002\x0000\u7001\x0000\u7001\x0000\u7001\x00BD" + "\u7002\x00D9\u7002\x0021\u7002\u0159\u7002\u0141\u7002\u07E5\u7002\x0000\u7002\u0712" + "\u7001\u0181\u7002\u6800\x0019\x0006\u7001\x0005\u7002\u07E6\u7001\x0000\u7002\u05FA" + "\u7001\u05FA\u7001\u05FA\u7001\u0142\u7001\u0142\u7001\u0141\u7002\u0141\u7002" + "\x0000\x001C\u4000\u3006\u4000\x0007\u4000\x0007\x003E\u7001\x0006\u7001\x0005\u7002\x003D" + "\u7002\u7800\x0000\x00C2\u7001\x00C2\u7001\x00C2\u7001\x00C2\u7001\u7800\x0000\u7800\x0000" + "\x0000\u7004\x0000\x0018\x0000\x0018\u7800\x0000\x00C1\u7002\x00C1\u7002\x00C1\u7002\x00C1\u7002" + "\u07FD\u7002\u7800\x0000\x0000\x0018\u6800\x0014\u7800\x0000\u7800\x0000\u2800\u601A\u7800" + "\x0000\u4000\u3006\u4000\u3006\u4000\u3006\u0800\x0014\u4000\u3006\u0800\x0018\u4000" + "\u3006\u4000\u3006\u0800\x0018\u0800\u7005\u0800\u7005\u0800\u7005\u7800\x0000" + "\u0800\u7005\u0800\x0018\u0800\x0018\u7800\x0000\u3000\u1010\u3000\u1010\u3000\u1010" + "\u7800\x0000\u6800\x0019\u6800\x0019\u1000\x0019\u2800\x0018\u2800\x0018\u1000\u601A\u3800" + "\x0018\u1000\x0018\u6800\x001C\u6800\x001C\u4000\u3006\u1000\x0018\u1000\x0018\u1000\x0018" + "\u1000\u7005\u1000\u7005\u1000\u7004\u1000\u7005\u1000\u7005\u4000\u3006\u4000" + "\u3006\u4000\u3006\u3000\u3409\u3000\u3409\u2800\x0018\u3000\x0018\u3000\x0018\u1000" + "\x0018\u4000\u3006\u1000\u7005\u1000\x0018\u1000\u7005\u4000\u3006\u3000\u1010" + "\u6800\x001C\u4000\u3006\u4000\u3006\u1000\u7004\u1000\u7004\u4000\u3006\u4000" + "\u3006\u6800\x001C\u1000\u7005\u1000\x001C\u1000\x001C\u1000\u7005\u7800\x0000\u1000" + "\u1010\u4000\u3006\u7800\x0000\u7800\x0000\u1000\u7005\u0800\u3409\u0800\u3409" + "\u0800\u7005\u4000\u3006\u0800\u7004\u0800\u7004\u0800\u7004\u7800\x0000\u0800" + "\u7004\u4000\u3006\u4000\u3006\u4000\u3006\u0800\x0018\u0800\x0018\u1000\u7005" + "\u7800\x0000\u4000\u3006\u7800\x0000\u4000\u3006\x0000\u3008\u4000\u3006\x0000\u7005" + "\x0000\u3008\x0000\u3008\x0000\u3008\u4000\u3006\x0000\u3008\u4000\u3006\x0000\u7005" + "\u4000\u3006\x0000\u3749\x0000\u3749\x0000\x0018\x0000\u7004\u7800\x0000\x0000\u7005\u7800" + "\x0000\u4000\u3006\x0000\u7005\u7800\x0000\u7800\x0000\x0000\u3008\x0000\u3008\u7800\x0000" + "\x0000\u080B\x0000\u080B\x0000\u080B\x0000\u06EB\x0000\x001C\u2800\u601A\x0000\u7005\u4000" + "\u3006\x0000\x0018\u2800\u601A\x0000\x001C\x0000\u7005\u4000\u3006\x0000\u7005\x0000\u074B" + "\x0000\u080B\x0000\u080B\u6800\x001C\u6800\x001C\u2800\u601A\u6800\x001C\u7800\x0000\u6800" + "\u050B\u6800\u050B\u6800\u04AB\u6800\u04AB\u6800\u04AB\x0000\x001C\x0000\u3008\x0000" + "\u3006\x0000\u3006\x0000\u3008\u7800\x0000\x0000\x001C\x0000\x0018\u7800\x0000\x0000\u7004\u4000" + "\u3006\u4000\u3006\x0000\x0018\x0000\u3609\x0000\u3609\x0000\u7004\u7800\x0000\x0000\u7005" + "\x0000\x001C\x0000\x001C\x0000\x001C\x0000\x0018\x0000\x001C\x0000\u3409\x0000\u3409\x0000\u3008\x0000" + "\u3008\u4000\u3006\x0000\x001C\x0000\x001C\u7800\x0000\x0000\x001C\x0000\x0018\x0000\u7005\x0000" + "\u3008\u4000\u3006\x0000\u3008\x0000\u3008\x0000\u3008\x0000\u3008\x0000\u7005\u4000" + "\u3006\x0000\u3008\x0000\u3008\u4000\u3006\x0000\u7005\x0000\u3008\u07FE\u7001\u07FE" + "\u7001\u7800\x0000\u07FE\u7001\x0000\u7005\x0000\x0018\x0000\u7004\x0000\u7005\x0000\x0018" + "\x0000\u070B\x0000\u070B\x0000\u070B\x0000\u070B\x0000\u042B\x0000\u054B\x0000\u080B\x0000" + "\u080B\u7800\x0000\u6800\x0014\x0000\u7005\x0000\x0018\x0000\u7005\u6000\u400C\x0000\u7005" + "\x0000\u7005\uE800\x0015\uE800\x0016\u7800\x0000\x0000\u746A\x0000\u746A\x0000\u746A\u7800" + "\x0000\u6800\u060B\u6800\u060B\u6800\x0014\u6800\x0018\u6800\x0018\u4000\u3006\u6000" + "\u400C\u7800\x0000\x0000\u7005\x0000\u7004\x0000\u3008\x0000\u7005\x0000\u04EB\u7800\x0000" + "\u4000\u3006\x0000\u3008\x0000\u7004\x0000\u7002\x0000\u7004\u07FD\u7002\x0000\u7002" + "\x0000\u7004\u07FD\u7002\x00ED\u7002\u07FE\u7001\x0000\u7002\u07E1\u7002\u07E1\u7002" + "\u07E2\u7001\u07E2\u7001\u07FD\u7002\u07E1\u7002\u7800\x0000\u07E2\u7001\u06D9" + "\u7002\u06D9\u7002\u06A9\u7002\u06A9\u7002\u0671\u7002\u0671\u7002\u0601\u7002" + "\u0601\u7002\u0641\u7002\u0641\u7002\u0609\u7002\u0609\u7002\u07FF\uF003\u07FF" + "\uF003\u07FD\u7002\u7800\x0000\u06DA\u7001\u06DA\u7001\u07FF\uF003\u6800\x001B" + "\u07FD\u7002\u6800\x001B\u06AA\u7001\u06AA\u7001\u0672\u7001\u0672\u7001\u7800" + "\x0000\u6800\x001B\u07FD\u7002\u07E5\u7002\u0642\u7001\u0642\u7001\u07E6\u7001" + "\u6800\x001B\u0602\u7001\u0602\u7001\u060A\u7001\u060A\u7001\u6800\x001B\u7800" + "\x0000\u6000\u400C\u6000\u400C\u6000\u400C\u6000\x000C\u6000\u400C\u4800\u1010" + "\u4800\u1010\u4800\u1010\x0000\u1010\u0800\u1010\u6800\x0014\u6800\x0014\u6800\x001D" + "\u6800\x001E\u6800\x0015\u6800\x001D\u6000\u400D\u5000\u400E\u7800\u1010\u7800\u1010" + "\u7800\u1010\u3800\x000C\u2800\x0018\u2800\x0018\u2800\x0018\u6800\x0018\u6800\x0018\uE800" + "\x001D\uE800\x001E\u6800\x0018\u6800\x0018\u6800\u5017\u6800\u5017\u6800\x0018\u3800" + "\x0019\uE800\x0015\uE800\x0016\u6800\x0018\u6800\x0019\u6800\x0018\u6800\x0018\u6000\u400C" + "\u4800\u1010\u7800\x0000\u1800\u060B\x0000\u7004\u2000\x0019\u2000\x0019\u6800\x0019" + "\uE800\x0015\uE800\x0016\x0000\u7004\u1800\u040B\u1800\u040B\x0000\u7004\u7800\x0000" + "\u2800\u601A\u7800\x0000\u4000\u3006\u4000\x0007\u4000\x0007\u4000\u3006\x0000\u7001" + "\u6800\x001C\u6800\x001C\x0000\u7001\x0000\u7002\x0000\u7001\x0000\u7001\x0000\u7002\u6800" + "\x0019\x0000\u7001\u07FE\u7001\u6800\x001C\u2800\x001C\x0000\u7002\x0072\u7001\x0000\u7001" + "\u6800\x001C\u6800\x0019\x0071\u7002\x0000\x001C\x0042\u742A\x0042\u742A\x0042\u780A\x0042\u780A" + "\x0041\u762A\x0041\u762A\x0041\u780A\x0041\u780A\x0000\u780A\x0000\u780A\x0000\u780A\x0006" + "\u7001\x0005\u7002\x0000\u742A\x0000\u780A\u6800\u06EB\u6800\x0019\u6800\x001C\u6800" + "\x0019\uE800\x0019\uE800\x0019\uE800\x0019\u2000\x0019\u2800\x0019\u6800\x001C\uE800\x0015" + "\uE800\x0016\u6800\x001C\x0000\x001C\u6800\x001C\u6800\x001C\x0000\x001C\u6800\u042B\u6800" + "\u042B\u6800\u05AB\u6800\u05AB\u1800\u072B\u1800\u072B\x006A\x001C\x006A\x001C\x0069" + "\x001C\x0069\x001C\u6800\u06CB\u6800\u040B\u6800\u040B\u6800\u040B\u6800\u040B\u6800" + "\u058B\u6800\u058B\u6800\u058B\u6800\u058B\u6800\u042B\u7800\x0000\u6800\x001C" + "\u6800\u056B\u6800\u056B\u6800\u06EB\u6800\u06EB\uE800\x0019\uE800\x0015\uE800" + "\x0016\u6800\x0019\uE800\x0016\uE800\x0015\u6800\x0019\u7800\x0000\x00C1\u7002\u7800\x0000" + "\x0005\u7002\u07FE\u7001\x0000\u7002\u6800\x001C\u6800\x001C\x0006\u7001\x0005\u7002\u4000" + "\u3006\u7800\x0000\u6800\x0018\u6800\x0018\u6800\u080B\u7800\x0000\u07FD\u7002\uE800" + "\x001D\uE800\x001E\u6800\x0018\u6800\x0014\u6800\x0018\u6800\u7004\u6800\x001C\x0000\u7004" + "\x0000\u7005\x0000\u772A\u6800\x0014\u6800\x0015\u6800\x0016\u6800\x0016\u6800\x001C\x0000" + "\u740A\x0000\u740A\x0000\u740A\u6800\x0014\x0000\u7004\x0000\u764A\x0000\u776A\x0000\u748A" + "\x0000\u7004\x0000\u7005\u6800\x0018\u4000\u3006\u6800\x001B\u6800\x001B\x0000\u7004\x0000" + "\u05EB\x0000\u05EB\x0000\u042B\x0000\u042B\x0000\u044B\x0000\u056B\x0000\u068B\x0000\u080B" + "\u6800\x001C\u6800\u048B\u6800\u048B\u6800\u048B\x0000\x001C\u6800\u080B\x0000\u7005" + "\x0000\u7005\x0000\u7005\u7800\x0000\x0000\u7004\u6800\x0018\u4000\x0007\u6800\x0018\x0000" + "\u776A\x0000\u776A\x0000\u776A\x0000\u762A\u6800\x001B\u6800\u7004\u6800\u7004\x0000" + "\x001B\x0000\x001B\x0006\u7001\x0000\u7002\u7800\x0000\u07FE\u7001\u7800\x0000\u2800\u601A" + "\u2800\x001C\x0000\u3008\x0000\x0018\x0000\u7004\x0000\u3008\x0000\u3008\x0000\x0018\x0000\x0013" + "\x0000\x0013\x0000\x0012\x0000\x0012\x0000\u7005\x0000\u7705\x0000\u7005\x0000\u76E5\x0000\u7545" + "\x0000\u7005\x0000\u75C5\x0000\u7005\x0000\u7005\x0000\u76A5\x0000\u7005\x0000\u7665\x0000" + "\u7005\x0000\u75A5\u7800\x0000\u0800\u7005\u4000\u3006\u0800\u7005\u0800\u7005" + "\u2000\x0019\u1000\x001B\u1000\x001B\u6800\x0015\u6800\x0016\u1000\u601A\u6800\x001C\u6800" + "\x0018\u6800\x0015\u6800\x0016\u6800\x0018\u6800\x0014\u6800\u5017\u6800\u5017\u6800" + "\x0015\u6800\x0016\u6800\x0015\u6800\u5017\u6800\u5017\u3800\x0018\u7800\x0000\u6800" + "\x0018\u3800\x0018\u6800\x0014\uE800\x0015\uE800\x0016\u2800\x0018\u2000\x0019\u2000\x0014" + "\u6800\x0018\u2800\u601A\u7800\x0000\u4800\u1010\u6800\x0019\u6800\x001B\u7800\x0000" + "\u6800\u1010\u6800\u1010\u6800\u1010";

	  // The B table has 930 entries for a total of 1860 bytes.

	  internal static readonly char[] B = ("\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0001\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0001\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0001\x0001\x0001\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0001\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0005\x0000\x0000\x0001\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0004\x0004\x0000\x0004\x0000\x0004\x0004\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0004\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0004\x0000\x0004\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0004\x0000\x0000\x0000\x0004\x0000\x0000\x0000\x0004\x0000\x0000\x0004\x0004\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0004\x0004\x0000" + "\x0000\x0000\x0000\x0000\x0004\x0000\x0004\x0004\x0000\x0000\x0004\x0004\x0004\x0004\x0004\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0004\x0000\x0000\x0000\x0004\x0004\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0004\x0000\x0000\x0000\x0000\x0004\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0004\x0004\x0004\x0004\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0004\x0000" + "\x0004\x0004\x0000\x0000\x0000\x0004\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0004\x0000\x0000\x0000" + "\x0000\x0000\x0001\x0000\x0001\x0000\x0000\x0001\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0001\x0000\x0000\x0000\x0000\x0000\x0001\x0000\x0000" + "\x0001\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0002\x0002\x0002\x0002\x0001\x0001\x0001\x0001\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0006\x0006\x0005\x0005\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0010\x0010\x0000\x0000\x0000\x0000\x0000\x0010\x0010\x0010\x0000\x0000\x0010\x0010\x0010" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0010\x0010\x0010\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0004\x0004\x0000\x0000\x0000\x0000\x0000\x0010\x0010" + "\x0010\x0010\x0010\x0010\x0010\x0010\x0010\x0010\x0010\x0010\x0010\x0010\x0000\x0000\x0004\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000" + "\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000\x0000").ToCharArray();

	  // In all, the character property tables require 19144 bytes.

		static CharacterData00()
		{
				CharMap = new char[][][] {new char[][] {new char[] {'\u00DF'}, new char[] {'\u0053', '\u0053'}}, new char[][] {new char[] {'\u0130'}, new char[] {'\u0130'}}, new char[][] {new char[] {'\u0149'}, new char[] {'\u02BC', '\u004E'}}, new char[][] {new char[] {'\u01F0'}, new char[] {'\u004A', '\u030C'}}, new char[][] {new char[] {'\u0390'}, new char[] {'\u0399', '\u0308', '\u0301'}}, new char[][] {new char[] {'\u03B0'}, new char[] {'\u03A5', '\u0308', '\u0301'}}, new char[][] {new char[] {'\u0587'}, new char[] {'\u0535', '\u0552'}}, new char[][] {new char[] {'\u1E96'}, new char[] {'\u0048', '\u0331'}}, new char[][] {new char[] {'\u1E97'}, new char[] {'\u0054', '\u0308'}}, new char[][] {new char[] {'\u1E98'}, new char[] {'\u0057', '\u030A'}}, new char[][] {new char[] {'\u1E99'}, new char[] {'\u0059', '\u030A'}}, new char[][] {new char[] {'\u1E9A'}, new char[] {'\u0041', '\u02BE'}}, new char[][] {new char[] {'\u1F50'}, new char[] {'\u03A5', '\u0313'}}, new char[][] {new char[] {'\u1F52'}, new char[] {'\u03A5', '\u0313', '\u0300'}}, new char[][] {new char[] {'\u1F54'}, new char[] {'\u03A5', '\u0313', '\u0301'}}, new char[][] {new char[] {'\u1F56'}, new char[] {'\u03A5', '\u0313', '\u0342'}}, new char[][] {new char[] {'\u1F80'}, new char[] {'\u1F08', '\u0399'}}, new char[][] {new char[] {'\u1F81'}, new char[] {'\u1F09', '\u0399'}}, new char[][] {new char[] {'\u1F82'}, new char[] {'\u1F0A', '\u0399'}}, new char[][] {new char[] {'\u1F83'}, new char[] {'\u1F0B', '\u0399'}}, new char[][] {new char[] {'\u1F84'}, new char[] {'\u1F0C', '\u0399'}}, new char[][] {new char[] {'\u1F85'}, new char[] {'\u1F0D', '\u0399'}}, new char[][] {new char[] {'\u1F86'}, new char[] {'\u1F0E', '\u0399'}}, new char[][] {new char[] {'\u1F87'}, new char[] {'\u1F0F', '\u0399'}}, new char[][] {new char[] {'\u1F88'}, new char[] {'\u1F08', '\u0399'}}, new char[][] {new char[] {'\u1F89'}, new char[] {'\u1F09', '\u0399'}}, new char[][] {new char[] {'\u1F8A'}, new char[] {'\u1F0A', '\u0399'}}, new char[][] {new char[] {'\u1F8B'}, new char[] {'\u1F0B', '\u0399'}}, new char[][] {new char[] {'\u1F8C'}, new char[] {'\u1F0C', '\u0399'}}, new char[][] {new char[] {'\u1F8D'}, new char[] {'\u1F0D', '\u0399'}}, new char[][] {new char[] {'\u1F8E'}, new char[] {'\u1F0E', '\u0399'}}, new char[][] {new char[] {'\u1F8F'}, new char[] {'\u1F0F', '\u0399'}}, new char[][] {new char[] {'\u1F90'}, new char[] {'\u1F28', '\u0399'}}, new char[][] {new char[] {'\u1F91'}, new char[] {'\u1F29', '\u0399'}}, new char[][] {new char[] {'\u1F92'}, new char[] {'\u1F2A', '\u0399'}}, new char[][] {new char[] {'\u1F93'}, new char[] {'\u1F2B', '\u0399'}}, new char[][] {new char[] {'\u1F94'}, new char[] {'\u1F2C', '\u0399'}}, new char[][] {new char[] {'\u1F95'}, new char[] {'\u1F2D', '\u0399'}}, new char[][] {new char[] {'\u1F96'}, new char[] {'\u1F2E', '\u0399'}}, new char[][] {new char[] {'\u1F97'}, new char[] {'\u1F2F', '\u0399'}}, new char[][] {new char[] {'\u1F98'}, new char[] {'\u1F28', '\u0399'}}, new char[][] {new char[] {'\u1F99'}, new char[] {'\u1F29', '\u0399'}}, new char[][] {new char[] {'\u1F9A'}, new char[] {'\u1F2A', '\u0399'}}, new char[][] {new char[] {'\u1F9B'}, new char[] {'\u1F2B', '\u0399'}}, new char[][] {new char[] {'\u1F9C'}, new char[] {'\u1F2C', '\u0399'}}, new char[][] {new char[] {'\u1F9D'}, new char[] {'\u1F2D', '\u0399'}}, new char[][] {new char[] {'\u1F9E'}, new char[] {'\u1F2E', '\u0399'}}, new char[][] {new char[] {'\u1F9F'}, new char[] {'\u1F2F', '\u0399'}}, new char[][] {new char[] {'\u1FA0'}, new char[] {'\u1F68', '\u0399'}}, new char[][] {new char[] {'\u1FA1'}, new char[] {'\u1F69', '\u0399'}}, new char[][] {new char[] {'\u1FA2'}, new char[] {'\u1F6A', '\u0399'}}, new char[][] {new char[] {'\u1FA3'}, new char[] {'\u1F6B', '\u0399'}}, new char[][] {new char[] {'\u1FA4'}, new char[] {'\u1F6C', '\u0399'}}, new char[][] {new char[] {'\u1FA5'}, new char[] {'\u1F6D', '\u0399'}}, new char[][] {new char[] {'\u1FA6'}, new char[] {'\u1F6E', '\u0399'}}, new char[][] {new char[] {'\u1FA7'}, new char[] {'\u1F6F', '\u0399'}}, new char[][] {new char[] {'\u1FA8'}, new char[] {'\u1F68', '\u0399'}}, new char[][] {new char[] {'\u1FA9'}, new char[] {'\u1F69', '\u0399'}}, new char[][] {new char[] {'\u1FAA'}, new char[] {'\u1F6A', '\u0399'}}, new char[][] {new char[] {'\u1FAB'}, new char[] {'\u1F6B', '\u0399'}}, new char[][] {new char[] {'\u1FAC'}, new char[] {'\u1F6C', '\u0399'}}, new char[][] {new char[] {'\u1FAD'}, new char[] {'\u1F6D', '\u0399'}}, new char[][] {new char[] {'\u1FAE'}, new char[] {'\u1F6E', '\u0399'}}, new char[][] {new char[] {'\u1FAF'}, new char[] {'\u1F6F', '\u0399'}}, new char[][] {new char[] {'\u1FB2'}, new char[] {'\u1FBA', '\u0399'}}, new char[][] {new char[] {'\u1FB3'}, new char[] {'\u0391', '\u0399'}}, new char[][] {new char[] {'\u1FB4'}, new char[] {'\u0386', '\u0399'}}, new char[][] {new char[] {'\u1FB6'}, new char[] {'\u0391', '\u0342'}}, new char[][] {new char[] {'\u1FB7'}, new char[] {'\u0391', '\u0342', '\u0399'}}, new char[][] {new char[] {'\u1FBC'}, new char[] {'\u0391', '\u0399'}}, new char[][] {new char[] {'\u1FC2'}, new char[] {'\u1FCA', '\u0399'}}, new char[][] {new char[] {'\u1FC3'}, new char[] {'\u0397', '\u0399'}}, new char[][] {new char[] {'\u1FC4'}, new char[] {'\u0389', '\u0399'}}, new char[][] {new char[] {'\u1FC6'}, new char[] {'\u0397', '\u0342'}}, new char[][] {new char[] {'\u1FC7'}, new char[] {'\u0397', '\u0342', '\u0399'}}, new char[][] {new char[] {'\u1FCC'}, new char[] {'\u0397', '\u0399'}}, new char[][] {new char[] {'\u1FD2'}, new char[] {'\u0399', '\u0308', '\u0300'}}, new char[][] {new char[] {'\u1FD3'}, new char[] {'\u0399', '\u0308', '\u0301'}}, new char[][] {new char[] {'\u1FD6'}, new char[] {'\u0399', '\u0342'}}, new char[][] {new char[] {'\u1FD7'}, new char[] {'\u0399', '\u0308', '\u0342'}}, new char[][] {new char[] {'\u1FE2'}, new char[] {'\u03A5', '\u0308', '\u0300'}}, new char[][] {new char[] {'\u1FE3'}, new char[] {'\u03A5', '\u0308', '\u0301'}}, new char[][] {new char[] {'\u1FE4'}, new char[] {'\u03A1', '\u0313'}}, new char[][] {new char[] {'\u1FE6'}, new char[] {'\u03A5', '\u0342'}}, new char[][] {new char[] {'\u1FE7'}, new char[] {'\u03A5', '\u0308', '\u0342'}}, new char[][] {new char[] {'\u1FF2'}, new char[] {'\u1FFA', '\u0399'}}, new char[][] {new char[] {'\u1FF3'}, new char[] {'\u03A9', '\u0399'}}, new char[][] {new char[] {'\u1FF4'}, new char[] {'\u038F', '\u0399'}}, new char[][] {new char[] {'\u1FF6'}, new char[] {'\u03A9', '\u0342'}}, new char[][] {new char[] {'\u1FF7'}, new char[] {'\u03A9', '\u0342', '\u0399'}}, new char[][] {new char[] {'\u1FFC'}, new char[] {'\u03A9', '\u0399'}}, new char[][] {new char[] {'\uFB00'}, new char[] {'\u0046', '\u0046'}}, new char[][] {new char[] {'\uFB01'}, new char[] {'\u0046', '\u0049'}}, new char[][] {new char[] {'\uFB02'}, new char[] {'\u0046', '\u004C'}}, new char[][] {new char[] {'\uFB03'}, new char[] {'\u0046', '\u0046', '\u0049'}}, new char[][] {new char[] {'\uFB04'}, new char[] {'\u0046', '\u0046', '\u004C'}}, new char[][] {new char[] {'\uFB05'}, new char[] {'\u0053', '\u0054'}}, new char[][] {new char[] {'\uFB06'}, new char[] {'\u0053', '\u0054'}}, new char[][] {new char[] {'\uFB13'}, new char[] {'\u0544', '\u0546'}}, new char[][] {new char[] {'\uFB14'}, new char[] {'\u0544', '\u0535'}}, new char[][] {new char[] {'\uFB15'}, new char[] {'\u0544', '\u053B'}}, new char[][] {new char[] {'\uFB16'}, new char[] {'\u054E', '\u0546'}}, new char[][] {new char[] {'\uFB17'}, new char[] {'\u0544', '\u053D'}}};
				{ // THIS CODE WAS AUTOMATICALLY CREATED BY GenerateCharacter:
				char[] data = A_DATA.ToCharArray();
				assert(data.Length == (930 * 2));
				int i = 0, j = 0;
				while (i < (930 * 2))
				{
					int entry = data[i++] << 16;
					A[j++] = entry | data[i++];
				}
				}

		}
	}

}