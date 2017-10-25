/*
 * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt.font
{

	/// <summary>
	/// The <code>OpenType</code> interface represents OpenType and
	/// TrueType fonts.  This interface makes it possible to obtain
	/// <i>sfnt</i> tables from the font.  A particular
	/// <code>Font</code> object can implement this interface.
	/// <para>
	/// For more information on TrueType and OpenType fonts, see the
	/// OpenType specification.
	/// ( <a href="http://www.microsoft.com/typography/otspec/">http://www.microsoft.com/typography/otspec/</a> ).
	/// </para>
	/// </summary>
	public interface OpenType
	{

	  /* 51 tag types so far */

	  /// <summary>
	  /// Character to glyph mapping.  Table tag "cmap" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Font header.  Table tag "head" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Naming table.  Table tag "name" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Glyph data.  Table tag "glyf" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Maximum profile.  Table tag "maxp" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// CVT preprogram.  Table tag "prep" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Horizontal metrics.  Table tag "hmtx" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Kerning.  Table tag "kern" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Horizontal device metrics.  Table tag "hdmx" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Index to location.  Table tag "loca" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// PostScript Information.  Table tag "post" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// OS/2 and Windows specific metrics.  Table tag "OS/2"
	  /// in the Open Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Control value table.  Table tag "cvt "
	  /// in the Open Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Grid-fitting and scan conversion procedure.  Table tag
	  /// "gasp" in the Open Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Vertical device metrics.  Table tag "VDMX" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Vertical metrics.  Table tag "vmtx" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Vertical metrics header.  Table tag "vhea" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Horizontal metrics header.  Table tag "hhea" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Adobe Type 1 font data.  Table tag "typ1" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Baseline table.  Table tag "bsln" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Glyph substitution.  Table tag "GSUB" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Digital signature.  Table tag "DSIG" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Font program.   Table tag "fpgm" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Font variation.   Table tag "fvar" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Glyph variation.  Table tag "gvar" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Compact font format (Type1 font).  Table tag
	  /// "CFF " in the Open Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Multiple master supplementary data.  Table tag
	  /// "MMSD" in the Open Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Multiple master font metrics.  Table tag
	  /// "MMFX" in the Open Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Baseline data.  Table tag "BASE" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Glyph definition.  Table tag "GDEF" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Glyph positioning.  Table tag "GPOS" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Justification.  Table tag "JSTF" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Embedded bitmap data.  Table tag "EBDT" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Embedded bitmap location.  Table tag "EBLC" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Embedded bitmap scaling.  Table tag "EBSC" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Linear threshold.  Table tag "LTSH" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// PCL 5 data.  Table tag "PCLT" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Accent attachment.  Table tag "acnt" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Axis variation.  Table tag "avar" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Bitmap data.  Table tag "bdat" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Bitmap location.  Table tag "bloc" in the Open
	  /// Type Specification.
	  /// </summary>

	   /// <summary>
	   /// CVT variation.  Table tag "cvar" in the Open
	   /// Type Specification.
	   /// </summary>

	  /// <summary>
	  /// Feature name.  Table tag "feat" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Font descriptors.  Table tag "fdsc" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Font metrics.  Table tag "fmtx" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Justification.  Table tag "just" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Ligature caret.   Table tag "lcar" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Glyph metamorphosis.  Table tag "mort" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Optical bounds.  Table tag "opbd" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Glyph properties.  Table tag "prop" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Tracking.  Table tag "trak" in the Open
	  /// Type Specification.
	  /// </summary>

	  /// <summary>
	  /// Returns the version of the <code>OpenType</code> font.
	  /// 1.0 is represented as 0x00010000. </summary>
	  /// <returns> the version of the <code>OpenType</code> font. </returns>
	  int Version {get;}

	  /// <summary>
	  /// Returns the table as an array of bytes for a specified tag.
	  /// Tags for sfnt tables include items like <i>cmap</i>,
	  /// <i>name</i> and <i>head</i>.  The <code>byte</code> array
	  /// returned is a copy of the font data in memory. </summary>
	  /// <param name="sfntTag"> a four-character code as a 32-bit integer </param>
	  /// <returns> a <code>byte</code> array that is the table that
	  /// contains the font data corresponding to the specified
	  /// tag. </returns>
	  sbyte[] GetFontTable(int sfntTag);

	  /// <summary>
	  /// Returns the table as an array of bytes for a specified tag.
	  /// Tags for sfnt tables include items like <i>cmap</i>,
	  /// <i>name</i> and <i>head</i>.  The byte array returned is a
	  /// copy of the font data in memory. </summary>
	  /// <param name="strSfntTag"> a four-character code as a
	  ///            <code>String</code> </param>
	  /// <returns> a <code>byte</code> array that is the table that
	  /// contains the font data corresponding to the specified
	  /// tag. </returns>
	  sbyte[] GetFontTable(String strSfntTag);

	  /// <summary>
	  /// Returns a subset of the table as an array of bytes
	  /// for a specified tag.  Tags for sfnt tables include
	  /// items like <i>cmap</i>, <i>name</i> and <i>head</i>.
	  /// The byte array returned is a copy of the font data in
	  /// memory. </summary>
	  /// <param name="sfntTag"> a four-character code as a 32-bit integer </param>
	  /// <param name="offset"> index of first byte to return from table </param>
	  /// <param name="count"> number of bytes to return from table </param>
	  /// <returns> a subset of the table corresponding to
	  ///            <code>sfntTag</code> and containing the bytes
	  ///            starting at <code>offset</code> byte and including
	  ///            <code>count</code> bytes. </returns>
	  sbyte[] GetFontTable(int sfntTag, int offset, int count);

	  /// <summary>
	  /// Returns a subset of the table as an array of bytes
	  /// for a specified tag.  Tags for sfnt tables include items
	  /// like <i>cmap</i>, <i>name</i> and <i>head</i>. The
	  /// <code>byte</code> array returned is a copy of the font
	  /// data in memory. </summary>
	  /// <param name="strSfntTag"> a four-character code as a
	  /// <code>String</code> </param>
	  /// <param name="offset"> index of first byte to return from table </param>
	  /// <param name="count">  number of bytes to return from table </param>
	  /// <returns> a subset of the table corresponding to
	  ///            <code>strSfntTag</code> and containing the bytes
	  ///            starting at <code>offset</code> byte and including
	  ///            <code>count</code> bytes. </returns>
	  sbyte[] GetFontTable(String strSfntTag, int offset, int count);

	  /// <summary>
	  /// Returns the size of the table for a specified tag. Tags for sfnt
	  /// tables include items like <i>cmap</i>, <i>name</i> and <i>head</i>. </summary>
	  /// <param name="sfntTag"> a four-character code as a 32-bit integer </param>
	  /// <returns> the size of the table corresponding to the specified
	  /// tag. </returns>
	  int GetFontTableSize(int sfntTag);

	  /// <summary>
	  /// Returns the size of the table for a specified tag. Tags for sfnt
	  /// tables include items like <i>cmap</i>, <i>name</i> and <i>head</i>. </summary>
	  /// <param name="strSfntTag"> a four-character code as a
	  /// <code>String</code> </param>
	  /// <returns> the size of the table corresponding to the specified tag. </returns>
	  int GetFontTableSize(String strSfntTag);


	}

	public static class OpenType_Fields
	{
	  public const int TAG_CMAP = 0x636d6170;
	  public const int TAG_HEAD = 0x68656164;
	  public const int TAG_NAME = 0x6e616d65;
	  public const int TAG_GLYF = 0x676c7966;
	  public const int TAG_MAXP = 0x6d617870;
	  public const int TAG_PREP = 0x70726570;
	  public const int TAG_HMTX = 0x686d7478;
	  public const int TAG_KERN = 0x6b65726e;
	  public const int TAG_HDMX = 0x68646d78;
	  public const int TAG_LOCA = 0x6c6f6361;
	  public const int TAG_POST = 0x706f7374;
	  public const int TAG_OS2 = 0x4f532f32;
	  public const int TAG_CVT = 0x63767420;
	  public const int TAG_GASP = 0x67617370;
	  public const int TAG_VDMX = 0x56444d58;
	  public const int TAG_VMTX = 0x766d7478;
	  public const int TAG_VHEA = 0x76686561;
	  public const int TAG_HHEA = 0x68686561;
	  public const int TAG_TYP1 = 0x74797031;
	  public const int TAG_BSLN = 0x62736c6e;
	  public const int TAG_GSUB = 0x47535542;
	  public const int TAG_DSIG = 0x44534947;
	  public const int TAG_FPGM = 0x6670676d;
	  public const int TAG_FVAR = 0x66766172;
	  public const int TAG_GVAR = 0x67766172;
	  public const int TAG_CFF = 0x43464620;
	  public const int TAG_MMSD = 0x4d4d5344;
	  public const int TAG_MMFX = 0x4d4d4658;
	  public const int TAG_BASE = 0x42415345;
	  public const int TAG_GDEF = 0x47444546;
	  public const int TAG_GPOS = 0x47504f53;
	  public const int TAG_JSTF = 0x4a535446;
	  public const int TAG_EBDT = 0x45424454;
	  public const int TAG_EBLC = 0x45424c43;
	  public const int TAG_EBSC = 0x45425343;
	  public const int TAG_LTSH = 0x4c545348;
	  public const int TAG_PCLT = 0x50434c54;
	  public const int TAG_ACNT = 0x61636e74;
	  public const int TAG_AVAR = 0x61766172;
	  public const int TAG_BDAT = 0x62646174;
	  public const int TAG_BLOC = 0x626c6f63;
	  public const int TAG_CVAR = 0x63766172;
	  public const int TAG_FEAT = 0x66656174;
	  public const int TAG_FDSC = 0x66647363;
	  public const int TAG_FMTX = 0x666d7478;
	  public const int TAG_JUST = 0x6a757374;
	  public const int TAG_LCAR = 0x6c636172;
	  public const int TAG_MORT = 0x6d6f7274;
	  public const int TAG_OPBD = 0x6d6f7274;
	  public const int TAG_PROP = 0x70726f70;
	  public const int TAG_TRAK = 0x7472616b;
	}

}