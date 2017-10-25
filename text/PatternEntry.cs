using System;

/*
 * Copyright (c) 1996, 2000, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright IBM Corp. 1996, 1997 - All Rights Reserved
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
	/// Utility class for normalizing and merging patterns for collation.
	/// This is to be used with MergeCollation for adding patterns to an
	/// existing rule table. </summary>
	/// <seealso cref=        MergeCollation
	/// @author     Mark Davis, Helena Shih </seealso>

	internal class PatternEntry
	{
		/// <summary>
		/// Gets the current extension, quoted
		/// </summary>
		public virtual void AppendQuotedExtension(StringBuffer toAddTo)
		{
			AppendQuoted(Extension_Renamed,toAddTo);
		}

		/// <summary>
		/// Gets the current chars, quoted
		/// </summary>
		public virtual void AppendQuotedChars(StringBuffer toAddTo)
		{
			AppendQuoted(Chars_Renamed,toAddTo);
		}

		/// <summary>
		/// WARNING this is used for searching in a Vector.
		/// Because Vector.indexOf doesn't take a comparator,
		/// this method is ill-defined and ignores strength.
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			PatternEntry other = (PatternEntry) obj;
			bool result = Chars_Renamed.Equals(other.Chars_Renamed);
			return result;
		}

		public override int HashCode()
		{
			return Chars_Renamed.HashCode();
		}

		/// <summary>
		/// For debugging.
		/// </summary>
		public override String ToString()
		{
			StringBuffer result = new StringBuffer();
			AddToBuffer(result, true, false, null);
			return result.ToString();
		}

		/// <summary>
		/// Gets the strength of the entry.
		/// </summary>
		internal int Strength
		{
			get
			{
				return Strength_Renamed;
			}
		}

		/// <summary>
		/// Gets the expanding characters of the entry.
		/// </summary>
		internal String Extension
		{
			get
			{
				return Extension_Renamed;
			}
		}

		/// <summary>
		/// Gets the core characters of the entry.
		/// </summary>
		internal String Chars
		{
			get
			{
				return Chars_Renamed;
			}
		}

		// ===== privates =====

		internal virtual void AddToBuffer(StringBuffer toAddTo, bool showExtension, bool showWhiteSpace, PatternEntry lastEntry)
		{
			if (showWhiteSpace && toAddTo.Length() > 0)
			{
				if (Strength_Renamed == Collator.PRIMARY || lastEntry != null)
				{
					toAddTo.Append('\n');
				}
				else
				{
					toAddTo.Append(' ');
				}
			}
			if (lastEntry != null)
			{
				toAddTo.Append('&');
				if (showWhiteSpace)
				{
					toAddTo.Append(' ');
				}
				lastEntry.AppendQuotedChars(toAddTo);
				AppendQuotedExtension(toAddTo);
				if (showWhiteSpace)
				{
					toAddTo.Append(' ');
				}
			}
			switch (Strength_Renamed)
			{
			case Collator.IDENTICAL:
				toAddTo.Append('=');
				break;
			case Collator.TERTIARY:
				toAddTo.Append(',');
				break;
			case Collator.SECONDARY:
				toAddTo.Append(';');
				break;
			case Collator.PRIMARY:
				toAddTo.Append('<');
				break;
			case RESET:
				toAddTo.Append('&');
				break;
			case UNSET:
				toAddTo.Append('?');
				break;
			}
			if (showWhiteSpace)
			{
				toAddTo.Append(' ');
			}
			AppendQuoted(Chars_Renamed,toAddTo);
			if (showExtension && Extension_Renamed.Length() != 0)
			{
				toAddTo.Append('/');
				AppendQuoted(Extension_Renamed,toAddTo);
			}
		}

		internal static void AppendQuoted(String chars, StringBuffer toAddTo)
		{
			bool inQuote = false;
			char ch = chars.CharAt(0);
			if (Character.IsSpaceChar(ch))
			{
				inQuote = true;
				toAddTo.Append('\'');
			}
			else
			{
			  if (PatternEntry.IsSpecialChar(ch))
			  {
					inQuote = true;
					toAddTo.Append('\'');
			  }
				else
				{
					switch (ch)
					{
						case 0x0010:
					case '\f':
				case '\r':
						case '\t':
					case '\n':
				case '@':
						inQuote = true;
						toAddTo.Append('\'');
						break;
					case '\'':
						inQuote = true;
						toAddTo.Append('\'');
						break;
					default:
						if (inQuote)
						{
							inQuote = false;
							toAddTo.Append('\'');
						}
						break;
					}
				}
			}
			toAddTo.Append(chars);
			if (inQuote)
			{
				toAddTo.Append('\'');
			}
		}

		//========================================================================
		// Parsing a pattern into a list of PatternEntries....
		//========================================================================

		internal PatternEntry(int strength, StringBuffer chars, StringBuffer extension)
		{
			this.Strength_Renamed = strength;
			this.Chars_Renamed = chars.ToString();
			this.Extension_Renamed = (extension.Length() > 0) ? extension.ToString() : "";
		}

		internal class Parser
		{
			internal String Pattern;
			internal int i;

			public Parser(String pattern)
			{
				this.Pattern = pattern;
				this.i = 0;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PatternEntry next() throws ParseException
			public virtual PatternEntry Next()
			{
				int newStrength = UNSET;

				NewChars.Length = 0;
				NewExtension.Length = 0;

				bool inChars = true;
				bool inQuote = false;
				while (i < Pattern.Length())
				{
					char ch = Pattern.CharAt(i);
					if (inQuote)
					{
						if (ch == '\'')
						{
							inQuote = false;
						}
						else
						{
							if (NewChars.Length() == 0)
							{
								NewChars.Append(ch);
							}
							else if (inChars)
							{
								NewChars.Append(ch);
							}
							else
							{
								NewExtension.Append(ch);
							}
						}
					}
					else
					{
						switch (ch)
						{
					case '=':
						if (newStrength != UNSET)
						{
							goto mainLoopBreak;
						}
						newStrength = Collator.IDENTICAL;
						break;
					case ',':
						if (newStrength != UNSET)
						{
							goto mainLoopBreak;
						}
						newStrength = Collator.TERTIARY;
						break;
					case ';':
						if (newStrength != UNSET)
						{
							goto mainLoopBreak;
						}
						newStrength = Collator.SECONDARY;
						break;
					case '<':
						if (newStrength != UNSET)
						{
							goto mainLoopBreak;
						}
						newStrength = Collator.PRIMARY;
						break;
					case '&':
						if (newStrength != UNSET)
						{
							goto mainLoopBreak;
						}
						newStrength = RESET;
						break;
					case '\t':
					case '\n':
					case '\f':
					case '\r':
					case ' ': // skip whitespace TODO use Character
						break;
					case '/':
						inChars = false;
						break;
					case '\'':
						inQuote = true;
						ch = Pattern.CharAt(++i);
						if (NewChars.Length() == 0)
						{
							NewChars.Append(ch);
						}
						else if (inChars)
						{
							NewChars.Append(ch);
						}
						else
						{
							NewExtension.Append(ch);
						}
						break;
					default:
						if (newStrength == UNSET)
						{
							throw new ParseException("missing char (=,;<&) : " + Pattern.Substring(i, (10 < Pattern.Length()) ? i + 10 : Pattern.Length()), i);
						}
						if (PatternEntry.IsSpecialChar(ch) && (inQuote == false))
						{
							throw new ParseException("Unquoted punctuation character : " + Convert.ToString(ch, 16), i);
						}
						if (inChars)
						{
							NewChars.Append(ch);
						}
						else
						{
							NewExtension.Append(ch);
						}
						break;
						}
					}
					i++;
				mainLoopContinue:;
				}
			mainLoopBreak:
				if (newStrength == UNSET)
				{
					return null;
				}
				if (NewChars.Length() == 0)
				{
					throw new ParseException("missing chars (=,;<&): " + Pattern.Substring(i, (10 < Pattern.Length()) ? i + 10 : Pattern.Length()), i);
				}

				return new PatternEntry(newStrength, NewChars, NewExtension);
			}

			// We re-use these objects in order to improve performance
			internal StringBuffer NewChars = new StringBuffer();
			internal StringBuffer NewExtension = new StringBuffer();

		}

		internal static bool IsSpecialChar(char ch)
		{
			return ((ch == '\u0020') || ((ch <= '\u002F') && (ch >= '\u0022')) || ((ch <= '\u003F') && (ch >= '\u003A')) || ((ch <= '\u0060') && (ch >= '\u005B')) || ((ch <= '\u007E') && (ch >= '\u007B')));
		}


		internal const int RESET = -2;
		internal const int UNSET = -1;

		internal int Strength_Renamed = UNSET;
		internal String Chars_Renamed = "";
		internal String Extension_Renamed = "";
	}

}