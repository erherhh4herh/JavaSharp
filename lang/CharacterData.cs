/*
 * Copyright (c) 2006, 2011, Oracle and/or its affiliates. All rights reserved.
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

	internal abstract class CharacterData
	{
		internal abstract int GetProperties(int ch);
		internal abstract int GetType(int ch);
		internal abstract bool IsWhitespace(int ch);
		internal abstract bool IsMirrored(int ch);
		internal abstract bool IsJavaIdentifierStart(int ch);
		internal abstract bool IsJavaIdentifierPart(int ch);
		internal abstract bool IsUnicodeIdentifierStart(int ch);
		internal abstract bool IsUnicodeIdentifierPart(int ch);
		internal abstract bool IsIdentifierIgnorable(int ch);
		internal abstract int ToLowerCase(int ch);
		internal abstract int ToUpperCase(int ch);
		internal abstract int ToTitleCase(int ch);
		internal abstract int Digit(int ch, int radix);
		internal abstract int GetNumericValue(int ch);
		internal abstract sbyte GetDirectionality(int ch);

		//need to implement for JSR204
		internal virtual int ToUpperCaseEx(int ch)
		{
			return ToUpperCase(ch);
		}

		internal virtual char[] ToUpperCaseCharArray(int ch)
		{
			return null;
		}

		internal virtual bool IsOtherLowercase(int ch)
		{
			return false;
		}

		internal virtual bool IsOtherUppercase(int ch)
		{
			return false;
		}

		internal virtual bool IsOtherAlphabetic(int ch)
		{
			return false;
		}

		internal virtual bool IsIdeographic(int ch)
		{
			return false;
		}

		// Character <= 0xff (basic latin) is handled by internal fast-path
		// to avoid initializing large tables.
		// Note: performance of this "fast-path" code may be sub-optimal
		// in negative cases for some accessors due to complicated ranges.
		// Should revisit after optimization of table initialization.

		internal static CharacterData Of(int ch)
		{
			if ((int)((uint)ch >> 8) == 0) // fast-path
			{
				return CharacterDataLatin1.Instance;
			}
			else
			{
				switch ((int)((uint)ch >> 16)) //plane 00-16
				{
				case(0):
					return CharacterData00.Instance;
				case(1):
					return CharacterData01.Instance;
				case(2):
					return CharacterData02.Instance;
				case(14):
					return CharacterData0E.Instance;
				case(15): // Private Use
				case(16): // Private Use
					return CharacterDataPrivateUse.Instance;
				default:
					return CharacterDataUndefined.Instance;
				}
			}
		}
	}

}