/*
 * Copyright (c) 2009, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.zip
{

	using ArrayDecoder = sun.nio.cs.ArrayDecoder;
	using ArrayEncoder = sun.nio.cs.ArrayEncoder;

	/// <summary>
	/// Utility class for zipfile name and comment decoding and encoding
	/// </summary>

	internal sealed class ZipCoder
	{

		internal String ToString(sbyte[] ba, int length)
		{
			CharsetDecoder cd = Decoder().Reset();
			int len = (int)(length * cd.MaxCharsPerByte());
			char[] ca = new char[len];
			if (len == 0)
			{
				return new String(ca);
			}
			// UTF-8 only for now. Other ArrayDeocder only handles
			// CodingErrorAction.REPLACE mode. ZipCoder uses
			// REPORT mode.
			if (IsUTF8 && cd is ArrayDecoder)
			{
				int clen = ((ArrayDecoder)cd).decode(ba, 0, length, ca);
				if (clen == -1) // malformed
				{
					throw new IllegalArgumentException("MALFORMED");
				}
				return new String(ca, 0, clen);
			}
			ByteBuffer bb = ByteBuffer.Wrap(ba, 0, length);
			CharBuffer cb = CharBuffer.Wrap(ca);
			CoderResult cr = cd.Decode(bb, cb, true);
			if (!cr.Underflow)
			{
				throw new IllegalArgumentException(cr.ToString());
			}
			cr = cd.Flush(cb);
			if (!cr.Underflow)
			{
				throw new IllegalArgumentException(cr.ToString());
			}
			return new String(ca, 0, cb.Position());
		}

		internal String ToString(sbyte[] ba)
		{
			return ToString(ba, ba.Length);
		}

		internal sbyte[] GetBytes(String s)
		{
			CharsetEncoder ce = Encoder().Reset();
			char[] ca = s.ToCharArray();
			int len = (int)(ca.Length * ce.MaxBytesPerChar());
			sbyte[] ba = new sbyte[len];
			if (len == 0)
			{
				return ba;
			}
			// UTF-8 only for now. Other ArrayDeocder only handles
			// CodingErrorAction.REPLACE mode.
			if (IsUTF8 && ce is ArrayEncoder)
			{
				int blen = ((ArrayEncoder)ce).encode(ca, 0, ca.Length, ba);
				if (blen == -1) // malformed
				{
					throw new IllegalArgumentException("MALFORMED");
				}
				return Arrays.CopyOf(ba, blen);
			}
			ByteBuffer bb = ByteBuffer.Wrap(ba);
			CharBuffer cb = CharBuffer.Wrap(ca);
			CoderResult cr = ce.Encode(cb, bb, true);
			if (!cr.Underflow)
			{
				throw new IllegalArgumentException(cr.ToString());
			}
			cr = ce.Flush(bb);
			if (!cr.Underflow)
			{
				throw new IllegalArgumentException(cr.ToString());
			}
			if (bb.Position() == ba.Length) // defensive copy?
			{
				return ba;
			}
			else
			{
				return Arrays.CopyOf(ba, bb.Position());
			}
		}

		// assume invoked only if "this" is not utf8
		internal sbyte[] GetBytesUTF8(String s)
		{
			if (IsUTF8)
			{
				return GetBytes(s);
			}
			if (Utf8 == null)
			{
				Utf8 = new ZipCoder(StandardCharsets.UTF_8);
			}
			return Utf8.GetBytes(s);
		}


		internal String ToStringUTF8(sbyte[] ba, int len)
		{
			if (IsUTF8)
			{
				return ToString(ba, len);
			}
			if (Utf8 == null)
			{
				Utf8 = new ZipCoder(StandardCharsets.UTF_8);
			}
			return Utf8.ToString(ba, len);
		}

		internal bool UTF8
		{
			get
			{
				return IsUTF8;
			}
		}

		private Charset Cs;
		private CharsetDecoder Dec;
		private CharsetEncoder Enc;
		private bool IsUTF8;
		private ZipCoder Utf8;

		private ZipCoder(Charset cs)
		{
			this.Cs = cs;
			this.IsUTF8 = cs.Name().Equals(StandardCharsets.UTF_8.Name());
		}

		internal static ZipCoder Get(Charset charset)
		{
			return new ZipCoder(charset);
		}

		private CharsetDecoder Decoder()
		{
			if (Dec == null)
			{
				Dec = Cs.NewDecoder().OnMalformedInput(CodingErrorAction.REPORT).OnUnmappableCharacter(CodingErrorAction.REPORT);
			}
			return Dec;
		}

		private CharsetEncoder Encoder()
		{
			if (Enc == null)
			{
				Enc = Cs.NewEncoder().OnMalformedInput(CodingErrorAction.REPORT).OnUnmappableCharacter(CodingErrorAction.REPORT);
			}
			return Enc;
		}
	}

}