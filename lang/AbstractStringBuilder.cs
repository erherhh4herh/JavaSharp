using System;

/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang
{

	using FloatingDecimal = sun.misc.FloatingDecimal;

	/// <summary>
	/// A mutable sequence of characters.
	/// <para>
	/// Implements a modifiable string. At any point in time it contains some
	/// particular sequence of characters, but the length and content of the
	/// sequence can be changed through certain method calls.
	/// 
	/// </para>
	/// <para>Unless otherwise noted, passing a {@code null} argument to a constructor
	/// or method in this class will cause a <seealso cref="NullPointerException"/> to be
	/// thrown.
	/// 
	/// @author      Michael McCloskey
	/// @author      Martin Buchholz
	/// @author      Ulf Zibis
	/// @since       1.5
	/// </para>
	/// </summary>
	internal abstract class AbstractStringBuilder : Appendable, CharSequence
	{
		/// <summary>
		/// The value is used for character storage.
		/// </summary>
		internal char[] Value_Renamed;

		/// <summary>
		/// The count is the number of characters used.
		/// </summary>
		internal int Count;

		/// <summary>
		/// This no-arg constructor is necessary for serialization of subclasses.
		/// </summary>
		internal AbstractStringBuilder()
		{
		}

		/// <summary>
		/// Creates an AbstractStringBuilder of the specified capacity.
		/// </summary>
		internal AbstractStringBuilder(int capacity)
		{
			Value_Renamed = new char[capacity];
		}

		/// <summary>
		/// Returns the length (character count).
		/// </summary>
		/// <returns>  the length of the sequence of characters currently
		///          represented by this object </returns>
		public virtual int Length()
		{
			return Count;
		}

		/// <summary>
		/// Returns the current capacity. The capacity is the amount of storage
		/// available for newly inserted characters, beyond which an allocation
		/// will occur.
		/// </summary>
		/// <returns>  the current capacity </returns>
		public virtual int Capacity()
		{
			return Value_Renamed.Length;
		}

		/// <summary>
		/// Ensures that the capacity is at least equal to the specified minimum.
		/// If the current capacity is less than the argument, then a new internal
		/// array is allocated with greater capacity. The new capacity is the
		/// larger of:
		/// <ul>
		/// <li>The {@code minimumCapacity} argument.
		/// <li>Twice the old capacity, plus {@code 2}.
		/// </ul>
		/// If the {@code minimumCapacity} argument is nonpositive, this
		/// method takes no action and simply returns.
		/// Note that subsequent operations on this object can reduce the
		/// actual capacity below that requested here.
		/// </summary>
		/// <param name="minimumCapacity">   the minimum desired capacity. </param>
		public virtual void EnsureCapacity(int minimumCapacity)
		{
			if (minimumCapacity > 0)
			{
				EnsureCapacityInternal(minimumCapacity);
			}
		}

		/// <summary>
		/// This method has the same contract as ensureCapacity, but is
		/// never synchronized.
		/// </summary>
		private void EnsureCapacityInternal(int minimumCapacity)
		{
			// overflow-conscious code
			if (minimumCapacity - Value_Renamed.Length > 0)
			{
				ExpandCapacity(minimumCapacity);
			}
		}

		/// <summary>
		/// This implements the expansion semantics of ensureCapacity with no
		/// size check or synchronization.
		/// </summary>
		internal virtual void ExpandCapacity(int minimumCapacity)
		{
			int newCapacity = Value_Renamed.Length * 2 + 2;
			if (newCapacity - minimumCapacity < 0)
			{
				newCapacity = minimumCapacity;
			}
			if (newCapacity < 0)
			{
				if (minimumCapacity < 0) // overflow
				{
					throw new OutOfMemoryError();
				}
				newCapacity = Integer.MaxValue;
			}
			Value_Renamed = Arrays.CopyOf(Value_Renamed, newCapacity);
		}

		/// <summary>
		/// Attempts to reduce storage used for the character sequence.
		/// If the buffer is larger than necessary to hold its current sequence of
		/// characters, then it may be resized to become more space efficient.
		/// Calling this method may, but is not required to, affect the value
		/// returned by a subsequent call to the <seealso cref="#capacity()"/> method.
		/// </summary>
		public virtual void TrimToSize()
		{
			if (Count < Value_Renamed.Length)
			{
				Value_Renamed = Arrays.CopyOf(Value_Renamed, Count);
			}
		}

		/// <summary>
		/// Sets the length of the character sequence.
		/// The sequence is changed to a new character sequence
		/// whose length is specified by the argument. For every nonnegative
		/// index <i>k</i> less than {@code newLength}, the character at
		/// index <i>k</i> in the new character sequence is the same as the
		/// character at index <i>k</i> in the old sequence if <i>k</i> is less
		/// than the length of the old character sequence; otherwise, it is the
		/// null character {@code '\u005Cu0000'}.
		/// 
		/// In other words, if the {@code newLength} argument is less than
		/// the current length, the length is changed to the specified length.
		/// <para>
		/// If the {@code newLength} argument is greater than or equal
		/// to the current length, sufficient null characters
		/// ({@code '\u005Cu0000'}) are appended so that
		/// length becomes the {@code newLength} argument.
		/// </para>
		/// <para>
		/// The {@code newLength} argument must be greater than or equal
		/// to {@code 0}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newLength">   the new length </param>
		/// <exception cref="IndexOutOfBoundsException">  if the
		///               {@code newLength} argument is negative. </exception>
		public virtual int Length
		{
			set
			{
				if (value < 0)
				{
					throw new StringIndexOutOfBoundsException(value);
				}
				EnsureCapacityInternal(value);
    
				if (Count < value)
				{
					Arrays.Fill(Value_Renamed, Count, value, '\0');
				}
    
				Count = value;
			}
		}

		/// <summary>
		/// Returns the {@code char} value in this sequence at the specified index.
		/// The first {@code char} value is at index {@code 0}, the next at index
		/// {@code 1}, and so on, as in array indexing.
		/// <para>
		/// The index argument must be greater than or equal to
		/// {@code 0}, and less than the length of this sequence.
		/// 
		/// </para>
		/// <para>If the {@code char} value specified by the index is a
		/// <a href="Character.html#unicode">surrogate</a>, the surrogate
		/// value is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index">   the index of the desired {@code char} value. </param>
		/// <returns>     the {@code char} value at the specified index. </returns>
		/// <exception cref="IndexOutOfBoundsException">  if {@code index} is
		///             negative or greater than or equal to {@code length()}. </exception>
		public virtual char CharAt(int index)
		{
			if ((index < 0) || (index >= Count))
			{
				throw new StringIndexOutOfBoundsException(index);
			}
			return Value_Renamed[index];
		}

		/// <summary>
		/// Returns the character (Unicode code point) at the specified
		/// index. The index refers to {@code char} values
		/// (Unicode code units) and ranges from {@code 0} to
		/// <seealso cref="#length()"/>{@code  - 1}.
		/// 
		/// <para> If the {@code char} value specified at the given index
		/// is in the high-surrogate range, the following index is less
		/// than the length of this sequence, and the
		/// {@code char} value at the following index is in the
		/// low-surrogate range, then the supplementary code point
		/// corresponding to this surrogate pair is returned. Otherwise,
		/// the {@code char} value at the given index is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index"> the index to the {@code char} values </param>
		/// <returns>     the code point value of the character at the
		///             {@code index} </returns>
		/// <exception cref="IndexOutOfBoundsException">  if the {@code index}
		///             argument is negative or not less than the length of this
		///             sequence. </exception>
		public virtual int CodePointAt(int index)
		{
			if ((index < 0) || (index >= Count))
			{
				throw new StringIndexOutOfBoundsException(index);
			}
			return Character.CodePointAtImpl(Value_Renamed, index, Count);
		}

		/// <summary>
		/// Returns the character (Unicode code point) before the specified
		/// index. The index refers to {@code char} values
		/// (Unicode code units) and ranges from {@code 1} to {@link
		/// #length()}.
		/// 
		/// <para> If the {@code char} value at {@code (index - 1)}
		/// is in the low-surrogate range, {@code (index - 2)} is not
		/// negative, and the {@code char} value at {@code (index -
		/// 2)} is in the high-surrogate range, then the
		/// supplementary code point value of the surrogate pair is
		/// returned. If the {@code char} value at {@code index -
		/// 1} is an unpaired low-surrogate or a high-surrogate, the
		/// surrogate value is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index"> the index following the code point that should be returned </param>
		/// <returns>    the Unicode code point value before the given index. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the {@code index}
		///            argument is less than 1 or greater than the length
		///            of this sequence. </exception>
		public virtual int CodePointBefore(int index)
		{
			int i = index - 1;
			if ((i < 0) || (i >= Count))
			{
				throw new StringIndexOutOfBoundsException(index);
			}
			return Character.CodePointBeforeImpl(Value_Renamed, index, 0);
		}

		/// <summary>
		/// Returns the number of Unicode code points in the specified text
		/// range of this sequence. The text range begins at the specified
		/// {@code beginIndex} and extends to the {@code char} at
		/// index {@code endIndex - 1}. Thus the length (in
		/// {@code char}s) of the text range is
		/// {@code endIndex-beginIndex}. Unpaired surrogates within
		/// this sequence count as one code point each.
		/// </summary>
		/// <param name="beginIndex"> the index to the first {@code char} of
		/// the text range. </param>
		/// <param name="endIndex"> the index after the last {@code char} of
		/// the text range. </param>
		/// <returns> the number of Unicode code points in the specified text
		/// range </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the
		/// {@code beginIndex} is negative, or {@code endIndex}
		/// is larger than the length of this sequence, or
		/// {@code beginIndex} is larger than {@code endIndex}. </exception>
		public virtual int CodePointCount(int beginIndex, int endIndex)
		{
			if (beginIndex < 0 || endIndex > Count || beginIndex > endIndex)
			{
				throw new IndexOutOfBoundsException();
			}
			return Character.CodePointCountImpl(Value_Renamed, beginIndex, endIndex - beginIndex);
		}

		/// <summary>
		/// Returns the index within this sequence that is offset from the
		/// given {@code index} by {@code codePointOffset} code
		/// points. Unpaired surrogates within the text range given by
		/// {@code index} and {@code codePointOffset} count as
		/// one code point each.
		/// </summary>
		/// <param name="index"> the index to be offset </param>
		/// <param name="codePointOffset"> the offset in code points </param>
		/// <returns> the index within this sequence </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code index}
		///   is negative or larger then the length of this sequence,
		///   or if {@code codePointOffset} is positive and the subsequence
		///   starting with {@code index} has fewer than
		///   {@code codePointOffset} code points,
		///   or if {@code codePointOffset} is negative and the subsequence
		///   before {@code index} has fewer than the absolute value of
		///   {@code codePointOffset} code points. </exception>
		public virtual int OffsetByCodePoints(int index, int codePointOffset)
		{
			if (index < 0 || index > Count)
			{
				throw new IndexOutOfBoundsException();
			}
			return Character.OffsetByCodePointsImpl(Value_Renamed, 0, Count, index, codePointOffset);
		}

		/// <summary>
		/// Characters are copied from this sequence into the
		/// destination character array {@code dst}. The first character to
		/// be copied is at index {@code srcBegin}; the last character to
		/// be copied is at index {@code srcEnd-1}. The total number of
		/// characters to be copied is {@code srcEnd-srcBegin}. The
		/// characters are copied into the subarray of {@code dst} starting
		/// at index {@code dstBegin} and ending at index:
		/// <pre>{@code
		/// dstbegin + (srcEnd-srcBegin) - 1
		/// }</pre>
		/// </summary>
		/// <param name="srcBegin">   start copying at this offset. </param>
		/// <param name="srcEnd">     stop copying at this offset. </param>
		/// <param name="dst">        the array to copy the data into. </param>
		/// <param name="dstBegin">   offset into {@code dst}. </param>
		/// <exception cref="IndexOutOfBoundsException">  if any of the following is true:
		///             <ul>
		///             <li>{@code srcBegin} is negative
		///             <li>{@code dstBegin} is negative
		///             <li>the {@code srcBegin} argument is greater than
		///             the {@code srcEnd} argument.
		///             <li>{@code srcEnd} is greater than
		///             {@code this.length()}.
		///             <li>{@code dstBegin+srcEnd-srcBegin} is greater than
		///             {@code dst.length}
		///             </ul> </exception>
		public virtual void GetChars(int srcBegin, int srcEnd, char[] dst, int dstBegin)
		{
			if (srcBegin < 0)
			{
				throw new StringIndexOutOfBoundsException(srcBegin);
			}
			if ((srcEnd < 0) || (srcEnd > Count))
			{
				throw new StringIndexOutOfBoundsException(srcEnd);
			}
			if (srcBegin > srcEnd)
			{
				throw new StringIndexOutOfBoundsException("srcBegin > srcEnd");
			}
			System.Array.Copy(Value_Renamed, srcBegin, dst, dstBegin, srcEnd - srcBegin);
		}

		/// <summary>
		/// The character at the specified index is set to {@code ch}. This
		/// sequence is altered to represent a new character sequence that is
		/// identical to the old character sequence, except that it contains the
		/// character {@code ch} at position {@code index}.
		/// <para>
		/// The index argument must be greater than or equal to
		/// {@code 0}, and less than the length of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index">   the index of the character to modify. </param>
		/// <param name="ch">      the new character. </param>
		/// <exception cref="IndexOutOfBoundsException">  if {@code index} is
		///             negative or greater than or equal to {@code length()}. </exception>
		public virtual void SetCharAt(int index, char ch)
		{
			if ((index < 0) || (index >= Count))
			{
				throw new StringIndexOutOfBoundsException(index);
			}
			Value_Renamed[index] = ch;
		}

		/// <summary>
		/// Appends the string representation of the {@code Object} argument.
		/// <para>
		/// The overall effect is exactly as if the argument were converted
		/// to a string by the method <seealso cref="String#valueOf(Object)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#append(String) appended"/> to this character sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="obj">   an {@code Object}. </param>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Append(Object obj)
		{
			return Append(Convert.ToString(obj));
		}

		/// <summary>
		/// Appends the specified string to this character sequence.
		/// <para>
		/// The characters of the {@code String} argument are appended, in
		/// order, increasing the length of this sequence by the length of the
		/// argument. If {@code str} is {@code null}, then the four
		/// characters {@code "null"} are appended.
		/// </para>
		/// <para>
		/// Let <i>n</i> be the length of this character sequence just prior to
		/// execution of the {@code append} method. Then the character at
		/// index <i>k</i> in the new character sequence is equal to the character
		/// at index <i>k</i> in the old character sequence, if <i>k</i> is less
		/// than <i>n</i>; otherwise, it is equal to the character at index
		/// <i>k-n</i> in the argument {@code str}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">   a string. </param>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Append(String str)
		{
			if (str == null)
			{
				return AppendNull();
			}
			int len = str.Length();
			EnsureCapacityInternal(Count + len);
			str.GetChars(0, len, Value_Renamed, Count);
			Count += len;
			return this;
		}

		// Documentation in subclasses because of synchro difference
		public virtual AbstractStringBuilder Append(StringBuffer sb)
		{
			if (sb == null)
			{
				return AppendNull();
			}
			int len = sb.Length();
			EnsureCapacityInternal(Count + len);
			sb.GetChars(0, len, Value_Renamed, Count);
			Count += len;
			return this;
		}

		/// <summary>
		/// @since 1.8
		/// </summary>
		internal virtual AbstractStringBuilder Append(AbstractStringBuilder asb)
		{
			if (asb == null)
			{
				return AppendNull();
			}
			int len = asb.Length();
			EnsureCapacityInternal(Count + len);
			asb.GetChars(0, len, Value_Renamed, Count);
			Count += len;
			return this;
		}

		// Documentation in subclasses because of synchro difference
		public virtual AbstractStringBuilder Append(CharSequence s)
		{
			if (s == null)
			{
				return AppendNull();
			}
			if (s is String)
			{
				return this.Append((String)s);
			}
			if (s is AbstractStringBuilder)
			{
				return this.Append((AbstractStringBuilder)s);
			}

			return this.Append(s, 0, s.Length());
		}

		private AbstractStringBuilder AppendNull()
		{
			int c = Count;
			EnsureCapacityInternal(c + 4);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] value = this.value;
			char[] value = this.Value_Renamed;
			value[c++] = 'n';
			value[c++] = 'u';
			value[c++] = 'l';
			value[c++] = 'l';
			Count = c;
			return this;
		}

		/// <summary>
		/// Appends a subsequence of the specified {@code CharSequence} to this
		/// sequence.
		/// <para>
		/// Characters of the argument {@code s}, starting at
		/// index {@code start}, are appended, in order, to the contents of
		/// this sequence up to the (exclusive) index {@code end}. The length
		/// of this sequence is increased by the value of {@code end - start}.
		/// </para>
		/// <para>
		/// Let <i>n</i> be the length of this character sequence just prior to
		/// execution of the {@code append} method. Then the character at
		/// index <i>k</i> in this character sequence becomes equal to the
		/// character at index <i>k</i> in this sequence, if <i>k</i> is less than
		/// <i>n</i>; otherwise, it is equal to the character at index
		/// <i>k+start-n</i> in the argument {@code s}.
		/// </para>
		/// <para>
		/// If {@code s} is {@code null}, then this method appends
		/// characters as if the s parameter was a sequence containing the four
		/// characters {@code "null"}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="s"> the sequence to append. </param>
		/// <param name="start">   the starting index of the subsequence to be appended. </param>
		/// <param name="end">     the end index of the subsequence to be appended. </param>
		/// <returns>  a reference to this object. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if
		///             {@code start} is negative, or
		///             {@code start} is greater than {@code end} or
		///             {@code end} is greater than {@code s.length()} </exception>
		public virtual AbstractStringBuilder Append(CharSequence s, int start, int end)
		{
			if (s == null)
			{
				s = "null";
			}
			if ((start < 0) || (start > end) || (end > s.Length()))
			{
				throw new IndexOutOfBoundsException("start " + start + ", end " + end + ", s.length() " + s.Length());
			}
			int len = end - start;
			EnsureCapacityInternal(Count + len);
			for (int i = start, j = Count; i < end; i++, j++)
			{
				Value_Renamed[j] = s.CharAt(i);
			}
			Count += len;
			return this;
		}

		/// <summary>
		/// Appends the string representation of the {@code char} array
		/// argument to this sequence.
		/// <para>
		/// The characters of the array argument are appended, in order, to
		/// the contents of this sequence. The length of this sequence
		/// increases by the length of the argument.
		/// </para>
		/// <para>
		/// The overall effect is exactly as if the argument were converted
		/// to a string by the method <seealso cref="String#valueOf(char[])"/>,
		/// and the characters of that string were then
		/// <seealso cref="#append(String) appended"/> to this character sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">   the characters to be appended. </param>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Append(char[] str)
		{
			int len = str.Length;
			EnsureCapacityInternal(Count + len);
			System.Array.Copy(str, 0, Value_Renamed, Count, len);
			Count += len;
			return this;
		}

		/// <summary>
		/// Appends the string representation of a subarray of the
		/// {@code char} array argument to this sequence.
		/// <para>
		/// Characters of the {@code char} array {@code str}, starting at
		/// index {@code offset}, are appended, in order, to the contents
		/// of this sequence. The length of this sequence increases
		/// by the value of {@code len}.
		/// </para>
		/// <para>
		/// The overall effect is exactly as if the arguments were converted
		/// to a string by the method <seealso cref="String#valueOf(char[],int,int)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#append(String) appended"/> to this character sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">      the characters to be appended. </param>
		/// <param name="offset">   the index of the first {@code char} to append. </param>
		/// <param name="len">      the number of {@code char}s to append. </param>
		/// <returns>  a reference to this object. </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///         if {@code offset < 0} or {@code len < 0}
		///         or {@code offset+len > str.length} </exception>
		public virtual AbstractStringBuilder Append(char[] str, int offset, int len)
		{
			if (len > 0) // let arraycopy report AIOOBE for len < 0
			{
				EnsureCapacityInternal(Count + len);
			}
			System.Array.Copy(str, offset, Value_Renamed, Count, len);
			Count += len;
			return this;
		}

		/// <summary>
		/// Appends the string representation of the {@code boolean}
		/// argument to the sequence.
		/// <para>
		/// The overall effect is exactly as if the argument were converted
		/// to a string by the method <seealso cref="String#valueOf(boolean)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#append(String) appended"/> to this character sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b">   a {@code boolean}. </param>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Append(bool b)
		{
			if (b)
			{
				EnsureCapacityInternal(Count + 4);
				Value_Renamed[Count++] = 't';
				Value_Renamed[Count++] = 'r';
				Value_Renamed[Count++] = 'u';
				Value_Renamed[Count++] = 'e';
			}
			else
			{
				EnsureCapacityInternal(Count + 5);
				Value_Renamed[Count++] = 'f';
				Value_Renamed[Count++] = 'a';
				Value_Renamed[Count++] = 'l';
				Value_Renamed[Count++] = 's';
				Value_Renamed[Count++] = 'e';
			}
			return this;
		}

		/// <summary>
		/// Appends the string representation of the {@code char}
		/// argument to this sequence.
		/// <para>
		/// The argument is appended to the contents of this sequence.
		/// The length of this sequence increases by {@code 1}.
		/// </para>
		/// <para>
		/// The overall effect is exactly as if the argument were converted
		/// to a string by the method <seealso cref="String#valueOf(char)"/>,
		/// and the character in that string were then
		/// <seealso cref="#append(String) appended"/> to this character sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c">   a {@code char}. </param>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Append(char c)
		{
			EnsureCapacityInternal(Count + 1);
			Value_Renamed[Count++] = c;
			return this;
		}

		/// <summary>
		/// Appends the string representation of the {@code int}
		/// argument to this sequence.
		/// <para>
		/// The overall effect is exactly as if the argument were converted
		/// to a string by the method <seealso cref="String#valueOf(int)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#append(String) appended"/> to this character sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">   an {@code int}. </param>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Append(int i)
		{
			if (i == Integer.MinValue)
			{
				Append("-2147483648");
				return this;
			}
			int appendedLength = (i < 0) ? Integer.StringSize(-i) + 1 : Integer.StringSize(i);
			int spaceNeeded = Count + appendedLength;
			EnsureCapacityInternal(spaceNeeded);
			Integer.GetChars(i, spaceNeeded, Value_Renamed);
			Count = spaceNeeded;
			return this;
		}

		/// <summary>
		/// Appends the string representation of the {@code long}
		/// argument to this sequence.
		/// <para>
		/// The overall effect is exactly as if the argument were converted
		/// to a string by the method <seealso cref="String#valueOf(long)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#append(String) appended"/> to this character sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   a {@code long}. </param>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Append(long l)
		{
			if (l == Long.MinValue)
			{
				Append("-9223372036854775808");
				return this;
			}
			int appendedLength = (l < 0) ? Long.StringSize(-l) + 1 : Long.StringSize(l);
			int spaceNeeded = Count + appendedLength;
			EnsureCapacityInternal(spaceNeeded);
			Long.GetChars(l, spaceNeeded, Value_Renamed);
			Count = spaceNeeded;
			return this;
		}

		/// <summary>
		/// Appends the string representation of the {@code float}
		/// argument to this sequence.
		/// <para>
		/// The overall effect is exactly as if the argument were converted
		/// to a string by the method <seealso cref="String#valueOf(float)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#append(String) appended"/> to this character sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="f">   a {@code float}. </param>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Append(float f)
		{
			FloatingDecimal.appendTo(f,this);
			return this;
		}

		/// <summary>
		/// Appends the string representation of the {@code double}
		/// argument to this sequence.
		/// <para>
		/// The overall effect is exactly as if the argument were converted
		/// to a string by the method <seealso cref="String#valueOf(double)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#append(String) appended"/> to this character sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="d">   a {@code double}. </param>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Append(double d)
		{
			FloatingDecimal.appendTo(d,this);
			return this;
		}

		/// <summary>
		/// Removes the characters in a substring of this sequence.
		/// The substring begins at the specified {@code start} and extends to
		/// the character at index {@code end - 1} or to the end of the
		/// sequence if no such character exists. If
		/// {@code start} is equal to {@code end}, no changes are made.
		/// </summary>
		/// <param name="start">  The beginning index, inclusive. </param>
		/// <param name="end">    The ending index, exclusive. </param>
		/// <returns>     This object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if {@code start}
		///             is negative, greater than {@code length()}, or
		///             greater than {@code end}. </exception>
		public virtual AbstractStringBuilder Delete(int start, int end)
		{
			if (start < 0)
			{
				throw new StringIndexOutOfBoundsException(start);
			}
			if (end > Count)
			{
				end = Count;
			}
			if (start > end)
			{
				throw new StringIndexOutOfBoundsException();
			}
			int len = end - start;
			if (len > 0)
			{
				System.Array.Copy(Value_Renamed, start + len, Value_Renamed, start, Count - end);
				Count -= len;
			}
			return this;
		}

		/// <summary>
		/// Appends the string representation of the {@code codePoint}
		/// argument to this sequence.
		/// 
		/// <para> The argument is appended to the contents of this sequence.
		/// The length of this sequence increases by
		/// <seealso cref="Character#charCount(int) Character.charCount(codePoint)"/>.
		/// 
		/// </para>
		/// <para> The overall effect is exactly as if the argument were
		/// converted to a {@code char} array by the method
		/// <seealso cref="Character#toChars(int)"/> and the character in that array
		/// were then <seealso cref="#append(char[]) appended"/> to this character
		/// sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="codePoint">   a Unicode code point </param>
		/// <returns>  a reference to this object. </returns>
		/// <exception cref="IllegalArgumentException"> if the specified
		/// {@code codePoint} isn't a valid Unicode code point </exception>
		public virtual AbstractStringBuilder AppendCodePoint(int codePoint)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = this.count;
			int count = this.Count;

			if (Character.IsBmpCodePoint(codePoint))
			{
				EnsureCapacityInternal(count + 1);
				Value_Renamed[count] = (char) codePoint;
				this.Count = count + 1;
			}
			else if (Character.IsValidCodePoint(codePoint))
			{
				EnsureCapacityInternal(count + 2);
				Character.ToSurrogates(codePoint, Value_Renamed, count);
				this.Count = count + 2;
			}
			else
			{
				throw new IllegalArgumentException();
			}
			return this;
		}

		/// <summary>
		/// Removes the {@code char} at the specified position in this
		/// sequence. This sequence is shortened by one {@code char}.
		/// 
		/// <para>Note: If the character at the given index is a supplementary
		/// character, this method does not remove the entire character. If
		/// correct handling of supplementary characters is required,
		/// determine the number of {@code char}s to remove by calling
		/// {@code Character.charCount(thisSequence.codePointAt(index))},
		/// where {@code thisSequence} is this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index">  Index of {@code char} to remove </param>
		/// <returns>      This object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if the {@code index}
		///              is negative or greater than or equal to
		///              {@code length()}. </exception>
		public virtual AbstractStringBuilder DeleteCharAt(int index)
		{
			if ((index < 0) || (index >= Count))
			{
				throw new StringIndexOutOfBoundsException(index);
			}
			System.Array.Copy(Value_Renamed, index + 1, Value_Renamed, index, Count - index - 1);
			Count--;
			return this;
		}

		/// <summary>
		/// Replaces the characters in a substring of this sequence
		/// with characters in the specified {@code String}. The substring
		/// begins at the specified {@code start} and extends to the character
		/// at index {@code end - 1} or to the end of the
		/// sequence if no such character exists. First the
		/// characters in the substring are removed and then the specified
		/// {@code String} is inserted at {@code start}. (This
		/// sequence will be lengthened to accommodate the
		/// specified String if necessary.)
		/// </summary>
		/// <param name="start">    The beginning index, inclusive. </param>
		/// <param name="end">      The ending index, exclusive. </param>
		/// <param name="str">   String that will replace previous contents. </param>
		/// <returns>     This object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if {@code start}
		///             is negative, greater than {@code length()}, or
		///             greater than {@code end}. </exception>
		public virtual AbstractStringBuilder Replace(int start, int end, String str)
		{
			if (start < 0)
			{
				throw new StringIndexOutOfBoundsException(start);
			}
			if (start > Count)
			{
				throw new StringIndexOutOfBoundsException("start > length()");
			}
			if (start > end)
			{
				throw new StringIndexOutOfBoundsException("start > end");
			}

			if (end > Count)
			{
				end = Count;
			}
			int len = str.Length();
			int newCount = Count + len - (end - start);
			EnsureCapacityInternal(newCount);

			System.Array.Copy(Value_Renamed, end, Value_Renamed, start + len, Count - end);
			str.GetChars(Value_Renamed, start);
			Count = newCount;
			return this;
		}

		/// <summary>
		/// Returns a new {@code String} that contains a subsequence of
		/// characters currently contained in this character sequence. The
		/// substring begins at the specified index and extends to the end of
		/// this sequence.
		/// </summary>
		/// <param name="start">    The beginning index, inclusive. </param>
		/// <returns>     The new string. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if {@code start} is
		///             less than zero, or greater than the length of this object. </exception>
		public virtual String Substring(int start)
		{
			return Substring(start, Count);
		}

		/// <summary>
		/// Returns a new character sequence that is a subsequence of this sequence.
		/// 
		/// <para> An invocation of this method of the form
		/// 
		/// <pre>{@code
		/// sb.subSequence(begin,&nbsp;end)}</pre>
		/// 
		/// behaves in exactly the same way as the invocation
		/// 
		/// <pre>{@code
		/// sb.substring(begin,&nbsp;end)}</pre>
		/// 
		/// This method is provided so that this class can
		/// implement the <seealso cref="CharSequence"/> interface.
		/// 
		/// </para>
		/// </summary>
		/// <param name="start">   the start index, inclusive. </param>
		/// <param name="end">     the end index, exclusive. </param>
		/// <returns>     the specified subsequence.
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          if {@code start} or {@code end} are negative,
		///          if {@code end} is greater than {@code length()},
		///          or if {@code start} is greater than {@code end}
		/// @spec JSR-51 </exception>
		public virtual CharSequence SubSequence(int start, int end)
		{
			return Substring(start, end);
		}

		/// <summary>
		/// Returns a new {@code String} that contains a subsequence of
		/// characters currently contained in this sequence. The
		/// substring begins at the specified {@code start} and
		/// extends to the character at index {@code end - 1}.
		/// </summary>
		/// <param name="start">    The beginning index, inclusive. </param>
		/// <param name="end">      The ending index, exclusive. </param>
		/// <returns>     The new string. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if {@code start}
		///             or {@code end} are negative or greater than
		///             {@code length()}, or {@code start} is
		///             greater than {@code end}. </exception>
		public virtual String Substring(int start, int end)
		{
			if (start < 0)
			{
				throw new StringIndexOutOfBoundsException(start);
			}
			if (end > Count)
			{
				throw new StringIndexOutOfBoundsException(end);
			}
			if (start > end)
			{
				throw new StringIndexOutOfBoundsException(end - start);
			}
			return new String(Value_Renamed, start, end - start);
		}

		/// <summary>
		/// Inserts the string representation of a subarray of the {@code str}
		/// array argument into this sequence. The subarray begins at the
		/// specified {@code offset} and extends {@code len} {@code char}s.
		/// The characters of the subarray are inserted into this sequence at
		/// the position indicated by {@code index}. The length of this
		/// sequence increases by {@code len} {@code char}s.
		/// </summary>
		/// <param name="index">    position at which to insert subarray. </param>
		/// <param name="str">       A {@code char} array. </param>
		/// <param name="offset">   the index of the first {@code char} in subarray to
		///             be inserted. </param>
		/// <param name="len">      the number of {@code char}s in the subarray to
		///             be inserted. </param>
		/// <returns>     This object </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if {@code index}
		///             is negative or greater than {@code length()}, or
		///             {@code offset} or {@code len} are negative, or
		///             {@code (offset+len)} is greater than
		///             {@code str.length}. </exception>
		public virtual AbstractStringBuilder Insert(int index, char[] str, int offset, int len)
		{
			if ((index < 0) || (index > Length()))
			{
				throw new StringIndexOutOfBoundsException(index);
			}
			if ((offset < 0) || (len < 0) || (offset > str.Length - len))
			{
				throw new StringIndexOutOfBoundsException("offset " + offset + ", len " + len + ", str.length " + str.Length);
			}
			EnsureCapacityInternal(Count + len);
			System.Array.Copy(Value_Renamed, index, Value_Renamed, index + len, Count - index);
			System.Array.Copy(str, offset, Value_Renamed, index, len);
			Count += len;
			return this;
		}

		/// <summary>
		/// Inserts the string representation of the {@code Object}
		/// argument into this character sequence.
		/// <para>
		/// The overall effect is exactly as if the second argument were
		/// converted to a string by the method <seealso cref="String#valueOf(Object)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#insert(int,String) inserted"/> into this character
		/// sequence at the indicated offset.
		/// </para>
		/// <para>
		/// The {@code offset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">   the offset. </param>
		/// <param name="obj">      an {@code Object}. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int offset, Object obj)
		{
			return Insert(offset, Convert.ToString(obj));
		}

		/// <summary>
		/// Inserts the string into this character sequence.
		/// <para>
		/// The characters of the {@code String} argument are inserted, in
		/// order, into this sequence at the indicated offset, moving up any
		/// characters originally above that position and increasing the length
		/// of this sequence by the length of the argument. If
		/// {@code str} is {@code null}, then the four characters
		/// {@code "null"} are inserted into this sequence.
		/// </para>
		/// <para>
		/// The character at index <i>k</i> in the new character sequence is
		/// equal to:
		/// <ul>
		/// <li>the character at index <i>k</i> in the old character sequence, if
		/// <i>k</i> is less than {@code offset}
		/// <li>the character at index <i>k</i>{@code -offset} in the
		/// argument {@code str}, if <i>k</i> is not less than
		/// {@code offset} but is less than {@code offset+str.length()}
		/// <li>the character at index <i>k</i>{@code -str.length()} in the
		/// old character sequence, if <i>k</i> is not less than
		/// {@code offset+str.length()}
		/// </para>
		/// </ul><para>
		/// The {@code offset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">   the offset. </param>
		/// <param name="str">      a string. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int offset, String str)
		{
			if ((offset < 0) || (offset > Length()))
			{
				throw new StringIndexOutOfBoundsException(offset);
			}
			if (str == null)
			{
				str = "null";
			}
			int len = str.Length();
			EnsureCapacityInternal(Count + len);
			System.Array.Copy(Value_Renamed, offset, Value_Renamed, offset + len, Count - offset);
			str.GetChars(Value_Renamed, offset);
			Count += len;
			return this;
		}

		/// <summary>
		/// Inserts the string representation of the {@code char} array
		/// argument into this sequence.
		/// <para>
		/// The characters of the array argument are inserted into the
		/// contents of this sequence at the position indicated by
		/// {@code offset}. The length of this sequence increases by
		/// the length of the argument.
		/// </para>
		/// <para>
		/// The overall effect is exactly as if the second argument were
		/// converted to a string by the method <seealso cref="String#valueOf(char[])"/>,
		/// and the characters of that string were then
		/// <seealso cref="#insert(int,String) inserted"/> into this character
		/// sequence at the indicated offset.
		/// </para>
		/// <para>
		/// The {@code offset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">   the offset. </param>
		/// <param name="str">      a character array. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int offset, char[] str)
		{
			if ((offset < 0) || (offset > Length()))
			{
				throw new StringIndexOutOfBoundsException(offset);
			}
			int len = str.Length;
			EnsureCapacityInternal(Count + len);
			System.Array.Copy(Value_Renamed, offset, Value_Renamed, offset + len, Count - offset);
			System.Array.Copy(str, 0, Value_Renamed, offset, len);
			Count += len;
			return this;
		}

		/// <summary>
		/// Inserts the specified {@code CharSequence} into this sequence.
		/// <para>
		/// The characters of the {@code CharSequence} argument are inserted,
		/// in order, into this sequence at the indicated offset, moving up
		/// any characters originally above that position and increasing the length
		/// of this sequence by the length of the argument s.
		/// </para>
		/// <para>
		/// The result of this method is exactly the same as if it were an
		/// invocation of this object's
		/// <seealso cref="#insert(int,CharSequence,int,int) insert"/>(dstOffset, s, 0, s.length())
		/// method.
		/// 
		/// </para>
		/// <para>If {@code s} is {@code null}, then the four characters
		/// {@code "null"} are inserted into this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dstOffset">   the offset. </param>
		/// <param name="s"> the sequence to be inserted </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="IndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int dstOffset, CharSequence s)
		{
			if (s == null)
			{
				s = "null";
			}
			if (s is String)
			{
				return this.Insert(dstOffset, (String)s);
			}
			return this.Insert(dstOffset, s, 0, s.Length());
		}

		/// <summary>
		/// Inserts a subsequence of the specified {@code CharSequence} into
		/// this sequence.
		/// <para>
		/// The subsequence of the argument {@code s} specified by
		/// {@code start} and {@code end} are inserted,
		/// in order, into this sequence at the specified destination offset, moving
		/// up any characters originally above that position. The length of this
		/// sequence is increased by {@code end - start}.
		/// </para>
		/// <para>
		/// The character at index <i>k</i> in this sequence becomes equal to:
		/// <ul>
		/// <li>the character at index <i>k</i> in this sequence, if
		/// <i>k</i> is less than {@code dstOffset}
		/// <li>the character at index <i>k</i>{@code +start-dstOffset} in
		/// the argument {@code s}, if <i>k</i> is greater than or equal to
		/// {@code dstOffset} but is less than {@code dstOffset+end-start}
		/// <li>the character at index <i>k</i>{@code -(end-start)} in this
		/// sequence, if <i>k</i> is greater than or equal to
		/// {@code dstOffset+end-start}
		/// </para>
		/// </ul><para>
		/// The {@code dstOffset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// </para>
		/// <para>The start argument must be nonnegative, and not greater than
		/// {@code end}.
		/// </para>
		/// <para>The end argument must be greater than or equal to
		/// {@code start}, and less than or equal to the length of s.
		/// 
		/// </para>
		/// <para>If {@code s} is {@code null}, then this method inserts
		/// characters as if the s parameter was a sequence containing the four
		/// characters {@code "null"}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="dstOffset">   the offset in this sequence. </param>
		/// <param name="s">       the sequence to be inserted. </param>
		/// <param name="start">   the starting index of the subsequence to be inserted. </param>
		/// <param name="end">     the end index of the subsequence to be inserted. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="IndexOutOfBoundsException">  if {@code dstOffset}
		///             is negative or greater than {@code this.length()}, or
		///              {@code start} or {@code end} are negative, or
		///              {@code start} is greater than {@code end} or
		///              {@code end} is greater than {@code s.length()} </exception>
		 public virtual AbstractStringBuilder Insert(int dstOffset, CharSequence s, int start, int end)
		 {
			if (s == null)
			{
				s = "null";
			}
			if ((dstOffset < 0) || (dstOffset > this.Length()))
			{
				throw new IndexOutOfBoundsException("dstOffset " + dstOffset);
			}
			if ((start < 0) || (end < 0) || (start > end) || (end > s.Length()))
			{
				throw new IndexOutOfBoundsException("start " + start + ", end " + end + ", s.length() " + s.Length());
			}
			int len = end - start;
			EnsureCapacityInternal(Count + len);
			System.Array.Copy(Value_Renamed, dstOffset, Value_Renamed, dstOffset + len, Count - dstOffset);
			for (int i = start; i < end; i++)
			{
				Value_Renamed[dstOffset++] = s.CharAt(i);
			}
			Count += len;
			return this;
		 }

		/// <summary>
		/// Inserts the string representation of the {@code boolean}
		/// argument into this sequence.
		/// <para>
		/// The overall effect is exactly as if the second argument were
		/// converted to a string by the method <seealso cref="String#valueOf(boolean)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#insert(int,String) inserted"/> into this character
		/// sequence at the indicated offset.
		/// </para>
		/// <para>
		/// The {@code offset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">   the offset. </param>
		/// <param name="b">        a {@code boolean}. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int offset, bool b)
		{
			return Insert(offset, Convert.ToString(b));
		}

		/// <summary>
		/// Inserts the string representation of the {@code char}
		/// argument into this sequence.
		/// <para>
		/// The overall effect is exactly as if the second argument were
		/// converted to a string by the method <seealso cref="String#valueOf(char)"/>,
		/// and the character in that string were then
		/// <seealso cref="#insert(int,String) inserted"/> into this character
		/// sequence at the indicated offset.
		/// </para>
		/// <para>
		/// The {@code offset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">   the offset. </param>
		/// <param name="c">        a {@code char}. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="IndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int offset, char c)
		{
			EnsureCapacityInternal(Count + 1);
			System.Array.Copy(Value_Renamed, offset, Value_Renamed, offset + 1, Count - offset);
			Value_Renamed[offset] = c;
			Count += 1;
			return this;
		}

		/// <summary>
		/// Inserts the string representation of the second {@code int}
		/// argument into this sequence.
		/// <para>
		/// The overall effect is exactly as if the second argument were
		/// converted to a string by the method <seealso cref="String#valueOf(int)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#insert(int,String) inserted"/> into this character
		/// sequence at the indicated offset.
		/// </para>
		/// <para>
		/// The {@code offset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">   the offset. </param>
		/// <param name="i">        an {@code int}. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int offset, int i)
		{
			return Insert(offset, Convert.ToString(i));
		}

		/// <summary>
		/// Inserts the string representation of the {@code long}
		/// argument into this sequence.
		/// <para>
		/// The overall effect is exactly as if the second argument were
		/// converted to a string by the method <seealso cref="String#valueOf(long)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#insert(int,String) inserted"/> into this character
		/// sequence at the indicated offset.
		/// </para>
		/// <para>
		/// The {@code offset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">   the offset. </param>
		/// <param name="l">        a {@code long}. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int offset, long l)
		{
			return Insert(offset, Convert.ToString(l));
		}

		/// <summary>
		/// Inserts the string representation of the {@code float}
		/// argument into this sequence.
		/// <para>
		/// The overall effect is exactly as if the second argument were
		/// converted to a string by the method <seealso cref="String#valueOf(float)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#insert(int,String) inserted"/> into this character
		/// sequence at the indicated offset.
		/// </para>
		/// <para>
		/// The {@code offset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">   the offset. </param>
		/// <param name="f">        a {@code float}. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int offset, float f)
		{
			return Insert(offset, Convert.ToString(f));
		}

		/// <summary>
		/// Inserts the string representation of the {@code double}
		/// argument into this sequence.
		/// <para>
		/// The overall effect is exactly as if the second argument were
		/// converted to a string by the method <seealso cref="String#valueOf(double)"/>,
		/// and the characters of that string were then
		/// <seealso cref="#insert(int,String) inserted"/> into this character
		/// sequence at the indicated offset.
		/// </para>
		/// <para>
		/// The {@code offset} argument must be greater than or equal to
		/// {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		/// of this sequence.
		/// 
		/// </para>
		/// </summary>
		/// <param name="offset">   the offset. </param>
		/// <param name="d">        a {@code double}. </param>
		/// <returns>     a reference to this object. </returns>
		/// <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public virtual AbstractStringBuilder Insert(int offset, double d)
		{
			return Insert(offset, Convert.ToString(d));
		}

		/// <summary>
		/// Returns the index within this string of the first occurrence of the
		/// specified substring. The integer returned is the smallest value
		/// <i>k</i> such that:
		/// <pre>{@code
		/// this.toString().startsWith(str, <i>k</i>)
		/// }</pre>
		/// is {@code true}.
		/// </summary>
		/// <param name="str">   any string. </param>
		/// <returns>  if the string argument occurs as a substring within this
		///          object, then the index of the first character of the first
		///          such substring is returned; if it does not occur as a
		///          substring, {@code -1} is returned. </returns>
		public virtual int IndexOf(String str)
		{
			return IndexOf(str, 0);
		}

		/// <summary>
		/// Returns the index within this string of the first occurrence of the
		/// specified substring, starting at the specified index.  The integer
		/// returned is the smallest value {@code k} for which:
		/// <pre>{@code
		///     k >= Math.min(fromIndex, this.length()) &&
		///                   this.toString().startsWith(str, k)
		/// }</pre>
		/// If no such value of <i>k</i> exists, then -1 is returned.
		/// </summary>
		/// <param name="str">         the substring for which to search. </param>
		/// <param name="fromIndex">   the index from which to start the search. </param>
		/// <returns>  the index within this string of the first occurrence of the
		///          specified substring, starting at the specified index. </returns>
		public virtual int IndexOf(String str, int fromIndex)
		{
			return String.IndexOf(Value_Renamed, 0, Count, str, fromIndex);
		}

		/// <summary>
		/// Returns the index within this string of the rightmost occurrence
		/// of the specified substring.  The rightmost empty string "" is
		/// considered to occur at the index value {@code this.length()}.
		/// The returned index is the largest value <i>k</i> such that
		/// <pre>{@code
		/// this.toString().startsWith(str, k)
		/// }</pre>
		/// is true.
		/// </summary>
		/// <param name="str">   the substring to search for. </param>
		/// <returns>  if the string argument occurs one or more times as a substring
		///          within this object, then the index of the first character of
		///          the last such substring is returned. If it does not occur as
		///          a substring, {@code -1} is returned. </returns>
		public virtual int LastIndexOf(String str)
		{
			return LastIndexOf(str, Count);
		}

		/// <summary>
		/// Returns the index within this string of the last occurrence of the
		/// specified substring. The integer returned is the largest value <i>k</i>
		/// such that:
		/// <pre>{@code
		///     k <= Math.min(fromIndex, this.length()) &&
		///                   this.toString().startsWith(str, k)
		/// }</pre>
		/// If no such value of <i>k</i> exists, then -1 is returned.
		/// </summary>
		/// <param name="str">         the substring to search for. </param>
		/// <param name="fromIndex">   the index to start the search from. </param>
		/// <returns>  the index within this sequence of the last occurrence of the
		///          specified substring. </returns>
		public virtual int LastIndexOf(String str, int fromIndex)
		{
			return String.LastIndexOf(Value_Renamed, 0, Count, str, fromIndex);
		}

		/// <summary>
		/// Causes this character sequence to be replaced by the reverse of
		/// the sequence. If there are any surrogate pairs included in the
		/// sequence, these are treated as single characters for the
		/// reverse operation. Thus, the order of the high-low surrogates
		/// is never reversed.
		/// 
		/// Let <i>n</i> be the character length of this character sequence
		/// (not the length in {@code char} values) just prior to
		/// execution of the {@code reverse} method. Then the
		/// character at index <i>k</i> in the new character sequence is
		/// equal to the character at index <i>n-k-1</i> in the old
		/// character sequence.
		/// 
		/// <para>Note that the reverse operation may result in producing
		/// surrogate pairs that were unpaired low-surrogates and
		/// high-surrogates before the operation. For example, reversing
		/// "\u005CuDC00\u005CuD800" produces "\u005CuD800\u005CuDC00" which is
		/// a valid surrogate pair.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a reference to this object. </returns>
		public virtual AbstractStringBuilder Reverse()
		{
			bool hasSurrogates = false;
			int n = Count - 1;
			for (int j = (n - 1) >> 1; j >= 0; j--)
			{
				int k = n - j;
				char cj = Value_Renamed[j];
				char ck = Value_Renamed[k];
				Value_Renamed[j] = ck;
				Value_Renamed[k] = cj;
				if (Character.IsSurrogate(cj) || Character.IsSurrogate(ck))
				{
					hasSurrogates = true;
				}
			}
			if (hasSurrogates)
			{
				ReverseAllValidSurrogatePairs();
			}
			return this;
		}

		/// <summary>
		/// Outlined helper method for reverse() </summary>
		private void ReverseAllValidSurrogatePairs()
		{
			for (int i = 0; i < Count - 1; i++)
			{
				char c2 = Value_Renamed[i];
				if (char.IsLowSurrogate(c2))
				{
					char c1 = Value_Renamed[i + 1];
					if (char.IsHighSurrogate(c1))
					{
						Value_Renamed[i++] = c1;
						Value_Renamed[i] = c2;
					}
				}
			}
		}

		/// <summary>
		/// Returns a string representing the data in this sequence.
		/// A new {@code String} object is allocated and initialized to
		/// contain the character sequence currently represented by this
		/// object. This {@code String} is then returned. Subsequent
		/// changes to this sequence do not affect the contents of the
		/// {@code String}.
		/// </summary>
		/// <returns>  a string representation of this sequence of characters. </returns>
		public override abstract String ToString();

		/// <summary>
		/// Needed by {@code String} for the contentEquals method.
		/// </summary>
		internal char[] Value
		{
			get
			{
				return Value_Renamed;
			}
		}

	}

}