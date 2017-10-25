using System.Collections.Generic;

/*
 * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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


	internal sealed class UnicodeProp
	{

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		ALPHABETIC
		{
			public boolean @is(int ch)
			{
				return Character.IsAlphabetic(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		LETTER
		{
			public boolean @is(int ch)
			{
				return char.IsLetter(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		IDEOGRAPHIC
		{
			public boolean @is(int ch)
			{
				return Character.IsIdeographic(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		LOWERCASE
		{
			public boolean @is(int ch)
			{
				return char.IsLower(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		UPPERCASE
		{
			public boolean @is(int ch)
			{
				return char.IsUpper(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		TITLECASE
		{
			public boolean @is(int ch)
			{
				return Character.IsTitleCase(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		WHITE_SPACE
		{
			// \p{Whitespace}
			public boolean @is(int ch)
			{
				return ((((1 << Character.SPACE_SEPARATOR) | (1 << Character.LINE_SEPARATOR) | (1 << Character.PARAGRAPH_SEPARATOR)) >> Character.GetType(ch)) & 1) != 0 || (ch >= 0x9 && ch <= 0xd) || (ch == 0x85);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		CONTROL
		{
			// \p{gc=Control}
			public boolean @is(int ch)
			{
				return Character.GetType(ch) == Character.CONTROL;
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		PUNCTUATION
		{
			// \p{gc=Punctuation}
			public boolean @is(int ch)
			{
				return ((((1 << Character.CONNECTOR_PUNCTUATION) | (1 << Character.DASH_PUNCTUATION) | (1 << Character.START_PUNCTUATION) | (1 << Character.END_PUNCTUATION) | (1 << Character.OTHER_PUNCTUATION) | (1 << Character.INITIAL_QUOTE_PUNCTUATION) | (1 << Character.FINAL_QUOTE_PUNCTUATION)) >> Character.GetType(ch)) & 1) != 0;
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		HEX_DIGIT
		{
			// \p{gc=Decimal_Number}
			// \p{Hex_Digit}    -> PropList.txt: Hex_Digit
			public boolean @is(int ch)
			{
				return DIGIT.@is(ch) || (ch >= 0x0030 && ch <= 0x0039) || (ch >= 0x0041 && ch <= 0x0046) || (ch >= 0x0061 && ch <= 0x0066) || (ch >= 0xFF10 && ch <= 0xFF19) || (ch >= 0xFF21 && ch <= 0xFF26) || (ch >= 0xFF41 && ch <= 0xFF46);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		ASSIGNED
		{
			public boolean @is(int ch)
			{
				return Character.GetType(ch) != Character.UNASSIGNED;
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		NONCHARACTER_CODE_POINT
		{
			// PropList.txt:Noncharacter_Code_Point
			public boolean @is(int ch)
			{
				return (ch & 0xfffe) == 0xfffe || (ch >= 0xfdd0 && ch <= 0xfdef);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		DIGIT
		{
			// \p{gc=Decimal_Number}
			public boolean @is(int ch)
			{
				return char.IsDigit(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		ALNUM
		{
			// \p{alpha}
			// \p{digit}
			public boolean @is(int ch)
			{
				return ALPHABETIC.@is(ch) || DIGIT.@is(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		BLANK
		{
			// \p{Whitespace} --
			// [\N{LF} \N{VT} \N{FF} \N{CR} \N{NEL}  -> 0xa, 0xb, 0xc, 0xd, 0x85
			//  \p{gc=Line_Separator}
			//  \p{gc=Paragraph_Separator}]
			public boolean @is(int ch)
			{
				return Character.GetType(ch) == Character.SPACE_SEPARATOR || ch == 0x9; // \N{HT}
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		GRAPH
		{
			// [^
			//  \p{space}
			//  \p{gc=Control}
			//  \p{gc=Surrogate}
			//  \p{gc=Unassigned}]
			public boolean @is(int ch)
			{
				return ((((1 << Character.SPACE_SEPARATOR) | (1 << Character.LINE_SEPARATOR) | (1 << Character.PARAGRAPH_SEPARATOR) | (1 << Character.CONTROL) | (1 << Character.SURROGATE) | (1 << Character.UNASSIGNED)) >> Character.GetType(ch)) & 1) == 0;
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		PRINT
		{
			// \p{graph}
			// \p{blank}
			// -- \p{cntrl}
			public boolean @is(int ch)
			{
				return (GRAPH.@is(ch) || BLANK.@is(ch)) && !CONTROL.@is(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		WORD
		{
			//  \p{alpha}
			//  \p{gc=Mark}
			//  \p{digit}
			//  \p{gc=Connector_Punctuation}
			//  \p{Join_Control}    200C..200D

			public boolean @is(int ch)
			{
				return ALPHABETIC.@is(ch) || ((((1 << Character.NON_SPACING_MARK) | (1 << Character.ENCLOSING_MARK) | (1 << Character.COMBINING_SPACING_MARK) | (1 << Character.DECIMAL_DIGIT_NUMBER) | (1 << Character.CONNECTOR_PUNCTUATION)) >> Character.GetType(ch)) & 1) != 0 || JOIN_CONTROL.@is(ch);
			}
		},

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		JOIN_CONTROL
		{
			//  200C..200D    PropList.txt:Join_Control
			public boolean @is(int ch)
			{
			   return (ch == 0x200C || ch == 0x200D);
			}
		}

		private static readonly Dictionary<String, String> posix = new java.util.HashMap<String, String>();
		private static readonly Dictionary<String, String> aliases = new java.util.HashMap<String, String>();
		static UnicodeProp()
		{
			posix.put("ALPHA", "ALPHABETIC");
			posix.put("LOWER", "LOWERCASE");
			posix.put("UPPER", "UPPERCASE");
			posix.put("SPACE", "WHITE_SPACE");
			posix.put("PUNCT", "PUNCTUATION");
			posix.put("XDIGIT","HEX_DIGIT");
			posix.put("ALNUM", "ALNUM");
			posix.put("CNTRL", "CONTROL");
			posix.put("DIGIT", "DIGIT");
			posix.put("BLANK", "BLANK");
			posix.put("GRAPH", "GRAPH");
			posix.put("PRINT", "PRINT");

			aliases.put("WHITESPACE", "WHITE_SPACE");
			aliases.put("HEXDIGIT","HEX_DIGIT");
			aliases.put("NONCHARACTERCODEPOINT", "NONCHARACTER_CODE_POINT");
			aliases.put("JOINCONTROL", "JOIN_CONTROL");

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public static UnicodeProp ForName(String propName)
		{
			propName = propName.ToUpperCase(Locale.ENGLISH);
			String alias = aliases.get(propName);
			if (alias != null)
			{
				propName = alias;
			}
			try
			{
				return valueOf(propName);
			}
			catch (IllegalArgumentException)
			{
			}
			return null;
		}

		public static UnicodeProp ForPOSIXName(String propName)
		{
			propName = posix.get(propName.ToUpperCase(Locale.ENGLISH));
			if (propName == null)
			{
				return null;
			}
			return valueOf(propName);
		}

		public = int ch
		public static readonly UnicodeProp public abstract boolean @is = new UnicodeProp("public abstract boolean @is", InnerEnum.public abstract boolean @is, int ch);

		private static readonly IList<UnicodeProp> valueList = new List<UnicodeProp>();

		static UnicodeProp()
		{
			valueList.Add(public abstract boolean @is);
		}

		public enum InnerEnum
		{
			public abstract boolean @is
		}

		private readonly string nameValue;
		private readonly int ordinalValue;
		private readonly InnerEnum innerEnumValue;
		private static int nextOrdinal = 0;

		public static IList<UnicodeProp> values()
		{
			return valueList;
		}

		public InnerEnum InnerEnumValue()
		{
			return innerEnumValue;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static UnicodeProp valueOf(string name)
		{
			foreach (UnicodeProp enumInstance in UnicodeProp.values())
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}