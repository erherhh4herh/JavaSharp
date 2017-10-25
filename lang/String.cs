using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	/// The {@code String} class represents character strings. All
	/// string literals in Java programs, such as {@code "abc"}, are
	/// implemented as instances of this class.
	/// <para>
	/// Strings are constant; their values cannot be changed after they
	/// are created. String buffers support mutable strings.
	/// Because String objects are immutable they can be shared. For example:
	/// <blockquote><pre>
	///     String str = "abc";
	/// </para>
	/// </pre></blockquote><para>
	/// is equivalent to:
	/// <blockquote><pre>
	///     char data[] = {'a', 'b', 'c'};
	///     String str = new String(data);
	/// </para>
	/// </pre></blockquote><para>
	/// Here are some more examples of how strings can be used:
	/// <blockquote><pre>
	///     System.out.println("abc");
	///     String cde = "cde";
	///     System.out.println("abc" + cde);
	///     String c = "abc".substring(2,3);
	///     String d = cde.substring(1, 2);
	/// </pre></blockquote>
	/// </para>
	/// <para>
	/// The class {@code String} includes methods for examining
	/// individual characters of the sequence, for comparing strings, for
	/// searching strings, for extracting substrings, and for creating a
	/// copy of a string with all characters translated to uppercase or to
	/// lowercase. Case mapping is based on the Unicode Standard version
	/// specified by the <seealso cref="java.lang.Character Character"/> class.
	/// </para>
	/// <para>
	/// The Java language provides special support for the string
	/// concatenation operator (&nbsp;+&nbsp;), and for conversion of
	/// other objects to strings. String concatenation is implemented
	/// through the {@code StringBuilder}(or {@code StringBuffer})
	/// class and its {@code append} method.
	/// String conversions are implemented through the method
	/// {@code toString}, defined by {@code Object} and
	/// inherited by all classes in Java. For additional information on
	/// string concatenation and conversion, see Gosling, Joy, and Steele,
	/// <i>The Java Language Specification</i>.
	/// 
	/// </para>
	/// <para> Unless otherwise noted, passing a <tt>null</tt> argument to a constructor
	/// or method in this class will cause a <seealso cref="NullPointerException"/> to be
	/// thrown.
	/// 
	/// </para>
	/// <para>A {@code String} represents a string in the UTF-16 format
	/// in which <em>supplementary characters</em> are represented by <em>surrogate
	/// pairs</em> (see the section <a href="Character.html#unicode">Unicode
	/// Character Representations</a> in the {@code Character} class for
	/// more information).
	/// Index values refer to {@code char} code units, so a supplementary
	/// character uses two positions in a {@code String}.
	/// </para>
	/// <para>The {@code String} class provides methods for dealing with
	/// Unicode code points (i.e., characters), in addition to those for
	/// dealing with Unicode code units (i.e., {@code char} values).
	/// 
	/// @author  Lee Boynton
	/// @author  Arthur van Hoff
	/// @author  Martin Buchholz
	/// @author  Ulf Zibis
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.Object#toString() </seealso>
	/// <seealso cref=     java.lang.StringBuffer </seealso>
	/// <seealso cref=     java.lang.StringBuilder </seealso>
	/// <seealso cref=     java.nio.charset.Charset
	/// @since   JDK1.0 </seealso>

	[Serializable]
	public sealed class String : Comparable<String>, CharSequence
	{
		/// <summary>
		/// The value is used for character storage. </summary>
		private readonly char[] Value;

		/// <summary>
		/// Cache the hash code for the string </summary>
		private int Hash; // Default to 0

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		private const long SerialVersionUID = -6849794470754667710L;

		/// <summary>
		/// Class String is special cased within the Serialization Stream Protocol.
		/// 
		/// A String instance is written into an ObjectOutputStream according to
		/// <a href="{@docRoot}/../platform/serialization/spec/output.html">
		/// Object Serialization Specification, Section 6.2, "Stream Elements"</a>
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[0];

		/// <summary>
		/// Initializes a newly created {@code String} object so that it represents
		/// an empty character sequence.  Note that use of this constructor is
		/// unnecessary since Strings are immutable.
		/// </summary>
		public String()
		{
			this.Value = "".Value;
		}

		/// <summary>
		/// Initializes a newly created {@code String} object so that it represents
		/// the same sequence of characters as the argument; in other words, the
		/// newly created string is a copy of the argument string. Unless an
		/// explicit copy of {@code original} is needed, use of this constructor is
		/// unnecessary since Strings are immutable.
		/// </summary>
		/// <param name="original">
		///         A {@code String} </param>
		public String(String original)
		{
			this.Value = original.Value;
			this.Hash = original.Hash;
		}

		/// <summary>
		/// Allocates a new {@code String} so that it represents the sequence of
		/// characters currently contained in the character array argument. The
		/// contents of the character array are copied; subsequent modification of
		/// the character array does not affect the newly created string.
		/// </summary>
		/// <param name="value">
		///         The initial value of the string </param>
		public String(char[] value)
		{
			this.Value = Arrays.CopyOf(value, value.Length);
		}

		/// <summary>
		/// Allocates a new {@code String} that contains characters from a subarray
		/// of the character array argument. The {@code offset} argument is the
		/// index of the first character of the subarray and the {@code count}
		/// argument specifies the length of the subarray. The contents of the
		/// subarray are copied; subsequent modification of the character array does
		/// not affect the newly created string.
		/// </summary>
		/// <param name="value">
		///         Array that is the source of characters
		/// </param>
		/// <param name="offset">
		///         The initial offset
		/// </param>
		/// <param name="count">
		///         The length
		/// </param>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the {@code offset} and {@code count} arguments index
		///          characters outside the bounds of the {@code value} array </exception>
		public String(char[] value, int offset, int count)
		{
			if (offset < 0)
			{
				throw new StringIndexOutOfBoundsException(offset);
			}
			if (count <= 0)
			{
				if (count < 0)
				{
					throw new StringIndexOutOfBoundsException(count);
				}
				if (offset <= value.Length)
				{
					this.Value = "".Value;
					return;
				}
			}
			// Note: offset or count might be near -1>>>1.
			if (offset > value.Length - count)
			{
				throw new StringIndexOutOfBoundsException(offset + count);
			}
			this.Value = Arrays.CopyOfRange(value, offset, offset + count);
		}

		/// <summary>
		/// Allocates a new {@code String} that contains characters from a subarray
		/// of the <a href="Character.html#unicode">Unicode code point</a> array
		/// argument.  The {@code offset} argument is the index of the first code
		/// point of the subarray and the {@code count} argument specifies the
		/// length of the subarray.  The contents of the subarray are converted to
		/// {@code char}s; subsequent modification of the {@code int} array does not
		/// affect the newly created string.
		/// </summary>
		/// <param name="codePoints">
		///         Array that is the source of Unicode code points
		/// </param>
		/// <param name="offset">
		///         The initial offset
		/// </param>
		/// <param name="count">
		///         The length
		/// </param>
		/// <exception cref="IllegalArgumentException">
		///          If any invalid Unicode code point is found in {@code
		///          codePoints}
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the {@code offset} and {@code count} arguments index
		///          characters outside the bounds of the {@code codePoints} array
		/// 
		/// @since  1.5 </exception>
		public String(int[] codePoints, int offset, int count)
		{
			if (offset < 0)
			{
				throw new StringIndexOutOfBoundsException(offset);
			}
			if (count <= 0)
			{
				if (count < 0)
				{
					throw new StringIndexOutOfBoundsException(count);
				}
				if (offset <= codePoints.Length)
				{
					this.Value = "".Value;
					return;
				}
			}
			// Note: offset or count might be near -1>>>1.
			if (offset > codePoints.Length - count)
			{
				throw new StringIndexOutOfBoundsException(offset + count);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int end = offset + count;
			int end = offset + count;

			// Pass 1: Compute precise size of char[]
			int n = count;
			for (int i = offset; i < end; i++)
			{
				int c = codePoints[i];
				if (Character.IsBmpCodePoint(c))
				{
					continue;
				}
				else if (Character.IsValidCodePoint(c))
				{
					n++;
				}
				else
				{
					throw new IllegalArgumentException(Convert.ToString(c));
				}
			}

			// Pass 2: Allocate and fill in char[]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] v = new char[n];
			char[] v = new char[n];

			for (int i = offset, j = 0; i < end; i++, j++)
			{
				int c = codePoints[i];
				if (Character.IsBmpCodePoint(c))
				{
					v[j] = (char)c;
				}
				else
				{
					Character.ToSurrogates(c, v, j++);
				}
			}

			this.Value = v;
		}

		/// <summary>
		/// Allocates a new {@code String} constructed from a subarray of an array
		/// of 8-bit integer values.
		/// 
		/// <para> The {@code offset} argument is the index of the first byte of the
		/// subarray, and the {@code count} argument specifies the length of the
		/// subarray.
		/// 
		/// </para>
		/// <para> Each {@code byte} in the subarray is converted to a {@code char} as
		/// specified in the method above.
		/// 
		/// </para>
		/// </summary>
		/// @deprecated This method does not properly convert bytes into characters.
		/// As of JDK&nbsp;1.1, the preferred way to do this is via the
		/// {@code String} constructors that take a {@link
		/// java.nio.charset.Charset}, charset name, or that use the platform's
		/// default charset.
		/// 
		/// <param name="ascii">
		///         The bytes to be converted to characters
		/// </param>
		/// <param name="hibyte">
		///         The top 8 bits of each 16-bit Unicode code unit
		/// </param>
		/// <param name="offset">
		///         The initial offset </param>
		/// <param name="count">
		///         The length
		/// </param>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the {@code offset} or {@code count} argument is invalid
		/// </exception>
		/// <seealso cref=  #String(byte[], int) </seealso>
		/// <seealso cref=  #String(byte[], int, int, java.lang.String) </seealso>
		/// <seealso cref=  #String(byte[], int, int, java.nio.charset.Charset) </seealso>
		/// <seealso cref=  #String(byte[], int, int) </seealso>
		/// <seealso cref=  #String(byte[], java.lang.String) </seealso>
		/// <seealso cref=  #String(byte[], java.nio.charset.Charset) </seealso>
		/// <seealso cref=  #String(byte[]) </seealso>
		[Obsolete("This method does not properly convert bytes into characters.")]
		public String(sbyte[] ascii, int hibyte, int offset, int count)
		{
			CheckBounds(ascii, offset, count);
			char[] value = new char[count];

			if (hibyte == 0)
			{
				for (int i = count; i-- > 0;)
				{
					value[i] = (char)(ascii[i + offset] & 0xff);
				}
			}
			else
			{
				hibyte <<= 8;
				for (int i = count; i-- > 0;)
				{
					value[i] = (char)(hibyte | (ascii[i + offset] & 0xff));
				}
			}
			this.Value = value;
		}

		/// <summary>
		/// Allocates a new {@code String} containing characters constructed from
		/// an array of 8-bit integer values. Each character <i>c</i>in the
		/// resulting string is constructed from the corresponding component
		/// <i>b</i> in the byte array such that:
		/// 
		/// <blockquote><pre>
		///     <b><i>c</i></b> == (char)(((hibyte &amp; 0xff) &lt;&lt; 8)
		///                         | (<b><i>b</i></b> &amp; 0xff))
		/// </pre></blockquote>
		/// </summary>
		/// @deprecated  This method does not properly convert bytes into
		/// characters.  As of JDK&nbsp;1.1, the preferred way to do this is via the
		/// {@code String} constructors that take a {@link
		/// java.nio.charset.Charset}, charset name, or that use the platform's
		/// default charset.
		/// 
		/// <param name="ascii">
		///         The bytes to be converted to characters
		/// </param>
		/// <param name="hibyte">
		///         The top 8 bits of each 16-bit Unicode code unit
		/// </param>
		/// <seealso cref=  #String(byte[], int, int, java.lang.String) </seealso>
		/// <seealso cref=  #String(byte[], int, int, java.nio.charset.Charset) </seealso>
		/// <seealso cref=  #String(byte[], int, int) </seealso>
		/// <seealso cref=  #String(byte[], java.lang.String) </seealso>
		/// <seealso cref=  #String(byte[], java.nio.charset.Charset) </seealso>
		/// <seealso cref=  #String(byte[]) </seealso>
		[Obsolete(" This method does not properly convert bytes into")]
		public String(sbyte[] ascii, int hibyte) : this(ascii, hibyte, 0, ascii.Length)
		{
		}

		/* Common private utility method used to bounds check the byte array
		 * and requested offset & length values used by the String(byte[],..)
		 * constructors.
		 */
		private static void CheckBounds(sbyte[] bytes, int offset, int length)
		{
			if (length < 0)
			{
				throw new StringIndexOutOfBoundsException(length);
			}
			if (offset < 0)
			{
				throw new StringIndexOutOfBoundsException(offset);
			}
			if (offset > bytes.Length - length)
			{
				throw new StringIndexOutOfBoundsException(offset + length);
			}
		}

		/// <summary>
		/// Constructs a new {@code String} by decoding the specified subarray of
		/// bytes using the specified charset.  The length of the new {@code String}
		/// is a function of the charset, and hence may not be equal to the length
		/// of the subarray.
		/// 
		/// <para> The behavior of this constructor when the given bytes are not valid
		/// in the given charset is unspecified.  The {@link
		/// java.nio.charset.CharsetDecoder} class should be used when more control
		/// over the decoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes">
		///         The bytes to be decoded into characters
		/// </param>
		/// <param name="offset">
		///         The index of the first byte to decode
		/// </param>
		/// <param name="length">
		///         The number of bytes to decode
		/// </param>
		/// <param name="charsetName">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported
		/// </exception>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the {@code offset} and {@code length} arguments index
		///          characters outside the bounds of the {@code bytes} array
		/// 
		/// @since  JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String(byte bytes[] , int offset, int length, String charsetName) throws java.io.UnsupportedEncodingException
		public String(sbyte[] bytes, int offset, int length, String charsetName)
		{
			if (charsetName == null)
			{
				throw new NullPointerException("charsetName");
			}
			CheckBounds(bytes, offset, length);
			this.Value = StringCoding.Decode(charsetName, bytes, offset, length);
		}

		/// <summary>
		/// Constructs a new {@code String} by decoding the specified subarray of
		/// bytes using the specified <seealso cref="java.nio.charset.Charset charset"/>.
		/// The length of the new {@code String} is a function of the charset, and
		/// hence may not be equal to the length of the subarray.
		/// 
		/// <para> This method always replaces malformed-input and unmappable-character
		/// sequences with this charset's default replacement string.  The {@link
		/// java.nio.charset.CharsetDecoder} class should be used when more control
		/// over the decoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes">
		///         The bytes to be decoded into characters
		/// </param>
		/// <param name="offset">
		///         The index of the first byte to decode
		/// </param>
		/// <param name="length">
		///         The number of bytes to decode
		/// </param>
		/// <param name="charset">
		///         The <seealso cref="java.nio.charset.Charset charset"/> to be used to
		///         decode the {@code bytes}
		/// </param>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the {@code offset} and {@code length} arguments index
		///          characters outside the bounds of the {@code bytes} array
		/// 
		/// @since  1.6 </exception>
		public String(sbyte[] bytes, int offset, int length, Charset charset)
		{
			if (charset == null)
			{
				throw new NullPointerException("charset");
			}
			CheckBounds(bytes, offset, length);
			this.Value = StringCoding.Decode(charset, bytes, offset, length);
		}

		/// <summary>
		/// Constructs a new {@code String} by decoding the specified array of bytes
		/// using the specified <seealso cref="java.nio.charset.Charset charset"/>.  The
		/// length of the new {@code String} is a function of the charset, and hence
		/// may not be equal to the length of the byte array.
		/// 
		/// <para> The behavior of this constructor when the given bytes are not valid
		/// in the given charset is unspecified.  The {@link
		/// java.nio.charset.CharsetDecoder} class should be used when more control
		/// over the decoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes">
		///         The bytes to be decoded into characters
		/// </param>
		/// <param name="charsetName">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported
		/// 
		/// @since  JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String(byte bytes[] , String charsetName) throws java.io.UnsupportedEncodingException
		public String(sbyte[] bytes, String charsetName) : this(bytes, 0, bytes.Length, charsetName)
		{
		}

		/// <summary>
		/// Constructs a new {@code String} by decoding the specified array of
		/// bytes using the specified <seealso cref="java.nio.charset.Charset charset"/>.
		/// The length of the new {@code String} is a function of the charset, and
		/// hence may not be equal to the length of the byte array.
		/// 
		/// <para> This method always replaces malformed-input and unmappable-character
		/// sequences with this charset's default replacement string.  The {@link
		/// java.nio.charset.CharsetDecoder} class should be used when more control
		/// over the decoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes">
		///         The bytes to be decoded into characters
		/// </param>
		/// <param name="charset">
		///         The <seealso cref="java.nio.charset.Charset charset"/> to be used to
		///         decode the {@code bytes}
		/// 
		/// @since  1.6 </param>
		public String(sbyte[] bytes, Charset charset) : this(bytes, 0, bytes.Length, charset)
		{
		}

		/// <summary>
		/// Constructs a new {@code String} by decoding the specified subarray of
		/// bytes using the platform's default charset.  The length of the new
		/// {@code String} is a function of the charset, and hence may not be equal
		/// to the length of the subarray.
		/// 
		/// <para> The behavior of this constructor when the given bytes are not valid
		/// in the default charset is unspecified.  The {@link
		/// java.nio.charset.CharsetDecoder} class should be used when more control
		/// over the decoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes">
		///         The bytes to be decoded into characters
		/// </param>
		/// <param name="offset">
		///         The index of the first byte to decode
		/// </param>
		/// <param name="length">
		///         The number of bytes to decode
		/// </param>
		/// <exception cref="IndexOutOfBoundsException">
		///          If the {@code offset} and the {@code length} arguments index
		///          characters outside the bounds of the {@code bytes} array
		/// 
		/// @since  JDK1.1 </exception>
		public String(sbyte[] bytes, int offset, int length)
		{
			CheckBounds(bytes, offset, length);
			this.Value = StringCoding.Decode(bytes, offset, length);
		}

		/// <summary>
		/// Constructs a new {@code String} by decoding the specified array of bytes
		/// using the platform's default charset.  The length of the new {@code
		/// String} is a function of the charset, and hence may not be equal to the
		/// length of the byte array.
		/// 
		/// <para> The behavior of this constructor when the given bytes are not valid
		/// in the default charset is unspecified.  The {@link
		/// java.nio.charset.CharsetDecoder} class should be used when more control
		/// over the decoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="bytes">
		///         The bytes to be decoded into characters
		/// 
		/// @since  JDK1.1 </param>
		public String(sbyte[] bytes) : this(bytes, 0, bytes.Length)
		{
		}

		/// <summary>
		/// Allocates a new string that contains the sequence of characters
		/// currently contained in the string buffer argument. The contents of the
		/// string buffer are copied; subsequent modification of the string buffer
		/// does not affect the newly created string.
		/// </summary>
		/// <param name="buffer">
		///         A {@code StringBuffer} </param>
		public String(StringBuffer buffer)
		{
			lock (buffer)
			{
				this.Value = Arrays.CopyOf(buffer.Value, buffer.Length());
			}
		}

		/// <summary>
		/// Allocates a new string that contains the sequence of characters
		/// currently contained in the string builder argument. The contents of the
		/// string builder are copied; subsequent modification of the string builder
		/// does not affect the newly created string.
		/// 
		/// <para> This constructor is provided to ease migration to {@code
		/// StringBuilder}. Obtaining a string from a string builder via the {@code
		/// toString} method is likely to run faster and is generally preferred.
		/// 
		/// </para>
		/// </summary>
		/// <param name="builder">
		///          A {@code StringBuilder}
		/// 
		/// @since  1.5 </param>
		public String(StringBuilder builder)
		{
			this.Value = Arrays.CopyOf(builder.Value, builder.Length());
		}

		/*
		* Package private constructor which shares value array for speed.
		* this constructor is always expected to be called with share==true.
		* a separate constructor is needed because we already have a public
		* String(char[]) constructor that makes a copy of the given char[].
		*/
		internal String(char[] value, bool share)
		{
			// assert share : "unshared not supported";
			this.Value = value;
		}

		/// <summary>
		/// Returns the length of this string.
		/// The length is equal to the number of <a href="Character.html#unicode">Unicode
		/// code units</a> in the string.
		/// </summary>
		/// <returns>  the length of the sequence of characters represented by this
		///          object. </returns>
		public int Length()
		{
			return Value.Length;
		}

		/// <summary>
		/// Returns {@code true} if, and only if, <seealso cref="#length()"/> is {@code 0}.
		/// </summary>
		/// <returns> {@code true} if <seealso cref="#length()"/> is {@code 0}, otherwise
		/// {@code false}
		/// 
		/// @since 1.6 </returns>
		public bool Empty
		{
			get
			{
				return Value.Length == 0;
			}
		}

		/// <summary>
		/// Returns the {@code char} value at the
		/// specified index. An index ranges from {@code 0} to
		/// {@code length() - 1}. The first {@code char} value of the sequence
		/// is at index {@code 0}, the next at index {@code 1},
		/// and so on, as for array indexing.
		/// 
		/// <para>If the {@code char} value specified by the index is a
		/// <a href="Character.html#unicode">surrogate</a>, the surrogate
		/// value is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index">   the index of the {@code char} value. </param>
		/// <returns>     the {@code char} value at the specified index of this string.
		///             The first {@code char} value is at index {@code 0}. </returns>
		/// <exception cref="IndexOutOfBoundsException">  if the {@code index}
		///             argument is negative or not less than the length of this
		///             string. </exception>
		public char CharAt(int index)
		{
			if ((index < 0) || (index >= Value.Length))
			{
				throw new StringIndexOutOfBoundsException(index);
			}
			return Value[index];
		}

		/// <summary>
		/// Returns the character (Unicode code point) at the specified
		/// index. The index refers to {@code char} values
		/// (Unicode code units) and ranges from {@code 0} to
		/// <seealso cref="#length()"/>{@code  - 1}.
		/// 
		/// <para> If the {@code char} value specified at the given index
		/// is in the high-surrogate range, the following index is less
		/// than the length of this {@code String}, and the
		/// {@code char} value at the following index is in the
		/// low-surrogate range, then the supplementary code point
		/// corresponding to this surrogate pair is returned. Otherwise,
		/// the {@code char} value at the given index is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index"> the index to the {@code char} values </param>
		/// <returns>     the code point value of the character at the
		///             {@code index} </returns>
		/// <exception cref="IndexOutOfBoundsException">  if the {@code index}
		///             argument is negative or not less than the length of this
		///             string.
		/// @since      1.5 </exception>
		public int CodePointAt(int index)
		{
			if ((index < 0) || (index >= Value.Length))
			{
				throw new StringIndexOutOfBoundsException(index);
			}
			return Character.CodePointAtImpl(Value, index, Value.Length);
		}

		/// <summary>
		/// Returns the character (Unicode code point) before the specified
		/// index. The index refers to {@code char} values
		/// (Unicode code units) and ranges from {@code 1} to {@link
		/// CharSequence#length() length}.
		/// 
		/// <para> If the {@code char} value at {@code (index - 1)}
		/// is in the low-surrogate range, {@code (index - 2)} is not
		/// negative, and the {@code char} value at {@code (index -
		/// 2)} is in the high-surrogate range, then the
		/// supplementary code point value of the surrogate pair is
		/// returned. If the {@code char} value at {@code index -
		/// 1} is an unpaired low-surrogate or a high-surrogate, the
		/// surrogate value is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index"> the index following the code point that should be returned </param>
		/// <returns>    the Unicode code point value before the given index. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the {@code index}
		///            argument is less than 1 or greater than the length
		///            of this string.
		/// @since     1.5 </exception>
		public int CodePointBefore(int index)
		{
			int i = index - 1;
			if ((i < 0) || (i >= Value.Length))
			{
				throw new StringIndexOutOfBoundsException(index);
			}
			return Character.CodePointBeforeImpl(Value, index, 0);
		}

		/// <summary>
		/// Returns the number of Unicode code points in the specified text
		/// range of this {@code String}. The text range begins at the
		/// specified {@code beginIndex} and extends to the
		/// {@code char} at index {@code endIndex - 1}. Thus the
		/// length (in {@code char}s) of the text range is
		/// {@code endIndex-beginIndex}. Unpaired surrogates within
		/// the text range count as one code point each.
		/// </summary>
		/// <param name="beginIndex"> the index to the first {@code char} of
		/// the text range. </param>
		/// <param name="endIndex"> the index after the last {@code char} of
		/// the text range. </param>
		/// <returns> the number of Unicode code points in the specified text
		/// range </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the
		/// {@code beginIndex} is negative, or {@code endIndex}
		/// is larger than the length of this {@code String}, or
		/// {@code beginIndex} is larger than {@code endIndex}.
		/// @since  1.5 </exception>
		public int CodePointCount(int beginIndex, int endIndex)
		{
			if (beginIndex < 0 || endIndex > Value.Length || beginIndex > endIndex)
			{
				throw new IndexOutOfBoundsException();
			}
			return Character.CodePointCountImpl(Value, beginIndex, endIndex - beginIndex);
		}

		/// <summary>
		/// Returns the index within this {@code String} that is
		/// offset from the given {@code index} by
		/// {@code codePointOffset} code points. Unpaired surrogates
		/// within the text range given by {@code index} and
		/// {@code codePointOffset} count as one code point each.
		/// </summary>
		/// <param name="index"> the index to be offset </param>
		/// <param name="codePointOffset"> the offset in code points </param>
		/// <returns> the index within this {@code String} </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code index}
		///   is negative or larger then the length of this
		///   {@code String}, or if {@code codePointOffset} is positive
		///   and the substring starting with {@code index} has fewer
		///   than {@code codePointOffset} code points,
		///   or if {@code codePointOffset} is negative and the substring
		///   before {@code index} has fewer than the absolute value
		///   of {@code codePointOffset} code points.
		/// @since 1.5 </exception>
		public int OffsetByCodePoints(int index, int codePointOffset)
		{
			if (index < 0 || index > Value.Length)
			{
				throw new IndexOutOfBoundsException();
			}
			return Character.OffsetByCodePointsImpl(Value, 0, Value.Length, index, codePointOffset);
		}

		/// <summary>
		/// Copy characters from this string into dst starting at dstBegin.
		/// This method doesn't perform any range checking.
		/// </summary>
		internal void GetChars(char[] dst, int dstBegin)
		{
			System.Array.Copy(Value, 0, dst, dstBegin, Value.Length);
		}

		/// <summary>
		/// Copies characters from this string into the destination character
		/// array.
		/// <para>
		/// The first character to be copied is at index {@code srcBegin};
		/// the last character to be copied is at index {@code srcEnd-1}
		/// (thus the total number of characters to be copied is
		/// {@code srcEnd-srcBegin}). The characters are copied into the
		/// subarray of {@code dst} starting at index {@code dstBegin}
		/// and ending at index:
		/// <blockquote><pre>
		///     dstBegin + (srcEnd-srcBegin) - 1
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="srcBegin">   index of the first character in the string
		///                        to copy. </param>
		/// <param name="srcEnd">     index after the last character in the string
		///                        to copy. </param>
		/// <param name="dst">        the destination array. </param>
		/// <param name="dstBegin">   the start offset in the destination array. </param>
		/// <exception cref="IndexOutOfBoundsException"> If any of the following
		///            is true:
		///            <ul><li>{@code srcBegin} is negative.
		///            <li>{@code srcBegin} is greater than {@code srcEnd}
		///            <li>{@code srcEnd} is greater than the length of this
		///                string
		///            <li>{@code dstBegin} is negative
		///            <li>{@code dstBegin+(srcEnd-srcBegin)} is larger than
		///                {@code dst.length}</ul> </exception>
		public void GetChars(int srcBegin, int srcEnd, char[] dst, int dstBegin)
		{
			if (srcBegin < 0)
			{
				throw new StringIndexOutOfBoundsException(srcBegin);
			}
			if (srcEnd > Value.Length)
			{
				throw new StringIndexOutOfBoundsException(srcEnd);
			}
			if (srcBegin > srcEnd)
			{
				throw new StringIndexOutOfBoundsException(srcEnd - srcBegin);
			}
			System.Array.Copy(Value, srcBegin, dst, dstBegin, srcEnd - srcBegin);
		}

		/// <summary>
		/// Copies characters from this string into the destination byte array. Each
		/// byte receives the 8 low-order bits of the corresponding character. The
		/// eight high-order bits of each character are not copied and do not
		/// participate in the transfer in any way.
		/// 
		/// <para> The first character to be copied is at index {@code srcBegin}; the
		/// last character to be copied is at index {@code srcEnd-1}.  The total
		/// number of characters to be copied is {@code srcEnd-srcBegin}. The
		/// characters, converted to bytes, are copied into the subarray of {@code
		/// dst} starting at index {@code dstBegin} and ending at index:
		/// 
		/// <blockquote><pre>
		///     dstBegin + (srcEnd-srcBegin) - 1
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// @deprecated  This method does not properly convert characters into
		/// bytes.  As of JDK&nbsp;1.1, the preferred way to do this is via the
		/// <seealso cref="#getBytes()"/> method, which uses the platform's default charset.
		/// 
		/// <param name="srcBegin">
		///         Index of the first character in the string to copy
		/// </param>
		/// <param name="srcEnd">
		///         Index after the last character in the string to copy
		/// </param>
		/// <param name="dst">
		///         The destination array
		/// </param>
		/// <param name="dstBegin">
		///         The start offset in the destination array
		/// </param>
		/// <exception cref="IndexOutOfBoundsException">
		///          If any of the following is true:
		///          <ul>
		///            <li> {@code srcBegin} is negative
		///            <li> {@code srcBegin} is greater than {@code srcEnd}
		///            <li> {@code srcEnd} is greater than the length of this String
		///            <li> {@code dstBegin} is negative
		///            <li> {@code dstBegin+(srcEnd-srcBegin)} is larger than {@code
		///                 dst.length}
		///          </ul> </exception>
		[Obsolete(" This method does not properly convert characters into")]
		public void GetBytes(int srcBegin, int srcEnd, sbyte[] dst, int dstBegin)
		{
			if (srcBegin < 0)
			{
				throw new StringIndexOutOfBoundsException(srcBegin);
			}
			if (srcEnd > Value.Length)
			{
				throw new StringIndexOutOfBoundsException(srcEnd);
			}
			if (srcBegin > srcEnd)
			{
				throw new StringIndexOutOfBoundsException(srcEnd - srcBegin);
			}
			Objects.RequireNonNull(dst);

			int j = dstBegin;
			int n = srcEnd;
			int i = srcBegin;
			char[] val = Value; // avoid getfield opcode

			while (i < n)
			{
				dst[j++] = (sbyte)val[i++];
			}
		}

		/// <summary>
		/// Encodes this {@code String} into a sequence of bytes using the named
		/// charset, storing the result into a new byte array.
		/// 
		/// <para> The behavior of this method when this string cannot be encoded in
		/// the given charset is unspecified.  The {@link
		/// java.nio.charset.CharsetEncoder} class should be used when more control
		/// over the encoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="charsetName">
		///         The name of a supported {@link java.nio.charset.Charset
		///         charset}
		/// </param>
		/// <returns>  The resultant byte array
		/// </returns>
		/// <exception cref="UnsupportedEncodingException">
		///          If the named charset is not supported
		/// 
		/// @since  JDK1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getBytes(String charsetName) throws java.io.UnsupportedEncodingException
		public sbyte[] GetBytes(String charsetName)
		{
			if (charsetName == null)
			{
				throw new NullPointerException();
			}
			return StringCoding.Encode(charsetName, Value, 0, Value.Length);
		}

		/// <summary>
		/// Encodes this {@code String} into a sequence of bytes using the given
		/// <seealso cref="java.nio.charset.Charset charset"/>, storing the result into a
		/// new byte array.
		/// 
		/// <para> This method always replaces malformed-input and unmappable-character
		/// sequences with this charset's default replacement byte array.  The
		/// <seealso cref="java.nio.charset.CharsetEncoder"/> class should be used when more
		/// control over the encoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <param name="charset">
		///         The <seealso cref="java.nio.charset.Charset"/> to be used to encode
		///         the {@code String}
		/// </param>
		/// <returns>  The resultant byte array
		/// 
		/// @since  1.6 </returns>
		public sbyte[] GetBytes(Charset charset)
		{
			if (charset == null)
			{
				throw new NullPointerException();
			}
			return StringCoding.Encode(charset, Value, 0, Value.Length);
		}

		/// <summary>
		/// Encodes this {@code String} into a sequence of bytes using the
		/// platform's default charset, storing the result into a new byte array.
		/// 
		/// <para> The behavior of this method when this string cannot be encoded in
		/// the default charset is unspecified.  The {@link
		/// java.nio.charset.CharsetEncoder} class should be used when more control
		/// over the encoding process is required.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  The resultant byte array
		/// 
		/// @since      JDK1.1 </returns>
		public sbyte[] Bytes
		{
			get
			{
				return StringCoding.Encode(Value, 0, Value.Length);
			}
		}

		/// <summary>
		/// Compares this string to the specified object.  The result is {@code
		/// true} if and only if the argument is not {@code null} and is a {@code
		/// String} object that represents the same sequence of characters as this
		/// object.
		/// </summary>
		/// <param name="anObject">
		///         The object to compare this {@code String} against
		/// </param>
		/// <returns>  {@code true} if the given object represents a {@code String}
		///          equivalent to this string, {@code false} otherwise
		/// </returns>
		/// <seealso cref=  #compareTo(String) </seealso>
		/// <seealso cref=  #equalsIgnoreCase(String) </seealso>
		public override bool Equals(Object anObject)
		{
			if (this == anObject)
			{
				return true;
			}
			if (anObject is String)
			{
				String anotherString = (String)anObject;
				int n = Value.Length;
				if (n == anotherString.Value.Length)
				{
					char[] v1 = Value;
					char[] v2 = anotherString.Value;
					int i = 0;
					while (n-- != 0)
					{
						if (v1[i] != v2[i])
						{
							return false;
						}
						i++;
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Compares this string to the specified {@code StringBuffer}.  The result
		/// is {@code true} if and only if this {@code String} represents the same
		/// sequence of characters as the specified {@code StringBuffer}. This method
		/// synchronizes on the {@code StringBuffer}.
		/// </summary>
		/// <param name="sb">
		///         The {@code StringBuffer} to compare this {@code String} against
		/// </param>
		/// <returns>  {@code true} if this {@code String} represents the same
		///          sequence of characters as the specified {@code StringBuffer},
		///          {@code false} otherwise
		/// 
		/// @since  1.4 </returns>
		public bool ContentEquals(StringBuffer sb)
		{
			return ContentEquals((CharSequence)sb);
		}

		private bool NonSyncContentEquals(AbstractStringBuilder sb)
		{
			char[] v1 = Value;
			char[] v2 = sb.Value;
			int n = v1.Length;
			if (n != sb.Length())
			{
				return false;
			}
			for (int i = 0; i < n; i++)
			{
				if (v1[i] != v2[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Compares this string to the specified {@code CharSequence}.  The
		/// result is {@code true} if and only if this {@code String} represents the
		/// same sequence of char values as the specified sequence. Note that if the
		/// {@code CharSequence} is a {@code StringBuffer} then the method
		/// synchronizes on it.
		/// </summary>
		/// <param name="cs">
		///         The sequence to compare this {@code String} against
		/// </param>
		/// <returns>  {@code true} if this {@code String} represents the same
		///          sequence of char values as the specified sequence, {@code
		///          false} otherwise
		/// 
		/// @since  1.5 </returns>
		public bool ContentEquals(CharSequence cs)
		{
			// Argument is a StringBuffer, StringBuilder
			if (cs is AbstractStringBuilder)
			{
				if (cs is StringBuffer)
				{
					lock (cs)
					{
					   return NonSyncContentEquals((AbstractStringBuilder)cs);
					}
				}
				else
				{
					return NonSyncContentEquals((AbstractStringBuilder)cs);
				}
			}
			// Argument is a String
			if (cs is String)
			{
				return Equals(cs);
			}
			// Argument is a generic CharSequence
			char[] v1 = Value;
			int n = v1.Length;
			if (n != cs.Length())
			{
				return false;
			}
			for (int i = 0; i < n; i++)
			{
				if (v1[i] != cs.CharAt(i))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Compares this {@code String} to another {@code String}, ignoring case
		/// considerations.  Two strings are considered equal ignoring case if they
		/// are of the same length and corresponding characters in the two strings
		/// are equal ignoring case.
		/// 
		/// <para> Two characters {@code c1} and {@code c2} are considered the same
		/// ignoring case if at least one of the following is true:
		/// <ul>
		///   <li> The two characters are the same (as compared by the
		///        {@code ==} operator)
		///   <li> Applying the method {@link
		///        java.lang.Character#toUpperCase(char)} to each character
		///        produces the same result
		///   <li> Applying the method {@link
		///        java.lang.Character#toLowerCase(char)} to each character
		///        produces the same result
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="anotherString">
		///         The {@code String} to compare this {@code String} against
		/// </param>
		/// <returns>  {@code true} if the argument is not {@code null} and it
		///          represents an equivalent {@code String} ignoring case; {@code
		///          false} otherwise
		/// </returns>
		/// <seealso cref=  #equals(Object) </seealso>
		public bool EqualsIgnoreCase(String anotherString)
		{
			return (this == anotherString) ? true : (anotherString != null) && (anotherString.Value.Length == Value.Length) && RegionMatches(true, 0, anotherString, 0, Value.Length);
		}

		/// <summary>
		/// Compares two strings lexicographically.
		/// The comparison is based on the Unicode value of each character in
		/// the strings. The character sequence represented by this
		/// {@code String} object is compared lexicographically to the
		/// character sequence represented by the argument string. The result is
		/// a negative integer if this {@code String} object
		/// lexicographically precedes the argument string. The result is a
		/// positive integer if this {@code String} object lexicographically
		/// follows the argument string. The result is zero if the strings
		/// are equal; {@code compareTo} returns {@code 0} exactly when
		/// the <seealso cref="#equals(Object)"/> method would return {@code true}.
		/// <para>
		/// This is the definition of lexicographic ordering. If two strings are
		/// different, then either they have different characters at some index
		/// that is a valid index for both strings, or their lengths are different,
		/// or both. If they have different characters at one or more index
		/// positions, let <i>k</i> be the smallest such index; then the string
		/// whose character at position <i>k</i> has the smaller value, as
		/// determined by using the &lt; operator, lexicographically precedes the
		/// other string. In this case, {@code compareTo} returns the
		/// difference of the two character values at position {@code k} in
		/// the two string -- that is, the value:
		/// <blockquote><pre>
		/// this.charAt(k)-anotherString.charAt(k)
		/// </pre></blockquote>
		/// If there is no index position at which they differ, then the shorter
		/// string lexicographically precedes the longer string. In this case,
		/// {@code compareTo} returns the difference of the lengths of the
		/// strings -- that is, the value:
		/// <blockquote><pre>
		/// this.length()-anotherString.length()
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="anotherString">   the {@code String} to be compared. </param>
		/// <returns>  the value {@code 0} if the argument string is equal to
		///          this string; a value less than {@code 0} if this string
		///          is lexicographically less than the string argument; and a
		///          value greater than {@code 0} if this string is
		///          lexicographically greater than the string argument. </returns>
		public int CompareTo(String anotherString)
		{
			int len1 = Value.Length;
			int len2 = anotherString.Value.Length;
			int lim = System.Math.Min(len1, len2);
			char[] v1 = Value;
			char[] v2 = anotherString.Value;

			int k = 0;
			while (k < lim)
			{
				char c1 = v1[k];
				char c2 = v2[k];
				if (c1 != c2)
				{
					return c1 - c2;
				}
				k++;
			}
			return len1 - len2;
		}

		/// <summary>
		/// A Comparator that orders {@code String} objects as by
		/// {@code compareToIgnoreCase}. This comparator is serializable.
		/// <para>
		/// Note that this Comparator does <em>not</em> take locale into account,
		/// and will result in an unsatisfactory ordering for certain locales.
		/// The java.text package provides <em>Collators</em> to allow
		/// locale-sensitive ordering.
		/// 
		/// </para>
		/// </summary>
		/// <seealso cref=     java.text.Collator#compare(String, String)
		/// @since   1.2 </seealso>
		public static readonly IComparer<String> CASE_INSENSITIVE_ORDER = new CaseInsensitiveComparator();
		[Serializable]
		private class CaseInsensitiveComparator : Comparator<String>
		{
			// use serialVersionUID from JDK 1.2.2 for interoperability
			internal const long SerialVersionUID = 8575799808933029326L;

			public virtual int Compare(String s1, String s2)
			{
				int n1 = s1.Length();
				int n2 = s2.Length();
				int min = System.Math.Min(n1, n2);
				for (int i = 0; i < min; i++)
				{
					char c1 = s1.CharAt(i);
					char c2 = s2.CharAt(i);
					if (c1 != c2)
					{
						c1 = char.ToUpper(c1);
						c2 = char.ToUpper(c2);
						if (c1 != c2)
						{
							c1 = char.ToLower(c1);
							c2 = char.ToLower(c2);
							if (c1 != c2)
							{
								// No overflow because of numeric promotion
								return c1 - c2;
							}
						}
					}
				}
				return n1 - n2;
			}

			/// <summary>
			/// Replaces the de-serialized object. </summary>
			internal virtual Object ReadResolve()
			{
				return CASE_INSENSITIVE_ORDER;
			}
		}

		/// <summary>
		/// Compares two strings lexicographically, ignoring case
		/// differences. This method returns an integer whose sign is that of
		/// calling {@code compareTo} with normalized versions of the strings
		/// where case differences have been eliminated by calling
		/// {@code Character.toLowerCase(Character.toUpperCase(character))} on
		/// each character.
		/// <para>
		/// Note that this method does <em>not</em> take locale into account,
		/// and will result in an unsatisfactory ordering for certain locales.
		/// The java.text package provides <em>collators</em> to allow
		/// locale-sensitive ordering.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">   the {@code String} to be compared. </param>
		/// <returns>  a negative integer, zero, or a positive integer as the
		///          specified String is greater than, equal to, or less
		///          than this String, ignoring case considerations. </returns>
		/// <seealso cref=     java.text.Collator#compare(String, String)
		/// @since   1.2 </seealso>
		public int CompareToIgnoreCase(String str)
		{
			return CASE_INSENSITIVE_ORDER.Compare(this, str);
		}

		/// <summary>
		/// Tests if two string regions are equal.
		/// <para>
		/// A substring of this {@code String} object is compared to a substring
		/// of the argument other. The result is true if these substrings
		/// represent identical character sequences. The substring of this
		/// {@code String} object to be compared begins at index {@code toffset}
		/// and has length {@code len}. The substring of other to be compared
		/// begins at index {@code ooffset} and has length {@code len}. The
		/// result is {@code false} if and only if at least one of the following
		/// is true:
		/// <ul><li>{@code toffset} is negative.
		/// <li>{@code ooffset} is negative.
		/// <li>{@code toffset+len} is greater than the length of this
		/// {@code String} object.
		/// <li>{@code ooffset+len} is greater than the length of the other
		/// argument.
		/// <li>There is some nonnegative integer <i>k</i> less than {@code len}
		/// such that:
		/// {@code this.charAt(toffset + }<i>k</i>{@code ) != other.charAt(ooffset + }
		/// <i>k</i>{@code )}
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="toffset">   the starting offset of the subregion in this string. </param>
		/// <param name="other">     the string argument. </param>
		/// <param name="ooffset">   the starting offset of the subregion in the string
		///                    argument. </param>
		/// <param name="len">       the number of characters to compare. </param>
		/// <returns>  {@code true} if the specified subregion of this string
		///          exactly matches the specified subregion of the string argument;
		///          {@code false} otherwise. </returns>
		public bool RegionMatches(int toffset, String other, int ooffset, int len)
		{
			char[] ta = Value;
			int to = toffset;
			char[] pa = other.Value;
			int po = ooffset;
			// Note: toffset, ooffset, or len might be near -1>>>1.
			if ((ooffset < 0) || (toffset < 0) || (toffset > (long)Value.Length - len) || (ooffset > (long)other.Value.Length - len))
			{
				return false;
			}
			while (len-- > 0)
			{
				if (ta[to++] != pa[po++])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Tests if two string regions are equal.
		/// <para>
		/// A substring of this {@code String} object is compared to a substring
		/// of the argument {@code other}. The result is {@code true} if these
		/// substrings represent character sequences that are the same, ignoring
		/// case if and only if {@code ignoreCase} is true. The substring of
		/// this {@code String} object to be compared begins at index
		/// {@code toffset} and has length {@code len}. The substring of
		/// {@code other} to be compared begins at index {@code ooffset} and
		/// has length {@code len}. The result is {@code false} if and only if
		/// at least one of the following is true:
		/// <ul><li>{@code toffset} is negative.
		/// <li>{@code ooffset} is negative.
		/// <li>{@code toffset+len} is greater than the length of this
		/// {@code String} object.
		/// <li>{@code ooffset+len} is greater than the length of the other
		/// argument.
		/// <li>{@code ignoreCase} is {@code false} and there is some nonnegative
		/// integer <i>k</i> less than {@code len} such that:
		/// <blockquote><pre>
		/// this.charAt(toffset+k) != other.charAt(ooffset+k)
		/// </pre></blockquote>
		/// <li>{@code ignoreCase} is {@code true} and there is some nonnegative
		/// integer <i>k</i> less than {@code len} such that:
		/// <blockquote><pre>
		/// Character.toLowerCase(this.charAt(toffset+k)) !=
		/// Character.toLowerCase(other.charAt(ooffset+k))
		/// </pre></blockquote>
		/// and:
		/// <blockquote><pre>
		/// Character.toUpperCase(this.charAt(toffset+k)) !=
		///         Character.toUpperCase(other.charAt(ooffset+k))
		/// </pre></blockquote>
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="ignoreCase">   if {@code true}, ignore case when comparing
		///                       characters. </param>
		/// <param name="toffset">      the starting offset of the subregion in this
		///                       string. </param>
		/// <param name="other">        the string argument. </param>
		/// <param name="ooffset">      the starting offset of the subregion in the string
		///                       argument. </param>
		/// <param name="len">          the number of characters to compare. </param>
		/// <returns>  {@code true} if the specified subregion of this string
		///          matches the specified subregion of the string argument;
		///          {@code false} otherwise. Whether the matching is exact
		///          or case insensitive depends on the {@code ignoreCase}
		///          argument. </returns>
		public bool RegionMatches(bool ignoreCase, int toffset, String other, int ooffset, int len)
		{
			char[] ta = Value;
			int to = toffset;
			char[] pa = other.Value;
			int po = ooffset;
			// Note: toffset, ooffset, or len might be near -1>>>1.
			if ((ooffset < 0) || (toffset < 0) || (toffset > (long)Value.Length - len) || (ooffset > (long)other.Value.Length - len))
			{
				return false;
			}
			while (len-- > 0)
			{
				char c1 = ta[to++];
				char c2 = pa[po++];
				if (c1 == c2)
				{
					continue;
				}
				if (ignoreCase)
				{
					// If characters don't match but case may be ignored,
					// try converting both characters to uppercase.
					// If the results match, then the comparison scan should
					// continue.
					char u1 = char.ToUpper(c1);
					char u2 = char.ToUpper(c2);
					if (u1 == u2)
					{
						continue;
					}
					// Unfortunately, conversion to uppercase does not work properly
					// for the Georgian alphabet, which has strange rules about case
					// conversion.  So we need to make one last check before
					// exiting.
					if (char.ToLower(u1) == char.ToLower(u2))
					{
						continue;
					}
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Tests if the substring of this string beginning at the
		/// specified index starts with the specified prefix.
		/// </summary>
		/// <param name="prefix">    the prefix. </param>
		/// <param name="toffset">   where to begin looking in this string. </param>
		/// <returns>  {@code true} if the character sequence represented by the
		///          argument is a prefix of the substring of this object starting
		///          at index {@code toffset}; {@code false} otherwise.
		///          The result is {@code false} if {@code toffset} is
		///          negative or greater than the length of this
		///          {@code String} object; otherwise the result is the same
		///          as the result of the expression
		///          <pre>
		///          this.substring(toffset).startsWith(prefix)
		///          </pre> </returns>
		public bool StartsWith(String prefix, int toffset)
		{
			char[] ta = Value;
			int to = toffset;
			char[] pa = prefix.Value;
			int po = 0;
			int pc = prefix.Value.Length;
			// Note: toffset might be near -1>>>1.
			if ((toffset < 0) || (toffset > Value.Length - pc))
			{
				return false;
			}
			while (--pc >= 0)
			{
				if (ta[to++] != pa[po++])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Tests if this string starts with the specified prefix.
		/// </summary>
		/// <param name="prefix">   the prefix. </param>
		/// <returns>  {@code true} if the character sequence represented by the
		///          argument is a prefix of the character sequence represented by
		///          this string; {@code false} otherwise.
		///          Note also that {@code true} will be returned if the
		///          argument is an empty string or is equal to this
		///          {@code String} object as determined by the
		///          <seealso cref="#equals(Object)"/> method.
		/// @since   1. 0 </returns>
		public bool StartsWith(String prefix)
		{
			return StartsWith(prefix, 0);
		}

		/// <summary>
		/// Tests if this string ends with the specified suffix.
		/// </summary>
		/// <param name="suffix">   the suffix. </param>
		/// <returns>  {@code true} if the character sequence represented by the
		///          argument is a suffix of the character sequence represented by
		///          this object; {@code false} otherwise. Note that the
		///          result will be {@code true} if the argument is the
		///          empty string or is equal to this {@code String} object
		///          as determined by the <seealso cref="#equals(Object)"/> method. </returns>
		public bool EndsWith(String suffix)
		{
			return StartsWith(suffix, Value.Length - suffix.Value.Length);
		}

		/// <summary>
		/// Returns a hash code for this string. The hash code for a
		/// {@code String} object is computed as
		/// <blockquote><pre>
		/// s[0]*31^(n-1) + s[1]*31^(n-2) + ... + s[n-1]
		/// </pre></blockquote>
		/// using {@code int} arithmetic, where {@code s[i]} is the
		/// <i>i</i>th character of the string, {@code n} is the length of
		/// the string, and {@code ^} indicates exponentiation.
		/// (The hash value of the empty string is zero.)
		/// </summary>
		/// <returns>  a hash code value for this object. </returns>
		public override int HashCode()
		{
			int h = Hash;
			if (h == 0 && Value.Length > 0)
			{
				char[] val = Value;

				for (int i = 0; i < Value.Length; i++)
				{
					h = 31 * h + val[i];
				}
				Hash = h;
			}
			return h;
		}

		/// <summary>
		/// Returns the index within this string of the first occurrence of
		/// the specified character. If a character with value
		/// {@code ch} occurs in the character sequence represented by
		/// this {@code String} object, then the index (in Unicode
		/// code units) of the first such occurrence is returned. For
		/// values of {@code ch} in the range from 0 to 0xFFFF
		/// (inclusive), this is the smallest value <i>k</i> such that:
		/// <blockquote><pre>
		/// this.charAt(<i>k</i>) == ch
		/// </pre></blockquote>
		/// is true. For other values of {@code ch}, it is the
		/// smallest value <i>k</i> such that:
		/// <blockquote><pre>
		/// this.codePointAt(<i>k</i>) == ch
		/// </pre></blockquote>
		/// is true. In either case, if no such character occurs in this
		/// string, then {@code -1} is returned.
		/// </summary>
		/// <param name="ch">   a character (Unicode code point). </param>
		/// <returns>  the index of the first occurrence of the character in the
		///          character sequence represented by this object, or
		///          {@code -1} if the character does not occur. </returns>
		public int IndexOf(int ch)
		{
			return IndexOf(ch, 0);
		}

		/// <summary>
		/// Returns the index within this string of the first occurrence of the
		/// specified character, starting the search at the specified index.
		/// <para>
		/// If a character with value {@code ch} occurs in the
		/// character sequence represented by this {@code String}
		/// object at an index no smaller than {@code fromIndex}, then
		/// the index of the first such occurrence is returned. For values
		/// of {@code ch} in the range from 0 to 0xFFFF (inclusive),
		/// this is the smallest value <i>k</i> such that:
		/// <blockquote><pre>
		/// (this.charAt(<i>k</i>) == ch) {@code &&} (<i>k</i> &gt;= fromIndex)
		/// </pre></blockquote>
		/// is true. For other values of {@code ch}, it is the
		/// smallest value <i>k</i> such that:
		/// <blockquote><pre>
		/// (this.codePointAt(<i>k</i>) == ch) {@code &&} (<i>k</i> &gt;= fromIndex)
		/// </pre></blockquote>
		/// is true. In either case, if no such character occurs in this
		/// string at or after position {@code fromIndex}, then
		/// {@code -1} is returned.
		/// 
		/// </para>
		/// <para>
		/// There is no restriction on the value of {@code fromIndex}. If it
		/// is negative, it has the same effect as if it were zero: this entire
		/// string may be searched. If it is greater than the length of this
		/// string, it has the same effect as if it were equal to the length of
		/// this string: {@code -1} is returned.
		/// 
		/// </para>
		/// <para>All indices are specified in {@code char} values
		/// (Unicode code units).
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">          a character (Unicode code point). </param>
		/// <param name="fromIndex">   the index to start the search from. </param>
		/// <returns>  the index of the first occurrence of the character in the
		///          character sequence represented by this object that is greater
		///          than or equal to {@code fromIndex}, or {@code -1}
		///          if the character does not occur. </returns>
		public int IndexOf(int ch, int fromIndex)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int max = value.length;
			int max = Value.Length;
			if (fromIndex < 0)
			{
				fromIndex = 0;
			}
			else if (fromIndex >= max)
			{
				// Note: fromIndex might be near -1>>>1.
				return -1;
			}

			if (ch < Character.MIN_SUPPLEMENTARY_CODE_POINT)
			{
				// handle most cases here (ch is a BMP code point or a
				// negative value (invalid code point))
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] value = this.value;
				char[] value = this.Value;
				for (int i = fromIndex; i < max; i++)
				{
					if (value[i] == ch)
					{
						return i;
					}
				}
				return -1;
			}
			else
			{
				return IndexOfSupplementary(ch, fromIndex);
			}
		}

		/// <summary>
		/// Handles (rare) calls of indexOf with a supplementary character.
		/// </summary>
		private int IndexOfSupplementary(int ch, int fromIndex)
		{
			if (Character.IsValidCodePoint(ch))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] value = this.value;
				char[] value = this.Value;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char hi = Character.highSurrogate(ch);
				char hi = Character.HighSurrogate(ch);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char lo = Character.lowSurrogate(ch);
				char lo = Character.LowSurrogate(ch);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int max = value.length - 1;
				int max = value.Length - 1;
				for (int i = fromIndex; i < max; i++)
				{
					if (value[i] == hi && value[i + 1] == lo)
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns the index within this string of the last occurrence of
		/// the specified character. For values of {@code ch} in the
		/// range from 0 to 0xFFFF (inclusive), the index (in Unicode code
		/// units) returned is the largest value <i>k</i> such that:
		/// <blockquote><pre>
		/// this.charAt(<i>k</i>) == ch
		/// </pre></blockquote>
		/// is true. For other values of {@code ch}, it is the
		/// largest value <i>k</i> such that:
		/// <blockquote><pre>
		/// this.codePointAt(<i>k</i>) == ch
		/// </pre></blockquote>
		/// is true.  In either case, if no such character occurs in this
		/// string, then {@code -1} is returned.  The
		/// {@code String} is searched backwards starting at the last
		/// character.
		/// </summary>
		/// <param name="ch">   a character (Unicode code point). </param>
		/// <returns>  the index of the last occurrence of the character in the
		///          character sequence represented by this object, or
		///          {@code -1} if the character does not occur. </returns>
		public int LastIndexOf(int ch)
		{
			return LastIndexOf(ch, Value.Length - 1);
		}

		/// <summary>
		/// Returns the index within this string of the last occurrence of
		/// the specified character, searching backward starting at the
		/// specified index. For values of {@code ch} in the range
		/// from 0 to 0xFFFF (inclusive), the index returned is the largest
		/// value <i>k</i> such that:
		/// <blockquote><pre>
		/// (this.charAt(<i>k</i>) == ch) {@code &&} (<i>k</i> &lt;= fromIndex)
		/// </pre></blockquote>
		/// is true. For other values of {@code ch}, it is the
		/// largest value <i>k</i> such that:
		/// <blockquote><pre>
		/// (this.codePointAt(<i>k</i>) == ch) {@code &&} (<i>k</i> &lt;= fromIndex)
		/// </pre></blockquote>
		/// is true. In either case, if no such character occurs in this
		/// string at or before position {@code fromIndex}, then
		/// {@code -1} is returned.
		/// 
		/// <para>All indices are specified in {@code char} values
		/// (Unicode code units).
		/// 
		/// </para>
		/// </summary>
		/// <param name="ch">          a character (Unicode code point). </param>
		/// <param name="fromIndex">   the index to start the search from. There is no
		///          restriction on the value of {@code fromIndex}. If it is
		///          greater than or equal to the length of this string, it has
		///          the same effect as if it were equal to one less than the
		///          length of this string: this entire string may be searched.
		///          If it is negative, it has the same effect as if it were -1:
		///          -1 is returned. </param>
		/// <returns>  the index of the last occurrence of the character in the
		///          character sequence represented by this object that is less
		///          than or equal to {@code fromIndex}, or {@code -1}
		///          if the character does not occur before that point. </returns>
		public int LastIndexOf(int ch, int fromIndex)
		{
			if (ch < Character.MIN_SUPPLEMENTARY_CODE_POINT)
			{
				// handle most cases here (ch is a BMP code point or a
				// negative value (invalid code point))
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] value = this.value;
				char[] value = this.Value;
				int i = System.Math.Min(fromIndex, value.Length - 1);
				for (; i >= 0; i--)
				{
					if (value[i] == ch)
					{
						return i;
					}
				}
				return -1;
			}
			else
			{
				return LastIndexOfSupplementary(ch, fromIndex);
			}
		}

		/// <summary>
		/// Handles (rare) calls of lastIndexOf with a supplementary character.
		/// </summary>
		private int LastIndexOfSupplementary(int ch, int fromIndex)
		{
			if (Character.IsValidCodePoint(ch))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] value = this.value;
				char[] value = this.Value;
				char hi = Character.HighSurrogate(ch);
				char lo = Character.LowSurrogate(ch);
				int i = System.Math.Min(fromIndex, value.Length - 2);
				for (; i >= 0; i--)
				{
					if (value[i] == hi && value[i + 1] == lo)
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns the index within this string of the first occurrence of the
		/// specified substring.
		/// 
		/// <para>The returned index is the smallest value <i>k</i> for which:
		/// <blockquote><pre>
		/// this.startsWith(str, <i>k</i>)
		/// </pre></blockquote>
		/// If no such value of <i>k</i> exists, then {@code -1} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">   the substring to search for. </param>
		/// <returns>  the index of the first occurrence of the specified substring,
		///          or {@code -1} if there is no such occurrence. </returns>
		public int IndexOf(String str)
		{
			return IndexOf(str, 0);
		}

		/// <summary>
		/// Returns the index within this string of the first occurrence of the
		/// specified substring, starting at the specified index.
		/// 
		/// <para>The returned index is the smallest value <i>k</i> for which:
		/// <blockquote><pre>
		/// <i>k</i> &gt;= fromIndex {@code &&} this.startsWith(str, <i>k</i>)
		/// </pre></blockquote>
		/// If no such value of <i>k</i> exists, then {@code -1} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">         the substring to search for. </param>
		/// <param name="fromIndex">   the index from which to start the search. </param>
		/// <returns>  the index of the first occurrence of the specified substring,
		///          starting at the specified index,
		///          or {@code -1} if there is no such occurrence. </returns>
		public int IndexOf(String str, int fromIndex)
		{
			return IndexOf(Value, 0, Value.Length, str.Value, 0, str.Value.Length, fromIndex);
		}

		/// <summary>
		/// Code shared by String and AbstractStringBuilder to do searches. The
		/// source is the character array being searched, and the target
		/// is the string being searched for.
		/// </summary>
		/// <param name="source">       the characters being searched. </param>
		/// <param name="sourceOffset"> offset of the source string. </param>
		/// <param name="sourceCount">  count of the source string. </param>
		/// <param name="target">       the characters being searched for. </param>
		/// <param name="fromIndex">    the index to begin searching from. </param>
		internal static int IndexOf(char[] source, int sourceOffset, int sourceCount, String target, int fromIndex)
		{
			return IndexOf(source, sourceOffset, sourceCount, target.Value, 0, target.Value.Length, fromIndex);
		}

		/// <summary>
		/// Code shared by String and StringBuffer to do searches. The
		/// source is the character array being searched, and the target
		/// is the string being searched for.
		/// </summary>
		/// <param name="source">       the characters being searched. </param>
		/// <param name="sourceOffset"> offset of the source string. </param>
		/// <param name="sourceCount">  count of the source string. </param>
		/// <param name="target">       the characters being searched for. </param>
		/// <param name="targetOffset"> offset of the target string. </param>
		/// <param name="targetCount">  count of the target string. </param>
		/// <param name="fromIndex">    the index to begin searching from. </param>
		internal static int IndexOf(char[] source, int sourceOffset, int sourceCount, char[] target, int targetOffset, int targetCount, int fromIndex)
		{
			if (fromIndex >= sourceCount)
			{
				return (targetCount == 0 ? sourceCount : -1);
			}
			if (fromIndex < 0)
			{
				fromIndex = 0;
			}
			if (targetCount == 0)
			{
				return fromIndex;
			}

			char first = target[targetOffset];
			int max = sourceOffset + (sourceCount - targetCount);

			for (int i = sourceOffset + fromIndex; i <= max; i++)
			{
				/* Look for first character. */
				if (source[i] != first)
				{
					while (++i <= max && source[i] != first);
				}

				/* Found first character, now look at the rest of v2 */
				if (i <= max)
				{
					int j = i + 1;
					int end = j + targetCount - 1;
					for (int k = targetOffset + 1; j < end && source[j] == target[k]; j++, k++)
					{
						;
					}

					if (j == end)
					{
						/* Found whole string. */
						return i - sourceOffset;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns the index within this string of the last occurrence of the
		/// specified substring.  The last occurrence of the empty string ""
		/// is considered to occur at the index value {@code this.length()}.
		/// 
		/// <para>The returned index is the largest value <i>k</i> for which:
		/// <blockquote><pre>
		/// this.startsWith(str, <i>k</i>)
		/// </pre></blockquote>
		/// If no such value of <i>k</i> exists, then {@code -1} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">   the substring to search for. </param>
		/// <returns>  the index of the last occurrence of the specified substring,
		///          or {@code -1} if there is no such occurrence. </returns>
		public int LastIndexOf(String str)
		{
			return LastIndexOf(str, Value.Length);
		}

		/// <summary>
		/// Returns the index within this string of the last occurrence of the
		/// specified substring, searching backward starting at the specified index.
		/// 
		/// <para>The returned index is the largest value <i>k</i> for which:
		/// <blockquote><pre>
		/// <i>k</i> {@code <=} fromIndex {@code &&} this.startsWith(str, <i>k</i>)
		/// </pre></blockquote>
		/// If no such value of <i>k</i> exists, then {@code -1} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">         the substring to search for. </param>
		/// <param name="fromIndex">   the index to start the search from. </param>
		/// <returns>  the index of the last occurrence of the specified substring,
		///          searching backward from the specified index,
		///          or {@code -1} if there is no such occurrence. </returns>
		public int LastIndexOf(String str, int fromIndex)
		{
			return LastIndexOf(Value, 0, Value.Length, str.Value, 0, str.Value.Length, fromIndex);
		}

		/// <summary>
		/// Code shared by String and AbstractStringBuilder to do searches. The
		/// source is the character array being searched, and the target
		/// is the string being searched for.
		/// </summary>
		/// <param name="source">       the characters being searched. </param>
		/// <param name="sourceOffset"> offset of the source string. </param>
		/// <param name="sourceCount">  count of the source string. </param>
		/// <param name="target">       the characters being searched for. </param>
		/// <param name="fromIndex">    the index to begin searching from. </param>
		internal static int LastIndexOf(char[] source, int sourceOffset, int sourceCount, String target, int fromIndex)
		{
			return LastIndexOf(source, sourceOffset, sourceCount, target.Value, 0, target.Value.Length, fromIndex);
		}

		/// <summary>
		/// Code shared by String and StringBuffer to do searches. The
		/// source is the character array being searched, and the target
		/// is the string being searched for.
		/// </summary>
		/// <param name="source">       the characters being searched. </param>
		/// <param name="sourceOffset"> offset of the source string. </param>
		/// <param name="sourceCount">  count of the source string. </param>
		/// <param name="target">       the characters being searched for. </param>
		/// <param name="targetOffset"> offset of the target string. </param>
		/// <param name="targetCount">  count of the target string. </param>
		/// <param name="fromIndex">    the index to begin searching from. </param>
		internal static int LastIndexOf(char[] source, int sourceOffset, int sourceCount, char[] target, int targetOffset, int targetCount, int fromIndex)
		{
			/*
			 * Check arguments; return immediately where possible. For
			 * consistency, don't check for null str.
			 */
			int rightIndex = sourceCount - targetCount;
			if (fromIndex < 0)
			{
				return -1;
			}
			if (fromIndex > rightIndex)
			{
				fromIndex = rightIndex;
			}
			/* Empty string always matches. */
			if (targetCount == 0)
			{
				return fromIndex;
			}

			int strLastIndex = targetOffset + targetCount - 1;
			char strLastChar = target[strLastIndex];
			int min = sourceOffset + targetCount - 1;
			int i = min + fromIndex;

			while (true)
			{
				while (i >= min && source[i] != strLastChar)
				{
					i--;
				}
				if (i < min)
				{
					return -1;
				}
				int j = i - 1;
				int start = j - (targetCount - 1);
				int k = strLastIndex - 1;

				while (j > start)
				{
					if (source[j--] != target[k--])
					{
						i--;
						goto startSearchForLastCharContinue;
					}
				}
				return start - sourceOffset + 1;
			startSearchForLastCharContinue:;
			}
		startSearchForLastCharBreak:;
		}

		/// <summary>
		/// Returns a string that is a substring of this string. The
		/// substring begins with the character at the specified index and
		/// extends to the end of this string. <para>
		/// Examples:
		/// <blockquote><pre>
		/// "unhappy".substring(2) returns "happy"
		/// "Harbison".substring(3) returns "bison"
		/// "emptiness".substring(9) returns "" (an empty string)
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="beginIndex">   the beginning index, inclusive. </param>
		/// <returns>     the specified substring. </returns>
		/// <exception cref="IndexOutOfBoundsException">  if
		///             {@code beginIndex} is negative or larger than the
		///             length of this {@code String} object. </exception>
		public String Substring(int beginIndex)
		{
			if (beginIndex < 0)
			{
				throw new StringIndexOutOfBoundsException(beginIndex);
			}
			int subLen = Value.Length - beginIndex;
			if (subLen < 0)
			{
				throw new StringIndexOutOfBoundsException(subLen);
			}
			return (beginIndex == 0) ? this : new String(Value, beginIndex, subLen);
		}

		/// <summary>
		/// Returns a string that is a substring of this string. The
		/// substring begins at the specified {@code beginIndex} and
		/// extends to the character at index {@code endIndex - 1}.
		/// Thus the length of the substring is {@code endIndex-beginIndex}.
		/// <para>
		/// Examples:
		/// <blockquote><pre>
		/// "hamburger".substring(4, 8) returns "urge"
		/// "smiles".substring(1, 5) returns "mile"
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="beginIndex">   the beginning index, inclusive. </param>
		/// <param name="endIndex">     the ending index, exclusive. </param>
		/// <returns>     the specified substring. </returns>
		/// <exception cref="IndexOutOfBoundsException">  if the
		///             {@code beginIndex} is negative, or
		///             {@code endIndex} is larger than the length of
		///             this {@code String} object, or
		///             {@code beginIndex} is larger than
		///             {@code endIndex}. </exception>
		public String Substring(int beginIndex, int endIndex)
		{
			if (beginIndex < 0)
			{
				throw new StringIndexOutOfBoundsException(beginIndex);
			}
			if (endIndex > Value.Length)
			{
				throw new StringIndexOutOfBoundsException(endIndex);
			}
			int subLen = endIndex - beginIndex;
			if (subLen < 0)
			{
				throw new StringIndexOutOfBoundsException(subLen);
			}
			return ((beginIndex == 0) && (endIndex == Value.Length)) ? this : new String(Value, beginIndex, subLen);
		}

		/// <summary>
		/// Returns a character sequence that is a subsequence of this sequence.
		/// 
		/// <para> An invocation of this method of the form
		/// 
		/// <blockquote><pre>
		/// str.subSequence(begin,&nbsp;end)</pre></blockquote>
		/// 
		/// behaves in exactly the same way as the invocation
		/// 
		/// <blockquote><pre>
		/// str.substring(begin,&nbsp;end)</pre></blockquote>
		/// 
		/// @apiNote
		/// This method is defined so that the {@code String} class can implement
		/// the <seealso cref="CharSequence"/> interface.
		/// 
		/// </para>
		/// </summary>
		/// <param name="beginIndex">   the begin index, inclusive. </param>
		/// <param name="endIndex">     the end index, exclusive. </param>
		/// <returns>  the specified subsequence.
		/// </returns>
		/// <exception cref="IndexOutOfBoundsException">
		///          if {@code beginIndex} or {@code endIndex} is negative,
		///          if {@code endIndex} is greater than {@code length()},
		///          or if {@code beginIndex} is greater than {@code endIndex}
		/// 
		/// @since 1.4
		/// @spec JSR-51 </exception>
		public CharSequence SubSequence(int beginIndex, int endIndex)
		{
			return this.Substring(beginIndex, endIndex - beginIndex);
		}

		/// <summary>
		/// Concatenates the specified string to the end of this string.
		/// <para>
		/// If the length of the argument string is {@code 0}, then this
		/// {@code String} object is returned. Otherwise, a
		/// {@code String} object is returned that represents a character
		/// sequence that is the concatenation of the character sequence
		/// represented by this {@code String} object and the character
		/// </para>
		/// sequence represented by the argument string.<para>
		/// Examples:
		/// <blockquote><pre>
		/// "cares".concat("s") returns "caress"
		/// "to".concat("get").concat("her") returns "together"
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="str">   the {@code String} that is concatenated to the end
		///                of this {@code String}. </param>
		/// <returns>  a string that represents the concatenation of this object's
		///          characters followed by the string argument's characters. </returns>
		public String Concat(String str)
		{
			int otherLen = str.Length();
			if (otherLen == 0)
			{
				return this;
			}
			int len = Value.Length;
			char[] buf = Arrays.CopyOf(Value, len + otherLen);
			str.GetChars(buf, len);
			return new String(buf, true);
		}

		/// <summary>
		/// Returns a string resulting from replacing all occurrences of
		/// {@code oldChar} in this string with {@code newChar}.
		/// <para>
		/// If the character {@code oldChar} does not occur in the
		/// character sequence represented by this {@code String} object,
		/// then a reference to this {@code String} object is returned.
		/// Otherwise, a {@code String} object is returned that
		/// represents a character sequence identical to the character sequence
		/// represented by this {@code String} object, except that every
		/// occurrence of {@code oldChar} is replaced by an occurrence
		/// of {@code newChar}.
		/// </para>
		/// <para>
		/// Examples:
		/// <blockquote><pre>
		/// "mesquite in your cellar".replace('e', 'o')
		///         returns "mosquito in your collar"
		/// "the war of baronets".replace('r', 'y')
		///         returns "the way of bayonets"
		/// "sparring with a purple porpoise".replace('p', 't')
		///         returns "starring with a turtle tortoise"
		/// "JonL".replace('q', 'x') returns "JonL" (no change)
		/// </pre></blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="oldChar">   the old character. </param>
		/// <param name="newChar">   the new character. </param>
		/// <returns>  a string derived from this string by replacing every
		///          occurrence of {@code oldChar} with {@code newChar}. </returns>
		public String Replace(char oldChar, char newChar)
		{
			if (oldChar != newChar)
			{
				int len = Value.Length;
				int i = -1;
				char[] val = Value; // avoid getfield opcode

				while (++i < len)
				{
					if (val[i] == oldChar)
					{
						break;
					}
				}
				if (i < len)
				{
					char[] buf = new char[len];
					for (int j = 0; j < i; j++)
					{
						buf[j] = val[j];
					}
					while (i < len)
					{
						char c = val[i];
						buf[i] = (c == oldChar) ? newChar : c;
						i++;
					}
					return new String(buf, true);
				}
			}
			return this;
		}

		/// <summary>
		/// Tells whether or not this string matches the given <a
		/// href="../util/regex/Pattern.html#sum">regular expression</a>.
		/// 
		/// <para> An invocation of this method of the form
		/// <i>str</i>{@code .matches(}<i>regex</i>{@code )} yields exactly the
		/// same result as the expression
		/// 
		/// <blockquote>
		/// <seealso cref="java.util.regex.Pattern"/>.{@link java.util.regex.Pattern#matches(String,CharSequence)
		/// matches(<i>regex</i>, <i>str</i>)}
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// <param name="regex">
		///          the regular expression to which this string is to be matched
		/// </param>
		/// <returns>  {@code true} if, and only if, this string matches the
		///          given regular expression
		/// </returns>
		/// <exception cref="PatternSyntaxException">
		///          if the regular expression's syntax is invalid
		/// </exception>
		/// <seealso cref= java.util.regex.Pattern
		/// 
		/// @since 1.4
		/// @spec JSR-51 </seealso>
		public bool Matches(String regex)
		{
			return Pattern.Matches(regex, this);
		}

		/// <summary>
		/// Returns true if and only if this string contains the specified
		/// sequence of char values.
		/// </summary>
		/// <param name="s"> the sequence to search for </param>
		/// <returns> true if this string contains {@code s}, false otherwise
		/// @since 1.5 </returns>
		public bool Contains(CharSequence s)
		{
			return IndexOf(s.ToString()) > -1;
		}

		/// <summary>
		/// Replaces the first substring of this string that matches the given <a
		/// href="../util/regex/Pattern.html#sum">regular expression</a> with the
		/// given replacement.
		/// 
		/// <para> An invocation of this method of the form
		/// <i>str</i>{@code .replaceFirst(}<i>regex</i>{@code ,} <i>repl</i>{@code )}
		/// yields exactly the same result as the expression
		/// 
		/// <blockquote>
		/// <code>
		/// <seealso cref="java.util.regex.Pattern"/>.{@link
		/// java.util.regex.Pattern#compile compile}(<i>regex</i>).{@link
		/// java.util.regex.Pattern#matcher(java.lang.CharSequence) matcher}(<i>str</i>).{@link
		/// java.util.regex.Matcher#replaceFirst replaceFirst}(<i>repl</i>)
		/// </code>
		/// </blockquote>
		/// 
		/// </para>
		/// <para>
		/// Note that backslashes ({@code \}) and dollar signs ({@code $}) in the
		/// replacement string may cause the results to be different than if it were
		/// being treated as a literal replacement string; see
		/// <seealso cref="java.util.regex.Matcher#replaceFirst"/>.
		/// Use <seealso cref="java.util.regex.Matcher#quoteReplacement"/> to suppress the special
		/// meaning of these characters, if desired.
		/// 
		/// </para>
		/// </summary>
		/// <param name="regex">
		///          the regular expression to which this string is to be matched </param>
		/// <param name="replacement">
		///          the string to be substituted for the first match
		/// </param>
		/// <returns>  The resulting {@code String}
		/// </returns>
		/// <exception cref="PatternSyntaxException">
		///          if the regular expression's syntax is invalid
		/// </exception>
		/// <seealso cref= java.util.regex.Pattern
		/// 
		/// @since 1.4
		/// @spec JSR-51 </seealso>
		public String ReplaceFirst(String regex, String replacement)
		{
			return Pattern.Compile(regex).Matcher(this).ReplaceFirst(replacement);
		}

		/// <summary>
		/// Replaces each substring of this string that matches the given <a
		/// href="../util/regex/Pattern.html#sum">regular expression</a> with the
		/// given replacement.
		/// 
		/// <para> An invocation of this method of the form
		/// <i>str</i>{@code .replaceAll(}<i>regex</i>{@code ,} <i>repl</i>{@code )}
		/// yields exactly the same result as the expression
		/// 
		/// <blockquote>
		/// <code>
		/// <seealso cref="java.util.regex.Pattern"/>.{@link
		/// java.util.regex.Pattern#compile compile}(<i>regex</i>).{@link
		/// java.util.regex.Pattern#matcher(java.lang.CharSequence) matcher}(<i>str</i>).{@link
		/// java.util.regex.Matcher#replaceAll replaceAll}(<i>repl</i>)
		/// </code>
		/// </blockquote>
		/// 
		/// </para>
		/// <para>
		/// Note that backslashes ({@code \}) and dollar signs ({@code $}) in the
		/// replacement string may cause the results to be different than if it were
		/// being treated as a literal replacement string; see
		/// <seealso cref="java.util.regex.Matcher#replaceAll Matcher.replaceAll"/>.
		/// Use <seealso cref="java.util.regex.Matcher#quoteReplacement"/> to suppress the special
		/// meaning of these characters, if desired.
		/// 
		/// </para>
		/// </summary>
		/// <param name="regex">
		///          the regular expression to which this string is to be matched </param>
		/// <param name="replacement">
		///          the string to be substituted for each match
		/// </param>
		/// <returns>  The resulting {@code String}
		/// </returns>
		/// <exception cref="PatternSyntaxException">
		///          if the regular expression's syntax is invalid
		/// </exception>
		/// <seealso cref= java.util.regex.Pattern
		/// 
		/// @since 1.4
		/// @spec JSR-51 </seealso>
		public String ReplaceAll(String regex, String replacement)
		{
			return Pattern.Compile(regex).Matcher(this).ReplaceAll(replacement);
		}

		/// <summary>
		/// Replaces each substring of this string that matches the literal target
		/// sequence with the specified literal replacement sequence. The
		/// replacement proceeds from the beginning of the string to the end, for
		/// example, replacing "aa" with "b" in the string "aaa" will result in
		/// "ba" rather than "ab".
		/// </summary>
		/// <param name="target"> The sequence of char values to be replaced </param>
		/// <param name="replacement"> The replacement sequence of char values </param>
		/// <returns>  The resulting string
		/// @since 1.5 </returns>
		public String Replace(CharSequence target, CharSequence replacement)
		{
			return Pattern.Compile(target.ToString(), Pattern.LITERAL).Matcher(this).ReplaceAll(Matcher.QuoteReplacement(replacement.ToString()));
		}

		/// <summary>
		/// Splits this string around matches of the given
		/// <a href="../util/regex/Pattern.html#sum">regular expression</a>.
		/// 
		/// <para> The array returned by this method contains each substring of this
		/// string that is terminated by another substring that matches the given
		/// expression or is terminated by the end of the string.  The substrings in
		/// the array are in the order in which they occur in this string.  If the
		/// expression does not match any part of the input then the resulting array
		/// has just one element, namely this string.
		/// 
		/// </para>
		/// <para> When there is a positive-width match at the beginning of this
		/// string then an empty leading substring is included at the beginning
		/// of the resulting array. A zero-width match at the beginning however
		/// never produces such empty leading substring.
		/// 
		/// </para>
		/// <para> The {@code limit} parameter controls the number of times the
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
		/// <para> The string {@code "boo:and:foo"}, for example, yields the
		/// following results with these parameters:
		/// 
		/// <blockquote><table cellpadding=1 cellspacing=0 summary="Split example showing regex, limit, and result">
		/// <tr>
		///     <th>Regex</th>
		///     <th>Limit</th>
		///     <th>Result</th>
		/// </tr>
		/// <tr><td align=center>:</td>
		///     <td align=center>2</td>
		///     <td>{@code { "boo", "and:foo" }}</td></tr>
		/// <tr><td align=center>:</td>
		///     <td align=center>5</td>
		///     <td>{@code { "boo", "and", "foo" }}</td></tr>
		/// <tr><td align=center>:</td>
		///     <td align=center>-2</td>
		///     <td>{@code { "boo", "and", "foo" }}</td></tr>
		/// <tr><td align=center>o</td>
		///     <td align=center>5</td>
		///     <td>{@code { "b", "", ":and:f", "", "" }}</td></tr>
		/// <tr><td align=center>o</td>
		///     <td align=center>-2</td>
		///     <td>{@code { "b", "", ":and:f", "", "" }}</td></tr>
		/// <tr><td align=center>o</td>
		///     <td align=center>0</td>
		///     <td>{@code { "b", "", ":and:f" }}</td></tr>
		/// </table></blockquote>
		/// 
		/// </para>
		/// <para> An invocation of this method of the form
		/// <i>str.</i>{@code split(}<i>regex</i>{@code ,}&nbsp;<i>n</i>{@code )}
		/// yields the same result as the expression
		/// 
		/// <blockquote>
		/// <code>
		/// <seealso cref="java.util.regex.Pattern"/>.{@link
		/// java.util.regex.Pattern#compile compile}(<i>regex</i>).{@link
		/// java.util.regex.Pattern#split(java.lang.CharSequence,int) split}(<i>str</i>,&nbsp;<i>n</i>)
		/// </code>
		/// </blockquote>
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="regex">
		///         the delimiting regular expression
		/// </param>
		/// <param name="limit">
		///         the result threshold, as described above
		/// </param>
		/// <returns>  the array of strings computed by splitting this string
		///          around matches of the given regular expression
		/// </returns>
		/// <exception cref="PatternSyntaxException">
		///          if the regular expression's syntax is invalid
		/// </exception>
		/// <seealso cref= java.util.regex.Pattern
		/// 
		/// @since 1.4
		/// @spec JSR-51 </seealso>
		public String[] Split(String regex, int limit)
		{
			/* fastpath if the regex is a
			 (1)one-char String and this character is not one of the
			    RegEx's meta characters ".$|()[{^?*+\\", or
			 (2)two-char String and the first char is the backslash and
			    the second is not the ascii digit or ascii letter.
			 */
			char ch = (char)0;
			if (((regex.Value.Length == 1 && ".$|()[{^?*+\\".IndexOf(ch = regex.CharAt(0)) == -1) || (regex.Length() == 2 && regex.CharAt(0) == '\\' && (((ch = regex.CharAt(1)) - '0') | ('9' - ch)) < 0 && ((ch - 'a') | ('z' - ch)) < 0 && ((ch - 'A') | ('Z' - ch)) < 0)) && (ch < Character.MIN_HIGH_SURROGATE || ch > Character.MAX_LOW_SURROGATE))
			{
				int off = 0;
				int next = 0;
				bool limited = limit > 0;
				List<String> list = new List<String>();
				while ((next = IndexOf(ch, off)) != -1)
				{
					if (!limited || list.Count < limit - 1)
					{
						list.Add(Substring(off, next));
						off = next + 1;
					} // last one
					else
					{
						//assert (list.size() == limit - 1);
						list.Add(Substring(off, Value.Length));
						off = Value.Length;
						break;
					}
				}
				// If no match was found, return this
				if (off == 0)
				{
					return new String[]{this};
				}

				// Add remaining segment
				if (!limited || list.Count < limit)
				{
					list.Add(Substring(off, Value.Length));
				}

				// Construct result
				int resultSize = list.Count;
				if (limit == 0)
				{
					while (resultSize > 0 && list[resultSize - 1].Length() == 0)
					{
						resultSize--;
					}
				}
				String[] result = new String[resultSize];
				return list.subList(0, resultSize).toArray(result);
			}
			return Pattern.Compile(regex).Split(this, limit);
		}

		/// <summary>
		/// Splits this string around matches of the given <a
		/// href="../util/regex/Pattern.html#sum">regular expression</a>.
		/// 
		/// <para> This method works as if by invoking the two-argument {@link
		/// #split(String, int) split} method with the given expression and a limit
		/// argument of zero.  Trailing empty strings are therefore not included in
		/// the resulting array.
		/// 
		/// </para>
		/// <para> The string {@code "boo:and:foo"}, for example, yields the following
		/// results with these expressions:
		/// 
		/// <blockquote><table cellpadding=1 cellspacing=0 summary="Split examples showing regex and result">
		/// <tr>
		///  <th>Regex</th>
		///  <th>Result</th>
		/// </tr>
		/// <tr><td align=center>:</td>
		///     <td>{@code { "boo", "and", "foo" }}</td></tr>
		/// <tr><td align=center>o</td>
		///     <td>{@code { "b", "", ":and:f" }}</td></tr>
		/// </table></blockquote>
		/// 
		/// 
		/// </para>
		/// </summary>
		/// <param name="regex">
		///         the delimiting regular expression
		/// </param>
		/// <returns>  the array of strings computed by splitting this string
		///          around matches of the given regular expression
		/// </returns>
		/// <exception cref="PatternSyntaxException">
		///          if the regular expression's syntax is invalid
		/// </exception>
		/// <seealso cref= java.util.regex.Pattern
		/// 
		/// @since 1.4
		/// @spec JSR-51 </seealso>
		public String[] Split(String regex)
		{
			return Split(regex, 0);
		}

		/// <summary>
		/// Returns a new String composed of copies of the
		/// {@code CharSequence elements} joined together with a copy of
		/// the specified {@code delimiter}.
		/// 
		/// <blockquote>For example,
		/// <pre>{@code
		///     String message = String.join("-", "Java", "is", "cool");
		///     // message returned is: "Java-is-cool"
		/// }</pre></blockquote>
		/// 
		/// Note that if an element is null, then {@code "null"} is added.
		/// </summary>
		/// <param name="delimiter"> the delimiter that separates each element </param>
		/// <param name="elements"> the elements to join together.
		/// </param>
		/// <returns> a new {@code String} that is composed of the {@code elements}
		///         separated by the {@code delimiter}
		/// </returns>
		/// <exception cref="NullPointerException"> If {@code delimiter} or {@code elements}
		///         is {@code null}
		/// </exception>
		/// <seealso cref= java.util.StringJoiner
		/// @since 1.8 </seealso>
		public static String Join(CharSequence delimiter, params CharSequence[] elements)
		{
			Objects.RequireNonNull(delimiter);
			Objects.RequireNonNull(elements);
			// Number of elements not likely worth Arrays.stream overhead.
			StringJoiner joiner = new StringJoiner(delimiter);
			foreach (CharSequence cs in elements)
			{
				joiner.Add(cs);
			}
			return joiner.ToString();
		}

		/// <summary>
		/// Returns a new {@code String} composed of copies of the
		/// {@code CharSequence elements} joined together with a copy of the
		/// specified {@code delimiter}.
		/// 
		/// <blockquote>For example,
		/// <pre>{@code
		///     List<String> strings = new LinkedList<>();
		///     strings.add("Java");strings.add("is");
		///     strings.add("cool");
		///     String message = String.join(" ", strings);
		///     //message returned is: "Java is cool"
		/// 
		///     Set<String> strings = new LinkedHashSet<>();
		///     strings.add("Java"); strings.add("is");
		///     strings.add("very"); strings.add("cool");
		///     String message = String.join("-", strings);
		///     //message returned is: "Java-is-very-cool"
		/// }</pre></blockquote>
		/// 
		/// Note that if an individual element is {@code null}, then {@code "null"} is added.
		/// </summary>
		/// <param name="delimiter"> a sequence of characters that is used to separate each
		///         of the {@code elements} in the resulting {@code String} </param>
		/// <param name="elements"> an {@code Iterable} that will have its {@code elements}
		///         joined together.
		/// </param>
		/// <returns> a new {@code String} that is composed from the {@code elements}
		///         argument
		/// </returns>
		/// <exception cref="NullPointerException"> If {@code delimiter} or {@code elements}
		///         is {@code null}
		/// </exception>
		/// <seealso cref=    #join(CharSequence,CharSequence...) </seealso>
		/// <seealso cref=    java.util.StringJoiner
		/// @since 1.8 </seealso>
		public static String join<T1>(CharSequence delimiter, Iterable<T1> elements) where T1 : CharSequence
		{
			Objects.RequireNonNull(delimiter);
			Objects.RequireNonNull(elements);
			StringJoiner joiner = new StringJoiner(delimiter);
			foreach (CharSequence cs in elements)
			{
				joiner.Add(cs);
			}
			return joiner.ToString();
		}

		/// <summary>
		/// Converts all of the characters in this {@code String} to lower
		/// case using the rules of the given {@code Locale}.  Case mapping is based
		/// on the Unicode Standard version specified by the <seealso cref="java.lang.Character Character"/>
		/// class. Since case mappings are not always 1:1 char mappings, the resulting
		/// {@code String} may be a different length than the original {@code String}.
		/// <para>
		/// Examples of lowercase  mappings are in the following table:
		/// <table border="1" summary="Lowercase mapping examples showing language code of locale, upper case, lower case, and description">
		/// <tr>
		///   <th>Language Code of Locale</th>
		///   <th>Upper Case</th>
		///   <th>Lower Case</th>
		///   <th>Description</th>
		/// </tr>
		/// <tr>
		///   <td>tr (Turkish)</td>
		///   <td>&#92;u0130</td>
		///   <td>&#92;u0069</td>
		///   <td>capital letter I with dot above -&gt; small letter i</td>
		/// </tr>
		/// <tr>
		///   <td>tr (Turkish)</td>
		///   <td>&#92;u0049</td>
		///   <td>&#92;u0131</td>
		///   <td>capital letter I -&gt; small letter dotless i </td>
		/// </tr>
		/// <tr>
		///   <td>(all)</td>
		///   <td>French Fries</td>
		///   <td>french fries</td>
		///   <td>lowercased all chars in String</td>
		/// </tr>
		/// <tr>
		///   <td>(all)</td>
		///   <td><img src="doc-files/capiota.gif" alt="capiota"><img src="doc-files/capchi.gif" alt="capchi">
		///       <img src="doc-files/captheta.gif" alt="captheta"><img src="doc-files/capupsil.gif" alt="capupsil">
		///       <img src="doc-files/capsigma.gif" alt="capsigma"></td>
		///   <td><img src="doc-files/iota.gif" alt="iota"><img src="doc-files/chi.gif" alt="chi">
		///       <img src="doc-files/theta.gif" alt="theta"><img src="doc-files/upsilon.gif" alt="upsilon">
		///       <img src="doc-files/sigma1.gif" alt="sigma"></td>
		///   <td>lowercased all chars in String</td>
		/// </tr>
		/// </table>
		/// 
		/// </para>
		/// </summary>
		/// <param name="locale"> use the case transformation rules for this locale </param>
		/// <returns> the {@code String}, converted to lowercase. </returns>
		/// <seealso cref=     java.lang.String#toLowerCase() </seealso>
		/// <seealso cref=     java.lang.String#toUpperCase() </seealso>
		/// <seealso cref=     java.lang.String#toUpperCase(Locale)
		/// @since   1.1 </seealso>
		public String ToLowerCase(Locale locale)
		{
			if (locale == null)
			{
				throw new NullPointerException();
			}

			int firstUpper;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = value.length;
			int len = Value.Length;

			/* Now check if there are any characters that need to be changed. */
			{
				for (firstUpper = 0 ; firstUpper < len;)
				{
					char c = Value[firstUpper];
					if ((c >= Character.MIN_HIGH_SURROGATE) && (c <= Character.MAX_HIGH_SURROGATE))
					{
						int supplChar = CodePointAt(firstUpper);
						if (supplChar != char.ToLower(supplChar))
						{
							goto scanBreak;
						}
						firstUpper += Character.CharCount(supplChar);
					}
					else
					{
						if (c != char.ToLower(c))
						{
							goto scanBreak;
						}
						firstUpper++;
					}
				}
				return this;
			}
			scanBreak:

			char[] result = new char[len];
			int resultOffset = 0; /* result may grow, so i+resultOffset
	                                * is the write location in result */

			/* Just copy the first few lowerCase characters. */
			System.Array.Copy(Value, 0, result, 0, firstUpper);

			String lang = locale.Language;
			bool localeDependent = (lang == "tr" || lang == "az" || lang == "lt");
			char[] lowerCharArray;
			int lowerChar;
			int srcChar;
			int srcCount;
			for (int i = firstUpper; i < len; i += srcCount)
			{
				srcChar = (int)Value[i];
				if ((char)srcChar >= Character.MIN_HIGH_SURROGATE && (char)srcChar <= Character.MAX_HIGH_SURROGATE)
				{
					srcChar = CodePointAt(i);
					srcCount = Character.CharCount(srcChar);
				}
				else
				{
					srcCount = 1;
				}
				if (localeDependent || srcChar == '\u03A3' || srcChar == '\u0130') // LATIN CAPITAL LETTER I WITH DOT ABOVE -  GREEK CAPITAL LETTER SIGMA
				{
					lowerChar = ConditionalSpecialCasing.ToLowerCaseEx(this, i, locale);
				}
				else
				{
					lowerChar = char.ToLower(srcChar);
				}
				if ((lowerChar == Character.ERROR) || (lowerChar >= Character.MIN_SUPPLEMENTARY_CODE_POINT))
				{
					if (lowerChar == Character.ERROR)
					{
						lowerCharArray = ConditionalSpecialCasing.ToLowerCaseCharArray(this, i, locale);
					}
					else if (srcCount == 2)
					{
						resultOffset += Character.ToChars(lowerChar, result, i + resultOffset) - srcCount;
						continue;
					}
					else
					{
						lowerCharArray = Character.ToChars(lowerChar);
					}

					/* Grow result if needed */
					int mapLen = lowerCharArray.Length;
					if (mapLen > srcCount)
					{
						char[] result2 = new char[result.Length + mapLen - srcCount];
						System.Array.Copy(result, 0, result2, 0, i + resultOffset);
						result = result2;
					}
					for (int x = 0; x < mapLen; ++x)
					{
						result[i + resultOffset + x] = lowerCharArray[x];
					}
					resultOffset += (mapLen - srcCount);
				}
				else
				{
					result[i + resultOffset] = (char)lowerChar;
				}
			}
			return new String(result, 0, len + resultOffset);
		}

		/// <summary>
		/// Converts all of the characters in this {@code String} to lower
		/// case using the rules of the default locale. This is equivalent to calling
		/// {@code toLowerCase(Locale.getDefault())}.
		/// <para>
		/// <b>Note:</b> This method is locale sensitive, and may produce unexpected
		/// results if used for strings that are intended to be interpreted locale
		/// independently.
		/// Examples are programming language identifiers, protocol keys, and HTML
		/// tags.
		/// For instance, {@code "TITLE".toLowerCase()} in a Turkish locale
		/// returns {@code "t\u005Cu0131tle"}, where '\u005Cu0131' is the
		/// LATIN SMALL LETTER DOTLESS I character.
		/// To obtain correct results for locale insensitive strings, use
		/// {@code toLowerCase(Locale.ROOT)}.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <returns>  the {@code String}, converted to lowercase. </returns>
		/// <seealso cref=     java.lang.String#toLowerCase(Locale) </seealso>
		public String ToLowerCase()
		{
			return ToLowerCase(Locale.Default);
		}

		/// <summary>
		/// Converts all of the characters in this {@code String} to upper
		/// case using the rules of the given {@code Locale}. Case mapping is based
		/// on the Unicode Standard version specified by the <seealso cref="java.lang.Character Character"/>
		/// class. Since case mappings are not always 1:1 char mappings, the resulting
		/// {@code String} may be a different length than the original {@code String}.
		/// <para>
		/// Examples of locale-sensitive and 1:M case mappings are in the following table.
		/// 
		/// <table border="1" summary="Examples of locale-sensitive and 1:M case mappings. Shows Language code of locale, lower case, upper case, and description.">
		/// <tr>
		///   <th>Language Code of Locale</th>
		///   <th>Lower Case</th>
		///   <th>Upper Case</th>
		///   <th>Description</th>
		/// </tr>
		/// <tr>
		///   <td>tr (Turkish)</td>
		///   <td>&#92;u0069</td>
		///   <td>&#92;u0130</td>
		///   <td>small letter i -&gt; capital letter I with dot above</td>
		/// </tr>
		/// <tr>
		///   <td>tr (Turkish)</td>
		///   <td>&#92;u0131</td>
		///   <td>&#92;u0049</td>
		///   <td>small letter dotless i -&gt; capital letter I</td>
		/// </tr>
		/// <tr>
		///   <td>(all)</td>
		///   <td>&#92;u00df</td>
		///   <td>&#92;u0053 &#92;u0053</td>
		///   <td>small letter sharp s -&gt; two letters: SS</td>
		/// </tr>
		/// <tr>
		///   <td>(all)</td>
		///   <td>Fahrvergn&uuml;gen</td>
		///   <td>FAHRVERGN&Uuml;GEN</td>
		///   <td></td>
		/// </tr>
		/// </table>
		/// </para>
		/// </summary>
		/// <param name="locale"> use the case transformation rules for this locale </param>
		/// <returns> the {@code String}, converted to uppercase. </returns>
		/// <seealso cref=     java.lang.String#toUpperCase() </seealso>
		/// <seealso cref=     java.lang.String#toLowerCase() </seealso>
		/// <seealso cref=     java.lang.String#toLowerCase(Locale)
		/// @since   1.1 </seealso>
		public String ToUpperCase(Locale locale)
		{
			if (locale == null)
			{
				throw new NullPointerException();
			}

			int firstLower;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = value.length;
			int len = Value.Length;

			/* Now check if there are any characters that need to be changed. */
			{
				for (firstLower = 0 ; firstLower < len;)
				{
					int c = (int)Value[firstLower];
					int srcCount;
					if ((c >= Character.MIN_HIGH_SURROGATE) && (c <= Character.MAX_HIGH_SURROGATE))
					{
						c = CodePointAt(firstLower);
						srcCount = Character.CharCount(c);
					}
					else
					{
						srcCount = 1;
					}
					int upperCaseChar = Character.ToUpperCaseEx(c);
					if ((upperCaseChar == Character.ERROR) || (c != upperCaseChar))
					{
						goto scanBreak;
					}
					firstLower += srcCount;
				}
				return this;
			}
			scanBreak:

			/* result may grow, so i+resultOffset is the write location in result */
			int resultOffset = 0;
			char[] result = new char[len]; // may grow

			/* Just copy the first few upperCase characters. */
			System.Array.Copy(Value, 0, result, 0, firstLower);

			String lang = locale.Language;
			bool localeDependent = (lang == "tr" || lang == "az" || lang == "lt");
			char[] upperCharArray;
			int upperChar;
			int srcChar;
			int srcCount;
			for (int i = firstLower; i < len; i += srcCount)
			{
				srcChar = (int)Value[i];
				if ((char)srcChar >= Character.MIN_HIGH_SURROGATE && (char)srcChar <= Character.MAX_HIGH_SURROGATE)
				{
					srcChar = CodePointAt(i);
					srcCount = Character.CharCount(srcChar);
				}
				else
				{
					srcCount = 1;
				}
				if (localeDependent)
				{
					upperChar = ConditionalSpecialCasing.ToUpperCaseEx(this, i, locale);
				}
				else
				{
					upperChar = Character.ToUpperCaseEx(srcChar);
				}
				if ((upperChar == Character.ERROR) || (upperChar >= Character.MIN_SUPPLEMENTARY_CODE_POINT))
				{
					if (upperChar == Character.ERROR)
					{
						if (localeDependent)
						{
							upperCharArray = ConditionalSpecialCasing.ToUpperCaseCharArray(this, i, locale);
						}
						else
						{
							upperCharArray = Character.ToUpperCaseCharArray(srcChar);
						}
					}
					else if (srcCount == 2)
					{
						resultOffset += Character.ToChars(upperChar, result, i + resultOffset) - srcCount;
						continue;
					}
					else
					{
						upperCharArray = Character.ToChars(upperChar);
					}

					/* Grow result if needed */
					int mapLen = upperCharArray.Length;
					if (mapLen > srcCount)
					{
						char[] result2 = new char[result.Length + mapLen - srcCount];
						System.Array.Copy(result, 0, result2, 0, i + resultOffset);
						result = result2;
					}
					for (int x = 0; x < mapLen; ++x)
					{
						result[i + resultOffset + x] = upperCharArray[x];
					}
					resultOffset += (mapLen - srcCount);
				}
				else
				{
					result[i + resultOffset] = (char)upperChar;
				}
			}
			return new String(result, 0, len + resultOffset);
		}

		/// <summary>
		/// Converts all of the characters in this {@code String} to upper
		/// case using the rules of the default locale. This method is equivalent to
		/// {@code toUpperCase(Locale.getDefault())}.
		/// <para>
		/// <b>Note:</b> This method is locale sensitive, and may produce unexpected
		/// results if used for strings that are intended to be interpreted locale
		/// independently.
		/// Examples are programming language identifiers, protocol keys, and HTML
		/// tags.
		/// For instance, {@code "title".toUpperCase()} in a Turkish locale
		/// returns {@code "T\u005Cu0130TLE"}, where '\u005Cu0130' is the
		/// LATIN CAPITAL LETTER I WITH DOT ABOVE character.
		/// To obtain correct results for locale insensitive strings, use
		/// {@code toUpperCase(Locale.ROOT)}.
		/// </para>
		/// <para>
		/// </para>
		/// </summary>
		/// <returns>  the {@code String}, converted to uppercase. </returns>
		/// <seealso cref=     java.lang.String#toUpperCase(Locale) </seealso>
		public String ToUpperCase()
		{
			return ToUpperCase(Locale.Default);
		}

		/// <summary>
		/// Returns a string whose value is this string, with any leading and trailing
		/// whitespace removed.
		/// <para>
		/// If this {@code String} object represents an empty character
		/// sequence, or the first and last characters of character sequence
		/// represented by this {@code String} object both have codes
		/// greater than {@code '\u005Cu0020'} (the space character), then a
		/// reference to this {@code String} object is returned.
		/// </para>
		/// <para>
		/// Otherwise, if there is no character with a code greater than
		/// {@code '\u005Cu0020'} in the string, then a
		/// {@code String} object representing an empty string is
		/// returned.
		/// </para>
		/// <para>
		/// Otherwise, let <i>k</i> be the index of the first character in the
		/// string whose code is greater than {@code '\u005Cu0020'}, and let
		/// <i>m</i> be the index of the last character in the string whose code
		/// is greater than {@code '\u005Cu0020'}. A {@code String}
		/// object is returned, representing the substring of this string that
		/// begins with the character at index <i>k</i> and ends with the
		/// character at index <i>m</i>-that is, the result of
		/// {@code this.substring(k, m + 1)}.
		/// </para>
		/// <para>
		/// This method may be used to trim whitespace (as defined above) from
		/// the beginning and end of a string.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  A string whose value is this string, with any leading and trailing white
		///          space removed, or this string if it has no leading or
		///          trailing white space. </returns>
		public String Trim()
		{
			int len = Value.Length;
			int st = 0;
			char[] val = Value; // avoid getfield opcode

			while ((st < len) && (val[st] <= ' '))
			{
				st++;
			}
			while ((st < len) && (val[len - 1] <= ' '))
			{
				len--;
			}
			return ((st > 0) || (len < Value.Length)) ? Substring(st, len) : this;
		}

		/// <summary>
		/// This object (which is already a string!) is itself returned.
		/// </summary>
		/// <returns>  the string itself. </returns>
		public override String ToString()
		{
			return this;
		}

		/// <summary>
		/// Converts this string to a new character array.
		/// </summary>
		/// <returns>  a newly allocated character array whose length is the length
		///          of this string and whose contents are initialized to contain
		///          the character sequence represented by this string. </returns>
		public char[] ToCharArray()
		{
			// Cannot use Arrays.copyOf because of class initialization order issues
			char[] result = new char[Value.Length];
			System.Array.Copy(Value, 0, result, 0, Value.Length);
			return result;
		}

		/// <summary>
		/// Returns a formatted string using the specified format string and
		/// arguments.
		/// 
		/// <para> The locale always used is the one returned by {@link
		/// java.util.Locale#getDefault() Locale.getDefault()}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="format">
		///         A <a href="../util/Formatter.html#syntax">format string</a>
		/// </param>
		/// <param name="args">
		///         Arguments referenced by the format specifiers in the format
		///         string.  If there are more arguments than format specifiers, the
		///         extra arguments are ignored.  The number of arguments is
		///         variable and may be zero.  The maximum number of arguments is
		///         limited by the maximum dimension of a Java array as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		///         The behaviour on a
		///         {@code null} argument depends on the <a
		///         href="../util/Formatter.html#syntax">conversion</a>.
		/// </param>
		/// <exception cref="java.util.IllegalFormatException">
		///          If a format string contains an illegal syntax, a format
		///          specifier that is incompatible with the given arguments,
		///          insufficient arguments given the format string, or other
		///          illegal conditions.  For specification of all possible
		///          formatting errors, see the <a
		///          href="../util/Formatter.html#detail">Details</a> section of the
		///          formatter class specification.
		/// </exception>
		/// <returns>  A formatted string
		/// </returns>
		/// <seealso cref=  java.util.Formatter
		/// @since  1.5 </seealso>
		public static String Format(String format, params Object[] args)
		{
			return (new Formatter()).Format(format, args).ToString();
		}

		/// <summary>
		/// Returns a formatted string using the specified locale, format string,
		/// and arguments.
		/// </summary>
		/// <param name="l">
		///         The <seealso cref="java.util.Locale locale"/> to apply during
		///         formatting.  If {@code l} is {@code null} then no localization
		///         is applied.
		/// </param>
		/// <param name="format">
		///         A <a href="../util/Formatter.html#syntax">format string</a>
		/// </param>
		/// <param name="args">
		///         Arguments referenced by the format specifiers in the format
		///         string.  If there are more arguments than format specifiers, the
		///         extra arguments are ignored.  The number of arguments is
		///         variable and may be zero.  The maximum number of arguments is
		///         limited by the maximum dimension of a Java array as defined by
		///         <cite>The Java&trade; Virtual Machine Specification</cite>.
		///         The behaviour on a
		///         {@code null} argument depends on the
		///         <a href="../util/Formatter.html#syntax">conversion</a>.
		/// </param>
		/// <exception cref="java.util.IllegalFormatException">
		///          If a format string contains an illegal syntax, a format
		///          specifier that is incompatible with the given arguments,
		///          insufficient arguments given the format string, or other
		///          illegal conditions.  For specification of all possible
		///          formatting errors, see the <a
		///          href="../util/Formatter.html#detail">Details</a> section of the
		///          formatter class specification
		/// </exception>
		/// <returns>  A formatted string
		/// </returns>
		/// <seealso cref=  java.util.Formatter
		/// @since  1.5 </seealso>
		public static String Format(Locale l, String format, params Object[] args)
		{
			return (new Formatter(l)).Format(format, args).ToString();
		}

		/// <summary>
		/// Returns the string representation of the {@code Object} argument.
		/// </summary>
		/// <param name="obj">   an {@code Object}. </param>
		/// <returns>  if the argument is {@code null}, then a string equal to
		///          {@code "null"}; otherwise, the value of
		///          {@code obj.toString()} is returned. </returns>
		/// <seealso cref=     java.lang.Object#toString() </seealso>
		public static String ValueOf(Object obj)
		{
			return (obj == null) ? "null" : obj.ToString();
		}

		/// <summary>
		/// Returns the string representation of the {@code char} array
		/// argument. The contents of the character array are copied; subsequent
		/// modification of the character array does not affect the returned
		/// string.
		/// </summary>
		/// <param name="data">     the character array. </param>
		/// <returns>  a {@code String} that contains the characters of the
		///          character array. </returns>
		public static String ValueOf(char[] data)
		{
			return new String(data);
		}

		/// <summary>
		/// Returns the string representation of a specific subarray of the
		/// {@code char} array argument.
		/// <para>
		/// The {@code offset} argument is the index of the first
		/// character of the subarray. The {@code count} argument
		/// specifies the length of the subarray. The contents of the subarray
		/// are copied; subsequent modification of the character array does not
		/// affect the returned string.
		/// 
		/// </para>
		/// </summary>
		/// <param name="data">     the character array. </param>
		/// <param name="offset">   initial offset of the subarray. </param>
		/// <param name="count">    length of the subarray. </param>
		/// <returns>  a {@code String} that contains the characters of the
		///          specified subarray of the character array. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code offset} is
		///          negative, or {@code count} is negative, or
		///          {@code offset+count} is larger than
		///          {@code data.length}. </exception>
		public static String ValueOf(char[] data, int offset, int count)
		{
			return new String(data, offset, count);
		}

		/// <summary>
		/// Equivalent to <seealso cref="#valueOf(char[], int, int)"/>.
		/// </summary>
		/// <param name="data">     the character array. </param>
		/// <param name="offset">   initial offset of the subarray. </param>
		/// <param name="count">    length of the subarray. </param>
		/// <returns>  a {@code String} that contains the characters of the
		///          specified subarray of the character array. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if {@code offset} is
		///          negative, or {@code count} is negative, or
		///          {@code offset+count} is larger than
		///          {@code data.length}. </exception>
		public static String CopyValueOf(char[] data, int offset, int count)
		{
			return new String(data, offset, count);
		}

		/// <summary>
		/// Equivalent to <seealso cref="#valueOf(char[])"/>.
		/// </summary>
		/// <param name="data">   the character array. </param>
		/// <returns>  a {@code String} that contains the characters of the
		///          character array. </returns>
		public static String CopyValueOf(char[] data)
		{
			return new String(data);
		}

		/// <summary>
		/// Returns the string representation of the {@code boolean} argument.
		/// </summary>
		/// <param name="b">   a {@code boolean}. </param>
		/// <returns>  if the argument is {@code true}, a string equal to
		///          {@code "true"} is returned; otherwise, a string equal to
		///          {@code "false"} is returned. </returns>
		public static String ValueOf(bool b)
		{
			return b ? "true" : "false";
		}

		/// <summary>
		/// Returns the string representation of the {@code char}
		/// argument.
		/// </summary>
		/// <param name="c">   a {@code char}. </param>
		/// <returns>  a string of length {@code 1} containing
		///          as its single character the argument {@code c}. </returns>
		public static String ValueOf(char c)
		{
			char[] data = new char[] {c};
			return new String(data, true);
		}

		/// <summary>
		/// Returns the string representation of the {@code int} argument.
		/// <para>
		/// The representation is exactly the one returned by the
		/// {@code Integer.toString} method of one argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="i">   an {@code int}. </param>
		/// <returns>  a string representation of the {@code int} argument. </returns>
		/// <seealso cref=     java.lang.Integer#toString(int, int) </seealso>
		public static String ValueOf(int i)
		{
			return Convert.ToString(i);
		}

		/// <summary>
		/// Returns the string representation of the {@code long} argument.
		/// <para>
		/// The representation is exactly the one returned by the
		/// {@code Long.toString} method of one argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">   a {@code long}. </param>
		/// <returns>  a string representation of the {@code long} argument. </returns>
		/// <seealso cref=     java.lang.Long#toString(long) </seealso>
		public static String ValueOf(long l)
		{
			return Convert.ToString(l);
		}

		/// <summary>
		/// Returns the string representation of the {@code float} argument.
		/// <para>
		/// The representation is exactly the one returned by the
		/// {@code Float.toString} method of one argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="f">   a {@code float}. </param>
		/// <returns>  a string representation of the {@code float} argument. </returns>
		/// <seealso cref=     java.lang.Float#toString(float) </seealso>
		public static String ValueOf(float f)
		{
			return Convert.ToString(f);
		}

		/// <summary>
		/// Returns the string representation of the {@code double} argument.
		/// <para>
		/// The representation is exactly the one returned by the
		/// {@code Double.toString} method of one argument.
		/// 
		/// </para>
		/// </summary>
		/// <param name="d">   a {@code double}. </param>
		/// <returns>  a  string representation of the {@code double} argument. </returns>
		/// <seealso cref=     java.lang.Double#toString(double) </seealso>
		public static String ValueOf(double d)
		{
			return Convert.ToString(d);
		}

		/// <summary>
		/// Returns a canonical representation for the string object.
		/// <para>
		/// A pool of strings, initially empty, is maintained privately by the
		/// class {@code String}.
		/// </para>
		/// <para>
		/// When the intern method is invoked, if the pool already contains a
		/// string equal to this {@code String} object as determined by
		/// the <seealso cref="#equals(Object)"/> method, then the string from the pool is
		/// returned. Otherwise, this {@code String} object is added to the
		/// pool and a reference to this {@code String} object is returned.
		/// </para>
		/// <para>
		/// It follows that for any two strings {@code s} and {@code t},
		/// {@code s.intern() == t.intern()} is {@code true}
		/// if and only if {@code s.equals(t)} is {@code true}.
		/// </para>
		/// <para>
		/// All literal strings and string-valued constant expressions are
		/// interned. String literals are defined in section 3.10.5 of the
		/// <cite>The Java&trade; Language Specification</cite>.
		/// 
		/// </para>
		/// </summary>
		/// <returns>  a string that has the same contents as this string, but is
		///          guaranteed to be from a pool of unique strings. </returns>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		public extern String intern();
	}

}