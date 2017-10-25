/*
 * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.regex
{

	using GetPropertyAction = sun.security.action.GetPropertyAction;


	/// <summary>
	/// Unchecked exception thrown to indicate a syntax error in a
	/// regular-expression pattern.
	/// 
	/// @author  unascribed
	/// @since 1.4
	/// @spec JSR-51
	/// </summary>

	public class PatternSyntaxException : IllegalArgumentException
	{
		private new const long SerialVersionUID = -3864639126226059218L;

		private readonly String Desc;
		private readonly String Pattern_Renamed;
		private readonly int Index_Renamed;

		/// <summary>
		/// Constructs a new instance of this class.
		/// </summary>
		/// <param name="desc">
		///         A description of the error
		/// </param>
		/// <param name="regex">
		///         The erroneous pattern
		/// </param>
		/// <param name="index">
		///         The approximate index in the pattern of the error,
		///         or <tt>-1</tt> if the index is not known </param>
		public PatternSyntaxException(String desc, String regex, int index)
		{
			this.Desc = desc;
			this.Pattern_Renamed = regex;
			this.Index_Renamed = index;
		}

		/// <summary>
		/// Retrieves the error index.
		/// </summary>
		/// <returns>  The approximate index in the pattern of the error,
		///         or <tt>-1</tt> if the index is not known </returns>
		public virtual int Index
		{
			get
			{
				return Index_Renamed;
			}
		}

		/// <summary>
		/// Retrieves the description of the error.
		/// </summary>
		/// <returns>  The description of the error </returns>
		public virtual String Description
		{
			get
			{
				return Desc;
			}
		}

		/// <summary>
		/// Retrieves the erroneous regular-expression pattern.
		/// </summary>
		/// <returns>  The erroneous pattern </returns>
		public virtual String Pattern
		{
			get
			{
				return Pattern_Renamed;
			}
		}

		private static readonly String Nl = java.security.AccessController.doPrivileged(new GetPropertyAction("line.separator"));

		/// <summary>
		/// Returns a multi-line string containing the description of the syntax
		/// error and its index, the erroneous regular-expression pattern, and a
		/// visual indication of the error index within the pattern.
		/// </summary>
		/// <returns>  The full detail message </returns>
		public override String Message
		{
			get
			{
				StringBuffer sb = new StringBuffer();
				sb.Append(Desc);
				if (Index_Renamed >= 0)
				{
					sb.Append(" near index ");
					sb.Append(Index_Renamed);
				}
				sb.Append(Nl);
				sb.Append(Pattern_Renamed);
				if (Index_Renamed >= 0)
				{
					sb.Append(Nl);
					for (int i = 0; i < Index_Renamed; i++)
					{
						sb.Append(' ');
					}
					sb.Append('^');
				}
				return sb.ToString();
			}
		}

	}

}