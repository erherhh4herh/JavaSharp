using System;

/*
 * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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

	using MessageUtils = sun.misc.MessageUtils;
	using HistoricallyNamedCharset = sun.nio.cs.HistoricallyNamedCharset;
	using ArrayDecoder = sun.nio.cs.ArrayDecoder;
	using ArrayEncoder = sun.nio.cs.ArrayEncoder;

	/// <summary>
	/// Utility class for string encoding and decoding.
	/// </summary>

	internal class StringCoding
	{

		private StringCoding()
		{
		}

		/// <summary>
		/// The cached coders for each thread </summary>
		private static readonly ThreadLocal<SoftReference<StringDecoder>> Decoder = new ThreadLocal<SoftReference<StringDecoder>>();
		private static readonly ThreadLocal<SoftReference<StringEncoder>> Encoder = new ThreadLocal<SoftReference<StringEncoder>>();

		private static bool WarnUnsupportedCharset_Renamed = true;

		private static T deref<T>(ThreadLocal<SoftReference<T>> tl)
		{
			SoftReference<T> sr = tl.Get();
			if (sr == null)
			{
				return null;
			}
			return sr.get();
		}

		private static void set<T>(ThreadLocal<SoftReference<T>> tl, T ob)
		{
			tl.Set(new SoftReference<T>(ob));
		}

		// Trim the given byte array to the given length
		//
		private static sbyte[] SafeTrim(sbyte[] ba, int len, Charset cs, bool isTrusted)
		{
			if (len == ba.Length && (isTrusted || System.SecurityManager == null))
			{
				return ba;
			}
			else
			{
				return Arrays.CopyOf(ba, len);
			}
		}

		// Trim the given char array to the given length
		//
		private static char[] SafeTrim(char[] ca, int len, Charset cs, bool isTrusted)
		{
			if (len == ca.Length && (isTrusted || System.SecurityManager == null))
			{
				return ca;
			}
			else
			{
				return Arrays.CopyOf(ca, len);
			}
		}

		private static int Scale(int len, float expansionFactor)
		{
			// We need to perform double, not float, arithmetic; otherwise
			// we lose low order bits when len is larger than 2**24.
			return (int)(len * (double)expansionFactor);
		}

		private static Charset LookupCharset(String csn)
		{
			if (Charset.IsSupported(csn))
			{
				try
				{
					return Charset.ForName(csn);
				}
				catch (UnsupportedCharsetException x)
				{
					throw new Error(x);
				}
			}
			return null;
		}

		private static void WarnUnsupportedCharset(String csn)
		{
			if (WarnUnsupportedCharset_Renamed)
			{
				// Use sun.misc.MessageUtils rather than the Logging API or
				// System.err since this method may be called during VM
				// initialization before either is available.
				MessageUtils.err("WARNING: Default charset " + csn + " not supported, using ISO-8859-1 instead");
				WarnUnsupportedCharset_Renamed = false;
			}
		}


		// -- Decoding --
		private class StringDecoder
		{
			internal readonly String RequestedCharsetName_Renamed;
			internal readonly Charset Cs;
			internal readonly CharsetDecoder Cd;
			internal readonly bool IsTrusted;

			internal StringDecoder(Charset cs, String rcn)
			{
				this.RequestedCharsetName_Renamed = rcn;
				this.Cs = cs;
				this.Cd = cs.NewDecoder().OnMalformedInput(CodingErrorAction.REPLACE).OnUnmappableCharacter(CodingErrorAction.REPLACE);
				this.IsTrusted = (cs.GetType().ClassLoader0 == null);
			}

			internal virtual String CharsetName()
			{
				if (Cs is HistoricallyNamedCharset)
				{
					return ((HistoricallyNamedCharset)Cs).historicalName();
				}
				return Cs.Name();
			}

			internal String RequestedCharsetName()
			{
				return RequestedCharsetName_Renamed;
			}

			internal virtual char[] Decode(sbyte[] ba, int off, int len)
			{
				int en = Scale(len, Cd.MaxCharsPerByte());
				char[] ca = new char[en];
				if (len == 0)
				{
					return ca;
				}
				if (Cd is ArrayDecoder)
				{
					int clen = ((ArrayDecoder)Cd).decode(ba, off, len, ca);
					return SafeTrim(ca, clen, Cs, IsTrusted);
				}
				else
				{
					Cd.Reset();
					ByteBuffer bb = ByteBuffer.Wrap(ba, off, len);
					CharBuffer cb = CharBuffer.Wrap(ca);
					try
					{
						CoderResult cr = Cd.Decode(bb, cb, true);
						if (!cr.Underflow)
						{
							cr.ThrowException();
						}
						cr = Cd.Flush(cb);
						if (!cr.Underflow)
						{
							cr.ThrowException();
						}
					}
					catch (CharacterCodingException x)
					{
						// Substitution is always enabled,
						// so this shouldn't happen
						throw new Error(x);
					}
					return SafeTrim(ca, cb.Position(), Cs, IsTrusted);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static char[] decode(String charsetName, byte[] ba, int off, int len) throws java.io.UnsupportedEncodingException
		internal static char[] Decode(String charsetName, sbyte[] ba, int off, int len)
		{
			StringDecoder sd = Deref(Decoder);
			String csn = (charsetName == null) ? "ISO-8859-1" : charsetName;
			if ((sd == null) || !(csn.Equals(sd.RequestedCharsetName()) || csn.Equals(sd.CharsetName())))
			{
				sd = null;
				try
				{
					Charset cs = LookupCharset(csn);
					if (cs != null)
					{
						sd = new StringDecoder(cs, csn);
					}
				}
				catch (IllegalCharsetNameException)
				{
				}
				if (sd == null)
				{
					throw new UnsupportedEncodingException(csn);
				}
				Set(Decoder, sd);
			}
			return sd.Decode(ba, off, len);
		}

		internal static char[] Decode(Charset cs, sbyte[] ba, int off, int len)
		{
			// (1)We never cache the "external" cs, the only benefit of creating
			// an additional StringDe/Encoder object to wrap it is to share the
			// de/encode() method. These SD/E objects are short-lifed, the young-gen
			// gc should be able to take care of them well. But the best approash
			// is still not to generate them if not really necessary.
			// (2)The defensive copy of the input byte/char[] has a big performance
			// impact, as well as the outgoing result byte/char[]. Need to do the
			// optimization check of (sm==null && classLoader0==null) for both.
			// (3)getClass().getClassLoader0() is expensive
			// (4)There might be a timing gap in isTrusted setting. getClassLoader0()
			// is only chcked (and then isTrusted gets set) when (SM==null). It is
			// possible that the SM==null for now but then SM is NOT null later
			// when safeTrim() is invoked...the "safe" way to do is to redundant
			// check (... && (isTrusted || SM == null || getClassLoader0())) in trim
			// but it then can be argued that the SM is null when the opertaion
			// is started...
			CharsetDecoder cd = cs.NewDecoder();
			int en = Scale(len, cd.MaxCharsPerByte());
			char[] ca = new char[en];
			if (len == 0)
			{
				return ca;
			}
			bool isTrusted = false;
			if (System.SecurityManager != null)
			{
				if (!(isTrusted = (cs.GetType().ClassLoader0 == null)))
				{
					ba = Arrays.CopyOfRange(ba, off, off + len);
					off = 0;
				}
			}
			cd.OnMalformedInput(CodingErrorAction.REPLACE).OnUnmappableCharacter(CodingErrorAction.REPLACE).Reset();
			if (cd is ArrayDecoder)
			{
				int clen = ((ArrayDecoder)cd).decode(ba, off, len, ca);
				return SafeTrim(ca, clen, cs, isTrusted);
			}
			else
			{
				ByteBuffer bb = ByteBuffer.Wrap(ba, off, len);
				CharBuffer cb = CharBuffer.Wrap(ca);
				try
				{
					CoderResult cr = cd.Decode(bb, cb, true);
					if (!cr.Underflow)
					{
						cr.ThrowException();
					}
					cr = cd.Flush(cb);
					if (!cr.Underflow)
					{
						cr.ThrowException();
					}
				}
				catch (CharacterCodingException x)
				{
					// Substitution is always enabled,
					// so this shouldn't happen
					throw new Error(x);
				}
				return SafeTrim(ca, cb.Position(), cs, isTrusted);
			}
		}

		internal static char[] Decode(sbyte[] ba, int off, int len)
		{
			String csn = Charset.DefaultCharset().Name();
			try
			{
				// use charset name decode() variant which provides caching.
				return Decode(csn, ba, off, len);
			}
			catch (UnsupportedEncodingException)
			{
				WarnUnsupportedCharset(csn);
			}
			try
			{
				return Decode("ISO-8859-1", ba, off, len);
			}
			catch (UnsupportedEncodingException x)
			{
				// If this code is hit during VM initialization, MessageUtils is
				// the only way we will be able to get any kind of error message.
				MessageUtils.err("ISO-8859-1 charset not available: " + x.ToString());
				// If we can not find ISO-8859-1 (a required encoding) then things
				// are seriously wrong with the installation.
				Environment.Exit(1);
				return null;
			}
		}

		// -- Encoding --
		private class StringEncoder
		{
			internal Charset Cs;
			internal CharsetEncoder Ce;
			internal readonly String RequestedCharsetName_Renamed;
			internal readonly bool IsTrusted;

			internal StringEncoder(Charset cs, String rcn)
			{
				this.RequestedCharsetName_Renamed = rcn;
				this.Cs = cs;
				this.Ce = cs.NewEncoder().OnMalformedInput(CodingErrorAction.REPLACE).OnUnmappableCharacter(CodingErrorAction.REPLACE);
				this.IsTrusted = (cs.GetType().ClassLoader0 == null);
			}

			internal virtual String CharsetName()
			{
				if (Cs is HistoricallyNamedCharset)
				{
					return ((HistoricallyNamedCharset)Cs).historicalName();
				}
				return Cs.Name();
			}

			internal String RequestedCharsetName()
			{
				return RequestedCharsetName_Renamed;
			}

			internal virtual sbyte[] Encode(char[] ca, int off, int len)
			{
				int en = Scale(len, Ce.MaxBytesPerChar());
				sbyte[] ba = new sbyte[en];
				if (len == 0)
				{
					return ba;
				}
				if (Ce is ArrayEncoder)
				{
					int blen = ((ArrayEncoder)Ce).encode(ca, off, len, ba);
					return SafeTrim(ba, blen, Cs, IsTrusted);
				}
				else
				{
					Ce.Reset();
					ByteBuffer bb = ByteBuffer.Wrap(ba);
					CharBuffer cb = CharBuffer.Wrap(ca, off, len);
					try
					{
						CoderResult cr = Ce.Encode(cb, bb, true);
						if (!cr.Underflow)
						{
							cr.ThrowException();
						}
						cr = Ce.Flush(bb);
						if (!cr.Underflow)
						{
							cr.ThrowException();
						}
					}
					catch (CharacterCodingException x)
					{
						// Substitution is always enabled,
						// so this shouldn't happen
						throw new Error(x);
					}
					return SafeTrim(ba, bb.Position(), Cs, IsTrusted);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static byte[] encode(String charsetName, char[] ca, int off, int len) throws java.io.UnsupportedEncodingException
		internal static sbyte[] Encode(String charsetName, char[] ca, int off, int len)
		{
			StringEncoder se = Deref(Encoder);
			String csn = (charsetName == null) ? "ISO-8859-1" : charsetName;
			if ((se == null) || !(csn.Equals(se.RequestedCharsetName()) || csn.Equals(se.CharsetName())))
			{
				se = null;
				try
				{
					Charset cs = LookupCharset(csn);
					if (cs != null)
					{
						se = new StringEncoder(cs, csn);
					}
				}
				catch (IllegalCharsetNameException)
				{
				}
				if (se == null)
				{
					throw new UnsupportedEncodingException(csn);
				}
				Set(Encoder, se);
			}
			return se.Encode(ca, off, len);
		}

		internal static sbyte[] Encode(Charset cs, char[] ca, int off, int len)
		{
			CharsetEncoder ce = cs.NewEncoder();
			int en = Scale(len, ce.MaxBytesPerChar());
			sbyte[] ba = new sbyte[en];
			if (len == 0)
			{
				return ba;
			}
			bool isTrusted = false;
			if (System.SecurityManager != null)
			{
				if (!(isTrusted = (cs.GetType().ClassLoader0 == null)))
				{
					ca = Arrays.CopyOfRange(ca, off, off + len);
					off = 0;
				}
			}
			ce.OnMalformedInput(CodingErrorAction.REPLACE).OnUnmappableCharacter(CodingErrorAction.REPLACE).Reset();
			if (ce is ArrayEncoder)
			{
				int blen = ((ArrayEncoder)ce).encode(ca, off, len, ba);
				return SafeTrim(ba, blen, cs, isTrusted);
			}
			else
			{
				ByteBuffer bb = ByteBuffer.Wrap(ba);
				CharBuffer cb = CharBuffer.Wrap(ca, off, len);
				try
				{
					CoderResult cr = ce.Encode(cb, bb, true);
					if (!cr.Underflow)
					{
						cr.ThrowException();
					}
					cr = ce.Flush(bb);
					if (!cr.Underflow)
					{
						cr.ThrowException();
					}
				}
				catch (CharacterCodingException x)
				{
					throw new Error(x);
				}
				return SafeTrim(ba, bb.Position(), cs, isTrusted);
			}
		}

		internal static sbyte[] Encode(char[] ca, int off, int len)
		{
			String csn = Charset.DefaultCharset().Name();
			try
			{
				// use charset name encode() variant which provides caching.
				return Encode(csn, ca, off, len);
			}
			catch (UnsupportedEncodingException)
			{
				WarnUnsupportedCharset(csn);
			}
			try
			{
				return Encode("ISO-8859-1", ca, off, len);
			}
			catch (UnsupportedEncodingException x)
			{
				// If this code is hit during VM initialization, MessageUtils is
				// the only way we will be able to get any kind of error message.
				MessageUtils.err("ISO-8859-1 charset not available: " + x.ToString());
				// If we can not find ISO-8859-1 (a required encoding) then things
				// are seriously wrong with the installation.
				Environment.Exit(1);
				return null;
			}
		}
	}

}