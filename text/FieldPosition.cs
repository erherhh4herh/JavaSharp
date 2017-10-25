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
 * (C) Copyright Taligent, Inc. 1996 - All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - All Rights Reserved
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
	/// <code>FieldPosition</code> is a simple class used by <code>Format</code>
	/// and its subclasses to identify fields in formatted output. Fields can
	/// be identified in two ways:
	/// <ul>
	///  <li>By an integer constant, whose names typically end with
	///      <code>_FIELD</code>. The constants are defined in the various
	///      subclasses of <code>Format</code>.
	///  <li>By a <code>Format.Field</code> constant, see <code>ERA_FIELD</code>
	///      and its friends in <code>DateFormat</code> for an example.
	/// </ul>
	/// <para>
	/// <code>FieldPosition</code> keeps track of the position of the
	/// field within the formatted output with two indices: the index
	/// of the first character of the field and the index of the last
	/// character of the field.
	/// 
	/// </para>
	/// <para>
	/// One version of the <code>format</code> method in the various
	/// <code>Format</code> classes requires a <code>FieldPosition</code>
	/// object as an argument. You use this <code>format</code> method
	/// to perform partial formatting or to get information about the
	/// formatted output (such as the position of a field).
	/// 
	/// </para>
	/// <para>
	/// If you are interested in the positions of all attributes in the
	/// formatted string use the <code>Format</code> method
	/// <code>formatToCharacterIterator</code>.
	/// 
	/// @author      Mark Davis
	/// </para>
	/// </summary>
	/// <seealso cref=         java.text.Format </seealso>
	public class FieldPosition
	{

		/// <summary>
		/// Input: Desired field to determine start and end offsets for.
		/// The meaning depends on the subclass of Format.
		/// </summary>
		internal int Field_Renamed = 0;

		/// <summary>
		/// Output: End offset of field in text.
		/// If the field does not occur in the text, 0 is returned.
		/// </summary>
		internal int EndIndex_Renamed = 0;

		/// <summary>
		/// Output: Start offset of field in text.
		/// If the field does not occur in the text, 0 is returned.
		/// </summary>
		internal int BeginIndex_Renamed = 0;

		/// <summary>
		/// Desired field this FieldPosition is for.
		/// </summary>
		private Format.Field Attribute;

		/// <summary>
		/// Creates a FieldPosition object for the given field.  Fields are
		/// identified by constants, whose names typically end with _FIELD,
		/// in the various subclasses of Format.
		/// </summary>
		/// <param name="field"> the field identifier </param>
		/// <seealso cref= java.text.NumberFormat#INTEGER_FIELD </seealso>
		/// <seealso cref= java.text.NumberFormat#FRACTION_FIELD </seealso>
		/// <seealso cref= java.text.DateFormat#YEAR_FIELD </seealso>
		/// <seealso cref= java.text.DateFormat#MONTH_FIELD </seealso>
		public FieldPosition(int field)
		{
			this.Field_Renamed = field;
		}

		/// <summary>
		/// Creates a FieldPosition object for the given field constant. Fields are
		/// identified by constants defined in the various <code>Format</code>
		/// subclasses. This is equivalent to calling
		/// <code>new FieldPosition(attribute, -1)</code>.
		/// </summary>
		/// <param name="attribute"> Format.Field constant identifying a field
		/// @since 1.4 </param>
		public FieldPosition(Format.Field attribute) : this(attribute, -1)
		{
		}

		/// <summary>
		/// Creates a <code>FieldPosition</code> object for the given field.
		/// The field is identified by an attribute constant from one of the
		/// <code>Field</code> subclasses as well as an integer field ID
		/// defined by the <code>Format</code> subclasses. <code>Format</code>
		/// subclasses that are aware of <code>Field</code> should give precedence
		/// to <code>attribute</code> and ignore <code>fieldID</code> if
		/// <code>attribute</code> is not null. However, older <code>Format</code>
		/// subclasses may not be aware of <code>Field</code> and rely on
		/// <code>fieldID</code>. If the field has no corresponding integer
		/// constant, <code>fieldID</code> should be -1.
		/// </summary>
		/// <param name="attribute"> Format.Field constant identifying a field </param>
		/// <param name="fieldID"> integer constant identifying a field
		/// @since 1.4 </param>
		public FieldPosition(Format.Field attribute, int fieldID)
		{
			this.Attribute = attribute;
			this.Field_Renamed = fieldID;
		}

		/// <summary>
		/// Returns the field identifier as an attribute constant
		/// from one of the <code>Field</code> subclasses. May return null if
		/// the field is specified only by an integer field ID.
		/// </summary>
		/// <returns> Identifier for the field
		/// @since 1.4 </returns>
		public virtual Format.Field FieldAttribute
		{
			get
			{
				return Attribute;
			}
		}

		/// <summary>
		/// Retrieves the field identifier.
		/// </summary>
		/// <returns> the field identifier </returns>
		public virtual int Field
		{
			get
			{
				return Field_Renamed;
			}
		}

		/// <summary>
		/// Retrieves the index of the first character in the requested field.
		/// </summary>
		/// <returns> the begin index </returns>
		public virtual int BeginIndex
		{
			get
			{
				return BeginIndex_Renamed;
			}
			set
			{
				BeginIndex_Renamed = value;
			}
		}

		/// <summary>
		/// Retrieves the index of the character following the last character in the
		/// requested field.
		/// </summary>
		/// <returns> the end index </returns>
		public virtual int EndIndex
		{
			get
			{
				return EndIndex_Renamed;
			}
			set
			{
				EndIndex_Renamed = value;
			}
		}



		/// <summary>
		/// Returns a <code>Format.FieldDelegate</code> instance that is associated
		/// with the FieldPosition. When the delegate is notified of the same
		/// field the FieldPosition is associated with, the begin/end will be
		/// adjusted.
		/// </summary>
		internal virtual Format.FieldDelegate FieldDelegate
		{
			get
			{
				return new Delegate(this);
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
			if (!(obj is FieldPosition))
			{
				return false;
			}
			FieldPosition other = (FieldPosition) obj;
			if (Attribute == null)
			{
				if (other.Attribute != null)
				{
					return false;
				}
			}
			else if (!Attribute.Equals(other.Attribute))
			{
				return false;
			}
			return (BeginIndex_Renamed == other.BeginIndex_Renamed && EndIndex_Renamed == other.EndIndex_Renamed && Field_Renamed == other.Field_Renamed);
		}

		/// <summary>
		/// Returns a hash code for this FieldPosition. </summary>
		/// <returns> a hash code value for this object </returns>
		public override int HashCode()
		{
			return (Field_Renamed << 24) | (BeginIndex_Renamed << 16) | EndIndex_Renamed;
		}

		/// <summary>
		/// Return a string representation of this FieldPosition. </summary>
		/// <returns>  a string representation of this object </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[field=" + Field_Renamed + ",attribute=" + Attribute + ",beginIndex=" + BeginIndex_Renamed + ",endIndex=" + EndIndex_Renamed + ']';
		}


		/// <summary>
		/// Return true if the receiver wants a <code>Format.Field</code> value and
		/// <code>attribute</code> is equal to it.
		/// </summary>
		private bool MatchesField(Format.Field attribute)
		{
			if (this.Attribute != null)
			{
				return this.Attribute.Equals(attribute);
			}
			return false;
		}

		/// <summary>
		/// Return true if the receiver wants a <code>Format.Field</code> value and
		/// <code>attribute</code> is equal to it, or true if the receiver
		/// represents an inteter constant and <code>field</code> equals it.
		/// </summary>
		private bool MatchesField(Format.Field attribute, int field)
		{
			if (this.Attribute != null)
			{
				return this.Attribute.Equals(attribute);
			}
			return (field == this.Field_Renamed);
		}


		/// <summary>
		/// An implementation of FieldDelegate that will adjust the begin/end
		/// of the FieldPosition if the arguments match the field of
		/// the FieldPosition.
		/// </summary>
		private class Delegate : Format.FieldDelegate
		{
			private readonly FieldPosition OuterInstance;

			public Delegate(FieldPosition outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/// <summary>
			/// Indicates whether the field has been  encountered before. If this
			/// is true, and <code>formatted</code> is invoked, the begin/end
			/// are not updated.
			/// </summary>
			internal bool EncounteredField;

			public virtual void Formatted(Format.Field attr, Object value, int start, int end, StringBuffer buffer)
			{
				if (!EncounteredField && outerInstance.MatchesField(attr))
				{
					outerInstance.BeginIndex = start;
					outerInstance.EndIndex = end;
					EncounteredField = (start != end);
				}
			}

			public virtual void Formatted(int fieldID, Format.Field attr, Object value, int start, int end, StringBuffer buffer)
			{
				if (!EncounteredField && outerInstance.MatchesField(attr, fieldID))
				{
					outerInstance.BeginIndex = start;
					outerInstance.EndIndex = end;
					EncounteredField = (start != end);
				}
			}
		}
	}

}