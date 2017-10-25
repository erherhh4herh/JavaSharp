using System;
using System.Collections.Generic;

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
 *   The original version of this source code and documentation is copyrighted
 * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
 * materials are provided under terms of a License Agreement between Taligent
 * and Sun. This technology is protected by multiple US and International
 * patents. This notice and attribution to Taligent may not be removed.
 *   Taligent is a registered trademark of Taligent, Inc.
 *
 */

namespace java.text
{



	/// <summary>
	/// <code>MessageFormat</code> provides a means to produce concatenated
	/// messages in a language-neutral way. Use this to construct messages
	/// displayed for end users.
	/// 
	/// <para>
	/// <code>MessageFormat</code> takes a set of objects, formats them, then
	/// inserts the formatted strings into the pattern at the appropriate places.
	/// 
	/// </para>
	/// <para>
	/// <strong>Note:</strong>
	/// <code>MessageFormat</code> differs from the other <code>Format</code>
	/// classes in that you create a <code>MessageFormat</code> object with one
	/// of its constructors (not with a <code>getInstance</code> style factory
	/// method). The factory methods aren't necessary because <code>MessageFormat</code>
	/// itself doesn't implement locale specific behavior. Any locale specific
	/// behavior is defined by the pattern that you provide as well as the
	/// subformats used for inserted arguments.
	/// 
	/// <h3><a name="patterns">Patterns and Their Interpretation</a></h3>
	/// 
	/// <code>MessageFormat</code> uses patterns of the following form:
	/// <blockquote><pre>
	/// <i>MessageFormatPattern:</i>
	///         <i>String</i>
	///         <i>MessageFormatPattern</i> <i>FormatElement</i> <i>String</i>
	/// 
	/// <i>FormatElement:</i>
	///         { <i>ArgumentIndex</i> }
	///         { <i>ArgumentIndex</i> , <i>FormatType</i> }
	///         { <i>ArgumentIndex</i> , <i>FormatType</i> , <i>FormatStyle</i> }
	/// 
	/// <i>FormatType: one of </i>
	///         number date time choice
	/// 
	/// <i>FormatStyle:</i>
	///         short
	///         medium
	///         long
	///         full
	///         integer
	///         currency
	///         percent
	///         <i>SubformatPattern</i>
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>Within a <i>String</i>, a pair of single quotes can be used to
	/// quote any arbitrary characters except single quotes. For example,
	/// pattern string <code>"'{0}'"</code> represents string
	/// <code>"{0}"</code>, not a <i>FormatElement</i>. A single quote itself
	/// must be represented by doubled single quotes {@code ''} throughout a
	/// <i>String</i>.  For example, pattern string <code>"'{''}'"</code> is
	/// interpreted as a sequence of <code>'{</code> (start of quoting and a
	/// left curly brace), <code>''</code> (a single quote), and
	/// <code>}'</code> (a right curly brace and end of quoting),
	/// <em>not</em> <code>'{'</code> and <code>'}'</code> (quoted left and
	/// right curly braces): representing string <code>"{'}"</code>,
	/// <em>not</em> <code>"{}"</code>.
	/// 
	/// </para>
	/// <para>A <i>SubformatPattern</i> is interpreted by its corresponding
	/// subformat, and subformat-dependent pattern rules apply. For example,
	/// pattern string <code>"{1,number,<u>$'#',##</u>}"</code>
	/// (<i>SubformatPattern</i> with underline) will produce a number format
	/// with the pound-sign quoted, with a result such as: {@code
	/// "$#31,45"}. Refer to each {@code Format} subclass documentation for
	/// details.
	/// 
	/// </para>
	/// <para>Any unmatched quote is treated as closed at the end of the given
	/// pattern. For example, pattern string {@code "'{0}"} is treated as
	/// pattern {@code "'{0}'"}.
	/// 
	/// </para>
	/// <para>Any curly braces within an unquoted pattern must be balanced. For
	/// example, <code>"ab {0} de"</code> and <code>"ab '}' de"</code> are
	/// valid patterns, but <code>"ab {0'}' de"</code>, <code>"ab } de"</code>
	/// and <code>"''{''"</code> are not.
	/// 
	/// <dl><dt><b>Warning:</b><dd>The rules for using quotes within message
	/// format patterns unfortunately have shown to be somewhat confusing.
	/// In particular, it isn't always obvious to localizers whether single
	/// quotes need to be doubled or not. Make sure to inform localizers about
	/// the rules, and tell them (for example, by using comments in resource
	/// bundle source files) which strings will be processed by {@code MessageFormat}.
	/// Note that localizers may need to use single quotes in translated
	/// strings where the original version doesn't have them.
	/// </dl>
	/// </para>
	/// <para>
	/// The <i>ArgumentIndex</i> value is a non-negative integer written
	/// using the digits {@code '0'} through {@code '9'}, and represents an index into the
	/// {@code arguments} array passed to the {@code format} methods
	/// or the result array returned by the {@code parse} methods.
	/// </para>
	/// <para>
	/// The <i>FormatType</i> and <i>FormatStyle</i> values are used to create
	/// a {@code Format} instance for the format element. The following
	/// table shows how the values map to {@code Format} instances. Combinations not
	/// shown in the table are illegal. A <i>SubformatPattern</i> must
	/// be a valid pattern string for the {@code Format} subclass used.
	/// 
	/// <table border=1 summary="Shows how FormatType and FormatStyle values map to Format instances">
	///    <tr>
	///       <th id="ft" class="TableHeadingColor">FormatType
	///       <th id="fs" class="TableHeadingColor">FormatStyle
	///       <th id="sc" class="TableHeadingColor">Subformat Created
	///    <tr>
	///       <td headers="ft"><i>(none)</i>
	///       <td headers="fs"><i>(none)</i>
	///       <td headers="sc"><code>null</code>
	///    <tr>
	///       <td headers="ft" rowspan=5><code>number</code>
	///       <td headers="fs"><i>(none)</i>
	///       <td headers="sc"><seealso cref="NumberFormat#getInstance(Locale) NumberFormat.getInstance"/>{@code (getLocale())}
	///    <tr>
	///       <td headers="fs"><code>integer</code>
	///       <td headers="sc"><seealso cref="NumberFormat#getIntegerInstance(Locale) NumberFormat.getIntegerInstance"/>{@code (getLocale())}
	///    <tr>
	///       <td headers="fs"><code>currency</code>
	///       <td headers="sc"><seealso cref="NumberFormat#getCurrencyInstance(Locale) NumberFormat.getCurrencyInstance"/>{@code (getLocale())}
	///    <tr>
	///       <td headers="fs"><code>percent</code>
	///       <td headers="sc"><seealso cref="NumberFormat#getPercentInstance(Locale) NumberFormat.getPercentInstance"/>{@code (getLocale())}
	///    <tr>
	///       <td headers="fs"><i>SubformatPattern</i>
	///       <td headers="sc">{@code new} <seealso cref="DecimalFormat#DecimalFormat(String,DecimalFormatSymbols) DecimalFormat"/>{@code (subformatPattern,} <seealso cref="DecimalFormatSymbols#getInstance(Locale) DecimalFormatSymbols.getInstance"/>{@code (getLocale()))}
	///    <tr>
	///       <td headers="ft" rowspan=6><code>date</code>
	///       <td headers="fs"><i>(none)</i>
	///       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#DEFAULT"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><code>short</code>
	///       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#SHORT"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><code>medium</code>
	///       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#DEFAULT"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><code>long</code>
	///       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#LONG"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><code>full</code>
	///       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#FULL"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><i>SubformatPattern</i>
	///       <td headers="sc">{@code new} <seealso cref="SimpleDateFormat#SimpleDateFormat(String,Locale) SimpleDateFormat"/>{@code (subformatPattern, getLocale())}
	///    <tr>
	///       <td headers="ft" rowspan=6><code>time</code>
	///       <td headers="fs"><i>(none)</i>
	///       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#DEFAULT"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><code>short</code>
	///       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#SHORT"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><code>medium</code>
	///       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#DEFAULT"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><code>long</code>
	///       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#LONG"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><code>full</code>
	///       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#FULL"/>{@code , getLocale())}
	///    <tr>
	///       <td headers="fs"><i>SubformatPattern</i>
	///       <td headers="sc">{@code new} <seealso cref="SimpleDateFormat#SimpleDateFormat(String,Locale) SimpleDateFormat"/>{@code (subformatPattern, getLocale())}
	///    <tr>
	///       <td headers="ft"><code>choice</code>
	///       <td headers="fs"><i>SubformatPattern</i>
	///       <td headers="sc">{@code new} <seealso cref="ChoiceFormat#ChoiceFormat(String) ChoiceFormat"/>{@code (subformatPattern)}
	/// </table>
	/// 
	/// <h4>Usage Information</h4>
	/// 
	/// </para>
	/// <para>
	/// Here are some examples of usage.
	/// In real internationalized programs, the message format pattern and other
	/// static strings will, of course, be obtained from resource bundles.
	/// Other parameters will be dynamically determined at runtime.
	/// </para>
	/// <para>
	/// The first example uses the static method <code>MessageFormat.format</code>,
	/// which internally creates a <code>MessageFormat</code> for one-time use:
	/// <blockquote><pre>
	/// int planet = 7;
	/// String event = "a disturbance in the Force";
	/// 
	/// String result = MessageFormat.format(
	///     "At {1,time} on {1,date}, there was {2} on planet {0,number,integer}.",
	///     planet, new Date(), event);
	/// </pre></blockquote>
	/// The output is:
	/// <blockquote><pre>
	/// At 12:30 PM on Jul 3, 2053, there was a disturbance in the Force on planet 7.
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>
	/// The following example creates a <code>MessageFormat</code> instance that
	/// can be used repeatedly:
	/// <blockquote><pre>
	/// int fileCount = 1273;
	/// String diskName = "MyDisk";
	/// Object[] testArgs = {new Long(fileCount), diskName};
	/// 
	/// MessageFormat form = new MessageFormat(
	///     "The disk \"{1}\" contains {0} file(s).");
	/// 
	/// System.out.println(form.format(testArgs));
	/// </pre></blockquote>
	/// The output with different values for <code>fileCount</code>:
	/// <blockquote><pre>
	/// The disk "MyDisk" contains 0 file(s).
	/// The disk "MyDisk" contains 1 file(s).
	/// The disk "MyDisk" contains 1,273 file(s).
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>
	/// For more sophisticated patterns, you can use a <code>ChoiceFormat</code>
	/// to produce correct forms for singular and plural:
	/// <blockquote><pre>
	/// MessageFormat form = new MessageFormat("The disk \"{1}\" contains {0}.");
	/// double[] filelimits = {0,1,2};
	/// String[] filepart = {"no files","one file","{0,number} files"};
	/// ChoiceFormat fileform = new ChoiceFormat(filelimits, filepart);
	/// form.setFormatByArgumentIndex(0, fileform);
	/// 
	/// int fileCount = 1273;
	/// String diskName = "MyDisk";
	/// Object[] testArgs = {new Long(fileCount), diskName};
	/// 
	/// System.out.println(form.format(testArgs));
	/// </pre></blockquote>
	/// The output with different values for <code>fileCount</code>:
	/// <blockquote><pre>
	/// The disk "MyDisk" contains no files.
	/// The disk "MyDisk" contains one file.
	/// The disk "MyDisk" contains 1,273 files.
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>
	/// You can create the <code>ChoiceFormat</code> programmatically, as in the
	/// above example, or by using a pattern. See <seealso cref="ChoiceFormat"/>
	/// for more information.
	/// <blockquote><pre>{@code
	/// form.applyPattern(
	///    "There {0,choice,0#are no files|1#is one file|1<are {0,number,integer} files}.");
	/// }</pre></blockquote>
	/// 
	/// </para>
	/// <para>
	/// <strong>Note:</strong> As we see above, the string produced
	/// by a <code>ChoiceFormat</code> in <code>MessageFormat</code> is treated as special;
	/// occurrences of '{' are used to indicate subformats, and cause recursion.
	/// If you create both a <code>MessageFormat</code> and <code>ChoiceFormat</code>
	/// programmatically (instead of using the string patterns), then be careful not to
	/// produce a format that recurses on itself, which will cause an infinite loop.
	/// </para>
	/// <para>
	/// When a single argument is parsed more than once in the string, the last match
	/// will be the final result of the parsing.  For example,
	/// <blockquote><pre>
	/// MessageFormat mf = new MessageFormat("{0,number,#.##}, {0,number,#.#}");
	/// Object[] objs = {new Double(3.1415)};
	/// String result = mf.format( objs );
	/// // result now equals "3.14, 3.1"
	/// objs = null;
	/// objs = mf.parse(result, new ParsePosition(0));
	/// // objs now equals {new Double(3.1)}
	/// </pre></blockquote>
	/// 
	/// </para>
	/// <para>
	/// Likewise, parsing with a {@code MessageFormat} object using patterns containing
	/// multiple occurrences of the same argument would return the last match.  For
	/// example,
	/// <blockquote><pre>
	/// MessageFormat mf = new MessageFormat("{0}, {0}, {0}");
	/// String forParsing = "x, y, z";
	/// Object[] objs = mf.parse(forParsing, new ParsePosition(0));
	/// // result now equals {new String("z")}
	/// </pre></blockquote>
	/// 
	/// <h4><a name="synchronization">Synchronization</a></h4>
	/// 
	/// </para>
	/// <para>
	/// Message formats are not synchronized.
	/// It is recommended to create separate format instances for each thread.
	/// If multiple threads access a format concurrently, it must be synchronized
	/// externally.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=          java.util.Locale </seealso>
	/// <seealso cref=          Format </seealso>
	/// <seealso cref=          NumberFormat </seealso>
	/// <seealso cref=          DecimalFormat </seealso>
	/// <seealso cref=          DecimalFormatSymbols </seealso>
	/// <seealso cref=          ChoiceFormat </seealso>
	/// <seealso cref=          DateFormat </seealso>
	/// <seealso cref=          SimpleDateFormat
	/// 
	/// @author       Mark Davis </seealso>

	public class MessageFormat : Format
	{

		private const long SerialVersionUID = 6479157306784022952L;

		/// <summary>
		/// Constructs a MessageFormat for the default
		/// <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale and the
		/// specified pattern.
		/// The constructor first sets the locale, then parses the pattern and
		/// creates a list of subformats for the format elements contained in it.
		/// Patterns and their interpretation are specified in the
		/// <a href="#patterns">class description</a>.
		/// </summary>
		/// <param name="pattern"> the pattern for this message format </param>
		/// <exception cref="IllegalArgumentException"> if the pattern is invalid </exception>
		public MessageFormat(String pattern)
		{
			this.Locale_Renamed = Locale.GetDefault(Locale.Category.FORMAT);
			ApplyPattern(pattern);
		}

		/// <summary>
		/// Constructs a MessageFormat for the specified locale and
		/// pattern.
		/// The constructor first sets the locale, then parses the pattern and
		/// creates a list of subformats for the format elements contained in it.
		/// Patterns and their interpretation are specified in the
		/// <a href="#patterns">class description</a>.
		/// </summary>
		/// <param name="pattern"> the pattern for this message format </param>
		/// <param name="locale"> the locale for this message format </param>
		/// <exception cref="IllegalArgumentException"> if the pattern is invalid
		/// @since 1.4 </exception>
		public MessageFormat(String pattern, Locale locale)
		{
			this.Locale_Renamed = locale;
			ApplyPattern(pattern);
		}

		/// <summary>
		/// Sets the locale to be used when creating or comparing subformats.
		/// This affects subsequent calls
		/// <ul>
		/// <li>to the <seealso cref="#applyPattern applyPattern"/>
		///     and <seealso cref="#toPattern toPattern"/> methods if format elements specify
		///     a format type and therefore have the subformats created in the
		///     <code>applyPattern</code> method, as well as
		/// <li>to the <code>format</code> and
		///     <seealso cref="#formatToCharacterIterator formatToCharacterIterator"/> methods
		///     if format elements do not specify a format type and therefore have
		///     the subformats created in the formatting methods.
		/// </ul>
		/// Subformats that have already been created are not affected.
		/// </summary>
		/// <param name="locale"> the locale to be used when creating or comparing subformats </param>
		public virtual Locale Locale
		{
			set
			{
				this.Locale_Renamed = value;
			}
			get
			{
				return Locale_Renamed;
			}
		}



		/// <summary>
		/// Sets the pattern used by this message format.
		/// The method parses the pattern and creates a list of subformats
		/// for the format elements contained in it.
		/// Patterns and their interpretation are specified in the
		/// <a href="#patterns">class description</a>.
		/// </summary>
		/// <param name="pattern"> the pattern for this message format </param>
		/// <exception cref="IllegalArgumentException"> if the pattern is invalid </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("fallthrough") public void applyPattern(String pattern)
		public virtual void ApplyPattern(String pattern) // fallthrough in switch is expected, suppress it
		{
				StringBuilder[] segments = new StringBuilder[4];
				// Allocate only segments[SEG_RAW] here. The rest are
				// allocated on demand.
				segments[SEG_RAW] = new StringBuilder();

				int part = SEG_RAW;
				int formatNumber = 0;
				bool inQuote = false;
				int braceStack = 0;
				MaxOffset = -1;
				for (int i = 0; i < pattern.Length(); ++i)
				{
					char ch = pattern.CharAt(i);
					if (part == SEG_RAW)
					{
						if (ch == '\'')
						{
							if (i + 1 < pattern.Length() && pattern.CharAt(i + 1) == '\'')
							{
								segments[part].Append(ch); // handle doubles
								++i;
							}
							else
							{
								inQuote = !inQuote;
							}
						}
						else if (ch == '{' && !inQuote)
						{
							part = SEG_INDEX;
							if (segments[SEG_INDEX] == null)
							{
								segments[SEG_INDEX] = new StringBuilder();
							}
						}
						else
						{
							segments[part].Append(ch);
						}
					}
					else
					{
						if (inQuote) // just copy quotes in parts
						{
							segments[part].Append(ch);
							if (ch == '\'')
							{
								inQuote = false;
							}
						}
						else
						{
							switch (ch)
							{
							case ',':
								if (part < SEG_MODIFIER)
								{
									if (segments[++part] == null)
									{
										segments[part] = new StringBuilder();
									}
								}
								else
								{
									segments[part].Append(ch);
								}
								break;
							case '{':
								++braceStack;
								segments[part].Append(ch);
								break;
							case '}':
								if (braceStack == 0)
								{
									part = SEG_RAW;
									MakeFormat(i, formatNumber, segments);
									formatNumber++;
									// throw away other segments
									segments[SEG_INDEX] = null;
									segments[SEG_TYPE] = null;
									segments[SEG_MODIFIER] = null;
								}
								else
								{
									--braceStack;
									segments[part].Append(ch);
								}
								break;
							case ' ':
								// Skip any leading space chars for SEG_TYPE.
								if (part != SEG_TYPE || segments[SEG_TYPE].Length() > 0)
								{
									segments[part].Append(ch);
								}
								break;
							case '\'':
								inQuote = true;
								// fall through, so we keep quotes in other parts
								goto default;
							default:
								segments[part].Append(ch);
								break;
							}
						}
					}
				}
				if (braceStack == 0 && part != 0)
				{
					MaxOffset = -1;
					throw new IllegalArgumentException("Unmatched braces in the pattern.");
				}
				this.Pattern = segments[0].ToString();
		}


		/// <summary>
		/// Returns a pattern representing the current state of the message format.
		/// The string is constructed from internal information and therefore
		/// does not necessarily equal the previously applied pattern.
		/// </summary>
		/// <returns> a pattern representing the current state of the message format </returns>
		public virtual String ToPattern()
		{
			// later, make this more extensible
			int lastOffset = 0;
			StringBuilder result = new StringBuilder();
			for (int i = 0; i <= MaxOffset; ++i)
			{
				CopyAndFixQuotes(Pattern, lastOffset, Offsets[i], result);
				lastOffset = Offsets[i];
				result.Append('{').Append(ArgumentNumbers[i]);
				Format fmt = Formats_Renamed[i];
				if (fmt == null)
				{
					// do nothing, string format
				}
				else if (fmt is NumberFormat)
				{
					if (fmt.Equals(NumberFormat.GetInstance(Locale_Renamed)))
					{
						result.Append(",number");
					}
					else if (fmt.Equals(NumberFormat.GetCurrencyInstance(Locale_Renamed)))
					{
						result.Append(",number,currency");
					}
					else if (fmt.Equals(NumberFormat.GetPercentInstance(Locale_Renamed)))
					{
						result.Append(",number,percent");
					}
					else if (fmt.Equals(NumberFormat.GetIntegerInstance(Locale_Renamed)))
					{
						result.Append(",number,integer");
					}
					else
					{
						if (fmt is DecimalFormat)
						{
							result.Append(",number,").Append(((DecimalFormat)fmt).ToPattern());
						}
						else if (fmt is ChoiceFormat)
						{
							result.Append(",choice,").Append(((ChoiceFormat)fmt).ToPattern());
						}
						else
						{
							// UNKNOWN
						}
					}
				}
				else if (fmt is DateFormat)
				{
					int index;
					for (index = MODIFIER_DEFAULT; index < DATE_TIME_MODIFIERS.Length; index++)
					{
						DateFormat df = DateFormat.GetDateInstance(DATE_TIME_MODIFIERS[index], Locale_Renamed);
						if (fmt.Equals(df))
						{
							result.Append(",date");
							break;
						}
						df = DateFormat.GetTimeInstance(DATE_TIME_MODIFIERS[index], Locale_Renamed);
						if (fmt.Equals(df))
						{
							result.Append(",time");
							break;
						}
					}
					if (index >= DATE_TIME_MODIFIERS.Length)
					{
						if (fmt is SimpleDateFormat)
						{
							result.Append(",date,").Append(((SimpleDateFormat)fmt).ToPattern());
						}
						else
						{
							// UNKNOWN
						}
					}
					else if (index != MODIFIER_DEFAULT)
					{
						result.Append(',').Append(DATE_TIME_MODIFIER_KEYWORDS[index]);
					}
				}
				else
				{
					//result.append(", unknown");
				}
				result.Append('}');
			}
			CopyAndFixQuotes(Pattern, lastOffset, Pattern.Length(), result);
			return result.ToString();
		}

		/// <summary>
		/// Sets the formats to use for the values passed into
		/// <code>format</code> methods or returned from <code>parse</code>
		/// methods. The indices of elements in <code>newFormats</code>
		/// correspond to the argument indices used in the previously set
		/// pattern string.
		/// The order of formats in <code>newFormats</code> thus corresponds to
		/// the order of elements in the <code>arguments</code> array passed
		/// to the <code>format</code> methods or the result array returned
		/// by the <code>parse</code> methods.
		/// <para>
		/// If an argument index is used for more than one format element
		/// in the pattern string, then the corresponding new format is used
		/// for all such format elements. If an argument index is not used
		/// for any format element in the pattern string, then the
		/// corresponding new format is ignored. If fewer formats are provided
		/// than needed, then only the formats for argument indices less
		/// than <code>newFormats.length</code> are replaced.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newFormats"> the new formats to use </param>
		/// <exception cref="NullPointerException"> if <code>newFormats</code> is null
		/// @since 1.4 </exception>
		public virtual Format[] FormatsByArgumentIndex
		{
			set
			{
				for (int i = 0; i <= MaxOffset; i++)
				{
					int j = ArgumentNumbers[i];
					if (j < value.Length)
					{
						Formats_Renamed[i] = value[j];
					}
				}
			}
			get
			{
				int maximumArgumentNumber = -1;
				for (int i = 0; i <= MaxOffset; i++)
				{
					if (ArgumentNumbers[i] > maximumArgumentNumber)
					{
						maximumArgumentNumber = ArgumentNumbers[i];
					}
				}
				Format[] resultArray = new Format[maximumArgumentNumber + 1];
				for (int i = 0; i <= MaxOffset; i++)
				{
					resultArray[ArgumentNumbers[i]] = Formats_Renamed[i];
				}
				return resultArray;
			}
		}

		/// <summary>
		/// Sets the formats to use for the format elements in the
		/// previously set pattern string.
		/// The order of formats in <code>newFormats</code> corresponds to
		/// the order of format elements in the pattern string.
		/// <para>
		/// If more formats are provided than needed by the pattern string,
		/// the remaining ones are ignored. If fewer formats are provided
		/// than needed, then only the first <code>newFormats.length</code>
		/// formats are replaced.
		/// </para>
		/// <para>
		/// Since the order of format elements in a pattern string often
		/// changes during localization, it is generally better to use the
		/// <seealso cref="#setFormatsByArgumentIndex setFormatsByArgumentIndex"/>
		/// method, which assumes an order of formats corresponding to the
		/// order of elements in the <code>arguments</code> array passed to
		/// the <code>format</code> methods or the result array returned by
		/// the <code>parse</code> methods.
		/// 
		/// </para>
		/// </summary>
		/// <param name="newFormats"> the new formats to use </param>
		/// <exception cref="NullPointerException"> if <code>newFormats</code> is null </exception>
		public virtual Format[] Formats
		{
			set
			{
				int runsToCopy = value.Length;
				if (runsToCopy > MaxOffset + 1)
				{
					runsToCopy = MaxOffset + 1;
				}
				for (int i = 0; i < runsToCopy; i++)
				{
					Formats_Renamed[i] = value[i];
				}
			}
			get
			{
				Format[] resultArray = new Format[MaxOffset + 1];
				System.Array.Copy(Formats_Renamed, 0, resultArray, 0, MaxOffset + 1);
				return resultArray;
			}
		}

		/// <summary>
		/// Sets the format to use for the format elements within the
		/// previously set pattern string that use the given argument
		/// index.
		/// The argument index is part of the format element definition and
		/// represents an index into the <code>arguments</code> array passed
		/// to the <code>format</code> methods or the result array returned
		/// by the <code>parse</code> methods.
		/// <para>
		/// If the argument index is used for more than one format element
		/// in the pattern string, then the new format is used for all such
		/// format elements. If the argument index is not used for any format
		/// element in the pattern string, then the new format is ignored.
		/// 
		/// </para>
		/// </summary>
		/// <param name="argumentIndex"> the argument index for which to use the new format </param>
		/// <param name="newFormat"> the new format to use
		/// @since 1.4 </param>
		public virtual void SetFormatByArgumentIndex(int argumentIndex, Format newFormat)
		{
			for (int j = 0; j <= MaxOffset; j++)
			{
				if (ArgumentNumbers[j] == argumentIndex)
				{
					Formats_Renamed[j] = newFormat;
				}
			}
		}

		/// <summary>
		/// Sets the format to use for the format element with the given
		/// format element index within the previously set pattern string.
		/// The format element index is the zero-based number of the format
		/// element counting from the start of the pattern string.
		/// <para>
		/// Since the order of format elements in a pattern string often
		/// changes during localization, it is generally better to use the
		/// <seealso cref="#setFormatByArgumentIndex setFormatByArgumentIndex"/>
		/// method, which accesses format elements based on the argument
		/// index they specify.
		/// 
		/// </para>
		/// </summary>
		/// <param name="formatElementIndex"> the index of a format element within the pattern </param>
		/// <param name="newFormat"> the format to use for the specified format element </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code formatElementIndex} is equal to or
		///            larger than the number of format elements in the pattern string </exception>
		public virtual void SetFormat(int formatElementIndex, Format newFormat)
		{
			Formats_Renamed[formatElementIndex] = newFormat;
		}



		/// <summary>
		/// Formats an array of objects and appends the <code>MessageFormat</code>'s
		/// pattern, with format elements replaced by the formatted objects, to the
		/// provided <code>StringBuffer</code>.
		/// <para>
		/// The text substituted for the individual format elements is derived from
		/// the current subformat of the format element and the
		/// <code>arguments</code> element at the format element's argument index
		/// as indicated by the first matching line of the following table. An
		/// argument is <i>unavailable</i> if <code>arguments</code> is
		/// <code>null</code> or has fewer than argumentIndex+1 elements.
		/// 
		/// <table border=1 summary="Examples of subformat,argument,and formatted text">
		///    <tr>
		///       <th>Subformat
		///       <th>Argument
		///       <th>Formatted Text
		///    <tr>
		///       <td><i>any</i>
		///       <td><i>unavailable</i>
		///       <td><code>"{" + argumentIndex + "}"</code>
		///    <tr>
		///       <td><i>any</i>
		///       <td><code>null</code>
		///       <td><code>"null"</code>
		///    <tr>
		///       <td><code>instanceof ChoiceFormat</code>
		///       <td><i>any</i>
		///       <td><code>subformat.format(argument).indexOf('{') &gt;= 0 ?<br>
		///           (new MessageFormat(subformat.format(argument), getLocale())).format(argument) :
		///           subformat.format(argument)</code>
		///    <tr>
		///       <td><code>!= null</code>
		///       <td><i>any</i>
		///       <td><code>subformat.format(argument)</code>
		///    <tr>
		///       <td><code>null</code>
		///       <td><code>instanceof Number</code>
		///       <td><code>NumberFormat.getInstance(getLocale()).format(argument)</code>
		///    <tr>
		///       <td><code>null</code>
		///       <td><code>instanceof Date</code>
		///       <td><code>DateFormat.getDateTimeInstance(DateFormat.SHORT, DateFormat.SHORT, getLocale()).format(argument)</code>
		///    <tr>
		///       <td><code>null</code>
		///       <td><code>instanceof String</code>
		///       <td><code>argument</code>
		///    <tr>
		///       <td><code>null</code>
		///       <td><i>any</i>
		///       <td><code>argument.toString()</code>
		/// </table>
		/// </para>
		/// <para>
		/// If <code>pos</code> is non-null, and refers to
		/// <code>Field.ARGUMENT</code>, the location of the first formatted
		/// string will be returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="arguments"> an array of objects to be formatted and substituted. </param>
		/// <param name="result"> where text is appended. </param>
		/// <param name="pos"> On input: an alignment field, if desired.
		///            On output: the offsets of the alignment field. </param>
		/// <returns> the string buffer passed in as {@code result}, with formatted
		/// text appended </returns>
		/// <exception cref="IllegalArgumentException"> if an argument in the
		///            <code>arguments</code> array is not of the type
		///            expected by the format element(s) that use it. </exception>
		public sealed override StringBuffer Format(Object[] arguments, StringBuffer result, FieldPosition pos)
		{
			return Subformat(arguments, result, pos, null);
		}

		/// <summary>
		/// Creates a MessageFormat with the given pattern and uses it
		/// to format the given arguments. This is equivalent to
		/// <blockquote>
		///     <code>(new <seealso cref="#MessageFormat(String) MessageFormat"/>(pattern)).<seealso cref="#format(java.lang.Object[], java.lang.StringBuffer, java.text.FieldPosition) format"/>(arguments, new StringBuffer(), null).toString()</code>
		/// </blockquote>
		/// </summary>
		/// <param name="pattern">   the pattern string </param>
		/// <param name="arguments"> object(s) to format </param>
		/// <returns> the formatted string </returns>
		/// <exception cref="IllegalArgumentException"> if the pattern is invalid,
		///            or if an argument in the <code>arguments</code> array
		///            is not of the type expected by the format element(s)
		///            that use it. </exception>
		public static String Format(String pattern, params Object[] arguments)
		{
			MessageFormat temp = new MessageFormat(pattern);
			return temp.Format(arguments);
		}

		// Overrides
		/// <summary>
		/// Formats an array of objects and appends the <code>MessageFormat</code>'s
		/// pattern, with format elements replaced by the formatted objects, to the
		/// provided <code>StringBuffer</code>.
		/// This is equivalent to
		/// <blockquote>
		///     <code><seealso cref="#format(java.lang.Object[], java.lang.StringBuffer, java.text.FieldPosition) format"/>((Object[]) arguments, result, pos)</code>
		/// </blockquote>
		/// </summary>
		/// <param name="arguments"> an array of objects to be formatted and substituted. </param>
		/// <param name="result"> where text is appended. </param>
		/// <param name="pos"> On input: an alignment field, if desired.
		///            On output: the offsets of the alignment field. </param>
		/// <exception cref="IllegalArgumentException"> if an argument in the
		///            <code>arguments</code> array is not of the type
		///            expected by the format element(s) that use it. </exception>
		public sealed override StringBuffer Format(Object arguments, StringBuffer result, FieldPosition pos)
		{
			return Subformat((Object[]) arguments, result, pos, null);
		}

		/// <summary>
		/// Formats an array of objects and inserts them into the
		/// <code>MessageFormat</code>'s pattern, producing an
		/// <code>AttributedCharacterIterator</code>.
		/// You can use the returned <code>AttributedCharacterIterator</code>
		/// to build the resulting String, as well as to determine information
		/// about the resulting String.
		/// <para>
		/// The text of the returned <code>AttributedCharacterIterator</code> is
		/// the same that would be returned by
		/// <blockquote>
		///     <code><seealso cref="#format(java.lang.Object[], java.lang.StringBuffer, java.text.FieldPosition) format"/>(arguments, new StringBuffer(), null).toString()</code>
		/// </blockquote>
		/// </para>
		/// <para>
		/// In addition, the <code>AttributedCharacterIterator</code> contains at
		/// least attributes indicating where text was generated from an
		/// argument in the <code>arguments</code> array. The keys of these attributes are of
		/// type <code>MessageFormat.Field</code>, their values are
		/// <code>Integer</code> objects indicating the index in the <code>arguments</code>
		/// array of the argument from which the text was generated.
		/// </para>
		/// <para>
		/// The attributes/value from the underlying <code>Format</code>
		/// instances that <code>MessageFormat</code> uses will also be
		/// placed in the resulting <code>AttributedCharacterIterator</code>.
		/// This allows you to not only find where an argument is placed in the
		/// resulting String, but also which fields it contains in turn.
		/// 
		/// </para>
		/// </summary>
		/// <param name="arguments"> an array of objects to be formatted and substituted. </param>
		/// <returns> AttributedCharacterIterator describing the formatted value. </returns>
		/// <exception cref="NullPointerException"> if <code>arguments</code> is null. </exception>
		/// <exception cref="IllegalArgumentException"> if an argument in the
		///            <code>arguments</code> array is not of the type
		///            expected by the format element(s) that use it.
		/// @since 1.4 </exception>
		public override AttributedCharacterIterator FormatToCharacterIterator(Object arguments)
		{
			StringBuffer result = new StringBuffer();
			List<AttributedCharacterIterator> iterators = new List<AttributedCharacterIterator>();

			if (arguments == null)
			{
				throw new NullPointerException("formatToCharacterIterator must be passed non-null object");
			}
			Subformat((Object[]) arguments, result, null, iterators);
			if (iterators.Count == 0)
			{
				return CreateAttributedCharacterIterator("");
			}
			return CreateAttributedCharacterIterator(iterators.ToArray());
		}

		/// <summary>
		/// Parses the string.
		/// 
		/// <para>Caveats: The parse may fail in a number of circumstances.
		/// For example:
		/// <ul>
		/// <li>If one of the arguments does not occur in the pattern.
		/// <li>If the format of an argument loses information, such as
		///     with a choice format where a large number formats to "many".
		/// <li>Does not yet handle recursion (where
		///     the substituted strings contain {n} references.)
		/// <li>Will not always find a match (or the correct match)
		///     if some part of the parse is ambiguous.
		///     For example, if the pattern "{1},{2}" is used with the
		///     string arguments {"a,b", "c"}, it will format as "a,b,c".
		///     When the result is parsed, it will return {"a", "b,c"}.
		/// <li>If a single argument is parsed more than once in the string,
		///     then the later parse wins.
		/// </ul>
		/// When the parse fails, use ParsePosition.getErrorIndex() to find out
		/// where in the string the parsing failed.  The returned error
		/// index is the starting offset of the sub-patterns that the string
		/// is comparing with.  For example, if the parsing string "AAA {0} BBB"
		/// is comparing against the pattern "AAD {0} BBB", the error index is
		/// 0. When an error occurs, the call to this method will return null.
		/// If the source is null, return an empty array.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos">    the parse position </param>
		/// <returns> an array of parsed objects </returns>
		public virtual Object[] Parse(String source, ParsePosition pos)
		{
			if (source == null)
			{
				Object[] empty = new Object[] {};
				return empty;
			}

			int maximumArgumentNumber = -1;
			for (int i = 0; i <= MaxOffset; i++)
			{
				if (ArgumentNumbers[i] > maximumArgumentNumber)
				{
					maximumArgumentNumber = ArgumentNumbers[i];
				}
			}
			Object[] resultArray = new Object[maximumArgumentNumber + 1];

			int patternOffset = 0;
			int sourceOffset = pos.Index_Renamed;
			ParsePosition tempStatus = new ParsePosition(0);
			for (int i = 0; i <= MaxOffset; ++i)
			{
				// match up to format
				int len = Offsets[i] - patternOffset;
				if (len == 0 || Pattern.RegionMatches(patternOffset, source, sourceOffset, len))
				{
					sourceOffset += len;
					patternOffset += len;
				}
				else
				{
					pos.ErrorIndex_Renamed = sourceOffset;
					return null; // leave index as is to signal error
				}

				// now use format
				if (Formats_Renamed[i] == null) // string format
				{
					// if at end, use longest possible match
					// otherwise uses first match to intervening string
					// does NOT recursively try all possibilities
					int tempLength = (i != MaxOffset) ? Offsets[i + 1] : Pattern.Length();

					int next;
					if (patternOffset >= tempLength)
					{
						next = source.Length();
					}
					else
					{
						next = source.IndexOf(Pattern.Substring(patternOffset, tempLength - patternOffset), sourceOffset);
					}

					if (next < 0)
					{
						pos.ErrorIndex_Renamed = sourceOffset;
						return null; // leave index as is to signal error
					}
					else
					{
						String strValue = source.Substring(sourceOffset, next - sourceOffset);
						if (!strValue.Equals("{" + ArgumentNumbers[i] + "}"))
						{
							resultArray[ArgumentNumbers[i]] = source.Substring(sourceOffset, next - sourceOffset);
						}
						sourceOffset = next;
					}
				}
				else
				{
					tempStatus.Index_Renamed = sourceOffset;
					resultArray[ArgumentNumbers[i]] = Formats_Renamed[i].ParseObject(source,tempStatus);
					if (tempStatus.Index_Renamed == sourceOffset)
					{
						pos.ErrorIndex_Renamed = sourceOffset;
						return null; // leave index as is to signal error
					}
					sourceOffset = tempStatus.Index_Renamed; // update
				}
			}
			int len = Pattern.Length() - patternOffset;
			if (len == 0 || Pattern.RegionMatches(patternOffset, source, sourceOffset, len))
			{
				pos.Index_Renamed = sourceOffset + len;
			}
			else
			{
				pos.ErrorIndex_Renamed = sourceOffset;
				return null; // leave index as is to signal error
			}
			return resultArray;
		}

		/// <summary>
		/// Parses text from the beginning of the given string to produce an object
		/// array.
		/// The method may not use the entire text of the given string.
		/// <para>
		/// See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		/// on message parsing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> A <code>String</code> whose beginning should be parsed. </param>
		/// <returns> An <code>Object</code> array parsed from the string. </returns>
		/// <exception cref="ParseException"> if the beginning of the specified string
		///            cannot be parsed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object[] parse(String source) throws ParseException
		public virtual Object[] Parse(String source)
		{
			ParsePosition pos = new ParsePosition(0);
			Object[] result = Parse(source, pos);
			if (pos.Index_Renamed == 0) // unchanged, returned object is null
			{
				throw new ParseException("MessageFormat parse error!", pos.ErrorIndex_Renamed);
			}

			return result;
		}

		/// <summary>
		/// Parses text from a string to produce an object array.
		/// <para>
		/// The method attempts to parse text starting at the index given by
		/// <code>pos</code>.
		/// If parsing succeeds, then the index of <code>pos</code> is updated
		/// to the index after the last character used (parsing does not necessarily
		/// use all characters up to the end of the string), and the parsed
		/// object array is returned. The updated <code>pos</code> can be used to
		/// indicate the starting point for the next call to this method.
		/// If an error occurs, then the index of <code>pos</code> is not
		/// changed, the error index of <code>pos</code> is set to the index of
		/// the character where the error occurred, and null is returned.
		/// </para>
		/// <para>
		/// See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		/// on message parsing.
		/// 
		/// </para>
		/// </summary>
		/// <param name="source"> A <code>String</code>, part of which should be parsed. </param>
		/// <param name="pos"> A <code>ParsePosition</code> object with index and error
		///            index information as described above. </param>
		/// <returns> An <code>Object</code> array parsed from the string. In case of
		///         error, returns null. </returns>
		/// <exception cref="NullPointerException"> if <code>pos</code> is null. </exception>
		public override Object ParseObject(String source, ParsePosition pos)
		{
			return Parse(source, pos);
		}

		/// <summary>
		/// Creates and returns a copy of this object.
		/// </summary>
		/// <returns> a clone of this instance. </returns>
		public override Object Clone()
		{
			MessageFormat other = (MessageFormat) base.Clone();

			// clone arrays. Can't do with utility because of bug in Cloneable
			other.Formats_Renamed = Formats_Renamed.clone(); // shallow clone
			for (int i = 0; i < Formats_Renamed.Length; ++i)
			{
				if (Formats_Renamed[i] != null)
				{
					other.Formats_Renamed[i] = (Format)Formats_Renamed[i].Clone();
				}
			}
			// for primitives or immutables, shallow clone is enough
			other.Offsets = Offsets.clone();
			other.ArgumentNumbers = ArgumentNumbers.clone();

			return other;
		}

		/// <summary>
		/// Equality comparison between two message format objects
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (this == obj) // quick check
			{
				return true;
			}
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			MessageFormat other = (MessageFormat) obj;
			return (MaxOffset == other.MaxOffset && Pattern.Equals(other.Pattern) && ((Locale_Renamed != null && Locale_Renamed.Equals(other.Locale_Renamed)) || (Locale_Renamed == null && other.Locale_Renamed == null)) && Arrays.Equals(Offsets,other.Offsets) && Arrays.Equals(ArgumentNumbers,other.ArgumentNumbers) && Arrays.Equals(Formats_Renamed,other.Formats_Renamed));
		}

		/// <summary>
		/// Generates a hash code for the message format object.
		/// </summary>
		public override int HashCode()
		{
			return Pattern.HashCode(); // enough for reasonable distribution
		}


		/// <summary>
		/// Defines constants that are used as attribute keys in the
		/// <code>AttributedCharacterIterator</code> returned
		/// from <code>MessageFormat.formatToCharacterIterator</code>.
		/// 
		/// @since 1.4
		/// </summary>
		public class Field : Format.Field
		{

			// Proclaim serial compatibility with 1.4 FCS
			internal new const long SerialVersionUID = 7899943957617360810L;

			/// <summary>
			/// Creates a Field with the specified name.
			/// </summary>
			/// <param name="name"> Name of the attribute </param>
			protected internal Field(String name) : base(name)
			{
			}

			/// <summary>
			/// Resolves instances being deserialized to the predefined constants.
			/// </summary>
			/// <exception cref="InvalidObjectException"> if the constant could not be
			///         resolved. </exception>
			/// <returns> resolved MessageFormat.Field constant </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readResolve() throws java.io.InvalidObjectException
			protected internal override Object ReadResolve()
			{
				if (this.GetType() != typeof(MessageFormat.Field))
				{
					throw new InvalidObjectException("subclass didn't correctly implement readResolve");
				}

				return ARGUMENT;
			}

			//
			// The constants
			//

			/// <summary>
			/// Constant identifying a portion of a message that was generated
			/// from an argument passed into <code>formatToCharacterIterator</code>.
			/// The value associated with the key will be an <code>Integer</code>
			/// indicating the index in the <code>arguments</code> array of the
			/// argument from which the text was generated.
			/// </summary>
			public static readonly Field ARGUMENT = new Field("message argument field");
		}

		// ===========================privates============================

		/// <summary>
		/// The locale to use for formatting numbers and dates.
		/// @serial
		/// </summary>
		private Locale Locale_Renamed;

		/// <summary>
		/// The string that the formatted values are to be plugged into.  In other words, this
		/// is the pattern supplied on construction with all of the {} expressions taken out.
		/// @serial
		/// </summary>
		private String Pattern = "";

		/// <summary>
		/// The initially expected number of subformats in the format </summary>
		private const int INITIAL_FORMATS = 10;

		/// <summary>
		/// An array of formatters, which are used to format the arguments.
		/// @serial
		/// </summary>
		private Format[] Formats_Renamed = new Format[INITIAL_FORMATS];

		/// <summary>
		/// The positions where the results of formatting each argument are to be inserted
		/// into the pattern.
		/// @serial
		/// </summary>
		private int[] Offsets = new int[INITIAL_FORMATS];

		/// <summary>
		/// The argument numbers corresponding to each formatter.  (The formatters are stored
		/// in the order they occur in the pattern, not in the order in which the arguments
		/// are specified.)
		/// @serial
		/// </summary>
		private int[] ArgumentNumbers = new int[INITIAL_FORMATS];

		/// <summary>
		/// One less than the number of entries in <code>offsets</code>.  Can also be thought of
		/// as the index of the highest-numbered element in <code>offsets</code> that is being used.
		/// All of these arrays should have the same number of elements being used as <code>offsets</code>
		/// does, and so this variable suffices to tell us how many entries are in all of them.
		/// @serial
		/// </summary>
		private int MaxOffset = -1;

		/// <summary>
		/// Internal routine used by format. If <code>characterIterators</code> is
		/// non-null, AttributedCharacterIterator will be created from the
		/// subformats as necessary. If <code>characterIterators</code> is null
		/// and <code>fp</code> is non-null and identifies
		/// <code>Field.MESSAGE_ARGUMENT</code>, the location of
		/// the first replaced argument will be set in it.
		/// </summary>
		/// <exception cref="IllegalArgumentException"> if an argument in the
		///            <code>arguments</code> array is not of the type
		///            expected by the format element(s) that use it. </exception>
		private StringBuffer Subformat(Object[] arguments, StringBuffer result, FieldPosition fp, IList<AttributedCharacterIterator> characterIterators)
		{
			// note: this implementation assumes a fast substring & index.
			// if this is not true, would be better to append chars one by one.
			int lastOffset = 0;
			int last = result.Length();
			for (int i = 0; i <= MaxOffset; ++i)
			{
				result.Append(Pattern.Substring(lastOffset, Offsets[i] - lastOffset));
				lastOffset = Offsets[i];
				int argumentNumber = ArgumentNumbers[i];
				if (arguments == null || argumentNumber >= arguments.Length)
				{
					result.Append('{').Append(argumentNumber).Append('}');
					continue;
				}
				// int argRecursion = ((recursionProtection >> (argumentNumber*2)) & 0x3);
				if (false) // if (argRecursion == 3){
				{
					// prevent loop!!!
					result.Append('\uFFFD');
				}
				else
				{
					Object obj = arguments[argumentNumber];
					String arg = null;
					Format subFormatter = null;
					if (obj == null)
					{
						arg = "null";
					}
					else if (Formats_Renamed[i] != null)
					{
						subFormatter = Formats_Renamed[i];
						if (subFormatter is ChoiceFormat)
						{
							arg = Formats_Renamed[i].Format(obj);
							if (arg.IndexOf('{') >= 0)
							{
								subFormatter = new MessageFormat(arg, Locale_Renamed);
								obj = arguments;
								arg = null;
							}
						}
					}
					else if (obj is Number)
					{
						// format number if can
						subFormatter = NumberFormat.GetInstance(Locale_Renamed);
					}
					else if (obj is DateTime)
					{
						// format a Date if can
						subFormatter = DateFormat.GetDateTimeInstance(DateFormat.SHORT, DateFormat.SHORT, Locale_Renamed); //fix
					}
					else if (obj is String)
					{
						arg = (String) obj;

					}
					else
					{
						arg = obj.ToString();
						if (arg == null)
						{
							arg = "null";
						}
					}

					// At this point we are in two states, either subFormatter
					// is non-null indicating we should format obj using it,
					// or arg is non-null and we should use it as the value.

					if (characterIterators != null)
					{
						// If characterIterators is non-null, it indicates we need
						// to get the CharacterIterator from the child formatter.
						if (last != result.Length())
						{
							characterIterators.Add(CreateAttributedCharacterIterator(result.Substring(last)));
							last = result.Length();
						}
						if (subFormatter != null)
						{
							AttributedCharacterIterator subIterator = subFormatter.FormatToCharacterIterator(obj);

							Append(result, subIterator);
							if (last != result.Length())
							{
								characterIterators.Add(CreateAttributedCharacterIterator(subIterator, Field.ARGUMENT, Convert.ToInt32(argumentNumber)));
								last = result.Length();
							}
							arg = null;
						}
						if (arg != null && arg.Length() > 0)
						{
							result.Append(arg);
							characterIterators.Add(CreateAttributedCharacterIterator(arg, Field.ARGUMENT, Convert.ToInt32(argumentNumber)));
							last = result.Length();
						}
					}
					else
					{
						if (subFormatter != null)
						{
							arg = subFormatter.Format(obj);
						}
						last = result.Length();
						result.Append(arg);
						if (i == 0 && fp != null && Field.ARGUMENT.Equals(fp.FieldAttribute))
						{
							fp.BeginIndex = last;
							fp.EndIndex = result.Length();
						}
						last = result.Length();
					}
				}
			}
			result.Append(Pattern.Substring(lastOffset, Pattern.Length() - lastOffset));
			if (characterIterators != null && last != result.Length())
			{
				characterIterators.Add(CreateAttributedCharacterIterator(result.Substring(last)));
			}
			return result;
		}

		/// <summary>
		/// Convenience method to append all the characters in
		/// <code>iterator</code> to the StringBuffer <code>result</code>.
		/// </summary>
		private void Append(StringBuffer result, CharacterIterator iterator)
		{
			if (iterator.First() != CharacterIterator_Fields.DONE)
			{
				char aChar;

				result.Append(iterator.First());
				while ((aChar = iterator.Next()) != CharacterIterator_Fields.DONE)
				{
					result.Append(aChar);
				}
			}
		}

		// Indices for segments
		private const int SEG_RAW = 0;
		private const int SEG_INDEX = 1;
		private const int SEG_TYPE = 2;
		private const int SEG_MODIFIER = 3; // modifier or subformat

		// Indices for type keywords
		private const int TYPE_NULL = 0;
		private const int TYPE_NUMBER = 1;
		private const int TYPE_DATE = 2;
		private const int TYPE_TIME = 3;
		private const int TYPE_CHOICE = 4;

		private static readonly String[] TYPE_KEYWORDS = new String[] {"", "number", "date", "time", "choice"};

		// Indices for number modifiers
		private const int MODIFIER_DEFAULT = 0; // common in number and date-time
		private const int MODIFIER_CURRENCY = 1;
		private const int MODIFIER_PERCENT = 2;
		private const int MODIFIER_INTEGER = 3;

		private static readonly String[] NUMBER_MODIFIER_KEYWORDS = new String[] {"", "currency", "percent", "integer"};

		// Indices for date-time modifiers
		private const int MODIFIER_SHORT = 1;
		private const int MODIFIER_MEDIUM = 2;
		private const int MODIFIER_LONG = 3;
		private const int MODIFIER_FULL = 4;

		private static readonly String[] DATE_TIME_MODIFIER_KEYWORDS = new String[] {"", "short", "medium", "long", "full"};

		// Date-time style values corresponding to the date-time modifiers.
		private static readonly int[] DATE_TIME_MODIFIERS = new int[] {DateFormat.DEFAULT, DateFormat.SHORT, DateFormat.MEDIUM, DateFormat.LONG, DateFormat.FULL};

		private void MakeFormat(int position, int offsetNumber, StringBuilder[] textSegments)
		{
			String[] segments = new String[textSegments.Length];
			for (int i = 0; i < textSegments.Length; i++)
			{
				StringBuilder oneseg = textSegments[i];
				segments[i] = (oneseg != null) ? oneseg.ToString() : "";
			}

			// get the argument number
			int argumentNumber;
			try
			{
				argumentNumber = Convert.ToInt32(segments[SEG_INDEX]); // always unlocalized!
			}
			catch (NumberFormatException e)
			{
				throw new IllegalArgumentException("can't parse argument number: " + segments[SEG_INDEX], e);
			}
			if (argumentNumber < 0)
			{
				throw new IllegalArgumentException("negative argument number: " + argumentNumber);
			}

			// resize format information arrays if necessary
			if (offsetNumber >= Formats_Renamed.Length)
			{
				int newLength = Formats_Renamed.Length * 2;
				Format[] newFormats = new Format[newLength];
				int[] newOffsets = new int[newLength];
				int[] newArgumentNumbers = new int[newLength];
				System.Array.Copy(Formats_Renamed, 0, newFormats, 0, MaxOffset + 1);
				System.Array.Copy(Offsets, 0, newOffsets, 0, MaxOffset + 1);
				System.Array.Copy(ArgumentNumbers, 0, newArgumentNumbers, 0, MaxOffset + 1);
				Formats_Renamed = newFormats;
				Offsets = newOffsets;
				ArgumentNumbers = newArgumentNumbers;
			}
			int oldMaxOffset = MaxOffset;
			MaxOffset = offsetNumber;
			Offsets[offsetNumber] = segments[SEG_RAW].Length();
			ArgumentNumbers[offsetNumber] = argumentNumber;

			// now get the format
			Format newFormat = null;
			if (segments[SEG_TYPE].Length() != 0)
			{
				int type = FindKeyword(segments[SEG_TYPE], TYPE_KEYWORDS);
				switch (type)
				{
				case TYPE_NULL:
					// Type "" is allowed. e.g., "{0,}", "{0,,}", and "{0,,#}"
					// are treated as "{0}".
					break;

				case TYPE_NUMBER:
					switch (FindKeyword(segments[SEG_MODIFIER], NUMBER_MODIFIER_KEYWORDS))
					{
					case MODIFIER_DEFAULT:
						newFormat = NumberFormat.GetInstance(Locale_Renamed);
						break;
					case MODIFIER_CURRENCY:
						newFormat = NumberFormat.GetCurrencyInstance(Locale_Renamed);
						break;
					case MODIFIER_PERCENT:
						newFormat = NumberFormat.GetPercentInstance(Locale_Renamed);
						break;
					case MODIFIER_INTEGER:
						newFormat = NumberFormat.GetIntegerInstance(Locale_Renamed);
						break;
					default: // DecimalFormat pattern
						try
						{
							newFormat = new DecimalFormat(segments[SEG_MODIFIER], DecimalFormatSymbols.GetInstance(Locale_Renamed));
						}
						catch (IllegalArgumentException e)
						{
							MaxOffset = oldMaxOffset;
							throw e;
						}
						break;
					}
					break;

				case TYPE_DATE:
				case TYPE_TIME:
					int mod = FindKeyword(segments[SEG_MODIFIER], DATE_TIME_MODIFIER_KEYWORDS);
					if (mod >= 0 && mod < DATE_TIME_MODIFIER_KEYWORDS.Length)
					{
						if (type == TYPE_DATE)
						{
							newFormat = DateFormat.GetDateInstance(DATE_TIME_MODIFIERS[mod], Locale_Renamed);
						}
						else
						{
							newFormat = DateFormat.GetTimeInstance(DATE_TIME_MODIFIERS[mod], Locale_Renamed);
						}
					}
					else
					{
						// SimpleDateFormat pattern
						try
						{
							newFormat = new SimpleDateFormat(segments[SEG_MODIFIER], Locale_Renamed);
						}
						catch (IllegalArgumentException e)
						{
							MaxOffset = oldMaxOffset;
							throw e;
						}
					}
					break;

				case TYPE_CHOICE:
					try
					{
						// ChoiceFormat pattern
						newFormat = new ChoiceFormat(segments[SEG_MODIFIER]);
					}
					catch (Exception e)
					{
						MaxOffset = oldMaxOffset;
						throw new IllegalArgumentException("Choice Pattern incorrect: " + segments[SEG_MODIFIER], e);
					}
					break;

				default:
					MaxOffset = oldMaxOffset;
					throw new IllegalArgumentException("unknown format type: " + segments[SEG_TYPE]);
				}
			}
			Formats_Renamed[offsetNumber] = newFormat;
		}

		private static int FindKeyword(String s, String[] list)
		{
			for (int i = 0; i < list.Length; ++i)
			{
				if (s.Equals(list[i]))
				{
					return i;
				}
			}

			// Try trimmed lowercase.
			String ls = s.Trim().ToLowerCase(Locale.ROOT);
			if (ls != s)
			{
				for (int i = 0; i < list.Length; ++i)
				{
					if (ls.Equals(list[i]))
					{
						return i;
					}
				}
			}
			return -1;
		}

		private static void CopyAndFixQuotes(String source, int start, int end, StringBuilder target)
		{
			bool quoted = false;

			for (int i = start; i < end; ++i)
			{
				char ch = source.CharAt(i);
				if (ch == '{')
				{
					if (!quoted)
					{
						target.Append('\'');
						quoted = true;
					}
					target.Append(ch);
				}
				else if (ch == '\'')
				{
					target.Append("''");
				}
				else
				{
					if (quoted)
					{
						target.Append('\'');
						quoted = false;
					}
					target.Append(ch);
				}
			}
			if (quoted)
			{
				target.Append('\'');
			}
		}

		/// <summary>
		/// After reading an object from the input stream, do a simple verification
		/// to maintain class invariants. </summary>
		/// <exception cref="InvalidObjectException"> if the objects read from the stream is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			@in.DefaultReadObject();
			bool isValid = MaxOffset >= -1 && Formats_Renamed.Length > MaxOffset && Offsets.Length > MaxOffset && ArgumentNumbers.Length > MaxOffset;
			if (isValid)
			{
				int lastOffset = Pattern.Length() + 1;
				for (int i = MaxOffset; i >= 0; --i)
				{
					if ((Offsets[i] < 0) || (Offsets[i] > lastOffset))
					{
						isValid = false;
						break;
					}
					else
					{
						lastOffset = Offsets[i];
					}
				}
			}
			if (!isValid)
			{
				throw new InvalidObjectException("Could not reconstruct MessageFormat from corrupt stream.");
			}
		}
	}

}