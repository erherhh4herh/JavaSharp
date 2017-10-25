using System;

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
	/// A thread-safe, mutable sequence of characters.
	/// A string buffer is like a <seealso cref="String"/>, but can be modified. At any
	/// point in time it contains some particular sequence of characters, but
	/// the length and content of the sequence can be changed through certain
	/// method calls.
	/// <para>
	/// String buffers are safe for use by multiple threads. The methods
	/// are synchronized where necessary so that all the operations on any
	/// particular instance behave as if they occur in some serial order
	/// that is consistent with the order of the method calls made by each of
	/// the individual threads involved.
	/// </para>
	/// <para>
	/// The principal operations on a {@code StringBuffer} are the
	/// {@code append} and {@code insert} methods, which are
	/// overloaded so as to accept data of any type. Each effectively
	/// converts a given datum to a string and then appends or inserts the
	/// characters of that string to the string buffer. The
	/// {@code append} method always adds these characters at the end
	/// of the buffer; the {@code insert} method adds the characters at
	/// a specified point.
	/// </para>
	/// <para>
	/// For example, if {@code z} refers to a string buffer object
	/// whose current contents are {@code "start"}, then
	/// the method call {@code z.append("le")} would cause the string
	/// buffer to contain {@code "startle"}, whereas
	/// {@code z.insert(4, "le")} would alter the string buffer to
	/// contain {@code "starlet"}.
	/// </para>
	/// <para>
	/// In general, if sb refers to an instance of a {@code StringBuffer},
	/// then {@code sb.append(x)} has the same effect as
	/// {@code sb.insert(sb.length(), x)}.
	/// </para>
	/// <para>
	/// Whenever an operation occurs involving a source sequence (such as
	/// appending or inserting from a source sequence), this class synchronizes
	/// only on the string buffer performing the operation, not on the source.
	/// Note that while {@code StringBuffer} is designed to be safe to use
	/// concurrently from multiple threads, if the constructor or the
	/// {@code append} or {@code insert} operation is passed a source sequence
	/// that is shared across threads, the calling code must ensure
	/// that the operation has a consistent and unchanging view of the source
	/// sequence for the duration of the operation.
	/// This could be satisfied by the caller holding a lock during the
	/// operation's call, by using an immutable source sequence, or by not
	/// sharing the source sequence across threads.
	/// </para>
	/// <para>
	/// Every string buffer has a capacity. As long as the length of the
	/// character sequence contained in the string buffer does not exceed
	/// the capacity, it is not necessary to allocate a new internal
	/// buffer array. If the internal buffer overflows, it is
	/// automatically made larger.
	/// </para>
	/// <para>
	/// Unless otherwise noted, passing a {@code null} argument to a constructor
	/// or method in this class will cause a <seealso cref="NullPointerException"/> to be
	/// thrown.
	/// </para>
	/// <para>
	/// As of  release JDK 5, this class has been supplemented with an equivalent
	/// class designed for use by a single thread, <seealso cref="StringBuilder"/>.  The
	/// {@code StringBuilder} class should generally be used in preference to
	/// this one, as it supports all of the same operations but it is faster, as
	/// it performs no synchronization.
	/// 
	/// @author      Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.StringBuilder </seealso>
	/// <seealso cref=     java.lang.String
	/// @since   JDK1.0 </seealso>
	 [Serializable]
	 public sealed class StringBuffer : AbstractStringBuilder, CharSequence
	 {

		/// <summary>
		/// A cache of the last value returned by toString. Cleared
		/// whenever the StringBuffer is modified.
		/// </summary>
		[NonSerialized]
		private char[] ToStringCache;

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		internal const long SerialVersionUID = 3388685877147921107L;

		/// <summary>
		/// Constructs a string buffer with no characters in it and an
		/// initial capacity of 16 characters.
		/// </summary>
		public StringBuffer() : base(16)
		{
		}

		/// <summary>
		/// Constructs a string buffer with no characters in it and
		/// the specified initial capacity.
		/// </summary>
		/// <param name="capacity">  the initial capacity. </param>
		/// <exception cref="NegativeArraySizeException">  if the {@code capacity}
		///               argument is less than {@code 0}. </exception>
		public StringBuffer(int capacity) : base(capacity)
		{
		}

		/// <summary>
		/// Constructs a string buffer initialized to the contents of the
		/// specified string. The initial capacity of the string buffer is
		/// {@code 16} plus the length of the string argument.
		/// </summary>
		/// <param name="str">   the initial contents of the buffer. </param>
		public StringBuffer(String str) : base(str.Length() + 16)
		{
			Append(str);
		}

		/// <summary>
		/// Constructs a string buffer that contains the same characters
		/// as the specified {@code CharSequence}. The initial capacity of
		/// the string buffer is {@code 16} plus the length of the
		/// {@code CharSequence} argument.
		/// <para>
		/// If the length of the specified {@code CharSequence} is
		/// less than or equal to zero, then an empty buffer of capacity
		/// {@code 16} is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="seq">   the sequence to copy.
		/// @since 1.5 </param>
		public StringBuffer(CharSequence seq) : this(seq.Length() + 16)
		{
			Append(seq);
		}

		public override int Length()
		{
			lock (this)
			{
				return Count;
			}
		}

		public override int Capacity()
		{
			lock (this)
			{
				return Value_Renamed.Length;
			}
		}


		public override void EnsureCapacity(int minimumCapacity)
		{
			lock (this)
			{
				if (minimumCapacity > Value_Renamed.Length)
				{
					ExpandCapacity(minimumCapacity);
				}
			}
		}

		/// <summary>
		/// @since      1.5
		/// </summary>
		public override void TrimToSize()
		{
			lock (this)
			{
				base.TrimToSize();
			}
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		/// <seealso cref=        #length() </seealso>
		public override int Length
		{
			set
			{
				lock (this)
				{
					ToStringCache = null;
					base.Length = value;
				}
			}
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		/// <seealso cref=        #length() </seealso>
		public override char CharAt(int index)
		{
			lock (this)
			{
				if ((index < 0) || (index >= Count))
				{
					throw new StringIndexOutOfBoundsException(index);
				}
				return Value_Renamed[index];
			}
		}

		/// <summary>
		/// @since      1.5
		/// </summary>
		public override int CodePointAt(int index)
		{
			lock (this)
			{
				return base.CodePointAt(index);
			}
		}

		/// <summary>
		/// @since     1.5
		/// </summary>
		public override int CodePointBefore(int index)
		{
			lock (this)
			{
				return base.CodePointBefore(index);
			}
		}

		/// <summary>
		/// @since     1.5
		/// </summary>
		public override int CodePointCount(int beginIndex, int endIndex)
		{
			lock (this)
			{
				return base.CodePointCount(beginIndex, endIndex);
			}
		}

		/// <summary>
		/// @since     1.5
		/// </summary>
		public override int OffsetByCodePoints(int index, int codePointOffset)
		{
			lock (this)
			{
				return base.OffsetByCodePoints(index, codePointOffset);
			}
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override void GetChars(int srcBegin, int srcEnd, char[] dst, int dstBegin)
		{
			lock (this)
			{
				base.GetChars(srcBegin, srcEnd, dst, dstBegin);
			}
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		/// <seealso cref=        #length() </seealso>
		public override void SetCharAt(int index, char ch)
		{
			lock (this)
			{
				if ((index < 0) || (index >= Count))
				{
					throw new StringIndexOutOfBoundsException(index);
				}
				ToStringCache = null;
				Value_Renamed[index] = ch;
			}
		}

		public override StringBuffer Append(Object obj)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(Convert.ToString(obj));
				return this;
			}
		}

		public override StringBuffer Append(String str)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(str);
				return this;
			}
		}

		/// <summary>
		/// Appends the specified {@code StringBuffer} to this sequence.
		/// <para>
		/// The characters of the {@code StringBuffer} argument are appended,
		/// in order, to the contents of this {@code StringBuffer}, increasing the
		/// length of this {@code StringBuffer} by the length of the argument.
		/// If {@code sb} is {@code null}, then the four characters
		/// {@code "null"} are appended to this {@code StringBuffer}.
		/// </para>
		/// <para>
		/// Let <i>n</i> be the length of the old character sequence, the one
		/// contained in the {@code StringBuffer} just prior to execution of the
		/// {@code append} method. Then the character at index <i>k</i> in
		/// the new character sequence is equal to the character at index <i>k</i>
		/// in the old character sequence, if <i>k</i> is less than <i>n</i>;
		/// otherwise, it is equal to the character at index <i>k-n</i> in the
		/// argument {@code sb}.
		/// </para>
		/// <para>
		/// This method synchronizes on {@code this}, the destination
		/// object, but does not synchronize on the source ({@code sb}).
		/// 
		/// </para>
		/// </summary>
		/// <param name="sb">   the {@code StringBuffer} to append. </param>
		/// <returns>  a reference to this object.
		/// @since 1.4 </returns>
		public override StringBuffer Append(StringBuffer sb)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(sb);
				return this;
			}
		}

		/// <summary>
		/// @since 1.8
		/// </summary>
		internal override StringBuffer Append(AbstractStringBuilder asb)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(asb);
				return this;
			}
		}

		/// <summary>
		/// Appends the specified {@code CharSequence} to this
		/// sequence.
		/// <para>
		/// The characters of the {@code CharSequence} argument are appended,
		/// in order, increasing the length of this sequence by the length of the
		/// argument.
		/// 
		/// </para>
		/// <para>The result of this method is exactly the same as if it were an
		/// invocation of this.append(s, 0, s.length());
		/// 
		/// </para>
		/// <para>This method synchronizes on {@code this}, the destination
		/// object, but does not synchronize on the source ({@code s}).
		/// 
		/// </para>
		/// <para>If {@code s} is {@code null}, then the four characters
		/// {@code "null"} are appended.
		/// 
		/// </para>
		/// </summary>
		/// <param name="s"> the {@code CharSequence} to append. </param>
		/// <returns>  a reference to this object.
		/// @since 1.5 </returns>
		public override StringBuffer Append(CharSequence s)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(s);
				return this;
			}
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.5 </exception>
		public override StringBuffer Append(CharSequence s, int start, int end)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(s, start, end);
				return this;
			}
		}

		public override StringBuffer Append(char[] str)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(str);
				return this;
			}
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Append(char[] str, int offset, int len)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(str, offset, len);
				return this;
			}
		}

		public override StringBuffer Append(bool b)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(b);
				return this;
			}
		}

		public override StringBuffer Append(char c)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(c);
				return this;
			}
		}

		public override StringBuffer Append(int i)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(i);
				return this;
			}
		}

		/// <summary>
		/// @since 1.5
		/// </summary>
		public override StringBuffer AppendCodePoint(int codePoint)
		{
			lock (this)
			{
				ToStringCache = null;
				base.AppendCodePoint(codePoint);
				return this;
			}
		}

		public override StringBuffer Append(long lng)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(lng);
				return this;
			}
		}

		public override StringBuffer Append(float f)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(f);
				return this;
			}
		}

		public override StringBuffer Append(double d)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Append(d);
				return this;
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.2 </exception>
		public override StringBuffer Delete(int start, int end)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Delete(start, end);
				return this;
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.2 </exception>
		public override StringBuffer DeleteCharAt(int index)
		{
			lock (this)
			{
				ToStringCache = null;
				base.DeleteCharAt(index);
				return this;
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.2 </exception>
		public override StringBuffer Replace(int start, int end, String str)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Replace(start, end, str);
				return this;
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.2 </exception>
		public override String Substring(int start)
		{
			lock (this)
			{
				return Substring(start, Count);
			}
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.4 </exception>
		public override CharSequence SubSequence(int start, int end)
		{
			lock (this)
			{
				return base.Substring(start, end - start);
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.2 </exception>
		public override String Substring(int start, int end)
		{
			lock (this)
			{
				return base.Substring(start, end - start);
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.2 </exception>
		public override StringBuffer Insert(int index, char[] str, int offset, int len)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Insert(index, str, offset, len);
				return this;
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Insert(int offset, Object obj)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Insert(offset, Convert.ToString(obj));
				return this;
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Insert(int offset, String str)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Insert(offset, str);
				return this;
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Insert(int offset, char[] str)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Insert(offset, str);
				return this;
			}
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.5 </exception>
		public override StringBuffer Insert(int dstOffset, CharSequence s)
		{
			// Note, synchronization achieved via invocations of other StringBuffer methods
			// after narrowing of s to specific type
			// Ditto for toStringCache clearing
			base.Insert(dstOffset, s);
			return this;
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc}
		/// @since      1.5 </exception>
		public override StringBuffer Insert(int dstOffset, CharSequence s, int start, int end)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Insert(dstOffset, s, start, end);
				return this;
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Insert(int offset, bool b)
		{
			// Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			// after conversion of b to String by super class method
			// Ditto for toStringCache clearing
			base.Insert(offset, b);
			return this;
		}

		/// <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Insert(int offset, char c)
		{
			lock (this)
			{
				ToStringCache = null;
				base.Insert(offset, c);
				return this;
			}
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Insert(int offset, int i)
		{
			// Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			// after conversion of i to String by super class method
			// Ditto for toStringCache clearing
			base.Insert(offset, i);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Insert(int offset, long l)
		{
			// Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			// after conversion of l to String by super class method
			// Ditto for toStringCache clearing
			base.Insert(offset, l);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Insert(int offset, float f)
		{
			// Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			// after conversion of f to String by super class method
			// Ditto for toStringCache clearing
			base.Insert(offset, f);
			return this;
		}

		/// <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		public override StringBuffer Insert(int offset, double d)
		{
			// Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			// after conversion of d to String by super class method
			// Ditto for toStringCache clearing
			base.Insert(offset, d);
			return this;
		}

		/// <summary>
		/// @since      1.4
		/// </summary>
		public override int IndexOf(String str)
		{
			// Note, synchronization achieved via invocations of other StringBuffer methods
			return base.IndexOf(str);
		}

		/// <summary>
		/// @since      1.4
		/// </summary>
		public override int IndexOf(String str, int fromIndex)
		{
			lock (this)
			{
				return base.IndexOf(str, fromIndex);
			}
		}

		/// <summary>
		/// @since      1.4
		/// </summary>
		public override int LastIndexOf(String str)
		{
			// Note, synchronization achieved via invocations of other StringBuffer methods
			return LastIndexOf(str, Count);
		}

		/// <summary>
		/// @since      1.4
		/// </summary>
		public override int LastIndexOf(String str, int fromIndex)
		{
			lock (this)
			{
				return base.LastIndexOf(str, fromIndex);
			}
		}

		/// <summary>
		/// @since   JDK1.0.2
		/// </summary>
		public override StringBuffer Reverse()
		{
			lock (this)
			{
				ToStringCache = null;
				base.Reverse();
				return this;
			}
		}

		public override String ToString()
		{
			lock (this)
			{
				if (ToStringCache == null)
				{
					ToStringCache = Arrays.CopyOfRange(Value_Renamed, 0, Count);
				}
				return new String(ToStringCache, true);
			}
		}

		/// <summary>
		/// Serializable fields for StringBuffer.
		/// 
		/// @serialField value  char[]
		///              The backing character array of this StringBuffer.
		/// @serialField count int
		///              The number of characters in this StringBuffer.
		/// @serialField shared  boolean
		///              A flag indicating whether the backing array is shared.
		///              The value is ignored upon deserialization.
		/// </summary>
		private static readonly java.io.ObjectStreamField[] SerialPersistentFields = new java.io.ObjectStreamField[] {new java.io.ObjectStreamField("value", typeof(char[])), new java.io.ObjectStreamField("count", Integer.TYPE), new java.io.ObjectStreamField("shared", Boolean.TYPE)};

		/// <summary>
		/// readObject is called to restore the state of the StringBuffer from
		/// a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(java.io.ObjectOutputStream s)
		{
			lock (this)
			{
				java.io.ObjectOutputStream.PutField fields = s.PutFields();
				fields.Put("value", Value_Renamed);
				fields.Put("count", Count);
				fields.Put("shared", false);
				s.WriteFields();
			}
		}

		/// <summary>
		/// readObject is called to restore the state of the StringBuffer from
		/// a stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
		private void ReadObject(java.io.ObjectInputStream s)
		{
			java.io.ObjectInputStream.GetField fields = s.ReadFields();
			Value_Renamed = (char[])fields.Get("value", null);
			Count = fields.Get("count", 0);
		}
	 }

}