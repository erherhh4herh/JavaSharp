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
	/// An era in the Thai Buddhist calendar system.
	/// <para>
	/// The Thai Buddhist calendar system has two eras.
	/// The current era, for years from 1 onwards, is known as the 'Buddhist' era.
	/// All previous years, zero or earlier in the proleptic count or one and greater
	/// in the year-of-era count, are part of the 'Before Buddhist' era.
	/// 
	/// <table summary="Buddhist years and eras" cellpadding="2" cellspacing="3" border="0" >
	/// <thead>
	/// <tr class="tableSubHeadingColor">
	/// <th class="colFirst" align="left">year-of-era</th>
	/// <th class="colFirst" align="left">era</th>
	/// <th class="colFirst" align="left">proleptic-year</th>
	/// <th class="colLast" align="left">ISO proleptic-year</th>
	/// </tr>
	/// </thead>
	/// <tbody>
	/// <tr class="rowColor">
	/// <td>2</td><td>BE</td><td>2</td><td>-542</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td>1</td><td>BE</td><td>1</td><td>-543</td>
	/// </tr>
	/// <tr class="rowColor">
	/// <td>1</td><td>BEFORE_BE</td><td>0</td><td>-544</td>
	/// </tr>
	/// <tr class="altColor">
	/// <td>2</td><td>BEFORE_BE</td><td>-1</td><td>-545</td>
	/// </tr>
	/// </tbody>
	/// </table>
	/// </para>
	/// <para>
	/// <b>Do not use {@code ordinal()} to obtain the numeric representation of {@code ThaiBuddhistEra}.
	/// Use {@code getValue()} instead.</b>
	/// 
	/// @implSpec
	/// This is an immutable and thread-safe enum.
	/// 
	/// @since 1.8
	/// </para>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: public enum ThaiBuddhistEra implements Era
	public enum ThaiBuddhistEra
	{

		/// <summary>
		/// The singleton instance for the era before the current one, 'Before Buddhist Era',
		/// which has the numeric value 0.
		/// </summary>
		BEFORE_BE,
		/// <summary>
		/// The singleton instance for the current era, 'Buddhist Era',
		/// which has the numeric value 1.
		/// </summary>
		BE

		//-----------------------------------------------------------------------
		/// <summary>
		/// Obtains an instance of {@code ThaiBuddhistEra} from an {@code int} value.
		/// <para>
		/// {@code ThaiBuddhistEra} is an enum representing the Thai Buddhist eras of BEFORE_BE/BE.
		/// This factory allows the enum to be obtained from the {@code int} value.
		/// 
		/// </para>
		/// </summary>
		/// <param name="thaiBuddhistEra">  the era to represent, from 0 to 1 </param>
		/// <returns> the BuddhistEra singleton, never null </returns>
		/// <exception cref="DateTimeException"> if the era is invalid </exception>
		public static ThaiBuddhistEra of(int thaiBuddhistEra)
		{
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			switch (thaiBuddhistEra)
			{
				case 0:
					return BEFORE_BE;
				case 1:
					return BE;
				default:
					throw new java.time.DateTimeException("Invalid era: " + thaiBuddhistEra);
			}
		}

		//-----------------------------------------------------------------------
		/// <summary>
		/// Gets the numeric era {@code int} value.
		/// <para>
		/// The era BEFORE_BE has the value 0, while the era BE has the value 1.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the era value, from 0 (BEFORE_BE) to 1 (BE) </returns>
		public int getValue()
		{
			return = 
		}

	}

}