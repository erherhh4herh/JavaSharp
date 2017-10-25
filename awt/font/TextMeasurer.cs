using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright Taligent, Inc. 1996 - 1997, All Rights Reserved
 * (C) Copyright IBM Corp. 1996 - 1998, All Rights Reserved
 *
 * The original version of this source code and documentation is
 * copyrighted and owned by Taligent, Inc., a wholly-owned subsidiary
 * of IBM. These materials are provided under terms of a License
 * Agreement between Taligent and Sun. This technology is protected
 * by multiple US and International patents.
 *
 * This notice and attribution to Taligent may not be removed.
 * Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.awt.font
{



	using AttributeValues = sun.font.AttributeValues;
	using BidiUtils = sun.font.BidiUtils;
	using TextLineComponent = sun.font.TextLineComponent;
	using TextLabelFactory = sun.font.TextLabelFactory;
	using FontResolver = sun.font.FontResolver;

	/// <summary>
	/// The <code>TextMeasurer</code> class provides the primitive operations
	/// needed for line break: measuring up to a given advance, determining the
	/// advance of a range of characters, and generating a
	/// <code>TextLayout</code> for a range of characters. It also provides
	/// methods for incremental editing of paragraphs.
	/// <para>
	/// A <code>TextMeasurer</code> object is constructed with an
	/// <seealso cref="java.text.AttributedCharacterIterator AttributedCharacterIterator"/>
	/// representing a single paragraph of text.  The value returned by the
	/// <seealso cref="AttributedCharacterIterator#getBeginIndex() getBeginIndex"/>
	/// method of <code>AttributedCharacterIterator</code>
	/// defines the absolute index of the first character.  The value
	/// returned by the
	/// <seealso cref="AttributedCharacterIterator#getEndIndex() getEndIndex"/>
	/// method of <code>AttributedCharacterIterator</code> defines the index
	/// past the last character.  These values define the range of indexes to
	/// use in calls to the <code>TextMeasurer</code>.  For example, calls to
	/// get the advance of a range of text or the line break of a range of text
	/// must use indexes between the beginning and end index values.  Calls to
	/// <seealso cref="#insertChar(java.text.AttributedCharacterIterator, int) insertChar"/>
	/// and
	/// <seealso cref="#deleteChar(java.text.AttributedCharacterIterator, int) deleteChar"/>
	/// reset the <code>TextMeasurer</code> to use the beginning index and end
	/// index of the <code>AttributedCharacterIterator</code> passed in those calls.
	/// </para>
	/// <para>
	/// Most clients will use the more convenient <code>LineBreakMeasurer</code>,
	/// which implements the standard line break policy (placing as many words
	/// as will fit on each line).
	/// 
	/// @author John Raley
	/// </para>
	/// </summary>
	/// <seealso cref= LineBreakMeasurer
	/// @since 1.3 </seealso>

	public sealed class TextMeasurer : Cloneable
	{

		// Number of lines to format to.
		private static float EST_LINES = (float) 2.1;

		/*
		static {
		    String s = System.getProperty("estLines");
		    if (s != null) {
		        try {
		            Float f = new Float(s);
		            EST_LINES = f.floatValue();
		        }
		        catch(NumberFormatException e) {
		        }
		    }
		    //System.out.println("EST_LINES="+EST_LINES);
		}
		*/

		private FontRenderContext FFrc;

		private int FStart;

		// characters in source text
		private char[] FChars;

		// Bidi for this paragraph
		private Bidi FBidi;

		// Levels array for chars in this paragraph - needed to reorder
		// trailing counterdirectional whitespace
		private sbyte[] FLevels;

		// line components in logical order
		private TextLineComponent[] FComponents;

		// index where components begin
		private int FComponentStart;

		// index where components end
		private int FComponentLimit;

		private bool HaveLayoutWindow;

		// used to find valid starting points for line components
		private BreakIterator FLineBreak = null;
		private CharArrayIterator CharIter = null;
		internal int LayoutCount = 0;
		internal int LayoutCharCount = 0;

		// paragraph, with resolved fonts and styles
		private StyledParagraph FParagraph;

		// paragraph data - same across all layouts
		private bool FIsDirectionLTR;
		private sbyte FBaseline;
		private float[] FBaselineOffsets;
		private float FJustifyRatio = 1;

		/// <summary>
		/// Constructs a <code>TextMeasurer</code> from the source text.
		/// The source text should be a single entire paragraph. </summary>
		/// <param name="text"> the source paragraph.  Cannot be null. </param>
		/// <param name="frc"> the information about a graphics device which is needed
		///       to measure the text correctly.  Cannot be null. </param>
		public TextMeasurer(AttributedCharacterIterator text, FontRenderContext frc)
		{

			FFrc = frc;
			InitAll(text);
		}

		protected internal Object Clone()
		{
			TextMeasurer other;
			try
			{
				other = (TextMeasurer) base.Clone();
			}
			catch (CloneNotSupportedException)
			{
				throw new Error();
			}
			if (FComponents != null)
			{
				other.FComponents = FComponents.clone();
			}
			return other;
		}

		private void InvalidateComponents()
		{
			FComponentStart = FComponentLimit = FChars.Length;
			FComponents = null;
			HaveLayoutWindow = false;
		}

		/// <summary>
		/// Initialize state, including fChars array, direction, and
		/// fBidi.
		/// </summary>
		private void InitAll(AttributedCharacterIterator text)
		{

			FStart = text.BeginIndex;

			// extract chars
			FChars = new char[text.EndIndex - FStart];

			int n = 0;
			for (char c = text.First(); c != java.text.CharacterIterator_Fields.DONE; c = text.Next())
			{
				FChars[n++] = c;
			}

			text.First();

			FBidi = new Bidi(text);
			if (FBidi.LeftToRight)
			{
				FBidi = null;
			}

			text.First();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Map<? extends java.text.AttributedCharacterIterator_Attribute, ?> paragraphAttrs = text.getAttributes();
			IDictionary<?, ?> paragraphAttrs = text.Attributes;
			NumericShaper shaper = AttributeValues.getNumericShaping(paragraphAttrs);
			if (shaper != null)
			{
				shaper.Shape(FChars, 0, FChars.Length);
			}

			FParagraph = new StyledParagraph(text, FChars);

			{
			// set paragraph attributes
				// If there's an embedded graphic at the start of the
				// paragraph, look for the first non-graphic character
				// and use it and its font to initialize the paragraph.
				// If not, use the first graphic to initialize.
				FJustifyRatio = AttributeValues.getJustification(paragraphAttrs);

				bool haveFont = TextLine.AdvanceToFirstFont(text);

				if (haveFont)
				{
					Font defaultFont = TextLine.GetFontAtCurrentPos(text);
					int charsStart = text.Index - text.BeginIndex;
					LineMetrics lm = defaultFont.GetLineMetrics(FChars, charsStart, charsStart + 1, FFrc);
					FBaseline = (sbyte) lm.BaselineIndex;
					FBaselineOffsets = lm.BaselineOffsets;
				}
				else
				{
					// hmmm what to do here?  Just try to supply reasonable
					// values I guess.

					GraphicAttribute graphic = (GraphicAttribute) paragraphAttrs[TextAttribute.CHAR_REPLACEMENT];
					FBaseline = TextLayout.GetBaselineFromGraphic(graphic);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Hashtable<java.text.AttributedCharacterIterator_Attribute, ?> fmap = new java.util.Hashtable<>(5, (float)0.9);
					Dictionary<AttributedCharacterIterator_Attribute, ?> fmap = new Dictionary<AttributedCharacterIterator_Attribute, ?>(5, (float)0.9);
					Font dummyFont = new Font(fmap);
					LineMetrics lm = dummyFont.GetLineMetrics(" ", 0, 1, FFrc);
					FBaselineOffsets = lm.BaselineOffsets;
				}
				FBaselineOffsets = TextLine.GetNormalizedOffsets(FBaselineOffsets, FBaseline);
			}

			InvalidateComponents();
		}

		/// <summary>
		/// Generate components for the paragraph.  fChars, fBidi should have been
		/// initialized already.
		/// </summary>
		private void GenerateComponents(int startingAt, int endingAt)
		{

			if (CollectStats)
			{
				FormattedChars += (endingAt - startingAt);
			}
			int layoutFlags = 0; // no extra info yet, bidi determines run and line direction
			TextLabelFactory factory = new TextLabelFactory(FFrc, FChars, FBidi, layoutFlags);

			int[] charsLtoV = null;

			if (FBidi != null)
			{
				FLevels = BidiUtils.getLevels(FBidi);
				int[] charsVtoL = BidiUtils.createVisualToLogicalMap(FLevels);
				charsLtoV = BidiUtils.createInverseMap(charsVtoL);
				FIsDirectionLTR = FBidi.BaseIsLeftToRight();
			}
			else
			{
				FLevels = null;
				FIsDirectionLTR = true;
			}

			try
			{
				FComponents = TextLine.GetComponents(FParagraph, FChars, startingAt, endingAt, charsLtoV, FLevels, factory);
			}
			catch (IllegalArgumentException e)
			{
				System.Console.WriteLine("startingAt=" + startingAt + "; endingAt=" + endingAt);
				System.Console.WriteLine("fComponentLimit=" + FComponentLimit);
				throw e;
			}

			FComponentStart = startingAt;
			FComponentLimit = endingAt;
			//debugFormatCount += (endingAt-startingAt);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int calcLineBreak(final int pos, final float maxAdvance)
		private int CalcLineBreak(int pos, float maxAdvance)
		{

			// either of these statements removes the bug:
			//generateComponents(0, fChars.length);
			//generateComponents(pos, fChars.length);

			int startPos = pos;
			float width = maxAdvance;

			int tlcIndex;
			int tlcStart = FComponentStart;

			for (tlcIndex = 0; tlcIndex < FComponents.Length; tlcIndex++)
			{
				int gaLimit = tlcStart + FComponents[tlcIndex].NumCharacters;
				if (gaLimit > startPos)
				{
					break;
				}
				else
				{
					tlcStart = gaLimit;
				}
			}

			// tlcStart is now the start of the tlc at tlcIndex

			for (; tlcIndex < FComponents.Length; tlcIndex++)
			{

				TextLineComponent tlc = FComponents[tlcIndex];
				int numCharsInGa = tlc.NumCharacters;

				int lineBreak = tlc.getLineBreakIndex(startPos - tlcStart, width);
				if (lineBreak == numCharsInGa && tlcIndex < FComponents.Length)
				{
					width -= tlc.getAdvanceBetween(startPos - tlcStart, lineBreak);
					tlcStart += numCharsInGa;
					startPos = tlcStart;
				}
				else
				{
					return tlcStart + lineBreak;
				}
			}

			if (FComponentLimit < FChars.Length)
			{
				// format more text and try again
				//if (haveLayoutWindow) {
				//    outOfWindow++;
				//}

				GenerateComponents(pos, FChars.Length);
				return CalcLineBreak(pos, maxAdvance);
			}

			return FChars.Length;
		}

		/// <summary>
		/// According to the Unicode Bidirectional Behavior specification
		/// (Unicode Standard 2.0, section 3.11), whitespace at the ends
		/// of lines which would naturally flow against the base direction
		/// must be made to flow with the line direction, and moved to the
		/// end of the line.  This method returns the start of the sequence
		/// of trailing whitespace characters to move to the end of a
		/// line taken from the given range.
		/// </summary>
		private int TrailingCdWhitespaceStart(int startPos, int limitPos)
		{

			if (FLevels != null)
			{
				// Back up over counterdirectional whitespace
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte baseLevel = (byte)(fIsDirectionLTR? 0 : 1);
				sbyte baseLevel = (sbyte)(FIsDirectionLTR? 0 : 1);
				for (int cdWsStart = limitPos; --cdWsStart >= startPos;)
				{
					if ((FLevels[cdWsStart] % 2) == baseLevel || Character.GetDirectionality(FChars[cdWsStart]) != Character.DIRECTIONALITY_WHITESPACE)
					{
						return ++cdWsStart;
					}
				}
			}

			return startPos;
		}

		private TextLineComponent[] MakeComponentsOnRange(int startPos, int limitPos)
		{

			// sigh I really hate to do this here since it's part of the
			// bidi algorithm.
			// cdWsStart is the start of the trailing counterdirectional
			// whitespace
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cdWsStart = trailingCdWhitespaceStart(startPos, limitPos);
			int cdWsStart = TrailingCdWhitespaceStart(startPos, limitPos);

			int tlcIndex;
			int tlcStart = FComponentStart;

			for (tlcIndex = 0; tlcIndex < FComponents.Length; tlcIndex++)
			{
				int gaLimit = tlcStart + FComponents[tlcIndex].NumCharacters;
				if (gaLimit > startPos)
				{
					break;
				}
				else
				{
					tlcStart = gaLimit;
				}
			}

			// tlcStart is now the start of the tlc at tlcIndex

			int componentCount;
			{
				bool split = false;
				int compStart = tlcStart;
				int lim = tlcIndex;
				for (bool cont = true; cont; lim++)
				{
					int gaLimit = compStart + FComponents[lim].NumCharacters;
					if (cdWsStart > System.Math.Max(compStart, startPos) && cdWsStart < System.Math.Min(gaLimit, limitPos))
					{
						split = true;
					}
					if (gaLimit >= limitPos)
					{
						cont = false;
					}
					else
					{
						compStart = gaLimit;
					}
				}
				componentCount = lim - tlcIndex;
				if (split)
				{
					componentCount++;
				}
			}

			TextLineComponent[] components = new TextLineComponent[componentCount];
			int newCompIndex = 0;
			int linePos = startPos;

			int breakPt = cdWsStart;

			int subsetFlag;
			if (breakPt == startPos)
			{
				subsetFlag = FIsDirectionLTR? TextLineComponent.LEFT_TO_RIGHT : TextLineComponent.RIGHT_TO_LEFT;
				breakPt = limitPos;
			}
			else
			{
				subsetFlag = TextLineComponent.UNCHANGED;
			}

			while (linePos < limitPos)
			{

				int compLength = FComponents[tlcIndex].NumCharacters;
				int tlcLimit = tlcStart + compLength;

				int start = System.Math.Max(linePos, tlcStart);
				int limit = System.Math.Min(breakPt, tlcLimit);

				components[newCompIndex++] = FComponents[tlcIndex].getSubset(start - tlcStart, limit - tlcStart, subsetFlag);
				linePos += (limit - start);
				if (linePos == breakPt)
				{
					breakPt = limitPos;
					subsetFlag = FIsDirectionLTR? TextLineComponent.LEFT_TO_RIGHT : TextLineComponent.RIGHT_TO_LEFT;
				}
				if (linePos == tlcLimit)
				{
					tlcIndex++;
					tlcStart = tlcLimit;
				}
			}

			return components;
		}

		private TextLine MakeTextLineOnRange(int startPos, int limitPos)
		{

			int[] charsLtoV = null;
			sbyte[] charLevels = null;

			if (FBidi != null)
			{
				Bidi lineBidi = FBidi.CreateLineBidi(startPos, limitPos);
				charLevels = BidiUtils.getLevels(lineBidi);
				int[] charsVtoL = BidiUtils.createVisualToLogicalMap(charLevels);
				charsLtoV = BidiUtils.createInverseMap(charsVtoL);
			}

			TextLineComponent[] components = MakeComponentsOnRange(startPos, limitPos);

			return new TextLine(FFrc, components, FBaselineOffsets, FChars, startPos, limitPos, charsLtoV, charLevels, FIsDirectionLTR);

		}

		private void EnsureComponents(int start, int limit)
		{

			if (start < FComponentStart || limit > FComponentLimit)
			{
				GenerateComponents(start, limit);
			}
		}

		private void MakeLayoutWindow(int localStart)
		{

			int compStart = localStart;
			int compLimit = FChars.Length;

			// If we've already gone past the layout window, format to end of paragraph
			if (LayoutCount > 0 && !HaveLayoutWindow)
			{
				float avgLineLength = System.Math.Max(LayoutCharCount / LayoutCount, 1);
				compLimit = System.Math.Min(localStart + (int)(avgLineLength * EST_LINES), FChars.Length);
			}

			if (localStart > 0 || compLimit < FChars.Length)
			{
				if (CharIter == null)
				{
					CharIter = new CharArrayIterator(FChars);
				}
				else
				{
					CharIter.Reset(FChars);
				}
				if (FLineBreak == null)
				{
					FLineBreak = BreakIterator.LineInstance;
				}
				FLineBreak.SetText(CharIter);
				if (localStart > 0)
				{
					if (!FLineBreak.IsBoundary(localStart))
					{
						compStart = FLineBreak.Preceding(localStart);
					}
				}
				if (compLimit < FChars.Length)
				{
					if (!FLineBreak.IsBoundary(compLimit))
					{
						compLimit = FLineBreak.Following(compLimit);
					}
				}
			}

			EnsureComponents(compStart, compLimit);
			HaveLayoutWindow = true;
		}

		/// <summary>
		/// Returns the index of the first character which will not fit on
		/// on a line beginning at <code>start</code> and possible
		/// measuring up to <code>maxAdvance</code> in graphical width.
		/// </summary>
		/// <param name="start"> the character index at which to start measuring.
		///  <code>start</code> is an absolute index, not relative to the
		///  start of the paragraph </param>
		/// <param name="maxAdvance"> the graphical width in which the line must fit </param>
		/// <returns> the index after the last character that will fit
		///  on a line beginning at <code>start</code>, which is not longer
		///  than <code>maxAdvance</code> in graphical width </returns>
		/// <exception cref="IllegalArgumentException"> if <code>start</code> is
		///          less than the beginning of the paragraph. </exception>
		public int GetLineBreakIndex(int start, float maxAdvance)
		{

			int localStart = start - FStart;

			if (!HaveLayoutWindow || localStart < FComponentStart || localStart >= FComponentLimit)
			{
				MakeLayoutWindow(localStart);
			}

			return CalcLineBreak(localStart, maxAdvance) + FStart;
		}

		/// <summary>
		/// Returns the graphical width of a line beginning at <code>start</code>
		/// and including characters up to <code>limit</code>.
		/// <code>start</code> and <code>limit</code> are absolute indices,
		/// not relative to the start of the paragraph.
		/// </summary>
		/// <param name="start"> the character index at which to start measuring </param>
		/// <param name="limit"> the character index at which to stop measuring </param>
		/// <returns> the graphical width of a line beginning at <code>start</code>
		///   and including characters up to <code>limit</code> </returns>
		/// <exception cref="IndexOutOfBoundsException"> if <code>limit</code> is less
		///         than <code>start</code> </exception>
		/// <exception cref="IllegalArgumentException"> if <code>start</code> or
		///          <code>limit</code> is not between the beginning of
		///          the paragraph and the end of the paragraph. </exception>
		public float GetAdvanceBetween(int start, int limit)
		{

			int localStart = start - FStart;
			int localLimit = limit - FStart;

			EnsureComponents(localStart, localLimit);
			TextLine line = MakeTextLineOnRange(localStart, localLimit);
			return line.Metrics.Advance;
			// could cache line in case getLayout is called with same start, limit
		}

		/// <summary>
		/// Returns a <code>TextLayout</code> on the given character range.
		/// </summary>
		/// <param name="start"> the index of the first character </param>
		/// <param name="limit"> the index after the last character.  Must be greater
		///   than <code>start</code> </param>
		/// <returns> a <code>TextLayout</code> for the characters beginning at
		///  <code>start</code> up to (but not including) <code>limit</code> </returns>
		/// <exception cref="IndexOutOfBoundsException"> if <code>limit</code> is less
		///         than <code>start</code> </exception>
		/// <exception cref="IllegalArgumentException"> if <code>start</code> or
		///          <code>limit</code> is not between the beginning of
		///          the paragraph and the end of the paragraph. </exception>
		public TextLayout GetLayout(int start, int limit)
		{

			int localStart = start - FStart;
			int localLimit = limit - FStart;

			EnsureComponents(localStart, localLimit);
			TextLine textLine = MakeTextLineOnRange(localStart, localLimit);

			if (localLimit < FChars.Length)
			{
				LayoutCharCount += limit - start;
				LayoutCount++;
			}

			return new TextLayout(textLine, FBaseline, FBaselineOffsets, FJustifyRatio);
		}

		private int FormattedChars = 0;
		private static bool WantStats = false; //"true".equals(System.getProperty("collectStats"));
		private bool CollectStats = false;

		private void PrintStats()
		{
			System.Console.WriteLine("formattedChars: " + FormattedChars);
			//formattedChars = 0;
			CollectStats = false;
		}

		/// <summary>
		/// Updates the <code>TextMeasurer</code> after a single character has
		/// been inserted
		/// into the paragraph currently represented by this
		/// <code>TextMeasurer</code>.  After this call, this
		/// <code>TextMeasurer</code> is equivalent to a new
		/// <code>TextMeasurer</code> created from the text;  however, it will
		/// usually be more efficient to update an existing
		/// <code>TextMeasurer</code> than to create a new one from scratch.
		/// </summary>
		/// <param name="newParagraph"> the text of the paragraph after performing
		/// the insertion.  Cannot be null. </param>
		/// <param name="insertPos"> the position in the text where the character was
		/// inserted.  Must not be less than the start of
		/// <code>newParagraph</code>, and must be less than the end of
		/// <code>newParagraph</code>. </param>
		/// <exception cref="IndexOutOfBoundsException"> if <code>insertPos</code> is less
		///         than the start of <code>newParagraph</code> or greater than
		///         or equal to the end of <code>newParagraph</code> </exception>
		/// <exception cref="NullPointerException"> if <code>newParagraph</code> is
		///         <code>null</code> </exception>
		public void InsertChar(AttributedCharacterIterator newParagraph, int insertPos)
		{

			if (CollectStats)
			{
				PrintStats();
			}
			if (WantStats)
			{
				CollectStats = true;
			}

			FStart = newParagraph.BeginIndex;
			int end = newParagraph.EndIndex;
			if (end - FStart != FChars.Length + 1)
			{
				InitAll(newParagraph);
			}

			char[] newChars = new char[end - FStart];
			int newCharIndex = insertPos - FStart;
			System.Array.Copy(FChars, 0, newChars, 0, newCharIndex);

			char newChar = newParagraph.setIndex(insertPos);
			newChars[newCharIndex] = newChar;
			System.Array.Copy(FChars, newCharIndex, newChars, newCharIndex + 1, end - insertPos - 1);
			FChars = newChars;

			if (FBidi != null || Bidi.RequiresBidi(newChars, newCharIndex, newCharIndex + 1) || newParagraph.GetAttribute(TextAttribute.BIDI_EMBEDDING) != null)
			{

				FBidi = new Bidi(newParagraph);
				if (FBidi.LeftToRight)
				{
					FBidi = null;
				}
			}

			FParagraph = StyledParagraph.InsertChar(newParagraph, FChars, insertPos, FParagraph);
			InvalidateComponents();
		}

		/// <summary>
		/// Updates the <code>TextMeasurer</code> after a single character has
		/// been deleted
		/// from the paragraph currently represented by this
		/// <code>TextMeasurer</code>.  After this call, this
		/// <code>TextMeasurer</code> is equivalent to a new <code>TextMeasurer</code>
		/// created from the text;  however, it will usually be more efficient
		/// to update an existing <code>TextMeasurer</code> than to create a new one
		/// from scratch.
		/// </summary>
		/// <param name="newParagraph"> the text of the paragraph after performing
		/// the deletion.  Cannot be null. </param>
		/// <param name="deletePos"> the position in the text where the character was removed.
		/// Must not be less than
		/// the start of <code>newParagraph</code>, and must not be greater than the
		/// end of <code>newParagraph</code>. </param>
		/// <exception cref="IndexOutOfBoundsException"> if <code>deletePos</code> is
		///         less than the start of <code>newParagraph</code> or greater
		///         than the end of <code>newParagraph</code> </exception>
		/// <exception cref="NullPointerException"> if <code>newParagraph</code> is
		///         <code>null</code> </exception>
		public void DeleteChar(AttributedCharacterIterator newParagraph, int deletePos)
		{

			FStart = newParagraph.BeginIndex;
			int end = newParagraph.EndIndex;
			if (end - FStart != FChars.Length - 1)
			{
				InitAll(newParagraph);
			}

			char[] newChars = new char[end - FStart];
			int changedIndex = deletePos - FStart;

			System.Array.Copy(FChars, 0, newChars, 0, deletePos - FStart);
			System.Array.Copy(FChars, changedIndex + 1, newChars, changedIndex, end - deletePos);
			FChars = newChars;

			if (FBidi != null)
			{
				FBidi = new Bidi(newParagraph);
				if (FBidi.LeftToRight)
				{
					FBidi = null;
				}
			}

			FParagraph = StyledParagraph.DeleteChar(newParagraph, FChars, deletePos, FParagraph);
			InvalidateComponents();
		}

		/// <summary>
		/// NOTE:  This method is only for LineBreakMeasurer's use.  It is package-
		/// private because it returns internal data.
		/// </summary>
		internal char[] Chars
		{
			get
			{
    
				return FChars;
			}
		}
	}

}