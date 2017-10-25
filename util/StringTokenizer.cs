using System.Collections.Generic;

/*
 * Copyright (c) 1994, 2004, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{

	/// <summary>
	/// The string tokenizer class allows an application to break a
	/// string into tokens. The tokenization method is much simpler than
	/// the one used by the <code>StreamTokenizer</code> class. The
	/// <code>StringTokenizer</code> methods do not distinguish among
	/// identifiers, numbers, and quoted strings, nor do they recognize
	/// and skip comments.
	/// <para>
	/// The set of delimiters (the characters that separate tokens) may
	/// be specified either at creation time or on a per-token basis.
	/// </para>
	/// <para>
	/// An instance of <code>StringTokenizer</code> behaves in one of two
	/// ways, depending on whether it was created with the
	/// <code>returnDelims</code> flag having the value <code>true</code>
	/// or <code>false</code>:
	/// <ul>
	/// <li>If the flag is <code>false</code>, delimiter characters serve to
	///     separate tokens. A token is a maximal sequence of consecutive
	///     characters that are not delimiters.
	/// <li>If the flag is <code>true</code>, delimiter characters are themselves
	///     considered to be tokens. A token is thus either one delimiter
	///     character, or a maximal sequence of consecutive characters that are
	///     not delimiters.
	/// </para>
	/// </ul><para>
	/// A <tt>StringTokenizer</tt> object internally maintains a current
	/// position within the string to be tokenized. Some operations advance this
	/// </para>
	/// current position past the characters processed.<para>
	/// A token is returned by taking a substring of the string that was used to
	/// create the <tt>StringTokenizer</tt> object.
	/// </para>
	/// <para>
	/// The following is one example of the use of the tokenizer. The code:
	/// <blockquote><pre>
	///     StringTokenizer st = new StringTokenizer("this is a test");
	///     while (st.hasMoreTokens()) {
	///         System.out.println(st.nextToken());
	///     }
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// prints the following output:
	/// <blockquote><pre>
	///     this
	///     is
	///     a
	///     test
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>
	/// <tt>StringTokenizer</tt> is a legacy class that is retained for
	/// compatibility reasons although its use is discouraged in new code. It is
	/// recommended that anyone seeking this functionality use the <tt>split</tt>
	/// method of <tt>String</tt> or the java.util.regex package instead.
	/// </para>
	/// <para>
	/// The following example illustrates how the <tt>String.split</tt>
	/// method can be used to break up a string into its basic tokens:
	/// <blockquote><pre>
	///     String[] result = "this is a test".split("\\s");
	///     for (int x=0; x&lt;result.length; x++)
	///         System.out.println(result[x]);
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// prints the following output:
	/// <blockquote><pre>
	///     this
	///     is
	///     a
	///     test
	/// </pre></blockquote>
	/// 
	/// @author  unascribed
	/// </para>
	/// </summary>
	/// <seealso cref=     java.io.StreamTokenizer
	/// @since   JDK1.0 </seealso>
	public class StringTokenizer : IEnumerator<Object>
	{
		private int CurrentPosition;
		private int NewPosition;
		private int MaxPosition;
		private String Str;
		private String Delimiters;
		private bool RetDelims;
		private bool DelimsChanged;

		/// <summary>
		/// maxDelimCodePoint stores the value of the delimiter character with the
		/// highest value. It is used to optimize the detection of delimiter
		/// characters.
		/// 
		/// It is unlikely to provide any optimization benefit in the
		/// hasSurrogates case because most string characters will be
		/// smaller than the limit, but we keep it so that the two code
		/// paths remain similar.
		/// </summary>
		private int MaxDelimCodePoint;

		/// <summary>
		/// If delimiters include any surrogates (including surrogate
		/// pairs), hasSurrogates is true and the tokenizer uses the
		/// different code path. This is because String.indexOf(int)
		/// doesn't handle unpaired surrogates as a single character.
		/// </summary>
		private bool HasSurrogates = false;

		/// <summary>
		/// When hasSurrogates is true, delimiters are converted to code
		/// points and isDelimiter(int) is used to determine if the given
		/// codepoint is a delimiter.
		/// </summary>
		private int[] DelimiterCodePoints;

		/// <summary>
		/// Set maxDelimCodePoint to the highest char in the delimiter set.
		/// </summary>
		private void SetMaxDelimCodePoint()
		{
			if (Delimiters == null)
			{
				MaxDelimCodePoint = 0;
				return;
			}

			int m = 0;
			int c;
			int count = 0;
			for (int i = 0; i < Delimiters.Length(); i += Character.CharCount(c))
			{
				c = Delimiters.CharAt(i);
				if (c >= Character.MIN_HIGH_SURROGATE && c <= Character.MAX_LOW_SURROGATE)
				{
					c = Delimiters.CodePointAt(i);
					HasSurrogates = true;
				}
				if (m < c)
				{
					m = c;
				}
				count++;
			}
			MaxDelimCodePoint = m;

			if (HasSurrogates)
			{
				DelimiterCodePoints = new int[count];
				for (int i = 0, j = 0; i < count; i++, j += Character.CharCount(c))
				{
					c = Delimiters.CodePointAt(j);
					DelimiterCodePoints[i] = c;
				}
			}
		}

		/// <summary>
		/// Constructs a string tokenizer for the specified string. All
		/// characters in the <code>delim</code> argument are the delimiters
		/// for separating tokens.
		/// <para>
		/// If the <code>returnDelims</code> flag is <code>true</code>, then
		/// the delimiter characters are also returned as tokens. Each
		/// delimiter is returned as a string of length one. If the flag is
		/// <code>false</code>, the delimiter characters are skipped and only
		/// serve as separators between tokens.
		/// </para>
		/// <para>
		/// Note that if <tt>delim</tt> is <tt>null</tt>, this constructor does
		/// not throw an exception. However, trying to invoke other methods on the
		/// resulting <tt>StringTokenizer</tt> may result in a
		/// <tt>NullPointerException</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">            a string to be parsed. </param>
		/// <param name="delim">          the delimiters. </param>
		/// <param name="returnDelims">   flag indicating whether to return the delimiters
		///                         as tokens. </param>
		/// <exception cref="NullPointerException"> if str is <CODE>null</CODE> </exception>
		public StringTokenizer(String str, String delim, bool returnDelims)
		{
			CurrentPosition = 0;
			NewPosition = -1;
			DelimsChanged = false;
			this.Str = str;
			MaxPosition = str.Length();
			Delimiters = delim;
			RetDelims = returnDelims;
			SetMaxDelimCodePoint();
		}

		/// <summary>
		/// Constructs a string tokenizer for the specified string. The
		/// characters in the <code>delim</code> argument are the delimiters
		/// for separating tokens. Delimiter characters themselves will not
		/// be treated as tokens.
		/// <para>
		/// Note that if <tt>delim</tt> is <tt>null</tt>, this constructor does
		/// not throw an exception. However, trying to invoke other methods on the
		/// resulting <tt>StringTokenizer</tt> may result in a
		/// <tt>NullPointerException</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">     a string to be parsed. </param>
		/// <param name="delim">   the delimiters. </param>
		/// <exception cref="NullPointerException"> if str is <CODE>null</CODE> </exception>
		public StringTokenizer(String str, String delim) : this(str, delim, false)
		{
		}

		/// <summary>
		/// Constructs a string tokenizer for the specified string. The
		/// tokenizer uses the default delimiter set, which is
		/// <code>"&nbsp;&#92;t&#92;n&#92;r&#92;f"</code>: the space character,
		/// the tab character, the newline character, the carriage-return character,
		/// and the form-feed character. Delimiter characters themselves will
		/// not be treated as tokens.
		/// </summary>
		/// <param name="str">   a string to be parsed. </param>
		/// <exception cref="NullPointerException"> if str is <CODE>null</CODE> </exception>
		public StringTokenizer(String str) : this(str, " \t\n\r\f", false)
		{
		}

		/// <summary>
		/// Skips delimiters starting from the specified position. If retDelims
		/// is false, returns the index of the first non-delimiter character at or
		/// after startPos. If retDelims is true, startPos is returned.
		/// </summary>
		private int SkipDelimiters(int startPos)
		{
			if (Delimiters == null)
			{
				throw new NullPointerException();
			}

			int position = startPos;
			while (!RetDelims && position < MaxPosition)
			{
				if (!HasSurrogates)
				{
					char c = Str.CharAt(position);
					if ((c > MaxDelimCodePoint) || (Delimiters.IndexOf(c) < 0))
					{
						break;
					}
					position++;
				}
				else
				{
					int c = Str.CodePointAt(position);
					if ((c > MaxDelimCodePoint) || !IsDelimiter(c))
					{
						break;
					}
					position += Character.CharCount(c);
				}
			}
			return position;
		}

		/// <summary>
		/// Skips ahead from startPos and returns the index of the next delimiter
		/// character encountered, or maxPosition if no such delimiter is found.
		/// </summary>
		private int ScanToken(int startPos)
		{
			int position = startPos;
			while (position < MaxPosition)
			{
				if (!HasSurrogates)
				{
					char c = Str.CharAt(position);
					if ((c <= MaxDelimCodePoint) && (Delimiters.IndexOf(c) >= 0))
					{
						break;
					}
					position++;
				}
				else
				{
					int c = Str.CodePointAt(position);
					if ((c <= MaxDelimCodePoint) && IsDelimiter(c))
					{
						break;
					}
					position += Character.CharCount(c);
				}
			}
			if (RetDelims && (startPos == position))
			{
				if (!HasSurrogates)
				{
					char c = Str.CharAt(position);
					if ((c <= MaxDelimCodePoint) && (Delimiters.IndexOf(c) >= 0))
					{
						position++;
					}
				}
				else
				{
					int c = Str.CodePointAt(position);
					if ((c <= MaxDelimCodePoint) && IsDelimiter(c))
					{
						position += Character.CharCount(c);
					}
				}
			}
			return position;
		}

		private bool IsDelimiter(int codePoint)
		{
			for (int i = 0; i < DelimiterCodePoints.Length; i++)
			{
				if (DelimiterCodePoints[i] == codePoint)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Tests if there are more tokens available from this tokenizer's string.
		/// If this method returns <tt>true</tt>, then a subsequent call to
		/// <tt>nextToken</tt> with no argument will successfully return a token.
		/// </summary>
		/// <returns>  <code>true</code> if and only if there is at least one token
		///          in the string after the current position; <code>false</code>
		///          otherwise. </returns>
		public virtual bool HasMoreTokens()
		{
			/*
			 * Temporarily store this position and use it in the following
			 * nextToken() method only if the delimiters haven't been changed in
			 * that nextToken() invocation.
			 */
			NewPosition = SkipDelimiters(CurrentPosition);
			return (NewPosition < MaxPosition);
		}

		/// <summary>
		/// Returns the next token from this string tokenizer.
		/// </summary>
		/// <returns>     the next token from this string tokenizer. </returns>
		/// <exception cref="NoSuchElementException">  if there are no more tokens in this
		///               tokenizer's string. </exception>
		public virtual String NextToken()
		{
			/*
			 * If next position already computed in hasMoreElements() and
			 * delimiters have changed between the computation and this invocation,
			 * then use the computed value.
			 */

			CurrentPosition = (NewPosition >= 0 && !DelimsChanged) ? NewPosition : SkipDelimiters(CurrentPosition);

			/* Reset these anyway */
			DelimsChanged = false;
			NewPosition = -1;

			if (CurrentPosition >= MaxPosition)
			{
				throw new NoSuchElementException();
			}
			int start = CurrentPosition;
			CurrentPosition = ScanToken(CurrentPosition);
			return Str.Substring(start, CurrentPosition - start);
		}

		/// <summary>
		/// Returns the next token in this string tokenizer's string. First,
		/// the set of characters considered to be delimiters by this
		/// <tt>StringTokenizer</tt> object is changed to be the characters in
		/// the string <tt>delim</tt>. Then the next token in the string
		/// after the current position is returned. The current position is
		/// advanced beyond the recognized token.  The new delimiter set
		/// remains the default after this call.
		/// </summary>
		/// <param name="delim">   the new delimiters. </param>
		/// <returns>     the next token, after switching to the new delimiter set. </returns>
		/// <exception cref="NoSuchElementException">  if there are no more tokens in this
		///               tokenizer's string. </exception>
		/// <exception cref="NullPointerException"> if delim is <CODE>null</CODE> </exception>
		public virtual String NextToken(String delim)
		{
			Delimiters = delim;

			/* delimiter string specified, so set the appropriate flag. */
			DelimsChanged = true;

			SetMaxDelimCodePoint();
			return NextToken();
		}

		/// <summary>
		/// Returns the same value as the <code>hasMoreTokens</code>
		/// method. It exists so that this class can implement the
		/// <code>Enumeration</code> interface.
		/// </summary>
		/// <returns>  <code>true</code> if there are more tokens;
		///          <code>false</code> otherwise. </returns>
		/// <seealso cref=     java.util.Enumeration </seealso>
		/// <seealso cref=     java.util.StringTokenizer#hasMoreTokens() </seealso>
		public virtual bool HasMoreElements()
		{
			return HasMoreTokens();
		}

		/// <summary>
		/// Returns the same value as the <code>nextToken</code> method,
		/// except that its declared return value is <code>Object</code> rather than
		/// <code>String</code>. It exists so that this class can implement the
		/// <code>Enumeration</code> interface.
		/// </summary>
		/// <returns>     the next token in the string. </returns>
		/// <exception cref="NoSuchElementException">  if there are no more tokens in this
		///               tokenizer's string. </exception>
		/// <seealso cref=        java.util.Enumeration </seealso>
		/// <seealso cref=        java.util.StringTokenizer#nextToken() </seealso>
		public virtual Object NextElement()
		{
			return NextToken();
		}

		/// <summary>
		/// Calculates the number of times that this tokenizer's
		/// <code>nextToken</code> method can be called before it generates an
		/// exception. The current position is not advanced.
		/// </summary>
		/// <returns>  the number of tokens remaining in the string using the current
		///          delimiter set. </returns>
		/// <seealso cref=     java.util.StringTokenizer#nextToken() </seealso>
		public virtual int CountTokens()
		{
			int count = 0;
			int currpos = CurrentPosition;
			while (currpos < MaxPosition)
			{
				currpos = SkipDelimiters(currpos);
				if (currpos >= MaxPosition)
				{
					break;
				}
				currpos = ScanToken(currpos);
				count++;
			}
			return count;
		}
	}

}