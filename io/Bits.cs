/*
 * Copyright (c) 2001, 2010, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	/// <summary>
	/// Utility methods for packing/unpacking primitive values in/out of byte arrays
	/// using big-endian byte ordering.
	/// </summary>
	internal class Bits
	{

		/*
		 * Methods for unpacking primitive values from byte arrays starting at
		 * given offsets.
		 */

		internal static bool GetBoolean(sbyte[] b, int off)
		{
			return b[off] != 0;
		}

		internal static char GetChar(sbyte[] b, int off)
		{
			return (char)((b[off + 1] & 0xFF) + (b[off] << 8));
		}

		internal static short GetShort(sbyte[] b, int off)
		{
			return (short)((b[off + 1] & 0xFF) + (b[off] << 8));
		}

		internal static int GetInt(sbyte[] b, int off)
		{
			return ((b[off + 3] & 0xFF)) + ((b[off + 2] & 0xFF) << 8) + ((b[off + 1] & 0xFF) << 16) + ((b[off]) << 24);
		}

		internal static float GetFloat(sbyte[] b, int off)
		{
			return Float.intBitsToFloat(GetInt(b, off));
		}

		internal static long GetLong(sbyte[] b, int off)
		{
			return ((b[off + 7] & 0xFFL)) + ((b[off + 6] & 0xFFL) << 8) + ((b[off + 5] & 0xFFL) << 16) + ((b[off + 4] & 0xFFL) << 24) + ((b[off + 3] & 0xFFL) << 32) + ((b[off + 2] & 0xFFL) << 40) + ((b[off + 1] & 0xFFL) << 48) + (((long) b[off]) << 56);
		}

		internal static double GetDouble(sbyte[] b, int off)
		{
			return Double.longBitsToDouble(GetLong(b, off));
		}

		/*
		 * Methods for packing primitive values into byte arrays starting at given
		 * offsets.
		 */

		internal static void PutBoolean(sbyte[] b, int off, bool val)
		{
			b[off] = (sbyte)(val ? 1 : 0);
		}

		internal static void PutChar(sbyte[] b, int off, char val)
		{
			b[off + 1] = (sbyte)(val);
			b[off] = (sbyte)((int)((uint)val >> 8));
		}

		internal static void PutShort(sbyte[] b, int off, short val)
		{
			b[off + 1] = (sbyte)(val);
			b[off] = (sbyte)((short)((ushort)val >> 8));
		}

		internal static void PutInt(sbyte[] b, int off, int val)
		{
			b[off + 3] = (sbyte)(val);
			b[off + 2] = (sbyte)((int)((uint)val >> 8));
			b[off + 1] = (sbyte)((int)((uint)val >> 16));
			b[off] = (sbyte)((int)((uint)val >> 24));
		}

		internal static void PutFloat(sbyte[] b, int off, float val)
		{
			PutInt(b, off, Float.FloatToIntBits(val));
		}

		internal static void PutLong(sbyte[] b, int off, long val)
		{
			b[off + 7] = (sbyte)(val);
			b[off + 6] = (sbyte)((long)((ulong)val >> 8));
			b[off + 5] = (sbyte)((long)((ulong)val >> 16));
			b[off + 4] = (sbyte)((long)((ulong)val >> 24));
			b[off + 3] = (sbyte)((long)((ulong)val >> 32));
			b[off + 2] = (sbyte)((long)((ulong)val >> 40));
			b[off + 1] = (sbyte)((long)((ulong)val >> 48));
			b[off] = (sbyte)((long)((ulong)val >> 56));
		}

		internal static void PutDouble(sbyte[] b, int off, double val)
		{
			PutLong(b, off, Double.DoubleToLongBits(val));
		}
	}

}