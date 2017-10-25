/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright IBM Corp. 1999-2003 - All Rights Reserved
 *
 * The original version of this source code and documentation is
 * copyrighted and owned by IBM. These materials are provided
 * under terms of a License Agreement between IBM and Sun.
 * This technology is protected by multiple US and International
 * patents. This notice and attribution to IBM may not be removed.
 */

namespace java.text
{

	using BidiBase = sun.text.bidi.BidiBase;

	/// <summary>
	/// This class implements the Unicode Bidirectional Algorithm.
	/// <para>
	/// A Bidi object provides information on the bidirectional reordering of the text
	/// used to create it.  This is required, for example, to properly display Arabic
	/// or Hebrew text.  These languages are inherently mixed directional, as they order
	/// numbers from left-to-right while ordering most other text from right-to-left.
	/// </para>
	/// <para>
	/// Once created, a Bidi object can be queried to see if the text it represents is
	/// all left-to-right or all right-to-left.  Such objects are very lightweight and
	/// this text is relatively easy to process.
	/// </para>
	/// <para>
	/// If there are multiple runs of text, information about the runs can be accessed
	/// by indexing to get the start, limit, and level of a run.  The level represents
	/// both the direction and the 'nesting level' of a directional run.  Odd levels
	/// are right-to-left, while even levels are left-to-right.  So for example level
	/// 0 represents left-to-right text, while level 1 represents right-to-left text, and
	/// level 2 represents left-to-right text embedded in a right-to-left run.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>
	public sealed class Bidi
	{

		/// <summary>
		/// Constant indicating base direction is left-to-right. </summary>
		public const int DIRECTION_LEFT_TO_RIGHT = 0;

		/// <summary>
		/// Constant indicating base direction is right-to-left. </summary>
		public const int DIRECTION_RIGHT_TO_LEFT = 1;

		/// <summary>
		/// Constant indicating that the base direction depends on the first strong
		/// directional character in the text according to the Unicode
		/// Bidirectional Algorithm.  If no strong directional character is present,
		/// the base direction is left-to-right.
		/// </summary>
		public const int DIRECTION_DEFAULT_LEFT_TO_RIGHT = -2;

		/// <summary>
		/// Constant indicating that the base direction depends on the first strong
		/// directional character in the text according to the Unicode
		/// Bidirectional Algorithm.  If no strong directional character is present,
		/// the base direction is right-to-left.
		/// </summary>
		public const int DIRECTION_DEFAULT_RIGHT_TO_LEFT = -1;

		private BidiBase BidiBase;

		/// <summary>
		/// Create Bidi from the given paragraph of text and base direction. </summary>
		/// <param name="paragraph"> a paragraph of text </param>
		/// <param name="flags"> a collection of flags that control the algorithm.  The
		/// algorithm understands the flags DIRECTION_LEFT_TO_RIGHT, DIRECTION_RIGHT_TO_LEFT,
		/// DIRECTION_DEFAULT_LEFT_TO_RIGHT, and DIRECTION_DEFAULT_RIGHT_TO_LEFT.
		/// Other values are reserved. </param>
		public Bidi(String paragraph, int flags)
		{
			if (paragraph == null)
			{
				throw new IllegalArgumentException("paragraph is null");
			}

			BidiBase = new BidiBase(paragraph.ToCharArray(), 0, null, 0, paragraph.Length(), flags);
		}

		/// <summary>
		/// Create Bidi from the given paragraph of text.
		/// <para>
		/// The RUN_DIRECTION attribute in the text, if present, determines the base
		/// direction (left-to-right or right-to-left).  If not present, the base
		/// direction is computes using the Unicode Bidirectional Algorithm, defaulting to left-to-right
		/// if there are no strong directional characters in the text.  This attribute, if
		/// present, must be applied to all the text in the paragraph.
		/// </para>
		/// <para>
		/// The BIDI_EMBEDDING attribute in the text, if present, represents embedding level
		/// information.  Negative values from -1 to -62 indicate overrides at the absolute value
		/// of the level.  Positive values from 1 to 62 indicate embeddings.  Where values are
		/// zero or not defined, the base embedding level as determined by the base direction
		/// is assumed.
		/// </para>
		/// <para>
		/// The NUMERIC_SHAPING attribute in the text, if present, converts European digits to
		/// other decimal digits before running the bidi algorithm.  This attribute, if present,
		/// must be applied to all the text in the paragraph.
		/// 
		/// </para>
		/// </summary>
		/// <param name="paragraph"> a paragraph of text with optional character and paragraph attribute information
		/// </param>
		/// <seealso cref= java.awt.font.TextAttribute#BIDI_EMBEDDING </seealso>
		/// <seealso cref= java.awt.font.TextAttribute#NUMERIC_SHAPING </seealso>
		/// <seealso cref= java.awt.font.TextAttribute#RUN_DIRECTION </seealso>
		public Bidi(AttributedCharacterIterator paragraph)
		{
			if (paragraph == null)
			{
				throw new IllegalArgumentException("paragraph is null");
			}

			BidiBase = new BidiBase(0, 0);
			BidiBase.Para = paragraph;
		}

		/// <summary>
		/// Create Bidi from the given text, embedding, and direction information.
		/// The embeddings array may be null.  If present, the values represent embedding level
		/// information.  Negative values from -1 to -61 indicate overrides at the absolute value
		/// of the level.  Positive values from 1 to 61 indicate embeddings.  Where values are
		/// zero, the base embedding level as determined by the base direction is assumed. </summary>
		/// <param name="text"> an array containing the paragraph of text to process. </param>
		/// <param name="textStart"> the index into the text array of the start of the paragraph. </param>
		/// <param name="embeddings"> an array containing embedding values for each character in the paragraph.
		/// This can be null, in which case it is assumed that there is no external embedding information. </param>
		/// <param name="embStart"> the index into the embedding array of the start of the paragraph. </param>
		/// <param name="paragraphLength"> the length of the paragraph in the text and embeddings arrays. </param>
		/// <param name="flags"> a collection of flags that control the algorithm.  The
		/// algorithm understands the flags DIRECTION_LEFT_TO_RIGHT, DIRECTION_RIGHT_TO_LEFT,
		/// DIRECTION_DEFAULT_LEFT_TO_RIGHT, and DIRECTION_DEFAULT_RIGHT_TO_LEFT.
		/// Other values are reserved. </param>
		public Bidi(char[] text, int textStart, sbyte[] embeddings, int embStart, int paragraphLength, int flags)
		{
			if (text == null)
			{
				throw new IllegalArgumentException("text is null");
			}
			if (paragraphLength < 0)
			{
				throw new IllegalArgumentException("bad length: " + paragraphLength);
			}
			if (textStart < 0 || paragraphLength > text.Length - textStart)
			{
				throw new IllegalArgumentException("bad range: " + textStart + " length: " + paragraphLength + " for text of length: " + text.Length);
			}
			if (embeddings != null && (embStart < 0 || paragraphLength > embeddings.Length - embStart))
			{
				throw new IllegalArgumentException("bad range: " + embStart + " length: " + paragraphLength + " for embeddings of length: " + text.Length);
			}

			BidiBase = new BidiBase(text, textStart, embeddings, embStart, paragraphLength, flags);
		}

		/// <summary>
		/// Create a Bidi object representing the bidi information on a line of text within
		/// the paragraph represented by the current Bidi.  This call is not required if the
		/// entire paragraph fits on one line.
		/// </summary>
		/// <param name="lineStart"> the offset from the start of the paragraph to the start of the line. </param>
		/// <param name="lineLimit"> the offset from the start of the paragraph to the limit of the line. </param>
		/// <returns> a {@code Bidi} object </returns>
		public Bidi CreateLineBidi(int lineStart, int lineLimit)
		{
			AttributedString astr = new AttributedString("");
			Bidi newBidi = new Bidi(astr.Iterator);

			return BidiBase.setLine(this, BidiBase, newBidi, newBidi.BidiBase,lineStart, lineLimit);
		}

		/// <summary>
		/// Return true if the line is not left-to-right or right-to-left.  This means it either has mixed runs of left-to-right
		/// and right-to-left text, or the base direction differs from the direction of the only run of text.
		/// </summary>
		/// <returns> true if the line is not left-to-right or right-to-left. </returns>
		public bool Mixed
		{
			get
			{
				return BidiBase.Mixed;
			}
		}

		/// <summary>
		/// Return true if the line is all left-to-right text and the base direction is left-to-right.
		/// </summary>
		/// <returns> true if the line is all left-to-right text and the base direction is left-to-right </returns>
		public bool LeftToRight
		{
			get
			{
				return BidiBase.LeftToRight;
			}
		}

		/// <summary>
		/// Return true if the line is all right-to-left text, and the base direction is right-to-left. </summary>
		/// <returns> true if the line is all right-to-left text, and the base direction is right-to-left </returns>
		public bool RightToLeft
		{
			get
			{
				return BidiBase.RightToLeft;
			}
		}

		/// <summary>
		/// Return the length of text in the line. </summary>
		/// <returns> the length of text in the line </returns>
		public int Length
		{
			get
			{
				return BidiBase.Length;
			}
		}

		/// <summary>
		/// Return true if the base direction is left-to-right. </summary>
		/// <returns> true if the base direction is left-to-right </returns>
		public bool BaseIsLeftToRight()
		{
			return BidiBase.baseIsLeftToRight();
		}

		/// <summary>
		/// Return the base level (0 if left-to-right, 1 if right-to-left). </summary>
		/// <returns> the base level </returns>
		public int BaseLevel
		{
			get
			{
				return BidiBase.ParaLevel;
			}
		}

		/// <summary>
		/// Return the resolved level of the character at offset.  If offset is
		/// {@literal <} 0 or &ge; the length of the line, return the base direction
		/// level.
		/// </summary>
		/// <param name="offset"> the index of the character for which to return the level </param>
		/// <returns> the resolved level of the character at offset </returns>
		public int GetLevelAt(int offset)
		{
			return BidiBase.getLevelAt(offset);
		}

		/// <summary>
		/// Return the number of level runs. </summary>
		/// <returns> the number of level runs </returns>
		public int RunCount
		{
			get
			{
				return BidiBase.countRuns();
			}
		}

		/// <summary>
		/// Return the level of the nth logical run in this line. </summary>
		/// <param name="run"> the index of the run, between 0 and <code>getRunCount()</code> </param>
		/// <returns> the level of the run </returns>
		public int GetRunLevel(int run)
		{
			return BidiBase.getRunLevel(run);
		}

		/// <summary>
		/// Return the index of the character at the start of the nth logical run in this line, as
		/// an offset from the start of the line. </summary>
		/// <param name="run"> the index of the run, between 0 and <code>getRunCount()</code> </param>
		/// <returns> the start of the run </returns>
		public int GetRunStart(int run)
		{
			return BidiBase.getRunStart(run);
		}

		/// <summary>
		/// Return the index of the character past the end of the nth logical run in this line, as
		/// an offset from the start of the line.  For example, this will return the length
		/// of the line for the last run on the line. </summary>
		/// <param name="run"> the index of the run, between 0 and <code>getRunCount()</code> </param>
		/// <returns> limit the limit of the run </returns>
		public int GetRunLimit(int run)
		{
			return BidiBase.getRunLimit(run);
		}

		/// <summary>
		/// Return true if the specified text requires bidi analysis.  If this returns false,
		/// the text will display left-to-right.  Clients can then avoid constructing a Bidi object.
		/// Text in the Arabic Presentation Forms area of Unicode is presumed to already be shaped
		/// and ordered for display, and so will not cause this function to return true.
		/// </summary>
		/// <param name="text"> the text containing the characters to test </param>
		/// <param name="start"> the start of the range of characters to test </param>
		/// <param name="limit"> the limit of the range of characters to test </param>
		/// <returns> true if the range of characters requires bidi analysis </returns>
		public static bool RequiresBidi(char[] text, int start, int limit)
		{
			return BidiBase.requiresBidi(text, start, limit);
		}

		/// <summary>
		/// Reorder the objects in the array into visual order based on their levels.
		/// This is a utility function to use when you have a collection of objects
		/// representing runs of text in logical order, each run containing text
		/// at a single level.  The elements at <code>index</code> from
		/// <code>objectStart</code> up to <code>objectStart + count</code>
		/// in the objects array will be reordered into visual order assuming
		/// each run of text has the level indicated by the corresponding element
		/// in the levels array (at <code>index - objectStart + levelStart</code>).
		/// </summary>
		/// <param name="levels"> an array representing the bidi level of each object </param>
		/// <param name="levelStart"> the start position in the levels array </param>
		/// <param name="objects"> the array of objects to be reordered into visual order </param>
		/// <param name="objectStart"> the start position in the objects array </param>
		/// <param name="count"> the number of objects to reorder </param>
		public static void ReorderVisually(sbyte[] levels, int levelStart, Object[] objects, int objectStart, int count)
		{
			BidiBase.reorderVisually(levels, levelStart, objects, objectStart, count);
		}

		/// <summary>
		/// Display the bidi internal state, used in debugging.
		/// </summary>
		public override String ToString()
		{
			return BidiBase.ToString();
		}

	}

}