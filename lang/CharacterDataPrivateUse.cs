﻿/*
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

	internal class CharacterDataPrivateUse : CharacterData
	{

		internal override int GetProperties(int ch)
		{
			return 0;
		}

		internal override int GetType(int ch)
		{
		return (ch & 0xFFFE) == 0xFFFE ? Character.UNASSIGNED : Character.PRIVATE_USE;
		}

		internal override bool IsJavaIdentifierStart(int ch)
		{
			return false;
		}

		internal override bool IsJavaIdentifierPart(int ch)
		{
			return false;
		}

		internal override bool IsUnicodeIdentifierStart(int ch)
		{
			return false;
		}

		internal override bool IsUnicodeIdentifierPart(int ch)
		{
			return false;
		}

		internal override bool IsIdentifierIgnorable(int ch)
		{
			return false;
		}

		internal override int ToLowerCase(int ch)
		{
			return ch;
		}

		internal override int ToUpperCase(int ch)
		{
			return ch;
		}

		internal override int ToTitleCase(int ch)
		{
			return ch;
		}

		internal override int Digit(int ch, int radix)
		{
			return -1;
		}

		internal override int GetNumericValue(int ch)
		{
			return -1;
		}

		internal override bool IsWhitespace(int ch)
		{
			return false;
		}

		internal override sbyte GetDirectionality(int ch)
		{
		return (ch & 0xFFFE) == 0xFFFE ? Character.DIRECTIONALITY_UNDEFINED : Character.DIRECTIONALITY_LEFT_TO_RIGHT;
		}

		internal override bool IsMirrored(int ch)
		{
			return false;
		}

		internal static readonly CharacterData Instance = new CharacterDataPrivateUse();
		private CharacterDataPrivateUse()
		{
		};
	}



}