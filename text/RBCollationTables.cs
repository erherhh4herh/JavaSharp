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

	/// <summary>
	/// This class contains the static state of a RuleBasedCollator: The various
	/// tables that are used by the collation routines.  Several RuleBasedCollators
	/// can share a single RBCollationTables object, easing memory requirements and
	/// improving performance.
	/// </summary>
	internal sealed class RBCollationTables
	{
		//===========================================================================================
		//  The following diagram shows the data structure of the RBCollationTables object.
		//  Suppose we have the rule, where 'o-umlaut' is the unicode char 0x00F6.
		//  "a, A < b, B < c, C, ch, cH, Ch, CH < d, D ... < o, O; 'o-umlaut'/E, 'O-umlaut'/E ...".
		//  What the rule says is, sorts 'ch'ligatures and 'c' only with tertiary difference and
		//  sorts 'o-umlaut' as if it's always expanded with 'e'.
		//
		// mapping table                     contracting list           expanding list
		// (contains all unicode char
		//  entries)                   ___    ____________       _________________________
		//  ________                +>|_*_|->|'c' |v('c') |  +>|v('o')|v('umlaut')|v('e')|
		// |_\u0001_|-> v('\u0001') | |_:_|  |------------|  | |-------------------------|
		// |_\u0002_|-> v('\u0002') | |_:_|  |'ch'|v('ch')|  | |             :           |
		// |____:___|               | |_:_|  |------------|  | |-------------------------|
		// |____:___|               |        |'cH'|v('cH')|  | |             :           |
		// |__'a'___|-> v('a')      |        |------------|  | |-------------------------|
		// |__'b'___|-> v('b')      |        |'Ch'|v('Ch')|  | |             :           |
		// |____:___|               |        |------------|  | |-------------------------|
		// |____:___|               |        |'CH'|v('CH')|  | |             :           |
		// |___'c'__|----------------         ------------   | |-------------------------|
		// |____:___|                                        | |             :           |
		// |o-umlaut|----------------------------------------  |_________________________|
		// |____:___|
		//
		// Noted by Helena Shih on 6/23/97
		//============================================================================================

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RBCollationTables(String rules, int decmp) throws ParseException
		public RBCollationTables(String rules, int decmp)
		{
			this.Rules_Renamed = rules;

			RBTableBuilder builder = new RBTableBuilder(new BuildAPI(this));
			builder.Build(rules, decmp); // this object is filled in through
												// the BuildAPI object
		}

		internal sealed class BuildAPI
		{
			private readonly RBCollationTables OuterInstance;

			/// <summary>
			/// Private constructor.  Prevents anyone else besides RBTableBuilder
			/// from gaining direct access to the internals of this class.
			/// </summary>
			internal BuildAPI(RBCollationTables outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			/// <summary>
			/// This function is used by RBTableBuilder to fill in all the members of this
			/// object.  (Effectively, the builder class functions as a "friend" of this
			/// class, but to avoid changing too much of the logic, it carries around "shadow"
			/// copies of all these variables until the end of the build process and then
			/// copies them en masse into the actual tables object once all the construction
			/// logic is complete.  This function does that "copying en masse". </summary>
			/// <param name="f2ary"> The value for frenchSec (the French-secondary flag) </param>
			/// <param name="swap"> The value for SE Asian swapping rule </param>
			/// <param name="map"> The collator's character-mapping table (the value for mapping) </param>
			/// <param name="cTbl"> The collator's contracting-character table (the value for contractTable) </param>
			/// <param name="eTbl"> The collator's expanding-character table (the value for expandTable) </param>
			/// <param name="cFlgs"> The hash table of characters that participate in contracting-
			///              character sequences (the value for contractFlags) </param>
			/// <param name="mso"> The value for maxSecOrder </param>
			/// <param name="mto"> The value for maxTerOrder </param>
			internal void FillInTables(bool f2ary, bool swap, UCompactIntArray map, List<List<EntryPair>> cTbl, List<int[]> eTbl, IntHashtable cFlgs, short mso, short mto)
			{
				outerInstance.FrenchSec_Renamed = f2ary;
				outerInstance.SeAsianSwapping = swap;
				outerInstance.Mapping = map;
				outerInstance.ContractTable = cTbl;
				outerInstance.ExpandTable = eTbl;
				outerInstance.ContractFlags = cFlgs;
				outerInstance.MaxSecOrder_Renamed = mso;
				outerInstance.MaxTerOrder_Renamed = mto;
			}
		}

		/// <summary>
		/// Gets the table-based rules for the collation object. </summary>
		/// <returns> returns the collation rules that the table collation object
		/// was created from. </returns>
		public String Rules
		{
			get
			{
				return Rules_Renamed;
			}
		}

		public bool FrenchSec
		{
			get
			{
				return FrenchSec_Renamed;
			}
		}

		public bool SEAsianSwapping
		{
			get
			{
				return SeAsianSwapping;
			}
		}

		// ==============================================================
		// internal (for use by CollationElementIterator)
		// ==============================================================

		/// <summary>
		///  Get the entry of hash table of the contracting string in the collation
		///  table. </summary>
		///  <param name="ch"> the starting character of the contracting string </param>
		internal List<EntryPair> GetContractValues(int ch)
		{
			int index = Mapping.elementAt(ch);
			return GetContractValuesImpl(index - CONTRACTCHARINDEX);
		}

		//get contract values from contractTable by index
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
		/// Returns true if this character appears anywhere in a contracting
		/// character sequence.  (Used by CollationElementIterator.setOffset().)
		/// </summary>
		internal bool UsedInContractSeq(int c)
		{
			return ContractFlags.get(c) == 1;
		}

		/// <summary>
		/// Return the maximum length of any expansion sequences that end
		/// with the specified comparison order.
		/// </summary>
		/// <param name="order"> a collation order returned by previous or next. </param>
		/// <returns> the maximum length of any expansion seuences ending
		///         with the specified order.
		/// </returns>
		/// <seealso cref= CollationElementIterator#getMaxExpansion </seealso>
		internal int GetMaxExpansion(int order)
		{
			int result = 1;

			if (ExpandTable != null)
			{
				// Right now this does a linear search through the entire
				// expansion table.  If a collator had a large number of expansions,
				// this could cause a performance problem, but in practise that
				// rarely happens
				for (int i = 0; i < ExpandTable.Count; i++)
				{
					int[] valueList = ExpandTable[i];
					int length = valueList.Length;

					if (length > result && valueList[length - 1] == order)
					{
						result = length;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get the entry of hash table of the expanding string in the collation
		/// table. </summary>
		/// <param name="idx"> the index of the expanding string value list </param>
		internal int[] GetExpandValueList(int idx)
		{
			return ExpandTable[idx - EXPANDCHARINDEX];
		}

		/// <summary>
		/// Get the comarison order of a character from the collation table. </summary>
		/// <returns> the comparison order of a character. </returns>
		internal int GetUnicodeOrder(int ch)
		{
			return Mapping.elementAt(ch);
		}

		internal short MaxSecOrder
		{
			get
			{
				return MaxSecOrder_Renamed;
			}
		}

		internal short MaxTerOrder
		{
			get
			{
				return MaxTerOrder_Renamed;
			}
		}

		/// <summary>
		/// Reverse a string.
		/// </summary>
		//shemran/Note: this is used for secondary order value reverse, no
		//              need to consider supplementary pair.
		internal static void Reverse(StringBuffer result, int from, int to)
		{
			int i = from;
			char swap;

			int j = to - 1;
			while (i < j)
			{
				swap = result.CharAt(i);
				result.SetCharAt(i, result.CharAt(j));
				result.SetCharAt(j, swap);
				i++;
				j--;
			}
		}

		internal static int GetEntry(List<EntryPair> list, String name, bool fwd)
		{
			for (int i = 0; i < list.Count; i++)
			{
				EntryPair pair = list[i];
				if (pair.Fwd == fwd && pair.EntryName.Equals(name))
				{
					return i;
				}
			}
			return UNMAPPED;
		}

		// ==============================================================
		// constants
		// ==============================================================
		//sherman/Todo: is the value big enough?????
		internal const int EXPANDCHARINDEX = 0x7E000000; // Expand index follows
		internal const int CONTRACTCHARINDEX = 0x7F000000; // contract indexes follow
		internal const int UNMAPPED = unchecked((int)0xFFFFFFFF);

		internal const int PRIMARYORDERMASK = unchecked((int)0xffff0000);
		internal const int SECONDARYORDERMASK = 0x0000ff00;
		internal const int TERTIARYORDERMASK = 0x000000ff;
		internal const int PRIMARYDIFFERENCEONLY = unchecked((int)0xffff0000);
		internal const int SECONDARYDIFFERENCEONLY = unchecked((int)0xffffff00);
		internal const int PRIMARYORDERSHIFT = 16;
		internal const int SECONDARYORDERSHIFT = 8;

		// ==============================================================
		// instance variables
		// ==============================================================
		private String Rules_Renamed = null;
		private bool FrenchSec_Renamed = false;
		private bool SeAsianSwapping = false;

		private UCompactIntArray Mapping = null;
		private List<List<EntryPair>> ContractTable = null;
		private List<int[]> ExpandTable = null;
		private IntHashtable ContractFlags = null;

		private short MaxSecOrder_Renamed = 0;
		private short MaxTerOrder_Renamed = 0;
	}

}