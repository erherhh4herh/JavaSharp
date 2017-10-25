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
 * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
 *
 * The original version of this source code and documentation
 * is copyrighted and owned by Taligent, Inc., a wholly-owned
 * subsidiary of IBM. These materials are provided under terms
 * of a License Agreement between Taligent and Sun. This technology
 * is protected by multiple US and International patents.
 *
 * This notice and attribution to Taligent may not be removed.
 * Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.text
{


	/// <summary>
	/// This interface defines a protocol for bidirectional iteration over text.
	/// The iterator iterates over a bounded sequence of characters.  Characters
	/// are indexed with values beginning with the value returned by getBeginIndex() and
	/// continuing through the value returned by getEndIndex()-1.
	/// <para>
	/// Iterators maintain a current character index, whose valid range is from
	/// getBeginIndex() to getEndIndex(); the value getEndIndex() is included to allow
	/// handling of zero-length text ranges and for historical reasons.
	/// The current index can be retrieved by calling getIndex() and set directly
	/// by calling setIndex(), first(), and last().
	/// </para>
	/// <para>
	/// The methods previous() and next() are used for iteration. They return DONE if
	/// they would move outside the range from getBeginIndex() to getEndIndex() -1,
	/// signaling that the iterator has reached the end of the sequence. DONE is
	/// also returned by other methods to indicate that the current index is
	/// outside this range.
	/// 
	/// <P>Examples:<P>
	/// 
	/// Traverse the text from start to finish
	/// <pre>{@code
	/// public void traverseForward(CharacterIterator iter) {
	///     for(char c = iter.first(); c != CharacterIterator.DONE; c = iter.next()) {
	///         processChar(c);
	///     }
	/// }
	/// }</pre>
	/// 
	/// Traverse the text backwards, from end to start
	/// <pre>{@code
	/// public void traverseBackward(CharacterIterator iter) {
	///     for(char c = iter.last(); c != CharacterIterator.DONE; c = iter.previous()) {
	///         processChar(c);
	///     }
	/// }
	/// }</pre>
	/// 
	/// Traverse both forward and backward from a given position in the text.
	/// Calls to notBoundary() in this example represents some
	/// additional stopping criteria.
	/// <pre>{@code
	/// public void traverseOut(CharacterIterator iter, int pos) {
	///     for (char c = iter.setIndex(pos);
	///              c != CharacterIterator.DONE && notBoundary(c);
	///              c = iter.next()) {
	///     }
	///     int end = iter.getIndex();
	///     for (char c = iter.setIndex(pos);
	///             c != CharacterIterator.DONE && notBoundary(c);
	///             c = iter.previous()) {
	///     }
	///     int start = iter.getIndex();
	///     processSection(start, end);
	/// }
	/// }</pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= StringCharacterIterator </seealso>
	/// <seealso cref= AttributedCharacterIterator </seealso>

	public interface CharacterIterator : Cloneable
	{

		/// <summary>
		/// Constant that is returned when the iterator has reached either the end
		/// or the beginning of the text. The value is '\\uFFFF', the "not a
		/// character" value which should not occur in any valid Unicode string.
		/// </summary>

		/// <summary>
		/// Sets the position to getBeginIndex() and returns the character at that
		/// position. </summary>
		/// <returns> the first character in the text, or DONE if the text is empty </returns>
		/// <seealso cref= #getBeginIndex() </seealso>
		char First();

		/// <summary>
		/// Sets the position to getEndIndex()-1 (getEndIndex() if the text is empty)
		/// and returns the character at that position. </summary>
		/// <returns> the last character in the text, or DONE if the text is empty </returns>
		/// <seealso cref= #getEndIndex() </seealso>
		char Last();

		/// <summary>
		/// Gets the character at the current position (as returned by getIndex()). </summary>
		/// <returns> the character at the current position or DONE if the current
		/// position is off the end of the text. </returns>
		/// <seealso cref= #getIndex() </seealso>
		char Current();

		/// <summary>
		/// Increments the iterator's index by one and returns the character
		/// at the new index.  If the resulting index is greater or equal
		/// to getEndIndex(), the current index is reset to getEndIndex() and
		/// a value of DONE is returned. </summary>
		/// <returns> the character at the new position or DONE if the new
		/// position is off the end of the text range. </returns>
		char Next();

		/// <summary>
		/// Decrements the iterator's index by one and returns the character
		/// at the new index. If the current index is getBeginIndex(), the index
		/// remains at getBeginIndex() and a value of DONE is returned. </summary>
		/// <returns> the character at the new position or DONE if the current
		/// position is equal to getBeginIndex(). </returns>
		char Previous();

		/// <summary>
		/// Sets the position to the specified position in the text and returns that
		/// character. </summary>
		/// <param name="position"> the position within the text.  Valid values range from
		/// getBeginIndex() to getEndIndex().  An IllegalArgumentException is thrown
		/// if an invalid value is supplied. </param>
		/// <returns> the character at the specified position or DONE if the specified position is equal to getEndIndex() </returns>
		char SetIndex(int position);

		/// <summary>
		/// Returns the start index of the text. </summary>
		/// <returns> the index at which the text begins. </returns>
		int BeginIndex {get;}

		/// <summary>
		/// Returns the end index of the text.  This index is the index of the first
		/// character following the end of the text. </summary>
		/// <returns> the index after the last character in the text </returns>
		int EndIndex {get;}

		/// <summary>
		/// Returns the current index. </summary>
		/// <returns> the current index. </returns>
		int Index {get;}

		/// <summary>
		/// Create a copy of this iterator </summary>
		/// <returns> A copy of this </returns>
		Object Clone();

	}

	public static class CharacterIterator_Fields
	{
		public const char DONE = '\uFFFF';
	}

}