using System;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
 *
 *   The original version of this source code and documentation is copyrighted
 * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
 * materials are provided under terms of a License Agreement between Taligent
 * and Sun. This technology is protected by multiple US and International
 * patents. This notice and attribution to Taligent may not be removed.
 *   Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.text
{

	/// <summary>
	/// Signals that an error has been reached unexpectedly
	/// while parsing. </summary>
	/// <seealso cref= java.lang.Exception </seealso>
	/// <seealso cref= java.text.Format </seealso>
	/// <seealso cref= java.text.FieldPosition
	/// @author      Mark Davis </seealso>
	public class ParseException : Exception
	{

		private new const long SerialVersionUID = 2703218443322787634L;

		/// <summary>
		/// Constructs a ParseException with the specified detail message and
		/// offset.
		/// A detail message is a String that describes this particular exception.
		/// </summary>
		/// <param name="s"> the detail message </param>
		/// <param name="errorOffset"> the position where the error is found while parsing. </param>
		public ParseException(String s, int errorOffset) : base(s)
		{
			this.ErrorOffset_Renamed = errorOffset;
		}

		/// <summary>
		/// Returns the position where the error was found.
		/// </summary>
		/// <returns> the position where the error was found </returns>
		public virtual int ErrorOffset
		{
			get
			{
				return ErrorOffset_Renamed;
			}
		}

		//============ privates ============
		/// <summary>
		/// The zero-based character offset into the string being parsed at which
		/// the error was found during parsing.
		/// @serial
		/// </summary>
		private int ErrorOffset_Renamed;
	}

}