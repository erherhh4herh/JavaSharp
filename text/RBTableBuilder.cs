using System.Collections.Generic;

/*
 * Copyright (c) 1999, 2012, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright IBM Corp. 1996-1998 - All Rights Reserved
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

	using UCompactIntArray = sun.text.UCompactIntArray;
	using IntHashtable = sun.text.IntHashtable;
	using ComposedCharIter = sun.text.ComposedCharIter;
	using CollatorUtilities = sun.text.CollatorUtilities;
	using NormalizerImpl = sun.text.normalizer.NormalizerImpl;

	/// <summary>
	/// This class contains all the code to parse a RuleBasedCollator pattern
	/// and build a RBCollationTables object from it.  A particular instance
	/// of tis class exists only during the actual build process-- once an
	/// RBCollationTables object has been built, the RBTableBuilder object
	/// goes away.  This object carries all of the state which is only needed
	/// during the build process, plus a "shadow" copy of all of the state
	/// that will go into the tables object itself.  This object communicates
	/// with RBCollationTables through a separate class, RBCollationTables.BuildAPI,
	/// this is an inner class of RBCollationTables and provides a separate
	/// private API for communication with RBTableBuilder.
	/// This class isn't just an inner class of RBCollationTables itself because
	/// of its large size.  For source-code readability, it seemed better for the
	/// builder to have its own source file.
	/// </summary>
	internal sealed class RBTableBuilder
	{

		public RBTableBuilder(RBCollationTables.BuildAPI tables)
		{
			this.Tables = tables;
		}

		/// <summary>
		/// Create a table-based collation object with the given rules.
		/// This is the main function that actually builds the tables and
		/// stores them back in the RBCollationTables object.  It is called
		/// ONLY by the RBCollationTables constructor. </summary>
		/// <seealso cref= RuleBasedCollator#RuleBasedCollator </seealso>
		/// <exception cref="ParseException"> If the rules format is incorrect. </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void build(String pattern, int decmp) throws ParseException
		public void Build(String pattern, int decmp)
		{
			bool isSource = true;
			int i = 0;
			String expChars;
			String groupChars;
			if (pattern.Length() == 0)
			{
				throw new ParseException("Build rules empty.", 0);
			}

			// This array maps Unicode characters to their collation ordering
			Mapping = new UCompactIntArray(RBCollationTables.UNMAPPED);
			// Normalize the build rules.  Find occurances of all decomposed characters
			// and normalize the rules before feeding into the builder.  By "normalize",
			// we mean that all precomposed Unicode characters must be converted into
			// a base character and one or more combining characters (such as accents).
			// When there are multiple combining characters attached to a base character,
			// the combining characters must be in their canonical order
			//
			// sherman/Note:
			//(1)decmp will be NO_DECOMPOSITION only in ko locale to prevent decompose
			//hangual syllables to jamos, so we can actually just call decompose with
			//normalizer's IGNORE_HANGUL option turned on
			//
			//(2)just call the "special version" in NormalizerImpl directly
			//pattern = Normalizer.decompose(pattern, false, Normalizer.IGNORE_HANGUL, true);
			//
			//Normalizer.Mode mode = CollatorUtilities.toNormalizerMode(decmp);
			//pattern = Normalizer.normalize(pattern, mode, 0, true);

			pattern = NormalizerImpl.canonicalDecomposeWithSingleQuotation(pattern);

			// Build the merged collation entries
			// Since rules can be specified in any order in the string
			// (e.g. "c , C < d , D < e , E .... C < CH")
			// this splits all of the rules in the string out into separate
			// objects and then sorts them.  In the above example, it merges the
			// "C < CH" rule in just before the "C < D" rule.
			//

			MPattern = new MergeCollation(pattern);

			int order = 0;

			// Now walk though each entry and add it to my own tables
			for (i = 0; i < MPattern.Count; ++i)
			{
				PatternEntry entry = MPattern.GetItemAt(i);
				if (entry != null)
				{
					groupChars = entry.Chars;
					if (groupChars.Length() > 1)
					{
						switch (groupChars.CharAt(groupChars.Length() - 1))
						{
						case '@':
							FrenchSec = true;
							groupChars = groupChars.Substring(0, groupChars.Length() - 1);
							break;
						case '!':
							SeAsianSwapping = true;
							groupChars = groupChars.Substring(0, groupChars.Length() - 1);
							break;
						}
					}

					order = Increment(entry.Strength, order);
					expChars = entry.Extension;

					if (expChars.Length() != 0)
					{
						AddExpandOrder(groupChars, expChars, order);
					}
					else if (groupChars.Length() > 1)
					{
						char ch = groupChars.CharAt(0);
						if (char.IsHighSurrogate(ch) && groupChars.Length() == 2)
						{
							AddOrder(Character.ToCodePoint(ch, groupChars.CharAt(1)), order);
						}
						else
						{
							AddContractOrder(groupChars, order);
						}
					}
					else
					{
						char ch = groupChars.CharAt(0);
						AddOrder(ch, order);
					}
				}
			}
			AddComposedChars();

			Commit();
			Mapping.compact();
			/*
			System.out.println("mappingSize=" + mapping.getKSize());
			for (int j = 0; j < 0xffff; j++) {
			    int value = mapping.elementAt(j);
			    if (value != RBCollationTables.UNMAPPED)
			        System.out.println("index=" + Integer.toString(j, 16)
			                   + ", value=" + Integer.toString(value, 16));
			}
			*/
			Tables.FillInTables(FrenchSec, SeAsianSwapping, Mapping, ContractTable, ExpandTable, ContractFlags, MaxSecOrder, MaxTerOrder);
		}

		/// <summary>
		/// Add expanding entries for pre-composed unicode characters so that this
		/// collator can be used reasonably well with decomposition turned off.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void addComposedChars() throws ParseException
		private void AddComposedChars()
		{
			// Iterate through all of the pre-composed characters in Unicode
			ComposedCharIter iter = new ComposedCharIter();
			int c;
			while ((c = iter.next()) != ComposedCharIter.DONE)
			{
				if (GetCharOrder(c) == RBCollationTables.UNMAPPED)
				{
					//
					// We don't already have an ordering for this pre-composed character.
					//
					// First, see if the decomposed string is already in our
					// tables as a single contracting-string ordering.
					// If so, just map the precomposed character to that order.
					//
					// TODO: What we should really be doing here is trying to find the
					// longest initial substring of the decomposition that is present
					// in the tables as a contracting character sequence, and find its
					// ordering.  Then do this recursively with the remaining chars
					// so that we build a list of orderings, and add that list to
					// the expansion table.
					// That would be more correct but also significantly slower, so
					// I'm not totally sure it's worth doing.
					//
					String s = iter.decomposition();

					//sherman/Note: if this is 1 character decomposed string, the
					//only thing need to do is to check if this decomposed character
					//has an entry in our order table, this order is not necessary
					//to be a contraction order, if it does have one, add an entry
					//for the precomposed character by using the same order, the
					//previous impl unnecessarily adds a single character expansion
					//entry.
					if (s.Length() == 1)
					{
						int order = GetCharOrder(s.CharAt(0));
						if (order != RBCollationTables.UNMAPPED)
						{
							AddOrder(c, order);
						}
						continue;
					}
					else if (s.Length() == 2)
					{
						char ch0 = s.CharAt(0);
						if (char.IsHighSurrogate(ch0))
						{
							int order = GetCharOrder(s.CodePointAt(0));
							if (order != RBCollationTables.UNMAPPED)
							{
								AddOrder(c, order);
							}
							continue;
						}
					}
					int contractOrder = GetContractOrder(s);
					if (contractOrder != RBCollationTables.UNMAPPED)
					{
						AddOrder(c, contractOrder);
					}
					else
					{
						//
						// We don't have a contracting ordering for the entire string
						// that results from the decomposition, but if we have orders
						// for each individual character, we can add an expanding
						// table entry for the pre-composed character
						//
						bool allThere = true;
						for (int i = 0; i < s.Length(); i++)
						{
							if (GetCharOrder(s.CharAt(i)) == RBCollationTables.UNMAPPED)
							{
								allThere = false;
								break;
							}
						}
						if (allThere)
						{
							AddExpandOrder(c, s, RBCollationTables.UNMAPPED);
						}
					}
				}
			}
		}

		/// <summary>
		/// Look up for unmapped values in the expanded character table.
		/// 
		/// When the expanding character tables are built by addExpandOrder,
		/// it doesn't know what the final ordering of each character
		/// in the expansion will be.  Instead, it just puts the raw character
		/// code into the table, adding CHARINDEX as a flag.  Now that we've
		/// finished building the mapping table, we can go back and look up
		/// that character to see what its real collation order is and
		/// stick that into the expansion table.  That lets us avoid doing
		/// a two-stage lookup later.
		/// </summary>
		private void Commit()
		{
			if (ExpandTable != null)
			{
				for (int i = 0; i < ExpandTable.Count; i++)
				{
					int[] valueList = ExpandTable[i];
					for (int j = 0; j < valueList.Length; j++)
					{
						int order = valueList[j];
						if (order < RBCollationTables.EXPANDCHARINDEX && order > CHARINDEX)
						{
							// found a expanding character that isn't filled in yet
							int ch = order - CHARINDEX;

							// Get the real values for the non-filled entry
							int realValue = GetCharOrder(ch);

							if (realValue == RBCollationTables.UNMAPPED)
							{
								// The real value is still unmapped, maybe it's ignorable
								valueList[j] = IGNORABLEMASK & ch;
							}
							else
							{
								// just fill in the value
								valueList[j] = realValue;
							}
						}
					}
				}
			}
		}
		/// <summary>
		///  Increment of the last order based on the comparison level.
		/// </summary>
		private int Increment(int aStrength, int lastValue)
		{
			switch (aStrength)
			{
			case Collator.PRIMARY:
				// increment priamry order  and mask off secondary and tertiary difference
				lastValue += PRIMARYORDERINCREMENT;
				lastValue &= RBCollationTables.PRIMARYORDERMASK;
				IsOverIgnore = true;
				break;
			case Collator.SECONDARY:
				// increment secondary order and mask off tertiary difference
				lastValue += SECONDARYORDERINCREMENT;
				lastValue &= RBCollationTables.SECONDARYDIFFERENCEONLY;
				// record max # of ignorable chars with secondary difference
				if (!IsOverIgnore)
				{
					MaxSecOrder++;
				}
				break;
			case Collator.TERTIARY:
				// increment tertiary order
				lastValue += TERTIARYORDERINCREMENT;
				// record max # of ignorable chars with tertiary difference
				if (!IsOverIgnore)
				{
					MaxTerOrder++;
				}
				break;
			}
			return lastValue;
		}

		/// <summary>
		///  Adds a character and its designated order into the collation table.
		/// </summary>
		private void AddOrder(int ch, int anOrder)
		{
			// See if the char already has an order in the mapping table
			int order = Mapping.elementAt(ch);

			if (order >= RBCollationTables.CONTRACTCHARINDEX)
			{
				// There's already an entry for this character that points to a contracting
				// character table.  Instead of adding the character directly to the mapping
				// table, we must add it to the contract table instead.
				int length = 1;
				if (Character.IsSupplementaryCodePoint(ch))
				{
					length = Character.ToChars(ch, KeyBuf, 0);
				}
				else
				{
					KeyBuf[0] = (char)ch;
				}
				AddContractOrder(new String(KeyBuf, 0, length), anOrder);
			}
			else
			{
				// add the entry to the mapping table,
				// the same later entry replaces the previous one
				Mapping.setElementAt(ch, anOrder);
			}
		}

		private void AddContractOrder(String groupChars, int anOrder)
		{
			AddContractOrder(groupChars, anOrder, true);
		}

		/// <summary>
		///  Adds the contracting string into the collation table.
		/// </summary>
		private void AddContractOrder(String groupChars, int anOrder, bool fwd)
		{
			if (ContractTable == null)
			{
				ContractTable = new List<>(INITIALTABLESIZE);
			}

			//initial character
			int ch = groupChars.CodePointAt(0);
			/*
			char ch0 = groupChars.charAt(0);
			int ch = Character.isHighSurrogate(ch0)?
			  Character.toCodePoint(ch0, groupChars.charAt(1)):ch0;
			  */
			// See if the initial character of the string already has a contract table.
			int entry = Mapping.elementAt(ch);
			List<EntryPair> entryTable = GetContractValuesImpl(entry - RBCollationTables.CONTRACTCHARINDEX);

			if (entryTable == null)
			{
				// We need to create a new table of contract entries for this base char
				int tableIndex = RBCollationTables.CONTRACTCHARINDEX + ContractTable.Count;
				entryTable = new List<>(INITIALTABLESIZE);
				ContractTable.Add(entryTable);

				// Add the initial character's current ordering first. then
				// update its mapping to point to this contract table
				entryTable.Add(new EntryPair(groupChars.Substring(0,Character.CharCount(ch)), entry));
				Mapping.setElementAt(ch, tableIndex);
			}

			// Now add (or replace) this string in the table
			int index = RBCollationTables.GetEntry(entryTable, groupChars, fwd);
			if (index != RBCollationTables.UNMAPPED)
			{
				EntryPair pair = entryTable[index];
				pair.Value = anOrder;
			}
			else
			{
				EntryPair pair = entryTable[entryTable.Count - 1];

				// NOTE:  This little bit of logic is here to speed CollationElementIterator
				// .nextContractChar().  This code ensures that the longest sequence in
				// this list is always the _last_ one in the list.  This keeps
				// nextContractChar() from having to search the entire list for the longest
				// sequence.
				if (groupChars.Length() > pair.EntryName.Length())
				{
					entryTable.Add(new EntryPair(groupChars, anOrder, fwd));
				}
				else
				{
					entryTable.Insert(entryTable.Count - 1, new EntryPair(groupChars, anOrder, fwd));
				}
			}

			// If this was a forward mapping for a contracting string, also add a
			// reverse mapping for it, so that CollationElementIterator.previous
			// can work right
			if (fwd && groupChars.Length() > 1)
			{
				AddContractFlags(groupChars);
				AddContractOrder((new StringBuffer(groupChars)).Reverse().ToString(), anOrder, false);
			}
		}

		/// <summary>
		/// If the given string has been specified as a contracting string
		/// in this collation table, return its ordering.
		/// Otherwise return UNMAPPED.
		/// </summary>
		private int GetContractOrder(String groupChars)
		{
			int result = RBCollationTables.UNMAPPED;
			if (ContractTable != null)
			{
				int ch = groupChars.CodePointAt(0);
				/*
				char ch0 = groupChars.charAt(0);
				int ch = Character.isHighSurrogate(ch0)?
				  Character.toCodePoint(ch0, groupChars.charAt(1)):ch0;
				  */
				List<EntryPair> entryTable = GetContractValues(ch);
				if (entryTable != null)
				{
					int index = RBCollationTables.GetEntry(entryTable, groupChars, true);
					if (index != RBCollationTables.UNMAPPED)
					{
						EntryPair pair = entryTable[index];
						result = pair.Value;
					}
				}
			}
			return result;
		}

		private int GetCharOrder(int ch)
		{
			int order = Mapping.elementAt(ch);

			if (order >= RBCollationTables.CONTRACTCHARINDEX)
			{
				List<EntryPair> groupList = GetContractValuesImpl(order - RBCollationTables.CONTRACTCHARINDEX);
				EntryPair pair = groupList[0];
				order = pair.Value;
			}
			return order;
		}

		/// <summary>
		///  Get the entry of hash table of the contracting string in the collation
		///  table. </summary>
		///  <param name="ch"> the starting character of the contracting string </param>
		private List<EntryPair> GetContractValues(int ch)
		{
			int index = Mapping.elementAt(ch);
			return GetContractValuesImpl(index - RBCollationTables.CONTRACTCHARINDEX);
		}

		private List<EntryPair> GetContractValuesImpl(int index)
		{
			if (index >= 0)
			{
				return ContractTable[index];
			}
			else // not found
			{
				return null;
			}
		}

		/// <summary>
		///  Adds the expanding string into the collation table.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private final void addExpandOrder(String contractChars, String expandChars, int anOrder) throws ParseException
		private void AddExpandOrder(String contractChars, String expandChars, int anOrder)
		{
			// Create an expansion table entry
			int tableIndex = AddExpansion(anOrder, expandChars);

			// And add its index into the main mapping table
			if (contractChars.Length() > 1)
			{
				char ch = contractChars.CharAt(0);
				if (char.IsHighSurrogate(ch) && contractChars.Length() == 2)
				{
					char ch2 = contractChars.CharAt(1);
					if (char.IsLowSurrogate(ch2))
					{
						//only add into table when it is a legal surrogate
						AddOrder(Character.ToCodePoint(ch, ch2), tableIndex);
					}
				}
				else
				{
					AddContractOrder(contractChars, tableIndex);
				}
			}
			else
			{
				AddOrder(contractChars.CharAt(0), tableIndex);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private final void addExpandOrder(int ch, String expandChars, int anOrder) throws ParseException
		private void AddExpandOrder(int ch, String expandChars, int anOrder)
		{
			int tableIndex = AddExpansion(anOrder, expandChars);
			AddOrder(ch, tableIndex);
		}

		/// <summary>
		/// Create a new entry in the expansion table that contains the orderings
		/// for the given characers.  If anOrder is valid, it is added to the
		/// beginning of the expanded list of orders.
		/// </summary>
		private int AddExpansion(int anOrder, String expandChars)
		{
			if (ExpandTable == null)
			{
				ExpandTable = new List<>(INITIALTABLESIZE);
			}

			// If anOrder is valid, we want to add it at the beginning of the list
			int offset = (anOrder == RBCollationTables.UNMAPPED) ? 0 : 1;

			int[] valueList = new int[expandChars.Length() + offset];
			if (offset == 1)
			{
				valueList[0] = anOrder;
			}

			int j = offset;
			for (int i = 0; i < expandChars.Length(); i++)
			{
				char ch0 = expandChars.CharAt(i);
				char ch1;
				int ch;
				if (char.IsHighSurrogate(ch0))
				{
					if (++i == expandChars.Length() || !char.IsLowSurrogate(ch1 = expandChars.CharAt(i)))
					{
						//ether we are missing the low surrogate or the next char
						//is not a legal low surrogate, so stop loop
						break;
					}
					ch = Character.ToCodePoint(ch0, ch1);

				}
				else
				{
					ch = ch0;
				}

				int mapValue = GetCharOrder(ch);

				if (mapValue != RBCollationTables.UNMAPPED)
				{
					valueList[j++] = mapValue;
				}
				else
				{
					// can't find it in the table, will be filled in by commit().
					valueList[j++] = CHARINDEX + ch;
				}
			}
			if (j < valueList.Length)
			{
				//we had at least one supplementary character, the size of valueList
				//is bigger than it really needs...
				int[] tmpBuf = new int[j];
				while (--j >= 0)
				{
					tmpBuf[j] = valueList[j];
				}
				valueList = tmpBuf;
			}
			// Add the expanding char list into the expansion table.
			int tableIndex = RBCollationTables.EXPANDCHARINDEX + ExpandTable.Count;
			ExpandTable.Add(valueList);

			return tableIndex;
		}

		private void AddContractFlags(String chars)
		{
			char c0;
			int c;
			int len = chars.Length();
			for (int i = 0; i < len; i++)
			{
				c0 = chars.CharAt(i);
				c = char.IsHighSurrogate(c0) ?Character.ToCodePoint(c0, chars.CharAt(++i)) :c0;
				ContractFlags.put(c, 1);
			}
		}

		// ==============================================================
		// constants
		// ==============================================================
		internal const int CHARINDEX = 0x70000000; // need look up in .commit()

		private const int IGNORABLEMASK = 0x0000ffff;
		private const int PRIMARYORDERINCREMENT = 0x00010000;
		private const int SECONDARYORDERINCREMENT = 0x00000100;
		private const int TERTIARYORDERINCREMENT = 0x00000001;
		private const int INITIALTABLESIZE = 20;
		private const int MAXKEYSIZE = 5;

		// ==============================================================
		// instance variables
		// ==============================================================

		// variables used by the build process
		private RBCollationTables.BuildAPI Tables = null;
		private MergeCollation MPattern = null;
		private bool IsOverIgnore = false;
		private char[] KeyBuf = new char[MAXKEYSIZE];
		private IntHashtable ContractFlags = new IntHashtable(100);

		// "shadow" copies of the instance variables in RBCollationTables
		// (the values in these variables are copied back into RBCollationTables
		// at the end of the build process)
		private bool FrenchSec = false;
		private bool SeAsianSwapping = false;

		private UCompactIntArray Mapping = null;
		private List<List<EntryPair>> ContractTable = null;
		private List<int[]> ExpandTable = null;

		private short MaxSecOrder = 0;
		private short MaxTerOrder = 0;
	}

}