using System;
using System.Diagnostics;

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

namespace java.util
{


	using LRUCache = sun.misc.LRUCache;

	/// <summary>
	/// A simple text scanner which can parse primitive types and strings using
	/// regular expressions.
	/// 
	/// <para>A <code>Scanner</code> breaks its input into tokens using a
	/// delimiter pattern, which by default matches whitespace. The resulting
	/// tokens may then be converted into values of different types using the
	/// various <tt>next</tt> methods.
	/// 
	/// </para>
	/// <para>For example, this code allows a user to read a number from
	/// <tt>System.in</tt>:
	/// <blockquote><pre>{@code
	///     Scanner sc = new Scanner(System.in);
	///     int i = sc.nextInt();
	/// }</pre></blockquote>
	/// 
	/// </para>
	/// <para>As another example, this code allows <code>long</code> types to be
	/// assigned from entries in a file <code>myNumbers</code>:
	/// <blockquote><pre>{@code
	///      Scanner sc = new Scanner(new File("myNumbers"));
	///      while (sc.hasNextLong()) {
	///          long aLong = sc.nextLong();
	///      }
	/// }</pre></blockquote>
	/// 
	/// </para>
	/// <para>The scanner can also use delimiters other than whitespace. This
	/// example reads several items in from a string:
	/// <blockquote><pre>{@code
	///     String input = "1 fish 2 fish red fish blue fish";
	///     Scanner s = new Scanner(input).useDelimiter("\\s*fish\\s*");
	///     System.out.println(s.nextInt());
	///     System.out.println(s.nextInt());
	///     System.out.println(s.next());
	///     System.out.println(s.next());
	///     s.close();
	/// }</pre></blockquote>
	/// </para>
	/// <para>
	/// prints the following output:
	/// <blockquote><pre>{@code
	///     1
	///     2
	///     red
	///     blue
	/// }</pre></blockquote>
	/// 
	/// </para>
	/// <para>The same output can be generated with this code, which uses a regular
	/// expression to parse all four tokens at once:
	/// <blockquote><pre>{@code
	///     String input = "1 fish 2 fish red fish blue fish";
	///     Scanner s = new Scanner(input);
	///     s.findInLine("(\\d+) fish (\\d+) fish (\\w+) fish (\\w+)");
	///     MatchResult result = s.match();
	///     for (int i=1; i<=result.groupCount(); i++)
	///         System.out.println(result.group(i));
	///     s.close();
	/// }</pre></blockquote>
	/// 
	/// </para>
	/// <para>The <a name="default-delimiter">default whitespace delimiter</a> used
	/// by a scanner is as recognized by <seealso cref="java.lang.Character"/>.{@link
	/// java.lang.Character#isWhitespace(char) isWhitespace}. The <seealso cref="#reset"/>
	/// method will reset the value of the scanner's delimiter to the default
	/// whitespace delimiter regardless of whether it was previously changed.
	/// 
	/// </para>
	/// <para>A scanning operation may block waiting for input.
	/// 
	/// </para>
	/// <para>The <seealso cref="#next"/> and <seealso cref="#hasNext"/> methods and their
	/// primitive-type companion methods (such as <seealso cref="#nextInt"/> and
	/// <seealso cref="#hasNextInt"/>) first skip any input that matches the delimiter
	/// pattern, and then attempt to return the next token. Both <tt>hasNext</tt>
	/// and <tt>next</tt> methods may block waiting for further input.  Whether a
	/// <tt>hasNext</tt> method blocks has no connection to whether or not its
	/// associated <tt>next</tt> method will block.
	/// 
	/// </para>
	/// <para> The <seealso cref="#findInLine"/>, <seealso cref="#findWithinHorizon"/>, and <seealso cref="#skip"/>
	/// methods operate independently of the delimiter pattern. These methods will
	/// attempt to match the specified pattern with no regard to delimiters in the
	/// input and thus can be used in special circumstances where delimiters are
	/// not relevant. These methods may block waiting for more input.
	/// 
	/// </para>
	/// <para>When a scanner throws an <seealso cref="InputMismatchException"/>, the scanner
	/// will not pass the token that caused the exception, so that it may be
	/// retrieved or skipped via some other method.
	/// 
	/// </para>
	/// <para>Depending upon the type of delimiting pattern, empty tokens may be
	/// returned. For example, the pattern <tt>"\\s+"</tt> will return no empty
	/// tokens since it matches multiple instances of the delimiter. The delimiting
	/// pattern <tt>"\\s"</tt> could return empty tokens since it only passes one
	/// space at a time.
	/// 
	/// </para>
	/// <para> A scanner can read text from any object which implements the {@link
	/// java.lang.Readable} interface.  If an invocation of the underlying
	/// readable's <seealso cref="java.lang.Readable#read"/> method throws an {@link
	/// java.io.IOException} then the scanner assumes that the end of the input
	/// has been reached.  The most recent <tt>IOException</tt> thrown by the
	/// underlying readable can be retrieved via the <seealso cref="#ioException"/> method.
	/// 
	/// </para>
	/// <para>When a <code>Scanner</code> is closed, it will close its input source
	/// if the source implements the <seealso cref="java.io.Closeable"/> interface.
	/// 
	/// </para>
	/// <para>A <code>Scanner</code> is not safe for multithreaded use without
	/// external synchronization.
	/// 
	/// </para>
	/// <para>Unless otherwise mentioned, passing a <code>null</code> parameter into
	/// any method of a <code>Scanner</code> will cause a
	/// <code>NullPointerException</code> to be thrown.
	/// 
	/// </para>
	/// <para>A scanner will default to interpreting numbers as decimal unless a
	/// different radix has been set by using the <seealso cref="#useRadix"/> method. The
	/// <seealso cref="#reset"/> method will reset the value of the scanner's radix to
	/// <code>10</code> regardless of whether it was previously changed.
	/// 
	/// <h3> <a name="localized-numbers">Localized numbers</a> </h3>
	/// 
	/// </para>
	/// <para> An instance of this class is capable of scanning numbers in the standard
	/// formats as well as in the formats of the scanner's locale. A scanner's
	/// <a name="initial-locale">initial locale </a>is the value returned by the {@link
	/// java.util.Locale#getDefault(Locale.Category)
	/// Locale.getDefault(Locale.Category.FORMAT)} method; it may be changed via the {@link
	/// #useLocale} method. The <seealso cref="#reset"/> method will reset the value of the
	/// scanner's locale to the initial locale regardless of whether it was
	/// previously changed.
	/// 
	/// </para>
	/// <para>The localized formats are defined in terms of the following parameters,
	/// which for a particular locale are taken from that locale's {@link
	/// java.text.DecimalFormat DecimalFormat} object, <tt>df</tt>, and its and
	/// <seealso cref="java.text.DecimalFormatSymbols DecimalFormatSymbols"/> object,
	/// <tt>dfs</tt>.
	/// 
	/// <blockquote><dl>
	///     <dt><i>LocalGroupSeparator&nbsp;&nbsp;</i>
	///         <dd>The character used to separate thousands groups,
	///         <i>i.e.,</i>&nbsp;<tt>dfs.</tt>{@link
	///         java.text.DecimalFormatSymbols#getGroupingSeparator
	///         getGroupingSeparator()}
	///     <dt><i>LocalDecimalSeparator&nbsp;&nbsp;</i>
	///         <dd>The character used for the decimal point,
	///     <i>i.e.,</i>&nbsp;<tt>dfs.</tt>{@link
	///     java.text.DecimalFormatSymbols#getDecimalSeparator
	///     getDecimalSeparator()}
	///     <dt><i>LocalPositivePrefix&nbsp;&nbsp;</i>
	///         <dd>The string that appears before a positive number (may
	///         be empty), <i>i.e.,</i>&nbsp;<tt>df.</tt>{@link
	///         java.text.DecimalFormat#getPositivePrefix
	///         getPositivePrefix()}
	///     <dt><i>LocalPositiveSuffix&nbsp;&nbsp;</i>
	///         <dd>The string that appears after a positive number (may be
	///         empty), <i>i.e.,</i>&nbsp;<tt>df.</tt>{@link
	///         java.text.DecimalFormat#getPositiveSuffix
	///         getPositiveSuffix()}
	///     <dt><i>LocalNegativePrefix&nbsp;&nbsp;</i>
	///         <dd>The string that appears before a negative number (may
	///         be empty), <i>i.e.,</i>&nbsp;<tt>df.</tt>{@link
	///         java.text.DecimalFormat#getNegativePrefix
	///         getNegativePrefix()}
	///     <dt><i>LocalNegativeSuffix&nbsp;&nbsp;</i>
	///         <dd>The string that appears after a negative number (may be
	///         empty), <i>i.e.,</i>&nbsp;<tt>df.</tt>{@link
	///     java.text.DecimalFormat#getNegativeSuffix
	///     getNegativeSuffix()}
	///     <dt><i>LocalNaN&nbsp;&nbsp;</i>
	///         <dd>The string that represents not-a-number for
	///         floating-point values,
	///         <i>i.e.,</i>&nbsp;<tt>dfs.</tt>{@link
	///         java.text.DecimalFormatSymbols#getNaN
	///         getNaN()}
	///     <dt><i>LocalInfinity&nbsp;&nbsp;</i>
	///         <dd>The string that represents infinity for floating-point
	///         values, <i>i.e.,</i>&nbsp;<tt>dfs.</tt>{@link
	///         java.text.DecimalFormatSymbols#getInfinity
	///         getInfinity()}
	/// </dl></blockquote>
	/// 
	/// <h4> <a name="number-syntax">Number syntax</a> </h4>
	/// 
	/// </para>
	/// <para> The strings that can be parsed as numbers by an instance of this class
	/// are specified in terms of the following regular-expression grammar, where
	/// Rmax is the highest digit in the radix being used (for example, Rmax is 9 in base 10).
	/// 
	/// <dl>
	///   <dt><i>NonAsciiDigit</i>:
	///       <dd>A non-ASCII character c for which
	///            <seealso cref="java.lang.Character#isDigit Character.isDigit"/><tt>(c)</tt>
	///                        returns&nbsp;true
	/// 
	///   <dt><i>Non0Digit</i>:
	///       <dd><tt>[1-</tt><i>Rmax</i><tt>] | </tt><i>NonASCIIDigit</i>
	/// 
	///   <dt><i>Digit</i>:
	///       <dd><tt>[0-</tt><i>Rmax</i><tt>] | </tt><i>NonASCIIDigit</i>
	/// 
	///   <dt><i>GroupedNumeral</i>:
	///       <dd><tt>(&nbsp;</tt><i>Non0Digit</i>
	///                   <i>Digit</i><tt>?
	///                   </tt><i>Digit</i><tt>?</tt>
	///       <dd>&nbsp;&nbsp;&nbsp;&nbsp;<tt>(&nbsp;</tt><i>LocalGroupSeparator</i>
	///                         <i>Digit</i>
	///                         <i>Digit</i>
	///                         <i>Digit</i><tt> )+ )</tt>
	/// 
	///   <dt><i>Numeral</i>:
	///       <dd><tt>( ( </tt><i>Digit</i><tt>+ )
	///               | </tt><i>GroupedNumeral</i><tt> )</tt>
	/// 
	///   <dt><a name="Integer-regex"><i>Integer</i>:</a>
	///       <dd><tt>( [-+]? ( </tt><i>Numeral</i><tt>
	///                               ) )</tt>
	///       <dd><tt>| </tt><i>LocalPositivePrefix</i> <i>Numeral</i>
	///                      <i>LocalPositiveSuffix</i>
	///       <dd><tt>| </tt><i>LocalNegativePrefix</i> <i>Numeral</i>
	///                 <i>LocalNegativeSuffix</i>
	/// 
	///   <dt><i>DecimalNumeral</i>:
	///       <dd><i>Numeral</i>
	///       <dd><tt>| </tt><i>Numeral</i>
	///                 <i>LocalDecimalSeparator</i>
	///                 <i>Digit</i><tt>*</tt>
	///       <dd><tt>| </tt><i>LocalDecimalSeparator</i>
	///                 <i>Digit</i><tt>+</tt>
	/// 
	///   <dt><i>Exponent</i>:
	///       <dd><tt>( [eE] [+-]? </tt><i>Digit</i><tt>+ )</tt>
	/// 
	///   <dt><a name="Decimal-regex"><i>Decimal</i>:</a>
	///       <dd><tt>( [-+]? </tt><i>DecimalNumeral</i>
	///                         <i>Exponent</i><tt>? )</tt>
	///       <dd><tt>| </tt><i>LocalPositivePrefix</i>
	///                 <i>DecimalNumeral</i>
	///                 <i>LocalPositiveSuffix</i>
	///                 <i>Exponent</i><tt>?</tt>
	///       <dd><tt>| </tt><i>LocalNegativePrefix</i>
	///                 <i>DecimalNumeral</i>
	///                 <i>LocalNegativeSuffix</i>
	///                 <i>Exponent</i><tt>?</tt>
	/// 
	///   <dt><i>HexFloat</i>:
	///       <dd><tt>[-+]? 0[xX][0-9a-fA-F]*\.[0-9a-fA-F]+
	///                 ([pP][-+]?[0-9]+)?</tt>
	/// 
	///   <dt><i>NonNumber</i>:
	///       <dd><tt>NaN
	///                          | </tt><i>LocalNan</i><tt>
	///                          | Infinity
	///                          | </tt><i>LocalInfinity</i>
	/// 
	///   <dt><i>SignedNonNumber</i>:
	///       <dd><tt>( [-+]? </tt><i>NonNumber</i><tt> )</tt>
	///       <dd><tt>| </tt><i>LocalPositivePrefix</i>
	///                 <i>NonNumber</i>
	///                 <i>LocalPositiveSuffix</i>
	///       <dd><tt>| </tt><i>LocalNegativePrefix</i>
	///                 <i>NonNumber</i>
	///                 <i>LocalNegativeSuffix</i>
	/// 
	///   <dt><a name="Float-regex"><i>Float</i></a>:
	///       <dd><i>Decimal</i>
	///           <tt>| </tt><i>HexFloat</i>
	///           <tt>| </tt><i>SignedNonNumber</i>
	/// 
	/// </dl>
	/// </para>
	/// <para>Whitespace is not significant in the above regular expressions.
	/// 
	/// @since   1.5
	/// </para>
	/// </summary>
	public sealed class Scanner : Iterator<String>, Closeable
	{

		// Internal buffer used to hold input
		private CharBuffer Buf;

		// Size of internal character buffer
		private const int BUFFER_SIZE = 1024; // change to 1024;

		// The index into the buffer currently held by the Scanner
		private int Position;

		// Internal matcher used for finding delimiters
		private Matcher Matcher;

		// Pattern used to delimit tokens
		private Pattern DelimPattern;

		// Pattern found in last hasNext operation
		private Pattern HasNextPattern;

		// Position after last hasNext operation
		private int HasNextPosition;

		// Result after last hasNext operation
		private String HasNextResult;

		// The input source
		private Readable Source;

		// Boolean is true if source is done
		private bool SourceClosed = false;

		// Boolean indicating more input is required
		private bool NeedInput = false;

		// Boolean indicating if a delim has been skipped this operation
		private bool Skipped = false;

		// A store of a position that the scanner may fall back to
		private int SavedScannerPosition = -1;

		// A cache of the last primitive type scanned
		private Object TypeCache = null;

		// Boolean indicating if a match result is available
		private bool MatchValid = false;

		// Boolean indicating if this scanner has been closed
		private bool Closed = false;

		// The current radix used by this scanner
		private int Radix_Renamed = 10;

		// The default radix for this scanner
		private int DefaultRadix = 10;

		// The locale used by this scanner
		private Locale Locale_Renamed = null;

		// A cache of the last few recently used Patterns
		private LRUCache<String, Pattern> patternCache = new LRUCacheAnonymousInnerClassHelper();

		private class LRUCacheAnonymousInnerClassHelper : LRUCache<String, Pattern>
		{
			public LRUCacheAnonymousInnerClassHelper() : base(7)
			{
			}

			protected internal virtual Pattern Create(String s)
			{
				return Pattern.Compile(s);
			}
			protected internal virtual bool HasName(Pattern p, String s)
			{
				return p.Pattern().Equals(s);
			}
		}

		// A holder of the last IOException encountered
		private IOException LastException;

		// A pattern for java whitespace
		private static Pattern WHITESPACE_PATTERN = Pattern.Compile("\\p{javaWhitespace}+");

		// A pattern for any token
		private static Pattern FIND_ANY_PATTERN = Pattern.Compile("(?s).*");

		// A pattern for non-ASCII digits
		private static Pattern NON_ASCII_DIGIT = Pattern.Compile("[\\p{javaDigit}&&[^0-9]]");

		// Fields and methods to support scanning primitive types

		/// <summary>
		/// Locale dependent values used to scan numbers
		/// </summary>
		private String GroupSeparator = "\\,";
		private String DecimalSeparator = "\\.";
		private String NanString = "NaN";
		private String InfinityString = "Infinity";
		private String PositivePrefix = "";
		private String NegativePrefix = "\\-";
		private String PositiveSuffix = "";
		private String NegativeSuffix = "";

		/// <summary>
		/// Fields and an accessor method to match booleans
		/// </summary>
		private static volatile Pattern BoolPattern_Renamed;
		private const String BOOLEAN_PATTERN = "true|false";
		private static Pattern BoolPattern()
		{
			Pattern bp = BoolPattern_Renamed;
			if (bp == null)
			{
				BoolPattern_Renamed = bp = Pattern.Compile(BOOLEAN_PATTERN, Pattern.CASE_INSENSITIVE);
			}
			return bp;
		}

		/// <summary>
		/// Fields and methods to match bytes, shorts, ints, and longs
		/// </summary>
		private Pattern IntegerPattern_Renamed;
		private String Digits = "0123456789abcdefghijklmnopqrstuvwxyz";
		private String Non0Digit = "[\\p{javaDigit}&&[^0]]";
		private int SIMPLE_GROUP_INDEX = 5;
		private String BuildIntegerPatternString()
		{
			String radixDigits = Digits.Substring(0, Radix_Renamed);
			// \\p{javaDigit} is not guaranteed to be appropriate
			// here but what can we do? The final authority will be
			// whatever parse method is invoked, so ultimately the
			// Scanner will do the right thing
			String digit = "((?i)[" + radixDigits + "]|\\p{javaDigit})";
			String groupedNumeral = "(" + Non0Digit + digit + "?" + digit + "?(" + GroupSeparator + digit + digit + digit + ")+)";
			// digit++ is the possessive form which is necessary for reducing
			// backtracking that would otherwise cause unacceptable performance
			String numeral = "((" + digit + "++)|" + groupedNumeral + ")";
			String javaStyleInteger = "([-+]?(" + numeral + "))";
			String negativeInteger = NegativePrefix + numeral + NegativeSuffix;
			String positiveInteger = PositivePrefix + numeral + PositiveSuffix;
			return "(" + javaStyleInteger + ")|(" + positiveInteger + ")|(" + negativeInteger + ")";
		}
		private Pattern IntegerPattern()
		{
			if (IntegerPattern_Renamed == null)
			{
				IntegerPattern_Renamed = patternCache.forName(BuildIntegerPatternString());
			}
			return IntegerPattern_Renamed;
		}

		/// <summary>
		/// Fields and an accessor method to match line separators
		/// </summary>
		private static volatile Pattern SeparatorPattern_Renamed;
		private static volatile Pattern LinePattern_Renamed;
		private const String LINE_SEPARATOR_PATTERN = "\r\n|[\n\r\u2028\u2029\u0085]";
		private static readonly String LINE_PATTERN = ".*(" + LINE_SEPARATOR_PATTERN + ")|.+$";

		private static Pattern SeparatorPattern()
		{
			Pattern sp = SeparatorPattern_Renamed;
			if (sp == null)
			{
				SeparatorPattern_Renamed = sp = Pattern.Compile(LINE_SEPARATOR_PATTERN);
			}
			return sp;
		}

		private static Pattern LinePattern()
		{
			Pattern lp = LinePattern_Renamed;
			if (lp == null)
			{
				LinePattern_Renamed = lp = Pattern.Compile(LINE_PATTERN);
			}
			return lp;
		}

		/// <summary>
		/// Fields and methods to match floats and doubles
		/// </summary>
		private Pattern FloatPattern_Renamed;
		private Pattern DecimalPattern_Renamed;
		private void BuildFloatAndDecimalPattern()
		{
			// \\p{javaDigit} may not be perfect, see above
			String digit = "([0-9]|(\\p{javaDigit}))";
			String exponent = "([eE][+-]?" + digit + "+)?";
			String groupedNumeral = "(" + Non0Digit + digit + "?" + digit + "?(" + GroupSeparator + digit + digit + digit + ")+)";
			// Once again digit++ is used for performance, as above
			String numeral = "((" + digit + "++)|" + groupedNumeral + ")";
			String decimalNumeral = "(" + numeral + "|" + numeral + DecimalSeparator + digit + "*+|" + DecimalSeparator + digit + "++)";
			String nonNumber = "(NaN|" + NanString + "|Infinity|" + InfinityString + ")";
			String positiveFloat = "(" + PositivePrefix + decimalNumeral + PositiveSuffix + exponent + ")";
			String negativeFloat = "(" + NegativePrefix + decimalNumeral + NegativeSuffix + exponent + ")";
			String @decimal = "(([-+]?" + decimalNumeral + exponent + ")|" + positiveFloat + "|" + negativeFloat + ")";
			String hexFloat = "[-+]?0[xX][0-9a-fA-F]*\\.[0-9a-fA-F]+([pP][-+]?[0-9]+)?";
			String positiveNonNumber = "(" + PositivePrefix + nonNumber + PositiveSuffix + ")";
			String negativeNonNumber = "(" + NegativePrefix + nonNumber + NegativeSuffix + ")";
			String signedNonNumber = "(([-+]?" + nonNumber + ")|" + positiveNonNumber + "|" + negativeNonNumber + ")";
			FloatPattern_Renamed = Pattern.Compile(@decimal + "|" + hexFloat + "|" + signedNonNumber);
			DecimalPattern_Renamed = Pattern.Compile(@decimal);
		}
		private Pattern FloatPattern()
		{
			if (FloatPattern_Renamed == null)
			{
				BuildFloatAndDecimalPattern();
			}
			return FloatPattern_Renamed;
		}
		private Pattern DecimalPattern()
		{
			if (DecimalPattern_Renamed == null)
			{
				BuildFloatAndDecimalPattern();
			}
			return DecimalPattern_Renamed;
		}

		// Constructors

		/// <summary>
		/// Constructs a <code>Scanner</code> that returns values scanned
		/// from the specified source delimited by the specified pattern.
		/// </summary>
		/// <param name="source"> A character source implementing the Readable interface </param>
		/// <param name="pattern"> A delimiting pattern </param>
		private Scanner(Readable source, Pattern pattern)
		{
			Debug.Assert(source != null, "source should not be null");
			Debug.Assert(pattern != null, "pattern should not be null");
			this.Source = source;
			DelimPattern = pattern;
			Buf = CharBuffer.Allocate(BUFFER_SIZE);
			Buf.Limit(0);
			Matcher = DelimPattern.Matcher(Buf);
			Matcher.UseTransparentBounds(true);
			Matcher.UseAnchoringBounds(false);
			UseLocale(Locale.GetDefault(Locale.Category.FORMAT));
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified source.
		/// </summary>
		/// <param name="source"> A character source implementing the <seealso cref="Readable"/>
		///         interface </param>
		public Scanner(Readable source) : this(Objects.RequireNonNull(source, "source"), WHITESPACE_PATTERN)
		{
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified input stream. Bytes from the stream are converted
		/// into characters using the underlying platform's
		/// <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>.
		/// </summary>
		/// <param name="source"> An input stream to be scanned </param>
		public Scanner(InputStream source) : this(new InputStreamReader(source), WHITESPACE_PATTERN)
		{
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified input stream. Bytes from the stream are converted
		/// into characters using the specified charset.
		/// </summary>
		/// <param name="source"> An input stream to be scanned </param>
		/// <param name="charsetName"> The encoding type used to convert bytes from the
		///        stream into characters to be scanned </param>
		/// <exception cref="IllegalArgumentException"> if the specified character set
		///         does not exist </exception>
		public Scanner(InputStream source, String charsetName) : this(MakeReadable(Objects.RequireNonNull(source, "source"), ToCharset(charsetName)), WHITESPACE_PATTERN)
		{
		}

		/// <summary>
		/// Returns a charset object for the given charset name. </summary>
		/// <exception cref="NullPointerException">          is csn is null </exception>
		/// <exception cref="IllegalArgumentException">      if the charset is not supported </exception>
		private static Charset ToCharset(String csn)
		{
			Objects.RequireNonNull(csn, "charsetName");
			try
			{
				return Charset.ForName(csn);
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (IllegalCharsetNameException | UnsupportedCharsetException e)
			{
				// IllegalArgumentException should be thrown
				throw new IllegalArgumentException(e);
			}
		}

		private static Readable MakeReadable(InputStream source, Charset charset)
		{
			return new InputStreamReader(source, charset);
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified file. Bytes from the file are converted into
		/// characters using the underlying platform's
		/// <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>.
		/// </summary>
		/// <param name="source"> A file to be scanned </param>
		/// <exception cref="FileNotFoundException"> if source is not found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Scanner(File source) throws FileNotFoundException
		public Scanner(File source) : this((ReadableByteChannel)((new FileInputStream(source)).Channel))
		{
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified file. Bytes from the file are converted into
		/// characters using the specified charset.
		/// </summary>
		/// <param name="source"> A file to be scanned </param>
		/// <param name="charsetName"> The encoding type used to convert bytes from the file
		///        into characters to be scanned </param>
		/// <exception cref="FileNotFoundException"> if source is not found </exception>
		/// <exception cref="IllegalArgumentException"> if the specified encoding is
		///         not found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Scanner(File source, String charsetName) throws FileNotFoundException
		public Scanner(File source, String charsetName) : this(Objects.RequireNonNull(source), ToDecoder(charsetName))
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Scanner(File source, CharsetDecoder dec) throws FileNotFoundException
		private Scanner(File source, CharsetDecoder dec) : this(MakeReadable((ReadableByteChannel)((new FileInputStream(source)).Channel), dec))
		{
		}

		private static CharsetDecoder ToDecoder(String charsetName)
		{
			Objects.RequireNonNull(charsetName, "charsetName");
			try
			{
				return Charset.ForName(charsetName).NewDecoder();
			}
//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java 'multi-catch' syntax:
			catch (IllegalCharsetNameException | UnsupportedCharsetException unused)
			{
				throw new IllegalArgumentException(charsetName);
			}
		}

		private static Readable MakeReadable(ReadableByteChannel source, CharsetDecoder dec)
		{
			return Channels.NewReader(source, dec, -1);
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified file. Bytes from the file are converted into
		/// characters using the underlying platform's
		/// <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>.
		/// </summary>
		/// <param name="source">
		///          the path to the file to be scanned </param>
		/// <exception cref="IOException">
		///          if an I/O error occurs opening source
		/// 
		/// @since   1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Scanner(java.nio.file.Path source) throws IOException
		public Scanner(Path source) : this(Files.newInputStream(source))
		{
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified file. Bytes from the file are converted into
		/// characters using the specified charset.
		/// </summary>
		/// <param name="source">
		///          the path to the file to be scanned </param>
		/// <param name="charsetName">
		///          The encoding type used to convert bytes from the file
		///          into characters to be scanned </param>
		/// <exception cref="IOException">
		///          if an I/O error occurs opening source </exception>
		/// <exception cref="IllegalArgumentException">
		///          if the specified encoding is not found
		/// @since   1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Scanner(java.nio.file.Path source, String charsetName) throws IOException
		public Scanner(Path source, String charsetName) : this(Objects.RequireNonNull(source), ToCharset(charsetName))
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Scanner(java.nio.file.Path source, Charset charset) throws IOException
		private Scanner(Path source, Charset charset) : this(MakeReadable(Files.newInputStream(source), charset))
		{
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified string.
		/// </summary>
		/// <param name="source"> A string to scan </param>
		public Scanner(String source) : this(new StringReader(source), WHITESPACE_PATTERN)
		{
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified channel. Bytes from the source are converted into
		/// characters using the underlying platform's
		/// <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>.
		/// </summary>
		/// <param name="source"> A channel to scan </param>
		public Scanner(ReadableByteChannel source) : this(MakeReadable(Objects.RequireNonNull(source, "source")), WHITESPACE_PATTERN)
		{
		}

		private static Readable MakeReadable(ReadableByteChannel source)
		{
			return MakeReadable(source, Charset.DefaultCharset().NewDecoder());
		}

		/// <summary>
		/// Constructs a new <code>Scanner</code> that produces values scanned
		/// from the specified channel. Bytes from the source are converted into
		/// characters using the specified charset.
		/// </summary>
		/// <param name="source"> A channel to scan </param>
		/// <param name="charsetName"> The encoding type used to convert bytes from the
		///        channel into characters to be scanned </param>
		/// <exception cref="IllegalArgumentException"> if the specified character set
		///         does not exist </exception>
		public Scanner(ReadableByteChannel source, String charsetName) : this(MakeReadable(Objects.RequireNonNull(source, "source"), ToDecoder(charsetName)), WHITESPACE_PATTERN)
		{
		}

		// Private primitives used to support scanning

		private void SaveState()
		{
			SavedScannerPosition = Position;
		}

		private void RevertState()
		{
			this.Position = SavedScannerPosition;
			SavedScannerPosition = -1;
			Skipped = false;
		}

		private bool RevertState(bool b)
		{
			this.Position = SavedScannerPosition;
			SavedScannerPosition = -1;
			Skipped = false;
			return b;
		}

		private void CacheResult()
		{
			HasNextResult = Matcher.Group();
			HasNextPosition = Matcher.End();
			HasNextPattern = Matcher.Pattern();
		}

		private void CacheResult(String result)
		{
			HasNextResult = result;
			HasNextPosition = Matcher.End();
			HasNextPattern = Matcher.Pattern();
		}

		// Clears both regular cache and type cache
		private void ClearCaches()
		{
			HasNextPattern = null;
			TypeCache = null;
		}

		// Also clears both the regular cache and the type cache
		private String CachedResult
		{
			get
			{
				Position = HasNextPosition;
				HasNextPattern = null;
				TypeCache = null;
				return HasNextResult;
			}
		}

		// Also clears both the regular cache and the type cache
		private void UseTypeCache()
		{
			if (Closed)
			{
				throw new IllegalStateException("Scanner closed");
			}
			Position = HasNextPosition;
			HasNextPattern = null;
			TypeCache = null;
		}

		// Tries to read more input. May block.
		private void ReadInput()
		{
			if (Buf.Limit() == Buf.Capacity())
			{
				MakeSpace();
			}

			// Prepare to receive data
			int p = Buf.Position();
			Buf.Position(Buf.Limit());
			Buf.Limit(Buf.Capacity());

			int n = 0;
			try
			{
				n = Source.Read(Buf);
			}
			catch (IOException ioe)
			{
				LastException = ioe;
				n = -1;
			}

			if (n == -1)
			{
				SourceClosed = true;
				NeedInput = false;
			}

			if (n > 0)
			{
				NeedInput = false;
			}

			// Restore current position and limit for reading
			Buf.Limit(Buf.Position());
			Buf.Position(p);
		}

		// After this method is called there will either be an exception
		// or else there will be space in the buffer
		private bool MakeSpace()
		{
			ClearCaches();
			int offset = SavedScannerPosition == -1 ? Position : SavedScannerPosition;
			Buf.Position(offset);
			// Gain space by compacting buffer
			if (offset > 0)
			{
				Buf.Compact();
				TranslateSavedIndexes(offset);
				Position -= offset;
				Buf.Flip();
				return true;
			}
			// Gain space by growing buffer
			int newSize = Buf.Capacity() * 2;
			CharBuffer newBuf = CharBuffer.Allocate(newSize);
			newBuf.Put(Buf);
			newBuf.Flip();
			TranslateSavedIndexes(offset);
			Position -= offset;
			Buf = newBuf;
			Matcher.Reset(Buf);
			return true;
		}

		// When a buffer compaction/reallocation occurs the saved indexes must
		// be modified appropriately
		private void TranslateSavedIndexes(int offset)
		{
			if (SavedScannerPosition != -1)
			{
				SavedScannerPosition -= offset;
			}
		}

		// If we are at the end of input then NoSuchElement;
		// If there is still input left then InputMismatch
		private void ThrowFor()
		{
			Skipped = false;
			if ((SourceClosed) && (Position == Buf.Limit()))
			{
				throw new NoSuchElementException();
			}
			else
			{
				throw new InputMismatchException();
			}
		}

		// Returns true if a complete token or partial token is in the buffer.
		// It is not necessary to find a complete token since a partial token
		// means that there will be another token with or without more input.
		private bool HasTokenInBuffer()
		{
			MatchValid = false;
			Matcher.UsePattern(DelimPattern);
			Matcher.Region(Position, Buf.Limit());

			// Skip delims first
			if (Matcher.LookingAt())
			{
				Position = Matcher.End();
			}

			// If we are sitting at the end, no more tokens in buffer
			if (Position == Buf.Limit())
			{
				return false;
			}

			return true;
		}

		/*
		 * Returns a "complete token" that matches the specified pattern
		 *
		 * A token is complete if surrounded by delims; a partial token
		 * is prefixed by delims but not postfixed by them
		 *
		 * The position is advanced to the end of that complete token
		 *
		 * Pattern == null means accept any token at all
		 *
		 * Triple return:
		 * 1. valid string means it was found
		 * 2. null with needInput=false means we won't ever find it
		 * 3. null with needInput=true means try again after readInput
		 */
		private String GetCompleteTokenInBuffer(Pattern pattern)
		{
			MatchValid = false;

			// Skip delims first
			Matcher.UsePattern(DelimPattern);
			if (!Skipped) // Enforcing only one skip of leading delims
			{
				Matcher.Region(Position, Buf.Limit());
				if (Matcher.LookingAt())
				{
					// If more input could extend the delimiters then we must wait
					// for more input
					if (Matcher.HitEnd() && !SourceClosed)
					{
						NeedInput = true;
						return null;
					}
					// The delims were whole and the matcher should skip them
					Skipped = true;
					Position = Matcher.End();
				}
			}

			// If we are sitting at the end, no more tokens in buffer
			if (Position == Buf.Limit())
			{
				if (SourceClosed)
				{
					return null;
				}
				NeedInput = true;
				return null;
			}

			// Must look for next delims. Simply attempting to match the
			// pattern at this point may find a match but it might not be
			// the first longest match because of missing input, or it might
			// match a partial token instead of the whole thing.

			// Then look for next delims
			Matcher.Region(Position, Buf.Limit());
			bool foundNextDelim = Matcher.Find();
			if (foundNextDelim && (Matcher.End() == Position))
			{
				// Zero length delimiter match; we should find the next one
				// using the automatic advance past a zero length match;
				// Otherwise we have just found the same one we just skipped
				foundNextDelim = Matcher.Find();
			}
			if (foundNextDelim)
			{
				// In the rare case that more input could cause the match
				// to be lost and there is more input coming we must wait
				// for more input. Note that hitting the end is okay as long
				// as the match cannot go away. It is the beginning of the
				// next delims we want to be sure about, we don't care if
				// they potentially extend further.
				if (Matcher.RequireEnd() && !SourceClosed)
				{
					NeedInput = true;
					return null;
				}
				int tokenEnd = Matcher.Start();
				// There is a complete token.
				if (pattern == null)
				{
					// Must continue with match to provide valid MatchResult
					pattern = FIND_ANY_PATTERN;
				}
				//  Attempt to match against the desired pattern
				Matcher.UsePattern(pattern);
				Matcher.Region(Position, tokenEnd);
				if (Matcher.Matches())
				{
					String s = Matcher.Group();
					Position = Matcher.End();
					return s;
				} // Complete token but it does not match
				else
				{
					return null;
				}
			}

			// If we can't find the next delims but no more input is coming,
			// then we can treat the remainder as a whole token
			if (SourceClosed)
			{
				if (pattern == null)
				{
					// Must continue with match to provide valid MatchResult
					pattern = FIND_ANY_PATTERN;
				}
				// Last token; Match the pattern here or throw
				Matcher.UsePattern(pattern);
				Matcher.Region(Position, Buf.Limit());
				if (Matcher.Matches())
				{
					String s = Matcher.Group();
					Position = Matcher.End();
					return s;
				}
				// Last piece does not match
				return null;
			}

			// There is a partial token in the buffer; must read more
			// to complete it
			NeedInput = true;
			return null;
		}

		// Finds the specified pattern in the buffer up to horizon.
		// Returns a match for the specified input pattern.
		private String FindPatternInBuffer(Pattern pattern, int horizon)
		{
			MatchValid = false;
			Matcher.UsePattern(pattern);
			int bufferLimit = Buf.Limit();
			int horizonLimit = -1;
			int searchLimit = bufferLimit;
			if (horizon > 0)
			{
				horizonLimit = Position + horizon;
				if (horizonLimit < bufferLimit)
				{
					searchLimit = horizonLimit;
				}
			}
			Matcher.Region(Position, searchLimit);
			if (Matcher.Find())
			{
				if (Matcher.HitEnd() && (!SourceClosed))
				{
					// The match may be longer if didn't hit horizon or real end
					if (searchLimit != horizonLimit)
					{
						 // Hit an artificial end; try to extend the match
						NeedInput = true;
						return null;
					}
					// The match could go away depending on what is next
					if ((searchLimit == horizonLimit) && Matcher.RequireEnd())
					{
						// Rare case: we hit the end of input and it happens
						// that it is at the horizon and the end of input is
						// required for the match.
						NeedInput = true;
						return null;
					}
				}
				// Did not hit end, or hit real end, or hit horizon
				Position = Matcher.End();
				return Matcher.Group();
			}

			if (SourceClosed)
			{
				return null;
			}

			// If there is no specified horizon, or if we have not searched
			// to the specified horizon yet, get more input
			if ((horizon == 0) || (searchLimit != horizonLimit))
			{
				NeedInput = true;
			}
			return null;
		}

		// Returns a match for the specified input pattern anchored at
		// the current position
		private String MatchPatternInBuffer(Pattern pattern)
		{
			MatchValid = false;
			Matcher.UsePattern(pattern);
			Matcher.Region(Position, Buf.Limit());
			if (Matcher.LookingAt())
			{
				if (Matcher.HitEnd() && (!SourceClosed))
				{
					// Get more input and try again
					NeedInput = true;
					return null;
				}
				Position = Matcher.End();
				return Matcher.Group();
			}

			if (SourceClosed)
			{
				return null;
			}

			// Read more to find pattern
			NeedInput = true;
			return null;
		}

		// Throws if the scanner is closed
		private void EnsureOpen()
		{
			if (Closed)
			{
				throw new IllegalStateException("Scanner closed");
			}
		}

		// Public methods

		/// <summary>
		/// Closes this scanner.
		/// 
		/// <para> If this scanner has not yet been closed then if its underlying
		/// <seealso cref="java.lang.Readable readable"/> also implements the {@link
		/// java.io.Closeable} interface then the readable's <tt>close</tt> method
		/// will be invoked.  If this scanner is already closed then invoking this
		/// method will have no effect.
		/// 
		/// </para>
		/// <para>Attempting to perform search operations after a scanner has
		/// been closed will result in an <seealso cref="IllegalStateException"/>.
		/// 
		/// </para>
		/// </summary>
		public void Close()
		{
			if (Closed)
			{
				return;
			}
			if (Source is Closeable)
			{
				try
				{
					((Closeable)Source).Close();
				}
				catch (IOException ioe)
				{
					LastException = ioe;
				}
			}
			SourceClosed = true;
			Source = null;
			Closed = true;
		}

		/// <summary>
		/// Returns the <code>IOException</code> last thrown by this
		/// <code>Scanner</code>'s underlying <code>Readable</code>. This method
		/// returns <code>null</code> if no such exception exists.
		/// </summary>
		/// <returns> the last exception thrown by this scanner's readable </returns>
		public IOException IoException()
		{
			return LastException;
		}

		/// <summary>
		/// Returns the <code>Pattern</code> this <code>Scanner</code> is currently
		/// using to match delimiters.
		/// </summary>
		/// <returns> this scanner's delimiting pattern. </returns>
		public Pattern Delimiter()
		{
			return DelimPattern;
		}

		/// <summary>
		/// Sets this scanner's delimiting pattern to the specified pattern.
		/// </summary>
		/// <param name="pattern"> A delimiting pattern </param>
		/// <returns> this scanner </returns>
		public Scanner UseDelimiter(Pattern pattern)
		{
			DelimPattern = pattern;
			return this;
		}

		/// <summary>
		/// Sets this scanner's delimiting pattern to a pattern constructed from
		/// the specified <code>String</code>.
		/// 
		/// <para> An invocation of this method of the form
		/// <tt>useDelimiter(pattern)</tt> behaves in exactly the same way as the
		/// invocation <tt>useDelimiter(Pattern.compile(pattern))</tt>.
		/// 
		/// </para>
		/// <para> Invoking the <seealso cref="#reset"/> method will set the scanner's delimiter
		/// to the <a href= "#default-delimiter">default</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> A string specifying a delimiting pattern </param>
		/// <returns> this scanner </returns>
		public Scanner UseDelimiter(String pattern)
		{
			DelimPattern = patternCache.forName(pattern);
			return this;
		}

		/// <summary>
		/// Returns this scanner's locale.
		/// 
		/// <para>A scanner's locale affects many elements of its default
		/// primitive matching regular expressions; see
		/// <a href= "#localized-numbers">localized numbers</a> above.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this scanner's locale </returns>
		public Locale Locale()
		{
			return this.Locale_Renamed;
		}

		/// <summary>
		/// Sets this scanner's locale to the specified locale.
		/// 
		/// <para>A scanner's locale affects many elements of its default
		/// primitive matching regular expressions; see
		/// <a href= "#localized-numbers">localized numbers</a> above.
		/// 
		/// </para>
		/// <para>Invoking the <seealso cref="#reset"/> method will set the scanner's locale to
		/// the <a href= "#initial-locale">initial locale</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale"> A string specifying the locale to use </param>
		/// <returns> this scanner </returns>
		public Scanner UseLocale(Locale locale)
		{
			if (locale.Equals(this.Locale_Renamed))
			{
				return this;
			}

			this.Locale_Renamed = locale;
			DecimalFormat df = (DecimalFormat)NumberFormat.GetNumberInstance(locale);
			DecimalFormatSymbols dfs = DecimalFormatSymbols.GetInstance(locale);

			// These must be literalized to avoid collision with regex
			// metacharacters such as dot or parenthesis
			GroupSeparator = "\\" + dfs.GroupingSeparator;
			DecimalSeparator = "\\" + dfs.DecimalSeparator;

			// Quoting the nonzero length locale-specific things
			// to avoid potential conflict with metacharacters
			NanString = "\\Q" + dfs.NaN + "\\E";
			InfinityString = "\\Q" + dfs.Infinity + "\\E";
			PositivePrefix = df.PositivePrefix;
			if (PositivePrefix.Length() > 0)
			{
				PositivePrefix = "\\Q" + PositivePrefix + "\\E";
			}
			NegativePrefix = df.NegativePrefix;
			if (NegativePrefix.Length() > 0)
			{
				NegativePrefix = "\\Q" + NegativePrefix + "\\E";
			}
			PositiveSuffix = df.PositiveSuffix;
			if (PositiveSuffix.Length() > 0)
			{
				PositiveSuffix = "\\Q" + PositiveSuffix + "\\E";
			}
			NegativeSuffix = df.NegativeSuffix;
			if (NegativeSuffix.Length() > 0)
			{
				NegativeSuffix = "\\Q" + NegativeSuffix + "\\E";
			}

			// Force rebuilding and recompilation of locale dependent
			// primitive patterns
			IntegerPattern_Renamed = null;
			FloatPattern_Renamed = null;

			return this;
		}

		/// <summary>
		/// Returns this scanner's default radix.
		/// 
		/// <para>A scanner's radix affects elements of its default
		/// number matching regular expressions; see
		/// <a href= "#localized-numbers">localized numbers</a> above.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the default radix of this scanner </returns>
		public int Radix()
		{
			return this.DefaultRadix;
		}

		/// <summary>
		/// Sets this scanner's default radix to the specified radix.
		/// 
		/// <para>A scanner's radix affects elements of its default
		/// number matching regular expressions; see
		/// <a href= "#localized-numbers">localized numbers</a> above.
		/// 
		/// </para>
		/// <para>If the radix is less than <code>Character.MIN_RADIX</code>
		/// or greater than <code>Character.MAX_RADIX</code>, then an
		/// <code>IllegalArgumentException</code> is thrown.
		/// 
		/// </para>
		/// <para>Invoking the <seealso cref="#reset"/> method will set the scanner's radix to
		/// <code>10</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="radix"> The radix to use when scanning numbers </param>
		/// <returns> this scanner </returns>
		/// <exception cref="IllegalArgumentException"> if radix is out of range </exception>
		public Scanner UseRadix(int radix)
		{
			if ((radix < Character.MIN_RADIX) || (radix > Character.MAX_RADIX))
			{
				throw new IllegalArgumentException("radix:" + radix);
			}

			if (this.DefaultRadix == radix)
			{
				return this;
			}
			this.DefaultRadix = radix;
			// Force rebuilding and recompilation of radix dependent patterns
			IntegerPattern_Renamed = null;
			return this;
		}

		// The next operation should occur in the specified radix but
		// the default is left untouched.
		private int Radix
		{
			set
			{
				if (this.Radix_Renamed != value)
				{
					// Force rebuilding and recompilation of value dependent patterns
					IntegerPattern_Renamed = null;
					this.Radix_Renamed = value;
				}
			}
		}

		/// <summary>
		/// Returns the match result of the last scanning operation performed
		/// by this scanner. This method throws <code>IllegalStateException</code>
		/// if no match has been performed, or if the last match was
		/// not successful.
		/// 
		/// <para>The various <code>next</code>methods of <code>Scanner</code>
		/// make a match result available if they complete without throwing an
		/// exception. For instance, after an invocation of the <seealso cref="#nextInt"/>
		/// method that returned an int, this method returns a
		/// <code>MatchResult</code> for the search of the
		/// <a href="#Integer-regex"><i>Integer</i></a> regular expression
		/// defined above. Similarly the <seealso cref="#findInLine"/>,
		/// <seealso cref="#findWithinHorizon"/>, and <seealso cref="#skip"/> methods will make a
		/// match available if they succeed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a match result for the last match operation </returns>
		/// <exception cref="IllegalStateException">  If no match result is available </exception>
		public MatchResult Match()
		{
			if (!MatchValid)
			{
				throw new IllegalStateException("No match result available");
			}
			return Matcher.ToMatchResult();
		}

		/// <summary>
		/// <para>Returns the string representation of this <code>Scanner</code>. The
		/// string representation of a <code>Scanner</code> contains information
		/// that may be useful for debugging. The exact format is unspecified.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The string representation of this scanner </returns>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("java.util.Scanner");
			sb.Append("[delimiters=" + DelimPattern + "]");
			sb.Append("[position=" + Position + "]");
			sb.Append("[match valid=" + MatchValid + "]");
			sb.Append("[need input=" + NeedInput + "]");
			sb.Append("[source closed=" + SourceClosed + "]");
			sb.Append("[skipped=" + Skipped + "]");
			sb.Append("[group separator=" + GroupSeparator + "]");
			sb.Append("[decimal separator=" + DecimalSeparator + "]");
			sb.Append("[positive prefix=" + PositivePrefix + "]");
			sb.Append("[negative prefix=" + NegativePrefix + "]");
			sb.Append("[positive suffix=" + PositiveSuffix + "]");
			sb.Append("[negative suffix=" + NegativeSuffix + "]");
			sb.Append("[NaN string=" + NanString + "]");
			sb.Append("[infinity string=" + InfinityString + "]");
			return sb.ToString();
		}

		/// <summary>
		/// Returns true if this scanner has another token in its input.
		/// This method may block while waiting for input to scan.
		/// The scanner does not advance past any input.
		/// </summary>
		/// <returns> true if and only if this scanner has another token </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		/// <seealso cref= java.util.Iterator </seealso>
		public bool HasNext()
		{
			EnsureOpen();
			SaveState();
			while (!SourceClosed)
			{
				if (HasTokenInBuffer())
				{
					return RevertState(true);
				}
				ReadInput();
			}
			bool result = HasTokenInBuffer();
			return RevertState(result);
		}

		/// <summary>
		/// Finds and returns the next complete token from this scanner.
		/// A complete token is preceded and followed by input that matches
		/// the delimiter pattern. This method may block while waiting for input
		/// to scan, even if a previous invocation of <seealso cref="#hasNext"/> returned
		/// <code>true</code>.
		/// </summary>
		/// <returns> the next token </returns>
		/// <exception cref="NoSuchElementException"> if no more tokens are available </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		/// <seealso cref= java.util.Iterator </seealso>
		public String Next()
		{
			EnsureOpen();
			ClearCaches();

			while (true)
			{
				String token = GetCompleteTokenInBuffer(null);
				if (token != null)
				{
					MatchValid = true;
					Skipped = false;
					return token;
				}
				if (NeedInput)
				{
					ReadInput();
				}
				else
				{
					ThrowFor();
				}
			}
		}

		/// <summary>
		/// The remove operation is not supported by this implementation of
		/// <code>Iterator</code>.
		/// </summary>
		/// <exception cref="UnsupportedOperationException"> if this method is invoked. </exception>
		/// <seealso cref= java.util.Iterator </seealso>
		public void Remove()
		{
			throw new UnsupportedOperationException();
		}

		/// <summary>
		/// Returns true if the next token matches the pattern constructed from the
		/// specified string. The scanner does not advance past any input.
		/// 
		/// <para> An invocation of this method of the form <tt>hasNext(pattern)</tt>
		/// behaves in exactly the same way as the invocation
		/// <tt>hasNext(Pattern.compile(pattern))</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a string specifying the pattern to scan </param>
		/// <returns> true if and only if this scanner has another token matching
		///         the specified pattern </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNext(String pattern)
		{
			return HasNext(patternCache.forName(pattern));
		}

		/// <summary>
		/// Returns the next token if it matches the pattern constructed from the
		/// specified string.  If the match is successful, the scanner advances
		/// past the input that matched the pattern.
		/// 
		/// <para> An invocation of this method of the form <tt>next(pattern)</tt>
		/// behaves in exactly the same way as the invocation
		/// <tt>next(Pattern.compile(pattern))</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a string specifying the pattern to scan </param>
		/// <returns> the next token </returns>
		/// <exception cref="NoSuchElementException"> if no such tokens are available </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public String Next(String pattern)
		{
			return Next(patternCache.forName(pattern));
		}

		/// <summary>
		/// Returns true if the next complete token matches the specified pattern.
		/// A complete token is prefixed and postfixed by input that matches
		/// the delimiter pattern. This method may block while waiting for input.
		/// The scanner does not advance past any input.
		/// </summary>
		/// <param name="pattern"> the pattern to scan for </param>
		/// <returns> true if and only if this scanner has another token matching
		///         the specified pattern </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNext(Pattern pattern)
		{
			EnsureOpen();
			if (pattern == null)
			{
				throw new NullPointerException();
			}
			HasNextPattern = null;
			SaveState();

			while (true)
			{
				if (GetCompleteTokenInBuffer(pattern) != null)
				{
					MatchValid = true;
					CacheResult();
					return RevertState(true);
				}
				if (NeedInput)
				{
					ReadInput();
				}
				else
				{
					return RevertState(false);
				}
			}
		}

		/// <summary>
		/// Returns the next token if it matches the specified pattern. This
		/// method may block while waiting for input to scan, even if a previous
		/// invocation of <seealso cref="#hasNext(Pattern)"/> returned <code>true</code>.
		/// If the match is successful, the scanner advances past the input that
		/// matched the pattern.
		/// </summary>
		/// <param name="pattern"> the pattern to scan for </param>
		/// <returns> the next token </returns>
		/// <exception cref="NoSuchElementException"> if no more tokens are available </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public String Next(Pattern pattern)
		{
			EnsureOpen();
			if (pattern == null)
			{
				throw new NullPointerException();
			}

			// Did we already find this pattern?
			if (HasNextPattern == pattern)
			{
				return CachedResult;
			}
			ClearCaches();

			// Search for the pattern
			while (true)
			{
				String token = GetCompleteTokenInBuffer(pattern);
				if (token != null)
				{
					MatchValid = true;
					Skipped = false;
					return token;
				}
				if (NeedInput)
				{
					ReadInput();
				}
				else
				{
					ThrowFor();
				}
			}
		}

		/// <summary>
		/// Returns true if there is another line in the input of this scanner.
		/// This method may block while waiting for input. The scanner does not
		/// advance past any input.
		/// </summary>
		/// <returns> true if and only if this scanner has another line of input </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextLine()
		{
			SaveState();

			String result = FindWithinHorizon(LinePattern(), 0);
			if (result != null)
			{
				MatchResult mr = this.Match();
				String lineSep = mr.Group(1);
				if (lineSep != null)
				{
					result = result.Substring(0, result.Length() - lineSep.Length());
					CacheResult(result);

				}
				else
				{
					CacheResult();
				}
			}
			RevertState();
			return (result != null);
		}

		/// <summary>
		/// Advances this scanner past the current line and returns the input
		/// that was skipped.
		/// 
		/// This method returns the rest of the current line, excluding any line
		/// separator at the end. The position is set to the beginning of the next
		/// line.
		/// 
		/// <para>Since this method continues to search through the input looking
		/// for a line separator, it may buffer all of the input searching for
		/// the line to skip if no line separators are present.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the line that was skipped </returns>
		/// <exception cref="NoSuchElementException"> if no line was found </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public String NextLine()
		{
			if (HasNextPattern == LinePattern())
			{
				return CachedResult;
			}
			ClearCaches();

			String result = FindWithinHorizon(LinePattern_Renamed, 0);
			if (result == null)
			{
				throw new NoSuchElementException("No line found");
			}
			MatchResult mr = this.Match();
			String lineSep = mr.Group(1);
			if (lineSep != null)
			{
				result = result.Substring(0, result.Length() - lineSep.Length());
			}
			if (result == null)
			{
				throw new NoSuchElementException();
			}
			else
			{
				return result;
			}
		}

		// Public methods that ignore delimiters

		/// <summary>
		/// Attempts to find the next occurrence of a pattern constructed from the
		/// specified string, ignoring delimiters.
		/// 
		/// <para>An invocation of this method of the form <tt>findInLine(pattern)</tt>
		/// behaves in exactly the same way as the invocation
		/// <tt>findInLine(Pattern.compile(pattern))</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a string specifying the pattern to search for </param>
		/// <returns> the text that matched the specified pattern </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public String FindInLine(String pattern)
		{
			return FindInLine(patternCache.forName(pattern));
		}

		/// <summary>
		/// Attempts to find the next occurrence of the specified pattern ignoring
		/// delimiters. If the pattern is found before the next line separator, the
		/// scanner advances past the input that matched and returns the string that
		/// matched the pattern.
		/// If no such pattern is detected in the input up to the next line
		/// separator, then <code>null</code> is returned and the scanner's
		/// position is unchanged. This method may block waiting for input that
		/// matches the pattern.
		/// 
		/// <para>Since this method continues to search through the input looking
		/// for the specified pattern, it may buffer all of the input searching for
		/// the desired token if no line separators are present.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> the pattern to scan for </param>
		/// <returns> the text that matched the specified pattern </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public String FindInLine(Pattern pattern)
		{
			EnsureOpen();
			if (pattern == null)
			{
				throw new NullPointerException();
			}
			ClearCaches();
			// Expand buffer to include the next newline or end of input
			int endPosition = 0;
			SaveState();
			while (true)
			{
				String token = FindPatternInBuffer(SeparatorPattern(), 0);
				if (token != null)
				{
					endPosition = Matcher.Start();
					break; // up to next newline
				}
				if (NeedInput)
				{
					ReadInput();
				}
				else
				{
					endPosition = Buf.Limit();
					break; // up to end of input
				}
			}
			RevertState();
			int horizonForLine = endPosition - Position;
			// If there is nothing between the current pos and the next
			// newline simply return null, invoking findWithinHorizon
			// with "horizon=0" will scan beyond the line bound.
			if (horizonForLine == 0)
			{
				return null;
			}
			// Search for the pattern
			return FindWithinHorizon(pattern, horizonForLine);
		}

		/// <summary>
		/// Attempts to find the next occurrence of a pattern constructed from the
		/// specified string, ignoring delimiters.
		/// 
		/// <para>An invocation of this method of the form
		/// <tt>findWithinHorizon(pattern)</tt> behaves in exactly the same way as
		/// the invocation
		/// <tt>findWithinHorizon(Pattern.compile(pattern, horizon))</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a string specifying the pattern to search for </param>
		/// <param name="horizon"> the search horizon </param>
		/// <returns> the text that matched the specified pattern </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		/// <exception cref="IllegalArgumentException"> if horizon is negative </exception>
		public String FindWithinHorizon(String pattern, int horizon)
		{
			return FindWithinHorizon(patternCache.forName(pattern), horizon);
		}

		/// <summary>
		/// Attempts to find the next occurrence of the specified pattern.
		/// 
		/// <para>This method searches through the input up to the specified
		/// search horizon, ignoring delimiters. If the pattern is found the
		/// scanner advances past the input that matched and returns the string
		/// that matched the pattern. If no such pattern is detected then the
		/// null is returned and the scanner's position remains unchanged. This
		/// method may block waiting for input that matches the pattern.
		/// 
		/// </para>
		/// <para>A scanner will never search more than <code>horizon</code> code
		/// points beyond its current position. Note that a match may be clipped
		/// by the horizon; that is, an arbitrary match result may have been
		/// different if the horizon had been larger. The scanner treats the
		/// horizon as a transparent, non-anchoring bound (see {@link
		/// Matcher#useTransparentBounds} and <seealso cref="Matcher#useAnchoringBounds"/>).
		/// 
		/// </para>
		/// <para>If horizon is <code>0</code>, then the horizon is ignored and
		/// this method continues to search through the input looking for the
		/// specified pattern without bound. In this case it may buffer all of
		/// the input searching for the pattern.
		/// 
		/// </para>
		/// <para>If horizon is negative, then an IllegalArgumentException is
		/// thrown.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> the pattern to scan for </param>
		/// <param name="horizon"> the search horizon </param>
		/// <returns> the text that matched the specified pattern </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		/// <exception cref="IllegalArgumentException"> if horizon is negative </exception>
		public String FindWithinHorizon(Pattern pattern, int horizon)
		{
			EnsureOpen();
			if (pattern == null)
			{
				throw new NullPointerException();
			}
			if (horizon < 0)
			{
				throw new IllegalArgumentException("horizon < 0");
			}
			ClearCaches();

			// Search for the pattern
			while (true)
			{
				String token = FindPatternInBuffer(pattern, horizon);
				if (token != null)
				{
					MatchValid = true;
					return token;
				}
				if (NeedInput)
				{
					ReadInput();
				}
				else
				{
					break; // up to end of input
				}
			}
			return null;
		}

		/// <summary>
		/// Skips input that matches the specified pattern, ignoring delimiters.
		/// This method will skip input if an anchored match of the specified
		/// pattern succeeds.
		/// 
		/// <para>If a match to the specified pattern is not found at the
		/// current position, then no input is skipped and a
		/// <tt>NoSuchElementException</tt> is thrown.
		/// 
		/// </para>
		/// <para>Since this method seeks to match the specified pattern starting at
		/// the scanner's current position, patterns that can match a lot of
		/// input (".*", for example) may cause the scanner to buffer a large
		/// amount of input.
		/// 
		/// </para>
		/// <para>Note that it is possible to skip something without risking a
		/// <code>NoSuchElementException</code> by using a pattern that can
		/// match nothing, e.g., <code>sc.skip("[ \t]*")</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a string specifying the pattern to skip over </param>
		/// <returns> this scanner </returns>
		/// <exception cref="NoSuchElementException"> if the specified pattern is not found </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public Scanner Skip(Pattern pattern)
		{
			EnsureOpen();
			if (pattern == null)
			{
				throw new NullPointerException();
			}
			ClearCaches();

			// Search for the pattern
			while (true)
			{
				String token = MatchPatternInBuffer(pattern);
				if (token != null)
				{
					MatchValid = true;
					Position = Matcher.End();
					return this;
				}
				if (NeedInput)
				{
					ReadInput();
				}
				else
				{
					throw new NoSuchElementException();
				}
			}
		}

		/// <summary>
		/// Skips input that matches a pattern constructed from the specified
		/// string.
		/// 
		/// <para> An invocation of this method of the form <tt>skip(pattern)</tt>
		/// behaves in exactly the same way as the invocation
		/// <tt>skip(Pattern.compile(pattern))</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="pattern"> a string specifying the pattern to skip over </param>
		/// <returns> this scanner </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public Scanner Skip(String pattern)
		{
			return Skip(patternCache.forName(pattern));
		}

		// Convenience methods for scanning primitives

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a boolean value using a case insensitive pattern
		/// created from the string "true|false".  The scanner does not
		/// advance past the input that matched.
		/// </summary>
		/// <returns> true if and only if this scanner's next token is a valid
		///         boolean value </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextBoolean()
		{
			return HasNext(BoolPattern());
		}

		/// <summary>
		/// Scans the next token of the input into a boolean value and returns
		/// that value. This method will throw <code>InputMismatchException</code>
		/// if the next token cannot be translated into a valid boolean value.
		/// If the match is successful, the scanner advances past the input that
		/// matched.
		/// </summary>
		/// <returns> the boolean scanned from the input </returns>
		/// <exception cref="InputMismatchException"> if the next token is not a valid boolean </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool NextBoolean()
		{
			ClearCaches();
			return Convert.ToBoolean(Next(BoolPattern()));
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a byte value in the default radix using the
		/// <seealso cref="#nextByte"/> method. The scanner does not advance past any input.
		/// </summary>
		/// <returns> true if and only if this scanner's next token is a valid
		///         byte value </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextByte()
		{
			return HasNextByte(DefaultRadix);
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a byte value in the specified radix using the
		/// <seealso cref="#nextByte"/> method. The scanner does not advance past any input.
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token as a byte value </param>
		/// <returns> true if and only if this scanner's next token is a valid
		///         byte value </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextByte(int radix)
		{
			Radix = radix;
			bool result = HasNext(IntegerPattern());
			if (result) // Cache it
			{
				try
				{
					String s = (Matcher.Group(SIMPLE_GROUP_INDEX) == null) ? ProcessIntegerToken(HasNextResult) : HasNextResult;
					TypeCache = Convert.ToByte(s, radix);
				}
				catch (NumberFormatException)
				{
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// Scans the next token of the input as a <tt>byte</tt>.
		/// 
		/// <para> An invocation of this method of the form
		/// <tt>nextByte()</tt> behaves in exactly the same way as the
		/// invocation <tt>nextByte(radix)</tt>, where <code>radix</code>
		/// is the default radix of this scanner.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <tt>byte</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public sbyte NextByte()
		{
			 return NextByte(DefaultRadix);
		}

		/// <summary>
		/// Scans the next token of the input as a <tt>byte</tt>.
		/// This method will throw <code>InputMismatchException</code>
		/// if the next token cannot be translated into a valid byte value as
		/// described below. If the translation is successful, the scanner advances
		/// past the input that matched.
		/// 
		/// <para> If the next token matches the <a
		/// href="#Integer-regex"><i>Integer</i></a> regular expression defined
		/// above then the token is converted into a <tt>byte</tt> value as if by
		/// removing all locale specific prefixes, group separators, and locale
		/// specific suffixes, then mapping non-ASCII digits into ASCII
		/// digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		/// negative sign (-) if the locale specific negative prefixes and suffixes
		/// were present, and passing the resulting string to
		/// <seealso cref="Byte#parseByte(String, int) Byte.parseByte"/> with the
		/// specified radix.
		/// 
		/// </para>
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token as a byte value </param>
		/// <returns> the <tt>byte</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public sbyte NextByte(int radix)
		{
			// Check cached result
			if ((TypeCache != null) && (TypeCache is Byte) && this.Radix_Renamed == radix)
			{
				sbyte val = ((Byte)TypeCache).ByteValue();
				UseTypeCache();
				return val;
			}
			Radix = radix;
			ClearCaches();
			// Search for next byte
			try
			{
				String s = Next(IntegerPattern());
				if (Matcher.Group(SIMPLE_GROUP_INDEX) == null)
				{
					s = ProcessIntegerToken(s);
				}
				return Convert.ToByte(s, radix);
			}
			catch (NumberFormatException nfe)
			{
				Position = Matcher.Start(); // don't skip bad token
				throw new InputMismatchException(nfe.Message);
			}
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a short value in the default radix using the
		/// <seealso cref="#nextShort"/> method. The scanner does not advance past any input.
		/// </summary>
		/// <returns> true if and only if this scanner's next token is a valid
		///         short value in the default radix </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextShort()
		{
			return HasNextShort(DefaultRadix);
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a short value in the specified radix using the
		/// <seealso cref="#nextShort"/> method. The scanner does not advance past any input.
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token as a short value </param>
		/// <returns> true if and only if this scanner's next token is a valid
		///         short value in the specified radix </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextShort(int radix)
		{
			Radix = radix;
			bool result = HasNext(IntegerPattern());
			if (result) // Cache it
			{
				try
				{
					String s = (Matcher.Group(SIMPLE_GROUP_INDEX) == null) ? ProcessIntegerToken(HasNextResult) : HasNextResult;
					TypeCache = Convert.ToInt16(s, radix);
				}
				catch (NumberFormatException)
				{
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// Scans the next token of the input as a <tt>short</tt>.
		/// 
		/// <para> An invocation of this method of the form
		/// <tt>nextShort()</tt> behaves in exactly the same way as the
		/// invocation <tt>nextShort(radix)</tt>, where <code>radix</code>
		/// is the default radix of this scanner.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <tt>short</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public short NextShort()
		{
			return NextShort(DefaultRadix);
		}

		/// <summary>
		/// Scans the next token of the input as a <tt>short</tt>.
		/// This method will throw <code>InputMismatchException</code>
		/// if the next token cannot be translated into a valid short value as
		/// described below. If the translation is successful, the scanner advances
		/// past the input that matched.
		/// 
		/// <para> If the next token matches the <a
		/// href="#Integer-regex"><i>Integer</i></a> regular expression defined
		/// above then the token is converted into a <tt>short</tt> value as if by
		/// removing all locale specific prefixes, group separators, and locale
		/// specific suffixes, then mapping non-ASCII digits into ASCII
		/// digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		/// negative sign (-) if the locale specific negative prefixes and suffixes
		/// were present, and passing the resulting string to
		/// <seealso cref="Short#parseShort(String, int) Short.parseShort"/> with the
		/// specified radix.
		/// 
		/// </para>
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token as a short value </param>
		/// <returns> the <tt>short</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public short NextShort(int radix)
		{
			// Check cached result
			if ((TypeCache != null) && (TypeCache is Short) && this.Radix_Renamed == radix)
			{
				short val = ((Short)TypeCache).ShortValue();
				UseTypeCache();
				return val;
			}
			Radix = radix;
			ClearCaches();
			// Search for next short
			try
			{
				String s = Next(IntegerPattern());
				if (Matcher.Group(SIMPLE_GROUP_INDEX) == null)
				{
					s = ProcessIntegerToken(s);
				}
				return Convert.ToInt16(s, radix);
			}
			catch (NumberFormatException nfe)
			{
				Position = Matcher.Start(); // don't skip bad token
				throw new InputMismatchException(nfe.Message);
			}
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as an int value in the default radix using the
		/// <seealso cref="#nextInt"/> method. The scanner does not advance past any input.
		/// </summary>
		/// <returns> true if and only if this scanner's next token is a valid
		///         int value </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextInt()
		{
			return HasNextInt(DefaultRadix);
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as an int value in the specified radix using the
		/// <seealso cref="#nextInt"/> method. The scanner does not advance past any input.
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token as an int value </param>
		/// <returns> true if and only if this scanner's next token is a valid
		///         int value </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextInt(int radix)
		{
			Radix = radix;
			bool result = HasNext(IntegerPattern());
			if (result) // Cache it
			{
				try
				{
					String s = (Matcher.Group(SIMPLE_GROUP_INDEX) == null) ? ProcessIntegerToken(HasNextResult) : HasNextResult;
					TypeCache = Convert.ToInt32(s, radix);
				}
				catch (NumberFormatException)
				{
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// The integer token must be stripped of prefixes, group separators,
		/// and suffixes, non ascii digits must be converted into ascii digits
		/// before parse will accept it.
		/// </summary>
		private String ProcessIntegerToken(String token)
		{
			String result = token.ReplaceAll("" + GroupSeparator, "");
			bool isNegative = false;
			int preLen = NegativePrefix.Length();
			if ((preLen > 0) && result.StartsWith(NegativePrefix))
			{
				isNegative = true;
				result = result.Substring(preLen);
			}
			int sufLen = NegativeSuffix.Length();
			if ((sufLen > 0) && result.EndsWith(NegativeSuffix))
			{
				isNegative = true;
				result = StringHelperClass.SubstringSpecial(result, result.Length() - sufLen, result.Length());
			}
			if (isNegative)
			{
				result = "-" + result;
			}
			return result;
		}

		/// <summary>
		/// Scans the next token of the input as an <tt>int</tt>.
		/// 
		/// <para> An invocation of this method of the form
		/// <tt>nextInt()</tt> behaves in exactly the same way as the
		/// invocation <tt>nextInt(radix)</tt>, where <code>radix</code>
		/// is the default radix of this scanner.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <tt>int</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public int NextInt()
		{
			return NextInt(DefaultRadix);
		}

		/// <summary>
		/// Scans the next token of the input as an <tt>int</tt>.
		/// This method will throw <code>InputMismatchException</code>
		/// if the next token cannot be translated into a valid int value as
		/// described below. If the translation is successful, the scanner advances
		/// past the input that matched.
		/// 
		/// <para> If the next token matches the <a
		/// href="#Integer-regex"><i>Integer</i></a> regular expression defined
		/// above then the token is converted into an <tt>int</tt> value as if by
		/// removing all locale specific prefixes, group separators, and locale
		/// specific suffixes, then mapping non-ASCII digits into ASCII
		/// digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		/// negative sign (-) if the locale specific negative prefixes and suffixes
		/// were present, and passing the resulting string to
		/// <seealso cref="Integer#parseInt(String, int) Integer.parseInt"/> with the
		/// specified radix.
		/// 
		/// </para>
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token as an int value </param>
		/// <returns> the <tt>int</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public int NextInt(int radix)
		{
			// Check cached result
			if ((TypeCache != null) && (TypeCache is Integer) && this.Radix_Renamed == radix)
			{
				int val = ((Integer)TypeCache).IntValue();
				UseTypeCache();
				return val;
			}
			Radix = radix;
			ClearCaches();
			// Search for next int
			try
			{
				String s = Next(IntegerPattern());
				if (Matcher.Group(SIMPLE_GROUP_INDEX) == null)
				{
					s = ProcessIntegerToken(s);
				}
				return Convert.ToInt32(s, radix);
			}
			catch (NumberFormatException nfe)
			{
				Position = Matcher.Start(); // don't skip bad token
				throw new InputMismatchException(nfe.Message);
			}
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a long value in the default radix using the
		/// <seealso cref="#nextLong"/> method. The scanner does not advance past any input.
		/// </summary>
		/// <returns> true if and only if this scanner's next token is a valid
		///         long value </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextLong()
		{
			return HasNextLong(DefaultRadix);
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a long value in the specified radix using the
		/// <seealso cref="#nextLong"/> method. The scanner does not advance past any input.
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token as a long value </param>
		/// <returns> true if and only if this scanner's next token is a valid
		///         long value </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextLong(int radix)
		{
			Radix = radix;
			bool result = HasNext(IntegerPattern());
			if (result) // Cache it
			{
				try
				{
					String s = (Matcher.Group(SIMPLE_GROUP_INDEX) == null) ? ProcessIntegerToken(HasNextResult) : HasNextResult;
					TypeCache = Convert.ToInt64(s, radix);
				}
				catch (NumberFormatException)
				{
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// Scans the next token of the input as a <tt>long</tt>.
		/// 
		/// <para> An invocation of this method of the form
		/// <tt>nextLong()</tt> behaves in exactly the same way as the
		/// invocation <tt>nextLong(radix)</tt>, where <code>radix</code>
		/// is the default radix of this scanner.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <tt>long</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public long NextLong()
		{
			return NextLong(DefaultRadix);
		}

		/// <summary>
		/// Scans the next token of the input as a <tt>long</tt>.
		/// This method will throw <code>InputMismatchException</code>
		/// if the next token cannot be translated into a valid long value as
		/// described below. If the translation is successful, the scanner advances
		/// past the input that matched.
		/// 
		/// <para> If the next token matches the <a
		/// href="#Integer-regex"><i>Integer</i></a> regular expression defined
		/// above then the token is converted into a <tt>long</tt> value as if by
		/// removing all locale specific prefixes, group separators, and locale
		/// specific suffixes, then mapping non-ASCII digits into ASCII
		/// digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		/// negative sign (-) if the locale specific negative prefixes and suffixes
		/// were present, and passing the resulting string to
		/// <seealso cref="Long#parseLong(String, int) Long.parseLong"/> with the
		/// specified radix.
		/// 
		/// </para>
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token as an int value </param>
		/// <returns> the <tt>long</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public long NextLong(int radix)
		{
			// Check cached result
			if ((TypeCache != null) && (TypeCache is Long) && this.Radix_Renamed == radix)
			{
				long val = ((Long)TypeCache).LongValue();
				UseTypeCache();
				return val;
			}
			Radix = radix;
			ClearCaches();
			try
			{
				String s = Next(IntegerPattern());
				if (Matcher.Group(SIMPLE_GROUP_INDEX) == null)
				{
					s = ProcessIntegerToken(s);
				}
				return Convert.ToInt64(s, radix);
			}
			catch (NumberFormatException nfe)
			{
				Position = Matcher.Start(); // don't skip bad token
				throw new InputMismatchException(nfe.Message);
			}
		}

		/// <summary>
		/// The float token must be stripped of prefixes, group separators,
		/// and suffixes, non ascii digits must be converted into ascii digits
		/// before parseFloat will accept it.
		/// 
		/// If there are non-ascii digits in the token these digits must
		/// be processed before the token is passed to parseFloat.
		/// </summary>
		private String ProcessFloatToken(String token)
		{
			String result = token.ReplaceAll(GroupSeparator, "");
			if (!DecimalSeparator.Equals("\\."))
			{
				result = result.ReplaceAll(DecimalSeparator, ".");
			}
			bool isNegative = false;
			int preLen = NegativePrefix.Length();
			if ((preLen > 0) && result.StartsWith(NegativePrefix))
			{
				isNegative = true;
				result = result.Substring(preLen);
			}
			int sufLen = NegativeSuffix.Length();
			if ((sufLen > 0) && result.EndsWith(NegativeSuffix))
			{
				isNegative = true;
				result = StringHelperClass.SubstringSpecial(result, result.Length() - sufLen, result.Length());
			}
			if (result.Equals(NanString))
			{
				result = "NaN";
			}
			if (result.Equals(InfinityString))
			{
				result = "Infinity";
			}
			if (isNegative)
			{
				result = "-" + result;
			}

			// Translate non-ASCII digits
			Matcher m = NON_ASCII_DIGIT.Matcher(result);
			if (m.Find())
			{
				StringBuilder inASCII = new StringBuilder();
				for (int i = 0; i < result.Length(); i++)
				{
					char nextChar = result.CharAt(i);
					if (char.IsDigit(nextChar))
					{
						int d = Character.Digit(nextChar, 10);
						if (d != -1)
						{
							inASCII.Append(d);
						}
						else
						{
							inASCII.Append(nextChar);
						}
					}
					else
					{
						inASCII.Append(nextChar);
					}
				}
				result = inASCII.ToString();
			}

			return result;
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a float value using the <seealso cref="#nextFloat"/>
		/// method. The scanner does not advance past any input.
		/// </summary>
		/// <returns> true if and only if this scanner's next token is a valid
		///         float value </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextFloat()
		{
			Radix = 10;
			bool result = HasNext(FloatPattern());
			if (result) // Cache it
			{
				try
				{
					String s = ProcessFloatToken(HasNextResult);
					TypeCache = Convert.ToSingle(Convert.ToSingle(s));
				}
				catch (NumberFormatException)
				{
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// Scans the next token of the input as a <tt>float</tt>.
		/// This method will throw <code>InputMismatchException</code>
		/// if the next token cannot be translated into a valid float value as
		/// described below. If the translation is successful, the scanner advances
		/// past the input that matched.
		/// 
		/// <para> If the next token matches the <a
		/// href="#Float-regex"><i>Float</i></a> regular expression defined above
		/// then the token is converted into a <tt>float</tt> value as if by
		/// removing all locale specific prefixes, group separators, and locale
		/// specific suffixes, then mapping non-ASCII digits into ASCII
		/// digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		/// negative sign (-) if the locale specific negative prefixes and suffixes
		/// were present, and passing the resulting string to
		/// <seealso cref="Float#parseFloat Float.parseFloat"/>. If the token matches
		/// the localized NaN or infinity strings, then either "Nan" or "Infinity"
		/// is passed to <seealso cref="Float#parseFloat(String) Float.parseFloat"/> as
		/// appropriate.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <tt>float</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Float</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public float NextFloat()
		{
			// Check cached result
			if ((TypeCache != null) && (TypeCache is Float))
			{
				float val = ((Float)TypeCache).FloatValue();
				UseTypeCache();
				return val;
			}
			Radix = 10;
			ClearCaches();
			try
			{
				return Convert.ToSingle(ProcessFloatToken(Next(FloatPattern())));
			}
			catch (NumberFormatException nfe)
			{
				Position = Matcher.Start(); // don't skip bad token
				throw new InputMismatchException(nfe.Message);
			}
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a double value using the <seealso cref="#nextDouble"/>
		/// method. The scanner does not advance past any input.
		/// </summary>
		/// <returns> true if and only if this scanner's next token is a valid
		///         double value </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextDouble()
		{
			Radix = 10;
			bool result = HasNext(FloatPattern());
			if (result) // Cache it
			{
				try
				{
					String s = ProcessFloatToken(HasNextResult);
					TypeCache = Convert.ToDouble(Convert.ToDouble(s));
				}
				catch (NumberFormatException)
				{
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// Scans the next token of the input as a <tt>double</tt>.
		/// This method will throw <code>InputMismatchException</code>
		/// if the next token cannot be translated into a valid double value.
		/// If the translation is successful, the scanner advances past the input
		/// that matched.
		/// 
		/// <para> If the next token matches the <a
		/// href="#Float-regex"><i>Float</i></a> regular expression defined above
		/// then the token is converted into a <tt>double</tt> value as if by
		/// removing all locale specific prefixes, group separators, and locale
		/// specific suffixes, then mapping non-ASCII digits into ASCII
		/// digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		/// negative sign (-) if the locale specific negative prefixes and suffixes
		/// were present, and passing the resulting string to
		/// <seealso cref="Double#parseDouble Double.parseDouble"/>. If the token matches
		/// the localized NaN or infinity strings, then either "Nan" or "Infinity"
		/// is passed to <seealso cref="Double#parseDouble(String) Double.parseDouble"/> as
		/// appropriate.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <tt>double</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Float</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if the input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public double NextDouble()
		{
			// Check cached result
			if ((TypeCache != null) && (TypeCache is Double))
			{
				double val = ((Double)TypeCache).DoubleValue();
				UseTypeCache();
				return val;
			}
			Radix = 10;
			ClearCaches();
			// Search for next float
			try
			{
				return Convert.ToDouble(ProcessFloatToken(Next(FloatPattern())));
			}
			catch (NumberFormatException nfe)
			{
				Position = Matcher.Start(); // don't skip bad token
				throw new InputMismatchException(nfe.Message);
			}
		}

		// Convenience methods for scanning multi precision numbers

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a <code>BigInteger</code> in the default radix using the
		/// <seealso cref="#nextBigInteger"/> method. The scanner does not advance past any
		/// input.
		/// </summary>
		/// <returns> true if and only if this scanner's next token is a valid
		///         <code>BigInteger</code> </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextBigInteger()
		{
			return HasNextBigInteger(DefaultRadix);
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a <code>BigInteger</code> in the specified radix using
		/// the <seealso cref="#nextBigInteger"/> method. The scanner does not advance past
		/// any input.
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token as an integer </param>
		/// <returns> true if and only if this scanner's next token is a valid
		///         <code>BigInteger</code> </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextBigInteger(int radix)
		{
			Radix = radix;
			bool result = HasNext(IntegerPattern());
			if (result) // Cache it
			{
				try
				{
					String s = (Matcher.Group(SIMPLE_GROUP_INDEX) == null) ? ProcessIntegerToken(HasNextResult) : HasNextResult;
					TypeCache = new BigInteger(s, radix);
				}
				catch (NumberFormatException)
				{
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// Scans the next token of the input as a {@link java.math.BigInteger
		/// BigInteger}.
		/// 
		/// <para> An invocation of this method of the form
		/// <tt>nextBigInteger()</tt> behaves in exactly the same way as the
		/// invocation <tt>nextBigInteger(radix)</tt>, where <code>radix</code>
		/// is the default radix of this scanner.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <tt>BigInteger</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if the input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public BigInteger NextBigInteger()
		{
			return NextBigInteger(DefaultRadix);
		}

		/// <summary>
		/// Scans the next token of the input as a {@link java.math.BigInteger
		/// BigInteger}.
		/// 
		/// <para> If the next token matches the <a
		/// href="#Integer-regex"><i>Integer</i></a> regular expression defined
		/// above then the token is converted into a <tt>BigInteger</tt> value as if
		/// by removing all group separators, mapping non-ASCII digits into ASCII
		/// digits via the <seealso cref="Character#digit Character.digit"/>, and passing the
		/// resulting string to the {@link
		/// java.math.BigInteger#BigInteger(java.lang.String)
		/// BigInteger(String, int)} constructor with the specified radix.
		/// 
		/// </para>
		/// </summary>
		/// <param name="radix"> the radix used to interpret the token </param>
		/// <returns> the <tt>BigInteger</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Integer</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if the input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public BigInteger NextBigInteger(int radix)
		{
			// Check cached result
			if ((TypeCache != null) && (TypeCache is BigInteger) && this.Radix_Renamed == radix)
			{
				BigInteger val = (BigInteger)TypeCache;
				UseTypeCache();
				return val;
			}
			Radix = radix;
			ClearCaches();
			// Search for next int
			try
			{
				String s = Next(IntegerPattern());
				if (Matcher.Group(SIMPLE_GROUP_INDEX) == null)
				{
					s = ProcessIntegerToken(s);
				}
				return new BigInteger(s, radix);
			}
			catch (NumberFormatException nfe)
			{
				Position = Matcher.Start(); // don't skip bad token
				throw new InputMismatchException(nfe.Message);
			}
		}

		/// <summary>
		/// Returns true if the next token in this scanner's input can be
		/// interpreted as a <code>BigDecimal</code> using the
		/// <seealso cref="#nextBigDecimal"/> method. The scanner does not advance past any
		/// input.
		/// </summary>
		/// <returns> true if and only if this scanner's next token is a valid
		///         <code>BigDecimal</code> </returns>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public bool HasNextBigDecimal()
		{
			Radix = 10;
			bool result = HasNext(DecimalPattern());
			if (result) // Cache it
			{
				try
				{
					String s = ProcessFloatToken(HasNextResult);
					TypeCache = new BigDecimal(s);
				}
				catch (NumberFormatException)
				{
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// Scans the next token of the input as a {@link java.math.BigDecimal
		/// BigDecimal}.
		/// 
		/// <para> If the next token matches the <a
		/// href="#Decimal-regex"><i>Decimal</i></a> regular expression defined
		/// above then the token is converted into a <tt>BigDecimal</tt> value as if
		/// by removing all group separators, mapping non-ASCII digits into ASCII
		/// digits via the <seealso cref="Character#digit Character.digit"/>, and passing the
		/// resulting string to the {@link
		/// java.math.BigDecimal#BigDecimal(java.lang.String) BigDecimal(String)}
		/// constructor.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the <tt>BigDecimal</tt> scanned from the input </returns>
		/// <exception cref="InputMismatchException">
		///         if the next token does not match the <i>Decimal</i>
		///         regular expression, or is out of range </exception>
		/// <exception cref="NoSuchElementException"> if the input is exhausted </exception>
		/// <exception cref="IllegalStateException"> if this scanner is closed </exception>
		public BigDecimal NextBigDecimal()
		{
			// Check cached result
			if ((TypeCache != null) && (TypeCache is BigDecimal))
			{
				BigDecimal val = (BigDecimal)TypeCache;
				UseTypeCache();
				return val;
			}
			Radix = 10;
			ClearCaches();
			// Search for next float
			try
			{
				String s = ProcessFloatToken(Next(DecimalPattern()));
				return new BigDecimal(s);
			}
			catch (NumberFormatException nfe)
			{
				Position = Matcher.Start(); // don't skip bad token
				throw new InputMismatchException(nfe.Message);
			}
		}

		/// <summary>
		/// Resets this scanner.
		/// 
		/// <para> Resetting a scanner discards all of its explicit state
		/// information which may have been changed by invocations of {@link
		/// #useDelimiter}, <seealso cref="#useLocale"/>, or <seealso cref="#useRadix"/>.
		/// 
		/// </para>
		/// <para> An invocation of this method of the form
		/// <tt>scanner.reset()</tt> behaves in exactly the same way as the
		/// invocation
		/// 
		/// <blockquote><pre>{@code
		///   scanner.useDelimiter("\\p{javaWhitespace}+")
		///          .useLocale(Locale.getDefault(Locale.Category.FORMAT))
		///          .useRadix(10);
		/// }</pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <returns> this scanner
		/// 
		/// @since 1.6 </returns>
		public Scanner Reset()
		{
			DelimPattern = WHITESPACE_PATTERN;
			UseLocale(Locale.GetDefault(Locale.Category.FORMAT));
			UseRadix(10);
			ClearCaches();
			return this;
		}
	}

}