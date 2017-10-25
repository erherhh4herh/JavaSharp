/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

	/*
	 * This class defines the constants that are used by the classes
	 * which manipulate Zip64 files.
	 */

	internal class ZipConstants64
	{

		/*
		 * ZIP64 constants
		 */
		internal const long ZIP64_ENDSIG = 0x06064b50L; // "PK\006\006"
		internal const long ZIP64_LOCSIG = 0x07064b50L; // "PK\006\007"
		internal const int ZIP64_ENDHDR = 56; // ZIP64 end header size
		internal const int ZIP64_LOCHDR = 20; // ZIP64 end loc header size
		internal const int ZIP64_EXTHDR = 24; // EXT header size
		internal const int ZIP64_EXTID = 0x0001; // Extra field Zip64 header ID

		internal const int ZIP64_MAGICCOUNT = 0xFFFF;
		internal const long ZIP64_MAGICVAL = 0xFFFFFFFFL;

		/*
		 * Zip64 End of central directory (END) header field offsets
		 */
		internal const int ZIP64_ENDLEN = 4; // size of zip64 end of central dir
		internal const int ZIP64_ENDVEM = 12; // version made by
		internal const int ZIP64_ENDVER = 14; // version needed to extract
		internal const int ZIP64_ENDNMD = 16; // number of this disk
		internal const int ZIP64_ENDDSK = 20; // disk number of start
		internal const int ZIP64_ENDTOD = 24; // total number of entries on this disk
		internal const int ZIP64_ENDTOT = 32; // total number of entries
		internal const int ZIP64_ENDSIZ = 40; // central directory size in bytes
		internal const int ZIP64_ENDOFF = 48; // offset of first CEN header
		internal const int ZIP64_ENDEXT = 56; // zip64 extensible data sector

		/*
		 * Zip64 End of central directory locator field offsets
		 */
		internal const int ZIP64_LOCDSK = 4; // disk number start
		internal const int ZIP64_LOCOFF = 8; // offset of zip64 end
		internal const int ZIP64_LOCTOT = 16; // total number of disks

		/*
		 * Zip64 Extra local (EXT) header field offsets
		 */
		internal const int ZIP64_EXTCRC = 4; // uncompressed file crc-32 value
		internal const int ZIP64_EXTSIZ = 8; // compressed size, 8-byte
		internal const int ZIP64_EXTLEN = 16; // uncompressed size, 8-byte

		/*
		 * Language encoding flag EFS
		 */
		internal const int EFS = 0x800; // If this bit is set the filename and
											// comment fields for this file must be
											// encoded using UTF-8.

		/*
		 * Constants below are defined here (instead of in ZipConstants)
		 * to avoid being exposed as public fields of ZipFile, ZipEntry,
		 * ZipInputStream and ZipOutputstream.
		 */

		/*
		 * Extra field header ID
		 */
		internal const int EXTID_ZIP64 = 0x0001; // Zip64
		internal const int EXTID_NTFS = 0x000a; // NTFS
		internal const int EXTID_UNIX = 0x000d; // UNIX
		internal const int EXTID_EXTT = 0x5455; // Info-ZIP Extended Timestamp

		/*
		 * EXTT timestamp flags
		 */
		internal const int EXTT_FLAG_LMT = 0x1; // LastModifiedTime
		internal const int EXTT_FLAG_LAT = 0x2; // LastAccessTime
		internal const int EXTT_FLAT_CT = 0x4; // CreationTime

		private ZipConstants64()
		{
		}
	}

}