/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// Checked exception thrown when an input character (or byte) sequence
	/// is valid but cannot be mapped to an output byte (or character)
	/// sequence.
	/// 
	/// @since 1.4
	/// </summary>

	public class UnmappableCharacterException : CharacterCodingException
	{

		private new const long SerialVersionUID = -7026962371537706123L;

		private int InputLength_Renamed;

		/// <summary>
		/// Constructs an {@code UnmappableCharacterException} with the
		/// given length. </summary>
		/// <param name="inputLength"> the length of the input </param>
		public UnmappableCharacterException(int inputLength)
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