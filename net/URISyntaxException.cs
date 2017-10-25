using System;

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

namespace java.net
{


	/// <summary>
	/// Checked exception thrown to indicate that a string could not be parsed as a
	/// URI reference.
	/// 
	/// @author Mark Reinhold </summary>
	/// <seealso cref= URI
	/// @since 1.4 </seealso>

	public class URISyntaxException : Exception
	{
		private new const long SerialVersionUID = 2137979680897488891L;

		private String Input_Renamed;
		private int Index_Renamed;

		/// <summary>
		/// Constructs an instance from the given input string, reason, and error
		/// index.
		/// </summary>
		/// <param name="input">   The input string </param>
		/// <param name="reason">  A string explaining why the input could not be parsed </param>
		/// <param name="index">   The index at which the parse error occurred,
		///                 or {@code -1} if the index is not known
		/// </param>
		/// <exception cref="NullPointerException">
		///          If either the input or reason strings are {@code null}
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the error index is less than {@code -1} </exception>
		public URISyntaxException(String input, String reason, int index) : base(reason)
		{
			if ((input == null) || (reason == null))
			{
				throw new NullPointerException();
			}
			if (index < -1)
			{
				throw new IllegalArgumentException();
			}
			this.Input_Renamed = input;
			this.Index_Renamed = index;
		}

		/// <summary>
		/// Constructs an instance from the given input string and reason.  The
		/// resulting object will have an error index of {@code -1}.
		/// </summary>
		/// <param name="input">   The input string </param>
		/// <param name="reason">  A string explaining why the input could not be parsed
		/// </param>
		/// <exception cref="NullPointerException">
		///          If either the input or reason strings are {@code null} </exception>
		public URISyntaxException(String input, String reason) : this(input, reason, -1)
		{
		}

		/// <summary>
		/// Returns the input string.
		/// </summary>
		/// <returns>  The input string </returns>
		public virtual String Input
		{
			get
			{
				return Input_Renamed;
			}
		}

		/// <summary>
		/// Returns a string explaining why the input string could not be parsed.
		/// </summary>
		/// <returns>  The reason string </returns>
		public virtual String Reason
		{
			get
			{
				return base.Message;
			}
		}

		/// <summary>
		/// Returns an index into the input string of the position at which the
		/// parse error occurred, or {@code -1} if this position is not known.
		/// </summary>
		/// <returns>  The error index </returns>
		public virtual int Index
		{
			get
			{
				return Index_Renamed;
			}
		}

		/// <summary>
		/// Returns a string describing the parse error.  The resulting string
		/// consists of the reason string followed by a colon character
		/// ({@code ':'}), a space, and the input string.  If the error index is
		/// defined then the string {@code " at index "} followed by the index, in
		/// decimal, is inserted after the reason string and before the colon
		/// character.
		/// </summary>
		/// <returns>  A string describing the parse error </returns>
		public override String Message
		{
			get
			{
				StringBuffer sb = new StringBuffer();
				sb.Append(Reason);
				if (Index_Renamed > -1)
				{
					sb.Append(" at index ");
					sb.Append(Index_Renamed);
				}
				sb.Append(": ");
				sb.Append(Input_Renamed);
				return sb.ToString();
			}
		}

	}

}