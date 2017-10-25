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
	/// Enumeration of the style of a localized date, time or date-time formatter.
	/// <para>
	/// These styles are used when obtaining a date-time style from configuration.
	/// See <seealso cref="DateTimeFormatter"/> and <seealso cref="DateTimeFormatterBuilder"/> for usage.
	/// 
	/// @implSpec
	/// This is an immutable and thread-safe enum.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class FormatStyle
	{
		// ordered from large to small

		/// <summary>
		/// Full text style, with the most detail.
		/// For example, the format might be 'Tuesday, April 12, 1952 AD' or '3:30:42pm PST'.
		/// </summary>
		FULL,
		public static readonly FormatStyle FULL = new FormatStyle("FULL", InnerEnum.FULL);
		/// <summary>
		/// Long text style, with lots of detail.
		/// For example, the format might be 'January 12, 1952'.
		/// </summary>
		LONG,
		public static readonly FormatStyle LONG = new FormatStyle("LONG", InnerEnum.LONG);
		/// <summary>
		/// Medium text style, with some detail.
		/// For example, the format might be 'Jan 12, 1952'.
		/// </summary>
		MEDIUM,
		public static readonly FormatStyle MEDIUM = new FormatStyle("MEDIUM", InnerEnum.MEDIUM);
		/// <summary>
		/// Short text style, typically numeric.
		/// For example, the format might be '12.13.52' or '3:30pm'.
		/// </summary>
		SHORT
		public static readonly FormatStyle SHORT = new FormatStyle("SHORT", InnerEnum.SHORT);

		private static readonly IList<FormatStyle> valueList = new List<FormatStyle>();

		static FormatStyle()
		{
			valueList.Add(FULL);
			valueList.Add(LONG);
			valueList.Add(MEDIUM);
			valueList.Add(SHORT);
		}

		public enum InnerEnum
		{
			FULL,
			LONG,
			MEDIUM,
			SHORT
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;


		public static IList<FormatStyle> values()
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

		public static FormatStyle valueOf(string name)
		{
			foreach (FormatStyle enumInstance in FormatStyle.values())
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