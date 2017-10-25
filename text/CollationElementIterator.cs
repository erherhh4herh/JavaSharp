using System.Collections.Generic;

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

	using CollatorUtilities = sun.text.CollatorUtilities;
	using NormalizerBase = sun.text.normalizer.NormalizerBase;

	/// <summary>
	/// The <code>CollationElementIterator</code> class is used as an iterator
	/// to walk through each character of an international string. Use the iterator
	/// to return the ordering priority of the positioned character. The ordering
	/// priority of a character, which we refer to as a key, defines how a character
	/// is collated in the given collation object.
	/// 
	/// <para>
	/// For example, consider the following in Spanish:
	/// <blockquote>
	/// <pre>
	/// "ca" &rarr; the first key is key('c') and second key is key('a').
	/// "cha" &rarr; the first key is key('ch') and second key is key('a').
	/// </pre>
	/// </blockquote>
	/// And in German,
	/// <blockquote>
	/// <pre>
	/// "\u00e4b" &rarr; the first key is key('a'), the second key is key('e'), and
	/// the third key is key('b').
	/// </pre>
	/// </blockquote>
	/// The key of a character is an integer composed of primary order(short),
	/// secondary order(byte), and tertiary order(byte). Java strictly defines
	/// the size and signedness of its primitive data types. Therefore, the static
	/// functions <code>primaryOrder</code>, <code>secondaryOrder</code>, and
	/// <code>tertiaryOrder</code> return <code>int</code>, <code>short</code>,
	/// and <code>short</code> respectively to ensure the correctness of the key
	/// value.
	/// 
	/// </para>
	/// <para>
	/// Example of the iterator usage,
	/// <blockquote>
	/// <pre>
	/// 
	///  String testString = "This is a test";
	///  Collator col = Collator.getInstance();
	///  if (col instanceof RuleBasedCollator) {
	///      RuleBasedCollator ruleBasedCollator = (RuleBasedCollator)col;
	///      CollationElementIterator collationElementIterator = ruleBasedCollator.getCollationElementIterator(testString);
	///      int primaryOrder = CollationElementIterator.primaryOrder(collationElementIterator.next());
	///          :
	///  }
	/// </pre>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>
	/// <code>CollationElementIterator.next</code> returns the collation order
	/// of the next character. A collation order consists of primary order,
	/// secondary order and tertiary order. The data type of the collation
	/// order is <strong>int</strong>. The first 16 bits of a collation order
	/// is its primary order; the next 8 bits is the secondary order and the
	/// last 8 bits is the tertiary order.
	/// 
	/// </para>
	/// <para><b>Note:</b> <code>CollationElementIterator</code> is a part of
	/// <code>RuleBasedCollator</code> implementation. It is only usable
	/// with <code>RuleBasedCollator</code> instances.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=                Collator </seealso>
	/// <seealso cref=                RuleBasedCollator
	/// @author             Helena Shih, Laura Werner, Richard Gillam </seealso>
	public sealed class CollationElementIterator
	{
		/// <summary>
		/// Null order which indicates the end of string is reached by the
		/// cursor.
		/// </summary>
		public const int NULLORDER = unchecked((int)0xffffffff);

		/// <summary>
		/// CollationElementIterator constructor.  This takes the source string and
		/// the collation object.  The cursor will walk thru the source string based
		/// on the predefined collation rules.  If the source string is empty,
		/// NULLORDER will be returned on the calls to next(). </summary>
		/// <param name="sourceText"> the source string. </param>
		/// <param name="owner"> the collation object. </param>
		internal CollationElementIterator(String sourceText, RuleBasedCollator owner)
		{
			this.Owner = owner;
			Ordering = owner.Tables;
			if (sourceText.Length() != 0)
			{
				NormalizerBase.Mode mode = CollatorUtilities.toNormalizerMode(owner.Decomposition);
				Text_Renamed = new NormalizerBase(sourceText, mode);
			}
		}

		/// <summary>
		/// CollationElementIterator constructor.  This takes the source string and
		/// the collation object.  The cursor will walk thru the source string based
		/// on the predefined collation rules.  If the source string is empty,
		/// NULLORDER will be returned on the calls to next(). </summary>
		/// <param name="sourceText"> the source string. </param>
		/// <param name="owner"> the collation object. </param>
		internal CollationElementIterator(CharacterIterator sourceText, RuleBasedCollator owner)
		{
			this.Owner = owner;
			Ordering = owner.Tables;
			NormalizerBase.Mode mode = CollatorUtilities.toNormalizerMode(owner.Decomposition);
			Text_Renamed = new NormalizerBase(sourceText, mode);
		}

		/// <summary>
		/// Resets the cursor to the beginning of the string.  The next call
		/// to next() will return the first collation element in the string.
		/// </summary>
		public void Reset()
		{
			if (Text_Renamed != null)
			{
				Text_Renamed.reset();
				NormalizerBase.Mode mode = CollatorUtilities.toNormalizerMode(Owner.Decomposition);
				Text_Renamed.Mode = mode;
			}
			Buffer = null;
			ExpIndex = 0;
			SwapOrder = 0;
		}

		/// <summary>
		/// Get the next collation element in the string.  <para>This iterator iterates
		/// over a sequence of collation elements that were built from the string.
		/// Because there isn't necessarily a one-to-one mapping from characters to
		/// collation elements, this doesn't mean the same thing as "return the
		/// collation element [or ordering priority] of the next character in the
		/// string".</p>
		/// </para>
		/// <para>This function returns the collation element that the iterator is currently
		/// pointing to and then updates the internal pointer to point to the next element.
		/// previous() updates the pointer first and then returns the element.  This
		/// means that when you change direction while iterating (i.e., call next() and
		/// then call previous(), or call previous() and then call next()), you'll get
		/// back the same element twice.</para>
		/// </summary>
		/// <returns> the next collation element </returns>
		public int Next()
		{
			if (Text_Renamed == null)
			{
				return NULLORDER;
			}
			NormalizerBase.Mode textMode = Text_Renamed.Mode;
			// convert the owner's mode to something the Normalizer understands
			NormalizerBase.Mode ownerMode = CollatorUtilities.toNormalizerMode(Owner.Decomposition);
			if (textMode != ownerMode)
			{
				Text_Renamed.Mode = ownerMode;
			}

			// if buffer contains any decomposed char values
			// return their strength orders before continuing in
			// the Normalizer's CharacterIterator.
			if (Buffer != null)
			{
				if (ExpIndex < Buffer.Length)
				{
					return StrengthOrder(Buffer[ExpIndex++]);
				}
				else
				{
					Buffer = null;
					ExpIndex = 0;
				}
			}
			else if (SwapOrder != 0)
			{
				if (Character.IsSupplementaryCodePoint(SwapOrder))
				{
					char[] chars = Character.ToChars(SwapOrder);
					SwapOrder = chars[1];
					return chars[0] << 16;
				}
				int order = SwapOrder << 16;
				SwapOrder = 0;
				return order;
			}
			int ch = Text_Renamed.next();

			// are we at the end of Normalizer's text?
			if (ch == NormalizerBase.DONE)
			{
				return NULLORDER;
			}

			int value = Ordering.GetUnicodeOrder(ch);
			if (value == RuleBasedCollator.UNMAPPED)
			{
				SwapOrder = ch;
				return UNMAPPEDCHARVALUE;
			}
			else if (value >= RuleBasedCollator.CONTRACTCHARINDEX)
			{
				value = NextContractChar(ch);
			}
			if (value >= RuleBasedCollator.EXPANDCHARINDEX)
			{
				Buffer = Ordering.GetExpandValueList(value);
				ExpIndex = 0;
				value = Buffer[ExpIndex++];
			}

			if (Ordering.SEAsianSwapping)
			{
				int consonant;
				if (IsThaiPreVowel(ch))
				{
					consonant = Text_Renamed.next();
					if (IsThaiBaseConsonant(consonant))
					{
						Buffer = MakeReorderedBuffer(consonant, value, Buffer, true);
						value = Buffer[0];
						ExpIndex = 1;
					}
					else if (consonant != NormalizerBase.DONE)
					{
						Text_Renamed.previous();
					}
				}
				if (IsLaoPreVowel(ch))
				{
					consonant = Text_Renamed.next();
					if (IsLaoBaseConsonant(consonant))
					{
						Buffer = MakeReorderedBuffer(consonant, value, Buffer, true);
						value = Buffer[0];
						ExpIndex = 1;
					}
					else if (consonant != NormalizerBase.DONE)
					{
						Text_Renamed.previous();
					}
				}
			}

			return StrengthOrder(value);
		}

		/// <summary>
		/// Get the previous collation element in the string.  <para>This iterator iterates
		/// over a sequence of collation elements that were built from the string.
		/// Because there isn't necessarily a one-to-one mapping from characters to
		/// collation elements, this doesn't mean the same thing as "return the
		/// collation element [or ordering priority] of the previous character in the
		/// string".</p>
		/// </para>
		/// <para>This function updates the iterator's internal pointer to point to the
		/// collation element preceding the one it's currently pointing to and then
		/// returns that element, while next() returns the current element and then
		/// updates the pointer.  This means that when you change direction while
		/// iterating (i.e., call next() and then call previous(), or call previous()
		/// and then call next()), you'll get back the same element twice.</para>
		/// </summary>
		/// <returns> the previous collation element
		/// @since 1.2 </returns>
		public int Previous()
		{
			if (Text_Renamed == null)
			{
				return NULLORDER;
			}
			NormalizerBase.Mode textMode = Text_Renamed.Mode;
			// convert the owner's mode to something the Normalizer understands
			NormalizerBase.Mode ownerMode = CollatorUtilities.toNormalizerMode(Owner.Decomposition);
			if (textMode != ownerMode)
			{
				Text_Renamed.Mode = ownerMode;
			}
			if (Buffer != null)
			{
				if (ExpIndex > 0)
				{
					return StrengthOrder(Buffer[--ExpIndex]);
				}
				else
				{
					Buffer = null;
					ExpIndex = 0;
				}
			}
			else if (SwapOrder != 0)
			{
				if (Character.IsSupplementaryCodePoint(SwapOrder))
				{
					char[] chars = Character.ToChars(SwapOrder);
					SwapOrder = chars[1];
					return chars[0] << 16;
				}
				int order = SwapOrder << 16;
				SwapOrder = 0;
				return order;
			}
			int ch = Text_Renamed.previous();
			if (ch == NormalizerBase.DONE)
			{
				return NULLORDER;
			}

			int value = Ordering.GetUnicodeOrder(ch);

			if (value == RuleBasedCollator.UNMAPPED)
			{
				SwapOrder = UNMAPPEDCHARVALUE;
				return ch;
			}
			else if (value >= RuleBasedCollator.CONTRACTCHARINDEX)
			{
				value = PrevContractChar(ch);
			}
			if (value >= RuleBasedCollator.EXPANDCHARINDEX)
			{
				Buffer = Ordering.GetExpandValueList(value);
				ExpIndex = Buffer.Length;
				value = Buffer[--ExpIndex];
			}

			if (Ordering.SEAsianSwapping)
			{
				int vowel;
				if (IsThaiBaseConsonant(ch))
				{
					vowel = Text_Renamed.previous();
					if (IsThaiPreVowel(vowel))
					{
						Buffer = MakeReorderedBuffer(vowel, value, Buffer, false);
						ExpIndex = Buffer.Length - 1;
						value = Buffer[ExpIndex];
					}
					else
					{
						Text_Renamed.next();
					}
				}
				if (IsLaoBaseConsonant(ch))
				{
					vowel = Text_Renamed.previous();
					if (IsLaoPreVowel(vowel))
					{
						Buffer = MakeReorderedBuffer(vowel, value, Buffer, false);
						ExpIndex = Buffer.Length - 1;
						value = Buffer[ExpIndex];
					}
					else
					{
						Text_Renamed.next();
					}
				}
			}

			return StrengthOrder(value);
		}

		/// <summary>
		/// Return the primary component of a collation element. </summary>
		/// <param name="order"> the collation element </param>
		/// <returns> the element's primary component </returns>
		public static int PrimaryOrder(int order)
		{
			order &= RBCollationTables.PRIMARYORDERMASK;
			return ((int)((uint)order >> RBCollationTables.PRIMARYORDERSHIFT));
		}
		/// <summary>
		/// Return the secondary component of a collation element. </summary>
		/// <param name="order"> the collation element </param>
		/// <returns> the element's secondary component </returns>
		public static short SecondaryOrder(int order)
		{
			order = order & RBCollationTables.SECONDARYORDERMASK;
			return ((short)(order >> RBCollationTables.SECONDARYORDERSHIFT));
		}
		/// <summary>
		/// Return the tertiary component of a collation element. </summary>
		/// <param name="order"> the collation element </param>
		/// <returns> the element's tertiary component </returns>
		public static short TertiaryOrder(int order)
		{
			return ((short)(order &= RBCollationTables.TERTIARYORDERMASK));
		}

		/// <summary>
		///  Get the comparison order in the desired strength.  Ignore the other
		///  differences. </summary>
		///  <param name="order"> The order value </param>
		internal int StrengthOrder(int order)
		{
			int s = Owner.Strength;
			if (s == Collator.PRIMARY)
			{
				order &= RBCollationTables.PRIMARYDIFFERENCEONLY;
			}
			else if (s == Collator.SECONDARY)
			{
				order &= RBCollationTables.SECONDARYDIFFERENCEONLY;
			}
			return order;
		}

		/// <summary>
		/// Sets the iterator to point to the collation element corresponding to
		/// the specified character (the parameter is a CHARACTER offset in the
		/// original string, not an offset into its corresponding sequence of
		/// collation elements).  The value returned by the next call to next()
		/// will be the collation element corresponding to the specified position
		/// in the text.  If that position is in the middle of a contracting
		/// character sequence, the result of the next call to next() is the
		/// collation element for that sequence.  This means that getOffset()
		/// is not guaranteed to return the same value as was passed to a preceding
		/// call to setOffset().
		/// </summary>
		/// <param name="newOffset"> The new character offset into the original text.
		/// @since 1.2 </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void setOffset(int newOffset)
		public int Offset
		{
			set
			{
				if (Text_Renamed != null)
				{
					if (value < Text_Renamed.BeginIndex || value >= Text_Renamed.EndIndex)
					{
							Text_Renamed.IndexOnly = value;
					}
					else
					{
						int c = Text_Renamed.setIndex(value);
    
						// if the desired character isn't used in a contracting character
						// sequence, bypass all the backing-up logic-- we're sitting on
						// the right character already
						if (Ordering.UsedInContractSeq(c))
						{
							// walk backwards through the string until we see a character
							// that DOESN'T participate in a contracting character sequence
							while (Ordering.UsedInContractSeq(c))
							{
								c = Text_Renamed.previous();
							}
							// now walk forward using this object's next() method until
							// we pass the starting point and set our current position
							// to the beginning of the last "character" before or at
							// our starting position
							int last = Text_Renamed.Index;
							while (Text_Renamed.Index <= value)
							{
								last = Text_Renamed.Index;
								Next();
							}
							Text_Renamed.IndexOnly = last;
							// we don't need this, since last is the last index
							// that is the starting of the contraction which encompass
							// value
							// text.previous();
						}
					}
				}
				Buffer = null;
				ExpIndex = 0;
				SwapOrder = 0;
			}
			get
			{
				return (Text_Renamed != null) ? Text_Renamed.Index : 0;
			}
		}



		/// <summary>
		/// Return the maximum length of any expansion sequences that end
		/// with the specified comparison order. </summary>
		/// <param name="order"> a collation order returned by previous or next. </param>
		/// <returns> the maximum length of any expansion sequences ending
		///         with the specified order.
		/// @since 1.2 </returns>
		public int GetMaxExpansion(int order)
		{
			return Ordering.GetMaxExpansion(order);
		}

		/// <summary>
		/// Set a new string over which to iterate.
		/// </summary>
		/// <param name="source">  the new source text
		/// @since 1.2 </param>
		public String Text
		{
			set
			{
				Buffer = null;
				SwapOrder = 0;
				ExpIndex = 0;
				NormalizerBase.Mode mode = CollatorUtilities.toNormalizerMode(Owner.Decomposition);
				if (Text_Renamed == null)
				{
					Text_Renamed = new NormalizerBase(value, mode);
				}
				else
				{
					Text_Renamed.Mode = mode;
					Text_Renamed.Text = value;
				}
			}
		}

		/// <summary>
		/// Set a new string over which to iterate.
		/// </summary>
		/// <param name="source">  the new source text.
		/// @since 1.2 </param>
		public CharacterIterator Text
		{
			set
			{
				Buffer = null;
				SwapOrder = 0;
				ExpIndex = 0;
				NormalizerBase.Mode mode = CollatorUtilities.toNormalizerMode(Owner.Decomposition);
				if (Text_Renamed == null)
				{
					Text_Renamed = new NormalizerBase(value, mode);
				}
				else
				{
					Text_Renamed.Mode = mode;
					Text_Renamed.Text = value;
				}
			}
		}

		//============================================================
		// privates
		//============================================================

		/// <summary>
		/// Determine if a character is a Thai vowel (which sorts after
		/// its base consonant).
		/// </summary>
		private static bool IsThaiPreVowel(int ch)
		{
			return (ch >= 0x0e40) && (ch <= 0x0e44);
		}

		/// <summary>
		/// Determine if a character is a Thai base consonant
		/// </summary>
		private static bool IsThaiBaseConsonant(int ch)
		{
			return (ch >= 0x0e01) && (ch <= 0x0e2e);
		}

		/// <summary>
		/// Determine if a character is a Lao vowel (which sorts after
		/// its base consonant).
		/// </summary>
		private static bool IsLaoPreVowel(int ch)
		{
			return (ch >= 0x0ec0) && (ch <= 0x0ec4);
		}

		/// <summary>
		/// Determine if a character is a Lao base consonant
		/// </summary>
		private static bool IsLaoBaseConsonant(int ch)
		{
			return (ch >= 0x0e81) && (ch <= 0x0eae);
		}

		/// <summary>
		/// This method produces a buffer which contains the collation
		/// elements for the two characters, with colFirst's values preceding
		/// another character's.  Presumably, the other character precedes colFirst
		/// in logical order (otherwise you wouldn't need this method would you?).
		/// The assumption is that the other char's value(s) have already been
		/// computed.  If this char has a single element it is passed to this
		/// method as lastValue, and lastExpansion is null.  If it has an
		/// expansion it is passed in lastExpansion, and colLastValue is ignored.
		/// </summary>
		private int[] MakeReorderedBuffer(int colFirst, int lastValue, int[] lastExpansion, bool forward)
		{

			int[] result;

			int firstValue = Ordering.GetUnicodeOrder(colFirst);
			if (firstValue >= RuleBasedCollator.CONTRACTCHARINDEX)
			{
				firstValue = forward? NextContractChar(colFirst) : PrevContractChar(colFirst);
			}

			int[] firstExpansion = null;
			if (firstValue >= RuleBasedCollator.EXPANDCHARINDEX)
			{
				firstExpansion = Ordering.GetExpandValueList(firstValue);
			}

			if (!forward)
			{
				int temp1 = firstValue;
				firstValue = lastValue;
				lastValue = temp1;
				int[] temp2 = firstExpansion;
				firstExpansion = lastExpansion;
				lastExpansion = temp2;
			}

			if (firstExpansion == null && lastExpansion == null)
			{
				result = new int [2];
				result[0] = firstValue;
				result[1] = lastValue;
			}
			else
			{
				int firstLength = firstExpansion == null? 1 : firstExpansion.Length;
				int lastLength = lastExpansion == null? 1 : lastExpansion.Length;
				result = new int[firstLength + lastLength];

				if (firstExpansion == null)
				{
					result[0] = firstValue;
				}
				else
				{
					System.Array.Copy(firstExpansion, 0, result, 0, firstLength);
				}

				if (lastExpansion == null)
				{
					result[firstLength] = lastValue;
				}
				else
				{
					System.Array.Copy(lastExpansion, 0, result, firstLength, lastLength);
				}
			}

			return result;
		}

		/// <summary>
		///  Check if a comparison order is ignorable. </summary>
		///  <returns> true if a character is ignorable, false otherwise. </returns>
		internal static bool IsIgnorable(int order)
		{
			return ((PrimaryOrder(order) == 0) ? true : false);
		}

		/// <summary>
		/// Get the ordering priority of the next contracting character in the
		/// string. </summary>
		/// <param name="ch"> the starting character of a contracting character token </param>
		/// <returns> the next contracting character's ordering.  Returns NULLORDER
		/// if the end of string is reached. </returns>
		private int NextContractChar(int ch)
		{
			// First get the ordering of this single character,
			// which is always the first element in the list
			List<EntryPair> list = Ordering.GetContractValues(ch);
			EntryPair pair = list[0];
			int order = pair.Value;

			// find out the length of the longest contracting character sequence in the list.
			// There's logic in the builder code to make sure the longest sequence is always
			// the last.
			pair = list[list.Count - 1];
			int maxLength = pair.EntryName.Length();

			// (the Normalizer is cloned here so that the seeking we do in the next loop
			// won't affect our real position in the text)
			NormalizerBase tempText = (NormalizerBase)Text_Renamed.clone();

			// extract the next maxLength characters in the string (we have to do this using the
			// Normalizer to ensure that our offsets correspond to those the rest of the
			// iterator is using) and store it in "fragment".
			tempText.previous();
			Key.Length = 0;
			int c = tempText.next();
			while (maxLength > 0 && c != NormalizerBase.DONE)
			{
				if (Character.IsSupplementaryCodePoint(c))
				{
					Key.Append(Character.ToChars(c));
					maxLength -= 2;
				}
				else
				{
					Key.Append((char)c);
					--maxLength;
				}
				c = tempText.next();
			}
			String fragment = Key.ToString();
			// now that we have that fragment, iterate through this list looking for the
			// longest sequence that matches the characters in the actual text.  (maxLength
			// is used here to keep track of the length of the longest sequence)
			// Upon exit from this loop, maxLength will contain the length of the matching
			// sequence and order will contain the collation-element value corresponding
			// to this sequence
			maxLength = 1;
			for (int i = list.Count - 1; i > 0; i--)
			{
				pair = list[i];
				if (!pair.Fwd)
				{
					continue;
				}

				if (fragment.StartsWith(pair.EntryName) && pair.EntryName.Length() > maxLength)
				{
					maxLength = pair.EntryName.Length();
					order = pair.Value;
				}
			}

			// seek our current iteration position to the end of the matching sequence
			// and return the appropriate collation-element value (if there was no matching
			// sequence, we're already seeked to the right position and order already contains
			// the correct collation-element value for the single character)
			while (maxLength > 1)
			{
				c = Text_Renamed.next();
				maxLength -= Character.CharCount(c);
			}
			return order;
		}

		/// <summary>
		/// Get the ordering priority of the previous contracting character in the
		/// string. </summary>
		/// <param name="ch"> the starting character of a contracting character token </param>
		/// <returns> the next contracting character's ordering.  Returns NULLORDER
		/// if the end of string is reached. </returns>
		private int PrevContractChar(int ch)
		{
			// This function is identical to nextContractChar(), except that we've
			// switched things so that the next() and previous() calls on the Normalizer
			// are switched and so that we skip entry pairs with the fwd flag turned on
			// rather than off.  Notice that we still use append() and startsWith() when
			// working on the fragment.  This is because the entry pairs that are used
			// in reverse iteration have their names reversed already.
			List<EntryPair> list = Ordering.GetContractValues(ch);
			EntryPair pair = list[0];
			int order = pair.Value;

			pair = list[list.Count - 1];
			int maxLength = pair.EntryName.Length();

			NormalizerBase tempText = (NormalizerBase)Text_Renamed.clone();

			tempText.next();
			Key.Length = 0;
			int c = tempText.previous();
			while (maxLength > 0 && c != NormalizerBase.DONE)
			{
				if (Character.IsSupplementaryCodePoint(c))
				{
					Key.Append(Character.ToChars(c));
					maxLength -= 2;
				}
				else
				{
					Key.Append((char)c);
					--maxLength;
				}
				c = tempText.previous();
			}
			String fragment = Key.ToString();

			maxLength = 1;
			for (int i = list.Count - 1; i > 0; i--)
			{
				pair = list[i];
				if (pair.Fwd)
				{
					continue;
				}

				if (fragment.StartsWith(pair.EntryName) && pair.EntryName.Length() > maxLength)
				{
					maxLength = pair.EntryName.Length();
					order = pair.Value;
				}
			}

			while (maxLength > 1)
			{
				c = Text_Renamed.previous();
				maxLength -= Character.CharCount(c);
			}
			return order;
		}

		internal const int UNMAPPEDCHARVALUE = 0x7FFF0000;

		private NormalizerBase Text_Renamed = null;
		private int[] Buffer = null;
		private int ExpIndex = 0;
		private StringBuffer Key = new StringBuffer(5);
		private int SwapOrder = 0;
		private RBCollationTables Ordering;
		private RuleBasedCollator Owner;
	}

}