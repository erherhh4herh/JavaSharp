/*
 * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.file
{

	/// <summary>
	/// Unchecked exception thrown when path string cannot be converted into a
	/// <seealso cref="Path"/> because the path string contains invalid characters, or
	/// the path string is invalid for other file system specific reasons.
	/// </summary>

	public class InvalidPathException : IllegalArgumentException
	{
		internal new const long SerialVersionUID = 4355821422286746137L;

		private String Input_Renamed;
		private int Index_Renamed;

		/// <summary>
		/// Constructs an instance from the given input string, reason, and error
		/// index.
		/// </summary>
		/// <param name="input">   the input string </param>
		/// <param name="reason">  a string explaining why the input was rejected </param>
		/// <param name="index">   the index at which the error occurred,
		///                 or <tt>-1</tt> if the index is not known
		/// </param>
		/// <exception cref="NullPointerException">
		///          if either the input or reason strings are <tt>null</tt>
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          if the error index is less than <tt>-1</tt> </exception>
		public InvalidPathException(String input, String reason, int index) : base(reason)
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
		/// resulting object will have an error index of <tt>-1</tt>.
		/// </summary>
		/// <param name="input">   the input string </param>
		/// <param name="reason">  a string explaining why the input was rejected
		/// </param>
		/// <exception cref="NullPointerException">
		///          if either the input or reason strings are <tt>null</tt> </exception>
		public InvalidPathException(String input, String reason) : this(input, reason, -1)
		{
		}

		/// <summary>
		/// Returns the input string.
		/// </summary>
		/// <returns>  the input string </returns>
		public virtual String Input
		{
			get
			{
				return Input_Renamed;
			}
		}

		/// <summary>
		/// Returns a string explaining why the input string was rejected.
		/// </summary>
		/// <returns>  the reason string </returns>
		public virtual String Reason
		{
			get
			{
				return base.Message;
			}
		}

		/// <summary>
		/// Returns an index into the input string of the position at which the
		/// error occurred, or <tt>-1</tt> if this position is not known.
		/// </summary>
		/// <returns>  the error index </returns>
		public virtual int Index
		{
			get
			{
				return Index_Renamed;
			}
		}

		/// <summary>
		/// Returns a string describing the error.  The resulting string
		/// consists of the reason string followed by a colon character
		/// (<tt>':'</tt>), a space, and the input string.  If the error index is
		/// defined then the string <tt>" at index "</tt> followed by the index, in
		/// decimal, is inserted after the reason string and before the colon
		/// character.
		/// </summary>
		/// <returns>  a string describing the error </returns>
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