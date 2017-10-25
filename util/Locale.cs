using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util
{


	using GetPropertyAction = sun.security.action.GetPropertyAction;
	using BaseLocale = sun.util.locale.BaseLocale;
	using InternalLocaleBuilder = sun.util.locale.InternalLocaleBuilder;
	using LanguageTag = sun.util.locale.LanguageTag;
	using LocaleExtensions = sun.util.locale.LocaleExtensions;
	using LocaleMatcher = sun.util.locale.LocaleMatcher;
	using LocaleObjectCache = sun.util.locale.LocaleObjectCache;
	using LocaleSyntaxException = sun.util.locale.LocaleSyntaxException;
	using LocaleUtils = sun.util.locale.LocaleUtils;
	using ParseStatus = sun.util.locale.ParseStatus;
	using LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter;
	using LocaleResources = sun.util.locale.provider.LocaleResources;
	using LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool;
	using ResourceBundleBasedAdapter = sun.util.locale.provider.ResourceBundleBasedAdapter;

	/// <summary>
	/// A <code>Locale</code> object represents a specific geographical, political,
	/// or cultural region. An operation that requires a <code>Locale</code> to perform
	/// its task is called <em>locale-sensitive</em> and uses the <code>Locale</code>
	/// to tailor information for the user. For example, displaying a number
	/// is a locale-sensitive operation&mdash; the number should be formatted
	/// according to the customs and conventions of the user's native country,
	/// region, or culture.
	/// 
	/// <para> The {@code Locale} class implements IETF BCP 47 which is composed of
	/// <a href="http://tools.ietf.org/html/rfc4647">RFC 4647 "Matching of Language
	/// Tags"</a> and <a href="http://tools.ietf.org/html/rfc5646">RFC 5646 "Tags
	/// for Identifying Languages"</a> with support for the LDML (UTS#35, "Unicode
	/// Locale Data Markup Language") BCP 47-compatible extensions for locale data
	/// exchange.
	/// 
	/// </para>
	/// <para> A <code>Locale</code> object logically consists of the fields
	/// described below.
	/// 
	/// <dl>
	///   <dt><a name="def_language"><b>language</b></a></dt>
	/// 
	///   <dd>ISO 639 alpha-2 or alpha-3 language code, or registered
	///   language subtags up to 8 alpha letters (for future enhancements).
	///   When a language has both an alpha-2 code and an alpha-3 code, the
	///   alpha-2 code must be used.  You can find a full list of valid
	///   language codes in the IANA Language Subtag Registry (search for
	///   "Type: language").  The language field is case insensitive, but
	///   <code>Locale</code> always canonicalizes to lower case.</dd>
	/// 
	///   <dd>Well-formed language values have the form
	///   <code>[a-zA-Z]{2,8}</code>.  Note that this is not the the full
	///   BCP47 language production, since it excludes extlang.  They are
	///   not needed since modern three-letter language codes replace
	///   them.</dd>
	/// 
	///   <dd>Example: "en" (English), "ja" (Japanese), "kok" (Konkani)</dd>
	/// 
	///   <dt><a name="def_script"><b>script</b></a></dt>
	/// 
	///   <dd>ISO 15924 alpha-4 script code.  You can find a full list of
	///   valid script codes in the IANA Language Subtag Registry (search
	///   for "Type: script").  The script field is case insensitive, but
	///   <code>Locale</code> always canonicalizes to title case (the first
	///   letter is upper case and the rest of the letters are lower
	///   case).</dd>
	/// 
	///   <dd>Well-formed script values have the form
	///   <code>[a-zA-Z]{4}</code></dd>
	/// 
	///   <dd>Example: "Latn" (Latin), "Cyrl" (Cyrillic)</dd>
	/// 
	///   <dt><a name="def_region"><b>country (region)</b></a></dt>
	/// 
	///   <dd>ISO 3166 alpha-2 country code or UN M.49 numeric-3 area code.
	///   You can find a full list of valid country and region codes in the
	///   IANA Language Subtag Registry (search for "Type: region").  The
	///   country (region) field is case insensitive, but
	///   <code>Locale</code> always canonicalizes to upper case.</dd>
	/// 
	///   <dd>Well-formed country/region values have
	///   the form <code>[a-zA-Z]{2} | [0-9]{3}</code></dd>
	/// 
	///   <dd>Example: "US" (United States), "FR" (France), "029"
	///   (Caribbean)</dd>
	/// 
	///   <dt><a name="def_variant"><b>variant</b></a></dt>
	/// 
	///   <dd>Any arbitrary value used to indicate a variation of a
	///   <code>Locale</code>.  Where there are two or more variant values
	///   each indicating its own semantics, these values should be ordered
	///   by importance, with most important first, separated by
	///   underscore('_').  The variant field is case sensitive.</dd>
	/// 
	///   <dd>Note: IETF BCP 47 places syntactic restrictions on variant
	///   subtags.  Also BCP 47 subtags are strictly used to indicate
	///   additional variations that define a language or its dialects that
	///   are not covered by any combinations of language, script and
	///   region subtags.  You can find a full list of valid variant codes
	///   in the IANA Language Subtag Registry (search for "Type: variant").
	/// 
	/// </para>
	///   <para>However, the variant field in <code>Locale</code> has
	///   historically been used for any kind of variation, not just
	///   language variations.  For example, some supported variants
	///   available in Java SE Runtime Environments indicate alternative
	///   cultural behaviors such as calendar type or number script.  In
	///   BCP 47 this kind of information, which does not identify the
	///   language, is supported by extension subtags or private use
	///   subtags.</dd>
	/// 
	///   <dd>Well-formed variant values have the form <code>SUBTAG
	///   (('_'|'-') SUBTAG)*</code> where <code>SUBTAG =
	///   [0-9][0-9a-zA-Z]{3} | [0-9a-zA-Z]{5,8}</code>. (Note: BCP 47 only
	///   uses hyphen ('-') as a delimiter, this is more lenient).</dd>
	/// 
	///   <dd>Example: "polyton" (Polytonic Greek), "POSIX"</dd>
	/// 
	///   <dt><a name="def_extensions"><b>extensions</b></a></dt>
	/// 
	///   <dd>A map from single character keys to string values, indicating
	///   extensions apart from language identification.  The extensions in
	///   <code>Locale</code> implement the semantics and syntax of BCP 47
	///   extension subtags and private use subtags. The extensions are
	///   case insensitive, but <code>Locale</code> canonicalizes all
	///   extension keys and values to lower case. Note that extensions
	///   cannot have empty values.</dd>
	/// 
	///   <dd>Well-formed keys are single characters from the set
	///   <code>[0-9a-zA-Z]</code>.  Well-formed values have the form
	///   <code>SUBTAG ('-' SUBTAG)*</code> where for the key 'x'
	///   <code>SUBTAG = [0-9a-zA-Z]{1,8}</code> and for other keys
	///   <code>SUBTAG = [0-9a-zA-Z]{2,8}</code> (that is, 'x' allows
	///   single-character subtags).</dd>
	/// 
	///   <dd>Example: key="u"/value="ca-japanese" (Japanese Calendar),
	///   key="x"/value="java-1-7"</dd>
	/// </dl>
	/// 
	/// <b>Note:</b> Although BCP 47 requires field values to be registered
	/// in the IANA Language Subtag Registry, the <code>Locale</code> class
	/// does not provide any validation features.  The <code>Builder</code>
	/// only checks if an individual field satisfies the syntactic
	/// requirement (is well-formed), but does not validate the value
	/// itself.  See <seealso cref="Builder"/> for details.
	/// 
	/// <h3><a name="def_locale_extension">Unicode locale/language extension</a></h3>
	/// 
	/// </para>
	/// <para>UTS#35, "Unicode Locale Data Markup Language" defines optional
	/// attributes and keywords to override or refine the default behavior
	/// associated with a locale.  A keyword is represented by a pair of
	/// key and type.  For example, "nu-thai" indicates that Thai local
	/// digits (value:"thai") should be used for formatting numbers
	/// (key:"nu").
	/// 
	/// </para>
	/// <para>The keywords are mapped to a BCP 47 extension value using the
	/// extension key 'u' (<seealso cref="#UNICODE_LOCALE_EXTENSION"/>).  The above
	/// example, "nu-thai", becomes the extension "u-nu-thai".code
	/// 
	/// </para>
	/// <para>Thus, when a <code>Locale</code> object contains Unicode locale
	/// attributes and keywords,
	/// <code>getExtension(UNICODE_LOCALE_EXTENSION)</code> will return a
	/// String representing this information, for example, "nu-thai".  The
	/// <code>Locale</code> class also provides {@link
	/// #getUnicodeLocaleAttributes}, <seealso cref="#getUnicodeLocaleKeys"/>, and
	/// <seealso cref="#getUnicodeLocaleType"/> which allow you to access Unicode
	/// locale attributes and key/type pairs directly.  When represented as
	/// a string, the Unicode Locale Extension lists attributes
	/// alphabetically, followed by key/type sequences with keys listed
	/// alphabetically (the order of subtags comprising a key's type is
	/// fixed when the type is defined)
	/// 
	/// </para>
	/// <para>A well-formed locale key has the form
	/// <code>[0-9a-zA-Z]{2}</code>.  A well-formed locale type has the
	/// form <code>"" | [0-9a-zA-Z]{3,8} ('-' [0-9a-zA-Z]{3,8})*</code> (it
	/// can be empty, or a series of subtags 3-8 alphanums in length).  A
	/// well-formed locale attribute has the form
	/// <code>[0-9a-zA-Z]{3,8}</code> (it is a single subtag with the same
	/// form as a locale type subtag).
	/// 
	/// </para>
	/// <para>The Unicode locale extension specifies optional behavior in
	/// locale-sensitive services.  Although the LDML specification defines
	/// various keys and values, actual locale-sensitive service
	/// implementations in a Java Runtime Environment might not support any
	/// particular Unicode locale attributes or key/type pairs.
	/// 
	/// <h4>Creating a Locale</h4>
	/// 
	/// </para>
	/// <para>There are several different ways to create a <code>Locale</code>
	/// object.
	/// 
	/// <h5>Builder</h5>
	/// 
	/// </para>
	/// <para>Using <seealso cref="Builder"/> you can construct a <code>Locale</code> object
	/// that conforms to BCP 47 syntax.
	/// 
	/// <h5>Constructors</h5>
	/// 
	/// </para>
	/// <para>The <code>Locale</code> class provides three constructors:
	/// <blockquote>
	/// <pre>
	///     <seealso cref="#Locale(String language)"/>
	///     <seealso cref="#Locale(String language, String country)"/>
	///     <seealso cref="#Locale(String language, String country, String variant)"/>
	/// </pre>
	/// </blockquote>
	/// These constructors allow you to create a <code>Locale</code> object
	/// with language, country and variant, but you cannot specify
	/// script or extensions.
	/// 
	/// <h5>Factory Methods</h5>
	/// 
	/// </para>
	/// <para>The method <seealso cref="#forLanguageTag"/> creates a <code>Locale</code>
	/// object for a well-formed BCP 47 language tag.
	/// 
	/// <h5>Locale Constants</h5>
	/// 
	/// </para>
	/// <para>The <code>Locale</code> class provides a number of convenient constants
	/// that you can use to create <code>Locale</code> objects for commonly used
	/// locales. For example, the following creates a <code>Locale</code> object
	/// for the United States:
	/// <blockquote>
	/// <pre>
	///     Locale.US
	/// </pre>
	/// </blockquote>
	/// 
	/// <h4><a name="LocaleMatching">Locale Matching</a></h4>
	/// 
	/// </para>
	/// <para>If an application or a system is internationalized and provides localized
	/// resources for multiple locales, it sometimes needs to find one or more
	/// locales (or language tags) which meet each user's specific preferences. Note
	/// that a term "language tag" is used interchangeably with "locale" in this
	/// locale matching documentation.
	/// 
	/// </para>
	/// <para>In order to do matching a user's preferred locales to a set of language
	/// tags, <a href="http://tools.ietf.org/html/rfc4647">RFC 4647 Matching of
	/// Language Tags</a> defines two mechanisms: filtering and lookup.
	/// <em>Filtering</em> is used to get all matching locales, whereas
	/// <em>lookup</em> is to choose the best matching locale.
	/// Matching is done case-insensitively. These matching mechanisms are described
	/// in the following sections.
	/// 
	/// </para>
	/// <para>A user's preference is called a <em>Language Priority List</em> and is
	/// expressed as a list of language ranges. There are syntactically two types of
	/// language ranges: basic and extended. See
	/// <seealso cref="Locale.LanguageRange Locale.LanguageRange"/> for details.
	/// 
	/// <h5>Filtering</h5>
	/// 
	/// </para>
	/// <para>The filtering operation returns all matching language tags. It is defined
	/// in RFC 4647 as follows:
	/// "In filtering, each language range represents the least specific language
	/// tag (that is, the language tag with fewest number of subtags) that is an
	/// acceptable match. All of the language tags in the matching set of tags will
	/// have an equal or greater number of subtags than the language range. Every
	/// non-wildcard subtag in the language range will appear in every one of the
	/// matching language tags."
	/// 
	/// </para>
	/// <para>There are two types of filtering: filtering for basic language ranges
	/// (called "basic filtering") and filtering for extended language ranges
	/// (called "extended filtering"). They may return different results by what
	/// kind of language ranges are included in the given Language Priority List.
	/// <seealso cref="Locale.FilteringMode"/> is a parameter to specify how filtering should
	/// be done.
	/// 
	/// <h5>Lookup</h5>
	/// 
	/// </para>
	/// <para>The lookup operation returns the best matching language tags. It is
	/// defined in RFC 4647 as follows:
	/// "By contrast with filtering, each language range represents the most
	/// specific tag that is an acceptable match.  The first matching tag found,
	/// according to the user's priority, is considered the closest match and is the
	/// item returned."
	/// 
	/// </para>
	/// <para>For example, if a Language Priority List consists of two language ranges,
	/// {@code "zh-Hant-TW"} and {@code "en-US"}, in prioritized order, lookup
	/// method progressively searches the language tags below in order to find the
	/// best matching language tag.
	/// <blockquote>
	/// <pre>
	///    1. zh-Hant-TW
	///    2. zh-Hant
	///    3. zh
	///    4. en-US
	///    5. en
	/// </pre>
	/// </blockquote>
	/// If there is a language tag which matches completely to a language range
	/// above, the language tag is returned.
	/// 
	/// </para>
	/// <para>{@code "*"} is the special language range, and it is ignored in lookup.
	/// 
	/// </para>
	/// <para>If multiple language tags match as a result of the subtag {@code '*'}
	/// included in a language range, the first matching language tag returned by
	/// an <seealso cref="Iterator"/> over a <seealso cref="Collection"/> of language tags is treated as
	/// the best matching one.
	/// 
	/// <h4>Use of Locale</h4>
	/// 
	/// </para>
	/// <para>Once you've created a <code>Locale</code> you can query it for information
	/// about itself. Use <code>getCountry</code> to get the country (or region)
	/// code and <code>getLanguage</code> to get the language code.
	/// You can use <code>getDisplayCountry</code> to get the
	/// name of the country suitable for displaying to the user. Similarly,
	/// you can use <code>getDisplayLanguage</code> to get the name of
	/// the language suitable for displaying to the user. Interestingly,
	/// the <code>getDisplayXXX</code> methods are themselves locale-sensitive
	/// and have two versions: one that uses the default
	/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale and one
	/// that uses the locale specified as an argument.
	/// 
	/// </para>
	/// <para>The Java Platform provides a number of classes that perform locale-sensitive
	/// operations. For example, the <code>NumberFormat</code> class formats
	/// numbers, currency, and percentages in a locale-sensitive manner. Classes
	/// such as <code>NumberFormat</code> have several convenience methods
	/// for creating a default object of that type. For example, the
	/// <code>NumberFormat</code> class provides these three convenience methods
	/// for creating a default <code>NumberFormat</code> object:
	/// <blockquote>
	/// <pre>
	///     NumberFormat.getInstance()
	///     NumberFormat.getCurrencyInstance()
	///     NumberFormat.getPercentInstance()
	/// </pre>
	/// </blockquote>
	/// Each of these methods has two variants; one with an explicit locale
	/// and one without; the latter uses the default
	/// <seealso cref="Locale.Category#FORMAT FORMAT"/> locale:
	/// <blockquote>
	/// <pre>
	///     NumberFormat.getInstance(myLocale)
	///     NumberFormat.getCurrencyInstance(myLocale)
	///     NumberFormat.getPercentInstance(myLocale)
	/// </pre>
	/// </blockquote>
	/// A <code>Locale</code> is the mechanism for identifying the kind of object
	/// (<code>NumberFormat</code>) that you would like to get. The locale is
	/// <STRONG>just</STRONG> a mechanism for identifying objects,
	/// <STRONG>not</STRONG> a container for the objects themselves.
	/// 
	/// <h4>Compatibility</h4>
	/// 
	/// </para>
	/// <para>In order to maintain compatibility with existing usage, Locale's
	/// constructors retain their behavior prior to the Java Runtime
	/// Environment version 1.7.  The same is largely true for the
	/// <code>toString</code> method. Thus Locale objects can continue to
	/// be used as they were. In particular, clients who parse the output
	/// of toString into language, country, and variant fields can continue
	/// to do so (although this is strongly discouraged), although the
	/// variant field will have additional information in it if script or
	/// extensions are present.
	/// 
	/// </para>
	/// <para>In addition, BCP 47 imposes syntax restrictions that are not
	/// imposed by Locale's constructors. This means that conversions
	/// between some Locales and BCP 47 language tags cannot be made without
	/// losing information. Thus <code>toLanguageTag</code> cannot
	/// represent the state of locales whose language, country, or variant
	/// do not conform to BCP 47.
	/// 
	/// </para>
	/// <para>Because of these issues, it is recommended that clients migrate
	/// away from constructing non-conforming locales and use the
	/// <code>forLanguageTag</code> and <code>Locale.Builder</code> APIs instead.
	/// Clients desiring a string representation of the complete locale can
	/// then always rely on <code>toLanguageTag</code> for this purpose.
	/// 
	/// <h5><a name="special_cases_constructor">Special cases</a></h5>
	/// 
	/// </para>
	/// <para>For compatibility reasons, two
	/// non-conforming locales are treated as special cases.  These are
	/// <b><tt>ja_JP_JP</tt></b> and <b><tt>th_TH_TH</tt></b>. These are ill-formed
	/// in BCP 47 since the variants are too short. To ease migration to BCP 47,
	/// these are treated specially during construction.  These two cases (and only
	/// these) cause a constructor to generate an extension, all other values behave
	/// exactly as they did prior to Java 7.
	/// 
	/// </para>
	/// <para>Java has used <tt>ja_JP_JP</tt> to represent Japanese as used in
	/// Japan together with the Japanese Imperial calendar. This is now
	/// representable using a Unicode locale extension, by specifying the
	/// Unicode locale key <tt>ca</tt> (for "calendar") and type
	/// <tt>japanese</tt>. When the Locale constructor is called with the
	/// arguments "ja", "JP", "JP", the extension "u-ca-japanese" is
	/// automatically added.
	/// 
	/// </para>
	/// <para>Java has used <tt>th_TH_TH</tt> to represent Thai as used in
	/// Thailand together with Thai digits. This is also now representable using
	/// a Unicode locale extension, by specifying the Unicode locale key
	/// <tt>nu</tt> (for "number") and value <tt>thai</tt>. When the Locale
	/// constructor is called with the arguments "th", "TH", "TH", the
	/// extension "u-nu-thai" is automatically added.
	/// 
	/// <h5>Serialization</h5>
	/// 
	/// </para>
	/// <para>During serialization, writeObject writes all fields to the output
	/// stream, including extensions.
	/// 
	/// </para>
	/// <para>During deserialization, readResolve adds extensions as described
	/// in <a href="#special_cases_constructor">Special Cases</a>, only
	/// for the two cases th_TH_TH and ja_JP_JP.
	/// 
	/// <h5>Legacy language codes</h5>
	/// 
	/// </para>
	/// <para>Locale's constructor has always converted three language codes to
	/// their earlier, obsoleted forms: <tt>he</tt> maps to <tt>iw</tt>,
	/// <tt>yi</tt> maps to <tt>ji</tt>, and <tt>id</tt> maps to
	/// <tt>in</tt>.  This continues to be the case, in order to not break
	/// backwards compatibility.
	/// 
	/// </para>
	/// <para>The APIs added in 1.7 map between the old and new language codes,
	/// maintaining the old codes internal to Locale (so that
	/// <code>getLanguage</code> and <code>toString</code> reflect the old
	/// code), but using the new codes in the BCP 47 language tag APIs (so
	/// that <code>toLanguageTag</code> reflects the new one). This
	/// preserves the equivalence between Locales no matter which code or
	/// API is used to construct them. Java's default resource bundle
	/// lookup mechanism also implements this mapping, so that resources
	/// can be named using either convention, see <seealso cref="ResourceBundle.Control"/>.
	/// 
	/// <h5>Three-letter language/country(region) codes</h5>
	/// 
	/// </para>
	/// <para>The Locale constructors have always specified that the language
	/// and the country param be two characters in length, although in
	/// practice they have accepted any length.  The specification has now
	/// been relaxed to allow language codes of two to eight characters and
	/// country (region) codes of two to three characters, and in
	/// particular, three-letter language codes and three-digit region
	/// codes as specified in the IANA Language Subtag Registry.  For
	/// compatibility, the implementation still does not impose a length
	/// constraint.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Builder </seealso>
	/// <seealso cref= ResourceBundle </seealso>
	/// <seealso cref= java.text.Format </seealso>
	/// <seealso cref= java.text.NumberFormat </seealso>
	/// <seealso cref= java.text.Collator
	/// @author Mark Davis
	/// @since 1.1 </seealso>
	[Serializable]
	public sealed class Locale : Cloneable
	{

		private static readonly Cache LOCALECACHE = new Cache();

		/// <summary>
		/// Useful constant for language.
		/// </summary>
		public static readonly Locale ENGLISH = CreateConstant("en", "");

		/// <summary>
		/// Useful constant for language.
		/// </summary>
		public static readonly Locale FRENCH = CreateConstant("fr", "");

		/// <summary>
		/// Useful constant for language.
		/// </summary>
		public static readonly Locale GERMAN = CreateConstant("de", "");

		/// <summary>
		/// Useful constant for language.
		/// </summary>
		public static readonly Locale ITALIAN = CreateConstant("it", "");

		/// <summary>
		/// Useful constant for language.
		/// </summary>
		public static readonly Locale JAPANESE = CreateConstant("ja", "");

		/// <summary>
		/// Useful constant for language.
		/// </summary>
		public static readonly Locale KOREAN = CreateConstant("ko", "");

		/// <summary>
		/// Useful constant for language.
		/// </summary>
		public static readonly Locale CHINESE = CreateConstant("zh", "");

		/// <summary>
		/// Useful constant for language.
		/// </summary>
		public static readonly Locale SIMPLIFIED_CHINESE = CreateConstant("zh", "CN");

		/// <summary>
		/// Useful constant for language.
		/// </summary>
		public static readonly Locale TRADITIONAL_CHINESE = CreateConstant("zh", "TW");

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale FRANCE = CreateConstant("fr", "FR");

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale GERMANY = CreateConstant("de", "DE");

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale ITALY = CreateConstant("it", "IT");

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale JAPAN = CreateConstant("ja", "JP");

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale KOREA = CreateConstant("ko", "KR");

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale CHINA = SIMPLIFIED_CHINESE;

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale PRC = SIMPLIFIED_CHINESE;

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale TAIWAN = TRADITIONAL_CHINESE;

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale UK = CreateConstant("en", "GB");

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale US = CreateConstant("en", "US");

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale CANADA = CreateConstant("en", "CA");

		/// <summary>
		/// Useful constant for country.
		/// </summary>
		public static readonly Locale CANADA_FRENCH = CreateConstant("fr", "CA");

		/// <summary>
		/// Useful constant for the root locale.  The root locale is the locale whose
		/// language, country, and variant are empty ("") strings.  This is regarded
		/// as the base locale of all locales, and is used as the language/country
		/// neutral locale for the locale sensitive operations.
		/// 
		/// @since 1.6
		/// </summary>
		public static readonly Locale ROOT = CreateConstant("", "");

		/// <summary>
		/// The key for the private use extension ('x').
		/// </summary>
		/// <seealso cref= #getExtension(char) </seealso>
		/// <seealso cref= Builder#setExtension(char, String)
		/// @since 1.7 </seealso>
		public const char PRIVATE_USE_EXTENSION = 'x';

		/// <summary>
		/// The key for Unicode locale extension ('u').
		/// </summary>
		/// <seealso cref= #getExtension(char) </seealso>
		/// <seealso cref= Builder#setExtension(char, String)
		/// @since 1.7 </seealso>
		public const char UNICODE_LOCALE_EXTENSION = 'u';

		/// <summary>
		/// serialization ID
		/// </summary>
		internal const long SerialVersionUID = 9149081749638150636L;

		/// <summary>
		/// Display types for retrieving localized names from the name providers.
		/// </summary>
		private const int DISPLAY_LANGUAGE = 0;
		private const int DISPLAY_COUNTRY = 1;
		private const int DISPLAY_VARIANT = 2;
		private const int DISPLAY_SCRIPT = 3;

		/// <summary>
		/// Private constructor used by getInstance method
		/// </summary>
		private Locale(BaseLocale baseLocale, LocaleExtensions extensions)
		{
			this.BaseLocale_Renamed = baseLocale;
			this.LocaleExtensions_Renamed = extensions;
		}

		/// <summary>
		/// Construct a locale from language, country and variant.
		/// This constructor normalizes the language value to lowercase and
		/// the country value to uppercase.
		/// <para>
		/// <b>Note:</b>
		/// <ul>
		/// <li>ISO 639 is not a stable standard; some of the language codes it defines
		/// (specifically "iw", "ji", and "in") have changed.  This constructor accepts both the
		/// old codes ("iw", "ji", and "in") and the new codes ("he", "yi", and "id"), but all other
		/// API on Locale will return only the OLD codes.
		/// <li>For backward compatibility reasons, this constructor does not make
		/// any syntactic checks on the input.
		/// <li>The two cases ("ja", "JP", "JP") and ("th", "TH", "TH") are handled specially,
		/// see <a href="#special_cases_constructor">Special Cases</a> for more information.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="language"> An ISO 639 alpha-2 or alpha-3 language code, or a language subtag
		/// up to 8 characters in length.  See the <code>Locale</code> class description about
		/// valid language values. </param>
		/// <param name="country"> An ISO 3166 alpha-2 country code or a UN M.49 numeric-3 area code.
		/// See the <code>Locale</code> class description about valid country values. </param>
		/// <param name="variant"> Any arbitrary value used to indicate a variation of a <code>Locale</code>.
		/// See the <code>Locale</code> class description for the details. </param>
		/// <exception cref="NullPointerException"> thrown if any argument is null. </exception>
		public Locale(String language, String country, String variant)
		{
			if (language == null || country == null || variant == null)
			{
				throw new NullPointerException();
			}
			BaseLocale_Renamed = BaseLocale.getInstance(ConvertOldISOCodes(language), "", country, variant);
			LocaleExtensions_Renamed = GetCompatibilityExtensions(language, "", country, variant);
		}

		/// <summary>
		/// Construct a locale from language and country.
		/// This constructor normalizes the language value to lowercase and
		/// the country value to uppercase.
		/// <para>
		/// <b>Note:</b>
		/// <ul>
		/// <li>ISO 639 is not a stable standard; some of the language codes it defines
		/// (specifically "iw", "ji", and "in") have changed.  This constructor accepts both the
		/// old codes ("iw", "ji", and "in") and the new codes ("he", "yi", and "id"), but all other
		/// API on Locale will return only the OLD codes.
		/// <li>For backward compatibility reasons, this constructor does not make
		/// any syntactic checks on the input.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="language"> An ISO 639 alpha-2 or alpha-3 language code, or a language subtag
		/// up to 8 characters in length.  See the <code>Locale</code> class description about
		/// valid language values. </param>
		/// <param name="country"> An ISO 3166 alpha-2 country code or a UN M.49 numeric-3 area code.
		/// See the <code>Locale</code> class description about valid country values. </param>
		/// <exception cref="NullPointerException"> thrown if either argument is null. </exception>
		public Locale(String language, String country) : this(language, country, "")
		{
		}

		/// <summary>
		/// Construct a locale from a language code.
		/// This constructor normalizes the language value to lowercase.
		/// <para>
		/// <b>Note:</b>
		/// <ul>
		/// <li>ISO 639 is not a stable standard; some of the language codes it defines
		/// (specifically "iw", "ji", and "in") have changed.  This constructor accepts both the
		/// old codes ("iw", "ji", and "in") and the new codes ("he", "yi", and "id"), but all other
		/// API on Locale will return only the OLD codes.
		/// <li>For backward compatibility reasons, this constructor does not make
		/// any syntactic checks on the input.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="language"> An ISO 639 alpha-2 or alpha-3 language code, or a language subtag
		/// up to 8 characters in length.  See the <code>Locale</code> class description about
		/// valid language values. </param>
		/// <exception cref="NullPointerException"> thrown if argument is null.
		/// @since 1.4 </exception>
		public Locale(String language) : this(language, "", "")
		{
		}

		/// <summary>
		/// This method must be called only for creating the Locale.*
		/// constants due to making shortcuts.
		/// </summary>
		private static Locale CreateConstant(String lang, String country)
		{
			BaseLocale @base = BaseLocale.createInstance(lang, country);
			return GetInstance(@base, null);
		}

		/// <summary>
		/// Returns a <code>Locale</code> constructed from the given
		/// <code>language</code>, <code>country</code> and
		/// <code>variant</code>. If the same <code>Locale</code> instance
		/// is available in the cache, then that instance is
		/// returned. Otherwise, a new <code>Locale</code> instance is
		/// created and cached.
		/// </summary>
		/// <param name="language"> lowercase 2 to 8 language code. </param>
		/// <param name="country"> uppercase two-letter ISO-3166 code and numric-3 UN M.49 area code. </param>
		/// <param name="variant"> vendor and browser specific code. See class description. </param>
		/// <returns> the <code>Locale</code> instance requested </returns>
		/// <exception cref="NullPointerException"> if any argument is null. </exception>
		internal static Locale GetInstance(String language, String country, String variant)
		{
			return GetInstance(language, "", country, variant, null);
		}

		internal static Locale GetInstance(String language, String script, String country, String variant, LocaleExtensions extensions)
		{
			if (language == null || script == null || country == null || variant == null)
			{
				throw new NullPointerException();
			}

			if (extensions == null)
			{
				extensions = GetCompatibilityExtensions(language, script, country, variant);
			}

			BaseLocale baseloc = BaseLocale.getInstance(language, script, country, variant);
			return GetInstance(baseloc, extensions);
		}

		internal static Locale GetInstance(BaseLocale baseloc, LocaleExtensions extensions)
		{
			LocaleKey key = new LocaleKey(baseloc, extensions);
			return LOCALECACHE.get(key);
		}

		private class Cache : LocaleObjectCache<LocaleKey, Locale>
		{
			internal Cache()
			{
			}

			protected internal override Locale CreateObject(LocaleKey key)
			{
				return new Locale(key.@base, key.Exts);
			}
		}

		private sealed class LocaleKey
		{
			internal readonly BaseLocale @base;
			internal readonly LocaleExtensions Exts;
			internal readonly int Hash;

			internal LocaleKey(BaseLocale baseLocale, LocaleExtensions extensions)
			{
				@base = baseLocale;
				Exts = extensions;

				// Calculate the hash value here because it's always used.
				int h = @base.HashCode();
				if (Exts != null)
				{
					h ^= Exts.HashCode();
				}
				Hash = h;
			}

			public override bool Equals(Object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (!(obj is LocaleKey))
				{
					return false;
				}
				LocaleKey other = (LocaleKey)obj;
				if (Hash != other.Hash || !@base.Equals(other.@base))
				{
					return false;
				}
				if (Exts == null)
				{
					return other.Exts == null;
				}
				return Exts.Equals(other.Exts);
			}

			public override int HashCode()
			{
				return Hash;
			}
		}

		/// <summary>
		/// Gets the current value of the default locale for this instance
		/// of the Java Virtual Machine.
		/// <para>
		/// The Java Virtual Machine sets the default locale during startup
		/// based on the host environment. It is used by many locale-sensitive
		/// methods if no locale is explicitly specified.
		/// It can be changed using the
		/// <seealso cref="#setDefault(java.util.Locale) setDefault"/> method.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the default locale for this instance of the Java Virtual Machine </returns>
		public static Locale Default
		{
			get
			{
				// do not synchronize this method - see 4071298
				return DefaultLocale;
			}
			set
			{
				lock (typeof(Locale))
				{
					SetDefault(Category.DISPLAY, value);
					SetDefault(Category.FORMAT, value);
					DefaultLocale = value;
				}
			}
		}

		/// <summary>
		/// Gets the current value of the default locale for the specified Category
		/// for this instance of the Java Virtual Machine.
		/// <para>
		/// The Java Virtual Machine sets the default locale during startup based
		/// on the host environment. It is used by many locale-sensitive methods
		/// if no locale is explicitly specified. It can be changed using the
		/// setDefault(Locale.Category, Locale) method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="category"> - the specified category to get the default locale </param>
		/// <exception cref="NullPointerException"> - if category is null </exception>
		/// <returns> the default locale for the specified Category for this instance
		///     of the Java Virtual Machine </returns>
		/// <seealso cref= #setDefault(Locale.Category, Locale)
		/// @since 1.7 </seealso>
		public static Locale GetDefault(Locale.Category category)
		{
			// do not synchronize this method - see 4071298
			switch (category.InnerEnumValue())
			{
			case java.util.Locale.Category.InnerEnum.DISPLAY:
				if (DefaultDisplayLocale == null)
				{
					lock (typeof(Locale))
					{
						if (DefaultDisplayLocale == null)
						{
							DefaultDisplayLocale = InitDefault(category);
						}
					}
				}
				return DefaultDisplayLocale;
			case java.util.Locale.Category.InnerEnum.FORMAT:
				if (DefaultFormatLocale == null)
				{
					lock (typeof(Locale))
					{
						if (DefaultFormatLocale == null)
						{
							DefaultFormatLocale = InitDefault(category);
						}
					}
				}
				return DefaultFormatLocale;
			default:
				Debug.Assert(false, "Unknown Category");
			break;
			}
			return Default;
		}

		private static Locale InitDefault()
		{
			String language, region, script, country, variant;
			language = AccessController.doPrivileged(new GetPropertyAction("user.language", "en"));
			// for compatibility, check for old user.region property
			region = AccessController.doPrivileged(new GetPropertyAction("user.region"));
			if (region != null)
			{
				// region can be of form country, country_variant, or _variant
				int i = region.IndexOf('_');
				if (i >= 0)
				{
					country = region.Substring(0, i);
					variant = region.Substring(i + 1);
				}
				else
				{
					country = region;
					variant = "";
				}
				script = "";
			}
			else
			{
				script = AccessController.doPrivileged(new GetPropertyAction("user.script", ""));
				country = AccessController.doPrivileged(new GetPropertyAction("user.country", ""));
				variant = AccessController.doPrivileged(new GetPropertyAction("user.variant", ""));
			}

			return GetInstance(language, script, country, variant, null);
		}

		private static Locale InitDefault(Locale.Category category)
		{
			return GetInstance(AccessController.doPrivileged(new GetPropertyAction(category.languageKey, DefaultLocale.Language)), AccessController.doPrivileged(new GetPropertyAction(category.scriptKey, DefaultLocale.Script)), AccessController.doPrivileged(new GetPropertyAction(category.countryKey, DefaultLocale.Country)), AccessController.doPrivileged(new GetPropertyAction(category.variantKey, DefaultLocale.Variant)), null);
		}


		/// <summary>
		/// Sets the default locale for the specified Category for this instance
		/// of the Java Virtual Machine. This does not affect the host locale.
		/// <para>
		/// If there is a security manager, its checkPermission method is called
		/// with a PropertyPermission("user.language", "write") permission before
		/// the default locale is changed.
		/// </para>
		/// <para>
		/// The Java Virtual Machine sets the default locale during startup based
		/// on the host environment. It is used by many locale-sensitive methods
		/// if no locale is explicitly specified.
		/// </para>
		/// <para>
		/// Since changing the default locale may affect many different areas of
		/// functionality, this method should only be used if the caller is
		/// prepared to reinitialize locale-sensitive code running within the
		/// same Java Virtual Machine.
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="category"> - the specified category to set the default locale </param>
		/// <param name="newLocale"> - the new default locale </param>
		/// <exception cref="SecurityException"> - if a security manager exists and its
		///     checkPermission method doesn't allow the operation. </exception>
		/// <exception cref="NullPointerException"> - if category and/or newLocale is null </exception>
		/// <seealso cref= SecurityManager#checkPermission(java.security.Permission) </seealso>
		/// <seealso cref= PropertyPermission </seealso>
		/// <seealso cref= #getDefault(Locale.Category)
		/// @since 1.7 </seealso>
		public static void SetDefault(Locale.Category category, Locale newLocale)
		{
			lock (typeof(Locale))
			{
				if (category == null)
				{
					throw new NullPointerException("Category cannot be NULL");
				}
				if (newLocale == null)
				{
					throw new NullPointerException("Can't set default locale to NULL");
				}
        
				SecurityManager sm = System.SecurityManager;
				if (sm != null)
				{
					sm.CheckPermission(new PropertyPermission("user.language", "write"));
				}
				switch (category.InnerEnumValue())
				{
				case java.util.Locale.Category.InnerEnum.DISPLAY:
					DefaultDisplayLocale = newLocale;
					break;
				case java.util.Locale.Category.InnerEnum.FORMAT:
					DefaultFormatLocale = newLocale;
					break;
				default:
					Debug.Assert(false, "Unknown Category");
				break;
				}
			}
		}

		/// <summary>
		/// Returns an array of all installed locales.
		/// The returned array represents the union of locales supported
		/// by the Java runtime environment and by installed
		/// <seealso cref="java.util.spi.LocaleServiceProvider LocaleServiceProvider"/>
		/// implementations.  It must contain at least a <code>Locale</code>
		/// instance equal to <seealso cref="java.util.Locale#US Locale.US"/>.
		/// </summary>
		/// <returns> An array of installed locales. </returns>
		public static Locale[] AvailableLocales
		{
			get
			{
				return LocaleServiceProviderPool.AllAvailableLocales;
			}
		}

		/// <summary>
		/// Returns a list of all 2-letter country codes defined in ISO 3166.
		/// Can be used to create Locales.
		/// <para>
		/// <b>Note:</b> The <code>Locale</code> class also supports other codes for
		/// country (region), such as 3-letter numeric UN M.49 area codes.
		/// Therefore, the list returned by this method does not contain ALL valid
		/// codes that can be used to create Locales.
		/// 
		/// </para>
		/// </summary>
		/// <returns> An array of ISO 3166 two-letter country codes. </returns>
		public static String[] ISOCountries
		{
			get
			{
				if (IsoCountries == null)
				{
					IsoCountries = GetISO2Table(LocaleISOData.IsoCountryTable);
				}
				String[] result = new String[IsoCountries.Length];
				System.Array.Copy(IsoCountries, 0, result, 0, IsoCountries.Length);
				return result;
			}
		}

		/// <summary>
		/// Returns a list of all 2-letter language codes defined in ISO 639.
		/// Can be used to create Locales.
		/// <para>
		/// <b>Note:</b>
		/// <ul>
		/// <li>ISO 639 is not a stable standard&mdash; some languages' codes have changed.
		/// The list this function returns includes both the new and the old codes for the
		/// languages whose codes have changed.
		/// <li>The <code>Locale</code> class also supports language codes up to
		/// 8 characters in length.  Therefore, the list returned by this method does
		/// not contain ALL valid codes that can be used to create Locales.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <returns> Am array of ISO 639 two-letter language codes. </returns>
		public static String[] ISOLanguages
		{
			get
			{
				if (IsoLanguages == null)
				{
					IsoLanguages = GetISO2Table(LocaleISOData.IsoLanguageTable);
				}
				String[] result = new String[IsoLanguages.Length];
				System.Array.Copy(IsoLanguages, 0, result, 0, IsoLanguages.Length);
				return result;
			}
		}

		private static String[] GetISO2Table(String table)
		{
			int len = table.Length() / 5;
			String[] isoTable = new String[len];
			for (int i = 0, j = 0; i < len; i++, j += 5)
			{
				isoTable[i] = table.Substring(j, 2);
			}
			return isoTable;
		}

		/// <summary>
		/// Returns the language code of this Locale.
		/// 
		/// <para><b>Note:</b> ISO 639 is not a stable standard&mdash; some languages' codes have changed.
		/// Locale's constructor recognizes both the new and the old codes for the languages
		/// whose codes have changed, but this function always returns the old code.  If you
		/// want to check for a specific language whose code has changed, don't do
		/// <pre>
		/// if (locale.getLanguage().equals("he")) // BAD!
		///    ...
		/// </pre>
		/// Instead, do
		/// <pre>
		/// if (locale.getLanguage().equals(new Locale("he").getLanguage()))
		///    ...
		/// </pre>
		/// </para>
		/// </summary>
		/// <returns> The language code, or the empty string if none is defined. </returns>
		/// <seealso cref= #getDisplayLanguage </seealso>
		public String Language
		{
			get
			{
				return BaseLocale_Renamed.Language;
			}
		}

		/// <summary>
		/// Returns the script for this locale, which should
		/// either be the empty string or an ISO 15924 4-letter script
		/// code. The first letter is uppercase and the rest are
		/// lowercase, for example, 'Latn', 'Cyrl'.
		/// </summary>
		/// <returns> The script code, or the empty string if none is defined. </returns>
		/// <seealso cref= #getDisplayScript
		/// @since 1.7 </seealso>
		public String Script
		{
			get
			{
				return BaseLocale_Renamed.Script;
			}
		}

		/// <summary>
		/// Returns the country/region code for this locale, which should
		/// either be the empty string, an uppercase ISO 3166 2-letter code,
		/// or a UN M.49 3-digit code.
		/// </summary>
		/// <returns> The country/region code, or the empty string if none is defined. </returns>
		/// <seealso cref= #getDisplayCountry </seealso>
		public String Country
		{
			get
			{
				return BaseLocale_Renamed.Region;
			}
		}

		/// <summary>
		/// Returns the variant code for this locale.
		/// </summary>
		/// <returns> The variant code, or the empty string if none is defined. </returns>
		/// <seealso cref= #getDisplayVariant </seealso>
		public String Variant
		{
			get
			{
				return BaseLocale_Renamed.Variant;
			}
		}

		/// <summary>
		/// Returns {@code true} if this {@code Locale} has any <a href="#def_extensions">
		/// extensions</a>.
		/// </summary>
		/// <returns> {@code true} if this {@code Locale} has any extensions
		/// @since 1.8 </returns>
		public bool HasExtensions()
		{
			return LocaleExtensions_Renamed != null;
		}

		/// <summary>
		/// Returns a copy of this {@code Locale} with no <a href="#def_extensions">
		/// extensions</a>. If this {@code Locale} has no extensions, this {@code Locale}
		/// is returned.
		/// </summary>
		/// <returns> a copy of this {@code Locale} with no extensions, or {@code this}
		///         if {@code this} has no extensions
		/// @since 1.8 </returns>
		public Locale StripExtensions()
		{
			return HasExtensions() ? Locale.GetInstance(BaseLocale_Renamed, null) : this;
		}

		/// <summary>
		/// Returns the extension (or private use) value associated with
		/// the specified key, or null if there is no extension
		/// associated with the key. To be well-formed, the key must be one
		/// of <code>[0-9A-Za-z]</code>. Keys are case-insensitive, so
		/// for example 'z' and 'Z' represent the same extension.
		/// </summary>
		/// <param name="key"> the extension key </param>
		/// <returns> The extension, or null if this locale defines no
		/// extension for the specified key. </returns>
		/// <exception cref="IllegalArgumentException"> if key is not well-formed </exception>
		/// <seealso cref= #PRIVATE_USE_EXTENSION </seealso>
		/// <seealso cref= #UNICODE_LOCALE_EXTENSION
		/// @since 1.7 </seealso>
		public String GetExtension(char key)
		{
			if (!LocaleExtensions.isValidKey(key))
			{
				throw new IllegalArgumentException("Ill-formed extension key: " + key);
			}
			return HasExtensions() ? LocaleExtensions_Renamed.getExtensionValue(key) : null;
		}

		/// <summary>
		/// Returns the set of extension keys associated with this locale, or the
		/// empty set if it has no extensions. The returned set is unmodifiable.
		/// The keys will all be lower-case.
		/// </summary>
		/// <returns> The set of extension keys, or the empty set if this locale has
		/// no extensions.
		/// @since 1.7 </returns>
		public Set<Character> ExtensionKeys
		{
			get
			{
				if (!HasExtensions())
				{
					return java.util.Collections.EmptySet();
				}
				return LocaleExtensions_Renamed.Keys;
			}
		}

		/// <summary>
		/// Returns the set of unicode locale attributes associated with
		/// this locale, or the empty set if it has no attributes. The
		/// returned set is unmodifiable.
		/// </summary>
		/// <returns> The set of attributes.
		/// @since 1.7 </returns>
		public Set<String> UnicodeLocaleAttributes
		{
			get
			{
				if (!HasExtensions())
				{
					return java.util.Collections.EmptySet();
				}
				return LocaleExtensions_Renamed.UnicodeLocaleAttributes;
			}
		}

		/// <summary>
		/// Returns the Unicode locale type associated with the specified Unicode locale key
		/// for this locale. Returns the empty string for keys that are defined with no type.
		/// Returns null if the key is not defined. Keys are case-insensitive. The key must
		/// be two alphanumeric characters ([0-9a-zA-Z]), or an IllegalArgumentException is
		/// thrown.
		/// </summary>
		/// <param name="key"> the Unicode locale key </param>
		/// <returns> The Unicode locale type associated with the key, or null if the
		/// locale does not define the key. </returns>
		/// <exception cref="IllegalArgumentException"> if the key is not well-formed </exception>
		/// <exception cref="NullPointerException"> if <code>key</code> is null
		/// @since 1.7 </exception>
		public String GetUnicodeLocaleType(String key)
		{
			if (!IsUnicodeExtensionKey(key))
			{
				throw new IllegalArgumentException("Ill-formed Unicode locale key: " + key);
			}
			return HasExtensions() ? LocaleExtensions_Renamed.getUnicodeLocaleType(key) : null;
		}

		/// <summary>
		/// Returns the set of Unicode locale keys defined by this locale, or the empty set if
		/// this locale has none.  The returned set is immutable.  Keys are all lower case.
		/// </summary>
		/// <returns> The set of Unicode locale keys, or the empty set if this locale has
		/// no Unicode locale keywords.
		/// @since 1.7 </returns>
		public Set<String> UnicodeLocaleKeys
		{
			get
			{
				if (LocaleExtensions_Renamed == null)
				{
					return java.util.Collections.EmptySet();
				}
				return LocaleExtensions_Renamed.UnicodeLocaleKeys;
			}
		}

		/// <summary>
		/// Package locale method returning the Locale's BaseLocale,
		/// used by ResourceBundle </summary>
		/// <returns> base locale of this Locale </returns>
		internal BaseLocale BaseLocale
		{
			get
			{
				return BaseLocale_Renamed;
			}
		}

		/// <summary>
		/// Package private method returning the Locale's LocaleExtensions,
		/// used by ResourceBundle. </summary>
		/// <returns> locale exnteions of this Locale,
		///         or {@code null} if no extensions are defined </returns>
		 internal LocaleExtensions LocaleExtensions
		 {
			 get
			 {
				 return LocaleExtensions_Renamed;
			 }
		 }

		/// <summary>
		/// Returns a string representation of this <code>Locale</code>
		/// object, consisting of language, country, variant, script,
		/// and extensions as below:
		/// <blockquote>
		/// language + "_" + country + "_" + (variant + "_#" | "#") + script + "-" + extensions
		/// </blockquote>
		/// 
		/// Language is always lower case, country is always upper case, script is always title
		/// case, and extensions are always lower case.  Extensions and private use subtags
		/// will be in canonical order as explained in <seealso cref="#toLanguageTag"/>.
		/// 
		/// <para>When the locale has neither script nor extensions, the result is the same as in
		/// Java 6 and prior.
		/// 
		/// </para>
		/// <para>If both the language and country fields are missing, this function will return
		/// the empty string, even if the variant, script, or extensions field is present (you
		/// can't have a locale with just a variant, the variant must accompany a well-formed
		/// language or country code).
		/// 
		/// </para>
		/// <para>If script or extensions are present and variant is missing, no underscore is
		/// added before the "#".
		/// 
		/// </para>
		/// <para>This behavior is designed to support debugging and to be compatible with
		/// previous uses of <code>toString</code> that expected language, country, and variant
		/// fields only.  To represent a Locale as a String for interchange purposes, use
		/// <seealso cref="#toLanguageTag"/>.
		/// 
		/// </para>
		/// <para>Examples: <ul>
		/// <li><tt>en</tt></li>
		/// <li><tt>de_DE</tt></li>
		/// <li><tt>_GB</tt></li>
		/// <li><tt>en_US_WIN</tt></li>
		/// <li><tt>de__POSIX</tt></li>
		/// <li><tt>zh_CN_#Hans</tt></li>
		/// <li><tt>zh_TW_#Hant-x-java</tt></li>
		/// <li><tt>th_TH_TH_#u-nu-thai</tt></li></ul>
		/// 
		/// </para>
		/// </summary>
		/// <returns> A string representation of the Locale, for debugging. </returns>
		/// <seealso cref= #getDisplayName </seealso>
		/// <seealso cref= #toLanguageTag </seealso>
		public override String ToString()
		{
			bool l = (BaseLocale_Renamed.Language.length() != 0);
			bool s = (BaseLocale_Renamed.Script.length() != 0);
			bool r = (BaseLocale_Renamed.Region.length() != 0);
			bool v = (BaseLocale_Renamed.Variant.length() != 0);
			bool e = (LocaleExtensions_Renamed != null && LocaleExtensions_Renamed.ID.length() != 0);

			StringBuilder result = new StringBuilder(BaseLocale_Renamed.Language);
			if (r || (l && (v || s || e)))
			{
				result.Append('_').Append(BaseLocale_Renamed.Region); // This may just append '_'
			}
			if (v && (l || r))
			{
				result.Append('_').Append(BaseLocale_Renamed.Variant);
			}

			if (s && (l || r))
			{
				result.Append("_#").Append(BaseLocale_Renamed.Script);
			}

			if (e && (l || r))
			{
				result.Append('_');
				if (!s)
				{
					result.Append('#');
				}
				result.Append(LocaleExtensions_Renamed.ID);
			}

			return result.ToString();
		}

		/// <summary>
		/// Returns a well-formed IETF BCP 47 language tag representing
		/// this locale.
		/// 
		/// <para>If this <code>Locale</code> has a language, country, or
		/// variant that does not satisfy the IETF BCP 47 language tag
		/// syntax requirements, this method handles these fields as
		/// described below:
		/// 
		/// </para>
		/// <para><b>Language:</b> If language is empty, or not <a
		/// href="#def_language" >well-formed</a> (for example "a" or
		/// "e2"), it will be emitted as "und" (Undetermined).
		/// 
		/// </para>
		/// <para><b>Country:</b> If country is not <a
		/// href="#def_region">well-formed</a> (for example "12" or "USA"),
		/// it will be omitted.
		/// 
		/// </para>
		/// <para><b>Variant:</b> If variant <b>is</b> <a
		/// href="#def_variant">well-formed</a>, each sub-segment
		/// (delimited by '-' or '_') is emitted as a subtag.  Otherwise:
		/// <ul>
		/// 
		/// <li>if all sub-segments match <code>[0-9a-zA-Z]{1,8}</code>
		/// (for example "WIN" or "Oracle_JDK_Standard_Edition"), the first
		/// ill-formed sub-segment and all following will be appended to
		/// the private use subtag.  The first appended subtag will be
		/// "lvariant", followed by the sub-segments in order, separated by
		/// hyphen. For example, "x-lvariant-WIN",
		/// "Oracle-x-lvariant-JDK-Standard-Edition".
		/// 
		/// <li>if any sub-segment does not match
		/// <code>[0-9a-zA-Z]{1,8}</code>, the variant will be truncated
		/// and the problematic sub-segment and all following sub-segments
		/// will be omitted.  If the remainder is non-empty, it will be
		/// emitted as a private use subtag as above (even if the remainder
		/// turns out to be well-formed).  For example,
		/// "Solaris_isjustthecoolestthing" is emitted as
		/// "x-lvariant-Solaris", not as "solaris".</li></ul>
		/// 
		/// </para>
		/// <para><b>Special Conversions:</b> Java supports some old locale
		/// representations, including deprecated ISO language codes,
		/// for compatibility. This method performs the following
		/// conversions:
		/// <ul>
		/// 
		/// <li>Deprecated ISO language codes "iw", "ji", and "in" are
		/// converted to "he", "yi", and "id", respectively.
		/// 
		/// <li>A locale with language "no", country "NO", and variant
		/// "NY", representing Norwegian Nynorsk (Norway), is converted
		/// to a language tag "nn-NO".</li></ul>
		/// 
		/// </para>
		/// <para><b>Note:</b> Although the language tag created by this
		/// method is well-formed (satisfies the syntax requirements
		/// defined by the IETF BCP 47 specification), it is not
		/// necessarily a valid BCP 47 language tag.  For example,
		/// <pre>
		///   new Locale("xx", "YY").toLanguageTag();</pre>
		/// 
		/// will return "xx-YY", but the language subtag "xx" and the
		/// region subtag "YY" are invalid because they are not registered
		/// in the IANA Language Subtag Registry.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a BCP47 language tag representing the locale </returns>
		/// <seealso cref= #forLanguageTag(String)
		/// @since 1.7 </seealso>
		public String ToLanguageTag()
		{
			if (LanguageTag != null)
			{
				return LanguageTag;
			}

			LanguageTag tag = LanguageTag.parseLocale(BaseLocale_Renamed, LocaleExtensions_Renamed);
			StringBuilder buf = new StringBuilder();

			String subtag = tag.Language;
			if (subtag.Length() > 0)
			{
				buf.Append(LanguageTag.canonicalizeLanguage(subtag));
			}

			subtag = tag.Script;
			if (subtag.Length() > 0)
			{
				buf.Append(LanguageTag.SEP);
				buf.Append(LanguageTag.canonicalizeScript(subtag));
			}

			subtag = tag.Region;
			if (subtag.Length() > 0)
			{
				buf.Append(LanguageTag.SEP);
				buf.Append(LanguageTag.canonicalizeRegion(subtag));
			}

			<String> subtags = tag.Variants;
			foreach (String s in subtags)
			{
				buf.Append(LanguageTag.SEP);
				// preserve casing
				buf.Append(s);
			}

			subtags = tag.Extensions;
			foreach (String s in subtags)
			{
				buf.Append(LanguageTag.SEP);
				buf.Append(LanguageTag.canonicalizeExtension(s));
			}

			subtag = tag.Privateuse;
			if (subtag.Length() > 0)
			{
				if (buf.Length() > 0)
				{
					buf.Append(LanguageTag.SEP);
				}
				buf.Append(LanguageTag.PRIVATEUSE).Append(LanguageTag.SEP);
				// preserve casing
				buf.Append(subtag);
			}

			String langTag = buf.ToString();
			lock (this)
			{
				if (LanguageTag == null)
				{
					LanguageTag = langTag;
				}
			}
			return LanguageTag;
		}

		/// <summary>
		/// Returns a locale for the specified IETF BCP 47 language tag string.
		/// 
		/// <para>If the specified language tag contains any ill-formed subtags,
		/// the first such subtag and all following subtags are ignored.  Compare
		/// to <seealso cref="Locale.Builder#setLanguageTag"/> which throws an exception
		/// in this case.
		/// 
		/// </para>
		/// <para>The following <b>conversions</b> are performed:<ul>
		/// 
		/// <li>The language code "und" is mapped to language "".
		/// 
		/// <li>The language codes "he", "yi", and "id" are mapped to "iw",
		/// "ji", and "in" respectively. (This is the same canonicalization
		/// that's done in Locale's constructors.)
		/// 
		/// <li>The portion of a private use subtag prefixed by "lvariant",
		/// if any, is removed and appended to the variant field in the
		/// result locale (without case normalization).  If it is then
		/// empty, the private use subtag is discarded:
		/// 
		/// <pre>
		///     Locale loc;
		///     loc = Locale.forLanguageTag("en-US-x-lvariant-POSIX");
		///     loc.getVariant(); // returns "POSIX"
		///     loc.getExtension('x'); // returns null
		/// 
		///     loc = Locale.forLanguageTag("de-POSIX-x-URP-lvariant-Abc-Def");
		///     loc.getVariant(); // returns "POSIX_Abc_Def"
		///     loc.getExtension('x'); // returns "urp"
		/// </pre>
		/// 
		/// <li>When the languageTag argument contains an extlang subtag,
		/// the first such subtag is used as the language, and the primary
		/// language subtag and other extlang subtags are ignored:
		/// 
		/// <pre>
		///     Locale.forLanguageTag("ar-aao").getLanguage(); // returns "aao"
		///     Locale.forLanguageTag("en-abc-def-us").toString(); // returns "abc_US"
		/// </pre>
		/// 
		/// <li>Case is normalized except for variant tags, which are left
		/// unchanged.  Language is normalized to lower case, script to
		/// title case, country to upper case, and extensions to lower
		/// case.
		/// 
		/// <li>If, after processing, the locale would exactly match either
		/// ja_JP_JP or th_TH_TH with no extensions, the appropriate
		/// extensions are added as though the constructor had been called:
		/// 
		/// <pre>
		///    Locale.forLanguageTag("ja-JP-x-lvariant-JP").toLanguageTag();
		///    // returns "ja-JP-u-ca-japanese-x-lvariant-JP"
		///    Locale.forLanguageTag("th-TH-x-lvariant-TH").toLanguageTag();
		///    // returns "th-TH-u-nu-thai-x-lvariant-TH"
		/// </pre></ul>
		/// 
		/// </para>
		/// <para>This implements the 'Language-Tag' production of BCP47, and
		/// so supports grandfathered (regular and irregular) as well as
		/// private use language tags.  Stand alone private use tags are
		/// represented as empty language and extension 'x-whatever',
		/// and grandfathered tags are converted to their canonical replacements
		/// where they exist.
		/// 
		/// </para>
		/// <para>Grandfathered tags with canonical replacements are as follows:
		/// 
		/// <table summary="Grandfathered tags with canonical replacements">
		/// <tbody align="center">
		/// <tr><th>grandfathered tag</th><th>&nbsp;</th><th>modern replacement</th></tr>
		/// <tr><td>art-lojban</td><td>&nbsp;</td><td>jbo</td></tr>
		/// <tr><td>i-ami</td><td>&nbsp;</td><td>ami</td></tr>
		/// <tr><td>i-bnn</td><td>&nbsp;</td><td>bnn</td></tr>
		/// <tr><td>i-hak</td><td>&nbsp;</td><td>hak</td></tr>
		/// <tr><td>i-klingon</td><td>&nbsp;</td><td>tlh</td></tr>
		/// <tr><td>i-lux</td><td>&nbsp;</td><td>lb</td></tr>
		/// <tr><td>i-navajo</td><td>&nbsp;</td><td>nv</td></tr>
		/// <tr><td>i-pwn</td><td>&nbsp;</td><td>pwn</td></tr>
		/// <tr><td>i-tao</td><td>&nbsp;</td><td>tao</td></tr>
		/// <tr><td>i-tay</td><td>&nbsp;</td><td>tay</td></tr>
		/// <tr><td>i-tsu</td><td>&nbsp;</td><td>tsu</td></tr>
		/// <tr><td>no-bok</td><td>&nbsp;</td><td>nb</td></tr>
		/// <tr><td>no-nyn</td><td>&nbsp;</td><td>nn</td></tr>
		/// <tr><td>sgn-BE-FR</td><td>&nbsp;</td><td>sfb</td></tr>
		/// <tr><td>sgn-BE-NL</td><td>&nbsp;</td><td>vgt</td></tr>
		/// <tr><td>sgn-CH-DE</td><td>&nbsp;</td><td>sgg</td></tr>
		/// <tr><td>zh-guoyu</td><td>&nbsp;</td><td>cmn</td></tr>
		/// <tr><td>zh-hakka</td><td>&nbsp;</td><td>hak</td></tr>
		/// <tr><td>zh-min-nan</td><td>&nbsp;</td><td>nan</td></tr>
		/// <tr><td>zh-xiang</td><td>&nbsp;</td><td>hsn</td></tr>
		/// </tbody>
		/// </table>
		/// 
		/// </para>
		/// <para>Grandfathered tags with no modern replacement will be
		/// converted as follows:
		/// 
		/// <table summary="Grandfathered tags with no modern replacement">
		/// <tbody align="center">
		/// <tr><th>grandfathered tag</th><th>&nbsp;</th><th>converts to</th></tr>
		/// <tr><td>cel-gaulish</td><td>&nbsp;</td><td>xtg-x-cel-gaulish</td></tr>
		/// <tr><td>en-GB-oed</td><td>&nbsp;</td><td>en-GB-x-oed</td></tr>
		/// <tr><td>i-default</td><td>&nbsp;</td><td>en-x-i-default</td></tr>
		/// <tr><td>i-enochian</td><td>&nbsp;</td><td>und-x-i-enochian</td></tr>
		/// <tr><td>i-mingo</td><td>&nbsp;</td><td>see-x-i-mingo</td></tr>
		/// <tr><td>zh-min</td><td>&nbsp;</td><td>nan-x-zh-min</td></tr>
		/// </tbody>
		/// </table>
		/// 
		/// </para>
		/// <para>For a list of all grandfathered tags, see the
		/// IANA Language Subtag Registry (search for "Type: grandfathered").
		/// 
		/// </para>
		/// <para><b>Note</b>: there is no guarantee that <code>toLanguageTag</code>
		/// and <code>forLanguageTag</code> will round-trip.
		/// 
		/// </para>
		/// </summary>
		/// <param name="languageTag"> the language tag </param>
		/// <returns> The locale that best represents the language tag. </returns>
		/// <exception cref="NullPointerException"> if <code>languageTag</code> is <code>null</code> </exception>
		/// <seealso cref= #toLanguageTag() </seealso>
		/// <seealso cref= java.util.Locale.Builder#setLanguageTag(String)
		/// @since 1.7 </seealso>
		public static Locale ForLanguageTag(String languageTag)
		{
			LanguageTag tag = LanguageTag.parse(languageTag, null);
			InternalLocaleBuilder bldr = new InternalLocaleBuilder();
			bldr.LanguageTag = tag;
			BaseLocale @base = bldr.BaseLocale;
			LocaleExtensions exts = bldr.LocaleExtensions;
			if (exts == null && @base.Variant.length() > 0)
			{
				exts = GetCompatibilityExtensions(@base.Language, @base.Script, @base.Region, @base.Variant);
			}
			return GetInstance(@base, exts);
		}

		/// <summary>
		/// Returns a three-letter abbreviation of this locale's language.
		/// If the language matches an ISO 639-1 two-letter code, the
		/// corresponding ISO 639-2/T three-letter lowercase code is
		/// returned.  The ISO 639-2 language codes can be found on-line,
		/// see "Codes for the Representation of Names of Languages Part 2:
		/// Alpha-3 Code".  If the locale specifies a three-letter
		/// language, the language is returned as is.  If the locale does
		/// not specify a language the empty string is returned.
		/// </summary>
		/// <returns> A three-letter abbreviation of this locale's language. </returns>
		/// <exception cref="MissingResourceException"> Throws MissingResourceException if
		/// three-letter language abbreviation is not available for this locale. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getISO3Language() throws MissingResourceException
		public String ISO3Language
		{
			get
			{
				String lang = BaseLocale_Renamed.Language;
				if (lang.Length() == 3)
				{
					return lang;
				}
    
				String language3 = GetISO3Code(lang, LocaleISOData.IsoLanguageTable);
				if (language3 == null)
				{
					throw new MissingResourceException("Couldn't find 3-letter language code for " + lang, "FormatData_" + ToString(), "ShortLanguage");
				}
				return language3;
			}
		}

		/// <summary>
		/// Returns a three-letter abbreviation for this locale's country.
		/// If the country matches an ISO 3166-1 alpha-2 code, the
		/// corresponding ISO 3166-1 alpha-3 uppercase code is returned.
		/// If the locale doesn't specify a country, this will be the empty
		/// string.
		/// 
		/// <para>The ISO 3166-1 codes can be found on-line.
		/// 
		/// </para>
		/// </summary>
		/// <returns> A three-letter abbreviation of this locale's country. </returns>
		/// <exception cref="MissingResourceException"> Throws MissingResourceException if the
		/// three-letter country abbreviation is not available for this locale. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getISO3Country() throws MissingResourceException
		public String ISO3Country
		{
			get
			{
				String country3 = GetISO3Code(BaseLocale_Renamed.Region, LocaleISOData.IsoCountryTable);
				if (country3 == null)
				{
					throw new MissingResourceException("Couldn't find 3-letter country code for " + BaseLocale_Renamed.Region, "FormatData_" + ToString(), "ShortCountry");
				}
				return country3;
			}
		}

		private static String GetISO3Code(String iso2Code, String table)
		{
			int codeLength = iso2Code.Length();
			if (codeLength == 0)
			{
				return "";
			}

			int tableLength = table.Length();
			int index = tableLength;
			if (codeLength == 2)
			{
				char c1 = iso2Code.CharAt(0);
				char c2 = iso2Code.CharAt(1);
				for (index = 0; index < tableLength; index += 5)
				{
					if (table.CharAt(index) == c1 && table.CharAt(index + 1) == c2)
					{
						break;
					}
				}
			}
			return index < tableLength ? StringHelperClass.SubstringSpecial(table, index + 2, index + 5) : null;
		}

		/// <summary>
		/// Returns a name for the locale's language that is appropriate for display to the
		/// user.
		/// If possible, the name returned will be localized for the default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.
		/// For example, if the locale is fr_FR and the default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale
		/// is en_US, getDisplayLanguage() will return "French"; if the locale is en_US and
		/// the default <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale is fr_FR,
		/// getDisplayLanguage() will return "anglais".
		/// If the name returned cannot be localized for the default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale,
		/// (say, we don't have a Japanese name for Croatian),
		/// this function falls back on the English name, and uses the ISO code as a last-resort
		/// value.  If the locale doesn't specify a language, this function returns the empty string.
		/// </summary>
		/// <returns> The name of the display language. </returns>
		public String DisplayLanguage
		{
			get
			{
				return GetDisplayLanguage(GetDefault(Category.DISPLAY));
			}
		}

		/// <summary>
		/// Returns a name for the locale's language that is appropriate for display to the
		/// user.
		/// If possible, the name returned will be localized according to inLocale.
		/// For example, if the locale is fr_FR and inLocale
		/// is en_US, getDisplayLanguage() will return "French"; if the locale is en_US and
		/// inLocale is fr_FR, getDisplayLanguage() will return "anglais".
		/// If the name returned cannot be localized according to inLocale,
		/// (say, we don't have a Japanese name for Croatian),
		/// this function falls back on the English name, and finally
		/// on the ISO code as a last-resort value.  If the locale doesn't specify a language,
		/// this function returns the empty string.
		/// </summary>
		/// <param name="inLocale"> The locale for which to retrieve the display language. </param>
		/// <returns> The name of the display language appropriate to the given locale. </returns>
		/// <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code> </exception>
		public String GetDisplayLanguage(Locale inLocale)
		{
			return GetDisplayString(BaseLocale_Renamed.Language, inLocale, DISPLAY_LANGUAGE);
		}

		/// <summary>
		/// Returns a name for the the locale's script that is appropriate for display to
		/// the user. If possible, the name will be localized for the default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.  Returns
		/// the empty string if this locale doesn't specify a script code.
		/// </summary>
		/// <returns> the display name of the script code for the current default
		///     <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale
		/// @since 1.7 </returns>
		public String DisplayScript
		{
			get
			{
				return GetDisplayScript(GetDefault(Category.DISPLAY));
			}
		}

		/// <summary>
		/// Returns a name for the locale's script that is appropriate
		/// for display to the user. If possible, the name will be
		/// localized for the given locale. Returns the empty string if
		/// this locale doesn't specify a script code.
		/// </summary>
		/// <param name="inLocale"> The locale for which to retrieve the display script. </param>
		/// <returns> the display name of the script code for the current default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale </returns>
		/// <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code>
		/// @since 1.7 </exception>
		public String GetDisplayScript(Locale inLocale)
		{
			return GetDisplayString(BaseLocale_Renamed.Script, inLocale, DISPLAY_SCRIPT);
		}

		/// <summary>
		/// Returns a name for the locale's country that is appropriate for display to the
		/// user.
		/// If possible, the name returned will be localized for the default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.
		/// For example, if the locale is fr_FR and the default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale
		/// is en_US, getDisplayCountry() will return "France"; if the locale is en_US and
		/// the default <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale is fr_FR,
		/// getDisplayCountry() will return "Etats-Unis".
		/// If the name returned cannot be localized for the default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale,
		/// (say, we don't have a Japanese name for Croatia),
		/// this function falls back on the English name, and uses the ISO code as a last-resort
		/// value.  If the locale doesn't specify a country, this function returns the empty string.
		/// </summary>
		/// <returns> The name of the country appropriate to the locale. </returns>
		public String DisplayCountry
		{
			get
			{
				return GetDisplayCountry(GetDefault(Category.DISPLAY));
			}
		}

		/// <summary>
		/// Returns a name for the locale's country that is appropriate for display to the
		/// user.
		/// If possible, the name returned will be localized according to inLocale.
		/// For example, if the locale is fr_FR and inLocale
		/// is en_US, getDisplayCountry() will return "France"; if the locale is en_US and
		/// inLocale is fr_FR, getDisplayCountry() will return "Etats-Unis".
		/// If the name returned cannot be localized according to inLocale.
		/// (say, we don't have a Japanese name for Croatia),
		/// this function falls back on the English name, and finally
		/// on the ISO code as a last-resort value.  If the locale doesn't specify a country,
		/// this function returns the empty string.
		/// </summary>
		/// <param name="inLocale"> The locale for which to retrieve the display country. </param>
		/// <returns> The name of the country appropriate to the given locale. </returns>
		/// <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code> </exception>
		public String GetDisplayCountry(Locale inLocale)
		{
			return GetDisplayString(BaseLocale_Renamed.Region, inLocale, DISPLAY_COUNTRY);
		}

		private String GetDisplayString(String code, Locale inLocale, int type)
		{
			if (code.Length() == 0)
			{
				return "";
			}

			if (inLocale == null)
			{
				throw new NullPointerException();
			}

			LocaleServiceProviderPool pool = LocaleServiceProviderPool.getPool(typeof(LocaleNameProvider));
			String key = (type == DISPLAY_VARIANT ? "%%" + code : code);
			String result = pool.getLocalizedObject(LocaleNameGetter.INSTANCE, inLocale, key, type, code);
				if (result != null)
				{
					return result;
				}

			return code;
		}

		/// <summary>
		/// Returns a name for the locale's variant code that is appropriate for display to the
		/// user.  If possible, the name will be localized for the default
		/// <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.  If the locale
		/// doesn't specify a variant code, this function returns the empty string.
		/// </summary>
		/// <returns> The name of the display variant code appropriate to the locale. </returns>
		public String DisplayVariant
		{
			get
			{
				return GetDisplayVariant(GetDefault(Category.DISPLAY));
			}
		}

		/// <summary>
		/// Returns a name for the locale's variant code that is appropriate for display to the
		/// user.  If possible, the name will be localized for inLocale.  If the locale
		/// doesn't specify a variant code, this function returns the empty string.
		/// </summary>
		/// <param name="inLocale"> The locale for which to retrieve the display variant code. </param>
		/// <returns> The name of the display variant code appropriate to the given locale. </returns>
		/// <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code> </exception>
		public String GetDisplayVariant(Locale inLocale)
		{
			if (BaseLocale_Renamed.Variant.length() == 0)
			{
				return "";
			}

			LocaleResources lr = LocaleProviderAdapter.forJRE().getLocaleResources(inLocale);

			String[] names = GetDisplayVariantArray(inLocale);

			// Get the localized patterns for formatting a list, and use
			// them to format the list.
			return FormatList(names, lr.getLocaleName("ListPattern"), lr.getLocaleName("ListCompositionPattern"));
		}

		/// <summary>
		/// Returns a name for the locale that is appropriate for display to the
		/// user. This will be the values returned by getDisplayLanguage(),
		/// getDisplayScript(), getDisplayCountry(), and getDisplayVariant() assembled
		/// into a single string. The the non-empty values are used in order,
		/// with the second and subsequent names in parentheses.  For example:
		/// <blockquote>
		/// language (script, country, variant)<br>
		/// language (country)<br>
		/// language (variant)<br>
		/// script (country)<br>
		/// country<br>
		/// </blockquote>
		/// depending on which fields are specified in the locale.  If the
		/// language, script, country, and variant fields are all empty,
		/// this function returns the empty string.
		/// </summary>
		/// <returns> The name of the locale appropriate to display. </returns>
		public String DisplayName
		{
			get
			{
				return GetDisplayName(GetDefault(Category.DISPLAY));
			}
		}

		/// <summary>
		/// Returns a name for the locale that is appropriate for display
		/// to the user.  This will be the values returned by
		/// getDisplayLanguage(), getDisplayScript(),getDisplayCountry(),
		/// and getDisplayVariant() assembled into a single string.
		/// The non-empty values are used in order,
		/// with the second and subsequent names in parentheses.  For example:
		/// <blockquote>
		/// language (script, country, variant)<br>
		/// language (country)<br>
		/// language (variant)<br>
		/// script (country)<br>
		/// country<br>
		/// </blockquote>
		/// depending on which fields are specified in the locale.  If the
		/// language, script, country, and variant fields are all empty,
		/// this function returns the empty string.
		/// </summary>
		/// <param name="inLocale"> The locale for which to retrieve the display name. </param>
		/// <returns> The name of the locale appropriate to display. </returns>
		/// <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code> </exception>
		public String GetDisplayName(Locale inLocale)
		{
			LocaleResources lr = LocaleProviderAdapter.forJRE().getLocaleResources(inLocale);

			String languageName = GetDisplayLanguage(inLocale);
			String scriptName = GetDisplayScript(inLocale);
			String countryName = GetDisplayCountry(inLocale);
			String[] variantNames = GetDisplayVariantArray(inLocale);

			// Get the localized patterns for formatting a display name.
			String displayNamePattern = lr.getLocaleName("DisplayNamePattern");
			String listPattern = lr.getLocaleName("ListPattern");
			String listCompositionPattern = lr.getLocaleName("ListCompositionPattern");

			// The display name consists of a main name, followed by qualifiers.
			// Typically, the format is "MainName (Qualifier, Qualifier)" but this
			// depends on what pattern is stored in the display locale.
			String mainName = null;
			String[] qualifierNames = null;

			// The main name is the language, or if there is no language, the script,
			// then if no script, the country. If there is no language/script/country
			// (an anomalous situation) then the display name is simply the variant's
			// display name.
			if (languageName.Length() == 0 && scriptName.Length() == 0 && countryName.Length() == 0)
			{
				if (variantNames.Length == 0)
				{
					return "";
				}
				else
				{
					return FormatList(variantNames, listPattern, listCompositionPattern);
				}
			}
			List<String> names = new List<String>(4);
			if (languageName.Length() != 0)
			{
				names.Add(languageName);
			}
			if (scriptName.Length() != 0)
			{
				names.Add(scriptName);
			}
			if (countryName.Length() != 0)
			{
				names.Add(countryName);
			}
			if (variantNames.Length != 0)
			{
				names.AddAll(variantNames);
			}

			// The first one in the main name
			mainName = names.Get(0);

			// Others are qualifiers
			int numNames = names.Size();
			qualifierNames = (numNames > 1) ? names.SubList(1, numNames).ToArray(new String[numNames - 1]) : new String[0];

			// Create an array whose first element is the number of remaining
			// elements.  This serves as a selector into a ChoiceFormat pattern from
			// the resource.  The second and third elements are the main name and
			// the qualifier; if there are no qualifiers, the third element is
			// unused by the format pattern.
			Object[] displayNames = new Object[] {new Integer(qualifierNames.Length != 0 ? 2 : 1), mainName, qualifierNames.Length != 0 ? FormatList(qualifierNames, listPattern, listCompositionPattern) : null};

			if (displayNamePattern != null)
			{
				return (new MessageFormat(displayNamePattern)).Format(displayNames);
			}
			else
			{
				// If we cannot get the message format pattern, then we use a simple
				// hard-coded pattern.  This should not occur in practice unless the
				// installation is missing some core files (FormatData etc.).
				StringBuilder result = new StringBuilder();
				result.Append((String)displayNames[1]);
				if (displayNames.Length > 2)
				{
					result.Append(" (");
					result.Append((String)displayNames[2]);
					result.Append(')');
				}
				return result.ToString();
			}
		}

		/// <summary>
		/// Overrides Cloneable.
		/// </summary>
		public override Object Clone()
		{
			try
			{
				Locale that = (Locale)base.Clone();
				return that;
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Override hashCode.
		/// Since Locales are often used in hashtables, caches the value
		/// for speed.
		/// </summary>
		public override int HashCode()
		{
			int hc = HashCodeValue;
			if (hc == 0)
			{
				hc = BaseLocale_Renamed.HashCode();
				if (LocaleExtensions_Renamed != null)
				{
					hc ^= LocaleExtensions_Renamed.HashCode();
				}
				HashCodeValue = hc;
			}
			return hc;
		}

		// Overrides

		/// <summary>
		/// Returns true if this Locale is equal to another object.  A Locale is
		/// deemed equal to another Locale with identical language, script, country,
		/// variant and extensions, and unequal to all other objects.
		/// </summary>
		/// <returns> true if this Locale is equal to the specified object. </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj) // quick check
			{
				return true;
			}
			if (!(obj is Locale))
			{
				return false;
			}
			BaseLocale otherBase = ((Locale)obj).BaseLocale_Renamed;
			if (!BaseLocale_Renamed.Equals(otherBase))
			{
				return false;
			}
			if (LocaleExtensions_Renamed == null)
			{
				return ((Locale)obj).LocaleExtensions_Renamed == null;
			}
			return LocaleExtensions_Renamed.Equals(((Locale)obj).LocaleExtensions_Renamed);
		}

		// ================= privates =====================================

		[NonSerialized]
		private BaseLocale BaseLocale_Renamed;
		[NonSerialized]
		private LocaleExtensions LocaleExtensions_Renamed;

		/// <summary>
		/// Calculated hashcode
		/// </summary>
		[NonSerialized]
		private volatile int HashCodeValue = 0;

		private volatile static Locale DefaultLocale = InitDefault();
		private volatile static Locale DefaultDisplayLocale = null;
		private volatile static Locale DefaultFormatLocale = null;

		[NonSerialized]
		private volatile String LanguageTag;

		/// <summary>
		/// Return an array of the display names of the variant. </summary>
		/// <param name="bundle"> the ResourceBundle to use to get the display names </param>
		/// <returns> an array of display names, possible of zero length. </returns>
		private String[] GetDisplayVariantArray(Locale inLocale)
		{
			// Split the variant name into tokens separated by '_'.
			StringTokenizer tokenizer = new StringTokenizer(BaseLocale_Renamed.Variant, "_");
			String[] names = new String[tokenizer.CountTokens()];

			// For each variant token, lookup the display name.  If
			// not found, use the variant name itself.
			for (int i = 0; i < names.Length; ++i)
			{
				names[i] = GetDisplayString(tokenizer.NextToken(), inLocale, DISPLAY_VARIANT);
			}

			return names;
		}

		/// <summary>
		/// Format a list using given pattern strings.
		/// If either of the patterns is null, then a the list is
		/// formatted by concatenation with the delimiter ','. </summary>
		/// <param name="stringList"> the list of strings to be formatted. </param>
		/// <param name="listPattern"> should create a MessageFormat taking 0-3 arguments
		/// and formatting them into a list. </param>
		/// <param name="listCompositionPattern"> should take 2 arguments
		/// and is used by composeList. </param>
		/// <returns> a string representing the list. </returns>
		private static String FormatList(String[] stringList, String listPattern, String listCompositionPattern)
		{
			// If we have no list patterns, compose the list in a simple,
			// non-localized way.
			if (listPattern == null || listCompositionPattern == null)
			{
				StringBuilder result = new StringBuilder();
				for (int i = 0; i < stringList.Length; ++i)
				{
					if (i > 0)
					{
						result.Append(',');
					}
					result.Append(stringList[i]);
				}
				return result.ToString();
			}

			// Compose the list down to three elements if necessary
			if (stringList.Length > 3)
			{
				MessageFormat format = new MessageFormat(listCompositionPattern);
				stringList = ComposeList(format, stringList);
			}

			// Rebuild the argument list with the list length as the first element
			Object[] args = new Object[stringList.Length + 1];
			System.Array.Copy(stringList, 0, args, 1, stringList.Length);
			args[0] = new Integer(stringList.Length);

			// Format it using the pattern in the resource
			MessageFormat format = new MessageFormat(listPattern);
			return format.Format(args);
		}

		/// <summary>
		/// Given a list of strings, return a list shortened to three elements.
		/// Shorten it by applying the given format to the first two elements
		/// recursively. </summary>
		/// <param name="format"> a format which takes two arguments </param>
		/// <param name="list"> a list of strings </param>
		/// <returns> if the list is three elements or shorter, the same list;
		/// otherwise, a new list of three elements. </returns>
		private static String[] ComposeList(MessageFormat format, String[] list)
		{
			if (list.Length <= 3)
			{
				return list;
			}

			// Use the given format to compose the first two elements into one
			String[] listItems = new String[] {list[0], list[1]};
			String newItem = format.Format(listItems);

			// Form a new list one element shorter
			String[] newList = new String[list.Length - 1];
			System.Array.Copy(list, 2, newList, 1, newList.Length - 1);
			newList[0] = newItem;

			// Recurse
			return ComposeList(format, newList);
		}

		// Duplicate of sun.util.locale.UnicodeLocaleExtension.isKey in order to
		// avoid its class loading.
		private static bool IsUnicodeExtensionKey(String s)
		{
			// 2alphanum
			return (s.Length() == 2) && LocaleUtils.isAlphaNumericString(s);
		}

		/// <summary>
		/// @serialField language    String
		///      language subtag in lower case. (See <a href="java/util/Locale.html#getLanguage()">getLanguage()</a>)
		/// @serialField country     String
		///      country subtag in upper case. (See <a href="java/util/Locale.html#getCountry()">getCountry()</a>)
		/// @serialField variant     String
		///      variant subtags separated by LOWLINE characters. (See <a href="java/util/Locale.html#getVariant()">getVariant()</a>)
		/// @serialField hashcode    int
		///      deprecated, for forward compatibility only
		/// @serialField script      String
		///      script subtag in title case (See <a href="java/util/Locale.html#getScript()">getScript()</a>)
		/// @serialField extensions  String
		///      canonical representation of extensions, that is,
		///      BCP47 extensions in alphabetical order followed by
		///      BCP47 private use subtags, all in lower case letters
		///      separated by HYPHEN-MINUS characters.
		///      (See <a href="java/util/Locale.html#getExtensionKeys()">getExtensionKeys()</a>,
		///      <a href="java/util/Locale.html#getExtension(char)">getExtension(char)</a>)
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("language", typeof(String)), new ObjectStreamField("country", typeof(String)), new ObjectStreamField("variant", typeof(String)), new ObjectStreamField("hashcode", typeof(int)), new ObjectStreamField("script", typeof(String)), new ObjectStreamField("extensions", typeof(String))};

		/// <summary>
		/// Serializes this <code>Locale</code> to the specified <code>ObjectOutputStream</code>. </summary>
		/// <param name="out"> the <code>ObjectOutputStream</code> to write </param>
		/// <exception cref="IOException">
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
		private void WriteObject(ObjectOutputStream @out)
		{
			ObjectOutputStream.PutField fields = @out.PutFields();
			fields.Put("language", BaseLocale_Renamed.Language);
			fields.Put("script", BaseLocale_Renamed.Script);
			fields.Put("country", BaseLocale_Renamed.Region);
			fields.Put("variant", BaseLocale_Renamed.Variant);
			fields.Put("extensions", LocaleExtensions_Renamed == null ? "" : LocaleExtensions_Renamed.ID);
			fields.Put("hashcode", -1); // place holder just for backward support
			@out.WriteFields();
		}

		/// <summary>
		/// Deserializes this <code>Locale</code>. </summary>
		/// <param name="in"> the <code>ObjectInputStream</code> to read </param>
		/// <exception cref="IOException"> </exception>
		/// <exception cref="ClassNotFoundException"> </exception>
		/// <exception cref="IllformedLocaleException">
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(ObjectInputStream @in)
		{
			ObjectInputStream.GetField fields = @in.ReadFields();
			String language = (String)fields.Get("language", "");
			String script = (String)fields.Get("script", "");
			String country = (String)fields.Get("country", "");
			String variant = (String)fields.Get("variant", "");
			String extStr = (String)fields.Get("extensions", "");
			BaseLocale_Renamed = BaseLocale.getInstance(ConvertOldISOCodes(language), script, country, variant);
			if (extStr.Length() > 0)
			{
				try
				{
					InternalLocaleBuilder bldr = new InternalLocaleBuilder();
					bldr.Extensions = extStr;
					LocaleExtensions_Renamed = bldr.LocaleExtensions;
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message);
				}
			}
			else
			{
				LocaleExtensions_Renamed = null;
			}
		}

		/// <summary>
		/// Returns a cached <code>Locale</code> instance equivalent to
		/// the deserialized <code>Locale</code>. When serialized
		/// language, country and variant fields read from the object data stream
		/// are exactly "ja", "JP", "JP" or "th", "TH", "TH" and script/extensions
		/// fields are empty, this method supplies <code>UNICODE_LOCALE_EXTENSION</code>
		/// "ca"/"japanese" (calendar type is "japanese") or "nu"/"thai" (number script
		/// type is "thai"). See <a href="Locale.html#special_cases_constructor">Special Cases</a>
		/// for more information.
		/// </summary>
		/// <returns> an instance of <code>Locale</code> equivalent to
		/// the deserialized <code>Locale</code>. </returns>
		/// <exception cref="java.io.ObjectStreamException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object readResolve() throws java.io.ObjectStreamException
		private Object ReadResolve()
		{
			return GetInstance(BaseLocale_Renamed.Language, BaseLocale_Renamed.Script, BaseLocale_Renamed.Region, BaseLocale_Renamed.Variant, LocaleExtensions_Renamed);
		}

		private static volatile String[] IsoLanguages = null;

		private static volatile String[] IsoCountries = null;

		private static String ConvertOldISOCodes(String language)
		{
			// we accept both the old and the new ISO codes for the languages whose ISO
			// codes have changed, but we always store the OLD code, for backward compatibility
			language = LocaleUtils.toLowerString(language).intern();
			if (language == "he")
			{
				return "iw";
			}
			else if (language == "yi")
			{
				return "ji";
			}
			else if (language == "id")
			{
				return "in";
			}
			else
			{
				return language;
			}
		}

		private static LocaleExtensions GetCompatibilityExtensions(String language, String script, String country, String variant)
		{
			LocaleExtensions extensions = null;
			// Special cases for backward compatibility support
			if (LocaleUtils.caseIgnoreMatch(language, "ja") && script.Length() == 0 && LocaleUtils.caseIgnoreMatch(country, "jp") && "JP".Equals(variant))
			{
				// ja_JP_JP -> u-ca-japanese (calendar = japanese)
				extensions = LocaleExtensions.CALENDAR_JAPANESE;
			}
			else if (LocaleUtils.caseIgnoreMatch(language, "th") && script.Length() == 0 && LocaleUtils.caseIgnoreMatch(country, "th") && "TH".Equals(variant))
			{
				// th_TH_TH -> u-nu-thai (numbersystem = thai)
				extensions = LocaleExtensions.NUMBER_THAI;
			}
			return extensions;
		}

		/// <summary>
		/// Obtains a localized locale names from a LocaleNameProvider
		/// implementation.
		/// </summary>
		private class LocaleNameGetter : LocaleServiceProviderPool.LocalizedObjectGetter<LocaleNameProvider, String>
		{
			internal static readonly LocaleNameGetter INSTANCE = new LocaleNameGetter();

			public override String GetObject(LocaleNameProvider localeNameProvider, Locale locale, String key, params Object[] @params)
			{
				Debug.Assert(@params.Length == 2);
				int type = (Integer)@params[0];
				String code = (String)@params[1];

				switch (type)
				{
				case DISPLAY_LANGUAGE:
					return localeNameProvider.GetDisplayLanguage(code, locale);
				case DISPLAY_COUNTRY:
					return localeNameProvider.GetDisplayCountry(code, locale);
				case DISPLAY_VARIANT:
					return localeNameProvider.GetDisplayVariant(code, locale);
				case DISPLAY_SCRIPT:
					return localeNameProvider.GetDisplayScript(code, locale);
				default:
					Debug.Assert(false); // shouldn't happen
				break;
				}

				return null;
			}
		}

		/// <summary>
		/// Enum for locale categories.  These locale categories are used to get/set
		/// the default locale for the specific functionality represented by the
		/// category.
		/// </summary>
		/// <seealso cref= #getDefault(Locale.Category) </seealso>
		/// <seealso cref= #setDefault(Locale.Category, Locale)
		/// @since 1.7 </seealso>
		public sealed class Category
		{

			/// <summary>
			/// Category used to represent the default locale for
			/// displaying user interfaces.
			/// </summary>
			DISPLAY("user.language.display",
			public static readonly Category DISPLAY("user.language.display" = new Category("DISPLAY("user.language.display"", InnerEnum.DISPLAY("user.language.display");
					"user.script.display",
					public static readonly Category "user.script.display" = new Category(""user.script.display"", InnerEnum."user.script.display");
					"user.country.display",
					public static readonly Category "user.country.display" = new Category(""user.country.display"", InnerEnum."user.country.display");
					"user.variant.display"),
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					"user.variant.display"),

			/// <summary>
			/// Category used to represent the default locale for
			/// formatting dates, numbers, and/or currencies.
			/// </summary>
			FORMAT("user.language.format",
			public static readonly Category FORMAT("user.language.format" = new Category("FORMAT("user.language.format"", InnerEnum.FORMAT("user.language.format");
				   "user.script.format",
				   public static readonly Category "user.script.format" = new Category(""user.script.format"", InnerEnum."user.script.format");
				   "user.country.format",
				   public static readonly Category "user.country.format" = new Category(""user.country.format"", InnerEnum."user.country.format");
				   internal "user.variant.format");

			internal Category(string name, InnerEnum innerEnum, Locale outerInstance, String languageKey, String scriptKey, String countryKey, String variantKey)
			{
				this.outerInstance = outerInstance;
				this.languageKey = languageKey;
				this.scriptKey = scriptKey;
				this.countryKey = countryKey;
				this.variantKey = variantKey;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			internal readonly String languageKey;
			internal readonly String scriptKey;
			internal readonly String countryKey;
			internal readonly String variantKey;

			public static IList<Category> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static Category valueOf(string name)
			{
				foreach (Category enumInstance in Category.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		/// <summary>
		/// <code>Builder</code> is used to build instances of <code>Locale</code>
		/// from values configured by the setters.  Unlike the <code>Locale</code>
		/// constructors, the <code>Builder</code> checks if a value configured by a
		/// setter satisfies the syntax requirements defined by the <code>Locale</code>
		/// class.  A <code>Locale</code> object created by a <code>Builder</code> is
		/// well-formed and can be transformed to a well-formed IETF BCP 47 language tag
		/// without losing information.
		/// 
		/// <para><b>Note:</b> The <code>Locale</code> class does not provide any
		/// syntactic restrictions on variant, while BCP 47 requires each variant
		/// subtag to be 5 to 8 alphanumerics or a single numeric followed by 3
		/// alphanumerics.  The method <code>setVariant</code> throws
		/// <code>IllformedLocaleException</code> for a variant that does not satisfy
		/// this restriction. If it is necessary to support such a variant, use a
		/// Locale constructor.  However, keep in mind that a <code>Locale</code>
		/// object created this way might lose the variant information when
		/// transformed to a BCP 47 language tag.
		/// 
		/// </para>
		/// <para>The following example shows how to create a <code>Locale</code> object
		/// with the <code>Builder</code>.
		/// <blockquote>
		/// <pre>
		///     Locale aLocale = new Builder().setLanguage("sr").setScript("Latn").setRegion("RS").build();
		/// </pre>
		/// </blockquote>
		/// 
		/// </para>
		/// <para>Builders can be reused; <code>clear()</code> resets all
		/// fields to their default values.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= Locale#forLanguageTag
		/// @since 1.7 </seealso>
		public sealed class Builder
		{
			internal readonly InternalLocaleBuilder LocaleBuilder;

			/// <summary>
			/// Constructs an empty Builder. The default value of all
			/// fields, extensions, and private use information is the
			/// empty string.
			/// </summary>
			public Builder()
			{
				LocaleBuilder = new InternalLocaleBuilder();
			}

			/// <summary>
			/// Resets the <code>Builder</code> to match the provided
			/// <code>locale</code>.  Existing state is discarded.
			/// 
			/// <para>All fields of the locale must be well-formed, see <seealso cref="Locale"/>.
			/// 
			/// </para>
			/// <para>Locales with any ill-formed fields cause
			/// <code>IllformedLocaleException</code> to be thrown, except for the
			/// following three cases which are accepted for compatibility
			/// reasons:<ul>
			/// <li>Locale("ja", "JP", "JP") is treated as "ja-JP-u-ca-japanese"
			/// <li>Locale("th", "TH", "TH") is treated as "th-TH-u-nu-thai"
			/// <li>Locale("no", "NO", "NY") is treated as "nn-NO"</ul>
			/// 
			/// </para>
			/// </summary>
			/// <param name="locale"> the locale </param>
			/// <returns> This builder. </returns>
			/// <exception cref="IllformedLocaleException"> if <code>locale</code> has
			/// any ill-formed fields. </exception>
			/// <exception cref="NullPointerException"> if <code>locale</code> is null. </exception>
			public Builder SetLocale(Locale locale)
			{
				try
				{
					LocaleBuilder.setLocale(locale.BaseLocale_Renamed, locale.LocaleExtensions_Renamed);
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message, e.ErrorIndex);
				}
				return this;
			}

			/// <summary>
			/// Resets the Builder to match the provided IETF BCP 47
			/// language tag.  Discards the existing state.  Null and the
			/// empty string cause the builder to be reset, like {@link
			/// #clear}.  Grandfathered tags (see {@link
			/// Locale#forLanguageTag}) are converted to their canonical
			/// form before being processed.  Otherwise, the language tag
			/// must be well-formed (see <seealso cref="Locale"/>) or an exception is
			/// thrown (unlike <code>Locale.forLanguageTag</code>, which
			/// just discards ill-formed and following portions of the
			/// tag).
			/// </summary>
			/// <param name="languageTag"> the language tag </param>
			/// <returns> This builder. </returns>
			/// <exception cref="IllformedLocaleException"> if <code>languageTag</code> is ill-formed </exception>
			/// <seealso cref= Locale#forLanguageTag(String) </seealso>
			public Builder SetLanguageTag(String languageTag)
			{
				ParseStatus sts = new ParseStatus();
				LanguageTag tag = LanguageTag.parse(languageTag, sts);
				if (sts.Error)
				{
					throw new IllformedLocaleException(sts.ErrorMessage, sts.ErrorIndex);
				}
				LocaleBuilder.LanguageTag = tag;
				return this;
			}

			/// <summary>
			/// Sets the language.  If <code>language</code> is the empty string or
			/// null, the language in this <code>Builder</code> is removed.  Otherwise,
			/// the language must be <a href="./Locale.html#def_language">well-formed</a>
			/// or an exception is thrown.
			/// 
			/// <para>The typical language value is a two or three-letter language
			/// code as defined in ISO639.
			/// 
			/// </para>
			/// </summary>
			/// <param name="language"> the language </param>
			/// <returns> This builder. </returns>
			/// <exception cref="IllformedLocaleException"> if <code>language</code> is ill-formed </exception>
			public Builder SetLanguage(String language)
			{
				try
				{
					LocaleBuilder.Language = language;
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message, e.ErrorIndex);
				}
				return this;
			}

			/// <summary>
			/// Sets the script. If <code>script</code> is null or the empty string,
			/// the script in this <code>Builder</code> is removed.
			/// Otherwise, the script must be <a href="./Locale.html#def_script">well-formed</a> or an
			/// exception is thrown.
			/// 
			/// <para>The typical script value is a four-letter script code as defined by ISO 15924.
			/// 
			/// </para>
			/// </summary>
			/// <param name="script"> the script </param>
			/// <returns> This builder. </returns>
			/// <exception cref="IllformedLocaleException"> if <code>script</code> is ill-formed </exception>
			public Builder SetScript(String script)
			{
				try
				{
					LocaleBuilder.Script = script;
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message, e.ErrorIndex);
				}
				return this;
			}

			/// <summary>
			/// Sets the region.  If region is null or the empty string, the region
			/// in this <code>Builder</code> is removed.  Otherwise,
			/// the region must be <a href="./Locale.html#def_region">well-formed</a> or an
			/// exception is thrown.
			/// 
			/// <para>The typical region value is a two-letter ISO 3166 code or a
			/// three-digit UN M.49 area code.
			/// 
			/// </para>
			/// <para>The country value in the <code>Locale</code> created by the
			/// <code>Builder</code> is always normalized to upper case.
			/// 
			/// </para>
			/// </summary>
			/// <param name="region"> the region </param>
			/// <returns> This builder. </returns>
			/// <exception cref="IllformedLocaleException"> if <code>region</code> is ill-formed </exception>
			public Builder SetRegion(String region)
			{
				try
				{
					LocaleBuilder.Region = region;
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message, e.ErrorIndex);
				}
				return this;
			}

			/// <summary>
			/// Sets the variant.  If variant is null or the empty string, the
			/// variant in this <code>Builder</code> is removed.  Otherwise, it
			/// must consist of one or more <a href="./Locale.html#def_variant">well-formed</a>
			/// subtags, or an exception is thrown.
			/// 
			/// <para><b>Note:</b> This method checks if <code>variant</code>
			/// satisfies the IETF BCP 47 variant subtag's syntax requirements,
			/// and normalizes the value to lowercase letters.  However,
			/// the <code>Locale</code> class does not impose any syntactic
			/// restriction on variant, and the variant value in
			/// <code>Locale</code> is case sensitive.  To set such a variant,
			/// use a Locale constructor.
			/// 
			/// </para>
			/// </summary>
			/// <param name="variant"> the variant </param>
			/// <returns> This builder. </returns>
			/// <exception cref="IllformedLocaleException"> if <code>variant</code> is ill-formed </exception>
			public Builder SetVariant(String variant)
			{
				try
				{
					LocaleBuilder.Variant = variant;
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message, e.ErrorIndex);
				}
				return this;
			}

			/// <summary>
			/// Sets the extension for the given key. If the value is null or the
			/// empty string, the extension is removed.  Otherwise, the extension
			/// must be <a href="./Locale.html#def_extensions">well-formed</a> or an exception
			/// is thrown.
			/// 
			/// <para><b>Note:</b> The key {@link Locale#UNICODE_LOCALE_EXTENSION
			/// UNICODE_LOCALE_EXTENSION} ('u') is used for the Unicode locale extension.
			/// Setting a value for this key replaces any existing Unicode locale key/type
			/// pairs with those defined in the extension.
			/// 
			/// </para>
			/// <para><b>Note:</b> The key {@link Locale#PRIVATE_USE_EXTENSION
			/// PRIVATE_USE_EXTENSION} ('x') is used for the private use code. To be
			/// well-formed, the value for this key needs only to have subtags of one to
			/// eight alphanumeric characters, not two to eight as in the general case.
			/// 
			/// </para>
			/// </summary>
			/// <param name="key"> the extension key </param>
			/// <param name="value"> the extension value </param>
			/// <returns> This builder. </returns>
			/// <exception cref="IllformedLocaleException"> if <code>key</code> is illegal
			/// or <code>value</code> is ill-formed </exception>
			/// <seealso cref= #setUnicodeLocaleKeyword(String, String) </seealso>
			public Builder SetExtension(char key, String value)
			{
				try
				{
					LocaleBuilder.setExtension(key, value);
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message, e.ErrorIndex);
				}
				return this;
			}

			/// <summary>
			/// Sets the Unicode locale keyword type for the given key.  If the type
			/// is null, the Unicode keyword is removed.  Otherwise, the key must be
			/// non-null and both key and type must be <a
			/// href="./Locale.html#def_locale_extension">well-formed</a> or an exception
			/// is thrown.
			/// 
			/// <para>Keys and types are converted to lower case.
			/// 
			/// </para>
			/// <para><b>Note</b>:Setting the 'u' extension via <seealso cref="#setExtension"/>
			/// replaces all Unicode locale keywords with those defined in the
			/// extension.
			/// 
			/// </para>
			/// </summary>
			/// <param name="key"> the Unicode locale key </param>
			/// <param name="type"> the Unicode locale type </param>
			/// <returns> This builder. </returns>
			/// <exception cref="IllformedLocaleException"> if <code>key</code> or <code>type</code>
			/// is ill-formed </exception>
			/// <exception cref="NullPointerException"> if <code>key</code> is null </exception>
			/// <seealso cref= #setExtension(char, String) </seealso>
			public Builder SetUnicodeLocaleKeyword(String key, String type)
			{
				try
				{
					LocaleBuilder.setUnicodeLocaleKeyword(key, type);
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message, e.ErrorIndex);
				}
				return this;
			}

			/// <summary>
			/// Adds a unicode locale attribute, if not already present, otherwise
			/// has no effect.  The attribute must not be null and must be <a
			/// href="./Locale.html#def_locale_extension">well-formed</a> or an exception
			/// is thrown.
			/// </summary>
			/// <param name="attribute"> the attribute </param>
			/// <returns> This builder. </returns>
			/// <exception cref="NullPointerException"> if <code>attribute</code> is null </exception>
			/// <exception cref="IllformedLocaleException"> if <code>attribute</code> is ill-formed </exception>
			/// <seealso cref= #setExtension(char, String) </seealso>
			public Builder AddUnicodeLocaleAttribute(String attribute)
			{
				try
				{
					LocaleBuilder.addUnicodeLocaleAttribute(attribute);
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message, e.ErrorIndex);
				}
				return this;
			}

			/// <summary>
			/// Removes a unicode locale attribute, if present, otherwise has no
			/// effect.  The attribute must not be null and must be <a
			/// href="./Locale.html#def_locale_extension">well-formed</a> or an exception
			/// is thrown.
			/// 
			/// <para>Attribute comparision for removal is case-insensitive.
			/// 
			/// </para>
			/// </summary>
			/// <param name="attribute"> the attribute </param>
			/// <returns> This builder. </returns>
			/// <exception cref="NullPointerException"> if <code>attribute</code> is null </exception>
			/// <exception cref="IllformedLocaleException"> if <code>attribute</code> is ill-formed </exception>
			/// <seealso cref= #setExtension(char, String) </seealso>
			public Builder RemoveUnicodeLocaleAttribute(String attribute)
			{
				try
				{
					LocaleBuilder.removeUnicodeLocaleAttribute(attribute);
				}
				catch (LocaleSyntaxException e)
				{
					throw new IllformedLocaleException(e.Message, e.ErrorIndex);
				}
				return this;
			}

			/// <summary>
			/// Resets the builder to its initial, empty state.
			/// </summary>
			/// <returns> This builder. </returns>
			public Builder Clear()
			{
				LocaleBuilder.clear();
				return this;
			}

			/// <summary>
			/// Resets the extensions to their initial, empty state.
			/// Language, script, region and variant are unchanged.
			/// </summary>
			/// <returns> This builder. </returns>
			/// <seealso cref= #setExtension(char, String) </seealso>
			public Builder ClearExtensions()
			{
				LocaleBuilder.clearExtensions();
				return this;
			}

			/// <summary>
			/// Returns an instance of <code>Locale</code> created from the fields set
			/// on this builder.
			/// 
			/// <para>This applies the conversions listed in <seealso cref="Locale#forLanguageTag"/>
			/// when constructing a Locale. (Grandfathered tags are handled in
			/// <seealso cref="#setLanguageTag"/>.)
			/// 
			/// </para>
			/// </summary>
			/// <returns> A Locale. </returns>
			public Locale Build()
			{
				BaseLocale baseloc = LocaleBuilder.BaseLocale;
				LocaleExtensions extensions = LocaleBuilder.LocaleExtensions;
				if (extensions == null && baseloc.Variant.length() > 0)
				{
					extensions = GetCompatibilityExtensions(baseloc.Language, baseloc.Script, baseloc.Region, baseloc.Variant);
				}
				return Locale.GetInstance(baseloc, extensions);
			}
		}

		/// <summary>
		/// This enum provides constants to select a filtering mode for locale
		/// matching. Refer to <a href="http://tools.ietf.org/html/rfc4647">RFC 4647
		/// Matching of Language Tags</a> for details.
		/// 
		/// <para>As an example, think of two Language Priority Lists each of which
		/// includes only one language range and a set of following language tags:
		/// 
		/// <pre>
		///    de (German)
		///    de-DE (German, Germany)
		///    de-Deva (German, in Devanagari script)
		///    de-Deva-DE (German, in Devanagari script, Germany)
		///    de-DE-1996 (German, Germany, orthography of 1996)
		///    de-Latn-DE (German, in Latin script, Germany)
		///    de-Latn-DE-1996 (German, in Latin script, Germany, orthography of 1996)
		/// </pre>
		/// 
		/// The filtering method will behave as follows:
		/// 
		/// <table cellpadding=2 summary="Filtering method behavior">
		/// <tr>
		/// <th>Filtering Mode</th>
		/// <th>Language Priority List: {@code "de-DE"}</th>
		/// <th>Language Priority List: {@code "de-*-DE"}</th>
		/// </tr>
		/// <tr>
		/// <td valign=top>
		/// <seealso cref="FilteringMode#AUTOSELECT_FILTERING AUTOSELECT_FILTERING"/>
		/// </td>
		/// <td valign=top>
		/// Performs <em>basic</em> filtering and returns {@code "de-DE"} and
		/// {@code "de-DE-1996"}.
		/// </td>
		/// <td valign=top>
		/// Performs <em>extended</em> filtering and returns {@code "de-DE"},
		/// {@code "de-Deva-DE"}, {@code "de-DE-1996"}, {@code "de-Latn-DE"}, and
		/// {@code "de-Latn-DE-1996"}.
		/// </td>
		/// </tr>
		/// <tr>
		/// <td valign=top>
		/// <seealso cref="FilteringMode#EXTENDED_FILTERING EXTENDED_FILTERING"/>
		/// </td>
		/// <td valign=top>
		/// Performs <em>extended</em> filtering and returns {@code "de-DE"},
		/// {@code "de-Deva-DE"}, {@code "de-DE-1996"}, {@code "de-Latn-DE"}, and
		/// {@code "de-Latn-DE-1996"}.
		/// </td>
		/// <td valign=top>Same as above.</td>
		/// </tr>
		/// <tr>
		/// <td valign=top>
		/// <seealso cref="FilteringMode#IGNORE_EXTENDED_RANGES IGNORE_EXTENDED_RANGES"/>
		/// </td>
		/// <td valign=top>
		/// Performs <em>basic</em> filtering and returns {@code "de-DE"} and
		/// {@code "de-DE-1996"}.
		/// </td>
		/// <td valign=top>
		/// Performs <em>basic</em> filtering and returns {@code null} because
		/// nothing matches.
		/// </td>
		/// </tr>
		/// <tr>
		/// <td valign=top>
		/// <seealso cref="FilteringMode#MAP_EXTENDED_RANGES MAP_EXTENDED_RANGES"/>
		/// </td>
		/// <td valign=top>Same as above.</td>
		/// <td valign=top>
		/// Performs <em>basic</em> filtering and returns {@code "de-DE"} and
		/// {@code "de-DE-1996"} because {@code "de-*-DE"} is mapped to
		/// {@code "de-DE"}.
		/// </td>
		/// </tr>
		/// <tr>
		/// <td valign=top>
		/// <seealso cref="FilteringMode#REJECT_EXTENDED_RANGES REJECT_EXTENDED_RANGES"/>
		/// </td>
		/// <td valign=top>Same as above.</td>
		/// <td valign=top>
		/// Throws <seealso cref="IllegalArgumentException"/> because {@code "de-*-DE"} is
		/// not a valid basic language range.
		/// </td>
		/// </tr>
		/// </table>
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #filter(List, Collection, FilteringMode) </seealso>
		/// <seealso cref= #filterTags(List, Collection, FilteringMode)
		/// 
		/// @since 1.8 </seealso>
		public enum FilteringMode
		{
			/// <summary>
			/// Specifies automatic filtering mode based on the given Language
			/// Priority List consisting of language ranges. If all of the ranges
			/// are basic, basic filtering is selected. Otherwise, extended
			/// filtering is selected.
			/// </summary>
			AUTOSELECT_FILTERING,

			/// <summary>
			/// Specifies extended filtering.
			/// </summary>
			EXTENDED_FILTERING,

			/// <summary>
			/// Specifies basic filtering: Note that any extended language ranges
			/// included in the given Language Priority List are ignored.
			/// </summary>
			IGNORE_EXTENDED_RANGES,

			/// <summary>
			/// Specifies basic filtering: If any extended language ranges are
			/// included in the given Language Priority List, they are mapped to the
			/// basic language range. Specifically, a language range starting with a
			/// subtag {@code "*"} is treated as a language range {@code "*"}. For
			/// example, {@code "*-US"} is treated as {@code "*"}. If {@code "*"} is
			/// not the first subtag, {@code "*"} and extra {@code "-"} are removed.
			/// For example, {@code "ja-*-JP"} is mapped to {@code "ja-JP"}.
			/// </summary>
			MAP_EXTENDED_RANGES,

			/// <summary>
			/// Specifies basic filtering: If any extended language ranges are
			/// included in the given Language Priority List, the list is rejected
			/// and the filtering method throws <seealso cref="IllegalArgumentException"/>.
			/// </summary>
			REJECT_EXTENDED_RANGES
		}

		/// <summary>
		/// This class expresses a <em>Language Range</em> defined in
		/// <a href="http://tools.ietf.org/html/rfc4647">RFC 4647 Matching of
		/// Language Tags</a>. A language range is an identifier which is used to
		/// select language tag(s) meeting specific requirements by using the
		/// mechanisms described in <a href="Locale.html#LocaleMatching">Locale
		/// Matching</a>. A list which represents a user's preferences and consists
		/// of language ranges is called a <em>Language Priority List</em>.
		/// 
		/// <para>There are two types of language ranges: basic and extended. In RFC
		/// 4647, the syntax of language ranges is expressed in
		/// <a href="http://tools.ietf.org/html/rfc4234">ABNF</a> as follows:
		/// <blockquote>
		/// <pre>
		///     basic-language-range    = (1*8ALPHA *("-" 1*8alphanum)) / "*"
		///     extended-language-range = (1*8ALPHA / "*")
		///                               *("-" (1*8alphanum / "*"))
		///     alphanum                = ALPHA / DIGIT
		/// </pre>
		/// </blockquote>
		/// For example, {@code "en"} (English), {@code "ja-JP"} (Japanese, Japan),
		/// {@code "*"} (special language range which matches any language tag) are
		/// basic language ranges, whereas {@code "*-CH"} (any languages,
		/// Switzerland), {@code "es-*"} (Spanish, any regions), and
		/// {@code "zh-Hant-*"} (Traditional Chinese, any regions) are extended
		/// language ranges.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref= #filter </seealso>
		/// <seealso cref= #filterTags </seealso>
		/// <seealso cref= #lookup </seealso>
		/// <seealso cref= #lookupTag
		/// 
		/// @since 1.8 </seealso>
		public sealed class LanguageRange
		{

		   /// <summary>
		   /// A constant holding the maximum value of weight, 1.0, which indicates
		   /// that the language range is a good fit for the user.
		   /// </summary>
			public const double MAX_WEIGHT = 1.0;

		   /// <summary>
		   /// A constant holding the minimum value of weight, 0.0, which indicates
		   /// that the language range is not a good fit for the user.
		   /// </summary>
			public const double MIN_WEIGHT = 0.0;

			internal readonly String Range_Renamed;
			internal readonly double Weight_Renamed;

			internal volatile int Hash = 0;

			/// <summary>
			/// Constructs a {@code LanguageRange} using the given {@code range}.
			/// Note that no validation is done against the IANA Language Subtag
			/// Registry at time of construction.
			/// 
			/// <para>This is equivalent to {@code LanguageRange(range, MAX_WEIGHT)}.
			/// 
			/// </para>
			/// </summary>
			/// <param name="range"> a language range </param>
			/// <exception cref="NullPointerException"> if the given {@code range} is
			///     {@code null} </exception>
			public LanguageRange(String range) : this(range, MAX_WEIGHT)
			{
			}

			/// <summary>
			/// Constructs a {@code LanguageRange} using the given {@code range} and
			/// {@code weight}. Note that no validation is done against the IANA
			/// Language Subtag Registry at time of construction.
			/// </summary>
			/// <param name="range">  a language range </param>
			/// <param name="weight"> a weight value between {@code MIN_WEIGHT} and
			///     {@code MAX_WEIGHT} </param>
			/// <exception cref="NullPointerException"> if the given {@code range} is
			///     {@code null} </exception>
			/// <exception cref="IllegalArgumentException"> if the given {@code weight} is less
			///     than {@code MIN_WEIGHT} or greater than {@code MAX_WEIGHT} </exception>
			public LanguageRange(String range, double weight)
			{
				if (range == null)
				{
					throw new NullPointerException();
				}
				if (weight < MIN_WEIGHT || weight > MAX_WEIGHT)
				{
					throw new IllegalArgumentException("weight=" + weight);
				}

				range = range.ToLowerCase();

				// Do syntax check.
				bool isIllFormed = false;
				String[] subtags = range.Split("-");
				if (IsSubtagIllFormed(subtags[0], true) || range.EndsWith("-"))
				{
					isIllFormed = true;
				}
				else
				{
					for (int i = 1; i < subtags.Length; i++)
					{
						if (IsSubtagIllFormed(subtags[i], false))
						{
							isIllFormed = true;
							break;
						}
					}
				}
				if (isIllFormed)
				{
					throw new IllegalArgumentException("range=" + range);
				}

				this.Range_Renamed = range;
				this.Weight_Renamed = weight;
			}

			internal static bool IsSubtagIllFormed(String subtag, bool isFirstSubtag)
			{
				if (subtag.Equals("") || subtag.Length() > 8)
				{
					return true;
				}
				else if (subtag.Equals("*"))
				{
					return false;
				}
				char[] charArray = subtag.ToCharArray();
				if (isFirstSubtag) // ALPHA
				{
					foreach (char c in charArray)
					{
						if (c < 'a' || c > 'z')
						{
							return true;
						}
					}
				} // ALPHA / DIGIT
				else
				{
					foreach (char c in charArray)
					{
						if (c < '0' || (c > '9' && c < 'a') || c > 'z')
						{
							return true;
						}
					}
				}
				return false;
			}

			/// <summary>
			/// Returns the language range of this {@code LanguageRange}.
			/// </summary>
			/// <returns> the language range. </returns>
			public String Range
			{
				get
				{
					return Range_Renamed;
				}
			}

			/// <summary>
			/// Returns the weight of this {@code LanguageRange}.
			/// </summary>
			/// <returns> the weight value. </returns>
			public double Weight
			{
				get
				{
					return Weight_Renamed;
				}
			}

			/// <summary>
			/// Parses the given {@code ranges} to generate a Language Priority List.
			/// 
			/// <para>This method performs a syntactic check for each language range in
			/// the given {@code ranges} but doesn't do validation using the IANA
			/// Language Subtag Registry.
			/// 
			/// </para>
			/// <para>The {@code ranges} to be given can take one of the following
			/// forms:
			/// 
			/// <pre>
			///   "Accept-Language: ja,en;q=0.4"  (weighted list with Accept-Language prefix)
			///   "ja,en;q=0.4"                   (weighted list)
			///   "ja,en"                         (prioritized list)
			/// </pre>
			/// 
			/// In a weighted list, each language range is given a weight value.
			/// The weight value is identical to the "quality value" in
			/// <a href="http://tools.ietf.org/html/rfc2616">RFC 2616</a>, and it
			/// expresses how much the user prefers  the language. A weight value is
			/// specified after a corresponding language range followed by
			/// {@code ";q="}, and the default weight value is {@code MAX_WEIGHT}
			/// when it is omitted.
			/// 
			/// </para>
			/// <para>Unlike a weighted list, language ranges in a prioritized list
			/// are sorted in the descending order based on its priority. The first
			/// language range has the highest priority and meets the user's
			/// preference most.
			/// 
			/// </para>
			/// <para>In either case, language ranges are sorted in descending order in
			/// the Language Priority List based on priority or weight. If a
			/// language range appears in the given {@code ranges} more than once,
			/// only the first one is included on the Language Priority List.
			/// 
			/// </para>
			/// <para>The returned list consists of language ranges from the given
			/// {@code ranges} and their equivalents found in the IANA Language
			/// Subtag Registry. For example, if the given {@code ranges} is
			/// {@code "Accept-Language: iw,en-us;q=0.7,en;q=0.3"}, the elements in
			/// the list to be returned are:
			/// 
			/// <pre>
			///  <b>Range</b>                                   <b>Weight</b>
			///    "iw" (older tag for Hebrew)             1.0
			///    "he" (new preferred code for Hebrew)    1.0
			///    "en-us" (English, United States)        0.7
			///    "en" (English)                          0.3
			/// </pre>
			/// 
			/// Two language ranges, {@code "iw"} and {@code "he"}, have the same
			/// highest priority in the list. By adding {@code "he"} to the user's
			/// Language Priority List, locale-matching method can find Hebrew as a
			/// matching locale (or language tag) even if the application or system
			/// offers only {@code "he"} as a supported locale (or language tag).
			/// 
			/// </para>
			/// </summary>
			/// <param name="ranges"> a list of comma-separated language ranges or a list of
			///     language ranges in the form of the "Accept-Language" header
			///     defined in <a href="http://tools.ietf.org/html/rfc2616">RFC
			///     2616</a> </param>
			/// <returns> a Language Priority List consisting of language ranges
			///     included in the given {@code ranges} and their equivalent
			///     language ranges if available. The list is modifiable. </returns>
			/// <exception cref="NullPointerException"> if {@code ranges} is null </exception>
			/// <exception cref="IllegalArgumentException"> if a language range or a weight
			///     found in the given {@code ranges} is ill-formed </exception>
			public static List<LanguageRange> Parse(String ranges)
			{
				return LocaleMatcher.parse(ranges);
			}

			/// <summary>
			/// Parses the given {@code ranges} to generate a Language Priority
			/// List, and then customizes the list using the given {@code map}.
			/// This method is equivalent to
			/// {@code mapEquivalents(parse(ranges), map)}.
			/// </summary>
			/// <param name="ranges"> a list of comma-separated language ranges or a list
			///     of language ranges in the form of the "Accept-Language" header
			///     defined in <a href="http://tools.ietf.org/html/rfc2616">RFC
			///     2616</a> </param>
			/// <param name="map"> a map containing information to customize language ranges </param>
			/// <returns> a Language Priority List with customization. The list is
			///     modifiable. </returns>
			/// <exception cref="NullPointerException"> if {@code ranges} is null </exception>
			/// <exception cref="IllegalArgumentException"> if a language range or a weight
			///     found in the given {@code ranges} is ill-formed </exception>
			/// <seealso cref= #parse(String) </seealso>
			/// <seealso cref= #mapEquivalents </seealso>
			public static List<LanguageRange> Parse(String ranges, Map<String, List<String>> map)
			{
				return MapEquivalents(Parse(ranges), map);
			}

			/// <summary>
			/// Generates a new customized Language Priority List using the given
			/// {@code priorityList} and {@code map}. If the given {@code map} is
			/// empty, this method returns a copy of the given {@code priorityList}.
			/// 
			/// <para>In the map, a key represents a language range whereas a value is
			/// a list of equivalents of it. {@code '*'} cannot be used in the map.
			/// Each equivalent language range has the same weight value as its
			/// original language range.
			/// 
			/// <pre>
			///  An example of map:
			///    <b>Key</b>                            <b>Value</b>
			///      "zh" (Chinese)                 "zh",
			///                                     "zh-Hans"(Simplified Chinese)
			///      "zh-HK" (Chinese, Hong Kong)   "zh-HK"
			///      "zh-TW" (Chinese, Taiwan)      "zh-TW"
			/// </pre>
			/// 
			/// The customization is performed after modification using the IANA
			/// Language Subtag Registry.
			/// 
			/// </para>
			/// <para>For example, if a user's Language Priority List consists of five
			/// language ranges ({@code "zh"}, {@code "zh-CN"}, {@code "en"},
			/// {@code "zh-TW"}, and {@code "zh-HK"}), the newly generated Language
			/// Priority List which is customized using the above map example will
			/// consists of {@code "zh"}, {@code "zh-Hans"}, {@code "zh-CN"},
			/// {@code "zh-Hans-CN"}, {@code "en"}, {@code "zh-TW"}, and
			/// {@code "zh-HK"}.
			/// 
			/// </para>
			/// <para>{@code "zh-HK"} and {@code "zh-TW"} aren't converted to
			/// {@code "zh-Hans-HK"} nor {@code "zh-Hans-TW"} even if they are
			/// included in the Language Priority List. In this example, mapping
			/// is used to clearly distinguish Simplified Chinese and Traditional
			/// Chinese.
			/// 
			/// </para>
			/// <para>If the {@code "zh"}-to-{@code "zh"} mapping isn't included in the
			/// map, a simple replacement will be performed and the customized list
			/// won't include {@code "zh"} and {@code "zh-CN"}.
			/// 
			/// </para>
			/// </summary>
			/// <param name="priorityList"> user's Language Priority List </param>
			/// <param name="map"> a map containing information to customize language ranges </param>
			/// <returns> a new Language Priority List with customization. The list is
			///     modifiable. </returns>
			/// <exception cref="NullPointerException"> if {@code priorityList} is {@code null} </exception>
			/// <seealso cref= #parse(String, Map) </seealso>
			public static List<LanguageRange> MapEquivalents(<LanguageRange> priorityList, Map<String, List<String>> map)
			{
				return LocaleMatcher.mapEquivalents(priorityList, map);
			}

			/// <summary>
			/// Returns a hash code value for the object.
			/// </summary>
			/// <returns>  a hash code value for this object. </returns>
			public override int HashCode()
			{
				if (Hash == 0)
				{
					int result = 17;
					result = 37 * result + Range_Renamed.HashCode();
					long bitsWeight = Double.DoubleToLongBits(Weight_Renamed);
					result = 37 * result + (int)(bitsWeight ^ ((long)((ulong)bitsWeight >> 32)));
					Hash = result;
				}
				return Hash;
			}

			/// <summary>
			/// Compares this object to the specified object. The result is true if
			/// and only if the argument is not {@code null} and is a
			/// {@code LanguageRange} object that contains the same {@code range}
			/// and {@code weight} values as this object.
			/// </summary>
			/// <param name="obj"> the object to compare with </param>
			/// <returns>  {@code true} if this object's {@code range} and
			///     {@code weight} are the same as the {@code obj}'s; {@code false}
			///     otherwise. </returns>
			public override bool Equals(Object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (!(obj is LanguageRange))
				{
					return false;
				}
				LanguageRange other = (LanguageRange)obj;
				return Hash == other.Hash && Range_Renamed.Equals(other.Range_Renamed) && Weight_Renamed == other.Weight_Renamed;
			}
		}

		/// <summary>
		/// Returns a list of matching {@code Locale} instances using the filtering
		/// mechanism defined in RFC 4647.
		/// </summary>
		/// <param name="priorityList"> user's Language Priority List in which each language
		///     tag is sorted in descending order based on priority or weight </param>
		/// <param name="locales"> {@code Locale} instances used for matching </param>
		/// <param name="mode"> filtering mode </param>
		/// <returns> a list of {@code Locale} instances for matching language tags
		///     sorted in descending order based on priority or weight, or an empty
		///     list if nothing matches. The list is modifiable. </returns>
		/// <exception cref="NullPointerException"> if {@code priorityList} or {@code locales}
		///     is {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if one or more extended language ranges
		///     are included in the given list when
		///     <seealso cref="FilteringMode#REJECT_EXTENDED_RANGES"/> is specified
		/// 
		/// @since 1.8 </exception>
		public static List<Locale> Filter(List<LanguageRange> priorityList, Collection<Locale> locales, FilteringMode mode)
		{
			return LocaleMatcher.filter(priorityList, locales, mode);
		}

		/// <summary>
		/// Returns a list of matching {@code Locale} instances using the filtering
		/// mechanism defined in RFC 4647. This is equivalent to
		/// <seealso cref="#filter(List, Collection, FilteringMode)"/> when {@code mode} is
		/// <seealso cref="FilteringMode#AUTOSELECT_FILTERING"/>.
		/// </summary>
		/// <param name="priorityList"> user's Language Priority List in which each language
		///     tag is sorted in descending order based on priority or weight </param>
		/// <param name="locales"> {@code Locale} instances used for matching </param>
		/// <returns> a list of {@code Locale} instances for matching language tags
		///     sorted in descending order based on priority or weight, or an empty
		///     list if nothing matches. The list is modifiable. </returns>
		/// <exception cref="NullPointerException"> if {@code priorityList} or {@code locales}
		///     is {@code null}
		/// 
		/// @since 1.8 </exception>
		public static List<Locale> Filter(List<LanguageRange> priorityList, Collection<Locale> locales)
		{
			return Filter(priorityList, locales, FilteringMode.AUTOSELECT_FILTERING);
		}

		/// <summary>
		/// Returns a list of matching languages tags using the basic filtering
		/// mechanism defined in RFC 4647.
		/// </summary>
		/// <param name="priorityList"> user's Language Priority List in which each language
		///     tag is sorted in descending order based on priority or weight </param>
		/// <param name="tags"> language tags </param>
		/// <param name="mode"> filtering mode </param>
		/// <returns> a list of matching language tags sorted in descending order
		///     based on priority or weight, or an empty list if nothing matches.
		///     The list is modifiable. </returns>
		/// <exception cref="NullPointerException"> if {@code priorityList} or {@code tags} is
		///     {@code null} </exception>
		/// <exception cref="IllegalArgumentException"> if one or more extended language ranges
		///     are included in the given list when
		///     <seealso cref="FilteringMode#REJECT_EXTENDED_RANGES"/> is specified
		/// 
		/// @since 1.8 </exception>
		public static List<String> FilterTags(List<LanguageRange> priorityList, Collection<String> tags, FilteringMode mode)
		{
			return LocaleMatcher.filterTags(priorityList, tags, mode);
		}

		/// <summary>
		/// Returns a list of matching languages tags using the basic filtering
		/// mechanism defined in RFC 4647. This is equivalent to
		/// <seealso cref="#filterTags(List, Collection, FilteringMode)"/> when {@code mode}
		/// is <seealso cref="FilteringMode#AUTOSELECT_FILTERING"/>.
		/// </summary>
		/// <param name="priorityList"> user's Language Priority List in which each language
		///     tag is sorted in descending order based on priority or weight </param>
		/// <param name="tags"> language tags </param>
		/// <returns> a list of matching language tags sorted in descending order
		///     based on priority or weight, or an empty list if nothing matches.
		///     The list is modifiable. </returns>
		/// <exception cref="NullPointerException"> if {@code priorityList} or {@code tags} is
		///     {@code null}
		/// 
		/// @since 1.8 </exception>
		public static List<String> FilterTags(List<LanguageRange> priorityList, Collection<String> tags)
		{
			return FilterTags(priorityList, tags, FilteringMode.AUTOSELECT_FILTERING);
		}

		/// <summary>
		/// Returns a {@code Locale} instance for the best-matching language
		/// tag using the lookup mechanism defined in RFC 4647.
		/// </summary>
		/// <param name="priorityList"> user's Language Priority List in which each language
		///     tag is sorted in descending order based on priority or weight </param>
		/// <param name="locales"> {@code Locale} instances used for matching </param>
		/// <returns> the best matching <code>Locale</code> instance chosen based on
		///     priority or weight, or {@code null} if nothing matches. </returns>
		/// <exception cref="NullPointerException"> if {@code priorityList} or {@code tags} is
		///     {@code null}
		/// 
		/// @since 1.8 </exception>
		public static Locale Lookup(List<LanguageRange> priorityList, Collection<Locale> locales)
		{
			return LocaleMatcher.lookup(priorityList, locales);
		}

		/// <summary>
		/// Returns the best-matching language tag using the lookup mechanism
		/// defined in RFC 4647.
		/// </summary>
		/// <param name="priorityList"> user's Language Priority List in which each language
		///     tag is sorted in descending order based on priority or weight </param>
		/// <param name="tags"> language tangs used for matching </param>
		/// <returns> the best matching language tag chosen based on priority or
		///     weight, or {@code null} if nothing matches. </returns>
		/// <exception cref="NullPointerException"> if {@code priorityList} or {@code tags} is
		///     {@code null}
		/// 
		/// @since 1.8 </exception>
		public static String LookupTag(List<LanguageRange> priorityList, Collection<String> tags)
		{
			return LocaleMatcher.lookupTag(priorityList, tags);
		}

	}

}