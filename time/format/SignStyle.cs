using System.Collections.Generic;

/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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

/*
 *
 *
 *
 *
 *
 * Copyright (c) 2008-2012, Stephen Colebourne & Michael Nascimento Santos
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *
 *  * Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 *  * Neither the name of JSR-310 nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
namespace java.time.format
{

	/// <summary>
	/// Enumeration of ways to handle the positive/negative sign.
	/// <para>
	/// The formatting engine allows the positive and negative signs of numbers
	/// to be controlled using this enum.
	/// See <seealso cref="DateTimeFormatterBuilder"/> for usage.
	/// 
	/// @implSpec
	/// This is an immutable and thread-safe enum.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class SignStyle
	{

		/// <summary>
		/// Style to output the sign only if the value is negative.
		/// <para>
		/// In strict parsing, the negative sign will be accepted and the positive sign rejected.
		/// In lenient parsing, any sign will be accepted.
		/// </para>
		/// </summary>
		NORMAL,
		public static readonly SignStyle NORMAL = new SignStyle("NORMAL", InnerEnum.NORMAL);
		/// <summary>
		/// Style to always output the sign, where zero will output '+'.
		/// <para>
		/// In strict parsing, the absence of a sign will be rejected.
		/// In lenient parsing, any sign will be accepted, with the absence
		/// of a sign treated as a positive number.
		/// </para>
		/// </summary>
		ALWAYS,
		public static readonly SignStyle ALWAYS = new SignStyle("ALWAYS", InnerEnum.ALWAYS);
		/// <summary>
		/// Style to never output sign, only outputting the absolute value.
		/// <para>
		/// In strict parsing, any sign will be rejected.
		/// In lenient parsing, any sign will be accepted unless the width is fixed.
		/// </para>
		/// </summary>
		NEVER,
		public static readonly SignStyle NEVER = new SignStyle("NEVER", InnerEnum.NEVER);
		/// <summary>
		/// Style to block negative values, throwing an exception on printing.
		/// <para>
		/// In strict parsing, any sign will be rejected.
		/// In lenient parsing, any sign will be accepted unless the width is fixed.
		/// </para>
		/// </summary>
		NOT_NEGATIVE,
		public static readonly SignStyle NOT_NEGATIVE = new SignStyle("NOT_NEGATIVE", InnerEnum.NOT_NEGATIVE);
		/// <summary>
		/// Style to always output the sign if the value exceeds the pad width.
		/// A negative value will always output the '-' sign.
		/// <para>
		/// In strict parsing, the sign will be rejected unless the pad width is exceeded.
		/// In lenient parsing, any sign will be accepted, with the absence
		/// of a sign treated as a positive number.
		/// </para>
		/// </summary>
		EXCEEDS_PAD
		public static readonly SignStyle EXCEEDS_PAD = new SignStyle("EXCEEDS_PAD", InnerEnum.EXCEEDS_PAD);

		private static readonly IList<SignStyle> valueList = new List<SignStyle>();

		static SignStyle()
		{
			valueList.Add(NORMAL);
			valueList.Add(ALWAYS);
			valueList.Add(NEVER);
			valueList.Add(NOT_NEGATIVE);
			valueList.Add(EXCEEDS_PAD);
		}

		public enum InnerEnum
		{
			NORMAL,
			ALWAYS,
			NEVER,
			NOT_NEGATIVE,
			EXCEEDS_PAD
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		/// <summary>
		/// Parse helper.
		/// </summary>
		/// <param name="positive">  true if positive sign parsed, false for negative sign </param>
		/// <param name="strict">  true if strict, false if lenient </param>
		/// <param name="fixedWidth">  true if fixed width, false if not
		/// @return </param>
		internal boolean Parse(bool positive, bool strict, bool fixedWidth)
		{
			switch (ordinal())
			{
				case 0: // NORMAL
					// valid if negative or (positive and lenient)
					return !positive || !strict;
				case 1: // ALWAYS
				case 4: // EXCEEDS_PAD
					return true;
				default:
					// valid if lenient and not fixed width
					return !strict && !fixedWidth;
			}
		}


		public static IList<SignStyle> values()
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

		public static SignStyle valueOf(string name)
		{
			foreach (SignStyle enumInstance in SignStyle.values())
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}