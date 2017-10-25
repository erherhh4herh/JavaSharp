/*
 * Copyright (c) 1998, 2003, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.font
{

	/// <summary>
	/// The <code>LineMetrics</code> class allows access to the
	/// metrics needed to layout characters along a line
	/// and to layout of a set of lines.  A <code>LineMetrics</code>
	/// object encapsulates the measurement information associated
	/// with a run of text.
	/// <para>
	/// Fonts can have different metrics for different ranges of
	/// characters.  The <code>getLineMetrics</code> methods of
	/// <seealso cref="java.awt.Font Font"/> take some text as an argument
	/// and return a <code>LineMetrics</code> object describing the
	/// metrics of the initial number of characters in that text, as
	/// returned by <seealso cref="#getNumChars"/>.
	/// </para>
	/// </summary>


	public abstract class LineMetrics
	{


		/// <summary>
		/// Returns the number of characters (<code>char</code> values) in the text whose
		/// metrics are encapsulated by this <code>LineMetrics</code>
		/// object. </summary>
		/// <returns> the number of characters (<code>char</code> values) in the text with which
		///         this <code>LineMetrics</code> was created. </returns>
		public abstract int NumChars {get;}

		/// <summary>
		/// Returns the ascent of the text.  The ascent
		/// is the distance from the baseline
		/// to the ascender line.  The ascent usually represents the
		/// the height of the capital letters of the text.  Some characters
		/// can extend above the ascender line. </summary>
		/// <returns> the ascent of the text. </returns>
		public abstract float Ascent {get;}

		/// <summary>
		/// Returns the descent of the text.  The descent
		/// is the distance from the baseline
		/// to the descender line.  The descent usually represents
		/// the distance to the bottom of lower case letters like
		/// 'p'.  Some characters can extend below the descender
		/// line. </summary>
		/// <returns> the descent of the text. </returns>
		public abstract float Descent {get;}

		/// <summary>
		/// Returns the leading of the text. The
		/// leading is the recommended
		/// distance from the bottom of the descender line to the
		/// top of the next line. </summary>
		/// <returns> the leading of the text. </returns>
		public abstract float Leading {get;}

		/// <summary>
		/// Returns the height of the text.  The
		/// height is equal to the sum of the ascent, the
		/// descent and the leading. </summary>
		/// <returns> the height of the text. </returns>
		public abstract float Height {get;}

		/// <summary>
		/// Returns the baseline index of the text.
		/// The index is one of
		/// <seealso cref="java.awt.Font#ROMAN_BASELINE ROMAN_BASELINE"/>,
		/// <seealso cref="java.awt.Font#CENTER_BASELINE CENTER_BASELINE"/>,
		/// <seealso cref="java.awt.Font#HANGING_BASELINE HANGING_BASELINE"/>. </summary>
		/// <returns> the baseline of the text. </returns>
		public abstract int BaselineIndex {get;}

		/// <summary>
		/// Returns the baseline offsets of the text,
		/// relative to the baseline of the text.  The
		/// offsets are indexed by baseline index.  For
		/// example, if the baseline index is
		/// <code>CENTER_BASELINE</code> then
		/// <code>offsets[HANGING_BASELINE]</code> is usually
		/// negative, <code>offsets[CENTER_BASELINE]</code>
		/// is zero, and <code>offsets[ROMAN_BASELINE]</code>
		/// is usually positive. </summary>
		/// <returns> the baseline offsets of the text. </returns>
		public abstract float[] BaselineOffsets {get;}

		/// <summary>
		/// Returns the position of the strike-through line
		/// relative to the baseline. </summary>
		/// <returns> the position of the strike-through line. </returns>
		public abstract float StrikethroughOffset {get;}

		/// <summary>
		/// Returns the thickness of the strike-through line. </summary>
		/// <returns> the thickness of the strike-through line. </returns>
		public abstract float StrikethroughThickness {get;}

		/// <summary>
		/// Returns the position of the underline relative to
		/// the baseline. </summary>
		/// <returns> the position of the underline. </returns>
		public abstract float UnderlineOffset {get;}

		/// <summary>
		/// Returns the thickness of the underline. </summary>
		/// <returns> the thickness of the underline. </returns>
		public abstract float UnderlineThickness {get;}
	}

}