using System;

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
	/// This class implements an input stream filter for reading files in the
	/// ZIP file format. Includes support for both compressed and uncompressed
	/// entries.
	/// 
	/// @author      David Connelly
	/// </summary>
	public class ZipInputStream : InflaterInputStream, ZipConstants
	{
		private ZipEntry Entry;
		private int Flag;
		private CRC32 Crc = new CRC32();
		private long Remaining;
		private sbyte[] Tmpbuf = new sbyte[512];

		private const int STORED = ZipEntry.STORED;
		private const int DEFLATED = ZipEntry.DEFLATED;

		private bool Closed = false;
		// this flag is set to true after EOF has reached for
		// one entry
		private bool EntryEOF = false;

		private ZipCoder Zc;

		/// <summary>
		/// Check to make sure that this stream has not been closed
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
		/// Creates a new ZIP input stream.
		/// 
		/// <para>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used to
		/// decode the entry names.
		/// 
		/// </para>
		/// </summary>
		/// <param name="in"> the actual input stream </param>
		public ZipInputStream(InputStream @in) : this(@in, StandardCharsets.UTF_8)
		{
		}

		/// <summary>
		/// Creates a new ZIP input stream.
		/// </summary>
		/// <param name="in"> the actual input stream
		/// </param>
		/// <param name="charset">
		///        The <seealso cref="java.nio.charset.Charset charset"/> to be
		///        used to decode the ZIP entry name (ignored if the
		///        <a href="package-summary.html#lang_encoding"> language
		///        encoding bit</a> of the ZIP entry's general purpose bit
		///        flag is set).
		/// 
		/// @since 1.7 </param>
		public ZipInputStream(InputStream @in, Charset charset) : base(new PushbackInputStream(@in, 512), new Inflater(true), 512)
		{
			UsesDefaultInflater = true;
			if (@in == null)
			{
				throw new NullPointerException("in is null");
			}
			if (charset == null)
			{
				throw new NullPointerException("charset is null");
			}
			this.Zc = ZipCoder.Get(charset);
		}

		/// <summary>
		/// Reads the next ZIP file entry and positions the stream at the
		/// beginning of the entry data. </summary>
		/// <returns> the next ZIP file entry, or null if there are no more entries </returns>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ZipEntry getNextEntry() throws java.io.IOException
		public virtual ZipEntry NextEntry
		{
			get
			{
				EnsureOpen();
				if (Entry != null)
				{
					CloseEntry();
				}
				Crc.Reset();
				Inf.Reset();
				if ((Entry = ReadLOC()) == null)
				{
					return null;
				}
				if (Entry.Method_Renamed == STORED)
				{
					Remaining = Entry.Size_Renamed;
				}
				EntryEOF = false;
				return Entry;
			}
		}

		/// <summary>
		/// Closes the current ZIP entry and positions the stream for reading the
		/// next entry. </summary>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeEntry() throws java.io.IOException
		public virtual void CloseEntry()
		{
			EnsureOpen();
			while (Read(Tmpbuf, 0, Tmpbuf.Length) != -1);
			EntryEOF = true;
		}

		/// <summary>
		/// Returns 0 after EOF has reached for the current entry data,
		/// otherwise always return 1.
		/// <para>
		/// Programs should not count on this method to return the actual number
		/// of bytes that could be read without blocking.
		/// 
		/// </para>
		/// </summary>
		/// <returns>     1 before EOF and 0 after EOF has reached for current entry. </returns>
		/// <exception cref="IOException">  if an I/O error occurs.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws java.io.IOException
		public override int Available()
		{
			EnsureOpen();
			if (EntryEOF)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}

		/// <summary>
		/// Reads from the current ZIP entry into an array of bytes.
		/// If <code>len</code> is not zero, the method
		/// blocks until some input is available; otherwise, no
		/// bytes are read and <code>0</code> is returned. </summary>
		/// <param name="b"> the buffer into which the data is read </param>
		/// <param name="off"> the start offset in the destination array <code>b</code> </param>
		/// <param name="len"> the maximum number of bytes read </param>
		/// <returns> the actual number of bytes read, or -1 if the end of the
		///         entry is reached </returns>
		/// <exception cref="NullPointerException"> if <code>b</code> is <code>null</code>. </exception>
		/// <exception cref="IndexOutOfBoundsException"> if <code>off</code> is negative,
		/// <code>len</code> is negative, or <code>len</code> is greater than
		/// <code>b.length - off</code> </exception>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte[] b, int off, int len) throws java.io.IOException
		public override int Read(sbyte[] b, int off, int len)
		{
			EnsureOpen();
			if (off < 0 || len < 0 || off > b.Length - len)
			{
				throw new IndexOutOfBoundsException();
			}
			else if (len == 0)
			{
				return 0;
			}

			if (Entry == null)
			{
				return -1;
			}
			switch (Entry.Method_Renamed)
			{
			case DEFLATED:
				len = base.Read(b, off, len);
				if (len == -1)
				{
					ReadEnd(Entry);
					EntryEOF = true;
					Entry = null;
				}
				else
				{
					Crc.Update(b, off, len);
				}
				return len;
			case STORED:
				if (Remaining <= 0)
				{
					EntryEOF = true;
					Entry = null;
					return -1;
				}
				if (len > Remaining)
				{
					len = (int)Remaining;
				}
				len = @in.Read(b, off, len);
				if (len == -1)
				{
					throw new ZipException("unexpected EOF");
				}
				Crc.Update(b, off, len);
				Remaining -= len;
				if (Remaining == 0 && Entry.Crc_Renamed != Crc.Value)
				{
					throw new ZipException("invalid entry CRC (expected 0x" + Entry.Crc_Renamed.ToString("x") + " but got 0x" + Crc.Value.ToString("x") + ")");
				}
				return len;
			default:
				throw new ZipException("invalid compression method");
			}
		}

		/// <summary>
		/// Skips specified number of bytes in the current ZIP entry. </summary>
		/// <param name="n"> the number of bytes to skip </param>
		/// <returns> the actual number of bytes skipped </returns>
		/// <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="IllegalArgumentException"> if {@code n < 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long skip(long n) throws java.io.IOException
		public override long Skip(long n)
		{
			if (n < 0)
			{
				throw new IllegalArgumentException("negative skip length");
			}
			EnsureOpen();
			int max = (int)System.Math.Min(n, Integer.MaxValue);
			int total = 0;
			while (total < max)
			{
				int len = max - total;
				if (len > Tmpbuf.Length)
				{
					len = Tmpbuf.Length;
				}
				len = Read(Tmpbuf, 0, len);
				if (len == -1)
				{
					EntryEOF = true;
					break;
				}
				total += len;
			}
			return total;
		}

		/// <summary>
		/// Closes this input stream and releases any system resources associated
		/// with the stream. </summary>
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

		private sbyte[] b = new sbyte[256];

		/*
		 * Reads local file (LOC) header for next entry.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ZipEntry readLOC() throws java.io.IOException
		private ZipEntry ReadLOC()
		{
			try
			{
				ReadFully(Tmpbuf, 0, ZipConstants_Fields.LOCHDR);
			}
			catch (EOFException)
			{
				return null;
			}
			if (get32(Tmpbuf, 0) != ZipConstants_Fields.LOCSIG)
			{
				return null;
			}
			// get flag first, we need check EFS.
			Flag = get16(Tmpbuf, ZipConstants_Fields.LOCFLG);
			// get the entry name and create the ZipEntry first
			int len = get16(Tmpbuf, ZipConstants_Fields.LOCNAM);
			int blen = b.Length;
			if (len > blen)
			{
				do
				{
					blen = blen * 2;
				} while (len > blen);
				b = new sbyte[blen];
			}
			ReadFully(b, 0, len);
			// Force to use UTF-8 if the EFS bit is ON, even the cs is NOT UTF-8
			ZipEntry e = CreateZipEntry(((Flag & EFS) != 0) ? Zc.ToStringUTF8(b, len) : Zc.ToString(b, len));
			// now get the remaining fields for the entry
			if ((Flag & 1) == 1)
			{
				throw new ZipException("encrypted ZIP entry not supported");
			}
			e.Method_Renamed = get16(Tmpbuf, ZipConstants_Fields.LOCHOW);
			e.Xdostime = get32(Tmpbuf, ZipConstants_Fields.LOCTIM);
			if ((Flag & 8) == 8)
			{
				/* "Data Descriptor" present */
				if (e.Method_Renamed != DEFLATED)
				{
					throw new ZipException("only DEFLATED entries can have EXT descriptor");
				}
			}
			else
			{
				e.Crc_Renamed = get32(Tmpbuf, ZipConstants_Fields.LOCCRC);
				e.Csize = get32(Tmpbuf, ZipConstants_Fields.LOCSIZ);
				e.Size_Renamed = get32(Tmpbuf, ZipConstants_Fields.LOCLEN);
			}
			len = get16(Tmpbuf, ZipConstants_Fields.LOCEXT);
			if (len > 0)
			{
				sbyte[] extra = new sbyte[len];
				ReadFully(extra, 0, len);
				e.SetExtra0(extra, e.Csize == ZIP64_MAGICVAL || e.Size_Renamed == ZIP64_MAGICVAL);
			}
			return e;
		}

		/// <summary>
		/// Creates a new <code>ZipEntry</code> object for the specified
		/// entry name.
		/// </summary>
		/// <param name="name"> the ZIP file entry name </param>
		/// <returns> the ZipEntry just created </returns>
		protected internal virtual ZipEntry CreateZipEntry(String name)
		{
			return new ZipEntry(name);
		}

		/*
		 * Reads end of deflated entry as well as EXT descriptor if present.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readEnd(ZipEntry e) throws java.io.IOException
		private void ReadEnd(ZipEntry e)
		{
			int n = Inf.Remaining;
			if (n > 0)
			{
				((PushbackInputStream)@in).Unread(Buf, Len - n, n);
			}
			if ((Flag & 8) == 8)
			{
				/* "Data Descriptor" present */
				if (Inf.BytesWritten > ZIP64_MAGICVAL || Inf.BytesRead > ZIP64_MAGICVAL)
				{
					// ZIP64 format
					ReadFully(Tmpbuf, 0, ZIP64_EXTHDR);
					long sig = get32(Tmpbuf, 0);
					if (sig != ZipConstants_Fields.EXTSIG) // no EXTSIG present
					{
						e.Crc_Renamed = sig;
						e.Csize = get64(Tmpbuf, ZIP64_EXTSIZ - ZIP64_EXTCRC);
						e.Size_Renamed = get64(Tmpbuf, ZIP64_EXTLEN - ZIP64_EXTCRC);
						((PushbackInputStream)@in).Unread(Tmpbuf, ZIP64_EXTHDR - ZIP64_EXTCRC - 1, ZIP64_EXTCRC);
					}
					else
					{
						e.Crc_Renamed = get32(Tmpbuf, ZIP64_EXTCRC);
						e.Csize = get64(Tmpbuf, ZIP64_EXTSIZ);
						e.Size_Renamed = get64(Tmpbuf, ZIP64_EXTLEN);
					}
				}
				else
				{
					ReadFully(Tmpbuf, 0, ZipConstants_Fields.EXTHDR);
					long sig = get32(Tmpbuf, 0);
					if (sig != ZipConstants_Fields.EXTSIG) // no EXTSIG present
					{
						e.Crc_Renamed = sig;
						e.Csize = get32(Tmpbuf, ZipConstants_Fields.EXTSIZ - ZipConstants_Fields.EXTCRC);
						e.Size_Renamed = get32(Tmpbuf, ZipConstants_Fields.EXTLEN - ZipConstants_Fields.EXTCRC);
						((PushbackInputStream)@in).Unread(Tmpbuf, ZipConstants_Fields.EXTHDR - ZipConstants_Fields.EXTCRC - 1, ZipConstants_Fields.EXTCRC);
					}
					else
					{
						e.Crc_Renamed = get32(Tmpbuf, ZipConstants_Fields.EXTCRC);
						e.Csize = get32(Tmpbuf, ZipConstants_Fields.EXTSIZ);
						e.Size_Renamed = get32(Tmpbuf, ZipConstants_Fields.EXTLEN);
					}
				}
			}
			if (e.Size_Renamed != Inf.BytesWritten)
			{
				throw new ZipException("invalid entry size (expected " + e.Size_Renamed + " but got " + Inf.BytesWritten + " bytes)");
			}
			if (e.Csize != Inf.BytesRead)
			{
				throw new ZipException("invalid entry compressed size (expected " + e.Csize + " but got " + Inf.BytesRead + " bytes)");
			}
			if (e.Crc_Renamed != Crc.Value)
			{
				throw new ZipException("invalid entry CRC (expected 0x" + e.Crc_Renamed.ToString("x") + " but got 0x" + Crc.Value.ToString("x") + ")");
			}
		}

		/*
		 * Reads bytes, blocking until all bytes are read.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readFully(byte[] b, int off, int len) throws java.io.IOException
		private void ReadFully(sbyte[] b, int off, int len)
		{
			while (len > 0)
			{
				int n = @in.Read(b, off, len);
				if (n == -1)
				{
					throw new EOFException();
				}
				off += n;
				len -= n;
			}
		}

	}

}