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

namespace java.util.regex
{

	/// <summary>
	/// The result of a match operation.
	/// 
	/// <para>This interface contains query methods used to determine the
	/// results of a match against a regular expression. The match boundaries,
	/// groups and group boundaries can be seen but not modified through
	/// a <code>MatchResult</code>.
	/// 
	/// @author  Michael McCloskey
	/// </para>
	/// </summary>
	/// <seealso cref= Matcher
	/// @since 1.5 </seealso>
	public interface MatchResult
	{

		/// <summary>
		/// Returns the start index of the match.
		/// </summary>
		/// <returns>  The index of the first character matched
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed </exception>
		int Start();

		/// <summary>
		/// Returns the start index of the subsequence captured by the given group
		/// during this match.
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
		int Start(int group);

		/// <summary>
		/// Returns the offset after the last character matched.
		/// </summary>
		/// <returns>  The offset after the last character matched
		/// </returns>
		/// <exception cref="IllegalStateException">
		///          If no match has yet been attempted,
		///          or if the previous match operation failed </exception>
		int End();

		/// <summary>
		/// Returns the offset after the last character of the subsequence
		/// captured by the given group during this match.
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
		int End(int group);

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
		String Group();

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
		String Group(int group);

		/// <summary>
		/// Returns the number of capturing groups in this match result's pattern.
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
		int GroupCount();

	}

}