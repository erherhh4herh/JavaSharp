using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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


	/// <summary>
	/// This class implements an output stream filter for writing files in the
	/// ZIP file format. Includes support for both compressed and uncompressed
	/// entries.
	/// 
	/// @author      David Connelly
	/// </summary>
	public class ZipOutputStream : DeflaterOutputStream, ZipConstants
	{

		/// <summary>
		/// Whether to use ZIP64 for zip files with more than 64k entries.
		/// Until ZIP64 support in zip implementations is ubiquitous, this
		/// system property allows the creation of zip files which can be
		/// read by legacy zip implementations which tolerate "incorrect"
		/// total entry count fields, such as the ones in jdk6, and even
		/// some in jdk7.
		/// </summary>
		private static readonly bool InhibitZip64 = Convert.ToBoolean(java.security.AccessController.doPrivileged(new sun.security.action.GetPropertyAction("jdk.util.zip.inhibitZip64", "false")));

		private class XEntry
		{
			internal readonly ZipEntry Entry;
			internal readonly long Offset;
			public XEntry(ZipEntry entry, long offset)
			{
				this.Entry = entry;
				this.Offset = offset;
			}
		}

		private XEntry Current;
		private List<XEntry> Xentries = new List<XEntry>();
		private HashSet<String> Names = new HashSet<String>();
		private CRC32 Crc = new CRC32();
		private long Written = 0;
		private long Locoff = 0;
		private sbyte[] Comment_Renamed;
		private int Method_Renamed = DEFLATED;
		private bool Finished;

		private bool Closed = false;

		private readonly ZipCoder Zc;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int version(ZipEntry e) throws ZipException
		private static int Version(ZipEntry e)
		{
			switch (e.Method_Renamed)
			{
			case DEFLATED:
				return 20;
			case STORED:
				return 10;
			default:
				throw new ZipException("unsupported compression method");
			}
		}

		/// <summary>
		/// Checks to make sure that this stream has not been closed.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureOpen() throws java.io.IOException
		private void EnsureOpen()
		{
			if (Closed)
			{
				throw new IOException("Stream closed");
			}
		}
		/// <summary>
		/// Compression method for uncompressed (STORED) entries.
		/// </summary>
		public const int STORED = ZipEntry.STORED;

		/// <summary>
		/// Compression method for compressed (DEFLATED) entries.
		/// </summary>
		public const int DEFLATED = ZipEntry.DEFLATED;

		/// <summary>
		/// Creates a new ZIP output stream.
		/// 
		/// <para>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used
		/// to encode the entry names and comments.
		/// 
		/// </para>
		/// </summary>
		/// <param name="out"> the actual output stream </param>
		public ZipOutputStream(OutputStream @out) : this(@out, StandardCharsets.UTF_8)
		{
		}

		/// <summary>
		/// Creates a new ZIP output stream.
		/// </summary>
		/// <param name="out"> the actual output stream
		/// </param>
		/// <param name="charset"> the <seealso cref="java.nio.charset.Charset charset"/>
		///                to be used to encode the entry names and comments
		/// 
		/// @since 1.7 </param>
		public ZipOutputStream(OutputStream @out, Charset charset) : base(@out, new Deflater(Deflater.DEFAULT_COMPRESSION, true))
		{
			if (charset == null)
			{
				throw new NullPointerException("charset is null");
			}
			this.Zc = ZipCoder.Get(charset);
			UsesDefaultDeflater = true;
		}

		/// <summary>
		/// Sets the ZIP file comment. </summary>
		/// <param name="comment"> the comment string </param>
		/// <exception cref="IllegalArgumentException"> if the length of the specified
		///            ZIP file comment is greater than 0xFFFF bytes </exception>
		public virtual String Comment
		{
			set
			{
				if (value != null)
				{
					this.Comment_Renamed = Zc.GetBytes(value);
					if (this.Comment_Renamed.Length > 0xffff)
					{
						throw new IllegalArgumentException("ZIP file comment too long.");
					}
				}
			}
		}

		/// <summary>
		/// Sets the default compression method for subsequent entries. This
		/// default will be used whenever the compression method is not specified
		/// for an individual ZIP file entry, and is initially set to DEFLATED. </summary>
		/// <param name="method"> the default compression method </param>
		/// <exception cref="IllegalArgumentException"> if the specified compression method
		///            is invalid </exception>
		public virtual int Method
		{
			set
			{
				if (value != DEFLATED && value != STORED)
				{
					throw new IllegalArgumentException("invalid compression method");
				}
				this.Method_Renamed = value;
			}
		}

		/// <summary>
		/// Sets the compression level for subsequent entries which are DEFLATED.
		/// The default setting is DEFAULT_COMPRESSION. </summary>
		/// <param name="level"> the compression level (0-9) </param>
		/// <exception cref="IllegalArgumentException"> if the compression level is invalid </exception>
		public virtual int Level
		{
			set
			{
				Def.Level = value;
			}
		}

		/// <summary>
		/// Begins writing a new ZIP file entry and positions the stream to the
		/// start of the entry data. Closes the current entry if still active.
		/// The default compression method will be used if no compression method
		/// was specified for the entry, and the current time will be used if
		/// the entry has no set modification time. </summary>
		/// <param name="e"> the ZIP entry to be written </param>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void putNextEntry(ZipEntry e) throws java.io.IOException
		public virtual void PutNextEntry(ZipEntry e)
		{
			EnsureOpen();
			if (Current != null)
			{
				CloseEntry(); // close previous entry
			}
			if (e.Xdostime == -1)
			{
				// by default, do NOT use extended timestamps in extra
				// data, for now.
				e.Time = DateTimeHelperClass.CurrentUnixTimeMillis();
			}
			if (e.Method_Renamed == -1)
			{
				e.Method_Renamed = Method_Renamed; // use default method
			}
			// store size, compressed size, and crc-32 in LOC header
			e.Flag = 0;
			switch (e.Method_Renamed)
			{
			case DEFLATED:
				// store size, compressed size, and crc-32 in data descriptor
				// immediately following the compressed entry data
				if (e.Size_Renamed == -1 || e.Csize == -1 || e.Crc_Renamed == -1)
				{
					e.Flag = 8;
				}

				break;
			case STORED:
				// compressed size, uncompressed size, and crc-32 must all be
				// set for entries using STORED compression method
				if (e.Size_Renamed == -1)
				{
					e.Size_Renamed = e.Csize;
				}
				else if (e.Csize == -1)
				{
					e.Csize = e.Size_Renamed;
				}
				else if (e.Size_Renamed != e.Csize)
				{
					throw new ZipException("STORED entry where compressed != uncompressed size");
				}
				if (e.Size_Renamed == -1 || e.Crc_Renamed == -1)
				{
					throw new ZipException("STORED entry missing size, compressed size, or crc-32");
				}
				break;
			default:
				throw new ZipException("unsupported compression method");
			}
			if (!Names.Add(e.Name_Renamed))
			{
				throw new ZipException("duplicate entry: " + e.Name_Renamed);
			}
			if (Zc.UTF8)
			{
				e.Flag |= EFS;
			}
			Current = new XEntry(e, Written);
			Xentries.Add(Current);
			WriteLOC(Current);
		}

		/// <summary>
		/// Closes the current ZIP entry and positions the stream for writing
		/// the next entry. </summary>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeEntry() throws java.io.IOException
		public virtual void CloseEntry()
		{
			EnsureOpen();
			if (Current != null)
			{
				ZipEntry e = Current.Entry;
				switch (e.Method_Renamed)
				{
				case DEFLATED:
					Def.Finish();
					while (!Def.Finished())
					{
						Deflate();
					}
					if ((e.Flag & 8) == 0)
					{
						// verify size, compressed size, and crc-32 settings
						if (e.Size_Renamed != Def.BytesRead)
						{
							throw new ZipException("invalid entry size (expected " + e.Size_Renamed + " but got " + Def.BytesRead + " bytes)");
						}
						if (e.Csize != Def.BytesWritten)
						{
							throw new ZipException("invalid entry compressed size (expected " + e.Csize + " but got " + Def.BytesWritten + " bytes)");
						}
						if (e.Crc_Renamed != Crc.Value)
						{
							throw new ZipException("invalid entry CRC-32 (expected 0x" + e.Crc_Renamed.ToString("x") + " but got 0x" + Crc.Value.ToString("x") + ")");
						}
					}
					else
					{
						e.Size_Renamed = Def.BytesRead;
						e.Csize = Def.BytesWritten;
						e.Crc_Renamed = Crc.Value;
						WriteEXT(e);
					}
					Def.Reset();
					Written += e.Csize;
					break;
				case STORED:
					// we already know that both e.size and e.csize are the same
					if (e.Size_Renamed != Written - Locoff)
					{
						throw new ZipException("invalid entry size (expected " + e.Size_Renamed + " but got " + (Written - Locoff) + " bytes)");
					}
					if (e.Crc_Renamed != Crc.Value)
					{
						throw new ZipException("invalid entry crc-32 (expected 0x" + e.Crc_Renamed.ToString("x") + " but got 0x" + Crc.Value.ToString("x") + ")");
					}
					break;
				default:
					throw new ZipException("invalid compression method");
				}
				Crc.Reset();
				Current = null;
			}
		}

		/// <summary>
		/// Writes an array of bytes to the current ZIP entry data. This method
		/// will block until all the bytes are written. </summary>
		/// <param name="b"> the data to be written </param>
		/// <param name="off"> the start offset in the data </param>
		/// <param name="len"> the number of bytes that are written </param>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void write(byte[] b, int off, int len) throws java.io.IOException
		public override void Write(sbyte[] b, int off, int len)
		{
			lock (this)
			{
				EnsureOpen();
				if (off < 0 || len < 0 || off > b.Length - len)
				{
					throw new IndexOutOfBoundsException();
				}
				else if (len == 0)
				{
					return;
				}
        
				if (Current == null)
				{
					throw new ZipException("no current ZIP entry");
				}
				ZipEntry entry = Current.Entry;
				switch (entry.Method_Renamed)
				{
				case DEFLATED:
					base.Write(b, off, len);
					break;
				case STORED:
					Written += len;
					if (Written - Locoff > entry.Size_Renamed)
					{
						throw new ZipException("attempt to write past end of STORED entry");
					}
					@out.Write(b, off, len);
					break;
				default:
					throw new ZipException("invalid compression method");
				}
				Crc.Update(b, off, len);
			}
		}

		/// <summary>
		/// Finishes writing the contents of the ZIP output stream without closing
		/// the underlying stream. Use this method when applying multiple filters
		/// in succession to the same output stream. </summary>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O exception has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finish() throws java.io.IOException
		public override void Finish()
		{
			EnsureOpen();
			if (Finished)
			{
				return;
			}
			if (Current != null)
			{
				CloseEntry();
			}
			// write central directory
			long off = Written;
			foreach (XEntry xentry in Xentries)
			{
				WriteCEN(xentry);
			}
			WriteEND(off, Written - off);
			Finished = true;
		}

		/// <summary>
		/// Closes the ZIP output stream as well as the stream being filtered. </summary>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public override void Close()
		{
			if (!Closed)
			{
				base.Close();
				Closed = true;
			}
		}

		/*
		 * Writes local file (LOC) header for specified entry.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeLOC(XEntry xentry) throws java.io.IOException
		private void WriteLOC(XEntry xentry)
		{
			ZipEntry e = xentry.Entry;
			int flag = e.Flag;
			bool hasZip64 = false;
			int elen = GetExtraLen(e.Extra_Renamed);

			WriteInt(ZipConstants_Fields.LOCSIG); // LOC header signature
			if ((flag & 8) == 8)
			{
				WriteShort(Version(e)); // version needed to extract
				WriteShort(flag); // general purpose bit flag
				WriteShort(e.Method_Renamed); // compression method
				WriteInt(e.Xdostime); // last modification time
				// store size, uncompressed size, and crc-32 in data descriptor
				// immediately following compressed entry data
				WriteInt(0);
				WriteInt(0);
				WriteInt(0);
			}
			else
			{
				if (e.Csize >= ZIP64_MAGICVAL || e.Size_Renamed >= ZIP64_MAGICVAL)
				{
					hasZip64 = true;
					WriteShort(45); // ver 4.5 for zip64
				}
				else
				{
					WriteShort(Version(e)); // version needed to extract
				}
				WriteShort(flag); // general purpose bit flag
				WriteShort(e.Method_Renamed); // compression method
				WriteInt(e.Xdostime); // last modification time
				WriteInt(e.Crc_Renamed); // crc-32
				if (hasZip64)
				{
					WriteInt(ZIP64_MAGICVAL);
					WriteInt(ZIP64_MAGICVAL);
					elen += 20; //headid(2) + size(2) + size(8) + csize(8)
				}
				else
				{
					WriteInt(e.Csize); // compressed size
					WriteInt(e.Size_Renamed); // uncompressed size
				}
			}
			sbyte[] nameBytes = Zc.GetBytes(e.Name_Renamed);
			WriteShort(nameBytes.Length);

			int elenEXTT = 0; // info-zip extended timestamp
			int flagEXTT = 0;
			if (e.Mtime != null)
			{
				elenEXTT += 4;
				flagEXTT |= EXTT_FLAG_LMT;
			}
			if (e.Atime != null)
			{
				elenEXTT += 4;
				flagEXTT |= EXTT_FLAG_LAT;
			}
			if (e.Ctime != null)
			{
				elenEXTT += 4;
				flagEXTT |= EXTT_FLAT_CT;
			}
			if (flagEXTT != 0)
			{
				elen += (elenEXTT + 5); // headid(2) + size(2) + flag(1) + data
			}
			WriteShort(elen);
			WriteBytes(nameBytes, 0, nameBytes.Length);
			if (hasZip64)
			{
				WriteShort(ZIP64_EXTID);
				WriteShort(16);
				WriteLong(e.Size_Renamed);
				WriteLong(e.Csize);
			}
			if (flagEXTT != 0)
			{
				WriteShort(EXTID_EXTT);
				WriteShort(elenEXTT + 1); // flag + data
				WriteByte(flagEXTT);
				if (e.Mtime != null)
				{
					WriteInt(fileTimeToUnixTime(e.Mtime));
				}
				if (e.Atime != null)
				{
					WriteInt(fileTimeToUnixTime(e.Atime));
				}
				if (e.Ctime != null)
				{
					WriteInt(fileTimeToUnixTime(e.Ctime));
				}
			}
			WriteExtra(e.Extra_Renamed);
			Locoff = Written;
		}

		/*
		 * Writes extra data descriptor (EXT) for specified entry.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeEXT(ZipEntry e) throws java.io.IOException
		private void WriteEXT(ZipEntry e)
		{
			WriteInt(ZipConstants_Fields.EXTSIG); // EXT header signature
			WriteInt(e.Crc_Renamed); // crc-32
			if (e.Csize >= ZIP64_MAGICVAL || e.Size_Renamed >= ZIP64_MAGICVAL)
			{
				WriteLong(e.Csize);
				WriteLong(e.Size_Renamed);
			}
			else
			{
				WriteInt(e.Csize); // compressed size
				WriteInt(e.Size_Renamed); // uncompressed size
			}
		}

		/*
		 * Write central directory (CEN) header for specified entry.
		 * REMIND: add support for file attributes
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeCEN(XEntry xentry) throws java.io.IOException
		private void WriteCEN(XEntry xentry)
		{
			ZipEntry e = xentry.Entry;
			int flag = e.Flag;
			int version = Version(e);
			long csize = e.Csize;
			long size = e.Size_Renamed;
			long offset = xentry.Offset;
			int elenZIP64 = 0;
			bool hasZip64 = false;

			if (e.Csize >= ZIP64_MAGICVAL)
			{
				csize = ZIP64_MAGICVAL;
				elenZIP64 += 8; // csize(8)
				hasZip64 = true;
			}
			if (e.Size_Renamed >= ZIP64_MAGICVAL)
			{
				size = ZIP64_MAGICVAL; // size(8)
				elenZIP64 += 8;
				hasZip64 = true;
			}
			if (xentry.Offset >= ZIP64_MAGICVAL)
			{
				offset = ZIP64_MAGICVAL;
				elenZIP64 += 8; // offset(8)
				hasZip64 = true;
			}
			WriteInt(ZipConstants_Fields.CENSIG); // CEN header signature
			if (hasZip64)
			{
				WriteShort(45); // ver 4.5 for zip64
				WriteShort(45);
			}
			else
			{
				WriteShort(version); // version made by
				WriteShort(version); // version needed to extract
			}
			WriteShort(flag); // general purpose bit flag
			WriteShort(e.Method_Renamed); // compression method
			WriteInt(e.Xdostime); // last modification time
			WriteInt(e.Crc_Renamed); // crc-32
			WriteInt(csize); // compressed size
			WriteInt(size); // uncompressed size
			sbyte[] nameBytes = Zc.GetBytes(e.Name_Renamed);
			WriteShort(nameBytes.Length);

			int elen = GetExtraLen(e.Extra_Renamed);
			if (hasZip64)
			{
				elen += (elenZIP64 + 4); // + headid(2) + datasize(2)
			}
			// cen info-zip extended timestamp only outputs mtime
			// but set the flag for a/ctime, if present in loc
			int flagEXTT = 0;
			if (e.Mtime != null)
			{
				elen += 4; // + mtime(4)
				flagEXTT |= EXTT_FLAG_LMT;
			}
			if (e.Atime != null)
			{
				flagEXTT |= EXTT_FLAG_LAT;
			}
			if (e.Ctime != null)
			{
				flagEXTT |= EXTT_FLAT_CT;
			}
			if (flagEXTT != 0)
			{
				elen += 5; // headid + sz + flag
			}
			WriteShort(elen);
			sbyte[] commentBytes;
			if (e.Comment_Renamed != null)
			{
				commentBytes = Zc.GetBytes(e.Comment_Renamed);
				WriteShort(System.Math.Min(commentBytes.Length, 0xffff));
			}
			else
			{
				commentBytes = null;
				WriteShort(0);
			}
			WriteShort(0); // starting disk number
			WriteShort(0); // internal file attributes (unused)
			WriteInt(0); // external file attributes (unused)
			WriteInt(offset); // relative offset of local header
			WriteBytes(nameBytes, 0, nameBytes.Length);

			// take care of EXTID_ZIP64 and EXTID_EXTT
			if (hasZip64)
			{
				WriteShort(ZIP64_EXTID); // Zip64 extra
				WriteShort(elenZIP64);
				if (size == ZIP64_MAGICVAL)
				{
					WriteLong(e.Size_Renamed);
				}
				if (csize == ZIP64_MAGICVAL)
				{
					WriteLong(e.Csize);
				}
				if (offset == ZIP64_MAGICVAL)
				{
					WriteLong(xentry.Offset);
				}
			}
			if (flagEXTT != 0)
			{
				WriteShort(EXTID_EXTT);
				if (e.Mtime != null)
				{
					WriteShort(5); // flag + mtime
					WriteByte(flagEXTT);
					WriteInt(fileTimeToUnixTime(e.Mtime));
				}
				else
				{
					WriteShort(1); // flag only
					WriteByte(flagEXTT);
				}
			}
			WriteExtra(e.Extra_Renamed);
			if (commentBytes != null)
			{
				WriteBytes(commentBytes, 0, System.Math.Min(commentBytes.Length, 0xffff));
			}
		}

		/*
		 * Writes end of central directory (END) header.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeEND(long off, long len) throws java.io.IOException
		private void WriteEND(long off, long len)
		{
			bool hasZip64 = false;
			long xlen = len;
			long xoff = off;
			if (xlen >= ZIP64_MAGICVAL)
			{
				xlen = ZIP64_MAGICVAL;
				hasZip64 = true;
			}
			if (xoff >= ZIP64_MAGICVAL)
			{
				xoff = ZIP64_MAGICVAL;
				hasZip64 = true;
			}
			int count = Xentries.Size();
			if (count >= ZIP64_MAGICCOUNT)
			{
				hasZip64 |= !InhibitZip64;
				if (hasZip64)
				{
					count = ZIP64_MAGICCOUNT;
				}
			}
			if (hasZip64)
			{
				long off64 = Written;
				//zip64 end of central directory record
				WriteInt(ZIP64_ENDSIG); // zip64 END record signature
				WriteLong(ZIP64_ENDHDR - 12); // size of zip64 end
				WriteShort(45); // version made by
				WriteShort(45); // version needed to extract
				WriteInt(0); // number of this disk
				WriteInt(0); // central directory start disk
				WriteLong(Xentries.Size()); // number of directory entires on disk
				WriteLong(Xentries.Size()); // number of directory entires
				WriteLong(len); // length of central directory
				WriteLong(off); // offset of central directory

				//zip64 end of central directory locator
				WriteInt(ZIP64_LOCSIG); // zip64 END locator signature
				WriteInt(0); // zip64 END start disk
				WriteLong(off64); // offset of zip64 END
				WriteInt(1); // total number of disks (?)
			}
			WriteInt(ZipConstants_Fields.ENDSIG); // END record signature
			WriteShort(0); // number of this disk
			WriteShort(0); // central directory start disk
			WriteShort(count); // number of directory entries on disk
			WriteShort(count); // total number of directory entries
			WriteInt(xlen); // length of central directory
			WriteInt(xoff); // offset of central directory
			if (Comment_Renamed != null) // zip file comment
			{
				WriteShort(Comment_Renamed.Length);
				WriteBytes(Comment_Renamed, 0, Comment_Renamed.Length);
			}
			else
			{
				WriteShort(0);
			}
		}

		/*
		 * Returns the length of extra data without EXTT and ZIP64.
		 */
		private int GetExtraLen(sbyte[] extra)
		{
			if (extra == null)
			{
				return 0;
			}
			int skipped = 0;
			int len = extra.Length;
			int off = 0;
			while (off + 4 <= len)
			{
				int tag = get16(extra, off);
				int sz = get16(extra, off + 2);
				if (sz < 0 || (off + 4 + sz) > len)
				{
					break;
				}
				if (tag == EXTID_EXTT || tag == EXTID_ZIP64)
				{
					skipped += (sz + 4);
				}
				off += (sz + 4);
			}
			return len - skipped;
		}

		/*
		 * Writes extra data without EXTT and ZIP64.
		 *
		 * Extra timestamp and ZIP64 data is handled/output separately
		 * in writeLOC and writeCEN.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeExtra(byte[] extra) throws java.io.IOException
		private void WriteExtra(sbyte[] extra)
		{
			if (extra != null)
			{
				int len = extra.Length;
				int off = 0;
				while (off + 4 <= len)
				{
					int tag = get16(extra, off);
					int sz = get16(extra, off + 2);
					if (sz < 0 || (off + 4 + sz) > len)
					{
						WriteBytes(extra, off, len - off);
						return;
					}
					if (tag != EXTID_EXTT && tag != EXTID_ZIP64)
					{
						WriteBytes(extra, off, sz + 4);
					}
					off += (sz + 4);
				}
				if (off < len)
				{
					WriteBytes(extra, off, len - off);
				}
			}
		}

		/*
		 * Writes a 8-bit byte to the output stream.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeByte(int v) throws java.io.IOException
		private void WriteByte(int v)
		{
			OutputStream @out = this.@out;
			@out.Write(v & 0xff);
			Written += 1;
		}

		/*
		 * Writes a 16-bit short to the output stream in little-endian byte order.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeShort(int v) throws java.io.IOException
		private void WriteShort(int v)
		{
			OutputStream @out = this.@out;
			@out.Write(((int)((uint)v >> 0)) & 0xff);
			@out.Write(((int)((uint)v >> 8)) & 0xff);
			Written += 2;
		}

		/*
		 * Writes a 32-bit int to the output stream in little-endian byte order.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeInt(long v) throws java.io.IOException
		private void WriteInt(long v)
		{
			OutputStream @out = this.@out;
			@out.Write((int)(((long)((ulong)v >> 0)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 8)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 16)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 24)) & 0xff));
			Written += 4;
		}

		/*
		 * Writes a 64-bit int to the output stream in little-endian byte order.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeLong(long v) throws java.io.IOException
		private void WriteLong(long v)
		{
			OutputStream @out = this.@out;
			@out.Write((int)(((long)((ulong)v >> 0)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 8)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 16)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 24)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 32)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 40)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 48)) & 0xff));
			@out.Write((int)(((long)((ulong)v >> 56)) & 0xff));
			Written += 8;
		}

		/*
		 * Writes an array of bytes to the output stream.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeBytes(byte[] b, int off, int len) throws java.io.IOException
		private void WriteBytes(sbyte[] b, int off, int len)
		{
			base.@out.Write(b, off, len);
			Written += len;
		}
	}

}