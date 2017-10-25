/*
 * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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

	/// <summary>
	/// An object to which <tt>char</tt> sequences and values can be appended.  The
	/// <tt>Appendable</tt> interface must be implemented by any class whose
	/// instances are intended to receive formatted output from a {@link
	/// java.util.Formatter}.
	/// 
	/// <para> The characters to be appended should be valid Unicode characters as
	/// described in <a href="Character.html#unicode">Unicode Character
	/// Representation</a>.  Note that supplementary characters may be composed of
	/// multiple 16-bit <tt>char</tt> values.
	/// 
	/// </para>
	/// <para> Appendables are not necessarily safe for multithreaded access.  Thread
	/// safety is the responsibility of classes that extend and implement this
	/// interface.
	/// 
	/// </para>
	/// <para> Since this interface may be implemented by existing classes
	/// with different styles of error handling there is no guarantee that
	/// errors will be propagated to the invoker.
	/// 
	/// @since 1.5
	/// </para>
	/// </summary>
	public interface Appendable
	{

		/// <summary>
		/// Appends the specified character sequence to this <tt>Appendable</tt>.
		/// 
		/// <para> Depending on which class implements the character sequence
		/// <tt>csq</tt>, the entire sequence may not be appended.  For
		/// instance, if <tt>csq</tt> is a <seealso cref="java.nio.CharBuffer"/> then
		/// the subsequence to append is defined by the buffer's position and limit.
		/// 
		/// </para>
		/// </summary>
		/// <param name="csq">
		///         The character sequence to append.  If <tt>csq</tt> is
		///         <tt>null</tt>, then the four characters <tt>"null"</tt> are
		///         appended to this Appendable.
		/// </param>
		/// <returns>  A reference to this <tt>Appendable</tt>
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Appendable append(CharSequence csq) throws java.io.IOException;
		Appendable Append(CharSequence csq);

		/// <summary>
		/// Appends a subsequence of the specified character sequence to this
		/// <tt>Appendable</tt>.
		/// 
		/// <para> An invocation of this method of the form <tt>out.append(csq, start,
		/// end)</tt> when <tt>csq</tt> is not <tt>null</tt>, behaves in
		/// exactly the same way as the invocation
		/// 
		/// <pre>
		///     out.append(csq.subSequence(start, end)) </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="csq">
		///         The character sequence from which a subsequence will be
		///         appended.  If <tt>csq</tt> is <tt>null</tt>, then characters
		///         will be appended as if <tt>csq</tt> contained the four
		///         characters <tt>"null"</tt>.
		/// </param>
		/// <param name="start">
		///         The index of the first character in the subsequence
		/// </param>
		/// <param name="end">
		///         The index of the character following the last character in the
		///         subsequence
		/// </param>
		/// <returns>  A reference to this <tt>Appendable</tt>
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          If <tt>start</tt> or <tt>end</tt> are negative, <tt>start</tt>
		///          is greater than <tt>end</tt>, or <tt>end</tt> is greater than
		///          <tt>csq.length()</tt>
		/// </exception>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Appendable append(CharSequence csq, int start, int end) throws java.io.IOException;
		Appendable Append(CharSequence csq, int start, int end);

		/// <summary>
		/// Appends the specified character to this <tt>Appendable</tt>.
		/// </summary>
		/// <param name="c">
		///         The character to append
		/// </param>
		/// <returns>  A reference to this <tt>Appendable</tt>
		/// </returns>
		/// <exception cref="IOException">
		///          If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Appendable append(char c) throws java.io.IOException;
		Appendable Append(char c);
	}

}