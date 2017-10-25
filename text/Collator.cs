using System.Collections.Generic;
using System.Collections.Concurrent;

/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
 * (C) Copyright Taligent, Inc. 1996-1998 -  All Rights Reserved
 * (C) Copyright IBM Corp. 1996-1998 - All Rights Reserved
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

	using LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter;
	using LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool;


	/// <summary>
	/// The <code>Collator</code> class performs locale-sensitive
	/// <code>String</code> comparison. You use this class to build
	/// searching and sorting routines for natural language text.
	/// 
	/// <para>
	/// <code>Collator</code> is an abstract base class. Subclasses
	/// implement specific collation strategies. One subclass,
	/// <code>RuleBasedCollator</code>, is currently provided with
	/// the Java Platform and is applicable to a wide set of languages. Other
	/// subclasses may be created to handle more specialized needs.
	/// 
	/// </para>
	/// <para>
	/// Like other locale-sensitive classes, you can use the static
	/// factory method, <code>getInstance</code>, to obtain the appropriate
	/// <code>Collator</code> object for a given locale. You will only need
	/// to look at the subclasses of <code>Collator</code> if you need
	/// to understand the details of a particular collation strategy or
	/// if you need to modify that strategy.
	/// 
	/// </para>
	/// <para>
	/// The following example shows how to compare two strings using
	/// the <code>Collator</code> for the default locale.
	/// <blockquote>
	/// <pre>{@code
	/// // Compare two strings in the default locale
	/// Collator myCollator = Collator.getInstance();
	/// if( myCollator.compare("abc", "ABC") < 0 )
	///     System.out.println("abc is less than ABC");
	/// else
	///     System.out.println("abc is greater than or equal to ABC");
	/// }</pre>
	/// </blockquote>
	/// 
	/// </para>
	/// <para>
	/// You can set a <code>Collator</code>'s <em>strength</em> property
	/// to determine the level of difference considered significant in
	/// comparisons. Four strengths are provided: <code>PRIMARY</code>,
	/// <code>SECONDARY</code>, <code>TERTIARY</code>, and <code>IDENTICAL</code>.
	/// The exact assignment of strengths to language features is
	/// locale dependant.  For example, in Czech, "e" and "f" are considered
	/// primary differences, while "e" and "&#283;" are secondary differences,
	/// "e" and "E" are tertiary differences and "e" and "e" are identical.
	/// The following shows how both case and accents could be ignored for
	/// US English.
	/// <blockquote>
	/// <pre>
	/// //Get the Collator for US English and set its strength to PRIMARY
	/// Collator usCollator = Collator.getInstance(Locale.US);
	/// usCollator.setStrength(Collator.PRIMARY);
	/// if( usCollator.compare("abc", "ABC") == 0 ) {
	///     System.out.println("Strings are equivalent");
	/// }
	/// </pre>
	/// </blockquote>
	/// </para>
	/// <para>
	/// For comparing <code>String</code>s exactly once, the <code>compare</code>
	/// method provides the best performance. When sorting a list of
	/// <code>String</code>s however, it is generally necessary to compare each
	/// <code>String</code> multiple times. In this case, <code>CollationKey</code>s
	/// provide better performance. The <code>CollationKey</code> class converts
	/// a <code>String</code> to a series of bits that can be compared bitwise
	/// against other <code>CollationKey</code>s. A <code>CollationKey</code> is
	/// created by a <code>Collator</code> object for a given <code>String</code>.
	/// <br>
	/// <strong>Note:</strong> <code>CollationKey</code>s from different
	/// <code>Collator</code>s can not be compared. See the class description
	/// for <seealso cref="CollationKey"/>
	/// for an example using <code>CollationKey</code>s.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=         RuleBasedCollator </seealso>
	/// <seealso cref=         CollationKey </seealso>
	/// <seealso cref=         CollationElementIterator </seealso>
	/// <seealso cref=         Locale
	/// @author      Helena Shih, Laura Werner, Richard Gillam </seealso>

	public abstract class Collator : IComparer<Object>, Cloneable
	{
		/// <summary>
		/// Collator strength value.  When set, only PRIMARY differences are
		/// considered significant during comparison. The assignment of strengths
		/// to language features is locale dependant. A common example is for
		/// different base letters ("a" vs "b") to be considered a PRIMARY difference. </summary>
		/// <seealso cref= java.text.Collator#setStrength </seealso>
		/// <seealso cref= java.text.Collator#getStrength </seealso>
		public const int PRIMARY = 0;
		/// <summary>
		/// Collator strength value.  When set, only SECONDARY and above differences are
		/// considered significant during comparison. The assignment of strengths
		/// to language features is locale dependant. A common example is for
		/// different accented forms of the same base letter ("a" vs "\u00E4") to be
		/// considered a SECONDARY difference. </summary>
		/// <seealso cref= java.text.Collator#setStrength </seealso>
		/// <seealso cref= java.text.Collator#getStrength </seealso>
		public const int SECONDARY = 1;
		/// <summary>
		/// Collator strength value.  When set, only TERTIARY and above differences are
		/// considered significant during comparison. The assignment of strengths
		/// to language features is locale dependant. A common example is for
		/// case differences ("a" vs "A") to be considered a TERTIARY difference. </summary>
		/// <seealso cref= java.text.Collator#setStrength </seealso>
		/// <seealso cref= java.text.Collator#getStrength </seealso>
		public const int TERTIARY = 2;

		/// <summary>
		/// Collator strength value.  When set, all differences are
		/// considered significant during comparison. The assignment of strengths
		/// to language features is locale dependant. A common example is for control
		/// characters ("&#092;u0001" vs "&#092;u0002") to be considered equal at the
		/// PRIMARY, SECONDARY, and TERTIARY levels but different at the IDENTICAL
		/// level.  Additionally, differences between pre-composed accents such as
		/// "&#092;u00C0" (A-grave) and combining accents such as "A&#092;u0300"
		/// (A, combining-grave) will be considered significant at the IDENTICAL
		/// level if decomposition is set to NO_DECOMPOSITION.
		/// </summary>
		public const int IDENTICAL = 3;

		/// <summary>
		/// Decomposition mode value. With NO_DECOMPOSITION
		/// set, accented characters will not be decomposed for collation. This
		/// is the default setting and provides the fastest collation but
		/// will only produce correct results for languages that do not use accents. </summary>
		/// <seealso cref= java.text.Collator#getDecomposition </seealso>
		/// <seealso cref= java.text.Collator#setDecomposition </seealso>
		public const int NO_DECOMPOSITION = 0;

		/// <summary>
		/// Decomposition mode value. With CANONICAL_DECOMPOSITION
		/// set, characters that are canonical variants according to Unicode
		/// standard will be decomposed for collation. This should be used to get
		/// correct collation of accented characters.
		/// <para>
		/// CANONICAL_DECOMPOSITION corresponds to Normalization Form D as
		/// described in
		/// <a href="http://www.unicode.org/unicode/reports/tr15/tr15-23.html">Unicode
		/// Technical Report #15</a>.
		/// </para>
		/// </summary>
		/// <seealso cref= java.text.Collator#getDecomposition </seealso>
		/// <seealso cref= java.text.Collator#setDecomposition </seealso>
		public const int CANONICAL_DECOMPOSITION = 1;

		/// <summary>
		/// Decomposition mode value. With FULL_DECOMPOSITION
		/// set, both Unicode canonical variants and Unicode compatibility variants
		/// will be decomposed for collation.  This causes not only accented
		/// characters to be collated, but also characters that have special formats
		/// to be collated with their norminal form. For example, the half-width and
		/// full-width ASCII and Katakana characters are then collated together.
		/// FULL_DECOMPOSITION is the most complete and therefore the slowest
		/// decomposition mode.
		/// <para>
		/// FULL_DECOMPOSITION corresponds to Normalization Form KD as
		/// described in
		/// <a href="http://www.unicode.org/unicode/reports/tr15/tr15-23.html">Unicode
		/// Technical Report #15</a>.
		/// </para>
		/// </summary>
		/// <seealso cref= java.text.Collator#getDecomposition </seealso>
		/// <seealso cref= java.text.Collator#setDecomposition </seealso>
		public const int FULL_DECOMPOSITION = 2;

		/// <summary>
		/// Gets the Collator for the current default locale.
		/// The default locale is determined by java.util.Locale.getDefault. </summary>
		/// <returns> the Collator for the default locale.(for example, en_US) </returns>
		/// <seealso cref= java.util.Locale#getDefault </seealso>
		public static Collator Instance
		{
			get
			{
				lock (typeof(Collator))
				{
					return GetInstance(Locale.Default);
				}
			}
		}

		/// <summary>
		/// Gets the Collator for the desired locale. </summary>
		/// <param name="desiredLocale"> the desired locale. </param>
		/// <returns> the Collator for the desired locale. </returns>
		/// <seealso cref= java.util.Locale </seealso>
		/// <seealso cref= java.util.ResourceBundle </seealso>
		public static Collator GetInstance(Locale desiredLocale)
		{
			SoftReference<Collator> @ref = Cache[desiredLocale];
			Collator result = (@ref != null) ? @ref.get() : null;
			if (result == null)
			{
				LocaleProviderAdapter adapter;
				adapter = LocaleProviderAdapter.getAdapter(typeof(CollatorProvider), desiredLocale);
				CollatorProvider provider = adapter.CollatorProvider;
				result = provider.GetInstance(desiredLocale);
				if (result == null)
				{
					result = LocaleProviderAdapter.forJRE().CollatorProvider.getInstance(desiredLocale);
				}
				while (true)
				{
					if (@ref != null)
					{
						// Remove the empty SoftReference if any
						Cache.Remove(desiredLocale, @ref);
					}
					@ref = Cache.PutIfAbsent(desiredLocale, new SoftReference<>(result));
					if (@ref == null)
					{
						break;
					}
					Collator cachedColl = @ref.get();
					if (cachedColl != null)
					{
						result = cachedColl;
						break;
					}
				}
			}
			return (Collator) result.Clone(); // make the world safe
		}

		/// <summary>
		/// Compares the source string to the target string according to the
		/// collation rules for this Collator.  Returns an integer less than,
		/// equal to or greater than zero depending on whether the source String is
		/// less than, equal to or greater than the target string.  See the Collator
		/// class description for an example of use.
		/// <para>
		/// For a one time comparison, this method has the best performance. If a
		/// given String will be involved in multiple comparisons, CollationKey.compareTo
		/// has the best performance. See the Collator class description for an example
		/// using CollationKeys.
		/// </para>
		/// </summary>
		/// <param name="source"> the source string. </param>
		/// <param name="target"> the target string. </param>
		/// <returns> Returns an integer value. Value is less than zero if source is less than
		/// target, value is zero if source and target are equal, value is greater than zero
		/// if source is greater than target. </returns>
		/// <seealso cref= java.text.CollationKey </seealso>
		/// <seealso cref= java.text.Collator#getCollationKey </seealso>
		public abstract int Compare(String source, String target);

		/// <summary>
		/// Compares its two arguments for order.  Returns a negative integer,
		/// zero, or a positive integer as the first argument is less than, equal
		/// to, or greater than the second.
		/// <para>
		/// This implementation merely returns
		///  <code> compare((String)o1, (String)o2) </code>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a negative integer, zero, or a positive integer as the
		///         first argument is less than, equal to, or greater than the
		///         second. </returns>
		/// <exception cref="ClassCastException"> the arguments cannot be cast to Strings. </exception>
		/// <seealso cref= java.util.Comparator
		/// @since   1.2 </seealso>
		public virtual int Compare(Object o1, Object o2)
		{
		return Compare((String)o1, (String)o2);
		}

		/// <summary>
		/// Transforms the String into a series of bits that can be compared bitwise
		/// to other CollationKeys. CollationKeys provide better performance than
		/// Collator.compare when Strings are involved in multiple comparisons.
		/// See the Collator class description for an example using CollationKeys. </summary>
		/// <param name="source"> the string to be transformed into a collation key. </param>
		/// <returns> the CollationKey for the given String based on this Collator's collation
		/// rules. If the source String is null, a null CollationKey is returned. </returns>
		/// <seealso cref= java.text.CollationKey </seealso>
		/// <seealso cref= java.text.Collator#compare </seealso>
		public abstract CollationKey GetCollationKey(String source);

		/// <summary>
		/// Convenience method for comparing the equality of two strings based on
		/// this Collator's collation rules. </summary>
		/// <param name="source"> the source string to be compared with. </param>
		/// <param name="target"> the target string to be compared with. </param>
		/// <returns> true if the strings are equal according to the collation
		/// rules.  false, otherwise. </returns>
		/// <seealso cref= java.text.Collator#compare </seealso>
		public virtual bool Equals(String source, String target)
		{
			return (Compare(source, target) == Collator.EQUAL);
		}

		/// <summary>
		/// Returns this Collator's strength property.  The strength property determines
		/// the minimum level of difference considered significant during comparison.
		/// See the Collator class description for an example of use. </summary>
		/// <returns> this Collator's current strength property. </returns>
		/// <seealso cref= java.text.Collator#setStrength </seealso>
		/// <seealso cref= java.text.Collator#PRIMARY </seealso>
		/// <seealso cref= java.text.Collator#SECONDARY </seealso>
		/// <seealso cref= java.text.Collator#TERTIARY </seealso>
		/// <seealso cref= java.text.Collator#IDENTICAL </seealso>
		public virtual int Strength
		{
			get
			{
				lock (this)
				{
					return Strength_Renamed;
				}
			}
			set
			{
				lock (this)
				{
					if ((value != PRIMARY) && (value != SECONDARY) && (value != TERTIARY) && (value != IDENTICAL))
					{
						throw new IllegalArgumentException("Incorrect comparison level.");
					}
					Strength_Renamed = value;
				}
			}
		}


		/// <summary>
		/// Get the decomposition mode of this Collator. Decomposition mode
		/// determines how Unicode composed characters are handled. Adjusting
		/// decomposition mode allows the user to select between faster and more
		/// complete collation behavior.
		/// <para>The three values for decomposition mode are:
		/// <UL>
		/// <LI>NO_DECOMPOSITION,
		/// <LI>CANONICAL_DECOMPOSITION
		/// <LI>FULL_DECOMPOSITION.
		/// </UL>
		/// See the documentation for these three constants for a description
		/// of their meaning.
		/// </para>
		/// </summary>
		/// <returns> the decomposition mode </returns>
		/// <seealso cref= java.text.Collator#setDecomposition </seealso>
		/// <seealso cref= java.text.Collator#NO_DECOMPOSITION </seealso>
		/// <seealso cref= java.text.Collator#CANONICAL_DECOMPOSITION </seealso>
		/// <seealso cref= java.text.Collator#FULL_DECOMPOSITION </seealso>
		public virtual int Decomposition
		{
			get
			{
				lock (this)
				{
					return Decmp;
				}
			}
			set
			{
				lock (this)
				{
					if ((value != NO_DECOMPOSITION) && (value != CANONICAL_DECOMPOSITION) && (value != FULL_DECOMPOSITION))
					{
						throw new IllegalArgumentException("Wrong decomposition mode.");
					}
					Decmp = value;
				}
			}
		}

		/// <summary>
		/// Returns an array of all locales for which the
		/// <code>getInstance</code> methods of this class can return
		/// localized instances.
		/// The returned array represents the union of locales supported
		/// by the Java runtime and by installed
		/// <seealso cref="java.text.spi.CollatorProvider CollatorProvider"/> implementations.
		/// It must contain at least a Locale instance equal to
		/// <seealso cref="java.util.Locale#US Locale.US"/>.
		/// </summary>
		/// <returns> An array of locales for which localized
		///         <code>Collator</code> instances are available. </returns>
		public static Locale[] AvailableLocales
		{
			get
			{
				lock (typeof(Collator))
				{
					LocaleServiceProviderPool pool = LocaleServiceProviderPool.getPool(typeof(CollatorProvider));
					return pool.AvailableLocales;
				}
			}
		}

		/// <summary>
		/// Overrides Cloneable
		/// </summary>
		public override Object Clone()
		{
			try
			{
				return (Collator)base.Clone();
			}
			catch (CloneNotSupportedException e)
			{
				throw new InternalError(e);
			}
		}

		/// <summary>
		/// Compares the equality of two Collators. </summary>
		/// <param name="that"> the Collator to be compared with this. </param>
		/// <returns> true if this Collator is the same as that Collator;
		/// false otherwise. </returns>
		public override bool Equals(Object that)
		{
			if (this == that)
			{
				return true;
			}
			if (that == null)
			{
				return false;
			}
			if (this.GetType() != that.GetType())
			{
				return false;
			}
			Collator other = (Collator) that;
			return ((Strength_Renamed == other.Strength_Renamed) && (Decmp == other.Decmp));
		}

		/// <summary>
		/// Generates the hash code for this Collator.
		/// </summary>
		public override abstract int HashCode();

		/// <summary>
		/// Default constructor.  This constructor is
		/// protected so subclasses can get access to it. Users typically create
		/// a Collator sub-class by calling the factory method getInstance. </summary>
		/// <seealso cref= java.text.Collator#getInstance </seealso>
		protected internal Collator()
		{
			Strength_Renamed = TERTIARY;
			Decmp = CANONICAL_DECOMPOSITION;
		}

		private int Strength_Renamed = 0;
		private int Decmp = 0;
		private static readonly ConcurrentMap<Locale, SoftReference<Collator>> Cache = new ConcurrentDictionary<Locale, SoftReference<Collator>>();

		//
		// FIXME: These three constants should be removed.
		//
		/// <summary>
		/// LESS is returned if source string is compared to be less than target
		/// string in the compare() method. </summary>
		/// <seealso cref= java.text.Collator#compare </seealso>
		internal const int LESS = -1;
		/// <summary>
		/// EQUAL is returned if source string is compared to be equal to target
		/// string in the compare() method. </summary>
		/// <seealso cref= java.text.Collator#compare </seealso>
		internal const int EQUAL = 0;
		/// <summary>
		/// GREATER is returned if source string is compared to be greater than
		/// target string in the compare() method. </summary>
		/// <seealso cref= java.text.Collator#compare </seealso>
		internal const int GREATER = 1;
	}

}