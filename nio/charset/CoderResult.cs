using System;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.nio.charset
{



	/// <summary>
	/// A description of the result state of a coder.
	/// 
	/// <para> A charset coder, that is, either a decoder or an encoder, consumes bytes
	/// (or characters) from an input buffer, translates them, and writes the
	/// resulting characters (or bytes) to an output buffer.  A coding process
	/// terminates for one of four categories of reasons, which are described by
	/// instances of this class:
	/// 
	/// <ul>
	/// 
	/// </para>
	///   <li><para> <i>Underflow</i> is reported when there is no more input to be
	///   processed, or there is insufficient input and additional input is
	///   required.  This condition is represented by the unique result object
	///   <seealso cref="#UNDERFLOW"/>, whose <seealso cref="#isUnderflow() isUnderflow"/> method
	///   returns <tt>true</tt>.  </para></li>
	/// 
	///   <li><para> <i>Overflow</i> is reported when there is insufficient room
	///   remaining in the output buffer.  This condition is represented by the
	///   unique result object <seealso cref="#OVERFLOW"/>, whose {@link #isOverflow()
	///   isOverflow} method returns <tt>true</tt>.  </para></li>
	/// 
	///   <li><para> A <i>malformed-input error</i> is reported when a sequence of
	///   input units is not well-formed.  Such errors are described by instances of
	///   this class whose <seealso cref="#isMalformed() isMalformed"/> method returns
	///   <tt>true</tt> and whose <seealso cref="#length() length"/> method returns the length
	///   of the malformed sequence.  There is one unique instance of this class for
	///   all malformed-input errors of a given length.  </para></li>
	/// 
	///   <li><para> An <i>unmappable-character error</i> is reported when a sequence
	///   of input units denotes a character that cannot be represented in the
	///   output charset.  Such errors are described by instances of this class
	///   whose <seealso cref="#isUnmappable() isUnmappable"/> method returns <tt>true</tt> and
	///   whose <seealso cref="#length() length"/> method returns the length of the input
	///   sequence denoting the unmappable character.  There is one unique instance
	///   of this class for all unmappable-character errors of a given length.
	///   </para></li>
	/// 
	/// </ul>
	/// 
	/// <para> For convenience, the <seealso cref="#isError() isError"/> method returns <tt>true</tt>
	/// for result objects that describe malformed-input and unmappable-character
	/// errors but <tt>false</tt> for those that describe underflow or overflow
	/// conditions.  </para>
	/// 
	/// 
	/// @author Mark Reinhold
	/// @author JSR-51 Expert Group
	/// @since 1.4
	/// </summary>

	public class CoderResult
	{

		private const int CR_UNDERFLOW = 0;
		private const int CR_OVERFLOW = 1;
		private const int CR_ERROR_MIN = 2;
		private const int CR_MALFORMED = 2;
		private const int CR_UNMAPPABLE = 3;

		private static readonly String[] Names = new String[] {"UNDERFLOW", "OVERFLOW", "MALFORMED", "UNMAPPABLE"};

		private readonly int Type;
		private readonly int Length_Renamed;

		private CoderResult(int type, int length)
		{
			this.Type = type;
			this.Length_Renamed = length;
		}

		/// <summary>
		/// Returns a string describing this coder result.
		/// </summary>
		/// <returns>  A descriptive string </returns>
		public override String ToString()
		{
			String nm = Names[Type];
			return Error ? nm + "[" + Length_Renamed + "]" : nm;
		}

		/// <summary>
		/// Tells whether or not this object describes an underflow condition.
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this object denotes underflow </returns>
		public virtual bool Underflow
		{
			get
			{
				return (Type == CR_UNDERFLOW);
			}
		}

		/// <summary>
		/// Tells whether or not this object describes an overflow condition.
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this object denotes overflow </returns>
		public virtual bool Overflow
		{
			get
			{
				return (Type == CR_OVERFLOW);
			}
		}

		/// <summary>
		/// Tells whether or not this object describes an error condition.
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this object denotes either a
		///          malformed-input error or an unmappable-character error </returns>
		public virtual bool Error
		{
			get
			{
				return (Type >= CR_ERROR_MIN);
			}
		}

		/// <summary>
		/// Tells whether or not this object describes a malformed-input error.
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this object denotes a
		///          malformed-input error </returns>
		public virtual bool Malformed
		{
			get
			{
				return (Type == CR_MALFORMED);
			}
		}

		/// <summary>
		/// Tells whether or not this object describes an unmappable-character
		/// error.
		/// </summary>
		/// <returns>  <tt>true</tt> if, and only if, this object denotes an
		///          unmappable-character error </returns>
		public virtual bool Unmappable
		{
			get
			{
				return (Type == CR_UNMAPPABLE);
			}
		}

		/// <summary>
		/// Returns the length of the erroneous input described by this
		/// object&nbsp;&nbsp;<i>(optional operation)</i>.
		/// </summary>
		/// <returns>  The length of the erroneous input, a positive integer
		/// </returns>
		/// <exception cref="UnsupportedOperationException">
		///          If this object does not describe an error condition, that is,
		///          if the <seealso cref="#isError() isError"/> does not return <tt>true</tt> </exception>
		public virtual int Length()
		{
			if (!Error)
			{
				throw new UnsupportedOperationException();
			}
			return Length_Renamed;
		}

		/// <summary>
		/// Result object indicating underflow, meaning that either the input buffer
		/// has been completely consumed or, if the input buffer is not yet empty,
		/// that additional input is required.
		/// </summary>
		public static readonly CoderResult UNDERFLOW = new CoderResult(CR_UNDERFLOW, 0);

		/// <summary>
		/// Result object indicating overflow, meaning that there is insufficient
		/// room in the output buffer.
		/// </summary>
		public static readonly CoderResult OVERFLOW = new CoderResult(CR_OVERFLOW, 0);

		private abstract class Cache
		{

			internal IDictionary<Integer, WeakReference<CoderResult>> Cache = null;

			protected internal abstract CoderResult Create(int len);

			internal virtual CoderResult Get(int len)
			{
				lock (this)
				{
					if (len <= 0)
					{
						throw new IllegalArgumentException("Non-positive length");
					}
					Integer k = new Integer(len);
					WeakReference<CoderResult> w;
					CoderResult e = null;
					if (Cache == null)
					{
						Cache = new Dictionary<Integer, WeakReference<CoderResult>>();
					}
					else if ((w = Cache[k]) != null)
					{
						e = w.get();
					}
					if (e == null)
					{
						e = Create(len);
						Cache[k] = new WeakReference<CoderResult>(e);
					}
					return e;
				}
			}

		}

		private static Cache malformedCache = new CacheAnonymousInnerClassHelper();

		private class CacheAnonymousInnerClassHelper : Cache
		{
			public CacheAnonymousInnerClassHelper()
			{
			}

			public override CoderResult Create(int len)
			{
				return new CoderResult(CR_MALFORMED, len);
			}
		}

		/// <summary>
		/// Static factory method that returns the unique object describing a
		/// malformed-input error of the given length.
		/// </summary>
		/// <param name="length">
		///          The given length
		/// </param>
		/// <returns>  The requested coder-result object </returns>
		public static CoderResult MalformedForLength(int length)
		{
			return malformedCache.get(length);
		}

		private static Cache unmappableCache = new CacheAnonymousInnerClassHelper2();

		private class CacheAnonymousInnerClassHelper2 : Cache
		{
			public CacheAnonymousInnerClassHelper2()
			{
			}

			public override CoderResult Create(int len)
			{
				return new CoderResult(CR_UNMAPPABLE, len);
			}
		}

		/// <summary>
		/// Static factory method that returns the unique result object describing
		/// an unmappable-character error of the given length.
		/// </summary>
		/// <param name="length">
		///          The given length
		/// </param>
		/// <returns>  The requested coder-result object </returns>
		public static CoderResult UnmappableForLength(int length)
		{
			return unmappableCache.get(length);
		}

		/// <summary>
		/// Throws an exception appropriate to the result described by this object.
		/// </summary>
		/// <exception cref="BufferUnderflowException">
		///          If this object is <seealso cref="#UNDERFLOW"/>
		/// </exception>
		/// <exception cref="BufferOverflowException">
		///          If this object is <seealso cref="#OVERFLOW"/>
		/// </exception>
		/// <exception cref="MalformedInputException">
		///          If this object represents a malformed-input error; the
		///          exception's length value will be that of this object
		/// </exception>
		/// <exception cref="UnmappableCharacterException">
		///          If this object represents an unmappable-character error; the
		///          exceptions length value will be that of this object </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void throwException() throws CharacterCodingException
		public virtual void ThrowException()
		{
			switch (Type)
			{
			case CR_UNDERFLOW:
				throw new BufferUnderflowException();
			case CR_OVERFLOW:
				throw new BufferOverflowException();
			case CR_MALFORMED:
				throw new MalformedInputException(Length_Renamed);
			case CR_UNMAPPABLE:
				throw new UnmappableCharacterException(Length_Renamed);
			default:
				Debug.Assert(false);
			break;
			}
		}

	}

}