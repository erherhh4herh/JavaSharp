using System;

/*
 * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.prefs
{

	/// <summary>
	/// Static methods for translating Base64 encoded strings to byte arrays
	/// and vice-versa.
	/// 
	/// @author  Josh Bloch </summary>
	/// <seealso cref=     Preferences
	/// @since   1.4 </seealso>
	internal class Base64
	{
		/// <summary>
		/// Translates the specified byte array into a Base64 string as per
		/// Preferences.put(byte[]).
		/// </summary>
		internal static String ByteArrayToBase64(sbyte[] a)
		{
			return ByteArrayToBase64(a, false);
		}

		/// <summary>
		/// Translates the specified byte array into an "alternate representation"
		/// Base64 string.  This non-standard variant uses an alphabet that does
		/// not contain the uppercase alphabetic characters, which makes it
		/// suitable for use in situations where case-folding occurs.
		/// </summary>
		internal static String ByteArrayToAltBase64(sbyte[] a)
		{
			return ByteArrayToBase64(a, true);
		}

		private static String ByteArrayToBase64(sbyte[] a, bool alternate)
		{
			int aLen = a.Length;
			int numFullGroups = aLen / 3;
			int numBytesInPartialGroup = aLen - 3 * numFullGroups;
			int resultLen = 4 * ((aLen + 2) / 3);
			StringBuffer result = new StringBuffer(resultLen);
			char[] intToAlpha = (alternate ? IntToAltBase64 : IntToBase64);

			// Translate all full groups from byte array elements to Base64
			int inCursor = 0;
			for (int i = 0; i < numFullGroups; i++)
			{
				int byte0 = a[inCursor++] & 0xff;
				int byte1 = a[inCursor++] & 0xff;
				int byte2 = a[inCursor++] & 0xff;
				result.Append(intToAlpha[byte0 >> 2]);
				result.Append(intToAlpha[(byte0 << 4) & 0x3f | (byte1 >> 4)]);
				result.Append(intToAlpha[(byte1 << 2) & 0x3f | (byte2 >> 6)]);
				result.Append(intToAlpha[byte2 & 0x3f]);
			}

			// Translate partial group if present
			if (numBytesInPartialGroup != 0)
			{
				int byte0 = a[inCursor++] & 0xff;
				result.Append(intToAlpha[byte0 >> 2]);
				if (numBytesInPartialGroup == 1)
				{
					result.Append(intToAlpha[(byte0 << 4) & 0x3f]);
					result.Append("==");
				}
				else
				{
					// assert numBytesInPartialGroup == 2;
					int byte1 = a[inCursor++] & 0xff;
					result.Append(intToAlpha[(byte0 << 4) & 0x3f | (byte1 >> 4)]);
					result.Append(intToAlpha[(byte1 << 2) & 0x3f]);
					result.Append('=');
				}
			}
			// assert inCursor == a.length;
			// assert result.length() == resultLen;
			return result.ToString();
		}

		/// <summary>
		/// This array is a lookup table that translates 6-bit positive integer
		/// index values into their "Base64 Alphabet" equivalents as specified
		/// in Table 1 of RFC 2045.
		/// </summary>
		private static readonly char[] IntToBase64 = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'};

		/// <summary>
		/// This array is a lookup table that translates 6-bit positive integer
		/// index values into their "Alternate Base64 Alphabet" equivalents.
		/// This is NOT the real Base64 Alphabet as per in Table 1 of RFC 2045.
		/// This alternate alphabet does not use the capital letters.  It is
		/// designed for use in environments where "case folding" occurs.
		/// </summary>
		private static readonly char[] IntToAltBase64 = new char[] {'!', '"', '#', '$', '%', '&', '\'', '(', ')', ',', '-', '.', ':', ';', '<', '>', '@', '[', ']', '^', '`', '_', '{', '|', '}', '~', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '?'};

		/// <summary>
		/// Translates the specified Base64 string (as per Preferences.get(byte[]))
		/// into a byte array.
		/// 
		/// @throw IllegalArgumentException if <tt>s</tt> is not a valid Base64
		///        string.
		/// </summary>
		internal static sbyte[] Base64ToByteArray(String s)
		{
			return Base64ToByteArray(s, false);
		}

		/// <summary>
		/// Translates the specified "alternate representation" Base64 string
		/// into a byte array.
		/// 
		/// @throw IllegalArgumentException or ArrayOutOfBoundsException
		///        if <tt>s</tt> is not a valid alternate representation
		///        Base64 string.
		/// </summary>
		internal static sbyte[] AltBase64ToByteArray(String s)
		{
			return Base64ToByteArray(s, true);
		}

		private static sbyte[] Base64ToByteArray(String s, bool alternate)
		{
			sbyte[] alphaToInt = (alternate ? AltBase64ToInt : Base64ToInt);
			int sLen = s.Length();
			int numGroups = sLen / 4;
			if (4 * numGroups != sLen)
			{
				throw new IllegalArgumentException("String length must be a multiple of four.");
			}
			int missingBytesInLastGroup = 0;
			int numFullGroups = numGroups;
			if (sLen != 0)
			{
				if (s.CharAt(sLen - 1) == '=')
				{
					missingBytesInLastGroup++;
					numFullGroups--;
				}
				if (s.CharAt(sLen - 2) == '=')
				{
					missingBytesInLastGroup++;
				}
			}
			sbyte[] result = new sbyte[3 * numGroups - missingBytesInLastGroup];

			// Translate all full groups from base64 to byte array elements
			int inCursor = 0, outCursor = 0;
			for (int i = 0; i < numFullGroups; i++)
			{
				int ch0 = Base64toInt(s.CharAt(inCursor++), alphaToInt);
				int ch1 = Base64toInt(s.CharAt(inCursor++), alphaToInt);
				int ch2 = Base64toInt(s.CharAt(inCursor++), alphaToInt);
				int ch3 = Base64toInt(s.CharAt(inCursor++), alphaToInt);
				result[outCursor++] = (sbyte)((ch0 << 2) | (ch1 >> 4));
				result[outCursor++] = (sbyte)((ch1 << 4) | (ch2 >> 2));
				result[outCursor++] = (sbyte)((ch2 << 6) | ch3);
			}

			// Translate partial group, if present
			if (missingBytesInLastGroup != 0)
			{
				int ch0 = Base64toInt(s.CharAt(inCursor++), alphaToInt);
				int ch1 = Base64toInt(s.CharAt(inCursor++), alphaToInt);
				result[outCursor++] = (sbyte)((ch0 << 2) | (ch1 >> 4));

				if (missingBytesInLastGroup == 1)
				{
					int ch2 = Base64toInt(s.CharAt(inCursor++), alphaToInt);
					result[outCursor++] = (sbyte)((ch1 << 4) | (ch2 >> 2));
				}
			}
			// assert inCursor == s.length()-missingBytesInLastGroup;
			// assert outCursor == result.length;
			return result;
		}

		/// <summary>
		/// Translates the specified character, which is assumed to be in the
		/// "Base 64 Alphabet" into its equivalent 6-bit positive integer.
		/// 
		/// @throw IllegalArgumentException or ArrayOutOfBoundsException if
		///        c is not in the Base64 Alphabet.
		/// </summary>
		private static int Base64toInt(char c, sbyte[] alphaToInt)
		{
			int result = alphaToInt[c];
			if (result < 0)
			{
				throw new IllegalArgumentException("Illegal character " + c);
			}
			return result;
		}

		/// <summary>
		/// This array is a lookup table that translates unicode characters
		/// drawn from the "Base64 Alphabet" (as specified in Table 1 of RFC 2045)
		/// into their 6-bit positive integer equivalents.  Characters that
		/// are not in the Base64 alphabet but fall within the bounds of the
		/// array are translated to -1.
		/// </summary>
		private static readonly sbyte[] Base64ToInt = new sbyte[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1, -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51};

		/// <summary>
		/// This array is the analogue of base64ToInt, but for the nonstandard
		/// variant that avoids the use of uppercase alphabetic characters.
		/// </summary>
		private static readonly sbyte[] AltBase64ToInt = new sbyte[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, -1, 62, 9, 10, 11, -1, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 12, 13, 14, -1, 15, 63, 16, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 17, -1, 18, 19, 21, 20, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 22, 23, 24, 25};

		public static void Main(String[] args)
		{
			int numRuns = Convert.ToInt32(args[0]);
			int numBytes = Convert.ToInt32(args[1]);
			Random rnd = new Random();
			for (int i = 0; i < numRuns; i++)
			{
				for (int j = 0; j < numBytes; j++)
				{
					sbyte[] arr = new sbyte[j];
					for (int k = 0; k < j; k++)
					{
						arr[k] = (sbyte)rnd.NextInt();
					}

					String s = ByteArrayToBase64(arr);
					sbyte[] b = Base64ToByteArray(s);
					if (!System.Array.Equals(arr, b))
					{
						System.Console.WriteLine("Dismal failure!");
					}

					s = ByteArrayToAltBase64(arr);
					b = AltBase64ToByteArray(s);
					if (!System.Array.Equals(arr, b))
					{
						System.Console.WriteLine("Alternate dismal failure!");
					}
				}
			}
		}
	}

}