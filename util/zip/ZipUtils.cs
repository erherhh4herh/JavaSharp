using System;

/*
 * Copyright (c) 2013, 2015, Oracle and/or its affiliates. All rights reserved.
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


	internal class ZipUtils
	{

		// used to adjust values between Windows and java epoch
		private const long WINDOWS_EPOCH_IN_MICROSECONDS = -11644473600000000L;

		/// <summary>
		/// Converts Windows time (in microseconds, UTC/GMT) time to FileTime.
		/// </summary>
		public static FileTime WinTimeToFileTime(long wtime)
		{
			return FileTime.From(wtime / 10 + WINDOWS_EPOCH_IN_MICROSECONDS, TimeUnit.MICROSECONDS);
		}

		/// <summary>
		/// Converts FileTime to Windows time.
		/// </summary>
		public static long FileTimeToWinTime(FileTime ftime)
		{
			return (ftime.To(TimeUnit.MICROSECONDS) - WINDOWS_EPOCH_IN_MICROSECONDS) * 10;
		}

		/// <summary>
		/// Converts "standard Unix time"(in seconds, UTC/GMT) to FileTime
		/// </summary>
		public static FileTime UnixTimeToFileTime(long utime)
		{
			return FileTime.From(utime, TimeUnit.SECONDS);
		}

		/// <summary>
		/// Converts FileTime to "standard Unix time".
		/// </summary>
		public static long FileTimeToUnixTime(FileTime ftime)
		{
			return ftime.To(TimeUnit.SECONDS);
		}

		/// <summary>
		/// Converts DOS time to Java time (number of milliseconds since epoch).
		/// </summary>
		private static long DosToJavaTime(long dtime)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") java.util.Date d = new java.util.Date((int)(((dtime >> 25) & 0x7f) + 80), (int)(((dtime >> 21) & 0x0f) - 1), (int)((dtime >> 16) & 0x1f), (int)((dtime >> 11) & 0x1f), (int)((dtime >> 5) & 0x3f), (int)((dtime << 1) & 0x3e));
			DateTime d = new DateTime((int)(((dtime >> 25) & 0x7f) + 80), (int)(((dtime >> 21) & 0x0f) - 1), (int)((dtime >> 16) & 0x1f), (int)((dtime >> 11) & 0x1f), (int)((dtime >> 5) & 0x3f), (int)((dtime << 1) & 0x3e)); // Use of date constructor.
			return d.Ticks;
		}

		/// <summary>
		/// Converts extended DOS time to Java time, where up to 1999 milliseconds
		/// might be encoded into the upper half of the returned long.
		/// </summary>
		/// <param name="xdostime"> the extended DOS time value </param>
		/// <returns> milliseconds since epoch </returns>
		public static long ExtendedDosToJavaTime(long xdostime)
		{
			long time = DosToJavaTime(xdostime);
			return time + (xdostime >> 32);
		}

		/// <summary>
		/// Converts Java time to DOS time.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") private static long javaToDosTime(long time)
		private static long JavaToDosTime(long time) // Use of date methods
		{
			DateTime d = new DateTime(time);
			int year = d.Year + 1900;
			if (year < 1980)
			{
				return ZipEntry.DOSTIME_BEFORE_1980;
			}
			return (year - 1980) << 25 | (d.Month + 1) << 21 | d.Date << 16 | d.Hour << 11 | d.Minute << 5 | d.Second >> 1;
		}

		/// <summary>
		/// Converts Java time to DOS time, encoding any milliseconds lost
		/// in the conversion into the upper half of the returned long.
		/// </summary>
		/// <param name="time"> milliseconds since epoch </param>
		/// <returns> DOS time with 2s remainder encoded into upper half </returns>
		public static long JavaToExtendedDosTime(long time)
		{
			if (time < 0)
			{
				return ZipEntry.DOSTIME_BEFORE_1980;
			}
			long dostime = JavaToDosTime(time);
			return (dostime != ZipEntry.DOSTIME_BEFORE_1980) ? dostime + ((time % 2000) << 32) : ZipEntry.DOSTIME_BEFORE_1980;
		}

		/// <summary>
		/// Fetches unsigned 16-bit value from byte array at specified offset.
		/// The bytes are assumed to be in Intel (little-endian) byte order.
		/// </summary>
		public static int Get16(sbyte[] b, int off)
		{
			return Byte.ToUnsignedInt(b[off]) | (Byte.ToUnsignedInt(b[off + 1]) << 8);
		}

		/// <summary>
		/// Fetches unsigned 32-bit value from byte array at specified offset.
		/// The bytes are assumed to be in Intel (little-endian) byte order.
		/// </summary>
		public static long Get32(sbyte[] b, int off)
		{
			return (Get16(b, off) | ((long)Get16(b, off + 2) << 16)) & 0xffffffffL;
		}

		/// <summary>
		/// Fetches signed 64-bit value from byte array at specified offset.
		/// The bytes are assumed to be in Intel (little-endian) byte order.
		/// </summary>
		public static long Get64(sbyte[] b, int off)
		{
			return Get32(b, off) | (Get32(b, off + 4) << 32);
		}
	}

}