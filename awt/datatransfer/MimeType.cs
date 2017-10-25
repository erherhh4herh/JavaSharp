using System;

/*
 * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.datatransfer
{



	/// <summary>
	/// A Multipurpose Internet Mail Extension (MIME) type, as defined
	/// in RFC 2045 and 2046.
	/// 
	/// THIS IS *NOT* - REPEAT *NOT* - A PUBLIC CLASS! DataFlavor IS
	/// THE PUBLIC INTERFACE, AND THIS IS PROVIDED AS A ***PRIVATE***
	/// (THAT IS AS IN *NOT* PUBLIC) HELPER CLASS!
	/// </summary>
	[Serializable]
	internal class MimeType : Externalizable, Cloneable
	{

		/*
		 * serialization support
		 */

		internal const long SerialVersionUID = -6568722458793895906L;

		/// <summary>
		/// Constructor for externalization; this constructor should not be
		/// called directly by an application, since the result will be an
		/// uninitialized, immutable <code>MimeType</code> object.
		/// </summary>
		public MimeType()
		{
		}

		/// <summary>
		/// Builds a <code>MimeType</code> from a <code>String</code>.
		/// </summary>
		/// <param name="rawdata"> text used to initialize the <code>MimeType</code> </param>
		/// <exception cref="NullPointerException"> if <code>rawdata</code> is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MimeType(String rawdata) throws MimeTypeParseException
		public MimeType(String rawdata)
		{
			Parse(rawdata);
		}

		/// <summary>
		/// Builds a <code>MimeType</code> with the given primary and sub
		/// type but has an empty parameter list.
		/// </summary>
		/// <param name="primary"> the primary type of this <code>MimeType</code> </param>
		/// <param name="sub"> the subtype of this <code>MimeType</code> </param>
		/// <exception cref="NullPointerException"> if either <code>primary</code> or
		///         <code>sub</code> is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MimeType(String primary, String sub) throws MimeTypeParseException
		public MimeType(String primary, String sub) : this(primary, sub, new MimeTypeParameterList())
		{
		}

		/// <summary>
		/// Builds a <code>MimeType</code> with a pre-defined
		/// and valid (or empty) parameter list.
		/// </summary>
		/// <param name="primary"> the primary type of this <code>MimeType</code> </param>
		/// <param name="sub"> the subtype of this <code>MimeType</code> </param>
		/// <param name="mtpl"> the requested parameter list </param>
		/// <exception cref="NullPointerException"> if either <code>primary</code>,
		///         <code>sub</code> or <code>mtpl</code> is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MimeType(String primary, String sub, MimeTypeParameterList mtpl) throws MimeTypeParseException
		public MimeType(String primary, String sub, MimeTypeParameterList mtpl)
		{
			//    check to see if primary is valid
			if (IsValidToken(primary))
			{
				PrimaryType_Renamed = primary.ToLowerCase(Locale.ENGLISH);
			}
			else
			{
				throw new MimeTypeParseException("Primary type is invalid.");
			}

			//    check to see if sub is valid
			if (IsValidToken(sub))
			{
				SubType_Renamed = sub.ToLowerCase(Locale.ENGLISH);
			}
			else
			{
				throw new MimeTypeParseException("Sub type is invalid.");
			}

			Parameters_Renamed = (MimeTypeParameterList)mtpl.Clone();
		}

		public override int HashCode()
		{

			// We sum up the hash codes for all of the strings. This
			// way, the order of the strings is irrelevant
			int code = 0;
			code += PrimaryType_Renamed.HashCode();
			code += SubType_Renamed.HashCode();
			code += Parameters_Renamed.HashCode();
			return code;
		} // hashCode()

		/// <summary>
		/// <code>MimeType</code>s are equal if their primary types,
		/// subtypes, and  parameters are all equal. No default values
		/// are taken into account. </summary>
		/// <param name="thatObject"> the object to be evaluated as a
		///    <code>MimeType</code> </param>
		/// <returns> <code>true</code> if <code>thatObject</code> is
		///    a <code>MimeType</code>; otherwise returns <code>false</code> </returns>
		public override bool Equals(Object thatObject)
		{
			if (!(thatObject is MimeType))
			{
				return false;
			}
			MimeType that = (MimeType)thatObject;
			bool isIt = ((this.PrimaryType_Renamed.Equals(that.PrimaryType_Renamed)) && (this.SubType_Renamed.Equals(that.SubType_Renamed)) && (this.Parameters_Renamed.Equals(that.Parameters_Renamed)));
			return isIt;
		} // equals()

		/// <summary>
		/// A routine for parsing the MIME type out of a String.
		/// </summary>
		/// <exception cref="NullPointerException"> if <code>rawdata</code> is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parse(String rawdata) throws MimeTypeParseException
		private void Parse(String rawdata)
		{
			int slashIndex = rawdata.IndexOf('/');
			int semIndex = rawdata.IndexOf(';');
			if ((slashIndex < 0) && (semIndex < 0))
			{
				//    neither character is present, so treat it
				//    as an error
				throw new MimeTypeParseException("Unable to find a sub type.");
			}
			else if ((slashIndex < 0) && (semIndex >= 0))
			{
				//    we have a ';' (and therefore a parameter list),
				//    but no '/' indicating a sub type is present
				throw new MimeTypeParseException("Unable to find a sub type.");
			}
			else if ((slashIndex >= 0) && (semIndex < 0))
			{
				//    we have a primary and sub type but no parameter list
				PrimaryType_Renamed = rawdata.Substring(0,slashIndex).Trim().ToLowerCase(Locale.ENGLISH);
				SubType_Renamed = rawdata.Substring(slashIndex + 1).Trim().ToLowerCase(Locale.ENGLISH);
				Parameters_Renamed = new MimeTypeParameterList();
			}
			else if (slashIndex < semIndex)
			{
				//    we have all three items in the proper sequence
				PrimaryType_Renamed = rawdata.Substring(0, slashIndex).Trim().ToLowerCase(Locale.ENGLISH);
				SubType_Renamed = StringHelperClass.SubstringSpecial(rawdata, slashIndex + 1, semIndex).Trim().ToLowerCase(Locale.ENGLISH);
				Parameters_Renamed = new MimeTypeParameterList(rawdata.Substring(semIndex));
			}
			else
			{
				//    we have a ';' lexically before a '/' which means we have a primary type
				//    & a parameter list but no sub type
				throw new MimeTypeParseException("Unable to find a sub type.");
			}

			//    now validate the primary and sub types

			//    check to see if primary is valid
			if (!IsValidToken(PrimaryType_Renamed))
			{
				throw new MimeTypeParseException("Primary type is invalid.");
			}

			//    check to see if sub is valid
			if (!IsValidToken(SubType_Renamed))
			{
				throw new MimeTypeParseException("Sub type is invalid.");
			}
		}

		/// <summary>
		/// Retrieve the primary type of this object.
		/// </summary>
		public virtual String PrimaryType
		{
			get
			{
				return PrimaryType_Renamed;
			}
		}

		/// <summary>
		/// Retrieve the sub type of this object.
		/// </summary>
		public virtual String SubType
		{
			get
			{
				return SubType_Renamed;
			}
		}

		/// <summary>
		/// Retrieve a copy of this object's parameter list.
		/// </summary>
		public virtual MimeTypeParameterList Parameters
		{
			get
			{
				return (MimeTypeParameterList)Parameters_Renamed.Clone();
			}
		}

		/// <summary>
		/// Retrieve the value associated with the given name, or null if there
		/// is no current association.
		/// </summary>
		public virtual String GetParameter(String name)
		{
			return Parameters_Renamed.Get(name);
		}

		/// <summary>
		/// Set the value to be associated with the given name, replacing
		/// any previous association.
		/// 
		/// @throw IllegalArgumentException if parameter or value is illegal
		/// </summary>
		public virtual void SetParameter(String name, String value)
		{
			Parameters_Renamed.Set(name, value);
		}

		/// <summary>
		/// Remove any value associated with the given name.
		/// 
		/// @throw IllegalArgumentExcpetion if parameter may not be deleted
		/// </summary>
		public virtual void RemoveParameter(String name)
		{
			Parameters_Renamed.Remove(name);
		}

		/// <summary>
		/// Return the String representation of this object.
		/// </summary>
		public override String ToString()
		{
			return BaseType + Parameters_Renamed.ToString();
		}

		/// <summary>
		/// Return a String representation of this object
		/// without the parameter list.
		/// </summary>
		public virtual String BaseType
		{
			get
			{
				return PrimaryType_Renamed + "/" + SubType_Renamed;
			}
		}

		/// <summary>
		/// Returns <code>true</code> if the primary type and the
		/// subtype of this object are the same as the specified
		/// <code>type</code>; otherwise returns <code>false</code>.
		/// </summary>
		/// <param name="type"> the type to compare to <code>this</code>'s type </param>
		/// <returns> <code>true</code> if the primary type and the
		///    subtype of this object are the same as the
		///    specified <code>type</code>; otherwise returns
		///    <code>false</code> </returns>
		public virtual bool Match(MimeType type)
		{
			if (type == null)
			{
				return false;
			}
			return PrimaryType_Renamed.Equals(type.PrimaryType) && (SubType_Renamed.Equals("*") || type.SubType.Equals("*") || (SubType_Renamed.Equals(type.SubType)));
		}

		/// <summary>
		/// Returns <code>true</code> if the primary type and the
		/// subtype of this object are the same as the content type
		/// described in <code>rawdata</code>; otherwise returns
		/// <code>false</code>.
		/// </summary>
		/// <param name="rawdata"> the raw data to be examined </param>
		/// <returns> <code>true</code> if the primary type and the
		///    subtype of this object are the same as the content type
		///    described in <code>rawdata</code>; otherwise returns
		///    <code>false</code>; if <code>rawdata</code> is
		///    <code>null</code>, returns <code>false</code> </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean match(String rawdata) throws MimeTypeParseException
		public virtual bool Match(String rawdata)
		{
			if (rawdata == null)
			{
				return false;
			}
			return Match(new MimeType(rawdata));
		}

		/// <summary>
		/// The object implements the writeExternal method to save its contents
		/// by calling the methods of DataOutput for its primitive values or
		/// calling the writeObject method of ObjectOutput for objects, strings
		/// and arrays. </summary>
		/// <exception cref="IOException"> Includes any I/O exceptions that may occur </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeExternal(java.io.ObjectOutput out) throws java.io.IOException
		public virtual void WriteExternal(ObjectOutput @out)
		{
			String s = ToString(); // contains ASCII chars only
			// one-to-one correspondence between ASCII char and byte in UTF string
			if (s.Length() <= 65535) // 65535 is max length of UTF string
			{
				@out.WriteUTF(s);
			}
			else
			{
				@out.WriteByte(0);
				@out.WriteByte(0);
				@out.WriteInt(s.Length());
				@out.Write(s.Bytes);
			}
		}

		/// <summary>
		/// The object implements the readExternal method to restore its
		/// contents by calling the methods of DataInput for primitive
		/// types and readObject for objects, strings and arrays.  The
		/// readExternal method must read the values in the same sequence
		/// and with the same types as were written by writeExternal. </summary>
		/// <exception cref="ClassNotFoundException"> If the class for an object being
		///              restored cannot be found. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
		public virtual void ReadExternal(ObjectInput @in)
		{
			String s = @in.ReadUTF();
			if (s == null || s.Length() == 0) // long mime type
			{
				sbyte[] ba = new sbyte[@in.ReadInt()];
				@in.ReadFully(ba);
				s = StringHelperClass.NewString(ba);
			}
			try
			{
				Parse(s);
			}
			catch (MimeTypeParseException e)
			{
				throw new IOException(e.ToString());
			}
		}

		/// <summary>
		/// Returns a clone of this object. </summary>
		/// <returns> a clone of this object </returns>

		public virtual Object Clone()
		{
			MimeType newObj = null;
			try
			{
				newObj = (MimeType)base.Clone();
			}
			catch (CloneNotSupportedException)
			{
			}
			newObj.Parameters_Renamed = (MimeTypeParameterList)Parameters_Renamed.Clone();
			return newObj;
		}

		private String PrimaryType_Renamed;
		private String SubType_Renamed;
		private MimeTypeParameterList Parameters_Renamed;

		//    below here be scary parsing related things

		/// <summary>
		/// Determines whether or not a given character belongs to a legal token.
		/// </summary>
		private static bool IsTokenChar(char c)
		{
			return ((c > 0x20) && (c < 0x7F)) && (TSPECIALS.IndexOf(c) < 0);
		}

		/// <summary>
		/// Determines whether or not a given string is a legal token.
		/// </summary>
		/// <exception cref="NullPointerException"> if <code>s</code> is null </exception>
		private bool IsValidToken(String s)
		{
			int len = s.Length();
			if (len > 0)
			{
				for (int i = 0; i < len; ++i)
				{
					char c = s.CharAt(i);
					if (!IsTokenChar(c))
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// A string that holds all the special chars.
		/// </summary>

		private static readonly String TSPECIALS = "()<>@,;:\\\"/[]?=";

	} // class MimeType

}