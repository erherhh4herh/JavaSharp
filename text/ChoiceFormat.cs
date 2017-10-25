using System;

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
	/// A <code>ChoiceFormat</code> allows you to attach a format to a range of numbers.
	/// It is generally used in a <code>MessageFormat</code> for handling plurals.
	/// The choice is specified with an ascending list of doubles, where each item
	/// specifies a half-open interval up to the next item:
	/// <blockquote>
	/// <pre>
	/// X matches j if and only if limit[j] &le; X &lt; limit[j+1]
	/// </pre>
	/// </blockquote>
	/// If there is no match, then either the first or last index is used, depending
	/// on whether the number (X) is too low or too high.  If the limit array is not
	/// in ascending order, the results of formatting will be incorrect.  ChoiceFormat
	/// also accepts <code>&#92;u221E</code> as equivalent to infinity(INF).
	/// 
	/// <para>
	/// <strong>Note:</strong>
	/// <code>ChoiceFormat</code> differs from the other <code>Format</code>
	/// classes in that you create a <code>ChoiceFormat</code> object with a
	/// constructor (not with a <code>getInstance</code> style factory
	/// method). The factory methods aren't necessary because <code>ChoiceFormat</code>
	/// doesn't require any complex setup for a given locale. In fact,
	/// <code>ChoiceFormat</code> doesn't implement any locale specific behavior.
	/// 
	/// </para>
	/// <para>
	/// When creating a <code>ChoiceFormat</code>, you must specify an array of formats
	/// and an array of limits. The length of these arrays must be the same.
	/// For example,
	/// <ul>
	/// <li>
	///     <em>limits</em> = {1,2,3,4,5,6,7}<br>
	///     <em>formats</em> = {"Sun","Mon","Tue","Wed","Thur","Fri","Sat"}
	/// <li>
	///     <em>limits</em> = {0, 1, ChoiceFormat.nextDouble(1)}<br>
	///     <em>formats</em> = {"no files", "one file", "many files"}<br>
	///     (<code>nextDouble</code> can be used to get the next higher double, to
	///     make the half-open interval.)
	/// </ul>
	/// 
	/// </para>
	/// <para>
	/// Here is a simple example that shows formatting and parsing:
	/// <blockquote>
	/// <pre>{@code
	/// double[] limits = {1,2,3,4,5,6,7};
	/// String[] dayOfWeekNames = {"Sun","Mon","Tue","Wed","Thur","Fri","Sat"};
	/// ChoiceFormat form = new ChoiceFormat(limits, dayOfWeekNames);
	/// ParsePosition status = new ParsePosition(0);
	/// for (double i = 0.0; i <= 8.0; ++i) {
	///     status.setIndex(0);
	///     System.out.println(i + " -> " + form.format(i) + " -> "
	///                              + form.parse(form.format(i),status));
	/// }
	/// }</pre>
	/// </blockquote>
	/// Here is a more complex example, with a pattern format:
	/// <blockquote>
	/// <pre>{@code
	/// double[] filelimits = {0,1,2};
	/// String[] filepart = {"are no files","is one file","are {2} files"};
	/// ChoiceFormat fileform = new ChoiceFormat(filelimits, filepart);
	/// Format[] testFormats = {fileform, null, NumberFormat.getInstance()};
	/// MessageFormat pattform = new MessageFormat("There {0} on {1}");
	/// pattform.setFormats(testFormats);
	/// Object[] testArgs = {null, "ADisk", null};
	/// for (int i = 0; i < 4; ++i) {
	///     testArgs[0] = new Integer(i);
	///     testArgs[2] = testArgs[0];
	///     System.out.println(pattform.format(testArgs));
	/// }
	/// }</pre>
	/// </blockquote>
	/// </para>
	/// <para>
	/// Specifying a pattern for ChoiceFormat objects is fairly straightforward.
	/// For example:
	/// <blockquote>
	/// <pre>{@code
	/// ChoiceFormat fmt = new ChoiceFormat(
	///      "-1#is negative| 0#is zero or fraction | 1#is one |1.0<is 1+ |2#is two |2<is more than 2.");
	/// System.out.println("Formatter Pattern : " + fmt.toPattern());
	/// 
	/// System.out.println("Format with -INF : " + fmt.format(Double.NEGATIVE_INFINITY));
	/// System.out.println("Format with -1.0 : " + fmt.format(-1.0));
	/// System.out.println("Format with 0 : " + fmt.format(0));
	/// System.out.println("Format with 0.9 : " + fmt.format(0.9));
	/// System.out.println("Format with 1.0 : " + fmt.format(1));
	/// System.out.println("Format with 1.5 : " + fmt.format(1.5));
	/// System.out.println("Format with 2 : " + fmt.format(2));
	/// System.out.println("Format with 2.1 : " + fmt.format(2.1));
	/// System.out.println("Format with NaN : " + fmt.format(Double.NaN));
	/// System.out.println("Format with +INF : " + fmt.format(Double.POSITIVE_INFINITY));
	/// }</pre>
	/// </blockquote>
	/// And the output result would be like the following:
	/// <blockquote>
	/// <pre>{@code
	/// Format with -INF : is negative
	/// Format with -1.0 : is negative
	/// Format with 0 : is zero or fraction
	/// Format with 0.9 : is zero or fraction
	/// Format with 1.0 : is one
	/// Format with 1.5 : is 1+
	/// Format with 2 : is two
	/// Format with 2.1 : is more than 2.
	/// Format with NaN : is negative
	/// Format with +INF : is more than 2.
	/// }</pre>
	/// </blockquote>
	/// 
	/// <h3><a name="synchronization">Synchronization</a></h3>
	/// 
	/// </para>
	/// <para>
	/// Choice formats are not synchronized.
	/// It is recommended to create separate format instances for each thread.
	/// If multiple threads access a format concurrently, it must be synchronized
	/// externally.
	/// 
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref=          DecimalFormat </seealso>
	/// <seealso cref=          MessageFormat
	/// @author       Mark Davis </seealso>
	public class ChoiceFormat : NumberFormat
	{

		// Proclaim serial compatibility with 1.1 FCS
		private new const long SerialVersionUID = 1795184449645032964L;

		/// <summary>
		/// Sets the pattern. </summary>
		/// <param name="newPattern"> See the class description. </param>
		public virtual void ApplyPattern(String newPattern)
		{
			StringBuffer[] segments = new StringBuffer[2];
			for (int i = 0; i < segments.Length; ++i)
			{
				segments[i] = new StringBuffer();
			}
			double[] newChoiceLimits = new double[30];
			String[] newChoiceFormats = new String[30];
			int count = 0;
			int part = 0;
			double startValue = 0;
			double oldStartValue = Double.NaN_Renamed;
			bool inQuote = false;
			for (int i = 0; i < newPattern.Length(); ++i)
			{
				char ch = newPattern.CharAt(i);
				if (ch == '\'')
				{
					// Check for "''" indicating a literal quote
					if ((i + 1) < newPattern.Length() && newPattern.CharAt(i + 1) == ch)
					{
						segments[part].Append(ch);
						++i;
					}
					else
					{
						inQuote = !inQuote;
					}
				}
				else if (inQuote)
				{
					segments[part].Append(ch);
				}
				else if (ch == '<' || ch == '#' || ch == '\u2264')
				{
					if (segments[0].Length() == 0)
					{
						throw new IllegalArgumentException();
					}
					try
					{
						String tempBuffer = segments[0].ToString();
						if (tempBuffer.Equals("\u221E"))
						{
							startValue = Double.PositiveInfinity;
						}
						else if (tempBuffer.Equals("-\u221E"))
						{
							startValue = Double.NegativeInfinity;
						}
						else
						{
							startValue = Convert.ToDouble(segments[0].ToString()).DoubleValue();
						}
					}
					catch (Exception)
					{
						throw new IllegalArgumentException();
					}
					if (ch == '<' && startValue != Double.PositiveInfinity && startValue != Double.NegativeInfinity)
					{
						startValue = NextDouble(startValue);
					}
					if (startValue <= oldStartValue)
					{
						throw new IllegalArgumentException();
					}
					segments[0].Length = 0;
					part = 1;
				}
				else if (ch == '|')
				{
					if (count == newChoiceLimits.Length)
					{
						newChoiceLimits = DoubleArraySize(newChoiceLimits);
						newChoiceFormats = DoubleArraySize(newChoiceFormats);
					}
					newChoiceLimits[count] = startValue;
					newChoiceFormats[count] = segments[1].ToString();
					++count;
					oldStartValue = startValue;
					segments[1].Length = 0;
					part = 0;
				}
				else
				{
					segments[part].Append(ch);
				}
			}
			// clean up last one
			if (part == 1)
			{
				if (count == newChoiceLimits.Length)
				{
					newChoiceLimits = DoubleArraySize(newChoiceLimits);
					newChoiceFormats = DoubleArraySize(newChoiceFormats);
				}
				newChoiceLimits[count] = startValue;
				newChoiceFormats[count] = segments[1].ToString();
				++count;
			}
			ChoiceLimits = new double[count];
			System.Array.Copy(newChoiceLimits, 0, ChoiceLimits, 0, count);
			ChoiceFormats = new String[count];
			System.Array.Copy(newChoiceFormats, 0, ChoiceFormats, 0, count);
		}

		/// <summary>
		/// Gets the pattern.
		/// </summary>
		/// <returns> the pattern string </returns>
		public virtual String ToPattern()
		{
			StringBuffer result = new StringBuffer();
			for (int i = 0; i < ChoiceLimits.Length; ++i)
			{
				if (i != 0)
				{
					result.Append('|');
				}
				// choose based upon which has less precision
				// approximate that by choosing the closest one to an integer.
				// could do better, but it's not worth it.
				double less = PreviousDouble(ChoiceLimits[i]);
				double tryLessOrEqual = System.Math.Abs(System.Math.IEEERemainder(ChoiceLimits[i], 1.0d));
				double tryLess = System.Math.Abs(System.Math.IEEERemainder(less, 1.0d));

				if (tryLessOrEqual < tryLess)
				{
					result.Append("" + ChoiceLimits[i]);
					result.Append('#');
				}
				else
				{
					if (ChoiceLimits[i] == Double.PositiveInfinity)
					{
						result.Append("\u221E");
					}
					else if (ChoiceLimits[i] == Double.NegativeInfinity)
					{
						result.Append("-\u221E");
					}
					else
					{
						result.Append("" + less);
					}
					result.Append('<');
				}
				// Append choiceFormats[i], using quotes if there are special characters.
				// Single quotes themselves must be escaped in either case.
				String text = ChoiceFormats[i];
				bool needQuote = text.IndexOf('<') >= 0 || text.IndexOf('#') >= 0 || text.IndexOf('\u2264') >= 0 || text.IndexOf('|') >= 0;
				if (needQuote)
				{
					result.Append('\'');
				}
				if (text.IndexOf('\'') < 0)
				{
					result.Append(text);
				}
				else
				{
					for (int j = 0; j < text.Length(); ++j)
					{
						char c = text.CharAt(j);
						result.Append(c);
						if (c == '\'')
						{
							result.Append(c);
						}
					}
				}
				if (needQuote)
				{
					result.Append('\'');
				}
			}
			return result.ToString();
		}

		/// <summary>
		/// Constructs with limits and corresponding formats based on the pattern.
		/// </summary>
		/// <param name="newPattern"> the new pattern string </param>
		/// <seealso cref= #applyPattern </seealso>
		public ChoiceFormat(String newPattern)
		{
			ApplyPattern(newPattern);
		}

		/// <summary>
		/// Constructs with the limits and the corresponding formats.
		/// </summary>
		/// <param name="limits"> limits in ascending order </param>
		/// <param name="formats"> corresponding format strings </param>
		/// <seealso cref= #setChoices </seealso>
		public ChoiceFormat(double[] limits, String[] formats)
		{
			SetChoices(limits, formats);
		}

		/// <summary>
		/// Set the choices to be used in formatting. </summary>
		/// <param name="limits"> contains the top value that you want
		/// parsed with that format, and should be in ascending sorted order. When
		/// formatting X, the choice will be the i, where
		/// limit[i] &le; X {@literal <} limit[i+1].
		/// If the limit array is not in ascending order, the results of formatting
		/// will be incorrect. </param>
		/// <param name="formats"> are the formats you want to use for each limit.
		/// They can be either Format objects or Strings.
		/// When formatting with object Y,
		/// if the object is a NumberFormat, then ((NumberFormat) Y).format(X)
		/// is called. Otherwise Y.toString() is called. </param>
		public virtual void SetChoices(double[] limits, String[] formats)
		{
			if (limits.Length != formats.Length)
			{
				throw new IllegalArgumentException("Array and limit arrays must be of the same length.");
			}
			ChoiceLimits = Arrays.CopyOf(limits, limits.Length);
			ChoiceFormats = Arrays.CopyOf(formats, formats.Length);
		}

		/// <summary>
		/// Get the limits passed in the constructor. </summary>
		/// <returns> the limits. </returns>
		public virtual double[] Limits
		{
			get
			{
				double[] newLimits = Arrays.CopyOf(ChoiceLimits, ChoiceLimits.Length);
				return newLimits;
			}
		}

		/// <summary>
		/// Get the formats passed in the constructor. </summary>
		/// <returns> the formats. </returns>
		public virtual Object[] Formats
		{
			get
			{
				Object[] newFormats = Arrays.CopyOf(ChoiceFormats, ChoiceFormats.Length);
				return newFormats;
			}
		}

		// Overrides

		/// <summary>
		/// Specialization of format. This method really calls
		/// <code>format(double, StringBuffer, FieldPosition)</code>
		/// thus the range of longs that are supported is only equal to
		/// the range that can be stored by double. This will never be
		/// a practical limitation.
		/// </summary>
		public override StringBuffer Format(long number, StringBuffer toAppendTo, FieldPosition status)
		{
			return Format((double)number, toAppendTo, status);
		}

		/// <summary>
		/// Returns pattern with formatted double. </summary>
		/// <param name="number"> number to be formatted and substituted. </param>
		/// <param name="toAppendTo"> where text is appended. </param>
		/// <param name="status"> ignore no useful status is returned. </param>
	   public override StringBuffer Format(double number, StringBuffer toAppendTo, FieldPosition status)
	   {
			// find the number
			int i;
			for (i = 0; i < ChoiceLimits.Length; ++i)
			{
				if (!(number >= ChoiceLimits[i]))
				{
					// same as number < choiceLimits, except catchs NaN
					break;
				}
			}
			--i;
			if (i < 0)
			{
				i = 0;
			}
			// return either a formatted number, or a string
			return toAppendTo.Append(ChoiceFormats[i]);
	   }

		/// <summary>
		/// Parses a Number from the input text. </summary>
		/// <param name="text"> the source text. </param>
		/// <param name="status"> an input-output parameter.  On input, the
		/// status.index field indicates the first character of the
		/// source text that should be parsed.  On exit, if no error
		/// occurred, status.index is set to the first unparsed character
		/// in the source text.  On exit, if an error did occur,
		/// status.index is unchanged and status.errorIndex is set to the
		/// first index of the character that caused the parse to fail. </param>
		/// <returns> A Number representing the value of the number parsed. </returns>
		public override Number Parse(String text, ParsePosition status)
		{
			// find the best number (defined as the one with the longest parse)
			int start = status.Index_Renamed;
			int furthest = start;
			double bestNumber = Double.NaN_Renamed;
			double tempNumber = 0.0;
			for (int i = 0; i < ChoiceFormats.Length; ++i)
			{
				String tempString = ChoiceFormats[i];
				if (text.RegionMatches(start, tempString, 0, tempString.Length()))
				{
					status.Index_Renamed = start + tempString.Length();
					tempNumber = ChoiceLimits[i];
					if (status.Index_Renamed > furthest)
					{
						furthest = status.Index_Renamed;
						bestNumber = tempNumber;
						if (furthest == text.Length())
						{
							break;
						}
					}
				}
			}
			status.Index_Renamed = furthest;
			if (status.Index_Renamed == start)
			{
				status.ErrorIndex_Renamed = furthest;
			}
			return new Double(bestNumber);
		}

		/// <summary>
		/// Finds the least double greater than {@code d}.
		/// If {@code NaN}, returns same value.
		/// <para>Used to make half-open intervals.
		/// 
		/// </para>
		/// </summary>
		/// <param name="d"> the reference value </param>
		/// <returns> the least double value greather than {@code d} </returns>
		/// <seealso cref= #previousDouble </seealso>
		public static double NextDouble(double d)
		{
			return NextDouble(d,true);
		}

		/// <summary>
		/// Finds the greatest double less than {@code d}.
		/// If {@code NaN}, returns same value.
		/// </summary>
		/// <param name="d"> the reference value </param>
		/// <returns> the greatest double value less than {@code d} </returns>
		/// <seealso cref= #nextDouble </seealso>
		public static double PreviousDouble(double d)
		{
			return NextDouble(d,false);
		}

		/// <summary>
		/// Overrides Cloneable
		/// </summary>
		public override Object Clone()
		{
			ChoiceFormat other = (ChoiceFormat) base.Clone();
			// for primitives or immutables, shallow clone is enough
			other.ChoiceLimits = ChoiceLimits.clone();
			other.ChoiceFormats = ChoiceFormats.clone();
			return other;
		}

		/// <summary>
		/// Generates a hash code for the message format object.
		/// </summary>
		public override int HashCode()
		{
			int result = ChoiceLimits.Length;
			if (ChoiceFormats.Length > 0)
			{
				// enough for reasonable distribution
				result ^= ChoiceFormats[ChoiceFormats.Length - 1].HashCode();
			}
			return result;
		}

		/// <summary>
		/// Equality comparision between two
		/// </summary>
		public override bool Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj) // quick check
			{
				return true;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			ChoiceFormat other = (ChoiceFormat) obj;
			return (Arrays.Equals(ChoiceLimits, other.ChoiceLimits) && Arrays.Equals(ChoiceFormats, other.ChoiceFormats));
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
			if (ChoiceLimits.Length != ChoiceFormats.Length)
			{
				throw new InvalidObjectException("limits and format arrays of different length.");
			}
		}

		// ===============privates===========================

		/// <summary>
		/// A list of lower bounds for the choices.  The formatter will return
		/// <code>choiceFormats[i]</code> if the number being formatted is greater than or equal to
		/// <code>choiceLimits[i]</code> and less than <code>choiceLimits[i+1]</code>.
		/// @serial
		/// </summary>
		private double[] ChoiceLimits;

		/// <summary>
		/// A list of choice strings.  The formatter will return
		/// <code>choiceFormats[i]</code> if the number being formatted is greater than or equal to
		/// <code>choiceLimits[i]</code> and less than <code>choiceLimits[i+1]</code>.
		/// @serial
		/// </summary>
		private String[] ChoiceFormats;

		/*
		static final long SIGN          = 0x8000000000000000L;
		static final long EXPONENT      = 0x7FF0000000000000L;
		static final long SIGNIFICAND   = 0x000FFFFFFFFFFFFFL;
	
		private static double nextDouble (double d, boolean positive) {
		    if (Double.isNaN(d) || Double.isInfinite(d)) {
		            return d;
		        }
		    long bits = Double.doubleToLongBits(d);
		    long significand = bits & SIGNIFICAND;
		    if (bits < 0) {
		        significand |= (SIGN | EXPONENT);
		    }
		    long exponent = bits & EXPONENT;
		    if (positive) {
		        significand += 1;
		        // FIXME fix overflow & underflow
		    } else {
		        significand -= 1;
		        // FIXME fix overflow & underflow
		    }
		    bits = exponent | (significand & ~EXPONENT);
		    return Double.longBitsToDouble(bits);
		}
		*/

		internal const long SIGN = unchecked((long)0x8000000000000000L);
		internal const long EXPONENT = 0x7FF0000000000000L;
		internal const long POSITIVEINFINITY = 0x7FF0000000000000L;

		/// <summary>
		/// Finds the least double greater than {@code d} (if {@code positive} is
		/// {@code true}), or the greatest double less than {@code d} (if
		/// {@code positive} is {@code false}).
		/// If {@code NaN}, returns same value.
		/// 
		/// Does not affect floating-point flags,
		/// provided these member functions do not:
		///          Double.longBitsToDouble(long)
		///          Double.doubleToLongBits(double)
		///          Double.isNaN(double)
		/// </summary>
		/// <param name="d">        the reference value </param>
		/// <param name="positive"> {@code true} if the least double is desired;
		///                 {@code false} otherwise </param>
		/// <returns> the least or greater double value </returns>
		public static double NextDouble(double d, bool positive)
		{

			/* filter out NaN's */
			if (Double.IsNaN(d))
			{
				return d;
			}

			/* zero's are also a special case */
			if (d == 0.0)
			{
				double smallestPositiveDouble = Double.longBitsToDouble(1L);
				if (positive)
				{
					return smallestPositiveDouble;
				}
				else
				{
					return -smallestPositiveDouble;
				}
			}

			/* if entering here, d is a nonzero value */

			/* hold all bits in a long for later use */
			long bits = Double.DoubleToLongBits(d);

			/* strip off the sign bit */
			long magnitude = bits & ~SIGN;

			/* if next double away from zero, increase magnitude */
			if ((bits > 0) == positive)
			{
				if (magnitude != POSITIVEINFINITY)
				{
					magnitude += 1;
				}
			}
			/* else decrease magnitude */
			else
			{
				magnitude -= 1;
			}

			/* restore sign bit and return */
			long signbit = bits & SIGN;
			return Double.longBitsToDouble(magnitude | signbit);
		}

		private static double[] DoubleArraySize(double[] array)
		{
			int oldSize = array.Length;
			double[] newArray = new double[oldSize * 2];
			System.Array.Copy(array, 0, newArray, 0, oldSize);
			return newArray;
		}

		private String[] DoubleArraySize(String[] array)
		{
			int oldSize = array.Length;
			String[] newArray = new String[oldSize * 2];
			System.Array.Copy(array, 0, newArray, 0, oldSize);
			return newArray;
		}

	}

}