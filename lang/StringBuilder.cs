using System;

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

namespace java.lang
{


	/// <summary>
	/// A mutable sequence of characters.  This class provides an API compatible
	/// with {@code StringBuffer}, but with no guarantee of synchronization.
	/// This class is designed for use as a drop-in replacement for
	/// {@code StringBuffer} in places where the string buffer was being
	/// used by a single thread (as is generally the case).   Where possible,
	/// it is recommended that this class be used in preference to
	/// {@code StringBuffer} as it will be faster under most implementations.
	/// 
	/// <para>The principal operations on a {@code StringBuilder} are the
	/// {@code append} and {@code insert} methods, which are
	/// overloaded so as to accept data of any type. Each effectively
	/// converts a given datum to a string and then appends or inserts the
	/// characters of that string to the string builder. The
	/// {@code append} method always adds these characters at the end
	/// of the builder; the {@code insert} method adds the characters at
	/// a specified point.
	/// </para>
	/// <para>
	/// For example, if {@code z} refers to a string builder object
	/// whose current contents are "{@code start}", then
	/// the method call {@code z.append("le")} would cause the string
	/// builder to contain "{@code startle}", whereas
	/// {@code z.insert(4, "le")} would alter the string builder to
	/// contain "{@code starlet}".
	/// </para>
	/// <para>
	/// In general, if sb refers to an instance of a {@code StringBuilder},
	/// then {@code sb.append(x)} has the same effect as
	/// {@code sb.insert(sb.length(), x)}.
	/// </para>
	/// <para>
	/// Every string builder has a capacity. As long as the length of the
	/// character sequence contained in the string builder does not exceed
	/// the capacity, it is not necessary to allocate a new internal
	/// buffer. If the internal buffer overflows, it is automatically made larger.
	/// 
	/// </para>
	/// <para>Instances of {@code StringBuilder} are not safe for
	/// use by multiple threads. If such synchronization is required then it is
	/// recommended that <seealso cref="java.lang.StringBuffer"/> be used.
	/// 
	/// </para>
	/// <para>Unless otherwise noted, passing a {@code null} argument to a constructor
	/// or method in this class will cause a <seealso cref="NullPointerException"/> to be
	/// thrown.
	/// 
	/// @author      Michael McCloskey
	/// </para>
	/// </summary>
	/// <seealso cref=         java.lang.StringBuffer </seealso>
	/// <seealso cref=         java.lang.String
	/// @since       1.5 </seealso>
	[Serializable]
	public sealed class StringBuilder : AbstractStringBuilder, CharSequence
	{

		/// <summary>
		/// use serialVersionUID for interoperability </summary>
		internal const long SerialVersionUID = 4383685877147921099L;

		/// <summary>
		/// Constructs a string builder with no characters in it and an
		/// initial capacity of 16 characters.
		/// </summary>
		public StringBuilder() : base(16)
		{
		}

		/// <summary>
		/// Constructs a string builder with no characters in it and an
		/// initial capacity specified by the {@code capacity} argument.
		/// </summary>
		/// <param name="capacity">  the initial capacity. </param>
		/// <exception cref="NegativeArraySizeException">  if the {@code capacity}
		///               argument is less than {@code 0}. </exception>
		public StringBuilder(int capacity) : base(capacity)
		{
		}

		/// <summary>
		/// Constructs a string builder initialized to the contents of the
		/// specified string. The initial capacity of the string builder is
		/// {@code 16} plus the length of the string argument.
		/// </summary>
		/// <param name="str">   the initial contents of the buffer. </param>
		public StringBuilder(String str) : base(str.Length() + 16)
		{
			Append(str);
		}

		/// <summary>
		/// Constructs a string builder that contains the same characters
		/// as the specified {@code CharSequence}. The initial capacity of
		/// the string builder is {@code 16} plus the length of the
		/// {@code CharSequence} argument.
		/// </summary>
		/// <param name="seq">   the sequence to copy. </param>
		public StringBuilder(CharSequence seq) : this(seq.Length() + 16)
		{
			Append(seq);
		}

		public override StringBuilder Append(Object obj)
		{
			return Append(Convert.ToString(obj));
		}

		public override StringBuilder Append(String str)
		{
			base.Append(str);
			return this;
		}

		/// <summary>
		/// Appends the specified {@code StringBuffer} to this sequence.
		/// <para>
		/// The characters of the {@code StringBuffer} argument are appended,
		/// in order, to this sequence, increasing the
		/// length of this sequence by the length of the argument.
		/// If {@code sb} is {@code null}, then the four characters
		/// {@code "null"} are appended to this sequence.
		/// </para>
		/// <para>
		/// Let <i>n</i> be the length of this character sequence just prior to
		/// execution of the {@code append} method. Then the character at index
		/// <i>k</i> in the new character sequence is equal to the character at
		/// index <i>k</i> in the old character sequence, if <i>k</i> is less than
		/// <i>n</i>; otherwise, it is equal to the character at index <i>k-n</i>
		/// in the argument {@code sb}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="sb">   the {@code StringBuffer} to append. </param>
		/// <returns>  a reference to this object. </returns>
		public override StringBuilder Append(StringBuffer sb)
		{
			base.Append(sb);
			return this;
		}

		public override StringBuilder Append(CharSequence s)
		{
			base.Append(s);
			return this;
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Append(CharSequence s, int start, int end)
		{
			base.Append(s, start, end);
			return this;
		}

		public override StringBuilder Append(char[] str)
		{
			base.Append(str);
			return this;
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Append(char[] str, int offset, int len)
		{
			base.Append(str, offset, len);
			return this;
		}

		public override StringBuilder Append(bool b)
		{
			base.Append(b);
			return this;
		}

		public override StringBuilder Append(char c)
		{
			base.Append(c);
			return this;
		}

		public override StringBuilder Append(int i)
		{
			base.Append(i);
			return this;
		}

		public override StringBuilder Append(long lng)
		{
			base.Append(lng);
			return this;
		}

		public override StringBuilder Append(float f)
		{
			base.Append(f);
			return this;
		}

		public override StringBuilder Append(double d)
		{
			base.Append(d);
			return this;
		}

		/// <summary>
		/// @since 1.5
		/// </summary>
		public override StringBuilder AppendCodePoint(int codePoint)
		{
			base.AppendCodePoint(codePoint);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Delete(int start, int end)
		{
			base.Delete(start, end);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder DeleteCharAt(int index)
		{
			base.DeleteCharAt(index);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Replace(int start, int end, String str)
		{
			base.Replace(start, end, str);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int index, char[] str, int offset, int len)
		{
			base.Insert(index, str, offset, len);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int offset, Object obj)
		{
				base.Insert(offset, obj);
				return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int offset, String str)
		{
			base.Insert(offset, str);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int offset, char[] str)
		{
			base.Insert(offset, str);
			return this;
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int dstOffset, CharSequence s)
		{
				base.Insert(dstOffset, s);
				return this;
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int dstOffset, CharSequence s, int start, int end)
		{
			base.Insert(dstOffset, s, start, end);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int offset, bool b)
		{
			base.Insert(offset, b);
			return this;
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int offset, char c)
		{
			base.Insert(offset, c);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int offset, int i)
		{
			base.Insert(offset, i);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int offset, long l)
		{
			base.Insert(offset, l);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int offset, float f)
		{
			base.Insert(offset, f);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuilder Insert(int offset, double d)
		{
			base.Insert(offset, d);
			return this;
		}

		public override int IndexOf(String str)
		{
			return base.IndexOf(str);
		}

		public override int IndexOf(String str, int fromIndex)
		{
			return base.IndexOf(str, fromIndex);
		}

		public override int LastIndexOf(String str)
		{
			return base.LastIndexOf(str);
		}

		public override int LastIndexOf(String str, int fromIndex)
		{
			return base.LastIndexOf(str, fromIndex);
		}

		public override StringBuilder Reverse()
		{
			base.Reverse();
			return this;
		}

		public override String ToString()
		{
			// Create a copy, don't share the array
			return new String(Value_Renamed, 0, Count);
		}

		/// <summary>
		/// Save the state of the {@code StringBuilder} instance to a stream
		/// (that is, serialize it).
		/// 
		/// @serialData the number of characters currently stored in the string
		///             builder ({@code int}), followed by the characters in the
		///             string builder ({@code char[]}).   The length of the
		///             {@code char} array may be greater than the number of
		///             characters currently stored in the string builder, in which
		///             case extra characters are ignored.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			s.DefaultWriteObject();
			s.WriteInt(Count);
			s.WriteObject(Value_Renamed);
		}

		/// <summary>
		/// readObject is called to restore the state of the StringBuffer from
		/// a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			s.DefaultReadObject();
			Count = s.ReadInt();
			Value_Renamed = (char[]) s.ReadObject();
		}

	}

}