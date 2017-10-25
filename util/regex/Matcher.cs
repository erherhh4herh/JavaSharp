using System;

/*
 * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.regex
{

	/// <summary>
	/// An engine that performs match operations on a {@link java.lang.CharSequence
	/// character sequence} by interpreting a <seealso cref="Pattern"/>.
	/// 
	/// <para> A matcher is created from a pattern by invoking the pattern's {@link
	/// Pattern#matcher matcher} method.  Once created, a matcher can be used to
	/// perform three different kinds of match operations:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> The <seealso cref="#matches matches"/> method attempts to match the entire
	///   input sequence against the pattern.  </para></li>
	/// 
	///   <li><para> The <seealso cref="#lookingAt lookingAt"/> method attempts to match the
	///   input sequence, starting at the beginning, against the pattern.  </para></li>
	/// 
	///   <li><para> The <seealso cref="#find find"/> method scans the input sequence looking for
	///   the next subsequence that matches the pattern.  </para></li>
	/// 
	/// </ul>
	/// 
	/// <para> Each of these methods returns a boolean indicating success or failure.
	/// More information about a successful match can be obtained by querying the
	/// state of the matcher.
	/// 
	/// </para>
	/// <para> A matcher finds matches in a subset of its input called the
	/// <i>region</i>. By default, the region contains all of the matcher's input.
	/// The region can be modified via the<seealso cref="#region region"/> method and queried
	/// via the <seealso cref="#regionStart regionStart"/> and <seealso cref="#regionEnd regionEnd"/>
	/// methods. The way that the region boundaries interact with some pattern
	/// constructs can be changed. See {@link #useAnchoringBounds
	/// useAnchoringBounds} and <seealso cref="#useTransparentBounds useTransparentBounds"/>
	/// for more details.
	/// 
	/// </para>
	/// <para> This class also defines methods for replacing matched subsequences with
	/// new strings whose contents can, if desired, be computed from the match
	/// result.  The <seealso cref="#appendReplacement appendReplacement"/> and {@link
	/// #appendTail appendTail} methods can be used in tandem in order to collect
	/// the result into an existing string buffer, or the more convenient {@link
	/// #replaceAll replaceAll} method can be used to create a string in which every
	/// matching subsequence in the input sequence is replaced.
	/// 
	/// </para>
	/// <para> The explicit state of a matcher includes the start and end indices of
	/// the most recent successful match.  It also includes the start and end
	/// indices of the input subsequence captured by each <a
	/// href="Pattern.html#cg">capturing group</a> in the pattern as well as a total
	/// count of such subsequences.  As a convenience, methods are also provided for
	/// returning these captured subsequences in string form.
	/// 
	/// </para>
	/// <para> The explicit state of a matcher is initially undefined; attempting to
	/// query any part of it before a successful match will cause an {@link
	/// IllegalStateException} to be thrown.  The explicit state of a matcher is
	/// recomputed by every match operation.
	/// 
	/// </para>
	/// <para> The implicit state of a matcher includes the input character sequence as
	/// well as the <i>append position</i>, which is initially zero and is updated
	/// by the <seealso cref="#appendReplacement appendReplacement"/> method.
	/// 
	/// </para>
	/// <para> A matcher may be reset explicitly by invoking its <seealso cref="#reset()"/>
	/// method or, if a new input sequence is desired, its {@link
	/// #reset(java.lang.CharSequence) reset(CharSequence)} method.  Resetting a
	/// matcher discards its explicit state information and sets the append position
	/// to zero.
	/// 
	/// </para>
	/// <para> Instances of this class are not safe for use by multiple concurrent
	/// threads. </para>
	/// 
	/// 
	/// @author      Mike McCloskey
	/// @author      Mark Reinhold
	/// @author      JSR-51 Expert Group
	/// @since       1.4
	/// @spec        JSR-51
	/// </summary>

	public sealed class Matcher : MatchResult
	{

		/// <summary>
		/// The Pattern object that created this Matcher.
		/// </summary>
		internal Pattern ParentPattern;

		/// <summary>
		/// The storage used by groups. They may contain invalid values if
		/// a group was skipped during the matching.
		/// </summary>
		internal int[] Groups;

		/// <summary>
		/// The range within the sequence that is to be matched. Anchors
		/// will match at these "hard" boundaries. Changing the region
		/// changes these values.
		/// </summary>
		internal int From, To;

		/// <summary>
		/// Lookbehind uses this value to ensure that the subexpression
		/// match ends at the point where the lookbehind was encountered.
		/// </summary>
		internal int LookbehindTo;

		/// <summary>
		/// The original string being matched.
		/// </summary>
		internal CharSequence Text;

		/// <summary>
		/// Matcher state used by the last node. NOANCHOR is used when a
		/// match does not have to consume all of the input. ENDANCHOR is
		/// the mode used for matching all the input.
		/// </summary>
		internal const int ENDANCHOR = 1;
		internal const int NOANCHOR = 0;
		internal int AcceptMode = NOANCHOR;

		/// <summary>
		/// The range of string that last matched the pattern. If the last
		/// match failed then first is -1; last initially holds 0 then it
		/// holds the index of the end of the last match (which is where the
		/// next search starts).
		/// </summary>
		internal int First = -1, Last = 0;

		/// <summary>
		/// The end index of what matched in the last match operation.
		/// </summary>
		internal int OldLast = -1;

		/// <summary>
		/// The index of the last position appended in a substitution.
		/// </summary>
		internal int LastAppendPosition = 0;

		/// <summary>
		/// Storage used by nodes to tell what repetition they are on in
		/// a pattern, and where groups begin. The nodes themselves are stateless,
		/// so they rely on this field to hold state during a match.
		/// </summary>
		internal int[] Locals;

		/// <summary>
		/// Boolean indicating whether or not more input could change
		/// the results of the last match.
		/// 
		/// If hitEnd is true, and a match was found, then more input
		/// might cause a different match to be found.
		/// If hitEnd is true and a match was not found, then more
		/// input could cause a match to be found.
		/// If hitEnd is false and a match was found, then more input
		/// will not change the match.
		/// If hitEnd is false and a match was not found, then more
		/// input will not cause a match to be found.
		/// </summary>
		internal bool HitEnd_Renamed;

		/// <summary>
		/// Boolean indicating whether or not more input could change
		/// a positive match into a negative one.
		/// 
		/// If requireEnd is true, and a match was found, then more
		/// input could cause the match to be lost.
		/// If requireEnd is false and a match was found, then more
		/// input might change the match but the match won't be lost.
		/// If a match was not found, then requireEnd has no meaning.
		/// </summary>
		internal bool RequireEnd_Renamed;

		/// <summary>
		/// If transparentBounds is true then the boundaries of this
		/// matcher's region are transparent to lookahead, lookbehind,
		/// and boundary matching constructs that try to see beyond them.
		/// </summary>
		internal bool TransparentBounds = false;

		/// <summary>
		/// If anchoringBounds is true then the boundaries of this
		/// matcher's region match anchors such as ^ and $.
		/// </summary>
		internal bool AnchoringBounds = true;

		/// <summary>
		/// No default constructor.
		/// </summary>
		internal Matcher()
		{
		}

		/// <summary>
		/// All matchers have the state used by Pattern during a match.
		/// </summary>
		internal Matcher(Pattern parent, CharSequence text)
		{
			this.ParentPattern = parent;
			this.Text = text;

			// Allocate state storage
			int parentGroupCount = System.Math.Max(parent.CapturingGroupCount, 10);
			Groups = new int[parentGroupCount * 2];
			Locals = new int[parent.LocalCount];

			// Put fields into initial states
			Reset();
		}

		/// <summary>
		/// Returns the pattern that is interpreted by this matcher.
		/// </summary>
		/// <returns>  The pattern for which this matcher was created </returns>
		public Pattern Pattern()
		{
			return ParentPattern;
		}

		/// <summary>
		/// Returns the match state of this matcher as a <seealso cref="MatchResult"/>.
		/// The result is unaffected by subsequent operations performed upon this
		/// matcher.
		/// </summary>
		/// <returns>  a <code>MatchResult</code> with the state of this matcher
		/// @since 1.5 </returns>
		public MatchResult ToMatchResult()
		{
			Matcher result = new Matcher(this.ParentPattern, Text.ToString());
			result.First = this.First;
			result.Last = this.Last;
			result.Groups = this.Groups.clone();
			return result;
		}

		/// <summary>
		/// Changes the <tt>Pattern</tt> that this <tt>Matcher</tt> uses to
		/// find matches with.
		///  
		/// <para> This method causes this matcher to lose information
		/// about the groups of the last match that occurred. The
		/// matcher's position in the input is maintained and its
		/// last append position is unaffected.</para>
		/// </summary>
		/// <param name="newPattern">
		///         The new pattern used by this matcher </param>
		/// <returns>  This matcher </returns>
		/// <exception cref="IllegalArgumentException">
		///          If newPattern is <tt>null</tt>
		/// @since 1.5 </exception>
		public Matcher UsePattern(Pattern newPattern)
		{
			if (newPattern == null)
			{
				throw new IllegalArgumentException("Pattern cannot be null");
			}
			ParentPattern = newPattern;

			// Reallocate state storage
			int parentGroupCount = System.Math.Max(newPattern.CapturingGroupCount, 10);
			Groups = new int[parentGroupCount * 2];
			Locals = new int[newPattern.LocalCount];
			for (int i = 0; i < Groups.Length; i++)
			{
				Groups[i] = -1;
			}
			for (int i = 0; i < Locals.Length; i++)
			{
				Locals[i] = -1;
			}
			return this;
		}

		/// <summary>
		/// Resets this matcher.
		/// 
		/// <para> Resetting a matcher discards all of its explicit state information
		/// and sets its append position to zero. The matcher's region is set to the
		/// default region, which is its entire character sequence. The anchoring
		/// and transparency of this matcher's region boundaries are unaffected.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  This matcher </returns>
		public Matcher Reset()
		{
			First = -1;
			Last = 0;
			OldLast = -1;
			for (int i = 0; i < Groups.Length; i++)
			{
				Groups[i] = -1;
			}
			for (int i = 0; i < Locals.Length; i++)
			{
				Locals[i] = -1;
			}
			LastAppendPosition = 0;
			From = 0;
			To = TextLength;
			return this;
		}

		/// <summary>
		/// Resets this matcher with a new input sequence.
		/// 
		/// <para> Resetting a matcher discards all of its explicit state information
		/// and sets its append position to zero.  The matcher's region is set to
		/// the default region, which is its entire character sequence.  The
		/// anchoring and transparency of this matcher's region boundaries are
		/// unaffected.
		/// 
		/// </para>
		/// </summary>
		/// <param name="input">
		///         The new input character sequence
		/// </param>
		/// <returns>  This matcher </returns>
		public Matcher Reset(CharSequence input)
		{
			Text = input;
			return Reset();
		}

		/// <summary>
		/// Returns the start index of the previous match.
		/// </summary>
		/// <returns>  The index of the first character matched
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed </exception>
		public int Start()
		{
			if (First < 0)
			{
				throw new IllegalStateException("No match available");
			}
			return First;
		}

		/// <summary>
		/// Returns the start index of the subsequence captured by the given group
		/// during the previous match operation.
		/// 
		/// <para> <a href="Pattern.html#cg">Capturing groups</a> are indexed from left
		/// to right, starting at one.  Group zero denotes the entire pattern, so
		/// the expression <i>m.</i><tt>start(0)</tt> is equivalent to
		/// <i>m.</i><tt>start()</tt>.  </para>
		/// </summary>
		/// <param name="group">
		///         The index of a capturing group in this matcher's pattern
		/// </param>
		/// <returns>  The index of the first character captured by the group,
		///          or <tt>-1</tt> if the match was successful but the group
		///          itself did not match anything
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If there is no capturing group in the pattern
		///          with the given index </exception>
		public int Start(int group)
		{
			if (First < 0)
			{
				throw new IllegalStateException("No match available");
			}
			if (group < 0 || group > GroupCount())
			{
				throw new IndexOutOfBoundsException("No group " + group);
			}
			return Groups[group * 2];
		}

		/// <summary>
		/// Returns the start index of the subsequence captured by the given
		/// <a href="Pattern.html#groupname">named-capturing group</a> during the
		/// previous match operation.
		/// </summary>
		/// <param name="name">
		///         The name of a named-capturing group in this matcher's pattern
		/// </param>
		/// <returns>  The index of the first character captured by the group,
		///          or {@code -1} if the match was successful but the group
		///          itself did not match anything
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If there is no capturing group in the pattern
		///          with the given name
		/// @since 1.8 </exception>
		public int Start(String name)
		{
			return Groups[GetMatchedGroupIndex(name) * 2];
		}

		/// <summary>
		/// Returns the offset after the last character matched.
		/// </summary>
		/// <returns>  The offset after the last character matched
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed </exception>
		public int End()
		{
			if (First < 0)
			{
				throw new IllegalStateException("No match available");
			}
			return Last;
		}

		/// <summary>
		/// Returns the offset after the last character of the subsequence
		/// captured by the given group during the previous match operation.
		/// 
		/// <para> <a href="Pattern.html#cg">Capturing groups</a> are indexed from left
		/// to right, starting at one.  Group zero denotes the entire pattern, so
		/// the expression <i>m.</i><tt>end(0)</tt> is equivalent to
		/// <i>m.</i><tt>end()</tt>.  </para>
		/// </summary>
		/// <param name="group">
		///         The index of a capturing group in this matcher's pattern
		/// </param>
		/// <returns>  The offset after the last character captured by the group,
		///          or <tt>-1</tt> if the match was successful
		///          but the group itself did not match anything
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If there is no capturing group in the pattern
		///          with the given index </exception>
		public int End(int group)
		{
			if (First < 0)
			{
				throw new IllegalStateException("No match available");
			}
			if (group < 0 || group > GroupCount())
			{
				throw new IndexOutOfBoundsException("No group " + group);
			}
			return Groups[group * 2 + 1];
		}

		/// <summary>
		/// Returns the offset after the last character of the subsequence
		/// captured by the given <a href="Pattern.html#groupname">named-capturing
		/// group</a> during the previous match operation.
		/// </summary>
		/// <param name="name">
		///         The name of a named-capturing group in this matcher's pattern
		/// </param>
		/// <returns>  The offset after the last character captured by the group,
		///          or {@code -1} if the match was successful
		///          but the group itself did not match anything
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If there is no capturing group in the pattern
		///          with the given name
		/// @since 1.8 </exception>
		public int End(String name)
		{
			return Groups[GetMatchedGroupIndex(name) * 2 + 1];
		}

		/// <summary>
		/// Returns the input subsequence matched by the previous match.
		/// 
		/// <para> For a matcher <i>m</i> with input sequence <i>s</i>,
		/// the expressions <i>m.</i><tt>group()</tt> and
		/// <i>s.</i><tt>substring(</tt><i>m.</i><tt>start(),</tt>&nbsp;<i>m.</i><tt>end())</tt>
		/// are equivalent.  </para>
		/// 
		/// <para> Note that some patterns, for example <tt>a*</tt>, match the empty
		/// string.  This method will return the empty string when the pattern
		/// successfully matches the empty string in the input.  </para>
		/// </summary>
		/// <returns> The (possibly empty) subsequence matched by the previous match,
		///         in string form
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed </exception>
		public String Group()
		{
			return Group(0);
		}

		/// <summary>
		/// Returns the input subsequence captured by the given group during the
		/// previous match operation.
		/// 
		/// <para> For a matcher <i>m</i>, input sequence <i>s</i>, and group index
		/// <i>g</i>, the expressions <i>m.</i><tt>group(</tt><i>g</i><tt>)</tt> and
		/// <i>s.</i><tt>substring(</tt><i>m.</i><tt>start(</tt><i>g</i><tt>),</tt>&nbsp;<i>m.</i><tt>end(</tt><i>g</i><tt>))</tt>
		/// are equivalent.  </para>
		/// 
		/// <para> <a href="Pattern.html#cg">Capturing groups</a> are indexed from left
		/// to right, starting at one.  Group zero denotes the entire pattern, so
		/// the expression <tt>m.group(0)</tt> is equivalent to <tt>m.group()</tt>.
		/// </para>
		/// 
		/// <para> If the match was successful but the group specified failed to match
		/// any part of the input sequence, then <tt>null</tt> is returned. Note
		/// that some groups, for example <tt>(a*)</tt>, match the empty string.
		/// This method will return the empty string when such a group successfully
		/// matches the empty string in the input.  </para>
		/// </summary>
		/// <param name="group">
		///         The index of a capturing group in this matcher's pattern
		/// </param>
		/// <returns>  The (possibly empty) subsequence captured by the group
		///          during the previous match, or <tt>null</tt> if the group
		///          failed to match part of the input
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If there is no capturing group in the pattern
		///          with the given index </exception>
		public String Group(int group)
		{
			if (First < 0)
			{
				throw new IllegalStateException("No match found");
			}
			if (group < 0 || group > GroupCount())
			{
				throw new IndexOutOfBoundsException("No group " + group);
			}
			if ((Groups[group * 2] == -1) || (Groups[group * 2 + 1] == -1))
			{
				return null;
			}
			return GetSubSequence(Groups[group * 2], Groups[group * 2 + 1]).ToString();
		}

		/// <summary>
		/// Returns the input subsequence captured by the given
		/// <a href="Pattern.html#groupname">named-capturing group</a> during the previous
		/// match operation.
		/// 
		/// <para> If the match was successful but the group specified failed to match
		/// any part of the input sequence, then <tt>null</tt> is returned. Note
		/// that some groups, for example <tt>(a*)</tt>, match the empty string.
		/// This method will return the empty string when such a group successfully
		/// matches the empty string in the input.  </para>
		/// </summary>
		/// <param name="name">
		///         The name of a named-capturing group in this matcher's pattern
		/// </param>
		/// <returns>  The (possibly empty) subsequence captured by the named group
		///          during the previous match, or <tt>null</tt> if the group
		///          failed to match part of the input
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If there is no capturing group in the pattern
		///          with the given name
		/// @since 1.7 </exception>
		public String Group(String name)
		{
			int group = GetMatchedGroupIndex(name);
			if ((Groups[group * 2] == -1) || (Groups[group * 2 + 1] == -1))
			{
				return null;
			}
			return GetSubSequence(Groups[group * 2], Groups[group * 2 + 1]).ToString();
		}

		/// <summary>
		/// Returns the number of capturing groups in this matcher's pattern.
		/// 
		/// <para> Group zero denotes the entire pattern by convention. It is not
		/// included in this count.
		/// 
		/// </para>
		/// <para> Any non-negative integer smaller than or equal to the value
		/// returned by this method is guaranteed to be a valid group index for
		/// this matcher.  </para>
		/// </summary>
		/// <returns> The number of capturing groups in this matcher's pattern </returns>
		public int GroupCount()
		{
			return ParentPattern.CapturingGroupCount - 1;
		}

		/// <summary>
		/// Attempts to match the entire region against the pattern.
		/// 
		/// <para> If the match succeeds then more information can be obtained via the
		/// <tt>start</tt>, <tt>end</tt>, and <tt>group</tt> methods.  </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, the entire region sequence
		///          matches this matcher's pattern </returns>
		public bool Matches()
		{
			return Match(From, ENDANCHOR);
		}

		/// <summary>
		/// Attempts to find the next subsequence of the input sequence that matches
		/// the pattern.
		/// 
		/// <para> This method starts at the beginning of this matcher's region, or, if
		/// a previous invocation of the method was successful and the matcher has
		/// not since been reset, at the first character not matched by the previous
		/// match.
		/// 
		/// </para>
		/// <para> If the match succeeds then more information can be obtained via the
		/// <tt>start</tt>, <tt>end</tt>, and <tt>group</tt> methods.  </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, a subsequence of the input
		///          sequence matches this matcher's pattern </returns>
		public bool Find()
		{
			int nextSearchIndex = Last;
			if (nextSearchIndex == First)
			{
				nextSearchIndex++;
			}

			// If next search starts before region, start it at region
			if (nextSearchIndex < From)
			{
				nextSearchIndex = From;
			}

			// If next search starts beyond region then it fails
			if (nextSearchIndex > To)
			{
				for (int i = 0; i < Groups.Length; i++)
				{
					Groups[i] = -1;
				}
				return false;
			}
			return Search(nextSearchIndex);
		}

		/// <summary>
		/// Resets this matcher and then attempts to find the next subsequence of
		/// the input sequence that matches the pattern, starting at the specified
		/// index.
		/// 
		/// <para> If the match succeeds then more information can be obtained via the
		/// <tt>start</tt>, <tt>end</tt>, and <tt>group</tt> methods, and subsequent
		/// invocations of the <seealso cref="#find()"/> method will start at the first
		/// character not matched by this match.  </para>
		/// </summary>
		/// <param name="start"> the index to start searching for a match </param>
		/// <exception cref="IndexOutOfBoundsException">
		///          If start is less than zero or if start is greater than the
		///          length of the input sequence.
		/// </exception>
		/// <returns>  <tt>true</tt> if, and only if, a subsequence of the input
		///          sequence starting at the given index matches this matcher's
		///          pattern </returns>
		public bool Find(int start)
		{
			int limit = TextLength;
			if ((start < 0) || (start > limit))
			{
				throw new IndexOutOfBoundsException("Illegal start index");
			}
			Reset();
			return Search(start);
		}

		/// <summary>
		/// Attempts to match the input sequence, starting at the beginning of the
		/// region, against the pattern.
		/// 
		/// <para> Like the <seealso cref="#matches matches"/> method, this method always starts
		/// at the beginning of the region; unlike that method, it does not
		/// require that the entire region be matched.
		/// 
		/// </para>
		/// <para> If the match succeeds then more information can be obtained via the
		/// <tt>start</tt>, <tt>end</tt>, and <tt>group</tt> methods.  </para>
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, a prefix of the input
		///          sequence matches this matcher's pattern </returns>
		public bool LookingAt()
		{
			return Match(From, NOANCHOR);
		}

		/// <summary>
		/// Returns a literal replacement <code>String</code> for the specified
		/// <code>String</code>.
		/// 
		/// This method produces a <code>String</code> that will work
		/// as a literal replacement <code>s</code> in the
		/// <code>appendReplacement</code> method of the <seealso cref="Matcher"/> class.
		/// The <code>String</code> produced will match the sequence of characters
		/// in <code>s</code> treated as a literal sequence. Slashes ('\') and
		/// dollar signs ('$') will be given no special meaning.
		/// </summary>
		/// <param name="s"> The string to be literalized </param>
		/// <returns>  A literal string replacement
		/// @since 1.5 </returns>
		public static String QuoteReplacement(String s)
		{
			if ((s.IndexOf('\\') == -1) && (s.IndexOf('$') == -1))
			{
				return s;
			}
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < s.Length(); i++)
			{
				char c = s.CharAt(i);
				if (c == '\\' || c == '$')
				{
					sb.Append('\\');
				}
				sb.Append(c);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Implements a non-terminal append-and-replace step.
		/// 
		/// <para> This method performs the following actions: </para>
		/// 
		/// <ol>
		/// 
		///   <li><para> It reads characters from the input sequence, starting at the
		///   append position, and appends them to the given string buffer.  It
		///   stops after reading the last character preceding the previous match,
		///   that is, the character at index {@link
		///   #start()}&nbsp;<tt>-</tt>&nbsp;<tt>1</tt>.  </para></li>
		/// 
		///   <li><para> It appends the given replacement string to the string buffer.
		///   </para></li>
		/// 
		///   <li><para> It sets the append position of this matcher to the index of
		///   the last character matched, plus one, that is, to <seealso cref="#end()"/>.
		///   </para></li>
		/// 
		/// </ol>
		/// 
		/// <para> The replacement string may contain references to subsequences
		/// captured during the previous match: Each occurrence of
		/// <tt>${</tt><i>name</i><tt>}</tt> or <tt>$</tt><i>g</i>
		/// will be replaced by the result of evaluating the corresponding
		/// <seealso cref="#group(String) group(name)"/> or <seealso cref="#group(int) group(g)"/>
		/// respectively. For  <tt>$</tt><i>g</i>,
		/// the first number after the <tt>$</tt> is always treated as part of
		/// the group reference. Subsequent numbers are incorporated into g if
		/// they would form a legal group reference. Only the numerals '0'
		/// through '9' are considered as potential components of the group
		/// reference. If the second group matched the string <tt>"foo"</tt>, for
		/// example, then passing the replacement string <tt>"$2bar"</tt> would
		/// cause <tt>"foobar"</tt> to be appended to the string buffer. A dollar
		/// sign (<tt>$</tt>) may be included as a literal in the replacement
		/// string by preceding it with a backslash (<tt>\$</tt>).
		/// 
		/// </para>
		/// <para> Note that backslashes (<tt>\</tt>) and dollar signs (<tt>$</tt>) in
		/// the replacement string may cause the results to be different than if it
		/// were being treated as a literal replacement string. Dollar signs may be
		/// treated as references to captured subsequences as described above, and
		/// backslashes are used to escape literal characters in the replacement
		/// string.
		/// 
		/// </para>
		/// <para> This method is intended to be used in a loop together with the
		/// <seealso cref="#appendTail appendTail"/> and <seealso cref="#find find"/> methods.  The
		/// following code, for example, writes <tt>one dog two dogs in the
		/// yard</tt> to the standard-output stream: </para>
		/// 
		/// <blockquote><pre>
		/// Pattern p = Pattern.compile("cat");
		/// Matcher m = p.matcher("one cat two cats in the yard");
		/// StringBuffer sb = new StringBuffer();
		/// while (m.find()) {
		///     m.appendReplacement(sb, "dog");
		/// }
		/// m.appendTail(sb);
		/// System.out.println(sb.toString());</pre></blockquote>
		/// </summary>
		/// <param name="sb">
		///         The target string buffer
		/// </param>
		/// <param name="replacement">
		///         The replacement string
		/// </param>
		/// <returns>  This matcher
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed
		/// </exception>
		/// <exception cref="IllegalArgumentException">
		///          If the replacement string refers to a named-capturing
		///          group that does not exist in the pattern
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the replacement string refers to a capturing group
		///          that does not exist in the pattern </exception>
		public Matcher AppendReplacement(StringBuffer sb, String replacement)
		{

			// If no match, return error
			if (First < 0)
			{
				throw new IllegalStateException("No match available");
			}

			// Process substitution string to replace group references with groups
			int cursor = 0;
			StringBuilder result = new StringBuilder();

			while (cursor < replacement.Length())
			{
				char nextChar = replacement.CharAt(cursor);
				if (nextChar == '\\')
				{
					cursor++;
					if (cursor == replacement.Length())
					{
						throw new IllegalArgumentException("character to be escaped is missing");
					}
					nextChar = replacement.CharAt(cursor);
					result.Append(nextChar);
					cursor++;
				}
				else if (nextChar == '$')
				{
					// Skip past $
					cursor++;
					// Throw IAE if this "$" is the last character in replacement
					if (cursor == replacement.Length())
					{
					   throw new IllegalArgumentException("Illegal group reference: group index is missing");
					}
					nextChar = replacement.CharAt(cursor);
					int refNum = -1;
					if (nextChar == '{')
					{
						cursor++;
						StringBuilder gsb = new StringBuilder();
						while (cursor < replacement.Length())
						{
							nextChar = replacement.CharAt(cursor);
							if (ASCII.IsLower(nextChar) || ASCII.IsUpper(nextChar) || ASCII.IsDigit(nextChar))
							{
								gsb.Append(nextChar);
								cursor++;
							}
							else
							{
								break;
							}
						}
						if (gsb.Length() == 0)
						{
							throw new IllegalArgumentException("named capturing group has 0 length name");
						}
						if (nextChar != '}')
						{
							throw new IllegalArgumentException("named capturing group is missing trailing '}'");
						}
						String gname = gsb.ToString();
						if (ASCII.IsDigit(gname.CharAt(0)))
						{
							throw new IllegalArgumentException("capturing group name {" + gname + "} starts with digit character");
						}
						if (!ParentPattern.NamedGroups().ContainsKey(gname))
						{
							throw new IllegalArgumentException("No group with name {" + gname + "}");
						}
						refNum = ParentPattern.NamedGroups()[gname];
						cursor++;
					}
					else
					{
						// The first number is always a group
						refNum = (int)nextChar - '0';
						if ((refNum < 0) || (refNum > 9))
						{
							throw new IllegalArgumentException("Illegal group reference");
						}
						cursor++;
						// Capture the largest legal group string
						bool done = false;
						while (!done)
						{
							if (cursor >= replacement.Length())
							{
								break;
							}
							int nextDigit = replacement.CharAt(cursor) - '0';
							if ((nextDigit < 0) || (nextDigit > 9)) // not a number
							{
								break;
							}
							int newRefNum = (refNum * 10) + nextDigit;
							if (GroupCount() < newRefNum)
							{
								done = true;
							}
							else
							{
								refNum = newRefNum;
								cursor++;
							}
						}
					}
					// Append group
					if (Start(refNum) != -1 && End(refNum) != -1)
					{
						result.Append(Text, Start(refNum), End(refNum));
					}
				}
				else
				{
					result.Append(nextChar);
					cursor++;
				}
			}
			// Append the intervening text
			sb.Append(Text, LastAppendPosition, First);
			// Append the match substitution
			sb.Append(result);

			LastAppendPosition = Last;
			return this;
		}

		/// <summary>
		/// Implements a terminal append-and-replace step.
		/// 
		/// <para> This method reads characters from the input sequence, starting at
		/// the append position, and appends them to the given string buffer.  It is
		/// intended to be invoked after one or more invocations of the {@link
		/// #appendReplacement appendReplacement} method in order to copy the
		/// remainder of the input sequence.  </para>
		/// </summary>
		/// <param name="sb">
		///         The target string buffer
		/// </param>
		/// <returns>  The target string buffer </returns>
		public StringBuffer AppendTail(StringBuffer sb)
		{
			sb.Append(Text, LastAppendPosition, TextLength);
			return sb;
		}

		/// <summary>
		/// Replaces every subsequence of the input sequence that matches the
		/// pattern with the given replacement string.
		/// 
		/// <para> This method first resets this matcher.  It then scans the input
		/// sequence looking for matches of the pattern.  Characters that are not
		/// part of any match are appended directly to the result string; each match
		/// is replaced in the result by the replacement string.  The replacement
		/// string may contain references to captured subsequences as in the {@link
		/// #appendReplacement appendReplacement} method.
		/// 
		/// </para>
		/// <para> Note that backslashes (<tt>\</tt>) and dollar signs (<tt>$</tt>) in
		/// the replacement string may cause the results to be different than if it
		/// were being treated as a literal replacement string. Dollar signs may be
		/// treated as references to captured subsequences as described above, and
		/// backslashes are used to escape literal characters in the replacement
		/// string.
		/// 
		/// </para>
		/// <para> Given the regular expression <tt>a*b</tt>, the input
		/// <tt>"aabfooaabfooabfoob"</tt>, and the replacement string
		/// <tt>"-"</tt>, an invocation of this method on a matcher for that
		/// expression would yield the string <tt>"-foo-foo-foo-"</tt>.
		/// 
		/// </para>
		/// <para> Invoking this method changes this matcher's state.  If the matcher
		/// is to be used in further matching operations then it should first be
		/// reset.  </para>
		/// </summary>
		/// <param name="replacement">
		///         The replacement string
		/// </param>
		/// <returns>  The string constructed by replacing each matching subsequence
		///          by the replacement string, substituting captured subsequences
		///          as needed </returns>
		public String ReplaceAll(String replacement)
		{
			Reset();
			bool result = Find();
			if (result)
			{
				StringBuffer sb = new StringBuffer();
				do
				{
					AppendReplacement(sb, replacement);
					result = Find();
				} while (result);
				AppendTail(sb);
				return sb.ToString();
			}
			return Text.ToString();
		}

		/// <summary>
		/// Replaces the first subsequence of the input sequence that matches the
		/// pattern with the given replacement string.
		/// 
		/// <para> This method first resets this matcher.  It then scans the input
		/// sequence looking for a match of the pattern.  Characters that are not
		/// part of the match are appended directly to the result string; the match
		/// is replaced in the result by the replacement string.  The replacement
		/// string may contain references to captured subsequences as in the {@link
		/// #appendReplacement appendReplacement} method.
		/// 
		/// </para>
		/// <para>Note that backslashes (<tt>\</tt>) and dollar signs (<tt>$</tt>) in
		/// the replacement string may cause the results to be different than if it
		/// were being treated as a literal replacement string. Dollar signs may be
		/// treated as references to captured subsequences as described above, and
		/// backslashes are used to escape literal characters in the replacement
		/// string.
		/// 
		/// </para>
		/// <para> Given the regular expression <tt>dog</tt>, the input
		/// <tt>"zzzdogzzzdogzzz"</tt>, and the replacement string
		/// <tt>"cat"</tt>, an invocation of this method on a matcher for that
		/// expression would yield the string <tt>"zzzcatzzzdogzzz"</tt>.  </para>
		/// 
		/// <para> Invoking this method changes this matcher's state.  If the matcher
		/// is to be used in further matching operations then it should first be
		/// reset.  </para>
		/// </summary>
		/// <param name="replacement">
		///         The replacement string </param>
		/// <returns>  The string constructed by replacing the first matching
		///          subsequence by the replacement string, substituting captured
		///          subsequences as needed </returns>
		public String ReplaceFirst(String replacement)
		{
			if (replacement == null)
			{
				throw new NullPointerException("replacement");
			}
			Reset();
			if (!Find())
			{
				return Text.ToString();
			}
			StringBuffer sb = new StringBuffer();
			AppendReplacement(sb, replacement);
			AppendTail(sb);
			return sb.ToString();
		}

		/// <summary>
		/// Sets the limits of this matcher's region. The region is the part of the
		/// input sequence that will be searched to find a match. Invoking this
		/// method resets the matcher, and then sets the region to start at the
		/// index specified by the <code>start</code> parameter and end at the
		/// index specified by the <code>end</code> parameter.
		/// 
		/// <para>Depending on the transparency and anchoring being used (see
		/// <seealso cref="#useTransparentBounds useTransparentBounds"/> and
		/// <seealso cref="#useAnchoringBounds useAnchoringBounds"/>), certain constructs such
		/// as anchors may behave differently at or around the boundaries of the
		/// region.
		/// 
		/// </para>
		/// </summary>
		/// <param name="start">
		///         The index to start searching at (inclusive) </param>
		/// <param name="end">
		///         The index to end searching at (exclusive) </param>
		/// <exception cref="IndexOutOfBoundsException">
		///          If start or end is less than zero, if
		///          start is greater than the length of the input sequence, if
		///          end is greater than the length of the input sequence, or if
		///          start is greater than end. </exception>
		/// <returns>  this matcher
		/// @since 1.5 </returns>
		public Matcher Region(int start, int end)
		{
			if ((start < 0) || (start > TextLength))
			{
				throw new IndexOutOfBoundsException("start");
			}
			if ((end < 0) || (end > TextLength))
			{
				throw new IndexOutOfBoundsException("end");
			}
			if (start > end)
			{
				throw new IndexOutOfBoundsException("start > end");
			}
			Reset();
			From = start;
			To = end;
			return this;
		}

		/// <summary>
		/// Reports the start index of this matcher's region. The
		/// searches this matcher conducts are limited to finding matches
		/// within <seealso cref="#regionStart regionStart"/> (inclusive) and
		/// <seealso cref="#regionEnd regionEnd"/> (exclusive).
		/// </summary>
		/// <returns>  The starting point of this matcher's region
		/// @since 1.5 </returns>
		public int RegionStart()
		{
			return From;
		}

		/// <summary>
		/// Reports the end index (exclusive) of this matcher's region.
		/// The searches this matcher conducts are limited to finding matches
		/// within <seealso cref="#regionStart regionStart"/> (inclusive) and
		/// <seealso cref="#regionEnd regionEnd"/> (exclusive).
		/// </summary>
		/// <returns>  the ending point of this matcher's region
		/// @since 1.5 </returns>
		public int RegionEnd()
		{
			return To;
		}

		/// <summary>
		/// Queries the transparency of region bounds for this matcher.
		/// 
		/// <para> This method returns <tt>true</tt> if this matcher uses
		/// <i>transparent</i> bounds, <tt>false</tt> if it uses <i>opaque</i>
		/// bounds.
		/// 
		/// </para>
		/// <para> See <seealso cref="#useTransparentBounds useTransparentBounds"/> for a
		/// description of transparent and opaque bounds.
		/// 
		/// </para>
		/// <para> By default, a matcher uses opaque region boundaries.
		/// 
		/// </para>
		/// </summary>
		/// <returns> <tt>true</tt> iff this matcher is using transparent bounds,
		///         <tt>false</tt> otherwise. </returns>
		/// <seealso cref= java.util.regex.Matcher#useTransparentBounds(boolean)
		/// @since 1.5 </seealso>
		public bool HasTransparentBounds()
		{
			return TransparentBounds;
		}

		/// <summary>
		/// Sets the transparency of region bounds for this matcher.
		/// 
		/// <para> Invoking this method with an argument of <tt>true</tt> will set this
		/// matcher to use <i>transparent</i> bounds. If the boolean
		/// argument is <tt>false</tt>, then <i>opaque</i> bounds will be used.
		/// 
		/// </para>
		/// <para> Using transparent bounds, the boundaries of this
		/// matcher's region are transparent to lookahead, lookbehind,
		/// and boundary matching constructs. Those constructs can see beyond the
		/// boundaries of the region to see if a match is appropriate.
		/// 
		/// </para>
		/// <para> Using opaque bounds, the boundaries of this matcher's
		/// region are opaque to lookahead, lookbehind, and boundary matching
		/// constructs that may try to see beyond them. Those constructs cannot
		/// look past the boundaries so they will fail to match anything outside
		/// of the region.
		/// 
		/// </para>
		/// <para> By default, a matcher uses opaque bounds.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b"> a boolean indicating whether to use opaque or transparent
		///         regions </param>
		/// <returns> this matcher </returns>
		/// <seealso cref= java.util.regex.Matcher#hasTransparentBounds
		/// @since 1.5 </seealso>
		public Matcher UseTransparentBounds(bool b)
		{
			TransparentBounds = b;
			return this;
		}

		/// <summary>
		/// Queries the anchoring of region bounds for this matcher.
		/// 
		/// <para> This method returns <tt>true</tt> if this matcher uses
		/// <i>anchoring</i> bounds, <tt>false</tt> otherwise.
		/// 
		/// </para>
		/// <para> See <seealso cref="#useAnchoringBounds useAnchoringBounds"/> for a
		/// description of anchoring bounds.
		/// 
		/// </para>
		/// <para> By default, a matcher uses anchoring region boundaries.
		/// 
		/// </para>
		/// </summary>
		/// <returns> <tt>true</tt> iff this matcher is using anchoring bounds,
		///         <tt>false</tt> otherwise. </returns>
		/// <seealso cref= java.util.regex.Matcher#useAnchoringBounds(boolean)
		/// @since 1.5 </seealso>
		public bool HasAnchoringBounds()
		{
			return AnchoringBounds;
		}

		/// <summary>
		/// Sets the anchoring of region bounds for this matcher.
		/// 
		/// <para> Invoking this method with an argument of <tt>true</tt> will set this
		/// matcher to use <i>anchoring</i> bounds. If the boolean
		/// argument is <tt>false</tt>, then <i>non-anchoring</i> bounds will be
		/// used.
		/// 
		/// </para>
		/// <para> Using anchoring bounds, the boundaries of this
		/// matcher's region match anchors such as ^ and $.
		/// 
		/// </para>
		/// <para> Without anchoring bounds, the boundaries of this
		/// matcher's region will not match anchors such as ^ and $.
		/// 
		/// </para>
		/// <para> By default, a matcher uses anchoring region boundaries.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b"> a boolean indicating whether or not to use anchoring bounds. </param>
		/// <returns> this matcher </returns>
		/// <seealso cref= java.util.regex.Matcher#hasAnchoringBounds
		/// @since 1.5 </seealso>
		public Matcher UseAnchoringBounds(bool b)
		{
			AnchoringBounds = b;
			return this;
		}

		/// <summary>
		/// <para>Returns the string representation of this matcher. The
		/// string representation of a <code>Matcher</code> contains information
		/// that may be useful for debugging. The exact format is unspecified.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The string representation of this matcher
		/// @since 1.5 </returns>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("java.util.regex.Matcher");
			sb.Append("[pattern=" + Pattern());
			sb.Append(" region=");
			sb.Append(RegionStart() + "," + RegionEnd());
			sb.Append(" lastmatch=");
			if ((First >= 0) && (Group() != null))
			{
				sb.Append(Group());
			}
			sb.Append("]");
			return sb.ToString();
		}

		/// <summary>
		/// <para>Returns true if the end of input was hit by the search engine in
		/// the last match operation performed by this matcher.
		/// 
		/// </para>
		/// <para>When this method returns true, then it is possible that more input
		/// would have changed the result of the last search.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  true iff the end of input was hit in the last match; false
		///          otherwise
		/// @since 1.5 </returns>
		public bool HitEnd()
		{
			return HitEnd_Renamed;
		}

		/// <summary>
		/// <para>Returns true if more input could change a positive match into a
		/// negative one.
		/// 
		/// </para>
		/// <para>If this method returns true, and a match was found, then more
		/// input could cause the match to be lost. If this method returns false
		/// and a match was found, then more input might change the match but the
		/// match won't be lost. If a match was not found, then requireEnd has no
		/// meaning.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  true iff more input could change a positive match into a
		///          negative one.
		/// @since 1.5 </returns>
		public bool RequireEnd()
		{
			return RequireEnd_Renamed;
		}

		/// <summary>
		/// Initiates a search to find a Pattern within the given bounds.
		/// The groups are filled with default values and the match of the root
		/// of the state machine is called. The state machine will hold the state
		/// of the match as it proceeds in this matcher.
		/// 
		/// Matcher.from is not set here, because it is the "hard" boundary
		/// of the start of the search which anchors will set to. The from param
		/// is the "soft" boundary of the start of the search, meaning that the
		/// regex tries to match at that index but ^ won't match there. Subsequent
		/// calls to the search methods start at a new "soft" boundary which is
		/// the end of the previous match.
		/// </summary>
		internal bool Search(int from)
		{
			this.HitEnd_Renamed = false;
			this.RequireEnd_Renamed = false;
			from = from < 0 ? 0 : from;
			this.First = from;
			this.OldLast = OldLast < 0 ? from : OldLast;
			for (int i = 0; i < Groups.Length; i++)
			{
				Groups[i] = -1;
			}
			AcceptMode = NOANCHOR;
			bool result = ParentPattern.Root.Match(this, from, Text);
			if (!result)
			{
				this.First = -1;
			}
			this.OldLast = this.Last;
			return result;
		}

		/// <summary>
		/// Initiates a search for an anchored match to a Pattern within the given
		/// bounds. The groups are filled with default values and the match of the
		/// root of the state machine is called. The state machine will hold the
		/// state of the match as it proceeds in this matcher.
		/// </summary>
		internal bool Match(int from, int anchor)
		{
			this.HitEnd_Renamed = false;
			this.RequireEnd_Renamed = false;
			from = from < 0 ? 0 : from;
			this.First = from;
			this.OldLast = OldLast < 0 ? from : OldLast;
			for (int i = 0; i < Groups.Length; i++)
			{
				Groups[i] = -1;
			}
			AcceptMode = anchor;
			bool result = ParentPattern.MatchRoot.Match(this, from, Text);
			if (!result)
			{
				this.First = -1;
			}
			this.OldLast = this.Last;
			return result;
		}

		/// <summary>
		/// Returns the end index of the text.
		/// </summary>
		/// <returns> the index after the last character in the text </returns>
		internal int TextLength
		{
			get
			{
				return Text.Length();
			}
		}

		/// <summary>
		/// Generates a String from this Matcher's input in the specified range.
		/// </summary>
		/// <param name="beginIndex">   the beginning index, inclusive </param>
		/// <param name="endIndex">     the ending index, exclusive </param>
		/// <returns> A String generated from this Matcher's input </returns>
		internal CharSequence GetSubSequence(int beginIndex, int endIndex)
		{
			return Text.SubSequence(beginIndex, endIndex);
		}

		/// <summary>
		/// Returns this Matcher's input character at index i.
		/// </summary>
		/// <returns> A char from the specified index </returns>
		internal char CharAt(int i)
		{
			return Text.CharAt(i);
		}

		/// <summary>
		/// Returns the group index of the matched capturing group.
		/// </summary>
		/// <returns> the index of the named-capturing group </returns>
		internal int GetMatchedGroupIndex(String name)
		{
			Objects.RequireNonNull(name, "Group name");
			if (First < 0)
			{
				throw new IllegalStateException("No match found");
			}
			if (!ParentPattern.NamedGroups().ContainsKey(name))
			{
				throw new IllegalArgumentException("No group with name <" + name + ">");
			}
			return ParentPattern.NamedGroups()[name];
		}
	}

}