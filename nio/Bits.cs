using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

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

namespace java.nio
{

	using Unsafe = sun.misc.Unsafe;
	using VM = sun.misc.VM;

	/// <summary>
	/// Access to bits, native and otherwise.
	/// </summary>

	internal class Bits // package-private
	{

		private Bits()
		{
		}


		// -- Swapping --

		internal static short Swap(short x)
		{
			return Short.ReverseBytes(x);
		}

		internal static char Swap(char x)
		{
			return Character.ReverseBytes(x);
		}

		internal static int Swap(int x)
		{
			return Integer.ReverseBytes(x);
		}

		internal static long Swap(long x)
		{
			return Long.ReverseBytes(x);
		}


		// -- get/put char --

		private static char MakeChar(sbyte b1, sbyte b0)
		{
			return (char)((b1 << 8) | (b0 & 0xff));
		}

		internal static char GetCharL(ByteBuffer bb, int bi)
		{
			return MakeChar(bb._get(bi + 1), bb._get(bi));
		}

		internal static char GetCharL(long a)
		{
			return MakeChar(_get(a + 1), _get(a));
		}

		internal static char GetCharB(ByteBuffer bb, int bi)
		{
			return MakeChar(bb._get(bi), bb._get(bi + 1));
		}

		internal static char GetCharB(long a)
		{
			return MakeChar(_get(a), _get(a + 1));
		}

		internal static char GetChar(ByteBuffer bb, int bi, bool bigEndian)
		{
			return bigEndian ? GetCharB(bb, bi) : GetCharL(bb, bi);
		}

		internal static char GetChar(long a, bool bigEndian)
		{
			return bigEndian ? GetCharB(a) : GetCharL(a);
		}

		private static sbyte Char1(char x)
		{
			return (sbyte)(x >> 8);
		}
		private static sbyte Char0(char x)
		{
			return (sbyte)(x);
		}

		internal static void PutCharL(ByteBuffer bb, int bi, char x)
		{
			bb._put(bi, Char0(x));
			bb._put(bi + 1, Char1(x));
		}

		internal static void PutCharL(long a, char x)
		{
			_put(a, Char0(x));
			_put(a + 1, Char1(x));
		}

		internal static void PutCharB(ByteBuffer bb, int bi, char x)
		{
			bb._put(bi, Char1(x));
			bb._put(bi + 1, Char0(x));
		}

		internal static void PutCharB(long a, char x)
		{
			_put(a, Char1(x));
			_put(a + 1, Char0(x));
		}

		internal static void PutChar(ByteBuffer bb, int bi, char x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutCharB(bb, bi, x);
			}
			else
			{
				PutCharL(bb, bi, x);
			}
		}

		internal static void PutChar(long a, char x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutCharB(a, x);
			}
			else
			{
				PutCharL(a, x);
			}
		}


		// -- get/put short --

		private static short MakeShort(sbyte b1, sbyte b0)
		{
			return (short)((b1 << 8) | (b0 & 0xff));
		}

		internal static short GetShortL(ByteBuffer bb, int bi)
		{
			return MakeShort(bb._get(bi + 1), bb._get(bi));
		}

		internal static short GetShortL(long a)
		{
			return MakeShort(_get(a + 1), _get(a));
		}

		internal static short GetShortB(ByteBuffer bb, int bi)
		{
			return MakeShort(bb._get(bi), bb._get(bi + 1));
		}

		internal static short GetShortB(long a)
		{
			return MakeShort(_get(a), _get(a + 1));
		}

		internal static short GetShort(ByteBuffer bb, int bi, bool bigEndian)
		{
			return bigEndian ? GetShortB(bb, bi) : GetShortL(bb, bi);
		}

		internal static short GetShort(long a, bool bigEndian)
		{
			return bigEndian ? GetShortB(a) : GetShortL(a);
		}

		private static sbyte Short1(short x)
		{
			return (sbyte)(x >> 8);
		}
		private static sbyte Short0(short x)
		{
			return (sbyte)(x);
		}

		internal static void PutShortL(ByteBuffer bb, int bi, short x)
		{
			bb._put(bi, Short0(x));
			bb._put(bi + 1, Short1(x));
		}

		internal static void PutShortL(long a, short x)
		{
			_put(a, Short0(x));
			_put(a + 1, Short1(x));
		}

		internal static void PutShortB(ByteBuffer bb, int bi, short x)
		{
			bb._put(bi, Short1(x));
			bb._put(bi + 1, Short0(x));
		}

		internal static void PutShortB(long a, short x)
		{
			_put(a, Short1(x));
			_put(a + 1, Short0(x));
		}

		internal static void PutShort(ByteBuffer bb, int bi, short x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutShortB(bb, bi, x);
			}
			else
			{
				PutShortL(bb, bi, x);
			}
		}

		internal static void PutShort(long a, short x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutShortB(a, x);
			}
			else
			{
				PutShortL(a, x);
			}
		}


		// -- get/put int --

		private static int MakeInt(sbyte b3, sbyte b2, sbyte b1, sbyte b0)
		{
			return (((b3) << 24) | ((b2 & 0xff) << 16) | ((b1 & 0xff) << 8) | ((b0 & 0xff)));
		}

		internal static int GetIntL(ByteBuffer bb, int bi)
		{
			return MakeInt(bb._get(bi + 3), bb._get(bi + 2), bb._get(bi + 1), bb._get(bi));
		}

		internal static int GetIntL(long a)
		{
			return MakeInt(_get(a + 3), _get(a + 2), _get(a + 1), _get(a));
		}

		internal static int GetIntB(ByteBuffer bb, int bi)
		{
			return MakeInt(bb._get(bi), bb._get(bi + 1), bb._get(bi + 2), bb._get(bi + 3));
		}

		internal static int GetIntB(long a)
		{
			return MakeInt(_get(a), _get(a + 1), _get(a + 2), _get(a + 3));
		}

		internal static int GetInt(ByteBuffer bb, int bi, bool bigEndian)
		{
			return bigEndian ? GetIntB(bb, bi) : GetIntL(bb, bi);
		}

		internal static int GetInt(long a, bool bigEndian)
		{
			return bigEndian ? GetIntB(a) : GetIntL(a);
		}

		private static sbyte Int3(int x)
		{
			return (sbyte)(x >> 24);
		}
		private static sbyte Int2(int x)
		{
			return (sbyte)(x >> 16);
		}
		private static sbyte Int1(int x)
		{
			return (sbyte)(x >> 8);
		}
		private static sbyte Int0(int x)
		{
			return (sbyte)(x);
		}

		internal static void PutIntL(ByteBuffer bb, int bi, int x)
		{
			bb._put(bi + 3, Int3(x));
			bb._put(bi + 2, Int2(x));
			bb._put(bi + 1, Int1(x));
			bb._put(bi, Int0(x));
		}

		internal static void PutIntL(long a, int x)
		{
			_put(a + 3, Int3(x));
			_put(a + 2, Int2(x));
			_put(a + 1, Int1(x));
			_put(a, Int0(x));
		}

		internal static void PutIntB(ByteBuffer bb, int bi, int x)
		{
			bb._put(bi, Int3(x));
			bb._put(bi + 1, Int2(x));
			bb._put(bi + 2, Int1(x));
			bb._put(bi + 3, Int0(x));
		}

		internal static void PutIntB(long a, int x)
		{
			_put(a, Int3(x));
			_put(a + 1, Int2(x));
			_put(a + 2, Int1(x));
			_put(a + 3, Int0(x));
		}

		internal static void PutInt(ByteBuffer bb, int bi, int x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutIntB(bb, bi, x);
			}
			else
			{
				PutIntL(bb, bi, x);
			}
		}

		internal static void PutInt(long a, int x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutIntB(a, x);
			}
			else
			{
				PutIntL(a, x);
			}
		}


		// -- get/put long --

		private static long MakeLong(sbyte b7, sbyte b6, sbyte b5, sbyte b4, sbyte b3, sbyte b2, sbyte b1, sbyte b0)
		{
			return ((((long)b7) << 56) | (((long)b6 & 0xff) << 48) | (((long)b5 & 0xff) << 40) | (((long)b4 & 0xff) << 32) | (((long)b3 & 0xff) << 24) | (((long)b2 & 0xff) << 16) | (((long)b1 & 0xff) << 8) | (((long)b0 & 0xff)));
		}

		internal static long GetLongL(ByteBuffer bb, int bi)
		{
			return MakeLong(bb._get(bi + 7), bb._get(bi + 6), bb._get(bi + 5), bb._get(bi + 4), bb._get(bi + 3), bb._get(bi + 2), bb._get(bi + 1), bb._get(bi));
		}

		internal static long GetLongL(long a)
		{
			return MakeLong(_get(a + 7), _get(a + 6), _get(a + 5), _get(a + 4), _get(a + 3), _get(a + 2), _get(a + 1), _get(a));
		}

		internal static long GetLongB(ByteBuffer bb, int bi)
		{
			return MakeLong(bb._get(bi), bb._get(bi + 1), bb._get(bi + 2), bb._get(bi + 3), bb._get(bi + 4), bb._get(bi + 5), bb._get(bi + 6), bb._get(bi + 7));
		}

		internal static long GetLongB(long a)
		{
			return MakeLong(_get(a), _get(a + 1), _get(a + 2), _get(a + 3), _get(a + 4), _get(a + 5), _get(a + 6), _get(a + 7));
		}

		internal static long GetLong(ByteBuffer bb, int bi, bool bigEndian)
		{
			return bigEndian ? GetLongB(bb, bi) : GetLongL(bb, bi);
		}

		internal static long GetLong(long a, bool bigEndian)
		{
			return bigEndian ? GetLongB(a) : GetLongL(a);
		}

		private static sbyte Long7(long x)
		{
			return (sbyte)(x >> 56);
		}
		private static sbyte Long6(long x)
		{
			return (sbyte)(x >> 48);
		}
		private static sbyte Long5(long x)
		{
			return (sbyte)(x >> 40);
		}
		private static sbyte Long4(long x)
		{
			return (sbyte)(x >> 32);
		}
		private static sbyte Long3(long x)
		{
			return (sbyte)(x >> 24);
		}
		private static sbyte Long2(long x)
		{
			return (sbyte)(x >> 16);
		}
		private static sbyte Long1(long x)
		{
			return (sbyte)(x >> 8);
		}
		private static sbyte Long0(long x)
		{
			return (sbyte)(x);
		}

		internal static void PutLongL(ByteBuffer bb, int bi, long x)
		{
			bb._put(bi + 7, Long7(x));
			bb._put(bi + 6, Long6(x));
			bb._put(bi + 5, Long5(x));
			bb._put(bi + 4, Long4(x));
			bb._put(bi + 3, Long3(x));
			bb._put(bi + 2, Long2(x));
			bb._put(bi + 1, Long1(x));
			bb._put(bi, Long0(x));
		}

		internal static void PutLongL(long a, long x)
		{
			_put(a + 7, Long7(x));
			_put(a + 6, Long6(x));
			_put(a + 5, Long5(x));
			_put(a + 4, Long4(x));
			_put(a + 3, Long3(x));
			_put(a + 2, Long2(x));
			_put(a + 1, Long1(x));
			_put(a, Long0(x));
		}

		internal static void PutLongB(ByteBuffer bb, int bi, long x)
		{
			bb._put(bi, Long7(x));
			bb._put(bi + 1, Long6(x));
			bb._put(bi + 2, Long5(x));
			bb._put(bi + 3, Long4(x));
			bb._put(bi + 4, Long3(x));
			bb._put(bi + 5, Long2(x));
			bb._put(bi + 6, Long1(x));
			bb._put(bi + 7, Long0(x));
		}

		internal static void PutLongB(long a, long x)
		{
			_put(a, Long7(x));
			_put(a + 1, Long6(x));
			_put(a + 2, Long5(x));
			_put(a + 3, Long4(x));
			_put(a + 4, Long3(x));
			_put(a + 5, Long2(x));
			_put(a + 6, Long1(x));
			_put(a + 7, Long0(x));
		}

		internal static void PutLong(ByteBuffer bb, int bi, long x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutLongB(bb, bi, x);
			}
			else
			{
				PutLongL(bb, bi, x);
			}
		}

		internal static void PutLong(long a, long x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutLongB(a, x);
			}
			else
			{
				PutLongL(a, x);
			}
		}


		// -- get/put float --

		internal static float GetFloatL(ByteBuffer bb, int bi)
		{
			return Float.intBitsToFloat(GetIntL(bb, bi));
		}

		internal static float GetFloatL(long a)
		{
			return Float.intBitsToFloat(GetIntL(a));
		}

		internal static float GetFloatB(ByteBuffer bb, int bi)
		{
			return Float.intBitsToFloat(GetIntB(bb, bi));
		}

		internal static float GetFloatB(long a)
		{
			return Float.intBitsToFloat(GetIntB(a));
		}

		internal static float GetFloat(ByteBuffer bb, int bi, bool bigEndian)
		{
			return bigEndian ? GetFloatB(bb, bi) : GetFloatL(bb, bi);
		}

		internal static float GetFloat(long a, bool bigEndian)
		{
			return bigEndian ? GetFloatB(a) : GetFloatL(a);
		}

		internal static void PutFloatL(ByteBuffer bb, int bi, float x)
		{
			PutIntL(bb, bi, Float.floatToRawIntBits(x));
		}

		internal static void PutFloatL(long a, float x)
		{
			PutIntL(a, Float.floatToRawIntBits(x));
		}

		internal static void PutFloatB(ByteBuffer bb, int bi, float x)
		{
			PutIntB(bb, bi, Float.floatToRawIntBits(x));
		}

		internal static void PutFloatB(long a, float x)
		{
			PutIntB(a, Float.floatToRawIntBits(x));
		}

		internal static void PutFloat(ByteBuffer bb, int bi, float x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutFloatB(bb, bi, x);
			}
			else
			{
				PutFloatL(bb, bi, x);
			}
		}

		internal static void PutFloat(long a, float x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutFloatB(a, x);
			}
			else
			{
				PutFloatL(a, x);
			}
		}


		// -- get/put double --

		internal static double GetDoubleL(ByteBuffer bb, int bi)
		{
			return Double.longBitsToDouble(GetLongL(bb, bi));
		}

		internal static double GetDoubleL(long a)
		{
			return Double.longBitsToDouble(GetLongL(a));
		}

		internal static double GetDoubleB(ByteBuffer bb, int bi)
		{
			return Double.longBitsToDouble(GetLongB(bb, bi));
		}

		internal static double GetDoubleB(long a)
		{
			return Double.longBitsToDouble(GetLongB(a));
		}

		internal static double GetDouble(ByteBuffer bb, int bi, bool bigEndian)
		{
			return bigEndian ? GetDoubleB(bb, bi) : GetDoubleL(bb, bi);
		}

		internal static double GetDouble(long a, bool bigEndian)
		{
			return bigEndian ? GetDoubleB(a) : GetDoubleL(a);
		}

		internal static void PutDoubleL(ByteBuffer bb, int bi, double x)
		{
			PutLongL(bb, bi, Double.doubleToRawLongBits(x));
		}

		internal static void PutDoubleL(long a, double x)
		{
			PutLongL(a, Double.doubleToRawLongBits(x));
		}

		internal static void PutDoubleB(ByteBuffer bb, int bi, double x)
		{
			PutLongB(bb, bi, Double.doubleToRawLongBits(x));
		}

		internal static void PutDoubleB(long a, double x)
		{
			PutLongB(a, Double.doubleToRawLongBits(x));
		}

		internal static void PutDouble(ByteBuffer bb, int bi, double x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutDoubleB(bb, bi, x);
			}
			else
			{
				PutDoubleL(bb, bi, x);
			}
		}

		internal static void PutDouble(long a, double x, bool bigEndian)
		{
			if (bigEndian)
			{
				PutDoubleB(a, x);
			}
			else
			{
				PutDoubleL(a, x);
			}
		}


		// -- Unsafe access --

		private static readonly Unsafe unsafe_Renamed = Unsafe.Unsafe;

		private static sbyte _get(long a)
		{
			return unsafe_Renamed.getByte(a);
		}

		private static void _put(long a, sbyte b)
		{
			unsafe_Renamed.putByte(a, b);
		}

		internal static Unsafe @unsafe()
		{
			return unsafe_Renamed;
		}


		// -- Processor and memory-system properties --

		private static readonly ByteOrder ByteOrder_Renamed;

		internal static ByteOrder ByteOrder()
		{
			if (ByteOrder_Renamed == null)
			{
				throw new Error("Unknown byte order");
			}
			return ByteOrder_Renamed;
		}

		static Bits()
		{
			long a = unsafe_Renamed.allocateMemory(8);
			try
			{
				unsafe_Renamed.putLong(a, 0x0102030405060708L);
				sbyte b = unsafe_Renamed.getByte(a);
				switch (b)
				{
				case 0x01:
					ByteOrder_Renamed = ByteOrder.BIG_ENDIAN;
					break;
				case 0x08:
					ByteOrder_Renamed = ByteOrder.LITTLE_ENDIAN;
					break;
				default:
					Debug.Assert(false);
					ByteOrder_Renamed = null;
				break;
				}
			}
			finally
			{
				unsafe_Renamed.freeMemory(a);
			}
			// setup access to this package in SharedSecrets
			sun.misc.SharedSecrets.JavaNioAccess = new JavaNioAccessAnonymousInnerClassHelper();
		}

		private class JavaNioAccessAnonymousInnerClassHelper : sun.misc.JavaNioAccess
		{
			public JavaNioAccessAnonymousInnerClassHelper()
			{
			}

			public override sun.misc.JavaNioAccess.BufferPool DirectBufferPool
			{
				get
				{
					return new BufferPoolAnonymousInnerClassHelper(this);
				}
			}

			private class BufferPoolAnonymousInnerClassHelper : sun.misc.JavaNioAccess.BufferPool
			{
				private readonly JavaNioAccessAnonymousInnerClassHelper OuterInstance;

				public BufferPoolAnonymousInnerClassHelper(JavaNioAccessAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public override String Name
				{
					get
					{
						return "direct";
					}
				}
				public override long Count
				{
					get
					{
						return Bits.Count;
					}
				}
				public override long TotalCapacity
				{
					get
					{
						return Bits.TotalCapacity;
					}
				}
				public override long MemoryUsed
				{
					get
					{
						return Bits.ReservedMemory;
					}
				}
			}
			public override ByteBuffer NewDirectByteBuffer(long addr, int cap, Object ob)
			{
				return new DirectByteBuffer(addr, cap, ob);
			}
			public override void Truncate(Buffer buf)
			{
				buf.Truncate();
			}
		}


		private static int PageSize_Renamed = -1;

		internal static int PageSize()
		{
			if (PageSize_Renamed == -1)
			{
				PageSize_Renamed = @unsafe().pageSize();
			}
			return PageSize_Renamed;
		}

		internal static int PageCount(long size)
		{
			return (int)(size + (long)PageSize() - 1L) / PageSize();
		}

		private static bool Unaligned_Renamed;
		private static bool UnalignedKnown = false;

		internal static bool Unaligned()
		{
			if (UnalignedKnown)
			{
				return Unaligned_Renamed;
			}
			String arch = AccessController.doPrivileged(new sun.security.action.GetPropertyAction("os.arch"));
			Unaligned_Renamed = arch.Equals("i386") || arch.Equals("x86") || arch.Equals("amd64") || arch.Equals("x86_64");
			UnalignedKnown = true;
			return Unaligned_Renamed;
		}


		// -- Direct memory management --

		// A user-settable upper limit on the maximum amount of allocatable
		// direct buffer memory.  This value may be changed during VM
		// initialization if it is launched with "-XX:MaxDirectMemorySize=<size>".
		private static volatile long MaxMemory = VM.maxDirectMemory();
		private static volatile long ReservedMemory;
		private static volatile long TotalCapacity;
		private static volatile long Count;
		private static bool MemoryLimitSet = false;

		// These methods should be called whenever direct memory is allocated or
		// freed.  They allow the user to control the amount of direct memory
		// which a process may access.  All sizes are specified in bytes.
		internal static void ReserveMemory(long size, int cap)
		{
			lock (typeof(Bits))
			{
				if (!MemoryLimitSet && VM.Booted)
				{
					MaxMemory = VM.maxDirectMemory();
					MemoryLimitSet = true;
				}
				// -XX:MaxDirectMemorySize limits the total capacity rather than the
				// actual memory usage, which will differ when buffers are page
				// aligned.
				if (cap <= MaxMemory - TotalCapacity)
				{
					ReservedMemory += size;
					TotalCapacity += cap;
					Count++;
					return;
				}
			}

			System.gc();
			try
			{
				Thread.Sleep(100);
			}
			catch (InterruptedException)
			{
				// Restore interrupt status
				Thread.CurrentThread.Interrupt();
			}
			lock (typeof(Bits))
			{
				if (TotalCapacity + cap > MaxMemory)
				{
					throw new OutOfMemoryError("Direct buffer memory");
				}
				ReservedMemory += size;
				TotalCapacity += cap;
				Count++;
			}

		}

		internal static void UnreserveMemory(long size, int cap)
		{
			lock (typeof(Bits))
			{
				if (ReservedMemory > 0)
				{
					ReservedMemory -= size;
					TotalCapacity -= cap;
					Count--;
					assert(ReservedMemory > -1);
				}
			}
		}

		// -- Monitoring of direct buffer usage --


		// -- Bulk get/put acceleration --

		// These numbers represent the point at which we have empirically
		// determined that the average cost of a JNI call exceeds the expense
		// of an element by element copy.  These numbers may change over time.
		internal const int JNI_COPY_TO_ARRAY_THRESHOLD = 6;
		internal const int JNI_COPY_FROM_ARRAY_THRESHOLD = 6;

		// This number limits the number of bytes to copy per call to Unsafe's
		// copyMemory method. A limit is imposed to allow for safepoint polling
		// during a large copy
		internal static readonly long UNSAFE_COPY_THRESHOLD = 1024L * 1024L;

		// These methods do no bounds checking.  Verification that the copy will not
		// result in memory corruption should be done prior to invocation.
		// All positions and lengths are specified in bytes.

		/// <summary>
		/// Copy from given source array to destination address.
		/// </summary>
		/// <param name="src">
		///          source array </param>
		/// <param name="srcBaseOffset">
		///          offset of first element of storage in source array </param>
		/// <param name="srcPos">
		///          offset within source array of the first element to read </param>
		/// <param name="dstAddr">
		///          destination address </param>
		/// <param name="length">
		///          number of bytes to copy </param>
		internal static void CopyFromArray(Object src, long srcBaseOffset, long srcPos, long dstAddr, long length)
		{
			long offset = srcBaseOffset + srcPos;
			while (length > 0)
			{
				long size = (length > UNSAFE_COPY_THRESHOLD) ? UNSAFE_COPY_THRESHOLD : length;
				unsafe_Renamed.copyMemory(src, offset, null, dstAddr, size);
				length -= size;
				offset += size;
				dstAddr += size;
			}
		}

		/// <summary>
		/// Copy from source address into given destination array.
		/// </summary>
		/// <param name="srcAddr">
		///          source address </param>
		/// <param name="dst">
		///          destination array </param>
		/// <param name="dstBaseOffset">
		///          offset of first element of storage in destination array </param>
		/// <param name="dstPos">
		///          offset within destination array of the first element to write </param>
		/// <param name="length">
		///          number of bytes to copy </param>
		internal static void CopyToArray(long srcAddr, Object dst, long dstBaseOffset, long dstPos, long length)
		{
			long offset = dstBaseOffset + dstPos;
			while (length > 0)
			{
				long size = (length > UNSAFE_COPY_THRESHOLD) ? UNSAFE_COPY_THRESHOLD : length;
				unsafe_Renamed.copyMemory(null, srcAddr, dst, offset, size);
				length -= size;
				srcAddr += size;
				offset += size;
			}
		}

		internal static void CopyFromCharArray(Object src, long srcPos, long dstAddr, long length)
		{
			copyFromShortArray(src, srcPos, dstAddr, length);
		}

		internal static void CopyToCharArray(long srcAddr, Object dst, long dstPos, long length)
		{
			copyToShortArray(srcAddr, dst, dstPos, length);
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void copyFromShortArray(Object src, long srcPos, long dstAddr, long length);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void copyToShortArray(long srcAddr, Object dst, long dstPos, long length);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void copyFromIntArray(Object src, long srcPos, long dstAddr, long length);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void copyToIntArray(long srcAddr, Object dst, long dstPos, long length);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void copyFromLongArray(Object src, long srcPos, long dstAddr, long length);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		internal static extern void copyToLongArray(long srcAddr, Object dst, long dstPos, long length);

	}

}