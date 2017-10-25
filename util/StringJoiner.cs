/*
 * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// {@code StringJoiner} is used to construct a sequence of characters separated
	/// by a delimiter and optionally starting with a supplied prefix
	/// and ending with a supplied suffix.
	/// <para>
	/// Prior to adding something to the {@code StringJoiner}, its
	/// {@code sj.toString()} method will, by default, return {@code prefix + suffix}.
	/// However, if the {@code setEmptyValue} method is called, the {@code emptyValue}
	/// supplied will be returned instead. This can be used, for example, when
	/// creating a string using set notation to indicate an empty set, i.e.
	/// <code>"{}"</code>, where the {@code prefix} is <code>"{"</code>, the
	/// {@code suffix} is <code>"}"</code> and nothing has been added to the
	/// {@code StringJoiner}.
	/// 
	/// @apiNote
	/// </para>
	/// <para>The String {@code "[George:Sally:Fred]"} may be constructed as follows:
	/// 
	/// <pre> {@code
	/// StringJoiner sj = new StringJoiner(":", "[", "]");
	/// sj.add("George").add("Sally").add("Fred");
	/// String desiredString = sj.toString();
	/// }</pre>
	/// </para>
	/// <para>
	/// A {@code StringJoiner} may be employed to create formatted output from a
	/// <seealso cref="java.util.stream.Stream"/> using
	/// <seealso cref="java.util.stream.Collectors#joining(CharSequence)"/>. For example:
	/// 
	/// <pre> {@code
	/// List<Integer> numbers = Arrays.asList(1, 2, 3, 4);
	/// String commaSeparatedNumbers = numbers.stream()
	///     .map(i -> i.toString())
	///     .collect(Collectors.joining(", "));
	/// }</pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.util.stream.Collectors#joining(CharSequence) </seealso>
	/// <seealso cref= java.util.stream.Collectors#joining(CharSequence, CharSequence, CharSequence)
	/// @since  1.8 </seealso>
	public sealed class StringJoiner
	{
		private readonly String Prefix;
		private readonly String Delimiter;
		private readonly String Suffix;

		/*
		 * StringBuilder value -- at any time, the characters constructed from the
		 * prefix, the added element separated by the delimiter, but without the
		 * suffix, so that we can more easily add elements without having to jigger
		 * the suffix each time.
		 */
		private StringBuilder Value;

		/*
		 * By default, the string consisting of prefix+suffix, returned by
		 * toString(), or properties of value, when no elements have yet been added,
		 * i.e. when it is empty.  This may be overridden by the user to be some
		 * other value including the empty String.
		 */
		private String EmptyValue_Renamed;

		/// <summary>
		/// Constructs a {@code StringJoiner} with no characters in it, with no
		/// {@code prefix} or {@code suffix}, and a copy of the supplied
		/// {@code delimiter}.
		/// If no characters are added to the {@code StringJoiner} and methods
		/// accessing the value of it are invoked, it will not return a
		/// {@code prefix} or {@code suffix} (or properties thereof) in the result,
		/// unless {@code setEmptyValue} has first been called.
		/// </summary>
		/// <param name="delimiter"> the sequence of characters to be used between each
		///         element added to the {@code StringJoiner} value </param>
		/// <exception cref="NullPointerException"> if {@code delimiter} is {@code null} </exception>
		public StringJoiner(CharSequence delimiter) : this(delimiter, "", "")
		{
		}

		/// <summary>
		/// Constructs a {@code StringJoiner} with no characters in it using copies
		/// of the supplied {@code prefix}, {@code delimiter} and {@code suffix}.
		/// If no characters are added to the {@code StringJoiner} and methods
		/// accessing the string value of it are invoked, it will return the
		/// {@code prefix + suffix} (or properties thereof) in the result, unless
		/// {@code setEmptyValue} has first been called.
		/// </summary>
		/// <param name="delimiter"> the sequence of characters to be used between each
		///         element added to the {@code StringJoiner} </param>
		/// <param name="prefix"> the sequence of characters to be used at the beginning </param>
		/// <param name="suffix"> the sequence of characters to be used at the end </param>
		/// <exception cref="NullPointerException"> if {@code prefix}, {@code delimiter}, or
		///         {@code suffix} is {@code null} </exception>
		public StringJoiner(CharSequence delimiter, CharSequence prefix, CharSequence suffix)
		{
			Objects.RequireNonNull(prefix, "The prefix must not be null");
			Objects.RequireNonNull(delimiter, "The delimiter must not be null");
			Objects.RequireNonNull(suffix, "The suffix must not be null");
			// make defensive copies of arguments
			this.Prefix = prefix.ToString();
			this.Delimiter = delimiter.ToString();
			this.Suffix = suffix.ToString();
			this.EmptyValue_Renamed = this.Prefix + this.Suffix;
		}

		/// <summary>
		/// Sets the sequence of characters to be used when determining the string
		/// representation of this {@code StringJoiner} and no elements have been
		/// added yet, that is, when it is empty.  A copy of the {@code emptyValue}
		/// parameter is made for this purpose. Note that once an add method has been
		/// called, the {@code StringJoiner} is no longer considered empty, even if
		/// the element(s) added correspond to the empty {@code String}.
		/// </summary>
		/// <param name="emptyValue"> the characters to return as the value of an empty
		///         {@code StringJoiner} </param>
		/// <returns> this {@code StringJoiner} itself so the calls may be chained </returns>
		/// <exception cref="NullPointerException"> when the {@code emptyValue} parameter is
		///         {@code null} </exception>
		public StringJoiner SetEmptyValue(CharSequence emptyValue)
		{
			this.EmptyValue_Renamed = Objects.RequireNonNull(emptyValue, "The empty value must not be null").ToString();
			return this;
		}

		/// <summary>
		/// Returns the current value, consisting of the {@code prefix}, the values
		/// added so far separated by the {@code delimiter}, and the {@code suffix},
		/// unless no elements have been added in which case, the
		/// {@code prefix + suffix} or the {@code emptyValue} characters are returned
		/// </summary>
		/// <returns> the string representation of this {@code StringJoiner} </returns>
		public override String ToString()
		{
			if (Value == null)
			{
				return EmptyValue_Renamed;
			}
			else
			{
				if (Suffix.Equals(""))
				{
					return Value.ToString();
				}
				else
				{
					int initialLength = Value.Length();
					String result = Value.Append(Suffix).ToString();
					// reset value to pre-append initialLength
					Value.Length = initialLength;
					return result;
				}
			}
		}

		/// <summary>
		/// Adds a copy of the given {@code CharSequence} value as the next
		/// element of the {@code StringJoiner} value. If {@code newElement} is
		/// {@code null}, then {@code "null"} is added.
		/// </summary>
		/// <param name="newElement"> The element to add </param>
		/// <returns> a reference to this {@code StringJoiner} </returns>
		public StringJoiner Add(CharSequence newElement)
		{
			PrepareBuilder().Append(newElement);
			return this;
		}

		/// <summary>
		/// Adds the contents of the given {@code StringJoiner} without prefix and
		/// suffix as the next element if it is non-empty. If the given {@code
		/// StringJoiner} is empty, the call has no effect.
		/// 
		/// <para>A {@code StringJoiner} is empty if <seealso cref="#add(CharSequence) add()"/>
		/// has never been called, and if {@code merge()} has never been called
		/// with a non-empty {@code StringJoiner} argument.
		/// 
		/// </para>
		/// <para>If the other {@code StringJoiner} is using a different delimiter,
		/// then elements from the other {@code StringJoiner} are concatenated with
		/// that delimiter and the result is appended to this {@code StringJoiner}
		/// as a single element.
		/// 
		/// </para>
		/// </summary>
		/// <param name="other"> The {@code StringJoiner} whose contents should be merged
		///              into this one </param>
		/// <exception cref="NullPointerException"> if the other {@code StringJoiner} is null </exception>
		/// <returns> This {@code StringJoiner} </returns>
		public StringJoiner Merge(StringJoiner other)
		{
			Objects.RequireNonNull(other);
			if (other.Value != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = other.value.length();
				int length = other.Value.Length();
				// lock the length so that we can seize the data to be appended
				// before initiate copying to avoid interference, especially when
				// merge 'this'
				StringBuilder builder = PrepareBuilder();
				builder.Append(other.Value, other.Prefix.Length(), length);
			}
			return this;
		}

		private StringBuilder PrepareBuilder()
		{
			if (Value != null)
			{
				Value.Append(Delimiter);
			}
			else
			{
				Value = (new StringBuilder()).Append(Prefix);
			}
			return Value;
		}

		/// <summary>
		/// Returns the length of the {@code String} representation
		/// of this {@code StringJoiner}. Note that if
		/// no add methods have been called, then the length of the {@code String}
		/// representation (either {@code prefix + suffix} or {@code emptyValue})
		/// will be returned. The value should be equivalent to
		/// {@code toString().length()}.
		/// </summary>
		/// <returns> the length of the current value of {@code StringJoiner} </returns>
		public int Length()
		{
			// Remember that we never actually append the suffix unless we return
			// the full (present) value or some sub-string or length of it, so that
			// we can add on more if we need to.
			return (Value != null ? Value.Length() + Suffix.Length() : EmptyValue_Renamed.Length());
		}
	}

}