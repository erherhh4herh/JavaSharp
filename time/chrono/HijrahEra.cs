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
 * Copyright (c) 2012, Stephen Colebourne & Michael Nascimento Santos
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
namespace java.time.chrono
{


	/// <summary>
	/// An era in the Hijrah calendar system.
	/// <para>
	/// The Hijrah calendar system has only one era covering the
	/// proleptic years greater than zero.
	/// </para>
	/// <para>
	/// <b>Do not use {@code ordinal()} to obtain the numeric representation of {@code HijrahEra}.
	/// Use {@code getValue()} instead.</b>
	/// 
	/// @implSpec
	/// This is an immutable and thread-safe enum.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: public enum HijrahEra implements Era
	public enum HijrahEra
	{

		/// <summary>
		/// The singleton instance for the current era, 'Anno Hegirae',
		/// which has the numeric value 1.
		/// </summary>
		AH

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code HijrahEra} from an {@code int} value.
		/// <para>
		/// The current era, which is the only accepted value, has the value 1
		/// 
		/// </para>
		/// </summary>
		/// <param name="hijrahEra">  the era to represent, only 1 supported </param>
		/// <returns> the HijrahEra.AH singleton, not null </returns>
		/// <exception cref="DateTimeException"> if the value is invalid </exception>
		public static HijrahEra of(int hijrahEra)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (hijrahEra == 1)
			{
				return AH;
			}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			else
			{
				throw new java.time.DateTimeException("Invalid era: " + hijrahEra);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the numeric era {@code int} value.
		/// <para>
		/// The era AH has the value 1.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the era value, 1 (AH) </returns>
		public int getValue()
		{
			return 1;
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the range of valid values for the specified field.
		/// <para>
		/// The range object expresses the minimum and maximum valid values for a field.
		/// This era is used to enhance the accuracy of the returned range.
		/// If it is not possible to return the range, because the field is not supported
		/// or for some other reason, an exception is thrown.
		/// </para>
		/// <para>
		/// If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		/// The {@code ERA} field returns the range.
		/// All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		/// </para>
		/// <para>
		/// If the field is not a {@code ChronoField}, then the result of this method
		/// is obtained by invoking {@code TemporalField.rangeRefinedBy(TemporalAccessor)}
		/// passing {@code this} as the argument.
		/// Whether the range can be obtained is determined by the field.
		/// </para>
		/// <para>
		/// The {@code ERA} field returns a range for the one valid Hijrah era.
		/// 
		/// </para>
		/// </summary>
		/// <param name="field">  the field to query the range for, not null </param>
		/// <returns> the range of valid values for the field, not null </returns>
		/// <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		/// <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		public java.time.temporal.ValueRange range(java.time.temporal.TemporalField field) // override as super would return range from 0 to 1
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			if (field == ERA)
			{
				return java.time.temporal.ValueRange.of(1, 1);
			}
			return = field
		}

	}

}