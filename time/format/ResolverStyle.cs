using System.Collections.Generic;

/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
 * Copyright (c) 2008-2013, Stephen Colebourne & Michael Nascimento Santos
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
	/// Enumeration of different ways to resolve dates and times.
	/// <para>
	/// Parsing a text string occurs in two phases.
	/// Phase 1 is a basic text parse according to the fields added to the builder.
	/// Phase 2 resolves the parsed field-value pairs into date and/or time objects.
	/// This style is used to control how phase 2, resolving, happens.
	/// 
	/// @implSpec
	/// This is an immutable and thread-safe enum.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
	public sealed class ResolverStyle
	{

		/// <summary>
		/// Style to resolve dates and times strictly.
		/// <para>
		/// Using strict resolution will ensure that all parsed values are within
		/// the outer range of valid values for the field. Individual fields may
		/// be further processed for strictness.
		/// </para>
		/// <para>
		/// For example, resolving year-month and day-of-month in the ISO calendar
		/// system using strict mode will ensure that the day-of-month is valid
		/// for the year-month, rejecting invalid values.
		/// </para>
		/// </summary>
		STRICT,
		public static readonly ResolverStyle STRICT = new ResolverStyle("STRICT", InnerEnum.STRICT);
		/// <summary>
		/// Style to resolve dates and times in a smart, or intelligent, manner.
		/// <para>
		/// Using smart resolution will perform the sensible default for each
		/// field, which may be the same as strict, the same as lenient, or a third
		/// behavior. Individual fields will interpret this differently.
		/// </para>
		/// <para>
		/// For example, resolving year-month and day-of-month in the ISO calendar
		/// system using smart mode will ensure that the day-of-month is from
		/// 1 to 31, converting any value beyond the last valid day-of-month to be
		/// the last valid day-of-month.
		/// </para>
		/// </summary>
		SMART,
		public static readonly ResolverStyle SMART = new ResolverStyle("SMART", InnerEnum.SMART);
		/// <summary>
		/// Style to resolve dates and times leniently.
		/// <para>
		/// Using lenient resolution will resolve the values in an appropriate
		/// lenient manner. Individual fields will interpret this differently.
		/// </para>
		/// <para>
		/// For example, lenient mode allows the month in the ISO calendar system
		/// to be outside the range 1 to 12.
		/// For example, month 15 is treated as being 3 months after month 12.
		/// </para>
		/// </summary>
		LENIENT
		public static readonly ResolverStyle LENIENT = new ResolverStyle("LENIENT", InnerEnum.LENIENT);

		private static readonly IList<ResolverStyle> valueList = new List<ResolverStyle>();

		static ResolverStyle()
		{
			valueList.Add(STRICT);
			valueList.Add(SMART);
			valueList.Add(LENIENT);
		}

		public enum InnerEnum
		{
			STRICT,
			SMART,
			LENIENT
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;


		public static IList<ResolverStyle> values()
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

		public static ResolverStyle valueOf(string name)
		{
			foreach (ResolverStyle enumInstance in ResolverStyle.values())
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