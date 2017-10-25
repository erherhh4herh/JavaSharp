/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.charset
{


	/// <summary>
	/// Checked exception thrown when an input byte sequence is not legal for given
	/// charset, or an input character sequence is not a legal sixteen-bit Unicode
	/// sequence.
	/// 
	/// @since 1.4
	/// </summary>

	public class MalformedInputException : CharacterCodingException
	{

		private new const long SerialVersionUID = -3438823399834806194L;

		private int InputLength_Renamed;

		/// <summary>
		/// Constructs an {@code MalformedInputException} with the given
		/// length. </summary>
		/// <param name="inputLength"> the length of the input </param>
		public MalformedInputException(int inputLength)
		{
			this.InputLength_Renamed = inputLength;
		}

		/// <summary>
		/// Returns the length of the input. </summary>
		/// <returns> the length of the input </returns>
		public virtual int InputLength
		{
			get
			{
				return InputLength_Renamed;
			}
		}

		/// <summary>
		/// Returns the message. </summary>
		/// <returns> the message </returns>
		public override String Message
		{
			get
			{
				return "Input length = " + InputLength_Renamed;
			}
		}

	}

}