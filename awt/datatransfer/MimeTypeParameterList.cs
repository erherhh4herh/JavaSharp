using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// An object that encapsulates the parameter list of a MimeType
	/// as defined in RFC 2045 and 2046.
	/// 
	/// @author jeff.dunn@eng.sun.com
	/// </summary>
	internal class MimeTypeParameterList : Cloneable
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MimeTypeParameterList()
		{
			Parameters = new Dictionary<>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MimeTypeParameterList(String rawdata) throws MimeTypeParseException
		public MimeTypeParameterList(String rawdata)
		{
			Parameters = new Dictionary<>();

			//    now parse rawdata
			Parse(rawdata);
		}

		public override int HashCode()
		{
			int code = Integer.MaxValue / 45; // "random" value for empty lists
			String paramName = null;
			IEnumerator<String> enum_ = this.Names;

			while (enum_.MoveNext())
			{
				paramName = enum_.Current;
				code += paramName.HashCode();
				code += this.Get(paramName).HashCode();
			}

			return code;
		} // hashCode()

		/// <summary>
		/// Two parameter lists are considered equal if they have exactly
		/// the same set of parameter names and associated values. The
		/// order of the parameters is not considered.
		/// </summary>
		public override bool Equals(Object thatObject)
		{
			//System.out.println("MimeTypeParameterList.equals("+this+","+thatObject+")");
			if (!(thatObject is MimeTypeParameterList))
			{
				return false;
			}
			MimeTypeParameterList that = (MimeTypeParameterList)thatObject;
			if (this.Size() != that.Size())
			{
				return false;
			}
			String name = null;
			String thisValue = null;
			String thatValue = null;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
			Set<java.util.Map_Entry<String, String>> entries = Parameters.entrySet();
			IEnumerator<java.util.Map_Entry<String, String>> iterator = entries.Iterator();
			java.util.Map_Entry<String, String> entry = null;
			while (iterator.MoveNext())
			{
				entry = iterator.Current;
				name = entry.Key;
				thisValue = entry.Value;
				thatValue = that.Parameters[name];
				if ((thisValue == null) || (thatValue == null))
				{
					// both null -> equal, only one null -> not equal
					if (thisValue != thatValue)
					{
						return false;
					}
				}
				else if (!thisValue.Equals(thatValue))
				{
					return false;
				}
			} // while iterator

			return true;
		} // equals()

		/// <summary>
		/// A routine for parsing the parameter list out of a String.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void parse(String rawdata) throws MimeTypeParseException
		protected internal virtual void Parse(String rawdata)
		{
			int length = rawdata.Length();
			if (length > 0)
			{
				int currentIndex = SkipWhiteSpace(rawdata, 0);
				int lastIndex = 0;

				if (currentIndex < length)
				{
					char currentChar = rawdata.CharAt(currentIndex);
					while ((currentIndex < length) && (currentChar == ';'))
					{
						String name;
						String value;
						bool foundit;

						//    eat the ';'
						++currentIndex;

						//    now parse the parameter name

						//    skip whitespace
						currentIndex = SkipWhiteSpace(rawdata, currentIndex);

						if (currentIndex < length)
						{
							//    find the end of the token char run
							lastIndex = currentIndex;
							currentChar = rawdata.CharAt(currentIndex);
							while ((currentIndex < length) && IsTokenChar(currentChar))
							{
								++currentIndex;
								currentChar = rawdata.CharAt(currentIndex);
							}
							name = rawdata.Substring(lastIndex, currentIndex - lastIndex).ToLowerCase();

							//    now parse the '=' that separates the name from the value

							//    skip whitespace
							currentIndex = SkipWhiteSpace(rawdata, currentIndex);

							if ((currentIndex < length) && (rawdata.CharAt(currentIndex) == '='))
							{
								//    eat it and parse the parameter value
								++currentIndex;

								//    skip whitespace
								currentIndex = SkipWhiteSpace(rawdata, currentIndex);

								if (currentIndex < length)
								{
									//    now find out whether or not we have a quoted value
									currentChar = rawdata.CharAt(currentIndex);
									if (currentChar == '"')
									{
										//    yup it's quoted so eat it and capture the quoted string
										++currentIndex;
										lastIndex = currentIndex;

										if (currentIndex < length)
										{
											//    find the next unescqped quote
											foundit = false;
											while ((currentIndex < length) && !foundit)
											{
												currentChar = rawdata.CharAt(currentIndex);
												if (currentChar == '\\')
												{
													//    found an escape sequence so pass this and the next character
													currentIndex += 2;
												}
												else if (currentChar == '"')
												{
													//    foundit!
													foundit = true;
												}
												else
												{
													++currentIndex;
												}
											}
											if (currentChar == '"')
											{
												value = Unquote(rawdata.Substring(lastIndex, currentIndex - lastIndex));
												//    eat the quote
												++currentIndex;
											}
											else
											{
												throw new MimeTypeParseException("Encountered unterminated quoted parameter value.");
											}
										}
										else
										{
											throw new MimeTypeParseException("Encountered unterminated quoted parameter value.");
										}
									}
									else if (IsTokenChar(currentChar))
									{
										//    nope it's an ordinary token so it ends with a non-token char
										lastIndex = currentIndex;
										foundit = false;
										while ((currentIndex < length) && !foundit)
										{
											currentChar = rawdata.CharAt(currentIndex);

											if (IsTokenChar(currentChar))
											{
												++currentIndex;
											}
											else
											{
												foundit = true;
											}
										}
										value = rawdata.Substring(lastIndex, currentIndex - lastIndex);
									}
									else
									{
										//    it ain't a value
										throw new MimeTypeParseException("Unexpected character encountered at index " + currentIndex);
									}

									//    now put the data into the hashtable
									Parameters[name] = value;
								}
								else
								{
									throw new MimeTypeParseException("Couldn't find a value for parameter named " + name);
								}
							}
							else
							{
								throw new MimeTypeParseException("Couldn't find the '=' that separates a parameter name from its value.");
							}
						}
						else
						{
							throw new MimeTypeParseException("Couldn't find parameter name");
						}

						//    setup the next iteration
						currentIndex = SkipWhiteSpace(rawdata, currentIndex);
						if (currentIndex < length)
						{
							currentChar = rawdata.CharAt(currentIndex);
						}
					}
					if (currentIndex < length)
					{
						throw new MimeTypeParseException("More characters encountered in input than expected.");
					}
				}
			}
		}

		/// <summary>
		/// return the number of name-value pairs in this list.
		/// </summary>
		public virtual int Size()
		{
			return Parameters.Count;
		}

		/// <summary>
		/// Determine whether or not this list is empty.
		/// </summary>
		public virtual bool Empty
		{
			get
			{
				return Parameters.Count == 0;
			}
		}

		/// <summary>
		/// Retrieve the value associated with the given name, or null if there
		/// is no current association.
		/// </summary>
		public virtual String Get(String name)
		{
			return Parameters[name.Trim().ToLowerCase()];
		}

		/// <summary>
		/// Set the value to be associated with the given name, replacing
		/// any previous association.
		/// </summary>
		public virtual void Set(String name, String value)
		{
			Parameters[name.Trim().ToLowerCase()] = value;
		}

		/// <summary>
		/// Remove any value associated with the given name.
		/// </summary>
		public virtual void Remove(String name)
		{
			Parameters.Remove(name.Trim().ToLowerCase());
		}

		/// <summary>
		/// Retrieve an enumeration of all the names in this list.
		/// </summary>
		public virtual IEnumerator<String> Names
		{
			get
			{
				return Parameters.Keys.GetEnumerator();
			}
		}

		public override String ToString()
		{
			// Heuristic: 8 characters per field
			StringBuilder buffer = new StringBuilder(Parameters.Count * 16);

			IEnumerator<String> keys = Parameters.Keys.GetEnumerator();
			while (keys.MoveNext())
			{
				buffer.Append("; ");

				String key = keys.Current;
				buffer.Append(key);
				buffer.Append('=');
				   buffer.Append(Quote(Parameters[key]));
			}

			return buffer.ToString();
		}

		/// <returns> a clone of this object </returns>

		 public virtual Object Clone()
		 {
			 MimeTypeParameterList newObj = null;
			 try
			 {
				 newObj = (MimeTypeParameterList)base.Clone();
			 }
			 catch (CloneNotSupportedException)
			 {
			 }
			 newObj.Parameters = (Hashtable)Parameters.clone();
			 return newObj;
		 }

		private Dictionary<String, String> Parameters;

		//    below here be scary parsing related things

		/// <summary>
		/// Determine whether or not a given character belongs to a legal token.
		/// </summary>
		private static bool IsTokenChar(char c)
		{
			return ((c > 0x20) && (c < 0x7F)) && (TSPECIALS.IndexOf(c) < 0);
		}

		/// <summary>
		/// return the index of the first non white space character in
		/// rawdata at or after index i.
		/// </summary>
		private static int SkipWhiteSpace(String rawdata, int i)
		{
			int length = rawdata.Length();
			if (i < length)
			{
				char c = rawdata.CharAt(i);
				while ((i < length) && char.IsWhiteSpace(c))
				{
					++i;
					c = rawdata.CharAt(i);
				}
			}

			return i;
		}

		/// <summary>
		/// A routine that knows how and when to quote and escape the given value.
		/// </summary>
		private static String Quote(String value)
		{
			bool needsQuotes = false;

			//    check to see if we actually have to quote this thing
			int length = value.Length();
			for (int i = 0; (i < length) && !needsQuotes; ++i)
			{
				needsQuotes = !IsTokenChar(value.CharAt(i));
			}

			if (needsQuotes)
			{
				StringBuilder buffer = new StringBuilder((int)(length * 1.5));

				//    add the initial quote
				buffer.Append('"');

				//    add the properly escaped text
				for (int i = 0; i < length; ++i)
				{
					char c = value.CharAt(i);
					if ((c == '\\') || (c == '"'))
					{
						buffer.Append('\\');
					}
					buffer.Append(c);
				}

				//    add the closing quote
				buffer.Append('"');

				return buffer.ToString();
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// A routine that knows how to strip the quotes and escape sequences from the given value.
		/// </summary>
		private static String Unquote(String value)
		{
			int valueLength = value.Length();
			StringBuilder buffer = new StringBuilder(valueLength);

			bool escaped = false;
			for (int i = 0; i < valueLength; ++i)
			{
				char currentChar = value.CharAt(i);
				if (!escaped && (currentChar != '\\'))
				{
					buffer.Append(currentChar);
				}
				else if (escaped)
				{
					buffer.Append(currentChar);
					escaped = false;
				}
				else
				{
					escaped = true;
				}
			}

			return buffer.ToString();
		}

		/// <summary>
		/// A string that holds all the special chars.
		/// </summary>
		private static readonly String TSPECIALS = "()<>@,;:\\\"/[]?=";

	}

}