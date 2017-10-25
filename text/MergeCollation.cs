using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2012, Oracle and/or its affiliates. All rights reserved.
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
	/// Patterns are strings of the form <entry>*, where <entry> has the
	/// form:
	/// <pattern> := <entry>*
	/// <entry> := <separator><chars>{"/"<extension>}
	/// <separator> := "=", ",", ";", "<", "&"
	/// <chars>, and <extension> are both arbitrary strings.
	/// unquoted whitespaces are ignored.
	/// 'xxx' can be used to quote characters
	/// One difference from Collator is that & is used to reset to a current
	/// point. Or, in other words, it introduces a new sequence which is to
	/// be added to the old.
	/// That is: "a < b < c < d" is the same as "a < b & b < c & c < d" OR
	/// "a < b < d & b < c"
	/// XXX: make '' be a single quote. </summary>
	/// <seealso cref= PatternEntry
	/// @author             Mark Davis, Helena Shih </seealso>

	internal sealed class MergeCollation
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			BYTEMASK = (1 << BYTEPOWER) - 1;
		}


		/// <summary>
		/// Creates from a pattern </summary>
		/// <exception cref="ParseException"> If the input pattern is incorrect. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MergeCollation(String pattern) throws ParseException
		public MergeCollation(String pattern)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			for (int i = 0; i < StatusArray.Length; i++)
			{
				StatusArray[i] = 0;
			}
			Pattern = pattern;
		}

		/// <summary>
		/// recovers current pattern
		/// </summary>
		public String Pattern
		{
			get
			{
				return GetPattern(true);
			}
			set
			{
				Patterns.Clear();
				AddPattern(value);
			}
		}

		/// <summary>
		/// recovers current pattern. </summary>
		/// <param name="withWhiteSpace"> puts spacing around the entries, and \n
		/// before & and < </param>
		public String GetPattern(bool withWhiteSpace)
		{
			StringBuffer result = new StringBuffer();
			PatternEntry tmp = null;
			List<PatternEntry> extList = null;
			int i;
			for (i = 0; i < Patterns.Count; ++i)
			{
				PatternEntry entry = Patterns[i];
				if (entry.Extension_Renamed.length() != 0)
				{
					if (extList == null)
					{
						extList = new List<>();
					}
					extList.Add(entry);
				}
				else
				{
					if (extList != null)
					{
						PatternEntry last = FindLastWithNoExtension(i - 1);
						for (int j = extList.Count - 1; j >= 0 ; j--)
						{
							tmp = extList[j];
							tmp.AddToBuffer(result, false, withWhiteSpace, last);
						}
						extList = null;
					}
					entry.AddToBuffer(result, false, withWhiteSpace, null);
				}
			}
			if (extList != null)
			{
				PatternEntry last = FindLastWithNoExtension(i - 1);
				for (int j = extList.Count - 1; j >= 0 ; j--)
				{
					tmp = extList[j];
					tmp.AddToBuffer(result, false, withWhiteSpace, last);
				}
				extList = null;
			}
			return result.ToString();
		}

		private PatternEntry FindLastWithNoExtension(int i)
		{
			for (--i;i >= 0; --i)
			{
				PatternEntry entry = Patterns[i];
				if (entry.Extension_Renamed.length() == 0)
				{
					return entry;
				}
			}
			return null;
		}

		/// <summary>
		/// emits the pattern for collation builder. </summary>
		/// <returns> emits the string in the format understable to the collation
		/// builder. </returns>
		public String EmitPattern()
		{
			return EmitPattern(true);
		}

		/// <summary>
		/// emits the pattern for collation builder. </summary>
		/// <param name="withWhiteSpace"> puts spacing around the entries, and \n
		/// before & and < </param>
		/// <returns> emits the string in the format understable to the collation
		/// builder. </returns>
		public String EmitPattern(bool withWhiteSpace)
		{
			StringBuffer result = new StringBuffer();
			for (int i = 0; i < Patterns.Count; ++i)
			{
				PatternEntry entry = Patterns[i];
				if (entry != null)
				{
					entry.AddToBuffer(result, true, withWhiteSpace, null);
				}
			}
			return result.ToString();
		}


		/// <summary>
		/// adds a pattern to the current one. </summary>
		/// <param name="pattern"> the new pattern to be added </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addPattern(String pattern) throws ParseException
		public void AddPattern(String pattern)
		{
			if (pattern == null)
			{
				return;
			}

			PatternEntry.Parser parser = new PatternEntry.Parser(pattern);

			PatternEntry entry = parser.Next();
			while (entry != null)
			{
				FixEntry(entry);
				entry = parser.Next();
			}
		}

		/// <summary>
		/// gets count of separate entries </summary>
		/// <returns> the size of pattern entries </returns>
		public int Count
		{
			get
			{
				return Patterns.Count;
			}
		}

		/// <summary>
		/// gets count of separate entries </summary>
		/// <param name="index"> the offset of the desired pattern entry </param>
		/// <returns> the requested pattern entry </returns>
		public PatternEntry GetItemAt(int index)
		{
			return Patterns[index];
		}

		//============================================================
		// privates
		//============================================================
		internal List<PatternEntry> Patterns = new List<PatternEntry>(); // a list of PatternEntries

		[NonSerialized]
		private PatternEntry SaveEntry = null;
		[NonSerialized]
		private PatternEntry LastEntry = null;

		// This is really used as a local variable inside fixEntry, but we cache
		// it here to avoid newing it up every time the method is called.
		[NonSerialized]
		private StringBuffer Excess = new StringBuffer();

		//
		// When building a MergeCollation, we need to do lots of searches to see
		// whether a given entry is already in the table.  Since we're using an
		// array, this would make the algorithm O(N*N).  To speed things up, we
		// use this bit array to remember whether the array contains any entries
		// starting with each Unicode character.  If not, we can avoid the search.
		// Using BitSet would make this easier, but it's significantly slower.
		//
		[NonSerialized]
		private sbyte[] StatusArray = new sbyte[8192];
		private readonly sbyte BITARRAYMASK = (sbyte)0x1;
		private readonly int BYTEPOWER = 3;
		private int BYTEMASK;

		/*
		  If the strength is RESET, then just change the lastEntry to
		  be the current. (If the current is not in patterns, signal an error).
		  If not, then remove the current entry, and add it after lastEntry
		  (which is usually at the end).
		  */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private final void fixEntry(PatternEntry newEntry) throws ParseException
		private void FixEntry(PatternEntry newEntry)
		{
			// check to see whether the new entry has the same characters as the previous
			// entry did (this can happen when a pattern declaring a difference between two
			// strings that are canonically equivalent is normalized).  If so, and the strength
			// is anything other than IDENTICAL or RESET, throw an exception (you can't
			// declare a string to be unequal to itself).       --rtg 5/24/99
			if (LastEntry != null && newEntry.Chars_Renamed.Equals(LastEntry.Chars_Renamed) && newEntry.Extension_Renamed.Equals(LastEntry.Extension_Renamed))
			{
				if (newEntry.Strength_Renamed != Collator.IDENTICAL && newEntry.Strength_Renamed != PatternEntry.RESET)
				{
						throw new ParseException("The entries " + LastEntry + " and " + newEntry + " are adjacent in the rules, but have conflicting " + "strengths: A character can't be unequal to itself.", -1);
				}
				else
				{
					// otherwise, just skip this entry and behave as though you never saw it
					return;
				}
			}

			bool changeLastEntry = true;
			if (newEntry.Strength_Renamed != PatternEntry.RESET)
			{
				int oldIndex = -1;

				if ((newEntry.Chars_Renamed.length() == 1))
				{

					char c = newEntry.Chars_Renamed.charAt(0);
					int statusIndex = c >> BYTEPOWER;
					sbyte bitClump = StatusArray[statusIndex];
					sbyte setBit = (sbyte)(BITARRAYMASK << (c & BYTEMASK));

					if (bitClump != 0 && (bitClump & setBit) != 0)
					{
						oldIndex = Patterns.LastIndexOf(newEntry);
					}
					else
					{
						// We're going to add an element that starts with this
						// character, so go ahead and set its bit.
						StatusArray[statusIndex] = (sbyte)(bitClump | setBit);
					}
				}
				else
				{
					oldIndex = Patterns.LastIndexOf(newEntry);
				}
				if (oldIndex != -1)
				{
					Patterns.RemoveAt(oldIndex);
				}

				Excess.Length = 0;
				int lastIndex = FindLastEntry(LastEntry, Excess);

				if (Excess.Length() != 0)
				{
					newEntry.Extension_Renamed = Excess + newEntry.Extension_Renamed;
					if (lastIndex != Patterns.Count)
					{
						LastEntry = SaveEntry;
						changeLastEntry = false;
					}
				}
				if (lastIndex == Patterns.Count)
				{
					Patterns.Add(newEntry);
					SaveEntry = newEntry;
				}
				else
				{
					Patterns.Insert(lastIndex, newEntry);
				}
			}
			if (changeLastEntry)
			{
				LastEntry = newEntry;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private final int findLastEntry(PatternEntry entry, StringBuffer excessChars) throws ParseException
		private int FindLastEntry(PatternEntry entry, StringBuffer excessChars)
		{
			if (entry == null)
			{
				return 0;
			}

			if (entry.Strength_Renamed != PatternEntry.RESET)
			{
				// Search backwards for string that contains this one;
				// most likely entry is last one

				int oldIndex = -1;
				if ((entry.Chars_Renamed.length() == 1))
				{
					int index = entry.Chars_Renamed.charAt(0) >> BYTEPOWER;
					if ((StatusArray[index] & (BITARRAYMASK << (entry.Chars_Renamed.charAt(0) & BYTEMASK))) != 0)
					{
						oldIndex = Patterns.LastIndexOf(entry);
					}
				}
				else
				{
					oldIndex = Patterns.LastIndexOf(entry);
				}
				if ((oldIndex == -1))
				{
					throw new ParseException("couldn't find last entry: " + entry, oldIndex);
				}
				return oldIndex + 1;
			}
			else
			{
				int i;
				for (i = Patterns.Count - 1; i >= 0; --i)
				{
					PatternEntry e = Patterns[i];
					if (e.Chars_Renamed.regionMatches(0,entry.Chars_Renamed,0, e.Chars_Renamed.length()))
					{
						excessChars.Append(StringHelperClass.SubstringSpecial(entry.Chars_Renamed, e.Chars_Renamed.length(), entry.Chars_Renamed.length()));
						break;
					}
				}
				if (i == -1)
				{
					throw new ParseException("couldn't find: " + entry, i);
				}
				return i + 1;
			}
		}
	}

}