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
	 * This interface defines the constants that are used by the classes
	 * which manipulate ZIP files.
	 *
	 * @author      David Connelly
	 */
	internal interface ZipConstants
	{
		/*
		 * Header signatures
		 */

		/*
		 * Header sizes in bytes (including signatures)
		 */

		/*
		 * Local file (LOC) header field offsets
		 */

		/*
		 * Extra local (EXT) header field offsets
		 */

		/*
		 * Central directory (CEN) header field offsets
		 */

		/*
		 * End of central directory (END) header field offsets
		 */
	}

	public static class ZipConstants_Fields
	{
		public const long LOCSIG = 0x04034b50L;
		public const long EXTSIG = 0x08074b50L;
		public const long CENSIG = 0x02014b50L;
		public const long ENDSIG = 0x06054b50L;
		public const int LOCHDR = 30;
		public const int EXTHDR = 16;
		public const int CENHDR = 46;
		public const int ENDHDR = 22;
		public const int LOCVER = 4;
		public const int LOCFLG = 6;
		public const int LOCHOW = 8;
		public const int LOCTIM = 10;
		public const int LOCCRC = 14;
		public const int LOCSIZ = 18;
		public const int LOCLEN = 22;
		public const int LOCNAM = 26;
		public const int LOCEXT = 28;
		public const int EXTCRC = 4;
		public const int EXTSIZ = 8;
		public const int EXTLEN = 12;
		public const int CENVEM = 4;
		public const int CENVER = 6;
		public const int CENFLG = 8;
		public const int CENHOW = 10;
		public const int CENTIM = 12;
		public const int CENCRC = 16;
		public const int CENSIZ = 20;
		public const int CENLEN = 24;
		public const int CENNAM = 28;
		public const int CENEXT = 30;
		public const int CENCOM = 32;
		public const int CENDSK = 34;
		public const int CENATT = 36;
		public const int CENATX = 38;
		public const int CENOFF = 42;
		public const int ENDSUB = 8;
		public const int ENDTOT = 10;
		public const int ENDSIZ = 12;
		public const int ENDOFF = 16;
		public const int ENDCOM = 20;
	}

}