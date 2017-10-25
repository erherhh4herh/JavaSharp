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
	/// <code>ParsePosition</code> is a simple class used by <code>Format</code>
	/// and its subclasses to keep track of the current position during parsing.
	/// The <code>parseObject</code> method in the various <code>Format</code>
	/// classes requires a <code>ParsePosition</code> object as an argument.
	/// 
	/// <para>
	/// By design, as you parse through a string with different formats,
	/// you can use the same <code>ParsePosition</code>, since the index parameter
	/// records the current position.
	/// 
	/// @author      Mark Davis
	/// </para>
	/// </summary>
	/// <seealso cref=         java.text.Format </seealso>

	public class ParsePosition
	{

		/// <summary>
		/// Input: the place you start parsing.
		/// <br>Output: position where the parse stopped.
		/// This is designed to be used serially,
		/// with each call setting index up for the next one.
		/// </summary>
		internal int Index_Renamed = 0;
		internal int ErrorIndex_Renamed = -1;

		/// <summary>
		/// Retrieve the current parse position.  On input to a parse method, this
		/// is the index of the character at which parsing will begin; on output, it
		/// is the index of the character following the last character parsed.
		/// </summary>
		/// <returns> the current parse position </returns>
		public virtual int Index
		{
			get
			{
				return Index_Renamed;
			}
			set
			{
				this.Index_Renamed = value;
			}
		}


		/// <summary>
		/// Create a new ParsePosition with the given initial index.
		/// </summary>
		/// <param name="index"> initial index </param>
		public ParsePosition(int index)
		{
			this.Index_Renamed = index;
		}
		/// <summary>
		/// Set the index at which a parse error occurred.  Formatters
		/// should set this before returning an error code from their
		/// parseObject method.  The default value is -1 if this is not set.
		/// </summary>
		/// <param name="ei"> the index at which an error occurred
		/// @since 1.2 </param>
		public virtual int ErrorIndex
		{
			set
			{
				ErrorIndex_Renamed = value;
			}
			get
			{
				return ErrorIndex_Renamed;
			}
		}


		/// <summary>
		/// Overrides equals
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!(obj is ParsePosition))
			{
				return false;
			}
			ParsePosition other = (ParsePosition) obj;
			return (Index_Renamed == other.Index_Renamed && ErrorIndex_Renamed == other.ErrorIndex_Renamed);
		}

		/// <summary>
		/// Returns a hash code for this ParsePosition. </summary>
		/// <returns> a hash code value for this object </returns>
		public override int HashCode()
		{
			return (ErrorIndex_Renamed << 16) | Index_Renamed;
		}

		/// <summary>
		/// Return a string representation of this ParsePosition. </summary>
		/// <returns>  a string representation of this object </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[index=" + Index_Renamed + ",errorIndex=" + ErrorIndex_Renamed + ']';
		}
	}

}