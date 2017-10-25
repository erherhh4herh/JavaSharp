/*
 * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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

	internal class CharArrayIterator : CharacterIterator
	{

		private char[] Chars;
		private int Pos;
		private int Begin;

		internal CharArrayIterator(char[] chars)
		{

			Reset(chars, 0);
		}

		internal CharArrayIterator(char[] chars, int begin)
		{

			Reset(chars, begin);
		}

		/// <summary>
		/// Sets the position to getBeginIndex() and returns the character at that
		/// position. </summary>
		/// <returns> the first character in the text, or DONE if the text is empty </returns>
		/// <seealso cref= getBeginIndex </seealso>
		public virtual char First()
		{

			Pos = 0;
			return Current();
		}

		/// <summary>
		/// Sets the position to getEndIndex()-1 (getEndIndex() if the text is empty)
		/// and returns the character at that position. </summary>
		/// <returns> the last character in the text, or DONE if the text is empty </returns>
		/// <seealso cref= getEndIndex </seealso>
		public virtual char Last()
		{

			if (Chars.Length > 0)
			{
				Pos = Chars.Length - 1;
			}
			else
			{
				Pos = 0;
			}
			return Current();
		}

		/// <summary>
		/// Gets the character at the current position (as returned by getIndex()). </summary>
		/// <returns> the character at the current position or DONE if the current
		/// position is off the end of the text. </returns>
		/// <seealso cref= getIndex </seealso>
		public virtual char Current()
		{

			if (Pos >= 0 && Pos < Chars.Length)
			{
				return Chars[Pos];
			}
			else
			{
				return java.text.CharacterIterator_Fields.DONE;
			}
		}

		/// <summary>
		/// Increments the iterator's index by one and returns the character
		/// at the new index.  If the resulting index is greater or equal
		/// to getEndIndex(), the current index is reset to getEndIndex() and
		/// a value of DONE is returned. </summary>
		/// <returns> the character at the new position or DONE if the new
		/// position is off the end of the text range. </returns>
		public virtual char Next()
		{

			if (Pos < Chars.Length - 1)
			{
				Pos++;
				return Chars[Pos];
			}
			else
			{
				Pos = Chars.Length;
				return java.text.CharacterIterator_Fields.DONE;
			}
		}

		/// <summary>
		/// Decrements the iterator's index by one and returns the character
		/// at the new index. If the current index is getBeginIndex(), the index
		/// remains at getBeginIndex() and a value of DONE is returned. </summary>
		/// <returns> the character at the new position or DONE if the current
		/// position is equal to getBeginIndex(). </returns>
		public virtual char Previous()
		{

			if (Pos > 0)
			{
				Pos--;
				return Chars[Pos];
			}
			else
			{
				Pos = 0;
				return java.text.CharacterIterator_Fields.DONE;
			}
		}

		/// <summary>
		/// Sets the position to the specified position in the text and returns that
		/// character. </summary>
		/// <param name="position"> the position within the text.  Valid values range from
		/// getBeginIndex() to getEndIndex().  An IllegalArgumentException is thrown
		/// if an invalid value is supplied. </param>
		/// <returns> the character at the specified position or DONE if the specified position is equal to getEndIndex() </returns>
		public virtual char SetIndex(int position)
		{

			position -= Begin;
			if (position < 0 || position > Chars.Length)
			{
				throw new IllegalArgumentException("Invalid index");
			}
			Pos = position;
			return Current();
		}

		/// <summary>
		/// Returns the start index of the text. </summary>
		/// <returns> the index at which the text begins. </returns>
		public virtual int BeginIndex
		{
			get
			{
				return Begin;
			}
		}

		/// <summary>
		/// Returns the end index of the text.  This index is the index of the first
		/// character following the end of the text. </summary>
		/// <returns> the index after the last character in the text </returns>
		public virtual int EndIndex
		{
			get
			{
				return Begin + Chars.Length;
			}
		}

		/// <summary>
		/// Returns the current index. </summary>
		/// <returns> the current index. </returns>
		public virtual int Index
		{
			get
			{
				return Begin + Pos;
			}
		}

		/// <summary>
		/// Create a copy of this iterator </summary>
		/// <returns> A copy of this </returns>
		public virtual Object Clone()
		{
			CharArrayIterator c = new CharArrayIterator(Chars, Begin);
			c.Pos = this.Pos;
			return c;
		}

		internal virtual void Reset(char[] chars)
		{
			Reset(chars, 0);
		}

		internal virtual void Reset(char[] chars, int begin)
		{

			this.Chars = chars;
			this.Begin = begin;
			Pos = 0;
		}
	}

}