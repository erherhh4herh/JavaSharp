using System;
using System.Diagnostics;
using System.Collections.Generic;

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
	/// A compiled representation of a regular expression.
	/// 
	/// <para> A regular expression, specified as a string, must first be compiled into
	/// an instance of this class.  The resulting pattern can then be used to create
	/// a <seealso cref="Matcher"/> object that can match arbitrary {@linkplain
	/// java.lang.CharSequence character sequences} against the regular
	/// expression.  All of the state involved in performing a match resides in the
	/// matcher, so many matchers can share the same pattern.
	/// 
	/// </para>
	/// <para> A typical invocation sequence is thus
	/// 
	/// <blockquote><pre>
	/// Pattern p = Pattern.<seealso cref="#compile compile"/>("a*b");
	/// Matcher m = p.<seealso cref="#matcher matcher"/>("aaaaab");
	/// boolean b = m.<seealso cref="Matcher#matches matches"/>();</pre></blockquote>
	/// 
	/// </para>
	/// <para> A <seealso cref="#matches matches"/> method is defined by this class as a
	/// convenience for when a regular expression is used just once.  This method
	/// compiles an expression and matches an input sequence against it in a single
	/// invocation.  The statement
	/// 
	/// <blockquote><pre>
	/// boolean b = Pattern.matches("a*b", "aaaaab");</pre></blockquote>
	/// 
	/// is equivalent to the three statements above, though for repeated matches it
	/// is less efficient since it does not allow the compiled pattern to be reused.
	/// 
	/// </para>
	/// <para> Instances of this class are immutable and are safe for use by multiple
	/// concurrent threads.  Instances of the <seealso cref="Matcher"/> class are not safe for
	/// such use.
	/// 
	/// 
	/// <h3><a name="sum">Summary of regular-expression constructs</a></h3>
	/// 
	/// <table border="0" cellpadding="1" cellspacing="0"
	///  summary="Regular expression constructs, and what they match">
	/// 
	/// <tr align="left">
	/// <th align="left" id="construct">Construct</th>
	/// <th align="left" id="matches">Matches</th>
	/// </tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="characters">Characters</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct characters"><i>x</i></td>
	///     <td headers="matches">The character <i>x</i></td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\\</tt></td>
	///     <td headers="matches">The backslash character</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\0</tt><i>n</i></td>
	///     <td headers="matches">The character with octal value <tt>0</tt><i>n</i>
	///         (0&nbsp;<tt>&lt;=</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;7)</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\0</tt><i>nn</i></td>
	///     <td headers="matches">The character with octal value <tt>0</tt><i>nn</i>
	///         (0&nbsp;<tt>&lt;=</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;7)</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\0</tt><i>mnn</i></td>
	///     <td headers="matches">The character with octal value <tt>0</tt><i>mnn</i>
	///         (0&nbsp;<tt>&lt;=</tt>&nbsp;<i>m</i>&nbsp;<tt>&lt;=</tt>&nbsp;3,
	///         0&nbsp;<tt>&lt;=</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;7)</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\x</tt><i>hh</i></td>
	///     <td headers="matches">The character with hexadecimal&nbsp;value&nbsp;<tt>0x</tt><i>hh</i></td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>&#92;u</tt><i>hhhh</i></td>
	///     <td headers="matches">The character with hexadecimal&nbsp;value&nbsp;<tt>0x</tt><i>hhhh</i></td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>&#92;x</tt><i>{h...h}</i></td>
	///     <td headers="matches">The character with hexadecimal&nbsp;value&nbsp;<tt>0x</tt><i>h...h</i>
	///         (<seealso cref="java.lang.Character#MIN_CODE_POINT Character.MIN_CODE_POINT"/>
	///         &nbsp;&lt;=&nbsp;<tt>0x</tt><i>h...h</i>&nbsp;&lt;=&nbsp;
	///          <seealso cref="java.lang.Character#MAX_CODE_POINT Character.MAX_CODE_POINT"/>)</td></tr>
	/// <tr><td valign="top" headers="matches"><tt>\t</tt></td>
	///     <td headers="matches">The tab character (<tt>'&#92;u0009'</tt>)</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\n</tt></td>
	///     <td headers="matches">The newline (line feed) character (<tt>'&#92;u000A'</tt>)</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\r</tt></td>
	///     <td headers="matches">The carriage-return character (<tt>'&#92;u000D'</tt>)</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\f</tt></td>
	///     <td headers="matches">The form-feed character (<tt>'&#92;u000C'</tt>)</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\a</tt></td>
	///     <td headers="matches">The alert (bell) character (<tt>'&#92;u0007'</tt>)</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\e</tt></td>
	///     <td headers="matches">The escape character (<tt>'&#92;u001B'</tt>)</td></tr>
	/// <tr><td valign="top" headers="construct characters"><tt>\c</tt><i>x</i></td>
	///     <td headers="matches">The control character corresponding to <i>x</i></td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="classes">Character classes</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct classes">{@code [abc]}</td>
	///     <td headers="matches">{@code a}, {@code b}, or {@code c} (simple class)</td></tr>
	/// <tr><td valign="top" headers="construct classes">{@code [^abc]}</td>
	///     <td headers="matches">Any character except {@code a}, {@code b}, or {@code c} (negation)</td></tr>
	/// <tr><td valign="top" headers="construct classes">{@code [a-zA-Z]}</td>
	///     <td headers="matches">{@code a} through {@code z}
	///         or {@code A} through {@code Z}, inclusive (range)</td></tr>
	/// <tr><td valign="top" headers="construct classes">{@code [a-d[m-p]]}</td>
	///     <td headers="matches">{@code a} through {@code d},
	///      or {@code m} through {@code p}: {@code [a-dm-p]} (union)</td></tr>
	/// <tr><td valign="top" headers="construct classes">{@code [a-z&&[def]]}</td>
	///     <td headers="matches">{@code d}, {@code e}, or {@code f} (intersection)</tr>
	/// <tr><td valign="top" headers="construct classes">{@code [a-z&&[^bc]]}</td>
	///     <td headers="matches">{@code a} through {@code z},
	///         except for {@code b} and {@code c}: {@code [ad-z]} (subtraction)</td></tr>
	/// <tr><td valign="top" headers="construct classes">{@code [a-z&&[^m-p]]}</td>
	///     <td headers="matches">{@code a} through {@code z},
	///          and not {@code m} through {@code p}: {@code [a-lq-z]}(subtraction)</td></tr>
	/// <tr><th>&nbsp;</th></tr>
	/// 
	/// <tr align="left"><th colspan="2" id="predef">Predefined character classes</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct predef"><tt>.</tt></td>
	///     <td headers="matches">Any character (may or may not match <a href="#lt">line terminators</a>)</td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\d</tt></td>
	///     <td headers="matches">A digit: <tt>[0-9]</tt></td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\D</tt></td>
	///     <td headers="matches">A non-digit: <tt>[^0-9]</tt></td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\h</tt></td>
	///     <td headers="matches">A horizontal whitespace character:
	///     <tt>[ \t\xA0&#92;u1680&#92;u180e&#92;u2000-&#92;u200a&#92;u202f&#92;u205f&#92;u3000]</tt></td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\H</tt></td>
	///     <td headers="matches">A non-horizontal whitespace character: <tt>[^\h]</tt></td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\s</tt></td>
	///     <td headers="matches">A whitespace character: <tt>[ \t\n\x0B\f\r]</tt></td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\S</tt></td>
	///     <td headers="matches">A non-whitespace character: <tt>[^\s]</tt></td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\v</tt></td>
	///     <td headers="matches">A vertical whitespace character: <tt>[\n\x0B\f\r\x85&#92;u2028&#92;u2029]</tt>
	///     </td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\V</tt></td>
	///     <td headers="matches">A non-vertical whitespace character: <tt>[^\v]</tt></td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\w</tt></td>
	///     <td headers="matches">A word character: <tt>[a-zA-Z_0-9]</tt></td></tr>
	/// <tr><td valign="top" headers="construct predef"><tt>\W</tt></td>
	///     <td headers="matches">A non-word character: <tt>[^\w]</tt></td></tr>
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="posix"><b>POSIX character classes (US-ASCII only)</b></th></tr>
	/// 
	/// <tr><td valign="top" headers="construct posix">{@code \p{Lower}}</td>
	///     <td headers="matches">A lower-case alphabetic character: {@code [a-z]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{Upper}}</td>
	///     <td headers="matches">An upper-case alphabetic character:{@code [A-Z]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{ASCII}}</td>
	///     <td headers="matches">All ASCII:{@code [\x00-\x7F]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{Alpha}}</td>
	///     <td headers="matches">An alphabetic character:{@code [\p{Lower}\p{Upper}]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{Digit}}</td>
	///     <td headers="matches">A decimal digit: {@code [0-9]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{Alnum}}</td>
	///     <td headers="matches">An alphanumeric character:{@code [\p{Alpha}\p{Digit}]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{Punct}}</td>
	///     <td headers="matches">Punctuation: One of {@code !"#$%&'()*+,-./:;<=>?@[\]^_`{|}~}</td></tr>
	///     <!-- {@code [\!"#\$%&'\(\)\*\+,\-\./:;\<=\>\?@\[\\\]\^_`\{\|\}~]}
	///          {@code [\X21-\X2F\X31-\X40\X5B-\X60\X7B-\X7E]} -->
	/// <tr><td valign="top" headers="construct posix">{@code \p{Graph}}</td>
	///     <td headers="matches">A visible character: {@code [\p{Alnum}\p{Punct}]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{Print}}</td>
	///     <td headers="matches">A printable character: {@code [\p{Graph}\x20]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{Blank}}</td>
	///     <td headers="matches">A space or a tab: {@code [ \t]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{Cntrl}}</td>
	///     <td headers="matches">A control character: {@code [\x00-\x1F\x7F]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{XDigit}}</td>
	///     <td headers="matches">A hexadecimal digit: {@code [0-9a-fA-F]}</td></tr>
	/// <tr><td valign="top" headers="construct posix">{@code \p{Space}}</td>
	///     <td headers="matches">A whitespace character: {@code [ \t\n\x0B\f\r]}</td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2">java.lang.Character classes (simple <a href="#jcc">java character type</a>)</th></tr>
	/// 
	/// <tr><td valign="top"><tt>\p{javaLowerCase}</tt></td>
	///     <td>Equivalent to java.lang.Character.isLowerCase()</td></tr>
	/// <tr><td valign="top"><tt>\p{javaUpperCase}</tt></td>
	///     <td>Equivalent to java.lang.Character.isUpperCase()</td></tr>
	/// <tr><td valign="top"><tt>\p{javaWhitespace}</tt></td>
	///     <td>Equivalent to java.lang.Character.isWhitespace()</td></tr>
	/// <tr><td valign="top"><tt>\p{javaMirrored}</tt></td>
	///     <td>Equivalent to java.lang.Character.isMirrored()</td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="unicode">Classes for Unicode scripts, blocks, categories and binary properties</th></tr>
	/// <tr><td valign="top" headers="construct unicode">{@code \p{IsLatin}}</td>
	///     <td headers="matches">A Latin&nbsp;script character (<a href="#usc">script</a>)</td></tr>
	/// <tr><td valign="top" headers="construct unicode">{@code \p{InGreek}}</td>
	///     <td headers="matches">A character in the Greek&nbsp;block (<a href="#ubc">block</a>)</td></tr>
	/// <tr><td valign="top" headers="construct unicode">{@code \p{Lu}}</td>
	///     <td headers="matches">An uppercase letter (<a href="#ucc">category</a>)</td></tr>
	/// <tr><td valign="top" headers="construct unicode">{@code \p{IsAlphabetic}}</td>
	///     <td headers="matches">An alphabetic character (<a href="#ubpc">binary property</a>)</td></tr>
	/// <tr><td valign="top" headers="construct unicode">{@code \p{Sc}}</td>
	///     <td headers="matches">A currency symbol</td></tr>
	/// <tr><td valign="top" headers="construct unicode">{@code \P{InGreek}}</td>
	///     <td headers="matches">Any character except one in the Greek block (negation)</td></tr>
	/// <tr><td valign="top" headers="construct unicode">{@code [\p{L}&&[^\p{Lu}]]}</td>
	///     <td headers="matches">Any letter except an uppercase letter (subtraction)</td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="bounds">Boundary matchers</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct bounds"><tt>^</tt></td>
	///     <td headers="matches">The beginning of a line</td></tr>
	/// <tr><td valign="top" headers="construct bounds"><tt>$</tt></td>
	///     <td headers="matches">The end of a line</td></tr>
	/// <tr><td valign="top" headers="construct bounds"><tt>\b</tt></td>
	///     <td headers="matches">A word boundary</td></tr>
	/// <tr><td valign="top" headers="construct bounds"><tt>\B</tt></td>
	///     <td headers="matches">A non-word boundary</td></tr>
	/// <tr><td valign="top" headers="construct bounds"><tt>\A</tt></td>
	///     <td headers="matches">The beginning of the input</td></tr>
	/// <tr><td valign="top" headers="construct bounds"><tt>\G</tt></td>
	///     <td headers="matches">The end of the previous match</td></tr>
	/// <tr><td valign="top" headers="construct bounds"><tt>\Z</tt></td>
	///     <td headers="matches">The end of the input but for the final
	///         <a href="#lt">terminator</a>, if&nbsp;any</td></tr>
	/// <tr><td valign="top" headers="construct bounds"><tt>\z</tt></td>
	///     <td headers="matches">The end of the input</td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="lineending">Linebreak matcher</th></tr>
	/// <tr><td valign="top" headers="construct lineending"><tt>\R</tt></td>
	///     <td headers="matches">Any Unicode linebreak sequence, is equivalent to
	///     <tt>&#92;u000D&#92;u000A|[&#92;u000A&#92;u000B&#92;u000C&#92;u000D&#92;u0085&#92;u2028&#92;u2029]
	///     </tt></td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="greedy">Greedy quantifiers</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct greedy"><i>X</i><tt>?</tt></td>
	///     <td headers="matches"><i>X</i>, once or not at all</td></tr>
	/// <tr><td valign="top" headers="construct greedy"><i>X</i><tt>*</tt></td>
	///     <td headers="matches"><i>X</i>, zero or more times</td></tr>
	/// <tr><td valign="top" headers="construct greedy"><i>X</i><tt>+</tt></td>
	///     <td headers="matches"><i>X</i>, one or more times</td></tr>
	/// <tr><td valign="top" headers="construct greedy"><i>X</i><tt>{</tt><i>n</i><tt>}</tt></td>
	///     <td headers="matches"><i>X</i>, exactly <i>n</i> times</td></tr>
	/// <tr><td valign="top" headers="construct greedy"><i>X</i><tt>{</tt><i>n</i><tt>,}</tt></td>
	///     <td headers="matches"><i>X</i>, at least <i>n</i> times</td></tr>
	/// <tr><td valign="top" headers="construct greedy"><i>X</i><tt>{</tt><i>n</i><tt>,</tt><i>m</i><tt>}</tt></td>
	///     <td headers="matches"><i>X</i>, at least <i>n</i> but not more than <i>m</i> times</td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="reluc">Reluctant quantifiers</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct reluc"><i>X</i><tt>??</tt></td>
	///     <td headers="matches"><i>X</i>, once or not at all</td></tr>
	/// <tr><td valign="top" headers="construct reluc"><i>X</i><tt>*?</tt></td>
	///     <td headers="matches"><i>X</i>, zero or more times</td></tr>
	/// <tr><td valign="top" headers="construct reluc"><i>X</i><tt>+?</tt></td>
	///     <td headers="matches"><i>X</i>, one or more times</td></tr>
	/// <tr><td valign="top" headers="construct reluc"><i>X</i><tt>{</tt><i>n</i><tt>}?</tt></td>
	///     <td headers="matches"><i>X</i>, exactly <i>n</i> times</td></tr>
	/// <tr><td valign="top" headers="construct reluc"><i>X</i><tt>{</tt><i>n</i><tt>,}?</tt></td>
	///     <td headers="matches"><i>X</i>, at least <i>n</i> times</td></tr>
	/// <tr><td valign="top" headers="construct reluc"><i>X</i><tt>{</tt><i>n</i><tt>,</tt><i>m</i><tt>}?</tt></td>
	///     <td headers="matches"><i>X</i>, at least <i>n</i> but not more than <i>m</i> times</td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="poss">Possessive quantifiers</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct poss"><i>X</i><tt>?+</tt></td>
	///     <td headers="matches"><i>X</i>, once or not at all</td></tr>
	/// <tr><td valign="top" headers="construct poss"><i>X</i><tt>*+</tt></td>
	///     <td headers="matches"><i>X</i>, zero or more times</td></tr>
	/// <tr><td valign="top" headers="construct poss"><i>X</i><tt>++</tt></td>
	///     <td headers="matches"><i>X</i>, one or more times</td></tr>
	/// <tr><td valign="top" headers="construct poss"><i>X</i><tt>{</tt><i>n</i><tt>}+</tt></td>
	///     <td headers="matches"><i>X</i>, exactly <i>n</i> times</td></tr>
	/// <tr><td valign="top" headers="construct poss"><i>X</i><tt>{</tt><i>n</i><tt>,}+</tt></td>
	///     <td headers="matches"><i>X</i>, at least <i>n</i> times</td></tr>
	/// <tr><td valign="top" headers="construct poss"><i>X</i><tt>{</tt><i>n</i><tt>,</tt><i>m</i><tt>}+</tt></td>
	///     <td headers="matches"><i>X</i>, at least <i>n</i> but not more than <i>m</i> times</td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="logical">Logical operators</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct logical"><i>XY</i></td>
	///     <td headers="matches"><i>X</i> followed by <i>Y</i></td></tr>
	/// <tr><td valign="top" headers="construct logical"><i>X</i><tt>|</tt><i>Y</i></td>
	///     <td headers="matches">Either <i>X</i> or <i>Y</i></td></tr>
	/// <tr><td valign="top" headers="construct logical"><tt>(</tt><i>X</i><tt>)</tt></td>
	///     <td headers="matches">X, as a <a href="#cg">capturing group</a></td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="backref">Back references</th></tr>
	/// 
	/// <tr><td valign="bottom" headers="construct backref"><tt>\</tt><i>n</i></td>
	///     <td valign="bottom" headers="matches">Whatever the <i>n</i><sup>th</sup>
	///     <a href="#cg">capturing group</a> matched</td></tr>
	/// 
	/// <tr><td valign="bottom" headers="construct backref"><tt>\</tt><i>k</i>&lt;<i>name</i>&gt;</td>
	///     <td valign="bottom" headers="matches">Whatever the
	///     <a href="#groupname">named-capturing group</a> "name" matched</td></tr>
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="quot">Quotation</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct quot"><tt>\</tt></td>
	///     <td headers="matches">Nothing, but quotes the following character</td></tr>
	/// <tr><td valign="top" headers="construct quot"><tt>\Q</tt></td>
	///     <td headers="matches">Nothing, but quotes all characters until <tt>\E</tt></td></tr>
	/// <tr><td valign="top" headers="construct quot"><tt>\E</tt></td>
	///     <td headers="matches">Nothing, but ends quoting started by <tt>\Q</tt></td></tr>
	///     <!-- Metachars: !$()*+.<>?[\]^{|} -->
	/// 
	/// <tr><th>&nbsp;</th></tr>
	/// <tr align="left"><th colspan="2" id="special">Special constructs (named-capturing and non-capturing)</th></tr>
	/// 
	/// <tr><td valign="top" headers="construct special"><tt>(?&lt;<a href="#groupname">name</a>&gt;</tt><i>X</i><tt>)</tt></td>
	///     <td headers="matches"><i>X</i>, as a named-capturing group</td></tr>
	/// <tr><td valign="top" headers="construct special"><tt>(?:</tt><i>X</i><tt>)</tt></td>
	///     <td headers="matches"><i>X</i>, as a non-capturing group</td></tr>
	/// <tr><td valign="top" headers="construct special"><tt>(?idmsuxU-idmsuxU)&nbsp;</tt></td>
	///     <td headers="matches">Nothing, but turns match flags <a href="#CASE_INSENSITIVE">i</a>
	/// <a href="#UNIX_LINES">d</a> <a href="#MULTILINE">m</a> <a href="#DOTALL">s</a>
	/// <a href="#UNICODE_CASE">u</a> <a href="#COMMENTS">x</a> <a href="#UNICODE_CHARACTER_CLASS">U</a>
	/// on - off</td></tr>
	/// <tr><td valign="top" headers="construct special"><tt>(?idmsux-idmsux:</tt><i>X</i><tt>)</tt>&nbsp;&nbsp;</td>
	///     <td headers="matches"><i>X</i>, as a <a href="#cg">non-capturing group</a> with the
	///         given flags <a href="#CASE_INSENSITIVE">i</a> <a href="#UNIX_LINES">d</a>
	/// <a href="#MULTILINE">m</a> <a href="#DOTALL">s</a> <a href="#UNICODE_CASE">u</a >
	/// <a href="#COMMENTS">x</a> on - off</td></tr>
	/// <tr><td valign="top" headers="construct special"><tt>(?=</tt><i>X</i><tt>)</tt></td>
	///     <td headers="matches"><i>X</i>, via zero-width positive lookahead</td></tr>
	/// <tr><td valign="top" headers="construct special"><tt>(?!</tt><i>X</i><tt>)</tt></td>
	///     <td headers="matches"><i>X</i>, via zero-width negative lookahead</td></tr>
	/// <tr><td valign="top" headers="construct special"><tt>(?&lt;=</tt><i>X</i><tt>)</tt></td>
	///     <td headers="matches"><i>X</i>, via zero-width positive lookbehind</td></tr>
	/// <tr><td valign="top" headers="construct special"><tt>(?&lt;!</tt><i>X</i><tt>)</tt></td>
	///     <td headers="matches"><i>X</i>, via zero-width negative lookbehind</td></tr>
	/// <tr><td valign="top" headers="construct special"><tt>(?&gt;</tt><i>X</i><tt>)</tt></td>
	///     <td headers="matches"><i>X</i>, as an independent, non-capturing group</td></tr>
	/// 
	/// </table>
	/// 
	/// <hr>
	/// 
	/// 
	/// <h3><a name="bs">Backslashes, escapes, and quoting</a></h3>
	/// 
	/// </para>
	/// <para> The backslash character (<tt>'\'</tt>) serves to introduce escaped
	/// constructs, as defined in the table above, as well as to quote characters
	/// that otherwise would be interpreted as unescaped constructs.  Thus the
	/// expression <tt>\\</tt> matches a single backslash and <tt>\{</tt> matches a
	/// left brace.
	/// 
	/// </para>
	/// <para> It is an error to use a backslash prior to any alphabetic character that
	/// does not denote an escaped construct; these are reserved for future
	/// extensions to the regular-expression language.  A backslash may be used
	/// prior to a non-alphabetic character regardless of whether that character is
	/// part of an unescaped construct.
	/// 
	/// </para>
	/// <para> Backslashes within string literals in Java source code are interpreted
	/// as required by
	/// <cite>The Java&trade; Language Specification</cite>
	/// as either Unicode escapes (section 3.3) or other character escapes (section 3.10.6)
	/// It is therefore necessary to double backslashes in string
	/// literals that represent regular expressions to protect them from
	/// interpretation by the Java bytecode compiler.  The string literal
	/// <tt>"&#92;b"</tt>, for example, matches a single backspace character when
	/// interpreted as a regular expression, while <tt>"&#92;&#92;b"</tt> matches a
	/// word boundary.  The string literal <tt>"&#92;(hello&#92;)"</tt> is illegal
	/// and leads to a compile-time error; in order to match the string
	/// <tt>(hello)</tt> the string literal <tt>"&#92;&#92;(hello&#92;&#92;)"</tt>
	/// must be used.
	/// 
	/// <h3><a name="cc">Character Classes</a></h3>
	/// 
	/// </para>
	///    <para> Character classes may appear within other character classes, and
	///    may be composed by the union operator (implicit) and the intersection
	///    operator (<tt>&amp;&amp;</tt>).
	///    The union operator denotes a class that contains every character that is
	///    in at least one of its operand classes.  The intersection operator
	///    denotes a class that contains every character that is in both of its
	///    operand classes.
	/// 
	/// </para>
	///    <para> The precedence of character-class operators is as follows, from
	///    highest to lowest:
	/// 
	///    <blockquote><table border="0" cellpadding="1" cellspacing="0"
	///                 summary="Precedence of character class operators.">
	///      <tr><th>1&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///        <td>Literal escape&nbsp;&nbsp;&nbsp;&nbsp;</td>
	///        <td><tt>\x</tt></td></tr>
	///     <tr><th>2&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///        <td>Grouping</td>
	///        <td><tt>[...]</tt></td></tr>
	///     <tr><th>3&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///        <td>Range</td>
	///        <td><tt>a-z</tt></td></tr>
	///      <tr><th>4&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///        <td>Union</td>
	///        <td><tt>[a-e][i-u]</tt></td></tr>
	///      <tr><th>5&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///        <td>Intersection</td>
	///        <td>{@code [a-z&&[aeiou]]}</td></tr>
	///    </table></blockquote>
	/// 
	/// </para>
	///    <para> Note that a different set of metacharacters are in effect inside
	///    a character class than outside a character class. For instance, the
	///    regular expression <tt>.</tt> loses its special meaning inside a
	///    character class, while the expression <tt>-</tt> becomes a range
	///    forming metacharacter.
	/// 
	/// <h3><a name="lt">Line terminators</a></h3>
	/// 
	/// </para>
	/// <para> A <i>line terminator</i> is a one- or two-character sequence that marks
	/// the end of a line of the input character sequence.  The following are
	/// recognized as line terminators:
	/// 
	/// <ul>
	/// 
	///   <li> A newline (line feed) character&nbsp;(<tt>'\n'</tt>),
	/// 
	///   <li> A carriage-return character followed immediately by a newline
	///   character&nbsp;(<tt>"\r\n"</tt>),
	/// 
	///   <li> A standalone carriage-return character&nbsp;(<tt>'\r'</tt>),
	/// 
	///   <li> A next-line character&nbsp;(<tt>'&#92;u0085'</tt>),
	/// 
	///   <li> A line-separator character&nbsp;(<tt>'&#92;u2028'</tt>), or
	/// 
	///   <li> A paragraph-separator character&nbsp;(<tt>'&#92;u2029</tt>).
	/// 
	/// </ul>
	/// </para>
	/// <para>If <seealso cref="#UNIX_LINES"/> mode is activated, then the only line terminators
	/// recognized are newline characters.
	/// 
	/// </para>
	/// <para> The regular expression <tt>.</tt> matches any character except a line
	/// terminator unless the <seealso cref="#DOTALL"/> flag is specified.
	/// 
	/// </para>
	/// <para> By default, the regular expressions <tt>^</tt> and <tt>$</tt> ignore
	/// line terminators and only match at the beginning and the end, respectively,
	/// of the entire input sequence. If <seealso cref="#MULTILINE"/> mode is activated then
	/// <tt>^</tt> matches at the beginning of input and after any line terminator
	/// except at the end of input. When in <seealso cref="#MULTILINE"/> mode <tt>$</tt>
	/// matches just before a line terminator or the end of the input sequence.
	/// 
	/// <h3><a name="cg">Groups and capturing</a></h3>
	/// 
	/// <h4><a name="gnumber">Group number</a></h4>
	/// </para>
	/// <para> Capturing groups are numbered by counting their opening parentheses from
	/// left to right.  In the expression <tt>((A)(B(C)))</tt>, for example, there
	/// are four such groups: </para>
	/// 
	/// <blockquote><table cellpadding=1 cellspacing=0 summary="Capturing group numberings">
	/// <tr><th>1&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///     <td><tt>((A)(B(C)))</tt></td></tr>
	/// <tr><th>2&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///     <td><tt>(A)</tt></td></tr>
	/// <tr><th>3&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///     <td><tt>(B(C))</tt></td></tr>
	/// <tr><th>4&nbsp;&nbsp;&nbsp;&nbsp;</th>
	///     <td><tt>(C)</tt></td></tr>
	/// </table></blockquote>
	/// 
	/// <para> Group zero always stands for the entire expression.
	/// 
	/// </para>
	/// <para> Capturing groups are so named because, during a match, each subsequence
	/// of the input sequence that matches such a group is saved.  The captured
	/// subsequence may be used later in the expression, via a back reference, and
	/// may also be retrieved from the matcher once the match operation is complete.
	/// 
	/// <h4><a name="groupname">Group name</a></h4>
	/// </para>
	/// <para>A capturing group can also be assigned a "name", a <tt>named-capturing group</tt>,
	/// and then be back-referenced later by the "name". Group names are composed of
	/// the following characters. The first character must be a <tt>letter</tt>.
	/// 
	/// <ul>
	///   <li> The uppercase letters <tt>'A'</tt> through <tt>'Z'</tt>
	///        (<tt>'&#92;u0041'</tt>&nbsp;through&nbsp;<tt>'&#92;u005a'</tt>),
	///   <li> The lowercase letters <tt>'a'</tt> through <tt>'z'</tt>
	///        (<tt>'&#92;u0061'</tt>&nbsp;through&nbsp;<tt>'&#92;u007a'</tt>),
	///   <li> The digits <tt>'0'</tt> through <tt>'9'</tt>
	///        (<tt>'&#92;u0030'</tt>&nbsp;through&nbsp;<tt>'&#92;u0039'</tt>),
	/// </ul>
	/// 
	/// </para>
	/// <para> A <tt>named-capturing group</tt> is still numbered as described in
	/// <a href="#gnumber">Group number</a>.
	/// 
	/// </para>
	/// <para> The captured input associated with a group is always the subsequence
	/// that the group most recently matched.  If a group is evaluated a second time
	/// because of quantification then its previously-captured value, if any, will
	/// be retained if the second evaluation fails.  Matching the string
	/// <tt>"aba"</tt> against the expression <tt>(a(b)?)+</tt>, for example, leaves
	/// group two set to <tt>"b"</tt>.  All captured input is discarded at the
	/// beginning of each match.
	/// 
	/// </para>
	/// <para> Groups beginning with <tt>(?</tt> are either pure, <i>non-capturing</i> groups
	/// that do not capture text and do not count towards the group total, or
	/// <i>named-capturing</i> group.
	/// 
	/// <h3> Unicode support </h3>
	/// 
	/// </para>
	/// <para> This class is in conformance with Level 1 of <a
	/// href="http://www.unicode.org/reports/tr18/"><i>Unicode Technical
	/// Standard #18: Unicode Regular Expression</i></a>, plus RL2.1
	/// Canonical Equivalents.
	/// </para>
	/// <para>
	/// <b>Unicode escape sequences</b> such as <tt>&#92;u2014</tt> in Java source code
	/// are processed as described in section 3.3 of
	/// <cite>The Java&trade; Language Specification</cite>.
	/// Such escape sequences are also implemented directly by the regular-expression
	/// parser so that Unicode escapes can be used in expressions that are read from
	/// files or from the keyboard.  Thus the strings <tt>"&#92;u2014"</tt> and
	/// <tt>"\\u2014"</tt>, while not equal, compile into the same pattern, which
	/// matches the character with hexadecimal value <tt>0x2014</tt>.
	/// </para>
	/// <para>
	/// A Unicode character can also be represented in a regular-expression by
	/// using its <b>Hex notation</b>(hexadecimal code point value) directly as described in construct
	/// <tt>&#92;x{...}</tt>, for example a supplementary character U+2011F
	/// can be specified as <tt>&#92;x{2011F}</tt>, instead of two consecutive
	/// Unicode escape sequences of the surrogate pair
	/// <tt>&#92;uD840</tt><tt>&#92;uDD1F</tt>.
	/// </para>
	/// <para>
	/// Unicode scripts, blocks, categories and binary properties are written with
	/// the <tt>\p</tt> and <tt>\P</tt> constructs as in Perl.
	/// <tt>\p{</tt><i>prop</i><tt>}</tt> matches if
	/// the input has the property <i>prop</i>, while <tt>\P{</tt><i>prop</i><tt>}</tt>
	/// does not match if the input has that property.
	/// </para>
	/// <para>
	/// Scripts, blocks, categories and binary properties can be used both inside
	/// and outside of a character class.
	/// 
	/// </para>
	/// <para>
	/// <b><a name="usc">Scripts</a></b> are specified either with the prefix {@code Is}, as in
	/// {@code IsHiragana}, or by using  the {@code script} keyword (or its short
	/// form {@code sc})as in {@code script=Hiragana} or {@code sc=Hiragana}.
	/// </para>
	/// <para>
	/// The script names supported by <code>Pattern</code> are the valid script names
	/// accepted and defined by
	/// <seealso cref="java.lang.Character.UnicodeScript#forName(String) UnicodeScript.forName"/>.
	/// 
	/// </para>
	/// <para>
	/// <b><a name="ubc">Blocks</a></b> are specified with the prefix {@code In}, as in
	/// {@code InMongolian}, or by using the keyword {@code block} (or its short
	/// form {@code blk}) as in {@code block=Mongolian} or {@code blk=Mongolian}.
	/// </para>
	/// <para>
	/// The block names supported by <code>Pattern</code> are the valid block names
	/// accepted and defined by
	/// <seealso cref="java.lang.Character.UnicodeBlock#forName(String) UnicodeBlock.forName"/>.
	/// </para>
	/// <para>
	/// 
	/// <b><a name="ucc">Categories</a></b> may be specified with the optional prefix {@code Is}:
	/// Both {@code \p{L}} and {@code \p{IsL}} denote the category of Unicode
	/// letters. Same as scripts and blocks, categories can also be specified
	/// by using the keyword {@code general_category} (or its short form
	/// {@code gc}) as in {@code general_category=Lu} or {@code gc=Lu}.
	/// </para>
	/// <para>
	/// The supported categories are those of
	/// <a href="http://www.unicode.org/unicode/standard/standard.html">
	/// <i>The Unicode Standard</i></a> in the version specified by the
	/// <seealso cref="java.lang.Character Character"/> class. The category names are those
	/// defined in the Standard, both normative and informative.
	/// </para>
	/// <para>
	/// 
	/// <b><a name="ubpc">Binary properties</a></b> are specified with the prefix {@code Is}, as in
	/// {@code IsAlphabetic}. The supported binary properties by <code>Pattern</code>
	/// are
	/// <ul>
	///   <li> Alphabetic
	///   <li> Ideographic
	///   <li> Letter
	///   <li> Lowercase
	///   <li> Uppercase
	///   <li> Titlecase
	///   <li> Punctuation
	///   <Li> Control
	///   <li> White_Space
	///   <li> Digit
	///   <li> Hex_Digit
	///   <li> Join_Control
	///   <li> Noncharacter_Code_Point
	///   <li> Assigned
	/// </ul>
	/// </para>
	/// <para>
	/// The following <b>Predefined Character classes</b> and <b>POSIX character classes</b>
	/// are in conformance with the recommendation of <i>Annex C: Compatibility Properties</i>
	/// of <a href="http://www.unicode.org/reports/tr18/"><i>Unicode Regular Expression
	/// </i></a>, when <seealso cref="#UNICODE_CHARACTER_CLASS"/> flag is specified.
	/// 
	/// <table border="0" cellpadding="1" cellspacing="0"
	///  summary="predefined and posix character classes in Unicode mode">
	/// <tr align="left">
	/// <th align="left" id="predef_classes">Classes</th>
	/// <th align="left" id="predef_matches">Matches</th>
	/// </tr>
	/// <tr><td><tt>\p{Lower}</tt></td>
	///     <td>A lowercase character:<tt>\p{IsLowercase}</tt></td></tr>
	/// <tr><td><tt>\p{Upper}</tt></td>
	///     <td>An uppercase character:<tt>\p{IsUppercase}</tt></td></tr>
	/// <tr><td><tt>\p{ASCII}</tt></td>
	///     <td>All ASCII:<tt>[\x00-\x7F]</tt></td></tr>
	/// <tr><td><tt>\p{Alpha}</tt></td>
	///     <td>An alphabetic character:<tt>\p{IsAlphabetic}</tt></td></tr>
	/// <tr><td><tt>\p{Digit}</tt></td>
	///     <td>A decimal digit character:<tt>p{IsDigit}</tt></td></tr>
	/// <tr><td><tt>\p{Alnum}</tt></td>
	///     <td>An alphanumeric character:<tt>[\p{IsAlphabetic}\p{IsDigit}]</tt></td></tr>
	/// <tr><td><tt>\p{Punct}</tt></td>
	///     <td>A punctuation character:<tt>p{IsPunctuation}</tt></td></tr>
	/// <tr><td><tt>\p{Graph}</tt></td>
	///     <td>A visible character: <tt>[^\p{IsWhite_Space}\p{gc=Cc}\p{gc=Cs}\p{gc=Cn}]</tt></td></tr>
	/// <tr><td><tt>\p{Print}</tt></td>
	///     <td>A printable character: {@code [\p{Graph}\p{Blank}&&[^\p{Cntrl}]]}</td></tr>
	/// <tr><td><tt>\p{Blank}</tt></td>
	///     <td>A space or a tab: {@code [\p{IsWhite_Space}&&[^\p{gc=Zl}\p{gc=Zp}\x0a\x0b\x0c\x0d\x85]]}</td></tr>
	/// <tr><td><tt>\p{Cntrl}</tt></td>
	///     <td>A control character: <tt>\p{gc=Cc}</tt></td></tr>
	/// <tr><td><tt>\p{XDigit}</tt></td>
	///     <td>A hexadecimal digit: <tt>[\p{gc=Nd}\p{IsHex_Digit}]</tt></td></tr>
	/// <tr><td><tt>\p{Space}</tt></td>
	///     <td>A whitespace character:<tt>\p{IsWhite_Space}</tt></td></tr>
	/// <tr><td><tt>\d</tt></td>
	///     <td>A digit: <tt>\p{IsDigit}</tt></td></tr>
	/// <tr><td><tt>\D</tt></td>
	///     <td>A non-digit: <tt>[^\d]</tt></td></tr>
	/// <tr><td><tt>\s</tt></td>
	///     <td>A whitespace character: <tt>\p{IsWhite_Space}</tt></td></tr>
	/// <tr><td><tt>\S</tt></td>
	///     <td>A non-whitespace character: <tt>[^\s]</tt></td></tr>
	/// <tr><td><tt>\w</tt></td>
	///     <td>A word character: <tt>[\p{Alpha}\p{gc=Mn}\p{gc=Me}\p{gc=Mc}\p{Digit}\p{gc=Pc}\p{IsJoin_Control}]</tt></td></tr>
	/// <tr><td><tt>\W</tt></td>
	///     <td>A non-word character: <tt>[^\w]</tt></td></tr>
	/// </table>
	/// </para>
	/// <para>
	/// <a name="jcc">
	/// Categories that behave like the java.lang.Character
	/// boolean is<i>methodname</i> methods (except for the deprecated ones) are
	/// available through the same <tt>\p{</tt><i>prop</i><tt>}</tt> syntax where
	/// the specified property has the name <tt>java<i>methodname</i></tt></a>.
	/// 
	/// <h3> Comparison to Perl 5 </h3>
	/// 
	/// </para>
	/// <para>The <code>Pattern</code> engine performs traditional NFA-based matching
	/// with ordered alternation as occurs in Perl 5.
	/// 
	/// </para>
	/// <para> Perl constructs not supported by this class: </para>
	/// 
	/// <ul>
	///    <li><para> Predefined character classes (Unicode character)
	/// </para>
	///    <para><tt>\X&nbsp;&nbsp;&nbsp;&nbsp;</tt>Match Unicode
	///    <a href="http://www.unicode.org/reports/tr18/#Default_Grapheme_Clusters">
	///    <i>extended grapheme cluster</i></a>
	///    </para></li>
	/// 
	///    <li><para> The backreference constructs, <tt>\g{</tt><i>n</i><tt>}</tt> for
	///    the <i>n</i><sup>th</sup><a href="#cg">capturing group</a> and
	///    <tt>\g{</tt><i>name</i><tt>}</tt> for
	///    <a href="#groupname">named-capturing group</a>.
	///    </para></li>
	/// 
	///    <li><para> The named character construct, <tt>\N{</tt><i>name</i><tt>}</tt>
	///    for a Unicode character by its name.
	///    </para></li>
	/// 
	///    <li><para> The conditional constructs
	///    <tt>(?(</tt><i>condition</i><tt>)</tt><i>X</i><tt>)</tt> and
	///    <tt>(?(</tt><i>condition</i><tt>)</tt><i>X</i><tt>|</tt><i>Y</i><tt>)</tt>,
	///    </para></li>
	/// 
	///    <li><para> The embedded code constructs <tt>(?{</tt><i>code</i><tt>})</tt>
	///    and <tt>(??{</tt><i>code</i><tt>})</tt>,</para></li>
	/// 
	///    <li><para> The embedded comment syntax <tt>(?#comment)</tt>, and </para></li>
	/// 
	///    <li><para> The preprocessing operations <tt>\l</tt> <tt>&#92;u</tt>,
	///    <tt>\L</tt>, and <tt>\U</tt>.  </para></li>
	/// 
	/// </ul>
	/// 
	/// <para> Constructs supported by this class but not by Perl: </para>
	/// 
	/// <ul>
	/// 
	///    <li><para> Character-class union and intersection as described
	///    <a href="#cc">above</a>.</para></li>
	/// 
	/// </ul>
	/// 
	/// <para> Notable differences from Perl: </para>
	/// 
	/// <ul>
	/// 
	///    <li><para> In Perl, <tt>\1</tt> through <tt>\9</tt> are always interpreted
	///    as back references; a backslash-escaped number greater than <tt>9</tt> is
	///    treated as a back reference if at least that many subexpressions exist,
	///    otherwise it is interpreted, if possible, as an octal escape.  In this
	///    class octal escapes must always begin with a zero. In this class,
	///    <tt>\1</tt> through <tt>\9</tt> are always interpreted as back
	///    references, and a larger number is accepted as a back reference if at
	///    least that many subexpressions exist at that point in the regular
	///    expression, otherwise the parser will drop digits until the number is
	///    smaller or equal to the existing number of groups or it is one digit.
	///    </para></li>
	/// 
	///    <li><para> Perl uses the <tt>g</tt> flag to request a match that resumes
	///    where the last match left off.  This functionality is provided implicitly
	///    by the <seealso cref="Matcher"/> class: Repeated invocations of the {@link
	///    Matcher#find find} method will resume where the last match left off,
	///    unless the matcher is reset.  </para></li>
	/// 
	///    <li><para> In Perl, embedded flags at the top level of an expression affect
	///    the whole expression.  In this class, embedded flags always take effect
	///    at the point at which they appear, whether they are at the top level or
	///    within a group; in the latter case, flags are restored at the end of the
	///    group just as in Perl.  </para></li>
	/// 
	/// </ul>
	/// 
	/// 
	/// <para> For a more precise description of the behavior of regular expression
	/// constructs, please see <a href="http://www.oreilly.com/catalog/regex3/">
	/// <i>Mastering Regular Expressions, 3nd Edition</i>, Jeffrey E. F. Friedl,
	/// O'Reilly and Associates, 2006.</a>
	/// </para>
	/// </summary>
	/// <seealso cref= java.lang.String#split(String, int) </seealso>
	/// <seealso cref= java.lang.String#split(String)
	/// 
	/// @author      Mike McCloskey
	/// @author      Mark Reinhold
	/// @author      JSR-51 Expert Group
	/// @since       1.4
	/// @spec        JSR-51 </seealso>

	[Serializable]
	public sealed class Pattern
	{

		/// <summary>
		/// Regular expression modifier values.  Instead of being passed as
		/// arguments, they can also be passed as inline modifiers.
		/// For example, the following statements have the same effect.
		/// <pre>
		/// RegExp r1 = RegExp.compile("abc", Pattern.I|Pattern.M);
		/// RegExp r2 = RegExp.compile("(?im)abc", 0);
		/// </pre>
		/// 
		/// The flags are duplicated so that the familiar Perl match flag
		/// names are available.
		/// </summary>

		/// <summary>
		/// Enables Unix lines mode.
		/// 
		/// <para> In this mode, only the <tt>'\n'</tt> line terminator is recognized
		/// in the behavior of <tt>.</tt>, <tt>^</tt>, and <tt>$</tt>.
		/// 
		/// </para>
		/// <para> Unix lines mode can also be enabled via the embedded flag
		/// expression&nbsp;<tt>(?d)</tt>.
		/// </para>
		/// </summary>
		public const int UNIX_LINES = 0x01;

		/// <summary>
		/// Enables case-insensitive matching.
		/// 
		/// <para> By default, case-insensitive matching assumes that only characters
		/// in the US-ASCII charset are being matched.  Unicode-aware
		/// case-insensitive matching can be enabled by specifying the {@link
		/// #UNICODE_CASE} flag in conjunction with this flag.
		/// 
		/// </para>
		/// <para> Case-insensitive matching can also be enabled via the embedded flag
		/// expression&nbsp;<tt>(?i)</tt>.
		/// 
		/// </para>
		/// <para> Specifying this flag may impose a slight performance penalty.  </para>
		/// </summary>
		public const int CASE_INSENSITIVE = 0x02;

		/// <summary>
		/// Permits whitespace and comments in pattern.
		/// 
		/// <para> In this mode, whitespace is ignored, and embedded comments starting
		/// with <tt>#</tt> are ignored until the end of a line.
		/// 
		/// </para>
		/// <para> Comments mode can also be enabled via the embedded flag
		/// expression&nbsp;<tt>(?x)</tt>.
		/// </para>
		/// </summary>
		public const int COMMENTS = 0x04;

		/// <summary>
		/// Enables multiline mode.
		/// 
		/// <para> In multiline mode the expressions <tt>^</tt> and <tt>$</tt> match
		/// just after or just before, respectively, a line terminator or the end of
		/// the input sequence.  By default these expressions only match at the
		/// beginning and the end of the entire input sequence.
		/// 
		/// </para>
		/// <para> Multiline mode can also be enabled via the embedded flag
		/// expression&nbsp;<tt>(?m)</tt>.  </para>
		/// </summary>
		public const int MULTILINE = 0x08;

		/// <summary>
		/// Enables literal parsing of the pattern.
		/// 
		/// <para> When this flag is specified then the input string that specifies
		/// the pattern is treated as a sequence of literal characters.
		/// Metacharacters or escape sequences in the input sequence will be
		/// given no special meaning.
		/// 
		/// </para>
		/// <para>The flags CASE_INSENSITIVE and UNICODE_CASE retain their impact on
		/// matching when used in conjunction with this flag. The other flags
		/// become superfluous.
		/// 
		/// </para>
		/// <para> There is no embedded flag character for enabling literal parsing.
		/// @since 1.5
		/// </para>
		/// </summary>
		public const int LITERAL = 0x10;

		/// <summary>
		/// Enables dotall mode.
		/// 
		/// <para> In dotall mode, the expression <tt>.</tt> matches any character,
		/// including a line terminator.  By default this expression does not match
		/// line terminators.
		/// 
		/// </para>
		/// <para> Dotall mode can also be enabled via the embedded flag
		/// expression&nbsp;<tt>(?s)</tt>.  (The <tt>s</tt> is a mnemonic for
		/// "single-line" mode, which is what this is called in Perl.)  </para>
		/// </summary>
		public const int DOTALL = 0x20;

		/// <summary>
		/// Enables Unicode-aware case folding.
		/// 
		/// <para> When this flag is specified then case-insensitive matching, when
		/// enabled by the <seealso cref="#CASE_INSENSITIVE"/> flag, is done in a manner
		/// consistent with the Unicode Standard.  By default, case-insensitive
		/// matching assumes that only characters in the US-ASCII charset are being
		/// matched.
		/// 
		/// </para>
		/// <para> Unicode-aware case folding can also be enabled via the embedded flag
		/// expression&nbsp;<tt>(?u)</tt>.
		/// 
		/// </para>
		/// <para> Specifying this flag may impose a performance penalty.  </para>
		/// </summary>
		public const int UNICODE_CASE = 0x40;

		/// <summary>
		/// Enables canonical equivalence.
		/// 
		/// <para> When this flag is specified then two characters will be considered
		/// to match if, and only if, their full canonical decompositions match.
		/// The expression <tt>"a&#92;u030A"</tt>, for example, will match the
		/// string <tt>"&#92;u00E5"</tt> when this flag is specified.  By default,
		/// matching does not take canonical equivalence into account.
		/// 
		/// </para>
		/// <para> There is no embedded flag character for enabling canonical
		/// equivalence.
		/// 
		/// </para>
		/// <para> Specifying this flag may impose a performance penalty.  </para>
		/// </summary>
		public const int CANON_EQ = 0x80;

		/// <summary>
		/// Enables the Unicode version of <i>Predefined character classes</i> and
		/// <i>POSIX character classes</i>.
		/// 
		/// <para> When this flag is specified then the (US-ASCII only)
		/// <i>Predefined character classes</i> and <i>POSIX character classes</i>
		/// are in conformance with
		/// <a href="http://www.unicode.org/reports/tr18/"><i>Unicode Technical
		/// Standard #18: Unicode Regular Expression</i></a>
		/// <i>Annex C: Compatibility Properties</i>.
		/// </para>
		/// <para>
		/// The UNICODE_CHARACTER_CLASS mode can also be enabled via the embedded
		/// flag expression&nbsp;<tt>(?U)</tt>.
		/// </para>
		/// <para>
		/// The flag implies UNICODE_CASE, that is, it enables Unicode-aware case
		/// folding.
		/// </para>
		/// <para>
		/// Specifying this flag may impose a performance penalty.  </para>
		/// @since 1.7
		/// </summary>
		public const int UNICODE_CHARACTER_CLASS = 0x100;

		/* Pattern has only two serialized components: The pattern string
		 * and the flags, which are all that is needed to recompile the pattern
		 * when it is deserialized.
		 */

		/// <summary>
		/// use serialVersionUID from Merlin b59 for interoperability </summary>
		private const long SerialVersionUID = 5073258162644648461L;

		/// <summary>
		/// The original regular-expression pattern string.
		/// 
		/// @serial
		/// </summary>
		private String Pattern_Renamed;

		/// <summary>
		/// The original pattern flags.
		/// 
		/// @serial
		/// </summary>
		private int Flags_Renamed;

		/// <summary>
		/// Boolean indicating this Pattern is compiled; this is necessary in order
		/// to lazily compile deserialized Patterns.
		/// </summary>
		[NonSerialized]
		private volatile bool Compiled = false;

		/// <summary>
		/// The normalized pattern string.
		/// </summary>
		[NonSerialized]
		private String NormalizedPattern;

		/// <summary>
		/// The starting point of state machine for the find operation.  This allows
		/// a match to start anywhere in the input.
		/// </summary>
		[NonSerialized]
		internal Node Root;

		/// <summary>
		/// The root of object tree for a match operation.  The pattern is matched
		/// at the beginning.  This may include a find that uses BnM or a First
		/// node.
		/// </summary>
		[NonSerialized]
		internal Node MatchRoot;

		/// <summary>
		/// Temporary storage used by parsing pattern slice.
		/// </summary>
		[NonSerialized]
		internal int[] Buffer;

		/// <summary>
		/// Map the "name" of the "named capturing group" to its group id
		/// node.
		/// </summary>
		[NonSerialized]
		internal volatile IDictionary<String, Integer> NamedGroups_Renamed;

		/// <summary>
		/// Temporary storage used while parsing group references.
		/// </summary>
		[NonSerialized]
		internal GroupHead[] GroupNodes;

		/// <summary>
		/// Temporary null terminated code point array used by pattern compiling.
		/// </summary>
		[NonSerialized]
		private int[] Temp;

		/// <summary>
		/// The number of capturing groups in this Pattern. Used by matchers to
		/// allocate storage needed to perform a match.
		/// </summary>
		[NonSerialized]
		internal int CapturingGroupCount;

		/// <summary>
		/// The local variable count used by parsing tree. Used by matchers to
		/// allocate storage needed to perform a match.
		/// </summary>
		[NonSerialized]
		internal int LocalCount;

		/// <summary>
		/// Index into the pattern string that keeps track of how much has been
		/// parsed.
		/// </summary>
		[NonSerialized]
		private int Cursor_Renamed;

		/// <summary>
		/// Holds the length of the pattern string.
		/// </summary>
		[NonSerialized]
		private int PatternLength;

		/// <summary>
		/// If the Start node might possibly match supplementary characters.
		/// It is set to true during compiling if
		/// (1) There is supplementary char in pattern, or
		/// (2) There is complement node of Category or Block
		/// </summary>
		[NonSerialized]
		private bool HasSupplementary;

		/// <summary>
		/// Compiles the given regular expression into a pattern.
		/// </summary>
		/// <param name="regex">
		///         The expression to be compiled </param>
		/// <returns> the given regular expression compiled into a pattern </returns>
		/// <exception cref="PatternSyntaxException">
		///          If the expression's syntax is invalid </exception>
		public static Pattern Compile(String regex)
		{
			return new Pattern(regex, 0);
		}

		/// <summary>
		/// Compiles the given regular expression into a pattern with the given
		/// flags.
		/// </summary>
		/// <param name="regex">
		///         The expression to be compiled
		/// </param>
		/// <param name="flags">
		///         Match flags, a bit mask that may include
		///         <seealso cref="#CASE_INSENSITIVE"/>, <seealso cref="#MULTILINE"/>, <seealso cref="#DOTALL"/>,
		///         <seealso cref="#UNICODE_CASE"/>, <seealso cref="#CANON_EQ"/>, <seealso cref="#UNIX_LINES"/>,
		///         <seealso cref="#LITERAL"/>, <seealso cref="#UNICODE_CHARACTER_CLASS"/>
		///         and <seealso cref="#COMMENTS"/>
		/// </param>
		/// <returns> the given regular expression compiled into a pattern with the given flags </returns>
		/// <exception cref="IllegalArgumentException">
		///          If bit values other than those corresponding to the defined
		///          match flags are set in <tt>flags</tt>
		/// </exception>
		/// <exception cref="PatternSyntaxException">
		///          If the expression's syntax is invalid </exception>
		public static Pattern Compile(String regex, int flags)
		{
			return new Pattern(regex, flags);
		}

		/// <summary>
		/// Returns the regular expression from which this pattern was compiled.
		/// </summary>
		/// <returns>  The source of this pattern </returns>
		public String Pattern()
		{
			return Pattern_Renamed;
		}

		/// <summary>
		/// <para>Returns the string representation of this pattern. This
		/// is the regular expression from which this pattern was
		/// compiled.</para>
		/// </summary>
		/// <returns>  The string representation of this pattern
		/// @since 1.5 </returns>
		public override String ToString()
		{
			return Pattern_Renamed;
		}

		/// <summary>
		/// Creates a matcher that will match the given input against this pattern.
		/// </summary>
		/// <param name="input">
		///         The character sequence to be matched
		/// </param>
		/// <returns>  A new matcher for this pattern </returns>
		public Matcher Matcher(CharSequence input)
		{
			if (!Compiled)
			{
				lock (this)
				{
					if (!Compiled)
					{
						Compile();
					}
				}
			}
			Matcher m = new Matcher(this, input);
			return m;
		}

		/// <summary>
		/// Returns this pattern's match flags.
		/// </summary>
		/// <returns>  The match flags specified when this pattern was compiled </returns>
		public int Flags()
		{
			return Flags_Renamed;
		}

		/// <summary>
		/// Compiles the given regular expression and attempts to match the given
		/// input against it.
		/// 
		/// <para> An invocation of this convenience method of the form
		/// 
		/// <blockquote><pre>
		/// Pattern.matches(regex, input);</pre></blockquote>
		/// 
		/// behaves in exactly the same way as the expression
		/// 
		/// <blockquote><pre>
		/// Pattern.compile(regex).matcher(input).matches()</pre></blockquote>
		/// 
		/// </para>
		/// <para> If a pattern is to be used multiple times, compiling it once and reusing
		/// it will be more efficient than invoking this method each time.  </para>
		/// </summary>
		/// <param name="regex">
		///         The expression to be compiled
		/// </param>
		/// <param name="input">
		///         The character sequence to be matched </param>
		/// <returns> whether or not the regular expression matches on the input </returns>
		/// <exception cref="PatternSyntaxException">
		///          If the expression's syntax is invalid </exception>
		public static bool Matches(String regex, CharSequence input)
		{
			Pattern p = Pattern.Compile(regex);
			Matcher m = p.Matcher(input);
			return m.Matches();
		}

		/// <summary>
		/// Splits the given input sequence around matches of this pattern.
		/// 
		/// <para> The array returned by this method contains each substring of the
		/// input sequence that is terminated by another subsequence that matches
		/// this pattern or is terminated by the end of the input sequence.  The
		/// substrings in the array are in the order in which they occur in the
		/// input. If this pattern does not match any subsequence of the input then
		/// the resulting array has just one element, namely the input sequence in
		/// string form.
		/// 
		/// </para>
		/// <para> When there is a positive-width match at the beginning of the input
		/// sequence then an empty leading substring is included at the beginning
		/// of the resulting array. A zero-width match at the beginning however
		/// never produces such empty leading substring.
		/// 
		/// </para>
		/// <para> The <tt>limit</tt> parameter controls the number of times the
		/// pattern is applied and therefore affects the length of the resulting
		/// array.  If the limit <i>n</i> is greater than zero then the pattern
		/// will be applied at most <i>n</i>&nbsp;-&nbsp;1 times, the array's
		/// length will be no greater than <i>n</i>, and the array's last entry
		/// will contain all input beyond the last matched delimiter.  If <i>n</i>
		/// is non-positive then the pattern will be applied as many times as
		/// possible and the array can have any length.  If <i>n</i> is zero then
		/// the pattern will be applied as many times as possible, the array can
		/// have any length, and trailing empty strings will be discarded.
		/// 
		/// </para>
		/// <para> The input <tt>"boo:and:foo"</tt>, for example, yields the following
		/// results with these parameters:
		/// 
		/// <blockquote><table cellpadding=1 cellspacing=0
		///              summary="Split examples showing regex, limit, and result">
		/// <tr><th align="left"><i>Regex&nbsp;&nbsp;&nbsp;&nbsp;</i></th>
		///     <th align="left"><i>Limit&nbsp;&nbsp;&nbsp;&nbsp;</i></th>
		///     <th align="left"><i>Result&nbsp;&nbsp;&nbsp;&nbsp;</i></th></tr>
		/// <tr><td align=center>:</td>
		///     <td align=center>2</td>
		///     <td><tt>{ "boo", "and:foo" }</tt></td></tr>
		/// <tr><td align=center>:</td>
		///     <td align=center>5</td>
		///     <td><tt>{ "boo", "and", "foo" }</tt></td></tr>
		/// <tr><td align=center>:</td>
		///     <td align=center>-2</td>
		///     <td><tt>{ "boo", "and", "foo" }</tt></td></tr>
		/// <tr><td align=center>o</td>
		///     <td align=center>5</td>
		///     <td><tt>{ "b", "", ":and:f", "", "" }</tt></td></tr>
		/// <tr><td align=center>o</td>
		///     <td align=center>-2</td>
		///     <td><tt>{ "b", "", ":and:f", "", "" }</tt></td></tr>
		/// <tr><td align=center>o</td>
		///     <td align=center>0</td>
		///     <td><tt>{ "b", "", ":and:f" }</tt></td></tr>
		/// </table></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="input">
		///         The character sequence to be split
		/// </param>
		/// <param name="limit">
		///         The result threshold, as described above
		/// </param>
		/// <returns>  The array of strings computed by splitting the input
		///          around matches of this pattern </returns>
		public String[] Split(CharSequence input, int limit)
		{
			int index = 0;
			bool matchLimited = limit > 0;
			List<String> matchList = new List<String>();
			Matcher m = Matcher(input);

			// Add segments before each match found
			while (m.Find())
			{
				if (!matchLimited || matchList.Size() < limit - 1)
				{
					if (index == 0 && index == m.Start() && m.Start() == m.End())
					{
						// no empty leading substring included for zero-width match
						// at the beginning of the input char sequence.
						continue;
					}
					String match = input.SubSequence(index, m.Start()).ToString();
					matchList.Add(match);
					index = m.End();
				} // last one
				else if (matchList.Size() == limit - 1)
				{
					String match = input.SubSequence(index, input.Length()).ToString();
					matchList.Add(match);
					index = m.End();
				}
			}

			// If no match was found, return this
			if (index == 0)
			{
				return new String[] {input.ToString()};
			}

			// Add remaining segment
			if (!matchLimited || matchList.Size() < limit)
			{
				matchList.Add(input.SubSequence(index, input.Length()).ToString());
			}

			// Construct result
			int resultSize = matchList.Size();
			if (limit == 0)
			{
				while (resultSize > 0 && matchList.Get(resultSize-1).Equals(""))
				{
					resultSize--;
				}
			}
			String[] result = new String[resultSize];
			return matchList.SubList(0, resultSize).ToArray(result);
		}

		/// <summary>
		/// Splits the given input sequence around matches of this pattern.
		/// 
		/// <para> This method works as if by invoking the two-argument {@link
		/// #split(java.lang.CharSequence, int) split} method with the given input
		/// sequence and a limit argument of zero.  Trailing empty strings are
		/// therefore not included in the resulting array. </para>
		/// 
		/// <para> The input <tt>"boo:and:foo"</tt>, for example, yields the following
		/// results with these expressions:
		/// 
		/// <blockquote><table cellpadding=1 cellspacing=0
		///              summary="Split examples showing regex and result">
		/// <tr><th align="left"><i>Regex&nbsp;&nbsp;&nbsp;&nbsp;</i></th>
		///     <th align="left"><i>Result</i></th></tr>
		/// <tr><td align=center>:</td>
		///     <td><tt>{ "boo", "and", "foo" }</tt></td></tr>
		/// <tr><td align=center>o</td>
		///     <td><tt>{ "b", "", ":and:f" }</tt></td></tr>
		/// </table></blockquote>
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="input">
		///         The character sequence to be split
		/// </param>
		/// <returns>  The array of strings computed by splitting the input
		///          around matches of this pattern </returns>
		public String[] Split(CharSequence input)
		{
			return Split(input, 0);
		}

		/// <summary>
		/// Returns a literal pattern <code>String</code> for the specified
		/// <code>String</code>.
		/// 
		/// <para>This method produces a <code>String</code> that can be used to
		/// create a <code>Pattern</code> that would match the string
		/// <code>s</code> as if it were a literal pattern.</para> Metacharacters
		/// or escape sequences in the input sequence will be given no special
		/// meaning.
		/// </summary>
		/// <param name="s"> The string to be literalized </param>
		/// <returns>  A literal string replacement
		/// @since 1.5 </returns>
		public static String Quote(String s)
		{
			int slashEIndex = s.IndexOf("\\E");
			if (slashEIndex == -1)
			{
				return "\\Q" + s + "\\E";
			}

			StringBuilder sb = new StringBuilder(s.Length() * 2);
			sb.Append("\\Q");
			slashEIndex = 0;
			int current = 0;
			while ((slashEIndex = s.IndexOf("\\E", current)) != -1)
			{
				sb.Append(s.Substring(current, slashEIndex - current));
				current = slashEIndex + 2;
				sb.Append("\\E\\\\E\\Q");
			}
			sb.Append(s.Substring(current, s.Length() - current));
			sb.Append("\\E");
			return sb.ToString();
		}

		/// <summary>
		/// Recompile the Pattern instance from a stream.  The original pattern
		/// string is read in and the object tree is recompiled from it.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{

			// Read in all fields
			s.DefaultReadObject();

			// Initialize counts
			CapturingGroupCount = 1;
			LocalCount = 0;

			// if length > 0, the Pattern is lazily compiled
			Compiled = false;
			if (Pattern_Renamed.Length() == 0)
			{
				Root = new Start(LastAccept);
				MatchRoot = LastAccept;
				Compiled = true;
			}
		}

		/// <summary>
		/// This private constructor is used to create all Patterns. The pattern
		/// string and match flags are all that is needed to completely describe
		/// a Pattern. An empty pattern string results in an object tree with
		/// only a Start node and a LastNode node.
		/// </summary>
		private Pattern(String p, int f)
		{
			Pattern_Renamed = p;
			Flags_Renamed = f;

			// to use UNICODE_CASE if UNICODE_CHARACTER_CLASS present
			if ((Flags_Renamed & UNICODE_CHARACTER_CLASS) != 0)
			{
				Flags_Renamed |= UNICODE_CASE;
			}

			// Reset group index count
			CapturingGroupCount = 1;
			LocalCount = 0;

			if (Pattern_Renamed.Length() > 0)
			{
				Compile();
			}
			else
			{
				Root = new Start(LastAccept);
				MatchRoot = LastAccept;
			}
		}

		/// <summary>
		/// The pattern is converted to normalizedD form and then a pure group
		/// is constructed to match canonical equivalences of the characters.
		/// </summary>
		private void Normalize()
		{
			bool inCharClass = false;
			int lastCodePoint = -1;

			// Convert pattern into normalizedD form
			NormalizedPattern = Normalizer.Normalize(Pattern_Renamed, Normalizer.Form.NFD);
			PatternLength = NormalizedPattern.Length();

			// Modify pattern to match canonical equivalences
			StringBuilder newPattern = new StringBuilder(PatternLength);
			for (int i = 0; i < PatternLength;)
			{
				int c = NormalizedPattern.CodePointAt(i);
				StringBuilder sequenceBuffer;
				if ((Character.GetType(c) == Character.NON_SPACING_MARK) && (lastCodePoint != -1))
				{
					sequenceBuffer = new StringBuilder();
					sequenceBuffer.AppendCodePoint(lastCodePoint);
					sequenceBuffer.AppendCodePoint(c);
					while (Character.GetType(c) == Character.NON_SPACING_MARK)
					{
						i += Character.CharCount(c);
						if (i >= PatternLength)
						{
							break;
						}
						c = NormalizedPattern.CodePointAt(i);
						sequenceBuffer.AppendCodePoint(c);
					}
					String ea = ProduceEquivalentAlternation(sequenceBuffer.ToString());
					newPattern.Length = newPattern.Length() - Character.CharCount(lastCodePoint);
					newPattern.Append("(?:").Append(ea).Append(")");
				}
				else if (c == '[' && lastCodePoint != '\\')
				{
					i = NormalizeCharClass(newPattern, i);
				}
				else
				{
					newPattern.AppendCodePoint(c);
				}
				lastCodePoint = c;
				i += Character.CharCount(c);
			}
			NormalizedPattern = newPattern.ToString();
		}

		/// <summary>
		/// Complete the character class being parsed and add a set
		/// of alternations to it that will match the canonical equivalences
		/// of the characters within the class.
		/// </summary>
		private int NormalizeCharClass(StringBuilder newPattern, int i)
		{
			StringBuilder charClass = new StringBuilder();
			StringBuilder eq = null;
			int lastCodePoint = -1;
			String result;

			i++;
			charClass.Append("[");
			while (true)
			{
				int c = NormalizedPattern.CodePointAt(i);
				StringBuilder sequenceBuffer;

				if (c == ']' && lastCodePoint != '\\')
				{
					charClass.Append((char)c);
					break;
				}
				else if (Character.GetType(c) == Character.NON_SPACING_MARK)
				{
					sequenceBuffer = new StringBuilder();
					sequenceBuffer.AppendCodePoint(lastCodePoint);
					while (Character.GetType(c) == Character.NON_SPACING_MARK)
					{
						sequenceBuffer.AppendCodePoint(c);
						i += Character.CharCount(c);
						if (i >= NormalizedPattern.Length())
						{
							break;
						}
						c = NormalizedPattern.CodePointAt(i);
					}
					String ea = ProduceEquivalentAlternation(sequenceBuffer.ToString());

					charClass.Length = charClass.Length() - Character.CharCount(lastCodePoint);
					if (eq == null)
					{
						eq = new StringBuilder();
					}
					eq.Append('|');
					eq.Append(ea);
				}
				else
				{
					charClass.AppendCodePoint(c);
					i++;
				}
				if (i == NormalizedPattern.Length())
				{
					throw Error("Unclosed character class");
				}
				lastCodePoint = c;
			}

			if (eq != null)
			{
				result = "(?:" + charClass.ToString() + eq.ToString() + ")";
			}
			else
			{
				result = charClass.ToString();
			}

			newPattern.Append(result);
			return i;
		}

		/// <summary>
		/// Given a specific sequence composed of a regular character and
		/// combining marks that follow it, produce the alternation that will
		/// match all canonical equivalences of that sequence.
		/// </summary>
		private String ProduceEquivalentAlternation(String source)
		{
			int len = CountChars(source, 0, 1);
			if (source.Length() == len)
			{
				// source has one character.
				return source;
			}

			String @base = source.Substring(0,len);
			String combiningMarks = source.Substring(len);

			String[] perms = ProducePermutations(combiningMarks);
			StringBuilder result = new StringBuilder(source);

			// Add combined permutations
			for (int x = 0; x < perms.Length; x++)
			{
				String next = @base + perms[x];
				if (x > 0)
				{
					result.Append("|" + next);
				}
				next = ComposeOneStep(next);
				if (next != null)
				{
					result.Append("|" + ProduceEquivalentAlternation(next));
				}
			}
			return result.ToString();
		}

		/// <summary>
		/// Returns an array of strings that have all the possible
		/// permutations of the characters in the input string.
		/// This is used to get a list of all possible orderings
		/// of a set of combining marks. Note that some of the permutations
		/// are invalid because of combining class collisions, and these
		/// possibilities must be removed because they are not canonically
		/// equivalent.
		/// </summary>
		private String[] ProducePermutations(String input)
		{
			if (input.Length() == CountChars(input, 0, 1))
			{
				return new String[] {input};
			}

			if (input.Length() == CountChars(input, 0, 2))
			{
				int c0 = Character.CodePointAt(input, 0);
				int c1 = Character.CodePointAt(input, Character.CharCount(c0));
				if (GetClass(c1) == GetClass(c0))
				{
					return new String[] {input};
				}
				String[] result = new String[2];
				result[0] = input;
				StringBuilder sb = new StringBuilder(2);
				sb.AppendCodePoint(c1);
				sb.AppendCodePoint(c0);
				result[1] = sb.ToString();
				return result;
			}

			int length = 1;
			int nCodePoints = CountCodePoints(input);
			for (int x = 1; x < nCodePoints; x++)
			{
				length = length * (x + 1);
			}

			String[] temp = new String[length];

			int[] combClass = new int[nCodePoints];
			for (int x = 0, i = 0; x < nCodePoints; x++)
			{
				int c = Character.CodePointAt(input, i);
				combClass[x] = GetClass(c);
				i += Character.CharCount(c);
			}

			// For each char, take it out and add the permutations
			// of the remaining chars
			int index = 0;
			int len;
			// offset maintains the index in code units.
	for (int x = 0, offset = 0; x < nCodePoints; x++, offset += len)
	{
				len = CountChars(input, offset, 1);
				bool skip = false;
				for (int y = x - 1; y >= 0; y--)
				{
					if (combClass[y] == combClass[x])
					{
						goto loopContinue;
					}
				}
				StringBuilder sb = new StringBuilder(input);
				String otherChars = sb.Delete(offset, offset + len).ToString();
				String[] subResult = ProducePermutations(otherChars);

				String prefix = input.Substring(offset, len);
				for (int y = 0; y < subResult.Length; y++)
				{
					temp[index++] = prefix + subResult[y];
				}
		loopContinue:;
	}
	loopBreak:
			String[] result = new String[index];
			for (int x = 0; x < index; x++)
			{
				result[x] = temp[x];
			}
			return result;
		}

		private int GetClass(int c)
		{
			return sun.text.Normalizer.getCombiningClass(c);
		}

		/// <summary>
		/// Attempts to compose input by combining the first character
		/// with the first combining mark following it. Returns a String
		/// that is the composition of the leading character with its first
		/// combining mark followed by the remaining combining marks. Returns
		/// null if the first two characters cannot be further composed.
		/// </summary>
		private String ComposeOneStep(String input)
		{
			int len = CountChars(input, 0, 2);
			String firstTwoCharacters = input.Substring(0, len);
			String result = Normalizer.Normalize(firstTwoCharacters, Normalizer.Form.NFC);

			if (result.Equals(firstTwoCharacters))
			{
				return null;
			}
			else
			{
				String remainder = input.Substring(len);
				return result + remainder;
			}
		}

		/// <summary>
		/// Preprocess any \Q...\E sequences in `temp', meta-quoting them.
		/// See the description of `quotemeta' in perlfunc(1).
		/// </summary>
		private void RemoveQEQuoting()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pLen = patternLength;
			int pLen = PatternLength;
			int i = 0;
			while (i < pLen - 1)
			{
				if (Temp[i] != '\\')
				{
					i += 1;
				}
				else if (Temp[i + 1] != 'Q')
				{
					i += 2;
				}
				else
				{
					break;
				}
			}
			if (i >= pLen - 1) // No \Q sequence found
			{
				return;
			}
			int j = i;
			i += 2;
			int[] newtemp = new int[j + 3 * (pLen - i) + 2];
			System.Array.Copy(Temp, 0, newtemp, 0, j);

			bool inQuote = true;
			bool beginQuote = true;
			while (i < pLen)
			{
				int c = Temp[i++];
				if (!ASCII.IsAscii(c) || ASCII.IsAlpha(c))
				{
					newtemp[j++] = c;
				}
				else if (ASCII.IsDigit(c))
				{
					if (beginQuote)
					{
						/*
						 * A unicode escape \[0xu] could be before this quote,
						 * and we don't want this numeric char to processed as
						 * part of the escape.
						 */
						newtemp[j++] = '\\';
						newtemp[j++] = 'x';
						newtemp[j++] = '3';
					}
					newtemp[j++] = c;
				}
				else if (c != '\\')
				{
					if (inQuote)
					{
						newtemp[j++] = '\\';
					}
					newtemp[j++] = c;
				}
				else if (inQuote)
				{
					if (Temp[i] == 'E')
					{
						i++;
						inQuote = false;
					}
					else
					{
						newtemp[j++] = '\\';
						newtemp[j++] = '\\';
					}
				}
				else
				{
					if (Temp[i] == 'Q')
					{
						i++;
						inQuote = true;
						beginQuote = true;
						continue;
					}
					else
					{
						newtemp[j++] = c;
						if (i != pLen)
						{
							newtemp[j++] = Temp[i++];
						}
					}
				}

				beginQuote = false;
			}

			PatternLength = j;
			Temp = Arrays.CopyOf(newtemp, j + 2); // double zero termination
		}

		/// <summary>
		/// Copies regular expression to an int array and invokes the parsing
		/// of the expression which will create the object tree.
		/// </summary>
		private void Compile()
		{
			// Handle canonical equivalences
			if (Has(CANON_EQ) && !Has(LITERAL))
			{
				Normalize();
			}
			else
			{
				NormalizedPattern = Pattern_Renamed;
			}
			PatternLength = NormalizedPattern.Length();

			// Copy pattern to int array for convenience
			// Use double zero to terminate pattern
			Temp = new int[PatternLength + 2];

			HasSupplementary = false;
			int c , count = 0;
			// Convert all chars into code points
			for (int x = 0; x < PatternLength; x += Character.CharCount(c))
			{
				c = NormalizedPattern.CodePointAt(x);
				if (IsSupplementary(c))
				{
					HasSupplementary = true;
				}
				Temp[count++] = c;
			}

			PatternLength = count; // patternLength now in code points

			if (!Has(LITERAL))
			{
				RemoveQEQuoting();
			}

			// Allocate all temporary objects here.
			Buffer = new int[32];
			GroupNodes = new GroupHead[10];
			NamedGroups_Renamed = null;

			if (Has(LITERAL))
			{
				// Literal pattern handling
				MatchRoot = NewSlice(Temp, PatternLength, HasSupplementary);
				MatchRoot.Next = LastAccept;
			}
			else
			{
				// Start recursive descent parsing
				MatchRoot = Expr(LastAccept);
				// Check extra pattern characters
				if (PatternLength != Cursor_Renamed)
				{
					if (Peek() == ')')
					{
						throw Error("Unmatched closing ')'");
					}
					else
					{
						throw Error("Unexpected internal error");
					}
				}
			}

			// Peephole optimization
			if (MatchRoot is Slice)
			{
				Root = BnM.Optimize(MatchRoot);
				if (Root == MatchRoot)
				{
					Root = HasSupplementary ? new StartS(MatchRoot) : new Start(MatchRoot);
				}
			}
			else if (MatchRoot is Begin || MatchRoot is First)
			{
				Root = MatchRoot;
			}
			else
			{
				Root = HasSupplementary ? new StartS(MatchRoot) : new Start(MatchRoot);
			}

			// Release temporary storage
			Temp = null;
			Buffer = null;
			GroupNodes = null;
			PatternLength = 0;
			Compiled = true;
		}

		internal IDictionary<String, Integer> NamedGroups()
		{
			if (NamedGroups_Renamed == null)
			{
				NamedGroups_Renamed = new Dictionary<>(2);
			}
			return NamedGroups_Renamed;
		}

		/// <summary>
		/// Used to print out a subtree of the Pattern to help with debugging.
		/// </summary>
		private static void PrintObjectTree(Node node)
		{
			while (node != null)
			{
				if (node is Prolog)
				{
					System.Console.WriteLine(node);
					PrintObjectTree(((Prolog)node).Loop);
					System.Console.WriteLine("**** end contents prolog loop");
				}
				else if (node is Loop)
				{
					System.Console.WriteLine(node);
					PrintObjectTree(((Loop)node).Body);
					System.Console.WriteLine("**** end contents Loop body");
				}
				else if (node is Curly)
				{
					System.Console.WriteLine(node);
					PrintObjectTree(((Curly)node).Atom);
					System.Console.WriteLine("**** end contents Curly body");
				}
				else if (node is GroupCurly)
				{
					System.Console.WriteLine(node);
					PrintObjectTree(((GroupCurly)node).Atom);
					System.Console.WriteLine("**** end contents GroupCurly body");
				}
				else if (node is GroupTail)
				{
					System.Console.WriteLine(node);
					System.Console.WriteLine("Tail next is " + node.Next);
					return;
				}
				else
				{
					System.Console.WriteLine(node);
				}
				node = node.Next;
				if (node != null)
				{
					System.Console.WriteLine("->next:");
				}
				if (node == Pattern.Accept_Renamed)
				{
					System.Console.WriteLine("Accept Node");
					node = null;
				}
			}
		}

		/// <summary>
		/// Used to accumulate information about a subtree of the object graph
		/// so that optimizations can be applied to the subtree.
		/// </summary>
		internal sealed class TreeInfo
		{
			internal int MinLength;
			internal int MaxLength;
			internal bool MaxValid;
			internal bool Deterministic;

			internal TreeInfo()
			{
				Reset();
			}
			internal void Reset()
			{
				MinLength = 0;
				MaxLength = 0;
				MaxValid = true;
				Deterministic = true;
			}
		}

		/*
		 * The following private methods are mainly used to improve the
		 * readability of the code. In order to let the Java compiler easily
		 * inline them, we should not put many assertions or error checks in them.
		 */

		/// <summary>
		/// Indicates whether a particular flag is set or not.
		/// </summary>
		private bool Has(int f)
		{
			return (Flags_Renamed & f) != 0;
		}

		/// <summary>
		/// Match next character, signal error if failed.
		/// </summary>
		private void Accept(int ch, String s)
		{
			int testChar = Temp[Cursor_Renamed++];
			if (Has(COMMENTS))
			{
				testChar = ParsePastWhitespace(testChar);
			}
			if (ch != testChar)
			{
				throw Error(s);
			}
		}

		/// <summary>
		/// Mark the end of pattern with a specific character.
		/// </summary>
		private void Mark(int c)
		{
			Temp[PatternLength] = c;
		}

		/// <summary>
		/// Peek the next character, and do not advance the cursor.
		/// </summary>
		private int Peek()
		{
			int ch = Temp[Cursor_Renamed];
			if (Has(COMMENTS))
			{
				ch = PeekPastWhitespace(ch);
			}
			return ch;
		}

		/// <summary>
		/// Read the next character, and advance the cursor by one.
		/// </summary>
		private int Read()
		{
			int ch = Temp[Cursor_Renamed++];
			if (Has(COMMENTS))
			{
				ch = ParsePastWhitespace(ch);
			}
			return ch;
		}

		/// <summary>
		/// Read the next character, and advance the cursor by one,
		/// ignoring the COMMENTS setting
		/// </summary>
		private int ReadEscaped()
		{
			int ch = Temp[Cursor_Renamed++];
			return ch;
		}

		/// <summary>
		/// Advance the cursor by one, and peek the next character.
		/// </summary>
		private int Next()
		{
			int ch = Temp[++Cursor_Renamed];
			if (Has(COMMENTS))
			{
				ch = PeekPastWhitespace(ch);
			}
			return ch;
		}

		/// <summary>
		/// Advance the cursor by one, and peek the next character,
		/// ignoring the COMMENTS setting
		/// </summary>
		private int NextEscaped()
		{
			int ch = Temp[++Cursor_Renamed];
			return ch;
		}

		/// <summary>
		/// If in xmode peek past whitespace and comments.
		/// </summary>
		private int PeekPastWhitespace(int ch)
		{
			while (ASCII.IsSpace(ch) || ch == '#')
			{
				while (ASCII.IsSpace(ch))
				{
					ch = Temp[++Cursor_Renamed];
				}
				if (ch == '#')
				{
					ch = PeekPastLine();
				}
			}
			return ch;
		}

		/// <summary>
		/// If in xmode parse past whitespace and comments.
		/// </summary>
		private int ParsePastWhitespace(int ch)
		{
			while (ASCII.IsSpace(ch) || ch == '#')
			{
				while (ASCII.IsSpace(ch))
				{
					ch = Temp[Cursor_Renamed++];
				}
				if (ch == '#')
				{
					ch = ParsePastLine();
				}
			}
			return ch;
		}

		/// <summary>
		/// xmode parse past comment to end of line.
		/// </summary>
		private int ParsePastLine()
		{
			int ch = Temp[Cursor_Renamed++];
			while (ch != 0 && !IsLineSeparator(ch))
			{
				ch = Temp[Cursor_Renamed++];
			}
			return ch;
		}

		/// <summary>
		/// xmode peek past comment to end of line.
		/// </summary>
		private int PeekPastLine()
		{
			int ch = Temp[++Cursor_Renamed];
			while (ch != 0 && !IsLineSeparator(ch))
			{
				ch = Temp[++Cursor_Renamed];
			}
			return ch;
		}

		/// <summary>
		/// Determines if character is a line separator in the current mode
		/// </summary>
		private bool IsLineSeparator(int ch)
		{
			if (Has(UNIX_LINES))
			{
				return ch == '\n';
			}
			else
			{
				return (ch == '\n' || ch == '\r' || (ch | 1) == '\u2029' || ch == '\u0085');
			}
		}

		/// <summary>
		/// Read the character after the next one, and advance the cursor by two.
		/// </summary>
		private int Skip()
		{
			int i = Cursor_Renamed;
			int ch = Temp[i + 1];
			Cursor_Renamed = i + 2;
			return ch;
		}

		/// <summary>
		/// Unread one next character, and retreat cursor by one.
		/// </summary>
		private void Unread()
		{
			Cursor_Renamed--;
		}

		/// <summary>
		/// Internal method used for handling all syntax errors. The pattern is
		/// displayed with a pointer to aid in locating the syntax error.
		/// </summary>
		private PatternSyntaxException Error(String s)
		{
			return new PatternSyntaxException(s, NormalizedPattern, Cursor_Renamed - 1);
		}

		/// <summary>
		/// Determines if there is any supplementary character or unpaired
		/// surrogate in the specified range.
		/// </summary>
		private bool FindSupplementary(int start, int end)
		{
			for (int i = start; i < end; i++)
			{
				if (IsSupplementary(Temp[i]))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines if the specified code point is a supplementary
		/// character or unpaired surrogate.
		/// </summary>
		private static bool IsSupplementary(int ch)
		{
			return ch >= Character.MIN_SUPPLEMENTARY_CODE_POINT || Character.IsSurrogate((char)ch);
		}

		/// <summary>
		///  The following methods handle the main parsing. They are sorted
		///  according to their precedence order, the lowest one first.
		/// </summary>

		/// <summary>
		/// The expression is parsed with branch nodes added for alternations.
		/// This may be called recursively to parse sub expressions that may
		/// contain alternations.
		/// </summary>
		private Node Expr(Node end)
		{
			Node prev = null;
			Node firstTail = null;
			Branch branch = null;
			Node branchConn = null;

			for (;;)
			{
				Node node = Sequence(end);
				Node nodeTail = Root; //double return
				if (prev == null)
				{
					prev = node;
					firstTail = nodeTail;
				}
				else
				{
					// Branch
					if (branchConn == null)
					{
						branchConn = new BranchConn();
						branchConn.Next = end;
					}
					if (node == end)
					{
						// if the node returned from sequence() is "end"
						// we have an empty expr, set a null atom into
						// the branch to indicate to go "next" directly.
						node = null;
					}
					else
					{
						// the "tail.next" of each atom goes to branchConn
						nodeTail.Next = branchConn;
					}
					if (prev == branch)
					{
						branch.Add(node);
					}
					else
					{
						if (prev == end)
						{
							prev = null;
						}
						else
						{
							// replace the "end" with "branchConn" at its tail.next
							// when put the "prev" into the branch as the first atom.
							firstTail.Next = branchConn;
						}
						prev = branch = new Branch(prev, node, branchConn);
					}
				}
				if (Peek() != '|')
				{
					return prev;
				}
				Next();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") private Node sequence(Node end)
		private Node Sequence(Node end)
		/// <summary>
		/// Parsing of sequences between alternations.
		/// </summary>
		{
			Node head = null;
			Node tail = null;
			Node node = null;
			for (;;)
			{
				int ch = Peek();
				switch (ch)
				{
				case '(':
					// Because group handles its own closure,
					// we need to treat it differently
					node = Group0();
					// Check for comment or flag group
					if (node == null)
					{
						continue;
					}
					if (head == null)
					{
						head = node;
					}
					else
					{
						tail.Next = node;
					}
					// Double return: Tail was returned in root
					tail = Root;
					continue;
				case '[':
					node = Clazz(true);
					break;
				case '\\':
					ch = NextEscaped();
					if (ch == 'p' || ch == 'P')
					{
						bool oneLetter = true;
						bool comp = (ch == 'P');
						ch = Next(); // Consume { if present
						if (ch != '{')
						{
							Unread();
						}
						else
						{
							oneLetter = false;
						}
						node = Family(oneLetter, comp);
					}
					else
					{
						Unread();
						node = Atom();
					}
					break;
				case '^':
					Next();
					if (Has(MULTILINE))
					{
						if (Has(UNIX_LINES))
						{
							node = new UnixCaret();
						}
						else
						{
							node = new Caret();
						}
					}
					else
					{
						node = new Begin();
					}
					break;
				case '$':
					Next();
					if (Has(UNIX_LINES))
					{
						node = new UnixDollar(Has(MULTILINE));
					}
					else
					{
						node = new Dollar(Has(MULTILINE));
					}
					break;
				case '.':
					Next();
					if (Has(DOTALL))
					{
						node = new All();
					}
					else
					{
						if (Has(UNIX_LINES))
						{
							node = new UnixDot();
						}
						else
						{
							node = new Dot();
						}
					}
					break;
				case '|':
				case ')':
					goto LOOPBreak;
				case ']': // Now interpreting dangling ] and } as literals
				case '}':
					node = Atom();
					break;
				case '?':
				case '*':
				case '+':
					Next();
					throw Error("Dangling meta character '" + ((char)ch) + "'");
				case 0:
					if (Cursor_Renamed >= PatternLength)
					{
						goto LOOPBreak;
					}
					// Fall through
				default:
					node = Atom();
					break;
				}

				node = Closure(node);

				if (head == null)
				{
					head = tail = node;
				}
				else
				{
					tail.Next = node;
					tail = node;
				}
			LOOPContinue:;
			}
		LOOPBreak:
			if (head == null)
			{
				return end;
			}
			tail.Next = end;
			Root = tail; //double return
			return head;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") private Node atom()
		private Node Atom()
		/// <summary>
		/// Parse and add a new Single or Slice.
		/// </summary>
		{
			int first = 0;
			int prev = -1;
			bool hasSupplementary = false;
			int ch = Peek();
			for (;;)
			{
				switch (ch)
				{
				case '*':
				case '+':
				case '?':
				case '{':
					if (first > 1)
					{
						Cursor_Renamed = prev; // Unwind one character
						first--;
					}
					break;
				case '$':
				case '.':
				case '^':
				case '(':
				case '[':
				case '|':
				case ')':
					break;
				case '\\':
					ch = NextEscaped();
					if (ch == 'p' || ch == 'P') // Property
					{
						if (first > 0) // Slice is waiting; handle it first
						{
							Unread();
							break;
						} // No slice; just return the family node
						else
						{
							bool comp = (ch == 'P');
							bool oneLetter = true;
							ch = Next(); // Consume { if present
							if (ch != '{')
							{
								Unread();
							}
							else
							{
								oneLetter = false;
							}
							return Family(oneLetter, comp);
						}
					}
					Unread();
					prev = Cursor_Renamed;
					ch = Escape(false, first == 0, false);
					if (ch >= 0)
					{
						Append(ch, first);
						first++;
						if (IsSupplementary(ch))
						{
							hasSupplementary = true;
						}
						ch = Peek();
						continue;
					}
					else if (first == 0)
					{
						return Root;
					}
					// Unwind meta escape sequence
					Cursor_Renamed = prev;
					break;
				case 0:
					if (Cursor_Renamed >= PatternLength)
					{
						break;
					}
					// Fall through
				default:
					prev = Cursor_Renamed;
					Append(ch, first);
					first++;
					if (IsSupplementary(ch))
					{
						hasSupplementary = true;
					}
					ch = Next();
					continue;
				}
				break;
			}
			if (first == 1)
			{
				return NewSingle(Buffer[0]);
			}
			else
			{
				return NewSlice(Buffer, first, hasSupplementary);
			}
		}

		private void Append(int ch, int len)
		{
			if (len >= Buffer.Length)
			{
				int[] tmp = new int[len + len];
				System.Array.Copy(Buffer, 0, tmp, 0, len);
				Buffer = tmp;
			}
			Buffer[len] = ch;
		}

		/// <summary>
		/// Parses a backref greedily, taking as many numbers as it
		/// can. The first digit is always treated as a backref, but
		/// multi digit numbers are only treated as a backref if at
		/// least that many backrefs exist at this point in the regex.
		/// </summary>
		private Node @ref(int refNum)
		{
			bool done = false;
			while (!done)
			{
				int ch = Peek();
				switch (ch)
				{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					int newRefNum = (refNum * 10) + (ch - '0');
					// Add another number if it doesn't make a group
					// that doesn't exist
					if (CapturingGroupCount - 1 < newRefNum)
					{
						done = true;
						break;
					}
					refNum = newRefNum;
					Read();
					break;
				default:
					done = true;
					break;
				}
			}
			if (Has(CASE_INSENSITIVE))
			{
				return new CIBackRef(refNum, Has(UNICODE_CASE));
			}
			else
			{
				return new BackRef(refNum);
			}
		}

		/// <summary>
		/// Parses an escape sequence to determine the actual value that needs
		/// to be matched.
		/// If -1 is returned and create was true a new object was added to the tree
		/// to handle the escape sequence.
		/// If the returned value is greater than zero, it is the value that
		/// matches the escape sequence.
		/// </summary>
		private int Escape(bool inclass, bool create, bool isrange)
		{
			int ch = Skip();
			switch (ch)
			{
			case '0':
				return o();
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				if (inclass)
				{
					break;
				}
				if (create)
				{
					Root = @ref((ch - '0'));
				}
				return -1;
			case 'A':
				if (inclass)
				{
					break;
				}
				if (create)
				{
					Root = new Begin();
				}
				return -1;
			case 'B':
				if (inclass)
				{
					break;
				}
				if (create)
				{
					Root = new Bound(Bound.NONE, Has(UNICODE_CHARACTER_CLASS));
				}
				return -1;
			case 'C':
				break;
			case 'D':
				if (create)
				{
					Root = Has(UNICODE_CHARACTER_CLASS) ? (new Utype(UnicodeProp.DIGIT)).Complement() : (new Ctype(ASCII.DIGIT)).Complement();
				}
				return -1;
			case 'E':
			case 'F':
				break;
			case 'G':
				if (inclass)
				{
					break;
				}
				if (create)
				{
					Root = new LastMatch();
				}
				return -1;
			case 'H':
				if (create)
				{
					Root = (new HorizWS()).Complement();
				}
				return -1;
			case 'I':
			case 'J':
			case 'K':
			case 'L':
			case 'M':
			case 'N':
			case 'O':
			case 'P':
			case 'Q':
				break;
			case 'R':
				if (inclass)
				{
					break;
				}
				if (create)
				{
					Root = new LineEnding();
				}
				return -1;
			case 'S':
				if (create)
				{
					Root = Has(UNICODE_CHARACTER_CLASS) ? (new Utype(UnicodeProp.WHITE_SPACE)).Complement() : (new Ctype(ASCII.SPACE)).Complement();
				}
				return -1;
			case 'T':
			case 'U':
				break;
			case 'V':
				if (create)
				{
					Root = (new VertWS()).Complement();
				}
				return -1;
			case 'W':
				if (create)
				{
					Root = Has(UNICODE_CHARACTER_CLASS) ? (new Utype(UnicodeProp.WORD)).Complement() : (new Ctype(ASCII.WORD)).Complement();
				}
				return -1;
			case 'X':
			case 'Y':
				break;
			case 'Z':
				if (inclass)
				{
					break;
				}
				if (create)
				{
					if (Has(UNIX_LINES))
					{
						Root = new UnixDollar(false);
					}
					else
					{
						Root = new Dollar(false);
					}
				}
				return -1;
			case 'a':
				return '\x0007';
			case 'b':
				if (inclass)
				{
					break;
				}
				if (create)
				{
					Root = new Bound(Bound.BOTH, Has(UNICODE_CHARACTER_CLASS));
				}
				return -1;
			case 'c':
				return c();
			case 'd':
				if (create)
				{
					Root = Has(UNICODE_CHARACTER_CLASS) ? new Utype(UnicodeProp.DIGIT) : new Ctype(ASCII.DIGIT);
				}
				return -1;
			case 'e':
				return '\x001B';
			case 'f':
				return '\f';
			case 'g':
				break;
			case 'h':
				if (create)
				{
					Root = new HorizWS();
				}
				return -1;
			case 'i':
			case 'j':
				break;
			case 'k':
				if (inclass)
				{
					break;
				}
				if (Read() != '<')
				{
					throw Error("\\k is not followed by '<' for named capturing group");
				}
				String name = Groupname(Read());
				if (!NamedGroups().ContainsKey(name))
				{
					throw Error("(named capturing group <" + name + "> does not exit");
				}
				if (create)
				{
					if (Has(CASE_INSENSITIVE))
					{
						Root = new CIBackRef(NamedGroups()[name], Has(UNICODE_CASE));
					}
					else
					{
						Root = new BackRef(NamedGroups()[name]);
					}
				}
				return -1;
			case 'l':
			case 'm':
				break;
			case 'n':
				return '\n';
			case 'o':
			case 'p':
			case 'q':
				break;
			case 'r':
				return '\r';
			case 's':
				if (create)
				{
					Root = Has(UNICODE_CHARACTER_CLASS) ? new Utype(UnicodeProp.WHITE_SPACE) : new Ctype(ASCII.SPACE);
				}
				return -1;
			case 't':
				return '\t';
			case 'u':
				return u();
			case 'v':
				// '\v' was implemented as VT/0x0B in releases < 1.8 (though
				// undocumented). In JDK8 '\v' is specified as a predefined
				// character class for all vertical whitespace characters.
				// So [-1, root=VertWS node] pair is returned (instead of a
				// single 0x0B). This breaks the range if '\v' is used as
				// the start or end value, such as [\v-...] or [...-\v], in
				// which a single definite value (0x0B) is expected. For
				// compatibility concern '\013'/0x0B is returned if isrange.
				if (isrange)
				{
					return '\x000B';
				}
				if (create)
				{
					Root = new VertWS();
				}
				return -1;
			case 'w':
				if (create)
				{
					Root = Has(UNICODE_CHARACTER_CLASS) ? new Utype(UnicodeProp.WORD) : new Ctype(ASCII.WORD);
				}
				return -1;
			case 'x':
				return x();
			case 'y':
				break;
			case 'z':
				if (inclass)
				{
					break;
				}
				if (create)
				{
					Root = new End();
				}
				return -1;
			default:
				return ch;
			}
			throw Error("Illegal/unsupported escape sequence");
		}

		/// <summary>
		/// Parse a character class, and return the node that matches it.
		/// 
		/// Consumes a ] on the way out if consume is true. Usually consume
		/// is true except for the case of [abc&&def] where def is a separate
		/// right hand node with "understood" brackets.
		/// </summary>
		private CharProperty Clazz(bool consume)
		{
			CharProperty prev = null;
			CharProperty node = null;
			BitClass bits = new BitClass();
			bool include = true;
			bool firstInClass = true;
			int ch = Next();
			for (;;)
			{
				switch (ch)
				{
					case '^':
						// Negates if first char in a class, otherwise literal
						if (firstInClass)
						{
							if (Temp[Cursor_Renamed - 1] != '[')
							{
								break;
							}
							ch = Next();
							include = !include;
							continue;
						}
						else
						{
							// ^ not first in class, treat as literal
							break;
						}
					case '[':
						firstInClass = false;
						node = Clazz(true);
						if (prev == null)
						{
							prev = node;
						}
						else
						{
							prev = Union(prev, node);
						}
						ch = Peek();
						continue;
					case '&':
						firstInClass = false;
						ch = Next();
						if (ch == '&')
						{
							ch = Next();
							CharProperty rightNode = null;
							while (ch != ']' && ch != '&')
							{
								if (ch == '[')
								{
									if (rightNode == null)
									{
										rightNode = Clazz(true);
									}
									else
									{
										rightNode = Union(rightNode, Clazz(true));
									}
								} // abc&&def
								else
								{
									Unread();
									rightNode = Clazz(false);
								}
								ch = Peek();
							}
							if (rightNode != null)
							{
								node = rightNode;
							}
							if (prev == null)
							{
								if (rightNode == null)
								{
									throw Error("Bad class syntax");
								}
								else
								{
									prev = rightNode;
								}
							}
							else
							{
								prev = Intersection(prev, node);
							}
						}
						else
						{
							// treat as a literal &
							Unread();
							break;
						}
						continue;
					case 0:
						firstInClass = false;
						if (Cursor_Renamed >= PatternLength)
						{
							throw Error("Unclosed character class");
						}
						break;
					case ']':
						firstInClass = false;
						if (prev != null)
						{
							if (consume)
							{
								Next();
							}
							return prev;
						}
						break;
					default:
						firstInClass = false;
						break;
				}
				node = Range(bits);
				if (include)
				{
					if (prev == null)
					{
						prev = node;
					}
					else
					{
						if (prev != node)
						{
							prev = Union(prev, node);
						}
					}
				}
				else
				{
					if (prev == null)
					{
						prev = node.Complement();
					}
					else
					{
						if (prev != node)
						{
							prev = SetDifference(prev, node);
						}
					}
				}
				ch = Peek();
			}
		}

		private CharProperty BitsOrSingle(BitClass bits, int ch)
		{
			/* Bits can only handle codepoints in [u+0000-u+00ff] range.
			   Use "single" node instead of bits when dealing with unicode
			   case folding for codepoints listed below.
			   (1)Uppercase out of range: u+00ff, u+00b5
			      toUpperCase(u+00ff) -> u+0178
			      toUpperCase(u+00b5) -> u+039c
			   (2)LatinSmallLetterLongS u+17f
			      toUpperCase(u+017f) -> u+0053
			   (3)LatinSmallLetterDotlessI u+131
			      toUpperCase(u+0131) -> u+0049
			   (4)LatinCapitalLetterIWithDotAbove u+0130
			      toLowerCase(u+0130) -> u+0069
			   (5)KelvinSign u+212a
			      toLowerCase(u+212a) ==> u+006B
			   (6)AngstromSign u+212b
			      toLowerCase(u+212b) ==> u+00e5
			*/
			int d;
			if (ch < 256 && !(Has(CASE_INSENSITIVE) && Has(UNICODE_CASE) && (ch == 0xff || ch == 0xb5 || ch == 0x49 || ch == 0x69 || ch == 0x53 || ch == 0x73 || ch == 0x4b || ch == 0x6b || ch == 0xc5 || ch == 0xe5))) //A+ring - K and k - S and s - I and i
			{
				return bits.Add(ch, Flags());
			}
			return NewSingle(ch);
		}

		/// <summary>
		/// Parse a single character or a character range in a character class
		/// and return its representative node.
		/// </summary>
		private CharProperty Range(BitClass bits)
		{
			int ch = Peek();
			if (ch == '\\')
			{
				ch = NextEscaped();
				if (ch == 'p' || ch == 'P') // A property
				{
					bool comp = (ch == 'P');
					bool oneLetter = true;
					// Consume { if present
					ch = Next();
					if (ch != '{')
					{
						Unread();
					}
					else
					{
						oneLetter = false;
					}
					return Family(oneLetter, comp);
				} // ordinary escape
				else
				{
					bool isrange = Temp[Cursor_Renamed + 1] == '-';
					Unread();
					ch = Escape(true, true, isrange);
					if (ch == -1)
					{
						return (CharProperty) Root;
					}
				}
			}
			else
			{
				Next();
			}
			if (ch >= 0)
			{
				if (Peek() == '-')
				{
					int endRange = Temp[Cursor_Renamed + 1];
					if (endRange == '[')
					{
						return BitsOrSingle(bits, ch);
					}
					if (endRange != ']')
					{
						Next();
						int m = Peek();
						if (m == '\\')
						{
							m = Escape(true, false, true);
						}
						else
						{
							Next();
						}
						if (m < ch)
						{
							throw Error("Illegal character range");
						}
						if (Has(CASE_INSENSITIVE))
						{
							return CaseInsensitiveRangeFor(ch, m);
						}
						else
						{
							return RangeFor(ch, m);
						}
					}
				}
				return BitsOrSingle(bits, ch);
			}
			throw Error("Unexpected character '" + ((char)ch) + "'");
		}

		/// <summary>
		/// Parses a Unicode character family and returns its representative node.
		/// </summary>
		private CharProperty Family(bool singleLetter, bool maybeComplement)
		{
			Next();
			String name;
			CharProperty node = null;

			if (singleLetter)
			{
				int c = Temp[Cursor_Renamed];
				if (!Character.IsSupplementaryCodePoint(c))
				{
					name = Convert.ToString((char)c);
				}
				else
				{
					name = new String(Temp, Cursor_Renamed, 1);
				}
				Read();
			}
			else
			{
				int i = Cursor_Renamed;
				Mark('}');
				while (Read() != '}')
				{
				}
				Mark('\x0000');
				int j = Cursor_Renamed;
				if (j > PatternLength)
				{
					throw Error("Unclosed character family");
				}
				if (i + 1 >= j)
				{
					throw Error("Empty character family");
				}
				name = new String(Temp, i, j - i - 1);
			}

			int i = name.IndexOf('=');
			if (i != -1)
			{
				// property construct \p{name=value}
				String value = name.Substring(i + 1);
				name = name.Substring(0, i).ToLowerCase(Locale.ENGLISH);
				if ("sc".Equals(name) || "script".Equals(name))
				{
					node = UnicodeScriptPropertyFor(value);
				}
				else if ("blk".Equals(name) || "block".Equals(name))
				{
					node = UnicodeBlockPropertyFor(value);
				}
				else if ("gc".Equals(name) || "general_category".Equals(name))
				{
					node = CharPropertyNodeFor(value);
				}
				else
				{
					throw Error("Unknown Unicode property {name=<" + name + ">, " + "value=<" + value + ">}");
				}
			}
			else
			{
				if (name.StartsWith("In"))
				{
					// \p{inBlockName}
					node = UnicodeBlockPropertyFor(name.Substring(2));
				}
				else if (name.StartsWith("Is"))
				{
					// \p{isGeneralCategory} and \p{isScriptName}
					name = name.Substring(2);
					UnicodeProp uprop = UnicodeProp.forName(name);
					if (uprop != null)
					{
						node = new Utype(uprop);
					}
					if (node == null)
					{
						node = CharPropertyNames.CharPropertyFor(name);
					}
					if (node == null)
					{
						node = UnicodeScriptPropertyFor(name);
					}
				}
				else
				{
					if (Has(UNICODE_CHARACTER_CLASS))
					{
						UnicodeProp uprop = UnicodeProp.forPOSIXName(name);
						if (uprop != null)
						{
							node = new Utype(uprop);
						}
					}
					if (node == null)
					{
						node = CharPropertyNodeFor(name);
					}
				}
			}
			if (maybeComplement)
			{
				if (node is Category || node is Block)
				{
					HasSupplementary = true;
				}
				node = node.Complement();
			}
			return node;
		}


		/// <summary>
		/// Returns a CharProperty matching all characters belong to
		/// a UnicodeScript.
		/// </summary>
		private CharProperty UnicodeScriptPropertyFor(String name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Character.UnicodeScript script;
			Character.UnicodeScript script;
			try
			{
				script = Character.UnicodeScript.forName(name);
			}
			catch (IllegalArgumentException)
			{
				throw Error("Unknown character script name {" + name + "}");
			}
			return new Script(script);
		}

		/// <summary>
		/// Returns a CharProperty matching all characters in a UnicodeBlock.
		/// </summary>
		private CharProperty UnicodeBlockPropertyFor(String name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Character.UnicodeBlock block;
			Character.UnicodeBlock block;
			try
			{
				block = Character.UnicodeBlock.ForName(name);
			}
			catch (IllegalArgumentException)
			{
				throw Error("Unknown character block name {" + name + "}");
			}
			return new Block(block);
		}

		/// <summary>
		/// Returns a CharProperty matching all characters in a named property.
		/// </summary>
		private CharProperty CharPropertyNodeFor(String name)
		{
			CharProperty p = CharPropertyNames.CharPropertyFor(name);
			if (p == null)
			{
				throw Error("Unknown character property name {" + name + "}");
			}
			return p;
		}

		/// <summary>
		/// Parses and returns the name of a "named capturing group", the trailing
		/// ">" is consumed after parsing.
		/// </summary>
		private String Groupname(int ch)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Character.ToChars(ch));
			while (ASCII.IsLower(ch = Read()) || ASCII.IsUpper(ch) || ASCII.IsDigit(ch))
			{
				sb.Append(Character.ToChars(ch));
			}
			if (sb.Length() == 0)
			{
				throw Error("named capturing group has 0 length name");
			}
			if (ch != '>')
			{
				throw Error("named capturing group is missing trailing '>'");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Parses a group and returns the head node of a set of nodes that process
		/// the group. Sometimes a double return system is used where the tail is
		/// returned in root.
		/// </summary>
		private Node Group0()
		{
			bool capturingGroup = false;
			Node head = null;
			Node tail = null;
			int save = Flags_Renamed;
			Root = null;
			int ch = Next();
			if (ch == '?')
			{
				ch = Skip();
				switch (ch)
				{
				case ':': //  (?:xxx) pure group
					head = CreateGroup(true);
					tail = Root;
					head.Next = Expr(tail);
					break;
				case '=': // (?=xxx) and (?!xxx) lookahead
				case '!':
					head = CreateGroup(true);
					tail = Root;
					head.Next = Expr(tail);
					if (ch == '=')
					{
						head = tail = new Pos(head);
					}
					else
					{
						head = tail = new Neg(head);
					}
					break;
				case '>': // (?>xxx)  independent group
					head = CreateGroup(true);
					tail = Root;
					head.Next = Expr(tail);
					head = tail = new Ques(head, INDEPENDENT);
					break;
				case '<': // (?<xxx)  look behind
					ch = Read();
					if (ASCII.IsLower(ch) || ASCII.IsUpper(ch))
					{
						// named captured group
						String name = Groupname(ch);
						if (NamedGroups().ContainsKey(name))
						{
							throw Error("Named capturing group <" + name + "> is already defined");
						}
						capturingGroup = true;
						head = CreateGroup(false);
						tail = Root;
						NamedGroups()[name] = CapturingGroupCount - 1;
						head.Next = Expr(tail);
						break;
					}
					int start = Cursor_Renamed;
					head = CreateGroup(true);
					tail = Root;
					head.Next = Expr(tail);
					tail.Next = lookbehindEnd;
					TreeInfo info = new TreeInfo();
					head.Study(info);
					if (info.MaxValid == false)
					{
						throw Error("Look-behind group does not have " + "an obvious maximum length");
					}
					bool hasSupplementary = FindSupplementary(start, PatternLength);
					if (ch == '=')
					{
						head = tail = (hasSupplementary ? new BehindS(head, info.MaxLength, info.MinLength) : new Behind(head, info.MaxLength, info.MinLength));
					}
					else if (ch == '!')
					{
						head = tail = (hasSupplementary ? new NotBehindS(head, info.MaxLength, info.MinLength) : new NotBehind(head, info.MaxLength, info.MinLength));
					}
					else
					{
						throw Error("Unknown look-behind group");
					}
					break;
				case '$':
				case '@':
					throw Error("Unknown group type");
				default: // (?xxx:) inlined match flags
					Unread();
					AddFlag();
					ch = Read();
					if (ch == ')')
					{
						return null; // Inline modifier only
					}
					if (ch != ':')
					{
						throw Error("Unknown inline modifier");
					}
					head = CreateGroup(true);
					tail = Root;
					head.Next = Expr(tail);
					break;
				}
			} // (xxx) a regular group
			else
			{
				capturingGroup = true;
				head = CreateGroup(false);
				tail = Root;
				head.Next = Expr(tail);
			}

			Accept(')', "Unclosed group");
			Flags_Renamed = save;

			// Check for quantifiers
			Node node = Closure(head);
			if (node == head) // No closure
			{
				Root = tail;
				return node; // Dual return
			}
			if (head == tail) // Zero length assertion
			{
				Root = node;
				return node; // Dual return
			}

			if (node is Ques)
			{
				Ques ques = (Ques) node;
				if (ques.Type == POSSESSIVE)
				{
					Root = node;
					return node;
				}
				tail.Next = new BranchConn();
				tail = tail.Next;
				if (ques.Type == GREEDY)
				{
					head = new Branch(head, null, tail);
				} // Reluctant quantifier
				else
				{
					head = new Branch(null, head, tail);
				}
				Root = tail;
				return head;
			}
			else if (node is Curly)
			{
				Curly curly = (Curly) node;
				if (curly.Type == POSSESSIVE)
				{
					Root = node;
					return node;
				}
				// Discover if the group is deterministic
				TreeInfo info = new TreeInfo();
				if (head.Study(info)) // Deterministic
				{
					GroupTail temp = (GroupTail) tail;
					head = Root = new GroupCurly(head.Next, curly.Cmin, curly.Cmax, curly.Type, ((GroupTail)tail).LocalIndex, ((GroupTail)tail).GroupIndex, capturingGroup);
					return head;
				} // Non-deterministic
				else
				{
					int temp = ((GroupHead) head).LocalIndex;
					Loop loop;
					if (curly.Type == GREEDY)
					{
						loop = new Loop(this.LocalCount, temp);
					}
					else // Reluctant Curly
					{
						loop = new LazyLoop(this.LocalCount, temp);
					}
					Prolog prolog = new Prolog(loop);
					this.LocalCount += 1;
					loop.Cmin = curly.Cmin;
					loop.Cmax = curly.Cmax;
					loop.Body = head;
					tail.Next = loop;
					Root = loop;
					return prolog; // Dual return
				}
			}
			throw Error("Internal logic error");
		}

		/// <summary>
		/// Create group head and tail nodes using double return. If the group is
		/// created with anonymous true then it is a pure group and should not
		/// affect group counting.
		/// </summary>
		private Node CreateGroup(bool anonymous)
		{
			int localIndex = LocalCount++;
			int groupIndex = 0;
			if (!anonymous)
			{
				groupIndex = CapturingGroupCount++;
			}
			GroupHead head = new GroupHead(localIndex);
			Root = new GroupTail(localIndex, groupIndex);
			if (!anonymous && groupIndex < 10)
			{
				GroupNodes[groupIndex] = head;
			}
			return head;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") private void addFlag()
		private void AddFlag()
		/// <summary>
		/// Parses inlined match flags and set them appropriately.
		/// </summary>
		{
			int ch = Peek();
			for (;;)
			{
				switch (ch)
				{
				case 'i':
					Flags_Renamed |= CASE_INSENSITIVE;
					break;
				case 'm':
					Flags_Renamed |= MULTILINE;
					break;
				case 's':
					Flags_Renamed |= DOTALL;
					break;
				case 'd':
					Flags_Renamed |= UNIX_LINES;
					break;
				case 'u':
					Flags_Renamed |= UNICODE_CASE;
					break;
				case 'c':
					Flags_Renamed |= CANON_EQ;
					break;
				case 'x':
					Flags_Renamed |= COMMENTS;
					break;
				case 'U':
					Flags_Renamed |= (UNICODE_CHARACTER_CLASS | UNICODE_CASE);
					break;
				case '-': // subFlag then fall through
					ch = Next();
					SubFlag();
					goto default;
				default:
					return;
				}
				ch = Next();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") private void subFlag()
		private void SubFlag()
		/// <summary>
		/// Parses the second part of inlined match flags and turns off
		/// flags appropriately.
		/// </summary>
		{
			int ch = Peek();
			for (;;)
			{
				switch (ch)
				{
				case 'i':
					Flags_Renamed &= ~CASE_INSENSITIVE;
					break;
				case 'm':
					Flags_Renamed &= ~MULTILINE;
					break;
				case 's':
					Flags_Renamed &= ~DOTALL;
					break;
				case 'd':
					Flags_Renamed &= ~UNIX_LINES;
					break;
				case 'u':
					Flags_Renamed &= ~UNICODE_CASE;
					break;
				case 'c':
					Flags_Renamed &= ~CANON_EQ;
					break;
				case 'x':
					Flags_Renamed &= ~COMMENTS;
					break;
				case 'U':
					Flags_Renamed &= ~(UNICODE_CHARACTER_CLASS | UNICODE_CASE);
					goto default;
				default:
					return;
				}
				ch = Next();
			}
		}

		internal const int MAX_REPS = 0x7FFFFFFF;

		internal const int GREEDY = 0;

		internal const int LAZY = 1;

		internal const int POSSESSIVE = 2;

		internal const int INDEPENDENT = 3;

		/// <summary>
		/// Processes repetition. If the next character peeked is a quantifier
		/// then new nodes must be appended to handle the repetition.
		/// Prev could be a single or a group, so it could be a chain of nodes.
		/// </summary>
		private Node Closure(Node prev)
		{
			Node atom;
			int ch = Peek();
			switch (ch)
			{
			case '?':
				ch = Next();
				if (ch == '?')
				{
					Next();
					return new Ques(prev, LAZY);
				}
				else if (ch == '+')
				{
					Next();
					return new Ques(prev, POSSESSIVE);
				}
				return new Ques(prev, GREEDY);
			case '*':
				ch = Next();
				if (ch == '?')
				{
					Next();
					return new Curly(prev, 0, MAX_REPS, LAZY);
				}
				else if (ch == '+')
				{
					Next();
					return new Curly(prev, 0, MAX_REPS, POSSESSIVE);
				}
				return new Curly(prev, 0, MAX_REPS, GREEDY);
			case '+':
				ch = Next();
				if (ch == '?')
				{
					Next();
					return new Curly(prev, 1, MAX_REPS, LAZY);
				}
				else if (ch == '+')
				{
					Next();
					return new Curly(prev, 1, MAX_REPS, POSSESSIVE);
				}
				return new Curly(prev, 1, MAX_REPS, GREEDY);
			case '{':
				ch = Temp[Cursor_Renamed + 1];
				if (ASCII.IsDigit(ch))
				{
					Skip();
					int cmin = 0;
					do
					{
						cmin = cmin * 10 + (ch - '0');
					} while (ASCII.IsDigit(ch = Read()));
					int cmax = cmin;
					if (ch == ',')
					{
						ch = Read();
						cmax = MAX_REPS;
						if (ch != '}')
						{
							cmax = 0;
							while (ASCII.IsDigit(ch))
							{
								cmax = cmax * 10 + (ch - '0');
								ch = Read();
							}
						}
					}
					if (ch != '}')
					{
						throw Error("Unclosed counted closure");
					}
					if (((cmin) | (cmax) | (cmax - cmin)) < 0)
					{
						throw Error("Illegal repetition range");
					}
					Curly curly;
					ch = Peek();
					if (ch == '?')
					{
						Next();
						curly = new Curly(prev, cmin, cmax, LAZY);
					}
					else if (ch == '+')
					{
						Next();
						curly = new Curly(prev, cmin, cmax, POSSESSIVE);
					}
					else
					{
						curly = new Curly(prev, cmin, cmax, GREEDY);
					}
					return curly;
				}
				else
				{
					throw Error("Illegal repetition");
				}
			default:
				return prev;
			}
		}

		/// <summary>
		///  Utility method for parsing control escape sequences.
		/// </summary>
		private int c()
		{
			if (Cursor_Renamed < PatternLength)
			{
				return Read() ^ 64;
			}
			throw Error("Illegal control escape sequence");
		}

		/// <summary>
		///  Utility method for parsing octal escape sequences.
		/// </summary>
		private int o()
		{
			int n = Read();
			if (((n - '0') | ('7' - n)) >= 0)
			{
				int m = Read();
				if (((m - '0') | ('7' - m)) >= 0)
				{
					int o = Read();
					if ((((o - '0') | ('7' - o)) >= 0) && (((n - '0') | ('3' - n)) >= 0))
					{
						return (n - '0') * 64 + (m - '0') * 8 + (o - '0');
					}
					Unread();
					return (n - '0') * 8 + (m - '0');
				}
				Unread();
				return (n - '0');
			}
			throw Error("Illegal octal escape sequence");
		}

		/// <summary>
		///  Utility method for parsing hexadecimal escape sequences.
		/// </summary>
		private int x()
		{
			int n = Read();
			if (ASCII.IsHexDigit(n))
			{
				int m = Read();
				if (ASCII.IsHexDigit(m))
				{
					return ASCII.ToDigit(n) * 16 + ASCII.ToDigit(m);
				}
			}
			else if (n == '{' && ASCII.IsHexDigit(Peek()))
			{
				int ch = 0;
				while (ASCII.IsHexDigit(n = Read()))
				{
					ch = (ch << 4) + ASCII.ToDigit(n);
					if (ch > Character.MAX_CODE_POINT)
					{
						throw Error("Hexadecimal codepoint is too big");
					}
				}
				if (n != '}')
				{
					throw Error("Unclosed hexadecimal escape sequence");
				}
				return ch;
			}
			throw Error("Illegal hexadecimal escape sequence");
		}

		/// <summary>
		///  Utility method for parsing unicode escape sequences.
		/// </summary>
		private int Cursor()
		{
			return Cursor_Renamed;
		}

		private void Setcursor(int pos)
		{
			Cursor_Renamed = pos;
		}

		private int Uxxxx()
		{
			int n = 0;
			for (int i = 0; i < 4; i++)
			{
				int ch = Read();
				if (!ASCII.IsHexDigit(ch))
				{
					throw Error("Illegal Unicode escape sequence");
				}
				n = n * 16 + ASCII.ToDigit(ch);
			}
			return n;
		}

		private int u()
		{
			int n = Uxxxx();
			if (char.IsHighSurrogate((char)n))
			{
				int cur = Cursor();
				if (Read() == '\\' && Read() == 'u')
				{
					int n2 = Uxxxx();
					if (char.IsLowSurrogate((char)n2))
					{
						return Character.ToCodePoint((char)n, (char)n2);
					}
				}
				Setcursor(cur);
			}
			return n;
		}

		//
		// Utility methods for code point support
		//

		private static int CountChars(CharSequence seq, int index, int lengthInCodePoints)
		{
			// optimization
			if (lengthInCodePoints == 1 && !char.IsHighSurrogate(seq.CharAt(index)))
			{
				assert(index >= 0 && index < seq.Length());
				return 1;
			}
			int length = seq.Length();
			int x = index;
			if (lengthInCodePoints >= 0)
			{
				assert(index >= 0 && index < length);
				for (int i = 0; x < length && i < lengthInCodePoints; i++)
				{
					if (char.IsHighSurrogate(seq.CharAt(x++)))
					{
						if (x < length && char.IsLowSurrogate(seq.CharAt(x)))
						{
							x++;
						}
					}
				}
				return x - index;
			}

			assert(index >= 0 && index <= length);
			if (index == 0)
			{
				return 0;
			}
			int len = -lengthInCodePoints;
			for (int i = 0; x > 0 && i < len; i++)
			{
				if (char.IsLowSurrogate(seq.CharAt(--x)))
				{
					if (x > 0 && char.IsHighSurrogate(seq.CharAt(x - 1)))
					{
						x--;
					}
				}
			}
			return index - x;
		}

		private static int CountCodePoints(CharSequence seq)
		{
			int length = seq.Length();
			int n = 0;
			for (int i = 0; i < length;)
			{
				n++;
				if (char.IsHighSurrogate(seq.CharAt(i++)))
				{
					if (i < length && char.IsLowSurrogate(seq.CharAt(i)))
					{
						i++;
					}
				}
			}
			return n;
		}

		/// <summary>
		///  Creates a bit vector for matching Latin-1 values. A normal BitClass
		///  never matches values above Latin-1, and a complemented BitClass always
		///  matches values above Latin-1.
		/// </summary>
		private sealed class BitClass : BmpCharProperty
		{
			internal readonly bool[] Bits;
			internal BitClass()
			{
				Bits = new bool[256];
			}
			internal BitClass(bool[] bits)
			{
				this.Bits = bits;
			}
			internal BitClass Add(int c, int flags)
			{
				Debug.Assert(c >= 0 && c <= 255);
				if ((flags & CASE_INSENSITIVE) != 0)
				{
					if (ASCII.IsAscii(c))
					{
						Bits[ASCII.ToUpper(c)] = true;
						Bits[ASCII.ToLower(c)] = true;
					}
					else if ((flags & UNICODE_CASE) != 0)
					{
						Bits[char.ToLower(c)] = true;
						Bits[char.ToUpper(c)] = true;
					}
				}
				Bits[c] = true;
				return this;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return ch < 256 && Bits[ch];
			}
		}

		/// <summary>
		///  Returns a suitably optimized, single character matcher.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private CharProperty newSingle(final int ch)
		private CharProperty NewSingle(int ch)
		{
			if (Has(CASE_INSENSITIVE))
			{
				int lower, upper;
				if (Has(UNICODE_CASE))
				{
					upper = char.ToUpper(ch);
					lower = char.ToLower(upper);
					if (upper != lower)
					{
						return new SingleU(lower);
					}
				}
				else if (ASCII.IsAscii(ch))
				{
					lower = ASCII.ToLower(ch);
					upper = ASCII.ToUpper(ch);
					if (lower != upper)
					{
						return new SingleI(lower, upper);
					}
				}
			}
			if (IsSupplementary(ch))
			{
				return new SingleS(ch); // Match a given Unicode character
			}
			return new Single(ch); // Match a given BMP character
		}

		/// <summary>
		///  Utility method for creating a string slice matcher.
		/// </summary>
		private Node NewSlice(int[] buf, int count, bool hasSupplementary)
		{
			int[] tmp = new int[count];
			if (Has(CASE_INSENSITIVE))
			{
				if (Has(UNICODE_CASE))
				{
					for (int i = 0; i < count; i++)
					{
						tmp[i] = char.ToLower(char.ToUpper(buf[i]));
					}
					return hasSupplementary? new SliceUS(tmp) : new SliceU(tmp);
				}
				for (int i = 0; i < count; i++)
				{
					tmp[i] = ASCII.ToLower(buf[i]);
				}
				return hasSupplementary? new SliceIS(tmp) : new SliceI(tmp);
			}
			for (int i = 0; i < count; i++)
			{
				tmp[i] = buf[i];
			}
			return hasSupplementary ? new SliceS(tmp) : new Slice(tmp);
		}

		/// <summary>
		/// The following classes are the building components of the object
		/// tree that represents a compiled regular expression. The object tree
		/// is made of individual elements that handle constructs in the Pattern.
		/// Each type of object knows how to match its equivalent construct with
		/// the match() method.
		/// </summary>

		/// <summary>
		/// Base class for all node classes. Subclasses should override the match()
		/// method as appropriate. This class is an accepting node, so its match()
		/// always returns true.
		/// </summary>
		internal class Node : Object
		{
			internal Node Next;
			internal Node()
			{
				Next = Pattern.Accept_Renamed;
			}
			/// <summary>
			/// This method implements the classic accept node.
			/// </summary>
			internal virtual bool Match(Matcher matcher, int i, CharSequence seq)
			{
				matcher.Last = i;
				matcher.Groups[0] = matcher.First;
				matcher.Groups[1] = matcher.Last;
				return true;
			}
			/// <summary>
			/// This method is good for all zero length assertions.
			/// </summary>
			internal virtual bool Study(TreeInfo info)
			{
				if (Next != null)
				{
					return Next.Study(info);
				}
				else
				{
					return info.Deterministic;
				}
			}
		}

		internal class LastNode : Node
		{
			/// <summary>
			/// This method implements the classic accept node with
			/// the addition of a check to see if the match occurred
			/// using all of the input.
			/// </summary>
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				if (matcher.AcceptMode == Matcher.ENDANCHOR && i != matcher.To)
				{
					return false;
				}
				matcher.Last = i;
				matcher.Groups[0] = matcher.First;
				matcher.Groups[1] = matcher.Last;
				return true;
			}
		}

		/// <summary>
		/// Used for REs that can start anywhere within the input string.
		/// This basically tries to match repeatedly at each spot in the
		/// input string, moving forward after each try. An anchored search
		/// or a BnM will bypass this node completely.
		/// </summary>
		internal class Start : Node
		{
			internal int MinLength;
			internal Start(Node node)
			{
				this.Next = node;
				TreeInfo info = new TreeInfo();
				Next.Study(info);
				MinLength = info.MinLength;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				if (i > matcher.To - MinLength)
				{
					matcher.HitEnd_Renamed = true;
					return false;
				}
				int guard = matcher.To - MinLength;
				for (; i <= guard; i++)
				{
					if (Next.Match(matcher, i, seq))
					{
						matcher.First = i;
						matcher.Groups[0] = matcher.First;
						matcher.Groups[1] = matcher.Last;
						return true;
					}
				}
				matcher.HitEnd_Renamed = true;
				return false;
			}
			internal override bool Study(TreeInfo info)
			{
				Next.Study(info);
				info.MaxValid = false;
				info.Deterministic = false;
				return false;
			}
		}

		/*
		 * StartS supports supplementary characters, including unpaired surrogates.
		 */
		internal sealed class StartS : Start
		{
			internal StartS(Node node) : base(node)
			{
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				if (i > matcher.To - MinLength)
				{
					matcher.HitEnd_Renamed = true;
					return false;
				}
				int guard = matcher.To - MinLength;
				while (i <= guard)
				{
					//if ((ret = next.match(matcher, i, seq)) || i == guard)
					if (Next.Match(matcher, i, seq))
					{
						matcher.First = i;
						matcher.Groups[0] = matcher.First;
						matcher.Groups[1] = matcher.Last;
						return true;
					}
					if (i == guard)
					{
						break;
					}
					// Optimization to move to the next character. This is
					// faster than countChars(seq, i, 1).
					if (char.IsHighSurrogate(seq.CharAt(i++)))
					{
						if (i < seq.Length() && char.IsLowSurrogate(seq.CharAt(i)))
						{
							i++;
						}
					}
				}
				matcher.HitEnd_Renamed = true;
				return false;
			}
		}

		/// <summary>
		/// Node to anchor at the beginning of input. This object implements the
		/// match for a \A sequence, and the caret anchor will use this if not in
		/// multiline mode.
		/// </summary>
		internal sealed class Begin : Node
		{
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int fromIndex = (matcher.AnchoringBounds) ? matcher.From : 0;
				if (i == fromIndex && Next.Match(matcher, i, seq))
				{
					matcher.First = i;
					matcher.Groups[0] = i;
					matcher.Groups[1] = matcher.Last;
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Node to anchor at the end of input. This is the absolute end, so this
		/// should not match at the last newline before the end as $ will.
		/// </summary>
		internal sealed class End : Node
		{
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int endIndex = (matcher.AnchoringBounds) ? matcher.To : matcher.TextLength;
				if (i == endIndex)
				{
					matcher.HitEnd_Renamed = true;
					return Next.Match(matcher, i, seq);
				}
				return false;
			}
		}

		/// <summary>
		/// Node to anchor at the beginning of a line. This is essentially the
		/// object to match for the multiline ^.
		/// </summary>
		internal sealed class Caret : Node
		{
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int startIndex = matcher.From;
				int endIndex = matcher.To;
				if (!matcher.AnchoringBounds)
				{
					startIndex = 0;
					endIndex = matcher.TextLength;
				}
				// Perl does not match ^ at end of input even after newline
				if (i == endIndex)
				{
					matcher.HitEnd_Renamed = true;
					return false;
				}
				if (i > startIndex)
				{
					char ch = seq.CharAt(i - 1);
					if (ch != '\n' && ch != '\r' && (ch | 1) != '\u2029' && ch != '\u0085')
					{
						return false;
					}
					// Should treat /r/n as one newline
					if (ch == '\r' && seq.CharAt(i) == '\n')
					{
						return false;
					}
				}
				return Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// Node to anchor at the beginning of a line when in unixdot mode.
		/// </summary>
		internal sealed class UnixCaret : Node
		{
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int startIndex = matcher.From;
				int endIndex = matcher.To;
				if (!matcher.AnchoringBounds)
				{
					startIndex = 0;
					endIndex = matcher.TextLength;
				}
				// Perl does not match ^ at end of input even after newline
				if (i == endIndex)
				{
					matcher.HitEnd_Renamed = true;
					return false;
				}
				if (i > startIndex)
				{
					char ch = seq.CharAt(i - 1);
					if (ch != '\n')
					{
						return false;
					}
				}
				return Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// Node to match the location where the last match ended.
		/// This is used for the \G construct.
		/// </summary>
		internal sealed class LastMatch : Node
		{
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				if (i != matcher.OldLast)
				{
					return false;
				}
				return Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// Node to anchor at the end of a line or the end of input based on the
		/// multiline mode.
		/// 
		/// When not in multiline mode, the $ can only match at the very end
		/// of the input, unless the input ends in a line terminator in which
		/// it matches right before the last line terminator.
		/// 
		/// Note that \r\n is considered an atomic line terminator.
		/// 
		/// Like ^ the $ operator matches at a position, it does not match the
		/// line terminators themselves.
		/// </summary>
		internal sealed class Dollar : Node
		{
			internal bool Multiline;
			internal Dollar(bool mul)
			{
				Multiline = mul;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int endIndex = (matcher.AnchoringBounds) ? matcher.To : matcher.TextLength;
				if (!Multiline)
				{
					if (i < endIndex - 2)
					{
						return false;
					}
					if (i == endIndex - 2)
					{
						char ch = seq.CharAt(i);
						if (ch != '\r')
						{
							return false;
						}
						ch = seq.CharAt(i + 1);
						if (ch != '\n')
						{
							return false;
						}
					}
				}
				// Matches before any line terminator; also matches at the
				// end of input
				// Before line terminator:
				// If multiline, we match here no matter what
				// If not multiline, fall through so that the end
				// is marked as hit; this must be a /r/n or a /n
				// at the very end so the end was hit; more input
				// could make this not match here
				if (i < endIndex)
				{
					char ch = seq.CharAt(i);
					 if (ch == '\n')
					 {
						 // No match between \r\n
						 if (i > 0 && seq.CharAt(i - 1) == '\r')
						 {
							 return false;
						 }
						 if (Multiline)
						 {
							 return Next.Match(matcher, i, seq);
						 }
					 }
					 else if (ch == '\r' || ch == '\u0085' || (ch | 1) == '\u2029')
					 {
						 if (Multiline)
						 {
							 return Next.Match(matcher, i, seq);
						 }
					 } // No line terminator, no match
					 else
					 {
						 return false;
					 }
				}
				// Matched at current end so hit end
				matcher.HitEnd_Renamed = true;
				// If a $ matches because of end of input, then more input
				// could cause it to fail!
				matcher.RequireEnd_Renamed = true;
				return Next.Match(matcher, i, seq);
			}
			internal override bool Study(TreeInfo info)
			{
				Next.Study(info);
				return info.Deterministic;
			}
		}

		/// <summary>
		/// Node to anchor at the end of a line or the end of input based on the
		/// multiline mode when in unix lines mode.
		/// </summary>
		internal sealed class UnixDollar : Node
		{
			internal bool Multiline;
			internal UnixDollar(bool mul)
			{
				Multiline = mul;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int endIndex = (matcher.AnchoringBounds) ? matcher.To : matcher.TextLength;
				if (i < endIndex)
				{
					char ch = seq.CharAt(i);
					if (ch == '\n')
					{
						// If not multiline, then only possible to
						// match at very end or one before end
						if (Multiline == false && i != endIndex - 1)
						{
							return false;
						}
						// If multiline return next.match without setting
						// matcher.hitEnd
						if (Multiline)
						{
							return Next.Match(matcher, i, seq);
						}
					}
					else
					{
						return false;
					}
				}
				// Matching because at the end or 1 before the end;
				// more input could change this so set hitEnd
				matcher.HitEnd_Renamed = true;
				// If a $ matches because of end of input, then more input
				// could cause it to fail!
				matcher.RequireEnd_Renamed = true;
				return Next.Match(matcher, i, seq);
			}
			internal override bool Study(TreeInfo info)
			{
				Next.Study(info);
				return info.Deterministic;
			}
		}

		/// <summary>
		/// Node class that matches a Unicode line ending '\R'
		/// </summary>
		internal sealed class LineEnding : Node
		{
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				// (u+000Du+000A|[u+000Au+000Bu+000Cu+000Du+0085u+2028u+2029])
				if (i < matcher.To)
				{
					int ch = seq.CharAt(i);
					if (ch == 0x0A || ch == 0x0B || ch == 0x0C || ch == 0x85 || ch == 0x2028 || ch == 0x2029)
					{
						return Next.Match(matcher, i + 1, seq);
					}
					if (ch == 0x0D)
					{
						i++;
						if (i < matcher.To && seq.CharAt(i) == 0x0A)
						{
							i++;
						}
						return Next.Match(matcher, i, seq);
					}
				}
				else
				{
					matcher.HitEnd_Renamed = true;
				}
				return false;
			}
			internal override bool Study(TreeInfo info)
			{
				info.MinLength++;
				info.MaxLength += 2;
				return Next.Study(info);
			}
		}

		/// <summary>
		/// Abstract node class to match one character satisfying some
		/// boolean property.
		/// </summary>
		private abstract class CharProperty : Node
		{
			internal abstract bool IsSatisfiedBy(int ch);
			internal virtual CharProperty Complement()
			{
				return new CharPropertyAnonymousInnerClassHelper(this);
			}

			private class CharPropertyAnonymousInnerClassHelper : CharProperty
			{
				private readonly CharProperty OuterInstance;

				public CharPropertyAnonymousInnerClassHelper(CharProperty outerInstance)
				{
					this.OuterInstance = outerInstance;
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return !OuterInstance.IsSatisfiedBy(ch);
				}
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				if (i < matcher.To)
				{
					int ch = Character.CodePointAt(seq, i);
					return IsSatisfiedBy(ch) && Next.Match(matcher, i + Character.CharCount(ch), seq);
				}
				else
				{
					matcher.HitEnd_Renamed = true;
					return false;
				}
			}
			internal override bool Study(TreeInfo info)
			{
				info.MinLength++;
				info.MaxLength++;
				return Next.Study(info);
			}
		}

		/// <summary>
		/// Optimized version of CharProperty that works only for
		/// properties never satisfied by Supplementary characters.
		/// </summary>
		private abstract class BmpCharProperty : CharProperty
		{
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				if (i < matcher.To)
				{
					return IsSatisfiedBy(seq.CharAt(i)) && Next.Match(matcher, i + 1, seq);
				}
				else
				{
					matcher.HitEnd_Renamed = true;
					return false;
				}
			}
		}

		/// <summary>
		/// Node class that matches a Supplementary Unicode character
		/// </summary>
		internal sealed class SingleS : CharProperty
		{
			internal readonly int c;
			internal SingleS(int c)
			{
				this.c = c;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return ch == c;
			}
		}

		/// <summary>
		/// Optimization -- matches a given BMP character
		/// </summary>
		internal sealed class Single : BmpCharProperty
		{
			internal readonly int c;
			internal Single(int c)
			{
				this.c = c;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return ch == c;
			}
		}

		/// <summary>
		/// Case insensitive matches a given BMP character
		/// </summary>
		internal sealed class SingleI : BmpCharProperty
		{
			internal readonly int Lower;
			internal readonly int Upper;
			internal SingleI(int lower, int upper)
			{
				this.Lower = lower;
				this.Upper = upper;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return ch == Lower || ch == Upper;
			}
		}

		/// <summary>
		/// Unicode case insensitive matches a given Unicode character
		/// </summary>
		internal sealed class SingleU : CharProperty
		{
			internal readonly int Lower;
			internal SingleU(int lower)
			{
				this.Lower = lower;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return Lower == ch || Lower == char.ToLower(char.ToUpper(ch));
			}
		}

		/// <summary>
		/// Node class that matches a Unicode block.
		/// </summary>
		internal sealed class Block : CharProperty
		{
			internal readonly Character.UnicodeBlock Block_Renamed;
			internal Block(Character.UnicodeBlock block)
			{
				this.Block_Renamed = block;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return Block_Renamed == Character.UnicodeBlock.Of(ch);
			}
		}

		/// <summary>
		/// Node class that matches a Unicode script
		/// </summary>
		internal sealed class Script : CharProperty
		{
			internal readonly Character.UnicodeScript Script_Renamed;
			internal Script(Character.UnicodeScript script)
			{
				this.Script_Renamed = script;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return Script_Renamed == Character.UnicodeScript.of(ch);
			}
		}

		/// <summary>
		/// Node class that matches a Unicode category.
		/// </summary>
		internal sealed class Category : CharProperty
		{
			internal readonly int TypeMask;
			internal Category(int typeMask)
			{
				this.TypeMask = typeMask;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return (TypeMask & (1 << Character.GetType(ch))) != 0;
			}
		}

		/// <summary>
		/// Node class that matches a Unicode "type"
		/// </summary>
		internal sealed class Utype : CharProperty
		{
			internal readonly UnicodeProp Uprop;
			internal Utype(UnicodeProp uprop)
			{
				this.Uprop = uprop;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return Uprop.@is(ch);
			}
		}

		/// <summary>
		/// Node class that matches a POSIX type.
		/// </summary>
		internal sealed class Ctype : BmpCharProperty
		{
			internal readonly int Ctype_Renamed;
			internal Ctype(int ctype)
			{
				this.Ctype_Renamed = ctype;
			}
			internal override bool IsSatisfiedBy(int ch)
			{
				return ch < 128 && ASCII.IsType(ch, Ctype_Renamed);
			}
		}

		/// <summary>
		/// Node class that matches a Perl vertical whitespace
		/// </summary>
		internal sealed class VertWS : BmpCharProperty
		{
			internal override bool IsSatisfiedBy(int cp)
			{
				return (cp >= 0x0A && cp <= 0x0D) || cp == 0x85 || cp == 0x2028 || cp == 0x2029;
			}
		}

		/// <summary>
		/// Node class that matches a Perl horizontal whitespace
		/// </summary>
		internal sealed class HorizWS : BmpCharProperty
		{
			internal override bool IsSatisfiedBy(int cp)
			{
				return cp == 0x09 || cp == 0x20 || cp == 0xa0 || cp == 0x1680 || cp == 0x180e || cp >= 0x2000 && cp <= 0x200a || cp == 0x202f || cp == 0x205f || cp == 0x3000;
			}
		}

		/// <summary>
		/// Base class for all Slice nodes
		/// </summary>
		internal class SliceNode : Node
		{
			internal int[] Buffer;
			internal SliceNode(int[] buf)
			{
				Buffer = buf;
			}
			internal override bool Study(TreeInfo info)
			{
				info.MinLength += Buffer.Length;
				info.MaxLength += Buffer.Length;
				return Next.Study(info);
			}
		}

		/// <summary>
		/// Node class for a case sensitive/BMP-only sequence of literal
		/// characters.
		/// </summary>
		internal sealed class Slice : SliceNode
		{
			internal Slice(int[] buf) : base(buf)
			{
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int[] buf = Buffer;
				int len = buf.Length;
				for (int j = 0; j < len; j++)
				{
					if ((i + j) >= matcher.To)
					{
						matcher.HitEnd_Renamed = true;
						return false;
					}
					if (buf[j] != seq.CharAt(i + j))
					{
						return false;
					}
				}
				return Next.Match(matcher, i + len, seq);
			}
		}

		/// <summary>
		/// Node class for a case_insensitive/BMP-only sequence of literal
		/// characters.
		/// </summary>
		internal class SliceI : SliceNode
		{
			internal SliceI(int[] buf) : base(buf)
			{
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int[] buf = Buffer;
				int len = buf.Length;
				for (int j = 0; j < len; j++)
				{
					if ((i + j) >= matcher.To)
					{
						matcher.HitEnd_Renamed = true;
						return false;
					}
					int c = seq.CharAt(i + j);
					if (buf[j] != c && buf[j] != ASCII.ToLower(c))
					{
						return false;
					}
				}
				return Next.Match(matcher, i + len, seq);
			}
		}

		/// <summary>
		/// Node class for a unicode_case_insensitive/BMP-only sequence of
		/// literal characters. Uses unicode case folding.
		/// </summary>
		internal sealed class SliceU : SliceNode
		{
			internal SliceU(int[] buf) : base(buf)
			{
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int[] buf = Buffer;
				int len = buf.Length;
				for (int j = 0; j < len; j++)
				{
					if ((i + j) >= matcher.To)
					{
						matcher.HitEnd_Renamed = true;
						return false;
					}
					int c = seq.CharAt(i + j);
					if (buf[j] != c && buf[j] != char.ToLower(char.ToUpper(c)))
					{
						return false;
					}
				}
				return Next.Match(matcher, i + len, seq);
			}
		}

		/// <summary>
		/// Node class for a case sensitive sequence of literal characters
		/// including supplementary characters.
		/// </summary>
		internal sealed class SliceS : SliceNode
		{
			internal SliceS(int[] buf) : base(buf)
			{
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int[] buf = Buffer;
				int x = i;
				for (int j = 0; j < buf.Length; j++)
				{
					if (x >= matcher.To)
					{
						matcher.HitEnd_Renamed = true;
						return false;
					}
					int c = Character.CodePointAt(seq, x);
					if (buf[j] != c)
					{
						return false;
					}
					x += Character.CharCount(c);
					if (x > matcher.To)
					{
						matcher.HitEnd_Renamed = true;
						return false;
					}
				}
				return Next.Match(matcher, x, seq);
			}
		}

		/// <summary>
		/// Node class for a case insensitive sequence of literal characters
		/// including supplementary characters.
		/// </summary>
		internal class SliceIS : SliceNode
		{
			internal SliceIS(int[] buf) : base(buf)
			{
			}
			internal virtual int ToLower(int c)
			{
				return ASCII.ToLower(c);
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int[] buf = Buffer;
				int x = i;
				for (int j = 0; j < buf.Length; j++)
				{
					if (x >= matcher.To)
					{
						matcher.HitEnd_Renamed = true;
						return false;
					}
					int c = Character.CodePointAt(seq, x);
					if (buf[j] != c && buf[j] != ToLower(c))
					{
						return false;
					}
					x += Character.CharCount(c);
					if (x > matcher.To)
					{
						matcher.HitEnd_Renamed = true;
						return false;
					}
				}
				return Next.Match(matcher, x, seq);
			}
		}

		/// <summary>
		/// Node class for a case insensitive sequence of literal characters.
		/// Uses unicode case folding.
		/// </summary>
		internal sealed class SliceUS : SliceIS
		{
			internal SliceUS(int[] buf) : base(buf)
			{
			}
			internal override int ToLower(int c)
			{
				return char.ToLower(char.ToUpper(c));
			}
		}

		private static bool InRange(int lower, int ch, int upper)
		{
			return lower <= ch && ch <= upper;
		}

		/// <summary>
		/// Returns node for matching characters within an explicit value range.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static CharProperty rangeFor(final int lower, final int upper)
		private static CharProperty RangeFor(int lower, int upper)
		{
			return new CharPropertyAnonymousInnerClassHelper(lower, upper);
		}

		private class CharPropertyAnonymousInnerClassHelper : CharProperty
		{
			private int Lower;
			private int Upper;

			public CharPropertyAnonymousInnerClassHelper(int lower, int upper)
			{
				this.Lower = lower;
				this.Upper = upper;
			}

			internal override bool IsSatisfiedBy(int ch)
			{
				return InRange(Lower, ch, Upper);
			}
		}

		/// <summary>
		/// Returns node for matching characters within an explicit value
		/// range in a case insensitive manner.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private CharProperty caseInsensitiveRangeFor(final int lower, final int upper)
		private CharProperty CaseInsensitiveRangeFor(int lower, int upper)
		{
			if (Has(UNICODE_CASE))
			{
				return new CharPropertyAnonymousInnerClassHelper2(this, lower, upper);
			}
			return new CharPropertyAnonymousInnerClassHelper3(this, lower, upper);
		}

		private class CharPropertyAnonymousInnerClassHelper2 : CharProperty
		{
			private readonly Pattern OuterInstance;

			private int Lower;
			private int Upper;

			public CharPropertyAnonymousInnerClassHelper2(Pattern outerInstance, int lower, int upper)
			{
				this.OuterInstance = outerInstance;
				this.Lower = lower;
				this.Upper = upper;
			}

			internal override bool IsSatisfiedBy(int ch)
			{
				if (InRange(Lower, ch, Upper))
				{
					return true;
				}
				int up = char.ToUpper(ch);
				return InRange(Lower, up, Upper) || InRange(Lower, char.ToLower(up), Upper);
			}
		}

		private class CharPropertyAnonymousInnerClassHelper3 : CharProperty
		{
			private readonly Pattern OuterInstance;

			private int Lower;
			private int Upper;

			public CharPropertyAnonymousInnerClassHelper3(Pattern outerInstance, int lower, int upper)
			{
				this.OuterInstance = outerInstance;
				this.Lower = lower;
				this.Upper = upper;
			}

			internal override bool IsSatisfiedBy(int ch)
			{
				return InRange(Lower, ch, Upper) || ASCII.IsAscii(ch) && (InRange(Lower, ASCII.ToUpper(ch), Upper) || InRange(Lower, ASCII.ToLower(ch), Upper));
			}
		}

		/// <summary>
		/// Implements the Unicode category ALL and the dot metacharacter when
		/// in dotall mode.
		/// </summary>
		internal sealed class All : CharProperty
		{
			internal override bool IsSatisfiedBy(int ch)
			{
				return true;
			}
		}

		/// <summary>
		/// Node class for the dot metacharacter when dotall is not enabled.
		/// </summary>
		internal sealed class Dot : CharProperty
		{
			internal override bool IsSatisfiedBy(int ch)
			{
				return (ch != '\n' && ch != '\r' && (ch | 1) != '\u2029' && ch != '\u0085');
			}
		}

		/// <summary>
		/// Node class for the dot metacharacter when dotall is not enabled
		/// but UNIX_LINES is enabled.
		/// </summary>
		internal sealed class UnixDot : CharProperty
		{
			internal override bool IsSatisfiedBy(int ch)
			{
				return ch != '\n';
			}
		}

		/// <summary>
		/// The 0 or 1 quantifier. This one class implements all three types.
		/// </summary>
		internal sealed class Ques : Node
		{
			internal Node Atom;
			internal int Type;
			internal Ques(Node node, int type)
			{
				this.Atom = node;
				this.Type = type;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				switch (Type)
				{
				case GREEDY:
					return (Atom.Match(matcher, i, seq) && Next.Match(matcher, matcher.Last, seq)) || Next.Match(matcher, i, seq);
				case LAZY:
					return Next.Match(matcher, i, seq) || (Atom.Match(matcher, i, seq) && Next.Match(matcher, matcher.Last, seq));
				case POSSESSIVE:
					if (Atom.Match(matcher, i, seq))
					{
						i = matcher.Last;
					}
					return Next.Match(matcher, i, seq);
				default:
					return Atom.Match(matcher, i, seq) && Next.Match(matcher, matcher.Last, seq);
				}
			}
			internal override bool Study(TreeInfo info)
			{
				if (Type != INDEPENDENT)
				{
					int minL = info.MinLength;
					Atom.Study(info);
					info.MinLength = minL;
					info.Deterministic = false;
					return Next.Study(info);
				}
				else
				{
					Atom.Study(info);
					return Next.Study(info);
				}
			}
		}

		/// <summary>
		/// Handles the curly-brace style repetition with a specified minimum and
		/// maximum occurrences. The * quantifier is handled as a special case.
		/// This class handles the three types.
		/// </summary>
		internal sealed class Curly : Node
		{
			internal Node Atom;
			internal int Type;
			internal int Cmin;
			internal int Cmax;

			internal Curly(Node node, int cmin, int cmax, int type)
			{
				this.Atom = node;
				this.Type = type;
				this.Cmin = cmin;
				this.Cmax = cmax;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int j;
				for (j = 0; j < Cmin; j++)
				{
					if (Atom.Match(matcher, i, seq))
					{
						i = matcher.Last;
						continue;
					}
					return false;
				}
				if (Type == GREEDY)
				{
					return Match0(matcher, i, j, seq);
				}
				else if (Type == LAZY)
				{
					return Match1(matcher, i, j, seq);
				}
				else
				{
					return Match2(matcher, i, j, seq);
				}
			}
			// Greedy match.
			// i is the index to start matching at
			// j is the number of atoms that have matched
			internal bool Match0(Matcher matcher, int i, int j, CharSequence seq)
			{
				if (j >= Cmax)
				{
					// We have matched the maximum... continue with the rest of
					// the regular expression
					return Next.Match(matcher, i, seq);
				}
				int backLimit = j;
				while (Atom.Match(matcher, i, seq))
				{
					// k is the length of this match
					int k = matcher.Last - i;
					if (k == 0) // Zero length match
					{
						break;
					}
					// Move up index and number matched
					i = matcher.Last;
					j++;
					// We are greedy so match as many as we can
					while (j < Cmax)
					{
						if (!Atom.Match(matcher, i, seq))
						{
							break;
						}
						if (i + k != matcher.Last)
						{
							if (Match0(matcher, matcher.Last, j + 1, seq))
							{
								return true;
							}
							break;
						}
						i += k;
						j++;
					}
					// Handle backing off if match fails
					while (j >= backLimit)
					{
					   if (Next.Match(matcher, i, seq))
					   {
							return true;
					   }
						i -= k;
						j--;
					}
					return false;
				}
				return Next.Match(matcher, i, seq);
			}
			// Reluctant match. At this point, the minimum has been satisfied.
			// i is the index to start matching at
			// j is the number of atoms that have matched
			internal bool Match1(Matcher matcher, int i, int j, CharSequence seq)
			{
				for (;;)
				{
					// Try finishing match without consuming any more
					if (Next.Match(matcher, i, seq))
					{
						return true;
					}
					// At the maximum, no match found
					if (j >= Cmax)
					{
						return false;
					}
					// Okay, must try one more atom
					if (!Atom.Match(matcher, i, seq))
					{
						return false;
					}
					// If we haven't moved forward then must break out
					if (i == matcher.Last)
					{
						return false;
					}
					// Move up index and number matched
					i = matcher.Last;
					j++;
				}
			}
			internal bool Match2(Matcher matcher, int i, int j, CharSequence seq)
			{
				for (; j < Cmax; j++)
				{
					if (!Atom.Match(matcher, i, seq))
					{
						break;
					}
					if (i == matcher.Last)
					{
						break;
					}
					i = matcher.Last;
				}
				return Next.Match(matcher, i, seq);
			}
			internal override bool Study(TreeInfo info)
			{
				// Save original info
				int minL = info.MinLength;
				int maxL = info.MaxLength;
				bool maxV = info.MaxValid;
				bool detm = info.Deterministic;
				info.Reset();

				Atom.Study(info);

				int temp = info.MinLength * Cmin + minL;
				if (temp < minL)
				{
					temp = 0xFFFFFFF; // arbitrary large number
				}
				info.MinLength = temp;

				if (maxV & info.MaxValid)
				{
					temp = info.MaxLength * Cmax + maxL;
					info.MaxLength = temp;
					if (temp < maxL)
					{
						info.MaxValid = false;
					}
				}
				else
				{
					info.MaxValid = false;
				}

				if (info.Deterministic && Cmin == Cmax)
				{
					info.Deterministic = detm;
				}
				else
				{
					info.Deterministic = false;
				}
				return Next.Study(info);
			}
		}

		/// <summary>
		/// Handles the curly-brace style repetition with a specified minimum and
		/// maximum occurrences in deterministic cases. This is an iterative
		/// optimization over the Prolog and Loop system which would handle this
		/// in a recursive way. The * quantifier is handled as a special case.
		/// If capture is true then this class saves group settings and ensures
		/// that groups are unset when backing off of a group match.
		/// </summary>
		internal sealed class GroupCurly : Node
		{
			internal Node Atom;
			internal int Type;
			internal int Cmin;
			internal int Cmax;
			internal int LocalIndex;
			internal int GroupIndex;
			internal bool Capture;

			internal GroupCurly(Node node, int cmin, int cmax, int type, int local, int group, bool capture)
			{
				this.Atom = node;
				this.Type = type;
				this.Cmin = cmin;
				this.Cmax = cmax;
				this.LocalIndex = local;
				this.GroupIndex = group;
				this.Capture = capture;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int[] groups = matcher.Groups;
				int[] locals = matcher.Locals;
				int save0 = locals[LocalIndex];
				int save1 = 0;
				int save2 = 0;

				if (Capture)
				{
					save1 = groups[GroupIndex];
					save2 = groups[GroupIndex + 1];
				}

				// Notify GroupTail there is no need to setup group info
				// because it will be set here
				locals[LocalIndex] = -1;

				bool ret = true;
				for (int j = 0; j < Cmin; j++)
				{
					if (Atom.Match(matcher, i, seq))
					{
						if (Capture)
						{
							groups[GroupIndex] = i;
							groups[GroupIndex + 1] = matcher.Last;
						}
						i = matcher.Last;
					}
					else
					{
						ret = false;
						break;
					}
				}
				if (ret)
				{
					if (Type == GREEDY)
					{
						ret = Match0(matcher, i, Cmin, seq);
					}
					else if (Type == LAZY)
					{
						ret = Match1(matcher, i, Cmin, seq);
					}
					else
					{
						ret = Match2(matcher, i, Cmin, seq);
					}
				}
				if (!ret)
				{
					locals[LocalIndex] = save0;
					if (Capture)
					{
						groups[GroupIndex] = save1;
						groups[GroupIndex + 1] = save2;
					}
				}
				return ret;
			}
			// Aggressive group match
			internal bool Match0(Matcher matcher, int i, int j, CharSequence seq)
			{
				// don't back off passing the starting "j"
				int min = j;
				int[] groups = matcher.Groups;
				int save0 = 0;
				int save1 = 0;
				if (Capture)
				{
					save0 = groups[GroupIndex];
					save1 = groups[GroupIndex + 1];
				}
				for (;;)
				{
					if (j >= Cmax)
					{
						break;
					}
					if (!Atom.Match(matcher, i, seq))
					{
						break;
					}
					int k = matcher.Last - i;
					if (k <= 0)
					{
						if (Capture)
						{
							groups[GroupIndex] = i;
							groups[GroupIndex + 1] = i + k;
						}
						i = i + k;
						break;
					}
					for (;;)
					{
						if (Capture)
						{
							groups[GroupIndex] = i;
							groups[GroupIndex + 1] = i + k;
						}
						i = i + k;
						if (++j >= Cmax)
						{
							break;
						}
						if (!Atom.Match(matcher, i, seq))
						{
							break;
						}
						if (i + k != matcher.Last)
						{
							if (Match0(matcher, i, j, seq))
							{
								return true;
							}
							break;
						}
					}
					while (j > min)
					{
						if (Next.Match(matcher, i, seq))
						{
							if (Capture)
							{
								groups[GroupIndex + 1] = i;
								groups[GroupIndex] = i - k;
							}
							return true;
						}
						// backing off
						i = i - k;
						if (Capture)
						{
							groups[GroupIndex + 1] = i;
							groups[GroupIndex] = i - k;
						}
						j--;

					}
					break;
				}
				if (Capture)
				{
					groups[GroupIndex] = save0;
					groups[GroupIndex + 1] = save1;
				}
				return Next.Match(matcher, i, seq);
			}
			// Reluctant matching
			internal bool Match1(Matcher matcher, int i, int j, CharSequence seq)
			{
				for (;;)
				{
					if (Next.Match(matcher, i, seq))
					{
						return true;
					}
					if (j >= Cmax)
					{
						return false;
					}
					if (!Atom.Match(matcher, i, seq))
					{
						return false;
					}
					if (i == matcher.Last)
					{
						return false;
					}
					if (Capture)
					{
						matcher.Groups[GroupIndex] = i;
						matcher.Groups[GroupIndex + 1] = matcher.Last;
					}
					i = matcher.Last;
					j++;
				}
			}
			// Possessive matching
			internal bool Match2(Matcher matcher, int i, int j, CharSequence seq)
			{
				for (; j < Cmax; j++)
				{
					if (!Atom.Match(matcher, i, seq))
					{
						break;
					}
					if (Capture)
					{
						matcher.Groups[GroupIndex] = i;
						matcher.Groups[GroupIndex + 1] = matcher.Last;
					}
					if (i == matcher.Last)
					{
						break;
					}
					i = matcher.Last;
				}
				return Next.Match(matcher, i, seq);
			}
			internal override bool Study(TreeInfo info)
			{
				// Save original info
				int minL = info.MinLength;
				int maxL = info.MaxLength;
				bool maxV = info.MaxValid;
				bool detm = info.Deterministic;
				info.Reset();

				Atom.Study(info);

				int temp = info.MinLength * Cmin + minL;
				if (temp < minL)
				{
					temp = 0xFFFFFFF; // Arbitrary large number
				}
				info.MinLength = temp;

				if (maxV & info.MaxValid)
				{
					temp = info.MaxLength * Cmax + maxL;
					info.MaxLength = temp;
					if (temp < maxL)
					{
						info.MaxValid = false;
					}
				}
				else
				{
					info.MaxValid = false;
				}

				if (info.Deterministic && Cmin == Cmax)
				{
					info.Deterministic = detm;
				}
				else
				{
					info.Deterministic = false;
				}
				return Next.Study(info);
			}
		}

		/// <summary>
		/// A Guard node at the end of each atom node in a Branch. It
		/// serves the purpose of chaining the "match" operation to
		/// "next" but not the "study", so we can collect the TreeInfo
		/// of each atom node without including the TreeInfo of the
		/// "next".
		/// </summary>
		internal sealed class BranchConn : Node
		{
			internal BranchConn()
			{
			};
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				return Next.Match(matcher, i, seq);
			}
			internal override bool Study(TreeInfo info)
			{
				return info.Deterministic;
			}
		}

		/// <summary>
		/// Handles the branching of alternations. Note this is also used for
		/// the ? quantifier to branch between the case where it matches once
		/// and where it does not occur.
		/// </summary>
		internal sealed class Branch : Node
		{
			internal Node[] Atoms = new Node[2];
			internal int Size = 2;
			internal Node Conn;
			internal Branch(Node first, Node second, Node branchConn)
			{
				Conn = branchConn;
				Atoms[0] = first;
				Atoms[1] = second;
			}

			internal void Add(Node node)
			{
				if (Size >= Atoms.Length)
				{
					Node[] tmp = new Node[Atoms.Length * 2];
					System.Array.Copy(Atoms, 0, tmp, 0, Atoms.Length);
					Atoms = tmp;
				}
				Atoms[Size++] = node;
			}

			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				for (int n = 0; n < Size; n++)
				{
					if (Atoms[n] == null)
					{
						if (Conn.Next.Match(matcher, i, seq))
						{
							return true;
						}
					}
					else if (Atoms[n].Match(matcher, i, seq))
					{
						return true;
					}
				}
				return false;
			}

			internal override bool Study(TreeInfo info)
			{
				int minL = info.MinLength;
				int maxL = info.MaxLength;
				bool maxV = info.MaxValid;

				int minL2 = Integer.MaxValue; //arbitrary large enough num
				int maxL2 = -1;
				for (int n = 0; n < Size; n++)
				{
					info.Reset();
					if (Atoms[n] != null)
					{
						Atoms[n].Study(info);
					}
					minL2 = System.Math.Min(minL2, info.MinLength);
					maxL2 = System.Math.Max(maxL2, info.MaxLength);
					maxV = (maxV & info.MaxValid);
				}

				minL += minL2;
				maxL += maxL2;

				info.Reset();
				Conn.Next.Study(info);

				info.MinLength += minL;
				info.MaxLength += maxL;
				info.MaxValid &= maxV;
				info.Deterministic = false;
				return false;
			}
		}

		/// <summary>
		/// The GroupHead saves the location where the group begins in the locals
		/// and restores them when the match is done.
		/// 
		/// The matchRef is used when a reference to this group is accessed later
		/// in the expression. The locals will have a negative value in them to
		/// indicate that we do not want to unset the group if the reference
		/// doesn't match.
		/// </summary>
		internal sealed class GroupHead : Node
		{
			internal int LocalIndex;
			internal GroupHead(int localCount)
			{
				LocalIndex = localCount;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int save = matcher.Locals[LocalIndex];
				matcher.Locals[LocalIndex] = i;
				bool ret = Next.Match(matcher, i, seq);
				matcher.Locals[LocalIndex] = save;
				return ret;
			}
			internal bool MatchRef(Matcher matcher, int i, CharSequence seq)
			{
				int save = matcher.Locals[LocalIndex];
				matcher.Locals[LocalIndex] = ~i; // HACK
				bool ret = Next.Match(matcher, i, seq);
				matcher.Locals[LocalIndex] = save;
				return ret;
			}
		}

		/// <summary>
		/// Recursive reference to a group in the regular expression. It calls
		/// matchRef because if the reference fails to match we would not unset
		/// the group.
		/// </summary>
		internal sealed class GroupRef : Node
		{
			internal GroupHead Head;
			internal GroupRef(GroupHead head)
			{
				this.Head = head;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				return Head.MatchRef(matcher, i, seq) && Next.Match(matcher, matcher.Last, seq);
			}
			internal override bool Study(TreeInfo info)
			{
				info.MaxValid = false;
				info.Deterministic = false;
				return Next.Study(info);
			}
		}

		/// <summary>
		/// The GroupTail handles the setting of group beginning and ending
		/// locations when groups are successfully matched. It must also be able to
		/// unset groups that have to be backed off of.
		/// 
		/// The GroupTail node is also used when a previous group is referenced,
		/// and in that case no group information needs to be set.
		/// </summary>
		internal sealed class GroupTail : Node
		{
			internal int LocalIndex;
			internal int GroupIndex;
			internal GroupTail(int localCount, int groupCount)
			{
				LocalIndex = localCount;
				GroupIndex = groupCount + groupCount;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int tmp = matcher.Locals[LocalIndex];
				if (tmp >= 0) // This is the normal group case.
				{
					// Save the group so we can unset it if it
					// backs off of a match.
					int groupStart = matcher.Groups[GroupIndex];
					int groupEnd = matcher.Groups[GroupIndex + 1];

					matcher.Groups[GroupIndex] = tmp;
					matcher.Groups[GroupIndex + 1] = i;
					if (Next.Match(matcher, i, seq))
					{
						return true;
					}
					matcher.Groups[GroupIndex] = groupStart;
					matcher.Groups[GroupIndex + 1] = groupEnd;
					return false;
				}
				else
				{
					// This is a group reference case. We don't need to save any
					// group info because it isn't really a group.
					matcher.Last = i;
					return true;
				}
			}
		}

		/// <summary>
		/// This sets up a loop to handle a recursive quantifier structure.
		/// </summary>
		internal sealed class Prolog : Node
		{
			internal Loop Loop;
			internal Prolog(Loop loop)
			{
				this.Loop = loop;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				return Loop.MatchInit(matcher, i, seq);
			}
			internal override bool Study(TreeInfo info)
			{
				return Loop.Study(info);
			}
		}

		/// <summary>
		/// Handles the repetition count for a greedy Curly. The matchInit
		/// is called from the Prolog to save the index of where the group
		/// beginning is stored. A zero length group check occurs in the
		/// normal match but is skipped in the matchInit.
		/// </summary>
		internal class Loop : Node
		{
			internal Node Body;
			internal int CountIndex; // local count index in matcher locals
			internal int BeginIndex; // group beginning index
			internal int Cmin, Cmax;
			internal Loop(int countIndex, int beginIndex)
			{
				this.CountIndex = countIndex;
				this.BeginIndex = beginIndex;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				// Avoid infinite loop in zero-length case.
				if (i > matcher.Locals[BeginIndex])
				{
					int count = matcher.Locals[CountIndex];

					// This block is for before we reach the minimum
					// iterations required for the loop to match
					if (count < Cmin)
					{
						matcher.Locals[CountIndex] = count + 1;
						bool b = Body.Match(matcher, i, seq);
						// If match failed we must backtrack, so
						// the loop count should NOT be incremented
						if (!b)
						{
							matcher.Locals[CountIndex] = count;
						}
						// Return success or failure since we are under
						// minimum
						return b;
					}
					// This block is for after we have the minimum
					// iterations required for the loop to match
					if (count < Cmax)
					{
						matcher.Locals[CountIndex] = count + 1;
						bool b = Body.Match(matcher, i, seq);
						// If match failed we must backtrack, so
						// the loop count should NOT be incremented
						if (!b)
						{
							matcher.Locals[CountIndex] = count;
						}
						else
						{
							return true;
						}
					}
				}
				return Next.Match(matcher, i, seq);
			}
			internal virtual bool MatchInit(Matcher matcher, int i, CharSequence seq)
			{
				int save = matcher.Locals[CountIndex];
				bool ret = false;
				if (0 < Cmin)
				{
					matcher.Locals[CountIndex] = 1;
					ret = Body.Match(matcher, i, seq);
				}
				else if (0 < Cmax)
				{
					matcher.Locals[CountIndex] = 1;
					ret = Body.Match(matcher, i, seq);
					if (ret == false)
					{
						ret = Next.Match(matcher, i, seq);
					}
				}
				else
				{
					ret = Next.Match(matcher, i, seq);
				}
				matcher.Locals[CountIndex] = save;
				return ret;
			}
			internal override bool Study(TreeInfo info)
			{
				info.MaxValid = false;
				info.Deterministic = false;
				return false;
			}
		}

		/// <summary>
		/// Handles the repetition count for a reluctant Curly. The matchInit
		/// is called from the Prolog to save the index of where the group
		/// beginning is stored. A zero length group check occurs in the
		/// normal match but is skipped in the matchInit.
		/// </summary>
		internal sealed class LazyLoop : Loop
		{
			internal LazyLoop(int countIndex, int beginIndex) : base(countIndex, beginIndex)
			{
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				// Check for zero length group
				if (i > matcher.Locals[BeginIndex])
				{
					int count = matcher.Locals[CountIndex];
					if (count < Cmin)
					{
						matcher.Locals[CountIndex] = count + 1;
						bool result = Body.Match(matcher, i, seq);
						// If match failed we must backtrack, so
						// the loop count should NOT be incremented
						if (!result)
						{
							matcher.Locals[CountIndex] = count;
						}
						return result;
					}
					if (Next.Match(matcher, i, seq))
					{
						return true;
					}
					if (count < Cmax)
					{
						matcher.Locals[CountIndex] = count + 1;
						bool result = Body.Match(matcher, i, seq);
						// If match failed we must backtrack, so
						// the loop count should NOT be incremented
						if (!result)
						{
							matcher.Locals[CountIndex] = count;
						}
						return result;
					}
					return false;
				}
				return Next.Match(matcher, i, seq);
			}
			internal override bool MatchInit(Matcher matcher, int i, CharSequence seq)
			{
				int save = matcher.Locals[CountIndex];
				bool ret = false;
				if (0 < Cmin)
				{
					matcher.Locals[CountIndex] = 1;
					ret = Body.Match(matcher, i, seq);
				}
				else if (Next.Match(matcher, i, seq))
				{
					ret = true;
				}
				else if (0 < Cmax)
				{
					matcher.Locals[CountIndex] = 1;
					ret = Body.Match(matcher, i, seq);
				}
				matcher.Locals[CountIndex] = save;
				return ret;
			}
			internal override bool Study(TreeInfo info)
			{
				info.MaxValid = false;
				info.Deterministic = false;
				return false;
			}
		}

		/// <summary>
		/// Refers to a group in the regular expression. Attempts to match
		/// whatever the group referred to last matched.
		/// </summary>
		internal class BackRef : Node
		{
			internal int GroupIndex;
			internal BackRef(int groupCount) : base()
			{
				GroupIndex = groupCount + groupCount;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int j = matcher.Groups[GroupIndex];
				int k = matcher.Groups[GroupIndex + 1];

				int groupSize = k - j;
				// If the referenced group didn't match, neither can this
				if (j < 0)
				{
					return false;
				}

				// If there isn't enough input left no match
				if (i + groupSize > matcher.To)
				{
					matcher.HitEnd_Renamed = true;
					return false;
				}
				// Check each new char to make sure it matches what the group
				// referenced matched last time around
				for (int index = 0; index < groupSize; index++)
				{
					if (seq.CharAt(i + index) != seq.CharAt(j + index))
					{
						return false;
					}
				}

				return Next.Match(matcher, i + groupSize, seq);
			}
			internal override bool Study(TreeInfo info)
			{
				info.MaxValid = false;
				return Next.Study(info);
			}
		}

		internal class CIBackRef : Node
		{
			internal int GroupIndex;
			internal bool DoUnicodeCase;
			internal CIBackRef(int groupCount, bool doUnicodeCase) : base()
			{
				GroupIndex = groupCount + groupCount;
				this.DoUnicodeCase = doUnicodeCase;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int j = matcher.Groups[GroupIndex];
				int k = matcher.Groups[GroupIndex + 1];

				int groupSize = k - j;

				// If the referenced group didn't match, neither can this
				if (j < 0)
				{
					return false;
				}

				// If there isn't enough input left no match
				if (i + groupSize > matcher.To)
				{
					matcher.HitEnd_Renamed = true;
					return false;
				}

				// Check each new char to make sure it matches what the group
				// referenced matched last time around
				int x = i;
				for (int index = 0; index < groupSize; index++)
				{
					int c1 = Character.CodePointAt(seq, x);
					int c2 = Character.CodePointAt(seq, j);
					if (c1 != c2)
					{
						if (DoUnicodeCase)
						{
							int cc1 = char.ToUpper(c1);
							int cc2 = char.ToUpper(c2);
							if (cc1 != cc2 && char.ToLower(cc1) != char.ToLower(cc2))
							{
								return false;
							}
						}
						else
						{
							if (ASCII.ToLower(c1) != ASCII.ToLower(c2))
							{
								return false;
							}
						}
					}
					x += Character.CharCount(c1);
					j += Character.CharCount(c2);
				}

				return Next.Match(matcher, i + groupSize, seq);
			}
			internal override bool Study(TreeInfo info)
			{
				info.MaxValid = false;
				return Next.Study(info);
			}
		}

		/// <summary>
		/// Searches until the next instance of its atom. This is useful for
		/// finding the atom efficiently without passing an instance of it
		/// (greedy problem) and without a lot of wasted search time (reluctant
		/// problem).
		/// </summary>
		internal sealed class First : Node
		{
			internal Node Atom;
			internal First(Node node)
			{
				this.Atom = BnM.Optimize(node);
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				if (Atom is BnM)
				{
					return Atom.Match(matcher, i, seq) && Next.Match(matcher, matcher.Last, seq);
				}
				for (;;)
				{
					if (i > matcher.To)
					{
						matcher.HitEnd_Renamed = true;
						return false;
					}
					if (Atom.Match(matcher, i, seq))
					{
						return Next.Match(matcher, matcher.Last, seq);
					}
					i += CountChars(seq, i, 1);
					matcher.First++;
				}
			}
			internal override bool Study(TreeInfo info)
			{
				Atom.Study(info);
				info.MaxValid = false;
				info.Deterministic = false;
				return Next.Study(info);
			}
		}

		internal sealed class Conditional : Node
		{
			internal Node Cond, Yes, Not;
			internal Conditional(Node cond, Node yes, Node not)
			{
				this.Cond = cond;
				this.Yes = yes;
				this.Not = not;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				if (Cond.Match(matcher, i, seq))
				{
					return Yes.Match(matcher, i, seq);
				}
				else
				{
					return Not.Match(matcher, i, seq);
				}
			}
			internal override bool Study(TreeInfo info)
			{
				int minL = info.MinLength;
				int maxL = info.MaxLength;
				bool maxV = info.MaxValid;
				info.Reset();
				Yes.Study(info);

				int minL2 = info.MinLength;
				int maxL2 = info.MaxLength;
				bool maxV2 = info.MaxValid;
				info.Reset();
				Not.Study(info);

				info.MinLength = minL + System.Math.Min(minL2, info.MinLength);
				info.MaxLength = maxL + System.Math.Max(maxL2, info.MaxLength);
				info.MaxValid = (maxV & maxV2 & info.MaxValid);
				info.Deterministic = false;
				return Next.Study(info);
			}
		}

		/// <summary>
		/// Zero width positive lookahead.
		/// </summary>
		internal sealed class Pos : Node
		{
			internal Node Cond;
			internal Pos(Node cond)
			{
				this.Cond = cond;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int savedTo = matcher.To;
				bool conditionMatched = false;

				// Relax transparent region boundaries for lookahead
				if (matcher.TransparentBounds)
				{
					matcher.To = matcher.TextLength;
				}
				try
				{
					conditionMatched = Cond.Match(matcher, i, seq);
				}
				finally
				{
					// Reinstate region boundaries
					matcher.To = savedTo;
				}
				return conditionMatched && Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// Zero width negative lookahead.
		/// </summary>
		internal sealed class Neg : Node
		{
			internal Node Cond;
			internal Neg(Node cond)
			{
				this.Cond = cond;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int savedTo = matcher.To;
				bool conditionMatched = false;

				// Relax transparent region boundaries for lookahead
				if (matcher.TransparentBounds)
				{
					matcher.To = matcher.TextLength;
				}
				try
				{
					if (i < matcher.To)
					{
						conditionMatched = !Cond.Match(matcher, i, seq);
					}
					else
					{
						// If a negative lookahead succeeds then more input
						// could cause it to fail!
						matcher.RequireEnd_Renamed = true;
						conditionMatched = !Cond.Match(matcher, i, seq);
					}
				}
				finally
				{
					// Reinstate region boundaries
					matcher.To = savedTo;
				}
				return conditionMatched && Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// For use with lookbehinds; matches the position where the lookbehind
		/// was encountered.
		/// </summary>
		internal static Node lookbehindEnd = new NodeAnonymousInnerClassHelper();

		private class NodeAnonymousInnerClassHelper : Node
		{
			public NodeAnonymousInnerClassHelper()
			{
			}

			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				return i == matcher.LookbehindTo;
			}
		}

		/// <summary>
		/// Zero width positive lookbehind.
		/// </summary>
		internal class Behind : Node
		{
			internal Node Cond;
			internal int Rmax, Rmin;
			internal Behind(Node cond, int rmax, int rmin)
			{
				this.Cond = cond;
				this.Rmax = rmax;
				this.Rmin = rmin;
			}

			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int savedFrom = matcher.From;
				bool conditionMatched = false;
				int startIndex = (!matcher.TransparentBounds) ? matcher.From : 0;
				int from = System.Math.Max(i - Rmax, startIndex);
				// Set end boundary
				int savedLBT = matcher.LookbehindTo;
				matcher.LookbehindTo = i;
				// Relax transparent region boundaries for lookbehind
				if (matcher.TransparentBounds)
				{
					matcher.From = 0;
				}
				for (int j = i - Rmin; !conditionMatched && j >= from; j--)
				{
					conditionMatched = Cond.Match(matcher, j, seq);
				}
				matcher.From = savedFrom;
				matcher.LookbehindTo = savedLBT;
				return conditionMatched && Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// Zero width positive lookbehind, including supplementary
		/// characters or unpaired surrogates.
		/// </summary>
		internal sealed class BehindS : Behind
		{
			internal BehindS(Node cond, int rmax, int rmin) : base(cond, rmax, rmin)
			{
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int rmaxChars = CountChars(seq, i, -Rmax);
				int rminChars = CountChars(seq, i, -Rmin);
				int savedFrom = matcher.From;
				int startIndex = (!matcher.TransparentBounds) ? matcher.From : 0;
				bool conditionMatched = false;
				int from = System.Math.Max(i - rmaxChars, startIndex);
				// Set end boundary
				int savedLBT = matcher.LookbehindTo;
				matcher.LookbehindTo = i;
				// Relax transparent region boundaries for lookbehind
				if (matcher.TransparentBounds)
				{
					matcher.From = 0;
				}

				for (int j = i - rminChars; !conditionMatched && j >= from; j -= j > from ? CountChars(seq, j, -1) : 1)
				{
					conditionMatched = Cond.Match(matcher, j, seq);
				}
				matcher.From = savedFrom;
				matcher.LookbehindTo = savedLBT;
				return conditionMatched && Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// Zero width negative lookbehind.
		/// </summary>
		internal class NotBehind : Node
		{
			internal Node Cond;
			internal int Rmax, Rmin;
			internal NotBehind(Node cond, int rmax, int rmin)
			{
				this.Cond = cond;
				this.Rmax = rmax;
				this.Rmin = rmin;
			}

			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int savedLBT = matcher.LookbehindTo;
				int savedFrom = matcher.From;
				bool conditionMatched = false;
				int startIndex = (!matcher.TransparentBounds) ? matcher.From : 0;
				int from = System.Math.Max(i - Rmax, startIndex);
				matcher.LookbehindTo = i;
				// Relax transparent region boundaries for lookbehind
				if (matcher.TransparentBounds)
				{
					matcher.From = 0;
				}
				for (int j = i - Rmin; !conditionMatched && j >= from; j--)
				{
					conditionMatched = Cond.Match(matcher, j, seq);
				}
				// Reinstate region boundaries
				matcher.From = savedFrom;
				matcher.LookbehindTo = savedLBT;
				return !conditionMatched && Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// Zero width negative lookbehind, including supplementary
		/// characters or unpaired surrogates.
		/// </summary>
		internal sealed class NotBehindS : NotBehind
		{
			internal NotBehindS(Node cond, int rmax, int rmin) : base(cond, rmax, rmin)
			{
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int rmaxChars = CountChars(seq, i, -Rmax);
				int rminChars = CountChars(seq, i, -Rmin);
				int savedFrom = matcher.From;
				int savedLBT = matcher.LookbehindTo;
				bool conditionMatched = false;
				int startIndex = (!matcher.TransparentBounds) ? matcher.From : 0;
				int from = System.Math.Max(i - rmaxChars, startIndex);
				matcher.LookbehindTo = i;
				// Relax transparent region boundaries for lookbehind
				if (matcher.TransparentBounds)
				{
					matcher.From = 0;
				}
				for (int j = i - rminChars; !conditionMatched && j >= from; j -= j > from ? CountChars(seq, j, -1) : 1)
				{
					conditionMatched = Cond.Match(matcher, j, seq);
				}
				//Reinstate region boundaries
				matcher.From = savedFrom;
				matcher.LookbehindTo = savedLBT;
				return !conditionMatched && Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// Returns the set union of two CharProperty nodes.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static CharProperty union(final CharProperty lhs, final CharProperty rhs)
		private static CharProperty Union(CharProperty lhs, CharProperty rhs)
		{
			return new CharPropertyAnonymousInnerClassHelper(lhs, rhs);
		}

		private class CharPropertyAnonymousInnerClassHelper : CharProperty
		{
			private java.util.regex.Pattern.CharProperty Lhs;
			private java.util.regex.Pattern.CharProperty Rhs;

			public CharPropertyAnonymousInnerClassHelper(java.util.regex.Pattern.CharProperty lhs, java.util.regex.Pattern.CharProperty rhs)
			{
				this.Lhs = lhs;
				this.Rhs = rhs;
			}

			internal override bool IsSatisfiedBy(int ch)
			{
				return Lhs.IsSatisfiedBy(ch) || Rhs.IsSatisfiedBy(ch);
			}
		}

		/// <summary>
		/// Returns the set intersection of two CharProperty nodes.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static CharProperty intersection(final CharProperty lhs, final CharProperty rhs)
		private static CharProperty Intersection(CharProperty lhs, CharProperty rhs)
		{
			return new CharPropertyAnonymousInnerClassHelper2(lhs, rhs);
		}

		private class CharPropertyAnonymousInnerClassHelper2 : CharProperty
		{
			private java.util.regex.Pattern.CharProperty Lhs;
			private java.util.regex.Pattern.CharProperty Rhs;

			public CharPropertyAnonymousInnerClassHelper2(java.util.regex.Pattern.CharProperty lhs, java.util.regex.Pattern.CharProperty rhs)
			{
				this.Lhs = lhs;
				this.Rhs = rhs;
			}

			internal override bool IsSatisfiedBy(int ch)
			{
				return Lhs.IsSatisfiedBy(ch) && Rhs.IsSatisfiedBy(ch);
			}
		}

		/// <summary>
		/// Returns the set difference of two CharProperty nodes.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static CharProperty setDifference(final CharProperty lhs, final CharProperty rhs)
		private static CharProperty SetDifference(CharProperty lhs, CharProperty rhs)
		{
			return new CharPropertyAnonymousInnerClassHelper3(lhs, rhs);
		}

		private class CharPropertyAnonymousInnerClassHelper3 : CharProperty
		{
			private java.util.regex.Pattern.CharProperty Lhs;
			private java.util.regex.Pattern.CharProperty Rhs;

			public CharPropertyAnonymousInnerClassHelper3(java.util.regex.Pattern.CharProperty lhs, java.util.regex.Pattern.CharProperty rhs)
			{
				this.Lhs = lhs;
				this.Rhs = rhs;
			}

			internal override bool IsSatisfiedBy(int ch)
			{
				return !Rhs.IsSatisfiedBy(ch) && Lhs.IsSatisfiedBy(ch);
			}
		}

		/// <summary>
		/// Handles word boundaries. Includes a field to allow this one class to
		/// deal with the different types of word boundaries we can match. The word
		/// characters include underscores, letters, and digits. Non spacing marks
		/// can are also part of a word if they have a base character, otherwise
		/// they are ignored for purposes of finding word boundaries.
		/// </summary>
		internal sealed class Bound : Node
		{
			internal static int LEFT = 0x1;
			internal static int RIGHT = 0x2;
			internal static int BOTH = 0x3;
			internal static int NONE = 0x4;
			internal int Type;
			internal bool UseUWORD;
			internal Bound(int n, bool useUWORD)
			{
				Type = n;
				this.UseUWORD = useUWORD;
			}

			internal bool IsWord(int ch)
			{
				return UseUWORD ? UnicodeProp.WORD.@is(ch) : (ch == '_' || char.IsLetterOrDigit(ch));
			}

			internal int Check(Matcher matcher, int i, CharSequence seq)
			{
				int ch;
				bool left = false;
				int startIndex = matcher.From;
				int endIndex = matcher.To;
				if (matcher.TransparentBounds)
				{
					startIndex = 0;
					endIndex = matcher.TextLength;
				}
				if (i > startIndex)
				{
					ch = Character.CodePointBefore(seq, i);
					left = (IsWord(ch) || ((Character.GetType(ch) == Character.NON_SPACING_MARK) && HasBaseCharacter(matcher, i - 1, seq)));
				}
				bool right = false;
				if (i < endIndex)
				{
					ch = Character.CodePointAt(seq, i);
					right = (IsWord(ch) || ((Character.GetType(ch) == Character.NON_SPACING_MARK) && HasBaseCharacter(matcher, i, seq)));
				}
				else
				{
					// Tried to access char past the end
					matcher.HitEnd_Renamed = true;
					// The addition of another char could wreck a boundary
					matcher.RequireEnd_Renamed = true;
				}
				return ((left ^ right) ? (right ? LEFT : RIGHT) : NONE);
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				return (Check(matcher, i, seq) & Type) > 0 && Next.Match(matcher, i, seq);
			}
		}

		/// <summary>
		/// Non spacing marks only count as word characters in bounds calculations
		/// if they have a base character.
		/// </summary>
		private static bool HasBaseCharacter(Matcher matcher, int i, CharSequence seq)
		{
			int start = (!matcher.TransparentBounds) ? matcher.From : 0;
			for (int x = i; x >= start; x--)
			{
				int ch = Character.CodePointAt(seq, x);
				if (char.IsLetterOrDigit(ch))
				{
					return true;
				}
				if (Character.GetType(ch) == Character.NON_SPACING_MARK)
				{
					continue;
				}
				return false;
			}
			return false;
		}

		/// <summary>
		/// Attempts to match a slice in the input using the Boyer-Moore string
		/// matching algorithm. The algorithm is based on the idea that the
		/// pattern can be shifted farther ahead in the search text if it is
		/// matched right to left.
		/// <para>
		/// The pattern is compared to the input one character at a time, from
		/// the rightmost character in the pattern to the left. If the characters
		/// all match the pattern has been found. If a character does not match,
		/// the pattern is shifted right a distance that is the maximum of two
		/// functions, the bad character shift and the good suffix shift. This
		/// shift moves the attempted match position through the input more
		/// quickly than a naive one position at a time check.
		/// </para>
		/// <para>
		/// The bad character shift is based on the character from the text that
		/// did not match. If the character does not appear in the pattern, the
		/// pattern can be shifted completely beyond the bad character. If the
		/// character does occur in the pattern, the pattern can be shifted to
		/// line the pattern up with the next occurrence of that character.
		/// </para>
		/// <para>
		/// The good suffix shift is based on the idea that some subset on the right
		/// side of the pattern has matched. When a bad character is found, the
		/// pattern can be shifted right by the pattern length if the subset does
		/// not occur again in pattern, or by the amount of distance to the
		/// next occurrence of the subset in the pattern.
		/// 
		/// Boyer-Moore search methods adapted from code by Amy Yu.
		/// </para>
		/// </summary>
		internal class BnM : Node
		{
			internal int[] Buffer;
			internal int[] LastOcc;
			internal int[] OptoSft;

			/// <summary>
			/// Pre calculates arrays needed to generate the bad character
			/// shift and the good suffix shift. Only the last seven bits
			/// are used to see if chars match; This keeps the tables small
			/// and covers the heavily used ASCII range, but occasionally
			/// results in an aliased match for the bad character shift.
			/// </summary>
			internal static Node Optimize(Node node)
			{
				if (!(node is Slice))
				{
					return node;
				}

				int[] src = ((Slice) node).Buffer;
				int patternLength = src.Length;
				// The BM algorithm requires a bit of overhead;
				// If the pattern is short don't use it, since
				// a shift larger than the pattern length cannot
				// be used anyway.
				if (patternLength < 4)
				{
					return node;
				}
				int i, j, k;
				int[] lastOcc = new int[128];
				int[] optoSft = new int[patternLength];
				// Precalculate part of the bad character shift
				// It is a table for where in the pattern each
				// lower 7-bit value occurs
				for (i = 0; i < patternLength; i++)
				{
					lastOcc[src[i] & 0x7F] = i + 1;
				}
				// Precalculate the good suffix shift
				// i is the shift amount being considered
	for (i = patternLength; i > 0; i--)
	{
					// j is the beginning index of suffix being considered
					for (j = patternLength - 1; j >= i; j--)
					{
						// Testing for good suffix
						if (src[j] == src[j - i])
						{
							// src[j..len] is a good suffix
							optoSft[j - 1] = i;
						}
						else
						{
							// No match. The array has already been
							// filled up with correct values before.
							goto NEXTContinue;
						}
					}
					// This fills up the remaining of optoSft
					// any suffix can not have larger shift amount
					// then its sub-suffix. Why???
					while (j > 0)
					{
						optoSft[--j] = i;
					}
		NEXTContinue:;
	}
	NEXTBreak:
				// Set the guard value because of unicode compression
				optoSft[patternLength - 1] = 1;
				if (node is SliceS)
				{
					return new BnMS(src, lastOcc, optoSft, node.Next);
				}
				return new BnM(src, lastOcc, optoSft, node.Next);
			}
			internal BnM(int[] src, int[] lastOcc, int[] optoSft, Node next)
			{
				this.Buffer = src;
				this.LastOcc = lastOcc;
				this.OptoSft = optoSft;
				this.Next = next;
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int[] src = Buffer;
				int patternLength = src.Length;
				int last = matcher.To - patternLength;

				// Loop over all possible match positions in text
	while (i <= last)
	{
					// Loop over pattern from right to left
					for (int j = patternLength - 1; j >= 0; j--)
					{
						int ch = seq.CharAt(i + j);
						if (ch != src[j])
						{
							// Shift search to the right by the maximum of the
							// bad character shift and the good suffix shift
							i += System.Math.Max(j + 1 - LastOcc[ch & 0x7F], OptoSft[j]);
							goto NEXTContinue;
						}
					}
					// Entire pattern matched starting at i
					matcher.First = i;
					bool ret = Next.Match(matcher, i + patternLength, seq);
					if (ret)
					{
						matcher.First = i;
						matcher.Groups[0] = matcher.First;
						matcher.Groups[1] = matcher.Last;
						return true;
					}
					i++;
		NEXTContinue:;
	}
	NEXTBreak:
				// BnM is only used as the leading node in the unanchored case,
				// and it replaced its Start() which always searches to the end
				// if it doesn't find what it's looking for, so hitEnd is true.
				matcher.HitEnd_Renamed = true;
				return false;
			}
			internal override bool Study(TreeInfo info)
			{
				info.MinLength += Buffer.Length;
				info.MaxValid = false;
				return Next.Study(info);
			}
		}

		/// <summary>
		/// Supplementary support version of BnM(). Unpaired surrogates are
		/// also handled by this class.
		/// </summary>
		internal sealed class BnMS : BnM
		{
			internal int LengthInChars;

			internal BnMS(int[] src, int[] lastOcc, int[] optoSft, Node next) : base(src, lastOcc, optoSft, next)
			{
				for (int x = 0; x < Buffer.Length; x++)
				{
					LengthInChars += Character.CharCount(Buffer[x]);
				}
			}
			internal override bool Match(Matcher matcher, int i, CharSequence seq)
			{
				int[] src = Buffer;
				int patternLength = src.Length;
				int last = matcher.To - LengthInChars;

				// Loop over all possible match positions in text
	while (i <= last)
	{
					// Loop over pattern from right to left
					int ch;
					for (int j = CountChars(seq, i, patternLength), x = patternLength - 1; j > 0; j -= Character.CharCount(ch), x--)
					{
						ch = Character.CodePointBefore(seq, i + j);
						if (ch != src[x])
						{
							// Shift search to the right by the maximum of the
							// bad character shift and the good suffix shift
							int n = System.Math.Max(x + 1 - LastOcc[ch & 0x7F], OptoSft[x]);
							i += CountChars(seq, i, n);
							goto NEXTContinue;
						}
					}
					// Entire pattern matched starting at i
					matcher.First = i;
					bool ret = Next.Match(matcher, i + LengthInChars, seq);
					if (ret)
					{
						matcher.First = i;
						matcher.Groups[0] = matcher.First;
						matcher.Groups[1] = matcher.Last;
						return true;
					}
					i += CountChars(seq, i, 1);
		NEXTContinue:;
	}
	NEXTBreak:
				matcher.HitEnd_Renamed = true;
				return false;
			}
		}

	///////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////

		/// <summary>
		///  This must be the very first initializer.
		/// </summary>
		internal static Node Accept_Renamed = new Node();

		internal static Node LastAccept = new LastNode();

		private class CharPropertyNames
		{

			internal static CharProperty CharPropertyFor(String name)
			{
				CharPropertyFactory m = Map.Get(name);
				return m == null ? null : m.Make();
			}

			private abstract class CharPropertyFactory
			{
				internal abstract CharProperty Make();
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void defCategory(String name, final int typeMask)
			internal static void DefCategory(String name, int typeMask)
			{
				Map.Put(name, new CharPropertyFactoryAnonymousInnerClassHelper(typeMask));
			}

			private class CharPropertyFactoryAnonymousInnerClassHelper : CharPropertyFactory
			{
				private int TypeMask;

				public CharPropertyFactoryAnonymousInnerClassHelper(int typeMask)
				{
					this.TypeMask = typeMask;
				}

				internal override CharProperty Make()
				{
					return new Category(TypeMask);
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void defRange(String name, final int lower, final int upper)
			internal static void DefRange(String name, int lower, int upper)
			{
				Map.Put(name, new CharPropertyFactoryAnonymousInnerClassHelper2(lower, upper));
			}

			private class CharPropertyFactoryAnonymousInnerClassHelper2 : CharPropertyFactory
			{
				private int Lower;
				private int Upper;

				public CharPropertyFactoryAnonymousInnerClassHelper2(int lower, int upper)
				{
					this.Lower = lower;
					this.Upper = upper;
				}

				internal override CharProperty Make()
				{
					return RangeFor(Lower, Upper);
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void defCtype(String name, final int ctype)
			internal static void DefCtype(String name, int ctype)
			{
				Map.Put(name, new CharPropertyFactoryAnonymousInnerClassHelper3(ctype));
			}

			private class CharPropertyFactoryAnonymousInnerClassHelper3 : CharPropertyFactory
			{
				private int Ctype;

				public CharPropertyFactoryAnonymousInnerClassHelper3(int ctype)
				{
					this.Ctype = ctype;
				}

				internal override CharProperty Make()
				{
					return new Ctype(Ctype);
				}
			}

			private abstract class CloneableProperty : CharProperty, Cloneable
			{
				public virtual CloneableProperty Clone()
				{
					try
					{
						return (CloneableProperty) base.Clone();
					}
					catch (CloneNotSupportedException e)
					{
						throw new AssertionError(e);
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void defClone(String name, final CloneableProperty p)
			internal static void DefClone(String name, CloneableProperty p)
			{
				Map.Put(name, new CharPropertyFactoryAnonymousInnerClassHelper(p));
			}

			private class CharPropertyFactoryAnonymousInnerClassHelper : CharPropertyFactory
			{
				private java.util.regex.Pattern.CharPropertyNames.CloneableProperty p;

				public CharPropertyFactoryAnonymousInnerClassHelper(java.util.regex.Pattern.CharPropertyNames.CloneableProperty p)
				{
					this.p = p;
				}

				internal override CharProperty Make()
				{
					return p.Clone();
				}
			}

			internal static readonly Dictionary<String, CharPropertyFactory> Map = new Dictionary<String, CharPropertyFactory>();

			static CharPropertyNames()
			{
				// Unicode character property aliases, defined in
				// http://www.unicode.org/Public/UNIDATA/PropertyValueAliases.txt
				DefCategory("Cn", 1 << Character.UNASSIGNED);
				DefCategory("Lu", 1 << Character.UPPERCASE_LETTER);
				DefCategory("Ll", 1 << Character.LOWERCASE_LETTER);
				DefCategory("Lt", 1 << Character.TITLECASE_LETTER);
				DefCategory("Lm", 1 << Character.MODIFIER_LETTER);
				DefCategory("Lo", 1 << Character.OTHER_LETTER);
				DefCategory("Mn", 1 << Character.NON_SPACING_MARK);
				DefCategory("Me", 1 << Character.ENCLOSING_MARK);
				DefCategory("Mc", 1 << Character.COMBINING_SPACING_MARK);
				DefCategory("Nd", 1 << Character.DECIMAL_DIGIT_NUMBER);
				DefCategory("Nl", 1 << Character.LETTER_NUMBER);
				DefCategory("No", 1 << Character.OTHER_NUMBER);
				DefCategory("Zs", 1 << Character.SPACE_SEPARATOR);
				DefCategory("Zl", 1 << Character.LINE_SEPARATOR);
				DefCategory("Zp", 1 << Character.PARAGRAPH_SEPARATOR);
				DefCategory("Cc", 1 << Character.CONTROL);
				DefCategory("Cf", 1 << Character.FORMAT);
				DefCategory("Co", 1 << Character.PRIVATE_USE);
				DefCategory("Cs", 1 << Character.SURROGATE);
				DefCategory("Pd", 1 << Character.DASH_PUNCTUATION);
				DefCategory("Ps", 1 << Character.START_PUNCTUATION);
				DefCategory("Pe", 1 << Character.END_PUNCTUATION);
				DefCategory("Pc", 1 << Character.CONNECTOR_PUNCTUATION);
				DefCategory("Po", 1 << Character.OTHER_PUNCTUATION);
				DefCategory("Sm", 1 << Character.MATH_SYMBOL);
				DefCategory("Sc", 1 << Character.CURRENCY_SYMBOL);
				DefCategory("Sk", 1 << Character.MODIFIER_SYMBOL);
				DefCategory("So", 1 << Character.OTHER_SYMBOL);
				DefCategory("Pi", 1 << Character.INITIAL_QUOTE_PUNCTUATION);
				DefCategory("Pf", 1 << Character.FINAL_QUOTE_PUNCTUATION);
				DefCategory("L", ((1 << Character.UPPERCASE_LETTER) | (1 << Character.LOWERCASE_LETTER) | (1 << Character.TITLECASE_LETTER) | (1 << Character.MODIFIER_LETTER) | (1 << Character.OTHER_LETTER)));
				DefCategory("M", ((1 << Character.NON_SPACING_MARK) | (1 << Character.ENCLOSING_MARK) | (1 << Character.COMBINING_SPACING_MARK)));
				DefCategory("N", ((1 << Character.DECIMAL_DIGIT_NUMBER) | (1 << Character.LETTER_NUMBER) | (1 << Character.OTHER_NUMBER)));
				DefCategory("Z", ((1 << Character.SPACE_SEPARATOR) | (1 << Character.LINE_SEPARATOR) | (1 << Character.PARAGRAPH_SEPARATOR)));
				DefCategory("C", ((1 << Character.CONTROL) | (1 << Character.FORMAT) | (1 << Character.PRIVATE_USE) | (1 << Character.SURROGATE))); // Other
				DefCategory("P", ((1 << Character.DASH_PUNCTUATION) | (1 << Character.START_PUNCTUATION) | (1 << Character.END_PUNCTUATION) | (1 << Character.CONNECTOR_PUNCTUATION) | (1 << Character.OTHER_PUNCTUATION) | (1 << Character.INITIAL_QUOTE_PUNCTUATION) | (1 << Character.FINAL_QUOTE_PUNCTUATION)));
				DefCategory("S", ((1 << Character.MATH_SYMBOL) | (1 << Character.CURRENCY_SYMBOL) | (1 << Character.MODIFIER_SYMBOL) | (1 << Character.OTHER_SYMBOL)));
				DefCategory("LC", ((1 << Character.UPPERCASE_LETTER) | (1 << Character.LOWERCASE_LETTER) | (1 << Character.TITLECASE_LETTER)));
				DefCategory("LD", ((1 << Character.UPPERCASE_LETTER) | (1 << Character.LOWERCASE_LETTER) | (1 << Character.TITLECASE_LETTER) | (1 << Character.MODIFIER_LETTER) | (1 << Character.OTHER_LETTER) | (1 << Character.DECIMAL_DIGIT_NUMBER)));
				DefRange("L1", 0x00, 0xFF); // Latin-1
				Map.Put("all", new CharPropertyFactoryAnonymousInnerClassHelper2());

				// Posix regular expression character classes, defined in
				// http://www.unix.org/onlinepubs/009695399/basedefs/xbd_chap09.html
				DefRange("ASCII", 0x00, 0x7F); // ASCII
				DefCtype("Alnum", ASCII.ALNUM); // Alphanumeric characters
				DefCtype("Alpha", ASCII.ALPHA); // Alphabetic characters
				DefCtype("Blank", ASCII.BLANK); // Space and tab characters
				DefCtype("Cntrl", ASCII.CNTRL); // Control characters
				DefRange("Digit", '0', '9'); // Numeric characters
				DefCtype("Graph", ASCII.GRAPH); // printable and visible
				DefRange("Lower", 'a', 'z'); // Lower-case alphabetic
				DefRange("Print", 0x20, 0x7E); // Printable characters
				DefCtype("Punct", ASCII.PUNCT); // Punctuation characters
				DefCtype("Space", ASCII.SPACE); // Space characters
				DefRange("Upper", 'A', 'Z'); // Upper-case alphabetic
				DefCtype("XDigit",ASCII.XDIGIT); // hexadecimal digits

				// Java character properties, defined by methods in Character.java
				DefClone("javaLowerCase", new CloneablePropertyAnonymousInnerClassHelper());
				DefClone("javaUpperCase", new CloneablePropertyAnonymousInnerClassHelper2());
				DefClone("javaAlphabetic", new CloneablePropertyAnonymousInnerClassHelper3());
				DefClone("javaIdeographic", new CloneablePropertyAnonymousInnerClassHelper4());
				DefClone("javaTitleCase", new CloneablePropertyAnonymousInnerClassHelper5());
				DefClone("javaDigit", new CloneablePropertyAnonymousInnerClassHelper6());
				DefClone("javaDefined", new CloneablePropertyAnonymousInnerClassHelper7());
				DefClone("javaLetter", new CloneablePropertyAnonymousInnerClassHelper8());
				DefClone("javaLetterOrDigit", new CloneablePropertyAnonymousInnerClassHelper9());
				DefClone("javaJavaIdentifierStart", new CloneablePropertyAnonymousInnerClassHelper10());
				DefClone("javaJavaIdentifierPart", new CloneablePropertyAnonymousInnerClassHelper11());
				DefClone("javaUnicodeIdentifierStart", new CloneablePropertyAnonymousInnerClassHelper12());
				DefClone("javaUnicodeIdentifierPart", new CloneablePropertyAnonymousInnerClassHelper13());
				DefClone("javaIdentifierIgnorable", new CloneablePropertyAnonymousInnerClassHelper14());
				DefClone("javaSpaceChar", new CloneablePropertyAnonymousInnerClassHelper15());
				DefClone("javaWhitespace", new CloneablePropertyAnonymousInnerClassHelper16());
				DefClone("javaISOControl", new CloneablePropertyAnonymousInnerClassHelper17());
				DefClone("javaMirrored", new CloneablePropertyAnonymousInnerClassHelper18());
			}

			private class CharPropertyFactoryAnonymousInnerClassHelper2 : CharPropertyFactory
			{
				public CharPropertyFactoryAnonymousInnerClassHelper2()
				{
				}

				internal override CharProperty Make()
				{
					return new All();
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return char.IsLower(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper2 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper2()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return char.IsUpper(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper3 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper3()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsAlphabetic(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper4 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper4()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsIdeographic(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper5 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper5()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsTitleCase(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper6 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper6()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return char.IsDigit(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper7 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper7()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsDefined(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper8 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper8()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return char.IsLetter(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper9 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper9()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return char.IsLetterOrDigit(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper10 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper10()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsJavaIdentifierStart(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper11 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper11()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsJavaIdentifierPart(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper12 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper12()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsUnicodeIdentifierStart(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper13 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper13()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsUnicodeIdentifierPart(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper14 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper14()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsIdentifierIgnorable(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper15 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper15()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsSpaceChar(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper16 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper16()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return char.IsWhiteSpace(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper17 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper17()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return char.IsControl(ch);
				}
			}

			private class CloneablePropertyAnonymousInnerClassHelper18 : CloneableProperty
			{
				public CloneablePropertyAnonymousInnerClassHelper18()
				{
				}

				internal override bool IsSatisfiedBy(int ch)
				{
					return Character.IsMirrored(ch);
				}
			}
		}

		/// <summary>
		/// Creates a predicate which can be used to match a string.
		/// </summary>
		/// <returns>  The predicate which can be used for matching on a string
		/// @since   1.8 </returns>
		public Predicate<String> AsPredicate()
		{
//JAVA TO C# CONVERTER TODO TASK: Java lambdas satisfy functional interfaces, while .NET lambdas satisfy delegates - change the appropriate interface to a delegate:
			return s => Matcher(s).Find();
		}

		/// <summary>
		/// Creates a stream from the given input sequence around matches of this
		/// pattern.
		/// 
		/// <para> The stream returned by this method contains each substring of the
		/// input sequence that is terminated by another subsequence that matches
		/// this pattern or is terminated by the end of the input sequence.  The
		/// substrings in the stream are in the order in which they occur in the
		/// input. Trailing empty strings will be discarded and not encountered in
		/// the stream.
		/// 
		/// </para>
		/// <para> If this pattern does not match any subsequence of the input then
		/// the resulting stream has just one element, namely the input sequence in
		/// string form.
		/// 
		/// </para>
		/// <para> When there is a positive-width match at the beginning of the input
		/// sequence then an empty leading substring is included at the beginning
		/// of the stream. A zero-width match at the beginning however never produces
		/// such empty leading substring.
		/// 
		/// </para>
		/// <para> If the input sequence is mutable, it must remain constant during the
		/// execution of the terminal stream operation.  Otherwise, the result of the
		/// terminal stream operation is undefined.
		/// 
		/// </para>
		/// </summary>
		/// <param name="input">
		///          The character sequence to be split
		/// </param>
		/// <returns>  The stream of strings computed by splitting the input
		///          around matches of this pattern </returns>
		/// <seealso cref=     #split(CharSequence)
		/// @since   1.8 </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public java.util.stream.Stream<String> splitAsStream(final CharSequence input)
		public Stream<String> SplitAsStream(CharSequence input)
		{
//JAVA TO C# CONVERTER TODO TASK: Local classes are not converted by Java to C# Converter:
//			class MatcherIterator implements java.util.Iterator<String>
	//		{
	//			private final Matcher matcher;
	//			// The start position of the next sub-sequence of input
	//			// when current == input.length there are no more elements
	//			private int current;
	//			// null if the next element, if any, needs to obtained
	//			private String nextElement;
	//			// > 0 if there are N next empty elements
	//			private int emptyElementCount;
	//
	//			MatcherIterator()
	//			{
	//				this.matcher = matcher(input);
	//			}
	//
	//			public String next()
	//			{
	//				if (!hasNext())
	//					throw new NoSuchElementException();
	//
	//				if (emptyElementCount == 0)
	//				{
	//					String n = nextElement;
	//					nextElement = null;
	//					return n;
	//				}
	//				else
	//				{
	//					emptyElementCount--;
	//					return "";
	//				}
	//			}
	//
	//			public boolean hasNext()
	//			{
	//				if (nextElement != null || emptyElementCount > 0)
	//					return true;
	//
	//				if (current == input.length())
	//					return false;
	//
	//				// Consume the next matching element
	//				// Count sequence of matching empty elements
	//				while (matcher.find())
	//				{
	//					nextElement = input.subSequence(current, matcher.start()).toString();
	//					current = matcher.end();
	//					if (!nextElement.isEmpty())
	//					{
	//						return true;
	//					} // no empty leading substring for zero-width
	//					else if (current > 0)
	//					{
	//											  // match at the beginning of the input
	//						emptyElementCount++;
	//					}
	//				}
	//
	//				// Consume last matching element
	//				nextElement = input.subSequence(current, input.length()).toString();
	//				current = input.length();
	//				if (!nextElement.isEmpty())
	//				{
	//					return true;
	//				}
	//				else
	//				{
	//					// Ignore a terminal sequence of matching empty elements
	//					emptyElementCount = 0;
	//					nextElement = null;
	//					return false;
	//				}
	//			}
	//		}
			return StreamSupport.Stream(Spliterators.SpliteratorUnknownSize(new MatcherIterator(), java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.NONNULL), false);
		}
	}

}