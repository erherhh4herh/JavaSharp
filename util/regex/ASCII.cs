/*
 * Copyright (c) 1999, 2000, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.regex
{


	/// <summary>
	/// Utility class that implements the standard C ctype functionality.
	/// 
	/// @author Hong Zhang
	/// </summary>

	internal sealed class ASCII
	{

		internal const int UPPER = 0x00000100;

		internal const int LOWER = 0x00000200;

		internal const int DIGIT = 0x00000400;

		internal const int SPACE = 0x00000800;

		internal const int PUNCT = 0x00001000;

		internal const int CNTRL = 0x00002000;

		internal const int BLANK = 0x00004000;

		internal const int HEX = 0x00008000;

		internal const int UNDER = 0x00010000;

		internal const int ASCII = 0x0000FF00;

		internal static readonly int ALPHA = (UPPER | LOWER);

		internal static readonly int ALNUM = (UPPER | LOWER | DIGIT);

		internal static readonly int GRAPH = (PUNCT | UPPER | LOWER | DIGIT);

		internal static readonly int WORD = (UPPER | LOWER | UNDER | DIGIT);

		internal static readonly int XDIGIT = (HEX);

		private static readonly int[] Ctype = new int[] {CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, SPACE + CNTRL + BLANK, SPACE + CNTRL, SPACE + CNTRL, SPACE + CNTRL, SPACE + CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, SPACE + BLANK, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, DIGIT + HEX + 0, DIGIT + HEX + 1, DIGIT + HEX + 2, DIGIT + HEX + 3, DIGIT + HEX + 4, DIGIT + HEX + 5, DIGIT + HEX + 6, DIGIT + HEX + 7, DIGIT + HEX + 8, DIGIT + HEX + 9, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, UPPER + HEX + 10, UPPER + HEX + 11, UPPER + HEX + 12, UPPER + HEX + 13, UPPER + HEX + 14, UPPER + HEX + 15, UPPER + 16, UPPER + 17, UPPER + 18, UPPER + 19, UPPER + 20, UPPER + 21, UPPER + 22, UPPER + 23, UPPER + 24, UPPER + 25, UPPER + 26, UPPER + 27, UPPER + 28, UPPER + 29, UPPER + 30, UPPER + 31, UPPER + 32, UPPER + 33, UPPER + 34, UPPER + 35, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT | UNDER, PUNCT, LOWER + HEX + 10, LOWER + HEX + 11, LOWER + HEX + 12, LOWER + HEX + 13, LOWER + HEX + 14, LOWER + HEX + 15, LOWER + 16, LOWER + 17, LOWER + 18, LOWER + 19, LOWER + 20, LOWER + 21, LOWER + 22, LOWER + 23, LOWER + 24, LOWER + 25, LOWER + 26, LOWER + 27, LOWER + 28, LOWER + 29, LOWER + 30, LOWER + 31, LOWER + 32, LOWER + 33, LOWER + 34, LOWER + 35, PUNCT, PUNCT, PUNCT, PUNCT, CNTRL};

		internal static int GetType(int ch)
		{
			return ((ch & 0xFFFFFF80) == 0 ? Ctype[ch] : 0);
		}

		internal static bool IsType(int ch, int type)
		{
			return (GetType(ch) & type) != 0;
		}

		internal static bool IsAscii(int ch)
		{
			return ((ch & 0xFFFFFF80) == 0);
		}

		internal static bool IsAlpha(int ch)
		{
			return IsType(ch, ALPHA);
		}

		internal static bool IsDigit(int ch)
		{
			return ((ch - '0') | ('9' - ch)) >= 0;
		}

		internal static bool IsAlnum(int ch)
		{
			return IsType(ch, ALNUM);
		}

		internal static bool IsGraph(int ch)
		{
			return IsType(ch, GRAPH);
		}

		internal static bool IsPrint(int ch)
		{
			return ((ch - 0x20) | (0x7E - ch)) >= 0;
		}

		internal static bool IsPunct(int ch)
		{
			return IsType(ch, PUNCT);
		}

		internal static bool IsSpace(int ch)
		{
			return IsType(ch, SPACE);
		}

		internal static bool IsHexDigit(int ch)
		{
			return IsType(ch, HEX);
		}

		internal static bool IsOctDigit(int ch)
		{
			return ((ch - '0') | ('7' - ch)) >= 0;
		}

		internal static bool IsCntrl(int ch)
		{
			return IsType(ch, CNTRL);
		}

		internal static bool IsLower(int ch)
		{
			return ((ch - 'a') | ('z' - ch)) >= 0;
		}

		internal static bool IsUpper(int ch)
		{
			return ((ch - 'A') | ('Z' - ch)) >= 0;
		}

		internal static bool IsWord(int ch)
		{
			return IsType(ch, WORD);
		}

		internal static int ToDigit(int ch)
		{
			return (Ctype[ch & 0x7F] & 0x3F);
		}

		internal static int ToLower(int ch)
		{
			return IsUpper(ch) ? (ch + 0x20) : ch;
		}

		internal static int ToUpper(int ch)
		{
			return IsLower(ch) ? (ch - 0x20) : ch;
		}

	}

}