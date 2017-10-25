/*
 * Copyright (c) 1995, 2015, Oracle and/or its affiliates. All rights reserved.
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
	/// This class is used to represent a ZIP file entry.
	/// 
	/// @author      David Connelly
	/// </summary>
	public class ZipEntry : ZipConstants, Cloneable
	{

		internal String Name_Renamed; // entry name
		internal long Xdostime = -1; // last modification time (in extended DOS time,
							// where milliseconds lost in conversion might
							// be encoded into the upper half)
		internal FileTime Mtime; // last modification time, from extra field data
		internal FileTime Atime; // last access time, from extra field data
		internal FileTime Ctime; // creation time, from extra field data
		internal long Crc_Renamed = -1; // crc-32 of entry data
		internal long Size_Renamed = -1; // uncompressed size of entry data
		internal long Csize = -1; // compressed size of entry data
		internal int Method_Renamed = -1; // compression method
		internal int Flag = 0; // general purpose flag
		internal sbyte[] Extra_Renamed; // optional extra field data for entry
		internal String Comment_Renamed; // optional comment string for entry

		/// <summary>
		/// Compression method for uncompressed entries.
		/// </summary>
		public const int STORED = 0;

		/// <summary>
		/// Compression method for compressed (deflated) entries.
		/// </summary>
		public const int DEFLATED = 8;

		/// <summary>
		/// DOS time constant for representing timestamps before 1980.
		/// </summary>
		internal static readonly long DOSTIME_BEFORE_1980 = (1 << 21) | (1 << 16);

		/// <summary>
		/// Approximately 128 years, in milliseconds (ignoring leap years etc).
		/// 
		/// This establish an approximate high-bound value for DOS times in
		/// milliseconds since epoch, used to enable an efficient but
		/// sufficient bounds check to avoid generating extended last modified
		/// time entries.
		/// 
		/// Calculating the exact number is locale dependent, would require loading
		/// TimeZone data eagerly, and would make little practical sense. Since DOS
		/// times theoretically go to 2107 - with compatibility not guaranteed
		/// after 2099 - setting this to a time that is before but near 2099
		/// should be sufficient.
		/// </summary>
		private static readonly long UPPER_DOSTIME_BOUND = 128L * 365 * 24 * 60 * 60 * 1000;

		/// <summary>
		/// Creates a new zip entry with the specified name.
		/// </summary>
		/// <param name="name">
		///         The entry name
		/// </param>
		/// <exception cref="NullPointerException"> if the entry name is null </exception>
		/// <exception cref="IllegalArgumentException"> if the entry name is longer than
		///         0xFFFF bytes </exception>
		public ZipEntry(String name)
		{
			Objects.RequireNonNull(name, "name");
			if (name.Length() > 0xFFFF)
			{
				throw new IllegalArgumentException("entry name too long");
			}
			this.Name_Renamed = name;
		}

		/// <summary>
		/// Creates a new zip entry with fields taken from the specified
		/// zip entry.
		/// </summary>
		/// <param name="e">
		///         A zip Entry object
		/// </param>
		/// <exception cref="NullPointerException"> if the entry object is null </exception>
		public ZipEntry(ZipEntry e)
		{
			Objects.RequireNonNull(e, "entry");
			Name_Renamed = e.Name_Renamed;
			Xdostime = e.Xdostime;
			Mtime = e.Mtime;
			Atime = e.Atime;
			Ctime = e.Ctime;
			Crc_Renamed = e.Crc_Renamed;
			Size_Renamed = e.Size_Renamed;
			Csize = e.Csize;
			Method_Renamed = e.Method_Renamed;
			Flag = e.Flag;
			Extra_Renamed = e.Extra_Renamed;
			Comment_Renamed = e.Comment_Renamed;
		}

		/// <summary>
		/// Creates a new un-initialized zip entry
		/// </summary>
		internal ZipEntry()
		{
		}

		/// <summary>
		/// Returns the name of the entry. </summary>
		/// <returns> the name of the entry </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Sets the last modification time of the entry.
		/// 
		/// <para> If the entry is output to a ZIP file or ZIP file formatted
		/// output stream the last modification time set by this method will
		/// be stored into the {@code date and time fields} of the zip file
		/// entry and encoded in standard {@code MS-DOS date and time format}.
		/// The <seealso cref="java.util.TimeZone#getDefault() default TimeZone"/> is
		/// used to convert the epoch time to the MS-DOS data and time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="time">
		///         The last modification time of the entry in milliseconds
		///         since the epoch
		/// </param>
		/// <seealso cref= #getTime() </seealso>
		/// <seealso cref= #getLastModifiedTime() </seealso>
		public virtual long Time
		{
			set
			{
				this.Xdostime = javaToExtendedDosTime(value);
				// Avoid setting the mtime field if value is in the valid
				// range for a DOS value
				if (Xdostime != DOSTIME_BEFORE_1980 && value <= UPPER_DOSTIME_BOUND)
				{
					this.Mtime = null;
				}
				else
				{
					this.Mtime = FileTime.From(value, TimeUnit.MILLISECONDS);
				}
			}
			get
			{
				if (Mtime != null)
				{
					return Mtime.ToMillis();
				}
				return (Xdostime != -1) ? extendedDosToJavaTime(Xdostime) : -1;
			}
		}


		/// <summary>
		/// Sets the last modification time of the entry.
		/// 
		/// <para> When output to a ZIP file or ZIP file formatted output stream
		/// the last modification time set by this method will be stored into
		/// zip file entry's {@code date and time fields} in {@code standard
		/// MS-DOS date and time format}), and the extended timestamp fields
		/// in {@code optional extra data} in UTC time.
		/// 
		/// </para>
		/// </summary>
		/// <param name="time">
		///         The last modification time of the entry </param>
		/// <returns> This zip entry
		/// </returns>
		/// <exception cref="NullPointerException"> if the {@code time} is null
		/// </exception>
		/// <seealso cref= #getLastModifiedTime()
		/// @since 1.8 </seealso>
		public virtual ZipEntry SetLastModifiedTime(FileTime time)
		{
			this.Mtime = Objects.RequireNonNull(time, "lastModifiedTime");
			this.Xdostime = javaToExtendedDosTime(time.To(TimeUnit.MILLISECONDS));
			return this;
		}

		/// <summary>
		/// Returns the last modification time of the entry.
		/// 
		/// <para> If the entry is read from a ZIP file or ZIP file formatted
		/// input stream, this is the last modification time from the zip
		/// file entry's {@code optional extra data} if the extended timestamp
		/// fields are present. Otherwise the last modification time is read
		/// from the entry's {@code date and time fields}, the {@link
		/// java.util.TimeZone#getDefault() default TimeZone} is used to convert
		/// the standard MS-DOS formatted date and time to the epoch time.
		/// 
		/// </para>
		/// </summary>
		/// <returns> The last modification time of the entry, null if not specified
		/// </returns>
		/// <seealso cref= #setLastModifiedTime(FileTime)
		/// @since 1.8 </seealso>
		public virtual FileTime LastModifiedTime
		{
			get
			{
				if (Mtime != null)
				{
					return Mtime;
				}
				if (Xdostime == -1)
				{
					return null;
				}
				return FileTime.From(Time, TimeUnit.MILLISECONDS);
			}
		}

		/// <summary>
		/// Sets the last access time of the entry.
		/// 
		/// <para> If set, the last access time will be stored into the extended
		/// timestamp fields of entry's {@code optional extra data}, when output
		/// to a ZIP file or ZIP file formatted stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="time">
		///         The last access time of the entry </param>
		/// <returns> This zip entry
		/// </returns>
		/// <exception cref="NullPointerException"> if the {@code time} is null
		/// </exception>
		/// <seealso cref= #getLastAccessTime()
		/// @since 1.8 </seealso>
		public virtual ZipEntry SetLastAccessTime(FileTime time)
		{
			this.Atime = Objects.RequireNonNull(time, "lastAccessTime");
			return this;
		}

		/// <summary>
		/// Returns the last access time of the entry.
		/// 
		/// <para> The last access time is from the extended timestamp fields
		/// of entry's {@code optional extra data} when read from a ZIP file
		/// or ZIP file formatted stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns> The last access time of the entry, null if not specified
		/// </returns>
		/// <seealso cref= #setLastAccessTime(FileTime)
		/// @since 1.8 </seealso>
		public virtual FileTime LastAccessTime
		{
			get
			{
				return Atime;
			}
		}

		/// <summary>
		/// Sets the creation time of the entry.
		/// 
		/// <para> If set, the creation time will be stored into the extended
		/// timestamp fields of entry's {@code optional extra data}, when
		/// output to a ZIP file or ZIP file formatted stream.
		/// 
		/// </para>
		/// </summary>
		/// <param name="time">
		///         The creation time of the entry </param>
		/// <returns> This zip entry
		/// </returns>
		/// <exception cref="NullPointerException"> if the {@code time} is null
		/// </exception>
		/// <seealso cref= #getCreationTime()
		/// @since 1.8 </seealso>
		public virtual ZipEntry SetCreationTime(FileTime time)
		{
			this.Ctime = Objects.RequireNonNull(time, "creationTime");
			return this;
		}

		/// <summary>
		/// Returns the creation time of the entry.
		/// 
		/// <para> The creation time is from the extended timestamp fields of
		/// entry's {@code optional extra data} when read from a ZIP file
		/// or ZIP file formatted stream.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the creation time of the entry, null if not specified </returns>
		/// <seealso cref= #setCreationTime(FileTime)
		/// @since 1.8 </seealso>
		public virtual FileTime CreationTime
		{
			get
			{
				return Ctime;
			}
		}

		/// <summary>
		/// Sets the uncompressed size of the entry data.
		/// </summary>
		/// <param name="size"> the uncompressed size in bytes
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the specified size is less
		///         than 0, is greater than 0xFFFFFFFF when
		///         <a href="package-summary.html#zip64">ZIP64 format</a> is not supported,
		///         or is less than 0 when ZIP64 is supported </exception>
		/// <seealso cref= #getSize() </seealso>
		public virtual long Size
		{
			set
			{
				if (value < 0)
				{
					throw new IllegalArgumentException("invalid entry size");
				}
				this.Size_Renamed = value;
			}
			get
			{
				return Size_Renamed;
			}
		}


		/// <summary>
		/// Returns the size of the compressed entry data.
		/// 
		/// <para> In the case of a stored entry, the compressed size will be the same
		/// as the uncompressed size of the entry.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the size of the compressed entry data, or -1 if not known </returns>
		/// <seealso cref= #setCompressedSize(long) </seealso>
		public virtual long CompressedSize
		{
			get
			{
				return Csize;
			}
			set
			{
				this.Csize = value;
			}
		}


		/// <summary>
		/// Sets the CRC-32 checksum of the uncompressed entry data.
		/// </summary>
		/// <param name="crc"> the CRC-32 value
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the specified CRC-32 value is
		///         less than 0 or greater than 0xFFFFFFFF </exception>
		/// <seealso cref= #getCrc() </seealso>
		public virtual long Crc
		{
			set
			{
				if (value < 0 || value > 0xFFFFFFFFL)
				{
					throw new IllegalArgumentException("invalid entry crc-32");
				}
				this.Crc_Renamed = value;
			}
			get
			{
				return Crc_Renamed;
			}
		}


		/// <summary>
		/// Sets the compression method for the entry.
		/// </summary>
		/// <param name="method"> the compression method, either STORED or DEFLATED
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the specified compression
		///          method is invalid </exception>
		/// <seealso cref= #getMethod() </seealso>
		public virtual int Method
		{
			set
			{
				if (value != STORED && value != DEFLATED)
				{
					throw new IllegalArgumentException("invalid compression method");
				}
				this.Method_Renamed = value;
			}
			get
			{
				return Method_Renamed;
			}
		}


		/// <summary>
		/// Sets the optional extra field data for the entry.
		/// 
		/// <para> Invoking this method may change this entry's last modification
		/// time, last access time and creation time, if the {@code extra} field
		/// data includes the extensible timestamp fields, such as {@code NTFS tag
		/// 0x0001} or {@code Info-ZIP Extended Timestamp}, as specified in
		/// <a href="http://www.info-zip.org/doc/appnote-19970311-iz.zip">Info-ZIP
		/// Application Note 970311</a>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="extra">
		///         The extra field data bytes
		/// </param>
		/// <exception cref="IllegalArgumentException"> if the length of the specified
		///         extra field data is greater than 0xFFFF bytes
		/// </exception>
		/// <seealso cref= #getExtra() </seealso>
		public virtual sbyte[] Extra
		{
			set
			{
				SetExtra0(value, false);
			}
			get
			{
				return Extra_Renamed;
			}
		}

		/// <summary>
		/// Sets the optional extra field data for the entry.
		/// </summary>
		/// <param name="extra">
		///        the extra field data bytes </param>
		/// <param name="doZIP64">
		///        if true, set size and csize from ZIP64 fields if present </param>
		internal virtual void SetExtra0(sbyte[] extra, bool doZIP64)
		{
			if (extra != null)
			{
				if (extra.Length > 0xFFFF)
				{
					throw new IllegalArgumentException("invalid extra field length");
				}
				// extra fields are in "HeaderID(2)DataSize(2)Data... format
				int off = 0;
				int len = extra.Length;
				while (off + 4 < len)
				{
					int tag = get16(extra, off);
					int sz = get16(extra, off + 2);
					off += 4;
					if (off + sz > len) // invalid data
					{
						break;
					}
					switch (tag)
					{
					case EXTID_ZIP64:
						if (doZIP64)
						{
							// LOC extra zip64 entry MUST include BOTH original
							// and compressed file size fields.
							// If invalid zip64 extra fields, simply skip. Even
							// it's rare, it's possible the entry size happens to
							// be the magic value and it "accidently" has some
							// bytes in extra match the id.
							if (sz >= 16)
							{
								Size_Renamed = get64(extra, off);
								Csize = get64(extra, off + 8);
							}
						}
						break;
					case EXTID_NTFS:
						if (sz < 32) // reserved  4 bytes + tag 2 bytes + size 2 bytes
						{
							break; // m[a|c]time 24 bytes
						}
						int pos = off + 4; // reserved 4 bytes
						if (get16(extra, pos) != 0x0001 || get16(extra, pos + 2) != 24)
						{
							break;
						}
						Mtime = winTimeToFileTime(get64(extra, pos + 4));
						Atime = winTimeToFileTime(get64(extra, pos + 12));
						Ctime = winTimeToFileTime(get64(extra, pos + 20));
						break;
					case EXTID_EXTT:
						int flag = Byte.ToUnsignedInt(extra[off]);
						int sz0 = 1;
						// The CEN-header extra field contains the modification
						// time only, or no timestamp at all. 'sz' is used to
						// flag its presence or absence. But if mtime is present
						// in LOC it must be present in CEN as well.
						if ((flag & 0x1) != 0 && (sz0 + 4) <= sz)
						{
							Mtime = unixTimeToFileTime(get32(extra, off + sz0));
							sz0 += 4;
						}
						if ((flag & 0x2) != 0 && (sz0 + 4) <= sz)
						{
							Atime = unixTimeToFileTime(get32(extra, off + sz0));
							sz0 += 4;
						}
						if ((flag & 0x4) != 0 && (sz0 + 4) <= sz)
						{
							Ctime = unixTimeToFileTime(get32(extra, off + sz0));
							sz0 += 4;
						}
						break;
					 default:
				 break;
					}
					off += sz;
				}
			}
			this.Extra_Renamed = extra;
		}


		/// <summary>
		/// Sets the optional comment string for the entry.
		/// 
		/// <para>ZIP entry comments have maximum length of 0xffff. If the length of the
		/// specified comment string is greater than 0xFFFF bytes after encoding, only
		/// the first 0xFFFF bytes are output to the ZIP file entry.
		/// 
		/// </para>
		/// </summary>
		/// <param name="comment"> the comment string
		/// </param>
		/// <seealso cref= #getComment() </seealso>
		public virtual String Comment
		{
			set
			{
				this.Comment_Renamed = value;
			}
			get
			{
				return Comment_Renamed;
			}
		}


		/// <summary>
		/// Returns true if this is a directory entry. A directory entry is
		/// defined to be one whose name ends with a '/'. </summary>
		/// <returns> true if this is a directory entry </returns>
		public virtual bool Directory
		{
			get
			{
				return Name_Renamed.EndsWith("/");
			}
		}

		/// <summary>
		/// Returns a string representation of the ZIP entry.
		/// </summary>
		public override String ToString()
		{
			return Name;
		}

		/// <summary>
		/// Returns the hash code value for this entry.
		/// </summary>
		public override int HashCode()
		{
			return Name_Renamed.HashCode();
		}

		/// <summary>
		/// Returns a copy of this entry.
		/// </summary>
		public virtual Object Clone()
		{
			try
			{
				ZipEntry e = (ZipEntry)base.Clone();
				e.Extra_Renamed = (Extra_Renamed == null) ? null : Extra_Renamed.clone();
				return e;
			}
			catch (CloneNotSupportedException e)
			{
				// This should never happen, since we are Cloneable
				throw new InternalError(e);
			}
		}
	}

}