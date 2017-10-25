using System.Collections.Generic;
using System.Runtime.InteropServices;

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
	/// This class is used to read entries from a zip file.
	/// 
	/// <para> Unless otherwise noted, passing a <tt>null</tt> argument to a constructor
	/// or method in this class will cause a <seealso cref="NullPointerException"/> to be
	/// thrown.
	/// 
	/// @author      David Connelly
	/// </para>
	/// </summary>
	public class ZipFile : ZipConstants, Closeable
	{
		private long Jzfile; // address of jzfile data
		private readonly String Name_Renamed; // zip file name
		private readonly int Total; // total number of entries
		private readonly bool Locsig; // if zip file starts with LOCSIG (usually true)
		private volatile bool CloseRequested = false;

		private const int STORED = ZipEntry.STORED;
		private const int DEFLATED = ZipEntry.DEFLATED;

		/// <summary>
		/// Mode flag to open a zip file for reading.
		/// </summary>
		public const int OPEN_READ = 0x1;

		/// <summary>
		/// Mode flag to open a zip file and mark it for deletion.  The file will be
		/// deleted some time between the moment that it is opened and the moment
		/// that it is closed, but its contents will remain accessible via the
		/// <tt>ZipFile</tt> object until either the close method is invoked or the
		/// virtual machine exits.
		/// </summary>
		public const int OPEN_DELETE = 0x4;

		static ZipFile()
		{
			/* Zip library is loaded from System.initializeSystemClass */
			initIDs();
			// A system prpperty to disable mmap use to avoid vm crash when
			// in-use zip file is accidently overwritten by others.
			String prop = sun.misc.VM.getSavedProperty("sun.zip.disableMemoryMapping");
			Usemmap = (prop == null || !(prop.Length() == 0 || prop.EqualsIgnoreCase("true")));
			sun.misc.SharedSecrets.setJavaUtilZipFileAccess(new JavaUtilZipFileAccessAnonymousInnerClassHelper()
		   );
		}

		private class JavaUtilZipFileAccessAnonymousInnerClassHelper : sun.misc.JavaUtilZipFileAccess
		{
			public JavaUtilZipFileAccessAnonymousInnerClassHelper()
			{
			}

			public virtual bool StartsWithLocHeader(ZipFile zip)
			{
				return zip.StartsWithLocHeader();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		private static readonly bool Usemmap;


		/// <summary>
		/// Opens a zip file for reading.
		/// 
		/// <para>First, if there is a security manager, its <code>checkRead</code>
		/// method is called with the <code>name</code> argument as its argument
		/// to ensure the read is allowed.
		/// 
		/// </para>
		/// <para>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used to
		/// decode the entry names and comments.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the name of the zip file </param>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if a security manager exists and its
		///         <code>checkRead</code> method doesn't allow read access to the file.
		/// </exception>
		/// <seealso cref= SecurityManager#checkRead(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ZipFile(String name) throws java.io.IOException
		public ZipFile(String name) : this(new File(name), OPEN_READ)
		{
		}

		/// <summary>
		/// Opens a new <code>ZipFile</code> to read from the specified
		/// <code>File</code> object in the specified mode.  The mode argument
		/// must be either <tt>OPEN_READ</tt> or <tt>OPEN_READ | OPEN_DELETE</tt>.
		/// 
		/// <para>First, if there is a security manager, its <code>checkRead</code>
		/// method is called with the <code>name</code> argument as its argument to
		/// ensure the read is allowed.
		/// 
		/// </para>
		/// <para>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used to
		/// decode the entry names and comments
		/// 
		/// </para>
		/// </summary>
		/// <param name="file"> the ZIP file to be opened for reading </param>
		/// <param name="mode"> the mode in which the file is to be opened </param>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException"> if a security manager exists and
		///         its <code>checkRead</code> method
		///         doesn't allow read access to the file,
		///         or its <code>checkDelete</code> method doesn't allow deleting
		///         the file when the <tt>OPEN_DELETE</tt> flag is set. </exception>
		/// <exception cref="IllegalArgumentException"> if the <tt>mode</tt> argument is invalid </exception>
		/// <seealso cref= SecurityManager#checkRead(java.lang.String)
		/// @since 1.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ZipFile(java.io.File file, int mode) throws java.io.IOException
		public ZipFile(File file, int mode) : this(file, mode, StandardCharsets.UTF_8)
		{
		}

		/// <summary>
		/// Opens a ZIP file for reading given the specified File object.
		/// 
		/// <para>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used to
		/// decode the entry names and comments.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file"> the ZIP file to be opened for reading </param>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ZipFile(java.io.File file) throws ZipException, java.io.IOException
		public ZipFile(File file) : this(file, OPEN_READ)
		{
		}

		private ZipCoder Zc;

		/// <summary>
		/// Opens a new <code>ZipFile</code> to read from the specified
		/// <code>File</code> object in the specified mode.  The mode argument
		/// must be either <tt>OPEN_READ</tt> or <tt>OPEN_READ | OPEN_DELETE</tt>.
		/// 
		/// <para>First, if there is a security manager, its <code>checkRead</code>
		/// method is called with the <code>name</code> argument as its argument to
		/// ensure the read is allowed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="file"> the ZIP file to be opened for reading </param>
		/// <param name="mode"> the mode in which the file is to be opened </param>
		/// <param name="charset">
		///        the <seealso cref="java.nio.charset.Charset charset"/> to
		///        be used to decode the ZIP entry name and comment that are not
		///        encoded by using UTF-8 encoding (indicated by entry's general
		///        purpose flag).
		/// </param>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred
		/// </exception>
		/// <exception cref="SecurityException">
		///         if a security manager exists and its <code>checkRead</code>
		///         method doesn't allow read access to the file,or its
		///         <code>checkDelete</code> method doesn't allow deleting the
		///         file when the <tt>OPEN_DELETE</tt> flag is set
		/// </exception>
		/// <exception cref="IllegalArgumentException"> if the <tt>mode</tt> argument is invalid
		/// </exception>
		/// <seealso cref= SecurityManager#checkRead(java.lang.String)
		/// 
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ZipFile(java.io.File file, int mode, java.nio.charset.Charset charset) throws java.io.IOException
		public ZipFile(File file, int mode, Charset charset)
		{
			if (((mode & OPEN_READ) == 0) || ((mode & ~(OPEN_READ | OPEN_DELETE)) != 0))
			{
				throw new IllegalArgumentException("Illegal mode: 0x" + mode.ToString("x"));
			}
			String name = file.Path;
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.CheckRead(name);
				if ((mode & OPEN_DELETE) != 0)
				{
					sm.CheckDelete(name);
				}
			}
			if (charset == null)
			{
				throw new NullPointerException("charset is null");
			}
			this.Zc = ZipCoder.Get(charset);
			long t0 = System.nanoTime();
			Jzfile = open(name, mode, file.LastModified(), Usemmap);
			sun.misc.PerfCounter.ZipFileOpenTime.addElapsedTimeFrom(t0);
			sun.misc.PerfCounter.ZipFileCount.increment();
			this.Name_Renamed = name;
			this.Total = getTotal(Jzfile);
			this.Locsig = startsWithLOC(Jzfile);
		}

		/// <summary>
		/// Opens a zip file for reading.
		/// 
		/// <para>First, if there is a security manager, its <code>checkRead</code>
		/// method is called with the <code>name</code> argument as its argument
		/// to ensure the read is allowed.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> the name of the zip file </param>
		/// <param name="charset">
		///        the <seealso cref="java.nio.charset.Charset charset"/> to
		///        be used to decode the ZIP entry name and comment that are not
		///        encoded by using UTF-8 encoding (indicated by entry's general
		///        purpose flag).
		/// </param>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="SecurityException">
		///         if a security manager exists and its <code>checkRead</code>
		///         method doesn't allow read access to the file
		/// </exception>
		/// <seealso cref= SecurityManager#checkRead(java.lang.String)
		/// 
		/// @since 1.7 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ZipFile(String name, java.nio.charset.Charset charset) throws java.io.IOException
		public ZipFile(String name, Charset charset) : this(new File(name), OPEN_READ, charset)
		{
		}

		/// <summary>
		/// Opens a ZIP file for reading given the specified File object. </summary>
		/// <param name="file"> the ZIP file to be opened for reading </param>
		/// <param name="charset">
		///        The <seealso cref="java.nio.charset.Charset charset"/> to be
		///        used to decode the ZIP entry name and comment (ignored if
		///        the <a href="package-summary.html#lang_encoding"> language
		///        encoding bit</a> of the ZIP entry's general purpose bit
		///        flag is set).
		/// </param>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred
		/// 
		/// @since 1.7 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ZipFile(java.io.File file, java.nio.charset.Charset charset) throws java.io.IOException
		public ZipFile(File file, Charset charset) : this(file, OPEN_READ, charset)
		{
		}

		/// <summary>
		/// Returns the zip file comment, or null if none.
		/// </summary>
		/// <returns> the comment string for the zip file, or null if none
		/// </returns>
		/// <exception cref="IllegalStateException"> if the zip file has been closed
		/// 
		/// Since 1.7 </exception>
		public virtual String Comment
		{
			get
			{
				lock (this)
				{
					EnsureOpen();
					sbyte[] bcomm = getCommentBytes(Jzfile);
					if (bcomm == null)
					{
						return null;
					}
					return Zc.ToString(bcomm, bcomm.Length);
				}
			}
		}

		/// <summary>
		/// Returns the zip file entry for the specified name, or null
		/// if not found.
		/// </summary>
		/// <param name="name"> the name of the entry </param>
		/// <returns> the zip file entry, or null if not found </returns>
		/// <exception cref="IllegalStateException"> if the zip file has been closed </exception>
		public virtual ZipEntry GetEntry(String name)
		{
			if (name == null)
			{
				throw new NullPointerException("name");
			}
			long jzentry = 0;
			lock (this)
			{
				EnsureOpen();
				jzentry = getEntry(Jzfile, Zc.GetBytes(name), true);
				if (jzentry != 0)
				{
					ZipEntry ze = GetZipEntry(name, jzentry);
					freeEntry(Jzfile, jzentry);
					return ze;
				}
			}
			return null;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long getEntry(long jzfile, sbyte[] name, bool addSlash);

		// freeEntry releases the C jzentry struct.
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void freeEntry(long jzfile, long jzentry);

		// the outstanding inputstreams that need to be closed,
		// mapped to the inflater objects they use.
		private readonly IDictionary<InputStream, Inflater> Streams = new WeakHashMap<InputStream, Inflater>();

		/// <summary>
		/// Returns an input stream for reading the contents of the specified
		/// zip file entry.
		/// 
		/// <para> Closing this ZIP file will, in turn, close all input
		/// streams that have been returned by invocations of this method.
		/// 
		/// </para>
		/// </summary>
		/// <param name="entry"> the zip file entry </param>
		/// <returns> the input stream for reading the contents of the specified
		/// zip file entry. </returns>
		/// <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <exception cref="IllegalStateException"> if the zip file has been closed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream getInputStream(ZipEntry entry) throws java.io.IOException
		public virtual InputStream GetInputStream(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new NullPointerException("entry");
			}
			long jzentry = 0;
			ZipFileInputStream @in = null;
			lock (this)
			{
				EnsureOpen();
				if (!Zc.UTF8 && (entry.Flag & EFS) != 0)
				{
					jzentry = getEntry(Jzfile, Zc.GetBytesUTF8(entry.Name_Renamed), false);
				}
				else
				{
					jzentry = getEntry(Jzfile, Zc.GetBytes(entry.Name_Renamed), false);
				}
				if (jzentry == 0)
				{
					return null;
				}
				@in = new ZipFileInputStream(this, jzentry);

				switch (getEntryMethod(jzentry))
				{
				case STORED:
					lock (Streams)
					{
						Streams[@in] = null;
					}
					return @in;
				case DEFLATED:
					// MORE: Compute good size for inflater stream:
					long size = getEntrySize(jzentry) + 2; // Inflater likes a bit of slack
					if (size > 65536)
					{
						size = 8192;
					}
					if (size <= 0)
					{
						size = 4096;
					}
					Inflater inf = Inflater;
					InputStream @is = new ZipFileInflaterInputStream(this, @in, inf, (int)size);
					lock (Streams)
					{
						Streams[@is] = inf;
					}
					return @is;
				default:
					throw new ZipException("invalid compression method");
				}
			}
		}

		private class ZipFileInflaterInputStream : InflaterInputStream
		{
			private readonly ZipFile OuterInstance;

			internal volatile bool CloseRequested = false;
			internal bool Eof = false;
			internal readonly ZipFileInputStream Zfin;

			internal ZipFileInflaterInputStream(ZipFile outerInstance, ZipFileInputStream zfin, Inflater inf, int size) : base(zfin, inf, size)
			{
				this.OuterInstance = outerInstance;
				this.Zfin = zfin;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
			public override void Close()
			{
				if (CloseRequested)
				{
					return;
				}
				CloseRequested = true;

				base.Close();
				Inflater inf;
				lock (outerInstance.Streams)
				{
					inf = outerInstance.Streams.Remove(this);
				}
				if (inf != null)
				{
					outerInstance.ReleaseInflater(inf);
				}
			}

			// Override fill() method to provide an extra "dummy" byte
			// at the end of the input stream. This is required when
			// using the "nowrap" Inflater option.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void fill() throws java.io.IOException
			protected internal override void Fill()
			{
				if (Eof)
				{
					throw new EOFException("Unexpected end of ZLIB input stream");
				}
				Len = @in.Read(Buf, 0, Buf.Length);
				if (Len == -1)
				{
					Buf[0] = 0;
					Len = 1;
					Eof = true;
				}
				Inf.SetInput(Buf, 0, Len);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int available() throws java.io.IOException
			public override int Available()
			{
				if (CloseRequested)
				{
					return 0;
				}
				long avail = Zfin.Size() - Inf.BytesWritten;
				return (avail > (long) Integer.MaxValue ? Integer.MaxValue : (int) avail);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws Throwable
			~ZipFileInflaterInputStream()
			{
				Close();
			}
		}

		/*
		 * Gets an inflater from the list of available inflaters or allocates
		 * a new one.
		 */
		private Inflater Inflater
		{
			get
			{
				Inflater inf;
				lock (InflaterCache)
				{
					while (null != (inf = InflaterCache.Poll()))
					{
						if (false == inf.Ended())
						{
							return inf;
						}
					}
				}
				return new Inflater(true);
			}
		}

		/*
		 * Releases the specified inflater to the list of available inflaters.
		 */
		private void ReleaseInflater(Inflater inf)
		{
			if (false == inf.Ended())
			{
				inf.Reset();
				lock (InflaterCache)
				{
					InflaterCache.Add(inf);
				}
			}
		}

		// List of available Inflater objects for decompression
		private Deque<Inflater> InflaterCache = new ArrayDeque<Inflater>();

		/// <summary>
		/// Returns the path name of the ZIP file. </summary>
		/// <returns> the path name of the ZIP file </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		private class ZipEntryIterator : Iterator<ZipEntry>, Iterator<ZipEntry>
		{
			private readonly ZipFile OuterInstance;

			internal int i = 0;

			public ZipEntryIterator(ZipFile outerInstance)
			{
				this.OuterInstance = outerInstance;
				outerInstance.EnsureOpen();
			}

			public virtual bool HasMoreElements()
			{
				return HasNext();
			}

			public virtual bool HasNext()
			{
				lock (OuterInstance)
				{
					outerInstance.EnsureOpen();
					return i < outerInstance.Total;
				}
			}

			public virtual ZipEntry NextElement()
			{
				return Next();
			}

			public virtual ZipEntry Next()
			{
				lock (OuterInstance)
				{
					outerInstance.EnsureOpen();
					if (i >= outerInstance.Total)
					{
						throw new NoSuchElementException();
					}
					long jzentry = getNextEntry(outerInstance.Jzfile, i++);
					if (jzentry == 0)
					{
						String message;
						if (outerInstance.CloseRequested)
						{
							message = "ZipFile concurrently closed";
						}
						else
						{
							message = getZipMessage(OuterInstance.Jzfile);
						}
						throw new ZipError("jzentry == 0" + ",\n jzfile = " + OuterInstance.Jzfile + ",\n total = " + OuterInstance.Total + ",\n name = " + OuterInstance.Name_Renamed + ",\n i = " + i + ",\n message = " + message);
					}
					ZipEntry ze = outerInstance.GetZipEntry(null, jzentry);
					freeEntry(outerInstance.Jzfile, jzentry);
					return ze;
				}
			}
		}

		/// <summary>
		/// Returns an enumeration of the ZIP file entries. </summary>
		/// <returns> an enumeration of the ZIP file entries </returns>
		/// <exception cref="IllegalStateException"> if the zip file has been closed </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.Iterator<? extends ZipEntry> entries()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.Iterator<? extends ZipEntry> entries()
		public virtual IEnumerator<?> Entries() where ? : ZipEntry
		{
			return new ZipEntryIterator(this);
		}

		/// <summary>
		/// Return an ordered {@code Stream} over the ZIP file entries.
		/// Entries appear in the {@code Stream} in the order they appear in
		/// the central directory of the ZIP file.
		/// </summary>
		/// <returns> an ordered {@code Stream} of entries in this ZIP file </returns>
		/// <exception cref="IllegalStateException"> if the zip file has been closed
		/// @since 1.8 </exception>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.stream.Stream<? extends ZipEntry> stream()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.stream.Stream<? extends ZipEntry> stream()
		public virtual Stream<?> Stream() where ? : ZipEntry
		{
			return StreamSupport.Stream(Spliterators.Spliterator(new ZipEntryIterator(this), Size(), java.util.Spliterator_Fields.ORDERED | java.util.Spliterator_Fields.DISTINCT | java.util.Spliterator_Fields.IMMUTABLE | java.util.Spliterator_Fields.NONNULL), false);
		}

		private ZipEntry GetZipEntry(String name, long jzentry)
		{
			ZipEntry e = new ZipEntry();
			e.Flag = getEntryFlag(jzentry); // get the flag first
			if (name != null)
			{
				e.Name_Renamed = name;
			}
			else
			{
				sbyte[] bname = getEntryBytes(jzentry, JZENTRY_NAME);
				if (!Zc.UTF8 && (e.Flag & EFS) != 0)
				{
					e.Name_Renamed = Zc.ToStringUTF8(bname, bname.Length);
				}
				else
				{
					e.Name_Renamed = Zc.ToString(bname, bname.Length);
				}
			}
			e.Xdostime = getEntryTime(jzentry);
			e.Crc_Renamed = getEntryCrc(jzentry);
			e.Size_Renamed = getEntrySize(jzentry);
			e.Csize = getEntryCSize(jzentry);
			e.Method_Renamed = getEntryMethod(jzentry);
			e.SetExtra0(getEntryBytes(jzentry, JZENTRY_EXTRA), false);
			sbyte[] bcomm = getEntryBytes(jzentry, JZENTRY_COMMENT);
			if (bcomm == null)
			{
				e.Comment_Renamed = null;
			}
			else
			{
				if (!Zc.UTF8 && (e.Flag & EFS) != 0)
				{
					e.Comment_Renamed = Zc.ToStringUTF8(bcomm, bcomm.Length);
				}
				else
				{
					e.Comment_Renamed = Zc.ToString(bcomm, bcomm.Length);
				}
			}
			return e;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long getNextEntry(long jzfile, int i);

		/// <summary>
		/// Returns the number of entries in the ZIP file. </summary>
		/// <returns> the number of entries in the ZIP file </returns>
		/// <exception cref="IllegalStateException"> if the zip file has been closed </exception>
		public virtual int Size()
		{
			EnsureOpen();
			return Total;
		}

		/// <summary>
		/// Closes the ZIP file.
		/// <para> Closing this ZIP file will close all of the input streams
		/// previously returned by invocations of the {@link #getInputStream
		/// getInputStream} method.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public virtual void Close()
		{
			if (CloseRequested)
			{
				return;
			}
			CloseRequested = true;

			lock (this)
			{
				// Close streams, release their inflaters
				lock (Streams)
				{
					if (false == Streams.Count == 0)
					{
						IDictionary<InputStream, Inflater> copy = new Dictionary<InputStream, Inflater>(Streams);
						Streams.Clear();
						foreach (java.util.Map_Entry<InputStream, Inflater> e in copy)
						{
							e.Key.Close();
							Inflater inf = e.Value;
							if (inf != null)
							{
								inf.End();
							}
						}
					}
				}

				// Release cached inflaters
				Inflater inf;
				lock (InflaterCache)
				{
					while (null != (inf = InflaterCache.Poll()))
					{
						inf.End();
					}
				}

				if (Jzfile != 0)
				{
					// Close the zip file
					long zf = this.Jzfile;
					Jzfile = 0;

					close(zf);
				}
			}
		}

		/// <summary>
		/// Ensures that the system resources held by this ZipFile object are
		/// released when there are no more references to it.
		/// 
		/// <para>
		/// Since the time when GC would invoke this method is undetermined,
		/// it is strongly recommended that applications invoke the <code>close</code>
		/// method as soon they have finished accessing this <code>ZipFile</code>.
		/// This will prevent holding up system resources for an undetermined
		/// length of time.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException"> if an I/O error has occurred </exception>
		/// <seealso cref=    java.util.zip.ZipFile#close() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws java.io.IOException
		~ZipFile()
		{
			Close();
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void close(long jzfile);

		private void EnsureOpen()
		{
			if (CloseRequested)
			{
				throw new IllegalStateException("zip file closed");
			}

			if (Jzfile == 0)
			{
				throw new IllegalStateException("The object is not initialized.");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureOpenOrZipException() throws java.io.IOException
		private void EnsureOpenOrZipException()
		{
			if (CloseRequested)
			{
				throw new ZipException("ZipFile closed");
			}
		}

		/*
		 * Inner class implementing the input stream used to read a
		 * (possibly compressed) zip file entry.
		 */
	   private class ZipFileInputStream : InputStream
	   {
		   private readonly ZipFile OuterInstance;

			internal volatile bool CloseRequested = false;
			protected internal long Jzentry; // address of jzentry data
			internal long Pos; // current position within entry data
			protected internal long Rem; // number of remaining bytes within entry
			protected internal long Size_Renamed; // uncompressed size of this entry

			internal ZipFileInputStream(ZipFile outerInstance, long jzentry)
			{
				this.OuterInstance = outerInstance;
				Pos = 0;
				Rem = getEntryCSize(jzentry);
				Size_Renamed = getEntrySize(jzentry);
				this.Jzentry = jzentry;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read(byte b[] , int off, int len) throws java.io.IOException
			public override int Read(sbyte[] b, int off, int len)
			{
				lock (OuterInstance)
				{
					long rem = this.Rem;
					long pos = this.Pos;
					if (rem == 0)
					{
						return -1;
					}
					if (len <= 0)
					{
						return 0;
					}
					if (len > rem)
					{
						len = (int) rem;
					}

					outerInstance.EnsureOpenOrZipException();
					len = ZipFile.read(OuterInstance.Jzfile, Jzentry, pos, b, off, len);
					if (len > 0)
					{
						this.Pos = (pos + len);
						this.Rem = (rem - len);
					}
				}
				if (Rem == 0)
				{
					Close();
				}
				return len;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int read() throws java.io.IOException
			public override int Read()
			{
				sbyte[] b = new sbyte[1];
				if (Read(b, 0, 1) == 1)
				{
					return b[0] & 0xff;
				}
				else
				{
					return -1;
				}
			}

			public override long Skip(long n)
			{
				if (n > Rem)
				{
					n = Rem;
				}
				Pos += n;
				Rem -= n;
				if (Rem == 0)
				{
					Close();
				}
				return n;
			}

			public override int Available()
			{
				return Rem > Integer.MaxValue ? Integer.MaxValue : (int) Rem;
			}

			public virtual long Size()
			{
				return Size_Renamed;
			}

			public override void Close()
			{
				if (CloseRequested)
				{
					return;
				}
				CloseRequested = true;

				Rem = 0;
				lock (OuterInstance)
				{
					if (Jzentry != 0 && OuterInstance.Jzfile != 0)
					{
						freeEntry(OuterInstance.Jzfile, Jzentry);
						Jzentry = 0;
					}
				}
				lock (outerInstance.Streams)
				{
					outerInstance.Streams.Remove(this);
				}
			}

			~ZipFileInputStream()
			{
				Close();
			}
	   }


		/// <summary>
		/// Returns {@code true} if, and only if, the zip file begins with {@code
		/// LOCSIG}.
		/// </summary>
		private bool StartsWithLocHeader()
		{
			return Locsig;
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long open(String name, int mode, long lastModified, bool usemmap);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int getTotal(long jzfile);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern boolean startsWithLOC(long jzfile);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int read(long jzfile, long jzentry, long pos, sbyte[] b, int off, int len);

		// access to the native zentry object
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long getEntryTime(long jzentry);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long getEntryCrc(long jzentry);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long getEntryCSize(long jzentry);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern long getEntrySize(long jzentry);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int getEntryMethod(long jzentry);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int getEntryFlag(long jzentry);
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern byte[] getCommentBytes(long jzfile);

		private const int JZENTRY_NAME = 0;
		private const int JZENTRY_EXTRA = 1;
		private const int JZENTRY_COMMENT = 2;
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern byte[] getEntryBytes(long jzentry, int type);

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern String getZipMessage(long jzfile);
	}

}